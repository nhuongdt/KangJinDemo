namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190112_1 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[importNS_NhanVien_ThongTinCoBan]
    @MaNhanVien [nvarchar](max),
    @TenNhanVien [nvarchar](max),
    @TenNhanVienKhongDau [nvarchar](max),
    @TenNhanVienKyTuDau [nvarchar](max),
    @GioiTinh [bit],
    @HonNhan [bit],
    @NgaySinh [nvarchar](max),
    @DiDong [nvarchar](max),
    @DienThoai [nvarchar](max),
    @Email [nvarchar](max),
    @NoiSinh [nvarchar](max),
    @CMND [nvarchar](max),
    @NgayCapCMND [nvarchar](max),
    @NoiCapCMND [nvarchar](max),
    @GhiChu [nvarchar](max),
    @ID_DonVi [uniqueidentifier],
    @TrangThai [bit]
	
AS
BEGIN
		DECLARE @ID_PhongBan uniqueidentifier
    	Set @ID_PhongBan = (select TOP 1 ID from NS_PhongBan where ID_DonVi is NULL)
		DECLARE @ID_NhanVien uniqueidentifier
		Set @ID_NhanVien = NEWID();
		insert into NS_NhanVien(ID, MaNhanVien, TenNhanVien, TenNhanVienKhongDau,TenNhanVienChuCaiDau, GioiTinh,TinhTrangHonNhan, NgaySinh, DienThoaiDiDong, DienThoaiNhaRieng, Email, NoiSinh, SoCMND, NgayCap, NoiCap, GhiChu, NguoiTao,NgayTao, DaNghiViec)
    		values(@ID_NhanVien, @MaNhanVien, @TenNhanVien, @TenNhanVienKhongDau,@TenNhanVienKyTuDau, @GioiTinh, @HonNhan, @NgaySinh, @DiDong, @DienThoai, @Email, @NoiSinh, @CMND, @NgayCapCMND, @NoiCapCMND, @GhiChu, 'admin',GETDATE(), @TrangThai);
		insert into NS_QuaTrinhCongTac (ID, ID_NhanVien, ID_DonVi, NgayApDung, LaChucVuHienThoi, LaDonViHienThoi, ID_PhongBan)
			values (NEWID(), @ID_NhanVien, @ID_DonVi, GETDATE(), '0', '1', @ID_PhongBan)
END");
            
        }
        
        public override void Down()
        {
        }
    }
}
