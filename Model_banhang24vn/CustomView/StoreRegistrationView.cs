using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model_banhang24vn.CustomView
{
   public class StoreRegistrationView
    {
        public string Sub { get; set; }

        public string Key { get; set; }

        public string Mobile { get; set; }

        public string Name { get; set; }

        public string SubDomain { get; set; }

        public string Business { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool? Status { get; set; }

        public string TenCuaHang { get; set; }

        public string DiaChi { get; set; }

        public string Email { get; set; }

        public DateTime? NgayTao { get; set; }

        public string MaKichHoat { get; set; }

        public int? SoLanKichHoat { get; set; }

        public string Quanhuyen { get; set; }

        public string ThietBi_DK { get; set; }
        public string KhuVuc_DK { get; set; }
        public string HeDieuHanh_DK { get; set; }
        public string DiaChiIP_DK { get; set; }

        public string GoiDichVu { get; set; }
        public string ID_GoiDichVu { get; set; }

        public int? Version { get; set; }
        public string  VersionText { get; set; }
        public string ThoGianGiaHan { get; set; }
        public string GhiChu { get; set; }
    }
    public class ExportCuaHangDangKy
    {
        public string SoDienThoaiDangKy { get; set; }
        public string TenCuaHang { get; set; }
        public string TenGianHangDangKy { get; set; }
        public string HoTenKhachHang { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string NganhNgheKinhDoanh { get; set; }
        public string GoiDichVu { get; set; }
        public int? SoLanKichHoat { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgayDangKy { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string ThietBi_DK { get; set; }
        public string KhuVuc_DK { get; set; }
        public string HeDieuHanh_DK { get; set; }
        public string DiaChiIP_DK { get; set; }
        public string PhienBan { get; set; }
        public string GhiChu { get; set; }
    }
    public class DichVuNapTien
    {
        public Guid ID { get; set; }
        public string TenKhachNap { get; set; }
        public string SubDoamin { get; set; }
        public string SoDienThoai { get; set; }
        public string GhiChu { get; set; }
        public decimal? SoTien { get; set; }
        public DateTime NgayTao { get; set; }
        public int? TrangThai { get; set; }
        public string TenCuaHang { get; set; }
    }
}
