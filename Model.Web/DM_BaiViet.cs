using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_BaiViet")]
    public partial class DM_BaiViet
    {
        public DM_BaiViet()
        {
            
        }

        [Key]
        public int ID { get; set; }
        public string TenBaiViet { get; set; }
        public string NoiDung { get; set; }
        public string Anh { get; set; }
        public int ID_NhomBaiViet { get; set; }
        public string MetaDescriptions { get; set; }
        public string MetaTitle { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public int? ThuTuHienThi { get; set; }
        public bool? TrangThai { get; set; }
        public string Link { get; set; }
        public int? LuotXem { get; set; }
        public DateTime? NgayDangBai { get; set; }
        public string Mota { get; set; }

        public virtual DM_NhomBaiViet DM_NhomBaiViet { get; set; }
    }
}
