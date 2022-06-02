using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_NhomDoiTuong")]
    public partial class DM_NhomDoiTuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_NhomDoiTuong()
        {
            DM_DoiTuong = new HashSet<DM_DoiTuong>();
            NhomDoiTuong_DonVi = new HashSet<NhomDoiTuong_DonVi>();
            DM_GiaBan_ApDung = new HashSet<DM_GiaBan_ApDung>();
            DM_KhuyenMai_ApDung = new HashSet<DM_KhuyenMai_ApDung>();
            HT_CauHinh_TichDiemApDung = new HashSet<HT_CauHinh_TichDiemApDung>();
            DM_NhomDoiTuong_ChiTiet = new HashSet<DM_NhomDoiTuong_ChiTiet>();
            DM_DoiTuong_Nhom = new HashSet<DM_DoiTuong_Nhom>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int LoaiDoiTuong { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaNhomDoiTuong { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar")]
        public string TenNhomDoiTuong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenNhomDoiTuong_KhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenNhomDoiTuong_KyTuDau { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? GiamGia { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? GiamGiaTheoPhanTram { get; set; } = true;

        [Column(TypeName = "nvarchar(max)")]
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
        public bool? TrangThai { get; set; } = true; // true/null: dang hoat dong, false: da xoa

        [Column(TypeName = "bit")]
        public bool? TuDongCapNhat { get; set; } = false;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong> DM_DoiTuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NhomDoiTuong_DonVi> NhomDoiTuong_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_GiaBan_ApDung> DM_GiaBan_ApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_KhuyenMai_ApDung> DM_KhuyenMai_ApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_CauHinh_TichDiemApDung> HT_CauHinh_TichDiemApDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_NhomDoiTuong_ChiTiet> DM_NhomDoiTuong_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DoiTuong_Nhom> DM_DoiTuong_Nhom { get; set; } 

        [NotMapped]
        public string Text_Search
        {
            get { return CommonStatic.ConvertToUnSign(TenNhomDoiTuong) + " " + CommonStatic.GetCharsStart(TenNhomDoiTuong); }
        }
    }
}
