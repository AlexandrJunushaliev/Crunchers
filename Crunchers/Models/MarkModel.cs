using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class MarkModel
    {
        public readonly int MarkId;
        public readonly string UserName;
        public readonly int ProductId;
        public readonly int Mark;
        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public MarkModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        public MarkModel(int markId, string userName, int productId, int mark)
        {
            MarkId = markId;
            UserName = userName;
            ProductId = productId;
            Mark = mark;
        }

        public void DeleteMark(int markId)
        {
            var sqlExpression = string.Format("delete from \"Marks\" where \"MarkId\"='{0}'", markId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void ChangeMark(int markId, int mark)
        {
            var sqlExpression =
                string.Format("Update \"Marks\" set \"Mark\"='{0}' where \"MarkId\"='{1}'", mark, markId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void AddMark(int productId, string userName, int mark)
        {
            var sqlExpression = string.Format(
                "insert into \"Marks\" (\"UserName\",\"ProductId\",\"Mark\") values ('{0}','{1}','{2}')",
                userName, productId, mark);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<MarkModel>> GetMarksByProductId(int productId)
        {
            var sqlExpression = string.Format("Select * from \"Marks\" where \"ProductId\"='{0}'", productId);
            var marks = new List<MarkModel>();
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
                        var markId = reader.GetInt32(3);
                        var userName = reader.GetString(0);
                        var mark = reader.GetInt32(2);
                        var markModel = new MarkModel(markId, userName, productId, mark);
                        marks.Add(markModel);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return marks;
        }
    }
}