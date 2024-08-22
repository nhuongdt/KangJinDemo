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

    public class TGT_NhatKyDieuChinhDTO: TGT_LichSuNapTraDTO
    {
        public Guid? ID_DoiTuong { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string NguoiTao { get; set; }
        public string TenDonVi { get; set; }
        public string STrangThai { get; set; }
        public bool? ChoThanhToan { get; set; }
    }

    public class TGT_NhatKyTatToanTGT
    {
        public Guid? ID { get; set; }
        public Guid? ID_DoiTuong { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public string MaHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public double? PhaiThanhToan { get; set; }
        public string NguoiTao { get; set; }
        public string STrangThai { get; set; }
        public string DienGiai { get; set; }
        public bool? ChoThanhToan { get; set; }
    }
    public class PhieuTatToanTheGiaTriDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_DoiTuong { get; set; }
        public Guid? ID_DonVi { get; set; }
        public Guid? ID_NhanVien { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public double? PhaiThanhToan { get; set; }
        public double? TongTienHang { get; set; }
        public string NguoiTao { get; set; }
        public string STrangThai { get; set; }
        public string DienGiai { get; set; }
        public bool? ChoThanhToan { get; set; }

        // infor thenap
        public string MaTheGiaTri { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public double? GiaTriNap { get; set; }
        public double? KhachDaTra { get; set; }
        public double? ConNo { get; set; }
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
    public class HoaHongGioiThieu_ChiTiet_DTO
    {
        public Guid? ID_DoiTuong { get; set; }
        public Guid? ID_HoaDon_DuocCK { get; set; }
        public Guid? ID_QuyHoaDon { get; set; }
        public int? LoaiHoaDon { get; set; }// loaiHD trich: hdban, the, gdv
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public double? TongThanhToan { get; set; }
        public double? KhachDaTra { get; set; }
        public double? SoTienTrich { get; set; } // = (TienThue) Số tiền thực tế tính chiết khấu cho khách
        public double? PTChietKhau { get; set; }
        public double? TienChietKhau { get; set; }
        public double? DaTrich { get; set; }// (PTChiPhi) số tiền đã trích trước đó
        public double? ConLai { get; set; }// (PTChiPhi) Số tiền còn lại được trích
        public int? TrangThai { get; set; }
    }

    public class BCHoaHongGioiThieu_ChiTiet: HoaHongGioiThieu_ChiTiet_DTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_CheckIn { get; set; }// idnguoigt
        public string MaPhieuTrich { get; set; }
        public DateTime? NgayLapPhieuTrich { get; set; }
        public double? TongChietKhau { get; set; }// loaiDoiTuong (int)
        public string SLoaiDoiTuong { get; set; }
        public string MaNguoiGioiThieu { get; set; }
        public string TenNguoiGioiThieu { get; set; }
        public int? TotalRow { get; set; }
        public double? SumTongTienHang { get; set; }
        public double? SumDaTrich { get; set; }
        public double? SumTienChietKhau { get; set; }
    }

    public class HoaHongGioiThieuDTO
    {
        public Guid? ID { get; set; }
        public Guid? ID_CheckIn { get; set; }// idnguoigt
        public Guid? ID_DonVi { get; set; }
        public string MaHoaDon { get; set; }
        public DateTime? NgayLapHoaDon { get; set; }
        public double? TongChietKhau { get; set; }// loaiDoiTuong (int)
        public string SLoaiDoiTuong { get; set; }
        public string MaNguoiGioiThieu { get; set; }
        public string TenNguoiGioiThieu { get; set; }
        public double? TongTienHang { get; set; }
        public double? KhachDaTra { get; set; }
        public double? ConNo { get; set; }
        public string DienGiai { get; set; }
        public int? TrangThai { get; set; }// 0.hoanthanh, 1.tamluu, 2.huy
        public string STrangThai { get; set; }// hoanthanh, huy
        public string NguoiTao { get; set; }
        public string TenDonVi { get; set; }
        public int? TotalRow { get; set; }
        public double? SumTongTienHang { get; set; }
        public double? SumKhachDaTra { get; set; }
        public double? SumConNo { get; set; }
    }
    public class DM_NguoiGioiThieuDTO
    {
        public Guid? ID_CheckIn { get; set; }// idnguoigt
        public double? LoaiDoiTuong { get; set; }// do muon truong {TongChietKhau} de luu, nen kieu dulieu = double
        public string SLoaiDoiTuong { get; set; }
        public string MaNguoiGioiThieu { get; set; }
        public string TenNguoiGioiThieu { get; set; }
        public string SDTNguoiGioiThieu { get; set; }
        public string DiaChiNguoiGioiThieu { get; set; }
        public double? TongTienHang { get; set; }
        public string STrangThai { get; set; }// hoanthanh, huy
        public DateTime? NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public int? TotalRow { get; set; }
        public double? SumTongTienHang { get; set; }
    }

    public class BaoCaoHoTroDTO
    {
        public Guid? ID_DoiTuong { get; set; }
        public string TenNhanVien { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenNhomHoTro { get; set; }
        public double? GiaTriSuDung { get; set; }
        public double? GtriHoTro_theoQuyDinh { get; set; }
        public double? DaHoTro { get; set; }
        public double? PTramHoTro { get; set; }
        public int? KieuHoTro { get; set; }//1.%, 0.vnd
        public string TenDonVi { get; set; }
        public int? TotalRow { get; set; }
        public double? TotalPage { get; set; }
        public double? SumGiaTriSuDung { get; set; }
        public double? SumGiaTriHoTro { get; set; }
    }
    public class BaoCaoDoanhThuKhachHangDTO
    {
        public string NgayThanhToan { get; set; }// yyyy-MM-dd
        public Guid? ID_DoiTuong { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public double? TongThanhToan { get; set; }
        public double? HoanCoc { get; set; }
        public double? HoanDichVu { get; set; }
        public int? TotalRow { get; set; }
        public double? SumTongThanhToan { get; set; }
        public double? SumHoanCoc { get; set; }
        public double? SumHoanDichVu { get; set; }
    }

    public class CellDTO
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Text { get; set; }
    }
}
