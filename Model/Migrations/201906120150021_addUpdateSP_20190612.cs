namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20190612 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROC [dbo].[insert_TonKhoKhoiTao]
AS
BEGIN
SET NOCOUNT ON;
--Insert tồn kho vào DM_HangHoa_TonKho
Update DonViQuiDoi set Xoa = 0 where Xoa is null
DECLARE @TabTK TABLE (ID UNIQUEIDENTIFIER, ID_DonViQuyDoi UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, TonKho float);
DECLARE @TableDonVi TABLE (ID_DonVi UNIQUEIDENTIFIER) INSERT INTO @TableDonVi
SELECT ID FROM DM_DonVi

DECLARE @ID_DonVi UNIQUEIDENTIFIER;

DECLARE CS_DonVi CURSOR SCROLL LOCAL FOR SELECT ID_DonVi FROM @TableDonVi
OPEN CS_DonVi
FETCH FIRST FROM CS_DonVi INTO @ID_DonVi
WHILE @@FETCH_STATUS = 0
BEGIN
		INSERT INTO @TabTK SELECT NEWID(), dvqd.ID, @ID_DonVi, dmlo.ID, [dbo].[FUNC_TinhSLTonKhiTaoHD](@ID_DonVi, dvqd.ID_HangHoa, dmlo.ID, DATEADD(minute, 1, GETDATE())) / dvqd.TyLeChuyenDoi
		FROM DonViQuiDoi dvqd
		INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		LEFT JOIN DM_LoHang dmlo on dvqd.ID_HangHoa = dmlo.ID_HangHoa
		where dvqd.Xoa = 0 and hh.LaHangHoa = 1
		FETCH NEXT FROM CS_DonVi INTO @ID_DonVi
END
CLOSE CS_DonVi
DEALLOCATE CS_DonVi
Update dm set dm.TonKho = tk.TonKho from DM_HangHoa_TonKho dm join @TabTK tk on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null))
Insert into DM_HangHoa_TonKho select NEWID(), tk.ID_DonViQuyDoi, tk.ID_DonVi, tk.ID_LoHang, tk.TonKho from @TabTK tk left join DM_HangHoa_TonKho dm on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) where dm.ID is null
END");

            Sql(@"ALTER FUNCTION [dbo].[FUNC_TinhSLTon]
    (
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier],
	@ID_LoHang [uniqueidentifier],
	@TimeStart [datetime]
	)
RETURNS FLOAT
AS

    BEGIN
	DECLARE @TonKho AS FLOAT;

    SELECT @TonKho =(a.TonCuoiKy / dvqd3.TyLeChuyenDoi) FROM 
    (
		SELECT 
		dhh.ID,
		SUM(ISNULL(HangHoa.TonDau,0)) AS TonCuoiKy
		FROM
			(
			SELECT
			td.ID_DonViQuiDoi,
			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
			NULL AS SoLuongNhap,
			NULL AS SoLuongXuat
			FROM
			(
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false' and hh.LaHangHoa = 1
				AND bhd.NgayLapHoaDon < @TimeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
    				And dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND dvqd.ID_HangHoa = @ID_HangHoa
				AND bhd.NgayLapHoaDon < @TimeStart and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
				null AS SoLuongXuat
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon < @TimeStart
    				AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
				null AS SoLuongXuat
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0

				AND bhd.NgaySua < @TimeStart
    				AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi
				) AS td
				GROUP BY td.ID_DonViQuiDoi
				) 
				AS HangHoa

				left JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
				LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
				LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
				GROUP BY dhh.ID
    )  a
    	left Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	where dvqd3.ladonvichuan = 1
    order by MaHangHoa
	RETURN @TonKho;
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhTonTheoLoHangHoa]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT ISNULL((a.TonCuoiKy / dvqd4.TyLeChuyenDoi),0) as TonKho, CAST(ISNULL(gv.GiaVon, 0) as float) as GiaVon, CAST(ISNULL(a.DonGia, 0) as float) as GiaNhap FROM 
	DonViQuiDoi dvqd4
	LEFT JOIN DM_GiaVon gv on dvqd4.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and (gv.ID_LoHang = @ID_LoHang or gv.ID_LoHang is null)
	LEFT JOIN
    (
    SELECT 
    dhh.ID,
    SUM(ISNULL(HangHoa.TonDau,0)) AS TonCuoiKy,
	tbl1.DonGia
    FROM
    (
    SELECT
    td.ID_DonViQuiDoi,
    SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    NULL AS SoLuongNhap,
    NULL AS SoLuongXuat
    FROM
    (
    SELECT 
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa  = 0 and bhd.ChoThanhToan = 'false' and hh.LaHangHoa =1 AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_ChiNhanh
    And dvqd.ID_HangHoa = @ID_HangHoa
    	and bhdct.ID_LoHang = @ID_LoHang
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    And dvqd.ID_HangHoa = @ID_HangHoa
    	and bhdct.ID_LoHang = @ID_LoHang
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    And dvqd.ID_HangHoa = @ID_HangHoa
    	and bhdct.ID_LoHang = @ID_LoHang
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    And dvqd.ID_HangHoa = @ID_HangHoa
    	and bhdct.ID_LoHang = @ID_LoHang
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi
    ) AS td
    GROUP BY td.ID_DonViQuiDoi
    ) 
    AS HangHoa
    --LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
		LEFT JOIn
		(select TOP(1) ct.ID_DonViQuiDoi, ct.DonGia from BH_HoaDon hd INNER JOIN
		BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
		where hd.LoaiHoaDon = '4' and ct.ID_LoHang = @ID_LoHang and hd.ID_DonVi = @ID_ChiNhanh
		ORDER BY NgayLapHoaDon DESC) as tbl1 on dvqd.ID = tbl1.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    GROUP BY dhh.ID, tbl1.DonGia
    ) a on dvqd4.ID_HangHoa = a.ID
	WHERE dvqd4.ID_HangHoa = @ID_HangHoa
    order by MaHangHoa
END
");
            Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMucLoHangBaoCao]
    @MaHH [nvarchar](max),
    @MaHHCoDau [nvarchar](max),
    @ListID_NhomHang [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @List_ThuocTinh [nvarchar](max)
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    	DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    		INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHHCoDau+',') where Name!='';
    			  Select @count =  (Select count(*) from @tablename);
    	    Select @countChar =   (Select count(*) from @tablenameChar);
    
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END

    --Begin @ListID_NhomHang != '%%'
   
    if(@MaHH = '' and @MaHHCoDau = '')
    --begin @MaHH = '%%'
    BEGIN

		if(@List_ThuocTinh != '')
		BEGIN
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, lohang.ID as ID_LoHang, lohang.MaLoHang, lohang.NgaySanXuat, lohang.NgayHetHan, lohang.NgayTao as NgayTaoLo, hh.TonToiThieu, hh.TonToiDa,hhtt.GiaTri + CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.NgayTao,
				dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, dnhh.TenNhomHangHoa as NhomHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh,
							CAST(ISNULL(gv.GiaVon, 0) as float) 
							as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable1
				from DM_LoHang lohang 
				LEFT JOIN DonViQuiDoi dvqd on lohang.ID_HangHoa = dvqd.ID_HangHoa
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				LEFT JOIN HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    			LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = lohang.ID
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
				where dvqd.xoa = 0 and dvqd.ladonvichuan = 1 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
				and hh.TheoDoi = 1 and hh.LaHangHoa = 1

				Select * from #dmhanghoatable1 hhtb2
    			where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
		END
		ELSE
		BEGIN
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, lohang.ID as ID_LoHang, lohang.MaLoHang, lohang.NgaySanXuat, lohang.NgayHetHan, lohang.NgayTao as NgayTaoLo, hh.TonToiThieu, hh.NgayTao,
				dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, dnhh.TenNhomHangHoa as NhomHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh,
							CAST(ISNULL(gv.GiaVon, 0) as float) 
							as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable2
				from DM_LoHang lohang 
				LEFT JOIN DonViQuiDoi dvqd on lohang.ID_HangHoa = dvqd.ID_HangHoa
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = lohang.ID
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
				where dvqd.xoa = 0 and dvqd.ladonvichuan = 1 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
				and hh.TheoDoi = 1 and hh.LaHangHoa = 1
			Select * from #dmhanghoatable2 hhtb2
		END
    
    END
    	--end @MaHH = '%%'
    if(@MaHH != '' or @MaHHCoDau != '')
    --begin @MaHH != '%%'
    BEGIN
    if(@List_ThuocTinh != '')
    BEGIN
		select dvqd.ID as ID_DonViQuiDoi, hh.ID, lohang.ID as ID_LoHang, lohang.MaLoHang, lohang.NgaySanXuat, lohang.NgayHetHan, lohang.NgayTao as NgayTaoLo, hh.TonToiThieu, hh.TonToiDa,hhtt.GiaTri + CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.NgayTao,
			dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, dnhh.TenNhomHangHoa as NhomHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh,
						CAST(ISNULL(gv.GiaVon, 0) as float) 
						as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable3
			from DM_LoHang lohang 
			LEFT JOIN DonViQuiDoi dvqd on lohang.ID_HangHoa = dvqd.ID_HangHoa
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			LEFT JOIN HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = lohang.ID
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
    		where 
    		((select count(*) from @tablename b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
			or hh.TenHangHoa like '%'+b.Name+'%'
    		or dvqd.MaHangHoa like '%'+b.Name+'%' or lohang.MaLoHang like '%'+b.Name+'%'  or lohang.TenLoHang like '%'+b.Name+'%')=@count or @count=0)
    		and ((select count(*) from @tablenameChar c where
    		hh.TenHangHoa like '%'+c.Name+'%' 
    		or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    	and dvqd.xoa =0 and dvqd.ladonvichuan = 1 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
    		and hh.TheoDoi =1 and hh.LaHangHoa = 1
		Select * from #dmhanghoatable3 hhtb2	
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    END
    ELSE
    BEGIN
		select dvqd.ID as ID_DonViQuiDoi, hh.ID, lohang.ID as ID_LoHang, lohang.MaLoHang, lohang.NgaySanXuat, lohang.NgayHetHan, lohang.NgayTao as NgayTaoLo, hh.TonToiThieu, hh.NgayTao,
			dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, dnhh.TenNhomHangHoa as NhomHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh,
						CAST(ISNULL(gv.GiaVon, 0) as float) 
						as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable4
			from DM_LoHang lohang 
			LEFT JOIN DonViQuiDoi dvqd on lohang.ID_HangHoa = dvqd.ID_HangHoa
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = lohang.ID
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
    		where 
    		((select count(*) from @tablename b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
			or hh.TenHangHoa like '%'+b.Name+'%'
    		or dvqd.MaHangHoa like '%'+b.Name+'%' or lohang.MaLoHang like '%'+b.Name+'%'  or lohang.TenLoHang like '%'+b.Name+'%')=@count or @count=0)
    		and ((select count(*) from @tablenameChar c where
    		hh.TenHangHoa like '%'+c.Name+'%' 
    		or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    	and dvqd.xoa =0 and dvqd.ladonvichuan = 1 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
    		and hh.TheoDoi =1 and hh.LaHangHoa = 1
		Select * from #dmhanghoatable4 hhtb2	
		END
	END
END");

            CreateStoredProcedure(name: "[dbo].[getListDanhSachHHImportKiemKe]", parametersAction: p => new
            {
                MaLoHangIP = p.String(),
                MaHangHoaIP = p.String(),
                ID_DonViIP = p.Guid(),
                TimeIP = p.DateTime()
            }, body: @"SET NOCOUNT ON;
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
    				SET @TonKhoHienTai = ISNULL([dbo].FUNC_TinhSLTonKhiTaoHD(@ID_DonViIP,@ID,@ID_LoHang,@TimeIP),0)
    				UPDATE @TableImport SET TonKho = @TonKhoHienTai / @TyLeChuyenDoi WHERE ID_DonViQuiDoi = @ID_DonViQuiDoi
    		FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi, @ID,@ID_LoHang,@QuanLyTheoLoHang, @MaHangHoa,@TenHangHoa, @ThuocTinh_GiaTri,@TenDonViTinh,@TyLeChuyenDoi, @GiaNhap,@MaLoHang,@GiaVon,@TonKho,@NgayHetHan
    	END
    CLOSE CS_Item
    DEALLOCATE CS_Item
    
    	SELECT * from @TableImport");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getListDanhSachHHImportKiemKe]");
        }
    }
}
