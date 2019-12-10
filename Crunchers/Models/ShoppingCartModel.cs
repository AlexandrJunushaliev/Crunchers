using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Configuration;
using Microsoft.AspNet.Identity;
using Unity;

namespace Crunchers.Models
{
    public class ShoppingCartModel
    {
        public readonly string UserId;
        public readonly string CartJson;
        private readonly DbCommand _dbCommand;
        private readonly DbConnection _dbConnection;

        public ShoppingCartModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        public ShoppingCartModel(string userId, string cartJson)
        {
            UserId = userId;
            CartJson = cartJson;
        }

        public async Task<IdentityResult> RegisterCart(string userId)
        {
            var sqlExpression = string.Format("Insert into \"ShoppingCarts\" values('{0}','{1}')", "{}", userId);
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
        
        public void ClearCart(string userId)
        {
            var sqlExpression = $"update \"ShoppingCarts\" set \"CartJson\"= '{{}}' where \"UserId\"='{userId}'";

            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void ChangeCart(string userId, int newValue, int productId)
        {
            var sqlExpression = newValue <= 0
                ? $"update \"ShoppingCarts\" set \"CartJson\"= \"CartJson\"::jsonb-'{productId}' where \"UserId\"='{userId}'"
                : $"update \"ShoppingCarts\" set \"CartJson\"= jsonb_set(\"CartJson\"::jsonb,\'{{{productId}}}\',\'{newValue}\'::jsonb) where \"UserId\"='{userId}'";

            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<ShoppingCartModel> GetCartJson(string userId)
        {
            var sqlExpression =
                string.Format("select * from \"ShoppingCarts\" where \"UserId\"='{0}'", userId);
            ShoppingCartModel shoppingCart = null;
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
                        var cartJson = reader.GetString(0);
                        shoppingCart = new ShoppingCartModel(userId, cartJson);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return shoppingCart??new ShoppingCartModel("","{}");
        }
    }
}