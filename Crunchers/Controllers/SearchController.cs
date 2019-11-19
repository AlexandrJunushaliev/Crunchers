using System;
using System.Web.Mvc;

namespace Crunchers.Controllers
{
    public class A
    {
        public string text;
    }
    public class SearchController : Controller
    {
        // GET
        public ActionResult Index(string text)
        {
            var a =new A(){text = text};
            return View(a);
        }
    }
}