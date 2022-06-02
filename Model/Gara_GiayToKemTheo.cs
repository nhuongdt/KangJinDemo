using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_GiayToKemTheo")]
    public class Gara_GiayToKemTheo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_GiayToKemTheo()
        { }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_PhieuTiepNhan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TieuDe { get; set; }

        [Column(TypeName = "int")]
        public int SoLuong { get; set; } = 1;

        [Column(TypeName = "nvarchar(max)")]
        public string FileDinhKem { get; set; } = "";

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; //0 - xóa

        public virtual Gara_PhieuTiepNhan Gara_PhieuTiepNhan { get; set; }
    }
}
