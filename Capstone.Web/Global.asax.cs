using Ninject;
using Ninject.Web.Common.WebHost;
using System;
using System.Collections.Generic;
using Capstone;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using StockGameService.Mock;

namespace Capstone.Web
{
    public class MvcApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            base.OnApplicationStarted();

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            // Bind Database
            //These commented out lines are for a mock if we ever decide to make one #springbreak
            //kernel.Bind<IStockGameDAL>().To<MockStockGameDal>();
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string connectionString = ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString;
            kernel.Bind<IStockGameDAL>().To<StockGameDAL>().WithConstructorArgument("connectionString", connectionString);



            return kernel;
        }
    }
}