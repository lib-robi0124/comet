using Comet.Domain.Entities;

namespace Comet.DataAccess.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User?> GetByEmailAsync(string email);
        Task<TUser?> GetByEmailAsync<TUser>(string email)
            where TUser : User;
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task ResetPasswordAsync(int userId, string newPassword);
    }
}
