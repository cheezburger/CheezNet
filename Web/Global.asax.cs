using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cheezburger.StarterKit
{
    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Home", "", new { controller = "Home", action = "Index" });
            routes.MapRoute("Auth.Begin", "login", new { controller = "Authorization", action = "BeginAuthorization" });
            routes.MapRoute("Auth.Complete", "cheezburger_login", new { controller = "Authorization", action = "CompleteAuthorization" });
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }
    }
}