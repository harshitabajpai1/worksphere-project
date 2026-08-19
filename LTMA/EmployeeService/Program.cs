using System.Text;
using EmployeeService.Data;
using EmployeeService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add controller support.
builder.Services.AddControllers();

// Connect EmployeeService to its own database.
builder.Services.AddDbContext<EmployeeDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration
                .GetConnectionString("EmployeeConnection")));

// EmployeeService uses this client to create authentication users
// through AuthService without accessing WorkSphereAuthDB directly.
builder.Services.AddHttpClient("AuthService", client =>
{
    var authServiceBaseUrl =
        builder.Configuration["AuthService:BaseUrl"];

    if (string.IsNullOrWhiteSpace(authServiceBaseUrl))
    {
        throw new InvalidOperationException(
            "AuthService:BaseUrl configuration is required.");
    }

    client.BaseAddress = new Uri(authServiceBaseUrl);
});

// Makes the incoming SuperAdmin JWT available for forwarding to AuthService.
builder.Services.AddHttpContextAccessor();

// Register department business logic.
builder.Services.AddScoped<
    IDepartmentService,
    DepartmentService>();

// Register employee business logic.
builder.Services.AddScoped<
    IEmployeeService,
    EmployeeService.Services.EmployeeService>();

// Read JWT settings from configuration.
var jwtKey =
    builder.Configuration["Jwt:Key"];

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    builder.Configuration["Jwt:Audience"];

// Configure JWT authentication.
//
// EmployeeService does not create the JWT.
// AuthService creates it.
//
// EmployeeService only validates the JWT
// sent with the request.
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,

                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtKey!))
            };
    });

// Enable [Authorize] and role-based authorization.
builder.Services.AddAuthorization();

// Swagger.
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

    options.AddSecurityRequirement(document =>
        new()
        {
            [new OpenApiSecuritySchemeReference("Bearer", document, null)] = new List<string>()
        });
});

var app = builder.Build();

// Swagger.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS.
app.UseHttpsRedirection();

// Authentication must come before authorization.
app.UseAuthentication();

app.UseAuthorization();

// Map controllers.
app.MapControllers();

app.Run();
