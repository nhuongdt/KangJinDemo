using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("HT_NhomNguoiDung_Quyen")]
    public partial class HT_NhomNguoiDung_Quyen
    {
        [Key]
        public Guid ID { get; set; }
        public Guid ID_NhomNguoiDung { get; set; }
        public string MaQuyen { get; set; }

        public virtual HT_NhomNguoiDung HT_NhomNguoiDung { get; set; }
        public virtual HT_Quyen HT_Quyen { get; set; }
    }
}
