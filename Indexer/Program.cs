using EasyNetQ;
using Indexer.DbContext;
using Indexer.Handlers;
using Indexer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Monitoring;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Configure PostgreSQL connection
var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost";
var postgresPort = Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432";
var postgresUser = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "user";
var postgresPass = Environment.GetEnvironmentVariable("POSTGRES_PASS") ?? "pass";
var postgresDb = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "db";

var connectionString = $"Host={postgresHost};Port={postgresPort};Username={postgresUser};Password={postgresPass};Database={postgresDb}";

// Register DbContext with the connection string
builder.Services.AddDbContext<DoLsDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure RabbitMQ connection
var rabbitmqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitmqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitmqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitmqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";
var rabbitmqUri =  $"amqp://{rabbitmqUser}:{rabbitmqPass}@{rabbitmqHost}:{rabbitmqPort}/";

//Monitoring and Tracing Setup

var loggerUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var zipkinUrl = Environment.GetEnvironmentVariable("ZIPKIN_URL") ?? "http://localhost:9411/api/v2/spans"; 
    
MonitoringService.SetupSerilog(loggerUrl);
MonitoringService.SetupTracing(zipkinUrl);


Console.WriteLine("Using RabbitMQ URI: " + rabbitmqUri);
builder.Services.AddSingleton(RabbitHutch.CreateBus(rabbitmqUri));
builder.Services.AddHostedService<MessageHandler>();

// Register services
builder.Services.AddSingleton<IndexRepository>(provider => new IndexRepository(connectionString));
builder.Services.AddSingleton<EmailIndexerService>(provider => new EmailIndexerService(provider.GetRequiredService<IndexRepository>()));

using IHost host = builder.Build();

// Apply pending migrations at runtime
await ApplyDatabaseMigrations(host);

// Start the host
host.Start();

await host.WaitForShutdownAsync();
Console.WriteLine("Shutting down indexer microservice");

// Method to apply pending migrations
async Task ApplyDatabaseMigrations(IHost host)
{
    using (var scope = host.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<DoLsDbContext>();

        try
        {
            // Apply any pending migrations at startup
            Console.WriteLine("Applying database migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
        }
    }
}
