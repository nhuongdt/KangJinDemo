using Model_banhang24vn.Common;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
   public class MenuTagsService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<MenuTag> _MenuTag;
        public MenuTagsService()
        {
            _MenuTag = unitOfWork.GetRepository<MenuTag>();
        }

        public MenuTag GetByAction(string controller, string actions)
        {
            return _MenuTag.Filter(o => o.Controller.ToUpper().Equals(controller.ToUpper()) && o.Actions.ToUpper().Equals(actions.ToUpper())).FirstOrDefault();
        }

        public IQueryable<MenuTag> GetAll()
        {
            return _MenuTag.All().OrderByDescending(o=>o.CreateDate);
        }
        public bool CheckLink(string Link, int Id = 0)
        {
            if (Id == 0)
            {
                return _MenuTag.Filter(o => o.Link.ToLower().Equals(Link.ToLower())).Any();
            }
            else
            {
                return _MenuTag.Filter(o => o.Link.ToLower().Equals(Link.ToLower()) && o.ID!=Id).Any();
            }
        }
        public bool CheckTenMenu(string text, int Id = 0)
        {
            if (Id == 0)
            {
                return _MenuTag.Filter(o => o.Text.ToLower().Equals(text.ToLower())).Any();
            }
            else
            {
                return _MenuTag.Filter(o => o.Text.ToLower().Equals(text.ToLower()) && o.ID != Id).Any();
            }
        }
        public JsonViewModel<string> Insert(MenuTag model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.error };
          
            if (string.IsNullOrWhiteSpace(model.Link))
            {
                result.Data = "Vui lòng nhập link";
            }
            else if (CheckLink(model.Link))
            {
                result.Data = "Link đã tồn tại";
            }
            else if (string.IsNullOrWhiteSpace(model.Text))
            {
                result.Data = "Vui lòng nhập tên menu";
            }
            else if (CheckTenMenu(model.Text))
            {
                result.Data = "Tên menu đã tồn tại";
            }
            else
            {
                model.CreateDate = DateTime.Now;
                _MenuTag.Create(model);
                unitOfWork.Save();
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }

        public JsonViewModel<string> Update(MenuTag model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.error };
            var data = _MenuTag.Find(model.ID);
            if (string.IsNullOrWhiteSpace(model.Link))
            {
                result.Data = "Vui lòng nhập link";
            }
            else if (CheckLink(model.Link,model.ID))
            {
                result.Data = "Link đã tồn tại";
            }
            else if (string.IsNullOrWhiteSpace(model.Text))
            {
                result.Data = "Vui lòng nhập tên menu";
            }
            else if (CheckTenMenu(model.Text, model.ID))
            {
                result.Data = "Tên menu đã tồn tại";
            }
            else if (data==null)
            {
                result.Data = "Menu không tồn tại hoặc đã bị xóa";
            }
            else
            {
                data.Text = model.Text;
                data.Status = model.Status;
                data.Link = model.Link;
                data.Title = model.Title;
                data.Tags = model.Tags;
                data.Description = model.Description;
                unitOfWork.Save();
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }

        public bool Delete(MenuTag model)
        {
            var data = _MenuTag.Find(model.ID);
            if (data == null)
            {
                return false;
            }
            _MenuTag.Delete(data);
            unitOfWork.Save();
            return true;
        }
        public MenuTag GetMetaTags(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return new MenuTag();
            var result = GetAll().Where(o=>o.Status==true || o.Status==null).FirstOrDefault(o => o.Link.ToUpper().Equals(filePath.ToUpper()));
            return result;
        }


        public IQueryable<MenuTag> SearhGrid(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return GetAll();
            }
            return GetAll().Where(o => o.Text.ToLower().Contains(text.ToLower()));
        }
    }
}
