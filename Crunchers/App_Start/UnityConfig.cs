using System.Web.Http;
using System.Web.Mvc;
using Crunchers;
using Crunchers.Controllers;
using Crunchers.Models;
using Unity;
using Unity.WebApi;

namespace Crunchers
{
    public static class UnityConfig
    {
        public static UnityContainer RegisterComponents()
        {
			var container = new UnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            return container;
        }
    }
}