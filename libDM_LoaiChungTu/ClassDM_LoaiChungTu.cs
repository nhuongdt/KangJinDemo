using Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace libDM_LoaiChungTu
{
    public class ClassDM_LoaiChungTu
    {
        private SsoftvnContext db;
        public ClassDM_LoaiChungTu(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public  DM_LoaiChungTu Select_LoaiChungTu(int id)
        {
           
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LoaiChungTu.Find(id);
            }
        }

        private bool DM_LoaiChungTuExists(int id)
        {
          
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoaiChungTu.Count(e => e.ID == id) > 0;
            }
        }

        public  bool Check_TenLoaiChungTuExist(string tenLoaiChungTu)
        {
           
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.DM_LoaiChungTu.Count(e => e.TenLoaiChungTu == tenLoaiChungTu) > 0;
            }
        }

        public  IQueryable<DM_LoaiChungTu> Gets(Expression<Func<DM_LoaiChungTu, bool>> query)
        {

            if (db == null)
            {
                return null;
            }
            else
            {
                if (query != null)
                {
                    IQueryable<DM_LoaiChungTu> values = db.DM_LoaiChungTu.Where(query);
                    return values;
                }
                else
                {
                    return db.DM_LoaiChungTu;
                }
            }

        }
        public  DM_LoaiChungTu Get(Expression<Func<DM_LoaiChungTu, bool>> query)
        {
           
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LoaiChungTu.Where(query).FirstOrDefault();
            }
        }

        #endregion

        //#region insert
        //public static string Add_LoaiChungTu(DM_LoaiChungTu objAdd)
        //{
        //    string strErr = string.Empty;
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    if (db == null)
        //    {
        //        return "Kết nối CSDL không hợp lệ";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            db.DM_LoaiChungTu.Add(objAdd);
        //            db.SaveChanges();
        //        }
        //        catch (Exception ex)
        //        {
        //            strErr = ex.Message;
        //        }
        //    }
        //    return strErr;
        //}
        //#endregion

        //#region update
        //public static string Update_LoaiChungTu(DM_LoaiChungTu objNew)
        //{
        //    string strErr = string.Empty;
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    if (db == null)
        //    {
        //        strErr = "Kết nối CSDL không hợp lệ";
        //        return strErr;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            DM_LoaiChungTu objUpd = db.DM_LoaiChungTu.Find(objNew.ID);
        //            if (objUpd != null)
        //            {
        //                #region update
        //                objUpd.GhiChu = objNew.GhiChu;
        //                //objUpd.MaNhomHangHoa = objNew.MaNhomHangHoa;
        //                objUpd.TenLoaiChungTu = objNew.TenLoaiChungTu;

        //                objUpd.NgaySua = objNew.NgaySua;
        //                objUpd.NguoiSua = objNew.NguoiSua;
        //                //
        //                db.Entry(objUpd).State = EntityState.Modified;
        //                #endregion

        //                //
        //                //
        //                db.SaveChanges();
        //            }
        //            else
        //            {
        //                #region insert new
        //                return "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
        //                #endregion
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            strErr = ex.Message;
        //        }
        //        //catch (DbUpdateConcurrencyException)
        //        //{
        //        //    if (!DM_NhomHangHoaExists(id))
        //        //    {
        //        //        return NotFound();
        //        //    }
        //        //    else
        //        //    {
        //        //        throw;
        //        //    }
        //        //}
        //    }
        //    return strErr;
        //}
        //#endregion

        //#region delete
        //static string CheckDelete_LoaiChungTu(SsoftvnContext db, DM_LoaiChungTu obj)
        //{
        //    string strCheck = string.Empty;

        //    return strCheck;
        //}

        //public static string Delete_LoaiChungTu(int id)
        //{
        //    string strErr = string.Empty;
        //    SsoftvnContext db = SystemDBContext.GetDBContext();
        //    if (db == null)
        //    {
        //        return "Kết nối CSDL không hợp lệ";
        //    }
        //    else
        //    {
        //        DM_LoaiChungTu objDel = db.DM_LoaiChungTu.Find(id);
        //        if (objDel != null)
        //        {
        //            string strCheck = CheckDelete_LoaiChungTu(db, objDel);
        //            if (strCheck == string.Empty)
        //            {
        //                try
        //                {
        //                    //
        //                    db.DM_LoaiChungTu.Remove(objDel);
        //                    //
        //                    db.SaveChanges();
        //                }
        //                catch (Exception exxx)
        //                {
        //                    strErr = exxx.Message;
        //                    return strErr;
        //                }
        //            }
        //            else
        //            {
        //                strErr = strCheck;
        //                return strErr;
        //            }
        //        }
        //        else
        //        {
        //            strErr = "Không tìm thấy dữ liệu cần xử lý trên hệ thống.";
        //            return strErr;
        //        }
        //    }
        //    return strErr;
        //}

        //#endregion
    }
}
