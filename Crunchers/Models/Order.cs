using System.ComponentModel.DataAnnotations;

namespace Crunchers.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public string Products { get; set; }
        public bool Active { get; set; }
        public bool Delivered { get; set; }
        public bool Paid { get; set; }
        public bool Deliver { get; set; }
        public string UserGuid { get; set; }
        public int Price { get; set; }
    }
}