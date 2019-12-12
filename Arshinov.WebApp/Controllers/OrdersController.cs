using Arshinov.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arshinov.WebApp.Controllers
{
    /*[Authorize]*/
    public class OrdersController : Controller
    {
        private OrdersService _ordersService;
        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        // GET
        public IActionResult Index()
        {
            return View(_ordersService.GetAll());
        }
        
        [HttpPost]
        public void Update()
        {
            _ordersService.Add(new Order() {Price = 215, User = new User()});
        }
    }
}