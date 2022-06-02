using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn
{
    public class M_DangKySuDung
    {
        #region Check web
        public static int CheckWeb(string subdomain)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            CuaHangDangKy chdk = db.CuaHangDangKies.Where(p => p.SubDomain == subdomain).FirstOrDefault();
            if(chdk != null)
            {
                if(chdk.TrangThai == true)
                {
                    if(chdk.HanSuDung >= DateTime.Today)
                    {
                        return 0;
                    }
                    else
                    {
                        return 2; //đã kích hoạt và hết hạn sử dụng phần mềm, chuyển qua trang hết hạn
                    }
                }
                else
                {
                    return 2; //chưa kich hoạt, chuyển qua trang nhập mã
                }
            }
            return 1; //cửa hàng không tồn tại, chuyển qua trang open24.vn
        }

        public static bool CheckCodeActive(string code, string subdomain)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            CuaHangDangKy chdk = db.CuaHangDangKies.Where(p => p.SubDomain == subdomain).Where(p => p.MaKichHoat == code).FirstOrDefault();
            if (chdk != null)
            {
                chdk.SoLanKichHoat = chdk.SoLanKichHoat.GetValueOrDefault() + 1;
                chdk.TrangThai = true;
                Random random = new Random();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                string makichhoat = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                chdk.MaKichHoat = makichhoat;
                db.Entry(chdk).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            } 
            else
                return false;
        }
        #endregion

        #region select
        public static List<CuaHangDangKy> Gets(Expression<Func<CuaHangDangKy, bool>> query)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.CuaHangDangKies.Where(query).Include(c => c.NganhNgheKinhDoanh).ToList();
        }
        public static CuaHangDangKy Get(Expression<Func<CuaHangDangKy, bool>> query)
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.CuaHangDangKies.Where(query).FirstOrDefault();
        }

        public static CuaHangDangKy SelectSubdomainDaDangKy(string strSub)
        {
            if (strSub != null && strSub.Trim() != "")
            {
                BanHang24vnContext db = new BanHang24vnContext();
                return db.CuaHangDangKies.Where(p => p.SubDomain != null && p.SubDomain.Trim().ToLower() == strSub.Trim().ToLower()).FirstOrDefault();
            }
            else
                return null;
        }

        public static CuaHangDangKy Select_DienThoai(string strSDT)
        {
            if (strSDT != null && strSDT.Trim() != "")
            {
                BanHang24vnContext db = new BanHang24vnContext();
                return db.CuaHangDangKies.Find(strSDT);
            }
            else
                return null;
        }

        public static List<CuaHangDangKy> SelectAll()
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.CuaHangDangKies.Include(c => c.NganhNgheKinhDoanh).ToList();
        }

        public static IQueryable<CuaHangDangKy> SelectsAll()
        {
            BanHang24vnContext db = new BanHang24vnContext();
            return db.CuaHangDangKies;
        }
        #endregion

        #region add
        public static string AddNewCuaHangDangKy(CuaHangDangKy objDangKy)
        {
            string strReturn = string.Empty;
            BanHang24vnContext db = new BanHang24vnContext();
            //
            string id = objDangKy.SoDienThoai;
            CuaHangDangKy objCheck_SDT = db.CuaHangDangKies.Find(id);
            if (objCheck_SDT != null)
            {
                return "Số điện thoại này đã được sử dụng để đăng ký. Hãy nhập số điện thoại khác.";
            }
            //
            string strSub = objDangKy.SubDomain;
            CuaHangDangKy objCheck_TenMien = db.CuaHangDangKies.Where(p => p.SubDomain != null && p.SubDomain.Trim().ToLower() == strSub.Trim().ToLower()).FirstOrDefault();
            if (objCheck_TenMien != null)
            {
                strReturn = "Địa chỉ WebSite đã được sử dụng. Hãy nhập địa chỉ Web khác.";
                return strReturn;
            }
            try
            {
                objDangKy.NgayTao = DateTime.Now;
                //
                db.CuaHangDangKies.Add(objDangKy);
                db.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return strReturn;
        }

        #endregion

        #region update
        public static string Update(CuaHangDangKy objNew)
        {
            string strErr = string.Empty;
            try
            {
                BanHang24vnContext db = new BanHang24vnContext();

                CuaHangDangKy objUpd = db.CuaHangDangKies.Find(objNew.SoDienThoai);
                if (objUpd != null)
                {
                    objUpd.DiaChi = objNew.DiaChi;
                    objUpd.Email = objNew.Email;
                    objUpd.HoTen = objNew.HoTen;
                    objUpd.NgayTao = objNew.NgayTao;
                    objUpd.SubDomain = objNew.SubDomain;
                    objUpd.TenCuaHang = objNew.TenCuaHang;
                    objUpd.UserKT = objNew.UserKT;
                    objUpd.MatKhauKT = objNew.MatKhauKT;
                    objUpd.ID_NganhKinhDoanh = objNew.ID_NganhKinhDoanh;
                    //
                    db.Entry(objUpd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    strErr = "Không tìm thấy dữ liệu cần cập nhật trên hệ thống";
                    return strErr;
                }
            }
            catch (Exception ex)
            {
                strErr = ex.Message;
            }
            return strErr;
        }
        #endregion

        #region delete
        public static string Delete_Subdomain(string strsubdomain, bool delDataBase)
        {
            string strErr = string.Empty;
            BanHang24vnContext db = new BanHang24vnContext();
            CuaHangDangKy objDel = db.CuaHangDangKies.Where(p => p.SubDomain.ToUpper() == strsubdomain.ToUpper()).FirstOrDefault();
            if (objDel != null)
            {
                try
                {
                    db.CuaHangDangKies.Remove(objDel);
                    db.SaveChanges();
                }
                catch (Exception exxx)
                {
                    return exxx.Message;
                }
            }
            return strErr;
        }

        public static string Delete_SDT(string strDienThoai)
        {
            string strErr = string.Empty;
            BanHang24vnContext db = new BanHang24vnContext();
            CuaHangDangKy objDel = db.CuaHangDangKies.Find(strDienThoai);
            if (objDel != null)
            {
                try
                {
                    db.CuaHangDangKies.Remove(objDel);
                    db.SaveChanges();
                }
                catch (Exception exxx)
                {
                    return exxx.Message;
                }
            }
            return strErr;
        }

        #endregion 
    }
}
