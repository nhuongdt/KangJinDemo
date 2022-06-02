using Model.Web.API;
using Model_banhang24vn.Infrastructure;
using Model_banhang24vn.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ssoft.Common.Common;

namespace Model.Web.Service
{
   public class NewsService
    {
        UnitOfWork unitOfWork = new UnitOfWork(new DbContextFactory<SsoftvnWebContext>());
        IRepository<DM_BaiViet> _DM_BaiViet;
        private SsoftvnWebContext db;
        public NewsService()
        {
            db = SystemDBContext.GetDBContext();
            _DM_BaiViet = unitOfWork.GetRepository<DM_BaiViet>();
        }

        public IQueryable<DM_BaiViet> getAll()
        {
            return db.DM_BaiViet.AsQueryable();
        }

        public IQueryable<DM_BaiViet> GetClient()
        {
             var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            return db.DM_BaiViet.Where(o=>o.NgayDangBai==null || o.NgayDangBai<= date).OrderBy(o=>o.ThuTuHienThi).ThenByDescending(o=>o.NgayTao);
        }

        public IQueryable<DM_BaiViet> GetNewsDate()
        {
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            return db.DM_BaiViet.Where(o => o.NgayDangBai == null || o.NgayDangBai <= date).OrderByDescending(o => o.NgayTao);
        }

        public IQueryable<DM_BaiViet> GetNewsView()
        {
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            return db.DM_BaiViet.Where(o => o.NgayDangBai == null || o.NgayDangBai <= date).OrderByDescending(o=>o.LuotXem).ThenByDescending(o => o.NgayTao);
        }

        public IQueryable<DM_NhomBaiViet> GetGroup( int type)
        {
            return db.DM_NhomBaiViet.Where(o=>o.TrangThai!=false && o.LoaiNhomBaiViet== type);
        }

        public IQueryable<DM_BaiViet> SearchNewsGrid(string text,int? groupId)
        {
            var data= db.DM_BaiViet.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                data = data.Where(o => o.TenBaiViet.ToUpper().Equals(text.ToUpper()));
            }
            if (groupId!=null)
            {
                data = data.Where(o => o.ID_NhomBaiViet== groupId);
            }
            return data.OrderByDescending(o=>o.NgayTao);
        }

        public void Insert(DM_BaiViet model , string tag)
        {
            db.DM_BaiViet.Add(model);
            db.SaveChanges();
            model.Link="/tin-tuc/"+model.ID+"/"+ StaticVariable.ConvetTitleToUrl(model.MetaTitle);
            //Xử lý tag
            if (!string.IsNullOrEmpty(tag))
            {
                string[] tags = tag.Split(',').Where(o=>o.Trim()!="").ToArray();

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
                            db.DM_Tags.Add(new DM_Tags { ID = tagId,
                                                        TenTheTag = item.Trim(),
                                                        KeyWordTag = StaticVariable.RemoveSign4VietnameseString(item.Trim()) });
                        }
                        //insert to content tag
                        db.DM_BaiViet_Tag.Add(new DM_BaiViet_Tag { ID=Guid.NewGuid(),ID_BaiViet = model.ID, ID_Tag = tagId,Loai=(int)LibEnum.StatusGroupNewsTag.tintuc });
                    }
                }
            }
            db.SaveChanges();
        }

        public JsonViewModel<string> Update(DM_BaiViet model, string tag)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)LibEnum.ErrorCode.Error };
            var data = db.DM_BaiViet.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bài viết không tồn tại hoặc đã bị xóa";
            }
            else
            {
                data.TenBaiViet = model.TenBaiViet;
                data.NoiDung = model.NoiDung;
                data.Mota = model.Mota;
                data.NgaySua =model.NgayTao;
                data.NguoiSua = model.NguoiTao;
                data.ID_NhomBaiViet = model.ID_NhomBaiViet;
                if (!string.IsNullOrWhiteSpace(model.Anh))
                {
                    data.Anh = model.Anh;
                }
                data.TrangThai = model.TrangThai;
                data.MetaTitle = model.MetaTitle;
                data.MetaDescriptions = model.MetaDescriptions;
                data.Link = "/tin-tuc/" + model.ID + "/" + StaticVariable.ConvetTitleToUrl(model.MetaTitle);
                var listTagDelete = db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == data.ID && o.Loai==(int)LibEnum.StatusGroupNewsTag.tintuc).AsEnumerable();
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
                            db.DM_BaiViet_Tag.Add(new DM_BaiViet_Tag { ID = Guid.NewGuid(), ID_BaiViet = model.ID, ID_Tag = tagId, Loai = (int)LibEnum.StatusGroupNewsTag.tintuc });
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
            var model = db.DM_BaiViet.FirstOrDefault(o => o.ID == id);
            if (model == null)
                return false;
            var listtag = db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == model.ID && o.Loai == (int)LibEnum.StatusGroupNewsTag.tintuc).AsEnumerable();
            db.DM_BaiViet_Tag.RemoveRange(listtag);
            db.DM_BaiViet.Remove(model);
            db.SaveChanges();
            return true;
        }

        public IQueryable<DM_BaiViet_Tag> GetTagsNews(int id)
        {
            return db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == id && o.Loai == (int)LibEnum.StatusGroupNewsTag.tintuc);
        }

        public bool InsertGroupNews(DM_NhomBaiViet model)
        {
            if(db.DM_NhomBaiViet.Any(o=>o.TenNhomBaiViet.ToLower().Equals(model.TenNhomBaiViet.ToLower())&& o.LoaiNhomBaiViet==model.LoaiNhomBaiViet && o.TrangThai==true))
            {
                return false;
            }
            db.DM_NhomBaiViet.Add(model);
            db.SaveChanges();
            model.Link = "/" + model.ID + "/" + model.LoaiNhomBaiViet + "/" + StaticVariable.ConvetTitleToUrl(model.TenNhomBaiViet);
            db.SaveChanges();
            return true;
        }

        public JsonViewModel<string> UpdateGroupNews(DM_NhomBaiViet model)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)LibEnum.ErrorCode.Error };
            var data = db.DM_NhomBaiViet.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Thể loại không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (db.DM_NhomBaiViet.Any(o =>o.ID!=model.ID &&
                                                o.TenNhomBaiViet.ToLower().Equals(model.TenNhomBaiViet.ToLower()) && 
                                                o.LoaiNhomBaiViet == model.LoaiNhomBaiViet
                                                && o.TrangThai == true))
                {
                    result.Data = "Tên thể loại đã bị trùng";
                }
                else
                {
                    data.ID_NhomCha = model.ID_NhomCha;
                    data.NgaySua = DateTime.Now;
                    data.NguoiSua = model.NguoiTao;
                    data.TenNhomBaiViet = model.TenNhomBaiViet;
                    data.GhiChu = model.GhiChu;
                    data.Link = "/" + data.ID + "/" + data.LoaiNhomBaiViet + "/" + StaticVariable.ConvetTitleToUrl(model.TenNhomBaiViet);
                    db.SaveChanges();
                    result.ErrorCode = (int)LibEnum.ErrorCode.Success;
                }
            }
            return result;
        }

        public JsonViewModel<string> DeleteGroupNews(int Id)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)LibEnum.ErrorCode.Error };
            var data = db.DM_NhomBaiViet.FirstOrDefault(o => o.ID == Id);
            if (data == null)
            {
                result.Data = "Thể loại không tồn tại hoặc đã bị xóa";
            }
            else
            {
                if (db.DM_BaiViet.Any(o => o.ID_NhomBaiViet == data.ID))
                {
                    result.Data = "Có bài viết đang sử dụng thể loại này không thể xóa";
                }
                else
                {
                    result.ErrorCode = (int)LibEnum.ErrorCode.Success;
                    data.TrangThai = false;
                    db.SaveChanges();
                }
            }
            return result;
        }

        public void UpdateViews(int id)
        {
            var model = getAll().FirstOrDefault(o => o.ID == id);
            if(model!=null)
            {
                model.LuotXem += 1;
                db.SaveChanges();
            }
        }
    }
}
