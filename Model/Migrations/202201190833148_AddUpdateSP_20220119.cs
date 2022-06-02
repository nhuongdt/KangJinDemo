namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20220119 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetHangHoaDatLichChiTiet]", parametersAction: p => new
            {
                Id = p.Guid()
            }, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT hh.ID, dvqd.MaHangHoa, hh.TenHangHoa, hh.LoaiHangHoa, dvqd.GiaBan AS DonGia FROM CSKH_DatLich_HangHoa dlhh
	INNER JOIN DM_HangHoa hh ON hh.ID = dlhh.IDHangHoa
	INNER JOIN DonViQuiDoi dvqd ON dvqd.ID_HangHoa = hh.ID
	WHERE dvqd.LaDonViChuan = 1
	AND dlhh.IDDatLich = @Id;");

            CreateStoredProcedure(name: "[dbo].[GetListDatLich]", parametersAction: p => new
            {
                IdChiNhanhs = p.String(),
                ThoiGianFrom = p.DateTime(),
                ThoiGianTo = p.DateTime(),
                TrangThais = p.String(20),
                TextSearch = p.String(),
                CurrentPage = p.Int(),
                PageSize = p.Int()
            }, body: @"SET NOCOUNT ON;
	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    	declare @tbTrangThai table (GiaTri varchar(2))
    	insert into @tbTrangThai
    	select Name from dbo.splitstring(@TrangThais);
    -- Insert statements for procedure here
	if(@PageSize != 0)
    	BEGIN
		with data_cte
    	as
    	(
	SELECT dl.Id, dl.ThoiGian, dl.IDDoiTuong AS IdKhachHang, ISNULL(dt.MaDoiTuong, '') AS MaKhachHang,
	dl.SoDienThoai, ISNULL(dt.TenDoiTuong, dl.TenDoiTuong) AS TenKhachHang, ISNULL(dt.DiaChi, dl.DiaChi) AS DiaChi,
	ISNULL(dt.NgaySinh_NgayTLap, dl.NgaySinh) AS NgaySinh, dl.IDXe, dl.BienSo, dl.LoaiXe AS MauXe, dl.TrangThai, dv.TenDonVi AS TenChiNhanh
	FROM CSKH_DatLich dl
	INNER JOIN @tblDonVi donvi ON dl.IDDonVi = donvi.ID_DonVi
	INNER JOIN DM_DonVi dv ON donvi.ID_DonVi = dv.ID
	LEFT JOIN DM_DoiTuong dt ON dt.ID = dl.IDDoiTuong
	LEFT JOIN Gara_DanhMucXe xe ON xe.ID = dl.IDXe
	WHERE exists (select GiaTri from @tbTrangThai tt where dl.TrangThai = tt.GiaTri)
	AND dl.ThoiGian BETWEEN @ThoiGianFrom AND @ThoiGianTo
	AND ((select count(Name) from @tblSearch b where     			
    		dl.TenDoiTuong like '%'+b.Name+'%'
    		or dl.SoDienThoai like '%'+b.Name+'%'
    		or dl.DiaChi like '%'+b.Name+'%'		
    		or dl.BienSo like '%'+b.Name+'%'
    		or dl.LoaiXe like '%'+b.Name+'%'
    		or dl.NgaySinh like '%'+b.Name+'%'
    		or dv.TenDonVi like '%'+b.Name+'%'
    		)=@count or @count=0)
			), count_cte
    		as
    		(
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    		SELECT dt.*, ct.* FROM data_cte dt
    		CROSS JOIN count_cte ct
    		ORDER BY dt.ThoiGian desc
    		OFFSET (@CurrentPage * @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
		BEGIN
	with data_cte
    	as
    	(
	SELECT dl.Id, dl.ThoiGian, dl.IDDoiTuong AS IdKhachHang, ISNULL(dt.MaDoiTuong, '') AS MaKhachHang,
	dl.SoDienThoai, ISNULL(dt.TenDoiTuong, dl.TenDoiTuong) AS TenKhachHang, ISNULL(dt.DiaChi, dl.DiaChi) AS DiaChi,
	ISNULL(dt.NgaySinh_NgayTLap, dl.NgaySinh) AS NgaySinh, dl.IDXe, dl.BienSo, dl.LoaiXe AS MauXe, dl.TrangThai, dv.TenDonVi AS TenChiNhanh
	FROM CSKH_DatLich dl
	INNER JOIN @tblDonVi donvi ON dl.IDDonVi = donvi.ID_DonVi
	INNER JOIN DM_DonVi dv ON donvi.ID_DonVi = dv.ID
	LEFT JOIN DM_DoiTuong dt ON dt.ID = dl.IDDoiTuong
	LEFT JOIN Gara_DanhMucXe xe ON xe.ID = dl.IDXe
	WHERE exists (select GiaTri from @tbTrangThai tt where dl.TrangThai = tt.GiaTri)
	AND dl.ThoiGian BETWEEN @ThoiGianFrom AND @ThoiGianTo
	AND ((select count(Name) from @tblSearch b where     			
    		dl.TenDoiTuong like '%'+b.Name+'%'
    		or dl.SoDienThoai like '%'+b.Name+'%'
    		or dl.DiaChi like '%'+b.Name+'%'		
    		or dl.BienSo like '%'+b.Name+'%'
    		or dl.LoaiXe like '%'+b.Name+'%'
    		or dl.NgaySinh like '%'+b.Name+'%'
    		or dv.TenDonVi like '%'+b.Name+'%'
    		)=@count or @count=0)
			)
			SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
    			ORDER BY dt.ThoiGian desc;
			END");

            CreateStoredProcedure(name: "[dbo].[GetListHangHoaDatLichCheckin]",
                body: @"SET NOCOUNT ON;

	-- Insert statements for procedure here
	DECLARE @tblResult TABLE (ID UNIQUEIDENTIFIER, MaHangHoa NVARCHAR(max), TenHangHoa NVARCHAR(MAX), GhiChu NVARCHAR(MAX), DonGia FLOAT, URLAnh NVARCHAR(MAX));
	INSERT INTO @tblResult
	SELECT hh.ID, dvqd.MaHangHoa, hh.TenHangHoa, hh.GhiChu, dvqd.GiaBan, '' FROM DM_HangHoa hh
	INNER JOIN DonViQuiDoi dvqd ON hh.ID = dvqd.ID_HangHoa
	WHERE dvqd.LaDonViChuan = 1 and hh.HienThiDatLich = 1
	ORDER BY TenHangHoa;

	UPDATE trs
	SET trs.URLAnh = a.URLAnh
	FROM @tblResult trs
	LEFT JOIN
	(SELECT hha.ID_HangHoa, MAX(hha.URLAnh) AS URLAnh FROM DM_HangHoa_Anh hha
	INNER JOIN @tblResult rs ON hha.ID_HangHoa = rs.ID
	WHERE hha.SoThuTu = 1
	GROUP BY hha.ID_HangHoa) a ON a.ID_HangHoa = trs.ID

	SELECT * FROM @tblResult");

            CreateStoredProcedure(name: "[dbo].[GetListNhanVienDatLichCheckin]", parametersAction: p => new
            {
                IdDonVi = p.Guid()
            }, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  TOP 10 nv.ID, nv.TenNhanVien, nva.URLAnh FROM NS_NhanVien nv
	INNER JOIN NS_QuaTrinhCongTac qtct ON nv.ID = qtct.ID_NhanVien
	LEFT JOIN NS_NhanVien_Anh nva ON nva.ID_NhanVien = nv.ID
	WHERE nv.TrangThai = 0 and qtct.ID_DonVi = @IdDonVi
	ORDER BY TenNhanVien;");

            CreateStoredProcedure(name: "[dbo].[GetNhanVienDatLichChiTiet]", parametersAction: p => new
            {
                Id = p.Guid()
            }, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT nv.ID, nv.MaNhanVien, nv.TenNhanVien FROM CSKH_DatLich_NhanVien dlnv
	INNER JOIN NS_NhanVien nv ON nv.ID = dlnv.IDNhanVien
	WHERE dlnv.IDDatLich = @Id");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDichVu_NhatKySuDungTongHop]
    @Text_Search [nvarchar](max),    
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
	@ThoiHan [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @dtNow datetime = getdate()

	declare @tblCTMua table(
		MaHoaDon nvarchar(max),
		NgayLapHoaDon datetime,
		NgayApDungGoiDV datetime,
		HanSuDungGoiDV datetime,
		ID_DonVi uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		SoLuong float,
		DonGia float,
		TienChietKhau float,
		ThanhTien float,
		GiamGiaHD float)
	insert into @tblCTMua
	exec BaoCaoGoiDV_GetCTMua @ID_ChiNhanh,@timeStart,@timeEnd

			select 
				b.MaHangHoa, 
				b.TenHangHoa, 
				b.MaLoHang as TenLoHang,
				b.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				CONCAT(b.TenHangHoa, b.ThuocTinhGiaTri) as TenHangHoaFull,
				b.TenDonViTinh,
				b.TenNhomHang,				
				b.MaDoiTuong as MaKhachHang,
				b.TenDoiTuong as TenKhachHang,
				b.DienThoai, 
				b.GioiTinh, 
				b.TenNguonKhach, 
				b.NhomKhachHang,
				b.NguoiGioiThieu,
				sum(SoLuong) as SoLuongMua,
				sum(SoLuongTra) as SoLuongTra,
				sum(SoLuongSuDung) as SoLuongSuDung,
				round(sum(SoLuong) - sum(SoLuongTra) -  sum(SoLuongSuDung),2) as SoLuongConLai
				from
				(
			
					select 
						ctm.ID_HoaDon,
						ctm.MaHoaDon,
						ctm.NgayLapHoaDon,
						ctm.NgayApDungGoiDV,
						ctm.HanSuDungGoiDV,
						dt.MaDoiTuong,
						dt.TenDoiTuong,
						dt.DienThoai,
						Case when dt.GioiTinhNam = 1 then N'Nam' else N'Nữ' end as GioiTinh,
						gt.TenDoiTuong as NguoiGioiThieu,
						nk.TenNguonKhach,
						isnull(dt.TenNhomDoiTuongs, N'Nhóm mặc định') as NhomKhachHang ,
						iif( hh.ID_NhomHang is null, '00000000-0000-0000-0000-000000000000',hh.ID_NhomHang) as ID_NhomHang,
						iif(@dtNow <=ctm.HanSuDungGoiDV,1,0) as ThoiHan,						
						ctm.SoLuong,
						ctm.DonGia,
						ctm.TienChietKhau,
						ctm.ThanhTien,
						isnull(tbl.SoLuongTra,0) as SoLuongTra,
						isnull(tbl.GiaTriTra,0) as GiaTriTra,
						isnull(tbl.SoLuongSuDung,0) as SoLuongSuDung,						
						ctm.SoLuong- isnull(tbl.SoLuongTra,0) - isnull(tbl.SoLuongSuDung,0)  as SoLuongConLai,
						qd.MaHangHoa,
						qd.TenDonViTinh,
						hh.TenHangHoa,
						qd.ThuocTinhGiaTri,
						lo.MaLoHang,
						nhom.TenNhomHangHoa as TenNhomHang
					from @tblCTMua ctm
					inner join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
					inner join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
					left join DM_LoHang lo on ctm.ID_LoHang= lo.ID
					left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
					left join DM_DoiTuong dt on ctm.ID_DoiTuong = dt.ID
					left join DM_DoiTuong gt on dt.ID_NguoiGioiThieu = gt.ID
					left join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID		
					
					left join (
						select 
							tblSD.ID_ChiTietGoiDV,
							sum(tblSD.SoLuongTra) as SoLuongTra,
							sum(tblSD.GiaTriTra) as GiaTriTra,
							sum(tblSD.SoLuongSuDung) as SoLuongSuDung,
							sum(tblSD.GiaVon) as GiaVon
						from 
						(
							---- hdsudung
							Select 								
								ct.ID_ChiTietGoiDV,														
								0 as SoLuongTra,
								0 as GiaTriTra,
								ct.SoLuong as SoLuongSuDung,
								ct.SoLuong * ct.GiaVon as GiaVon
							FROM BH_HoaDon hd
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							join @tblCTMua ctm on ct.ID_ChiTietGoiDV = ctm.ID
							where hd.ChoThanhToan= 0
							and hd.LoaiHoaDon in (1,25)
							and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
							

							union all
							--- hdtra
							Select 							
								ct.ID_ChiTietGoiDV,															
								ct.SoLuong as SoLuongTra,
								ct.ThanhTien as GiaTriTra,
								0 as SoLuongSuDung,
								0 as GiaVon
							FROM BH_HoaDon hd
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							join @tblCTMua ctm on ct.ID_ChiTietGoiDV = ctm.ID
							where hd.ChoThanhToan= 0
							and hd.LoaiHoaDon = 6
							and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)							
							)tblSD group by tblSD.ID_ChiTietGoiDV

					) tbl on ctm.ID= tbl.ID_ChiTietGoiDV
				where hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    			and qd.Xoa like @TrangThai
				and exists (select ID from dbo.GetListNhomHangHoa(@ID_NhomHang) nhomS where nhom.ID= nhomS.ID)
				AND ((select count(Name) from @tblSearchString b where 
					ctm.MaHoaDon like '%'+b.Name+'%'
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or qd.MaHangHoa like '%'+b.Name+'%'
    				or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'
    				or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoituong like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
					)=@count or @count=0)
			) b where b.ThoiHan like @ThoiHan
				group by b.MaHangHoa, b.TenHangHoa, b.ThuocTinhGiaTri,b.TenDonViTinh, b.MaLoHang, b.TenNhomHang,				
				b.MaDoiTuong, b.TenDoiTuong, b.DienThoai, b.GioiTinh, b.TenNguonKhach, b.NhomKhachHang, b.NguoiGioiThieu
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDoanhThuSuaChuaChiTiet]
    @IdChiNhanhs [nvarchar](max),
    @ThoiGianFrom [datetime],
    @ThoiGianTo [datetime],
    @DoanhThuFrom [float],
    @DoanhThuTo [float],
    @LoiNhuanFrom [float],
    @LoiNhuanTo [float],
    @TextSearch [nvarchar](max),
	@IdNhomHangHoa UNIQUEIDENTIFIER
AS
--exec BaoCaoDoanhThuSuaChuaChiTiet 'd93b17ea-89b9-4ecf-b242-d03b8cde71de', '2021-06-07', '2021-06-08', null, null, null, null, '', null
BEGIN

    SET NOCOUNT ON;
	--DECLARE @IdChiNhanhs [nvarchar](max),
 --   @ThoiGianFrom [datetime],
 --   @ThoiGianTo [datetime],
 --   @DoanhThuFrom [float],
 --   @DoanhThuTo [float],
 --   @LoiNhuanFrom [float],
 --   @LoiNhuanTo [float],
 --   @TextSearch [nvarchar](max), @IdNhomHangHoa UNIQUEIDENTIFIER;
	--SET @IdChiNhanhs = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de';
	--SET @ThoiGianFrom = '2021-09-30';
	--SET @ThoiGianTo = '2021-10-01';
	--SET @TextSearch = 'HDSC202105000591'
    -- Insert statements for procedure here
    -- Insert statements for procedure here
    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
	DECLARE @tblIdNhomHangHoa TABLE(ID UNIQUEIDENTIFIER);
	IF(@IdNhomHangHoa IS NOT NULL)
	BEGIN
		WITH parents AS (
		  SELECT ID, TenNhomHangHoa, ID_Parent
		  FROM DM_NhomHangHoa
		  WHERE ID = @IdNhomHangHoa

		  UNION ALL

		  SELECT nhh.ID, nhh.TenNhomHangHoa, nhh.ID_Parent
		  FROM DM_NhomHangHoa nhh
		  INNER JOIN parents p ON nhh.ID_Parent = p.ID
		)
		INSERT INTO @tblIdNhomHangHoa SELECT ID FROM parents;
	END
	ELSE
	BEGIN
		INSERT INTO @tblIdNhomHangHoa SELECT ID FROM DM_NhomHangHoa;
	END

    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    	Select @count =  (Select count(*) from @tblSearch);
    
    	DECLARE @tblHoaDonSuaChua TABLE (IDPhieuTiepNhan UNIQUEIDENTIFIER, MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), 
    	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
    	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_DonViQuiDoi UNIQUEIDENTIFIER, IDChiTiet UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    	MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), SoLuong FLOAT, DonGia FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TienThue FLOAT,
    	GiamGia FLOAT, DoanhThu FLOAT,
    	GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), ChiPhi FLOAT);
    
    	INSERT INTO @tblHoaDonSuaChua
    	SELECT ptn.ID, ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, dmx.BienSo, dt.MaDoiTuong, dt.TenDoiTuong, nv.TenNhanVien, hd.ID,
    	hd.MaHoaDon, hd.NgayLapHoaDon, hdct.ID_DonViQuiDoi, hdct.ID, hdct.ID_ChiTietDinhLuong,
    	dvqd.MaHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh, hdct.SoLuong, 
		IIF(hdct.ID_ParentCombo = hdct.ID OR hdct.ID_ParentCombo IS NULL, hdct.DonGia, 0), 
		hdct.TienChietKhau*hdct.SoLuong, 
		IIF((hdct.ID_ParentCombo = hdct.ID OR hdct.ID_ParentCombo IS NULL) AND (hdct.ID_ChiTietDinhLuong IS NULL OR hdct.ID_ChiTietDinhLuong = hdct.ID),hdct.ThanhTien, 0) AS ThanhTien, 
		IIF((hdct.ID_ParentCombo = hdct.ID OR hdct.ID_ParentCombo IS NULL) AND (hdct.ID_ChiTietDinhLuong IS NULL OR hdct.ID_ChiTietDinhLuong = hdct.ID),
		IIF(hd.TongThueKhachHang = 0 AND hd.TongTienThueBaoHiem <> 0, (hdct.DonGia - hdct.TienChietKhau)*hdct.SoLuong * (hd.TongTienThue/hd.TongTienHang), hdct.TienThue*hdct.SoLuong), 0) AS TienThue,
    	IIF((hdct.ID_ParentCombo = hdct.ID OR hdct.ID_ParentCombo IS NULL) AND (hdct.ID_ChiTietDinhLuong IS NULL OR hdct.ID_ChiTietDinhLuong = hdct.ID),
		IIF(hd.TongTienHang = 0, 0, 
		hdct.ThanhTien * hd.TongGiamGia/hd.TongTienHang),0) AS GiamGia, 
		IIF((hdct.ID_ParentCombo = hdct.ID OR hdct.ID_ParentCombo IS NULL) AND (hdct.ID_ChiTietDinhLuong IS NULL OR hdct.ID_ChiTietDinhLuong = hdct.ID),
		IIF(hd.TongTienHang =0,hdct.ThanhTien,(hdct.ThanhTien * (1 - hd.TongGiamGia/hd.TongTienHang))),0) AS DoanhThu,
    	hdct.GhiChu, dv.MaDonVi, dv.TenDonVi, 0 FROM Gara_PhieuTiepNhan ptn
    	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
    	INNER JOIN BH_HoaDon_ChiTiet hdct ON hd.ID = hdct.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID
    	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
		INNER JOIN @tblIdNhomHangHoa nhh ON hh.ID_NhomHang = nhh.ID
    	INNER JOIN Gara_DanhMucXe dmx ON ptn.ID_Xe = dmx.ID
    	INNER JOIN DM_DoiTuong dt ON dt.ID = ptn.ID_KhachHang
    	LEFT JOIN NS_NhanVien nv ON ptn.ID_CoVanDichVu = nv.ID
    	INNER JOIN DM_DonVi dv ON dv.ID = ptn.ID_DonVi
    	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
    	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0  --AND ptn.TrangThai != 0 
    	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
    	AND ((select count(Name) from @tblSearch b where     			
    			ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
    			or dmx.BienSo like '%'+b.Name+'%'
    			or dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or nv.TenNhanVien like '%'+b.Name+'%'
    			or hd.MaHoaDon like '%'+b.Name+'%'
    			or hd.DienGiai like '%'+b.Name+'%'
				or hh.TenHangHoa like '%'+b.Name+'%'
				or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
				or dvqd.MaHangHoa like '%'+b.Name+'%'
    			)=@count or @count=0);
		--SELECT * FROM @tblHoaDonSuaChua

		--SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, hdsc.IDChiTiet, hdsc.ID_ChiTietDinhLuong,
  --  	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
  --  	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon,
  --  	hdsc.MaHangHoa, hdsc.TenHangHoa, hdsc.TenDonViTinh, hdsc.SoLuong, hdsc.DonGia, hdsc.TienChietKhau, hdsc.ThanhTien, hdsc.TienThue,
  --  	hdsc.GiamGia, hdsc.DoanhThu,
  --  	hdsc.GhiChu, hdsc.MaDonVi, hdsc.TenDonVi, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk
  --  	FROM @tblHoaDonSuaChua hdsc
  --  	LEFT JOIN BH_HoaDon xk ON hdsc.ID = xk.ID_HoaDon
  --  	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon 
		--AND xkct.ID_ChiTietGoiDV = hdsc.IDChiTiet
  --  	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL 

		DECLARE @tblGiaVonThanhPhan TABLE(IDChiTietDinhLuong UNIQUEIDENTIFIER, TongTIenVon FLOAT)
		INSERT INTO @tblGiaVonThanhPhan
		SELECT hdsc.ID_ChiTietDinhLuong, SUM(ISNULL(xkct.GiaVon,0) * ISNULL(xkct.SoLuong,0)) AS TongTienVon
    	FROM @tblHoaDonSuaChua hdsc
    	LEFT JOIN BH_HoaDon xk ON hdsc.ID = xk.ID_HoaDon
    	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon 
		AND xkct.ID_ChiTietGoiDV = hdsc.IDChiTiet
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) AND hdsc.ID_ChiTietDinhLuong IS NOT NULL
		GROUP BY hdsc.ID_ChiTietDinhLuong
    
    	DECLARE @tblBaoCaoDoanhThu TABLE(MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), IDChiTiet UNIQUEIDENTIFIER, 
    	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
    	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), SoLuong FLOAT, DonGia FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, 
    	TienThue FLOAT,
    	GiamGia FLOAT, DoanhThu FLOAT, GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), TienVon FLOAT, LoiNhuan FLOAT, ChiPhi FLOAT)
    
    	INSERT INTO @tblBaoCaoDoanhThu
		SELECT bcsc.MaPhieuTiepNhan, bcsc.NgayVaoXuong, bcsc.BienSo, bcsc.IDChiTiet,
    	bcsc.MaDoiTuong, bcsc.TenDoiTuong, bcsc.CoVanDichVu,
    	bcsc.ID, bcsc.MaHoaDon, bcsc.NgayLapHoaDon,
    	bcsc.MaHangHoa, bcsc.TenHangHoa, bcsc.TenDonViTinh, bcsc.SoLuong, bcsc.DonGia, bcsc.TienChietKhau, bcsc.ThanhTien, bcsc.TienThue,
    	bcsc.GiamGia, bcsc.DoanhThu,
    	bcsc.GhiChu, bcsc.MaDonVi, bcsc.TenDonVi, SUM(ISNULL(bcsc.GiaVon,0)*ISNULL(bcsc.SoLuongxk,0)) AS TienVon,
    	bcsc.DoanhThu - SUM(ISNULL(bcsc.GiaVon,0)*ISNULL(bcsc.SoLuongxk,0)) AS LoiNhuan, 0
		FROM
    	(
		SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, hdsc.IDChiTiet, hdsc.ID_ChiTietDinhLuong,
    	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
    	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon,
    	hdsc.MaHangHoa, hdsc.TenHangHoa, hdsc.TenDonViTinh, hdsc.SoLuong, hdsc.DonGia, hdsc.TienChietKhau, hdsc.ThanhTien, hdsc.TienThue,
    	hdsc.GiamGia, hdsc.DoanhThu,
    	hdsc.GhiChu, hdsc.MaDonVi, hdsc.TenDonVi, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk
    	FROM @tblHoaDonSuaChua hdsc
    	LEFT JOIN BH_HoaDon xk ON hdsc.ID = xk.ID_HoaDon
    	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon 
		AND xkct.ID_ChiTietGoiDV = hdsc.IDChiTiet
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL 
		--AND (hdsc.IDChiTiet = hdsc.ID_ChiTietDinhLuong OR hdsc.ID_ChiTietDinhLuong IS NULL)
		UNION ALL
		SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, NULL, null,
    	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
    	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon,
    	dvqd.MaHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh, 0 AS SoLuong, 0 AS DonGia, 0 AS TienChietKhau, 0 AS ThanhTien, 0 AS TienThue,
    	0 AS GiamGia, 0 AS DoanhThu,
    	'' AS GhiChu, hdsc.MaDonVi, hdsc.TenDonVi, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk FROM
		(SELECT IDPhieuTiepNhan, MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu, 
		MaDonVi, TenDonVi, MaHoaDon, ID, NgayLapHoaDon
    	FROM @tblHoaDonSuaChua GROUP BY IDPhieuTiepNhan, MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu, MaDonVi, 
		TenDonVi, MaHoaDon, ID, NgayLapHoaDon)
		hdsc
    	INNER JOIN BH_HoaDon xk ON hdsc.ID = xk.ID_HoaDon
		INNER JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon 
		INNER JOIN DonViQuiDoi dvqd ON xkct.ID_DonViQuiDoi = dvqd.ID
    	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
		INNER JOIN @tblIdNhomHangHoa nhh ON hh.ID_NhomHang = nhh.ID
		--AND xkct.ID_ChiTietGoiDV = hdsc.IDChiTiet
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) AND xkct.ID_ChiTietGoiDV IS NULL
		UNION ALL
		--Xuât kho cho phiếu tiếp nhận
		SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, NULL, null,
    	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
    	null, '', null,
    	dvqd.MaHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh, 0, 0, 0, 0, 0,
    	0, 0,
    	'', hdsc.MaDonVi, hdsc.TenDonVi, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk FROM
		(SELECT IDPhieuTiepNhan, MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu, MaDonVi, TenDonVi
    	FROM @tblHoaDonSuaChua GROUP BY IDPhieuTiepNhan, MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu, MaDonVi, TenDonVi)
		hdsc
    	INNER JOIN BH_HoaDon xk ON hdsc.IDPhieuTiepNhan = xk.ID_PhieuTiepNhan
		INNER JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon 
		INNER JOIN DonViQuiDoi dvqd ON xkct.ID_DonViQuiDoi = dvqd.ID
    	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
		INNER JOIN @tblIdNhomHangHoa nhh ON hh.ID_NhomHang = nhh.ID
		--AND xkct.ID_ChiTietGoiDV = hdsc.IDChiTiet
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) AND xk.ID_HoaDon IS NULL
		) bcsc WHERE bcsc.IDChiTiet = bcsc.ID_ChiTietDinhLuong OR bcsc.ID_ChiTietDinhLuong IS NULL
		--AND ((select count(Name) from @tblSearch b where 
		--	MaHangHoa like '%'+b.Name+'%'
  --  			)=@count or @count=0) 
    	GROUP BY bcsc.MaPhieuTiepNhan, bcsc.NgayVaoXuong, bcsc.BienSo, bcsc.IDChiTiet, bcsc.ID_ChiTietDinhLuong,
    	bcsc.MaDoiTuong, bcsc.TenDoiTuong, bcsc.CoVanDichVu,
    	bcsc.ID, bcsc.MaHoaDon, bcsc.NgayLapHoaDon,
    	bcsc.MaHangHoa, bcsc.TenHangHoa, bcsc.TenDonViTinh, bcsc.SoLuong, bcsc.DonGia, bcsc.TienChietKhau, bcsc.ThanhTien, bcsc.TienThue,
    	bcsc.GiamGia, bcsc.DoanhThu,
    	bcsc.GhiChu, bcsc.MaDonVi, bcsc.TenDonVi;

		UPDATE bcdt SET
		bcdt.TienVon = gvtp.TongTIenVon, bcdt.LoiNhuan = bcdt.DoanhThu - gvtp.TongTIenVon
		FROM @tblBaoCaoDoanhThu bcdt
		INNER JOIN @tblGiaVonThanhPhan gvtp ON bcdt.IDChiTiet = gvtp.IDChiTietDinhLuong;
    
		DECLARE @tblChiPhi TABLE(IDChiTiet UNIQUEIDENTIFIER, ChiPhi FLOAT);
		INSERT INTO @tblChiPhi
		SELECT hdcp.ID_HoaDon_ChiTiet, SUM(hdcp.ThanhTien) FROM BH_HoaDon_ChiPhi hdcp
		INNER JOIN @tblBaoCaoDoanhThu bcdt ON hdcp.ID_HoaDon_ChiTiet = bcdt.IDChiTiet
		GROUP BY hdcp.ID_HoaDon_ChiTiet

		UPDATE bcdt SET
		bcdt.ChiPhi = hdcp.ChiPhi, bcdt.LoiNhuan = bcdt.LoiNhuan - hdcp.ChiPhi
		FROM @tblBaoCaoDoanhThu bcdt
		INNER JOIN @tblChiPhi hdcp ON bcdt.IDChiTiet = hdcp.IDChiTiet;

    	DECLARE @SThanhTien FLOAT,  @SChietKhau FLOAT, @SThue FLOAT, @SGiamGia FLOAT, @SDoanhThu FLOAT, @STongTienVon FLOAT, @SLoiNhuan FLOAT, @SChiPhi FLOAT;
    	SELECT @SThanhTien = SUM(ThanhTien), @SChietKhau = SUM(TienChietKhau), @SThue = SUM(TienThue), @SGiamGia = SUM(GiamGia), 
		@SDoanhThu = SUM(DoanhThu), @STongTienVon = SUM(TienVon), @SLoiNhuan = SUM(LoiNhuan), @SChiPhi = SUM(ChiPhi)
    	FROM @tblBaoCaoDoanhThu
    
    	SELECT MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu , ID AS IDHoaDon, MaHoaDon,
    	NgayLapHoaDon, MaHangHoa, TenHangHoa, TenDonViTinh, ISNULL(SoLuong, 0) AS SoLuong, ISNULL(DonGia, 0) AS DonGia, ISNULL(TienChietKhau, 0) AS TienChietKhau, 
    	ISNULL(TienThue,0) AS TienThue, ISNULL(ThanhTien,0) AS ThanhTien, ISNULL(GiamGia, 0) AS GiamGia, ISNULL(DoanhThu, 0) AS DoanhThu, ISNULL(TienVon,0) AS TienVon, ISNULL(LoiNhuan,0) AS LoiNhuan,
    	GhiChu, MaDonVi, TenDonVi, ChiPhi, ISNULL(@SThanhTien, 0) AS SThanhTien, ISNULL(@SChietKhau,0) AS SChietKhau,
    	ISNULL(@SThue,0) AS SThue, ISNULL(@SGiamGia,0) AS SGiamGia, ISNULL(@SDoanhThu, 0) AS SDoanhThu, ISNULL(@STongTienVon,0) AS STongTienVon,
    	ISNULL(@SLoiNhuan,0) AS SLoiNhuan, ISNULL(@SChiPhi, 0) AS SChiPhi
    	FROM @tblBaoCaoDoanhThu
    	WHERE (@DoanhThuFrom IS NULL OR DoanhThu >= @DoanhThuFrom)
    	AND (@DoanhThuTo IS NULL OR DoanhThu <= @DoanhThuTo)
    	AND (@LoiNhuanFrom IS NULL OR LoiNhuan >= @LoiNhuanFrom)
    	AND (@LoiNhuanTo IS NULL OR LoiNhuan <= @LoiNhuanTo)
    	ORDER BY NgayLapHoaDon
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDoanhThuSuaChuaTheoXe]
    @IdChiNhanhs [nvarchar](max),
    @ThoiGianFrom [datetime],
    @ThoiGianTo [datetime],
    @SoLanTiepNhanFrom [float],
    @SoLanTiepNhanTo [float],
    @SoLuongHoaDonFrom [float],
    @SoLuongHoaDonTo [float],
    @DoanhThuFrom [float],
    @DoanhThuTo [float],
    @LoiNhuanFrom [float],
    @LoiNhuanTo [float],
    @TextSearch [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
 --   DECLARE @IdChiNhanhs [nvarchar](max),
 --   @ThoiGianFrom [datetime],
 --   @ThoiGianTo [datetime],
 --   @SoLanTiepNhanFrom [float],
 --   @SoLanTiepNhanTo [float],
 --   @SoLuongHoaDonFrom [float],
 --   @SoLuongHoaDonTo [float],
 --   @DoanhThuFrom [float],
 --   @DoanhThuTo [float],
 --   @LoiNhuanFrom [float],
 --   @LoiNhuanTo [float],
 --   @TextSearch [nvarchar](max);
	--SET @IdChiNhanhs = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de';
	--SET @ThoiGianFrom = '2021-12-01';
	--SET @ThoiGianTo = '2022-01-01';
	--SET @TextSearch = '30A-794.76'
    -- Insert statements for procedure here
    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    	Select @count =  (Select count(*) from @tblSearch);
    
    	DECLARE @tblHoaDonSuaChua TABLE (IDXe UNIQUEIDENTIFIER, BienSo NVARCHAR(MAX), SoMay NVARCHAR(MAX), SoKhung NVARCHAR(MAX), MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), DienThoai NVARCHAR(MAX), 
    	IDPhieuTiepNhan UNIQUEIDENTIFIER, IDHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, DoanhThu FLOAT, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));
    
    	INSERT INTO @tblHoaDonSuaChua
    	SELECT dmx.ID, dmx.BienSo, dmx.SoMay, dmx.SoKhung,
    	dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, ptn.ID, hd.ID, hd.NgayLapHoaDon, hd.TongThanhToan - hd.TongTienThue, dv.MaDonVi, dv.TenDonVi
    	FROM Gara_PhieuTiepNhan ptn
    	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
    	INNER JOIN Gara_DanhMucXe dmx ON ptn.ID_Xe = dmx.ID
    	LEFT JOIN DM_DoiTuong dt ON dt.ID = dmx.ID_KhachHang
    	INNER JOIN DM_DonVi dv ON dv.ID = ptn.ID_DonVi
    	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
    	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0
    	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
    	AND ((select count(Name) from @tblSearch b where     			
    			dmx.BienSo like '%'+b.Name+'%'
    			or dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dmx.SoMay like '%'+b.Name+'%'
    			or dmx.SoKhung like '%'+b.Name+'%'
    			or dt.DienThoai like '%'+b.Name+'%'
    			or dv.TenDonVi like '%'+b.Name + '%'
    			)=@count or @count=0);

				--SELECT * FROM @tblHoaDonSuaChua
    
    	DECLARE @tblTienVon TABLE(IDXe UNIQUEIDENTIFIER, TienVon FLOAT);
    
    	INSERT INTO @tblTienVon
    	SELECT hdsc.IDXe, SUM(ISNULL(hdsc.GiaVon,0)*ISNULL(hdsc.SoLuongxk,0)) AS TienVon
    	FROM (
		SELECT hdsc.IDXe, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk
    	FROM @tblHoaDonSuaChua hdsc
    	LEFT JOIN BH_HoaDon xk ON hdsc.IDHoaDon = xk.ID_HoaDon
    	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL
		UNION ALL
		SELECT hdsc.IDXe, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk
    	FROM (SELECT IDPhieuTiepNhan, IDXe FROM @tblHoaDonSuaChua GROUP BY IDPhieuTiepNhan, IDXe) hdsc
    	INNER JOIN BH_HoaDon xk ON hdsc.IDPhieuTiepNhan = xk.ID_PhieuTiepNhan
    	INNER JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) AND xk.ID_HoaDon IS NULL
		) hdsc
    	GROUP BY hdsc.IDXe;
    
    	DECLARE @SSoLanTiepNhan FLOAT, @SSoLuongHoaDon FLOAT, @STongDoanhThu FLOAT, @STienVon FLOAT, @SLoiNhuan FLOAT, @SChiPhi FLOAT;
    
    	DECLARE @tblBaoCaoDoanhThu TABLE(IDXe UNIQUEIDENTIFIER, BienSo NVARCHAR(MAX), SoKhung NVARCHAR(MAX), SoMay NVARCHAR(MAX), MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX),
    	DienThoai NVARCHAR(MAX), SoLanTiepNhan FLOAT, SoLuongHoaDon FLOAT, TongDoanhThu FLOAT, TongTienVon FLOAT, LoiNhuan FLOAT, NgayGiaoDichGanNhat DATETIME, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), ChiPhi FLOAT)
    	
    	INSERT INTO @tblBaoCaoDoanhThu
    	SELECT hd.IDXe, hd.BienSo, hd.SoKhung, hd.SoMay, hd.MaDoiTuong, hd.TenDoiTuong, hd.DienThoai, hd.SoLanTiepNhan, hd.SoLuongHoaDon,
    	ISNULL(hd.TongDoanhThu,0) AS TongDoanhThu, ISNULL(tv.TienVon,0) AS TongTienVon, ISNULL(hd.TongDoanhThu,0) - ISNULL(tv.TienVon,0) AS LoiNhuan, hd.NgayGiaoDichGanNhat, hd.MaDonVi, hd.TenDonVi, 0
    	FROM
    	(
    	SELECT IDXe, BienSo, SoMay, SoKhung,  MaDoiTuong, TenDoiTuong, DienThoai, MaDonVi, TenDonVi, COUNT(DISTINCT IDPhieuTiepNhan) AS SoLanTiepNhan, COUNT(IDHoaDon) AS SoLuongHoaDon, SUM(DoanhThu) AS TongDoanhThu,
    	MAX(NgayLapHoaDon) AS NgayGiaoDichGanNhat
    	FROM @tblHoaDonSuaChua
    	GROUP BY IDXe, BienSo, SoMay, SoKhung,  MaDoiTuong, TenDoiTuong, DienThoai, MaDonVi, TenDonVi) AS hd
    	LEFT JOIN @tblTienVon tv ON hd.IDXe = tv.IDXe
    	WHERE (@SoLanTiepNhanFrom IS NULL OR hd.SoLanTiepNhan >= @SoLanTiepNhanFrom)
    	AND (@SoLanTiepNhanTo IS NULL OR hd.SoLanTiepNhan <= @SoLanTiepNhanTo)
    	AND (@SoLuongHoaDonFrom IS NULL OR hd.SoLuongHoaDon >= @SoLuongHoaDonFrom)
    	AND (@SoLuongHoaDonTo IS NULL OR hd.SoLuongHoaDon <= @SoLuongHoaDonTo)
    	AND (@DoanhThuFrom IS NULL OR hd.TongDoanhThu >= @DoanhThuFrom)
    	AND (@DoanhThuTo IS NULL OR hd.TongDoanhThu <= @DoanhThuTo)
    	AND (@LoiNhuanFrom IS NULL OR hd.TongDoanhThu - tv.TienVon >= @LoiNhuanFrom)
    	AND (@LoiNhuanTo IS NULL OR hd.TongDoanhThu - tv.TienVon <= @LoiNhuanTo);

		DECLARE @tblChiPhi TABLE (IDXe UNIQUEIDENTIFIER, ChiPhi FLOAT);
		INSERT INTO @tblChiPhi
		SELECT hdsc.IDXe, SUM(hdcp.ThanhTien) FROM BH_HoaDon_ChiPhi hdcp
		INNER JOIN @tblHoaDonSuaChua hdsc ON hdcp.ID_HoaDon = hdsc.IDHoaDon
		GROUP BY hdsc.IDXe;

		UPDATE bcdt
		SET bcdt.ChiPhi = hdcp.ChiPhi, bcdt.LoiNhuan = bcdt.LoiNhuan - hdcp.ChiPhi FROM @tblBaoCaoDoanhThu bcdt
		INNER JOIN @tblChiPhi hdcp ON bcdt.IDXe = hdcp.IDXe;
    
    	SELECT @SSoLanTiepNhan = SUM(SoLanTiepNhan), @SSoLuongHoaDon = SUM(SoLuongHoaDon), @STongDoanhThu = SUM(TongDoanhThu), @STienVon = SUM(TongTienVon), @SLoiNhuan = SUM(LoiNhuan), @SChiPhi = SUM(ChiPhi) FROM @tblBaoCaoDoanhThu
    
    	SELECT *, CAST(@SSoLanTiepNhan AS FLOAT) AS SSoLanTiepNhan, @SSoLuongHoaDon AS SSoLuongHoaDon, @STongDoanhThu AS STongDoanhThu, @STienVon AS STienVon, @SLoiNhuan AS SLoiNhuan, @SChiPhi AS SChiPhi FROM @tblBaoCaoDoanhThu
    	ORDER BY BienSo
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TonKho_TongHop]
    @ID_DonVis [nvarchar](max),
    @ThoiGian [datetime],
	@SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@TonKho INT
AS
BEGIN
   SET NOCOUNT ON;
    DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER, MaDonVi nvarchar(max), TenDonVi nvarchar(max));
	INSERT INTO @tblChiNhanh 
	SELECT dv.id, dv.MaDonVi, dv.TenDonVi 
	FROM splitstring(@ID_DonVis) cn
	join DM_DonVi dv on cn.Name = dv.ID;   

	declare @tblNhomHang table(ID UNIQUEIDENTIFIER)
	

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung);

		declare @tkDauKy table (ID_DonVi uniqueidentifier,ID_HangHoa uniqueidentifier,	ID_LoHang uniqueidentifier null, TonKho float,GiaVon float)		
		insert into @tkDauKy
		exec dbo.GetAll_TonKhoDauKy @ID_DonVis, @ThoiGian	

		if @ID_NhomHang is null
		begin
			select ID AS ID_ChiNhanh,
				TenDonVi AS TenChiNhanh, 
				round(sum(TonCuoiKy),3) as SoLuong, 
				SUM(GiaTriCuoiKy) as GiaTri
			from
			(
    		SELECT dv.ID,
				dv.MaDonVi,
				dv.TenDonVi,
    			ROUND(ISNULL(tonkho.TonKho, 0), 3) AS TonCuoiKy,
    			IIF(@XemGiaVon = '1', ISNULL(tonkho.TonKho, 0) * ISNULL(tonkho.GiaVon, 0),0) AS GiaTriCuoiKy 
			FROM DM_HangHoa dhh   	
			JOIN DonViQuiDoi dvqd1 ON dhh.ID = dvqd1.ID_HangHoa		
			left JOIN DM_NhomHangHoa nhh ON  dhh.ID_NhomHang = nhh.ID 
    		LEFT JOIN DM_LoHang lh ON dhh.ID = lh.ID_HangHoa    			
			cross join @tblChiNhanh dv 
    		LEFT JOIN @tkDauKy tonkho ON dhh.ID = tonkho.ID_HangHoa AND dv.ID = tonkho.ID_DonVi
    		 AND (lh.ID = tonkho.ID_LoHang OR (tonkho.ID_LoHang IS NULL and dhh.QuanLyTheoLoHang='0')) 
    		WHERE dhh.LaHangHoa = 1 
			AND dhh.TheoDoi LIKE @TheoDoi AND dvqd1.Xoa LIKE @TrangThai AND dvqd1.LaDonViChuan = 1
			and exists (select ID from @tblChiNhanh dv2 where dv.ID= dv2.ID)
			AND 
			IIF(@TonKho = 1, ISNULL(tonkho.TonKho, 0), 1) > 0
			AND IIF(@TonKho = 2, ISNULL(tonkho.TonKho, 0), 0) <= 0
			and IIF(@TonKho = 3, isnull(tonkho.TonKho,0), -1) < 0		
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
    					or dvqd1.ThuocTinhGiaTri like '%'+b.Name+'%'
						or dv.MaDonVi like '%'+b.Name+'%'
						or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)
						) a
						GROUP BY a.ID, a.TenDonVi
		end
		else
		begin
			insert into @tblNhomHang
			SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) 	

			select ID AS ID_ChiNhanh,
				TenDonVi AS TenChiNhanh, 
				round(sum(TonCuoiKy),3) as SoLuong, 
				SUM(GiaTriCuoiKy) as GiaTri
			from
			(
    		SELECT dv.ID,
				dv.MaDonVi,
				dv.TenDonVi,
    			ROUND(ISNULL(tonkho.TonKho, 0), 3) AS TonCuoiKy,
    			IIF(@XemGiaVon = '1', ISNULL(tonkho.TonKho, 0) * ISNULL(tonkho.GiaVon, 0),0) AS GiaTriCuoiKy 
			FROM DM_HangHoa dhh   	
			JOIN DonViQuiDoi dvqd1 ON dhh.ID = dvqd1.ID_HangHoa		
			left JOIN DM_NhomHangHoa nhh ON  dhh.ID_NhomHang = nhh.ID 
    		LEFT JOIN DM_LoHang lh ON dhh.ID = lh.ID_HangHoa    			
			cross join @tblChiNhanh dv 
    		LEFT JOIN @tkDauKy tonkho ON dhh.ID = tonkho.ID_HangHoa AND dv.ID = tonkho.ID_DonVi
    		 AND (lh.ID = tonkho.ID_LoHang OR (tonkho.ID_LoHang IS NULL and dhh.QuanLyTheoLoHang='0')) 
    		WHERE dhh.LaHangHoa = 1 
			AND dhh.TheoDoi LIKE @TheoDoi AND dvqd1.Xoa LIKE @TrangThai AND dvqd1.LaDonViChuan = 1
			and exists (select ID from @tblChiNhanh dv2 where dv.ID= dv2.ID)
			and exists (SELECT ID FROM @tblNhomHang allnhh where nhh.ID = allnhh.ID)	
			AND 
			IIF(@TonKho = 1, ISNULL(tonkho.TonKho, 0), 1) > 0
			AND IIF(@TonKho = 2, ISNULL(tonkho.TonKho, 0), 0) <= 0
			and IIF(@TonKho = 3, isnull(tonkho.TonKho,0), -1) < 0		
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
    					or dvqd1.ThuocTinhGiaTri like '%'+b.Name+'%'
						or dv.MaDonVi like '%'+b.Name+'%'
						or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)
						) a
						GROUP BY a.ID, a.TenDonVi
		end

		
END");

			Sql(@"ALTER PROCEDURE [dbo].[DanhMucKhachHang_CongNo_Paging]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @MaKH [nvarchar](max),
    @LoaiKH [int],
    @ID_NhomKhachHang [nvarchar](max),
    @timeStartKH [datetime],
    @timeEndKH [datetime],
    @CurrentPage [int],
    @PageSize [float],
    @Where [nvarchar](max),
    @SortBy [nvarchar](100)
AS
BEGIN
    set nocount on
    	
    	if @SortBy ='' set @SortBy = ' dt.NgayTao DESC'
    	if @Where='' set @Where= ''
    	else set @Where= ' AND '+ @Where 
    
    	declare @from int= @CurrentPage * @PageSize + 1  
    	declare @to int= (@CurrentPage + 1) * @PageSize 
    
    	declare @sql1 nvarchar(max)= concat('
    	declare @tblIDNhoms table (ID varchar(36)) 
    	declare @idNhomDT nvarchar(max) = ''', @ID_NhomKhachHang, '''
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh select * from splitstring(''',@ID_ChiNhanh, ''')
    	
    	if @idNhomDT =''%%''
    		begin
				insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')

    			-- check QuanLyKHTheochiNhanh
    			--declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))  
				
				declare @QLTheoCN bit = 0;
				declare @countQL int=0;
				select distinct QuanLyKhachHangTheoDonVi into #temp from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)
				set @countQL = (select COUNT(*) from #temp)
				if	@countQL= 1 
						set @QLTheoCN = (select QuanLyKhachHangTheoDonVi from #temp)
				
    			if @QLTheoCN = 1
    				begin									
    					insert into @tblIDNhoms(ID)
    					select *  from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = ',@LoaiKH ,'
    						union all
    						-- get Nhom at this ChiNhanh
    						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) ) tbl
    				end
    			else
    				begin				
    				-- insert all
    				insert into @tblIDNhoms(ID)
    				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    				where LoaiDoiTuong = ',@LoaiKH, ' 
    				end		
    		end
    	else
    		begin
    			set @idNhomDT = REPLACE( @idNhomDT,''%'','''')
    			insert into @tblIDNhoms(ID) values (@idNhomDT)
    		end
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](''',@MaKH,''', '' '') where Name!='''';
    	Select @count =  (Select count(*) from @tblSearchString);')
    
    	declare @sql2 nvarchar(max)= concat(' WITH Data_CTE ',
    									' AS ',
    									' ( ',
    
    N'SELECT  * 
    		FROM
    		(  
    			SELECT 
    		  dt.ID as ID,
    		  dt.MaDoiTuong, 
    			  case when dt.IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' 
				  else  ISNULL(dt.IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
    		  dt.TenDoiTuong_KhongDau,
    		  dt.TenDoiTuong_ChuCaiDau,
    			  dt.ID_TrangThai,
				   dt.TheoDoi,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
			   dt.NgayGiaoDichGanNhat,
    			  ISNULL(dt.DienThoai,'''') as DienThoai,
    			  ISNULL(dt.Email,'''') as Email,
    			  ISNULL(dt.DiaChi,'''') as DiaChi,
    			  ISNULL(dt.MaSoThue,'''') as MaSoThue,
				  dt.TaiKhoanNganHang,
    		  ISNULL(dt.GhiChu,'''') as GhiChu,
    		  dt.NgayTao,
    		  dt.DinhDang_NgaySinh,
    		  ISNULL(dt.NguoiTao,'''') as NguoiTao,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    			  dt.ID_DonVi, --- Collate Vietnamese_CI_AS
    		  dt.LaCaNhan,
    		  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
    			 case when right(rtrim(dt.TenNhomDoiTuongs),1) ='','' then LEFT(Rtrim(dt.TenNhomDoiTuongs),
				  len(dt.TenNhomDoiTuongs)-1) else				  
				  ISNULL(dt.TenNhomDoiTuongs, N''Nhóm mặc định'') end   as TenNhomDT,-- remove last coma
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
    			ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
			  a.PhiDichVu,
    			  CAST(0 as float) as TongNapThe , 
    			  CAST(0 as float) as SuDungThe , 
    			  CAST(0 as float) as HoanTraTheGiaTri , 
    			  CAST(0 as float) as SoDuTheGiaTri , 
    			  concat(dt.MaDoiTuong,'' '',lower(dt.MaDoiTuong) ,'' '', dt.TenDoiTuong,'' '', dt.DienThoai,'' '', dt.TenDoiTuong_KhongDau)  as Name_Phone			
    		  FROM DM_DoiTuong dt  ','')
    	
    	declare @sql3 nvarchar(max)= concat('
    				LEFT JOIN (
    					SELECT tblThuChi.ID_DoiTuong,
    						SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) + sum(ISNULL(tblThuChi.ThuTuThe,0))
							- sum(isnull(tblThuChi.PhiDichVu,0)) 
						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    					SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
						sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    					SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    					SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang,
						sum(isnull(tblThuChi.PhiDichVu,0)) as PhiDichVu
    				FROM
    				(
    					select 
							cp.ID_NhaCungCap as ID_DoiTuong,
							0 as GiaTriTra,
    						0 as DoanhThu,
							0 AS TienThu,
    						0 AS TienChi, 
    						0 AS SoLanMuaHang,
							0 as ThuTuThe,
							sum(cp.ThanhTien) as PhiDichVu
						from BH_HoaDon_ChiPhi cp
						join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
						where hd.ChoThanhToan = 0
						and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
						group by cp.ID_NhaCungCap

						union all
    						---- tongban
    						SELECT 
    							bhd.ID_DoiTuong,    	
								0 as GiaTriTra,
    							bhd.PhaiThanhToan as DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,25) AND bhd.ChoThanhToan = ''0'' 
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 

							
							union all
							---- doanhthu tuthe
							select 
								bhd.ID_DoiTuong,
								0 as GiaTriTra,
    							0 AS DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								bhd.PhaiThanhToan as ThuTuThe,
								0 as PhiDichVu
							from BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon = 22 AND bhd.ChoThanhToan = ''0'' 
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 						

						
    							 union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,
								bhd.PhaiThanhToan AS GiaTriTra,    							
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu
    						FROM BH_HoaDon bhd   						
    						WHERE bhd.LoaiHoaDon in (6,4) AND bhd.ChoThanhToan = ''0''  
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,		
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
							   							
    							 union all
    
    							-- tienthu
    							SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon   						
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.HinhThucThanhToan!= 6
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
    						AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID) 
    							AND qhd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd  ,
    							
    							''' union all
    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon 							
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.HinhThucThanhToan!= 6
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
    							AND qhd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd  ,
    						''' AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)' )
    
    declare @sql4 nvarchar(max)= concat(' Union All
    							-- solan mua hang
    						Select 
    							hd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    								0 as TienChi,
    							COUNT(*) AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu
    						From BH_HoaDon hd 
    						where hd.LoaiHoaDon in (1,19,25)
    						and hd.ChoThanhToan = 0
    						AND hd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd ,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    							 GROUP BY hd.ID_DoiTuong  	
    							)AS tblThuChi
    						GROUP BY tblThuChi.ID_DoiTuong
    				) a on dt.ID = a.ID_DoiTuong  					
    						WHERE  dt.loaidoituong =', @loaiKH  ,					
    						' and dt.NgayTao between ''',@timeStartKH, ''' and ''',@timeEndKH,
    							''' and ( MaDoiTuong like ''%', @MaKH, '%'' OR  TenDoiTuong like ''%',@MaKH, '%'' or TenDoiTuong_KhongDau like ''%',@MaKH, '%'' or DienThoai like ''%',@MaKH, '%'' or Email like ''%',@MaKH, '%'' )',
    						
    						  '', @Where, ')b
    				 where b.ID not like ''%00000000-0000-0000-0000-0000%''
    				 and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    				  ),
    			Count_CTE ',
    			' AS ',
    			' ( ',
    			' SELECT COUNT(*) AS TotalRow, CEILING(COUNT(*) / CAST(',@PageSize, ' as float )) as TotalPage ,
					SUM(TongBan) as TongBanAll,
					SUM(TongBanTruTraHang) as TongBanTruTraHangAll,
					SUM(TongTichDiem) as TongTichDiemAll,
					SUM(NoHienTai) as NoHienTaiAll,
					SUM(PhiDichVu) as TongPhiDichVu
					FROM Data_CTE ',
    			' ) ',
    			' SELECT dt.*, cte.TotalPage, cte.TotalRow, 
					cte.TongBanAll, 
					cte.TongBanTruTraHangAll,
					cte.TongTichDiemAll,
					cte.NoHienTaiAll,
					cte.TongPhiDichVu,
    				  ISNULL(trangthai.TenTrangThai,'''') as TrangThaiKhachHang,
    				  ISNULL(qh.TenQuanHuyen,'''') as PhuongXa,
    				  ISNULL(tt.TenTinhThanh,'''') as KhuVuc,
    				  ISNULL(dv.TenDonVi,'''') as ConTy,
    				  ISNULL(dv.SoDienThoai,'''') as DienThoaiChiNhanh,
    				  ISNULL(dv.DiaChi,'''') as DiaChiChiNhanh,
    				  ISNULL(nk.TenNguonKhach,'''') as TenNguonKhach,
    				  ISNULL(dt2.TenDoiTuong,'''') as NguoiGioiThieu,
					  ISNULL(nvpt.MaNhanVien,'''') as MaNVPhuTrach,
					 ISNULL(nvpt.TenNhanVien,'''') as TenNhanVienPhuTrach',
    			' FROM Data_CTE dt',
    			' CROSS JOIN Count_CTE cte',
    			' LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID ',
    		' LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID ',
    			' LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID ',
    			' LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID ',
				' LEFT join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID ',
    		' LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID ',
    			' LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID ',
    			' ORDER BY ',@SortBy,
    			' OFFSET (', @CurrentPage, ' * ', @PageSize ,') ROWS ',
    			' FETCH NEXT ', @PageSize , ' ROWS ONLY ')
    
    			
    			exec (@sql1 + @sql2 + @sql3 + @sql4)
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_afterUseAndTra]
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

--	use SSOFT_SPA12112018
--DECLARE @IDChiNhanhs nvarchar(max) = null,
--	@IDNhanViens nvarchar(max) = null,
--	@DateFrom datetime = null,
--	@DateTo datetime = null,
--	@TextSearch nvarchar(max) = null,
--	@CurrentPage int =null,
--	@PageSize int = null;

--	SET @IDChiNhanhs = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de';
--	SET @PageSize = '10';
--	SET @CurrentPage = 0;
--	SET @TextSearch = '';

	DECLARE @tblNhanVien TABLE (ID UNIQUEIDENTIFIER, TenNhanVien NVARCHAR(MAX));
	IF(ISNULL(@IDNhanViens, '') != '' )
	BEGIN
		INSERT INTO @tblNhanVien
		SELECT nv.ID, nv.TenNhanVien FROM NS_NhanVien nv
		INNER JOIN (SELECT name from dbo.splitstring(@IDNhanViens)) a ON a.Name = nv.ID;
	END
	ELSE
	BEGIN
		INSERT INTO @tblNhanVien
		SELECT ID, TenNhanVien FROM NS_NhanVien;
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max))
	DECLARE @count int
	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!=''
	select @count =  (Select count(*) from @tblSearch)

	DECLARE @VDateFrom DATETIME;
	IF(ISNULL(@DateFrom, '') != '')
	BEGIN
		SET @VDateFrom = @DateFrom;
	END
	ELSE
	BEGIN
		SET @VDateFrom = '1999-01-01';
	END
	DECLARE @VDateTo DATETIME;
	IF(ISNULL(@DateTo, '') != '')
	BEGIN
		SET @VDateTo = @DateTo;
	END
	ELSE
	BEGIN
		SET @VDateTo = DATEADD(DAY, 10, GETDATE());
	END

	DECLARE @tblHoaDonGoiDichVu TABLE(ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), ID_HoaDon UNIQUEIDENTIFIER, ID_BangGia UNIQUEIDENTIFIER,
	ID_NhanVien UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, NguoiTao NVARCHAR(MAX), ID_Xe UNIQUEIDENTIFIER, DienGiai NVARCHAR(MAX), NgayLapHoaDon DATETIME, TenNhanVien NVARCHAR(MAX), 
	ID_DoiTuong UNIQUEIDENTIFIER, TenDoiTuong NVARCHAR(MAX), MaDoiTuong NVARCHAR(MAX), BienSo NVARCHAR(MAX), PhaiThanhToan FLOAT, TongTienHang FLOAT, TongGiamGia FLOAT, DiemGiaoDich FLOAT,
	LoaiHoaDon INT, TongTienThue FLOAT, TongThueKhachHang FLOAT)

	INSERT INTO @tblHoaDonGoiDichVu
	SELECT 
	hd.ID, 
	hd.MaHoaDon,
	hd.ID_HoaDon,
	hd.ID_BangGia,
	hd.ID_NhanVien,
	hd.ID_DonVi,
	hd.NguoiTao,
	hd.ID_Xe,
	hd.DienGiai,
	hd.NgayLapHoaDon,
	ISNULL(nv.TenNhanVien,'' ) as TenNhanVien,
	hd.ID_DoiTuong,
	ISNULL(dt.TenDoiTuong,N'Khách lẻ' ) as TenDoiTuong,
	ISNULL(dt.MaDoiTuong,'kl' ) as MaDoiTuong,
	xe.BienSo,
	ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
	ISNULL(hd.TongTienHang, 0) as TongTienHang,
	ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
	ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich,
	hd.LoaiHoaDon,
	ISNULL(hd.TongTienThue, 0) as TongTienThue ,
	ISNULL(hd.TongThueKhachHang, 0) as TongThueKhachHang
	FROM BH_HoaDon hd
	INNER JOIN (SELECT name FROM dbo.splitstring(ISNULL(@IDChiNhanhs, ''))) dv ON dv.Name = hd.ID_DonVi
	LEFT JOIN @tblNhanVien nv ON hd.ID_NhanVien = nv.ID
	LEFT JOIN Gara_DanhMucXe xe ON xe.ID = hd.ID_Xe
	LEFT JOIN DM_DoiTuong dt ON hd.ID_DoiTuong = dt.ID
	WHERE hd.LoaiHoaDon = 19 and hd.ChoThanhToan = 0 AND hd.NgayLapHoaDon BETWEEN @VDateFrom AND @VDateTo
	and 
	((select count(Name) from @tblSearch b where     			
	hd.MaHoaDon like '%'+b.Name+'%'
	or hd.NguoiTao like '%'+b.Name+'%'
	or xe.BienSo like '%'+b.Name+'%'	
	or dt.MaDoiTuong like '%'+b.Name+'%'		
	or dt.TenDoiTuong like '%'+b.Name+'%'
	or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
	or dt.DienThoai like '%'+b.Name+'%'		
	)=@count or @count=0)

	-- Hóa đơn mua
	DECLARE @tblTonGoiDichVuChiTiet TABLE(IDGoiDV UNIQUEIDENTIFIER, IDDonViQuiDoi UNIQUEIDENTIFIER, IDChiTietGoiDichVu UNIQUEIDENTIFIER, SoLuongTon FLOAT);
	INSERT INTO @tblTonGoiDichVuChiTiet
	select 
		hd.ID as ID_GoiDV,
		hdct.ID_DonViQuiDoi,
		hdct.ID,
		hdct.SoLuong
	from @tblHoaDonGoiDichVu hd
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	where hdct.ID_ChiTietDinhLuong = hdct.ID  OR hdct.ID_ChiTietDinhLuong is null;

	UPDATE tdv
	SET tdv.SoLuongTon = tdv.SoLuongTon - hdsd.SoLuongSuDung
	FROM @tblTonGoiDichVuChiTiet tdv
	INNER JOIN
	(select ct2.ID_ChiTietGoiDV, SUM(ct2.SoLuong) AS SoLuongSuDung	
	from BH_HoaDon_ChiTiet ct2 
	join BH_HoaDon hd2 on ct2.ID_HoaDon =hd2.ID
	where hd2.ChoThanhToan= 0 and hd2.LoaiHoaDon = 1  and (ct2.ID_ChiTietDinhLuong is null or ct2.ID_ChiTietDinhLuong =ct2.ID)
	GROUP BY ct2.ID_ChiTietGoiDV
	) hdsd
	ON hdsd.ID_ChiTietGoiDV = tdv.IDChiTietGoiDichVu;

	DECLARE @tblTonGoiDichVu TABLE(IDGoiDV UNIQUEIDENTIFIER, SoLuongTon FLOAT);
	INSERT INTO @tblTonGoiDichVu
	SELECT IDGoiDV, SUM(SoLuongTon) FROM @tblTonGoiDichVuChiTiet
	GROUP BY IDGoiDV;
	-- Hóa đơn trả
	UPDATE tdv
	SET tdv.SoLuongTon = tdv.SoLuongTon - hdt.SoLuongTra
	FROM @tblTonGoiDichVu tdv
	INNER JOIN
	(select 
		hd3.ID_HoaDon as ID_HoaDonGoc, 
		SUM(ISNULL(ct3.SoLuong, 0)) AS SoLuongTra
	from BH_HoaDon_ChiTiet ct3
	INNER join BH_HoaDon hd3 on ct3.ID_HoaDon = hd3.ID 
	where hd3.ChoThanhToan =0 AND hd3.ID_HoaDon IS NOT NULL
	group by hd3.ID_HoaDon) hdt ON hdt.ID_HoaDonGoc = tdv.IDGoiDV

	SELECT R.ID, R.MaHoaDon, R.ID_HoaDon, R.ID_BangGia, R.ID_NhanVien, R.ID_DonVi, R.NguoiTao, R.ID_Xe,
	R.DienGiai, R.NgayLapHoaDon, R.TenNhanVien, R.ID_DoiTuong, R.TenDoiTuong, R.MaDoiTuong,
	R.BienSo, R.PhaiThanhToan, R.TongTienHang, R.TongGiamGia, R.DiemGiaoDich, R.LoaiHoaDon, R.TongTienThue, R.TongThueKhachHang,
	R.TotalRow, R.SoLuongTon AS SoLuongConLai FROM
	(SELECT row_number() over (order by gdv.NgayLapHoaDon desc) as Rn,
				COUNT(gdv.ID) OVER () as TotalRow, * FROM @tblHoaDonGoiDichVu gdv
	INNER JOIN @tblTonGoiDichVu tdv ON gdv.ID = tdv.IDGoiDV
	WHERE tdv.SoLuongTon > 0) R
	WHERE R.Rn BETWEEN (@CurrentPage * @PageSize) + 1 AND @PageSize * (@CurrentPage + 1);
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_TongQuanThuChi]
    @ID_ChiNhanh [uniqueidentifier],
    @timeStart [datetime],
    @timeEnd [datetime]
AS
BEGIN

SELECT
CAST(ROUND(SUM(k.TienThu_Thu), 0) as float) as TienThu_Thu,
CAST(ROUND(SUM(k.TienMat_Thu), 0) as float) as TienMat_Thu,
CAST(ROUND(SUM(k.NganHang_Thu), 0) as float) as NganHang_Thu,
CAST(ROUND(SUM(k.TienThu_Chi), 0) as float) as TienThu_Chi,
CAST(ROUND(SUM(k.TienMat_Chi), 0) as float) as TienMat_Chi,
CAST(ROUND(SUM(k.NganHang_Chi), 0) as float) as NganHang_Chi,
CAST(ROUND(SUM(k.ThuNo_Tong), 0) as float) as ThuNo_Tong,
CAST(ROUND(SUM(k.ThuNo_Mat), 0) as float) as ThuNo_Mat,
CAST(ROUND(SUM(k.ThuNo_NganHang), 0) as float) as ThuNo_NganHang
FROM
(
    Select 
	0 as TienThu_Chi,
	0 as TienMat_Chi,
	0 as NganHang_Chi,
	sum(qct.TienThu) as TienThu_Thu,
	SUM(iif(qct.HinhThucThanhToan=1, qct.TienThu,0)) as TienMat_Thu, 
	SUM(iif(qct.HinhThucThanhToan=2 or qct.HinhThucThanhToan =3 , qct.TienThu,0)) as NganHang_Thu,
	--SUM(TienMat + TienGui) as TienThu_Thu, SUM(TienMat) as TienMat_Thu, Sum(TienGui) as NganHang_Thu,
	0 as ThuNo_Tong,
	0 as ThuNo_Mat,
	0 as ThuNo_NganHang
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
	where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
	and qhd.ID_DonVi = @ID_ChiNhanh
	and qhd.LoaiHoaDon = 11
	and (qhd.PhieuDieuChinhCongNo != 1 or qhd.PhieuDieuChinhCongNo is null)
	and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
	and qct.HinhThucThanhToan not in (4,5,6)

	Union all -- tiền chi
	select 
	sum(qct.TienThu) as TienThu_Chi,
	SUM(iif(qct.HinhThucThanhToan=1, qct.TienThu,0)) as TienMat_Chi, 
	SUM(iif(qct.HinhThucThanhToan=2 or qct.HinhThucThanhToan =3 , qct.TienThu,0)) as TienThu_Chi,
	0 as TienThu_Thu,
	0 as TienMat_Thu,
	0 as NganHang_Thu,
	0,
	0,
	0
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
	where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
	and qhd.ID_DonVi = @ID_ChiNhanh
	and qhd.LoaiHoaDon = 12
	and (qhd.PhieuDieuChinhCongNo != 1  OR qhd.PhieuDieuChinhCongNo is null)
	and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
	and qct.HinhThucThanhToan not in (4,5,6)

	Union all -- thu congno khach
	select 
	0,
	0,
	0,	
	0 as TienThu_Thu,
	0 as TienMat_Thu,
	0 as NganHang_Thu,
	sum(qct.TienThu) as ThuNo_Tong,
	SUM(iif(qct.HinhThucThanhToan=1, qct.TienThu,0)) as ThuNo_Mat, 
	SUM(iif(qct.HinhThucThanhToan=2 or qct.HinhThucThanhToan =3 , qct.TienThu,0)) as ThuNo_NganHang
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
	join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
	where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
	and qhd.ID_DonVi = @ID_ChiNhanh
	and qhd.LoaiHoaDon = 11
	and (qhd.PhieuDieuChinhCongNo != 1  OR qhd.PhieuDieuChinhCongNo is null)
	and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
	and hd.LoaiHoaDon in (1,19)
	and qct.HinhThucThanhToan not in (4,5,6)
	and hd.ChoThanhToan= 0 
	and hd.NgayLapHoaDon < @timeStart
) as k
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetNoKhachHang_byDate]
    @ID_DoiTuong [uniqueidentifier],
    @ToDate [nvarchar](20) ,
	@IDChiNhanhs nvarchar(max) = null
AS
BEGIN

    set nocount on

	declare @LoaiDoiTuong int= (select top 1 LoaiDoiTuong from DM_DoiTuong where ID = @ID_DoiTuong)

	declare @tblChiNhanh table (ID uniqueidentifier)
	declare @tblHDBan table (ID_DoiTuong uniqueidentifier, DoanhThu float)

	if ISNULL(@IDChiNhanhs,'')=''
		begin
			---- all chinhanh
			insert into @tblChiNhanh
			select ID from DM_DonVi where TrangThai is null or TrangThai= 1
		end
	else
		begin
			insert into @tblChiNhanh
			select name from dbo.splitstring(@IDChiNhanhs)
		end

	if @LoaiDoiTuong= 3
	begin
			insert into @tblHDBan
			SELECT 
    			bhd.ID_BaoHiem,							   					
				sum(isnull(bhd.PhaiThanhToanBaoHiem,0)) AS DoanhThu   									
    		FROM BH_HoaDon bhd
			join @tblChiNhanh cn on bhd.ID_DonVi= cn.ID					
    		WHERE bhd.ID_BaoHiem = @ID_DoiTuong
    			AND bhd.LoaiHoaDon in (1,7,19,22,25) 
				and bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon < @ToDate
    		GROUP BY bhd.ID_BaoHiem
	end
	else
		begin
			insert into @tblHDBan
			SELECT 
    			bhd.ID_DoiTuong,							   					
				sum(isnull(bhd.PhaiThanhToan,0)) AS DoanhThu   					
    		FROM BH_HoaDon bhd
			join @tblChiNhanh cn on bhd.ID_DonVi= cn.ID					
    		WHERE bhd.ID_DoiTuong = @ID_DoiTuong
    			AND bhd.LoaiHoaDon in (1,7,19,22,25) 
				and bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon < @ToDate
    		GROUP BY bhd.ID_DoiTuong
		end

    	select dt.ID, dt.MaDoiTuong, dt.TenDoiTuong,
    		ISNULL(tbl.NoHienTai,0) as NoHienTai
    	from     	
    		(
    			SELECT
    			td.ID_DoiTuong ,
    			SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) 
				- SUM(isnull(td.PhiDichVu,0))  - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
    			SUM(ISNULL(td.DoanhThu,0))- SUM(ISNULL(td.GiaTriTra,0)) AS TongBanTruTraHang,
    			SUM(ISNULL(td.DoanhThu,0)) AS TongMua
    
    			FROM
    			(
				---- chiphi DV ngoai
					select 
							cp.ID_NhaCungCap as ID_DoiTuong,
							0 as GiaTriTra,
    						0 as DoanhThu,
							0 AS TienThu,
    						0 AS TienChi,    					
							sum(cp.ThanhTien) as PhiDichVu
						from BH_HoaDon_ChiPhi cp
						join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
						join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
						where hd.ChoThanhToan = 0
						and cp.ID_NhaCungCap = @ID_DoiTuong
						and hd.NgayLapHoaDon < @ToDate
						group by cp.ID_NhaCungCap
						union all
				
				---- tongban
    				SELECT 
    					ID_DoiTuong,							
    					0 AS GiaTriTra,
    					DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as PhiDichVu
    				FROM @tblHDBan
    				
    				UNION All

					-- tongtra
    				SELECT bhd.ID_DoiTuong,						
    					SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as PhiDichVu
    				FROM BH_HoaDon bhd
					join @tblChiNhanh cn on bhd.ID_DonVi= cn.ID
    				WHERE bhd.ID_DoiTuong= @ID_DoiTuong
						AND bhd.LoaiHoaDon in (4,6) 
						and bhd.ChoThanhToan = 0    					
    					AND bhd.NgayLapHoaDon < @ToDate
    				GROUP BY bhd.ID_DoiTuong
    					
    				UNION ALL

					-- tienthu
    				SELECT 
    					qhdct.ID_DoiTuong,							
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    					0 AS TienChi,
						0 as PhiDichVu
    				FROM Quy_HoaDon qhd
					join @tblChiNhanh cn on qhd.ID_DonVi= cn.ID
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon    				
    				WHERE qhdct.ID_DoiTuong= @ID_DoiTuong
    					AND qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != 0 OR qhd.TrangThai is null)
    					AND qhd.NgayLapHoaDon < @ToDate
    					and qhdct.HinhThucThanhToan!=6
						AND (qhdct.LoaiThanhToan is null OR qhdct.LoaiThanhToan !=3)
    				GROUP BY qhdct.ID_DoiTuong
    
    				UNION ALL

					----tienchi
    				SELECT 
    					qhdct.ID_DoiTuong,							
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					SUM(ISNULL(qhdct.TienThu,0)) AS TienChi,
						0 as PhiDichVu
    				FROM Quy_HoaDon qhd
					join @tblChiNhanh cn on qhd.ID_DonVi= cn.ID
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				WHERE qhdct.ID_DoiTuong= @ID_DoiTuong
    					AND qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != 0 OR qhd.TrangThai is null)
    					AND qhd.NgayLapHoaDon < @ToDate
						AND (qhdct.LoaiThanhToan is null OR qhdct.LoaiThanhToan !=3)
    				GROUP BY qhdct.ID_DoiTuong	
				
    		) AS td GROUP BY td.ID_DoiTuong
			
    	) tbl
		join DM_DoiTuong dt on tbl.ID_DoiTuong = dt.ID
END");

			Sql(@"ALTER PROCEDURE [dbo].[TheGiaTri_GetLichSuNapTien]
    @IDChiNhanhs [nvarchar](max),
    @ID_Cutomer [nvarchar](max),
    @TextSearch [nvarchar](max) = null,
    @DateFrom [nvarchar](max) = null,
    @DateTo [nvarchar](max) = null,
    @CurrentPage [int] = null,
    @PageSize [int] = null
AS
BEGIN
    SET NOCOUNT ON;
    	
    	declare @paramIn nvarchar(max)=' declare @isNull_txtSearch int = 1 '
    
    	declare @tblDefined nvarchar(max)='', @sql1 nvarchar(max) ='',  @sql2 nvarchar(max) ='',
    	@whereIn nvarchar(max)='', @whereOut nvarchar(max)='',
    	
    	@paramDefined nvarchar(max)= N'
    			@IDChiNhanhs_In [nvarchar](max) ,
    			@ID_Cutomer_In [nvarchar](max),
    			@TextSearch_In [nvarchar](max),
    			@DateFrom_In [datetime],
    			@DateTo_In [datetime],		
    			@CurrentPage_In [int],
    			@PageSize_In [int]
    			 '
    		set @whereIn = ' where 1 = 1 and hd.LoaiHoaDon in (22,23) and hd.ChoThanhToan = 0'
    		set @whereOut = ' where 1 = 1'
    
    		if isnull(@CurrentPage,'') ='' set @CurrentPage = 0
    		if isnull(@PageSize,'') ='' set @PageSize = 20
    
    		if isnull(@IDChiNhanhs,'')!=''
    			begin
    				set @tblDefined = concat(@tblDefined, N' declare @tblChiNhanh table (ID uniqueidentifier)
    					insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ')
    				set @whereIn= CONCAT(@whereIn, ' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)')
    			end
    
    		if isnull(@ID_Cutomer,'')!=''
    		begin
    			set @whereIn= CONCAT(@whereIn, ' and hd.ID_DoiTuong = @ID_Cutomer_In')
    		end
    
    	   if isnull(@DateFrom,'')!=''
    		begin
    			set @whereIn= CONCAT(@whereIn, ' and hd.NgayLapHoaDon >= @DateFrom_In')
    		end
    
    		if isnull(@DateTo,'')!=''
    		begin
    			set @whereIn= CONCAT(@whereIn, ' and hd.NgayLapHoaDon < @DateTo_In')
    		end
    
    		if isnull(@TextSearch,'')!='' and isnull(@TextSearch,'')!='%%'
    			begin			
    				set @paramIn = CONCAT(@paramIn, ' set @isNull_txtSearch = 0')
    				set @TextSearch = CONCAT(N'%', @TextSearch, '%')
    				set @whereOut= CONCAT(@whereOut, ' 
    					and (MaHoaDon like @TextSearch_In
    						OR MaDoiTuong like @TextSearch_In
    						OR TenDoiTuong like @TextSearch_In
    						OR TenDoiTuong_KhongDau like @TextSearch_In
    						OR MaHoaDon like @TextSearch_In
    						OR DienGiai like @TextSearch_In
    						OR DienGiaiUnSign like @TextSearch_In)'
    					)					
    			end
    
    			set @sql1 = concat(N'
    
    			select hd.ID,
    				hd.ID_DoiTuong,			
					hd.MaHoaDon,
					hd.LoaiHoaDon,
					hd.NgayLapHoaDon,
					hd.TongChiPhi,
					hd.TongChietKhau,
					hd.TongTienHang,
					hd.TongTienThue,
					hd.TongGiamGia,
					hd.PhaiThanhToan,     
					hd.TongTienHang as SoDuSauNap,
					hd.DienGiai
    				into #htThe
    			from BH_HoaDon hd ', @whereIn)
    			   
    		
    			set @sql2 = concat(N'
				---- get luyke soduSauNap
				declare @soduLuyke float = 0
				declare @ID_HoaDon uniqueidentifier, @SoDuSauNap float
				declare _cur cursor for

				select ID, SoDuSauNap
				from #htThe order by NgayLapHoaDon 
	
				open _cur
				FETCH NEXT FROM _cur
				INTO @ID_HoaDon, @SoDuSauNap
				WHILE @@FETCH_STATUS = 0
				BEGIN   
					 set @soduLuyke = @soduLuyke + @SoDuSauNap
					 update #htThe set SoDuSauNap= @soduLuyke where ID= @ID_HoaDon		
					FETCH NEXT FROM _cur

				INTO @ID_HoaDon, @SoDuSauNap

				END
				CLOSE _cur;
				DEALLOCATE _cur;

    	
    			select tbl.*, 
    				dt.MaDoiTuong as MaKhachHang,
    				dt.TenDoiTuong as TenKhachHang
    				from
    				(
    				select hd.ID,
    					hd.ID_DoiTuong,
    					hd.LoaiHoaDon,
    					hd.MaHoaDon,
    					hd.NgayLapHoaDon,
    					hd.TongChiPhi as MucNap,
    					hd.TongChietKhau as KhuyenMaiVND,
    					hd.TongTienHang as TongTienNap,
    					hd.SoDuSauNap,
    					hd.TongGiamGia as ChietKhauVND,
    					hd.PhaiThanhToan,
    					hd.DienGiai,
    					iif(@isNull_txtSearch =0, dbo.FUNC_ConvertStringToUnsign(hd.DienGiai), hd.DienGiai ) as DienGiaiUnSign,
    					isnull(thu.KhachDaTra,0) as KhachDaTra
    				from #htThe hd
    				left join
    				(
    					select 
    						qct.ID_HoaDonLienQuan,
    						sum(qct.TienThu) as KhachDaTra
    					from Quy_HoaDon_ChiTiet qct
    					join #htThe hd on qct.ID_HoaDonLienQuan = hd.ID
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					where qhd.TrangThai= 1 or qhd.TrangThai= 1
    					group by qct.ID_HoaDonLienQuan
    				) thu
    				on hd.ID = thu.ID_HoaDonLienQuan
    			) tbl 
    			join DM_DoiTuong dt on tbl.ID_DoiTuong = dt.ID
    			', @whereOut , ' order by tbl.NgayLapHoaDon desc')
    
    		set @sql2= CONCAT(@paramIn, '; ', @tblDefined, @sql1,'; ', @sql2)		 
    		
		print @sql2
    		exec sp_executesql @sql2, 
    			@paramDefined,
    			@IDChiNhanhs_In= @IDChiNhanhs,
    			@ID_Cutomer_In = @ID_Cutomer,
    			@TextSearch_In = @TextSearch,
    			@DateFrom_In = @DateFrom,
    			@DateTo_In = @DateTo,			
    			@CurrentPage_In = @CurrentPage,
    			@PageSize_In = @PageSize
END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN]
    @ID_PhieuTiepNhan [uniqueidentifier],
    @ChenhLech_SoKM [float] --- not use
AS
BEGIN
    SET NOCOUNT ON;
		
	declare @ID_HoaDon uniqueidentifier
	declare _cur cursor for

	select distinct lich.ID_HoaDon
	from Gara_LichBaoDuong lich 
	join BH_HoaDon hd on lich.ID_HoaDon= hd.ID
    where hd.ID_PhieuTiepNhan= @ID_PhieuTiepNhan
    and lich.SoKmBaoDuong > 0
    and lich.TrangThai= 1


	open _cur
	FETCH NEXT FROM _cur
	INTO @ID_HoaDon
	WHILE @@FETCH_STATUS = 0
	BEGIN   
		 exec Insert_LichNhacBaoDuong @ID_HoaDon
		FETCH NEXT FROM _cur

	INTO @ID_HoaDon

	END
	CLOSE _cur;
	DEALLOCATE _cur;
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateTonKho_multipleDVT]
    @isUpdate [int],
    @ID_DonVi [uniqueidentifier],
    @ID_DonViQuyDoi [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @TonKho float
AS
BEGIN
    SET NOCOUNT ON;
    
    	if @isUpdate = 2
    	begin
    		---- get infor donviquidoi by ID
    		declare @ID_HangHoa uniqueidentifier, @TyLeChuyenDoi float, @LaDonViChuan bit
    		select @ID_HangHoa = ID_HangHoa, 
    			@TyLeChuyenDoi= iif(TyLeChuyenDoi is null or TyLeChuyenDoi =0,1, TyLeChuyenDoi), 
    			@LaDonViChuan= LaDonViChuan
    		from DonViQuiDoi where id= @ID_DonViQuyDoi
    
    		--- get infor hanghoa
    		declare @QuanLyTheoLo bit, @LaHangHoa bit
    		select @QuanLyTheoLo = QuanLyTheoLoHang, @LaHangHoa = LaHangHoa from DM_HangHoa where ID = @ID_HangHoa
    
    		---- get all dvt
    		select ID, iif(TyLeChuyenDoi is null or TyLeChuyenDoi =0,1, TyLeChuyenDoi) as TyLeChuyenDoi,LaDonViChuan
    		into #allDVT
    		from DonViQuiDoi where ID_HangHoa = @ID_HangHoa
    
    		if @LaHangHoa='0'
    			begin
    			--- reset tonkho if change hanghoa --> dichvu
    				update tk  set TonKho = 0 
    				from DM_HangHoa_TonKho tk
    				where exists (select ID from #allDVT qd where tk.ID_DonViQuyDoi = qd.ID)
    			end
    		else
    		begin
    			declare @cur_ID_DonViQuiDoi uniqueidentifier, @cur_TyLeChuyenDoi float
    			if @LaDonViChuan ='1'
    			begin
    				declare _cur cursor for
    				select ID, TyLeChuyenDoi from #allDVT
    				open _cur
    				fetch next from _cur into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
    				while @@FETCH_STATUS =0
    				begin
    					--- update tonkho for dvt #
    					update DM_HangHoa_TonKho set TonKho = @TonKho / @cur_TyLeChuyenDoi
    					where ID_DonVi = @ID_DonVi and ID_DonViQuyDoi = @cur_ID_DonViQuiDoi
    					and (@QuanLyTheoLo='0' or @QuanLyTheoLo is null or ID_LoHang= @ID_LoHang)
    
    					fetch next from _cur
    					into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
    				end
    				close _cur
    				deallocate _cur
    			end
    			else
    			begin
    				declare @IDQuiDoiChuan uniqueidentifier 
    				select @IDQuiDoiChuan = ID from #allDVT where LaDonViChuan='1'
    
    				declare @tonkhoAll float = @TonKho * @TyLeChuyenDoi 
    				---- update tonkho for dvt chuan
    				update DM_HangHoa_TonKho set TonKho = @tonkhoAll
    				where ID_DonVi = @ID_DonVi and ID_DonViQuyDoi = @IDQuiDoiChuan
    					and (@QuanLyTheoLo='0' or @QuanLyTheoLo is null or ID_LoHang= @ID_LoHang)
    
    				---- update tokho for dvt # (vd: hang co 3 dvt tro len)
    				declare _cur cursor for
    				select ID, TyLeChuyenDoi from #allDVT where ID != @IDQuiDoiChuan and ID != @ID_DonViQuyDoi
    				open _cur
    				fetch next from _cur into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
    				while @@FETCH_STATUS =0
    				begin					
    					update DM_HangHoa_TonKho set TonKho = @tonkhoAll / @cur_TyLeChuyenDoi
    					where ID_DonVi = @ID_DonVi and ID_DonViQuyDoi = @cur_ID_DonViQuiDoi
    					and (@QuanLyTheoLo='0' or @QuanLyTheoLo is null or ID_LoHang= @ID_LoHang)
    
    					fetch next from _cur
    					into @cur_ID_DonViQuiDoi, @cur_TyLeChuyenDoi
    				end
    				close _cur
    				deallocate _cur
    			end		
    		end					
    	end
END");

            Sql(@"ALTER PROCEDURE [dbo].[ValueCard_GetListHisUsed]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_KhachHang [nvarchar](max),
    @DateFrom datetime,
    @DateTo datetime,
	@Currentpage int,
	@PageSize int
AS
BEGIN
    SET NOCOUNT ON;
	set @DateTo = DATEADD(day, 1, @DateTo)

    DECLARE @TblHisCard TABLE(
    				STT INT, ID UNIQUEIDENTIFIER, ID_DoiTuong UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(50),TenDoiTuong NVARCHAR(500),
					MaHoaDon NVARCHAR(500),  SLoaiHoaDon nvarchar(max), 
    				MaHoaDonSQ NVARCHAR(MAX),LoaiHoaDonSQ INT, 
					NoiDungThu nvarchar(max),
					NgayLapHoaDon DATETIME, TienThe FLOAT, ThuChiThe FLOAT, SoDuTruoc FLOAT, SoDuSau FLOAT, TrangThai_TheGiaTri int)
    	INSERT INTO @TblHisCard
    		SELECT 
    				ROW_NUMBER() OVER(ORDER BY qhd.ID) AS STT,
    				qhd.ID,
    				dt.ID as ID_DoiTuong,-- used to caculator sodutruoc phatsinh
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				MaHoaDons,
    				ISNULL(LoaiHoaDons, N'Loại khác, ') as LoaiHoaDons,
    				qhd.MaHoaDon as MaHoaDonSQ, 
    				qhd.LoaiHoaDon as LoaiHoaDonSQ,
					qhd.NoiDungThu,
    				qhd.NgayLapHoaDon,
    				SUM(ISNULL(qct.TienThu,0)) as TienThe,
    				case when qhd.LoaiHoaDon = 11 then - SUM(ISNULL(qct.TienThu,0)) else SUM(ISNULL(qct.TienThu,0)) end as ThuChiThe,
    				0 as SoDuTruoc,
    				0 as SoDuSau,
    				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
    				else '12' end as TrangThai
    			FROM Quy_HoaDon qhd
    			join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    			join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
    			join (
    				--merger text MaHoaDon to 1 row
    				Select distinct qhdXML.ID, 
    							 (
    						select distinct hd.MaHoaDon +', '  AS [text()]
    						from BH_HoaDon hd
    						join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    						where qct.ID_HoaDon = qhdXML.ID
    						and hd.LoaiHoaDon in (1, 3, 6, 19)
    						and ChoThanhToan ='0'
    						For XML PATH ('')
    					) MaHoaDons
    				from Quy_HoaDon qhdXML
    			) tbl on qhd.ID= tbl.ID
    			
    
    			-- get LoaiHoaDon
    			join (
    				Select distinct qhdXML2.ID, 
    					 (
    					 -- merger text LoaiHoaDon to 1 row
    						select distinct 
    							 tbl1.SLoaiHoaDon +', '  AS [text()]
    						from 
    							(
    							-- get text HoaDon by LoaiHoaDon
    							select 
    								case hd.LoaiHoaDon
    									when 1 then N'Bán hàng'
    									when 3 then N'Đặt hàng'
    									when 6 then N'Trả hàng'
    								else 
    									N'Gói dịch vụ' end as SLoaiHoaDon
    							from BH_HoaDon hd
    							join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    							where hd.LoaiHoaDon in (1, 3, 6, 19)
    							and ChoThanhToan ='0' 
    							and qct.ID_HoaDon = qhdXML2.ID
    							) tbl1
    						For XML PATH ('')
    					) LoaiHoaDons
    				from Quy_HoaDon qhdXML2
    		) tbl2 on qhd.ID= tbl2.ID
    
    	where (qhd.TrangThai = 1 or qhd.TrangThai is null)		
    		and qct.HinhThucThanhToan=4
    	and qhd.NgayLapHoaDon >= @DateFrom 
    	and  qhd.NgayLapHoaDon <= @DateTo
    	--and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    	and dt.ID like @ID_KhachHang
    	GROUP BY qhd.ID,
    		dt.ID,
    		dt.MaDoiTuong,
    		dt.TenDoiTuong,
    		qhd.MaHoaDon,
    		qhd.LoaiHoaDon,
    		qhd.NgayLapHoaDon,
    		dt.TrangThai_TheGiaTri,
    		MaHoaDons,
    		LoaiHoaDons,
			qhd.NoiDungThu
    		    	
    			DECLARE @SoDuTruocPhatSinh INT;
    
    			DECLARE @STT INT;
    			DECLARE @ID UNIQUEIDENTIFIER;
    			DECLARE @ID_DoiTuong UNIQUEIDENTIFIER;
    			DECLARE @MaDoiTuong NVARCHAR(50);
    			DECLARE @TenDoiTuong NVARCHAR(500);
    			DECLARE @MaHoaDon NVARCHAR(500);
    			DECLARE @LoaiHoaDon nvarchar(max);
    			DECLARE @MaHoaDonSQ NVARCHAR(50);
    			DECLARE @LoaiHoaDonSQ INT;
    			DECLARE @NgayLapHoaDon DATETIME;
    			DECLARE @TienThe FLOAT;
    			DECLARE @ThuChiThe FLOAT;
    			DECLARE @SoDuTruoc FLOAT;
    			DECLARE @SoDuSau FLOAT;
    			DECLARE @TrangThai_TheGiaTri INT;
    
    			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR 
				SELECT STT, ID,ID_DoiTuong, MaDoiTuong, TenDoiTuong, MaHoaDon, SLoaiHoaDon, MaHoaDonSQ, LoaiHoaDonSQ, NgayLapHoaDon, TienThe, ThuChiThe, SoDuTruoc, SoDuSau, TrangThai_TheGiaTri
    			FROM @TblHisCard
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT 
			INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe,@ThuChiThe, @SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
    					SET @SoDuTruocPhatSinh = [dbo].[TinhSoDuKHTheoThoiGian](@ID_DoiTuong,@NgayLapHoaDon)
    
    					UPDATE @TblHisCard SET SoDuTruoc= @SoDuTruocPhatSinh, SoDuSau = @SoDuTruocPhatSinh + @ThuChiThe
    					WHERE STT = @STT
    
    					FETCH NEXT FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe, @ThuChiThe,@SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    
    					
    			END
    		CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT
    
			declare @tongTang float = 0, @tongGiam float =0, @totalRow int =0
			
			select @tongTang = sum(iif(LoaiHoaDonSQ=12,TienThe,0)),
					@tongGiam = sum(iif(LoaiHoaDonSQ=11,TienThe,0)),
					@totalRow = count(id)
			from @TblHisCard

			select *,
				@tongTang as TongPhatSinhTang,
				@tongGiam as TongPhatSinhGiam,
				@totalRow as TotalRow
			from (
    		SELECT 
				row_number() over(order by NgayLapHoaDon desc) as Rn,
				ID, ID_DoiTuong,MaDoiTuong,TenDoiTuong,
    			LEFT(MaHoaDon, LEN(MaHoaDon) - 1) as MaHoaDon,
    			LEFT(SLoaiHoaDon, LEN(SLoaiHoaDon) - 1) as SLoaiHoaDon,		
				NoiDungThu as DienGiai,
    			MaHoaDonSQ, LoaiHoaDonSQ,NgayLapHoaDon,TienThe,SoDuTruoc, SoDuSau, TrangThai_TheGiaTri				
    		FROM @TblHisCard 
    		WHERE TrangThai_TheGiaTri like '%11%' --11.DangHoatDong
    		) b where b.Rn between (@CurrentPage * @PageSize) + 1 and @PageSize * (@CurrentPage + 1)
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateLichBD_whenChangeNgayLapHD]
    @ID_HoaDon [uniqueidentifier],
    @NgayLapHDOld [datetime],
    @NgayLapNew [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    declare @chenhlech int= DATEDIFF(day, @NgayLapHDOld, @NgayLapNew)
    	if @chenhlech!=0
    		begin
    			
    			--- update if lich exist
    			update lich set lich.NgayBaoDuongDuKien= DATEADD(day, @chenhlech,lich.NgayBaoDuongDuKien)			
    			from Gara_LichBaoDuong lich
    			where lich.ID_HoaDon= @ID_HoaDon
    			and lich.TrangThai = 1
    			    
    		end
END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetHangHoaDatLichChiTiet]");
            DropStoredProcedure("[dbo].[GetListDatLich]");
            DropStoredProcedure("[dbo].[GetListHangHoaDatLichCheckin]");
            DropStoredProcedure("[dbo].[GetListNhanVienDatLichCheckin]");
            DropStoredProcedure("[dbo].[GetNhanVienDatLichChiTiet]");
        }
    }
}
