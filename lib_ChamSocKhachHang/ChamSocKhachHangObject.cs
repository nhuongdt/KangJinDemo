using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib_ChamSocKhachHang
{
    internal class ChamSocKhachHangObject
    {
    }
    public class DatLichIn
    {
        public Guid IdChiNhanh { get; set; }
        public string TenKhachHang { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public DateTime? NgaySinh { get; set; }
        public string BienSo { get; set; } = "";
        public string LoaiXe { get; set; } = "";
        public DateTime ThoiGian { get; set; }

        public List<Guid> ListIdHangHoa { get; set; } = new List<Guid>();
        public List<Guid> ListIdNhanVien { get; set; } = new List<Guid>();
        public int LoaiDatLich { get; set; } = 1;
    }

    public class ParamGetListDatLich
    {
        public List<string> IdChiNhanhs { get; set; }
        public DateTime? ThoiGianFrom { get; set; }
        public DateTime? ThoiGianTo { get; set; }
        public List<int> TrangThais { get; set; }
        public string TextSearch { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

    public class GetListDatLichResult
    {
        public Guid Id { get; set; }
        public DateTime ThoiGian { get; set; }
        public Guid? IdKhachHang { get; set; }
        public string MaKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string TenKhachHang { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public Guid? IdXe { get; set; }
        public string BienSo { get; set; }
        public string MauXe { get; set; }
        public int TrangThai { get; set; }
        public string TenChiNhanh { get; set; }
        public Guid IdDonVi { get; set; }
        public string MaChiNhanh { get; set; }
        public int TotalRow { get; set; }
        public double TotalPage { get; set; }
    }

    public class HangHoaDatLichChiTiet
    {
        public Guid ID { get; set; }
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public int LoaiHangHoa { get; set; }
        public double DonGia { get; set; }
        public string URLAnh { get; set; }
    }

    public class NhanVienDatLichChiTiet
    {
        public Guid ID { get; set; }
        public string MaNhanVien { get; set; }
        public string TenNhanVien { get; set; }
    }
}
