using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_KhuyenMai")]
    public partial class DM_KhuyenMai
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_KhuyenMai()
        {
            DM_KhuyenMai_ChiTiet = new HashSet<DM_KhuyenMai_ChiTiet>();
            DM_KhuyenMai_ApDung = new HashSet<DM_KhuyenMai_ApDung>();
            BH_HoaDon = new HashSet<BH_HoaDon>();
            BH_HoaDon_ChiTiet = new HashSet<BH_HoaDon_ChiTiet>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaKhuyenMai { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenKhuyenMai { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool TrangThai { get; set; }// 1. Kichhoat, 2.Chua apdung

        [Column(TypeName = "int")]
        public int HinhThuc { get; set; } // hình thức khuyến mại (21.HH - giam HH, 22.HH - Tang Hang, 23. HH - TangDiem, 24. HH - GiaBan theo SL Mua)

        [Column(TypeName = "int")]
        public int LoaiKhuyenMai { get; set; } // Khuyến mại theo

        [Column(TypeName = "datetime")]
        public DateTime ThoiGianBatDau { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ThoiGianKetThuc { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NgayApDung { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string ThangApDung { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string ThuApDung { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GioApDung { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int ApDungNgaySinhNhat { get; set; } // 0 - ko set, 1 - ngày, 2 - tuần, 3 - tháng

        [Column(TypeName = "bit")]
        public bool TatCaDonVi { get; set; }

        [Column(TypeName = "bit")]
        public bool TatCaDoiTuong { get; set; }

        [Column(TypeName = "bit")]
        public bool TatCaNhanVien { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ChiTiet> DM_KhuyenMai_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ApDung> DM_KhuyenMai_ApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }

        [NotMapped]
        public string TenHinhThucKM { get; set; }
    }
}
