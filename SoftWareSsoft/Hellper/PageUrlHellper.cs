using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoftWareSsoft.Hellper
{
    public static class PageUrlHellper
    {
        public static string IsActive(this HtmlHelper html,
                             string control,
                             string rawurl)
        {
            var routeData = html.ViewContext.RouteData;

            // both must match
            var returnActive = control == (string)routeData.Values["controller"] &&
                               rawurl == html.ViewContext.RequestContext.HttpContext.Request.RawUrl;

            return returnActive ? "menu-active-page" : "";
        }
    }
    public class SsoftPageUrl
    {
        public const string TinTuc = "tin-tuc";
        public const string SanPham = "san-pham";
        public const string LuckyBeauty = "phan-mem-lucky-beauty";
        public const string LuckyGara = "phan-mem-lucky-gara";
        public const string LuckyHrm = "phan-mem-lucky-hrm";
        public const string LienHe = "lien-he";
    }
  
}