using System.Text;
using LeaveService.Data;
using LeaveService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add controller support.
builder.Services.AddControllers();

// Connect LeaveService to its own database.
builder.Services.AddDbContext<LeaveDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LeaveConnection")));

// LeaveService uses this client to get employee information.
builder.Services.AddHttpClient("EmployeeService", client =>
{
    var employeeServiceBaseUrl = builder.Configuration["EmployeeService:BaseUrl"];

    if (string.IsNullOrWhiteSpace(employeeServiceBaseUrl))
    {
        throw new InvalidOperationException("EmployeeService:BaseUrl configuration is required.");
    }

    client.BaseAddress = new Uri(employeeServiceBaseUrl);
});

// Makes the logged-in user's JWT available for forwarding.
builder.Services.AddHttpContextAccessor();

// Register leave business logic.
builder.Services.AddScoped<ILeaveService, LeaveService.Services.LeaveService>();

// Read JWT settings from configuration.
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Configure JWT authentication.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });

// Enable role-based authorization.
builder.Services.AddAuthorization();

// Register Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token here."
    });

    options.AddSecurityRequirement(document => new()
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = new List<string>()
    });
});

var app = builder.Build();

// Enable Swagger in Development mode.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
