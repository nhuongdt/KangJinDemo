using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HeThong_SMS")]
    public partial class HeThong_SMS
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NguoiGui { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhachHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int SoTinGui { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDung { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime ThoiGianGui { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 0;

        [Column(TypeName = "float")]
        public double GiaTien { get; set; } = 0;

        [Column(TypeName = "int")]
        public int LoaiTinNhan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string IDTinNhan { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_HoaDon { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual HT_NguoiDung HT_NguoiDung { get; set; }

        public virtual BH_HoaDon BH_HoaDon { get; set; }
    }
}
