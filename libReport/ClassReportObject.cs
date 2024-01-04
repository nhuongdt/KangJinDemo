using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libReport
{
    class ClassReportObject
    {
    }
    public class Report_HangHoa_BanHangPRC
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenLoHang { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public double SoLuongBan { get; set; }
        public double GiaTriBan { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double DoanhThuThuan { get; set; }
    }

    public class Report_HangHoa_BanHang
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenHangHoaCV { get; set; }
        public string TenHangHoaGC { get; set; }
        public double SoLuongBan { get; set; }
        public double GiaTriBan { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double DoanhThuThuan { get; set; }
    }

    public class Report_SumHangHoa_BanHang
    {
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public int LoaiHoaDon { get; set; }
        public string TenHangHoaCV { get; set; }
        public string TenHangHoaGC { get; set; }
        public string MaHangHoa { get; set; }
        public double? GiaVon { get; set; }
    }

    public class BaoCaoBanHang_TongHopPRC
    {
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double? DoanhThuThuan { get; set; }
        public double? TongTienThue { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
        public double Rowns { get; set; }
        public double TongSoLuong { get; set; }
        public double TongThanhTien { get; set; }
        public double TongGiamGiaHD { get; set; }
        public double? SumTienThue { get; set; }
        public double TongTienVon { get; set; }
        public double? TongChiPhi { get; set; }
        public double TongLaiLo { get; set; } 
        public double? TongDoanhThuThuan { get; set; }
    }

    public class BaoCaoBanHang_ChiTietPRC
    {
        public double Rowns { get; set; }
        public double TongSoLuong { get; set; }
        public double TongThanhTien { get; set; }
        public double TongGiamGiaHD { get; set; }
        public double? DoanhThuThuan { get; set; }
        public double? TongTienThue { get; set; }
        public double? TongChiPhi { get; set; }
        public double TongTienVon { get; set; }
        public double TongLaiLo { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }

        public string NhomKhachHang { get; set; }
        public string TenNguonKhach { get; set; }
        public string DienThoai { get; set; }
        public string GioiTinh { get; set; }
        public string NguoiGioiThieu { get; set; }

        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoaThayThe { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string MaGoiDichVu { get; set; }// xem hd dc sử dụng từ GDV nào
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double GiaBan { get; set; }
        public double TienChietKhau { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double? DoanhThu { get; set; } // = thanhtien - giamgiahd
        public double? TienThue { get; set; }
        public double GiaVon { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
        public string ThoiGianBaoHanh { get; set; }
        public DateTime? HanBaoHanh { get; set; }
        public string TrangThai { get; set; }
        public double? SoNgay { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
    }

    public class BaoCaoBanHang_DinhDanhDichVu
    {
        public Guid? ID_HoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string MaHangHoa { get; set; }
        public string MaDinhDanh { get; set; }
        public string MaHoaDon { get; set; }
        public string TenHangHoa { get; set; }
        public string TenNhomHangHoa { get; set; }
        public double? SoLuong { get; set; }
        public string TenDonViTinh { get; set; }
        public double? DonGia { get; set; }
        public double? TienChietKhau { get; set; }
        public double? ThanhTien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }

        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
    }


    public class BaoCaoBanHang_NhomHangPRC
    {
        public string TenNhomHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double? DoanhThu { get; set; }
        public double? TienThue { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
    }

    public class BaoCaoBanHang_TheoKhachHangPRC
    {
        public Guid ID_KhachHang { get; set; }
        public string NhomKhachHang { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DienThoai { get; set; }
        public double? SoLuongMua { get; set; }
        public double? GiaTriMua { get; set; }
        public double? SoLuongTra { get; set; }
        public double? GiaTriTra { get; set; }
        public double SoLuong { get; set; }// = mua - tra
        public double DoanhThu { get; set; }// ThanhTien - giamgiaHD
        public double? TongTienThue { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string NguoiQuanLy { get; set; }
    }

    public class BaoCaoBanHang_TheoKhachHangTanSuat
    {
        public Guid ID_KhachHang { get; set; }
        public string TenNhomDoiTuongs { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DienThoai { get; set; }
        public string DiaChi { get; set; }
        public int? SoLanDen { get; set; }
        public double? GiaTriMua { get; set; }
        public double? GiaTriTra { get; set; }
        public double? DoanhThu { get; set; }
        public DateTime? NgayGiaoDichGanNhat { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public int? TongSoLanDen { get; set; }
        public double? TongMua { get; set; }
        public double? TongTra { get; set; }
        public double? TongDoanhThu { get; set; }
    }

    public class BCTanSuat_NhatKyGiaoDich
    {
        public Guid ID { get; set; } // id of hoadon
        public string TenDonVi { get; set; }
        public string SLoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public DateTime? NgayApDungGoiDV { get; set; }
        public DateTime? HanSuDungGoiDV { get; set; }
        public double? TongTienHang { get; set; } // if thegiatri = mucnap + khuyenmai
        public double? TongTienThue { get; set; }
        public double? TongGiamGia { get; set; }
        public double? TongChiPhi { get; set; } // used to thegiatri = mucnap
        public double? TongChietKhau { get; set; } // used to thegiatri = khuyenmai
        public double? PhaiThanhToan { get; set; }
        public double? TienMat { get; set; }
        public double? TienGui { get; set; }
        public double? ThuTuThe { get; set; }
        public double? DaThanhToan { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public double? SumTienHang { get; set; }
        public double? SumTienThue { get; set; }
        public double? SumGiamGia { get; set; }
        public double? SumChiPhi { get; set; }
        public double? SumChietKhau { get; set; }
        public double? SumPhaiThanhToan { get; set; }
        public double? SumTienMat { get; set; }
        public double? SumTienGui { get; set; }
        public double? SumThuTuThe { get; set; }
        public double? SumDaThanhToan { get; set; }
    }

    public class BaoCaoBanHangChiTiet_TheoKhachHangPRC
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenLoHang { get; set; }
        public double? SoLuongMua { get; set; }
        public double? GiaTriMua { get; set; }
        public double? SoLuongTra { get; set; }
        public double? GiaTriTra { get; set; }
        public double? SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double? TongTienThue { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
    }

    public class BaoCaoBanHang_TheoNhanVienPRC
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double SoLuongBan { get; set; }
        public double ThanhTien { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double GiamGiaHD { get; set; }
        public double? DoanhThu { get; set; }
        public double? TienThue { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
    }

    public class BaoCaoBanHangChiTiet_TheoNhanVienPRC
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuongBan { get; set; }
        public double ThanhTien { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double GiamGiaHD { get; set; }
        public double? DoanhThu { get; set; }
        public double? TienThue { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
    }

    public class BaoCaoBanHang_HangTraLaiPRC
    {
        public string MaChungTuGoc { get; set; }
        public string MaChungTu { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriTra { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
        public int? LoaiHoaDon { get; set; }// loaihd of hdgoc
    }

    public class BaoCaoHangKhuyenMai
    {
        public string MaKhuyenMai { get; set; }
        public string TenKhuyenMai { get; set; }
        public string sHinhThuc { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public double TongTienHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double GiaTriKM { get; set; }
        public string NguoiTao { get; set; }
        public string TenNhanVien { get; set; }
    }


    public class BaoCaoBanHang_LoiNhuanPRC
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuongBan { get; set; }
        public double ThanhTien { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double GiamGiaHD { get; set; }
        public double DoanhThuThuan { get; set; }
        public double? TienThue { get; set; }
        public double TienVon { get; set; }
        public double? ChiPhi { get; set; }
        public double LaiLo { get; set; }
        public double TySuat { get; set; }
    }

    public class BaoCaoDatHang_TongHopPRC
    {
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuongDat { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriDat { get; set; }
        public double SoLuongNhan { get; set; }
    }

    public class BaoCaoDatHang_ChiTietPRC
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string TenKhachHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuongDat { get; set; }
        public double TongTienHang { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriDat { get; set; }
        public double SoLuongNhan { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
    }

    public class BaoCaoDatHang_NhomHangPRC
    {
        public string TenNhomHangHoa { get; set; }
        public double SoLuongDat { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriDat { get; set; }
        public double SoLuongNhan { get; set; }
    }

    public class BaoCaoNhapHang_TongHopPRC
    {
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriNhap { get; set; }
        public double? TienThue { get; set; }
    }

    public class BaoCaoNhapHang_ChiTietPRC
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double DonGia { get; set; }
        public double TienChietKhau { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriNhap { get; set; }
        public double? TienThue { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
    }

    public class BaoCaoNhapHang_NhomHangPRC
    {
        public string TenNhomHangHoa { get; set; }
        public double SoLuongNhap { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriNhap { get; set; }
        public double? TienThue { get; set; }
    }

    public class BaoCaoNhapHang_TheoNhaCungCapRC
    {
        public string NhomKhachHang { get; set; }
        public string MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public string DienThoai { get; set; }
        public double SoLuongNhap { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriNhap { get; set; }
        public double? TienThue { get; set; }
    }

    public class BaoCaoNhapHang_TraHangNhapPRC
    {
        public string MaChungTuGoc { get; set; }
        public string MaChungTu { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double GiaTriTra { get; set; }
        public string TenNhanVien { get; set; }
    }

    public class BaoCaoKho_TonKhoPRC
    {
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double TonCuoiKy { get; set; }
        public double TonQuyCach { get; set; }
        public double GiaTriCuoiKy { get; set; }
    }

    public class BaoCaoKho_TonKho_TongHopPRC
    {
        public Guid ID_ChiNhanh { get; set; }
        public string TenChiNhanh { get; set; }
        public double SoLuong { get; set; }
        public double GiaTri { get; set; }
    }

    public class BaoCaoKho_NhapXuaTonPRC
    {
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double TonDauKy { get; set; }
        public double GiaTriDauKy { get; set; }
        public double SoLuongNhap { get; set; }
        public double GiaTriNhap { get; set; }
        public double SoLuongXuat { get; set; }
        public double GiaTriXuat { get; set; }
        public double TonCuoiKy { get; set; }
        public double TonQuyCach { get; set; }
        public double GiaTriCuoiKy { get; set; }
    }

    public class BaoCaoKho_NhapXuaTonChiTietPRC
    {
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double TonDauKy { get; set; }
        public double GiaTriDauKy { get; set; }
        public double SoLuongNhap_NCC { get; set; }
        public double SoLuongNhap_Kiem { get; set; }
        public double SoLuongNhap_Tra { get; set; }
        public double SoLuongNhap_Chuyen { get; set; }
        public double SoLuongNhap_SX { get; set; }
        public double SoLuongXuat_Ban { get; set; }
        public double SoLuongXuat_Huy { get; set; }
        public double SoLuongXuat_NCC { get; set; }
        public double SoLuongXuat_Kiem { get; set; }
        public double SoLuongXuat_Chuyen { get; set; }
        public double SoLuongXuat_SX { get; set; }
        public double TonCuoiKy { get; set; }
        public double GiaTriCuoiKy { get; set; }
    }

    public class BaoCaoKho_XuatChuyenHangPRC
    {
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
    }

    public class BaoCaoKho_XuatChuyenHangChiTietPRC
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public string MaChiNhanhChuyen { get; set; }
        public string ChiNhanhChuyen { get; set; }
        public string MaChiNhanhNhan { get; set; }
        public string ChiNhanhNhan { get; set; }
        public double SoLuong { get; set; }
        public double GiaVon { get; set; }
        public double GiaTri { get; set; }
        public double DonGia { get; set; }
        public double ThanhTien { get; set; }
    }
    public class BaoCaoKho_ChiTietHangNhapKhoPRC
    {
        public int? LoaiHoaDon { get; set; }
        public string TenLoaiChungTu { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string BienSo { get; set; }
        public string NguoiTao { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }  
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double SoLuong { get; set; }
        public double? GiaNhap { get; set; } // dongia sau ck
        public double ThanhTien { get; set; }
        public string GhiChu { get; set; }
        public string DienGiai { get; set; }
        public string TenNhanVien { get; set; }
    }

    public class BaoCaoKho_XuatDichVuDinhLuongPRC
    {
        public int? LoaiHoaDon { get; set; }
        public string TenLoaiChungTu { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string TenNhanVien { get; set; }
        public string NhomDichVu { get; set; }
        public Guid? ID_DichVu { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string TenDonViDichVu { get; set; }
        public double SoLuongDichVu { get; set; }
        public double? GiaBanDichVu { get; set; }
        public double? ThanhTienDichVu { get; set; }
        public double GiaTriDichVu { get; set; }
        public double? PtramSuDung { get; set; }
        public string NVThucHiens { get; set; }
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public double SoLuongDinhLuongBanDau { get; set; }
        public double GiaTriDinhLuongBanDau { get; set; }
        public string GhiChu { get; set; }
        public double SoLuongThucTe { get; set; }
        public double GiaTriThucTe { get; set; }
        public double SoLuongChenhLech { get; set; }
        public double GiaTriChenhLech { get; set; }
        public string TrangThai { get; set; }
    }

    public class BaoCaoTaiChinh_CongNoPRC
    {
        public string NhomDoiTac { get; set; }
        public string MaDoiTac { get; set; }
        public string TenDoiTac { get; set; }  
        public string MaNhanVien { get; set; }// nvPhutrach
        public string TenNhanVien { get; set; }
        public double PhaiThuDauKy { get; set; }
        public double PhaiTraDauKy { get; set; }
        public double TongTienChi { get; set; }
        public double TongTienThu { get; set; }
        public double PhaiThuCuoiKy { get; set; }
        public double PhaiTraCuoiKy { get; set; }
    }

    public class BaoCaoTaiChinh_ThuChiPRC
    {
        public string NhomDoiTuong { get; set; }
        public string MaHoaDon { get; set; }
        public string MaPhieuThu { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaNguoiNop { get; set; }
        public string TenNguoiNop { get; set; }
        public double TienMat { get; set; }
        public double TienGui { get; set; }
        public double TienPOS { get; set; }
        public double ThuChi { get; set; }
        public string SoTaiKhoan { get; set; }
        public string TenNganHang { get; set; }
        public string NoiDungThuChi { get; set; }
        public string GhiChu { get; set; }
        public string LoaiThuChi { get; set; }
        public string TenChiNhanh { get; set; }
    }

    public class BaoCaoTaiChinh_SoQuyPRC
    {
        public Guid ID_HoaDon { get; set; }
        public string MaPhieuThu { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string KhoanMuc { get; set; }
        public string TenDoiTac { get; set; }
        public double TonDauKy { get; set; }
        public double TienMat { get; set; }
        public double TienGui { get; set; }
        public double TienThu { get; set; }
        public double TienChi { get; set; }
        public double ThuTienMat { get; set; }
        public double ThuTienGui { get; set; }
        public double ThuTienPOS { get; set; }
        public double ChiTienMat { get; set; }
        public double ChiTienGui { get; set; }
        public double ChiTienPOS { get; set; }
        public double TonLuyKe { get; set; }
        public double TonLuyKeTienGui { get; set; }
        public double TonLuyKeTienMat { get; set; }
        public string SoTaiKhoan { get; set; }
        public string NganHang { get; set; }
        public string GhiChu { get; set; }
        public Guid IDDonVi { get; set; }
        public string TenDonVi { get; set; }
    }

    public class BaoCaoTaiChinh_SoQuyTheoChiNhanhPRC
    {
        public Guid ID { get; set; }
        public string TenDonVi { get; set; }
        public double TonTienMat { get; set; }
        public double TonTienGui { get; set; }
        public double TongThuChi { get; set; }
    }

    public class Report_TaiChinh_TheoThang
    {
        public string TaiChinh { get; set; }
        public double Thang1 { get; set; }
        public double Thang2 { get; set; }
        public double Thang3 { get; set; }
        public double Thang4 { get; set; }
        public double Thang5 { get; set; }
        public double Thang6 { get; set; }
        public double Thang7 { get; set; }
        public double Thang8 { get; set; }
        public double Thang9 { get; set; }
        public double Thang10 { get; set; }
        public double Thang11 { get; set; }
        public double Thang12 { get; set; }
        public double Tong { get; set; }
        public string GhiChu { get; set; } = "";
    }

    public class ReportTaiChinhMonth_DoanhThuBanHangPRC
    {
        public int ThangLapHoaDon { get; set; }
        public double DoanhThu { get; set; }
        public double GiaVonGDV { get; set; }
        public double GiaTriTra { get; set; }
        public double GiamGiaHD { get; set; }
    }

    public class ReportTaiChinh_ChiPhiSuaChuaPRC
    {
        public int Thang { get; set; }
        public double ChiPhi { get; set; }
    }

    public class ReportTaiChinhMonth_GiaVonBanHangPRC
    {
        public int ThangLapHoaDon { get; set; }
        public double TongGiaVonBan { get; set; }
        public double TongGiaVonTra { get; set; }
    }

    public class ReportTaiChinhMonth_ChiPhiBanHangPRC
    {
        public int ThangLapHoaDon { get; set; }
        public double GiaTriHuy { get; set; }
        public double DiemThanhToan { get; set; }
    }

    public class ReportTaiChinhMonth_SoQuyBanHangPRC
    {
        public int ThangLapHoaDon { get; set; }
        public double ThuNhapKhac { get; set; }
        public double ChiPhiKhac { get; set; }
        public double PhiTraHangNhap { get; set; }
        public double KhachThanhToan { get; set; }
    }

    public class Report_TaiChinh_TheoQuy
    {
        public string TaiChinh { get; set; }
        public double Quy1 { get; set; }
        public double Quy2 { get; set; }
        public double Quy3 { get; set; }
        public double Quy4 { get; set; }
        public double Tong { get; set; }
    }

    public class Report_TaiChinh_TheoNam
    {
        public string TaiChinh { get; set; }
        public double Nam { get; set; }
        public double Tong { get; set; }
    }

    public class ReportTaiChinhYear_DoanhThuBanHangPRC
    {
        public int NamLapHoaDon { get; set; }
        public double DoanhThu { get; set; }
        public double GiaVonGDV { get; set; }
        public double GiaTriTra { get; set; }
        public double GiamGiaHD { get; set; }
    }

    public class ReportTaiChinhYear_ChiPhiBanHangPRC
    {
        public int NamLapHoaDon { get; set; }
        public double GiaTriHuy { get; set; }
        public double DiemThanhToan { get; set; }
    }

    public class ReportTaiChinhYear_SoQuyBanHangPRC
    {
        public int NamLapHoaDon { get; set; }
        public double ThuNhapKhac { get; set; }
        public double ChiPhiKhac { get; set; }
        public double PhiTraHangNhap { get; set; }
        public double KhachThanhToan { get; set; }
    }

    public class ReportTaiChinhYear_GiaVonBanHangPRC
    {
        public int NamLapHoaDon { get; set; }
        public double TongGiaVonBan { get; set; }
        public double TongGiaVonTra { get; set; }
    }

    public class BaoCaoTaiChinh_PTTCTheoThangPRC
    {
        public Guid? ID_KhoanThuChi { get; set; }
        public string KhoanMuc { get; set; }
        public double? Thang1 { get; set; }
        public double? Thang2 { get; set; }
        public double? Thang3 { get; set; }
        public double? Thang4 { get; set; }
        public double? Thang5 { get; set; }
        public double? Thang6 { get; set; }
        public double? Thang7 { get; set; }
        public double? Thang8 { get; set; }
        public double? Thang9 { get; set; }
        public double? Thang10 { get; set; }
        public double? Thang11 { get; set; }
        public double? Thang12 { get; set; }
        public double? TongCong { get; set; }
    }

    public class BaoCaoTaiChinh_PTTCTheoQuyPRC
    {
        public Guid? ID_KhoanThuChi { get; set; }
        public string KhoanMuc { get; set; }
        public double? Quy1 { get; set; }
        public double? Quy2 { get; set; }
        public double? Quy3 { get; set; }
        public double? Quy4 { get; set; }
        public double? TongCong { get; set; }
    }

    public class BaoCaoTaiChinh_PTTCTheoNamPRC
    {
        public Guid? ID_KhoanThuChi { get; set; }
        public string KhoanMuc { get; set; }
        public double? TongCong { get; set; }
    }

    public class SP_ReportDiscountAll
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double? HoaHongThucHien { get; set; }
        public double? HoaHongThucHien_TheoYC { get; set; }
        public double? HoaHongTuVan { get; set; }
        public double? HoaHongBanGoiDV { get; set; }
        public double? TongHangHoa { get; set; }
        public double? HoaHongDoanhThuHD { get; set; }
        public double? HoaHongThucThuHD { get; set; }
        public double? HoaHongVND { get; set; }
        public double? TongHoaDon { get; set; }
        public double? DoanhThu { get; set; }
        public double? ThucThu { get; set; }
        public double? HoaHongDoanhThuDS { get; set; }
        public double? HoaHongThucThuDS { get; set; }
        public double? TongDoanhSo { get; set; }
        public double? Tong { get; set; }

        public double TongHoaHongThucHien { get; set; }
        public double TongHoaHongThucHien_TheoYC { get; set; }
        public double TongHoaHongTuVan { get; set; }
        public double TongHoaHongBanGoiDV { get; set; }
        public double TongHoaHong_TheoHangHoa { get; set; }

        public double TongHoaHongDoanhThu { get; set; }
        public double TongHoaHongThucThu { get; set; }
        public double TongHoaHongVND { get; set; }
        public double TongHoaHong_TheoHoaDon { get; set; }
        public double TongDoanhThu { get; set; }
        public double TongThucThu { get; set; }

        public double TongHoaHongDoanhThuDS { get; set; }
        public double TongHoaHongThucThuDS { get; set; }
        public double TongHoaHong_TheoDoanhSo { get; set; }
        public double TongHoaHongAll { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class ParamReportDiscount
    {
        public string TextSearch { get; set; }
        public string HangHoaSearch { get; set; }
        public string TxtCustomer{ get; set; }
        public string ID_ChiNhanhs { get; set; }
        public string ID_NhomHang { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int TrangThai { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string TextReport { get; set; }
        public string TodayBC { get; set; }
        public string ColumnsHide { get; set; }
        public int? TypeReport { get; set; }
        public int? Status_ColumnHide { get; set; }
        public int? StatusInvoice { get; set; }
        public List<string> LstIDChiNhanh { get; set; }
        public bool IsExport { get; set; }
        public Guid? ID_NhanVienLogin { get; set; }
        public List<string> LaHangHoas { get; set; } // 1.lahanghoa, 2.ladichvu, 3.combo
        public List<string> LoaiChungTus { get; set; } //1.hdban, 6.tra, 19.gdv, 22.thegiatri, 25.hdsc
        public List<string> DepartmentIDs { get; set; }
    }

    public class BaoCaoChietKhau_TongHopPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double HoaHongThucHien { get; set; }
        public double HoaHongThucHien_TheoYC { get; set; }
        public double HoaHongTuVan { get; set; }
        public double HoaHongBanGoiDV { get; set; }
        public double Tong { get; set; }

        public double TongHoaHongThucHien { get; set; }
        public double TongHoaHongThucHien_TheoYC { get; set; }
        public double TongHoaHongTuVan { get; set; }
        public double TongHoaHongBanGoiDV { get; set; }
        public double TongAll { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class BaoCaoChietKhau_ChiTietPRC
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DienThoaiKH { get; set; }
        public string TenNVPhuTrach { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenLoHang { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double? SoLuong { get; set; }
        public double? ThanhTien { get; set; }
        public double? HeSo { get; set; }
        public double? GtriSauHeSo { get; set; }
        public double? PTThucHien { get; set; }
        public double HoaHongThucHien { get; set; }
        public double? PTThucHien_TheoYC { get; set; }
        public double HoaHongThucHien_TheoYC { get; set; }
        public double? PTTuVan { get; set; }
        public double HoaHongTuVan { get; set; }
        public double? PTBanGoi { get; set; }
        public double HoaHongBanGoiDV { get; set; }
        public double? TongAll { get; set; }

        public double TongSoLuong { get; set; }
        public double TongHoaHongThucHien { get; set; }
        public double TongHoaHongThucHien_TheoYC { get; set; }
        public double TongHoaHongTuVan { get; set; }
        public double TongHoaHongBanGoiDV { get; set; }
        public double TongAllAll { get; set; }
        public double TongThanhTien { get; set; }// tong gtri tinhck
        public double? TongThanhTienSauHS { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public Guid? ID_ChiTietHoaDon { get; set; }
    }

    public class SP_ReportDiscountInvoice_General
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double? HoaHongDoanhThu { get; set; }
        public double? HoaHongThucThu { get; set; }
        public double? HoaHongVND { get; set; }
        public double? TongAll { get; set; }

        public double TongHoaHongDoanhThu { get; set; }
        public double TongHoaHongThucThu { get; set; }
        public double TongHoaHongVND { get; set; }
        public double TongAllAll { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    } 
    public class HoaDon_ChuaPhanBoHoaHong
    {
        public Guid? ID { get; set; }// idhoadon
        public Guid? IDSoQuy { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public double? DoanhThu { get; set; }
        public string MaPhieuThu { get; set; }
        public DateTime? NgayLapPhieuThu { get; set; }
        public double? ThucThu { get; set; }
        public double? TongDoanhThu { get; set; }
        public double? TongThucThu { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class SP_ReportDiscountInvoice_Detail
    {
        public Guid? ID { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public DateTime? NgayLapPhieuThu { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DienThoaiKH { get; set; }
        public string MaNVPhuTrach { get; set; }
        public string TenNVPhuTrach { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double? DoanhThu { get; set; }
        public double? ThucThu { get; set; }
        public double? PTDoanhThu { get; set; }
        public double? HoaHongDoanhThu { get; set; }
        public double? ChiPhiNganHang { get; set; }
        public double? TongChiPhiNganHang { get; set; }
        public double? ThucThu_ThucTinh { get; set; }
        public double? HeSo { get; set; }
        public double? PTThucThu { get; set; }
        public double? HoaHongThucThu { get; set; }
        public double? HoaHongVND { get; set; }
        public double? TongAll { get; set; }

        public double TongDoanhThu { get; set; }
        public double TongThucThu { get; set; }
        public double TongHoaHongDoanhThu { get; set; }
        public double TongHoaHongThucThu { get; set; }
        public double TongHoaHongVND { get; set; }
        public double TongAllAll { get; set; }
        public double? SumAllChiPhiNganHang { get; set; }
        public double? SumThucThu_ThucTinh { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class SP_ReportDiscountSales
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double? DoanhThu { get; set; }
        public double? ThucThu { get; set; }
        public double? HoaHongDoanhThu { get; set; }
        public double? HoaHongThucThu { get; set; }
        public double? TongAll { get; set; }

        public double TongDoanhThu { get; set; }
        public double TongThucThu { get; set; }
        public double TongHoaHongDoanhThu { get; set; }
        public double TongHoaHongThucThu { get; set; }
        public double TongAllAll { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class SP_ReportDiscountSales_Detail
    {
        public string LoaiNVApDung { get; set; }
        public string HinhThuc { get; set; }
        public DateTime ApDungTuNgay { get; set; }
        public DateTime? ApDungDenNgay { get; set; }
        public double DoanhThu { get; set; }
        public double ThucThu { get; set; }
        public double GiaTriChietKhau { get; set; }
        public double HoaHong { get; set; }
        public int LaPhanTram { get; set; }
        public double TongDoanhThu { get; set; }
        public double TongThucThu { get; set; }
        public double TongGiaTriChietKhau { get; set; }
        public double TongAll { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class ListInvoice_DiscountSales
    {
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double DoanhThu { get; set; }
        public string MaPhieuThu { get; set; }
        public double? ThucThu { get; set; }
    }

    public class BaoCaoGoiDichVu_SoDuTongHopPRC
    {

        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string NhomKhachHang { get; set; }
        public string TenNguonKhach { get; set; }
        public string DienThoai { get; set; }
        public string GioiTinh { get; set; }
        public string NguoiGioiThieu { get; set; }
        public double SoLuong { get; set; }
        public double ThanhTien { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double SoLuongSuDung { get; set; }
        public double GiaVon { get; set; }
        public double SoLuongConLai { get; set; }
        public DateTime? NgayApDungGoiDV { get; set; }
        public DateTime? HanSuDungGoiDV { get; set; }
        public double SoNgayConHan { get; set; }
        public double SoNgayHetHan { get; set; }
    }

    public class BaoCaoGoiDichVu_SoDuChiTietPRC
    {

        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string NhomKhachHang { get; set; }
        public string TenNguonKhach { get; set; }
        public string DienThoai { get; set; }
        public string GioiTinh { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double DonGia { get; set; }
        public double? ThanhTienChuaCK { get; set; }
        public double TienChietKhau { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGiaHD { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double SoLuongSuDung { get; set; }
        public double GiaVon { get; set; }
        public double SoLuongConLai { get; set; }
        public string NhanVienChietKhau { get; set; }
    }

    public class BaoCaoGoiDichVu_NhatKySuDungTongHopPRC
    {
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string NhomKhachHang { get; set; }
        public string TenNguonKhach { get; set; }
        public string DienThoai { get; set; }
        public string GioiTinh { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuongMua { get; set; }
        public double SoLuongTra { get; set; }
        public double SoLuongSuDung { get; set; }
        public double SoLuongConLai { get; set; }
    }

    public class BaoCaoGoiDichVu_NhatKySuDungChiTietPRC
    {
        public string BienSo { get; set; }
        public string MaChuXe { get; set; }
        public string TenChuXe { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string NhomKhachHang { get; set; }
        public string TenNguonKhach { get; set; }
        public string DienThoai { get; set; }
        public string GioiTinh { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string MaHoaDon { get; set; }
        public string MaGoiDV { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public string TenNhomHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuong { get; set; }
        public double? GiaTriSD { get; set; }
        public double? TienVon { get; set; }
        public string NhanVienChietKhau { get; set; }
        public string GhiChu { get; set; }
    }

    public class BaoCaoGoiDichVu_TonChuaSuDungPRC
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuongBan { get; set; }
        public double GiaTriBan { get; set; }
        public double SoLuongTra { get; set; }
        public double GiaTriTra { get; set; }
        public double SoLuongSuDung { get; set; }
        public double GiaTriSuDung { get; set; }
        public double SoLuongConLai { get; set; }
        public double GiaTriConLai { get; set; }
    }

    public class BaoCaoGoiDichVu_NhapXuatTonPRC
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenLoHang { get; set; }
        public double SoLuongConLaiDK { get; set; }
        public double GiaTriConLaiDK { get; set; }
        public double SoLuongBanGK { get; set; }
        public double GiaTriBanGK { get; set; }
        public double SoLuongTraGK { get; set; }
        public double GiaTriTraGK { get; set; }
        public double SoLuongSuDungGK { get; set; }
        public double GiaTriSuDungGK { get; set; }
        public double SoLuongConLaiCK { get; set; }
        public double GiaTriConLaiCK { get; set; }
    }
    public class BaoCaoGoiDichVu_BanDoiTra
    {
        public Guid GDVMua_ID { get; set; }
        public Guid? GDVDoi_ID { get; set; }
        public int? GDVMua_LoaiHoaDon { get; set; }// check link to page
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string GDVMua_MaHoaDon { get; set; }
        public DateTime? GDVMua_NgayLapHoaDon { get; set; }

        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public double? SoLuongMua { get; set; }
        public double? GiaTriMua { get; set; }

        public string GDVDoi_MaHoaDon { get; set; }
        public string GDVDoi_MaHangHoa { get; set; }
        public string GDVDoi_TenHangHoa { get; set; }
        public string GDVDoi_TenDonViTinh { get; set; }
        public double? SoLuongDoi { get; set; }
        public double? GiaTriDoi { get; set; }
        public double? GiaTriChenhLech { get; set; }
        public int? TotalRow { get; set; }

    }

    public class Param_ReportGoiDichVu : CommonParamSearch
    {
        public List<string> LoaiHangHoas { get; set; }
        public int? TinhTrang { get; set; } = 0;
        public int? ThoiHanSuDung { get; set; } = 0;
        public Guid? ID_NhomHang { get; set; }
    }

    public class Param_BCGDVDoiTra : Param_ReportGoiDichVu
    {
        public string TxtDVMua { get; set; }
        public string TxtDVDoi { get; set; }
    }

    public class ParamReportValueCard
    {
        public string TextSearch { get; set; }
        public string ID_ChiNhanhs { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Status { get; set; }
        public string TextReport { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string ColumnsHide { get; set; }
        public List<string> LstIDChiNhanh { get; set; }
    }

    public class SP_ReportValueCard_Balance
    {
        public Guid ID { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public string TrangThai_TheGiaTri { get; set; }
        public double SoDuDauKy { get; set; }
        public double PhatSinhTang { get; set; }
        public double PhatSinhGiam { get; set; }
        public double SoDuCuoiKy { get; set; }

        public double TongSoDuDauKy { get; set; }
        public double TongPhatSinhTang { get; set; }
        public double TongPhatSinhGiam { get; set; }
        public double TongSoDuCuoiKy { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class SP_ReportValueCard_HisUsed
    {
        public Guid ID { get; set; }// id QuyHD
        public Guid? ID_DoiTuong { get; set; }// id QuyHD
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string SLoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public string MaHoaDonSQ { get; set; }
        public int LoaiHoaDonSQ { get; set; }// 11. su dung the, 12. Hoan tra giatri the
        public double TienThe { get; set; }
        public double SoDuTruoc { get; set; }
        public double? PhatSinhGiam { get; set; }
        public double? PhatSinhTang { get; set; }
        public double SoDuSau { get; set; }
        public string DienGiai { get; set; }

        public double TongSoDuDauKy { get; set; }
        public double TongPhatSinhTang { get; set; }
        public double TongPhatSinhGiam { get; set; }
        public double TongSoDuCuoiKy { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class SP_ValueCard_ServiceUsed
    {
        public Guid ID_HoaDon { get; set; }
        public Guid ID_PhieuThuChi { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string SLoaiHoaDon { get; set; }// BanHang, DatHang, GoiDV, TraHang
        public string MaHoaDon { get; set; }
        public string MaPhieuThu { get; set; }
        public double PhatSinhGiam { get; set; }
        public double PhatSinhTang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double DonGia { get; set; }
        public double TienChietKhau { get; set; }
        public double ThanhTien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
    }

    public class BaoCaoNhanVien_TongHopPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string DanTocTonGiao { get; set; }
        public string SoCMND { get; set; }
        public string DienThoaiDiDong { get; set; }
        public string DiaChiTT { get; set; }
        public DateTime? NgayVaoLamViec { get; set; }
        public string TenPhongBan { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgayVaoDoan { get; set; }
        public string NoiVaoDoan { get; set; }
        public DateTime? NgayNhapNgu { get; set; }
        public DateTime? NgayXuatNgu { get; set; }
        public DateTime? NgayVaoDang { get; set; }
        public DateTime? NgayVaoDangChinhThuc { get; set; }
        public DateTime? NgayRoiDang { get; set; }
        public string NoiSinhHoatDang { get; set; }
        public string LyDoRoiDang { get; set; }
    }

    public class BaoCaoNhanVien_HopDongPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public string LoaiHopDong { get; set; }
        public string SoHopDong { get; set; }
        public DateTime NgayKy { get; set; }
        public string ThoiHan { get; set; }
        public string TrangThaiHopDong { get; set; }
    }

    public class BaoCaoNhanVien_BaoHiemPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public string LoaiBaoHiem { get; set; }
        public string SoBaoHiem { get; set; }
        public DateTime NgayCap { get; set; }
        public DateTime NgayHetHan { get; set; }
        public string NoiBaoHiem { get; set; }
        public string TrangThaiBaoHiem { get; set; }
    }

    public class BaoCaoNhanVien_TuoiPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public string Tuoi { get; set; }
        public string GhiChu { get; set; }
        public string TrangThaiNV { get; set; }
    }

    public class BaoCaoNhanVien_KhenThuongPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public string HinhThuc { get; set; }
        public string SoQuyetDinh { get; set; }
        public string NoiDung { get; set; }
        public DateTime? NgayBanHang { get; set; }
    }

    public class BaoCaoNhanVien_LuongPhuCapPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public string TenLoaiLuong { get; set; }
        public DateTime? NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public double SoTien { get; set; }
        public double HeSo { get; set; }
        public string Bac { get; set; }
        public string NoiDung { get; set; }
        public string TrangThaiLuong { get; set; }
    }

    public class BaoCaoNhanVien_MienGiamThuePRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public string KhoanMienGiam { get; set; }
        public DateTime? NgayApDung { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public double SoTien { get; set; }
        public string GhiChu { get; set; }
    }

    public class BaoCaoNhanVien_DaoTaoPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string NoiHoc { get; set; }
        public string NganhHoc { get; set; }
        public string HeDaoTao { get; set; }
        public string BangCap { get; set; }
    }

    public class BaoCaoNhanVien_QuaTrinhCongTacPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string CoQuan { get; set; }
        public string ViTri { get; set; }
        public string DiaChi { get; set; }
    }

    public class BaoCaoNhanVien_GiaDinhPRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public string HoTen { get; set; }
        public string NgaySinhGD { get; set; }
        public string QuanHe { get; set; }
        public string DiaChi { get; set; }
    }

    public class BaoCaoNhanVien_SucKhoePRC
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string TenPhongBan { get; set; }
        public DateTime? NgayKham { get; set; }
        public double ChieuCao { get; set; }
        public double CanNang { get; set; }
        public string TinhHinhSucKhoe { get; set; }
    }

    public class Report_NhomDoiTuong_ByName
    {
        public Report_NhomDoiTuong_ByName(Guid _id, string _tennhomdoituong, string _tennhomdoituongkd, string _tennhomdoituongktd)
        {
            ID = _id;
            TenNhomDoiTuong = _tennhomdoituong;
            TenNhomDoiTuong_KhongDau = _tennhomdoituongkd;
            TenNhomDoiTuong_KyTuDau = _tennhomdoituongktd;
        }

        public Report_NhomDoiTuong_ByName() { }
        public Guid? ID { get; set; }
        public string TenNhomDoiTuong { get; set; }
        public string TenNhomDoiTuong_KhongDau { get; set; }
        public string TenNhomDoiTuong_KyTuDau { get; set; }
    }

    public class Report_DM_LoaiChungTuPRC
    {
        public int ID { get; set; }
        public string TenChungTu { get; set; }
    }

    public class Report_NhomHangHoa_byName
    {
        public Guid ID { get; set; }
        public string TenNhomHang { get; set; }
        public Guid? ID_Parent { get; set; }
    }

    public class Report_NhomHangHoa
    {
        public Guid? ID_NhomHangHoa { get; set; }
    }

    public class Report_NhomDoiTuongPRC
    {
        public Guid? ID { get; set; }
        public string TenNhomDoiTuong { get; set; }
        public string TenNhomDoiTuong_KhongDau { get; set; }
        public string TenNhomDoiTuong_KyTuDau { get; set; }
    }

    public class ListYear
    {
        public int Year { get; set; }
    }

    public class ParamSearchReportKhachHang
    {
        public List<string> LstIDChiNhanh { get; set; }
        public List<string> LstIDNhomKhach { get; set; }
        public string LoaiChungTus { get; set; }
        public string TrangThaiKhach { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string NgayGiaoDichFrom { get; set; }// ngaygiaodich gannhat
        public string NgayGiaoDichTo{ get; set; } 
        public string NgayTaoKHFrom { get; set; }// ngaytao khachhang
        public string NgayTaoKHTo { get; set; }
        public double? DoanhThuTu { get; set; }
        public double? DoanhThuDen { get; set; }
        public int? SoLanDenFrom { get; set; }
        public int? SoLanDenTo { get; set; }
        public string TextSearch { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string Export_Time { get; set; }
        public string Export_ChiNhanh { get; set; }
        public string ColumnSort { get; set; }
        public string TypeSort { get; set; }
        public string columnsHide { get; set; }
    }

    #region Report Gara
    public class BaoCaoDoanhThuSuaChuaTongHop
    {
        public string MaPhieuTiepNhan { get; set; }
        public DateTime NgayVaoXuong { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string CoVanDichVu { get; set; }
        public Guid? IDHoaDon { get; set; }
        public string MaHoaDon { get; set; }
	    public DateTime? NgayLapHoaDon { get; set; }
        public double TongTienHang { get; set; }
        public double TongChietKhau { get; set; }
        public double TongTienThue { get; set; }
        public double TongChiPhi { get; set; }
        public double TongGiamGia { get; set; } 
	    public double DoanhThu { get; set; }
        public double TienVon { get; set; }
        public double LoiNhuan { get; set; }
        public string GhiChu { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double STongTienHang { get; set; }
        public double SChietKhau { get; set; }
	    public double SThue { get; set; }
        public double SChiPhi { get; set; }
        public double SGiamGia { get; set; }
        public double SDoanhThu { get; set; }
        public double STongTienVon { get; set; }
	    public double SLoiNhuan { get; set; }
    }

    public class BaoCaoDoanhThuSuaChuaTongHop_Export
    {
        public string MaPhieuTiepNhan { get; set; }
        public DateTime NgayVaoXuong { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string CoVanDichVu { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public double TongTienHang { get; set; }
        public double TongChietKhau { get; set; }
        public double TongGiamGia { get; set; }
        public double DoanhThu { get; set; }
        public double TongTienThue { get; set; }
        public double TienVon { get; set; }
        public double TongChiPhi { get; set; }
        public double LoiNhuan { get; set; }
        public string GhiChu { get; set; }
        public string TenDonVi { get; set; }
    }

    public class BaoCaoDoanhThuSuaChuaChiTiet
    {
        public string MaPhieuTiepNhan { get; set; }
        public DateTime NgayVaoXuong { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string CoVanDichVu { get; set; }
        public Guid? IDHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public double SoLuong { get; set; }
        public double DonGia { get; set; }
        public double TienChietKhau { get; set; }
        public double TienThue { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGia { get; set; }
        public double DoanhThu { get; set; }
        public double TienVon { get; set; }
        public double LoiNhuan { get; set; }
        public string GhiChu { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double ChiPhi { get; set; }
        public double SThanhTien { get; set; }
        public double SChietKhau { get; set; }
        public double SThue { get; set; }
        public double SGiamGia { get; set; }
        public double SDoanhThu { get; set; }
        public double STongTienVon { get; set; }
        public double SLoiNhuan { get; set; }
        public double SChiPhi { get; set; }

    }

    public class BaoCaoDoanhThuSuaChuaChiTiet_Export
    {
        public string MaPhieuTiepNhan { get; set; }
        public DateTime NgayVaoXuong { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string CoVanDichVu { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public double SoLuong { get; set; }
        public double DonGia { get; set; }
        public double TienChietKhau { get; set; }
        public double ThanhTien { get; set; }
        public double GiamGia { get; set; }
        public double DoanhThu { get; set; }
        public double TienThue { get; set; }
        public double TienVon { get; set; }
        public double ChiPhi { get; set; }
        public double LoiNhuan { get; set; }
        public string GhiChu { get; set; }
        public string TenDonVi { get; set; }
    }

    public class BaoCaoDoanhThuSuaChuaTheoXe
    {
        public Guid IDXe { get; set; }
        public string BienSo { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public double SoLanTiepNhan { get; set; }
        public double SoLuongHoaDon { get; set; }
        public double TongDoanhThu { get; set; }
        public double TongTienVon { get; set; }
        public double LoiNhuan { get; set; }
        public DateTime NgayGiaoDichGanNhat { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double ChiPhi { get; set; }
        public double SSoLanTiepNhan { get; set; }
        public double SSoLuongHoaDon { get; set; }
        public double STongDoanhThu { get; set; }
        public double STienVon { get; set; }
        public double SLoiNhuan { get; set; }
        public double SChiPhi { get; set; }
    }

    public class BaoCaoDoanhThuSuaChuaTheoXe_Export
    {
        public string BienSo { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public double SoLanTiepNhan { get; set; }
        public double SoLuongHoaDon { get; set; }
        public double TongDoanhThu { get; set; }
        public double TongTienVon { get; set; }
        public double ChiPhi { get; set; }
        public double LoiNhuan { get; set; }
        public DateTime NgayGiaoDichGanNhat { get; set; }
        public string TenDonVi { get; set; }
    }

    public class BaoCaoDoanhThuSuaChuaTheoCoVan
    {
        public Guid IDCoVan { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double SoLanTiepNhan { get; set; }
        public double SoLuongHoaDon { get; set; }
        public double TongDoanhThu { get; set; }
        public double TongTienVon { get; set; }
        public double LoiNhuan { get; set; }
        public DateTime NgayGiaoDichGanNhat { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double ChiPhi { get; set; }
        public double SSoLanTiepNhan { get; set; }
        public double SSoLuongHoaDon { get; set; }
        public double STongDoanhThu { get; set; }
        public double STienVon { get; set; }
        public double SLoiNhuan { get; set; }
        public double SChiPhi { get; set; }
    }

    public class BaoCaoDoanhThuSuaChuaTheoCoVan_Export
    {
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public double SoLanTiepNhan { get; set; }
        public double SoLuongHoaDon { get; set; }
        public double TongDoanhThu { get; set; }
        public double TongTienVon { get; set; }
        public double ChiPhi { get; set; }
        public double LoiNhuan { get; set; }
        public DateTime NgayGiaoDichGanNhat { get; set; }
        public string TenDonVi { get; set; }
    }
    #endregion
}
