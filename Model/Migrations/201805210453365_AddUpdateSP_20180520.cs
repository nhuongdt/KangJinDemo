namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180520 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoI]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"-- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) as GiaTriCuoiKy
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
			MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			ISNULL(td.TonKho, 0) AS TonKho
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				ISNULL(cs.TonKho, 0) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				WHERE dvqd.ladonvichuan = '1'
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
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM 
    				(
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
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    		GROUP BY dhh.ID , dhh.ID_NhomHang
    		) a
    	    LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    		where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"-- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    
    	SELECT 		
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) as GiaTriCuoiKy
    	FROM 
		(
    		SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap2,0)) - SUM(ISNULL(td.SoLuongXuat2,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				ISNULL(cs.TonKho, 0) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian bắt đầu đến khi chốt sổ
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    		GROUP BY dhh.ID , dhh.ID_NhomHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoIII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    
    	SELECT 
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) as GiaTriCuoiKy
    	FROM 
    	--select * FROM
		(
    		SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			--NULL as TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongXuat,0)) - SUM(ISNULL(td.SoLuongNhap,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				ISNULL(cs.TonKho, 0) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				--NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				NULL AS TonCuoiKy
    				FROM 
    				(
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
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    		GROUP BY dhh.ID , dhh.ID_NhomHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoIV]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT 
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) as GiaTriCuoiKy
    	FROM 
    	--select * FROM
		(
    		SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat
    			FROM
    			(
    
    				-- phát sinh xuất nhập tồn đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
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
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
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
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    		GROUP BY dhh.ID , dhh.ID_NhomHang
			) a
    			LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
			where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
			order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopNhapKho]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String()
            }, body: @"SELECT 
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap
    	FROM 
		(
    		SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap
    		FROM
    		(
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.LoaiHoaDon,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    		GROUP BY dhh.ID , dhh.ID_NhomHang
		) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by GiaTriNhap desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopNhapKhoChiTiet]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String()
            }, body: @"SELECT 
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
		a.TenLoaiChungTu,
		a.MaHoaDon,
		a.NgayLapHoaDon,
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap
    	FROM 
		(
    		SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
			HangHoa.TenLoaiChungTu,
			HangHoa.MaHoaDon,
			Max(HangHoa.NgayLapHoaDon) AS NgayLapHoaDon,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap
    		FROM
    		(
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.LoaiHoaDon,
					ct.TenLoaiChungTu,
					pstk.MaHoaDon,
					Max(pstk.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap
    				FROM 
    				(
    				SELECT 
					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon
    
    				UNION ALL
    				SELECT 
					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon
    
    				UNION ALL
    				SELECT 
					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon
    			) AS pstk
				LEFT JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, ct.TenLoaiChungTu, pstk.MaHoaDon
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    		GROUP BY dhh.ID , dhh.ID_NhomHang, HangHoa.TenLoaiChungTu, HangHoa.MaHoaDon
		) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopXuatKho]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String()
            }, body: @"SELECT 
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat
    	FROM 
    	--select * FROM
		(
			SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat
    		FROM
    		(
    				SELECT
    				pstk.ID_DonViQuiDoi,
					pstk.LoaiHoaDon,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    		GROUP BY dhh.ID , dhh.ID_NhomHang
		) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by GiaTriXuat desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopXuatKhoChiTiet]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String()
            }, body: @"SELECT 
		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
		a.TenLoaiChungTu,
		a.MaHoaDon,
		a.NgayLapHoaDon,
		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		dvqd3.TenDonViTinh, 
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat
    	FROM 
    	--select * FROM
		(
			SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,
			HangHoa.TenLoaiChungTu,
			HangHoa.MaHoaDon,
			Max(HangHoa.NgayLapHoaDon) AS NgayLapHoaDon,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat
    		FROM
    		(
    				SELECT
    				pstk.ID_DonViQuiDoi,
					pstk.LoaiHoaDon,
					ct.TenLoaiChungTu,
					pstk.MaHoaDon,
					Max(pstk.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon
    
    				UNION ALL
    				SELECT 
					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					bhd.LoaiHoaDon,
					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon
    			) AS pstk
				LEFT JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, ct.TenLoaiChungTu, pstk.MaHoaDon
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    		GROUP BY dhh.ID , dhh.ID_NhomHang, HangHoa.TenLoaiChungTu, HangHoa.MaHoaDon
		) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[getListDM_LoaiChungTu]", parametersAction: p => new
            {
                ID_LoaiChungTu = p.String()
            }, body: @"select ID, TenLoaiChungTu as TenChungTu from DM_LoaiChungTu where ID in (select * from splitstring(@ID_LoaiChungTu))
	ORDER BY ID");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_KhachHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    	hh.ID_NhomHang,
    	a.SoLuongKhachHang,
    	CAST(ROUND(a.SoLuongMua , 3) as float ) as SoLuongMua,
    	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
    	FROM
    	(
    		SELECT 
    		c.ID_DonViQuiDoi,
    		COUNT(*) as SoLuongKhachHang,
    		SUM(ISNULL(c.SoLuongMua, 0)) as SoLuongMua,
    		SUM(ISNULL(c.GiaTri, 0)) as GiaTri
    		FROM
    		(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongMua,
    			SUM(ISNULL(hdct.ThanhTien, 0) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.LoaiHoaDon = 1
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and hh.LaHangHoa like @LaHangHoa
				and hd.TongTienHang > 0
    			GROUP BY dvqd.ID, hd.ID_DoiTuong
    		) c
    		GROUP BY ID_DonViQuiDoi
    	) a
    	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    	ORDER BY SoLuongKhachHang DESC");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportHangHoa_TonKhoI]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TonKhoII]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TonKhoIII]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TonKhoIV]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TongHopNhapKho]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TongHopNhapKhoChiTiet]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TongHopXuatKho]");
            DropStoredProcedure("[dbo].[ReportHangHoa_TongHopXuatKhoChiTiet]");
            DropStoredProcedure("[dbo].[getListDM_LoaiChungTu]");
        }
    }
}