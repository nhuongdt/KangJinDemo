using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChietKhauMacDinh_HoaDon")]
    public partial class ChietKhauMacDinh_HoaDon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChietKhauMacDinh_HoaDon()
        {
            ChietKhauMacDinh_HoaDon_ChiTiet = new HashSet<ChietKhauMacDinh_HoaDon_ChiTiet>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "int")]
        public int TinhChietKhauTheo { get; set; }// 1.ThucThu, 2.DoanhThu, , 3.VND

        [Column(TypeName = "float")]
        public double GiaTriChietKhau { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string ChungTuApDung { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } // 0.Đã xóa

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChietKhauMacDinh_HoaDon_ChiTiet> ChietKhauMacDinh_HoaDon_ChiTiet { get; set; }

        [NotMapped]
        public List<ChietKhauMacDinh_HoaDon_ChiTiet> NhanViens { get; set; }
    }
}

