using Comet.ViewModels.Auction;
using Comet.ViewModels.Models;

namespace Comet.Services.Interfaces
{
    public interface IAuctionService
    {
        // Bid operations
        Task<BidResult> PlaceBidAsync(BidVM bidViewModel);
        Task<IEnumerable<BidVM>> GetProductBidsAsync(int productId);
        Task<decimal?> GetCurrentHighestBidAsync(int productId);
        Task<bool> UpdateWinningBidAsync(int productId);

        // New methods for better functionality
        Task<List<BidHistoryVM>> GetBidHistoryAsync(int productId, int? currentUserId = null);
        Task<List<MyRecentBidVM>> GetUserBidsAsync(int userId);
        Task<BidStatisticsVM> GetProductBidStatisticsAsync(int productId);
        Task<bool> ValidateBidAsync(int productId, decimal bidAmount, decimal? currentHighestBid = null);
    }
}
