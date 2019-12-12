using Microsoft.AspNetCore.Identity;

namespace Arshinov.WebApp.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        
        public int CityId { get; set; }
        public City City { get; set; }
    }
}