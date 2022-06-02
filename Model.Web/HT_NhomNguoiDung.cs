using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("HT_NhomNguoiDung")]
    public partial class HT_NhomNguoiDung
    {
        public HT_NhomNguoiDung()
        {
            HT_NguoiDung = new HashSet<HT_NguoiDung>();
            HT_NhomNguoiDung_Quyen = new HashSet<HT_NhomNguoiDung_Quyen>();
        }

        [Key]
        public Guid ID { get; set; }
        public string TenNhomNguoiDung { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgaySua { get; set; }
        public string NguoiSua { get; set; }
        public bool TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NguoiDung> HT_NguoiDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NhomNguoiDung_Quyen> HT_NhomNguoiDung_Quyen { get; set; }
    }
}
