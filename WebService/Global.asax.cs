using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Sqloogle.Libs.NLog;

namespace WebService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly Logger _logger = LogManager.GetLogger("Global");

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ViewEngines.Engines.Clear();
        }

        void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            var response = new {success = false, message = exception.Message};
            
            _logger.Error(exception.Message);

            Server.ClearError();

            var callback = Request.QueryString.AllKeys.Any(k => k == "callback") ? Request.QueryString.Get("callback") : string.Empty;
            Response.ContentType = String.IsNullOrEmpty(callback) ? "text/plain" : "text/javascript";
            Response.StatusCode = 500;
            Response.Write(
                string.IsNullOrEmpty(callback) ?
                System.Web.Helpers.Json.Encode(response) :
                string.Format("{0}({1});", callback, System.Web.Helpers.Json.Encode(response))
            );
                
        }

    }
}