using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class CharacteristicsService : Service<Characteristic> 

    {
        public CharacteristicsService(DbContext dbContext) : base(dbContext)
        {
        }
    }
}