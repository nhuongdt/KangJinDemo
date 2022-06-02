using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;

namespace libDM_DoiTuong
{
    public class ClassDM_DoiTuong_Nhom
    {
        private SsoftvnContext db;
        public ClassDM_DoiTuong_Nhom(SsoftvnContext _db)
        {
            db = _db;
        }
        #region insert
        public string Add_DM_DoiTuong_Nhom(DM_DoiTuong_Nhom objAdd)
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
                    db.DM_DoiTuong_Nhom.Add(objAdd);
                    db.SaveChanges();
                }
                catch (Exception dbEx)
                {
                    Model.CookieStore.WriteLog("classDM_DoiTuong_Nhom - Add_DM_DoiTuong_Nhom" + dbEx.Message + dbEx.InnerException);
                    strErr = dbEx.Message + dbEx.InnerException;
                }
            }
            return strErr;
        }
        #endregion

        #region delete
        /// <summary>
        /// Delete all Nhom of DoiTuong in DM_DoiTuong_Nhom
        /// </summary>
        /// <param name="lstIDDoiTuong"></param>
        /// <returns></returns>
        public void Delete_DM_DoiTuong_Nhom(List<Guid> lstIDDoiTuong)
        {
            var lstCTHD = db.DM_DoiTuong_Nhom.Where(x => lstIDDoiTuong.Any(y => y == x.ID_DoiTuong));
            db.DM_DoiTuong_Nhom.RemoveRange(lstCTHD);
            db.SaveChanges();
        }

        /// <summary>
        ///  Delete 1 Nhom of DoiTuong in DM_DoiTuong_Nhom
        /// </summary>
        /// <param name="idDoiTuong"></param>
        /// <param name="idNhom"></param>
        /// <returns></returns>
        public string DeleteNhom_ofDoiTuong(Guid idDoiTuong, Guid idNhom)
        {
            string sErr = string.Empty;
            try
            {
                IQueryable<DM_DoiTuong_Nhom> lstCTHD = db.DM_DoiTuong_Nhom.Where(p => p.ID_DoiTuong == idDoiTuong && p.ID_NhomDoiTuong == idNhom);
                if (lstCTHD != null && lstCTHD.Count() > 0)
                {
                    var objDelete = db.DM_DoiTuong_Nhom.Find(lstCTHD.FirstOrDefault().ID);
                    db.DM_DoiTuong_Nhom.Remove(objDelete);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("classDM_DoiTuong_Nhom - DeleteNhom_ofDoiTuong" + ex.Message + ex.InnerException);
                sErr = ex.ToString();
            }
            return sErr;
        }

        #endregion
    }
}
