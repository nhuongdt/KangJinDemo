using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace libHT_NguoiDung
{
    public class classHT_NguoiDung_Nhom
    {
        private SsoftvnContext db;

        public classHT_NguoiDung_Nhom(SsoftvnContext _db)
        {
            db = _db;
        }
        #region Select
        public IQueryable<HT_NguoiDung_Nhom> Gets(Expression<Func<HT_NguoiDung_Nhom, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NguoiDung_Nhom.Where(query);
            }
        }
        public HT_NguoiDung_Nhom Select_HT_NguoiDung_Nhom(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NguoiDung_Nhom.Where(idnd => idnd.IDNguoiDung == id).FirstOrDefault();
            }
        }

        public HT_NguoiDung_Nhom Get(Expression<Func<HT_NguoiDung_Nhom, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_NguoiDung_Nhom.Where(query).FirstOrDefault();
            }
        }

        private bool HT_NguoiDung_NhomExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.HT_NguoiDung_Nhom.Count(e => e.ID == id) > 0;
            }
        }
        #endregion

        #region Insert
        public string Add_NguoiDung_Nhom(HT_NguoiDung_Nhom objUserAdd)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return null;
            }
            else
            {
                try
                {
                    db.HT_NguoiDung_Nhom.Add(objUserAdd);
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

        #region update
        public string Update_NhomNguoiDung(HT_NguoiDung_Nhom objNew)
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
                    HT_NguoiDung_Nhom objUpd = db.HT_NguoiDung_Nhom.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.IDNhomNguoiDung = objNew.IDNhomNguoiDung;
                        db.Entry(objUpd).State = EntityState.Modified;
                        #endregion
                        //
                        db.SaveChanges();
                    }
                    else
                    {
                        #region insert new
                        strErr = "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                        return strErr;
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion
    }
}
