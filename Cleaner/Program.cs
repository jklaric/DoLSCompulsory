using Monitoring;
using EasyNetQ;
using EasyNetQ.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

//RabbitMQ Setup
var rabbitmqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitmqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitmqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitmqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";
var rabbitmqUri =  $"amqp://{rabbitmqUser}:{rabbitmqPass}@{rabbitmqHost}:{rabbitmqPort}/";
Console.WriteLine($"RabbitMQ URI: {rabbitmqUri}");

//Monitoring and Tracing Setup

var loggerUrl = Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341";
var zipkinUrl = Environment.GetEnvironmentVariable("ZIPKIN_URL") ?? "http://localhost:9411/api/v2/spans"; 
    
MonitoringService.SetupSerilog(loggerUrl);
MonitoringService.SetupTracing(zipkinUrl);

builder.Services.AddSingleton(RabbitHutch.CreateBus(rabbitmqUri));

//Add services
builder.Services.AddSingleton<MessagePublisher>();

using IHost host = builder.Build();

host.Start();

using (MonitoringService.ActivitySource.StartActivity("CleaningEmails"))
{
    var messagePublisher = host.Services.GetRequiredService<MessagePublisher>();
    var emailCleanerService = new EmailCleanerService(messagePublisher);

    var envDataPath = Environment.GetEnvironmentVariable("DATA_PATH") ?? "../data";
    await emailCleanerService.CleanEmailsAsync(envDataPath);

}

await host.WaitForShutdownAsync();
Console.WriteLine("Shutting down cleaner microservice");    
await MonitoringService.Dispose();