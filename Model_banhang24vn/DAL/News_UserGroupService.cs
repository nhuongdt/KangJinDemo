using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model_banhang24vn.Common;

namespace Model_banhang24vn.DAL
{
    public class News_UserGroupService
    {
        BanHang24vnContext db;
        public News_UserGroupService()
        {
            db = new BanHang24vnContext();
        }

        /// <summary>
        /// Láy toàn bộ danh sách nhóm người dùng
        /// </summary>
        /// <returns></returns>
        public IQueryable<News_UserGroup> GetAll()
        {
            return db.News_UserGroup.OrderByDescending(o => o.CreatDate).AsQueryable();
        }

        /// <summary>
        /// Láy toàn bộ danh sách nhóm người dùng đang hoạt động
        /// </summary>
        /// <returns></returns>
        public IQueryable<News_UserGroup> GetAllActive()
        {
            return db.News_UserGroup.Where(o => o.Status == true).AsQueryable();
        }

        /// <summary>
        ///  Lấy 1 bản ghi nhóm người dùng theo ID
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public News_UserGroup Getbykey(Guid groupID)
        {
            return db.News_UserGroup.FirstOrDefault(o => o.ID.Equals(groupID));
        }

        /// <summary>
        /// Lấy 1 bản ghi nhóm người dùng theo tên
        /// </summary>
        /// <param name="GroupUserName"></param>
        /// <returns></returns>
        public News_UserGroup Getbyname(string GroupUserName)
        {
            return db.News_UserGroup.FirstOrDefault(o => o.GroupName.Equals(GroupUserName));
        }

        /// <summary>
        /// Thêm mới nhóm người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonViewModel<string> Insert(News_UserGroup model, List<string> listRole)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            if (db.News_UserGroup.Where(o => o.GroupName.ToUpper().Equals(model.GroupName.ToUpper())).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Tên nhóm đã tồn tại, vui lòng nhập tên nhóm khác";
            }
            else
            {
                model.ID = Guid.NewGuid();
                model.CreatDate = DateTime.Now;
                db.News_UserGroup.Add(model);
                for (int i = 0; i < listRole.Count; i++)
                {
                    db.UserGroupRoles.Add(new UserGroupRole
                    {
                        ID = Guid.NewGuid(),
                        RoleKey = listRole[i].Trim(),
                        UserGroupId = model.ID,
                    });
                }
                db.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Update nhóm người dùng bao gồm cả các quyền của nhóm người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonViewModel<string> Update(News_UserGroup model, List<string> listRoleUpdate)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            var userGroup = db.News_UserGroup.FirstOrDefault(o => o.ID.Equals(model.ID));
            if (userGroup == null)
            {
                result.ErrorCode = (int)Notification.ErrorCode.notfound;
                result.Data = "Nhóm người dùng không tồn tại hoặc đã bị xóa.";
            }
            else if (db.News_UserGroup.Where(o => o.GroupName.ToUpper().Equals(model.GroupName.ToUpper()) && o.ID != model.ID).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Tên nhóm mới đã tồn tại, vui lòng nhập tên khác.";
            }
            else
            {
                if (listRoleUpdate.Count > 0)
                {
                    var userRole = db.UserGroupRoles.Where(o => o.UserGroupId == userGroup.ID);
                    var userRoleDelete = userRole.Where(o => !listRoleUpdate.Contains(o.RoleKey)).ToList();
                    db.UserGroupRoles.RemoveRange(userRoleDelete);
                    var listRolekey = userRole.Select(c => c.RoleKey).ToList();
                    var userRoleInsert = listRoleUpdate.Where(o => !listRolekey.Contains(o)).ToList();
                    for (int i = 0; i < userRoleInsert.Count; i++)
                    {
                        db.UserGroupRoles.Add(new UserGroupRole
                        {
                            ID = Guid.NewGuid(),
                            RoleKey = userRoleInsert[i].Trim(),
                            UserGroupId = model.ID,
                        });
                    }
                }
                userGroup.Description = model.Description;
                userGroup.GroupName = model.GroupName;
                userGroup.Status = model.Status;
                userGroup.ModifiedDate = DateTime.Now;
                userGroup.ModifiedBy = model.ModifiedBy;
                db.SaveChanges();
            }
            return result;
        }

        /// <summary>
        /// Xóa nhóm người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonViewModel<string> Delete(News_UserGroup model)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            var userGroup = db.News_UserGroup.FirstOrDefault(o => o.ID == model.ID);
            if (userGroup == null)
            {
                result.ErrorCode = (int)Notification.ErrorCode.notfound;
                result.Data = "Nhóm người dùng không tồn tại hoặc đã bị xóa.";
            }
            var listUserGroupRole = db.UserGroupRoles.Where(o => o.UserGroupId == userGroup.ID).ToList();
            db.UserGroupRoles.RemoveRange(listUserGroupRole);
            db.News_UserGroup.Remove(userGroup);
            db.SaveChanges();
            return result;
        }

        public IQueryable<News_UserGroup> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return GetAll();
            }
            text = StringExtensions.ConvertToUnSign(text.Trim().ToLower());
            return db.News_UserGroup.AsEnumerable().Where(o => StringExtensions.ConvertToUnSign(o.GroupName).Contains(text)
                                                             || StringExtensions.ConvertToUnSign(o.Description).Contains(text)
                                                                 ).OrderBy(o => o.CreatDate).AsQueryable();
        }
    }
}
