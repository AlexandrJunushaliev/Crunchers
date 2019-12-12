using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class OrderProductsService : Service<OrderProduct>
    {
        public OrderProductsService(DbContext dbContext) : base(dbContext)
        {
        }
    }
}