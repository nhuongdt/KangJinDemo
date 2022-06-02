using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm_Link")]
    public partial class OptinForm_Link
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_OptinForm { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DoiTuongOptinForm { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChamSocKhachHang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Link { get; set; } = string.Empty;

        public virtual OptinForm OptinForm { get; set; }
        public virtual OptinForm_DoiTuong OptinForm_DoiTuong { get; set; }
        public virtual ChamSocKhachHang ChamSocKhachHang { get; set; }
    }
}
