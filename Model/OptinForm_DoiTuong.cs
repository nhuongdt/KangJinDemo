using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("OptinForm_DoiTuong")]
    public partial class OptinForm_DoiTuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OptinForm_DoiTuong()
        {
            OptinForm_Link = new HashSet<OptinForm_Link>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string AnhDaiDien { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenDoiTuong { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenDoiTuong_KhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenDoiTuong_ChuCaiDau { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int GioiTinh { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySinh { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string Email { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string DiaChi { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TinhThanh { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuanHuyen { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaSoThue { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool LaCaNhan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiGioiThieu { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienPhuTrach { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        public virtual DM_TinhThanh DM_TinhThanh { get; set; }
        public virtual DM_QuanHuyen DM_QuanHuyen { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_Link> OptinForm_Link { get; set; }
    }
    public class OF_DoiTuongPRC
    {
        public Guid ID { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenOptinForm { get; set; }
        public DateTime NgayTao { get; set; }
        public int boolGioiTinh { get; set; }
        public bool boolLaCaNhan { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string MaSoThue { get; set; }
        public string LaCaNhan { get; set; }
        public string NguoiGioiThieu { get; set; }
        public string AnhDaiDien { get; set; }
        public string TenTinhThanh { get; set; }
        public string TenQuanHuyen { get; set; }
        public Guid? ID_TinhThanh { get; set; }
        public Guid? ID_QuanHuyen { get; set; }
        public int TrangThai { get; set; }
        public int TrangThaiTrung { get; set; }
        public string MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string DienThoaiKhachHang { get; set; }
        public bool? GioiTinhKhachHang { get; set; }
        public DateTime? NgaySinhKhachHang { get; set; }
        public string DinhDang_NgaySinh { get; set; }
        public string EmailKhachHang { get; set; }
        public string DiaChiKhachHang { get; set; }
        public string MaSoThueKhachHang { get; set; }
        public bool? LaCaNhanKhachHang { get; set; }
        public Guid? ID_NguoiGioiThieu { get; set; }
        public string NguoiGioiThieuKhachHang { get; set; }
    }
}
