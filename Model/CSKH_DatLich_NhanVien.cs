using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("CSKH_DatLich_NhanVien")]
    public partial class CSKH_DatLich_NhanVien
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDDatLich { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IDNhanVien { get; set; }

        public virtual CSKH_DatLich CSKH_DatLich { get; set;}
        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
