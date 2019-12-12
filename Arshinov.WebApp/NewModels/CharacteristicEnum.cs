namespace Arshinov.WebApp.Models
{
    public class CharacteristicEnum
    {
        public int Id { get; set; }
        
        public int CharacteristicId { get; set; }
        public Characteristic Characteristic { get; set; }
        
        public string Value { get; set; }
    }
}