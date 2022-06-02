using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm_TruongThongTin")]
    public partial class OptinForm_TruongThongTin
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OptinForm_TruongThongTin()
        {
            OptinForm_ThietLap = new HashSet<OptinForm_ThietLap>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenTruongThongTin { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int LoaiTruongThongTin { get; set; }

        [Column(TypeName = "int")]
        public int STT { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_ThietLap> OptinForm_ThietLap { get; set; }
    }
    public class OF_TruongThongTinPROC
    {
        public int STT { get; set; }
        public Guid ID { get; set; }
        public string TenTruongThongTin { get; set; }
        public int checkSuDung { get; set; }
        public int checkBatBuoc { get; set; }
        public string GoiY { get; set; }
        public string NoiDungThongBao { get; set; }
        public string WebDieuHuong { get; set; }
        public string ButtonName { get; set; }
        public string NoiDungHieuLuc { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool TrangThaiThoiGian { get; set; }
    }
    public class OF_linkKhachHangDangKyPROC
    {
        public string Link { get; set; }
        public double SoLuotDangKy { get; set; }
    }
    public class OF_BieuDoDangKyPROC
    {
        public string ThoiGian { get; set; }
        public double SoLuotDangKy { get; set; }
    }
    public class OF_OptinFormPROC
    {
        public Guid ID { get; set; }
        public string TenOptinForm { get; set; }
        public string MaNhung { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NoiDung { get; set; }
        public double SoLuotTruyCap { get; set; }
        public double SoLuotDangKy { get; set; }
        public int TrangThai { get; set; }
        public int LoaiOptinForm { get; set; }
        public string GhiChu { get; set; }
    }
}
