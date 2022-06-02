using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_CongViec")]
    public partial class NS_CongViec
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_LoaiCongViec { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhachHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LienHe { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ThoiGianTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGianDen { get; set; }

        [Column(TypeName = "int")]
        public int NhacTruoc { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienChiaSe { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDung { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGianLienHeLai { get; set; }

        [Column(TypeName = "int")]
        public int NhacTruocLienHeLai { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienQuanLy { get; set; } //foreign key: NS_NhanVien. nhân viên quản lý phiếu tư vấn, ... Mặc định nhân viên tạo là nhân viên quản lý

        [Column(TypeName = "nvarchar(max)")]
        public string KetQuaCongViec { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string LyDoHenLai { get; set; } = string.Empty;

        public virtual NS_CongViec_PhanLoai NS_CongViec_PhanLoai { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
        public virtual DM_LienHe DM_LienHe { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
        public virtual NS_NhanVien NS_NhanVienQuanLy { get; set; }
        public virtual DM_DonVi DM_DonVi { get; set; }
    }
}
