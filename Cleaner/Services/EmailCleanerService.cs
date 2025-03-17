
using MimeKit;
using Monitoring;
using Serilog;
using Shared.Models;

public class EmailCleanerService(MessagePublisher messagePublisher) : IEmailCleanerService
{
    public async Task CleanEmailsAsync(string filePath)
    {
        using (MonitoringService.ActivitySource.StartActivity("LoadFiles"))
        {
            string[] emails = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories);
            var emailPaths = emails.Select(Path.GetFullPath);
            var emailCleaningTasks = emailPaths.Select(emailPath => CleanEmailAsync(emailPath));
            Log.Logger.Information($"Cleaning total of {emails.Length} emails on path {Directory.GetParent(filePath)}");
            await Task.WhenAll(emailCleaningTasks);
        }
    }

    public async Task CleanEmailAsync(string path)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("CleanEmail");
            try
            {
                var content = await MimeMessage.LoadAsync(path);
                var fileName = Path.GetFileName(path);
                var cleanedFileName = Directory.GetParent(path).FullName +"/" + fileName.Substring(0, fileName.Length - 1);
                cleanedFileName += ".txt";
                string body = content.TextBody;

                Log.Logger.Information($"Cleaning email: {cleanedFileName}");
            
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var cleanedEmail = new ProcessedEmailDto
                    {
                        EmailName = cleanedFileName,
                        EmailContent = body
                    };
                
                    await messagePublisher.PublishCleanedEmail(cleanedEmail);
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error($"Error processing {path}: {e.Message}");
                throw;
            }
        }
    }
