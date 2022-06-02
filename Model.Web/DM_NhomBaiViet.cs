using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DM_NhomBaiViet")]
    public partial class DM_NhomBaiViet
    {
        public DM_NhomBaiViet()
        {
            DM_BaiViet = new HashSet<DM_BaiViet>();
            DM_NhomBaiVietCha = new HashSet<DM_NhomBaiViet>();
            DM_TuyenDung = new HashSet<DM_TuyenDung>();
        }

        [Key]
        public int ID { get; set; }
        public string TenNhomBaiViet { get; set; }
        public int LoaiNhomBaiViet { get; set; }
        public string GhiChu { get; set; }
        public int? ID_NhomCha { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public string Link { get; set; }
        public bool? TrangThai { get; set; }

        public virtual DM_NhomBaiViet DM_NhomBaiViet1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_BaiViet> DM_BaiViet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_NhomBaiViet> DM_NhomBaiVietCha { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DM_TuyenDung> DM_TuyenDung { get; set; }
    }
}
