using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_Anh_Slider")]
    public partial class DM_Anh_Slider
    {
        [Key]
        public Guid ID { get; set; }
        public string Text { get; set; }
        public string Link { get; set; }
        public int? ThuTuHienThi { get; set; }
        public string Mota { get; set; }
        public bool? TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiSua { get; set; }
        public DateTime? NgaySua { get; set; }
    }
}
