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
    }
}
