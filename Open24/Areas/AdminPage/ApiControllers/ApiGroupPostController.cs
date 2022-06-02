using log4net;
using Model_banhang24vn;
using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.CustomView.Client;
using Model_banhang24vn.DAL;
using Open24.Areas.AdminPage.Hellper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
 
namespace Open24.Areas.AdminPage.ApiControllers
{
    public class ApiGroupPostController : ApiBaseController
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly GroupPostService _GroupPostService;
        public ApiGroupPostController()
        {
            _GroupPostService = new GroupPostService();
        }
       
        /// <summary>
        /// Load combobox
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult LoadAllCombobox(int? GroupId)
        {
            try
            {
                var data = _GroupPostService.GetGroupParent(GroupId ?? 0).Select(o =>
                                            new { ID = o.ID, Name = o.Name }).ToList();
                data.Insert(0, new { ID = 0, Name = string.Empty });
                return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }
        /// <summary>
        /// load nhóm bài viết không thuộc lớp cha
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetGroupchilden()
        {
            try
            {
                var data = _GroupPostService.GetGroupchilden().Select(o =>
                                            new { ID = o.ID, Name = o.Name }).AsEnumerable();
                return RetunJsonAction(true, string.Empty, data);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return ActionFalseNotData("Đã xảy ra lỗi trong quá trình lấy thể loại bài viết");
        }

        /// <summary>
        /// Thêm mới nhóm bài viết
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize]
        public IHttpActionResult InsertPostGroup(News_Categories model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần thêm mới.");
                }
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return ActionFalseNotData("Vui lòng nhập tên nhóm.");
                }
                else
                {
                    model.CreateDate = DateTime.Now;
                    model.CreateBy = contant.SESSIONNGUOIDUNG.UserName;
                    if (model.ParentID == 0) model.ParentID = null;
                    var result = _GroupPostService.Insert(model);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        return InsertSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Cập nhật nhóm bài viết
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize]
        public IHttpActionResult UpdatePostGroup(News_Categories model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được thông tin cần cập nhật.");
                }
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    return ActionFalseNotData("Vui lòng nhập tên nhóm.");
                }
                else
                {
                    model.ModifiedDate = DateTime.Now;
                    model.ModifiedBy = contant.SESSIONNGUOIDUNG.UserName;
                    if (model.ParentID == 0) model.ParentID = null;
                    var result = _GroupPostService.Update(model);
                    if (result.ErrorCode == (int)Notification.ErrorCode.success)
                    {
                        return UpdateSuccess();
                    }
                    else
                    {
                        return ActionFalseNotData(result.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error( ex.Message);
            }
            return Exeption();
        }

        /// <summary>
        /// Xóa nhóm bài viết
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [RBACAuthorize]
        public IHttpActionResult DeletePostGroup(PostGroupView model)
        {
            try
            {
                if (model == null)
                {
                    return ActionFalseNotData("Không lấy được nhóm cần xóa.");
                }
                var result = _GroupPostService.Delete(model.ID);
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
        /// Load dữ liệu tree
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [RBACAuthorize]
        public IHttpActionResult Loadtree()
        {
            var data = _GroupPostService.Query.AsEnumerable();
            bool roleEdit = contant.CheckRole(StaticRole.NHOMBAIVIET_UPDATE);
            bool roledelete = contant.CheckRole(StaticRole.NHOMBAIVIET_DELETE);
            var json = data.Where(o => o.ParentID == null).Select(o =>
                    new CategoryParentView
                    {
                        id = o.ID,
                        edit = roleEdit ? "<a href='#' class='blue'onclick='btnUpdateGroupPost(" + o.ID + ")'><span class='glyphicon glyphicon-pencil' ></span> </a>" : string.Empty,
                        delete = roledelete ? "<a href='#' class='red' onclick='btnDeleteGrouppost(" +o.ID + ")'><span class='glyphicon glyphicon-trash' ></span></a>" : string.Empty,
                        text = o.Name,
                        children = GetChildren(data, o.ID,roleEdit,roledelete)
                    });
            return Json(json);
        }

        private List<CategoryParentView> GetChildren(IEnumerable<News_Categories> data, int roleKey,bool roleEdit,bool roledelete)
        {
            return data.Where(o => o.ParentID != null && o.ParentID.Equals(roleKey)).Select(o =>
                       new CategoryParentView
                       {
                           id = o.ID,
                           edit = roleEdit ? "<a href='#' class='blue'onclick='btnUpdateGroupPost(" + o.ID + ")'><span class='glyphicon glyphicon-pencil' ></span> </a>" : string.Empty,
                           delete = roledelete ? "<a href='#' class='red' onclick='btnDeleteGrouppost(" + o.ID + ")'><span class='glyphicon glyphicon-trash' ></span></a>" : string.Empty,
                           text = o.Name,
                           children = GetChildren(data, o.ID,roleEdit,roledelete)
                       }).ToList();
        }

        /// <summary>
        /// Load default khi sửa nhóm bài viết
        /// </summary>
        /// <param name="GroupId"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetByGroupPost(int? GroupId)
        {
            try
            {
                var model = _GroupPostService.Query.FirstOrDefault(o => o.ID == GroupId);
                if(model!=null)
                {
                    return RetunJsonAction(true, string.Empty, model);
                }
                else
                {
                    return ActionFalseNotData("Bản ghi đã bị xóa hoặc");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return Exeption();
        }
    }
}
