using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class DM_DoiTuongDTO
    {
        public Guid ID { get; set; }
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string NhomDoiTuong { get; set; }
        public string DienThoai { get; set; }
        public bool? GioiTinh { get; set; }
        //public string NgaySinh { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string KhuVuc { get; set; }
        public string PhuongXa { get; set; }
        public double? NoHienTai { get; set; }
        public double? NoCanTra { get; set; }
        public double? TongBanTru { get; set; }
        public string MaSoThue { get; set; }
        public string GhiChu { get; set; }
        public Guid? ID_NguonKhach { get; set; }
        public Guid? ID_TinhThanh { get; set; }
        public Guid? ID_QuanHuyen { get; set; }
        public Guid? ID_NguoiGioiThieu { get; set; }
        public Guid? ID_NhanVienPhuTrach { get; set; }
        public string ID_NhomDoiTuong { get; set; }

        // HoaDon
        public string MaHoaDon { get; set; }
        public string ThoiGianLapHD { get; set; }
        public string NguoiLap { get; set; }
        public int LoaiHoaDon { get; set; }
        public double GiaTri { get; set; }
        public double? TongTichDiem { get; set; }
    }
    public class CommonParamSearch
    {
        public List<string> IDChiNhanhs { get; set; }
        public List<string> TrangThais { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string TextSearch { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
    }

    public class ParamNKyGDV : CommonParamSearch
    {
        public List<string> IDCustomers { get; set; }
        public List<string> IDCars { get; set; }
        public string LoaiHoaDons { get; set; }
    }

    public class Param_ReportText
    {
        public List<int> ColumnHide { get; set; }
        public string ReportName { get; set; }
        public string ReportTime { get; set; }
        public string ReportBranch { get; set; }// Tên chi nhánh
    }
}
