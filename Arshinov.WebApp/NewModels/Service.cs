using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public abstract class Service
    {
        protected DbContext _dbContext;

        public Service(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }

    public abstract class Service<T> : Service where T : class
    {
        protected DbSet<T> _dbSet;

        public Service(DbContext dbContext) : base(dbContext)
        {
            _dbSet = dbContext.Set<T>();
        }

        public virtual IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public virtual async Task<T> Find(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        
        public virtual async Task Add(T item)
        {
            await _dbSet.AddAsync(item);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task Remove(T item)
        {
            _dbSet.Remove(item);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task RemoveById(object id)
        {
            await Remove(await Find(id));
        }

        public virtual async Task SaveChangesAsync(T item)
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}