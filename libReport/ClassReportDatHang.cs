using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportDatHang
    {
        private SsoftvnContext _db;

        public ClassReportDatHang(SsoftvnContext db)
        {
            _db = db;
        }

        public List<BaoCaoDatHang_TongHopPRC> GetBaoCaoDatHang_TongHopPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classRpBanHang = new ClassReportBanHang(_db);
                var obj = classRpBanHang.ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoDatHang_TongHopPRC>("exec BaoCaoDatHang_TongHop @Text_Search, @timeStart, @timeEnd, @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportDatHang - GetBaoCaoDatHang_TongHopPRC: " + ex.Message);
                return new List<BaoCaoDatHang_TongHopPRC>();
            }
        }

        public List<BaoCaoDatHang_ChiTietPRC> GetBaoCaoDatHang_ChiTietPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classRpBanHang = new ClassReportBanHang(_db);
                var obj = classRpBanHang.ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoDatHang_ChiTietPRC>("exec BaoCaoDatHang_ChiTiet @Text_Search,@timeStart, @timeEnd, @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportDatHang - GetBaoCaoDatHang_ChiTietPRC: " + ex.Message);
                return new List<BaoCaoDatHang_ChiTietPRC>();
            }
        }

        public List<BaoCaoDatHang_NhomHangPRC> GetBaoCaoDatHang_NhomHangPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                ClassReportBanHang classRpBanHang = new ClassReportBanHang(_db);
                var obj = classRpBanHang.ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TenNhomHang", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoDatHang_NhomHangPRC>("exec BaoCaoDatHang_NhomHang @TenNhomHang, @timeStart, @timeEnd, @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportDatHang - GetBaoCaoDatHang_NhomHangPRC: " + ex.Message);
                return new List<BaoCaoDatHang_NhomHangPRC>();
            }
        }
    }
}
