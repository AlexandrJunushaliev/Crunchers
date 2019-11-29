﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Arshinov.WebApp.Models
{
    //вопрос: делать ли код fluentAPI? делать OrderWorker и Order? доступ к полям необходим, но он может запутать пользователя

    public class OrderModel
    {
        public readonly int Id;
        public readonly string Products;
        public readonly bool Active;
        public readonly bool Delivered;
        public readonly bool Paid;
        public readonly bool Deliver;
        public readonly string UserGuid;
        public readonly int Price;

        private readonly DbConnection _dbConnection;
        private readonly DbCommand _dbCommand;

        public OrderModel(DbCommand dbCommand, DbConnection dbConnection)
        {
            _dbCommand = dbCommand;
            _dbConnection = dbConnection;
            //FIXME::_dbConnection.ConnectionString = Configuration.GetConnectionString("ShopDbConnection");
        }

        private OrderModel(int id, string products, bool active, bool delivered, bool paid, bool deliver,
            string userGuid, int price)
        {
            Id = id;
            Products = products;
            Active = active;
            Delivered = delivered;
            Deliver = deliver;
            Paid = paid;
            UserGuid = userGuid;
            Price = price;
        }

        public void UpdateOrder(bool value, string row,int orderId)
        {
            var sqlExpression =
                string.Format("UPDATE \"Orders\" SET \"{1}\"='{2}' WHERE \"Id\"='{0}'",orderId,row,value);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<OrderModel>> GetAllOrders()
        {
            var orders = new List<OrderModel>();
            var sqlExpression = string.Format("SELECT * FROM \"Orders\" ");
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
                        var id = reader.GetInt32(0);
                        var products = reader.GetString(1);
                        var active = reader.GetBoolean(2);
                        var delivered = reader.GetBoolean(3);
                        var paid = reader.GetBoolean(4);
                        var deliver = reader.GetBoolean(5);
                        var guid = reader.GetString(6);
                        var price = reader.GetInt32(7);
                        var order = new OrderModel(id, products, active, delivered, paid, deliver, guid, price);
                        orders.Add(order);
                    }
                }

                reader.Close();
            }

            return orders;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersByUserGuid(string userGuid)
        {
            var orders = new List<OrderModel>();
            var sqlExpression = string.Format("SELECT * FROM \"Orders\" WHERE \"UserGuid\"='{0}'", userGuid);
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
                        var id = reader.GetInt32(0);
                        var products = reader.GetString(1);
                        var active = reader.GetBoolean(2);
                        var delivered = reader.GetBoolean(3);
                        var paid = reader.GetBoolean(4);
                        var deliver = reader.GetBoolean(5);
                        var guid = reader.GetString(6);
                        var price = reader.GetInt32(7);
                        var order = new OrderModel(id, products, active, delivered, paid, deliver, guid, price);
                        orders.Add(order);
                    }
                }

                reader.Close();
            }

            return orders;
        }
    }
}