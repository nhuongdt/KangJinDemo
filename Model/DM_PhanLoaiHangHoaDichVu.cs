using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_PhanLoaiHangHoaDichVu")]
    public partial class DM_PhanLoaiHangHoaDichVu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_PhanLoaiHangHoaDichVu()
        {
            DM_HangHoa = new HashSet<DM_HangHoa>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaPhanLoai { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string TenPhanLoai { get; set; }

        [Column(TypeName = "ntext")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? ThoiGianBaoHanh { get; set; } = 0;

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
        public virtual ICollection<DM_HangHoa> DM_HangHoa { get; set; }
    }
}
