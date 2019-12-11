using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class SearchModel
    {
        private readonly DbConnection _dbConnection;
        private readonly DbCommand _dbCommand;

        public SearchModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        public async Task<Tuple<IEnumerable<ProductModel>, IEnumerable<CategoryModel>>> FullTextSearch(string text)
        {
            var parsedText = text.Split(' ');
            var stringBuilder = new StringBuilder();
            foreach (var word in parsedText)
            {
                stringBuilder.Append($"{word} | ");
            }

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            
            var separatedText = stringBuilder.ToString();
            var sqlExpressionForCategories = string.Format(
                "SELECT \"CategoryName\",\"CategoryId\" FROM \"Categories\" WHERE to_tsvector(\"CategoryName\") @@ to_tsquery('{0}')",
                separatedText);
            var sqlExpressionForProducts = string.Format(
                "SELECT \"ProductId\" FROM \"Products\" WHERE to_tsvector(\"ProductName\") @@ to_tsquery('{0}')",
                separatedText);
            var categories = new List<CategoryModel>();
            var productIds = new List<int>();
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpressionForCategories;
                var categoryReader = await _dbCommand.ExecuteReaderAsync();
                if (categoryReader.HasRows)
                {
                    while (categoryReader.Read())
                    {
                        var categoryId = categoryReader.GetInt32(1);
                        var categoryName = categoryReader.GetString(0);
                        var category = new CategoryModel(categoryName, categoryId);
                        categories.Add(category);
                    }
                }

                categoryReader.Close();
                
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpressionForProducts;
                var productReader = await _dbCommand.ExecuteReaderAsync();
                if (productReader.HasRows)
                {
                    while (productReader.Read())
                    {
                        productIds.Add(productReader.GetInt32(0));
                    }
                }

                productReader.Close();
                _dbConnection.Close();
            }

            var products = await new ProductModel().GetPrimaryProductsInfo(productIds);

            return Tuple.Create<IEnumerable<ProductModel>, IEnumerable<CategoryModel>>(products,categories);
        }
    }
}