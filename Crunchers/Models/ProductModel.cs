using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class ProductModel
    {
        public readonly int ProductId;
        public readonly int ImageId;
        public readonly int CategoryId;
        public readonly string ProductName;
        public readonly int ProductPrice;
        public readonly int RatingSum;
        public readonly int RatingsAmount;
        public readonly Tuple<string, dynamic> ValueToCharName;
        private readonly DbCommand _dbCommand;
        private readonly DbConnection _dbConnection;

        public ProductModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        private ProductModel(int productId, int imageId, int categoryId, string productName, int productPrice,
            int ratingSum, int ratingsAmount,
            Tuple<string, dynamic> valueToCharName)
        {
            ProductId = productId;
            ImageId = imageId;
            CategoryId = categoryId;
            ProductName = productName;
            ProductPrice = productPrice;
            RatingSum = ratingSum;
            RatingsAmount = ratingsAmount;
            ValueToCharName = valueToCharName;
        }

        public async Task<IEnumerable<ProductModel>> GetProductsByCategoryId(int categoryId)
        {
            var products = new List<ProductModel>();
            var sqlExpression =
                string.Format(
                    "SELECT * FROM \"Products\" p JOIN \"Images\" i ON p.\"ProductId\"=i.\"ProductId\" AND i.\"ImageRole\"='Preview' JOIN \"CharacteristicValues\" CV ON p.\"ProductId\" = CV.\"ProductId\" AND p.\"CategoryId\"={0} JOIN \"Characteristics\" C on CV.\"CharacteristicId\"=C.\"CharacteristicId\"",
                    categoryId);
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
                        var productName = reader.GetString(2);
                        var productId = reader.GetInt32(0);
                        var productPrice = reader.GetInt32(3);
                        var ratingSum = reader.GetInt32(4);
                        var ratingAmount = reader.GetInt32(5);
                        var imageId = reader.GetInt32(6);
                        var unit = reader.GetString(18);
                        var valueInt = reader.GetValue(12);
                        var valueString = reader.GetValue(13);
                        if (valueInt.ToString() == "")
                        {
                            var product = new ProductModel(productId, imageId, categoryId, productName, productPrice,
                                ratingSum, ratingAmount,new Tuple<string, dynamic>(unit,valueString));
                            products.Add(product);
                        }
                        else
                        {
                            var product = new ProductModel(productId, imageId, categoryId, productName, productPrice,
                                ratingSum, ratingAmount,new Tuple<string, dynamic>(unit,valueInt));
                            products.Add(product);
                        }
                            
                        
                       
                    }
                }

                reader.Close();
            }

            return products;
        }

        public void AddProduct(string imageLink, int categoryId, string productName, int productPrice,
            IEnumerable<Tuple<dynamic, int, string>> valuesToCharacteristics)
        {
            var productId = 0;
            var sqlExpressionForProduct =
                string.Format(
                    "INSERT INTO \"Products\" (\"CategoryId\", \"ProductName\", \"ProductPrice\", \"RatingsSum\", \"RatingsAmount\") VALUES ('{0}','{1}','{2}','{3}','{4}') RETURNING \"ProductId\"",
                    categoryId, productName, productPrice, 0, 0);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpressionForProduct;
                var reader = _dbCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        productId = reader.GetInt32(0);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            var sqlCommands = new List<string>();
            var sqlExpressionForAddPhoto =
                string.Format(
                    "INSERT INTO \"Images\" (\"ProductId\", \"ImageLink\", \"ImageRole\") VALUES ('{0}','{1}','{2}')",
                    productId, imageLink, "Preview");
            sqlCommands.Add(sqlExpressionForAddPhoto);
            foreach (var valueToCharacteristic in valuesToCharacteristics)
            {
                if (valueToCharacteristic.Item1 is string)
                {
                    var sqlExpression =
                        string.Format(
                            "INSERT INTO \"CharacteristicValues\" (\"ProductId\", \"CharacteristicId\", \"ValueString\") VALUES ('{0}','{1}','{2}')",
                            productId, valueToCharacteristic.Item2,
                            valueToCharacteristic.Item1);
                    sqlCommands.Add(sqlExpression);
                }
                else
                {
                    var sqlExpression =
                        string.Format(
                            "INSERT INTO \"CharacteristicValues\" (\"ProductId\", \"CharacteristicId\", \"ValueInt\") VALUES ('{0}','{1}','{2}')",
                            productId, valueToCharacteristic.Item2, valueToCharacteristic.Item1);
                    sqlCommands.Add(sqlExpression);
                }
            }

            var newdbConnection = MvcApplication.Container.Resolve<DbConnection>();
            newdbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;

            using (newdbConnection)
            {
                newdbConnection.Open();
                _dbCommand.Connection = newdbConnection;
                foreach (var sqlCommand in sqlCommands)
                {
                    _dbCommand.CommandText = sqlCommand;
                    _dbCommand.ExecuteNonQuery();
                }

                newdbConnection.Close();
            }
        }
    }
}