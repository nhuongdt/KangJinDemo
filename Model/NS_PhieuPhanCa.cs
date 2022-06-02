using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_PhieuPhanCa")]
    public partial class NS_PhieuPhanCa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_PhieuPhanCa()
        {
            NS_PhieuPhanCa_CaLamViec = new HashSet<NS_PhieuPhanCa_CaLamViec>();
            NS_PhieuPhanCa_NhanVien = new HashSet<NS_PhieuPhanCa_NhanVien>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaPhieu { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime TuNgay { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DenNgay { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVienTao { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }// 2.datao bangluong

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int LoaiPhanCa { get; set; }// 1.ca tuần, 2.ca sáng, 3. ca cố định (2.not use)

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }
        public virtual DM_DonVi DM_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhieuPhanCa_CaLamViec> NS_PhieuPhanCa_CaLamViec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhieuPhanCa_NhanVien> NS_PhieuPhanCa_NhanVien { get; set; }
    }
}
