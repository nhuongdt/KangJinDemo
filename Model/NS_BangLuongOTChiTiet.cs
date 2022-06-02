using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_BangLuongOTChiTiet")]
    public partial class NS_BangLuongOTChiTiet
    {
        public NS_BangLuongOTChiTiet()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_BangLuongChiTiet { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_CaLamViec { get; set; }

        [Column(TypeName = "int")]
        public int LoaiLuong { get; set; } = 1;

        [Column(TypeName = "int")]
        public int LoaiNgay { get; set; } = 1;

        [Column(TypeName = "float")]
        public double SoTien { get; set; } = 0;

        [Column(TypeName = "float")]
        public double HeSoLuongOT { get; set; } = 1;

        [Column(TypeName = "float")]
        public double ThanhTien { get; set; }

        public virtual NS_CaLamViec NS_CaLamViec { get; set; }

        public virtual NS_BangLuong_ChiTiet NS_BangLuong_ChiTiet { get; set; }
    }
}
