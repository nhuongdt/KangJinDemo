using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("BH_HoaDon_Anh")]
    public partial class BH_HoaDon_Anh
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BH_HoaDon_Anh()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IdHoaDon { get; set; }

        [Column(TypeName = "nvarchar")]
        public string URLAnh { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        public virtual BH_HoaDon BH_HoaDon { get; set; }
    }
}
