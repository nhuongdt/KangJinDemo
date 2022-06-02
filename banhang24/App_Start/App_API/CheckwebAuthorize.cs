using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace banhang24.App_Start.App_API
{
    public class CheckwebAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            int checkweb = Model_banhang24vn.M_DangKySuDung.CheckWeb(filterContext.RouteData.Values["subdomain"].ToString());
            //int checkweb = 0;
            switch (checkweb)
            {
                case 1:
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { Areas = "", Controller = "Home", Action = "RedirectToUrl" }));
                    break;
                case 2:
                    filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { Areas = "", Controller = "Home", Action = "Expire" }));
                    break;
                //case 3:
                //    filterContext.Result = new RedirectToRouteResult(new
                //    RouteValueDictionary(new { Areas = "", Controller = "Home", Action = "Active" }));
                //    break;
                default:
                    base.OnAuthorization(filterContext);
                    break;
            }
            
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return true;
        }
    }
}