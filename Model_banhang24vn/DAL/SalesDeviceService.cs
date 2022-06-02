using Model_banhang24vn.Common;
using Model_banhang24vn.CustomView;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.DAL
{
    public class SalesDeviceService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<SalesDevice> _SalesDevice;
        IRepository<News_User> _News_User;
        IRepository<SalesGroupDevice> _SalesGroupDevice;
        IRepository<SalesImgDevice> _SalesImgDevice;
        public SalesDeviceService()
        {
            _SalesImgDevice = unitOfWork.GetRepository<SalesImgDevice>();
            _SalesGroupDevice = unitOfWork.GetRepository<SalesGroupDevice>();
            _News_User = unitOfWork.GetRepository<News_User>();
            _SalesDevice = unitOfWork.GetRepository<SalesDevice>();
        }

        public IQueryable<SalesDevice> Query {get{ return _SalesDevice.All(); } }

        public IQueryable<SalesDevice> GetDataForGroupDevices(long? GroupId)
        {
            return _SalesDevice.Filter(o => o.GroupDeviceId == GroupId);
        }

        public IQueryable<SalesDeviceView> GetSelectJoin(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return from sale in _SalesDevice.All()
                       join user in _News_User.All()
                       on sale.EditUser equals user.ID
                       into users
                       from x in users.DefaultIfEmpty()
                       select new SalesDeviceView
                       {
                           ID = sale.ID,
                           EditUser = x != null ? x.Name : string.Empty,
                           Name = sale.Name,
                           ApplicationReal = sale.ApplicationReal,
                           DigitalInformation = sale.DigitalInformation,
                           EditDate = sale.EditDate,
                           GroupDeviceId = sale.GroupDeviceId,
                           GroupDeviceName = sale.SalesGroupDevice != null ? sale.SalesGroupDevice.Name : null,
                           SpecialPoint = sale.SpecialPoint,
                           Status = sale.Status,
                           TimeGuarantee = sale.TimeGuarantee,
                           Trademark = sale.Trademark,
                           ViewBuy = sale.ViewBuy,
                           Price = sale.Price,
                           Url=sale.Url,
                           PriceSale = sale.PriceSale,
                           IsSalePrice = sale.IsSalePrice,
                           Encoder=sale.Encoder,
                           SalesImgDevices = sale.SalesImgDevices.Select(c => new {
                               ID = c.ID,
                               SrcImage = c.SrcImage,
                               SalesDeviceID = c.SalesDeviceID
                           }).AsQueryable()

                       };
            }
          return from sale in _SalesDevice.All()
                       join user in _News_User.All()
                       on sale.EditUser equals user.ID
                       into users
                       from x in users.DefaultIfEmpty()
                       where  ( sale.Name.ToLower().Contains(search.ToLower())
                            || (x!=null && x.Name.ToLower().Contains(search)))
                       select new SalesDeviceView
                       {
                           ID = sale.ID,
                           EditUser = x!=null?x.Name:string.Empty,
                           Name = sale.Name,
                           ApplicationReal = sale.ApplicationReal,
                           DigitalInformation = sale.DigitalInformation,
                           EditDate = sale.EditDate,
                           GroupDeviceId = sale.GroupDeviceId,
                           GroupDeviceName = sale.SalesGroupDevice != null ? sale.SalesGroupDevice.Name : null,
                           SpecialPoint=sale.SpecialPoint,
                           Status=sale.Status,
                           TimeGuarantee=sale.TimeGuarantee,
                           Trademark=sale.Trademark,
                           ViewBuy=sale.ViewBuy,
                           Price=sale.Price,
                           Url = sale.Url,
                           PriceSale =sale.PriceSale,
                           IsSalePrice=sale.IsSalePrice,
                           SalesImgDevices = sale.SalesImgDevices.Select(c=>new {
                               ID=c.ID,
                               SrcImage=c.SrcImage,
                               SalesDeviceID=c.SalesDeviceID
                           }).AsQueryable()

                       };
        }

        public IQueryable<SalesGroupDevice> GetAllGroupDevices()
        {
            return _SalesGroupDevice.All();
        }

        public int InsertSalesDevices(SalesDevice model)
        {
           var GroupDevices = _SalesGroupDevice.Filter(o => o.ID == model.GroupDeviceId).FirstOrDefault();
            if (GroupDevices != null)
            {
                var data = _SalesDevice.Create(model);
                unitOfWork.Save();
                model.Encoder = GroupDevices.Encoder + "" + data.ID;
                model.Url = "/San-pham/" + model.ID + "/" + StaticVariable.ConvetTitleToUrl(model.Name);
                if (model.SalesImgDevices != null && model.SalesImgDevices.Count > 0)
                {
                    var listImg = model.SalesImgDevices.Select(o => new SalesImgDevice
                    {
                        SalesDeviceID = data.ID,
                        SrcImage = o.SrcImage
                    }).ToList();
                    _SalesImgDevice.Create(listImg);
                }
                unitOfWork.Save();
                return (int)Notification.ErrorCode.success;
            }
            return (int)Notification.ErrorCode.notfound;
        }

        public JsonViewModel<string> UpdateSalesDevices(SalesDevice model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.error };
            var data = _SalesDevice.Filter(o => o.ID.Equals(model.ID)).FirstOrDefault();
            var GroupDevices = _SalesGroupDevice.Filter(o => o.ID == model.GroupDeviceId).FirstOrDefault();
            if (data == null)
            {
                result.Data = "bản ghi không tồn tại hoặc đã bị xóa, vui lòng tải lại trang";
            }
            else
            {
                data.ApplicationReal = model.ApplicationReal;
                data.DigitalInformation = model.DigitalInformation;
                data.Name = model.Name;
                data.EditDate = model.EditDate;
                data.EditUser = model.EditUser;
                data.Url = "/San-pham/" + data.ID + "/" + StaticVariable.ConvetTitleToUrl(data.Name);
                data.GroupDeviceId = model.GroupDeviceId;
                data.IsSalePrice = model.IsSalePrice;
                data.Price = model.Price;
                if (model.IsSalePrice == true)
                {
                    data.PriceSale = model.PriceSale;
                }
                data.SpecialPoint = model.SpecialPoint;
                data.Status = model.Status;
                data.TimeGuarantee = model.TimeGuarantee;
                data.Trademark = model.Trademark;
                if (model.SalesImgDevices != null && model.SalesImgDevices.Count > 0)
                {
                    var listimgIdOld = model.SalesImgDevices.Where(o => o.ID != 0).Select(o => o.ID);
                    var listImgNew = model.SalesImgDevices.Where(o => o.ID == 0).Select(o => new SalesImgDevice
                    {
                        SalesDeviceID = data.ID,
                        SrcImage = o.SrcImage
                    });
                    _SalesImgDevice.Delete(o => !listimgIdOld.Contains(o.ID) && o.SalesDeviceID == data.ID);
                    _SalesImgDevice.Create(listImgNew);
                }
                else
                {
                    _SalesImgDevice.Delete(o => o.SalesDeviceID == data.ID);
                }
                data.Encoder = GroupDevices != null ? GroupDevices.Encoder + "" + data.ID:"SP_"+data.ID;
                _SalesDevice.Update(data);
                unitOfWork.Save();
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
             return result;
            
        }

        public void DeleteSalesDevices( long Id)
        {
            var model = _SalesDevice.Filter(o => o.ID == Id).FirstOrDefault();
                if(model != null) {
                _SalesImgDevice.Delete(o => o.SalesDeviceID == model.ID);
                _SalesDevice.Delete(model);
                unitOfWork.Save();
            }
        }

        public JsonViewModel<string > InsertSalesGroupDevices(SalesGroupDevice model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.error };

            if (_SalesGroupDevice.All().Any(o => o.Name.ToLower().Equals(model.Name.ToLower())))
            {
                result.Data = "Tên nhóm thiết bị đã bị trùng";
            }
            else
            {
              var data =  _SalesGroupDevice.Create(model);
                unitOfWork.Save();
                model.Encoder = StaticVariable.GetCharFirst(model.Name) + "_" + data.ID;
                unitOfWork.Save();
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;
        }

        public JsonViewModel<string> UpdateSalesGroupDevices(SalesGroupDevice model)
        {
            var result = new JsonViewModel<string>() { ErrorCode= (int)Notification.ErrorCode.error };
            var data = _SalesGroupDevice.Filter(o => o.ID==model.ID).FirstOrDefault();
            if (data == null)
            {
                result.Data = "Nhóm thiết bị đã bị xóa vui lòng tải lại trang";
            }
            else if ( _SalesGroupDevice.Filter(o => o.ID != model.ID
               && o.Name.ToLower().Equals(model.Name.ToLower())).Any())
            {
                result.Data = "Tên nhóm thiết bị đã bị trùng";
            }
            else if(model.Status==false && _SalesDevice.All().Any(o=>o.Status==true && o.GroupDeviceId==model.ID))
            {
                result.Data = "Không thể vô hiệu hóa nhóm khi tồn tại thiết bị đang sử dụng loại nhóm này";
            }
            else
            {
                data.Name = model.Name;
                data.Note = model.Note;
                data.Status = model.Status;
                data.Encoder = StaticVariable.GetCharFirst(data.Name) + "_" + data.ID;
                _SalesGroupDevice.Update(data);
                unitOfWork.Save();
                result.ErrorCode = (int)Notification.ErrorCode.success;
            }
            return result;

        }
        public JsonViewModel<string> DeleteSalesGroupDevices(long Id)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            var model = _SalesGroupDevice.Filter(o => o.ID == Id).FirstOrDefault();
            if (model != null)
            {
                if (_SalesDevice.Filter(o => o.GroupDeviceId == model.ID).Any())
                {
                    result.ErrorCode= (int)Notification.ErrorCode.error;
                    result.Data = "Không thể xóa do đã tồn tại thiết bị đang sử dụng nhóm này";
                }
                else
                {
                    _SalesGroupDevice.Delete(model);
                    unitOfWork.Save();
                }
            }
            return result;
        }
    }
}
