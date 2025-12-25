using Comet.DataAccess.DataContext;
using Comet.DataAccess.Interfaces;
using Comet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Errors.Model;
using System.Security.Cryptography;
using System.Text;

namespace Comet.DataAccess.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;
            var user = await _entities
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;
            // Verify password
            if (!VerifyPassword(password, user.Password))
                return null;
            // Update last login time
            await UpdateLastLoginAsync(user.Id);
            return user;
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<TUser?> GetByEmailAsync<TUser>(string email) where TUser : User
        {
            return await _context.Set<TUser>()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        Task IRepository<User>.AddAsync(User entity)
        {
            return AddAsync(entity);
        }
        public async Task ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User {userId} not found");

            // Only administrators can reset passwords (add authorization check in service layer)
            user.Password = HashPassword(newPassword);
            await UpdateAsync(user);
        }
        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException($"User {userId} not found");

            // Verify current password
            if (!VerifyPassword(currentPassword, user.Password))
                throw new InvalidOperationException("Current password is incorrect");

            // Update password
            user.Password = HashPassword(newPassword);
            await UpdateAsync(user);
        }
        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null) return;
            var entry = _context.Entry(user);
        }
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
        private bool VerifyPassword(string password, string hashedPassword)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            var hashedInput = Convert.ToBase64String(hash);
            return hashedInput == hashedPassword;
        }
    }
}
