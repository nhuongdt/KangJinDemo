namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180918_2 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER FUNCTION [dbo].[FUNC_GiaVon]
(
	@ID [uniqueidentifier],
	@ID_DonViQuiDoiTH [uniqueidentifier],
	@ID_DonViQuiDoi NVARCHAR(MAX),
	@ID_HangHoa [uniqueidentifier],
	@ID_LoHang [uniqueidentifier],
	@ID_DonVi [uniqueidentifier],
	@NgayLapHoaDon [datetime],
	@TienChietKhau [float],
	@DonGia [float],
	@GiaVon [float],
	@LoaiHoaDon [int],
	@YeuCau [nvarchar](max),
	@SoLuong [float],
	@TongGiamGia [float],
	@TongTienHang [float],
	@TyLeChuyenDoi [float], 
	@TonKhoHienTai [float]
)
RETURNS FLOAT
AS
BEGIN
DECLARE @GiaVonReturn AS FLOAT;

DECLARE @GiaVonHienTai FLOAT;
	DECLARE @ListChiTietByIDQD TABLE (ID UNIQUEIDENTIFIER,ID_HoaDonCT UNIQUEIDENTIFIER,TyLeChuyenDoi FLOAT, NgayLapHoaDon DATETIME, ID_DonVi UNIQUEIDENTIFIER,ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TienChietKhau FLOAT,DonGia FLOAT,GiaVon FLOAT, GiaVon_NhanChuyenHang FLOAT, LoaiHoaDon INT, YeuCau nvarchar(max), SoLuong FLOAT, ChoThanhToan bit) 
	INSERT INTO @ListChiTietByIDQD
	SELECT TOP(1) hd.ID, bhct.ID as ID_HoaDonCT,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hd.ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon,bhct.GiaVon_NhanChuyenHang, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan
	FROM BH_HoaDon_ChiTiet bhct
	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	where bhct.ID_DonViQuiDoi in (SELECT name from SplitString(@ID_DonViQuiDoi)) and hd.ChoThanhToan = 'false'  
	and hd.LoaiHoaDon != 3 and hd.LoaiHoaDon != 19 and (bhct.ID_LoHang = @ID_LoHang or @ID_LoHang is null) AND ((hd.ID_DonVi = @ID_DonVi and hd.NgayLapHoaDon < @NgayLapHoaDon and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)) or (hd.YeuCau = '4'  and hd.ID_CheckIn = @ID_DonVi and hd.NgaySua < @NgayLapHoaDon))
	order by NgayLapHoaDon desc, SoThuTu, hd.LoaiHoaDon desc, hd.MaHoaDon desc
	
	--IF(@TonKhoHienTai < 0)
	--BEGIN
	--	SET @TonKhoHienTai = 0;
	--END
	DECLARE @TongTienHangDemo FLOAT;
		DECLARE @SoLuongDemo FLOAT;
	DECLARE @CountCT INT;
	SELECT @CountCT = COUNT(ID_HoaDonCT) FROM @ListChiTietByIDQD 
	if(@CountCT = 0)
	BEGIN 
		if(@LoaiHoaDon = 4)
		BEGIN
		-- Lấy Sum số lượng và Đơn giá của hóa đơn có nhiều chi tiết chung ID_HangHoa
		SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia - bhctdm.TienChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
		FROM BH_HoaDon_ChiTiet bhctdm
		left Join DonViQuiDoi dvqddm on bhctdm.ID_DonViQuiDoi = dvqddm.ID
		WHERE bhctdm.ID_HoaDon = @ID AND dvqddm.ID_HangHoa = @ID_HangHoa AND (bhctdm.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
		GROUP BY dvqddm.ID_HangHoa

			IF(@TongTienHang !=0)
			BEGIN
				SET @GiaVonReturn = (@TongTienHangDemo / @SoLuongDemo)  * (1 - (@TongGiamGia / @TongTienHang))
			END
			ELSE
			BEGIN
				SET @GiaVonReturn = (@TongTienHangDemo / @SoLuongDemo)
			END
		END
		ELSE
		BEGIN
			IF(@LoaiHoaDon = 10)
			BEGIN
			-- Lấy Sum số lượng và Đơn giá của hóa đơn có nhiều chi tiết chung ID_HangHoa
				SELECT @TongTienHangDemo = SUM(bhctdm.TienChietKhau * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.TienChietKhau * dvqddm.TyLeChuyenDoi) 
				FROM BH_HoaDon_ChiTiet bhctdm
				left Join DonViQuiDoi dvqddm on bhctdm.ID_DonViQuiDoi = dvqddm.ID
				WHERE bhctdm.ID_HoaDon = @ID AND dvqddm.ID_HangHoa = @ID_HangHoa AND (bhctdm.ID_LoHang = @ID_LoHang or @ID_LoHang is null)

				DECLARE @ID_DonViCheckIn1 UNIQUEIDENTIFIER;
				SELECT @ID_DonViCheckIn1 = ID_CheckIn FROM BH_HoaDon WHERE ID = @ID
				IF(@YeuCau = '1' OR (@YeuCau = '4' AND @ID_DonVi != @ID_DonViCheckIn1))
				BEGIN
					SET @GiaVonReturn = 0
				END
				IF(@YeuCau = '4' AND @ID_DonVi = @ID_DonViCheckIn1)
				BEGIN
					SET @GiaVonReturn = @TongTienHangDemo / @SoLuongDemo
				END
			END
			ELSE
			BEGIN
				SET @GiaVonReturn = 0
			END
		END
		DELETE FROM @ListChiTietByIDQD
	END
	ELSE
	BEGIN
		-- Lấy giá vốn theo yêu cầu
		DECLARE @ID_DonViCheckIn2 UNIQUEIDENTIFIER;
		DECLARE @IDHoaDonOfHAfTerHDGV UNIQUEIDENTIFIER;
		SELECT @IDHoaDonOfHAfTerHDGV = ID FROM @ListChiTietByIDQD
		
		SELECT @ID_DonViCheckIn2 = ID_CheckIn FROM BH_HoaDon WHERE ID = @IDHoaDonOfHAfTerHDGV
		IF(@ID_DonViCheckIn2 is not null)
		BEGIN
			DECLARE @YeuCauCheck NVARCHAR(MAX);
			SELECT @YeuCauCheck = YeuCau FROM @ListChiTietByIDQD
			IF(@YeuCauCheck = '1' OR (@YeuCauCheck = '4' AND @ID_DonVi != @ID_DonViCheckIn2))
			BEGIN
				select @GiaVonHienTai = (ISNULL(GiaVon / TyLeChuyenDoi, 0)) from  @ListChiTietByIDQD
			END
			IF(@YeuCauCheck = '4' AND @ID_DonVi = @ID_DonViCheckIn2)
			BEGIN
				select @GiaVonHienTai = (ISNULL(GiaVon_NhanChuyenHang / TyLeChuyenDoi, 0)) from  @ListChiTietByIDQD
			END
		END
		ELSE
		BEGIN
			select @GiaVonHienTai = (ISNULL(GiaVon / TyLeChuyenDoi, 0)) from  @ListChiTietByIDQD
		END
		
		SET @GiaVonReturn = @GiaVonHienTai
		--begin loaihoadon == 4
		if(@LoaiHoaDon = 4)
		BEGIN
		-- Lấy Sum số lượng và Đơn giá của hóa đơn có nhiều chi tiết chung ID_HangHoa
		SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia - bhctdm.TienChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
		FROM BH_HoaDon_ChiTiet bhctdm
		left Join DonViQuiDoi dvqddm on bhctdm.ID_DonViQuiDoi = dvqddm.ID
		WHERE bhctdm.ID_HoaDon = @ID AND dvqddm.ID_HangHoa = @ID_HangHoa AND (bhctdm.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
		GROUP BY dvqddm.ID_HangHoa
		-- end lấy SUM

			IF((@TonKhoHienTai + @SoLuongDemo) > 0 and @TonKhoHienTai > 0)
			BEGIN
				IF(@TongTienHang != 0)
				BEGIN
					SET @GiaVonReturn = (@TongTienHangDemo * (1 - (@TongGiamGia/@TongTienHang)) +  @GiaVonHienTai * @TonKhoHienTai) / (@TonKhoHienTai + @SoLuongDemo)				
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = (@TongTienHangDemo +  @GiaVonHienTai * @TonKhoHienTai) / (@TonKhoHienTai + @SoLuongDemo)				
				END
			END
			ELSE
			BEGIN
				IF(@TongTienHang != 0)
				BEGIN
					SET @GiaVonReturn = (@TongTienHangDemo / @SoLuongDemo) * (1 - (@TongGiamGia / @TongTienHang))
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = (@TongTienHangDemo / @SoLuongDemo)
				END
			END
		END
		--end loaihoadon = 4

		--BEGIn loaihoadon = 7
		IF(@LoaiHoaDon = 7)
		BEGIN
		-- Lấy Sum số lượng và Đơn giá của hóa đơn có nhiều chi tiết chung ID_HangHoa
		SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
		FROM BH_HoaDon_ChiTiet bhctdm
		left Join DonViQuiDoi dvqddm on bhctdm.ID_DonViQuiDoi = dvqddm.ID
		WHERE bhctdm.ID_HoaDon = @ID AND dvqddm.ID_HangHoa = @ID_HangHoa AND (bhctdm.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
		GROUP BY dvqddm.ID_HangHoa
		-- end lấy SUM

			IF((@TonKhoHienTai - @SoLuongDemo) > 0)
			BEGIN
				SET @GiaVonReturn = (@GiaVonHienTai * @TonKhoHienTai - @TongTienHangDemo)/(@TonKhoHienTai - @SoLuongDemo)
			END
			ELSE
			BEGIN
				SET @GiaVonReturn = @GiaVonHienTai
			END
		END
		--END loaihoadon = 7

		--BEGIN loaihoadon =10
		IF(@LoaiHoaDon = 10)
		BEGIN
		-- Lấy Sum số lượng và Đơn giá của hóa đơn có nhiều chi tiết chung ID_HangHoa
		SELECT @TongTienHangDemo = SUM(bhctdm.TienChietKhau * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.TienChietKhau * dvqddm.TyLeChuyenDoi) 
		FROM BH_HoaDon_ChiTiet bhctdm
		left Join DonViQuiDoi dvqddm on bhctdm.ID_DonViQuiDoi = dvqddm.ID
		WHERE bhctdm.ID_HoaDon = @ID AND dvqddm.ID_HangHoa = @ID_HangHoa AND (bhctdm.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
		GROUP BY dvqddm.ID_HangHoa
		-- end lấy SUM

			DECLARE @ID_DonViCheckIn UNIQUEIDENTIFIER;
			SELECT @ID_DonViCheckIn = ID_CheckIn FROM BH_HoaDon WHERE ID = @ID
			IF(@YeuCau = '1' OR (@YeuCau = '4' AND @ID_DonVi != @ID_DonViCheckIn))
			BEGIN
				SET @GiaVonReturn = @GiaVonHienTai
			END
			IF(@YeuCau = '4' AND @ID_DonVi = @ID_DonViCheckIn)
			BEGIN
				IF((@TonKhoHienTai + @SoLuongDemo) > 0 and @TonKhoHienTai > 0)
				BEGIN
					SET @GiaVonReturn = (@GiaVonHienTai * @TonKhoHienTai + @TongTienHangDemo) / (@TonKhoHienTai + @SoLuongDemo)
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = (@TongTienHangDemo / @SoLuongDemo) / @TyLeChuyenDoi
				END
			END
		END
		-- END loaihoadon =10

		--BEGIN loaihoadon = 6
		IF(@LoaiHoaDon=6)
		BEGIN
		-- Lấy Sum số lượng và Đơn giá của hóa đơn có nhiều chi tiết chung ID_HangHoa
		SELECT @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
		FROM BH_HoaDon_ChiTiet bhctdm
		left Join DonViQuiDoi dvqddm on bhctdm.ID_DonViQuiDoi = dvqddm.ID
		WHERE bhctdm.ID_HoaDon = @ID AND dvqddm.ID_HangHoa = @ID_HangHoa AND (bhctdm.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
		GROUP BY dvqddm.ID_HangHoa
		-- end lấy SUM

			DECLARE @ID_HoaDonTH UNIQUEIDENTIFIER;
			DECLARE @Check INT;
			SELECT @ID_HoaDonTH = ID_HoaDon FROM BH_HoaDon WHERE ID = @ID
	
			IF(@ID_HoaDonTH is not null)
			BEGIN
				DECLARE @GiaVonHDBan FLOAT;
				SELECT @GiaVonHDBan = GiaVon FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @ID_HoaDonTH and ID_DonViQuiDoi = @ID_DonViQuiDoiTH and (ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				IF(@TonKhoHienTai + @SoLuongDemo > 0 and @TonKhoHienTai > 0)
				BEGIN
					SET @GiaVonReturn = (@GiaVonHienTai * @TonKhoHienTai + (@GiaVonHDBan / @TyLeChuyenDoi) *@SoLuongDemo) /(@TonKhoHienTai + @SoLuongDemo)
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = @GiaVonHDBan / @TyLeChuyenDoi
				END
			END
			ELSE
			BEGIN
				SET @GiaVonReturn = @GiaVonHienTai
			END
		END
		--END loaihoadon=6

		-- BEGIN loaihoadon khác
		IF(@LoaiHoaDon = 1 OR @LoaiHoaDon = 5 OR @LoaiHoaDon = 8 OR @LoaiHoaDon = 9 OR @LoaiHoaDon = 18)
		BEGIN
			SET @GiaVonReturn = @GiaVonHienTai
		END
		-- END LoaiHoaDon khác
		DELETE FROM @ListChiTietByIDQD
	END
	
	RETURN @GiaVonReturn
END");

            CreateStoredProcedure(name: "[dbo].[SP_GetAll_UserContact_Where]", parametersAction: p => new
            {
                txtSearch = p.String()
            }, body: @"DECLARE @where AS nvarchar (max)=N''

IF @txtSearch !=''
	SET @where = ' WHERE '+ @txtSearch
ELSE
	SET @where = ' WHERE MaLienHe LIKE ''%%'''

DECLARE @sqlExc nvarchar(max) = 'SELECT DM_LienHe.ID, DM_LienHe.ID_DoiTuong, MaLienHe,TenLienHe, DM_LienHe.ID_TinhThanh, DM_LienHe.ID_QuanHuyen,DM_LienHe.SoDienThoai,DM_LienHe.NgaySinh,DM_LienHe.DiaChi,
	ISNULL(TenTinhThanh,'''') AS TenTinhThanh,ISNULL(TenQuanHuyen,'''') AS TenQuanHuyen ,DM_LienHe.Email,DM_LienHe.GhiChu,
	DM_LienHe.NgayTao, TenDoiTuong, DM_LienHe.ChucVu, DienThoaiCoDinh, ISNULL(DM_LienHe.XungHo,0) as XungHo
	FROM DM_LienHe 
	left join DM_DoiTuong dt ON DM_LienHe.ID_DoiTuong= dt.ID
	left join DM_TinhThanh tt ON DM_LienHe.ID_TinhThanh = tt.ID
	left join DM_QuanHuyen qh ON DM_LienHe.ID_QuanHuyen = qh.ID ' + @where
EXEC sp_executesql  @sqlExc");

            CreateStoredProcedure("[dbo].[SP_GetMaLienHe_Max]",
                body: @"SELECT MAX(CAST (dbo.udf_GetNumeric(MaLienHe) AS INT)) MaxCode
	FROM DM_LienHe");

            AlterStoredProcedure(name: "[dbo].[UpDateGiaVonDMHangHoa]",
                body: @"DECLARE @TongHopNhapXuat TABLE (ID UNIQUEIDENTIFIER, ID_HoaDonCT UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_DonVi UNIQUEIDENTIFIER,ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TienChietKhau FLOAT,DonGia FLOAT,GiaVon FLOAT, LoaiHoaDon INT, YeuCau nvarchar(max), SoLuong FLOAT, ChoThanhToan bit, ID_LoHang UNIQUEIDENTIFIER, TongGiamGia FLOAT, TongTienHang FLOAT, TyLeChuyenDoi float) 
    INSERT INTO @TongHopNhapXuat
    	SELECT * from(
    	SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgayLapHoaDon, hd.ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
    	FROM BH_HoaDon_ChiTiet bhct
    	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
    	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
    	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	where hd.LoaiHoaDon != 3 and hd.ChoThanhToan = 'false' and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)
    	--order by hd.NgayLapHoaDon
    
    	UNION all
    
    	SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgayLapHoaDon, hd.ID_CheckIn as ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
    	FROM BH_HoaDon_ChiTiet bhct
    	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
    	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
    	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	where hd.LoaiHoaDon = 10 and hd.ChoThanhToan = 'false' and hd.YeuCau = '4'
    	) as a
    	order by a.NgayLapHoaDon
    
    DECLARE @ID UNIQUEIDENTIFIER;
    DECLARE @ID_HoaDonCT UNIQUEIDENTIFIER;
    DECLARE @NgayLapHoaDon DATETIME;
    DECLARE @ID_DonVi UNIQUEIDENTIFIER;
    DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
    DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
    DECLARE @TienChietKhau FLOAT;
    DECLARE @DonGia FLOAT;
    DECLARE @GiaVon FLOAT;
    DECLARE @LoaiHoaDon INT;
    DECLARE @YeuCau NVARCHAR(MAX);
    DECLARE @SoLuong FLOAT;
    DECLARE @ChoThanhToan bit;
    DECLARE @ID_LoHang UNIQUEIDENTIFIER;
    DECLARE @TongGiamGia FLOAT;
    DECLARE @TongTienHang FLOAT;
    DECLARE @TyLeChuyenDoi FLOAT;
    
    DECLARE @GiaVonUpDate FLOAT;	
    DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @TongHopNhapXuat ORDER BY NgayLapHoaDon
    
    OPEN CS_Item 
    FETCH FIRST FROM CS_Item INTO @ID, @ID_HoaDonCT,@NgayLapHoaDon,@ID_DonVi, @ID_DonViQuiDoi,@ID_HangHoa, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong,@ChoThanhToan, @ID_LoHang, @TongGiamGia, @TongTienHang, @TyLeChuyenDoi
    WHILE @@FETCH_STATUS = 0
    BEGIN
    	   Declare @ID_DonViQuiDoiDVT nvarchar(max);
    	   SET @ID_DonViQuiDoiDVT = '';
    	   SELECT @ID_DonViQuiDoiDVT =   
    		SUBSTRING(
    				(
    					SELECT ','+CAST(ST1.ID as nvarchar(max))
    					FROM dbo.DonViQuiDoi ST1
    					WHERE ST1.ID_HangHoa = @ID_HangHoa
    					ORDER BY ST1.ID
    					FOR XML PATH ('')
    				), 2, 1000)
    		FROM DonViQuiDoi ST2 WHERE ST2.ID_HangHoa = @ID_HangHoa

			DECLARE @TonKhoHienTai FLOAT;
			SET @TonKhoHienTai = ISNULL([dbo].FUNC_TinhSLTon(@ID_DonVi,@ID_HangHoa,@ID_LoHang, @NgayLapHoaDon),0)
    		SET @GiaVonUpDate = [dbo].FUNC_GiaVon(@ID, @ID_DonViQuiDoi,@ID_DonViQuiDoiDVT, @ID_HangHoa,@ID_LoHang, @ID_DonVi,@NgayLapHoaDon, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong, @TongGiamGia, @TongTienHang,@TyLeChuyenDoi,@TonKhoHienTai)
    	  
    		--SET @GiaVonUpDate = 0;
    		-- UPDATE Giá vốn cho từng hóa đơn chi tiết
    		IF(@LoaiHoaDon = 10)		
    		BEGIN
    			DECLARE @ID_DonViCheckIn [uniqueidentifier];
    			SELECT @ID_DonViCheckIn = ID_CheckIn FROM BH_HoaDon WHERE ID = @ID
    			IF(@YeuCau = '1' OR (@YeuCau = '4' AND @ID_DonVi != @ID_DonViCheckIn))
    			BEGIN
    				UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate where ID = @ID_HoaDonCT
    			END
    			IF(@YeuCau = '4' AND @ID_DonVi = @ID_DonViCheckIn)
    			BEGIN
    				UPDATE BH_HoaDon_ChiTiet SET GiaVon_NhanChuyenHang = @GiaVonUpDate where ID = @ID_HoaDonCT
    			END
    		END
    		ELSE
    		BEGIN
    			IF(@LoaiHoaDon = 18)
    			BEGIN
    				UPDATE BH_HoaDon_ChiTiet SET DonGia = @GiaVonUpDate, PTChietKhau = (Case When GiaVon - @GiaVonUpDate > 0 then GiaVon - @GiaVonUpDate else 0 end), TienChietKhau = (Case When GiaVon - @GiaVonUpDate > 0 then 0 else GiaVon - @GiaVonUpDate end)  where ID = @ID_HoaDonCT
					SELECT @GiaVonUpDate = GiaVon FROM BH_HoaDon_ChiTiet WHERE ID = @ID_HoaDonCT
    			END
				ELSE IF(@LoaiHoaDon = 8)
    			BEGIN
    				DECLARE @ThanhTienNew FLOAT;
    				SET @ThanhTienNew = @GiaVonUpDate * @SoLuong
    				UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate, ThanhTien = @ThanhTienNew where ID = @ID_HoaDonCT
    			END
    			ELSE IF(@LoaiHoaDon = 9)
    			BEGIN
					DECLARE @SoLuongNew FLOAT;
					SELECT @SoLuongNew = ThanhTien - @TonKhoHienTai FROM BH_HoaDon_ChiTiet WHERE ID = @ID_HoaDonCT
					IF(@SoLuong < 0)
					BEGIN
						IF(@SoLuongNew < 0)
						BEGIN
							DECLARE @LechGiamNew FLOAT;
							SELECT @LechGiamNew = TongTienHang - (@SoLuong - @SoLuongNew) FROM BH_HoaDon WHERE ID = @ID
							UPDATE BH_HoaDon SET TongTienHang = @LechGiamNew, TongGiamGia = @LechGiamNew + TongChiPhi WHERE ID = @ID
						END
						IF(@SoLuongNew > 0)
						BEGIN
							DECLARE @LechTangNew FLOAT;
							SELECT @LechTangNew = TongChiPhi + @SoLuongNew FROM BH_HoaDon WHERE ID = @ID
							UPDATE BH_HoaDon SET TongChiPhi = @LechTangNew, TongTienHang = TongTienHang - @SoLuong , TongGiamGia = @LechTangNew + (TongTienHang - @SoLuong) WHERE ID = @ID
						END
						IF(@SoLuongNew = 0)
						BEGIN
							UPDATE BH_HoaDon SET TongTienHang = TongTienHang - @SoLuong , TongGiamGia = TongChiPhi + (TongTienHang - @SoLuong) WHERE ID = @ID
						END
					END
					IF(@SoLuong > 0)
					BEGIN
						IF(@SoLuongNew > 0)
						BEGIN
							UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi - (@SoLuong - @SoLuongNew) , TongGiamGia = TongChiPhi - (@SoLuong - @SoLuongNew) + TongTienHang WHERE ID = @ID
						END
						IF(@SoLuongNew < 0)
						BEGIN
							UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi - @SoLuong, TongTienHang = TongTienHang + @SoLuongNew, TongGiamGia = TongChiPhi - @SoLuong + (TongTienHang + @SoLuongNew) WHERE ID = @ID
						END
						IF(@SoLuongNew = 0)
						BEGIN
							UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi - @SoLuong, TongGiamGia = TongChiPhi - @SoLuong + TongTienHang WHERE ID = @ID
						END
					END
					IF(@SoLuong = 0)
					BEGIN
						IF(@SoLuongNew > 0)
						BEGIN
							UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi + @SoLuongNew, TongGiamGia = TongChiPhi + @SoLuongNew + TongTienHang WHERE ID = @ID
						END
						IF(@SoLuongNew < 0)
						BEGIN
							UPDATE BH_HoaDon SET TongTienHang = TongTienHang + @SoLuongNew, TongGiamGia = TongChiPhi + @SoLuongNew + TongTienHang WHERE ID = @ID
						END
					END
    				UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate,TienChietKhau = @TonKhoHienTai, SoLuong = ThanhTien - @TonKhoHienTai, ThanhToan = @GiaVonUpDate * (ThanhTien - @TonKhoHienTai)  where ID = @ID_HoaDonCT
    			END
    			ELSE
    			BEGIN
    				UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate where ID = @ID_HoaDonCT
    			END
    		END
    		-- END update giá vốn cho từng hóa đơn chi tiết
    
    		--Update giá vốn cho từng đơn vi qui đổi theo tỷ lệ chuyển đổi
    
    
    		DECLARE @TableDonViQuiDoi table(ID_DonViQuiDoiGV UNIQUEIDENTIFIER, TyLeChuyenDoiGV FLOAT) insert into @TableDonViQuiDoi 
    		select dvqdgv.ID as ID_DonViQuiDoiGV, dvqdgv.TyLeChuyenDoi as TyLeChuyenDoiGV from DonViQuiDoi dvqdgv where dvqdgv.ID_HangHoa = @ID_HangHoa
    
    		DECLARE @ID_DonViQuiDoiGV UNIQUEIDENTIFIER;
    		DECLARE @TyLeChuyenDoiGV FLOAT;
    		
    		DECLARE CS_ItemGV CURSOR SCROLL LOCAL FOR SELECT * FROM @TableDonViQuiDoi
    
    		 OPEN CS_ItemGV 
    		 FETCH FIRST FROM CS_ItemGV INTO @ID_DonViQuiDoiGV, @TyLeChuyenDoiGV
    		 WHILE @@FETCH_STATUS = 0
    		 BEGIN
    			DECLARE @GiaVonCheck FLOAT; 
    			select @GiaVonCheck = COUNT(ID) from DM_GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoiGV and ID_DonVi = @ID_DonVi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL)
    			
    			IF(@GiaVonCheck = 0)
    			BEGIN
    				INSERT INTO DM_GiaVon(ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon) values (newID(), @ID_DonViQuiDoiGV, @ID_DonVi,@ID_LoHang, @GiaVonUpDate * @TyLeChuyenDoiGV)
    			END
    			ELSE
    			BEGIN
    				DECLARE @GiaVonNew FLOAT; 
    				SET @GiaVonNew = @GiaVonUpDate * @TyLeChuyenDoiGV
    				UPDATE DM_GiaVon SET GiaVon = @GiaVonNew where ID_DonViQuiDoi = @ID_DonViQuiDoiGV and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL) and ID_DonVi = @ID_DonVi
    			END
    
    			FETCH NEXT FROM CS_ItemGV INTO @ID_DonViQuiDoiGV, @TyLeChuyenDoiGV
    		 END
    		 CLOSE CS_ItemGV
    		 DEALLOCATE CS_ItemGV 
    		 DELETE FROM @TableDonViQuiDoi
    		--end update giá vốn cho từng đơn vị qui đổi
    
    	   FETCH NEXT FROM CS_Item INTO @ID, @ID_HoaDonCT,@NgayLapHoaDon,@ID_DonVi, @ID_DonViQuiDoi,@ID_HangHoa, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong,@ChoThanhToan,@ID_LoHang, @TongGiamGia, @TongTienHang,@TyLeChuyenDoi
    	 
    	END
    	CLOSE CS_Item
    	 DEALLOCATE CS_Item");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GetAll_UserContact_Where]");
            DropStoredProcedure("[dbo].[SP_GetMaLienHe_Max]");
        }
    }
}