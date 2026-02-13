namespace Comet.ViewModels.Admin
{
    public class AdminDashboardVM
    {
        public int TotalProducts { get; set; }
        public int PublishedProducts { get; set; }
        public int UnpublishedProducts => TotalProducts - PublishedProducts;
        public List<ImportHistoryVM> RecentUploads { get; set; } = new();
        public SystemHealthVM SystemHealth { get; set; } = new();
    }
}
