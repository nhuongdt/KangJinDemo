using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace lib_ChamSocKhachHang
{
    public class class_PhanHoi
    {
        private SsoftvnContext db;
        public class_PhanHoi(SsoftvnContext _db)
        {
            db = _db;
        }
        public ChamSocKhachHang Select_PhanHoi(Guid? id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.ChamSocKhachHang.Find(id);
            }
        }

        public List<ChamSocKhachHang> Gets(Expression<Func<ChamSocKhachHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.ChamSocKhachHang.ToList();
                else
                    return db.ChamSocKhachHang.Where(query).ToList();
            }
        }

        public string GetMaPhieuChamSoc()
        {
            string format = "{0:0000}";
            string machamsoc = "CS0";
            string madv = db.ChamSocKhachHang.Where(p => p.Ma_TieuDe.Contains(machamsoc)).Where(p => p.Ma_TieuDe.Length == 7).OrderByDescending(p => p.Ma_TieuDe).Select(p => p.Ma_TieuDe).FirstOrDefault();
            if (madv == null)
            {
                machamsoc = machamsoc + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(machamsoc.Length, 4)) + 1;
                machamsoc = machamsoc + string.Format(format, tempstt);
            }
            return machamsoc;
        }

        #region insert
        public string Add_PhanHoi(ChamSocKhachHang objAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.ChamSocKhachHang.Add(objAdd);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var eve in dbEx.EntityValidationErrors)
                    {
                        sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                                        eve.Entry.Entity.GetType().Name,
                                                        eve.Entry.State));
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendLine(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                                                        ve.PropertyName,
                                                        ve.ErrorMessage));
                        }
                    }
                    throw new DbEntityValidationException(sb.ToString(), dbEx);
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public string Update_PhanHoi(ChamSocKhachHang obj)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    #region ChamSocKhachHang
                    ChamSocKhachHang objUpd = db.ChamSocKhachHang.Find(obj.ID);
                    string sMa_TieuDe = string.Empty;
                    if (obj.Ma_TieuDe == null)
                    {
                        sMa_TieuDe = GetMaPhieuChamSoc();
                    }
                    else
                    {
                        sMa_TieuDe = obj.Ma_TieuDe;
                    }
                    objUpd.ID = obj.ID;
                    objUpd.Ma_TieuDe = sMa_TieuDe;
                    objUpd.ID_KhachHang = obj.ID_KhachHang;
                    objUpd.ID_LoaiTuVan = obj.ID_LoaiTuVan;
                    objUpd.ID_NhanVien = obj.ID_NhanVien;
                    objUpd.PhanLoai = 2;
                    objUpd.NhacNho = obj.NhacNho;
                    objUpd.MucDoPhanHoi = obj.MucDoPhanHoi;
                    objUpd.NguoiTao = obj.NguoiTao;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NoiDung = obj.NoiDung;
                    objUpd.TraLoi = obj.TraLoi;
                    objUpd.NgayGio = obj.NgayGio;
                    objUpd.ThoiGianHenLai = obj.ThoiGianHenLai;
                    objUpd.NgayTao = objUpd.NgayTao;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.TrangThai = obj.TrangThai;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
                    //
                    db.SaveChanges();

                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion



        #region delete
        string CheckDelete_PhanHoi(SsoftvnContext db, ChamSocKhachHang obj)
        {
            string strCheck = string.Empty;

            return strCheck;
        }
        #endregion

        public string Delete_PhanHoi(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                ChamSocKhachHang objDel = db.ChamSocKhachHang.Find(id);
                if (objDel != null)
                {
                    string strCheck = CheckDelete_PhanHoi(db, objDel);
                    if (strCheck == string.Empty)
                    {
                        try
                        {
                            //List<BH_HoaDon> lstHoaDon = db.BH_HoaDon.Where(p => p.ID_NhanVien == id).ToList();
                            //if (lstHoaDon != null && lstHoaDon.Count > 0)
                            //{
                            //    db.BH_HoaDon.RemoveRange(lstHoaDon);
                            //}
                            db.ChamSocKhachHang.Remove(objDel);
                            //
                            db.SaveChanges();
                        }
                        catch (Exception exxx)
                        {
                            strErr = exxx.Message;
                            return strErr;
                        }
                    }
                    else
                    {
                        strErr = strCheck;
                        return strErr;
                    }
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }
    }
}
