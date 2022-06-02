using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_ViTri")]
    public partial class DM_ViTri
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_ViTri()
        {
            BH_HoaDon = new HashSet<BH_HoaDon>();
            BH_HoaDon_ChiTiet = new HashSet<BH_HoaDon_ChiTiet>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaViTri { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_KhuVuc { get; set; }

        [Column(TypeName = "bit")]
        public bool? TinhTrang { get; set; } = false;

        [StringLength(500)]
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

        [Column(TypeName = "float")]
        public double? DonGiaGio { get; set; } = 0;

        [Column(TypeName = "image")]
        public byte[] Anh { get; set; }

        [Column(TypeName = "float")]
        public double? DonGiaNgay { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoaiPhong { get; set; }

        [Column(TypeName = "int")]
        public int? Tang { get; set; } = 1;

        [Column(TypeName = "nvarchar(max)")]
        public string TenViTri { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }

        public virtual DM_KhuVuc DM_KhuVuc { get; set; }

        public virtual DM_LoaiPhong DM_LoaiPhong { get; set; }
    }
}
