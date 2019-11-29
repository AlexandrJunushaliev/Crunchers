using System;
using Microsoft.AspNetCore.Mvc;

namespace Arshinov.WebApp.Controllers
{
    public class A
    {
        public string text;
    }

    public class SearchController : Controller
    {
        // GET
        public IActionResult Index(string text)
        {
            var a = new A() {text = text};
            return View(a);
        }
    }
}