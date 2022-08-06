using banhang24.Hellper;
using libHT_NguoiDung;
using libQuy_HoaDon;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace banhang24.Controllers
{
    [App_Start.App_API.CheckwebAuthorize]
    public class HangHoaController : Controller
    {
        //
        // GET: /HangHoa/
        #region danh mục
        [RBACAuthorize(RoleKey = RoleKey.HangHoa_XemDS)]
        public ActionResult danhsachhanghoa()
        {
            using (SsoftvnContext db = SystemDBContext.GetDBContext())
            {
                classHT_NguoiDung classHTNguoiDung = new classHT_NguoiDung(db);
                userLogin objUser_Cookies = classHTNguoiDung.GetUserCookies(this);
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
        public ActionResult _themmoicapnhathanghoa(string id)
        {
            return PartialView();
        }

        public ActionResult _themmoicapnhatnhomhanghoa(string id)
        {
            return PartialView();
        }

        public ActionResult _themmoicapnhatdichvu()
        {
            return PartialView();
        }
        [HttpPost]
        [System.Web.Mvc.ValidateAntiForgeryToken]
        public System.Web.Mvc.ActionResult HangHoaUpload(HttpPostedFileBase file, Guid ID_DonVi, Guid IDNhanVien)
        {
            List<ErrorDMHangHoa> abc = new List<ErrorDMHangHoa>();
            if (ModelState.IsValid)
            {
                using (SsoftvnContext db = SystemDBContext.GetDBContext())
                {
                    Class_officeDocument classOffice = new Class_officeDocument(db);
                    System.IO.Stream excelstream = file.InputStream;
                    abc = classOffice.checkExcel(excelstream, ID_DonVi, IDNhanVien);

                    //if (file != null && file.ContentLength > 0)
                    //{
                    //    String path = System.Web.HttpContext.Current.Server.MapPath("~/Template/FileUpload/" + file.FileName);
                    //    file.SaveAs(path);
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("File", "Please Upload Your file");
                    //}
                }
            }
            //return View("danhsachhanghoa", abc);
            return PartialView("danhsachhanghoa", abc);
            //return View("danhsachhanghoa");
        }
        #endregion
        #region chương trình giá
        [RBACAuthorize(RoleKey = RoleKey.ThietLapGia_XemDS)]
        public ActionResult danhsachgia()
        {
            return View();
        }

        public ActionResult _themmoicapnhatbanggia(string id)
        {
            return PartialView();
        }

        #endregion
        [RBACAuthorize(RoleKey = RoleKey.KiemKho_XemDs)]
        public ActionResult kiemkho()
        {
            ViewBag.LoaiHoaDon = 9;
            return View();
        }
        [RBACAuthorize(RoleKey = RoleKey.KiemKho_XemDs)]
        public ActionResult tonkho()
        {
            ViewBag.LoaiHoaDon = 9;
            return View();
        }
        public ActionResult _print()
        {
            return PartialView();
        }
        public ActionResult _printchooseHH()
        {
            return PartialView();
        }
        public ActionResult _Suahanghoa()
        {
            return PartialView();
        }

        public ActionResult _suahanghoadichvu()
        {
            return PartialView();
        }

        public ActionResult _hienthithekho()
        {
            return PartialView();
        }

        public ActionResult _showphieukiemhangct()
        {
            return PartialView();
        }
        public ActionResult _showphieubanhang()
        {
            return PartialView();
        }
        public ActionResult _showphieuxuathuy()
        {
            return PartialView();
        }
        public ActionResult _showphieutrahangnhap()
        {
            return PartialView();
        }
        public ActionResult _showphieutrahang()
        {
            return PartialView();
        }
        public ActionResult _showphieunhaphang()
        {
            return PartialView();
        }

        public ActionResult _showphieudieuchinh()
        {
            return PartialView();
        }

        public ActionResult phieudieuchinh()
        {
            return View();
        }
        public ActionResult dieuchinh()
        {
            return View();
        }

        public ActionResult LoHangHoa()
        {
            return View();
        }

        [RBACAuthorize(RoleKey = RoleKey.DanhMucGiaVonTieuChuan)]
        public ActionResult DanhMucGiaVonTieuChuan()
        {
            return View();
        }
        [RBACAuthorize(RoleKey = RoleKey.PhieuDieuChinh_ThemMoi)]
        public ActionResult ThemPhieuDieuChinh(string id)
        {
            ViewBag.LoaiHoaDon = id.Split('?')[0];
            return View();
        }

        public ActionResult _editLoHang()
        {
            return PartialView();
        }
    }
}
