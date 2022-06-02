namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_CauHinhPhanMem : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[Report_DatHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
    	dvqd.ID as ID_DonViQuiDoi,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end + 
		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	CAST(Round(a.SoLuongDat, 3) as float) as SoLuongDat,
    		CAST(Round(a.TongTienHang, 0) as float) as TongTienHang,
    		CAST(Round(a.GiamGiaHD, 0) as float) as GiamGiaHD,
    	CAST(Round(a.GiaTriDat, 0) as float) as GiaTriDat,
    	hh.ID_NhomHang
    	FROM
    	(
    			SELECT
    			ID_DonViQuiDoi,
    			ID_DoiTuong, 
				ID_LoHang,
    			Sum(ISNULL(SoLuongDat, 0)) as SoLuongDat,
    				Sum(ISNULL(TongTienHang, 0)) as TongTienHang,
    				Sum(ISNULL(GiamGiaHD, 0)) as GiamGiaHD,
    			Sum(ISNULL(GiaTriDat, 0)) as GiaTriDat
    			FROM
    			(
    			SELECT
    				dvqd.ID as ID_DonViQuiDoi,
					Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				Case when dt.MaDoiTuong is null then 'Khách lẻ' else dt.MaDoiTuong end as MaDoiTuong,
    				Case when dt.TenDoiTuong_KhongDau is null then 'khach le' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
    				Case when dt.TenDoiTuong_ChuCaiDau is null then 'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
    				Case when hd.ID_DoiTuong is null then '00000000-0000-0000-0000-000000000000' else hd.ID_DoiTuong end as ID_DoiTuong,
    				ISNULL(hdct.SoLuong, 0) as SoLuongDat,
    					ISNULL(hdct.ThanhTien, 0) as TongTienHang,
    					ISNULL(hdct.ThanhTien, 0) * ISNULL(hd.TongGiamGia / hd.TongTienHang, 0) as GiamGiaHD,
    				ISNULL(hdct.ThanhTien, 0) * (1- ISNULL(hd.TongGiamGia / hd.TongTienHang, 0)) as GiaTriDat
    			FROM
    			BH_HoaDon_ChiTiet hdct
    			inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
    			and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
    			and hh.LaHangHoa like @LaHangHoa
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    				and hd.TongTienHang != 0
    			) b
    		where MaDoiTuong like @maKH or TenDoiTuong_KhongDau like @maKH or TenDoiTuong_ChuCaiDau like @maKH
    
    		GROUP BY ID_DonViQuiDoi, ID_DoiTuong, ID_LoHang
    	) a
    		left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    	left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	left join DM_DoiTuong dt on a.ID_DoiTuong = dt.ID
			left join DM_LoHang lh on a.ID_LoHang = lh.ID
    		left join (Select Main.id_hanghoa,
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
    		order by mahanghoa, GiaTriDat desc");

            AlterStoredProcedure(name: "[dbo].[Report_DatHang_GiaoDichChiTiet]", parametersAction: p => new
            {
                MaHD = p.String(),
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT
	a.ID_NhomHang,
	a.MaHangHoa,
	a.TenHangHoaFull,
	a.TenHangHoa,
	a.ThuocTinh_GiaTri,
	a.TenDonViTinh,
	a.TenLoHang,
	CAST(ROUND(a.SoLuongDat, 3) as float) as SoLuongDat,
	CAST(ROUND(ISNULL(a.SoLuongNhan, 0), 3) as float) as SoLuongNhan
	FROM
	(
	SELECT
	hd.ID as ID_HoaDon,
	hh.ID_NhomHang,
	dvqd.MaHangHoa,
	hh.TenHangHoa +
	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end  + 
	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
	hh.TenHangHoa,
	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
	ISNULL(hdct.SoLuong, 0) as SoLuongDat,
		(Select SUM(hdctx.SoLuong) from BH_HoaDon hdx
		inner join BH_HoaDon_ChiTiet hdctx on hdx.ID = hdctx.ID_HoaDon
		where hdx.LoaiHoaDon = 1 
		and hdctx.ID_DonViQuiDoi = dvqd.ID
		and hdx.ID_HoaDon = hd.ID
		and hdx.ChoThanhToan = 0
		group by ID_DonViQuiDoi
		) as SoLuongNhan
	FROM
	BH_HoaDon_ChiTiet hdct
	inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
	left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
	left join (Select Main.id_hanghoa,
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
	where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
	and hd.ChoThanhToan = 0
	and hd.MaHoaDon like @MaHD
	and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
	and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
	) a");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_BanHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
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
    	CAST(ROUND(a.GiaTriBan , 0) as float ) as GiaTriBan,
    	CAST(ROUND(a.SoLuongTra , 3) as float ) as SoLuongTra,
    	CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
    	CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan
    	FROM
    	(
    		SELECT 
    		dvqd.ID as ID_DonViQuiDoi,
			Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    		SUM(Case when hd.LoaiHoaDon = 1 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongBan,
    		SUM(Case when hd.LoaiHoaDon = 6 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongTra,
    			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
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
    	ORDER BY DoanhThuThuan DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_HoaDonNhapChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"Select 
    		hh.ID_NhomHang,
    	bhhd.MaHoaDon,
    	bhhd.NgayLapHoaDon,
    		dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		dvn.TenDonVi as ChiNhanhChuyen,
    	dv.TenDonVi as ChiNhanhNhan,
    	CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
    	CAST(ROUND(ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0), 0) as float) as DonGia,
    	CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float) as ThanhTien
    	From
    	BH_HoaDon bhhd 
    	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
    	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
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
    	where (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    and hh.LaHangHoa like @LaHangHoa
    	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
    		and bhhd.ChoThanhToan = 0
    	and (bhhd.ID_CheckIn is not null and bhhd.ID_CheckIn in (select * from splitstring(@ID_ChiNhanh)) 
    		and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4')
    	Order by NgayLapHoaDon DESC, ThanhTien DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_HoaDonXuatChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"Select 
    		hh.ID_NhomHang,
    	bhhd.MaHoaDon,
    	bhhd.NgayLapHoaDon,
    		dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		dvn.TenDonVi as ChiNhanhChuyen,
    	dv.TenDonVi as ChiNhanhNhan,
    	CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3) as float) as SoLuong,
    	CAST(ROUND(ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0), 0) as float) as DonGia,
    	CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0) as float) as ThanhTien
    	From
    	BH_HoaDon bhhd 
    	inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	left join DM_DonVi dv on bhhd.ID_CheckIn = dv.ID
    	left join DM_DonVi dvn on bhhd.ID_DonVi = dvn.ID
		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    	where (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    and hh.LaHangHoa like @LaHangHoa
    	and bhhd.NgayLapHoaDon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd
    		and bhhd.ChoThanhToan = 0
    	and ((bhhd.loaihoadon = '10' and bhhd.YeuCau = '1' and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))) or
    	(bhhd.ID_CheckIn is not null --and bhhd.ID_CheckIn != @ID_ChiNhanh 
    		and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4' 
    		and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))))
    	Order by NgayLapHoaDon DESC, ThanhTien DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_NhaCungCap]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
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
    	a.SoLuongNhaCungCap,
    	CAST(ROUND(a.SoLuongSanPham , 3) as float ) as SoLuongSanPham,
    	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
    	FROM
    	(
    		SELECT 
    		c.ID_DonViQuiDoi,
			c.ID_LoHang,
    		COUNT(*) as SoLuongNhaCungCap,
    		SUM(ISNULL(c.SoLuongSanPham, 0)) as SoLuongSanPham,
    		SUM(ISNULL(c.GiaTri, 0)) as GiaTri
    		FROM
    		(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
    			SUM(ISNULL(hdct.ThanhTien, 0) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.LoaiHoaDon = 4
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and hh.LaHangHoa like @LaHangHoa
    				and hd.TongTienHang > 0
    			GROUP BY dvqd.ID, hd.ID_DoiTuong, ID_LoHang
    		) c
    		GROUP BY ID_DonViQuiDoi, ID_LoHang
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
    	ORDER BY SoLuongNhaCungCap DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_NhanVien]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
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
    	a.SoLuongNhanVien,
    	CAST(ROUND(a.SoLuongban , 3) as float ) as SoLuongBan,
    	CAST(ROUND(a.GiaTri , 0) as float ) as GiaTri
    	FROM
    	(
    		SELECT 
    		c.ID_DonViQuiDoi,
			c.ID_LoHang,
    		COUNT(*) as SoLuongNhanVien,
    		SUM(ISNULL(c.SoLuongBan, 0)) as SoLuongBan,
    		SUM(ISNULL(c.GiaTri, 0)) as GiaTri
    		FROM
    		(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongBan,
    			SUM(ISNULL(hdct.ThanhTien, 0) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0))) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.LoaiHoaDon = 1
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and hh.LaHangHoa like @LaHangHoa
    				and hd.TongTienHang > 0
    			GROUP BY dvqd.ID, hd.ID_NhanVien,ID_LoHang
    		) c
    		GROUP BY ID_DonViQuiDoi, ID_LoHang
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
    	ORDER BY SoLuongNhanVien DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_NhapHangChiTietNCC]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
    	hd.MaHoaDon,
    	hd.NgayLapHoaDon,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end  + 
		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    	hh.ID_NhomHang,
    	CAST(ROUND(hdct.SoLuong , 3) as float) AS SoLuong,
    		CAST(ROUND(hdct.DonGia , 3) as float) AS DonGia,
    	CAST(ROUND(hdct.ThanhTien , 0) as float ) AS GiaTri
    	FROM
    	BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    	where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    	and hd.LoaiHoaDon = 4
    	and hd.ChoThanhToan = 0
    	and hd.ID_DonVi  in (select * from splitstring(@ID_ChiNhanh))
    	and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    	and hh.LaHangHoa like @LaHangHoa
    	ORDER BY NgayLapHoaDon DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_NhapHangNCC]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
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
    	CAST(ROUND(a.SoLuong , 3) as float) AS SoLuong,
    	CAST(ROUND(a.GiaTri , 0) as float ) AS GiaTri
    	FROM
    	(
    		SELECT 
    		dvqd.ID as ID_DonViQuiDoi,
			Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuong,
    		SUM(ISNULL(hdct.ThanhTien, 0)) as GiaTri
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.LoaiHoaDon = 4
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi  in (select * from splitstring(@ID_ChiNhanh))
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
    	ORDER BY GiaTri DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopNhapKho]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT 
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
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap
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
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap
    		FROM
    		(
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    				SELECT
    				pstk.ID_DonViQuiDoi,
					pstk.ID_LoHang,
    				pstk.LoaiHoaDon,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
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
    order by GiaTriNhap desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopNhapKhoChiTiet]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT 
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		a.TenLoaiChungTu,
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
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
    	CAST(ROUND((a.SoLuongNhap / dvqd3.TyLeChuyenDoi) , 3) as float) as SoLuongNhap, 
    	CAST(ROUND(a.GiaTriNhap, 0) as float) as GiaTriNhap
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			HangHoa.TenLoaiChungTu,
    			HangHoa.MaHoaDon,
    			Max(HangHoa.NgayLapHoaDon) AS NgayLapHoaDon,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
    		SUM(ISNULL(HangHoa.GiaTriNhap, 0))  AS GiaTriNhap
    		FROM
    		(
    			-- phát sinh xuất nhập trong khoảng thời gian truy vấn
    				SELECT
    				pstk.ID_DonViQuiDoi,
					pstk.ID_LoHang,
    				pstk.LoaiHoaDon,
    					ct.TenLoaiChungTu,
    					pstk.MaHoaDon,
    					Max(pstk.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(pstk.SoLuongNhap) AS SoLuongNhap,
    				SUM(pstk.GiaTriNhap) AS GiaTriNhap
    				FROM 
    				(
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6') 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi * dvqd.GiaVon) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE bhd.LoaiHoaDon = '9' 
    				AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriNhap
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    				LEFT JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, ct.TenLoaiChungTu, pstk.MaHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    				and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, HangHoa.TenLoaiChungTu, HangHoa.MaHoaDon, dvqd.Xoa, HangHoa.ID_LoHang
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
    order by NgayLapHoaDon desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopXuatKho]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT 
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
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat
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
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat
    		FROM
    		(
    				SELECT
    				pstk.ID_DonViQuiDoi,
					pstk.ID_LoHang,
    					pstk.LoaiHoaDon,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
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
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
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
    order by GiaTriXuat desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TongHopXuatKhoChiTiet]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String(),
                LoaiChungTu = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT 
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		a.TenLoaiChungTu,
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
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
    	CAST(ROUND((a.SoLuongXuat / dvqd3.TyLeChuyenDoi), 3) as float) as SoLuongXuat,
    	CAST(ROUND(a.GiaTriXuat, 0) as float) as GiaTriXuat
    	FROM 
    	--select * FROM
    		(
    			SELECT 
    		dhh.ID,
			HangHoa.ID_LoHang,
    		dhh.ID_NhomHang,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			HangHoa.TenLoaiChungTu,
    			HangHoa.MaHoaDon,
    			Max(HangHoa.NgayLapHoaDon) AS NgayLapHoaDon,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
    		MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
    		SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
    		SUM(ISNULL(HangHoa.GiaTriXuat, 0))  AS GiaTriXuat
    		FROM
    		(
    				SELECT
    				pstk.ID_DonViQuiDoi,
					pstk.ID_LoHang,
    					pstk.LoaiHoaDon,
    					ct.TenLoaiChungTu,
    					pstk.MaHoaDon,
    					Max(pstk.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(pstk.SoLuongXuat) AS SoLuongXuat,
    				SUM(pstk.GiaTriXuat) AS GiaTriXuat
    				FROM 
    				(
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				SUM(ISNULL(bhdct.ThanhTien, 0)) AS GiaTriXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    					bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					bhd.LoaiHoaDon,
    					Max(bhd.NgayLapHoaDon) AS NgayLapHoaDon,
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
    				GROUP BY bhdct.ID_DonViQuiDoi, bhd.LoaiHoaDon, bhd.MaHoaDon, ID_LoHang, hh.QuanLyTheoLoHang
    			) AS pstk
    				LEFT JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
    			GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, ct.TenLoaiChungTu, pstk.MaHoaDon, pstk.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    				and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, HangHoa.TenLoaiChungTu, HangHoa.MaHoaDon, dvqd.Xoa, HangHoa.ID_LoHang
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
    order by NgayLapHoaDon desc");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TraHangNhap]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
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
    	CAST(ROUND(a.SoLuong , 3) as float) AS SoLuong,
    	CAST(ROUND(a.GiaTri , 0) as float ) AS GiaTri
    	FROM
    	(
    		SELECT 
    		dvqd.ID as ID_DonViQuiDoi,
			Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuong,
    		SUM(ISNULL(hdct.ThanhTien, 0)) as GiaTri
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.LoaiHoaDon = 7
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi  in (select * from splitstring(@ID_ChiNhanh))
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
    	ORDER BY GiaTri DESC");

            AlterStoredProcedure(name: "[dbo].[ReportHangHoa_TraHangNhapChiTiet]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
    	hd.MaHoaDon,
    	hd.NgayLapHoaDon,
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
    	CAST(ROUND(hdct.SoLuong , 3) as float) AS SoLuong,
    		CAST(ROUND(hdct.DonGia , 3) as float) AS DonGia,
    	CAST(ROUND(hdct.ThanhTien , 0) as float ) AS GiaTri
    	FROM
    	BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
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
    	where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    	and hd.LoaiHoaDon = 7
    	and hd.ChoThanhToan = 0
    	and hd.ID_DonVi  in (select * from splitstring(@ID_ChiNhanh))
    	and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    	and hh.LaHangHoa like @LaHangHoa
    	ORDER BY NgayLapHoaDon DESC");

            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_MuaHangChiTiet]", parametersAction: p => new
            {
                ID_KhachHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"IF (@ID_KhachHang != '')
    	BEGIN
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
    		CAST(ROUND(a.SoLuongMua,3) as float) as SoLuongMua,
    		CAST(ROUND(a.GiaTriMua, 0) as float) as GiaTriMua,
    		CAST(ROUND(a.SoLuongTra,3) as float) as SoLuongTra,
    		CAST(ROUND(a.GiaTriTra * (-1), 0) as float) as GiaTriTra,
    		CAST(ROUND(a.GiaTriMua - a.GiaTriTra, 0) as float) as GiaTriThuan
    		FROM
    		 (
    			SELECT 
    			KH.ID,
				KH.ID_LoHang,
    			SUM(ISNULL(KH.SoLuongMua, 0)) as SoLuongMua,
    			SUM(ISNULL(KH.SoLuongTra, 0)) as SoLuongTra,
    			SUM(ISNULL(KH.GiaTriMua, 0)) as GiaTriMua,
    			SUM(ISNULL(KH.GiaTriTra, 0)) as GiaTriTra
    			FROM
    			(
    				SELECT 
    				dvqd.ID,
					Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongMua,
    				NULL AS SoLuongTra,
    				SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.SoLuong * hdct.TienChietKhau, 0)) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriMua,
    				NULL As GiaTriTra
    				FROM
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.ID_DoiTuong = @ID_KhachHang
    				and hd.ChoThanhToan = 0
    				and hd.loaihoadon = 1 
    					and hd.TongTienHang != 0
    				GROUP BY dvqd.ID, ID_LoHang
    				UNION ALL
    				SELECT 
    				dvqd.ID,
					Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				NULL as SoLuongMua,
    				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongTra,
    				NULL As GiaTriMua,
    				SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.SoLuong * hdct.TienChietKhau, 0)) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriTra
    				FROM
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.ID_DoiTuong = @ID_KhachHang
    				and hd.ChoThanhToan = 0
    				and hd.loaihoadon = 6
    					and hd.TongTienHang != 0
    				GROUP BY dvqd.ID, ID_LoHang
    			) as KH
    			GROUP BY KH.ID, KH.ID_LoHang
    		) a
    		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
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
    		ORDER BY GiaTriThuan DESC
    	END
    	ELSE
    	BEGIN
    		SELECT
    		dvqd.MaHangHoa,
    		hh.TenHangHoa +
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    		hh.TenHangHoa,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		CAST(ROUND(a.SoLuongMua,3) as float) as SoLuongMua,
    		CAST(ROUND(a.GiaTriMua, 0) as float) as GiaTriMua,
    		CAST(ROUND(a.SoLuongTra,3) as float) as SoLuongTra,
    		CAST(ROUND(a.GiaTriTra * (-1), 0) as float) as GiaTriTra,
    		CAST(ROUND(a.GiaTriMua - a.GiaTriTra, 0) as float) as GiaTriThuan
    		FROM
    		 (
    			SELECT 
    			KH.ID,
				KH.ID_LoHang,
    			SUM(ISNULL(KH.SoLuongMua, 0)) as SoLuongMua,
    			SUM(ISNULL(KH.SoLuongTra, 0)) as SoLuongTra,
    			SUM(ISNULL(KH.GiaTriMua, 0)) as GiaTriMua,
    			SUM(ISNULL(KH.GiaTriTra, 0)) as GiaTriTra
    			FROM
    			(
    				SELECT 
    				dvqd.ID,
					Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongMua,
    				NULL AS SoLuongTra,
    				SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriMua,
    				NULL As GiaTriTra
    				--SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
    				FROM
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.ID_DoiTuong is null
    				and hd.ChoThanhToan = 0
    				and hd.loaihoadon = 1 
    					and hd.TongTienHang != 0
    				GROUP BY dvqd.ID, ID_LoHang
    				UNION ALL
    				SELECT 
    				dvqd.ID,
					Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    				NULL as SoLuongMua,
    				SUM(ISNULL(hdct.SoLuong, 0)) AS SoLuongTra,
    				NULL As GiaTriMua,
    				SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) As GiaTriTra
    				--SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
    				FROM
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.ID_DoiTuong is null
    				and hd.ChoThanhToan = 0
    				and hd.loaihoadon = 6
    					and hd.TongTienHang != 0
    				GROUP BY dvqd.ID, ID_LoHang
    			) as KH
    			GROUP BY KH.ID, KH.ID_LoHang
    		) a
    		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
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
    		ORDER BY GiaTriThuan DESC
    	END");

            AlterStoredProcedure(name: "[dbo].[ReportNCC_MuaHangChiTiet]", parametersAction: p => new
            {
                ID_NCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"IF (@ID_NCC != '')
    	BEGIN
    		SELECT
    		dvqd.MaHangHoa,
    		--case when dvqd.TenDonViTinh = '' or dvqd.TenDonViTinh is null then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
    			hh.TenHangHoa +
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    		hh.TenHangHoa,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		CAST(ROUND(a.SoLuongSanPham,3) as float) as SoLuongSanPham,
    		CAST(ROUND(a.GiaTri, 0) as float) as GiaTri
    		FROM
    		 (
    			SELECT 
    			dvqd.ID,
				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
    			SUM(ISNULL(hdct.ThanhTien, 0) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and hd.ID_DoiTuong = @ID_NCC
    			and hd.ChoThanhToan = 0
    			and hd.loaihoadon = 4
    			GROUP BY dvqd.ID, ID_LoHang
    		) a
    		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
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
    		ORDER BY GiaTri DESC
    	END
    	ELSE
    	BEGIN
    		SELECT
    		dvqd.MaHangHoa,
    		--case when dvqd.TenDonViTinh = '' then hh.TenHangHoa else hh.TenHangHoa + ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoa,
    			hh.TenHangHoa +
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    		hh.TenHangHoa,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		CAST(ROUND(a.SoLuongSanPham,3) as float) as SoLuongSanPham,
    		CAST(ROUND(a.GiaTri, 0) as float) as GiaTri
    		FROM
    		 (
    			SELECT 
    			dvqd.ID,
				Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
    			SUM(ISNULL(hdct.ThanhTien * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang,  0))), 0)) as GiaTri
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and hd.ID_DoiTuong is null
    			and hd.ChoThanhToan = 0
    			and hd.loaihoadon = 4
    			GROUP BY dvqd.ID, ID_LoHang
    		) a
    		left join DonViQuiDoi dvqd on a.ID = dvqd.ID
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
    		ORDER BY GiaTri DESC
    	END");

        }
        
        public override void Down()
        {
        }
    }
}