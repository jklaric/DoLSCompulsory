using EasyNetQ;
using Indexer.Handlers;
using Indexer.Services;
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
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddSingleton<IEmailIndexerService>(provider => new EmailIndexerService());



using IHost host = builder.Build();

host.Start();


await host.WaitForShutdownAsync();
Console.WriteLine("Shutting down indexer microservice");
