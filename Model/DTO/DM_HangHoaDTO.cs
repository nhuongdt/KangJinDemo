using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DM_Ton
    {
        public double? TonKho { get; set; }
    }

    public class NhomHangHoa_KhoangApDungDTO
    {
        public Guid? Id_NhomHang { get; set; }
        public string TenNhomHangHoa { get; set; }
        public double? GiaTriSuDungTu { get; set; }
        public double? GiaTriSuDungDen { get; set; }
        public double? GiaTriHoTro { get; set; }
        public int? KieuHoTro { get; set; }
    }

    public class NhomHangHoa_SanPhamHoTroDTO
    {
        public Guid? Id_NhomHang { get; set; }// nhóm hỗ trợ
        public Guid? Id_DonViQuiDoi { get; set; }
        public Guid? Id_LoHang { get; set; }
        public double? SoLuong { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public int? LaSanPhamNgayThuoc { get; set; }
        public int? LoaiHangHoa { get; set; }
        public Guid? ID_NhomHangHoa { get; set; } // nhóm hàng hóa
        public string TenNhomHangHoa { get; set; } // nhóm hàng hóa
    }

    public class NhomHangHoa_TongSuDung
    {
        public Guid? Id_NhomHang { get; set; }
        public double? TongGiaTriSuDung { get; set; }

    }
    public class DieuChinhGiaVon_HangHoaDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public double? TonKho { get; set; }
        public double? GiaVon { get; set; }
        public double? GiaVonTieuChuan { get; set; }
        public string SrcImage { get; set; }
    }

    public class DM_HangHoaDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DonVi { get; set; }
        public Guid? ID_HangHoaCungLoai { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonVi { get; set; }
        public string TenHangHoa_KhongDau { get; set; }
        public string TenHangHoa_KyTuDau { get; set; }
        public string TenHangHoaUnsign { get; set; }
        public string TenHangHoaCharStart { get; set; }
        public string MaHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string MaLoHang { get; set; }
        public string MaNhomHangHoa { get; set; }
        public string TenNhomHangHoa { get; set; }
        public string SrcImage { get; set; }
        public string MaHangHoaUnsign { get; set; }
        public string MaHangHoaCharStart { get; set; }
        public string NhomHangHoa { get; set; }
        public string PhanLoaiHangHoa { get; set; }
        //kiểm kho
        public double? TienChietKhau { get; set; }
        public double? ThanhToan { get; set; }
        public double TyLeChuyenDoi { get; set; }

        public Guid? ID_NhomHangHoa { get; set; }
        public string ID_Random { get; set; }
        public double? GiaBan { get; set; }
        public double? GiaBanMaVach { get; set; }
        public double? GiaTraLai { get; set; }
        public double? GiaNhap { get; set; }
        public double? DonGia { get; set; }
        public double? GiaVon { get; set; }
        public double? TonKho { get; set; }
        public double? TonCuoiKy { get; set; }
        public double? TonDau { get; set; }
        public double? TonToiDa { get; set; }
        public double? TonToiThieu { get; set; }
        public double? SoLuongNhanChuyen { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongNhap { get; set; }
        public double? QuyCach { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public bool TrangThai { get; set; }
        public bool? LaHangHoa { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public bool? LaChaCungLoai { get; set; }
        public bool CheckCohangCungLoai { get; set; }
        public bool TheoDoi { get; set; }
        public bool DuocBanTrucTiep { get; set; }
        public bool? Xoa { get; set; }
        public string sLoaiHangHoa { get; set; }
        public string GhiChu { get; set; }
        public string ThuocTinh { get; set; }
        public double? SoLuong { get; set; }
        public double? SoLuongTra { get; set; }
        public double? SoLuongChuyenXuat { get; set; }
        public double? GiamGia { get; set; }
        public double? ThanhTien { get; set; }
        public List<DonViTinh> DonViTinh { get; set; }
        public List<HangHoa_ThuocTinh> HangHoa_ThuocTinh { get; set; }
        public List<DM_HangHoa_ViTriDTO> DM_HangHoa_ViTri { get; set; }
        public List<DM_HangHoa_AnhDTO> DM_HangHoa_Anh { get; set; }
        public List<DinhLuongDichVuDTO> DinhLuongDichVu { get; set; }
        public List<TheKhoDTO> TheKhoDTO { get; set; }
        public List<DM_HangHoaDTO> ListChildren { get; set; }
        public string DonViTinhChuan { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public string TenDonViTinh { get; set; }
        public Guid? ID_BangGia { get; set; }
        public int? ThoiGianBaoHanh { get; set; }
        public int? LoaiBaoHanh { get; set; }
        public string DonViTinhQuyCach { get; set; }
        public double ChiPhiThucHien { get; set; }
        public bool ChiPhiTinhTheoPT { get; set; }
        public bool CheckDinhLuongDV { get; set; }
        public double? SoLuongQuyCach { get; set; }
        public string ThuocTinhGiaTri { get; set; }
        public int? SoPhutThucHien { get; set; }
        public int? DichVuTheoGio { get; set; }
        public int? DuocTichDiem { get; set; }
        public double? PTThue { get; set; }
        public double? TienThue { get; set; }
        public int? ChangeGiaBan { get; set; }
        public int? LoaiBaoDuong { get; set; }
        public int? QuanLyBaoDuong { get; set; }
        public int? SoKmBaoHanh { get; set; }
        public int? LoaiHangHoa { get; set; }
        public int? HoaHongTruocChietKhau { get; set; }
        public Guid? ID_Xe { get; set; }
        public string BienSo { get; set; }
        public double? ChietKhauMD_NV { get; set; }
        public bool? ChietKhauMD_NVTheoPT { get; set; }
    }

    public class DM_DonViTinhClick
    {
        public Guid ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public double GiaNhap { get; set; }
        public double GiaBanHH { get; set; }
        public double GiaVon { get; set; }
        public double TonKho { get; set; }
        public Guid? ID_LoHang { get; set; }
    }

    public class DM_ViTriHangHoaDTO
    {
        public Guid ID { get; set; }
        public Guid ID_HangHoa { get; set; }
        public string TenViTri { get; set; }
    }

    public class DM_HangHoa_ViTriDTO
    {
        public Guid ID { get; set; }
        public string TenViTri { get; set; }
    }


    public class DM_HangHoaCungLoai
    {
        public string ID_ThuocTinh { get; set; }
        public string TenHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public double GiaVon { get; set; }
        public double GiaBan { get; set; }
        public double TonKho { get; set; }
        public bool TrangThai { get; set; }
        public string TenDonViTinh { get; set; }
        public bool LaDonViChuan { get; set; }
        public double TyLeChuyenDoi { get; set; }
    }

    public class DM_HangHoaInMaVach
    {
        public string TenHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public double GiaBan { get; set; }
    }

    public class DM_HangHoaSearch
    {
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public string TenDonViTinh { get; set; }
        public string TenHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string SrcImage { get; set; }
        public bool? LaHangHoa { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public double? GiaVon { get; set; }
        public double? GiaBan { get; set; }
        public double? GiaNhap { get; set; }
        public double? TonKho { get; set; }
        public List<DonViTinh> DonViTinh { get; set; }
        public bool CheckDinhLuongDV { get; set; }
        public double? QuyCach { get; set; }
        public int? LoaiHangHoa { get; set; }
    }

    public class BCDM_LoHangDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        //public Guid? ID_DonVi { get; set; }
        public Guid? ID_LoHang { get; set; }
        public Guid? ID_NhomHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public string MaLoHang { get; set; }
        public string NhomHangHoa { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime? NgayTaoLo { get; set; }
        public double? SoNgayConHan { get; set; }
        public string TrangThai { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public double GiaBan { get; set; }
        public double GiaVon { get; set; }
        public double? TonKho { get; set; }
        public double? TonToiThieu { get; set; }
        public double? TonToiDa { get; set; }
        public double GiaTriTon { get; set; }
        public List<DonViTinh> DonViTinh { get; set; }
        public List<HangHoa_ThuocTinh> HangHoa_ThuocTinh { get; set; }
    }

    public class DemCheck
    {
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public Guid? ID_HangHoaCungLoai { get; set; }
    }

    public class DM_HangHoa_Excel
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string NhomHangHoa { get; set; }
        public string LoaiHangHoa { get; set; }
        public double? GiaBan { get; set; }
        public double? GiaVon { get; set; }
        public double? TonKho { get; set; }
        public string GhiChu { get; set; }
        public string TrangThai { get; set; }
    }

    public class DM_LoHang_Excel
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string NhomHangHoa { get; set; }
        public double? GiaBan { get; set; }
        public double? TonKho { get; set; }
        public double? GiaVon { get; set; }
        public double? GiaTriTon { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public string TrangThai { get; set; }
        public double? SoNgayConHan { get; set; }
    }

    public class DM_TheKhoHangHoa_Excel
    {
        public string MaHoaDon { get; set; }
        public string LoaiChungTu { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double? GiaVon { get; set; }
        public double? SoLuong { get; set; }
        public double? TonKho { get; set; }
    }
    public class DM_HangHoaHoaDon
    {
        public string MaHangHoa { get; set; }
        public string MaLoHang { get; set; }
        public double? GiaBan { get; set; }
        public double? GiamGia { get; set; }
        public double SoLuong { get; set; }
        public int ThuTuHoaDon { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public Guid? ID_LoHang { get; set; }
    }
    public class ErrorDMHangHoa
    {
        public string TenTruongDuLieu { get; set; }
        public string ViTri { get; set; }
        public string ThuocTinh { get; set; }
        public string DienGiai { get; set; }
        public int rowError { get; set; }
        public int loaiError { get; set; }
        public string nameHH_excel { get; set; }
        public string nameHH_sql { get; set; }
    }
    public class exportHangHoaCapNhatPRC
    {
        public string MaHangHoa_sql { get; set; }
        public string NhomHangHoa_sql { get; set; }
        public string TenHangHoa_sql { get; set; }
        public string GhiChu_sql { get; set; }
        public double GiaBan_sql { get; set; }
        public double TonKho_sql { get; set; }
        public string NhomHangHoa_excel { get; set; }
        public string TenHangHoa_excel { get; set; }
        public string GhiChu_excel { get; set; }
        public double GiaBan_excel { get; set; }
        public double TonKho_excel { get; set; }
    }
    public class DanhSachHangNhap
    {
        public string MaHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double GiamGia { get; set; }
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public string TenHangHoa { get; set; }
        public double ThanhTien { get; set; }
        public string TenDonViTinh { get; set; }
        public double TienChietKhau { get; set; }
        public double GiaBan { get; set; }
        public double GiaTraLai { get; set; }
    }
    public class DanhSachTraHangNhap
    {
        public string MaHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double GiaTraLai { get; set; }
        public double GiamGia { get; set; }
    }
    public class DM_HangHoa_AnhDTO
    {
        public Guid ID { get; set; }
        public string URLAnh { get; set; }
    }
    public class DonViTinh
    {
        public Guid ID { get; set; }
        public Guid ID_HangHoa { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public double GiaBan { get; set; }
        public double GiaVon { get; set; }
        public bool LaDonViChuan { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public bool? Xoa { get; set; }
        public string DonViTinhChuan { get; set; }
    }

    public class DinhLuongDichVuDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DichVu { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public double SoLuong { get; set; }
        public double? QuyCach { get; set; }
        public double GiaVon { get; set; }
        public double ThanhTien { get; set; }
        public double? SoLuongQuyCach { get; set; }
    }
    public class TheKhoDTO
    {
        public Guid ID_HoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public int LoaiHoaDon { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public double SoLuong { get; set; }
        public double? GiaVon { get; set; }
        public double? TonCuoi { get; set; }
    }
    public class SP_DM_HangHoaDTO
    {
        public Guid ID { get; set; }
        public Guid? ID_LoHang { get; set; }
        public string Name { get; set; }
        public string TenHangHoa { get; set; }
        public string MaHangHoa { get; set; }
        public double? TonKho { get; set; }
        public double? TonToiThieu { get; set; }
        public double? GiaBan { get; set; }
        public double? GiaVon { get; set; }
        public string TenDonViTinh { get; set; }
        public Guid? ID_NhomHangHoa { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public string SrcImage { get; set; }
        public bool LaHangHoa { get; set; }
        public string ThuocTinh_GiaTri { get; set; }
        public bool? QuanLyTheoLoHang { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double TyLeChuyenDoi { get; set; }
        public bool? LaDonViChuan { get; set; }
        public double? QuyCach { get; set; }
        public double? PhiDichVu { get; set; }// = column GiaVon (DonViQuiDoi)
        public string DonViTinhQuyCach { get; set; }
        public bool? LaPTPhiDichVu { get; set; }
        public int? ThoiGianBaoHanh { get; set; }
        public int? LoaiBaoHanh { get; set; }
        public int? SoPhutThucHien { get; set; }
        public string GhiChuHH { get; set; }
        public int? SoGoiDV { get; set; }
        public DateTime? HanSuDungGoiDV_Min { get; set; }
        public string BackgroundColor { get; set; }
        public string CssImg { get; set; }
        public string TenNhomHangHoa { get; set; }
        public int? DichVuTheoGio { get; set; }
        public int? DuocTichDiem { get; set; }
        public int? LoaiHangHoa { get; set; }
        public int? HoaHongTruocChietKhau { get; set; }
        public double? ChietKhauMD_NV { get; set; }
        public bool? ChietKhauMD_NVTheoPT { get; set; }
    }

    public static class Comonndll
    {
        /// <summary>
        /// Dãy tiếng việt có dấu cần chuyển đổi
        /// </summary>
        public static readonly string[] VietnameseSigns = new string[]

      { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
        "đ",
        "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
        "í","ì","ỉ","ĩ","ị",
        "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
        "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
        "ý","ỳ","ỷ","ỹ","ỵ",};

        /// <summary>
        /// Dãy tiếng việt ko dấu cần chuyển đổi
        /// </summary>
        public static readonly string[] EnglishSigns = new string[]
        { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
            "d",
            "e","e","e","e","e","e","e","e","e","e","e",
            "i","i","i","i","i",
            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
            "u","u","u","u","u","u","u","u","u","u","u",
            "y","y","y","y","y",};
        public static string RemoveSign4VietnameseString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return string.Empty;
            str = str.ToLower();
            for (int i = 0; i < VietnameseSigns.Length; i++)
            {
                str = str.Replace(VietnameseSigns[i], EnglishSigns[i]);
            }
            return str;

        }
    }

}
