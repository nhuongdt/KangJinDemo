using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm")]
    public partial class OptinForm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OptinForm()
        {
            OptinForm_ThietLap = new HashSet<OptinForm_ThietLap>();
            OptinForm_ThietLapThongBao = new HashSet<OptinForm_ThietLapThongBao>();
            OptinForm_NgayNghiLe = new HashSet<OptinForm_NgayNghiLe>();
            OptinForm_NgayLamViec = new HashSet<OptinForm_NgayLamViec>();
            OptinForm_Link = new HashSet<OptinForm_Link>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenOptinForm { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenOptinForm_KhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenOptinForm_ChuCaiDau { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int LoaiOptinForm { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDung { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string MaNhung { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime TuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DenNgay { get; set; }

        [Column(TypeName = "bit")]
        public bool TrangThaiThoiGian { get; set; }

        [Column(TypeName = "int")]
        public int LoaiThoiGian { get; set; }

        [Column(TypeName = "int")]
        public int KhoangCachThoiGian { get; set; }

        [Column(TypeName = "float")]
        public double SoLuotTruyCap { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_ThietLap> OptinForm_ThietLap { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_ThietLapThongBao> OptinForm_ThietLapThongBao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_NgayNghiLe> OptinForm_NgayNghiLe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_NgayLamViec> OptinForm_NgayLamViec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_Link> OptinForm_Link { get; set; }
    }
}
