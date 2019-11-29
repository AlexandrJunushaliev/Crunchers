using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Arshinov.WebApp.Models
{
    public class UserModel
    {
        public readonly string UserId;
        public readonly string FullName;
        public readonly int CityId;
        private readonly DbCommand _dbCommand;
        private readonly DbConnection _dbConnection;

        public UserModel(DbCommand dbCommand, DbConnection dbConnection)
        {
            _dbCommand = dbCommand;
            _dbConnection = dbConnection;
            //FIXME::_dbConnection.ConnectionString = Configuration.GetConnectionString("ShopDbConnection");
        }

        private UserModel(string userId, string fullName, int cityId)
        {
            UserId = userId;
            FullName = fullName;
            CityId = cityId;
        }

        public async Task<IdentityResult> AddUser(string userId)
        {
            var sqlExpression = string.Format("Insert into \"Users\" (\"UserId\") values ('{0}')", userId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                await _dbCommand.ExecuteNonQueryAsync();
                _dbConnection.Close();
            }

            return new IdentityResult();
        }

        public void ChangeUserInfo(string userId, dynamic value, string row)
        {
            var sqlExpression = string.Format("Update \"Users\" set \"{0}\" = '{1}' where \"UserId\"='{2}'", row, value,
                userId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<UserModel> GetUser(string userId)
        {
            var sqlExpression = string.Format("Select * from \"Users\" where \"UserId\"='{0}'", userId);
            UserModel user = null;
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
                        var fullName = reader.GetString(1);
                        var cityId = reader.GetValue(2);
                        if (cityId.ToString() == "")
                        {
                            user = new UserModel(userId, fullName, 0);
                        }
                        else
                        {
                            user = new UserModel(userId, fullName, int.Parse(cityId.ToString()));
                        }
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return user;
        }
    }
}