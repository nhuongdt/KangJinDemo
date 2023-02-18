using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Quy_HoaDon")]
    public partial class Quy_HoaDon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Quy_HoaDon()
        {
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
            BH_NhanVienThucHien = new HashSet<BH_NhanVienThucHien>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaHoaDon { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayLapHoaDon { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NgoaiTe { get; set; }

        [StringLength(250)]
        [Column(TypeName = "nvarchar")]
        public string NguoiNopTien { get; set; } = string.Empty;
        
        [Column(TypeName = "nvarchar")]
        public string NoiDungThu { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double TongTienThu { get; set; }

        [Column(TypeName = "bit")]
        public bool ThuCuaNhieuDoiTuong { get; set; } = false; // muontam: la phieutamung luong

        [Column(TypeName = "bit")]
        public bool? HachToanKinhDoanh { get; set; } = true;

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? LoaiHoaDon { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? TrangThai { get; set; } = true; // false: Huy

        [Column(TypeName = "int")]
        public int? PhieuDieuChinhCongNo { get; set; } = 0;// 1.phieu dieuchinh congno, 2.naptiencoc, 3.khong butru cngno

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual DM_LoaiChungTu DM_LoaiChungTu { get; set; }

        public virtual DM_TienTe DM_TienTe { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_NhanVienThucHien> BH_NhanVienThucHien { get; set; }

        [NotMapped]
       public string TenNhanVien { get; set; }

        [NotMapped]
        public string NoiDungThuChi { get; set; }
        [NotMapped]
        public string PhuongThucTT { get; set; }
        [NotMapped]
        public double TienMat { get; set; }
        [NotMapped]
        public double TienGui { get; set; }

        [NotMapped]
        public string TenChiNhanh { get; set; }

        [NotMapped]
        public Guid? ID_DoiTuong { get; set; }// used to check ID_DoiTuong default
    }
}
