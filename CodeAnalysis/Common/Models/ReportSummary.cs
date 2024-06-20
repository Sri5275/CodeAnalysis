namespace CodeAnalysis.Common.Models
{
    public class ReportSummary
    {
        public string id { get; set; }
        public string user_id { get; set; }
        public string repo_name { get; set; }
        public DateTime timestamp { get; set; }
        public int avgScore { get; set; }
        public bool criticalErrors { get; set; }
    }
}
