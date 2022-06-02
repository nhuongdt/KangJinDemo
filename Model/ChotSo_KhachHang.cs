using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChotSo_KhachHang")]
    public partial class ChotSo_KhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChotSo_KhachHang()
        {

        }
        [Column(TypeName = "uniqueidentifier")]
        [Key]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_KhachHang { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayChotSo { get; set; }

        [Column(TypeName = "float")]
        public double CongNo { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
    }
}
