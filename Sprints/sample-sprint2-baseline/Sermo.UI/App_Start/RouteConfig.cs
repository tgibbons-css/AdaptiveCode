using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sermo.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("");             // don't route the home page, index.html

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            // add route for default startup to jump to room list
            routes.MapRoute(
                name: "Root",
                url: "",
                defaults: new { controller = "Room", action = "List", id = UrlParameter.Optional }
            );

        }
    }
}
