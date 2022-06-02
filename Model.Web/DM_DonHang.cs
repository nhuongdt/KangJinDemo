using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_DonHang")]
    public partial class DM_DonHang
    {
        public DM_DonHang()
        {
            DM_DonHangChiTiet = new HashSet<DM_DonHangChiTiet>();
        }
        [Key]
        public Guid ID { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public int? TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public double DaThanhToan { get; set; }
        public double ConNo { get; set; }
        public string TenNguoiNhan { get; set; }
        public string SoDienThoaiNguoiNhan { get; set; }
        public string DiaChiNguoiNhan { get; set; }
        public string EmailNguoiNhan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_DonHangChiTiet> DM_DonHangChiTiet { get; set; }
    }
}
