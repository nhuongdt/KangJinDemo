using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace libReport
{
    public class ClassReportHoaHong : ClassReportPublic
    {
        private SsoftvnContext _db;

        public ClassReportHoaHong(SsoftvnContext db)
        {
            _db = db;
        }

        public List<SP_ReportDiscountAll> SP_ReportDiscountAll(ParamReportDiscount lstParam)
        {
            string idChiNhanh = string.Join(",", lstParam.LstIDChiNhanh);
            string idPhongBans = string.Empty;
            if (lstParam.DepartmentIDs != null && lstParam.DepartmentIDs.Count > 0)
            {
                idPhongBans = string.Join(",", lstParam.DepartmentIDs);
            }
            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanh));
            paramSql.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            paramSql.Add(new SqlParameter("DepartmentIDs", idPhongBans ?? (object)DBNull.Value));
            paramSql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSql.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("DateTo", lstParam.DateTo));
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            List<SP_ReportDiscountAll> data = _db.Database.SqlQuery<SP_ReportDiscountAll>("EXEC getList_ChietKhauNhanVienTongHop @ID_ChiNhanhs, @ID_NhanVienLogin,@DepartmentIDs, @TextSearch, @DateFrom, @DateTo, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<BaoCaoChietKhau_TongHopPRC> SP_ReportDiscountProduct_General(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            string loaichungtus = "1,6,19,22,25";
            string idPhongBans = string.Empty;

            if (lstParam.LoaiChungTus != null && lstParam.LoaiChungTus.Count > 0)
            {
                loaichungtus = string.Join(",", lstParam.LoaiChungTus);
            }
            if (lstParam.DepartmentIDs != null && lstParam.DepartmentIDs.Count > 0)
            {
                idPhongBans = string.Join(",", lstParam.DepartmentIDs);
            }
            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            paramSql.Add(new SqlParameter("DepartmentIDs", idPhongBans ?? (object)DBNull.Value));
            paramSql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSql.Add(new SqlParameter("LoaiChungTus", loaichungtus));
            paramSql.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("DateTo", lstParam.DateTo));
            paramSql.Add(new SqlParameter("Status_ColumnHide", lstParam.Status_ColumnHide));// ẩn cột thứ
            paramSql.Add(new SqlParameter("StatusInvoice", lstParam.StatusInvoice)); // hdTamluu = 2, hdHoanThanh = 1, all.0
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            List<BaoCaoChietKhau_TongHopPRC> data = _db.Database.SqlQuery<BaoCaoChietKhau_TongHopPRC>("EXEC ReportDiscountProduct_General @ID_ChiNhanhs, @ID_NhanVienLogin, @DepartmentIDs, @TextSearch ," +
                "@LoaiChungTus, @DateFrom, @DateTo, @Status_ColumnHide, @StatusInvoice, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<BaoCaoChietKhau_ChiTietPRC> SP_ReportDiscountProduct_Detail(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            string lahanghoas = "1,2,3";
            string loaichungtus = "1,6,19,22,25";
            string idPhongBans = string.Empty;
            if (lstParam.LaHangHoas != null && lstParam.LaHangHoas.Count > 0)
            {
                lahanghoas = string.Join(",", lstParam.LaHangHoas);
            }
            if (lstParam.LoaiChungTus != null && lstParam.LoaiChungTus.Count > 0)
            {
                loaichungtus = string.Join(",", lstParam.LoaiChungTus);
            }
            if (lstParam.DepartmentIDs != null && lstParam.DepartmentIDs.Count > 0)
            {
                idPhongBans = string.Join(",", lstParam.DepartmentIDs);
            }

            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            paramSql.Add(new SqlParameter("DepartmentIDs", idPhongBans ?? (object)DBNull.Value));
            paramSql.Add(new SqlParameter("ID_NhomHang", lstParam.ID_NhomHang));
            paramSql.Add(new SqlParameter("LaHangHoas", lahanghoas));
            paramSql.Add(new SqlParameter("LoaiChungTus", loaichungtus));
            paramSql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSql.Add(new SqlParameter("TextSearchHangHoa", lstParam.HangHoaSearch));
            paramSql.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("DateTo", lstParam.DateTo));
            paramSql.Add(new SqlParameter("Status_ColumnHide", lstParam.Status_ColumnHide));
            paramSql.Add(new SqlParameter("StatusInvoice", lstParam.StatusInvoice));
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            List<BaoCaoChietKhau_ChiTietPRC> data = _db.Database.SqlQuery<BaoCaoChietKhau_ChiTietPRC>("EXEC ReportDiscountProduct_Detail @ID_ChiNhanhs,@ID_NhanVienLogin, @DepartmentIDs, @ID_NhomHang," +
                " @LaHangHoas, @LoaiChungTus, @TextSearch, @TextSearchHangHoa, @DateFrom, @DateTo, @Status_ColumnHide, @StatusInvoice, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<HoaDon_ChuaPhanBoHoaHong> GetListHoaDon_ChuaPhanBoCK(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            string loaichungtus = "1,6,19,22,25,2,32";
            if (lstParam.LoaiChungTus != null && lstParam.LoaiChungTus.Count > 0)
            {
                loaichungtus = string.Join(",", lstParam.LoaiChungTus);
            }
            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            paramSql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSql.Add(new SqlParameter("LoaiChungTus", loaichungtus));
            paramSql.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("DateTo", lstParam.DateTo));
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            List<HoaDon_ChuaPhanBoHoaHong> data = _db.Database.SqlQuery<HoaDon_ChuaPhanBoHoaHong>("EXEC GetListHoaDon_ChuaPhanBoCK @ID_ChiNhanhs, @ID_NhanVienLogin,  @TextSearch, " +
                "@LoaiChungTus, @DateFrom, @DateTo, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<SP_ReportDiscountInvoice_General> SP_ReportDiscountInvoice_General(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            string loaichungtus = "1,6,19,22,25";
            string idPhongBans = string.Empty;

            if (lstParam.LoaiChungTus != null && lstParam.LoaiChungTus.Count > 0)
            {
                loaichungtus = string.Join(",", lstParam.LoaiChungTus);
            }
            if (lstParam.DepartmentIDs != null && lstParam.DepartmentIDs.Count > 0)
            {
                idPhongBans = string.Join(",", lstParam.DepartmentIDs);
            }
            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            paramSql.Add(new SqlParameter("DepartmentIDs", idPhongBans ?? (object)DBNull.Value));
            paramSql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSql.Add(new SqlParameter("LoaiChungTus", loaichungtus));
            paramSql.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("DateTo", lstParam.DateTo));
            paramSql.Add(new SqlParameter("Status_ColumnHide", lstParam.Status_ColumnHide));// ẩn cột thứ ..
            paramSql.Add(new SqlParameter("StatusInvoice", lstParam.StatusInvoice));// hdTamluu = 2, hdHoanThanh = 1, all.0
            paramSql.Add(new SqlParameter("Status_DoanhThu", lstParam.TrangThai));// chỉ lọc theo doanhthu/thucthu/vnd > 0
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            List<SP_ReportDiscountInvoice_General> data = _db.Database.SqlQuery<SP_ReportDiscountInvoice_General>("EXEC ReportDiscountInvoice @ID_ChiNhanhs, @ID_NhanVienLogin, @DepartmentIDs, @TextSearch, " +
                "@LoaiChungTus, @DateFrom, @DateTo, @Status_ColumnHide, @StatusInvoice, @Status_DoanhThu, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<SP_ReportDiscountInvoice_Detail> SP_ReportDiscountInvoice_Detail(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            string loaichungtus = "1,6,19,22,25";
            string idPhongBans = string.Empty;
            if (lstParam.LoaiChungTus != null && lstParam.LoaiChungTus.Count > 0)
            {
                loaichungtus = string.Join(",", lstParam.LoaiChungTus);
            }
            if (lstParam.DepartmentIDs != null && lstParam.DepartmentIDs.Count > 0)
            {
                idPhongBans = string.Join(",", lstParam.DepartmentIDs);
            }
            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            paramSql.Add(new SqlParameter("DepartmentIDs", idPhongBans ?? (object)DBNull.Value));
            paramSql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSql.Add(new SqlParameter("LoaiChungTus", loaichungtus));
            paramSql.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("DateTo", lstParam.DateTo));
            paramSql.Add(new SqlParameter("Status_ColumnHide", lstParam.Status_ColumnHide));
            paramSql.Add(new SqlParameter("StatusInvoice", lstParam.StatusInvoice));
            paramSql.Add(new SqlParameter("Status_DoanhThu", lstParam.TrangThai));
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            List<SP_ReportDiscountInvoice_Detail> data = _db.Database.SqlQuery<SP_ReportDiscountInvoice_Detail>("EXEC ReportDiscountInvoice_Detail @ID_ChiNhanhs, @ID_NhanVienLogin, @DepartmentIDs, " +
                " @TextSearch, @LoaiChungTus, @DateFrom, @DateTo, @Status_ColumnHide, @StatusInvoice, @Status_DoanhThu, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<SP_ReportDiscountSales> SP_ReportDiscountSales(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            string trangthai = string.Empty;
            string idPhongBans = string.Empty;
            if (lstParam.TrangThai != 0)
            {
                trangthai = lstParam.TrangThai.ToString();
            }
            if (lstParam.DepartmentIDs != null && lstParam.DepartmentIDs.Count > 0)
            {
                idPhongBans = string.Join(",", lstParam.DepartmentIDs);
            }

            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVienLogin", lstParam.ID_NhanVienLogin));
            paramSql.Add(new SqlParameter("DepartmentIDs", idPhongBans ?? (object)DBNull.Value));
            paramSql.Add(new SqlParameter("FromDate", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("ToDate", lstParam.DateTo));
            paramSql.Add(new SqlParameter("TextSearch", lstParam.TextSearch));
            paramSql.Add(new SqlParameter("Status_DoanhThu", trangthai));
            paramSql.Add(new SqlParameter("CurrentPage", lstParam.CurrentPage));
            paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
            List<SP_ReportDiscountSales> data = _db.Database.SqlQuery<SP_ReportDiscountSales>("EXEC GetAll_DiscountSale @ID_ChiNhanhs, @ID_NhanVienLogin, @DepartmentIDs, " +
                " @FromDate, @ToDate, @TextSearch, @Status_DoanhThu, @CurrentPage, @PageSize", paramSql.ToArray()).ToList();
            return data;
        }

        public List<SP_ReportDiscountSales_Detail> SP_ReportDiscountSales_Detail(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVien", lstParam.TextSearch)); // mượn tạm trường để lưu ID_NhanVien
            paramSql.Add(new SqlParameter("timeStar", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("timeEnd", lstParam.DateTo));
            List<SP_ReportDiscountSales_Detail> data = _db.Database.SqlQuery<SP_ReportDiscountSales_Detail>("EXEC getList_ChietKhauNhanVienTheoDoanhSobyID @ID_ChiNhanhs, @ID_NhanVien, " +
                " @timeStar, @timeEnd", paramSql.ToArray()).ToList();
            return data;
        }

        public List<SP_ReportDiscountSales_Detail> DiscountSale_byIDNhanVien(ParamReportDiscount lstParam)
        {
            string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
            List<SqlParameter> paramSql = new List<SqlParameter>();
            paramSql.Add(new SqlParameter("IDChiNhanhs", idChiNhanhs));
            paramSql.Add(new SqlParameter("ID_NhanVien", lstParam.TextSearch)); // mượn tạm trường để lưu ID_NhanVien
            paramSql.Add(new SqlParameter("FromDate", lstParam.DateFrom));
            paramSql.Add(new SqlParameter("ToDate", lstParam.DateTo));
            List<SP_ReportDiscountSales_Detail> data = _db.Database.SqlQuery<SP_ReportDiscountSales_Detail>("EXEC DiscountSale_byIDNhanVien @IDChiNhanhs, @ID_NhanVien, " +
                " @FromDate, @ToDate", paramSql.ToArray()).ToList();
            return data;
        }

        // not use
        public List<ListInvoice_DiscountSales> CKTheoDoanhSo_GetListHoaDon(ParamReportDiscount lstParam)
        {
            try
            {
                string idChiNhanhs = string.Join(",", lstParam.LstIDChiNhanh);
                string idNhanVien = lstParam.ID_NhomHang;// muontam truong

                List<SqlParameter> paramSql = new List<SqlParameter>();
                paramSql.Add(new SqlParameter("ID_ChiNhanhs", idChiNhanhs));
                paramSql.Add(new SqlParameter("ID_NhanVien", idNhanVien));
                paramSql.Add(new SqlParameter("DateFrom", lstParam.DateFrom));
                paramSql.Add(new SqlParameter("DateTo", lstParam.DateTo));
                paramSql.Add(new SqlParameter("Currentpage", lstParam.CurrentPage));
                paramSql.Add(new SqlParameter("PageSize", lstParam.PageSize));
                List<ListInvoice_DiscountSales> data = _db.Database.SqlQuery<ListInvoice_DiscountSales>("EXEC CKTheoDoanhThu_GetListHoaDon @ID_ChiNhanhs, @DateFrom, @DateTo, " +
                    "@Currentpage, @PageSize", paramSql.ToArray()).ToList();
                return data;
            }
            catch (Exception ex)
            {
                CookieStore.WriteLog("libReport - CKTheoDoanhSo_GetListHoaDon: " + ex.Message + ex.InnerException);
                return new List<ListInvoice_DiscountSales>();
            }
        }
    }
}
