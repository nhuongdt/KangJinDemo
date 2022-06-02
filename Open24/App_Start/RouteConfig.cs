using Model_banhang24vn.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Open24
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*RobotsHandler}", new { RobotsHandler = @"(.*/)?robots.txt(/.*)?" });
            routes.IgnoreRoute("{*SitemapHandler}", new { SitemapHandler = @"(.*/)?sitemap.xml(/.*)?" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                        name: "HomeIndex",
                        url: "",
                        defaults: new { controller = "Open24", action = "Index", id = UrlParameter.Optional },
                          namespaces: new[] { "Open24.Controllers" }
                    );
            routes.MapRoute(
                name: "phidv",
                url: "bang-gia-phan-mem", // URL with parameters
                defaults: new { controller = "Home", action = "PhiDV" },
                namespaces: new[] { "Open24.Controllers" }
            );

            routes.MapRoute(
                name: "contact",
                 url: "lien-he", // URL with parameters
                defaults: new { controller = "Home", action = "Contact" },
                namespaces: new[] { "Open24.Controllers" }
            );

            routes.MapRoute(
               name: "khachhang",
                 url: "khach-hang", // URL with parameters
                defaults: new { controller = "Open24", action = "KhachHang" },
                namespaces: new[] { "Open24.Controllers" }
            );

            routes.MapRoute(
               name: "gioithieu",
                 url: "gioi-thieu", // URL with parameters
                defaults: new { controller = "Home", action = "GioiThieu" },
                namespaces: new[] { "Open24.Controllers" }
            );
            routes.MapRoute(
             name: "dangkydungthuopen",
               url: "dung-thu-mien-phi", // URL with parameters
              defaults: new { controller = "Home", action = "DangKyVersion1" },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
            name: "dangkydungthuthanhcong",
              url: "dang-ky-thanh-cong", // URL with parameters
             defaults: new { controller = "Home", action = "DangKyThanhCongVersion1" },
             namespaces: new[] { "Open24.Controllers" }
         );
            routes.MapRoute(
               name: "tintuc",
                 url: "tin-tuc", // URL with parameters
                defaults: new { controller = "Open24", action = "tintuc" },
                namespaces: new[] { "Open24.Controllers" }
            );

            routes.MapRoute(
                 name: "LandingPage",
                   url: "LandingPage", // URL with parameters
                  defaults: new { controller = "Open24", action = "LandingPage" },
                  namespaces: new[] { "Open24.Controllers" }
              );
            routes.MapRoute(
               name: "dieukhoan",
                 url: "dieu-khoan", // URL with parameters
                defaults: new { controller = "Home", action = "DieuKhoan" },
                namespaces: new[] { "Open24.Controllers" }
            );

            routes.MapRoute(
              name: "hotrophanmem",
                url: "huong-dan-su-dung", // URL with parameters
               defaults: new { controller = "Home", action = "HoTroPhanMem" },
               namespaces: new[] { "Open24.Controllers" }
           );
            routes.MapRoute(
             name: "hotrotimkiem",
               url: "ho-tro-tim-kiem", // URL with parameters
              defaults: new { controller = "Home", action = "SearchHoTroPhanMem", id = UrlParameter.Optional },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
             name: "hotrochitiet",
               url: "nhom-vai-tro/{id}", // URL with parameters
              defaults: new { controller = "Home", action = "HoTroChiTiet", keyId = (int)Notification.HoTroNhom.theonhom, id = UrlParameter.Optional },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
             name: "hotrochitiet1",
               url: "huong-dan-su-dung/{keyId}/{id}/{title}", // URL with parameters
              defaults: new { controller = "Home", action = "HoTroChiTiet", id = UrlParameter.Optional },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
           name: "gioitieukhachhang",
           url: "gioi-thieu-khach-hang", // URL with parameters
           defaults: new { controller = "Open24", action = "HopTac" },
           namespaces: new[] { "Open24.Controllers" }
       );
            //      routes.MapRoute(
            //        name: "nhahang",
            //        url: MaNganhNgheKinhDoanh.TitleNhaHangCafe, // URL with parameters
            //         defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.NhaHangCafe },
            //         namespaces: new[] { "Open24.Controllers" }
            //     );
            //      routes.MapRoute(
            //        name: "spathammy",
            //        url: MaNganhNgheKinhDoanh.TitleSpaTMV, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.SpaTMV },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //        name: "salon",
            //       url: MaNganhNgheKinhDoanh.TitleSalonGym, // URL with parameters
            //      defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.SalonGym },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //        name: "thoitrang",
            //          url: MaNganhNgheKinhDoanh.TitleThoiTrangPK, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.ThoiTrangPK },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //       name: "nhathuoc",
            //        url: MaNganhNgheKinhDoanh.TitleNhaThuoc, // URL with parameters
            //       defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.NhaThuoc },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //        name: "sieuthi",
            //         url: MaNganhNgheKinhDoanh.TitleSieuThi, // URL with parameters
            //         defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.SieuThi },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //       name: "congnghe",
            //         url: MaNganhNgheKinhDoanh.TitleThietBiCongNghe, // URL with parameters
            //      defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.ThietBiCongNghe },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //      name: "noithat",
            //        url: MaNganhNgheKinhDoanh.TitleNoiThat, // URL with parameters
            //       defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.NoiThat },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );

            //      routes.MapRoute(
            //        name: "vanphongpham",
            //        url: MaNganhNgheKinhDoanh.TitleVanPhongPham, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.VanPhongPham },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //        name: "mypham",
            //          url: MaNganhNgheKinhDoanh.TitleMyPham, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.MyPham },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //         name: "giadung",
            //        url: MaNganhNgheKinhDoanh.TitleGiaDungDienMay, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.GiaDungDienMay },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //        name: "dochoi",
            //          url: MaNganhNgheKinhDoanh.TitleDoChoiTreEm, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.DoChoiTreEm },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //      name: "phuongtien",
            //        url: MaNganhNgheKinhDoanh.TitleOtoXeMay, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.OtoXeMay },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //        name: "nongsan",
            //         url: MaNganhNgheKinhDoanh.TitleNongSanTP, // URL with parameters
            //        defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.NongSanTP },
            //        namespaces: new[] { "Open24.Controllers" }
            //    );
            //      routes.MapRoute(
            //    name: "Other",
            //     url: MaNganhNgheKinhDoanh.TitleOther, // URL with parameters
            //    defaults: new { controller = "Home", action = "TinhNang", keyId = MaNganhNgheKinhDoanh.Other },
            //    namespaces: new[] { "Open24.Controllers" }
            //);

            routes.MapRoute(
          name: "nhahang",
          url: MaNganhNgheKinhDoanh.TitleNhaHangCafe, // URL with parameters
           defaults: new { controller = "open24", action = "NhaHang"},
           namespaces: new[] { "Open24.Controllers" }
       );
            routes.MapRoute(
              name: "spathammy",
              url: MaNganhNgheKinhDoanh.TitleSpaTMV, // URL with parameters
              defaults: new { controller = "open24", action = "Spa"},
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "salon",
             url: MaNganhNgheKinhDoanh.TitleSalonGym, // URL with parameters
            defaults: new { controller = "open24", action = "Salon"},
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "thoitrang",
                url: MaNganhNgheKinhDoanh.TitleThoiTrangPK, // URL with parameters
              defaults: new { controller = "open24", action = "ThoiTrang" },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
             name: "nhathuoc",
              url: MaNganhNgheKinhDoanh.TitleNhaThuoc, // URL with parameters
             defaults: new { controller = "open24", action = "NhaThuoc"},
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
           name: "phongkham",
            url: MaNganhNgheKinhDoanh.TitlePhongKham, // URL with parameters
           defaults: new { controller = "open24", action = "PhongKham"},
            namespaces: new[] { "Open24.Controllers" }
        );
            routes.MapRoute(
              name: "sieuthi",
               url: MaNganhNgheKinhDoanh.TitleSieuThi, // URL with parameters
               defaults: new { controller = "open24", action = "SieuThi"},
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
            name: "noithat",
              url: MaNganhNgheKinhDoanh.TitleNoiThat, // URL with parameters
             defaults: new { controller = "open24", action = "NoiThat"},
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "vanphongpham",
              url: MaNganhNgheKinhDoanh.TitleVanPhongPham, // URL with parameters
              defaults: new { controller = "open24", action = "VanPhongPham" },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "mypham",
                url: MaNganhNgheKinhDoanh.TitleMyPham, // URL with parameters
              defaults: new { controller = "open24", action = "MyPham" },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "dochoi",
                url: MaNganhNgheKinhDoanh.TitleDoChoiTreEm, // URL with parameters
              defaults: new { controller = "open24", action = "DoChoi" },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
            name: "phuongtien",
              url: MaNganhNgheKinhDoanh.TitleOtoXeMay, // URL with parameters
              defaults: new { controller = "open24", action = "Garaoto" },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
            name: "nhansu",
              url: MaNganhNgheKinhDoanh.TitleNhanSu, // URL with parameters
              defaults: new { controller = "open24", action = "NhanSu" },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
          name: "phutung",
            url: MaNganhNgheKinhDoanh.TitlePhuTung, // URL with parameters
            defaults: new { controller = "open24", action = "PhuTung" },
            namespaces: new[] { "Open24.Controllers" }
        );
            routes.MapRoute(
              name: "nongsan",
               url: MaNganhNgheKinhDoanh.TitleNongSanTP, // URL with parameters
              defaults: new { controller = "open24", action = "NongSan"},
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
          name: "Other",
           url: MaNganhNgheKinhDoanh.TitleOther, // URL with parameters
          defaults: new { controller = "open24", action = "LinhVucKhac" },
          namespaces: new[] { "Open24.Controllers" }
      );

          

            routes.MapRoute(
                name: "detailnews",
                url: "tin-tuc/{title}.html", // URL with parameters
                defaults: new { controller = "Open24", action = "TinTucChiTiet", id = UrlParameter.Optional },
                namespaces: new[] { "Open24.Controllers" }
            );
            routes.MapRoute(
               name: "tags",
               url: "tag/{tagId}", // URL with parameters
               defaults: new { controller = "Open24", action = "TinTucTags", id = UrlParameter.Optional },
               namespaces: new[] { "Open24.Controllers" }
           );
            routes.MapRoute(
               name: "Groupnews",
               url: "blog/{Category}", // URL with parameters
               defaults: new { controller = "Open24", action = "TinTucGroup", id = UrlParameter.Optional },
               namespaces: new[] { "Open24.Controllers" }
           );
            routes.MapRoute(
              name: "OrderSucces",
              url: "dat-hang-thanh-cong/{ID}", // URL with parameters
              defaults: new { controller = "Home", action = "OrderSuccess", id = UrlParameter.Optional },
              namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "KhachHangDetail",
              url: "khach-hang/{title}.html", // URL with parameters
              defaults: new { controller = "Open24", action = "KhachHangChiTiet", id = UrlParameter.Optional },
                namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
           name: "SanPhamDetail",
           url: "san-pham/{keyId}/{title}", // URL with parameters
           defaults: new { controller = "Home", action = "Cart", id = UrlParameter.Optional },
             namespaces: new[] { "Open24.Controllers" }
       );
            routes.MapRoute(
             name: "GioHang",
             url: "Gio-hang", // URL with parameters
             defaults: new { controller = "Home", action = "GioHang", id = UrlParameter.Optional },
               namespaces: new[] { "Open24.Controllers" }
         );
            routes.MapRoute(
           name: "Thanhtoan",
           url: "thanh-toan", // URL with parameters
           defaults: new { controller = "Home", action = "thanhtoan", id = UrlParameter.Optional },
           namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "DangKy",
              url: "dang-ky", // URL with parameters
              defaults: new { controller = "Home", action = "DangKyVersion1", id = UrlParameter.Optional },
                namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
              name: "DangKyThanhCong",
              url: "dang-ky-thanh-cong/{id}", // URL with parameters
              defaults: new { controller = "Home", action = "Success", id = UrlParameter.Optional },
                namespaces: new[] { "Open24.Controllers" }
          );
            routes.MapRoute(
             name: "dungthuthanhcong",
             url: "dang-ky-dung-thu-thanh-cong", // URL with parameters
             defaults: new { controller = "Open24", action = "thanhcong"},
               namespaces: new[] { "Open24.Controllers" }
         );
         //   routes.MapRoute(
         //    name: "hotrophanmembanhang24",
         //    url: "ho-tro-phan-mem-open24", // URL with parameters
         //    defaults: new { controller = "Home", action = "HotroBanhang24", id = UrlParameter.Optional },
         //      namespaces: new[] { "Open24.Controllers" }
         //);
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Open24", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Open24.Controllers" }
            );
        }
    }
}
