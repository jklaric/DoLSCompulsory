using DotEnv;
using DotNetEnv;
using Monitoring;
using Npgsql;
using WebApi.Repository;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
var postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "user";
var postgresPass = Environment.GetEnvironmentVariable("POSTGRES_PASS") ?? "pass";
var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "db";

var connectionString = $"Host={postgresHost};Port={postgresPort};Username={postgresUser};Password={postgresPass};Database={postgresDb}";

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
await using var dataSource = dataSourceBuilder.Build();
builder.Services.AddSingleton(dataSource);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//Monitoring and Tracing Setup

var loggerUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var zipkinUrl = Environment.GetEnvironmentVariable("ZIPKIN_URL") ?? "http://localhost:9411/api/v2/spans"; 
    
MonitoringService.SetupSerilog(loggerUrl);
MonitoringService.SetupTracing(zipkinUrl);

builder.Services.AddSingleton<ISearchRepository, SearchRepository>();
builder.Services.AddSingleton<ISearchService, SearchService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevCorsPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
