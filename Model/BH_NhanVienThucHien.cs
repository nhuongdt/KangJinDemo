using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("BH_NhanVienThucHien")]
    public partial class BH_NhanVienThucHien
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BH_NhanVienThucHien()
        {

        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; } //foreign key NS_NhanVien

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChiTietHoaDon { get; set; } //foreign key BH_HoaDon_ChiTiet

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_HoaDon { get; set; }

        [Column(TypeName = "float")]
        public double TienChietKhau { get; set; }

        [Column(TypeName = "float")]
        public double PT_ChietKhau { get; set; }

        [Column(TypeName = "bit")]
        public bool ThucHien_TuVan { get; set; } //true: thuc hien, false: tư vấn

        [Column(TypeName = "bit")]
        public bool TheoYeuCau { get; set; }

        [Column(TypeName = "int")]
        public int? TinhChietKhauTheo { get; set; }// 1.ThucThu, 2.DoanhThu, 3.VND (HoaDon), 4.LaChietKhau BanGoi (HangHoa)

        [Column(TypeName = "float")]
        public double? HeSo { get; set; } = 1;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_QuyHoaDon { get; set; }

        [Column(TypeName = "int")]
        public int? TinhHoaHongTruocCK { get; set; } = 0; //0. Tính hoa hồng sau chiết khấu, 1. Tính hoa hồng trước chiết khấu

        public virtual NS_NhanVien NS_NhanVien { get; set; }
        public virtual BH_HoaDon_ChiTiet BH_HoaDonChiTiet { get; set; }
        public virtual BH_HoaDon BH_HoaDon { get; set; }
        public virtual Quy_HoaDon Quy_HoaDon { get; set; }
    }

    public partial class BH_NhanVienThucHienDTO
    {
        public Guid ID { get; set; }
        public Guid ID_NhanVien { get; set; }
        public Guid? ID_HoaDon { get; set; }
        public Guid? ID_ChiTietHoaDon { get; set; }
        public double TienChietKhau { get; set; }
        public bool ThucHien_TuVan { get; set; }
        public bool TheoYeuCau { get; set; }
        public string TenNhanVien { get; set; }
        public double PT_ChietKhau { get; set; }
        public int? TinhChietKhauTheo { get; set; }
        public double? HeSo { get; set; } = 1;
        public int? TinhHoaHongTruocCK { get; set; } = 0;
    }
}
