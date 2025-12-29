using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Comet.Services.Interfaces;
using Comet.ViewModels.Models;
using Microsoft.Extensions.Logging;

namespace Comet.Services.Implementations
{
    public class AuctionService : IAuctionService
    {
        private readonly IBidRepository _bidRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<AuctionService> _logger;

        public AuctionService(
            IBidRepository bidRepository,
            IProductRepository productRepository,
            ILogger<AuctionService> logger)
        {
            _bidRepository = bidRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<BidResult> PlaceBidAsync(BidVM bidViewModel)
        {
            var product = await _productRepository.GetByIdAsync(bidViewModel.ProductId);
            if (product == null)
                return new BidResult { Success = false, Message = "Product not found" };

            if (!product.IsPublished)
                return new BidResult { Success = false, Message = "Product is not available for bidding" };

            if (!product.Price.HasValue)
                return new BidResult { Success = false, Message = "Product has no starting price" };

            var currentHighestBid = await _bidRepository.GetCurrentHighestBidAsync(bidViewModel.ProductId);
            var minimumBid = currentHighestBid > 0 ? currentHighestBid + 1 : product.Price.Value;

            if (bidViewModel.Amount < minimumBid)
                return new BidResult
                {
                    Success = false,
                    Message = $"Bid must be at least ${minimumBid:F2}"
                };
            try
            {
                var bid = new Bid
                {
                    ProductId = bidViewModel.ProductId,
                    Amount = bidViewModel.Amount,
                    CompanyName = bidViewModel.CompanyName,
                    BidTime = DateTime.UtcNow,
                    IsWinningBid = false
                };
                await _bidRepository.AddAsync(bid);
                _logger.LogInformation("New bid placed: ${Amount} on product {ProductId}", bid.Amount, product.Id);

                return new BidResult
                {
                    Success = true,
                    Message = "Bid placed successfully",
                    BidId = bid.Id,
                    CurrentHighestBid = bid.Amount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing bid");
                return new BidResult { Success = false, Message = "Error placing bid. Please try again." };
            }
        }
        public async Task<IEnumerable<BidVM>> GetProductBidsAsync(int productId)
        {
            var bids = await _bidRepository.GetBidsByProductIdAsync(productId);
            return bids.Select(b => new BidVM
            {
                Amount = b.Amount,
                CompanyName = b.CompanyName
            });
        }
        public async Task<decimal> GetCurrentHighestBidAsync(int productId)
        {
            return await _bidRepository.GetCurrentHighestBidAsync(productId);
        }
        public async Task<bool> UpdateWinningBidAsync(int productId)
        {
            var highestBid = await _bidRepository.GetHighestBidAsync(productId);
            if (highestBid == null)
                return false;

            // Reset all bids for this product to non-winning
            var bids = await _bidRepository.GetBidsByProductIdAsync(productId);
            foreach (var bid in bids)
            {
                bid.IsWinningBid = false;
            }
            // Set the highest bid as winning
            highestBid.IsWinningBid = true;

            try
            {
                await _bidRepository.UpdateAsync(highestBid);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating winning bid");
                return false;
            }
        }
    }
        public class BidResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public int? BidId { get; set; }
            public decimal CurrentHighestBid { get; set; }
        }
    
}
