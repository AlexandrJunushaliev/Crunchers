using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class CharacteristicModel
    {
        public readonly string CharacteristicName;
        public readonly int CategoryId;
        public readonly int CharacteristicId;
        public readonly string CharacteristicType;

        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public CharacteristicModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        public CharacteristicModel(string characteristicName, int categoryId, int characteristicId,
            string characteristicType)
        {
            CharacteristicName = characteristicName;
            CategoryId = categoryId;
            CharacteristicId = characteristicId;
            CharacteristicType = characteristicType;
        }

        public void AddCharacteristic(string characteristicName, string characteristicType, int categoryId)
        {
            var sqlExpression =
                string.Format(
                    "INSERT INTO \"Characteristics\" (\"CategoryId\",\"CharacteristicType\",\"CharacteristicName\") VALUES ('{0}','{1}','{2}')",
                    categoryId, characteristicType, characteristicName);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<CharacteristicModel>> GetCharacteristics(int categoryId)
        {
            var characteristics = new List<CharacteristicModel>();
            var sqlExpression =
                string.Format("SELECT * FROM \"Characteristics\" WHERE \"CategoryId\"={0}", categoryId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                var reader = await _dbCommand.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var characteristicName = reader.GetString(3);
                        var characteristicId = reader.GetInt32(0);
                        var characteristicType = reader.GetString(2);
                        var characteristic = new CharacteristicModel(characteristicName, categoryId, characteristicId,
                            characteristicType);
                        characteristics.Add(characteristic);
                    }
                }

                reader.Close();
            }

            return characteristics;
        }

        public void ChangeCharacteristic(string characteristicType, string characteristicName, int characteristicId)
        {
            var sqlExpression =
                string.Format(
                    "UPDATE \"Characteristics\" SET \"CharacteristicType\"='{2}', \"CharacteristicName\"='{1}' WHERE \"CharacteristicId\"='{0}'",
                    characteristicId, characteristicName, characteristicType);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
            }
        }
        public void DeleteCharacteristic(int characteristicId)
        {
            var sqlExpression =
                string.Format("DELETE FROM \"Characteristics\" WHERE (\"CharacteristicId\") = '{0}'",characteristicId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
    }
}