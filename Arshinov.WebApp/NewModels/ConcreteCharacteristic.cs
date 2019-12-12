using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arshinov.WebApp.Models
{
    public class ConcreteCharacteristic
    {
        public int Id { get; set; }

        
        public int? CharacteristicId { get; set; }
        public Characteristic Characteristic { get; set; }
        

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public double DoubleValue { get; set; }
        public int IntegerValue { get; set; }
        public int? CharacteristicEnumId { get; set; }
        public CharacteristicEnum CharacteristicEnum { get; set; }

        [NotMapped]
        public string Value
        {
            get
            {
                if (Characteristic.CharacteristicType == CharacteristicType.Double)
                {
                    return DoubleValue.ToString();
                }

                if (Characteristic.CharacteristicType == CharacteristicType.Integer)
                {
                    return IntegerValue.ToString();
                }

                if (Characteristic.CharacteristicType == CharacteristicType.Enum)
                {
                    return CharacteristicEnum.Value;
                }
                throw new Exception("Хранимый тип характеристики не существует");
            }
        }
    }
}