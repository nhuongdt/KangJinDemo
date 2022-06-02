using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace banhang24.Hellper
{
    public class RBACAuthorize : AuthorizeAttribute
    {
        public string RoleKey { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // hàm check xem người dùng còn tồn tại không
            if (!contant.checkUser())
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.HttpContext.Response.StatusCode = 403;
                //filterContext.Result = new JsonResult
                //{
                //    Data = new
                //    {
                //        Error = "NotAuthorized",
                //        LogOnUrl = "/Login",
                //    },
                //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                //};
                filterContext.Result = new RedirectResult("/Login");
            }
            else
            {
                base.OnAuthorization(filterContext);
            }

        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Nếu không truyền key default là check xem tồn tại người dùng ko
            if (string.IsNullOrEmpty(RoleKey))
            {
                return true;
            }
            else // hàm check quyền trong DB
            {
                return contant.CheckRolePermission(RoleKey);
            }
        }
        // nếu không có quyền return ra trang cần hiển thị
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.HttpContext.Response.StatusCode = 403;
            filterContext.Result = new JsonResult
            {
                Data = new
                {
                    Error = "NotAuthorized",
                    LogOnUrl = "#/DashBoard"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}