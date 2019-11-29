﻿using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Arshinov.WebApp.Models
{
    public class PointsOfPickUpModel
    {
        public readonly int PointId;
        public readonly int CityId;
        public readonly string Address;
        private DbCommand _dbCommand;
        private DbConnection _dbConnection;

        public PointsOfPickUpModel(DbCommand dbCommand, DbConnection dbConnection)
        {
            _dbCommand = dbCommand;
            _dbConnection = dbConnection;
            //FIXME::_dbConnection.ConnectionString = Configuration.GetConnectionString("ShopDbConnection");
        }

        private PointsOfPickUpModel(int pointId, int cityId, string address)
        {
            PointId = pointId;
            CityId = cityId;
            Address = address;
        }

        public void AddPointOfPickUpByCityId(int cityId,string address)
        {
            var sqlExpression =
                string.Format("insert into \"PointsOfPickUp\" (\"CityId\",\"Address\") VALUES ('{0}','{1}')", cityId,
                    address);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void ChangePointsOfPickUp(string address, int pointId)
        {
            var sqlExpression = string.Format("update \"PointsOfPickUp\" set \"Address\"='{0}' where \"PointId\"='{1}'",
                address, pointId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        public void DeletePointOfPickUp(int pointId)
        {
            var sqlExpression = string.Format("delete from \"PointsOfPickUp\" where \"PointId\"='{0}'", pointId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.Connection = _dbConnection;
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
        public async Task<IEnumerable<PointsOfPickUpModel>> GetPointsOfPickUpByCityId(int cityId)
        {
            var points = new List<PointsOfPickUpModel>();
            var sqlExpression = string.Format("select * from \"PointsOfPickUp\" where \"CityId\"='{0}'", cityId);
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
                        var pointId = reader.GetInt32(0);
                        var address = reader.GetString(2);
                        var point = new PointsOfPickUpModel(pointId,cityId,address);
                        points.Add(point);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return points;
        }
    }
}