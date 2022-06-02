namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20190725 : DbMigration
    {
        public override void Up()
        {
            Sql(@"update ChietKhauMacDinh_NhanVien set ChietKhau_BanGoi = ChietKhau_yeucau;update ChietKhauMacDinh_NhanVien set LaPhanTram_BanGoi = LaPhanTram_yeucau;
update BH_NhanVienThucHien set TinhChietKhauTheo = 4 where TheoYeuCau = 1 and id_chitiethoadon is not null;
update BH_NhanVienThucHien set TheoYeuCau = 0 where TinhChietKhauTheo = 4;");

            Sql(@"ALTER FUNCTION [dbo].[FUNC_GetStartChar](@stringToSplit NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN 
set @stringToSplit= rtrim(ltrim(@stringToSplit))
DECLARE @returnList TABLE ([Name] [nvarchar] (500))   
DECLARE @name NVARCHAR(255)
DECLARE @pos INT

 WHILE CHARINDEX(' ', @stringToSplit) > 0
 BEGIN
  SELECT @pos  = CHARINDEX(' ', @stringToSplit)  
  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)

  INSERT INTO @returnList  
  SELECT @name

  SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
 END

 INSERT INTO @returnList
 SELECT @stringToSplit

 RETURN (Select dbo.Func_ConvertStringToUnsign(Left(Name, 1))AS [text()]
    								From @returnList
    								For XML PATH (''))
END

");

            CreateStoredProcedure(name: "[dbo].[Update_ChietKhau_BanGoiByID]", parametersAction: p => new
            {
                ID = p.Guid(),
                ChietKhau_BanGoi = p.Double(),
                LaPhanTram_BanGoi = p.Boolean()
            }, body: @"update ChietKhauMacDinh_NhanVien set ChietKhau_BanGoi = @ChietKhau_BanGoi, LaPhanTram_BanGoi = @LaPhanTram_BanGoi where ID = @ID");


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
	DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
	SELECT @ID_HangHoa = ID_HangHoa FROM DonViQuiDoi WHERE ID = @ID_DonViQuiDoi;

	IF (SELECT COUNT(MaHoaDon) FROM BH_HoaDon where LoaiHoaDon = 18) = 0
	BEGIN
		SET @MaHoaDon = 'DCGV00001'
		insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
    	Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVon)

	Insert into BH_HoaDon (ID, MaHoaDon, LoaiHoaDon, ChoThanhToan, ID_DonVi, ID_NhanVien, NgayLapHoaDon, TongTienHang, TongChietKhau,
				TongTienThue, TongChiPhi,TongGiamGia,PhaiThanhToan, DienGiai, YeuCau, NguoiTao, NgayTao)
    Values(@ID_HoaDon, @MaHoaDon, '18', '0', @ID_DonVi, @ID_NhanVien, GETDATE(), '0', @GiaVon, @GiaVon, '0','0','0',
			N'Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ', N'Hoàn thành', 'admin', GETDATE());

	Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang, TonLuyKe)
    Values(NEWID(), @ID_HoaDon, '1','0','0', @GiaVon, @GiaVon, '0', '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang, dbo.FUNC_TonLuyKeTruocThoiGian(@ID_DonVi, @ID_HangHoa, @ID_LoHang, GETDATE()));
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
    Values(@ID_HoaDon, @MaHoaDon, '18', '0', @ID_DonVi, @ID_NhanVien, GETDATE(), '0', @GiaVon, @GiaVon, '0','0','0',
			N'Phiếu điều chỉnh được tạo tự động khi khởi tạo giá vốn hàng hóa: ', N'Hoàn thành', 'admin', GETDATE());

	DECLARE @GiaVonCheck FLOAT; 
    	select @GiaVonCheck = COUNT(ID) from DM_GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoi and ID_DonVi = @ID_DonVi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL)
	IF(@GiaVonCheck = 0)
	BEGIN
		Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang, TonLuyKe)
		Values(NEWID(), @ID_HoaDon, '1','0','0', @GiaVon, @GiaVon, '0', '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang, dbo.FUNC_TonLuyKeTruocThoiGian(@ID_DonVi, @ID_HangHoa, @ID_LoHang, GETDATE()))
	END
	ELSE
	BEGIN
		DECLARE @GiaVonOld FLOAT; 
    			select @GiaVonOld = GiaVon from DM_GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoi and ID_DonVi = @ID_DonVi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL)
		Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang, TonLuyKe)
		Values(NEWID(), @ID_HoaDon, '1','0',@GiaVonOld, @GiaVon, (Case When @GiaVon - @GiaVonOld > 0 then @GiaVon - @GiaVonOld else 0 end), (Case When @GiaVon - @GiaVonOld > 0 then 0 else @GiaVon - @GiaVonOld end), '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang, dbo.FUNC_TonLuyKeTruocThoiGian(@ID_DonVi, @ID_HangHoa, @ID_LoHang, GETDATE()))
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


            Sql(@"ALTER PROCEDURE [dbo].[SP_AddChietKhau_ByIDNhom]
    @ID_NhomHangs [nvarchar](max),
    @ID_NhanVien [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    DECLARE @i int = 0
    	DECLARE @IDQuiDoi uniqueidentifier
    	SET @i= 
    		(SELECT COUNT(*) FROM DM_HangHoa hh
    		join DonViQuiDoi qd ON hh.iD= qd.ID_HangHoa
    		WHERE hh.ID_NhomHang in (Select * from splitstring(@ID_NhomHangs)))
    
    	WHILE @i>0
    		BEGIN
    			SELECT @IDQuiDoi= tb.ID FROM (
    				SELECT ROW_NUMBER() OVER (Order by qd.ID) AS RowNumber, qd.ID FROM DM_HangHoa hh
    				join DonViQuiDoi qd ON hh.iD= qd.ID_HangHoa
    				WHERE hh.ID_NhomHang in (Select * from splitstring(@ID_NhomHangs))
					and (qd.Xoa is null OR qd.Xoa='0')
    				and qd.ID not in (select ID_DonViQuiDoi from ChietKhauMacDinh_NhanVien where ID_NhanVien like @ID_NhanVien  and ID_DonVi like @ID_DonVi)
    				) tb
    			WHERE tb.RowNumber = @i
    			SET @i= @i -1;
    
    			if @IDQuiDoi is not null
    				INSERT INTO ChietKhauMacDinh_NhanVien (ID, ID_NhanVien, ID_DonVi, ChietKhau, LaPhanTram,ChietKhau_BanGoi, LaPhanTram_BanGoi, ChietKhau_YeuCau, LaPhanTram_YeuCau, TheoChietKhau_ThucHien, ChietKhau_TuVan, LaPhanTram_TuVan,ID_DonViQuiDoi, NgayNhap)
    				values ( NEWID(),@ID_NhanVien,@ID_DonVi, 0,'0',0,'0',0,'0',0,0,0,@IDQuiDoi,getdate())
    		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetListChietKhauNhanVien_By_IDQuiDoi]
    @ID_DonViQuiDoi [nvarchar](max),
	@ID_DonVi uniqueidentifier
AS
BEGIN
    SELECT ck.ID, nv.ID as ID_NhanVien, MaNhanVien, TenNhanVien,ISNULL(nv.DienThoaiDiDong,'') as DienThoaiDiDong,
		ChietKhau,
		ChietKhau_TuVan,
		ChietKhau_BanGoi,
		ChietKhau_YeuCau,
		TheoChietKhau_ThucHien,
		case when ck.LaPhanTram= '0' then case when ChietKhau='0' then '1' else '0' end else ck.LaPhanTram end AS LaPhanTram,
		case when ck.LaPhanTram_YeuCau= '0' then case when ChietKhau_YeuCau='0' then '1' else '0' end else ck.LaPhanTram_YeuCau end AS LaPhanTram_YeuCau,
		case when ck.LaPhanTram_TuVan= '0' then case when ChietKhau_TuVan='0' then '1' else '0' end else ck.LaPhanTram_TuVan end AS LaPhanTram_TuVan,	   		
		case when ck.LaPhanTram_BanGoi= '0' then case when ChietKhau_BanGoi='0' then '1' else '0' end else ck.LaPhanTram_BanGoi end AS LaPhanTram_BanGoi
    FROM ChietKhauMacDinh_NhanVien ck
    join NS_NhanVien nv on ck.ID_NhanVien = nv.ID
    WHERE ID_DonViQuiDoi like @ID_DonViQuiDoi and ck.ID_DonVi= @ID_DonVi
END");
            Sql(@"ALTER PROCEDURE [dbo].[Update_ChietKhau_YeuCauByID]
    @ID [uniqueidentifier],
    @ChietKhau_YeuCau [float],
    @LaPhanTram_YeuCau [bit],
	@TheoChietKhau_ThucHien int
AS
BEGIN
    update ChietKhauMacDinh_NhanVien set ChietKhau_YeuCau = @ChietKhau_YeuCau, LaPhanTram_YeuCau = @LaPhanTram_YeuCau, TheoChietKhau_ThucHien= @TheoChietKhau_ThucHien where ID = @ID
END");

            Sql(@"DROP TRIGGER [dbo].[trg_DeleteNhomDoiTuongs]");
        }
        
        public override void Down()
        {
            DropStoredProcedure(@"[dbo].[Update_ChietKhau_BanGoiByID]");
        }
    }
}
