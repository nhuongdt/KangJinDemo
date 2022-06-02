using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_PhongBan")]
    public partial class NS_PhongBan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_PhongBan()
        {
            NS_PhongBanCha = new HashSet<NS_PhongBan>();
            NS_QuaTrinhCongTac = new HashSet<NS_QuaTrinhCongTac>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaPhongBan { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenPhongBan { get; set; } = string.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_PhongBanCha { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonVi { get; set; }

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        public virtual NS_PhongBan NS_PhongBan1 { get; set; }
        public virtual DM_DonVi DM_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhongBan> NS_PhongBanCha { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_NhanVien> NS_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_QuaTrinhCongTac> NS_QuaTrinhCongTac { get; set; }
    }
}
