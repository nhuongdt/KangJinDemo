namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20181108 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetInForStaff_Working_byChiNhanhDaTaoND]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"SELECT nv.ID,MaNhanVien,TenNhanVien,DienThoaiDiDong, GioiTinh
    	from HT_NguoiDung nd 
		join NS_NhanVien nv on nv.ID = nd.ID_NhanVien
    	join NS_QuaTrinhCongTac qt on nv.ID = qt.ID_NhanVien
    	where qt.ID_DonVi like @ID_DonVi and nd.DangHoatDong = 1");

            CreateStoredProcedure(name: "[dbo].[getlist_HoaDonBanHang_FindMaHang_NVien]", parametersAction: p => new
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
    	c.TongTienHang, c.TongGiamGia, 
		-- neu HD DatHang/HD Ban: PhaiTT = PhaiTT
		-- neu HĐoiTra: PhaiTT = PhaiTT- TongHDTra
		CASE WHEN c.LoaiHoaDonGoc = 3 THEN c.PhaiThanhToan
		ELSE
			case when c.PhaiThanhToan < c.TongTienHDTra then 0
			else c.PhaiThanhToan - c.TongTienHDTra end
		END AS PhaiThanhToan,
		--c.PhaiThanhToan,
		c.ThuTuThe, c.TienMat, c.ChuyenKhoan, c.KhachDaTra,c.TongChietKhau,
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
    	ISNULL(c.CTHoaDon_NVThucHien,'') as NhanVienThucHien -- string contail all MaHangHoa,TenHangHoa of HoaDon

    	FROM
    	(
    		select 
    		a.ID as ID,
    		hdXMLOut.HoaDon_HangHoa,
    		hdXMLOut2.CTHoaDon_NVThucHien,

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
    		--Case when vt.ID is not null then vt.ID else N'00000000-0000-0000-0000-000000000000' end as TenViTri,
    			ISNULL(vt.TenViTri,'') as TenPhongBan,
    		bhhd.MaHoaDon,
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
    		bhhd.NgayLapHoaDon,
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
    					--and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    				where bhhd.LoaiHoaDon = '1' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
    
    				Union all
    				Select
    					d.ID,
    					Case when RowNumber = 1 then d.TienMat else 0 end as TienMat,
    					Case when RowNumber = 1 then d.TienGui else 0 end as TienGui,
    					Case when RowNumber = 1 then d.ThuTuThe else 0 end as ThuTuThe,
    					Case when RowNumber = 1 then d.TienThu else 0 end as TienThu
    				FROM
    				(
    					SELECT ROW_NUMBER() Over(PARTITION BY ID_DatHang ORDER BY f.NgayLapHoaDon)
    					As RowNumber,* FROM 
    					(
    						Select
    						bhhd.ID,
    						bhhd.NgayLapHoaDon,
    						hdt.ID as ID_DatHang,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienGui, 0) else ISNULL(hdct.TienGui, 0) * (-1) end end as TienGui,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
    						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu
    						from BH_HoaDon bhhd
    						inner join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
    						left join Quy_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDonLienQuan
    						left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID 
    							--and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    						where hdt.LoaiHoaDon = '3' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
    					) f
    				) d
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
			left join (
					Select distinct hdXML2.ID, 
    					 (
    						select distinct(nv.TenNhanVien) +', '  AS [text()]
    						from BH_HoaDon_ChiTiet ct
    						left join BH_NhanVienThucHien nvth on ct.ID= nvth.ID_ChiTietHoaDon
    						join  NS_NhanVien nv on  nvth.ID_NhanVien= nv.ID
    						where ct.ID_HoaDon = hdXML2.ID
    						For XML PATH ('')
    					) CTHoaDon_NVThucHien
    				from BH_HoaDon hdXML2
					) hdXMLOut2 on a.ID= hdXMLOut2.ID
    		) as c
    	WHERE MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD or DienThoai like @maHD or MaDoiTuong like @maHD
    		OR HoaDon_HangHoa like @maHD
    	ORDER BY c.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[SP_Check_LoaiCongViec_IsUsed]", parametersAction: p => new
            {
                ID_LoaiCongViec = p.String()
            }, body: @"DECLARE @valReturn bit ='0'
    	DECLARE @ID nvarchar(max);
    	SELECT @ID = ID from NS_CongViec WHERE ID_LoaiCongViec like @ID_LoaiCongViec
    
    	IF @ID IS NULL SET @valReturn= '0'
		ELSE SET @valReturn= '1'
    
    	SELECT @valReturn AS Exist");

            CreateStoredProcedure(name: "[dbo].[SP_GetListCongViec_ByKhachHang]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_NhanVien = p.String()
            }, body: @"SELECT cv.ID, cv.ID_LoaiCongViec, LoaiCongViec, ThoiGianTu, ThoiGianDen,cv.ID_KhachHang,cv.ID_LienHe,
	ISNULL(dt.TenDoiTuong,'') as TenDoiTuong, ISNULL(lh.TenLienHe,'') as TenLienHe,
	ISNULL(cv.ID_NhanVienQuanLy,'00000000-0000-0000-0000-000000000000') as ID_NhanVienQuanLy, nvth.TenNhanVien as TenNVThucHien, 
	ISNULL(cv.ID_NhanVienChiaSe,'00000000-0000-0000-0000-000000000000') as ID_NhanVienChiaSe, ISNull(nv.TenNhanVien,'') as TenNVChiaSe,
	NoiDung,ThoiGianLienHeLai, cv.NhacTruoc, cv.NhacTruocLienHeLai, cv.TrangThai, ISNULL(KetQuaCongViec,'') AS KetQuaCongViec, ISNULL(LyDoHenLai,'') AS LyDoHenLai,
	CASE 
		WHEN cv.TrangThai = 0 THEN N'Đã xóa'
		WHEN cv.TrangThai = 1 THEN N'Đang xử lý'
		WHEN cv.TrangThai = 2 THEN N'Hoàn thành'
		WHEN cv.TrangThai = 3 THEN N'Hủy'
	ELSE '' END AS TrangThaiStr,

	CASE 
		--WHEN cv.NhacTruoc IS NULL OR cv.NhacTruoc=0 THEN N'Không'
		WHEN cv.NhacTruoc =1 THEN N'5 phút'
		WHEN cv.NhacTruoc=2 THEN N'10 phút'
		WHEN cv.NhacTruoc=3 THEN N'15 phút'
		WHEN cv.NhacTruoc=4 THEN N'30 phút'
		WHEN cv.NhacTruoc=5 THEN N'1 tiếng'
		WHEN cv.NhacTruoc=6 THEN N'2 tiếng'
		WHEN cv.NhacTruoc=7 THEN N'3 tiếng'
		WHEN cv.NhacTruoc=8 THEN '4 tiếng'
		WHEN cv.NhacTruoc=9 THEN N'5 tiếng'
		WHEN cv.NhacTruoc=10 THEN N'6 tiếng'
		WHEN cv.NhacTruoc=11 THEN N'7 tiếng'
		WHEN cv.NhacTruoc=12 THEN N'8 tiếng'
		WHEN cv.NhacTruoc=13 THEN N'9 tiếng'
		WHEN cv.NhacTruoc=14 THEN N'10 tiếng'
		WHEN cv.NhacTruoc=15 THEN N'11 tiếng'
		WHEN cv.NhacTruoc=16 THEN N'18 tiếng'
		WHEN cv.NhacTruoc=17 THEN N'0.5 ngày'
		WHEN cv.NhacTruoc=18 THEN N'1 ngày'
		WHEN cv.NhacTruoc=19 THEN N'2 ngày'
		WHEN cv.NhacTruoc=20 THEN N'3 ngày'
	ELSE '' END AS NhacTruocStr,

	CASE 
		--WHEN cv.NhacTruocLienHeLai IS NULL OR cv.NhacTruocLienHeLai=0 THEN N'Không'
		WHEN cv.NhacTruocLienHeLai =1 THEN N'5 phút'
		WHEN cv.NhacTruocLienHeLai=2 THEN N'10 phút'
		WHEN cv.NhacTruocLienHeLai=3 THEN N'15 phút'
		WHEN cv.NhacTruocLienHeLai=4 THEN N'30 phút'
		WHEN cv.NhacTruocLienHeLai=5 THEN N'1 tiếng'
		WHEN cv.NhacTruocLienHeLai=6 THEN N'2 tiếng'
		WHEN cv.NhacTruocLienHeLai=7 THEN N'3 tiếng'
		WHEN cv.NhacTruocLienHeLai=8 THEN N'4 tiếng'
		WHEN cv.NhacTruocLienHeLai=9 THEN N'5 tiếng'
		WHEN cv.NhacTruocLienHeLai=10 THEN N'6 tiếng'
		WHEN cv.NhacTruocLienHeLai=11 THEN N'7 tiếng'
		WHEN cv.NhacTruocLienHeLai=12 THEN N'8 tiếng'
		WHEN cv.NhacTruocLienHeLai=13 THEN N'9 tiếng'
		WHEN cv.NhacTruocLienHeLai=14 THEN N'10 tiếng'
		WHEN cv.NhacTruocLienHeLai=15 THEN N'11 tiếng'
		WHEN cv.NhacTruocLienHeLai=16 THEN N'18 tiếng'
		WHEN cv.NhacTruocLienHeLai=17 THEN N'0.5 ngày'
		WHEN cv.NhacTruocLienHeLai=18 THEN N'1 ngày'
		WHEN cv.NhacTruocLienHeLai=19 THEN N'2 ngày'
		WHEN cv.NhacTruocLienHeLai=20 THEN N'3 ngày'
	ELSE '' END AS NhacTruocLienHeLaiStr

FROM NS_CongViec cv
left join NS_CongViec_PhanLoai lcv on cv.ID_LoaiCongViec = lcv.ID
left join DM_LienHe lh on cv.ID_LienHe = lh.ID
left join DM_DoiTuong dt on cv.ID_KhachHang = dt.ID
left join NS_NhanVien nv on cv.ID_NhanVienChiaSe = nv.ID
left join NS_NhanVien nvth on cv.ID_NhanVienQuanLy = nvth.ID
where cv.ID_KhachHang like @ID_DoiTuong AND cv.TrangThai !=0 AND ( cv.ID_NhanVienQuanLy like @ID_NhanVien OR cv.ID_NhanVienChiaSe like @ID_NhanVien)");

            CreateStoredProcedure(name: "[dbo].[SP_GetListCongViec_Where]", parametersAction: p => new
            {
                txtSearch = p.String()
            }, body: @"DECLARE @where AS nvarchar (max)=N''
    
    IF @txtSearch !=''
    	SET @where = ' WHERE '+ @txtSearch
    
    DECLARE @sqlExc nvarchar(max) = 'SELECT cv.ID, cv.ID_LoaiCongViec, LoaiCongViec, ThoiGianTu, ThoiGianDen,cv.ID_KhachHang,cv.ID_LienHe,
	ISNULL(dt.TenDoiTuong,'''') as TenDoiTuong, ISNULL(lh.TenLienHe,'''') as TenLienHe,
	ISNULL(cv.ID_NhanVienQuanLy,''00000000-0000-0000-0000-000000000000'') as ID_NhanVienQuanLy, nvth.TenNhanVien as TenNVThucHien, 
	ISNULL(cv.ID_NhanVienChiaSe,''00000000-0000-0000-0000-000000000000'') as ID_NhanVienChiaSe, ISNull(nv.TenNhanVien,'''') as TenNVChiaSe,
	NoiDung,ThoiGianLienHeLai, cv.NhacTruoc, cv.NhacTruocLienHeLai, cv.TrangThai, ISNULL(KetQuaCongViec,'''') AS KetQuaCongViec, ISNULL(LyDoHenLai,'''') AS LyDoHenLai,
	CASE WHEN cv.TrangThai = 1 THEN N''Đang xử lý''
		WHEN cv.TrangThai = 2 THEN N''Hoàn thành''
		WHEN cv.TrangThai = 3 THEN N''Hủy''
	ELSE '''' END AS TrangThaiStr,

	CASE 
		WHEN cv.NhacTruoc IS NULL OR cv.NhacTruoc=0 THEN N''Không''
		WHEN cv.NhacTruoc =1 THEN N''5 phút''
		WHEN cv.NhacTruoc=2 THEN N''10 phút''
		WHEN cv.NhacTruoc=3 THEN N''15 phút''
		WHEN cv.NhacTruoc=4 THEN N''30 phút''
		WHEN cv.NhacTruoc=5 THEN N''1 tiếng''
		WHEN cv.NhacTruoc=6 THEN N''2 tiếng''
		WHEN cv.NhacTruoc=7 THEN N''3 tiếng''
		WHEN cv.NhacTruoc=8 THEN ''4 tiếng''
		WHEN cv.NhacTruoc=9 THEN N''5 tiếng''
		WHEN cv.NhacTruoc=10 THEN N''6 tiếng''
		WHEN cv.NhacTruoc=11 THEN N''7 tiếng''
		WHEN cv.NhacTruoc=12 THEN N''8 tiếng''
		WHEN cv.NhacTruoc=13 THEN N''9 tiếng''
		WHEN cv.NhacTruoc=14 THEN N''10 tiếng''
		WHEN cv.NhacTruoc=15 THEN N''11 tiếng''
		WHEN cv.NhacTruoc=16 THEN N''18 tiếng''
		WHEN cv.NhacTruoc=17 THEN N''0.5 ngày''
		WHEN cv.NhacTruoc=18 THEN N''1 ngày''
		WHEN cv.NhacTruoc=19 THEN N''2 ngày''
		WHEN cv.NhacTruoc=20 THEN N''3 ngày''
	ELSE '''' END AS NhacTruocStr,

	CASE 
		WHEN cv.NhacTruocLienHeLai IS NULL OR cv.NhacTruocLienHeLai=0 THEN N''Không''
		WHEN cv.NhacTruocLienHeLai =1 THEN N''5 phút''
		WHEN cv.NhacTruocLienHeLai=2 THEN N''10 phút''
		WHEN cv.NhacTruocLienHeLai=3 THEN N''15 phút''
		WHEN cv.NhacTruocLienHeLai=4 THEN N''30 phút''
		WHEN cv.NhacTruocLienHeLai=5 THEN N''1 tiếng''
		WHEN cv.NhacTruocLienHeLai=6 THEN N''2 tiếng''
		WHEN cv.NhacTruocLienHeLai=7 THEN N''3 tiếng''
		WHEN cv.NhacTruocLienHeLai=8 THEN N''4 tiếng''
		WHEN cv.NhacTruocLienHeLai=9 THEN N''5 tiếng''
		WHEN cv.NhacTruocLienHeLai=10 THEN N''6 tiếng''
		WHEN cv.NhacTruocLienHeLai=11 THEN N''7 tiếng''
		WHEN cv.NhacTruocLienHeLai=12 THEN N''8 tiếng''
		WHEN cv.NhacTruocLienHeLai=13 THEN N''9 tiếng''
		WHEN cv.NhacTruocLienHeLai=14 THEN N''10 tiếng''
		WHEN cv.NhacTruocLienHeLai=15 THEN N''11 tiếng''
		WHEN cv.NhacTruocLienHeLai=16 THEN N''18 tiếng''
		WHEN cv.NhacTruocLienHeLai=17 THEN N''0.5 ngày''
		WHEN cv.NhacTruocLienHeLai=18 THEN N''1 ngày''
		WHEN cv.NhacTruocLienHeLai=19 THEN N''2 ngày''
		WHEN cv.NhacTruocLienHeLai=20 THEN N''3 ngày''
	ELSE '''' END AS NhacTruocLienHeLaiStr

FROM NS_CongViec cv
left join NS_CongViec_PhanLoai lcv on cv.ID_LoaiCongViec = lcv.ID
left join DM_LienHe lh on cv.ID_LienHe = lh.ID
left join DM_DoiTuong dt on cv.ID_KhachHang = dt.ID
left join NS_NhanVien nv on cv.ID_NhanVienChiaSe = nv.ID
left join NS_NhanVien nvth on cv.ID_NhanVienQuanLy = nvth.ID' + @where + ' ORDER BY cv.NgayTao DESC'
    EXEC sp_executesql  @sqlExc");
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetInForStaff_Working_byChiNhanhDaTaoND]");
            DropStoredProcedure("[dbo].[getlist_HoaDonBanHang_FindMaHang_NVien]");
            DropStoredProcedure("[dbo].[SP_Check_LoaiCongViec_IsUsed]");
            DropStoredProcedure("[dbo].[SP_GetListCongViec_ByKhachHang]");
            DropStoredProcedure("[dbo].[SP_GetListCongViec_Where]");
        }
    }
}