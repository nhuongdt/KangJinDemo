using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("BH_HoaDon")]
    public partial class BH_HoaDon
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BH_HoaDon()
        {
            BH_HoaDon_ChiTiet = new HashSet<BH_HoaDon_ChiTiet>();
            Kho_HoaDon = new HashSet<Kho_HoaDon>();
            Quy_HoaDon_ChiTiet = new HashSet<Quy_HoaDon_ChiTiet>();
            BH_NhanVienThucHien = new HashSet<BH_NhanVienThucHien>();
            HeThong_SMS = new HashSet<HeThong_SMS>();
            Gara_LichBaoDuong = new HashSet<Gara_LichBaoDuong>();
            BH_HoaDon_ChiPhi = new HashSet<BH_HoaDon_ChiPhi>();
            BH_HoaDon_Anh = new HashSet<BH_HoaDon_Anh>();
        }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar")]
        public string MaHoaDon { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime NgayLapHoaDon { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioVao { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? GioRa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ViTri { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_CheckIn { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_DoiTuong { get; set; } = Guid.Empty;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NgoaiTe { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_BangGia { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_HoaDon { get; set; } // Mua(1 chứa ID of 2) ---> Trả (2 chứa ID 3) ---> Gốc (3)
                                             // sudung gdv: ID_HoaDon = null
                                             // trahoadon/tragoi, xulydh: ID_HoaDon !=null
        [Column(TypeName = "int")]
        public int LoaiHoaDon { get; set; }

        [Column(TypeName = "bit")]
        public bool? ChoThanhToan { get; set; } = false;

        [Column(TypeName = "float")]
        public double TongTienHang { get; set; } // KiemKho: tong SL lech giam, TheGiaTri: TongTienNap + KhuyenMai

        [Column(TypeName = "float")]
        public double TongChietKhau { get; set; } = 0;// Loai 1 - HoaDon  (PT Giam), Loai 9 - KiemKho (GTGiam), Loai 22 - TheGiaTri (KhuyenMai) 

        [Column(TypeName = "float")]
        public double TongTienThue { get; set; } = 0;// TheGiaTri: so du sau nap

        [Column(TypeName = "float")]
        public double TongGiamGia { get; set; } = 0;// KiemKho: tong SL chenh lech (tang/giam), HD HoTro: số ngày thuốc

        [Column(TypeName = "float")]
        public double TongChiPhi { get; set; } = 0;
        // TraHang: chi phi hang tra lai (Khách chịu)
        // TheGiaTri: MucNap hoac gtri chenh lech khi dieu chinh the (+: tanglen, - giam di),
        // KiemKho: tong SL lech tang

        [Column(TypeName = "float")]
        public double PhaiThanhToan { get; set; } = 0;// KiemKho: GTTang - Khách hàng thanh toán

        [Column(TypeName = "nvarchar(max)")]
        public string DienGiai { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? SoLanIn { get; set; }

        [Column(TypeName = "float")]
        public double? DiemKhuyenMai { get; set; } = 0;

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string NguoiTao { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayTao { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string NguoiSua { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string YeuCau { get; set; } // HDDatHang (1:Phieu tam, 2: Dang giao hang, 3: HoanThanh, 4: Huy)
                                           // HDChuyenHang (1: Dang chuyen, 2: Phieu tam, 3: Huy, 4: Da Nhan)
                                           // HDBan: 5. HD deleted & update again, 4.Huy
        [Column(TypeName = "datetime")]
        public DateTime? NgayApDungGoiDV { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? HanSuDungGoiDV { get; set; }

        [Column(TypeName = "bit")]
        public bool? An_Hien { get; set; }

        [Column(TypeName = "float")]
        public double? TyGia { get; set; } = 1;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhuyenMai { get; set; }

        [Column(TypeName = "float")]
        public double? KhuyeMai_GiamGia { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string KhuyenMai_GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double? DiemGiaoDich { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? SoLuongKhachHang { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_PhieuTiepNhan { get; set; } //Gara_PhieuTiepNhan

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_BaoHiem { get; set; } // DM_DoiTuong

        [Column(TypeName = "nvarchar(max)")]
        public string LienHeBaoHiem { get; set; } = "";

        [StringLength(20)]
        [Column(TypeName = "nvarchar")]
        public string SoDienThoaiLienHeBaoHiem { get; set; } = "";

        [Column(TypeName = "float")]
        public double? PhaiThanhToanBaoHiem { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TongThanhToan { get; set; } = 0; // PhaiThanhToan + PhaiThanhToanBaoHiem

        [Column(TypeName = "float")]
        public double? ChiPhi { get; set; } = 0;// HoaDon: chiphi cửa hàng phải chịu (vd: chi phí ship)

        [Column(TypeName = "nvarchar(max)")]
        public string ChiPhi_GhiChu { get; set; } = "";

        [Column(TypeName = "int")]
        public int? SoVuBaoHiem { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? KhauTruTheoVu { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? GiamTruBoiThuong { get; set; } = 0; //Chế tài

        [Column(TypeName = "float")]
        public double? PTGiamTruBoiThuong { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TongTienThueBaoHiem { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? PTThueBaoHiem { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? PTThueHoaDon { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? BHThanhToanTruocThue { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? TongTienBHDuyet { get; set; } = 0; // SUM(DonGiaBaoHiem * SoLuong)

        [Column(TypeName = "float")]
        public double? TongThueKhachHang { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? CongThucBaoHiem { get; set; } = 0;

        [Column(TypeName = "float")]
        public double? GiamTruThanhToanBaoHiem { get; set; } = 0; //Giảm trừ thanh toán bảo hiểm sau khi tính thuế

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_Xe { get; set; } //Gara_DanhMucXe

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiTiet> BH_HoaDon_ChiTiet { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_ChiPhi> BH_HoaDon_ChiPhi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_HoaDon_Anh> BH_HoaDon_Anh { get; set; }

        public virtual DM_DoiTuong DM_DoiTuong { get; set; }

        public virtual DM_DonVi DM_DonVi { get; set; }

        public virtual NS_NhanVien NS_NhanVien { get; set; }

        public virtual DM_TienTe DM_TienTe { get; set; }

        public virtual DM_ViTri DM_ViTri { get; set; }

        public virtual DM_GiaBan DM_GiaBan { get; set; }

        public virtual DM_LoaiChungTu DM_LoaiChungTu { get; set; }

        public virtual DM_KhuyenMai DM_KhuyenMai { get; set; }

        public virtual DM_DoiTuong DM_DoiTuongBaoHiem { get; set; }
        public virtual Gara_PhieuTiepNhan Gara_PhieuTiepNhan { get; set; }
        public virtual Gara_DanhMucXe Gara_DanhMucXe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kho_HoaDon> Kho_HoaDon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Quy_HoaDon_ChiTiet> Quy_HoaDon_ChiTiet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<HeThong_SMS> HeThong_SMS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BH_NhanVienThucHien> BH_NhanVienThucHien { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Gara_LichBaoDuong> Gara_LichBaoDuong { get; set; }

        [NotMapped]
        public List<BH_NhanVienThucHien> BH_NhanVienThucHiens { get; set; }

        [NotMapped]
        public Guid ID_QuyHoaDon { get; set; }
    }
}
