using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libGara
{
    public class HangXe
    {
        public Guid ID { get; set; }
        public string MaHangXe { get; set; }
        public string TenHangXe { get; set; }
        public string Logo { get; set; }
        public int TrangThai { get; set; }
    }

    public class LoaiXe
    {
        public Guid ID { get; set; }
        public string MaLoaiXe { get; set; }
        public string TenLoaiXe { get; set; }
        public int TrangThai { get; set; }
    }

    public class MauXe
    {
        public Guid ID { get; set; }
        public string TenMauXe { get; set; }
        public Guid? ID_HangXe { get; set; }
        public Guid? ID_LoaiXe { get; set; }
        public string TenLoaiXe { get; set; }
        public string TenHangXe { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; } = 1;
    }

    public class Xe
    {
        public Guid ID { get; set; }
        public Guid? ID_MauXe { get; set; }
        public Guid? ID_HangXe { get; set; }
        public Guid? ID_LoaiXe { get; set; }
        public Guid? ID_KhachHang { get; set; }
        public string BienSo { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public string DungTich { get; set; }
        public string HopSo { get; set; }
        public string MauSon { get; set; }
        public int? NamSanXuat { get; set; }
        public string TenMauXe { get; set; }
        public string TenHangXe { get; set; }
        public string TenLoaiXe { get; set; }
        public string TenDoiTuong { get; set; }
        public string MaDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public Guid? ID_HangHoa { get; set; }
    }

    public class ParamSearch
    {
        public string ID_HangXe { get; set; } = "%%";
        public string ID_LoaiXe { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string TextSearch { get; set; }
        public string TrangThai { get; set; }
        public List<string> LstIDChiNhanh { get; set; }
        public string NamSanXuat { get; set; } //Đời xe
        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public string IDXe { get; set; } = "";
    }

    public class ParamGetListPhieuTiepNhan_v2
    {
        public List<string> IdChiNhanhs { get; set; } = new List<string>();
        public DateTime? NgayTiepNhan_From { get; set; } = null;
        public DateTime? NgayTiepNhan_To { get; set; } = null;
        public DateTime? NgayXuatXuongDuKien_From { get; set; } = null;
        public DateTime? NgayXuatXuongDuKien_To { get; set; } = null;
        public DateTime? NgayXuatXuong_From { get; set; } = null;
        public DateTime? NgayXuatXuong_To { get; set; } = null;
        public List<int> TrangThais { get; set; } = new List<int>();
        public string TextSearch { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int BaoHiem { get; set; } = 3;
    }
    public class GetListPhieuTiepNhan_v2
    {
        public Guid ID { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public DateTime NgayVaoXuong { get; set; }
        public int SoKmVao { get; set; }
        public DateTime? NgayXuatXuongDuKien { get; set; }
        public DateTime? NgayXuatXuong { get; set; }
        public int SoKmRa { get; set; }
        public string TenLienHe { get; set; }
        public string SoDienThoaiLienHe { get; set; }
        public string GhiChu { get; set; }
        public int TrangThaiPhieuTiepNhan { get; set; }
        public Guid ID_Xe { get; set; }
        public string BienSo { get; set; }
        public string SoMay { get; set; }
        public string SoKhung { get; set; }
        public int? NamSanXuat { get; set; }
        public string TenMauXe { get; set; }
        public string TenHangXe { get; set; }
        public string TenLoaiXe { get; set; }
        public Guid ID_KhachHang { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public Guid? ID_BaoHiem { get; set; }
        public string MaBaoHiem { get; set; }
        public string TenBaoHiem { get; set; }
        public string NguoiLienHeBH { get; set; }
        public string SoDienThoaiLienHeBH { get; set; }
        public string Email { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public string DiaChi { get; set; }
        public string MauSon { get; set; }
        public string DungTich { get; set; }
        public string HopSo { get; set; }
        public bool? LaChuXe { get; set; }
        public Guid? ID_ChuXe { get; set; }
        public string ChuXe { get; set; }
        public string ChuXe_SDT { get; set; }
        public string ChuXe_DiaChi { get; set; }
        public string ChuXe_Email { get; set; }
        public string CoVan_SDT { get; set; }
        public int TrangThai { get; set; }
        public Guid? ID_CoVanDichVu { get; set; }
        public string CoVanDichVu { get; set; }
        public string MaCoVan { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public string MaNhanVienTiepNhan { get; set; }
        public string NhanVienTiepNhan { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public DateTime NgayTao { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
        public Guid? ID_DonVi { get; set; }
    }

    public class GetListPhieuTiepNhan_v2_Export
    {
        public string MaPhieuTiepNhan { get; set; }
        public DateTime NgayVaoXuong { get; set; }
        public string BienSo { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public string TenLienHe { get; set; }
        public string SoDienThoaiLienHe { get; set; }
        public string TenMauXe { get; set; }
        public int SoKmVao { get; set; }
        public DateTime? NgayXuatXuongDuKien { get; set; }
        public DateTime? NgayXuatXuong { get; set; }
        public int SoKmRa { get; set; }
        public string CoVanDichVu { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public string NhanVienTiepNhan { get; set; }
        public string TrangThaiPhieuTiepNhan { get; set; }
        public string TenDonVi { get; set; }
        public string TenBaoHiem { get; set; }
    }

    public class ParamGetListPhieuNhapXuatKhoByIDPhieuTiepNhan
    {
        public Guid IDPhieuTiepNhan { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetListPhieuNhapXuatKhoByIDPhieuTiepNhan
    {
        public Guid ID { get; set; }
        public int LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public Guid? ID_HoaDonSuaChua { get; set; }
        public string HoaDonSuaChua { get; set; }
        public bool? TrangThaiHoaDonSuaChua { get; set; }
        public double SoLuong { get; set; }
        public double GiaTri { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class GetListPhieuNhapXuatKhoByIDPhieuTiepNhan_Export
    {
        public string MaPhieu { get; set; }
        public DateTime NgayLap { get; set; }
        public string LoaiPhieu { get; set; }
        public string MaHoaDon { get; set; }
        public double SoLuong { get; set; }
        public double GiaTri { get; set; }
    }

    public class ParamGetListGaraDanhMucXe_v1
    {
        public string IdHangXe { get; set; } = "";
        public string IdLoaiXe { get; set; } = "";
        public string IdMauXe { get; set; } = "";
        public List<int> TrangThais { get; set; } = new List<int>();
        public string TextSearch { get; set; } = "";
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class GetListGaraDanhMucXe_v1
    {
        public Guid ID { get; set; }
        public string BienSo { get; set; }
        public string DungTich { get; set; }
        public string GhiChu { get; set; }
        public string HopSo { get; set; }
        public string MauSon { get; set; }
        public Guid? ID_KhachHang { get; set; }
        public Guid ID_MauXe { get; set; }
        public Guid ID_HangXe { get; set; }
        public Guid ID_LoaiXe { get; set; }
        public int NamSanXuat { get; set; }
        public DateTime NgayTao { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public int TrangThai { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string MaSoThue { get; set; }
        public string Email { get; set; }
        public DateTime? NgaySinh_NgayTLap { get; set; }
        public string DiaChi { get; set; }
        public string MaTinhThanh { get; set; }
        public string TenTinhTHanh { get; set; }
        public string MaQuanHuyen { get; set; }
        public string TenQuanHuyen { get; set; }
        public string TenHangXe { get; set; }
        public string TenLoaiXe { get; set; }
        public string TenMauXe { get; set; }
        public string NguoiTao { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
        public Guid? ID_HangHoa{ get; set; }
        public int NguoiSoHuu { get; set; }
    }

    public class GetListGaraDanhMucXe_v1_Export
    {
        public string BienSo { get; set; }
        public string MaChuXe { get; set; }
        public string ChuXe { get; set; }
        public string DienThoai { get; set; }
        public string HangXe { get; set; }
        public string LoaiXe { get; set; }
        public string MauXe { get; set; }
        public int NamSanXuat { get; set; }
        public string SoKhung { get; set; }
        public string SoMay { get; set; }
        public string MauSon { get; set; }
        public string DungTich { get; set; }
        public string HopSo { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
    }

    public class LichSuSuaChua_Export
    {
        public string MaHoaDon { get; set; }
        public string MaBaoGia { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double PhaiThanhToanKhachHang { get; set; }
        public double PhaiThanhToanBaoHiem { get; set; }
        public double DaThanhToanKhacchHang { get; set; }
        public double DaThanhToanBaoHiem { get; set; }
        public double ConThieuKhachHang { get; set; }
        public double ConThieuBaoHiem { get; set; }
        public string GhiChu { get; set; }
    }

    public class PhieuTiepNhanBaoGia_Export
    {
        public string MaBaoGia { get; set; }
        public DateTime NgayLapBaoGia { get; set; }
        public double TongTienHang { get; set; }
        public double KhachCanTra { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
    }

    public class NamSanXuat
    {
        public List<int> NamSanXuatInit()
        {
            List<int> namSanXuat = new List<int>() { 2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015, 2016,
            2017, 2018, 2019, 2020};
            return namSanXuat;
        }
    }

    public class ParamUpdateLichBaoDuong
    {
        public List<Guid> IDHangHoas { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public DateTime? NgayLapHoaDonOld { get; set; }
    }

    public class ParamSeachLichBaoDuong
    {
        public List<string> IDChiNhanhs { get; set; }
        public string TextSeach { get; set; }
        public DateTime? NgayBaoDuongFrom { get; set; }
        public DateTime? NgayBaoDuongTo { get; set; }
        public DateTime? NgayNhacFrom { get; set; }
        public DateTime? NgayNhacTo { get; set; }
        public List<string> IDNhanVienPhuTrachs { get; set; }
        public string IDNhomHangs { get; set; }
        public string ID_Xe { get; set; }
        public string ID_PhieuTiepNhan { get; set; }
        public string LanNhacs { get; set; }
        public string TrangThais { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public List<string> ColumnsHide { get; set; }
    }

    public class GetListNhatKyBaoDuongTheoXe
    {
        public Guid ID { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public int SoKmVao { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double SoLuong { get; set; }
        public int LanBaoDuong { get; set; }
        public int TrangThai { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    //public class GetListLichBaoDuongTheoXe
    //{
    //    public Guid ID { get; set; }
    //    public DateTime NgayBaoDuongDuKien { get; set; }
    //    public string MaHangHoa { get; set; }
    //    public string TenHangHoa { get; set; }
    //    public int LanBaoDuong { get; set; }
    //    public int SoKmBaoDuong { get; set; }
    //    public int TrangThai { get; set; }
    //    public int TotalRow { get; set; }
    //    public double TotalPage { get; set; }
    //}

    public class GetListPhieuBanGiao_v1_Result
    {
        public Guid Id { get; set; }
        public string MaPhieu { get; set; }
        public DateTime NgayGiaoXe { get; set; }
        public int SoKmBanGiao { get; set; }
        public Guid IdXe { get; set; }
        public string BienSo { get; set; }
        public string TenMauXe { get; set; }
        public string TenHangXe { get; set; }
        public string TenLoaiXe { get; set; }
        public string Somay { get; set; }
        public string SoKhung { get; set; }
        public int NamSanXuat { get; set; }
        public string MauSon { get; set; }
        public Guid? IdKhachHang { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public int LaNhanVien { get; set; }
        public Guid? IdNhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string DienThoaiNhanVien { get; set; }
        public Guid IdNhanVienBanGiao { get; set; }
        public string MaNhanVienGiao { get; set; }
        public string TenNhanVienGiao { get; set; }
        public Guid? IdNhanVienTiepNhan { get; set; }
        public string MaNhanVienNhan { get; set; }
        public string TenNhanVienNhan { get; set; }
        public DateTime? NgayNhanXe { get; set; }
        public string GhiChuBanGiao { get; set; }
        public string GhiChuTiepNhan { get; set; }
        public int TrangThai { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }
    public class GetListPhieuBanGiao_v1_Input
    {
        public DateTime? NgayGiaoXeFrom { get; set; }
        public DateTime? NgayGiaoXeTo { get; set; }
        public List<int> TrangThais { get; set; }
        public string TextSearch { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class GetListNhatKyByIdPhieuBanGiao_v1_Result
    {
        public Guid Id { get; set; }
        public DateTime ThoiGianHoatDong { get; set; }
        public double SoGioHoatDong { get; set; }
        public int SoKmHienTai { get; set; }
        public string GhiChu { get; set; }
        public int LaNhanVien { get; set; }
        public Guid? IdKhachHang { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public Guid? IdNhanVienThucHien { get; set; }
        public string MaNhanVienThucHien { get; set; }
        public string TenNhanVienThucHien { get; set; }
        public int TrangThai { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }
    public class GetListNhatKyByIdPhieuBanGiao_v1_Input
    {
        public Guid IdPhieuBanGiao { get; set; }
        public List<int> TrangThais { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class NhatKyHoatDong_Input
    {
        public Guid Id { get; set; }
        public Guid IdPhieuBanGiao { get; set; }
        public Guid? IdNhanVienThucHien { get; set; }
        public Guid? IdKhachHang { get; set; }
        public int LaNhanVien { get; set; } = 0;
        public DateTime ThoiGianHoatDong { get; set; }
        public double SoGioHoatDong { get; set; } = 0;
        public int SoKmHienTai { get; set; } = 0;
        public string GhiChu { get; set; } = "";
        public int TrangThai { get; set; } = 1;
        public string UserName { get; set; } = "";
    }

    public class GetListPhuTungTheoDoiByIdXe_v1_Result
    {
        public Guid Id { get; set; }
        public Guid IdHoaDon { get; set; }
        public Guid IdDonViQuiDoi { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public int LoaiHoaDon { get; set; }
        public Guid IdHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double DinhMucBaoDuong { get; set; }
        public double ThoiGianHoatDong { get; set; }
        public double ThoiGianConLai { get; set; }
    }
}
