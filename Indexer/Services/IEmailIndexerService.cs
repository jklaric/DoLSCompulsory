using Shared.Models;

namespace Indexer.Services;

public interface IEmailIndexerService
{
    public Task IndexEmail(ProcessedEmailDto cleanEmail);
}