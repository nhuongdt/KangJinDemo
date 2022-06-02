using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm_NgayLamViec")]
    public partial class OptinForm_NgayLamViec
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_OptinForm { get; set; }

        [Column(TypeName = "int")]
        public int NgayLamViec { get; set; }

        [Column(TypeName = "bit")]
        public bool TrangThaiLamViec { get; set; }

        [Column(TypeName = "int")]
        public int ThoiGianBatDau { get; set; }

        [Column(TypeName = "int")]
        public int ThoiGianKetThuc { get; set; }

        public virtual OptinForm OptinForm { get; set; }
    }
}
