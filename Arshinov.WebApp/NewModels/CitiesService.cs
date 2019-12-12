using Microsoft.EntityFrameworkCore;

namespace Arshinov.WebApp.Models
{
    public class CitiesService : Service<City>
    {
        public CitiesService(DbContext dbContext) : base(dbContext)
        {
        }
    }
}