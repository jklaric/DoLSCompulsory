using Monitoring;
using Npgsql;
using Serilog;
using WebApi.DTOs;

namespace WebApi.Repository;

public class SearchRepository(NpgsqlDataSource _dataSource) : ISearchRepository
{
    public async Task<List<SearchResultDto>> SearchByTermAsync(string searchTerm)
    {
        using var activity = MonitoringService.ActivitySource.StartActivity("RepositorySearchByTerm");
        
        var query = @"
        SELECT e.emailid, e.emailname, e.emailcontent, o.count AS occurrence_count
        FROM emails e
        JOIN occurrences o ON e.emailid = o.emailid
        JOIN words w ON o.wordid = w.wordid
        WHERE w.wordvalue = @searchTerm
        ORDER BY o.count DESC;
    ";

        try
        {
            await using var conn = await _dataSource.OpenConnectionAsync();
            await using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("searchTerm", searchTerm);

            var results = new List<SearchResultDto>();

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(new SearchResultDto
                {
                    EmailId = reader.GetInt32(0),
                    EmailName = reader.GetString(1),
                    EmailContent = reader.GetString(2),
                    OccurrenceCount = reader.GetInt32(3)
                });
            }

            return results;
        }
        catch (Exception e)
        {
            Log.Logger.Error("An error occurred while searching by term: {Error}", e.Message);
            throw;
        }
    }

}