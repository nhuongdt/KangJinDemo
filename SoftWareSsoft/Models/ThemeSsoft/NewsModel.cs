using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoftWareSsoft.Models.ThemeSsoft
{
    public class NewsModel
    {
        public int ID { get; set; }
        public string TieuDe { get; set; }
        public string Mota { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayTao { get; set; }
        public string NguoiTao { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescriptions { get; set; }
    }
    public class RecruitmentModel: NewsModel
    {
        public string ThoiGian { get; set; }
        public int? SoLuong { get; set; }
        public string MucLuong { get; set; }
        public string TinhThanh { get; set; }
        public bool ConHan { get; set; }
        public int ID_nhombaiviet { get; set; }
        public string TenNhom { get; set; }
    }

    public class NewsObjectModel
    {
        public int ID { get; set; }
        public string TenBaiViet { get; set; }
        public string Anh { get; set; }
        public string NoiDung { get; set; }
        public string Mota { get; set; }
        public bool? TrangThai { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescriptions { get; set; }
        public int ID_NhomBaiViet { get; set; }
        public string Tags { get; set; }
        public DateTime? NgayDangBai { get; set; }
        public bool IsLichHen { get; set; }
        public string Link { get; set; }
        public bool IsNews { get; set; }
        public string TenNhom { get; set; }
    }

    public class RecruitmentObjectModel
    {
        public int ID { get; set; }
        public string TieuDe { get; set; }
        public string Mota { get; set; }
        public int? TrangThai { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescriptions { get; set; }
        public int ID_NhomBaiViet { get; set; }
        public string Tags { get; set; }
        public DateTime? NgayDangBai { get; set; }
        public bool IsLichHen { get; set; }
        public string Link { get; set; }
        public bool IsNews { get; set; }
        public string TenNhom { get; set; }
        public DateTime Tungay { get; set; }
        public DateTime Denngay { get; set; }
        public string DiaChi { get; set; }
        public int? Soluong { get; set; }
        public string MucLuong { get; set; }
        public string MaTinhThanh { get; set; }
    }
    public class CustomerObjectModel
    {
        public int ID { get; set; }
        public string TenKhach { get; set; }
        public string Anh { get; set; }
        public string NoiDung { get; set; }
        public string Mota { get; set; }
        public int? TrangThai { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescriptions { get; set; }
        public string  MaTinhThanh { get; set; }
        public string Tags { get; set; }
        public DateTime? NgayDangBai { get; set; }
        public bool IsLichHen { get; set; }
        public string Link { get; set; }
        public bool IsNews { get; set; }
        public string MaSanPham { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
    }

    public class GroupNewsObjectModel
    {
        public int ID { get; set; }
        public string Ten { get; set; }
        public string GhiChu { get; set; }
        public int? NhomID { get; set; }
        public int Loai { get; set; }
        public bool IsNews { get; set; }

    }
}