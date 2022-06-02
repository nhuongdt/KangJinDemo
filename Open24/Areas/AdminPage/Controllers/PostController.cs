using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model_banhang24vn;
using Model_banhang24vn.CustomView;
using Open24.Areas.AdminPage.Hellper;
using Model_banhang24vn.Common;
using Model_banhang24vn.DAL;
 
namespace Open24.Areas.AdminPage.Controllers
{
    [RBACAuthorize]
    public class PostController : Controller
    {
        // GET: AdminPage/Post
        private readonly GroupPostService _GroupPostService = new GroupPostService();
        [RBACAuthorize(RoleKey = StaticRole.BAIVIET_VIEW)]
        public ActionResult Index()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.BAIVIET_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.BAIVIET_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.BAIVIET_DELETE)
            };
            return View(checkRoleView);
        }

        public ActionResult NewPost()
        {
            return View();
        }
        public ActionResult AddPost()
        {
            return View();
        }
        public ActionResult AddPost1()
        {
            return View();
        }
        [RBACAuthorize(RoleKey = StaticRole.NHOMBAIVIET_VIEW)]
        public ActionResult GroupPost()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.NHOMBAIVIET_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.NHOMBAIVIET_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.NHOMBAIVIET_DELETE)
            };
            return View(checkRoleView);
        }
    }
}