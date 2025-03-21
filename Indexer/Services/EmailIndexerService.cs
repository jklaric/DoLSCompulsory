﻿using Shared.Models;
using System.Text.RegularExpressions;
using Monitoring;
using Serilog;

namespace Indexer.Services
{
    public class EmailIndexerService
    {
        private readonly IndexRepository _indexRepository;

        
        public EmailIndexerService(IndexRepository indexRepository)
        {
            _indexRepository = indexRepository ?? throw new ArgumentNullException(nameof(indexRepository));
        }
        
        public async Task IndexEmail(ProcessedEmailDto cleanEmail)
        {
            using var activity = MonitoringService.ActivitySource.StartActivity("IndexEmail");  
            var words = ExtractWords(cleanEmail.EmailContent);
          
            var wordOccurrences = words
                .GroupBy(w => w)  
                .Select(g => new { Word = g.Key, Count = g.Count() })
                .ToList();
            
            Log.Logger.Information($"Processing file: {cleanEmail.EmailName}");
            foreach (var wordOccurrence in wordOccurrences)
            {
                var wordData = new WordDto()
                {
                    Word = wordOccurrence.Word,
                    Count = wordOccurrence.Count
                };
                
                await _indexRepository.IndexEmail(cleanEmail, wordData);
            }
            
        }
        
        private static string[] ExtractWords(string content)
        {
            using var activity = MonitoringService.ActivitySource.StartActivity("ExtractWords");
            var matches = Regex.Matches(content.ToLower(), @"\b\w+\b");
            return matches.Select(m => m.Value).ToArray();
        }
    }
}