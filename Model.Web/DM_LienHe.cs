using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_LienHe")]
    public partial class DM_LienHe
    {
        [Key]
        public Guid ID { get; set; }
        public string TenNguoiLienHe { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string GhiChu { get; set; }
        public string SoDienThoai { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTao { get; set; }
    }
}
