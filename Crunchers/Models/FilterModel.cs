using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class FilterModel
    {
        public readonly double? From;
        public readonly double? To;
        public readonly int CharacteristicId;

        public readonly string CharacteristicName;

        public readonly int FilterId;
        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public FilterModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        public FilterModel(double? from, double? to, int characteristicId, int filterId, string characteristicName)
        {
            From = from;
            To = to;
            CharacteristicId = characteristicId;
            FilterId = filterId;
            CharacteristicName = characteristicName;
        }

        public void AddFilter(int characteristicId, double from, double to)
        {
            var sqlExpression =
                string.Format(
                    "Insert into \"Filters\"(\"CharacteristicId\",\"From\",\"To\") values ('{0}','{1}','{2}')",
                    characteristicId, from, to);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void DeleteFilter(int filterId)
        {
            var sqlExpression =
                string.Format(
                    "Delete from \"Filters\" WHERE \"FilterId\"='{0}'",
                    filterId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<FilterModel>> GetFiltersByCharacteristicId(int characteristicId)
        {
            var sqlExpression =
                string.Format(
                    "select f.*, c.\"CharacteristicName\" from \"Filters\" f join \"Characteristics\" c on f.\"CharacteristicId\" = c.\"CharacteristicId\" and c.\"CharacteristicId\"='{0}'",
                    characteristicId);
            List<FilterModel> filters = new List<FilterModel>();
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
                        var from = reader.GetDouble(2);
                        var filterId = reader.GetInt32(0);
                        var to = reader.GetDouble(3);
                        var characteristicName = reader.GetString(4);
                        filters.Add(new FilterModel(from, to, characteristicId, filterId,characteristicName));
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return filters;
        }
    }
}