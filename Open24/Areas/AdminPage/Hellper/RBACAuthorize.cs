using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
 
namespace Open24.Areas.AdminPage.Hellper
{
    public class RBACAuthorize: AuthorizeAttribute
    {
        public string RoleKey { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (contant.SESSIONNGUOIDUNG == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { Areas = "AdminPage", Controller = "Account", Action = "Index" }));
            }
            else
            {
                base.OnAuthorization(filterContext);
            }

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(RoleKey))
            {
                return true;
            }
            else
            {
                return contant.CheckRole(RoleKey);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new
                      RouteValueDictionary(new { Controller = "NonAuthorize", Action = "Index" }));
        }
    }
}