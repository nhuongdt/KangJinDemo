using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Model_banhang24vn.DAL
{
    public class News_UserService
    {
        BanHang24vnContext db;
        public News_UserService()
        {
            db = new BanHang24vnContext();
        }

        public void Updateurl()
        {
            //var listnews = db.News_Articles.ToList();
            //foreach(var item in listnews)
            //{
            //    item.Url = string.Format("/tin-tuc/{0}-{1}.html", StaticVariable.ConvetTitleToUrl(item.Title), item.ID);
            //}
            var listCustomer = db.Customers.ToList();
            foreach (var item in listCustomer)
            {
                item.Url = string.Format("/khach-hang/{0}-{1}.html", StaticVariable.ConvetTitleToUrl(item.Name), item.ID);
            }
            db.SaveChanges();
        }

        /// <summary>
        /// Login khi đăng nhập
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="UserPassWord"></param>
        /// <returns></returns>
        public JsonViewModel<News_User> LogginAccount(string userName, string UserPassWord)
        {
            var result = new JsonViewModel<News_User>();
            try
            {
                string pasConverMd5 = ConvertMD5.GetMd5Hash(UserPassWord);
                News_User dataUser = db.News_User.FirstOrDefault(o => o.UserName.Equals(userName) && o.Password.Equals(pasConverMd5));
                if (dataUser != null)
                {
                    if (dataUser.Status == true)
                    {
                        result.Data = dataUser;
                        result.ErrorCode = (int)Notification.ErrorCode.success;
                    }
                    else
                    {
                        result.ErrorCode = (int)Notification.ErrorCode.notactive;
                    }
                }
                else
                {
                    result.ErrorCode = (int)Notification.ErrorCode.notfound;
                }
            }
            catch (Exception)
            {
                result.ErrorCode = (int)Notification.ErrorCode.exception;
            }
            return result;
        }

        public string GetNameByPhone(string phonenumber)
        {
            News_User user = db.News_User.FirstOrDefault(o => o.Phone == phonenumber);
            if(user != null)
            {
                return user.Name;
            }    
            else
            {
                return "";
            }    
        }

        /// <summary>
        /// Lây toàn bộ danh sách người dùng
        /// </summary>
        /// <returns></returns>
        public IQueryable<News_User> GetAll()
        {
            return db.News_User.OrderByDescending(p => p.CreatDate).AsQueryable();
        }

        public News_User Getbykey(Guid Id)
        {
            return db.News_User.FirstOrDefault(o => o.ID == Id);
        }

        /// <summary>
        /// Thêm mới người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonViewModel<string> InsertUser(News_User model)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            var validate = CheckUser(model);
            if (validate.ErrorCode == (int)Notification.ErrorCode.success)
            {
                model.ID = Guid.NewGuid();
                model.CreatDate = DateTime.Now;
                model.Password = ConvertMD5.GetMd5Hash(model.Password);
                model.UserName.Trim();
                model.Phone = model.Phone != null ? model.Phone.Trim() : string.Empty;
                model.Email = model.Email != null ? model.Email.Trim() : string.Empty;
                db.News_User.Add(model);
                db.SaveChanges();

            }
            else
            {
                return validate;
            }

            return result;

        }

        /// <summary>
        /// Kiểm tra tài khoản đã tồn tại hay chưa
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public JsonViewModel<string> CheckUser(News_User model)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            if (db.News_User.Where(o => o.UserName.Equals(model.UserName)).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Tên tài khoản đã tồn tại, vui lòng nhập số khác.";
            }
            else if (db.News_User.Where(o => o.Phone != null && o.Phone.Equals(model.Phone)).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Số điện thoại đã tồn tại, vui lòng nhập số khác.";
            }
            else if (!string.IsNullOrWhiteSpace(model.Email)
                 && db.News_User.Where(o => o.Email != null && o.Email.ToLower().Equals(model.Email.ToLower())).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Email đã tồn tại, vui lòng nhập số khác.";
            }
            return result;
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        public int DeleteUser(News_User User)
        {
            var model = db.News_User.FirstOrDefault(o => o.ID.Equals(User.ID));
            if (model != null)
            {
                db.News_User.Remove(model);
                db.SaveChanges();
                return (int)Notification.ErrorCode.success;
            }
            else
            {
                return (int)Notification.ErrorCode.notfound;
            }
        }

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonViewModel<string> UpdateUser(News_User model)
        {
            var result = new JsonViewModel<string>();
            var user = db.News_User.FirstOrDefault(o => o.ID == model.ID);

            if (!string.IsNullOrWhiteSpace(model.Email) && !StaticVariable.EmailIsValid(model.Email.Trim()))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Địa chỉ email không hợp lệ.";
                return result;
            }
            else if (!string.IsNullOrWhiteSpace(model.Phone) && !StaticVariable.PhoneIsValid(model.Phone.Trim()))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Số điện thoại không hợp lệ.";
                return result;
            }
            else if (string.IsNullOrWhiteSpace(model.Name))
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Vui lòng nhập họ tên.";
                return result;

            }
            else if (db.News_User.Where(o => o.Phone != null && o.Phone.Equals(model.Phone) && o.ID != user.ID).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Số điện thoại đã tồn tại, vui lòng nhập số khác.";
            }
            else if (!string.IsNullOrWhiteSpace(model.Email)
                 && db.News_User.Where(o => o.Email != null && o.Email.ToLower().Equals(model.Email.ToLower()) && o.ID != user.ID).Any())
            {
                result.ErrorCode = (int)Notification.ErrorCode.exist;
                result.Data = "Email đã tồn tại, vui lòng nhập số khác.";
            }
            if (user != null)
            {
                user.ModifiedDate = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.Password = ConvertMD5.GetMd5Hash(model.Password);
                }
                user.Phone = model.Phone != null ? model.Phone.Trim() : string.Empty;
                user.Email = model.Email != null ? model.Email.Trim() : string.Empty;
                user.Address = model.Address;
                user.GroupID = model.GroupID;
                user.Status = model.Status;
                user.BirthDay = model.BirthDay;
                user.ModifiedBy = model.ModifiedBy;
                user.Name = model.Name;
                db.SaveChanges();
                result.ErrorCode = (int)Notification.ErrorCode.success;
                return result;
            }
            result.ErrorCode = (int)Notification.ErrorCode.notfound;
            result.Data = "Người dùng không tồn tại hoặc đã bị xóa vui lòng tải lại trang";
            return result;
        }

        /// <summary>
        /// Kiểm tra người dùng Id có nhóm quyền groupId không
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>
        /// True: có tồn tại
        /// </returns>
        public bool GetUserForGroupName(Guid groupId, Guid userId)
        {
            return db.News_User.Where(o => o.ID == userId && o.GroupID == groupId).Any();
        }

        /// <summary>
        /// Cập nhật người dùng hiện tại
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public JsonViewModel<string> UpdateProfile(UserProfileView model)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            var user = db.News_User.FirstOrDefault(o => o.ID == model.UserId);
            if (user != null)
            {
                if (user.Password.Equals(ConvertMD5.GetMd5Hash(model.PasswordOld)) ||
                    (string.IsNullOrEmpty(model.PasswordOld)
                    && string.IsNullOrWhiteSpace(model.PasswordNew)
                    && string.IsNullOrWhiteSpace(model.Passwordconfluent)))
                {
                    user.ModifiedBy = model.UserNameModified;
                    user.ModifiedDate = DateTime.Now;
                    user.Address = model.Address;
                    user.BirthDay = model.BirthDay;
                    user.Name = model.Name;
                    user.UserName = model.UserName.Trim();
                    user.Phone = model.Phone != null ? model.Phone.Trim() : string.Empty;
                    user.Email = model.Email != null ? model.Email.Trim() : string.Empty;
                    if (!string.IsNullOrWhiteSpace(model.PasswordNew))
                    {
                        user.Password = ConvertMD5.GetMd5Hash(model.PasswordNew);
                    }
                    db.SaveChanges();
                }
                else
                {
                    result.ErrorCode = (int)Notification.ErrorCode.error;
                    result.Data = "Mật khẩu mới và cũ không giống nhau.";
                }
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.notfound;
                result.Data = "Không tìm thấy người dùng hiện tại.";
            }
            return result;
        }

        public IQueryable<News_User> Search(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return GetAll();
            }
            text = StringExtensions.ConvertToUnSign(text.Trim().ToLower());
            var model = db.News_User.AsEnumerable()
                                                     .Where(o => StringExtensions.ConvertToUnSign(o.UserName).Contains(text)
                                                                || StringExtensions.ConvertToUnSign(o.Name).Contains(text)
                                                                || StringExtensions.ConvertToUnSign(o.Address).Contains(text)
                                                                || StringExtensions.ConvertToUnSign(o.Email).Contains(text))
                                                                .AsQueryable();
            return model;
        }
    }
}
