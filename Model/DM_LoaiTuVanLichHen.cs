using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_LoaiTuVanLichHen")]
    public partial class DM_LoaiTuVanLichHen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_LoaiTuVanLichHen()
        {
            ChamSocKhachHang = new HashSet<ChamSocKhachHang>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenLoaiTuVanLichHen { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TuVan_LichHen { get; set; } //1 - loại tư vấn, , 2- Phản hồi, 3 - loại lịch hẹn, 4 - Công việc

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "int")]
        public int? TrangThai { get; set; } = 1; //0. Xoa 1. not xoa

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang { get; set; }
    }
}
