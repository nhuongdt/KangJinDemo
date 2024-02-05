namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20220629 : DbMigration
    {
        public override void Up()
        {
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
		DECLARE @TraLaiSoDu AS FLOAT;

		SELECT @TongNap = SUM(TongTienHang) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @TongDieuChinh = SUM(TongChiPhi) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 23 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @SuDungThe = SUM(qhdct.TienThu) FROM Quy_HoaDon_ChiTiet qhdct
		JOIN Quy_HoaDon qhd
		ON qhdct.ID_HoaDon = qhd.ID
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 11 and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and qhdct.HinhThucThanhToan=4

		SELECT @HoanTraTienThe = SUM(qhdct.TienThu) FROM Quy_HoaDon_ChiTiet qhdct
		JOIN Quy_HoaDon qhd ON qhdct.ID_HoaDon = qhd.ID		
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 12 and (qhd.TrangThai = 1 or qhd.TrangThai is null)
			and qhdct.HinhThucThanhToan=4

		SELECT @TraLaiSoDu = SUM(hd.TongTienHang) FROM BH_HoaDon hd			
		WHERE hd.ID_DoiTuong = @ID_DoiTuong AND hd.NgayLapHoaDon < @Time and hd.LoaiHoaDon = 32 and hd.ChoThanhToan= 0		

		SET @SoDu = ISNULL(@TongNap, 0) +  ISNULL(@TongDieuChinh, 0)  - ISNULL(@SuDungThe, 0) + ISNULL(@HoanTraTienThe,0) - ISNULL(@TraLaiSoDu, 0);
	RETURN @SoDu
END");

			CreateStoredProcedure(name: "[dbo].[GetQuyHoaDon_byIDHoaDon]", parametersAction: p => new
			{
				ID = p.Guid(),
				ID_Parent = p.Guid()
			}, body: @"SET NOCOUNT ON;

	declare @tblThuDatHang table
		(ID uniqueidentifier, LoaiHoaDon int, MaHoaDon nvarchar(max), NgayLapHoaDon datetime,TongTienThu float,
		NguoiTao nvarchar(max), NguoiSua nvarchar(max), TrangThai bit, PhuongThucTT nvarchar(max))

	declare @tblThuHoaDon table
		(ID uniqueidentifier, LoaiHoaDon int,MaHoaDon nvarchar(max), NgayLapHoaDon datetime,TongTienThu float,
		NguoiTao nvarchar(max), NguoiSua nvarchar(max), TrangThai bit, PhuongThucTT nvarchar(max))


    ----- get hd if was create from hdDatHang
	declare @isFirst bit ='0'
	if ( @ID = (select top 1 hd.ID
		from BH_HoaDon hd where hd.ID_HoaDon= @ID_Parent
		and hd.ChoThanhToan='0'
		order by hd.NgayApDungGoiDV
		))
	begin		
		----- get phieu thu dathang
				insert into @tblThuDatHang		
				select 
						tblMain.ID,
						qhd.LoaiHoaDon,
						qhd.MaHoaDon,
						qhd.NgayLapHoaDon,
						sum(qct.TienThu) as TongTienThu,
						qhd.NguoiTao,
						qhd.NguoiSua,
						qhd.TrangThai,								
						max(Left(tblMain.sPhuongThuc,Len(tblMain.sPhuongThuc)-1)) As PhuongThucTT	
					from
					(
						Select distinct hdXML.ID, 							
						 (
							select distinct (case qct.HinhThucThanhToan
										when 1 then N'Tiền mặt'
										when 2 then N'POS'
										when 3 then N'Chuyển khoản'
										when 4 then N'Thẻ giá trị'
										when 5 then N'Điểm'
										when 6 then iif(qhd.LoaiHoaDon=11, N'Thu từ cọc', N'Chi từ cọc')
									else '' end) +', '  AS [text()]
							from Quy_HoaDon_ChiTiet qct
							join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID						
							where qct.ID_HoaDon = hdXML.ID
							and qct.ID_HoaDonLienQuan = @ID_Parent
							and (qhd.TrangThai is null or qhd.TrangThai='1')
							For XML PATH ('')
						) sPhuongThuc		
				From Quy_HoaDon hdXML				
				) tblMain 
				 left join Quy_HoaDon qhd on tblMain.ID = qhd.ID			
				left join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
				where qct.ID_HoaDonLienQuan = @ID_Parent
				group by tblMain.ID	, 
						qhd.LoaiHoaDon,
						qhd.MaHoaDon,
						qhd.NgayLapHoaDon,		
						qhd.NguoiTao,
						qhd.NguoiSua,
						qhd.TrangThai	
	end


	----- get phieu thu hoadon (thu cua chinh no): lấy cả phiếu thu đã hủy
		insert into @tblThuHoaDon		
		select 
						tblMain.ID,
						qhd.LoaiHoaDon,
						qhd.MaHoaDon,
						qhd.NgayLapHoaDon,
						sum(qct.TienThu) as TongTienThu,
						qhd.NguoiTao,
						qhd.NguoiSua,
						qhd.TrangThai,								
						max(Left(tblMain.sPhuongThuc,Len(tblMain.sPhuongThuc)-1)) As PhuongThucTT	
					from
					(
						Select distinct hdXML.ID, 							
						 (
							select distinct (case qct.HinhThucThanhToan
										when 1 then N'Tiền mặt'
										when 2 then N'POS'
										when 3 then N'Chuyển khoản'
										when 4 then N'Thẻ giá trị'
										when 5 then N'Điểm'
										when 6 then iif(qhd.LoaiHoaDon=11, N'Thu từ cọc', N'Chi từ cọc')
									else '' end) +', '  AS [text()]
							from Quy_HoaDon_ChiTiet qct
							join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID						
							where qct.ID_HoaDon = hdXML.ID
							and qct.ID_HoaDonLienQuan = @ID
							For XML PATH ('')
						) sPhuongThuc		
				From Quy_HoaDon hdXML				
				) tblMain 
				 left join Quy_HoaDon qhd on tblMain.ID = qhd.ID			
				left join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
				where qct.ID_HoaDonLienQuan = @ID
				group by tblMain.ID	,
						qhd.LoaiHoaDon,
						qhd.MaHoaDon,
						qhd.NgayLapHoaDon,		
						qhd.NguoiTao,
						qhd.NguoiSua,
						qhd.TrangThai	
	
		select *,
			iif(tbl.LoaiHoaDon=11, N'Phiếu thu', N'Phiếu chi') as SLoaiHoaDon
		from
		(
		select *
		from @tblThuDatHang thuDH
		union all
		select *
		from @tblThuHoaDon thuHD
		) tbl order by tbl.NgayLapHoaDon desc");

			Sql(@"ALTER PROCEDURE [dbo].[getListNhanVien_allDonVi]
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
    Select 
    	nv.ID,
    	nv.MaNhanVien,
		nv.TenNhanVien,
		max(ct.ID_PhongBan) as ID_PhongBan
		--'00000000-0000-0000-0000-000000000000' as ID_PhongBan -- add avoid error when return class NS_NhanVien_DonVi
    	From NS_NhanVien nv
    	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
    	where ct.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh)) 
		and (nv.TrangThai is null or nv.TrangThai = 1) and nv.DaNghiViec = 0
    	GROUP by nv.ID, nv.MaNhanVien, nv.TenNhanVien
END");

			Sql(@"ALTER PROCEDURE [dbo].[getlistNhanVien_CaiDatChietKhau]
    @ID_DonVi [uniqueidentifier],
    @Text_NhanVien [nvarchar](max),
    @Text_NhanVien_TV [nvarchar](max),
    @TrangThai [nvarchar](max)
AS
BEGIN
	
    Select a.ID as ID_NhanVien,
    	a.MaNhanVien,
		a.TenNhanVien, 
		a.DienThoaiDiDong, 
		a.GioiTinh,
		a.MaPhongBan,
		a.ID_PhongBan,
		ISNULL(AnhNV.URLAnh, '') AS URLAnh,
		ISNULL(a.MaPhongBan, '') AS MaPhongBan,
		ISNULL(a.TenPhongBan, N'Phòng mặc định') AS TenPhongBan		
    	From
    	(   	
			Select DISTINCT nv.ID, 
				nv.MaNhanVien, 
				nv.TenNhanVien,
				nv.TenNhanVienChuCaiDau, 
				nv.TenNhanVienKhongDau, 
				nv.DienThoaiDiDong, 			
    			Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
    			Case when ck.ID_NhanVien is null then 0 else 1 end as CaiDat,
				nv.GioiTinh,
				ct.ID_PhongBan,
				pb.MaPhongBan,
				pb.TenPhongBan
    		From NS_NhanVien nv
    		join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
			join NS_PhongBan pb on ct.ID_PhongBan= pb.ID
    		left join (select ck.ID_NhanVien
					from ChietKhauMacDinh_NhanVien ck 
					where ID_DonVi= @ID_DonVi 
					and exists (select ID from DonViQuiDoi qd where ID_DonViQuiDoi= qd.ID and Xoa='0')
					group by ck.ID_NhanVien) ck on nv.ID= ck.ID_NhanVien    
    		where (nv.MaNhanVien like @Text_NhanVien or nv.MaNhanVien like @Text_NhanVien_TV 
    		or nv.DienThoaiDiDong like @Text_NhanVien_TV or nv.TenNhanVienKhongDau like @Text_NhanVien or nv.TenNhanVienChuCaiDau like @Text_NhanVien)
    		and DaNghiViec != '1'
    		and ct.ID_DonVi = @ID_DonVi
    	)a
		left join (SELECT ID_NhanVien, URLAnh FROM NS_NhanVien_Anh WHERE SoThuTu = 1 GROUP BY ID_NhanVien, URLAnh) as AnhNV
		ON a.ID = AnhNV.ID_NhanVien
    	where a.CaiDat like @TrangThai
    	and a.TrangThai != 0
    
END");

			Sql(@"ALTER PROCEDURE [dbo].[TongQuanDoanhThuCongNo]
    @IdChiNhanhs [nvarchar](max),
    @DateFrom [datetime],
    @DateTo [datetime]
AS
BEGIN
    SET NOCOUNT ON;
	--	DECLARE @IdChiNhanhs [nvarchar](max),
 --   @DateFrom [datetime],
 --   @DateTo [datetime];
	--SET @IdChiNhanhs = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de';
	--SET @DateFrom = '2021-10-20'
	--SET @DateTo = '2021-10-21'

    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    -- Insert statements for procedure here
    	DECLARE @tblHoaDon TABLE(ID UNIQUEIDENTIFIER, LoaiHoaDon INT, TongThanhToan FLOAT, PhaiThanhToan FLOAT, TongTienThue FLOAT, IDHoaDon UNIQUEIDENTIFIER);
    	INSERT INTO @tblHoaDon
    	SELECT ID, LoaiHoaDon, TongThanhToan, PhaiThanhToan, TongTienThue + TongTienThueBaoHiem, hd.ID_HoaDon FROM BH_HoaDon hd
    	INNER JOIN @tblDonVi dv ON hd.ID_DonVi = dv.ID_DonVi
    	WHERE hd.LoaiHoaDon IN (1, 4, 6, 7, 19, 25)
    	AND hd.ChoThanhToan = 0
    	AND hd.NgayLapHoaDon BETWEEN @DateFrom AND @DateTo;

		--SELECT * FROM @tblHoaDon
    
    	DECLARE @tblSoQuy TABLE(ID UNIQUEIDENTIFIER, LoaiHoaDon INT, TienMat FLOAT, TienGui FLOAT, TienThu FLOAT, IDHoaDonLienQuan UNIQUEIDENTIFIER);
    	INSERT INTO @tblSoQuy
    	SELECT qhd.ID, qhd.LoaiHoaDon, qhdct.TienMat, qhdct.TienGui, qhdct.TienThu, qhdct.ID_HoaDonLienQuan FROM Quy_HoaDon qhd
    	INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    	INNER JOIN @tblDonVi dv ON qhd.ID_DonVi = dv.ID_DonVi
    	WHERE qhd.NgayLapHoaDon BETWEEN @DateFrom AND @DateTo
		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and (qhd.PhieuDieuChinhCongNo = 0 or qhd.PhieuDieuChinhCongNo is null)
    	AND qhd.LoaiHoaDon IN (11, 12);
		--SELECT * FROM @tblSoQuy
    
    	DECLARE @DoanhThuSuaChua FLOAT,
    	@DoanhThuBanHang FLOAT,
    	@PhaiTraKhachHang FLOAT,
    	@PhaiTraNhaCungCap FLOAT,
    	@PhaiThuNhaCungCap FLOAT,
    	@Thu_TienMat FLOAT,
    	@Thu_NganHang FLOAT,
    	@Chi_TienMat FLOAT,
    	@Chi_NganHang FLOAT,
    	@HoaDonDaThu FLOAT,
    	@HoaDonDaChi FLOAT,
		@ThueSuaChua FLOAT,
		@ThueBanHang FLOAT;
    	SELECT @DoanhThuSuaChua =SUM(CASE WHEN hd.LoaiHoaDon = 25 THEN hd.TongThanhToan ELSE 0 END), 
    		@PhaiTraKhachHang = SUM(CASE WHEN hd.LoaiHoaDon = 6 THEN hd.TongThanhToan ELSE 0 END),
    		@DoanhThuBanHang = SUM(CASE WHEN hd.LoaiHoaDon IN (1, 19)
    		THEN
    			hd.PhaiThanhToan
    			ELSE 0
    		END),
    	@PhaiTraNhaCungCap = SUM(CASE WHEN hd.LoaiHoaDon = 4 THEN hd.PhaiThanhToan ELSE 0 END),
    	@PhaiThuNhaCungCap = SUM(CASE WHEN hd.LoaiHoaDon = 7 THEN hd.PhaiThanhToan ELSE 0 END),
		@ThueSuaChua = SUM(CASE WHEN hd.LoaiHoaDon = 25 THEN hd.TongTienThue ELSE 0 END),
		@ThueBanHang = SUM(CASE WHEN hd.LoaiHoaDon IN (1, 19) THEN hd.TongTienThue ELSE 0 END)
    	FROM @tblHoaDon hd;
    
    	SELECT @Thu_TienMat = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.TienMat ELSE 0 END), 
    	@Thu_NganHang = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.TienGui ELSE 0 END),
    	@Chi_TienMat = SUM(CASE WHEN sq.LoaiHoaDon = 12 THEN sq.TienMat ELSE 0 END),
    	@Chi_NganHang = SUM(CASE WHEN sq.LoaiHoaDon = 12 THEN sq.TienGui ELSE 0 END)
    	FROM @tblSoQuy sq;
    
    	SELECT @HoaDonDaThu = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.ThanhToan ELSE 0 END),  
    	@HoaDonDaChi = SUM(CASE WHEN sq.LoaiHoaDon = 12 AND hd.LoaiHoaDon != 7 THEN sq.ThanhToan ELSE 0 END) FROM @tblHoaDon hd
    	INNER JOIN (SELECT IDHoaDonLienQuan, SUM(TienThu) AS ThanhToan, LoaiHoaDon FROM @tblSoQuy GROUP BY IDHoaDonLienQuan, LoaiHoaDon) sq
    	ON hd.ID = sq.IDHoaDonLienQuan OR (hd.IDHoaDon = sq.IDHoaDonLienQuan AND hd.LoaiHoaDon != 6)
    
    	SET @DoanhThuSuaChua = ISNULL(@DoanhThuSuaChua, 0);
    	SET @DoanhThuBanHang = ISNULL(@DoanhThuBanHang, 0);
    	SET @PhaiTraKhachHang = ISNULL(@PhaiTraKhachHang, 0);
    	SET @PhaiTraNhaCungCap = ISNULL(@PhaiTraNhaCungCap, 0);
    	SET @PhaiThuNhaCungCap = ISNULL(@PhaiThuNhaCungCap, 0);
    	SET @Thu_TienMat = ISNULL(@Thu_TienMat, 0);
    	SET @Thu_NganHang = ISNULL(@Thu_NganHang, 0);
    	SET @Chi_TienMat = ISNULL(@Chi_TienMat, 0);
    	SET @Chi_NganHang = ISNULL(@Chi_NganHang, 0);
    	SET @HoaDonDaThu = ISNULL(@HoaDonDaThu, 0);
    	SET @HoaDonDaChi = ISNULL(@HoaDonDaChi, 0);
		SET @ThueBanHang = ISNULL(@ThueBanHang, 0);
		SET @ThueSuaChua = ISNULL(@ThueSuaChua, 0)
    
    	SELECT @DoanhThuSuaChua - @ThueSuaChua AS DoanhThuSuaChua, @DoanhThuBanHang - @ThueBanHang AS DoanhThuBanHang,
    	@DoanhThuBanHang + @DoanhThuSuaChua - @ThueBanHang - @ThueSuaChua AS TongDoanhThu, @DoanhThuBanHang + @DoanhThuSuaChua + @PhaiThuNhaCungCap - @HoaDonDaThu AS CongNoPhaiThu,
    	@PhaiTraNhaCungCap + @PhaiTraKhachHang - @HoaDonDaChi AS CongNoPhaiTra, 
    	-(@DoanhThuBanHang + @DoanhThuSuaChua + @PhaiThuNhaCungCap - @HoaDonDaThu) + (@PhaiTraNhaCungCap + @PhaiTraKhachHang - @HoaDonDaChi) AS TongCongNo,
    	@Thu_TienMat AS ThuTienMat, @Thu_NganHang AS ThuNganHang, @Thu_TienMat + @Thu_NganHang AS TongTienThu,
    	@Chi_TienMat AS ChiTienMat, @Chi_NganHang AS ChiNganHang, @Chi_TienMat + @Chi_NganHang AS TongTienChi;
END");

            CreateStoredProcedure(name: "[dbo].[GetInforTheGiaTri_byID]", parametersAction: p => new
            {
                ID = p.Guid()
            }, body: @"SET NOCOUNT ON;

				select  tblThe.ID,
						tblThe.ID_DonVi,
						tblThe.ID_DoiTuong,
						tblThe.ID_NhanVien,
						tblThe.MaHoaDon,
						tblThe.NgayLapHoaDon,
						tblThe.LoaiHoaDon,
						tblThe.TongChiPhi as MucNap,
						tblThe.TongChietKhau as KhuyenMaiVND,
						tblThe.KhuyenMaiPT,
						tblThe.ChietKhauPT,
						tblThe.TongTienHang as ThanhTien,
    					tblThe.TongTienHang as TongTienNap,
    					tblThe.TongTienThue as SoDuSauNap,
    					tblThe.TongGiamGia as ChietKhauVND,
						tblThe.DienGiai,
						tblThe.NguoiTao,
						tblThe.DienGiai as GhiChu,
						tblThe.PhaiThanhToan,
						tblThe.ChoThanhToan,
						ISNULL(soquy.TienMat,0) as TienMat,
    					ISNULL(soquy.TienPOS,0) as TienATM,
    					ISNULL(soquy.TienCK,0) as TienGui,
    					ISNULL(soquy.TienThu,0) as KhachDaTra,
    					dv.TenDonVi,
    					dv.SoDienThoai as DienThoaiChiNhanh,
    					dv.DiaChi as DiaChiChiNhanh,
						dt.MaDoiTuong as MaKhachHang,
						dt.TenDoiTuong as TenKhachHang,
						dt.DienThoai as SoDienThoai,
						dt.DiaChi as DiaChiKhachHang
				from
				(
					select hd.ID, 
						hd.MaHoaDon, 
						hd.LoaiHoaDon,
						hd.NgayLapHoaDon,
						hd.ID_DonVi,
						hd.ID_DoiTuong,
						hd.ID_NhanVien,
						hd.TongGiamGia,
						hd.TongChietKhau,
						hd.TongTienThue,
						hd.TongChiPhi,
						hd.TongTienHang,
						hd.PhaiThanhToan,
						hd.TongThanhToan,
						hd.ChoThanhToan,
						hd.DienGiai,
						hd.NguoiTao,
						iif(hd.TongChiPhi=0,0, hd.TongGiamGia/hd.TongChiPhi * 100) as ChietKhauPT,
						iif(hd.TongChiPhi=0,0, hd.TongChietKhau/hd.TongChiPhi * 100) as KhuyenMaiPT,    				
    					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai
    				from BH_HoaDon hd    				
					where hd.ID= @ID
				) tblThe
				left join DM_DoiTuong dt on tblThe.ID_DoiTuong = dt.ID
				left join DM_DonVi dv on tblThe.ID_DonVi= dv.ID
				left join NS_NhanVien nv on tblThe.ID_NhanVien= nv.ID
				left join ( select quy.ID_HoaDonLienQuan, 
    					sum(quy.TienThu) as TienThu,
    					sum(quy.TienMat) as TienMat,
    					sum(quy.TienPOS) as TienPOS,
    					sum(quy.TienCK) as TienCK
    				from
    				(
    					select qct.ID_HoaDonLienQuan,
    						iif(qct.HinhThucThanhToan = 1, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0) as TienMat,
    						iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) as TienThu,
    						case when tk.TaiKhoanPOS = '1' then iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) else 0 end as TienPOS,
    						case when tk.TaiKhoanPOS = '0' then iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) else 0 end as TienCK
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang= tk.ID
    					where qct.ID_HoaDonLienQuan= @ID and (qhd.TrangThai= 1 or qhd.TrangThai is null)
    				) quy 
    				group by quy.ID_HoaDonLienQuan) soquy on tblThe.ID= soquy.ID_HoaDonLienQuan");

            Sql(@"ALTER PROCEDURE [dbo].[LoadGiaBanChiTiet]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_BangGia [nvarchar](max),
    @maHoaDon [nvarchar](max),
    @maHoaDonVie [nvarchar](max)
AS
BEGIN
    DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@maHoaDon+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@maHoaDonVie+',') where Name!='';
    	Select @count =  (Select count(*) from @tablename);
    	Select @countChar =   (Select count(*) from @tablenameChar);
    
    if(@ID_BangGia != '')
    	BEGIN
    		Select gbct.ID, dvqd.ID_HangHoa,hh.TenHangHoa,
    			hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
				dvqd.ThuocTinhGiaTri as HangHoaThuocTinh,
    			hh.QuanLyTheoLoHang, hh.NgayTao,dvqd.Xoa, hh.ID_NhomHang, dvqd.TenDonViTinh as DonViTinh, dvqd.GiaNhap as GiaNhapCuoi, gbct.GiaBan as GiaMoi, dvqd.MaHangHoa,
    		ISNULL(CAST(gv.GiaVon as FLOAT), 0) as GiaVon, dvqd.GiaBan, dvqd.GiaBan as GiaChung, dvqd.ID as IDQuyDoi, gbct.ID_GiaBan, nhh.TenNhomHangHoa
    		from DonViQuiDoi dvqd
    		left join DM_GiaBan_ChiTiet gbct on dvqd.ID = gbct.ID_DonViQuiDoi
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    		where dvqd.Xoa = 0 and hh.TheoDoi =1 and gbct.ID_Giaban = @ID_BangGia
			and ((select count(*) from @tablename b where 
    					hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    						or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
							or hh.TenHangHoa like '%'+b.Name+'%' 
    						or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    					and ((select count(*) from @tablenameChar c where
    						hh.TenHangHoa like '%'+c.Name+'%' 
    						or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
			 
    		order by gbct.NgayNhap desc
    	END
    	ELSE
    	BEGIN
    		Select dvqd.ID, dvqd.ID_HangHoa,hh.TenHangHoa,
    			hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
				dvqd.ThuocTinhGiaTri as HangHoaThuocTinh,
    			hh.QuanLyTheoLoHang,hh.NgayTao,dvqd.Xoa, hh.ID_NhomHang, dvqd.TenDonViTinh as DonViTinh, dvqd.GiaNhap as GiaNhapCuoi, dvqd.GiaBan as GiaMoi, dvqd.MaHangHoa,
    		ISNULL(CAST(gv.GiaVon as FLOAT), 0) as GiaVon, dvqd.GiaBan, dvqd.ID as IDQuyDoi, NEWID() as ID_GiaBan, nhh.TenNhomHangHoa
    		from DonViQuiDoi dvqd
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    				
    		where ((select count(*) from @tablename b where 
    					hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    						or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
							or hh.TenHangHoa like '%'+b.Name+'%' 
    						or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    					and ((select count(*) from @tablenameChar c where
    						hh.TenHangHoa like '%'+c.Name+'%' 
    						or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    			and dvqd.Xoa = 0 and hh.TheoDoi =1
    		order by hh.NgayTao desc	
    		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhLaiBangLuong]
    @ID_BangLuong [uniqueidentifier],
    @NguoiSua [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @IDNhanVienLogin uniqueidentifier= (select top 1 ID_NhanVien from HT_NguoiDung where LaAdmin='1')
    
    	select bl.TuNgay, bl.DenNgay, bl.ID_DonVi, ct.ID_NhanVien	
    	into #tempbangluong
    	from NS_BangLuong_ChiTiet ct
    	join NS_BangLuong bl on ct.ID_BangLuong= bl.ID
    	where bl.ID like @ID_BangLuong
    
    	declare @IDChiNhanhs uniqueidentifier, @FromDate datetime, @ToDate datetime, @KieuLuongs varchar(10)= '1,2,3,4'
    	select @IDChiNhanhs = ID_DonVi, @FromDate = TuNgay, @ToDate = DenNgay from (select top 1 * from  #tempbangluong ) a
    
    
    	declare @IDNhanViens varchar(max) = 	
    		(select cast(ID_NhanVien as varchar(40)) +',' AS [text()]
    		from #tempbangluong
    		for xml path('')
    		)   
    	
    
    		set @IDNhanViens = LEFT(@IDNhanViens,LEN(@IDNhanViens)-1) -- remove last comma
    
    		declare @ngaycongchuan float = (select dbo.TinhNgayCongChuan(@FromDate,@ToDate,@IDChiNhanhs))
    		
    		declare @tblCong CongThuCong
    		insert into @tblCong
    		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate
    
    		declare @tblThietLapLuong ThietLapLuong
    		insert into @tblThietLapLuong
    		exec GetNS_ThietLapLuong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate
    
    		declare @tblLuong table (LoaiLuong int, ID_NhanVien uniqueidentifier, LuongCoBan float, SoNgayDiLam float, LuongChinh float)				
    		insert into @tblLuong		
    		exec TinhLuongCoBan @ngaycongchuan, @tblCong, @tblThietLapLuong
		
    
    		declare @tblLuongOT table (ID_NhanVien uniqueidentifier, LuongOT float)				
    		insert into @tblLuongOT		
    		exec TinhLuongOT @ngaycongchuan, @tblCong, @tblThietLapLuong

			
    		declare @tblPhuCap table (ID_NhanVien uniqueidentifier, PhuCapCoDinh float, PhuCapTheoNgayCong float)
    		insert into @tblPhuCap
    		exec TinhPhuCapLuong @tblCong, @tblThietLapLuong
			
    
    		declare @tblPhuCapTheoPtram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  PhuCapCoDinh float, HeSo float, NgayApDung datetime, NgayKetThuc datetime, SoNgayDiLam float)
    		insert into @tblPhuCapTheoPtram
    		exec GetPhuCapCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%', @tblCong, @tblThietLapLuong
    
    		declare @tblGiamTru table (ID_NhanVien uniqueidentifier, GiamTruCoDinhVND float, GiamTruTheoLan float, SoLanDiMuon float)
    		insert into @tblGiamTru
    		exec TinhGiamTruLuong @tblCong, @tblThietLapLuong	

				
    
    	----	 get phucap codinh theo %luongchinh
    	declare @tblLuongPC table (ID_NhanVien uniqueidentifier,LoaiLuong int, LuongCoBan float, SoNgayDiLam float, LuongChinh float,PhuCapCoDinh_TheoPtramLuong float)						
    	insert into @tblLuongPC	
    	select 
    		pcluong.ID_NhanVien, pcluong.LoaiLuong, pcluong.LuongCoBan, pcluong.SoNgayDiLam, pcluong.LuongChinh, 
    		sum(PhuCapCoDinh_TheoPtramLuong) as PhuCapCoDinh_TheoPtramLuong
    	from
    		(select luong.ID_NhanVien, LoaiLuong, LuongCoBan, luong.SoNgayDiLam, LuongChinh,
    			case when PhuCapCoDinh is null then 0 else LuongChinh * PhuCapCoDinh * HeSo/100 end as PhuCapCoDinh_TheoPtramLuong
    		from @tblLuong luong
    		left join @tblPhuCapTheoPtram pc on luong.ID_NhanVien= pc.ID_NhanVien
    		) pcluong 
    		group by pcluong.ID_NhanVien, pcluong.LuongChinh, pcluong.LoaiLuong, pcluong.LuongCoBan,pcluong.SoNgayDiLam
    

			---- ===== Tinhluong trong khoangthoigian

			declare @tblFromTo table (ID_BangLuong uniqueidentifier null, DateFrom datetime, DateTo datetime, isGiaoNhau int)

			select bl.ID, bl.MaBangLuong, bl.ID_DonVi, 
			 bl.TuNgay, bl.DenNgay
			into #tblTemp
			from NS_BangLuong bl
			where bl.TrangThai in (3,4)
			and ID_DonVi = @IDChiNhanhs
			and (
				(@FromDate <= TuNgay and @ToDate <= DenNgay and  @ToDate >= TuNgay)
				or (@FromDate >= TuNgay and @FromDate <= DenNgay and  @ToDate >= TuNgay)
				or (@FromDate >= TuNgay and @ToDate <= DenNgay and  @ToDate >= TuNgay)
				or (@FromDate <= TuNgay and  @ToDate >= DenNgay)
				)
	

			----====  get cac khoang thoi gian ----
				declare @cur_IDBangLuong uniqueidentifier, @cur_FDate datetime, @cur_TDate datetime
				declare _cur cursor
				for
					select ID, TuNgay, DenNgay
					from #tblTemp
				open _cur
				fetch next from _cur
				into @cur_IDBangLuong, @cur_FDate, @cur_TDate
				while @@FETCH_STATUS = 0
				begin
					
					if @FromDate < @cur_FDate
					begin
						if @ToDate < @cur_FDate
							insert into @tblFromTo values (null, @FromDate, @ToDate,0)
						else
							if @ToDate >= @cur_FDate and @ToDate < @cur_TDate
								insert into @tblFromTo values (null, @FromDate, @cur_FDate,0),
								(@cur_IDBangLuong, @cur_FDate, @ToDate,1)
							else
								insert into @tblFromTo values (null, @FromDate, @cur_FDate,0),
								(@cur_IDBangLuong, @cur_FDate, @cur_TDate,1),
								(null, @cur_TDate, @ToDate,0)								
					end
					else
					begin
						if @ToDate < @cur_TDate
							insert into @tblFromTo values (@cur_IDBangLuong, @FromDate, @ToDate,1)
						else
						insert into @tblFromTo values (@cur_IDBangLuong, @FromDate, @cur_TDate,1),
							(null, @cur_TDate, @ToDate,0)
					end
					fetch next from _cur into @cur_IDBangLuong, @cur_FDate, @cur_TDate
				end
				close _cur
				deallocate _cur

				drop table #tblTemp

				if (select count(*) from @tblFromTo) = 0
					insert into @tblFromTo values (null, @FromDate, @ToDate,0)

				------ ====== lay ds nhan vien thuoc (khong thuoc bang luong)
				
				declare @tblNhanVien table (ID_NhanVien uniqueidentifier)
				DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, 
				HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
    			TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, 
				TongHoaHongBanGoiDV float, TongAll float)			

				DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
    			TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)		

				DECLARE @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
    			Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float)     			

				---- temp
				DECLARE @temp_CKHangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, 
				HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
    			TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, 
				TongHoaHongBanGoiDV float, TongAll float)

				DECLARE @temp_CKHoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
    			TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)			

				DECLARE @temp_CKDoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
    			Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float)     			   		

				declare @cur2_IDBangLuong uniqueidentifier, @cur2_FDate datetime, @cur2_TDate datetime, @cur2_isGiaoNhau int
				declare _cur2 cursor for
					select distinct * from @tblFromTo where DateFrom != DateTo	order by DateFrom
				open _cur2
				fetch next from _cur2
				into @cur2_IDBangLuong, @cur2_FDate, @cur2_TDate, @cur2_isGiaoNhau			
				while @@FETCH_STATUS =0
				begin			
					
					---- get nv exist bangluong				
					insert into @tblNhanVien
					select ct.ID_NhanVien
					from NS_BangLuong_ChiTiet ct
					join NS_BangLuong bl on ct.ID_BangLuong= bl.ID
					join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
					where ct.ID_BangLuong = @cur2_IDBangLuong 					

					---- get ck hanghoa from-to
					insert into @temp_CKHangHoa
					exec ReportDiscountProduct_General @IDChiNhanhs, @IDNhanVienLogin, '', '%%','0,1,6,19,22,25', @cur2_FDate,@cur2_TDate, 16,1,0,100

					---- get ck hoadon from-to
					insert into @temp_CKHoaDon
					exec ReportDiscountInvoice @IDChiNhanhs,@IDNhanVienLogin,'','%%','0,1,6,19,22,25', @cur2_FDate, @cur2_TDate, 8,1,0,0,100

					---- get ck doanhthu from-to
					insert into @temp_CKDoanhSo
					exec GetAll_DiscountSale @IDChiNhanhs,@IDNhanVienLogin, '', @cur2_FDate, @cur2_TDate, '%%', '', 0,1000		
				
					insert into @tab_HangHoa
					select * 
					from @temp_CKHangHoa ck
					where not exists (
						select nv.ID_NhanVien
						from @tblNhanVien nv where ck.ID_NhanVien = nv.ID_NhanVien
					)

					insert into @tab_HoaDon
					select * 
					from @temp_CKHoaDon ck
					where not exists (
						select nv.ID_NhanVien
						from @tblNhanVien nv where ck.ID_NhanVien = nv.ID_NhanVien
					)

					insert into @tab_DoanhSo
					select * 
					from @temp_CKDoanhSo ck
					where not exists (
						select nv.ID_NhanVien
						from @tblNhanVien nv where ck.ID_NhanVien = nv.ID_NhanVien
					)						

					---- neu da tao bangluong, update phucap codinh = 0
					if @cur2_IDBangLuong is not null
					begin
						update pc set pc.PhuCapCoDinh =0
						from @tblPhuCap pc
						where exists (
							select nv.ID_NhanVien
							from @tblNhanVien nv where pc.ID_NhanVien = nv.ID_NhanVien
						)

						update pc set pc.GiamTruCoDinhVND =0
						from @tblGiamTru pc
						where exists (
							select nv.ID_NhanVien
							from @tblNhanVien nv where pc.ID_NhanVien = nv.ID_NhanVien
						)
					end

					delete from @temp_CKHangHoa
					delete from @temp_CKHoaDon
					delete from @temp_CKDoanhSo
					delete from @tblNhanVien

					fetch next from _cur2
					into @cur2_IDBangLuong, @cur2_FDate, @cur2_TDate, @cur2_isGiaoNhau
				end
				close _cur2
				deallocate _cur2
    
    
    	----- hoahong		
    	--DECLARE @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
    	--Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float) 
    	--INSERT INTO @tab_DoanhSo exec GetAll_DiscountSale @IDChiNhanhs,@IDNhanVienLogin, @FromDate, @ToDate, '%%', '', 0,1000	
    
    	--DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
    	--TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)
    	--INSERT INTO @tab_HoaDon exec ReportDiscountInvoice @IDChiNhanhs,@IDNhanVienLogin,'%%','0,1,6,19,22,25', @FromDate, @ToDate, 8,1,0,0,100
    
    	--DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
    	--TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, TongHoaHongBanGoiDV float, TongAll float)
    	--INSERT INTO @tab_HangHoa exec ReportDiscountProduct_General @IDChiNhanhs,@IDNhanVienLogin,'%%', '0,1,6,19,22,25',@FromDate, @ToDate, 16,1,0,100

    
    	declare @tblHoaHong table (ID_NhanVien uniqueidentifier, TongDoanhThu float, HoaHong float, HoaHongHangHoa float, HoaHongHoaDon float, HoaHongDoanhThu float)
    	insert into  @tblHoaHong
    	SELECT a.ID_NhanVien, sum(TongDoanhThu) as TongDoanhThu,				
    		SUM(TongDoanhSo + TongHoaDon + TongHangHoa) as HoaHong,
    		SUM(TongHangHoa) as HoaHongHangHoa,
    		SUM(TongHoaDon) as HoaHongHoaDon,
    		SUM(TongDoanhSo) as HoaHongDoanhThu
    	FROM 
    	(
    		select ID_NhanVien,	Tong as TongHangHoa,0 as TongHoaDon, 0 as TongDoanhSo, 0 as TongDoanhThu						
    		from @tab_HangHoa
    		UNION ALL
    		Select ID_NhanVien,	0 as TongHangHoa,TongAll as TongHoaDon,	0 as TongDoanhSo, 0 as TongDoanhThu							
    		from @tab_HoaDon
    		UNION ALL
    		Select ID_NhanVien,	0 as TongHangHoa,0 as TongHoaDon,TongAll as TongDoanhSo, TongDoanhThu
    		from @tab_DoanhSo
    	) as a
    	GROUP BY a.ID_NhanVien
    
    	-- giamtru codinh %tongluongnhan
    	declare @tblGiamTruTheoPTram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  GiamTruCoDinh float, HeSo float,
    		NgayApDung datetime, NgayKetThuc datetime, SoLanDiMuon float)
    	insert into @tblGiamTruTheoPTram
    	exec GetGiamTruCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%',@tblCong, @tblThietLapLuong	
    
    	select nv.MaNhanVien, nv.TenNhanVien, 
    			luong.*,
    			cast(PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong as float) as PhuCap,
    			cast(PhatDiMuon + GiamTruCoDinhVND as float) as TongTienPhat,
    			cast(LuongChinh as float)  as TongLuongNhan, -- save to DB
    			cast(LuongChinh + LuongOT +  PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong + KhenThuong + ChietKhau - PhatDiMuon - GiamTruCoDinhVND as float) as LuongThucNhan
    	into #tblluong
    		from
    			(
    			select 
    				tbl.ID_NhanVien, 
    				max(tbl.LoaiLuong) as LoaiLuong,
    				max(tbl.LuongCoBan) as LuongCoBan,
    				sum(tbl.LuongChinh) as LuongChinh,
    				sum(LuongOT) as LuongOT,	
    				sum(PhuCapCoDinh_TheoPtramLuong) as PhuCapCoDinh_TheoPtramLuong,				
    				sum(PhuCapCoDinh) as PhuCapCoBan,
    				sum(PhuCapTheoNgay) as PhuCapKhac,
    				sum(GiamTruCoDinhVND) as GiamTruCoDinhVND,
    				sum(GiamTruTheoLan) as PhatDiMuon,
    				sum(HoaHong) as ChietKhau,
    				sum(HoaHongHangHoa) as HoaHongHangHoa,
    				sum(HoaHongHoaDon) as HoaHongHoaDon,
    				sum(HoaHongDoanhThu) as HoaHongDoanhThu,
    				sum(TongDoanhThu) as TongDoanhThu,
    				sum(SoNgayDiLam) as NgayCongThuc,
    				sum(SoGioOT) as SoGioOT,
    				sum(SoLanDiMuon) as SoLanDiMuon,
    				sum(KhenThuong) as KhenThuong,
    				@ngaycongchuan as  NgayCongChuan
    			from 
    				(select 
    					ID_NhanVien, LoaiLuong, LuongCoBan, LuongChinh,
    					cast(0 as float) as LuongOT, 
    					SoNgayDiLam, cast(0 as float) as SoGioOT, 
    					PhuCapCoDinh_TheoPtramLuong,
    					cast(0 as float) as PhuCapCoDinh, cast(0 as float) as PhuCapTheoNgay,
    					cast(0 as float) as GiamTruCoDinhVND, cast(0 as float) as GiamTruTheoLan, cast(0 as float) as SoLanDiMuon,
    					cast(0 as float) as HoaHong,cast(0 as float) as HoaHongHangHoa, cast(0 as float) as HoaHongHoaDon, cast(0 as float) as HoaHongDoanhThu, 0 as TongDoanhThu,
    					cast(0 as float) as KhenThuong
    				from @tblLuongPC
    
    				union all
    
    				select 
    					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan, 0 as LuongChinh,
    					LuongOT,
    					0 as SoNgayDiLam,0 as SoGioOT,
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					0 as PhuCapCoDinh, 0 as PhuCapTheoNgay,
    					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
    					0 as HoaHong, 0 as HoaHongHangHoa,0 as HoaHongHoaDon, 0 as HoaHongDoanhThu, 0 as TongDoanhThu,
    					0 as KhenThuong
    				from @tblLuongOT
    
    				union all
    				select 
    					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan, 0 as LuongChinh, 0 as LuongOT, 
    					0 as SoNgayDiLam, 0 as SoGioOT,
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					PhuCapCoDinh, PhuCapTheoNgayCong,
    					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
    					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
    					0 as KhenThuong
    				from @tblPhuCap
    
    				union all
    				select 
    					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
    					0 as SoNgayDiLam, 0 as SoGioOT,
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
    					GiamTruCoDinhVND, GiamTruTheoLan, SoLanDiMuon,
    					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
    					0 as KhenThuong
    				from @tblGiamTru
    
    				union all
    				select 
    					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
    					0 as SoNgayDiLam, 0 as SoGioOT, 
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
    					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
    					HoaHong, HoaHongHangHoa, HoaHongHoaDon, HoaHongDoanhThu, TongDoanhThu,
    					0 as KhenThuong
    				from @tblHoaHong
    				) tbl group by tbl.ID_NhanVien
    			) luong
    		join NS_NhanVien nv on luong.ID_NhanVien= nv.ID
		
    
    		-- get max maphieuluong 
    		declare @maxcodePL varchar(20) = (select ISNULL(MAX(CAST (dbo.udf_GetNumeric(MaBangLuongChiTiet) AS float)),0) 
    		from NS_BangLuong_ChiTiet where ID_BangLuong != @ID_BangLuong)

			---- if bangluong da thanhtoan --> get chi tiet phieuluong (ID_QuyCT, ID_bangluongCT, ID_NhanVien)
			select qct.ID, qct.ID_BangLuongChiTiet, qct.ID_NhanVien
			into #qctLuong
			from Quy_HoaDon_ChiTiet qct
			join NS_BangLuong_ChiTiet blct on qct.ID_BangLuongChiTiet = blct.ID
			where blct.ID_BangLuong = @ID_BangLuong
    
    		exec DeleteBangLuongChiTietById @ID_BangLuong
    		
    		insert into NS_BangLuong_ChiTiet
    		select  
    			NEWID(), @ID_BangLuong, ID_NhanVien, NgayCongThuc, NgayCongChuan, LuongCoBan, 
    			PhuCapCoBan,
    			PhuCapKhac + isnull(PhuCapCoDinh_TheoPtramLuong,0) as PhuCapKhac,
    			KhenThuong,
    			0, 0, 0,0, -- thue, mienthue, baohiem,kyluat
    			PhatDiMuon,
    			LuongOT,
    			LuongThucNhan - GiamTruCoDinh_TheoPTram, -- luong chinh + ot + hoahong + phucap - phat
    			TongTienPhat + GiamTruCoDinh_TheoPTram,
    			TongLuongNhan, -- luongchinh
    			N'tính lại lương', -- ghichu
    			1, -- tranngthai
    			@NguoiSua, GETDATE(),
    			@NguoiSua, GETDATE(),
    			0, -- baohiemcty
    			TongDoanhThu, -- doanhso
    			ChietKhau,
    			MaPhieu
    		from
    			(
    				select *, CONCAT('PL0000', RN + @maxcodePL) as MaPhieu
    				from
    				(
    					select luong.* ,
    						ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as GiamTruCoDinh_TheoPTram,
    						ROW_NUMBER() over (order by luong.MaNhanVien) RN 
    					from #tblluong luong
    					left join @tblGiamTruTheoPTram gt on luong.ID_NhanVien= gt.ID_NhanVien
    					where exists(select Name from dbo.splitstring(@KieuLuongs) kl where luong.LoaiLuong= kl.Name)
    					and exists(select Name from dbo.splitstring(@IDNhanViens) nv where luong.ID_NhanVien= nv.Name)
						and luong.LuongThucNhan !=0
    				) pluong
    			) a
    
    		update NS_BangLuong set TrangThai= 1, NguoiSua = @NguoiSua, NgaySua= GETDATE() where id= @ID_BangLuong
    
    		---- update status, id_bangluongchitiet in NS_CongBoSung
    		exec UpdateStatusCongBoSung_WhenCreatBangLuong @ID_BangLuong, @FromDate, @ToDate

			---- update again quyct with new idbangluongct
			update qct1 set qct1.ID_BangLuongChiTiet= a.ID_BangLuongChiTiet
			from Quy_HoaDon_ChiTiet qct1
			join (
			select qct.ID, blct.ID as ID_BangLuongChiTiet
			from NS_BangLuong_ChiTiet blct
			join #qctLuong qct on blct.ID_NhanVien = qct.ID_NhanVien
			where blct.ID_BangLuong = @ID_BangLuong
			) a on qct1.ID = a.ID
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhLuongNhanVien]
    @IDChiNhanhs [uniqueidentifier],
    @IDNhanViens [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @KieuLuongs [nvarchar](20),
    @CurrentPage [int],
    @PageSize [float]
AS
BEGIN
    SET NOCOUNT ON;			

    		declare @IDNhanVienLogin uniqueidentifier= (select top 1 ID_NhanVien from HT_NguoiDung where LaAdmin='1')    
    		declare @ngaycongchuan float = (select dbo.TinhNgayCongChuan(@FromDate,@ToDate,@IDChiNhanhs))
    		
    		declare @tblCong CongThuCong
    		insert into @tblCong
    		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate
    
    		declare @tblThietLapLuong ThietLapLuong
    		insert into @tblThietLapLuong
    		exec GetNS_ThietLapLuong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate
    
    		declare @tblLuong table (LoaiLuong int, ID_NhanVien uniqueidentifier, LuongCoBan float, SoNgayDiLam float, LuongChinh float)				
    		insert into @tblLuong		
    		exec TinhLuongCoBan @ngaycongchuan, @tblCong, @tblThietLapLuong
    
    		declare @tblLuongOT table (ID_NhanVien uniqueidentifier, LuongOT float)				
    		insert into @tblLuongOT		
    		exec TinhLuongOT @ngaycongchuan, @tblCong, @tblThietLapLuong
    		
    		declare @tblPhuCap table (ID_NhanVien uniqueidentifier, PhuCapCoDinh float, PhuCapTheoNgayCong float)
    		insert into @tblPhuCap
    		exec TinhPhuCapLuong @tblCong, @tblThietLapLuong
    
    		declare @tblPhuCapTheoPtram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  PhuCapCoDinh float, HeSo float,
    		NgayApDung datetime, NgayKetThuc datetime, SoNgayDiLam float)
    		insert into @tblPhuCapTheoPtram
    		exec GetPhuCapCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%',@tblCong, @tblThietLapLuong
    
    		declare @tblGiamTru table (ID_NhanVien uniqueidentifier, GiamTruCoDinhVND float, GiamTruTheoLan float, SoLanDiMuon float)
    		insert into @tblGiamTru
    		exec TinhGiamTruLuong @tblCong, @tblThietLapLuong	

    
    		-- get phucap codinh theo %luongchinh
    	declare @tblLuongPC table (ID_NhanVien uniqueidentifier,LoaiLuong int, LuongCoBan float, SoNgayDiLam float, LuongChinh float,PhuCapCoDinh_TheoPtramLuong float)						
    	insert into @tblLuongPC	
    	select 
    		pcluong.ID_NhanVien, pcluong.LoaiLuong, pcluong.LuongCoBan, pcluong.SoNgayDiLam, pcluong.LuongChinh, 
    		sum(PhuCapCoDinh_TheoPtramLuong) as PhuCapCoDinh_TheoPtramLuong
    	from
    		(select luong.ID_NhanVien, LoaiLuong, LuongCoBan, luong.SoNgayDiLam, LuongChinh,
    			case when PhuCapCoDinh is null then 0 else LuongChinh * PhuCapCoDinh * HeSo/100 end as PhuCapCoDinh_TheoPtramLuong
    		from @tblLuong luong
    		left join @tblPhuCapTheoPtram pc on luong.ID_NhanVien= pc.ID_NhanVien
    		) pcluong 
    		group by pcluong.ID_NhanVien, pcluong.LuongChinh, pcluong.LoaiLuong, pcluong.LuongCoBan,pcluong.SoNgayDiLam
    
    		
			---- ===== Tinhluong trong khoangthoigian

			declare @tblFromTo table (ID_BangLuong uniqueidentifier null, DateFrom datetime, DateTo datetime, isGiaoNhau int)

			select bl.ID, bl.MaBangLuong, bl.ID_DonVi, 
			 bl.TuNgay, bl.DenNgay
			into #tblTemp
			from NS_BangLuong bl
			where bl.TrangThai in (3,4)
			and ID_DonVi = @IDChiNhanhs
			and (
				(@FromDate <= TuNgay and @ToDate <= DenNgay and  @ToDate >= TuNgay)
				or (@FromDate >= TuNgay and @FromDate <= DenNgay and  @ToDate >= TuNgay)
				or (@FromDate >= TuNgay and @ToDate <= DenNgay and  @ToDate >= TuNgay)
				or (@FromDate <= TuNgay and  @ToDate >= DenNgay)
				)
	

		----====  get cac khoang thoi gian ----
				declare @cur_IDBangLuong uniqueidentifier, @cur_FDate datetime, @cur_TDate datetime
				declare _cur cursor
				for
					select ID, TuNgay, DenNgay
					from #tblTemp
				open _cur
				fetch next from _cur
				into @cur_IDBangLuong, @cur_FDate, @cur_TDate
				while @@FETCH_STATUS = 0
				begin
					
					if @FromDate < @cur_FDate
					begin
						if @ToDate < @cur_FDate
							insert into @tblFromTo values (null, @FromDate, @ToDate,0)
						else
							if @ToDate >= @cur_FDate and @ToDate < @cur_TDate
								insert into @tblFromTo values (null, @FromDate, @cur_FDate,0),
								(@cur_IDBangLuong, @cur_FDate, @ToDate,1)
							else
								insert into @tblFromTo values (null, @FromDate, @cur_FDate,0),
								(@cur_IDBangLuong, @cur_FDate, @cur_TDate,1),
								(null, @cur_TDate, @ToDate,0)								
					end
					else
					begin
						if @ToDate < @cur_TDate
							insert into @tblFromTo values (@cur_IDBangLuong, @FromDate, @ToDate,1)
						else
						insert into @tblFromTo values (@cur_IDBangLuong, @FromDate, @cur_TDate,1),
							(null, @cur_TDate, @ToDate,0)
					end
					fetch next from _cur into @cur_IDBangLuong, @cur_FDate, @cur_TDate
				end
				close _cur
				deallocate _cur

				drop table #tblTemp

				if (select count(*) from @tblFromTo) = 0
					insert into @tblFromTo values (null, @FromDate, @ToDate,0)

				------ ====== lay ds nhan vien thuoc (khong thuoc bang luong)
				
				declare @tblNhanVien table (ID_NhanVien uniqueidentifier)
				DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, 
				HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
    			TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, 
				TongHoaHongBanGoiDV float, TongAll float)			

				DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
    			TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)		

				DECLARE @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
    			Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float)     			

				---- temp
				DECLARE @temp_CKHangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, 
				HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
    			TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, 
				TongHoaHongBanGoiDV float, TongAll float)

				DECLARE @temp_CKHoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
    			TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)			

				DECLARE @temp_CKDoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
    			Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float)     			   		

				declare @cur2_IDBangLuong uniqueidentifier, @cur2_FDate datetime, @cur2_TDate datetime, @cur2_isGiaoNhau int
				declare _cur2 cursor for
					select distinct * from @tblFromTo where DateFrom != DateTo	order by DateFrom
				open _cur2
				fetch next from _cur2
				into @cur2_IDBangLuong, @cur2_FDate, @cur2_TDate, @cur2_isGiaoNhau			
				while @@FETCH_STATUS =0
				begin			
					
					---- get nv exist bangluong				
					insert into @tblNhanVien
					select ct.ID_NhanVien
					from NS_BangLuong_ChiTiet ct
					join NS_BangLuong bl on ct.ID_BangLuong= bl.ID
					join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
					where ct.ID_BangLuong = @cur2_IDBangLuong 					

					---- get ck hanghoa from-to
					insert into @temp_CKHangHoa
					exec ReportDiscountProduct_General @IDChiNhanhs, @IDNhanVienLogin,'','%%','0,1,6,19,22,25', @cur2_FDate,@cur2_TDate, 16,1,0,100

					---- get ck hoadon from-to
					insert into @temp_CKHoaDon
					exec ReportDiscountInvoice @IDChiNhanhs,@IDNhanVienLogin,'','%%','0,1,6,19,22,25', @cur2_FDate, @cur2_TDate, 8,1,0,0,100

					---- get ck doanhthu from-to
					insert into @temp_CKDoanhSo
					exec GetAll_DiscountSale @IDChiNhanhs,@IDNhanVienLogin, '',@cur2_FDate, @cur2_TDate, '%%', '', 0,1000		
				
					insert into @tab_HangHoa
					select * 
					from @temp_CKHangHoa ck
					where not exists (
						select nv.ID_NhanVien
						from @tblNhanVien nv where ck.ID_NhanVien = nv.ID_NhanVien
					)

					insert into @tab_HoaDon
					select * 
					from @temp_CKHoaDon ck
					where not exists (
						select nv.ID_NhanVien
						from @tblNhanVien nv where ck.ID_NhanVien = nv.ID_NhanVien
					)

					insert into @tab_DoanhSo
					select * 
					from @temp_CKDoanhSo ck
					where not exists (
						select nv.ID_NhanVien
						from @tblNhanVien nv where ck.ID_NhanVien = nv.ID_NhanVien
					)						

					---- neu da tao bangluong, update phucap codinh = 0
					if @cur2_IDBangLuong is not null
					begin
						update pc set pc.PhuCapCoDinh =0
						from @tblPhuCap pc
						where exists (
							select nv.ID_NhanVien
							from @tblNhanVien nv where pc.ID_NhanVien = nv.ID_NhanVien
						)

						update pc set pc.GiamTruCoDinhVND =0
						from @tblGiamTru pc
						where exists (
							select nv.ID_NhanVien
							from @tblNhanVien nv where pc.ID_NhanVien = nv.ID_NhanVien
						)
					end

					delete from @temp_CKHangHoa
					delete from @temp_CKHoaDon
					delete from @temp_CKDoanhSo
					delete from @tblNhanVien

					fetch next from _cur2
					into @cur2_IDBangLuong, @cur2_FDate, @cur2_TDate, @cur2_isGiaoNhau
				end
				close _cur2
				deallocate _cur2
    
    
    	-- hoahong		
    	--DECLARE @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
    	--Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float) 
    	--INSERT INTO @tab_DoanhSo exec GetAll_DiscountSale @IDChiNhanhs,@IDNhanVienLogin, @FromDate, @ToDate, '%%', '', 0,1000		
    
    	--DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
    	--TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)
    	--INSERT INTO @tab_HoaDon exec ReportDiscountInvoice @IDChiNhanhs,@IDNhanVienLogin,'%%','0,1,6,19,22,25', @FromDate, @ToDate, 8,1,0,0,100
    
    	--DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
    	--TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, TongHoaHongBanGoiDV float, TongAll float)
    	--INSERT INTO @tab_HangHoa exec ReportDiscountProduct_General @IDChiNhanhs, @IDNhanVienLogin,'%%','0,1,6,19,22,25', @FromDate, @ToDate, 16,1,0,100
    
    	declare @tblHoaHong table (ID_NhanVien uniqueidentifier, TongDoanhThu float, HoaHong float, HoaHongHangHoa float, HoaHongHoaDon float, HoaHongDoanhThu float)
    	insert into  @tblHoaHong
    	SELECT a.ID_NhanVien, sum(TongDoanhThu) as TongDoanhThu,		
    		SUM(TongDoanhSo + TongHoaDon + TongHangHoa) as HoaHong,
    		SUM(TongHangHoa) as HoaHongHangHoa,
    		SUM(TongHoaDon) as HoaHongHoaDon,
    		SUM(TongDoanhSo) as HoaHongDoanhThu
    	FROM 
    	(
    		select ID_NhanVien,	Tong as TongHangHoa,0 as TongHoaDon, 0 as TongDoanhSo, 0 as TongDoanhThu					
    		from @tab_HangHoa
    		UNION ALL
    		Select ID_NhanVien,	0 as TongHangHoa,TongAll as TongHoaDon,	0 as TongDoanhSo, 0 as TongDoanhThu						
    		from @tab_HoaDon
    		UNION ALL
    		Select ID_NhanVien,	0 as TongHangHoa,0 as TongHoaDon,TongAll as TongDoanhSo, TongDoanhThu
    		from @tab_DoanhSo
    	) as a
    	GROUP BY a.ID_NhanVien
    
    	-- giamtru codinh %tongluongnhan
    	declare @tblGiamTruTheoPTram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  GiamTruCoDinh float, HeSo float,
    		NgayApDung datetime, NgayKetThuc datetime, SoLanDiMuon float)
    	insert into @tblGiamTruTheoPTram
    	exec GetGiamTruCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%',@tblCong, @tblThietLapLuong	
    
		
    	select nv.MaNhanVien, nv.TenNhanVien, 
    			luong.*,		
    			cast(PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong as float) as PhuCap,				
    			cast(PhatDiMuon + GiamTruCoDinhVND as float) as TongTienPhat,
    			cast(LuongChinh as float)  as TongLuongNhan, -- save to DB
    			cast(LuongChinh + LuongOT +  PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong 
				+ KhenThuong + ChietKhau - PhatDiMuon - GiamTruCoDinhVND as float) as LuongThucNhan1
    	into #tblluong
    		from
    			(
    			select 
    				tbl.ID_NhanVien, 
    				max(tbl.LoaiLuong) as LoaiLuong,
    				max(tbl.LuongCoBan) as LuongCoBan,
    				ceiling(sum(tbl.LuongChinh)) as LuongChinh,
    				sum(LuongOT) as LuongOT,	
    				ceiling(sum(PhuCapCoDinh_TheoPtramLuong)) as PhuCapCoDinh_TheoPtramLuong,				
    				sum(PhuCapCoDinh) as PhuCapCoBan,
    				sum(PhuCapTheoNgay) as PhuCapKhac,
    				sum(GiamTruCoDinhVND) as GiamTruCoDinhVND,
    				sum(GiamTruTheoLan) as PhatDiMuon,
    				sum(HoaHong) as ChietKhau,
    				sum(HoaHongHangHoa) as HoaHongHangHoa,
    				sum(HoaHongHoaDon) as HoaHongHoaDon,
    				sum(HoaHongDoanhThu) as HoaHongDoanhThu,
    				sum(TongDoanhThu) as TongDoanhThu,
    				sum(SoNgayDiLam) as NgayCongThuc,
    				sum(SoGioOT) as SoGioOT,
    				sum(SoLanDiMuon) as SoLanDiMuon,
    				sum(KhenThuong) as KhenThuong,
    				@ngaycongchuan as  NgayCongChuan
    			from 
    				(select 
    					ID_NhanVien, LoaiLuong, LuongCoBan, LuongChinh,
    					cast(0 as float) as LuongOT, 
    					SoNgayDiLam, cast(0 as float) as SoGioOT, 
    					PhuCapCoDinh_TheoPtramLuong,
    					cast(0 as float) as PhuCapCoDinh, cast(0 as float) as PhuCapTheoNgay,
    					cast(0 as float) as GiamTruCoDinhVND, cast(0 as float) as GiamTruTheoLan, cast(0 as float) as SoLanDiMuon,
    					cast(0 as float) as HoaHong,cast(0 as float) as HoaHongHangHoa, cast(0 as float) as HoaHongHoaDon, cast(0 as float) as HoaHongDoanhThu, 0 as TongDoanhThu,
    					cast(0 as float) as KhenThuong
    				from @tblLuongPC
    
    				union all
    
    				select 
    					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan, 0 as LuongChinh,
    					LuongOT,
    					0 as SoNgayDiLam,0 as SoGioOT,
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					0 as PhuCapCoDinh, 0 as PhuCapTheoNgay,
    					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
    					0 as HoaHong, 0 as HoaHongHangHoa,0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
    					0 as KhenThuong
    				from @tblLuongOT
    
    				union all
    				select 
    					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan, 0 as LuongChinh, 0 as LuongOT, 
    					0 as SoNgayDiLam, 0 as SoGioOT,
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					PhuCapCoDinh, PhuCapTheoNgayCong,
    					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
    					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
    					0 as KhenThuong
    				from @tblPhuCap
    
    				union all
    				select 
    					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
    					0 as SoNgayDiLam, 0 as SoGioOT,
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
    					GiamTruCoDinhVND, GiamTruTheoLan, SoLanDiMuon,
    					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu, 0 as TongDoanhThu,
    					0 as KhenThuong
    				from @tblGiamTru
    
    				union all
    				select 
    					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
    					0 as SoNgayDiLam, 0 as SoGioOT, 
    					0 as PhuCapCoDinh_TheoPtramLuong,
    					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
    					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
    					HoaHong, HoaHongHangHoa, HoaHongHoaDon, HoaHongDoanhThu, TongDoanhThu,
    					0 as KhenThuong
    				from @tblHoaHong
    				) tbl group by tbl.ID_NhanVien
    			) luong
    		join NS_NhanVien nv on luong.ID_NhanVien= nv.ID
			where (nv.TrangThai= 1 or nv.TrangThai is null)
			and (nv.DaNghiViec= 0 or nv.DaNghiViec is null)
			
    		
    		if @IDNhanViens='' or 	@IDNhanViens='%%'	
    			select luong.* ,
					luong.LuongThucNhan1- ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as LuongThucNhan,
    				ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as GiamTruCoDinh_TheoPTram
    			from #tblluong luong
    			left join @tblGiamTruTheoPTram gt on luong.ID_NhanVien= gt.ID_NhanVien
    			where exists(select Name from dbo.splitstring(@KieuLuongs) kl where luong.LoaiLuong= kl.Name)
				and luong.LuongThucNhan1 != 0
    			order by luong.MaNhanVien
    		else
    			-- search by nhanvien
    			select luong.* ,
					luong.LuongThucNhan1- ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as LuongThucNhan,
    				ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as GiamTruCoDinh_TheoPTram
    			from #tblluong luong
    			left join @tblGiamTruTheoPTram gt on luong.ID_NhanVien= gt.ID_NhanVien
    			where exists(select Name from dbo.splitstring(@KieuLuongs) kl where luong.LoaiLuong= kl.Name )
    			and exists(select Name from dbo.splitstring(@IDNhanViens) nv where luong.ID_NhanVien= nv.Name)
    			order by luong.MaNhanVien
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDon_afterTraHang_DichVu]
   @IDChiNhanhs nvarchar(max) = null,
   @IDNhanViens nvarchar(max) = null,
   @DateFrom datetime = null,
   @DateTo datetime = null,
   @TextSearch nvarchar(max) = null,
   @CurrentPage int =null,
   @PageSize int = null
AS
BEGIN
	set nocount on;

	if isnull(@CurrentPage,'') =''
		set @CurrentPage = 0
	if isnull(@PageSize,'') =''
		set @PageSize = 10

	declare @sql nvarchar(max) ='', @sqlTemp nvarchar(max),  	
				@whereTemp nvarchar(max)= '', @whereTempOut nvarchar(max)= '', 
				@paramDefined nvarchar(max)=''

	
	set @whereTemp = N' where 1 = 1 and hd.ChoThanhToan = 0 and hd.LoaiHoaDon in (1,25) and
						(hdct.ID_ChiTietDinhLuong is null OR hdct.ID_ChiTietDinhLuong = hdct.ID)'
	set @whereTempOut= N' where 1 = 1  '

	set @paramDefined = N' @IDChiNhanhs_In nvarchar(max) ,
		   @IDNhanViens_In nvarchar(max) ,
		   @DateFrom_In datetime ,
		   @DateTo_In datetime ,
		   @TextSearch_In nvarchar(max) ,
		   @CurrentPage_In int =null,
		   @PageSize_In int  '

	if isnull(@IDChiNhanhs,'') !=''
	begin
		set @sqlTemp = N' DECLARE @tblChiNhanh table (ID nvarchar(max)) insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) '
		set @whereTemp = CONCAT(@whereTemp,  N' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID) ')	
	end

	if isnull(@IDNhanViens,'') !=''
	begin
		set @sqlTemp = CONCAT(@sqlTemp, N'
			DECLARE @tblNhanVien table (ID uniqueidentifier)
			insert into @tblNhanVien select name from dbo.splitstring(@IDNhanViens_In)')
		set @whereTemp = CONCAT(@whereTemp,  ' and exists (select nv.ID from @tblNhanVien nv where hd.ID_NhanVien = nv.ID)')
	end

	if isnull(@TextSearch,'') !=''
	begin
		set @sqlTemp = CONCAT(@sqlTemp, N'
								DECLARE @tblSearch TABLE (Name [nvarchar](max))
								DECLARE @count int
								 INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''' 
								 select @count =  (Select count(*) from @tblSearch)')
		set @whereTempOut = CONCAT(@whereTempOut,  ' and
					((select count(Name) from @tblSearch b where     			
					a.MaHoaDon like ''%''+b.Name+''%''								
					or dt.MaDoiTuong like ''%''+b.Name+''%''		
					or dt.TenDoiTuong like ''%''+b.Name+''%''
					or dt.TenDoiTuong_KhongDau like ''%''+b.Name+''%''
					or dt.DienThoai like ''%''+b.Name+''%''		
					or xe.BienSo like ''%''+b.Name+''%''			
					)=@count or @count=0)')
	end

	if isnull(@DateFrom,'') !=''
	begin		
		set @whereTemp= CONCAT(@whereTemp,  N' and hd.NgayLapHoaDon >= @DateFrom_In')
	end

	if isnull(@DateTo,'') !=''
	begin		
		set @DateTo = DATEADD(day, 1, @DateTo)
		set @whereTemp= CONCAT(@whereTemp,  N' and hd.NgayLapHoaDon < @DateTo_In')
	end

	set @sqlTemp = CONCAT(@sqlTemp, N'
			select *
			into #tblView
			from 
			(
				select a.*,
					row_number() over (order by a.NgayLapHoaDon desc) as Rn,
					COUNT(a.ID) OVER () as TotalRow,
					dt.DienThoai,
					xe.BienSo,
					ISNULL(dt.MaDoiTuong,''kl'' ) as MaDoiTuong,
					ISNULL(dt.TenDoiTuong,N''Khách lẻ'' ) as TenDoiTuong,
					ISNULL(dt.TenDoiTuong_KhongDau,''kl'' ) as TenDoiTuong_KhongDau,
					ISNULL(dt.TenDoiTuong_ChuCaiDau,''kl'' ) as TenDoiTuong_ChuCaiDau
				from
				(
				select 
					conlai.ID,
					conlai.ID_DoiTuong,
					max(conlai.ID_Xe) as ID_Xe,
					max(conlai.MaHoaDon) as MaHoaDon,
					max(conlai.NgayLapHoaDon) as NgayLapHoaDon,
					sum(conlai.SoLuongBan) as SoLuongBan,
					sum(conlai.SoLuongTra) as SoLuongTra,
					sum(conlai.SoLuongBan) - ISNULL(sum(conlai.SoLuongTra),0) as SoLuongConLai
				from
				(
					-- sum SLMua
    					Select 
    						hd.ID as ID,									
							hd.ID_DoiTuong,
							hd.ID_Xe,
							hd.MaHoaDon,
							hd.NgayLapHoaDon,							
    						sum(iif(hd.LoaiHoaDon = 1 
								or ((hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
								and (hdct.ID_ParentCombo is null or hdct.ID_ParentCombo= hdct.ID)) , 
							hdct.SoLuong,  isnull(xk.SoLuongXuat,0))) as SoLuongBan,						
    						0 as SoLuongTra
    					from BH_HoaDon hd   				
    					inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
						left join 
						(
							select SUM(ctxk.SoLuong) as SoLuongXuat, ctxk.ID_ChiTietGoiDV, hdxk.ID_HoaDon
							from BH_HoaDon_ChiTiet ctxk 
							join BH_HoaDon hdxk on ctxk.ID_HoaDon = hdxk.ID
							where hdxk.ID_HoaDon is not null
							and hdxk.LoaiHoaDon = 8 and hdxk.ChoThanhToan=0
							group by ctxk.ID_ChiTietGoiDV, hdxk.ID_HoaDon			
						) xk on hd.ID= xk.ID_HoaDon and hdct.ID= xk.ID_ChiTietGoiDV  						
						', @whereTemp, 
						' Group by hd.ID,hd.NgayLapHoaDon , hd.LoaiHoaDon,hd.ID_DoiTuong,hd.MaHoaDon, hd.ID_Xe
					
						union all
						-- sum SLTra
						select 
    						hd.ID_HoaDon as ID,			
							hd.ID_DoiTuong,
							''00000000-0000-0000-0000-000000000000'' as ID_Xe,
							'''' as MaHoaDon,
							null as NgayLapHoaDon,
    						0 as SoLuongBan,						
    						Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongTra
    					from BH_HoaDon hd    				
    					inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon  
						where 1 = 1 and hd.ChoThanhToan = 0  and hd.loaihoadon = 6
						and (hdct.ID_ChiTietDinhLuong is null OR hdct.ID_ChiTietDinhLuong = hdct.ID)						
						and hdct.ChatLieu= 1
    					Group by hd.ID_HoaDon,hd.ID_DoiTuong
				) conlai group by conlai.ID, conlai.ID_DoiTuong						
			) a
			left join DM_DoiTuong dt on a.ID_DoiTuong = dt.ID 
			left join Gara_DanhMucXe xe on a.ID_Xe = xe.ID
			',
			@whereTempOut,  ' and a.SoLuongConLai > 0 ' ,
			') b where b.Rn between  (@CurrentPage_In * @PageSize_In) + 1 and @PageSize_In * (@CurrentPage_In + 1)')

	

		set @sql= CONCAT(@sql, N' select 
							tbl.ID,
							tbl.SoLuongTra,
    						tbl.SoLuongBan,
							tbl.TotalRow,
							tbl.MaDoiTuong,
							tbl.TenDoiTuong,
							tbl.DienThoai,
							tbl.BienSo,
    						hd.MaHoaDon,
    						hd.LoaiHoaDon,
    						hd.NgayLapHoaDon,   						
    						hd.ID_DoiTuong,	
    						hd.ID_HoaDon,
    						hd.ID_BangGia,
    						hd.ID_NhanVien,
    						hd.ID_DonVi,
							hd.ID_Xe,
    						hd.NguoiTao,	
    						hd.DienGiai,	    						
							isnull(thuchi.TienThu,0) as KhachDaTra,
							ISNULL(thuchi.ThuTuThe, 0) as ThuTuThe,
    						ISNULL(nv.TenNhanVien,'''' ) as TenNhanVien,							
    						ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
    						ISNULL(hd.TongTienHang, 0) as TongTienHang,
    						ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
							isnull(hd.TongChietKhau,0) as  TongChietKhau,
    						ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich,							
							ISNULL(hd.TongTienThue, 0) as TongTienThue,						
							isnull(hd.PTThueHoaDon,0) as  PTThueHoaDon,
							ISNULL(hd.TongThueKhachHang, 0) as TongThueKhachHang,	
							isnull(hd.TongTienThueBaoHiem,0) as  TongTienThueBaoHiem,
							isnull(hd.TongTienBHDuyet,0) as  TongTienBHDuyet,
							isnull(hd.SoVuBaoHiem,0) as  SoVuBaoHiem,
							isnull(hd.PTThueBaoHiem,0) as  PTThueBaoHiem,
							isnull(hd.KhauTruTheoVu,0) as  KhauTruTheoVu,
							isnull(hd.GiamTruBoiThuong,0) as  GiamTruBoiThuong,
							isnull(hd.PTGiamTruBoiThuong,0) as  PTGiamTruBoiThuong,
							isnull(hd.BHThanhToanTruocThue,0) as  BHThanhToanTruocThue,
							isnull(hd.PhaiThanhToanBaoHiem,0) as  PhaiThanhToanBaoHiem
						from #tblView tbl
						join BH_HoaDon hd on tbl.ID= hd.ID
						left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
						left join (
							select 
								tblQuy.ID_HoaDonLienQuan,
								sum(tblQuy.ThuTuThe) as ThuTuThe,
								sum(tblQuy.TienThu) as TienThu
							from
							(
								---- Thu tu HDMua
								select qct.ID_HoaDonLienQuan,
									case when qhd.TrangThai= 0 then 0 else SUM(iif(qct.HinhThucThanhToan= 4, qct.TienThu,0)) end as ThuTuThe,							
									Case when qhd.TrangThai = 0 then 0 else 
										Case when qhd.LoaiHoaDon = 11 then SUM(ISNULL(qct.Tienthu, 0)) else - SUM(ISNULL(qct.Tienthu, 0)) end end as TienThu
								from Quy_HoaDon_ChiTiet qct					
								join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID		
								join #tblView tbl on qct.ID_HoaDonLienQuan = tbl.ID				
								group by qct.ID_HoaDonLienQuan, qhd.TrangThai,qhd.LoaiHoaDon
							) tblQuy group by tblQuy.ID_HoaDonLienQuan
						) thuchi on tbl.ID= thuchi.ID_HoaDonLienQuan')

		--print @sqlTemp
		--print @sql

		set @sql = CONCAT(@sqlTemp, @sql)
				
		exec sp_executesql @sql, @paramDefined, 
							@IDChiNhanhs_In = @IDChiNhanhs,
							@IDNhanViens_In = @IDNhanViens,
							@DateFrom_In = @DateFrom,
							@DateTo_In = @DateTo,
							@TextSearch_In = @TextSearch,
							@CurrentPage_In = @CurrentPage,
							@PageSize_In = @PageSize
    	
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TongHopHangNhap]
    @ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
	@timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@LoaiChungTu [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;

	DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblIdDonVi
	SELECT donviinput.Name FROM [dbo].[splitstring](@ID_DonVi) donviinput

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

	DECLARE @tblHoaDon TABLE(TenNhomHang NVARCHAR(MAX), MaHangHoa NVARCHAR(MAX), TenHangHoaFull NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), 
	ThuocTinh_GiaTri NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), TenLoHang NVARCHAR(MAX),
	ID_DonVi UNIQUEIDENTIFIER,
	LoaiHoaDon INT, TyLeChuyenDoi FLOAT, 
	SoLuong FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, GiaVon FLOAT, YeuCau NVARCHAR(MAX), GiamGiaHDPT FLOAT);
	INSERT INTO @tblHoaDon

	SELECT nhh.TenNhomHangHoa AS TenNhomHang, dvqdChuan.MaHangHoa, 
		CONCAT(hh.TenHangHoa,dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
		hh.TenHangHoa, 
		ISNULL(dvqd.ThuocTinhGiaTri, '') AS ThuocTinh_GiaTri, 
		dvqdChuan.TenDonViTinh,
		lh.MaLoHang AS TenLoHang, 
		iif(bhd.LoaiHoaDon=10, bhd.ID_CheckIn,bhd.ID_DonVi) as ID_DonVi,
		bhd.LoaiHoaDon, dvqd.TyLeChuyenDoi, 
		bhdct.SoLuong, 
		bhdct.TienChietKhau,
		bhdct.ThanhTien, 
		case bhd.LoaiHoaDon
			when 6 then case when ctm.GiaVon is null  then bhdct.GiaVon else ctm.GiaVon end
			when 10 then case when bhd.ID_CheckIn= dv.ID then bhdct.GiaVon_NhanChuyenHang else bhdct.GiaVon end
		else bhdct.GiaVon end as GiaVon,		
		--iif(bhd.LoaiHoaDon=6, iif(ctm.GiaVon is null, bhdct.GiaVon, ctm.GiaVon), bhdct.GiaVon) as GiaVon,
		bhd.YeuCau, 
		IIF(bhd.TongTienHang = 0, 0, bhd.TongGiamGia / bhd.TongTienHang) as GiamGiaHDPT
    FROM BH_HoaDon_ChiTiet bhdct
	left join BH_HoaDon_ChiTiet ctm on bhdct.ID_ChiTietGoiDV = ctm.ID --- khachtrahang: lay giavon tu hoadonmua
    INNER JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
	join @tblIdDonVi dv on (bhd.ID_DonVi = dv.ID and bhd.LoaiHoaDon!=10) or (bhd.ID_CheckIn = dv.ID and bhd.YeuCau='4')
    INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
	INNER JOIN DonViQuiDoi dvqdChuan ON dvqdChuan.ID_HangHoa = dvqd.ID_HangHoa AND dvqdChuan.LaDonViChuan = 1
	INNER JOIN (select Name from splitstring(@LoaiChungTu)) lhd ON bhd.LoaiHoaDon = lhd.Name
	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
	INNER JOIN DM_NhomHangHoa nhh  ON nhh.ID = hh.ID_NhomHang   
    INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh   ON nhh.ID = allnhh.ID  
	LEFT JOIN DM_LoHang lh  ON lh.ID = bhdct.ID_LoHang OR (bhdct.ID_LoHang IS NULL and lh.ID is null)   
    WHERE bhd.ChoThanhToan = 0
	and (bhdct.ChatLieu is null or bhdct.ChatLieu!='2')
    AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) >= @timeStart 
	AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) < @timeEnd
	-- chi lay phieu chuyenhang (neu la chinhanh nhan)
	--and exists (select ID from @tblIdDonVi dv where (bhd.ID_DonVi = dv.ID and bhd.LoaiHoaDon!=10) or (bhd.ID_CheckIn = dv.ID and bhd.YeuCau='4'))
	AND hh.LaHangHoa = 1 AND hh.TheoDoi LIKE @TheoDoi AND dvqd.Xoa LIKE @TrangThai 
	AND ((select count(Name) from @tblSearchString b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa like '%'+b.Name+'%'
    			or lh.MaLoHang like '%' +b.Name +'%' 
    		or dvqd.MaHangHoa like '%'+b.Name+'%'
			or dvqdChuan.MaHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    			or dvqd.TenDonViTinh like '%'+b.Name+'%'
    			or dvqd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)	
	

	SELECT pstk.TenNhomHang, pstk.MaHangHoa, pstk.TenHangHoaFull, pstk.TenHangHoa, pstk.ThuocTinh_GiaTri,
	pstk.TenDonViTinh, pstk.TenLoHang,
	dv.TenDonVi, dv.MaDonVi, 
	SUM(pstk.SoLuongNhap) AS SoLuong, IIF(@XemGiaVon = '1',ROUND(SUM(pstk.GiaTriNhap),0) , 0) as ThanhTien
	FROM 
	(
	-- hoadon
    SELECT 
    TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	IIF(LoaiHoaDon = 10 and YeuCau = '4', TienChietKhau* TyLeChuyenDoi, SoLuong * TyLeChuyenDoi) AS SoLuongNhap,
	IIF(LoaiHoaDon = 10 and YeuCau = '4' , TienChietKhau* GiaVon,iif(LoaiHoaDon = 6, SoLuong * GiaVon, ThanhTien*(1-GiamGiaHDPT))) AS GiaTriNhap
    FROM @tblHoaDon WHERE LoaiHoaDon != 9

	UNION ALL
    SELECT 
	-- phieukiemke
    TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	Sum(SoLuong * TyLeChuyenDoi) as SoLuongNhap,	
	SUM(SoLuong * TyLeChuyenDoi * GiaVon) AS GiaTriNhap
    FROM @tblHoaDon
    WHERE LoaiHoaDon = 9 and SoLuong > 0 --- chi lay chitiet kiemke neu soluonglech tang
    GROUP BY TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi
	) pstk
	join DM_DonVi dv on pstk.ID_DonVi= dv.ID
	GROUP BY pstk.TenNhomHang, pstk.MaHangHoa, pstk.TenHangHoaFull, pstk.TenHangHoa, pstk.ThuocTinh_GiaTri, pstk.TenDonViTinh, pstk.TenLoHang,
	pstk.ID_DonVi, dv.TenDonVi, dv.MaDonVi
	order by MaHangHoa

END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhapHang_NhomHang]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
set nocount on;
		

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

			DECLARE @tblSearchString TABLE (Name [nvarchar](max));
			DECLARE @count int;
			INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
			Select @count =  (Select count(*) from @tblSearchString);

		SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
			sum(a.TienThue) as TienThue,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuongNhap, 
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien)), 0) as float) else 0 end as ThanhTien,
    		Case When @XemGiaVon = '1' then  CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) else 0 end as GiamGiaHD,
    		Case When @XemGiaVon = '1' then  CAST(ROUND((Sum(a.ThanhTien - a.GiamGiaHD)), 0) as float) else 0 end as GiaTriNhap
    	FROM
    	(
    		Select
    		nhh.TenNhomHangHoa as TenNhomHangHoa,
			hh.ID_NhomHang,
    		hdct.SoLuong,
			hdct.SoLuong * hdct.TienThue as TienThue,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where hd.NgayLapHoaDon between @timeStart and  @timeEnd
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon in (4,13,14)
			and dvqd.Xoa like @TrangThai
    		and hh.TheoDoi like @TheoDoi
			and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where hh.ID_NhomHang= allnhh.ID)) 
			 AND
				((select count(Name) from @tblSearchString b where 
    					nhh.TenNhomHangHoa like '%'+b.Name+'%' 
    					or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'    							
    					)=@count or @count=0)	
    	) a
    	Group by a.ID_NhomHang
    	OrDER BY GiaTriNhap DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhapHang_TongHop]
    @Text_Search [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	set nocount on	
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

			DECLARE @tblSearchString TABLE (Name [nvarchar](max));
			DECLARE @count int;
			INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';
			Select @count =  (Select count(*) from @tblSearchString);

	 SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    		a.MaHangHoa,
    		Max(a.TenHangHoaFull) as  TenHangHoaFull,
    		Max(a.TenHangHoa) as TenHangHoa,
    		Max(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		Max(a.TenDonViTinh) as TenDonViTinh,
    		a.TenLoHang,
			sum(a.TienThue) as TienThue,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuong, 
    		Case When @XemGiaVon = '1' then Sum(a.ThanhTien)  else 0 end as ThanhTien,
    		Case When @XemGiaVon = '1' then Sum(a.GiamGiaHD)  else 0 end as GiamGiaHD,
    		Case When @XemGiaVon = '1' then Sum(a.ThanhTien - a.GiamGiaHD) else 0 end as GiaTriNhap
    	FROM
    	(
    		Select 
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		nhh.TenNhomHangHoa as TenNhomHangHoa,
    		dvqd.MaHangHoa,
			CONCAT(hh.TenHangHoa, dvqd.ThuocTinhGiaTri) as TenHangHoaFull,    		
    		hh.TenHangHoa,
    		dvqd.TenDonViTinh  as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri  as ThuocTinh_GiaTri,
    		lh.MaLoHang as TenLoHang,
    		Case When hdct.ID_LoHang is not null then hdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    		hdct.SoLuong,
			hdct.SoLuong * hdct.TienThue as TienThue,
    		hdct.DonGia - hdct.TienChietKhau as GiaBan,
    		Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon, 0) else 0 end as GiaVon, 
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where hd.NgayLapHoaDon between @timeStart and  @timeEnd
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon in (4,13,14)
    		and hh.TheoDoi like @TheoDoi
			and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhh.ID= allnhh.ID)	)
    		and dvqd.Xoa like @TrangThai
			AND
		((select count(Name) from @tblSearchString b where 
    			 hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lh.MaLoHang like '%' +b.Name +'%' 
    				or dvqd.MaHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or dvqd.TenDonViTinh like '%'+b.Name+'%'
    				or dvqd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
    	) a
    	Group by a.MaHangHoa, a.TenLoHang, a.ID_LoHang, a.TenHangHoa
    	OrDER BY a.TenHangHoa
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhapHang_TheoNhaCungCap]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomNCC [nvarchar](max),
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

			DECLARE @tblSearchString TABLE (Name [nvarchar](max));
			DECLARE @count int;
			INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
			Select @count =  (Select count(*) from @tblSearchString);

    SELECT b.NhomKhachHang,
    	b.MaNhaCungCap,
    	b.TenNhaCungCap,
    	b.DienThoai,
    	b.SoLuongNhap, b.ThanhTien, b.GiamGiaHD, b.GiaTriNhap, b.TienThue
    	 FROM
    	(
    		SELECT
    			Case When DoiTuong_Nhom.ID_NhomDoiTuong is null then '30000000-0000-0000-0000-000000000003' else DoiTuong_Nhom.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomKhachHang,
    			dt.MaDoiTuong AS MaNhaCungCap,
    			dt.TenDoiTuong AS TenNhaCungCap,
    			Case when dt.DienThoai is null then '' else dt.DienThoai end AS DienThoai,
    			a.SoLuongNhap,
    			a.ThanhTien,    				
    			a.GiamGiaHD,
    			a.GiaTriNhap,
				a.TienThue
    		FROM
    		(
    			SELECT
    				NCC.ID_NhaCungCap,
					sum(TienThue) as TienThue,
    				CAST(ROUND((SUM(NCC.SoLuong)), 3) as float) as SoLuongNhap, 
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.ThanhTien)), 0) as float) else 0 end as ThanhTien,
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.GiamGiaHD)), 0) as float) else 0 end as GiamGiaHD,
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.ThanhTien - NCC.GiamGiaHD)), 0) as float) else 0 end as GiaTriNhap
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_NhaCungCap,
					hdct.TienThue * hdct.SoLuong as TienThue,
    				ISNULL(hdct.SoLuong, 0) AS SoLuong,
    				ISNULL(hdct.ThanhTien, 0) AS ThanhTien,
					Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang) end as GiamGiaHD
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on dvqd.ID = hdct.ID_DonViQuiDoi
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon in (4,13,14)
    				and hh.TheoDoi like @TheoDoi
					and dvqd.Xoa like @TrangThai
					and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where hh.ID_NhomHang= allnhh.ID))
					AND
						((select count(Name) from @tblSearchString b where 
    							dt.MaDoiTuong like '%'+b.Name+'%' 
    							or dt.TenDoiTuong like '%'+b.Name+'%' 
    							or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    							or dt.DienThoai like '%' +b.Name +'%' 
    							)=@count or @count=0)								
    			) AS NCC
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
    	where (@ID_NhomNCC ='' or b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomNCC)))
    	ORDER BY GiaTriNhap DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetAllBangLuong]
    @IDChiNhanhs [nvarchar](max),
    @TxtSearch [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TrangThais [nvarchar](10),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
   DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TxtSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	DECLARE @tblChiNhanh TABLE (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs);
    
    	with data_cte
    	as (
    	select bl.*, 		
			soquy.DaTra,
			soquy.LuongThucNhan,
			soquy.TruTamUngLuong,
			soquy.ThanhToan,
			soquy.LuongThucNhan - soquy.DaTra as ConLai
    	from NS_BangLuong bl
		
    	join (select ct.ID_BangLuong,
    				sum(ct.LuongThucNhan) as LuongThucNhan,
    				sum(isnull(soquy.TienThu,0)) as ThanhToan,
					sum(isnull(soquy.TruTamUngLuong,0)) as TruTamUngLuong,
					sum(isnull(soquy.TienThu,0)) +	sum(isnull(soquy.TruTamUngLuong,0)) as DaTra
    			from NS_BangLuong_ChiTiet ct
    			left join( select qct.ID_BangLuongChiTiet , 
						sum(qct.TienThu) as TienThu,
						sum(ISNULL(qct.TruTamUngLuong,0)) as TruTamUngLuong
    						from Quy_HoaDon_ChiTiet qct  
    						join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID where qhd.TrangThai= 1 
    						group by  qct.ID_BangLuongChiTiet) soquy on ct.ID= soquy.ID_BangLuongChiTiet					
    			group by ct.ID_BangLuong
    			) soquy on bl.ID = soquy.ID_BangLuong
    	where exists (select Name from dbo.splitstring(@TrangThais) tt where bl.TrangThai = tt.Name)
    	and exists (select ID from @tblChiNhanh dv where bl.ID_DonVi= dv.ID)
    	and ((bl.TuNgay >= @FromDate and (bl.TuNgay <= @ToDate or bl.DenNgay <= @ToDate))
    		or ( bl.DenNgay <= @ToDate and ( bl.DenNgay >= @FromDate or bl.TuNgay >= @FromDate))
    			)
    	AND ((select count(Name) from @tblSearchString b where 
    					bl.MaBangLuong like '%'+b.Name+'%' 
    					or bl.TenBangLuong like '%'+b.Name+'%' 
    						or bl.GhiChu like '%'+b.Name+'%' 
    				
    						)=@count or @count=0)	
    	),
    	count_cte
    	as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(LuongThucNhan) as TongPhaiTra,		
					sum(TruTamUngLuong) as TongTamUng,
					sum(ThanhToan) as TongThanhToan,
    				sum(DaTra) as TongDaTra,
    				sum(ConLai) as TongConLai
    			from data_cte
    		)
    		select dt.*, cte.*, ISNULL(nv.TenNhanVien,'') as NguoiDuyet,
				dv.SoDienThoai AS DienThoaiChiNhanh,
				dv.DiaChi AS DiaChiChiNhanh
    		from data_cte dt
			join DM_DonVi dv on dt.ID_DonVi= dv.ID
    		left join NS_NhanVien nv on dt.ID_NhanVienDuyet = nv.ID
    		cross join count_cte cte
    		order by dt.NgayTao desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateStatusCongBoSung_WhenCreatBangLuong]
    @ID_BangLuong [uniqueidentifier],
    @FromDate [datetime],
    @ToDate [datetime]
AS
BEGIN
    SET NOCOUNT ON;	
		set @ToDate = DATEADD(day, 1, @ToDate) ---- todate = denngay in NS_BangLuong

		---- get infor bangluong ----
    	declare @statusSalary int, @ID_DonVi uniqueidentifier
			select @statusSalary = TrangThai, @ID_DonVi = ID_DonVi from NS_BangLuong where ID= @ID_BangLuong
    	declare @statusCong int = @statusSalary

		if @statusSalary = 1 set @statusCong = 2

		--- bangluong - 0 (huy) --> cong = 1 (reset ve tao moi)
		--- bangluong - 1 (tamluu) --> cong = 2
		--- bangluong - 2 (cần tính lại) --> cong = 2
		--- bangluong - 3 (da chot) = cong
		--- bangluong - 4 (da thanh toan) = cong
	
    	if	@statusSalary like '[1-3]' -- @statusSalary= 1/2/3
			begin    						
				update bs set TrangThai = @statusCong, 
    						 ID_BangLuongChiTiet = blct.ID
    			from NS_CongBoSung bs
    			join NS_BangLuong_ChiTiet blct on bs.ID_NhanVien= blct.ID_NhanVien
    			where blct.ID_BangLuong= @ID_BangLuong
    			and bs.NgayCham between @FromDate and  @ToDate
				and bs.TrangThai in (1,2) --- taomoi, tamluu
				and bs.ID_DonVi = @ID_DonVi
			end
    	else
			begin
			if @statusSalary = 4
				begin
					----- get list id_bangluongchitiet
					select ID into #tblCTLuong from NS_BangLuong_ChiTiet where ID_BangLuong= @ID_BangLuong
	
					update bs set TrangThai = iif(soquy.DaThanhToan > 0 ,4,3)    					
    				from NS_CongBoSung bs
    				join #tblCTLuong blct on bs.ID_BangLuongChiTiet = blct.ID 						
					join (
						---- get soquy ttluong
						select qct.ID_BangLuongChiTiet,
							sum(iif(qhd.TrangThai ='0',0, qct.TienThu + isnull(qct.TruTamUngLuong,0))) as DaThanhToan
						from Quy_HoaDon_ChiTiet qct 
						join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
						where exists (select ID from #tblCTLuong ctluong where qct.ID_BangLuongChiTiet = ctluong.ID)
						group by qct.ID_BangLuongChiTiet
					) soquy on blct.ID= soquy.ID_BangLuongChiTiet
    				where bs.NgayCham between @FromDate and  @ToDate
				end
				else
					 ---- huybangluong
					begin													   		
							update bs set TrangThai = 1, 
    								ID_BangLuongChiTiet = null
    						from NS_CongBoSung bs
    						join NS_BangLuong_ChiTiet blct on bs.ID_BangLuongChiTiet = blct.ID
    						where blct.ID_BangLuong= @ID_BangLuong
    						and bs.NgayCham between @FromDate and  @ToDate									
					end    
			end   
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapChuyenHang]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	 SET NOCOUNT ON;
	 DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	    		
    		where nd.ID = @ID_NguoiDung);
	
	-- because like @TheoDoi was slow (không hiểu tại sao chỉ BC này bị chậm vi like @TheoDoi, các BC khác vẫn bình thường)
	declare @sTrangThai varchar(10) ='0,1'
	  set @TheoDoi= REPLACE(@TheoDoi,'%','')
		if @TheoDoi !=''
		set @sTrangThai= @TheoDoi	

    select 
				dv.MaDonVi, dv.TenDonVi,
				isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
				isnull(lo.MaLoHang,'') as TenLoHang,
				qd.MaHangHoa, qd.TenDonViTinh, 
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				hh.TenHangHoa,
				CONCAT(hh.TenHangHoa,qd.ThuocTinhGiaTri) as TenHangHoaFull,
				round(SoLuong,3) as SoLuong,
				iif(@XemGiaVon='1', round(ThanhTien,3),0) as ThanhTien
			from
			(
				select 
					qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_CheckIn,
					sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
					sum(ThanhTien) as ThanhTien
				from(
				select ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn,
					sum(ct.TienChietKhau) as SoLuong,
					sum(ct.TienChietKhau * ct.GiaVon) as ThanhTien
				from BH_HoaDon_ChiTiet ct
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
				where hd.ChoThanhToan=0
				and hd.LoaiHoaDon= 10 and (hd.YeuCau='1' or hd.YeuCau='4') --- YeuCau: 1.DangChuyen, 4.DaNhan, 2.PhieuTam, 3.Huy
				and hd.NgaySua >=@timeStart and hd.NgaySua <@timeEnd
				and exists (select ID from @tblIdDonVi dv where hd.ID_CheckIn= dv.ID)
				group by ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn
				)tblHD
				join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID
				group by qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_CheckIn
			)tblQD
			join DM_DonVi dv on tblQD.ID_CheckIn = dv.ID
			join DM_HangHoa hh on tblQD.ID_HangHoa= hh.ID
			join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
			left join DM_LoHang lo on tblQD.ID_LoHang= lo.ID and (lo.ID= tblQD.ID_LoHang or tblQD.ID_LoHang is null and lo.ID is null)
			where hh.LaHangHoa = 1
			and exists (select Name from dbo.splitstring(@sTrangThai) tt where hh.TheoDoi= tt.Name )
			and qd.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID= allnhh.ID)
			AND ((select count(Name) from @tblSearchString b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa like '%'+b.Name+'%'
    		or lo.MaLoHang like '%' +b.Name +'%' 
			or qd.MaHangHoa like '%'+b.Name+'%'
			or qd.MaHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    		or qd.TenDonViTinh like '%'+b.Name+'%'
    		or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
			or dv.MaDonVi like '%'+b.Name+'%'
			or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)
		order by dv.TenDonVi, hh.TenHangHoa, lo.MaLoHang	
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_XuatChuyenHang]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	 SET NOCOUNT ON;
	 DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	    		
    		where nd.ID = @ID_NguoiDung);

			select 
				dv.MaDonVi, dv.TenDonVi,
				isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
				isnull(lo.MaLoHang,'') as TenLoHang,
				qd.MaHangHoa, qd.TenDonViTinh, 
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				hh.TenHangHoa,
				CONCAT(hh.TenHangHoa,qd.ThuocTinhGiaTri) as TenHangHoaFull,
				round(SoLuong,3) as SoLuong,
				iif(@XemGiaVon='1', round(ThanhTien,3),0) as ThanhTien
			from
			(
				select 
					qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_DonVi,
					sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
					sum(ThanhTien) as ThanhTien
				from(
				select ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi,
					sum(ct.TienChietKhau) as SoLuong,
					sum(ct.TienChietKhau * ct.GiaVon) as ThanhTien
				from BH_HoaDon_ChiTiet ct
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
				where hd.ChoThanhToan=0
				and hd.LoaiHoaDon= 10 and (hd.YeuCau='1' or hd.YeuCau='4') --- YeuCau: 1.DangChuyen, 4.DaNhan, 2.PhieuTam, 3.Huy
				and hd.NgayLapHoaDon >=@timeStart and hd.NgayLapHoaDon <@timeEnd
				and exists (select ID from @tblIdDonVi dv where hd.ID_DonVi= dv.ID)
				group by ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi
				)tblHD
				join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID
				group by qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_DonVi
			)tblQD
			join DM_DonVi dv on tblQD.ID_DonVi = dv.ID
			join DM_HangHoa hh on tblQD.ID_HangHoa= hh.ID
			join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
			left join DM_LoHang lo on tblQD.ID_LoHang= lo.ID and (lo.ID= tblQD.ID_LoHang or (tblQD.ID_LoHang is null and lo.ID is null))
			where hh.LaHangHoa = 1
			and hh.TheoDoi like @TheoDoi
			and qd.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID= allnhh.ID)
			AND ((select count(Name) from @tblSearchString b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa like '%'+b.Name+'%'
    		or lo.MaLoHang like '%' +b.Name +'%' 
			or qd.MaHangHoa like '%'+b.Name+'%'
			or qd.MaHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    		or qd.TenDonViTinh like '%'+b.Name+'%'
    		or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
			or dv.MaDonVi like '%'+b.Name+'%'
			or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)
		order by dv.TenDonVi, hh.TenHangHoa, lo.MaLoHang	
    
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListLichHen_FullCalendar]
    @IDChiNhanhs [nvarchar](max),
    @IDLoaiTuVans [nvarchar](max),
    @IDNhanVienPhuTrachs [nvarchar](max),
    @TrangThaiCVs [nvarchar](20),
    @PhanLoai [nvarchar](20),
    @DoUuTien [nvarchar](4),
    @LoaiDoiTuong [nvarchar](10),
    @FromDate [datetime],
    @ToDate [datetime],
    @IDKhachHang [nvarchar](40),
	@TextSearch nvarchar(max),
	@CurrentPage int,
	@PageSize int
AS
BEGIN
    SET NOCOUNT ON;

	set @FromDate = DATEADD(SECOND, -1, @FromDate)

	declare @today datetime = format( DATEADD(SECOND, -1, getdate()),'yyyy-MM-dd')
	declare @SFromDate nvarchar(max) = CONVERT(varchar,  DATEADD(SECOND, -1, @FromDate),23)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);

	declare @tblChiNhanh table(ID uniqueidentifier)
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@IDChiNhanhs)

	declare @tblTrangThai table(TrangThaiCV int)
	insert into @tblTrangThai
	select Name from dbo.splitstring(@TrangThaiCVs)

	CREATE TABLE #temp(	
		ID uniqueidentifier not null, 
    	Ma_TieuDe nvarchar(max),
    	ID_DonVi uniqueidentifier, 
    	ID_KhachHang uniqueidentifier, 
    	ID_LoaiTuVan uniqueidentifier,
    	ID_NhanVien uniqueidentifier,	
    	ID_NhanVienQuanLy uniqueidentifier,
    	NgayTao datetime,
    	NgayGio datetime,
    	NgayGioKetThuc datetime,	
    	NgayHoanThanh datetime,
		KieuLap int ,
		SoLanLap int,
		GiaTriLap varchar(max),
		TuanLap int,
		TrangThaiKetThuc int,
		GiaTriKetThuc varchar(max),
		SoLanDaHen int,
		TrangThai int,
		GhiChu nvarchar(max),
		NoiDung nvarchar(max),
		NguoiTao nvarchar(max),
		MucDoUuTien int,
		KetQua nvarchar(max),
		NhacNho int,
		KieuNhacNho int,
		ID_Parent uniqueidentifier,
		NgayCu datetime   	
)

CREATE TABLE #tempCur(	
		ID uniqueidentifier not null, 
    	Ma_TieuDe nvarchar(max),
    	ID_DonVi uniqueidentifier, 
    	ID_KhachHang uniqueidentifier, 
    	ID_LoaiTuVan uniqueidentifier,
    	ID_NhanVien uniqueidentifier,	
    	ID_NhanVienQuanLy uniqueidentifier,
    	NgayTao datetime,
    	NgayGio datetime,
    	NgayGioKetThuc datetime,	
    	NgayHoanThanh datetime,
		KieuLap int not null,
		SoLanLap int,
		GiaTriLap varchar(max),
		TuanLap int,
		TrangThaiKetThuc int,
		GiaTriKetThuc varchar(max),
		SoLanDaHen int,
		TrangThai int,
		GhiChu nvarchar(max),
		NoiDung nvarchar(max),
		NguoiTao nvarchar(max),
		MucDoUuTien int,
		KetQua nvarchar(max),
		NhacNho int,
		KieuNhacNho int,
		ID_Parent uniqueidentifier,
		NgayCu datetime   	
)

CREATE CLUSTERED INDEX calendar_KieLap ON #tempCur(KieuLap)

CREATE TABLE #temp2(	
		ID uniqueidentifier PRIMARY KEY,     	
		ID_Parent uniqueidentifier not null,
		NgayCu datetime   	,
		NgayCu_yyyyMMdd datetime
)
 

	--declare @tblNVPhuTrach table(ID uniqueidentifier)
	--insert into @tblNVPhuTrach
	--select Name from dbo.splitstring(@IDNhanVienPhuTrachs)

	--declare @tblLoaiCV table(ID uniqueidentifier)
	--insert into @tblLoaiCV
	--select Name from dbo.splitstring(@IDLoaiTuVans)
	
    
    declare @tblCalendar table(ID uniqueidentifier,Ma_TieuDe nvarchar (max), ID_DonVi uniqueidentifier, ID_KhachHang uniqueidentifier,ID_LoaiTuVan uniqueidentifier null,
    	ID_NhanVien uniqueidentifier, ID_NhanVienQuanLy uniqueidentifier null, 
    	NgayTao datetime,NgayHenGap datetime, NgayGioKetThuc datetime null,NgayHoanThanh datetime null,
    	TrangThai varchar(10), GhiChu nvarchar(max), NoiDung nvarchar(max), NguoiTao nvarchar(max),
    	MucDoUuTien int, KetQua nvarchar(max), NhacNho int null, KieuNhacNho int null,
    	KieuLap int null, SoLanLap int null, GiaTriLap nvarchar(max) null,TuanLap int null, TrangThaiKetThuc int null,GiaTriKetThuc nvarchar(max), 
    	ExistDB bit,ID_Parent uniqueidentifier null, NgayCu datetime null)	
    
	insert into #temp WITH(TABLOCKX)
    --- table LichHen
    select cs.ID, 
    	Ma_TieuDe,
    	cs.ID_DonVi, 
    	ID_KhachHang, 
    	ID_LoaiTuVan,
    	ID_NhanVien,	
    	ID_NhanVienQuanLy,
    	NgayTao,
    	NgayGio,
    	NgayGioKetThuc,	
    	NgayHoanThanh,
    	ISNULL(KieuLap,0) as KieuLap,
    	ISNULL(SoLanLap,0) as SoLanLap,
    	ISNULL(GiaTriLap,'') as GiaTriLap,
    	ISNULL(TuanLap,0) as TuanLap,
    	ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
    	ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc, 	
    	ISNULL(a.SoLanDaHen,0) as SoLanDaHen,
    	TrangThai,
		ISNULL(GhiChu,'') as GhiChu,
    	ISNULL(NoiDung,'') as NoiDung,
    	NguoiTao,
    	2 as MucDoUuTien,
    	KetQua,
    	NhacNho, 
    	ISNULL(KieuNhacNho,0) as KieuNhacNho,
		case when cs.ID_Parent is null then cs.ID else cs.ID_Parent end as ID_Parent,
    	cs.NgayCu
    from ChamSocKhachHangs cs
    left join  
		( select ISNULL(ID_Parent,'00000000-0000-0000-0000-000000000000') as ID_Parent,
    		count(ID) as SoLanDaHen
    		from ChamSocKhachHangs
    		where PhanLoai = 3
    		group by ID_Parent) a 
			on cs.ID= a.ID_Parent
    where KieuLap is not null
    	and (TrangThaiKetThuc = 1 
    	OR (TrangThaiKetThuc = 2 and ISNULL(GiaTriKetThuc,'')  > @SFromDate)
    	OR (TrangThaiKetThuc = 3 and ISNULL(a.SoLanDaHen,0)  < ISNULL(GiaTriKetThuc,1)) 
		OR TrangThaiKetThuc is null
    	)	
    and PhanLoai = 3 
	and exists (select ID from @tblChiNhanh cn where cn.id = cs.ID_DonVi)
    
    -- get row was update (ID_Parent !=null)
	insert into #temp2  WITH(TABLOCKX)
    select ID, ID_Parent, NgayCu, format(NgayCu,'yyyy-MM-dd') as NgayCu_yyyyMMdd 
	from #temp where ID_Parent != ID

	insert into #tempCur WITH(TABLOCKX)
	select t1.* 
	from #temp t1
	left join #temp2 t2 on t1.ID= t2.ID
	where t1.SoLanLap > 0 and t1.TrangThai = 1 ---- trangthai = 1. Dang xu ly <=> chưa hẹn gặp khách
	and t2.ID is null
   
    declare @ID uniqueidentifier, @Ma_TieuDe nvarchar(max), @ID_DonVi uniqueidentifier, @ID_KhachHang uniqueidentifier,@ID_LoaiTuVan uniqueidentifier, 
    		@ID_NhanVien uniqueidentifier,@ID_NhanVienQuanLy uniqueidentifier,
    		@NgayTao datetime,@NgayGio datetime,@NgayGioKetThuc datetime, @NgayHoanThanh datetime,
    		@KieuLap int, @SoLanLap int, @GiaTriLap varchar(max), @TuanLap int, @TrangThaiKetThuc int,@GiaTriKetThuc varchar(max),			
    		@SoLanDaHen int, @TrangThai varchar, @GhiChu nvarchar(max),@NoiDung nvarchar(max),
    		@NguoiTao nvarchar(max), @MucDoUuTien int, @KetQua nvarchar(max), @NhacNho int, @KieuNhacNho int, @ID_Parent uniqueidentifier, @NgayCu datetime
    
    		--- lap ngay
    		declare _cur cursor
    		for
    			select * 
				from #tempCur t1				
				where t1.KieuLap = 1 
    		open _cur
    		fetch next from _cur
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc, @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc, @SoLanDaHen,@TrangThai,@GhiChu,@NoiDung,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin		
    				-- chi add row < @ToDate
    				declare @dateadd datetime = @NgayGio
				
    				declare @lanlap int = 1			
    				while @dateadd < @ToDate 
    					begin	
    						declare @dateadd_yyyyMMdd datetime= format(@dateadd,'yyyy-MM-dd')
    						if @TrangThaiKetThuc= 1 
    							OR (@TrangThaiKetThuc = 2 and  @dateadd < @GiaTriKetThuc )  --- khong bao gio OR KetThuc vao ngay OR sau x lan (todo)
    							OR (@TrangThaiKetThuc= 3 and @lanlap <= @GiaTriKetThuc - @SoLanDaHen)
    							begin
    								set @NgayGioKetThuc = DATEADD(hour,1,@dateadd)
    								declare @newidDay uniqueidentifier = NEWID()
    								declare @count1 int = 0;
    								if @dateadd = @NgayGio set @newidDay = @ID		
    								select @count1 = count(ID) from #temp2 where ID_Parent = @ID_Parent 
    									and NgayCu_yyyyMMdd = @dateadd_yyyyMMdd								
    								if @count1 = 0		
										begin
    										insert into @tblCalendar values (@newidDay,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
    										@NgayTao, @dateadd,@NgayGioKetThuc, @NgayHoanThanh, @TrangThai,@GhiChu,@NoiDung,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,
    										@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc, IIF(@dateadd = @NgayGio,'1','0'),@ID_Parent, @NgayCu)																			
											set @lanlap= @lanlap + 1
										end
    							end
    						set @dateadd = DATEADD(day, @SoLanLap, @dateadd)
    					end
    				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap,@TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,@NoiDung,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,@ID_Parent, @NgayCu
    			end
    		close _cur;
    		deallocate _cur;
    
    		--- lap tuan
    		declare _cur2 cursor
    		for
				select * 
				from #tempCur t1				
				where t1.KieuLap = 2 
    		open _cur2
    		fetch next from _cur2
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,@NoiDung,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin	
    				declare @weekRepeat datetime = @NgayGio			
					declare @hour int= datepart(hour, @NgayGio)
					declare @minute int= datepart(minute, @NgayGio)
    				declare @lanlapWeek int = 1
    				while @weekRepeat < @ToDate -- lặp đến khi thuộc khoảng thời gian tìm kiếm
    					begin	
    								declare @firstOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 0)) -- lay ngay dau tien cua tuan
    								declare @lastOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 7)) -- lay ngay cuoi cung cua tuan
    								declare @dateRepeat datetime = @firstOfWeek	
									

    								while @dateRepeat < @lastOfWeek -- tim kiem trong tuan duoc lap lai
    									begin
    										if dateadd(hour,23, @dateRepeat) >= @NgayGio
    											begin
    												declare @dateOfWeek varchar(1) = cast(DATEPART(WEEKDAY,@dateRepeat) as varchar(1)) -- lấy ngày trong tuần (thứ 2,3,..)	
    												if @dateOfWeek = 1 set @dateOfWeek = 8
    												declare @datefrom datetime = dateadd(minute, @minute, dateadd(hour,@hour, @dateRepeat))
    												set @NgayGioKetThuc = DATEADD(hour,1,@datefrom) -- add 2 hour
													declare @dateRepeat_yyyyMMdd datetime= format(@dateRepeat,'yyyy-MM-dd')
    
    												if CHARINDEX(@dateOfWeek, @GiaTriLap ) > 0 
    												and  (@TrangThaiKetThuc= 1 OR (@TrangThaiKetThuc = 2 and  @dateRepeat <= @GiaTriKetThuc)
    													OR (@TrangThaiKetThuc= 3 and @lanlapWeek <= @GiaTriKetThuc - @SoLanDaHen))
    													begin														
    														declare @newidWeek uniqueidentifier = NEWID()
    														declare @exitDB bit='0'
    														if @dateRepeat_yyyyMMdd = format(@NgayGio,'yyyy-MM-dd') 
    															begin
    																set @newidWeek = @ID
    																set @exitDB ='1'
    															end
    														declare @count2 int=0
    														select @count2 = count(ID) from #temp2 where ID_Parent = @ID_Parent 
    																and NgayCu_yyyyMMdd = @dateRepeat_yyyyMMdd								
    														if @count2 = 0	
    															begin
    																insert into @tblCalendar values (@newidWeek,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
    																		@NgayTao, @datefrom,@NgayGioKetThuc,  @NgayHoanThanh, 
    																		@TrangThai,@GhiChu,@NoiDung,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,
    																		@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc,@exitDB, @ID_Parent, @NgayCu)
    																set @lanlapWeek= @lanlapWeek + 1																
    															end
    													end												
    											end										
    										set @dateRepeat = DATEADD(day, 1, @dateRepeat)											
    									end							
    						set @weekRepeat = DATEADD(WEEK, @SoLanLap, @weekRepeat)	-- lap lai x tuan/lan	
    					end			
    				FETCH NEXT FROM _cur2 into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,@NoiDung,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    			end
    		close _cur2;
    		deallocate _cur2;
    
    		--- lap thang
    		declare _cur cursor
    		for				
				select * 
				from #tempCur t1				
				where t1.KieuLap = 3 
    		open _cur
    		fetch next from _cur
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,@NoiDung,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin		
    				declare @monthRepeat datetime = @NgayGio	
    				declare @lanlapMonth int = 1
    				while @monthRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
    					begin	
    						if  @monthRepeat > @FromDate			
    							begin	
    								declare @datefromMonth datetime= @monthRepeat
									declare @monthRepeat_yyyyMMdd datetime= format(@monthRepeat,'yyyy-MM-dd')

    								set @NgayGioKetThuc = DATEADD(hour,1,@datefromMonth)
    								 -- hàng tháng vào ngày ..xx..
    								if	@TuanLap = 0 
    									and (@TrangThaiKetThuc = 1 
    									OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
    									OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
    									)
    									begin
    											declare @newidMonth1 uniqueidentifier = NEWID()
    											if @monthRepeat = @NgayGio set @newidMonth1 = @ID
    											declare @count3 int=0
    											select @count3 = count(ID) from #temp2 where ID_Parent = @ID_Parent 
    													and NgayCu_yyyyMMdd = @monthRepeat_yyyyMMdd								
    											if @count3 = 0	
    											insert into @tblCalendar values (@newidMonth1,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
    																@NgayTao, @monthRepeat,@NgayGioKetThuc,  @NgayHoanThanh,
    																@TrangThai,@GhiChu,@NoiDung,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, 
    																@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc, IIF(@monthRepeat = @NgayGio,'1','0'), @ID_Parent, @NgayCu)
    									end 
    								else
    									-- hàng tháng vào thứ ..x.. tuần thứ ..y.. của tháng
    									begin
    										declare @dateOfWeek_Month int = DATEPART(WEEKDAY,@monthRepeat) -- thu may trong tuan
    										if @dateOfWeek_Month = 8 set @dateOfWeek_Month = 1
    										declare @weekOfMonth int = DATEDIFF(WEEK, DATEADD(MONTH, DATEDIFF(MONTH, 0, @monthRepeat), 0), @monthRepeat) +1 -- tuan thu may cua thang
    										if @dateOfWeek_Month = @GiaTriLap and @weekOfMonth = @TuanLap 
    										and (@TrangThaiKetThuc = 1 
    											OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
    											OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
    											)
    											begin
    												declare @newidMonth2 uniqueidentifier = NEWID()
    												if @monthRepeat = @NgayGio set @newidMonth2 = @ID
    												declare @count4 int=0
    												select @count4 = count(ID) from #temp2 where ID_Parent = @ID_Parent 
    														and NgayCu_yyyyMMdd = @monthRepeat_yyyyMMdd							
    												if @count4 = 0	
    												insert into @tblCalendar values (@newidMonth2,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
    																@NgayTao, @monthRepeat,@NgayGioKetThuc,  @NgayHoanThanh,
    																@TrangThai,@GhiChu,@NoiDung,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, 
    																@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc,IIF(@monthRepeat = @NgayGio,'1','0'), @ID_Parent, @NgayCu)
    											end
    									end						
    							end
    						set @monthRepeat = DATEADD(MONTH, @SoLanLap, @monthRepeat)	-- lap lai x thang/lan	
    						set @lanlapMonth = @lanlapMonth +1
    					end			
    				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,@NoiDung,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    			end
    		close _cur;
    		deallocate _cur;
    
    
    		--- lap nam
    		declare _cur cursor
    		for
				select * 
				from #tempCur t1				
				where t1.KieuLap = 4 
    		open _cur
    		fetch next from _cur
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,@NoiDung,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin		
    				declare @yearRepeat datetime = @NgayGio
					declare @yearRepeat_yyyyMMdd datetime= format(@yearRepeat,'yyyy-MM-dd')
    				declare @lanlapYear int = 1
    				while @yearRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
    					begin						
    						if  @yearRepeat > @FromDate			
    							begin	
    								declare @dateOfMonth int = datepart(day,@yearRepeat)
    								declare @monthOfYear int = datepart(MONTH,@yearRepeat)
    								set @NgayGioKetThuc= DATEADD(hour,1, @yearRepeat)
    
    								if @dateOfMonth = @GiaTriLap and @monthOfYear= @TuanLap
    									and (@TrangThaiKetThuc = 1 
    										OR (@TrangThaiKetThuc = 2 and @yearRepeat < @GiaTriKetThuc)
    										OR (@TrangThaiKetThuc = 3 and @lanlapYear <= @GiaTriKetThuc - @SoLanDaHen)
    										)
    									begin
    										declare @newidYear uniqueidentifier = NEWID()										
    										if @yearRepeat = @NgayGio set @newidYear = @ID
    										declare @count5 int=0
    										select @count5 = count(ID) from #temp2 where ID_Parent = @ID_Parent 
    												and NgayCu_yyyyMMdd = @yearRepeat_yyyyMMdd							
    										if @count5 = 0	
    										insert into @tblCalendar values (@newidYear,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
    															@NgayTao, @yearRepeat,@NgayGioKetThuc, @NgayHoanThanh, 
    															@TrangThai,@GhiChu,@NoiDung,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, 
    															@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc,IIF(@yearRepeat = @NgayGio,'1','0'), @ID_Parent, @NgayCu)
    									end
    							end
    						set @yearRepeat = DATEADD(YEAR, @SoLanLap, @yearRepeat)	-- lap lai x nam/lan	
    						set @lanlapYear = @lanlapYear +1
    					end			
    				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,@NoiDung,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    			end
    		close _cur;
    		deallocate _cur;

			--select * from #temp
    	
    	-- add LichHen da duoc update (SoLanLap = 0)
    	insert into @tblCalendar
    	select tbl1.ID, 
    		Ma_TieuDe,
    		ID_DonVi, 
    		ID_KhachHang, 
    		ID_LoaiTuVan,
    		ID_NhanVien,	
    		ID_NhanVienQuanLy,
    		NgayTao,
    		NgayGio,
    		NgayGioKetThuc,
    		NgayHoanThanh,
    		TrangThai,
    		GhiChu,
			NoiDung,
    		NguoiTao,
    		2 as MucDoUuTien, 
    		KetQua,
    		NhacNho,
    		ISNULL(KieuNhacNho,0) as KieuNhacNho,
    		ISNULL(KieuLap,0) as KieuLap,
    		ISNULL(SoLanLap,0) as SoLanLap,
    		ISNULL(GiaTriLap,'') as GiaTriLap,
    		ISNULL(TuanLap,0) as TuanLap,
    		ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
    		ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc,
    		'1' as ExistDB, 
    		tbl1.ID_Parent,
    		tbl1.NgayCu
    	from #temp tbl1
		left join #temp2 tbl2 on tbl1.ID = tbl2.ID
		where (KieuLap = 0 OR TrangThai !='1' 
    		Or tbl2.ID is not null
    		)
    		and NgayGio between @FromDate and @ToDate;
    			
    	--drop table #temp 
    	--drop table #temp2 ;

		with data_cte
		as 
		(    
    	select b.*, 
			case when PhanLoai= 3 then 'rgb(11, 128, 67)' -- lichhen: xanhla
				else
					case when MucDoUuTien= 1 then '#ff6b77' else 'rgb(3, 155, 229)' -- uutiencao: hongnhat, nguoclai: congviec xanhtroi
			end end as color,
			TenNhanVien, 
    		ISNULL(LoaiDoiTuong,0) as LoaiDoiTuong, 
    		ISNULL(MaDoiTuong,'') as MaDoiTuong, 
    		ISNULL(TenDoiTuong,'') as TenDoiTuong, 
    		ISNULL(DienThoai,'') as DienThoai, 
			ISNULL(TenNhomDoiTuongs,N'Nhóm mặc định') as TenNhomDoiTuongs,    		
			dt.ID_NguonKhach,
    		ISNULL(TenLoaiTuVanLichHen,'') as TenLoaiTuVanLichHen, ISNULL(nk.TenNguonKhach,'') as TenNguonKhach,
			case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as IDNhomDoiTuongs,
			case when NgayBatDau > @today then 0 else 1 end as DateSort
			
    	from
    		(select *, format(NgayGio, 'yyyy-MM-dd') as NgayBatDau
    		from
    			(-- lichhen
    			select ID,
    				Ma_TieuDe,
    				ID_DonVi, 
    				ID_KhachHang, 
    				ISNULL(ID_LoaiTuVan,'00000000-0000-0000-0000-000000000000') as ID_LoaiTuVan,
    				ID_NhanVien,
    				ISNULL(ID_NhanVienQuanLy,'00000000-0000-0000-0000-000000000000') as ID_NhanVienQuanLy,
    				NgayTao,
    				NgayHenGap as NgayGio,
    				NgayGioKetThuc,
    				NgayHoanThanh,
    				TrangThai,
    				GhiChu,
					NoiDung,
    				NguoiTao,
    				MucDoUuTien,
    				KetQua,
    				NhacNho,			
    				KieuNhacNho,
    				KieuLap,
    				SoLanLap, 
    				GiaTriLap, 
    				TuanLap, 
    				TrangThaiKetThuc ,
    				GiaTriKetThuc,
    				3 as PhanLoai,
    				'0' as CaNgay,
    				ExistDB, 
    				ID_Parent,
    				NgayCu
    			from @tblCalendar
    			where exists (select ID from @tblTrangThai tt where tt.TrangThaiCV = TrangThai) 
    			and NgayHenGap between @FromDate and @ToDate
    
    			union all
    			-- cong viec
    			select 
    					cs.ID,
    					Ma_TieuDe,
    					cs.ID_DonVi, 
    					ID_KhachHang, 
    					ISNULL(ID_LoaiTuVan,'00000000-0000-0000-0000-000000000000') as ID_LoaiTuVan,
    					ID_NhanVien,
    					ISNULL(ID_NhanVienQuanLy,'00000000-0000-0000-0000-000000000000') as ID_NhanVienQuanLy,
    					cs.NgayTao,
    					NgayGio,
    					NgayGioKetThuc,
    					NgayHoanThanh,
    					cs.TrangThai,
    					cs.GhiChu,
						cs.NoiDung,
    					cs.NguoiTao,
    					MucDoUuTien,
    					KetQua,
    					cs.NhacNho,
    					ISNULL(KieuNhacNho,0) as KieuNhacNho,
    					ISNULL(KieuLap,0) as KieuLap,
    					ISNULL(SoLanLap,0) as SoLanLap,
    					ISNULL(GiaTriLap,'') as GiaTriLap,
    					ISNULL(TuanLap,0) as TuanLap,
    					ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
    					ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc,
    					4 as PhanLoai,
    					ISNULL(cs.CaNgay,'0') as CaNgay,
    					'1' as ExistDB,
    					ID_Parent,
    					NgayCu
    				from ChamSocKhachHangs cs
    				where PhanLoai= 4
    				and exists (select ID from @tblTrangThai tt where tt.TrangThaiCV = TrangThai) 
    				and NgayGio between  @FromDate and  @ToDate
    				) a
    --			where 
				--exists (select ID from @tblNVPhuTrach nvpt where nvpt.ID= a.ID_NhanVien)
    --			and 
				--exists (select ID from @tblLoaiCV loaiCV where loaiCV.ID = a.ID_LoaiTuVan)
    		)b
    		left join DM_DoiTuong dt on b.ID_KhachHang= dt.ID
    		left join NS_NhanVien nv on b.ID_NhanVien= nv.ID   	
			left join DM_LoaiTuVanLichHen loai on b.ID_LoaiTuVan= loai.ID
    		left join DM_NguonKhachHang nk on dt.ID_NguonKhach= nk.ID
    		where --(dt.LoaiDoiTuong like @LoaiDoiTuong OR LoaiDoiTuong is null)
    	--	and
			b.PhanLoai like @PhanLoai
    		and b.MucDoUuTien like @DoUuTien
    		and ISNULL(dt.ID,'00000000-0000-0000-0000-000000000000') like @IDKhachHang
			AND
		((select count(Name) from @tblSearchString tblS where 
				b.Ma_TieuDe like '%'+tblS.Name+'%' 
    			or b.GhiChu like N'%'+tblS.Name+'%'
				or dt.TenDoiTuong like '%'+tblS.Name+'%'
    			or dt.TenDoiTuong_KhongDau  like '%'+tblS.Name+'%'
				or dt.MaDoiTuong like '%'+tblS.Name+'%'
    			or dt.DienThoai  like '%'+tblS.Name+'%'
				or nv.MaNhanVien like N'%'+tblS.Name+'%'
				or nv.TenNhanVien like N'%'+tblS.Name+'%'					
    			or nv.TenNhanVienKhongDau like '%'+tblS.Name+'%')=@count or @count=0)    
				
			),
			count_cte
			as
			(
			select count (ID) as TotalRow,
				CEILING(count(ID)/CAST(@PageSize as float )) as TotalPage
			from data_cte
			)
			select dt.*, cte.*
			from data_cte dt
			cross join count_cte cte
			order by dt.DateSort, dt.NgayGio
		
			
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetChiTietHD_MultipleHoaDon]
    @lstID_HoaDon [nvarchar](max)
AS
BEGIN
    SELECT 
    		cthd.ID,
    			cthd.ID_HoaDon,
    			cthd.ID_DonViQuiDoi,
    			cthd.ID_LoHang,
    			dvqd.ID_HangHoa,			
    			cthd.DonGia,
    			cthd.GiaVon,
    			cthd.SoLuong,
    			cthd.ThanhTien,
    			cthd.ThanhToan,
    		cthd.TienChietKhau AS GiamGia,
    			cthd.PTChietKhau,
    			cthd.ThoiGian,
    			cthd.GhiChu,
    			iif(cthd.TenHangHoaThayThe is null or cthd.TenHangHoaThayThe ='', hh.TenHangHoa, cthd.TenHangHoaThayThe) as TenHangHoa,
    			isnull(cthd.PTThue,0) as PTThue,
    			isnull(cthd.TienThue,0) as TienThue,
    			isnull(cthd.ThanhToan,0) as ThanhToan,
    			isnull(cthd.DonGiaBaoHiem,0) as DonGiaBaoHiem,
    		(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
    		CAST(SoThuTu AS float) AS SoThuTu,
    			cthd.ID_KhuyenMai,
    			dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    			hh.LaHangHoa,
    			hh.QuanLyTheoLoHang,			
    		dvqd.TenDonViTinh,
    			dvqd.MaHangHoa,
    			dvqd.TyLeChuyenDoi,
    			dvqd.ThuocTinhGiaTri, 				
    			hh.ID_NhomHang as ID_NhomHangHoa,
    			ISNULL(MaLoHang,'') as MaLoHang  ,
    			lo.NgaySanXuat, 
    			lo.NgayHetHan,
    			hd.YeuCau,    
    			ISNULL(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
    			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
    			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
    			iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
    			concat(TenHangHoa ,    	
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lo.MaLoHang is null then '' else '. Lô: ' + lo.MaLoHang end) as TenHangHoaFull   				    									
    	FROM BH_HoaDon hd
    	JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    	JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    	JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
    		LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
    		left JOIN DM_NhomHangHoa nhh ON hh.ID_NhomHang= nhh.ID    		
    	WHERE cthd.ID_HoaDon in (Select * from splitstring(@lstID_HoaDon))  
    		and (cthd.ID_ChiTietDinhLuong= cthd.ID OR cthd.ID_ChiTietDinhLuong is null)
    			and (cthd.ID_ParentCombo= cthd.ID OR cthd.ID_ParentCombo is null)
END");

        }
        
        public override void Down()
        {
			DropStoredProcedure("[dbo].[GetQuyHoaDon_byIDHoaDon]");
            DropStoredProcedure("[dbo].[GetInforTheGiaTri_byID]");
        }
    }
}
