using banhang24.Hellper;
using libHT_NguoiDung;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using libNS_NhanVien;

namespace banhang24.Controllers
{
    [App_Start.App_API.CheckwebAuthorize]
    public class SoQuyController : Controller
    {
        [RBACAuthorize(RoleKey = RoleKey.SoQuy_XemDs)]
        public ActionResult SoQuy2(string id)
        {
            using (SsoftvnContext _dbcontext = SystemDBContext.GetDBContext())
            {
                var ID_ND = new Guid(CookieStore.GetCookieAes(SystemConsts.NGUOIDUNGID));
                ViewBag.ShopCookies = CookieStore.GetCookieAes("shop").ToUpper();
                var RoleModel = new RoleModel() { View = true, Insert = true, Update = true, Delete = true, Export = true };
                ViewBag.LoaiHoaDon = id.Substring(0, 1);
                if (!_dbcontext.HT_NguoiDung.Any(o => o.ID == ID_ND && o.LaAdmin))
                {
                    classHT_NguoiDung classNguoiDung = new classHT_NguoiDung(_dbcontext);
                    var listQuyen = classNguoiDung.GetListQuyen().Select(o => o.MaQuyen);
                    RoleModel.Insert = listQuyen.Any(o => o.Equals(RoleKey.SoQuy_ThemMoi));
                    RoleModel.Update = listQuyen.Any(o => o.Equals(RoleKey.SoQuy_CapNhat));
                    RoleModel.Delete = listQuyen.Any(o => o.Equals(RoleKey.SoQuy_Xoa));
                    RoleModel.Export = listQuyen.Any(o => o.Equals(RoleKey.SoQuy_XuatFile));
                }
                return View(RoleModel);
            }
            return View();
        }
       
        public ActionResult _editphieuchiHD()
        {
            return PartialView();
        }
    }
}
