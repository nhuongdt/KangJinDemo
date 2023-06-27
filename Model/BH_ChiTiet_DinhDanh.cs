using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("BH_ChiTiet_DinhDanh")]
    public partial class BH_ChiTiet_DinhDanh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BH_ChiTiet_DinhDanh()
        {
            
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column(TypeName = "int")]
        public int MaDinhDanh { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IdHoaDonChiTiet { get; set; }

        public virtual BH_HoaDon_ChiTiet BH_HoaDon_ChiTiet { get; set; }
    }
}
