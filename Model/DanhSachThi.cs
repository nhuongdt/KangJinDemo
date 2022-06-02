using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DanhSachThi")]
    public partial class DanhSachThi
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string Ma { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Ten { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_MonThi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_GiaoVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TrongTai { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGianBatDau { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGianKetThuc { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }
    }
}
