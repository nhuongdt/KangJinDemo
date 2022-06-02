using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_BangLuong")]
    public partial class NS_BangLuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_BangLuong()
        {
            NS_BangLuong_ChiTiet = new HashSet<NS_BangLuong_ChiTiet>();
            //NS_NgayNghiLe = new HashSet<NS_NgayNghiLe>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaBangLuong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenBangLuong { get; set; }

        //[Column(TypeName = "uniqueidentifier")]
        //public Guid ID_KyTinhCong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienDuyet { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DenNgay { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }// 0.Xoa, 1.LuuTam, 2. Cần tính lại bảng lương (do công thuộc bảng lương tạm, nhưng lại bị cập nhật )
                                        // 3.DaChotLuong (DaDuyet), 4.DaThanhToan, 5.Huy

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayThanhToanLuong { get; set; }

        [Column(TypeName = "bit")]
        public bool SuDungHRM { get; set; } = false;

        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; } 

        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; } 

        [Column(TypeName = "bit")]
        public bool LaBangLuongBoSung { get; set; } = false;

        public virtual NS_NhanVien NS_NhanVienDuyet { get; set; }

        //public virtual NS_KyTinhCong NS_KyTinhCong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BangLuong_ChiTiet> NS_BangLuong_ChiTiet { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NS_NgayNghiLe> NS_NgayNghiLe { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        //[NotMapped]
        //public DateTime TuNgay { get; set; } // bang luong tu ngay - den ngay
        //[NotMapped]
        //public DateTime DenNgay { get; set; }
    }
}
