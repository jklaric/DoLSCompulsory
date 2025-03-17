using EasyNetQ;
using Indexer.Services;
using Microsoft.Extensions.Hosting;
using Monitoring;
using Serilog;
using Shared.Models;
namespace Indexer.Handlers;

    public class MessageHandler(IBus bus, EmailIndexerService emailIndexerService) : BackgroundService 
    {
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var activity = MonitoringService.ActivitySource.StartActivity("MessageHandler");
           
            try
            {
                Log.Logger.Information("Subscribing to CleanEmail");
                await bus.PubSub.SubscribeAsync<ProcessedEmailDto>("CleanEmail", async msg =>
                {
                    await HandleCleanEmail(msg);
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
       
        public async Task HandleCleanEmail(ProcessedEmailDto message)
        {
            using var activity = MonitoringService.ActivitySource.StartActivity("HandleCleanEmail");
            Log.Logger.Information("Handling clean email");
            try
            {
               
                var cleanEmail = new ProcessedEmailDto()
                {
                    EmailName = message.EmailName,
                    EmailContent = message.EmailContent
                };
                
               await emailIndexerService.IndexEmail(cleanEmail);
            }
            catch (Exception e)
            {
                Log.Logger.Error($"Error occured: {e}");
                throw;
            }
           
        }
    }
