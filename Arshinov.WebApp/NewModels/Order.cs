using System;
using System.Collections.Generic;

namespace Arshinov.WebApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }

        public decimal Price { get; set; }
        
        public List<OrderProduct> OrderProducts { get; set; }
    }
}