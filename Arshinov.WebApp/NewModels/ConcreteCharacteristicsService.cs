using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class ConcreteCharacteristicsService : Service<ConcreteCharacteristic>
    {
        public ConcreteCharacteristicsService(DbContext dbContext) : base(dbContext)
        {
        }
    }
}