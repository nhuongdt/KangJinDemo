namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181018 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetList_GoiDichVu_Where]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                maHD = p.String()
            }, body: @"SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.ID_ViTri,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
    	c.TheoDoi,
    	c.ID_DonVi,
    	c.ID_KhuyenMai,
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.MaHoaDonGoc,
    	c.TongTienHDTra,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
    	c.NguoiTaoHD,
    	c.DiaChiKhachHang,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TenPhongBan,
    	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan, c.ThuTuThe, c.TienMat, c.ChuyenKhoan, c.KhachDaTra,c.TongChietKhau,
    	c.TrangThai,
    	c.KhuyenMai_GhiChu,
    	c.KhuyeMai_GiamGia,
    	c.LoaiHoaDonGoc,
    	c.LoaiHoaDon,
    	c.DiaChiChiNhanh,
    	c.DienThoaiChiNhanh,
    	c.DiemGiaoDich,
    	c.DiemSauGD, -- add 02.08.2018 (bind InHoaDon)
    	c.HoaDon_HangHoa, -- string contail all MaHangHoa,TenHangHoa of HoaDon
		CONVERT(nvarchar(10),c.NgayApDungGoiDV,103) as NgayApDungGoiDV,
		CONVERT(nvarchar(10),c.HanSuDungGoiDV,103) as HanSuDungGoiDV
    	FROM
    	(
    		select 
    		a.ID as ID,
    		hdXMLOut.HoaDon_HangHoa,
    		bhhd.ID_DoiTuong,
    			-- Neu theo doi = null --> kiem tra neu la khach le --> theodoi = true, nguoc lai = 1
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,
    		bhhd.ID_HoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DonVi,
    		bhhd.ChoThanhToan,
    		bhhd.ID_KhuyenMai,
    		bhhd.KhuyenMai_GhiChu,
    		bhhd.LoaiHoaDon,
    		ISNULL(bhhd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(bhhd.DiemGiaoDich,0) AS DiemGiaoDich,
    		Case when gb.ID is not null then gb.ID else N'00000000-0000-0000-0000-000000000000' end as ID_BangGia,
    		Case when vt.ID is not null then vt.ID else N'00000000-0000-0000-0000-000000000000' end as ID_ViTri,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		bhhd.MaHoaDon,
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
    		bhhd.NgayLapHoaDon,
			bhhd.NgayApDungGoiDV,
			bhhd.HanSuDungGoiDV,
    		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end as MaDoiTuong,
    		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
    		Case when dt.TenDoiTuong is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
    		Case when dt.TenDoiTuong is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
    		Case when dt.Email is null then N'' else dt.Email end as Email,
    		Case when dt.DienThoai is null then N'' else dt.DienThoai end as DienThoai,
    		Case when dt.DiaChi is null then N'' else dt.DiaChi end as DiaChiKhachHang,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD, --- nhuongdt add 02.08.2018
    		Case when tt.TenTinhThanh is null then tt.TenTinhThanh else N'' end as KhuVuc,
    		Case when qh.TenQuanHuyen is null then qh.TenQuanHuyen else N'' end as PhuongXa,
    		Case when dv.TenDonVi is null then N'' else dv.TenDonVi end as TenDonVi,
    		Case when dv.DiaChi is null then N'' else dv.DiaChi end as DiaChiChiNhanh,
    		Case when dv.SoDienThoai is null then N'' else dv.SoDienThoai end as DienThoaiChiNhanh,
    		Case when nv.TenNhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		bhhd.TongChietKhau,
    		Case when gb.TenGiaBan is null then N'Bảng giá chung' else gb.TenGiaBan end as TenBangGia,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
    		CAST(ROUND(ISNULL(hdt.PhaiThanhToan,0),0) as float) as TongTienHDTra,
    		a.ThuTuThe,
    		a.TienMat,
    		a.ChuyenKhoan,
    		a.KhachDaTra as KhachDaTra,
    		ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
    		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			Select 
    			b.ID,
    			SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
    			SUM(ISNULL(b.TienGui, 0)) as ChuyenKhoan,
    			SUM(ISNULL(b.TienThu, 0)) as KhachDaTra
    			from
    			(
    				Select 
    				bhhd.ID,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienGui, 0) else ISNULL(hdct.TienGui, 0) * (-1) end end as TienGui,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID  
    				where bhhd.LoaiHoaDon = '19' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))     			
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
    			left join 
    				(Select distinct hdXML.ID, 
    					 (
    						select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
    						from BH_HoaDon_ChiTiet ct
    						join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    						join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
    						where ct.ID_HoaDon = hdXML.ID
    						For XML PATH ('')
    					) HoaDon_HangHoa
    				from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
    	WHERE MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD or DienThoai like @maHD or MaDoiTuong like @maHD
    		OR HoaDon_HangHoa like @maHD
    	ORDER BY c.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getList_HoaDonbyNhanVien]", parametersAction: p => new
            {
                ID_NhanVien = p.Guid()
            }, body: @"select hd.ID, nv.ID as ID_NhanVien, hd.MaHoaDon, dt.TenDoiTuong as TenKhachHang, hd.NgayLapHoaDon, hd.TongTienHang 
	from BH_HoaDon hd
	left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
	left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
	where hd.ID_NhanVien = @ID_NhanVien");

            CreateStoredProcedure(name: "[dbo].[getList_HoaHongNhanVien]", parametersAction: p => new
            {
                ID_NhanVien = p.Guid(),
                MaHH = p.String(),
                MaHH_TV = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_DonVi = p.Guid()
            }, body: @"-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	IF(@TrangThai != '')
	BEGIN
		SET NOCOUNT ON;
		DECLARE @tablename TABLE(
		Name [nvarchar](max))
    		DECLARE @tablenameChar TABLE(
		Name [nvarchar](max))
		DECLARE @count int
		DECLARE @countChar int
		INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
		INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    			Select @count =  (Select count(*) from @tablename);
		Select @countChar =   (Select count(*) from @tablenameChar);
		-- Insert statements for procedure here
		SELECT ckmd.ID, ckmd.ID_DonViQuiDoi AS IDQuyDoi, nhh.TenNhomHangHoa, dvqd.MaHangHoa, 
		CASE
			WHEN dvqd.TenDonViTinh = '' OR dvqd.TenDonViTinh IS NULL
				THEN hh.TenHangHoa
			ELSE
				hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')'
		END AS TenHangHoaFull, hh.TenHangHoa, 
		Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end AS ThuocTinh_GiaTri,
		ckmd.ChietKhau,
		ckmd.LaPhanTram AS LaPTChietKhau,
		ckmd.ChietKhau_YeuCau AS YeuCau,
		ckmd.LaPhanTram_YeuCau	AS LaPTYeuCau,
		ckmd.ChietKhau_TuVan AS TuVan,
		ckmd.LaPhanTram_TuVan AS LaPTTuVan,
		dvqd.GiaBan
		 from ChietKhauMacDinh_NhanVien ckmd
		left join NS_NhanVien nv on ckmd.ID_NhanVien = nv.ID
		left join DonViQuiDoi dvqd on dvqd.ID = ckmd.ID_DonViQuiDoi
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_NhomHangHoa nhh on nhh.ID = hh.ID_NhomHang
		left join 
    			(
    				Select Main.id_hanghoa,
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
    			) tt on hh.ID = tt.id_hanghoa
		where nv.ID = @ID_NhanVien AND ckmd.ID_DonVi = @ID_DonVi
		AND ((select TOP 1 [name] from splitstring(@ID_NhomHang) ORDER BY [name]) = '' OR hh.ID_NhomHang = (SELECT * FROM splitstring(@ID_NhomHang) WHERE [name] like hh.ID_NhomHang))
		AND ((select count(*) from @tablename b where 
    			dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    			and ((select count(*) from @tablenameChar c where
    			dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
		AND hh.LaHangHoa LIKE @LaHangHoa AND hh.TheoDoi like @TheoDoi AND dvqd.Xoa like @TrangThai
		ORDER BY ckmd.NgayNhap DESC
	END
	ELSE
	BEGIN
			SET NOCOUNT ON;
		DECLARE @tablename1 TABLE(
		Name [nvarchar](max))
    		DECLARE @tablenameChar1 TABLE(
		Name [nvarchar](max))
		DECLARE @count1 int
		DECLARE @countChar1 int
		INSERT INTO @tablename1(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
		INSERT INTO @tablenameChar1(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    			Select @count1 =  (Select count(*) from @tablename1);
		Select @countChar1 =   (Select count(*) from @tablenameChar1);
		-- Insert statements for procedure here
		SELECT ckmd.ID, ckmd.ID_DonViQuiDoi AS IDQuyDoi, nhh.TenNhomHangHoa, dvqd.MaHangHoa, 
		CASE
			WHEN dvqd.TenDonViTinh = '' OR dvqd.TenDonViTinh IS NULL
				THEN hh.TenHangHoa
			ELSE
				hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')'
		END AS TenHangHoaFull, hh.TenHangHoa, 
		Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end AS ThuocTinh_GiaTri,
		ckmd.ChietKhau,
		ckmd.LaPhanTram AS LaPTChietKhau,
		ckmd.ChietKhau_YeuCau AS YeuCau,
		ckmd.LaPhanTram_YeuCau	AS LaPTYeuCau,
		ckmd.ChietKhau_TuVan AS TuVan,
		ckmd.LaPhanTram_TuVan AS LaPTTuVan,
		dvqd.GiaBan
		 from ChietKhauMacDinh_NhanVien ckmd
		left join NS_NhanVien nv on ckmd.ID_NhanVien = nv.ID
		left join DonViQuiDoi dvqd on dvqd.ID = ckmd.ID_DonViQuiDoi
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_NhomHangHoa nhh on nhh.ID = hh.ID_NhomHang
		left join 
    			(
    				Select Main.id_hanghoa,
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
    			) tt on hh.ID = tt.id_hanghoa
		where nv.ID = @ID_NhanVien AND ckmd.ID_DonVi = @ID_DonVi
		AND ((select TOP 1 [name] from splitstring(@ID_NhomHang) ORDER BY [name]) = '' OR hh.ID_NhomHang = (SELECT * FROM splitstring(@ID_NhomHang) WHERE [name] like hh.ID_NhomHang))
		AND ((select count(*) from @tablename1 b where 
    			dvqd.MaHangHoa like '%'+b.Name+'%' )=@count1 or @count1=0)
    			and ((select count(*) from @tablenameChar1 c where
    			dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar1 or @countChar1=0)
		AND hh.LaHangHoa LIKE @LaHangHoa AND hh.TheoDoi like @TheoDoi AND dvqd.Xoa is null
		ORDER BY ckmd.NgayNhap DESC
	END");

            CreateStoredProcedure(name: "[dbo].[getList_HoaHongNhanVien_Excel]", parametersAction: p => new
            {
                ID_NhanVien = p.Guid(),
                MaHH = p.String(),
                MaHH_TV = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_DonVi = p.Guid()
            }, body: @"IF(@TrangThai != '')
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    DECLARE @count int
    DECLARE @countChar int
    INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    		Select @count =  (Select count(*) from @tablename);
    Select @countChar =   (Select count(*) from @tablenameChar);
    -- Insert statements for procedure here
	SELECT nhh.TenNhomHangHoa, dvqd.MaHangHoa, 
	CASE
		WHEN dvqd.TenDonViTinh = '' OR dvqd.TenDonViTinh IS NULL
			THEN hh.TenHangHoa + Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end
		ELSE
			hh.TenHangHoa +Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end + ' (' + dvqd.TenDonViTinh + ')'
	END AS TenHangHoaFull,
	CASE WHEN ckmd.LaPhanTram = '1' THEN CAST(ckmd.ChietKhau as nvarchar(max)) + '%' ELSE CAST(ckmd.ChietKhau as nvarchar(max)) + 'VND' END as ChietKhau,
	CASE WHEN ckmd.LaPhanTram_TuVan = '1' THEN CAST(ckmd.ChietKhau_TuVan as nvarchar(max)) + '%' ELSE CAST(ckmd.ChietKhau_TuVan as nvarchar(max)) + 'VND' END as TuVan,
	dvqd.GiaBan
	 from ChietKhauMacDinh_NhanVien ckmd
	left join NS_NhanVien nv on ckmd.ID_NhanVien = nv.ID
	left join DonViQuiDoi dvqd on dvqd.ID = ckmd.ID_DonViQuiDoi
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join DM_NhomHangHoa nhh on nhh.ID = hh.ID_NhomHang
	left join 
    		(
    			Select Main.id_hanghoa,
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
    		) tt on hh.ID = tt.id_hanghoa
	where nv.ID = @ID_NhanVien AND ckmd.ID_DonVi = @ID_DonVi
	AND ((select TOP 1 [name] from splitstring(@ID_NhomHang) ORDER BY [name]) = '' OR hh.ID_NhomHang = (SELECT * FROM splitstring(@ID_NhomHang) WHERE [name] like hh.ID_NhomHang))
	AND ((select count(*) from @tablename b where 
    		dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    		and ((select count(*) from @tablenameChar c where
    		dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
	AND hh.LaHangHoa LIKE @LaHangHoa AND hh.TheoDoi like @TheoDoi  AND dvqd.Xoa like @TrangThai
	ORDER BY ckmd.NgayNhap DESC
END
ELSE
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @tablename1 TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar1 TABLE(
    Name [nvarchar](max))
    DECLARE @count1 int
    DECLARE @countChar1 int
    INSERT INTO @tablename1(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    INSERT INTO @tablenameChar1(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    		Select @count1 =  (Select count(*) from @tablename1);
    Select @countChar1 =   (Select count(*) from @tablenameChar1);
    -- Insert statements for procedure here
	SELECT nhh.TenNhomHangHoa, dvqd.MaHangHoa, 
	CASE
		WHEN dvqd.TenDonViTinh = '' OR dvqd.TenDonViTinh IS NULL
			THEN hh.TenHangHoa + Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end
		ELSE
			hh.TenHangHoa +Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end + ' (' + dvqd.TenDonViTinh + ')'
	END AS TenHangHoaFull,
	CASE WHEN ckmd.LaPhanTram = '1' THEN CAST(ckmd.ChietKhau as nvarchar(max)) + '%' ELSE CAST(ckmd.ChietKhau as nvarchar(max)) + 'VND' END as ChietKhau,
	CASE WHEN ckmd.LaPhanTram_TuVan = '1' THEN CAST(ckmd.ChietKhau_TuVan as nvarchar(max)) + '%' ELSE CAST(ckmd.ChietKhau_TuVan as nvarchar(max)) + 'VND' END as TuVan,
	dvqd.GiaBan
	 from ChietKhauMacDinh_NhanVien ckmd
	left join NS_NhanVien nv on ckmd.ID_NhanVien = nv.ID
	left join DonViQuiDoi dvqd on dvqd.ID = ckmd.ID_DonViQuiDoi
	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join DM_NhomHangHoa nhh on nhh.ID = hh.ID_NhomHang
	left join 
    		(
    			Select Main.id_hanghoa,
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
    		) tt on hh.ID = tt.id_hanghoa
	where nv.ID = @ID_NhanVien AND ckmd.ID_DonVi = @ID_DonVi
	AND ((select TOP 1 [name] from splitstring(@ID_NhomHang) ORDER BY [name]) = '' OR hh.ID_NhomHang = (SELECT * FROM splitstring(@ID_NhomHang) WHERE [name] like hh.ID_NhomHang))
	AND ((select count(*) from @tablename1 b where 
    		dvqd.MaHangHoa like '%'+b.Name+'%' )=@count1 or @count1=0)
    		and ((select count(*) from @tablenameChar1 c where
    		dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar1 or @countChar1=0)
	AND hh.LaHangHoa LIKE @LaHangHoa AND hh.TheoDoi like @TheoDoi AND dvqd.Xoa is null
	ORDER BY ckmd.NgayNhap DESC
END");

            CreateStoredProcedure(name: "[dbo].[getListDanhSachHHImport]", parametersAction: p => new
            {
                MaLoHangIP = p.String(),
                MaHangHoaIP = p.String(),
                ID_DonViIP = p.Guid(),
                TimeIP = p.DateTime()
            }, body: @"DECLARE @TableImport TABLE (ID_DonViQuiDoi UNIQUEIDENTIFIER, ID UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, QuanLyTheoLoHang BIT, MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX),
	ThuocTinh_GiaTri NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), TyLeChuyenDoi FLOAT, GiaNhap FLOAT, MaLoHang NVARCHAR(MAX), GiaVon FLOAT, TonKho FLOAT, NgayHetHan DATETIME) INSERT INTO @TableImport
    Select *
    FROM
    (
    select 
    	dvqd.ID as ID_DonViQuiDoi,
    	hh.ID as ID,
    	lh.ID as ID_LoHang,
    	case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa,
    	Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	dvqd.TenDonViTinh,
		dvqd.TyLeChuyenDoi,
		dvqd.GiaNhap,
    	Case when lh.ID is null then '' else lh.MaLoHang end as MaLoHang,
    	Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVon,
		0 as TonKho,
		Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan
    	FROM 
    	DonViQuiDoi dvqd 
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_LoHang lh on lh.ID_HangHoa = hh.ID and lh.MaLoHang = @MaLoHangIP 
    		left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or gv.ID_LoHang is null) and gv.ID_DonVi = @ID_DonViIP)
    	left join 
    		(
    			Select Main.id_hanghoa,
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
    		) tt on hh.ID = tt.id_hanghoa
    	where dvqd.MaHangHoa = @MaHangHoaIP 
    		and dvqd.Xoa is null
    		and hh.TheoDoi = 1 
    			) as p order by NgayHetHan

	DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
	DECLARE @ID UNIQUEIDENTIFIER;
	DECLARE @ID_LoHang UNIQUEIDENTIFIER;
	DECLARE @QuanLyTheoLoHang BIT;
	DECLARE @MaHangHoa NVARCHAR(MAX);
	DECLARE @TenHangHoa NVARCHAR(MAX);
	DECLARE @ThuocTinh_GiaTri NVARCHAR(MAX);
	DECLARE @TenDonViTinh NVARCHAR(MAX);
	DECLARE @TyLeChuyenDoi FLOAT;
	DECLARE @GiaNhap FLOAT;
	DECLARE @MaLoHang NVARCHAR(MAX);
	DECLARE @GiaVon FLOAT;
	DECLARE @TonKho FLOAT;
	DECLARE @NgayHetHan DATETIME;

	 DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @TableImport
    
    OPEN CS_Item 
    FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi, @ID,@ID_LoHang,@QuanLyTheoLoHang, @MaHangHoa,@TenHangHoa, @ThuocTinh_GiaTri,@TenDonViTinh,@TyLeChuyenDoi, @GiaNhap,@MaLoHang,@GiaVon,@TonKho,@NgayHetHan
    WHILE @@FETCH_STATUS = 0
    BEGIN
		DECLARE @TonKhoHienTai FLOAT;
    				SET @TonKhoHienTai = ISNULL([dbo].FUNC_TinhSLTonKhiTaoHD(@ID_DonViIP,@ID,@ID_LoHang,@TimeIP),0)
    				UPDATE @TableImport SET TonKho = @TonKhoHienTai * @TyLeChuyenDoi WHERE ID_DonViQuiDoi = @ID_DonViQuiDoi
		FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi, @ID,@ID_LoHang,@QuanLyTheoLoHang, @MaHangHoa,@TenHangHoa, @ThuocTinh_GiaTri,@TenDonViTinh,@TyLeChuyenDoi, @GiaNhap,@MaLoHang,@GiaVon,@TonKho,@NgayHetHan
    	END
    CLOSE CS_Item
    DEALLOCATE CS_Item

	SELECT * from @TableImport");

            CreateStoredProcedure(name: "[dbo].[insert_ChietKhauMacDinhNhanVien]", parametersAction: p => new
            {
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                MaHH = p.String(),
                ChietKhau = p.Double(),
                LaPTChietKhau = p.Boolean(),
                TuVan = p.Double(),
                LaPTTuVan = p.Boolean(),
                Timezone = p.Int()
            }, body: @"DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
	SELECT @ID_DonViQuiDoi = ID from DonViQuiDoi where MaHangHoa = @MaHH
    insert into ChietKhauMacDinh_NhanVien(ID, ID_NhanVien, ID_DonVi, ChietKhau, LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan, ID_DonViQuiDoi, NgayNhap)
    			values (newID(), @ID_NhanVien, @ID_DonVi, @ChietKhau, @LaPTChietKhau, 0, 0, @TuVan, @LaPTTuVan, @ID_DonViQuiDoi, dateadd(hour,@Timezone,GETUTCDATE()))");

            CreateStoredProcedure(name: "[dbo].[SP_CheckHangHoa_DangKinhDoanh]", parametersAction: p => new
            {
                MaHangHoa = p.String()
            }, body: @"DECLARE @ID_HangHoa nvarchar(max);
	DECLARE @valReturn bit ='0'

	-- check HangHoa exist in DonViQuiDoi and not delete
	SELECT @ID_HangHoa = ID_HangHoa FROM DonViQuiDoi WHERE MaHangHoa like @MaHangHoa and (Xoa = '0'  OR Xoa is null)

	-- check HangHoa đang được theo dõi
	IF @ID_HangHoa IS NULL 
		SET @valReturn= '0'
	ELSE
		BEGIN
			DECLARE @ID nvarchar(max)
			SELECT @ID = ID FROM DM_HangHoa where ID= @ID_HangHoa and (TheoDoi='1' OR TheoDoi is null)
			IF @ID IS NULL SET @valReturn= '0'
			ELSE SET @valReturn= '1'
		END
SELECT @valReturn AS Exist");

            CreateStoredProcedure(name: "[dbo].[SP_CheckHangHoa_QuanLyTheoLo]", parametersAction: p => new
            {
                MaHangHoa = p.String()
            }, body: @"DECLARE @ID_HangHoa nvarchar(max);
	DECLARE @valReturn bit ='0'

	-- check HangHoa exist in DonViQuiDoi and not delete
	SELECT @ID_HangHoa = ID_HangHoa FROM DonViQuiDoi WHERE MaHangHoa like @MaHangHoa and (Xoa = '0'  OR Xoa is null)

	-- check HangHoa co quan ly theo lo khong?
	IF @ID_HangHoa IS NULL 
		SET @valReturn= '0'
	ELSE
		BEGIN
			DECLARE @QuanLyTheoLo bit
			SELECT @QuanLyTheoLo = QuanLyTheoLoHang FROM DM_HangHoa WHERE ID= @ID_HangHoa and (TheoDoi='1' OR TheoDoi is null)
			IF @QuanLyTheoLo IS NULL SET @valReturn= '0'
			ELSE SET @valReturn= @QuanLyTheoLo
		END
SELECT @valReturn AS Exist");

            CreateStoredProcedure(name: "[dbo].[SP_DeleteCustomer_In_HTThongBao]", parametersAction: p => new
            {
                where = p.String()
            }, body: @"DECLARE @sql nvarchar(max)
	SET @sql='
	DELETE FROM HT_ThongBao where '+ @where
	EXEC (@sql)");

            CreateStoredProcedure(name: "[dbo].[SP_GetChiTietHD_MultipleHoaDon]", parametersAction: p => new
            {
                lstID_HoaDon = p.String()
            }, body: @"SELECT 
    		cthd.ID,cthd.ID_HoaDon,DonGia,cthd.GiaVon,SoLuong,ThanhTien,ID_DonViQuiDoi,
    		cthd.TienChietKhau AS GiamGia,PTChietKhau,ThoiGian,cthd.GhiChu,
    		(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
    		CAST(SoThuTu AS float) AS SoThuTu,cthd.ID_KhuyenMai,
    			(REPLACE(REPLACE(TenHangHoa,CHAR(13),''),CHAR(10),'') +
    				CASE WHEN (ThuocTinh_GiaTri is null or ThuocTinh_GiaTri = '') then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    				CASE WHEN TenDonVitinh = '' or TenDonViTinh is null then '' else ' (' + TenDonViTinh + ')' end +
    				CASE WHEN MaLoHang is null then '' else '. Lô: ' + MaLoHang end) as TenHangHoaFull,
    				
    		DM_HangHoa.ID AS ID_HangHoa,LaHangHoa,QuanLyTheoLoHang,TenHangHoa,		
    		TenDonViTinh,MaHangHoa,TyLeChuyenDoi,YeuCau,MaLoHang,
    		DM_LoHang.ID AS ID_LoHang,ISNULL(MaLoHang,'') as MaLoHang,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri
    		FROM BH_HoaDon
    		JOIN BH_HoaDon_ChiTiet cthd ON BH_HoaDon.ID= cthd.ID_HoaDon
    		JOIN DonViQuiDoi ON cthd.ID_DonViQuiDoi = DonViQuiDoi.ID
    		JOIN DM_HangHoa ON DonViQuiDoi.ID_HangHoa= DM_HangHoa.ID
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
					) as ThuocTinh on DM_HangHoa.ID = ThuocTinh.id_hanghoa
    		LEFT JOIN DM_LoHang ON cthd.ID_LoHang = DM_LoHang.ID
    		WHERE cthd.ID_HoaDon in (Select * from splitstring(@lstID_HoaDon))  
			and (cthd.ID_ChiTietDinhLuong= cthd.ID OR cthd.ID_ChiTietDinhLuong is null)");

            CreateStoredProcedure(name: "[dbo].[SP_Insert_HT_ThongBao]", parametersAction: p => new
            {
                ID_DonVi = p.String(),
                LoaiThongBao = p.Int(),
                NoiDungThongBao = p.String(),
                NguoiDungDaDoc = p.String()
            }, body: @"INSERT INTO HT_ThongBao (ID,ID_DonVi,LoaiThongBao,NoiDungThongBao,NgayTao,NguoiDungDaDoc) VALUES (NEWID(), @ID_DonVi,@NoiDungThongBao, GETDATE(),'','')");

            CreateStoredProcedure(name: "[dbo].[SP_CheckLoHangExist]", parametersAction: p => new
            {
                MaHangHoa = p.String(),
                MaLoHang = p.String()
            }, body: @"DECLARE @ID_HangHoa nvarchar(max);
	DECLARE @valReturn bit ='0'

	-- check HangHoa exist in DonViQuiDoi and not delete
	SELECT @ID_HangHoa = ID_HangHoa FROM DonViQuiDoi WHERE MaHangHoa like @MaHangHoa and (Xoa = '0'  OR Xoa is null)

	-- check LoHang exist in DM_LoHang
	IF @ID_HangHoa IS NULL 
		SET @valReturn= '0'
	ELSE
		BEGIN
			DECLARE @ID_LoHang nvarchar(max)
			SELECT @ID_LoHang = ID FROM DM_LoHang WHERE ID_HangHoa= @ID_HangHoa and MaLoHang like @MaLoHang
			IF @ID_LoHang IS NULL SET @valReturn= '0'
			ELSE SET @valReturn= '1'
		END
SELECT @valReturn AS Exist");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietChungCongPhanTram]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DonViQuiDoi SET GiaBan = GiaBan + @giaTri * GiaBan/ 100 where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
    	END
    	else if(@LoaiGiaChon = 2) 
    	BEGIN
    		update DonViQUiDoi SET GiaBan =GiaNhap + @giaTri * GiaNhap/ 100 where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
    	END
    	else
    	BEGIN
    		DECLARE @ListIDDVQD TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER)
    			INSERT INTO @ListIDDVQD SELECT DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang
				FROM DonViQuiDoi
				LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
    			DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
				DECLARE @ID_LoHang UNIQUEIDENTIFIER;
    
    			DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD
    			OPEN CS_Item 
    			FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi,@ID_LoHang
    			WHILE @@FETCH_STATUS = 0 
    			BEGIN
					IF(@ID_LoHang is null)
					BEGIN
    					DECLARE @GiaVon FLOAT;
    					SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang is NULL
						UPDATE DonViQuiDoi SET GiaBan = @GiaVon + @giaTri * @GiaVon/ 100 where ID = @ID_DonViQuiDoi AND ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))); 
					END
    				FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi,@ID_LoHang
    			END
    		CLOSE CS_Item
    		DEALLOCATE CS_Item
    	END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DonViQuiDoi SET GiaBan = GiaBan + @giaTri * GiaBan/ 100;
    	END
    	else if(@LoaiGiaChon = 2) 
    	BEGIN
    		update DonViQUiDoi SET GiaBan =GiaNhap + @giaTri * GiaNhap/ 100;
    	END
    	else
    	BEGIN
    			DECLARE @ListIDDVQD1 TABLE(ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER)
    			INSERT INTO @ListIDDVQD1 SELECT DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1
				FROM DonViQuiDoi
				LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
    			DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
				DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;
    
    			DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD1
    			OPEN CS_Item 
    			FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi1,@ID_LoHang1
    			WHILE @@FETCH_STATUS = 0 
    			BEGIN
					IF(@ID_LoHang1 is null)
					BEGIN
    					DECLARE @GiaVon1 FLOAT;
    					SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang is NULL
						UPDATE DonViQuiDoi SET GiaBan = @GiaVon1 + @giaTri * @GiaVon1/ 100 where ID = @ID_DonViQuiDoi1
					END
    				FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi1,@ID_LoHang1
    			END
    		CLOSE CS_Item
    		DEALLOCATE CS_Item
    	END
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietChungCongVND]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    		if(@LoaiGiaChon = 0)
    		BEGIN
    			update DonViQuiDoi SET GiaBan = GiaBan + @giaTri where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))
    		END
    	else if(@LoaiGiaChon = 2) 
    		BEGIN
    			update DonViQUiDoi SET GiaBan =GiaNhap + @giaTri where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))
    		END
    	else
    		BEGIN
    			DECLARE @ListIDDVQD TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER)
    				INSERT INTO @ListIDDVQD SELECT DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang
					FROM DonViQuiDoi
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
    				DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang UNIQUEIDENTIFIER;
    
    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi, @ID_LoHang
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang is null)
						BEGIN
    						DECLARE @GiaVon FLOAT;
    						SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang is NULL
							UPDATE DonViQuiDoi SET GiaBan = @GiaVon + @giaTri where ID = @ID_DonViQuiDoi AND ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
						END
    					FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi, @ID_LoHang
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    		END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    		BEGIN
    			update DonViQuiDoi SET GiaBan = GiaBan + @giaTri
    		END
    	else if(@LoaiGiaChon = 2) 
    		BEGIN
    			update DonViQUiDoi SET GiaBan =GiaNhap + @giaTri  
    		END
    	else
    		BEGIN
    				DECLARE @ListIDDVQD1 TABLE(ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER)
    				INSERT INTO @ListIDDVQD1 SELECT DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1
					FROM DonViQuiDoi
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
    				DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;
    
    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD1
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi1, @ID_LoHang1
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang1 is null)
						BEGIN
    						DECLARE @GiaVon1 FLOAT;
    						SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang is NULL
							UPDATE DonViQuiDoi SET GiaBan = @GiaVon1 + @giaTri where ID = @ID_DonViQuiDoi1
						END
    					FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi1, @ID_LoHang1
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    		END
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietChungTruPhanTram]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    		if(@LoaiGiaChon = 0)
    			BEGIN
    				update DonViQuiDoi SET GiaBan = GiaBan - @giaTri * GiaBan/ 100 where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
    			END
    			else if(@LoaiGiaChon = 2) 
    			BEGIN
    				update DonViQUiDoi SET GiaBan =GiaNhap - @giaTri * GiaNhap/ 100 where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
    			END
    			else
    			BEGIN
    				DECLARE @ListIDDVQD TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER)
    					INSERT INTO @ListIDDVQD SELECT DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang 
						FROM DonViQuiDoi
						LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
    					DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
						DECLARE @ID_LoHang UNIQUEIDENTIFIER;
    
    					DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD
    					OPEN CS_Item 
    					FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi, @ID_LoHang
    					WHILE @@FETCH_STATUS = 0 
    					BEGIN
							IF(@ID_LoHang is null)
							BEGIN
    							DECLARE @GiaVon FLOAT;
    							SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang is NULL
    							update DonViQUiDoi SET GiaBan = @GiaVon - @giaTri * @GiaVon/ 100 where ID = @ID_DonViQuiDoi AND ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
							END
							FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi, @ID_LoHang
    					END
    				CLOSE CS_Item
    				DEALLOCATE CS_Item
    			END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DonViQuiDoi SET GiaBan = GiaBan - @giaTri * GiaBan/ 100;
    	END
    	else if(@LoaiGiaChon = 2) 
    	BEGIN
    		update DonViQUiDoi SET GiaBan =GiaNhap - @giaTri * GiaNhap/ 100;
    	END
    	else
    	BEGIN
    			DECLARE @ListIDDVQD1 TABLE(ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER)
    				INSERT INTO @ListIDDVQD1 SELECT DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1 
					FROM DonViQuiDoi
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
    				DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;
    
    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD1
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi1, @ID_LoHang1
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang1 is null)
						BEGIN
    						DECLARE @GiaVon1 FLOAT;
    						SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang is NULL
    						update DonViQUiDoi SET GiaBan = @GiaVon1 - @giaTri * @GiaVon1/ 100 where ID = @ID_DonViQuiDoi1
						END
						FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi1, @ID_LoHang1
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietChungTruVND]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    	    if(@LoaiGiaChon = 0)
    	BEGIN
    		update DonViQuiDoi SET GiaBan = CASE WHEN GiaBan - @giaTri >= 0 THEN GiaBan - @giaTri ELSE 0 END where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
    	END
    	else if(@LoaiGiaChon = 2) 
    	BEGIN
    		update DonViQUiDoi SET GiaBan = CASE WHEN GiaNhap - @giaTri >= 0 THEN GiaNhap - @giaTri ELSE 0 END where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
    	END
    	else
    	BEGIN
    			DECLARE @ListIDDVQD TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER)
    				INSERT INTO @ListIDDVQD SELECT DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang 
					FROM DonViQuiDoi
    				LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa

					DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang UNIQUEIDENTIFIER;
    
    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi,@ID_LoHang
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang is null)
						BEGIN
    						DECLARE @GiaVon FLOAT;
    						SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang is NULL
    						update DonViQUiDoi SET GiaBan = CASE WHEN @GiaVon - @giaTri >= 0 THEN @GiaVon - @giaTri ELSE 0 END where ID= @ID_DonViQuiDoi and ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)));
						END
						FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi,@ID_LoHang
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DonViQuiDoi SET GiaBan = CASE WHEN GiaBan - @giaTri >= 0 THEN GiaBan - @giaTri ELSE 0 END;
    	END
    	else if(@LoaiGiaChon = 2) 
    	BEGIN
    		update DonViQUiDoi SET GiaBan = CASE WHEN GiaNhap - @giaTri >= 0 THEN GiaNhap - @giaTri ELSE 0 END;
    	END
    	else
    	BEGIN
    			DECLARE @ListIDDVQD1 TABLE(ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER)
    				INSERT INTO @ListIDDVQD1 SELECT DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1 
					FROM DonViQuiDoi
    				LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa

					DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;
    
    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListIDDVQD1
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @ID_DonViQuiDoi1,@ID_LoHang1
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang1 is null)
						BEGIN
    						DECLARE @GiaVon1 FLOAT;
    						SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang is NULL
    						update DonViQUiDoi SET GiaBan = CASE WHEN @GiaVon1 - @giaTri >= 0 THEN @GiaVon1 - @giaTri ELSE 0 END where ID= @ID_DonViQuiDoi1
						END
						FETCH NEXT FROM CS_Item INTO @ID_DonViQuiDoi1,@ID_LoHang1
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietCongPhanTram]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ID [uniqueidentifier],
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan + (@giaTri* GiaBan /100)  where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))))
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri*(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) /100 where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))))
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri *(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
    	END
    	else
    	BEGIN
    			DECLARE @ListGiaBanChiTiet TABLE (IDGB UNIQUEIDENTIFIER, ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet SELECT DM_GiaBan_ChiTiet.ID as IDGB, DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang 
					FROM DM_GiaBan_ChiTiet 
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang UNIQUEIDENTIFIER;
    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi,@ID_LoHang
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang is null)
						BEGIN
    						DECLARE @GiaVon FLOAT;
    						SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = @GiaVon + @giaTri * @GiaVon/100 where ID = @IDGB and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))))  
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi,@ID_LoHang
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan + (@giaTri* GiaBan /100)  where ID_GiaBan = @ID 
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri*(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) /100 where ID_GiaBan = @ID  
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri *(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID  
    	END
    	else
    	BEGIN
    				DECLARE @ListGiaBanChiTiet1 TABLE (IDGB1 UNIQUEIDENTIFIER, ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet1 SELECT DM_GiaBan_ChiTiet.ID as IDGB1, DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1 
					FROM DM_GiaBan_ChiTiet 
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB1 UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;
    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet1
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1,@ID_LoHang1
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang1 is null)
						BEGIN
    						DECLARE @GiaVon1 FLOAT;
    						SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = @GiaVon1 + @giaTri * @GiaVon1/100 where ID = @IDGB1
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1,@ID_LoHang1
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietCongVND]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ID [nvarchar](max),
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan + @giaTri where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))))
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))))  
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
    	END
    	else
    	BEGIN
    			DECLARE @ListGiaBanChiTiet TABLE (IDGB UNIQUEIDENTIFIER, ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet SELECT DM_GiaBan_ChiTiet.ID as IDGB, DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang 
					FROM DM_GiaBan_ChiTiet
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang UNIQUEIDENTIFIER;

    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi,@ID_LoHang
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang is null)
						BEGIN
    						DECLARE @GiaVon FLOAT;
    						SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = @GiaVon + @giaTri where ID = @IDGB and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi,@ID_LoHang
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    		
    	END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan + @giaTri where ID_GiaBan = @ID 
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri where ID_GiaBan = @ID  
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) + @giaTri where ID_GiaBan = @ID  
    	END
    	else
    	BEGIN
    				DECLARE @ListGiaBanChiTiet1 TABLE (IDGB1 UNIQUEIDENTIFIER, ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet1 SELECT DM_GiaBan_ChiTiet.ID as IDGB1, DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1 
					FROM DM_GiaBan_ChiTiet
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB1 UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;

    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet1
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1,@ID_LoHang1
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang1 is null)
						BEGIN
    						DECLARE @GiaVon1 FLOAT;
    						SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = @GiaVon1 + @giaTri where ID = @IDGB1
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1,@ID_LoHang1
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietTruPhanTram]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ID [uniqueidentifier],
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan - (@giaTri* GiaBan /100)  where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))))
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri*(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) /100 where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri *(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
    	END
    	else
    	BEGIN
    			DECLARE @ListGiaBanChiTiet TABLE (IDGB UNIQUEIDENTIFIER, ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet SELECT DM_GiaBan_ChiTiet.ID as IDGB, DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang 
					FROM DM_GiaBan_ChiTiet
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang UNIQUEIDENTIFIER;

    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi,@ID_LoHang
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang is NUll)
						BEGIN
    						DECLARE @GiaVon FLOAT;
    						SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = @GiaVon - @giaTri * @GiaVon/100 where ID = @IDGB and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi, @ID_LoHang
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    		
    	END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = GiaBan - (@giaTri* GiaBan /100)  where ID_GiaBan = @ID 
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri*(select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) /100 where ID_GiaBan = @ID  
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan =(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri *(select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi)/100 where ID_GiaBan = @ID  
    	END
    	else
    	BEGIN
    				DECLARE @ListGiaBanChiTiet1 TABLE (IDGB1 UNIQUEIDENTIFIER, ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet1 SELECT DM_GiaBan_ChiTiet.ID as IDGB1, DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1 
					FROM DM_GiaBan_ChiTiet
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB1 UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;

    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet1
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1,@ID_LoHang1
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang1 is NUll)
						BEGIN
    						DECLARE @GiaVon1 FLOAT;
    						SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = @GiaVon1 - @giaTri * @GiaVon1/100 where ID = @IDGB1
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1, @ID_LoHang1
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    		
    	END
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PutGiaBanChiTietTruVND]
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiGiaChon [int],
    @giaTri [float],
    @ID [uniqueidentifier],
    @ListID_NhomHang [nvarchar](max)
AS
BEGIN
    if(@ListID_NhomHang != '')
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN GiaBan - @giaTri >= 0 THEN GiaBan - @giaTri ELSE 0 END where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang))))
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri >=0 THEN (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri ELSE 0 END where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri >= 0 THEN (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri ELSE 0 END where ID_GiaBan = @ID and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
    	END
    	else
    	BEGIN
    			DECLARE @ListGiaBanChiTiet TABLE (IDGB UNIQUEIDENTIFIER, ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet SELECT DM_GiaBan_ChiTiet.ID as IDGB, DonViQuiDoi.ID as ID_DonViQuiDoi, DM_LoHang.ID as ID_LoHang
					FROM DM_GiaBan_ChiTiet 
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang UNIQUEIDENTIFIER;

    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi, @ID_LoHang
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang is null)
						BEGIN
    						DECLARE @GiaVon FLOAT;
    						SELECT @GiaVon = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN @GiaVon - @giaTri >=0 THEN @GiaVon - @giaTri ELSE 0 END where ID = @IDGB and ID_DonViQuiDoi in (select ID from DonViQuiDoi where ID_HangHoa in (select ID from DM_HangHoa where ID_NhomHang in (select * from splitstring(@ListID_NhomHang)))) 
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB, @ID_DonViQuiDoi, @ID_LoHang
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
    	ELSE
    	BEGIN
    		if(@LoaiGiaChon = 0)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN GiaBan - @giaTri >=0 THEN GiaBan- @giaTri ELSE 0 END where ID_GiaBan = @ID 
    	END
    	else if(@LoaiGiaChon = 1) 
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri >=0 THEN (select GiaBan from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri ELSE 0 END where ID_GiaBan = @ID  
    	END
    	else if(@LoaiGiaChon = 2)
    	BEGIN
    		update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri >= 0 THEN (select GiaNhap from DonViQuiDoi where id= DM_GiaBan_ChiTiet.ID_DonViQuiDoi) - @giaTri ELSE 0 END where ID_GiaBan = @ID  
    	END
    	else
    	BEGIN
    				DECLARE @ListGiaBanChiTiet1 TABLE (IDGB1 UNIQUEIDENTIFIER, ID_DonViQuiDoi1 UNIQUEIDENTIFIER, ID_LoHang1 UNIQUEIDENTIFIER);
    				INSERT INTO @ListGiaBanChiTiet1 SELECT DM_GiaBan_ChiTiet.ID as IDGB1, DonViQuiDoi.ID as ID_DonViQuiDoi1, DM_LoHang.ID as ID_LoHang1
					FROM DM_GiaBan_ChiTiet 
					LEFT JOIN DonViQuiDoi on DM_GiaBan_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
					LEFT JOIN DM_LoHang on DonViQuiDoi.ID_HangHoa = DM_LoHang.ID_HangHoa
					WHERE ID_GiaBan = @ID
    
    				DECLARE @IDGB1 UNIQUEIDENTIFIER;
    				DECLARE @ID_DonViQuiDoi1 UNIQUEIDENTIFIER;
					DECLARE @ID_LoHang1 UNIQUEIDENTIFIER;

    				DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @ListGiaBanChiTiet1
    				OPEN CS_Item 
    				FETCH FIRST FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1, @ID_LoHang1
    				WHILE @@FETCH_STATUS = 0 
    				BEGIN
						IF(@ID_LoHang1 is null)
						BEGIN
    						DECLARE @GiaVon1 FLOAT;
    						SELECT @GiaVon1 = GiaVon FROM DM_GiaVon WHERE ID_DonVi = @ID_ChiNhanh AND ID_DonViQuiDoi = @ID_DonViQuiDoi1 AND ID_LoHang IS NULL
    						update DM_GiaBan_ChiTiet SET GiaBan = CASE WHEN @GiaVon1 - @giaTri >=0 THEN @GiaVon1 - @giaTri ELSE 0 END where ID = @IDGB1
						END
    					FETCH NEXT FROM CS_Item INTO @IDGB1, @ID_DonViQuiDoi1, @ID_LoHang1
    				END
    			CLOSE CS_Item
    			DEALLOCATE CS_Item
    	END
    	END
END");
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetList_GoiDichVu_Where]");
            DropStoredProcedure("[dbo].[getList_HoaDonbyNhanVien]");
            DropStoredProcedure("[dbo].[getList_HoaHongNhanVien]");
            DropStoredProcedure("[dbo].[getList_HoaHongNhanVien_Excel]");
            DropStoredProcedure("[dbo].[getListDanhSachHHImport]");
            DropStoredProcedure("[dbo].[insert_ChietKhauMacDinhNhanVien]");
            DropStoredProcedure("[dbo].[SP_CheckHangHoa_DangKinhDoanh]");
            DropStoredProcedure("[dbo].[SP_CheckHangHoa_QuanLyTheoLo]");
            DropStoredProcedure("[dbo].[SP_DeleteCustomer_In_HTThongBao]");
            DropStoredProcedure("[dbo].[SP_GetChiTietHD_MultipleHoaDon]");
            DropStoredProcedure("[dbo].[SP_Insert_HT_ThongBao]");
            DropStoredProcedure("[dbo].[SP_CheckLoHangExist]");
        }
    }
}

