using Npgsql;
using Shared.Models;
using System.Threading;
using Dapper;

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
            throw new Exception($"Failed to retrieve EmailId for {cleanEmail.EmailName}");
        }

        var insertOrGetWordSql = @"
    WITH ins AS (
        INSERT INTO words (wordvalue)
        VALUES (@Word)
        ON CONFLICT (wordvalue) DO NOTHING
        RETURNING wordid
    )
    SELECT wordid FROM ins
    UNION ALL
    SELECT wordid FROM words WHERE wordvalue = @Word LIMIT 1;";

        wordId = await connection.ExecuteScalarAsync<int?>(insertOrGetWordSql, new { Word = wordData.Word });

        if (wordId == null || wordId == 0)
        {
            throw new Exception($"Failed to retrieve WordId for {wordData.Word}");
        }


        var insertOccurrenceSql = @"
            INSERT INTO Occurrences (WordId, EmailId, Count)
            VALUES (@WordId, @EmailId, @Count);";

        await connection.ExecuteAsync(insertOccurrenceSql, new { WordId = wordId, EmailId = emailId, Count = wordData.Count });

        Console.WriteLine($"Inserted occurrence for WordId {wordId}, EmailId {emailId}, Count {wordData.Count}");
    }
    catch (Exception e)
    {
        Console.WriteLine("Error: " + e.Message);
        throw;
    }
    finally
    {
        _semaphore.Release();
    }
}

}
