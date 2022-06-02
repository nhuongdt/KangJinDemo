using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;

namespace libDM_HangHoa
{
    public class classDinhLuongDichVu
    {
        private SsoftvnContext db;

        public classDinhLuongDichVu(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DinhLuongDichVu Select_DinhLuongDichVu(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DinhLuongDichVus.Find(id);
            }
        }
        public bool DinhLuongDichVuExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DinhLuongDichVus.Count(e => e.ID == id) > 0;
            }
        }

        public List<DinhLuongDichVu> Gets(Expression<Func<DinhLuongDichVu, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                List<DinhLuongDichVu> values = db.DinhLuongDichVus.Where(query).ToList();
                return values;
            }
        }
        public DinhLuongDichVu Get(Expression<Func<DinhLuongDichVu, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DinhLuongDichVus.Where(query).FirstOrDefault();
            }
        }

        #endregion

        #region insert
        public string Add_DinhLuongDichVu(DinhLuongDichVu objAdd)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    db.DinhLuongDichVus.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ClassDinhLuongDichVu - Add_DinhLuongDichVu: " + ex.Message + ex.InnerException);
                    return ex.Message;
                }
            }
            return string.Empty;
        }
        #endregion

        #region update
        public string Update_DinhLuongDichVu(DinhLuongDichVu objNew)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    DinhLuongDichVu objUpd = db.DinhLuongDichVus.Find(objNew.ID);
                    if (objUpd != null)
                    {
                        #region update
                        objUpd.GhiChu = objNew.GhiChu;
                        objUpd.ID_DichVu = objNew.ID_DichVu;
                        objUpd.ID_DonViQuiDoi = objNew.ID_DonViQuiDoi;
                        objUpd.SoLuong = objNew.SoLuong;
                        objUpd.STT = objNew.STT;
                        //
                        db.Entry(objUpd).State = EntityState.Modified;
                        //
                        #endregion
                        //
                        db.SaveChanges();
                    }
                    else
                    {
                        return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return string.Empty;
        }
        #endregion

        #region delete
        public string Delete_DinhLuongDichVu(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                DinhLuongDichVu objDel = db.DinhLuongDichVus.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        db.DinhLuongDichVus.Remove(objDel);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        return exxx.Message;
                    }
                }
            }
            return string.Empty;
        }

        public string Delete_DinhLuongDichVus_IDDichVu(Guid idDichVu)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                List<DinhLuongDichVu> lstDels = db.DinhLuongDichVus.Where(p => p.ID_DichVu == idDichVu).ToList();
                if (lstDels != null && lstDels.Count() > 0)
                {
                    try
                    {
                        db.DinhLuongDichVus.RemoveRange(lstDels);
                        db.SaveChanges();
                    }
                    catch (Exception exxx)
                    {
                        return exxx.Message;
                    }
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
