using Serilog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CMS.Data;
using CMS.Infrastructure.Notifications.Repositories;
using CMS.Infrastructure.Notifications.NotificationServices;
using CMS.Application.Notifications.Interfaces;
using CMS.Application.Notifications.Services;
using CMS.Domain.NotificationModels.Configuration;
using CMS.Data.Seeders;
using CMS.Application.Shared.Configuration;
using CMS.Application.Auth.Interfaces;
using CMS.Application.Auth.Services;
using CMS.Application.Auth.DTOs.Mapping;
using CMS.Infrastructure.Auth.Repositories;
using CMS.Infrastructure.Auth.Services;
using DotNetEnv;

// Load .env so environment variables are available for configuration
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// DB setup - Read from environment variable or appsettings.json
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string is required. Set CONNECTION_STRING environment variable or ConnectionStrings:DefaultConnection in appsettings.json");

// Log effective configuration for local debugging
Console.WriteLine($"[Startup] Effective ConnectionString: {connectionString}");
var aspnetcore_urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
Console.WriteLine($"[Startup] ASPNETCORE_URLS env var: {aspnetcore_urls}");
Console.WriteLine($"[Startup] ASPNETCORE_ENVIRONMENT: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure JWT Settings - Read from environment variables or appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT Secret Key is required. Set JWT_SECRET_KEY environment variable or JwtSettings:SecretKey in appsettings.json");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
    ?? jwtSettings["Issuer"]
    ?? "CMS_API";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
    ?? jwtSettings["Audience"]
    ?? "CMS_Client";

builder.Services.Configure<JwtSettings>(options =>
{
    options.SecretKey = jwtSecretKey;
    options.Issuer = jwtIssuer;
    options.Audience = jwtAudience;
    options.AccessTokenExpirationMinutes = int.Parse(
        Environment.GetEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRATION_MINUTES")
        ?? jwtSettings["AccessTokenExpirationMinutes"]
        ?? "60");
    options.RefreshTokenExpirationDays = int.Parse(
        Environment.GetEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRATION_DAYS")
        ?? jwtSettings["RefreshTokenExpirationDays"]
        ?? "7");
});

// Setup JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// AutoMapper
builder.Services.AddAutoMapper(typeof(AuthMappingProfile));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, CMS.Infrastructure.Auth.Services.SendGridEmailService>();
builder.Services.AddScoped<IVerificationCodeRepository, CMS.Infrastructure.Auth.Repositories.VerificationCodeRepository>();
builder.Services.AddScoped<IInvitationRepository, CMS.Infrastructure.Auth.Repositories.InvitationRepository>();

// Register Services
builder.Services.AddScoped<IJwtService, CMS.Infrastructure.Auth.Services.JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// All entities now use the single CmsDbContext

// Configure SendGrid from environment variables
builder.Services.Configure<SendGridConfig>(options =>
{
    options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? string.Empty;
    options.FromEmail = Environment.GetEnvironmentVariable("SENDGRID_FROM_EMAIL") ?? string.Empty;
    options.FromName = Environment.GetEnvironmentVariable("SENDGRID_FROM_NAME") ?? "CMS";
});

// Configure Twilio from environment variables
builder.Services.Configure<TwilioConfig>(options =>
{
    options.AccountSid = Environment.GetEnvironmentVariable("AccountSid") ?? string.Empty;
    options.AuthToken = Environment.GetEnvironmentVariable("AuthToken") ?? string.Empty;
    options.FromNumber = Environment.GetEnvironmentVariable("FromNumber") ?? string.Empty;
});

// Notification repositories
builder.Services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationQueueRepository, NotificationQueueRepository>();
builder.Services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

// Notification services
builder.Services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
builder.Services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();
builder.Services.AddScoped<INotificationScheduler, NotificationScheduler>();
builder.Services.AddScoped<INotificationSender, CMS.Infrastructure.Notifications.NotificationServices.NotificationSender>();

// Notification sender implementation used by scheduler
builder.Services.AddScoped<CMS.Application.Notifications.Services.INotificationSender, CMS.Infrastructure.Notifications.NotificationServices.NotificationSender>();

// Notification providers (concrete registration) - use fully-qualified types to avoid ambiguity
builder.Services.AddScoped<CMS.Infrastructure.Notifications.NotificationServices.SendGridEmailService>();
builder.Services.AddScoped<CMS.Infrastructure.Notifications.NotificationServices.TwilioSmsService>();

// Template notification service that needs both email and sms providers
builder.Services.AddScoped<ITemplateNotificationService>(sp =>
{
    var templateService = sp.GetRequiredService<INotificationTemplateService>();
    var emailService = sp.GetRequiredService<CMS.Infrastructure.Notifications.NotificationServices.SendGridEmailService>();
    var smsService = sp.GetRequiredService<CMS.Infrastructure.Notifications.NotificationServices.TwilioSmsService>();
    var logger = sp.GetRequiredService<ILogger<CMS.Infrastructure.Notifications.NotificationServices.NotificationService>>();
    
    return new CMS.Infrastructure.Notifications.NotificationServices.NotificationService(
        templateService, emailService, smsService, logger);
});

// JSON settings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
});

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger configuration with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CMS API",
        Version = "v1",
        Description = "Clinic Management System API"
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });


});

// Config CORS - Read from appsettings.json or use defaults
var corsSettings = builder.Configuration.GetSection("CorsSettings");
var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:4200", "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // In development allow any origin so browser receives CORS headers even on errors.
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            return;
        }

        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply the named CORS policy early in the pipeline so all responses include CORS headers.
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Register custom middlewares
app.UseMiddleware<CMS.Api.Middleware.RequestCorrelationMiddleware>();
app.UseMiddleware<CMS.Api.Middleware.GlobalExceptionMiddleware>();
app.UseMiddleware<CMS.Api.Middleware.ResponseWrapperMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<CmsDbContext>();
        var configuration = services.GetRequiredService<IConfiguration>();
        await DatabaseSeeder.SeedAsync(context, configuration);
        Log.Information("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while seeding the database");
    }
}

app.Run();
