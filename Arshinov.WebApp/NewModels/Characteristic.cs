using System.ComponentModel.DataAnnotations.Schema;

namespace Arshinov.WebApp.Models
{
    public class Characteristic
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Column("CharacteristicType")]
        public string CharacteristicTypeString
        {
            get => CharacteristicType.ToString();
            private set => CharacteristicType = EnumExtensions.ParseEnum<CharacteristicType>(value);
        }
        [NotMapped]
        public CharacteristicType CharacteristicType { get; set; }

        public string Unit { get; set; }
    }
}