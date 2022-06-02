using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Kho_HoaDon")]
    public partial class Kho_HoaDon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kho_HoaDon()
        {
            Kho_HoaDon_ChiTiet = new HashSet<Kho_HoaDon_ChiTiet>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaHoaDon { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayLapHoaDon { get; set; }

        [Column(TypeName = "int")]
        public int LoaiChungTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DoiTuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NgoaiTe { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiGiao { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? TongThanhTien { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChungTuLienQuan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string DienGiai { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_PhieuSuaChua { get; set; }

        public virtual BH_HoaDon BH_HoaDon { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_LoaiChungTu DM_LoaiChungTu { get; set; }

        public virtual DM_TienTe DM_TienTe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon_ChiTiet> Kho_HoaDon_ChiTiet { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
