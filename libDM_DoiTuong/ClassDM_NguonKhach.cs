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
    public class ClassDM_NguonKhach
    {
        private SsoftvnContext db;
        public ClassDM_NguonKhach(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DM_NguonKhachHang Select_ByID(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_NguonKhachHang.Find(id);
            }
        }

        public List<DM_NguonKhachHang> Gets(Expression<Func<DM_NguonKhachHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_NguonKhachHang.ToList();
                else
                    return db.DM_NguonKhachHang.Where(query).ToList();
            }
        }

        public DM_NguonKhachHang Get(Expression<Func<DM_NguonKhachHang, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_NguonKhachHang.Where(query).FirstOrDefault();
            }
        }
        #endregion

        #region insert
        public string Add(DM_NguonKhachHang objAdd)
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
                    db.DM_NguonKhachHang.Add(objAdd);
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
        public string Update(DM_NguonKhachHang obj)
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
                    DM_NguonKhachHang objUpd = db.DM_NguonKhachHang.Find(obj.ID);
                    objUpd.TenNguonKhach = obj.TenNguonKhach;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySua = DateTime.Now;
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
        public string Delete(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_NguonKhachHang objDel = db.DM_NguonKhachHang.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.DM_NguonKhachHang.Remove(objDel);
                        db.SaveChanges();
                        return "";
                    }
                    catch (DbEntityValidationException exxx)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var eve in exxx.EntityValidationErrors)
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
                        throw new DbEntityValidationException(sb.ToString(), exxx);
                    }
                }
                else
                {
                    return strErr;
                }
            }
        }
        #endregion

        #region function other
        #endregion
    }
}
