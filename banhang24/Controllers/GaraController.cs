using System;
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
    public class GaraController : Controller
    {
        public ActionResult DanhMucXe()
        {
            return View();
        }
        public ActionResult BanLamViec()
        {
            return View();
        }
        public ActionResult Gara()
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
        public ActionResult TongQuanGara()
        {
            return View();
        }
        public ActionResult Gara_DoanhThuCoVan()
        {
            return View("~/Views/Gara/BaoCao/Gara_DoanhThuCoVan.cshtml");
        }
        public ActionResult Gara_DoanhThSuaChua()
        {
            return View("~/Views/Gara/BaoCao/Gara_DoanhThuSuaChua.cshtml");
        }
        public ActionResult Gara_DoanhThuTongHop()
        {
            return View("~/Views/Gara/BaoCao/Gara_DoanhThuSuaChuaTongHop.cshtml");
        }
        public ActionResult Gara_DoanhThuBienSoXe()
        {
            return View("~/Views/Gara/BaoCao/Gara_DoanhThuTheoBienSoXe.cshtml");
        }
        public ActionResult Gara_LichNhacBaoDuong()
        {
            return View();
        }

        public ActionResult NhatKyXe()
        {
            return View("~/Views/Gara/QuanLyBaoDuong/NhatKyXe.cshtml");
        }

        public ActionResult PhieuBanGiaoXe()
        {
            return View("~/Views/Gara/QuanLyBaoDuong/DanhSachPhieuBanGiaoXe.cshtml");
        }
    }
}
