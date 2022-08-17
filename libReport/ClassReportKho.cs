using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportKho
    {
        private SsoftvnContext _db;

        public ClassReportKho(SsoftvnContext db)
        {
            _db = db;
        }

        public ReportSale_ParamCommon ReportSale_GetCommonParam(array_BaoCaoKhoHang param)
        {
            string theoDoi = "%%";
            string trangThai = "%%";
            string LaHH_search = "%%";
            var idChiNhanhs = string.Join(",", param.lstIDChiNhanh);

            switch (param.TinhTrang)
            {
                case 2:// dangkinhdoanh
                    theoDoi = "%1%";
                    trangThai = "%0%";
                    break;
                case 3:// ngungkinhdoanh
                    theoDoi = "%0%";
                    break;
                case 4:// hangxoabo
                    trangThai = "%1%";
                    break;
            }
            switch (param.LaHangHoa)
            {
                case 0:
                    LaHH_search = "0";
                    break;
                case 1:
                    LaHH_search = "1";
                    break;
            }
            return new ReportSale_ParamCommon()
            {
                TheoDoi = theoDoi,
                TrangThai = trangThai,
                LoaiHangHoa = LaHH_search,
                IDChiNhanhs = idChiNhanhs,
            };
        }

        public List<BaoCaoKho_TonKhoPRC> GetBaoCaoKho_TonKhoPRC(array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("ThoiGian", param.timeEnd));
                sql.Add(new SqlParameter("ID_DonVi", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                sql.Add(new SqlParameter("TonKho", param.TonKho));
                return _db.Database.SqlQuery<BaoCaoKho_TonKhoPRC>("exec BaoCaoKho_TonKho @ID_DonVi, @ThoiGian, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @TonKho", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_TonKhoPRC: " + ex.Message);
                return new List<BaoCaoKho_TonKhoPRC>();
            }
        }

        public List<BaoCaoKho_TonKho_TongHopPRC> GetBaoCaoKho_TonKho_TongHopPRC(array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("ThoiGian", param.timeEnd));
                sql.Add(new SqlParameter("ID_DonVis", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang??(object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                sql.Add(new SqlParameter("TonKho", param.TonKho));
                return _db.Database.SqlQuery<BaoCaoKho_TonKho_TongHopPRC>("exec BaoCaoKho_TonKho_TongHop @ID_DonVis, @ThoiGian, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @TonKho", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_TonKho_TongHopPRC: " + ex.Message);
                return new List<BaoCaoKho_TonKho_TongHopPRC>();
            }
        }

        public List<BaoCaoKho_NhapXuaTonPRC> GetBaoCaoKho_NhapXuatTonPRC(array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_DonVi", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                sql.Add(new SqlParameter("CoPhatSinh", param.TrangThai));
                return _db.Database.SqlQuery<BaoCaoKho_NhapXuaTonPRC>("exec BaoCaoKho_NhapXuatTon @ID_DonVi, @timeStart, @timeEnd, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @CoPhatSinh", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_NhapXuatTonPRC: " + ex.Message);
                return new List<BaoCaoKho_NhapXuaTonPRC>();
            }
        }

        public List<BaoCaoKho_NhapXuaTonChiTietPRC> GetBaoCaoKho_NhapXuatTonChiTietPRC(array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_DonVi", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                sql.Add(new SqlParameter("CoPhatSinh", param.TrangThai));
                return _db.Database.SqlQuery<BaoCaoKho_NhapXuaTonChiTietPRC>("exec BaoCaoKho_NhapXuatTonChiTiet @ID_DonVi, @timeStart, @timeEnd, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @CoPhatSinh", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_NhapXuatTonChiTietPRC: " + ex.Message);
                return new List<BaoCaoKho_NhapXuaTonChiTietPRC>();
            }
        }
        public List<BaoCaoKho_XuatChuyenHangPRC> GetBaoCaoKho_XuatChuyenHangPRC(bool Xuat, array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LaHangHoa", obj.TheoDoi));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));

                if (Xuat)
                {
                    return _db.Database.SqlQuery<BaoCaoKho_XuatChuyenHangPRC>("exec BaoCaoKho_XuatChuyenHang @SearchString, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
                }
                else
                {
                    return _db.Database.SqlQuery<BaoCaoKho_XuatChuyenHangPRC>("exec BaoCaoKho_NhapChuyenHang @SearchString, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_XuatChuyenHangPRC: " + ex.Message);
                return new List<BaoCaoKho_XuatChuyenHangPRC>();
            }
        }

        public List<BaoCaoKho_XuatChuyenHangChiTietPRC> GetBaoCaoKho_XuatChuyenHangChiTietPRC(bool Xuat, array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LaHangHoa", obj.TheoDoi));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                if (Xuat)
                {
                    return _db.Database.SqlQuery<BaoCaoKho_XuatChuyenHangChiTietPRC>("exec BaoCaoKho_XuatChuyenHangChiTiet @SearchString, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
                }
                else
                {
                    return _db.Database.SqlQuery<BaoCaoKho_XuatChuyenHangChiTietPRC>("exec BaoCaoKho_NhapChuyenHangChiTiet @SearchString, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang, @ID_NguoiDung", sql.ToArray()).ToList();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_XuatChuyenHangChiTietPRC: " + ex.Message);
                return new List<BaoCaoKho_XuatChuyenHangChiTietPRC>();
            }
        }

        public List<BaoCaoKho_XuatChuyenHangPRC> GetBaoCaoKho_TongHopHangNhapXuatKho(array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa.Trim()));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_DonVi", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                if (param.XuatKho??true)
                {
                    return _db.Database.SqlQuery<BaoCaoKho_XuatChuyenHangPRC>("exec BaoCaoKho_TongHopHangXuat @ID_DonVi, @timeStart, @timeEnd, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @LoaiChungTu", sql.ToArray()).ToList();
                }
                else
                {
                    return _db.Database.SqlQuery<BaoCaoKho_XuatChuyenHangPRC>("exec BaoCaoKho_TongHopHangNhap @ID_DonVi, @timeStart, @timeEnd, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @LoaiChungTu", sql.ToArray()).ToList();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_TongHopHangNhapXuatKho: " + ex.Message);
                return new List<BaoCaoKho_XuatChuyenHangPRC>();
            }
        }

        public List<BaoCaoKho_ChiTietHangNhapKhoPRC> GetBaoCaoKho_ChiTietHangNhapXuatKho(array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_DonVi", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                if (param.XuatKho ?? true)
                {
                    return _db.Database.SqlQuery<BaoCaoKho_ChiTietHangNhapKhoPRC>("exec BaoCaoKho_ChiTietHangXuat @ID_DonVi, @timeStart, @timeEnd, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @LoaiChungTu", sql.ToArray()).ToList();
                }
                else
                {
                    return _db.Database.SqlQuery<BaoCaoKho_ChiTietHangNhapKhoPRC>("exec BaoCaoKho_ChiTietHangNhap @ID_DonVi, @timeStart, @timeEnd, @SearchString, @ID_NhomHang, @TheoDoi, @TrangThai, @ID_NguoiDung, @LoaiChungTu", sql.ToArray()).ToList();
                }
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_ChiTietHangNhapXuatKho: " + ex.Message);
                return new List<BaoCaoKho_ChiTietHangNhapKhoPRC>();
            }
        }

        public List<BaoCaoKho_XuatDichVuDinhLuongPRC> GetBaoCaoKho_XuatDichVuDinhLuong(array_BaoCaoKhoHang param)
        {
            try
            {
                var obj = ReportSale_GetCommonParam(param);
                List<SqlParameter> sql = new List<SqlParameter>();
                sql.Add(new SqlParameter("SearchString", param.MaHangHoa));
                sql.Add(new SqlParameter("LoaiChungTu", param.LoaiChungTu));
                sql.Add(new SqlParameter("timeStart", param.timeStart));
                sql.Add(new SqlParameter("timeEnd", param.timeEnd));
                sql.Add(new SqlParameter("ID_ChiNhanh", obj.IDChiNhanhs));
                sql.Add(new SqlParameter("LaHangHoa", obj.LoaiHangHoa));
                sql.Add(new SqlParameter("TheoDoi", obj.TheoDoi));
                sql.Add(new SqlParameter("TrangThai", obj.TrangThai));
                sql.Add(new SqlParameter("ID_NhomHang", param.ID_NhomHang ?? (object)DBNull.Value));
                sql.Add(new SqlParameter("ID_NguoiDung", param.ID_NguoiDung));
                return _db.Database.SqlQuery<BaoCaoKho_XuatDichVuDinhLuongPRC>("exec dbo.BaoCaoKho_XuatDichVuDinhLuong @SearchString,@LoaiChungTu, @timeStart, @timeEnd, @ID_ChiNhanh, @LaHangHoa, @TheoDoi, @TrangThai, @ID_NhomHang,@ID_NguoiDung", sql.ToArray()).ToList();
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - ClassReportKho - GetBaoCaoKho_XuatDichVuDinhLuong: " + ex.Message);
                return new List<BaoCaoKho_XuatDichVuDinhLuongPRC>();
            }
        }
    }
}

public class array_BaoCaoKhoHang
{
    public string MaHangHoa { get; set; }
    public DateTime timeStart { get; set; }
    public DateTime timeEnd { get; set; }
    public Guid ID_DonVi { get; set; }
    public string ID_ChiNhanh { get; set; }
    public int LaHangHoa { get; set; }
    public int TinhTrang { get; set; }
    public Guid? ID_NhomHang { get; set; }
    public string LoaiChungTu { get; set; }
    public Guid ID_NguoiDung { get; set; }
    public string columnsHide { get; set; }
    public string columnsHideCT { get; set; }
    public string TodayBC { get; set; }
    public string TenChiNhanh { get; set; }
    public int TrangThai { get; set; }
    public int TonKho { get; set; }
    public List<string> lstIDChiNhanh { get; set; }
    public bool? XuatKho { get; set; }
    public int? PageSize { get; set; }
    public int? CurrentPage { get; set; }
}
