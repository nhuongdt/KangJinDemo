using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_ThietLapLuongChiTiet")]
    public partial class NS_ThietLapLuongChiTiet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_ThietLapLuongChiTiet()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_LuongPhuCap { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_CaLamViec { get; set; }

        [Column(TypeName = "float")]
        public double LuongNgayThuong { get; set; } = 0;

        [Column(TypeName = "int")]
        public int NgayThuong_LaPhanTramLuong { get; set; } = 0;

        [Column(TypeName = "int")]
        public int Thu7_LaPhanTramLuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double Thu7_GiaTri { get; set; } = 0;

        [Column(TypeName = "int")]
        public int CN_LaPhanTramLuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double ThCN_GiaTri { get; set; } = 0;

        [Column(TypeName = "int")]
        public int NgayNghi_LaPhanTramLuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double NgayNghi_GiaTri { get; set; } = 0;

        [Column(TypeName = "int")]
        public int NgayLe_LaPhanTramLuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double NgayLe_GiaTri { get; set; } = 0;

        [Column(TypeName = "int")]
        public int LaOT { get; set; }

        public virtual NS_Luong_PhuCap NS_Luong_PhuCap { get; set; }

        public virtual NS_CaLamViec NS_CaLamViec { get; set; }
    }
}
