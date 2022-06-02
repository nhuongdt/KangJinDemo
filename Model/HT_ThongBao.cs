using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_ThongBao")]
    public partial class HT_ThongBao
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "int")]
        public int LoaiThongBao { get; set; }// 0. Tồn kho, 1. Điều chuyển, 2. Lô hàng, 3. Sinh nhat KH, 4. Bảo dưỡng

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDungThongBao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiDungDaDoc { get; set; } = string.Empty;

        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
