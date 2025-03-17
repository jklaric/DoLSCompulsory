using Shared.Models;
using EasyNetQ;
using Monitoring;
using Serilog;

public class MessagePublisher(IBus _bus)
{
    public async Task PublishCleanedEmail(ProcessedEmailDto cleanedEmail)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("PublishEmail");
        

            int maxRetries = 3;
            int retryDelay = 2000;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Log.Logger.Information($"Publishing email: {cleanedEmail.EmailName}");
                    await _bus.PubSub.PublishAsync(cleanedEmail, msg => msg.WithTopic("CleanEmail"));
                    break;
                }
                catch (TaskCanceledException ex)
                {
                    Log.Logger.Error($"Task was canceled: {ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error($"Error publishing email: {ex.Message}");
                    if (attempt == maxRetries)
                    {
                        Log.Logger.Error($"Max retries reached: {ex.Message}");
                        break;
                    }

                    await Task.Delay(retryDelay * attempt);
                }
            }
        }
    }