using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 
namespace Open24.Areas.AdminPage.Hellper
{
    public static class Utilities
    {
        public static string IsActive(this HtmlHelper html,
                                  string control,
                                  string action)
        {
            var routeData = html.ViewContext.RouteData;

            // both must match
            var returnActive = control.ToLower() == (string)routeData.Values["controller"].ToString().ToLower() &&
                               action.ToLower() == (string)routeData.Values["action"].ToString().ToLower();

            return returnActive ? "undore" : "";
        }
    }
}