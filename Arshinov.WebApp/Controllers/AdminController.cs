using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Arshinov.WebApp.Models;
using Arshinov.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Arshinov.WebApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;

        public AdminController()
        {
        }

        public AdminController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public SignInManager<User> SignInManager
        {
            get => _signInManager /*FIXME:: ?? HttpContext.GetOwinContext().Get<SignInManager<User>>()*/;
            private set => _signInManager = value;
        }

        public UserManager<User> UserManager
        {
            get => _userManager /*FIXME::?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>()*/;
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

        [HttpPost]
        public void UpdateOrder(bool value, string row, int orderId)
        {
            new OrderModel().UpdateOrder(!value, row, orderId);
        }

        public async Task<ActionResult> ManageCategories()
        {
            var categories = await new CategoryModel().GetCategories();
            return View(categories);
        }

        [HttpPost]
        public ActionResult AddCategory(string categoryName)
        {
            new CategoryModel().AddCategory(categoryName);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        [HttpPost]
        public ActionResult ChangeCategory(dynamic value, string row, int categoryId)
        {
            new CategoryModel().ChangeCategory(value[0], row, categoryId);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        [HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            new CategoryModel().DeleteCategory(categoryId);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
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

        [HttpPost]
        public ActionResult AddCharacteristic(string characteristicType, int categoryId, string characteristicName,
            string unit)
        {
            new CharacteristicModel().AddCharacteristic(characteristicName, characteristicType, categoryId, unit);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        [HttpPost]
        public ActionResult ChangeCharacteristic(string characteristicType, string characteristicName,
            int characteristicId,
            string unit)
        {
            new CharacteristicModel().ChangeCharacteristic(characteristicType, characteristicName, characteristicId,
                unit);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        [HttpPost]
        public ActionResult DeleteCharacteristic(int characteristicId)
        {
            new CharacteristicModel().DeleteCharacteristic(characteristicId);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
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
            //FIXME::
            /*
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
            new ProductModel().AddProduct(imageLink, categoryId, productName, productPrice, valuesToChars);*/
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        
        public async Task<ActionResult> ManageProduct(int productId, int categoryId)
        {
            var product = await new ProductModel().GetProductById(productId);
            var characteristics =
                await new CharacteristicModel().GetCharacteristics(categoryId);
            var productResponse = new ProductResponse() {Characteristics = characteristics, Products = product};
            return View(productResponse);
        }

        [HttpPost]
        public async Task<ActionResult> ChangeProduct(string imageLink, string productName, int productPrice,
            int productId, int categoryId)
        {
            string documentContents;
            //FIXME::
            /*
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
            */
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }
        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            new ProductModel().DeleteProduct(productId);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        public async Task<ActionResult> ManageCities()
        {
            var cities = await new CityModel().GetAllCities();
            return View(cities);
        }

        [HttpPost]
        public ActionResult ChangeCity(int cityId,string nameRu,string nameEng)
        {
            new CityModel().ChangeCity(cityId,nameRu,nameEng);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }
        [HttpPost]
        public ActionResult AddCity(string nameRu,string nameEng)
        {
            new CityModel().AddCity(nameRu,nameEng);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        public async Task<ActionResult> ManagePointsOfPickUpByCityId(int cityId)
        {
            var points = await new PointsOfPickUpModel().GetPointsOfPickUpByCityId(cityId);
            return View(points);
        }
        
        [HttpPost]
        public ActionResult AddPointOfPickUp(int cityId,string address)
        {
            new PointsOfPickUpModel().AddPointOfPickUpByCityId(cityId,address);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }

        [HttpPost]
        public ActionResult DeletePointOfPickUp(int pointId)
        {
            new PointsOfPickUpModel().DeletePointOfPickUp(pointId);
            //FIXME::return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }
        [HttpPost]
        public ActionResult ChangePointOfPickUp(int pointId,string address)
        {
            new PointsOfPickUpModel().ChangePointsOfPickUp(address,pointId);
            //return Json("Success", JsonRequestBehavior.AllowGet);
            return Json("Success");
        }
    }
}