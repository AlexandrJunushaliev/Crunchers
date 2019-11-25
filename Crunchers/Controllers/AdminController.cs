using System.CodeDom;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
using Crunchers.Models;
using Microsoft.AspNet.Identity.Owin;
using Unity;

namespace Crunchers.Controllers
{
    [Authorize(Roles = "admin")]
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
        public void UpdateOrder(bool value, string row,int orderId)
        {
            new OrderModel().UpdateOrder(!value, row, orderId);
        }

        public async Task<ActionResult> ManageCategories()
        {
            var categories = await new CategoryModel().GetCategories();
            return View(categories);
        }
        [System.Web.Http.HttpPost]
        public void AddCategory(string categoryName)
        {
            new CategoryModel().AddCategory(categoryName);
        }
        [System.Web.Http.HttpPost]
        public void ChangeCategory(dynamic value,string row,int categoryId)
        {
            new CategoryModel().ChangeCategory(value[0],row,categoryId);
        }

        [System.Web.Http.HttpPost]
        public void DeleteCategory(int categoryId)
        {
            new CategoryModel().DeleteCategory(categoryId);
        }
        public async Task<ActionResult> ManageCharacteristics(int categoryId)
        {
            var characteristics = await new CharacteristicModel().GetCharacteristics(categoryId);
            return View(characteristics);
        }
        [System.Web.Http.HttpPost]
        public void AddCharacteristic(string characteristicType, int categoryId,string characteristicName)
        {
            new CharacteristicModel().AddCharacteristic(characteristicName,characteristicType,categoryId);
        }
        [System.Web.Http.HttpPost]
        public void ChangeCharacteristic(string characteristicType,string characteristicName,int characteristicId)
        {
            new CharacteristicModel().ChangeCharacteristic(characteristicType,characteristicName,characteristicId);
        }
        [System.Web.Http.HttpPost]
        public void DeleteCharacteristic(int characteristicId)
        {
            new CharacteristicModel().DeleteCharacteristic(characteristicId);
        }
        
    }
}