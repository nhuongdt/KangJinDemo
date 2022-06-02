using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Model_banhang24vn;
using Open24.Areas.AdminPage.Hellper;
using Model_banhang24vn.DAL;
using System.Threading.Tasks;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using log4net;
using System.Reflection;
 
namespace Open24.Areas.AdminPage.Controllers
{
    [RBACAuthorize]
    public class News_UserController : BaseController
    {
        #region Declare

        // GET: AdminPage/News_User

        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private News_UserService _News_UserService;
        private News_UserGroupService _News_UserGroupService;
        public News_UserController()
        {
            _News_UserService = new News_UserService();
            _News_UserGroupService = new News_UserGroupService();
        }

        #endregion

        #region View

        [RBACAuthorize(RoleKey = StaticRole.USER_VIEW)]
        public ActionResult Index()
        {
            var checkRoleView = new UserRoleView()
            {
                RoleInsert = contant.CheckRole(StaticRole.USER_INSERT),
                RoleUpdate = contant.CheckRole(StaticRole.USER_UPDATE),
                RoleDelete = contant.CheckRole(StaticRole.USER_DELETE)
            };
            return View(checkRoleView);
        }

        [RBACAuthorize(RoleKey = StaticRole.USER_INSERT)]
        public ActionResult Create()
        {
            return View();
        }

        #endregion

        #region GetData

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAll()
        {
            try
            {
                var listUser = _News_UserService.GetAll().AsQueryable();
                var view = new DataGridView()
                {
                    Page = 1,
                    PageCount = (int)Math.Ceiling((double)listUser.Count() / Notification.PageDefault),
                    Data = listUser.Take(Notification.PageDefault).ToList(),
                };
                if (view.PageCount == 0)
                {
                    view.PageCount = 1;
                }
                view.PageItem = GridPagedingHellper.PageItems(view.Page, view.PageCount, listUser.Count());
                return RetunJsonGetAction(true, string.Empty, view);
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return ActionFalseNotData("Đã xảy ra lỗi trong quá trình lấy dữ liệu.");

        }

        /// <summary>
        /// Event Search next page Grid
        /// </summary>
        /// <param name="daTatable"></param>
        /// <returns></returns>
        public JsonResult GetDataForShearchUser(DataGridView daTatable)
        {
            try
            {
                var data = _News_UserService.Search(daTatable.Search);
                if (daTatable.Sort == (int)GridPagedingHellper.GridSort.SortUp)
                {
                    switch (daTatable.Columname)
                    {
                        case (int)GridPagedingHellper.columUser.Name:
                            data = data.OrderBy(o => o.Name);
                            break;
                        case (int)GridPagedingHellper.columUser.userName:
                            data = data.OrderBy(o => o.UserName);
                            break;
                        case (int)GridPagedingHellper.columUser.BirthDay:
                            data = data.OrderBy(o => o.BirthDay);
                            break;
                        case (int)GridPagedingHellper.columUser.Adress:
                            data = data.OrderBy(o => o.Address);
                            break;
                        case (int)GridPagedingHellper.columUser.Email:
                            data = data.OrderBy(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columUser.Phone:
                            data = data.OrderBy(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columUser.CreateDate:
                            data = data.OrderBy(o => o.CreatDate);
                            break;
                        case (int)GridPagedingHellper.columUser.CreateBy:
                            data = data.OrderBy(o => o.CreatedBy);
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
                        case (int)GridPagedingHellper.columUser.Name:
                            data = data.OrderByDescending(o => o.Name);
                            break;
                        case (int)GridPagedingHellper.columUser.userName:
                            data = data.OrderByDescending(o => o.UserName);
                            break;
                        case (int)GridPagedingHellper.columUser.BirthDay:
                            data = data.OrderByDescending(o => o.BirthDay);
                            break;
                        case (int)GridPagedingHellper.columUser.Adress:
                            data = data.OrderByDescending(o => o.Address);
                            break;
                        case (int)GridPagedingHellper.columUser.Email:
                            data = data.OrderByDescending(o => o.Email);
                            break;
                        case (int)GridPagedingHellper.columUser.Phone:
                            data = data.OrderByDescending(o => o.Phone);
                            break;
                        case (int)GridPagedingHellper.columUser.CreateDate:
                            data = data.OrderByDescending(o => o.CreatDate);
                            break;
                        case (int)GridPagedingHellper.columUser.CreateBy:
                            data = data.OrderByDescending(o => o.CreatedBy);
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
                return RetunJsonAction(true, null, daTatable);
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return ActionFalseNotData("Đã xảy ra lỗi trong quá trình xử lý dữ liệu.");
        }

        /// <summary>
        /// Lấy danh sách nhóm người dùng
        /// </summary>
        /// <returns></returns>
        public ActionResult GetGroupUser()
        {
            try
            {
                var listUser = _News_UserGroupService.GetAllActive().AsEnumerable();
                return RetunJsonGetAction(true, string.Empty, listUser);
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
                return Exeption();
            }
        }

        #endregion

        #region Event insert update delete

        /// <summary>
        /// Thêm mới người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize(RoleKey = StaticRole.USER_INSERT)]
        public ActionResult CreateUser(News_User model, string ConfluentPassword)
        {
            try
            {
                var validate = ValidateUser(model, ConfluentPassword);
                if (validate.ErrorCode != (int)Notification.ErrorCode.success)
                {
                    return RetunJsonAction(false, validate.Data, new List<News_User>());
                }
                model.CreatedBy = contant.SESSIONNGUOIDUNG.UserName;
                var result = _News_UserService.InsertUser(model);
                if (result.ErrorCode == (int)Notification.ErrorCode.success)
                {
                    return InsertSuccess();
                }
                else if (result.ErrorCode == (int)Notification.ErrorCode.exist)
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
        /// Xóa người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize(RoleKey = StaticRole.USER_DELETE)]
        public ActionResult DeleteUser(News_User user)
        {
            try
            {
                if (_News_UserService.DeleteUser(user) == (int)Notification.ErrorCode.success)
                {
                    return DeleteSuccess();
                }
                else
                {
                    return ActionFalseNotData("Lỗi người dùng đó không tồn tại hoặc đã bị xóa.");
                }

            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize(RoleKey = StaticRole.USER_UPDATE)]
        public ActionResult UpdateUser(News_User model)
        {
            try
            {
                model.ModifiedBy = contant.SESSIONNGUOIDUNG.UserName;
                var result = _News_UserService.UpdateUser(model);
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
        #endregion

        #region Validate data

        /// <summary>
        /// Kiểm tra dữ liệu user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private JsonViewModel<string> ValidateUser(News_User model, string ConfluentPassword)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            if (string.IsNullOrWhiteSpace(model.UserName))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập tên tài khoản.";
            }
            else if (string.IsNullOrWhiteSpace(model.Name))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập họ tên.";
            }
            else if (StaticVariable.VietnameseSigns.Any(o => model.UserName.ToLower().Contains(o.ToLower())))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Tài khoản không được chứa dấu, vui lòng nhập lại";
            }
            else if (string.IsNullOrWhiteSpace(model.Password))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập mật khẩu.";
            }
            else if (model.GroupID == null)
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng chọn quyền.";
            }
            else if (!string.IsNullOrWhiteSpace(model.Email) && !StaticVariable.EmailIsValid(model.Email.Trim()))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Địa chỉ email không hợp lệ.";
            }
            else if (!string.IsNullOrWhiteSpace(model.Phone) && !StaticVariable.PhoneIsValid(model.Phone.Trim()))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Số điện thoại không hợp lệ.";
            }
            else if (string.IsNullOrWhiteSpace(ConfluentPassword) || model.Password.Trim() != ConfluentPassword.Trim())
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập lại mật khẩu khớp với nhau.";
            }
            return result;
        }

        #endregion

    }
}
