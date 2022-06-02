using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Open24.Areas.AdminPage.Controllers
{
    public class HoTroController : Controller
    {
        private readonly HoTroService _HoTroService;
        public HoTroController()
        {
            _HoTroService = new HoTroService();
        }
        // GET: AdminPage/HoTro
        public ActionResult Index()
        {
            return View();
        }
        [RBACAuthorize(RoleKey = StaticRole.CAUHOI_VIEW)]
        public ActionResult HoiDap()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.CAUHOI_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.CAUHOI_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.CAUHOI_DELETE)
            };
            return View(checkRoleView);
        }
        [RBACAuthorize(RoleKey = StaticRole.NHOMNGANH_VIEW)]
        public ActionResult NhomVaiTro()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.NHOMNGANH_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.NHOMNGANH_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.NHOMNGANH_DELETE)
            };
            return View(checkRoleView);
        }

        [RBACAuthorize(RoleKey = StaticRole.HT_TINHNANG_VIEW)]
        public ActionResult TinhNang()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.HT_TINHNANG_INSERT)
            };
            return View(checkRoleView);
        }

        [RBACAuthorize]
        public ActionResult EditTinhNang( long? id)
        {
            if (id != null && id != 0)
            {
                ViewBag.ID_TinhNang = id;
            }
            else
            {
                ViewBag.ID_TinhNang = 0;

            }
            return View();
        }

        /// <summary>
        /// lấy danh sách nhóm quyền
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllTinhNang()
        {
            try
            {
                var data = _HoTroService.GetAllTinhNang.ToList();
                var json = data.Where(o => o.ID_Cha == null).Select(o =>
                     new RoleParentView
                     {
                         id = o.ID.ToString(),
                         text = o.Ten,
                         children = GetChildren(data, o.ID)
                     });
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch 
            {
               
            }
            return Json(new List<RoleParentView>(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách quyền con
        /// </summary>
        /// <param name="data"></param>
        /// <param name="roleKey"></param>
        /// <returns></returns>
        private List<RoleParentView> GetChildren(List<LH_TinhNang> data, long roleKey)
        {
            return data.Where(o => o.ID_Cha != null && o.ID_Cha.Equals(roleKey)).Select(o =>
                       new RoleParentView
                       {
                           id = o.ID.ToString(),
                           text = o.Ten,
                           children = GetChildren(data, o.ID)
                       }).ToList();
        }

        public JsonResult GetAllTinhNangCha()
        {
            try
            {
                var data = _HoTroService.GetAllTinhNang;
                var json = data.Where(o => o.ID_Cha == null).AsEnumerable().Select(o =>
                     new RoleParentView
                     {
                         id = o.ID.ToString(),
                         text = o.Ten
                     }).ToList();
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch
            {

            }
            return Json(new List<RoleParentView>(), JsonRequestBehavior.AllowGet);
        }
    }
}