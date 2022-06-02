namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreProcedure_20180302 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[DanhMucKhachHang_CongNo]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaKH = p.String(),
                LoaiKH = p.Int(),
                ID_NhomKhachHang = p.String(),
                timeStartKH = p.DateTime(),
                timeEndKH = p.DateTime()
            }, body: @"SELECT * 
		FROM
		(
    	  SELECT 
		  dt.ID as ID,
    	  dt.MaDoiTuong, 
		  Case when dt.ID_NhomDoiTuong is not null then dt.ID_NhomDoiTuong else N'00000000-0000-0000-0000-000000000000' end as ID_NhomDoiTuong,
		  --dt.ID_NhomDoiTuong,
    	  dt.TenDoiTuong,
		  dt.GioiTinhNam,
		  dt.NgaySinh_NgayTLap,
		  dt.DienThoai,
		  dt.Email,
		  dt.DiaChi,
		  dt.MaSoThue,
		  dt.GhiChu,
		  dt.NgayTao,
		  dt.NguoiTao,
		  dt.ID_NguonKhach,
		  dt.ID_NhanVienPhuTrach,
		  dt.ID_NguoiGioiThieu,
		  dt.LaCaNhan,
		  Case when ndt.TenNhomDoiTuong != '' then ndt.TenNhomDoiTuong else N'Nhóm mặc định' end as TenNhomDT,
		  dt.ID_TinhThanh,
		  dt.ID_QuanHuyen,
		  Case when qh.TenQuanHuyen != '' then qh.TenQuanHuyen else N'' end as PhuongXa,
		  Case when tt.TenTinhThanh != '' then tt.TenTinhThanh else N'' end as KhuVuc,
		  Case when dv.TenDonVi != '' then dv.TenDonVi else N'' end as ConTy,
		  Case when dv.SoDienThoai != '' then dv.SoDienThoai else N'' end as DienThoaiChiNhanh,
		  Case when dv.DiaChi != '' then dv.DiaChi else N'' end as DiaChiChiNhanh,
    	  CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua
    	  FROM
			DM_DoiTuong dt
			LEFT join DM_NhomDoiTuong ndt on dt.ID_NhomDoiTuong = ndt.ID
			LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
			LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
			LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
			LEFT Join
    		(
    			  SELECT HangHoa.ID_KhachHang,
    				SUM(ISNULL(HangHoa.NoHienTai, 0)) as NoHienTai, 
    				SUM(ISNULL(HangHoa.TongBan, 0)) as TongBan,
    				SUM(ISNULL(HangHoa.TongBanTruTraHang, 0)) as TongBanTruTraHang,
					SUM(ISNULL(HangHoa.TongMua, 0)) as TongMua
    				FROM
    				(
    					SELECT
    					td.ID_DoiTuong AS ID_KhachHang,
    					SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
							NULL AS TongBan,
							NULL AS TongBanTruTraHang,
							NULL AS TongMua
    					FROM
    					(
    						SELECT 
    						bhd.ID_DoiTuong,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false'
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
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- sổ quỹ
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    						NULL AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null)
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong
    
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						SUM(ISNULL(qhd.TongTienThu,0)) AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null)
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong
    					) AS td
    						GROUP BY td.ID_DoiTuong
    						UNION ALL
    							-- Tổng bán phát sinh trong khoảng thời gian truy vấn
    						SELECT
    						pstv.ID_DoiTuong AS ID_KhachHang,
    						NULL AS NoHienTai,
							SUM(ISNULL(pstv.DoanhThu,0)) AS TongBan,
							SUM(ISNULL(pstv.DoanhThu,0)) -  SUM(ISNULL(pstv.GiaTriTra,0)) AS TongBanTruTraHang,
							SUM(ISNULL(pstv.GiaTriTra,0)) AS TongMua
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
    						) AS pstv
    						GROUP BY pstv.ID_DoiTuong
    					)AS HangHoa
    						GROUP BY HangHoa.ID_KhachHang
    				) a
					on dt.ID = a.ID_KhachHang
    				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH
					or dt.dienthoai like @maKH)
					--and dt.ID_NhomDoiTuong like @ID_NhomKhachHang
    				and dt.loaidoituong = @loaiKH
					and dt.NgayTao >= @timeStartKH and dt.NgayTao < @timeEndKH
				)b
				where ID_NhomDoiTuong like @ID_NhomKhachHang
    				ORDER BY NoHienTai desc");

            CreateStoredProcedure(name: "[dbo].[DanhMucKhachHang_CongNo_ChotSo]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaKH = p.String(),
                LoaiKH = p.Int(),
                ID_NhomKhachHang = p.String(),
                timeStartKH = p.DateTime(),
                timeEndKH = p.DateTime()
            }, body: @"DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
		SELECT * 
		FROM
		(
    	  SELECT 
		  dt.ID as ID,
    	  dt.MaDoiTuong, 
		  Case when dt.ID_NhomDoiTuong is not null then dt.ID_NhomDoiTuong else N'00000000-0000-0000-0000-000000000000' end as ID_NhomDoiTuong,
		  --dt.ID_NhomDoiTuong,
    	  dt.TenDoiTuong,
		  dt.GioiTinhNam,
		  dt.NgaySinh_NgayTLap,
		  dt.DienThoai,
		  dt.Email,
		  dt.DiaChi,
		  dt.MaSoThue,
		  dt.GhiChu,
		  dt.NgayTao,
		  dt.NguoiTao,
		  dt.ID_NguonKhach,
		  dt.ID_NhanVienPhuTrach,
		  dt.ID_NguoiGioiThieu,
		  dt.LaCaNhan,
		  dt.ID_TinhThanh,
		  dt.ID_QuanHuyen,
--		  Case when qh.TenQuanHuyen != '' then qh.TenQuanHuyen else N'' end as PhuongXa,
--		  Case when tt.TenTinhThanh != '' then tt.TenTinhThanh else N'' end as KhuVuc,
		  Case when dv.TenDonVi != '' then dv.TenDonVi else N'' end as ConTy,
		  Case when dv.SoDienThoai != '' then dv.SoDienThoai else N'' end as DienThoaiChiNhanh,
		  Case when dv.DiaChi != '' then dv.DiaChi else N'' end as DiaChiChiNhanh,
    	  CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua
    	  FROM
			DM_DoiTuong dt
			LEFT join DM_NhomDoiTuong ndt on dt.ID_NhomDoiTuong = ndt.ID
			LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
			LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
			LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
			LEFT Join
    		(
    			  SELECT HangHoa.ID_KhachHang,
    				SUM(ISNULL(HangHoa.NoHienTai, 0)) as NoHienTai, 
    				SUM(ISNULL(HangHoa.TongBan, 0)) as TongBan,
    				SUM(ISNULL(HangHoa.TongBanTruTraHang, 0)) as TongBanTruTraHang,
					SUM(ISNULL(HangHoa.TongMua, 0)) as TongMua
    				FROM
    				(
    					SELECT
    					td.ID_DoiTuong AS ID_KhachHang,
    					SUM(ISNULL(td.CongNo, 0)) + SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
							NULL AS TongBan,
							NULL AS TongBanTruTraHang,
							NULL AS TongMua
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
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo
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
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- sổ quỹ
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    						NULL AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong
    
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						SUM(ISNULL(qhd.TongTienThu,0)) AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong
    					) AS td
    						GROUP BY td.ID_DoiTuong
    						UNION ALL
    							-- Tổng bán phát sinh trong khoảng thời gian truy vấn
    						SELECT
    						pstv.ID_DoiTuong AS ID_KhachHang,
    						NULL AS NoHienTai,
							SUM(ISNULL(pstv.DoanhThu,0)) AS TongBan,
							SUM(ISNULL(pstv.DoanhThu,0)) -  SUM(ISNULL(pstv.GiaTriTra,0)) AS TongBanTruTraHang,
							SUM(ISNULL(pstv.GiaTriTra,0)) AS TongMua
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
    						) AS pstv
    						GROUP BY pstv.ID_DoiTuong
    					)AS HangHoa
    						GROUP BY HangHoa.ID_KhachHang
    				) a
					on dt.ID = a.ID_KhachHang
    				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH
					or dt.dienthoai like @maKH)
					--and dt.ID_NhomDoiTuong like @ID_NhomKhachHang
    				and dt.loaidoituong = @loaiKH
					and dt.NgayTao >= @timeStartKH and dt.NgayTao < @timeEndKH
				)b
				where ID_NhomDoiTuong like @ID_NhomKhachHang
    				ORDER BY NoHienTai desc");

            CreateStoredProcedure(name: "[dbo].[Load_DMHangHoa_TonKho]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"-- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT 
		dvqd3.ID_HangHoa as ID,
		dhh.TenHangHoa,
		dvqd3.MaHangHoa, 
		CAST(ROUND(ISNULL(a.TonCuoiKy / dvqd3.TyLeChuyenDoi, 0), 3) as float) as TonKho,
		CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
		CAST(ROUND((dvqd3.GiaVon), 0) as float) as GiaVon,
		dvqd3.TenDonViTinh, 
		dhh.ID_NhomHang as ID_NhomHangHoa,
		dvqd3.ID as ID_DonViQuiDoi,
		an.URLAnh as SrcImage,
		dhh.LaHangHoa

		--dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
  --  	CAST(ROUND((dvqd3.GiaVon), 0) as float) as GiaVon, 
  --  	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
		DonViQuiDoi dvqd3
    	LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd3.ID_HangHoa
		LEFT JOIN 
    		 (
    		SELECT 
    		dhh.ID,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4'))AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
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
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9')AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
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
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    		) AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		GROUP BY dhh.ID
    ) a
	on dvqd3.ID_HangHoa = a.ID
	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
   -- LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    where dvqd3.xoa is null and dhh.duocbantructiep = '1'
    order by TenHangHoa");

            CreateStoredProcedure(name: "[dbo].[Load_DMHangHoa_TonKho_ChotSo]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"-- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT 
		dvqd3.ID_HangHoa as ID,
		a.TenHangHoa, 
		dvqd3.MaHangHoa, 
		CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonKho,
		CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
		CAST(ROUND((dvqd3.GiaVon), 0) as float) as GiaVon,
		dvqd3.TenDonViTinh, 
		a.ID_NhomHang as ID_NhomHangHoa,
		dvqd3.ID as ID_DonViQuiDoi,
		an.URLAnh as SrcImage,
		a.LaHangHoa
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
			dhh.LaHangHoa,
			dhh.ID_NhomHang,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
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
    				ISNULL(cs.TonKho, 0) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					where dvqd.ladonvichuan = '1'
						AND dvqd.Xoa IS NULL
						AND hh.DuocBanTrucTiep = '1'
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
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
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
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
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    		) AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON (dvqd.ID = HangHoa.ID_DonViQuiDoi and dvqd.Xoa IS NULL)
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		GROUP BY dhh.ID, dhh.LaHangHoa, dhh.ID_NhomHang
    ) a
    LEFT Join DonViQuiDoi dvqd3 on (a.ID = dvqd3.ID_HangHoa and dvqd3.Xoa is null)
	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    order by TenHangHoa");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[DanhMucKhachHang_CongNo]");
            DropStoredProcedure("[dbo].[DanhMucKhachHang_CongNo_ChotSo]");
            DropStoredProcedure("[dbo].[Load_DMHangHoa_TonKho]");
            DropStoredProcedure("[dbo].[Load_DMHangHoa_TonKho_ChotSo]");
        }
    }
}
