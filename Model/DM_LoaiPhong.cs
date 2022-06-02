using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_LoaiPhong")]
    public partial class DM_LoaiPhong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_LoaiPhong()
        {
            DM_ViTri = new HashSet<DM_ViTri>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaLoai { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenLoai { get; set; }

        [Column(TypeName = "int")]
        public int? SoNguoi_Min { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? SoNguoi_Max { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool SuDung { get; set; }

        [Column(TypeName = "int")]
        public int? MauSac { get; set; } = 0;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_ViTri> DM_ViTri { get; set; }
    }
}
