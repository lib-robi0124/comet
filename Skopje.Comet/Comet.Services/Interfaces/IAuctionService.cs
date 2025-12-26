namespace Comet.Services.Interfaces
{
    public interface IAuctionService
    {
        Task PlaceBidAsync(int productId, decimal price);
    }
}
