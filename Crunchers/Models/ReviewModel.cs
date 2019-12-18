using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class ReviewModel
    {
        public readonly int ReviewId;
        public readonly string UserName;
        public readonly string ReviewText;
        public readonly int ProductId;
        public readonly DateTime ReviewDate;
        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public ReviewModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        public ReviewModel(int reviewId, string userName, string reviewText, int productId, DateTime reviewDate)
        {
            ReviewId = reviewId;
            UserName = userName;
            ReviewText = reviewText;
            ProductId = productId;
            ReviewDate = reviewDate;
        }

        public void DeleteReview(int reviewId)
        {
            var sqlExpression = string.Format("delete from \"Reviews\" where \"ReviewId\"='{0}'", reviewId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void AddReview(string reviewText, string userName, DateTime reviewDate, int productId)
        {
            var sqlExpression =
                string.Format(
                    "insert into \"Reviews\" (\"ReviewText\",\"UserName\",\"ReviewDate\",\"ProductId\") values ('{0}','{1}','{2}','{3}')",
                    reviewText, userName, reviewDate, productId);
            var sqlExpressionCheckForAvailable =
                string.Format("select * from \"Reviews\" where \"UserName\"='{0}' and \"ProductId\"='{1}'", userName,
                    productId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpressionCheckForAvailable;

                var reader = _dbCommand.ExecuteReader();
                var available = !reader.HasRows;
                reader.Close();
                if (available)
                {
                    _dbCommand.CommandText = sqlExpression;
                    _dbCommand.ExecuteNonQuery();
                }
                _dbConnection.Close();
            }
        }

        public void ChangeReview(string reviewText, DateTime reviewDate, int reviewId)
        {
            var sqlExpression =
                string.Format(
                    "update \"Reviews\" set \"ReviewText\"='{0}',\"ReviewDate\"='{1}' where \"ReviewId\"='{2}'",
                    reviewText, reviewDate, reviewId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public async Task<IEnumerable<ReviewModel>> GetAllReviewsByProductId(int productId)
        {
            var sqlExpression = string.Format("Select * from \"Reviews\" where \"ProductId\"='{0}'", productId);
            var reviews = new List<ReviewModel>();
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
                        var reviewId = reader.GetInt32(0);
                        var userName = reader.GetString(1);
                        var reviewText = reader.GetString(2);
                        var reviewDate = reader.GetDateTime(3);
                        var review = new ReviewModel(reviewId, userName, reviewText, productId, reviewDate);
                        reviews.Add(review);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return reviews;
        }
    }
}