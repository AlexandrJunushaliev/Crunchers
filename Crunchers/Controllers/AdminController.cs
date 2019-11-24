using System.ComponentModel;
using System.Threading.Tasks;
using System.Web;
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
    }
}