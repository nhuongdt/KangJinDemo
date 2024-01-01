using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace banhang24.Hellper
{
    public static class ReadLoadHellper
    {
        public static string GetSubDomain(this HtmlHelper html)
        {

            return "https://" + CookieStore.GetCookieAes("SubDomain").ToString() + ".open24.vn/";
        }
    }

    public class UrlPage
    {
        public const string Danhsachhanghoa = "Product";
        public const string TrangChu = "DashBoard";
        public const string TrangChuHRM = "DashBoardHRM";
        public const string ThietLapGia = "PriceBook";
        public const string PhieuDieuChinh = "CouponAdjustment";
        public const string TaoPhieuDieuChinh = "CouponAdjustment/Adjustment";
        public const string KiemKho = "StockTakes";
        public const string TonKho = "StockTakes/inventory";
        public const string HoaDon = "Invoices";
        public const string LoHangHoa = "Shipment";
        public const string DanhMucGiaVonTieuChuan = "DanhMucGiaVonTieuChuan";
        public const string ThemPhieuDieuChinh = "ThemPhieuDieuChinh";
        public const string DieuChinhGiaVon = "DieuChinhGiaVon";
        public const string TraHang = "Returns";
        public const string NhapHang = "PurchaseOrder";
        public const string DatHangNCC = "DatHangNCC";
        public const string DatHangNCCItem = "DatHangNCCItem";
        public const string NhapNoiBo = "NhapNoiBo";
        public const string NhapNoiBoItem = "NhapNoiBoItem";
        public const string NhapHangThua = "NhapHangThua";
        public const string NhapHangThuaItem = "NhapHangThuaItem";
        public const string NhapHangItem = "PurchaseOrderItem";
        public const string NhapHangItem2 = "PurchaseOrderItem2";
        public const string TraHangNhap = "PurchaseReturns";
        public const string TraHangNhapChiTiet = "PurchaseReturnsCT";
        public const string TraHangNhapChiTiet2 = "PurchaseReturnsCT2";
        public const string ChuyenHang = "Transfers";
        public const string ChuyenHangChiTiet = "TransfersCT";
        public const string ChuyenHangChiTiet2 = "TransfersCT2";
        public const string XuatHuy = "DamageItems";
        public const string PhieuXuatHuy = "DamageItemsCT";
        public const string XuatKhoChiTiet = "XuatKhoChiTiet";
        public const string DatHang = "Order";
        public const string GoiDichVu = "ServicePackage";
        public const string DichVuSMS = "ServiceSms";
        public const string KhachHang = "Customers";
        public const string NhaCungCap = "Suppliers";
        public const string TuVan = "Advisory";
        public const string LichHen = "Schedule";
        public const string LichHen1 = "Schedule1";
        public const string NguoiLienHe = "UserContact";
        public const string Congviec = "Work";
        public const string HoaHongKhachGioiThieu = "HoaHongKhachGioiThieu";
        //public const string Congviec1 = "Work1";
        public const string PhanHoi = "Feedback";
        public const string SoQuy = "CashFlow";
        public const string SoQuy2 = "CashFlow2";
        public const string PhieuThu = "ReceiptVoucher";
        public const string PhieuChi = "PaymentVoucher";
        public const string BC_CuoiNgay = "EndOfDayReport";
        public const string BC_BanHang = "SaleReport";
        public const string BC_DatHang = "OrderReport";
        public const string BC_HangHoa = "ProductReport";
        public const string BC_KhachHang = "CustomerReport";
        public const string BC_NhaCungCap = "SupplierReport";
        public const string BC_NhanVien = "UserReport";
        public const string BC_TaiChinh = "FinancialReport";
        public const string R_BanHang = "TotalSalesReport";
        public const string R_DatHang = "OrdersReport";
        public const string R_ChietKhau = "DiscountReport";
        public const string R_ChietKhau1 = "DiscountReport1";
        public const string R_TaiChinh = "FinancialsReport";
        public const string R_NhapHang = "ImportGoodsReport";
        public const string R_TheGiaTri = "ValueCardReport";
        public const string R_Kho = "WarehouseReport";
        public const string R_NhanVien = "UsersReport";
        public const string R_GoiDichVu = "ServicePackageReport";
        public const string NhanVien = "User";
        public const string ChietKhau = "Discount";
        public const string ChietKhauNew = "DiscountNew";
        public const string CD_ThietLapCuaHang = "PosParameter";
        public const string CD_QuanLyMauIn = "PrintTemplates";
        public const string CD_NguoiDung = "SetUpUsers";
        public const string CD_NguoiDung_2 = "SetUpUsers2";
        public const string CD_ChiNhanh = "Branches";
        public const string CD_LichSuHD = "AuditTrail";
        public const string CD_SoDo = "Department";
        public const string CD_KhuyenMai = "Promotion";
        public const string CD_TaiKhoan = "Profile";
        public const string CaLamViec = "ShiftsWork";
        public const string PhanCongCaLamViec = "ShareShiftsWork";
        public const string OptinForm = "OptinForm";
        public const string OptinFormKH = "AddOptinFormKH";
        public const string OptinFormLH = "AddOptinFormLH";
        public const string datlich = "datlich";
        public const string NapTienTheGiaTri = "RechargeValueCard";
        public const string DanhSachDangKyOptinForm = "DanhSachDangKyOptinForm";
        public const string Calendar = "Calendar";
        public const string KyTinhCong = "CycleWork";
        public const string ChamCong = "timekeeping";
        public const string ChamCong2_1 = "timekeeping2";
        public const string BangLuong = "payroll";
        public const string BangLuongChiTiet = "SalaryDetail";
        public const string LoaiBaoHiem = "typeofinsurance";
        public const string KhenThuongKyLuat = "bonusanddiscipline";
        public const string QuanLyMayChamCong = "QuanLyMayChamCong";
        public const string TaiDuLieuMayChamCong = "TaiDuLieuMayChamCong";
        public const string TongQuanNhanSu = "TongQuanNhanSu";
        public const string Nhahang = "NhaHang";
        public const string QuanLyCheckIn = "QuanLyCheckIn";

        //const Gara
        public const string BaoGia = "Quotation";
        public const string DanhMucXe = "DanhMucXe";
        public const string BanLamViec = "BanLamViec";
        public const string Gara = "Gara";
        public const string BaoHiem = "BaoHiem";
        public const string DanhSachXe = "DanhSachXe";
        public const string DanhSachPhieuTiepNhan = "DanhSachPhieuTiepNhan";
        public const string HoaDonSuaChua = "HoaDonSuaChua";
        public const string HoaDonBaoHanh = "HoaDonBaoHanh";
        public const string TongQuanGara = "TongQuanGara";
        public const string BaoCaoSuaChua = "BaoCaoSuaChua";
        public const string BaoCaoHoatDongXe = "BaoCaoHoatDongXe";
        public const string PhieuBanGiaoXe = "PhieuBanGiaoXe";
        public const string Gara_LichNhacBaoDuong = "Gara_LichNhacBaoDuong";


        public const string Title_HangHoa = "Hàng hóa";
        public const string Title_GiaoDich = "Giao dịch";
        public const string Title_DoiTac = "Đối tác";
        public const string Title_BaoCao = "Báo cáo";
        public const string Title_NhanSu = "Nhân sự";
        public const string Title_MarKeting = "MarKeting";
        public const string Title_CaiDat = "Cài đặt";
        public const string Title_TongQuan = "Tổng quan";
        public const string Title_SoQuy = "Sổ quỹ";
        public const string Title_NhaHang = "Giao dịch";
        public const string Tilte_QuanLyMayChamCong = "Quản lý máy chấm công";
        public const string Tilte_TaiDuLieuMayChamCong = "Tải dữ liệu máy chấm công";
        public const string Tilte_TongQuanNhanSu = "Quản lý nhân sự";
        public const string Title_Gara = "Gara";

    }

    public class RoleKey
    {
        public const string HoaDon_XemDs = "HoaDon_XemDs";
        public const string TraHang_XemDs = "TraHang_XemDs";
        public const string NhapHang_XemDs = "NhapHang_XemDs";
        public const string NhapHang_ThemMoi = "NhapHang_ThemMoi";
        public const string TraHangNhap_XemDs = "TraHangNhap_XemDs";
        public const string TraHangNhap_ThemMoi = "TraHangNhap_ThemMoi";
        public const string ChuyenHang_XemDs = "ChuyenHang_XemDs";
        public const string ChuyenHang_ThemMoi = "ChuyenHang_ThemMoi";
        public const string XuatHuy_XemDs = "XuatHuy_XemDs";
        public const string DatHang_XemDs = "DatHang_XemDs";
        public const string NhaCungCap_XemDs = "NhaCungCap_XemDs";
        public const string KhachHang_XemDs = "KhachHang_XemDs";
        public const string HangHoa_XemDS = "HangHoa_XemDs";
        public const string ThietLapGia_XemDS = "ThietLapGia_XemDS";
        public const string KiemKho_XemDs = "KiemKho_XemDs";
        public const string PhongBan_XemDs = "PhongBan_XemDs";

        public const string SoQuy_ThemMoi = "SoQuy_ThemMoi";
        public const string SoQuy_CapNhat = "SoQuy_CapNhat";
        public const string SoQuy_Xoa = "SoQuy_Xoa";
        public const string SoQuy_XemDs = "SoQuy_XemDs";
        public const string SoQuy_XemDS_PhongBan = "SoQuy_XemDS_PhongBan";
        public const string SoQuy_XemDS_HeThong = "SoQuy_XemDS_HeThong";
        public const string SoQuy_XuatFile = "SoQuy_XuatFile";

        public const string PhieuDieuChinh = "PhieuDieuChinh";
        public const string PhieuDieuChinh_XemDS = "PhieuDieuChinh_XemDS";
        public const string PhieuDieuChinh_ThemMoi = "PhieuDieuChinh_ThemMoi";
        public const string DanhMucGiaVonTieuChuan = "DanhMucGiaVonTieuChuan";

        public const string MauIn_xemDs = "MauIn_XemDS";
        public const string NguoiDung_XemDS = "NguoiDung_XemDS";
        public const string ChiNhanh_XemDs = "ChiNhanh_XemDs";
        public const string NhanVien_XemDs = "NhanVien_XemDs";
        public const string ChietKhau_XemDs = "HoaHongNhanVien";
        public const string TuVan_XemDs = "TuVan_XemDs";
        public const string PhanHoi_XemDs = "PhanHoi_XemDs";
        public const string LichHen_XemDs = "LichHen_XemDs";
        public const string HeThong_ThietLap = "ThietLapCuaHang";
        public const string KhuyenMai_XemDs = "KhuyenMai_XemDs";
        public const string GoiDichVu_XemDs = "GoiDichVu_XemDS";
        public const string NhaBep_TruyCap = "NhaBep_TruyCap";

        public const string HoaDon_XemDs_PhongBan = "HoaDon_XemDS_PhongBan";
        public const string DatHang_XemDS_PhongBan = "DatHang_XemDS_PhongBan";
        public const string TraHang_XemDS_PhongBan = "TraHang_XemDS_PhongBan";
        public const string GoiDichVu_XemDS_PhongBan = "GoiDichVu_XemDS_PhongBan";
        public const string ChuyenHang_XemDS_PhongBan = "ChuyenHang_XemDS_PhongBan";
        public const string NhapHang_XemDS_PhongBan = "NhapHang_XemDS_PhongBan";
        public const string TraHangNhap_XemDS_PhongBan = "TraHangNhap_XemDS_PhongBan";
        public const string XuatHuy_XemDS_PhongBan = "XuatHuy_XemDS_PhongBan";
        public const string KhachHang_XemDS_PhongBan = "KhachHang_XemDS_PhongBan";
        public const string NhaCungCap_XemDS_PhongBan = "NhaCungCap_XemDS_PhongBan";
        public const string TuVan_XemDS_PhongBan = "TuVan_XemDS_PhongBan";
        public const string LichHen_XemDS_PhongBan = "LichHen_XemDS_PhongBan";
        public const string LienHe_XemDS_PhongBan = "LienHe_XemDS_PhongBan";
        public const string CongViec_XemDS_PhongBan = "CongViec_XemDS_PhongBan";


        public const string HoaDon_XemDs_HeThong = "HoaDon_XemDS_HeThong";
        public const string DatHang_XemDS_HeThong = "DatHang_XemDS_HeThong";
        public const string TraHang_XemDS_HeThong = "TraHang_XemDS_HeThong";
        public const string GoiDichVu_XemDS_HeThong = "GoiDichVu_XemDS_HeThong";
        public const string ChuyenHang_XemDS_HeThong = "ChuyenHang_XemDS_HeThong";
        public const string NhapHang_XemDS_HeThong = "NhapHang_XemDS_HeThong";
        public const string TraHangNhap_XemDS_HeThong = "TraHangNhap_XemDS_HeThong";
        public const string XuatHuy_XemDS_HeThong = "XuatHuy_XemDS_HeThong";
        public const string KhachHang_XemDS_HeThong = "KhachHang_XemDS_HeThong";
        public const string NhaCungCap_XemDS_HeThong = "NhaCungCap_XemDS_HeThong";
        public const string TuVan_XemDS_HeThong = "TuVan_XemDS_HeThong";
        public const string LichHen_XemDS_HeThong = "LichHen_XemDS_HeThong";
        public const string LienHe_XemDS_HeThong = "LienHe_XemDS_HeThong";
        public const string CongViec_XemDS_HeThong = "CongViec_XemDS_HeThong";

        public const string HoaDon = "HoaDon";
        public const string DatHang = "DatHang";
        public const string TraHang = "TraHang";
        public const string GoiDichVu = "GoiDichVu";
        public const string ChuyenHang = "ChuyenHang";
        public const string NhapHang = "NhapHang";
        public const string TraHangNhap = "TraHangNhap";
        public const string XuatHuy = "XuatHuy";
        public const string KhachHang = "KhachHang";
        public const string NhaCungCap = "NhaCungCap";
        public const string TuVan = "TuVan";
        public const string LichHen = "LichHen";
        public const string LienHe = "LienHe";
        public const string CongViec = "CongViec";
        public const string PhanHoi = "PhanHoi";

        public const string TheGiaTri_ThemMoi = "TheGiaTri_ThemMoi";
        public const string TheGiaTri_ThayDoiThoiGian = "TheGiaTri_ThayDoiThoiGian";
        public const string TheGiaTri_CapNhat = "TheGiaTri_CapNhat";
        public const string TheGiaTri_Xoa = "TheGiaTri_Xoa";
        public const string TheGiaTri_XemDS = "TheGiaTri_XemDS";
        public const string TheGiaTri_XemDS_PhongBan = "TheGiaTri_XemDS_PhongBan";
        public const string TheGiaTri_XemDS_HeThong = "TheGiaTri_XemDS_HeThong";
        public const string TheGiaTri_XuatFile = "TheGiaTri_XuatFile";
        public const string TheGiaTri_In = "TheGiaTri_In";

        public const string HoaHongKhachGioiThieu_XemDS = "HoaHongKhachGioiThieu_XemDS";
    }

    public class RoleMauIn
    {
        public const string DatHang_View = "DatHang_XuatFile";
        public const string DatHang_Insert = "DatHang_ThemMoi";
        public const string GoiDichVu_View = "GoiDichVu_XemDS";
        public const string GoiDichVu_Insert = "GoiDichVu_ThemMoi";
        public const string HoaDon_View = "HoaDon_XemDs";
        public const string HoaDon_Insert = "HoaDon_ThemMoi";
        public const string TraHang_View = "TraHang_XuatFile";
        public const string TraHang_Insert = "TraHang_ThemMoi";
        public const string NhapHang_View = "NhapHang_XuatFile";
        public const string NhapHang_Insert = "NhapHang_ThemMoi";
        public const string TraHangNhap_View = "TraHangNhap_XuatFile";
        public const string TraHangNhap_Insert = "TraHangNhap_ThemMoi";
        public const string SoQuy_View = "SoQuy_XuatFile";
        public const string SoQuy_Insert = "SoQuy_ThemMoi";
        public const string ChuyenHang_View = "ChuyenHang_XuatFile";
        public const string ChuyenHang_Insert = "ChuyenHang_ThemMoi";
        public const string XuatHuy_View = "XuatHuy_XuatFile";
        public const string XuatHuy_Insert = "XuatHuy_ThemMoi";
        public const string MauIn_xemDs = "MauIn_xemDs";
        public const string MauIn_ThemMoi = "MauIn_ThemMoi";
        public const string MauIn_CapNhat = "MauIn_CapNhat";
        public const string MauIn_SaoChep = "MauIn_SaoChep";
        public const string MauIn_Xoa = "MauIn_Xoa";

    }

    public class RoleNhanVien
    {
        public const string NS_LoaiLuong_CapNhat = "NS_LoaiLuong_CapNhat";
        public const string NS_LoaiLuong_ThemMoi = "NS_LoaiLuong_ThemMoi";
        public const string NS_LoaiLuong_Xoa = "NS_LoaiLuong_Xoa";
        public const string NS_PhongBan_ThemMoi = "NS_PhongBan_ThemMoi";
        public const string NS_PhongBan_CapNhat = "NS_PhongBan_CapNhat";
        public const string NS_PhongBan_Xoa = "NS_PhongBan_Xoa";
        public const string NhanVien_Import = "NhanVien_Import";   //import các file 
        public const string NhanVien_XuatFile = "NhanVien_XuatFile"; // export các file
        public const string NhanVien_Update = "NhanVien_CapNhat";
        public const string NhanVien_Insert = "NhanVien_ThemMoi";
        public const string NhanVien_Delete = "NhanVien_Xoa";
    }

    public class RoleNhanSu
    {
        public const string CaLamViec_XemDS = "CaLamViec_XemDS";
        public const string CaLamViec_ThemMoi = "CaLamViec_ThemMoi";
        public const string CaLamViec_CapNhat = "CaLamViec_CapNhat";
        public const string CaLamViec_Xoa = "CaLamViec_Xoa";
        public const string CaLamViec_NhapFile = "CaLamViec_NhapFile";
        public const string CaLamViec_XuatFile = "CaLamViec_XuatFile";

        public const string PhieuPhanCa_XemDS = "PhieuPhanCa_XemDS";
        public const string PhieuPhanCa_CapNhat = "PhieuPhanCa_CapNhat";
        public const string PhieuPhanCa_ThemMoi = "PhieuPhanCa_ThemMoi";
        public const string PhieuPhanCa_Xoa = "PhieuPhanCa_Xoa";
        public const string PhieuPhanCa_XuatFile = "PhieuPhanCa_XuatFile";

        public const string KyHieuCong_XemDS = "KyHieuCong_XemDS";
        public const string KyHieuCong_ThemMoi = "KyHieuCong_ThemMoi";
        public const string KyHieuCong_CapNhat = "KyHieuCong_CapNhat";
        public const string KyHieuCong_Xoa = "KyHieuCong_Xoa";

        public const string NgayNghiLe_XemDS = "NgayNghiLe_XemDS";
        public const string NgayNghiLe_ThemMoi = "NgayNghiLe_ThemMoi";
        public const string NgayNghiLe_CapNhat = "NgayNghiLe_CapNhat";
        public const string NgayNghiLe_Xoa = "NgayNghiLe_Xoa";

        public const string KyTinhCong_XemDS = "KyTinhCong_XemDS";
        public const string KyTinhCong_ThemMoi = "KyTinhCong_ThemMoi";
        public const string KyTinhCong_CapNhat = "KyTinhCong_CapNhat";
        public const string KyTinhCong_Xoa = "KyTinhCong_Xoa";
        public const string KyTinhCong_ChotCong = "KyTinhCong_ChotCong";

        public const string ChamCong_XemDS = "ChamCong_XemDS";
        public const string ChamCong_AddHoSo = "ChamCong_AddHoSo";// themmoi
        public const string ChamCong_XuatFile = "ChamCong_XuatFile";
        public const string ChamCong_ChamCong = "ChamCong_ChamCong";// capnhat
        public const string ChamCong_GuiBangCong = "ChamCong_GuiBangCong";

        public const string BangLuong_XemDS = "BangLuong_XemDS";
        public const string BangLuong_ThemMoi = "BangLuong_ThemMoi";
        public const string BangLuong_CapNhat = "BangLuong_CapNhat";
        public const string BangLuong_HuyBo = "BangLuong_HuyBo";
        public const string BangLuong_ThanhToan = "BangLuong_ThanhToan";
        public const string BangLuong_XuatFile = "BangLuong_XuatFile";
        public const string BangLuong_PheDuyet = "BangLuong_PheDuyet";
        public const string BangLuong_MoLai = "BangLuong_MoLai"; // mở lại bảng lương đã chốt

        public const string BangLuongChiTiet_XemThongTin = "BangLuongChiTiet_XemThongTin";
        public const string BangLuongChiTiet_ThayDoiThietLapLuong = "BangLuongChiTiet_ThayDoiThietLapLuong"; // thaydoi hesoluong, luongcoban, hesoluong
        public const string BangLuongChiTiet_SuaDoi = "BangLuongChiTiet_SuaDoi"; // thaydoi soca/ngay apdung so voi thucte

        public const string ThietLapLuong_ThemMoi = "ThietLapLuong_ThemMoi";
        public const string ThietLapLuong_CapNhat = "ThietLapLuong_CapNhat";
        public const string ThietLapLuong_Xoa = "ThietLapLuong_Xoa";
        public const string ThietLapLuong_SaoChep = "ThietLapLuong_SaoChep";
        public const string ThietLapLuong_XemDS = "ThietLapLuong_XemDS";

        public const string BangCong_XemDS = "BangCong_XemDS";
        public const string BangCong_XemDS_PhongBan = "BangCong_XemDS_PhongBan";
        public const string BangCong_XemDS_HeThong = "BangCong_XemDS_HeThong";

        public const string KhenThuongKyLuat_XemDS = "KhenThuongKyLuat_XemDS";
        public const string KhenThuongKyLuat_ThemMoi = "KhenThuongKyLuat_ThemMoi";
        public const string KhenThuongKyLuat_CapNhat = "KhenThuongKyLuat_CapNhat";
        public const string KhenThuongKyLuat_XuatFile = "KhenThuongKyLuat_XuatFile";
        public const string KhenThuongKyLuat_Xoa = "KhenThuongKyLuat_Xoa";
    }
    public class fliepath
    {
        public const string nhanvien = "nhanvien";
    }

    public class commonEnum
    {
        public static class MauInTeamPlates
        {
            public const string DatHang = "BG";
            public const string HoaDon = "HDBL";
            public const string GoiDV = "GDV";
            public const string TraHang = "TH";
            public const string DoiTraHang = "DTH";
            public const string NhapHang = "PNK";
            public const string TraHangNhap = "THNCC";
            public const string ChuyenHang = "CH";
            public const string PhieuThu = "SQPT";
            public const string PhieuChi = "SQPC";
            public const string XuatHuy = "XH";
            public const string kiemKho = "PKK";
            public const string MaVach = "IMV";
            public const string DieuChinh = "DCGV";
            public const string TheGiaTri = "TGT";
            public const string PhieuTiepNhan = "PTN";
            public const string PhieuXuatXuong = "XuatXuong";
            public const string PhieuLuong = "BL";// phải get đúng in DM_LoaiChungTu
        }

        public static Dictionary<string, string> DanhSachTenMauIn = new Dictionary<string, string>()
        {
            {MauInTeamPlates.DatHang.ToString(),"Đặt hàng" },
            {MauInTeamPlates.HoaDon.ToString(),"Hóa đơn" },
            {MauInTeamPlates.TraHang.ToString(),"Trả hàng" },
            {MauInTeamPlates.DoiTraHang.ToString(),"Đổi trả hàng" },
            {MauInTeamPlates.NhapHang.ToString(),"Nhập hàng" },
            {MauInTeamPlates.TraHangNhap.ToString(),"Trả hàng nhập" },
            {MauInTeamPlates.ChuyenHang.ToString(),"Chuyển hàng" },
            {MauInTeamPlates.PhieuThu.ToString(),"Phiếu thu" },
            {MauInTeamPlates.PhieuChi.ToString(),"Phiếu chi" },
            {MauInTeamPlates.XuatHuy.ToString(),"Xuất hủy" },
            {MauInTeamPlates.kiemKho.ToString(),"Kiểm kho" },
            {MauInTeamPlates.DieuChinh.ToString(),"Điều chỉnh" },
            {MauInTeamPlates.GoiDV.ToString(),"Gói dịch vụ" },
            {MauInTeamPlates.TheGiaTri.ToString(),"Thẻ giá trị" },
            {MauInTeamPlates.PhieuTiepNhan.ToString(),"Phiếu tiếp nhận" },
            {MauInTeamPlates.PhieuLuong.ToString(),"Phiếu lương" }

        };

        public static Dictionary<string, string> DanhSachMauInA4 = new Dictionary<string, string>()
        {
            {MauInTeamPlates.DatHang.ToString(),"DatHang.txt" },
            {MauInTeamPlates.HoaDon.ToString(),"HoaDonBanLe.txt" },
            {MauInTeamPlates.TraHang.ToString(),"TraHang.txt" },
            {MauInTeamPlates.DoiTraHang.ToString(),"DoiTraHang.txt" },
            {MauInTeamPlates.NhapHang.ToString(),"NhapHang.txt" },
            {MauInTeamPlates.TraHangNhap.ToString(),"TraHangNhap.txt" },
            {MauInTeamPlates.ChuyenHang.ToString(),"Chuyenhang.txt" },
            {MauInTeamPlates.PhieuThu.ToString(),"PhieuThu.txt" },
            {MauInTeamPlates.PhieuChi.ToString(),"PhieuChi.txt" },
            {MauInTeamPlates.XuatHuy.ToString(),"XuatHuy.txt" },
            {MauInTeamPlates.kiemKho.ToString(),"KiemKho.txt" },
            {MauInTeamPlates.DieuChinh.ToString(),"DieuChinh.txt" },
            {MauInTeamPlates.GoiDV.ToString(),"GoiDichVu.txt" },
            {MauInTeamPlates.MaVach.ToString(),"InMaVach.txt" },
            {MauInTeamPlates.TheGiaTri.ToString(),"TheGiaTri.txt" },
            {MauInTeamPlates.PhieuTiepNhan.ToString(),"PhieuTiepNhan.txt" },
            {MauInTeamPlates.PhieuLuong.ToString(),"PhieuLuong.txt" },
        };

        public static Dictionary<string, string> DanhSachMauInK80 = new Dictionary<string, string>()
        {
            {MauInTeamPlates.DatHang.ToString(),"K80_DatHang.txt" },
              {MauInTeamPlates.HoaDon.ToString(),"K80_HoaDonBanLe.txt" },
                {MauInTeamPlates.TraHang.ToString(),"K80_TraHang.txt" },
                  {MauInTeamPlates.DoiTraHang.ToString(),"K80_DoiTraHang.txt" },
                    {MauInTeamPlates.NhapHang.ToString(),"K80_NhapHang.txt" },
                      {MauInTeamPlates.TraHangNhap.ToString(),"K80_TraHangNhap.txt" },
                        {MauInTeamPlates.ChuyenHang.ToString(),"K80_Chuyenhang.txt" },
                          {MauInTeamPlates.PhieuThu.ToString(),"K80_PhieuThu.txt" },
                          {MauInTeamPlates.PhieuChi.ToString(),"K80_PhieuChi.txt" },
                          {MauInTeamPlates.XuatHuy.ToString(),"K80_XuatHuy.txt" },
                            {MauInTeamPlates.kiemKho.ToString(),"K80_KiemKho.txt" },
                             {MauInTeamPlates.DieuChinh.ToString(),"K80_DieuChinh.txt" },
                             {MauInTeamPlates.GoiDV.ToString(),"K80_GoiDichVu.txt" },
                             {MauInTeamPlates.MaVach.ToString(),"K80_InMaVach.txt" },
                             {MauInTeamPlates.TheGiaTri.ToString(),"K80_TheGiaTri.txt" },
              {MauInTeamPlates.PhieuTiepNhan.ToString(),"K80_PhieuTiepNhan.txt" },
        };

        public static Dictionary<string, string> Gara_DanhSachMauHoaDon = new Dictionary<string, string>()
        {
              {TypeMauin.hanghoa_dv.ToString(),"HoaDon_HangHoaDichVu.txt" },
              {TypeMauin.nhomhang.ToString(),"HoaDon_NhomHang.txt" },
              {TypeMauin.combo.ToString(),"Combo.txt" },
              {TypeMauin. HHDV_vaNhom.ToString(),"HoaDon_HangHoaDichVu_Nhom.txt" },
        };

        public enum TypeMauin
        {
            dangdung = 0,
            a4,
            k80,
            hanghoa_dv,
            nhomhang,
            combo,
            HHDV_vaNhom
        }

        public static Dictionary<int, string> DanhSachTypeMauIn = new Dictionary<int, string>()
        {
            { (int)TypeMauin.dangdung,"Đang dùng" },
              {(int)TypeMauin.a4,"A4" },
                {(int)TypeMauin.k80,"K80" },
                 {(int)TypeMauin.hanghoa_dv,"Theo hàng hóa - dịch vụ" },
                {(int)TypeMauin.nhomhang,"Theo nhóm hàng" },
                {(int)TypeMauin.combo,"Combo" },
                {(int)TypeMauin.HHDV_vaNhom,"Theo hàng hóa dịch vụ - và nhóm" },
        };
        public static Dictionary<int, string> Gara_MauInHoaDon = new Dictionary<int, string>()
        {
            { (int)TypeMauin.dangdung,"Đang dùng" },
              {(int)TypeMauin.a4,"A4" },
                {(int)TypeMauin.k80,"K80" },
            {(int)TypeMauin.hanghoa_dv,"Theo hàng hóa - dịch vụ" },
                {(int)TypeMauin.nhomhang,"Theo nhóm hàng" },
                {(int)TypeMauin.combo,"Combo" },
                {(int)TypeMauin. HHDV_vaNhom,"Theo hàng hóa dịch vụ - và nhóm" },
        };

        public enum TypeDate
        {
            ddmmyyyy = 0,
            mmyyyy,
            yyyy,
        }
        public static Dictionary<int, string> listTypeDate = new Dictionary<int, string>()
        {
            { (int)TypeDate.ddmmyyyy,"Theo ngày/tháng/năm" },
              {(int)TypeDate.mmyyyy,"Theo tháng/năm" },
                {(int)TypeDate.yyyy,"Theo năm" }
        };

        public enum GroupReport
        {
            banhang = 1,
            dathang,
            nhaphang,
            taichinh,
            kho,
            chietkhau,
            goidichvu,
            nhanvien,
            hoahong
        }
        public enum CheckBoxColum
        {
            lienhe = 1,
            goiDichVu = 2,
            listKhachHang = 3,
            listNhaCungCap = 4,
            RpValueCardBalance = 5,
            RpValueCardHisUsed = 6,
            RpDiscountProduct = 7,
            RpDiscountProduct_Detail = 8,
            RpDiscountInvoice = 9,
            RpDiscountInvoice_Detail = 10,
            RpDioscountRevenue = 11,
            RpDioscountAll = 12,
            RpValueCardServiveUsed = 13,
            lstsoquy = 14,
        }
        public enum TypeReport
        {
            tonghop = 1,
            chitiet,
            nhomhang,
            khachhang,
            nhomkhachhang,
            nhanvien,
            hangtralai,
            loinhuan,
            hangkhuyenmai,
            nhacungcap = 10,
            trahangnhap,
            thutien,
            chitien,
            soquy,
            soquy2,
            soquy3,
            taichinh,
            nhapxuatton,
            xuattonchitiet,
            dieuchuyenhhXC = 20,
            dieuchuyenhhNC,
            dieuchuyenhhCT,
            thnhapkhoHH,
            thnhapkhoGD,
            thxuatkhoHH,
            thxuatkhoGD,
            duchitiet,
            nhatkysdct,
            nhatkysdth,
            tonchuasudung = 30,
            rnv_th,
            rnv_hopdong,
            rnv_baohiem,
            rnv_tuoi,
            rnv_kyluat,
            rnv_phucap,
            rnv_miengiamthue,
            rnv_daotao,
            rnv_congtac,
            rnv_giadinh = 40,
            rnv_suckhoe,
            xuatkhohhtheodinhluong,
            tonkhoth = 43,
            phantichthuchi,
            hanghoath,
            hanghoact,
            hoadonth,
            hoadonct,
            doanhso,
            hoahongth,
            hoahongct,
            khachhang_tansuat,
        }
        public enum TypeColumnNhanVien
        {
            ngaysinh = 1,
            gioitinh,
            noisinh,
            dienthoai,
            email,
            cmnd,
            ngaycap,
            noicap,
            dantoc,
            tongiao,
            honnhan,
            quoctich,
            hkdiachi,
            hkxaphuong,
            hkquanhuyen,
            hktinhthanh,
            ttdiachi,
            ttxaphuong,
            ttquanhuyen,
            tttinhthanh,
            phongban,
            ngayvaolamviec,
            trangthai,
        }
        public enum TypeReportTotal
        {
            Total_nhomhang = 1,
            Total_mahang,
            Total_tenhang,
            Total_soluong,
            Total_thanhtien,
            Total_tienvon,
            Total_giamgia,
            Total_lailo,
            Total_lohang,
            Total_donvitinh,
            Total_doanhthuthuan,
            Total_tienthue,
        }

        public static Dictionary<string, string> listNameReportTotal = new Dictionary<string, string>()
        {
            { TypeReportTotal.Total_nhomhang.ToString(),"Nhóm hàng" },
              {TypeReportTotal.Total_mahang.ToString(),"Mã hàng" },
                {TypeReportTotal.Total_tenhang.ToString(),"Tên hàng" },
                  { TypeReportTotal.Total_donvitinh.ToString(),"Đvt" },
                    { TypeReportTotal.Total_lohang.ToString(),"Lô hàng" },
                      { TypeReportTotal.Total_soluong.ToString(),"Số lượng" },
              {TypeReportTotal.Total_thanhtien.ToString(),"Thành tiền" },
              {TypeReportTotal.Total_giamgia.ToString(),"Giảm giá" },
              {TypeReportTotal.Total_doanhthuthuan.ToString(),"Doanh thu" },
              {TypeReportTotal.Total_tienthue.ToString(),"Tiền thuế" },
                {TypeReportTotal.Total_tienvon.ToString(),"Tiền vốn" },
                {TypeReportDetail.Detail_chiphi.ToString(),"Chi phí" },
                {TypeReportTotal.Total_lailo.ToString(),"Lãi lỗ" },
        };

        public enum TypeReportDetail
        {
            Detail_machungtu = 1,
            Detail_ngaychungtu,
            Detail_mahang,
            Detail_tenhang,
            Detail_tenhangthaythe,
            Detail_soluong,
            Detail_giaban,
            Detail_chietkhau,
            Detail_thanhtien,
            Detail_giavon,
            Detail_tienvon,
            Detail_giamgiahd,
            Detail_lailo,
            Detail_nhanvien,
            Detail_makhach,
            Detail_tenkhach,
            Detail_donvitinh,
            Detail_lohang,
            Detail_thoigianbh,
            Detail_hanbaohanh,
            Detail_trangthai,
            Detail_ngayconhan,
            Detail_ghichu,
            Detail_nhomkhach,
            Detail_nguonkhach,
            Detail_dienthoai,
            Detail_gioitinh,
            Detail_nguoigioithieu,
            Detail_nhomhang,
            Detail_doanhthuthuan,
            Detail_tienthue,
            Detail_chiphi,
            Detail_maDinhDanhDV,
        }

        public static Dictionary<string, string> listNameReportDetail = new Dictionary<string, string>()
        {   {TypeReportDetail.Detail_makhach.ToString(),"Mã khách hàng" },
               { TypeReportDetail.Detail_tenkhach.ToString(),"Tên khách hàng" },
               { TypeReportDetail.Detail_nhomkhach.ToString(),"Nhóm khách" },
               { TypeReportDetail.Detail_nguonkhach.ToString(),"Nguồn khách" },
               { TypeReportDetail.Detail_dienthoai.ToString(),"Điện thoại" },
               { TypeReportDetail.Detail_nguoigioithieu.ToString(),"Người giới thiệu" },
            {TypeReportDetail.Detail_machungtu.ToString(),"Mã chứng từ" },
               { TypeReportDetail.Detail_ngaychungtu.ToString(),"Ngày chứng từ" },
              {TypeReportDetail.Detail_nhomhang.ToString(),"Nhóm hàng" },
              {TypeReportDetail.Detail_mahang.ToString(),"Mã hàng" },
                {TypeReportDetail.Detail_tenhang.ToString(),"Tên hàng" },
                {TypeReportDetail.Detail_tenhangthaythe.ToString(),"Tên hàng hóa thay thế" },
                   { TypeReportDetail.Detail_donvitinh.ToString(),"Đvt" },
                    { TypeReportDetail.Detail_lohang.ToString(),"Lô hàng" },
                  { TypeReportDetail.Detail_soluong.ToString(),"Số lượng" },
                   { TypeReportDetail.Detail_giaban.ToString(),"Giá bán" },
                    { TypeReportDetail.Detail_chietkhau.ToString(),"Chiết khấu" },
              {TypeReportDetail.Detail_thanhtien.ToString(),"Thành tiền" },
              {TypeReportDetail.Detail_giamgiahd.ToString(),"Giảm giá HĐ" },
              {TypeReportDetail.Detail_doanhthuthuan.ToString(),"Doanh thu" },
              {TypeReportDetail.Detail_tienthue.ToString(),"Tiền thuế" },
                   {TypeReportDetail.Detail_giavon.ToString(),"Giá vốn" },
                {TypeReportDetail.Detail_tienvon.ToString(),"Tiền vốn" },
                {TypeReportDetail.Detail_chiphi.ToString(),"Chi phí" },
                {TypeReportDetail.Detail_lailo.ToString(),"Lãi lỗ" },
                      {TypeReportDetail.Detail_thoigianbh.ToString(),"Thời gian bảo hành" },
                      {TypeReportDetail.Detail_hanbaohanh.ToString(),"Hạn bảo hành" },
                      {TypeReportDetail.Detail_trangthai.ToString(),"Trạng thái" },
                      {TypeReportDetail.Detail_ngayconhan.ToString(),"Ngày còn hạn/hết hạn" },
                 {ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
                 {ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
                   {TypeReportDetail.Detail_ghichu.ToString(),"Ghi chú" },
        };
        public static Dictionary<string, string> listNameReportDetail_DinhDanhDV = new Dictionary<string, string>()
        {
               { TypeReportDetail.Detail_ngaychungtu.ToString(),"Ngày sử dụng DV" },
            {TypeReportDetail.Detail_makhach.ToString(),"Mã khách hàng" },
               { TypeReportDetail.Detail_tenkhach.ToString(),"Tên khách hàng" },
               { TypeReportDetail.Detail_dienthoai.ToString(),"Điện thoại" },
                {TypeReportDetail.Detail_mahang.ToString(),"Mã dịch vụ" },
              {TypeReportDetail.Detail_maDinhDanhDV.ToString(),"Mã định danh dịch vụ" },
            {TypeReportDetail.Detail_machungtu.ToString(),"Mã chứng từ" },
                {TypeReportDetail.Detail_tenhang.ToString(),"Tên dịch vụ" },
              {TypeReportDetail.Detail_nhomhang.ToString(),"Nhóm dịch vụ" },
                  { TypeReportDetail.Detail_soluong.ToString(),"Số lượng" },
                  {TypeReportDetail.Detail_donvitinh.ToString(),"Đơn vị tính" },
                   { TypeReportDetail.Detail_giaban.ToString(),"Đơn giá" },
                    { TypeReportDetail.Detail_chietkhau.ToString(),"Chiết khấu" },
              {TypeReportDetail.Detail_thanhtien.ToString(),"Thành tiền" },
                 {ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
                 {ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
                   {TypeReportDetail.Detail_ghichu.ToString(),"Ghi chú" },
        };

        public enum TypeReportGroupProduct
        {
            nhomhang = 1,
            soluong,
            thanhtien,
            tienvon,
            giamgia,
            lailo,
            doanhthuthuan,
            tienthue
        }

        public static Dictionary<string, string> listNameReportGroupProduct = new Dictionary<string, string>()
        {
            { TypeReportGroupProduct.nhomhang.ToString(),"Nhóm hàng" },
               { TypeReportGroupProduct.soluong.ToString(),"Số lượng" },
              {TypeReportGroupProduct.thanhtien.ToString(),"Thành tiền" },
                  { TypeReportGroupProduct.giamgia.ToString(),"Giảm giá" },
                  { TypeReportGroupProduct.doanhthuthuan.ToString(),"Doanh thu" },
                {TypeReportGroupProduct.tienthue.ToString(),"Tiền thuế" },
           {TypeReportGroupProduct.tienvon.ToString(),"Tiền vốn" },
            {TypeReportDetail.Detail_chiphi.ToString(),"Chi phí" },
                   { TypeReportGroupProduct.lailo.ToString(),"Lãi lỗ" }
        };
        public enum TypeReportCustomer
        {
            Customer_nhomkhach = 1,
            Customer_makhac,
            Customer_tenkhach,
            Customer_dienthoai,
            Customer_soluongban,
            Customer_soluongtra,
            Customer_soluong,
            Customer_thanhtien,
            Customer_tienvon,
            Customer_giamgia,
            Customer_lailo,
            Customer_nggioithieu,
            Customer_ngquanly,
            Customer_diachi,
            Customer_solanden,
            Customer_gtmua,
            Customer_gttra,
            Customer_doanhthu,
            Customer_ngaygiaodichgannhat,
        }
        public static Dictionary<string, string> listNameReportCustomer = new Dictionary<string, string>()
        {
            { TypeReportCustomer.Customer_nhomkhach.ToString(),"Nhóm khách hàng" },
               { TypeReportCustomer.Customer_makhac.ToString(),"Mã khách" },
              {TypeReportCustomer.Customer_tenkhach.ToString(),"Tên khách hàng" },
                {TypeReportCustomer.Customer_dienthoai.ToString(),"Điện thoại" },
                  {TypeReportCustomer.Customer_soluongban.ToString(),"Số lượng bán" },
            {TypeReportCustomer.Customer_gtmua.ToString(),"Giá trị bán" },
                  {TypeReportCustomer.Customer_soluongtra.ToString(),"Số lượng trả" },
                  {TypeReportCustomer.Customer_gttra.ToString(),"Giá trị trả" },
                  {TypeReportCustomer.Customer_soluong.ToString(),"Số lượng" },
            { commonEnum.TypeReportGroupProduct.doanhthuthuan.ToString(),"Doanh thu" },
                         {commonEnum.TypeReportGroupProduct.tienthue.ToString(),"Tiền thuế" },
                     {TypeReportCustomer.Customer_tienvon.ToString(),"Tiền vốn" },
                {TypeReportDetail.Detail_chiphi.ToString(),"Chi phí" },
                        {TypeReportCustomer.Customer_lailo.ToString(),"Lãi lỗ" },
                         {TypeReportCustomer.Customer_nggioithieu.ToString(),"Người giới thiệu" },
                          {TypeReportCustomer.Customer_ngquanly.ToString(),"Người quản lý" },
        };

        public static Dictionary<string, string> Dictionary_BaoCaoTanSuatDoanhThu = new Dictionary<string, string>()
        {
            { TypeReportCustomer.Customer_nhomkhach.ToString(),"Nhóm khách hàng" },
               { TypeReportCustomer.Customer_makhac.ToString(),"Mã khách" },
              {TypeReportCustomer.Customer_tenkhach.ToString(),"Tên khách hàng" },
                {TypeReportCustomer.Customer_dienthoai.ToString(),"Điện thoại" },
                  {TypeReportCustomer.Customer_diachi.ToString(),"Địa chỉ" },
                   { TypeReportCustomer.Customer_solanden.ToString(),"Số lần đến" },
                     {TypeReportCustomer.Customer_gtmua.ToString(),"Tổng mua" },
                     {TypeReportCustomer.Customer_gttra.ToString(),"Tổng trả" },
                     {TypeReportCustomer.Customer_doanhthu.ToString(),"Doanh thu" },
                     {TypeReportCustomer.Customer_ngaygiaodichgannhat.ToString(),"Ngày giao dịch gần nhất" },
        };

        public enum TypeReportGrCustomer
        {
            GrCustomer_nhomkhach = 1,
            GrCustomer_soluong,
            GrCustomer_thanhtien,
            GrCustomer_tienvon,
            GrCustomer_giamgia,
            GrCustomer_lailo
        }
        public static Dictionary<string, string> listNameReportGrCustomer = new Dictionary<string, string>()
        {
            { TypeReportGrCustomer.GrCustomer_nhomkhach.ToString(),"Nhóm khách hàng" },
                  {TypeReportGrCustomer.GrCustomer_soluong.ToString(),"Số lượng" },
                   { TypeReportGrCustomer.GrCustomer_thanhtien.ToString(),"Thành tiền" },
                     {TypeReportGrCustomer.GrCustomer_tienvon.ToString(),"Tiền vốn" },
                         {TypeReportGrCustomer.GrCustomer_giamgia.ToString(),"Giảm giá" },
                        {TypeReportGrCustomer.GrCustomer_lailo.ToString(),"Lãi lỗ" },
        };

        public enum TypeReportUser
        {
            MaNhanVien = 1,
            User_nhanvien,
            User_soluong,
            User_giamgia,
            User_thanhtien,
            User_soluongtra,
            User_giatritra,
            User_tienvon,
            User_lailo
        }
        public static Dictionary<string, string> listNameReportUser = new Dictionary<string, string>()
        {
            { TypeReportUser.MaNhanVien.ToString(),"Mã nhân viên" },
            { TypeReportUser.User_nhanvien.ToString(),"Tên nhân viên" },
                  {TypeReportUser.User_soluong.ToString(),"Số lượng bán" },
                   { TypeReportUser.User_thanhtien.ToString(),"Thành tiền" },
                         {TypeReportUser.User_soluongtra.ToString(),"Số lượng trả" },
                         {TypeReportUser.User_giatritra.ToString(),"Giá trị trả" },
                        {TypeReportUser.User_giamgia.ToString(),"Giảm giá HĐ" },
                        {TypeReportGroupProduct.doanhthuthuan.ToString(),"Doanh thu" },
                        {TypeReportGroupProduct.tienthue.ToString(),"Tiền thuế" },
                     {TypeReportUser.User_tienvon.ToString(),"Tiền vốn" },
                      {TypeReportDetail.Detail_chiphi.ToString(),"Chi phí" },
                        {TypeReportUser.User_lailo.ToString(),"Lãi lỗ" },
        };

        public enum TypeReportProductReturn
        {
            ProductReturn_machungtug = 1,
            ProductReturn_machungtu,
            ProductReturn_ngaychungtu,
            ProductReturn_mahang,
            ProductReturn_tenhang,
            ProductReturn_soluong,
            ProductReturn_giamgia,
            ProductReturn_thanhtien,
            ProductReturn_phaithanhtoan,
            ProductReturn_nhanvien,
            ProductReturn_donvitinh,
            ProductReturn_lohang,
            ProductReturn_ghichu,
        }
        public static Dictionary<string, string> listNameReportProductReturn = new Dictionary<string, string>()
        {
                  {TypeReportProductReturn.ProductReturn_machungtug.ToString(),"Mã chứng từ gốc" },
                   {TypeReportProductReturn.ProductReturn_machungtu.ToString(),"Mã chứng từ" },
                   { TypeReportProductReturn.ProductReturn_ngaychungtu.ToString(),"Ngày chứng từ" },
                       {TypeReportProductReturn.ProductReturn_mahang.ToString(),"Mã hàng" },
                       {TypeReportProductReturn.ProductReturn_tenhang.ToString(),"Tên hàng" },
                       {TypeReportProductReturn.ProductReturn_donvitinh.ToString(),"Đvt" },
                       {TypeReportProductReturn.ProductReturn_lohang.ToString(),"Lô hàng" },
                     {TypeReportProductReturn.ProductReturn_soluong.ToString(),"Số lượng" },
                       {TypeReportProductReturn.ProductReturn_thanhtien.ToString(),"Thành tiền" },
                       {TypeReportProductReturn.ProductReturn_giamgia.ToString(),"Giảm giá HĐ" },
                      {TypeReportProductReturn.ProductReturn_phaithanhtoan.ToString(),"Phải thanh toán" },
                     { TypeReportProductReturn.ProductReturn_nhanvien.ToString(),"Nhân viên" },
                     { TypeReportProductReturn.ProductReturn_ghichu.ToString(),"Ghi chú" },
        };

        public enum ReportPromotion
        {
            makhuyenmai,
            tenkhuyenmai,
            hinhthuc,
            mahoadon,
            ngaylaphd,
            madoituong,
            tendoituong,
            tongtienhang,
            nguoitao,
            nvban,
            soluong,
            giatrikm,
            mahanghoa,
            tenhanghoa,
            donvitinh
        }
        public static Dictionary<string, string> DictionaryReportPromotion = new Dictionary<string, string>()
        {
            {ReportPromotion.makhuyenmai.ToString(),"Mã khuyến mại" },
            {ReportPromotion.tenkhuyenmai.ToString(),"Tên khuyến mại" },
            {ReportPromotion.hinhthuc.ToString(),"Hình thức" },
            {ReportPromotion.mahoadon.ToString(),"Mã hóa đơn" },
            {ReportPromotion.ngaylaphd.ToString(),"Ngày lập hóa đơn" },
            {ReportPromotion.madoituong.ToString(),"Mã đối tượng" },
            {ReportPromotion.tendoituong.ToString(),"Tên đối tượng" },
            {ReportPromotion.tongtienhang.ToString(),"Doanh thu" },
            {ReportPromotion.mahanghoa.ToString(),"Mã hàng hóa" },
            {ReportPromotion.tenhanghoa.ToString(),"Tên hàng hóa" },
            {ReportPromotion.soluong.ToString(),"Số lượng" },
            {ReportPromotion.giatrikm.ToString(),"Giá trị khuyến mại" },
            {ReportPromotion.nguoitao.ToString(),"Người tạo" },
            {ReportPromotion.nvban.ToString(),"Nhân viên bán" },
        };

        public enum TypeReportprofit
        {
            profit_mahang = 1,
            profit_tenhang,
            profit_soluongb,
            profit_giangiahd,
            profit_thanhtien,
            profit_soluongtra,
            profit_giatritra,
            profit_doanhthu,
            profit_tongtienvon,
            profit_loinhuan,
            profit_tysuat,
            profit_donvitinh,
            profit_lohang
        }
        public static Dictionary<string, string> listNameReportprofit = new Dictionary<string, string>()
        {
                  {TypeReportprofit.profit_mahang.ToString(),"Mã hàng" },
                        {TypeReportprofit.profit_tenhang.ToString(),"Tên hàng" },
                         {TypeReportprofit.profit_donvitinh.ToString(),"Đvt" },
                       {TypeReportprofit.profit_lohang.ToString(),"Lô hàng" },
                   { TypeReportprofit.profit_soluongb.ToString(),"Số lượng bán" },
                         {TypeReportprofit.profit_thanhtien.ToString(),"Thành tiền" },
                     {TypeReportprofit.profit_soluongtra.ToString(),"Số lượng trả" },
                        {TypeReportprofit.profit_giatritra.ToString(),"Giá trị trả" },
                         {TypeReportprofit.profit_giangiahd.ToString(),"Giảm giá HĐ" },
                     { TypeReportprofit.profit_doanhthu.ToString(),"Doanh thu (thuần)" },
                     { ColumnInvoices.tienthue.ToString(),"Tiền thuế" },
                            {TypeReportprofit.profit_tongtienvon.ToString(),"Tổng tiền vốn" },
                             {TypeReportDetail.Detail_chiphi.ToString(),"Chi phí" },
                        {TypeReportprofit.profit_loinhuan.ToString(),"Lợi nhuận" },
                     { TypeReportprofit.profit_tysuat.ToString(),"Tỷ suất" },
        };

        public enum TypeReportImport
        {
            Import_mahang = 1,
            Import_tenhang,
            Import_soluongnhap,
            Import_giatrinhap,
            Import_machungtu,
            Import_ngaychungtu,
            Import_dongia,
            Import_thanhtien,
            Import_nhomhang,
            Import_machungtugoc,
            Import_manhacc,
            Import_tennhacc,
            Import_chietkhau,
            Import_giamgiahd,
            Import_nhanvien,
            Import_phone,
            Import_donvitinh,
            Import_lohang,
            Import_ghichu,
            Import_tienthue,

        }
        public static Dictionary<string, string> ReportImportGoodsTotal = new Dictionary<string, string>()
        {
                  {TypeReportImport.Import_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportImport.Import_mahang.ToString(),"Mã hàng" },
                    {TypeReportImport.Import_tenhang.ToString(),"Tên hàng" },
                    { TypeReportImport.Import_donvitinh.ToString(),"Đvt" },
                    {TypeReportImport.Import_lohang.ToString(),"Lô hàng" },
                   { TypeReportImport.Import_soluongnhap.ToString(),"Số lượng nhập" },
                    {TypeReportImport.Import_thanhtien.ToString(),"Thành tiền" },
                       {TypeReportImport.Import_giamgiahd.ToString(),"Giảm giá HĐ" },
                    {TypeReportImport.Import_giatrinhap.ToString(),"Giá trị nhập" },
                    {TypeReportImport.Import_tienthue.ToString(),"Tiền thuế" },

        };
        public static Dictionary<string, string> ReportImportGoodsDetail = new Dictionary<string, string>()
        {
                { TypeReportImport.Import_machungtu.ToString(),"Mã chứng từ" },
                    {TypeReportImport.Import_ngaychungtu.ToString(),"Ngày chứng từ" },
                     {TypeReportImport.Import_manhacc.ToString(),"Mã nhà cung cấp" },
                  {TypeReportImport.Import_tennhacc.ToString(),"Tên nhà cung cấp" },
                  {TypeReportImport.Import_mahang.ToString(),"Mã hàng" },
                    {TypeReportImport.Import_tenhang.ToString(),"Tên hàng" },
                     { TypeReportImport.Import_donvitinh.ToString(),"Đvt" },
                    {TypeReportImport.Import_lohang.ToString(),"Lô hàng" },
                   { TypeReportImport.Import_soluongnhap.ToString(),"Số lượng nhập" },
                    {TypeReportImport.Import_dongia.ToString(),"Đơn giá" },
                      {TypeReportImport.Import_chietkhau.ToString(),"Chiết khấu" },
                    { TypeReportImport.Import_thanhtien.ToString(),"Thành tiền" },
                      {TypeReportImport.Import_giamgiahd.ToString(),"Giảm giá HĐ" },
                      {TypeReportImport.Import_giatrinhap.ToString(),"Giá trị nhập" },
                      {TypeReportImport.Import_tienthue.ToString(),"Tiền thuế" },
                    { TypeReportImport.Import_nhanvien.ToString(),"Nhân viên" },
                     { TypeReportImport.Import_ghichu.ToString(),"Ghi chú" },

        };
        public static Dictionary<string, string> ReportImportGoodsGroup = new Dictionary<string, string>()
        {
                { TypeReportImport.Import_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportImport.Import_soluongnhap.ToString(),"Số lượng nhập" },
                    {TypeReportImport.Import_thanhtien.ToString(),"Thành tiền" },
                      {TypeReportImport.Import_giamgiahd.ToString(),"Giảm giá HĐ" },
                  {TypeReportImport.Import_giatrinhap.ToString(),"Giá trị nhập" },
                  {TypeReportImport.Import_tienthue.ToString(),"Tiền thuế" },
        };
        public static Dictionary<string, string> ReportImportGoodsSupplier = new Dictionary<string, string>()
        {   { TypeReportImport.Import_nhomhang.ToString(),"Nhóm nhà cung cấp" },
                { TypeReportImport.Import_manhacc.ToString(),"Mã nhà cung cấp" },
                  {TypeReportImport.Import_tennhacc.ToString(),"Tên nhà cung cấp" },
                    {TypeReportImport.Import_phone.ToString(),"Điện thoại" },
                    {TypeReportImport.Import_soluongnhap.ToString(),"Số lượng nhập" },
                   {TypeReportImport.Import_thanhtien.ToString(),"Thành tiền" },
                    {TypeReportImport.Import_giamgiahd.ToString(),"Giảm giá HĐ" },
                  {TypeReportImport.Import_giatrinhap.ToString(),"Giá trị nhập" },
                  {TypeReportImport.Import_tienthue.ToString(),"Tiền thuế" },
        };
        public static Dictionary<string, string> ReportImportGoodsReturn = new Dictionary<string, string>()
        {
                    {TypeReportImport.Import_machungtugoc.ToString(),"Mã chứng từ gốc" },
                    {TypeReportImport.Import_machungtu.ToString(),"Mã chứng từ" },
                    {TypeReportImport.Import_ngaychungtu.ToString(),"Ngày chứng từ" },
                    {TypeReportImport.Import_mahang.ToString(),"Mã hàng" },
                    { TypeReportImport.Import_tenhang.ToString(),"Tên hàng" },
                    { TypeReportImport.Import_donvitinh.ToString(),"Đvt" },
                    {TypeReportImport.Import_lohang.ToString(),"Lô hàng" },
                    {TypeReportImport.Import_soluongnhap.ToString(),"Số lượng" },
                      {TypeReportImport.Import_thanhtien.ToString(),"Thành tiền" },
                    {TypeReportImport.Import_giamgiahd.ToString(),"Giảm giá HĐ" },
                    {TypeReportImport.Import_giatrinhap.ToString(),"Giá trị trả" },
                    {TypeReportImport.Import_nhanvien.ToString(),"Tên nhân viên" },
        };

        public enum TypeReportOrder
        {
            Order_nhomhang = 1,
            Order_mahang,
            Order_machungtu,
            Order_tenhang,
            Order_soluong,
            Order_thanhtien,
            Order_giamgia,
            Order_giatri,
            Order_ngay,
            Order_khachhang,
            Order_thanhtoan,
            Order_soluongnhan,
            Order_nhanvien,
            Order_donvitinh,
            Order_lohang,
            Order_ghichu,
        };
        public static Dictionary<string, string> ReportOrderTotal = new Dictionary<string, string>()
        {
                  {TypeReportOrder.Order_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportOrder.Order_mahang.ToString(),"Mã hàng" },
                   { TypeReportOrder.Order_tenhang.ToString(),"Tên hàng" },
                    {TypeReportOrder.Order_donvitinh.ToString(),"Đvt" },
                     {TypeReportOrder.Order_lohang.ToString(),"Lô hàng" },
                    {TypeReportOrder.Order_soluong.ToString(),"Số lượng đặt" },
                     {TypeReportOrder.Order_thanhtien.ToString(),"Thành tiền" },
                      {TypeReportOrder.Order_giamgia.ToString(),"Giảm giá HĐ" },
                       {TypeReportOrder.Order_giatri.ToString(),"Giá trị đặt" },
                       {TypeReportOrder.Order_soluongnhan.ToString(),"Số lượng nhận" },
        };
        public static Dictionary<string, string> ReportOrderDetail = new Dictionary<string, string>()
        {
                { TypeReportOrder.Order_machungtu.ToString(),"Mã chứng từ" },
                    {TypeReportOrder.Order_ngay.ToString(),"Ngày chứng từ" },
                  {TypeReportOrder.Order_khachhang.ToString(),"Nhà cung cấp" },
                            {TypeReportOrder.Order_mahang.ToString(),"Mã hàng" },
                   { TypeReportOrder.Order_tenhang.ToString(),"Tên hàng" },
                     {TypeReportOrder.Order_donvitinh.ToString(),"Đvt" },
                     {TypeReportOrder.Order_lohang.ToString(),"Lô hàng" },
                    {TypeReportOrder.Order_soluong.ToString(),"Số lượng đặt" },
                   { TypeReportOrder.Order_thanhtien.ToString(),"Thành tiền" },
                    {TypeReportOrder.Order_giamgia.ToString(),"Giảm giá HĐ" },
                      {TypeReportOrder.Order_giatri.ToString(),"Giá trị đặt" },
                          {TypeReportOrder.Order_soluongnhan.ToString(),"Số lượng nhận" },
                        {TypeReportOrder.Order_ghichu.ToString(),"Ghi chú" },

        };
        public static Dictionary<string, string> ReportOrderGroup = new Dictionary<string, string>()
        {
               {TypeReportOrder.Order_nhomhang.ToString(),"Nhóm hàng" },
                  {TypeReportOrder.Order_soluong.ToString(),"Số lượng đặt" },
                     {TypeReportOrder.Order_thanhtien.ToString(),"Thành tiền" },
                      {TypeReportOrder.Order_giamgia.ToString(),"Giảm giá HĐ" },
                       {TypeReportOrder.Order_giatri.ToString(),"Giá trị đặt" },
                         {TypeReportOrder.Order_soluongnhan.ToString(),"Số lượng nhận" },
        };

        public enum TypeReportWarehouse
        {
            Warehouse_nhomhang = 1,
            Warehouse_mahang,
            Warehouse_tondauky,
            Warehouse_tenhang,
            Warehouse_soluong,
            Warehouse_thanhtien,
            Warehouse_giamgia,
            Warehouse_giatri,
            Warehouse_giatridau,
            Warehouse_giatricuoi,
            Warehouse_giatrixuat,
            Warehouse_toncuoiky,
            Warehouse_ngay,
            Warehouse_soluongnhan,
            Warehouse_maphieu,
            Warehouse_dongia,
            Warehouse_chinhanhchuyen,
            Warehouse_chinhanhnhan,
            Warehouse_loaichungtu,
            Warehouse_machungtu,
            Warehouse_nhap,
            Warehouse_xuat,
            Warehouse_soluongquycach,
            Warehouse_donvitinh,
            Warehouse_lohang,
            Warehouse_giavon,
            Warehouse_MaChiNhanh,
            Warehouse_TenChiNhanh,
            Warehouse_ghichuct,
            Warehouse_nhanvien,
        };

        public static Dictionary<string, string> ReportWarehouseTotal = new Dictionary<string, string>()
        {
                  {TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                   { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                      { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                         { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng tồn" },
                     {TypeReportWarehouse.Warehouse_soluongquycach.ToString(),"Số lượng tồn(quy cách)"},
                      {TypeReportWarehouse.Warehouse_giatri.ToString(),"Giá trị tồn" },
        };
        public static Dictionary<string, string> ReportWarehouseExport = new Dictionary<string, string>()
        {
            {TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                  {TypeReportWarehouse.Warehouse_tondauky.ToString(),"Tồn đầu kỳ" },
                     {TypeReportWarehouse.Warehouse_giatridau.ToString(),"Giá trị đầu kỳ" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng nhập" },
                     {TypeReportWarehouse.Warehouse_giatri.ToString(),"Giá trị nhập" },
                      {TypeReportWarehouse.Warehouse_soluongnhan.ToString(),"Số lượng xuất" },
                       {TypeReportWarehouse.Warehouse_giatrixuat.ToString(),"Giá trị xuất" },
                       {TypeReportWarehouse.Warehouse_toncuoiky.ToString(),"Tồn cuối kỳ" },
                        {TypeReportWarehouse.Warehouse_soluongquycach.ToString(),"Tồn quy cách" },
                         {TypeReportWarehouse.Warehouse_giatricuoi.ToString(),"Giá trị cuối kỳ" },
        };
        public static Dictionary<string, string> ReportWarehouseDetail = new Dictionary<string, string>()
        {   { TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                  {TypeReportWarehouse.Warehouse_tondauky.ToString(),"Tồn đầu kỳ" },
                     {TypeReportWarehouse.Warehouse_giatri.ToString(),"Giá trị đầu kỳ" },
                    {TypeReportWarehouse.Warehouse_nhap.ToString(),"Nhập" },
                      {TypeReportWarehouse.Warehouse_xuat.ToString(),"Xuất" },
                       {TypeReportWarehouse.Warehouse_toncuoiky.ToString(),"Tồn cuối kỳ" },
                         {TypeReportWarehouse.Warehouse_giatricuoi.ToString(),"Giá trị cuối kỳ" },
        };
        public static Dictionary<string, string> ReportWarehouseImportStoreHH = new Dictionary<string, string>()
        {
             { TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng nhập" },
                     {TypeReportWarehouse.Warehouse_giatri.ToString(),"Giá trị nhập" },
        };
        public static Dictionary<string, string> ReportWarehouseTransportExport = new Dictionary<string, string>()
        {
                 { TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng chuyển" },
                     {TypeReportWarehouse.Warehouse_giatri.ToString(),"Giá trị chuyển" },
        };
        public static Dictionary<string, string> ReportWarehouseTransportImport = new Dictionary<string, string>()
        {
                { TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                    {TypeReportWarehouse.Warehouse_soluongnhan.ToString(),"Số lượng nhận" },
                     {TypeReportWarehouse.Warehouse_giatri.ToString(),"Giá trị nhận" },
        };
        public static Dictionary<string, string> ReportWarehouseTransportDetail = new Dictionary<string, string>()
        {
                  { TypeReportWarehouse.Warehouse_maphieu.ToString(),"Mã phiếu" },
             { TypeReportWarehouse.Warehouse_ngay.ToString(),"Ngày chuyển" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                    {TypeReportWarehouse.Warehouse_chinhanhchuyen.ToString(),"Chi nhánh chuyển" },
                   { TypeReportWarehouse.Warehouse_chinhanhnhan.ToString(),"Chi nhánh nhận" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng" },
                     {TypeReportWarehouse.Warehouse_giavon.ToString(),"Giá vốn" },
                     {TypeReportWarehouse.Warehouse_giatri.ToString(),"Giá trị nhận" },
                      {TypeReportWarehouse.Warehouse_dongia.ToString(),"Đơn giá" },
                     {TypeReportWarehouse.Warehouse_thanhtien.ToString(),"Thành tiền" },
        };
        public static Dictionary<string, string> ReportWarehouseExportStoreHH = new Dictionary<string, string>()
        {
                   { TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                   {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng xuất" },
                     {TypeReportWarehouse.Warehouse_giatrixuat.ToString(),"Giá trị xuất" },
        };
        public static Dictionary<string, string> ReportWarehouseExportStoreGD = new Dictionary<string, string>()
        {
             { TypeReportWarehouse.Warehouse_loaichungtu.ToString(),"Loại chứng từ" },
                    {TypeReportWarehouse.Warehouse_machungtu.ToString(),"Mã chứng từ" },
                    { TypeReportWarehouse.Warehouse_ngay.ToString(),"Ngày chứng từ" },
                    { ColumnInvoices.bienso.ToString(),"Biển số" },
                     { ColumnInvoices.nguoitao.ToString(),"User lập phiếu" },
                    { ColumnInvoices.makhachhang.ToString(),"Mã khách hàng" },
                    { ColumnInvoices.tenkhachhang.ToString(),"Tên khách hàng" },
                       { TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng" },
                     {TypeReportWarehouse.Warehouse_thanhtien.ToString(),"Giá trị xuất" },
                     {TypeReportWarehouse.Warehouse_ghichuct.ToString(),"Ghi chú" },
                     {ColumnInvoices.ghichu.ToString(),"Ghi chú hóa đơn" },
                     {TypeReportWarehouse.Warehouse_nhanvien.ToString(),"NV lập hóa đơn" },
        };

        public static Dictionary<string, string> BaoCaoNhapKhoChiTiet = new Dictionary<string, string>()
        {
             { TypeReportWarehouse.Warehouse_loaichungtu.ToString(),"Loại chứng từ" },
                    {TypeReportWarehouse.Warehouse_machungtu.ToString(),"Mã chứng từ" },
                    { TypeReportWarehouse.Warehouse_ngay.ToString(),"Ngày chứng từ" },
                    { ColumnInvoices.nguoitao.ToString(),"User lập phiếu" },
                    { ColumnInvoices.makhachhang.ToString(),"Mã người cung cấp" },
                    { ColumnInvoices.tenkhachhang.ToString(),"Tên người cung cấp" },
                       { TypeReportWarehouse.Warehouse_nhomhang.ToString(),"Nhóm hàng" },
                    {TypeReportWarehouse.Warehouse_mahang.ToString(),"Mã hàng" },
                   { TypeReportWarehouse.Warehouse_tenhang.ToString(),"Tên hàng" },
                    { TypeReportWarehouse.Warehouse_donvitinh.ToString(),"Đvt" },
                   { TypeReportWarehouse.Warehouse_lohang.ToString(),"Lô hàng" },
                { TypeReportWarehouse.Warehouse_MaChiNhanh.ToString(),"Mã chi nhánh" },
                   { TypeReportWarehouse.Warehouse_TenChiNhanh.ToString(),"Tên chi nhánh" },
                    {TypeReportWarehouse.Warehouse_soluong.ToString(),"Số lượng" },
                     {TypeReportWarehouse.Warehouse_dongia.ToString(),"Giá nhập" },
                     {TypeReportWarehouse.Warehouse_thanhtien.ToString(),"Giá trị nhập" },
                     {TypeReportWarehouse.Warehouse_ghichuct.ToString(),"Ghi chú" },
                     {ColumnInvoices.ghichu.ToString(),"Ghi chú hóa đơn" },
        };
        public enum TypeReportFinancial

        {
            Financial_nhomdoitac = 1,
            Financial_madoitac,
            Financial_tendoitac,
            Financial_dauky,
            Financial_trongky,
            Financial_cuoiky,
            Financial_hdlienquan,
            Financial_maphieu,
            Financial_ngaylap,
            Financial_ten,
            Financial_tongtienthu,
            Financial_khoanmuc,
            Financial_ghichu,
            Financial_loai,
            Financial_tienthu,
            Financial_tienchi,
            Financial_tonluyke,
            Financial_sotaikhoan,
            Financial_nganhang,
            Financial_tienmat,
            Financial_tiengui

        }
        public static Dictionary<string, string> ReportFinancialTotal = new Dictionary<string, string>()
        {
             { TypeReportFinancial.Financial_nhomdoitac.ToString(),"Nhóm đối tác" },
                    {TypeReportFinancial.Financial_madoitac.ToString(),"Mã đối tác" },
                    { TypeReportFinancial.Financial_tendoitac.ToString(),"Tên đối tác" },
                       { TypeReportFinancial.Financial_dauky.ToString(),"Đầu kỳ" },
                   { TypeReportFinancial.Financial_trongky.ToString(),"Trong kỳ" },
                    {TypeReportFinancial.Financial_cuoiky.ToString(),"Cuối kỳ" }
        };
        public static Dictionary<string, string> ReportFinancialCollectMoney = new Dictionary<string, string>()
        {
             { TypeReportFinancial.Financial_nhomdoitac.ToString(),"Nhóm đối tác" },
                    {TypeReportFinancial.Financial_hdlienquan.ToString(),"HĐ liên quan" },
                    { TypeReportFinancial.Financial_maphieu.ToString(),"Mã phiếu thu" },
                       { TypeReportFinancial.Financial_ngaylap.ToString(),"Ngày lập phiếu" },
                   { TypeReportFinancial.Financial_madoitac.ToString(),"Mã người nộp" },
                   { TypeReportFinancial.Financial_ten.ToString(),"Tên người nộp" },
                    {TypeReportFinancial.Financial_tongtienthu.ToString(),"Tổng tiền thu" },
                         { TypeReportFinancial.Financial_khoanmuc.ToString(),"Khoản thu" },
                       { TypeReportFinancial.Financial_ghichu.ToString(),"Ghi chú" },
                   { TypeReportFinancial.Financial_loai.ToString(),"Loại phiếu" },
        };
        public static Dictionary<string, string> ReportFinancialPay = new Dictionary<string, string>()
        {
             { TypeReportFinancial.Financial_nhomdoitac.ToString(),"Nhóm đối tác" },
                    {TypeReportFinancial.Financial_hdlienquan.ToString(),"HĐ liên quan" },
                    { TypeReportFinancial.Financial_maphieu.ToString(),"Mã phiếu chi" },
                       { TypeReportFinancial.Financial_ngaylap.ToString(),"Ngày lập phiếu" },
                   { TypeReportFinancial.Financial_ten.ToString(),"Tên người nhận" },
                    {TypeReportFinancial.Financial_tongtienthu.ToString(),"Tổng tiền chi" },
                         { TypeReportFinancial.Financial_khoanmuc.ToString(),"Khoản mục" },
                       { TypeReportFinancial.Financial_ghichu.ToString(),"Ghi chú" },
                   { TypeReportFinancial.Financial_loai.ToString(),"Loại phiếu" },
        };
        public static Dictionary<string, string> ReportFinancialBookCash = new Dictionary<string, string>()
        {

                    { TypeReportFinancial.Financial_maphieu.ToString(),"Mã phiếu" },
                       { TypeReportFinancial.Financial_ngaylap.ToString(),"Thời gian" },
                         { TypeReportFinancial.Financial_khoanmuc.ToString(),"Khoản mục" },
                   { TypeReportFinancial.Financial_ten.ToString(),"Người nộp/ nhận" },
                    {TypeReportFinancial.Financial_tienthu.ToString(),"Tiền thu" },
                     {TypeReportFinancial.Financial_tienchi.ToString(),"Tiền chi" },
                   { TypeReportFinancial.Financial_tonluyke.ToString(),"Tồn lũy kế" },
                       { TypeReportFinancial.Financial_ghichu.ToString(),"Ghi chú" },
        };
        public static Dictionary<string, string> ReportFinancialBookBank = new Dictionary<string, string>()
        {

                  { TypeReportFinancial.Financial_maphieu.ToString(),"Mã phiếu" },
                   { TypeReportFinancial.Financial_ngaylap.ToString(),"Thời gian" },
                   { TypeReportFinancial.Financial_khoanmuc.ToString(),"Khoản mục" },
                   { TypeReportFinancial.Financial_ten.ToString(),"Người nộp/ nhận" },
                    {TypeReportFinancial.Financial_tienthu.ToString(),"Tiền thu" },
                     {TypeReportFinancial.Financial_tienchi.ToString(),"Tiền chi" },
                   { TypeReportFinancial.Financial_tonluyke.ToString(),"Tồn lũy kế" },
                    { TypeReportFinancial.Financial_sotaikhoan.ToString(),"Số tài khoản" },
                     { TypeReportFinancial.Financial_nganhang.ToString(),"Ngân hàng" },
                       { TypeReportFinancial.Financial_ghichu.ToString(),"Ghi chú" },
        };
        public static Dictionary<string, string> ReportFinancialSurvive = new Dictionary<string, string>()
        {

                  { TypeReportFinancial.Financial_maphieu.ToString(),"Mã phiếu" },
                   { TypeReportFinancial.Financial_ngaylap.ToString(),"Thời gian" },
                   { TypeReportFinancial.Financial_khoanmuc.ToString(),"Khoản mục" },
                   { TypeReportFinancial.Financial_ten.ToString(),"Người nộp/ nhận" },
                    {TypeReportFinancial.Financial_tienthu.ToString(),"Tiền thu" },
                     {TypeReportFinancial.Financial_tienchi.ToString(),"Tiền chi" },
                   { TypeReportFinancial.Financial_tonluyke.ToString(),"Tồn lũy kế" },
                    { TypeReportFinancial.Financial_sotaikhoan.ToString(),"Số tài khoản" },
                     { TypeReportFinancial.Financial_nganhang.ToString(),"Ngân hàng" },
                       { TypeReportFinancial.Financial_ghichu.ToString(),"Ghi chú" },
        };

        public enum TypeUserContact
        {
            malienhe = 1,
            khachang,
            ten,
            didong,
            sinhnhat,
            email,
            chucvu,
            diachi,
            tinhthanh,
            quanhuyen,
            ghichu
        }

        public static Dictionary<string, string> UserContact = new Dictionary<string, string>()
        {
             { TypeUserContact.malienhe.ToString(),"Mã liên hệ" },
             { TypeUserContact.ten.ToString(),"Tên liên hệ" },
             { TypeUserContact.khachang.ToString(),"Khách hàng" },
                    {TypeUserContact.didong.ToString(),"Di động" },
                   { TypeUserContact.sinhnhat.ToString(),"Sinh nhật" },
                    {TypeUserContact.email.ToString(),"Email" },
                   { TypeUserContact.chucvu.ToString(),"Chức vụ" },
                    {TypeUserContact.diachi.ToString(),"Địa chỉ" },
                     {TypeUserContact.tinhthanh.ToString(),"Tỉnh thành" },
                     {TypeUserContact.quanhuyen.ToString(),"Quận huyện" },
                     {TypeUserContact.ghichu.ToString(),"Ghi chú" },
        };


        public enum TypeServicepackage
        {
            mahoadon,
            ngaylaphoadon,
            ngayapdung,
            ngayhethan,
            makhachhang,
            khachhang,
            email,
            diachi,
            didong,
            khuvuc,
            phuongxa,
            chinhanhbanhang,
            nguoiban,
            nguoitao,
            ghichu,
            tongtienhang,
            giamgia,
            khachcantra,
            khachdatra,
            conlai,
            trangthai,
            tiendoidiem,
            tienthegiatri,
            tienmat,
            chuyenkhoan,
            tienpos,
        }

        public static Dictionary<string, string> ServicePackage = new Dictionary<string, string>()
        {
             { TypeServicepackage. mahoadon.ToString(),"Mã hóa đơn" },
             { TypeServicepackage. ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
             { TypeServicepackage. ngayapdung.ToString(),"Ngày áp dụng" },
                    {TypeServicepackage. ngayhethan.ToString(),"Ngày hết hạn" },
                    {ColumnInvoices. bienso.ToString(),"Biển số xe" },
                    {TypeServicepackage.makhachhang.ToString(),"Mã khách hàng" },
                    {TypeServicepackage.khachhang.ToString(),"Tên khách hàng" },
                    {TypeServicepackage. didong.ToString(),"Điện thoại" },
                    {TypeServicepackage. diachi.ToString(),"Địa chỉ" },
                    {TypeServicepackage. khuvuc.ToString(),"Tỉnh thành" },
             { TypeServicepackage.chinhanhbanhang.ToString(),"Chi nhánh" },
                     {TypeServicepackage. nguoiban.ToString(),"Người bán" },
                     { TypeServicepackage. nguoitao.ToString(),"Người tạo" },
                       { TypeServicepackage.tongtienhang.ToString(),"Tổng tiền hàng" },
                       { ColumnInvoices.tienthue.ToString(),"Tiền thuế" },
                    {TypeServicepackage.giamgia.ToString(),"Giảm giá" },
             { TypeServicepackage. khachcantra.ToString(),"Khách cần trả" },
                      { ColumnInvoices. butruTraHang.ToString(),"Bù trừ trả hàng" },
             { ColumnInvoices. gtriSauTra.ToString(),"Giá trị sau trả" },
                    {TypeServicepackage.khachdatra.ToString(),"Khách đã trả" },
              { TypeServicepackage. tienmat.ToString(),"Tiền mặt" },
             { TypeServicepackage. chuyenkhoan.ToString(),"Chuyển khoản" },
                    {TypeServicepackage.tienpos.ToString(),"Tiền POS" },
             { TypeServicepackage. tiendoidiem.ToString(),"Tiền đổi điểm" },
             { TypeServicepackage. tienthegiatri.ToString(),"Tiền thẻ giá trị" },
                   { TypeServicepackage.conlai.ToString(),"Còn nợ" },
                     {TypeServicepackage.ghichu.ToString(),"Ghi chú" },
                    {TypeServicepackage.trangthai.ToString(),"Trạng thái" },
        };
        public enum TypeRGoiDichVu
        {
            magoi = 1,
            ngayban,
            makhach,
            tenkhach,
            soluongban,
            thanhtien,
            soluongsudung,
            soluongconlai,
            ngayapdung,
            hansudung,
            songayconlai,
            songayquahan,
            mahanghoa,
            tenhanghoa,
            dongia,
            chietkhau,
            giamgiahd,
            nhanvien,
            giatriban,
            giatrisudung,
            giatriconlai,
            tondau,
            toncuoi,
            trongky,
            nhomkhach,
            nguonkhach,
            dienthoai,
            gioitinh,
            nguoigioithieu,
            nhomhanghoa,
            ghichu,
            soluongtra,
            giatritra,
            giavon,
            donvitinh,
            lohanghoa,
            mahoadon

        }

        public enum Column_BCGDVBanDoiTra
        {
            MaKhachHang,
            TenKhachHang,
            GDVMua_MaHoaDon,
            GDVMua_NgayLapHoaDon,

            MaHangHoa,
            TenHangHoa,
            TenDonViTinh,
            SoLuongMua,
            GiaTriMua,

            GDVDoi_MaHoaDon,
            GDVDoi_MaHangHoa,
            GDVDoi_TenHangHoa,
            GDVDoi_TenDonViTinh,
            SoLuongDoi,
            GiaTriDoi,
            GiaTriChenhLech
        }
        public static Dictionary<string, string> TypeRGoiDichVuDuTH = new Dictionary<string, string>()
        {
             { TypeRGoiDichVu.magoi.ToString(),"Mã gói DV" },
             { TypeRGoiDichVu.ngayban.ToString(),"Ngày bán" },
             { TypeRGoiDichVu.makhach.ToString(),"Mã khách" },
             { TypeRGoiDichVu.tenkhach.ToString(),"Tên khách" },
             { TypeRGoiDichVu.nhomkhach.ToString(),"Nhóm khách" },
             { TypeRGoiDichVu.nguonkhach.ToString(),"Nguồn khách" },
             { TypeRGoiDichVu.dienthoai.ToString(),"Điện thoại" },
             { TypeRGoiDichVu.gioitinh.ToString(),"Giới tính" },
             { TypeRGoiDichVu.nguoigioithieu.ToString(),"Người giới thiệu" },
             { TypeRGoiDichVu.soluongban.ToString(),"Số lượng bán" },
             { TypeRGoiDichVu.thanhtien.ToString(),"Thành tiền" },
             { TypeRGoiDichVu.soluongtra.ToString(),"Số lượng trả" },
             { TypeRGoiDichVu.giatritra.ToString(),"Giá trị trả" },
             { TypeRGoiDichVu.soluongsudung.ToString(),"Số lượng sử dụng" },
             { TypeRGoiDichVu.giavon.ToString(),"Giá vốn" },
             { TypeRGoiDichVu.soluongconlai.ToString(),"Số lượng còn lại" },
             { TypeRGoiDichVu.ngayapdung.ToString(),"Ngày áp dụng" },
             { TypeRGoiDichVu.hansudung.ToString(),"Hạn sử dụng" },
             { TypeRGoiDichVu.songayconlai.ToString(),"Số ngày còn hạn" },
             { TypeRGoiDichVu.songayquahan.ToString(),"Số ngày quá hạn" },

        };
        public static Dictionary<string, string> ListColumnBCGDV_BanDoiTra = new Dictionary<string, string>()
        {
             { Column_BCGDVBanDoiTra.MaKhachHang.ToString(),"Mã khách hàng" },
             { Column_BCGDVBanDoiTra.TenKhachHang.ToString(),"Tên khách hàng" },
             { Column_BCGDVBanDoiTra.GDVMua_MaHoaDon.ToString(),"Mã chứng từ" },
             { Column_BCGDVBanDoiTra.GDVMua_NgayLapHoaDon.ToString(),"Ngày lập" },
             { Column_BCGDVBanDoiTra.MaHangHoa.ToString(),"Mã hàng" },
             { Column_BCGDVBanDoiTra.TenHangHoa.ToString(),"Tên hàng" },
             { Column_BCGDVBanDoiTra.TenDonViTinh.ToString(),"DVT" },
             { Column_BCGDVBanDoiTra.SoLuongMua.ToString(),"Số lượng" },
             { Column_BCGDVBanDoiTra.GiaTriMua.ToString(),"Giá trị" },

             { Column_BCGDVBanDoiTra.GDVDoi_MaHoaDon.ToString(),"Mã chứng từ" },
             { Column_BCGDVBanDoiTra.GDVDoi_MaHangHoa.ToString(),"Mã hàng" },
             { Column_BCGDVBanDoiTra.GDVDoi_TenHangHoa.ToString(),"Tên hàng" },
             { Column_BCGDVBanDoiTra.GDVDoi_TenDonViTinh.ToString(),"DVT" },
             { Column_BCGDVBanDoiTra.SoLuongDoi.ToString(),"Số lượng" },
             { Column_BCGDVBanDoiTra.GiaTriDoi.ToString(),"Giá trị" },
             { Column_BCGDVBanDoiTra.GiaTriChenhLech.ToString(),"Chênh lệch" },

        };
        public static Dictionary<string, string> TypeRGoiDichVuDuCT = new Dictionary<string, string>()
        {
             { TypeRGoiDichVu.magoi.ToString(),"Mã gói DV" },
             { TypeRGoiDichVu.ngayban.ToString(),"Ngày bán" },
             { TypeRGoiDichVu.makhach.ToString(),"Mã khách" },
             { TypeRGoiDichVu.tenkhach.ToString(),"Tên khách" },
             { TypeRGoiDichVu.nhomkhach.ToString(),"Nhóm khách" },
             { TypeRGoiDichVu.nguonkhach.ToString(),"Nguồn khách" },
             { TypeRGoiDichVu.dienthoai.ToString(),"Điện thoại" },
             { TypeRGoiDichVu.gioitinh.ToString(),"Giới tính" },
             { TypeRGoiDichVu.nguoigioithieu.ToString(),"Người giới thiệu" },
             { TypeRGoiDichVu.mahanghoa.ToString(),"Mã hàng hóa" },
             { TypeRGoiDichVu.tenhanghoa.ToString(),"Tên hàng hóa" },
             { TypeRGoiDichVu.donvitinh.ToString(),"Đvt" },
             { TypeRGoiDichVu.lohanghoa.ToString(),"Lô hàng" },
             { TypeRGoiDichVu.soluongban.ToString(),"Số lượng" },
             { TypeRGoiDichVu.dongia.ToString(),"Đơn giá" },
             { ColumnInvoices.thanhtienchuack.ToString(),"Thành tiền chưa CK" },
             { TypeRGoiDichVu.chietkhau.ToString(),"Chiết khấu" },
             { TypeRGoiDichVu.thanhtien.ToString(),"Thành tiền" },
             { TypeRGoiDichVu.giamgiahd.ToString(),"Giảm giá HĐ" },
             { TypeRGoiDichVu.soluongtra.ToString(),"Số lượng trả" },
             { TypeRGoiDichVu.giatritra.ToString(),"Giá trị trả" },
             { TypeRGoiDichVu.soluongsudung.ToString(),"Số lượng sử dụng" },
             { TypeRGoiDichVu.giavon.ToString(),"Giá vốn" },
             { TypeRGoiDichVu.soluongconlai.ToString(),"Số lượng còn lại" },
             { TypeRGoiDichVu.nhanvien.ToString(),"Nhân viên bán" },

        };
        public static Dictionary<string, string> TypeRGoiDichVuNhatKyTH = new Dictionary<string, string>()
        {
             { TypeRGoiDichVu.makhach.ToString(),"Mã khách" },
             { TypeRGoiDichVu.tenkhach.ToString(),"Tên khách" },
             { TypeRGoiDichVu.nhomkhach.ToString(),"Nhóm khách" },
             { TypeRGoiDichVu.nguonkhach.ToString(),"Nguồn khách" },
             { TypeRGoiDichVu.dienthoai.ToString(),"Điện thoại" },
             { TypeRGoiDichVu.gioitinh.ToString(),"Giới tính" },
             { TypeRGoiDichVu.nguoigioithieu.ToString(),"Người giới thiệu" },
             { TypeRGoiDichVu.nhomhanghoa.ToString(),"Nhóm hàng hóa" },
             { TypeRGoiDichVu.mahanghoa.ToString(),"Mã hàng hóa" },
             { TypeRGoiDichVu.tenhanghoa.ToString(),"Tên hàng hóa" },
             { TypeRGoiDichVu.donvitinh.ToString(),"Đvt" },
             { TypeRGoiDichVu.lohanghoa.ToString(),"Lô hàng" },
             { TypeRGoiDichVu.soluongban.ToString(),"Số lượng mua" },
             { TypeRGoiDichVu.soluongtra.ToString(),"Số lượng trả" },
             { TypeRGoiDichVu.soluongsudung.ToString(),"Số lượng sử dụng" },
             { TypeRGoiDichVu.soluongconlai.ToString(),"Số lượng còn lại" },
             //{ TypeRGoiDichVu.ghichu.ToString(),"Ghi chú" },

        };
        public static Dictionary<string, string> TypeRGoiDichVuNhatKyCT = new Dictionary<string, string>()
        {
             { ColumnInvoices.bienso.ToString(),"Biển số xe" },
             { ColumnInvoices.machuxe.ToString(),"Mã chủ xe" },
             { ColumnInvoices.tenchuxe.ToString(),"Tên chủ xe" },
            { TypeRGoiDichVu.makhach.ToString(),"Mã khách" },
             { TypeRGoiDichVu.tenkhach.ToString(),"Tên khách" },
             { TypeRGoiDichVu.nhomkhach.ToString(),"Nhóm khách" },
             { TypeRGoiDichVu.nguonkhach.ToString(),"Nguồn khách" },
             { TypeRGoiDichVu.dienthoai.ToString(),"Điện thoại" },
             { TypeRGoiDichVu.gioitinh.ToString(),"Giới tính" },
             { TypeRGoiDichVu.nguoigioithieu.ToString(),"Người giới thiệu" },
             { TypeRGoiDichVu.magoi.ToString(),"Mã gói DV" },
             { TypeRGoiDichVu.mahoadon.ToString(),"Mã hóa đơn" },
             { TypeRGoiDichVu.ngayapdung.ToString(),"Ngày sử dụng" },
             { TypeRGoiDichVu.nhomhanghoa.ToString(),"Nhóm hàng hóa" },
             { TypeRGoiDichVu.mahanghoa.ToString(),"Mã hàng hóa" },
             { TypeRGoiDichVu.tenhanghoa.ToString(),"Tên hàng hóa" },
             { TypeRGoiDichVu.donvitinh.ToString(),"Đvt" },
             { TypeRGoiDichVu.lohanghoa.ToString(),"Lô hàng" },
             { TypeRGoiDichVu.soluongban.ToString(),"Số lượng" },
             { TypeReportTotal.Total_doanhthuthuan.ToString(),"Giá trị" },
             { TypeReportTotal.Total_tienvon.ToString(),"Tiền vốn" },
             { TypeRGoiDichVu.nhanvien.ToString(),"Nhân viên thực hiện" },
             { TypeRGoiDichVu.ghichu.ToString(),"Ghi chú" },

        };
        public static Dictionary<string, string> TypeRGoiDichVuTonChuaSD = new Dictionary<string, string>()
        {
             { TypeRGoiDichVu.mahanghoa.ToString(),"Mã hàng hóa" },
             { TypeRGoiDichVu.tenhanghoa.ToString(),"Tên hàng hóa" },
             { TypeRGoiDichVu.donvitinh.ToString(),"Đvt" },
             { TypeRGoiDichVu.lohanghoa.ToString(),"Lô hàng" },
             { TypeRGoiDichVu.soluongban.ToString(),"Số lượng bán" },
             { TypeRGoiDichVu.giatriban.ToString(),"Giá trị bán" },
             { TypeRGoiDichVu.soluongtra.ToString(),"Số lượng trả" },
             { TypeRGoiDichVu.giatritra.ToString(),"Giá trị trả" },
             { TypeRGoiDichVu.soluongsudung.ToString(),"Số lượng sử dụng" },
             { TypeRGoiDichVu.giatrisudung.ToString(),"Giá trị sử dụng" },
             { TypeRGoiDichVu.soluongconlai.ToString(),"Số lượng còn lại" },
             { TypeRGoiDichVu.giatriconlai.ToString(),"Giá trị còn lại" },


        };
        public static Dictionary<string, string> TypeRGoiDichVuNhatXuatTon = new Dictionary<string, string>()
        {
             { TypeRGoiDichVu.mahanghoa.ToString(),"Mã hàng hóa" },
             { TypeRGoiDichVu.tenhanghoa.ToString(),"Tên hàng hóa" },
             { TypeRGoiDichVu.donvitinh.ToString(),"Đvt" },
             { TypeRGoiDichVu.lohanghoa.ToString(),"Lô hàng" },
             { TypeRGoiDichVu.tondau.ToString(),"Tồn đầu" },
             { TypeRGoiDichVu.giatriban.ToString(),"Giá trị đầu kỳ" },
             { TypeRGoiDichVu.trongky.ToString(),"Trong kỳ" },
             { TypeRGoiDichVu.toncuoi.ToString(),"Tồn cuối" },
             { TypeRGoiDichVu.giatriconlai.ToString(),"Giá trị cuối kỳ" },


        };
        public enum TypeRChietKhau
        {
            manhanvien = 1,
            tennhanvien,
            hoahongthuchien,
            hoahongthuchien_theoyc,
            hoahongtuvan,
            hoahongdichvu,
            tong,
            mahoadon,
            ngaylap,
            mahang,
            tenhang,
            donvitinh,
            lohang,
            soluong,
            giatritinhck,
            gtriSauHeSo,
            makhachhang,
            tenkhachhang,
            dienthoaikh,
            tennhomhang
        }
        public static Dictionary<string, string> TypeRChietKhauTH = new Dictionary<string, string>()
        {
             { TypeRChietKhau.manhanvien.ToString(),"Mã nhân viên" },
             { TypeRChietKhau.tennhanvien.ToString(),"Tên nhân viên" },
             { TypeRChietKhau.hoahongthuchien.ToString(),"Hoa hồng thực hiện" },
             { TypeRChietKhau.hoahongthuchien_theoyc.ToString(),"Hoa hồng NV hỗ trợ" },
             { TypeRChietKhau.hoahongtuvan.ToString(),"Hoa hồng tư vấn" },
             { TypeRChietKhau.hoahongdichvu.ToString(),"Hoa hồng bán gói dịch vụ" },
             { TypeRChietKhau.tong.ToString(),"Tổng" },
        };
        public static Dictionary<string, string> TypeRChietKhauCT = new Dictionary<string, string>()
        {
             { TypeRChietKhau.mahoadon.ToString(),"Mã hóa đơn" },
             { TypeRChietKhau.ngaylap.ToString(),"Ngày lập hóa đơn" },
             { TypeRChietKhau.makhachhang.ToString(),"Mã khách hàng" },
             { TypeRChietKhau.tenkhachhang.ToString(),"Tên khách hàng" },
             { TypeRChietKhau.dienthoaikh.ToString(),"Điện thoại" },
             {ColumnKhachHang. nvphutrach.ToString(),"NV phụ trách" },
             { TypeRChietKhau.tennhomhang.ToString(),"Tên nhóm hàng" },
             { TypeRChietKhau.mahang.ToString(),"Mã hàng/dịch vụ" },
             { TypeRChietKhau.tenhang.ToString(),"Tên hàng/dịch vụ" },
             { TypeRChietKhau.donvitinh.ToString(),"Đvt" },
             { TypeRChietKhau.lohang.ToString(),"Lô hàng" },
             { TypeRChietKhau.manhanvien.ToString(),"Mã nhân viên" },
             { TypeRChietKhau.tennhanvien.ToString(),"Tên nhân viên" },
             { TypeRChietKhau.soluong.ToString(),"Số lượng" },
             { TypeRChietKhau.giatritinhck.ToString(),"Giá trị tính CK" },
             { ColumnReportNhanVien.heso.ToString(),"Hệ số" },
             { TypeRChietKhau.gtriSauHeSo.ToString(),"Giá trị tính" },
             { TypeRChietKhau.hoahongthuchien.ToString(),"Hoa hồng thực hiện" },
             { TypeRChietKhau.hoahongthuchien_theoyc.ToString(),"Hoa hồng NV hỗ trợ" },
             { TypeRChietKhau.hoahongtuvan.ToString(),"Hoa hồng tư vấn" },
             { TypeRChietKhau.hoahongdichvu.ToString(),"Hoa hồng bán gói dịch vụ" },
             { TypeRChietKhau.tong.ToString(),"Tổng" },
        };

        public enum TypeTime
        {
            none = 0,
            minutes5,
            minutes10,
            minutes15,
            minutes30,
            hours1,
            hours2,
            hours3,
            hours4,
            hours5,
            hours6,
            hours7,
            hours8,
            hours9,
            hours10,
            hours11,
            hours18,
            days05,
            days1,
            days2,
            days3,
        }

        public static Dictionary<int, string> TypeTimeRemind = new Dictionary<int, string>()
        {
             {(int) TypeTime.none,"Không" },
             { (int)TypeTime.minutes5,"5 phút" },
             { (int)TypeTime.minutes10,"10 phút" },
             { (int)TypeTime.minutes15,"15 phút" },
             { (int)TypeTime.minutes30,"30 phút" },
             { (int)TypeTime.hours1,"1 tiếng" },
             { (int)TypeTime.hours2,"2 tiếng" },
             { (int)TypeTime.hours3,"3 tiếng" },
             { (int)TypeTime.hours4,"4 tiếng" },
             { (int)TypeTime.hours5,"5 tiếng" },
             { (int)TypeTime.hours6,"6 tiếng" },
              { (int)TypeTime.hours7,"7 tiếng" },
             { (int)TypeTime.hours8,"8 tiếng" },
             { (int)TypeTime.hours9,"9 tiếng" },
             { (int)TypeTime.hours10,"10 tiếng" },
             { (int)TypeTime.hours11,"11 tiếng" },
             { (int)TypeTime.hours18,"18 tiếng" },
             { (int)TypeTime.days05,"0.5 ngày" },
             { (int)TypeTime.days1,"1 ngày" },
             { (int)TypeTime.days2,"2 ngày" },
             { (int)TypeTime.days3,"3 ngày" },
        };

        public enum ColumnKhachHang
        {
            madoituong,
            tendoituong,
            dienthoai,
            nhomkhach,
            gioitinh,
            ngaysinh,
            email,
            masothue,
            sotaikhoan,
            diachi,
            tinhthanh,
            quanhuyen,
            nguonkhach,
            nguoigioithieu,
            nvphutrach,
            nguoitao,
            ngaytao,
            nohientai,
            tongban,
            tongbantrutrahang,
            tongtichdiem,
            ngaygiaodichgannhat,
            trangthaikhachhang,
            ghichu,
            trangthaiSoDuCoc,
            gtriNapCoc,
            gtriSuDungCoc,
            gtriSoDuCoc,
            laCaNhan,
            manvphutrach,
            tongthuKhach,
            tongChiKhach,
            gtriDVSuDung,
            gtriDVKhachTra
        }

        public static Dictionary<string, string> ListKhachHang = new Dictionary<string, string>()
        {
             { ColumnKhachHang. madoituong.ToString(),"Mã khách hàng" },
             { ColumnKhachHang. tendoituong.ToString(),"Tên khách hàng" },
             { ColumnKhachHang. dienthoai.ToString(),"Điện thoại" },
             {ColumnKhachHang. nhomkhach.ToString(),"Nhóm khách" },
             {ColumnKhachHang.gioitinh.ToString(),"Giới tính" },
             { ColumnKhachHang. ngaysinh.ToString(),"Ngày sinh" },
             {ColumnKhachHang. email.ToString(),"Email" },
             {ColumnKhachHang. masothue.ToString(),"Mã số thuế" },
             {ColumnKhachHang. sotaikhoan.ToString(),"Số tài khoản" },
             {ColumnKhachHang. diachi.ToString(),"Địa chỉ" },
                {ColumnKhachHang.tinhthanh.ToString(),"Tỉnh thành" },
                {ColumnKhachHang. quanhuyen.ToString(),"Quận huyện" },
                {ColumnKhachHang. nguonkhach.ToString(),"Nguồn khách" },
                {ColumnKhachHang. nguoigioithieu.ToString(),"Người giới thiệu" },
                {ColumnKhachHang. nvphutrach.ToString(),"NV phụ trách" },
                { ColumnKhachHang. nguoitao.ToString(),"Người tạo" },
                {ColumnKhachHang. ngaytao.ToString(),"Ngày tạo" },
                {ColumnKhachHang.nohientai.ToString(),"Nợ hiện tại" },
                { ColumnKhachHang.tongban.ToString(),"Tổng bán" },
            {ColumnKhachHang.tongbantrutrahang.ToString(),"Tổng bán trừ trả hàng" },
            {ColumnKhachHang.tongthuKhach.ToString(),"Tổng gtrị thanh toán" },
            {ColumnKhachHang.tongChiKhach.ToString(),"Tổng hoàn cọc" },
            {ColumnKhachHang.gtriDVSuDung.ToString(),"Tổng gtrị sử dụng" },
            {ColumnKhachHang.gtriDVKhachTra.ToString(),"Tổng hoàn dịch vụ" },
             { ColumnKhachHang. tongtichdiem.ToString(),"Tổng tích điểm" },
             { ColumnKhachHang. ngaygiaodichgannhat.ToString(),"Ngày giao dịch gần nhất" },
             {ColumnKhachHang.trangthaikhachhang.ToString(),"Trạng thái khách hàng" },
             { ColumnKhachHang. ghichu.ToString(),"Ghi chú" },
        };

        public enum ColumnNhaCungCap
        {
            madoituong,
            tendoituong,
            dienthoai,
            nhomkhach,
            email,
            diachi,
            tinhthanh,
            quanhuyen,
            nguoitao,
            ngaytao,
            nohientai,
            tongmua,
            cpDichvu,
            ghichu,
            napcoc,
            sudungcoc,
            soducoc
        }

        public static Dictionary<string, string> ListNhaCungCap = new Dictionary<string, string>()
        {
             { ColumnNhaCungCap. madoituong.ToString(),"Mã nhà cung cấp" },
             { ColumnNhaCungCap. tendoituong.ToString(),"Tên nhà cung cấp" },
             { ColumnNhaCungCap. dienthoai.ToString(),"Điện thoại" },
             {ColumnNhaCungCap. nhomkhach.ToString(),"Nhóm NCC" },
             {ColumnNhaCungCap. email.ToString(),"Email" },
             {ColumnNhaCungCap. diachi.ToString(),"Địa chỉ" },
                {ColumnNhaCungCap.tinhthanh.ToString(),"Tỉnh thành" },
                {ColumnNhaCungCap. quanhuyen.ToString(),"Quận huyện" },
                { ColumnNhaCungCap. nguoitao.ToString(),"Người tạo" },
                {ColumnNhaCungCap.nohientai.ToString(),"Nợ hiện tại" },
                { ColumnNhaCungCap.tongmua.ToString(),"Tổng mua" },
                { ColumnNhaCungCap.napcoc.ToString(),"Tổng nạp cọc" },
                { ColumnNhaCungCap.sudungcoc.ToString(),"Sử dụng cọc" },
                { ColumnNhaCungCap.soducoc.ToString(),"Số dư cọc" },
                { ColumnNhaCungCap.ghichu.ToString(),"Ghi chú" },
        };

        public enum ColumnReportNhanVien
        {
            manhanvien,
            tennhanvien,
            gioitinh,
            ngaysinh,
            dantoc,
            socmnd,
            sodienthoai,
            diachi,
            ngayvaolam,
            phongban, ghichu,
            ngayvaodoan,
            noivaodoan,
            ngaynhapngu,
            ngayxuatngu,
            ngayvaodang,
            ngayvaodangchinhthuc,
            ngayroidang,
            noisinhhoatdang,
            ttkhac,
            loaihopdong, sohopdong,
            ngayky, thoihan, trangthai,
            loaibaohiem, sobaohiem, ngaycap, ngayhethan, noicap,
            tuoi, hinhthuc, soquyetdinh, noidung,
            loailuong, ngayapdung, ngayketthuc, sotien, heso, bac,
            khoanmiengiam,
            tungay, denngay, noihoc, nganhhoc, hedaotao, bangcap,
            coquan, vitri,
            tennguoithan, ngaysinhnew, quanhe,
            ngaykham, chieucao, cannang, tinhhinhsuckhoe

        }
        public static Dictionary<string, string> ListReportNVTongHop = new Dictionary<string, string>()
        {
             { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
            { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.dantoc.ToString(),"Dân tộc" },
              { ColumnReportNhanVien.socmnd.ToString(),"Số CMND" },
              { ColumnReportNhanVien.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnReportNhanVien.diachi.ToString(),"Địa chỉ" },
              { ColumnReportNhanVien.ngayvaolam.ToString(),"Ngày vào làm" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.ghichu.ToString(),"Ghi chú" },
              { ColumnReportNhanVien.ngayvaodoan.ToString(),"Ngày vào đoàn" },
              { ColumnReportNhanVien.noivaodoan.ToString(),"Nơi vào đoàn" },
              { ColumnReportNhanVien.ngaynhapngu.ToString(),"Ngày nhập ngũ" },
              { ColumnReportNhanVien.ngayxuatngu.ToString(),"Ngày xuất ngũ" },
              { ColumnReportNhanVien.ngayvaodang.ToString(),"Ngày vào đảng" },
              { ColumnReportNhanVien.ngayvaodangchinhthuc.ToString(),"Ngày vào đảng chính thức" },
              { ColumnReportNhanVien.ngayroidang.ToString(),"Ngày rời đảng" },
              { ColumnReportNhanVien.noisinhhoatdang.ToString(),"nơi sinh hoạt đảng" },
              { ColumnReportNhanVien.ttkhac.ToString(),"Thông tin khác" },
        };
        public static Dictionary<string, string> ListReportNVHopDong = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.loaihopdong.ToString(),"Loại hợp đồng" },
               { ColumnReportNhanVien.sohopdong.ToString(),"Số hợp đồng" },
               { ColumnReportNhanVien.ngayky.ToString(),"Ngày ký" },
               { ColumnReportNhanVien.thoihan.ToString(),"Thời hạn" },
               { ColumnReportNhanVien.trangthai.ToString(),"Trạng thái" },

        };
        public static Dictionary<string, string> ListReportNVBaoHiem = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.loaibaohiem.ToString(),"Loại bảo hiểm" },
               { ColumnReportNhanVien.sobaohiem.ToString(),"Số bảo hiểm" },
               { ColumnReportNhanVien.ngaycap.ToString(),"Ngày cấp" },
               { ColumnReportNhanVien.ngayhethan.ToString(),"Ngày hết hạn" },
               { ColumnReportNhanVien.noicap.ToString(),"Nơi cấp" },
                { ColumnReportNhanVien.trangthai.ToString(),"Trạng thái" },

        };
        public static Dictionary<string, string> ListReportNVTuoi = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.tuoi.ToString(),"Tuổi" },
               { ColumnReportNhanVien.ghichu.ToString(),"Ghi chú" },
                { ColumnReportNhanVien.trangthai.ToString(),"Trạng thái" },

        };
        public static Dictionary<string, string> ListReportNVKyluat = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.hinhthuc.ToString(),"Hình thức" },
              { ColumnReportNhanVien.soquyetdinh.ToString(),"Số quyết định" },
              { ColumnReportNhanVien.noidung.ToString(),"Nội dung tóm tắt" },
              { ColumnReportNhanVien.ngayapdung.ToString(),"Ngày ban hành" },
        };
        public static Dictionary<string, string> ListReportNVPhuCap = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.loailuong.ToString(),"Loại lương" },
              { ColumnReportNhanVien.ngayapdung.ToString(),"Ngày áp dụng" },
              { ColumnReportNhanVien.ngayketthuc.ToString(),"Ngày kết thúc" },
              { ColumnReportNhanVien.sotien.ToString(),"Số tiền" },
              { ColumnReportNhanVien.heso.ToString(),"Hệ số" },
              { ColumnReportNhanVien.bac.ToString(),"Bậc" },
              { ColumnReportNhanVien.noidung.ToString(),"Nội dung" },
              { ColumnReportNhanVien.trangthai.ToString(),"Trạng thái" },
        };
        public static Dictionary<string, string> ListReportNVThue = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.khoanmiengiam.ToString(),"Khoản miễn giảm" },
              { ColumnReportNhanVien.ngayapdung.ToString(),"Ngày áp dụng" },
              { ColumnReportNhanVien.ngayketthuc.ToString(),"Ngày kết thúc" },
              { ColumnReportNhanVien.sotien.ToString(),"Số tiền" },
              { ColumnReportNhanVien.ghichu.ToString(),"Ghi chú" },
        };
        public static Dictionary<string, string> ListReportNVDaoTao = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.tungay.ToString(),"Từ ngày" },
              { ColumnReportNhanVien.denngay.ToString(),"Đến ngày" },
              { ColumnReportNhanVien.noihoc.ToString(),"Nơi học" },
              { ColumnReportNhanVien.nganhhoc.ToString(),"Ngành học" },
              { ColumnReportNhanVien.hedaotao.ToString(),"Hệ đào tạo" },
              { ColumnReportNhanVien.bangcap.ToString(),"Bằng cấp" },
        };
        public static Dictionary<string, string> ListReportNVCongTac = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.tungay.ToString(),"Từ ngày" },
              { ColumnReportNhanVien.denngay.ToString(),"Đến ngày" },
              { ColumnReportNhanVien.coquan.ToString(),"Cơ quan" },
              { ColumnReportNhanVien.vitri.ToString(),"Vị trí" },
              { ColumnReportNhanVien.diachi.ToString(),"Đại chỉ" },
        };
        public static Dictionary<string, string> ListReportNVGiaDinh = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.tennguoithan.ToString(),"Họ và tên "},
              { ColumnReportNhanVien.ngaysinhnew.ToString(),"Ngày sinh"},
              { ColumnReportNhanVien.quanhe.ToString(),"Quan hệ"},
              { ColumnReportNhanVien.diachi.ToString(),"Địa chỉ"},
        };
        public static Dictionary<string, string> ListReportNVSucKhoe = new Dictionary<string, string>()
        {
              { ColumnReportNhanVien.manhanvien.ToString(),"Mã nhân viên" },
              { ColumnReportNhanVien.tennhanvien.ToString(),"Tên nhân viên" },
              { ColumnReportNhanVien.gioitinh.ToString(),"Giới tính" },
              { ColumnReportNhanVien.ngaysinh.ToString(),"Ngày sinh" },
              { ColumnReportNhanVien.phongban.ToString(),"Phòng ban" },
              { ColumnReportNhanVien.ngaykham.ToString(),"Ngày khám"},
              { ColumnReportNhanVien.chieucao.ToString(),"Chiều cao"},
              { ColumnReportNhanVien.cannang.ToString(),"Cân nặng"},
              { ColumnReportNhanVien.tinhhinhsuckhoe.ToString(),"Tình hình sức khỏe"},
        };
        public enum HienThiMauIn
        {
            TenCongTy = 1,
            TenSP,
            MaSP,
            Gia,
        }

        public enum colummReportDinhLuong
        {
            loaichungtu = 0,
            machungtu,
            ngaychungtu,
            maphieutiepnhan,
            biensoxe,
            nhomdichvu,
            madichvu,
            tendichvu,
            dv_donvitinh,
            dv_soluong,
            dv_giatri,
            nhomhang,
            mahanghoa,
            tenhanghoa,
            hh_donvitinh,
            hh_soluong,
            hh_giatri,
            tt_soluong,
            tt_giatri,
            cl_giatri,
            cl_soluong,
            trangthai,
            machinhanh,
            tenchinhanh,
            ghichuct,
            nhanvienlap
        }
        public static Dictionary<string, string> ListReportDinhLuong = new Dictionary<string, string>()
        {
              { colummReportDinhLuong.loaichungtu.ToString(),"Loại chứng từ" },
              { colummReportDinhLuong.machungtu.ToString(),"Mã chứng từ" },
              { colummReportDinhLuong.ngaychungtu.ToString(),"Ngày chứng từ" },
              { colummReportDinhLuong.maphieutiepnhan.ToString(),"Mã phiếu tiếp nhận" },
              { colummReportDinhLuong.biensoxe.ToString(),"Biển số xe" },
              { ColumnInvoices.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnInvoices.tenkhachhang.ToString(),"Tên khách hàng" },
              { colummReportDinhLuong.machinhanh.ToString(),"Mã chi nhánh" },
              { colummReportDinhLuong.tenchinhanh.ToString(),"Tên chi nhánh" },
              { colummReportDinhLuong.nhanvienlap.ToString(),"NV lập hóa đơn" },
              { colummReportDinhLuong.nhomdichvu.ToString(),"Nhóm dịch vụ" },
              { colummReportDinhLuong.madichvu.ToString(),"Mã dịch vụ" },
              { colummReportDinhLuong.tendichvu.ToString(),"Tên dịch vụ" },
              { colummReportDinhLuong.dv_donvitinh.ToString(),"Đvt dịch vụ" },
              { colummReportDinhLuong.dv_soluong.ToString(),"Số lượng dịch vụ" },
              { TypeReportDetail.Detail_giaban.ToString(),"Giá bán" },
              { TypeReportDetail.Detail_thanhtien.ToString(),"Thành tiền" },
              { colummReportDinhLuong.dv_giatri.ToString(),"Giá trị dịch vụ" },
              { TypeReportprofit.profit_tysuat.ToString(),"% Sử dụng" },// giatri/thanhtien *100
              { ColumnValueCard.nvthuchien.ToString(),"NV thực hiện" },
              { colummReportDinhLuong.nhomhang.ToString(),"Nhóm hàng hóa" },
              { colummReportDinhLuong.mahanghoa.ToString(),"Mã hàng hóa" },
              { colummReportDinhLuong.tenhanghoa.ToString(),"Tên hàng hóa" },
              { colummReportDinhLuong.hh_donvitinh.ToString(),"Đvt hàng hóa" },
              { colummReportDinhLuong.hh_soluong.ToString(),"Số lượng hàng hóa" },
              { colummReportDinhLuong.hh_giatri.ToString(),"Giá vốn tiêu chuẩn" },
              { colummReportDinhLuong.ghichuct.ToString(),"Ghi chú" },
              { colummReportDinhLuong.tt_soluong.ToString(),"Số lượng đã xuất" },
              { colummReportDinhLuong.tt_giatri.ToString(),"Giá trị đã xuất" },
              { colummReportDinhLuong.cl_soluong.ToString(),"Số lượng chênh lệch" },
              { colummReportDinhLuong.cl_giatri.ToString(),"Giá trị chênh lệch" },
              { colummReportDinhLuong.trangthai.ToString(),"Trạng thái" },
        };

        public static Dictionary<string, string> ListReportNhomHoTro = new Dictionary<string, string>()
        {
              { TypeRpDiscountInvoice.tennhanvien.ToString(),"Tư vấn phụ trách" },
              { ColumnInvoices.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnInvoices.tenkhachhang.ToString(),"Tên khách hàng" },
              { colummReportDinhLuong.tenchinhanh.ToString(),"Tên chi nhánh" },
              { colummReportDinhLuong.nhomdichvu.ToString(),"Nhóm dịch vụ" },
              { colummReportDinhLuong.dv_giatri.ToString(),"Giá trị nhóm dịch vụ mua" },
              { colummReportDinhLuong.hh_giatri.ToString(),"Hỗ trợ theo quy định" },
              { colummReportDinhLuong.tt_giatri.ToString(),"Đã hỗ trợ" },
              { TypeReportprofit.profit_tysuat.ToString(),"Mức vượt" },
        };
        public enum columncalamviec
        {
            maca = 0,
            tenca,
            trangthai,
            giovao,
            giora,
            nghigiuacatu,
            nghigiuacaden,
            giolamthemngaytu,
            giolamthemngayden,
            giolamthemdemtu,
            giolamthemdemden,
            tongconggio,
            cachlaygiocong,
            lacadem,
            nguoitao,
            ngaytao,
            ghichu

        }
        public static Dictionary<string, string> listdanhmucalamviec = new Dictionary<string, string>()
        {
              { columncalamviec.maca.ToString(),"Mã ca" },
              { columncalamviec.tenca.ToString(),"Tên ca" },
              { columncalamviec.trangthai.ToString(),"Trạng thái" },
              { columncalamviec.giovao.ToString(),"Giờ vào" },
              { columncalamviec.giora.ToString(),"Giờ ra" },
              { columncalamviec.nghigiuacatu.ToString(),"Nghỉ giữa ca từ" },
              { columncalamviec.nghigiuacaden.ToString(),"Nghỉ giữa ca đến" },
              { columncalamviec.giolamthemngaytu.ToString(),"Giờ làm thêm ban ngày từ" },
              { columncalamviec.giolamthemngayden.ToString(),"Giờ làm thêm ban ngày đến" },
              { columncalamviec.giolamthemdemtu.ToString(),"Giờ làm thêm ban đêm từ" },
              { columncalamviec.giolamthemdemden.ToString(),"Giờ làm thêm ban đêm đến" },
              { columncalamviec.tongconggio.ToString(),"Tổng công giờ" },
              { columncalamviec.cachlaygiocong.ToString(),"Cách lấy giờ công" },
              { columncalamviec.lacadem.ToString(),"Là ca đêm" },
              { columncalamviec.ngaytao.ToString(),"Ngày tạo" },
              { columncalamviec.nguoitao.ToString(),"Người tạo" },
              { columncalamviec.ghichu.ToString(),"Ghi chú" },

        };

        public enum columnphieuphanca
        {
            maphieu,
            loaica,
            trangthai,
            tungay,
            denngay,
            nguoitao,
            ngaytao,
            ghichu

        }
        public static Dictionary<string, string> ListPhieuPhanCa = new Dictionary<string, string>()
        {
              { columnphieuphanca.maphieu.ToString(),"Mã phiếu" },
              { columnphieuphanca.loaica.ToString(),"Loại phiếu" },
              { columnphieuphanca.trangthai.ToString(),"Trạng thái" },
              { columnphieuphanca.tungay.ToString(),"Từ ngày" },
              { columnphieuphanca.denngay.ToString(),"Đến ngày" },
              { columnphieuphanca.nguoitao.ToString(),"Người tạo" },
              { columnphieuphanca.ngaytao.ToString(),"Ngày tạo" },
              { columnphieuphanca.ghichu.ToString(),"Ghi chú" },

        };

        public enum ChungTuApDung
        {
            BanHang = 1,
            DatHang = 3,
            TraHang = 6,
            GoiDichVu = 19,
            TheGiaTri = 22,
            HoaDonSuaChua = 25,
        }

        public static Dictionary<int, string> ListChungTuApDung = new Dictionary<int, string>()
        {
              { (int)ChungTuApDung.BanHang,"Bán hàng"},
              { (int)ChungTuApDung.TraHang,"Trả hàng" },
              { (int)ChungTuApDung.GoiDichVu,"Gói dịch vụ" },
              { (int)ChungTuApDung.TheGiaTri,"Thẻ giá trị" },
              { (int)ChungTuApDung.HoaDonSuaChua,"Hóa đơn sửa chữa" },
        };

        public enum ColumnReportValueCard_Balance
        {
            makhachhang,
            tenkhachhang,
            dienthoaikhachhang,
            trangthaithegiatri,
            sodudauky,
            phatsinhtang,
            phatsinhgiam,
            soducuoiky,
        }

        public static Dictionary<string, string> ReportValueCard_Balance = new Dictionary<string, string>()
        {
             { ColumnReportValueCard_Balance. makhachhang.ToString(),"Mã khách hàng" },
             { ColumnReportValueCard_Balance. tenkhachhang.ToString(),"Tên khách hàng" },
             { ColumnReportValueCard_Balance. dienthoaikhachhang.ToString(),"Điện thoại" },
             {ColumnReportValueCard_Balance. trangthaithegiatri.ToString(),"Trạng thái thẻ" },
             {ColumnReportValueCard_Balance.sodudauky.ToString(),"Số dư đầu kỳ" },
             { ColumnReportValueCard_Balance. phatsinhtang.ToString(),"Tăng" },
             {ColumnReportValueCard_Balance. phatsinhgiam.ToString(),"Giảm" },
             {ColumnReportValueCard_Balance. soducuoiky.ToString(),"Số dư cuối kỳ" },
        };

        public enum ColumnReportValueCard_HisUsed
        {
            ngaylaphoadon,
            makhachhang,
            tenkhachhang,
            loaichungtu,
            mahoadon,
            maphieuthuchi,
            sodudauky,
            phatsinhtang,
            phatsinhgiam,
            soducuoiky,
            mahanghoa,
            tenhanghoa,
            soluong,
            dongia,
            tienchietkhau,
            thanhtien,
        }

        public static Dictionary<string, string> ReportValueCard_HisUsed = new Dictionary<string, string>()
        {
             { ColumnReportValueCard_HisUsed. ngaylaphoadon.ToString(),"Ngày" },
             { ColumnReportValueCard_HisUsed. makhachhang.ToString(),"Mã khách hàng" },
             { ColumnReportValueCard_HisUsed. tenkhachhang.ToString(),"Tên khách hàng" },
             { ColumnReportValueCard_HisUsed. loaichungtu.ToString(),"Hoạt động" },
             {ColumnReportValueCard_HisUsed. mahoadon.ToString(),"Hóa đơn liên quan" },
             {ColumnReportValueCard_HisUsed. maphieuthuchi.ToString(),"Phiếu sử dụng " },
             {ColumnReportValueCard_HisUsed.sodudauky.ToString(),"Số dư trước phát sinh" },
             {ColumnReportValueCard_HisUsed. phatsinhgiam.ToString(),"Phát sinh giảm" },
             { ColumnReportValueCard_HisUsed. phatsinhtang.ToString(),"Phát sinh tăng" },
             {ColumnReportValueCard_HisUsed. soducuoiky.ToString(),"Số dư sau phát sinh" },
        };

        public static Dictionary<string, string> ReportValueCard_ServiceUsed = new Dictionary<string, string>()
        {
             { ColumnReportValueCard_HisUsed. ngaylaphoadon.ToString(),"Ngày sử dụng" },
             { ColumnReportValueCard_HisUsed. makhachhang.ToString(),"Mã khách hàng" },
             { ColumnReportValueCard_HisUsed. tenkhachhang.ToString(),"Tên khách hàng" },
             { ColumnReportValueCard_HisUsed. loaichungtu.ToString(),"Hoạt động" },
             { ColumnReportValueCard_HisUsed. mahoadon.ToString(),"Hóa đơn liên quan" },
             {ColumnReportValueCard_HisUsed. maphieuthuchi.ToString(),"Phiếu sử dụng " },
             {ColumnReportValueCard_HisUsed. phatsinhgiam.ToString(),"Phát sinh giảm" },
             { ColumnReportValueCard_HisUsed. phatsinhtang.ToString(),"Phát sinh tăng" },
             {ColumnReportValueCard_HisUsed. mahanghoa.ToString(),"Mã hàng hóa" },
             {ColumnReportValueCard_HisUsed. tenhanghoa.ToString(),"Tên hàng hóa" },
             {ColumnReportValueCard_HisUsed.soluong.ToString(),"Số lượng" },
             { ColumnReportValueCard_HisUsed. dongia.ToString(),"Đơn giá" },
             {ColumnReportValueCard_HisUsed. tienchietkhau.ToString(),"Chiết khấu" },
             {ColumnReportValueCard_HisUsed. thanhtien.ToString(),"Thành tiền" },
             {TypeRpDiscountInvoice.manhanvien.ToString(),"Mã nhân viên" },
             {TypeRpDiscountInvoice. tennhanvien.ToString(),"Tên nhân viên" },
        };
        public enum ECashFlow
        {
            loaiphieu,
            maphieu,
            ngaylapphieu,
            loaithuchi,
            giatri,
            hinhthuc,
            taikhoanchuyen,
            taikhoanpos,
            manguoinop,
            tennguoinop,
            dienthoai,
            nguonkhach,
            ghichu,
            trangthai,
            nvlapphieu,
            chinhanh,
        }

        public static Dictionary<string, string> ListCheckCashFlow = new Dictionary<string, string>()
        {
             { ECashFlow. loaiphieu.ToString(),"Loại phiếu" },
             { ECashFlow. maphieu.ToString(),"Mã phiếu" },
             { ECashFlow. ngaylapphieu.ToString(),"Thời gian" },
             { ECashFlow. loaithuchi.ToString(),"Loại thu chi" },
             { ECashFlow. giatri.ToString(),"Giá trị" },
             {ECashFlow. hinhthuc.ToString(),"Hình thức" },
             {ECashFlow. taikhoanchuyen.ToString(),"Tài khoản chuyển" },
             {ECashFlow. taikhoanpos.ToString(),"Tài khoản POS" },
             {ECashFlow. manguoinop.ToString(),"Mã người nộp" },
             {ECashFlow.tennguoinop.ToString(),"Tên người nộp" },
             { ECashFlow. dienthoai.ToString(),"Điện thoại" },
             { ColumnInvoices. diachi.ToString(),"Địa chỉ KH" },
             {ECashFlow. nguonkhach.ToString(),"Nguồn khách" },
             {ECashFlow. ghichu.ToString(),"Ghi chú" },
              {ECashFlow.trangthai.ToString(),"Trạng thái" },
             { ECashFlow. nvlapphieu.ToString(),"Nhân viên lập" },
             {ColumnInvoices. tenchinhanh.ToString(),"Chi nhánh" },
        };

        public enum TypeRpDiscountInvoice
        {
            manhanvien = 1,
            tennhanvien,
            hoahongdoanhthu,
            hoahongthucthu,
            hoahongvnd,
            tong,
            mahoadon,
            ngaylap,
            ngaylapphieuthu,
            ptdoanhthu,
            ptthucthu,
            thucthu,
            doanhthu,
            heso,
            makhachhang,
            tenkhachhang,
            dienthoaikh,

            theohanghoa,
            theohoadon,
            theodoanhso,
            tongckall,
            chiphiNganHang,
            thucthu_thuctinh
        }

        public static Dictionary<string, string> RpDiscountInvoice = new Dictionary<string, string>()
        {
             { TypeRpDiscountInvoice.manhanvien.ToString(),"Mã nhân viên" },
             { TypeRpDiscountInvoice.tennhanvien.ToString(),"Tên nhân viên" },
             { TypeRpDiscountInvoice.hoahongdoanhthu.ToString(),"Hoa hồng theo doanh thu" },
             { TypeRpDiscountInvoice.hoahongthucthu.ToString(),"Hoa hồng theo thực thu" },
             { TypeRpDiscountInvoice.hoahongvnd.ToString(),"Hoa hồng theo VNĐ" },
             { TypeRpDiscountInvoice.tong.ToString(),"Tổng" },
        };

        public static Dictionary<string, string> RpDiscountInvoice_Detail = new Dictionary<string, string>()
        {
             { TypeRpDiscountInvoice.mahoadon.ToString(),"Mã hóa đơn" },
             { TypeRpDiscountInvoice.ngaylap.ToString(),"Ngày lập hóa đơn" },
             { TypeRpDiscountInvoice.ngaylapphieuthu.ToString(),"Ngày lập phiếu thu" },
             { TypeRpDiscountInvoice.makhachhang.ToString(),"Mã khách hàng" },
             { TypeRpDiscountInvoice.tenkhachhang.ToString(),"Tên khách hàng" },
             { TypeRpDiscountInvoice.dienthoaikh.ToString(),"Điện thoại" },
             { ColumnKhachHang.manvphutrach.ToString(),"Mã NV phụ trách" },
             { ColumnKhachHang.nvphutrach.ToString(),"NV phụ trách" },
             { TypeRpDiscountInvoice.manhanvien.ToString(),"Mã nhân viên" },
             { TypeRpDiscountInvoice.tennhanvien.ToString(),"Tên nhân viên" },
             { TypeRpDiscountInvoice.doanhthu.ToString(),"Doanh thu" },
             { TypeRpDiscountInvoice.thucthu.ToString(),"Thực thu" },
             { TypeRpDiscountInvoice.chiphiNganHang.ToString(),"Chi phí ngân hàng" },
             { TypeRpDiscountInvoice.thucthu_thuctinh.ToString(),"Thực thu - thực tính" },
             { TypeRpDiscountInvoice.hoahongthucthu.ToString(),"Hoa hồng theo thực thu" },
             { TypeRpDiscountInvoice.hoahongvnd.ToString(),"Hoa hồng theo VNĐ" },
             { TypeRpDiscountInvoice.tong.ToString(),"Tổng" },
        };

        public static Dictionary<string, string> RpDiscountRevenue = new Dictionary<string, string>()
        {
             { TypeRpDiscountInvoice.manhanvien.ToString(),"Mã nhân viên" },
             { TypeRpDiscountInvoice.tennhanvien.ToString(),"Tên nhân viên" },
             { TypeRpDiscountInvoice.doanhthu.ToString(),"Doanh thu" },
             { TypeRpDiscountInvoice.thucthu.ToString(),"Thực thu" },
             { TypeRpDiscountInvoice.hoahongdoanhthu.ToString(),"Hoa hồng theo doanh thu" },
             { TypeRpDiscountInvoice.hoahongthucthu.ToString(),"Hoa hồng theo thực thu" },
             { TypeRpDiscountInvoice.tong.ToString(),"Tổng" },
        };

        public static Dictionary<string, string> RpDiscountAll = new Dictionary<string, string>()
        {
             { TypeRpDiscountInvoice.manhanvien.ToString(),"Mã nhân viên" },
             { TypeRpDiscountInvoice.tennhanvien.ToString(),"Tên nhân viên" },
             { TypeRpDiscountInvoice.theohanghoa.ToString(),"Theo hàng hóa" },
             { TypeRpDiscountInvoice.theohoadon.ToString(),"Theo hóa đơn" },
             { TypeRpDiscountInvoice.theodoanhso.ToString(),"Theo daonh số" },
             { TypeRpDiscountInvoice.tongckall.ToString(),"Tổng" },
        };

        public enum ColumnTransfers
        {
            mahoadon = 0,
            chinhanhchuyen,
            nguoichuyen,
            chinhanhnhan,
            nguoinhan,
            ngaychuyen,
            ngaynhan,
            giatrichuyen,
            giatrinhan,
            ghichu,
            trangthai,
        }
        public static Dictionary<string, string> ListColumnTransfers = new Dictionary<string, string>()
        {
              { ColumnTransfers.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnTransfers.chinhanhchuyen.ToString(),"Từ chi nhánh" },
              { ColumnTransfers.nguoichuyen.ToString(),"Người tạo" },
              { ColumnTransfers.chinhanhnhan.ToString(),"Tới chi nhánh" },
              { ColumnTransfers.nguoinhan.ToString(),"Người nhận" },
              { ColumnTransfers.ngaychuyen.ToString(),"Ngày chuyển" },
              { ColumnTransfers.ngaynhan.ToString(),"Ngày nhận" },
              { ColumnTransfers.giatrichuyen.ToString(),"Giá trị chuyển" },
              { ColumnTransfers.giatrinhan.ToString(),"Giá trị nhận" },
              { ColumnTransfers.ghichu.ToString(),"Ghi chú" },
              { ColumnTransfers.trangthai.ToString(),"Trạng thái" },
        };

        public enum ColumnInvoices
        {
            mahoadon = 0,
            madathang,
            ngaylaphoadon,
            maphieutiepnhan,
            bienso,
            makhachhang,
            tenkhachhang,
            email,
            sodienthoai,
            diachi,
            mabaohiem,
            tenbaohiem,
            tenchinhanh,
            nguoiban,
            nguoitao,
            thanhtienchuack,
            giamgiact,
            giatrisudung,
            tongtienhang,
            tongchiphi,
            tonggiamgia,
            tienthue,
            tongphaitra,
            khachcantra,
            butruTraHang,
            gtriSauTra,
            tongtienBHduyet,
            khautrutheovu,
            giamtruboithuong,
            BHchitratruocVAT,
            tongthueBH,

            tiencoc,
            tienmat,
            chuyenkhoan,
            pos,
            tiendoidiem,
            thegiatri,
            khachdatra,
            baohiemcantra,
            baohiemdatra,
            conno,
            ghichu,
            trangthai,
            machuxe,
            tenchuxe
        }
        public static Dictionary<string, string> ListColumnInvoices = new Dictionary<string, string>()
        {
              { ColumnInvoices.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnInvoices.madathang.ToString(),"Mã đặt hàng" },
              { ColumnInvoices.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnInvoices.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnInvoices.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnInvoices.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnInvoices.diachi.ToString(),"Địa chỉ" },
              { ColumnOrders.khuvuc.ToString(),"Tỉnh thành" },
              { ColumnInvoices.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnInvoices.nguoiban.ToString(),"Nhân viên bán" },
              { ColumnInvoices.nguoitao.ToString(),"Người lập hóa đơn" },
              { ColumnInvoices.thanhtienchuack.ToString(),"Tổng tiền chi tiết" },
              { ColumnInvoices.giamgiact.ToString(),"Giảm giá chi tiết" },
              { ColumnInvoices.giatrisudung.ToString(),"Giá trị sử dụng" },
              { ColumnInvoices.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnInvoices.tongchiphi.ToString(),"Tổng chi phí" },
              { ColumnInvoices.tienthue.ToString(),"Tiền thuế" },
              { ColumnInvoices.tonggiamgia.ToString(),"Tổng giảm giá" },
              { ColumnInvoices.tongphaitra.ToString(),"Tổng phải trả" },
              { ColumnInvoices.khachcantra.ToString(),"Khách cần trả" },
              { ColumnInvoices.khachdatra.ToString(),"Khách đã trả" },

              { ColumnInvoices.tienmat.ToString(),"Tiền mặt" },
              { ColumnInvoices.chuyenkhoan.ToString(),"Chuyển khoản" },
              { ColumnInvoices.pos.ToString(),"Tiền POS" },
              { ColumnInvoices.tiendoidiem.ToString(),"Tiền đổi điểm" },
              { ColumnInvoices.thegiatri.ToString(),"Tiền thẻ giá trị" },
              { ColumnInvoices.conno.ToString(),"Còn nợ" },
              { ColumnInvoices.ghichu.ToString(),"Ghi chú" },
              { ColumnInvoices.trangthai.ToString(),"Trạng thái" },
        };
        public static Dictionary<string, string> ListColumnInvoicesSuaChua = new Dictionary<string, string>()
        {
              { ColumnInvoices.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnInvoices.madathang.ToString(),"Mã đặt hàng" },
              { ColumnInvoices.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnInvoices.maphieutiepnhan.ToString(),"Mã phiếu tiếp nhận" },
              { ColumnInvoices.bienso.ToString(),"Biển số xe" },
              { ColumnInvoices.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnInvoices.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnInvoices.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnInvoices.diachi.ToString(),"Địa chỉ" },
              { ColumnOrders.khuvuc.ToString(),"Tỉnh thành" },
              { ColumnInvoices.mabaohiem.ToString(),"Mã cty bảo hiểm" },
              { ColumnInvoices.tenbaohiem.ToString(),"Tên cty bảo hiểm" },
              { ColumnInvoices.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnInvoices.nguoiban.ToString(),"Nhân viên bán" },
              { ColumnInvoices.nguoitao.ToString(),"Người lập hóa đơn" },
              { ColumnInvoices.thanhtienchuack.ToString(),"Tổng tiền chi tiết" },
              { ColumnInvoices.giamgiact.ToString(),"Giảm giá chi tiết" },
              { ColumnInvoices.giatrisudung.ToString(),"Giá trị sử dụng" },
              { ColumnInvoices.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnInvoices.tongchiphi.ToString(),"Tổng chi phí" },
              { ColumnInvoices.tienthue.ToString(),"Tiền thuế" },
              { ColumnInvoices.tonggiamgia.ToString(),"Tổng giảm giá" },
              { ColumnInvoices.tongphaitra.ToString(),"Tổng phải trả" },
              { ColumnInvoices.khachcantra.ToString(),"Khách cần trả" },
              { ColumnInvoices.khachdatra.ToString(),"Khách đã trả" },

               { ColumnInvoices.tongtienBHduyet.ToString(),"Tổng tiền BH duyệt" },
              { ColumnInvoices.khautrutheovu.ToString(),"Khấu trừ theo vụ" },
              { ColumnInvoices.giamtruboithuong.ToString(),"Chế tài" },
              { ColumnInvoices.BHchitratruocVAT.ToString(),"BH cần trả (trước VAT)" },
              { ColumnInvoices.tongthueBH.ToString(),"Tổng thuế bảo hiểm" },

              { ColumnInvoices.baohiemcantra.ToString(),"BH cần trả (sau VAT)" },
              { ColumnInvoices.baohiemdatra.ToString(),"Bảo hiểm đã trả" },
              { ColumnInvoices.tiencoc.ToString(),"Thu từ cọc" },
              { ColumnInvoices.tienmat.ToString(),"Tiền mặt" },
              { ColumnInvoices.chuyenkhoan.ToString(),"Chuyển khoản" },
              { ColumnInvoices.pos.ToString(),"Tiền POS" },
              { ColumnInvoices.tiendoidiem.ToString(),"Tiền đổi điểm" },
              { ColumnInvoices.thegiatri.ToString(),"Tiền thẻ giá trị" },
              { ColumnInvoices.conno.ToString(),"Còn nợ" },
              { ColumnInvoices.ghichu.ToString(),"Ghi chú" },
              { ColumnInvoices.trangthai.ToString(),"Trạng thái" },
        };

        public static Dictionary<string, string> ListColumnInvoicesBaoHanh = new Dictionary<string, string>()
        {
              { ColumnInvoices.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnInvoices.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnInvoices.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnInvoices.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnInvoices.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnInvoices.diachi.ToString(),"Địa chỉ" },
              { ColumnOrders.khuvuc.ToString(),"Tỉnh thành" },
              { ColumnInvoices.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnInvoices.nguoiban.ToString(),"Nhân viên bán" },
              { ColumnInvoices.nguoitao.ToString(),"Người lập hóa đơn" },
              { ColumnInvoices.ghichu.ToString(),"Ghi chú" },
              { ColumnInvoices.trangthai.ToString(),"Trạng thái" },
        };

        public enum ColumnOrders
        {
            madathang = 0,
            ngaylaphoadon,
            maphieutiepnhan,
            bienso,
            makhachhang,
            tenkhachhang,
            email,
            sodienthoai,
            diachi,
            khuvuc,
            phuongxa,
            tenchinhanh,
            nguoiban,
            nguoitao,
            tongtienhang,
            tonggiamgia,
            khachcantra,
            khachdatra,
            ghichu,
            trangthai,
        }
        public static Dictionary<string, string> ListColumnOrders = new Dictionary<string, string>()
        {
              { ColumnOrders.madathang.ToString(),"Mã đặt hàng" },
              { ColumnOrders.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
                { ColumnOrders.maphieutiepnhan.ToString(),"Mã phiếu tiếp nhận" },
              { ColumnOrders.bienso.ToString(),"Biển số xe" },
              { ColumnOrders.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnOrders.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnOrders.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnOrders.diachi.ToString(),"Địa chỉ" },
              { ColumnOrders.khuvuc.ToString(),"Tỉnh thành" },
              { ColumnOrders.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnOrders.nguoiban.ToString(),"Nhân viên bán" },
              { ColumnOrders.nguoitao.ToString(),"Người lập hóa đơn" },
              { ColumnOrders.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnInvoices.tienthue.ToString(),"Tổng thuế" },
              { ColumnOrders.tonggiamgia.ToString(),"Tổng giảm giá" },
              { ColumnInvoices.tongchiphi.ToString(),"Tổng chi phí" },
              { ColumnOrders.khachcantra.ToString(),"Khách cần trả" },
              { ColumnOrders.khachdatra.ToString(),"Khách đã trả" },
              { ColumnOrders.ghichu.ToString(),"Ghi chú" },
              { ColumnOrders.trangthai.ToString(),"Trạng thái" },
        };

        public enum ColumnReturns
        {
            matrahang = 0,
            mahoadon,
            ngaylaphoadon,
            makhachhang,
            tenkhachhang,
            email,
            sodienthoai,
            diachi,
            tenchinhanh,
            nguoiban,
            nguoitao,
            tongtienhang,
            tonggiamgia,
            tongsaugiamgia,
            phitrahang,
            cantrakhach,
            datrakhach,
            conno,
            ghichu,
            trangthai,
        }
        public static Dictionary<string, string> ListColumnReturns = new Dictionary<string, string>()
        {
              { ColumnReturns.matrahang.ToString(),"Mã trả hàng" },
              { ColumnReturns.mahoadon.ToString(),"Mã hóa đơn gốc" },
              { ColumnReturns.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnReturns.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnReturns.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnReturns.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnReturns.diachi.ToString(),"Địa chỉ" },
              { ColumnOrders.khuvuc.ToString(),"Tỉnh thành" },
              { ColumnReturns.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnReturns.nguoiban.ToString(),"Nhân viên bán" },
              { ColumnReturns.nguoitao.ToString(),"Người lập hóa đơn" },
              { ColumnReturns.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnReturns.tonggiamgia.ToString(),"Tổng giảm giá" },
              { ColumnReturns.tongsaugiamgia.ToString(),"Tổng sau giảm giá" },
              { ColumnReturns.phitrahang.ToString(),"Phí trả hàng" },
              { ColumnInvoices.tienthue.ToString(),"Tiền thuế" },
              { ColumnReturns.cantrakhach.ToString(),"Cần trả khách" },
              { ColumnReturns.datrakhach.ToString(),"Đã trả khách" },
              { ColumnReturns.conno.ToString(),"Còn nợ" },
              { ColumnReturns.ghichu.ToString(),"Ghi chú" },
              { ColumnReturns.trangthai.ToString(),"Trạng thái" },
        };

        public enum ColumnValueCard
        {
            mahoadon = 0,
            ngaylaphoadon,
            makhachhang,
            tenkhachhang,
            sodienthoai,
            mucnap,
            khuyenmai,
            tongtiennap,
            sodusaunap,
            chietkhau,
            khachcantra,
            tienmat,
            chuyenkhoan,
            pos,
            khachdatra,
            nvthuchien,
            ghichu,
            trangthai,
            tatToanCongNo
        }
        public static Dictionary<string, string> ListColumnValueCard = new Dictionary<string, string>()
        {
              { ColumnValueCard.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnValueCard.ngaylaphoadon.ToString(),"Ngày lập phiếu" },
              { ColumnValueCard.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnValueCard.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnValueCard.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnValueCard.tongtiennap.ToString(),"Tổng tiền nạp/ trả" },
               { ColumnInvoices.tongchiphi.ToString(),"Phí hoàn thẻ" },
              { ColumnValueCard.khachcantra.ToString(),"Phải thanh toán" },
              { ColumnValueCard.tienmat.ToString(),"Tiền mặt" },
              { ColumnValueCard.chuyenkhoan.ToString(),"Chuyển khoản" },
              { ColumnValueCard.pos.ToString(),"Tiền POS" },
              { ColumnValueCard.khachdatra.ToString(),"Đã thanh toán" },
              { ColumnValueCard.tatToanCongNo.ToString(),"Giá trị tất toán" },
              { ColumnInvoices.conno.ToString(),"Còn nợ" },
              { ColumnValueCard.nvthuchien.ToString(),"Nhân viên bán" },
              { ColumnValueCard.ghichu.ToString(),"Ghi chú" },
              { ColumnValueCard.trangthai.ToString(),"Trạng thái" },
        };


        public enum ColumnPurchaseReturns
        {
            mahoadon = 0,
            ngaylaphoadon,
            makhachhang,
            tenkhachhang,
            sodienthoai,
            diachi,
            tenchinhanh,
            nguoiban,
            tongtienhang,
            tonggiamgia,
            cantrakhach,
            datrakhach,
            conno,
            ghichu,
            trangthai,
        }
        public static Dictionary<string, string> ListColumnPurchaseReturns = new Dictionary<string, string>()
        {
              { ColumnPurchaseReturns.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnInvoices.madathang.ToString(),"Mã phiếu nhập" },
              { ColumnPurchaseReturns.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnPurchaseReturns.makhachhang.ToString(),"Mã nhà cung cấp" },
              { ColumnPurchaseReturns.tenkhachhang.ToString(),"Tên nhà cung cấp" },
              { ColumnPurchaseReturns.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnPurchaseReturns.diachi.ToString(),"Địa chỉ" },
              { ColumnPurchaseReturns.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnInvoices.nguoitao.ToString(),"User lập phiếu" },
              { ColumnPurchaseReturns.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnInvoices.tienthue.ToString(),"Tổng tiền thuế" },
              { ColumnPurchaseReturns.tonggiamgia.ToString(),"Tổng giảm giá" },
              { ColumnPurchaseReturns.cantrakhach.ToString(),"NCC cần trả" },
              { ColumnPurchaseReturns.datrakhach.ToString(),"NCC đã trả" },
              { ColumnPurchaseReturns.conno.ToString(),"Còn nợ" },
              { ColumnPurchaseReturns.ghichu.ToString(),"Ghi chú" },
              { ColumnPurchaseReturns.trangthai.ToString(),"Trạng thái" },
        };

        public enum ColumnPurchaseOrder
        {
            mahoadon = 0,
            ngaylaphoadon,
            makhachhang,
            tenkhachhang,
            sodienthoai,
            diachi,
            tenchinhanh,
            nguoiban,
            tongtienhang,
            tonggiamgia,
            cantrakhach,
            datrakhach,
            ghichu,
            trangthai,
        }
        public static Dictionary<string, string> ListColumnPurchaseOrder = new Dictionary<string, string>()
        {
              { ColumnPurchaseOrder.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnInvoices.madathang.ToString(),"Mã đặt hàng" },
              { ColumnPurchaseOrder.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnInvoices.nguoitao.ToString(),"User lập phiếu" },
              { ColumnPurchaseOrder.makhachhang.ToString(),"Mã nhà cung cấp" },
              { ColumnPurchaseOrder.tenkhachhang.ToString(),"Tên nhà cung cấp" },
              { ColumnPurchaseOrder.sodienthoai.ToString(),"Số điện thoại" },
              { ColumnPurchaseOrder.diachi.ToString(),"Địa chỉ" },
              { ColumnPurchaseOrder.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnInvoices.thanhtienchuack.ToString(),"Tổng tiền trước CK" },
              { ColumnInvoices.giamgiact.ToString(),"Tổng chiết khấu" },
              { ColumnPurchaseOrder.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnInvoices.tienthue.ToString(),"Tổng tiền thuế" },
              { ColumnPurchaseOrder.tonggiamgia.ToString(),"Tổng giảm giá" },
              { ColumnInvoices.tongchiphi.ToString(),"Chi phí ship" },
              { ColumnPurchaseOrder.cantrakhach.ToString(),"Cần trả NCC" },
              { ColumnInvoices.tienmat.ToString(),"Tiền mặt" },
              { ColumnInvoices.pos.ToString(),"Tiền POS" },
              { ColumnInvoices.chuyenkhoan.ToString(),"Chuyển khoản" },
              { ColumnInvoices.tiencoc.ToString(),"Tiền đặt cọc" },
              { ColumnPurchaseOrder.datrakhach.ToString(),"Đã trả NCC" },
              { ColumnInvoices.conno.ToString(),"Còn nợ" },
              { ColumnPurchaseOrder.ghichu.ToString(),"Ghi chú" },
              { ColumnPurchaseOrder.trangthai.ToString(),"Trạng thái" },
        };

        public static Dictionary<string, string> ListColumnNhapNoiBo = new Dictionary<string, string>()
        {
              { ColumnPurchaseOrder.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnPurchaseOrder.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnInvoices.nguoitao.ToString(),"User lập phiếu" },
               { ColumnPurchaseOrder.makhachhang.ToString(),"Mã người cung cấp" },
              { ColumnPurchaseOrder.tenkhachhang.ToString(),"Nhân viên/ Nhà cung cấp" },
              { ColumnPurchaseOrder.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnPurchaseOrder.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnPurchaseOrder.tonggiamgia.ToString(),"Tổng giảm giá" },
              { ColumnInvoices.tongchiphi.ToString(),"Chi phí ship" },
                 { ColumnPurchaseOrder.cantrakhach.ToString(),"Phải thanh toán" },
                   { ColumnPurchaseOrder.datrakhach.ToString(),"Đã thanh toán" },
                       { ColumnInvoices.conno.ToString(),"Còn nợ" },
              { ColumnPurchaseOrder.ghichu.ToString(),"Ghi chú" },
              { ColumnPurchaseOrder.trangthai.ToString(),"Trạng thái" },
        };

        public static Dictionary<string, string> ListColumnNhapHangThua = new Dictionary<string, string>()
        {
              { ColumnPurchaseOrder.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnPurchaseOrder.ngaylaphoadon.ToString(),"Ngày lập hóa đơn" },
              { ColumnInvoices.nguoitao.ToString(),"User lập phiếu" },
               { ColumnPurchaseOrder.makhachhang.ToString(),"Mã khách hàng" },
              { ColumnPurchaseOrder.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnPurchaseOrder.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnPurchaseOrder.tongtienhang.ToString(),"Tổng tiền hàng" },
              { ColumnPurchaseOrder.tonggiamgia.ToString(),"Tổng giảm giá" },
               { ColumnInvoices.tongchiphi.ToString(),"Chi phí ship" },
                 { ColumnPurchaseOrder.cantrakhach.ToString(),"Phải thanh toán" },
                   { ColumnPurchaseOrder.datrakhach.ToString(),"Đã thanh toán" },
                       { ColumnInvoices.conno.ToString(),"Còn nợ" },
              { ColumnPurchaseOrder.ghichu.ToString(),"Ghi chú" },
              { ColumnPurchaseOrder.trangthai.ToString(),"Trạng thái" },
        };

        public enum ColumnDamageItems
        {
            mahoadon = 0,
            mahoadonsuachua,
            bienso,
            loaiphieu,
            ngaylaphoadon,
            tenchinhanh,
            tennhanvien,
            tongxuat,
            ghichu,
            trangthai,
        }
        public static Dictionary<string, string> ListColumnDamageItems = new Dictionary<string, string>()
        {
              { ColumnDamageItems.mahoadon.ToString(),"Mã hóa đơn" },
              { ColumnDamageItems.loaiphieu.ToString(),"Loại phiếu" },
              { ColumnKhachHang.ngaytao.ToString(),"Ngày xác nhận" },
              { ColumnDamageItems.mahoadonsuachua.ToString(),"HĐ liên quan" },
              { ColumnDamageItems.ngaylaphoadon.ToString(),"Ngày lập HĐ" },
              { ColumnInvoices.tenkhachhang.ToString(),"Tên khách hàng" },
              { ColumnDamageItems.tenchinhanh.ToString(),"Chi nhánh" },
              { ColumnDamageItems.tennhanvien.ToString(),"Người yêu cầu" },
              { ColumnDamageItems.tongxuat.ToString(),"Tổng giá trị xuất" },
              { ColumnDamageItems.ghichu.ToString(),"Ghi chú" },
              { ColumnDamageItems.trangthai.ToString(),"Trạng thái" },
        };
    }
    public class RoleModel
    {
        public bool View { get; set; }
        public bool Insert { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Import { get; set; }
        public bool Export { get; set; }
        public bool Print { get; set; }
        public string Log { get; set; }
        public bool NhanSu { get; set; }
    }
    public class RoleKyTinhCongModel : RoleModel
    {
        public bool ChotCong { get; set; }
        public bool ThanhToan { get; set; }
        public bool MoLaiBangLuong { get; set; }

    }
    public class RoleChamCongModel : RoleModel
    {
        public bool GuiBangCong { get; set; }
        public bool ChamCong { get; set; }
    }
}