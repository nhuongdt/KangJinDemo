using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DanhSachThi_ChiTiet")]
    public partial class DanhSachThi_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DanhSachThi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ThiSinh { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaDuThi { get; set; }
    }
}
