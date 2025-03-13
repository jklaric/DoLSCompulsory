using Cleaner.Models;
using EasyNetQ;

public class MessagePublisher(IBus _bus)
{
    public async Task PublishCleanedEmail(ProcessedEmailDto cleanedEmail)
    {
        int maxRetries = 3;
        int retryDelay = 2000; // Delay in milliseconds between retries

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Console.WriteLine("Publishing email: " + cleanedEmail.EmailName);
                await _bus.PubSub.PublishAsync(cleanedEmail, msg => msg.WithTopic("CleanEmail"));
                Console.WriteLine("Published email successfully: " + cleanedEmail.EmailName);
                break; // Exit the loop if successful
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine($"Task was canceled: {ex.Message}");
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing email: {ex.Message}");
                if (attempt == maxRetries)
                {
                    Console.WriteLine("Max retries reached, aborting.");
                    break;
                }

                // Wait before retrying
                await Task.Delay(retryDelay * attempt);
            }
        }
    }


}