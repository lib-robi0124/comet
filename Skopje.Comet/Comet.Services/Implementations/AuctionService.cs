using Comet.DataAccess.DataContext;
using Comet.Domain.Entities;
using Comet.Services.Interfaces;

namespace Comet.Services.Implementations
{
    public class AuctionService : IAuctionService
    {
        private readonly AppDbContext _context;

        public AuctionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task PlaceBidAsync(int productId, decimal price)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            if (product.Price.HasValue && price <= product.Price.Value)
                throw new Exception("Bid must be higher than minimum price");

            var bid = new Bid
            {
                ProductId = productId,
                OfferedPrice = price
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync();
        }
    }
}
