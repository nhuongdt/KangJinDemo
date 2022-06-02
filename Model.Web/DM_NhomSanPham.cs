using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_NhomSanPham")]
    public partial class DM_NhomSanPham
    {
        public DM_NhomSanPham()
        {
            DM_SanPham = new HashSet<DM_SanPham>();
            DM_NhomSanPhamCha = new HashSet<DM_NhomSanPham>();
        }

        [Key]
        public Guid ID { get; set; }
        public string TenNhomSanPham { get; set; }
        public string MaNhomSanPham { get; set; }
        public Guid? ID_NhomCha { get; set; }
        public int ViTriHienThi { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public bool? TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }

        public virtual DM_NhomSanPham DM_NhomSanPham1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_NhomSanPham> DM_NhomSanPhamCha { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_SanPham> DM_SanPham { get; set; }
    }
}
