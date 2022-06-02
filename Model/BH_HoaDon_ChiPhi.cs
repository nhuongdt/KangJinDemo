using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("BH_HoaDon_ChiPhi")]
    public partial class BH_HoaDon_ChiPhi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BH_HoaDon_ChiPhi()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HoaDon { get; set; } //ID trong bảng BH_HoaDon

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HoaDon_ChiTiet { get; set; } //ID trong bảng BH_HoaDon_ChiTiet

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhaCungCap { get; set; } //ID trong bảng DM_DoiTuong

        [Column(TypeName = "float")]
        public double SoLuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double DonGia { get; set; } = 0;

        [Column(TypeName = "float")]
        public double ThanhTien { get; set; } = 0;

        [Column(TypeName = "nvarchar")]
        public string GhiChu { get; set; } = "";

        public virtual BH_HoaDon BH_HoaDon { get; set; }
        public virtual BH_HoaDon_ChiTiet BH_HoaDon_ChiTiet { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

    }
}
