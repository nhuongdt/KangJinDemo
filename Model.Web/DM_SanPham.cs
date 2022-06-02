using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_SanPham")]
    public partial class DM_SanPham
    {
        public DM_SanPham()
        {
            DM_DonHangChiTiet = new HashSet<DM_DonHangChiTiet>();
        }
        [Key]
        public Guid ID { get; set; }
        public string TenSanPham { get; set; }
        public string MaSanPham { get; set; }
        public string GhiChu { get; set; }
        public string Anh { get; set; }
        public string ListAnh { get; set; }
        public double GiaNhap { get; set; }
        public double GiaBan { get; set; }
        public double SoLuong { get; set; }
        public Guid ID_NhomSanPham { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public int? TrangThai { get; set; }
        public DateTime? TopHot { get; set; }
        public int? SoLuotXem { get; set; }
        public bool? QuanLyKhoHang { get; set; }
        public string LyDoKM { get; set; }
        public double GiaKM { get; set; }
        public string NhaCungCap { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
        public virtual DM_NhomSanPham DM_NhomSanPham { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DonHangChiTiet> DM_DonHangChiTiet { get; set; }
    }
}
