using Cleaner.Models;
using MimeKit;

public class EmailCleanerService(MessagePublisher messagePublisher) : IEmailCleanerService
{
    public async Task CleanEmailsAsync(string filePath)
    {
        Console.WriteLine("Cleaning emails on path " + Directory.GetParent(filePath));
        string[] emails = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories);
        var emailPaths = emails.Select(Path.GetFullPath);
        var emailCleaningTasks = emailPaths.Select(emailPath => CleanEmailAsync(emailPath));

        try
        {
            await Task.WhenAll(emailCleaningTasks);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task CleanEmailAsync(string path)
    {
        try
        {
            var content = await MimeMessage.LoadAsync(path);
            
            var fileName = Path.GetFileName(path);
            var cleanedFileName = Directory.GetParent(path).FullName +"/" + fileName.Substring(0, fileName.Length - 1);
            cleanedFileName += ".txt";
            string body = content.TextBody;

            Console.WriteLine("Cleaning email: " + cleanedFileName);
            
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