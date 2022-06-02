namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreProcedure_20180205 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietI]", parametersAction: p => new
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

	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
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
	    -- dhh.LaHangHoa,
		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
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
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				NULL AS SoLuongXuat,
				NULL AS GiaTriXuat,
				ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as TonKho
				FROM DonViQUiDoi dvqd
				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
				UNION ALL

				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
				SELECT 
				bhdct.ID_DonViQuiDoi,                                                                                                                                                                             
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                      
			) AS td
			GROUP BY td.ID_DonViQuiDoi, td.TonKho
    
			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
			UNION ALL
				SELECT
				pstk.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- xuất kho
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- trả hàng nhà cung cấp
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL

				-- Xuất hủy
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Xuất Chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập NCC
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập kiem ke
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập Khách trả hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietII]", parametersAction: p => new
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

	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
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
	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) as GiaTriCuoiKy
	FROM 
    (
		SELECT 
		dhh.ID,
		dhh.ID_NhomHang,
		MAX(dhh.TenHangHoa) AS TenHangHoa,
		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
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
			SUM(ISNULL(td.TonKho + td.SoLuongXuat_Ban + td.SoLuongXuat_Huy + td.SoLuongXuat_NCC + td.SoLuongXuat_Kiem + td.SoLuongXuat_Chuyen   - td.SoLuongNhap_NCC - td.SoLuongNhap_Kiem - td.SoLuongNhap_Tra - td.SoLuongNhap_Chuyen, 0)) AS TonDauKy,
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
				ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as TonKho

				FROM DonViQUiDoi dvqd
				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
				UNION ALL

				-- phát sinh xuất nhập tồn từ thời gian bắt đầu tới khi chốt sổ
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- xuất kho
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- trả hàng nhà cung cấp
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL

				-- Xuất hủy
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Xuất Chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập NCC
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập kiem ke
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập Khách trả hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                          
			) AS td
			GROUP BY td.ID_DonViQuiDoi, td.TonKho
    
			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
			UNION ALL
				 SELECT
				pstk.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- xuất kho
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- trả hàng nhà cung cấp
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL

				-- Xuất hủy
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Xuất Chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập NCC
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập kiem ke
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập Khách trả hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietIII]", parametersAction: p => new
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

	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
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
	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) as GiaTriCuoiKy
	FROM 
    (
		SELECT 
		dhh.ID,
		dhh.ID_NhomHang,
		MAX(dhh.TenHangHoa) AS TenHangHoa,
		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
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
				ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as TonKho

				FROM DonViQUiDoi dvqd
				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
				UNION ALL

				-- phát sinh xuất nhập tồn từ thời gian kết thúc tới khi chốt sổ
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- xuất kho
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- trả hàng nhà cung cấp
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL

				-- Xuất hủy
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Xuất Chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập NCC
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập kiem ke
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập Khách trả hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                          
			) AS td
			GROUP BY td.ID_DonViQuiDoi, td.TonKho
    
			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
			UNION ALL
				SELECT
				pstk.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- xuất kho
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- trả hàng nhà cung cấp
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL

				-- Xuất hủy
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Xuất Chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập NCC
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập kiem ke
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập Khách trả hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietIV]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"-- lấy ngày chốt sổ
	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
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
	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) as GiaTriCuoiKy
	FROM 
    (
		SELECT 
		dhh.ID,
		dhh.ID_NhomHang,
		MAX(dhh.TenHangHoa) AS TenHangHoa,
		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonDauKy,
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
				-- phát sinh xuất nhập tồn từ thời gian kết thúc tới khi chốt sổ
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- xuất kho
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- trả hàng nhà cung cấp
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL

				-- Xuất hủy
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Xuất Chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập NCC
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập kiem ke
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập Khách trả hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon < @timeStart
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                          
			) AS td
			GROUP BY td.ID_DonViQuiDoi, td.TonKho
    
			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
			UNION ALL
				SELECT
				pstk.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- xuất kho
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- trả hàng nhà cung cấp
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL

				-- Xuất hủy
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Xuất Chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập NCC
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập kiem ke
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập Khách trả hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
				UNION ALL
				-- Nhập chuyển hàng
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonI]", parametersAction: p => new
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

	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap,
	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat,
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
	    -- dhh.LaHangHoa,
		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
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
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				NULL AS SoLuongXuat,
				NULL AS GiaTriXuat,
				ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as TonKho
				FROM DonViQUiDoi dvqd
				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
				--WHERE (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH) --and (nhh.ID like @ID_NhomHangHoa or nhh.ID_Parent like @ID_NhomHangHoa) 
				UNION ALL

				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
				SUM(pstk.GiaTriXuat) AS GiaTriXuat,
				NULL AS TonKho
				FROM 
				(
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
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
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"-- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);

	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap,
	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat,
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
				NULL AS SoLuongNhap1,
				NULL AS GiaTriNhap1,
				NULL AS SoLuongXuat1,
				NULL AS GiaTriXuat1,
				NULL AS SoLuongNhap2,
				NULL AS GiaTriNhap2,
				NULL AS SoLuongXuat2,
				NULL AS GiaTriXuat2,
				ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as TonKho
				FROM DonViQUiDoi dvqd
				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
				UNION ALL

				-- phát sinh xuất nhập tồn từ thời gian bắt đầu đến khi chốt sổ
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				GROUP BY bhdct.ID_DonViQuiDoi
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				GROUP BY bhdct.ID_DonViQuiDoi

				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap1,
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
				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				GROUP BY bhdct.ID_DonViQuiDoi
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
				GROUP BY bhdct.ID_DonViQuiDoi

				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				AND bhd.ID_DonVi = @ID_ChiNhanh
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				GROUP BY bhdct.ID_DonViQuiDoi
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
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
				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				GROUP BY bhdct.ID_DonViQuiDoi

				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap1,
				NULL AS GiaTriNhap1,
				NULL AS SoLuongXuat1,
				NULL AS GiaTriXuat1,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap2,
				null AS SoLuongXuat2,
				NULL AS GiaTriXuat2,
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
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
				GROUP BY bhdct.ID_DonViQuiDoi
			) AS td
			GROUP BY td.ID_DonViQuiDoi, td.TonKho
		) 
		AS HangHoa
		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
		where dhh.LaHangHoa like @LaHangHoa
		GROUP BY dhh.ID , dhh.ID_NhomHang
    ) a
	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonIII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
 -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar

	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap,
	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat,
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
			--NULL as TonDau,
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
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				NULL AS SoLuongXuat,
				NULL AS GiaTriXuat,
				ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as TonKho
				FROM DonViQUiDoi dvqd
				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
				UNION ALL

				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
				GROUP BY bhdct.ID_DonViQuiDoi
			) AS td
			GROUP BY td.ID_DonViQuiDoi, td.TonKho
    
			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
			UNION ALL
				SELECT
				pstk.ID_DonViQuiDoi,
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
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
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
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
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
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
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
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");


            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonIV]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"--DECLARE @timeChotSo Datetime
 --   Select @timeChotSo =  (select convert(datetime, '2018/01/01'))
 -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar

	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) as GiaTriDauKy,
	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap,
	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat,
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
	    -- dhh.LaHangHoa,
		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
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
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat
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
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS GiaTriXuat
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat
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
				NULL AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
				SUM(pstk.GiaTriXuat) AS GiaTriXuat
				FROM 
				(
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
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
				NULL AS GiaTriNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
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
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat
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
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat
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
				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
				null AS SoLuongXuat,
				NULL AS GiaTriXuat
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    order by TonCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoI]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaKH = p.String(),
                LoaiKH = p.Int()
            }, body: @"DECLARE @timeChotSo Datetime
	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);

	  --SELECT HangHoa.ID_KhachHang as ID_DoiTuong, dt.MaDoiTuong, dt.TenDoiTuong,
	  SELECT a.ID_KhachHang, 
	  dt.MaDoiTuong AS MaKhachHang, 
	  dt.TenDoiTuong AS TenKhachHang,
	  CAST(ROUND(a.NoDauKy , 0) as float) as NoDauKy,
	  CAST(ROUND(a.GhiNo , 0) as float) as GhiNo,
	  CAST(ROUND(a.GhiCo , 0) as float) as GhiCo,
	  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
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
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    		)AS HangHoa
			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
				GROUP BY HangHoa.ID_KhachHang
				) a
				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
				and dt.loaidoituong = @loaiKH
				ORDER BY NoCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoII]", parametersAction: p => new
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
	  CAST(ROUND(a.NoCuoiKy, 0) as float) as NoCuoiKy
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
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
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
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
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
				GROUP BY HangHoa.ID_KhachHang
				) a
				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
				and dt.loaidoituong = @loaiKH
				ORDER BY NoCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoIII]", parametersAction: p => new
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
	  CAST(ROUND(a.NoCuoiKy, 0) as float) as NoCuoiKy
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
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
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
				GROUP BY HangHoa.ID_KhachHang
				) a
				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
				and dt.loaidoituong = @loaiKH
				ORDER BY NoCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[ReportKhachHang_CongNoIV]", parametersAction: p => new
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
	  CAST(ROUND(a.NoCuoiKy, 0) as float) as NoCuoiKy
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
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
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
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
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhd.TongTienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai not like 'false' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
				GROUP BY HangHoa.ID_KhachHang
				) a
				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
				where (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH)
				and dt.loaidoituong = @loaiKH
				ORDER BY NoCuoiKy desc");

            CreateStoredProcedure(name: "[dbo].[GetListHHSearch]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                Search = p.String()
            }, body: @"SELECT Top(20) a.ID,dhh3.TenHangHoa,dvqd3.GiaVon,dvqd3.ID as ID_DonViQuiDoi,
dvqd3.MaHangHoa,dvqd3.TenDonViTinh, ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi),3) as TonKho FROM 
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    AND bhd.ID_DonVi = @ID_ChiNhanh
	--AND dvqd.MaHangHoa like @Search
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
	--AND dvqd.MaHangHoa like @Search
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
	--AND dvqd.MaHangHoa like @Search
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
    AND bhd.ID_DonVi = @ID_ChiNhanh
	--AND dvqd.MaHangHoa like @Search
    GROUP BY bhdct.ID_DonViQuiDoi
    ) AS td
    GROUP BY td.ID_DonViQuiDoi
    ) 
    AS HangHoa
    --LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    GROUP BY dhh.ID
    ) a
	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
	LEFT Join DM_HangHoa dhh3 on a.ID = dhh3.ID
	LEFT Join DM_NhomHangHoa nhh3 on dhh3.ID_NhomHang = nhh3.ID
	where dvqd3.ladonvichuan = 1 and dhh3.TenHangHoa like @Search or dhh3.TenHangHoa_KhongDau like @Search or dhh3.TenHangHoa_KyTuDau like @Search or dvqd3.MaHangHoa like @Search
    order by MaHangHoa");

            CreateStoredProcedure(name: "[dbo].[LoadDanhMucHangHoa]", parametersAction: p => new
            {
                MaHH = p.String(),
                ListID_NhomHang = p.String(),
                ID_ChiNhanh = p.Guid(),
                KinhDoanhFilter = p.String(),
                LaHangHoaFilter = p.String()
            }, body: @"DECLARE @timeStart Datetime
 DECLARE @SQL VARCHAR(254)
if (@ListID_NhomHang = '%%')
BEGIN
 Set @timeStart =  (select convert(datetime, '2018/01/01'))
 Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 if (@SQL > 0)
 BEGiN
   Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 END
 Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.LaHangHoa,aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
 (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho FROM (
 (select dvqd.ID,hh.ID as ID_HangHoa,hh.ID_HangHoaCungLoai,hh.LaHangHoa, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
 from DonViQUiDoi dvqd
 left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
 left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
 left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
 where (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH) and dvqd.xoa is null and dvqd.ladonvichuan = '1'
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
    AND bhd.ID_DonVi = @ID_ChiNhanh
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
END
ELSE
BEGIN

 Set @timeStart =  (select convert(datetime, '2018/01/01'))
 Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 if (@SQL > 0)
 BEGiN
   Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 END
 Select aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.LaHangHoa,aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
 (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho FROM (
 (select dvqd.ID,hh.ID as ID_HangHoa,hh.ID_HangHoaCungLoai,hh.LaHangHoa, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
 from DonViQUiDoi dvqd
 left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
 left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
 left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
 where (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH) and dvqd.xoa is null and dvqd.ladonvichuan = '1' and nhh.id=(select * from splitstring(@ListID_NhomHang) where name like nhh.ID)) aa
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
    AND bhd.ID_DonVi = @ID_ChiNhanh
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
END");

            CreateStoredProcedure(name: "[dbo].[LoadFirstDanhMucHangHoa]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                currentPage = p.Int(),
                pageSize = p.Int()
            }, body: @"DECLARE @timeStart Datetime
 DECLARE @SQL VARCHAR(254)
 Set @timeStart =  (select convert(datetime, '2018/01/01'))
 Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 if (@SQL > 0)
 BEGiN
   Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 END
  CREATE TABLE #IDCungLoaiTable (
  Count int,
  ID_HangHoaCungLoai [uniqueidentifier]
 )
INSERT INTO #IDCungLoaiTable
 select count(ID_HangHoaCungLoai) as Count, ID_HangHoaCungLoai from dm_hanghoa
 group by ID_HangHoaCungLoai

 Select ROW_NUMBER() OVER (ORDER BY aa.NgayTao desc) AS RowNum, aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.LaHangHoa,aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
 (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho FROM (
 (select ROW_NUMBER() OVER (ORDER BY hh.NgayTao desc) AS RowNum, dvqd.ID,hh.ID as ID_HangHoa,tableIDCL.ID_HangHoaCungLoai,hh.LaHangHoa, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
 from #IDCungLoaiTable tableIDCL
 left join DM_HangHoa hh on tableIDCL.ID_HangHoaCungLoai = hh.ID_HangHoaCungLoai
 left join DonViQuiDoi dvqd on hh.ID = dvqd.ID_hangHoa
 left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
 left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
 where dvqd.xoa is null and dvqd.ladonvichuan = '1') aa
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
    AND bhd.ID_DonVi = @ID_ChiNhanh
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
--	Where RowNum >= 1 AND RowNum <20
	Where RowNum >= (@currentPage * @pageSize) + 1 AND RowNum <= (@currentPage * @pageSize) + @pageSize");

            CreateStoredProcedure(name: "[dbo].[LoadFirstPageCountHH]", body: @"select ID_HangHoa as ID from DonViQuiDoi dvqd where dvqd.Xoa is null and dvqd.LaDonViChuan = 1");

            CreateStoredProcedure(name: "[dbo].[TinhSLTon]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                ID_HangHoa = p.Guid()
            }, body: @"SELECT (a.TonCuoiKy / dvqd3.TyLeChuyenDoi) as TonKho FROM 
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_ChiNhanh
	And dvqd.ID_HangHoa = @ID_HangHoa
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
    AND bhd.NgayLapHoaDon < @timeEnd
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
    AND bhd.NgayLapHoaDon < @timeEnd
	AND dvqd.ID_HangHoa = @ID_HangHoa
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
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY bhdct.ID_DonViQuiDoi
    ) AS td
    GROUP BY td.ID_DonViQuiDoi
    ) 
    AS HangHoa
    --LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    GROUP BY dhh.ID
    ) a
	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
	where dvqd3.ladonvichuan = 1 --and dvqd3.ID_HangHoa = @ID_HangHoa
    order by MaHangHoa");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonChiTietI]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonChiTietII]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonChiTietIII]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonChiTietIV]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonI]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonII]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonIII]");
            DropStoredProcedure("[dbo].[ReportHangHoa_XuatNhapTonIV]");
            DropStoredProcedure("[dbo].[ReportKhachHang_CongNoI]");
            DropStoredProcedure("[dbo].[ReportKhachHang_CongNoII]");
            DropStoredProcedure("[dbo].[ReportKhachHang_CongNoIII]");
            DropStoredProcedure("[dbo].[ReportKhachHang_CongNoIV]");
            DropStoredProcedure("[dbo].[GetListHHSearch]");
            DropStoredProcedure("[dbo].[LoadDanhMucHangHoa]");
            DropStoredProcedure("[dbo].[LoadFirstDanhMucHangHoa]");
            DropStoredProcedure("[dbo].[LoadFirstPageCountHH]");
            DropStoredProcedure("[dbo].[TinhSLTon]");
        }
    }
}
