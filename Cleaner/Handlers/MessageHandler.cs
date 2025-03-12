using Cleaner.Models;
using EasyNetQ;
using Microsoft.Extensions.Hosting;

namespace Cleaner.Handler;

public class MessageHandler() : BackgroundService 
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Starting message handler");
        
        var bus = RabbitHutch.CreateBus("host=localhost");

        await bus.PubSub.SubscribeAsync<ProcessedEmailDto>("CleanEmail", async msg =>
        {
            await Task.CompletedTask;
        }).AsTask(); 
        
        Console.WriteLine("Subscribed to CleanEmail");
        
        while (!stoppingToken.IsCancellationRequested)
        {
           await Task.Delay(1000, stoppingToken);
        }
        
        Console.WriteLine("Stopping message handler");
    }
}