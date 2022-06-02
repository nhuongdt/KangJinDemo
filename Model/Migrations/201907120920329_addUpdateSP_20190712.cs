namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20190712 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTon]", parametersAction: p => new
            {
                ID_DonVi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                SearchString = p.String(),
                ID_NhomHang = p.Guid(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid(),
                CoPhatSinh = p.Int()
            }, body: @"SET NOCOUNT ON;
    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblTonDauKy TABLE(TenNhomHang NVARCHAR(MAX), IDHangHoa UNIQUEIDENTIFIER, MaHangHoa NVARCHAR(MAX), TenHangHoaFull NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX),
	ThuocTinh_GiaTri NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), IDLoHang UNIQUEIDENTIFIER, TenLoHang NVARCHAR(MAX), TonDauKy FLOAT, QuyCach FLOAT, GiaTriDauKy FLOAT)
	INSERT INTO @tblTonDauKy
	SELECT nhh.TenNhomHangHoa,
	dvqd1.ID_HangHoa, 
    dvqd1.MaHangHoa, 
    dhh.TenHangHoa + dvqd1.ThuocTinhGiaTri, 
    dhh.TenHangHoa, 
    dvqd1.ThuocTinhGiaTri, 
    dvqd1.TenDonViTinh, 
	lh.ID,
    ISNULL(lh.MaLoHang, ''), 
    ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0),
	ISNULL(dhh.QuyCach, 0),
    IIF(@XemGiaVon = '1', ROUND(ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0) * ISNULL(gv.GiaVon, 0), 0), 0)
	FROM DM_HangHoa dhh
    LEFT JOIN DM_LoHang lh
    ON dhh.ID = lh.ID_HangHoa
    LEFT JOIN
    (SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang ORDER BY tbltemp.ThoiGian DESC) AS RN FROM (
	SELECT hd.LoaiHoaDon, dvqd.ID_HangHoa, hd.ID_CheckIn, hdct.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang, hdct.ID_LoHang , 
	IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_DonVi, hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
	FROM BH_HoaDon_ChiTiet hdct
	INNER JOIN BH_HoaDon hd
	ON hd.ID = hdct.ID_HoaDon
	INNER JOIN DonViQuiDoi dvqd
	ON dvqd.ID = hdct.ID_DonViQuiDoi
	WHERE ((hd.ID_DonVi = @ID_DonVi and ((hd.YeuCau  != '2' AND hd.YeuCau != '3') OR hd.YeuCau IS NULL)) OR (hd.ID_CheckIn = @ID_DonVi AND hd.YeuCau = '4')) AND hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10)) as tbltemp
	WHERE tbltemp.ThoiGian < @timeStart) tonkho
    ON dhh.ID = tonkho.ID_HangHoa AND (lh.ID = tonkho.ID_LoHang OR tonkho.ID_LoHang IS NULL)
    INNER JOIN DonViQuiDoi dvqd1
    ON dhh.ID = dvqd1.ID_HangHoa
    LEFT JOIN DM_GiaVon gv
    ON gv.ID_DonViQuiDoi = dvqd1.ID AND (gv.ID_LoHang IS NULL OR lh.ID = gv.ID_LoHang) AND gv.ID_DonVi = @ID_DonVi
    INNER JOIN DM_NhomHangHoa nhh
    ON nhh.ID = dhh.ID_NhomHang
    INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh
    ON nhh.ID = allnhh.ID
    WHERE (tonkho.RN = 1 or tonkho.RN is null) AND dhh.LaHangHoa = 1 AND dhh.TheoDoi LIKE @TheoDoi AND dvqd1.Xoa LIKE @TrangThai AND dvqd1.LaDonViChuan = 1
	AND ((select count(Name) from @tblSearchString b where 
    		dhh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or dhh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			or dhh.TenHangHoa like '%'+b.Name+'%'
    			or lh.MaLoHang like '%' +b.Name +'%' 
    		or dvqd1.MaHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    			or dvqd1.TenDonViTinh like '%'+b.Name+'%'
    			or dvqd1.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0);

	DECLARE @tblPhatSinhTrongKy TABLE(ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, SoLuongNhap FLOAT,
	GiaTriNhap FLOAT, SoLuongXuat FLOAT, GiaTriXuat FLOAT);
	INSERT INTO @tblPhatSinhTrongKy
	SELECT pstk.ID_HangHoa, pstk.ID_LoHang, SUM(pstk.SoLuongNhap), ROUND(SUM(pstk.GiaTriNhap),0),
	SUM(pstk.SoLuongXuat), ROUND(SUM(pstk.GiaTriXuat), 0) FROM 
	(SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap,
    0 AS GiaTriNhap,
    SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
	SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0 AND hh.LaHangHoa = 1
    AND bhd.ID_DonVi = @ID_DonVi
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
	UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap,
    0 AS GiaTriNhap,
    SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
	SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_DonVi and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_DonVi
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
	UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
	SUM(bhdct.TienChietKhau* bhdct.GiaVon) AS GiaTriNhap,
    0 AS SoLuongXuat,
    0 AS GiaTriXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_DonVi and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4' AND bhd.ChoThanhToan = 0
    AND bhd.NgaySua >= @timeStart AND bhd.NgaySua < @timeEnd
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
	UNION ALL
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
	SUM(bhdct.SoLuong* bhdct.GiaVon) AS GiaTriNhap,
    0 AS SoLuongXuat,
    0 AS GiaTriXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_DonVi
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang

	UNION ALL
	SELECT ctkk.ID_HangHoa, ctkk.ID_LoHang, IIF(ctkk.SoLuong >= 0, ctkk.SoLuong, 0) AS SoLuongNhap,
	IIF(ctkk.SoLuong >= 0, ctkk.GiaTri, 0) AS GiaTriNhap, IIF(ctkk.SoLuong < 0, ctkk.SoLuong * (-1), 0) AS SoLuongXuat, 
	IIF(ctkk.SoLuong < 0, ctkk.GiaTri * (-1), 0) AS GiaTriXuat FROM
	(SELECT 
	dvqd.ID_HangHoa,
	bhdct.ID_LoHang,
	SUM(bhdct.ThanhTien * dvqd.TyLeChuyenDoi) - MAX(TienChietKhau * dvqd.TyLeChuyenDoi) AS SoLuong,
	SUM(bhdct.ThanhTien * bhdct.GiaVon) - MAX(TienChietKhau * bhdct.GiaVon) AS GiaTri
	FROM BH_HoaDon_ChiTiet bhdct
	LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
	WHERE bhd.LoaiHoaDon = '9' 
	AND bhd.ChoThanhToan = 0
	AND bhd.ID_DonVi = @ID_DonVi
	AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
	GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang, bhd.ID) ctkk) pstk
	GROUP BY pstk.ID_HangHoa, pstk.ID_LoHang

	IF(@CoPhatSinh = 1)
	BEGIN
		SELECT dk.TenNhomHang, dk.MaHangHoa, dk.TenHangHoaFull, dk.TenHangHoa, dk.ThuocTinh_GiaTri, dk.TenDonViTinh, dk.TenLoHang, dk.TonDauKy, 
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy, 0) AS GiaTriDauKy, ISNULL(tk.SoLuongNhap, 0) AS SoLuongNhap, IIF(@XemGiaVon = '1', ISNULL(tk.GiaTriNhap, 0), 0) AS GiaTriNhap, 
		ISNULL(tk.SoLuongXuat, 0) AS SoLuongXuat, IIF(@XemGiaVon = '1', ISNULL(tk.GiaTriXuat, 0), 0) AS GiaTriXuat, dk.TonDauKy + ISNULL(tk.SoLuongNhap, 0) - ISNULL(tk.SoLuongXuat, 0) AS TonCuoiKy, 
		(dk.TonDauKy + ISNULL(tk.SoLuongNhap, 0) - ISNULL(tk.SoLuongXuat, 0))*dk.QuyCach AS TonQuyCach,
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy + ISNULL(tk.GiaTriNhap, 0) - ISNULL(tk.GiaTriXuat, 0), 0) AS GiaTriCuoiKy FROM @tblTonDauKy dk
		LEFT JOIN @tblPhatSinhTrongKy tk
		ON dk.IDHangHoa = tk.ID_HangHoa AND (dk.IDLoHang IS NULL OR dk.IDLoHang = tk.ID_LoHang)
	END
	ELSE
	BEGIN
		SELECT dk.TenNhomHang, dk.MaHangHoa, dk.TenHangHoaFull, dk.TenHangHoa, dk.ThuocTinh_GiaTri, dk.TenDonViTinh, dk.TenLoHang, dk.TonDauKy, 
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy, 0) AS GiaTriDauKy, ISNULL(tk.SoLuongNhap, 0) AS SoLuongNhap, IIF(@XemGiaVon = '1', ISNULL(tk.GiaTriNhap, 0), 0) AS GiaTriNhap, 
		ISNULL(tk.SoLuongXuat, 0) AS SoLuongXuat, IIF(@XemGiaVon = '1', ISNULL(tk.GiaTriXuat, 0), 0) AS GiaTriXuat, dk.TonDauKy + ISNULL(tk.SoLuongNhap, 0) - ISNULL(tk.SoLuongXuat, 0) AS TonCuoiKy, 
		(dk.TonDauKy + ISNULL(tk.SoLuongNhap, 0) - ISNULL(tk.SoLuongXuat, 0))*dk.QuyCach AS TonQuyCach,
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy + ISNULL(tk.GiaTriNhap, 0) - ISNULL(tk.GiaTriXuat, 0), 0) AS GiaTriCuoiKy FROM @tblTonDauKy dk
		INNER JOIN @tblPhatSinhTrongKy tk
		ON dk.IDHangHoa = tk.ID_HangHoa AND (dk.IDLoHang IS NULL OR dk.IDLoHang = tk.ID_LoHang)
	END");

            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_NhapXuatTonChiTiet]", parametersAction: p => new
            {
                ID_DonVi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                SearchString = p.String(),
                ID_NhomHang = p.Guid(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid(),
                CoPhatSinh = p.Int()
            }, body: @"SET NOCOUNT ON;
    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblTonDauKy TABLE(TenNhomHang NVARCHAR(MAX), IDHangHoa UNIQUEIDENTIFIER, MaHangHoa NVARCHAR(MAX), TenHangHoaFull NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX),
	ThuocTinh_GiaTri NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), IDLoHang UNIQUEIDENTIFIER, TenLoHang NVARCHAR(MAX), TonDauKy FLOAT, QuyCach FLOAT, GiaTriDauKy FLOAT)
	INSERT INTO @tblTonDauKy
	SELECT nhh.TenNhomHangHoa,
	dvqd1.ID_HangHoa, 
    dvqd1.MaHangHoa, 
    dhh.TenHangHoa + dvqd1.ThuocTinhGiaTri, 
    dhh.TenHangHoa, 
    dvqd1.ThuocTinhGiaTri, 
    dvqd1.TenDonViTinh, 
	lh.ID,
    ISNULL(lh.MaLoHang, ''), 
    ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0),
	ISNULL(dhh.QuyCach, 0),
    IIF(@XemGiaVon = '1', ROUND(ISNULL(IIF(tonkho.LoaiHoaDon = 10 AND tonkho.ID_CheckIn = @ID_DonVi, tonkho.TonLuyKe_NhanChuyenHang, tonkho.TonLuyKe), 0) * ISNULL(gv.GiaVon, 0), 0), 0)
	FROM DM_HangHoa dhh
    LEFT JOIN DM_LoHang lh
    ON dhh.ID = lh.ID_HangHoa
    LEFT JOIN
    (SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang ORDER BY tbltemp.ThoiGian DESC) AS RN FROM (
	SELECT hd.LoaiHoaDon, dvqd.ID_HangHoa, hd.ID_CheckIn, hdct.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang, hdct.ID_LoHang , 
	IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_DonVi, hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
	FROM BH_HoaDon_ChiTiet hdct
	INNER JOIN BH_HoaDon hd
	ON hd.ID = hdct.ID_HoaDon
	INNER JOIN DonViQuiDoi dvqd
	ON dvqd.ID = hdct.ID_DonViQuiDoi
	WHERE ((hd.ID_DonVi = @ID_DonVi and ((hd.YeuCau  != '2' AND hd.YeuCau != '3') OR hd.YeuCau IS NULL)) OR (hd.ID_CheckIn = @ID_DonVi AND hd.YeuCau = '4')) AND hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10)) as tbltemp
	WHERE tbltemp.ThoiGian < @timeStart) tonkho
    ON dhh.ID = tonkho.ID_HangHoa AND (lh.ID = tonkho.ID_LoHang OR tonkho.ID_LoHang IS NULL)
    INNER JOIN DonViQuiDoi dvqd1
    ON dhh.ID = dvqd1.ID_HangHoa
    LEFT JOIN DM_GiaVon gv
    ON gv.ID_DonViQuiDoi = dvqd1.ID AND (gv.ID_LoHang IS NULL OR lh.ID = gv.ID_LoHang) AND gv.ID_DonVi = @ID_DonVi
    INNER JOIN DM_NhomHangHoa nhh
    ON nhh.ID = dhh.ID_NhomHang
    INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh
    ON nhh.ID = allnhh.ID
    WHERE (tonkho.RN = 1 or tonkho.RN is null) AND dhh.LaHangHoa = 1 AND dhh.TheoDoi LIKE @TheoDoi AND dvqd1.Xoa LIKE @TrangThai AND dvqd1.LaDonViChuan = 1
	AND ((select count(Name) from @tblSearchString b where 
    		dhh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or dhh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			or dhh.TenHangHoa like '%'+b.Name+'%'
    			or lh.MaLoHang like '%' +b.Name +'%' 
    		or dvqd1.MaHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    			or dvqd1.TenDonViTinh like '%'+b.Name+'%'
    			or dvqd1.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0);

	DECLARE @tblPhatSinhTrongKy TABLE(ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, SoLuongNhap_NCC FLOAT,
	SoLuongNhap_Kiem FLOAT, SoLuongNhap_Tra FLOAT, SoLuongNhap_Chuyen FLOAT, SoLuongNhap_SX FLOAT, SoLuongXuat_Ban FLOAT, SoLuongXuat_Huy FLOAT,
	SoLuongXuat_NCC FLOAT, SoLuongXuat_Kiem FLOAT, SoLuongXuat_Chuyen FLOAT, SoLuongXuat_SX FLOAT, GiaTri FLOAT, LuyKe FLOAT);
	INSERT INTO @tblPhatSinhTrongKy
	SELECT
    pstk.ID_HangHoa,
    pstk.ID_LoHang,
    SUM(pstk.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
    SUM(pstk.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
    SUM(pstk.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
    SUM(pstk.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
    SUM(pstk.SoLuongNhap_SX) AS SoLuongNhap_SX,
    SUM(pstk.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
    SUM(pstk.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
    SUM(pstk.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
    SUM(pstk.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
    SUM(pstk.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,
    SUM(pstk.SoLuongXuat_SX) AS SoLuongXuat_SX,
    ROUND(SUM(pstk.GiaTri),0) AS GiaTri,
	SUM(pstk.SoLuongNhap_NCC) + SUM(pstk.SoLuongNhap_Kiem) + SUM(pstk.SoLuongNhap_Tra) + SUM(pstk.SoLuongNhap_Chuyen)- SUM(pstk.SoLuongXuat_Ban) - SUM(pstk.SoLuongXuat_Huy)  - SUM(pstk.SoLuongXuat_NCC)  - SUM(pstk.SoLuongXuat_Kiem)  - SUM(pstk.SoLuongXuat_Chuyen) AS LuyKe
    FROM 
    (
    	-- Xuất bán
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap_NCC,
    0 AS SoLuongNhap_Kiem,
    0 AS SoLuongNhap_Tra,
    0 AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Ban,
    0 AS SoLuongXuat_Huy,
    0 AS SoLuongXuat_NCC,
    0 AS SoLuongXuat_Kiem,
    0 AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    SUM(bhdct.SoLuong * bhdct.GiaVon) * (-1) AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0 AND hh.LaHangHoa = 1
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_DonVi
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
                                                                                                                                                                                                                                                            
    UNION ALL
    -- trả hàng nhà cung cấp
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap_NCC,
    0 AS SoLuongNhap_Kiem,
    0 AS SoLuongNhap_Tra,
    0 AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    0 AS SoLuongXuat_Ban,
    0 AS SoLuongXuat_Huy,
    SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_NCC,
    0 AS SoLuongXuat_Kiem, 
    0 AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    SUM(bhdct.SoLuong * bhdct.GiaVon) * (-1) AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_DonVi
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
    UNION ALL
    
    -- Xuất hủy
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap_NCC,
    0 AS SoLuongNhap_Kiem,
    0 AS SoLuongNhap_Tra,
    0 AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    0 AS SoLuongXuat_Ban,
    SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Huy,
    0 AS SoLuongXuat_NCC,
    0 AS SoLuongXuat_Kiem, 
    0 AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    SUM(bhdct.SoLuong * bhdct.GiaVon) * (-1) AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_DonVi
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
    UNION ALL
    -- Xuất Chuyển hàng
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap_NCC,
    0 AS SoLuongNhap_Kiem,
    0 AS SoLuongNhap_Tra,
    0 AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    0 AS SoLuongXuat_Ban,
    0 AS SoLuongXuat_Huy,
    0 AS SoLuongXuat_NCC,
    0 AS SoLuongXuat_Kiem, 
    SUM(bhdct.tienchietkhau* dvqd.TyLeChuyenDoi) AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    SUM(bhdct.SoLuong * bhdct.GiaVon) * (-1) AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_DonVi and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_DonVi
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
    UNION ALL
	-- Xuất kiểm kê
	SELECT 
    pkk.ID_HangHoa,
    pkk.ID_LoHang,
    0 AS SoLuongNhap_NCC,
	IIF(pkk.SoLuong >= 0, pkk.SoLuong, 0) AS SoLuongNhap_Kiem, 
    0 AS SoLuongNhap_Tra,
    0 AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    0 AS SoLuongXuat_Ban,
    0 AS SoLuongXuat_Huy,
    0 AS SoLuongXuat_NCC,
	IIF(pkk.SoLuong < 0, pkk.SoLuong *(-1), 0) AS SoLuongXuat_Kiem,
    0 AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    pkk.GiaTri AS GiaTri
	FROM
	(SELECT dvqd.ID_HangHoa, bhdct.ID_LoHang, SUM(bhdct.ThanhTien * dvqd.TyLeChuyenDoi) - MAX(TienChietKhau * dvqd.TyLeChuyenDoi) AS SoLuong, 
	(SUM(bhdct.ThanhTien * dvqd.TyLeChuyenDoi) - MAX(bhdct.TienChietKhau * dvqd.TyLeChuyenDoi)) * MAX(bhdct.GiaVon / dvqd.TyLeChuyenDoi)  AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_DonVi
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang, bhd.ID) pkk
    UNION ALL
    -- Nhập NCC
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_NCC,
    0 AS SoLuongNhap_Kiem,
    0 AS SoLuongNhap_Tra,
    0 AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    0 AS SoLuongXuat_Ban,
    0 AS SoLuongXuat_Huy,
    0 AS SoLuongXuat_NCC,
    0 AS SoLuongXuat_Kiem, 
    0 AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    SUM(bhdct.SoLuong * bhdct.GiaVon) AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_DonVi
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
    
    UNION ALL
    -- Nhập Khách trả hàng
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap_NCC,
    0 AS SoLuongNhap_Kiem,
    SUM(bhdct.SoLuong* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Tra,
    0 AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    0 AS SoLuongXuat_Ban,
    0 AS SoLuongXuat_Huy,
    0 AS SoLuongXuat_NCC,
    0 AS SoLuongXuat_Kiem, 
    0 AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    SUM(bhdct.SoLuong * bhdct.GiaVon) * (-1) AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    AND bhd.ID_DonVi = @ID_DonVi
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang
    UNION ALL
    -- Nhập chuyển hàng
    SELECT 
    dvqd.ID_HangHoa,
    bhdct.ID_LoHang,
    0 AS SoLuongNhap_NCC,
    0 AS SoLuongNhap_Kiem,
    0 AS SoLuongNhap_Tra,
    SUM(bhdct.TienChietKhau* dvqd.TyLeChuyenDoi) AS SoLuongNhap_Chuyen,
    0 AS SoLuongNhap_SX,
    0 AS SoLuongXuat_Ban,
    0 AS SoLuongXuat_Huy,
    0 AS SoLuongXuat_NCC,
    0 AS SoLuongXuat_Kiem, 
    0 AS SoLuongXuat_Chuyen,
    0 AS SoLuongXuat_SX,
    SUM(bhdct.SoLuong * bhdct.GiaVon) * (-1) AS GiaTri
    FROM BH_HoaDon_ChiTiet bhdct   
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_DonVi and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa = 0 and bhd.ChoThanhToan = 0
    AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    GROUP BY dvqd.ID_HangHoa, bhdct.ID_LoHang) pstk
	GROUP BY pstk.ID_HangHoa, pstk.ID_LoHang

	IF(@CoPhatSinh = 1)
	BEGIN
		SELECT dk.TenNhomHang, dk.MaHangHoa, dk.TenHangHoaFull, dk.TenHangHoa, dk.ThuocTinh_GiaTri, dk.TenDonViTinh, dk.TenLoHang, dk.TonDauKy, 
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy, 0) AS GiaTriDauKy,
		ISNULL(tk.SoLuongNhap_NCC, 0) AS SoLuongNhap_NCC,
		ISNULL(tk.SoLuongNhap_Kiem, 0) AS SoLuongNhap_Kiem,
		ISNULL(tk.SoLuongNhap_Tra, 0) AS SoLuongNhap_Tra,
		ISNULL(tk.SoLuongNhap_Chuyen, 0) AS SoLuongNhap_Chuyen,
		ISNULL(tk.SoLuongNhap_SX, 0) AS SoLuongNhap_SX,
		ISNULL(tk.SoLuongXuat_Ban, 0) AS SoLuongXuat_Ban,
		ISNULL(tk.SoLuongXuat_Huy, 0) AS SoLuongXuat_Huy,
		ISNULL(tk.SoLuongXuat_NCC, 0) AS SoLuongXuat_NCC,
		ISNULL(tk.SoLuongXuat_Kiem, 0) AS SoLuongXuat_Kiem,
		ISNULL(tk.SoLuongXuat_Chuyen, 0) AS SoLuongXuat_Chuyen,
		ISNULL(tk.SoLuongXuat_SX, 0) AS SoLuongXuat_SX,
		dk.TonDauKy + ISNULL(tk.LuyKe, 0) AS TonCuoiKy, 
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy + ISNULL(tk.GiaTri, 0), 0) AS GiaTriCuoiKy 
		FROM @tblTonDauKy dk
		LEFT JOIN @tblPhatSinhTrongKy tk
		ON dk.IDHangHoa = tk.ID_HangHoa AND (dk.IDLoHang IS NULL OR dk.IDLoHang = tk.ID_LoHang)
	END
	ELSE
	BEGIN
		SELECT dk.TenNhomHang, dk.MaHangHoa, dk.TenHangHoaFull, dk.TenHangHoa, dk.ThuocTinh_GiaTri, dk.TenDonViTinh, dk.TenLoHang, dk.TonDauKy, 
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy, 0) AS GiaTriDauKy,
		ISNULL(tk.SoLuongNhap_NCC, 0) AS SoLuongNhap_NCC,
		ISNULL(tk.SoLuongNhap_Kiem, 0) AS SoLuongNhap_Kiem,
		ISNULL(tk.SoLuongNhap_Tra, 0) AS SoLuongNhap_Tra,
		ISNULL(tk.SoLuongNhap_Chuyen, 0) AS SoLuongNhap_Chuyen,
		ISNULL(tk.SoLuongNhap_SX, 0) AS SoLuongNhap_SX,
		ISNULL(tk.SoLuongXuat_Ban, 0) AS SoLuongXuat_Ban,
		ISNULL(tk.SoLuongXuat_Huy, 0) AS SoLuongXuat_Huy,
		ISNULL(tk.SoLuongXuat_NCC, 0) AS SoLuongXuat_NCC,
		ISNULL(tk.SoLuongXuat_Kiem, 0) AS SoLuongXuat_Kiem,
		ISNULL(tk.SoLuongXuat_Chuyen, 0) AS SoLuongXuat_Chuyen,
		ISNULL(tk.SoLuongXuat_SX, 0) AS SoLuongXuat_SX,
		dk.TonDauKy + ISNULL(tk.LuyKe, 0) AS TonCuoiKy, 
		IIF(@XemGiaVon = '1', dk.GiaTriDauKy + ISNULL(tk.GiaTri, 0), 0) AS GiaTriCuoiKy 
		FROM @tblTonDauKy dk
		INNER JOIN @tblPhatSinhTrongKy tk
		ON dk.IDHangHoa = tk.ID_HangHoa AND (dk.IDLoHang IS NULL OR dk.IDLoHang = tk.ID_LoHang)
	END");

            CreateStoredProcedure(name: "[dbo].[GetTPDinhLuong_ofCTHD]", parametersAction: p => new
            {
                ID_CTHD = p.Guid()
            }, body: @"SET NOCOUNT ON;

    select MaHangHoa, TenHangHoa, ID_DonViQuiDoi, TenDonViTinh, SoLuong, ct.GiaVon, ID_HoaDon,ID_ChiTietGoiDV,SoLuongDinhLuong_BanDau,
			case when ISNULL(QuyCach,0) = 0 then ISNULL(TyLeChuyenDoi,1) else ISNULL(QuyCach,0) * ISNULL(TyLeChuyenDoi,1) end as QuyCach,
			qd.TenDonViTinh as DonViTinhQuyCach, ct.GhiChu		
	from BH_HoaDon_ChiTiet ct
	Join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	where ID_ChiTietDinhLuong = @ID_CTHD and ct.ID != @ID_CTHD");

            CreateStoredProcedure(name: "[dbo].[SP_DeleteHoaDon_byID]", parametersAction: p => new
            {
                ID = p.Guid()
            }, body: @"SET NOCOUNT ON;

    delete from BH_NhanVienThucHien where ID_HoaDon= @ID
	delete from BH_NhanVienThucHien where exists (select ID from BH_HoaDon_ChiTiet where ID = BH_NhanVienThucHien.ID_ChiTietHoaDon and ID_HoaDon= @ID)
    delete from BH_HoaDon_ChiTiet where ID_HoaDon = @ID
    delete from BH_HoaDon where ID = @ID");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetHoaDonChoThanhToan]
    @LoaiHoaDon [int],
	@ID_DonVi varchar(40)
AS
BEGIN
    select 
    	hd.ID, hd.ID_DoiTuong, hd.ID_NhanVien, hd.ID_ViTri, hd.ID_DonVi, hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienThue, hd.TongGiamGia, hd.TongChietKhau, 
    	hd.TongChiPhi, hd.TongTienHang, ISNULL(hd.ChoThanhToan,'0') as ChoThanhToan,
    	hd.DienGiai,CAST(ISNULL(hd.KhuyeMai_GiamGia, 0) as float) as KhuyeMai_GiamGia , ISNULL(hd.YeuCau,'') as YeuCau, hd.LoaiHoaDon,
    	dv.TenDonVi, ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, ISNULL(nv.TenNhanVien,'') as TenNhanVien,
    	ISNULL(vt.TenViTri,'') as TenViTri, ISNULL(gb.TenGiaBan, N'Bảng giá chung') as TenGiaBan,
    	ISNULL(dt.DienThoai,'') as DienThoai, ISNULL(dt.Email,'') as Email,
		hd.NguoiTao,
		CAST(ISNULL(KhachDaTra,0) as float) as KhachDaTra,
		hd.PhaiThanhToan - ISNULL(KhachDaTra,0) as PhaiThanhToan,
		ISNULL(hd.ID_BangGia,'00000000-0000-0000-0000-000000000000') as ID_BangGia,
		ISNULL(dh.MaHoaDon,'') as MaHoaDonGoc, -- neu DatHang, sau do LuuTam --> get MaHoaDonDatHang used to check delete HDDatHang at .js
		hd.ID_HoaDon -- ID_HoaDon DatHang --> used to update TrangThai HDDatHang after XuLy
    from BH_HoaDon hd
	left join BH_HoaDon dh on hd.ID_HoaDon= dh.ID
    left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    left join DM_DonVi dv on hd.ID_DonVi= dv.ID
    left join DM_ViTri vt on hd.ID_ViTri= vt.ID
    left join DM_GiaBan gb on  hd.ID_BangGia= gb.ID
    left join (
			select 
				ID_HoaDonLienQuan,
				SUM(ISNULL(a.KhachDaTra, 0)) as KhachDaTra
			from	
				(
				-- get TongThu from HDTamLuu
				Select 					
    				qct.ID_HoaDonLienQuan,
    				Case when qhd.TrangThai='0' then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(qct.Tienthu, 0) else -ISNULL(qct.Tienthu, 0) end end as KhachDaTra					
   				from Quy_HoaDon_ChiTiet qct			 
    			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 				
    			where qhd.ID_DonVi = @ID_DonVi

				union all

				-- get TongThu from HDDatHang
    			select hd.ID as ID_HoaDonLienQuan,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(qct.TienThu, 0) else - ISNULL(qct.TienThu, 0) end end as KhachDaTra
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID				
				left join BH_HoaDon hdd on hdd.ID= qct.ID_HoaDonLienQuan
				left join BH_HoaDon hd on hd.ID_HoaDon= hdd.ID
				where hdd.LoaiHoaDon = '3' and qhd.ID_DonVi = @ID_DonVi
				) a group by a.ID_HoaDonLienQuan
	) tblQuy on tblQuy.ID_HoaDonLienQuan = hd.ID
    where hd.LoaiHoaDon like @LoaiHoaDon and hd.ID_DonVi like @ID_DonVi
	and hd.ChoThanhToan = '1' and hd.MaHoaDon not like '%DH%'
END

--SP_GetHoaDonChoThanhToan 1,'9FBD9BD9-0360-47E8-BD30-20331C7B0A04'");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTon]");
            DropStoredProcedure("[dbo].[BaoCaoKho_NhapXuatTonChiTiet]");
            DropStoredProcedure("[dbo].[GetTPDinhLuong_ofCTHD]");
            DropStoredProcedure("[dbo].[SP_DeleteHoaDon_byID]");
        }
    }
}
