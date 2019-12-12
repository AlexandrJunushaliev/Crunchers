using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class CategoriesService : Service<Category>
    {
        public CategoriesService(DbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Category> Find(object id)
        {
            return await _dbSet.Include(x => x.Characteristics).FirstOrDefaultAsync(x => x.Id == (int) id);
        }
    }
}