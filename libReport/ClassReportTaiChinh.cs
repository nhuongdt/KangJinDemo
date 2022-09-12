using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportTaiChinh
    {
        private SsoftvnContext _db;

        public ClassReportTaiChinh(SsoftvnContext db)
        {
            _db = db;
        }

        public List<BaoCaoTaiChinh_CongNoPRC> GetBaoCaoTaiChinh_CongNo(string maKH_search, string maKH_TV, DateTime timeStart, DateTime timeEnd, Guid ID_DonVi, string LaDT_search, string ID_NhomDoiTuong_search,
            string ID_NhomDoiTuong_SP)
        {
            try
            {
                var tbl_timeCSt = from cs in _db.ChotSo
                                  where cs.ID_DonVi == ID_DonVi
                                  select new
                                  {
                                      cs.NgayChotSo
                                  };
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaKH", maKH_search));
                sql.Add(new SqlParameter("MaKH_TV", maKH_TV));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_DonVi));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong_SP", ID_NhomDoiTuong_SP));
                try
                {
                    if (tbl_timeCSt != null && tbl_timeCSt.Count() > 0)
                    {
                        DateTime timeCS = tbl_timeCSt.FirstOrDefault().NgayChotSo;
                        if (DateTime.Compare(timeCS, timeStart) < 0) // report 1
                        {
                            return _db.Database.SqlQuery<BaoCaoTaiChinh_CongNoPRC>("exec BaoCaoTaiChinh_CongNoI @MaKH, @MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP", sql.ToArray()).ToList();
                        }
                        else if (DateTime.Compare(timeEnd, timeCS) < 0) // report 3
                        {
                            return _db.Database.SqlQuery<BaoCaoTaiChinh_CongNoPRC>("exec BaoCaoTaiChinh_CongNoIII @MaKH, @MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP", sql.ToArray()).ToList();
                        }
                        else
                        {
                            return _db.Database.SqlQuery<BaoCaoTaiChinh_CongNoPRC>("exec BaoCaoTaiChinh_CongNoII @MaKH, @MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP", sql.ToArray()).ToList();
                        }
                    }
                    else
                    {
                        return _db.Database.SqlQuery<BaoCaoTaiChinh_CongNoPRC>("exec BaoCaoTaiChinh_CongNoIV @MaKH, @MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP", sql.ToArray()).ToList();
                    }
                }
                catch //report 4
                {
                    return _db.Database.SqlQuery<BaoCaoTaiChinh_CongNoPRC>("exec BaoCaoTaiChinh_CongNoIV @MaKH, @MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP", sql.ToArray()).ToList();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_CongNo: " + ex.Message);
                return new List<BaoCaoTaiChinh_CongNoPRC>();
            }
        }

        public List<BaoCaoTaiChinh_CongNoPRC> GetBaoCaoTaiChinh_CongNo_v2(string textSearch, DateTime timeStart, DateTime timeEnd, Guid IdChiNhanh, string LoaiKH, string IdNhomDoiTuong)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TextSearch", textSearch));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", IdChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LoaiKH));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", IdNhomDoiTuong));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_CongNoPRC>("exec BaoCaoTaiChinh_CongNo_v2 @TextSearch, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_CongNo_v2: " + ex.Message);
                return new List<BaoCaoTaiChinh_CongNoPRC>();
            }
        }
        public List<BaoCaoTaiChinh_ThuChiPRC> GetBaoCaoTaiChinh_ThuChi(string maKH_search, string maKH_TV, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh, string LaDT_search,
            string ID_NhomDoiTuong_search, string ID_NhomDoiTuong_SP, string loaiThuChi_search, string HachToanKD_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaKH", maKH_search));
                sql.Add(new SqlParameter("MaKH_TV", maKH_TV));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong_SP", ID_NhomDoiTuong_SP));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_ThuChiPRC>("exec BaoCaoTaiChinh_ThuChi @MaKH, @MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP, @lstThuChi, @HachToanKD", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_ThuChi: " + ex.Message);
                return new List<BaoCaoTaiChinh_ThuChiPRC>();
            }
        }

        public List<BaoCaoTaiChinh_ThuChiPRC> GetBaoCaoTaiChinh_ThuChi_v2(string TextSearch, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh, string loaiKH,
            string ID_NhomDoiTuong, string lstThuChi, bool? HachToanKD)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TextSearch", TextSearch));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", loaiKH));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                sql.Add(new SqlParameter("lstThuChi", lstThuChi));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD == null ? (object)DBNull.Value : HachToanKD.Value));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_ThuChiPRC>("exec BaoCaoTaiChinh_ThuChi_v2 @TextSearch, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong, @lstThuChi, @HachToanKD", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_ThuChi_v2: " + ex.Message);
                return new List<BaoCaoTaiChinh_ThuChiPRC>();
            }
        }

        public List<BaoCaoTaiChinh_SoQuyPRC> GetBaoCaoTaiChinh_SoQuy(string maKH_search, string maKH_TV, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh, string LaDT_search,
            string ID_NhomDoiTuong_search, string ID_NhomDoiTuong_SP, string loaiThuChi_search, string HachToanKD_Search, string LoaiTien_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaKH", maKH_search));
                sql.Add(new SqlParameter("MaKH_TV", maKH_TV));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong_SP", ID_NhomDoiTuong_SP));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_SoQuyPRC>("exec BaoCaoTaiChinh_SoQuy @MaKH, @MaKH_TV, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_SoQuy: " + ex.Message);
                return new List<BaoCaoTaiChinh_SoQuyPRC>();
            }
        }

        public List<BaoCaoTaiChinh_SoQuyPRC> GetBaoCaoTaiChinh_SoQuy_v2(string TextSearch, DateTime timeStart, DateTime timeEnd, string ID_ChiNhanh, string LoaiDoiTuong,
            string ID_NhomDoiTuong, string loaiThuChi, bool? HachToanKD, string LoaiTien)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TextSearch", TextSearch));
                sql.Add(new SqlParameter("timeStart", timeStart));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LoaiDoiTuong));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD == null ? (object)DBNull.Value : HachToanKD.Value));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_SoQuyPRC>("exec BaoCaoTaiChinh_SoQuy_v2 @TextSearch, @timeStart, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_SoQuy_v2: " + ex.Message);
                return new List<BaoCaoTaiChinh_SoQuyPRC>();
            }
        }

        public List<BaoCaoTaiChinh_SoQuyTheoChiNhanhPRC> GetBaoCaoTaiChinh_SoQuyTheoChiNhanh(string maKH_search, string maKH_TV, DateTime timeEnd, string ID_ChiNhanh, string LaDT_search,
            string ID_NhomDoiTuong_search, string ID_NhomDoiTuong_SP, string loaiThuChi_search, string HachToanKD_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaKH", maKH_search));
                sql.Add(new SqlParameter("MaKH_TV", maKH_TV));
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong_SP", ID_NhomDoiTuong_SP));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_SoQuyTheoChiNhanhPRC>("exec BaoCaoTaiChinh_ChiNhanh @MaKH, @MaKH_TV, @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP, @lstThuChi, @HachToanKD", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_SoQuyTheoChiNhanh: " + ex.Message);
                return new List<BaoCaoTaiChinh_SoQuyTheoChiNhanhPRC>();
            }
        }

        public List<BaoCaoTaiChinh_SoQuyTheoChiNhanhPRC> GetBaoCaoTaiChinh_SoQuyTheoChiNhanh_v2(DateTime timeEnd, string ID_ChiNhanh, string LoaiDoiTuong,
            string ID_NhomDoiTuong, string loaiThuChi, bool? HachToanKD)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("timeEnd", timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LoaiDoiTuong));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD == null ? (object)DBNull.Value : HachToanKD.Value));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_SoQuyTheoChiNhanhPRC>("exec BaoCaoTaiChinh_ChiNhanh_v2 @timeEnd, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong, @lstThuChi, @HachToanKD", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - BaoCaoTaiChinh_ChiNhanh_v2: " + ex.Message);
                return new List<BaoCaoTaiChinh_SoQuyTheoChiNhanhPRC>();
            }
        }

        public List<Report_TaiChinh_TheoThang> getListTaiChinh_TheoThangQuyNam(int Thang1Quy2Nam3, int year, string ID_ChiNhanh)
        {
            try
            {
                List<Report_TaiChinh_TheoThang> lst = new List<Report_TaiChinh_TheoThang>();
                Report_TaiChinh_TheoThang tc_DoanhThuBanHang = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamTruDoanhThu = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamGiaHoaDon = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_DoanhThuThuan = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoThang();

                tc_DoanhThuBanHang.TaiChinh = "Doanh thu bán hàng (1)";
                tc_GiamTruDoanhThu.TaiChinh = "Giảm trừ doanh thu (2 = 2.1 + 2.2)";
                tc_GiamGiaHoaDon.TaiChinh = "     Giảm giá hóa đơn (2.1)";
                tc_GiaTriHangBanBiTraLai.TaiChinh = "     Giá trị hàng bán bị trả lại (2.2)";
                tc_XuatHangSuDungGoiDichVu.TaiChinh = "Xuất hàng sử dụng gói dịch vụ (5)";
                tc_DoanhThuThuan.TaiChinh = "Doanh thu thuần (3 = 1-2)";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("year", year));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                List<ReportTaiChinhMonth_DoanhThuBanHangPRC> tbl_DoanhThuBanHang = new List<ReportTaiChinhMonth_DoanhThuBanHangPRC>();
                switch (Thang1Quy2Nam3)
                {
                    case 1:
                        tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhMonth_DoanhThuBanHangPRC>("exec ReportTaiChinhMonth_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray()).ToList();
                        break;
                    case 2:
                        tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhMonth_DoanhThuBanHangPRC>("exec ReportTaiChinhMonth_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray()).ToList();
                        break;
                    case 3:
                        tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhMonth_DoanhThuBanHangPRC>("exec ReportTaiChinhMonth_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray()).ToList();
                        break;
                    default:
                        break;
                }
                ResertDM(tc_DoanhThuBanHang);
                ResertDM(tc_GiamTruDoanhThu);
                ResertDM(tc_GiamGiaHoaDon);
                ResertDM(tc_GiaTriHangBanBiTraLai);
                ResertDM(tc_XuatHangSuDungGoiDichVu);
                ResertDM(tc_DoanhThuThuan);
                foreach (var item in tbl_DoanhThuBanHang)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_DoanhThuBanHang.Thang1 = item.DoanhThu;
                        tc_GiamTruDoanhThu.Thang1 = item.GiaTriTra + item.GiamGiaHD;
                        tc_GiamGiaHoaDon.Thang1 = item.GiamGiaHD;
                        tc_GiaTriHangBanBiTraLai.Thang1 = item.GiaTriTra;
                        tc_XuatHangSuDungGoiDichVu.Thang1 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang1 = tc_DoanhThuBanHang.Thang1 - tc_GiamTruDoanhThu.Thang1;
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_DoanhThuBanHang.Thang2 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang2 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang2 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang2 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang2 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang2 = (tc_DoanhThuBanHang.Thang2 - tc_GiamTruDoanhThu.Thang2);

                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_DoanhThuBanHang.Thang3 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang3 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang3 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang3 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang3 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang3 = (tc_DoanhThuBanHang.Thang3 - tc_GiamTruDoanhThu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_DoanhThuBanHang.Thang4 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang4 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang4 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang4 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang4 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang4 = (tc_DoanhThuBanHang.Thang4 - tc_GiamTruDoanhThu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_DoanhThuBanHang.Thang5 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang5 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang5 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang5 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang5 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang5 = (tc_DoanhThuBanHang.Thang5 - tc_GiamTruDoanhThu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_DoanhThuBanHang.Thang6 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang6 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang6 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang6 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang6 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang6 = (tc_DoanhThuBanHang.Thang6 - tc_GiamTruDoanhThu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_DoanhThuBanHang.Thang7 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang7 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang7 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang7 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang7 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang7 = (tc_DoanhThuBanHang.Thang7 - tc_GiamTruDoanhThu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_DoanhThuBanHang.Thang8 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang8 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang8 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang8 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang8 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang8 = (tc_DoanhThuBanHang.Thang8 - tc_GiamTruDoanhThu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_DoanhThuBanHang.Thang9 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang9 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang9 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang9 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang9 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang9 = (tc_DoanhThuBanHang.Thang9 - tc_GiamTruDoanhThu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_DoanhThuBanHang.Thang10 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang10 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang10 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang10 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang10 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang10 = (tc_DoanhThuBanHang.Thang10 - tc_GiamTruDoanhThu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_DoanhThuBanHang.Thang11 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang11 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang11 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang11 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang11 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang11 = (tc_DoanhThuBanHang.Thang11 - tc_GiamTruDoanhThu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_DoanhThuBanHang.Thang12 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang12 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang12 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang12 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang12 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang12 = (tc_DoanhThuBanHang.Thang12 - tc_GiamTruDoanhThu.Thang12);
                    }

                }
                tc_DoanhThuBanHang.Tong = (tc_DoanhThuBanHang.Thang1 + tc_DoanhThuBanHang.Thang2 + tc_DoanhThuBanHang.Thang3 + tc_DoanhThuBanHang.Thang4 + tc_DoanhThuBanHang.Thang5 + tc_DoanhThuBanHang.Thang6 + tc_DoanhThuBanHang.Thang7 + tc_DoanhThuBanHang.Thang8 + tc_DoanhThuBanHang.Thang9 + tc_DoanhThuBanHang.Thang10 + tc_DoanhThuBanHang.Thang11 + tc_DoanhThuBanHang.Thang12);
                tc_GiamTruDoanhThu.Tong = (tc_GiamTruDoanhThu.Thang1 + tc_GiamTruDoanhThu.Thang2 + tc_GiamTruDoanhThu.Thang3 + tc_GiamTruDoanhThu.Thang4 + tc_GiamTruDoanhThu.Thang5 + tc_GiamTruDoanhThu.Thang6 + tc_GiamTruDoanhThu.Thang7 + tc_GiamTruDoanhThu.Thang8 + tc_GiamTruDoanhThu.Thang9 + tc_GiamTruDoanhThu.Thang10 + tc_GiamTruDoanhThu.Thang11 + tc_GiamTruDoanhThu.Thang12);
                tc_GiamGiaHoaDon.Tong = (tc_GiamGiaHoaDon.Thang1 + tc_GiamGiaHoaDon.Thang2 + tc_GiamGiaHoaDon.Thang3 + tc_GiamGiaHoaDon.Thang4 + tc_GiamGiaHoaDon.Thang5 + tc_GiamGiaHoaDon.Thang6 + tc_GiamGiaHoaDon.Thang7 + tc_GiamGiaHoaDon.Thang8 + tc_GiamGiaHoaDon.Thang9 + tc_GiamGiaHoaDon.Thang10 + tc_GiamGiaHoaDon.Thang11 + tc_GiamGiaHoaDon.Thang12);
                tc_GiaTriHangBanBiTraLai.Tong = (tc_GiaTriHangBanBiTraLai.Thang1 + tc_GiaTriHangBanBiTraLai.Thang2 + tc_GiaTriHangBanBiTraLai.Thang3 + tc_GiaTriHangBanBiTraLai.Thang4 + tc_GiaTriHangBanBiTraLai.Thang5 + tc_GiaTriHangBanBiTraLai.Thang6 + tc_GiaTriHangBanBiTraLai.Thang7 + tc_GiaTriHangBanBiTraLai.Thang8 + tc_GiaTriHangBanBiTraLai.Thang9 + tc_GiaTriHangBanBiTraLai.Thang10 + tc_GiaTriHangBanBiTraLai.Thang11 + tc_GiaTriHangBanBiTraLai.Thang12);
                tc_XuatHangSuDungGoiDichVu.Tong = (tc_XuatHangSuDungGoiDichVu.Thang1 + tc_XuatHangSuDungGoiDichVu.Thang2 + tc_XuatHangSuDungGoiDichVu.Thang3 + tc_XuatHangSuDungGoiDichVu.Thang4 + tc_XuatHangSuDungGoiDichVu.Thang5 + tc_XuatHangSuDungGoiDichVu.Thang6 + tc_XuatHangSuDungGoiDichVu.Thang7 + tc_XuatHangSuDungGoiDichVu.Thang8 + tc_XuatHangSuDungGoiDichVu.Thang9 + tc_XuatHangSuDungGoiDichVu.Thang10 + tc_XuatHangSuDungGoiDichVu.Thang11 + tc_XuatHangSuDungGoiDichVu.Thang12);
                tc_DoanhThuThuan.Tong = (tc_DoanhThuThuan.Thang1 + tc_DoanhThuThuan.Thang2 + tc_DoanhThuThuan.Thang3 + tc_DoanhThuThuan.Thang4 + tc_DoanhThuThuan.Thang5 + tc_DoanhThuThuan.Thang6 + tc_DoanhThuThuan.Thang7 + tc_DoanhThuThuan.Thang8 + tc_DoanhThuThuan.Thang9 + tc_DoanhThuThuan.Thang10 + tc_DoanhThuThuan.Thang11 + tc_DoanhThuThuan.Thang12);
                lst.Add(tc_DoanhThuBanHang);
                lst.Add(tc_GiamTruDoanhThu);
                lst.Add(tc_GiamGiaHoaDon);
                lst.Add(tc_GiaTriHangBanBiTraLai);
                lst.Add(tc_DoanhThuThuan);
                //DM    
                List<SqlParameter> prmgv = new List<SqlParameter>();
                prmgv.Add(new SqlParameter("year", year));
                prmgv.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                List<ReportTaiChinhMonth_GiaVonBanHangPRC> tbl_GiaVon = new List<ReportTaiChinhMonth_GiaVonBanHangPRC>();
                switch (Thang1Quy2Nam3)
                {
                    case 1:
                        tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhMonth_GiaVonBanHangPRC>("exec ReportTaiChinhMonth_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray()).ToList();
                        break;
                    case 2:
                        tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhMonth_GiaVonBanHangPRC>("exec ReportTaiChinhMonth_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray()).ToList();
                        break;
                    case 3:
                        tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhMonth_GiaVonBanHangPRC>("exec ReportTaiChinhMonth_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray()).ToList();
                        break;
                    default:
                        break;
                }
                Report_TaiChinh_TheoThang tc_GiaVonHangBan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_GiaVonHangBan);
                tc_GiaVonHangBan.TaiChinh = "Giá vốn hàng bán (4)";
                Report_TaiChinh_TheoThang tc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanGopVeBanHang.TaiChinh = "Lợi nhuận gộp về bán hàng (6 = 3-4-5)";
                ResertDM(tc_LoiNhuanGopVeBanHang);

                foreach (var item in tbl_GiaVon)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_GiaVonHangBan.Thang1 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang1 = (tc_DoanhThuThuan.Thang1 - tc_GiaVonHangBan.Thang1 - tc_XuatHangSuDungGoiDichVu.Thang1);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_GiaVonHangBan.Thang2 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang2 = (tc_DoanhThuThuan.Thang2 - tc_GiaVonHangBan.Thang2 - tc_XuatHangSuDungGoiDichVu.Thang2);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_GiaVonHangBan.Thang3 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang3 = (tc_DoanhThuThuan.Thang3 - tc_GiaVonHangBan.Thang3 - tc_XuatHangSuDungGoiDichVu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_GiaVonHangBan.Thang4 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang4 = (tc_DoanhThuThuan.Thang4 - tc_GiaVonHangBan.Thang4 - tc_XuatHangSuDungGoiDichVu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_GiaVonHangBan.Thang5 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang5 = (tc_DoanhThuThuan.Thang5 - tc_GiaVonHangBan.Thang5 - tc_XuatHangSuDungGoiDichVu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_GiaVonHangBan.Thang6 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang6 = (tc_DoanhThuThuan.Thang6 - tc_GiaVonHangBan.Thang6 - tc_XuatHangSuDungGoiDichVu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_GiaVonHangBan.Thang7 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang7 = (tc_DoanhThuThuan.Thang7 - tc_GiaVonHangBan.Thang7 - tc_XuatHangSuDungGoiDichVu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_GiaVonHangBan.Thang8 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang8 = (tc_DoanhThuThuan.Thang8 - tc_GiaVonHangBan.Thang8 - tc_XuatHangSuDungGoiDichVu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_GiaVonHangBan.Thang9 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang9 = (tc_DoanhThuThuan.Thang9 - tc_GiaVonHangBan.Thang9 - tc_XuatHangSuDungGoiDichVu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_GiaVonHangBan.Thang10 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang10 = (tc_DoanhThuThuan.Thang10 - tc_GiaVonHangBan.Thang10 - tc_XuatHangSuDungGoiDichVu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_GiaVonHangBan.Thang11 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang11 = (tc_DoanhThuThuan.Thang11 - tc_GiaVonHangBan.Thang11 - tc_XuatHangSuDungGoiDichVu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_GiaVonHangBan.Thang12 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang12 = (tc_DoanhThuThuan.Thang12 - tc_GiaVonHangBan.Thang12 - tc_XuatHangSuDungGoiDichVu.Thang12);
                    }
                }
                tc_GiaVonHangBan.Tong = (tc_GiaVonHangBan.Thang1 + tc_GiaVonHangBan.Thang2 + tc_GiaVonHangBan.Thang3 + tc_GiaVonHangBan.Thang4 + tc_GiaVonHangBan.Thang5 + tc_GiaVonHangBan.Thang6 + tc_GiaVonHangBan.Thang7 + tc_GiaVonHangBan.Thang8 + tc_GiaVonHangBan.Thang9 + tc_GiaVonHangBan.Thang10 + tc_GiaVonHangBan.Thang11 + tc_GiaVonHangBan.Thang12);
                tc_LoiNhuanGopVeBanHang.Tong = (tc_LoiNhuanGopVeBanHang.Thang1 + tc_LoiNhuanGopVeBanHang.Thang2 + tc_LoiNhuanGopVeBanHang.Thang3 + tc_LoiNhuanGopVeBanHang.Thang4 + tc_LoiNhuanGopVeBanHang.Thang5 + tc_LoiNhuanGopVeBanHang.Thang6 + tc_LoiNhuanGopVeBanHang.Thang7 + tc_LoiNhuanGopVeBanHang.Thang8 + tc_LoiNhuanGopVeBanHang.Thang9 + tc_LoiNhuanGopVeBanHang.Thang10 + tc_LoiNhuanGopVeBanHang.Thang11 + tc_LoiNhuanGopVeBanHang.Thang12);
                lst.Add(tc_GiaVonHangBan);
                lst.Add(tc_XuatHangSuDungGoiDichVu);
                lst.Add(tc_LoiNhuanGopVeBanHang);
                Report_TaiChinh_TheoThang tc_ChiPhi = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhi);
                //tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2 + 7.3)";
                tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2)";
                Report_TaiChinh_TheoThang tc_PhiGIaohangTraDoiTac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiGIaohangTraDoiTac);
                tc_PhiGIaohangTraDoiTac.TaiChinh = "     Phí giao hàng (trả đối tác) (7.1)";
                Report_TaiChinh_TheoThang tc_XuatKhoHangHoa = new Report_TaiChinh_TheoThang();
                tc_XuatKhoHangHoa.TaiChinh = "     Xuất kho hàng hóa (7.1)";
                ResertDM(tc_XuatKhoHangHoa);
                Report_TaiChinh_TheoThang tc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoThang();
                tc_GiaTriThanhToanDiem.TaiChinh = "     Giá trị thanh toán bằng điểm (7.2)";
                ResertDM(tc_GiaTriThanhToanDiem);
                List<SqlParameter> prmcp = new List<SqlParameter>();
                prmcp.Add(new SqlParameter("year", year));
                prmcp.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                List<ReportTaiChinhMonth_ChiPhiBanHangPRC> tbl_ChiPhi = new List<ReportTaiChinhMonth_ChiPhiBanHangPRC>();
                switch (Thang1Quy2Nam3)
                {
                    case 1:
                        tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhMonth_ChiPhiBanHangPRC>("exec ReportTaiChinhMonth_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray()).ToList();
                        break;
                    case 2:
                        tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhMonth_ChiPhiBanHangPRC>("exec ReportTaiChinhMonth_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray()).ToList();
                        break;
                    case 3:
                        tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhMonth_ChiPhiBanHangPRC>("exec ReportTaiChinhMonth_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray()).ToList();
                        break;
                    default:
                        break;
                }
                foreach (var item in tbl_ChiPhi)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_ChiPhi.Thang1 = (tc_PhiGIaohangTraDoiTac.Thang1 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang1 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang1 = (item.DiemThanhToan);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_ChiPhi.Thang2 = (tc_PhiGIaohangTraDoiTac.Thang2 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang2 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang2 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_ChiPhi.Thang3 = (tc_PhiGIaohangTraDoiTac.Thang3 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang3 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang3 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_ChiPhi.Thang4 = (tc_PhiGIaohangTraDoiTac.Thang4 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang4 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang4 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_ChiPhi.Thang5 = (tc_PhiGIaohangTraDoiTac.Thang5 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang5 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang5 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_ChiPhi.Thang6 = (tc_PhiGIaohangTraDoiTac.Thang6 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang6 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang6 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_ChiPhi.Thang7 = (tc_PhiGIaohangTraDoiTac.Thang7 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang7 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang7 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_ChiPhi.Thang8 = (tc_PhiGIaohangTraDoiTac.Thang8 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang8 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang8 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_ChiPhi.Thang9 = (tc_PhiGIaohangTraDoiTac.Thang9 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang9 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang9 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_ChiPhi.Thang10 = (tc_PhiGIaohangTraDoiTac.Thang10 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang10 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang10 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_ChiPhi.Thang11 = (tc_PhiGIaohangTraDoiTac.Thang11 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang11 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang11 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_ChiPhi.Thang12 = (tc_PhiGIaohangTraDoiTac.Thang12 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang12 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang12 = (item.DiemThanhToan);
                    }
                }
                tc_ChiPhi.Tong = (tc_ChiPhi.Thang1 + tc_ChiPhi.Thang2 + tc_ChiPhi.Thang3 + tc_ChiPhi.Thang4 + tc_ChiPhi.Thang5 + tc_ChiPhi.Thang6 + tc_ChiPhi.Thang7 + tc_ChiPhi.Thang8 + tc_ChiPhi.Thang9 + tc_ChiPhi.Thang10 + tc_ChiPhi.Thang11 + tc_ChiPhi.Thang12);
                tc_PhiGIaohangTraDoiTac.Tong = (tc_PhiGIaohangTraDoiTac.Thang1 + tc_PhiGIaohangTraDoiTac.Thang2 + tc_PhiGIaohangTraDoiTac.Thang3 + tc_PhiGIaohangTraDoiTac.Thang4 + tc_PhiGIaohangTraDoiTac.Thang5 + tc_PhiGIaohangTraDoiTac.Thang6 + tc_PhiGIaohangTraDoiTac.Thang7 + tc_PhiGIaohangTraDoiTac.Thang8 + tc_PhiGIaohangTraDoiTac.Thang9 + tc_PhiGIaohangTraDoiTac.Thang10 + tc_PhiGIaohangTraDoiTac.Thang11 + tc_PhiGIaohangTraDoiTac.Thang12);
                tc_XuatKhoHangHoa.Tong = (tc_XuatKhoHangHoa.Thang1 + tc_XuatKhoHangHoa.Thang2 + tc_XuatKhoHangHoa.Thang3 + tc_XuatKhoHangHoa.Thang4 + tc_XuatKhoHangHoa.Thang5 + tc_XuatKhoHangHoa.Thang6 + tc_XuatKhoHangHoa.Thang7 + tc_XuatKhoHangHoa.Thang8 + tc_XuatKhoHangHoa.Thang9 + tc_XuatKhoHangHoa.Thang10 + tc_XuatKhoHangHoa.Thang11 + tc_XuatKhoHangHoa.Thang12);
                tc_GiaTriThanhToanDiem.Tong = (tc_GiaTriThanhToanDiem.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_GiaTriThanhToanDiem.Thang3 + tc_GiaTriThanhToanDiem.Thang4 + tc_GiaTriThanhToanDiem.Thang5 + tc_GiaTriThanhToanDiem.Thang6 + tc_GiaTriThanhToanDiem.Thang7 + tc_GiaTriThanhToanDiem.Thang8 + tc_GiaTriThanhToanDiem.Thang9 + tc_GiaTriThanhToanDiem.Thang10 + tc_GiaTriThanhToanDiem.Thang11 + tc_GiaTriThanhToanDiem.Thang12);
                lst.Add(tc_ChiPhi);
                //lst.Add(tc_PhiGIaohangTraDoiTac);
                lst.Add(tc_XuatKhoHangHoa);
                lst.Add(tc_GiaTriThanhToanDiem);
                Report_TaiChinh_TheoThang tc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = "Lợi nhuận từ hoạt động kinh doanh (8 = 5-6)";
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 = (tc_LoiNhuanGopVeBanHang.Thang1 - tc_ChiPhi.Thang1);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 = (tc_LoiNhuanGopVeBanHang.Thang2 - tc_ChiPhi.Thang2);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 = (tc_LoiNhuanGopVeBanHang.Thang3 - tc_ChiPhi.Thang3);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 = (tc_LoiNhuanGopVeBanHang.Thang4 - tc_ChiPhi.Thang4);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 = (tc_LoiNhuanGopVeBanHang.Thang5 - tc_ChiPhi.Thang5);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 = (tc_LoiNhuanGopVeBanHang.Thang6 - tc_ChiPhi.Thang6);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 = (tc_LoiNhuanGopVeBanHang.Thang7 - tc_ChiPhi.Thang7);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 = (tc_LoiNhuanGopVeBanHang.Thang8 - tc_ChiPhi.Thang8);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 = (tc_LoiNhuanGopVeBanHang.Thang9 - tc_ChiPhi.Thang9);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 = (tc_LoiNhuanGopVeBanHang.Thang10 - tc_ChiPhi.Thang10);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 = (tc_LoiNhuanGopVeBanHang.Thang11 - tc_ChiPhi.Thang11);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 = (tc_LoiNhuanGopVeBanHang.Thang12 - tc_ChiPhi.Thang12);
                tc_LoiNhuanTuHoatDongKinhDoanh.Tong = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang12);
                lst.Add(tc_LoiNhuanTuHoatDongKinhDoanh);
                Report_TaiChinh_TheoThang tc_ThuNhapKhac8 = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac8);
                tc_ThuNhapKhac8.TaiChinh = "Thu nhập khác (9 = 9.1 + 9.2)";
                Report_TaiChinh_TheoThang tc_PhiTraHang = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiTraHang);
                tc_PhiTraHang.TaiChinh = "     Phí trả hàng (9.1)";
                //Report_TaiChinh_TheoThang DM15 = new Report_TaiChinh_TheoThang();
                //ResertDM(DM15);
                //DM15.TaiChinh = "     Khách hàng thanh toán";
                Report_TaiChinh_TheoThang tc_ThuNhapKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac);
                tc_ThuNhapKhac.TaiChinh = "     Thu nhập khác (9.2)";
                Report_TaiChinh_TheoThang tc_ChiPhiKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhiKhac);
                tc_ChiPhiKhac.TaiChinh = "Chi phí khác (10)";
                Report_TaiChinh_TheoThang tc_LoiNhuanThuan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_LoiNhuanThuan);
                tc_LoiNhuanThuan.TaiChinh = "Lợi nhuận thuần (11 = (7+8)-9)";
                List<SqlParameter> prmsq = new List<SqlParameter>();
                prmsq.Add(new SqlParameter("year", year));
                prmsq.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                List<ReportTaiChinhMonth_SoQuyBanHangPRC> tbl_sq = new List<ReportTaiChinhMonth_SoQuyBanHangPRC>();
                switch (Thang1Quy2Nam3)
                {
                    case 1:
                        tbl_sq = _db.Database.SqlQuery<ReportTaiChinhMonth_SoQuyBanHangPRC>("exec ReportTaiChinhMonth_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray()).ToList();
                        break;
                    case 2:
                        tbl_sq = _db.Database.SqlQuery<ReportTaiChinhMonth_SoQuyBanHangPRC>("exec ReportTaiChinhMonth_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray()).ToList();
                        break;
                    case 3:
                        tbl_sq = _db.Database.SqlQuery<ReportTaiChinhMonth_SoQuyBanHangPRC>("exec ReportTaiChinhMonth_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray()).ToList();
                        break;
                    default:
                        break;
                }

                foreach (var item in tbl_sq)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_PhiTraHang.Thang1 = (item.PhiTraHangNhap);
                        //DM15.Thang1 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang1 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang1 = (item.ChiPhiKhac);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_PhiTraHang.Thang2 = (item.PhiTraHangNhap);
                        //DM15.Thang2 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang2 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang2 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_PhiTraHang.Thang3 = (item.PhiTraHangNhap);
                        //DM15.Thang3 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang3 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang3 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_PhiTraHang.Thang4 = (item.PhiTraHangNhap);
                        //DM15.Thang4 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang4 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang4 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_PhiTraHang.Thang5 = (item.PhiTraHangNhap);
                        //DM15.Thang5 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang5 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang5 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_PhiTraHang.Thang6 = (item.PhiTraHangNhap);
                        //DM15.Thang6 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang6 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang6 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_PhiTraHang.Thang7 = (item.PhiTraHangNhap);
                        //DM15.Thang7 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang7 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang7 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_PhiTraHang.Thang8 = (item.PhiTraHangNhap);
                        //DM15.Thang8 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang8 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang8 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_PhiTraHang.Thang9 = (item.PhiTraHangNhap);
                        // DM15.Thang9 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang9 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang9 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_PhiTraHang.Thang10 = (item.PhiTraHangNhap);
                        // DM15.Thang10 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang10 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang10 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_PhiTraHang.Thang11 = (item.PhiTraHangNhap);
                        //DM15.Thang11 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang11 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang11 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_PhiTraHang.Thang12 = (item.PhiTraHangNhap);
                        //DM15.Thang12 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang12 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang12 = (item.ChiPhiKhac);
                    }
                }

                tc_ThuNhapKhac8.Thang1 = (tc_PhiTraHang.Thang1 + tc_ThuNhapKhac.Thang1);
                tc_ThuNhapKhac8.Thang2 = (tc_PhiTraHang.Thang2 + tc_ThuNhapKhac.Thang2);
                tc_ThuNhapKhac8.Thang3 = (tc_PhiTraHang.Thang3 + tc_ThuNhapKhac.Thang3);
                tc_ThuNhapKhac8.Thang4 = (tc_PhiTraHang.Thang4 + tc_ThuNhapKhac.Thang4);
                tc_ThuNhapKhac8.Thang5 = (tc_PhiTraHang.Thang5 + tc_ThuNhapKhac.Thang5);
                tc_ThuNhapKhac8.Thang6 = (tc_PhiTraHang.Thang6 + tc_ThuNhapKhac.Thang6);
                tc_ThuNhapKhac8.Thang7 = (tc_PhiTraHang.Thang7 + tc_ThuNhapKhac.Thang7);
                tc_ThuNhapKhac8.Thang8 = (tc_PhiTraHang.Thang8 + tc_ThuNhapKhac.Thang8);
                tc_ThuNhapKhac8.Thang9 = (tc_PhiTraHang.Thang9 + tc_ThuNhapKhac.Thang9);
                tc_ThuNhapKhac8.Thang10 = (tc_PhiTraHang.Thang10 + tc_ThuNhapKhac.Thang10);
                tc_ThuNhapKhac8.Thang11 = (tc_PhiTraHang.Thang11 + tc_ThuNhapKhac.Thang11);
                tc_ThuNhapKhac8.Thang12 = (tc_PhiTraHang.Thang12 + tc_ThuNhapKhac.Thang12);
                tc_ThuNhapKhac8.Tong = (tc_ThuNhapKhac8.Thang1 + tc_ThuNhapKhac8.Thang2 + tc_ThuNhapKhac8.Thang3 + tc_ThuNhapKhac8.Thang4 + tc_ThuNhapKhac8.Thang5 + tc_ThuNhapKhac8.Thang6 + tc_ThuNhapKhac8.Thang7 + tc_ThuNhapKhac8.Thang8 + tc_ThuNhapKhac8.Thang9 + tc_ThuNhapKhac8.Thang10 + tc_ThuNhapKhac8.Thang11 + tc_ThuNhapKhac8.Thang12);
                tc_PhiTraHang.Tong = (tc_PhiTraHang.Thang1 + tc_PhiTraHang.Thang2 + tc_PhiTraHang.Thang3 + tc_PhiTraHang.Thang4 + tc_PhiTraHang.Thang5 + tc_PhiTraHang.Thang6 + tc_PhiTraHang.Thang7 + tc_PhiTraHang.Thang8 + tc_PhiTraHang.Thang9 + tc_PhiTraHang.Thang10 + tc_PhiTraHang.Thang11 + tc_PhiTraHang.Thang12);
                //DM15.Tong = (DM15.Thang1 + DM15.Thang2 + DM15.Thang3 + DM15.Thang4 + DM15.Thang5 + DM15.Thang6 + DM15.Thang7 + DM15.Thang8 + DM15.Thang9 + DM15.Thang10 + DM15.Thang11 + DM15.Thang12);
                tc_ThuNhapKhac.Tong = (tc_ThuNhapKhac.Thang1 + tc_ThuNhapKhac.Thang2 + tc_ThuNhapKhac.Thang3 + tc_ThuNhapKhac.Thang4 + tc_ThuNhapKhac.Thang5 + tc_ThuNhapKhac.Thang6 + tc_ThuNhapKhac.Thang7 + tc_ThuNhapKhac.Thang8 + tc_ThuNhapKhac.Thang9 + tc_ThuNhapKhac.Thang10 + tc_ThuNhapKhac.Thang11 + tc_ThuNhapKhac.Thang12);
                tc_ChiPhiKhac.Tong = (tc_ChiPhiKhac.Thang1 + tc_ChiPhiKhac.Thang2 + tc_ChiPhiKhac.Thang3 + tc_ChiPhiKhac.Thang4 + tc_ChiPhiKhac.Thang5 + tc_ChiPhiKhac.Thang6 + tc_ChiPhiKhac.Thang7 + tc_ChiPhiKhac.Thang8 + tc_ChiPhiKhac.Thang9 + tc_ChiPhiKhac.Thang10 + tc_ChiPhiKhac.Thang11 + tc_ChiPhiKhac.Thang12);
                lst.Add(tc_ThuNhapKhac8);
                lst.Add(tc_PhiTraHang);
                //lst.Add(DM15);
                lst.Add(tc_ThuNhapKhac);
                lst.Add(tc_ChiPhiKhac);
                tc_LoiNhuanThuan.Thang1 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_ThuNhapKhac8.Thang1 - tc_ChiPhiKhac.Thang1);
                tc_LoiNhuanThuan.Thang2 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_ThuNhapKhac8.Thang2 - tc_ChiPhiKhac.Thang2);
                tc_LoiNhuanThuan.Thang3 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_ThuNhapKhac8.Thang3 - tc_ChiPhiKhac.Thang3);
                tc_LoiNhuanThuan.Thang4 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_ThuNhapKhac8.Thang4 - tc_ChiPhiKhac.Thang4);
                tc_LoiNhuanThuan.Thang5 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_ThuNhapKhac8.Thang5 - tc_ChiPhiKhac.Thang5);
                tc_LoiNhuanThuan.Thang6 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_ThuNhapKhac8.Thang6 - tc_ChiPhiKhac.Thang6);
                tc_LoiNhuanThuan.Thang7 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_ThuNhapKhac8.Thang7 - tc_ChiPhiKhac.Thang7);
                tc_LoiNhuanThuan.Thang8 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_ThuNhapKhac8.Thang8 - tc_ChiPhiKhac.Thang8);
                tc_LoiNhuanThuan.Thang9 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_ThuNhapKhac8.Thang9 - tc_ChiPhiKhac.Thang9);
                tc_LoiNhuanThuan.Thang10 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_ThuNhapKhac8.Thang10 - tc_ChiPhiKhac.Thang10);
                tc_LoiNhuanThuan.Thang11 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang11);
                tc_LoiNhuanThuan.Thang12 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang12);
                tc_LoiNhuanThuan.Tong = (tc_LoiNhuanThuan.Thang1 + tc_LoiNhuanThuan.Thang2 + tc_LoiNhuanThuan.Thang3 + tc_LoiNhuanThuan.Thang4 + tc_LoiNhuanThuan.Thang5 + tc_LoiNhuanThuan.Thang6 + tc_LoiNhuanThuan.Thang7 + tc_LoiNhuanThuan.Thang8 + tc_LoiNhuanThuan.Thang9 + tc_LoiNhuanThuan.Thang10 + tc_LoiNhuanThuan.Thang11 + tc_LoiNhuanThuan.Thang12);
                lst.Add(tc_LoiNhuanThuan);
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - getListTaiChinh_TheoThang: " + ex.Message);
                return new List<Report_TaiChinh_TheoThang>();
            }

        }

        public List<Report_TaiChinh_TheoThang> getListTaiChinh_TheoThang(int year, string ID_ChiNhanh)
        {
            try
            {
                List<Report_TaiChinh_TheoThang> lst = new List<Report_TaiChinh_TheoThang>();
                Report_TaiChinh_TheoThang tc_DoanhThuBanHang = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamTruDoanhThu = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamGiaHoaDon = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_DoanhThuThuan = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoThang();

                tc_DoanhThuBanHang.TaiChinh = "Doanh thu bán hàng (1)";
                tc_DoanhThuBanHang.GhiChu = "Tổng doanh thu bán hàng, gói dịch vụ, hóa đơn sửa chữa (Open24Gara)";
                tc_GiamTruDoanhThu.TaiChinh = "Giảm trừ doanh thu (2 = 2.1 + 2.2)";
                tc_GiamGiaHoaDon.TaiChinh = "     Giảm giá hóa đơn (2.1)";
                tc_GiaTriHangBanBiTraLai.TaiChinh = "     Giá trị hàng bán bị trả lại (2.2)";
                tc_GiaTriHangBanBiTraLai.GhiChu = "Tổng tiền hóa đơn trả hàng";
                tc_XuatHangSuDungGoiDichVu.TaiChinh = "Xuất hàng sử dụng gói dịch vụ (5)";
                tc_XuatHangSuDungGoiDichVu.GhiChu = "Giá vốn khi sử dụng gói dịch vụ, sửa chữa (Open24Gara)";
                tc_DoanhThuThuan.TaiChinh = "Doanh thu thuần (3 = 1 - 2)";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("year", year));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhMonth_DoanhThuBanHangPRC>("exec ReportTaiChinhMonth_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray());

                ResertDM(tc_DoanhThuBanHang);
                ResertDM(tc_GiamTruDoanhThu);
                ResertDM(tc_GiamGiaHoaDon);
                ResertDM(tc_GiaTriHangBanBiTraLai);
                ResertDM(tc_XuatHangSuDungGoiDichVu);
                ResertDM(tc_DoanhThuThuan);
                foreach (var item in tbl_DoanhThuBanHang)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_DoanhThuBanHang.Thang1 = item.DoanhThu;
                        tc_GiamTruDoanhThu.Thang1 = item.GiaTriTra + item.GiamGiaHD;
                        tc_GiamGiaHoaDon.Thang1 = item.GiamGiaHD;
                        tc_GiaTriHangBanBiTraLai.Thang1 = item.GiaTriTra;
                        tc_XuatHangSuDungGoiDichVu.Thang1 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang1 = tc_DoanhThuBanHang.Thang1 - tc_GiamTruDoanhThu.Thang1;
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_DoanhThuBanHang.Thang2 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang2 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang2 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang2 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang2 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang2 = (tc_DoanhThuBanHang.Thang2 - tc_GiamTruDoanhThu.Thang2);

                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_DoanhThuBanHang.Thang3 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang3 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang3 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang3 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang3 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang3 = (tc_DoanhThuBanHang.Thang3 - tc_GiamTruDoanhThu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_DoanhThuBanHang.Thang4 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang4 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang4 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang4 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang4 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang4 = (tc_DoanhThuBanHang.Thang4 - tc_GiamTruDoanhThu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_DoanhThuBanHang.Thang5 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang5 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang5 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang5 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang5 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang5 = (tc_DoanhThuBanHang.Thang5 - tc_GiamTruDoanhThu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_DoanhThuBanHang.Thang6 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang6 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang6 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang6 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang6 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang6 = (tc_DoanhThuBanHang.Thang6 - tc_GiamTruDoanhThu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_DoanhThuBanHang.Thang7 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang7 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang7 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang7 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang7 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang7 = (tc_DoanhThuBanHang.Thang7 - tc_GiamTruDoanhThu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_DoanhThuBanHang.Thang8 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang8 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang8 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang8 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang8 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang8 = (tc_DoanhThuBanHang.Thang8 - tc_GiamTruDoanhThu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_DoanhThuBanHang.Thang9 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang9 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang9 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang9 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang9 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang9 = (tc_DoanhThuBanHang.Thang9 - tc_GiamTruDoanhThu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_DoanhThuBanHang.Thang10 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang10 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang10 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang10 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang10 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang10 = (tc_DoanhThuBanHang.Thang10 - tc_GiamTruDoanhThu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_DoanhThuBanHang.Thang11 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang11 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang11 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang11 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang11 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang11 = (tc_DoanhThuBanHang.Thang11 - tc_GiamTruDoanhThu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_DoanhThuBanHang.Thang12 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang12 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang12 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang12 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang12 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang12 = (tc_DoanhThuBanHang.Thang12 - tc_GiamTruDoanhThu.Thang12);
                    }

                }
                tc_DoanhThuBanHang.Tong = (tc_DoanhThuBanHang.Thang1 + tc_DoanhThuBanHang.Thang2 + tc_DoanhThuBanHang.Thang3 + tc_DoanhThuBanHang.Thang4 + tc_DoanhThuBanHang.Thang5 + tc_DoanhThuBanHang.Thang6 + tc_DoanhThuBanHang.Thang7 + tc_DoanhThuBanHang.Thang8 + tc_DoanhThuBanHang.Thang9 + tc_DoanhThuBanHang.Thang10 + tc_DoanhThuBanHang.Thang11 + tc_DoanhThuBanHang.Thang12);
                tc_GiamTruDoanhThu.Tong = (tc_GiamTruDoanhThu.Thang1 + tc_GiamTruDoanhThu.Thang2 + tc_GiamTruDoanhThu.Thang3 + tc_GiamTruDoanhThu.Thang4 + tc_GiamTruDoanhThu.Thang5 + tc_GiamTruDoanhThu.Thang6 + tc_GiamTruDoanhThu.Thang7 + tc_GiamTruDoanhThu.Thang8 + tc_GiamTruDoanhThu.Thang9 + tc_GiamTruDoanhThu.Thang10 + tc_GiamTruDoanhThu.Thang11 + tc_GiamTruDoanhThu.Thang12);
                tc_GiamGiaHoaDon.Tong = (tc_GiamGiaHoaDon.Thang1 + tc_GiamGiaHoaDon.Thang2 + tc_GiamGiaHoaDon.Thang3 + tc_GiamGiaHoaDon.Thang4 + tc_GiamGiaHoaDon.Thang5 + tc_GiamGiaHoaDon.Thang6 + tc_GiamGiaHoaDon.Thang7 + tc_GiamGiaHoaDon.Thang8 + tc_GiamGiaHoaDon.Thang9 + tc_GiamGiaHoaDon.Thang10 + tc_GiamGiaHoaDon.Thang11 + tc_GiamGiaHoaDon.Thang12);
                tc_GiaTriHangBanBiTraLai.Tong = (tc_GiaTriHangBanBiTraLai.Thang1 + tc_GiaTriHangBanBiTraLai.Thang2 + tc_GiaTriHangBanBiTraLai.Thang3 + tc_GiaTriHangBanBiTraLai.Thang4 + tc_GiaTriHangBanBiTraLai.Thang5 + tc_GiaTriHangBanBiTraLai.Thang6 + tc_GiaTriHangBanBiTraLai.Thang7 + tc_GiaTriHangBanBiTraLai.Thang8 + tc_GiaTriHangBanBiTraLai.Thang9 + tc_GiaTriHangBanBiTraLai.Thang10 + tc_GiaTriHangBanBiTraLai.Thang11 + tc_GiaTriHangBanBiTraLai.Thang12);
                tc_XuatHangSuDungGoiDichVu.Tong = (tc_XuatHangSuDungGoiDichVu.Thang1 + tc_XuatHangSuDungGoiDichVu.Thang2 + tc_XuatHangSuDungGoiDichVu.Thang3 + tc_XuatHangSuDungGoiDichVu.Thang4 + tc_XuatHangSuDungGoiDichVu.Thang5 + tc_XuatHangSuDungGoiDichVu.Thang6 + tc_XuatHangSuDungGoiDichVu.Thang7 + tc_XuatHangSuDungGoiDichVu.Thang8 + tc_XuatHangSuDungGoiDichVu.Thang9 + tc_XuatHangSuDungGoiDichVu.Thang10 + tc_XuatHangSuDungGoiDichVu.Thang11 + tc_XuatHangSuDungGoiDichVu.Thang12);
                tc_DoanhThuThuan.Tong = (tc_DoanhThuThuan.Thang1 + tc_DoanhThuThuan.Thang2 + tc_DoanhThuThuan.Thang3 + tc_DoanhThuThuan.Thang4 + tc_DoanhThuThuan.Thang5 + tc_DoanhThuThuan.Thang6 + tc_DoanhThuThuan.Thang7 + tc_DoanhThuThuan.Thang8 + tc_DoanhThuThuan.Thang9 + tc_DoanhThuThuan.Thang10 + tc_DoanhThuThuan.Thang11 + tc_DoanhThuThuan.Thang12);
                lst.Add(tc_DoanhThuBanHang);
                lst.Add(tc_GiamTruDoanhThu);
                lst.Add(tc_GiamGiaHoaDon);
                lst.Add(tc_GiaTriHangBanBiTraLai);
                lst.Add(tc_DoanhThuThuan);
                //DM    
                List<SqlParameter> prmgv = new List<SqlParameter>();
                prmgv.Add(new SqlParameter("year", year));
                prmgv.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhMonth_GiaVonBanHangPRC>("exec ReportTaiChinhMonth_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray());

                Report_TaiChinh_TheoThang tc_GiaVonHangBan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_GiaVonHangBan);
                tc_GiaVonHangBan.TaiChinh = "Giá vốn hàng bán (4)";
                tc_GiaVonHangBan.GhiChu = "Giá vốn khi bán hàng";
                Report_TaiChinh_TheoThang tc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanGopVeBanHang.TaiChinh = "Lợi nhuận gộp về bán hàng (6 = 3 - 4 - 5)";
                ResertDM(tc_LoiNhuanGopVeBanHang);

                foreach (var item in tbl_GiaVon)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_GiaVonHangBan.Thang1 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang1 = (tc_DoanhThuThuan.Thang1 - tc_GiaVonHangBan.Thang1 - tc_XuatHangSuDungGoiDichVu.Thang1);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_GiaVonHangBan.Thang2 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang2 = (tc_DoanhThuThuan.Thang2 - tc_GiaVonHangBan.Thang2 - tc_XuatHangSuDungGoiDichVu.Thang2);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_GiaVonHangBan.Thang3 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang3 = (tc_DoanhThuThuan.Thang3 - tc_GiaVonHangBan.Thang3 - tc_XuatHangSuDungGoiDichVu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_GiaVonHangBan.Thang4 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang4 = (tc_DoanhThuThuan.Thang4 - tc_GiaVonHangBan.Thang4 - tc_XuatHangSuDungGoiDichVu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_GiaVonHangBan.Thang5 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang5 = (tc_DoanhThuThuan.Thang5 - tc_GiaVonHangBan.Thang5 - tc_XuatHangSuDungGoiDichVu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_GiaVonHangBan.Thang6 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang6 = (tc_DoanhThuThuan.Thang6 - tc_GiaVonHangBan.Thang6 - tc_XuatHangSuDungGoiDichVu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_GiaVonHangBan.Thang7 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang7 = (tc_DoanhThuThuan.Thang7 - tc_GiaVonHangBan.Thang7 - tc_XuatHangSuDungGoiDichVu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_GiaVonHangBan.Thang8 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang8 = (tc_DoanhThuThuan.Thang8 - tc_GiaVonHangBan.Thang8 - tc_XuatHangSuDungGoiDichVu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_GiaVonHangBan.Thang9 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang9 = (tc_DoanhThuThuan.Thang9 - tc_GiaVonHangBan.Thang9 - tc_XuatHangSuDungGoiDichVu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_GiaVonHangBan.Thang10 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang10 = (tc_DoanhThuThuan.Thang10 - tc_GiaVonHangBan.Thang10 - tc_XuatHangSuDungGoiDichVu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_GiaVonHangBan.Thang11 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang11 = (tc_DoanhThuThuan.Thang11 - tc_GiaVonHangBan.Thang11 - tc_XuatHangSuDungGoiDichVu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_GiaVonHangBan.Thang12 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang12 = (tc_DoanhThuThuan.Thang12 - tc_GiaVonHangBan.Thang12 - tc_XuatHangSuDungGoiDichVu.Thang12);
                    }
                }
                tc_GiaVonHangBan.Tong = (tc_GiaVonHangBan.Thang1 + tc_GiaVonHangBan.Thang2 + tc_GiaVonHangBan.Thang3 + tc_GiaVonHangBan.Thang4 + tc_GiaVonHangBan.Thang5 + tc_GiaVonHangBan.Thang6 + tc_GiaVonHangBan.Thang7 + tc_GiaVonHangBan.Thang8 + tc_GiaVonHangBan.Thang9 + tc_GiaVonHangBan.Thang10 + tc_GiaVonHangBan.Thang11 + tc_GiaVonHangBan.Thang12);
                tc_LoiNhuanGopVeBanHang.Tong = (tc_LoiNhuanGopVeBanHang.Thang1 + tc_LoiNhuanGopVeBanHang.Thang2 + tc_LoiNhuanGopVeBanHang.Thang3 + tc_LoiNhuanGopVeBanHang.Thang4 + tc_LoiNhuanGopVeBanHang.Thang5 + tc_LoiNhuanGopVeBanHang.Thang6 + tc_LoiNhuanGopVeBanHang.Thang7 + tc_LoiNhuanGopVeBanHang.Thang8 + tc_LoiNhuanGopVeBanHang.Thang9 + tc_LoiNhuanGopVeBanHang.Thang10 + tc_LoiNhuanGopVeBanHang.Thang11 + tc_LoiNhuanGopVeBanHang.Thang12);
                lst.Add(tc_GiaVonHangBan);
                lst.Add(tc_XuatHangSuDungGoiDichVu);
                lst.Add(tc_LoiNhuanGopVeBanHang);
                Report_TaiChinh_TheoThang tc_ChiPhi = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhi);
                //tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2 + 7.3)";
                tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2)";
                Report_TaiChinh_TheoThang tc_PhiGIaohangTraDoiTac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiGIaohangTraDoiTac);
                tc_PhiGIaohangTraDoiTac.TaiChinh = "     Phí giao hàng (trả đối tác) (7.1)";
                Report_TaiChinh_TheoThang tc_XuatKhoHangHoa = new Report_TaiChinh_TheoThang();
                tc_XuatKhoHangHoa.TaiChinh = "     Xuất kho hàng hóa (7.1)";
                tc_XuatKhoHangHoa.GhiChu = "Tổng giá trị hóa đơn xuất kho, không tính hóa đơn xuất kho sửa chữa";
                ResertDM(tc_XuatKhoHangHoa);
                Report_TaiChinh_TheoThang tc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoThang();
                tc_GiaTriThanhToanDiem.TaiChinh = "     Giá trị thanh toán bằng điểm (7.2)";
                ResertDM(tc_GiaTriThanhToanDiem);
                List<SqlParameter> prmcp = new List<SqlParameter>();
                prmcp.Add(new SqlParameter("year", year));
                prmcp.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhMonth_ChiPhiBanHangPRC>("exec ReportTaiChinhMonth_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray());
                foreach (var item in tbl_ChiPhi)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_ChiPhi.Thang1 = (tc_PhiGIaohangTraDoiTac.Thang1 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang1 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang1 = (item.DiemThanhToan);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_ChiPhi.Thang2 = (tc_PhiGIaohangTraDoiTac.Thang2 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang2 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang2 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_ChiPhi.Thang3 = (tc_PhiGIaohangTraDoiTac.Thang3 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang3 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang3 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_ChiPhi.Thang4 = (tc_PhiGIaohangTraDoiTac.Thang4 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang4 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang4 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_ChiPhi.Thang5 = (tc_PhiGIaohangTraDoiTac.Thang5 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang5 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang5 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_ChiPhi.Thang6 = (tc_PhiGIaohangTraDoiTac.Thang6 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang6 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang6 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_ChiPhi.Thang7 = (tc_PhiGIaohangTraDoiTac.Thang7 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang7 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang7 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_ChiPhi.Thang8 = (tc_PhiGIaohangTraDoiTac.Thang8 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang8 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang8 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_ChiPhi.Thang9 = (tc_PhiGIaohangTraDoiTac.Thang9 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang9 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang9 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_ChiPhi.Thang10 = (tc_PhiGIaohangTraDoiTac.Thang10 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang10 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang10 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_ChiPhi.Thang11 = (tc_PhiGIaohangTraDoiTac.Thang11 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang11 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang11 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_ChiPhi.Thang12 = (tc_PhiGIaohangTraDoiTac.Thang12 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang12 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang12 = (item.DiemThanhToan);
                    }
                }
                tc_ChiPhi.Tong = (tc_ChiPhi.Thang1 + tc_ChiPhi.Thang2 + tc_ChiPhi.Thang3 + tc_ChiPhi.Thang4 + tc_ChiPhi.Thang5 + tc_ChiPhi.Thang6 + tc_ChiPhi.Thang7 + tc_ChiPhi.Thang8 + tc_ChiPhi.Thang9 + tc_ChiPhi.Thang10 + tc_ChiPhi.Thang11 + tc_ChiPhi.Thang12);
                tc_PhiGIaohangTraDoiTac.Tong = (tc_PhiGIaohangTraDoiTac.Thang1 + tc_PhiGIaohangTraDoiTac.Thang2 + tc_PhiGIaohangTraDoiTac.Thang3 + tc_PhiGIaohangTraDoiTac.Thang4 + tc_PhiGIaohangTraDoiTac.Thang5 + tc_PhiGIaohangTraDoiTac.Thang6 + tc_PhiGIaohangTraDoiTac.Thang7 + tc_PhiGIaohangTraDoiTac.Thang8 + tc_PhiGIaohangTraDoiTac.Thang9 + tc_PhiGIaohangTraDoiTac.Thang10 + tc_PhiGIaohangTraDoiTac.Thang11 + tc_PhiGIaohangTraDoiTac.Thang12);
                tc_XuatKhoHangHoa.Tong = (tc_XuatKhoHangHoa.Thang1 + tc_XuatKhoHangHoa.Thang2 + tc_XuatKhoHangHoa.Thang3 + tc_XuatKhoHangHoa.Thang4 + tc_XuatKhoHangHoa.Thang5 + tc_XuatKhoHangHoa.Thang6 + tc_XuatKhoHangHoa.Thang7 + tc_XuatKhoHangHoa.Thang8 + tc_XuatKhoHangHoa.Thang9 + tc_XuatKhoHangHoa.Thang10 + tc_XuatKhoHangHoa.Thang11 + tc_XuatKhoHangHoa.Thang12);
                tc_GiaTriThanhToanDiem.Tong = (tc_GiaTriThanhToanDiem.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_GiaTriThanhToanDiem.Thang3 + tc_GiaTriThanhToanDiem.Thang4 + tc_GiaTriThanhToanDiem.Thang5 + tc_GiaTriThanhToanDiem.Thang6 + tc_GiaTriThanhToanDiem.Thang7 + tc_GiaTriThanhToanDiem.Thang8 + tc_GiaTriThanhToanDiem.Thang9 + tc_GiaTriThanhToanDiem.Thang10 + tc_GiaTriThanhToanDiem.Thang11 + tc_GiaTriThanhToanDiem.Thang12);
                lst.Add(tc_ChiPhi);
                //lst.Add(tc_PhiGIaohangTraDoiTac);
                lst.Add(tc_XuatKhoHangHoa);
                lst.Add(tc_GiaTriThanhToanDiem);
                Report_TaiChinh_TheoThang tc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = "Lợi nhuận từ hoạt động kinh doanh (8 = 6 - 7)";
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 = (tc_LoiNhuanGopVeBanHang.Thang1 - tc_ChiPhi.Thang1);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 = (tc_LoiNhuanGopVeBanHang.Thang2 - tc_ChiPhi.Thang2);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 = (tc_LoiNhuanGopVeBanHang.Thang3 - tc_ChiPhi.Thang3);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 = (tc_LoiNhuanGopVeBanHang.Thang4 - tc_ChiPhi.Thang4);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 = (tc_LoiNhuanGopVeBanHang.Thang5 - tc_ChiPhi.Thang5);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 = (tc_LoiNhuanGopVeBanHang.Thang6 - tc_ChiPhi.Thang6);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 = (tc_LoiNhuanGopVeBanHang.Thang7 - tc_ChiPhi.Thang7);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 = (tc_LoiNhuanGopVeBanHang.Thang8 - tc_ChiPhi.Thang8);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 = (tc_LoiNhuanGopVeBanHang.Thang9 - tc_ChiPhi.Thang9);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 = (tc_LoiNhuanGopVeBanHang.Thang10 - tc_ChiPhi.Thang10);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 = (tc_LoiNhuanGopVeBanHang.Thang11 - tc_ChiPhi.Thang11);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 = (tc_LoiNhuanGopVeBanHang.Thang12 - tc_ChiPhi.Thang12);
                tc_LoiNhuanTuHoatDongKinhDoanh.Tong = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang12);
                lst.Add(tc_LoiNhuanTuHoatDongKinhDoanh);
                Report_TaiChinh_TheoThang tc_ThuNhapKhac8 = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac8);
                tc_ThuNhapKhac8.TaiChinh = "Thu nhập khác (9 = 9.1 + 9.2)";
                Report_TaiChinh_TheoThang tc_PhiTraHang = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiTraHang);
                tc_PhiTraHang.TaiChinh = "     Phí trả hàng (9.1)";
                tc_PhiTraHang.GhiChu = "Trả hàng nhà cung cấp";
                //Report_TaiChinh_TheoThang DM15 = new Report_TaiChinh_TheoThang();
                //ResertDM(DM15);
                //DM15.TaiChinh = "     Khách hàng thanh toán";
                Report_TaiChinh_TheoThang tc_ThuNhapKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac);
                tc_ThuNhapKhac.TaiChinh = "     Thu nhập khác (9.2)";
                tc_ThuNhapKhac.GhiChu = "Tổng tiền thu, không tính thu tiền từ bán hàng.";
                Report_TaiChinh_TheoThang tc_ChiPhiKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhiKhac);
                tc_ChiPhiKhac.TaiChinh = "Chi phí khác (10)";
                tc_ChiPhiKhac.GhiChu = "Tổng tiền chi, không tính chi tiền từ nhập, trả hàng.";
                Report_TaiChinh_TheoThang tc_LoiNhuanThuan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_LoiNhuanThuan);
                tc_LoiNhuanThuan.TaiChinh = "Lợi nhuận thuần (11 = (8 + 9) - 10)";
                List<SqlParameter> prmsq = new List<SqlParameter>();
                prmsq.Add(new SqlParameter("year", year));
                prmsq.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_sq = _db.Database.SqlQuery<ReportTaiChinhMonth_SoQuyBanHangPRC>("exec ReportTaiChinhMonth_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray());
                foreach (var item in tbl_sq)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_PhiTraHang.Thang1 = (item.PhiTraHangNhap);
                        //DM15.Thang1 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang1 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang1 = (item.ChiPhiKhac);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_PhiTraHang.Thang2 = (item.PhiTraHangNhap);
                        //DM15.Thang2 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang2 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang2 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_PhiTraHang.Thang3 = (item.PhiTraHangNhap);
                        //DM15.Thang3 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang3 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang3 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_PhiTraHang.Thang4 = (item.PhiTraHangNhap);
                        //DM15.Thang4 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang4 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang4 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_PhiTraHang.Thang5 = (item.PhiTraHangNhap);
                        //DM15.Thang5 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang5 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang5 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_PhiTraHang.Thang6 = (item.PhiTraHangNhap);
                        //DM15.Thang6 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang6 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang6 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_PhiTraHang.Thang7 = (item.PhiTraHangNhap);
                        //DM15.Thang7 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang7 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang7 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_PhiTraHang.Thang8 = (item.PhiTraHangNhap);
                        //DM15.Thang8 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang8 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang8 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_PhiTraHang.Thang9 = (item.PhiTraHangNhap);
                        // DM15.Thang9 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang9 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang9 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_PhiTraHang.Thang10 = (item.PhiTraHangNhap);
                        // DM15.Thang10 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang10 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang10 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_PhiTraHang.Thang11 = (item.PhiTraHangNhap);
                        //DM15.Thang11 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang11 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang11 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_PhiTraHang.Thang12 = (item.PhiTraHangNhap);
                        //DM15.Thang12 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang12 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang12 = (item.ChiPhiKhac);
                    }
                }

                tc_ThuNhapKhac8.Thang1 = (tc_PhiTraHang.Thang1 + tc_ThuNhapKhac.Thang1);
                tc_ThuNhapKhac8.Thang2 = (tc_PhiTraHang.Thang2 + tc_ThuNhapKhac.Thang2);
                tc_ThuNhapKhac8.Thang3 = (tc_PhiTraHang.Thang3 + tc_ThuNhapKhac.Thang3);
                tc_ThuNhapKhac8.Thang4 = (tc_PhiTraHang.Thang4 + tc_ThuNhapKhac.Thang4);
                tc_ThuNhapKhac8.Thang5 = (tc_PhiTraHang.Thang5 + tc_ThuNhapKhac.Thang5);
                tc_ThuNhapKhac8.Thang6 = (tc_PhiTraHang.Thang6 + tc_ThuNhapKhac.Thang6);
                tc_ThuNhapKhac8.Thang7 = (tc_PhiTraHang.Thang7 + tc_ThuNhapKhac.Thang7);
                tc_ThuNhapKhac8.Thang8 = (tc_PhiTraHang.Thang8 + tc_ThuNhapKhac.Thang8);
                tc_ThuNhapKhac8.Thang9 = (tc_PhiTraHang.Thang9 + tc_ThuNhapKhac.Thang9);
                tc_ThuNhapKhac8.Thang10 = (tc_PhiTraHang.Thang10 + tc_ThuNhapKhac.Thang10);
                tc_ThuNhapKhac8.Thang11 = (tc_PhiTraHang.Thang11 + tc_ThuNhapKhac.Thang11);
                tc_ThuNhapKhac8.Thang12 = (tc_PhiTraHang.Thang12 + tc_ThuNhapKhac.Thang12);
                tc_ThuNhapKhac8.Tong = (tc_ThuNhapKhac8.Thang1 + tc_ThuNhapKhac8.Thang2 + tc_ThuNhapKhac8.Thang3 + tc_ThuNhapKhac8.Thang4 + tc_ThuNhapKhac8.Thang5 + tc_ThuNhapKhac8.Thang6 + tc_ThuNhapKhac8.Thang7 + tc_ThuNhapKhac8.Thang8 + tc_ThuNhapKhac8.Thang9 + tc_ThuNhapKhac8.Thang10 + tc_ThuNhapKhac8.Thang11 + tc_ThuNhapKhac8.Thang12);
                tc_PhiTraHang.Tong = (tc_PhiTraHang.Thang1 + tc_PhiTraHang.Thang2 + tc_PhiTraHang.Thang3 + tc_PhiTraHang.Thang4 + tc_PhiTraHang.Thang5 + tc_PhiTraHang.Thang6 + tc_PhiTraHang.Thang7 + tc_PhiTraHang.Thang8 + tc_PhiTraHang.Thang9 + tc_PhiTraHang.Thang10 + tc_PhiTraHang.Thang11 + tc_PhiTraHang.Thang12);
                //DM15.Tong = (DM15.Thang1 + DM15.Thang2 + DM15.Thang3 + DM15.Thang4 + DM15.Thang5 + DM15.Thang6 + DM15.Thang7 + DM15.Thang8 + DM15.Thang9 + DM15.Thang10 + DM15.Thang11 + DM15.Thang12);
                tc_ThuNhapKhac.Tong = (tc_ThuNhapKhac.Thang1 + tc_ThuNhapKhac.Thang2 + tc_ThuNhapKhac.Thang3 + tc_ThuNhapKhac.Thang4 + tc_ThuNhapKhac.Thang5 + tc_ThuNhapKhac.Thang6 + tc_ThuNhapKhac.Thang7 + tc_ThuNhapKhac.Thang8 + tc_ThuNhapKhac.Thang9 + tc_ThuNhapKhac.Thang10 + tc_ThuNhapKhac.Thang11 + tc_ThuNhapKhac.Thang12);
                tc_ChiPhiKhac.Tong = (tc_ChiPhiKhac.Thang1 + tc_ChiPhiKhac.Thang2 + tc_ChiPhiKhac.Thang3 + tc_ChiPhiKhac.Thang4 + tc_ChiPhiKhac.Thang5 + tc_ChiPhiKhac.Thang6 + tc_ChiPhiKhac.Thang7 + tc_ChiPhiKhac.Thang8 + tc_ChiPhiKhac.Thang9 + tc_ChiPhiKhac.Thang10 + tc_ChiPhiKhac.Thang11 + tc_ChiPhiKhac.Thang12);
                lst.Add(tc_ThuNhapKhac8);
                lst.Add(tc_PhiTraHang);
                //lst.Add(DM15);
                lst.Add(tc_ThuNhapKhac);
                lst.Add(tc_ChiPhiKhac);
                tc_LoiNhuanThuan.Thang1 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_ThuNhapKhac8.Thang1 - tc_ChiPhiKhac.Thang1);
                tc_LoiNhuanThuan.Thang2 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_ThuNhapKhac8.Thang2 - tc_ChiPhiKhac.Thang2);
                tc_LoiNhuanThuan.Thang3 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_ThuNhapKhac8.Thang3 - tc_ChiPhiKhac.Thang3);
                tc_LoiNhuanThuan.Thang4 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_ThuNhapKhac8.Thang4 - tc_ChiPhiKhac.Thang4);
                tc_LoiNhuanThuan.Thang5 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_ThuNhapKhac8.Thang5 - tc_ChiPhiKhac.Thang5);
                tc_LoiNhuanThuan.Thang6 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_ThuNhapKhac8.Thang6 - tc_ChiPhiKhac.Thang6);
                tc_LoiNhuanThuan.Thang7 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_ThuNhapKhac8.Thang7 - tc_ChiPhiKhac.Thang7);
                tc_LoiNhuanThuan.Thang8 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_ThuNhapKhac8.Thang8 - tc_ChiPhiKhac.Thang8);
                tc_LoiNhuanThuan.Thang9 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_ThuNhapKhac8.Thang9 - tc_ChiPhiKhac.Thang9);
                tc_LoiNhuanThuan.Thang10 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_ThuNhapKhac8.Thang10 - tc_ChiPhiKhac.Thang10);
                tc_LoiNhuanThuan.Thang11 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang11);
                tc_LoiNhuanThuan.Thang12 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 + tc_ThuNhapKhac8.Thang12 - tc_ChiPhiKhac.Thang12);
                tc_LoiNhuanThuan.Tong = (tc_LoiNhuanThuan.Thang1 + tc_LoiNhuanThuan.Thang2 + tc_LoiNhuanThuan.Thang3 + tc_LoiNhuanThuan.Thang4 + tc_LoiNhuanThuan.Thang5 + tc_LoiNhuanThuan.Thang6 + tc_LoiNhuanThuan.Thang7 + tc_LoiNhuanThuan.Thang8 + tc_LoiNhuanThuan.Thang9 + tc_LoiNhuanThuan.Thang10 + tc_LoiNhuanThuan.Thang11 + tc_LoiNhuanThuan.Thang12);
                lst.Add(tc_LoiNhuanThuan);
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - getListTaiChinh_TheoThang: " + ex.Message);
                return new List<Report_TaiChinh_TheoThang>();
            }

        }

        public List<Report_TaiChinh_TheoThang> getListTaiChinh_TheoThang_Gara(int year, string ID_ChiNhanh)
        {
            try
            {
                List<Report_TaiChinh_TheoThang> lst = new List<Report_TaiChinh_TheoThang>();
                Report_TaiChinh_TheoThang tc_DoanhThuBanHang = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamTruDoanhThu = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamGiaHoaDon = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_DoanhThuThuan = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoThang();

                tc_DoanhThuBanHang.TaiChinh = "Doanh thu bán hàng (1)";
                tc_DoanhThuBanHang.GhiChu = "Tổng doanh thu bán hàng, gói dịch vụ, hóa đơn sửa chữa (Open24Gara)";
                tc_GiamTruDoanhThu.TaiChinh = "Giảm trừ doanh thu (2 = 2.1 + 2.2)";
                tc_GiamGiaHoaDon.TaiChinh = "     Giảm giá hóa đơn (2.1)";
                tc_GiaTriHangBanBiTraLai.TaiChinh = "     Giá trị hàng bán bị trả lại (2.2)";
                tc_GiaTriHangBanBiTraLai.GhiChu = "Tổng tiền hóa đơn trả hàng";
                tc_XuatHangSuDungGoiDichVu.TaiChinh = "Xuất hàng sửa chữa (5)";
                tc_XuatHangSuDungGoiDichVu.GhiChu = "Giá vốn khi sử dụng gói dịch vụ, sửa chữa (Open24Gara)";
                tc_DoanhThuThuan.TaiChinh = "Doanh thu thuần (3 = 1 - 2)";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("year", year));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhMonth_DoanhThuBanHangPRC>("exec ReportTaiChinhMonth_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray());

                ResertDM(tc_DoanhThuBanHang);
                ResertDM(tc_GiamTruDoanhThu);
                ResertDM(tc_GiamGiaHoaDon);
                ResertDM(tc_GiaTriHangBanBiTraLai);
                ResertDM(tc_XuatHangSuDungGoiDichVu);
                ResertDM(tc_DoanhThuThuan);
                foreach (var item in tbl_DoanhThuBanHang)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_DoanhThuBanHang.Thang1 = item.DoanhThu;
                        tc_GiamTruDoanhThu.Thang1 = item.GiaTriTra + item.GiamGiaHD;
                        tc_GiamGiaHoaDon.Thang1 = item.GiamGiaHD;
                        tc_GiaTriHangBanBiTraLai.Thang1 = item.GiaTriTra;
                        tc_XuatHangSuDungGoiDichVu.Thang1 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang1 = tc_DoanhThuBanHang.Thang1 - tc_GiamTruDoanhThu.Thang1;
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_DoanhThuBanHang.Thang2 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang2 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang2 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang2 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang2 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang2 = (tc_DoanhThuBanHang.Thang2 - tc_GiamTruDoanhThu.Thang2);

                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_DoanhThuBanHang.Thang3 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang3 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang3 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang3 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang3 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang3 = (tc_DoanhThuBanHang.Thang3 - tc_GiamTruDoanhThu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_DoanhThuBanHang.Thang4 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang4 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang4 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang4 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang4 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang4 = (tc_DoanhThuBanHang.Thang4 - tc_GiamTruDoanhThu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_DoanhThuBanHang.Thang5 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang5 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang5 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang5 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang5 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang5 = (tc_DoanhThuBanHang.Thang5 - tc_GiamTruDoanhThu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_DoanhThuBanHang.Thang6 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang6 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang6 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang6 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang6 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang6 = (tc_DoanhThuBanHang.Thang6 - tc_GiamTruDoanhThu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_DoanhThuBanHang.Thang7 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang7 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang7 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang7 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang7 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang7 = (tc_DoanhThuBanHang.Thang7 - tc_GiamTruDoanhThu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_DoanhThuBanHang.Thang8 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang8 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang8 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang8 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang8 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang8 = (tc_DoanhThuBanHang.Thang8 - tc_GiamTruDoanhThu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_DoanhThuBanHang.Thang9 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang9 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang9 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang9 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang9 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang9 = (tc_DoanhThuBanHang.Thang9 - tc_GiamTruDoanhThu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_DoanhThuBanHang.Thang10 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang10 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang10 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang10 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang10 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang10 = (tc_DoanhThuBanHang.Thang10 - tc_GiamTruDoanhThu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_DoanhThuBanHang.Thang11 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang11 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang11 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang11 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang11 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang11 = (tc_DoanhThuBanHang.Thang11 - tc_GiamTruDoanhThu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_DoanhThuBanHang.Thang12 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang12 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang12 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang12 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang12 = item.GiaVonGDV;
                        tc_DoanhThuThuan.Thang12 = (tc_DoanhThuBanHang.Thang12 - tc_GiamTruDoanhThu.Thang12);
                    }

                }
                tc_DoanhThuBanHang.Tong = (tc_DoanhThuBanHang.Thang1 + tc_DoanhThuBanHang.Thang2 + tc_DoanhThuBanHang.Thang3 + tc_DoanhThuBanHang.Thang4 + tc_DoanhThuBanHang.Thang5 + tc_DoanhThuBanHang.Thang6 + tc_DoanhThuBanHang.Thang7 + tc_DoanhThuBanHang.Thang8 + tc_DoanhThuBanHang.Thang9 + tc_DoanhThuBanHang.Thang10 + tc_DoanhThuBanHang.Thang11 + tc_DoanhThuBanHang.Thang12);
                tc_GiamTruDoanhThu.Tong = (tc_GiamTruDoanhThu.Thang1 + tc_GiamTruDoanhThu.Thang2 + tc_GiamTruDoanhThu.Thang3 + tc_GiamTruDoanhThu.Thang4 + tc_GiamTruDoanhThu.Thang5 + tc_GiamTruDoanhThu.Thang6 + tc_GiamTruDoanhThu.Thang7 + tc_GiamTruDoanhThu.Thang8 + tc_GiamTruDoanhThu.Thang9 + tc_GiamTruDoanhThu.Thang10 + tc_GiamTruDoanhThu.Thang11 + tc_GiamTruDoanhThu.Thang12);
                tc_GiamGiaHoaDon.Tong = (tc_GiamGiaHoaDon.Thang1 + tc_GiamGiaHoaDon.Thang2 + tc_GiamGiaHoaDon.Thang3 + tc_GiamGiaHoaDon.Thang4 + tc_GiamGiaHoaDon.Thang5 + tc_GiamGiaHoaDon.Thang6 + tc_GiamGiaHoaDon.Thang7 + tc_GiamGiaHoaDon.Thang8 + tc_GiamGiaHoaDon.Thang9 + tc_GiamGiaHoaDon.Thang10 + tc_GiamGiaHoaDon.Thang11 + tc_GiamGiaHoaDon.Thang12);
                tc_GiaTriHangBanBiTraLai.Tong = (tc_GiaTriHangBanBiTraLai.Thang1 + tc_GiaTriHangBanBiTraLai.Thang2 + tc_GiaTriHangBanBiTraLai.Thang3 + tc_GiaTriHangBanBiTraLai.Thang4 + tc_GiaTriHangBanBiTraLai.Thang5 + tc_GiaTriHangBanBiTraLai.Thang6 + tc_GiaTriHangBanBiTraLai.Thang7 + tc_GiaTriHangBanBiTraLai.Thang8 + tc_GiaTriHangBanBiTraLai.Thang9 + tc_GiaTriHangBanBiTraLai.Thang10 + tc_GiaTriHangBanBiTraLai.Thang11 + tc_GiaTriHangBanBiTraLai.Thang12);
                tc_XuatHangSuDungGoiDichVu.Tong = (tc_XuatHangSuDungGoiDichVu.Thang1 + tc_XuatHangSuDungGoiDichVu.Thang2 + tc_XuatHangSuDungGoiDichVu.Thang3 + tc_XuatHangSuDungGoiDichVu.Thang4 + tc_XuatHangSuDungGoiDichVu.Thang5 + tc_XuatHangSuDungGoiDichVu.Thang6 + tc_XuatHangSuDungGoiDichVu.Thang7 + tc_XuatHangSuDungGoiDichVu.Thang8 + tc_XuatHangSuDungGoiDichVu.Thang9 + tc_XuatHangSuDungGoiDichVu.Thang10 + tc_XuatHangSuDungGoiDichVu.Thang11 + tc_XuatHangSuDungGoiDichVu.Thang12);
                tc_DoanhThuThuan.Tong = (tc_DoanhThuThuan.Thang1 + tc_DoanhThuThuan.Thang2 + tc_DoanhThuThuan.Thang3 + tc_DoanhThuThuan.Thang4 + tc_DoanhThuThuan.Thang5 + tc_DoanhThuThuan.Thang6 + tc_DoanhThuThuan.Thang7 + tc_DoanhThuThuan.Thang8 + tc_DoanhThuThuan.Thang9 + tc_DoanhThuThuan.Thang10 + tc_DoanhThuThuan.Thang11 + tc_DoanhThuThuan.Thang12);
                lst.Add(tc_DoanhThuBanHang);
                lst.Add(tc_GiamTruDoanhThu);
                lst.Add(tc_GiamGiaHoaDon);
                lst.Add(tc_GiaTriHangBanBiTraLai);
                lst.Add(tc_DoanhThuThuan);
                //DM    
                List<SqlParameter> prmgv = new List<SqlParameter>();
                prmgv.Add(new SqlParameter("year", year));
                prmgv.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhMonth_GiaVonBanHangPRC>("exec ReportTaiChinhMonth_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray());

                Report_TaiChinh_TheoThang tc_GiaVonHangBan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_GiaVonHangBan);
                tc_GiaVonHangBan.TaiChinh = "Giá vốn hàng bán (4)";
                tc_GiaVonHangBan.GhiChu = "Giá vốn khi bán hàng";
                Report_TaiChinh_TheoThang tc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanGopVeBanHang.TaiChinh = "Lợi nhuận gộp về bán hàng (7 = 3 - 4 - 5 - 6)";
                ResertDM(tc_LoiNhuanGopVeBanHang);
                List<SqlParameter> prmcpsc = new List<SqlParameter>();
                prmcpsc.Add(new SqlParameter("Year", year));
                prmcpsc.Add(new SqlParameter("IdChiNhanh", ID_ChiNhanh));
                prmcpsc.Add(new SqlParameter("LoaiBaoCao", 1));
                var tbl_ChiPhiSuaChua = _db.Database.SqlQuery<ReportTaiChinh_ChiPhiSuaChuaPRC>("exec ReportTaiChinh_ChiPhiSuaChua @Year, @IdChiNhanh, @LoaiBaoCao", prmcpsc.ToArray());
                Report_TaiChinh_TheoThang tc_ChiPhiSuaChua = new Report_TaiChinh_TheoThang();
                tc_ChiPhiSuaChua.TaiChinh = "Chi phí sửa chữa (6)";
                ResertDM(tc_ChiPhiSuaChua);
                foreach (var item in tbl_ChiPhiSuaChua)
                {
                    if (item.Thang == 1)
                    {
                        tc_ChiPhiSuaChua.Thang1 = item.ChiPhi;
                    }
                    if (item.Thang == 2)
                    {
                        tc_ChiPhiSuaChua.Thang2 = item.ChiPhi;
                    }
                    if (item.Thang == 3)
                    {
                        tc_ChiPhiSuaChua.Thang3 = item.ChiPhi;
                    }
                    if (item.Thang == 4)
                    {
                        tc_ChiPhiSuaChua.Thang4 = item.ChiPhi;
                    }
                    if (item.Thang == 5)
                    {
                        tc_ChiPhiSuaChua.Thang5 = item.ChiPhi;
                    }
                    if (item.Thang == 6)
                    {
                        tc_ChiPhiSuaChua.Thang6 = item.ChiPhi;
                    }
                    if (item.Thang == 7)
                    {
                        tc_ChiPhiSuaChua.Thang7 = item.ChiPhi;
                    }
                    if (item.Thang == 8)
                    {
                        tc_ChiPhiSuaChua.Thang8 = item.ChiPhi;
                    }
                    if (item.Thang == 9)
                    {
                        tc_ChiPhiSuaChua.Thang9 = item.ChiPhi;
                    }
                    if (item.Thang == 10)
                    {
                        tc_ChiPhiSuaChua.Thang10 = item.ChiPhi;
                    }
                    if (item.Thang == 11)
                    {
                        tc_ChiPhiSuaChua.Thang11 = item.ChiPhi;
                    }
                    if (item.Thang == 12)
                    {
                        tc_ChiPhiSuaChua.Thang12 = item.ChiPhi;
                    }
                }
                tc_ChiPhiSuaChua.Tong = tc_ChiPhiSuaChua.Thang1 + tc_ChiPhiSuaChua.Thang2 + tc_ChiPhiSuaChua.Thang3
                    + tc_ChiPhiSuaChua.Thang4 + tc_ChiPhiSuaChua.Thang5 + tc_ChiPhiSuaChua.Thang6 + tc_ChiPhiSuaChua.Thang7
                    + tc_ChiPhiSuaChua.Thang8 + tc_ChiPhiSuaChua.Thang9 + tc_ChiPhiSuaChua.Thang10 + tc_ChiPhiSuaChua.Thang11
                    + tc_ChiPhiSuaChua.Thang12;


                foreach (var item in tbl_GiaVon)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_GiaVonHangBan.Thang1 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang1 = (tc_DoanhThuThuan.Thang1 - tc_GiaVonHangBan.Thang1
                            - tc_XuatHangSuDungGoiDichVu.Thang1 - tc_ChiPhiSuaChua.Thang1);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_GiaVonHangBan.Thang2 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang2 = (tc_DoanhThuThuan.Thang2 - tc_GiaVonHangBan.Thang2
                            - tc_XuatHangSuDungGoiDichVu.Thang2 - tc_ChiPhiSuaChua.Thang2);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_GiaVonHangBan.Thang3 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang3 = (tc_DoanhThuThuan.Thang3 - tc_GiaVonHangBan.Thang3
                            - tc_XuatHangSuDungGoiDichVu.Thang3 - tc_ChiPhiSuaChua.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_GiaVonHangBan.Thang4 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang4 = (tc_DoanhThuThuan.Thang4 - tc_GiaVonHangBan.Thang4
                            - tc_XuatHangSuDungGoiDichVu.Thang4 - tc_ChiPhiSuaChua.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_GiaVonHangBan.Thang5 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang5 = (tc_DoanhThuThuan.Thang5 - tc_GiaVonHangBan.Thang5
                            - tc_XuatHangSuDungGoiDichVu.Thang5 - tc_ChiPhiSuaChua.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_GiaVonHangBan.Thang6 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang6 = (tc_DoanhThuThuan.Thang6 - tc_GiaVonHangBan.Thang6
                            - tc_XuatHangSuDungGoiDichVu.Thang6 - tc_ChiPhiSuaChua.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_GiaVonHangBan.Thang7 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang7 = (tc_DoanhThuThuan.Thang7 - tc_GiaVonHangBan.Thang7
                            - tc_XuatHangSuDungGoiDichVu.Thang7 - tc_ChiPhiSuaChua.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_GiaVonHangBan.Thang8 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang8 = (tc_DoanhThuThuan.Thang8 - tc_GiaVonHangBan.Thang8
                            - tc_XuatHangSuDungGoiDichVu.Thang8 - tc_ChiPhiSuaChua.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_GiaVonHangBan.Thang9 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang9 = (tc_DoanhThuThuan.Thang9 - tc_GiaVonHangBan.Thang9
                            - tc_XuatHangSuDungGoiDichVu.Thang9 - tc_ChiPhiSuaChua.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_GiaVonHangBan.Thang10 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang10 = (tc_DoanhThuThuan.Thang10 - tc_GiaVonHangBan.Thang10
                            - tc_XuatHangSuDungGoiDichVu.Thang10 - tc_ChiPhiSuaChua.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_GiaVonHangBan.Thang11 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang11 = (tc_DoanhThuThuan.Thang11 - tc_GiaVonHangBan.Thang11
                            - tc_XuatHangSuDungGoiDichVu.Thang11 - tc_ChiPhiSuaChua.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_GiaVonHangBan.Thang12 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang12 = (tc_DoanhThuThuan.Thang12 - tc_GiaVonHangBan.Thang12
                            - tc_XuatHangSuDungGoiDichVu.Thang12 - tc_ChiPhiSuaChua.Thang12);
                    }
                }
                tc_GiaVonHangBan.Tong = (tc_GiaVonHangBan.Thang1 + tc_GiaVonHangBan.Thang2 + tc_GiaVonHangBan.Thang3 + tc_GiaVonHangBan.Thang4 + tc_GiaVonHangBan.Thang5 + tc_GiaVonHangBan.Thang6 + tc_GiaVonHangBan.Thang7 + tc_GiaVonHangBan.Thang8 + tc_GiaVonHangBan.Thang9 + tc_GiaVonHangBan.Thang10 + tc_GiaVonHangBan.Thang11 + tc_GiaVonHangBan.Thang12);
                tc_LoiNhuanGopVeBanHang.Tong = (tc_LoiNhuanGopVeBanHang.Thang1 + tc_LoiNhuanGopVeBanHang.Thang2 + tc_LoiNhuanGopVeBanHang.Thang3 + tc_LoiNhuanGopVeBanHang.Thang4 + tc_LoiNhuanGopVeBanHang.Thang5 + tc_LoiNhuanGopVeBanHang.Thang6 + tc_LoiNhuanGopVeBanHang.Thang7 + tc_LoiNhuanGopVeBanHang.Thang8 + tc_LoiNhuanGopVeBanHang.Thang9 + tc_LoiNhuanGopVeBanHang.Thang10 + tc_LoiNhuanGopVeBanHang.Thang11 + tc_LoiNhuanGopVeBanHang.Thang12);
                lst.Add(tc_GiaVonHangBan);
                lst.Add(tc_XuatHangSuDungGoiDichVu);
                lst.Add(tc_ChiPhiSuaChua);
                lst.Add(tc_LoiNhuanGopVeBanHang);
                Report_TaiChinh_TheoThang tc_ChiPhi = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhi);
                //tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2 + 7.3)";
                tc_ChiPhi.TaiChinh = "Chi phí (8 = 8.1 + 8.2)";
                Report_TaiChinh_TheoThang tc_PhiGIaohangTraDoiTac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiGIaohangTraDoiTac);
                tc_PhiGIaohangTraDoiTac.TaiChinh = "     Phí giao hàng (trả đối tác) (8.1)";
                Report_TaiChinh_TheoThang tc_XuatKhoHangHoa = new Report_TaiChinh_TheoThang();
                tc_XuatKhoHangHoa.TaiChinh = "     Xuất kho hàng hóa (8.1)";
                tc_XuatKhoHangHoa.GhiChu = "Tổng giá trị hóa đơn xuất kho, không tính hóa đơn xuất kho sửa chữa";
                ResertDM(tc_XuatKhoHangHoa);
                Report_TaiChinh_TheoThang tc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoThang();
                tc_GiaTriThanhToanDiem.TaiChinh = "     Giá trị thanh toán bằng điểm (8.2)";
                ResertDM(tc_GiaTriThanhToanDiem);
                List<SqlParameter> prmcp = new List<SqlParameter>();
                prmcp.Add(new SqlParameter("year", year));
                prmcp.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhMonth_ChiPhiBanHangPRC>("exec ReportTaiChinhMonth_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray());
                foreach (var item in tbl_ChiPhi)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_ChiPhi.Thang1 = (tc_PhiGIaohangTraDoiTac.Thang1 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang1 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang1 = (item.DiemThanhToan);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_ChiPhi.Thang2 = (tc_PhiGIaohangTraDoiTac.Thang2 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang2 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang2 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_ChiPhi.Thang3 = (tc_PhiGIaohangTraDoiTac.Thang3 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang3 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang3 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_ChiPhi.Thang4 = (tc_PhiGIaohangTraDoiTac.Thang4 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang4 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang4 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_ChiPhi.Thang5 = (tc_PhiGIaohangTraDoiTac.Thang5 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang5 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang5 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_ChiPhi.Thang6 = (tc_PhiGIaohangTraDoiTac.Thang6 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang6 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang6 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_ChiPhi.Thang7 = (tc_PhiGIaohangTraDoiTac.Thang7 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang7 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang7 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_ChiPhi.Thang8 = (tc_PhiGIaohangTraDoiTac.Thang8 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang8 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang8 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_ChiPhi.Thang9 = (tc_PhiGIaohangTraDoiTac.Thang9 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang9 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang9 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_ChiPhi.Thang10 = (tc_PhiGIaohangTraDoiTac.Thang10 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang10 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang10 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_ChiPhi.Thang11 = (tc_PhiGIaohangTraDoiTac.Thang11 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang11 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang11 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_ChiPhi.Thang12 = (tc_PhiGIaohangTraDoiTac.Thang12 + item.GiaTriHuy + item.DiemThanhToan);
                        tc_XuatKhoHangHoa.Thang12 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang12 = (item.DiemThanhToan);
                    }
                }
                tc_ChiPhi.Tong = (tc_ChiPhi.Thang1 + tc_ChiPhi.Thang2 + tc_ChiPhi.Thang3 + tc_ChiPhi.Thang4 + tc_ChiPhi.Thang5 + tc_ChiPhi.Thang6 + tc_ChiPhi.Thang7 + tc_ChiPhi.Thang8 + tc_ChiPhi.Thang9 + tc_ChiPhi.Thang10 + tc_ChiPhi.Thang11 + tc_ChiPhi.Thang12);
                tc_PhiGIaohangTraDoiTac.Tong = (tc_PhiGIaohangTraDoiTac.Thang1 + tc_PhiGIaohangTraDoiTac.Thang2 + tc_PhiGIaohangTraDoiTac.Thang3 + tc_PhiGIaohangTraDoiTac.Thang4 + tc_PhiGIaohangTraDoiTac.Thang5 + tc_PhiGIaohangTraDoiTac.Thang6 + tc_PhiGIaohangTraDoiTac.Thang7 + tc_PhiGIaohangTraDoiTac.Thang8 + tc_PhiGIaohangTraDoiTac.Thang9 + tc_PhiGIaohangTraDoiTac.Thang10 + tc_PhiGIaohangTraDoiTac.Thang11 + tc_PhiGIaohangTraDoiTac.Thang12);
                tc_XuatKhoHangHoa.Tong = (tc_XuatKhoHangHoa.Thang1 + tc_XuatKhoHangHoa.Thang2 + tc_XuatKhoHangHoa.Thang3 + tc_XuatKhoHangHoa.Thang4 + tc_XuatKhoHangHoa.Thang5 + tc_XuatKhoHangHoa.Thang6 + tc_XuatKhoHangHoa.Thang7 + tc_XuatKhoHangHoa.Thang8 + tc_XuatKhoHangHoa.Thang9 + tc_XuatKhoHangHoa.Thang10 + tc_XuatKhoHangHoa.Thang11 + tc_XuatKhoHangHoa.Thang12);
                tc_GiaTriThanhToanDiem.Tong = (tc_GiaTriThanhToanDiem.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_GiaTriThanhToanDiem.Thang3 + tc_GiaTriThanhToanDiem.Thang4 + tc_GiaTriThanhToanDiem.Thang5 + tc_GiaTriThanhToanDiem.Thang6 + tc_GiaTriThanhToanDiem.Thang7 + tc_GiaTriThanhToanDiem.Thang8 + tc_GiaTriThanhToanDiem.Thang9 + tc_GiaTriThanhToanDiem.Thang10 + tc_GiaTriThanhToanDiem.Thang11 + tc_GiaTriThanhToanDiem.Thang12);
                lst.Add(tc_ChiPhi);
                //lst.Add(tc_PhiGIaohangTraDoiTac);
                lst.Add(tc_XuatKhoHangHoa);
                lst.Add(tc_GiaTriThanhToanDiem);
                Report_TaiChinh_TheoThang tc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = "Lợi nhuận từ hoạt động kinh doanh (9 = 7 - 8)";
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 = (tc_LoiNhuanGopVeBanHang.Thang1 - tc_ChiPhi.Thang1);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 = (tc_LoiNhuanGopVeBanHang.Thang2 - tc_ChiPhi.Thang2);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 = (tc_LoiNhuanGopVeBanHang.Thang3 - tc_ChiPhi.Thang3);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 = (tc_LoiNhuanGopVeBanHang.Thang4 - tc_ChiPhi.Thang4);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 = (tc_LoiNhuanGopVeBanHang.Thang5 - tc_ChiPhi.Thang5);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 = (tc_LoiNhuanGopVeBanHang.Thang6 - tc_ChiPhi.Thang6);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 = (tc_LoiNhuanGopVeBanHang.Thang7 - tc_ChiPhi.Thang7);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 = (tc_LoiNhuanGopVeBanHang.Thang8 - tc_ChiPhi.Thang8);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 = (tc_LoiNhuanGopVeBanHang.Thang9 - tc_ChiPhi.Thang9);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 = (tc_LoiNhuanGopVeBanHang.Thang10 - tc_ChiPhi.Thang10);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 = (tc_LoiNhuanGopVeBanHang.Thang11 - tc_ChiPhi.Thang11);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 = (tc_LoiNhuanGopVeBanHang.Thang12 - tc_ChiPhi.Thang12);
                tc_LoiNhuanTuHoatDongKinhDoanh.Tong = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang12);
                lst.Add(tc_LoiNhuanTuHoatDongKinhDoanh);
                Report_TaiChinh_TheoThang tc_ThuNhapKhac8 = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac8);
                tc_ThuNhapKhac8.TaiChinh = "Thu nhập khác (10 = 10.1 + 10.2)";
                Report_TaiChinh_TheoThang tc_PhiTraHang = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiTraHang);
                tc_PhiTraHang.TaiChinh = "     Phí trả hàng (10.1)";
                tc_PhiTraHang.GhiChu = "Trả hàng nhà cung cấp";
                //Report_TaiChinh_TheoThang DM15 = new Report_TaiChinh_TheoThang();
                //ResertDM(DM15);
                //DM15.TaiChinh = "     Khách hàng thanh toán";
                Report_TaiChinh_TheoThang tc_ThuNhapKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac);
                tc_ThuNhapKhac.TaiChinh = "     Thu nhập khác (10.2)";
                tc_ThuNhapKhac.GhiChu = "Tổng tiền thu, không tính thu tiền từ bán hàng.";
                Report_TaiChinh_TheoThang tc_ChiPhiKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhiKhac);
                tc_ChiPhiKhac.TaiChinh = "Chi phí khác (11)";
                tc_ChiPhiKhac.GhiChu = "Tổng tiền chi, không tính chi tiền từ nhập, trả hàng.";
                Report_TaiChinh_TheoThang tc_LoiNhuanThuan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_LoiNhuanThuan);
                tc_LoiNhuanThuan.TaiChinh = "Lợi nhuận thuần (12 = (9 + 10) - 11)";
                List<SqlParameter> prmsq = new List<SqlParameter>();
                prmsq.Add(new SqlParameter("year", year));
                prmsq.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_sq = _db.Database.SqlQuery<ReportTaiChinhMonth_SoQuyBanHangPRC>("exec ReportTaiChinhMonth_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray());
                foreach (var item in tbl_sq)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_PhiTraHang.Thang1 = (item.PhiTraHangNhap);
                        //DM15.Thang1 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang1 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang1 = (item.ChiPhiKhac);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_PhiTraHang.Thang2 = (item.PhiTraHangNhap);
                        //DM15.Thang2 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang2 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang2 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_PhiTraHang.Thang3 = (item.PhiTraHangNhap);
                        //DM15.Thang3 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang3 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang3 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_PhiTraHang.Thang4 = (item.PhiTraHangNhap);
                        //DM15.Thang4 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang4 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang4 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_PhiTraHang.Thang5 = (item.PhiTraHangNhap);
                        //DM15.Thang5 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang5 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang5 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_PhiTraHang.Thang6 = (item.PhiTraHangNhap);
                        //DM15.Thang6 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang6 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang6 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_PhiTraHang.Thang7 = (item.PhiTraHangNhap);
                        //DM15.Thang7 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang7 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang7 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_PhiTraHang.Thang8 = (item.PhiTraHangNhap);
                        //DM15.Thang8 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang8 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang8 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_PhiTraHang.Thang9 = (item.PhiTraHangNhap);
                        // DM15.Thang9 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang9 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang9 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_PhiTraHang.Thang10 = (item.PhiTraHangNhap);
                        // DM15.Thang10 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang10 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang10 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_PhiTraHang.Thang11 = (item.PhiTraHangNhap);
                        //DM15.Thang11 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang11 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang11 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_PhiTraHang.Thang12 = (item.PhiTraHangNhap);
                        //DM15.Thang12 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang12 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang12 = (item.ChiPhiKhac);
                    }
                }

                tc_ThuNhapKhac8.Thang1 = (tc_PhiTraHang.Thang1 + tc_ThuNhapKhac.Thang1);
                tc_ThuNhapKhac8.Thang2 = (tc_PhiTraHang.Thang2 + tc_ThuNhapKhac.Thang2);
                tc_ThuNhapKhac8.Thang3 = (tc_PhiTraHang.Thang3 + tc_ThuNhapKhac.Thang3);
                tc_ThuNhapKhac8.Thang4 = (tc_PhiTraHang.Thang4 + tc_ThuNhapKhac.Thang4);
                tc_ThuNhapKhac8.Thang5 = (tc_PhiTraHang.Thang5 + tc_ThuNhapKhac.Thang5);
                tc_ThuNhapKhac8.Thang6 = (tc_PhiTraHang.Thang6 + tc_ThuNhapKhac.Thang6);
                tc_ThuNhapKhac8.Thang7 = (tc_PhiTraHang.Thang7 + tc_ThuNhapKhac.Thang7);
                tc_ThuNhapKhac8.Thang8 = (tc_PhiTraHang.Thang8 + tc_ThuNhapKhac.Thang8);
                tc_ThuNhapKhac8.Thang9 = (tc_PhiTraHang.Thang9 + tc_ThuNhapKhac.Thang9);
                tc_ThuNhapKhac8.Thang10 = (tc_PhiTraHang.Thang10 + tc_ThuNhapKhac.Thang10);
                tc_ThuNhapKhac8.Thang11 = (tc_PhiTraHang.Thang11 + tc_ThuNhapKhac.Thang11);
                tc_ThuNhapKhac8.Thang12 = (tc_PhiTraHang.Thang12 + tc_ThuNhapKhac.Thang12);
                tc_ThuNhapKhac8.Tong = (tc_ThuNhapKhac8.Thang1 + tc_ThuNhapKhac8.Thang2 + tc_ThuNhapKhac8.Thang3 + tc_ThuNhapKhac8.Thang4 + tc_ThuNhapKhac8.Thang5 + tc_ThuNhapKhac8.Thang6 + tc_ThuNhapKhac8.Thang7 + tc_ThuNhapKhac8.Thang8 + tc_ThuNhapKhac8.Thang9 + tc_ThuNhapKhac8.Thang10 + tc_ThuNhapKhac8.Thang11 + tc_ThuNhapKhac8.Thang12);
                tc_PhiTraHang.Tong = (tc_PhiTraHang.Thang1 + tc_PhiTraHang.Thang2 + tc_PhiTraHang.Thang3 + tc_PhiTraHang.Thang4 + tc_PhiTraHang.Thang5 + tc_PhiTraHang.Thang6 + tc_PhiTraHang.Thang7 + tc_PhiTraHang.Thang8 + tc_PhiTraHang.Thang9 + tc_PhiTraHang.Thang10 + tc_PhiTraHang.Thang11 + tc_PhiTraHang.Thang12);
                //DM15.Tong = (DM15.Thang1 + DM15.Thang2 + DM15.Thang3 + DM15.Thang4 + DM15.Thang5 + DM15.Thang6 + DM15.Thang7 + DM15.Thang8 + DM15.Thang9 + DM15.Thang10 + DM15.Thang11 + DM15.Thang12);
                tc_ThuNhapKhac.Tong = (tc_ThuNhapKhac.Thang1 + tc_ThuNhapKhac.Thang2 + tc_ThuNhapKhac.Thang3 + tc_ThuNhapKhac.Thang4 + tc_ThuNhapKhac.Thang5 + tc_ThuNhapKhac.Thang6 + tc_ThuNhapKhac.Thang7 + tc_ThuNhapKhac.Thang8 + tc_ThuNhapKhac.Thang9 + tc_ThuNhapKhac.Thang10 + tc_ThuNhapKhac.Thang11 + tc_ThuNhapKhac.Thang12);
                tc_ChiPhiKhac.Tong = (tc_ChiPhiKhac.Thang1 + tc_ChiPhiKhac.Thang2 + tc_ChiPhiKhac.Thang3 + tc_ChiPhiKhac.Thang4 + tc_ChiPhiKhac.Thang5 + tc_ChiPhiKhac.Thang6 + tc_ChiPhiKhac.Thang7 + tc_ChiPhiKhac.Thang8 + tc_ChiPhiKhac.Thang9 + tc_ChiPhiKhac.Thang10 + tc_ChiPhiKhac.Thang11 + tc_ChiPhiKhac.Thang12);
                lst.Add(tc_ThuNhapKhac8);
                lst.Add(tc_PhiTraHang);
                //lst.Add(DM15);
                lst.Add(tc_ThuNhapKhac);
                lst.Add(tc_ChiPhiKhac);
                tc_LoiNhuanThuan.Thang1 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_ThuNhapKhac8.Thang1 - tc_ChiPhiKhac.Thang1);
                tc_LoiNhuanThuan.Thang2 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_ThuNhapKhac8.Thang2 - tc_ChiPhiKhac.Thang2);
                tc_LoiNhuanThuan.Thang3 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_ThuNhapKhac8.Thang3 - tc_ChiPhiKhac.Thang3);
                tc_LoiNhuanThuan.Thang4 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_ThuNhapKhac8.Thang4 - tc_ChiPhiKhac.Thang4);
                tc_LoiNhuanThuan.Thang5 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_ThuNhapKhac8.Thang5 - tc_ChiPhiKhac.Thang5);
                tc_LoiNhuanThuan.Thang6 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_ThuNhapKhac8.Thang6 - tc_ChiPhiKhac.Thang6);
                tc_LoiNhuanThuan.Thang7 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_ThuNhapKhac8.Thang7 - tc_ChiPhiKhac.Thang7);
                tc_LoiNhuanThuan.Thang8 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_ThuNhapKhac8.Thang8 - tc_ChiPhiKhac.Thang8);
                tc_LoiNhuanThuan.Thang9 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_ThuNhapKhac8.Thang9 - tc_ChiPhiKhac.Thang9);
                tc_LoiNhuanThuan.Thang10 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_ThuNhapKhac8.Thang10 - tc_ChiPhiKhac.Thang10);
                tc_LoiNhuanThuan.Thang11 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang11);
                tc_LoiNhuanThuan.Thang12 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 + tc_ThuNhapKhac8.Thang12 - tc_ChiPhiKhac.Thang12);
                tc_LoiNhuanThuan.Tong = (tc_LoiNhuanThuan.Thang1 + tc_LoiNhuanThuan.Thang2 + tc_LoiNhuanThuan.Thang3 + tc_LoiNhuanThuan.Thang4 + tc_LoiNhuanThuan.Thang5 + tc_LoiNhuanThuan.Thang6 + tc_LoiNhuanThuan.Thang7 + tc_LoiNhuanThuan.Thang8 + tc_LoiNhuanThuan.Thang9 + tc_LoiNhuanThuan.Thang10 + tc_LoiNhuanThuan.Thang11 + tc_LoiNhuanThuan.Thang12);
                lst.Add(tc_LoiNhuanThuan);
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - getListTaiChinh_TheoThang: " + ex.Message);
                return new List<Report_TaiChinh_TheoThang>();
            }

        }

        public List<Report_TaiChinh_TheoQuy> getListTaiChinh_TheoQuy(int year, string ID_ChiNhanh)
        {
            try
            {
                List<Report_TaiChinh_TheoQuy> lst = new List<Report_TaiChinh_TheoQuy>();
                Report_TaiChinh_TheoThang tc_DoanhThuBanHang = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamTruDoanhThu = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamGiaHoaDon = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_DoanhThuThuan = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoThang();
                tc_DoanhThuBanHang.TaiChinh = "Doanh thu bán hàng (1)";
                tc_GiamTruDoanhThu.TaiChinh = "Giảm trừ doanh thu (2 = 2.1 + 2.2)";
                tc_GiamGiaHoaDon.TaiChinh = "     Giảm giá hóa đơn (2.1)";
                tc_GiaTriHangBanBiTraLai.TaiChinh = "     Giá trị hàng bán bị trả lại (2.2)";
                tc_XuatHangSuDungGoiDichVu.TaiChinh = "Xuất hàng sử dụng gói dịch vụ (5)";
                tc_DoanhThuThuan.TaiChinh = "Doanh thu thuần (3 = 1 - 2)";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("year", year));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhMonth_DoanhThuBanHangPRC>("exec ReportTaiChinhMonth_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray());
                ResertDM(tc_DoanhThuBanHang);
                ResertDM(tc_GiamTruDoanhThu);
                ResertDM(tc_GiamGiaHoaDon);
                ResertDM(tc_GiaTriHangBanBiTraLai);
                ResertDM(tc_DoanhThuThuan);
                foreach (var item in tbl_DoanhThuBanHang)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_DoanhThuBanHang.Thang1 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang1 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang1 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang1 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang1 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang1 = (tc_DoanhThuBanHang.Thang1 - tc_GiamTruDoanhThu.Thang1);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_DoanhThuBanHang.Thang2 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang2 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang2 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang2 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang2 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang2 = (tc_DoanhThuBanHang.Thang2 - tc_GiamTruDoanhThu.Thang2);

                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_DoanhThuBanHang.Thang3 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang3 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang3 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang3 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang3 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang3 = (tc_DoanhThuBanHang.Thang3 - tc_GiamTruDoanhThu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_DoanhThuBanHang.Thang4 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang4 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang4 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang4 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang4 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang4 = (tc_DoanhThuBanHang.Thang4 - tc_GiamTruDoanhThu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_DoanhThuBanHang.Thang5 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang5 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang5 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang5 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang5 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang5 = (tc_DoanhThuBanHang.Thang5 - tc_GiamTruDoanhThu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_DoanhThuBanHang.Thang6 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang6 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang6 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang6 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang6 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang6 = (tc_DoanhThuBanHang.Thang6 - tc_GiamTruDoanhThu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_DoanhThuBanHang.Thang7 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang7 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang7 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang7 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang7 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang7 = (tc_DoanhThuBanHang.Thang7 - tc_GiamTruDoanhThu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_DoanhThuBanHang.Thang8 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang8 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang8 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang8 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang8 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang8 = (tc_DoanhThuBanHang.Thang8 - tc_GiamTruDoanhThu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_DoanhThuBanHang.Thang9 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang9 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang9 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang9 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang9 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang9 = (tc_DoanhThuBanHang.Thang9 - tc_GiamTruDoanhThu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_DoanhThuBanHang.Thang10 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang10 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang10 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang10 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang10 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang10 = (tc_DoanhThuBanHang.Thang10 - tc_GiamTruDoanhThu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_DoanhThuBanHang.Thang11 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang11 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang11 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang11 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang11 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang11 = (tc_DoanhThuBanHang.Thang11 - tc_GiamTruDoanhThu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_DoanhThuBanHang.Thang12 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang12 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang12 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang12 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang12 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang12 = (tc_DoanhThuBanHang.Thang12 - tc_GiamTruDoanhThu.Thang12);
                    }
                }
                tc_DoanhThuBanHang.Tong = (tc_DoanhThuBanHang.Thang1 + tc_DoanhThuBanHang.Thang2 + tc_DoanhThuBanHang.Thang3 + tc_DoanhThuBanHang.Thang4 + tc_DoanhThuBanHang.Thang5 + tc_DoanhThuBanHang.Thang6 + tc_DoanhThuBanHang.Thang7 + tc_DoanhThuBanHang.Thang8 + tc_DoanhThuBanHang.Thang9 + tc_DoanhThuBanHang.Thang10 + tc_DoanhThuBanHang.Thang11 + tc_DoanhThuBanHang.Thang12);
                tc_GiamTruDoanhThu.Tong = (tc_GiamTruDoanhThu.Thang1 + tc_GiamTruDoanhThu.Thang2 + tc_GiamTruDoanhThu.Thang3 + tc_GiamTruDoanhThu.Thang4 + tc_GiamTruDoanhThu.Thang5 + tc_GiamTruDoanhThu.Thang6 + tc_GiamTruDoanhThu.Thang7 + tc_GiamTruDoanhThu.Thang8 + tc_GiamTruDoanhThu.Thang9 + tc_GiamTruDoanhThu.Thang10 + tc_GiamTruDoanhThu.Thang11 + tc_GiamTruDoanhThu.Thang12);
                tc_GiamGiaHoaDon.Tong = (tc_GiamGiaHoaDon.Thang1 + tc_GiamGiaHoaDon.Thang2 + tc_GiamGiaHoaDon.Thang3 + tc_GiamGiaHoaDon.Thang4 + tc_GiamGiaHoaDon.Thang5 + tc_GiamGiaHoaDon.Thang6 + tc_GiamGiaHoaDon.Thang7 + tc_GiamGiaHoaDon.Thang8 + tc_GiamGiaHoaDon.Thang9 + tc_GiamGiaHoaDon.Thang10 + tc_GiamGiaHoaDon.Thang11 + tc_GiamGiaHoaDon.Thang12);
                tc_GiaTriHangBanBiTraLai.Tong = (tc_GiaTriHangBanBiTraLai.Thang1 + tc_GiaTriHangBanBiTraLai.Thang2 + tc_GiaTriHangBanBiTraLai.Thang3 + tc_GiaTriHangBanBiTraLai.Thang4 + tc_GiaTriHangBanBiTraLai.Thang5 + tc_GiaTriHangBanBiTraLai.Thang6 + tc_GiaTriHangBanBiTraLai.Thang7 + tc_GiaTriHangBanBiTraLai.Thang8 + tc_GiaTriHangBanBiTraLai.Thang9 + tc_GiaTriHangBanBiTraLai.Thang10 + tc_GiaTriHangBanBiTraLai.Thang11 + tc_GiaTriHangBanBiTraLai.Thang12);
                tc_XuatHangSuDungGoiDichVu.Tong = (tc_XuatHangSuDungGoiDichVu.Thang1 + tc_XuatHangSuDungGoiDichVu.Thang2 + tc_XuatHangSuDungGoiDichVu.Thang3 + tc_XuatHangSuDungGoiDichVu.Thang4 + tc_XuatHangSuDungGoiDichVu.Thang5 + tc_XuatHangSuDungGoiDichVu.Thang6 + tc_XuatHangSuDungGoiDichVu.Thang7 + tc_XuatHangSuDungGoiDichVu.Thang8 + tc_XuatHangSuDungGoiDichVu.Thang9 + tc_XuatHangSuDungGoiDichVu.Thang10 + tc_XuatHangSuDungGoiDichVu.Thang11 + tc_XuatHangSuDungGoiDichVu.Thang12);
                tc_DoanhThuThuan.Tong = (tc_DoanhThuThuan.Thang1 + tc_DoanhThuThuan.Thang2 + tc_DoanhThuThuan.Thang3 + tc_DoanhThuThuan.Thang4 + tc_DoanhThuThuan.Thang5 + tc_DoanhThuThuan.Thang6 + tc_DoanhThuThuan.Thang7 + tc_DoanhThuThuan.Thang8 + tc_DoanhThuThuan.Thang9 + tc_DoanhThuThuan.Thang10 + tc_DoanhThuThuan.Thang11 + tc_DoanhThuThuan.Thang12);
                //DM    
                List<SqlParameter> prmgv = new List<SqlParameter>();
                prmgv.Add(new SqlParameter("year", year));
                prmgv.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhMonth_GiaVonBanHangPRC>("exec ReportTaiChinhMonth_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray());
                Report_TaiChinh_TheoThang tc_GiaVonBanHang = new Report_TaiChinh_TheoThang();
                ResertDM(tc_GiaVonBanHang);
                tc_GiaVonBanHang.TaiChinh = "Giá vốn hàng bán (4)";
                Report_TaiChinh_TheoThang tc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanGopVeBanHang.TaiChinh = "Lợi nhuận gộp về bán hàng (6 = 3 - 4 - 5)";
                ResertDM(tc_LoiNhuanGopVeBanHang);

                foreach (var item in tbl_GiaVon)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_GiaVonBanHang.Thang1 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang1 = (tc_DoanhThuThuan.Thang1 - tc_GiaVonBanHang.Thang1 - tc_XuatHangSuDungGoiDichVu.Thang1);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_GiaVonBanHang.Thang2 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang2 = (tc_DoanhThuThuan.Thang2 - tc_GiaVonBanHang.Thang2 - tc_XuatHangSuDungGoiDichVu.Thang2);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_GiaVonBanHang.Thang3 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang3 = (tc_DoanhThuThuan.Thang3 - tc_GiaVonBanHang.Thang3 - tc_XuatHangSuDungGoiDichVu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_GiaVonBanHang.Thang4 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang4 = (tc_DoanhThuThuan.Thang4 - tc_GiaVonBanHang.Thang4 - tc_XuatHangSuDungGoiDichVu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_GiaVonBanHang.Thang5 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang5 = (tc_DoanhThuThuan.Thang5 - tc_GiaVonBanHang.Thang5 - tc_XuatHangSuDungGoiDichVu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_GiaVonBanHang.Thang6 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang6 = (tc_DoanhThuThuan.Thang6 - tc_GiaVonBanHang.Thang6 - tc_XuatHangSuDungGoiDichVu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_GiaVonBanHang.Thang7 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang7 = (tc_DoanhThuThuan.Thang7 - tc_GiaVonBanHang.Thang7 - tc_XuatHangSuDungGoiDichVu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_GiaVonBanHang.Thang8 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang8 = (tc_DoanhThuThuan.Thang8 - tc_GiaVonBanHang.Thang8 - tc_XuatHangSuDungGoiDichVu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_GiaVonBanHang.Thang9 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang9 = (tc_DoanhThuThuan.Thang9 - tc_GiaVonBanHang.Thang9 - tc_XuatHangSuDungGoiDichVu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_GiaVonBanHang.Thang10 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang10 = (tc_DoanhThuThuan.Thang10 - tc_GiaVonBanHang.Thang10 - tc_XuatHangSuDungGoiDichVu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_GiaVonBanHang.Thang11 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang11 = (tc_DoanhThuThuan.Thang11 - tc_GiaVonBanHang.Thang11 - tc_XuatHangSuDungGoiDichVu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_GiaVonBanHang.Thang12 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang12 = (tc_DoanhThuThuan.Thang12 - tc_GiaVonBanHang.Thang12 - tc_XuatHangSuDungGoiDichVu.Thang12);
                    }
                }
                tc_GiaVonBanHang.Tong = (tc_GiaVonBanHang.Thang1 + tc_GiaVonBanHang.Thang2 + tc_GiaVonBanHang.Thang3 + tc_GiaVonBanHang.Thang4 + tc_GiaVonBanHang.Thang5 + tc_GiaVonBanHang.Thang6 + tc_GiaVonBanHang.Thang7 + tc_GiaVonBanHang.Thang8 + tc_GiaVonBanHang.Thang9 + tc_GiaVonBanHang.Thang10 + tc_GiaVonBanHang.Thang11 + tc_GiaVonBanHang.Thang12);
                tc_LoiNhuanGopVeBanHang.Tong = (tc_LoiNhuanGopVeBanHang.Thang1 + tc_LoiNhuanGopVeBanHang.Thang2 + tc_LoiNhuanGopVeBanHang.Thang3 + tc_LoiNhuanGopVeBanHang.Thang4 + tc_LoiNhuanGopVeBanHang.Thang5 + tc_LoiNhuanGopVeBanHang.Thang6 + tc_LoiNhuanGopVeBanHang.Thang7 + tc_LoiNhuanGopVeBanHang.Thang8 + tc_LoiNhuanGopVeBanHang.Thang9 + tc_LoiNhuanGopVeBanHang.Thang10 + tc_LoiNhuanGopVeBanHang.Thang11 + tc_LoiNhuanGopVeBanHang.Thang12);

                Report_TaiChinh_TheoThang tc_ChiPhi = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhi);
                //tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2 + 7.3)";
                tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2)";
                Report_TaiChinh_TheoThang tc_PhiGiaoHangTraDoiTac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiGiaoHangTraDoiTac);
                tc_PhiGiaoHangTraDoiTac.TaiChinh = "     Phí giao hàng (trả đối tác) (7.1)";
                Report_TaiChinh_TheoThang tcXuatKhoHangHoa = new Report_TaiChinh_TheoThang();
                tcXuatKhoHangHoa.TaiChinh = "     Xuất kho hàng hóa (7.1)";
                ResertDM(tcXuatKhoHangHoa);
                Report_TaiChinh_TheoThang tc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoThang();
                tc_GiaTriThanhToanDiem.TaiChinh = "     Giá trị thanh toán bằng điểm (7.2)";
                ResertDM(tc_GiaTriThanhToanDiem);
                List<SqlParameter> prmcp = new List<SqlParameter>();
                prmcp.Add(new SqlParameter("year", year));
                prmcp.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhMonth_ChiPhiBanHangPRC>("exec ReportTaiChinhMonth_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray());
                foreach (var item in tbl_ChiPhi)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_ChiPhi.Thang1 = (tc_PhiGiaoHangTraDoiTac.Thang1 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang1 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang1 = (item.DiemThanhToan);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_ChiPhi.Thang2 = (tc_PhiGiaoHangTraDoiTac.Thang2 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang2 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang2 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_ChiPhi.Thang3 = (tc_PhiGiaoHangTraDoiTac.Thang3 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang3 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang3 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_ChiPhi.Thang4 = (tc_PhiGiaoHangTraDoiTac.Thang4 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang4 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang4 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_ChiPhi.Thang5 = (tc_PhiGiaoHangTraDoiTac.Thang5 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang5 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang5 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_ChiPhi.Thang6 = (tc_PhiGiaoHangTraDoiTac.Thang6 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang6 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang6 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_ChiPhi.Thang7 = (tc_PhiGiaoHangTraDoiTac.Thang7 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang7 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang7 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_ChiPhi.Thang8 = (tc_PhiGiaoHangTraDoiTac.Thang8 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang8 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang8 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_ChiPhi.Thang9 = (tc_PhiGiaoHangTraDoiTac.Thang9 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang9 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang9 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_ChiPhi.Thang10 = (tc_PhiGiaoHangTraDoiTac.Thang10 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang10 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang10 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_ChiPhi.Thang11 = (tc_PhiGiaoHangTraDoiTac.Thang11 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang11 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang11 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_ChiPhi.Thang12 = (tc_PhiGiaoHangTraDoiTac.Thang12 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang12 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang12 = (item.DiemThanhToan);
                    }
                }
                tc_ChiPhi.Tong = (tc_ChiPhi.Thang1 + tc_ChiPhi.Thang2 + tc_ChiPhi.Thang3 + tc_ChiPhi.Thang4 + tc_ChiPhi.Thang5 + tc_ChiPhi.Thang6 + tc_ChiPhi.Thang7 + tc_ChiPhi.Thang8 + tc_ChiPhi.Thang9 + tc_ChiPhi.Thang10 + tc_ChiPhi.Thang11 + tc_ChiPhi.Thang12);
                tc_PhiGiaoHangTraDoiTac.Tong = (tc_PhiGiaoHangTraDoiTac.Thang1 + tc_PhiGiaoHangTraDoiTac.Thang2 + tc_PhiGiaoHangTraDoiTac.Thang3 + tc_PhiGiaoHangTraDoiTac.Thang4 + tc_PhiGiaoHangTraDoiTac.Thang5 + tc_PhiGiaoHangTraDoiTac.Thang6 + tc_PhiGiaoHangTraDoiTac.Thang7 + tc_PhiGiaoHangTraDoiTac.Thang8 + tc_PhiGiaoHangTraDoiTac.Thang9 + tc_PhiGiaoHangTraDoiTac.Thang10 + tc_PhiGiaoHangTraDoiTac.Thang11 + tc_PhiGiaoHangTraDoiTac.Thang12);
                tcXuatKhoHangHoa.Tong = (tcXuatKhoHangHoa.Thang1 + tcXuatKhoHangHoa.Thang2 + tcXuatKhoHangHoa.Thang3 + tcXuatKhoHangHoa.Thang4 + tcXuatKhoHangHoa.Thang5 + tcXuatKhoHangHoa.Thang6 + tcXuatKhoHangHoa.Thang7 + tcXuatKhoHangHoa.Thang8 + tcXuatKhoHangHoa.Thang9 + tcXuatKhoHangHoa.Thang10 + tcXuatKhoHangHoa.Thang11 + tcXuatKhoHangHoa.Thang12);
                tc_GiaTriThanhToanDiem.Tong = (tc_GiaTriThanhToanDiem.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_GiaTriThanhToanDiem.Thang3 + tc_GiaTriThanhToanDiem.Thang4 + tc_GiaTriThanhToanDiem.Thang5 + tc_GiaTriThanhToanDiem.Thang6 + tc_GiaTriThanhToanDiem.Thang7 + tc_GiaTriThanhToanDiem.Thang8 + tc_GiaTriThanhToanDiem.Thang9 + tc_GiaTriThanhToanDiem.Thang10 + tc_GiaTriThanhToanDiem.Thang11 + tc_GiaTriThanhToanDiem.Thang12);

                Report_TaiChinh_TheoThang tc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = "Lợi nhuận từ hoạt động kinh doanh (8 = 6 - 7)";
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 = (tc_LoiNhuanGopVeBanHang.Thang1 - tc_ChiPhi.Thang1);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 = (tc_LoiNhuanGopVeBanHang.Thang2 - tc_ChiPhi.Thang2);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 = (tc_LoiNhuanGopVeBanHang.Thang3 - tc_ChiPhi.Thang3);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 = (tc_LoiNhuanGopVeBanHang.Thang4 - tc_ChiPhi.Thang4);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 = (tc_LoiNhuanGopVeBanHang.Thang5 - tc_ChiPhi.Thang5);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 = (tc_LoiNhuanGopVeBanHang.Thang6 - tc_ChiPhi.Thang6);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 = (tc_LoiNhuanGopVeBanHang.Thang7 - tc_ChiPhi.Thang7);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 = (tc_LoiNhuanGopVeBanHang.Thang8 - tc_ChiPhi.Thang8);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 = (tc_LoiNhuanGopVeBanHang.Thang9 - tc_ChiPhi.Thang9);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 = (tc_LoiNhuanGopVeBanHang.Thang10 - tc_ChiPhi.Thang10);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 = (tc_LoiNhuanGopVeBanHang.Thang11 - tc_ChiPhi.Thang11);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 = (tc_LoiNhuanGopVeBanHang.Thang12 - tc_ChiPhi.Thang12);
                tc_LoiNhuanTuHoatDongKinhDoanh.Tong = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang12);

                Report_TaiChinh_TheoThang tc_ThuNhapKhac8 = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac8);
                tc_ThuNhapKhac8.TaiChinh = "Thu nhập khác (9 = 9.1 + 9.2)";
                Report_TaiChinh_TheoThang tc_PhiTraHang = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiTraHang);
                tc_PhiTraHang.TaiChinh = "     Phí trả hàng (9.1)";
                //Report_TaiChinh_TheoThang DM15 = new Report_TaiChinh_TheoThang();
                //ResertDM(DM15);
                //DM15.TaiChinh = "     Khách hàng thanh toán";
                Report_TaiChinh_TheoThang tc_ThuNhapKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac);
                tc_ThuNhapKhac.TaiChinh = "     Thu nhập khác (9.2)";
                Report_TaiChinh_TheoThang tc_ChiPhiKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhiKhac);
                tc_ChiPhiKhac.TaiChinh = "Chi phí khác (10)";
                Report_TaiChinh_TheoThang tc_LoiNhuanThuan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_LoiNhuanThuan);
                tc_LoiNhuanThuan.TaiChinh = "Lợi nhuận thuần (11 = (8 + 9) - 10)";
                List<SqlParameter> prmsq = new List<SqlParameter>();
                prmsq.Add(new SqlParameter("year", year));
                prmsq.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_sq = _db.Database.SqlQuery<ReportTaiChinhMonth_SoQuyBanHangPRC>("exec ReportTaiChinhMonth_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray());

                foreach (var item in tbl_sq)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_PhiTraHang.Thang1 = (item.PhiTraHangNhap);
                        //DM15.Thang1 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang1 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang1 = (item.ChiPhiKhac);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_PhiTraHang.Thang2 = (item.PhiTraHangNhap);
                        //DM15.Thang2 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang2 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang2 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_PhiTraHang.Thang3 = (item.PhiTraHangNhap);
                        //DM15.Thang3 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang3 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang3 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_PhiTraHang.Thang4 = (item.PhiTraHangNhap);
                        //DM15.Thang4 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang4 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang4 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_PhiTraHang.Thang5 = (item.PhiTraHangNhap);
                        //DM15.Thang5 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang5 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang5 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_PhiTraHang.Thang6 = (item.PhiTraHangNhap);
                        //DM15.Thang6 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang6 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang6 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_PhiTraHang.Thang7 = (item.PhiTraHangNhap);
                        //DM15.Thang7 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang7 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang7 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_PhiTraHang.Thang8 = (item.PhiTraHangNhap);
                        //DM15.Thang8 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang8 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang8 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_PhiTraHang.Thang9 = (item.PhiTraHangNhap);
                        //DM15.Thang9 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang9 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang9 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_PhiTraHang.Thang10 = (item.PhiTraHangNhap);
                        //DM15.Thang10 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang10 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang10 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_PhiTraHang.Thang11 = (item.PhiTraHangNhap);
                        //DM15.Thang11 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang11 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang11 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_PhiTraHang.Thang12 = (item.PhiTraHangNhap);
                        //DM15.Thang12 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang12 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang12 = (item.ChiPhiKhac);
                    }
                }

                tc_ThuNhapKhac8.Thang1 = (tc_PhiTraHang.Thang1 + tc_ThuNhapKhac.Thang1);
                tc_ThuNhapKhac8.Thang2 = (tc_PhiTraHang.Thang2 + tc_ThuNhapKhac.Thang2);
                tc_ThuNhapKhac8.Thang3 = (tc_PhiTraHang.Thang3 + tc_ThuNhapKhac.Thang3);
                tc_ThuNhapKhac8.Thang4 = (tc_PhiTraHang.Thang4 + tc_ThuNhapKhac.Thang4);
                tc_ThuNhapKhac8.Thang5 = (tc_PhiTraHang.Thang5 + tc_ThuNhapKhac.Thang5);
                tc_ThuNhapKhac8.Thang6 = (tc_PhiTraHang.Thang6 + tc_ThuNhapKhac.Thang6);
                tc_ThuNhapKhac8.Thang7 = (tc_PhiTraHang.Thang7 + tc_ThuNhapKhac.Thang7);
                tc_ThuNhapKhac8.Thang8 = (tc_PhiTraHang.Thang8 + tc_ThuNhapKhac.Thang8);
                tc_ThuNhapKhac8.Thang9 = (tc_PhiTraHang.Thang9 + tc_ThuNhapKhac.Thang9);
                tc_ThuNhapKhac8.Thang10 = (tc_PhiTraHang.Thang10 + tc_ThuNhapKhac.Thang10);
                tc_ThuNhapKhac8.Thang11 = (tc_PhiTraHang.Thang11 + tc_ThuNhapKhac.Thang11);
                tc_ThuNhapKhac8.Thang12 = (tc_PhiTraHang.Thang12 + tc_ThuNhapKhac.Thang12);
                tc_ThuNhapKhac8.Tong = (tc_ThuNhapKhac8.Thang1 + tc_ThuNhapKhac8.Thang2 + tc_ThuNhapKhac8.Thang3 + tc_ThuNhapKhac8.Thang4 + tc_ThuNhapKhac8.Thang5 + tc_ThuNhapKhac8.Thang6 + tc_ThuNhapKhac8.Thang7 + tc_ThuNhapKhac8.Thang8 + tc_ThuNhapKhac8.Thang9 + tc_ThuNhapKhac8.Thang10 + tc_ThuNhapKhac8.Thang11 + tc_ThuNhapKhac8.Thang12);
                tc_PhiTraHang.Tong = (tc_PhiTraHang.Thang1 + tc_PhiTraHang.Thang2 + tc_PhiTraHang.Thang3 + tc_PhiTraHang.Thang4 + tc_PhiTraHang.Thang5 + tc_PhiTraHang.Thang6 + tc_PhiTraHang.Thang7 + tc_PhiTraHang.Thang8 + tc_PhiTraHang.Thang9 + tc_PhiTraHang.Thang10 + tc_PhiTraHang.Thang11 + tc_PhiTraHang.Thang12);
                //DM15.Tong = (DM15.Thang1 + DM15.Thang2 + DM15.Thang3 + DM15.Thang4 + DM15.Thang5 + DM15.Thang6 + DM15.Thang7 + DM15.Thang8 + DM15.Thang9 + DM15.Thang10 + DM15.Thang11 + DM15.Thang12);
                tc_ThuNhapKhac.Tong = (tc_ThuNhapKhac.Thang1 + tc_ThuNhapKhac.Thang2 + tc_ThuNhapKhac.Thang3 + tc_ThuNhapKhac.Thang4 + tc_ThuNhapKhac.Thang5 + tc_ThuNhapKhac.Thang6 + tc_ThuNhapKhac.Thang7 + tc_ThuNhapKhac.Thang8 + tc_ThuNhapKhac.Thang9 + tc_ThuNhapKhac.Thang10 + tc_ThuNhapKhac.Thang11 + tc_ThuNhapKhac.Thang12);
                tc_ChiPhiKhac.Tong = (tc_ChiPhiKhac.Thang1 + tc_ChiPhiKhac.Thang2 + tc_ChiPhiKhac.Thang3 + tc_ChiPhiKhac.Thang4 + tc_ChiPhiKhac.Thang5 + tc_ChiPhiKhac.Thang6 + tc_ChiPhiKhac.Thang7 + tc_ChiPhiKhac.Thang8 + tc_ChiPhiKhac.Thang9 + tc_ChiPhiKhac.Thang10 + tc_ChiPhiKhac.Thang11 + tc_ChiPhiKhac.Thang12);
                tc_LoiNhuanThuan.Thang1 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_ThuNhapKhac8.Thang1 - tc_ChiPhiKhac.Thang1);
                tc_LoiNhuanThuan.Thang2 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_ThuNhapKhac8.Thang2 - tc_ChiPhiKhac.Thang2);
                tc_LoiNhuanThuan.Thang3 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_ThuNhapKhac8.Thang3 - tc_ChiPhiKhac.Thang3);
                tc_LoiNhuanThuan.Thang4 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_ThuNhapKhac8.Thang4 - tc_ChiPhiKhac.Thang4);
                tc_LoiNhuanThuan.Thang5 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_ThuNhapKhac8.Thang5 - tc_ChiPhiKhac.Thang5);
                tc_LoiNhuanThuan.Thang6 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_ThuNhapKhac8.Thang6 - tc_ChiPhiKhac.Thang6);
                tc_LoiNhuanThuan.Thang7 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_ThuNhapKhac8.Thang7 - tc_ChiPhiKhac.Thang7);
                tc_LoiNhuanThuan.Thang8 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_ThuNhapKhac8.Thang8 - tc_ChiPhiKhac.Thang8);
                tc_LoiNhuanThuan.Thang9 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_ThuNhapKhac8.Thang9 - tc_ChiPhiKhac.Thang9);
                tc_LoiNhuanThuan.Thang10 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_ThuNhapKhac8.Thang10 - tc_ChiPhiKhac.Thang10);
                tc_LoiNhuanThuan.Thang11 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang11);
                tc_LoiNhuanThuan.Thang12 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang12);
                tc_LoiNhuanThuan.Tong = (tc_LoiNhuanThuan.Thang1 + tc_LoiNhuanThuan.Thang2 + tc_LoiNhuanThuan.Thang3 + tc_LoiNhuanThuan.Thang4 + tc_LoiNhuanThuan.Thang5 + tc_LoiNhuanThuan.Thang6 + tc_LoiNhuanThuan.Thang7 + tc_LoiNhuanThuan.Thang8 + tc_LoiNhuanThuan.Thang9 + tc_LoiNhuanThuan.Thang10 + tc_LoiNhuanThuan.Thang11 + tc_LoiNhuanThuan.Thang12);
                Report_TaiChinh_TheoQuy qtc_DoanhThuBanHang = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiamTruDoanhThu = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiamGiaHoaDon = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_DoanhThuThuan = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiaVonBanHang = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ChiPhi = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_PhiGiaoHangTraDoiTac = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtcXuatKhoHangHoa = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ThuNhapKhac8 = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_PhiTraHang = new Report_TaiChinh_TheoQuy();
                //Report_TaiChinh_TheoQuy DQ15 = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ThuNhapKhac = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ChiPhiKhac = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_LoiNhuanThuan = new Report_TaiChinh_TheoQuy();
                qtc_DoanhThuBanHang.TaiChinh = tc_DoanhThuBanHang.TaiChinh;
                qtc_DoanhThuBanHang.Quy1 = (tc_DoanhThuBanHang.Thang1 + tc_DoanhThuBanHang.Thang2 + tc_DoanhThuBanHang.Thang3);
                qtc_DoanhThuBanHang.Quy2 = (tc_DoanhThuBanHang.Thang4 + tc_DoanhThuBanHang.Thang5 + tc_DoanhThuBanHang.Thang6);
                qtc_DoanhThuBanHang.Quy3 = (tc_DoanhThuBanHang.Thang7 + tc_DoanhThuBanHang.Thang8 + tc_DoanhThuBanHang.Thang9);
                qtc_DoanhThuBanHang.Quy4 = (tc_DoanhThuBanHang.Thang10 + tc_DoanhThuBanHang.Thang11 + tc_DoanhThuBanHang.Thang12);
                qtc_DoanhThuBanHang.Tong = tc_DoanhThuBanHang.Tong;

                qtc_GiamTruDoanhThu.TaiChinh = tc_GiamTruDoanhThu.TaiChinh;
                qtc_GiamTruDoanhThu.Quy1 = (tc_GiamTruDoanhThu.Thang1 + tc_GiamTruDoanhThu.Thang2 + tc_GiamTruDoanhThu.Thang3);
                qtc_GiamTruDoanhThu.Quy2 = (tc_GiamTruDoanhThu.Thang4 + tc_GiamTruDoanhThu.Thang5 + tc_GiamTruDoanhThu.Thang6);
                qtc_GiamTruDoanhThu.Quy3 = (tc_GiamTruDoanhThu.Thang7 + tc_GiamTruDoanhThu.Thang8 + tc_GiamTruDoanhThu.Thang9);
                qtc_GiamTruDoanhThu.Quy4 = (tc_GiamTruDoanhThu.Thang10 + tc_GiamTruDoanhThu.Thang11 + tc_GiamTruDoanhThu.Thang12);
                qtc_GiamTruDoanhThu.Tong = tc_GiamTruDoanhThu.Tong;

                qtc_GiamGiaHoaDon.TaiChinh = tc_GiamGiaHoaDon.TaiChinh;
                qtc_GiamGiaHoaDon.Quy1 = (tc_GiamGiaHoaDon.Thang1 + tc_GiamGiaHoaDon.Thang2 + tc_GiamGiaHoaDon.Thang3);
                qtc_GiamGiaHoaDon.Quy2 = (tc_GiamGiaHoaDon.Thang4 + tc_GiamGiaHoaDon.Thang5 + tc_GiamGiaHoaDon.Thang6);
                qtc_GiamGiaHoaDon.Quy3 = (tc_GiamGiaHoaDon.Thang7 + tc_GiamGiaHoaDon.Thang8 + tc_GiamGiaHoaDon.Thang9);
                qtc_GiamGiaHoaDon.Quy4 = (tc_GiamGiaHoaDon.Thang10 + tc_GiamGiaHoaDon.Thang11 + tc_GiamGiaHoaDon.Thang12);
                qtc_GiamGiaHoaDon.Tong = tc_GiamGiaHoaDon.Tong;

                qtc_GiaTriHangBanBiTraLai.TaiChinh = tc_GiaTriHangBanBiTraLai.TaiChinh;
                qtc_GiaTriHangBanBiTraLai.Quy1 = (tc_GiaTriHangBanBiTraLai.Thang1 + tc_GiaTriHangBanBiTraLai.Thang2 + tc_GiaTriHangBanBiTraLai.Thang3);
                qtc_GiaTriHangBanBiTraLai.Quy2 = (tc_GiaTriHangBanBiTraLai.Thang4 + tc_GiaTriHangBanBiTraLai.Thang5 + tc_GiaTriHangBanBiTraLai.Thang6);
                qtc_GiaTriHangBanBiTraLai.Quy3 = (tc_GiaTriHangBanBiTraLai.Thang7 + tc_GiaTriHangBanBiTraLai.Thang8 + tc_GiaTriHangBanBiTraLai.Thang9);
                qtc_GiaTriHangBanBiTraLai.Quy4 = (tc_GiaTriHangBanBiTraLai.Thang10 + tc_GiaTriHangBanBiTraLai.Thang11 + tc_GiaTriHangBanBiTraLai.Thang12);
                qtc_GiaTriHangBanBiTraLai.Tong = tc_GiaTriHangBanBiTraLai.Tong;

                qtc_XuatHangSuDungGoiDichVu.TaiChinh = tc_XuatHangSuDungGoiDichVu.TaiChinh;
                qtc_XuatHangSuDungGoiDichVu.Quy1 = (tc_XuatHangSuDungGoiDichVu.Thang1 + tc_XuatHangSuDungGoiDichVu.Thang2 + tc_XuatHangSuDungGoiDichVu.Thang3);
                qtc_XuatHangSuDungGoiDichVu.Quy2 = (tc_XuatHangSuDungGoiDichVu.Thang4 + tc_XuatHangSuDungGoiDichVu.Thang5 + tc_XuatHangSuDungGoiDichVu.Thang6);
                qtc_XuatHangSuDungGoiDichVu.Quy3 = (tc_XuatHangSuDungGoiDichVu.Thang7 + tc_XuatHangSuDungGoiDichVu.Thang8 + tc_XuatHangSuDungGoiDichVu.Thang9);
                qtc_XuatHangSuDungGoiDichVu.Quy4 = (tc_XuatHangSuDungGoiDichVu.Thang10 + tc_XuatHangSuDungGoiDichVu.Thang11 + tc_XuatHangSuDungGoiDichVu.Thang12);
                qtc_XuatHangSuDungGoiDichVu.Tong = tc_XuatHangSuDungGoiDichVu.Tong;

                qtc_DoanhThuThuan.TaiChinh = tc_DoanhThuThuan.TaiChinh;
                qtc_DoanhThuThuan.Quy1 = (tc_DoanhThuThuan.Thang1 + tc_DoanhThuThuan.Thang2 + tc_DoanhThuThuan.Thang3);
                qtc_DoanhThuThuan.Quy2 = (tc_DoanhThuThuan.Thang4 + tc_DoanhThuThuan.Thang5 + tc_DoanhThuThuan.Thang6);
                qtc_DoanhThuThuan.Quy3 = (tc_DoanhThuThuan.Thang7 + tc_DoanhThuThuan.Thang8 + tc_DoanhThuThuan.Thang9);
                qtc_DoanhThuThuan.Quy4 = (tc_DoanhThuThuan.Thang10 + tc_DoanhThuThuan.Thang11 + tc_DoanhThuThuan.Thang12);
                qtc_DoanhThuThuan.Tong = tc_DoanhThuThuan.Tong;

                qtc_GiaVonBanHang.TaiChinh = tc_GiaVonBanHang.TaiChinh;
                qtc_GiaVonBanHang.Quy1 = (tc_GiaVonBanHang.Thang1 + tc_GiaVonBanHang.Thang2 + tc_GiaVonBanHang.Thang3);
                qtc_GiaVonBanHang.Quy2 = (tc_GiaVonBanHang.Thang4 + tc_GiaVonBanHang.Thang5 + tc_GiaVonBanHang.Thang6);
                qtc_GiaVonBanHang.Quy3 = (tc_GiaVonBanHang.Thang7 + tc_GiaVonBanHang.Thang8 + tc_GiaVonBanHang.Thang9);
                qtc_GiaVonBanHang.Quy4 = (tc_GiaVonBanHang.Thang10 + tc_GiaVonBanHang.Thang11 + tc_GiaVonBanHang.Thang12);
                qtc_GiaVonBanHang.Tong = tc_GiaVonBanHang.Tong;

                qtc_LoiNhuanGopVeBanHang.TaiChinh = tc_LoiNhuanGopVeBanHang.TaiChinh;
                qtc_LoiNhuanGopVeBanHang.Quy1 = (tc_LoiNhuanGopVeBanHang.Thang1 + tc_LoiNhuanGopVeBanHang.Thang2 + tc_LoiNhuanGopVeBanHang.Thang3);
                qtc_LoiNhuanGopVeBanHang.Quy2 = (tc_LoiNhuanGopVeBanHang.Thang4 + tc_LoiNhuanGopVeBanHang.Thang5 + tc_LoiNhuanGopVeBanHang.Thang6);
                qtc_LoiNhuanGopVeBanHang.Quy3 = (tc_LoiNhuanGopVeBanHang.Thang7 + tc_LoiNhuanGopVeBanHang.Thang8 + tc_LoiNhuanGopVeBanHang.Thang9);
                qtc_LoiNhuanGopVeBanHang.Quy4 = (tc_LoiNhuanGopVeBanHang.Thang10 + tc_LoiNhuanGopVeBanHang.Thang11 + tc_LoiNhuanGopVeBanHang.Thang12);
                qtc_LoiNhuanGopVeBanHang.Tong = tc_LoiNhuanGopVeBanHang.Tong;

                qtc_ChiPhi.TaiChinh = tc_ChiPhi.TaiChinh;
                qtc_ChiPhi.Quy1 = (tc_ChiPhi.Thang1 + tc_ChiPhi.Thang2 + tc_ChiPhi.Thang3);
                qtc_ChiPhi.Quy2 = (tc_ChiPhi.Thang4 + tc_ChiPhi.Thang5 + tc_ChiPhi.Thang6);
                qtc_ChiPhi.Quy3 = (tc_ChiPhi.Thang7 + tc_ChiPhi.Thang8 + tc_ChiPhi.Thang9);
                qtc_ChiPhi.Quy4 = (tc_ChiPhi.Thang10 + tc_ChiPhi.Thang11 + tc_ChiPhi.Thang12);
                qtc_ChiPhi.Tong = tc_ChiPhi.Tong;

                //qtc_PhiGiaoHangTraDoiTac.TaiChinh = tc_PhiGiaoHangTraDoiTac.TaiChinh;
                //qtc_PhiGiaoHangTraDoiTac.Quy1 = (tc_PhiGiaoHangTraDoiTac.Thang1 + tc_PhiGiaoHangTraDoiTac.Thang2 + tc_PhiGiaoHangTraDoiTac.Thang3);
                //qtc_PhiGiaoHangTraDoiTac.Quy2 = (tc_PhiGiaoHangTraDoiTac.Thang4 + tc_PhiGiaoHangTraDoiTac.Thang5 + tc_PhiGiaoHangTraDoiTac.Thang6);
                //qtc_PhiGiaoHangTraDoiTac.Quy3 = (tc_PhiGiaoHangTraDoiTac.Thang7 + tc_PhiGiaoHangTraDoiTac.Thang8 + tc_PhiGiaoHangTraDoiTac.Thang9);
                //qtc_PhiGiaoHangTraDoiTac.Quy4 = (tc_PhiGiaoHangTraDoiTac.Thang10 + tc_PhiGiaoHangTraDoiTac.Thang11 + tc_PhiGiaoHangTraDoiTac.Thang12);
                //qtc_PhiGiaoHangTraDoiTac.Tong = tc_PhiGiaoHangTraDoiTac.Tong;

                qtcXuatKhoHangHoa.TaiChinh = tcXuatKhoHangHoa.TaiChinh;
                qtcXuatKhoHangHoa.Quy1 = (tcXuatKhoHangHoa.Thang1 + tcXuatKhoHangHoa.Thang2 + tcXuatKhoHangHoa.Thang3);
                qtcXuatKhoHangHoa.Quy2 = (tcXuatKhoHangHoa.Thang4 + tcXuatKhoHangHoa.Thang5 + tcXuatKhoHangHoa.Thang6);
                qtcXuatKhoHangHoa.Quy3 = (tcXuatKhoHangHoa.Thang7 + tcXuatKhoHangHoa.Thang8 + tcXuatKhoHangHoa.Thang9);
                qtcXuatKhoHangHoa.Quy4 = (tcXuatKhoHangHoa.Thang10 + tcXuatKhoHangHoa.Thang11 + tcXuatKhoHangHoa.Thang12);
                qtcXuatKhoHangHoa.Tong = tcXuatKhoHangHoa.Tong;

                qtc_GiaTriThanhToanDiem.TaiChinh = tc_GiaTriThanhToanDiem.TaiChinh;
                qtc_GiaTriThanhToanDiem.Quy1 = (tc_GiaTriThanhToanDiem.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_GiaTriThanhToanDiem.Thang3);
                qtc_GiaTriThanhToanDiem.Quy2 = (tc_GiaTriThanhToanDiem.Thang4 + tc_GiaTriThanhToanDiem.Thang5 + tc_GiaTriThanhToanDiem.Thang6);
                qtc_GiaTriThanhToanDiem.Quy3 = (tc_GiaTriThanhToanDiem.Thang7 + tc_GiaTriThanhToanDiem.Thang8 + tc_GiaTriThanhToanDiem.Thang9);
                qtc_GiaTriThanhToanDiem.Quy4 = (tc_GiaTriThanhToanDiem.Thang10 + tc_GiaTriThanhToanDiem.Thang11 + tc_GiaTriThanhToanDiem.Thang12);
                qtc_GiaTriThanhToanDiem.Tong = tc_GiaTriThanhToanDiem.Tong;

                qtc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh;
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy1 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang3);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy2 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang6);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy3 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang9);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy4 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang12);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Tong = tc_LoiNhuanTuHoatDongKinhDoanh.Tong;

                qtc_ThuNhapKhac8.TaiChinh = tc_ThuNhapKhac8.TaiChinh;
                qtc_ThuNhapKhac8.Quy1 = (tc_ThuNhapKhac8.Thang1 + tc_ThuNhapKhac8.Thang2 + tc_ThuNhapKhac8.Thang3);
                qtc_ThuNhapKhac8.Quy2 = (tc_ThuNhapKhac8.Thang4 + tc_ThuNhapKhac8.Thang5 + tc_ThuNhapKhac8.Thang6);
                qtc_ThuNhapKhac8.Quy3 = (tc_ThuNhapKhac8.Thang7 + tc_ThuNhapKhac8.Thang8 + tc_ThuNhapKhac8.Thang9);
                qtc_ThuNhapKhac8.Quy4 = (tc_ThuNhapKhac8.Thang10 + tc_ThuNhapKhac8.Thang11 + tc_ThuNhapKhac8.Thang12);
                qtc_ThuNhapKhac8.Tong = tc_ThuNhapKhac8.Tong;

                qtc_PhiTraHang.TaiChinh = tc_PhiTraHang.TaiChinh;
                qtc_PhiTraHang.Quy1 = (tc_PhiTraHang.Thang1 + tc_PhiTraHang.Thang2 + tc_PhiTraHang.Thang3);
                qtc_PhiTraHang.Quy2 = (tc_PhiTraHang.Thang4 + tc_PhiTraHang.Thang5 + tc_PhiTraHang.Thang6);
                qtc_PhiTraHang.Quy3 = (tc_PhiTraHang.Thang7 + tc_PhiTraHang.Thang8 + tc_PhiTraHang.Thang9);
                qtc_PhiTraHang.Quy4 = (tc_PhiTraHang.Thang10 + tc_PhiTraHang.Thang11 + tc_PhiTraHang.Thang12);
                qtc_PhiTraHang.Tong = tc_PhiTraHang.Tong;

                //DQ15.TaiChinh = DM15.TaiChinh;
                //DQ15.Quy1 = (DM15.Thang1 + DM15.Thang2 + DM15.Thang3);
                //DQ15.Quy2 = (DM15.Thang4 + DM15.Thang5 + DM15.Thang6);
                //DQ15.Quy3 = (DM15.Thang7 + DM15.Thang8 + DM15.Thang9);
                //DQ15.Quy4 = (DM15.Thang10 + DM15.Thang11 + DM15.Thang12);
                //DQ15.Tong = DM15.Tong;

                qtc_ThuNhapKhac.TaiChinh = tc_ThuNhapKhac.TaiChinh;
                qtc_ThuNhapKhac.Quy1 = (tc_ThuNhapKhac.Thang1 + tc_ThuNhapKhac.Thang2 + tc_ThuNhapKhac.Thang3);
                qtc_ThuNhapKhac.Quy2 = (tc_ThuNhapKhac.Thang4 + tc_ThuNhapKhac.Thang5 + tc_ThuNhapKhac.Thang6);
                qtc_ThuNhapKhac.Quy3 = (tc_ThuNhapKhac.Thang7 + tc_ThuNhapKhac.Thang8 + tc_ThuNhapKhac.Thang9);
                qtc_ThuNhapKhac.Quy4 = (tc_ThuNhapKhac.Thang10 + tc_ThuNhapKhac.Thang11 + tc_ThuNhapKhac.Thang12);
                qtc_ThuNhapKhac.Tong = tc_ThuNhapKhac.Tong;

                qtc_ChiPhiKhac.TaiChinh = tc_ChiPhiKhac.TaiChinh;
                qtc_ChiPhiKhac.Quy1 = (tc_ChiPhiKhac.Thang1 + tc_ChiPhiKhac.Thang2 + tc_ChiPhiKhac.Thang3);
                qtc_ChiPhiKhac.Quy2 = (tc_ChiPhiKhac.Thang4 + tc_ChiPhiKhac.Thang5 + tc_ChiPhiKhac.Thang6);
                qtc_ChiPhiKhac.Quy3 = (tc_ChiPhiKhac.Thang7 + tc_ChiPhiKhac.Thang8 + tc_ChiPhiKhac.Thang9);
                qtc_ChiPhiKhac.Quy4 = (tc_ChiPhiKhac.Thang10 + tc_ChiPhiKhac.Thang11 + tc_ChiPhiKhac.Thang12);
                qtc_ChiPhiKhac.Tong = tc_ChiPhiKhac.Tong;

                qtc_LoiNhuanThuan.TaiChinh = tc_LoiNhuanThuan.TaiChinh;
                qtc_LoiNhuanThuan.Quy1 = (tc_LoiNhuanThuan.Thang1 + tc_LoiNhuanThuan.Thang2 + tc_LoiNhuanThuan.Thang3);
                qtc_LoiNhuanThuan.Quy2 = (tc_LoiNhuanThuan.Thang4 + tc_LoiNhuanThuan.Thang5 + tc_LoiNhuanThuan.Thang6);
                qtc_LoiNhuanThuan.Quy3 = (tc_LoiNhuanThuan.Thang7 + tc_LoiNhuanThuan.Thang8 + tc_LoiNhuanThuan.Thang9);
                qtc_LoiNhuanThuan.Quy4 = (tc_LoiNhuanThuan.Thang10 + tc_LoiNhuanThuan.Thang11 + tc_LoiNhuanThuan.Thang12);
                qtc_LoiNhuanThuan.Tong = tc_LoiNhuanThuan.Tong;

                lst.Add(qtc_DoanhThuBanHang);
                lst.Add(qtc_GiamTruDoanhThu);
                lst.Add(qtc_GiamGiaHoaDon);
                lst.Add(qtc_GiaTriHangBanBiTraLai);
                lst.Add(qtc_DoanhThuThuan);
                lst.Add(qtc_GiaVonBanHang);
                lst.Add(qtc_XuatHangSuDungGoiDichVu);
                lst.Add(qtc_LoiNhuanGopVeBanHang);
                lst.Add(qtc_ChiPhi);
                //lst.Add(qtc_PhiGiaoHangTraDoiTac);
                lst.Add(qtcXuatKhoHangHoa);
                lst.Add(qtc_GiaTriThanhToanDiem);
                lst.Add(qtc_LoiNhuanTuHoatDongKinhDoanh);
                lst.Add(qtc_ThuNhapKhac8);
                lst.Add(qtc_PhiTraHang);
                //lst.Add(DQ15);
                lst.Add(qtc_ThuNhapKhac);
                lst.Add(qtc_ChiPhiKhac);
                lst.Add(qtc_LoiNhuanThuan);
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - getListTaiChinh_TheoQuy: " + ex.Message);
                return new List<Report_TaiChinh_TheoQuy>();
            }
        }

        public List<Report_TaiChinh_TheoQuy> getListTaiChinh_TheoQuy_Gara(int year, string ID_ChiNhanh)
        {
            try
            {
                List<Report_TaiChinh_TheoQuy> lst = new List<Report_TaiChinh_TheoQuy>();
                Report_TaiChinh_TheoThang tc_DoanhThuBanHang = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamTruDoanhThu = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiamGiaHoaDon = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_DoanhThuThuan = new Report_TaiChinh_TheoThang();
                Report_TaiChinh_TheoThang tc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoThang();
                tc_DoanhThuBanHang.TaiChinh = "Doanh thu bán hàng (1)";
                tc_GiamTruDoanhThu.TaiChinh = "Giảm trừ doanh thu (2 = 2.1 + 2.2)";
                tc_GiamGiaHoaDon.TaiChinh = "     Giảm giá hóa đơn (2.1)";
                tc_GiaTriHangBanBiTraLai.TaiChinh = "     Giá trị hàng bán bị trả lại (2.2)";
                tc_XuatHangSuDungGoiDichVu.TaiChinh = "Xuất hàng sửa chữa (5)";
                tc_DoanhThuThuan.TaiChinh = "Doanh thu thuần (3 = 1 - 2)";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("year", year));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhMonth_DoanhThuBanHangPRC>("exec ReportTaiChinhMonth_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray());
                ResertDM(tc_DoanhThuBanHang);
                ResertDM(tc_GiamTruDoanhThu);
                ResertDM(tc_GiamGiaHoaDon);
                ResertDM(tc_GiaTriHangBanBiTraLai);
                ResertDM(tc_DoanhThuThuan);
                foreach (var item in tbl_DoanhThuBanHang)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_DoanhThuBanHang.Thang1 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang1 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang1 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang1 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang1 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang1 = (tc_DoanhThuBanHang.Thang1 - tc_GiamTruDoanhThu.Thang1);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_DoanhThuBanHang.Thang2 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang2 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang2 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang2 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang2 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang2 = (tc_DoanhThuBanHang.Thang2 - tc_GiamTruDoanhThu.Thang2);

                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_DoanhThuBanHang.Thang3 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang3 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang3 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang3 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang3 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang3 = (tc_DoanhThuBanHang.Thang3 - tc_GiamTruDoanhThu.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_DoanhThuBanHang.Thang4 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang4 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang4 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang4 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang4 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang4 = (tc_DoanhThuBanHang.Thang4 - tc_GiamTruDoanhThu.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_DoanhThuBanHang.Thang5 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang5 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang5 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang5 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang5 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang5 = (tc_DoanhThuBanHang.Thang5 - tc_GiamTruDoanhThu.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_DoanhThuBanHang.Thang6 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang6 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang6 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang6 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang6 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang6 = (tc_DoanhThuBanHang.Thang6 - tc_GiamTruDoanhThu.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_DoanhThuBanHang.Thang7 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang7 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang7 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang7 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang7 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang7 = (tc_DoanhThuBanHang.Thang7 - tc_GiamTruDoanhThu.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_DoanhThuBanHang.Thang8 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang8 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang8 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang8 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang8 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang8 = (tc_DoanhThuBanHang.Thang8 - tc_GiamTruDoanhThu.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_DoanhThuBanHang.Thang9 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang9 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang9 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang9 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang9 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang9 = (tc_DoanhThuBanHang.Thang9 - tc_GiamTruDoanhThu.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_DoanhThuBanHang.Thang10 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang10 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang10 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang10 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang10 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang10 = (tc_DoanhThuBanHang.Thang10 - tc_GiamTruDoanhThu.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_DoanhThuBanHang.Thang11 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang11 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang11 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang11 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang11 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang11 = (tc_DoanhThuBanHang.Thang11 - tc_GiamTruDoanhThu.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_DoanhThuBanHang.Thang12 = (item.DoanhThu);
                        tc_GiamTruDoanhThu.Thang12 = (item.GiaTriTra + item.GiamGiaHD);
                        tc_GiamGiaHoaDon.Thang12 = (item.GiamGiaHD);
                        tc_GiaTriHangBanBiTraLai.Thang12 = (item.GiaTriTra);
                        tc_XuatHangSuDungGoiDichVu.Thang12 = (item.GiaVonGDV);
                        tc_DoanhThuThuan.Thang12 = (tc_DoanhThuBanHang.Thang12 - tc_GiamTruDoanhThu.Thang12);
                    }
                }
                tc_DoanhThuBanHang.Tong = (tc_DoanhThuBanHang.Thang1 + tc_DoanhThuBanHang.Thang2 + tc_DoanhThuBanHang.Thang3 + tc_DoanhThuBanHang.Thang4 + tc_DoanhThuBanHang.Thang5 + tc_DoanhThuBanHang.Thang6 + tc_DoanhThuBanHang.Thang7 + tc_DoanhThuBanHang.Thang8 + tc_DoanhThuBanHang.Thang9 + tc_DoanhThuBanHang.Thang10 + tc_DoanhThuBanHang.Thang11 + tc_DoanhThuBanHang.Thang12);
                tc_GiamTruDoanhThu.Tong = (tc_GiamTruDoanhThu.Thang1 + tc_GiamTruDoanhThu.Thang2 + tc_GiamTruDoanhThu.Thang3 + tc_GiamTruDoanhThu.Thang4 + tc_GiamTruDoanhThu.Thang5 + tc_GiamTruDoanhThu.Thang6 + tc_GiamTruDoanhThu.Thang7 + tc_GiamTruDoanhThu.Thang8 + tc_GiamTruDoanhThu.Thang9 + tc_GiamTruDoanhThu.Thang10 + tc_GiamTruDoanhThu.Thang11 + tc_GiamTruDoanhThu.Thang12);
                tc_GiamGiaHoaDon.Tong = (tc_GiamGiaHoaDon.Thang1 + tc_GiamGiaHoaDon.Thang2 + tc_GiamGiaHoaDon.Thang3 + tc_GiamGiaHoaDon.Thang4 + tc_GiamGiaHoaDon.Thang5 + tc_GiamGiaHoaDon.Thang6 + tc_GiamGiaHoaDon.Thang7 + tc_GiamGiaHoaDon.Thang8 + tc_GiamGiaHoaDon.Thang9 + tc_GiamGiaHoaDon.Thang10 + tc_GiamGiaHoaDon.Thang11 + tc_GiamGiaHoaDon.Thang12);
                tc_GiaTriHangBanBiTraLai.Tong = (tc_GiaTriHangBanBiTraLai.Thang1 + tc_GiaTriHangBanBiTraLai.Thang2 + tc_GiaTriHangBanBiTraLai.Thang3 + tc_GiaTriHangBanBiTraLai.Thang4 + tc_GiaTriHangBanBiTraLai.Thang5 + tc_GiaTriHangBanBiTraLai.Thang6 + tc_GiaTriHangBanBiTraLai.Thang7 + tc_GiaTriHangBanBiTraLai.Thang8 + tc_GiaTriHangBanBiTraLai.Thang9 + tc_GiaTriHangBanBiTraLai.Thang10 + tc_GiaTriHangBanBiTraLai.Thang11 + tc_GiaTriHangBanBiTraLai.Thang12);
                tc_XuatHangSuDungGoiDichVu.Tong = (tc_XuatHangSuDungGoiDichVu.Thang1 + tc_XuatHangSuDungGoiDichVu.Thang2 + tc_XuatHangSuDungGoiDichVu.Thang3 + tc_XuatHangSuDungGoiDichVu.Thang4 + tc_XuatHangSuDungGoiDichVu.Thang5 + tc_XuatHangSuDungGoiDichVu.Thang6 + tc_XuatHangSuDungGoiDichVu.Thang7 + tc_XuatHangSuDungGoiDichVu.Thang8 + tc_XuatHangSuDungGoiDichVu.Thang9 + tc_XuatHangSuDungGoiDichVu.Thang10 + tc_XuatHangSuDungGoiDichVu.Thang11 + tc_XuatHangSuDungGoiDichVu.Thang12);
                tc_DoanhThuThuan.Tong = (tc_DoanhThuThuan.Thang1 + tc_DoanhThuThuan.Thang2 + tc_DoanhThuThuan.Thang3 + tc_DoanhThuThuan.Thang4 + tc_DoanhThuThuan.Thang5 + tc_DoanhThuThuan.Thang6 + tc_DoanhThuThuan.Thang7 + tc_DoanhThuThuan.Thang8 + tc_DoanhThuThuan.Thang9 + tc_DoanhThuThuan.Thang10 + tc_DoanhThuThuan.Thang11 + tc_DoanhThuThuan.Thang12);
                //DM
                //
                List<SqlParameter> prmcpsc = new List<SqlParameter>();
                prmcpsc.Add(new SqlParameter("Year", year));
                prmcpsc.Add(new SqlParameter("IdChiNhanh", ID_ChiNhanh));
                prmcpsc.Add(new SqlParameter("LoaiBaoCao", 1));
                var tbl_ChiPhiSuaChua = _db.Database.SqlQuery<ReportTaiChinh_ChiPhiSuaChuaPRC>("exec ReportTaiChinh_ChiPhiSuaChua @Year, @IdChiNhanh, @LoaiBaoCao", prmcpsc.ToArray());
                Report_TaiChinh_TheoThang tc_ChiPhiSuaChua = new Report_TaiChinh_TheoThang();
                tc_ChiPhiSuaChua.TaiChinh = "Chi phí sửa chữa (6)";
                ResertDM(tc_ChiPhiSuaChua);
                foreach (var item in tbl_ChiPhiSuaChua)
                {
                    if (item.Thang == 1)
                    {
                        tc_ChiPhiSuaChua.Thang1 = item.ChiPhi;
                    }
                    if (item.Thang == 2)
                    {
                        tc_ChiPhiSuaChua.Thang2 = item.ChiPhi;
                    }
                    if (item.Thang == 3)
                    {
                        tc_ChiPhiSuaChua.Thang3 = item.ChiPhi;
                    }
                    if (item.Thang == 4)
                    {
                        tc_ChiPhiSuaChua.Thang4 = item.ChiPhi;
                    }
                    if (item.Thang == 5)
                    {
                        tc_ChiPhiSuaChua.Thang5 = item.ChiPhi;
                    }
                    if (item.Thang == 6)
                    {
                        tc_ChiPhiSuaChua.Thang6 = item.ChiPhi;
                    }
                    if (item.Thang == 7)
                    {
                        tc_ChiPhiSuaChua.Thang7 = item.ChiPhi;
                    }
                    if (item.Thang == 8)
                    {
                        tc_ChiPhiSuaChua.Thang8 = item.ChiPhi;
                    }
                    if (item.Thang == 9)
                    {
                        tc_ChiPhiSuaChua.Thang9 = item.ChiPhi;
                    }
                    if (item.Thang == 10)
                    {
                        tc_ChiPhiSuaChua.Thang10 = item.ChiPhi;
                    }
                    if (item.Thang == 11)
                    {
                        tc_ChiPhiSuaChua.Thang11 = item.ChiPhi;
                    }
                    if (item.Thang == 12)
                    {
                        tc_ChiPhiSuaChua.Thang12 = item.ChiPhi;
                    }
                }
                tc_ChiPhiSuaChua.Tong = tc_ChiPhiSuaChua.Thang1 + tc_ChiPhiSuaChua.Thang2 + tc_ChiPhiSuaChua.Thang3
                    + tc_ChiPhiSuaChua.Thang4 + tc_ChiPhiSuaChua.Thang5 + tc_ChiPhiSuaChua.Thang6 + tc_ChiPhiSuaChua.Thang7
                    + tc_ChiPhiSuaChua.Thang8 + tc_ChiPhiSuaChua.Thang9 + tc_ChiPhiSuaChua.Thang10 + tc_ChiPhiSuaChua.Thang11
                    + tc_ChiPhiSuaChua.Thang12;

                List<SqlParameter> prmgv = new List<SqlParameter>();
                prmgv.Add(new SqlParameter("year", year));
                prmgv.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhMonth_GiaVonBanHangPRC>("exec ReportTaiChinhMonth_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray());
                Report_TaiChinh_TheoThang tc_GiaVonBanHang = new Report_TaiChinh_TheoThang();
                ResertDM(tc_GiaVonBanHang);
                tc_GiaVonBanHang.TaiChinh = "Giá vốn hàng bán (4)";
                Report_TaiChinh_TheoThang tc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanGopVeBanHang.TaiChinh = "Lợi nhuận gộp về bán hàng (7 = 3 - 4 - 5 -6)";
                ResertDM(tc_LoiNhuanGopVeBanHang);

                foreach (var item in tbl_GiaVon)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_GiaVonBanHang.Thang1 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang1 = (tc_DoanhThuThuan.Thang1 - tc_GiaVonBanHang.Thang1
                            - tc_XuatHangSuDungGoiDichVu.Thang1 - tc_ChiPhiSuaChua.Thang1);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_GiaVonBanHang.Thang2 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang2 = (tc_DoanhThuThuan.Thang2 - tc_GiaVonBanHang.Thang2
                            - tc_XuatHangSuDungGoiDichVu.Thang2 - tc_ChiPhiSuaChua.Thang2);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_GiaVonBanHang.Thang3 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang3 = (tc_DoanhThuThuan.Thang3 - tc_GiaVonBanHang.Thang3
                            - tc_XuatHangSuDungGoiDichVu.Thang3 - tc_ChiPhiSuaChua.Thang3);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_GiaVonBanHang.Thang4 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang4 = (tc_DoanhThuThuan.Thang4 - tc_GiaVonBanHang.Thang4
                            - tc_XuatHangSuDungGoiDichVu.Thang4 - tc_ChiPhiSuaChua.Thang4);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_GiaVonBanHang.Thang5 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang5 = (tc_DoanhThuThuan.Thang5 - tc_GiaVonBanHang.Thang5
                            - tc_XuatHangSuDungGoiDichVu.Thang5 - tc_ChiPhiSuaChua.Thang5);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_GiaVonBanHang.Thang6 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang6 = (tc_DoanhThuThuan.Thang6 - tc_GiaVonBanHang.Thang6
                            - tc_XuatHangSuDungGoiDichVu.Thang6 - tc_ChiPhiSuaChua.Thang6);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_GiaVonBanHang.Thang7 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang7 = (tc_DoanhThuThuan.Thang7 - tc_GiaVonBanHang.Thang7
                            - tc_XuatHangSuDungGoiDichVu.Thang7 - tc_ChiPhiSuaChua.Thang7);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_GiaVonBanHang.Thang8 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang8 = (tc_DoanhThuThuan.Thang8 - tc_GiaVonBanHang.Thang8
                            - tc_XuatHangSuDungGoiDichVu.Thang8 - tc_ChiPhiSuaChua.Thang8);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_GiaVonBanHang.Thang9 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang9 = (tc_DoanhThuThuan.Thang9 - tc_GiaVonBanHang.Thang9
                            - tc_XuatHangSuDungGoiDichVu.Thang9 - tc_ChiPhiSuaChua.Thang9);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_GiaVonBanHang.Thang10 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang10 = (tc_DoanhThuThuan.Thang10 - tc_GiaVonBanHang.Thang10
                            - tc_XuatHangSuDungGoiDichVu.Thang10 - tc_ChiPhiSuaChua.Thang10);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_GiaVonBanHang.Thang11 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang11 = (tc_DoanhThuThuan.Thang11 - tc_GiaVonBanHang.Thang11
                            - tc_XuatHangSuDungGoiDichVu.Thang11 - tc_ChiPhiSuaChua.Thang11);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_GiaVonBanHang.Thang12 = (item.TongGiaVonBan - item.TongGiaVonTra);
                        tc_LoiNhuanGopVeBanHang.Thang12 = (tc_DoanhThuThuan.Thang12 - tc_GiaVonBanHang.Thang12
                            - tc_XuatHangSuDungGoiDichVu.Thang12 - tc_ChiPhiSuaChua.Thang12);
                    }
                }
                tc_GiaVonBanHang.Tong = (tc_GiaVonBanHang.Thang1 + tc_GiaVonBanHang.Thang2 + tc_GiaVonBanHang.Thang3 + tc_GiaVonBanHang.Thang4 + tc_GiaVonBanHang.Thang5 + tc_GiaVonBanHang.Thang6 + tc_GiaVonBanHang.Thang7 + tc_GiaVonBanHang.Thang8 + tc_GiaVonBanHang.Thang9 + tc_GiaVonBanHang.Thang10 + tc_GiaVonBanHang.Thang11 + tc_GiaVonBanHang.Thang12);
                tc_LoiNhuanGopVeBanHang.Tong = (tc_LoiNhuanGopVeBanHang.Thang1 + tc_LoiNhuanGopVeBanHang.Thang2 + tc_LoiNhuanGopVeBanHang.Thang3 + tc_LoiNhuanGopVeBanHang.Thang4 + tc_LoiNhuanGopVeBanHang.Thang5 + tc_LoiNhuanGopVeBanHang.Thang6 + tc_LoiNhuanGopVeBanHang.Thang7 + tc_LoiNhuanGopVeBanHang.Thang8 + tc_LoiNhuanGopVeBanHang.Thang9 + tc_LoiNhuanGopVeBanHang.Thang10 + tc_LoiNhuanGopVeBanHang.Thang11 + tc_LoiNhuanGopVeBanHang.Thang12);

                Report_TaiChinh_TheoThang tc_ChiPhi = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhi);
                //tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2 + 7.3)";
                tc_ChiPhi.TaiChinh = "Chi phí (8 = 8.1 + 8.2)";
                Report_TaiChinh_TheoThang tc_PhiGiaoHangTraDoiTac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiGiaoHangTraDoiTac);
                tc_PhiGiaoHangTraDoiTac.TaiChinh = "     Phí giao hàng (trả đối tác) (8.1)";
                Report_TaiChinh_TheoThang tcXuatKhoHangHoa = new Report_TaiChinh_TheoThang();
                tcXuatKhoHangHoa.TaiChinh = "     Xuất kho hàng hóa (8.1)";
                ResertDM(tcXuatKhoHangHoa);
                Report_TaiChinh_TheoThang tc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoThang();
                tc_GiaTriThanhToanDiem.TaiChinh = "     Giá trị thanh toán bằng điểm (8.2)";
                ResertDM(tc_GiaTriThanhToanDiem);
                List<SqlParameter> prmcp = new List<SqlParameter>();
                prmcp.Add(new SqlParameter("year", year));
                prmcp.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhMonth_ChiPhiBanHangPRC>("exec ReportTaiChinhMonth_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray());
                foreach (var item in tbl_ChiPhi)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_ChiPhi.Thang1 = (tc_PhiGiaoHangTraDoiTac.Thang1 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang1 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang1 = (item.DiemThanhToan);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_ChiPhi.Thang2 = (tc_PhiGiaoHangTraDoiTac.Thang2 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang2 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang2 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_ChiPhi.Thang3 = (tc_PhiGiaoHangTraDoiTac.Thang3 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang3 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang3 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_ChiPhi.Thang4 = (tc_PhiGiaoHangTraDoiTac.Thang4 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang4 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang4 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_ChiPhi.Thang5 = (tc_PhiGiaoHangTraDoiTac.Thang5 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang5 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang5 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_ChiPhi.Thang6 = (tc_PhiGiaoHangTraDoiTac.Thang6 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang6 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang6 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_ChiPhi.Thang7 = (tc_PhiGiaoHangTraDoiTac.Thang7 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang7 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang7 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_ChiPhi.Thang8 = (tc_PhiGiaoHangTraDoiTac.Thang8 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang8 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang8 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_ChiPhi.Thang9 = (tc_PhiGiaoHangTraDoiTac.Thang9 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang9 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang9 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_ChiPhi.Thang10 = (tc_PhiGiaoHangTraDoiTac.Thang10 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang10 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang10 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_ChiPhi.Thang11 = (tc_PhiGiaoHangTraDoiTac.Thang11 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang11 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang11 = (item.DiemThanhToan);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_ChiPhi.Thang12 = (tc_PhiGiaoHangTraDoiTac.Thang12 + item.GiaTriHuy + item.DiemThanhToan);
                        tcXuatKhoHangHoa.Thang12 = (item.GiaTriHuy);
                        tc_GiaTriThanhToanDiem.Thang12 = (item.DiemThanhToan);
                    }
                }
                tc_ChiPhi.Tong = (tc_ChiPhi.Thang1 + tc_ChiPhi.Thang2 + tc_ChiPhi.Thang3 + tc_ChiPhi.Thang4 + tc_ChiPhi.Thang5 + tc_ChiPhi.Thang6 + tc_ChiPhi.Thang7 + tc_ChiPhi.Thang8 + tc_ChiPhi.Thang9 + tc_ChiPhi.Thang10 + tc_ChiPhi.Thang11 + tc_ChiPhi.Thang12);
                tc_PhiGiaoHangTraDoiTac.Tong = (tc_PhiGiaoHangTraDoiTac.Thang1 + tc_PhiGiaoHangTraDoiTac.Thang2 + tc_PhiGiaoHangTraDoiTac.Thang3 + tc_PhiGiaoHangTraDoiTac.Thang4 + tc_PhiGiaoHangTraDoiTac.Thang5 + tc_PhiGiaoHangTraDoiTac.Thang6 + tc_PhiGiaoHangTraDoiTac.Thang7 + tc_PhiGiaoHangTraDoiTac.Thang8 + tc_PhiGiaoHangTraDoiTac.Thang9 + tc_PhiGiaoHangTraDoiTac.Thang10 + tc_PhiGiaoHangTraDoiTac.Thang11 + tc_PhiGiaoHangTraDoiTac.Thang12);
                tcXuatKhoHangHoa.Tong = (tcXuatKhoHangHoa.Thang1 + tcXuatKhoHangHoa.Thang2 + tcXuatKhoHangHoa.Thang3 + tcXuatKhoHangHoa.Thang4 + tcXuatKhoHangHoa.Thang5 + tcXuatKhoHangHoa.Thang6 + tcXuatKhoHangHoa.Thang7 + tcXuatKhoHangHoa.Thang8 + tcXuatKhoHangHoa.Thang9 + tcXuatKhoHangHoa.Thang10 + tcXuatKhoHangHoa.Thang11 + tcXuatKhoHangHoa.Thang12);
                tc_GiaTriThanhToanDiem.Tong = (tc_GiaTriThanhToanDiem.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_GiaTriThanhToanDiem.Thang3 + tc_GiaTriThanhToanDiem.Thang4 + tc_GiaTriThanhToanDiem.Thang5 + tc_GiaTriThanhToanDiem.Thang6 + tc_GiaTriThanhToanDiem.Thang7 + tc_GiaTriThanhToanDiem.Thang8 + tc_GiaTriThanhToanDiem.Thang9 + tc_GiaTriThanhToanDiem.Thang10 + tc_GiaTriThanhToanDiem.Thang11 + tc_GiaTriThanhToanDiem.Thang12);

                Report_TaiChinh_TheoThang tc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoThang();
                tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = "Lợi nhuận từ hoạt động kinh doanh (9 = 7 - 8)";
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 = (tc_LoiNhuanGopVeBanHang.Thang1 - tc_ChiPhi.Thang1);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 = (tc_LoiNhuanGopVeBanHang.Thang2 - tc_ChiPhi.Thang2);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 = (tc_LoiNhuanGopVeBanHang.Thang3 - tc_ChiPhi.Thang3);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 = (tc_LoiNhuanGopVeBanHang.Thang4 - tc_ChiPhi.Thang4);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 = (tc_LoiNhuanGopVeBanHang.Thang5 - tc_ChiPhi.Thang5);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 = (tc_LoiNhuanGopVeBanHang.Thang6 - tc_ChiPhi.Thang6);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 = (tc_LoiNhuanGopVeBanHang.Thang7 - tc_ChiPhi.Thang7);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 = (tc_LoiNhuanGopVeBanHang.Thang8 - tc_ChiPhi.Thang8);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 = (tc_LoiNhuanGopVeBanHang.Thang9 - tc_ChiPhi.Thang9);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 = (tc_LoiNhuanGopVeBanHang.Thang10 - tc_ChiPhi.Thang10);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 = (tc_LoiNhuanGopVeBanHang.Thang11 - tc_ChiPhi.Thang11);
                tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 = (tc_LoiNhuanGopVeBanHang.Thang12 - tc_ChiPhi.Thang12);
                tc_LoiNhuanTuHoatDongKinhDoanh.Tong = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang12);

                Report_TaiChinh_TheoThang tc_ThuNhapKhac8 = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac8);
                tc_ThuNhapKhac8.TaiChinh = "Thu nhập khác (10 = 10.1 + 10.2)";
                Report_TaiChinh_TheoThang tc_PhiTraHang = new Report_TaiChinh_TheoThang();
                ResertDM(tc_PhiTraHang);
                tc_PhiTraHang.TaiChinh = "     Phí trả hàng (10.1)";
                //Report_TaiChinh_TheoThang DM15 = new Report_TaiChinh_TheoThang();
                //ResertDM(DM15);
                //DM15.TaiChinh = "     Khách hàng thanh toán";
                Report_TaiChinh_TheoThang tc_ThuNhapKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ThuNhapKhac);
                tc_ThuNhapKhac.TaiChinh = "     Thu nhập khác (10.2)";
                Report_TaiChinh_TheoThang tc_ChiPhiKhac = new Report_TaiChinh_TheoThang();
                ResertDM(tc_ChiPhiKhac);
                tc_ChiPhiKhac.TaiChinh = "Chi phí khác (11)";
                Report_TaiChinh_TheoThang tc_LoiNhuanThuan = new Report_TaiChinh_TheoThang();
                ResertDM(tc_LoiNhuanThuan);
                tc_LoiNhuanThuan.TaiChinh = "Lợi nhuận thuần (12 = (9 + 10) - 11)";
                List<SqlParameter> prmsq = new List<SqlParameter>();
                prmsq.Add(new SqlParameter("year", year));
                prmsq.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_sq = _db.Database.SqlQuery<ReportTaiChinhMonth_SoQuyBanHangPRC>("exec ReportTaiChinhMonth_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray());

                foreach (var item in tbl_sq)
                {
                    if (item.ThangLapHoaDon == 1)
                    {
                        tc_PhiTraHang.Thang1 = (item.PhiTraHangNhap);
                        //DM15.Thang1 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang1 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang1 = (item.ChiPhiKhac);
                    }
                    else if (item.ThangLapHoaDon == 2)
                    {
                        tc_PhiTraHang.Thang2 = (item.PhiTraHangNhap);
                        //DM15.Thang2 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang2 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang2 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 3)
                    {
                        tc_PhiTraHang.Thang3 = (item.PhiTraHangNhap);
                        //DM15.Thang3 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang3 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang3 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 4)
                    {
                        tc_PhiTraHang.Thang4 = (item.PhiTraHangNhap);
                        //DM15.Thang4 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang4 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang4 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 5)
                    {
                        tc_PhiTraHang.Thang5 = (item.PhiTraHangNhap);
                        //DM15.Thang5 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang5 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang5 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 6)
                    {
                        tc_PhiTraHang.Thang6 = (item.PhiTraHangNhap);
                        //DM15.Thang6 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang6 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang6 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 7)
                    {
                        tc_PhiTraHang.Thang7 = (item.PhiTraHangNhap);
                        //DM15.Thang7 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang7 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang7 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 8)
                    {
                        tc_PhiTraHang.Thang8 = (item.PhiTraHangNhap);
                        //DM15.Thang8 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang8 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang8 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 9)
                    {
                        tc_PhiTraHang.Thang9 = (item.PhiTraHangNhap);
                        //DM15.Thang9 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang9 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang9 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 10)
                    {
                        tc_PhiTraHang.Thang10 = (item.PhiTraHangNhap);
                        //DM15.Thang10 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang10 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang10 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 11)
                    {
                        tc_PhiTraHang.Thang11 = (item.PhiTraHangNhap);
                        //DM15.Thang11 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang11 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang11 = (item.ChiPhiKhac);
                    }

                    else if (item.ThangLapHoaDon == 12)
                    {
                        tc_PhiTraHang.Thang12 = (item.PhiTraHangNhap);
                        //DM15.Thang12 = (item.KhachThanhToan);
                        tc_ThuNhapKhac.Thang12 = (item.ThuNhapKhac);
                        tc_ChiPhiKhac.Thang12 = (item.ChiPhiKhac);
                    }
                }

                tc_ThuNhapKhac8.Thang1 = (tc_PhiTraHang.Thang1 + tc_ThuNhapKhac.Thang1);
                tc_ThuNhapKhac8.Thang2 = (tc_PhiTraHang.Thang2 + tc_ThuNhapKhac.Thang2);
                tc_ThuNhapKhac8.Thang3 = (tc_PhiTraHang.Thang3 + tc_ThuNhapKhac.Thang3);
                tc_ThuNhapKhac8.Thang4 = (tc_PhiTraHang.Thang4 + tc_ThuNhapKhac.Thang4);
                tc_ThuNhapKhac8.Thang5 = (tc_PhiTraHang.Thang5 + tc_ThuNhapKhac.Thang5);
                tc_ThuNhapKhac8.Thang6 = (tc_PhiTraHang.Thang6 + tc_ThuNhapKhac.Thang6);
                tc_ThuNhapKhac8.Thang7 = (tc_PhiTraHang.Thang7 + tc_ThuNhapKhac.Thang7);
                tc_ThuNhapKhac8.Thang8 = (tc_PhiTraHang.Thang8 + tc_ThuNhapKhac.Thang8);
                tc_ThuNhapKhac8.Thang9 = (tc_PhiTraHang.Thang9 + tc_ThuNhapKhac.Thang9);
                tc_ThuNhapKhac8.Thang10 = (tc_PhiTraHang.Thang10 + tc_ThuNhapKhac.Thang10);
                tc_ThuNhapKhac8.Thang11 = (tc_PhiTraHang.Thang11 + tc_ThuNhapKhac.Thang11);
                tc_ThuNhapKhac8.Thang12 = (tc_PhiTraHang.Thang12 + tc_ThuNhapKhac.Thang12);
                tc_ThuNhapKhac8.Tong = (tc_ThuNhapKhac8.Thang1 + tc_ThuNhapKhac8.Thang2 + tc_ThuNhapKhac8.Thang3 + tc_ThuNhapKhac8.Thang4 + tc_ThuNhapKhac8.Thang5 + tc_ThuNhapKhac8.Thang6 + tc_ThuNhapKhac8.Thang7 + tc_ThuNhapKhac8.Thang8 + tc_ThuNhapKhac8.Thang9 + tc_ThuNhapKhac8.Thang10 + tc_ThuNhapKhac8.Thang11 + tc_ThuNhapKhac8.Thang12);
                tc_PhiTraHang.Tong = (tc_PhiTraHang.Thang1 + tc_PhiTraHang.Thang2 + tc_PhiTraHang.Thang3 + tc_PhiTraHang.Thang4 + tc_PhiTraHang.Thang5 + tc_PhiTraHang.Thang6 + tc_PhiTraHang.Thang7 + tc_PhiTraHang.Thang8 + tc_PhiTraHang.Thang9 + tc_PhiTraHang.Thang10 + tc_PhiTraHang.Thang11 + tc_PhiTraHang.Thang12);
                //DM15.Tong = (DM15.Thang1 + DM15.Thang2 + DM15.Thang3 + DM15.Thang4 + DM15.Thang5 + DM15.Thang6 + DM15.Thang7 + DM15.Thang8 + DM15.Thang9 + DM15.Thang10 + DM15.Thang11 + DM15.Thang12);
                tc_ThuNhapKhac.Tong = (tc_ThuNhapKhac.Thang1 + tc_ThuNhapKhac.Thang2 + tc_ThuNhapKhac.Thang3 + tc_ThuNhapKhac.Thang4 + tc_ThuNhapKhac.Thang5 + tc_ThuNhapKhac.Thang6 + tc_ThuNhapKhac.Thang7 + tc_ThuNhapKhac.Thang8 + tc_ThuNhapKhac.Thang9 + tc_ThuNhapKhac.Thang10 + tc_ThuNhapKhac.Thang11 + tc_ThuNhapKhac.Thang12);
                tc_ChiPhiKhac.Tong = (tc_ChiPhiKhac.Thang1 + tc_ChiPhiKhac.Thang2 + tc_ChiPhiKhac.Thang3 + tc_ChiPhiKhac.Thang4 + tc_ChiPhiKhac.Thang5 + tc_ChiPhiKhac.Thang6 + tc_ChiPhiKhac.Thang7 + tc_ChiPhiKhac.Thang8 + tc_ChiPhiKhac.Thang9 + tc_ChiPhiKhac.Thang10 + tc_ChiPhiKhac.Thang11 + tc_ChiPhiKhac.Thang12);
                tc_LoiNhuanThuan.Thang1 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_ThuNhapKhac8.Thang1 - tc_ChiPhiKhac.Thang1);
                tc_LoiNhuanThuan.Thang2 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_ThuNhapKhac8.Thang2 - tc_ChiPhiKhac.Thang2);
                tc_LoiNhuanThuan.Thang3 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang3 + tc_ThuNhapKhac8.Thang3 - tc_ChiPhiKhac.Thang3);
                tc_LoiNhuanThuan.Thang4 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_ThuNhapKhac8.Thang4 - tc_ChiPhiKhac.Thang4);
                tc_LoiNhuanThuan.Thang5 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_ThuNhapKhac8.Thang5 - tc_ChiPhiKhac.Thang5);
                tc_LoiNhuanThuan.Thang6 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang6 + tc_ThuNhapKhac8.Thang6 - tc_ChiPhiKhac.Thang6);
                tc_LoiNhuanThuan.Thang7 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_ThuNhapKhac8.Thang7 - tc_ChiPhiKhac.Thang7);
                tc_LoiNhuanThuan.Thang8 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_ThuNhapKhac8.Thang8 - tc_ChiPhiKhac.Thang8);
                tc_LoiNhuanThuan.Thang9 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang9 + tc_ThuNhapKhac8.Thang9 - tc_ChiPhiKhac.Thang9);
                tc_LoiNhuanThuan.Thang10 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_ThuNhapKhac8.Thang10 - tc_ChiPhiKhac.Thang10);
                tc_LoiNhuanThuan.Thang11 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang11);
                tc_LoiNhuanThuan.Thang12 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang12 + tc_ThuNhapKhac8.Thang11 - tc_ChiPhiKhac.Thang12);
                tc_LoiNhuanThuan.Tong = (tc_LoiNhuanThuan.Thang1 + tc_LoiNhuanThuan.Thang2 + tc_LoiNhuanThuan.Thang3 + tc_LoiNhuanThuan.Thang4 + tc_LoiNhuanThuan.Thang5 + tc_LoiNhuanThuan.Thang6 + tc_LoiNhuanThuan.Thang7 + tc_LoiNhuanThuan.Thang8 + tc_LoiNhuanThuan.Thang9 + tc_LoiNhuanThuan.Thang10 + tc_LoiNhuanThuan.Thang11 + tc_LoiNhuanThuan.Thang12);
                Report_TaiChinh_TheoQuy qtc_DoanhThuBanHang = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiamTruDoanhThu = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiamGiaHoaDon = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ChiPhiSuaChua = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_DoanhThuThuan = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiaVonBanHang = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ChiPhi = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_PhiGiaoHangTraDoiTac = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtcXuatKhoHangHoa = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ThuNhapKhac8 = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_PhiTraHang = new Report_TaiChinh_TheoQuy();
                //Report_TaiChinh_TheoQuy DQ15 = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ThuNhapKhac = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_ChiPhiKhac = new Report_TaiChinh_TheoQuy();
                Report_TaiChinh_TheoQuy qtc_LoiNhuanThuan = new Report_TaiChinh_TheoQuy();
                qtc_DoanhThuBanHang.TaiChinh = tc_DoanhThuBanHang.TaiChinh;
                qtc_DoanhThuBanHang.Quy1 = (tc_DoanhThuBanHang.Thang1 + tc_DoanhThuBanHang.Thang2 + tc_DoanhThuBanHang.Thang3);
                qtc_DoanhThuBanHang.Quy2 = (tc_DoanhThuBanHang.Thang4 + tc_DoanhThuBanHang.Thang5 + tc_DoanhThuBanHang.Thang6);
                qtc_DoanhThuBanHang.Quy3 = (tc_DoanhThuBanHang.Thang7 + tc_DoanhThuBanHang.Thang8 + tc_DoanhThuBanHang.Thang9);
                qtc_DoanhThuBanHang.Quy4 = (tc_DoanhThuBanHang.Thang10 + tc_DoanhThuBanHang.Thang11 + tc_DoanhThuBanHang.Thang12);
                qtc_DoanhThuBanHang.Tong = tc_DoanhThuBanHang.Tong;

                qtc_GiamTruDoanhThu.TaiChinh = tc_GiamTruDoanhThu.TaiChinh;
                qtc_GiamTruDoanhThu.Quy1 = (tc_GiamTruDoanhThu.Thang1 + tc_GiamTruDoanhThu.Thang2 + tc_GiamTruDoanhThu.Thang3);
                qtc_GiamTruDoanhThu.Quy2 = (tc_GiamTruDoanhThu.Thang4 + tc_GiamTruDoanhThu.Thang5 + tc_GiamTruDoanhThu.Thang6);
                qtc_GiamTruDoanhThu.Quy3 = (tc_GiamTruDoanhThu.Thang7 + tc_GiamTruDoanhThu.Thang8 + tc_GiamTruDoanhThu.Thang9);
                qtc_GiamTruDoanhThu.Quy4 = (tc_GiamTruDoanhThu.Thang10 + tc_GiamTruDoanhThu.Thang11 + tc_GiamTruDoanhThu.Thang12);
                qtc_GiamTruDoanhThu.Tong = tc_GiamTruDoanhThu.Tong;

                qtc_GiamGiaHoaDon.TaiChinh = tc_GiamGiaHoaDon.TaiChinh;
                qtc_GiamGiaHoaDon.Quy1 = (tc_GiamGiaHoaDon.Thang1 + tc_GiamGiaHoaDon.Thang2 + tc_GiamGiaHoaDon.Thang3);
                qtc_GiamGiaHoaDon.Quy2 = (tc_GiamGiaHoaDon.Thang4 + tc_GiamGiaHoaDon.Thang5 + tc_GiamGiaHoaDon.Thang6);
                qtc_GiamGiaHoaDon.Quy3 = (tc_GiamGiaHoaDon.Thang7 + tc_GiamGiaHoaDon.Thang8 + tc_GiamGiaHoaDon.Thang9);
                qtc_GiamGiaHoaDon.Quy4 = (tc_GiamGiaHoaDon.Thang10 + tc_GiamGiaHoaDon.Thang11 + tc_GiamGiaHoaDon.Thang12);
                qtc_GiamGiaHoaDon.Tong = tc_GiamGiaHoaDon.Tong;

                qtc_GiaTriHangBanBiTraLai.TaiChinh = tc_GiaTriHangBanBiTraLai.TaiChinh;
                qtc_GiaTriHangBanBiTraLai.Quy1 = (tc_GiaTriHangBanBiTraLai.Thang1 + tc_GiaTriHangBanBiTraLai.Thang2 + tc_GiaTriHangBanBiTraLai.Thang3);
                qtc_GiaTriHangBanBiTraLai.Quy2 = (tc_GiaTriHangBanBiTraLai.Thang4 + tc_GiaTriHangBanBiTraLai.Thang5 + tc_GiaTriHangBanBiTraLai.Thang6);
                qtc_GiaTriHangBanBiTraLai.Quy3 = (tc_GiaTriHangBanBiTraLai.Thang7 + tc_GiaTriHangBanBiTraLai.Thang8 + tc_GiaTriHangBanBiTraLai.Thang9);
                qtc_GiaTriHangBanBiTraLai.Quy4 = (tc_GiaTriHangBanBiTraLai.Thang10 + tc_GiaTriHangBanBiTraLai.Thang11 + tc_GiaTriHangBanBiTraLai.Thang12);
                qtc_GiaTriHangBanBiTraLai.Tong = tc_GiaTriHangBanBiTraLai.Tong;

                qtc_XuatHangSuDungGoiDichVu.TaiChinh = tc_XuatHangSuDungGoiDichVu.TaiChinh;
                qtc_XuatHangSuDungGoiDichVu.Quy1 = (tc_XuatHangSuDungGoiDichVu.Thang1 + tc_XuatHangSuDungGoiDichVu.Thang2 + tc_XuatHangSuDungGoiDichVu.Thang3);
                qtc_XuatHangSuDungGoiDichVu.Quy2 = (tc_XuatHangSuDungGoiDichVu.Thang4 + tc_XuatHangSuDungGoiDichVu.Thang5 + tc_XuatHangSuDungGoiDichVu.Thang6);
                qtc_XuatHangSuDungGoiDichVu.Quy3 = (tc_XuatHangSuDungGoiDichVu.Thang7 + tc_XuatHangSuDungGoiDichVu.Thang8 + tc_XuatHangSuDungGoiDichVu.Thang9);
                qtc_XuatHangSuDungGoiDichVu.Quy4 = (tc_XuatHangSuDungGoiDichVu.Thang10 + tc_XuatHangSuDungGoiDichVu.Thang11 + tc_XuatHangSuDungGoiDichVu.Thang12);
                qtc_XuatHangSuDungGoiDichVu.Tong = tc_XuatHangSuDungGoiDichVu.Tong;

                qtc_ChiPhiSuaChua.TaiChinh = tc_ChiPhiSuaChua.TaiChinh;
                qtc_ChiPhiSuaChua.Quy1 = (tc_ChiPhiSuaChua.Thang1 + tc_ChiPhiSuaChua.Thang2 + tc_ChiPhiSuaChua.Thang3);
                qtc_ChiPhiSuaChua.Quy2 = (tc_ChiPhiSuaChua.Thang4 + tc_ChiPhiSuaChua.Thang5 + tc_ChiPhiSuaChua.Thang6);
                qtc_ChiPhiSuaChua.Quy3 = (tc_ChiPhiSuaChua.Thang7 + tc_ChiPhiSuaChua.Thang8 + tc_ChiPhiSuaChua.Thang9);
                qtc_ChiPhiSuaChua.Quy4 = (tc_ChiPhiSuaChua.Thang10 + tc_ChiPhiSuaChua.Thang11 + tc_ChiPhiSuaChua.Thang12);
                qtc_ChiPhiSuaChua.Tong = tc_ChiPhiSuaChua.Tong;

                qtc_DoanhThuThuan.TaiChinh = tc_DoanhThuThuan.TaiChinh;
                qtc_DoanhThuThuan.Quy1 = (tc_DoanhThuThuan.Thang1 + tc_DoanhThuThuan.Thang2 + tc_DoanhThuThuan.Thang3);
                qtc_DoanhThuThuan.Quy2 = (tc_DoanhThuThuan.Thang4 + tc_DoanhThuThuan.Thang5 + tc_DoanhThuThuan.Thang6);
                qtc_DoanhThuThuan.Quy3 = (tc_DoanhThuThuan.Thang7 + tc_DoanhThuThuan.Thang8 + tc_DoanhThuThuan.Thang9);
                qtc_DoanhThuThuan.Quy4 = (tc_DoanhThuThuan.Thang10 + tc_DoanhThuThuan.Thang11 + tc_DoanhThuThuan.Thang12);
                qtc_DoanhThuThuan.Tong = tc_DoanhThuThuan.Tong;

                qtc_GiaVonBanHang.TaiChinh = tc_GiaVonBanHang.TaiChinh;
                qtc_GiaVonBanHang.Quy1 = (tc_GiaVonBanHang.Thang1 + tc_GiaVonBanHang.Thang2 + tc_GiaVonBanHang.Thang3);
                qtc_GiaVonBanHang.Quy2 = (tc_GiaVonBanHang.Thang4 + tc_GiaVonBanHang.Thang5 + tc_GiaVonBanHang.Thang6);
                qtc_GiaVonBanHang.Quy3 = (tc_GiaVonBanHang.Thang7 + tc_GiaVonBanHang.Thang8 + tc_GiaVonBanHang.Thang9);
                qtc_GiaVonBanHang.Quy4 = (tc_GiaVonBanHang.Thang10 + tc_GiaVonBanHang.Thang11 + tc_GiaVonBanHang.Thang12);
                qtc_GiaVonBanHang.Tong = tc_GiaVonBanHang.Tong;

                qtc_LoiNhuanGopVeBanHang.TaiChinh = tc_LoiNhuanGopVeBanHang.TaiChinh;
                qtc_LoiNhuanGopVeBanHang.Quy1 = (tc_LoiNhuanGopVeBanHang.Thang1 + tc_LoiNhuanGopVeBanHang.Thang2 + tc_LoiNhuanGopVeBanHang.Thang3);
                qtc_LoiNhuanGopVeBanHang.Quy2 = (tc_LoiNhuanGopVeBanHang.Thang4 + tc_LoiNhuanGopVeBanHang.Thang5 + tc_LoiNhuanGopVeBanHang.Thang6);
                qtc_LoiNhuanGopVeBanHang.Quy3 = (tc_LoiNhuanGopVeBanHang.Thang7 + tc_LoiNhuanGopVeBanHang.Thang8 + tc_LoiNhuanGopVeBanHang.Thang9);
                qtc_LoiNhuanGopVeBanHang.Quy4 = (tc_LoiNhuanGopVeBanHang.Thang10 + tc_LoiNhuanGopVeBanHang.Thang11 + tc_LoiNhuanGopVeBanHang.Thang12);
                qtc_LoiNhuanGopVeBanHang.Tong = tc_LoiNhuanGopVeBanHang.Tong;

                qtc_ChiPhi.TaiChinh = tc_ChiPhi.TaiChinh;
                qtc_ChiPhi.Quy1 = (tc_ChiPhi.Thang1 + tc_ChiPhi.Thang2 + tc_ChiPhi.Thang3);
                qtc_ChiPhi.Quy2 = (tc_ChiPhi.Thang4 + tc_ChiPhi.Thang5 + tc_ChiPhi.Thang6);
                qtc_ChiPhi.Quy3 = (tc_ChiPhi.Thang7 + tc_ChiPhi.Thang8 + tc_ChiPhi.Thang9);
                qtc_ChiPhi.Quy4 = (tc_ChiPhi.Thang10 + tc_ChiPhi.Thang11 + tc_ChiPhi.Thang12);
                qtc_ChiPhi.Tong = tc_ChiPhi.Tong;

                //qtc_PhiGiaoHangTraDoiTac.TaiChinh = tc_PhiGiaoHangTraDoiTac.TaiChinh;
                //qtc_PhiGiaoHangTraDoiTac.Quy1 = (tc_PhiGiaoHangTraDoiTac.Thang1 + tc_PhiGiaoHangTraDoiTac.Thang2 + tc_PhiGiaoHangTraDoiTac.Thang3);
                //qtc_PhiGiaoHangTraDoiTac.Quy2 = (tc_PhiGiaoHangTraDoiTac.Thang4 + tc_PhiGiaoHangTraDoiTac.Thang5 + tc_PhiGiaoHangTraDoiTac.Thang6);
                //qtc_PhiGiaoHangTraDoiTac.Quy3 = (tc_PhiGiaoHangTraDoiTac.Thang7 + tc_PhiGiaoHangTraDoiTac.Thang8 + tc_PhiGiaoHangTraDoiTac.Thang9);
                //qtc_PhiGiaoHangTraDoiTac.Quy4 = (tc_PhiGiaoHangTraDoiTac.Thang10 + tc_PhiGiaoHangTraDoiTac.Thang11 + tc_PhiGiaoHangTraDoiTac.Thang12);
                //qtc_PhiGiaoHangTraDoiTac.Tong = tc_PhiGiaoHangTraDoiTac.Tong;

                qtcXuatKhoHangHoa.TaiChinh = tcXuatKhoHangHoa.TaiChinh;
                qtcXuatKhoHangHoa.Quy1 = (tcXuatKhoHangHoa.Thang1 + tcXuatKhoHangHoa.Thang2 + tcXuatKhoHangHoa.Thang3);
                qtcXuatKhoHangHoa.Quy2 = (tcXuatKhoHangHoa.Thang4 + tcXuatKhoHangHoa.Thang5 + tcXuatKhoHangHoa.Thang6);
                qtcXuatKhoHangHoa.Quy3 = (tcXuatKhoHangHoa.Thang7 + tcXuatKhoHangHoa.Thang8 + tcXuatKhoHangHoa.Thang9);
                qtcXuatKhoHangHoa.Quy4 = (tcXuatKhoHangHoa.Thang10 + tcXuatKhoHangHoa.Thang11 + tcXuatKhoHangHoa.Thang12);
                qtcXuatKhoHangHoa.Tong = tcXuatKhoHangHoa.Tong;

                qtc_GiaTriThanhToanDiem.TaiChinh = tc_GiaTriThanhToanDiem.TaiChinh;
                qtc_GiaTriThanhToanDiem.Quy1 = (tc_GiaTriThanhToanDiem.Thang1 + tc_GiaTriThanhToanDiem.Thang2 + tc_GiaTriThanhToanDiem.Thang3);
                qtc_GiaTriThanhToanDiem.Quy2 = (tc_GiaTriThanhToanDiem.Thang4 + tc_GiaTriThanhToanDiem.Thang5 + tc_GiaTriThanhToanDiem.Thang6);
                qtc_GiaTriThanhToanDiem.Quy3 = (tc_GiaTriThanhToanDiem.Thang7 + tc_GiaTriThanhToanDiem.Thang8 + tc_GiaTriThanhToanDiem.Thang9);
                qtc_GiaTriThanhToanDiem.Quy4 = (tc_GiaTriThanhToanDiem.Thang10 + tc_GiaTriThanhToanDiem.Thang11 + tc_GiaTriThanhToanDiem.Thang12);
                qtc_GiaTriThanhToanDiem.Tong = tc_GiaTriThanhToanDiem.Tong;

                qtc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh;
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy1 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang1 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang2 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang3);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy2 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang4 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang5 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang6);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy3 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang7 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang8 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang9);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Quy4 = (tc_LoiNhuanTuHoatDongKinhDoanh.Thang10 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang11 + tc_LoiNhuanTuHoatDongKinhDoanh.Thang12);
                qtc_LoiNhuanTuHoatDongKinhDoanh.Tong = tc_LoiNhuanTuHoatDongKinhDoanh.Tong;

                qtc_ThuNhapKhac8.TaiChinh = tc_ThuNhapKhac8.TaiChinh;
                qtc_ThuNhapKhac8.Quy1 = (tc_ThuNhapKhac8.Thang1 + tc_ThuNhapKhac8.Thang2 + tc_ThuNhapKhac8.Thang3);
                qtc_ThuNhapKhac8.Quy2 = (tc_ThuNhapKhac8.Thang4 + tc_ThuNhapKhac8.Thang5 + tc_ThuNhapKhac8.Thang6);
                qtc_ThuNhapKhac8.Quy3 = (tc_ThuNhapKhac8.Thang7 + tc_ThuNhapKhac8.Thang8 + tc_ThuNhapKhac8.Thang9);
                qtc_ThuNhapKhac8.Quy4 = (tc_ThuNhapKhac8.Thang10 + tc_ThuNhapKhac8.Thang11 + tc_ThuNhapKhac8.Thang12);
                qtc_ThuNhapKhac8.Tong = tc_ThuNhapKhac8.Tong;

                qtc_PhiTraHang.TaiChinh = tc_PhiTraHang.TaiChinh;
                qtc_PhiTraHang.Quy1 = (tc_PhiTraHang.Thang1 + tc_PhiTraHang.Thang2 + tc_PhiTraHang.Thang3);
                qtc_PhiTraHang.Quy2 = (tc_PhiTraHang.Thang4 + tc_PhiTraHang.Thang5 + tc_PhiTraHang.Thang6);
                qtc_PhiTraHang.Quy3 = (tc_PhiTraHang.Thang7 + tc_PhiTraHang.Thang8 + tc_PhiTraHang.Thang9);
                qtc_PhiTraHang.Quy4 = (tc_PhiTraHang.Thang10 + tc_PhiTraHang.Thang11 + tc_PhiTraHang.Thang12);
                qtc_PhiTraHang.Tong = tc_PhiTraHang.Tong;

                //DQ15.TaiChinh = DM15.TaiChinh;
                //DQ15.Quy1 = (DM15.Thang1 + DM15.Thang2 + DM15.Thang3);
                //DQ15.Quy2 = (DM15.Thang4 + DM15.Thang5 + DM15.Thang6);
                //DQ15.Quy3 = (DM15.Thang7 + DM15.Thang8 + DM15.Thang9);
                //DQ15.Quy4 = (DM15.Thang10 + DM15.Thang11 + DM15.Thang12);
                //DQ15.Tong = DM15.Tong;

                qtc_ThuNhapKhac.TaiChinh = tc_ThuNhapKhac.TaiChinh;
                qtc_ThuNhapKhac.Quy1 = (tc_ThuNhapKhac.Thang1 + tc_ThuNhapKhac.Thang2 + tc_ThuNhapKhac.Thang3);
                qtc_ThuNhapKhac.Quy2 = (tc_ThuNhapKhac.Thang4 + tc_ThuNhapKhac.Thang5 + tc_ThuNhapKhac.Thang6);
                qtc_ThuNhapKhac.Quy3 = (tc_ThuNhapKhac.Thang7 + tc_ThuNhapKhac.Thang8 + tc_ThuNhapKhac.Thang9);
                qtc_ThuNhapKhac.Quy4 = (tc_ThuNhapKhac.Thang10 + tc_ThuNhapKhac.Thang11 + tc_ThuNhapKhac.Thang12);
                qtc_ThuNhapKhac.Tong = tc_ThuNhapKhac.Tong;

                qtc_ChiPhiKhac.TaiChinh = tc_ChiPhiKhac.TaiChinh;
                qtc_ChiPhiKhac.Quy1 = (tc_ChiPhiKhac.Thang1 + tc_ChiPhiKhac.Thang2 + tc_ChiPhiKhac.Thang3);
                qtc_ChiPhiKhac.Quy2 = (tc_ChiPhiKhac.Thang4 + tc_ChiPhiKhac.Thang5 + tc_ChiPhiKhac.Thang6);
                qtc_ChiPhiKhac.Quy3 = (tc_ChiPhiKhac.Thang7 + tc_ChiPhiKhac.Thang8 + tc_ChiPhiKhac.Thang9);
                qtc_ChiPhiKhac.Quy4 = (tc_ChiPhiKhac.Thang10 + tc_ChiPhiKhac.Thang11 + tc_ChiPhiKhac.Thang12);
                qtc_ChiPhiKhac.Tong = tc_ChiPhiKhac.Tong;

                qtc_LoiNhuanThuan.TaiChinh = tc_LoiNhuanThuan.TaiChinh;
                qtc_LoiNhuanThuan.Quy1 = (tc_LoiNhuanThuan.Thang1 + tc_LoiNhuanThuan.Thang2 + tc_LoiNhuanThuan.Thang3);
                qtc_LoiNhuanThuan.Quy2 = (tc_LoiNhuanThuan.Thang4 + tc_LoiNhuanThuan.Thang5 + tc_LoiNhuanThuan.Thang6);
                qtc_LoiNhuanThuan.Quy3 = (tc_LoiNhuanThuan.Thang7 + tc_LoiNhuanThuan.Thang8 + tc_LoiNhuanThuan.Thang9);
                qtc_LoiNhuanThuan.Quy4 = (tc_LoiNhuanThuan.Thang10 + tc_LoiNhuanThuan.Thang11 + tc_LoiNhuanThuan.Thang12);
                qtc_LoiNhuanThuan.Tong = tc_LoiNhuanThuan.Tong;

                lst.Add(qtc_DoanhThuBanHang);
                lst.Add(qtc_GiamTruDoanhThu);
                lst.Add(qtc_GiamGiaHoaDon);
                lst.Add(qtc_GiaTriHangBanBiTraLai);
                lst.Add(qtc_DoanhThuThuan);
                lst.Add(qtc_GiaVonBanHang);
                lst.Add(qtc_XuatHangSuDungGoiDichVu);
                lst.Add(qtc_ChiPhiSuaChua);
                lst.Add(qtc_LoiNhuanGopVeBanHang);
                lst.Add(qtc_ChiPhi);
                //lst.Add(qtc_PhiGiaoHangTraDoiTac);
                lst.Add(qtcXuatKhoHangHoa);
                lst.Add(qtc_GiaTriThanhToanDiem);
                lst.Add(qtc_LoiNhuanTuHoatDongKinhDoanh);
                lst.Add(qtc_ThuNhapKhac8);
                lst.Add(qtc_PhiTraHang);
                //lst.Add(DQ15);
                lst.Add(qtc_ThuNhapKhac);
                lst.Add(qtc_ChiPhiKhac);
                lst.Add(qtc_LoiNhuanThuan);
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - getListTaiChinh_TheoQuy: " + ex.Message);
                return new List<Report_TaiChinh_TheoQuy>();
            }
        }

        public List<Report_TaiChinh_TheoNam> getListTaiChinh_TheoNam(int year, string ID_ChiNhanh)
        {
            try
            {
                List<Report_TaiChinh_TheoNam> lst = new List<Report_TaiChinh_TheoNam>();
                Report_TaiChinh_TheoNam tc_DoanhThuBanHang = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_GiamTruDoanhThu = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_GiamGiaHoaDon = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_DoanhThuThuan = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoNam();

                tc_DoanhThuBanHang.TaiChinh = "Doanh thu bán hàng (1)";
                tc_GiamTruDoanhThu.TaiChinh = "Giảm trừ doanh thu (2 = 2.1 + 2.2)";
                tc_GiamGiaHoaDon.TaiChinh = "     Giảm giá hóa đơn (2.1)";
                tc_GiaTriHangBanBiTraLai.TaiChinh = "     Giá trị hàng bán bị trả lại (2.2)";
                tc_XuatHangSuDungGoiDichVu.TaiChinh = "Xuất hàng sử dụng gói dịch vụ (5)";
                tc_DoanhThuThuan.TaiChinh = "Doanh thu thuần (3 = 1 - 2)";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("year", year));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhYear_DoanhThuBanHangPRC>("exec ReportTaiChinhYear_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray());
                ResertDN(tc_DoanhThuBanHang);
                ResertDN(tc_GiamTruDoanhThu);
                ResertDN(tc_GiamGiaHoaDon);
                ResertDN(tc_GiaTriHangBanBiTraLai);
                ResertDN(tc_DoanhThuThuan);
                foreach (var item in tbl_DoanhThuBanHang)
                {
                    tc_DoanhThuBanHang.Nam = (item.DoanhThu);
                    tc_GiamTruDoanhThu.Nam = (item.GiaTriTra + item.GiamGiaHD);
                    tc_GiamGiaHoaDon.Nam = (item.GiamGiaHD);
                    tc_GiaTriHangBanBiTraLai.Nam = (item.GiaTriTra);
                    tc_XuatHangSuDungGoiDichVu.Nam = (item.GiaVonGDV);
                    tc_DoanhThuThuan.Nam = (tc_DoanhThuBanHang.Nam - tc_GiamTruDoanhThu.Nam);
                }
                tc_DoanhThuBanHang.Tong = (tc_DoanhThuBanHang.Nam);
                tc_GiamTruDoanhThu.Tong = (tc_GiamTruDoanhThu.Nam);
                tc_GiamGiaHoaDon.Tong = (tc_GiamGiaHoaDon.Nam);
                tc_GiaTriHangBanBiTraLai.Tong = (tc_GiaTriHangBanBiTraLai.Nam);
                tc_XuatHangSuDungGoiDichVu.Tong = (tc_XuatHangSuDungGoiDichVu.Nam);
                tc_DoanhThuThuan.Tong = (tc_DoanhThuThuan.Nam);
                lst.Add(tc_DoanhThuBanHang);
                lst.Add(tc_GiamTruDoanhThu);
                lst.Add(tc_GiamGiaHoaDon);
                lst.Add(tc_GiaTriHangBanBiTraLai);
                lst.Add(tc_DoanhThuThuan);
                //DM    
                List<SqlParameter> prmgv = new List<SqlParameter>();
                prmgv.Add(new SqlParameter("year", year));
                prmgv.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhYear_GiaVonBanHangPRC>("exec ReportTaiChinhYear_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray());
                Report_TaiChinh_TheoNam tc_GiaVonHangBan = new Report_TaiChinh_TheoNam();
                ResertDN(tc_GiaVonHangBan);
                tc_GiaVonHangBan.TaiChinh = "Giá vốn hàng bán (4)";
                Report_TaiChinh_TheoNam tc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoNam();
                tc_LoiNhuanGopVeBanHang.TaiChinh = "Lợi nhuận gộp về bán hàng (6 = 3 - 4 - 5)";
                ResertDN(tc_LoiNhuanGopVeBanHang);

                foreach (var item in tbl_GiaVon)
                {
                    tc_GiaVonHangBan.Nam = (item.TongGiaVonBan - item.TongGiaVonTra);
                    tc_LoiNhuanGopVeBanHang.Nam = (tc_DoanhThuThuan.Nam - tc_GiaVonHangBan.Nam - tc_XuatHangSuDungGoiDichVu.Nam);
                }
                tc_GiaVonHangBan.Tong = (tc_GiaVonHangBan.Nam);
                tc_LoiNhuanGopVeBanHang.Tong = (tc_LoiNhuanGopVeBanHang.Nam);
                lst.Add(tc_GiaVonHangBan);
                lst.Add(tc_XuatHangSuDungGoiDichVu);
                lst.Add(tc_LoiNhuanGopVeBanHang);
                Report_TaiChinh_TheoNam tc_ChiPhi = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ChiPhi);
                //tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2 + 7.3)";
                tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2)";
                Report_TaiChinh_TheoNam tc_PhiGiaoHang = new Report_TaiChinh_TheoNam();
                ResertDN(tc_PhiGiaoHang);
                tc_PhiGiaoHang.TaiChinh = "     Phí giao hàng (trả đối tác) (7.1)";
                Report_TaiChinh_TheoNam tc_XuatKhoHangHoa = new Report_TaiChinh_TheoNam();
                tc_XuatKhoHangHoa.TaiChinh = "     Xuất kho hàng hóa (7.1)";
                ResertDN(tc_XuatKhoHangHoa);
                Report_TaiChinh_TheoNam tc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoNam();
                tc_GiaTriThanhToanDiem.TaiChinh = "     Giá trị thanh toán bằng điểm (7.2)";
                ResertDN(tc_GiaTriThanhToanDiem);
                List<SqlParameter> prmcp = new List<SqlParameter>();
                prmcp.Add(new SqlParameter("year", year));
                prmcp.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhYear_ChiPhiBanHangPRC>("exec ReportTaiChinhYear_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray());
                foreach (var item in tbl_ChiPhi)
                {
                    tc_ChiPhi.Nam = (tc_PhiGiaoHang.Nam + item.GiaTriHuy + item.DiemThanhToan);
                    tc_XuatKhoHangHoa.Nam = (item.GiaTriHuy);
                    tc_GiaTriThanhToanDiem.Nam = (item.DiemThanhToan);
                }
                tc_ChiPhi.Tong = (tc_ChiPhi.Nam);
                tc_PhiGiaoHang.Tong = (tc_PhiGiaoHang.Nam);
                tc_XuatKhoHangHoa.Tong = (tc_XuatKhoHangHoa.Nam);
                tc_GiaTriThanhToanDiem.Tong = (tc_GiaTriThanhToanDiem.Nam);
                lst.Add(tc_ChiPhi);
                //lst.Add(tc_PhiGiaoHang);
                lst.Add(tc_XuatKhoHangHoa);
                lst.Add(tc_GiaTriThanhToanDiem);
                Report_TaiChinh_TheoNam tc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoNam();
                tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = "Lợi nhuận từ hoạt động kinh doanh (8 = 6 - 7)";
                tc_LoiNhuanTuHoatDongKinhDoanh.Nam = (tc_LoiNhuanGopVeBanHang.Nam - tc_ChiPhi.Nam);
                tc_LoiNhuanTuHoatDongKinhDoanh.Tong = (tc_LoiNhuanTuHoatDongKinhDoanh.Nam);
                lst.Add(tc_LoiNhuanTuHoatDongKinhDoanh);
                Report_TaiChinh_TheoNam tc_ThuNhapKhac8 = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ThuNhapKhac8);
                tc_ThuNhapKhac8.TaiChinh = "Thu nhập khác (9 = 9.1 + 9.2)";
                Report_TaiChinh_TheoNam tc_PhiTraHang = new Report_TaiChinh_TheoNam();
                ResertDN(tc_PhiTraHang);
                tc_PhiTraHang.TaiChinh = "     Phí trả hàng (9.1)";
                //Report_TaiChinh_TheoNam DM15 = new Report_TaiChinh_TheoNam();
                //ResertDN(DM15);
                //DM15.TaiChinh = "     Khách hàng thanh toán";
                Report_TaiChinh_TheoNam tc_ThuNhapKhac = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ThuNhapKhac);
                tc_ThuNhapKhac.TaiChinh = "     Thu nhập khác (9.2)";
                Report_TaiChinh_TheoNam tc_ChiPhiKhac = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ChiPhiKhac);
                tc_ChiPhiKhac.TaiChinh = "Chi phí khác (10)";
                Report_TaiChinh_TheoNam tc_LoiNhuanThuan = new Report_TaiChinh_TheoNam();
                ResertDN(tc_LoiNhuanThuan);
                tc_LoiNhuanThuan.TaiChinh = "Lợi nhuận thuần (11 = (8 + 9) - 10)";
                List<SqlParameter> prmsq = new List<SqlParameter>();
                prmsq.Add(new SqlParameter("year", year));
                prmsq.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_sq = _db.Database.SqlQuery<ReportTaiChinhYear_SoQuyBanHangPRC>("exec ReportTaiChinhYear_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray());
                foreach (var item in tbl_sq)
                {
                    tc_PhiTraHang.Nam = (item.PhiTraHangNhap);
                    //DM15.Nam = (item.KhachThanhToan);
                    tc_ThuNhapKhac.Nam = (item.ThuNhapKhac);
                    tc_ChiPhiKhac.Nam = (item.ChiPhiKhac);
                }
                tc_ThuNhapKhac8.Nam = (tc_PhiTraHang.Nam + tc_ThuNhapKhac.Nam);
                tc_ThuNhapKhac8.Tong = (tc_ThuNhapKhac8.Nam);
                tc_PhiTraHang.Tong = (tc_PhiTraHang.Nam);
                //DM15.Tong = (DM15.Nam);
                tc_ThuNhapKhac.Tong = (tc_ThuNhapKhac.Nam);
                tc_ChiPhiKhac.Tong = (tc_ChiPhiKhac.Nam);
                lst.Add(tc_ThuNhapKhac8);
                lst.Add(tc_PhiTraHang);
                //lst.Add(DM15);
                lst.Add(tc_ThuNhapKhac);
                lst.Add(tc_ChiPhiKhac);
                tc_LoiNhuanThuan.Nam = (tc_LoiNhuanTuHoatDongKinhDoanh.Nam + tc_ThuNhapKhac8.Nam - tc_ChiPhiKhac.Nam);
                tc_LoiNhuanThuan.Tong = (tc_LoiNhuanThuan.Nam);
                lst.Add(tc_LoiNhuanThuan);
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - getListTaiChinh_TheoNam: " + ex.Message);
                return new List<Report_TaiChinh_TheoNam>();
            }

        }

        public List<Report_TaiChinh_TheoNam> getListTaiChinh_TheoNam_Gara(int year, string ID_ChiNhanh)
        {
            try
            {
                List<Report_TaiChinh_TheoNam> lst = new List<Report_TaiChinh_TheoNam>();
                Report_TaiChinh_TheoNam tc_DoanhThuBanHang = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_GiamTruDoanhThu = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_GiamGiaHoaDon = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_GiaTriHangBanBiTraLai = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_DoanhThuThuan = new Report_TaiChinh_TheoNam();
                Report_TaiChinh_TheoNam tc_XuatHangSuDungGoiDichVu = new Report_TaiChinh_TheoNam();

                tc_DoanhThuBanHang.TaiChinh = "Doanh thu bán hàng (1)";
                tc_GiamTruDoanhThu.TaiChinh = "Giảm trừ doanh thu (2 = 2.1 + 2.2)";
                tc_GiamGiaHoaDon.TaiChinh = "     Giảm giá hóa đơn (2.1)";
                tc_GiaTriHangBanBiTraLai.TaiChinh = "     Giá trị hàng bán bị trả lại (2.2)";
                tc_XuatHangSuDungGoiDichVu.TaiChinh = "Xuất hàng sửa chữa (5)";
                tc_DoanhThuThuan.TaiChinh = "Doanh thu thuần (3 = 1 - 2)";
                List<SqlParameter> prm = new List<SqlParameter>();
                prm.Add(new SqlParameter("year", year));
                prm.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_DoanhThuBanHang = _db.Database.SqlQuery<ReportTaiChinhYear_DoanhThuBanHangPRC>("exec ReportTaiChinhYear_DoanhThuBanHang @year, @ID_ChiNhanh", prm.ToArray());
                ResertDN(tc_DoanhThuBanHang);
                ResertDN(tc_GiamTruDoanhThu);
                ResertDN(tc_GiamGiaHoaDon);
                ResertDN(tc_GiaTriHangBanBiTraLai);
                ResertDN(tc_DoanhThuThuan);
                foreach (var item in tbl_DoanhThuBanHang)
                {
                    tc_DoanhThuBanHang.Nam = (item.DoanhThu);
                    tc_GiamTruDoanhThu.Nam = (item.GiaTriTra + item.GiamGiaHD);
                    tc_GiamGiaHoaDon.Nam = (item.GiamGiaHD);
                    tc_GiaTriHangBanBiTraLai.Nam = (item.GiaTriTra);
                    tc_XuatHangSuDungGoiDichVu.Nam = (item.GiaVonGDV);
                    tc_DoanhThuThuan.Nam = (tc_DoanhThuBanHang.Nam - tc_GiamTruDoanhThu.Nam);
                }
                tc_DoanhThuBanHang.Tong = (tc_DoanhThuBanHang.Nam);
                tc_GiamTruDoanhThu.Tong = (tc_GiamTruDoanhThu.Nam);
                tc_GiamGiaHoaDon.Tong = (tc_GiamGiaHoaDon.Nam);
                tc_GiaTriHangBanBiTraLai.Tong = (tc_GiaTriHangBanBiTraLai.Nam);
                tc_XuatHangSuDungGoiDichVu.Tong = (tc_XuatHangSuDungGoiDichVu.Nam);
                tc_DoanhThuThuan.Tong = (tc_DoanhThuThuan.Nam);
                lst.Add(tc_DoanhThuBanHang);
                lst.Add(tc_GiamTruDoanhThu);
                lst.Add(tc_GiamGiaHoaDon);
                lst.Add(tc_GiaTriHangBanBiTraLai);
                lst.Add(tc_DoanhThuThuan);
                //DM    
                List<SqlParameter> prmgv = new List<SqlParameter>();
                prmgv.Add(new SqlParameter("year", year));
                prmgv.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_GiaVon = _db.Database.SqlQuery<ReportTaiChinhYear_GiaVonBanHangPRC>("exec ReportTaiChinhYear_GiaVonBanHang @year, @ID_ChiNhanh", prmgv.ToArray());
                Report_TaiChinh_TheoNam tc_GiaVonHangBan = new Report_TaiChinh_TheoNam();
                ResertDN(tc_GiaVonHangBan);
                tc_GiaVonHangBan.TaiChinh = "Giá vốn hàng bán (4)";
                Report_TaiChinh_TheoNam tc_LoiNhuanGopVeBanHang = new Report_TaiChinh_TheoNam();
                tc_LoiNhuanGopVeBanHang.TaiChinh = "Lợi nhuận gộp về bán hàng (7 = 3 - 4 - 5 - 6)";
                ResertDN(tc_LoiNhuanGopVeBanHang);
                List<SqlParameter> prmcpsc = new List<SqlParameter>();
                prmcpsc.Add(new SqlParameter("Year", year));
                prmcpsc.Add(new SqlParameter("IdChiNhanh", ID_ChiNhanh));
                prmcpsc.Add(new SqlParameter("LoaiBaoCao", 2));
                var tbl_ChiPhiSuaChua = _db.Database.SqlQuery<ReportTaiChinh_ChiPhiSuaChuaPRC>("exec ReportTaiChinh_ChiPhiSuaChua @Year, @IdChiNhanh, @LoaiBaoCao", prmcpsc.ToArray());
                Report_TaiChinh_TheoNam tc_ChiPhiSuaChua = new Report_TaiChinh_TheoNam();
                tc_ChiPhiSuaChua.TaiChinh = "Chi phí sửa chữa (6)";
                ResertDN(tc_ChiPhiSuaChua);
                foreach (var item in tbl_ChiPhiSuaChua)
                {
                    tc_ChiPhiSuaChua.Nam = item.ChiPhi;
                }
                tc_ChiPhiSuaChua.Tong = tc_ChiPhiSuaChua.Nam;


                foreach (var item in tbl_GiaVon)
                {
                    tc_GiaVonHangBan.Nam = (item.TongGiaVonBan - item.TongGiaVonTra);
                    tc_LoiNhuanGopVeBanHang.Nam = (tc_DoanhThuThuan.Nam - tc_GiaVonHangBan.Nam - tc_XuatHangSuDungGoiDichVu.Nam - tc_ChiPhiSuaChua.Nam);
                }
                tc_GiaVonHangBan.Tong = (tc_GiaVonHangBan.Nam);
                tc_LoiNhuanGopVeBanHang.Tong = (tc_LoiNhuanGopVeBanHang.Nam);


                lst.Add(tc_GiaVonHangBan);
                lst.Add(tc_XuatHangSuDungGoiDichVu);
                lst.Add(tc_ChiPhiSuaChua);
                lst.Add(tc_LoiNhuanGopVeBanHang);
                Report_TaiChinh_TheoNam tc_ChiPhi = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ChiPhi);
                //tc_ChiPhi.TaiChinh = "Chi phí (7 = 7.1 + 7.2 + 7.3)";
                tc_ChiPhi.TaiChinh = "Chi phí (8 = 8.1 + 8.2)";
                Report_TaiChinh_TheoNam tc_PhiGiaoHang = new Report_TaiChinh_TheoNam();
                ResertDN(tc_PhiGiaoHang);
                tc_PhiGiaoHang.TaiChinh = "     Phí giao hàng (trả đối tác) (8.1)";
                Report_TaiChinh_TheoNam tc_XuatKhoHangHoa = new Report_TaiChinh_TheoNam();
                tc_XuatKhoHangHoa.TaiChinh = "     Xuất kho hàng hóa (8.1)";
                ResertDN(tc_XuatKhoHangHoa);
                Report_TaiChinh_TheoNam tc_GiaTriThanhToanDiem = new Report_TaiChinh_TheoNam();
                tc_GiaTriThanhToanDiem.TaiChinh = "     Giá trị thanh toán bằng điểm (8.2)";
                ResertDN(tc_GiaTriThanhToanDiem);
                List<SqlParameter> prmcp = new List<SqlParameter>();
                prmcp.Add(new SqlParameter("year", year));
                prmcp.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_ChiPhi = _db.Database.SqlQuery<ReportTaiChinhYear_ChiPhiBanHangPRC>("exec ReportTaiChinhYear_ChiPhiBanHang @year, @ID_ChiNhanh", prmcp.ToArray());
                foreach (var item in tbl_ChiPhi)
                {
                    tc_ChiPhi.Nam = (tc_PhiGiaoHang.Nam + item.GiaTriHuy + item.DiemThanhToan);
                    tc_XuatKhoHangHoa.Nam = (item.GiaTriHuy);
                    tc_GiaTriThanhToanDiem.Nam = (item.DiemThanhToan);
                }
                tc_ChiPhi.Tong = (tc_ChiPhi.Nam);
                tc_PhiGiaoHang.Tong = (tc_PhiGiaoHang.Nam);
                tc_XuatKhoHangHoa.Tong = (tc_XuatKhoHangHoa.Nam);
                tc_GiaTriThanhToanDiem.Tong = (tc_GiaTriThanhToanDiem.Nam);
                lst.Add(tc_ChiPhi);
                //lst.Add(tc_PhiGiaoHang);
                lst.Add(tc_XuatKhoHangHoa);
                lst.Add(tc_GiaTriThanhToanDiem);
                Report_TaiChinh_TheoNam tc_LoiNhuanTuHoatDongKinhDoanh = new Report_TaiChinh_TheoNam();
                tc_LoiNhuanTuHoatDongKinhDoanh.TaiChinh = "Lợi nhuận từ hoạt động kinh doanh (9 = 7 - 8)";
                tc_LoiNhuanTuHoatDongKinhDoanh.Nam = (tc_LoiNhuanGopVeBanHang.Nam - tc_ChiPhi.Nam);
                tc_LoiNhuanTuHoatDongKinhDoanh.Tong = (tc_LoiNhuanTuHoatDongKinhDoanh.Nam);
                lst.Add(tc_LoiNhuanTuHoatDongKinhDoanh);
                Report_TaiChinh_TheoNam tc_ThuNhapKhac8 = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ThuNhapKhac8);
                tc_ThuNhapKhac8.TaiChinh = "Thu nhập khác (10 = 10.1 + 10.2)";
                Report_TaiChinh_TheoNam tc_PhiTraHang = new Report_TaiChinh_TheoNam();
                ResertDN(tc_PhiTraHang);
                tc_PhiTraHang.TaiChinh = "     Phí trả hàng (10.1)";
                //Report_TaiChinh_TheoNam DM15 = new Report_TaiChinh_TheoNam();
                //ResertDN(DM15);
                //DM15.TaiChinh = "     Khách hàng thanh toán";
                Report_TaiChinh_TheoNam tc_ThuNhapKhac = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ThuNhapKhac);
                tc_ThuNhapKhac.TaiChinh = "     Thu nhập khác (10.2)";
                Report_TaiChinh_TheoNam tc_ChiPhiKhac = new Report_TaiChinh_TheoNam();
                ResertDN(tc_ChiPhiKhac);
                tc_ChiPhiKhac.TaiChinh = "Chi phí khác (11)";
                Report_TaiChinh_TheoNam tc_LoiNhuanThuan = new Report_TaiChinh_TheoNam();
                ResertDN(tc_LoiNhuanThuan);
                tc_LoiNhuanThuan.TaiChinh = "Lợi nhuận thuần (12 = (9 + 10) - 11)";
                List<SqlParameter> prmsq = new List<SqlParameter>();
                prmsq.Add(new SqlParameter("year", year));
                prmsq.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                var tbl_sq = _db.Database.SqlQuery<ReportTaiChinhYear_SoQuyBanHangPRC>("exec ReportTaiChinhYear_SoQuyBanHang @year, @ID_ChiNhanh", prmsq.ToArray());
                foreach (var item in tbl_sq)
                {
                    tc_PhiTraHang.Nam = (item.PhiTraHangNhap);
                    //DM15.Nam = (item.KhachThanhToan);
                    tc_ThuNhapKhac.Nam = (item.ThuNhapKhac);
                    tc_ChiPhiKhac.Nam = (item.ChiPhiKhac);
                }
                tc_ThuNhapKhac8.Nam = (tc_PhiTraHang.Nam + tc_ThuNhapKhac.Nam);
                tc_ThuNhapKhac8.Tong = (tc_ThuNhapKhac8.Nam);
                tc_PhiTraHang.Tong = (tc_PhiTraHang.Nam);
                //DM15.Tong = (DM15.Nam);
                tc_ThuNhapKhac.Tong = (tc_ThuNhapKhac.Nam);
                tc_ChiPhiKhac.Tong = (tc_ChiPhiKhac.Nam);
                lst.Add(tc_ThuNhapKhac8);
                lst.Add(tc_PhiTraHang);
                //lst.Add(DM15);
                lst.Add(tc_ThuNhapKhac);
                lst.Add(tc_ChiPhiKhac);
                tc_LoiNhuanThuan.Nam = (tc_LoiNhuanTuHoatDongKinhDoanh.Nam + tc_ThuNhapKhac8.Nam - tc_ChiPhiKhac.Nam);
                tc_LoiNhuanThuan.Tong = (tc_LoiNhuanThuan.Nam);
                lst.Add(tc_LoiNhuanThuan);
                return lst;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - getListTaiChinh_TheoNam: " + ex.Message);
                return new List<Report_TaiChinh_TheoNam>();
            }

        }

        public List<BaoCaoTaiChinh_PTTCTheoThangPRC> GetBaoCaoTaiChinh_PhanTichThuChiTheoThang(string maKH_search, string maKH_TV, int year, string ID_ChiNhanh, string LaDT_search,
            string ID_NhomDoiTuong_search, string ID_NhomDoiTuong_SP, string loaiThuChi_search, string HachToanKD_Search, string LoaiTien_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaKH", maKH_search));
                sql.Add(new SqlParameter("MaKH_TV", maKH_TV));
                sql.Add(new SqlParameter("year", year));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong_SP", ID_NhomDoiTuong_SP));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_PTTCTheoThangPRC>("exec BaoCaoTaiChinh_PhanTichThuChiTheoThang @MaKH, @MaKH_TV, @year, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_PhanTichThuChiTheoThang: " + ex.Message);
                return new List<BaoCaoTaiChinh_PTTCTheoThangPRC>();
            }
        }

        public List<BaoCaoTaiChinh_PTTCTheoThangPRC> GetBaoCaoTaiChinh_PhanTichThuChiTheoThang_v2(int year, string ID_ChiNhanh, string LaDT_search,
            string ID_NhomDoiTuong, string loaiThuChi_search, bool? HachToanKD_Search, string LoaiTien_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("year", year));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search == null ? (object)DBNull.Value : HachToanKD_Search.Value));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_PTTCTheoThangPRC>("exec BaoCaoTaiChinh_PhanTichThuChiTheoThang_v2 @year, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_PhanTichThuChiTheoThang_v2: " + ex.Message);
                return new List<BaoCaoTaiChinh_PTTCTheoThangPRC>();
            }
        }

        public List<BaoCaoTaiChinh_PTTCTheoQuyPRC> GetBaoCaoTaiChinh_PhanTichThuChiTheoQuy(string maKH_search, string maKH_TV, int year, string ID_ChiNhanh,
            string LaDT_search, string ID_NhomDoiTuong_search, string ID_NhomDoiTuong_SP, string loaiThuChi_search, string HachToanKD_Search, string LoaiTien_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaKH", maKH_search));
                sql.Add(new SqlParameter("MaKH_TV", maKH_TV));
                sql.Add(new SqlParameter("year", year));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong_SP", ID_NhomDoiTuong_SP));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_PTTCTheoQuyPRC>("exec BaoCaoTaiChinh_PhanTichThuChiTheoQuy @MaKH, @MaKH_TV, @year, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_PhanTichThuChiTheoQuy: " + ex.Message);
                return new List<BaoCaoTaiChinh_PTTCTheoQuyPRC>();
            }
        }

        public List<BaoCaoTaiChinh_PTTCTheoQuyPRC> GetBaoCaoTaiChinh_PhanTichThuChiTheoQuy_v2(int year, string ID_ChiNhanh,
            string LaDT_search, string ID_NhomDoiTuong_search, string loaiThuChi_search, bool? HachToanKD_Search, string LoaiTien_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("year", year));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search == null ? (object)DBNull.Value : HachToanKD_Search.Value));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_PTTCTheoQuyPRC>("exec BaoCaoTaiChinh_PhanTichThuChiTheoQuy_v2 @year, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_PhanTichThuChiTheoQuy_v2: " + ex.Message);
                return new List<BaoCaoTaiChinh_PTTCTheoQuyPRC>();
            }
        }

        public List<BaoCaoTaiChinh_PTTCTheoNamPRC> GetBaoCaoTaiChinh_PhanTichThuChiTheoNam(string maKH_search, string maKH_TV, int year, string ID_ChiNhanh,
            string LaDT_search, string ID_NhomDoiTuong_search, string ID_NhomDoiTuong_SP, string loaiThuChi_search, string HachToanKD_Search, string LoaiTien_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("MaKH", maKH_search));
                sql.Add(new SqlParameter("MaKH_TV", maKH_TV));
                sql.Add(new SqlParameter("year", year));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong_SP", ID_NhomDoiTuong_SP));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_PTTCTheoNamPRC>("exec BaoCaoTaiChinh_PhanTichThuChiTheoNam_v2 @MaKH, @MaKH_TV, @year, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong,@ID_NhomDoiTuong_SP, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_PhanTichThuChiTheoNam: " + ex.Message);
                return new List<BaoCaoTaiChinh_PTTCTheoNamPRC>();
            }
        }

        public List<BaoCaoTaiChinh_PTTCTheoNamPRC> GetBaoCaoTaiChinh_PhanTichThuChiTheoNam_v2(int year, string ID_ChiNhanh,
            string LaDT_search, string ID_NhomDoiTuong_search, string loaiThuChi_search, bool? HachToanKD_Search, string LoaiTien_Search)
        {
            try
            {
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("year", year));
                sql.Add(new SqlParameter("ID_ChiNhanh", ID_ChiNhanh));
                sql.Add(new SqlParameter("loaiKH", LaDT_search));
                sql.Add(new SqlParameter("ID_NhomDoiTuong", ID_NhomDoiTuong_search));
                sql.Add(new SqlParameter("lstThuChi", loaiThuChi_search));
                sql.Add(new SqlParameter("HachToanKD", HachToanKD_Search == null ? (object)DBNull.Value : HachToanKD_Search.Value));
                sql.Add(new SqlParameter("LoaiTien", LoaiTien_Search));
                return _db.Database.SqlQuery<BaoCaoTaiChinh_PTTCTheoNamPRC>("exec BaoCaoTaiChinh_PhanTichThuChiTheoNam_v2 @year, @ID_ChiNhanh, @loaiKH, @ID_NhomDoiTuong, @lstThuChi, @HachToanKD, @LoaiTien", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - GetBaoCaoTaiChinh_PhanTichThuChiTheoNam_v2: " + ex.Message);
                return new List<BaoCaoTaiChinh_PTTCTheoNamPRC>();
            }
        }

        public List<ReportThuChi_LoaiTien> BCThuChi_TheoLoaiTien(ParamPreportThuChi param)
        {
            try
            {
                string idDonVis = string.Empty, loaiDTs = string.Empty, khoanthuchis = string.Empty;
                if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count() > 0)
                {
                    idDonVis = string.Join(",", param.IDChiNhanhs);
                }
                if (param.LoaiDoiTuongs != null && param.LoaiDoiTuongs.Count() > 0)
                {
                    loaiDTs = string.Join(",", param.LoaiDoiTuongs);
                }
                if (param.KhoanMucThuChis != null && param.KhoanMucThuChis.Count() > 0)
                {
                    khoanthuchis = string.Join(",", param.KhoanMucThuChis);
                }
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("IDChiNhanhs", idDonVis));
                sql.Add(new SqlParameter("DateFrom", param.DateFrom));
                sql.Add(new SqlParameter("DateTo", param.DateTo));
                sql.Add(new SqlParameter("LoaiDoiTuongs", loaiDTs));
                sql.Add(new SqlParameter("KhoanMucThuChis", khoanthuchis));
                return _db.Database.SqlQuery<ReportThuChi_LoaiTien>("exec dbo.BCThuChi_TheoLoaiTien @IDChiNhanhs, @DateFrom, @DateTo, @LoaiDoiTuongs, @KhoanMucThuChis", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - BCThuChi_TheoLoaiTien: " + ex.Message);
                return new List<ReportThuChi_LoaiTien>();
            }
        } 
        public List<BaoCao_CongNoChiTietDTO> LoadBaoCaoCongNoChitiet(CommonParamSearch param)
        {
            try
            {
                string idDonVis = string.Empty, trangthaiCongNo=string.Empty;
                if (param.IDChiNhanhs != null && param.IDChiNhanhs.Count() > 0)
                {
                    idDonVis = string.Join(",", param.IDChiNhanhs);
                }
                if (param.TrangThais != null && param.TrangThais.Count() > 0)
                {
                    trangthaiCongNo = string.Join(",", param.TrangThais);
                }
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("IDChiNhanhs", idDonVis));
                sql.Add(new SqlParameter("DateFrom", param.DateFrom?? (object) DBNull.Value));
                sql.Add(new SqlParameter("DateTo", param.DateTo ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("TextSearch", param.TextSearch ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("TrangThais", trangthaiCongNo ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("CurrentPage", param.CurrentPage));
                sql.Add(new SqlParameter("PageSize", param.PageSize));
                return _db.Database.SqlQuery<BaoCao_CongNoChiTietDTO>("exec dbo.GetBaoCaoCongNoChiTiet @IDChiNhanhs, @DateFrom, @DateTo, " +
                    "@TextSearch, @TrangThais, @CurrentPage, @PageSize", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportTaiChinh - LoadBaoCaoCongNoChitiet: " + ex.Message);
                return new List<BaoCao_CongNoChiTietDTO>();
            }
        }

        private void ResertDN(Report_TaiChinh_TheoNam DN)
        {
            DN.Nam = 0;
            DN.Tong = 0;
        }

        private void ResertDM(Report_TaiChinh_TheoThang DM)
        {
            DM.Thang1 = 0;
            DM.Thang2 = 0;
            DM.Thang3 = 0;
            DM.Thang4 = 0;
            DM.Thang5 = 0;
            DM.Thang6 = 0;
            DM.Thang7 = 0;
            DM.Thang8 = 0;
            DM.Thang9 = 0;
            DM.Thang10 = 0;
            DM.Thang11 = 0;
            DM.Thang12 = 0;
            DM.Tong = 0;
        }

    }
}
