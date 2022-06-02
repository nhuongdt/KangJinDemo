using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Quy_KhoanThuChi")]
    public partial class Quy_KhoanThuChi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quy_KhoanThuChi()
        {
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaKhoanThuChi { get; set; }

        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string NoiDungThuChi { get; set; } = string.Empty;

        [StringLength(500)]
        [Column(TypeName = "nvarchar")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool LaKhoanThu { get; set; }

        [Column(TypeName = "bit")]
        public bool BuTruCongNo { get; set; }

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
        public bool? TinhLuong { get; set; } = false;

        [Column(TypeName = "int")]
        public int? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }
    }
}
