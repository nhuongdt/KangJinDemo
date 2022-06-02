namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190610 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER FUNCTION [dbo].[FUNC_TinhSLTonKhiTaoHD]
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
	DECLARE @timeStartCS DATETIME;
	Set @timeStartCS =  (select convert(datetime, '2016/01/01'))
	DECLARE @SQL VARCHAR(254)
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStartCS =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END	

	DECLARE @NgayKiemKeGanNhat DATETIME;
	DECLARE @CheckPhieuKiem INT;
	DECLARE @TonPhieuKiem FLOAT;
	SET @CheckPhieuKiem = 0;
	SET @TonKho = 0;
	SELECT TOP(1) @NgayKiemKeGanNhat = hd.NgayLapHoaDon, @CheckPhieuKiem = COUNT(hd.ID), @TonPhieuKiem = SUM(bhct.ThanhTien * dvqd.TyLeChuyenDoi) FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	 where dvqd.ID_HangHoa = @ID_HangHoa and (bhct.ID_LoHang = @ID_LoHang or @ID_LoHang is null) and hd.NgayLapHoaDon < @TimeStart and hd.NgayLapHoaDon > @timeStartCS and LoaiHoaDon = 9 AND ChoThanhToan = 0
	 and ID_DonVi = @ID_ChiNhanh
	 GROUP BY hd.NgayLapHoaDon
	 order by NgayLapHoaDon desc
	 
	  
	IF(@CheckPhieuKiem = 0)
	BEGIN
			SELECT @TonKho =(a.TonCuoiKy / dvqd3.TyLeChuyenDoi) FROM 
			(
				SELECT 
				dhh.ID,
				SUM(HangHoa.TonDau) AS TonCuoiKy
				FROM
					(
					SELECT
					td.ID_DonViQuiDoi,
					SUM(td.TonKho) + SUM(td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
					0 AS SoLuongNhap,
					0 AS SoLuongXuat
					FROM
					(
						SELECT 
    						dvqd.ID As ID_DonViQuiDoi,
    						0 AS SoLuongNhap,
    						0 AS SoLuongXuat,
    						SUM(ISNULL(cs.TonKho, 0)) as TonKho
    						FROM DonViQUiDoi dvqd
    						left join chotso_hanghoa cs on dvqd.ID_HangHoa = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    						where dvqd.ladonvichuan = '1' and dvqd.ID_HangHoa = @ID_HangHoa and (cs.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
    						GROUP BY dvqd.ID
    					UNION ALL

						SELECT 
						bhdct.ID_DonViQuiDoi,
						0 AS SoLuongNhap,
						SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
						WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false' and hh.LaHangHoa = 1
						AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @timeStartCS
						AND bhd.ID_DonVi = @ID_ChiNhanh
    						And dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
						UNION ALL
						SELECT 
						bhdct.ID_DonViQuiDoi,
						0 AS SoLuongNhap,
						SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
						OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
						AND bhd.ID_DonVi = @ID_ChiNhanh
    						AND dvqd.ID_HangHoa = @ID_HangHoa
						AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @timeStartCS and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi
    
						UNION ALL
						SELECT 
						bhdct.ID_DonViQuiDoi,
						SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
						0 AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
						WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
						AND bhd.ID_DonVi = @ID_ChiNhanh
						AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @timeStartCS
    						AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi
    
						UNION ALL
						SELECT 
						bhdct.ID_DonViQuiDoi,
						SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
						0 AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0

						AND bhd.NgaySua < @TimeStart AND bhd.NgaySua > @timeStartCS
    						AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi
						) AS td
						GROUP BY td.ID_DonViQuiDoi
						) 
						AS HangHoa
						left JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
						LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
						GROUP BY dhh.ID
			)  a
			left Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    			where dvqd3.ladonvichuan = 1
		END
		ELSE
		BEGIN
			SELECT @TonKho =(a.TonCuoiKy / dvqd3.TyLeChuyenDoi) FROM 
			(
				SELECT 
				dhh.ID,
				SUM(HangHoa.TonDau) AS TonCuoiKy
				FROM
					(
					SELECT
					td.ID_DonViQuiDoi,
					SUM(td.TonKho) + SUM(td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
					0 AS SoLuongNhap,
					0 AS SoLuongXuat
					FROM
					(
						SELECT 
						bhdct.ID_DonViQuiDoi,
						0 AS SoLuongNhap,
						SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
						WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false' and hh.LaHangHoa = 1
						AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @NgayKiemKeGanNhat
						AND bhd.ID_DonVi = @ID_ChiNhanh
    						And dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
						UNION ALL
						SELECT 
						bhdct.ID_DonViQuiDoi,
						0 AS SoLuongNhap,
						SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
						OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
						AND bhd.ID_DonVi = @ID_ChiNhanh
    						AND dvqd.ID_HangHoa = @ID_HangHoa
						AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @NgayKiemKeGanNhat and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi
    
						UNION ALL
						SELECT 
						bhdct.ID_DonViQuiDoi,
						SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
						0 AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
						WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
						AND bhd.ID_DonVi = @ID_ChiNhanh
						AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @NgayKiemKeGanNhat
    						AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi
    
						UNION ALL
						SELECT 
						bhdct.ID_DonViQuiDoi,
						SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
						0 AS SoLuongXuat,
						0 AS TonKho
						FROM BH_HoaDon_ChiTiet bhdct
						LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
						LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
						WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0

						AND bhd.NgaySua < @TimeStart AND bhd.NgaySua > @NgayKiemKeGanNhat
    						AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
						GROUP BY bhdct.ID_DonViQuiDoi
						) AS td
						GROUP BY td.ID_DonViQuiDoi
						) 
						AS HangHoa
						left JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
						LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
						GROUP BY dhh.ID
			)  a
			left Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    			where dvqd3.ladonvichuan = 1

				SET @TonKho = @TonKho + @TonPhieuKiem
		END
	RETURN @TonKho;
END");

            CreateStoredProcedure(name: "[dbo].[insert_DM_HangHoaTonKHo]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuyDoi = p.Guid(),
                ID_LoHang = p.Guid(),
                LaDonViTinhChuan = p.Int(),
                TonKho = p.Double()
            }, body: @"if(@LaDonViTinhChuan = 1)
BEGIN
Insert into DM_HangHoa_TonKho
select NEWID() as ID, @ID_DonViQuyDoi as ID_DonViQuyDoi, ID as ID_DonVi, @ID_LoHang as ID_LoHang, 
Case when ID = @ID_ChiNhanh Then @TonKho else 0 end as TonKho FROM DM_DonVi
END
ELSE
BEGIN
	Insert into DM_HangHoa_TonKho
	select NEWID() as ID, @ID_DonViQuyDoi as ID_DonViQuyDoi, ID as ID_DonVi, @ID_LoHang as ID_LoHang, 
	Case when ID = @ID_ChiNhanh Then @TonKho else 0 end as TonKho FROM DM_DonVi

	DECLARE @TongTon float
	SET @TongTon = (select SUM(tk.TonKho * dvqd2.TyLeChuyenDoi) from DonViQuiDoi dvqd1
	join DonViQuiDoi dvqd2 on dvqd1.ID_HangHoa = dvqd2.ID_HangHoa
	join DM_HangHoa_TonKho tk on tk.ID_DonViQuyDoi = dvqd2.ID and tk.ID_DonVi = @ID_ChiNhanh and (tk.ID_LoHang = @ID_LoHang or tk.ID_LoHang is null)
	where dvqd1.ID = @ID_DonViQuyDoi
	)
	print @TongTon
	Update  hhtk Set hhtk.TonKho = tk.KK FROM DM_HangHoa_TonKho hhtk INNER JOIN (
	select dvqd2.ID_HangHoa, dvqd2.ID, dvqd2.TyLeChuyenDoi, tk.TonKho, @TongTon / dvqd2.TyLeChuyenDoi as KK from DonViQuiDoi dvqd1
	join DonViQuiDoi dvqd2 on dvqd1.ID_HangHoa = dvqd2.ID_HangHoa
	join DM_HangHoa_TonKho tk on tk.ID_DonViQuyDoi = dvqd2.ID and tk.ID_DonVi = @ID_ChiNhanh
	where dvqd1.ID = @ID_DonViQuyDoi
	) tk on hhtk.ID_DonViQuyDoi = tk.ID and hhtk.ID_DonVi = @ID_ChiNhanh and (hhtk.ID_LoHang = @ID_LoHang or hhtk.ID_LoHang is null)
END");

            CreateStoredProcedure(name: "[dbo].[insert_TonKhoKhoiTao]",
                body: @"--Insert tồn kho vào DM_HangHoa_TonKho
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
Insert into DM_HangHoa_TonKho select NEWID(), tk.ID_DonViQuyDoi, tk.ID_DonVi, tk.ID_LoHang, tk.TonKho from @TabTK tk left join DM_HangHoa_TonKho dm on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) where dm.ID is null");

            CreateStoredProcedure(name: "[dbo].[insert_TonKhoKhoiTaoByInsert]",
                body: @"DECLARE @ID_DonViInsert Uniqueidentifier;
SET @ID_DonViInsert = (select Top 1 ID_DonVi from BH_HoaDon where SoLanIn = -9 and ChoThanhToan = '0')
--Insert tồn kho vào DM_HangHoa_TonKho
DECLARE @TabTK TABLE (ID UNIQUEIDENTIFIER, ID_DonViQuyDoi UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, TonKho float, ChoThanhToan bit);
DECLARE @TableDonVi TABLE (ID_DonVi UNIQUEIDENTIFIER) INSERT INTO @TableDonVi
SELECT ID FROM DM_DonVi where ID != @ID_DonViInsert
DECLARE @ID_DonVi UNIQUEIDENTIFIER;
DECLARE CS_DonVi CURSOR SCROLL LOCAL FOR SELECT ID_DonVi FROM @TableDonVi
OPEN CS_DonVi
DECLARE @TabID_DonViQuiDoi TABLE (ID_DonViQuiDoi UNIQUEIDENTIFIER, ChoThanhToan bit);
INSERT INTO @TabID_DonViQuiDoi Select DISTINCT ct.ID_DonViQuiDoi, hd.ChoThanhToan FROM BH_HoaDon_ChiTiet ct join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
where hd.SoLanIn = -9

-- insert DM_HangHoa_TonKho cho DV insert
INSERT INTO @TabTK SELECT NEWID(), dvqd.ID, @ID_DonViInsert, dmlo.ID, [dbo].[FUNC_TinhSLTonKhiTaoHD](@ID_DonViInsert, dvqd.ID_HangHoa, dmlo.ID, DATEADD(minute, 1, GETDATE())) / dvqd.TyLeChuyenDoi, dv.ChoThanhToan
		FROM DonViQuiDoi dvqd
		INNER JOIN @TabID_DonViQuiDoi dv on dvqd.ID = dv.ID_DonViQuiDoi
		INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		LEFT JOIN DM_LoHang dmlo on dvqd.ID_HangHoa = dmlo.ID_HangHoa
		where dvqd.Xoa = 0 and hh.LaHangHoa = 1

Update dm set dm.TonKho = tk.TonKho from DM_HangHoa_TonKho dm join @TabTK tk on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) and tk.ChoThanhToan = '0'
Insert into DM_HangHoa_TonKho select NEWID(), tk.ID_DonViQuyDoi, tk.ID_DonVi, tk.ID_LoHang, tk.TonKho from @TabTK tk left join DM_HangHoa_TonKho dm on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) where dm.ID is null
FETCH FIRST FROM CS_DonVi INTO @ID_DonVi
WHILE @@FETCH_STATUS = 0
BEGIN
	Insert into DM_HangHoa_TonKho select NEWID(), tk.ID_DonViQuyDoi, @ID_DonVi as ID_DonVi, tk.ID_LoHang, 0 as TonKho from @TabTK tk 
	left join DM_HangHoa_TonKho dm on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = @ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) where dm.ID is null
	FETCH NEXT FROM CS_DonVi INTO @ID_DonVi
END
CLOSE CS_DonVi
DEALLOCATE CS_DonVi");

            CreateStoredProcedure(name: "[dbo].[UpdateNhomDoiTuongs_ByID]", parametersAction: p => new
            {
                IDDoiTuong = p.Guid()
            }, body: @"declare @TenNhoms nvarchar(max)= (select (
										Select ndt.TenNhomDoiTuong + ', ' AS [text()]
												From dbo.DM_DoiTuong_Nhom dtn
												inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
												Where dtn.ID_DoiTuong = dt.ID 
												order by ndt.TenNhomDoiTuong
												For XML PATH ('')
										)
									from  dbo.DM_DoiTuong dt where dt.ID = @IDDoiTuong)
	if @TenNhoms is null
		set @TenNhoms= N'Nhóm mặc định'

	update DM_DoiTuong --IDNhoms
	set IDNhomDoiTuongs = (select (
								Select convert(nvarchar(max), ndt.ID)  + ', ' AS [text()]
										From dbo.DM_DoiTuong_Nhom dtn
										inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
										Where dtn.ID_DoiTuong = dt.ID 
										order by ndt.TenNhomDoiTuong
										For XML PATH ('')
								)
							from  dbo.DM_DoiTuong dt where dt.ID = @IDDoiTuong)
	,	-- tennhoms
	TenNhomDoiTuongs = @TenNhoms
	where ID= @IDDoiTuong");

            CreateStoredProcedure(name: "[dbo].[UpdateTonForDM_hangHoa_TonKho]", parametersAction: p => new
            {
                LoaiNhatKy = p.Int(),
                LoaiHoaDon = p.Int(),
                IDHoaDonInput = p.Guid(),
                ThoiGian = p.DateTime(),
                ChoThanhToan = p.Boolean(),
                IDChiNhanhInput = p.Guid(),
                IDCheckIn = p.Guid(),
                YeuCau = p.String()
            }, body: @"DECLARE @ChiTietHoaDon1 TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, SoLuong FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, ID_LoHang UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TyLeChuyenDoi FLOAT) 
			INSERT INTO @ChiTietHoaDon1 SELECT dvqd2.ID,SUM(ROUND(hdct.SoLuong * dvqd1.TyLeChuyenDoi / dvqd2.TyLeChuyenDoi, 3)) as SoLuong,
			SUM(ROUND(hdct.TienChietKhau * dvqd1.TyLeChuyenDoi / dvqd2.TyLeChuyenDoi, 3)) as TienChietKhau,SUM(ROUND(hdct.ThanhTien * dvqd1.TyLeChuyenDoi / dvqd2.TyLeChuyenDoi, 3)) as ThanhTien, hdct.ID_LoHang, dvqd2.ID_HangHoa, dvqd2.TyLeChuyenDoi 
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN DonViQuiDoi dvqd1
			on hdct.ID_DonViQuiDoi = dvqd1.ID
			INNER JOIN DM_HangHoa hh
			on dvqd1.ID_HangHoa = hh.ID
			INNER JOIN DonViQuiDoi dvqd2
			on hh.ID = dvqd2.ID_HangHoa
			WHERE hdct.ID_HoaDon = @IDHoaDonInput
			GROUP BY dvqd2.ID, hdct.ID_LoHang, dvqd2.ID_HangHoa,dvqd2.TyLeChuyenDoi

			UPDATE hhtonkho SET hhtonkho.TonKho = cthoadon.TonKho
			FROM DM_HangHoa_TonKho hhtonkho
			INNER JOIN (SELECT *, [dbo].FUNC_TinhSLTonKhiTaoHD(@IDChiNhanhInput,ID_HangHoa,ID_LoHang, DATEADD(minute, 1,GETDATE()))/TyLeChuyenDoi as TonKho FROM @ChiTietHoaDon1) as cthoadon on hhtonkho.ID_DonViQuyDoi = cthoadon.ID_DonViQuiDoi and (hhtonkho.ID_LoHang = cthoadon.ID_LoHang or cthoadon.ID_LoHang is null) and hhtonkho.ID_DonVi = @IDChiNhanhInput
			
			IF(@LoaiHoaDon = 10 and @YeuCau != '1')
			BEGIN
				UPDATE hhtonkho1 SET hhtonkho1.TonKho = cthoadon1.TonKho
			FROM DM_HangHoa_TonKho hhtonkho1
			INNER JOIN (SELECT *, [dbo].FUNC_TinhSLTonKhiTaoHD(@IDCheckIn,ID_HangHoa,ID_LoHang, DATEADD(minute, 1,GETDATE()))/TyLeChuyenDoi as TonKho FROM @ChiTietHoaDon1) as cthoadon1 on hhtonkho1.ID_DonViQuyDoi = cthoadon1.ID_DonViQuiDoi and (hhtonkho1.ID_LoHang = cthoadon1.ID_LoHang or cthoadon1.ID_LoHang is null) and hhtonkho1.ID_DonVi = @IDCheckIn
			END");

            CreateStoredProcedure(name: "[dbo].[UpdateTonKhoChoChiNhanhMoi]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"INSERT INTO DM_HangHoa_TonKho
    	Select NEWID(), ID_DonViQuyDoi, @ID_ChiNhanh, ID_LoHang, 0 from DM_HangHoa_TonKho WHERE ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'");

            Sql(@"ALTER PROCEDURE [dbo].[AddHHByNhomHangHoaKiemKho]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_NhomHangHoa [nvarchar](max),
    @TimeKK [datetime]
AS
BEGIN
    DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    IF(@ID_NhomHangHoa != '')
    BEGIN	
    
    		Set @timeStart =  (select convert(datetime, '2018/01/01'))
    		Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    		if (@SQL > 0)
    		BEGiN
    		Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    		END
    		Select ('IDRandom' +CAST(newID() AS varchar(100)) + '_') as ID_Random,
    		tr.ID_DonViQuiDoi,
    		tr.ID_HangHoa as ID,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		tr.QuanLyTheoLoHang,
    			tr.TyLeChuyenDoi,
    		tr.LaHangHoa,
    		--gv.GiaVon as GiaVon,
    		Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
    			Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as DonGia,
    		MAX(tr.GiaBan) as GiaBan,
    		MAX(tr.GiaNhap) as GiaNhap,
    		Sum(tr.TonCuoiKy) as SoLuong,
    		MAX(tr.SrcImage) as SrcImage,
    		Case when tr.ID_LoHang is null then null else tr.ID_LoHang end as ID_LoHang,
    		MAX(tr.MaLoHang) as MaLoHang,
    		MAX(tr.NgaySanXuat) as NgaySanXuat,
    			MAX(tr.NgayHetHan) as NgayHetHan
    		 FROM
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi,dvqd3.ID_HangHoa, dvqd3.MaHangHoa,
    		a.TenHH AS TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, dvqd3.TyLeChuyenDoi,
    			a.QuanLyTheoLoHang,
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,
    		CAST(ROUND((dvqd3.GiaNhap), 0) as float) as GiaNhap,
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,
    			ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
    				a.ID_LoHang,
    				a.MaLoHang,
    					a.NgaySanXuat,
    					a.NgayHetHan,
    				a.TrangThai,
    				a.LaHangHoa
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    			 Select  
    			 dhh1.ID,
    			 dhh1.LaHangHoa,
    			 dhh1.TenHangHoa as TenHH,
    			 dhh1.TenHangHoa,
    			 dhh1.TenHangHoa_KhongDau,
    			 dhh1.TenHangHoa_KyTuDau,
    			 dvqd1.TenDonViTinh,
    			 dhh1.QuanLyTheoLoHang,
    			 lh1.ID as ID_LoHang,
    			 lh1.MaLoHang,
    			 convert(nvarchar(max), CAST((lh1.NgaySanXuat) as date), 101) as NgaySanXuat,
    				 convert(nvarchar(max), CAST((lh1.NgayHetHan) as date), 101) as NgayHetHan,
    			 lh1.TrangThai,
    			 0 as TonCuoiKy
    			 from
    			 DonViQuiDoi dvqd1 
    			 join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
    			 left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa
    			 Union all
    
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
    			MAX(dhh.TenHangHoa) As TenHH,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' + MAX(dhh.TenHangHoa)  AS TenHangHoa,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' +  MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end)  + ' ' + MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    			dhh.QuanLyTheoLoHang,
    			MAX(lh.ID)  As ID_LoHang,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
    			MAX(lh.NgaySanXuat)  As NgaySanXuat,
    			MAX(lh.NgayHetHan)  As NgayHetHan,
    			lh.TrangThai,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
    			left join
    			(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					where dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                                     
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			Where dvqd.Xoa = 0 and dhh.TheoDoi = 1
    				and ((dhh.QuanLyTheoLoHang = 1 and lh.MaLoHang != '') or dhh.QuanLyTheoLoHang = 0)
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd3.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    where dvqd3.Xoa = 0 and dnhh.id=(select * from splitstring(@ID_NhomHangHoa) where [name] like dnhh.ID)
    		
    		)
    		tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		where tr.TrangThai = 1 or tr.TrangThai is null
    		Group by tr.ID_DonViQuiDoi,tr.ID_HangHoa, tr.ID_LoHang, tr.QuanLyTheoLoHang, gv.ID, gv.GiaVon, tr.LaHangHoa, tr.TyLeChuyenDoi
    		order by MAX(tr.NgayHetHan)
    END
    ELSE
    BEGIN
    			Set @timeStart =  (select convert(datetime, '2018/01/01'))
    			Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    			if (@SQL > 0)
    			BEGiN
    			Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    			END
    			Select ('IDRandom' +CAST(newID() AS varchar(100)) + '_') as ID_Random,
    		tr.ID_DonViQuiDoi,
    		tr.ID_HangHoa as ID,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		tr.QuanLyTheoLoHang,
    			tr.TyLeChuyenDoi,
    		tr.LaHangHoa,
    		--gv.GiaVon as GiaVon,
    		Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
    			Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as DonGia,
    		MAX(tr.GiaBan) as GiaBan,
    		MAX(tr.GiaNhap) as GiaNhap,
    		Sum(tr.TonCuoiKy) as SoLuong,
    		MAX(tr.SrcImage) as SrcImage,
    		Case when tr.ID_LoHang is null then null else tr.ID_LoHang end as ID_LoHang,
    		MAX(tr.MaLoHang) as MaLoHang,
    			MAX(tr.NgaySanXuat) as NgaySanXuat,
    			MAX(tr.NgayHetHan) as NgayHetHan
    		 FROM
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi,dvqd3.ID_HangHoa, dvqd3.MaHangHoa,
    		a.TenHH AS TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, dvqd3.TyLeChuyenDoi,
    			a.QuanLyTheoLoHang,
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,
    		CAST(ROUND((dvqd3.GiaNhap), 0) as float) as GiaNhap,
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,
    			ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
    				a.ID_LoHang,
    				a.MaLoHang,
    				a.NgaySanXuat,
    					a.NgayHetHan,
    				a.TrangThai,
    				a.LaHangHoa
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    			 Select  
    			 dhh1.ID,
    			 dhh1.LaHangHoa,
    			 dhh1.TenHangHoa as TenHH,
    			 dhh1.TenHangHoa,
    			 dhh1.TenHangHoa_KhongDau,
    			 dhh1.TenHangHoa_KyTuDau,
    			 dvqd1.TenDonViTinh,
    			 dhh1.QuanLyTheoLoHang,
    			 lh1.ID as ID_LoHang,
    			 lh1.MaLoHang,
    			 convert(nvarchar(max), CAST((lh1.NgaySanXuat) as date), 101) as NgaySanXuat,
    				 convert(nvarchar(max), CAST((lh1.NgayHetHan) as date), 101) as NgayHetHan,
    			 lh1.TrangThai,
    			 0 as TonCuoiKy
    			 from
    			 DonViQuiDoi dvqd1 
    			 join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
    			 left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa
    			 Union all
    
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
    			MAX(dhh.TenHangHoa) As TenHH,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' + MAX(dhh.TenHangHoa)  AS TenHangHoa,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' +  MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end)  + ' ' + MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    			dhh.QuanLyTheoLoHang,
    			MAX(lh.ID)  As ID_LoHang,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
    			MAX(lh.NgaySanXuat)  As NgaySanXuat,
    			MAX(lh.NgayHetHan)  As NgayHetHan,
    			lh.TrangThai,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
    			left join
    			(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					where dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                                     
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh 
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart and bhd.NgayLapHoaDon <= @TimeKK 
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			Where dvqd.Xoa = 0 and dhh.TheoDoi = 1 and dhh.TheoDoi = 1
    				and ((dhh.QuanLyTheoLoHang = 1 and lh.MaLoHang != '') or dhh.QuanLyTheoLoHang = 0) 
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    where dvqd3.Xoa = 0
    		
    		)
    		tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		where tr.TrangThai = 1 or tr.TrangThai is null
    		Group by tr.ID_DonViQuiDoi,tr.ID_HangHoa, tr.ID_LoHang, tr.QuanLyTheoLoHang, gv.ID, gv.GiaVon, tr.LaHangHoa, tr.TyLeChuyenDoi
    		order by MAX(tr.NgayHetHan)
    END
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_TongHop]
    @Text_Search [nvarchar](max),
    @MaHH [nvarchar](max),
    @TenNhomHang [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
	@LoaiChungTu [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
		SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    		a.MaHangHoa,
    		Max(a.TenHangHoaFull) as  TenHangHoaFull,
    		Max(a.TenHangHoa) as TenHangHoa,
    		Max(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		Max(a.TenDonViTinh) as TenDonViTinh,
    		a.TenLoHang,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuong, 
    		CAST(ROUND((Sum(a.ThanhTien)), 0) as float) as ThanhTien,
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.SoLuong * a.GiaVon)), 0) as float) else 0 end as TienVon,
    		CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD)), 0) as float) else 0 end as LaiLo
    	FROM
    	(
    		Select
    		nhh.TenNhomHangHoa as TenNhomHangHoa,
    		dvqd.MaHangHoa,
    		hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
    		hh.TenHangHoa,
    		dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		Case When hdct.ID_LoHang is not null then hdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    		hdct.SoLuong,
    		hdct.DonGia - hdct.TienChietKhau as GiaBan,
    		ISNULL(hdct.GiaVon, 0) as GiaVon,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((hd.TongGiamGia + hd.KhuyeMai_GiamGia) / hd.TongTienHang) end as GiamGiaHD 
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0
			and hd.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
			and (hh.ID_NhomHang like @ID_NhomHang or hh.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and dvqd.Xoa like @TrangThai
    		and (dvqd.MaHangHoa like @Text_Search or hh.TenHangHoa_KhongDau like @Text_Search or hh.TenHangHoa_KyTuDau like @Text_Search or nhh.TenNhomHangHoa_KhongDau like @TenNhomHang or nhh.TenNhomHangHoa_KyTuDau like @TenNhomHang or dvqd.MaHangHoa like @MaHH)
    	) a
    		Group by a.MaHangHoa, a.TenLoHang, a.ID_LoHang
    		OrDER BY LaiLo DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonChiTietI]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON
    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		tr.TonDauKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy *ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriDauKy,
    		MAX(tr.SoLuongNhap_NCC) as SoLuongNhap_NCC,
    		MAX(tr.SoLuongNhap_Kiem) as SoLuongNhap_Kiem,
    		MAX(tr.SoLuongNhap_Tra) as SoLuongNhap_Tra,
    		MAX(tr.SoLuongNhap_Chuyen) as SoLuongNhap_Chuyen,
    		MAX(tr.SoLuongNhap_SX) as SoLuongNhap_SX,
    		MAX(tr.SoLuongXuat_Ban) as SoLuongXuat_Ban,
    		MAX(tr.SoLuongXuat_Huy) as SoLuongXuat_Huy,
    		MAX(tr.SoLuongXuat_NCC) as SoLuongXuat_NCC,
    		MAX(tr.SoLuongXuat_Kiem) as SoLuongXuat_Kiem,
    		MAX(tr.SoLuongXuat_Chuyen) as SoLuongXuat_Chuyen,
    		MAX(tr.SoLuongXuat_SX) as SoLuongXuat_SX,
    		tr.TonCuoiKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT 
    		dvqd3.ID as ID_DonViQuiDoi,
    		a.ID_LoHang,
    		a.TenNhomHangHoa,
    		dvqd3.mahanghoa, 
			a.TenHangHoa +dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    		a.TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    		CAST(ROUND((a.SoLuongNhap_NCC / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap_NCC, 
    		CAST(ROUND((a.SoLuongNhap_Kiem / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_Kiem,
    		CAST(ROUND((a.SoLuongNhap_Tra / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap_Tra, 
    		CAST(ROUND((a.SoLuongNhap_Chuyen / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_Chuyen,
    		CAST(ROUND((a.SoLuongNhap_SX / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_SX,
    		CAST(ROUND((a.SoLuongXuat_Ban / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongXuat_Ban, 
    		CAST(ROUND((a.SoLuongXuat_Huy / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Huy,
    		CAST(ROUND((a.SoLuongXuat_NCC / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongXuat_NCC, 
    		CAST(ROUND((a.SoLuongXuat_Kiem / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Kiem,
    		CAST(ROUND((a.SoLuongXuat_Chuyen / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Chuyen,
    		CAST(ROUND((a.SoLuongXuat_SX / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_SX,
    		CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		SUM(HangHoa.TonDauKy) AS TonDauKy,
    		SUM(HangHoa.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
    		SUM(HangHoa.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
    		SUM(HangHoa.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
    		SUM(HangHoa.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
    		SUM(HangHoa.SoLuongNhap_SX) AS SoLuongNhap_SX,
    		SUM(HangHoa.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
    		SUM(HangHoa.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
    		SUM(HangHoa.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
    		SUM(HangHoa.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
    		SUM(HangHoa.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,
    		SUM(HangHoa.SoLuongXuat_SX) AS SoLuongXuat_SX,
    		SUM(HangHoa.TonDauKy) + SUM(HangHoa.SoLuongNhap_NCC) + SUM(HangHoa.SoLuongNhap_Kiem) + SUM(HangHoa.SoLuongNhap_Tra) + SUM(HangHoa.SoLuongNhap_Chuyen)- SUM(HangHoa.SoLuongXuat_Ban) - SUM(HangHoa.SoLuongXuat_Huy)  - SUM(HangHoa.SoLuongXuat_NCC)  - SUM(HangHoa.SoLuongXuat_Kiem)  - SUM(HangHoa.SoLuongXuat_Chuyen) AS TonCuoiKy
    		FROM
    		(
    			SELECT 
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(td.TonKho + td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDauKy,
    			0 AS SoLuongNhap_NCC,
    			0 AS SoLuongNhap_Kiem,
    			0 AS SoLuongNhap_Tra,
    			0 AS SoLuongNhap_Chuyen,
    			0 AS SoLuongNhap_SX,
    			0 AS SoLuongXuat_Ban,
    			0 AS SoLuongXuat_Huy,
    			0 AS SoLuongXuat_NCC,
    			0 AS SoLuongXuat_Kiem,
    			0 AS SoLuongXuat_Chuyen,
    			0 AS SoLuongXuat_SX,
    			0 AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi, 
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,                                                                                                                                                                            
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                      
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.ID_LoHang,
    				0 AS TonDauKy,
    				SUM(pstk.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
    				SUM(pstk.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
    				SUM(pstk.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
    				SUM(pstk.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
    				SUM(pstk.SoLuongNhap_SX) AS SoLuongNhap_SX,
    				SUM(pstk.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
    				SUM(pstk.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
    				SUM(pstk.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
    				SUM(pstk.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
    				SUM(pstk.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,
    				SUM(pstk.SoLuongXuat_SX) AS SoLuongXuat_SX,
    				0 AS TonCuoiKy
    				FROM 
    				(
    					-- Xuất bán
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                            
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
					-- Xuất kiểm kê
					SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
					0 AS SoLuongNhap_Kiem, 
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
					SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					AND bhdct.SoLuong > 0
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	left join DM_LoHang lh on a.ID_LoHang = lh.ID
    where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonChiTietII]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    	DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	 -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		tr.TonDauKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriDauKy,
    		MAX(tr.SoLuongNhap_NCC) as SoLuongNhap_NCC,
    		MAX(tr.SoLuongNhap_Kiem) as SoLuongNhap_Kiem,
    		MAX(tr.SoLuongNhap_Tra) as SoLuongNhap_Tra,
    		MAX(tr.SoLuongNhap_Chuyen) as SoLuongNhap_Chuyen,
    		MAX(tr.SoLuongNhap_SX) as SoLuongNhap_SX,
    		MAX(tr.SoLuongXuat_Ban) as SoLuongXuat_Ban,
    		MAX(tr.SoLuongXuat_Huy) as SoLuongXuat_Huy,
    		MAX(tr.SoLuongXuat_NCC) as SoLuongXuat_NCC,
    		MAX(tr.SoLuongXuat_Kiem) as SoLuongXuat_Kiem,
    		MAX(tr.SoLuongXuat_Chuyen) as SoLuongXuat_Chuyen,
    		MAX(tr.SoLuongXuat_SX) as SoLuongXuat_SX,
    		tr.TonCuoiKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT
    		dvqd3.ID as ID_DonViQuiDoi,
    		a.ID_LoHang,
    		a.TenNhomHangHoa,
    		dvqd3.mahanghoa, 
    		a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    		a.TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri, 
    		dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy,
    		CAST(ROUND((a.SoLuongNhap_NCC / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap_NCC, 
    		CAST(ROUND((a.SoLuongNhap_Kiem / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_Kiem,
    		CAST(ROUND((a.SoLuongNhap_Tra / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap_Tra, 
    		CAST(ROUND((a.SoLuongNhap_Chuyen / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_Chuyen,
    		CAST(ROUND((a.SoLuongNhap_SX / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_SX,
    		CAST(ROUND((a.SoLuongXuat_Ban / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongXuat_Ban, 
    		CAST(ROUND((a.SoLuongXuat_Huy / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Huy,
    		CAST(ROUND((a.SoLuongXuat_NCC / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongXuat_NCC, 
    		CAST(ROUND((a.SoLuongXuat_Kiem / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Kiem,
    		CAST(ROUND((a.SoLuongXuat_Chuyen / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Chuyen,
    		CAST(ROUND((a.SoLuongXuat_SX / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_SX,
    		CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    (
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		SUM(HangHoa.TonDauKy) AS TonDauKy,
    		SUM(HangHoa.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
    		SUM(HangHoa.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
    		SUM(HangHoa.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
    		SUM(HangHoa.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
    		SUM(HangHoa.SoLuongNhap_SX) AS SoLuongNhap_SX,
    		SUM(HangHoa.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
    		SUM(HangHoa.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
    		SUM(HangHoa.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
    		SUM(HangHoa.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
    		SUM(HangHoa.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,
    		SUM(HangHoa.SoLuongXuat_SX) AS SoLuongXuat_SX,
    		SUM(HangHoa.TonDauKy) + SUM(HangHoa.SoLuongNhap_NCC) + SUM(HangHoa.SoLuongNhap_Kiem) + SUM(HangHoa.SoLuongNhap_Tra) + SUM(HangHoa.SoLuongNhap_Chuyen)
    			- SUM(HangHoa.SoLuongXuat_Ban) - SUM(HangHoa.SoLuongXuat_Huy)  - SUM(HangHoa.SoLuongXuat_NCC)  - SUM(HangHoa.SoLuongXuat_Kiem)  - SUM(HangHoa.SoLuongXuat_Chuyen) AS TonCuoiKy
    		FROM
    		(
    			SELECT 
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(td.TonKho + td.SoLuongXuat_Ban + td.SoLuongXuat_Huy + td.SoLuongXuat_NCC + td.SoLuongXuat_Kiem + td.SoLuongXuat_Chuyen) 
    				- SUM(td.SoLuongNhap_NCC + td.SoLuongNhap_Kiem + td.SoLuongNhap_Tra + td.SoLuongNhap_Chuyen) AS TonDauKy,
    			SUM(td.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
    			SUM(td.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
    			SUM(td.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
    			SUM(td.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
    			SUM(td.SoLuongNhap_SX) AS SoLuongNhap_SX,
    			SUM(td.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
    			SUM(td.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
    			SUM(td.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
    			SUM(td.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
    			SUM(td.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,
    			SUM(td.SoLuongXuat_SX) AS SoLuongXuat_SX,
    			0 AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID as ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				NULL SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh    				
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian bắt đầu tới khi chốt sổ
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
					-- Xuất kiểm kê
					SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
					0 AS SoLuongNhap_Kiem, 
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
					SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					and bhdct.SoLuong > 0
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				 SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.ID_LoHang,
    				0 AS TonDauKy,
    				SUM(pstk.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
    				SUM(pstk.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
    				SUM(pstk.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
    				SUM(pstk.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
    				SUM(pstk.SoLuongNhap_SX) AS SoLuongNhap_SX,
    				SUM(pstk.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
    				SUM(pstk.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
    				SUM(pstk.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
    				SUM(pstk.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
    				SUM(pstk.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,
    				SUM(pstk.SoLuongXuat_SX) AS SoLuongXuat_SX,
    				--SUM(td.TonKho + td.SoLuongXuat_Ban + td.SoLuongXuat_Huy + td.SoLuongXuat_NCC + td.SoLuongXuat_Kiem + td.SoLuongXuat_Chuyen   - td.SoLuongNhap_NCC - td.SoLuongNhap_Kiem - td.SoLuongNhap_Tra - td.SoLuongNhap_Chuyen, 0)) AS TonCuoiKy,
    				0 AS TonCuoiKy
    				FROM 
    				(
    					-- Xuất bán
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	left join DM_LoHang lh on a.ID_LoHang = lh.ID
    where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonChiTietIII]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    	DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	 -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		tr.TonDauKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy *  ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriDauKy,
    		MAX(tr.SoLuongNhap_NCC) as SoLuongNhap_NCC,
    		MAX(tr.SoLuongNhap_Kiem) as SoLuongNhap_Kiem,
    		MAX(tr.SoLuongNhap_Tra) as SoLuongNhap_Tra,
    		MAX(tr.SoLuongNhap_Chuyen) as SoLuongNhap_Chuyen,
    		MAX(tr.SoLuongNhap_SX) as SoLuongNhap_SX,
    		MAX(tr.SoLuongXuat_Ban) as SoLuongXuat_Ban,
    		MAX(tr.SoLuongXuat_Huy) as SoLuongXuat_Huy,
    		MAX(tr.SoLuongXuat_NCC) as SoLuongXuat_NCC,
    		MAX(tr.SoLuongXuat_Kiem) as SoLuongXuat_Kiem,
    		MAX(tr.SoLuongXuat_Chuyen) as SoLuongXuat_Chuyen,
    		MAX(tr.SoLuongXuat_SX) as SoLuongXuat_SX,
    		tr.TonCuoiKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT
    		dvqd3.ID as ID_DonViQuiDoi,
    		a.ID_LoHang,
    		a.TenNhomHangHoa,
    		dvqd3.mahanghoa, 
    		a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    		a.TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy,
    		CAST(ROUND((a.SoLuongNhap_NCC / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap_NCC, 
    		CAST(ROUND((a.SoLuongNhap_Kiem / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_Kiem,
    		CAST(ROUND((a.SoLuongNhap_Tra / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap_Tra, 
    		CAST(ROUND((a.SoLuongNhap_Chuyen / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_Chuyen,
    		CAST(ROUND((a.SoLuongNhap_SX / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongNhap_SX,
    		CAST(ROUND((a.SoLuongXuat_Ban / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongXuat_Ban, 
    		CAST(ROUND((a.SoLuongXuat_Huy / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Huy,
    		CAST(ROUND((a.SoLuongXuat_NCC / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongXuat_NCC, 
    		CAST(ROUND((a.SoLuongXuat_Kiem / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Kiem,
    		CAST(ROUND((a.SoLuongXuat_Chuyen / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_Chuyen,
    		CAST(ROUND((a.SoLuongXuat_SX / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat_SX,
    		CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    (
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(HangHoa.TonCuoiKy) - SUM(HangHoa.SoLuongNhap_NCC) - SUM(HangHoa.SoLuongNhap_Kiem) - SUM(HangHoa.SoLuongNhap_Tra) - SUM(HangHoa.SoLuongNhap_Chuyen) + SUM(HangHoa.SoLuongXuat_Ban) + SUM(HangHoa.SoLuongXuat_Huy)  + SUM(HangHoa.SoLuongXuat_NCC)  + SUM(HangHoa.SoLuongXuat_Kiem)  + SUM(HangHoa.SoLuongXuat_Chuyen) AS TonDauKy,
    		SUM(HangHoa.SoLuongNhap_NCC)  AS SoLuongNhap_NCC,
    		SUM(HangHoa.SoLuongNhap_Kiem)  AS SoLuongNhap_Kiem,
    		SUM(HangHoa.SoLuongNhap_Tra)  AS SoLuongNhap_Tra,
    		SUM(HangHoa.SoLuongNhap_Chuyen)  AS SoLuongNhap_Chuyen,
    		SUM(HangHoa.SoLuongNhap_SX)  AS SoLuongNhap_SX,
    		SUM(HangHoa.SoLuongXuat_Ban)  AS SoLuongXuat_Ban,
    		SUM(HangHoa.SoLuongXuat_Huy)  AS SoLuongXuat_Huy,
    		SUM(HangHoa.SoLuongXuat_NCC)  AS SoLuongXuat_NCC,
    		SUM(HangHoa.SoLuongXuat_Kiem)  AS SoLuongXuat_Kiem,
    		SUM(HangHoa.SoLuongXuat_Chuyen)  AS SoLuongXuat_Chuyen,
    		SUM(HangHoa.SoLuongXuat_SX)  AS SoLuongXuat_SX,
    		SUM(HangHoa.TonCuoiKy) AS TonCuoiKy
    		FROM
    		(
    			SELECT 
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			0 AS TonDauKy,
    			0 AS SoLuongNhap_NCC,
    			0 AS SoLuongNhap_Kiem,
    			0 AS SoLuongNhap_Tra,
    			0 AS SoLuongNhap_Chuyen,
    			0 AS SoLuongNhap_SX,
    			0 AS SoLuongXuat_Ban,
    			0 AS SoLuongXuat_Huy,
    			0 AS SoLuongXuat_NCC,
    			0 AS SoLuongXuat_Kiem,
    			0 AS SoLuongXuat_Chuyen,
    			0 AS SoLuongXuat_SX,
    			SUM(td.TonKho  + td.SoLuongXuat_Ban + td.SoLuongXuat_Huy + td.SoLuongXuat_NCC + td.SoLuongXuat_Kiem +
    			 td.SoLuongXuat_Chuyen - td.SoLuongNhap_NCC - td.SoLuongNhap_Tra - td.SoLuongNhap_Kiem - td.SoLuongNhap_Chuyen) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID as ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				NULL SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian kết thúc tới khi chốt sổ
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
					-- Xuất kiểm kê
					SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap_NCC,
					0 AS SoLuongNhap_Kiem, 
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
					SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					and bhdct.SoLuong > 0
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				0 AS TonDauKy,
    				SUM(pstk.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
    				SUM(pstk.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
    				SUM(pstk.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
    				SUM(pstk.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
    				SUM(pstk.SoLuongNhap_SX) AS SoLuongNhap_SX,
    				SUM(pstk.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
    				SUM(pstk.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
    				SUM(pstk.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
    				SUM(pstk.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
    				SUM(pstk.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,
    				SUM(pstk.SoLuongXuat_SX) AS SoLuongXuat_SX,
    				0 AS TonCuoiKy
    				FROM 
    				(
    					-- Xuất bán
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem,
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				0 AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				0 AS SoLuongNhap_NCC,
    				0 AS SoLuongNhap_Kiem,
    				0 AS SoLuongNhap_Tra,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				0 AS SoLuongNhap_SX,
    				0 AS SoLuongXuat_Ban,
    				0 AS SoLuongXuat_Huy,
    				0 AS SoLuongXuat_NCC,
    				0 AS SoLuongXuat_Kiem, 
    				0 AS SoLuongXuat_Chuyen,
    				0 AS SoLuongXuat_SX,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
    where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonI]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
		 DECLARE @timeChotSo Datetime
		 Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		tr.TonDauKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriDauKy,
    		MAX(tr.SoLuongNhap) as SoLuongNhap,
    		MAX(tr.GiaTriNhap) as GiaTriNhap,
    		MAX(tr.SoLuongXuat) as SoLuongXuat,
    		MAX(tr.GiaTriXuat) as GiaTriXuat,
    		tr.TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT 
    	dvqd3.ID as ID_DonViQuiDoi, 
    	a.ID_LoHang,
    	a.TenNhomHangHoa,
    	dvqd3.mahanghoa,
    	a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    	a.TenHangHoa,
    	dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    	Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then  CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
		CAST(ROUND((a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi), 3) as float) as TonQuyCach
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    			--
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDau,0))  AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat,
    		SUM(HangHoa.TonDau) + SUM(HangHoa.SoLuongNhap) - SUM(HangHoa.SoLuongXuat) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(td.TonKho + td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
    			0 AS SoLuongNhap,
    			0 AS GiaTriNhap,
    			0 AS SoLuongXuat,
    			0 AS GiaTriXuat,
    			ISNULL(td.TonKho, 0) AS TonKho
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQuiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                    
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.ID_LoHang,
    				0 AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat,
    				0 AS TonKho
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriXuat,
    				--SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
					SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
					UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat,
					SUM(ISNULL(bhdct.SoLuong,0)* bhdct.GiaVon) * (-1) AS GiaTriXuat,
					0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang

    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.SoLuong * bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					AND bhdct.SoLuong > 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		 and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    		) a
    		LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonII]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    
    	SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		tr.TonDauKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * ISNULL(gv.GiaVon, 0), 0)  as float) else 0 end as GiaTriDauKy,
    		MAX(tr.SoLuongNhap) as SoLuongNhap,
    		MAX(tr.GiaTriNhap) as GiaTriNhap,
    		MAX(tr.SoLuongXuat) as SoLuongXuat,
    		MAX(tr.GiaTriXuat) as GiaTriXuat,
    		tr.TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy *ISNULL(gv.GiaVon, 0), 0)  as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	
    	SELECT 
    	dvqd3.ID as ID_DonViQuiDoi, 
    	a.ID_LoHang,
    	a.TenNhomHangHoa,
    	dvqd3.mahanghoa, 
    	a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    	a.TenHangHoa,
    	dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    	Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
		CAST(ROUND((a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi), 3) as float) as TonQuyCach
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		SUM(HangHoa.TonDauKy)  AS TonDauKy,
    		SUM(HangHoa.SoLuongNhap)  AS SoLuongNhap,
    		SUM(HangHoa.GiaTriNhap)  AS GiaTriNhap,
    		SUM(HangHoa.SoLuongXuat)  AS SoLuongXuat,
    		SUM(HangHoa.GiaTriXuat)  AS GiaTriXuat,
    		SUM(HangHoa.TonCuoiKy) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(td.TonKho + td.SoLuongXuat1) - SUM(td.SoLuongNhap1) AS TonDauKy,
    			SUM(td.SoLuongNhap1 + td.SoLuongNhap2) AS SoLuongNhap,
    			SUM(td.GiaTriNhap1 + td.GiaTriNhap2) AS GiaTriNhap,
    			SUM(td.SoLuongXuat1 + td.SoLuongXuat2) AS SoLuongXuat,
    			SUM(td.GiaTriXuat1 + td.GiaTriXuat2) AS GiaTriXuat,
    			SUM(td.TonKho + td.SoLuongNhap2) - SUM(td.SoLuongXuat2) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian bắt đầu đến khi chốt sổ
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
					SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang

					UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat1,
					SUM(bhdct.SoLuong* bhdct.GiaVon) *(-1) AS GiaTriXuat1,
					0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
					0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap1,
					0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong > 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
					SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriNhap1,
					0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
					SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
					UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				0 AS SoLuongNhap2,
    				0 AS GiaTriNhap2,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat2,
					SUM(bhdct.SoLuong* bhdct.GiaVon) * (-1) AS GiaTriXuat2,
    				--SUM(bhdct.ThanhToan) *(-1) AS GiaTriXuat2,
					0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap2,
    				--SUM(bhdct.ThanhTien, 0)) AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap2,
    				--SUM(bhdct.ThanhToan) AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong > 0
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS GiaTriNhap1,
    				0 AS SoLuongXuat1,
    				0 AS GiaTriXuat1,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
					SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriNhap2,
    				--SUM(bhdct.ThanhTien, 0)) AS GiaTriNhap2,
    				0 AS SoLuongXuat2,
    				0 AS GiaTriXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		left join DM_LoHang lh on a.ID_LoHang = lh.ID
	    where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    		and LaDonViChuan = 1
    		and a.Xoa like @TrangThai
    ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonIII]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		tr.TonDauKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriDauKy,
    		MAX(tr.SoLuongNhap) as SoLuongNhap,
    		MAX(tr.GiaTriNhap) as GiaTriNhap,
    		MAX(tr.SoLuongXuat) as SoLuongXuat,
    		MAX(tr.GiaTriXuat) as GiaTriXuat,
    		tr.TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT
    	dvqd3.ID as ID_DonViQuiDoi,
    	a.ID_LoHang,
    	a.TenNhomHangHoa,
    	dvqd3.mahanghoa, 
    	a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    	a.TenHangHoa,
    	dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    	Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy,     	
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
		CAST(ROUND((a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi), 3) as float) as TonQuyCach
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		SUM(HangHoa.TonCuoiKy + HangHoa.SoLuongXuat - HangHoa.SoLuongNhap)  AS TonDauKy,
    		SUM(HangHoa.SoLuongNhap) AS SoLuongNhap,
    		SUM(HangHoa.GiaTriNhap) AS GiaTriNhap,
    		SUM(HangHoa.SoLuongXuat) AS SoLuongXuat,
    		SUM(HangHoa.GiaTriXuat) AS GiaTriXuat,
    		SUM(HangHoa.TonCuoiKy) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			0 AS SoLuongNhap,
    			0 AS GiaTriNhap,
    			0 AS SoLuongXuat,
    			0 AS GiaTriXuat,
    			SUM(td.TonKho + td.SoLuongXuat) - SUM(td.SoLuongNhap) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				--0 AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat,
    				0 AS TonCuoiKy
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
					SUM(bhdct.tienchietkhau* bhdct.GiaVon) AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang

					UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat,
					SUM(bhdct.SoLuong* bhdct.GiaVon) * (-1) AS GiaTriXuat,
					0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong > 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	left join DM_LoHang lh on a.ID_LoHang = lh.ID
		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    		and LaDonViChuan = 1
    		and a.Xoa like @TrangThai
    ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonIV]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		tr.TonDauKy,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriDauKy,
    		MAX(tr.SoLuongNhap) as SoLuongNhap,
    		MAX(tr.GiaTriNhap) as GiaTriNhap,
    		MAX(tr.SoLuongXuat) as SoLuongXuat,
    		MAX(tr.GiaTriXuat) as GiaTriXuat,
    		tr.TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT 
    	dvqd3.ID as ID_DonViQuiDoi,
    	a.ID_LoHang,
    	a.TenNhomHangHoa,
    	dvqd3.mahanghoa, 
    	a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    	a.TenHangHoa,
    	dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    	Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    	a.TonDauKy / dvqd3.TyLeChuyenDoi as TonDauKy,
    	a.SoLuongNhap / dvqd3.TyLeChuyenDoi as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then a.GiaTriNhap else 0 end as GiaTriNhap,
    	a.SoLuongXuat / dvqd3.TyLeChuyenDoi as SoLuongXuat,
    	Case When @XemGiaVon = '1' then a.GiaTriXuat else 0 end as GiaTriXuat,
    	a.TonCuoiKy / dvqd3.TyLeChuyenDoi as TonCuoiKy,
		a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi as TonQuyCach
    	FROM 
		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		CAST(ROUND(SUM(HangHoa.TonDau), 3) as float) AS TonDauKy,
    		CAST(ROUND(SUM(HangHoa.SoLuongNhap), 3) as float) AS SoLuongNhap,
    		CAST(ROUND(SUM(HangHoa.GiaTriNhap), 0) as float) AS GiaTriNhap,
    		CAST(ROUND(SUM(HangHoa.SoLuongXuat), 3) as float) AS SoLuongXuat,
    		CAST(ROUND(SUM(HangHoa.GiaTriXuat), 0) as float) AS GiaTriXuat,
    		CAST(ROUND(SUM(HangHoa.TonDau) + SUM(HangHoa.SoLuongNhap) - SUM(HangHoa.SoLuongXuat), 3) as float)  AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
    			0 AS SoLuongNhap,
    			0 AS GiaTriNhap,
    			0 AS SoLuongXuat,
    			0 AS GiaTriXuat
    			FROM
    			(
    				-- phát sinh xuất nhập tồn đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				0 AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
					SUM(bhdct.SoLuong * bhdct.GiaVon) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
					SUM(bhdct.tienchietkhau * bhdct.GiaVon) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang

					UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS GiaTriNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat,
					SUM(bhdct.SoLuong * bhdct.GiaVon) * (-1) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.SoLuong * bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.SoLuong * bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong > 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(bhdct.tienchietkhau * bhdct.GiaVon) AS GiaTriNhap,
    				0 AS SoLuongXuat,
    				0 AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    				and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
    		LEFT JOIN (Select Main.id_hanghoa,
    						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    						From
    						(
    						Select distinct hh_tt.id_hanghoa,
    							(
    								Select tt.GiaTri + ' - ' AS [text()]
    								From dbo.hanghoa_thuoctinh tt
    								Where tt.id_hanghoa = hh_tt.id_hanghoa
    								order by tt.ThuTuNhap 
    								For XML PATH ('')
    							) hanghoa_thuoctinh
    						From dbo.hanghoa_thuoctinh hh_tt
    						) Main
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TonKhoI]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		MAX(tr.TonCuoiKy) as TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT
    		dvqd3.ID as ID_DonViQuiDoi,
    		a.ID_LoHang,
    		a.TenNhomHangHoa,
    		dvqd3.mahanghoa,
    		a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    		a.TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
			CAST(ROUND((a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi), 3) as float) as TonQuyCach
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
			dhh.ID_NhomHang,
			dvqd.Xoa,
    		MAX(lh.ID) as ID_LoHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
			MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
			MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(HangHoa.TonDau) + SUM(HangHoa.SoLuongNhap) - SUM(HangHoa.SoLuongXuat) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang As ID_LoHang,
    			SUM(td.TonKho + td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
    			0 AS SoLuongNhap,
    			0 AS SoLuongXuat,
    			td.TonKho
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1' 
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
					-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				UNION ALL	
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.ID_LoHang,
    				0 AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				0 AS TonKho
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		and dhh.TheoDoi like @TheoDoi

    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    		) a
    	    LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
    		where  (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    		order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TonKhoII]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		MAX(tr.TonCuoiKy) as TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT 		
    		dvqd3.ID as ID_DonViQuiDoi,
    		a.ID_LoHang,
    		a.TenNhomHangHoa,
    		dvqd3.mahanghoa,
    		a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    		a.TenHangHoa,
    		dvqd3.ThuocTinhGiaTri  as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
			CAST(ROUND((a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi), 3) as float) as TonQuyCach
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang,
    		dvqd.Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau)  as TenNhomHangHoa_KyTuDau,
    		SUM(HangHoa.TonCuoiKy) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang As ID_LoHang,
    			SUM(td.TonKho + td.SoLuongNhap2) - SUM(td.SoLuongXuat2) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    				-- phát sinh xuất nhập tồn từ thời gian bắt đầu đến khi chốt sổ
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				0 AS SoLuongNhap2,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap1,
    				0 AS SoLuongXuat1,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				0 AS SoLuongXuat2,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	left join DM_LoHang lh on a.ID_LoHang = lh.ID
    where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    		and LaDonViChuan = 1
    		and a.Xoa like @TrangThai
    ) tr
    left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TonKhoIII]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		MAX(tr.TonCuoiKy) as TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    	SELECT 
    		dvqd3.ID as ID_DonViQuiDoi,
    		a.ID_LoHang,
    		a.TenNhomHangHoa,
    		dvqd3.mahanghoa,
    		a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    		a.TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh,
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
			CAST(ROUND((a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi), 3) as float) as TonQuyCach
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang as ID_NhomHang,
    		dvqd.Xoa as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		SUM(HangHoa.TonCuoiKy) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			0 AS SoLuongNhap,
    			0 AS SoLuongXuat,
    			SUM(td.TonKho + td.SoLuongXuat) - SUM(td.SoLuongNhap) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				SUM(cs.TonKho) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1' 
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi,td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.ID_LoHang,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				0 AS TonCuoiKy
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
   
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat,
    				0 AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		left join DM_LoHang lh on a.ID_LoHang = lh.ID
    where  (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    		and LaDonViChuan = 1
    		and a.Xoa like @TrangThai
    ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    		order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TonKhoIV]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	SELECT 
    		MAX (tr.TenNhomHangHoa) as TenNhomHang,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoaFull) as TenHangHoaFull,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		MAX(tr.TenLoHang) as TenLoHang,
    		MAX(tr.TonCuoiKy) as TonCuoiKy,
			MAX(tr.TonQuyCach) as TonQuyCach,
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
		SELECT 
    	dvqd3.ID as ID_DonViQuiDoi,
    	a.ID_LoHang,
    	a.TenNhomHangHoa,
    	dvqd3.mahanghoa,
    	a.TenHangHoa +dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    	a.TenHangHoa,
    	dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    	Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
		CAST(ROUND((a.TonCuoiKy * a.QuyCach / dvqd3.TyLeChuyenDoi), 3) as float) as TonQuyCach
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang,
    		dvqd.Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		SUM(HangHoa.TonDau) + SUM(HangHoa.SoLuongNhap) - SUM(HangHoa.SoLuongXuat) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
    			0 AS SoLuongNhap,
    			0 AS SoLuongXuat
    			FROM
    			(
    				-- phát sinh xuất nhập tồn đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.ID_LoHang,
    				0 AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    			) a
    			LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
    			where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    			) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    		order by GiaTriCuoiKy desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TonKhoIV_TongHop]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh_SP [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	-- tách list ID
	DECLARE @tab as table(ID_ChiNhanh uniqueidentifier)
	Insert into @tab select * from splitstring(@ID_ChiNhanh_SP)
	DECLARE @ID_ChiNhanh uniqueidentifier;
	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT * FROM @tab
	OPEN CS_ItemUpDate 
	FETCH FIRST FROM CS_ItemUpDate INTO @ID_ChiNhanh
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @tmp as table(ID_ChiNhanh uniqueidentifier, TenChiNhanh nvarchar(max), SoLuong float, GiaTri float)
		Insert into @tmp
		SELECT 
		@ID_ChiNhanh as ID_ChiNhanh,
		(Select TenDonVi from DM_DonVi where ID = @ID_ChiNhanh) as TenChiNhanh,
    	Sum(tr.TonCuoiKy) as SoLuong,
    	Sum(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0)) as GiaTri
    	FROM
    	(
		SELECT 
    	dvqd3.ID as ID_DonViQuiDoi,
    	a.ID_LoHang,
    	a.TenNhomHangHoa,
    	dvqd3.mahanghoa,
    	a.TenHangHoa,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		dhh.ID_NhomHang,
    		dvqd.Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		SUM(HangHoa.TonDau) + SUM(HangHoa.SoLuongNhap) - SUM(HangHoa.SoLuongXuat) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
    			0 AS SoLuongNhap,
    			0 AS SoLuongXuat
    			FROM
    			(
    				-- phát sinh xuất nhập tồn đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong * dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau * dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.ID_LoHang,
    				0 AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				0 AS SoLuongNhap,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				0 AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    		and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    			) a
    			LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
    			where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    			) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		--Group by gv.ID_DonVi 

		UPDATE @tab SET ID_ChiNhanh = NEWID() WHERE ID_ChiNhanh = @ID_ChiNhanh
	FETCH NEXT FROM CS_ItemUpDate INTO @ID_ChiNhanh
	END
	--select MAX(TenChiNhanh) as TenChiNhanh, Sum(SoLuong) as SoLuong, Sum(GiaTri) as GiaTri from @tmp
	--group by TenChiNhanh
	select 
	ID_ChiNhanh,
	TenChiNhanh,
	Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(SoLuong, 0), 3) as float) else 0 end as SoLuong,
	Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(GiaTri , 0), 0) as float) else 0 end as GiaTri
	 from @tmp
	 order by GiaTri DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[getList_DMLoHang_TonKho]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_DonViQuiDoi [nvarchar](max),
    @timeChotSo [datetime],
	@ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	  DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    		SELECT 
			Case when lh.ID is null then NEWID() else lh.ID end as ID_LoHang,
    		dhh.ID as ID_HangHoa,
			Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end As TenLoHangFull,
			Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end As TenLoHang,
			lh.NgaySanXuat,
    		lh.NgayHetHan,
			ISNULL(tk.TonKho,0) AS TonKho,
			Case when @XemGiaVon = '1' then CAST(ROUND(ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaVon	
    		FROM
    			DonViQuiDoi dvqd 
				INNER JOIN DM_HangHoa_TonKho tk on dvqd.ID = tk.ID_DonViQuyDoi and tk.ID_DonVi = @ID_ChiNhanh
    			INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    			LEFT JOIN DM_LoHang lh on tk.ID_LoHang = lh.ID
    			left join DM_GiaVon gv on gv.ID_DonViQuiDoi = dvqd.ID and gv.ID_LoHang = tk.ID_LoHang and gv.ID_DonVi = @ID_ChiNhanh
    		where dvqd.ID = @ID_DonViQuiDoi
    		order by lh.NgayHetHan
END");

            Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoabyMaHH_LoHang]
    @MaLoHang [nvarchar](max),
    @MaHangHoa [nvarchar](max),
    @ID_DonVi [uniqueidentifier]
AS
BEGIN
    Select 
    		*
    	FROM
    	(
		select 
    	dvqd.ID,
    	hh.ID as ID_HangHoa,
    	lh.ID as ID_LoHang,
    	case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
    	hh.TenHangHoa,
    	dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd.TenDonViTinh,
    	Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
    	Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
    	Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan,
    	Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVon
    	FROM 
    	DonViQuiDoi dvqd 
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	left join DM_LoHang lh on lh.ID_HangHoa = hh.ID
    	left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or gv.ID_LoHang is null) and gv.ID_DonVi = @ID_DonVi)
    	where dvqd.MaHangHoa = @MaHangHoa 
    	and dvqd.Xoa = '0'
    	and hh.TheoDoi = 1
		) as p
    where p.TenLoHang = @MaLoHang
END");

            Sql(@"ALTER PROCEDURE [dbo].[getListHangHoaLoHang_ByMaHangHoa]
    @MaHH [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    SELECT Top(20) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    		a.QuanLyTheoLoHang,
    		Case when gv.ID is null then 0 else CAST(ROUND((gv.GiaVon), 0) as float) end  as GiaVon, 
    		CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,  
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,
    		an.URLAnh as SrcImage,
    		Case When a.ID_LoHang is null then NEWID() else a.ID_LoHang end as ID_LoHang,
    		a.MaLoHang as TenLoHang,
    		a.NgaySanXuat,
    		a.NgayHetHan
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    		dhh.LaHangHoa,
    		MAX(dhh.TenHangHoa) As TenHangHoa,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		dhh.QuanLyTheoLoHang,
    		MAX(lh.ID)  As ID_LoHang,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
    		MAX(lh.NgaySanXuat)  As NgaySanXuat,
    		MAX(lh.NgayHetHan)  As NgayHetHan,
    		lh.TrangThai,
    		SUM(HangHoa.TonCuoiKy) AS TonCuoiKy
    		FROM 
    			DonViQuiDoi dvqd 
    			left join
    		(
				Select ID_DonViQuyDoi As ID_DonViQuiDoi,
				Case when ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else  ID_LoHang end as ID_LoHang,
				TonKho As TonCuoiKy
				From DM_HangHoa_TonKho where ID_DonVi = @ID_ChiNhanh
    		) AS HangHoa
    		on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		Where dvqd.Xoa = 0 and dhh.TheoDoi = 1
    		and (lh.TrangThai = 1 or lh.TrangThai is null)
    		GROUP BY dhh.ID, dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    	LEFT Join DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and (gv.ID_LoHang = a.ID_LoHang or a.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh
    	Where dvqd3.MaHangHoa = @MaHH
    	and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
    order by a.NgayHetHan
END");

            Sql(@"ALTER PROCEDURE [dbo].[getListHangHoaLoHang_ChotSo_EnTer]
    @MaHH [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
   Select TOP(40)
		dvqd1.ID as ID_DonViQuiDoi,
    	dhh1.ID,
		dvqd1.MaHangHoa,
    	dhh1.TenHangHoa,
		dvqd1.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd1.TenDonViTinh,
		dhh1.QuanLyTheoLoHang,
		dvqd1.TyLeChuyenDoi,
		dhh1.LaHangHoa,
		Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
		dvqd1.GiaBan,
		dvqd1.GiaNhap,
		ISNULL(hhtonkho.TonKho,0) as TonKho,
		ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
		Case when lh1.ID is null then null else lh1.ID end as ID_LoHang,
		lh1.MaLoHang,
    	lh1.NgaySanXuat,
		lh1.NgayHetHan
    	from
    	DonViQuiDoi dvqd1
    	left join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
		LEFT join DM_HangHoa_Anh an on (dvqd1.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    	left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa and (lh1.TrangThai = 1 or lh1.TrangThai is null)
		left join DM_GiaVon gv on dvqd1.ID = gv.ID_DonViQuiDoi and (lh1.ID = gv.ID_LoHang or lh1.ID is null) and gv.ID_DonVi = @ID_ChiNhanh
		left join DM_HangHoa_TonKho hhtonkho on dvqd1.ID = hhtonkho.ID_DonViQuyDoi and (hhtonkho.ID_LoHang = lh1.ID or lh1.ID is null) and hhtonkho.ID_DonVi = @ID_ChiNhanh
		where dvqd1.Xoa = 0 and dvqd1.MaHangHoa = @MaHH
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListNhanVienChuaCoND]
	@ID_ChiNhanh UNIQUEIDENTIFIER
AS
BEGIN
    select * from NS_NhanVien nv
	left join NS_QuaTrinhCongTac qtct on nv.ID = qtct.ID_NhanVien
	where (nv.TrangThai is null or nv.TrangThai = 1) and qtct.ID_DonVi = @ID_ChiNhanh and nv.DaNghiViec != 1 and  nv.ID not in (select ID_NhanVien from HT_nguoiDung)
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListTonTheoLoHangHoa]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN

	SELECT dmlo.ID as ID_LoHang, dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan, ISNULL(hhtonkho.TonKho, 0) as TonKho FROM
	DM_LoHang dmlo
	LEFT JOIN DonViQuiDoi dvqd on dmlo.ID_HangHoa = dvqd.ID_HangHoa
	LEFT JOIN DM_HangHoa_TonKho hhtonkho ON dvqd.ID = hhtonkho.ID_DonViQuyDoi AND hhtonkho.ID_DonVi = @ID_ChiNhanh AND hhtonkho.ID_LoHang = dmlo.ID
	WHERE dvqd.ID_HangHoa = @ID_HangHoa and dvqd.LaDonViChuan = 1
END");

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
   
    if(@MaHH = '%%')
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
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on gv.ID_DonViQuiDoi = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
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
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on gv.ID_DonViQuiDoi = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
				where dvqd.xoa = 0 and dvqd.ladonvichuan = 1 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
				and hh.TheoDoi = 1 and hh.LaHangHoa = 1
			Select * from #dmhanghoatable2 hhtb2
		END
    
    END
    	--end @MaHH = '%%'
    if(@MaHH != '%%')
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
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on gv.ID_DonViQuiDoi = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
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
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on gv.ID_DonViQuiDoi = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = lohang.ID
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

            Sql(@"ALTER PROCEDURE [dbo].[LoadGiaBanChiTiet]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_BangGia [nvarchar](max),
    @maHoaDon [nvarchar](max),
    @maHoaDonVie [nvarchar](max)
AS
BEGIN
    DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@maHoaDon+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@maHoaDonVie+',') where Name!='';
    	Select @count =  (Select count(*) from @tablename);
    	Select @countChar =   (Select count(*) from @tablenameChar);
    
    if(@ID_BangGia != '')
    	BEGIN
    		Select gbct.ID, dvqd.ID_HangHoa,hh.TenHangHoa,
    			hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
				dvqd.ThuocTinhGiaTri as HangHoaThuocTinh,
    			hh.QuanLyTheoLoHang, hh.NgayTao,dvqd.Xoa, hh.ID_NhomHang, dvqd.TenDonViTinh as DonViTinh, dvqd.GiaNhap as GiaNhapCuoi, gbct.GiaBan as GiaMoi, dvqd.MaHangHoa,
    		ISNULL(CAST(gv.GiaVon as FLOAT), 0) as GiaVon, dvqd.GiaBan, dvqd.GiaBan as GiaChung, dvqd.ID as IDQuyDoi, gbct.ID_GiaBan, nhh.TenNhomHangHoa
    		from DonViQuiDoi dvqd
    		left join DM_GiaBan_ChiTiet gbct on dvqd.ID = gbct.ID_DonViQuiDoi
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    		where dvqd.Xoa = 0 and hh.TheoDoi =1 and gbct.ID_Giaban = @ID_BangGia
			and ((select count(*) from @tablename b where 
    					hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    						or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
							or hh.TenHangHoa like '%'+b.Name+'%' 
    						or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    					and ((select count(*) from @tablenameChar c where
    						hh.TenHangHoa like '%'+c.Name+'%' 
    						or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
			 
    		order by gbct.NgayNhap desc
    	END
    	ELSE
    	BEGIN
    		Select dvqd.ID, dvqd.ID_HangHoa,hh.TenHangHoa,
    			hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
				dvqd.ThuocTinhGiaTri as HangHoaThuocTinh,
    			hh.QuanLyTheoLoHang,hh.NgayTao,dvqd.Xoa, hh.ID_NhomHang, dvqd.TenDonViTinh as DonViTinh, dvqd.GiaNhap as GiaNhapCuoi, dvqd.GiaBan as GiaMoi, dvqd.MaHangHoa,
    		ISNULL(CAST(gv.GiaVon as FLOAT), 0) as GiaVon, dvqd.GiaBan, dvqd.ID as IDQuyDoi, NEWID() as ID_GiaBan, nhh.TenNhomHangHoa
    		from DonViQuiDoi dvqd
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    				
    		where ((select count(*) from @tablename b where 
    					hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    						or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
							or hh.TenHangHoa like '%'+b.Name+'%' 
    						or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    					and ((select count(*) from @tablenameChar c where
    						hh.TenHangHoa like '%'+c.Name+'%' 
    						or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    			and dvqd.Xoa = 0 and hh.TheoDoi =1
    		order by hh.NgayTao desc	
    		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[selectHangHoa_XuatNhapTon]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON;
    SELECT  NEWID() AS ID, @ID_ChiNhanh as ID_DonVi, a.ID as ID_HangHoa,(a.TonCuoiKy / dvqd3.TyLeChuyenDoi) as TonKho,@timeEnd as  NgayChotSo, a.ID_LoHang as ID_LoHang FROM 
    (
    SELECT 
    dhh.ID,
    HangHoa.ID_LoHang,
    SUM(HangHoa.TonDau) AS TonCuoiKy
    FROM
    (
    SELECT
    td.ID_DonViQuiDoi,
    td.ID_LoHang,
    SUM(td.SoLuongNhap) - SUM(td.SoLuongXuat) AS TonDau,
    0 AS SoLuongNhap,
    0 AS SoLuongXuat
    FROM
    (
    SELECT 
    bhdct.ID_DonViQuiDoi,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi, bhdct.ID_LoHang                                                                                                                                                                                                                                                             
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    	bhdct.ID_LoHang,
    0 AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi,bhdct.ID_LoHang
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    	bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    0 AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi,bhdct.ID_LoHang
    
    UNION ALL
    SELECT 
    bhdct.ID_DonViQuiDoi,
    	bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    0 AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY bhdct.ID_DonViQuiDoi,bhdct.ID_LoHang
    ) AS td
    GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    ) 
    AS HangHoa
    --LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
	where dhh.LaHangHoa = 1
    --LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    GROUP BY dhh.ID, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	where dvqd3.ladonvichuan = 1
    order by MaHangHoa
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetAll_DMLoHang_TonKho]
    @ID_ChiNhanh [uniqueidentifier],
    @timeChotSo [datetime]
AS
BEGIN
SET NOCOUNT ON;
    Select
    		lh.ID,
    		tr.ID_DonViQuiDoi,
    		lh.ID_HangHoa,
    		MAX(tr.TenLoHang) as TenLoHangFull,
    		MAX(tr.TenLoHang) as MaLoHang,
    		MAX(tr.NgaySanXuat) as NgaySanXuat,
    		MAX(tr.NgayHetHan) as NgayHetHan,
    		ISNULL(lh.TrangThai,'1') as TrangThai,
    		Sum(tr.TonCuoiKy)  as TonKho		
    		 FROM
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    			a.TenHH AS TenHangHoa,   
    			dvqd3.TenDonViTinh, 
    			a.QuanLyTheoLoHang,
    			CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
    			Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,    
    			a.ID_LoHang,
    			a.MaLoHang as TenLoHang,
    			a.NgaySanXuat,
    			a.NgayHetHan,
    			a.TrangThai
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    			 Select  
    			 dhh1.ID,
    			 dhh1.LaHangHoa,
    			 dhh1.TenHangHoa as TenHH,
    			 dhh1.TenHangHoa,
    			 dhh1.TenHangHoa_KhongDau,
    			 dhh1.TenHangHoa_KyTuDau,
    			 dvqd1.TenDonViTinh,
    			 dhh1.QuanLyTheoLoHang,
    			 lh1.ID as ID_LoHang,
    			 lh1.MaLoHang,
    			 lh1.NgaySanXuat,
    			 lh1.NgayHetHan,
    			 lh1.TrangThai,
    			 0 as TonCuoiKy
    			 from
    			 DonViQuiDoi dvqd1 
    			 join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
    			 left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa
    			 Union all
    
    		SELECT 
    			dhh.ID,
    			dhh.LaHangHoa,
    			MAX(dhh.TenHangHoa) As TenHH,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' + MAX(dhh.TenHangHoa)  AS TenHangHoa,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' +  MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end)  + ' ' + MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    			MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    			dhh.QuanLyTheoLoHang,
    			MAX(lh.ID)  As ID_LoHang,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
    			MAX(lh.NgaySanXuat)  As NgaySanXuat,
    			MAX(lh.NgayHetHan)  As NgayHetHan,
    			lh.TrangThai,
    			SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
    			left join
    			(
    			SELECT
    				td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    				SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    					dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					NULL AS SoLuongNhap,
    					NULL AS SoLuongXuat,
    					SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    					where dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					NULL AS SoLuongNhap,
    					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                                     
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					NULL AS SoLuongNhap,
    					SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    					null AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    					null AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			Where dvqd.Xoa = '0' and dhh.TheoDoi = 1
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    where dvqd3.Xoa = '0'
    		and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
    		)
    		tr
    		inner join DM_LoHang lh on tr.ID_LoHang = lh.ID
    		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, tr.QuanLyTheoLoHang, lh.ID_HangHoa, lh.ID, lh.TrangThai
    		order by lh.ID, tr.ID_DonViQuiDoi
END

--SP_GetAll_DMLoHang_TonKho 'd93b17ea-89b9-4ecf-b242-d03b8cde71de','2016-01-01'");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetInforHoaDon_ByMaHoaDon]
    @MaHoaDon [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
    select 
    		hd.ID,
    		hd.MaHoaDon,
    		hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
    		hd.TongTienHang,
    		ISNULL(hd.TongGiamGia,0) + ISNULL(hd.KhuyeMai_GiamGia, 0) as TongGiamGia,
    		CAST(ISNULL(hd.PhaiThanhToan,0) as float)  as PhaiThanhToan,
    		CAST(ISNULL(TongThuChi,0) as float) as KhachDaTra,	
			CAST(ISNULL(TienDoiDiem,0) as float) as TienDoiDiem,	
			CAST(ISNULL(ThuTuThe,0) as float) as ThuTuThe,	
    		ISNULL(dt.TenDoiTuong,N'Khách lẻ')  as TenDoiTuong,
    		ISNULL(bg.TenGiaBan,N'Bảng giá chung') as TenBangGia,
    		ISNULL(nv.TenNhanVien,N'')  as TenNhanVien,
    		ISNULL(dv.TenDonVi,N'')  as TenDonVi,    		
    		case when hd.NgayApDungGoiDV is null then '' else  convert(varchar(14), hd.NgayApDungGoiDV ,103) end  as NgayApDungGoiDV,
    		case when hd.HanSuDungGoiDV is null then '' else  convert(varchar(14), hd.HanSuDungGoiDV ,103) end as HanSuDungGoiDV,
    		hd.NguoiTao as NguoiTaoHD,
    		hd.DienGiai,
    		hd.ID_DonVi,
    		-- get avoid error at variable at class BH_HoaDonDTO
    		NEWID() as ID_DonViQuiDoi,
    
    
    		case 
    			when hd.LoaiHoaDon = 1 OR hd.LoaiHoaDon =6 OR hd.LoaiHoaDon =19 then
    									case when hd.ChoThanhToan is null then N'Đã hủy'
    									else N'Hoàn thành' end
    			when hd.LoaiHoaDon = 3 then
    									case when hd.YeuCau ='1' then N'Phiếu tạm'
    									else 
    										case when hd.YeuCau ='2' then  N'Đang giao hàng'
    										else 
    											case when hd.YeuCau ='3' then N'Hoàn thành'
    											else  N'Đã hủy' end
    										end
    									end
    		end as TrangThai													
    	from BH_HoaDon hd
    	left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    	left join DM_DonVi dv on hd.ID_DonVi= dv.ID
    	left join DM_GiaBan bg on hd.ID_BangGia= bg.ID
    	left join 
    		(select ID_HoaDonLienQuan, SUM(TongThuChi) as TongThuChi, SUM(TienDoiDiem) as TienDoiDiem, SUM(ThuTuThe) as ThuTuThe  from 
				(
					select qct.ID_HoaDonLienQuan, ISNULL(qct.TienThu,0) as TongThuChi,
						
						case when qhd.LoaiHoaDon = 11 then 
										case when ISNULL(qct.DiemThanhToan, 0) = 0 then 0 else ISNULL(qct.TienThu, 0) end
									else case when ISNULL(qct.DiemThanhToan, 0) = 0 then 0 else -ISNULL(qct.TienThu, 0) end end  as TienDoiDiem,
    					Case when qhd.LoaiHoaDon = 11 then ISNULL(qct.ThuTuThe, 0) else -ISNULL(qct.ThuTuThe, 0) end  as ThuTuThe
    				from Quy_HoaDon_ChiTiet qct
    				left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID where qhd.TrangThai ='1'
    				) tbl group by tbl.ID_HoaDonLienQuan
			) soquy on hd.ID = soquy.ID_HoaDonLienQuan		
    	where hd.MaHoaDon = @MaHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhSLTon]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT SUM(TonKho) as TonKho FROM DM_HangHoa_TonKho hhtonkho
	left join DonViQuiDoi dvqd on hhtonkho.ID_DonViQuyDoi = dvqd.ID
	WHERE dvqd.ID_HangHoa = @ID_HangHoa and dvqd.LaDonViChuan = 1 and hhtonkho.ID_DonVi = @ID_ChiNhanh
	GROUP BY ID_HangHoa
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhTonTheoLoHangHoa]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT hhtonkho.TonKho, ISNULL(gv.GiaVon, 0) as GiaVon, tbl1.DonGia as GiaNhap
	FROM DonViQuiDoi dvqd
	LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh and hhtonkho.ID_LoHang = @ID_LoHang
	LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = @ID_LoHang
	LEFT JOIN
		(select TOP(1) ct.ID_DonViQuiDoi, ct.DonGia from BH_HoaDon hd INNER JOIN
		BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
		where hd.LoaiHoaDon = '4' and ct.ID_LoHang = @ID_LoHang and hd.ID_DonVi = @ID_ChiNhanh
		ORDER BY NgayLapHoaDon DESC) as tbl1 on dvqd.ID = tbl1.ID_DonViQuiDoi
		WHERE dvqd.ID_HangHoa = @ID_HangHoa
END
");

            Sql(@"ALTER PROCEDURE [dbo].[TraCuuHangHoa]
    @ID_ChiNhanh [uniqueidentifier],
    @MaHH [nvarchar](max),
    @MaHHCoDau [nvarchar](max)
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
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHHCoDau+',') where Name!='';
    	  Select @count =  (Select count(*) from @tablename);
    	    Select @countChar =   (Select count(*) from @tablenameChar);
    DECLARE @TenDonVi nvarchar(max);
    	SELECT @TenDonVi = TenDonVi FROM DM_DonVi WHERE ID = @ID_ChiNhanh
    SELECT @ID_ChiNhanh as ID_DonVi,@TenDonVi as TenDonVi, dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHangHoa,a.LaHangHoa ,a.ID,hh3.ID_HangHoaCungLoai, dvqd3.TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh,hh3.TheoDoi as TrangThai,hh3.QuanLyTheoLoHang,
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,  
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonKho
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT
    		dhh.ID,
    		dhh.LaHangHoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh) AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
    			left join
    			(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    				--SUM(ISNULL(td.TonKho,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					where dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgaySua >= @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		--LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			Where dvqd.Xoa = 0 and dhh.TheoDoi = 1
    		GROUP BY dhh.ID,dhh.LaHangHoa
    ) a
    LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	left join DM_HangHoa hh3 on dvqd3.ID_HangHoa = hh3.ID
    		LEFT JOIN (Select Main.id_hanghoa,
    						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    						From
    						(
    						Select distinct hh_tt.id_hanghoa,
    							(
    								Select tt.GiaTri + ' - ' AS [text()]
    								From dbo.hanghoa_thuoctinh tt
    								Where tt.id_hanghoa = hh_tt.id_hanghoa
    								order by tt.ThuTuNhap 
    								For XML PATH ('')
    							) hanghoa_thuoctinh
    						From dbo.hanghoa_thuoctinh hh_tt
    						) Main
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where ((select count(*) from @tablename b where 
    	dvqd3.MaHangHoa like '%'+b.Name+'%' 
    		or a.TenHangHoa_KhongDau like '%'+b.Name+'%'
			or a.TenHangHoa like '%'+b.Name+'%' 
    		or a.TenHangHoa_KyTuDau like '%'+b.Name+'%' )=@count or @count=0)
    	and ((select count(*) from @tablenameChar c where
    		dvqd3.MaHangHoa like '%'+c.Name+'%' 
    		or a.TenHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    	--(dvqd3.MaHangHoa like @MaHH or a.TenHangHoa_KhongDau like @MaHH or a.TenHangHoa_KyTuDau like @MaHH or dvqd3.MaHangHoa like @MaHH_TV)
    	and dvqd3.Xoa = 0 and hh3.LaHangHoa = 1
    		order by MaHangHoa
    		--and RowNum >= (@currentPage * @pageSize) + 1 AND RowNum <= (@currentPage * @pageSize) + @pageSize
END");

            Sql(@"
DECLARE @DViCount INT;
SELECT @DViCount = COUNT(ID) FROM DM_DonVi;
IF(@DViCount != 0)
BEGIN
	--======== DM_DoiTuong =====
update DM_DoiTuong set TenDoiTuong_KhongDau ='' where TenDoiTuong_KhongDau is null;
update DM_DoiTuong set TenDoiTuong_ChuCaiDau ='' where TenDoiTuong_ChuCaiDau is null;
update DM_DoiTuong set DienThoai ='' where DienThoai is null;
update DM_DoiTuong set Email ='' where Email is null;
update DM_DoiTuong set MaSoThue ='' where MaSoThue is null;
update DM_DoiTuong set GhiChu ='' where GhiChu is null;
update DM_DoiTuong set DiaChi ='' where DiaChi is null;
update DM_DoiTuong set GioiTinhNam ='1' where GioiTinhNam is null;
update DM_DoiTuong set NguoiTao ='' where NguoiTao is null;
UPDATE dt
	SET dt.IDNhomDoiTuongs = ISNULL(DoiTuong_Nhom.ID_NhomDoiTuong, '00000000-0000-0000-0000-000000000000') , 
		dt.TenNhomDoiTuongs = ISNULL(DoiTuong_Nhom.TenNhomDT,N'Nhóm mặc định')
	from DM_DoiTuong AS dt
	Left join (Select Main.ID as ID_DoiTuong,
    			Left(Main.dt_n,Len(Main.dt_n)-1) As TenNhomDT,
    			Left(Main.id_n,Len(Main.id_n)-1) As ID_NhomDoiTuong
    			From
    			(
    				Select distinct hh_tt.ID,
    				(
    					Select ndt.TenNhomDoiTuong + ', ' AS [text()]
    					From dbo.DM_DoiTuong_Nhom dtn
    					inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    					Where dtn.ID_DoiTuong = hh_tt.ID
    					order by ndt.TenNhomDoiTuong
    					For XML PATH ('')
    				) dt_n,
    				(
    				Select convert(nvarchar(max), ndt.ID)  + ', ' AS [text()]
    				From dbo.DM_DoiTuong_Nhom dtn
    				inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    				Where dtn.ID_DoiTuong = hh_tt.ID
    				order by ndt.TenNhomDoiTuong
    				For XML PATH ('')
    				) id_n
				From dbo.DM_DoiTuong hh_tt
				) Main) as DoiTuong_Nhom on dt.ID = DoiTuong_Nhom.ID_DoiTuong
	where IDNhomDoiTuongs is null;

--======== DM_LienHe =====
update DM_LienHe set SoDienThoai ='' where SoDienThoai is null;
update DM_LienHe set DienThoaiCoDinh ='' where DienThoaiCoDinh is null;
update DM_LienHe set Email ='' where Email is null;
update DM_LienHe set GhiChu ='' where GhiChu is null;
update DM_LienHe set NguoiTao ='' where NguoiTao is null;
update DM_LienHe set DiaChi ='' where DiaChi is null;
update DM_LienHe set TrangThai =1 where TrangThai is null;

--======== DM_NhomDoiTuong =====
update DM_NhomDoiTuong set TenNhomDoiTuong_KhongDau ='' where TenNhomDoiTuong_KhongDau is null;
update DM_NhomDoiTuong set TenNhomDoiTuong_KyTuDau ='' where TenNhomDoiTuong_KyTuDau is null;
update DM_NhomDoiTuong set GiamGia = 0 where GiamGia is null;
update DM_NhomDoiTuong set GiamGiaTheoPhanTram ='' where GiamGiaTheoPhanTram is null;
update DM_NhomDoiTuong set TrangThai ='1' where TrangThai is null;
update DM_NhomDoiTuong set TuDongCapNhat ='0' where TuDongCapNhat is null;

--======== DM_NhomDoiTuong_ChiTiet =====
update DM_NhomDoiTuong_ChiTiet set GiaTriSo =0  where GiaTriSo is null;
update DM_NhomDoiTuong_ChiTiet set GiaTriBool ='0'  where GiaTriBool is null;

--======== BH_HoaDon =====
update BH_HoaDon set ID_DoiTuong ='00000000-0000-0000-0000-000000000000' where LoaiHoaDon in (1,3,6,19,22,23) and ID_DoiTuong is null;
update BH_HoaDon set KhuyeMai_GiamGia = 0  where KhuyeMai_GiamGia is null and ChoThanhToan = '0';
update BH_HoaDon set DiemGiaoDich = 0  where DiemGiaoDich is null and ChoThanhToan = '0';

--======== BH_HoaDon_ChiTiet =====
update BH_HoaDon_ChiTiet set ThoiGianBaoHanh = 0  where ThoiGianBaoHanh is null;
update BH_HoaDon_ChiTiet set LoaiThoiGianBH = 0  where LoaiThoiGianBH is null;
update BH_HoaDon_ChiTiet set PTThue = 0  where PTThue is null;
update BH_HoaDon_ChiTiet set SoLuongDinhLuong_BanDau = 0  where SoLuongDinhLuong_BanDau is null;

--======== BH_NhanVienThucHien =====
update BH_NhanVienThucHien set HeSo = 1  where HeSo is null;
update BH_NhanVienThucHien set HeSo = 1  where HeSo is null;

--Quy_HoaDon--
UPDATE qct SET qct.ID_DoiTuong = hd.ID_DoiTuong FROM Quy_HoaDon_ChiTiet AS qct INNER JOIN BH_HoaDon AS hd ON qct.ID_HoaDonLienQuan = hd.ID WHERE qct.ID_DoiTuong IS NULL;
UPDATE Quy_HoaDon_ChiTiet SET ID_DoiTuong = '00000000-0000-0000-0000-000000000000' WHERE ID_DoiTuong IS NULL
-- xoa dich vu trong ChotSo_HangHoa
delete ChotSo_HangHoa where ID in
(select cs.ID from ChotSo_HangHoa cs
join DM_HangHoa hh on cs.ID_HangHoa = hh.ID where hh.LaHangHoa = 0);
END
");

            Sql(@"CREATE trigger [dbo].[trg_DMDoiTuong] on [dbo].[DM_DoiTuong]
for insert, update
as 
	set nocount on
	declare @IDDoiTuong UNIQUEIDENTIFIER = (select ID from inserted)
	exec UpdateNhomDoiTuongs_ByID @IDDoiTuong");

            Sql(@"CREATE TRIGGER [dbo].[UpdateHangHoaThuocTinh] on [dbo].[HangHoa_ThuocTinh]
FOR INSERT, UPDATE
AS
BEGIN
	DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
	select @ID_HangHoa = ID_HangHoa from inserted

	UPDATE dvqd
	SET dvqd.ThuocTinhGiaTri = '_' + ThuocTinh.ThuocTinh_GiaTri
	FROM DonViQuiDoi dvqd
	LEFT JOIN (Select Main.id_hanghoa,
    						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    						From
    						(
    						Select distinct hh_tt.id_hanghoa,
    							(
    								Select tt.GiaTri + ' - ' AS [text()]
    								From dbo.hanghoa_thuoctinh tt
    								Where tt.id_hanghoa = hh_tt.id_hanghoa
    								order by tt.ThuTuNhap 
    								For XML PATH ('')
    							) hanghoa_thuoctinh
    						From dbo.hanghoa_thuoctinh hh_tt
    						) Main
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
	 WHERE dvqd.ID_HangHoa = @ID_HangHoa 
END
");

            Sql(@"CREATE trigger [dbo].[UpdateThuocTinhKhiDelete] on [dbo].[HangHoa_ThuocTinh]
for delete
as 
	set nocount on
	DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
	select @ID_HangHoa = ID_HangHoa from deleted

	UPDATE dvqd
	SET dvqd.ThuocTinhGiaTri = '_' + ThuocTinh.ThuocTinh_GiaTri
	FROM DonViQuiDoi dvqd
	LEFT JOIN (Select Main.id_hanghoa,
    						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    						From
    						(
    						Select distinct hh_tt.id_hanghoa,
    							(
    								Select tt.GiaTri + ' - ' AS [text()]
    								From dbo.hanghoa_thuoctinh tt
    								Where tt.id_hanghoa = hh_tt.id_hanghoa
    								order by tt.ThuTuNhap 
    								For XML PATH ('')
    							) hanghoa_thuoctinh
    						From dbo.hanghoa_thuoctinh hh_tt
    						) Main
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
	 WHERE dvqd.ID_HangHoa = @ID_HangHoa ");

            Sql(@"CREATE trigger [dbo].[trg_DeleteNhomDoiTuongs] on [dbo].[DM_DoiTuong_Nhom]
for delete
as 
	set nocount on
	declare @IDDoiTuong UNIQUEIDENTIFIER = (select top 1 ID_DoiTuong from deleted)
	exec UpdateNhomDoiTuongs_ByID @IDDoiTuong");

            Sql(@"CREATE trigger [dbo].[trg_UpdateNhomDoiTuongs] on [dbo].[DM_DoiTuong_Nhom]
for insert, update
as 
	set nocount on
	declare @IDDoiTuong UNIQUEIDENTIFIER = (select ID_DoiTuong from inserted)
	exec UpdateNhomDoiTuongs_ByID @IDDoiTuong");

            DropStoredProcedure("[dbo].[LoadAllDanhMucHangHoa]");
            DropStoredProcedure("[dbo].[LoadHangHoaCungLoai]");
            DropStoredProcedure("[dbo].[LoadFirstDanhMucHangHoa]");
            DropStoredProcedure("[dbo].[LoadFirstDanhMucHangHoaSort]");
            DropStoredProcedure("[dbo].[LoadFirstPageCountHH]");
            DropStoredProcedure("[dbo].[TinhTonDanhMucHangHoa]");
            DropStoredProcedure("[dbo].[TinhTonFirstDanhMucHangHoa]");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[insert_DM_HangHoaTonKHo]");
            DropStoredProcedure("[dbo].[insert_TonKhoKhoiTao]");
            DropStoredProcedure("[dbo].[insert_TonKhoKhoiTaoByInsert]");
            DropStoredProcedure("[dbo].[UpdateNhomDoiTuongs_ByID]");
            DropStoredProcedure("[dbo].[UpdateTonForDM_hangHoa_TonKho]");
            DropStoredProcedure("[dbo].[UpdateTonKhoChoChiNhanhMoi]");
        }
    }
}
