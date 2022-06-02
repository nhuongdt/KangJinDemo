using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using System.Net.Http;

namespace banhang24.Areas.DanhMuc
{
    public class DanhMucAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DanhMuc";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute(
                name: "AreaNameWebApi",
                routeTemplate: string.Concat("api/DanhMuc/{controller}/{action}/{id}"),
                defaults: new { id = RouteParameter.Optional }
            );
            context.Routes.MapHttpRoute(
                name: "AreaNameWebApi2",
                routeTemplate: string.Concat("api/DanhMuc/{controller}/{id}"),
                defaults: new { action = "get", id = RouteParameter.Optional }
            );
            context.MapRoute(
                "DanhMuc_default",
                "DanhMuc/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
