using Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libReport
{
    public class ClassReportGara
    {
        private SsoftvnContext _db;
        public ClassReportGara(SsoftvnContext db)
        {
            _db = db;
        }

        public List<BaoCaoDoanhThuSuaChuaTongHop> GetBaoCaoDoanhThuSuaChuaTongHop(string IdChiNhanhs, DateTime? ThoiGianFrom,
            DateTime ThoiGianTo, double? DoanhThuFrom, double? DoanhThuTo, double? LoiNhuanFrom, double? LoiNhuanTo, string TextSearch)
        {
            try
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("IdChiNhanhs", IdChiNhanhs));
                paramlist.Add(new SqlParameter("ThoiGianFrom", ThoiGianFrom == null ? (object)DBNull.Value : ThoiGianFrom.Value));
                paramlist.Add(new SqlParameter("ThoiGianTo", ThoiGianTo));
                paramlist.Add(new SqlParameter("DoanhThuFrom", DoanhThuFrom == null ? (object)DBNull.Value : DoanhThuFrom.Value));
                paramlist.Add(new SqlParameter("DoanhThuTo", DoanhThuTo == null ? (object)DBNull.Value : DoanhThuTo.Value));
                paramlist.Add(new SqlParameter("LoiNhuanFrom", LoiNhuanFrom == null ? (object)DBNull.Value : LoiNhuanFrom.Value));
                paramlist.Add(new SqlParameter("LoiNhuanTo", LoiNhuanTo == null ? (object)DBNull.Value : LoiNhuanTo.Value));
                paramlist.Add(new SqlParameter("TextSearch", TextSearch));
                string sqlQuery = "exec BaoCaoDoanhThuSuaChuaTongHop @IdChiNhanhs, " +
                    "@ThoiGianFrom, @ThoiGianTo, @DoanhThuFrom, @DoanhThuTo, " +
                    "@LoiNhuanFrom, @LoiNhuanTo, @TextSearch";
                return _db.Database.SqlQuery<BaoCaoDoanhThuSuaChuaTongHop>(sqlQuery, paramlist.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGara - GetBaoCaoDoanhThuSuaChuaTongHop: " + ex.Message);
                return new List<BaoCaoDoanhThuSuaChuaTongHop>();
            }
        }

        public List<BaoCaoDoanhThuSuaChuaChiTiet> GetBaoCaoDoanhThuSuaChuaChiTiet(string IdChiNhanhs, DateTime? ThoiGianFrom,
            DateTime ThoiGianTo, double? DoanhThuFrom, double? DoanhThuTo, double? LoiNhuanFrom, double? LoiNhuanTo, string TextSearch, Guid? IdNhomHangHoa)
        {
            try
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("IdChiNhanhs", IdChiNhanhs));
                paramlist.Add(new SqlParameter("ThoiGianFrom", ThoiGianFrom == null ? (object)DBNull.Value : ThoiGianFrom.Value));
                paramlist.Add(new SqlParameter("ThoiGianTo", ThoiGianTo));
                paramlist.Add(new SqlParameter("DoanhThuFrom", DoanhThuFrom == null ? (object)DBNull.Value : DoanhThuFrom.Value));
                paramlist.Add(new SqlParameter("DoanhThuTo", DoanhThuTo == null ? (object)DBNull.Value : DoanhThuTo.Value));
                paramlist.Add(new SqlParameter("LoiNhuanFrom", LoiNhuanFrom == null ? (object)DBNull.Value : LoiNhuanFrom.Value));
                paramlist.Add(new SqlParameter("LoiNhuanTo", LoiNhuanTo == null ? (object)DBNull.Value : LoiNhuanTo.Value));
                paramlist.Add(new SqlParameter("TextSearch", TextSearch));
                paramlist.Add(new SqlParameter("IdNhomHangHoa", IdNhomHangHoa == null ? (object)DBNull.Value : IdNhomHangHoa.Value));
                string sqlQuery = "exec BaoCaoDoanhThuSuaChuaChiTiet @IdChiNhanhs, " +
                    "@ThoiGianFrom, @ThoiGianTo, @DoanhThuFrom, @DoanhThuTo, " +
                    "@LoiNhuanFrom, @LoiNhuanTo, @TextSearch, @IdNhomHangHoa";
                return _db.Database.SqlQuery<BaoCaoDoanhThuSuaChuaChiTiet>(sqlQuery, paramlist.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGara - BaoCaoDoanhThuSuaChuaChiTiet: " + ex.Message);
                return new List<BaoCaoDoanhThuSuaChuaChiTiet>();
            }
        }

        public List<BaoCaoDoanhThuSuaChuaTheoXe> GetBaoCaoDoanhThuSuaChuaTheoXe(string IdChiNhanhs, DateTime? ThoiGianFrom,
            DateTime ThoiGianTo, double? SoLanTiepNhanFrom, double? SoLanTiepNhanTo, double? SoLuongHoaDonFrom, double? SoLuongHoaDonTo, double? DoanhThuFrom, double? DoanhThuTo, double? LoiNhuanFrom, double? LoiNhuanTo, string TextSearch)
        {
            try
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("IdChiNhanhs", IdChiNhanhs));
                paramlist.Add(new SqlParameter("ThoiGianFrom", ThoiGianFrom == null ? (object)DBNull.Value : ThoiGianFrom.Value));
                paramlist.Add(new SqlParameter("ThoiGianTo", ThoiGianTo));
                paramlist.Add(new SqlParameter("SoLanTiepNhanFrom", SoLanTiepNhanFrom == null ? (object)DBNull.Value : SoLanTiepNhanFrom.Value));
                paramlist.Add(new SqlParameter("SoLanTiepNhanTo", SoLanTiepNhanTo == null ? (object)DBNull.Value : SoLanTiepNhanTo.Value));
                paramlist.Add(new SqlParameter("SoLuongHoaDonFrom", SoLuongHoaDonFrom == null ? (object)DBNull.Value : SoLuongHoaDonFrom.Value));
                paramlist.Add(new SqlParameter("SoLuongHoaDonTo", SoLuongHoaDonTo == null ? (object)DBNull.Value : SoLuongHoaDonTo.Value));
                paramlist.Add(new SqlParameter("DoanhThuFrom", DoanhThuFrom == null ? (object)DBNull.Value : DoanhThuFrom.Value));
                paramlist.Add(new SqlParameter("DoanhThuTo", DoanhThuTo == null ? (object)DBNull.Value : DoanhThuTo.Value));
                paramlist.Add(new SqlParameter("LoiNhuanFrom", LoiNhuanFrom == null ? (object)DBNull.Value : LoiNhuanFrom.Value));
                paramlist.Add(new SqlParameter("LoiNhuanTo", LoiNhuanTo == null ? (object)DBNull.Value : LoiNhuanTo.Value));
                paramlist.Add(new SqlParameter("TextSearch", TextSearch));
                string sqlQuery = "exec BaoCaoDoanhThuSuaChuaTheoXe @IdChiNhanhs, @ThoiGianFrom, @ThoiGianTo, " +
                    "@SoLanTiepNhanFrom, @SoLanTiepNhanTo, " +
                    "@SoLuongHoaDonFrom, @SoLuongHoaDonTo, " +
                    "@DoanhThuFrom, @DoanhThuTo, " +
                    "@LoiNhuanFrom, @LoiNhuanTo, @TextSearch";
                return _db.Database.SqlQuery<BaoCaoDoanhThuSuaChuaTheoXe>(sqlQuery, paramlist.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGara - BaoCaoDoanhThuSuaChuaTheoXe: " + ex.Message);
                return new List<BaoCaoDoanhThuSuaChuaTheoXe>();
            }
        }

        public List<BaoCaoDoanhThuSuaChuaTheoCoVan> GetBaoCaoDoanhThuSuaChuaTheoCoVan(string IdChiNhanhs, DateTime? ThoiGianFrom,
            DateTime ThoiGianTo, double? SoLanTiepNhanFrom, double? SoLanTiepNhanTo, double? SoLuongHoaDonFrom, double? SoLuongHoaDonTo, double? DoanhThuFrom, double? DoanhThuTo, double? LoiNhuanFrom, double? LoiNhuanTo, string TextSearch)
        {
            try
            {
                List<SqlParameter> paramlist = new List<SqlParameter>();
                paramlist.Add(new SqlParameter("IdChiNhanhs", IdChiNhanhs));
                paramlist.Add(new SqlParameter("ThoiGianFrom", ThoiGianFrom == null ? (object)DBNull.Value : ThoiGianFrom.Value));
                paramlist.Add(new SqlParameter("ThoiGianTo", ThoiGianTo));
                paramlist.Add(new SqlParameter("SoLanTiepNhanFrom", SoLanTiepNhanFrom == null ? (object)DBNull.Value : SoLanTiepNhanFrom.Value));
                paramlist.Add(new SqlParameter("SoLanTiepNhanTo", SoLanTiepNhanTo == null ? (object)DBNull.Value : SoLanTiepNhanTo.Value));
                paramlist.Add(new SqlParameter("SoLuongHoaDonFrom", SoLuongHoaDonFrom == null ? (object)DBNull.Value : SoLuongHoaDonFrom.Value));
                paramlist.Add(new SqlParameter("SoLuongHoaDonTo", SoLuongHoaDonTo == null ? (object)DBNull.Value : SoLuongHoaDonTo.Value));
                paramlist.Add(new SqlParameter("DoanhThuFrom", DoanhThuFrom == null ? (object)DBNull.Value : DoanhThuFrom.Value));
                paramlist.Add(new SqlParameter("DoanhThuTo", DoanhThuTo == null ? (object)DBNull.Value : DoanhThuTo.Value));
                paramlist.Add(new SqlParameter("LoiNhuanFrom", LoiNhuanFrom == null ? (object)DBNull.Value : LoiNhuanFrom.Value));
                paramlist.Add(new SqlParameter("LoiNhuanTo", LoiNhuanTo == null ? (object)DBNull.Value : LoiNhuanTo.Value));
                paramlist.Add(new SqlParameter("TextSearch", TextSearch));
                string sqlQuery = "exec BaoCaoDoanhThuSuaChuaTheoCoVan @IdChiNhanhs, @ThoiGianFrom, @ThoiGianTo, " +
                    "@SoLanTiepNhanFrom, @SoLanTiepNhanTo, " +
                    "@SoLuongHoaDonFrom, @SoLuongHoaDonTo, " +
                    "@DoanhThuFrom, @DoanhThuTo, " +
                    "@LoiNhuanFrom, @LoiNhuanTo, @TextSearch";
                return _db.Database.SqlQuery<BaoCaoDoanhThuSuaChuaTheoCoVan>(sqlQuery, paramlist.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGara - BaoCaoDoanhThuSuaChuaTheoCoVan: " + ex.Message);
                return new List<BaoCaoDoanhThuSuaChuaTheoCoVan>();
            }
        }

        public List<BaoCaoHoatDongXe_TongHop> BaoCaoHoatDongXe_TongHop(ParamRpHoatDongXe param)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            string IdChiNhanhs = string.Empty, idNhomHangs = string.Empty, idNhomPTungs = string.Empty, idNhanViens = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                IdChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            if (param.IDNhomHangs != null && param.IDNhomHangs.Count > 0)
            {
                idNhomHangs = string.Join(",", param.IDNhomHangs);
            }
            if (param.IDNhomPhuTungs != null && param.IDNhomPhuTungs.Count > 0)
            {
                idNhomPTungs = string.Join(",", param.IDNhomPhuTungs);
            }
            if (param.IDNhanViens != null && param.IDNhanViens.Count > 0)
            {
                idNhanViens = string.Join(",", param.IDNhanViens);
            }
            paramlist.Add(new SqlParameter("IDChiNhanhs", IdChiNhanhs));
            paramlist.Add(new SqlParameter("ToDate", param.DateTo ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDNhomHangs", idNhomHangs ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDNhanViens", idNhanViens ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDNhomPhuTungs", idNhomPTungs ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("TrangThai", param.TrangThai ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("TextSearch", param.TextSearch ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
            string sqlQuery = "exec BaoCaoHoatDongXe_TongHop @IDChiNhanhs, @ToDate, " +
                    "@IDNhomHangs, @IDNhanViens, @IDNhomPhuTungs, " +
                    "@TrangThai, @TextSearch, " +
                    "@CurrentPage, @PageSize";
            return _db.Database.SqlQuery<BaoCaoHoatDongXe_TongHop>(sqlQuery, paramlist.ToArray()).ToList();
        }
        public List<BaoCaoHoatDongXe_ChiTiet> BaoCaoHoatDongXe_ChiTiet(ParamRpHoatDongXe param)
        {
            List<SqlParameter> paramlist = new List<SqlParameter>();
            string IdChiNhanhs = string.Empty, idNhomHangs = string.Empty, idNhanViens = string.Empty, idNhomPTungs = string.Empty;
            if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                IdChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }
            if (param.IDNhomHangs != null && param.IDNhomHangs.Count > 0)
            {
                idNhomHangs = string.Join(",", param.IDNhomHangs);
            }
            if (param.IDNhanViens != null && param.IDNhanViens.Count > 0)
            {
                idNhanViens = string.Join(",", param.IDNhanViens);
            }
            if (param.IDNhomPhuTungs != null && param.IDNhomPhuTungs.Count > 0)
            {
                idNhomPTungs = string.Join(",", param.IDNhomPhuTungs);
            }
            paramlist.Add(new SqlParameter("IDChiNhanhs", IdChiNhanhs));
            paramlist.Add(new SqlParameter("FromDate", param.DateFrom ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("ToDate", param.DateTo ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDNhomHangs", idNhomHangs ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDNhanViens", idNhanViens ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("IDNhomPhuTungs", idNhomPTungs ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("SoGioFrom", param.SoGioHoaDongTu ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("SoGioTo", param.SoGioHoaDongDen ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("TextSearch", param.TextSearch ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("CurrentPage", param.CurrentPage ?? (object)DBNull.Value));
            paramlist.Add(new SqlParameter("PageSize", param.PageSize ?? (object)DBNull.Value));
            string sqlQuery = "exec BaoCaoHoatDongXe_ChiTiet @IDChiNhanhs, @FromDate, @ToDate, " +
                    "@IDNhomHangs, @IDNhanViens, @IDNhomPhuTungs, " +
                    "@SoGioFrom, @SoGioTo, " +
                    "@TextSearch, " +
                    "@CurrentPage, @PageSize";
            return _db.Database.SqlQuery<BaoCaoHoatDongXe_ChiTiet>(sqlQuery, paramlist.ToArray()).ToList();
        }

        public void BaoCaoBaoDuongPhuTungTheoDoi()
        {

        }
    }
}
