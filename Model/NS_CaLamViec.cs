using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("NS_CaLamViec")]
    public partial class NS_CaLamViec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NS_CaLamViec()
        {
            NS_CaLamViec_DonVi = new HashSet<NS_CaLamViec_DonVi>();
            NS_PhieuPhanCa_CaLamViec = new HashSet<NS_PhieuPhanCa_CaLamViec>();
            NS_BangLuongOTChiTiet = new HashSet<NS_BangLuongOTChiTiet>();
            NS_ThietLapLuongChiTiet = new HashSet<NS_ThietLapLuongChiTiet>();
            NS_CongBoSung = new HashSet<NS_CongBoSung>();
            //NS_BangLuong_ChiTiet = new HashSet<NS_BangLuong_ChiTiet>();
        }
        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string MaCa { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string TenCa { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime GioVao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime GioRa { get; set; }

        [Column(TypeName = "float")]
        public double TongGioCong { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NghiGiuaCaTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NghiGiuaCaDen { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioOTBanNgayTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioOTBanNgayDen { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioOTBanDemTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioOTBanDemDen { get; set; }

        [Column(TypeName = "int")]
        public int? ThoiGianDiMuonVeSom { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? SoPhutDiMuon { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? SoPhutVeSom { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime? GioVaoTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioVaoDen { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioRaTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioRaDen { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TinhOTBanNgayTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TinhOTBanNgayDen { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TinhOTBanDemTu { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? TinhOTBanDemDen { get; set; }

        [Column(TypeName = "int")]
        public int LaCaDem { get; set; }

        [Column(TypeName = "int")]
        public int CachLayGioCong { get; set; }

        [Column(TypeName = "float")]
        public double SoGioOTToiThieu { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChuCaLamViec { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChuTinhGio { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int TrangThai { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string CaLamViec_KhongDau { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string CaLamViec_ChuCaiDau { get; set; } = string.Empty;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CaLamViec_DonVi> NS_CaLamViec_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_PhieuPhanCa_CaLamViec> NS_PhieuPhanCa_CaLamViec { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_BangLuongOTChiTiet> NS_BangLuongOTChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_ThietLapLuongChiTiet> NS_ThietLapLuongChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NS_CongBoSung> NS_CongBoSung { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<NS_BangLuong_ChiTiet> NS_BangLuong_ChiTiet { get; set; }
    }
    public class DM_CaLamViecPROC
    {
        public Guid ID { get; set; }
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public int TrangThai { get; set; }
        public DateTime GioVao { get; set; }
        public DateTime GioRa { get; set; }
        public DateTime? NghiGiuaCaTu { get; set; }
        public DateTime? NghiGiuaCaDen { get; set; }
        public DateTime? GioOTBanNgayTu { get; set; }
        public DateTime? GioOTBanNgayDen { get; set; }
        public DateTime? GioOTBanDemTu { get; set; }
        public DateTime? GioOTBanDemDen { get; set; }
        public double TongGioCong { get; set; }
        public int? SoPhutDiMuon { get; set; }
        public int? SoPhutVeSom { get; set; }
        public DateTime? GioVaoTu { get; set; }
        public DateTime? GioVaoDen { get; set; }
        public DateTime? GioRaTu { get; set; }
        public DateTime? GioRaDen { get; set; }
        public DateTime? TinhOTBanNgayTu { get; set; }
        public DateTime? TinhOTBanNgayDen { get; set; }
        public DateTime? TinhOTBanDemTu { get; set; }
        public DateTime? TinhOTBanDemDen { get; set; }
        public int LaCaDem { get; set; }
        public int CachLayGioCong { get; set; }
        public double SoGioOTToiThieu { get; set; }
        public string GhiChuCaLamViec { get; set; }
        public string GhiChuTinhGio { get; set; }
        public string NguoiTao { get; set; }
        public DateTime NgayTao { get; set; }
    }
    public class Export_CaLamViecPROC
    {
        public string MaCa { get; set; }
        public string TenCa { get; set; }
        public string TrangThai { get; set; }
        public string GioVao { get; set; }
        public string GioRa { get; set; }
        public string NghiGiuaCaTu { get; set; }
        public string NghiGiuaCaDen { get; set; }
        public string GioOTBanNgayTu { get; set; }
        public string GioOTBanNgayDen { get; set; }
        public string GioOTBanDemTu { get; set; }       
        public string GioOTBanDemDen { get; set; }
        public double TongGioCong { get; set; }
        public string CachLayGioCong { get; set; }
        public string LaCaDem { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string GhiChuCaLamViec { get; set; }
        public int? SoPhutDiMuon { get; set; }
        public int? SoPhutVeSom { get; set; }
        public string GioVaoTu { get; set; }
        public string GioVaoDen { get; set; }
        public string GioRaTu { get; set; }
        public string GioRaDen { get; set; }
        public string TinhOTBanNgayTu { get; set; }
        public string TinhOTBanNgayDen { get; set; }
        public string TinhOTBanDemTu { get; set; }
        public string TinhOTBanDemDen { get; set; }
        public double? SoGioOTToiThieu { get; set; }
        public string GhiChuTinhGio { get; set; }
    }
    public class CaTuan_NhanVienPhanCaPROC
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string Thu2 { get; set; }
        public string Thu3 { get; set; }
        public string Thu4 { get; set; }
        public string Thu5 { get; set; }
        public string Thu6 { get; set; }
        public string Thu7 { get; set; }
        public string ChuNhat { get; set; }
    }
    public class CaThang_NhanVienPhanCaPROC
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string Ngay01 { get; set; }
        public string Ngay02 { get; set; }
        public string Ngay03 { get; set; }
        public string Ngay04 { get; set; }
        public string Ngay05 { get; set; }
        public string Ngay06 { get; set; }
        public string Ngay07 { get; set; }
        public string Ngay08 { get; set; }
        public string Ngay09 { get; set; }
        public string Ngay10 { get; set; }
        public string Ngay11 { get; set; }
        public string Ngay12 { get; set; }
        public string Ngay13 { get; set; }
        public string Ngay14 { get; set; }
        public string Ngay15 { get; set; }
        public string Ngay16 { get; set; }
        public string Ngay17 { get; set; }
        public string Ngay18 { get; set; }
        public string Ngay19 { get; set; }
        public string Ngay20 { get; set; }
        public string Ngay21 { get; set; }
        public string Ngay22 { get; set; }
        public string Ngay23 { get; set; }
        public string Ngay24 { get; set; }
        public string Ngay25 { get; set; }
        public string Ngay26 { get; set; }
        public string Ngay27 { get; set; }
        public string Ngay28 { get; set; }
        public string Ngay29 { get; set; }
        public string Ngay30 { get; set; }
        public string Ngay31 { get; set; }
    }
    public class MacDinh_NhanVienPhanCaPROC
    {
        public Guid ID_NhanVien { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
        public string CaLamViec { get; set; }
    }
}
