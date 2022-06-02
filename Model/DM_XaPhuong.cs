using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_XaPhuong")]
    public partial class DM_XaPhuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DM_XaPhuong()
        {
            NS_NhanVienTT = new HashSet<NS_NhanVien>();
            NS_NhanVienHKTT = new HashSet<NS_NhanVien>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenXaPhuong { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_QuanHuyen { get; set; }

        public virtual DM_QuanHuyen DM_QuanHuyen { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien> NS_NhanVienTT { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien> NS_NhanVienHKTT { get; set; }
    }
}
