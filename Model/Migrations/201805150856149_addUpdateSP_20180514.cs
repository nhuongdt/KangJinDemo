namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20180514 : DbMigration
    {
        public override void Up()
        {
            
            
            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_BanHang]", parametersAction: p => new
            {
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT * FROM
    	(
    		SELECT
    			a.ID_KhachHang,
    			case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end AS MaKhachHang,
    			case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end AS TenKhachHang,
    			case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
    			case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
    			CAST(ROUND((a.DoanhThu), 0) as float) as DoanhThu, 
    			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
    			CAST(ROUND((a.DoanhThu - a.GiaTriTra), 0) as float) as DoanhThuThuan,
    			Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    		FROM
    		(
    			SELECT
    				NCC.ID_KhachHang,
    				SUM(ISNULL(NCC.DoanhThu, 0)) as DoanhThu,
    				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
    				NULL AS GiaTriTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 1
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				NULL AS DoanhThu,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 6
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    			) AS NCC
    			Group by NCC.ID_KhachHang
    		) a
    		left join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    	) b
    	where MaKhachHang like @MaKH or TenDoiTuong_KhongDau like @MaKH or TenDoiTuong_ChuCaiDau like @MaKH
    	ORDER BY DoanhThuThuan DESC");

            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoI]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaKH = p.String(),
                LoaiKH = p.Int()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
    	  CAST(ROUND(a.NoDauKy , 0) as float) as NoDauKy,
    	  CAST(ROUND(a.GhiNo , 0) as float) as GhiNo,
    	  CAST(ROUND(a.GhiCo , 0) as float) as GhiCo,
    	  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
    				and dt.loaidoituong = @loaiKH
    				ORDER BY NoCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoII]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaKH = p.String(),
                LoaiKH = p.Int()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
    	  CAST(ROUND(a.NoDauKy , 0) as float) as NoDauKy,
    	  CAST(ROUND(a.GhiNo , 0) as float) as GhiNo,
    	  CAST(ROUND(a.GhiCo , 0) as float) as GhiCo,
    	  CAST(ROUND(a.NoCuoiKy, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
    				and dt.loaidoituong = @loaiKH
    				ORDER BY NoCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoIII]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaKH = p.String(),
                LoaiKH = p.Int()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
    	  CAST(ROUND(a.NoCuoiKy - a.GhiNo + a.GhiCo, 0) as float) as NoDauKy,
    	  CAST(ROUND(a.GhiNo , 0) as float) as GhiNo,
    	  CAST(ROUND(a.GhiCo , 0) as float) as GhiCo,
    	  CAST(ROUND(a.NoCuoiKy, 0) as float) as NoCuoiKy,
          Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
    				and dt.loaidoituong = @loaiKH
    				ORDER BY NoCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoIV]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaKH = p.String(),
                LoaiKH = p.Int()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
    	  CAST(ROUND(a.NoCuoiKy - a.GhiNo + a.GhiCo, 0) as float) as NoDauKy,
    	  CAST(ROUND(a.GhiNo , 0) as float) as GhiNo,
    	  CAST(ROUND(a.GhiCo , 0) as float) as GhiCo,
    	  CAST(ROUND(a.NoCuoiKy, 0) as float) as NoCuoiKy,
    		Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
    				and dt.loaidoituong = @loaiKH
    				ORDER BY NoCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_MuaHang]", parametersAction: p => new
            {
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT * FROM
    	(
    		SELECT
    			a.ID_KhachHang,
    			case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end AS MaKhachHang,
    			case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end AS TenKhachHang,
    			case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
    			case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
    				Case when dt.TongTichDiem is null then 0 else CAST(ROUND(dt.TongTichDiem, 3) as float) end AS TongTichDiem,
    			CAST(ROUND((a.SoLuongMua), 0) as float) as SoLuongMua, 
    			CAST(ROUND((a.GiaTriMua), 0) as float) as GiaTriMua,
    			CAST(ROUND((a.SoLuongTra), 0) as float) as SoLuongTra, 
    			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
    			CAST(ROUND((a.GiaTriMua - a.GiaTriTra), 0) as float) as GiaTriThuan,
    			Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    		FROM
    		(
    			SELECT
    				NCC.ID_KhachHang,
    				SUM(ISNULL(NCC.GiaTriMua, 0)) as GiaTriMua,
    				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra,
    				SUM(ISNULL(NCC.SoLuongMua, 0)) as SoLuongMua,
    				SUM(ISNULL(NCC.SoLuongTra, 0)) as SoLuongTra
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriMua,
    				NULL AS GiaTriTra,
    				NULL AS SoLuongMua,
    				NULL AS SoLuongTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 1
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @maKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				NULL AS GiaTriMua,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra,
    				NULL AS SoLuongMua,
    				NULL AS SoLuongTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 6
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @maKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				NULL AS GiaTriMua,
    				NULL as GiaTriTra,
    				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongMua,
    				NULL AS SoLuongTra
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 1
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @maKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				NULL AS GiaTriMua,
    				NULL as GiaTriTra,
    				NULL AS SoLuongMua,
    				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongTra
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 6
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @maKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong 
    			) AS NCC
    			Group by NCC.ID_KhachHang
    		) a
    		left join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    	) b
    	where MaKhachHang like @MaKH or TenDoiTuong_KhongDau like @MaKH or TenDoiTuong_ChuCaiDau like @MaKH
    	ORDER BY GiaTriThuan DESC");

            AlterStoredProcedure(name: "[dbo].[ReportNCC_MuaHang]", parametersAction: p => new
            {
                MaNCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT * FROM
    	(
    		SELECT
    			a.ID_NCC,
    			case when dt.MaDoiTuong is null then N'NCC lẻ' else dt.MaDoiTuong end AS MaNCC,
    			case when dt.TenDoiTuong is null then N'Nhà cung cấp lẻ' else dt.TenDoiTuong end AS TenNCC,
    			case when dt.TenDoiTuong_KhongDau is null then N'nha cung cap le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
    			case when dt.TenDoiTuong_ChuCaiDau is null then N'nccl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
    			CAST(ROUND(a.SoLuongSanPham, 3) as float) as SoLuongSanPham, 
    			CAST(ROUND(a.GiaTri, 0) as float) as GiaTri,
    			Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    		FROM
    		(
    			SELECT
    				NCC.ID_NCC,
    				SUM(ISNULL(NCC.SoLuongSanPham, 0)) as SoLuongSanPham,
    				SUM(ISNULL(NCC.GiaTri, 0)) as GiaTri
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_NCC,
    				NULL AS SoLuongSanPham,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTri
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 4
    				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.DienThoai like @MaNCC  or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_NCC,
    				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongSanPham,
    				NULL AS GiaTri
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 4
    				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.DienThoai like @MaNCC  or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    			) AS NCC
    			Group by NCC.ID_NCC
    		) a
    		left join DM_DoiTuong dt on a.ID_NCC = dt.ID
			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    	) b
    	where MaNCC like @MaNCC or TenDoiTuong_KhongDau like @MaNCC or TenDoiTuong_ChuCaiDau like @MaNCC
    	ORDER BY GiaTri DESC");

            AlterStoredProcedure(name: "[dbo].[ReportNCC_NhapHang]", parametersAction: p => new
            {
                MaNCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT * FROM
    	(
    		SELECT
    			a.ID_NCC,
    			case when dt.MaDoiTuong is null then N'NCC lẻ' else dt.MaDoiTuong end AS MaNCC,
    			case when dt.TenDoiTuong is null then N'Nhà cung cấp lẻ' else dt.TenDoiTuong end AS TenNCC,
    			case when dt.TenDoiTuong_KhongDau is null then N'nha cung cap le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
    			case when dt.TenDoiTuong_ChuCaiDau is null then N'nccl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
    			CAST(ROUND((a.GiaTriNhap), 0) as float) as GiaTriNhap, 
    			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
    			CAST(ROUND((a.GiaTriNhap - a.GiaTriTra), 0) as float) as GiaTriThuan,
    			Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    		FROM
    		(
    			SELECT
    				NCC.ID_NCC,
    				SUM(ISNULL(NCC.GiaTriNhap, 0)) as GiaTriNhap,
    				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_NCC,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriNhap,
    				NULL AS GiaTriTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 4
    				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.DienThoai like @MaNCC or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_NCC,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 7
    				and (dt.MaDoiTuong like @MaNCC or dt.TenDoiTuong_KhongDau like @MaNCC or dt.TenDoiTuong_ChuCaiDau like @MaNCC or dt.DienThoai like @MaNCC or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    			) AS NCC
    			Group by NCC.ID_NCC
    		) a
    		left join DM_DoiTuong dt on a.ID_NCC = dt.ID
			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    	) b
    	where MaNCC like @MaNCC or TenDoiTuong_KhongDau like @MaNCC or TenDoiTuong_ChuCaiDau like @MaNCC
    	ORDER BY GiaTriThuan DESC");

            CreateStoredProcedure(name: "[dbo].[delete_NhomDoiTuong]", parametersAction: p => new
            {
                ID_NhomDoiTuong = p.Guid()
            }, body: @"Update DM_NhomDoiTuong set TrangThai = 0 where ID = @ID_NhomDoiTuong
	Delete from DM_NhomDoiTuong_ChiTiet where ID_NhomDoiTuong = @ID_NhomDoiTuong
	Delete from DM_DoiTuong_Nhom where ID_NhomDoiTuong = @ID_NhomDoiTuong");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[delete_NhomDoiTuong]");
        }
    }
}
