using Serilog;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

// Load environment variables
DotNetEnv.Env.Load();
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new InvalidOperationException("CONNECTION_STRING environment variable is required");

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// DB setup
builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseSqlServer(connectionString));

//TODO Setup JWT

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

//TODO Http Client

// JSON settings
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString;
});

// Repository Services
builder.Services.AddScoped<CMS.Application.Clinic.Interfaces.IDoctorRepository, CMS.Infrastructure.Clinic.Repositories.DoctorRepository>();
builder.Services.AddScoped<CMS.Application.Clinic.Interfaces.ILeaveRepository, CMS.Infrastructure.Clinic.Repositories.LeaveRepository>();
builder.Services.AddScoped<CMS.Application.Appointments.Interfaces.ITimeSlotRepository, CMS.Infrastructure.Appointments.Repositories.TimeSlotRepository>();

// Application Services
builder.Services.AddScoped<CMS.Application.Appointments.Interfaces.ITimeSlotService, CMS.Application.Appointments.Services.TimeSlotService>();
builder.Services.AddScoped<CMS.Application.Clinic.Interfaces.IDoctorService, CMS.Application.Clinic.Services.DoctorService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Config CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("fe", pol => pol.WithOrigins("http://localhost:4200", "http://localhost:5173")
        .AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//TODO Register CORS
app.UseCors("fe");

// Register custom middlewares
app.UseMiddleware<CMS.Api.Middleware.RequestCorrelationMiddleware>();
app.UseMiddleware<CMS.Api.Middleware.GlobalExceptionMiddleware>();
app.UseMiddleware<CMS.Api.Middleware.ResponseWrapperMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
