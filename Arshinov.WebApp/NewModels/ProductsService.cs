using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class ProductsService : Service<Product>
    {
        public ProductsService(DbContext dbContext) : base(dbContext)
        {
        }
    }
}