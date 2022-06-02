using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_ThongBao_CaiDat")]
    public partial class HT_ThongBao_CaiDat
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NguoiDung { get; set; }

        [Column(TypeName = "bit")]
        public bool NhacSinhNhat { get; set; }

        [Column(TypeName = "bit")]
        public bool NhacCongNo { get; set; }

        [Column(TypeName = "bit")]
        public bool NhacTonKho { get; set; }

        [Column(TypeName = "bit")]
        public bool NhacDieuChuyen { get; set; }

        [Column(TypeName = "bit")]
        public bool? NhacLoHang { get; set; } = false;

        [Column(TypeName = "int")]
        public int? LichHen { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? GoiDichVu { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? TheGiaTri { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? BaoDuongXe { get; set; } = 0;

        public virtual HT_NguoiDung HT_NguoiDung { get; set; }
    }
}
