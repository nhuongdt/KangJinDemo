using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_NhomHangHoa")]
    public partial class DM_NhomHangHoa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_NhomHangHoa()
        {
            DM_HangHoa = new HashSet<DM_HangHoa>();
            DM_NhomHangHoa1 = new HashSet<DM_NhomHangHoa>();
            NhomHangHoa_DonVi = new HashSet<NhomHangHoa_DonVi>();
            DM_KhuyenMai_ChiTiet = new HashSet<DM_KhuyenMai_ChiTiet>();
            DM_KhuyenMai_ChiTiet1 = new HashSet<DM_KhuyenMai_ChiTiet>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaNhomHangHoa { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenNhomHangHoa { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenNhomHangHoa_KhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenNhomHangHoa_KyTuDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_Parent { get; set; }

        [Column(TypeName = "bit")]
        public bool LaNhomHangHoa { get; set; }

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

        [Column(TypeName = "bit")]
        public bool? HienThi_Chinh { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? HienThi_Phu { get; set; } = false;

        [Column(TypeName = "nvarchar(max)")]
        public string MayIn { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_Kho { get; set; }

        [Column(TypeName = "bit")]
        public bool? HienThi_BanThe { get; set; } = false;

        [Column(TypeName = "int")]
        public int? MauHienThi { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa> DM_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_NhomHangHoa> DM_NhomHangHoa1 { get; set; }

        public virtual DM_NhomHangHoa DM_NhomHangHoa2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhomHangHoa_DonVi> NhomHangHoa_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ChiTiet> DM_KhuyenMai_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ChiTiet> DM_KhuyenMai_ChiTiet1 { get; set; }
    }
}
