using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_LienHe")]
    public partial class DM_LienHe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_LienHe()
        {
            The_TheKhachHang = new HashSet<The_TheKhachHang>();
            DM_LienHe_Anh = new HashSet<DM_LienHe_Anh>();
            NS_CongViec = new HashSet<NS_CongViec>();
            ChamSocKhachHang = new HashSet<ChamSocKhachHang>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DoiTuong { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaLienHe { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenLienHe { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? XungHo { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string DienThoaiCoDinh { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string Email { get; set; } = string.Empty;

        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string GhiChu { get; set; } = string.Empty;

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
        public string DiaChi { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string CanNang { get; set; } = string.Empty;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string ChieuCao { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? TrangThai { get; set; } = 1; // 0: xoa, 1: chua xoa

        [Column(TypeName = "nvarchar(max)")]
        public string ChucVu { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChucVu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TinhThanh { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuanHuyen { get; set; }

        public virtual DM_TinhThanh DM_TinhThanh { get; set; }

        public virtual DM_QuanHuyen DM_QuanHuyen { get; set; }

        public virtual DM_ChucVu DM_ChucVu { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<The_TheKhachHang> The_TheKhachHang { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_LienHe_Anh> DM_LienHe_Anh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongViec> NS_CongViec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang { get; set; }

        [NotMapped]
        public string TenDoiTuong { get; set; }
        [NotMapped]
        public string TenTinhThanh { get; set; }
        [NotMapped]
        public string TenQuanHuyen { get; set; }
    }

    public partial class SP_DM_LienHe
    {
        public Guid ID { get; set; }

        public Guid ID_DoiTuong { get; set; }

        public int LoaiLienHe { get; set; }

        public string MaLienHe { get; set; }

        public string TenLienHe { get; set; }

        public string TenDoiTuong { get; set; }

        public string SoDienThoai { get; set; }

        public int XungHo { get; set; } // anh, chi, ong, ba

        public string DienThoaiCoDinh { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string Email { get; set; }

        public string ChucVu { get; set; }

        public string DiaChi { get; set; }

        public string TenTinhThanh { get; set; }

        public string TenQuanHuyen { get; set; }

        public string GhiChu { get; set; }

        public DateTime? NgayTao { get; set; }

        public string NguoiTao { get; set; }

        public Guid? ID_TinhThanh { get; set; }

        public Guid? ID_QuanHuyen { get; set; }

        public Guid? ID_NhanVienPhuTrach { get; set; }

        //public string DiaChiFull { get; set; }
    }
}
