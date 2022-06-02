namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExecTonLuyKe_20190629 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[splitstringByChar] (@stringToSplit NVARCHAR(MAX), @charSplit CHAR)
RETURNS
 @returnList TABLE ([Name] [nvarchar] (500))
AS
BEGIN

 DECLARE @name NVARCHAR(255)
 DECLARE @pos INT

 WHILE CHARINDEX(@charSplit, @stringToSplit) > 0
 BEGIN
  SELECT @pos  = CHARINDEX(@charSplit, @stringToSplit)  
  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)

  INSERT INTO @returnList 
  SELECT @name

  SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
 END

 INSERT INTO @returnList
 SELECT @stringToSplit

 RETURN
END");

            Sql(@"CREATE FUNCTION [dbo].[GetListNhomHangHoa] ( @IDNhomHang UNIQUEIDENTIFIER )
RETURNS
 @tblNhomHang TABLE (ID UNIQUEIDENTIFIER)
AS
BEGIN
	IF(@IDNhomHang IS NOT NULL)
	BEGIN
		DECLARE @tblNhomHangTemp TABLE (ID UNIQUEIDENTIFIER);
		INSERT INTO @tblNhomHang VALUES (@IDNhomHang);
		INSERT INTO @tblNhomHangTemp VALUES (@IDNhomHang);
		DECLARE @intFlag INT;
		SET @intFlag = 1;
		WHILE (@intFlag != 0)
		BEGIN
			SELECT @intFlag = COUNT(ID) FROM DM_NhomHangHoa WHERE ID_Parent IN (SELECT ID FROM @tblNhomHangTemp) AND (TrangThai = 0 OR TrangThai IS NULL);
			IF(@intFlag != 0)
			BEGIN
				INSERT INTO @tblNhomHangTemp
				SELECT ID FROM DM_NhomHangHoa WHERE ID_Parent IN (SELECT ID FROM @tblNhomHangTemp); 
				DELETE FROM @tblNhomHangTemp WHERE ID IN (SELECT ID FROM @tblNhomHang);
				INSERT INTO @tblNhomHang
				SELECT ID FROM @tblNhomHangTemp
			END
		END
	END
	ELSE
	BEGIN
		INSERT INTO @tblNhomHang
		SELECT ID FROM DM_NhomHangHoa WHERE (TrangThai = 0 OR TrangThai IS NULL)
	END
	RETURN
END");
            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TonKho]", parametersAction: p => new
            {
                ID_DonVi = p.Guid(),
                ThoiGian = p.DateTime(),
                SearchString = p.String(),
                ID_NhomHang = p.Guid(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"SET NOCOUNT ON;
	SET @ThoiGian = DATEADD(DAY, 1, @ThoiGian);
	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	SELECT nhh.TenNhomHangHoa AS TenNhomHang, 
	dvqd1.MaHangHoa AS MaHangHoa, 
	dhh.TenHangHoa + dvqd1.ThuocTinhGiaTri AS TenHangHoaFull, 
	dhh.TenHangHoa AS TenHangHoa, 
	dvqd1.ThuocTinhGiaTri AS ThuocTinh_GiaTri, 
	dvqd1.TenDonViTinh, 
	ISNULL(lh.MaLoHang, '') AS TenLoHang, 
	ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0) AS TonCuoiKy,
	ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0) * dhh.QuyCach AS TonQuyCach,
	IIF(@XemGiaVon = '1', ROUND(ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0) * ISNULL(gv.GiaVon, 0), 0), 0) AS GiaTriCuoiKy FROM
	DM_HangHoa dhh
	LEFT JOIN DM_LoHang lh
	ON dhh.ID = lh.ID_HangHoa
	LEFT JOIN
	(SELECT hd.LoaiHoaDon, dvqd.ID_HangHoa, hd.ID_CheckIn, hdct.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang, hdct.ID_LoHang , ROW_NUMBER() OVER (PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon DESC) AS RN FROM BH_HoaDon_ChiTiet hdct
	INNER JOIN BH_HoaDon hd
	ON hd.ID = hdct.ID_HoaDon
	INNER JOIN DonViQuiDoi dvqd
	ON dvqd.ID = hdct.ID_DonViQuiDoi
	WHERE ((hd.ID_DonVi = @ID_DonVi and ((hd.YeuCau  != '2' AND hd.YeuCau != '3') OR hd.YeuCau IS NULL)) OR (hd.ID_CheckIn = @ID_DonVi AND hd.YeuCau = '4')) AND hd.ChoThanhToan = 0 AND hd.NgayLapHoaDon <= @ThoiGian) tonkho
	ON dhh.ID = tonkho.ID_HangHoa AND (lh.ID = tonkho.ID_LoHang OR tonkho.ID_LoHang IS NULL)
	INNER JOIN DonViQuiDoi dvqd1
	ON dhh.ID = dvqd1.ID_HangHoa
	LEFT JOIN DM_GiaVon gv
	ON gv.ID_DonViQuiDoi = dvqd1.ID AND (gv.ID_LoHang IS NULL OR lh.ID = gv.ID_LoHang) AND gv.ID_DonVi = @ID_DonVi
	INNER JOIN DM_NhomHangHoa nhh
	ON nhh.ID = dhh.ID_NhomHang
	INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh
	ON nhh.ID = allnhh.ID
	WHERE (tonkho.RN = 1 or tonkho.RN is null) AND dhh.LaHangHoa = 1 AND dhh.TheoDoi LIKE @TheoDoi AND dvqd1.Xoa LIKE @TrangThai AND dvqd1.LaDonViChuan = 1
	--AND dhh.ID = 'AEF2A3D5-CA5C-4F42-89A0-0D204EE795F1'
	AND ((select count(Name) from @tblSearchString b where 
    			dhh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or dhh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
				or dhh.TenHangHoa like '%'+b.Name+'%'
				or lh.MaLoHang like '%' +b.Name +'%' 
    			or dvqd1.MaHangHoa like '%'+b.Name+'%'
				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
				or dvqd1.TenDonViTinh like '%'+b.Name+'%'
				or dvqd1.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0);");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TonKho_TongHop]", parametersAction: p => new
            {
                ID_DonVis = p.String(),
                ThoiGian = p.DateTime(),
                ID_NhomHang = p.Guid(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"SET NOCOUNT ON;
    	DECLARE @tblIDDonVi TABLE(ID_ChiNhanh UNIQUEIDENTIFIER, TenChiNhanh NVARCHAR(MAX), SoLuong FLOAT, GiaTri FLOAT);
    	INSERT INTO @tblIDDonVi SELECT dv.ID, dv.TenDonVi, 0, 0 FROM splitstring(@ID_DonVis) dvs INNER JOIN DM_DonVi dv ON dv.ID = dvs.Name;
    	SET @ThoiGian = DATEADD(DAY, 1, @ThoiGian);
    	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung);
    	DECLARE @ID_DonVi UNIQUEIDENTIFIER;
    	DECLARE CS_DonVis CURSOR SCROLL LOCAL FOR SELECT ID_ChiNhanh FROM @tblIDDonVi
    	OPEN CS_DonVis
    	FETCH FIRST FROM CS_DonVis INTO @ID_DonVi
    	WHILE @@FETCH_STATUS = 0
    	BEGIN
    		DECLARE @SoLuong FLOAT = 0;
    		DECLARE @GiaTri FLOAT = 0;
    		SELECT 
    		@SoLuong = SUM(ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0)),
    		@GiaTri = SUM(IIF(@XemGiaVon = '1', ROUND(ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0) * ISNULL(gv.GiaVon, 0), 0), 0)) FROM
    		DM_HangHoa dhh
			LEFT JOIN DM_LoHang lh
    		ON dhh.ID = lh.ID_HangHoa
    		LEFT JOIN
    		(SELECT hd.LoaiHoaDon, dvqd.ID_HangHoa, hd.ID_CheckIn, hdct.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang, hdct.ID_LoHang , ROW_NUMBER() OVER (PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon DESC) AS RN FROM BH_HoaDon_ChiTiet hdct
    		INNER JOIN BH_HoaDon hd
    		ON hd.ID = hdct.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqd
    		ON dvqd.ID = hdct.ID_DonViQuiDoi
    		WHERE ((hd.ID_DonVi = @ID_DonVi and ((hd.YeuCau  != '2' AND hd.YeuCau != '3') OR hd.YeuCau IS NULL)) OR (hd.ID_CheckIn = @ID_DonVi AND hd.YeuCau = '4')) AND hd.ChoThanhToan = 0 AND hd.NgayLapHoaDon <= @ThoiGian) tonkho
    		ON dhh.ID = tonkho.ID_HangHoa AND (lh.ID = tonkho.ID_LoHang OR tonkho.ID_LoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd1
    		ON dhh.ID = dvqd1.ID_HangHoa
    		LEFT JOIN DM_GiaVon gv
    		ON gv.ID_DonViQuiDoi = dvqd1.ID AND (gv.ID_LoHang IS NULL OR lh.ID = gv.ID_LoHang) AND gv.ID_DonVi = @ID_DonVi
    		INNER JOIN DM_NhomHangHoa nhh
    		ON nhh.ID = dhh.ID_NhomHang
    		INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh
    		ON nhh.ID = allnhh.ID
    		WHERE (tonkho.RN = 1 or tonkho.RN is null) AND dhh.LaHangHoa = 1 AND dhh.TheoDoi LIKE @TheoDoi AND dvqd1.Xoa LIKE @TrangThai AND dvqd1.LaDonViChuan = 1
    		--AND dhh.ID = 'AEF2A3D5-CA5C-4F42-89A0-0D204EE795F1'
    
    		UPDATE @tblIDDonVi SET SoLuong = @SoLuong, GiaTri = @GiaTri WHERE ID_ChiNhanh = @ID_DonVi;
    	FETCH NEXT FROM CS_DonVis INTO @ID_DonVi
    	END
    	CLOSE CS_DonVis
    	DEALLOCATE CS_DonVis
    
    	SELECT * FROM @tblIDDonVi");


            CreateStoredProcedure(name: "[dbo].[GetMaHoaDon_AuTo]", parametersAction: p => new
            {
                LoaiHoaDon = p.Int()
            }, body: @"SET NOCOUNT ON;
    DECLARE @MaHoaDon varchar(5);
	DECLARE @Return float

    if @LoaiHoaDon = 22 --Phieu thẻ giá trị --
    	set @MaHoaDon ='TGT'
    if @LoaiHoaDon = 4 --Phiếu nhập --
    	set @MaHoaDon ='PNK'	
    if @LoaiHoaDon = 7 --Trả hàng NCC --
    	set @MaHoaDon ='THNCC'

	if @LoaiHoaDon = 88 --Hàng hóa --
    	set @MaHoaDon ='HH0'

	if @LoaiHoaDon = 99 --Dịch vụ --
    	set @MaHoaDon ='DV0'
    
	if(@LoaiHoaDon != 99 and @LoaiHoaDon != 88)
	BEGIN
    	SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaHoaDon) AS float))
    	FROM BH_HoaDon WHERE CHARINDEX(@MaHoaDon,MaHoaDon) > 0 and CHARINDEX('Copy',MaHoaDon)= 0 and CHARINDEX('_',MaHoaDon)= 0

		if	@Return is null 
			select Cast(0 as float) as MaxCode
		else 
			select @Return as MaxCode
	END
	ELSE
	BEGIN
		SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaHangHoa) AS float))
    	FROM DonViQuiDoi WHERE CHARINDEX(@MaHoaDon,MaHangHoa) > 0 and CHARINDEX('Copy',MaHangHoa)= 0 and CHARINDEX('_',MaHangHoa)= 0

		if	@Return is null 
			select Cast(0 as float) as MaxCode
		else 
			select @Return as MaxCode
	END");

            Sql(@"ALTER PROCEDURE [dbo].[getListDanhSachHHImportKiemKe]
    @MaLoHangIP [nvarchar](max),
    @MaHangHoaIP [nvarchar](max),
    @ID_DonViIP [uniqueidentifier],
    @TimeIP [datetime]
AS
BEGIN
    DECLARE @TableImport TABLE (ID_DonViQuiDoi UNIQUEIDENTIFIER, ID UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, QuanLyTheoLoHang BIT, MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX),
    	ThuocTinh_GiaTri NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), TyLeChuyenDoi FLOAT, GiaNhap FLOAT, MaLoHang NVARCHAR(MAX), GiaVon FLOAT, TonKho FLOAT, NgayHetHan DATETIME) INSERT INTO @TableImport
    Select *
    FROM
    (
    select 
    	dvqd.ID as ID_DonViQuiDoi,
    	hh.ID as ID,
    	lh.ID as ID_LoHang,
    	case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa,
    	dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd.TenDonViTinh,
    		dvqd.TyLeChuyenDoi,
    		dvqd.GiaNhap,
    	Case when lh.ID is null then '' else lh.MaLoHang end as MaLoHang,
    	Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVon,
    		0 as TonKho,
    		Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan
    	FROM 
    	DonViQuiDoi dvqd 
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_LoHang lh on lh.ID_HangHoa = hh.ID and lh.MaLoHang = @MaLoHangIP 
    		left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or gv.ID_LoHang is null) and gv.ID_DonVi = @ID_DonViIP)
    	where dvqd.MaHangHoa = @MaHangHoaIP 
    		and dvqd.Xoa = 0
    		and hh.TheoDoi = 1 
    			) as p order by NgayHetHan
    
    	DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
    	DECLARE @ID UNIQUEIDENTIFIER;
    	DECLARE @ID_LoHang UNIQUEIDENTIFIER;
    	DECLARE @QuanLyTheoLoHang BIT;
    	DECLARE @MaHangHoa NVARCHAR(MAX);
    	DECLARE @TenHangHoa NVARCHAR(MAX);
    	DECLARE @ThuocTinh_GiaTri NVARCHAR(MAX);
    	DECLARE @TenDonViTinh NVARCHAR(MAX);
    	DECLARE @TyLeChuyenDoi FLOAT;
    	DECLARE @GiaNhap FLOAT;
    	DECLARE @MaLoHang NVARCHAR(MAX);
    	DECLARE @GiaVon FLOAT;
    	DECLARE @TonKho FLOAT;
    	DECLARE @NgayHetHan DATETIME;
    
    	 DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @TableImport
    
    OPEN CS_Item 
    FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi, @ID,@ID_LoHang,@QuanLyTheoLoHang, @MaHangHoa,@TenHangHoa, @ThuocTinh_GiaTri,@TenDonViTinh,@TyLeChuyenDoi, @GiaNhap,@MaLoHang,@GiaVon,@TonKho,@NgayHetHan
    WHILE @@FETCH_STATUS = 0
    BEGIN
    		DECLARE @TonKhoHienTai FLOAT;
    				SET @TonKhoHienTai = ISNULL([dbo].FUNC_TonLuyKeTruocThoiGian(@ID_DonViIP,@ID,@ID_LoHang,@TimeIP),0)
    				UPDATE @TableImport SET TonKho = @TonKhoHienTai / @TyLeChuyenDoi WHERE ID_DonViQuiDoi = @ID_DonViQuiDoi
    		FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi, @ID,@ID_LoHang,@QuanLyTheoLoHang, @MaHangHoa,@TenHangHoa, @ThuocTinh_GiaTri,@TenDonViTinh,@TyLeChuyenDoi, @GiaNhap,@MaLoHang,@GiaVon,@TonKho,@NgayHetHan
    	END
    CLOSE CS_Item
    DEALLOCATE CS_Item
    
    	SELECT * from @TableImport
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhSLTon]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON;
	SELECT SUM(hhton.TonKho) as TonKho FROM DM_HangHoa_TonKho hhton
	inner join DonViQuiDoi dvqd on hhton.ID_DonViQuyDoi = dvqd.ID 
	where dvqd.LaDonViChuan = 1 and hhton.ID_DonVi = @ID_ChiNhanh and dvqd.ID_HangHoa = @ID_HangHoa
	GROUP BY ID_DonViQuyDoi
END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoKho_TonKho_TongHop]");
            DropStoredProcedure("[dbo].[BaoCaoKho_TonKho");
            DropStoredProcedure("[dbo].[GetMaHoaDon_AuTo]");
        }
    }
}
