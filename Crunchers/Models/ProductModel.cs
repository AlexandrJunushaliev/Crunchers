using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
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
        public readonly int CharacteristicId;
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

        private ProductModel(int productId, int imageId, string imageLink, int categoryId, string productName,
            int productPrice,
            int ratingSum, int ratingsAmount, int characteristicId,
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
            CharacteristicId = characteristicId;
        }

        public List<Tuple<dynamic, int, string>> ConnectValuesWithChars(
            IEnumerable<CharacteristicModel> characteristics,
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
                        double res;
                        if (x.CharacteristicType == "Числовое значение" &&
                            !double.TryParse(values[i - 1].ToString(CultureInfo.GetCultureInfo("en-GB")), out res))
                        {
                            throw new ArgumentException("Введено нечисловое значение");
                        }

                        return double.TryParse(values[i - 1].ToString(CultureInfo.GetCultureInfo("en-GB")), out res)
                            ? new Tuple<dynamic, int, string>(res, x.CharacteristicId, x.Unit)
                            : values[i - 1] == ""
                                ? new Tuple<dynamic, int, string>("Нет", x.CharacteristicId, x.Unit)
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
                        var valueReal = reader.GetValue(12);
                        var valueString = reader.GetValue(13);
                        var charId = reader.GetInt32(14);
                        if (valueReal.ToString() == "")
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, charId, new Tuple<string, dynamic>(unit, valueString));
                            products.Add(product);
                        }
                        else
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, charId, new Tuple<string, dynamic>(unit, valueReal));
                            products.Add(product);
                        }
                    }
                }

                reader.Close();
            }

            return products;
        }

        public async Task<IEnumerable<ProductModel>> FilterProducts(IEnumerable<Tuple<int, string>> filterValuesPairs,
            IEnumerable<FilterModel> filterModels, int categoryId)
        {
            var filters = filterModels.Join(filterValuesPairs, x => x.FilterId, y => y.Item1, (x, y) => new
            {
                x.From,
                x.To,
                x.CharacteristicId,
                Exactly = y.Item2
            });
            var products = await new ProductModel().GetProductsByCategoryId(categoryId);
            var result = products.Join(filters, x => x.CharacteristicId, y => y.CharacteristicId, (x, y) => new
                {
                    productCharacteristic = x,
                    y.Exactly,
                    y.From,
                    y.To
                }).GroupBy(x => x.productCharacteristic.ProductId).Where(x => x.All(y => (bool)
                    (y.Exactly != ""
                        ? y.productCharacteristic.ValueToCharName.Item2 == y.Exactly
                        : y.productCharacteristic.ValueToCharName.Item2 >= y.From &&
                          y.productCharacteristic.ValueToCharName.Item2 <= y.To)))
                .Select(x => x.Key);
            return products.Where(x => result.Contains(x.ProductId));
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
                        var charId = reader.GetInt32(10);
                        var valueReal = reader.GetValue(17);
                        var valueString = reader.GetValue(18);
                        if (valueReal.ToString() != "")
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, charId, new Tuple<string, dynamic>(unit, valueReal));
                            products.Add(product);
                        }
                        else
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, charId, new Tuple<string, dynamic>(unit, valueString));
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
                        var valueReal = reader.GetValue(12);
                        var valueString = reader.GetValue(13);
                        var charId = reader.GetInt32(14);
                        if (valueReal.ToString() == "")
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, charId, new Tuple<string, dynamic>(unit, valueString));
                            products.Add(product);
                        }
                        else
                        {
                            var product = new ProductModel(productId, imageId, imageLink, categoryId, productName,
                                productPrice,
                                ratingSum, ratingAmount, charId, new Tuple<string, dynamic>(unit, valueReal));
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

            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                foreach (var sqlCommand in sqlCommands)
                {
                    _dbCommand.CommandText = sqlCommand;
                    _dbCommand.ExecuteNonQuery();
                }

                foreach (var valueToCharacteristic in valuesToCharacteristics)
                {
                    string sqlExpression;
                    if (valueToCharacteristic.Item1 is string)
                    {
                        sqlExpression =
                            string.Format(
                                "UPDATE \"CharacteristicValues\" SET  \"ValueString\"='{2}' WHERE \"ProductId\"='{0}' AND \"CharacteristicId\"='{1}'",
                                productId, valueToCharacteristic.Item2,
                                valueToCharacteristic.Item1);
                    }
                    else
                    {
                        sqlExpression =
                            string.Format(
                                "UPDATE \"CharacteristicValues\" SET \"ValueReal\"='{2}' WHERE \"ProductId\"='{0}' AND \"CharacteristicId\"='{1}'",
                                productId, valueToCharacteristic.Item2,
                                valueToCharacteristic.Item1.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        sqlCommands.Add(sqlExpression);
                    }

                    _dbCommand.CommandText = sqlExpression;
                    var number = _dbCommand.ExecuteNonQuery();
                    if (number != 0) continue;
                    if (valueToCharacteristic.Item1 is string)
                    {
                        sqlExpression =
                            string.Format(
                                "INSERT INTO \"CharacteristicValues\" (\"ProductId\", \"CharacteristicId\", \"ValueString\") VALUES ('{0}','{1}','{2}')",
                                productId, valueToCharacteristic.Item2,
                                valueToCharacteristic.Item1);
                        sqlCommands.Add(sqlExpression);
                    }
                    else
                    {
                        sqlExpression =
                            string.Format(
                                "INSERT INTO \"CharacteristicValues\" (\"ProductId\", \"CharacteristicId\", \"ValueReal\") VALUES ('{0}','{1}','{2}')",
                                productId, valueToCharacteristic.Item2,
                                valueToCharacteristic.Item1.ToString(CultureInfo.GetCultureInfo("en-GB")));
                        sqlCommands.Add(sqlExpression);
                    }

                    _dbCommand.CommandText = sqlExpression;
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
                            "INSERT INTO \"CharacteristicValues\" (\"ProductId\", \"CharacteristicId\", \"ValueReal\") VALUES ('{0}','{1}','{2}')",
                            productId, valueToCharacteristic.Item2,
                            valueToCharacteristic.Item1.ToString(CultureInfo.GetCultureInfo("en-GB")));
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