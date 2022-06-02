using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("DM_KhuyenMai_ChiTiet")]
    public partial class DM_KhuyenMai_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_KhuyenMai { get; set; }

        [Column(TypeName = "int")]
        public int? STT { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TongTienHang { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? GiamGia { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? GiamGiaTheoPhanTram { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonViQuiDoi { get; set; } //ID hàng tặng, giảm giá

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomHangHoa { get; set; } //ID nhóm hàng tặng, giảm giá

        [Column(TypeName = "float")]
        public double? SoLuong { get; set; } = 0; // số lượng hàng tặng

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DonViQuiDoiMua { get; set; } //id hàng mua

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhomHangHoaMua { get; set; } //id nhóm hàng mua

        [Column(TypeName = "float")]
        public double? SoLuongMua { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? GiaKhuyenMai { get; set; } = 0;
        public virtual DM_KhuyenMai DM_KhuyenMai { get; set; }
        public virtual DonViQuiDoi DonViQuiDoi { get; set; }
        public virtual DM_NhomHangHoa DM_NhomHangHoa { get; set; }
        public virtual DonViQuiDoi DonViQuiDoi1 { get; set; }
        public virtual DM_NhomHangHoa DM_NhomHangHoa1 { get; set; }
    }
}
