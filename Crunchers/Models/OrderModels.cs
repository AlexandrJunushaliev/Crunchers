using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using Crunchers.Controllers;

namespace Crunchers.Models
{
    public class ShortProductInfoForOrder
    {
        public readonly int ProductId;
        public readonly string ProductName;

        public ShortProductInfoForOrder(string productName, int productId)
        {
            ProductName = productName;
            ProductId = productId;
        }
    }

    public class OrderModel
    {
        public readonly int OrderId;
        public readonly bool Active;
        public readonly bool Delivered;
        public readonly bool Paid;
        public readonly bool IsForPickUp;
        public readonly int Price;
        public readonly DateTime ComfortTimeFrom;
        public readonly string Address;
        public readonly string Name;
        public readonly string Phone;
        public readonly string Email;
        public readonly DateTime ComfortTimeTo;
        private readonly DbConnection _dbConnection;
        private readonly DbCommand _dbCommand;

        public OrderModel()
        {
            _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;
        }

        private OrderModel(int orderId, bool active, bool delivered, bool paid, bool isForPickUp, int price,
            DateTime comfortTimeFrom, string address, string name, string phone, string email, DateTime comfortTimeTo)
        {
            OrderId = orderId;
            Active = active;
            Delivered = delivered;
            Paid = paid;
            IsForPickUp = isForPickUp;
            Price = price;
            ComfortTimeFrom = comfortTimeFrom;
            Address = address;
            Name = name;
            Phone = phone;
            Email = email;
            ComfortTimeTo = comfortTimeTo;
        }

        public override bool Equals(object obj)
        {
            return obj is OrderModel item && this.OrderId.Equals(item.OrderId);
        }

        public override int GetHashCode()
        {
            return this.OrderId.GetHashCode();
        }

        public async Task<IEnumerable<ProductModel>> GetProductsInOrdersWithCurrent(int currentProductId)
        {
            var sqlExpression = string.Format(
                "with po as(select * from \"ProductsFromOrders\" where \"ProductId\"='{0}') select p.\"ProductId\" from \"ProductsFromOrders\" p join po on po.\"OrderId\"=p.\"OrderId\" and p.\"ProductId\"!={0} group by p.\"ProductId\" order by  count(p.\"ProductId\") desc limit 3",
                currentProductId);
            var productIds = new List<int>();
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
                        productIds.Add(reader.GetInt32(0));
                    }
                }
                reader.Close();
                _dbConnection.Close();
            }
            return await new ProductModel().GetPrimaryProductsInfo(productIds);
        }

        public async Task UpdateOrder(bool value, string row, int orderId)
        {
            var sqlExpression =
                string.Format("UPDATE \"Orders\" SET \"{1}\"='{2}' WHERE \"OrderId\"='{0}'", orderId, row, value);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                await _dbCommand.ExecuteNonQueryAsync();
            }

            var order = (await new OrderModel().GetOrderById(orderId)).Item1.Distinct().First();
            var body = new StringBuilder($"Заказ номер {order.OrderId}:\n");
            if (order.Active)
            {
                body.Append("Активен\n");
            }
            else
            {
                body.Append("Неактивен\n");
            }

            if (order.Delivered && order.IsForPickUp)
            {
                body.Append("Доставлен\n");
                body.Append($"Заберите ваш заказ по адресу {order.Address}\n");
            }

            if (!order.Delivered && !order.IsForPickUp)
            {
                body.Append(
                    $"Ваш заказ будет доставлен {order.ComfortTimeFrom.Date.ToShortDateString()} с {order.ComfortTimeFrom.Hour} ч. до {order.ComfortTimeTo.Hour} ч.\n");
            }

            if (order.Paid)
            {
                body.Append("Оплачен\n");
            }
            else
            {
                body.Append($"К оплате {order.Price}");
            }

            AdminController.SendEmail(order.Email, "Смена статуса заказа", body.ToString());
        }

        public void UpdatePrice(int value, int orderId)
        {
            var sqlExpression =
                string.Format("UPDATE \"Orders\" SET \"Price\"='{0}' WHERE \"OrderId\"='{1}'", value, orderId);
            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = sqlExpression;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task AddProductsFromOrder(IEnumerable<int> productsIds, int orderId)
        {
            var sqlList = productsIds.Select(productsId =>
                string.Format("insert into \"ProductsFromOrders\" (\"ProductId\",\"OrderId\") values ('{0}','{1}')",
                    productsId, orderId)).ToList();


            using (_dbConnection)
            {
                _dbConnection.Open();
                foreach (var sql in sqlList)
                {
                    _dbCommand.CommandText = sql;
                    _dbCommand.Connection = _dbConnection;
                    await _dbCommand.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<int> AddOrder(bool paid, bool isForPickUp, DateTime comfortTimeFrom, string address,
            string name,
            string phone, string email, DateTime comfortTimeTo)
        {
            var orderId = 0;
            var sqlExpression =
                string.Format(
                    "insert into \"Orders\" (\"Active\", \"Delivered\", \"Paid\", \"IsForPickUp\", \"Price\", \"ComfortTimeFrom\", \"Address\", \"Name\",\"Phone\", \"Email\", \"ComfortTimeTo\") values(true, false,'{0}', '{1}',0,'{2}','{3}','{4}','{5}','{6}','{7}') returning \"OrderId\"",
                    paid, isForPickUp, comfortTimeFrom, address, name, phone, email, comfortTimeTo);
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
                        orderId = reader.GetInt32(0);
                    }
                }

                reader.Close();
                _dbConnection.Close();
            }

            return orderId;
        }

        public async Task<Tuple<IEnumerable<OrderModel>, IEnumerable<ShortProductInfoForOrder>>> GetOrderById(
            int orderId)
        {
            var orders = new List<OrderModel>();
            var products = new List<ShortProductInfoForOrder>();
            var sqlExpression =
                string.Format(
                    "select * from \"Orders\" join \"ProductsFromOrders\" on \"Orders\".\"OrderId\" = \"ProductsFromOrders\".\"OrderId\" and \"Orders\".\"OrderId\"='{0}' join \"Products\" p on \"ProductsFromOrders\".\"ProductId\" = p.\"ProductId\"",
                    orderId);
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
                        var active = reader.GetBoolean(1);
                        var delivered = reader.GetBoolean(2);
                        var paid = reader.GetBoolean(3);
                        var isForPickUp = reader.GetBoolean(4);
                        var price = reader.GetInt32(5);
                        var comfortTimeFrom = reader.GetDateTime(6);
                        var address = reader.GetString(7);
                        var name = reader.GetString(8);
                        var phone = reader.GetString(9);
                        var email = reader.GetString(10);
                        var comfortTimeTo = reader.GetDateTime(11);
                        var productId = reader.GetInt32(12);
                        var productName = reader.GetString(16);
                        var product = new ShortProductInfoForOrder(productName, productId);
                        var order = new OrderModel(orderId, active, delivered, paid, isForPickUp, price,
                            comfortTimeFrom, address, name, phone, email, comfortTimeTo);
                        orders.Add(order);
                        products.Add(product);
                    }
                }

                reader.Close();
            }

            return Tuple.Create<IEnumerable<OrderModel>, IEnumerable<ShortProductInfoForOrder>>(orders, products);
        }

        public async Task<IEnumerable<OrderModel>> GetActiveOrders()
        {
            var orders = new List<OrderModel>();
            var sqlExpression = string.Format("select * from \"Orders\" where \"Active\"=true");
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
                        var orderId = reader.GetInt32(0);
                        var active = reader.GetBoolean(1);
                        var delivered = reader.GetBoolean(2);
                        var paid = reader.GetBoolean(3);
                        var isForPickUp = reader.GetBoolean(4);
                        var price = reader.GetInt32(5);
                        var comfortTimeFrom = reader.GetDateTime(6);
                        var address = reader.GetString(7);
                        var name = reader.GetString(8);
                        var phone = reader.GetString(9);
                        var email = reader.GetString(10);
                        var comfortTimeTo = reader.GetDateTime(11);
                        var order = new OrderModel(orderId, active, delivered, paid, isForPickUp, price,
                            comfortTimeFrom, address, name, phone, email, comfortTimeTo);
                        orders.Add(order);
                    }
                }

                reader.Close();
            }

            return orders;
        }
        
        public async Task<IEnumerable<OrderModel>> GetAllOrders()
        {
            var orders = new List<OrderModel>();
            var sqlExpression = string.Format("select * from \"Orders\"");
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
                        var orderId = reader.GetInt32(0);
                        var active = reader.GetBoolean(1);
                        var delivered = reader.GetBoolean(2);
                        var paid = reader.GetBoolean(3);
                        var isForPickUp = reader.GetBoolean(4);
                        var price = reader.GetInt32(5);
                        var comfortTimeFrom = reader.GetDateTime(6);
                        var address = reader.GetString(7);
                        var name = reader.GetString(8);
                        var phone = reader.GetString(9);
                        var email = reader.GetString(10);
                        var comfortTimeTo = reader.GetDateTime(11);
                        var order = new OrderModel(orderId, active, delivered, paid, isForPickUp, price,
                            comfortTimeFrom, address, name, phone, email, comfortTimeTo);
                        orders.Add(order);
                    }
                }

                reader.Close();
            }

            return orders;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersByUserName(string userName)
        {
            var orders = new List<OrderModel>();
            var sqlExpression = string.Format("SELECT * FROM \"Orders\" WHERE \"Email\"='{0}'", userName);
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
                        var orderId = reader.GetInt32(0);
                        var active = reader.GetBoolean(1);
                        var delivered = reader.GetBoolean(2);
                        var paid = reader.GetBoolean(3);
                        var isForPickUp = reader.GetBoolean(4);
                        var price = reader.GetInt32(5);
                        var comfortTimeFrom = reader.GetDateTime(6);
                        var address = reader.GetString(7);
                        var name = reader.GetString(8);
                        var phone = reader.GetString(9);
                        var email = reader.GetString(10);
                        var comfortTimeTo = reader.GetDateTime(11);
                        var order = new OrderModel(orderId, active, delivered, paid, isForPickUp, price,
                            comfortTimeFrom, address, name, phone, email, comfortTimeTo);
                        orders.Add(order);
                    }
                }

                reader.Close();
            }

            return orders;
        }
    }
}