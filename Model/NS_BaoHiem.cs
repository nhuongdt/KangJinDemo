using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_BaoHiem")]
    public partial class NS_BaoHiem
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoaiBaoHiem { get; set; }

        [Column(TypeName = "int")]
        public int LoaiBaoHiem { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiBaoHiem { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SoBaoHiem { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayCap { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayHetHan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual NS_LoaiBaoHiem NS_LoaiBaoHiem { get; set; }
    }
}
