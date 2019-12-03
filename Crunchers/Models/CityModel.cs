using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class CityModel
    {
        public readonly string NameRu;
        public readonly int CityId;
        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public CityModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        private CityModel(string nameRu, int cityId)
        {
            CityId = cityId;
            NameRu = nameRu;
        }

        public void AddCity(string nameRu)
        {
            var sqlExpression = string.Format("Insert into \"Cities\" (\"NameRu\") values ('{0}')",
                nameRu);
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

        public void ChangeCity(int cityId, string nameRu)
        {
            var sqlExpression = string.Format(
                "Update \"Cities\" set \"NameRu\" = '{0}' where \"CityId\"='{1}'", nameRu,
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
                        var city = new CityModel(nameRu, cityId);
                        cities.Add(city);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return cities;
        }
       
        
        public async Task<IEnumerable<CityModel>> GetCityByNameRu(string nameRu)
        {
            var cities = new List<CityModel>();
            var sqlExpression = string.Format("Select * from \"Cities\" where \"NameRu\"='{0}'",nameRu);
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
                        var cityId = reader.GetInt32(0);
                        var city = new CityModel(nameRu, cityId);
                        cities.Add(city);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return cities;
        }
        public async Task<CityModel> GetCityById(int cityId)
        {
            var cities = new List<CityModel>();
            var sqlExpression = string.Format("Select * from \"Cities\" where \"CityId\"='{0}'",cityId);
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
                        var city = new CityModel(nameRu, cityId);
                        cities.Add(city);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return cities.FirstOrDefault();
        }
    }
}