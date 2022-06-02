using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportNhanVien
    {
        private SsoftvnContext _db;
        public ClassReportNhanVien(SsoftvnContext db)
        {
            _db = db;
        }

        public List<BaoCaoNhanVien_TongHopPRC> GetBaoCaoNhanVien_TongHop(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_TongHopPRC>("exec BaoCaoNhanVien_TongHop @MaNV," +
                    "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                    "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                    "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_TongHop: " + ex.Message);
                return new List<BaoCaoNhanVien_TongHopPRC>();
            }
        }

        public List<BaoCaoNhanVien_HopDongPRC> GetBaoCaoNhanVien_TheoHopDong(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_HopDongPRC>("exec BaoCaoNhanVien_TheoHopDong @MaNV," +
                    "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                    "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                    "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_TheoHopDong: " + ex.Message);
                return new List<BaoCaoNhanVien_HopDongPRC>();
            }
        }

        public List<BaoCaoNhanVien_BaoHiemPRC> GetBaoCaoNhanVien_TheoBaoHiem(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_BaoHiemPRC>("exec BaoCaoNhanVien_TheoBaoHiem @MaNV," +
                    "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                    "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                    "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_TheoBaoHiem: " + ex.Message);
                return new List<BaoCaoNhanVien_BaoHiemPRC>();
            }
        }

        public List<BaoCaoNhanVien_TuoiPRC> GetBaoCaoNhanVien_TheoTuoi(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai, double Min, double Max)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                sql.Add(new SqlParameter("Min", Min));
                sql.Add(new SqlParameter("Max", Max));
                return _db.Database.SqlQuery<BaoCaoNhanVien_TuoiPRC>("exec BaoCaoNhanVien_TheoDoTuoi @MaNV," +
                    "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                    "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                    "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai, @Min, @Max", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_TheoTuoi: " + ex.Message);
                return new List<BaoCaoNhanVien_TuoiPRC>();
            }
        }

        public List<BaoCaoNhanVien_KhenThuongPRC> GetBaoCaoNhanVien_KhenThuong(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_KhenThuongPRC>("exec BaoCaoNhanVien_KhenThuong @MaNV," +
                "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_KhenThuong: " + ex.Message);
                return new List<BaoCaoNhanVien_KhenThuongPRC>();
            }
        }

        public List<BaoCaoNhanVien_LuongPhuCapPRC> GetBaoCaoNhanVien_LuongPhuCap(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_LuongPhuCapPRC>("exec BaoCaoNhanVien_LuongPhuCap @MaNV," +
                    "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                    "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                    "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_LuongPhuCap: " + ex.Message);
                return new List<BaoCaoNhanVien_LuongPhuCapPRC>();
            }
        }

        public List<BaoCaoNhanVien_MienGiamThuePRC> GetBaoCaoNhanVien_MienGiamThue(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_MienGiamThuePRC>("exec BaoCaoNhanVien_MienGiamThue @MaNV," +
                "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_MienGiamThue: " + ex.Message);
                return new List<BaoCaoNhanVien_MienGiamThuePRC>();
            }
        }

        public List<BaoCaoNhanVien_DaoTaoPRC> GetBaoCaoNhanVien_DaoTao(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_DaoTaoPRC>("exec BaoCaoNhanVien_DaoTao @MaNV," +
                "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_DaoTao: " + ex.Message);
                return new List<BaoCaoNhanVien_DaoTaoPRC>();
            }
        }

        public List<BaoCaoNhanVien_QuaTrinhCongTacPRC> GetBaoCaoNhanVien_QuaTrinhCongTac(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_QuaTrinhCongTacPRC>("exec BaoCaoNhanVien_QuaTrinhCongTac @MaNV," +
                    "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                    "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                    "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_QuaTrinhCongTac: " + ex.Message);
                return new List<BaoCaoNhanVien_QuaTrinhCongTacPRC>();
            }
        }

        public List<BaoCaoNhanVien_GiaDinhPRC> GetBaoCaoNhanVien_ThongTinGiaDinh(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_GiaDinhPRC>("exec BaoCaoNhanVien_ThongTinGiaDinh @MaNV," +
                "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_ThongTinGiaDinh: " + ex.Message);
                return new List<BaoCaoNhanVien_GiaDinhPRC>();
            }
        }

        public List<BaoCaoNhanVien_SucKhoePRC> GetBaoCaoNhanVien_ThongTinSucKhoe(string MaNV_search, string MaNV_TV, string ID_ChiNhanh, DateTime timeCreate_Start, DateTime timeCreate_End,
            string ID_PhongBan_search, string ID_PhongBan_SP, string GioiTinh, string LoaiHopDong, DateTime timeBirthday_Start, DateTime timeBirthday_End, string LoaiChinhTri,
            string LoaiBaoHiem, string LoaiDanToc_search, string LoaiDanToc_SP, string TrangThai)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaNV", MaNV_search));
                sql.Add(new SqlParameter("MaNV_TV", MaNV_TV));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("timeCreate_start", timeCreate_Start));
                sql.Add(new SqlParameter("timeCreate_end", timeCreate_End));
                sql.Add(new SqlParameter("ID_PhongBan", ID_PhongBan_search));
                sql.Add(new SqlParameter("ID_PhongBan_SP", ID_PhongBan_SP));
                sql.Add(new SqlParameter("GioiTinh", GioiTinh));
                sql.Add(new SqlParameter("LoaiHopDong", LoaiHopDong));
                sql.Add(new SqlParameter("timeBirthday_start", timeBirthday_Start));
                sql.Add(new SqlParameter("timeBirthday_end", timeBirthday_End));
                sql.Add(new SqlParameter("LoaiChinhTri", LoaiChinhTri));
                sql.Add(new SqlParameter("LoaiBaoHiem", LoaiBaoHiem));
                sql.Add(new SqlParameter("LoaiDanToc", LoaiDanToc_search));
                sql.Add(new SqlParameter("LoaiDanToc_SP", LoaiDanToc_SP));
                sql.Add(new SqlParameter("TrangThai", TrangThai));
                return _db.Database.SqlQuery<BaoCaoNhanVien_SucKhoePRC>("exec BaoCaoNhanVien_ThongTinSucKhoe @MaNV," +
                "@MaNV_TV,@ID_ChiNhanh,@timeCreate_start,@timeCreate_end,@ID_PhongBan," +
                "@ID_PhongBan_SP,@GioiTinh,@LoaiHopDong,@timeBirthday_start,@timeBirthday_end,@LoaiChinhTri," +
                "@LoaiBaoHiem,@LoaiDanToc,@LoaiDanToc_SP,@TrangThai", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportNhanVien - GetBaoCaoNhanVien_ThongTinSucKhoe: " + ex.Message);
                return new List<BaoCaoNhanVien_SucKhoePRC>();
            }
        }

    }
}
