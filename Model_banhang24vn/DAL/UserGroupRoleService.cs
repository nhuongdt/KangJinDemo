using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
    public class UserGroupRoleService
    {
        BanHang24vnContext db;
        public UserGroupRoleService()
        {
            db = new BanHang24vnContext();
        }

        public IQueryable<UserGroupRole> GetAll()
        {
            return db.UserGroupRoles.AsQueryable();
        }

        /// <summary>
        /// Check quyền của mỗi người dùng
        /// </summary>
        /// <param name="userGroupId"></param>
        /// <param name="rolekey"></param>
        /// <returns></returns>
        public bool CheckRoleUser(Guid userGroupId, string rolekey)
        {
            return db.UserGroupRoles.Where(o => o.UserGroupId == userGroupId && o.RoleKey.Equals(rolekey)).Any();
        }

        /// <summary>
        /// Lấy danh sách quyền theo nhóm quyền
        /// </summary>
        /// <param name="UserGroupId"></param>
        /// <returns></returns>
        public  List<string> GetRoleForUserGroup(Guid UserGroupId)
        {
            return db.UserGroupRoles.Where(o => o.UserGroupId == UserGroupId).Select(o => o.RoleKey).ToList();
        }

        

       
    }
}
