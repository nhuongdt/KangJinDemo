using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_LoaiKhenThuong")]
    public partial class NS_LoaiKhenThuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_LoaiKhenThuong()
        {
            NS_KhenThuong = new HashSet<NS_KhenThuong>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar")]
        public string TenLoaiKhenThuong { get; set; }

        [Column(TypeName = "int")]
        public int PhanLoai { get; set; } = 1; //1. Kỷ luât, 2. Khen thưởng

        [Column(TypeName = "nvarchar(max)")]
        public string MoTa { get; set; }

        [Column(TypeName = "float")]
        public double TienThuong { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime TuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DenNgay { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_KhenThuong> NS_KhenThuong { get; set; }
    }
}
