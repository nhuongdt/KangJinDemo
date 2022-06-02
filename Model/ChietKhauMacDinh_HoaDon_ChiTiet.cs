using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChietKhauMacDinh_HoaDon_ChiTiet")]
    public partial class ChietKhauMacDinh_HoaDon_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ChietKhauHoaDon { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        public virtual ChietKhauMacDinh_HoaDon ChietKhauMacDinh_HoaDon { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
