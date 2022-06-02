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
using libDM_LoaiChungTu;
using libNS_NhanVien;


namespace banhang24.Controllers
{
    public class GiaoDichController : Controller
    {
        [App_Start.App_API.CheckwebAuthorize]

        [RBACAuthorize(RoleKey = RoleKey.HoaDon_XemDs)]
        public ActionResult HoaDon(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                // get cookie nganh nghe kinh doanh
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;
                ViewBag.LoaiHoaDon = id.Substring(0,1);
                if (objUser_Cookies != null)
                {
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.HoaDon_XemDs)]
        public ActionResult HoaDonSuaChua()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                // get cookie nganh nghe kinh doanh
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;

                if (objUser_Cookies != null)
                {
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.TraHang_XemDs)]
        public ActionResult TraHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();

                if (objUser_Cookies != null)
                {
                    ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;

                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.NhapHang_XemDs)]
        public ActionResult NhapHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                    string apiUrl = Url.HttpRouteUrl("DefaultApi", new { controller = "GiaoDich" });
                    ViewBag.ApiUrl = new Uri(Request.Url, apiUrl).AbsoluteUri.ToString();
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.TraHangNhap_XemDs)]
        public ActionResult TraHangNhap()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    string apiUrl = Url.HttpRouteUrl("DefaultApi", new { controller = "GiaoDich" });
                    ViewBag.ApiUrl = new Uri(Request.Url, apiUrl).AbsoluteUri.ToString();

                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.TraHangNhap_ThemMoi)]
        public ActionResult TraHangNhapChiTiet_2()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    string apiUrl = Url.HttpRouteUrl("DefaultApi", new { controller = "GiaoDich" });
                    ViewBag.ApiUrl = new Uri(Request.Url, apiUrl).AbsoluteUri.ToString();
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.NhapHang_XemDs)]
        public ActionResult NhapHangItem1_2()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                if (objUser_Cookies != null)
                {
                    ViewBag.LoaiHoaDon = 4;
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        public ActionResult _ThemMoiHangHoa()
        {
            return PartialView();
        }

        public ActionResult _ShowModalMessage()
        {
            return PartialView();
        }

        public ActionResult _modalDelete()
        {
            return PartialView();
        }

        [RBACAuthorize(RoleKey = RoleKey.ChuyenHang_XemDs)]
        public ActionResult ChuyenHang()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    ViewBag.LoaiHoaDon = 10;
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.ChuyenHang_XemDs)]
        public ActionResult ChuyenHangChiTiet_2()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                if (objUser_Cookies != null)
                {
                    ViewBag.LoaiHoaDon = 10;
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.XuatHuy_XemDs)]
        public ActionResult XuatHuy()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                if (objUser_Cookies != null)
                {
                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        public ActionResult PhieuXuatHuy()
        {
            return View();
        }

        public ActionResult XuatKhoChiTiet()
        {
            ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
            return View();
        }

        [RBACAuthorize(RoleKey = RoleKey.DatHang_XemDs)]
        public ActionResult DatHang(string id)
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                ViewBag.LoaiHoaDon = id.Substring(0, 1);
                if (objUser_Cookies != null)
                {
                    ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;

                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.DatHang_XemDs)]
        public ActionResult BaoGia()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                if (objUser_Cookies != null)
                {
                    ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;

                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }

        [RBACAuthorize(RoleKey = RoleKey.GoiDichVu_XemDs)]
        public ActionResult GoiDichVu()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                if (objUser_Cookies != null)
                {
                    ViewBag.cookieUserLogin = objUser_Cookies.TaiKhoan;

                    return View();
                }
                else
                {
                    return Redirect("/Login");
                }
            }
        }
        public ActionResult _popupbanggiaban()
        {
            return PartialView();
        }

        public ActionResult NapTienTheGiaTri()
        {
            using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
            {
                var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                var RoleModel = new RoleModel() { Delete = true, Export = true, Insert = true, Update = true, View = true, Print=true };
                if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                {
                    classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                    var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                    RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleKey.TheGiaTri_ThemMoi));
                    RoleModel.Update = listQuyen.Any(o => o.Equals(RoleKey.TheGiaTri_CapNhat));
                    RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleKey.TheGiaTri_Xoa));
                    RoleModel.Export = listQuyen.Any(o => o.Equals(RoleKey.TheGiaTri_XuatFile));
                    RoleModel.Print = listQuyen.Any(o => o.Equals(RoleKey.TheGiaTri_In));
                }
                return View(RoleModel);
            }
        }
        public ActionResult DanhSachXe()
        {
            return View();
        }
        public ActionResult DanhSachPhieuTiepNhan()
        {
            return View();
        }
    }
}
