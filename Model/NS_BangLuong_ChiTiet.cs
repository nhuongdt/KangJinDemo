using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_BangLuong_ChiTiet")]
    public partial class NS_BangLuong_ChiTiet
    {
        public NS_BangLuong_ChiTiet()
        {
            NS_BangLuongOTChiTiet = new HashSet<NS_BangLuongOTChiTiet>();
            NS_CongBoSung = new HashSet<NS_CongBoSung>();
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_BangLuong { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaBangLuongChiTiet { get; set; }

        //[Column(TypeName = "uniqueidentifier")]
        //public Guid ID_BangCong_ChiTiet { get; set; } // null

        //[Column(TypeName = "uniqueidentifier")]
        //public Guid? ID_CaLamViec { get; set; } // multiple ca, phieu

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_NhanVien { get; set; }

        [Column(TypeName = "float")]
        public double NgayCongThuc { get; set; } = 0;

        [Column(TypeName = "float")]
        public double NgayCongChuan { get; set; } = 0;

        [Column(TypeName = "float")]
        public double LuongCoBan { get; set; } = 0;

        [Column(TypeName = "float")]
        public double PhuCapCoBan { get; set; } = 0;

        [Column(TypeName = "float")]
        public double PhuCapKhac { get; set; } = 0;

        [Column(TypeName = "float")]
        public double KhenThuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double ThueTNCN { get; set; } = 0;

        [Column(TypeName = "float")]
        public double MienGiamThue { get; set; } = 0;

        [Column(TypeName = "float")]
        public double BaoHiem { get; set; } = 0;

        [Column(TypeName = "float")]
        public double KyLuat { get; set; } = 0;

        [Column(TypeName = "float")]
        public double PhatDiMuon { get; set; } = 0;

        [Column(TypeName = "float")]
        public double LuongOT { get; set; } = 0;

        [Column(TypeName = "float")]
        public double LuongThucNhan { get; set; } = 0;

        [Column(TypeName = "float")]
        public double TongTienPhat { get; set; } = 0;

        [Column(TypeName = "float")]
        public double TongLuongNhan { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; }// idcas

        [Column(TypeName = "int")]
        public int TrangThai { get; set; } = 0;// = same trangthai in NS_BangLuong

        [Column(TypeName = "nvarchar")]
        public string NguoiTao { get; set; }// idphucap

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar")]
        public string NguoiSua { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "float")]
        public double BaoHiemCongTyDong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double DoanhSo { get; set; } = 0;

        [Column(TypeName = "float")]
        public double ChietKhau { get; set; } = 0;

        public virtual NS_BangLuong NS_BangLuong { get; set; }

        //public virtual NS_CaLamViec NS_CaLamViec { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BangLuongOTChiTiet> NS_BangLuongOTChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongBoSung> NS_CongBoSung { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }
    }
}
