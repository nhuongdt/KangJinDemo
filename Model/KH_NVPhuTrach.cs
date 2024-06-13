using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class KH_NVPhuTrach
    {
        [Key]
        public Guid ID { get; set; }
        public Guid ID_KhachHang { get; set; }
        [ForeignKey("ID_KhachHang")]
        public DM_DoiTuong DM_DoiTuong { get; set; }
        public Guid ID_NhanVienPhuTrach { get; set; }
        [ForeignKey("ID_NhanVienPhuTrach")]
        public NS_NhanVien NS_NhanVien { get; set; }
        public byte? VaiTro { get; set; }// 1. Tư vấn chính, 2. Tư vấn phụ, 3. Telesales, 4. NV chăm sóc đặc biệt
    }
}
