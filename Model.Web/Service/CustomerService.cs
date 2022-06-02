using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Web.API;
using Ssoft.Common.Common;

namespace Model.Web.Service
{
   public  class CustomerService
    {
        private SsoftvnWebContext db;
        public CustomerService()
        {
            db = SystemDBContext.GetDBContext();
        }
        public IQueryable<DM_KhachHang> getAll()
        {
            return db.DM_KhachHang.AsQueryable();
        }

        public IQueryable<DM_KhachHang> SearchClient(string text, string adress, string product)
        {
            var data = db.DM_KhachHang.Where(o => o.TrangThai != (int)LibEnum.IsStatus.an);
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.Normalize(NormalizationForm.FormC).ToLower();
                data = data.Where(o => o.TenKhachHang.ToLower().Contains(text));
            }
            if (!string.IsNullOrWhiteSpace(adress))
            {
                data = data.Where(o => o.MaTinhThanh.Contains(adress));
            }
            if (!string.IsNullOrWhiteSpace(product))
            {
                data = data.Where(o => o.ID_SanPham==product);
            }

             return   data.OrderByDescending(o => o.NgayTao);
        }

        public IQueryable<DM_KhachHang> SearchCustomerGrid(string text)
        {
            var data = db.DM_KhachHang.AsQueryable() ;
            if (!string.IsNullOrWhiteSpace(text))
            {
                data = data.Where(o => o.TenKhachHang.ToUpper().Equals(text.ToUpper()));
            }
            return data.OrderByDescending(o => o.NgayTao);
        }

        public void Insert(DM_KhachHang model, string tag)
        {
            model.HienThiTrangChu = true;
            db.DM_KhachHang.Add(model);
            db.SaveChanges();
            model.Link = "/khach-hang/" + model.ID + "/" + StaticVariable.ConvetTitleToUrl(model.MetaTitle);
  
            //Xử lý tag
            if (!string.IsNullOrEmpty(tag))
            {
                string[] tags = tag.Split(',').Where(o => o.Trim() != "").ToArray();

                List<string> TagNews = new List<string>();
                foreach (var item in tags)
                {
                    var tagId = StaticVariable.ToUnsignString(item.Trim());
                    if (!TagNews.Any(o => o.Equals(tagId)))
                    {
                        TagNews.Add(tagId);
                        //insert to to tag table
                        if (!db.DM_Tags.Any(o => o.ID.Equals(tagId)))
                        {
                            db.DM_Tags.Add(new DM_Tags
                            {
                                ID = tagId,
                                TenTheTag = item.Trim(),
                                KeyWordTag = StaticVariable.RemoveSign4VietnameseString(item.Trim())
                            });
                        }
                        //insert to content tag
                        db.DM_BaiViet_Tag.Add(new DM_BaiViet_Tag { ID = Guid.NewGuid(), ID_BaiViet = model.ID, ID_Tag = tagId, Loai = (int)LibEnum.StatusGroupNewsTag.khachhang });
                    }
                }
            }
            db.SaveChanges();
        }


        public JsonViewModel<string> Update(DM_KhachHang model, string tag)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)LibEnum.ErrorCode.Error };
            var data = db.DM_KhachHang.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Đối tác không tồn tại hoặc đã bị xóa";
            }
            else
            {
                data.TenKhachHang = model.TenKhachHang;
                data.NoiDung = model.NoiDung;
                data.Mota = model.Mota;
                data.NgaySua = model.NgayTao;
                data.NguoiSua = model.NguoiTao;
                data.DiaChi = model.DiaChi;
                data.Email = model.Email;
                data.SoDienThoai = model.SoDienThoai;
                data.MaTinhThanh = model.MaTinhThanh;
                data.ID_SanPham = model.ID_SanPham;

                if (!string.IsNullOrWhiteSpace(model.Anh))
                {
                    data.Anh = model.Anh;
                }
                data.TrangThai = model.TrangThai;
                data.MetaTitle = model.MetaTitle;
                data.MetaDescription = model.MetaDescription;
                data.Link = "/khach-hang/" + model.ID + "/" + StaticVariable.ConvetTitleToUrl(model.MetaTitle);
                var listTagDelete = db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == data.ID && o.Loai == (int)LibEnum.StatusGroupNewsTag.khachhang).AsEnumerable();
                db.DM_BaiViet_Tag.RemoveRange(listTagDelete);
                db.SaveChanges();
                //Xử lý tag
                if (!string.IsNullOrEmpty(tag))
                {
                    string[] tags = tag.Split(',');

                    List<string> TagNews = new List<string>();
                    foreach (var item in tags)
                    {
                        var tagId = StaticVariable.ToUnsignString(item.Trim());
                        if (!TagNews.Any(o => o.Equals(tagId)))
                        {
                            TagNews.Add(tagId);
                            //insert to to tag table
                            if (!db.DM_Tags.Any(o => o.ID.Equals(tagId)))
                            {
                                db.DM_Tags.Add(new DM_Tags
                                {
                                    ID = tagId.Normalize(NormalizationForm.FormC),
                                    TenTheTag = item.Trim(),
                                    KeyWordTag = StaticVariable.RemoveSign4VietnameseString(item.Trim())
                                });
                            }
                            //insert to content tag
                            db.DM_BaiViet_Tag.Add(new DM_BaiViet_Tag { ID = Guid.NewGuid(), ID_BaiViet = model.ID, ID_Tag = tagId, Loai = (int)LibEnum.StatusGroupNewsTag.khachhang });
                        }
                    }
                }
                result.ErrorCode = (int)LibEnum.ErrorCode.Success;
                db.SaveChanges();
            }
            return result;
        }


        public bool Delete(int id)
        {
            var model = db.DM_KhachHang.FirstOrDefault(o => o.ID == id);
            if (model == null)
                return false;
            var listtag = db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == model.ID && o.Loai == (int)LibEnum.StatusGroupNewsTag.khachhang).AsEnumerable();
            db.DM_BaiViet_Tag.RemoveRange(listtag);
            db.DM_KhachHang.Remove(model);
            db.SaveChanges();
            return true;
        }
        public IQueryable<DM_BaiViet_Tag> GetTagsNews(int id)
        {
            return db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == id && o.Loai == (int)LibEnum.StatusGroupNewsTag.khachhang);
        }
    }
}
