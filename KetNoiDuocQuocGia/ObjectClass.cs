using System;
using System.Collections.Generic;
using System.Text;

namespace KetNoiDuocQuocGia
{
    public class UserDangNhap
    {
        public string usr { get; set; }
        public string pwd { get; set; }
    }

    public class TokenObject
    {
        public string token { get; set; }
        public string token_type { get; set; }
    }

    public class ThongTinDonVi
    {
        public string ma_co_so_kcb { get; set; }
        public string ten_co_so_kcb { get; set; }
    }
    public class ThongTinBenhNhan
    {
        public string ma_benh_nhan { get; set; }
        public string ho_ten { get; set; }
        public int tuoi { get; set; }
        public int gioi_tinh { get; set; } //1- nam, 2- nữ, 3- khác
        public string dia_chi { get; set; }
    }
    public class ThongTinBenh
    {
        public string ma_benh { get; set; }
        public string ten_benh { get; set; }
    }
    public class ThongTinDonThuoc
    {
        public string ma_thuoc { get; set; }
        public string ten_thuoc { get; set; }
        public string don_vi_tinh { get; set; }
        public string ham_luong { get; set; }
        public string duong_dung { get; set; }
        public string lieu_dung { get; set; }
        public string so_dang_ky { get; set; }
        public int so_luong { get; set; }
    }

    public class DonThuocObj
    {
        public string ma_don_thuoc_co_so_kcb { get; set; }
        public ThongTinDonVi thong_tin_don_vi { get; set; }
        public ThongTinBenhNhan thong_tin_benh_nhan { get; set; }
        public ThongTinBenh thong_tin_benh { get; set; }
        public List<ThongTinDonThuoc> thong_tin_don_thuoc { get; set; }
        public string nguoi_ke_don { get; set; }
        public string ngay_ke_don { get; set; } //Định dang yyyyMMddHHmm
    }

    public class ResultApiDonThuoc
    {
        public string code { get; set; }
        public string mess { get; set; }
        public string ma_don_thuoc_quoc_gia { get; set; }
    }

    public class HoaDonChiTiet
    {
        public string ma_thuoc { get; set; }
        public string ten_thuoc { get; set; }
        public string so_lo { get; set; }
        public string ngay_san_xuat { get; set; } //định dạng yyyyMMdd
        public string han_dung { get; set; } //định dạng yyyyMMdd
        public string don_vi_tinh { get; set; }
        public string ham_luong { get; set; }
        public string duong_dung { get; set; }
        public string lieu_dung { get; set; }
        public string so_dang_ky { get; set; }
        public int so_luong { get; set; }
        public int don_gia { get; set; }
        public int thanh_tien { get; set; }
        public int ty_le_quy_doi { get; set; }
    }

    public class HoaDonObj
    {
        public string ma_hoa_don { get; set; }
        public string ma_co_so { get; set; }
        public string ma_don_thuoc_quoc_gia { get; set; }
        public string ngay_ban { get; set; } //định dang yyyyMMdd
        public string ho_ten_nguoi_ban { get; set; }
        public string ho_ten_khach_hang { get; set; }
        public List<HoaDonChiTiet> hoa_don_chi_tiet { get; set; }
    }

    public class ResultApiHoaDon
    {
        public string ma_hoa_don { get; set; }
        public string code { get; set; }
        public string mess { get; set; }
    }

    public class NhapXuatChiTiet
    {
        public string ma_thuoc { get; set; }
        public string ten_thuoc { get; set; }
        public string so_lo { get; set; }
        public string ngay_san_xuat { get; set; } //định dạng yyyyMMdd
        public string han_dung { get; set; } //định dạng yyyyMMdd
        public string so_dklh { get; set; } //số đăng ký lưu hành
        public int so_luong { get; set; }
        public int don_gia { get; set; }
        public string don_vi_tinh { get; set; }
    }

    public class PhieuNhapObj
    {
        public string ma_phieu { get; set; }
        public string ma_co_so { get; set; }
        public string ngay_nhap { get; set; } //định dạng yyyyMMdd
        public int loai_phieu_nhap { get; set; } //1- nhập từ nhà cung cấp, 2- khách trả, 3- nhập tồn
        public string ghi_chu { get; set; }
        public string ten_co_so_cung_cap { get; set; }
        public List<NhapXuatChiTiet> chi_tiet { get; set; }
    }

    public class PhieuXuatObj
    {
        public string ma_phieu { get; set; }
        public string ma_co_so { get; set; }
        public string ngay_xuat { get; set; } //định dạng yyyyMMdd
        public int loai_phieu_xuat { get; set; } //1- nhập từ nhà cung cấp, 2- khách trả, 3- nhập tồn
        public string ghi_chu { get; set; }
        public string ten_co_so_nhan { get; set; }
        public List<NhapXuatChiTiet> chi_tiet { get; set; }
    }

    public class ThuocCoSo
    {
        public string ma_co_so { get; set; }
        public string ten_thuoc { get; set; }
        public string so_dang_ky { get; set; }
        public string ten_hoat_chat { get; set; }
        public string ham_luong { get; set; }
        public string dong_goi { get; set; }
        public string hang_san_xuat { get; set; }
        public string nuoc_san_xuat { get; set; }
        public string don_vi_tinh { get; set; }
    }
}
