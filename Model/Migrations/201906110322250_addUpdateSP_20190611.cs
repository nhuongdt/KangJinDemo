namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20190611 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonChiTietIV]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
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
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonDauKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriDauKy,
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
    		Case When @XemGiaVon = '1' then CAST(ROUND(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0), 0) as float) else 0 end as GiaTriCuoiKy
    	FROM
    	(
    SELECT 
    		dvqd3.ID as ID_DonViQuiDoi,
    		a.ID_LoHang,
    		a.TenNhomHangHoa,
    		dvqd3.mahanghoa, 
    	a.TenHangHoa + dvqd3.ThuocTinhGiaTri as TenHangHoaFull,
    	a.TenHangHoa,
    	dvqd3.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
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
    		dhh.ID_NhomHang as ID_NhomHang,
    		MAX(dnhh.TenNhomHangHoa) as TenNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa_KhongDau) as TenNhomHangHoa_KhongDau,
    		MAX(dnhh.TenNhomHangHoa_KyTuDau) as TenNhomHangHoa_KyTuDau,
    		dvqd.Xoa as Xoa,
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
					-- Xuất kiểm kê
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
					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				--AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
					AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					and bhdct.SoLuong > 0
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
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
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    				UNION ALL
					-- Xuất kiem ke
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
					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) *(-1) AS SoLuongXuat_Kiem,
    				NULL AS SoLuongXuat_Chuyen,
    				NULL AS SoLuongXuat_SX,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					And bhdct.SoLuong < 0
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
					AND bhdct.SoLuong > 0
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
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
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = '0' and bhd.ChoThanhToan = 'false'
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
    				where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    	) tr
    left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    	Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, gv.ID, gv.GiaVon,tr.TonCuoiKy,tr.TonDauKy
    	order by GiaTriCuoiKy desc
END");
        }
        
        public override void Down()
        {
        }
    }
}
