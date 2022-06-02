using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChietKhauDoanhThu_NhanVien")]
    public partial class ChietKhauDoanhThu_NhanVien
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ChietKhauDoanhThu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        public virtual ChietKhauDoanhThu ChietKhauDoanhThu { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
    }
}
