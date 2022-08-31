using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class TheGiaTriDTO
    {
        public Guid? ID { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public double? MucNap { get; set; }
        public double? KhuyenMaiVND { get; set; }
        public double? TongTienNap { get; set; }
        public double? SoDuSauNap { get; set; }
        public double? ChietKhauVND { get; set; }
        public double? PhaiThanhToan { get; set; }
        public double? KhachDaTra { get; set; }
        public string DienGiai { get; set; }
    }
    public class TGT_LichSuNapTraDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_SoQuy { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public string SLoaiHoaDon { get; set; }
        public double? PhatSinhTang { get; set; }
        public double? PhatSinhGiam { get; set; }
        public double? PhaiThanhToan { get; set; }
        public double? DaThanhToan { get; set; }
        public string DienGiai { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public double? TongTang { get; set; }
        public double? TongGiam { get; set; }
    }
    public class BaoCaoHoatDongXe_TongHop
    {
        public Guid? ID_Xe { get; set; }
        public Guid? ID_DonVi { get; set; }
        public string TenDonVi { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string BienSo { get; set; }
        public int? SoGioHoatDong { get; set; }
        public string TenNhomPhuTung { get; set; }
        public string MaPhuTung { get; set; }
        public string TenPhuTung { get; set; }
        public string TenDonViTinh { get; set; }
        public int? MocBaoHanh { get; set; }
        public int? BHConLai { get; set; }
        public string BHTrangThai { get; set; }
        public double? TongSoGioHoatDong { get; set; }
        public int? TotalRow { get; set; }
        public Guid? ID_HangHoa { get; set; }
    }
    public class BaoCaoHoatDongXe_ChiTiet
    {
        public Guid? ID_PhieuTiepNhan { get; set; }
        public string TenDonVi { get; set; }
        public string MaPhieuTiepNhan { get; set; }
        public DateTime? NgayVaoXuong { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string BienSo { get; set; }
        public int? SoGioHoatDong { get; set; }
        public string TenNhanVien { get; set; }
        public string GhiChu { get; set; }
        public string TenNhomPhuTung { get; set; }
        public string MaPhuTung { get; set; }
        public string TenPhuTung { get; set; }
        public string TenDonViTinh { get; set; }
        public double? TongSoGioHoatDong { get; set; }
        public int? TotalRow { get; set; }
        public Guid? ID_HangHoa { get; set; }
        public Guid? ID_Xe { get; set; }
    }

    public class SoQuyDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_HoaDonGoc { get; set; }
        public int? LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public string MaPhieuChi { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double? PhaiThanhToan { get; set; }
        public double? DuNoKH { get; set; }
        public string strLoaiHoaDon { get; set; }
        public int? LoaiThanhToan { get; set; }//1.coc, 2.dieuchinhcongno, 3.khong tinh congno
        public int? TotalRow { get; set; }
    }
    public class NhatKyThanhToanDTO
    {
        public Guid? ID { get; set; }
        public int? LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public double? TongTienThu { get; set; }
        public string NguoiTao { get; set; }
        public string NguoiSua { get; set; }
        public bool? TrangThai { get; set; }
        public string PhuongThucTT { get; set; }
        public string SLoaiHoaDon { get; set; }
    }

    public class ParamRpHoatDongXe : CommonParamSearch
    {
        public List<string> IDNhomHangs { get; set; }
        public List<string> IDNhomPhuTungs { get; set; }
        public List<string> IDNhanViens { get; set; }
        public int? TrangThai { get; set; }
        public double? SoGioHoaDongTu { get; set; }
        public double? SoGioHoaDongDen { get; set; }
        public Param_ReportText ReportText { get; set; }
    }

    public class DTOInt
    {
        public int? Loai { get; set; }
    }

    public class PhieuDieuChinhDTO
    {
        public Guid? ID { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string TenDonVi { get; set; }
        public string NguoiTao { get; set; }
        public string DienGiai { get; set; }
        public string TrangThai { get; set; }
        public bool? ChoThanhToan { get; set; }
        public int? TongSLMatHang { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_DonVi { get; set; }
    }

    public class PhieuDieuChinhChiTietDTO : PhieuDieuChinhDTO
    {
        public Guid? ID_ChiTiet { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinhGiaTri { get; set; }
        public double? GiaBan { get; set; }// giaban of hanghoa
        public double? DonGia { get; set; }// giavoncu
        public double? ThanhTien { get; set; }// giavonmoi
        public double? GiaVon { get; set; }// giavonTB
        public Guid? ID_HangHoa { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
    }

    public class Excel_PhieuDieuChinhChiTiet
    {
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public double? GiaBan { get; set; }// giaban hh
        public double? ThanhTien { get; set; }// gvtieuchuan moi
        public double? GiaVon { get; set; }
    }


    public class HD_CTHDHoTroDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_DoiTuong { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public Guid? Id_NhomHang { get; set; }
        public string TenNhomHangHoa { get; set; }
        public double? SoNgayThuoc { get; set; }
        public string NguoiTao { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public bool? IsChuyenPhatNhanh { get; set; }// = column An_Hien
        public string DienGiai { get; set; }

        public Guid? ID_ChiTietHD { get; set; }
        public Guid? ID_ChiTietDinhLuong { get; set; }
        public Guid? ID_ChiTietGoiDV { get; set; }
        public Guid? Id_DonViQuiDoi { get; set; }
        public Guid? Id_LoHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public int? LoaiHangHoa { get; set; }
        public string MaLoHang { get; set; }
        public string TenDonViTinh { get; set; }
        public double? TienChietKhau { get; set; }
        public double? SoLuong { get; set; }
        public string ChatLieu { get; set; }
    }
}
