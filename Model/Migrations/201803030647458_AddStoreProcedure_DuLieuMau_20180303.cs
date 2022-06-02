namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreProcedure_DuLieuMau_20180303 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[insert_DoiTuong]", parametersAction: p => new
            {
                ID = p.Guid(),
                MaDoiTuong = p.String(),
                TenDoiTuong = p.String(),
                TenDoiTuong_KhongDau = p.String(),
                TenDoiTuong_ChuCaiDau = p.String(),
                GioiTinhNam = p.Boolean(),
                LoaiDoiTuong = p.Int(),
                LaCaNhan = p.Int(),
                timeCreate = p.DateTime()
            }, body: @"insert into DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong,TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, chiase, theodoi, GioiTinhNam, NguoiTao, NgayTao)
    Values (@ID, @LoaiDoiTuong, @LaCaNhan, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau, @TenDoiTuong_ChuCaiDau, '0', '0', @GioiTinhNam, 'admin', @timeCreate)");

            CreateStoredProcedure(name: "[dbo].[insert_DonViQuiDoi]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_HangHoa = p.Guid(),
                MaHangHoa = p.String(),
                GiaVon = p.Double(),
                GiaBan = p.Double(),
                timeCreate = p.DateTime()
            }, body: @"insert into DonViQuiDoi (ID, MaHangHoa, ID_HangHoa, TyLeChuyenDoi, LaDonViChuan, GiaVon, GiaNhap, GiaBan, NguoiTao, NgayTao)
	Values (@ID, @MaHangHoa,@ID_HangHoa, '1', '1', @GiaVon, '0', @GiaBan, 'admin', @timeCreate)");

            CreateStoredProcedure(name: "[dbo].[insert_HangHoa]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_NhomHang = p.Guid(),
                TenHangHoa = p.String(),
                TenHangHoa_KhongDau = p.String(),
                TenHangHoa_KyTuDau = p.String(),
                LaHangHoa = p.Boolean(),
                timeCreate = p.DateTime()
            }, body: @"insert into DM_HangHoa (ID, TenHangHoa, LaHangHoa, ID_NhomHang, ID_HangHoaCungLoai, QuyCach, ChiPhiThucHien, ChiPhiTinhTheoPT, TheoDoi, NguoiTao, NgayTao, DuocBanTrucTiep, TenHangHoa_KhongDau, TenHangHoa_KyTuDau, LaChaCungLoai)
    Values(@ID, @TenHangHoa, @LaHangHoa, @ID_NhomHang, NEWID(), '0', '0','1','1', 'admin', @timeCreate, '1', @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau, '1')");

            CreateStoredProcedure(name: "[dbo].[insert_HoaDon]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_DoiTuong = p.Guid(),
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                MaHoaDon = p.String(),
                LoaiHoaDon = p.Int(),
                TongTienHang = p.Double(),
                timeCreate = p.DateTime()
            }, body: @"insert into BH_HoaDon (ID, MaHoaDon, NgayLapHoaDon,ID_DoiTuong,ID_NhanVien, LoaiHoaDon, ChoThanhToan, TongTienHang, 
	TongChietKhau, TongTienThue, TongGiamGia, TongChiPhi, PhaiThanhToan, DienGiai, NguoiTao, NgayTao, ID_DonVi, TyGia)
    Values (@ID, @MaHoaDon, @timeCreate, @ID_DoiTuong, @ID_NhanVien, @LoaiHoaDon, '0', @TongTienHang, '0', '0', '0', '0', @TongTienHang, '', 'admin', @timeCreate, @ID_DonVi, '1')");

            CreateStoredProcedure(name: "[dbo].[insert_HoaDon_ChiTiet]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                SoLuong = p.Double(),
                DonGia = p.Double(),
                ThanhTien = p.Double(),
                GiaVon = p.Double()
            }, body: @"insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi)
    Values (NEWID(), @ID_HoaDon, '0', @SoLuong, @DonGia, @ThanhTien, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi)");

            CreateStoredProcedure(name: "[dbo].[insert_NhomHangHoa]", parametersAction: p => new
            {
                ID = p.Guid(),
                MaNhomHangHoa = p.String(),
                TenNhomHangHoa = p.String(),
                timeCreate = p.DateTime()
            }, body: @"Insert into DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa, laNhomhanghoa, HienThi_Chinh, HienThi_Phu, HienThi_BanThe, NguoiTao, NgayTao)
	values(@ID, @MaNhomHangHoa, @TenNhomHangHoa, '1', '1', '1', '1','admin', @timeCreate) ");

            CreateStoredProcedure(name: "[dbo].[insert_Quy_HoaDon]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                MaHoaDon = p.String(),
                LoaiHoaDon = p.Int(),
                TongTienThu = p.Double(),
                tiemCreate = p.DateTime()
            }, body: @"insert into Quy_HoaDon (ID, MaHoaDon, NgayLapHoaDon, NgayTao, ID_NhanVien, NguoiNopTien, NoiDungThu, TongTienThu, ThuCuaNhieuDoiTuong, Nguoitao, ID_DonVi, LoaiHoaDon)
    Values (@ID, @MaHoaDon, @tiemCreate, @tiemCreate, @ID_NhanVien, '', '',  @TongTienThu, '0', 'admin', @ID_DonVi, @LoaiHoaDon)");

            CreateStoredProcedure(name: "[dbo].[insert_Quy_HoaDon_ChiTiet]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid(),
                ID_HoaDonLienQuan = p.Guid(),
                ID_DoiTuong = p.Guid(),
                ID_NhanVien = p.Guid(),
                TienThu = p.Double()
            }, body: @"insert into Quy_HoaDon_ChiTiet (ID, ID_HoaDon, ID_NhanVien, ID_DoiTuong, ThuTuThe, TienMat, TienGui, TienThu, ID_HoaDonLienQuan)
    Values (NEWID(), @ID_HoaDon,@ID_NhanVien,@ID_DoiTuong, '0', @TienThu, '0', @TienThu, @ID_HoaDonLienQuan)");

            CreateStoredProcedure(name: "[dbo].[XoaDuLieuHeThong]", parametersAction: p => new
            {
                CheckHH = p.Int(),
                CheckKH = p.Int()
            }, body: @"delete HT_Quyen_Nhom where ID_NhomNguoiDung != (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = (select Top(1)ID from HT_NguoiDung order by NgayTao))
			delete from HT_NhomNguoiDung where ID != (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = (select Top(1)ID from HT_NguoiDung order by NgayTao))
			delete from HT_NguoiDung_Nhom where IDNguoiDung != (select Top(1)ID from HT_NguoiDung order by NgayTao)  
			delete from HT_NguoiDung where ID !=(select Top(1)ID from HT_NguoiDung order by NgayTao)  
			delete from chotso
			delete from chotso_hanghoa
			delete from chotso_khachHang
			delete from DM_KhuyenMai_ApDung
			delete from DM_GiaBan_ApDung
			delete from DM_GiaBan_ChiTiet
			delete from DM_GiaBan
			delete from Quy_KhoanThuChi
			delete from Quy_HoaDon_ChiTiet
			delete from Quy_HoaDon
			delete from BH_NhanVienThucHien
			delete from BH_HoaDon_ChiTiet
			delete from BH_HoaDon
			if(@CheckKH =0)
			BEGIN
				delete from DM_NhomDoiTuong	
				delete from DM_DoiTuong
			END
			delete from DM_KhuyenMai_ChiTiet
			delete from DM_KhuyenMai_ApDung
			delete from DM_KhuyenMai
			delete from ChietKhauMacDinh_NhanVien
			
			if(@CheckHH = 0)
			BEGIN
				delete from DonViQuiDoi
				delete from DM_ThuocTinh
				delete from HangHoa_ThuocTinh
				delete from DM_HangHoa_Anh
				delete from DM_HangHoa
				delete from DM_NhomHangHoa
				delete from DM_ThuocTinh
				delete from DinhLuongDichVu
			END
			
			delete from DM_ViTri
			delete from DM_KhuVuc
			delete from NS_QuaTrinhCongTac where ID_NhanVien ! = (select Top(1)ID_NhanVien from HT_NguoiDung order by NgayTao)
			delete from NS_NhanVien where ID !=(select Top(1)ID_NhanVien from HT_NguoiDung order by NgayTao)
			delete from HT_NhatKySuDung where LoaiNhatKy != 20
			delete from Kho_DonVi where ID_DonVi != (select Top(1)ID from DM_DonVi order by NgayTao)
			delete from DM_Kho where ID != (select ID_Kho from Kho_DonVi where ID_DonVi = (select Top(1)ID from DM_DonVi order by NgayTao) )
			delete from DM_DonVi where ID !=(select Top(1)ID from DM_DonVi order by NgayTao)
			delete from ChamSocKhachHangs
			delete from ChietKhauMacDinh_NhanVien
			delete from CongDoan_DichVu
			delete from CongNoDauKi
			delete from DanhSachThi_ChiTiet	
			delete from DanhSachThi
			delete from DM_ChucVu
			delete from DM_HinhThucThanhToan
			delete from DM_HinhThucVanChuyen
			delete from DM_KhoanPhuCap
			delete from DM_LienHe
			delete from DM_LoaiGiaPhong
			delete from DM_LoaiNhapXuat
			delete from DM_LoaiPhieuThanhToan
			delete from DM_LoaiPhong
			delete from DM_LoaiTuVanLichHen
			delete from DM_LoHang
			delete from DM_LopHoc
			delete from DM_LyDoHuyLichHen
			delete from DM_MaVach
			delete from DM_MayChamCong
			delete from DM_NganHang
			delete from DM_MayChamCong
			delete from DM_NoiDungQuanTam
			delete from DM_PhanLoaiHangHoaDichVu
			delete from DM_NguonKhachHang
			delete from DM_ThueSuat
			delete from DM_NguonKhachHang
			delete from HT_CauHinh_TichDiemApDung
			delete from HT_CauHinh_TichDiemChiTiet		
			delete from DM_TichDiem	
			delete from NhomDoiTuong_DonVi where ID_DonVi != (select Top(1)ID from DM_DonVi order by NgayTao)
			delete from NhomHangHoa_DonVi where ID_DonVi != (select Top(1)ID from DM_DonVi order by NgayTao)
			delete from NS_ChamCong_ChiTiet	
			delete from NS_LuongDoanhThu_ChiTiet 
			delete from NS_LuongDoanhThu
			delete from NS_HoSoLuong 
			delete from The_NhomThe
			delete from The_TheKhachHang_ChiTiet
			delete from The_TheKhachHang");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[insert_DoiTuong]");
            DropStoredProcedure("[dbo].[insert_DonViQuiDoi]");
            DropStoredProcedure("[dbo].[insert_HangHoa]");
            DropStoredProcedure("[dbo].[insert_HoaDon]");
            DropStoredProcedure("[dbo].[insert_HoaDon_ChiTiet]");
            DropStoredProcedure("[dbo].[insert_NhomHangHoa]");
            DropStoredProcedure("[dbo].[insert_Quy_HoaDon]");
            DropStoredProcedure("[dbo].[insert_Quy_HoaDon_ChiTiet]");
            DropStoredProcedure("[dbo].[XoaDuLieuHeThong]");
        }
    }
}
