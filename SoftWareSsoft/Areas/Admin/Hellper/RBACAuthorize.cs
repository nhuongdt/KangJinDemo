using SoftWareSsoft.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SoftWareSsoft.Areas.Admin.Hellper
{
    public class RBACAuthorize : AuthorizeAttribute
    {
        public string RoleKey { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (ContantaAdmin.GetSessionWorkUser() == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { Areas = "Admin", Controller = "Account", Action = "Login" }));
            }
            else
            {
                base.OnAuthorization(filterContext);
            }
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (string.IsNullOrEmpty(RoleKey))
            {
                return true;
            }
            else
            {
                return ContantaAdmin.CheckRole(RoleKey);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new
                      RouteValueDictionary(new { Areas = "Admin", Controller = "NonAuthorize", Action = "Index" }));
        }
    }
}