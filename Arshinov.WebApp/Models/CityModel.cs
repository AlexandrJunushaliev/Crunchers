﻿using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arshinov.WebApp.Models
{
    public class CityModel
    {
        public readonly string NameRu;
        public readonly int CityId;
        public readonly string NameEng;
        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public CityModel(DbCommand dbCommand, DbConnection dbConnection)
        {
            _dbCommand = dbCommand;
            _dbConnection = dbConnection;
            //FIXME::_dbConnection.ConnectionString = Configuration.GetConnectionString("ShopDbConnection");
        }

        private CityModel(string nameRu, int cityId, string nameEng)
        {
            CityId = cityId;
            NameRu = nameRu;
            NameEng = nameEng;
        }

        public void AddCity(string nameRu, string nameEng)
        {
            var sqlExpression = string.Format("Insert into \"Cities\" (\"NameRu\",\"NameEng\") values ('{0}','{1}')",
                nameRu, nameEng);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void DeleteCity(int cityId)
        {
            var sqlExpression = string.Format("delete from \"Cities\" where \"CityId\"='{0}'", cityId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void ChangeCity(int cityId, string nameRu, string nameEng)
        {
            var sqlExpression = string.Format(
                "Update \"Cities\" set \"NameRu\" = '{0}',\"NameEng\"='{1}' where \"CityId\"='{2}'", nameRu,
                nameEng,
                cityId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<CityModel>> GetAllCities()
        {
            var cities = new List<CityModel>();
            var sqlExpression = string.Format("Select * from \"Cities\"");
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                var reader = await _dbCommand.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var nameRu = reader.GetString(1);
                        var cityId = reader.GetInt32(0);
                        var nameEng = reader.GetString(2);
                        var city = new CityModel(nameRu, cityId, nameEng);
                        cities.Add(city);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return cities;
        }
    }
}