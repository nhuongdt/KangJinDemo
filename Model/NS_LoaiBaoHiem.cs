using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_LoaiBaoHiem")]
    public partial class NS_LoaiBaoHiem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_LoaiBaoHiem ()
        {
            NS_BaoHiem = new HashSet<NS_BaoHiem>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar")]
        public string TenBaoHiem { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayApDung { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "float")]
        public double TyLeCongTy { get; set; } = 0;

        [Column(TypeName = "float")]
        public double TyLeNhanVien { get; set; } = 0;

        [Column(TypeName = "float")]
        public double Tong { get; set; } = 0;

        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BaoHiem> NS_BaoHiem { get; set; }
    }
}
