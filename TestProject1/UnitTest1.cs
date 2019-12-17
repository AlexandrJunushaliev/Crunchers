using System;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Mvc;
using Crunchers;
using Crunchers.Models;
using NUnit.Framework;
using Unity;

namespace TestProject1
{
    /*public class MyApplication : MvcApplication
    {
        protected new void Application_Start()
        {
            Container = UnityConfig.RegisterComponents();
            AreaRegistration.RegisterAllAreas();
            
        }
    }*/

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            MvcApplication mvcApplication = new MvcApplication();
            MvcApplication.Container = UnityConfig.RegisterComponents();
            /*var configuration = WebConfigurationManager.OpenMachineConfiguration();
            configuration.ConnectionStrings.ConnectionStrings["ShopDbConnection"].ConnectionString =
                @"Server=testcrunchers.postgres.database.azure.com;Database={your_database};Port=5432;User Id=postgres@testcrunchers;Password=321nimdA;Ssl Mode=Require;";
            configuration.Save();*/

            string create = @"CREATE SEQUENCE public.""Categories_CategoryId_seq""
    INCREMENT 1
    START 25
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;

ALTER SEQUENCE public.""Categories_CategoryId_seq""
    OWNER TO postgres;
CREATE TABLE public.""Categories""
(
    ""CategoryId"" integer NOT NULL DEFAULT nextval('""Categories_CategoryId_seq""'::regclass),
    ""CategoryName"" character varying(255) COLLATE pg_catalog.""default"",
    CONSTRAINT ""Categories_pkey"" PRIMARY KEY (""CategoryId"")
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.""Categories""
    OWNER to postgres;

-- Index: ""Categories""_""CategoryName""_uindex

-- DROP INDEX public.""""""Categories""""_""""CategoryName""""_uindex"";

CREATE UNIQUE INDEX """"""Categories""""_""""CategoryName""""_uindex""
    ON public.""Categories"" USING btree
    (""CategoryName"" COLLATE pg_catalog.""default"")
    TABLESPACE pg_default;";


            var _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            var _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            typeof(ConfigurationElementCollection)
                .GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(ConfigurationManager.ConnectionStrings, false);
            WebConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("ShopDbConnection",
                @"Server=testcrunchers.postgres.database.azure.com;Database=shopdb;Port=5432;User Id=postgres@testcrunchers;Password=321nimdA;Ssl Mode=Require;"));

            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;

            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = create;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }

        [Test]
        public void Test1()
        {
            new CategoryModel().AddCategory("Category1");
            var categories = new CategoryModel().GetCategories().Result;
            Assert.IsNotEmpty(categories);
            Assert.AreEqual(categories.First().CategoryName, "Category1");
            new CategoryModel().DeleteCategory(categories.First().CategoryId);
            Assert.IsEmpty(new CategoryModel().GetCategories().Result);


            string create = @"DROP INDEX public.""""""Categories""""_""""CategoryName""""_uindex"";
DROP TABLE public.""Categories"";
DROP SEQUENCE public.""Categories_CategoryId_seq"";";


            var _dbCommand = MvcApplication.Container.Resolve<DbCommand>();
            var _dbConnection = MvcApplication.Container.Resolve<DbConnection>();
            _dbConnection.ConnectionString =
                WebConfigurationManager.ConnectionStrings["ShopDbConnection"].ConnectionString;

            using (_dbConnection)
            {
                _dbConnection.Open();
                _dbCommand.CommandText = create;
                _dbCommand.Connection = _dbConnection;
                _dbCommand.ExecuteNonQuery();
                _dbConnection.Close();
            }
        }
    }
}