using System.Web.Mvc;

namespace Crunchers.Controllers
{
    public class ShoppingCartController : Controller
    {
        // GET
        public ActionResult Index()
        {
            return View();
        }
    }
}