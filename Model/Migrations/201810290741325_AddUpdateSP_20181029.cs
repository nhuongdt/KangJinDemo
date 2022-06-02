namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181029 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinBaoHiem]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                SoBaoHiem = p.String(),
                LoaiBaoHiem = p.Int(),
                NgayCap = p.String(),
                NgayHetHan = p.String(),
                NoiCap = p.String(),
                GhiChu = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	insert into NS_BaoHiem(ID, ID_NhanVien, SoBaoHiem, LoaiBaoHiem, NgayCap,NgayHetHan, NoiBaoHiem, GhiChu, TrangThai)
	values(NEWID(), @ID_NhanVien, @SoBaoHiem, @LoaiBaoHiem, @NgayCap, @NgayHetHan, @NoiCap, @GhiChu, '1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinChinhTri]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                NgayVaoDoan = p.String(),
                NoiVaoDoan = p.String(),
                NgayNhapNgu = p.String(),
                NgayXuatNgu = p.String(),
                NgayVaoDang = p.String(),
                NgayChinhThucVaoDang = p.String(),
                NgayRoiDang = p.String(),
                ThongTin = p.String(),
                LyDo = p.String()
            }, body: @"Update NS_NhanVien set NgayVaoDoan = @NgayVaoDoan, NoiVaoDoan = @NoiVaoDoan, NgayNhapNgu = @NgayNhapNgu, NgayXuatNgu = @NgayXuatNgu, NgayVaoDang = @NgayVaoDang, 
	NgayVaoDangChinhThuc = @NgayChinhThucVaoDang, NgayRoiDang = @NgayRoiDang, NoiSinhHoatDang = @ThongTin, GhiChuThongTinChinhTri = @LyDo where MaNhanVien = @MaNhanVien");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinGiaDinh]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                HoTen = p.String(),
                NgaySinh = p.Int(),
                NoiO = p.String(),
                QuanHe = p.String(),
                DiaChi = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	insert into NS_NhanVien_GiaDinh(ID, ID_NhanVien,HoTen, NgaySinh, NoiO, QuanHe, DiaChi, TrangThai)
	values(NEWID(), @ID_NhanVien, @HoTen, @NgaySinh,@NoiO,@QuanHe,@DiaChi,'1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinHopDong]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                SoHopDong = p.String(),
                LoaiHopDong = p.String(),
                NgayKy = p.String(),
                GhiChu = p.String(),
                ThoiHan = p.Double(),
                DonViThoiHan = p.Boolean()
            }, body: @"if (@LoaiHopDong = 1)
	BEGIN
		set @ThoiHan = 0;
		set @DonViThoiHan = 0;
	END
	DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	insert into NS_HopDong (ID, ID_NhanVien, SoHopDong, LoaiHopDong, NgayKy, GhiChu, ThoiHan, DonViThoiHan, TrangThai)
	values(NEWID(), @ID_NhanVien, @SoHopDong, @LoaiHopDong, @NgayKy, @GhiChu, @ThoiHan, @DonViThoiHan, '1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinKhenThuong]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                HinhThuc = p.String(),
                SoQuyetDinh = p.String(),
                NgayKy = p.String(),
                NoiDung = p.String(),
                GhiChu = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	insert into NS_KhenThuong(ID, ID_NhanVien, HinhThuc, SoQuyetDinh, NgayBanHang, NoiDung, GhiChu, TrangThai)
	values(NEWID(), @ID_NhanVien, @HinhThuc, @SoQuyetDinh,@NgayKy, @NoiDung, @GhiChu, '1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinKhoanLuong]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                MaLoaiLuong = p.String(),
                NgayApDung = p.String(),
                NgayKetThuc = p.String(),
                SoTien = p.String(),
                HeSo = p.String(),
                Bac = p.String(),
                NoiDung = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	DECLARE @ID_LoaiLuong uniqueidentifier
	set @ID_LoaiLuong = (select ID from NS_LoaiLuong where TenLoaiLuong = @MaLoaiLuong);
	DECLARE @SoTienF as float
    		set @SoTienF = (select CAST(ROUND(@SoTien, 2) as float))
    	DECLARE @HeSoF as float
    		set @HeSoF = (select CAST(ROUND(@HeSo, 2) as float))

	insert into NS_Luong_PhuCap(ID, ID_NhanVien, ID_LoaiLuong, NgayApDung, NgayKetThuc, SoTien, HeSo, Bac, NoiDung, TrangThai)
	values(NEWID(), @ID_NhanVien, @ID_LoaiLuong, @NgayApDung,@NgayKetThuc,@SoTienF, @HeSoF, @Bac, @NoiDung,'1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinMienGiamThue]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                KhoanMienGiam = p.String(),
                NgayApDung = p.String(),
                NgayKetThuc = p.String(),
                SoTien = p.String(),
                NoiDung = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	DECLARE @SoTienF as float
    		set @SoTienF = (select CAST(ROUND(@SoTien, 2) as float))
	insert into NS_MienGiamThue(ID, ID_NhanVien, KhoanMienGiam, NgayApDung, NgayKetThuc, SoTien, GhiChu, TrangThai)
	values(NEWID(), @ID_NhanVien, @KhoanMienGiam, @NgayApDung,@NgayKetThuc,@SoTienF,@NoiDung,'1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinQuaTrinhCongTac]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                TuNgay = p.String(),
                DenNgay = p.String(),
                CoQuan = p.String(),
                ViTri = p.String(),
                DiaChi = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	insert into NS_NhanVien_CongTac (ID, ID_NhanVien, TuNgay, DenNgay, CoQuan, ViTri, DiaChi, TrangThai)
	values(NEWID(), @ID_NhanVien, @TuNgay, @DenNgay,@CoQuan,@ViTri,@DiaChi,'1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinQuyTrinhDaoTao]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                TuNgay = p.String(),
                DenNgay = p.String(),
                NoiHoc = p.String(),
                NganhHoc = p.String(),
                HeDaoTao = p.String(),
                BangCap = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	insert into NS_NhanVien_DaoTao (ID, ID_NhanVien, TuNgay, DenNgay, NoiHoc, NganhHoc, HeDaoTao,BangCap, TrangThai)
	values(NEWID(), @ID_NhanVien, @TuNgay, @DenNgay,@NoiHoc,@NganhHoc,@HeDaoTao,@BangCap,'1')");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_ThongTinSucKhoe]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                NgayKham = p.String(),
                ChieuCao = p.String(),
                CanNang = p.String(),
                TinhTrangSK = p.String()
            }, body: @"DECLARE @ID_NhanVien uniqueidentifier
	set @ID_NhanVien = (select ID from NS_NhanVien where MaNhanVien = @MaNhanVien);
	DECLARE @ChieuCaoF as float
    		set @ChieuCaoF = (select CAST(ROUND(@ChieuCao, 2) as float))
	DECLARE @CanNangF as float
    		set @CanNangF = (select CAST(ROUND(@CanNang, 2) as float))
	insert into NS_NhanVien_SucKhoe (ID, ID_NhanVien,NgayKham, ChieuCao, CanNang, TinhHinhSucKhoe, TrangThai)
	values(NEWID(), @ID_NhanVien, @NgayKham, @ChieuCaoF, @CanNangF,@TinhTrangSK,'1')");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinBaoHiem]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinChinhTri]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinGiaDinh]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinHopDong]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinKhenThuong]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinKhoanLuong]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinMienGiamThue]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinQuaTrinhCongTac]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinQuyTrinhDaoTao]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_ThongTinSucKhoe]");
        }
    }
}