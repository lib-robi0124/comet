namespace Comet.ViewModels.Auction
{
    public class CategoryFilterVM
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public bool IsSelected { get; set; }
        public string DisplayText => $"{Name} ({Count})";
    }
}
