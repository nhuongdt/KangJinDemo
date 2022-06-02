using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("HT_CauHinhPhanMem")]
    public partial class HT_CauHinhPhanMem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HT_CauHinhPhanMem()
        {
            HT_CauHinh_TichDiemChiTiet = new HashSet<HT_CauHinh_TichDiemChiTiet>();
            HT_CauHinh_GioiHanTraHang = new HashSet<HT_CauHinh_GioiHanTraHang>();
        }

        [Key]
        [Column(TypeName = "uniqueidentifier")]
        public Guid ID { get; set; }

        [Column(TypeName = "uniqueidentifier")]
        public Guid ID_DonVi { get; set; } //danh mục đơn vị

        [Column(TypeName = "bit")]
        public bool GiaVonTrungBinh { get; set; } //gia vốn trung bình

        [Column(TypeName = "bit")]
        public bool CoDonViTinh { get; set; } //sản phẩm có đơn vị tính

        [Column(TypeName = "bit")]
        public bool DatHang { get; set; } //Tính năng đặt hàng

        [Column(TypeName = "bit")]
        public bool XuatAm { get; set; } // Bán hàng, chuyển hàng, trả hàng nhập khi hết tồn kho

        [Column(TypeName = "bit")]
        public bool DatHangXuatAm { get; set; } //Cho phép đặt hàng khi hết tồn kho

        [Column(TypeName = "bit")]
        public bool ThayDoiThoiGianBanHang { get; set; } //Cho phép thay đổi thời gian bán hàng

        [Column(TypeName = "bit")]
        public bool SoLuongTrenChungTu { get; set; } //Hiển thị tổng số lượng hàng hóa trên chứng từ

        [Column(TypeName = "bit")]
        public bool TinhNangTichDiem { get; set; } //Tính năng tích điểm

        [Column(TypeName = "bit")]
        public bool GioiHanThoiGianTraHang { get; set; } //Giới hạn thời gian trả hàng

        [Column(TypeName = "bit")]
        public bool SanPhamCoThuocTinh { get; set; } //sản phẩm có thuộc tính

        [Column(TypeName = "bit")]
        public bool BanVaChuyenKhiHangDaDat { get; set; } //Bán hàng và chuyển hàng khi sản phẩm đã đặt

        [Column(TypeName = "bit")]
        public bool TinhNangSanXuatHangHoa { get; set; } //Tính năng sản xuất hàng hóa

        [Column(TypeName = "bit")]
        public bool SuDungCanDienTu { get; set; } //Sử dụng cân điện tử

        [Column(TypeName = "bit")]
        public bool KhoaSo { get; set; } //Tính năng khóa sổ

        [Column(TypeName = "bit")]
        public bool InBaoGiaKhiBanHang { get; set; } //Cho phép in báo giá khi bán hàng

        [Column(TypeName = "bit")]
        public bool QuanLyKhachHangTheoDonVi { get; set; } //Quản lý khách hàng theo đơn vị, chi nhánh

        [Column(TypeName = "bit")]
        public bool? KhuyenMai { get; set; } = false; //Cho phép sử dụng tính năng khuyến mãi

        [Column(TypeName = "bit")]
        public bool? LoHang { get; set; } = false; //Cho phép sử dụng tính nắng lô hàng

        [Column(TypeName = "bit")]
        public bool? SuDungMauInMacDinh { get; set; } = true; //Cho phép thiết lập sử dụng mẫu in mặc định

        [Column(TypeName = "bit")]
        public bool? ApDungGopKhuyenMai { get; set; } = false; //Áp dụng gộp các chương trình khuyến mãi

        [Column(TypeName = "bit")]
        public bool? ThongTinChiTietNhanVien { get; set; } = false; //Hiển thị nhập thông tin chi tiết nhân viên

        [Column(TypeName = "bit")]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public bool? BanHangOffline { get; set; } = false; //Sử dụng tính năng bán hàng offline

        [Column(TypeName = "int")]
        public int? ThoiGianNhacHanSuDungLo { get; set; } = 1; // Thời gian cảnh báo hạn sử dụng lô

        [Column(TypeName = "int")]
        public int? SuDungMaChungTu { get; set; } = 0; //Sử dụng thiết lập mã chứng từ (1 - Sử dụng, 0 - Không)

        [Column(TypeName = "int")]
        public int? ChoPhepTrungSoDienThoai { get; set; } = 0; //Trùng số điện thoại khi thêm khách hàng - đối tác (1 - Cho phép trùng, 0 - Không)

        public virtual DM_DonVi DM_DonVi { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_CauHinh_TichDiemChiTiet> HT_CauHinh_TichDiemChiTiet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HT_CauHinh_GioiHanTraHang> HT_CauHinh_GioiHanTraHang { get; set; }
    }
}
