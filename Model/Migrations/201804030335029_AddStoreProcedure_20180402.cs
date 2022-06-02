namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreProcedure_20180402 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[import_DanhMucHangHoa]", parametersAction: p => new {
                TenNhomHangHoaCha = p.String(),
                MaNhomHangHoaCha = p.String(),
                timeCreateNHHCha = p.DateTime(),
                TenNhomHangHoa = p.String(),
                MaNhomHangHoa = p.String(),
                timeCreateNHH = p.DateTime(),
                LaHangHoa = p.Boolean(),
                timeCreateHH = p.DateTime(),
                TenHanghoa = p.String(),
                TenHangHoa_KhongDau = p.String(),
                TenHangHoa_KyTuDau = p.String(),
                GhiChu = p.String(),
                QuyCach = p.Double(),
                DuocBanTrucTiep = p.Boolean(),
                MaDonViCoBan = p.String(),
                MaHangHoa = p.String(),
                TenDonViTinh = p.String(),
                GiaVon = p.Double(),
                GiaBan = p.Double(),
                timeCreateDVQD = p.DateTime(),
                LaDonViChuan = p.Boolean(),
                TyLeChuyenDoi = p.Double(),
                MaHoaDon = p.String(),
                DienGiai = p.String(),
                TonKho = p.Double(),
                timeCreateHD = p.DateTime(),
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid()
            }, body: @"-- insert NhomHangHoa parent
    DECLARE @ID_NhomHangHoaCha  as uniqueidentifier
    	set @ID_NhomHangHoaCha = null
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin
    		SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha, @MaNhomHangHoaCha, '1', '1', '1', '1', 'admin', @timeCreateNHHCha, null)
    		End
    	End
-- insert NhomHangHoa
	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
		if (len(@TenNhomHangHoa) > 0)
    	Begin
    		SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @MaNhomHangHoa, '1', '1', '1', '1', 'admin', @timeCreateNHH, @ID_NhomHangHoaCha)
    		End
    	End
-- insert HangHoa
	DECLARE @ID_HangHoa  as uniqueidentifier
    	set @ID_HangHoa = newID();
	if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
	Begin
		insert into DM_HangHoa (ID, ID_HangHoaCungLoai, LaChaCungLoai, ID_NhomHang, LaHangHoa, NgayTao,NguoiTao, TenHangHoa,TenHangHoa_KhongDau, TenHangHoa_KyTuDau,
    		TenKhac, TheoDoi, GhiChu, ChiPhiThucHien, ChiPhiTinhTheoPT, QuyCach, DuocBanTrucTiep)
		Values (@ID_HangHoa, newID(), '1',@ID_NhomHangHoa, @LaHangHoa, @timeCreateHH, 'admin', @TenHangHoa, @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau,
    		'', '1', @GhiChu, '0', '1', @QuyCach, @DuocBanTrucTiep)
	end
	else
	Begin
		set @ID_HangHoa = (Select ID_HangHoa from DonViQuiDoi where MaHangHoa like @MaDonViCoBan);
	end
-- insert DonViQuiDoi
	DECLARE @ID_DonViQuiDoi  as uniqueidentifier
    	set @ID_DonViQuiDoi = newID();
	 insert into DonViQuiDoi (ID, MaHangHoa, TenDonViTinh, ID_HangHoa, TyLeChuyenDoi, LaDonViChuan, GiaVon, GiaNhap, GiaBan, NguoiTao, NgayTao)
    	Values (@ID_DonViQuiDoi, @MaHangHoa,@TenDonViTinh, @ID_HangHoa, @TyLeChuyenDoi, @LaDonViChuan, @GiaVon, '0', @GiaBan, 'admin', @timeCreateDVQD)
-- insert TonKho
	if (@TonKho > 0)
	Begin
	DECLARE @ID_HoaDon  as uniqueidentifier
    	set @ID_HoaDon = newID();
		insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
			values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, @timeCreateHD, @ID_DonVi, @ID_NhanVien, @TonKho, '0', @TonKho, '0', '0', '0', '0', '9')
	    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi)
			Values (NEWID(), @ID_HoaDon, '1', @TonKho, '0', @TonKho, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi)
	End");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[import_DanhMucHangHoa]");
        }
    }
}