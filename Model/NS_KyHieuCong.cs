using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_KyHieuCong")]
    public partial class NS_KyHieuCong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_KyHieuCong()
        {
            
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string KyHieu { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string MoTa { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TrangThaiCong { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double CongQuyDoi { get; set; }

        [Column(TypeName = "bit")]
        public bool LayGioMacDinh { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioVao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioRa { get; set; }

        //[Column(TypeName = "uniqueidentifier")]
        //public Guid? ID_KyTinhCong { get; set; }

        //public virtual NS_KyTinhCong NS_KyTinhCong { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
