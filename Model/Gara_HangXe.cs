using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_HangXe")]
    public class Gara_HangXe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_HangXe()
        {
            Gara_MauXe = new HashSet<Gara_MauXe>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaHangXe { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenHangXe { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Logo { get; set; } = "";

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; //0- Xóa

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = "";

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_MauXe> Gara_MauXe { get; set; }
    }
}
