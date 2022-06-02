using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("BH_HoaDon_ChiTiet")]
    public partial class BH_HoaDon_ChiTiet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BH_HoaDon_ChiTiet()
        {
            Kho_HoaDon_ChiTiet = new HashSet<Kho_HoaDon_ChiTiet>();
            BH_NhanVienThucHien = new HashSet<BH_NhanVienThucHien>();
            BH_HoaDon_ChiPhi = new HashSet<BH_HoaDon_ChiPhi>();
        }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_HoaDon { get; set; }

        [Column(TypeName = "int")]
        public int SoThuTu { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGian { get; set; }

        [Column(TypeName = "float")]
        public double? ThoiGianBaoHanh { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? LoaiThoiGianBH { get; set; } = 0;//1.ngay, 2.thang, 3.nam

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhoHang { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoHang { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(250)]
        public string ChatLieu { get; set; } = string.Empty;// 1. Tra HD, 2.Tra GDV, 3. Xuly DH,  4.Sudung GDV, else: empty/null, 5.ChiTiet bi xóa khi update HD

        [Column(TypeName = "nvarchar")]
        [StringLength(250)]
        public string MauSac { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar")]
        [StringLength(250)]
        public string KichCo { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double SoLuong { get; set; }  // if phieukiemke: SoLuong = SoLuongLech: (-) giam, (+) tang

        [Column(TypeName = "float")]
        public double DonGia { get; set; } = 0;

        [Column(TypeName = "float")]
        public double ThanhTien { get; set; } = 0; // if phieukiemke: ThanhTien = SL ThucTe

        [Column(TypeName = "float")]
        public double PTChietKhau { get; set; } = 0;// if: dieuchinh giavon PTChietKhau = GiaVonLech (tang): soduong (+)

        [Column(TypeName = "float")]
        public double TienChietKhau { get; set; } = 0;// if: dieuchinh giavon PTChietKhau = GiaVonLech (giam): so am (-)
                                                 //  if phieukiemke: TienChietKhau = TonKho (DB) +/-
                                                 // dieuchuyen: soluong thucte nhan

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ThueSuat { get; set; }

        [Column(TypeName = "float")]
        public double TienThue { get; set; } = 0;

        [Column(TypeName = "float")]
        public double PTChiPhi { get; set; } = 0;

        [Column(TypeName = "float")]
        public double TienChiPhi { get; set; } = 0;

        [Column(TypeName = "float")]
        public double ThanhToan { get; set; } = 0;// if kiemkho: giatrilech = soluonglech * giavon

        [Column(TypeName = "float")]
        public double? GiaVon { get; set; } = 0; // Giá vốn kho chuyển trong hóa đơn chuyển hàng

        [Column(TypeName = "float")]
        public double? GiaVon_NhanChuyenHang { get; set; } = 0; //Giá vốn kho nhận trong hóa đơn chuyển hàng

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? SoLanDaIn { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_TangKem { get; set; } // neu 3 SP cung tang 1 HH --> luu ID_TangKem vao ca 3 HH

        [Column(TypeName = "bit")]
        public bool? TangKem { get; set; } = false;

        [Column(TypeName = "float")]
        public double? ThoiGianThucHien { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? SoLuong_TL { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? SoLuong_YC { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool? Chieu { get; set; } = false;

        [Column(TypeName = "bit")]
        public bool? Sang { get; set; } = false;

        [Column(TypeName = "float")]
        public double? PTThue { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TonLuyKe { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TonLuyKe_NhanChuyenHang { get; set; } = 0;

        [Column(TypeName = "bit")]
        public bool An_Hien { get; set; } = true;

        [Column(TypeName = "uniqueidentifier")]
        [Required]
        public Guid ID_DonViQuiDoi { get; set; }

        [Column(TypeName = "float")]
        public double? Bep_SoLuongYeuCau { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? Bep_SoLuongChoCungUng { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? Bep_SoLuongHoanThanh { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhuyenMai { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChiTietDinhLuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChiTietGoiDV { get; set; }

        [Column(TypeName = "float")]
        public double? SoLuongDinhLuong_BanDau { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ViTri { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGianHoanThanh { get; set; }// not set value default: nhahang (dichvu theogio), banhang (thoigian ketthuc dichvu)

        [Column(TypeName = "int")]
        public int? QuaThoiGian { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? DiemKhuyenMai { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string TenHangHoaThayThe { get; set; } = "";

        [Column(TypeName = "float")]
        public double? DonGiaBaoHiem { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LichBaoDuong { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ParentCombo { get; set; }

        public virtual BH_HoaDon BH_HoaDon { get; set; }

        public virtual DM_Kho DM_Kho { get; set; }

        public virtual DM_LoHang DM_LoHang { get; set; }

        public virtual DonViQuiDoi DonViQuiDoi { get; set; }

        public virtual DM_KhuyenMai DM_KhuyenMai { get; set; }

        public virtual DM_ViTri DM_ViTri { get; set; }

        public virtual Gara_LichBaoDuong Gara_LichBaoDuong { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon_ChiTiet> Kho_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_NhanVienThucHien> BH_NhanVienThucHien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiPhi> BH_HoaDon_ChiPhi { get; set; }

        [NotMapped]
        public ICollection<BH_HoaDon_ChiTiet> ThanhPhan_DinhLuong { get; set; }
        [NotMapped]
        public ICollection<BH_HoaDon_ChiTiet> ThanhPhanComBo { get; set; }
        [NotMapped]
        public string MaHangHoa { get; set; }
        [NotMapped]
        public string MaLoHang { get; set; }

        [NotMapped]
        public double? GiaBanHH { get; set; }

        [NotMapped]
        public Guid? ID_HangHoa { get; set; }
    }
    public partial class BH_ChiTietHoaDon_Excel
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoaFull { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string MaLoHang { get; set; }
        public double SoLuong { get; set; }
        public double DonGia { get; set; }
        public double GiamGia { get; set; }
        public double? TienThue { get; set; }
        public double ThanhTien { get; set; }
        public double? ThanhToan { get; set; }
        public string GhiChu { get; set; }
    }
    public partial class BH_ChiTietPhieuChuyenHang_Excel
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string TenDonViTinh { get; set; }
        public string MaLoHang { get; set; }
        public double SoLuong { get; set; }
        public double GiaTriChuyen { get; set; }
        public double ThanhTien { get; set; }
        public string GhiChu { get; set; }
    }

    public class BH_HoaDon_ChiTietComparer : IEqualityComparer<BH_HoaDon_ChiTiet>
    {
        public bool Equals(BH_HoaDon_ChiTiet x, BH_HoaDon_ChiTiet y)
        {
            return x.ID_DonViQuiDoi == y.ID_DonViQuiDoi
            && x.DonGia == y.DonGia && x.ID_HoaDon == y.ID_HoaDon
            && x.SoLuong == y.SoLuong && x.ThanhTien == y.ThanhTien
            && x.TienChietKhau == y.TienChietKhau && (x.GiaVon == y.GiaVon || y.GiaVon == null)
            && (x.ID_LoHang == null || x.ID_LoHang == y.ID_LoHang) && x.GhiChu == y.GhiChu;
        }

        public int GetHashCode(BH_HoaDon_ChiTiet obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;

            int hashID_DonViQuiDoi = obj.ID_DonViQuiDoi == null ? 0 : obj.ID_DonViQuiDoi.GetHashCode();
            int hashDonGia = obj.DonGia.GetHashCode();
            int hashID_HoaDon = obj.ID_HoaDon == null ? 0 : obj.ID_HoaDon.GetHashCode();
            int hashSoLuong = obj.SoLuong.GetHashCode();
            int hashThanhTien = obj.ThanhTien.GetHashCode();
            int hashTienChietKhau = obj.TienChietKhau.GetHashCode();
            int hashGiaVon = obj.GiaVon == null ? 0 : obj.GiaVon.GetHashCode();
            int hashID_LoHang = obj.ID_LoHang == null ? 0 : obj.ID_LoHang.GetHashCode();
            int hashGhiChu = obj.GhiChu == null ? 0 : obj.GhiChu.GetHashCode();

            return hashID_DonViQuiDoi ^ hashDonGia ^ hashID_HoaDon ^ hashSoLuong ^ hashThanhTien ^ hashTienChietKhau ^ hashGiaVon ^ hashID_LoHang ^ hashGhiChu;
        }
    }

    public class BH_HoaDon_ChiTietIDComparer : IEqualityComparer<BH_HoaDon_ChiTiet>
    {
        public bool Equals(BH_HoaDon_ChiTiet x, BH_HoaDon_ChiTiet y)
        {
            return x.ID == y.ID;
        }

        public int GetHashCode(BH_HoaDon_ChiTiet obj)
        {
            if (Object.ReferenceEquals(obj, null)) return 0;

            int hashID_DonViQuiDoi = obj.ID == null ? 0 : obj.ID.GetHashCode();

            return hashID_DonViQuiDoi;
        }
    }
}
