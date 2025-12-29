using Comet.DataAccess.DataContext;
using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Comet.ViewModels.ModelsUser;
using Microsoft.EntityFrameworkCore;

namespace Comet.DataAccess.Implementations
{
    public class BidRepository : IBidRepository
    {
        private readonly AppDbContext _context;
        public BidRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Bid?> GetByIdAsync(int id)
        {
            return await _context.Bids
                .Include(b => b.Product)
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<IEnumerable<Bid>> GetAllAsync()
        {
            return await _context.Bids
                .Include(b => b.Product)
                .OrderByDescending(b => b.BidTime)
                .ToListAsync();
        }
        public async Task AddAsync(Bid entity)
        {
            await _context.Bids.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Bid entity)
        {
            _context.Bids.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Bids.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bids.AnyAsync(e => e.Id == id);
        }
        public async Task<IEnumerable<Bid>> GetBidsByProductIdAsync(int productId)
        {
            return await _context.Bids
                .Where(b => b.ProductId == productId)
                .OrderByDescending(b => b.Amount)
                .ThenBy(b => b.BidTime)
                .ToListAsync();
        }
        public async Task<Bid?> GetHighestBidAsync(int productId)
        {
            return await _context.Bids
                .Where(b => b.ProductId == productId)
                .OrderByDescending(b => b.Amount)
                .ThenBy(b => b.BidTime)
                .FirstOrDefaultAsync();
        }
        public async Task<decimal> GetCurrentHighestBidAsync(int productId)
        {
            var highestBid = await GetHighestBidAsync(productId);
            return highestBid?.Amount ?? 0;
        }
        public async Task<bool> HasUserBidOnProductAsync(int productId, int buyerUserId)
        {
            return await _context.Bids
                .AnyAsync(b => b.ProductId == productId && b.BuyerUserId == buyerUserId);
        }
        public async Task<IEnumerable<Bid>> GetUserBidsAsync(int buyerUserId)
        {
            return await _context.Bids
                .Where(b => b.BuyerUserId == buyerUserId)
                .Include(b => b.Product)
                .OrderByDescending(b => b.BidTime)
                .ToListAsync();
        }
    }
}
