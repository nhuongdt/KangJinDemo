using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Model;
using libHT_NguoiDung;

namespace libDM_DoiTuong
{
    public class ClassDM_LienHe
    {
        private SsoftvnContext db;
        public ClassDM_LienHe(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public DM_LienHe Select_ByID(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.DM_LienHe.Find(id);
            }
        }

        public List<DM_LienHe> Gets(Expression<Func<DM_LienHe, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_LienHe.ToList();
                else
                    return db.DM_LienHe.Where(query).ToList();
            }
        }
        /// <summary>
        /// get list user contact, filter by NhanVien, if Admin: load full
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        public IEnumerable<SP_DM_LienHe> SP_GetAllUserContact_byWhere_FilterNhanVien(string txtSearch = null, string idManagers = null, string nguoitao = null)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (txtSearch == null || txtSearch == "null")
                {
                    txtSearch = string.Empty;
                }

                List<SqlParameter> lstParam = new List<SqlParameter>();
                lstParam.Add(new SqlParameter("txtSearch", txtSearch));

                var lst = db.Database.SqlQuery<SP_DM_LienHe>("exec SP_GetAll_UserContact_Where @txtSearch", lstParam.ToArray()).ToList();

                if (idManagers != "null" && idManagers != null)
                {
                    var lstIDManager = idManagers.Split(',');
                    if (lstIDManager.Length > 0)
                    {
                        lst = lst.Where(x => lstIDManager.Contains(x.ID_NhanVienPhuTrach.ToString()) || x.ID_NhanVienPhuTrach == null || x.NguoiTao == nguoitao).ToList();
                    }
                }
                return lst;
            }
        }

        public List<SP_DM_LienHe> GetInforContact_byID(Guid id)
        {
            SqlParameter param = new SqlParameter("ID", id);
            return db.Database.SqlQuery<SP_DM_LienHe>("exec dbo.GetInforContact_byID @ID", param).ToList();
        }

        #endregion

        #region insert
        public string Add(DM_LienHe objAdd)
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
                    db.DM_LienHe.Add(objAdd);
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
        public string Update(DM_LienHe obj)
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
                    #region DM_LienHe
                    DM_LienHe objUpd = db.DM_LienHe.Find(obj.ID);
                    objUpd.MaLienHe = obj.MaLienHe;
                    objUpd.TenLienHe = obj.TenLienHe;
                    objUpd.ID_DoiTuong = obj.ID_DoiTuong;
                    objUpd.ID_QuanHuyen = obj.ID_QuanHuyen;
                    objUpd.ID_TinhThanh = obj.ID_TinhThanh;
                    objUpd.SoDienThoai = obj.SoDienThoai;
                    objUpd.DienThoaiCoDinh = obj.DienThoaiCoDinh;
                    objUpd.XungHo = obj.XungHo;
                    objUpd.Email = obj.Email;
                    objUpd.GhiChu = obj.GhiChu;
                    objUpd.DiaChi = obj.DiaChi;
                    objUpd.NguoiSua = obj.NguoiSua;
                    objUpd.NgaySinh = obj.NgaySinh;
                    objUpd.NgaySua = DateTime.Now;
                    objUpd.NgaySinh = obj.NgaySinh;
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
        public string Delete(Guid id)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                DM_LienHe objDel = db.DM_LienHe.Find(id);
                if (objDel != null)
                {
                    try
                    {
                        objDel.TrangThai = 0; // 0: xoa, 1: chua xoa
                        db.Entry(objDel).State = EntityState.Modified;
                        db.SaveChanges();

                        // remove all Anh of LienHe
                        var lstDoiTuong = db.NS_CongViec.Where(x => x.ID_LienHe == id);
                        foreach (var item in lstDoiTuong)
                        {
                            ResetLienHe_inNS_CongViec(item.ID);
                        }
                        return "";
                    }
                    catch (Exception ex)
                    {
                        CookieStore.WriteLog("classDM_LienHe - Delete: " + ex.InnerException + ex.Message);
                        return ex.Message;
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
        public string SP_GetautoCode()
        {
            string format = "{0:0000000}";
            string autoCode = "NLH";

            try
            {
                var objReturn = db.Database.SqlQuery<SP_MaxCode>("EXEC GetMaLienHe_Max");
                if (objReturn != null && objReturn.Count() > 0)
                {
                    autoCode = autoCode + string.Format(format, objReturn.FirstOrDefault().MaxCode + 1);
                }
                else
                {
                    autoCode = autoCode + string.Format(format, 1);
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("classDM_LienHe - GetMaLienHe_Max" + ex.InnerException + ex.Message);

                // if Ma vuot qua kieu Int > 2,147,483,647 --> radom
                autoCode = autoCode + string.Format(format, 1);
            }
            return autoCode;
        }

        /// <summary>
        /// reset ID_LienHe in NS_CongViec if delete LienHe
        /// </summary>
        /// <param name="idLienHe"></param>
        /// <returns></returns>
        public string ResetLienHe_inNS_CongViec(Guid? idLienHe)
        {
            string strErr = string.Empty;
            try
            {
                NS_CongViec objDel = db.NS_CongViec.Find(idLienHe);
                objDel.ID_LienHe = null;
                db.Entry(objDel).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ResetLienHe_inNS_CongViec " + ex.Message + ex.InnerException);
            }
            return strErr;
        }

        #endregion

        #region DM_LienHe_Anh
        public string Add_Image(Model.DM_LienHe_Anh objAnh)
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
                    db.DM_LienHe_Anh.Add(objAnh);
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

        public List<DM_LienHe_Anh> Get_ImgUserContact(Expression<Func<DM_LienHe_Anh, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_LienHe_Anh.ToList();
                else
                    return db.DM_LienHe_Anh.Where(query).ToList();
            }
        }

        public List<DM_LienHe_Anh> Gets_DM_LienHeAnh(Expression<Func<DM_LienHe_Anh, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.DM_LienHe_Anh.ToList();
                else
                    return db.DM_LienHe_Anh.Where(query).ToList();
            }
        }
        #endregion
    }
}
