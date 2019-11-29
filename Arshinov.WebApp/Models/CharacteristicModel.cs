﻿using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arshinov.WebApp.Models
{
    public class CharacteristicModel
    {
        public readonly string CharacteristicName;
        public readonly int CategoryId;
        public readonly int CharacteristicId;
        public readonly string CharacteristicType;
        public readonly string Unit;

        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public CharacteristicModel(DbCommand dbCommand, DbConnection dbConnection)
        {
            _dbCommand = dbCommand;
            _dbConnection = dbConnection;
            //FIXME::_dbConnection.ConnectionString = Configuration.GetConnectionString("ShopDbConnection");
        }

        private CharacteristicModel(string characteristicName, int categoryId, int characteristicId,
            string characteristicType,string unit)
        {
            CharacteristicName = characteristicName;
            CategoryId = categoryId;
            CharacteristicId = characteristicId;
            CharacteristicType = characteristicType;
            Unit = unit;
        }

        public void AddCharacteristic(string characteristicName, string characteristicType, int categoryId,string unit)
        {
            var sqlExpression =
                string.Format(
                    "INSERT INTO \"Characteristics\" (\"CategoryId\",\"CharacteristicType\",\"CharacteristicName\",\"Unit\") VALUES ('{0}','{1}','{2}','{3}')",
                    categoryId, characteristicType, characteristicName,unit);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<CharacteristicModel>> GetAllCharacteristics()
        {
            var characteristics = new List<CharacteristicModel>();
            var sqlExpression =
                string.Format("SELECT * FROM \"Characteristics\"");
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
                        var categoryId = reader.GetInt32(1);
                        var unit = reader.GetString(4);
                        var characteristic = new CharacteristicModel(characteristicName, categoryId, characteristicId,
                            characteristicType,unit);
                        characteristics.Add(characteristic);
                    }
                }

                reader.Close();
            }

            return characteristics;
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
                        var unit = reader.GetString(4);
                        var characteristic = new CharacteristicModel(characteristicName, categoryId, characteristicId,
                            characteristicType,unit);
                        characteristics.Add(characteristic);
                    }
                }

                reader.Close();
            }

            return characteristics;
        }

        public void ChangeCharacteristic(string characteristicType, string characteristicName, int characteristicId,string unit)
        {
            var sqlExpression =
                string.Format(
                    "UPDATE \"Characteristics\" SET \"CharacteristicType\"='{2}', \"CharacteristicName\"='{1}',\"Unit\"='{3}' WHERE \"CharacteristicId\"='{0}'",
                    characteristicId, characteristicName, characteristicType,unit);
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