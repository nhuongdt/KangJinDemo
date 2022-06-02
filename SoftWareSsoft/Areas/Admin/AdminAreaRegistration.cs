using System.Web.Http;
using System.Web.Mvc;

namespace SoftWareSsoft.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.MapHttpRoute(
               name: "SsoftApi",
               routeTemplate: "SsoftApi/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
               );
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "SoftWareSsoft.Areas.Admin.Controllers" }

            );
        }
    }
}