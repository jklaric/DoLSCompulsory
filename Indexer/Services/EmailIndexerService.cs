using Shared.Models;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Indexer.Services
{
    public class EmailIndexerService : IEmailIndexerService

    {
    public async Task IndexEmail(ProcessedEmailDto cleanEmail)
    {

        var words = ExtractWords(cleanEmail.EmailContent);


        var wordOccurrences = words
            .GroupBy(w => w)
            .Select(g => new { Word = g.Key, Count = g.Count() })
            .ToList();


        Console.WriteLine($"Processing email: {cleanEmail.EmailName}");
        foreach (var wordOccurrence in wordOccurrences)
        {
            Console.WriteLine($"Word: {wordOccurrence.Word}, Count: {wordOccurrence.Count}");
        }
        //TODO - Implement the logic to store the word occurrences in a database
    }

    // Utility method to extract words using a regex pattern
    private static string[] ExtractWords(string content)
    {
        var matches = Regex.Matches(content.ToLower(), @"\b\w+\b");
        return matches.Select(m => m.Value).ToArray();
    }
    }
}