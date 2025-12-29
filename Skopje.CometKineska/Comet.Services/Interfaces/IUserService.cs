using Comet.Domain.Entities;
using Comet.ViewModels.ModelsUser;

namespace Comet.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Authenticate a user by email and password. Returns domain User on success or null.
        /// </summary>
        Task<User?> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Convenience method that takes a LoginVM and authenticates.
        /// </summary>
        Task<User?> AuthenticateAsync(LoginVM model);
    }
}
