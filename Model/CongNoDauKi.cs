using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("CongNoDauKi")]
    public partial class CongNoDauKi
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DoiTuong { get; set; }

        [Column(TypeName = "int")]
        public int NamHachToan { get; set; }

        [Column(TypeName = "bit")]
        public bool LaPhaiThu { get; set; }

        [Column(TypeName = "float")]
        public double SoTien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TienTe { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }
    }
}
