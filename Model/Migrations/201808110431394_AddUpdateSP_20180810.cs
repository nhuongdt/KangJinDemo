namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180810 : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[LoadAllDanhMucHangHoa]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select bb.ID_DonViQuiDoi, bb.ID_HangHoaCungLoai,bb.GiaVon, bb.GiaBan,
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho FROM (
    (SELECT 
    				dvqd.ID,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID
					) aa
    right join
    (
    SELECT  dvqd3.ID as ID_DonViQuiDoi,dhh3.ID_HangHoaCungLoai, dvqd3.GiaVon, dvqd3.GiaBan, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    (
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
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
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		where dvqd3.xoa is null and dvqd3.ladonvichuan = 1 and dhh3.TheoDoi =1
    	)bb on aa.ID = bb.ID_DonViQuiDoi)");

            AlterStoredProcedure(name: "[dbo].[LoadFirstDanhMucHangHoa]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                currentPage = p.Int(),
                pageSize = p.Int()
            }, body: @"DECLARE @TonKhoTheoChiNhanh TABLE(
    		ID_DonViQuiDoi uniqueidentifier,
    		ID_HangHoaCungLoai uniqueidentifier,
    		GiaVon float,
    		GiaBan float,
    		TonKho float
    	);
    	INSERT INTO @TonKhoTheoChiNhanh exec LoadAllDanhMucHangHoa @ID_ChiNhanh
    	
    DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select bb.ID_DonViQuiDoi,bb.ID_HangHoa as ID,bb.TonToiThieu,bb.GhiChu, bb.QuanLyTheoLoHang, bb.TonToiDa, bb.LaHangHoa,
	bb.LaChaCungLoai, bb.DuocBanTrucTiep,bb.TrangThai,bb.NgayTao, bb.ID_HangHoaCungLoai, bb.MaHangHoa, bb.ID_NhomHangHoa,
	bb.TenNhomHangHoa as NhomHangHoa, bb.TenHangHoa, bb.TenDonViTinh, bb.TenHangHoa_KhongDau, bb.TenHangHoa_KyTuDau, bb.GiaVon, bb.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho INTO #tblTonFirst FROM (
    (SELECT 
    				dvqd.ID,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID
					) aa
    right join
    (
    SELECT ROW_NUMBER() OVER (ORDER BY dhh3.NgayTao desc) AS RowNum,dvqd3.ID as ID_DonViQuiDoi,dvqd3.ID_HangHoa,dhh3.TonToiDa, dhh3.TonToiThieu,dhh3.GhiChu,dhh3.QuanLyTheoLoHang,dhh3.LaHangHoa,
	dhh3.LaChaCungLoai,dhh3.DuocBanTrucTiep,dhh3.TheoDoi as TrangThai,dhh3.NgayTao,dhh3.ID_HangHoaCungLoai,dnhh3.ID as ID_NhomHangHoa,
	dnhh3.TenNhomHangHoa, dvqd3.mahanghoa, dhh3.TenHangHoa,dhh3.TenHangHoa_KhongDau, dhh3.TenHangHoa_KyTuDau,dvqd3.GiaVon,dvqd3.GiaBan,
	dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    (
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
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
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		where dvqd3.xoa is null and dvqd3.ladonvichuan = 1 and dhh3.LaChaCungLoai = 1 and dhh3.TheoDoi =1
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    	Where RowNum >= (@currentPage * @pageSize) + 1 AND RowNum <= (@currentPage * @pageSize) + @pageSize
    
    		Select 
    			 hhtb2.ID_DonViQuiDoi,
    			 hhtb2.ID,
    			 hhtb2.LaHangHoa, 
    			 hhtb2.LaChaCungLoai, 
    			 hhtb2.DuocBanTrucTiep,
    			 hhtb2.TrangThai,
    			 hhtb2.NgayTao, 
    			 hhtb2.ID_HangHoaCungLoai, 
    			 hhtb2.MaHangHoa, 
    			 hhtb2.ID_NhomHangHoa, 
    			 hhtb2.NhomHangHoa, 
    			 hhtb2.TenHangHoa, 
    			 hhtb2.TenDonViTinh, 
    			 hhtb2.TenHangHoa_KhongDau, 
    			 hhtb2.TenHangHoa_KyTuDau, 
    			 hhtb2.TonToiThieu,
    			 hhtb2.TonToiDa,
				 hhtb2.GhiChu, 
				 hhtb2.QuanLyTheoLoHang,
    			 CASE
    					WHEN hhtb2.LaChaCungLoai = '1'
    						THEN
    							(SELECT SUM(tmphhtb2.GiaBan) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)/(SELECT Count(tmphhtb2.GiaBan) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)
    					ELSE
    						hhtb2.GiaBan
    				END AS GiaBan,
    			 CASE
    					WHEN hhtb2.LaChaCungLoai = '1'
    						THEN
    							(SELECT SUM(tmphhtb2.GiaVon) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)/(SELECT Count(tmphhtb2.GiaVon) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)
    					ELSE
    						hhtb2.GiaVon
    				END AS GiaVon,
    			 CASE
    					WHEN hhtb2.LaChaCungLoai = '1'
    						THEN
    							(SELECT SUM(tmphhtb2.TonKho) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)
    					ELSE
    						hhtb2.TonKho
    				END AS TonKho
    		from #tblTonFirst hhtb2");

            AlterStoredProcedure(name: "[dbo].[TinhTonDanhMucHangHoa]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHHCoDau = p.String(),
                ListID_NhomHang = p.String(),
                ID_ChiNhanh = p.Guid(),
                KinhDoanhFilter = p.String(),
                LaHangHoaFilter = p.String(),
                List_ThuocTinh = p.String()
            }, body: @"DECLARE @timeStart Datetime
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

    if (@ListID_NhomHang = '%%')
    --Begin @ListID_NhomHang = '%%'
    	BEGIN
    	if(@MaHH = '%%')
    	--Begin @MaHH = '%%'	
    	BEGIN
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    	
    		Select bb.ID_DonViQuiDoi,bb.ThuocTinh,bb.TonToiThieu, bb.TonToiDa, bb.ID_HangHoaCungLoai,bb.LaChaCungLoai, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable FROM (
    		(SELECT 
    				dvqd.ID,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID
					) aa
    		right join
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi,CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, dhh3.TonToiThieu, dhh3.TonToiDa,
			dhh3.ID_HangHoaCungLoai, dhh3.LaChaCungLoai, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    		( 
    		SELECT 
    		dhh.ID,
    		dhh.TenHangHoa,
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
    		WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    		AND bhd.NgayLapHoaDon >= @timeStart
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    	    
    		UNION ALL
    		SELECT
    		bhdct.ID_DonViQuiDoi,
    		NULL AS SoLuongNhap,
    		SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    		FROM BH_HoaDon_ChiTiet bhdct
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    		OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		AND bhd.NgayLapHoaDon >= @timeStart
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
    		WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0  and hh.LaHangHoa = 1
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		AND bhd.NgayLapHoaDon >= @timeStart
    		GROUP BY bhdct.ID_DonViQuiDoi
    	    
    		UNION ALL
    		SELECT
    		bhdct.ID_DonViQuiDoi,
    		SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    		null AS SoLuongXuat
    		FROM BH_HoaDon_ChiTiet bhdct
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    		AND bhd.NgayLapHoaDon >= @timeStart 
    		GROUP BY bhdct.ID_DonViQuiDoi 
    		) AS td 
    		GROUP BY td.ID_DonViQuiDoi
    		)
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    	    
    		GROUP BY dhh.ID, dhh.TenHangHoa
    		) a
    		right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
			LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
			left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
			where dvqd3.xoa is null and dvqd3.ladonvichuan = 1
    		and dhh3.TheoDoi like @KinhDoanhFilter and dhh3.LaHangHoa like @LaHangHoaFilter
    		)bb on aa.ID = bb.ID_DonViQuiDoi)
    		order by TonCuoiKy desc
    	
    	if(@List_ThuocTinh != '')
    		BEGIN
    		Select * from #dmhanghoatable hhtb2
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    		END
    		ELSE
    		BEGIN
    		Select * from #dmhanghoatable hhtb2
    		END
    	
    	END
    	--end @MaHH = '%%'
    	if(@MaHH != '%%')
    	--begin @MaHH != '%%'
    	BEGIN
    		Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select bb.ID_DonViQuiDoi,bb.ThuocTinh,bb.TonToiThieu, bb.TonToiDa, bb.ID_HangHoaCungLoai,bb.LaChaCungLoai, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable2 FROM (
    		(SELECT 
    				dvqd.ID,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID
					) aa
    right join
    (
    SELECT dvqd3.ID as ID_DonViQuiDoi,CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, dhh3.TonToiThieu, dhh3.TonToiDa,
			dhh3.ID_HangHoaCungLoai, dhh3.LaChaCungLoai, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    ( 
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
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
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
		left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		where 
		((select count(*) from @tablename b where 
		dhh3.TenHangHoa_KhongDau like '%'+b.Name+'%' 
		or dhh3.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
		or dvqd3.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
		and ((select count(*) from @tablenameChar c where
		dhh3.TenHangHoa like '%'+c.Name+'%' 
		or dvqd3.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
		--(hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa like @MaHH or dvqd.MaHangHoa like @MaHHCoDau) 
		and dvqd3.xoa is null and dvqd3.ladonvichuan = 1
		--	and (hh.LaChaCungLoai =(case when @List_ThuocTinh != '' then 0 else 1 end) or hh.LaChaCungLoai = (case when @List_ThuocTinh ='' then 1 else 1 end))
		and dhh3.TheoDoi like @KinhDoanhFilter and dhh3.LaHangHoa like @LaHangHoaFilter
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    	order by TonCuoiKy desc
    	
    	if(@List_ThuocTinh != '')
    BEGIN
    Select * from #dmhanghoatable2 hhtb2
    	where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    END
    ELSE
    BEGIN
    		Select * from #dmhanghoatable2 hhtb2
    END
    	END
    --end @MaHH != '%%'
    	END
    --END @ListID_NhomHang = '%%'	
    	
    --Begin @ListID_NhomHang != '%%'
    if (@ListID_NhomHang != '%%')
    BEGIN
    if(@MaHH = '%%')
    	--begin @MaHH = '%%'
    	BEGIN
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select bb.ID_DonViQuiDoi,bb.ThuocTinh,bb.TonToiThieu, bb.TonToiDa, bb.ID_HangHoaCungLoai,bb.LaChaCungLoai, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable1 FROM (
    		(SELECT 
    				dvqd.ID,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID
					) aa
    right join
    (
    SELECT dvqd3.ID as ID_DonViQuiDoi,CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, dhh3.TonToiThieu, dhh3.TonToiDa,
			dhh3.ID_HangHoaCungLoai, dhh3.LaChaCungLoai, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    ( 
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
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
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
		left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		where dvqd3.xoa is null and dvqd3.ladonvichuan = 1 and dnhh3.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh3.ID)
    	and dhh3.TheoDoi like @KinhDoanhFilter and dhh3.LaHangHoa like @LaHangHoaFilter
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    	order by TonCuoiKy desc
    
    	if(@List_ThuocTinh != '')
    BEGIN
    Select * from #dmhanghoatable1 hhtb2
    	    where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    END
    ELSE
    BEGIN
    Select * from #dmhanghoatable1 hhtb2
    END
    
    	END
    	--end @MaHH = '%%'
    	if(@MaHH != '%%')
    	--begin @MaHH != '%%'
    	BEGIN
    		Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select bb.ID_DonViQuiDoi,bb.ThuocTinh,bb.TonToiThieu, bb.TonToiDa, bb.ID_HangHoaCungLoai,bb.LaChaCungLoai, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable3 FROM (
    		(SELECT 
    				dvqd.ID,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID
					) aa
    right join
    (
    SELECT dvqd3.ID as ID_DonViQuiDoi,CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, dhh3.TonToiThieu, dhh3.TonToiDa,
			dhh3.ID_HangHoaCungLoai, dhh3.LaChaCungLoai, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    ( 
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
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
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
		left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		where 
		((select count(*) from @tablename b where 
		dhh3.TenHangHoa_KhongDau like '%'+b.Name+'%' 
		or dhh3.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
		or dvqd3.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
		and ((select count(*) from @tablenameChar c where
		dhh3.TenHangHoa like '%'+c.Name+'%' 
		or dvqd3.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
		--(hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa like @MaHH or dvqd.MaHangHoa like @MaHHCoDau) 
		and dvqd3.xoa is null and dvqd3.ladonvichuan = 1 and dnhh3.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh3.ID)
		--	and (hh.LaChaCungLoai =(case when @List_ThuocTinh != '' then 0 else 1 end) or hh.LaChaCungLoai = (case when @List_ThuocTinh ='' then 1 else 1 end))
		and dhh3.TheoDoi like @KinhDoanhFilter and dhh3.LaHangHoa like @LaHangHoaFilter
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    	order by TonCuoiKy desc
    
    	if(@List_ThuocTinh != '')
    BEGIN
    Select * from #dmhanghoatable3 hhtb2	
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    END
    ELSE
    BEGIN
    Select * from #dmhanghoatable3 hhtb2	
    END
    	END
    --end @MaHH != '%%'
    	END
    	--END @ListID_NhomHang != '%%'");

            AlterStoredProcedure(name: "[dbo].[TinhTonFirstDanhMucHangHoa]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select bb.ID_DonViQuiDoi, bb.TonToiThieu, bb.TonToiDa,bb.LaChaCungLoai, bb.ID_HangHoaCungLoai, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho INTO #tblTonFirst FROM (
    (SELECT 
    				dvqd.ID,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dvqd.ID
					) aa
    right join
    (
    SELECT dvqd3.ID as ID_DonViQuiDoi, dhh3.TonToiThieu, dhh3.TonToiDa,
			dhh3.ID_HangHoaCungLoai, dhh3.LaChaCungLoai, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    (
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
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
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		where dvqd3.xoa is null and dvqd3.ladonvichuan = 1 and dhh3.TheoDoi =1
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    
    		Select 
    			 hhtb2.ID_DonViQuiDoi,
    			 hhtb2.ID_HangHoaCungLoai,
    			 hhtb2.LaChaCungLoai,
    			 hhtb2.TonToiThieu,
    			 hhtb2.TonToiDa,	
    			 hhtb2.TonKho   				
    		from #tblTonFirst hhtb2");

            CreateStoredProcedure(name: "[dbo].[getList_DMLoHang_TonKho]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuiDoi = p.String(),
                timeChotSo = p.DateTime()
            }, body: @"SELECT 
		lh.ID as ID_LoHang,
		lh.ID_HangHoa,
		lh.TenLoHang as TenLoHangFull,
		lh.MaLoHang as TenLoHang,
		lh.NgaySanXuat,
		lh.NgayHetHan,
		CAST(ROUND((ISNULL(HangHoa.TonCuoiKy,0) / dvqd.TyLeChuyenDoi), 3) as float) as TonKho
    	FROM
		   DM_LoHang lh
		   left join
    		(
    		SELECT
    		td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
			td.ID_LoHang,
    		SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			--SUM(ISNULL(td.TonKho,0)) AS TonCuoiKy
    		FROM
    		(
    		-- lấy danh sách hàng hóa tồn kho
    			SELECT 
    			dvqd.ID As ID_DonViQuiDoi,
				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(cs.TonKho, 0)) as TonKho
    			FROM DonViQuiDoi dvqd
    			left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
					and dvqd.ID = @ID_DonViQuiDoi
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    			OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    		) AS td
			GROUP BY ID_DonViQuiDoi, ID_LoHang
		) AS HangHoa
		on lh.ID = HangHoa.ID_LoHang
		inner join DM_HangHoa hh on lh.ID_HangHoa = hh.ID
		inner join DonViQuiDoi dvqd on hh.ID = dvqd.ID_HangHoa
		where dvqd.ID = @ID_DonViQuiDoi
		order by lh.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[getList_DMLoHangByIDDonViQD]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                timeChotSo = p.DateTime()
            }, body: @"SELECT lh.ID,
		lh.ID_HangHoa,
		dvqd.ID as ID_DonViQuiDoi,
		lh.MaLoHang,
		lh.NgaySanXuat,
		lh.NgayHetHan,
		CAST(ROUND((ISNULL(HangHoa.TonCuoiKy,0) / dvqd.TyLeChuyenDoi), 3) as float) as TonKho
    	FROM
		   DM_LoHang lh
		   left join
    		(
    		SELECT
    		td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
			td.ID_LoHang,
    		SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			--SUM(ISNULL(td.TonKho,0)) AS TonCuoiKy
    		FROM
    		(
    		-- lấy danh sách hàng hóa tồn kho
    			SELECT 
    			dvqd.ID As ID_DonViQuiDoi,
				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(cs.TonKho, 0)) as TonKho
    			FROM DonViQuiDoi dvqd
    			left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
					and dvqd.ID = @ID_DonViQuiDoi
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    			OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    		) AS td
			GROUP BY ID_DonViQuiDoi, ID_LoHang
		) AS HangHoa
		on lh.ID = HangHoa.ID_LoHang
		inner join DM_HangHoa hh on lh.ID_HangHoa = hh.ID
		inner join DonViQuiDoi dvqd on hh.ID = dvqd.ID_HangHoa
		where dvqd.ID = @ID_DonViQuiDoi
		order by lh.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[getList_DMLoHangTonKho_byID]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                ID_LoHang = p.Guid(),
                timeChotSo = p.DateTime()
            }, body: @"SELECT CAST(ROUND((ISNULL(HangHoa.TonCuoiKy,0) / dvqd.TyLeChuyenDoi), 3) as float) as TonKho
    	FROM
		   DM_LoHang lh
		   left join
    		(
    		SELECT
    		td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
			td.ID_LoHang,
    		SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			--SUM(ISNULL(td.TonKho,0)) AS TonCuoiKy
    		FROM
    		(
    		-- lấy danh sách hàng hóa tồn kho
    			SELECT 
    			dvqd.ID As ID_DonViQuiDoi,
				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(cs.TonKho, 0)) as TonKho
    			FROM DonViQuiDoi dvqd
    			left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
					and dvqd.ID = @ID_DonViQuiDoi
					and cs.ID_LoHang = @ID_LoHang
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dvqd.ID = @ID_DonViQuiDoi
				AND bhdct.ID_LoHang = @ID_LoHang
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    			OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
				AND bhdct.ID_LoHang = @ID_LoHang
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
				AND bhdct.ID_LoHang = @ID_LoHang
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				AND dvqd.ID = @ID_DonViQuiDoi
				AND bhdct.ID_LoHang = @ID_LoHang
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    		) AS td
			GROUP BY ID_DonViQuiDoi, ID_LoHang
		) AS HangHoa
		on lh.ID = HangHoa.ID_LoHang
		inner join DM_HangHoa hh on lh.ID_HangHoa = hh.ID
		inner join DonViQuiDoi dvqd on hh.ID = dvqd.ID_HangHoa
		where dvqd.ID = @ID_DonViQuiDoi
		AND lh.ID = @ID_LoHang
		order by lh.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[SP_GetAll_DMLoHang_TonKho]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                timeChotSo = p.DateTime()
            }, body: @"SELECT lh.ID as ID,
		lh.ID_HangHoa,
		dvqd.ID as ID_DonViQuiDoi,
		lh.MaLoHang,
		lh.NgaySanXuat,
		lh.NgayHetHan,
		CAST(ROUND((ISNULL(HangHoa.TonCuoiKy,0) / dvqd.TyLeChuyenDoi), 3) as float) as TonKho
    	FROM
		   DM_LoHang lh
		   left join
    		(
    		SELECT
    		td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
			td.ID_LoHang,
    		SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			--SUM(ISNULL(td.TonKho,0)) AS TonCuoiKy
    		FROM
    		(
    		-- lấy danh sách hàng hóa tồn kho
    			SELECT 
    			dvqd.ID As ID_DonViQuiDoi,
				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(cs.TonKho, 0)) as TonKho
    			FROM DonViQuiDoi dvqd
    			left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
					--and dvqd.ID = @ID_DonViQuiDoi
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				--AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    			OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				--AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				--AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
				--AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    		) AS td
			GROUP BY ID_DonViQuiDoi, ID_LoHang
		) AS HangHoa
		on lh.ID = HangHoa.ID_LoHang
		inner join DM_HangHoa hh on lh.ID_HangHoa = hh.ID
		inner join DonViQuiDoi dvqd on hh.ID = dvqd.ID_HangHoa
		order by ID_HangHoa, MaLoHang");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_DMLoHang_TonKho]");
            DropStoredProcedure("[dbo].[getList_DMLoHangByIDDonViQD]");
            DropStoredProcedure("[dbo].[getList_DMLoHangTonKho_byID]");
            DropStoredProcedure("[dbo].[SP_GetAll_DMLoHang_TonKho]");
        }
    }
}
