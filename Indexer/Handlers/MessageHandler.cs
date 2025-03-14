using EasyNetQ;
using Indexer.Services;
using Microsoft.Extensions.Hosting;
using Shared.Models;
namespace Indexer.Handlers;

    public class MessageHandler(IBus bus, EmailIndexerService emailIndexerService) : BackgroundService 
    {
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           
            try
            {
                Console.WriteLine("Subscribing to CleanEmail");
                await bus.PubSub.SubscribeAsync<ProcessedEmailDto>("CleanEmail", async msg =>
                {
                    await HandleCleanEmail(msg);
                });
                
                Console.WriteLine("Subscribed to CleanEmail");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured: " + e);
                throw;
            }
            
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        
            Console.WriteLine("Message handler is stopping..");
        }
       
        public async Task HandleCleanEmail(ProcessedEmailDto message)
        {
            Console.WriteLine("Handling clean email");
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
                Console.WriteLine("Error occurred: " + e);
                throw;
            }
           
        }
    }
