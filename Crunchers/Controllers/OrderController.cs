using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using Crunchers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Crunchers.Controllers
{
    public class OrderController : Controller
    {
        // GET
        public async Task<ActionResult> Index(string nameRu = "Казань")
        {
            var points = await new PointsOfPickUpModel().GetPointsOfPickUpByNameRu(nameRu);
            var model = new MakeOrderViewModel();
            model.MakeDeliverOrder = new MakeDeliverOrder {NameRu = nameRu};
            model.MakePickUpOrder = new MakePickUpOrder {NameRu = nameRu};
            model.PointsOfPickUp = points;
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Index(MakeOrderViewModel model)
        {
            var points = model.MakeDeliverOrder?.NameRu != null
                ? await new PointsOfPickUpModel().GetPointsOfPickUpByNameRu(model.MakeDeliverOrder.NameRu)
                : await new PointsOfPickUpModel().GetPointsOfPickUpByNameRu(model.MakePickUpOrder.NameRu);
            model.PointsOfPickUp = points;
            if (model.MakeDeliverOrder != null)
            {
                model.MakePickUpOrder = new MakePickUpOrder();
                model.MakePickUpOrder.NameRu = model.MakeDeliverOrder.NameRu;
                model.MakePickUpOrder.Email = model.MakeDeliverOrder.Email;
                model.MakePickUpOrder.Name = model.MakeDeliverOrder.Name;
                model.MakePickUpOrder.CardNumber = model.MakeDeliverOrder.CardNumber;
                model.MakePickUpOrder.PhoneNumber = model.MakeDeliverOrder.PhoneNumber;
            }
            else
            {
                model.MakeDeliverOrder = new MakeDeliverOrder();
                model.MakeDeliverOrder.NameRu = model.MakePickUpOrder.NameRu;
                model.MakeDeliverOrder.Email = model.MakePickUpOrder.Email;
                model.MakeDeliverOrder.Name = model.MakePickUpOrder.Name;
                model.MakeDeliverOrder.CardNumber = model.MakePickUpOrder.CardNumber;
                model.MakeDeliverOrder.PhoneNumber = model.MakePickUpOrder.PhoneNumber;
            }

            if (ModelState.IsValid)
            {
                int orderId;
                if (model.MakeDeliverOrder != null && TryValidateModel(model.MakeDeliverOrder))
                {
                    var deliverModel = model.MakeDeliverOrder;
                    orderId = await new OrderModel().AddOrder(deliverModel.CardNumber != null, false,
                        deliverModel.ComfortDate.Add(new TimeSpan(0, model.MakeDeliverOrder.FromTime, 0, 0)),
                        deliverModel.Address, deliverModel.Name, deliverModel.PhoneNumber,
                        deliverModel.Email,
                        deliverModel.ComfortDate.Add(new TimeSpan(0, model.MakeDeliverOrder.ToTime, 0, 0)));
                }
                else
                {
                    orderId = await new OrderModel().AddOrder(model.MakePickUpOrder.CardNumber != null, true,
                        DateTime.Now,
                        $", г.{model.MakePickUpOrder.NameRu}" + model.MakePickUpOrder.PointOfPickUp,
                        model.MakePickUpOrder.Name,
                        model.MakePickUpOrder.PhoneNumber,
                        model.MakePickUpOrder.Email,
                        DateTime.Now);
                }

                return RedirectToAction("SuccessOrder", new {orderId});
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        public ActionResult SuccessOrder(int orderId)
        {
            return View(orderId);
        }

        [HttpPost]
        public async Task<ActionResult> AddProductsToOrder(int orderId, int price)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }

            var values = JsonConvert.DeserializeObject<Dictionary<int, int>>(documentContents);

            await new OrderModel().AddProductsFromOrder(values.Keys, orderId);
            new OrderModel().UpdatePrice(price, orderId);
            var order = (await new OrderModel().GetOrderById(orderId)).Item1.Distinct().First();
            var body = new StringBuilder($"Ваш заказ номер {order.OrderId} будет доставлен по адресу {order.Address}");
            if (!order.IsForPickUp)
            {
                body.Append(
                    $", {order.ComfortTimeFrom.Date.ToShortDateString()}, c {order.ComfortTimeFrom.Hour} до {order.ComfortTimeTo.Hour}");
            }

            if (!order.Paid)
            {
                body.Append($". К оплате {order.Price} р");
            }

            body.Append(".");
            AdminController.SendEmail(order.Email, "Спасибо за покупку!", body.ToString());
            new ShoppingCartModel().ClearCart(User.Identity.GetUserId());
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ShowOrderInfo(int orderId)
        {
            var order = await new OrderModel().GetOrderById(orderId);
            return View(order);
        }
    }
}