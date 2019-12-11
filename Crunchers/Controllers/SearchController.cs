using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Crunchers.Models;

namespace Crunchers.Controllers
{
    public class A
    {
        public string text;
    }
    public class SearchController : Controller
    {
        // GET
        public async Task<ActionResult> Index(string text)
        {
            var resp = await new SearchModel().FullTextSearch(text);
            return View(resp);
        }
    }
}