﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Arshinov.WebApp.Models
{
    public class ProductModel
    {
        public readonly int ProductId;
        public readonly int ImageId;
        public readonly string ImageLink;
        public readonly int CategoryId;
        public readonly string ProductName;
        public readonly int ProductPrice;
        public readonly int RatingSum;
        public readonly int RatingsAmount;
        public readonly Tuple<string, dynamic> ValueToCharName;
        private readonly DbCommand _dbCommand;
        private readonly DbConnection _dbConnection;

        public ProductModel(DbCommand dbCommand, DbConnection dbConnection)
        {
            _dbCommand = dbCommand;
            _dbConnection = dbConnection;
            //FIXME::_dbConnection.ConnectionString = Configuration.GetConnectionString("ShopDbConnection");
        }

        private ProductModel(int productId, int imageId, string imageLink, int categoryId, string productName,
            int productPrice,
            int ratingSum, int ratingsAmount,
            Tuple<string, dynamic> valueToCharName)
        {
            ProductId = productId;
            ImageId = imageId;
            ImageLink = imageLink;
            CategoryId = categoryId;
            ProductName = productName;
            ProductPrice = productPrice;
            RatingSum = ratingSum;
            RatingsAmount = ratingsAmount;
            ValueToCharName = valueToCharName;
        }

        public List<Tuple<dynamic, int,string>> ConnectValuesWithChars(IEnumerable<CharacteristicModel> characteristics,
            dynamic[] values)
        {
            var locker = new object();

            lock (locker)
            {
                var i = 0;
                var valuesToChars = characteristics
                    .Select(x =>
                    {
                        i += 1;
                        int res;
                        if (x.CharacteristicType == "Числовое значение" && !int.TryParse(values[i - 1], out res))
                        {
                            throw new ArgumentException("Введено нечисловое значение");
                        }

                        return int.TryParse(values[i - 1], out res)
                            ? new Tuple<dynamic, int, string>(res, x.CharacteristicId, x.Unit)
                            : new Tuple<dynamic, int, string>(values[i - 1], x.CharacteristicId, x.Unit);
                    }).ToList();
                i = 0;
                return valuesToChars;
            }
            
        }

        public async Task<IEnumerable<ProductModel>> GetAllProducts()
        {
            var products = new List<ProductModel>();
            var sqlExpression =
                string.Format(
                    "SELECT * FROM \"Products\" p JOIN \"Images\" i ON p.\"ProductId\"=i.\"ProductId\" AND i.\"ImageRole\"='Preview' JOIN \"CharacteristicValues\" CV ON p.\"ProductId\" = CV.\"ProductId\" JOIN \"Characteristics\" C on CV.\"CharacteristicId\"=C.\"CharacteristicId\" and p.\"ProductId\"=CV.\"ProductId\"");
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
                        var categoryId = reader.GetInt32(1);
                        var productName = reader.GetString(2);
                        var productId = reader.GetInt32(0);
                        var productPrice = reader.GetInt32(3);
                        var ratingSum = reader.GetInt32(4);
                        var ratingAmount = reader.GetInt32(5);
                        var imageId = reader.GetInt32(6);
                        var imageLink = reader.GetString(7);
                        var unit = reader.GetString(18);
                        var valueInt = reader.GetValue(12);
                        var valueString = reader.GetValue(13);
                        if (valueInt.ToString() == "")
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, new Tuple<string, dynamic>(unit, valueString));
                            products.Add(product);
                        }
                        else
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, new Tuple<string, dynamic>(unit, valueInt));
                            products.Add(product);
                        }
                    }
                }

                reader.Close();
            }

            return products;
        }
        public async Task<IEnumerable<ProductModel>> GetProductById(int productId)
        {
            var sqlExpression = string.Format(
                "select * from \"Products\" p  join \"Images\" i on p.\"ProductId\" = i.\"ProductId\" and i.\"ImageRole\"='Preview' join  \"Characteristics\" cv on p.\"CategoryId\"=cv.\"CategoryId\" and p.\"ProductId\"={0}  left join  \"CharacteristicValues\" C on p.\"ProductId\" = C.\"ProductId\" AND c.\"CharacteristicId\"=cv.\"CharacteristicId\"",
                productId);
            var products = new List<ProductModel>();
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
                        var categoryId = reader.GetInt32(1);
                        var productName = reader.GetString(2);
                        var productPrice = reader.GetInt32(3);
                        var ratingSum = reader.GetInt32(4);
                        var ratingAmount = reader.GetInt32(5);
                        var imageId = reader.GetInt32(6);
                        var imageLink = reader.GetString(7);
                        var unit = reader.GetString(14);
                        var charType = reader.GetString(12);
                        var valueInt = reader.GetValue(17);
                        var valueString = reader.GetValue(18);
                        if (charType == "Числовое значение")
                        {
                            var value = valueInt.ToString() == "" ? 0 : valueInt;
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, new Tuple<string, dynamic>(unit, value));
                            products.Add(product);
                        }
                        else
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, new Tuple<string, dynamic>(unit, valueString));
                            products.Add(product);
                        }
                    }
                }

                reader.Close();
            }

            return products;
        }

        public async Task<IEnumerable<ProductModel>> GetProductsByCategoryId(int categoryId)
        {
            var products = new List<ProductModel>();
            var sqlExpression =
                string.Format(
                    "SELECT * FROM \"Products\" p JOIN \"Images\" i ON p.\"ProductId\"=i.\"ProductId\" AND i.\"ImageRole\"='Preview' JOIN \"CharacteristicValues\" CV ON p.\"ProductId\" = CV.\"ProductId\" AND p.\"CategoryId\"={0} JOIN \"Characteristics\" C on CV.\"CharacteristicId\"=C.\"CharacteristicId\" and p.\"ProductId\"=CV.\"ProductId\"",
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
                        var imageLink = reader.GetString(7);
                        var unit = reader.GetString(18);
                        var valueInt = reader.GetValue(12);
                        var valueString = reader.GetValue(13);
                        if (valueInt.ToString() == "")
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, new Tuple<string, dynamic>(unit, valueString));
                            products.Add(product);
                        }
                        else
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, new Tuple<string, dynamic>(unit, valueInt));
                            products.Add(product);
                        }
                    }
                }

                reader.Close();
            }

            return products;
        }

        public void ChangeProduct(int productId, string productName, int productPrice, string imageLink,
            IEnumerable<Tuple<dynamic, int, string>> valuesToCharacteristics)
        {
            var sqlCommands = new List<string>();
            var sqlExpressionForProduct =
                string.Format(
                    "UPDATE \"Products\" SET \"ProductName\"='{0}',\"ProductPrice\"='{1}' WHERE \"ProductId\"='{2}'",
                    productName, productPrice, productId);
            sqlCommands.Add(sqlExpressionForProduct);
            var sqlExpressionForImage =
                string.Format(
                    "UPDATE \"Images\" SET \"ImageLink\"='{0}' WHERE \"ImageRole\"='preview' AND \"ProductId\"='{1}'",
                    imageLink, productId);
            sqlCommands.Add(sqlExpressionForImage);
            foreach (var valueToCharacteristic in valuesToCharacteristics)
            {
                if (valueToCharacteristic.Item1 is string)
                {
                    var sqlExpression =
                        string.Format(
                            "UPDATE \"CharacteristicValues\" SET  \"ValueString\"='{2}' WHERE \"ProductId\"='{0}' AND \"CharacteristicId\"='{1}'",
                            productId, valueToCharacteristic.Item2,
                            valueToCharacteristic.Item1);
                    sqlCommands.Add(sqlExpression);
                }
                else
                {
                    var sqlExpression =
                        string.Format(
                            "UPDATE \"CharacteristicValues\" SET \"ValueInt\"='{2}' WHERE \"ProductId\"='{0}' AND \"CharacteristicId\"='{1}'",
                            productId, valueToCharacteristic.Item2,
                            valueToCharacteristic.Item1);
                    sqlCommands.Add(sqlExpression);
                }
            }

            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                foreach (var sqlCommand in sqlCommands)
                {
                    _dbCommand.CommandText = sqlCommand;
                    _dbCommand.ExecuteNonQuery();
                }

                _dbConnection.Close();
            }
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

            /*
            FIXME::
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
            */
        }

        public void DeleteProduct(int productId)
        {
            var sqlExpression = string.Format("Delete from \"Products\" where \"ProductId\"='{0}'", productId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
    }
}