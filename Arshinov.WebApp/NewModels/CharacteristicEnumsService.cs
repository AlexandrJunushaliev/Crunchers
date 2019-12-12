using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class CharacteristicEnumsService : Service<CharacteristicEnum>
    {
        public CharacteristicEnumsService(DbContext dbContext) : base(dbContext)
        {
        }
    }
}