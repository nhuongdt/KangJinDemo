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
    public class RegisterServiceSmsService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<RegisterServiceSm> _RegisterServiceSm;
        IRepository<SupplierSm> _SupplierSmService;
        public RegisterServiceSmsService()
        {
            _RegisterServiceSm = unitOfWork.GetRepository<RegisterServiceSm>();
            _SupplierSmService = unitOfWork.GetRepository<SupplierSm>();
        }

        public IQueryable<RegisterServiceSm> Query { get { return _RegisterServiceSm.All(); } }
        public IQueryable<SupplierSm> Query_Supli { get { return _SupplierSmService.All(); } }

        public void InsertBrandName(RegisterServiceSm model)
        {
            _RegisterServiceSm.Create(model);
            unitOfWork.Save();
        }

        public void UpdateBrandName(RegisterServiceSm model)
        {
            RegisterServiceSm branname = GetBrandNameById(model.ID);
            branname.Name = model.Name;
            branname.Note = model.Note;
            _RegisterServiceSm.Update(branname);
            unitOfWork.Save();
        }

        public void UpdateBrandNameDelete(Guid id)
        {
            RegisterServiceSm branname = GetBrandNameById(id);
            branname.Status = 0; // 0 : xóa , 1: kích hoạt , 2: Chờ kích hoạt
            _RegisterServiceSm.Update(branname);
            unitOfWork.Save();
        }

        public List<RegisterServiceSmDTO> GetAllBrandName(string str)
        {
            var data = from brand in Query
                       join sms in _SupplierSmService.All() on brand.ID_SupplierSms equals sms.ID
                       where brand.Status != 0 && brand.SoDienThoaiCuaHang == str
                       orderby brand.CreateDate descending
                       select new RegisterServiceSmDTO()
                       {
                           ID = brand.ID,
                           TenNhaCungCap = sms.Name,
                           BrandName = brand.Name,
                           NgayTao = brand.CreateDate,
                           GhiChu = brand.Note,
                           TrangThai = brand.Status == 1 ? "Kích hoạt" : (brand.Status == 2 ? "Chờ kích hoạt" : "Xóa"),
                           Status = brand.Status
                       };
            return data.ToList();
        }

        public RegisterServiceSm GetBrandNameById(Guid id)
        {
            return Query.Where(o => o.ID == id).FirstOrDefault();
        }

        public SupplierSm GetSupplierByID(Guid id)
        {
            return Query_Supli.Where(o => o.ID == id).FirstOrDefault();
        }

        public bool CheckBrandNameExist(string nameBrand)
        {
            List<RegisterServiceSm> demo = Query.Where(o => o.Name == nameBrand.Trim() && o.Status != 0).ToList();
            if (demo.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckBrandNameExistEdit(string nameBrand, Guid id)
        {
            List<RegisterServiceSm> demo = Query.Where(o => o.Name == nameBrand.Trim() && o.ID != id && o.Status != 0).ToList();
            if (demo.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IQueryable<RegisterServiceSm> SearGridServiceSms(string text)
        {
            var data = Query;
            if (!string.IsNullOrWhiteSpace(text))
            {
                data = data.Where(o => o.Name.Contains(text) || o.SoDienThoaiCuaHang.Contains(text));
            }
            return data;
        }

        public IQueryable<RegisterServiceSm> SearGridDetailServiceSms(string phone)
        {
            var data = Query;
            if (!string.IsNullOrWhiteSpace(phone))
            {
                data = data.Where(o =>  o.SoDienThoaiCuaHang.Equals(phone));
            }
            return data;
        }
        public IQueryable<SupplierSm> GetAllSupplierSms()
        {
            return _SupplierSmService.All();
        }

        public JsonViewModel<string> Update(RegisterServiceSm model)
        {
            var result = new JsonViewModel<string> { ErrorCode = (int)Notification.ErrorCode.error };
            var data = _RegisterServiceSm.Filter(o => o.ID == model.ID).FirstOrDefault();
            if (data == null)
            {
                result.Data = "Bản ghi không tồn tại hoặc đã bị xóa";
            }
            else
            {
                data.Status = model.Status;
                if (data.Status == (int)Notification.ServiceSms.dakichhoat)
                {
                    data.Price = model.Price;
                }
                data.ID_SupplierSms = model.ID_SupplierSms;
                data.DateActivated = DateTime.Now;
                data.UserActivated = model.UserActivated;
                unitOfWork.Save();
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }
    }

    public class RegisterServiceSmDTO
    {
        public Guid ID { get; set; }
        public string TenNhaCungCap { get; set; }
        public string BrandName { get; set; }
        public DateTime NgayTao { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
        public int? Status { get; set; }
    }

    public class SupplierSmService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<SupplierSm> _SupplierSm;


        public SupplierSmService()
        {
            _SupplierSm = unitOfWork.GetRepository<SupplierSm>();
        }

        public IQueryable<SupplierSm> Query { get { return _SupplierSm.All(); } }

        public void InsertBrandName(SupplierSm model)
        {
            _SupplierSm.Create(model);
            unitOfWork.Save();
        }

        public IQueryable<SupplierSm> GetSupplierDefault()
        {
            return Query.Where(o => o.IsDefault == true);
        }

        public SupplierSm GetAPIKeyByIDSupllierSMS(Guid id)
        {
            return Query.Where(o => o.ID == id).FirstOrDefault();
        }
    }
}
