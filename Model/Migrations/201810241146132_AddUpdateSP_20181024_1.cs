namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181024_1 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getlistNS_PhongBan]", body: @"select
Cast(ROUND(ROW_NUMBER () over (order by MaPhongBan), 0) as float) as STT, 
MaPhongBan, TenPhongBan from NS_PhongBan
ORDER BY MaPhongBan");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinCoBan]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                TenNhanVien = p.String(),
                TenNhanVienKhongDau = p.String(),
                TenNhanVienKyTuDau = p.String(),
                GioiTinh = p.Boolean(),
                HonNhan = p.Boolean(),
                NgaySinh = p.String(),
                DiDong = p.String(),
                DienThoai = p.String(),
                Email = p.String(),
                NoiSinh = p.String(),
                CMND = p.String(),
                NgayCapCMND = p.String(),
                NoiCapCMND = p.String(),
                GhiChu = p.String(),
                MaPhongBan = p.String(),
                TrangThai = p.Boolean()
            }, body: @"DECLARE @ID_PhongBan uniqueidentifier
	Set @ID_PhongBan = (select ID from NS_PhongBan where MaPhongBan = @MaPhongBan)
	if (@ID_PhongBan is not null or LEN(@ID_PhongBan) > 0)
	Begin
		insert into NS_NhanVien(ID, MaNhanVien, TenNhanVien, TenNhanVienKhongDau,TenNhanVienChuCaiDau, GioiTinh,TinhTrangHonNhan, NgaySinh, DienThoaiDiDong, DienThoaiNhaRieng, Email, NoiSinh, SoCMND, NgayCap, NoiCap, GhiChu, ID_NSPhongBan, NguoiTao,NgayTao, DaNghiViec)
		values(NEWID(), @MaNhanVien, @TenNhanVien, @TenNhanVienKhongDau,@TenNhanVienKyTuDau, @GioiTinh, @HonNhan, @NgaySinh, @DiDong, @DienThoai, @Email, @NoiSinh, @CMND, @NgayCapCMND, @NoiCapCMND, @GhiChu, @ID_PhongBan, 'admin',GETDATE(), @TrangThai);
	End	
	else
	Begin
		insert into NS_NhanVien(ID, MaNhanVien, TenNhanVien, TenNhanVienKhongDau,TenNhanVienChuCaiDau, GioiTinh,TinhTrangHonNhan, NgaySinh, DienThoaiDiDong, DienThoaiNhaRieng, Email, NoiSinh, SoCMND, NgayCap, NoiCap, GhiChu, ID_NSPhongBan, NguoiTao,NgayTao, DaNghiViec)
		values(NEWID(), @MaNhanVien, @TenNhanVien, @TenNhanVienKhongDau,@TenNhanVienKyTuDau, @GioiTinh, @HonNhan, @NgaySinh, @DiDong, @DienThoai, @Email, @NoiSinh, @CMND, @NgayCapCMND, @NoiCapCMND, @GhiChu, null, 'admin',GETDATE(), @TrangThai);
	End");

            CreateStoredProcedure(name: "[dbo].[SP_UpdateChietKhauNhanVien_StringSQL]", parametersAction: p => new
            {
                stringSQL = p.String()
            }, body: @"EXEC (@stringSQL)");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getlistNS_PhongBan]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinCoBan]");
        }
    }
}
