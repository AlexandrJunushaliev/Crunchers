using System.Collections.Generic;

namespace Arshinov.WebApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Characteristic> Characteristics { get; set; }
    }
}