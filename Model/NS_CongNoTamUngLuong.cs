using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_CongNoTamUngLuong")]
    public class NS_CongNoTamUngLuong
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; } = Guid.NewGuid();

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "float")]
        public double CongNo { get; set; } = 0;

        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
