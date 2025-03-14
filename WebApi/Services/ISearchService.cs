using WebApi.DTOs;

namespace WebApi.Services;

public interface ISearchService
{
    public Task<List<SearchResultDto>> SearchByTermAsync(string term);
}