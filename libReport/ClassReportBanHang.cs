using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportBanHang
    {
        private SsoftvnContext _db;

        public ClassReportBanHang(SsoftvnContext db)
        {
            _db = db;
        }
        public ReportSale_ParamCommon ReportSale_GetCommonParam(array_BaoCaoBanHang param)
        {
            string theoDoi = "%%";
            string trangThai = "%%";
            var idChiNhanhs = string.Join(",", param.lstIDChiNhanh);

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
            return new ReportSale_ParamCommon()
            {
                TheoDoi = theoDoi,
                TrangThai = trangThai,
                LoaiHangHoa = param.LoaiHangHoa,
                IDChiNhanhs = idChiNhanhs,
            };
        }

        public List<Report_HangHoa_BanHang> TongGiaTriHH_BanHang(string maHH, DateTime timeStart, DateTime timeEnd, int laHangHoa, Guid? ID_NhomHang, Guid ID_ChiNhanh)
        {
            try
            {
                List<Report_HangHoa_BanHang> lst = new List<Report_HangHoa_BanHang>();
                if (laHangHoa != 3)
                {
                    var tbl = from hd in _db.BH_HoaDon
                              join bhct in _db.BH_HoaDon_ChiTiet on hd.ID equals bhct.ID_HoaDon
                              join dvqd in _db.DonViQuiDois on bhct.ID_DonViQuiDoi equals dvqd.ID
                              join hh in _db.DM_HangHoa on dvqd.ID_HangHoa equals hh.ID
                              where (hd.LoaiHoaDon == 1 || hd.LoaiHoaDon == 2 || hd.LoaiHoaDon == 6) & hd.NgayLapHoaDon >= timeStart & hd.NgayLapHoaDon < timeEnd & /*dvqd.Xoa != true & hh.TheoDoi != false &*/ hd.ChoThanhToan == false & hd.ID_DonVi == ID_ChiNhanh
                              select new
                              {
                                  hh.ID_NhomHang,
                                  dvqd.MaHangHoa,
                                  hh.TenHangHoa,
                                  hh.LaHangHoa,
                                  bhct.SoLuong,
                                  bhct.ThanhTien,
                                  hd.LoaiHoaDon,
                              };
                    if (laHangHoa == 0)
                    {
                        tbl = tbl.Where(x => x.LaHangHoa == false);
                    }
                    else if (laHangHoa == 1)
                    {
                        tbl = tbl.Where(x => x.LaHangHoa == true);
                    }
                    if (ID_NhomHang != null)
                    {
                        tbl = tbl.Where(x => x.ID_NhomHang == ID_NhomHang);
                    }
                    var tbl1 = tbl.AsEnumerable().Select(t => new Report_SumHangHoa_BanHang
                    {
                        MaHangHoa = t.MaHangHoa,
                        TenHangHoaCV = CommonStatic.ConvertToUnSign(t.TenHangHoa).ToLower(),
                        TenHangHoaGC = CommonStatic.GetCharsStart(t.TenHangHoa).ToLower(),
                        SoLuong = t.SoLuong,
                        ThanhTien = t.ThanhTien,
                        LoaiHoaDon = t.LoaiHoaDon
                    });
                    if (maHH != null & maHH != "" & maHH != "null")
                    {
                        maHH = CommonStatic.ConvertToUnSign(maHH).ToLower();
                        tbl1 = tbl1.Where(x => x.TenHangHoaCV.Contains(@maHH) || x.TenHangHoaGC.Contains(@maHH) || x.MaHangHoa.Contains(@maHH));
                    }

                    var tbl2 = from sum in tbl1
                               group sum by new
                               {
                               } into g
                               select new Report_HangHoa_BanHang
                               {
                                   SoLuongBan = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => x.SoLuong) ?? 0, 2),
                                   GiaTriBan = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 1 || x.LoaiHoaDon == 2).Sum(x => x.ThanhTien) ?? 0, 2),
                                   SoLuongTra = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 6).Sum(x => x.SoLuong) ?? 0, 2),
                                   GiaTriTra = Math.Round((double?)g.Where(x => x.LoaiHoaDon == 6).Sum(x => x.ThanhTien) ?? 0, 2)
                               };
                    try
                    {
                        lst = tbl2.ToList();
                    }
                    catch
                    {

                    }
                }
                return lst;

            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - TongGiaTriHH_BanHang: " + ex.Message);
                return new List<Report_HangHoa_BanHang>();
            }
        }

        public List<BaoCaoBanHang_TongHopPRC> GetBaoCaoBanHang_TongHopPRC(array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("pageNumber", param.pageNumber));
                sql.Add(new SqlParameter("pageSize", param.pageSize));
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHang_TongHopPRC>("exec BaoCaoBanHang_TongHop_Page @pageNumber,@pageSize, @SearchString, @timeStart, @timeEnd," +
                    " @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu, @ID_NguoiDung" +
                    "", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHang_TongHopPRC: " + ex.Message);
                return new List<BaoCaoBanHang_TongHopPRC>();
            }
        }

        public List<BaoCaoBanHang_ChiTietPRC> GetBaoCaoBanHang_ChiTietPRC(array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("pageNumber", param.pageNumber));
                sql.Add(new SqlParameter("pageSize", param.pageSize));
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("HanBaoHanh", param.HanBaoHanh));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHang_ChiTietPRC>("exec BaoCaoBanHang_ChiTiet_Page @pageNumber,@pageSize, " +
                    "@SearchString, @timeStart, @timeEnd, @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu,@HanBaoHanh, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHang_ChiTietPRC: " + ex.Message);
                return new List<BaoCaoBanHang_ChiTietPRC>();
            }
        }
        public List<BaoCaoBanHang_DinhDanhDichVu> BaoCaoBanHang_DinhDanhDichVu(array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("pageNumber", param.pageNumber));
                sql.Add(new SqlParameter("pageSize", param.pageSize));
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                return _db.Database.SqlQuery<BaoCaoBanHang_DinhDanhDichVu>("exec BCBanHang_theoMaDinhDanh @pageNumber,@pageSize, " +
                    "@SearchString, @timeStart, @timeEnd, @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("ClassReportBanHang - BaoCaoBanHang_DinhDanhDichVu: " + ex.Message);
                return new List<BaoCaoBanHang_DinhDanhDichVu>();
            }
        }

        public List<BaoCaoBanHang_NhomHangPRC> GetBaoCaoBanHang_NhomHangPRC(array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHang_NhomHangPRC>("exec BaoCaoBanHang_NhomHang @SearchString, @timeStart, @timeEnd, " +
                    "@ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHang_NhomHangPRC: " + ex.Message);
                return new List<BaoCaoBanHang_NhomHangPRC>();
            }
        }

        public List<BaoCaoBanHang_TheoKhachHangPRC> GetBaoCaoBanHang_TheoKhachHangPRC(array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                string ID_NhomKhachHang_search = "%%";
                if (!string.IsNullOrEmpty(param.ID_NhomKhachHang) && param.ID_NhomKhachHang != "null" && param.ID_NhomKhachHang != "undefined")
                    ID_NhomKhachHang_search = param.ID_NhomKhachHang.Trim().Replace(",null", "");
              
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NhomKhachHang", param.ID_NhomKhachHang));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHang_TheoKhachHangPRC>("exec BaoCaoBanHang_TheoKhachHang @SearchString, @timeStart, @timeEnd," +
                    " @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NhomKhachHang,@LoaiChungTu, @ID_NguoiDung", sql.ToArray()).ToList();

            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHang_TheoKhachHangPRC: " + ex.Message);
                return new List<BaoCaoBanHang_TheoKhachHangPRC>();
            }
        }

        public List<BaoCaoBanHang_TheoKhachHangTanSuat> BaoCaoKhachHang_TanSuat(ParamSearchReportKhachHang lstParam)
        {
            var isDonVis = string.Join(",", lstParam.LstIDChiNhanh);
            var isNhomKHs = string.Join(",", lstParam.LstIDNhomKhach);
            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", isDonVis));
            sql.Add(new SqlParameter("IDNhomKhachs", isNhomKHs));
            sql.Add(new SqlParameter("LoaiChungTus", lstParam.LoaiChungTus));
            sql.Add(new SqlParameter("TrangThaiKhach", lstParam.TrangThaiKhach));
            sql.Add(new SqlParameter("FromDate", lstParam.FromDate));
            sql.Add(new SqlParameter("ToDate", lstParam.ToDate));
            sql.Add(new SqlParameter("NgayGiaoDichFrom", lstParam.NgayGiaoDichFrom));
            sql.Add(new SqlParameter("NgayGiaoDichTo", lstParam.NgayGiaoDichTo));  
            sql.Add(new SqlParameter("NgayTaoKHFrom", lstParam.NgayTaoKHFrom));
            sql.Add(new SqlParameter("NgayTaoKHTo", lstParam.NgayTaoKHTo));
            sql.Add(new SqlParameter("DoanhThuTu", lstParam.DoanhThuTu == null ? (object)DBNull.Value : lstParam.DoanhThuTu.Value));
            sql.Add(new SqlParameter("DoanhThuDen", lstParam.DoanhThuDen == null ? (object)DBNull.Value : lstParam.DoanhThuDen.Value));
            sql.Add(new SqlParameter("SoLanDenFrom", lstParam.SoLanDenFrom));
            sql.Add(new SqlParameter("SoLanDenTo", lstParam.SoLanDenTo == null ? (object)DBNull.Value : lstParam.SoLanDenTo.Value));
            sql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            sql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            sql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            sql.Add(new SqlParameter("ColumnSort", lstParam.ColumnSort ?? (object)DBNull.Value));
            sql.Add(new SqlParameter("TypeSort", lstParam.TypeSort ?? (object)DBNull.Value));

            return _db.Database.SqlQuery<BaoCaoBanHang_TheoKhachHangTanSuat>("exec BaoCaoKhachHang_TanSuat @IDChiNhanhs, @IDNhomKhachs, @LoaiChungTus, @TrangThaiKhach," +
                " @FromDate, @ToDate, @NgayGiaoDichFrom, @NgayGiaoDichTo, @NgayTaoKHFrom, @NgayTaoKHTo," +
                " @DoanhThuTu, @DoanhThuDen, @SoLanDenFrom,@SoLanDenTo, @TextSearch, @CurrentPage, @PageSize, @ColumnSort, @TypeSort", sql.ToArray()).ToList();
        }

        public List<BCTanSuat_NhatKyGiaoDich> GetNhatKyGiaoDich_ofKhachHang(ParamSearchReportKhachHang lstParam)
        {
            var isDonVis = string.Join(",", lstParam.LstIDChiNhanh);
            var idKhachHang = string.Join(",", lstParam.LstIDNhomKhach);
            var chungtus = string.Join(",", lstParam.LoaiChungTus);

            List<SqlParameter> sql = new List<SqlParameter>();
            sql.Add(new SqlParameter("IDChiNhanhs", isDonVis));
            sql.Add(new SqlParameter("ID_KhachHang", idKhachHang));
            sql.Add(new SqlParameter("LoaiChungTu", chungtus));
            sql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            sql.Add(new SqlParameter("FromDate", lstParam.FromDate));
            sql.Add(new SqlParameter("ToDate", lstParam.ToDate));
            sql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            sql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            return _db.Database.SqlQuery<BCTanSuat_NhatKyGiaoDich>("exec GetNhatKyGiaoDich_ofKhachHang @IDChiNhanhs, @ID_KhachHang, " +
               "@LoaiChungTu,  @TextSearch, @FromDate, @ToDate, @CurrentPage, @PageSize", sql.ToArray()).ToList();
        }

        public List<BaoCaoBanHangChiTiet_TheoKhachHangPRC> GetBaoCaoBanHangChiTiet_TheoKhachHangPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ID_KhachHang", param.NV_GioiThieu));// muontamtruong
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHangChiTiet_TheoKhachHangPRC>("exec BaoCaoBanHangChiTiet_TheoKhachHang @ID_KhachHang, @timeStart, @timeEnd, " +
                    "@ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHangChiTiet_TheoKhachHangPRC: " + ex.Message);
                return new List<BaoCaoBanHangChiTiet_TheoKhachHangPRC>();
            }
        }

        public List<BaoCaoBanHang_TheoNhanVienPRC> GetBaoCaoBanHang_TheoNhanVienPRC(array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                var idPhongBans = string.Empty;
                if (param.lstPhongBan != null && param.lstPhongBan.Count > 0)
                {
                    idPhongBans = string.Join(",", param.lstPhongBan);
                }

                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("TenNhanVien", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                sql.Add(new SqlParameter("IDPhongBans", idPhongBans));
                return _db.Database.SqlQuery<BaoCaoBanHang_TheoNhanVienPRC>(" exec BaoCaoBanHang_TheoNhanVien @TenNhanVien, @timeStart, @timeEnd, @ID_ChiNhanh, @LoaiHangHoa, " +
                    "@TheoDoi, @TrangThai, @ID_NhomHang ,@LoaiChungTu, @ID_NguoiDung, @IDPhongBans", sql.ToArray()).ToList();

            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHang_TheoNhanVienPRC: " + ex.Message);
                return new List<BaoCaoBanHang_TheoNhanVienPRC>();
            }
        }

        public List<BaoCaoBanHangChiTiet_TheoNhanVienPRC> GetBaoCaoBanHangChiTiet_TheoNhanVienPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("ID_NhanVien", param.NV_GioiThieu));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang?? (object)DBNull.Value ));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHangChiTiet_TheoNhanVienPRC>("exec BaoCaoBanHangChiTiet_TheoNhanVien @ID_NhanVien, @timeStart, @timeEnd, " +
                    "@ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHangChiTiet_TheoNhanVienPRC: " + ex.Message);
                return new List<BaoCaoBanHangChiTiet_TheoNhanVienPRC>();
            }
        }

        public List<BaoCaoBanHang_HangTraLaiPRC> GetBaoCaoBanHang_HangTraLaiPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHang_HangTraLaiPRC>("exec BaoCaoBanHang_HangTraLai @SearchString, @timeStart, @timeEnd, @ID_ChiNhanh, " +
                    "@LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHang_HangTraLaiPRC: " + ex.Message);
                return new List<BaoCaoBanHang_HangTraLaiPRC>();
            }
        }

        public List<BaoCaoHangKhuyenMai> SP_BaoCaoHangKhuyenMai(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoHangKhuyenMai>("exec SP_BaoCaoKhuyenMai @SearchString, @timeStart, @timeEnd," +
                    " @ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu, @ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - SP_BaoCaoHangKhuyenMai: " + ex.Message);
                return new List<BaoCaoHangKhuyenMai>();
            }
        }

        public List<BaoCaoBanHang_LoiNhuanPRC> GetBaoCaoBanHang_LoiNhuanPRC(libReport.array_BaoCaoBanHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LoaiHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoBanHang_LoiNhuanPRC>("exec BaoCaoBanHang_LoiNhuan @SearchString, @timeStart, @timeEnd, " +
                    "@ID_ChiNhanh, @LoaiHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@LoaiChungTu, @ID_NguoiDung", sql.ToArray()).ToList();

            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportBanHang - GetBaoCaoBanHang_LoiNhuanPRC: " + ex.Message);
                return new List<BaoCaoBanHang_LoiNhuanPRC>();
            }
        }
    }

    public class array_BaoCaoBanHang
    {
        public string MaHangHoa { get; set; }
        public string MaKhachHang { get; set; }
        public string NV_GioiThieu { get; set; }
        public DateTime timeStart { get; set; }
        public DateTime timeEnd { get; set; }
        public string ID_ViTri { get; set; }
        public string LoaiHangHoa { get; set; }
        public int TinhTrang { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public string ID_NhomKhachHang { get; set; }
        public string LoaiChungTu { get; set; }
        public string HanBaoHanh { get; set; }
        public Guid ID_NguoiDung { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string columnsHide { get; set; }
        public string TodayBC { get; set; }
        public string chitietBC { get; set; }
        public string TenChiNhanh { get; set; }
        public List<string> lstIDChiNhanh { get; set; }
        public List<string> lstPhongBan { get; set; }
        public List<string> lstNhomKhach { get; set; }
    }

    public class ReportSale_ParamCommon
    {
        public string LoaiHangHoa { get; set; }
        public string TheoDoi { get; set; }
        public string TrangThai { get; set; }
        public string IDChiNhanhs { get; set; }
        public string ThoiHanSuDung { get; set; }
    }
}
