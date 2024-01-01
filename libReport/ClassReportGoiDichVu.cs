using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportGoiDichVu
    {
        private SsoftvnContext _db;
        public ClassReportGoiDichVu(SsoftvnContext db)
        {
            _db = db;
        }

        public ReportSale_ParamCommon ReportGDV_GetCommonParam(Param_ReportGoiDichVu param)
        {
            string theoDoi = "%%", trangThai = "%%", thoiHan = "%%";
            string idChiNhanhs = string.Empty;
            if (param != null && param.IDChiNhanhs != null && param.IDChiNhanhs.Count > 0)
            {
                idChiNhanhs = string.Join(",", param.IDChiNhanhs);
            }

            switch (param.TinhTrang)
            {
                case 2:
                    theoDoi = "%1%";
                    trangThai = "%0%";
                    break;
                case 3:
                    theoDoi = "%0%";
                    break;
                case 4:
                    trangThai = "%1%";
                    break;
            }

            switch (param.ThoiHanSuDung)
            {
                case 2:
                    thoiHan = "%1%";
                    break;
                case 3:
                    thoiHan = "%0%";
                    break;
            }

            return new ReportSale_ParamCommon()
            {
                TheoDoi = theoDoi,
                TrangThai = trangThai,
                ThoiHanSuDung = thoiHan,
                IDChiNhanhs = idChiNhanhs,
            };
        }

        public List<BaoCaoGoiDichVu_SoDuTongHopPRC> GetBaoCaoDichVu_SoDuTongHop(string maHD_search, string MaHH_search, string MaKH_search, string MaKH_TV, DateTime timeStart, DateTime timeEnd,
            string ID_ChiNhanh, string LaHH_search, string TheoDoi, string TrangThai, string ThoiHan, string ID_NhomHang_search, string ID_NhomHang_SP, Guid ID_NguoiDung)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", maHD_search));
                sql.Add(new SqlParameter("MaHH", MaHH_search));
                sql.Add(new SqlParameter("MaKH", MaKH_search));
                sql.Add(new SqlParameter("MaKH_TV", MaKH_TV));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("LaHangHoa", LaHH_search));
                sql.Add(new SqlParameter("TheoDoi", TheoDoi));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                sql.Add(new SqlParameter("ThoiHan", ThoiHan));
                sql.Add(new SqlParameter("ID_NhomHang", ID_NhomHang_search));
                sql.Add(new SqlParameter("ID_NhomHang_SP", ID_NhomHang_SP));
                sql.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoGoiDichVu_SoDuTongHopPRC>("exec BaoCaoDichVu_SoDuTongHop @Text_Search,@MaHH,@MaKH,@MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai,@ThoiHan, @ID_NhomHang,@ID_NhomHang_SP, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGoiDichVu - GetBaoCaoDichVu_SoDuTongHop: " + ex.Message);
                return new List<BaoCaoGoiDichVu_SoDuTongHopPRC>();
            }
        }

        public List<BaoCaoGoiDichVu_SoDuChiTietPRC> GetBaoCaoDichVu_SoDuChiTiet(string maHD_search, string MaHH_search, string MaKH_search, string MaKH_TV, DateTime timeStart, DateTime timeEnd,
            string ID_ChiNhanh, string LaHH_search, string TheoDoi, string TrangThai, string ThoiHan, string ID_NhomHang_search, string ID_NhomHang_SP, Guid ID_NguoiDung)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", maHD_search));
                sql.Add(new SqlParameter("MaHH", MaHH_search));
                sql.Add(new SqlParameter("MaKH", MaKH_search));
                sql.Add(new SqlParameter("MaKH_TV", MaKH_TV));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("LaHangHoa", LaHH_search));
                sql.Add(new SqlParameter("TheoDoi", TheoDoi));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                sql.Add(new SqlParameter("ThoiHan", ThoiHan));
                sql.Add(new SqlParameter("ID_NhomHang", ID_NhomHang_search));
                sql.Add(new SqlParameter("ID_NhomHang_SP", ID_NhomHang_SP));
                sql.Add(new SqlParameter("ID_NguoiDung", ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoGoiDichVu_SoDuChiTietPRC>("exec BaoCaoDichVu_SoDuChiTiet @Text_Search,@MaHH,@MaKH,@MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai,@ThoiHan, @ID_NhomHang,@ID_NhomHang_SP, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGoiDichVu - GetBaoCaoDichVu_SoDuChiTiet: " + ex.Message);
                return new List<BaoCaoGoiDichVu_SoDuChiTietPRC>();
            }
        }

        public List<BaoCaoGoiDichVu_NhatKySuDungTongHopPRC> GetBaoCaoDichVu_NhatKySuDungTongHop(string MaHangHoa, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh, string LaHH_search,
            string TheoDoi, string TrangThai, string ThoiHan, Guid? ID_NhomHang)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", MaHangHoa));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("LaHangHoa", LaHH_search));
                sql.Add(new SqlParameter("TheoDoi", TheoDoi));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                sql.Add(new SqlParameter("ThoiHan", ThoiHan));
                sql.Add(new SqlParameter("ID_NhomHang", ID_NhomHang == null ? (object)DBNull.Value : ID_NhomHang.Value));
                return _db.Database.SqlQuery<BaoCaoGoiDichVu_NhatKySuDungTongHopPRC>("exec BaoCaoDichVu_NhatKySuDungTongHop @Text_Search, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai,@ThoiHan, @ID_NhomHang", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGoiDichVu - GetBaoCaoDichVu_NhatKySuDungTongHop: " + ex.Message);
                return new List<BaoCaoGoiDichVu_NhatKySuDungTongHopPRC>();
            }
        }

        public List<BaoCaoGoiDichVu_NhatKySuDungChiTietPRC> GetBaoCaoDichVu_NhatKySuDungChiTiet(string MaHangHoa, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh, string LaHH_search,
            string TheoDoi, string TrangThai, string ThoiHan, Guid? ID_NhomHang)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", MaHangHoa));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("LaHangHoa", LaHH_search));
                sql.Add(new SqlParameter("TheoDoi", TheoDoi));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                sql.Add(new SqlParameter("ThoiHan", ThoiHan));
                sql.Add(new SqlParameter("ID_NhomHang", ID_NhomHang == null ? (object)DBNull.Value : ID_NhomHang.Value));
                return _db.Database.SqlQuery<BaoCaoGoiDichVu_NhatKySuDungChiTietPRC>("exec BaoCaoDichVu_NhatKySuDungChiTiet @Text_Search, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai,@ThoiHan, @ID_NhomHang", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGoiDichVu - GetBaoCaoDichVu_NhatKySuDungChiTiet: " + ex.Message);
                return new List<BaoCaoGoiDichVu_NhatKySuDungChiTietPRC>();
            }
        }

        public List<BaoCaoGoiDichVu_TonChuaSuDungPRC> GetBaoCaoDichVu_TonChuaSuDung(string maHD_search, string MaHH_search, string MaKH_search, string MaKH_TV, DateTime timeStart, DateTime timeEnd,
            string ID_ChiNhanh, string LaHH_search, string TheoDoi, string TrangThai, string ThoiHan, string ID_NhomHang_search, string ID_NhomHang_SP)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", maHD_search));
                sql.Add(new SqlParameter("MaHH", MaHH_search));
                sql.Add(new SqlParameter("MaKH", MaKH_search));
                sql.Add(new SqlParameter("MaKH_TV", MaKH_TV));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("LaHangHoa", LaHH_search));
                sql.Add(new SqlParameter("TheoDoi", TheoDoi));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                sql.Add(new SqlParameter("ThoiHan", ThoiHan));
                sql.Add(new SqlParameter("ID_NhomHang", ID_NhomHang_search));
                sql.Add(new SqlParameter("ID_NhomHang_SP", ID_NhomHang_SP));
                return _db.Database.SqlQuery<BaoCaoGoiDichVu_TonChuaSuDungPRC>("exec BaoCaoDichVu_TonChuaSuDung @Text_Search,@MaHH,@MaKH,@MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai,@ThoiHan, @ID_NhomHang,@ID_NhomHang_SP", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGoiDichVu - GetBaoCaoDichVu_TonChuaSuDung: " + ex.Message);
                return new List<BaoCaoGoiDichVu_TonChuaSuDungPRC>();
            }
        }

        public List<BaoCaoGoiDichVu_NhapXuatTonPRC> GetBaoCaoDichVu_NhapXuatTon(string maHD_search, string MaHH_search, string MaKH_search, string MaKH_TV, DateTime timeStart, DateTime timeEnd,
            string ID_ChiNhanh, string LaHH_search, string TheoDoi, string TrangThai, string ThoiHan, string ID_NhomHang_search, string ID_NhomHang_SP)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("Text_Search", maHD_search));
                sql.Add(new SqlParameter("MaHH", MaHH_search));
                sql.Add(new SqlParameter("MaKH", MaKH_search));
                sql.Add(new SqlParameter("MaKH_TV", MaKH_TV));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("LaHangHoa", LaHH_search));
                sql.Add(new SqlParameter("TheoDoi", TheoDoi));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                sql.Add(new SqlParameter("ThoiHan", ThoiHan));
                sql.Add(new SqlParameter("ID_NhomHang", ID_NhomHang_search));
                sql.Add(new SqlParameter("ID_NhomHang_SP", ID_NhomHang_SP));
                return _db.Database.SqlQuery<BaoCaoGoiDichVu_NhapXuatTonPRC>("exec BaoCaoDichVu_NhapXuatTon @Text_Search,@MaHH,@MaKH,@MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai,@ThoiHan, @ID_NhomHang,@ID_NhomHang_SP", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGoiDichVu - GetBaoCaoDichVu_NhapXuatTon: " + ex.Message);
                return new List<BaoCaoGoiDichVu_NhapXuatTonPRC>();
            }
        }

        public List<BaoCaoGoiDichVu_BanDoiTra> BaoCaoGoiDichVu_BanDoiTra(Param_BCGDVDoiTra param)
        {
            try
            {
                var obj = ReportGDV_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>
                {
                    new SqlParameter("IDChiNhanhs", obj.IDChiNhanhs),
                    new SqlParameter("FromDate", param.DateFrom),
                    new SqlParameter("ToDate", param.DateTo),
                    new SqlParameter("TxtMaHD", param.TextSearch ?? (object)DBNull.Value),
                    new SqlParameter("TxtDVMua", param.TxtDVMua ?? (object)DBNull.Value),
                    new SqlParameter("TxtDVDoi", param.TxtDVDoi ?? (object)DBNull.Value),
                    new SqlParameter("ThoiHanSuDung", obj.ThoiHanSuDung),
                    new SqlParameter("CurrentPage", param.CurrentPage ?? 0),
                    new SqlParameter("PageSize", param.PageSize ?? 10)
                };
                return _db.Database.SqlQuery<BaoCaoGoiDichVu_BanDoiTra>("exec BaoCaoGoiDichVu_BanDoiTra @IDChiNhanhs,@FromDate, @ToDate," +
                    "@TxtMaHD, @TxtDVMua, @TxtDVDoi, @ThoiHanSuDung, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportGoiDichVu - BaoCaoGoiDichVu_BanDoiTra: " + ex.Message);
                return new List<BaoCaoGoiDichVu_BanDoiTra>();
            }
        }
    }
}
