namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190424 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[SP_DeleteHoaDon_whenTimeout]
    @NgayLapHoaDon [nvarchar](max),
    @ID_DonVi [nvarchar](50),
    @LoaiHoaDon int,
    @UserLogin [nvarchar](50)

AS
BEGIN
		 declare @ID varchar(40)
    
    	SELECT @ID = ( Select top 1 ID from BH_HoaDon
    	WHERE NgayLapHoaDon =  convert(datetime, @NgayLapHoaDon) 
		AND ID_DonVi like @ID_DonVi AND LoaiHoaDon = @LoaiHoaDon AND NguoiTao like @UserLogin)    	
    
    	IF @ID IS NOT NULL 
    		--Update BH_HoaDon Set ChoThanhToan =null  where ID = @ID
			delete from BH_NhanVienThucHien where ID_HoaDon= @ID
			delete from BH_NhanVienThucHien where exists (select ID from BH_HoaDon_ChiTiet where ID = BH_NhanVienThucHien.ID_ChiTietHoaDon and ID_HoaDon= @ID)
    		delete from BH_HoaDon_ChiTiet where ID_HoaDon = @ID
    		delete from BH_HoaDon where ID = @ID
END

--SP_DeleteHoaDon_whenTimeout '2019-04-23 09:37:16','d93b17ea-89b9-4ecf-b242-d03b8cde71de',1,'admin'

");

            Sql(@"CREATE TRIGGER [dbo].[CallUpdateGiaVon] on [dbo].[HT_NhatKySuDung]
FOR INSERT
AS
BEGIN
	DECLARE @IDHoaDonInput UNIQUEIDENTIFIER;
	DECLARE @IDChiNhanhInput UNIQUEIDENTIFIER;
	DECLARE @ThoiGian DATETIME;
	DECLARE @LoaiHoaDon INT;
	DECLARE @YeuCau NVARCHAR(MAX);
	DECLARE @ChoThanhToan BIT;
	SELECT @LoaiHoaDon = LoaiHoaDon, @IDHoaDonInput = ID_HoaDon, @ThoiGian = ThoiGianUpdateGV FROM inserted
	IF(@IDHoaDonInput is not null)
	BEGIN
		IF(@LoaiHoaDon = 1 OR @LoaiHoaDon = 4 OR @LoaiHoaDon = 6 OR @LoaiHoaDon = 7 OR @LoaiHoaDon = 8 OR @LoaiHoaDon = 9 OR @LoaiHoaDon = 18)
		BEGIN
			SELECT @IDChiNhanhInput = ID_DonVi, @ChoThanhToan = ChoThanhToan FROM BH_HoaDon WHERE ID = @IDHoaDonInput
			exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian
		END
		IF(@LoaiHoaDon = 10)
		BEGIN
			DECLARE @IDCheckIn UNIQUEIDENTIFIER;
			SELECT @IDChiNhanhInput = ID_DonVi, @IDCheckIn = ID_CheckIn, @YeuCau = YeuCau FROM BH_HoaDon WHERE ID = @IDHoaDonInput
			IF(@YeuCau = '1' OR @YeuCau = '3')
			BEGIN
				exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian
			END
			IF(@YeuCau = '4')
			BEGIN
				exec UpdateGiaVonVer2 @IDHoaDonInput, @IDCheckIn, @ThoiGian
			END
		END
	END

END
");

            Sql(@"ALTER PROCEDURE [dbo].[insert_DM_GiaVon]
    @ID_DonViQuiDoi [uniqueidentifier],
    @ID_DonVi [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @GiaVon [float],
	@ID_NhanVien [uniqueidentifier]
AS
BEGIN
	DECLARE @MaHoaDon VARCHAR(10)
	DECLARE @ID_HoaDon as uniqueidentifier
	set @ID_HoaDon = NEWID()

	IF (SELECT COUNT(MaHoaDon) FROM BH_HoaDon where LoaiHoaDon = 18) = 0
	BEGIN
		SET @MaHoaDon = 'DCGV00001'
		insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
    	Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVon)

	Insert into BH_HoaDon (ID, MaHoaDon, LoaiHoaDon, ChoThanhToan, ID_DonVi, ID_NhanVien, NgayLapHoaDon, TongTienHang, TongChietKhau,
				TongTienThue, TongChiPhi,TongGiamGia,PhaiThanhToan, DienGiai, YeuCau, NguoiTao, NgayTao)
    Values(@ID_HoaDon, @MaHoaDon, '18', '0', @ID_DonVi, @ID_NhanVien, dateadd(hour, 7, GETUTCDATE()), '0', @GiaVon, @GiaVon, '0','0','0',
			N'Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ', N'Hoàn thành', 'admin', dateadd(hour, 7, GETUTCDATE()));

	Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang)
    Values(NEWID(), @ID_HoaDon, '1','0','0', @GiaVon, @GiaVon, '0', '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang)
		END
	ELSE
	BEGIN
		SELECT @MaHoaDon = MAX(RIGHT(MaHoaDon, 5)) FROM BH_HoaDon where LoaiHoaDon = 18
		SELECT @MaHoaDon = CASE
			WHEN @MaHoaDon >= 0 and @MaHoaDon < 9 THEN 'DCGV0000' + CONVERT(CHAR, CONVERT(INT, @MaHoaDon) + 1)
			WHEN @MaHoaDon >= 9 and @MaHoaDon < 19 THEN 'DCGV000' + CONVERT(CHAR, CONVERT(INT, @MaHoaDon) + 1)
			WHEN @MaHoaDon >= 9 and @MaHoaDon < 99 THEN 'DCGV000' + CONVERT(CHAR, CONVERT(INT, @MaHoaDon) + 1)
			WHEN @MaHoaDon >= 99 and @MaHoaDon < 999 THEN 'DCGV00' + CONVERT(CHAR, CONVERT(INT, @MaHoaDon) + 1)
			WHEN @MaHoaDon >= 999 and @MaHoaDon < 9999 THEN 'DCGV0' + CONVERT(CHAR, CONVERT(INT, @MaHoaDon) + 1)
			WHEN @MaHoaDon >= 9999 THEN 'DCGV' + CONVERT(CHAR, CONVERT(INT, @MaHoaDon) + 1)
	END
	--insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
 --   	Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVon)
 
	Insert into BH_HoaDon (ID, MaHoaDon, LoaiHoaDon, ChoThanhToan, ID_DonVi, ID_NhanVien, NgayLapHoaDon, TongTienHang, TongChietKhau,
				TongTienThue, TongChiPhi,TongGiamGia,PhaiThanhToan, DienGiai, YeuCau, NguoiTao, NgayTao)
    Values(@ID_HoaDon, @MaHoaDon, '18', '0', @ID_DonVi, @ID_NhanVien, dateadd(hour, 7, GETUTCDATE()), '0', @GiaVon, @GiaVon, '0','0','0',
			N'Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ', N'Hoàn thành', 'admin', dateadd(hour, 7, GETUTCDATE()));

	DECLARE @GiaVonCheck FLOAT; 
    	select @GiaVonCheck = COUNT(ID) from DM_GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoi and ID_DonVi = @ID_DonVi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL)
	IF(@GiaVonCheck = 0)
	BEGIN
		Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang)
		Values(NEWID(), @ID_HoaDon, '1','0','0', @GiaVon, @GiaVon, '0', '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang)
	END
	ELSE
	BEGIN
		DECLARE @GiaVonOld FLOAT; 
    			select @GiaVonOld = GiaVon from DM_GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoi and ID_DonVi = @ID_DonVi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL)
		Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang)
		Values(NEWID(), @ID_HoaDon, '1','0',@GiaVonOld, @GiaVon, (Case When @GiaVon - @GiaVonOld > 0 then @GiaVon - @GiaVonOld else 0 end), (Case When @GiaVon - @GiaVonOld > 0 then 0 else @GiaVon - @GiaVonOld end), '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang)
	END

	--tĩnh sửa ngày 17/10 nhân dịp anh trịnh đi vắng
				
    	IF(@GiaVonCheck = 0)
    	BEGIN
    		insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
    		Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVon)
    	END
    	ELSE
    	BEGIN
    		UPDATE DM_GiaVon SET GiaVon = @GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL) and ID_DonVi = @ID_DonVi
    	END
--end tĩnh sửa
    END
END

");
        }
        
        public override void Down()
        {
        }
    }
}
