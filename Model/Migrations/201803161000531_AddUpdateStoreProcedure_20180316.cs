namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateStoreProcedure_20180316 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[XuatFileDanhMucHH]", parametersAction: p => new
            {
                MaHH = p.String(),
                ListID_NhomHang = p.String(),
                ID_ChiNhanh = p.Guid(),
                KinhDoanhFilter = p.String(),
                LaHangHoaFilter = p.String(),
                List_ThuocTinh = p.String()
            }, body: @"DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
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
    	
    		Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.TonToiThieu, aa.TonToiDa, aa.LaHangHoa,aa.ThuocTinh, aa.LaChaCungLoai, aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable FROM (
    		(select dvqd.ID,hh.ID as ID_HangHoa,hh.TonToiThieu, hh.TonToiDa,hh.ID_HangHoaCungLoai,hh.LaHangHoa,hhtt.GiaTri+CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh ,hh.LaChaCungLoai, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    		from DonViQuiDoi dvqd
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    		left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    		where (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH) and dvqd.xoa is null and dvqd.ladonvichuan = 1
--    			(hh.LaChaCungLoai =(case when @List_ThuocTinh != '' then 0 else 1 end) or hh.LaChaCungLoai = (case when @List_ThuocTinh ='' then 1 else 1 end))
    		and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter) aa
    		left join
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
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
    		WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'
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
    		WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
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
--    		AND bhd.ID_DonVi = @ID_ChiNhanh
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
    		LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		)bb on aa.ID = bb.ID_DonViQuiDoi)
    		order by TonCuoiKy desc
    	
    	if(@List_ThuocTinh != '')
    		BEGIN
    		Select *	
    		from #dmhanghoatable
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    		END
    		ELSE
    		BEGIN
    		Select *
    		from #dmhanghoatable
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
    Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.LaHangHoa,aa.TonToiThieu, aa.TonToiDa, aa.ThuocTinh, aa.LaChaCungLoai, aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable2 FROM (
    (select dvqd.ID,hh.ID as ID_HangHoa,hh.TonToiThieu, hh.TonToiDa, hh.ID_HangHoaCungLoai,hh.LaHangHoa,hhtt.GiaTri+CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh ,hh.LaChaCungLoai, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DonViQUiDoi dvqd
    left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    left join HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    where (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH) and dvqd.xoa is null and dvqd.ladonvichuan = 1
    --	and (hh.LaChaCungLoai =(case when @List_ThuocTinh != '' then 0 else 1 end) or hh.LaChaCungLoai = (case when @List_ThuocTinh ='' then 1 else 1 end))
    and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter) aa
    left join
    (
    SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'
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
--    AND bhd.ID_DonVi = @ID_ChiNhanh
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
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
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
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    	order by TonCuoiKy desc
    	
    	if(@List_ThuocTinh != '')
    BEGIN
    Select *
    		from #dmhanghoatable2
    	where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    END
    ELSE
    BEGIN
    		Select *
    			 from #dmhanghoatable2
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
    Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.TonToiThieu, aa.TonToiDa, aa.LaHangHoa,aa.LaChaCungLoai,aa.DuocBanTrucTiep,aa.TrangThai,aa.ThuocTinh, aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable1 FROM (
    (select dvqd.ID,hh.ID as ID_HangHoa, hh.TonToiThieu, hh.TonToiDa, hh.ID_HangHoaCungLoai,hh.LaHangHoa,hh.LaChaCungLoai,hhtt.GiaTri+CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DonViQUiDoi dvqd
    left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	left join HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    where (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH) and dvqd.xoa is null and dvqd.ladonvichuan = 1 and nhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like nhh.ID
--    	and (hh.LaChaCungLoai =(case when @List_ThuocTinh != '' then 0 else 1 end) or hh.LaChaCungLoai = (case when @List_ThuocTinh ='' then 1 else 1 end))
    	and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter)) aa
    left join
    (
    SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'
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
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
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
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    	order by TonCuoiKy desc
    
    	if(@List_ThuocTinh != '')
    BEGIN
    Select
    		 *
    		from #dmhanghoatable1
    	    where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    END
    ELSE
    BEGIN
    Select *
    		from #dmhanghoatable1
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
    Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.TonToiThieu, aa.TonToiDa, aa.LaHangHoa,aa.LaChaCungLoai,aa.DuocBanTrucTiep,aa.TrangThai,aa.ThuocTinh, aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable3 FROM (
    (select dvqd.ID,hh.ID as ID_HangHoa,hh.TonToiThieu, hh.TonToiDa, hh.ID_HangHoaCungLoai,hh.LaHangHoa,hh.LaChaCungLoai,hhtt.GiaTri+CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DonViQUiDoi dvqd
    left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    left join HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    where (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH) and dvqd.xoa is null and dvqd.ladonvichuan = 1 and nhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like nhh.ID
    --    and (hh.LaChaCungLoai =(case when @List_ThuocTinh != '' then 0 else 1 end) or hh.LaChaCungLoai = (case when @List_ThuocTinh ='' then 1 else 1 end))	
    	and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter)) aa
    left join
    (
    SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'
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
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
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
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    	order by TonCuoiKy desc
    
    	if(@List_ThuocTinh != '')
    BEGIN
    Select *
    		from #dmhanghoatable3	
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    END
    ELSE
    BEGIN
    Select *
    		from #dmhanghoatable3	
    END
    	END
    --end @MaHH != '%%'
    	END
    	--END @ListID_NhomHang != '%%'");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[XuatFileDanhMucHH]");
        }
    }
}

