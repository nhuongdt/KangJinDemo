using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
    public class CustomerService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<Customer> _Customer;
        IRepository<NganhNgheKinhDoanh> _NganhNgheKinhDoanh;
        IRepository<TinhThanh_QuanHuyen> _DM_TinhThanh;
        public CustomerService()
        {
            _Customer = unitOfWork.GetRepository<Customer>();
            _NganhNgheKinhDoanh = unitOfWork.GetRepository<NganhNgheKinhDoanh>();
            _DM_TinhThanh = unitOfWork.GetRepository<TinhThanh_QuanHuyen>();
        }

        public IQueryable<Customer> GetAll()
        {
            return _Customer.All().OrderByDescending(o=>o.CreatedDate);
        }

        public IQueryable<CustomerView> GetAllDetailJoin()
        {
            var data = from nn in _NganhNgheKinhDoanh.All()
                       join cmer in _Customer.All()
                       on nn.ID equals cmer.TypeBusiness
                       join tt in _DM_TinhThanh.All()
                       on cmer.DistrictCity equals tt.ID
                       orderby cmer.CreatedDate descending
                       select new CustomerView
                       {
                           ID = cmer.ID,
                           Adress = cmer.Adress,
                           CreatedBy = cmer.CreatedBy,
                           CreatedDate = cmer.CreatedDate,
                           Description = cmer.Description,
                           DistrictCityname = tt.QuanHuyen+ " - " + tt.TinhThanh,
                           Email = cmer.Email,
                           Images = cmer.Images,
                           ModifiedBy = cmer.ModifiedBy,
                           ModifiedDate = cmer.ModifiedDate,
                           Name = cmer.Name,
                           Note = cmer.Note,
                           Phone = cmer.Phone,
                           Status = cmer.Status,
                           TypeBusinessname = nn.TenNganhNghe,
                           Url = cmer.Url,
                           DistrictCityId=tt.ID,
                           TypeBusinessId=nn.ID,
                           prioritize=cmer.prioritize
                       };
            return data;
        }

        public IQueryable<CustomerView> SearhGrid(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return GetAllDetailJoin();
            }
            text = StringExtensions.ConvertToUnSign(text.Trim());
            var data = GetAllDetailJoin().AsEnumerable().Where(o => StringExtensions.ConvertToUnSign(o.Name).Contains(text)
                                                 || StringExtensions.ConvertToUnSign(o.TypeBusinessname).Contains(text)
                                                 || StringExtensions.ConvertToUnSign(o.DistrictCityname).Contains(text)
                                                 || StringExtensions.ConvertToUnSign(o.Phone).Contains(text)
                                                 || StringExtensions.ConvertToUnSign(o.Email).Contains(text)
                                                 || StringExtensions.ConvertToUnSign(o.CreatedBy ).Contains(text));
            return data.AsQueryable();
        }

        public JsonViewModel<string> Insert(Customer model)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            //model.Url = StaticVariable.ConvetTitleToUrl(model.Name);
            model.CreatedDate = DateTime.Now;
            model.ModifiedDate = DateTime.Now;
            model.ModifiedBy = model.CreatedBy;
            model.ViewCount = 0;
            model.prioritize = 0;
            _Customer.Create(model);
            unitOfWork.Save();
             model.Url = string.Format("/khach-hang/{0}-{1}.html", model.Url, model.ID);
            unitOfWork.Save();
            return result;
        }

        public JsonViewModel<string> Update(Customer model)
        {
            var result = new JsonViewModel<string>();
            result.ErrorCode = (int)Notification.ErrorCode.success;
            var curent = _Customer.GetById(model.ID);
            if (curent != null)
            {
                curent.Adress = model.Adress;
                curent.Description = model.Description;
                curent.DistrictCity = model.DistrictCity;
                curent.Email = model.Email;
                if (!curent.Images.Equals(model.Images))
                {
                    result.Data = curent.Images;
                }
                curent.Images = model.Images;
                curent.ModifiedBy = model.ModifiedBy;
                curent.Name = model.Name;
                curent.Note = model.Note;
                curent.Phone = model.Phone;
                curent.Status = model.Status;
                curent.TypeBusiness = model.TypeBusiness;
                //curent.Url = StaticVariable.ConvetTitleToUrl(model.Name);
                curent.ModifiedDate = DateTime.Now;
                curent.prioritize = model.prioritize;
                curent.Url = string.Format("/khach-hang/{0}-{1}.html", model.Url, curent.ID);
                _Customer.Update(curent);
                unitOfWork.Save();
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.notfound;
                result.Data = "Đối tác không tồn tại hoặc đã bị xóa, vui lòng kiểm tra lại.";
            }

            return result;
        }

        public int Delete(int Id)
        {
            var model = _Customer.All().FirstOrDefault(o => o.ID == Id);
            if (model == null)
            {
                return (int)Notification.ErrorCode.notfound;
            }
            else
            {
                _Customer.Delete(model);
                unitOfWork.Save();
                return (int)Notification.ErrorCode.success;
            }
        }
        public Customer GetByDetail(int Id)
        {
            var result = _Customer.GetById(Id);
            if (result != null)
            {
                result.ViewCount += 1;
                unitOfWork.Save();
            }
            return result;
        }
        public Customer GetByDetail(string url)
        {
            var result = _Customer.Filter(o=>o.Url.ToLower().Equals(url.ToLower())).First();
            if (result != null)
            {
                result.ViewCount += 1;
                unitOfWork.Save();
            }
            return result;
        }
        public Customer GetBykey(int Id)
        {
            return _Customer.GetById(Id);
        }
        public Customer GetViewDetail(int Id)
        {
            var result = _Customer.GetById(Id);
            if (result != null)
            {
                var user = unitOfWork.GetRepository<News_User>().Filter(o => o.UserName == result.CreatedBy).FirstOrDefault();
                result.CreatedBy = user != null ? user.Name : string.Empty;
            }
            return result;
        }
        public IQueryable<CustomerView> SearFilter(string text,Guid? businesId,Guid? AdressId)
        {
            var data = SearhGrid(text);
            if(businesId!=null && businesId!=new Guid())
            {
                data = data.Where(o => o.TypeBusinessId == businesId);
            }
            if(AdressId!= null && AdressId != new Guid())
            {
                var AddressName = _DM_TinhThanh.Find(AdressId).TinhThanh;
                var listQhTt = _DM_TinhThanh.Filter(o => o.TinhThanh == AddressName).Select(o => o.ID).ToList();
                data = data.Where(o => listQhTt.Contains(o.DistrictCityId??new Guid()));
            }
            return data;

        }
    }
}
