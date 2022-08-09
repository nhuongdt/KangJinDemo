using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NhomHang_ChiTietSanPhamHoTro")]
    public partial class NhomHang_ChiTietSanPhamHoTro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhomHang_ChiTietSanPhamHoTro()
        {

        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid Id { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid Id_NhomHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid Id_DonViQuiDoi { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? Id_LoHang { get; set; }

        [Column(TypeName = "float")]
        public double SoLuong { get; set; }

        [Column(TypeName = "int")]
        public int LaSanPhamNgayThuoc { get; set; } //0. False, 1. True

        public virtual DM_NhomHangHoa DM_NhomHangHoa { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }

        public virtual DM_LoHang DM_LoHang { get; set; }
    }
}
