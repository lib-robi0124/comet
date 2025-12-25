using Comet.DataAccess.DataContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Comet.DataAccess.Implementations
{
    public class Repository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _entities;
        public Repository(AppDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }
        public virtual async Task<T?> GetByIdAsync(int id)
             => await _entities.FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAllAsync() 
            => await _entities.ToListAsync();
        public virtual async Task<T> AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public virtual async Task UpdateAsync(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }
        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public virtual async Task<bool> ExistsAsync(int id) 
            => await _entities.FindAsync(id) != null;
    }
}
