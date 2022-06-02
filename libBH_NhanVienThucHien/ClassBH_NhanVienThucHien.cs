using System;
using System.Linq;
using Model;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace libBH_NhanVienThucHien
{
    public class ClassBH_NhanVienThucHien
    {
        private SsoftvnContext db;
        public ClassBH_NhanVienThucHien(SsoftvnContext _db)
        {
            db = _db;
        }
        #region select
        public List<BH_NhanVienThucHien> Gets(Expression<Func<BH_NhanVienThucHien, bool>> query)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.BH_NhanVienThucHien.Where(query).ToList();
            }
        }

        public BH_NhanVienThucHien Select_BH_NhanVienThucHien(Guid id)
        {
            if (db == null)
            {
                return null;
            }
            else
            {
                return db.BH_NhanVienThucHien.Find(id);
            }
        }

        private bool BH_NhanVienThucHienExists(Guid id)
        {
            if (db == null)
            {
                return false;
            }
            else
            {
                return db.BH_NhanVienThucHien.Count(e => e.ID == id) > 0;
            }
        }
        #endregion

        #region insert
        public string Insert(BH_NhanVienThucHien objAdd)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                db.BH_NhanVienThucHien.Add(objAdd);
                db.SaveChanges();
            }
            return string.Empty;
        }

        public string Inserts(List<BH_NhanVienThucHien> lst)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                db.BH_NhanVienThucHien.AddRange(lst);
                db.SaveChanges();
            }
            return string.Empty;
        }
        /// <summary>
        /// insert 1 row into table BH_NhanVienThucHien
        /// </summary>
        /// <param name="objAdd"></param>
        /// <returns></returns>
        public string SP_Insert_NhanVienThucHien(BH_NhanVienThucHien objAdd)
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
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_ChiTietHoaDon", objAdd.ID_ChiTietHoaDon));
                    lstParam.Add(new SqlParameter("ID_NhanVien", objAdd.ID_NhanVien));
                    lstParam.Add(new SqlParameter("ThucHien_TuVan", objAdd.ThucHien_TuVan));
                    lstParam.Add(new SqlParameter("TienChietKhau", objAdd.TienChietKhau));
                    lstParam.Add(new SqlParameter("PT_ChietKhau", objAdd.PT_ChietKhau));
                    lstParam.Add(new SqlParameter("TheoYeuCau", objAdd.TheoYeuCau));
                    db.Database.ExecuteSqlCommand("EXEC SP_Insert_NhanVienThucHien @ID_ChiTietHoaDon, @ID_NhanVien, @ThucHien_TuVan, @TienChietKhau,@PT_ChietKhau,@TheoYeuCau", lstParam.ToArray());
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_Insert_NhanVienThucHien: " + ex.InnerException + ex.Message);
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        /// <summary>
        /// get data from BH_NhanVienThucHien (HD Mua) & insert BH_NhanVienThucHien (HDTra)
        /// </summary>
        /// <param name="idHoaDonMua"></param>
        /// <param name="TongTienTra"></param>
        /// <param name="idHoaDonTra"></param>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public string SP_InsertChietKhauTraHang(Guid? idHoaDonMua, double tongTienTra, Guid idHoaDonTra, Guid idDonVi)
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
                    List<SqlParameter> lstParam = new List<SqlParameter>();
                    lstParam.Add(new SqlParameter("ID_HoaDon", idHoaDonMua));
                    lstParam.Add(new SqlParameter("TongTienTra", tongTienTra));
                    lstParam.Add(new SqlParameter("ID_HoaDonTra", idHoaDonTra));
                    lstParam.Add(new SqlParameter("ID_DonVi", idDonVi));

                    db.Database.ExecuteSqlCommand("EXEC SP_InsertChietKhauHoaDonTraHang_NhanVien @ID_HoaDon, @TongTienTra, @ID_HoaDonTra, @ID_DonVi", lstParam.ToArray());
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("SP_InsertChietKhauTraHang: " + ex.InnerException + ex.Message);
                    strErr = ex.Message;
                }
            }
            return strErr;
        }

        /// <summary>
        /// trả hàng: trừ chiết khấu nhân viên theo hóa đơn mua
        /// </summary>
        /// <param name="idHoaDonMua"></param>
        /// <param name="tongTienTra"></param>
        /// <param name="idHoaDonTra"></param>
        /// <param name="idDonVi"></param>
        /// <returns></returns>
        public string ChiTietTraHang_insertChietKhauNV(Guid? idHoaDonMua)
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
                    SqlParameter param = new SqlParameter("ID_HoaDon", idHoaDonMua);
                    db.Database.ExecuteSqlCommand("EXEC ChiTietTraHang_insertChietKhauNV @ID_HoaDon", param);
                }
                catch (Exception ex)
                {
                    CookieStore.WriteLog("ChiTietTraHang_insertChietKhauNV: " + ex.InnerException + ex.Message);
                    strErr = ex.Message;
                }
            }
            return strErr;
        }


        #endregion

        #region update
        public string Update(BH_NhanVienThucHien obj)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                try
                {
                    BH_NhanVienThucHien objUpd = db.BH_NhanVienThucHien.Find(obj.ID);
                    if (objUpd != null)
                    {
                        objUpd.ID_NhanVien = obj.ID_NhanVien;
                        objUpd.ID_ChiTietHoaDon = obj.ID_ChiTietHoaDon;
                        objUpd.TheoYeuCau = obj.TheoYeuCau;
                        objUpd.ThucHien_TuVan = obj.ThucHien_TuVan;
                        objUpd.TienChietKhau = obj.TienChietKhau;
                        db.Entry(objUpd).State = EntityState.Modified;
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

        public string Delete_BH_NhanVienThucHien(Guid id)
        {
            if (db == null)
            {
                return "Kết nối CSDL không hợp lệ";
            }
            else
            {
                BH_NhanVienThucHien objDel = db.BH_NhanVienThucHien.Find(id);
                if (objDel != null)
                {
                    db.BH_NhanVienThucHien.Remove(objDel);
                }
            }
            return string.Empty;
        }
        #endregion
    }
}
