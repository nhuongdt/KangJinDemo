using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("DS_HoSoUngTuyen")]
    public partial class DS_HoSoUngTuyen
    {
        public DS_HoSoUngTuyen()
        {
            DS_FileDinhKems = new HashSet<DS_FileDinhKem>();
        }
        [Key]
        public int ID { get; set; }
        
        public int ID_TuyenDung { get; set; }

        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        public DateTime NgayTao { get; set; }

        public bool? GioiTinh { get; set; }

        public string Email { get; set; }

        public string SoDienThoai { get; set; }

        public string DiaChi { get; set; }

        public string TruongTotNghiep { get; set; }

        public string HeDaoTao { get; set; }

        public string ChuyenNganh { get; set; }

        public string KyNang { get; set; }

        public int TrangThai { get; set; }

        public virtual DM_TuyenDung DM_TuyenDung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DS_FileDinhKem> DS_FileDinhKems { get; set; }

    }
}
