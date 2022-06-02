using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_LoaiLuong")]
    public partial class NS_LoaiLuong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_LoaiLuong()
        {
            NS_Luong_PhuCap = new HashSet<NS_Luong_PhuCap>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string TenLoaiLuong { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? LoaiLuong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }// 0.xoa, 1.phucaptheongay, 2. phucapcodinh vnd, 3.phucapcodinh %, 4.phạt đi muộn/lần, 5.giamtru codinh(vnd), 6.giamtru codinh (%tongthunhap)

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_Luong_PhuCap> NS_Luong_PhuCap { get; set; }
    }
}
