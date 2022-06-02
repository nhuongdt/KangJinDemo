using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_CauHinh_TichDiemChiTiet")]
    public partial class HT_CauHinh_TichDiemChiTiet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HT_CauHinh_TichDiemChiTiet()
        {
            HT_CauHinh_TichDiemApDung = new HashSet<HT_CauHinh_TichDiemApDung>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_CauHinh { get; set; }

        [Column(TypeName = "float")]
        public double TyLeDoiDiem { get; set; }

        [Column(TypeName = "bit")]
        public bool ThanhToanBangDiem { get; set; }

        [Column(TypeName = "int")]
        public int DiemThanhToan { get; set; }

        [Column(TypeName = "float")]
        public double TienThanhToan { get; set; }

        [Column(TypeName = "int")]
        public int? SoLanMua { get; set; } = 0; //Sử dụng điểm sau số lần mua

        [Column(TypeName = "bit")]
        public bool TichDiemGiamGia { get; set; } //Tích điểm cho sản phẩm giảm giá

        [Column(TypeName = "bit")]
        public bool? TichDiemHoaDonGiamGia { get; set; } = false; //Tích điểm cho hóa đơn giảm giá

        [Column(TypeName = "bit")]
        public bool TichDiemHoaDonDiemThuong { get; set; } //Tích điểm cho hóa đơn thanh toán bằng điểm thưởng

        [Column(TypeName = "bit")]
        public bool ToanBoKhachHang { get; set; }

        [Column(TypeName = "bit")]
        public bool KhoiTaoTichDiem { get; set; }

        public virtual HT_CauHinhPhanMem HT_CauHinhPhanMem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_CauHinh_TichDiemApDung> HT_CauHinh_TichDiemApDung { get; set; }
    }
}
