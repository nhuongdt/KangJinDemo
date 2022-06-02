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
    public class ContactService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<Contact> _Contact;
        public ContactService()
        {
            _Contact = unitOfWork.GetRepository<Contact>();
        }

        public void Insert(Contact model)
        {
            _Contact.Create(model);
            unitOfWork.Save();
        }

        public IQueryable<Contact> GetAll()
        {
            return _Contact.All();
        }

        public IQueryable<Contact> GetAllLienHe()
        {
            return _Contact.All().Where(o=>o.Type== (int)Notification.TypeContact.lienhe);
        }

        public IQueryable<Contact> SearhGrid(string search, int typeSoftWare = -1, int type= (int)Notification.TypeContact.lienhe)
        {
            var data = _Contact.Filter(o => o.Type == type);
            if (typeSoftWare >= 0)
            {
                data = data.Where(o => o.TypeSoftWare == typeSoftWare);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(o => (o.FullName != null && o.FullName.ToLower().Contains(search))
                                             || (o.Phone != null && o.Phone.ToLower().Contains(search))
                                             || (o.Email != null && o.Email.Contains(search))
                                             || (o.Address != null && o.Address.Contains(search)));
            }
           
            return data;
        }
        public IQueryable<Contact> SearhOrderGrid(string search, int typeSoftWare = -1)
        {
            var data = _Contact.Filter(o => o.Type != (int)Notification.TypeContact.lienhe);
            if (typeSoftWare >= 0)
            {
                data = data.Where(o => o.TypeSoftWare == typeSoftWare);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(o => (o.FullName != null && o.FullName.ToLower().Contains(search))
                                             || (o.Phone != null && o.Phone.ToLower().Contains(search))
                                             || (o.Email != null && o.Email.Contains(search))
                                             || (o.Address != null && o.Address.Contains(search)));
            }

            return data;
        }
        public JsonViewModel<string> SaveContact(Contact model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            var data = _Contact.Filter(o => o.ID == model.ID).FirstOrDefault();
            if (data != null)
            {
                data.Status = model.Status;
                result.Data = "Cập nhật trạng thái đơn đặt hàng thành công";
                unitOfWork.Save();
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Không tồn tại bản ghi hoặc bản ghi đã bị xóa";
            }
            return result;
        }
    }
}
