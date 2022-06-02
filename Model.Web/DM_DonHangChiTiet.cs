using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_DonHangChiTiet")]
    public partial class DM_DonHangChiTiet
    {
        [Key]
        public Guid ID { get; set; }
        public Guid ID_DonHang { get; set; }
        public Guid ID_SanPham { get; set; }
        public double SoLuong { get; set; }
        public double DonGia { get; set; }
        public string GhiChu { get; set; }

        public virtual DM_DonHang DM_DonHang { get; set; }
        public virtual DM_SanPham DM_SanPham { get; set; }
    }
}
