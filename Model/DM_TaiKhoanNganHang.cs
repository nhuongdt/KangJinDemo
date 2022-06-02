using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_TaiKhoanNganHang")]
    public partial class DM_TaiKhoanNganHang
    {
        public DM_TaiKhoanNganHang()
        {
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NganHang { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenChuThe { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string SoTaiKhoan { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool TaiKhoanPOS { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? TrangThai { get; set; } = 1; // 0- Xóa, 1- Đang sử dụng

        public virtual DM_NganHang DM_NganHang { get; set; }
        public virtual DM_DonVi DM_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }
    }
}
