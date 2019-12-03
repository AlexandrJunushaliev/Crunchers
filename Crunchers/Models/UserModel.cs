using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using Unity;

namespace Crunchers.Models
{
    public class UserModel
    {
        public readonly string UserId;
        public readonly string FullName;
        public readonly int CityId;
        private readonly DbCommand _dbCommand;
        private readonly DbConnection _dbConnection;

        public UserModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
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
                        var fullName = reader.GetValue(1);
                        fullName = fullName.ToString();
                        var cityId = reader.GetValue(2);
                        cityId = cityId.ToString() == "" ? 0 : int.Parse(cityId.ToString());        
                        user=new UserModel(userId,(string)fullName, (int)cityId);
                    }
                }
                reader.Close();
                _dbConnection.Close();
            }
            return user;
        }
    }
}