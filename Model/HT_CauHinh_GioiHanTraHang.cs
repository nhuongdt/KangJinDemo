using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_CauHinh_GioiHanTraHang")]
    public partial class HT_CauHinh_GioiHanTraHang
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_CauHinh { get; set; }

        [Column(TypeName = "int")]
        public int SoNgayGioiHan { get; set; }

        [Column(TypeName = "bit")]
        public bool ChoPhepTraHang { get; set; } //true: Cảnh báo khi trả hàng; false: Không cho phép trả hàng

        public virtual HT_CauHinhPhanMem HT_CauHinhPhanMem { get; set; }
    }
}
