using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Monitoring;
using Serilog;
using Shared.Models;

namespace Cleaner.Handlers;

public class MessageHandler(IBus bus): BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("MessageHandler");
           
        try
        {
            Log.Logger.Information("Subscribing to CleanEmail");
            await bus.PubSub.SubscribeAsync<ProcessedEmailDto>("CleanEmail", async msg =>
            {
                Log.Logger.Information("message recieved {msg}", msg);
            });
        }
        catch (Exception e)
        {
            Log.Logger.Error($"Error occurred: {e}");
            throw;
        }
            
            
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
        
        Log.Logger.Information("Message handler is stopping..");
    }
}