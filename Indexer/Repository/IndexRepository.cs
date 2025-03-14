using Dapper;
using Npgsql;
using Shared.Models;

public class IndexRepository
{
    private readonly string _connectionString;  

    public IndexRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task IndexEmail(ProcessedEmailDto cleanEmail, WordDto wordData)
    {
        var wordId = 0;
        var emailId = 0;
        
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var insertEmailSql = @"
            INSERT INTO Emails (EmailName, EmailContent)
            SELECT @EmailName, @EmailContent
            WHERE NOT EXISTS (SELECT 1 FROM Emails WHERE EmailName = @EmailName)
            RETURNING EmailId;";

        try
        {
            emailId = await connection.ExecuteScalarAsync<int>(insertEmailSql, new { cleanEmail.EmailName, cleanEmail.EmailContent });
        }
        catch (Exception e)
        {
            Console.WriteLine("Error occurred pushing email: " + e);
            throw;
        }
        
        var insertOrGetWordSql = @"
            INSERT INTO Words (WordValue)
            SELECT @Word
            WHERE NOT EXISTS (SELECT 1 FROM Words WHERE WordValue = @Word)
            RETURNING WordId;";

        try
        {
            wordId = await connection.ExecuteScalarAsync<int>(insertOrGetWordSql, new { Word = wordData.Word });
        }
        catch (Exception e)
        {
            Console.WriteLine("Error occurred pushing word: " + e);
            throw;
        }

        var insertOccurrenceSql = @"
            INSERT INTO Occurrences (WordId, EmailId, Count)
            VALUES (@WordId, @EmailId, @Count);";

        try
        {
            await connection.ExecuteAsync(insertOccurrenceSql, new { WordId = wordId, EmailId = emailId, Count = wordData.Count });
        }
        catch (Exception e)
        {
            Console.WriteLine("Error occurred pushing occurrence: " + e);
            throw;
        }
    }
}
