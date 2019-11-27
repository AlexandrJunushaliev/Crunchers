using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Crunchers.Models;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;

namespace Crunchers.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ActionResult ManageShop()
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

        public class ProductResponse
        {
            public IEnumerable<CharacteristicModel> Characteristics;
            public IEnumerable<ProductModel> Products;
        }

        public async Task<ActionResult> ManageCategoryProducts(int categoryId)
        {
            var productResponse = new ProductResponse
            {
                Products = await new ProductModel().GetProductsByCategoryId(categoryId),
                Characteristics = await new CharacteristicModel().GetCharacteristics(categoryId)
            };
            return View(productResponse);
        }
        public async Task<ActionResult> ManageAllProducts()
        {
            var productResponse = new ProductResponse
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
            var productResponse = new ProductResponse() {Characteristics = characteristics, Products = product};
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
        public ActionResult ChangeCity(int cityId,string nameRu,string nameEng)
        {
            new CityModel().ChangeCity(cityId,nameRu,nameEng);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public ActionResult AddCity(string nameRu,string nameEng)
        {
            new CityModel().AddCity(nameRu,nameEng);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}