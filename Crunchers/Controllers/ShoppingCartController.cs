using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Crunchers.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Crunchers.Controllers
{
    class ProductComparer : IEqualityComparer<ProductModel>
    {
        public bool Equals(ProductModel x, ProductModel y)
        {
            if (y == null && x == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.ProductId == y.ProductId;
        }

        public int GetHashCode(ProductModel obj)
        {
            return obj.ProductId.GetHashCode();
        }
    }

    public class ShoppingCartController : Controller
    {
        public class ShoppingCartResponse
        {
            public Dictionary<int, int> LocalCart;
            public Dictionary<int, int> Cart;
            public IEnumerable<ProductModel> CartProducts;
        }


        public async Task<ActionResult> Index(string localCartJson)
        {
            if (localCartJson == "null")
            {
                localCartJson = "{}";
            }

            var shoppingCart = await new ShoppingCartModel().GetCartJson(User?.Identity?.GetUserId());
            var cart = JsonConvert.DeserializeObject<Dictionary<int, int>>(shoppingCart.CartJson);
            var localCart = JsonConvert.DeserializeObject<Dictionary<int, int>>(localCartJson);
            if (localCart.Values.Any(x => x <= 0))
            {
                return View(new ShoppingCartResponse()
                    {LocalCart = localCart, Cart = cart, CartProducts = new ProductModel[0]});
            }

            var cartProducts = await new ProductModel().GetPrimaryProductsInfo(cart.Keys);
            var localCartProducts = await new ProductModel().GetPrimaryProductsInfo(localCart.Keys);
            var unionProducts = cartProducts.Union(localCartProducts, new ProductComparer());
            var response = new ShoppingCartResponse()
                {LocalCart = localCart, Cart = cart, CartProducts = unionProducts};
            return View(response);
        }

        [System.Web.Http.HttpPost]
        public ActionResult ChangeCart(int productId, int value)
        {
            new ShoppingCartModel().ChangeCart(User.Identity.GetUserId(), value, productId);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}