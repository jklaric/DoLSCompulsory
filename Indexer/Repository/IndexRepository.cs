using Npgsql;
using Shared.Models;
using System.Threading;
using Dapper;
using Monitoring;
using Serilog;

public class IndexRepository
{
    private readonly string _connectionString;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(20);

    public IndexRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task IndexEmail(ProcessedEmailDto cleanEmail, WordDto wordData)
{
    using var activity = MonitoringService.ActivitySource.StartActivity("RepositoryIndexEmail");
    int? wordId = 0;
    var emailId = 0;

    await _semaphore.WaitAsync();
    try
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var insertEmailSql = @"
            WITH ins AS (
                INSERT INTO Emails (EmailName, EmailContent)
                VALUES (@EmailName, @EmailContent)
                ON CONFLICT (EmailName) DO NOTHING
                RETURNING EmailId
            )
            SELECT EmailId FROM ins
            UNION ALL
            SELECT EmailId FROM Emails WHERE EmailName = @EmailName;";

        emailId = await connection.ExecuteScalarAsync<int>(insertEmailSql, new { cleanEmail.EmailName, cleanEmail.EmailContent });

        if (emailId == 0)
        {
            Log.Logger.Error($"Failed to retrieve EmailId for {cleanEmail.EmailName}");
        }

        var insertOrGetWordSql = @"
    WITH ins AS (
        INSERT INTO words (wordvalue)
        VALUES (@Word)
        ON CONFLICT (wordvalue) DO UPDATE SET wordvalue = words.wordvalue
        RETURNING wordid
    )
    SELECT wordid FROM ins
    UNION ALL
    SELECT wordid FROM words WHERE wordvalue = @Word LIMIT 1;";

        wordId = await connection.ExecuteScalarAsync<int?>(insertOrGetWordSql, new { Word = wordData.Word });

        if (wordId == null || wordId == 0)
        {
            Log.Logger.Error($"Failed to retrieve WordId for {wordData.Word}");
            throw new Exception($"WordId is NULL for word: {wordData.Word}");
        }

        Log.Logger.Information($"Retrieved WordId: {wordId} for word: {wordData.Word}");

        var insertOccurrenceSql = @"
    INSERT INTO Occurrences (WordId, EmailId, Count)
    VALUES (@WordId, @EmailId, @Count);";

        await connection.ExecuteAsync(insertOccurrenceSql, new { WordId = wordId, EmailId = emailId, Count = wordData.Count });

        Log.Logger.Information($"Inserted occurrence for WordId {wordId}, EmailId {emailId}, Count {wordData.Count}");

    }
    catch (Exception e)
    {
        Log.Logger.Error("Error: " + e.Message);
        throw;
    }
    finally
    {
        _semaphore.Release();
    }
}

}
