using System.Web.Http;
using System.Web.Mvc;

namespace Open24.Areas.AdminPage
{
    public class AdminPageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AdminPage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.MapHttpRoute(
                name: "Open24WebApi",
                routeTemplate: "Open24Api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );


            //context.MapRoute(
            //    name: "admin",
            //    url: "admin", 
            //    defaults: new { controller = "Post", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Open24.Areas.AdminPage.Controllers" }
            //);
            //context.MapRoute(
            //    name: "adminAddNews",
            //    url: "admin/them-tin-tuc",
            //    defaults: new { controller = "Post", action = "AddPost", id = UrlParameter.Optional },
            //    namespaces: new[] { "Open24.Areas.AdminPage.Controllers" }
            //);
            //context.MapRoute(
            //    name: "adminAddNewsRecruit",
            //    url: "admin/them-tin-tuyen-dung",
            //    defaults: new { controller = "Post", action = "AddPost1", id = UrlParameter.Optional },
            //    namespaces: new[] { "Open24.Areas.AdminPage.Controllers" }
            //);
            context.MapRoute(
                "AdminPage_default",
                "AdminPage/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new [] { "Open24.Areas.AdminPage.Controllers" }
            );
        }
    }
}