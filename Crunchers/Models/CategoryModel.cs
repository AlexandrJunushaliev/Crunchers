﻿using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Unity;

namespace Crunchers.Models
{
    public class CategoryModel
    {
        public readonly string CategoryName;
        public readonly int CategoryId;
        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public CategoryModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }
        private CategoryModel(string categoryName,int categoryId)
        {
            CategoryName = categoryName;
            CategoryId = categoryId;
        }

        public void AddCategory(string categoryName)
        {
            Thread.Sleep(2000);
            var sqlExpression =
                string.Format("INSERT INTO \"Categories\" (\"CategoryName\") VALUES ('{0}')",categoryName);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
        public void DeleteCategory(int categoryId)
        {
            var sqlExpression =
                string.Format("DELETE FROM \"Categories\" WHERE (\"CategoryId\") = '{0}'",categoryId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
        public async Task<IEnumerable<CategoryModel>> GetCategories()
        {
            var categories = new List<CategoryModel>();
            var sqlExpression =
                string.Format("SELECT * FROM \"Categories\"");
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                var reader =  await _dbCommand.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var categoryName = reader.GetString(1);
                        var categoryId = reader.GetInt32(0);

                        var category = new CategoryModel(categoryName, categoryId);
                        categories.Add(category);
                    }
                }
                reader.Close();
            }
            return categories;
        }
        public void ChangeCategory(dynamic value, string row,int categoryId)
        {
            var sqlExpression =
                string.Format("UPDATE \"Categories\" SET \"{1}\"='{2}' WHERE \"CategoryId\"='{0}'",categoryId,row,value);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
            }
        }
    }
}