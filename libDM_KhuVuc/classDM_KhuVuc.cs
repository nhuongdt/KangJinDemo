using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace libDM_KhuVuc
{
    public class classDM_KhuVuc
    {
        #region select
        private SsoftvnContext db;
        public classDM_KhuVuc(SsoftvnContext _db)
        {
            db = _db;
        }
        public DM_KhuVuc Select_KhuVuc(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_KhuVuc.Find(id);
            }
        }

        public List<DM_KhuVuc> Gets(Expression<Func<DM_KhuVuc, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_KhuVuc.ToList();
                else
                    return db.DM_KhuVuc.Where(query).ToList();
            }
        }
        public DM_KhuVuc Get(Expression<Func<DM_KhuVuc, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_KhuVuc.Where(query).FirstOrDefault();
            }
        }

        public bool DM_KhuVucExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_KhuVuc.Count(e => e.ID == id) > 0;
            }
        }

        public bool Check_TenKhuVucExist(string tenKhuVuc)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_KhuVuc.Count(e => e.TenKhuVuc == tenKhuVuc) > 0;
            }
        }

        public bool Check_TenKhuVucEditExist(string tenKhuVuc, Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_KhuVuc.Count(e => e.TenKhuVuc == tenKhuVuc && e.ID != id) > 0;
            }
        }
        #endregion

        #region insert
        public string Add_KhuVuc(DM_KhuVuc objAdd)
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
                    db.DM_KhuVuc.Add(objAdd);
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
        public string Update_KhuVuc(DM_KhuVuc obj)
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
                    #region DM_KhuVuc
                    DM_KhuVuc objUpd = db.DM_KhuVuc.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.TenKhuVuc = obj.TenKhuVuc;
                    objUpd.GhiChu = obj.GhiChu;
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
        public string Delete_KhuVuc(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_KhuVuc objDel = db.DM_KhuVuc.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.DM_KhuVuc.Remove(objDel);
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
                    strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
                    return strErr;
                }
            }
            return strErr;
        }

        public object Select_KhuVuc(object value)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region
        public string GetautoCode()
        {
            string format = "{0:0000}";
            string mahoadon = "PB0";
            string madv = db.DM_KhuVuc.Where(p => p.MaKhuVuc.Contains(mahoadon)).Where(p => p.MaKhuVuc.Length == 7).OrderByDescending(p => p.MaKhuVuc).Select(p => p.MaKhuVuc).FirstOrDefault();
            if (madv == null)
            {
                mahoadon = mahoadon + string.Format(format, 1);
            }
            else
            {
                int tempstt = int.Parse(madv.Substring(mahoadon.Length, 4)) + 1;
                mahoadon = mahoadon + string.Format(format, tempstt);
            }
            return mahoadon;
        }
        #endregion
    }

    public class DM_KhuVucDTO
    {
        public Guid ID { get; set; }

        public string MaKhuVuc { get; set; }

        public string NguoiTao { get; set; }

        public DateTime? NgayTao { get; set; }
    }
}
