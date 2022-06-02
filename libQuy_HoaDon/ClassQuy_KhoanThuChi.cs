using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace libQuy_HoaDon
{
    public class ClassQuy_KhoanThuChi
    {
        private SsoftvnContext db;

        public ClassQuy_KhoanThuChi(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public Quy_KhoanThuChi SelectQuy_KhoanThuChi(Guid? id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.Quy_KhoanThuChi.Find(id);
            }
        }

        public List<Quy_KhoanThuChi> Gets(Expression<Func<Quy_KhoanThuChi, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.Quy_KhoanThuChi.ToList();
                else
                    return db.Quy_KhoanThuChi.Where(query).ToList();
            }
        }
        #endregion

        #region insert
        public string AddQuy_KhoanThuChi(Quy_KhoanThuChi objQuy_KhoanThuChiAdd)
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
                    db.Quy_KhoanThuChi.Add(objQuy_KhoanThuChiAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    strErr = ex.Message + ex.InnerException;
                }
            }
            return strErr;
        }
        #endregion

        #region update
        public string Update_Quy_KhoanThuChi(Quy_KhoanThuChi obj)
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
                    Quy_KhoanThuChi objUpd = db.Quy_KhoanThuChi.Find(obj.ID);
                    objUpd.ID = obj.ID;

                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("Update_Quy_KhoanThuChi: " + ex.Message + ex.InnerException);
                }
            }
            return strErr;
        }
        #endregion
    }
}