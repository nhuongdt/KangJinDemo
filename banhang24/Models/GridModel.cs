using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace banhang24.Models
{
    public class GridModel
    {
       public int currentPage { get; set; }
        public int pageSize { get; set; }
        public string maHoaDon { get; set; }
        public int tonkho { get; set; }
        public int kinhdoanh { get; set; }
        public string loaihangs { get; set; }
        public string[] idnhomhang { get; set; }
        public Guid iddonvi { get; set; }
        public string[] listthuoctinh { get; set; }
        public string columsort { get; set; }
        public string sort { get; set; }
        public string ColumnHides { get; set; }
        public List<ColumSearch> listSearchColumn { get; set; }
    }

    public class GridModelHHMaVach
    {
        public List<DM_HangHoaDTO> listHH { get; set; }
        public bool InGia { get; set; }
        public bool InMaHH { get; set; }
        public bool InTenHH { get; set; }
        public bool InTenCH { get; set; }
        public Guid ID_BangGia { get; set; }
        public int SoBanGhi { get; set; }
    }
    public class RoleParentView
    {
        public Guid id { get; set; }
        public string text { get; set; }
        public Guid? parentId { get; set; }
        public Guid? ID_DonVi { get; set; }
        public string parentText { get; set; }
        public List<RoleParentView> children { get; set; }

    }
    public class array_LichSuThaoTac
    {
        public string ID_NhanVien { get; set; }
        public Guid ID_ChiNhanh { get; set; }
        public string NoiDung { get; set; }
        public string ChucNang { get; set; }
        public DateTime timeStart { get; set; }
        public DateTime timeEnd { get; set; }
        public string ThaoTac { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string columnsHide { get; set; }
        public string txtTime { get; set; }
        public string nameChiNhanh { get; set; }
        public string XemDS_HeThong { get; set; }
        public string XemDS_PhongBan { get; set; }
        public Guid ID_NguoiDung { get; set; }
    }
    public class array_SaoChepChietKhau
    {
        public Guid ID_DonVi{ get; set; }
        public Guid ID_NhanVien { get; set; }
        public string ID_NhanVien_new { get; set; }
        public int PhuongThuc { get; set; }
    }
    //trinhpv Class report
    public class array_TongQuan
    {
        public DateTime dayStart { get; set; }
        public DateTime dayEnd { get; set; }
        public Guid ID_NguoiDung { get; set; }
        public string ID_DonVi { get; set; }
    }
   
    public class array_BaoCaoNhapHang
    {
        public string MaHangHoa { get; set; }
        public string MaKhachHang { get; set; }
        public DateTime timeStart { get; set; }
        public DateTime timeEnd { get; set; }
        public string ID_ChiNhanh { get; set; }
        public int LaHangHoa { get; set; }
        public int TinhTrang { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public string ID_NhomDoiTuong { get; set; }
        public Guid ID_NguoiDung { get; set; }
        public string columnsHide { get; set; }
        public string TodayBC { get; set; }
        public string TenChiNhanh { get; set; }
        public List<string> lstIDChiNhanh { get; set; }
    }
    public class array_BaoCaoTaiChinh
    {
        public string MaHangHoa { get; set; }
        public int year { get; set; }
        public DateTime timeStart { get; set; }
        public DateTime timeEnd { get; set; }
        public Guid ID_DonVi { get; set; }
        public string ID_ChiNhanh { get; set; }
        public int LoaiDoiTuong { get; set; }
        public string LoaiThuChi { get; set; }
        public string ID_NhomKhachHang { get; set; }
        public string ID_NhomNhaCungCap { get; set; }
        public int HachToanKD { get; set; }
        public int LoaiTien { get; set; }
        public string columnsHide { get; set; }
        public string TodayBC { get; set; }
        public string TenChiNhanh { get; set; }
        public List<string> lstIDChiNhanh { get; set; }
    }
    public class array_BaoCaoGoiDichVu
    {
        public string MaHangHoa { get; set; }
        public string MaKhachHang { get; set; }
        public DateTime timeStart { get; set; }
        public DateTime timeEnd { get; set; }
        public string ID_ChiNhanh { get; set; }
        public int LaHangHoa { get; set; }
        public int TinhTrang { get; set; }
        public int ThoiHanSuDung { get; set; }
        public Guid? ID_NhomHang { get; set; }
        public Guid ID_NguoiDung { get; set; }
        public string columnsHide { get; set; }
        public string TodayBC { get; set; }
        public string TenChiNhanh { get; set; }
        public List<string> lstIDChiNhanh { get; set; }
    }
    public class array_BaoCaoNhanVien
    {
        public string MaNhanVien { get; set; }
        public string ID_ChiNhanh { get; set; }
        public DateTime timeCreate_Start { get; set; }
        public DateTime timeCreate_End { get; set; }
        public Guid? ID_PhongBan { get; set; }
        public string GioiTinh { get; set; }
        public string LoaiHopDong { get; set; }
        public DateTime timeBirthday_Start { get; set; }
        public DateTime timeBirthday_End { get; set; }
        public string LoaiChinhTri { get; set; }
        public string LoaiBaoHiem { get; set; }
        public string LoaiDanToc { get; set; }
        public string TrangThai { get; set; }
        public string columnsHide { get; set; }
        public string TodayBC { get; set; }
        public string TenChiNhanh { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public Guid ID_DonVi { get; set; }
    }
    public class array_NhanVienPhanCa
    {
        public string MaNhanVien { get; set; }
        public Guid? ID_ChiNhanh { get; set; }
        public Guid? ID_PhongBan { get; set; }
        public int LoaiCa { get; set; }
        public int TrangThai { get; set; }
        public DateTime TuNgay { get; set; }
    }
    // optinForm
    public class array_DoiTuongOptinForm
    {
        public string ID_OptinFrom { get; set; }
        public string TenKhachHang { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public int LoaiOptinForm { get; set; }
        public string TrangThaiXuLy { get; set; }
        public string TrangThaiChuaXuLy { get; set; }
        public string TrangThaiHuyBo { get; set; }
        public string TrangThaiFromBat { get; set; }
        public string TrangThaiFromTat { get; set; }
        public string TrangThaiFromXoa { get; set; }
        public int paperSize { get; set; }
        public string columnsHide { get; set; }
        public string TodayBC { get; set; }
        public string TenChiNhanh { get; set; }
    }
}