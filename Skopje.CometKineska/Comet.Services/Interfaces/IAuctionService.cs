using Comet.Services.Implementations;
using Comet.ViewModels.Models;

namespace Comet.Services.Interfaces
{
    public interface IAuctionService
    {
        Task<BidResult> PlaceBidAsync(BidVM bidViewModel);
        Task<IEnumerable<BidVM>> GetProductBidsAsync(int productId);
        Task<decimal> GetCurrentHighestBidAsync(int productId);
        Task<bool> UpdateWinningBidAsync(int productId);
    }
}
