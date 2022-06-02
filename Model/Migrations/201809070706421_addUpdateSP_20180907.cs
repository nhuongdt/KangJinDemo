namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20180907 : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[getList_DMLoHangByIDDonViQD]", parametersAction: p => new
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
    		where dvqd.ID = @ID_DonViQuiDoi and lh.TrangThai = 1
    		order by lh.NgayTao DESC");

            AlterStoredProcedure(name: "[dbo].[getList_DMLoHangTonKho_byID]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                ID_LoHang = p.Guid(),
                timeChotSo = p.DateTime()
            }, body: @"SELECT 
	--HangHoa.ID_LoHang,
	CAST(ROUND((ISNULL(HangHoa.TonCuoiKy,0) / dvqd.TyLeChuyenDoi), 3) as float) as TonKho
    	FROM
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
    		inner join DonViQuiDoi dvqd on HangHoa.ID_DonViQuiDoi = dvqd.ID	
    		where dvqd.ID = @ID_DonViQuiDoi
    		AND HangHoa.ID_LoHang = @ID_LoHang");

            AlterStoredProcedure(name: "[dbo].[insert_HangHoa]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_NhomHang = p.Guid(),
                TenHangHoa = p.String(),
                TenHangHoa_KhongDau = p.String(),
                TenHangHoa_KyTuDau = p.String(),
                LaHangHoa = p.Boolean(),
                timeCreate = p.DateTime(),
                QuanLyTheoLoHang = p.Boolean()
            }, body: @"insert into DM_HangHoa (ID, TenHangHoa, LaHangHoa, ID_NhomHang, ID_HangHoaCungLoai, QuyCach, ChiPhiThucHien, ChiPhiTinhTheoPT, TheoDoi, NguoiTao, NgayTao, DuocBanTrucTiep, TenHangHoa_KhongDau, TenHangHoa_KyTuDau, LaChaCungLoai, QuanLyTheoLoHang)
    Values(@ID, @TenHangHoa, @LaHangHoa, @ID_NhomHang, NEWID(), '0', '0','1','1', 'admin', @timeCreate, '1', @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau, '1', @QuanLyTheoLoHang)");

            AlterStoredProcedure(name: "[dbo].[LoadHangHoaCungLoai]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_CungLoai = p.Guid()
            }, body: @"DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.LaHangHoa,aa.TonToiDa,aa.TonToiThieu, aa.GhiChu, aa.QuanLyTheoLoHang, aa.LaChaCungLoai, aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho FROM (
    (select dvqd.ID,hh.ID as ID_HangHoa,hh.ID_HangHoaCungLoai,hh.TonToiDa, hh.TonToiThieu,hh.GhiChu, hh.QuanLyTheoLoHang, hh.LaHangHoa,hh.LaChaCungLoai, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, CAST(ISNULL(gv.GiaVon, 0) as float) as GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DM_HangHoa hh
    left join DonViQuiDoi dvqd on hh.ID = dvqd.ID_hangHoa
    left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
	LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
    where dvqd.xoa is null and dvqd.ladonvichuan = '1' and hh.ID_HangHoaCungLoai = @ID_CungLoai) aa
    left join
    (
    SELECT  dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false' and LaHangHoa = 1
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
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and LaHangHoa = 1
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
    	)bb on aa.ID = bb.ID_DonViQuiDoi)");

            CreateStoredProcedure(name: "[dbo].[getList_DMHangHoa_Import]", parametersAction: p => new
            {
                MaHH = p.String(),
                ID_ChiNhanh = p.Guid(),
                ID_LoHang = p.Guid()
            }, body: @"-- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHangHoa,
			gv.ID as ID_GiaVon,
			gv.ID_LoHang as ID_LoHang,
    	Case when gv.ID is null then 0 else CAST(ROUND((gv.GiaVon), 0) as float) end  as GiaVon, 
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,  
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM 
    			DonViQuiDoi dvqd 
    			left join
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		--LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1
    		GROUP BY dhh.ID, dhh.LaHangHoa
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
		LEFT Join DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and (gv.ID_LoHang = @ID_LoHang or @ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh
    where
    	dvqd3.MaHangHoa = @MaHH");

            CreateStoredProcedure(name: "[dbo].[getlist_HoaDonBanHang_FindMaHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                maHD = p.String()
            }, body: @"SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.ID_ViTri,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
    	c.TheoDoi,
    	c.ID_DonVi,
    	c.ID_KhuyenMai,
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.MaHoaDonGoc,
    	c.TongTienHDTra,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
    	c.NguoiTaoHD,
    	c.DiaChiKhachHang,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TenPhongBan,
    	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan, c.ThuTuThe, c.TienMat, c.ChuyenKhoan, c.KhachDaTra,c.TongChietKhau,
    	c.TrangThai,
    	c.KhuyenMai_GhiChu,
    	c.KhuyeMai_GiamGia,
    	c.LoaiHoaDonGoc,
    	c.LoaiHoaDon,
    	c.DiaChiChiNhanh,
    	c.DienThoaiChiNhanh,
    	c.DiemGiaoDich,
    	c.DiemSauGD, -- add 02.08.2018 (bind InHoaDon)
		c.HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	FROM
    	(
    		select 
    		a.ID as ID,
			hdXMLOut.HoaDon_HangHoa,
    		bhhd.ID_DoiTuong,
    			-- Neu theo doi = null --> kiem tra neu la khach le --> theodoi = true, nguoc lai = 1
    			CASE 
    				WHEN dt.TheoDoi IS NULL THEN 
    					CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    				ELSE dt.TheoDoi
    			END AS TheoDoi,
    		bhhd.ID_HoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DonVi,
    		bhhd.ChoThanhToan,
    		bhhd.ID_KhuyenMai,
    		bhhd.KhuyenMai_GhiChu,
    			bhhd.LoaiHoaDon,
    		ISNULL(bhhd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(bhhd.DiemGiaoDich,0) AS DiemGiaoDich,
    		Case when gb.ID is not null then gb.ID else N'00000000-0000-0000-0000-000000000000' end as ID_BangGia,
    		Case when vt.ID is not null then vt.ID else N'00000000-0000-0000-0000-000000000000' end as ID_ViTri,
    		--Case when vt.ID is not null then vt.ID else N'00000000-0000-0000-0000-000000000000' end as TenViTri,
    			ISNULL(vt.TenViTri,'') as TenPhongBan,
    		bhhd.MaHoaDon,
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
    		bhhd.NgayLapHoaDon,
    		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end as MaDoiTuong,
    		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
    		Case when dt.TenDoiTuong is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
    		Case when dt.TenDoiTuong is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
    		Case when dt.Email is null then N'' else dt.Email end as Email,
    		Case when dt.DienThoai is null then N'' else dt.DienThoai end as DienThoai,
    		Case when dt.DiaChi is null then N'' else dt.DiaChi end as DiaChiKhachHang,
    			ISNULL(dt.TongTichDiem,0) AS DiemSauGD, --- nhuongdt add 02.08.2018
    		Case when tt.TenTinhThanh is null then tt.TenTinhThanh else N'' end as KhuVuc,
    		Case when qh.TenQuanHuyen is null then qh.TenQuanHuyen else N'' end as PhuongXa,
    		Case when dv.TenDonVi is null then N'' else dv.TenDonVi end as TenDonVi,
    		Case when dv.DiaChi is null then N'' else dv.DiaChi end as DiaChiChiNhanh,
    		Case when dv.SoDienThoai is null then N'' else dv.SoDienThoai end as DienThoaiChiNhanh,
    		Case when nv.TenNhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		bhhd.TongChietKhau,
    		Case when gb.TenGiaBan is null then N'Bảng giá chung' else gb.TenGiaBan end as TenBangGia,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
    		CAST(ROUND(ISNULL(hdt.PhaiThanhToan,0),0) as float) as TongTienHDTra,
    		a.ThuTuThe,
    		a.TienMat,
    		a.ChuyenKhoan,
    		a.KhachDaTra as KhachDaTra,
    		ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
    		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			Select 
    			b.ID,
    			SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
    			SUM(ISNULL(b.TienGui, 0)) as ChuyenKhoan,
    			SUM(ISNULL(b.TienThu, 0)) as KhachDaTra
    			from
    			(
    				Select 
    				bhhd.ID,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienGui, 0) else ISNULL(hdct.TienGui, 0) * (-1) end end as TienGui,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID  
					--and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    				where bhhd.LoaiHoaDon = '1' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
    
    				Union all
    				Select
    					d.ID,
    					Case when RowNumber = 1 then d.TienMat else 0 end as TienMat,
    					Case when RowNumber = 1 then d.TienGui else 0 end as TienGui,
    					Case when RowNumber = 1 then d.ThuTuThe else 0 end as ThuTuThe,
    					Case when RowNumber = 1 then d.TienThu else 0 end as TienThu
    				FROM
    				(
    					SELECT ROW_NUMBER() Over(PARTITION BY ID_DatHang ORDER BY f.NgayLapHoaDon)
    					As RowNumber,* FROM 
    					(
    						Select
    						bhhd.ID,
    						bhhd.NgayLapHoaDon,
    						hdt.ID as ID_DatHang,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienGui, 0) else ISNULL(hdct.TienGui, 0) * (-1) end end as TienGui,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu
    						from BH_HoaDon bhhd
    						inner join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
    						left join Quy_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDonLienQuan
    						left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID 
							--and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    						where hdt.LoaiHoaDon = '3' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
    					) f
    				) d
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
			left join 
				(Select distinct hdXML.ID, 
					 (
						select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
						from BH_HoaDon_ChiTiet ct
						join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
						join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
						where ct.ID_HoaDon = hdXML.ID
						For XML PATH ('')
					) HoaDon_HangHoa
				from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
    	WHERE MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD or DienThoai like @maHD or MaDoiTuong like @maHD
		OR HoaDon_HangHoa like @maHD
    	ORDER BY c.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getlist_HoaDonDatHang_FindMaHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                maHD = p.String()
            }, body: @"SELECT 
    	c.ID,
    	c.MaHoaDon,
    	c.LoaiHoaDon,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
    	c.ID_BangGia,
    	c.ID_DonVi,
    	c.YeuCau,
    	c.NguoiTaoHD,
    	c.DiaChiKhachHang,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan, c.KhachDaTra,c.TongChietKhau,
    	c.TrangThai,
    	c.TheoDoi,
    	c.TenPhongBan,
		c.HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon

    	FROM
    	(
    		select 
    		a.ID as ID,
    		bhhd.MaHoaDon,
			hdXMLOut.HoaDon_HangHoa,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DoiTuong,
    		bhhd.ID_BangGia,
    		bhhd.NgayLapHoaDon,
    		bhhd.YeuCau,
    		bhhd.ID_DonVi,
    			CASE 
    				WHEN dt.TheoDoi IS NULL THEN 
    					CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    				ELSE dt.TheoDoi
    			END AS TheoDoi,
    		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
    		Case when dt.TenDoiTuong is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
    		Case when dt.TenDoiTuong is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
    		Case when dt.Email is null then N'' else dt.Email end as Email,
    		Case when dt.DienThoai is null then N'' else dt.DienThoai end as DienThoai,
    		Case when dt.DiaChi is null then N'' else dt.DiaChi end as DiaChiKhachHang,
    		Case when tt.TenTinhThanh is null then tt.TenTinhThanh else N'' end as KhuVuc,
    		Case when qh.TenQuanHuyen is null then qh.TenQuanHuyen else N'' end as PhuongXa,
    		Case when dv.TenDonVi is null then N'' else dv.TenDonVi end as TenDonVi,
    		Case when nv.TenNhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    			ISNULL(vt.TenViTri,'') as TenPhongBan,
    		Case when gb.TenGiaBan is null then N'Bảng giá chung' else gb.TenGiaBan end as TenBangGia,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
    		a.KhachDaTra as KhachDaTra,
    		bhhd.TongChietKhau,
    		Case When bhhd.YeuCau = '1' then N'Phiếu tạm' when bhhd.YeuCau = '3' then N'Hoàn thành' when bhhd.YeuCau = '2' then N'Đang giao hàng' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			select 
    			b.ID,
    			SUM(ISNULL(b.KhachDaTra, 0)) as KhachDaTra
    			from
    			(
    				Select 
    				bhhd.ID,
    					Case when bhhd.ChoThanhToan is null then 0 else ISNULL(hdct.Tienthu, 0) end as KhachDaTra
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
    					left join Quy_HoaDon qhd on (hdct.ID_HoaDon = qhd.ID and qhd.trangthai is null)	
    				where bhhd.LoaiHoaDon = '3' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    
    
    				union all
    				Select
    				hdt.ID,
    					Case when bhhd.ChoThanhToan is null then (Case when qhd.LoaiHoaDon = '11' then 0 else ISNULL(hdct.Tienthu, 0) * (-1) end)
    					else (Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end) end as KhachDaTra
    				from BH_HoaDon bhhd
    				inner join BH_HoaDon hdt on (bhhd.ID_HoaDon = hdt.ID and hdt.ChoThanhToan = '0')
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
    				left join Quy_HoaDon qhd on (hdct.ID_HoaDon = qhd.ID)
    				where hdt.LoaiHoaDon = '3' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
			left join 
				(Select distinct hdXML.ID, 
					 (
						select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
						from BH_HoaDon_ChiTiet ct
						join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
						join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
						where ct.ID_HoaDon = hdXML.ID
						For XML PATH ('')
					) HoaDon_HangHoa
				from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
    	WHERE MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD or DienThoai like @maHD
		OR HoaDon_HangHoa like @maHD
    	ORDER BY c.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getlist_HoaDonTraHang_FindMaHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                MaPT = p.String(),
                MaHD = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.LoaiHoaDon,
    	c.ID_ViTri,
    	c.ID_DonVi,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
    	c.TongTienHDDoiTra,
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.MaHoaDonGoc, 
    	c.MaPhieuChi,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
    	c.NguoiTaoHD,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan,c.TongChiPhi, c.KhachDaTra,c.TongChietKhau,
    	c.TrangThai,
    	c.TheoDoi,
    	c.TenPhongBan,
    	c.DienThoaiChiNhanh,
    	c.DiaChiChiNhanh,
    	c.DiemGiaoDich,
		c.HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	FROM
    	(
    		select 
			hdXMLOut.HoaDon_HangHoa,
    		a.ID as ID,
    		bhhd.MaHoaDon,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_BangGia,
    		bhhd.ID_HoaDon,
    		bhhd.ID_ViTri,
    		bhhd.ID_DonVi,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DoiTuong,
    		ISNULL(hddt.PhaiThanhToan,0) as TongTienHDDoiTra,
    		ISNULL(bhhd.DiemGiaoDich,0) as DiemGiaoDich,
    		bhhd.ChoThanhToan,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		Case when hdb.MaHoaDon is null then '' else hdb.MaHoaDon end as MaHoaDonGoc,
    		a.MaPhieuChi,
    		bhhd.NgayLapHoaDon,
    			CASE 
    				WHEN dt.TheoDoi IS NULL THEN 
    					CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    				ELSE dt.TheoDoi
    			END AS TheoDoi,
    		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
    		Case when dt.DienThoai is null then N'' else dt.DienThoai end as DienThoai,
    			Case when dt.DiaChi is null then N'' else dt.DiaChi end as DiaChiKhachHang,
    		Case when dv.TenDonVi is null then N'' else dv.TenDonVi end as TenDonVi,
    			Case when dv.DiaChi is null then N'' else dv.DiaChi end as DiaChiChiNhanh,
    		Case when dv.SoDienThoai is null then N'' else dv.SoDienThoai end as DienThoaiChiNhanh,
    		Case when nv.TenNhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		Case when gb.TenGiaBan is null then N'Bảng giá chung' else gb.TenGiaBan end as TenBangGia,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.TongChiPhi, 0) as float) as TongChiPhi,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
    		a.KhachDaTra as KhachDaTra,
    		bhhd.TongChietKhau,
    
    		Case When bhhd.YeuCau = '4' then N'Đã hủy' else N'Hoàn thành' end as TrangThai
    		FROM
    		(
    			Select 
    			bhhd.ID,
    			qhd.MaHoaDon as MaPhieuChi,
    			ISNULL(hdct.Tienthu, 0) as KhachDaTra
    			from BH_HoaDon bhhd
    			left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    			left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID
    			where bhhd.LoaiHoaDon = '6' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdb on bhhd.ID_HoaDon = hdb.ID
    		left join BH_HoaDon hddt on bhhd.ID = hddt.ID_HoaDon			
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
			left join 
				(Select distinct hdXML.ID, 
					 (
						select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
						from BH_HoaDon_ChiTiet ct
						join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
						join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
						where ct.ID_HoaDon = hdXML.ID
						For XML PATH ('')
					) HoaDon_HangHoa
				from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
    	WHERE (MaHoaDon like @MaPT OR MaHoaDonGoc like @MaPT OR HoaDon_HangHoa like @MaPT) -- find MaHangHoa/TenHangHoa/MaHDTra/MaHDGoc
    	--and MaHoaDonGoc like @MaHD
    	and TrangThai like @TrangThai
    	ORDER BY c.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getListHangHoaLoHang_ByMaHangHoa]", parametersAction: p => new
            {
                MaHH = p.String(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT Top(20) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
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
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM 
    			DonViQuiDoi dvqd 
    			left join
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		--LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1 and dhh.DuocBanTrucTiep = 1
				and (lh.TrangThai = 1 or lh.TrangThai is null)
				--and dvqd.MaHangHoa = @MaHH
    		GROUP BY dhh.ID, dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
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
	LEFT Join DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and (gv.ID_LoHang = a.ID_LoHang or a.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh
	Where dvqd3.MaHangHoa = @MaHH
	and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
    order by a.NgayHetHan");

            CreateStoredProcedure(name: "[dbo].[getListHangHoaLoHang_ChotSo_ByMaHangHoa]", parametersAction: p => new
            {
                MaHH = p.String(),
                ID_ChiNhanh = p.Guid()
            }, body: @"-- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT Top(20) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
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
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
			MAX(dhh.TenHangHoa) As TenHangHoa,
			dhh.QuanLyTheoLoHang,
			MAX(lh.ID)  As ID_LoHang,
			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
			MAX(lh.NgaySanXuat)  As NgaySanXuat,
			MAX(lh.NgayHetHan)  As NgayHetHan,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1 and dhh.DuocBanTrucTiep = 1
				and (lh.TrangThai = 1 or lh.TrangThai is null)
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang
    ) a
    LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
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
	LEFT Join DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and (gv.ID_LoHang = a.ID_LoHang or a.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh
    Where dvqd3.MaHangHoa = @MaHH
	and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
	order by a.NgayHetHan");

            CreateStoredProcedure(name: "[dbo].[getListHangHoaLoHang_ChotSo_EnTer]", parametersAction: p => new
            {
                MaHH = p.String(),
                ID_ChiNhanh = p.Guid()
            }, body: @"-- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar

		SELECT dvqd4.ID as ID_DonViQuiDoi,dvqd4.ID_HangHoa as ID, dvqd4.MaHangHoa, hh4.TenHangHoa, Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri, dvqd4.TenDonViTinh, hh4.QuanLyTheoLoHang,ISNULL(TonKhoTable.GiaVon,0) as GiaVon,
	dvqd4.GiaBan, dvqd4.GiaNhap, TonKho, ID_LoHang,MaLoHang, NgaySanXuat, NgayHetHan FROM
	DonViQuiDoi dvqd4
	left join DM_HangHoa hh4 on dvqd4.ID_HangHoa = hh4.ID
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
    					) as ThuocTinh on dvqd4.ID_HangHoa = ThuocTinh.id_hanghoa
	 left join(

	SELECT tableTon.ID_DonViQuiDoi, MaHangHoa,TenHangHoa, TenDonViTinh, QuanLyTheoLoHang, CAST(ISNULL(gv.GiaVon,0) as float) as GiaVon,
	GiaBan, GiaNhap, TonKho, tableTon.ID_LoHang, MaLoHang, NgaySanXuat, NgayHetHan FROM
	(
    	SELECT Top(20) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
			a.TenHangHoa,
			dvqd3.TenDonViTinh,
			a.QuanLyTheoLoHang,
    	CAST(ROUND((dvqd3.GiaVon), 0) as float) as GiaVon, 
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,
		CAST(ROUND((dvqd3.GiaNhap), 0) as float) as GiaNhap,  
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonKho,
    			ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
				a.ID_LoHang,
				a.MaLoHang ,
				a.NgaySanXuat,
				a.NgayHetHan
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
			MAX(dhh.TenHangHoa) As TenHangHoa,
			dhh.QuanLyTheoLoHang,
			MAX(Case when lh.ID is null or dhh.QuanLyTheoLoHang = '0' then null else lh.ID end)  As ID_LoHang,
			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then null else lh.MaLoHang end) As MaLoHang,
			MAX(lh.NgaySanXuat)  As NgaySanXuat,
			MAX(lh.NgayHetHan)  As NgayHetHan,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID and lh.TrangThai = 1
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang
    ) a
    left Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    Where dvqd3.MaHangHoa = @MaHH
	and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
	order by a.NgayHetHan
	) as tableTon 
	left join DM_GiaVon gv on (tableTon.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tableTon.ID_LoHang = gv.ID_LoHang or tableTon.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
	) as TonKhoTable on TonKhoTable.ID_DonViQuiDoi = dvqd4.ID
	where dvqd4.Xoa is null and dvqd4.MaHangHoa = @MaHH");

            CreateStoredProcedure(name: "[dbo].[getListHangHoaLoHang_EnTer]", parametersAction: p => new
            {
                MaHH = p.String(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT dvqd4.ID as ID_DonViQuiDoi,dvqd4.MaHangHoa,dvqd4.ID_HangHoa as ID, hh4.TenHangHoa, Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri, dvqd4.TenDonViTinh, hh4.QuanLyTheoLoHang,ISNULL(TonKhoTable.GiaVon,0) as GiaVon,
	 dvqd4.GiaBan, dvqd4.GiaNhap, TonKho, ID_LoHang,MaLoHang, NgaySanXuat, NgayHetHan FROM
	DonViQuiDoi dvqd4
	left join DM_HangHoa hh4 on dvqd4.ID_HangHoa = hh4.ID
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
    					) as ThuocTinh on dvqd4.ID_HangHoa = ThuocTinh.id_hanghoa
	 left join(
	
	SELECT tableTon.ID_DonViQuiDoi, MaHangHoa,TenHangHoa, TenDonViTinh, QuanLyTheoLoHang, CAST(ISNULL(gv.GiaVon,0) as float) as GiaVon,
	GiaBan, GiaNhap, TonKho, tableTon.ID_LoHang, MaLoHang, NgaySanXuat, NgayHetHan FROM
	(
    	SELECT Top(20) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHangHoa,
			dvqd3.TenDonViTinh,
			a.QuanLyTheoLoHang,
    		CAST(ROUND((dvqd3.GiaVon), 0) as float) as GiaVon, 
    		CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,
			CAST(ROUND((dvqd3.GiaNhap), 0) as float) as GiaNhap,   
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonKho,
    			ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
				a.ID_LoHang,
				a.MaLoHang,
				a.NgaySanXuat,
				a.NgayHetHan
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
				MAX(dhh.TenHangHoa) As TenHangHoa,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
			dhh.QuanLyTheoLoHang,
			MAX(Case when lh.ID is null or dhh.QuanLyTheoLoHang = '0' then null else lh.ID end)  As ID_LoHang,
			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then null else lh.MaLoHang end) As MaLoHang,
			MAX(lh.NgaySanXuat)  As NgaySanXuat,
			MAX(lh.NgayHetHan)  As NgayHetHan,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM 
    			DonViQuiDoi dvqd 
    			left join
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID and lh.TrangThai = 1
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1
    		GROUP BY dhh.ID, dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
	Where dvqd3.MaHangHoa = @MaHH
	and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
    order by a.NgayHetHan
	) as tableTon
	left join DM_GiaVon gv on (tableTon.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tableTon.ID_LoHang = gv.ID_LoHang or tableTon.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
	) as TonKhoTable on TonKhoTable.ID_DonViQuiDoi = dvqd4.ID
	where dvqd4.Xoa is null and dvqd4.MaHangHoa = @MaHH");

            CreateStoredProcedure(name: "[dbo].[getListXuatKho_Import]", parametersAction: p => new
            {
                MaHangHoa = p.String(),
                MaLohang = p.String(),
                SoLuong = p.Double(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT 
			dvqd3.ID as ID_DonViQuiDoi, 
			Case When a.ID_LoHang is null then NEWID() else a.ID_LoHang end as ID_LoHang,
			dvqd3.MaHangHoa,
    		a.TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
			a.QuanLyTheoLoHang,
			Case when gv.ID is null then 0 else CAST(ROUND((gv.GiaVon), 0) as float) end  as GiaVon, 
    		CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,  
			CAST(ROUND((@SoLuong), 0) as float) as SoLuong,
			  CAST(ROUND((@SoLuong), 0) as float) as SoLuongXuatHuy,
			Case when gv.ID is null then 0 else CAST(ROUND((@SoLuong * gv.GiaVon), 0) as float) end  as GiaTriHuy,  
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonKho,
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
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM 
    			DonViQuiDoi dvqd 
    			left join
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
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
					--and dvqd.MaHangHoa = @MaHH
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		--LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1
				and lh.TrangThai = 1 or lh.TrangThai is null
				--and dvqd.MaHangHoa = @MaHH
    		GROUP BY dhh.ID, dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
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
	LEFT Join DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and (gv.ID_LoHang = a.ID_LoHang or a.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh
	Where dvqd3.MaHangHoa = @MaHangHoa
	and a.MaLoHang = @MaLoHang
    order by a.NgayHetHan");

            CreateStoredProcedure(name: "[dbo].[getListXuatKho_Import_ChotSo]", parametersAction: p => new
            {
                MaHangHoa = p.String(),
                MaLoHang = p.String(),
                SoLuong = p.Double(),
                ID_ChiNhanh = p.Guid()
            }, body: @"-- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT dvqd3.ID as ID_DonViQuiDoi,
		Case When a.ID_LoHang is null then NEWID() else a.ID_LoHang end as ID_LoHang,
		 dvqd3.MaHangHoa,
    		a.TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
			a.QuanLyTheoLoHang,
    		Case when gv.ID is null then 0 else CAST(ROUND((gv.GiaVon), 0) as float) end  as GiaVon,  
    		CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
			 CAST(ROUND((@SoLuong), 0) as float) as SoLuong,
			  CAST(ROUND((@SoLuong), 0) as float) as SoLuongXuatHuy,
			Case when gv.ID is null then 0 else CAST(ROUND((@SoLuong * gv.GiaVon), 0) as float) end  as GiaTriHuy,  
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonKho,
				a.MaLoHang as TenLoHang,
				a.NgaySanXuat,
				a.NgayHetHan
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
			MAX(dhh.TenHangHoa) As TenHangHoa,
			dhh.QuanLyTheoLoHang,
			MAX(lh.ID)  As ID_LoHang,
			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
			MAX(lh.NgaySanXuat)  As NgaySanXuat,
			MAX(lh.NgayHetHan)  As NgayHetHan,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1
				and lh.TrangThai = 1 or lh.TrangThai is null
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang
    ) a
    LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
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
	LEFT Join DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and (gv.ID_LoHang = a.ID_LoHang or a.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh
    Where dvqd3.MaHangHoa = @MaHangHoa
	and a.MaLoHang = @MaLoHang
	order by a.NgayHetHan");

            CreateStoredProcedure(name: "[dbo].[insert_DM_GiaVon]", parametersAction: p => new
            {
                ID_DonViQuiDoi = p.Guid(),
                ID_DonVi = p.Guid(),
                ID_LoHang = p.Guid(),
                GiaVon = p.Double()
            }, body: @"insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
	Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVon)");

            CreateStoredProcedure(name: "[dbo].[insert_HoaDonLoHang_ChiTiet]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                ID_LoHang = p.Guid(),
                SoLuong = p.Double(),
                DonGia = p.Double(),
                ThanhTien = p.Double(),
                GiaVon = p.Double()
            }, body: @"DECLARE @SoThuTu  as int
    SET @SoThuTu =  (Select Max(SoThuTu) FROM BH_HoaDon_ChiTiet where ID_HoaDon = @ID_HoaDon);
    		if (@SoThuTu is null)
    		Begin
    			SET @SoThuTu = 1;
    		End
    		else
    		Begin
    			SET @SoThuTu = @SoThuTu + 1;
    		End
    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    Values (NEWID(), @ID_HoaDon, @SoThuTu, @SoLuong, @DonGia, @ThanhTien, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi, @ID_LoHang)");

            CreateStoredProcedure(name: "[dbo].[insert_LoHang]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_HangHoa = p.Guid(),
                MaLohang = p.String(),
                NgaySanXuat = p.DateTime(),
                NgayHetHan = p.DateTime()
            }, body: @"insert into DM_LoHang (ID, ID_HangHoa, MaLoHang, TenLoHang, NgaySanXuat, NgayHetHan, NguoiTao, NgayTao, TrangThai)
	Values (@ID, @ID_HangHoa, @MaLoHang, @MaLoHang, @NgaySanXuat, @NgayHetHan, 'admin', GETDATE(), '1')");

            CreateStoredProcedure(name: "[dbo].[LoadDanhMucLoHangBaoCao]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHHCoDau = p.String(),
                ListID_NhomHang = p.String(),
                ID_ChiNhanh = p.Guid(),
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
    		Select bb.ID_DonViQuiDoi,bb.ID_HangHoa as ID, bb.ID_LoHang, bb.MaLoHang, bb.NgaySanXuat, bb.NgayHetHan, bb.TonToiThieu, bb.TonToiDa, bb.ThuocTinh,bb.NgayTao, bb.MaHangHoa, bb.ID_NhomHangHoa,
    			  bb.NhomHangHoa, bb.TenHangHoa, bb.TenDonViTinh, bb.GiaVon, bb.GiaBan, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable FROM (
    		(SELECT 
    				dmlo1.ID As ID_LoHang,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DM_LoHang dmlo1
					left join DonViQuiDoi dvqd on dmlo1.ID_HangHoa = dvqd.ID_HangHoa
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on dmlo1.ID_HangHoa = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dmlo1.ID
    			) aa
    		right join
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi, dmlo.ID as ID_LoHang, dvqd3.ID_HangHoa as ID_HangHoa,dmlo.MaLoHang,dmlo.NgaySanXuat, dmlo.NgayHetHan ,dhh3.TonToiThieu,dhh3.TonToiDa,hhtt3.GiaTri + CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh,
    			dvqd3.MaHangHoa, dhh3.NgayTao, dhh3.ID_NhomHang as ID_NhomHangHoa,
    			dnhh3.TenNhomHangHoa as NhomHangHoa, dhh3.TenHangHoa, dvqd3.TenDonViTinh, CAST(ISNULL(gv.GiaVon, 0) as FLOAT) as GiaVon, dvqd3.GiaBan,((a.TonDau / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    		( 
    		SELECT
    		td.ID_LoHang,
    		SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    		NULL AS SoLuongNhap,
    		NULL AS SoLuongXuat
    		FROM
    		(
    		SELECT
    		bhdct.ID_LoHang,
    		NULL AS SoLuongNhap,
    			
    		SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    		FROM DM_LoHang dmlo
			left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false' and hh.LaHangHoa = 1
    		AND bhd.NgayLapHoaDon >= @timeStart
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		GROUP BY bhdct.ID_LoHang                                                                                                                                                                                                                                                         
    	    
    		UNION ALL
    		SELECT
    		bhdct.ID_LoHang,
    		NULL AS SoLuongNhap,
    		SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    		FROM DM_LoHang dmlo
			left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    		OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		AND bhd.NgayLapHoaDon >= @timeStart
    		GROUP BY bhdct.ID_LoHang
    	    
    		UNION ALL
    		SELECT
    		bhdct.ID_LoHang,
    		SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    		null AS SoLuongXuat
    		FROM DM_LoHang dmlo
			left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi 
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		AND bhd.NgayLapHoaDon >= @timeStart
    		GROUP BY bhdct.ID_LoHang
    	    
    		UNION ALL
    		SELECT
    		bhdct.ID_LoHang,
    		SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    		null AS SoLuongXuat
    		FROM DM_LoHang dmlo
			left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		AND bhd.NgayLapHoaDon >= @timeStart
    		GROUP BY bhdct.ID_LoHang 
    		) AS td 
    		GROUP BY td.ID_LoHang
    		) a
    		right Join DM_LoHang dmlo on a.ID_LoHang = dmlo.ID
			left join DonViQuiDoi dvqd3 on dvqd3.ID_HangHoa = dmlo.ID_HangHoa
    		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
    		left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
			LEFT JOIN DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = dmlo.ID
    		where dvqd3.xoa is null and dvqd3.ladonvichuan = 1
    		and dhh3.TheoDoi =1 and dhh3.LaHangHoa = 1
    		)bb on aa.ID_LoHang = bb.ID_LoHang)
    	
    	if(@List_ThuocTinh != '')
    		BEGIN
    		Select *
    		from #dmhanghoatable hhtb2
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
    		END
    		ELSE
    		BEGIN
    		Select *
    		from #dmhanghoatable hhtb2
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
    Select bb.ID_DonViQuiDoi,bb.ID_HangHoa as ID,bb.ID_LoHang, bb.MaLoHang, bb.NgaySanXuat, bb.NgayHetHan, bb.TonToiThieu, bb.TonToiDa, bb.ThuocTinh,bb.NgayTao, bb.MaHangHoa, bb.ID_NhomHangHoa,
    			  bb.NhomHangHoa, bb.TenHangHoa, bb.TenDonViTinh, bb.GiaVon, bb.GiaBan, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable2 FROM (
    		(SELECT 
    				dmlo1.ID As ID_LoHang,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DM_LoHang dmlo1 
					left join DonViQuiDoi dvqd on dmlo1.ID_HangHoa = dvqd.ID_HangHoa
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dmlo1.ID
    			) aa
    		right join
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi,dvqd3.ID_HangHoa as ID_HangHoa,dmlo.ID as ID_LoHang, dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan, dhh3.TonToiThieu,dhh3.TonToiDa,hhtt3.GiaTri + CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh,
    			dvqd3.MaHangHoa, dhh3.NgayTao, dhh3.ID_NhomHang as ID_NhomHangHoa,
    			dnhh3.TenNhomHangHoa as NhomHangHoa, dhh3.TenHangHoa, dvqd3.TenDonViTinh, CAST(ISNULL(gv.GiaVon, 0) as FLOAT) as GiaVon, dvqd3.GiaBan,((a.TonDau / dvqd3.TyLeChuyenDoi)) as TonCuoiKy  FROM 
    ( 
    SELECT
    td.ID_LoHang,
    SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    NULL AS SoLuongNhap,
    NULL AS SoLuongXuat
    FROM
    (
    SELECT
    bhdct.ID_LoHang,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM DM_LoHang dmlo
			left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false' and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_LoHang                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM DM_LoHang dmlo
			left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_LoHang
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi 
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_LoHang
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_LoHang 
    ) AS td 
    GROUP BY td.ID_LoHang
    ) a
    	right Join DM_LoHang dmlo on a.ID_LoHang = dmlo.ID
		left join DonViQuiDoi dvqd3 on dvqd3.ID_HangHoa = dmlo.ID_HangHoa
    	LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
    	left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		LEFT JOIN DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = dmlo.ID
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
    		and dhh3.TheoDoi =1 and dhh3.LaHangHoa = 1
    	)bb on aa.ID_LoHang = bb.ID_LoHang)
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
     Select bb.ID_DonViQuiDoi,bb.ID_HangHoa as ID,bb.ID_LoHang, bb.MaLoHang, bb.NgaySanXuat, bb.NgayHetHan, bb.TonToiThieu, bb.TonToiDa, bb.ThuocTinh,bb.NgayTao, bb.MaHangHoa, bb.ID_NhomHangHoa,
    			  bb.NhomHangHoa, bb.TenHangHoa, bb.TenDonViTinh, bb.GiaVon, bb.GiaBan, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable1 FROM (
    		(SELECT 
    				dmlo1.ID As ID_LoHang,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DM_LoHang dmlo1
					left join DonViQuiDoi dvqd on dmlo1.ID_HangHoa = dvqd.ID_HangHoa
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dmlo1.ID
    			) aa
    		right join
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi,dvqd3.ID_HangHoa as ID_HangHoa,dmlo.ID as ID_LoHang, dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan, dhh3.TonToiThieu,dhh3.TonToiDa,hhtt3.GiaTri + CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh,
    			dvqd3.MaHangHoa, dhh3.NgayTao, dhh3.ID_NhomHang as ID_NhomHangHoa,
    			dnhh3.TenNhomHangHoa as NhomHangHoa, dhh3.TenHangHoa, dvqd3.TenDonViTinh, CAST(ISNULL(gv.GiaVon,0) as FLOAT) as GiaVon, dvqd3.GiaBan,((a.TonDau / dvqd3.TyLeChuyenDoi)) as TonCuoiKy  FROM 
    ( 
    
    SELECT
    td.ID_LoHang,
    SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    NULL AS SoLuongNhap,
    NULL AS SoLuongXuat
    FROM
    (
    SELECT
    bhdct.ID_LoHang,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_LoHang                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_LoHang
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi 
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_LoHang
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_LoHang 
    ) AS td 
    GROUP BY td.ID_LoHang
    ) a
    	right Join DM_LoHang dmlo on a.ID_LoHang = dmlo.ID
		left join DonViQuiDoi dvqd3 on dvqd3.ID_HangHoa = dmlo.ID_HangHoa
		LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
		left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
		LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		LEFT JOIN DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = dmlo.ID
    	where dvqd3.xoa is null and dvqd3.ladonvichuan = 1 and dnhh3.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh3.ID)
    	and dhh3.TheoDoi =1 and dhh3.LaHangHoa = 1
    	)bb on aa.ID_LoHang = bb.ID_LoHang)
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
    Select bb.ID_DonViQuiDoi,bb.ID_HangHoa as ID,bb.ID_LoHang, bb.MaLoHang, bb.NgaySanXuat,bb.NgayHetHan, bb.TonToiThieu, bb.TonToiDa, bb.ThuocTinh,bb.NgayTao, bb.MaHangHoa, bb.ID_NhomHangHoa,
    			  bb.NhomHangHoa, bb.TenHangHoa, bb.TenDonViTinh, bb.GiaVon, bb.GiaBan, 
    		(ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho into #dmhanghoatable3 FROM (
    		(SELECT 
    				dmlo1.ID As ID_LoHang,
    				SUM(ISNULL((case when hh.LaHangHoa = 1 then cs.TonKho else 0 end)/ dvqd.tylechuyendoi , 0)) as TonKho
    				FROM DM_LoHang dmlo1
					left join DonViQuiDoi dvqd on dmlo1.ID_HangHoa = dvqd.ID_HangHoa
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				where dvqd.ladonvichuan = '1'
    				GROUP BY dmlo1.ID
    			) aa
    		right join
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi,dvqd3.ID_HangHoa as ID_HangHoa,dmlo.ID as ID_LoHang, dmlo.MaLoHang, dmlo.NgaySanXuat, dmlo.NgayHetHan, dhh3.TonToiThieu,dhh3.TonToiDa,hhtt3.GiaTri + CAST(hhtt3.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh,
    			dvqd3.MaHangHoa, dhh3.NgayTao, dhh3.ID_NhomHang as ID_NhomHangHoa,
    			dnhh3.TenNhomHangHoa as NhomHangHoa, dhh3.TenHangHoa, dvqd3.TenDonViTinh, CAST(ISNULL(gv.GiaVon, 0) as FLOAT) as GiaVon, dvqd3.GiaBan,((a.TonDau / dvqd3.TyLeChuyenDoi)) as TonCuoiKy  FROM 
    ( 
    SELECT
    td.ID_LoHang,
    SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    NULL AS SoLuongNhap,
    NULL AS SoLuongXuat
    FROM
    (
    SELECT
    bhdct.ID_LoHang,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_LoHang                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_LoHang
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi 
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_LoHang
    
    UNION ALL
    SELECT
    bhdct.ID_LoHang,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM DM_LoHang dmlo
	left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_LoHang 
    ) AS td 
    GROUP BY td.ID_LoHang
    ) a
    	right Join DM_LoHang dmlo on a.ID_LoHang = dmlo.ID
		left join DonViQuiDoi dvqd3 on dvqd3.ID_HangHoa = dmlo.ID_HangHoa
    	LEFT JOIN DM_HangHoa dhh3 ON dhh3.ID = dvqd3.ID_HangHoa
    	left join HangHoa_ThuocTinh hhtt3 on dvqd3.ID_HangHoa = hhtt3.ID_HangHoa
    	LEFT JOIN DM_NhomHangHoa dnhh3 ON dnhh3.ID = dhh3.ID_NhomHang
		LEFT JOIN DM_GiaVon gv on dvqd3.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang = dmlo.ID
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
    		and dhh3.TheoDoi =1 and dhh3.LaHangHoa = 1
    	)bb on aa.ID_LoHang = bb.ID_LoHang)
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

            CreateStoredProcedure(name: "[dbo].[Search_DMHangHoaLoHang_TonKho]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    	  Select @count =  (Select count(*) from @tablename);
    	    Select @countChar =   (Select count(*) from @tablenameChar);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
		Select Top(20)
		tr.ID_DonViQuiDoi,
		MAX(tr.MaHangHoa) as MaHangHoa,
		MAX(tr.TenHangHoa) as TenHangHoa,
		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
		MAX(tr.TenDonViTinh) as TenDonViTinh,
		tr.QuanLyTheoLoHang,
		--gv.GiaVon as GiaVon,
		Case When @XemGiaVon != '1' or gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
		MAX(tr.GiaBan) as GiaBan,
		Sum(tr.TonCuoiKy) as TonCuoiKy,
		MAX(tr.SrcImage) as SrcImage,
		Case when tr.ID_LoHang is null then NEWID() else tr.ID_LoHang end as ID_LoHang,
		MAX(tr.TenLoHang) as TenLoHang,
		MAX(tr.NgaySanXuat) as NgaySanXuat,
		MAX(tr.NgayHetHan) as NgayHetHan
		 FROM
		(
		SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHH as TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
			a.QuanLyTheoLoHang,
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,  
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,
    			an.URLAnh as SrcImage,
				a.ID_LoHang,
				a.MaLoHang as TenLoHang,
				a.NgaySanXuat,
				a.NgayHetHan,
				a.TrangThai
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
			  where dvqd1.xoa is null and dhh1.duocbantructiep = '1'  and dhh1.TheoDoi = '1'
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
			--MAX(Case when lh.ID is null or dhh.QuanLyTheoLoHang = '0' then NEWID() else lh.ID end)  As ID_LoHang,
			--MAX(lh.ID)  As ID_LoHang,
			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
			MAX(lh.NgaySanXuat)  As NgaySanXuat,
			MAX(lh.NgayHetHan)  As NgayHetHan,
			lh.TrangThai,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM 
    			DonViQuiDoi dvqd 
				--left join DM_LoHang lh on dvqd.ID_HangHoa = lh.ID_HangHoa
    			left join
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID-- or HangHoa.ID_LoHang is null or dhh.QuanLyTheoLoHang is null
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1 and dhh.DuocBanTrucTiep = '1'
    		GROUP BY dhh.ID, dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    	right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
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
    where
    	 ((select count(*) from @tablename b where 
    	dvqd3.MaHangHoa like '%'+b.Name+'%' 
    		or a.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or a.TenHangHoa_KyTuDau like '%'+b.Name+'%' )=@count or @count=0)
    	and ((select count(*) from @tablenameChar c where
    		dvqd3.MaHangHoa like '%'+c.Name+'%' 
    		or a.TenHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    	and 
    		dvqd3.Xoa is null
		and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
		) tr
		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where tr.TrangThai = 1 or tr.TrangThai is null
		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, tr.QuanLyTheoLoHang, gv.ID, gv.GiaVon
		order by MAX(tr.NgayHetHan)");

            CreateStoredProcedure(name: "[dbo].[Search_DMHangHoaLoHang_TonKho_ChotSo]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    	DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    	  Select @count =  (Select count(*) from @tablename);
    	    Select @countChar =   (Select count(*) from @tablenameChar);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
	Select Top(20)
		tr.ID_DonViQuiDoi,
		MAX(tr.MaHangHoa) as MaHangHoa,
		MAX(tr.TenHangHoa) as TenHangHoa,
		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
		MAX(tr.TenDonViTinh) as TenDonViTinh,
		tr.QuanLyTheoLoHang,
		--gv.GiaVon as GiaVon,
		Case When @XemGiaVon != '1' or gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
		MAX(tr.GiaBan) as GiaBan,
		Sum(tr.TonCuoiKy) as TonCuoiKy,
		MAX(tr.SrcImage) as SrcImage,
		Case when tr.ID_LoHang is null then NEWID() else tr.ID_LoHang end as ID_LoHang,
		MAX(tr.TenLoHang) as TenLoHang,
		MAX(tr.NgaySanXuat) as NgaySanXuat,
		MAX(tr.NgayHetHan) as NgayHetHan
		 FROM
		(
		SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHH AS TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
			a.QuanLyTheoLoHang,
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,
    			an.URLAnh as SrcImage,
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
			  where dvqd1.xoa is null and dhh1.duocbantructiep = '1'  and dhh1.TheoDoi = '1'
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1 and dhh.DuocBanTrucTiep = '1'
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
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
		--or a.MaLoHang like '%'+b.Name+'%' 
    		or a.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or a.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
			)=@count or @count=0)
    	and ((select count(*) from @tablenameChar c where
    		dvqd3.MaHangHoa like '%'+c.Name+'%' 
				--or a.MaLoHang like '%'+c.Name+'%' 
    		or a.TenHangHoa like '%'+c.Name+'%' 
			)= @countChar or @countChar=0)
    	and dvqd3.Xoa is null
		and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
		)
		tr
		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where tr.TrangThai = 1 or tr.TrangThai is null
		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, tr.QuanLyTheoLoHang, gv.ID, gv.GiaVon
		order by MAX(tr.NgayHetHan)");

            CreateStoredProcedure(name: "[dbo].[update_QuanLyLoHang]", body: @"update HT_CauHinhPhanMem set LoHang = '1'");

            CreateStoredProcedure(name: "[dbo].[UpDateGiaVonDMHangHoa]",
                body: @"DECLARE @TongHopNhapXuat TABLE (ID UNIQUEIDENTIFIER, ID_HoaDonCT UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_DonVi UNIQUEIDENTIFIER,ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TienChietKhau FLOAT,DonGia FLOAT,GiaVon FLOAT, LoaiHoaDon INT, YeuCau nvarchar(max), SoLuong FLOAT, ChoThanhToan bit, ID_LoHang UNIQUEIDENTIFIER, TongGiamGia FLOAT, TongTienHang FLOAT, TyLeChuyenDoi float) 
 INSERT INTO @TongHopNhapXuat
	SELECT * from(
	SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgayLapHoaDon, hd.ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
	FROM BH_HoaDon_ChiTiet bhct
	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	where hd.LoaiHoaDon != 3
	--order by hd.NgayLapHoaDon

	UNION all

	SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgayLapHoaDon, hd.ID_CheckIn as ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
	FROM BH_HoaDon_ChiTiet bhct
	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	where hd.LoaiHoaDon = 10 and hd.YeuCau = '4'
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
		SET @GiaVonUpDate = [dbo].FUNC_GiaVon(@ID, @ID_DonViQuiDoi,@ID_DonViQuiDoiDVT, @ID_HangHoa,@ID_LoHang, @ID_DonVi,@NgayLapHoaDon, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong, @TongGiamGia, @TongTienHang,@TyLeChuyenDoi)
	  
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
			END
			IF(@LoaiHoaDon = 8)
			BEGIN
				DECLARE @ThanhTienNew FLOAT;
				SET @ThanhTienNew = @GiaVonUpDate * @SoLuong
				UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate, ThanhTien = @ThanhTienNew where ID = @ID_HoaDonCT
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
				INSERT INTO DM_GiaVon(ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon) values (newID(), @ID_DonViQuiDoiGV, @ID_DonVi,@ID_LoHang, (@GiaVonUpDate / @TyLeChuyenDoi) * @TyLeChuyenDoiGV)
			END
			ELSE
			BEGIN
				DECLARE @GiaVonNew FLOAT; 
				SET @GiaVonNew = (@GiaVonUpDate / @TyLeChuyenDoi) * @TyLeChuyenDoiGV
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
	 DEALLOCATE CS_Item ");

            CreateStoredProcedure(name: "[dbo].[UpDateGiaVonDMHangHoaKhiTaoHD]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_HoaDon = p.Guid(),
                NgayHDEdit = p.DateTime()
            }, body: @"DECLARE @ListUpdateChiTietHD TABLE(ID_ChiNhanhHD UNIQUEIDENTIFIER, ID_DonViQuiDoiHH UNIQUEIDENTIFIER, ID_LoHangHH UNIQUEIDENTIFIER, DateTimeHD DateTime)
 INSERT INTO @ListUpdateChiTietHD SELECT * FROM (
	SELECT hd.ID_DonVi as ID_ChiNhanhHD, bhct.ID_DonViQuiDoi as ID_DonViQuiDoiHH, bhct.ID_LoHang as ID_LoHangHH, hd.NgayLapHoaDon as DateTimeHD FROM BH_HoaDon_ChiTiet bhct
	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
	where hd.ID = @ID_HoaDon and hd.ID_DonVi = @ID_ChiNhanh
 ) as b

 DECLARE @ID_ChiNhanhHD UNIQUEIDENTIFIER;
 DECLARE @ID_DonViQuiDoiHH UNIQUEIDENTIFIER;
 DECLARE @ID_LoHangHH UNIQUEIDENTIFIER;
 DECLARE @DateTimeHD DateTime;

 DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT * FROM @ListUpdateChiTietHD
 --forech chitiet của hd cần update
 OPEN CS_ItemUpDate 
 FETCH FIRST FROM CS_ItemUpDate INTO @ID_ChiNhanhHD, @ID_DonViQuiDoiHH,@ID_LoHangHH,@DateTimeHD
 WHILE @@FETCH_STATUS = 0
 BEGIN
	
	DECLARE @TongHopNhapXuat TABLE (ID UNIQUEIDENTIFIER, ID_HoaDonCT UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_DonVi UNIQUEIDENTIFIER,ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TienChietKhau FLOAT,DonGia FLOAT,GiaVon FLOAT, LoaiHoaDon INT, YeuCau nvarchar(max), SoLuong FLOAT, ChoThanhToan bit, ID_LoHang UNIQUEIDENTIFIER, TongGiamGia FLOAT, TongTienHang FLOAT, TyLeChuyenDoi float) 
    INSERT INTO @TongHopNhapXuat
	SELECT * from(
	SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgayLapHoaDon, hd.ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
	FROM BH_HoaDon_ChiTiet bhct
	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	WHERE hd.LoaiHoaDon != 3 and bhct.ID_DonViQuiDoi = @ID_DonViQuiDoiHH and (bhct.ID_LoHang = @ID_LoHangHH OR @ID_LoHangHH IS NULL) and hd.NgayLapHoaDon >= @NgayHDEdit
	--order by hd.NgayLapHoaDon

	UNION all

	SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgayLapHoaDon, hd.ID_CheckIn as ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
	FROM BH_HoaDon_ChiTiet bhct
	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	WHERE bhct.ID_DonViQuiDoi = @ID_DonViQuiDoiHH and (bhct.ID_LoHang = @ID_LoHangHH OR @ID_LoHangHH IS NULL) and hd.NgayLapHoaDon >= @NgayHDEdit and hd.LoaiHoaDon = 10 and hd.YeuCau = '4'
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
 DECLARE @countitem INT;
 SET @countitem = 1;

 DECLARE @GiaVonUpDate FLOAT;	
 DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @TongHopNhapXuat ORDER BY NgayLapHoaDon
 --foreach tất cả chi tiết của các hàng hóa trong hd cần update
 OPEN CS_Item 
 FETCH FIRST FROM CS_Item INTO @ID, @ID_HoaDonCT,@NgayLapHoaDon,@ID_DonVi, @ID_DonViQuiDoi,@ID_HangHoa, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong,@ChoThanhToan, @ID_LoHang, @TongGiamGia, @TongTienHang, @TyLeChuyenDoi
 WHILE @@FETCH_STATUS = 0
  BEGIN
		Declare @ID_DonViQuiDoiDVT nvarchar(max);
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
	   SET @GiaVonUpDate = [dbo].FUNC_GiaVon(@ID,@ID_DonViQuiDoi,@ID_DonViQuiDoiDVT, @ID_HangHoa,@ID_LoHang, @ID_DonVi,@NgayLapHoaDon, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong, @TongGiamGia, @TongTienHang,@TyLeChuyenDoi)
	
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
			END
			IF(@LoaiHoaDon = 8)
			BEGIN
				DECLARE @ThanhTienNew FLOAT;
				SET @ThanhTienNew = @GiaVonUpDate * @SoLuong
				UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate, ThanhTien = @ThanhTienNew where ID = @ID_HoaDonCT
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
		-- foreach đơn vị tính để update vào dm_giavon
		 OPEN CS_ItemGV 
		 FETCH FIRST FROM CS_ItemGV INTO @ID_DonViQuiDoiGV, @TyLeChuyenDoiGV
		 WHILE @@FETCH_STATUS = 0
		 BEGIN
			DECLARE @GiaVonCheck FLOAT; 
			select @GiaVonCheck = COUNT(ID) from DM_GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoiGV and ID_DonVi = @ID_DonVi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL)
			IF(@GiaVonCheck = 0)
			BEGIN
				INSERT INTO DM_GiaVon(ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon) values (newID(), @ID_DonViQuiDoiGV, @ID_DonVi,@ID_LoHang, (@GiaVonUpDate / @TyLeChuyenDoi) * @TyLeChuyenDoiGV)
			END
			ELSE
			BEGIN
				DECLARE @GiaVonNew FLOAT; 
				SET @GiaVonNew = (@GiaVonUpDate / @TyLeChuyenDoi) * @TyLeChuyenDoiGV
				UPDATE DM_GiaVon SET GiaVon = @GiaVonNew where ID_DonViQuiDoi = @ID_DonViQuiDoiGV and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL) and ID_DonVi = @ID_DonVi
			END

			FETCH NEXT FROM CS_ItemGV INTO @ID_DonViQuiDoiGV, @TyLeChuyenDoiGV
		 END
		 CLOSE CS_ItemGV
		 DEALLOCATE CS_ItemGV 
		 DELETE FROM @TableDonViQuiDoi
		--end update giá vốn cho từng đơn vị qui đổi

		SET @countitem = @countitem + 1;
	    FETCH NEXT FROM CS_Item INTO @ID, @ID_HoaDonCT,@NgayLapHoaDon,@ID_DonVi, @ID_DonViQuiDoi,@ID_HangHoa, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong,@ChoThanhToan,@ID_LoHang, @TongGiamGia, @TongTienHang,@TyLeChuyenDoi
	 
		END
		CLOSE CS_Item
		DEALLOCATE CS_Item 

	
		FETCH NEXT FROM CS_ItemUpDate INTO @ID_ChiNhanhHD, @ID_DonViQuiDoiHH,@ID_LoHangHH,@DateTimeHD
 END
 CLOSE CS_ItemUpDate
 DEALLOCATE CS_ItemUpDate");

            Sql(@"CREATE FUNCTION [dbo].[FUNC_TinhSLTon]
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

				AND bhd.NgayLapHoaDon < @TimeStart
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

            Sql(@"CREATE FUNCTION [dbo].[FUNC_GiaVon]
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
	@TyLeChuyenDoi [float]
)
RETURNS FLOAT
AS
BEGIN
DECLARE @GiaVonReturn AS FLOAT;
DECLARE @TonKhoHienTai FLOAT;
DECLARE @GiaVonHienTai FLOAT;
	DECLARE @ListChiTietByIDQD TABLE (ID_HoaDonCT UNIQUEIDENTIFIER,TyLeChuyenDoi FLOAT, NgayLapHoaDon DATETIME, ID_DonVi UNIQUEIDENTIFIER,ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TienChietKhau FLOAT,DonGia FLOAT,GiaVon FLOAT, GiaVon_NhanChuyenHang FLOAT, LoaiHoaDon INT, YeuCau nvarchar(max), SoLuong FLOAT, ChoThanhToan bit) 
	INSERT INTO @ListChiTietByIDQD
	SELECT TOP(1) bhct.ID as ID_HoaDonCT,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hd.ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon,bhct.GiaVon_NhanChuyenHang, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan
	FROM BH_HoaDon_ChiTiet bhct
	left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
	left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	where bhct.ID_DonViQuiDoi in (SELECT name from SplitString(@ID_DonViQuiDoi)) and hd.ChoThanhToan = 'false'  
	and hd.NgayLapHoaDon < @NgayLapHoaDon and hd.LoaiHoaDon != 3 and (bhct.ID_LoHang = @ID_LoHang or @ID_LoHang is null) AND ((hd.ID_DonVi = @ID_DonVi and (hd.YeuCau != '2' or hd.YeuCau is null)) or (hd.YeuCau = '4'  and hd.ID_CheckIn = @ID_DonVi))
	order by NgayLapHoaDon desc, SoThuTu, hd.LoaiHoaDon desc, hd.MaHoaDon desc

	SET @TonKhoHienTai = ISNULL([dbo].FUNC_TinhSLTon(@ID_DonVi,@ID_HangHoa,@ID_LoHang, @NgayLapHoaDon),0)

	DECLARE @CountCT INT;
	SELECT @CountCT = COUNT(ID_HoaDonCT) FROM @ListChiTietByIDQD 
	if(@CountCT = 0)
	BEGIN 
		if(@LoaiHoaDon = 4)
		BEGIN
			IF(@TongTienHang !=0)
			BEGIN
				SET @GiaVonReturn = (@DonGia - @TienChietKhau) * (1 - (@TongGiamGia / @TongTienHang))
			END
			ELSE
			BEGIN
				SET @GiaVonReturn = @DonGia - @TienChietKhau
			END
		END
		ELSE
		BEGIN
			IF(@LoaiHoaDon = 10)
			BEGIN
				DECLARE @ID_DonViCheckIn1 UNIQUEIDENTIFIER;
				SELECT @ID_DonViCheckIn1 = ID_CheckIn FROM BH_HoaDon WHERE ID = @ID
				IF(@YeuCau = '1' OR (@YeuCau = '4' AND @ID_DonVi != @ID_DonViCheckIn1))
				BEGIN
					SET @GiaVonReturn = 0
				END
				IF(@YeuCau = '4' AND @ID_DonVi = @ID_DonViCheckIn1)
				BEGIN
					SET @GiaVonReturn = @DonGia
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
		SELECT @ID_DonViCheckIn2 = ID_CheckIn FROM BH_HoaDon WHERE ID = @ID
		DECLARE @YeuCauCheck NVARCHAR(MAX);
		SELECT @YeuCauCheck = YeuCau FROM @ListChiTietByIDQD
		IF(@YeuCauCheck = '1' OR (@YeuCauCheck = '4' AND @ID_DonVi != @ID_DonViCheckIn2))
		BEGIN
			select @GiaVonHienTai = (ISNULL(GiaVon, 0)/ TyLeChuyenDoi) * @TyLeChuyenDoi from  @ListChiTietByIDQD
		END
		IF(@YeuCauCheck = '4' AND @ID_DonVi = @ID_DonViCheckIn2)
		BEGIN
			select @GiaVonHienTai = (ISNULL(GiaVon_NhanChuyenHang, 0)/ TyLeChuyenDoi) * @TyLeChuyenDoi from  @ListChiTietByIDQD
		END
		ELSE
		BEGIN
			select @GiaVonHienTai = (ISNULL(GiaVon, 0)/ TyLeChuyenDoi) * @TyLeChuyenDoi from  @ListChiTietByIDQD
		END
		
		SET @GiaVonReturn = @GiaVonHienTai
		--begin loaihoadon == 4
		if(@LoaiHoaDon = 4)
		BEGIN
			IF((@TonKhoHienTai + @SoLuong * @TyLeChuyenDoi) > 0)
			BEGIN
				IF(@TongTienHang != 0)
				BEGIN
					SET @GiaVonReturn = ((@DonGia - @TienChietKhau) * (1 - (@TongGiamGia/@TongTienHang)) * @SoLuong +  @GiaVonHienTai * @TonKhoHienTai) / (@TonKhoHienTai + @SoLuong * @TyLeChuyenDoi)				
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = ((@DonGia - @TienChietKhau) * @SoLuong +  @GiaVonHienTai * @TonKhoHienTai) / (@TonKhoHienTai + @SoLuong * @TyLeChuyenDoi)				
				END
			END
			ELSE
			BEGIN
				IF(@TongTienHang != 0)
				BEGIN
					SET @GiaVonReturn = (@DonGia - @TienChietKhau) * (1 - (@TongGiamGia / @TongTienHang))
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = @DonGia - @TienChietKhau
				END
			END
		END
		--end loaihoadon = 4

		--BEGIn loaihoadon = 7
		IF(@LoaiHoaDon = 7)
		BEGIN
			IF((@TonKhoHienTai - @SoLuong *@TyLeChuyenDoi) > 0)
			BEGIN
				SET @GiaVonReturn = (@GiaVonHienTai * @TonKhoHienTai - @DonGia * @SoLuong)/(@TonKhoHienTai - @SoLuong * @TyLeChuyenDoi)
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
			DECLARE @ID_DonViCheckIn UNIQUEIDENTIFIER;
			SELECT @ID_DonViCheckIn = ID_CheckIn FROM BH_HoaDon WHERE ID = @ID
			IF(@YeuCau = '1' OR (@YeuCau = '4' AND @ID_DonVi != @ID_DonViCheckIn))
			BEGIN
				SET @GiaVonReturn = @GiaVonHienTai
			END
			IF(@YeuCau = '4' AND @ID_DonVi = @ID_DonViCheckIn)
			BEGIN
				IF((@TonKhoHienTai + @TienChietKhau * @TyLeChuyenDoi) > 0)
				BEGIN
					SET @GiaVonReturn = (@GiaVonHienTai * @TonKhoHienTai + @DonGia*@TienChietKhau* @TyLeChuyenDoi) / (@TonKhoHienTai + @TienChietKhau * @TyLeChuyenDoi)
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = @DonGia
				END
			END
		END
		-- END loaihoadon =10

		--BEGIN loaihoadon = 6
		IF(@LoaiHoaDon=6)
		BEGIN
			DECLARE @ID_HoaDonTH UNIQUEIDENTIFIER;
			DECLARE @Check INT;
			SELECT @ID_HoaDonTH = ID_HoaDon FROM BH_HoaDon WHERE ID = @ID
	
			IF(@ID_HoaDonTH is not null)
			BEGIN
				DECLARE @GiaVonHDBan FLOAT;
				SELECT @GiaVonHDBan = GiaVon FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @ID_HoaDonTH and ID_DonViQuiDoi = @ID_DonViQuiDoiTH
				IF(@TonKhoHienTai + @SoLuong > 0)
				BEGIN
					SET @GiaVonReturn = (@GiaVonHienTai * @TonKhoHienTai + @GiaVonHDBan *@SoLuong) /(@TonKhoHienTai + @SoLuong)
				END
				ELSE
				BEGIN
					SET @GiaVonReturn = @GiaVonHDBan
				END
			END
			ELSE
			BEGIN
				SET @GiaVonReturn = @GiaVonHienTai
			END
		END
		--END loaihoadon=6

		-- BEGIN loaihoadon = 18
		IF(@LoaiHoaDon = 18)
		BEGIN
			SET @GiaVonReturn = @GiaVonHienTai
		END
		-- END loaihoadon =18

		-- BEGIN loaihoadon khác
		IF(@LoaiHoaDon = 1 OR @LoaiHoaDon = 5 OR @LoaiHoaDon = 8 OR @LoaiHoaDon = 9)
		BEGIN
			SET @GiaVonReturn = @GiaVonHienTai
		END
		-- END LoaiHoaDon khác
		DELETE FROM @ListChiTietByIDQD
	END
	
	RETURN @GiaVonReturn
END
");

            //Sql("EXEC UpDateGiaVonDMHangHoa");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_DMHangHoa_Import]");
            DropStoredProcedure("[dbo].[getlist_HoaDonBanHang_FindMaHang]");
            DropStoredProcedure("[dbo].[getlist_HoaDonDatHang_FindMaHang]");
            DropStoredProcedure("[dbo].[getlist_HoaDonTraHang_FindMaHang]");
            DropStoredProcedure("[dbo].[getListHangHoaLoHang_ByMaHangHoa]");
            DropStoredProcedure("[dbo].[getListHangHoaLoHang_ChotSo_ByMaHangHoa]");
            DropStoredProcedure("[dbo].[getListHangHoaLoHang_ChotSo_EnTer]");
            DropStoredProcedure("[dbo].[getListHangHoaLoHang_EnTer]");
            DropStoredProcedure("[dbo].[getListXuatKho_Import]");
            DropStoredProcedure("[dbo].[getListXuatKho_Import_ChotSo]");
            DropStoredProcedure("[dbo].[insert_DM_GiaVon]");
            DropStoredProcedure("[dbo].[insert_HoaDonLoHang_ChiTiet]");
            DropStoredProcedure("[dbo].[insert_LoHang]");
            DropStoredProcedure("[dbo].[LoadDanhMucLoHangBaoCao]");
            DropStoredProcedure("[dbo].[Search_DMHangHoaLoHang_TonKho]");
            DropStoredProcedure("[dbo].[Search_DMHangHoaLoHang_TonKho_ChotSo]");
            DropStoredProcedure("[dbo].[update_QuanLyLoHang]");
            DropStoredProcedure("[dbo].[UpDateGiaVonDMHangHoa]");
            DropStoredProcedure("[dbo].[UpDateGiaVonDMHangHoaKhiTaoHD]");
        }
    }
}