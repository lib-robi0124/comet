using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Comet.Services.Interfaces;
using Comet.ViewModels.ModelsUser;
using Microsoft.Extensions.Logging;

namespace Comet.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            try
            {
                var user = await _userRepository.AuthenticateAsync(email.Trim(), password);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user {Email}", email);
                throw;
            }
        }

        public Task<User?> AuthenticateAsync(LoginVM model)
        {
            return AuthenticateAsync(model.Email ?? string.Empty, model.Password ?? string.Empty);
        }
    }
}