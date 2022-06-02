using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model_banhang24vn.Common;
using System.Web.Configuration;

namespace Model_banhang24vn.DAL
{
 public   class AdvertisingService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<Advertisement> _Advertisement;
        public AdvertisingService()
        {
            _Advertisement = unitOfWork.GetRepository<Advertisement>();
        }

        public IQueryable<Advertisement> GetAll()
        {
            return _Advertisement.All();
        }

        public IQueryable<Advertisement> SearhGrid(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return GetAll();
            }
            else
            {
                search = search.ToLower().Trim();
                return _Advertisement.Filter(o => (o.Title != null && o.Title.ToLower().Contains(search))
                                        || (o.UrlImage != null && o.UrlImage.ToLower().Contains(search)));
            }
        }

        public void Insert(Advertisement model)
        {
            if (model.ToDate != null && model.FromDate == null)
                model.FromDate = DateTime.Now;
            model.EditDate = DateTime.Now;
            model.UrlImage = WebConfigurationManager.AppSettings["Webhttp"].ToString() + model.UrlImage;
            _Advertisement.Create(model);
            unitOfWork.Save();
        }
        public JsonViewModel<string> Update(Advertisement model)
        {
            var result = new JsonViewModel<string> { ErrorCode = (int)Notification.ErrorCode.success };
            var data = _Advertisement.Filter(o => o.ID == model.ID).FirstOrDefault();
            if(data!=null)
            {
                if (model.ToDate != null && model.FromDate == null)
                    data.FromDate = DateTime.Now;
                else
                    data.FromDate = model.FromDate;
                data.EditDate = DateTime.Now;
                data.Title = model.Title;
                data.Status = model.Status;
                data.Position = model.Position;
                data.EditUser = model.EditUser;
                data.Link = model.Link;
                data.ToDate = model.ToDate;
                if(!data.UrlImage.Contains(model.UrlImage))
                 data.UrlImage = WebConfigurationManager.AppSettings["Webhttp"].ToString() + model.UrlImage;
                unitOfWork.Save();
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Bản ghi đã bị xóa hoặc không tồn tại, vui lòng thử lại sau";

            }
            return result;
        }
        public void delete(long id)
        {
            _Advertisement.Delete(o => o.ID == id);
            unitOfWork.Save();
        }
    }
}
