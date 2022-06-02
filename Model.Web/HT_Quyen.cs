using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Web
{
    [Table("HT_Quyen")]
    public partial class HT_Quyen
    {
        public HT_Quyen()
        {
            HT_NhomNguoiDung_Quyen = new HashSet<HT_NhomNguoiDung_Quyen>();
        }
        [Key]
        public string MaQuyen { get; set; }
        public string TenQuyen { get; set; }
        public string MoTa { get; set; }
        public bool DuocSuDung { get; set; }
        public string QuyenCha { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_NhomNguoiDung_Quyen> HT_NhomNguoiDung_Quyen { get; set; }
    }
}
