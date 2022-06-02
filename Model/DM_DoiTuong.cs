using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_DoiTuong")]
    public partial class DM_DoiTuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_DoiTuong()
        {
            BH_HoaDon = new HashSet<BH_HoaDon>();
            DM_DoiTuong1 = new HashSet<DM_DoiTuong>();
            DM_HangHoa = new HashSet<DM_HangHoa>();
            DM_LienHe = new HashSet<DM_LienHe>();
            Kho_HoaDon = new HashSet<Kho_HoaDon>();
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
            The_TheKhachHang = new HashSet<The_TheKhachHang>();
            ChamSocKhachHang = new HashSet<ChamSocKhachHang>();
            ChotSo_KhachHang = new HashSet<ChotSo_KhachHang>();
            DM_DoiTuong_Anh = new HashSet<DM_DoiTuong_Anh>();
            DM_DoiTuong_Nhom = new HashSet<DM_DoiTuong_Nhom>();
            NS_CongViec = new HashSet<NS_CongViec>();
            HeThong_SMS = new HashSet<HeThong_SMS>();
            DM_DoiTuong_CongNo = new HashSet<DM_DoiTuong_CongNo>();
            Gara_DanhMucXe = new HashSet<Gara_DanhMucXe>();
            Gara_PhieuTiepNhan = new HashSet<Gara_PhieuTiepNhan>();
            BH_HoaDonBaoHiem = new HashSet<BH_HoaDon>();
            Gara_PhieuTiepNhanBaoHiem = new HashSet<Gara_PhieuTiepNhan>();
            BH_HoaDon_ChiPhi = new HashSet<BH_HoaDon_ChiPhi>();
            CSKH_DatLich = new HashSet<CSKH_DatLich>();
            Gara_Xe_PhieuBanGiao = new HashSet<Gara_Xe_PhieuBanGiao>();
            Gara_Xe_NhatKyHoatDong = new HashSet<Gara_Xe_NhatKyHoatDong>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int LoaiDoiTuong { get; set; } //1- khách hàng, 2- nhà cung cấp, 3- bảo hiểm

        [Column(TypeName = "bit")]
        public bool LaCaNhan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomDoiTuong { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaDoiTuong { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenDoiTuong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenDoiTuong_KhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenDoiTuong_ChuCaiDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DienThoai { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Fax { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Website { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string MaSoThue { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TaiKhoanNganHang { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? GioiHanCongNo { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TinhThanh { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySinh_NgayTLap { get; set; }

        [Column(TypeName = "bit")]
        public bool ChiaSe { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool TheoDoi { get; set; } = false; //Trạng thái: xóa: true (1), đang sử dụng: false (0)

        [Column(TypeName = "int")]
        public int? ID_Index { get; set; }

        [Column(TypeName = "bit")]
        public bool? TheoDoiVanTay { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuanHuyen { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienPhuTrach { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayDoiNhom { get; set; }

        [Column(TypeName = "float")]
        public double? DiemKhoiTao { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? DoanhSoKhoiTao { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NguoiGioiThieu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienGioiThieu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NguonKhach { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string CapTai_DKKD { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChi { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? GioiTinhNam { get; set; } = true;

        [Column(TypeName = "datetime")]
        public DateTime? NgayCapCMTND_DKKD { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiCapCMTND_DKKD { get; set; } = string.Empty;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string SDT_CoQuan { get; set; } = string.Empty;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string SDT_NhaRieng { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string SoCMTND_DKKD { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string ThuongTru { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string XungHo { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuocGia { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayGiaoDichGanNhat { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomCu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string ChucVu { get; set; } = string.Empty;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string LinhVuc { get; set; } = string.Empty;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string NgheNghiep { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenKhac { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NganHang { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "float")]
        public double? TongTichDiem { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string DinhDang_NgaySinh { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TrangThai { get; set; }

        [Column(TypeName = "int")]
        public int? TrangThai_TheGiaTri { get; set; } = 1; // 1: Đang hoạt động , 2: NGừng hoạt động

        [Column(TypeName = "nvarchar(max)")]
        public string TenNhomDoiTuongs { get; set; } = "Nhóm mặc định";

        [Column(TypeName = "nvarchar(max)")]
        public string IDNhomDoiTuongs { get; set; } = "";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeThong_SMS> HeThong_SMS { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong> DM_DoiTuong1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongViec> NS_CongViec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong_CongNo> DM_DoiTuong_CongNo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiPhi> BH_HoaDon_ChiPhi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CSKH_DatLich> CSKH_DatLich { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong2 { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_NganHang DM_NganHang { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual DM_NhomDoiTuong DM_NhomDoiTuong { get; set; }

        public virtual DM_QuanHuyen DM_QuanHuyen { get; set; }

        public virtual DM_QuocGia DM_QuocGia { get; set; }

        public virtual DM_TinhThanh DM_TinhThanh { get; set; }

        public virtual DM_NguonKhachHang DM_NguonKhachHang { get; set; }

        public virtual DM_DoiTuong_TrangThai DM_DoiTuong_TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa> DM_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_LienHe> DM_LienHe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon> Kho_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<The_TheKhachHang> The_TheKhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChotSo_KhachHang> ChotSo_KhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong_Anh> DM_DoiTuong_Anh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong_Nhom> DM_DoiTuong_Nhom { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_DanhMucXe> Gara_DanhMucXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_PhieuTiepNhan> Gara_PhieuTiepNhan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDonBaoHiem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_PhieuTiepNhan> Gara_PhieuTiepNhanBaoHiem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_PhieuBanGiao> Gara_Xe_PhieuBanGiao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_NhatKyHoatDong> Gara_Xe_NhatKyHoatDong { get; set; }

        [NotMapped]
        public string PhuongXa { get; set; }
        [NotMapped]
        public double? NoHienTai { get; set; }
        [NotMapped]
        public double? TongBan { get; set; }
        [NotMapped]
        public string TenNhomDT { get; set; }
        [NotMapped]
        public string KhuVuc { get; set; }
        [NotMapped]
        public double? TongBanTruTraHang { get; set; }
        [NotMapped]
        public string CongTy { get; set; }
        [NotMapped]
        public string DienThoaiChiNhanh { get; set; }
        [NotMapped]
        public string DiaChiChiNhanh { get; set; }
        [NotMapped]
        public double? NoCanTra { get; set; }
        [NotMapped]
        public double? TongMua { get; set; }
        [NotMapped]
        public string TenDTUnSign { get; set; }
        [NotMapped]
        public string TenDTCharStarts { get; set; }
    }
    public partial class DM_DoiTuong_Excel
    {
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string TenNhomDT { get; set; }
        public string GioiTinhNam { get; set; }
        public DateTime? NgaySinh_NgayTLap { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string KhuVuc { get; set; }
        public string PhuongXa { get; set; }
        public string NguonKhach { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string TenNhanVienPhuTrach { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public double NoHienTai { get; set; }
        public double TongBan { get; set; }
        public double TongBanTruTraHang { get; set; }
        public double? TongTichDiem { get; set; }
        public DateTime? NgayGiaoDichGanNhat { get; set; }
        public string TrangThaiKhachHang { get; set; }
        public string GhiChu { get; set; }
    }
    public partial class DM_NhaCungCap_Excel
    {
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string TenNhomDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string KhuVuc { get; set; }
        public string PhuongXa { get; set; }
        public string NguoiTao { get; set; }
        public double NoCanTraHienTai { get; set; }
        public double TongMua { get; set; }
        public double? PhiDichVu { get; set; }
        public string GhiChu { get; set; }
    }

    public class SP_DM_DoiTuong
    {
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public string TenNhomDT { get; set; }
        public bool? GioiTinhNam { get; set; }
        public string GioiTinh { get { return (GioiTinhNam ?? false) ? "Nam" : "Nữ"; } }
        public DateTime? NgaySinh_NgayTLap { get; set; }
        public string Email { get; set; }
        public string MaSoThue { get; set; }
        public string TaiKhoanNganHang { get; set; }
        public string DiaChi { get; set; }
        public string KhuVuc { get; set; }
        public string PhuongXa { get; set; }
        public string TenNguonKhach { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string TenNhanVienPhuTrach { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public double? NoHienTai { get; set; }
        public double? TongBan { get; set; }
        public double? TongBanTruTraHang { get; set; }
        public double? PhiDichVu { get; set; }
        public double? TongTichDiem { get; set; }
        public DateTime? NgayGiaoDichGanNhat { get; set; }
        public string TrangThaiKhachHang { get; set; }
        public string GhiChu { get; set; }
        public bool? TheoDoi { get; set; }

        public Guid ID { get; set; }
        public string ID_NhomDoiTuong { get; set; }
        public string TenDoiTuong_KhongDau { get; set; }
        public string TenDoiTuong_ChuCaiDau { get; set; }
        public Guid? ID_NguonKhach { get; set; }
        public Guid? ID_NhanVienPhuTrach { get; set; }
        public Guid? ID_NguoiGioiThieu { get; set; }
        public bool LaCaNhan { get; set; }
        public Guid? ID_TinhThanh { get; set; }
        public Guid? ID_QuanHuyen { get; set; }
        public string DienThoaiChiNhanh { get; set; }
        public double? TongMua { get; set; }
        public double? SoLanMuaHang { get; set; }
        public string Name_Phone { get; set; }
        public string DinhDang_NgaySinh { get; set; }
        public Guid? ID_TrangThai { get; set; }
        public int? TrangThai_TheGiaTri { get; set; }
        public double? NoTruoc { get { return 0; } }
        public string MaNVPhuTrach { get; set; }
        public int? TotalRow { get; set; } // sum footer
        public double? TotalPage { get; set; }
        public double? TongBanAll { get; set; }
        public double? TongBanTruTraHangAll { get; set; }
        public double? TongPhiDichVu { get; set; }
        public double? TongTichDiemAll { get; set; }
        public double? NoHienTaiAll { get; set; }
    }

    public class DoiTuongSMSDTO
    {
        public Guid ID { get; set; }
        public string TenNhomDT { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public DateTime? NgaySinh_NgayTLap { get; set; }
        public string TrangThaiKHGuiTin { get; set; }
        public int CountTrangThaiGuiTin { get; set; }
        public int TrangThai { get; set; }
        public DateTime? ThoiGianGui { get; set; }
        public string NguoiGui { get; set; }
        public double? TotalPage { get; set; }
        public int? TotalRow { get; set; }
    }

    public class DoiTuongSMSGiaoDich
    {
        public Guid ID { get; set; }
        public Guid ID_HoaDon { get; set; }
        public string TenNhomDT { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public int? LoaiHoaDon { get; set; }
        public string MaHoaDon { get; set; }
        public string LoaiGiaoDich { get; set; }
        public string TrangThaiGuiTin { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayLapHoaDon { get; set; }
        public int CountTrangThaiGuiTin { get; set; }
        public DateTime? ThoiGianGui { get; set; }
        public string NguoiGui { get; set; }
        public double? TotalPage { get; set; }
        public int? TotalRow { get; set; }
    }

    public partial class SP_KhachHang_HangHoa
    {
        public Guid ID_DoiTuong { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public double SoLuong { get; set; }
    }

    public partial class SP_KhachHang_TheGiaTri
    {
        public double? SoDuTheGiaTri { get; set; }
        public double? TongThuTheGiaTri { get; set; }
        public double? SuDungThe { get; set; }
        public double? HoanTraTheGiaTri { get; set; }
        public double? CongNoThe { get; set; }
    }

    public partial class Params_GetListKhachHang
    {
        public List<Guid> ID_DonVis { get; set; }
        public int LoaiDoiTuong { get; set; }
        public string MaDoiTuong { get; set; }
        public string ID_NhomDoiTuong { get; set; }
        public DateTime? NgayTao_TuNgay { get; set; }
        public DateTime? NgayTao_DenNgay { get; set; }
        public DateTime? TongBan_TuNgay { get; set; }
        public DateTime? TongBan_DenNgay { get; set; }
        public double? TongBan_Tu { get; set; }
        public double? TongBan_Den { get; set; }
        public double? NoHienTai_Tu { get; set; }
        public double? NoHienTai_Den { get; set; }
        public int No_TrangThai { get; set; } // 0.Tất cả, 1.Còn nợ, 2.Hết nợ
        public int GioiTinh { get; set; }
        public int LoaiKhach { get; set; }// ca nha, cong ty
        public List<string> ID_TinhThanhs { get; set; }
        public DateTime? NgaySinh_TuNgay { get; set; }
        public DateTime? NgaySinh_DenNgay { get; set; }
        public int LoaiNgaySinh { get; set; }// 0.Ngay/Thang, 1.Nam
        public string ID_NguonKhach { get; set; }
        public List<string> ID_NhanVienQuanLys { get; set; }
        public int TrangThai_SapXep { get; set; } // 0.Không SX, 1.Có
        public string Cot_SapXep { get; set; }
        public string NguoiTao { get; set; }
        public string ColumnsHide { get; set; } // use when export excel
        public Guid? ID_TrangThai { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string WhereSql { get; set; }
        public List<ColumSearch> SearchColumns { get; set; }
    }

    public partial class Params_GetListKHSMSDTOLT
    {
        public string ID_NhomDoiTuong { get; set; }
        public DateTime? NgaySinh_TuNgay { get; set; }
        public DateTime? NgaySinh_DenNgay { get; set; }
        public int? LoaiNgaySinh { get; set; }// 0.Ngay/Thang, 1.Nam
        public List<string> ID_NhanVienQuanLys { get; set; }
        public string NguoiTao { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TrangThai { get; set; }
    }

    public partial class Params_GetListKHSMSGiaoDich
    {
        public string ID_NhomDoiTuong { get; set; }
        public string iddonvi { get; set; }
        public DateTime? NgaySinh_TuNgay { get; set; }
        public DateTime? NgaySinh_DenNgay { get; set; }
        public int LoaiNgaySinh { get; set; }// 0.Ngay/Thang, 1.Nam
        public List<string> ID_NhanVienQuanLys { get; set; }
        public List<string> ID_DonViArr { get; set; }
        public string NguoiTao { get; set; }
        public int LoaiGiaoDich { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TrangThai { get; set; }
    }
}
