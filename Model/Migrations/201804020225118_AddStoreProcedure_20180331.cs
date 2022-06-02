namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreProcedure_20180331 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[import_DoiTuong]", parametersAction: p => new
            {
                MaNhomDoiTuong = p.String(),
                TenNhomDoiTuong = p.String(),
                MaDoiTuong = p.String(),
                TenDoiTuong = p.String(),
                TenDoiTuong_KhongDau = p.String(),
                TenDoiTuong_ChuCaiDau = p.String(),
                GioiTinhNam = p.Boolean(),
                LoaiDoiTuong = p.Int(),
                LaCaNhan = p.Int(),
                timeCreate = p.DateTime(),
                NgaySinh_NgayTLap = p.DateTime(),
                DiaChi = p.String(),
                Email = p.String(),
                Fax = p.String(),
                web = p.String(),
                GhiChu = p.String(),
                DienThoai = p.String(),
                MaSoThue = p.String(),
                STK = p.String(),
                MaHoaDonThu = p.String(),
                MaHoaDonChi = p.String(),
                ID_NhanVien = p.Guid(),
                ID_DonVi = p.Guid(),
                NoCanThu = p.Double(),
                NoCanTra = p.Double()
            }, body: @"DECLARE @ID_NhomDoiTuong  as uniqueidentifier
	set @ID_NhomDoiTuong = null
	DECLARE @ID  as uniqueidentifier
	SET @ID = NewID();
	DECLARE @ID_QuyHD  as uniqueidentifier
	SET @ID_QuyHD = NewID();
	if (len(@MaNhomDoiTuong) > 0)
	Begin
		SET @ID_NhomDoiTuong =  (Select ID FROM DM_NhomDoiTuong where TenNhomDoiTuong like @TenNhomDoiTuong and LoaiDoiTuong = @LoaiDoiTuong);
		if (@ID_NhomDoiTuong is null or len(@ID_NhomDoiTuong) = 0)
		BeGin
			SET @ID_NhomDoiTuong = newID();
			insert into DM_NhomDoiTuong (ID, MaNhomDoiTuong, TenNhomDoiTuong, LoaiDoiTuong)
			values (@ID_NhomDoiTuong, @MaNhomDoiTuong, @TenNhomDoiTuong, @LoaiDoiTuong)
		End
	End
    insert into DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, ID_NhomDoiTuong, MaDoiTuong, TenDoiTuong,TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau,DienThoai, Fax,
	Email, Website, MaSoThue, TaiKhoanNganHang, GhiChu, NgaySinh_NgayTlap, chiase, theodoi, DiaChi, GioiTinhNam, NguoiTao, NgayTao)
    Values (@ID, @LoaiDoiTuong, @LaCaNhan,@ID_NhomDoiTuong, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau, @TenDoiTuong_ChuCaiDau, @DienThoai, @Fax,
	@Email, @web, @MaSoThue,@STK, @GhiChu, @NgaySinh_NgayTLap, '0', '0',@DiaChi, @GioiTinhNam, 'admin', @timeCreate)
	if (@NoCanThu > 0)
	Begin
		insert into Quy_HoaDon (ID, MaHoaDon, NgayLapHoaDon, NgayTao, ID_NhanVien, NguoiNopTien, TongTienThu, ThuCuaNhieuDoiTuong, NguoiTao, ID_DonVi, LoaiHoaDon)
		values (@ID_QuyHD,@MaHoaDonChi,@timeCreate,@timeCreate,@ID_NhanVien,@TenDoiTuong, @NoCanThu,'0', 'admin', @ID_DonVi, '12')
		insert into Quy_HoaDon_ChiTiet (ID, ID_HoaDon, ID_DoiTuong, ThuTuThe, TienMat, TienGui, TienThu)
		values (newID(), @ID_QuyHD, @ID, '0', @NoCanThu, '0', @NoCanThu)
	End
	if (@NoCanTra > 0)
	Begin
		insert into Quy_HoaDon (ID, MaHoaDon, NgayLapHoaDon, NgayTao, ID_NhanVien, NguoiNopTien, TongTienThu, ThuCuaNhieuDoiTuong, NguoiTao, ID_DonVi, LoaiHoaDon)
		values (@ID_QuyHD,@MaHoaDonThu,@timeCreate,@timeCreate,@ID_NhanVien,@TenDoiTuong, @NoCanTra,'0', 'admin', @ID_DonVi, '11')
		insert into Quy_HoaDon_ChiTiet (ID, ID_HoaDon, ID_DoiTuong, ThuTuThe, TienMat, TienGui, TienThu)
		values (newID(), @ID_QuyHD, @ID, '0', @NoCanTra, '0', @NoCanTra)
	End");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[import_DoiTuong]");
        }
    }
}