namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20210307 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_ChiNhanh_v2]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.Boolean()
            }, body: @"SET NOCOUNT ON;
SELECT b.ID_DonVi as ID, dv.TenDonVi, b.ThuTienMat - b.ChiTienMat AS TonTienMat, b.ThuTienGui - b.ChiTienGui AS TonTienGui,
b.ThuTienMat - b.ChiTienMat + b.ThuTienGui - b.ChiTienGui AS TongThuChi  FROM 
(SELECT a.ID_DonVi, SUM(IIF(a.LoaiHoaDon = 11, a.TienGui, 0)) AS ThuTienGui,  
SUM(IIF(a.LoaiHoaDon = 12, a.TienGui, 0)) AS ChiTienGui,
SUM(IIF(a.LoaiHoaDon = 11, a.TienMat, 0)) AS ThuTienMat,
SUM(IIF(a.LoaiHoaDon = 12, a.TienMat, 0)) AS ChiTienMat FROM 
(select qhd.ID_DonVi, qhd.LoaiHoaDon, MAX(qhdct.TienGui) AS TienGui, MAX(qhdct.TienMat) AS TienMat from Quy_HoaDon qhd
inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
left join DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
WHERE qhd.NgayLapHoaDon < DATEADD(DAY,1, @timeEnd)
    and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
	and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(qhdct.ID_DoiTuong IS NOT NULL, dt.loaidoituong, 1)) in (select * from splitstring(@loaiKH)))
    and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
	and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
	and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
	and qhdct.HinhThucThanhToan != 6
--and dt.ID = '00000000-0000-0000-0000-000000000000'
Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhd.ID, qhdct.ID,
    			 qhd.HachToanKinhDoanh, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, qhd.ID_DonVi) a
				 GROUP BY a.ID_DonVi) b
				 INNER JOIN DM_DonVi dv ON b.ID_DonVi =dv.ID;");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_CongNo_v2]", parametersAction: p => new
            {
                TextSearch = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String()
            }, body: @"SET NOCOUNT ON;
	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

    DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
		IF @timeChotSo != null
		BEGIN
			IF @timeChotSo < @timeStart
			BEGIN
    		 SELECT 
				MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
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
    		  '00000000-0000-0000-0000-000000000000'
			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    		SUM(HangHoa.GhiNo) as GhiNo,
    		SUM(HangHoa.GhiCo) as GhiCo
    		FROM
    		(
    			SELECT
    				td.ID_DoiTuong AS ID_KhachHang,
    				SUM(td.CongNo) + SUM(td.DoanhThu) + SUM(td.TienChi) - SUM(td.TienThu) - SUM(td.GiaTriTra) AS NoDauKy,
    				0 AS GhiNo,
    				0 AS GhiCo
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    					ID_KhachHang As ID_DoiTuong,
    					CongNo,
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    			-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				SUM(bhd.TongThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    			SELECT
    				pstv.ID_DoiTuong AS ID_KhachHang,
    				0 AS NoDauKy,
    				SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    				SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo
    			FROM
    			(
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.TongThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25)  AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    		)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi='0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    				LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		ORDER BY MAX(b.MaKhachHang) DESC
			END
			ELSE IF @timeChotSo > @timeEnd
			BEGIN
				SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
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
    		  '00000000-0000-0000-0000-000000000000'
			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    		SUM(HangHoa.GhiNo) as GhiNo,
    		SUM(HangHoa.GhiCo) as GhiCo,
    		SUM(HangHoa.NoCuoiKy) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    			0 AS GhiNo,
    			0 AS GhiCo,
    			SUM(td.CongNo) - SUM(td.DoanhThu) - SUM(td.TienChi) + SUM(td.TienThu) + SUM(td.GiaTriTra) AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    			SELECT 
    			ID_KhachHang As ID_DoiTuong,
    			CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM ChotSo_KhachHang
    			where ID_DonVi = @ID_ChiNhanh
    			UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    			SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    			SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    			SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo,
    			0 AS NoCuoiKy
    			FROM
    			(
    			SELECT 
    			bhd.ID_DoiTuong,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi = '0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
			END
			ELSE
			BEGIN
			SELECT 
			 MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
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
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo,0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    			SUM(HangHoa.GhiNo) as GhiNo,
    			SUM(HangHoa.GhiCo) as GhiCo,
    			SUM(HangHoa.NoCuoiKy) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(td.CongNo) - SUM(td.DoanhThu) - SUM(td.TienChi) + SUM(td.TienThu) + SUM(td.GiaTriTra) AS NoDauKy,
    				SUM(td.DoanhThu) + SUM(td.TienChi) AS GhiNo,
    				SUM(td.TienThu) + SUM(td.GiaTriTra) AS GhiCo,
    				0 AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				CongNo AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    				UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    				SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    				SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    				SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo,
    			    SUM(pstv.CongNo) + SUM(pstv.DoanhThu) + SUM(pstv.TienChi) - SUM(pstv.TienThu) - SUM(pstv.GiaTriTra) AS NoCuoiKy
    			FROM
    			(
    				-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				CongNo AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    
    			SELECT 
    			bhd.ID_DoiTuong,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi ='0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
			END
		END
		ELSE
		BEGIN
			SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
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
    	  SELECT 
			  a.ID_KhachHang, 
    		  dt.MaDoiTuong AS MaKhachHang, 
    		  dt.TenDoiTuong AS TenKhachHang,
    		  a.NoDauKy,
    		 --a.GhiNo As TongTienChi,
    		 -- a.GhiCo As TongTienThu,
			  iif(a.GhiNo<= 0, iif(a.GhiCo < 0, -a.GhiCo, 0 ), a.GhiNo) as TongTienChi,
			  iif(a.GhiCo <=0, iif(a.GhiNo < 0, -a.GhiNo, 0 ), a.GhiCo) as TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    		  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    			SUM(HangHoa.NoDauKy) as NoDauKy, 
    			SUM(HangHoa.GhiNo) as GhiNo,
    			SUM(HangHoa.GhiCo) as GhiCo,
    			SUM(HangHoa.NoDauKy + HangHoa.GhiNo - HangHoa.GhiCo) as NoCuoiKy
    		FROM
    		(
    			SELECT
    				td.ID_DoiTuong AS ID_KhachHang,
    				SUM(td.DoanhThu) + SUM(td.TienChi) - SUM(td.TienThu) - SUM(td.GiaTriTra) AS NoDauKy,
    				0 AS GhiNo,
    				0 AS GhiCo,
    				0 AS NoCuoiKy
    			FROM
    			(

				---- CÔNG NỢ ĐẦU KỲ
				---- doanhthu khachhang
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong

				union all
				---- doanhthu baohiem
    			SELECT 
    				bhd.ID_BaoHiem,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToanBaoHiem) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon in (1,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				and bhd.ID_BaoHiem is not null
    			GROUP BY bhd.ID_BaoHiem

    			-- trahang of khachhag
    			UNION All
    			SELECT bhd.ID_DoiTuong,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (4,6) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong

    			-- thucthu khachhang + baohiem + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND qhdct.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND qhdct.ID_DoiTuong is not null
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
				-- phieuchi khachhang + ncc+ baohiem
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon    		
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND qhdct.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND qhdct.ID_DoiTuong is not null
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL

    				-- Công nợ phát sinh trong khoảng thời gian truy vấn (---- CÔNG NỢ TRONG KỲ  ------)
    			SELECT
    				pstv.ID_DoiTuong AS ID_KhachHang,
    				0 AS NoDauKy,
					SUM(pstv.DoanhThu) - SUM(pstv.TienThu) AS GhiCo,
    				SUM(pstv.GiaTriTra) - SUM(pstv.TienChi) AS GhiNo,
    			
    				0 AS NoCuoiKy
    			FROM
    			(
				-- KhachHang: doanh thu
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong

				union all
				---- doanhthu baohiem
    			SELECT 
    				bhd.ID_BaoHiem,
					0 AS GiaTriTra,    				
    				sum( bhd.PhaiThanhToanBaoHiem) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon in (1,25) AND bhd.ChoThanhToan = '0'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				and bhd.ID_BaoHiem is not null
    			GROUP BY bhd.ID_BaoHiem

    			-- khachhang: trahang
    			UNION All
    			SELECT bhd.ID_DoiTuong,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong

    			--  phieuthu: kh + bh + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd		
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND qhdct.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND qhdct.ID_DoiTuong is not null
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
					-- phieuchi: kh + bh + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND qhdct.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND qhdct.ID_DoiTuong is not null
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    				left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where  dt.TheoDoi ='0' 
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy	
    			ORDER BY MAX(b.MaKhachHang) DESC
		END");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoNam_v2]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.Boolean(),
                LoaiTien = p.String()
            }, body: @"set nocount on;
		declare @tblNhomDT table(ID_NhomDoiTuong varchar(40))
		insert into @tblNhomDT
		select * from dbo.splitstring(@ID_NhomDoiTuong)
    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	select 
			b.ID_KhoanThuChi,
			b.KhoanMuc,
			sum(b.Thang1) as Thang1,
			sum(b.Thang2) as Thang2,
			sum(b.Thang3) as Thang3,
			sum(b.Thang4) as Thang4,
			sum(b.Thang5) as Thang5,
			sum(b.Thang6) as Thang6,
			sum(b.Thang7) as Thang7,
			sum(b.Thang8) as Thang8,
			sum(b.Thang9) as Thang9,
			sum(b.Thang10) as Thang10,
			sum(b.Thang11) as Thang11,
			sum(b.Thang12) as Thang12,
			max(STT) as STT

	from
	(
		select 
			a.ID_KhoanThuChi,
			case a.LoaiThuChi
			when 3 then N'Thu tiền bán hàng'
			when 5 then N'Thu trả hàng nhà cung cấp'		
			else case when a.ID_KhoanThuChi is null then N'Thu mặc định' else NoiDungThuChi end end as KhoanMuc,
			case when a.ThangLapHoaDon = 1 then tienthu end as Thang1,
			case when a.ThangLapHoaDon = 2 then tienthu end as Thang2,
			case when a.ThangLapHoaDon = 3 then tienthu end as Thang3,
			case when a.ThangLapHoaDon = 4 then tienthu end as Thang4,
			case when a.ThangLapHoaDon = 5 then tienthu end as Thang5,
			case when a.ThangLapHoaDon = 6 then tienthu end as Thang6,
			case when a.ThangLapHoaDon = 7 then tienthu end as Thang7,
			case when a.ThangLapHoaDon = 8 then tienthu end as Thang8,
			case when a.ThangLapHoaDon = 9 then tienthu end as Thang9,
			case when a.ThangLapHoaDon = 10 then tienthu end as Thang10,
			case when a.ThangLapHoaDon = 11 then tienthu end as Thang11,
			case when a.ThangLapHoaDon = 12 then tienthu end as Thang12		,
			ROW_NUMBER() OVER(ORDER BY a.NoiDungThuChi) as STT		
		from
		(
		select 
    			--a1.ID_NhomDoiTuong,
				a1.LoaiThuChi,			
				a1.ID_KhoanThuChi,
				a1.NoiDungThuChi,
    			a1.ThangLapHoaDon,
				Case when @LoaiTien = '%1%' then a1.TienMat
					when @LoaiTien = '%2%' then a1.TienGui else a1.tienmat + a1.TienGui end as TienThu,
    			Case when a1.TienMat > 0 and TienGui = 0 then '1'  
    			 when a1.TienGui > 0 and TienMat = 0 then '2' 
    			 when a1.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
				select    					
					qhdct.ID_KhoanThuChi,
    				ktc.NoiDungThuChi,
    				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    				Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    				 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    				 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    				 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    				 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    				 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
					case when dt.LoaiDoiTuong= 1 then	
					case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end
					else
						case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000002' else dt.IDNhomDoiTuongs end end as ID_NhomDoiTuong,   
					tienmat, tiengui,
    				tienmat +  tiengui as TienThu,
					DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
    				hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan= 0)
    			and qhd.LoaiHoaDon = 11
				and qhdct.HinhThucThanhToan != 6
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') 
				) a1
				where (EXISTS(SELECT Name FROM splitstring(a1.ID_NhomDoiTuong) dtDB inner JOIN @tblNhomDT dtS ON dtDB.Name = dtS.ID_NhomDoiTuong) 
				or @ID_NhomDoiTuong = '')
				--where  (a1.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
		) a where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) --and a.LoaiTien like @LoaiTien
	) b group by b.ID_KhoanThuChi, b.KhoanMuc
			DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then MAX(b.TienMat)
				when @LoaiTien = '%2%' then MAX(b.TienGui) else
				MAX(b.TienMat + b.TienGui) end as TienThu
				--MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,    		
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan = 0)
    			and qhd.LoaiHoaDon = 12
				and qhdct.HinhThucThanhToan != 6
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0')
				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong, qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select *
			from
			(
			select max(ID_KhoanThuChi) as ID_KhoanThuChi, -- deu chi tien nhaphang, nhưng ID_KhoanThuChi # nhau --> thi bi douple, nen chi group tho KhoanMua va lay max (ID_KhoanThuChi)
			KhoanMuc, 
			MAX(STT) as STT,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY KhoanMuc
			) tblview where TongCong > 0
			order by STT");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoQuy_v2]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.Boolean(),
                LoaiTien = p.String()
            }, body: @"set nocount on;
		declare @tblNhomDT table(ID_NhomDoiTuong varchar(40))
		insert into @tblNhomDT
		select * from dbo.splitstring(@ID_NhomDoiTuong)

    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	select 
			b.ID_KhoanThuChi,
			b.KhoanMuc,
			sum(b.Thang1) as Thang1,
			sum(b.Thang2) as Thang2,
			sum(b.Thang3) as Thang3,
			sum(b.Thang4) as Thang4,
			sum(b.Thang5) as Thang5,
			sum(b.Thang6) as Thang6,
			sum(b.Thang7) as Thang7,
			sum(b.Thang8) as Thang8,
			sum(b.Thang9) as Thang9,
			sum(b.Thang10) as Thang10,
			sum(b.Thang11) as Thang11,
			sum(b.Thang12) as Thang12,
			max(STT) as STT

	from
	(
		select 
			a.ID_KhoanThuChi,
			case a.LoaiThuChi
			when 3 then N'Thu tiền bán hàng'
			when 5 then N'Thu trả hàng nhà cung cấp'		
			else case when a.ID_KhoanThuChi is null then N'Thu mặc định' else NoiDungThuChi end end as KhoanMuc,
			case when a.ThangLapHoaDon = 1 then tienthu end as Thang1,
			case when a.ThangLapHoaDon = 2 then tienthu end as Thang2,
			case when a.ThangLapHoaDon = 3 then tienthu end as Thang3,
			case when a.ThangLapHoaDon = 4 then tienthu end as Thang4,
			case when a.ThangLapHoaDon = 5 then tienthu end as Thang5,
			case when a.ThangLapHoaDon = 6 then tienthu end as Thang6,
			case when a.ThangLapHoaDon = 7 then tienthu end as Thang7,
			case when a.ThangLapHoaDon = 8 then tienthu end as Thang8,
			case when a.ThangLapHoaDon = 9 then tienthu end as Thang9,
			case when a.ThangLapHoaDon = 10 then tienthu end as Thang10,
			case when a.ThangLapHoaDon = 11 then tienthu end as Thang11,
			case when a.ThangLapHoaDon = 12 then tienthu end as Thang12		,
			ROW_NUMBER() OVER(ORDER BY a.NoiDungThuChi) as STT		
		from
		(
		select 
    			--a1.ID_NhomDoiTuong,
				a1.LoaiThuChi,			
				a1.ID_KhoanThuChi,
				a1.NoiDungThuChi,
    			a1.ThangLapHoaDon,
				Case when @LoaiTien = '%1%' then a1.TienMat
					when @LoaiTien = '%2%' then a1.TienGui else a1.tienmat + a1.TienGui end as TienThu,
    			Case when a1.TienMat > 0 and TienGui = 0 then '1'  
    			 when a1.TienGui > 0 and TienMat = 0 then '2' 
    			 when a1.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
				select    					
					qhdct.ID_KhoanThuChi,
    				ktc.NoiDungThuChi,
    				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    				Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    				 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    				 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    				 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    				 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    				 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
					case when dt.LoaiDoiTuong= 1 then	
					case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end
					else
						case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000002' else dt.IDNhomDoiTuongs end end as ID_NhomDoiTuong,   
					tienmat, tiengui,
    				tienmat +  tiengui as TienThu,
					DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
    				hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan= 0)
    			and qhd.LoaiHoaDon = 11
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') 
				and qhdct.HinhThucThanhToan != 6
				) a1
				where (EXISTS(SELECT Name FROM splitstring(a1.ID_NhomDoiTuong) dtDB inner JOIN @tblNhomDT dtS ON dtDB.Name = dtS.ID_NhomDoiTuong) 
				or @ID_NhomDoiTuong = '')
				--where  (a1.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
		) a where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) --and a.LoaiTien like @LoaiTien
	) b group by b.ID_KhoanThuChi, b.KhoanMuc
		DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then MAX(b.TienMat)
				when @LoaiTien = '%2%' then MAX(b.TienGui) else
				MAX(b.TienMat + b.TienGui) end as TienThu
				--MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and qhd.LoaiHoaDon = 12
				and qhdct.HinhThucThanhToan != 6
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') -- DieuChinh CongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID, qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong, qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select *
			from
			(
			select max(ID_KhoanThuChi) as ID_KhoanThuChi,
			KhoanMuc, 
			MAX(STT) as STT,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) as Quy1,
			ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) as Quy2,
			ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + ISNULL(SUM(Thang9),0) as Quy3,
			ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as Quy4,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY  KhoanMuc  
			) tblview where TongCong > 0
			order by STT");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoThang_v2]", parametersAction: p => new
            {
                year = p.Int(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.Boolean(),
                LoaiTien = p.String()
            }, body: @"SET NOCOUNT ON;
    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	SELECT
			ID_KhoanThuChi,
			CASE When c.LoaiThuChi = 3 then N'Thu tiền bán hàng'
			When c.LoaiThuChi = 5 then N'Thu trả hàng nhà cung cấp'
			When ID_KhoanThuChi is null then N'Thu mặc định'
			else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then SUM(b.TienMat)
				when @LoaiTien = '%2%' then SUM(b.TienGui) else
				SUM(b.TienMat + b.TienGui) end as TienThu
    		FROM
    		(
			select 
    		a.ID_NhomDoiTuong,
			a.LoaiThuChi,
			a.ID_HoaDon,
			a.ID_DoiTuong,
			a.ID_KhoanThuChi,
    		a.ThangLapHoaDon,
			a.TienMat,
			a.TienGui,
			a.TienThu,
    		Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			when a.TienGui > 0 and TienMat = 0 then '2' 
    			when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    			 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    			 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    			 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    			 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    			--Case When dtn.ID_NhomDoiTuong is null then
    			--Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
				case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end as ID_NhomDoiTuong,
    			max(qhdct.TienMat) as TienMat,
    			max(qhdct.TienGui) as TienGui,
    			max(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan = 0 or qhdct.DiemThanhToan is null)
				and qhd.LoaiHoaDon = 11
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				AND qhdct.HinhThucThanhToan != 6
				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') -- DieuChinh CongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID, qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi, 
    			qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dt.IDNhomDoiTuongs, qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then MAX(b.TienMat)
				when @LoaiTien = '%2%' then MAX(b.TienGui) else
				MAX(b.TienMat + b.TienGui) end as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienMat,
				a.TienGui,
				a.TienThu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
				and qhd.LoaiHoaDon = 12
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				AND qhdct.HinhThucThanhToan != 6
				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') -- dcCongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID,qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong,qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select max(ID_KhoanThuChi) as ID_KhoanThuChi, -- deu chi tien nhaphang, nhưng ID_KhoanThuChi # nhau --> thi bi douple, nen chi group tho KhoanMua va lay max (ID_KhoanThuChi)
			KhoanMuc, 
			CAST(ROUND(SUM(Thang1), 0) as float) as Thang1,
			CAST(ROUND(SUM(Thang2), 0) as float) as Thang2,
			CAST(ROUND(SUM(Thang3), 0) as float) as Thang3,
			CAST(ROUND(SUM(Thang4), 0) as float) as Thang4,
			CAST(ROUND(SUM(Thang5), 0) as float) as Thang5,
			CAST(ROUND(SUM(Thang6), 0) as float) as Thang6,
			CAST(ROUND(SUM(Thang7), 0) as float) as Thang7,
			CAST(ROUND(SUM(Thang8), 0) as float) as Thang8,
			CAST(ROUND(SUM(Thang9), 0) as float) as Thang9,
			CAST(ROUND(SUM(Thang10), 0) as float) as Thang10,
			CAST(ROUND(SUM(Thang11), 0) as float) as Thang11,
			CAST(ROUND(SUM(Thang12), 0) as float) as Thang12,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY  KhoanMuc
			order by MAX(STT)");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_SoQuy_v2]", parametersAction: p => new
            {
                TextSearch = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.Boolean(),
                LoaiTien = p.String()
            }, body: @"SET NOCOUNT ON;
	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    --	tinh ton dau ky
    	Declare @TonDauKy float
    	Set @TonDauKy = (Select
    	CAST(ROUND(SUM(TienThu - TienChi), 0) as float) as TonDauKy
    	FROM
    	(
    		select 
    			case when qhd.LoaiHoaDon = 11 then qhdct.TienMat + qhdct.TienGui else 0 end as TienThu,
    			Case when qhd.LoaiHoaDon = 12 then qhdct.TienMat + qhdct.TienGui else 0 end as TienChi,
    			Case when qhdct.TienMat > 0 and qhdct.TienGui = 0 then '1' 
    			when qhdct.TienGui > 0 and qhdct.TienMat = 0 then '2'
    			when qhdct.TienGui > 0 and qhdct.TienMat > 0 then '12' else '' end as LoaiTien,
				qhd.HachToanKinhDoanh as HachToanKinhDoanh
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		where qhd.NgayLapHoaDon < @timeStart
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
			and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
			and qhdct.HinhThucThanhToan != 6
    		) a 
    		where LoaiTien like @LoaiTien
			and (HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    	) 
		
    	if (@TonDauKy is null)
    	BeGin
    		Set @TonDauKy = 0;
    	END
    	Declare @tmp table (ID_HoaDon UNIQUEIDENTIFIER,MaPhieuThu nvarchar(max), NgayLapHoaDon datetime, KhoanMuc nvarchar(max), TenDoiTac nvarchar(max),
    	TienMat float, TienGui float, TienThu float, TienChi float, ThuTienMat float, ChiTienMat float, ThuTienGui float, ChiTienGui float, TonLuyKeTienMat float,TonLuyKeTienGui float,TonLuyKe float, SoTaiKhoan nvarchar(max), NganHang nvarchar(max), GhiChu nvarchar(max),
		IDDonVi UNIQUEIDENTIFIER, TenDonVi NVARCHAR(MAX));
    	Insert INTO @tmp
    		 SELECT 
				b.ID_HoaDon,
				b.MaPhieuThu as MaPhieuThu,
    			b.NgayLapHoaDon as NgayLapHoaDon,
				MAX(b.NoiDungThuChi) as KhoanMuc,
    			MAX(b.TenNguoiNop) as TenDoiTac, 
    			SUM (b.TienMat) as TienMat,
    			SUM (b.TienGui) as TienGui,
    			SUM (b.TienThu) as TienThu,
    			SUM (b.TienChi) as TienChi,
    			SUM (b.ThuTienMat) as ThuTienMat,
    			SUM (b.ChiTienMat) as ChiTienMat, 
    			SUM (b.ThuTienGui) as ThuTienGui,
    			SUM (b.ChiTienGui) as ChiTienGui, 
				0 as TonLuyKe,
    			0 as TonLuyKeTienMat,
    			0 as TonLuyKeTienGui,
    			MAX(b.SoTaiKhoan) as SoTaiKhoan,
    			MAX(b.NganHang) as NganHang,
    			MAX(b.GhiChu) as GhiChu,
				dv.ID,
				dv.TenDonVi
    		FROM
    		(
				select 
    			a.HachToanKinhDoanh,
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
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' else '' end  as LoaiTien,
				a.ID_DonVi
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			MAX (tknh.SoTaiKhoan) as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    			max(qhdct.TienMat) as TienMat,
    			max(qhdct.TienGui) as TienGui,
    			max(qhdct.TienThu) as TienThu,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
				qhd.ID_DonVi
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
				and qhdct.HinhThucThanhToan != 6
				AND ((select count(Name) from @tblSearch b where     			
    			dt.TenDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or qhd.MaHoaDon like '%' + b.Name + '%'
				or qhd.NguoiNopTien like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau,qhdct.ID_NhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, qhd.ID_DonVi, qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
			inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
			where LoaiTien like @LoaiTien
    		Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaPhieuThu, b.NgayLapHoaDon, dv.TenDonVi, dv.ID
    		ORDER BY NgayLapHoaDon
    -- tính tồn lũy kế
	    IF (EXISTS (select * from @tmp))
		BEGIN
    			DECLARE @Ton float;
    			SET @Ton = @TonDauKy;
    			DECLARE @TonTienMat float;
    			SET @TonTienMat = @TonDauKy;
    			DECLARE @TonTienGui float;
    			SET @TonTienGui = @TonDauKy;
    			
    			DECLARE @TienThu float;
    			DECLARE @TienChi float;
    			DECLARE @ThuTienMat float;
    			DECLARE @ChiTienMat float;
    			DECLARE @ThuTienGui float;
    			DECLARE @ChiTienGui float;
    			DECLARE @TonLuyKe float;
				DECLARE @ID_HoaDon UNIQUEIDENTIFIER;
    	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT TienThu, TienChi, ThuTienGui, ThuTienMat, ChiTienGui, ChiTienMat, ID_HoaDon FROM @tmp ORDER BY NgayLapHoaDon
    	OPEN CS_ItemUpDate 
    FETCH FIRST FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon
    WHILE @@FETCH_STATUS = 0
    BEGIN
    	SET @Ton = @Ton + @TienThu - @TienChi;
    	SET @TonTienMat = @TonTienMat + @ThuTienMat - @ChiTienMat;
    	SET @TonTienGui = @TonTienGui + @ThuTienGui - @ChiTienGui;
    	UPDATE @tmp SET TonLuyKe = @Ton, TonLuyKeTienMat = @TonTienMat, TonLuyKeTienGui = @TonTienGui WHERE ID_HoaDon = @ID_HoaDon
    	FETCH NEXT FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon
    END
    CLOSE CS_ItemUpDate
    DEALLOCATE CS_ItemUpDate
	END
	ELSE
	BEGIN
		Insert INTO @tmp
    	SELECT '00000000-0000-0000-0000-000000000000', 'TRINH0001', '1989-04-07','','','0','0','0','0','0','0','0','0', @TonDauKy, @TonDauKy, @TonDauKy, '','','', '00000000-0000-0000-0000-000000000000', ''
	END
    	Select 
		ID_HoaDon,
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
    	GhiChu,
		IDDonVi, TenDonVi
    	 from @tmp order by NgayLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_ThuChi_v2]", parametersAction: p => new
            {
                TextSearch = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.Boolean()
            }, body: @"SET NOCOUNT ON;

DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

    SELECT 
    MAX(b.TenNhomDoiTuong) as NhomDoiTuong,
    b.MaHoaDon,
    MAX(b.MaPhieuThu) as MaPhieuThu,
    MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    MAX(b.ManguoiNop) as ManguoiNop, 
    MAX(b.TenNguoiNop) as TenNguoiNop, 
    MAX(b.ThuChi) as ThuChi, 
    MAX(b.NoiDungThuChi) as NoiDungThuChi,
    MAX(b.GhiChu) as GhiChu,
    MAX(b.LoaiThuChi) as LoaiThuChi,
	dv.TenDonVi AS TenChiNhanh
    FROM
    (
	  select 
		a.ID_DoiTuong,
		a.ID_HoaDon,
		a.TenNhomDoiTuong,
		a.ID_NhomDoiTuong,
    	a.MaHoaDon,
		a.MaPhieuThu,
    	a.NgayLapHoaDon,
		a.MaNguoiNop,
    	a.TenNguoiNop,
    	--a.ThuChi,
		a.TienMat + a.TienGui as ThuChi,
    	a.NoiDungThuChi,
    	a.GhiChu,
    	Case when a.LoaiThuChi = 1 then N'Phiếu thu khác'  
    	when a.LoaiThuChi = 2 then N'Phiếu chi khác' 
    	when a.LoaiThuChi = 3 then N'Thu tiền khách trả'  
    	when a.LoaiThuChi = 4 then N'Chi tiền đổi trả hàng'  
    	when a.LoaiThuChi = 5 then N'Thu tiền nhà NCC'  
    	when a.LoaiThuChi = 6 then N'Chi tiền trả NCC' else '' end as LoaiThuChi,
		a.ID_DonVi
    	From
    	(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			MAX (tknh.SoTaiKhoan) as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
				--Max(dt.TenNhomDoiTuongs) as TenNhomDoiTuong,
				case when qhdct.ID_NhanVien is not null then N'Nhân viên' else MAX(dt.TenNhomDoiTuongs) end as TenNhomDoiTuong,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case WHEN qhdct.ID_NhanVien is not null
				then
    				'00000000-0000-0000-0000-000000000000' 
				else 
				case When dtn.ID_NhomDoiTuong is null 
					
				then '00000000-0000-0000-0000-000000000000'  else dtn.ID_NhomDoiTuong 
				end
				end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
				case when qhdct.ID_NhanVien is not null then nv.MaNhanVien else dt.MaDoiTuong end as ManguoiNop,
    			Sum(qhdct.TienMat) as TienMat,
    			Sum(qhdct.TienGui) as TienGui,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
				qhd.ID_DonVi
    		From Quy_HoaDon qhd 			
    			join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
				left join NS_NhanVien nv on qhdct.ID_NhanVien= nv.ID
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart and @timeEnd 
				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, dt.loaidoituong) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				AND ((select count(Name) from @tblSearch b where     			
    			dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or qhd.MaHoaDon like '%' + b.Name + '%'
				or hd.MaHoaDon like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhdct.ID_NhanVien, dt.MaDoiTuong,dt.LoaiDoiTuong,  nv.MaNhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID, qhd.ID_DonVi
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) 
    	) b
		inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
		where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = ''
    	Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi, dv.TenDonVi
    	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[GetAllKhachHang_NotWhere]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_NhanVienLogin = p.Guid()
            }, body: @"SET NOCOUNT ON;
	
	declare @xemAll bit=( select LaAdmin from HT_NguoiDung where ID_NhanVien = @ID_NhanVienLogin)

	declare @tblNhanVien table (ID_NhanVien uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'KhachHang_XemDS_PhonBan','KhachHang_XemDS_HeThong');


	declare @tblIDNhoms table (ID varchar(36))
	declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where ID_DonVi like @ID_ChiNhanh)
			insert into @tblIDNhoms(ID) values ('00000000-0000-0000-0000-000000000000')

			if @QLTheoCN = 1
				begin									
					insert into @tblIDNhoms(ID)
					select *  from (
						-- get Nhom not not exist in NhomDoiTuong_DonVi
						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
						and LoaiDoiTuong = 1 
						union all
						-- get Nhom at this ChiNhanh
						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where ID_DonVi like @ID_ChiNhanh) tbl
				end
			else
				begin				
				-- insert all
				insert into @tblIDNhoms(ID)
				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
				where LoaiDoiTuong = 1
				end	

				  SELECT  *
    		FROM
    		(
    		  SELECT 
    		  dt.ID as ID,
			  dt.LoaiDoiTuong,
    		  dt.MaDoiTuong, 
			  dt.TheoDoi,
			  case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
			  case when dt.TenDoiTuong_KhongDau is null then '' else ltrim(dt.TenDoiTuong_KhongDau) end as TenDoiTuong_KhongDau,   		 
    		  dt.TenDoiTuong_ChuCaiDau,
			  dt.ID_TrangThai,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
			  dt.NgayGiaoDichGanNhat,
    		  dt.DienThoai,
    		  dt.Email,
    		  dt.DiaChi,
    		  dt.MaSoThue,
    		  ISNULL(dt.GhiChu,'') as GhiChu,
    		  dt.NgayTao,
    		  dt.DinhDang_NgaySinh,
    		  ISNULL(dt.NguoiTao,'') as NguoiTao,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    		  dt.LaCaNhan,
    		  ISNULL(dt.TongTichDiem,0) as TongTichDiem,
			  case when right(rtrim(dt.TenNhomDoiTuongs),1) =',' then LEFT(Rtrim(dt.TenNhomDoiTuongs), len(dt.TenNhomDoiTuongs)-1) else ISNULL(dt.TenNhomDoiTuongs,N'Nhóm mặc định') end as TenNhomDT,-- remove last coma
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
			  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri, 
			  concat(dt.MaDoiTuong,' ',lower(dt.MaDoiTuong) ,' ', dt.TenDoiTuong,' ', dt.DienThoai,' ', dt.TenDoiTuong_KhongDau)  as Name_Phone
    		  FROM DM_DoiTuong dt    				  
    		)b
    			where b.ID not like '%00000000-0000-0000-0000-0000%'	
				and b.TheoDoi= 0
				and b.LoaiDoiTuong= 1
				and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
				and (@xemAll ='1' or 
				(EXISTS(SELECT ID_NhanVien from @tblNhanVien nv where b.ID_NhanVienPhuTrach= nv.ID_NhanVien)
				or b.ID_NhanVienPhuTrach is null))
				order by b.ngaytao desc");

            CreateStoredProcedure(name: "[dbo].[GetList_HoaDonNhapHang]", parametersAction: p => new
            {
                TextSearch = p.String(),
                LoaiHoaDon = p.Int(),
                IDChiNhanhs = p.String(),
                FromDate = p.DateTime(),
                ToDate = p.DateTime(),
                TrangThais = p.String(),
                CurrentPage = p.Int(),
                PageSize = p.Int(),
                ColumnSort = p.String(),
                SortBy = p.String()
            }, body: @"SET NOCOUNT ON;
	declare @tblChiNhanh table (ID varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@IDChiNhanhs)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	with data_cte
	as (
	select hdQuy.*	, hdQuy.PhaiThanhToan - hdQuy.KhachDaTra as ConNo
	from
	(	
	select hd.id, hd.MaHoaDon, hd.LoaiHoaDon, hd.DienGiai, hd.PhaiThanhToan, hd.ChoThanhToan,
	hd.NgayLapHoaDon, hd.ID_NhanVien, hd.ID_BangGia, hd.TongTienHang, hd.TongChietKhau, hd.TongGiamGia, hd.TongChiPhi,
	hd.TongTienThue, hd.TongThanhToan, hd.ID_DoiTuong, isnull(quy.TienThu,0) as KhachDaTra	, hd.NguoiTao, hd.NguoiTao as NguoiTaoHD,
	dv.TenDonVi,hd.ID_DonVi,
	isnull(dv.SoDienThoai,'') as DienThoaiChiNhanh,
	isnull(dv.DiaChi,'') as DiaChiChiNhanh,
	isnull(dt.MaDoiTuong,'') as MaDoiTuong,
	isnull(dt.TenDoiTuong,'') as TenDoiTuong,
	isnull(dt.DienThoai,'') as DienThoai,
	isnull(dt.TenDoiTuong_KhongDau,'') as TenDoiTuong_KhongDau,
	isnull(nv.MaNhanVien,'') as MaNhanVien,
	isnull(nv.TenNhanVien,'') as TenNhanVien,
	isnull(nv.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
	case  hd.ChoThanhToan
				when 1 then '1'
				when 0 then '0'
			else '2' end as TrangThaiHD
	from BH_HoaDon hd
	join DM_DonVi dv on hd.ID_DonVi= dv.ID
	left join  DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
	left join
	(
		select a.ID_HoaDonLienQuan, sum(TienThu) as TienThu
		from(
			select qct.ID_HoaDonLienQuan,   iif(qhd.LoaiHoaDon = 11,-qct.TienThu, qct.TienThu) as TienThu
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
			join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
			where hd.LoaiHoaDon= @LoaiHoaDon
			and (qhd.TrangThai= 1 or qhd.TrangThai is null)
			and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
			and  exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
			) a group by a.ID_HoaDonLienQuan
	) quy on hd.id = quy.ID_HoaDonLienQuan
	where hd.LoaiHoaDon= @LoaiHoaDon and
	NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
	and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
   ) hdQuy
   where 
   exists (select ID from dbo.splitstring(@TrangThais) tt where hdQuy.TrangThaiHD= tt.Name)	
	and
	((select count(Name) from @tblSearch b where     			
		hdQuy.MaHoaDon like '%'+b.Name+'%'
		or hdQuy.NguoiTao like '%'+b.Name+'%'				
		or hdQuy.TenNhanVien like '%'+b.Name+'%'
		or hdQuy.TenNhanVienKhongDau like '%'+b.Name+'%'
		or hdQuy.DienGiai like '%'+b.Name+'%'
		or hdQuy.MaDoiTuong like '%'+b.Name+'%'		
		or hdQuy.TenDoiTuong like '%'+b.Name+'%'
		or hdQuy.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or hdQuy.DienThoai like '%'+b.Name+'%'		
		)=@count or @count=0)	
		),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumDaThanhToan,				
				sum(TongChiPhi) as SumTongChiPhi,
				sum(PhaiThanhToan) as SumPhaiThanhToan,
				sum(TongThanhToan) as SumTongThanhToan,				
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo
			from data_cte
		)
		select dt.*, cte.*	
		from data_cte dt
		cross join count_cte cte
		order by 
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='ConNo' then ConNo end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='ConNo' then ConNo end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end DESC,
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC			
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

            CreateStoredProcedure(name: "[dbo].[GetNhatKyTienCoc_OfDoiTuong]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                IDDonVis = p.String(),
                FromDate = p.DateTime(),
                ToDate = p.DateTime(),
                CurrentPage = p.Int(),
                PageSize = p.Int()
            }, body: @"SET NOCOUNT ON;
	
	declare @tblChiNhanh table(ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@IDDonVis)

	declare @LoaiDoiTuong int = (select LoaiDoiTuong from DM_DoiTuong where ID= @ID_DoiTuong)
	if @FromDate is null
		set @FromDate ='2020-01-01'
	if @ToDate is null
		set @ToDate = DATEADD(DAY,1,GETDATE())
	

	declare @tblDiary table(
	 ID_PhieuThu uniqueidentifier, MaPhieuThu nvarchar(max), NgayLapPhieuThu datetime,
		  ID_HoaDon uniqueidentifier, MaHoaDon nvarchar(max), GiaTri float, sLoaiHoaDon nvarchar(max), SoDu float
	)

	-- phieu naptiencoc
	insert into @tblDiary
	select * ,0
	from
	(
	select
		hd.ID as ID_PhieuThu,
		hd.MaHoaDon as MaPhieuThu,
		hd.NgayLapHoaDon,
		null as ID,
		'' as MaHoaDon,
		case when @LoaiDoiTuong= 2 then iif(hd.LoaiHoaDon=11,-hd.TongTienThu, hd.TongTienThu)
		else iif(hd.LoaiHoaDon=11,hd.TongTienThu, -hd.TongTienThu) end as GiaTri,
		case when hd.LoaiHoaDon= 11 then
			case when @LoaiDoiTuong= 2 then N'Chi trả cọc' else N'Nạp tiền cọc' end
		else
			case when @LoaiDoiTuong= 2 then N'Nạp tiền cọc' else N'Chi trả cọc' end
		end as sLoaiHoaDon			
	from Quy_HoaDon hd
	join Quy_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
	where ct.ID_DoiTuong like @ID_DoiTuong
	and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
	and hd.TrangThai='1'
	and ct.LoaiThanhToan= 1
	and exists(select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)

	union all
	-- sudung coc
	select
		hd.ID as ID_PhieuThu,
		hd.MaHoaDon as MaPhieuThu, 
		hd.NgayLapHoaDon, 
		hdsd.ID,
		hdsd.MaHoaDon,
		-sum(ct.TienThu) as GiaTri,
		case hdsd.LoaiHoaDon
			when 1 then N'Hóa đơn bán'
			when 4 then N'Nhập hàng'
			when 6 then N'Trả hàng'
			when 7 then N'Trả hàng nhạp'
			when 19 then N'Gói dịch vụ'
			when 25 then N'Hóa đơn sửa chữa'
	else '' end as sLoaiHoaDon
	from Quy_HoaDon hd
	join Quy_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
	join BH_HoaDon hdsd on ct.ID_HoaDonLienQuan = hdsd.ID
	where ct.ID_DoiTuong like @ID_DoiTuong
	and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
	and exists(select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
	and hd.TrangThai='1'
	and ct.HinhThucThanhToan= 6
	group by hd.ID ,
		hd.MaHoaDon ,
		hd.NgayLapHoaDon, 
		hdsd.ID,
		hdsd.MaHoaDon,
		hdsd.LoaiHoaDon
	) a 
	
	declare @ID_PhieuThu uniqueidentifier, @MaPhieuThu nvarchar(max), @NgayLapPhieuThu datetime,
		 @ID_HoaDon uniqueidentifier, @MaHoaDon nvarchar(max), @GiaTri float, @sLoaiHoaDon nvarchar(max), @SoDu float, @SoDuSauPhatSinh float
	
	set @SoDuSauPhatSinh =0
	declare _cur cursor
	for
	select *
	from @tblDiary tmp
	order by NgayLapPhieuThu 
	open _cur
	fetch next from _cur 
	into @ID_PhieuThu ,@MaPhieuThu , @NgayLapPhieuThu,
		  @ID_HoaDon, @MaHoaDon , @GiaTri, @sLoaiHoaDon , @SoDu
	while @@FETCH_STATUS =0
		begin							
			set @SoDuSauPhatSinh = @SoDuSauPhatSinh +  @GiaTri
			update @tblDiary set SoDu = @SoDuSauPhatSinh where ID_PhieuThu = @ID_PhieuThu
			
			FETCH NEXT FROM _cur 
			INTO @ID_PhieuThu ,@MaPhieuThu , @NgayLapPhieuThu,  @ID_HoaDon, @MaHoaDon , @GiaTri, @sLoaiHoaDon , @SoDu 		 
		end  

		close _cur
		deallocate _cur;

		with data_cte
		as
		(
		select * from @tblDiary
		),
		count_cte
		as (
    			select count(ID_PhieuThu) as TotalRow,
    				CEILING(COUNT(ID_PhieuThu) / CAST(@PageSize as float ))  as TotalPage    				
    			from data_cte
    		)
			-- do return class HD_QHD_QHDCT at C#
    		select 
				dt.ID_PhieuThu as ID_PhieuChi,
				dt.MaPhieuThu as MaPhieuChi,
				dt.NgayLapPhieuThu as NgayLapHoaDon,
				dt.MaHoaDon as MaHoaDonGoc,
				dt.ID_HoaDon as ID_HoaDonGoc,
				dt.GiaTri as TongTienHDTra,
				dt.SoDu,
				dt.sLoaiHoaDon,			
				cte.*
    		from data_cte dt    		
    		cross join count_cte cte
    		order by dt.NgayLapPhieuThu desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY");

            CreateStoredProcedure(name: "[dbo].[GetQuyChiTiet_byIDQuy]", parametersAction: p => new
            {
                ID = p.Guid()
            }, body: @"SET NOCOUNT ON;
   select qhd.id,  qhd.MaHoaDon, qhd.NguoiTao, qhd.LoaiHoaDon, qhd.TongTienThu, qhd.ID_NhanVien, qhd.NoiDungThu,
	   qct.TienMat, qct.TienGui, qct.TienThu, qct.DiemThanhToan, qct.ID_TaiKhoanNganHang, qct.ID_KhoanThuChi, 
	   qct.ID_DoiTuong,
	   qct.ID_HoaDonLienQuan,
	   qct.ID_NhanVien as ID_NhanVienCT, -- thu/chi cho NV nao
	   qct.HinhThucThanhToan,
	   cast(iif(qhd.LoaiHoaDon = 11,'1','0') as bit) as LaKhoanThu,
	   iif(qct.LoaiThanhToan = 1,1,0) as LaTienCoc,
	   isnull(hd.MaHoaDon,'') as MaHoaDonHD,
	   qhd.TongTienThu,
	   case dt.LoaiDoiTuong
		when 1 then 1
		when 2 then 3
		when 3 then 3
		else 0 end as LoaiDoiTuong,
		iif(qhd.TrangThai ='0', N'Đã hủy', N'Đã thanh toán') as GhiChu,	  
	   iif( hd.NgayLapHoaDon is null, qhd.NgayLapHoaDon, hd.NgayLapHoaDon) as NgayLapHoaDon,
	   case qct.HinhThucThanhToan
			when 1 then  N'Tiền mặt'
			when 2 then  N'POS'
			when 3 then  N'Chuyển khoản'
			when 4 then  N'Thu từ thẻ'
			when 5 then  N'Đổi điểm'
			when 6 then  N'Thu từ cọc'
		end as PhuongThuc	
   from Quy_HoaDon qhd
   join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
   left join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
   left join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
   where qhd.ID= @ID");

            CreateStoredProcedure(name: "[dbo].[GetSoDuDatCoc_ofDoiTuong]", parametersAction: p => new
            {
                ID_DoiTuong = p.Guid(),
                ID_DonVi = p.Guid()
            }, body: @"SET NOCOUNT ON;
	declare @LoaiDoiTuong int = (select LoaiDoiTuong from DM_DoiTuong where id= @ID_DoiTuong)
	declare @LoaiThuCoc int =11, @LoaiChiCoc int = 12
	if @LoaiDoiTuong = 2 
	begin
		set @LoaiThuCoc = 12
		set @LoaiChiCoc = 11 -- NCC tra lai tiencoc
	end

	select 		
		cast(sum(TongNap) as float) as TongThuTheGiaTri, 
		cast(sum(isnull(SuDung,0)) as float) as SuDungThe,
		cast(sum(TraCoc) as float) as HoanTraTheGiatri,
		cast(SUM(TongNap)  - SUM(isnull(SuDung,0)) - SUM(TraCoc) as float) as SoDuTheGiaTri
	from (
		-- tong nap (hay thucthu) tien coc
		SELECT 
			sum(qct.TienThu) as TongNap,
			0 as SuDung,
			0 as TraCoc
		FROM Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
		where qhd.LoaiHoaDon = @LoaiThuCoc and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and qct.ID_DoiTuong like @ID_DoiTuong 
		and qct.LoaiThanhToan = 1
		and qhd.ID_DonVi = @ID_DonVi	

		union all
			-- tra lai tiencoc
		SELECT 
			0 as TongNap,
			0 as SuDung,
			sum(qct.TienThu) as TraCoc
		FROM Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
		where qhd.LoaiHoaDon = @LoaiChiCoc and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and qct.ID_DoiTuong like @ID_DoiTuong 
		and qct.LoaiThanhToan = 1	
		and qhd.ID_DonVi = @ID_DonVi	

		union all
		-- su dung coc
		SELECT 
			0 as TongNap,
			SUM(qct.TienThu) as SuDungThe,
			0 as HoanTraTheGiatri
		FROM Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd ON qct.ID_HoaDon = qhd.ID		
		WHERE qct.ID_DoiTuong like @ID_DoiTuong 
		and qct.HinhThucThanhToan= 6
		and qhd.ID_DonVi = @ID_DonVi	
		and qhd.LoaiHoaDon = @LoaiThuCoc and (qhd.TrangThai = 1 or qhd.TrangThai is null)   	
	
		) tbl  ");

            Sql(@"ALTER FUNCTION [dbo].[TinhSoDuKHTheoThoiGian]
(
	@ID_DoiTuong [uniqueidentifier],
	@Time [datetime]
)
RETURNS FLOAT
AS
BEGIN
		DECLARE @SoDu AS FLOAT;		
		DECLARE @TongNap AS FLOAT;
		DECLARE @SuDungThe AS FLOAT;
		DECLARE @HoanTraTienThe AS FLOAT;
		DECLARE @TongDieuChinh AS FLOAT;

		SELECT @TongNap = SUM(TongTienHang) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @TongDieuChinh = SUM(TongChiPhi) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 23 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @SuDungThe = SUM(qhdct.TienThu) FROM Quy_HoaDon_ChiTiet qhdct
		INNER JOIN Quy_HoaDon qhd
		ON qhdct.ID_HoaDon = qhd.ID
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 11 and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and qhdct.HinhThucThanhToan=4

		SELECT @HoanTraTienThe = SUM(qhdct.TienThu) FROM Quy_HoaDon_ChiTiet qhdct
		INNER JOIN Quy_HoaDon qhd
		ON qhdct.ID_HoaDon = qhd.ID
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 12 and (qhd.TrangThai = 1 or qhd.TrangThai is null)
			and qhdct.HinhThucThanhToan=4

		SET @SoDu = ISNULL(@TongNap, 0) +  ISNULL(@TongDieuChinh, 0)  - ISNULL(@SuDungThe, 0) + ISNULL(@HoanTraTienThe,0);
	RETURN @SoDu
END");


            Sql(@"ALTER PROCEDURE [dbo].[GetHisChargeValueCard]
    @ID_DoiTuong [uniqueidentifier],
    @IDChiNhanhs [nvarchar](max),
	@FromDate datetime,
	@ToDate datetime,
	@CurrentPage int,
	@PageSize int
AS
BEGIN
    SET NOCOUNT ON;
	with data_cte
		as(
		select *, ISNULL(b.TienThu,0) as KhachDaTra
		from  
    		   (select hd.ID, MaHoaDon, LoaiHoaDon, NgayLapHoaDon, 			
    				TongChiPhi as MucNap,
    				TongChiPhi as ThanhTien,
    				ISNULL(TongTienThue,0) as SoDuSauNap ,
    				ISNULL(TongChietKhau,0) as KhuyenMaiVND,
    				ISNULL(hd.TongChietKhau,0) / ISNULL(IIF(hd.TongChiPhi=0,1,hd.TongChiPhi),1) * 100 as KhuyenMaiPT,
    				ISNULL(hd.TongGiamGia,0) / IIF(hd.TongChiPhi=0,1,hd.TongChiPhi) * 100 as ChietKhauPT,
    				TongTienHang as TongTienNap,
    				TongGiamGia as ChietKhauVND,
    				ISNULL(hd.DienGiai,'') as GhiChu,
    				hd.NguoiTao,
    				PhaiThanhToan, TenDoiTuong as TenKhachHang, DienThoai as SoDienThoai, TenDonVi
    			from BH_HoaDon hd
    			join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			join DM_DonVi dv on hd.ID_DonVi= dv.ID
    			where LoaiHoaDon in (22,23) and ChoThanhToan='0' and ID_DoiTuong= @ID_DoiTuong
				and hd.NgayLapHoaDon >=@FromDate and hd.NgayLapHoaDon <@ToDate
    			) a
    		left join (select qct.ID_HoaDonLienQuan, sum(qct.TienThu) as TienThu, MAX(qhd.MaHoaDon) as MaPhieuThu
    					from Quy_HoaDon_ChiTiet qct 
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    					where qct.ID_DoiTuong= @ID_DoiTuong
    					and qhd.TrangThai ='1'
    					group by qct.ID_HoaDonLienQuan) b
    		on a.ID= b.ID_HoaDonLienQuan
			),
			count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage				
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListChiTietHoaDonXuatFile]
    @IDHoaDon [uniqueidentifier]
AS
BEGIN
    SELECT 
    		dvqd.MaHangHoa,
    			TenHangHoa +
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as TenHangHoa,
    		--Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		--Case when DM_LoHang.MaLoHang is null then '' else '. Lô: ' + DM_LoHang.MaLoHang end as TenHangHoa,
			dvqd.TenDonViTinh,
			DM_LoHang.MaLoHang as MaLoHang, cthd.TienThue,
			iif(thanhtoan = 0 or ThanhToan is null, thanhtien, thanhtoan) as ThanhToan,
			SoLuong,ROUND(DonGia, 0) as DonGia,TienChietKhau as GiamGia,ThanhTien, cthd.GhiChu
    		FROM BH_HoaDon hd
    		JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    		JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    		JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
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
    					) as ThuocTinh on hh.ID = ThuocTinh.id_hanghoa
    		LEFT JOIN DM_LoHang ON cthd.ID_LoHang = DM_LoHang.ID
    		WHERE cthd.ID_HoaDon = @IDHoaDon
    		order by SoThuTu desc
END");


            Sql(@"ALTER PROCEDURE [dbo].[Search_DMHangHoaLoHang_TonKho]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON;
    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)

	set @MaHH= RTRIM(LTRIM(@MaHH))

	 DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@MaHH, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    DECLARE @tablename TABLE(
    Name [nvarchar](max))
  
    		SELECT Top(50) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHH as TenHangHoa,
    		dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    		a.QuanLyTheoLoHang,
			Case When @XemGiaVon != '1' or gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
    		CAST(ROUND(dvqd3.GiaBan, 0) as float) as GiaBan,  
    		CAST(ROUND(a.TonCuoiKy, 3) as float) as TonCuoiKy,
    		an.URLAnh as SrcImage,
    		Case when a.ID_LoHang is null then NEWID() else a.ID_LoHang end as ID_LoHang,
			a.MaLoHang,
    		a.MaLoHang as TenLoHang,
    		a.NgaySanXuat,
    		a.NgayHetHan,
    		a.TrangThai
			FROM 
    		 (
    		SELECT 
    		dvqd.ID,
    		dhh.TenHangHoa As TenHH,
    		Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end + ' ' + dhh.TenHangHoa  AS TenHangHoa,
    		Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end + ' ' +  dhh.TenHangHoa_KhongDau AS TenHangHoa_KhongDau,
    		Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end  + ' ' + dhh.TenHangHoa_KyTuDau AS TenHangHoa_KyTuDau,
    		dvqd.TenDonViTinh AS TenDonViTinh,
    		dhh.QuanLyTheoLoHang,
    		lh.ID  As ID_LoHang,
    		Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end As MaLoHang,
    		lh.NgaySanXuat  As NgaySanXuat,
    		lh.NgayHetHan  As NgayHetHan,
    		lh.TrangThai,
    		ISNULL(HangHoa.TonCuoiKy,0) AS TonCuoiKy
    		FROM 
    		DonViQuiDoi dvqd 
    		Inner join
    		(
    			Select ID_DonViQuyDoi As ID_DonViQuiDoi,
				Case when ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else  ID_LoHang end as ID_LoHang,
				TonKho As TonCuoiKy
				From DM_HangHoa_TonKho where ID_DonVi = @ID_ChiNhanh
    		) AS HangHoa
    		on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		Where dvqd.Xoa = '0' and dhh.TheoDoi = 1
    ) a
    	inner Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))    	
		left join DM_GiaVon gv on (dvqd3.ID = gv.ID_DonViQuiDoi and (a.ID_LoHang = gv.ID_LoHang or a.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    where  dvqd3.Xoa = '0'
			and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
			and  (a.TrangThai = 1 or a.TrangThai is null)
 

			AND ((select count(Name) from @tblSearchString b where 
    				a.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    				or a.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or a.TenHangHoa like '%'+b.Name+'%'
    				or a.MaLoHang like '%' +b.Name +'%' 
    				or dvqd3.MaHangHoa like '%'+b.Name+'%'
    
					)=@count or @count=0)
    order by a.NgayHetHan
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateGiaVonVer2]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanhInput [uniqueidentifier],
    @ThoiGian [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @ChiTietHoaDon TABLE (MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER);
    	INSERT INTO @ChiTietHoaDon
    	select hdcthd.MaHoaDon, @ThoiGian, hh.ID, hdctcthd.ID_LoHang, @IDChiNhanhInput FROM BH_HoaDon hdcthd
    	INNER JOIN BH_HoaDon_ChiTiet hdctcthd
    	ON hdcthd.ID = hdctcthd.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqd
    	ON hdctcthd.ID_DonViQuiDoi = dvqd.ID
    	INNER JOIN DM_HangHoa hh
    	on hh.ID = dvqd.ID_HangHoa
    	WHERE hdcthd.ID = @IDHoaDonInput
		GROUP BY hh.ID, hdctcthd.ID_LoHang, hdcthd.MaHoaDon;

    	DECLARE @ChiTietHoaDonUpdate TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @ChiTietHoaDonUpdate
		select * from 
		(
    	select hdupdate.ID as IDHoaDon, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID as ID_ChiTietHoaDon, 
    	CASE 
    		WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END AS NgayLapHoaDon, hdctupdate.SoThuTu, hdctupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, hdctupdate.TienChietKhau, hdctupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    	[dbo].[FUNC_TonLuyKeTruocThoiGian](cthdthemmoiupdate.ID_ChiNhanh, hhupdate.ID, hdctupdate.ID_LoHang, CASE
    		WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END) as TonKho, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi as GiaVon, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi as GiaVonNhan,
    	hhupdate.ID as ID_HangHoa, hhupdate.LaHangHoa, dvqdupdate.ID as IDDonViQuiDoi, hdctupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, hdupdate.ID_CheckIn, hdupdate.YeuCau FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    	ON hdupdate.ID = hdctupdate.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqdupdate
    	ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    	INNER JOIN DM_HangHoa hhupdate
    	on hhupdate.ID = dvqdupdate.ID_HangHoa
    	INNER JOIN @ChiTietHoaDon cthdthemmoiupdate
    	ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    	WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND hdupdate.LoaiHoaDon != 25 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    	((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon >= cthdthemmoiupdate.NgayLapHoaDon and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    	or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua >= cthdthemmoiupdate.NgayLapHoaDon))
		) as table1
    	order by NgayLapHoaDon, SoThuTu desc, LoaiHoaDon, MaHoaDon;

		--Begin TinhGiaVonTrungBinh
		DECLARE @TinhGiaVonTrungBinh BIT;
		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanhInput;
		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
		BEGIN
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu, hdct.SoLuong, hdct.DonGia, hd.TongTienHang, hdct.TienChietKhau, hdct.ThanhTien, hd.TongGiamGia, dvqd.TyLeChuyenDoi, 0, hdct.GiaVon / dvqd.TyLeChuyenDoi, 
    		hdct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi,
    		hh.ID, hh.LaHangHoa, hdct.ID_DonViQuiDoi, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, cthdthemmoi.ID_ChiNhanh, hd.ID_CheckIn, hd.YeuCau FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct
    		ON hd.ID = hdct.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqd
    		ON hdct.ID_DonViQuiDoi = dvqd.ID
    		INNER JOIN DM_HangHoa hh
    		on hh.ID = dvqd.ID_HangHoa
    		INNER JOIN @ChiTietHoaDon cthdthemmoi
    		ON cthdthemmoi.ID_HangHoa = hh.ID
    		WHERE hd.ChoThanhToan = 0 AND hd.LoaiHoaDon != 3 AND hd.LoaiHoaDon != 19 AND hd.LoaiHoaDon != 25 AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) AND
    		((hd.ID_DonVi = cthdthemmoi.ID_ChiNhanh and hd.NgayLapHoaDon < cthdthemmoi.NgayLapHoaDon and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    		or (hd.YeuCau = '4'  and hd.ID_CheckIn = cthdthemmoi.ID_ChiNhanh and hd.NgaySua < cthdthemmoi.NgayLapHoaDon))
    		order by NgayLapHoaDon desc, SoThuTu desc, hd.LoaiHoaDon, hd.MaHoaDon;
			
			--select * from @ChiTietHoaDonGiaVon 
			--select * from @ChiTietHoaDonUpdate -- trước

    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @BangUpdateGiaVon
    		SELECT *, ROW_NUMBER() OVER (PARTITION BY tableUpdateGiaVon.ID_HangHoa, tableUpdateGiaVon.ID_LoHang ORDER BY tableUpdateGiaVon.NgayLapHoaDon) AS RN FROM
    		(SELECT * FROM @ChiTietHoaDonUpdate
    		UNION ALL
    		SELECT cthdGiaVon.IDHoaDon, cthdGiaVon.IDHoaDonBan, cthdGiaVon.MaHoaDon, cthdGiaVon.LoaiHoaDon, cthdGiaVon.ID_ChiTietHoaDon, cthdGiaVon.NgayLapHoaDon, cthdGiaVon.SoThuThu, cthdGiaVon.SoLuong, cthdGiaVon.DonGia, cthdGiaVon.TongTienHang,
    		cthdGiaVon.ChietKhau, cthdGiaVon.ThanhTien, cthdGiaVon.TongGiamGia, cthdGiaVon.TyLeChuyenDoi, cthdGiaVon.TonKho, 
			CASE WHEN cthdGiaVon.GiaVon IS NULL
			THEN 0
			ELSE
			cthdGiaVon.GiaVon END AS GiaVon, 
			CASE WHEN cthdGiaVon.GiaVonNhan IS NULL
			THEN 0
			ELSE
			cthdGiaVon.GiaVonNhan END AS GiaVonNhan, cthd1.ID_HangHoa, cthdGiaVon.LaHangHoa, cthdGiaVon.IDDonViQuiDoi, cthd1.ID_LoHang , cthdGiaVon.ID_ChiTietDinhLuong,
    		cthd1.ID_ChiNhanh, cthdGiaVon.ID_CheckIn, cthdGiaVon.YeuCau FROM @ChiTietHoaDon cthd1
			LEFT JOIN (SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon1 WHERE cthdGiaVon1.RN = 1) AS cthdGiaVon
			ON cthd1.ID_HangHoa = cthdGiaVon.ID_HangHoa AND (cthd1.ID_LoHang = cthdGiaVon.ID_LoHang OR cthdGiaVon.ID_LoHang IS NULL)) AS tableUpdateGiaVon;
		
			DECLARE @TableTrungGianUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER,IDHangHoa UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, GiaVonNhapHang FLOAT, GiaVonNhanHang FLOAT)
			INSERT INTO @TableTrungGianUpDate
			SELECT IDHoaDon, ID_HangHoa, ID_LoHang, CASE WHEN MAX(TongTienHang) != 0 THEN SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) * (1-(MAX(TongGiamGia) / MAX(TongTienHang))) ELSE SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) END as GiaVonNhapHang,
			CASE WHEN LoaiHoaDon = 10 THEN SUM(ChietKhau * DonGia)/ SUM(IIF(ChietKhau = 0, 1, ChietKhau) * TyLeChuyenDoi) ELSE 0 END as GiaVonNhanHang
			FROM @BangUpdateGiaVon WHERE IDHoaDon = @IDHoaDonInput
			AND ID_HangHoa in (SELECT ID_HangHoa FROM @BangUpdateGiaVon WHERE IDHoaDon = @IDHoaDonInput AND RN = 1)
			GROUP BY ID_HangHoa, ID_LoHang, IDHoaDon,LoaiHoaDon
			
			DECLARE @RNCheck INT;
			SELECT @RNCheck = MAX(RN) FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang
			IF(@RNCheck = 1)
			BEGIN
				UPDATE @BangUpdateGiaVon SET RN = 2
			END

    		UPDATE bhctup SET bhctup.GiaVon = 
    		CASE
    			WHEN bhctup.LoaiHoaDon = 4 
				THEN giavontbup.GiaVonNhapHang
    			ELSE bhctup.GiaVon
    		END, bhctup.GiaVonNhan =
    		CASE
    			WHEN bhctup.LoaiHoaDon = 10 AND bhctup.YeuCau = '4' AND bhctup.ID_CheckIn = ID_ChiNhanhThemMoi THEN giavontbup.GiaVonNhanHang
    			ELSE bhctup.GiaVonNhan
    		END
			FROM @BangUpdateGiaVon bhctup
			JOIN @TableTrungGianUpDate giavontbup on bhctup.IDHoaDon =giavontbup.IDHoaDon and bhctup.ID_HangHoa = giavontbup.IDHangHoa and (bhctup.ID_LoHang = giavontbup.IDLoHang or giavontbup.IDLoHang is null)
    		WHERE bhctup.IDHoaDon = @IDHoaDonInput AND bhctup.RN = 1;
			--END tính giá vốn trung bình cho lần nhập hàng và chuyển hàng đầu tiền
    		DECLARE @GiaVonCapNhat TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, IDHoaDonCu UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, IDChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, GiaVonCu FLOAT, IDHangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, IDChiTietDinhLuong UNIQUEIDENTIFIER,
    		IDChiNhanhThemMoi UNIQUEIDENTIFIER, IDCheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @GiaVonCapNhat
    		SELECT tableUpdate.IDHoaDon, tableUpdate.IDHoaDonBan, tableGiaVon.IDHoaDon, tableUpdate.MaHoaDon, tableUpdate.LoaiHoaDon, tableUpdate.ID_ChiTietHoaDon,tableUpdate.NgayLapHoaDon, tableUpdate.SoLuong, tableUpdate.DonGia,
    		tableUpdate.TongTienHang, tableUpdate.ChietKhau, tableUpdate.ThanhTien, tableUpdate.TongGiamGia, tableUpdate.TyLeChuyenDoi, tableUpdate.TonKho, tableUpdate.GiaVon, tableUpdate.GiaVonNhan, tableGiaVon.GiaVon, tableUpdate.ID_HangHoa, tableUpdate.LaHangHoa,
    		tableUpdate.IDDonViQuiDoi, tableUpdate.ID_LoHang, tableUpdate.ID_ChiTietDinhLuong, tableUpdate.ID_ChiNhanhThemMoi, tableUpdate.ID_CheckIn, tableUpdate.YeuCau, tableUpdate.RN FROM @BangUpdateGiaVon tableUpdate
    		LEFT JOIN (SELECT (CASE WHEN ID_CheckIn = ID_ChiNhanhThemMoi THEN GiaVonNhan ELSE GiaVon END) AS GiaVon, ID_HangHoa, IDHoaDon, ID_LoHang, RN + 1 AS RN FROM @BangUpdateGiaVon) AS tableGiaVon
    		ON tableUpdate.ID_HangHoa = tableGiaVon.ID_HangHoa AND tableUpdate.RN = tableGiaVon.RN AND (tableUpdate.ID_LoHang = tableGiaVon.ID_LoHang OR tableUpdate.ID_LoHang IS NULL);
			
    		DECLARE @IDHoaDon UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonBan UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonCu UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX);
    		DECLARE @LoaiHoaDon INT;
    		DECLARE @IDChiTietHoaDon UNIQUEIDENTIFIER;
    		DECLARE @SoLuong FLOAT;
    		DECLARE @DonGia FLOAT;
    		DECLARE @TongTienHang FLOAT;
    		DECLARE @ChietKhau FLOAT;
			DECLARE @ThanhTien FLOAT;
    		DECLARE @TongGiamGia FLOAT;
    		DECLARE @TyLeChuyenDoi FLOAT;
    		DECLARE @TonKho FLOAT;
    		DECLARE @GiaVonCu FLOAT;
    		DECLARE @IDHangHoa UNIQUEIDENTIFIER;
    		DECLARE @IDDonViQuiDoi UNIQUEIDENTIFIER;
    		DECLARE @IDLoHang UNIQUEIDENTIFIER;
    		DECLARE @IDChiNhanhThemMoi UNIQUEIDENTIFIER;
    		DECLARE @IDCheckIn UNIQUEIDENTIFIER;
    		DECLARE @YeuCau NVARCHAR(MAX);
    		DECLARE @RN INT;
			DECLARE @GiaVon FLOAT;
			DECLARE @GiaVonNhan FLOAT;
    		DECLARE @GiaVonMoi FLOAT;
    		DECLARE @GiaVonCuUpdate FLOAT;
    		DECLARE @IDHangHoaUpdate UNIQUEIDENTIFIER;
    		DECLARE @IDLoHangUpdate UNIQUEIDENTIFIER;
			DECLARE @GiaVonHoaDonBan FLOAT;
    		DECLARE @TongTienHangDemo FLOAT;
    		DECLARE @SoLuongDemo FLOAT;
			DECLARE @ThanhTienDemo FLOAT;
			DECLARE @ChietKhauDemo FLOAT;
    		DECLARE CS_GiaVon CURSOR SCROLL LOCAL FOR SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
    		GiaVonCu, IDHangHoa, IDDonViQuiDoi, IDLoHang, IDChiNhanhThemMoi, IDCheckIn, YeuCau, RN, GiaVon, GiaVonNhan FROM @GiaVonCapNhat WHERE RN > 1 and LaHangHoa = 1
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau,@ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    		@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    			iF(@IDHangHoaUpdate = @IDHangHoa AND (@IDLoHangUpdate = @IDLoHang OR @IDLoHang IS NULL))
    			BEGIN
    				SET @GiaVonCu = @GiaVonCuUpdate;
    			END
    			ELSE
    			BEGIN
    				SET @IDHangHoaUpdate = @IDHangHoa;
    				SET @IDLoHangUpdate = @IDLoHang;
					SET @GiaVonCuUpdate = @GiaVonCu;
    			END
				IF(@GiaVonCu IS NOT NULL)
				BEGIN
    				IF(@LoaiHoaDon = 4)
    				BEGIN
						SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong  * (bhctdm.DonGia - bhctdm.ChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					--SELECT @TongTienHangDemo = SUM(bhctdm.ThanhTien - bhctdm.SoLuong * bhctdm.ChietKhau), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    					BEGIN
    						IF(@TongTienHang != 0)
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + (@TongTienHangDemo* (1-(@TongGiamGia/@TongTienHang))))/(@SoLuongDemo + @TonKho);
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo)/(@SoLuongDemo + @TonKho);
    						END
    					END
    					ELSE
    					BEGIN
					
    						IF(@TongTienHang != 0)
    						BEGIN
    							SET	@GiaVonMoi = (@TongTienHangDemo / @SoLuongDemo) * (1 - (@TongGiamGia / @TongTienHang));
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
    						END
    					END
    				END
    				ELSE IF (@LoaiHoaDon = 7)
    				BEGIN
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * bhctdm.DonGia * ( 1- bhctdm.TongGiamGia/iif(bhctdm.TongTienHang=0,1,bhctdm.TongTienHang))) ,
						@SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@TonKho - @SoLuongDemo > 0)
    					BEGIN
    						SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) - @TongTienHangDemo)/(@TonKho - @SoLuongDemo);
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    				END
    				ELSE IF(@LoaiHoaDon = 10)
    				BEGIN
    					SELECT @TongTienHangDemo = SUM(bhctdm.ChietKhau * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.ChietKhau * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang

    					IF(@YeuCau = '1' OR (@YeuCau = '4' AND @IDChiNhanhThemMoi != @IDCheckIn))
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    					ELSE IF (@YeuCau = '4' AND @IDChiNhanhThemMoi = @IDCheckIn)
    					BEGIN
    						IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    						BEGIN
    							SET @GiaVonMoi = (@GiaVonCu * @TonKho + @TongTienHangDemo)/(@TonKho + @SoLuongDemo);
    						END
    						ELSE
    						BEGIN
								IF(@SoLuongDemo = 0)
								BEGIN
									SET @GiaVonMoi = @GiaVonCu;
								END
								ELSE
								BEGIN
    								SET @GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
								END
    						END
    					END
    				END
    				ELSE IF (@LoaiHoaDon = 6)
    				BEGIN
    					SELECT @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@IDHoaDonBan IS NOT NULL)
    					BEGIN
    						SET @GiaVonHoaDonBan = -1;
    						SELECT @GiaVonHoaDonBan = GiaVon FROM @GiaVonCapNhat WHERE IDHoaDon = @IDHoaDonBan AND IDDonViQuiDoi = @IDDonViQuiDoi AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL);
						
    						IF(@GiaVonHoaDonBan = -1)
    						BEGIN
							
    							SELECT @GiaVonHoaDonBan = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDonBan AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
						
    						END
    						IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    						BEGIN
							
    							SET @GiaVonMoi = (@GiaVonCu * @TonKho + @GiaVonHoaDonBan * @SoLuongDemo) / (@TonKho + @SoLuongDemo);
    						END
    						ELSE
    						BEGIN
    							SET @GiaVonMoi = @GiaVonHoaDonBan;
    						END
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    				END
    				ELSE IF(@LoaiHoaDon = 18)
					BEGIN
						SELECT @GiaVonMoi = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDon AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
					END
					ELSE
    				BEGIN
    					SET @GiaVonMoi = @GiaVonCu;
    				END
				END
				ELSE
				BEGIN
					IF(@IDCheckIn = @IDChiNhanhThemMoi)
					BEGIN
						SET @GiaVonMoi = @GiaVonNhan
					END
					ELSE
					BEGIN
						SET @GiaVonMoi = @GiaVon
					END
				END

    			IF(@IDHoaDon = @IDHoaDonCu)
    			BEGIN

    				SET @GiaVonMoi = @GiaVonCuUpdate;	
    			END
    			ELSE
    			BEGIN
    				SET @GiaVonCuUpdate = @GiaVonMoi;
    			END
    			IF(@LoaiHoaDon = 10 AND @YeuCau = '4' AND @IDCheckIn = @IDChiNhanhThemMoi)
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVonNhan = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			ELSE
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVon = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon

    		--Update BH_HoaDon_ChiTiet
    		UPDATE hoadonchitiet1
    		SET hoadonchitiet1.GiaVon = _giavoncapnhat1.GiaVon * _giavoncapnhat1.TyLeChuyenDoi, hoadonchitiet1.GiaVon_NhanChuyenHang = _giavoncapnhat1.GiaVonNhan * _giavoncapnhat1.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet1
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat1
    		ON hoadonchitiet1.ID = _giavoncapnhat1.IDChiTietHoaDon
    		WHERE _giavoncapnhat1.LoaiHoaDon != 8 AND _giavoncapnhat1.LoaiHoaDon != 18 AND _giavoncapnhat1.LoaiHoaDon != 9 AND _giavoncapnhat1.RN > 1;

			UPDATE hoadonchitiet4
    		SET hoadonchitiet4.GiaVon = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi, hoadonchitiet4.ThanhToan = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi *hoadonchitiet4.SoLuong
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet4
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat4
    		ON hoadonchitiet4.ID = _giavoncapnhat4.IDChiTietHoaDon
    		WHERE _giavoncapnhat4.LoaiHoaDon = 9 AND _giavoncapnhat4.RN > 1;
    
    		UPDATE hoadonchitiet2
    		SET hoadonchitiet2.GiaVon = _giavoncapnhat2.GiaVon * _giavoncapnhat2.TyLeChuyenDoi, hoadonchitiet2.ThanhTien = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong* _giavoncapnhat2.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet2
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat2
    		ON hoadonchitiet2.ID = _giavoncapnhat2.IDChiTietHoaDon
    		WHERE _giavoncapnhat2.LoaiHoaDon = 8 AND _giavoncapnhat2.RN > 1;
    
    		UPDATE hoadonchitiet3
    		SET hoadonchitiet3.DonGia = ISNULL(_giavoncapnhat3.GiaVonCu,0) * _giavoncapnhat3.TyLeChuyenDoi, hoadonchitiet3.PTChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) ELSE 0 END,
    		hoadonchitiet3.TienChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN 0 ELSE hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) END
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet3
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat3
    		ON hoadonchitiet3.ID = _giavoncapnhat3.IDChiTietHoaDon
    		WHERE _giavoncapnhat3.LoaiHoaDon = 18 AND _giavoncapnhat3.RN > 1;
    
    		UPDATE chitietdinhluong
    		SET chitietdinhluong.GiaVon = gvDinhLuong.GiaVonDinhLuong / chitietdinhluong.SoLuong
    		FROM BH_HoaDon_ChiTiet AS chitietdinhluong
    		INNER JOIN
    		(SELECT SUM(ct.GiaVon * ct.SoLuong) AS GiaVonDinhLuong, ct.ID_ChiTietDinhLuong FROM BH_HoaDon_ChiTiet ct
    		INNER JOIN (SELECT IDChiTietDinhLuong FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDChiTietDinhLuong) gv
    		ON (ct.ID_ChiTietDinhLuong = gv.IDChiTietDinhLuong and ct.ID_ChiTietDinhLuong IS NOT NULL)
    		WHERE gv.IDChiTietDinhLuong IS NOT NULL AND ct.ID != ct.ID_ChiTietDinhLuong
    		GROUP BY ct.ID_ChiTietDinhLuong) AS gvDinhLuong
    		ON chitietdinhluong.ID = gvDinhLuong.ID_ChiTietDinhLuong
    		--END Update BH_HoaDon_ChiTiet
    		--Update DM_GiaVon
    		UPDATE _dmGiaVon1
    		SET _dmGiaVon1.GiaVon = ISNULL(_gvUpdateDM1.GiaVon, 0)
    		FROM (SELECT dvqd1.ID AS IDDonViQuiDoi, _giavon1.IDLoHang AS IDLoHang, (CASE WHEN _giavon1.IDCheckIn = _giavon1.IDChiNhanhThemMoi THEN _giavon1.GiaVonNhan ELSE _giavon1.GiaVon END) * dvqd1.TyLeChuyenDoi AS GiaVon, _giavon1.IDChiNhanhThemMoi AS IDChiNhanh FROM @GiaVonCapNhat _giavon1
    		INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN > 1 GROUP BY IDHangHoa,IDLoHang) AS _maxGiaVon1
    		ON _giavon1.IDHangHoa = _maxGiaVon1.IDHangHoa AND _giavon1.RN = _maxGiaVon1.RN AND (_giavon1.IDLoHang = _maxGiaVon1.IDLoHang OR _maxGiaVon1.IDLoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd1
    		ON dvqd1.ID_HangHoa = _giavon1.IDHangHoa) AS _gvUpdateDM1
    		LEFT JOIN DM_GiaVon _dmGiaVon1
    		ON _gvUpdateDM1.IDChiNhanh = _dmGiaVon1.ID_DonVi AND _gvUpdateDM1.IDDonViQuiDoi = _dmGiaVon1.ID_DonViQuiDoi AND (_gvUpdateDM1.IDLoHang = _dmGiaVon1.ID_LoHang OR _gvUpdateDM1.IDLoHang IS NULL)
    		WHERE _dmGiaVon1.ID IS NOT NULL;
    
    		INSERT INTO DM_GiaVon (ID, ID_DonVi, ID_DonViQuiDoi, ID_LoHang, GiaVon)
    		SELECT NEWID(), _gvUpdateDM.IDChiNhanh, _gvUpdateDM.IDDonViQuiDoi, _gvUpdateDM.IDLoHang, _gvUpdateDM.GiaVon FROM 
    		(SELECT dvqd2.ID AS IDDonViQuiDoi, _giavon2.IDLoHang AS IDLoHang, (CASE WHEN _giavon2.IDCheckIn = _giavon2.IDChiNhanhThemMoi THEN _giavon2.GiaVonNhan ELSE _giavon2.GiaVon END) * dvqd2.TyLeChuyenDoi AS GiaVon, _giavon2.IDChiNhanhThemMoi AS IDChiNhanh FROM @GiaVonCapNhat _giavon2
    		INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDHangHoa, IDLoHang) AS _maxGiaVon
    		ON _giavon2.IDHangHoa = _maxGiaVon.IDHangHoa AND _giavon2.RN = _maxGiaVon.RN AND (_giavon2.IDLoHang = _maxGiaVon.IDLoHang OR _maxGiaVon.IDLoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd2
    		ON dvqd2.ID_HangHoa = _giavon2.IDHangHoa) AS _gvUpdateDM
    		LEFT JOIN DM_GiaVon _dmGiaVon
    		ON _gvUpdateDM.IDChiNhanh = _dmGiaVon.ID_DonVi AND _gvUpdateDM.IDDonViQuiDoi = _dmGiaVon.ID_DonViQuiDoi AND (_gvUpdateDM.IDLoHang = _dmGiaVon.ID_LoHang OR _gvUpdateDM.IDLoHang IS NULL)
    		WHERE _dmGiaVon.ID IS NULL;
    		--End Update DM_GiaVon
		END
		--END TinhGiaVonTrungBinh
END");

            Sql(@"update Quy_HoaDon_ChiTiet set LoaiThanhToan= 0

--- 1.mat, 2.pos, 3.chuyenkhoan, 4.thegiatri, 5.diem
update ct2 set ct2.HinhThucThanhToan = ct1.HinhThucTT
from Quy_HoaDon_ChiTiet ct2
join(
select Mahoadon, ngaylaphoadon, ct.ID, 
case when DiemThanhToan > 0 then 5
else
	case when ThuTuThe > 0 then 4
	else 
case when TienThu = 0 then 1
	else
		case when TaiKhoanPOS is null then 1
		else iif(TaiKhoanPOS='1',2,3)
		end end end end as HinhThucTT,
		ct.TienMat, ct.TienGui, ct.TienThu, DiemThanhToan, ThuTuThe, ct.HinhThucThanhToan, ct.LoaiThanhToan, ChiPhiNganHang, LaPTChiPhiNganHang
from Quy_HoaDon_ChiTiet ct
join Quy_HoaDon hd on ct.ID_HoaDon= hd.ID
left join DM_TaiKhoanNganHang tk on ct.ID_TaiKhoanNganHang = tk.ID
) ct1 on ct2.ID= ct1.ID");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_ChiNhanh_v2]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_CongNo_v2]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoNam_v2]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoQuy_v2]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoThang_v2]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_SoQuy_v2]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_ThuChi_v2]");
            DropStoredProcedure("[dbo].[GetAllKhachHang_NotWhere]");
            DropStoredProcedure("[dbo].[GetList_HoaDonNhapHang]");
            DropStoredProcedure("[dbo].[GetNhatKyTienCoc_OfDoiTuong]");
            DropStoredProcedure("[dbo].[GetQuyChiTiet_byIDQuy]");
            DropStoredProcedure("[dbo].[GetSoDuDatCoc_ofDoiTuong]");
        }
    }
}