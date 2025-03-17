using Monitoring;
using WebApi.DTOs;
using WebApi.Repository;

namespace WebApi.Services;

public class SearchService(ISearchRepository _repository) : ISearchService
{
    public async Task<List<SearchResultDto>> SearchByTermAsync(string term)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("ServiceSearchByTerm");
        return await _repository.SearchByTermAsync(term);
    }
}