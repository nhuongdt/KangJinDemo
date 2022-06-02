namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180818 : DbMigration
    {
        public override void Up()
        {
            
            AlterStoredProcedure(name: "[dbo].[Report_BanHang]", parametersAction: p => new
            {
                MaHD = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT 
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
    			a.MaHangHoa,
    			a.TenHangHoaFull,
    			a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    			a.TenDonViTinh,
    				a.TenLoHang,
    		CAST(ROUND((a.SoLuong), 3) as float) as SoLuong, 
    		CAST(ROUND((a.GiaBan), 0) as float) as GiaBan,
    			CAST(ROUND((a.TienChietKhau), 0) as float) as TienChietKhau,
    		CAST(ROUND((a.ThanhTien), 0) as float) as ThanhTien,
    		CAST(ROUND((a.GiaVon), 0) as float) as GiaVon,
    		CAST(ROUND((a.SoLuong * a.GiaVon), 0) as float) as TienVon,
    			CAST(ROUND((a.GiamGiaHD), 0) as float) as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD), 0) as float) else 0 end as LaiLo, 
    		a.TenNhanVien,
    		a.TenDonViTinh,
    		a.ID_NhomHang  
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    			ISNULL(hdct.DonGia, 0) as GiaBan,
    			ISNULL(hdct.TienChietKhau, 0) as TienChietKhau,
    		--ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0) as GiaBan,
    		Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon, 0) else 0 end as GiaVon, 
    		nv.TenNhanVien,
    		hh.ID_NhomHang,
    		hdct.ThanhTien,
    			hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon = 1
    		and hh.LaHangHoa like @LaHangHoa
    		and hd.MaHoaDon like @maHD
    			and hd.TongTienHang > 0
    	) a
    	order by a.NgayLapHoaDon desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_LoiNhuan]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT 
    	*, 
    	Case When HH.DoanhThuThuan != 0 then CAST(ROUND((HH.LoiNhuan / HH.DoanhThuThuan) * 100, 2) as float ) else 0 end  as TySuat
    	FROM
    	(
    		SELECT
    		dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		hh.ID_NhomHang,
    		CAST(ROUND(a.SoLuongban , 3) as float ) as SoLuongBan,
    		CAST(ROUND(a.GiaTriBan , 0) as float ) as DoanhThu,
    		CAST(ROUND(a.SoLuongTra , 3) as float ) as SoLuongTra,
    		CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
    		CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan,
    		Case When @XemGiaVon = '1' then CAST(ROUND(a.TongGiaVonBan - a.TongGiaVonTra , 0) as float ) else 0 end as TongGiaVon,
    		Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriBan - a.GiaTriTra - a.TongGiaVonBan + a.TongGiaVonTra , 0) as float )  else 0 end as LoiNhuan
    		FROM
    		(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    			SUM(Case when hd.LoaiHoaDon = 1 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongTra,
    			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.ThanhTien, 0) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) else 0 end) as GiaTriBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) else 0 end) as GiaTriTra,
    			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.SoLuong, 0) * ISNULL(hdct.GiaVon, 0)) else 0 end) as TongGiaVonBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.SoLuong, 0) * ISNULL(hdct.GiaVon, 0)) else 0 end) as TongGiaVonTra
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi  in (select * from splitstring(@ID_ChiNhanh))
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and hh.LaHangHoa like @LaHangHoa
    				and hd.TongTienHang > 0
    			GROUP BY dvqd.ID, ID_LoHang
    		) a
    		left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    	) as HH
    	ORDER BY LoiNhuan DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_NhapChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT
    		dvqd.ID,
    		hh.ID_NhomHang,
    	dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND(a.SoLuong , 3) as float ) as SoLuong,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTri , 0) as float ) else 0 end as GiaTri
    	FROM
    	(
    			SELECT
    				b.ID_DonViQuiDoi as ID_DonViQuiDoi,
    					b. ID_LoHang,
    				SUM(ISNULL(b.SoLuong, 3)) as SoLuong,
    				SUM(ISNULL(b.GiaTri, 0)) as GiaTri
    			FROM
    			(
    				SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				ISNULL(hdct.tienchietkhau,0) as SoLuong,
    				ISNULL(hdct.ThanhTien,0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.ID_CheckIn is not null 
    				and hd.ID_CheckIn in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = '10' and hd.YeuCau = '4')
    			and hd.ChoThanhToan = 0
    			) b
    			GROUP BY b.ID_DonViQuiDoi, b.ID_LoHang
    	) a
    	inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    		where (dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    		and hh.LaHangHoa like  @LaHangHoa
    	ORDER BY MaHangHoa DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_NhapChuyenHangChiTiet]", parametersAction: p => new
            {
                ID_ChiNhanh = p.String(),
                ID_DonViQuiDoi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    Select 
    		bhhd.MaHoaDon,
    		bhhd.NgayLapHoaDon,
    		dvn.TenDonVi as TuDonVi,
    			dv.TenDonVi as ToiDonVi,
    		CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
    		Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(hdct.DonGia, 0), 0) as float) else 0 end as DonGia,
    		Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float)else 0 end as ThanhTien,
    		N'Đã nhận hàng' as TrangThai
    	From
    	BH_HoaDon bhhd 
    	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
    	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
    	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
    	where hdct.ID_DonViQuiDoi = @ID_DonViQuiDoi
    	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
    	and (bhhd.ID_CheckIn is not null 
    		and bhhd.ID_CheckIn in (select * from splitstring(@ID_ChiNhanh))
    		and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4')
    	Order by NgayLapHoaDon DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoI]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT
    		--a.ID_LoHang,
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end  + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang As ID_LoHang,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			ISNULL(td.TonKho, 0) AS TonKho
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    					SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				WHERE dvqd.ladonvichuan = '1'
    					GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    					-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                            
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang,hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    		) a
    	    LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    		where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    order by GiaTriCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    
    	SELECT 		
    		a.ID_LoHang,
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang As ID_LoHang,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongNhap2,0)) - SUM(ISNULL(td.SoLuongXuat2,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    						SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian bắt đầu đến khi chốt sổ
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap1,
    				null AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS SoLuongNhap2,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS SoLuongXuat1,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap2,
    				null AS SoLuongXuat2,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    						and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by GiaTriCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoIII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    
    	SELECT 
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			--NULL as TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(td.TonKho,0) + ISNULL(td.SoLuongXuat,0)) - SUM(ISNULL(td.SoLuongNhap,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi,td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				--NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				NULL AS TonCuoiKy
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    		and a.Xoa like @TrangThai
    order by GiaTriCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TonKhoIV]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT 
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		Case when a.TenNhomHangHoa is null or a.TenNhomHangHoa = '' then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
    		dvqd3.mahanghoa,
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat
    			FROM
    			(
    
    				-- phát sinh xuất nhập tồn đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    			) a
    			LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    				left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    							) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    			where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    					and a.Xoa like @TrangThai
    			order by GiaTriCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT
    	dvqd.ID,
    	hh.ID_NhomHang,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND(a.SoLuong , 3) as float ) as SoLuong,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTri , 0) as float ) else 0 end as GiaTri
    	FROM
    	(
    			SELECT
    				b.ID_DonViQuiDoi as ID_DonViQuiDoi,
    					b.ID_LoHang,
    				SUM(ISNULL(b.SoLuong, 0)) as SoLuong,
    				SUM(ISNULL(b.GiaTri, 0)) as GiaTri
    			FROM
    			(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				ISNULL(hdct.SoLuong,0) as SoLuong,
    				ISNULL(hdct.ThanhTien, 0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.LoaiHoaDon = '10' and hd.YeuCau = '1')
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    
    			Union ALL
    				SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
    				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				ISNULL(hdct.tienchietkhau,0) as SoLuong,
    				ISNULL(hdct.ThanhTien, 0) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.ID_CheckIn is not null 
    				and hd.LoaiHoaDon = '10' and hd.YeuCau = '4')
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			) b
    			GROUP BY b.ID_DonViQuiDoi, ID_LoHang
    	) a
    	inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    		where (dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    		and hh.LaHangHoa like  @LaHangHoa
    	ORDER BY MaHangHoa DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatChuyenHangChiTiet]", parametersAction: p => new
            {
                ID_ChiNhanh = p.String(),
                ID_DonViQuiDoi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    Select 
    		bhhd.MaHoaDon,
    		bhhd.NgayLapHoaDon,
    			dvn.TenDonVi as TuDonVi,
    		dv.TenDonVi as ToiDonVi,
    		CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
    		Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(hdct.DonGia, 0), 0) as float) else 0 end as DonGia,
    		Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float) else 0 end as ThanhTien,
    		Case when bhhd.YeuCau = '1' then N'Đang chuyển hàng' else N'Đã chuyển hàng' end as TrangThai
    	From
    	BH_HoaDon bhhd 
    	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
    	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
    	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
    	where hdct.ID_DonViQuiDoi = @ID_DonViQuiDoi
    	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
    	and ((bhhd.loaihoadon = '10' and bhhd.YeuCau = '1' 
    		and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))) or
    	(bhhd.ID_CheckIn is not null 
    		--and bhhd.ID_CheckIn != @ID_ChiNhanh 
    		and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4' 
    		and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))))
    	Order by NgayLapHoaDon DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatHuy]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	hh.ID_NhomHang,
    	CAST(ROUND(a.TongSoLuongHuy , 3) as float) AS TongSoLuongHuy,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.TongGiaTriHuy , 0) as float ) else 0 end AS TongGiaTriHuy
    	FROM
    	(
    		SELECT 
    		dvqd.ID as ID_DonViQuiDoi,
    			Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    		SUM(ISNULL(hdct.SoLuong, 0)) as TongSoLuongHuy,
    		SUM(ISNULL(hdct.ThanhTien, 0)) as TongGiaTriHuy
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.LoaiHoaDon = 8
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    		and hh.LaHangHoa like @LaHangHoa
    		GROUP BY dvqd.ID, ID_LoHang
    	) a
    	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    	ORDER BY TongGiaTriHuy DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietI]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    	DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	 -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    
    	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
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
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    (
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
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
    				td.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				NULL AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi, 
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,                                                                                                                                                                            
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    	DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	 -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    
    	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri, 
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
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
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    (
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
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
    				td.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian bắt đầu tới khi chốt sổ
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
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
    				--SUM(ISNULL(td.TonKho + td.SoLuongXuat_Ban + td.SoLuongXuat_Huy + td.SoLuongXuat_NCC + td.SoLuongXuat_Kiem + td.SoLuongXuat_Chuyen   - td.SoLuongNhap_NCC - td.SoLuongNhap_Kiem - td.SoLuongNhap_Tra - td.SoLuongNhap_Chuyen, 0)) AS TonCuoiKy,
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietIII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    	DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	 -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    
    	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
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
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    (
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
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
    				td.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian kết thúc tới khi chốt sổ
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonChiTietIV]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, 
    	a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	dvqd3.TenDonViTinh, 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
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
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    	FROM 
    (
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa) AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '5' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '7' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '8' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '4' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    				WHERE bhd.LoaiHoaDon = '9' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				WHERE bhd.LoaiHoaDon = '6' AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
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
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonI]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa,
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then  CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    
    	FROM 
    	--select * FROM
    		(
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
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
    				td.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				NULL AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQuiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				WHERE dvqd.ladonvichuan = '1'
    					GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                    
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.TonKho, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    		) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    		where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    
    	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
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
    				td.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap1,
    				NULL AS GiaTriNhap1,
    				NULL AS SoLuongXuat1,
    				NULL AS GiaTriXuat1,
    				NULL AS SoLuongNhap2,
    				NULL AS GiaTriNhap2,
    				NULL AS SoLuongXuat2,
    				NULL AS GiaTriXuat2,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ thời gian bắt đầu đến khi chốt sổ
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
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
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonIII]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    
    	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
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
    				td.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				NULL AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					WHERE dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian kết thúc
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_XuatNhapTonIV]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    --DECLARE @timeChotSo Datetime
    --   Select @timeChotSo =  (select convert(datetime, '2018/01/01'))
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	SELECT a.ID as ID_HangHoa, dvqd3.ID as ID_DonViQuiDoi, a.ID_NhomHang, dvqd3.LaDonViChuan, dvqd3.mahanghoa, 
    		a.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	a.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonDauKy, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonDauKy / dvqd3.TyLeChuyenDoi) * dvqd3.Giavon, 0) as float) else 0 end as GiaTriDauKy,
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriNhap, 0) as float) else 0 end as GiaTriNhap,
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiaTriXuat, 0) as float) else 0 end as GiaTriXuat,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy,
    	Case When @XemGiaVon = '1' then CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi) * dvqd3.GiaVon, 0) as float) else 0 end as GiaTriCuoiKy
    
    	FROM 
    	--select * FROM
    (
    		SELECT 
    		dhh.ID,
    			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
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
    				td.ID_LoHang,
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
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeStart
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                          
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeStart
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    			UNION ALL
    				SELECT
    				pstk.ID_DonViQuiDoi,
    					pstk.ID_LoHang,
    				NULL AS TonDau,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS GiaTriNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap,
    				null AS SoLuongXuat,
    				NULL AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    				and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    		left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where (MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH) and LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    order by TonCuoiKy desc");

            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_LoiNhuan]", parametersAction: p => new
            {
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
	Set @XemGiaVon = (Select 
		Case when nd.LaAdmin = '1' then '1' else
		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
		From
		HT_NguoiDung nd	
		where nd.ID = @ID_NguoiDung)
    SELECT * FROM
    	(
    		SELECT
    			a.ID_KhachHang,
    			case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end AS MaKhachHang,
    			case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end AS TenKhachHang,
    			case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
    			case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
    			CAST(ROUND((a.TongTienHang), 0) as float) as TongTienHang,
    			CAST(ROUND((a.DoanhThu - a.TongTienHang), 0) as float) as GiamGiaHD,
    			CAST(ROUND((a.DoanhThu), 0) as float) as DoanhThu, 
    			CAST(ROUND((a.GiaTriTra * (-1)), 0) as float) as GiaTriTra,
    			CAST(ROUND((a.DoanhThu - a.GiaTriTra), 0) as float) as GiaTriThuan,
    			Case When @XemGiaVon = '1' then CAST(ROUND((a.TongGiaVonBan - a.TongGiaVonTra), 0) as float) else 0 end as TongGiaVon,
    			Case When @XemGiaVon = '1' then CAST(ROUND((a.DoanhThu - a.GiaTriTra - a.TongGiaVonBan + a.TongGiaVonTra), 0) as float) else 0 end as LoiNhuanGop,
    			Case When dtn.ID_NhomDoiTuong is null then '00000000-0000-0000-0000-000000000000' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
    		FROM
    		(
    			SELECT
    				NCC.ID_KhachHang,
    				SUM(ISNULL(NCC.DoanhThu, 0)) as DoanhThu,
    				SUM(ISNULL(NCC.GiaTriTra, 0)) as GiaTriTra,
    				SUM(ISNULL(NCC.TongTienHang, 0)) as TongTienHang,
    				SUM(ISNULL(NCC.TongGiaVonBan, 0)) as TongGiaVonBan,
    				SUM(ISNULL(NCC.TongGiaVonTra, 0)) as TongGiaVonTra
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
    				NULL AS GiaTriTra,
    				SUM(ISNULL(hd.TongTienHang, 0)) as TongTienHang, 
    				NULL AS TongGiaVonBan,
    				NULL AS TongGiaVonTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 1
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				NULL AS DoanhThu,
    				SUM(ISNULL(hd.PhaiThanhToan, 0)) as GiaTriTra,
    				NULL as TongTienHang, 
    				NULL AS TongGiaVonBan,
    				NULL AS TongGiaVonTra
    				FROM
    				BH_HoaDon hd 
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 6
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				NULL AS DoanhThu,
    				NULL as GiaTriTra,
    				NULL as TongTienHang, 
    				SUM(ISNULL(hdct.SoLuong * hdct.GiaVon, 0)) AS TongGiaVonBan,
    				NULL AS TongGiaVonTra
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 1
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong
    
    				UNION ALL
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				NULL AS DoanhThu,
    				NULL as GiaTriTra,
    				NULL as TongTienHang, 
    				NULL AS TongGiaVonBan,
    				SUM(ISNULL(hdct.SoLuong * hdct.GiaVon, 0)) AS TongGiaVonTra
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 6
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @MaKH or dt.MaDoiTuong is NULL)
    				GROUP BY hd.ID_DoiTuong 
    			) AS NCC
    			Group by NCC.ID_KhachHang
    		) a
    		left join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    	) b
    	where MaKhachHang like @MaKH or TenDoiTuong_KhongDau like @MaKH or TenDoiTuong_ChuCaiDau like @MaKH
    	ORDER BY LoiNhuanGop DESC");

            CreateStoredProcedure(name: "[dbo].[SP_GetChiTietHoaDon_ByIDHoaDon]", parametersAction: p => new
            {
                ID_HoaDon = p.String()
            }, body: @"SELECT 
		BH_HoaDon_ChiTiet.ID,BH_HoaDon_ChiTiet.ID_HoaDon,DonGia,BH_HoaDon_ChiTiet.GiaVon,SoLuong,ThanhTien,ID_DonViQuiDoi,
		BH_HoaDon_ChiTiet.TienChietKhau AS GiamGia,PTChietKhau,ThoiGian,BH_HoaDon_ChiTiet.GhiChu,
		CAST(SoThuTu AS float) AS SoThuTu,BH_HoaDon_ChiTiet.ID_KhuyenMai,
		DM_HangHoa.ID AS ID_HangHoa,LaHangHoa,QuanLyTheoLoHang,TenHangHoa,		
		TenDonViTinh,MaHangHoa,TyLeChuyenDoi,YeuCau,MaLoHang,
		DM_LoHang.ID AS ID_LoHang,ISNULL(MaLoHang,'') as MaLoHang,
		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri
		FROM BH_HoaDon
		JOIN BH_HoaDon_ChiTiet ON BH_HoaDon.ID= BH_HoaDon_ChiTiet.ID_HoaDon
		JOIN DonViQuiDoi ON BH_HoaDon_ChiTiet.ID_DonViQuiDoi = DonViQuiDoi.ID
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
		LEFT JOIN DM_LoHang ON BH_HoaDon_ChiTiet.ID_LoHang = DM_LoHang.ID
		--where MaLoHang !=''
		WHERE BH_HoaDon_ChiTiet.ID_HoaDon LIKE '%'+ @ID_HoaDon + '%'");

            CreateStoredProcedure(name: "[dbo].[SP_GetSoLuongHangMua_ofKhachHang]",
                body: @"Select 
a.ID_DoiTuong,
a.ID_DonViQuiDoi,
Sum(a.soluongMua - a.SoLuongTra) as SoLuong
FROM
(
		select ID_DoiTuong,
		ID_DonViQuiDoi,
		0 as SoLuongTra,
		 SUM(ct.soluong) as soluongMua 
		from BH_HoaDon hd
		join BH_HoaDon_ChiTiet ct on  hd.ID = ct.ID_HoaDon
		where ID_DoiTuong is not null and LoaiHoaDon=1
		group by hd.ID_DoiTuong, ct.ID_DonViQuiDoi
	Union all
		select ID_DoiTuong, 
		 ID_DonViQuiDoi,
		SUM(ct.soluong) as soluongTra, 
		0 as SoLuongMua
		from BH_HoaDon hd
	join BH_HoaDon_ChiTiet ct on  hd.ID = ct.ID_HoaDon
	where ID_DoiTuong is not null and LoaiHoaDon=6
	group by hd.ID_DoiTuong,ct.ID_DonViQuiDoi
	) as a
	Group by a.ID_DoiTuong, a.ID_DonViQuiDoi having (SUM(a.soluongMua - a.soluongTra)) >=0");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GetChiTietHoaDon_ByIDHoaDon]");
            DropStoredProcedure("[dbo].[SP_GetSoLuongHangMua_ofKhachHang]");
        }
    }
}