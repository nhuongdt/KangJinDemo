using log4net;
using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
 
namespace Open24.Areas.AdminPage.Controllers
{
    [RBACAuthorize]
    public class News_UserGroupController : BaseController
    {
        #region Declare
        // GET: AdminPage/News_UserGroup
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private News_UserGroupService _News_UserGroupService;
        private RoleService _RoleService;
        private UserGroupRoleService _UserGroupRoleService;
        public News_UserGroupController()
        {
            _News_UserGroupService = new News_UserGroupService();
            _RoleService = new RoleService();
            _UserGroupRoleService = new UserGroupRoleService();
        }

        #endregion

        #region View

        [RBACAuthorize(RoleKey = StaticRole.USERGROUP_VIEW)]
        public ActionResult Index()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.USERGROUP_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.USERGROUP_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.USERGROUP_DELETE)
            };
            return View(checkRoleView);
        }

        #endregion

        #region Event Insert Delete Update Search

        /// <summary>
        /// Thêm mới nhóm người dùng
        /// </summary>
        /// <param name="UserGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize(RoleKey = StaticRole.USERGROUP_INSERT)]
        public JsonResult Create(News_UserGroup UserGroup, List<string> checkList)
        {
            try
            {
                if (UserGroup == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần thêm mới.");
                }
                if (string.IsNullOrWhiteSpace(UserGroup.GroupName))
                {
                    return ActionFalseNotData("Vui lòng nhập tên nhóm người dùng.");
                }
                UserGroup.CreatedBy = contant.SESSIONNGUOIDUNG.UserName;
                var listRole = new List<string>();
                if (checkList != null && checkList.Count > 0)
                {
                    listRole = _RoleService.SelectRoleChildren(checkList).Select(o => o.RoleKey).ToList();
                }
                var result = _News_UserGroupService.Insert(UserGroup, listRole);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    return InsertSuccess();
                }
                else
                {
                    return ActionFalseNotData(result.Data);

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Update nhóm người dùng
        /// </summary>
        /// <param name="UserGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize(RoleKey = StaticRole.USERGROUP_UPDATE)]
        public JsonResult Update(News_UserGroup UserGroup, List<string> checkList)
        {
            try
            {
                var listRole = new List<string>();
                UserGroup.ModifiedBy = contant.SESSIONNGUOIDUNG.UserName;
                if (checkList != null && checkList.Count > 0)
                {
                    listRole = _RoleService.SelectRoleChildren(checkList).Select(o => o.RoleKey).ToList();
                }
                if(string.IsNullOrWhiteSpace(UserGroup.GroupName))
                {
                    return ActionFalseNotData("Vui lòng nhập thông tin nhóm người dùng");
                }
                var result = _News_UserGroupService.Update(UserGroup, listRole);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    return UpdateSuccess();
                }
                else
                {
                    return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Xóa nhóm người dùng
        /// </summary>
        /// <param name="UserGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize(RoleKey = StaticRole.USERGROUP_DELETE)]
        public JsonResult Delete(News_UserGroup UserGroup)
        {
            try
            {
                var result = _News_UserGroupService.Delete(UserGroup);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    return DeleteSuccess();
                }
                else
                {
                    return ActionFalseNotData(result.Data);
                }
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Tìm kiếm dữ liệu
        /// </summary>
        /// <param name="daTatable"></param>
        /// <returns></returns>
        public JsonResult GetDataUserGroup(DataGridView daTatable)
        {
            var data = _News_UserGroupService.Search(daTatable.Search);
            if (daTatable.Sort == (int)GridPagedingHellper.GridSort.SortUp)
            {
                switch (daTatable.Columname)
                {
                    case (int)GridPagedingHellper.columUserGroup.creatBy:
                        data = data.OrderBy(o => o.CreatedBy);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.creatDate:
                        data = data.OrderBy(o => o.CreatDate);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.detail:
                        data = data.OrderBy(o => o.Description);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.groupName:
                        data = data.OrderBy(o => o.GroupName);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.modifyBy:
                        data = data.OrderBy(o => o.ModifiedBy);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.modifyDate:
                        data = data.OrderBy(o => o.ModifiedDate);
                        break;
                    default:
                        data = data.OrderBy(o => o.Status);
                        break;
                }
            }
            else
            {
                switch (daTatable.Columname)
                {
                    case (int)GridPagedingHellper.columUserGroup.creatBy:
                        data = data.OrderByDescending(o => o.CreatedBy);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.creatDate:
                        data = data.OrderByDescending(o => o.CreatDate);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.detail:
                        data = data.OrderByDescending(o => o.Description);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.groupName:
                        data = data.OrderByDescending(o => o.GroupName);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.modifyBy:
                        data = data.OrderByDescending(o => o.ModifiedBy);
                        break;
                    case (int)GridPagedingHellper.columUserGroup.modifyDate:
                        data = data.OrderByDescending(o => o.ModifiedDate);
                        break;
                    default:
                        data = data.OrderByDescending(o => o.Status);
                        break;
                }
            }
            daTatable.PageCount = (int)Math.Ceiling((double)data.Count() / daTatable.Limit);
            if (daTatable.PageCount == 0 || daTatable.PageCount == 1)
            {
                daTatable.PageCount = 1;
                daTatable.Page = 1;
                daTatable.Data = data.ToList();
            }
            else
            {
                daTatable.Data = data.Skip(daTatable.Limit * (daTatable.Page - 1)).Take(daTatable.Limit).ToList();
            }
            daTatable.PageItem = GridPagedingHellper.PageItems(daTatable.Page, daTatable.PageCount, data.Count());
            return Json(daTatable);

        }

        #endregion

        #region GetData

        /// <summary>
        /// Lấy danh sách nhóm người dùng
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllUserGroup()
        {
            try
            {
                var listUser = _News_UserGroupService.GetAll();
                var view = new DataGridView()
                {
                    Page = 1,
                    PageCount = (int)Math.Ceiling((double)listUser.Count() / Notification.PageDefault),
                    Data = _News_UserGroupService.GetAll().Take(Notification.PageDefault).ToList(),
                };
                if (view.PageCount == 0)
                {
                    view.PageCount = 1;
                }
                view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, listUser.Count());
                return RetunJsonGetAction(true, null, view);
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return GetExeption();

        }

        /// <summary>
        /// lấy danh sách nhóm quyền
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllRole()
        {
            try
            {
                var data = _RoleService.GetAll().ToList();
                var json = data.Where(o => o.RoleParent == null).Select(o =>
                     new RoleParentView
                     {
                         id = o.RoleKey,
                         text = o.Name,
                         children = GetChildren(data, o.RoleKey)
                     });
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                logger.Error( e.Message);
            }
            return Json(new List<Role>(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách quyền con
        /// </summary>
        /// <param name="data"></param>
        /// <param name="roleKey"></param>
        /// <returns></returns>
        private List<RoleParentView> GetChildren(List<Role> data, string roleKey)
        {
            return data.Where(o => o.RoleParent != null && o.RoleParent.Equals(roleKey)).Select(o =>
                       new RoleParentView
                       {
                           id = o.RoleKey,
                           text = o.Name,
                           children = GetChildren(data, o.RoleKey)
                       }).ToList();
        }

        /// <summary>
        /// Lấy danh sách quyền theo người dùng đã chọn khi sửa
        /// </summary>
        /// <param name="UserGroupid"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserRole(Guid? UserGroupid)
        {
            try
            {
                return Json(_UserGroupRoleService.GetRoleForUserGroup(UserGroupid ?? new Guid()), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Json(new List<string>(), JsonRequestBehavior.AllowGet);
        }


        #endregion
    }
}