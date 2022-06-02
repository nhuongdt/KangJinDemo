using Model.Web.API;
using Ssoft.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web.Service
{
    public class RecruitmentService
    {
        private SsoftvnWebContext db;
        public RecruitmentService()
        {
            db = SystemDBContext.GetDBContext();
        }
        public IQueryable<DM_TuyenDung> GetAll()
        {
            return db.DM_TuyenDung.AsQueryable();
        }

        public IQueryable<DM_TuyenDung> GetAllActive()
        {
            return db.DM_TuyenDung.Where(o => o.TrangThai != (int)LibEnum.IsStatus.an);
        }

        public IQueryable<DM_TuyenDung> GetHome(string matinh)
        {
            var date = DateTime.Now.Date;
            return db.DM_TuyenDung.Where(o=>o.TrangThai!=(int)LibEnum.IsStatus.an && o.DenNgay>= date && o.MaTinhThanh.Contains(matinh));
        }

        public IQueryable<DM_NhomBaiViet> GetGroup()
        {
            return db.DM_NhomBaiViet.Where(o => o.LoaiNhomBaiViet == (int)LibEnum.StatusGroupNews.tuyendung && o.TrangThai == true);
        }

        public IQueryable<DM_TuyenDung> GetDetailGroup(int? groupId)
        {
            if (groupId == null)
            {
                return db.DM_TuyenDung.Where(o => o.TrangThai != (int)LibEnum.IsStatus.an);
            }
            return db.DM_TuyenDung.Where(o => o.TrangThai != (int)LibEnum.IsStatus.an && o.ID_NhomBaiViet == groupId);
        }

        public IQueryable<DM_TuyenDung>  SearchNewsGrid(string text, int? groupId)
        {
            var data = db.DM_TuyenDung.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                data = data.Where(o => o.TieuDe.ToUpper().Equals(text.ToUpper()));
            }
            if (groupId != null)
            {
                data = data.Where(o => o.ID_NhomBaiViet == groupId);
            }
            return data.OrderByDescending(o => o.NgayTao);
        }

        public void Insert(DM_TuyenDung model, string tag)
        {
            model.NgayDangBai = DateTime.Now;
            db.DM_TuyenDung.Add(model);
            db.SaveChanges();
            model.Link = "/tuyen-dung/" + model.ID + "/" + StaticVariable.ConvetTitleToUrl(model.MetaTitle);
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
                        db.DM_BaiViet_Tag.Add(new DM_BaiViet_Tag { ID = Guid.NewGuid(), ID_BaiViet = model.ID, ID_Tag = tagId, Loai = (int)LibEnum.StatusGroupNewsTag.tuyendung });
                    }
                }
            }
            db.SaveChanges();
        }

        public JsonViewModel<string> Update(DM_TuyenDung model, string tag)
        {
            var result = new JsonViewModel<string>() { ErrorCode = (int)LibEnum.ErrorCode.Error };
            var data = db.DM_TuyenDung.FirstOrDefault(o => o.ID == model.ID);
            if (data == null)
            {
                result.Data = "Bài tuyển dụng không tồn tại hoặc đã bị xóa";
            }
            else
            {
                data.TieuDe = model.TieuDe;
                data.SoLuong = model.SoLuong;
                data.MoTa = model.MoTa;
                data.TuNgay = model.TuNgay;
                data.DenNgay = model.DenNgay;
                data.NgaySua = DateTime.Now;
                data.NguoiTao = model.NguoiTao;
                data.MaTinhThanh = model.MaTinhThanh;
                data.DiaChi = model.DiaChi;
                data.MetaTitle = model.MetaTitle;
                data.MetaDescription = model.MetaDescription;
                data.ID_NhomBaiViet = model.ID_NhomBaiViet;
                data.TrangThai = model.TrangThai;
                data.SoLuong = model.SoLuong;
                data.MucLuong = model.MucLuong;
                data.Link = "/tuyen-dung/" + model.ID + "/" + StaticVariable.ConvetTitleToUrl(model.MetaTitle);
                var listTagDelete = db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == data.ID && o.Loai == (int)LibEnum.StatusGroupNewsTag.tuyendung).AsEnumerable();
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
                            db.DM_BaiViet_Tag.Add(new DM_BaiViet_Tag { ID = Guid.NewGuid(), ID_BaiViet = model.ID, ID_Tag = tagId, Loai = (int)LibEnum.StatusGroupNewsTag.tuyendung });
                        }
                    }
                }
                result.ErrorCode = (int)LibEnum.ErrorCode.Success;
                db.SaveChanges();
            }
            return result;
        }

        public IQueryable<DM_BaiViet_Tag> GetTagsRecruitment(int id)
        {
            return db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == id && o.Loai == (int)LibEnum.StatusGroupNewsTag.tuyendung);
        }

        public bool Delete(int id)
        {
            var model = db.DM_TuyenDung.FirstOrDefault(o => o.ID == id);
            if (model == null)
                return false;
            var listtag = db.DM_BaiViet_Tag.Where(o => o.ID_BaiViet == model.ID && o.Loai == (int)LibEnum.StatusGroupNewsTag.tuyendung).AsEnumerable();
            db.DM_BaiViet_Tag.RemoveRange(listtag);
            db.DM_TuyenDung.Remove(model);
            db.SaveChanges();
            return true;
        }

        public bool CheckConHan(DateTime input)
        {
            if (DateTime.Compare(input.Date, DateTime.Now.Date) < 0)
                return false;
            return true;
        }

        public string GetTinhThanh(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            var result = db.DM_TinhThanh.Where(o => input.Contains(o.MaTinhThanh)).Select(o => o.TenTinhThanh);
            return string.Join(",", result);
        }

        public string ConvertMucLuong(string input)
        {
            var result = string.Empty;
            var mucluongsplit = input.Split(';').ToArray();
            if (mucluongsplit.Length == 1 || mucluongsplit[0] == mucluongsplit[1])
            {
                if (mucluongsplit[0] != "0.5")
                {
                    result = mucluongsplit[0] + " triệu";
                }
                else
                {
                    result = mucluongsplit[0].Split('.')[1] + " trăm nghìn";
                }
            }
            else
            {
                if (mucluongsplit[0] != "0.5")
                {
                    result = mucluongsplit[0] + " - " + mucluongsplit[1] + " triệu";
                }
                else
                {
                    result = mucluongsplit[0].Split('.')[1] + " trăm nghìn - " + mucluongsplit[1] + " triệu";
                }
            }
            return result;
        }

        public void InsertFileDinhKem(string LinkFile,string nameFile,int IdHoSo,int size)
        {
            var model = new DS_FileDinhKem()
            {
                ID_HoSoUngTuyen = IdHoSo,
                LinkFile = LinkFile,
                Size= size,
                TenFile = nameFile
            };
            db.DS_FileDinhKem.Add(model);
            db.SaveChanges();
        }

        public int InsertHoSoUngTuyen(DS_HoSoUngTuyen model)
        {
            model.TrangThai =(int)LibEnum.IsStatusTuyenDung.taomoi;
            model.NgayTao = DateTime.Now;
            db.DS_HoSoUngTuyen.Add(model);
            db.SaveChanges();
            return model.ID;
        }
        public void UpdateTrangThaiHoSoUngTuyen(int id)
        {
            var model = db.DS_HoSoUngTuyen.FirstOrDefault(o => o.ID == id);
            if(model!=null)
            {
                model.TrangThai= (int)LibEnum.IsStatusTuyenDung.dadoc;
                db.SaveChanges();
            }
        }
        public void RemoveHoSoUngTuyen(int id)
        {
            var model = db.DS_HoSoUngTuyen.FirstOrDefault(o => o.ID == id);
            if (model != null)
            {
                model.TrangThai = (int)LibEnum.IsStatusTuyenDung.xoa;
                db.SaveChanges();
            }
        }
        public IQueryable<DS_HoSoUngTuyen> SearchGridHoSoUngTuyen(string text, List<int> trangthai)
        {
            if (trangthai == null)
            {
                trangthai = new List<int>();
            }
            if (string.IsNullOrWhiteSpace(text))
                return db.DS_HoSoUngTuyen.Where(o => trangthai.Contains(o.TrangThai));
            return db.DS_HoSoUngTuyen.Where(o => o.HoTen.ToUpper().Contains(text.ToUpper())
                                                || o.Email.ToUpper().Contains(text.ToUpper())
                                                || o.SoDienThoai.ToUpper().Contains(text.ToUpper())
                                                || o.DiaChi.ToUpper().Contains(text.ToUpper())).Where(o => trangthai.Contains(o.TrangThai));

        }
       
    }
}
