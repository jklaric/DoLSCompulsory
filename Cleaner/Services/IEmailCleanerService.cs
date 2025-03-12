using Cleaner.Models;

public interface IEmailCleanerService
{
    Task<IEnumerable<ProcessedEmailDto>> CleanEmailsAsync();
    Task<ProcessedEmailDto> CleanEmailAsync(string path);
}