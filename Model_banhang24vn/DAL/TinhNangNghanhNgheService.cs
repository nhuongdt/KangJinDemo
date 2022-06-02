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
  public  class TinhNangNghanhNgheService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<BanHang24vnContext>());
        IRepository<TinhNangNghanhNghe> _TinhNangNghanhNghe;

        public TinhNangNghanhNgheService()
        {
            _TinhNangNghanhNghe = unitOfWork.GetRepository<TinhNangNghanhNghe>();
        }

        public IQueryable<TinhNangNghanhNghe> Query { get { return _TinhNangNghanhNghe.All(); } }

        public IQueryable<TinhNangNghanhNghe> GetDetailForNganhNgheId(Guid? nganhngheId)
        {
            return _TinhNangNghanhNghe.Filter(o => o.Id_NganhNghe == nganhngheId);
        }

        public JsonViewModel<string> Insert(TinhNangNghanhNghe model, List<AnhTinhNangNghanhNghe> listImage)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            //int stt = _TinhNangNghanhNghe.All().Any(o=>o.Id_NganhNghe==model.Id_NganhNghe) ? _TinhNangNghanhNghe.Filter(o => o.Id_NganhNghe == model.Id_NganhNghe).Max(o => o.STT??0) + 1 : 1;
            var resultData=   _TinhNangNghanhNghe.Create(model);
            if (listImage!=null &&listImage.Any())
            {
                var modelImage = listImage.Select(o => new AnhTinhNangNghanhNghe
                {
                    Id_NganhNgheTinhNang = resultData.Id,
                    SrcImage = o.SrcImage,
                    Note = o.Note

                }).ToList();
                unitOfWork.GetRepository<AnhTinhNangNghanhNghe>().Create(modelImage);
            }
            unitOfWork.Save();
            return result;

        }

        public JsonViewModel<string> Update(TinhNangNghanhNghe model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            var data = _TinhNangNghanhNghe.Filter(o => o.Id == model.Id).FirstOrDefault();
            var _AnhTinhNangNghanhNghe = unitOfWork.GetRepository<AnhTinhNangNghanhNghe>();
            if (data != null)
            {
                data.TenTinhNang = model.TenTinhNang;
                data.TieuDe = model.TieuDe;
                data.DateEdit = model.DateEdit;
                data.NoiDung = model.NoiDung;
                data.Status = model.Status;
                data.UserEdit = model.UserEdit;
                data.STT = model.STT;
                if (model.AnhTinhNangNghanhNghes != null && model.AnhTinhNangNghanhNghes.Any())
                {
                    var imageOld = model.AnhTinhNangNghanhNghes.Where(o => o.Id != 0).Select(o => o.Id).ToArray();
                    var imagenew = model.AnhTinhNangNghanhNghes.Where(o =>  o.Id == 0).Select(
                        o => new AnhTinhNangNghanhNghe
                        {
                            Id_NganhNgheTinhNang = data.Id,
                            Note = o.Note,
                            SrcImage = o.SrcImage
                        }).ToList();
                    var listdelete = _AnhTinhNangNghanhNghe.Filter(o => o.Id_NganhNgheTinhNang==data.Id&& !imageOld.Contains(o.Id));
                    var listUpdate = _AnhTinhNangNghanhNghe.Filter(o => o.Id_NganhNgheTinhNang == data.Id && imageOld.Contains(o.Id));
                    foreach(var item in listUpdate)
                    {
                        item.Note = model.AnhTinhNangNghanhNghes.First(o => o.Id.Equals(item.Id)).Note;
                        _AnhTinhNangNghanhNghe.Update(item);

                    }
                    foreach (var item in listdelete)
                    {
                        _AnhTinhNangNghanhNghe.Delete(item);
                    }
                    if (imagenew.Count > 0)
                    {
                        _AnhTinhNangNghanhNghe.Create(imagenew);
                    }
                }
                else
                {
                    _AnhTinhNangNghanhNghe.Delete(o => o.Id_NganhNgheTinhNang == data.Id);
                }
                unitOfWork.Save();
            }
            else
            {
                result.ErrorCode = (int)Notification.ErrorCode.error;
                result.Data = "Không tìm thấy bản ghi vừa cập nhật, vui lòng tải lại trang.";
            }
            return result;

        }

        public JsonViewModel<string> Delete(long id)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)Notification.ErrorCode.success };
            var model = _TinhNangNghanhNghe.Find(id);
            if (model == null)
            {
                result.Data = "Tính năng ngành nghề không tồn tại hoặc đã bị xóa, vui lòng kiểm tra lại.";
                result.ErrorCode = (int)Notification.ErrorCode.error;
            }
            else
            {
                var listImageDelete = model.AnhTinhNangNghanhNghes;
                if(listImageDelete!=null && listImageDelete.Any())
                {
                    var _AnhTinhNangNghanhNghe = unitOfWork.GetRepository<AnhTinhNangNghanhNghe>();
                    _AnhTinhNangNghanhNghe.Delete(listImageDelete.ToList());
                }
                _TinhNangNghanhNghe.Delete(model);
                unitOfWork.Save();
            }
            return result;
        }

        public IQueryable<AnhTinhNangNghanhNghe> GetImageForNganhNgheId(Guid? nganhNgheId)
        {
            return from busnines in _TinhNangNghanhNghe.All()
                       join img in unitOfWork.GetRepository<AnhTinhNangNghanhNghe>().All()
                       on busnines.Id equals img.Id_NganhNgheTinhNang
                       where busnines.Id_NganhNghe == nganhNgheId
                       select img;


        }
    }
}
