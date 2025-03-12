using Cleaner.Models;

public interface IEmailCleanerService
{
    Task CleanEmailsAsync(string _FilePath);
    Task CleanEmailAsync(string path);
}