using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Model;

namespace libDM_DoiTuong
{
   public class ClassThe_TheKhachHang
    {
        private SsoftvnContext db;
        public ClassThe_TheKhachHang(SsoftvnContext _db)
        {
            db = _db;

        }
        #region select
        public  The_TheKhachHang Select_HoaDon(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.The_TheKhachHang.Find(id);
            }
        }

        public  IQueryable<The_TheKhachHang> Gets(Expression<Func<The_TheKhachHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.The_TheKhachHang;
                else
                    return db.The_TheKhachHang.Where(query);
            }
        }

        public  The_TheKhachHang Get(Expression<Func<The_TheKhachHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.The_TheKhachHang.Where(query).FirstOrDefault();
            }
        }

        public  bool The_TheKhachHangExists(Guid id)
        {

            if (db == null)
            {
                return false;
            }
            else
            {

                return db.The_TheKhachHang.Count(e => e.ID == id) > 0;
            }
        }

        #endregion

        #region insert
        public  string Add_HoaDon(The_TheKhachHang objAdd)
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
                    db.The_TheKhachHang.Add(objAdd);
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
        public  string Update_HoaDon(The_TheKhachHang obj)
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
                    #region The_TheKhachHang
                    The_TheKhachHang objUpd = db.The_TheKhachHang.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.MaThe = obj.MaThe;
                    #endregion
                    db.Entry(objUpd).State = EntityState.Modified;
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
         string CheckDelete_HoaDon(SsoftvnContext db, The_TheKhachHang obj)
        {
            string strCheck = string.Empty;

            List<CongDoan_DichVu> lstCongDoans = db.CongDoan_DichVu.Where(p => p.ID_CongDoan == obj.ID).ToList();
            if (lstCongDoans != null && lstCongDoans.Count > 0)
            {
                strCheck = "Hàng hóa/Dịch vụ đã được sử dụng để lập danh mục công đoạn cho hàng hóa/dịch vụ khác.";
                return strCheck;
            }

            return strCheck;
        }

        public  string Delete_HoaDon(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                The_TheKhachHang objDel = db.The_TheKhachHang.Find(id);
                if (objDel != null)
                {
                    string strCheck = CheckDelete_HoaDon(db, objDel);
                    if (strCheck == string.Empty)
                    {
                        try
                        {
                            db.The_TheKhachHang.Remove(objDel);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            CookieStore.WriteLog("classThe_TheKhachHang - Delete_HoaDon: " + ex.Message + ex.InnerException);
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
        #endregion
    }
}
