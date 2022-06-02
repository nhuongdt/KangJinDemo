using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Gara_PhieuTiepNhan")]
    public class Gara_PhieuTiepNhan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gara_PhieuTiepNhan()
        {
            Gara_HangMucSuaChua = new HashSet<Gara_HangMucSuaChua>();
            Gara_GiayToKemTheo = new HashSet<Gara_GiayToKemTheo>();
            BH_HoaDon = new HashSet<BH_HoaDon>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaPhieuTiepNhan { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayVaoXuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; } //Nhân viên lập phiếu

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_CoVanDichVu { get; set; } //Nhân viên

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_Xe { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_KhachHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }
        
        [Column(TypeName = "datetime")]
        public DateTime? NgayXuatXuongDuKien { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenLienHe { get; set; } = "";

        [StringLength(20)]
        [Column(TypeName = "nvarchar")]
        public string SoDienThoaiLienHe { get; set; } = "";

        [Column(TypeName = "int")]
        public int SoKmVao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = ""; // Ghi chú phiếu tiếp nhận

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 1; // 0.Xóa, 1.Đang sửa (đã tiếp nhận và chưa click hoàn thành), 2.Đã sửa xong, 3. Đã xuất xưởng

        [Column(TypeName = "int")]
        public int SoKmRa { get; set; } //Mặc định = số km vào

        [Column(TypeName = "datetime")]
        public DateTime? NgayXuatXuong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string XuatXuong_GhiChu { get; set; } = ""; //Ghi chú phiếu xuất xưởng

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = "";

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_BaoHiem { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiLienHeBH { get; set; } = "";

        [StringLength(20)]
        [Column(TypeName = "nvarchar")]
        public string SoDienThoaiLienHeBH { get; set; } = "";

        public virtual NS_NhanVien NS_NhanVien { get; set; }
        public virtual NS_NhanVien NS_NhanVienCoVan { get; set; }
        public virtual Gara_DanhMucXe Gara_DanhMucXe { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual DM_DoiTuong DM_DoiTuongBaoHiem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_HangMucSuaChua> Gara_HangMucSuaChua { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_GiayToKemTheo> Gara_GiayToKemTheo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }
    }
}
