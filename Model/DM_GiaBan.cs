using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_GiaBan")]
    public partial class DM_GiaBan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_GiaBan()
        {
            DM_GiaBan_ChiTiet = new HashSet<DM_GiaBan_ChiTiet>();
            DM_GiaBan_ApDung = new HashSet<DM_GiaBan_ApDung>();
            BH_HoaDon = new HashSet<BH_HoaDon>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        public string TenGiaBan { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool ApDung { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DenNgay { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NgayTrongTuan { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string LoaiChungTuApDung { get; set; } = string.Empty;

        [Column(TypeName = "ntext")]
        public string GhiChu { get; set; } = string.Empty;

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

        [Column(TypeName = "bit")]
        public bool TatCaDoiTuong { get; set; }

        [Column(TypeName = "bit")]
        public bool TatCaDonVi { get; set; }

        [Column(TypeName = "bit")]
        public bool TatCaNhanVien { get; set; }

        [Column(TypeName = "int")]
        public int? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaBan_ChiTiet> DM_GiaBan_ChiTiet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaBan_ApDung> DM_GiaBan_ApDung { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon> BH_HoaDon { get; set; }

        [NotMapped]
        public virtual List<GiaBanChiTietBasicDTO> GiaBanChiTiet { get; set; }
    }

    public partial class GiaBanChiTietBasicDTO
    {
        public Guid ID { get; set; }
        public Guid ID_DonViQuiDoi { get; set; }
        public double GiaBan { get; set; }
    }
}
