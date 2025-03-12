using Cleaner.Handler;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1.Cmp;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(RabbitHutch.CreateBus("host=localhost"));
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddSingleton<MessagePublisher>();

using IHost host = builder.Build();

host.Start();

var messagePublisher = host.Services.GetRequiredService<MessagePublisher>();
var emailCleanerService = new EmailCleanerService(messagePublisher);


var processedEmails = await emailCleanerService.CleanEmailsAsync();
await emailCleanerService.PublishEmailsAsync(processedEmails);

await host.WaitForShutdownAsync();
