using Serilog;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using CMS.Application;
using CMS.Infrastructure;
using CMS.Services;

// Load environment variables
DotNetEnv.Env.Load();
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Global handlers to capture unhandled exceptions during runtime
AppDomain.CurrentDomain.UnhandledException += (sender, e) => Log.Fatal(e.ExceptionObject as Exception, "Unhandled exception in AppDomain");
TaskScheduler.UnobservedTaskException += (sender, e) => { Log.Fatal(e.Exception, "Unobserved task exception"); e.SetObserved(); };


// DB setup
builder.Services.AddDbContext<CmsDbContext>(options =>
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        options.UseSqlServer(connectionString);
    }
    else
    {
        // When running in tests or no connection string is provided, use InMemory DB
        options.UseInMemoryDatabase("Cms_InMemory_Db");
    }
});

//Appointments
builder.Services.AddScoped<IPatient, PatientRepository>();
builder.Services.AddScoped<CMS.Application.PatientService>();
builder.Services.AddScoped<CMS.Services.BillPdfService>();

builder.Services.AddDistributedMemoryCache();



//TODO Setup JWT

//Automapper

//TODO Http Client

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
builder.Services.AddSwaggerGen();

// Config CORS
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAngular", pol => pol.WithOrigins("http://localhost:4200/")
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );

});


var app = builder.Build();

app.Lifetime.ApplicationStopped.Register(() => Log.Information("Application stopped at {Time}", DateTime.UtcNow));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//TODO Register CORS
app.UseCors("AllowAngular");


// Register custom middlewares
app.UseMiddleware<CMS.Api.Middleware.RequestCorrelationMiddleware>();
app.UseMiddleware<CMS.Api.Middleware.GlobalExceptionMiddleware>();
// app.UseMiddleware<CMS.Api.Middleware.ResponseWrapperMiddleware>(); // Disabled for payment compatibility

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
