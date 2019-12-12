using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arshinov.WebApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ImageLink { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }

        public int RatingCount { get; set; }
        public int RatingSum { get; set; }

        [NotMapped] public double Rating => ((double) RatingSum) / RatingCount;
        
        public List<ConcreteCharacteristic> ConcreteCharacteristics { get; set; }
    }
}