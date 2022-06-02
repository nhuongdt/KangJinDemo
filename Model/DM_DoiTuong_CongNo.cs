using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_DoiTuong_CongNo")]
    public partial class DM_DoiTuong_CongNo
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DoiTuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "float")]
        public double NoHienTai { get; set; }

        [Column(TypeName = "float")]
        public double TongBan { get; set; }

        [Column(TypeName = "float")]
        public double TongTra { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
        
        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
