using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
//using System.Web.Http;
using System.Web.Mvc;
using libDM_DoiTuong;
using Model;
using Newtonsoft.Json.Linq;
using libHT_NguoiDung;
using banhang24.Hellper;

namespace banhang24.Controllers
{
    [App_Start.App_API.CheckwebAuthorize]
    public class DoiTacController : Controller
    {
        [RBACAuthorize(RoleKey = RoleKey.KhachHang_XemDs)]
        public ActionResult Khachhang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    string apiUrl = Url.HttpRouteUrl("DefaultApi", new { controller = "DoiTac" });
                    ViewBag.LoaiDoiTuong = 1;
                    ViewBag.ApiUrl = new Uri(Request.Url, apiUrl).AbsoluteUri.ToString();
                    ViewBag.Userlogin = objUser_Cookies.TaiKhoan;
                    ViewBag.UserID = objUser_Cookies.ID;
                    ViewBag.IDNhanVien = objUser_Cookies.ID_NhanVien;
                    ViewBag.IDDonVi = objUser_Cookies.ID_DonVi;
                    ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.NhaCungCap_XemDs)]
        public ActionResult Nhacungcap()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    string apiUrl = Url.HttpRouteUrl("DefaultApi", new { controller = "DoiTac" });
                    ViewBag.LoaiDoiTuong = 2;
                    ViewBag.ApiUrl = new Uri(Request.Url, apiUrl).AbsoluteUri.ToString();
                    ViewBag.Userlogin = objUser_Cookies.TaiKhoan;
                    ViewBag.IDNhanVien = objUser_Cookies.ID_NhanVien;
                    ViewBag.UserID = objUser_Cookies.ID;
                    ViewBag.IDDonVi = objUser_Cookies.ID_DonVi;
                    ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }
        public ActionResult _ThemMoiKhachHang()
        {
            return PartialView();
        }
        public ActionResult _ThemMoiNCC()
        {
            return PartialView();
        }
        public ActionResult _ChuyenNhom()
        {
            return PartialView();
        }
        public ActionResult _ThemMoiNguonKhach()
        {
            return PartialView();
        }
        public ActionResult NhaBaoHiem()
        {
            return View();
        }

        [RBACAuthorize(RoleKey = RoleKey.HoaHongKhachGioiThieu_XemDS)]
        public ActionResult HoaHongKhachGioiThieu()
        {
            return View();
        }
    }
}