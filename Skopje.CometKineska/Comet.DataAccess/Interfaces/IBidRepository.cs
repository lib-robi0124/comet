using Comet.Domain.Entities;

namespace Comet.DataAccess.Interfaces
{
    public interface IBidRepository : IRepository<Bid>
    {
        Task<IEnumerable<Bid>> GetBidsByProductIdAsync(int productId);
        Task<Bid?> GetHighestBidAsync(int productId);
        Task<decimal> GetCurrentHighestBidAsync(int productId);
        Task<bool> HasUserBidOnProductAsync(int productId, int buyerUserId);
        Task<IEnumerable<Bid>> GetUserBidsAsync(int buyerUserId);
    }
}
