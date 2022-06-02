using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_DanhMucXe")]
    public class Gara_DanhMucXe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_DanhMucXe()
        {
            Gara_PhieuTiepNhan = new HashSet<Gara_PhieuTiepNhan>();
            DM_HangHoa = new HashSet<DM_HangHoa>();
            Gara_LichBaoDuong = new HashSet<Gara_LichBaoDuong>();
            BH_HoaDon = new HashSet<BH_HoaDon>();
            CSKH_DatLich = new HashSet<CSKH_DatLich>();
            Gara_Xe_PhieuBanGiao = new HashSet<Gara_Xe_PhieuBanGiao>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(30)]
        [Column(TypeName = "nvarchar")]
        public string BienSo { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhachHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_MauXe { get; set; } = Guid.Empty;

        [Column(TypeName = "int")]
        public int NamSanXuat { get; set; } = 0; //đời xe

        [Column(TypeName = "nvarchar(max)")]
        public string SoMay { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")]
        public string SoKhung { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")]
        public string MauSon { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")]
        public string DungTich { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")]
        public string HopSo { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; //0 - Xóa

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = "";

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "int")]
        public int? LoaiApDung { get; set; } = 0; // 0 - Sửa chữa , 1- Xuất bán

        [Column(TypeName = "int")]
        public int? NguoiSoHuu { get; set; } = 0; //0 or null - Khách hàng, 1 - Gara

        public virtual Gara_MauXe Gara_MauXe { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_PhieuTiepNhan> Gara_PhieuTiepNhan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa> DM_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_LichBaoDuong> Gara_LichBaoDuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CSKH_DatLich> CSKH_DatLich { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_Xe_PhieuBanGiao> Gara_Xe_PhieuBanGiao { get; set; }
    }
}
