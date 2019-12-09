using System.Web.Helpers;
using System.Web.Mvc;
using Crunchers.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Crunchers.Controllers
{
    public class OrderController : Controller
    {
        // GET
        public ActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(MakeOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            // Это сообщение означает наличие ошибки; повторное отображение формы
            return View(model);
        }
    }
}