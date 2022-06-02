using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("ChamSocKhachHangs")]
    public partial class ChamSocKhachHang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChamSocKhachHang()
        {
            ChamSocKhachHang2 = new HashSet<ChamSocKhachHang>();
            OptinForm_Link = new HashSet<OptinForm_Link>();
            ChamSocKhachHang_NhanVien = new HashSet<ChamSocKhachHang_NhanVien>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_KhachHang { get; set; } = Guid.Empty; //foreign key: DM_DoiTuong

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LoaiTuVan { get; set; } //foreign key: DM_LoaiTuVanLichHen

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Ma_TieuDe { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int PhanLoai { get; set; } //1 - Tư vấn. 2 - Phản hồi. 3 - Lịch hẹn. 4- Công việc

        [Column(TypeName = "datetime")]
        public DateTime NgayGio { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? NgayGioKetThuc { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NoiDung { get; set; } = string.Empty;// lichhen (loai=3): muon tam truong nay luu MaDichVu --> get TenDichVu/HangHoa when update

        [Column(TypeName = "nvarchar(max)")]
        public string TraLoi { get; set; } = string.Empty; //phản hồi khách hàng phiếu tư vấn

        [Column(TypeName = "nvarchar(max)")]
        public string TrangThai { get; set; } = string.Empty; //1 - Hoàn thành. 2 - Đang xử lý, chưa thực hiện, Tham khảo. 3 - Chưa xử lý, chưa thực hiện, tiềm năng. 4 - Hủy
        //TrangThaiCongViec: 1.Dang xu ly.2:HoanThanh.3:Huy.0:Xoa
        //LichHen: 1.Đặt lịch, 2. Hoàn thành, 3.Huy

        [Column(TypeName = "int")]
        public int NhacNho { get; set; } = 0; //chọn thời gian nhắc nhở trước khi đến lịch hẹn, quy đổi ra đơn vị phút nếu chọn là giờ. thời gian = 0 là không nhắc nhỏ

        [Column(TypeName = "int")]
        public int MucDoPhanHoi { get; set; } = 2; //1 - Thấp, 2 - Bình thường, 3 - Cao

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVien { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_LienHe { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_NhanVienQuanLy { get; set; } //foreign key: NS_NhanVien. nhân viên quản lý phiếu tư vấn, ... Mặc định nhân viên tạo là nhân viên quản lý

        [Column(TypeName = "datetime")]
        public DateTime? ThoiGianHenLai { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiTao { get; set; } = string.Empty;
        
        [Column(TypeName = "datetime")]
        public DateTime NgayTao { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string NguoiSua { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgaySua { get; set; }

        [Column(TypeName = "int")]
        public int? SoLuong { get; set; } = 0;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_HangHoa { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_ChamSocKhachHang { get; set; }

        [Column(TypeName = "int")]
        public int? MucDoUuTien { get; set; } = 0; //3 - Thấp, 2 - Trung bình, 1 - Cao

        [Column(TypeName = "nvarchar(max)")]
        public string FileDinhKem { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime? NgayHoanThanh { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string KetQua { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string GhiChu { get; set; } = string.Empty;

        [Column(TypeName = "bit")]
        public bool? CaNgay { get; set; } = false;

        [Column(TypeName = "uniqueidentifier")]
        public Guid? ID_Parent { get; set; }

        [Column(TypeName = "int")]
        public int? KieuLap { get; set; } = 0;//0.khonglap, 1.ngay,2.tuan,3.thang,4.nam

        [Column(TypeName = "int")]
        public int? SoLanLap { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string GiaTriLap { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? TuanLap { get; set; } = 0;

        [Column(TypeName = "int")]
        public int? TrangThaiKetThuc { get; set; } = 0;

        [Column(TypeName = "nvarchar(max)")]
        public string GiaTriKetThuc { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        public int? KieuNhacNho { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime? NgayCu { get; set; }

        public virtual DM_LoaiTuVanLichHen DM_LoaiTuVanLichHen { get; set; }
        public virtual DM_DoiTuong DM_DoiTuong { get; set; }
        public virtual NS_NhanVien NS_NhanVien { get; set; }
        public virtual NS_NhanVien NS_NhanVien1 { get; set; }
        public virtual DM_DonVi DM_DonVi { get; set; }
        public virtual DM_HangHoa DM_HangHoa { get; set; }
        public virtual ChamSocKhachHang ChamSocKhachHang1 { get; set; }
        public virtual DM_LienHe DM_LienHe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang> ChamSocKhachHang2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptinForm_Link> OptinForm_Link { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChamSocKhachHang_NhanVien> ChamSocKhachHang_NhanVien { get; set; }

        [NotMapped]
        public string TenNV { get; set; }

        [NotMapped]
        public string MaTieuDeUnsign { get; set; }
        [NotMapped]
        public string TenLoaiTuVanUnSign { get; set; }
        [NotMapped]
        public string TenKhachHangUnSign { get; set; }
        [NotMapped]
        public string TenNhanVienUnSign { get; set; }
        [NotMapped]
        public string TenLoaiTV { get; set; }
        [NotMapped]
        public string TenKhachHang { get; set; }
        [NotMapped]
        public string strMucDoPhanHoi { get; set; }
        [NotMapped]
        public string SoDienThoai { get; set; }


    }

    public partial class LH_ChamSocKhachHang
    {
        public Guid ID { get; set; }
        public Guid? ID_KhachHang { get; set; } //foreign key: NS_NhanVien
        public Guid? ID_LoaiTuVan { get; set; } //foreign key: DM_LoaiTuVanLichHen
        public Guid? ID_NhanVien { get; set; }
        public Guid ID_NhanVienQuanLy { get; set; } //foreign key: NS_NguoiDung. tài khoản quản lý phiếu tư vấn, ... Mặc định người tạo là người quản lý
        public string NguoiTao { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu
        public string NguoiSua { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu 

        public string Ma_TieuDe { get; set; }
        public DateTime NgayGio { get; set; }
        public DateTime? NgayGioKetThuc { get; set; }
        public string TenKhachHang { get; set; }
        public string TenLoaiTV { get; set; }
        public int NhacNho { get; set; }
        public string TrangThai { get; set; }
        public string NoiDung { get; set; }
        public string TenNV { get; set; }
        public string Ma_TieuDe_CV { get; set; }
        public string Ma_TieuDe_GC { get; set; }
        public string TenKhachHang_GC { get; set; }
        public string TenKhachHang_CV { get; set; }
    }

    public partial class NS_CongViecDTO
    {
        public Guid ID { get; set; }
        public Guid ID_LoaiCongViec { get; set; }
        public Guid ID_KhachHang { get; set; }
        public Guid? ID_LienHe { get; set; }
        public Guid? ID_NhanVienChiaSe { get; set; }
        public Guid? ID_NhanVienQuanLy { get; set; }
        public string LoaiCongViec { get; set; } //foreign key: NS_NhanVien
        public string TenKhachHang { get; set; } //foreign key: DM_LoaiTuVanLichHen
        public string TenLienHe { get; set; }
        public string KetQuaCongViec { get; set; }
        public string LyDoHenLai { get; set; }
        public string TenNhanVien { get; set; } //foreign key: NS_NguoiDung. tài khoản quản lý phiếu tư vấn, ... Mặc định người tạo là người quản lý
        public int TrangThai { get; set; }
        public string TrangThaiText { get; set; }
        public int LoaiDoiTuongCV { get; set; }
        public DateTime ThoiGianTu { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu
        public DateTime? ThoiGianDen { get; set; } //foreign key: NS_NguoiDung, tài khoản tạo phiếu 

        public int? NhacTruoc { get; set; }
        public string NhacTruocText { get; set; }
        public string NhacTruocLienHeLaiText { get; set; }
        public string NoiDung { get; set; }
        public DateTime? ThoiGianLienHeLai { get; set; }
        public int? NhacTruocLienHeLai { get; set; }

        public string TenCongViecUnSign { get; set; }
        public string TenCongViecStartChar { get; set; }
        public string TenKhachHangUnSign { get; set; }
        public string TenKhachHangStartChar { get; set; }
        public string TenLienHeUnSign { get; set; }
        public string TenLienHeStartChar { get; set; }
        public string TenNhanVienUnSign { get; set; }
        public string TenNhanVienStartChar { get; set; }
        public string TenNguoiTao { get; set; }

        public Guid? TrangThaiKhach { get; set; }

    }

    public class ListLHPages
    {
        public int SoTrang { get; set; }
    }
    public class listPhanLoai
    {
        public string PhanLoai { get; set; }
    }
}
