using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using Model;
using System.Data.SqlClient;
using Model_banhang24vn;

namespace libHT
{
    public class ClassHT_CongTy
    {
        private SsoftvnContext db;

        public ClassHT_CongTy(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public  HT_CongTy Select_HoaDon(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.HT_CongTy.Find(id);
            }
        }

        public  IQueryable<HT_CongTy> Gets(Expression<Func<HT_CongTy, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                    return db.HT_CongTy;
                else
                    return db.HT_CongTy.Where(query);
            }
        }

        public  HT_CongTy Get(Expression<Func<HT_CongTy, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                if (query == null)
                {
                    return db.HT_CongTy.FirstOrDefault();
                }
                else
                {
                    return db.HT_CongTy.Where(query).FirstOrDefault();
                }
            }
        }
        #endregion

        #region insert
        public  string Add_HoaDon(HT_CongTy objAdd)
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
                    db.HT_CongTy.Add(objAdd);
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

        public  string Insert_HT_ThongBao(HT_ThongBao objAdd)
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
                    db.HT_ThongBao.Add(objAdd);
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
        public  string Update_HoaDon(HT_CongTy obj)
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
                    #region HT_CongTy
                    HT_CongTy objUpd = db.HT_CongTy.Find(obj.ID);
                    objUpd.ID = obj.ID;
                    objUpd.Website = obj.Website;
                    objUpd.TenCongTy = obj.TenCongTy;
                    objUpd.DiaChi = obj.DiaChi;
                    objUpd.SoDienThoai = obj.SoDienThoai;
                    objUpd.DiaChiNganHang = obj.DiaChiNganHang;
                    objUpd.NgayCongChuan = obj.NgayCongChuan;
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

        public string UpdateHTCty(HT_CongTy cty)
        {
            string strErr = string.Empty;
            if (db == null)
            {
                strErr = "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    HT_CongTy objUpdate = db.HT_CongTy.Find(cty.ID);
                    objUpdate.TenCongTy = cty.TenCongTy;
                    objUpdate.DiaChi = cty.DiaChi;
                    objUpdate.SoDienThoai = cty.SoDienThoai;
                    objUpdate.SoFax = cty.SoFax;
                    objUpdate.MaSoThue = cty.MaSoThue;
                    objUpdate.Mail = cty.Mail;
                    objUpdate.Website = cty.Website;
                    objUpdate.TenGiamDoc = cty.TenGiamDoc;
                    objUpdate.TenKeToanTruong = cty.TenKeToanTruong;
                    objUpdate.Logo = cty.Logo;
                    objUpdate.GhiChu = cty.GhiChu;
                    objUpdate.TaiKhoanNganHang = cty.TaiKhoanNganHang;
                    objUpdate.ID_NganHang = cty.ID_NganHang;
                    objUpdate.DiaChiNganHang = cty.DiaChiNganHang;
                    objUpdate.TenVT = cty.TenVT;
                    objUpdate.DiaChiVT = cty.DiaChiVT;
                    objUpdate.DangHoatDong = cty.DangHoatDong;
                    objUpdate.DangKyNhanSu = cty.DangKyNhanSu;
                    objUpdate.NgayCongChuan = cty.NgayCongChuan;
                    objUpdate.ZaloAccessToken = cty.ZaloAccessToken;
                    objUpdate.ZaloRefreshToken = cty.ZaloRefreshToken;
                    objUpdate.EmailAccount = cty.EmailAccount;
                    objUpdate.EmailPassword = cty.EmailPassword;
                    objUpdate.ZaloCodeVerifier = cty.ZaloCodeVerifier;
                    db.Entry(objUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch(Exception ex)
                {
                    strErr = ex.Message;
                }
            }
            return strErr;
        }
        #endregion

        #region delete
        public  string Delete_HT_ThongBao(string where)
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
                    SqlParameter param = new SqlParameter("@where", where);
                    db.Database.ExecuteSqlCommand("EXEC SP_DeleteCustomer_In_HTThongBao @where ", param);
                    return strErr;
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ClassHT_CongTy - Delete_HT_ThongBao: " + ex.InnerException + ex.Message);
                    return "Error";
                }
            }
        }
        #endregion
    }
}
