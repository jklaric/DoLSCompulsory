
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

var rabbitmqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitmqPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitmqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitmqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";
var rabbitmqUri =  $"amqp://{rabbitmqUser}:{rabbitmqPass}@{rabbitmqHost}:{rabbitmqPort}/";

Console.WriteLine("Using RabbitMQ URI: " + rabbitmqUri);
builder.Services.AddSingleton(RabbitHutch.CreateBus(rabbitmqUri));
builder.Services.AddSingleton<MessagePublisher>();

using IHost host = builder.Build();

host.Start();

var messagePublisher = host.Services.GetRequiredService<MessagePublisher>();
var emailCleanerService = new EmailCleanerService(messagePublisher);

var envDataPath = Environment.GetEnvironmentVariable("DATA_PATH") ?? "../Data/";
await emailCleanerService.CleanEmailsAsync(envDataPath);

await host.WaitForShutdownAsync();
Console.WriteLine("Shutting down cleaner microservice");    