    using libHT_NguoiDung;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using banhang24.Hellper;

namespace banhang24.Controllers
{
    [App_Start.App_API.CheckwebAuthorize]
    public class BanHangController : Controller
    {
        //[OutputCache(Location = OutputCacheLocation.ServerAndClient, Duration = int.MaxValue)]
        public ActionResult _getHoaDonOffline()
        {
            return PartialView();
        }

        [RBACAuthorize(RoleKey = RoleKey.NhaBep_TruyCap)]
        public ActionResult Kitchen()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    ViewBag.cookieDonVi = objUser_Cookies.ID_DonVi;
                    ViewBag.cookieIDUser = objUser_Cookies.ID;
                    ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        public ActionResult POSDisplay(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                var subDomain = CookieStore.GetCookieAes("SubDomain");
                if (objUser_Cookies != null)
                {
                    ViewBag.cookieDonVi = objUser_Cookies.ID_DonVi;
                    ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                    ViewBag.SubDomain = subDomain;
                    ViewBag.imgaddr = id == null? "" : id;
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        public ActionResult BanLe()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                var subDomain = CookieStore.GetCookieAes("SubDomain");
                if (objUser_Cookies != null)
                {
                    string strIDDonVi = CookieStore.GetCookieAes("IdDonVi");
                    if (strIDDonVi == null || strIDDonVi == "")
                    {
                        strIDDonVi = objUser_Cookies.ID_DonVi.ToString();
                    }

                    ViewBag.cookieDonVi = strIDDonVi;
                    ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                    ViewBag.SubDomain = subDomain;
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        public ActionResult _CreateAccountBank()
        {
            return PartialView();
        }
        public ActionResult Nhahang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                var subDomain = CookieStore.GetCookieAes("SubDomain");
                if (objUser_Cookies != null)
                {
                    ViewBag.cookieDonVi = objUser_Cookies.ID_DonVi;
                    ViewBag.cookieIDUser = objUser_Cookies.ID;
                    ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;
                    ViewBag.cookieIDNhanVien = objUser_Cookies.ID_NhanVien;
                    ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                    ViewBag.SubDomain = subDomain;
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }
    }
}
