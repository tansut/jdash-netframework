using JDash.NetFramework.Api.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace JDash.NetFramework.Api.Routing
{

    public class JDashAssembliesResolver : DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
            var controllersAssembly = Assembly.GetExecutingAssembly();
            baseAssemblies.Add(controllersAssembly);
            return assemblies;
        }
    }

    public class Configurator
    {
        private static string _endpoint;
       

        public static void UseJDash<TJDashConfigurator>(string endpoint = null)
            where TJDashConfigurator : BaseJDashConfigurator
        {
            Configuration.ConfiguratorType = typeof(TJDashConfigurator);
            if (endpoint == null)
                endpoint = "jdash/api/v1/"; 

            _endpoint = endpoint;

            GlobalConfiguration.Configuration.Services.Replace(typeof(IAssembliesResolver), new JDashAssembliesResolver());

            // Web API routes
            RouteTable.Routes.MapHttpRoute(
                    "JDashApi" + "Status",
                    _endpoint + "status",
                   new { controller = "JDashApi", action = "status" }
            );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "MyDashboards",
                 _endpoint + "dashboard/my",
                 new { controller = "JDashApi", action = "MyDashboards" }
            );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "CreateDashboard",
                 _endpoint + "dashboard/create",
                 new { controller = "JDashApi", action = "CreateDashboard" }
             );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "SearchDashboards",
                 _endpoint + "dashboard/search",
                 new { controller = "JDashApi", action = "SearchDashboards" }
            );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "DeleteDashboard",
                 _endpoint + "dashboard/delete/{id}",
                 new { controller = "JDashApi", action = "DeleteDashboard" }
            );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "SaveDashboard",
                 _endpoint + "dashboard/save/{id}",
                 new { controller = "JDashApi", action = "SaveDashboard" }
            );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "GetDashboard",
                 _endpoint + "dashboard/{id}",
                 new { controller = "JDashApi", action = "GetDashboard" }
            );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "DeleteDashlet",
                 _endpoint + "dashlet/delete/{id}",
                 new { controller = "JDashApi", action = "DeleteDashlet" }
            );

            RouteTable.Routes.MapHttpRoute(
              "JDashApi" + "CreateDashlet",
               _endpoint + "dashlet/create",
               new { controller = "JDashApi", action = "CreateDashlet" }
            );

            RouteTable.Routes.MapHttpRoute(
                "JDashApi" + "SaveDashlet",
                 _endpoint + "dashlet/save/{id}",
                new { controller = "JDashApi", action = "SaveDashlet" }
             );

        }
 
    }
}