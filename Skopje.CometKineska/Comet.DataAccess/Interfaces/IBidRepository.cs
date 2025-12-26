using Comet.Domain.Entities;

namespace Comet.DataAccess.Interfaces
{
    public interface IBidRepository : IRepository<Bid>
    {
        Task<IEnumerable<Bid>> GetBidsByProductIdAsync(int productId);
        Task<Bid?> GetHighestBidAsync(int productId);
        Task<decimal> GetCurrentHighestBidAsync(int productId);
        Task<bool> HasUserBidOnProductAsync(int productId, string email);
        Task<IEnumerable<Bid>> GetUserBidsAsync(string email);
    }
}
