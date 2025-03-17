using WebApi.DTOs;

namespace WebApi.Repository;

public interface ISearchRepository
{
    public Task<List<SearchResultDto>> SearchByTermAsync(string term);
}