using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Crunchers.Models;
using Microsoft.AspNet.Identity;

namespace Crunchers.Controllers
{
    public class CityController : Controller
    { 
        public class CityResponse
        {
            public IEnumerable<CityModel> Cities;
            public IEnumerable<PointsOfPickUpModel> PointsOfPickUp;
            public bool IsHavePickUpPoints;
            public string City;
        }
        public async Task<ActionResult> Index(string currentCity="")
        {
            var cities = await new CityModel().GetAllCities();

            if (!cities.Any(x => x.NameRu == currentCity))
            {
                return View(new CityResponse() {Cities = cities, IsHavePickUpPoints = false});
            }

            var city = cities.Where(x => x.NameRu == currentCity).FirstOrDefault();
            var pointsOfPickUp = await new PointsOfPickUpModel().GetPointsOfPickUpByCityId(city.CityId);
            if (!pointsOfPickUp.Any())
            {
                return View(new CityResponse() {Cities = cities, IsHavePickUpPoints = false});
            }

            return View(new CityResponse()
                {Cities = cities, IsHavePickUpPoints = true, PointsOfPickUp = pointsOfPickUp,City = currentCity});
        }

        [System.Web.Http.HttpPost]
        public ActionResult ChangeUsersCity(int cityId)
        {
            new UserModel().ChangeUserInfo(User.Identity.GetUserId(),cityId,"CityId");
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

    }
}