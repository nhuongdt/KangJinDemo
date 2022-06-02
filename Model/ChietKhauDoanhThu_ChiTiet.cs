using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChietKhauDoanhThu_ChiTiet")]
    public partial class ChietKhauDoanhThu_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_ChietKhauDoanhThu { get; set; }

        [Column(TypeName = "float")]
        public double DoanhThuTu { get; set; }

        [Column(TypeName = "float")]
        public double DoanhThuDen { get; set; }

        [Column(TypeName = "float")]
        public double GiaTriChietKhau { get; set; }

        [Column(TypeName = "int")]
        public int LaPhanTram { get; set; }

        public virtual ChietKhauDoanhThu ChietKhauDoanhThu { get; set; }
    }
}
