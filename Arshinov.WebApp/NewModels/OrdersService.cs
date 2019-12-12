using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class OrdersService : Service<Order>
    {
        public OrdersService(DbContext dbContext) : base(dbContext)
        {
        }
    }
}