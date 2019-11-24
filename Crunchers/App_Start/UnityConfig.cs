using System.Data.Common;
using System.Web.Http;
using System.Web.Mvc;
using Crunchers;
using Crunchers.Controllers;
using Crunchers.Models;
using Npgsql;
using Unity;
using Unity.WebApi;

namespace Crunchers
{
    public static class UnityConfig
    {
        public static UnityContainer RegisterComponents()
        {
			var container = new UnityContainer();
            //Add database into container 
            container.RegisterType<DbConnection, NpgsqlConnection>();
            container.RegisterType<DbCommand, NpgsqlCommand>();
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
            return container;
        }
    }
}