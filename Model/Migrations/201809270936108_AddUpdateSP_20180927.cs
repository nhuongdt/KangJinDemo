namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180927 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHangChiTiet_TheoKhachHang]", parametersAction: p => new
            {
                ID_KhachHang = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	SELECT 
    		MAX(b.MaHangHoa) as MaHangHoa,
    		MAX(b.TenHangHoaFull) as TenHangHoaFull,
    		MAX(b.TenHangHoa) as TenHangHoa,
    		MAX(b.TenDonViTinh) as TenDonViTinh,
    		MAX (b.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(b.TenLoHang) as TenLoHang,
    	SUM(b.SoLuongBan) as SoLuongBan,
    	SUM(b.GiaTriBan) as ThanhTien,
    	Case When @XemGiaVon = '1' then SUM(b.TongGiaVon) else 0 end as TienVon,
		SUM(b.GiamGiaHD) as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND(SUM(b.GiaTriBan) - SUM(b.GiamGiaHD) - SUM(b.TongGiaVon), 0) as float) else 0 end as LaiLo    	
    	FROM 
    	(
    SELECT 
    		a.ID_DonViQuiDoi,
    		dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when a.MaLoHang is null then '' else '. Lô: ' + a.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when a.MaLoHang is null then '' else '. Lô: ' + a.MaLoHang end as TenLoHang,
    					Case When a.ID_LoHang is not null then a.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    		Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		CAST(ROUND(SUM(ISNULL(a.GiamGiaHD, 0)), 0) as float) as GiamGiaHD,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongBan, 0)), 3) as float) as SoLuongBan,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriBan, 0)), 0) as float) as GiaTriBan,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongBan, 0) * ISNULL(a.GiaVon * dvqd.TyLeChuyenDoi, 0)), 0) as float) as TongGiaVon    	
    	FROM
    	(
    		SELECT
					Case when hdb.ID_DoiTuong is null then '00000000-4000-0000-0004-000000000000' else hdb.ID_DoiTuong end as ID_KhachHang,
    				lh.MaLoHang,
    				lh.ID as ID_LoHang,
    				hdb.ID as ID_HoaDon,
    				hdct.ID_DonViQuiDoi,
					Case when hdb.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdb.TongGiamGia, 0) / ISNULL(hdb.TongTienHang, 0)) end as GiamGiaHD,
    				ISNULL(hdct.SoLuong, 0) as SoLuongBan,
    				ISNULL(hdct.ThanhTien, 0) as GiaTriBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVon
    		FROM BH_HoaDon hdb 
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon 
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    		and (hdb.ID_DoiTuong = @ID_KhachHang or hdb.ID_DoiTuong is null)
    	) a
    		inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    		where hh.LaHangHoa like @LaHangHoa
			and a.ID_KhachHang = @ID_KhachHang
    		and hh.TheoDoi like @TheoDoi
    	GROUP BY a.ID_DonViQuiDoi, a.ID_HoaDon, hh.ID_NhomHang, dvqd.Xoa, a.MaLoHang, dvqd.MaHangHoa, hh.TenHangHoa, ThuocTinh.ThuocTinh_GiaTri, dvqd.TenDonViTinh, a.ID_LoHang
    		) b
    		where (b.ID_NhomHang like @ID_NhomHang or b.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and b.Xoa like @TrangThai
    		GROUP BY b.ID_DonViQuiDoi, b.ID_LoHang");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_CongNoI]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
		SELECT
    		Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomDoiTac,
    		c.MaDoiTac,
			c.TenDoiTac,
			c.PhaiThuDauKy,
			c.PhaiTraDauKy,
			c.TongTienChi,
			c.TongTienThu,
			c.PhaiThuCuoiKy,
			c.PhaiTraCuoiKy
    	  FROM 
    	(
		 SELECT 
			b.ID_KhachHang,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
			MAX(b.TongTienThu) as TongTienThu,
			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
		  a.NoDauKy,
		  a.GhiNo As TongTienChi,
		  a.GhiCo As TongTienThu,
		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
		  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(ISNULL(HangHoa.NoDauKy, 0)) as NoDauKy, 
    		SUM(ISNULL(HangHoa.GhiNo, 0)) as GhiNo,
    		SUM(ISNULL(HangHoa.GhiCo, 0)) as GhiCo
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(td.CongNo, 0)) + SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoDauKy,
    				NULL AS GhiNo,
    				NULL AS GhiCo
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				ISNULL(CongNo, 0) AS CongNo,
    				NULL AS GiaTriTra,
    				NULL AS DoanhThu,
    				NULL AS TienThu,
    				NULL AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    				UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    				SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			NULL AS NoDauKy,
    				SUM(ISNULL(pstv.DoanhThu,0)) + SUM(ISNULL(pstv.TienChi,0)) AS GhiNo,
    				SUM(ISNULL(pstv.TienThu,0)) + SUM(ISNULL(pstv.GiaTriTra,0)) AS GhiCo
    			FROM
    			(
    			SELECT 
    			bhd.ID_DoiTuong,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    		)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
				) b
				LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    			where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) 
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		) as c
    		left join DM_DoiTuong dt on c.ID_KhachHang = dt.ID
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
								ORDER BY MaDoiTac DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_CongNoII]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LoaiKH = p.String(),
                ID_NhomDoiTuong = p.String()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
  SELECT
    		Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomDoiTac,
    		c.MaDoiTac,
			c.TenDoiTac,
			c.PhaiThuDauKy,
			c.PhaiTraDauKy,
			c.TongTienChi,
			c.TongTienThu,
			c.PhaiThuCuoiKy,
			c.PhaiTraCuoiKy
    	  FROM 
    	(
		 SELECT 
			b.ID_KhachHang,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
			MAX(b.TongTienThu) as TongTienThu,
			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
		  a.NoDauKy,
		  a.GhiNo As TongTienChi,
		  a.GhiCo As TongTienThu,
		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
		  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(ISNULL(HangHoa.NoDauKy, 0)) as NoDauKy, 
    			SUM(ISNULL(HangHoa.GhiNo, 0)) as GhiNo,
    			SUM(ISNULL(HangHoa.GhiCo, 0)) as GhiCo,
    			SUM(ISNULL(HangHoa.NoCuoiKy, 0)) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(td.CongNo, 0)) - SUM(ISNULL(td.DoanhThu,0)) - SUM(ISNULL(td.TienChi,0)) + SUM(ISNULL(td.TienThu,0)) + SUM(ISNULL(td.GiaTriTra,0)) AS NoDauKy,
    				SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) AS GhiNo,
    				SUM(ISNULL(td.TienThu,0)) + SUM(ISNULL(td.GiaTriTra,0)) AS GhiCo,
    				NULL AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				ISNULL(CongNo, 0) AS CongNo,
    				NULL AS GiaTriTra,
    				NULL AS DoanhThu,
    				NULL AS TienThu,
    				NULL AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    				UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    				SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			NULL AS NoDauKy,
    				SUM(ISNULL(pstv.DoanhThu,0)) + SUM(ISNULL(pstv.TienChi,0)) AS GhiNo,
    				SUM(ISNULL(pstv.TienThu,0)) + SUM(ISNULL(pstv.GiaTriTra,0)) AS GhiCo,
    			    SUM(ISNULL(pstv.CongNo, 0)) + SUM(ISNULL(pstv.DoanhThu,0)) + SUM(ISNULL(pstv.TienChi,0)) - SUM(ISNULL(pstv.TienThu,0)) - SUM(ISNULL(pstv.GiaTriTra,0)) AS NoCuoiKy
    			FROM
    			(
    				-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				ISNULL(CongNo, 0) AS CongNo,
    				NULL AS GiaTriTra,
    				NULL AS DoanhThu,
    				NULL AS TienThu,
    				NULL AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    
    			SELECT 
    			bhd.ID_DoiTuong,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    			where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) 
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		) as c
    		left join DM_DoiTuong dt on c.ID_KhachHang = dt.ID
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
								ORDER BY MaDoiTac DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_CongNoIII]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LoaiKH = p.String(),
                ID_NhomDoiTuong = p.String()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
		SELECT
    		Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomDoiTac,
    		c.MaDoiTac,
			c.TenDoiTac,
			c.PhaiThuDauKy,
			c.PhaiTraDauKy,
			c.TongTienChi,
			c.TongTienThu,
			c.PhaiThuCuoiKy,
			c.PhaiTraCuoiKy
    	  FROM 
    	(
		 SELECT 
			b.ID_KhachHang,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
			MAX(b.TongTienThu) as TongTienThu,
			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
		  a.NoDauKy,
		  a.GhiNo As TongTienChi,
		  a.GhiCo As TongTienThu,
		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
		  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(ISNULL(HangHoa.NoDauKy, 0)) as NoDauKy, 
    			SUM(ISNULL(HangHoa.GhiNo, 0)) as GhiNo,
    			SUM(ISNULL(HangHoa.GhiCo, 0)) as GhiCo,
    			SUM(ISNULL(HangHoa.NoCuoiKy, 0)) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			NULL AS NoDauKy,
    				NULL AS GhiNo,
    				NULL AS GhiCo,
    				SUM(ISNULL(td.CongNo, 0)) - SUM(ISNULL(td.DoanhThu,0)) - SUM(ISNULL(td.TienChi,0)) + SUM(ISNULL(td.TienThu,0)) + SUM(ISNULL(td.GiaTriTra,0)) AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				ISNULL(CongNo, 0) AS CongNo,
    				NULL AS GiaTriTra,
    				NULL AS DoanhThu,
    				NULL AS TienThu,
    				NULL AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    				UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    				SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			NULL AS NoDauKy,
    				SUM(ISNULL(pstv.DoanhThu,0)) + SUM(ISNULL(pstv.TienChi,0)) AS GhiNo,
    				SUM(ISNULL(pstv.TienThu,0)) + SUM(ISNULL(pstv.GiaTriTra,0)) AS GhiCo,
    			    NULL AS NoCuoiKy
    			FROM
    			(
    			SELECT 
    			bhd.ID_DoiTuong,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    			where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) 
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		) as c
    		left join DM_DoiTuong dt on c.ID_KhachHang = dt.ID
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
								ORDER BY MaDoiTac DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_CongNoIV]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LoaiKH = p.String(),
                ID_NhomDoiTuong = p.String()
            }, body: @"SELECT
    		Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomDoiTac,
    		c.MaDoiTac,
			c.TenDoiTac,
			c.PhaiThuDauKy,
			c.PhaiTraDauKy,
			c.TongTienChi,
			c.TongTienThu,
			c.PhaiThuCuoiKy,
			c.PhaiTraCuoiKy
    	  FROM 
    	(
		 SELECT 
			b.ID_KhachHang,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
			MAX(b.TongTienThu) as TongTienThu,
			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
		  a.NoDauKy,
		  a.GhiNo As TongTienChi,
		  a.GhiCo As TongTienThu,
		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
		  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(ISNULL(HangHoa.NoDauKy, 0)) as NoDauKy, 
    			SUM(ISNULL(HangHoa.GhiNo, 0)) as GhiNo,
    			SUM(ISNULL(HangHoa.GhiCo, 0)) as GhiCo,
    			SUM(ISNULL(HangHoa.NoDauKy, 0) + ISNULL(HangHoa.GhiNo, 0) - ISNULL(HangHoa.GhiCo, 0)) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoDauKy,
    				NULL AS GhiNo,
    				NULL AS GhiCo,
    				NULL AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				ISNULL(CongNo, 0) AS CongNo,
    				NULL AS GiaTriTra,
    				NULL AS DoanhThu,
    				NULL AS TienThu,
    				NULL AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				NULL AS CongNo,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    				UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    				SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			NULL AS NoDauKy,
    				SUM(ISNULL(pstv.DoanhThu,0)) + SUM(ISNULL(pstv.TienChi,0)) AS GhiNo,
    				SUM(ISNULL(pstv.TienThu,0)) + SUM(ISNULL(pstv.GiaTriTra,0)) AS GhiCo,
    			    NULL AS NoCuoiKy
    			FROM
    			(
    			SELECT 
    			bhd.ID_DoiTuong,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    			where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) 
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		) as c
    		left join DM_DoiTuong dt on c.ID_KhachHang = dt.ID
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
								ORDER BY MaDoiTac DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_SoQuy]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.String(),
                LoaiTien = p.String()
            }, body: @"--	tinh ton dau ky
	Declare @TonDauKy float
	Set @TonDauKy = (Select
	CAST(ROUND(SUM(TienThu), 0) as float) as TonDauKy
	FROM
	(
	select 
			Case when qhd.LoaiHoaDon = 11 then ISNULL(qhdct.TienThu,0) else ISNULL(qhdct.TienThu,0) *(-1) end as TienThu,
			Case when qhdct.TienMat > 0 and qhdct.TienGui = 0 then '1' else 
    			Case when qhdct.TienGui > 0 and qhdct.TienMat = 0 then '2' else
    			Case when qhdct.TienGui > 0 and qhdct.TienMat > 0 then '12' else '' end end end as LoaiTien			
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
   -- 		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
   -- 		left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
			--left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
			--left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
			--left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon < @timeStart
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and qhdct.DiemThanhToan is null
			) a 
			where LoaiTien like @LoaiTien
	)
	

	if (@TonDauKy is null)
	BeGin
		Set @TonDauKy = 0;
	END
	Declare @tmp table (MaPhieuThu nvarchar(max), NgayLapHoaDon datetime, KhoanMuc nvarchar(max), TenDoiTac nvarchar(max),
	TienMat float, TienGui float, TienThu float, TienChi float, ThuTienMat float, ChiTienMat float, ThuTienGui float, ChiTienGui float, TonLuyKeTienMat float,TonLuyKeTienGui float,TonLuyKe float, SoTaiKhoan nvarchar(max), NganHang nvarchar(max), GhiChu nvarchar(max))
	Insert INTO @tmp
	SELECT
    		c.MaPhieuThu, 
			c.NgayLapHoaDon, 
			c.NoiDungThuChi as KhoanMuc,
			c.TenDoiTac, 
			c.TienMat,
			c.TienGui,
			c.TienThu,
			c.TienChi,
			c.ThuTienMat,
			c.ChiTienMat,
			c.ThuTienGui,
			c.ChiTienGui,
			0 as TonLuyKe,
			0 as TonLuyKeTienMat,
			0 as TonLuyKeTienGui,
			c.SoTaiKhoan, 
			c.NganHang,
			c.GhiChu
    	  FROM 
    	(
		 SELECT 
			b.ID_DoiTuong,
    		MAX(b.MaHoaDon) as MaHoaDon,
    		MAX(b.MaPhieuThu) as MaPhieuThu,
    		MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    		MAX(b.TenNguoiNop) as TenDoiTac, 
			MAX (b.TienMat) as TienMat,
			MAX (b.TienGui) as TienGui,
			MAX (b.TienThu) as TienThu,
    		MAX (b.TienChi) as TienChi, 
			MAX (b.ThuTienGui) as ThuTienGui,
    		MAX (b.ChiTienGui) as ChiTienGui, 
			MAX (b.ThuTienMat) as ThuTienMat,
    		MAX (b.ChiTienMat) as ChiTienMat, 
			MAX(b.NoiDungThuChi) as NoiDungThuChi,
			MAX(b.SoTaiKhoan) as SoTaiKhoan,
			MAX(b.NganHang) as NganHang,
			MAX(b.GhiChu) as GhiChu,
			MAX(b.LoaiTien) as LoaiTien
    	 FROM
    	(
    select 
		a.HachToanKinhDoanh,
		a.ID_NhomDoiTuong,
		a.ID_DoiTuong,
		a.ID_HoaDon,
		a.MaHoaDon,
    	a.MaPhieuThu,
		a.NgayLapHoaDon,
    	a.TenNguoiNop,
		a.TienMat,
		a.TienGui,
		case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
		Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
		case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
		Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat,
		case when a.LoaiHoaDon = 11 then a.TienThu else 0 end as TienThu,
		Case when a.LoaiHoaDon = 12 then a.TienThu else 0 end as TienChi,
		a.NoiDungThuChi,
		a.NganHang,
		a.SoTaiKhoan,
		a.GhiChu,
		Case when a.TienMat > 0 and TienGui = 0 then '1' else 
    			Case when a.TienGui > 0 and TienMat = 0 then '2' else
    			Case when a.TienGui > 0 and TienMat > 0 then '12' else '' end end end as LoaiTien
    	--Case when a.LoaiThuChi in ('2', '4', '6') then a.Thuchi *(-1) else a.ThuChi end as ThuChi,
    	From
    	(
    		select 
				qhd.LoaiHoaDon,
				MAX(qhd.ID) as ID_HoaDon,
				MAX(dt.ID) as ID_DoiTuong,
				MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
				MAX (tknh.SoTaiKhoan) as SoTaiKhoan,
				MAX (nh.TenNganHang) as NganHang,
				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
				Case when (hd.LoaiHoaDon = 3 and qhd.LoaiHoaDon = 12) then 3 else -- trả tiền thừa
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
				Case When dtn.ID_NhomDoiTuong is null then
				Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
				Sum(ISNULL(qhdct.TienMat,0)) as TienMat,
				Sum(ISNULL(qhdct.TienGui,0)) as TienGui,
				Sum(ISNULL(qhdct.TienThu,0)) as TienThu,
    			qhd.NgayLapHoaDon,
				MAX(qhd.NoiDungThu) as GhiChu,
				hd.MaHoaDon
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    		left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and qhdct.DiemThanhToan is null
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, 
			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    	)a
    	where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    	and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
		and a.HachToanKinhDoanh like @HachToanKD
		) b
    			where ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) 
				and LoaiTien like @LoaiTien
    			Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon
    		) as c
    		left join DM_DoiTuong dt on c.ID_DoiTuong = dt.ID
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
    		ORDER BY NgayLapHoaDon
-- tính tồn lũy kế
			DECLARE @Ton float;
			SET @Ton = @TonDauKy;
			DECLARE @TonTienMat float;
			SET @TonTienMat = @TonDauKy;
			DECLARE @TonTienGui float;
			SET @TonTienGui = @TonDauKy;

			DECLARE @MaPhieuThu nvarchar(max);
			DECLARE @NgayLapHoaDon datetime;
			DECLARE @KhoanMuc nvarchar(max);
			DECLARE @TenDoiTac nvarchar(max);
			DECLARE @TienMat float;
			DECLARE @TenGui float;
			DECLARE @TienThu float;
			DECLARE @TienChi float;
			DECLARE @ThuTienMat float;
			DECLARE @ChiTienMat float;
			DECLARE @ThuTienGui float;
			DECLARE @ChiTienGui float;
			DECLARE @TonLuyKe float;
			DECLARE @TonLuyKeTienMat float;
			DECLARE @TonLuyKeTienGui float;
			DECLARE @SoTaiKhoan nvarchar(max);
			DECLARE @NganHang nvarchar(max);
			DECLARE @GhiChu nvarchar(max);
	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT * FROM @tmp ORDER BY NgayLapHoaDon
	OPEN CS_ItemUpDate 
    FETCH FIRST FROM CS_ItemUpDate INTO @MaPhieuThu,@NgayLapHoaDon,@KhoanMuc,@TenDoiTac, @TienMat,@TenGui, @TienThu,@TienChi,@ThuTienMat,@ChiTienMat,@ThuTienGui,@ChiTienGui, @TonLuyKe,@TonLuyKeTienMat, @TonLuyKeTienGui, @SoTaiKhoan,@NganHang,@GhiChu
    WHILE @@FETCH_STATUS = 0
    BEGIN
		SET @Ton = @Ton + @TienThu - @TienChi;
		SET @TonTienMat = @TonTienMat + @ThuTienMat - @ChiTienMat;
		SET @TonTienGui = @TonTienGui + @ThuTienGui - @ChiTienGui;
		UPDATE @tmp SET TonLuyKe = @Ton, TonLuyKeTienMat = @TonTienMat, TonLuyKeTienGui = @TonTienGui WHERE MaPhieuThu = @MaPhieuThu
	FETCH NEXT FROM CS_ItemUpDate INTO @MaPhieuThu,@NgayLapHoaDon,@KhoanMuc,@TenDoiTac, @TienMat,@TenGui, @TienThu,@TienChi,@ThuTienMat,@ChiTienMat,@ThuTienGui,@ChiTienGui,@TonLuyKe,@TonLuyKeTienMat, @TonLuyKeTienGui, @SoTaiKhoan,@NganHang,@GhiChu
    END
    CLOSE CS_ItemUpDate
    DEALLOCATE CS_ItemUpDate
	Select 
	MaPhieuThu,
	NgayLapHoaDon,
	KhoanMuc,
	TenDoiTac,
	@TonDauKy as TonDauKy,
	TienMat,
	TienGui,
	TienThu,
	TienChi,
	ThuTienMat,
	ChiTienMat,
	ThuTienGui,
	ChiTienGui,
	TonLuyKe,
	TonLuyKeTienMat,
	TonLuyKeTienGui,
	SoTaiKhoan, 
	NganHang, 
	GhiChu
	 from @tmp order by NgayLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_ThuChi]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.String()
            }, body: @"SELECT
    		Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomDoiTuong,
    		c.MaHoaDon, c.MaPhieuThu, c.NgayLapHoaDon, c.TenNguoiNop, 
			CAST(ROUND(c.ThuChi, 0) as float) as ThuChi,
			c.NoiDungThuChi, c.GhiChu, c.LoaiThuChi
    	  FROM 
    	(
		 SELECT 
			b.ID_DoiTuong,
    		MAX(b.MaHoaDon) as MaHoaDon,
    		MAX(b.MaPhieuThu) as MaPhieuThu,
    		MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    		MAX(b.TenNguoiNop) as TenNguoiNop, 
    		MAX(b.ThuChi) as ThuChi, 
			MAX(b.NoiDungThuChi) as NoiDungThuChi,
			MAX(b.GhiChu) as GhiChu,
			MAX(b.LoaiThuChi) as LoaiThuChi
    	 FROM
    	(
    select 
		a.HachToanKinhDoanh,
		a.ID_NhomDoiTuong,
		a.ID_DoiTuong,
		a.ID_HoaDon,
		a.MaHoaDon,
    	a.MaPhieuThu,
		a.NgayLapHoaDon,
    	a.TenNguoiNop,
		a.ThuChi,
		a.NoiDungThuChi,
		a.GhiChu,
		Case when a.LoaiThuChi = 1 then N'Phiếu thu khác' else 
    			Case when a.LoaiThuChi = 2 then N'Phiếu chi khác' else
    			Case when a.LoaiThuChi = 3 then N'Thu tiền khách trả' else 
    			Case when a.LoaiThuChi = 4 then N'Chi tiền đổi trả hàng' else 
    			Case when a.LoaiThuChi = 5 then N'Thu tiền nhà NCC' else 
    			Case when a.LoaiThuChi = 6 then N'Chi tiền trả NCC' else '' end end end end end end as LoaiThuChi
    	--Case when a.LoaiThuChi in ('2', '4', '6') then a.Thuchi *(-1) else a.ThuChi end as ThuChi,
    	From
    	(
    		select 
				MAX(qhd.ID) as ID_HoaDon,
				MAX(dt.ID) as ID_DoiTuong,
				MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
				Case when (hd.LoaiHoaDon = 3 and qhd.LoaiHoaDon = 12) then 3 else -- trả tiền thừa
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
				Case When dtn.ID_NhomDoiTuong is null then
				Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			Sum(ISNULL(qhdct.TienThu,0)) as ThuChi,
    			qhd.NgayLapHoaDon,
				MAX(qhd.NoiDungThu) as GhiChu,
				hd.MaHoaDon
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    		left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    		where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and qhdct.DiemThanhToan is null	
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, 
			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    	)a
    	where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    	and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
		and a.HachToanKinhDoanh like @HachToanKD
		) b
    			where ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) 
    			Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon
    		) as c
    		left join DM_DoiTuong dt on c.ID_DoiTuong = dt.ID
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
    		ORDER BY NgayLapHoaDon DESC");

            Sql(@"CREATE FUNCTION [dbo].[FUNC_ConvertStringToUnsign](@inputVar NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN    
    IF (@inputVar IS NULL OR @inputVar = '')  RETURN ''
   
    DECLARE @RT NVARCHAR(MAX)
    DECLARE @SIGN_CHARS NCHAR(256)
    DECLARE @UNSIGN_CHARS NCHAR (256)
 
    SET @SIGN_CHARS = N'ăâđêôơưàảãạáằẳẵặắầẩẫậấèẻẽẹéềểễệếìỉĩịíòỏõọóồổỗộốờởỡợớùủũụúừửữựứỳỷỹỵýĂÂĐÊÔƠƯÀẢÃẠÁẰẲẴẶẮẦẨẪẬẤÈẺẼẸÉỀỂỄỆẾÌỈĨỊÍÒỎÕỌÓỒỔỖỘỐỜỞỠỢỚÙỦŨỤÚỪỬỮỰỨỲỶỸỴÝ' + NCHAR(272) + NCHAR(208)
    SET @UNSIGN_CHARS = N'aadeoouaaaaaaaaaaaaaaaeeeeeeeeeeiiiiiooooooooooooooouuuuuuuuuuyyyyyAADEOOUAAAAAAAAAAAAAAAEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOUUUUUUUUUUYYYYYDD'
 
    DECLARE @COUNTER int
    DECLARE @COUNTER1 int
   
    SET @COUNTER = 1
    WHILE (@COUNTER <= LEN(@inputVar))
    BEGIN  
        SET @COUNTER1 = 1
        WHILE (@COUNTER1 <= LEN(@SIGN_CHARS) + 1)
        BEGIN
            IF UNICODE(SUBSTRING(@SIGN_CHARS, @COUNTER1,1)) = UNICODE(SUBSTRING(@inputVar,@COUNTER ,1))
            BEGIN          
                IF @COUNTER = 1
                    SET @inputVar = SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@inputVar, @COUNTER+1,LEN(@inputVar)-1)      
                ELSE
                    SET @inputVar = SUBSTRING(@inputVar, 1, @COUNTER-1) +SUBSTRING(@UNSIGN_CHARS, @COUNTER1,1) + SUBSTRING(@inputVar, @COUNTER+1,LEN(@inputVar)- @COUNTER)
                BREAK
            END
            SET @COUNTER1 = @COUNTER1 +1
        END
        SET @COUNTER = @COUNTER +1
    END
    -- SET @inputVar = replace(@inputVar,' ','-')
    RETURN LOWER(@inputVar)
END");

            Sql(@"CREATE FUNCTION [dbo].[FUNC_GetStartChar](@stringToSplit NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN 
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
END");
            
            CreateStoredProcedure(name: "[dbo].[BaoCaoChietKhau_ChiTiet]", parametersAction: p => new
            {
                MaNV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ThucHien_TuVan = p.String()
            }, body: @"Select 
	hd.MaHoaDon,
	hd.NgayLapHoaDon,
	dvqd.MaHangHoa,
    hh.TenHangHoa +
    Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    hh.TenHangHoa,
    Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
	nv.MaNhanVien,
	nv.TenNhanVien,
	Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
	Case when ck.ThucHien_TuVan = 0 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan
	from
	BH_NhanVienThucHien ck
	inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
	inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
	inner join NS_NhanVien nv on ck.ID_NhanVien = nv.ID
	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
	Where hd.ChoThanhToan = 0 and
	(hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	and ck.ThucHien_TuVan like @ThucHien_TuVan
	and (nv.MaNhanVien like @MaNV or nv.TenNhanVien like @MaNV)
	OrDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoChietKhau_TongHop]", parametersAction: p => new
            {
                MaNV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ThucHien_TuVan = p.String()
            }, body: @"SELECT 
	a.MaNhanVien,
	MAX(a.TenNhanVien) as TenNhanVien,
	CAST(ROUND(SUM(a.HoaHongThucHien), 0) as float) as HoaHongThucHien,
	CAST(ROUND(SUM(a.HoaHongTuVan), 0) as float) as HoaHongTuVan,
	CAST(ROUND(SUM(a.HoaHongThucHien + a.HoaHongTuVan), 0) as float) as Tong
	FROM
	(
	Select 
	nv.MaNhanVien,
	nv.TenNhanVien,
	Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
	Case when ck.ThucHien_TuVan = 0 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan
	from
	BH_NhanVienThucHien ck
	inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
	inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
	inner join NS_NhanVien nv on ck.ID_NhanVien = nv.ID
	Where hd.ChoThanhToan = 0 and
	(hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	and ck.ThucHien_TuVan like @ThucHien_TuVan
	and (nv.MaNhanVien like @MaNV or nv.TenNhanVien like @MaNV)
	) a
	GROUP BY a.MaNhanVien
	ORDER BY Tong DESC");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoBanHangChiTiet_TheoKhachHang]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_CongNoI]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_CongNoII]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_CongNoIII]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_CongNoIV]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_SoQuy]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_ThuChi]");
            DropStoredProcedure("[dbo].[BaoCaoChietKhau_ChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoChietKhau_TongHop]");
        }
    }
}