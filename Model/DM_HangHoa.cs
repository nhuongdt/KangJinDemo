using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_HangHoa")]
    public partial class DM_HangHoa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_HangHoa()
        {
            DonViQuiDois = new HashSet<DonViQuiDoi>();
            DM_HangHoa_Anh = new HashSet<DM_HangHoa_Anh>();
            HangHoa_ThuocTinh = new HashSet<HangHoa_ThuocTinh>();
            ChotSo_HangHoa = new HashSet<ChotSo_HangHoa>();
            DM_ViTriHangHoa = new HashSet<DM_ViTriHangHoa>();
            ChamSocKhachHang = new HashSet<ChamSocKhachHang>();
            DM_HangHoa_BaoDuongChiTiet = new HashSet<DM_HangHoa_BaoDuongChiTiet>();
            Gara_LichBaoDuong = new HashSet<Gara_LichBaoDuong>();
            CSKH_DatLich_HangHoa = new HashSet<CSKH_DatLich_HangHoa>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenHangHoa { get; set; }

        [Column(TypeName = "bit")]
        public bool? LaHangHoa { get; set; } = true;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomHang { get; set; } = Guid.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_PhanLoai { get; set; }// not use

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuocGia { get; set; }// not use

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DoiTuong { get; set; }// not use

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_HangHoaCungLoai { get; set; }

        [Column(TypeName = "float")]
        public double? QuyCach { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DVTQuyCach { get; set; }// not use

        [Column(TypeName = "int")]
        public int? LoaiBaoHanh { get; set; } = 0; // 0. Khong bao hanh, 1.Ngay, 2.Thang, 2.Nam

        [Column(TypeName = "int")]
        public int? ThoiGianBaoHanh { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string TenTGBaoHanh { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double ChiPhiThucHien { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool ChiPhiTinhTheoPT { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? TinhCPSauChietKhau { get; set; } = false;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? SoPhutThucHien { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? ChietKhauMD_NV { get; set; } = 0;// not use

        [Column(TypeName = "bit")]
        public bool? ChietKhauMD_NVTheoPT { get; set; } = false;// not use

        [Column(TypeName = "int")]
        public int? TinhGiaVon { get; set; } = 0;// not use 

        [Column(TypeName = "bit")]
        public bool TheoDoi { get; set; } = true;

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

        [Column(TypeName = "nvarchar(max)")]
        public string TenKhac { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double TonToiDa { get; set; } = 0;

        [Column(TypeName = "float")]
        public double TonToiThieu { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool DuocBanTrucTiep { get; set; } = true;

        [Column(TypeName = "nvarchar(max)")]
        public string TenHangHoa_KhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenHangHoa_KyTuDau { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? TonCuoiKy { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? LaChaCungLoai { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? QuanLyTheoLoHang { get; set; } = false;

        [Column(TypeName = "nvarchar(max)")]
        public string DonViTinhQuyCach { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int DichVuTheoGio { get; set; } = 0;

        [Column(TypeName = "int")]
        public int DuocTichDiem { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? SoKmBaoDuong { get; set; } = 0; //0 - Không cài đặt

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_Xe { get; set; } // Chọn xe để xuất bán xe

        [Column(TypeName = "int")]
        public int? LoaiHangHoa { get; set; } = 1; // 1. Hàng hóa, 2. Dịch vụ, 3. Combo

        [Column(TypeName = "int")]
        public int? QuanLyBaoDuong { get; set; } = 0; // 0. Không, 1. Có

        [Column(TypeName = "int")]
        public int? LoaiBaoDuong { get; set; } = 2; // 1. Thời gian, 2. Km

        [Column(TypeName = "int")]
        public int? SoKmBaoHanh { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? HienThiDatLich { get; set; } = 0; //0. Không hiển thị, 1. Hiển thị

        [Column(TypeName = "int")]
        public int? HoaHongTruocChietKhau { get; set; } = 0; //0. Tính hoa hồng sau chiết khấu, 1. Tính hoa hồng trước chiết khấu

        //public bool DuocTichDiem { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        public virtual DM_NhomHangHoa DM_NhomHangHoa { get; set; }

        public virtual DM_PhanLoaiHangHoaDichVu DM_PhanLoaiHangHoaDichVu { get; set; }

        public virtual DM_QuocGia DM_QuocGia { get; set; }

        public virtual Gara_DanhMucXe Gara_DanhMucXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonViQuiDoi> DonViQuiDois { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa_Anh> DM_HangHoa_Anh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HangHoa_ThuocTinh> HangHoa_ThuocTinh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChotSo_HangHoa> ChotSo_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_ViTriHangHoa> DM_ViTriHangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa_BaoDuongChiTiet> DM_HangHoa_BaoDuongChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_LichBaoDuong> Gara_LichBaoDuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CSKH_DatLich_HangHoa> CSKH_DatLich_HangHoa { get; set; }
    }
}
