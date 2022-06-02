namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180918 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_ChiTietHangNhap]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                LoaiChungTu = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

	SELECT 
				c.TenLoaiChungTu as LoaiHoaDon,
				c.MaHoaDon as MaHoaDon,
				c.NgayLapHoaDon as NgayLapHoaDon,
				c.TenNhomHangHoa as TenNhomHang,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
    		c.SoLuongNhap as SoLuong,
    		Case When @XemGiaVon = '1' then  c.GiaTriNhap else 0 end as ThanhTien
	FROM
	(
    SELECT 
    		a.TenLoaiChungTu,
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		Case when a.ID_NhomHang is null then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
			Case when a.ID_NhomHang is null then N'nhom mac dinh' else a.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
			Case when a.ID_NhomHang is null then N'nmd' else a.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
			a.TenHangHoa_KhongDau,
		a.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			HangHoa.TenLoaiChungTu,
    			HangHoa.MaHoaDon,
    			Max(HangHoa.NgayLapHoaDon) AS NgayLapHoaDon,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
			MAX(dnhh.TenNhomHangHoa_KhongDau) AS TenNhomHangHoa_KhongDau,
			MAX(dnhh.TenNhomHangHoa_KyTuDau) AS TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap
    		FROM
    		(
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
					SUM(ISNULL(bhdct.ThanhToan, 0)) AS GiaTriNhap
    				--SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    				LEFT JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, ct.TenLoaiChungTu, pstk.MaHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    				and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, HangHoa.TenLoaiChungTu, HangHoa.MaHoaDon, dvqd.Xoa, HangHoa.ID_LoHang
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
    where LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    ) c
		where c.ID_NhomHang like @ID_NhomHang
		and (MaHoaDon like @MaHH_TV or MaHoaDon like @MaHH or MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_ChiTietHangXuat]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                LoaiChungTu = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

	SELECT 
				c.TenLoaiChungTu as LoaiHoaDon,
				c.MaHoaDon as MaHoaDon,
				c.NgayLapHoaDon as NgayLapHoaDon,
				c.TenNhomHangHoa as TenNhomHang,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
    		c.SoLuongXuat as SoLuong,
    		Case When @XemGiaVon = '1' then  c.GiaTriXuat else 0 end as ThanhTien
	FROM
	(
    SELECT 
    		a.TenLoaiChungTu,
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		Case when a.ID_NhomHang is null then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
			Case when a.ID_NhomHang is null then N'nhom mac dinh' else a.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
			Case when a.ID_NhomHang is null then N'nmd' else a.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
		a.TenHangHoa_KhongDau,
		a.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat
    	FROM 
    	--select * FROM
    		(
    			SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			HangHoa.TenLoaiChungTu,
    			HangHoa.MaHoaDon,
    			Max(HangHoa.NgayLapHoaDon) AS NgayLapHoaDon,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
			MAX(dnhh.TenNhomHangHoa_KhongDau) AS TenNhomHangHoa_KhongDau,
			MAX(dnhh.TenNhomHangHoa_KyTuDau) AS TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat
    		FROM
    		(
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    				LEFT JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, ct.TenLoaiChungTu, pstk.MaHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    				and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, HangHoa.TenLoaiChungTu, HangHoa.MaHoaDon, dvqd.Xoa, HangHoa.ID_LoHang
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
    where LaDonViChuan = 1
    	and a.Xoa like @TrangThai
     ) c
		where c.ID_NhomHang like @ID_NhomHang
		and (MaHoaDon like @MaHH_TV or MaHoaDon like @MaHH or MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	SELECT 
				c.TenNhomHangHoa as TenNhomHang,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
    		CAST(ROUND(c.SoLuong, 3) as float) as SoLuong, 
    		Case When @XemGiaVon = '1' then  CAST(ROUND(c.GiaTri, 0) as float) else 0 end as ThanhTien
	FROM
	(
    SELECT
    		dvqd.ID,
    		Case when hh.ID_NhomHang is not null then hh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when dnhh.TenNhomHangHoa is null then N'Nhóm mặc định' else dnhh.TenNhomHangHoa end as TenNhomHangHoa,
				Case when dnhh.TenNhomHangHoa_KhongDau is null then N'nhom mac dinh' else dnhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
				Case when dnhh.TenNhomHangHoa_KyTuDau is null then N'nmd' else dnhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    	dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
		hh.TenHangHoa_KhongDau,
		hh.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND(a.SoLuong , 3) as float ) as SoLuong,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTri , 0) as float ) else 0 end as GiaTri
    	FROM
    	(
    			SELECT
    				b.ID_DonViQuiDoi as ID_DonViQuiDoi,
    					b. ID_LoHang,
    				SUM(ISNULL(b.SoLuong, 3)) as SoLuong,
    				SUM(ISNULL(b.GiaTri, 0)) as GiaTri
    			FROM
    			(
    				SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				ISNULL(hdct.tienchietkhau,0) as SoLuong,
    				ISNULL(hdct.ThanhTien,0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.ID_CheckIn is not null 
    				and hd.ID_CheckIn in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = '10' and hd.YeuCau = '4')
    			and hd.ChoThanhToan = 0
    			) b
    			GROUP BY b.ID_DonViQuiDoi, b.ID_LoHang
    	) a
    	inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
				LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
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
    		where hh.LaHangHoa like  @LaHangHoa
			and hh.TheoDoi like @TheoDoi
    	) c
			 where c.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    			and c.Xoa like @TrangThai
    	ORDER BY ThanhTien DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapChuyenHangChiTiet]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

	SELECT 
				c.MaHoaDon as MaHoaDon,
				c.NgayLapHoaDon as NgayLapHoaDon,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
				c.ChiNhanhChuyen,
				c.ChiNhanhNhan,
    		CAST(ROUND(c.SoLuong, 3) as float) as SoLuong, 
			Case When @XemGiaVon = '1' then  CAST(ROUND(c.DonGia, 0) as float) else 0 end as DonGia,
    		Case When @XemGiaVon = '1' then  CAST(ROUND(c.ThanhTien, 0) as float) else 0 end as ThanhTien
	FROM
	(
    Select 
    		Case when hh.ID_NhomHang is not null then hh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when dnhh.TenNhomHangHoa is null then N'Nhóm mặc định' else dnhh.TenNhomHangHoa end as TenNhomHangHoa,
				Case when dnhh.TenNhomHangHoa_KhongDau is null then N'nhom mac dinh' else dnhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
				Case when dnhh.TenNhomHangHoa_KyTuDau is null then N'nmd' else dnhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    	bhhd.MaHoaDon,
    	bhhd.NgayLapHoaDon,
    		dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
			hh.TenHangHoa_KhongDau,
		hh.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		dvn.TenDonVi as ChiNhanhChuyen,
    	dv.TenDonVi as ChiNhanhNhan,
    	CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
    	CAST(ROUND(ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0), 0) as float) as DonGia,
    	CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float) as ThanhTien
    	From
    	BH_HoaDon bhhd 
    	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
    	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
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
    	where hh.TheoDoi like @TheoDoi
    and hh.LaHangHoa like @LaHangHoa
    	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
    		and bhhd.ChoThanhToan = 0
    	and (bhhd.ID_CheckIn is not null and bhhd.ID_CheckIn in (select * from splitstring(@ID_ChiNhanh)) 
    		and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4')
    	) c
		where c.ID_NhomHang like @ID_NhomHang
		and (MaHoaDon like @MaHH_TV or MaHoaDon like @MaHH or MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    	and c.Xoa like @TrangThai
    	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonChiTietI]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT 
		dvqd3.ID as ID_DonViQuiDoi,
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
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
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    	    -- dhh.LaHangHoa,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDauKy,0))  AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0))  AS SoLuongNhap_NCC,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0))  AS SoLuongNhap_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0))  AS SoLuongNhap_Tra,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0))  AS SoLuongNhap_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongNhap_SX,0))  AS SoLuongNhap_SX,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0))  AS SoLuongXuat_Ban,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  AS SoLuongXuat_Huy,
    		SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  AS SoLuongXuat_NCC,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0))  AS SoLuongXuat_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0))  AS SoLuongXuat_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongXuat_SX,0))  AS SoLuongXuat_SX,
    		SUM(ISNULL(HangHoa.TonDauKy,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0))- SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0)) - SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT 
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDauKy,
    			NULL AS SoLuongNhap_NCC,
    			NULL AS SoLuongNhap_Kiem,
    			NULL AS SoLuongNhap_Tra,
    			NULL AS SoLuongNhap_Chuyen,
    			NULL AS SoLuongNhap_SX,
    			NULL AS SoLuongXuat_Ban,
    			NULL AS SoLuongXuat_Huy,
    			NULL AS SoLuongXuat_NCC,
    			NULL AS SoLuongXuat_Kiem,
    			NULL AS SoLuongXuat_Chuyen,
    			NULL AS SoLuongXuat_SX,
    			NULL AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				NULL AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
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
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                      
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDauKy,
    				SUM(ISNULL(pstk.SoLuongNhap_NCC, 0)) AS SoLuongNhap_NCC,
    				SUM(ISNULL(pstk.SoLuongNhap_Kiem, 0)) AS SoLuongNhap_Kiem,
    				SUM(ISNULL(pstk.SoLuongNhap_Tra, 0)) AS SoLuongNhap_Tra,
    				SUM(ISNULL(pstk.SoLuongNhap_Chuyen, 0)) AS SoLuongNhap_Chuyen,
    				SUM(ISNULL(pstk.SoLuongNhap_SX, 0)) AS SoLuongNhap_SX,
    				SUM(ISNULL(pstk.SoLuongXuat_Ban, 0)) AS SoLuongXuat_Ban,
    				SUM(ISNULL(pstk.SoLuongXuat_Huy, 0)) AS SoLuongXuat_Huy,
    				SUM(ISNULL(pstk.SoLuongXuat_NCC, 0)) AS SoLuongXuat_NCC,
    				SUM(ISNULL(pstk.SoLuongXuat_Kiem, 0)) AS SoLuongXuat_Kiem,
    				SUM(ISNULL(pstk.SoLuongXuat_Chuyen, 0)) AS SoLuongXuat_Chuyen,
    				SUM(ISNULL(pstk.SoLuongXuat_SX, 0)) AS SoLuongXuat_SX,
    				NULL AS TonCuoiKy
    				FROM 
    				(
    					-- Xuất bán
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                            
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
    where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonChiTietII]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT
		dvqd3.ID as ID_DonViQuiDoi,
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri, 
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
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
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDauKy,0))  AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0))  AS SoLuongNhap_NCC,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0))  AS SoLuongNhap_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0))  AS SoLuongNhap_Tra,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0))  AS SoLuongNhap_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongNhap_SX,0))  AS SoLuongNhap_SX,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0))  AS SoLuongXuat_Ban,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  AS SoLuongXuat_Huy,
    		SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  AS SoLuongXuat_NCC,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0))  AS SoLuongXuat_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0))  AS SoLuongXuat_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongXuat_SX,0))  AS SoLuongXuat_SX,
    		SUM(ISNULL(HangHoa.TonDauKy,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0))
			- SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0)) - SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT 
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			--SUM(ISNULL(td.TonKho + td.SoLuongXuat_Ban + td.SoLuongXuat_Huy + td.SoLuongXuat_NCC + td.SoLuongXuat_Kiem + td.SoLuongXuat_Chuyen  
				-- - td.SoLuongNhap_NCC - td.SoLuongNhap_Kiem - td.SoLuongNhap_Tra - td.SoLuongNhap_Chuyen, 0)) AS TonDauKy,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongXuat_Ban,0) + ISNULL(td.SoLuongXuat_Huy,0) + ISNULL(td.SoLuongXuat_NCC,0) + ISNULL(td.SoLuongXuat_Kiem,0) + ISNULL(td.SoLuongXuat_Chuyen,0)) 
				- SUM(ISNULL(td.SoLuongNhap_NCC,0) + ISNULL(td.SoLuongNhap_Kiem,0) + ISNULL(td.SoLuongNhap_Tra,0) + ISNULL(td.SoLuongNhap_Chuyen,0)) AS TonDauKy,
				SUM(ISNULL(td.SoLuongNhap_NCC, 0)) AS SoLuongNhap_NCC,
    			SUM(ISNULL(td.SoLuongNhap_Kiem, 0)) AS SoLuongNhap_Kiem,
    			SUM(ISNULL(td.SoLuongNhap_Tra, 0)) AS SoLuongNhap_Tra,
    			SUM(ISNULL(td.SoLuongNhap_Chuyen, 0)) AS SoLuongNhap_Chuyen,
    			SUM(ISNULL(td.SoLuongNhap_SX, 0)) AS SoLuongNhap_SX,
    			SUM(ISNULL(td.SoLuongXuat_Ban, 0)) AS SoLuongXuat_Ban,
    			SUM(ISNULL(td.SoLuongXuat_Huy, 0)) AS SoLuongXuat_Huy,
    			SUM(ISNULL(td.SoLuongXuat_NCC, 0)) AS SoLuongXuat_NCC,
    			SUM(ISNULL(td.SoLuongXuat_Kiem, 0)) AS SoLuongXuat_Kiem,
    			SUM(ISNULL(td.SoLuongXuat_Chuyen, 0)) AS SoLuongXuat_Chuyen,
    			SUM(ISNULL(td.SoLuongXuat_SX, 0)) AS SoLuongXuat_SX,
    			NULL AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID as ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
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
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				NULL AS TonDauKy,
    				SUM(ISNULL(pstk.SoLuongNhap_NCC, 0)) AS SoLuongNhap_NCC,
    				SUM(ISNULL(pstk.SoLuongNhap_Kiem, 0)) AS SoLuongNhap_Kiem,
    				SUM(ISNULL(pstk.SoLuongNhap_Tra, 0)) AS SoLuongNhap_Tra,
    				SUM(ISNULL(pstk.SoLuongNhap_Chuyen, 0)) AS SoLuongNhap_Chuyen,
    				SUM(ISNULL(pstk.SoLuongNhap_SX, 0)) AS SoLuongNhap_SX,
    				SUM(ISNULL(pstk.SoLuongXuat_Ban, 0)) AS SoLuongXuat_Ban,
    				SUM(ISNULL(pstk.SoLuongXuat_Huy, 0)) AS SoLuongXuat_Huy,
    				SUM(ISNULL(pstk.SoLuongXuat_NCC, 0)) AS SoLuongXuat_NCC,
    				SUM(ISNULL(pstk.SoLuongXuat_Kiem, 0)) AS SoLuongXuat_Kiem,
    				SUM(ISNULL(pstk.SoLuongXuat_Chuyen, 0)) AS SoLuongXuat_Chuyen,
    				SUM(ISNULL(pstk.SoLuongXuat_SX, 0)) AS SoLuongXuat_SX,
    				--SUM(ISNULL(td.TonKho + td.SoLuongXuat_Ban + td.SoLuongXuat_Huy + td.SoLuongXuat_NCC + td.SoLuongXuat_Kiem + td.SoLuongXuat_Chuyen   - td.SoLuongNhap_NCC - td.SoLuongNhap_Kiem - td.SoLuongNhap_Tra - td.SoLuongNhap_Chuyen, 0)) AS TonCuoiKy,
    				NULL AS TonCuoiKy
    				FROM 
    				(
    					-- Xuất bán
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
     where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonChiTietIII]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT
		dvqd3.ID as ID_DonViQuiDoi,
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
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
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) - SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0)) - SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0)) - SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0)) - SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0)) + SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0)) + SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  + SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  + SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0))  + SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0)) AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0))  AS SoLuongNhap_NCC,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0))  AS SoLuongNhap_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0))  AS SoLuongNhap_Tra,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0))  AS SoLuongNhap_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongNhap_SX,0))  AS SoLuongNhap_SX,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0))  AS SoLuongXuat_Ban,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  AS SoLuongXuat_Huy,
    		SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  AS SoLuongXuat_NCC,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0))  AS SoLuongXuat_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0))  AS SoLuongXuat_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongXuat_SX,0))  AS SoLuongXuat_SX,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT 
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			NULL AS TonDauKy,
    			NULL AS SoLuongNhap_NCC,
    			NULL AS SoLuongNhap_Kiem,
    			NULL AS SoLuongNhap_Tra,
    			NULL AS SoLuongNhap_Chuyen,
    			NULL AS SoLuongNhap_SX,
    			NULL AS SoLuongXuat_Ban,
    			NULL AS SoLuongXuat_Huy,
    			NULL AS SoLuongXuat_NCC,
    			NULL AS SoLuongXuat_Kiem,
    			NULL AS SoLuongXuat_Chuyen,
    			NULL AS SoLuongXuat_SX,
    			SUM(ISNULL(td.TonKho , 0) + ISNULL(td.SoLuongXuat_Ban, 0) + ISNULL(td.SoLuongXuat_Huy, 0) + ISNULL(td.SoLuongXuat_NCC, 0) + ISNULL(td.SoLuongXuat_Kiem, 0) +
    			 ISNULL(td.SoLuongXuat_Chuyen,  0) - ISNULL(td.SoLuongNhap_NCC, 0) - ISNULL(td.SoLuongNhap_Tra, 0) - ISNULL(td.SoLuongNhap_Kiem, 0) - ISNULL(td.SoLuongNhap_Chuyen, 0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID as ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    
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
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDauKy,
    				SUM(ISNULL(pstk.SoLuongNhap_NCC, 0)) AS SoLuongNhap_NCC,
    				SUM(ISNULL(pstk.SoLuongNhap_Kiem, 0)) AS SoLuongNhap_Kiem,
    				SUM(ISNULL(pstk.SoLuongNhap_Tra, 0)) AS SoLuongNhap_Tra,
    				SUM(ISNULL(pstk.SoLuongNhap_Chuyen, 0)) AS SoLuongNhap_Chuyen,
    				SUM(ISNULL(pstk.SoLuongNhap_SX, 0)) AS SoLuongNhap_SX,
    				SUM(ISNULL(pstk.SoLuongXuat_Ban, 0)) AS SoLuongXuat_Ban,
    				SUM(ISNULL(pstk.SoLuongXuat_Huy, 0)) AS SoLuongXuat_Huy,
    				SUM(ISNULL(pstk.SoLuongXuat_NCC, 0)) AS SoLuongXuat_NCC,
    				SUM(ISNULL(pstk.SoLuongXuat_Kiem, 0)) AS SoLuongXuat_Kiem,
    				SUM(ISNULL(pstk.SoLuongXuat_Chuyen, 0)) AS SoLuongXuat_Chuyen,
    				SUM(ISNULL(pstk.SoLuongXuat_SX, 0)) AS SoLuongXuat_SX,
    				NULL AS TonCuoiKy
    				FROM 
    				(
    					-- Xuất bán
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
   where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonChiTietIV]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    SELECT 
		dvqd3.ID as ID_DonViQuiDoi,
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa, 
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
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
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDauKy,0)) AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0))  AS SoLuongNhap_NCC,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0))  AS SoLuongNhap_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0))  AS SoLuongNhap_Tra,
    		SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0))  AS SoLuongNhap_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongNhap_SX,0))  AS SoLuongNhap_SX,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0))  AS SoLuongXuat_Ban,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  AS SoLuongXuat_Huy,
    		SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  AS SoLuongXuat_NCC,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0))  AS SoLuongXuat_Kiem,
    		SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0))  AS SoLuongXuat_Chuyen,
    		SUM(ISNULL(HangHoa.SoLuongXuat_SX,0))  AS SoLuongXuat_SX,
    		SUM(ISNULL(HangHoa.TonDauKy,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_NCC,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Kiem,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Tra,0)) + SUM(ISNULL(HangHoa.SoLuongNhap_Chuyen,0)) - 
    		SUM(ISNULL(HangHoa.SoLuongXuat_Ban,0)) - SUM(ISNULL(HangHoa.SoLuongXuat_Huy,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_NCC,0))  - SUM(ISNULL(HangHoa.SoLuongXuat_Kiem,0)) - SUM(ISNULL(HangHoa.SoLuongXuat_Chuyen,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT 
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap_NCC, 0) + ISNULL(td.SoLuongNhap_Tra, 0) + ISNULL(td.SoLuongNhap_Kiem, 0) + ISNULL(td.SoLuongNhap_Chuyen, 0) -  ISNULL(td.SoLuongXuat_Ban, 0) - 
    			ISNULL(td.SoLuongXuat_Huy, 0) - ISNULL(td.SoLuongXuat_NCC, 0) - ISNULL(td.SoLuongXuat_Kiem, 0) - ISNULL(td.SoLuongXuat_Chuyen,  0)) AS TonDauKy,
    			NULL AS SoLuongNhap_NCC,
    			NULL AS SoLuongNhap_Kiem,
    			NULL AS SoLuongNhap_Tra,
    			NULL AS SoLuongNhap_Chuyen,
    			NULL AS SoLuongNhap_SX,
    			NULL AS SoLuongXuat_Ban,
    			NULL AS SoLuongXuat_Huy,
    			NULL AS SoLuongXuat_NCC,
    			NULL AS SoLuongXuat_Kiem,
    			NULL AS SoLuongXuat_Chuyen,
    			NULL AS SoLuongXuat_SX,
    			NULL AS TonCuoiKy
    			FROM
    			(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang               
    					                                                                                                                                                                                                                                              
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDauKy,
    				SUM(ISNULL(pstk.SoLuongNhap_NCC, 0)) AS SoLuongNhap_NCC,
    				SUM(ISNULL(pstk.SoLuongNhap_Kiem, 0)) AS SoLuongNhap_Kiem,
    				SUM(ISNULL(pstk.SoLuongNhap_Tra, 0)) AS SoLuongNhap_Tra,
    				SUM(ISNULL(pstk.SoLuongNhap_Chuyen, 0)) AS SoLuongNhap_Chuyen,
    				SUM(ISNULL(pstk.SoLuongNhap_SX, 0)) AS SoLuongNhap_SX,
    				SUM(ISNULL(pstk.SoLuongXuat_Ban, 0)) AS SoLuongXuat_Ban,
    				SUM(ISNULL(pstk.SoLuongXuat_Huy, 0)) AS SoLuongXuat_Huy,
    				SUM(ISNULL(pstk.SoLuongXuat_NCC, 0)) AS SoLuongXuat_NCC,
    				SUM(ISNULL(pstk.SoLuongXuat_Kiem, 0)) AS SoLuongXuat_Kiem,
    				SUM(ISNULL(pstk.SoLuongXuat_Chuyen, 0)) AS SoLuongXuat_Chuyen,
    				SUM(ISNULL(pstk.SoLuongXuat_SX, 0)) AS SoLuongXuat_SX,
    				NULL AS TonCuoiKy
    				FROM 
    				(
    					-- Xuất bán
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- xuất kho
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Kiem, -- kiểm kê
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- trả hàng nhà cung cấp
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    
    				-- Xuất hủy
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Xuất Chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập NCC
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập kiem ke
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập Khách trả hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    				NULL AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
    				-- Nhập chuyển hàng
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang, 
    				NULL AS SoLuongNhap_NCC,
    				NULL AS SoLuongNhap_Kiem,
    				NULL AS SoLuongNhap_Tra,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    				NULL AS SoLuongNhap_SX,
    				NULL AS SoLuongXuat_Ban,
    				NULL AS SoLuongXuat_Huy,
    				NULL AS SoLuongXuat_NCC,
    				NULL AS SoLuongXuat_Kiem, 
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
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
				where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
	) tr
    left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
	Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
	order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonI]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
		MAX(tr.SoLuongNhap) as SoLuongNhap,
		MAX(tr.GiaTriNhap) as GiaTriNhap,
		MAX(tr.SoLuongXuat) as SoLuongXuat,
		MAX(tr.GiaTriXuat) as GiaTriXuat,
		tr.TonCuoiKy,
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT 
		dvqd3.ID as ID_DonViQuiDoi, 
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	--Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then  CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	--Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
			--
    		MAX(lh.ID) as ID_LoHang,
			Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
			--
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDau,0))  AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS GiaTriNhap,
    			NULL AS SoLuongXuat,
    			NULL AS GiaTriXuat,
    			ISNULL(td.TonKho, 0) AS TonKho
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				NULL AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
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
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                    
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat,
    				NULL AS TonKho
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhToan,0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
			--
			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
			--
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
    		where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonII]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
		MAX(tr.SoLuongNhap) as SoLuongNhap,
		MAX(tr.GiaTriNhap) as GiaTriNhap,
		MAX(tr.SoLuongXuat) as SoLuongXuat,
		MAX(tr.GiaTriXuat) as GiaTriXuat,
		tr.TonCuoiKy,
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	
    	SELECT 
		dvqd3.ID as ID_DonViQuiDoi, 
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
			Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDauKy,0))  AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongXuat1,0)) - SUM(ISNULL(td.SoLuongNhap1,0)) AS TonDauKy,
    			SUM(ISNULL(td.SoLuongNhap1, 0) + ISNULL(td.SoLuongNhap2, 0)) AS SoLuongNhap,
    			SUM(ISNULL(td.GiaTriNhap1, 0) + ISNULL(td.GiaTriNhap2, 0)) AS GiaTriNhap,
    			SUM(ISNULL(td.SoLuongXuat1, 0) + ISNULL(td.SoLuongXuat2, 0)) AS SoLuongXuat,
    			SUM(ISNULL(td.GiaTriXuat1, 0) + ISNULL(td.GiaTriXuat2, 0)) AS GiaTriXuat,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap2,0)) - SUM(ISNULL(td.SoLuongXuat2,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
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
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap1,
    				null AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.ThanhToan,0)) AS GiaTriNhap1,
    				null AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap1,
    				null AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat2,
    				NULL AS TonKho
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
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap2,
    				null AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
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
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.ThanhToan,0)) AS GiaTriNhap2,
    				null AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap2,
    				null AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				NULL AS TonKho
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
    where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				 and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonIII]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
		MAX(tr.SoLuongNhap) as SoLuongNhap,
		MAX(tr.GiaTriNhap) as GiaTriNhap,
		MAX(tr.SoLuongXuat) as SoLuongXuat,
		MAX(tr.GiaTriXuat) as GiaTriXuat,
		tr.TonCuoiKy,
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT
		dvqd3.ID as ID_DonViQuiDoi,
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy,     	
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    			MAX(lh.ID) as ID_LoHang,
			Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0) + ISNULL(HangHoa.SoLuongXuat,0) - ISNULL(HangHoa.SoLuongNhap,0))  AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			NULL AS SoLuongNhap,
    			NULL AS GiaTriNhap,
    			NULL AS SoLuongXuat,
    			NULL AS GiaTriXuat,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongXuat,0)) - SUM(ISNULL(td.SoLuongNhap,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				NULL AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
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
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				--NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat,
    				NULL AS TonCuoiKy
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhToan,0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
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
     where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				 and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonIV]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    --DECLARE @timeChotSo Datetime
    --   Select @timeChotSo =  (select convert(datetime, '2018/01/01'))
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * gv.GiaVon, 0) as float) else 0 end as GiaTriDauKy,
		MAX(tr.SoLuongNhap) as SoLuongNhap,
		MAX(tr.GiaTriNhap) as GiaTriNhap,
		MAX(tr.SoLuongXuat) as SoLuongXuat,
		MAX(tr.GiaTriXuat) as GiaTriXuat,
		tr.TonCuoiKy,
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT 
		dvqd3.ID as ID_DonViQuiDoi,
		a.ID_LoHang,
		a.TenNhomHangHoa,
		dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    			MAX(lh.ID) as ID_LoHang,
			Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    	    -- dhh.LaHangHoa,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDau,0))  AS TonDauKy,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS GiaTriNhap,
    			NULL AS SoLuongXuat,
    			NULL AS GiaTriXuat
    			FROM
    			(
    
    				-- phát sinh xuất nhập tồn đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
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
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhToan,0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
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
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
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
  where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
				) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TongHopHangNhap]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                LoaiChungTu = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	SELECT 
				c.TenNhomHangHoa as TenNhomHang,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
    			c.SoLuongNhap as SoLuong, 
    			Case When @XemGiaVon = '1' then  c.GiaTriNhap else 0 end as ThanhTien
	FROM
	(
    SELECT 
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		Case when a.ID_NhomHang is null then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
			Case when a.ID_NhomHang is null then N'nhom mac dinh' else a.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
			Case when a.ID_NhomHang is null then N'nmd' else a.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
		a.TenHangHoa_KhongDau,
		a.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
			MAX(dnhh.TenNhomHangHoa_KhongDau) AS TenNhomHangHoa_KhongDau,
			MAX(dnhh.TenNhomHangHoa_KyTuDau) AS TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap
    		FROM
    		(
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				pstk.LoaiHoaDon,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhToan,0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
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
    where LaDonViChuan = 1
    	and a.Xoa like @TrangThai
	) c
			 where c.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    	ORDER BY ThanhTien DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TongHopHangXuat]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                LoaiChungTu = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	SELECT 
				c.TenNhomHangHoa as TenNhomHang,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
    			c.SoLuongXuat as SoLuong, 
    			Case When @XemGiaVon = '1' then  c.GiaTriXuat else 0 end as ThanhTien
	FROM
	(
    SELECT 
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		Case when a.ID_NhomHang is null then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
			Case when a.ID_NhomHang is null then N'nhom mac dinh' else a.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
			Case when a.ID_NhomHang is null then N'nmd' else a.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
		a.TenHangHoa_KhongDau,
		a.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat
    	FROM 
    	--select * FROM
    		(
    			SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
			MAX(dnhh.TenNhomHangHoa_KhongDau) AS TenNhomHangHoa_KhongDau,
			MAX(dnhh.TenNhomHangHoa_KyTuDau) AS TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat
    		FROM
    		(
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    					pstk.LoaiHoaDon,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
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
    where LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    ) c
			 where c.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    	ORDER BY ThanhTien DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TonKhoI]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT
    		dvqd3.ID as ID_DonViQuiDoi,
			a.ID_LoHang,
			a.TenNhomHangHoa,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end  + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
			Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
				Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang As ID_LoHang,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			ISNULL(td.TonKho, 0) AS TonKho
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
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				WHERE dvqd.ladonvichuan = '1' 
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM 
    				(
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
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
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
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
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
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
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
    		where  a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    		order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TonKhoII]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT 		
    		dvqd3.ID as ID_DonViQuiDoi,
			a.ID_LoHang,
			a.TenNhomHangHoa,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    			MAX(lh.ID) as ID_LoHang,
    		Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang As ID_LoHang,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap2,0)) - SUM(ISNULL(td.SoLuongXuat2,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    						SUM(ISNULL(cs.TonKho, 0)) as TonKho
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
    				NULL AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				NULL AS TonKho
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
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
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
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
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
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
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
    where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
     ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TonKhoIII]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    	SELECT 
    		dvqd3.ID as ID_DonViQuiDoi,
			a.ID_LoHang,
			a.TenNhomHangHoa,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    			MAX(lh.ID) as ID_LoHang,
    		Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			--NULL as TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongXuat,0)) - SUM(ISNULL(td.SoLuongNhap,0)) AS TonCuoiKy
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
    					WHERE dvqd.ladonvichuan = '1' 
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
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
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
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
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi,td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				--NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				NULL AS TonCuoiKy
    				FROM 
    				(
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
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where  a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    		and a.Xoa like @TrangThai
   ) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    		order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TonKhoIV]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
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
		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * gv.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
	FROM
	(
    SELECT 
		dvqd3.ID as ID_DonViQuiDoi,
		a.ID_LoHang,
		a.TenNhomHangHoa,
    	dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat
    			FROM
    			(
    
    				-- phát sinh xuất nhập tồn đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
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
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
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
    			where a.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
			) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
			Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy
    		order by GiaTriCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_XuatChuyenHang]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	SELECT 
				c.TenNhomHangHoa as TenNhomHang,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
    		CAST(ROUND(c.SoLuong, 3) as float) as SoLuong, 
    		Case When @XemGiaVon = '1' then  CAST(ROUND(c.GiaTri, 0) as float) else 0 end as ThanhTien
	FROM
	(
		SELECT
    	dvqd.ID,
		a.ID_LoHang,
		Case when hh.ID_NhomHang is not null then hh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when dnhh.TenNhomHangHoa is null then N'Nhóm mặc định' else dnhh.TenNhomHangHoa end as TenNhomHangHoa,
				Case when dnhh.TenNhomHangHoa_KhongDau is null then N'nhom mac dinh' else dnhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
				Case when dnhh.TenNhomHangHoa_KyTuDau is null then N'nmd' else dnhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
		hh.TenHangHoa_KhongDau,
		hh.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND(a.SoLuong , 3) as float ) as SoLuong,
    	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
    	FROM
    	(
    			SELECT
    				b.ID_DonViQuiDoi as ID_DonViQuiDoi,
					b.ID_LoHang,
    				SUM(ISNULL(b.SoLuong, 0)) as SoLuong,
    				SUM(ISNULL(b.GiaTri, 0)) as GiaTri
    			FROM
    			(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				ISNULL(hdct.SoLuong,0) as SoLuong,
    				ISNULL(hdct.ThanhTien, 0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.LoaiHoaDon = '10' and hd.YeuCau = '1')
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    
    			Union ALL
    				SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				ISNULL(hdct.tienchietkhau,0) as SoLuong,
    				ISNULL(hdct.ThanhTien, 0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.ID_CheckIn is not null 
    				and hd.LoaiHoaDon = '10' and hd.YeuCau = '4')
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			) b
    			GROUP BY b.ID_DonViQuiDoi, ID_LoHang
    	) a
    	inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
				LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
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
						where hh.LaHangHoa like  @LaHangHoa
						and hh.TheoDoi like @TheoDoi
		) c
			 where c.ID_NhomHang like @ID_NhomHang
				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    			and c.Xoa like @TrangThai
    	ORDER BY ThanhTien DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_XuatChuyenHangChiTiet]", parametersAction: p => new {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

	SELECT 
				c.MaHoaDon as MaHoaDon,
				c.NgayLapHoaDon as NgayLapHoaDon,
    			c.MaHangHoa,
    			c.TenHangHoaFull,
    			c.TenHangHoa,
    			c.ThuocTinh_GiaTri,
    			c.TenDonViTinh,
    			c.TenLoHang,
				c.ChiNhanhChuyen,
				c.ChiNhanhNhan,
    		CAST(ROUND(c.SoLuong, 3) as float) as SoLuong, 
			Case When @XemGiaVon = '1' then  CAST(ROUND(c.DonGia, 0) as float) else 0 end as DonGia,
    		Case When @XemGiaVon = '1' then  CAST(ROUND(c.ThanhTien, 0) as float) else 0 end as ThanhTien
	FROM
	(
    Select
		Case when hh.ID_NhomHang is not null then hh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
			Case when dnhh.TenNhomHangHoa is null then N'Nhóm mặc định' else dnhh.TenNhomHangHoa end as TenNhomHangHoa,
				Case when dnhh.TenNhomHangHoa_KhongDau is null then N'nhom mac dinh' else dnhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
				Case when dnhh.TenNhomHangHoa_KyTuDau is null then N'nmd' else dnhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    	bhhd.MaHoaDon,
    	bhhd.NgayLapHoaDon,
    		dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
		hh.TenHangHoa_KhongDau,
		hh.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		dvn.TenDonVi as ChiNhanhChuyen,
    	dv.TenDonVi as ChiNhanhNhan,
    	CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
    	CAST(ROUND(ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0), 0) as float) as DonGia,
    	CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float) as ThanhTien
    	From
    	BH_HoaDon bhhd 
    	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
    	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
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
    	where hh.TheoDoi like @TheoDoi
		and hh.LaHangHoa like @LaHangHoa
    	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
    		and bhhd.ChoThanhToan = 0
    	and ((bhhd.loaihoadon = '10' and bhhd.YeuCau = '1' and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))) or
    	(bhhd.ID_CheckIn is not null 
    		and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4' 
    		and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))))
		) c
		where c.ID_NhomHang like @ID_NhomHang
		and (MaHoaDon like @MaHH_TV or MaHoaDon like @MaHH or MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    	and c.Xoa like @TrangThai
    	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhapHang_ChiTiet]", parametersAction: p => new {
                Text_Search = p.String(),
                MaHH = p.String(),
                MaNCC = p.String(),
                MaNCC_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhomNCC = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    SELECT 
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
    			--a.ID_NhomDoiTuong,
    			a.MaNhaCungCap,
    			a.TenNhaCungCap,
    			a.MaHangHoa,
    			a.TenHangHoaFull,
    			a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    			a.TenDonViTinh,
    				a.TenLoHang,
    		CAST(ROUND((a.SoLuong), 3) as float) as SoLuong, 
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.GiaBan), 0) as float) else 0 end  as DonGia,
    		Case When @XemGiaVon = '1' then	CAST(ROUND((a.TienChietKhau), 0) as float) else 0 end  as TienChietKhau,
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.ThanhTien), 0) as float) else 0 end  as ThanhTien,
    		Case When @XemGiaVon = '1' then	CAST(ROUND((a.GiamGiaHD), 0) as float) else 0 end  as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.ThanhTien - a.GiamGiaHD), 0) as float) else 0 end as GiaTriNhap, 
    		a.TenNhanVien
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    			Case When dt.ID is null then '20000000-0000-0000-0000-000000000002' else dt.ID end as ID_NhaCungCap,
    			Case When dtn.ID_NhomDoiTuong is null then '30000000-0000-0000-0000-000000000003' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			Case when dt.ID is null then N'NCC Lẻ' else dt.MaDoiTuong end as MaNhaCungCap,
    			Case when dt.ID is null then N'Nhà cung cấp lẻ' else dt.TenDoiTuong end as TenNhaCungCap,
    			Case when dt.ID is null then N'nha cung cap le' else dt.TenDoiTuong_KhongDau end as TenNhaCungCap_KhongDau,
    			Case when dt.ID is null then N'nccl' else dt.TenDoiTuong_ChuCaiDau end as TenNhaCungCap_KyTuDau,
    			dt.DienThoai,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    				--hh.TenHangHoa_KhongDau,
    				--hh.TenHangHoa_KyTuDau,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    			ISNULL(hdct.DonGia, 0) as GiaBan,
    			ISNULL(hdct.TienChietKhau, 0) as TienChietKhau,
    		nv.TenNhanVien,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    			--hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon = 4
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (hd.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and a.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomNCC))
    		and (a.MaNhaCungCap like @MaNCC_TV or a.TenNhaCungCap_KhongDau like @MaNCC or a.TenNhaCungCap_KyTuDau like @MaNCC or a.DienThoai = @MaNCC_TV)
    	order by a.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhapHang_NhomHang]", parametersAction: p => new {
                TenNhomHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuongNhap, 
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien)), 0) as float) else 0 end as ThanhTien,
    		Case When @XemGiaVon = '1' then  CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) else 0 end as GiamGiaHD,
    		Case When @XemGiaVon = '1' then  CAST(ROUND((Sum(a.ThanhTien - a.GiamGiaHD)), 0) as float) else 0 end as GiaTriNhap
    	FROM
    	(
    		Select 
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    			Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    			Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD
    		--hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) as GiamGiaHD
    			
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon = 4
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.TenNhomHangHoa_KhongDau like @TenNhomHang or a.TenNhomHangHoa_KyTuDau like @TenNhomHang)
    	Group by a.ID_NhomHang
    		OrDER BY GiaTriNhap DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhapHang_TheoNhaCungCap]", parametersAction: p => new {
                MaNCC = p.String(),
                MaNCC_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomNhaCungCap = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    SELECT b.NhomKhachHang,
    	b.MaNhaCungCap,
    	b.TenNhaCungCap,
    	b.DienThoai,
    	b.SoLuongNhap, b.ThanhTien, b.GiamGiaHD, b.GiaTriNhap
    	 FROM
    	(
    		SELECT
    				Case When DoiTuong_Nhom.ID_NhomDoiTuong is null then '30000000-0000-0000-0000-000000000003' else DoiTuong_Nhom.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    				Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomKhachHang,
    			case when dt.MaDoiTuong is null then N'NCC lẻ' else dt.MaDoiTuong end AS MaNhaCungCap,
    			case when dt.TenDoiTuong is null then N'Nhà cung cấp lẻ' else dt.TenDoiTuong end AS TenNhaCungCap,
    			case when dt.TenDoiTuong_KhongDau is null then N'nha cung cap le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
    			case when dt.TenDoiTuong_ChuCaiDau is null then N'nccl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
    			Case when dt.DienThoai is null then '' else dt.DienThoai end AS DienThoai,
    			a.SoLuongNhap,
    				a.ThanhTien,    				
    				a.GiamGiaHD,
    				a.GiaTriNhap
    		FROM
    		(
    			SELECT
    				NCC.ID_NhaCungCap,
    				CAST(ROUND((SUM(NCC.SoLuong)), 3) as float) as SoLuongNhap, 
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.ThanhTien)), 0) as float) else 0 end as ThanhTien,
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.GiamGiaHD)), 0) as float) else 0 end as GiamGiaHD,
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.ThanhTien - NCC.GiamGiaHD)), 0) as float) else 0 end as GiaTriNhap
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_NhaCungCap,
    				ISNULL(hdct.SoLuong, 0) AS SoLuong,
    					ISNULL(hdct.ThanhTien, 0) AS ThanhTien,
						Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    					--hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) as GiamGiaHD,
    					Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    					Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    					inner join DonViQuiDoi dvqd on dvqd.ID = hdct.ID_DonViQuiDoi
    					inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 4
    					and hh.LaHangHoa like @LaHangHoa
    					and hh.TheoDoi like @TheoDoi
    			) AS NCC
    				Where NCC.Xoa like @TrangThai
    				and NCC.ID_NhomHang like @ID_NhomHang
    			Group by NCC.ID_NhaCungCap
    		) a
    		left join DM_DoiTuong dt on a.ID_NhaCungCap = dt.ID
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
    	) b
    	where ID_NhomDoiTuong in (select * from splitstring(@ID_NhomNhaCungCap))
    		and (MaNhaCungCap like @MaNCC_TV or MaNhaCungCap like @MaNCC or TenDoiTuong_KhongDau like @MaNCC or TenDoiTuong_ChuCaiDau like @MaNCC or DienThoai like @MaNCC)
    	ORDER BY GiaTriNhap DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhapHang_TongHop]", parametersAction: p => new {
                Text_Search = p.String(),
                MaHH = p.String(),
                TenNhomHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
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
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien)), 0) as float) else 0 end as ThanhTien,
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) else 0 end as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien - a.GiamGiaHD)), 0) as float) else 0 end as GiaTriNhap
    	FROM
    	(
    		Select 
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    			Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    			Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    				hh.TenHangHoa_KhongDau,
    				hh.TenHangHoa_KyTuDau,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    					Case When hdct.ID_LoHang is not null then hdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    			ISNULL(hdct.SoLuong, 0) as SoLuong,
    		ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0) as GiaBan,
    		Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon, 0) else 0 end as GiaVon, 
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon = 4
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.MaHangHoa like @Text_Search or a.TenHangHoa_KhongDau like @Text_Search or a.TenHangHoa_KyTuDau like @Text_Search or a.TenNhomHangHoa_KhongDau like @TenNhomHang or a.TenNhomHangHoa_KyTuDau like @TenNhomHang or a.MaHangHoa like @MaHH)
    	Group by a.MaHangHoa, a.TenLoHang, a.ID_LoHang
    		OrDER BY GiaTriNhap DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhapHang_TraHangNhap]", parametersAction: p => new {
                MaHH = p.String(),
                MaHD = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	
    SELECT 
    	a.MaChungTuGoc, 
    		a.MaChungTu, 
    		a.NgayLapHoaDon,
    		a.MaHangHoa,
    		a.TenHangHoaFull,
    		a.TenHangHoa,
    		a.TenDonViTinh,
    		a.ThuocTinh_GiaTri,
    		a.TenLoHang,
    		CAST(ROUND(a.SoLuongTra, 0) as float) as SoLuong,
    		Case When @XemGiaVon = '1' then CAST(ROUND(a.ThanhTien, 0) as float) else 0 end as ThanhTien,
    			Case When @XemGiaVon = '1' then CAST(ROUND(a.GiamGiaHD, 0) as float) else 0 end as GiamGiaHD,
    			Case When @XemGiaVon = '1' then CAST(ROUND(a.ThanhTien -a.GiamGiaHD, 0) as float) else 0 end as GiaTriTra,
    		a.TenNhanVien
    	FROM
    	(
    		SELECT
    				Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    				Case when hdb.ID is null then N'HD trả nhanh' else hdb.MaHoaDon end as MaChungTuGoc,
    				hdt.MaHoaDon as MaChungTu,
    				hdt.NgayLapHoaDon,
    				dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    				hh.TenHangHoa_KhongDau,
    				hh.TenHangHoa_KyTuDau,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    				ISNULL(hdct.SoLuong, 0) as SoLuongTra,
					Case when hdt.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang, 0)) end as GiamGiaHD,
    				--ISNULL(hdct.ThanhTien, 0) *(ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang, 0))as GiamGiaHD,
    				ISNULL(hdct.ThanhTien, 0) as ThanhTien,
    				nv.TenNhanVien
    		FROM
    			BH_HoaDon hdt 
    		left join BH_HoaDon hdb on hdt.ID_HoaDon = hdb.ID
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    			left join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join NS_NhanVien nv on hdt.ID_NhanVien = nv.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
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
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    			and hdt.ChoThanhToan = 0
    		and hdt.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdt.LoaiHoaDon = 7
    			and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.MaHangHoa like @MaHD or a.MaHangHoa like @MaHH or a.TenHangHoa_KhongDau like @MaHH or a.TenHangHoa_KyTuDau like @MaHH or a.MaChungTu like @MaHD)
    		ORDER BY a.NgayLapHoaDon DESC");

            CreateStoredProcedure("[dbo].[getList_NhomDoiTuong]", parametersAction: p => new {
                LoaiDoiTuong = p.Int()
            }, body: @"SELECT 
	a.ID, 
	MAX(TenNhomDoiTuong) as TenNhomDoiTuong,
	MAX(TenNhomDoiTuong_KhongDau) as TenNhomDoiTuong_KhongDau,
	MAX(TenNhomDoiTuong_KyTuDau) as TenNhomDoiTuong_KyTuDau
	FROM
	(
	select 
	Case when dtn.ID is null then '30000000-0000-0000-0000-000000000003' else dtn.ID_NhomDoiTuong end as ID,
	Case when dtn.ID is null then N'Nhóm mặc định' else ndt.TenNhomDoiTuong end as TenNhomDoiTuong,
	Case when dtn.ID is null then N'nhom mac dinh' else ndt.TenNhomDoiTuong_KhongDau end as TenNhomDoiTuong_KhongDau,
	Case when dtn.ID is null then N'nmd' else ndt.TenNhomDoiTuong_KyTuDau end as TenNhomDoiTuong_KyTuDau
	from DM_DoiTuong dt
	left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
	left join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
	where dt.LoaiDoiTuong = @LoaiDoiTuong)a
	Group by a.ID");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoKho_ChiTietHangNhap]");
            DropStoredProcedure("[dbo].[BaoCaoKho_ChiTietHangXuat]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapChuyenHang]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapChuyenHangChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonChiTietI]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonChiTietII]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonChiTietIII]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonChiTietIV]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonI]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonII]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonIII]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonIV]");
            DropStoredProcedure("[dbo].[BaoCaoKho_TongHopHangNhap]");
            DropStoredProcedure("[dbo].[BaoCaoKho_TongHopHangXuat]");
            DropStoredProcedure("[dbo].[BaoCaoKho_TonKhoI]");
            DropStoredProcedure("[dbo].[BaoCaoKho_TonKhoII]");
            DropStoredProcedure("[dbo].[BaoCaoKho_TonKhoIII]");
            DropStoredProcedure("[dbo].[BaoCaoKho_TonKhoIV]");
            DropStoredProcedure("[dbo].[BaoCaoKho_XuatChuyenHang]");
            DropStoredProcedure("[dbo].[BaoCaoKho_XuatChuyenHangChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoNhapHang_ChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoNhapHang_NhomHang]");
            DropStoredProcedure("[dbo].[BaoCaoNhapHang_TheoNhaCungCap]");
            DropStoredProcedure("[dbo].[BaoCaoNhapHang_TongHop]");
            DropStoredProcedure("[dbo].[BaoCaoNhapHang_TraHangNhap]");
            DropStoredProcedure("[dbo].[getList_NhomDoiTuong]");
        }
    }
}
