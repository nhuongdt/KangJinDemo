using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_NguoiDung_Nhom")]
    public partial class HT_NguoiDung_Nhom
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDNhomNguoiDung { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDNguoiDung { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        public virtual HT_NguoiDung HT_NguoiDung { get; set; }

        public virtual HT_NhomNguoiDung HT_NhomNguoiDung { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
