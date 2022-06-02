using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_QuaTrinhCongTac")]
    public partial class NS_QuaTrinhCongTac
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_PhongBan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChucVu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayApDung { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayHetHan { get; set; }

        [Column(TypeName = "bit")]
        public bool LaChucVuHienThoi { get; set; }

        [Column(TypeName = "bit")]
        public bool LaDonViHienThoi { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayLap { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiLap { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual DM_ChucVu DM_ChucVu { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual NS_PhongBan NS_PhongBan { get; set; }
    }
}
