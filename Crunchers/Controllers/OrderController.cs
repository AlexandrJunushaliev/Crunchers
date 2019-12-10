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
            }
            else
            {
                model.MakeDeliverOrder = new MakeDeliverOrder();

                model.MakeDeliverOrder.NameRu = model.MakePickUpOrder.NameRu;
            }

            if (ModelState.IsValid)
            {
                int orderId;
                if (model.MakeDeliverOrder != null && TryValidateModel(model.MakeDeliverOrder))
                {
                    var deliverModel = model.MakeDeliverOrder;
                    orderId = await new OrderModel().AddOrder(deliverModel.CardNumber != null, false,
                        deliverModel.ComfortDate.Add(new TimeSpan(0, model.MakeDeliverOrder.FromTime, 0, 0)),
                        deliverModel.Address +$", г.{deliverModel.NameRu}", deliverModel.Name, deliverModel.PhoneNumber,
                        deliverModel.Email,
                        deliverModel.ComfortDate.Add(new TimeSpan(0, model.MakeDeliverOrder.ToTime, 0, 0)));
                }
                else
                {
                    orderId = await new OrderModel().AddOrder(model.MakePickUpOrder.CardNumber != null, true,
                        DateTime.Now,
                        model.MakePickUpOrder.PointOfPickUp +$", г.{model.MakePickUpOrder.NameRu}", model.MakePickUpOrder.Name,
                        model.MakePickUpOrder.PhoneNumber,
                        model.MakePickUpOrder.Email,
                        DateTime.Now);
                }
                
                return RedirectToAction("SuccessOrder",new {orderId});
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }

        public ActionResult SuccessOrder(int orderId)
        {
            return View(orderId);
        }

        [HttpPost]
        public async Task<ActionResult> AddProductsToOrder(int orderId,int price)
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
            
            await new OrderModel().AddProductsFromOrder(values.Keys,orderId);
            new OrderModel().UpdatePrice(price,orderId);
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("nicon.goniashvili@gmail.com", "KirzovieSapogi"),
                EnableSsl = true
            };
            client.Send("nicon.goniashvili@gmail.com", "dzhunall@mail.ru", "test", "testbody");
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}