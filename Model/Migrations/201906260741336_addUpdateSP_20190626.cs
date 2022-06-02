namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20190626 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[FUNC_TonLuyKeTruocThoiGian]
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
	DECLARE @timeStartCS DATETIME;
	Set @timeStartCS =  (select convert(datetime, '2016/01/01'))
	DECLARE @SQL VARCHAR(254)
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStartCS =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END	

	SET @TonKho = 0;
	SELECT TOP(1) @TonKho = CASE WHEN @ID_ChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' THEN bhct.TonLuyKe_NhanChuyenHang ELSE bhct.TonLuyKe END FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	 where hd.LoaiHoaDon != 3 AND hd.LoaiHoaDon != 19 and dvqd.ID_HangHoa = @ID_HangHoa and (bhct.ID_LoHang = @ID_LoHang or @ID_LoHang is null) and 
	 ((hd.ID_DonVi = @ID_ChiNhanh and hd.NgayLapHoaDon < @TimeStart and hd.NgayLapHoaDon > @timeStartCS and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
     or (hd.YeuCau = '4'  and hd.ID_CheckIn = @ID_ChiNhanh and hd.NgaySua < @TimeStart and hd.NgaySua > @timeStartCS)) and ChoThanhToan = 0
	 order by NgayLapHoaDon desc

	RETURN @TonKho;
END");

            CreateStoredProcedure(name: "[dbo].[SP_GetHoaDonThanhToanNhaBep]", parametersAction: p => new
            {
                LoaiHoaDon = p.Int(),
                ID_DonVi = p.String()
            }, body: @"SET NOCOUNT ON;
    select 
    	hd.ID, hd.ID_DoiTuong, hd.SoLuongKhachHang, hd.ID_BangGia, hd.ID_NhanVien, hd.ID_ViTri, hd.ID_DonVi, hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienThue, hd.TongGiamGia, hd.TongChietKhau, 
    	hd.TongChiPhi, hd.PhaiThanhToan, hd.TongTienHang, ISNULL(hd.ChoThanhToan,'0') as ChoThanhToan,
    	hd.DienGiai,CAST(ISNULL(hd.KhuyeMai_GiamGia, 0) as float) as KhuyeMai_GiamGia , ISNULL(hd.YeuCau,'') as YeuCau, hd.LoaiHoaDon,
    	dv.TenDonVi, ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, ISNULL(nv.TenNhanVien,'') as TenNhanVien,
    	ISNULL(vt.TenViTri,'') as TenViTri, ISNULL(gb.TenGiaBan, N'Bảng giá chung') as TenGiaBan,
    	ISNULL(dt.DienThoai,'') as DienThoai, ISNULL(dt.Email,'') as Email
    from BH_HoaDon hd
    left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    left join DM_DonVi dv on hd.ID_DonVi= dv.ID
    left join DM_ViTri vt on hd.ID_ViTri= vt.ID
    left join DM_GiaBan gb on  hd.ID_BangGia= gb.ID
    left join Quy_HoaDon_ChiTiet qct on hd.ID_HoaDon= qct.ID_HoaDonLienQuan
    where hd.LoaiHoaDon like @LoaiHoaDon and hd.ID_DonVi like @ID_DonVi
	and UPPER(SUBSTRING(hd.MaHoaDon,1,2)) Like 'DH'");

            CreateStoredProcedure(name: "[dbo].[UpdateTonLuyKeTheoHoaDon]", parametersAction: p => new
            {
                IDHoaDonInput = p.Guid(),
                IDChiNhanhInput = p.Guid(),
                ThoiGian = p.DateTime(),
                Loai = p.Int()
            }, body: @"SET NOCOUNT ON;

	--DECLARE @IDHoaDonInput UNIQUEIDENTIFIER;
	--DECLARE @IDChiNhanhInput UNIQUEIDENTIFIER;
	--DECLARE @ThoiGian DATETIME;
	--SET @IDHoaDonInput = '642A2708-A7D0-4B33-AA97-E71BB74F2A3B';
	--SET @IDChiNhanhInput = 'D78CF00C-78E2-4832-A33B-4B2FECEA1BBB';
	--SET @ThoiGian = '2019-06-25 16:39:53.263';
	DECLARE @ThoiGianHD DATETIME;
	DECLARE @LoaiHoaDonTruyenVao INT;
	SELECT @ThoiGianHD = NgayLapHoaDon, @LoaiHoaDonTruyenVao = LoaiHoaDon FROM BH_HoaDon where ID = @IDHoaDonInput

	IF(@Loai = 1)
	BEGIN
		--- Lấy ra danh sách chi tiết của Hóa đơn truyền vào
		DECLARE @ChiTietHoaDon TABLE (IDHoaDon UNIQUEIDENTIFIER,ID_ChiTietHoaDon UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, TonKho FLOAT, LoaiHoaDon INT, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TyLeChuyenDoi FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT);
		INSERT INTO @ChiTietHoaDon
		select hdcthd.ID ,hdctcthd.ID, hdcthd.MaHoaDon, @ThoiGian, hh.ID, hdctcthd.ID_LoHang, @IDChiNhanhInput, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput, hh.ID, hdctcthd.ID_LoHang, @ThoiGian),
		hdcthd.LoaiHoaDon, hdcthd.ID_CheckIn, hdcthd.YeuCau, hdctcthd.SoLuong, dvqd.TyLeChuyenDoi, hdctcthd.TienChietKhau, hdctcthd.ThanhTien FROM BH_HoaDon hdcthd
		INNER JOIN BH_HoaDon_ChiTiet hdctcthd
		ON hdcthd.ID = hdctcthd.ID_HoaDon
		INNER JOIN DonViQuiDoi dvqd
		ON hdctcthd.ID_DonViQuiDoi = dvqd.ID
		INNER JOIN DM_HangHoa hh
		on hh.ID = dvqd.ID_HangHoa
		WHERE hdcthd.ID = @IDHoaDonInput

		DECLARE @TonChuanHDTruyenVaoChoPhieuTruyen TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
		INSERT INTO @TonChuanHDTruyenVaoChoPhieuTruyen
		SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
		CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE SUM(TienChietKhau * TyLeChuyenDoi) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDon GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau

		DECLARE @TonChuanHDTruyenVao TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
		INSERT INTO @TonChuanHDTruyenVao
		SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
		CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, 
		CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '1' and ID_ChiNhanh != ID_CheckIn, SUM(TienChietKhau * TyLeChuyenDoi) ,
		IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '4' and ID_ChiNhanh != ID_CheckIn,SUM(TienChietKhau * TyLeChuyenDoi) - SUM(SoLuong * TyLeChuyenDoi), SUM(TienChietKhau * TyLeChuyenDoi))) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDon GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau
		
		DECLARE @TonLuyKeTruyenVao TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
		INSERT INTO @TonLuyKeTruyenVao
		SELECT *, IIF(LoaiHoaDon IN (1, 5, 7, 8), TonKho - SoLuong, 
		IIF(LoaiHoaDon IN (4, 6), TonKho + SoLuong, IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho - TienChietKhau, 
		IIF(LoaiHoaDon = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho + TienChietKhau, 
		IIF(LoaiHoaDon = 9, ThanhTien, TonKho))))) AS TonLuyKe FROM @TonChuanHDTruyenVaoChoPhieuTruyen
		
		DECLARE @TonLuyKeUpdateTruyenVao TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
		INSERT INTO @TonLuyKeUpdateTruyenVao
		SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanh, cthdud.YeuCau FROM @TonLuyKeTruyenVao tlk
		INNER JOIN @ChiTietHoaDon cthdud
		ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanh AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

		UPDATE hdct
		SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
		FROM BH_HoaDon_ChiTiet hdct
		INNER JOIN @TonLuyKeUpdateTruyenVao tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet

		--- Lấy ra all chi tiết liên quan đến chi tiết hóa đơn truyền vào
		DECLARE @ChiTietHoaDonUpdate TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @ChiTietHoaDonUpdate
    	select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    	CASE 
    		WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    	CASE 
    		WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    		THEN
    			hdctupdate.TonLuyKe_NhanChuyenHang
    		ELSE
    			hdctupdate.TonLuyKe
    	END AS TonKho, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    	hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    	ON hdupdate.ID = hdctupdate.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqdupdate
    	ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    	INNER JOIN DM_HangHoa hhupdate
    	on hhupdate.ID = dvqdupdate.ID_HangHoa
    	INNER JOIN @TonChuanHDTruyenVao cthdthemmoiupdate
    	ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    	WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    	((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > cthdthemmoiupdate.NgayLapHoaDon and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    	or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > cthdthemmoiupdate.NgayLapHoaDon))
    	order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

		---Lấy ra những chi tiết của loaiHoaDon = 9 (Kiểm kê)

		DECLARE @ChiTietHoaDonUpdateKiemKe TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @ChiTietHoaDonUpdateKiemKe
    	select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    	CASE 
    		WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    	hdctupdate.TonLuyKe, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    	hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    	ON hdupdate.ID = hdctupdate.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqdupdate
    	ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    	INNER JOIN DM_HangHoa hhupdate
    	on hhupdate.ID = dvqdupdate.ID_HangHoa
    	INNER JOIN @TonChuanHDTruyenVao cthdthemmoiupdate
    	ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    	WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    	((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > cthdthemmoiupdate.NgayLapHoaDon and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    	or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > cthdthemmoiupdate.NgayLapHoaDon))
    	order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

		DECLARE @ChiTietHoaDonCanUpdate TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @ChiTietHoaDonCanUpdate
		SELECT cthdup.IDHoaDon, cthdup.IDHoaDonBan, cthdup.MaHoaDon, cthdup.LoaiHoaDon, cthdup.ID_ChiTietHoaDon, 
    	cthdup.NgayLapHoaDon, cthdup.SoThuTu, cthdup.SoLuong, cthdup.DonGia, cthdup.TongTienHang, cthdup.ChietKhau, cthdup.ThanhTien, cthdup.TongGiamGia, cthdup.TyLeChuyenDoi,
    	cthdup.TonKho, cthdup.GiaVon, cthdup.GiaVonNhan,
    	cthdup.ID_HangHoa, cthdup.LaHangHoa, cthdup.IDDonViQuiDoi, cthdup.ID_LoHang, cthdup.ID_ChiTietDinhLuong, cthdup.ID_ChiNhanhThemMoi, cthdup.ID_CheckIn, cthdup.YeuCau FROM @ChiTietHoaDonUpdate cthdup
		LEFT JOIN @ChiTietHoaDonUpdateKiemKe ctkiemke on cthdup.ID_HangHoa = ctkiemke.ID_HangHoa and (cthdup.ID_LoHang = ctkiemke.ID_LoHang or cthdup.ID_LoHang is null)
		WHERE (cthdup.NgayLapHoaDon < ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null)
	
		DECLARE @TonChuan TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
		INSERT INTO @TonChuan
		SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi AS ID_ChiNhanh, ID_CheckIn, YeuCau,
		MAX(SoLuong) AS SoLuong, MAX(TonKho) AS TonKho, MAX(ChietKhau) AS TienChietKhau, MAX(ThanhTien) AS ThanhTien FROM @ChiTietHoaDonCanUpdate GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau

		DECLARE @TonLuyKe TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
		INSERT INTO @TonLuyKe
		SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho - SoLuong, 
		IIF(@LoaiHoaDonTruyenVao IN (4, 6, 9), TonKho + SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho - TienChietKhau, 
		IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho + TienChietKhau, TonKho)))) AS TonLuyKe FROM @TonChuan

		DECLARE @TonLuyKeUpdate TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
		INSERT INTO @TonLuyKeUpdate
		SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanhThemMoi, cthdud.YeuCau FROM @TonLuyKe tlk
		INNER JOIN @ChiTietHoaDonCanUpdate cthdud
		ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanhThemMoi AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

		UPDATE hdct
		SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
		FROM BH_HoaDon_ChiTiet hdct
		INNER JOIN @TonLuyKeUpdate tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet
	END
	IF(@Loai = 3)
	BEGIN
		
		--- Lấy ra danh sách chi tiết của Hóa đơn truyền vào
		DECLARE @ChiTietHoaDonXoa TABLE (IDHoaDon UNIQUEIDENTIFIER,ID_ChiTietHoaDon UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, TonKho FLOAT, LoaiHoaDon INT, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TyLeChuyenDoi FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT);
		INSERT INTO @ChiTietHoaDonXoa
		select hdcthd.ID ,hdctcthd.ID, hdcthd.MaHoaDon, @ThoiGian, hh.ID, hdctcthd.ID_LoHang, @IDChiNhanhInput, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput, hh.ID, hdctcthd.ID_LoHang, @ThoiGian),
		hdcthd.LoaiHoaDon, hdcthd.ID_CheckIn, hdcthd.YeuCau, hdctcthd.SoLuong, dvqd.TyLeChuyenDoi, hdctcthd.TienChietKhau, hdctcthd.ThanhTien FROM BH_HoaDon hdcthd
		INNER JOIN BH_HoaDon_ChiTiet hdctcthd
		ON hdcthd.ID = hdctcthd.ID_HoaDon
		INNER JOIN DonViQuiDoi dvqd
		ON hdctcthd.ID_DonViQuiDoi = dvqd.ID
		INNER JOIN DM_HangHoa hh
		on hh.ID = dvqd.ID_HangHoa
		WHERE hdcthd.ID = @IDHoaDonInput

		DECLARE @TonChuanHDTruyenVaoXoa TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
		INSERT INTO @TonChuanHDTruyenVaoXoa
		SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
		CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE SUM(TienChietKhau * TyLeChuyenDoi) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDonXoa GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau
		
		--- Lấy ra all chi tiết liên quan đến chi tiết hóa đơn truyền vào
			DECLARE @ChiTietHoaDonUpdateXoa TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), ID_CheckInPhieuUp UNIQUEIDENTIFIER);
    		INSERT INTO @ChiTietHoaDonUpdateXoa
    		select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdupdate.NgaySua
    			ELSE
    				hdupdate.NgayLapHoaDon
    		END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdctupdate.TonLuyKe_NhanChuyenHang
    			ELSE
    				hdctupdate.TonLuyKe
    		END AS TonKho, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    		hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau, hdupdate.ID_CheckIn FROM BH_HoaDon hdupdate
    		INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    		ON hdupdate.ID = hdctupdate.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqdupdate
    		ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    		INNER JOIN DM_HangHoa hhupdate
    		on hhupdate.ID = dvqdupdate.ID_HangHoa
    		INNER JOIN @TonChuanHDTruyenVaoXoa cthdthemmoiupdate
    		ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    		WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    		((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > cthdthemmoiupdate.NgayLapHoaDon and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > cthdthemmoiupdate.NgayLapHoaDon))
    		order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

			---Lấy ra những chi tiết của loaiHoaDon = 9 (Kiểm kê)

			DECLARE @ChiTietHoaDonUpdateKiemKeXoa TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), ID_CheckInPhieuUp UNIQUEIDENTIFIER);
    		INSERT INTO @ChiTietHoaDonUpdateKiemKeXoa
    		select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdupdate.NgaySua
    			ELSE
    				hdupdate.NgayLapHoaDon
    		END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		hdctupdate.TonLuyKe, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    		hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau, hdupdate.ID_CheckIn FROM BH_HoaDon hdupdate
    		INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    		ON hdupdate.ID = hdctupdate.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqdupdate
    		ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    		INNER JOIN DM_HangHoa hhupdate
    		on hhupdate.ID = dvqdupdate.ID_HangHoa
    		INNER JOIN @TonChuanHDTruyenVaoXoa cthdthemmoiupdate
    		ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    		WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    		((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > cthdthemmoiupdate.NgayLapHoaDon and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > cthdthemmoiupdate.NgayLapHoaDon))
    		order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

			DECLARE @ChiTietHoaDonCanUpdateXoa TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), ID_CheckInPhieuUp UNIQUEIDENTIFIER);
    		INSERT INTO @ChiTietHoaDonCanUpdateXoa
			SELECT cthdup.IDHoaDon, cthdup.IDHoaDonBan, cthdup.MaHoaDon, cthdup.LoaiHoaDon, cthdup.ID_ChiTietHoaDon, 
    		cthdup.NgayLapHoaDon, cthdup.SoThuTu, cthdup.SoLuong, cthdup.DonGia, cthdup.TongTienHang, cthdup.ChietKhau, cthdup.ThanhTien, cthdup.TongGiamGia, cthdup.TyLeChuyenDoi,
    		cthdup.TonKho, cthdup.GiaVon, cthdup.GiaVonNhan,
    		cthdup.ID_HangHoa, cthdup.LaHangHoa, cthdup.IDDonViQuiDoi, cthdup.ID_LoHang, cthdup.ID_ChiTietDinhLuong, cthdup.ID_ChiNhanhThemMoi, cthdup.ID_CheckIn, cthdup.YeuCau, cthdup.ID_CheckInPhieuUp FROM @ChiTietHoaDonUpdateXoa cthdup
			LEFT JOIN @ChiTietHoaDonUpdateKiemKeXoa ctkiemke on cthdup.ID_HangHoa = ctkiemke.ID_HangHoa and (cthdup.ID_LoHang = ctkiemke.ID_LoHang or cthdup.ID_LoHang is null)
			WHERE (cthdup.NgayLapHoaDon < ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null) and cthdup.IDHoaDon != @IDHoaDonInput

			DECLARE @TonChuanXoa TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, ID_CheckInPhieuUp UNIQUEIDENTIFIER)
			INSERT INTO @TonChuanXoa
			SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi AS ID_ChiNhanh, ID_CheckIn, YeuCau,
			MAX(SoLuong) AS SoLuong, MAX(TonKho) AS TonKho, MAX(ChietKhau) AS TienChietKhau, MAX(ThanhTien) AS ThanhTien, ID_CheckInPhieuUp FROM @ChiTietHoaDonCanUpdateXoa GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau,ID_CheckInPhieuUp
			
			DECLARE @TonLuyKeXoa TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, ID_CheckInPhieuUp UNIQUEIDENTIFIER, TonLuyKe FLOAT);
			INSERT INTO @TonLuyKeXoa
			SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho + SoLuong, 
			IIF(@LoaiHoaDonTruyenVao IN (4, 6, 9), TonKho - SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '3') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho + TienChietKhau, 
			IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '3' AND ID_CheckIn = @IDChiNhanhInput, TonKho - TienChietKhau, TonKho)))) AS TonLuyKe FROM @TonChuanXoa
			
			DECLARE @TonLuyKeUpdateXoa TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), ID_CheckInPhieuUp UNIQUEIDENTIFIER)
			INSERT INTO @TonLuyKeUpdateXoa
			SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanhThemMoi, cthdud.YeuCau, cthdud.ID_CheckInPhieuUp FROM @TonLuyKeXoa tlk
			INNER JOIN @ChiTietHoaDonCanUpdateXoa cthdud
			ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanhThemMoi AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

			UPDATE hdct
			SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckInPhieuUp OR tlkupdate.ID_CheckInPhieuUp IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL and tlkupdate.LoaiHoaDon = 10, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN @TonLuyKeUpdateXoa tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet
	END
	IF(@Loai = 2)
	BEGIN
		IF(@ThoiGian > @ThoiGianHD)
		BEGIN
		--- Lấy ra danh sách chi tiết của Hóa đơn truyền vào
			DECLARE @ChiTietHoaDon1 TABLE (IDHoaDon UNIQUEIDENTIFIER,ID_ChiTietHoaDon UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, TonKho FLOAT, LoaiHoaDon INT, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TyLeChuyenDoi FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT);
			INSERT INTO @ChiTietHoaDon1
			select hdcthd.ID ,hdctcthd.ID, hdcthd.MaHoaDon, @ThoiGianHD, hh.ID, hdctcthd.ID_LoHang, @IDChiNhanhInput, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput, hh.ID, hdctcthd.ID_LoHang, @ThoiGianHD),
			hdcthd.LoaiHoaDon, hdcthd.ID_CheckIn, hdcthd.YeuCau, hdctcthd.SoLuong, dvqd.TyLeChuyenDoi, hdctcthd.TienChietKhau, hdctcthd.ThanhTien FROM BH_HoaDon hdcthd
			INNER JOIN BH_HoaDon_ChiTiet hdctcthd
			ON hdcthd.ID = hdctcthd.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON hdctcthd.ID_DonViQuiDoi = dvqd.ID
			INNER JOIN DM_HangHoa hh
			on hh.ID = dvqd.ID_HangHoa
			WHERE hdcthd.ID = @IDHoaDonInput

			DECLARE @TonChuanHDTruyenVao1 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
			INSERT INTO @TonChuanHDTruyenVao1
			SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
			CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE SUM(TienChietKhau * TyLeChuyenDoi) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDon1 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau

			--DECLARE @TonChuanHDTruyenVao1 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
			--INSERT INTO @TonChuanHDTruyenVao1
			--SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau, 
			--CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, 
			--CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '1' and ID_ChiNhanh != ID_CheckIn, SUM(TienChietKhau * TyLeChuyenDoi) ,
			--IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '4' and ID_ChiNhanh != ID_CheckIn,SUM(TienChietKhau * TyLeChuyenDoi) - SUM(SoLuong * TyLeChuyenDoi), SUM(TienChietKhau * TyLeChuyenDoi))) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDon1 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau
			
			DECLARE @TonLuyKeTruyenVao1 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
			INSERT INTO @TonLuyKeTruyenVao1
			SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho - SoLuong, 
			IIF(@LoaiHoaDonTruyenVao IN (4, 6), TonKho + SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho - TienChietKhau, 
			IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho + TienChietKhau, 
			IIF(@LoaiHoaDonTruyenVao = 9, ThanhTien, TonKho))))) AS TonLuyKe FROM @TonChuanHDTruyenVao1

			DECLARE @TonLuyKeUpdateTruyenVao1 TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
			INSERT INTO @TonLuyKeUpdateTruyenVao1
			SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanh, cthdud.YeuCau FROM @TonLuyKeTruyenVao1 tlk
			INNER JOIN @ChiTietHoaDon1 cthdud
			ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanh AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

			UPDATE hdct
			SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN @TonLuyKeUpdateTruyenVao1 tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet

		--- Lấy ra all chi tiết liên quan đến chi tiết hóa đơn truyền vào
			DECLARE @ChiTietHoaDonUpdate1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonUpdate1
    		select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdupdate.NgaySua
    			ELSE
    				hdupdate.NgayLapHoaDon
    		END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		hdctupdate.TonLuyKe, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    		hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    		INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    		ON hdupdate.ID = hdctupdate.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqdupdate
    		ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    		INNER JOIN DM_HangHoa hhupdate
    		on hhupdate.ID = dvqdupdate.ID_HangHoa
    		INNER JOIN @TonChuanHDTruyenVao1 cthdthemmoiupdate
    		ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    		WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    		((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGianHD and hdupdate.NgayLapHoaDon < @ThoiGian and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGianHD and hdupdate.NgaySua < @ThoiGian))
    		order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

			---Lấy ra những chi tiết của loaiHoaDon = 9 (Kiểm kê)
			DECLARE @ChiTietHoaDonUpdateKiemKe1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonUpdateKiemKe1 
			select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdupdate.NgaySua
    			ELSE
    				hdupdate.NgayLapHoaDon
    		END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdctupdate.TonLuyKe_NhanChuyenHang
    			ELSE
    				hdctupdate.TonLuyKe
    		END AS TonKho, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    		hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    		INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    		ON hdupdate.ID = hdctupdate.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqdupdate
    		ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    		INNER JOIN DM_HangHoa hhupdate
    		on hhupdate.ID = dvqdupdate.ID_HangHoa
    		INNER JOIN @TonChuanHDTruyenVao1 cthdthemmoiupdate
    		ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    		WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    		((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGianHD and hdupdate.NgayLapHoaDon < @ThoiGian and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGianHD and hdupdate.NgaySua < @ThoiGian))
    		order by NgayLapHoaDon, SoThuTu desc, LoaiHoaDon, MaHoaDon;

			DECLARE @ChiTietHoaDonCanUpdate1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonCanUpdate1
			SELECT cthdup.IDHoaDon, cthdup.IDHoaDonBan, cthdup.MaHoaDon, cthdup.LoaiHoaDon, cthdup.ID_ChiTietHoaDon, 
    		cthdup.NgayLapHoaDon, cthdup.SoThuTu, cthdup.SoLuong, cthdup.DonGia, cthdup.TongTienHang, cthdup.ChietKhau, cthdup.ThanhTien, cthdup.TongGiamGia, cthdup.TyLeChuyenDoi,
    		cthdup.TonKho, cthdup.GiaVon, cthdup.GiaVonNhan,
    		cthdup.ID_HangHoa, cthdup.LaHangHoa, cthdup.IDDonViQuiDoi, cthdup.ID_LoHang, cthdup.ID_ChiTietDinhLuong, cthdup.ID_ChiNhanhThemMoi, cthdup.ID_CheckIn, cthdup.YeuCau FROM @ChiTietHoaDonUpdate1 cthdup
			LEFT JOIN @ChiTietHoaDonUpdateKiemKe1 ctkiemke on cthdup.ID_HangHoa = ctkiemke.ID_HangHoa and (cthdup.ID_LoHang = ctkiemke.ID_LoHang or cthdup.ID_LoHang is null)
			WHERE (cthdup.NgayLapHoaDon < ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null)

			DECLARE @TonChuan1 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
			INSERT INTO @TonChuan1
			SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi AS ID_ChiNhanh, ID_CheckIn, YeuCau,
			MAX(SoLuong) AS SoLuong, MAX(TonKho) AS TonKho, MAX(ChietKhau) AS TienChietKhau, MAX(ThanhTien) AS ThanhTien FROM @ChiTietHoaDonCanUpdate1 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau

			DECLARE @TonLuyKe1 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
			INSERT INTO @TonLuyKe1
			SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho - SoLuong, 
			IIF(@LoaiHoaDonTruyenVao IN (4, 6, 9), TonKho + SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho - TienChietKhau, 
			IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho + TienChietKhau, TonKho)))) AS TonLuyKe FROM @TonChuan1

			DECLARE @TonLuyKeUpdate1 TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
			INSERT INTO @TonLuyKeUpdate1
			SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanhThemMoi, cthdud.YeuCau FROM @TonLuyKe1 tlk
			INNER JOIN @ChiTietHoaDonCanUpdate1 cthdud
			ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanhThemMoi AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

			UPDATE hdct
			SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN @TonLuyKeUpdate1 tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet

			DECLARE @DemKK INT;
			SELECT @DemKK = COUNT(*) FROm @ChiTietHoaDonUpdateKiemKe1;
			IF(@DemKK > 0)
			BEGIN
				DECLARE @ChiTietHoaDonUpdateThem TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    			ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    			ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    			INSERT INTO @ChiTietHoaDonUpdateThem
    			select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    			CASE 
    				WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    				THEN
    					hdupdate.NgaySua
    				ELSE
    					hdupdate.NgayLapHoaDon
    			END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    			CASE 
    				WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    				THEN
    					hdctupdate.TonLuyKe_NhanChuyenHang
    				ELSE
    					hdctupdate.TonLuyKe
    			END AS TonKho, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    			hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    			ON hdupdate.ID = hdctupdate.ID_HoaDon
    			INNER JOIN DonViQuiDoi dvqdupdate
    			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    			INNER JOIN DM_HangHoa hhupdate
    			on hhupdate.ID = dvqdupdate.ID_HangHoa
    			INNER JOIN @TonChuanHDTruyenVao1 cthdthemmoiupdate
    			ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    			WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    			((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGian and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    			or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGian))
    			order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;
					
				DECLARE @ChiTietHoaDonUpdateKiemKeThem TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    			ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    			ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    			INSERT INTO @ChiTietHoaDonUpdateKiemKeThem 
				select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    			CASE 
    				WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    				THEN
    					hdupdate.NgaySua
    				ELSE
    					hdupdate.NgayLapHoaDon
    			END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    			hdctupdate.TonLuyKe, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    			hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    			ON hdupdate.ID = hdctupdate.ID_HoaDon
    			INNER JOIN DonViQuiDoi dvqdupdate
    			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    			INNER JOIN DM_HangHoa hhupdate
    			on hhupdate.ID = dvqdupdate.ID_HangHoa
    			INNER JOIN @TonChuanHDTruyenVao1 cthdthemmoiupdate
    			ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    			WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    			((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGian and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    			or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGian))
    			order by NgayLapHoaDon, SoThuTu desc, LoaiHoaDon, MaHoaDon;

				DECLARE @ChiTietHoaDonCanUpdateThem TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    			ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    			ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    			INSERT INTO @ChiTietHoaDonCanUpdateThem
				SELECT cthdup.IDHoaDon, cthdup.IDHoaDonBan, cthdup.MaHoaDon, cthdup.LoaiHoaDon, cthdup.ID_ChiTietHoaDon, 
    			cthdup.NgayLapHoaDon, cthdup.SoThuTu, cthdup.SoLuong, cthdup.DonGia, cthdup.TongTienHang, cthdup.ChietKhau, cthdup.ThanhTien, cthdup.TongGiamGia, cthdup.TyLeChuyenDoi,
    			cthdup.TonKho, cthdup.GiaVon, cthdup.GiaVonNhan,
    			cthdup.ID_HangHoa, cthdup.LaHangHoa, cthdup.IDDonViQuiDoi, cthdup.ID_LoHang, cthdup.ID_ChiTietDinhLuong, cthdup.ID_ChiNhanhThemMoi, cthdup.ID_CheckIn, cthdup.YeuCau FROM @ChiTietHoaDonUpdateThem cthdup
				LEFT JOIN @ChiTietHoaDonUpdateKiemKeThem ctkiemke on cthdup.ID_HangHoa = ctkiemke.ID_HangHoa and (cthdup.ID_LoHang = ctkiemke.ID_LoHang or cthdup.ID_LoHang is null)
				WHERE (cthdup.NgayLapHoaDon < ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null)

				DECLARE @TonChuanThem TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
				INSERT INTO @TonChuanThem
				SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi AS ID_ChiNhanh, ID_CheckIn, YeuCau,
				MAX(SoLuong) AS SoLuong, MAX(TonKho) AS TonKho, MAX(ChietKhau) AS TienChietKhau, MAX(ThanhTien) AS ThanhTien FROM @ChiTietHoaDonCanUpdateThem GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau

				DECLARE @TonLuyKeThem TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
				INSERT INTO @TonLuyKeThem
				SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho + SoLuong, 
				IIF(@LoaiHoaDonTruyenVao IN (4, 6, 9), TonKho - SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho + TienChietKhau, 
				IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho - TienChietKhau, TonKho)))) AS TonLuyKe FROM @TonChuanThem

				DECLARE @TonLuyKeUpdateThem TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
				INSERT INTO @TonLuyKeUpdateThem
				SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanhThemMoi, cthdud.YeuCau FROM @TonLuyKeThem tlk
				INNER JOIN @ChiTietHoaDonCanUpdateThem cthdud
				ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanhThemMoi AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

				UPDATE hdct
				SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN @TonLuyKeUpdateThem tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet

			END
		END
		ELSE
		BEGIN
		--- Lấy ra danh sách chi tiết của Hóa đơn truyền vào
			DECLARE @ChiTietHoaDon2 TABLE (IDHoaDon UNIQUEIDENTIFIER,ID_ChiTietHoaDon UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, TonKho FLOAT, LoaiHoaDon INT, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TyLeChuyenDoi FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT);
			INSERT INTO @ChiTietHoaDon2
			select hdcthd.ID ,hdctcthd.ID, hdcthd.MaHoaDon, @ThoiGianHD, hh.ID, hdctcthd.ID_LoHang, @IDChiNhanhInput, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput, hh.ID, hdctcthd.ID_LoHang, @ThoiGianHD),
			hdcthd.LoaiHoaDon, hdcthd.ID_CheckIn, hdcthd.YeuCau, hdctcthd.SoLuong, dvqd.TyLeChuyenDoi, hdctcthd.TienChietKhau, hdctcthd.ThanhTien FROM BH_HoaDon hdcthd
			INNER JOIN BH_HoaDon_ChiTiet hdctcthd
			ON hdcthd.ID = hdctcthd.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON hdctcthd.ID_DonViQuiDoi = dvqd.ID
			INNER JOIN DM_HangHoa hh
			on hh.ID = dvqd.ID_HangHoa
			WHERE hdcthd.ID = @IDHoaDonInput

			DECLARE @TonChuanHDTruyenVao2 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
			INSERT INTO @TonChuanHDTruyenVao2
			SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
			CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE SUM(TienChietKhau * TyLeChuyenDoi) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDon1 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau

			--DECLARE @TonChuanHDTruyenVao2 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
			--	INSERT INTO @TonChuanHDTruyenVao2
			--	SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
			--	CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, 
			--	CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '1' and ID_ChiNhanh != ID_CheckIn, SUM(TienChietKhau * TyLeChuyenDoi) ,
			--	IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '4' and ID_ChiNhanh != ID_CheckIn,SUM(TienChietKhau * TyLeChuyenDoi) - SUM(SoLuong * TyLeChuyenDoi), SUM(TienChietKhau * TyLeChuyenDoi))) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDon2 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau

		--- Lấy ra all chi tiết liên quan đến chi tiết hóa đơn truyền vào
			DECLARE @ChiTietHoaDonUpdate2 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, TonKho_Nhan FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonUpdate2
    		select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdupdate.NgaySua
    			ELSE
    				hdupdate.NgayLapHoaDon
    		END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		hdctupdate.TonLuyKe,hdctupdate.TonLuyKe_NhanChuyenHang, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    		hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    		INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    		ON hdupdate.ID = hdctupdate.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqdupdate
    		ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    		INNER JOIN DM_HangHoa hhupdate
    		on hhupdate.ID = dvqdupdate.ID_HangHoa
    		INNER JOIN @TonChuanHDTruyenVao2 cthdthemmoiupdate
    		ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    		WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    		((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGian and hdupdate.NgayLapHoaDon < @ThoiGianHD and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGian and hdupdate.NgaySua <= @ThoiGianHD))
    		order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

			---Lấy ra những chi tiết của loaiHoaDon = 9 (Kiểm kê)
			DECLARE @ChiTietHoaDonUpdateKiemKe2 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, TonKho_Nhan FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonUpdateKiemKe2
    		select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    		CASE 
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdupdate.NgaySua
    			ELSE
    				hdupdate.NgayLapHoaDon
    		END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		hdctupdate.TonLuyKe,hdctupdate.TonLuyKe_NhanChuyenHang, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    		hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    		INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    		ON hdupdate.ID = hdctupdate.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqdupdate
    		ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    		INNER JOIN DM_HangHoa hhupdate
    		on hhupdate.ID = dvqdupdate.ID_HangHoa
    		INNER JOIN @TonChuanHDTruyenVao2 cthdthemmoiupdate
    		ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    		WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    		((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGian and hdupdate.NgayLapHoaDon < @ThoiGianHD and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGian and hdupdate.NgaySua <= @ThoiGianHD))
    		order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

			DECLARE @ChiTietHoaDonCanUpdate2 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT,TonKho_Nhan FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonCanUpdate2
			SELECT cthdup.IDHoaDon, cthdup.IDHoaDonBan, cthdup.MaHoaDon, cthdup.LoaiHoaDon, cthdup.ID_ChiTietHoaDon, 
    		cthdup.NgayLapHoaDon, cthdup.SoThuTu, cthdup.SoLuong, cthdup.DonGia, cthdup.TongTienHang, cthdup.ChietKhau, cthdup.ThanhTien, cthdup.TongGiamGia, cthdup.TyLeChuyenDoi,
    		cthdup.TonKho,cthdup.TonKho_Nhan, cthdup.GiaVon, cthdup.GiaVonNhan,
    		cthdup.ID_HangHoa, cthdup.LaHangHoa, cthdup.IDDonViQuiDoi, cthdup.ID_LoHang, cthdup.ID_ChiTietDinhLuong, cthdup.ID_ChiNhanhThemMoi, cthdup.ID_CheckIn, cthdup.YeuCau FROM @ChiTietHoaDonUpdate2 cthdup
			LEFT JOIN (SELECT *,ROW_NUMBER() OVER (PARTITION BY ID_HangHoa ORDER BY NgayLapHoaDon) AS STT FROM @ChiTietHoaDonUpdateKiemKe2) ctkiemke on cthdup.ID_HangHoa = ctkiemke.ID_HangHoa and (cthdup.ID_LoHang = ctkiemke.ID_LoHang or cthdup.ID_LoHang is null)
			WHERE (cthdup.NgayLapHoaDon < ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null) and cthdup.IDHoaDon != @IDHoaDonInput

			DECLARE @TonChuan2 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
			INSERT INTO @TonChuan2
			SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi AS ID_ChiNhanh, ID_CheckIn, YeuCau,
			MAX(SoLuong) AS SoLuong, MAX(TonKho) AS TonKho, MAX(ChietKhau) AS TienChietKhau, MAX(ThanhTien) AS ThanhTien FROM @ChiTietHoaDonCanUpdate2 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau

			DECLARE @TonLuyKe2 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
			INSERT INTO @TonLuyKe2
			SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho + SoLuong, 
			IIF(@LoaiHoaDonTruyenVao IN (4, 6, 9), TonKho - SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho + TienChietKhau, 
			IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho - TienChietKhau, TonKho)))) AS TonLuyKe FROM @TonChuan2

			DECLARE @TonLuyKeUpdate2 TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
			INSERT INTO @TonLuyKeUpdate2
			SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanhThemMoi, cthdud.YeuCau FROM @TonLuyKe2 tlk
			INNER JOIN @ChiTietHoaDonCanUpdate2 cthdud
			ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanhThemMoi AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

			UPDATE hdct
			SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN @TonLuyKeUpdate2 tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet

			DECLARE @DemKKThem INT;
			SELECT @DemKKThem = COUNT(*) FROM @ChiTietHoaDonUpdateKiemKe2;
			IF(@DemKKThem > 0)
			BEGIN
				DECLARE @TonLuyKeTruyenVao2 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
				INSERT INTO @TonLuyKeTruyenVao2
				SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho - SoLuong, 
				IIF(@LoaiHoaDonTruyenVao IN (4, 6), TonKho + SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho - TienChietKhau, 
				IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho + TienChietKhau, 
				IIF(@LoaiHoaDonTruyenVao = 9, ThanhTien, TonKho))))) AS TonLuyKe FROM @TonChuanHDTruyenVao2

				DECLARE @TonLuyKeUpdateTruyenVao2 TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
				INSERT INTO @TonLuyKeUpdateTruyenVao2
				SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanh, cthdud.YeuCau FROM @TonLuyKeTruyenVao2 tlk
				INNER JOIN @ChiTietHoaDon2 cthdud
				ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanh AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

				UPDATE hdct
				SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN @TonLuyKeUpdateTruyenVao2 tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet
				
				DECLARE @ChiTietHoaDonUpdateThem1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    			ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    			ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    			INSERT INTO @ChiTietHoaDonUpdateThem1
    			select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    			CASE 
    				WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    				THEN
    					hdupdate.NgaySua
    				ELSE
    					hdupdate.NgayLapHoaDon
    			END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    			CASE
    			WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    			THEN
    				hdctupdate.TonLuyKe_NhanChuyenHang
    			ELSE
    				hdctupdate.TonLuyKe
    			END as TonLuyKe, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    			hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    			ON hdupdate.ID = hdctupdate.ID_HoaDon
    			INNER JOIN DonViQuiDoi dvqdupdate
    			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    			INNER JOIN DM_HangHoa hhupdate
    			on hhupdate.ID = dvqdupdate.ID_HangHoa
    			INNER JOIN @TonChuanHDTruyenVao2 cthdthemmoiupdate
    			ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    			WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    			((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGianHD and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    			or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGianHD))
    			order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;
					
				DECLARE @ChiTietHoaDonUpdateKiemKeThem1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    			ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    			ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    			INSERT INTO @ChiTietHoaDonUpdateKiemKeThem1 
				select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    			CASE 
    				WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    				THEN
    					hdupdate.NgaySua
    				ELSE
    					hdupdate.NgayLapHoaDon
    			END AS NgayLapHoaDon, hdctupdate.SoThuTu, cthdthemmoiupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, cthdthemmoiupdate.TienChietKhau, cthdthemmoiupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    			hdctupdate.TonLuyKe, hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    			hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, cthdthemmoiupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, cthdthemmoiupdate.ID_CheckIn, cthdthemmoiupdate.YeuCau FROM BH_HoaDon hdupdate
    			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    			ON hdupdate.ID = hdctupdate.ID_HoaDon
    			INNER JOIN DonViQuiDoi dvqdupdate
    			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    			INNER JOIN DM_HangHoa hhupdate
    			on hhupdate.ID = dvqdupdate.ID_HangHoa
    			INNER JOIN @TonChuanHDTruyenVao2 cthdthemmoiupdate
    			ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    			WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    			((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon > @ThoiGianHD and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    			or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua > @ThoiGianHD))
    			order by NgayLapHoaDon, SoThuTu desc, LoaiHoaDon, MaHoaDon;

				DECLARE @ChiTietHoaDonCanUpdateThem1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    			ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    			ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    			INSERT INTO @ChiTietHoaDonCanUpdateThem1
				SELECT cthdup.IDHoaDon, cthdup.IDHoaDonBan, cthdup.MaHoaDon, cthdup.LoaiHoaDon, cthdup.ID_ChiTietHoaDon, 
    			cthdup.NgayLapHoaDon, cthdup.SoThuTu, cthdup.SoLuong, cthdup.DonGia, cthdup.TongTienHang, cthdup.ChietKhau, cthdup.ThanhTien, cthdup.TongGiamGia, cthdup.TyLeChuyenDoi,
    			cthdup.TonKho, cthdup.GiaVon, cthdup.GiaVonNhan,
    			cthdup.ID_HangHoa, cthdup.LaHangHoa, cthdup.IDDonViQuiDoi, cthdup.ID_LoHang, cthdup.ID_ChiTietDinhLuong, cthdup.ID_ChiNhanhThemMoi, cthdup.ID_CheckIn, cthdup.YeuCau FROM @ChiTietHoaDonUpdateThem1 cthdup
				LEFT JOIN @ChiTietHoaDonUpdateKiemKeThem1 ctkiemke on cthdup.ID_HangHoa = ctkiemke.ID_HangHoa and (cthdup.ID_LoHang = ctkiemke.ID_LoHang or cthdup.ID_LoHang is null)
				WHERE (cthdup.NgayLapHoaDon < ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null)

				DECLARE @TonChuanThem1 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
				INSERT INTO @TonChuanThem1
				SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi AS ID_ChiNhanh, ID_CheckIn, YeuCau,
				MAX(SoLuong) AS SoLuong, MAX(TonKho) AS TonKho, MAX(ChietKhau) AS TienChietKhau, MAX(ThanhTien) AS ThanhTien FROM @ChiTietHoaDonCanUpdateThem1 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau

				DECLARE @TonLuyKeThem1 TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
				INSERT INTO @TonLuyKeThem1
				SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho - SoLuong, 
				IIF(@LoaiHoaDonTruyenVao IN (4, 6, 9), TonKho + SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho - TienChietKhau, 
				IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho + TienChietKhau, TonKho)))) AS TonLuyKe FROM @TonChuanThem1

				DECLARE @TonLuyKeUpdateThem1 TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
				INSERT INTO @TonLuyKeUpdateThem1
				SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanhThemMoi, cthdud.YeuCau FROM @TonLuyKeThem1 tlk
				INNER JOIN @ChiTietHoaDonCanUpdateThem1 cthdud
				ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanhThemMoi AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

				UPDATE hdct
				SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN @TonLuyKeUpdateThem1 tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet
			END
			ELSE
			BEGIN
				DECLARE @ChiTietHoaDonDaUpDate TABLE (IDHoaDon UNIQUEIDENTIFIER,ID_ChiTietHoaDon UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, TonKho FLOAT, LoaiHoaDon INT, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TyLeChuyenDoi FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT);
				INSERT INTO @ChiTietHoaDonDaUpDate
				select hdcthd.ID ,hdctcthd.ID, hdcthd.MaHoaDon, @ThoiGianHD, hh.ID, hdctcthd.ID_LoHang, @IDChiNhanhInput, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput, hh.ID, hdctcthd.ID_LoHang, @ThoiGianHD),
				hdcthd.LoaiHoaDon, hdcthd.ID_CheckIn, hdcthd.YeuCau, hdctcthd.SoLuong, dvqd.TyLeChuyenDoi, hdctcthd.TienChietKhau, hdctcthd.ThanhTien FROM BH_HoaDon hdcthd
				INNER JOIN BH_HoaDon_ChiTiet hdctcthd
				ON hdcthd.ID = hdctcthd.ID_HoaDon
				INNER JOIN DonViQuiDoi dvqd
				ON hdctcthd.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN DM_HangHoa hh
				on hh.ID = dvqd.ID_HangHoa
				WHERE hdcthd.ID = @IDHoaDonInput

				DECLARE @TonChuanHDTruyenVaoDaUpDateChoTruyenVao TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
				INSERT INTO @TonChuanHDTruyenVaoDaUpDateChoTruyenVao
				SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
				CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE SUM(TienChietKhau * TyLeChuyenDoi) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDon1 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau

				--DECLARE @TonChuanHDTruyenVaoDaUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT)
				--INSERT INTO @TonChuanHDTruyenVaoDaUpDate
				--SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau,
				--CASE WHEN @LoaiHoaDonTruyenVao = 9 THEN SUM(ThanhTien * TyLeChuyenDoi) - MAX(TonKho) ELSE SUM(SoLuong * TyLeChuyenDoi) END  AS SoLuong, MAX(TonKho) AS TonKho, 
				--CASE WHEN @LoaiHoaDonTruyenVao = 9  THEN MAX(TienChietKhau * TyLeChuyenDoi) ELSE IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '1' and ID_ChiNhanh != ID_CheckIn, SUM(TienChietKhau * TyLeChuyenDoi) ,
				--IIF(@LoaiHoaDonTruyenVao = 10 and YeuCau = '4' and ID_ChiNhanh != ID_CheckIn,SUM(TienChietKhau * TyLeChuyenDoi) - SUM(SoLuong * TyLeChuyenDoi), SUM(TienChietKhau * TyLeChuyenDoi))) END AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDonDaUpDate GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanh, ID_CheckIn, YeuCau
			
				DECLARE @TonLuyKeTruyenVaoDaUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
				INSERT INTO @TonLuyKeTruyenVaoDaUpDate
				SELECT *, IIF(@LoaiHoaDonTruyenVao IN (1, 5, 7, 8), TonKho - SoLuong, 
				IIF(@LoaiHoaDonTruyenVao IN (4, 6), TonKho + SoLuong, IIF((@LoaiHoaDonTruyenVao = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND @LoaiHoaDonTruyenVao = 10 AND YeuCau = '4') AND ID_ChiNhanh = @IDChiNhanhInput, TonKho - TienChietKhau, 
				IIF(@LoaiHoaDonTruyenVao = 10 AND YeuCau = '4' AND ID_CheckIn = @IDChiNhanhInput, TonKho + TienChietKhau, 
				IIF(@LoaiHoaDonTruyenVao = 9, ThanhTien, TonKho))))) AS TonLuyKe FROM @TonChuanHDTruyenVaoDaUpDateChoTruyenVao

				DECLARE @TonLuyKeUpdateTruyenVaoDaUpDate TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX))
				INSERT INTO @TonLuyKeUpdateTruyenVaoDaUpDate
				SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanh, cthdud.YeuCau FROM @TonLuyKeTruyenVaoDaUpDate tlk
				INNER JOIN @ChiTietHoaDonDaUpDate cthdud
				ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanh AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

				UPDATE hdct
				SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN @TonLuyKeUpdateTruyenVaoDaUpDate tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet
			END
		END
	END");

            CreateStoredProcedure(name: "[dbo].[UpdateTonLuyKeInit]", parametersAction: p => new
            {
                ThoiGian = p.DateTime()
            }, body: @"--DECLARE @ThoiGian DATETIME;
		--SET @ThoiGian = '2019-01-01';
		--DECLARE @ID_DonVi UNIQUEIDENTIFIER;
		--SET @ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
		SET NOCOUNT ON;
		DECLARE @ID_DonVi UNIQUEIDENTIFIER;
		--DECLARE @ThoiGian DATETIME;
		--SET @ThoiGian = '2019-01-01';
		DECLARE @TonLuyKeUpdate TABLE (ID_ChiTiet UNIQUEIDENTIFIER, LoaiHoaDon int, TonLuyKe FLOAT, ID_CheckIn UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
		DECLARE @ChiTietHoaDonUpdate1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuTu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
		DECLARE @TonChuan TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT);
		DECLARE @TonLuyKe TABLE(IDHoaDon UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_LoHang UNIQUEIDENTIFIER, LoaiHoaDon INT, ID_ChiNhanh UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), SoLuong FLOAT, TonKho FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TonLuyKe FLOAT);
		DECLARE CSDonVi CURSOR FOR SELECT ID FROM DM_DonVi
		OPEN CSDonVi
		FETCH NEXT FROM CSDonVi INTO @ID_DonVi
		WHILE @@FETCH_STATUS = 0
		BEGIN
		
    	INSERT INTO @ChiTietHoaDonUpdate1
    	select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    	CASE 
    		WHEN hdupdate.YeuCau = '4' AND @ID_DonVi = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END AS NgayLapHoaDon, hdctupdate.SoThuTu, hdctupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, hdctupdate.TienChietKhau, hdctupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    	[dbo].[FUNC_TinhSLTonKhiTaoHD](@ID_DonVi, hhupdate.ID, hdctupdate.ID_LoHang, CASE
    		WHEN hdupdate.YeuCau = '4' AND @ID_DonVi = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END), hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi, hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi,
    	hhupdate.ID, hhupdate.LaHangHoa, dvqdupdate.ID, hdctupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, @ID_DonVi, hdupdate.ID_CheckIn, hdupdate.YeuCau FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    	ON hdupdate.ID = hdctupdate.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqdupdate
    	ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    	INNER JOIN DM_HangHoa hhupdate
    	on hhupdate.ID = dvqdupdate.ID_HangHoa
    	LEFT JOIN DM_LoHang lh
    	ON lh.ID_HangHoa = hhupdate.ID
    	WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = lh.ID OR lh.ID IS NULL) AND
    	((hdupdate.ID_DonVi = @ID_DonVi and hdupdate.NgayLapHoaDon >= @ThoiGian and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    	or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = @ID_DonVi and hdupdate.NgaySua >= @ThoiGian));
	
		--SELECT * FROM @ChiTietHoaDonUpdate1

		
		INSERT INTO @TonChuan
		SELECT IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi AS ID_ChiNhanh, ID_CheckIn, YeuCau,
		SUM(SoLuong * TyLeChuyenDoi) AS SoLuong, MAX(TonKho) AS TonKho, SUM(ChietKhau * TyLeChuyenDoi) AS TienChietKhau, SUM(ThanhTien * TyLeChuyenDoi) AS ThanhTien FROM @ChiTietHoaDonUpdate1 GROUP BY IDHoaDon, ID_HangHoa, NgayLapHoaDon, ID_LoHang, LoaiHoaDon, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau


		
		INSERT INTO @TonLuyKe
		SELECT *, IIF(LoaiHoaDon IN (1, 5, 7, 8), TonKho - SoLuong, 
		IIF(LoaiHoaDon IN (4, 6), TonKho + SoLuong, IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @ID_DonVi AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_ChiNhanh = @ID_DonVi, TonKho - TienChietKhau, 
		IIF(LoaiHoaDon = 10 AND YeuCau = '4' AND ID_CheckIn = @ID_DonVi, TonKho + TienChietKhau, 
		IIF(LoaiHoaDon = 9, ThanhTien, TonKho))))) AS TonLuyKe FROM @TonChuan

		--SELECT * FROM @TonLuyKe WHERE ID_HangHoa = 'D3E6137C-31ED-43D4-A280-46DDDE99EC89'

		
		INSERT INTO @TonLuyKeUpdate
		SELECT cthdud.ID_ChiTietHoaDon, cthdud.LoaiHoaDon, tlk.TonLuyKe, cthdud.ID_CheckIn, cthdud.ID_ChiNhanhThemMoi, cthdud.YeuCau FROM @TonLuyKe tlk
		INNER JOIN @ChiTietHoaDonUpdate1 cthdud
		ON (tlk.ID_CheckIn = cthdud.ID_CheckIn OR cthdud.ID_CheckIn IS NULL) AND tlk.ID_ChiNhanh = cthdud.ID_ChiNhanhThemMoi AND tlk.ID_HangHoa = cthdud.ID_HangHoa AND (tlk.ID_LoHang = cthdud.ID_LoHang OR cthdud.ID_LoHang IS NULL) AND tlk.IDHoaDon = cthdud.IDHoaDon AND tlk.NgayLapHoaDon = cthdud.NgayLapHoaDon AND tlk.LoaiHoaDon = cthdud.LoaiHoaDon AND (tlk.YeuCau = cthdud.YeuCau OR cthdud.YeuCau IS NULL)

		UPDATE hdct
		SET hdct.TonLuyKe = IIF(tlkupdate.ID_ChiNhanh != tlkupdate.ID_CheckIn OR tlkupdate.ID_CheckIn IS NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @ID_DonVi AND tlkupdate.ID_CheckIn IS NOT NULL, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
		FROM BH_HoaDon_ChiTiet hdct
		INNER JOIN @TonLuyKeUpdate tlkupdate ON hdct.ID = tlkupdate.ID_ChiTiet
		
		DELETE FROM @TonLuyKeUpdate;
		DELETE FROM @ChiTietHoaDonUpdate1;
		DELETE FROM @TonChuan;
		DELETE FROM @TonLuyKe;
		FETCH NEXT FROM CSDonVi INTO @ID_DonVi
		END
		CLOSE CSDonVi
		DEALLOCATE CSDonVi");

            Sql(@"ALTER PROCEDURE [dbo].[getList_DMLoHangByIDDonViQD]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @timeChotSo [datetime]
AS
BEGIN
	SELECT dmlo.ID, dvqd.ID_HangHoa, dvqd.ID as ID_DonViQuiDoi, dmlo.NgaySanXuat, dmlo.NgayHetHan, CAST(ROUND((ISNULL(tonkho.TonKho,0) / dvqd.TyLeChuyenDoi), 3) as float) as TonKho FROM DM_LoHang dmlo
	LEFT JOIN DonViQuiDoi dvqd on dmlo.ID_HangHoa = dvqd.ID_HangHoa
	LEFT JOIN DM_HangHoa_TonKho tonkho on tonkho.ID_DonViQuyDoi = dvqd.ID and tonkho.ID_LoHang = dmlo.ID and tonkho.ID_DonVi = @ID_ChiNhanh
	WHERE dvqd.ID = @ID_DonViQuiDoi
	order by dmlo.NgayTao DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListNhanVienEdit]
    @ID_NhanVien [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
    select * from NS_NhanVien nv where (nv.TrangThai is null or nv.TrangThai = 1) and nv.DaNghiViec != 1 and nv.ID not in (select ID_NhanVien from HT_nguoiDung where ID_NhanVien != @ID_NhanVien)
END");

            Sql(@"ALTER PROC [dbo].[insert_TonKhoKhoiTao]
AS
BEGIN
SET NOCOUNT ON;
--Insert tồn kho vào DM_HangHoa_TonKho
exec UpdateTonLuyKeInit '2019-01-01'
Update DonViQuiDoi set Xoa = 0 where Xoa is null
update DM_HangHoa set QuanLyTheoLoHang = '0' where QuanLyTheoLoHang is null;
DECLARE @TabTK TABLE (ID UNIQUEIDENTIFIER, ID_DonViQuyDoi UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, TonKho float);
DECLARE @TableDonVi TABLE (ID_DonVi UNIQUEIDENTIFIER) INSERT INTO @TableDonVi
SELECT ID FROM DM_DonVi

DECLARE @ID_DonVi UNIQUEIDENTIFIER;

DECLARE CS_DonVi CURSOR SCROLL LOCAL FOR SELECT ID_DonVi FROM @TableDonVi
OPEN CS_DonVi
FETCH FIRST FROM CS_DonVi INTO @ID_DonVi
WHILE @@FETCH_STATUS = 0
BEGIN
		INSERT INTO @TabTK SELECT NEWID(), dvqd.ID, @ID_DonVi, dmlo.ID, [dbo].[FUNC_TinhSLTonKhiTaoHD](@ID_DonVi, dvqd.ID_HangHoa, dmlo.ID, DATEADD(minute, 1, GETDATE())) / dvqd.TyLeChuyenDoi
		FROM DonViQuiDoi dvqd
		INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		LEFT JOIN DM_LoHang dmlo on dvqd.ID_HangHoa = dmlo.ID_HangHoa
		where dvqd.Xoa = 0 and hh.LaHangHoa = 1
		FETCH NEXT FROM CS_DonVi INTO @ID_DonVi
END
CLOSE CS_DonVi
DEALLOCATE CS_DonVi
Update dm set dm.TonKho = tk.TonKho from DM_HangHoa_TonKho dm join @TabTK tk on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null))
Insert into DM_HangHoa_TonKho select NEWID(), tk.ID_DonViQuyDoi, tk.ID_DonVi, tk.ID_LoHang, tk.TonKho from @TabTK tk left join DM_HangHoa_TonKho dm on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) where dm.ID is null
END
");

            Sql(@"ALTER PROCEDURE [dbo].[selectKhachHang_CongNo]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
    SELECT 
    		NEWID() AS ID,
    		*, @ID_ChiNhanh as ID_DonVi, @timeEnd as NgayChotSo
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS CongNo
    			FROM
    			(
    			-- Doanh thu tu ban hang
    			SELECT 
    			bhd.ID_DoiTuong,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri tra tu ban hang
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			
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
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
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
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    		) 
    		  AS HangHoa
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhSLTon]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON;
	--select * from DonViQuiDoi where MaHangHoa = 'HH00475'
	
	--DECLARE @timeEnd DATETIME;
	--DECLARE @ID_ChiNhanh UNIQUEIDENTIFIER;
	--DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
	--SET @timeEnd = GETDAtE();
	--SET @ID_ChiNhanh = '9FBD9BD9-0360-47E8-BD30-20331C7B0A04';
	--SET @ID_HangHoa = 'DAD9CB12-F1DF-4653-B23A-496350B2D349';
	SELECT
    ISNULL(SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)),0) AS TonKho
    FROM
    (
    SELECT 
    dvqd.ID_HangHoa,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 'false' and hh.LaHangHoa =1 AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	And dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa                                                                                                                                                                                                                                                             
    
    UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
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
    GROUP BY dvqd.ID_HangHoa
    
    UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa

	UNION ALL
	SELECT 
    dvqd.ID_HangHoa,
	SUM(ISNULL(bhdct.ThanhTien,0)* dvqd.TyLeChuyenDoi) - MAX(ISNULL(bhdct.TienChietKhau,0) * dvqd.TyLeChuyenDoi)  AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgayLapHoaDon < @timeEnd
    	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_HoaDon 
    
    UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    AND bhd.NgaySua < @timeEnd
    	AND dvqd.ID_HangHoa = @ID_HangHoa
    GROUP BY dvqd.ID_HangHoa
    ) AS td
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhTonTheoLoHangHoa]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	 SELECT [dbo].[FUNC_TonLuyKeTruocThoiGian](@ID_ChiNhanh, @ID_HangHoa, @ID_LoHang, @timeEnd) as TonKho, CAST(ISNULL(gv.GiaVon, 0) as float) as GiaVon, CAST(ISNULL(tbl1.DonGia, 0) as float) as GiaNhap FROM 
	DonViQuiDoi dvqd
	LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and (gv.ID_LoHang = @ID_LoHang or gv.ID_LoHang is null)
	LEFT JOIn
		(select TOP(1) ct.ID_DonViQuiDoi, ct.DonGia from BH_HoaDon hd INNER JOIN
		BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
		where hd.LoaiHoaDon = '4' and ct.ID_LoHang = @ID_LoHang and hd.ID_DonVi = @ID_ChiNhanh
		ORDER BY NgayLapHoaDon DESC) as tbl1 on dvqd.ID = tbl1.ID_DonViQuiDoi
		WHERE dvqd.ID_HangHoa = @ID_HangHoa
END
");

            CreateStoredProcedure(name: "[dbo].[getList_ChietKhauNhanVienTongHop]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                MaNhanVien = p.String(),
                MaNhanVien_TV = p.String(),
                LaDoanhThu = p.String(),
                LaThucThu = p.String(),
                DateFrom = p.String(),
                DateTo = p.String(),
                Status_ColumHideHD = p.Int(),
                Status_ColumHideHH = p.Int()
            }, body: @"SET NOCOUNT ON;
	DECLARE @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float)
	INSERT INTO @tab_DoanhSo exec getList_ChietKhauNhanVienTheoDoanhSo @ID_ChiNhanhs, @MaNhanVien, @MaNhanVien_TV, @LaDoanhThu, @LaThucThu, @DateFrom, @DateTo;

	DECLARE @tab_HoaDon TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float)
	INSERT INTO @tab_HoaDon exec SP_ReportDiscountInvoice @ID_ChiNhanhs, @DateFrom, @DateTo, @Status_ColumHideHD;

	DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucHien float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float)
	INSERT INTO @tab_HangHoa exec SP_ReportDiscountProduct_General @ID_ChiNhanhs, @DateFrom, @DateTo, @Status_ColumHideHH;
	SELECT a.MaNhanVien, a.TenNhanVien, 
	SUM(HoaHongThucHien) as HoaHongThucHien,
	SUM(HoaHongTuVan) as HoaHongTuVan,
	SUM(HoaHongBanGoiDV) as HoaHongBanGoiDV,
	SUM(TongHangHoa) as TongHangHoa,
	SUM(HoaHongDoanhThuHD) as HoaHongDoanhThuHD,
	SUM(HoaHongThucThuHD) as HoaHongThucThuHD,
	SUM(HoaHongVND) as HoaHongVND,
	SUM(TongHoaDon) as TongHoaDon,
	SUM(DoanhThu) as DoanhThu,
	SUM(ThucThu) as ThucThu,
	SUM(HoaHongDoanhThuDS) as HoaHongDoanhThuDS,
	SUM(HoaHongThucThuDS) as HoaHongThucThuDS,
	SUM(TongDoanhSo) as TongDoanhSo,
	SUM(TongDoanhSo + TongHoaDon + TongHangHoa) as Tong
	FROM 
	(
	select MaNhanVien, TenNhanVien, 
	HoaHongThucHien, 
	HoaHongTuVan, 
	HoaHongBanGoiDV, 
	Tong as TongHangHoa,
	0 as HoaHongDoanhThuHD,
	0 as HoaHongThucThuHD,
	0 as HoaHongVND,
	0 as TongHoaDon,
	0 as DoanhThu,
	0 as ThucThu,
	0 as HoaHongDoanhThuDS,
	0 as HoaHongThucThuDS,
	0 as TongDoanhSo
	from @tab_HangHoa
	UNION ALL
	Select MaNhanVien, TenNhanVien, 
	0 as HoaHongThucHien,
	0 as HoaHongTuVan,
	0 as HoaHongBanGoiDV,
	0 as TongHangHoa,
	HoaHongDoanhThu as HoaHongDoanhThuHD,
	HoaHongThucThu as HoaHongThucThuHD,
	HoaHongVND,
	TongAll as TongHoaDon,
	0 as DoanhThu,
	0 as ThucThu,
	0 as HoaHongDoanhThuDS,
	0 as HoaHongThucThuDS,
	0 as TongDoanhSo
	from @tab_HoaDon
	UNION ALL
	Select MaNhanVien, TenNhanVien, 
	0 as HoaHongThucHien,
	0 as HoaHongTuVan,
	0 as HoaHongBanGoiDV,
	0 as TongHangHoa,
	0 as HoaHongDoanhThuHD,
	0 as HoaHongThucThuHD,
	0 as HoaHongVND,
	0 as TongHoaDon,
	TongDoanhThu as DoanhThu,
	TongThucThu as ThucThu,
	HoaHongDoanhThu as HoaHongDoanhThuDS,
	HoaHongThucThu as HoaHongThucThuDS,
	TongAll as TongDoanhSo
	from @tab_DoanhSo
	) as a
	GROUP BY a.MaNhanVien, a.TenNhanVien");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GetHoaDonThanhToanNhaBep]");
            DropStoredProcedure("[dbo].[UpdateTonLuyKeTheoHoaDon]");
            DropStoredProcedure("[dbo].[UpdateTonLuyKeInit]");
            DropStoredProcedure("[dbo].[getList_ChietKhauNhanVienTongHop]");
        }
    }
}
