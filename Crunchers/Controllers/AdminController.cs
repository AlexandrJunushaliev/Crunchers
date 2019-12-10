using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Crunchers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Crunchers.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public AdminController()
        {
        }

        public void SendEmail(string recipient, string subject, string body)
        {
            SmtpClient client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential("nicon.goniashvili@gmail.com", "KirzovieSapogi")
            };
            client.Send("nicon.goniashvili@gmail.com", recipient, subject, body);
        }

        public async Task<ActionResult> ManageShop()
        {
            return View();
        }

        // GET
        public async Task<ActionResult> ManageOrders()
        {
            var orders = await new OrderModel().GetAllOrders();
            return View(orders);
        }

        [System.Web.Http.HttpPost]
        public void UpdateOrder(bool value, string row, int orderId)
        {
            new OrderModel().UpdateOrder(!value, row, orderId);
        }

        public async Task<ActionResult> ManageCategories()
        {
            var categories = await new CategoryModel().GetCategories();
            return View(categories);
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddCategory(string categoryName)
        {
            new CategoryModel().AddCategory(categoryName);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult ChangeCategory(dynamic value, string row, int categoryId)
        {
            new CategoryModel().ChangeCategory(value[0], row, categoryId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            new CategoryModel().DeleteCategory(categoryId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ManageCharacteristics(int categoryId)
        {
            var characteristics = await new CharacteristicModel().GetCharacteristics(categoryId);
            return View(characteristics);
        }

        public async Task<ActionResult> ManageAllCharacteristics()
        {
            var characteristics = await new CharacteristicModel().GetAllCharacteristics();
            return View(characteristics);
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddCharacteristic(string characteristicType, int categoryId, string characteristicName,
            string unit)
        {
            new CharacteristicModel().AddCharacteristic(characteristicName, characteristicType, categoryId, unit);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult ChangeCharacteristic(string characteristicType, string characteristicName,
            int characteristicId,
            string unit)
        {
            new CharacteristicModel().ChangeCharacteristic(characteristicType, characteristicName, characteristicId,
                unit);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeleteCharacteristic(int characteristicId)
        {
            new CharacteristicModel().DeleteCharacteristic(characteristicId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public class ProductAdminResponse
        {
            public IEnumerable<CharacteristicModel> Characteristics;
            public IEnumerable<ProductModel> Products;
        }

        public async Task<ActionResult> ManageCategoryProducts(int categoryId)
        {
            var productResponse = new ProductAdminResponse
            {
                Products = await new ProductModel().GetProductsByCategoryId(categoryId),
                Characteristics = await new CharacteristicModel().GetCharacteristics(categoryId)
            };
            return View(productResponse);
        }

        public async Task<ActionResult> ManageAllProducts()
        {
            var productResponse = new ProductAdminResponse
            {
                Products = await new ProductModel().GetAllProducts()
            };
            return View(productResponse);
        }

        public async Task<ActionResult> AddProduct(string imageLink, string productName, int productPrice,
            int categoryId)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }


            var values = JsonConvert.DeserializeObject<dynamic[]>(documentContents);
            var characteristics = await new CharacteristicModel().GetCharacteristics(categoryId);
            var valuesToChars = new ProductModel().ConnectValuesWithChars(characteristics, values);
            new ProductModel().AddProduct(imageLink, categoryId, productName, productPrice, valuesToChars);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> ManageProduct(int productId, int categoryId)
        {
            var product = await new ProductModel().GetProductById(productId);
            var characteristics =
                await new CharacteristicModel().GetCharacteristics(categoryId);
            var productResponse = new ProductAdminResponse() {Characteristics = characteristics, Products = product};
            return View(productResponse);
        }

        [System.Web.Http.HttpPost]
        public async Task<ActionResult> ChangeProduct(string imageLink, string productName, int productPrice,
            int productId, int categoryId)
        {
            string documentContents;
            using (Stream receiveStream = Request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }

            var values = JsonConvert.DeserializeObject<dynamic[]>(documentContents);
            var characteristics = await new CharacteristicModel().GetCharacteristics(categoryId);
            var valuesToChars = new ProductModel().ConnectValuesWithChars(characteristics, values);
            new ProductModel().ChangeProduct(productId, productName, productPrice, imageLink, valuesToChars);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            new ProductModel().DeleteProduct(productId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ManageCities()
        {
            var cities = await new CityModel().GetAllCities();
            return View(cities);
        }

        [System.Web.Http.HttpPost]
        public ActionResult ChangeCity(int cityId, string nameRu)
        {
            new CityModel().ChangeCity(cityId, nameRu);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddCity(string nameRu)
        {
            new CityModel().AddCity(nameRu);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeleteCity(int cityId)
        {
            new CityModel().DeleteCity(cityId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> ManagePointsOfPickUpByCityId(int cityId)
        {
            var points = await new PointsOfPickUpModel().GetPointsOfPickUpByCityId(cityId);
            return View(points);
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddPointOfPickUp(int cityId, string address)
        {
            new PointsOfPickUpModel().AddPointOfPickUpByCityId(cityId, address);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeletePointOfPickUp(int pointId)
        {
            new PointsOfPickUpModel().DeletePointOfPickUp(pointId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult ChangePointOfPickUp(int pointId, string address)
        {
            new PointsOfPickUpModel().ChangePointsOfPickUp(address, pointId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public class FiltersResponse
        {
            public CharacteristicModel Characteristic;
            public IEnumerable<FilterModel> Filters;
        }

        public async Task<ActionResult> ManageFiltersByCharacteristicId(int characteristicId)
        {
            var characteristic = await new CharacteristicModel().GetCharacteristic(characteristicId);
            var filters = await new FilterModel().GetFiltersByCharacteristicId(characteristicId);
            var response = new FiltersResponse() {Characteristic = characteristic, Filters = filters};
            return View(response);
        }

        [System.Web.Http.HttpPost]
        public ActionResult AddFilterByCharacteristicId(int characteristicId, double from = -1, double to = -1)
        {
            new FilterModel().AddFilter(characteristicId, from, to);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [System.Web.Http.HttpPost]
        public ActionResult DeleteFilter(int filterId)
        {
            new FilterModel().DeleteFilter(filterId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}