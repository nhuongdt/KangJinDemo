using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_KyTinhCong")]
    public partial class NS_KyTinhCong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_KyTinhCong()
        {
            //NS_NgayNghiLe = new HashSet<NS_NgayNghiLe>();
            //NS_ChamCong_ChiTiet = new HashSet<NS_ChamCong_ChiTiet>();
            //NS_KyHieuCong = new HashSet<NS_KyHieuCong>();
            //NS_BangLuong = new HashSet<NS_BangLuong>();
            NS_KhenThuong = new HashSet<NS_KhenThuong>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "int")]
        public int Ky { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime TuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DenNgay { get; set; }

        [Column(TypeName = "int")]
        public int NamNhuan { get; set; } // not use

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }// 0.Xoa, 1.TaoMoi, 2.ChotKy, 3.DaApDung

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NS_NgayNghiLe> NS_NgayNghiLe { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NS_ChamCong_ChiTiet> NS_ChamCong_ChiTiet { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NS_KyHieuCong> NS_KyHieuCong { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NS_BangLuong> NS_BangLuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_KhenThuong> NS_KhenThuong { get; set; }
    }
}
