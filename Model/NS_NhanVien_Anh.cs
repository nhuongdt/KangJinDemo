using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_NhanVien_Anh")]
    public partial class NS_NhanVien_Anh
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int SoThuTu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string URLAnh { get; set; } = string.Empty;

        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
