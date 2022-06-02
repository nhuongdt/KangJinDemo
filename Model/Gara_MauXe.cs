using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_MauXe")]
    public class Gara_MauXe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_MauXe()
        {
            Gara_DanhMucXe = new HashSet<Gara_DanhMucXe>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenMauXe { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangXe { get; set; } = Guid.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_LoaiXe { get; set; } = Guid.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = "";

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

        public virtual Gara_HangXe Gara_HangXe { get; set; }
        public virtual Gara_LoaiXe Gara_LoaiXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_DanhMucXe> Gara_DanhMucXe { get; set; }
    }
}
