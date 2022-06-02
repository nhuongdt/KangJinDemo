using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using banhang24.App_API;

namespace banhang24
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*BarCodeHandler}", new { BarCodeHandler = @"(.*/)?Barcode.ashx(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.Add(new SubdomainRoute());

            routes.MapRoute(
                name: "HomeActions",
                url: "",
                defaults:  new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "HangHoaActions",
                url: "p/{action}",
                defaults: new
                {
                    controller = "HangHoa",
                    action = "danhsachhanghoa"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "GiaoDichActions",
                url: "e/{action}",
                defaults: new
                {
                    controller = "GiaoDich",
                    action = "HoaDon"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "DoiTacActions",
                url: "c/{action}",
                defaults: new
                {
                    controller = "DoiTac",
                    action = "KhachHang"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );
            routes.MapRoute(
               name: "Report",
               url: "x/{action}",
               defaults: new
               {
                   controller = "Report",
                   action = "Index"
               }
                , namespaces: new[] { "banhang24.Controllers" }
           );

            routes.MapRoute(
                name: "TuVanActions",
                url: "s/{action}",
                defaults: new
                {
                    controller = "TuVan",
                    action = "Index"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "SoQuyActions",
                url: "f/{action}",
                defaults: new
                {
                    controller = "SoQuy",
                    action = "Index"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "marketing",
                url: "mark/{action}",
                defaults: new
                {
                    controller = "Marketing",
                    action = "OptinForm"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );
            routes.MapRoute(
                name: "BaoCaoActions",
                url: "r/{action}",
                defaults: new
                {
                    controller = "BaoCao",
                    action = "Index"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "BanHangActions",
                url: "$/{action}/{id}",
                defaults: new
                {
                    controller = "BanHang",
                    action = "HoaDon",
                    id = UrlParameter.Optional
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );
            routes.MapRoute(
                name: "NhanSuAction",
                url: "n/{action}",
                defaults: new
                {
                    controller = "NhanSu",
                    action = "CaLamViec"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );
            routes.MapRoute(
                name: "SpaActions",
                url: "b/{action}",
                defaults: new
                {
                    controller = "Spa",
                    action = "Spa"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "ThietLapActions",
                url: "t/{action}",
                defaults: new
                {
                    controller = "ThietLap",
                    action = "ThietLapChung"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "PhongBanActions",
                url: "m/{action}",
                defaults: new
                {
                    controller = "PhongBan",
                    action = "PhongBan"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "GaraActions",
                url: "g/{action}",
                defaults: new
                {
                    controller = "Gara",
                    action = "DanhMucLoaiXe"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );

            routes.MapRoute(
                name: "SharedActions",
                url: "Shared/{action}",
                defaults: new
                {
                    controller = "Shared",
                    action = "_header"
                }
                 , namespaces: new[] { "banhang24.Controllers" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                 , namespaces: new[] { "banhang24.Controllers" }
            );
        }
    }
}