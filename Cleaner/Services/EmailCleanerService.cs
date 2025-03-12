
using Cleaner.Handler;
using Cleaner.Models;
using MimeKit;

public class EmailCleanerService(MessagePublisher messagePublisher) : IEmailCleanerService
{
    private readonly string _FilePath = "Data/maildir/";
    public async Task<IEnumerable<ProcessedEmailDto>> CleanEmailsAsync()
    {
        string[] emails = Directory.GetFiles(_FilePath, "*", SearchOption.AllDirectories);
        var emailPaths = emails.Select(email => Path.GetFullPath(email)).Take(15);
        var emailCleaningTasks = emailPaths.Select(emailPath => CleanEmailAsync(emailPath));
        
        var processedEmails = await Task.WhenAll(emailCleaningTasks);
        return processedEmails;
    }

    public async Task<ProcessedEmailDto> CleanEmailAsync(string path)
    {
        try
        {
            var content = await MimeMessage.LoadAsync(path);
            
            var from = content.From.ToString();
            var to = content.To.ToString();
            var fileName = Path.GetFileName(path);
            var cleanedFileName = fileName.Substring(0, fileName.Length - 1);
            cleanedFileName += ".txt";
            string body = content.TextBody;
            
            return new ProcessedEmailDto
            {
                EmailName = cleanedFileName,
                EmailFrom = from,
                EmailTo = to,
                EmailContent = body
            };
        }
        catch (Exception e)
        {
            Console.WriteLine("Error processing " + path + ": " + e.Message);
            return null;
        }
    }
    
    public async Task PublishEmailsAsync(IEnumerable<ProcessedEmailDto> cleanedEmails)
    {
       await Parallel.ForEachAsync(cleanedEmails, async (cleanedEmail, CancellationToken) =>
       {
           Console.WriteLine("Publishing email: " + cleanedEmail.EmailName);
           await messagePublisher.PublishCleanedEmail(cleanedEmail);
       });
    }
}