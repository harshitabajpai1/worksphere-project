using AuthService.Data;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controller support.
// Controllers will handle requests such as
// POST /api/Auth/login and POST /api/Auth/change-password.
builder.Services.AddControllers();

// Register Entity Framework Core.
//
// AuthDbContext is responsible for communicating
// with our SQL Server database.
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AuthConnection")));

// Register PasswordHasher.
//
// We never store a user's actual password in the database.
// PasswordHasher converts the password into a secure hash
// before we save it.
builder.Services.AddScoped<
    IPasswordHasher<User>,
    PasswordHasher<User>>();

// Register the authentication service.
//
// AuthenticationService contains the business logic
// for login, changing passwords and getting user details.
builder.Services.AddScoped<
    IAuthService,
    AuthenticationService>();

// Register the JWT service.
//
// JwtService is responsible for creating JWT tokens
// after successful login.
builder.Services.AddScoped<
    IJwtService,
    JwtService>();

// Read JWT settings from appsettings.json.
var jwtKey =
    builder.Configuration["Jwt:Key"];

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"];

var jwtAudience =
    builder.Configuration["Jwt:Audience"];

// Configure JWT authentication.
//
// Whenever an endpoint has [Authorize], ASP.NET Core
// will check the JWT token sent by the client.
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // Check that the token was created
                // by our expected issuer.
                ValidateIssuer = true,

                // Check that the token was created
                // for our application.
                ValidateAudience = true,

                // Check that the token has not expired.
                ValidateLifetime = true,

                // Check that the token was signed
                // using our secret key.
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtKey!))
            };
    });

// Enable authorization.
//
// This allows us to use [Authorize] and
// [Authorize(Roles = "SuperAdmin")] on controllers.
builder.Services.AddAuthorization();

// Register services required by Swagger.
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

// Create a service scope so that we can get
// database-related services from Dependency Injection.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Get our database context.
    var context =
        services.GetRequiredService<AuthDbContext>();

    // Get PasswordHasher from Dependency Injection.
    var passwordHasher =
        services.GetRequiredService<
            IPasswordHasher<User>>();

    // Create the initial SuperAdmin if one
    // does not already exist.
    //
    // DbInitializer also applies pending migrations.
    await DbInitializer.InitializeAsync(
        context,
        passwordHasher);
}

// Enable Swagger when running in Development mode.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS.
app.UseHttpsRedirection();

// Authentication checks WHO the user is.
// This must come before authorization.
app.UseAuthentication();

// Authorization checks whether the authenticated
// user is allowed to access the requested resource.
app.UseAuthorization();

// Connect controller routes to the application.
app.MapControllers();

// Start the application.
app.Run();