using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportNhapHang
    {
        private SsoftvnContext _db;

        public ClassReportNhapHang(SsoftvnContext db)
        {
            _db = db;
        }

        public List<BaoCaoNhapHang_TongHopPRC> GetBaoCaoNhapHang_TongHopPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classBanHang = new ClassReportBanHang(_db);
                var obj = classBanHang.ReportSale_GetCommonParam(param);

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa ?? (object)DBNull.Value));// not use
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoNhapHang_TongHopPRC>("exec BaoCaoNhapHang_TongHop @Text_Search, @timeStart, @timeEnd, " +
                    "@ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhapHang - GetBaoCaoNhapHang_TongHopPRC: " + ex.Message);
                return new List<BaoCaoNhapHang_TongHopPRC>();
            }
        }

        public List<BaoCaoNhapHang_ChiTietPRC> GetBaoCaoNhapHang_ChiTietPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classBanHang = new ClassReportBanHang(_db);
                var obj = classBanHang.ReportSale_GetCommonParam(param);

                var idNhomNCCs = string.Empty;
                if (param.lstNhomKhach != null && param.lstNhomKhach.Count > 0)
                {
                    idNhomNCCs = string.Join(",", param.lstNhomKhach);
                }

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", param.MaHangHoa));
                sql.Add(new SqlParameter("MaNCC", param.MaKhachHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("ID_NhomNCC", idNhomNCCs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoNhapHang_ChiTietPRC>("exec BaoCaoNhapHang_ChiTiet @Text_Search, @MaNCC, " +
                    "@timeStart, @timeEnd, @ID_ChiNhanh,@ID_NhomNCC, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhapHang - GetBaoCaoNhapHang_ChiTietPRC: " + ex.Message);
                return new List<BaoCaoNhapHang_ChiTietPRC>();
            }
        }

        public List<BaoCaoNhapHang_NhomHangPRC> GetBaoCaoNhapHang_NhomHangPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classBanHang = new ClassReportBanHang(_db);
                var obj = classBanHang.ReportSale_GetCommonParam(param);

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TextSearch", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoNhapHang_NhomHangPRC>("exec BaoCaoNhapHang_NhomHang @TextSearch, @timeStart, @timeEnd, @ID_ChiNhanh, " +
                    "@LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhapHang - GetBaoCaoNhapHang_NhomHangPRC: " + ex.Message);
                return new List<BaoCaoNhapHang_NhomHangPRC>();
            }
        }

        public List<BaoCaoNhapHang_TheoNhaCungCapRC> GetBaoCaoNhapHang_TheoNhaCungCapRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classBanHang = new ClassReportBanHang(_db);
                var obj = classBanHang.ReportSale_GetCommonParam(param);
                var idNhomNCCs = string.Empty;
                if (param.lstNhomKhach != null && param.lstNhomKhach.Count > 0)
                {
                    idNhomNCCs = string.Join(",", param.lstNhomKhach);
                }

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TextSearch", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NhomNCC", idNhomNCCs));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoNhapHang_TheoNhaCungCapRC>("exec BaoCaoNhapHang_TheoNhaCungCap @TextSearch,  @timeStart, @timeEnd, @ID_ChiNhanh," +
                    " @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NhomNCC, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhapHang - GetBaoCaoNhapHang_TheoNhaCungCapRC: " + ex.Message);
                return new List<BaoCaoNhapHang_TheoNhaCungCapRC>();
            }
        }

        public List<BaoCaoNhapHang_TraHangNhapPRC> GetBaoCaoNhapHang_TraHangNhapPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classBanHang = new ClassReportBanHang(_db);
                var obj = classBanHang.ReportSale_GetCommonParam(param);
                var idNhomNCCs = string.Empty;
                if (param.lstNhomKhach != null && param.lstNhomKhach.Count > 0)
                {
                    idNhomNCCs = string.Join(",", param.lstNhomKhach);
                }
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TextSearch", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoNhapHang_TraHangNhapPRC>("exec BaoCaoNhapHang_TraHangNhap @TextSearch, @timeStart, @timeEnd, @ID_ChiNhanh, " +
                    "@LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhapHang - GetBaoCaoNhapHang_TraHangNhapPRC: " + ex.Message);
                return new List<BaoCaoNhapHang_TraHangNhapPRC>();
            }
        }
    }
}
