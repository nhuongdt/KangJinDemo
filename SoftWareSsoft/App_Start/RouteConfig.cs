using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ssoft.Common.Common;
using SoftWareSsoft.Hellper;

namespace SoftWareSsoft
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*RobotsHandler}", new { RobotsHandler = @"(.*/)?robots.txt(/.*)?" });
            routes.IgnoreRoute("{*SitemapHandler}", new { SitemapHandler = @"(.*/)?sitemap.xml(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
           // routes.MapRoute(
           //    name: "sitemap",
           //      url: "sitemap.xml", // URL with parameters
           //     defaults: new { controller = "ThemeSsoft", action = "SitemapXml" },
           //     namespaces: new[] { "SoftWareSsoft.Controllers" }
           // );
           // routes.MapRoute(
           //   name: "robots",
           //     url: "robots.txt", // URL with parameters
           //    defaults: new { controller = "ThemeSsoft", action = "RobotsText" },
           //    namespaces: new[] { "SoftWareSsoft.Controllers" }
           //);
            //---------- SSoft -----------//
            routes.MapRoute(
            name: "luckybeauty",
             url: "phan-mem-lucky-beauty", // URL with parameters
             defaults: new { controller = "ThemeSsoft", action = "ProductDetail", TypeId = (int)LibEnum.TypeSoftWareSsoft.beauty },
            namespaces: new[] { "SoftWareSsoft.Controllers" }
            );

            routes.MapRoute(
            name: "luckygara",
             url: "phan-mem-lucky-gara", // URL with parameters
             defaults: new { controller = "ThemeSsoft", action = "ProductDetail", TypeId = (int)LibEnum.TypeSoftWareSsoft.gara },
            namespaces: new[] { "SoftWareSsoft.Controllers" }
            );
            routes.MapRoute(
            name: "luckyhrm",
             url: "phan-mem-lucky-hrm", // URL with parameters
             defaults: new { controller = "ThemeSsoft", action = "ProductDetail", TypeId = (int)LibEnum.TypeSoftWareSsoft.hrm },
            namespaces: new[] { "SoftWareSsoft.Controllers" }
            );
            routes.MapRoute(
               name: "sanpham",
                url: "san-pham", // URL with parameters
                defaults: new { controller = "ThemeSsoft", action = "Product" },
               namespaces: new[] { "SoftWareSsoft.Controllers" }
               );
            routes.MapRoute(
              name: "lienhe",
               url: "lien-he", // URL with parameters
               defaults: new { controller = "ThemeSsoft", action = "Contact" },
              namespaces: new[] { "SoftWareSsoft.Controllers" }
              );
            routes.MapRoute(
             name: "tintuc",
              url: "tin-tuc", // URL with parameters
              defaults: new { controller = "ThemeSsoft", action = "News" },
             namespaces: new[] { "SoftWareSsoft.Controllers" }
             );
            routes.MapRoute(
             name: "tuyendung",
              url: "tuyen-dung", // URL with parameters
              defaults: new { controller = "ThemeSsoft", action = "Recruitment" },
             namespaces: new[] { "SoftWareSsoft.Controllers" }
             );
            routes.MapRoute(
            name: "chitiettuyendung",
             url: "tuyen-dung/{keyId}/{title}", // URL with parameters
             defaults: new { controller = "ThemeSsoft", action = "RecruitmentDetail" },
            namespaces: new[] { "SoftWareSsoft.Controllers" }
            );
            routes.MapRoute(
            name: "khachhang",
             url: "khach-hang", // URL with parameters
             defaults: new { controller = "ThemeSsoft", action = "Customer" },
            namespaces: new[] { "SoftWareSsoft.Controllers" }
            );
            routes.MapRoute(
               name: "chitietbaiviet",
               url: "tin-tuc/{keyId}/{title}", // URL with parameters
               defaults: new { controller = "ThemeSsoft", action = "NewsDetail", id = UrlParameter.Optional },
               namespaces: new[] { "SoftWareSsoft.Controllers" }
           );
            routes.MapRoute(
              name: "chitietkhachhang",
              url: "khach-hang/{keyId}/{title}", // URL with parameters
              defaults: new { controller = "ThemeSsoft", action = "CustomerDetail", id = UrlParameter.Optional },
              namespaces: new[] { "SoftWareSsoft.Controllers" }
          );
            routes.MapRoute(
            name: "gioithieu",
            url: "gioi-thieu-ve-ssoft", // URL with parameters
            defaults: new { controller = "ThemeSsoft", action = "Introduce", id = UrlParameter.Optional },
            namespaces: new[] { "SoftWareSsoft.Controllers" }
        );
            routes.MapRoute(
               name: "HomeIndex",
               url: "",
               defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                 namespaces: new[] { "SoftWareSsoft.Controllers" }
           );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                  namespaces: new[] { "SoftWareSsoft.Controllers" }
            );
        }
    }
}
