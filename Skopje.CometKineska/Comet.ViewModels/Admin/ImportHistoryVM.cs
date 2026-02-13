namespace Comet.ViewModels.Admin
{
    public class ImportHistoryVM
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }
        public string ImportedBy { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public bool Success { get; set; }
    }
}
