using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_LoHang")]
    public partial class DM_LoHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_LoHang()
        {
            BH_HoaDon_ChiTiet = new HashSet<BH_HoaDon_ChiTiet>();
            Kho_HoaDon_ChiTiet = new HashSet<Kho_HoaDon_ChiTiet>();
            Kho_TonKhoKhoiTao = new HashSet<Kho_TonKhoKhoiTao>();
            ChotSo_HangHoa = new HashSet<ChotSo_HangHoa>();
            DM_GiaVon = new HashSet<DM_GiaVon>();
            DM_HangHoa_TonKho = new HashSet<DM_HangHoa_TonKho>();
            DinhLuongDichVu = new HashSet<DinhLuongDichVu>();
            NhomHang_ChiTietSanPhamHoTro = new HashSet<NhomHang_ChiTietSanPhamHoTro>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HangHoa { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaLoHang { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySanXuat { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayHetHan { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenLoHang { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? TrangThai { get; set; } = true; /*1. Đang sử dụng, 2. Không sử dụng*/

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon_ChiTiet> Kho_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_TonKhoKhoiTao> Kho_TonKhoKhoiTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChotSo_HangHoa> ChotSo_HangHoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaVon> DM_GiaVon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_HangHoa_TonKho> DM_HangHoa_TonKho { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DinhLuongDichVu> DinhLuongDichVu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhomHang_ChiTietSanPhamHoTro> NhomHang_ChiTietSanPhamHoTro { get; set; }
    }

    // nhuongdt
    public class SP_DM_LoHang
    {
        public Guid ID { get; set; } // ID_LoHang
        public Guid ID_HangHoa { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double? TonKho { get; set; }
        public bool? TrangThai { get; set; }
    }

    //tinhlv
    public class ListDM_LoHangDTO
    {
        public Guid ID { get; set; } // ID_LoHang
        public Guid ID_HangHoa { get; set; }
        public Guid? ID_DonViQuiDoi { get; set; }
        public string MaLoHang { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double? TonKho { get; set; }
    }

    public class ListDMLoHetHan
    {
        public Guid ID_LoHang { get; set; } // ID_LoHang
        public Guid ID_DonVi { get; set; }
        public string MaLoHang { get; set; }
        public string MaHangHoa { get; set; }
        public DateTime? NgaySanXuat { get; set; }
        public DateTime? NgayHetHan { get; set; }
        public double? TonKho { get; set; }
    }
}
