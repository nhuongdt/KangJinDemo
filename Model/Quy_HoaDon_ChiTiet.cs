using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Quy_HoaDon_ChiTiet")]
    public partial class Quy_HoaDon_ChiTiet
    {
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HoaDon { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DoiTuong { get; set; } = Guid.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TheKhachHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_BangLuongChiTiet { get; set; }

        [Column(TypeName = "float")]
        public double? DiemThanhToan { get; set; }

        [Column(TypeName = "float")]
        public double ThuTuThe { get; set; } // use for Spa (The tra truoc, or Tra Sau)

        [Column(TypeName = "float")]
        public double TienMat { get; set; }

        [Column(TypeName = "float")]
        public double TienGui { get; set; } // TienATM or TienCK

        [Column(TypeName = "float")]
        public double TienThu { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_HoaDonLienQuan { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TaiKhoanNganHang { get; set; } // at DM_TaiKhoanNganHang

        [Column(TypeName = "float")]
        public double? ChiPhiNganHang { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NganHang { get; set; } 

        [Column(TypeName = "bit")]
        public bool? LaPTChiPhiNganHang { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? ThuPhiTienGui { get; set; } = false; // not use

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhoanThuChi { get; set; } 

        [Column(TypeName = "float")]
        public double TruTamUngLuong { get; set; } = 0;// used to tinhluong tru tamung

        [Column(TypeName = "int")]
        public int? HinhThucThanhToan { get; set; } = 1;// 1.mat, 2.pos, 3.ck, 4.thegiatri, 5.diem, 6.coc

        [Column(TypeName = "int")]
        public int? LoaiThanhToan { get; set; } = 0;// 0.default, 1.tiencoc, 3.khong butru cong no, 4.hoàn trả tiền thẻ

        public virtual BH_HoaDon BH_HoaDon { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        public virtual DM_NganHang DM_NganHang { get; set; }

        public virtual Quy_HoaDon Quy_HoaDon { get; set; }

        public virtual Quy_KhoanThuChi Quy_KhoanThuChi { get; set; }

        public virtual The_TheKhachHang The_TheKhachHang { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual DM_TaiKhoanNganHang DM_TaiKhoanNganHang { get; set; }

        public virtual NS_BangLuong_ChiTiet NS_BangLuong_ChiTiet { get; set; }

        public static implicit operator List<object>(Quy_HoaDon_ChiTiet v)
        {
            throw new NotImplementedException();
        }
        [NotMapped]
        public string MaBangLuongChiTiet { get; set; }
    }
}
