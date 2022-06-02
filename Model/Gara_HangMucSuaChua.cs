using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_HangMucSuaChua")]
    public class Gara_HangMucSuaChua
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_HangMucSuaChua()
        { }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_PhieuTiepNhan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenHangMuc { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TinhTrang { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")]
        public string PhuongAnSuaChua { get; set; } = "";

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; //0 - xóa

        [Column(TypeName = "nvarchar(max)")]
        public string Anh { get; set; } = "";

        public virtual Gara_PhieuTiepNhan Gara_PhieuTiepNhan { get; set; }
    }
}
