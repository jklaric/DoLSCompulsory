namespace WebApi.DTOs;


    public class SearchResultDto
    {
        public int EmailId { get; set; }
        public string EmailName { get; set; }
        public string EmailContent { get; set; }
        public int OccurrenceCount { get; set; }
    }

