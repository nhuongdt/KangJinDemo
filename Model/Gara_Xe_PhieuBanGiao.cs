using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_Xe_PhieuBanGiao")]
    public class Gara_Xe_PhieuBanGiao
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_Xe_PhieuBanGiao()
        {
            Gara_Xe_NhatKyHoatDong = new HashSet<Gara_Xe_NhatKyHoatDong>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar")]
        public string MaPhieu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid IdXe { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayGiaoXe { get; set; } //Ngày giao xe cho khách hàng

        [Column(TypeName = "datetime")]
        public DateTime? NgayNhanXe { get; set; } //Ngày nhận xe về gara

        [Column(TypeName = "datetime")]
        public DateTime? NgayNhanXeDuKien { get; set; } //Ngày nhận xe dự kiến

        [Column(TypeName = "int")]
        public int SoKmBanGiao { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid IdNhanVienBanGiao { get; set; } //Id Nhân viên giao xe

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IdNhanVienTiepNhan { get; set; } //Id nhân viên tiếp nhận lại xe

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IdNhanVien { get; set; } //Nhân viên nhận xe đi sử dụng = khách hàng

        [Column(TypeName = "uniqueidentifier")]
        public Guid? IdKhachHang { get; set; } //Khách hàng nhận xe sử dụng

        [Column(TypeName = "int")]
        public int LaNhanVien { get; set; } = 0; //0 - khách hàng, 1- Nhân viên

        [Column(TypeName = "nvarchar")]
        public string GhiChuBanGiao { get; set; } = "";

        [Column(TypeName = "nvarchar")]
        public string GhiChuTiepNhan { get; set; } = "";

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; //0- Xóa, 1- Đang hoạt động, 2- Hoàn thành

        [Column(TypeName = "datetime")]
        public DateTime NgayTaoBanGiao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySuaBanGiao { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiTaoBanGiao { get; set; } = "";

        [Column(TypeName = "nvarchar")]
        public string NguoiSuaBanGiao { get; set; } = "";

        [Column(TypeName = "datetime")]
        public DateTime? NgayTaoTiepNhan { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySuaTiepNhan { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiTaoTiepNhan { get; set; } = "";

        [Column(TypeName = "nvarchar")]
        public string NguoiSuaTiepNhan { get; set; } = "";

        public virtual Gara_DanhMucXe Gara_DanhMucXe { get; set; }
        public virtual NS_NhanVien NS_NhanVienBanGiao { get; set; }
        public virtual NS_NhanVien NS_NhanVienTiepNhan { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_NhatKyHoatDong> Gara_Xe_NhatKyHoatDong { get; set; }
    }
}
