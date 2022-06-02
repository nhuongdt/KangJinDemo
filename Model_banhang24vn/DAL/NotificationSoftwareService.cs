using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model_banhang24vn.Common;
namespace Model_banhang24vn.DAL
{
   public class NotificationSoftwareService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<NotificationSoftware> _NotificationSoftware;
        public NotificationSoftwareService()
        {
            _NotificationSoftware = unitOfWork.GetRepository<NotificationSoftware>();
        }

        public IQueryable<NotificationSoftware> Query { get { return _NotificationSoftware.All(); } }

        public IQueryable<NotificationSoftware> SearhGrid(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Query;
            return _NotificationSoftware.Filter(o => o.Subdomain.Contains(text) || o.Title.Contains(text) || o.EditUser.Contains(text));
        }

        public void Insert(NotificationSoftware model)
        {
            _NotificationSoftware.Create(model);
            unitOfWork.Save();
        }
        public void Delete(NotificationSoftware model)
        {
            _NotificationSoftware.Delete(model.ID);
            unitOfWork.Save();
        }
        public int Update(NotificationSoftware model)
        {
            var data = _NotificationSoftware.Find(o => o.ID == model.ID);
            if (data==null)
            {
                return (int)Notification.ErrorCode.error;
            }
            data.ApplyDate = model.ApplyDate;
            data.BodyContent = model.BodyContent;
            data.EditDate = model.EditDate;
            data.EditUser = model.EditUser;
            data.Status = model.Status;
            data.Subdomain = model.Subdomain;
            data.Title = model.Title;
            data.Type = model.Type;
            unitOfWork.Save();
            return (int)Notification.ErrorCode.success;
        }

    }
}
