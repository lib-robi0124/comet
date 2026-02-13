namespace Comet.ViewModels.Admin
{
    public class SystemHealthVM
    {
        public string Status { get; set; } = "Healthy";
        public DateTime LastChecked { get; set; }
        public int TotalProducts { get; set; }
        public int ActiveUsers { get; set; }
        public double ResponseTime { get; set; }
        public string DatabaseStatus { get; set; } = "Connected";
    }
}
