
using Cleaner.Handler;
using Cleaner.Models;
using MimeKit;

public class EmailCleanerService(MessagePublisher messagePublisher) : IEmailCleanerService
{
    public async Task CleanEmailsAsync(string _FilePath = "../Data/")
    {
        Console.WriteLine("Cleaning emails on path " + _FilePath);
        string[] emails = Directory.GetFiles(_FilePath, "*", SearchOption.AllDirectories);
        var emailPaths = emails.Select(Path.GetFullPath);
        var emailCleaningTasks = emailPaths.Select(emailPath => CleanEmailAsync(emailPath));
        
        await Task.WhenAll(emailCleaningTasks);
    }

    public async Task CleanEmailAsync(string path)
    {
        try
        {
            var content = await MimeMessage.LoadAsync(path);
            
            var fileName = Path.GetFileName(path);
            var cleanedFileName = fileName.Substring(0, fileName.Length - 1);
            cleanedFileName += ".txt";
            string body = content.TextBody;

            if (!string.IsNullOrEmpty(body))
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
            Console.WriteLine("Error processing " + path + ": " + e.Message);
            throw;
        }
    }
}