using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libGara
{
    public class ClassBanGiaoXe
    {
        private SsoftvnContext _db;
        public ClassBanGiaoXe(SsoftvnContext db)
        {
            _db = db;
        }

        public bool CheckExistMaPhieu(string maphieu)
        {
            if(_db.Gara_Xe_PhieuBanGiao.Where(p=>p.MaPhieu == maphieu && p.TrangThai != 0).Count() > 0)
            {
                return true;
            }    
            else
            {
                return false;
            }    
        }

        public bool InsertPhieuBanGiao(Gara_Xe_PhieuBanGiao pbg)
        {
            try
            {
                _db.Gara_Xe_PhieuBanGiao.Add(pbg);
                _db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool UpdatePhieubanGiao(Gara_Xe_PhieuBanGiao pbg)
        {
            bool result = false;
            Gara_Xe_PhieuBanGiao pbgOld = _db.Gara_Xe_PhieuBanGiao.Where(p => p.Id == pbg.Id).FirstOrDefault();
            if(pbgOld != null)
            {
                pbgOld.MaPhieu = pbg.MaPhieu;
                pbgOld.IdXe = pbg.IdXe;
                pbgOld.NgayGiaoXe = pbg.NgayGiaoXe;
                pbgOld.SoKmBanGiao = pbg.SoKmBanGiao;
                pbgOld.IdNhanVienBanGiao = pbg.IdNhanVienBanGiao;
                pbgOld.IdNhanVien = pbg.IdNhanVien;
                pbgOld.IdKhachHang = pbg.IdKhachHang;
                pbgOld.LaNhanVien = pbg.LaNhanVien;
                pbgOld.GhiChuBanGiao = pbg.GhiChuBanGiao;
                //pbgOld.TrangThai = pbg.TrangThai;
                pbgOld.NgaySuaBanGiao = pbg.NgaySuaBanGiao;
                pbgOld.NguoiSuaBanGiao = pbg.NguoiSuaBanGiao;
                _db.SaveChanges();
                result = true;
            }    

            return result;
        }

        public bool InsertNhatKyHoatDong(Gara_Xe_NhatKyHoatDong nky)
        {
            try
            {
                _db.Gara_Xe_NhatKyHoatDong.Add(nky);
                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateNhatKyHoatDong(Gara_Xe_NhatKyHoatDong nky)
        {
            try
            {
                Gara_Xe_NhatKyHoatDong nkyOld = new Gara_Xe_NhatKyHoatDong();
                nkyOld = _db.Gara_Xe_NhatKyHoatDong.Where(p => p.Id == nky.Id).FirstOrDefault();
                if(nkyOld != null)
                {
                    nkyOld.IdNhanVienThucHien = nky.IdNhanVienThucHien;
                    nkyOld.IdKhachHang = nky.IdKhachHang;
                    nkyOld.LaNhanVien = nky.LaNhanVien;
                    nkyOld.ThoiGianHoatDong = nky.ThoiGianHoatDong;
                    nkyOld.SoGioHoatDong = nky.SoGioHoatDong;
                    nkyOld.SoKmHienTai = nky.SoKmHienTai;
                    nkyOld.GhiChu = nky.GhiChu;
                    nkyOld.NgaySua = nky.NgayTao;
                    nkyOld.NguoiSua = nky.NguoiTao;
                    _db.SaveChanges();
                }    
                else
                {
                    return false;
                }    
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<GetListPhieuBanGiao_v1_Result> GetListPhieuBanGiao_v1(GetListPhieuBanGiao_v1_Input input)
        {
            string strTrangThai = "";
            if (input.TrangThais.Count > 0)
            {
                strTrangThai = string.Join(",", input.TrangThais);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("NgayGiaoXeFrom", input.NgayGiaoXeFrom == null ? (object)DBNull.Value : input.NgayGiaoXeFrom.Value));
            sql.Add(new SqlParameter("NgayGiaoXeTo", input.NgayGiaoXeTo == null ? (object)DBNull.Value : input.NgayGiaoXeTo.Value));
            sql.Add(new SqlParameter("TrangThais", strTrangThai));
            sql.Add(new SqlParameter("TextSearch", input.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", input.CurrentPage));
            sql.Add(new SqlParameter("PageSize", input.PageSize));
            string sqlquery = "GetListPhieuBanGiao_v1 @NgayGiaoXeFrom, @NgayGiaoXeTo, @TrangThais, @TextSearch, @CurrentPage, @PageSize";
            List<GetListPhieuBanGiao_v1_Result> result = _db.Database.SqlQuery<GetListPhieuBanGiao_v1_Result>(sqlquery, sql.ToArray()).ToList();
            return result;
        }

        public List<GetListNhatKyByIdPhieuBanGiao_v1_Result> GetListNhatKyByIdPhieuBanGiao_v1(GetListNhatKyByIdPhieuBanGiao_v1_Input input)
        {
            string strTrangThai = "";
            if (input.TrangThais.Count > 0)
            {
                strTrangThai = string.Join(",", input.TrangThais);
            }
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IdPhieuBanGiao", input.IdPhieuBanGiao));
            sql.Add(new SqlParameter("TrangThais", strTrangThai));
            sql.Add(new SqlParameter("CurrentPage", input.CurrentPage));
            sql.Add(new SqlParameter("PageSize", input.PageSize));
            string query = "GetListNhatKyByIdPhieuBanGiao_v1 @IdPhieuBanGiao, @TrangThais, @CurrentPage, @PageSize";
            List<GetListNhatKyByIdPhieuBanGiao_v1_Result> result = _db.Database.SqlQuery<GetListNhatKyByIdPhieuBanGiao_v1_Result>(query, sql.ToArray()).ToList();
            return result;
        }

        public List<GetListPhuTungTheoDoiByIdXe_v1_Result> GetListPhuTungTheoDoiByIdXe_v1(Guid IdXe)
        {
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IdXe", IdXe));
            string query = "GetListPhuTungTheoDoiByIdXe_v1 @IdXe";
            List<GetListPhuTungTheoDoiByIdXe_v1_Result> result = _db.Database.SqlQuery<GetListPhuTungTheoDoiByIdXe_v1_Result>(query, sql.ToArray()).ToList();
            return result;
        }

        public void UpdateKhachHangNhatKyHoatDong(Guid IdPhieuBanGiao, Guid IdKhachHang)
        {
            List<Gara_Xe_NhatKyHoatDong> lst = new List<Gara_Xe_NhatKyHoatDong>();
            lst = _db.Gara_Xe_NhatKyHoatDong.Where(p => p.IdPhieuBanGiao == IdPhieuBanGiao).ToList();
            if(lst.Count > 0)
            {
                lst.ForEach(p => p.IdKhachHang = IdKhachHang);
                _db.SaveChanges();
            }    
        }
    }
}
