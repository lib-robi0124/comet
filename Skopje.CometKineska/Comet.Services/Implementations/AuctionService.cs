using Comet.DataAccess.DataContext;
using Comet.Domain.Entities;
using Comet.Services.Interfaces;
using Comet.ViewModels.Auction;
using Comet.ViewModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Comet.Services.Implementations
{
    public class AuctionService : IAuctionService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuctionService> _logger;

        public AuctionService(
            AppDbContext context,
            ILogger<AuctionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BidResult> PlaceBidAsync(BidVM bidViewModel)
        {
            var result = new BidResult();

            try
            {
                // Validate product exists and is active
                var product = await _context.Products
                    .Include(p => p.Bids)
                    .FirstOrDefaultAsync(p => p.Id == bidViewModel.ProductId);

                if (product == null)
                {
                    result.Success = false;
                    result.Message = "Product not found.";
                    return result;
                }

                // Check if auction is still active (using AuctionStartTime)
                // Note: Add AuctionEndDate to Product entity if needed
                if (!product.IsPublished)
                {
                    result.Success = false;
                    result.Message = "This auction is not active.";
                    return result;
                }

                // Get current highest bid
                var currentHighestBid = await GetCurrentHighestBidAsync(bidViewModel.ProductId);

                // Validate bid amount
                var validationResult = await ValidateBidAsync(
                    bidViewModel.ProductId,
                    bidViewModel.Amount,
                    currentHighestBid
                );

                if (!validationResult)
                {
                    result.Success = false;
                    result.Message = $"Bid must be higher than current bid: {currentHighestBid?.ToString("C") ?? product.MinimumBidPrice.ToString("C")}";
                    return result;
                }

                // Create new bid
                var bid = new Bid
                {
                    ProductId = bidViewModel.ProductId,
                    Amount = bidViewModel.Amount,
                    CompanyName = bidViewModel.CompanyName,
                    BidTime = DateTime.Now,
                    IsWinningBid = true // Temporary, will update others
                };

                // Set BuyerUserId if user is logged in
                if (bidViewModel.UserId > 0)
                {
                    bid.BuyerUserId = bidViewModel.UserId;
                }

                _context.Bids.Add(bid);

                // Update previous winning bids to not winning
                var previousWinningBids = await _context.Bids
                    .Where(b => b.ProductId == bidViewModel.ProductId && b.IsWinningBid)
                    .ToListAsync();

                foreach (var previousBid in previousWinningBids)
                {
                    previousBid.IsWinningBid = false;
                }

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Bid placed successfully!";
                result.BidId = bid.Id;
                result.NewCurrentBid = bid.Amount;

                _logger.LogInformation("Bid placed successfully for product {ProductId}: {Amount}",
                    bidViewModel.ProductId, bidViewModel.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid for product {ProductId}", bidViewModel.ProductId);
                result.Success = false;
                result.Message = "An error occurred while placing your bid.";
                result.Errors.Add(ex.Message);
            }

            return result;
        }
        public async Task<IEnumerable<BidVM>> GetProductBidsAsync(int productId)
        {
            try
            {
                var bids = await _context.Bids
                    .Where(b => b.ProductId == productId)
                    .Include(b => b.Product)
                    .OrderByDescending(b => b.Amount)
                    .ThenByDescending(b => b.BidTime)
                    .Select(b => new BidVM
                    {
                        ProductId = b.ProductId,
                        Amount = b.Amount,
                        CompanyName = b.CompanyName,
                        ProductCode = b.Product.ProductCode,
                        BidTime = b.BidTime
                    })
                    .ToListAsync();

                return bids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bids for product {ProductId}", productId);
                return new List<BidVM>();
            }
        }
        public async Task<decimal?> GetCurrentHighestBidAsync(int productId)
        {
            try
            {
                var highestBid = await _context.Bids
                    .Where(b => b.ProductId == productId)
                    .OrderByDescending(b => b.Amount)
                    .Select(b => (decimal?)b.Amount)
                    .FirstOrDefaultAsync();

                // If no bids, return the minimum bid price from product
                if (!highestBid.HasValue)
                {
                    var product = await _context.Products
                        .Where(p => p.Id == productId)
                        .Select(p => (decimal?)p.MinimumBidPrice)
                        .FirstOrDefaultAsync();

                    return product;
                }

                return highestBid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current highest bid for product {ProductId}", productId);
                return null;
            }
        }
        public async Task<bool> UpdateWinningBidAsync(int productId)
        {
            try
            {
                var highestBid = await _context.Bids
                    .Where(b => b.ProductId == productId)
                    .OrderByDescending(b => b.Amount)
                    .FirstOrDefaultAsync();

                if (highestBid != null)
                {
                    // Reset all bids to not winning
                    var allBids = await _context.Bids
                        .Where(b => b.ProductId == productId)
                        .ToListAsync();

                    foreach (var bid in allBids)
                    {
                        bid.IsWinningBid = false;
                    }

                    // Set the highest bid as winning
                    highestBid.IsWinningBid = true;
                    await _context.SaveChangesAsync();

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating winning bid for product {ProductId}", productId);
                return false;
            }
        }
        public async Task<List<BidHistoryVM>> GetBidHistoryAsync(int productId, int? currentUserId = null)
        {
            try
            {
                var bids = await _context.Bids
                    .Where(b => b.ProductId == productId)
                    .OrderByDescending(b => b.BidTime)
                    .Select(b => new BidHistoryVM
                    {
                        BidderName = b.CompanyName,
                        Amount = b.Amount,
                        BidTime = b.BidTime,
                        IsCurrentUser = currentUserId.HasValue && b.BuyerUserId == currentUserId.Value
                    })
                    .Take(20) // Limit to last 20 bids for performance
                    .ToListAsync();

                return bids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bid history for product {ProductId}", productId);
                return new List<BidHistoryVM>();
            }
        }
        public async Task<List<MyRecentBidVM>> GetUserBidsAsync(int userId)
        {
            try
            {
                var userBids = await _context.Bids
                    .Where(b => b.BuyerUserId == userId)
                    .Include(b => b.Product)
                    .OrderByDescending(b => b.BidTime)
                    .Select(b => new MyRecentBidVM
                    {
                        ProductId = b.ProductId,
                        ProductCode = b.Product.ProductCode,
                        MyBidAmount = b.Amount,
                        CurrentHighestBid = b.Product.MinimumBidPrice,
                        BidTime = b.BidTime,
                        IsWinning = b.IsWinningBid
                    })
                    .Take(50) // Limit to last 50 bids
                    .ToListAsync();

                return userBids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user bids for user {UserId}", userId);
                return new List<MyRecentBidVM>();
            }
        }
        public async Task<BidStatisticsVM> GetProductBidStatisticsAsync(int productId)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Bids)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                    return new BidStatisticsVM();

                var bids = product.Bids ?? new List<Bid>();

                var statistics = new BidStatisticsVM
                {
                    TotalBids = bids.Count,
                    StartingPrice = product.MinimumBidPrice,
                    CurrentHighestBid = bids.Any() ? bids.Max(b => b.Amount) : product.MinimumBidPrice,
                    AverageBid = bids.Any() ? bids.Average(b => b.Amount) : null,
                    UniqueBidders = bids.Select(b => b.CompanyName).Distinct().Count(),
                    LastBidTime = bids.Any() ? bids.Max(b => b.BidTime) : null
                };

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bid statistics for product {ProductId}", productId);
                return new BidStatisticsVM();
            }
        }
        public async Task<bool> ValidateBidAsync(int productId, decimal bidAmount, decimal? currentHighestBid = null)
        {
            try
            {
                // Get current highest bid if not provided
                if (!currentHighestBid.HasValue)
                {
                    currentHighestBid = await GetCurrentHighestBidAsync(productId);
                }

                // Get product for minimum bid increment rules
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                    return false;

                // Check if auction is still active
                if (!product.IsPublished)
                    return false;

                // Basic validation: bid must be higher than current highest
                if (currentHighestBid.HasValue && bidAmount <= currentHighestBid.Value)
                    return false;

                // If no current bid, check against minimum bid price
                if (!currentHighestBid.HasValue && bidAmount < product.MinimumBidPrice)
                    return false;

                // You can add more rules here, like minimum increment
                // Example: bid must be at least $10 more than current bid
                // if (currentHighestBid.HasValue && bidAmount < currentHighestBid.Value + 10)
                //     return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating bid for product {ProductId}", productId);
                return false;
            }
        }
    }
    
}
