using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_Xe_NhatKyHoatDong")]
    public class Gara_Xe_NhatKyHoatDong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_Xe_NhatKyHoatDong()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IdPhieuBanGiao { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IdNhanVienThucHien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IdKhachHang { get; set; }

        [Column(TypeName = "int")]
        public int LaNhanVien { get; set; } = 0; //0- Khách hàng, 1- Nhân viên

        [Column(TypeName = "datetime")]
        public DateTime ThoiGianHoatDong { get; set; }

        [Column(TypeName = "float")]
        public double SoGioHoatDong { get; set; } = 0;

        [Column(TypeName = "int")]
        public int SoKmHienTai { get; set; } = 0;

        [Column(TypeName = "nvarchar")]
        public string GhiChu { get; set; } = "";

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; //0- Xóa, 1- Đang sử dụng

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = "";

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = "";

        public virtual Gara_Xe_PhieuBanGiao Gara_Xe_PhieuBanGiao { get; set; }
        public virtual NS_NhanVien NS_NhanVien_NVThucHien { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
    }
}
