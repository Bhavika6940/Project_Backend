using Microsoft.EntityFrameworkCore;
using Project.Data;
using System.Text.Json;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Project.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Configure dependency tracking
builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
{
    module.EnableSqlCommandTextInstrumentation = true;
});

// Configure Azure Storage
var azureStorageSection = builder.Configuration.GetSection("AzureStorage");
if (!azureStorageSection.Exists())
{
    throw new InvalidOperationException("AzureStorage configuration section is missing!");
}

builder.Services.Configure<AzureStorageConfig>(azureStorageSection);
builder.Services.AddOptions<AzureStorageConfig>()
    .Bind(azureStorageSection)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.WithOrigins("http://localhost:3000")          // exact origin — no trailing slash
              .AllowAnyHeader()                    // or specify if you want to tighten
              .AllowAnyMethod()                    // GET, POST, etc.
              .AllowCredentials();                 // 👈 sets Access-Control-Allow-Credentials:true
    });
});



//builder.Services.AddCors(opt =>
//{
//    opt.AddPolicy("ReactDev",
//        p => p.WithOrigins("http://localhost:3000")   // React dev server
//              .AllowAnyHeader()
//              .AllowAnyMethod());
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors("ReactDev");
app.UseCors("DevCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
