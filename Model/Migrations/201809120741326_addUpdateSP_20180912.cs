namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20180912 : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[insert_NhomHangHoa]", parametersAction: p => new
            {
                ID = p.Guid(),
                MaNhomHangHoa = p.String(),
                TenNhomHangHoa = p.String(),
                TenNhomHangHoa_KhongDau = p.String(),
                TenNhomHangHoa_KyTuDau = p.String(),
                timeCreate = p.DateTime()
            }, body: @"Insert into DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, laNhomhanghoa, HienThi_Chinh, HienThi_Phu, HienThi_BanThe, NguoiTao, NgayTao)
    	values(@ID, @MaNhomHangHoa, @TenNhomHangHoa,@TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, '1', '1', '1', '1','admin', @timeCreate) ");

            AlterStoredProcedure(name: "[dbo].[ReportNhanVien_BanHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"SELECT 
    	a.ID_NhanVien,
    	nv.TenNhanVien, 
    	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)), 0) as float) as DoanhThu,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriTra, 0)) * (-1), 0) as float) as GiaTriTra,
    	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.GiaTriTra, 0)), 0) as float) as DoanhThuThuan
    	FROM
    	(
    		SELECT 
    			nv.ID as ID_NhanVien,
    			Sum(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
    			NULL as GiaTriTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hd on nv.ID = hd.ID_NhanVien
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hd.LoaiHoaDon = 1
    			and nv.ID in (Select * from splitstring(@ID_NhanVien))
    		GROUP BY nv.ID
    		UNION ALL
    		SELECT
    			nv.ID as ID_NhanVien,
    			NULL as DoanhThu,
    			Sum((ISNULL(hdct.ThanhTien, 0)- ISNULL(hdct.TienChietKhau, 0)) * (1 - (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang,0)))) as GiaTriTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
    		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    			and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    			and nv.ID in (Select * from splitstring(@ID_NhanVien))
    				and hdt.TongTienHang != 0
    		GROUP BY nv.ID
    	) a
    	left join NS_NhanVien nv on a.ID_NhanVien = nv.ID
    	GROUP BY a.ID_NhanVien, nv.TenNhanVien");

            AlterStoredProcedure(name: "[dbo].[ReportNhanVien_LoiNhuan]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    SELECT 
    	a.ID_NhanVien,
    	nv.TenNhanVien, 
    	CAST(ROUND(SUM(ISNULL(a.TongTienHang, 0)), 0) as float) as TongTienHang,
    	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.TongTienHang, 0)), 0) as float) as GiamGiaHD,
    	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)), 0) as float) as DoanhThu,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriTra, 0)) * (-1), 0) as float) as GiaTriTra,
    	CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.GiaTriTra, 0)), 0) as float) as DoanhThuThuan,
    	Case When @XemGiaVon = '1' then CAST(ROUND(SUM(ISNULL(a.TongGiaVonBan, 0)) - SUM(ISNULL(a.TongGiaVonTra, 0)), 0) as float) else 0 end as TongGiaVon, 
    	Case When @XemGiaVon = '1' then CAST(ROUND(SUM(ISNULL(a.DoanhThu, 0)) - SUM(ISNULL(a.GiaTriTra, 0)) - SUM(ISNULL(a.TongGiaVonBan, 0)) + SUM(ISNULL(a.TongGiaVonTra, 0)), 0) as float) else 0 end as LoiNhuanGop
    	FROM
    	(
    		SELECT 
    			nv.ID as ID_NhanVien,
    			SUM(ISNULL(hd.TongTienHang, 0))  as TongTienHang,
    			NULL as GiamGiaHD,
    			SUM(ISNULL(hd.PhaiThanhToan, 0)) as DoanhThu,
    			NULL as GiaTriTra,
    			NULL as TongGiaVonBan,
    			NULL as TongGiaVonTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hd on nv.ID = hd.ID_NhanVien
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hd.LoaiHoaDon = 1
    			and nv.ID in (Select * from splitstring(@ID_NhanVien))
    		GROUP BY nv.ID
    
    		UNION ALL
    		SELECT
    			nv.ID as ID_NhanVien,
    			NULL as TongTienHang,
    			NULL as GiamGiaHD,
    			NULL as DoanhThu,
    			NULL as GiaTriTra,
    			SUM(ISNULL(hdct.SoLuong, 0) * (ISNULL(hdct.GiaVon, 0))) as TongGiaVonBan,
    			NULL as TongGiaVonTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon 
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    			and nv.ID in (Select * from splitstring(@ID_NhanVien))
    		GROUP BY nv.ID
    
    		UNION ALL
    		SELECT
    			nv.ID as ID_NhanVien,
    			NULL as TongTienHang,
    			NULL as GiamGiaHD,
    			NULL as DoanhThu,
    			Sum((ISNULL(hdct.ThanhTien, 0)- ISNULL(hdct.TienChietKhau, 0)) * (1 - (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang,0)))) as GiaTriTra,
    			NULL as TongGiaVonBan,
    			SUM(ISNULL(hdct.SoLuong, 0) * (ISNULL(hdct.GiaVon, 0))) as TongGiaVonTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
    		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    			and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    			and nv.ID in (Select * from splitstring(@ID_NhanVien))
    				and hdt.TongTienHang != 0
    		GROUP BY nv.ID
    	) a
    	left join NS_NhanVien nv on a.ID_NhanVien = nv.ID
    	GROUP BY a.ID_NhanVien, nv.TenNhanVien");

            AlterStoredProcedure(name: "[dbo].[ReportNhanVien_MuaHangChiTiet]", parametersAction: p => new
            {
                ID_NV = p.String(),
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
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
    	CAST(ROUND(a.SoLuongSanPham,3) as float) as SoLuong,
    	CAST(ROUND(a.GiaTri, 0) as float) as GiaTri
    	FROM
    		(
    		SELECT 
    		dvqd.ID,
    			Case when hdct.ID_LoHang is null then '10000000-0000-0000-0000-000000000001' else hdct.ID_LoHang end as ID_LoHang,
    		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham,
    		SUM((ISNULL(hdct.ThanhTien, 0) - ISNULL(hdct.TienChietKhau, 0)) * (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))) as GiaTri
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hd.ID_NhanVien = @ID_NV
    		and hd.ChoThanhToan = 0
    		and hd.loaihoadon = 1
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and hh.LaHangHoa like @LaHangHoa
				and hd.TongTienHang != 0
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
    	ORDER BY GiaTri DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_ChiTiet]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
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
    		a.TenNhanVien
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
    		Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0) else 0 end as GiaVon, 
    		nv.TenNhanVien,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
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
    			and hh.TheoDoi like @TheoDoi
    		and (hd.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    	order by a.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_HangTraLai]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHD = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	
    SELECT 
    	a.MaChungTuGoc, 
    		a.MaChungTu, 
    		a.NgayLapHoaDon,
    		a.MaHangHoa,
    		a.TenHangHoaFull,
    		a.TenHangHoa,
    		a.TenDonViTinh,
    		a.ThuocTinh_GiaTri,
    		a.TenLoHang,
    		CAST(ROUND(a.SoLuongTra, 0) as float) as SoLuong,
    		CAST(ROUND(a.GiaTriTra, 0) as float) as GiaTriTra,
    		a.TenNhanVien
    	FROM
    	(
    		SELECT
    				Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    				Case when hdb.ID is null then N'HD trả nhanh' else hdb.MaHoaDon end as MaChungTuGoc,
    				hdt.MaHoaDon as MaChungTu,
    				hdt.NgayLapHoaDon,
    				dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    				hh.TenHangHoa_KhongDau,
    				hh.TenHangHoa_KyTuDau,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    				ISNULL(hdct.SoLuong, 0) as SoLuongTra,
    			--ISNULL(hdct.ThanhTien, 0) *(1- ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang, 0))as GiaTriTra,
    				ISNULL(hdct.ThanhTien, 0) as GiaTriTra,
    				nv.TenNhanVien
    		FROM
    			BH_HoaDon hdt 
    		left join BH_HoaDon hdb on hdt.ID_HoaDon = hdb.ID
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    			left join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join NS_NhanVien nv on hdt.ID_NhanVien = nv.ID
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
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    			and hdt.ChoThanhToan = 0
    		and hdt.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdt.LoaiHoaDon = 6
    			and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.MaHangHoa like @MaHD or a.MaHangHoa like @MaHH or a.TenHangHoa_KhongDau like @MaHH or a.TenHangHoa_KyTuDau like @MaHH or a.MaChungTu like @MaHD)
    		ORDER BY a.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_LoiNhuan]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	SELECT *, 
    	Case When HH.DoanhThuThuan != 0 then CAST(ROUND((HH.LaiLo / HH.DoanhThuThuan) * 100, 2) as float ) else 0 end  as TySuat
    	FROM
    	(
    	SELECT 
    		MAX(b.MaHangHoa) as MaHangHoa,
    		MAX(b.TenHangHoaFull) as TenHangHoaFull,
    		MAX(b.TenHangHoa) as TenHangHoa,
    		MAX(b.TenDonViTinh) as TenDonViTinh,
    		MAX(b.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    	SUM(b.SoLuongBan) as SoLuongBan,
    	SUM(b.GiaTriBan) as ThanhTien,
    	SUM(b.SoLuongTra) as SoLuongTra,
    	SUM(b.GiaTriTra) as GiaTriTra,
    		SUM(b.GiamGiaHD) as GiamGiaHD,
    		CAST(ROUND(SUM(b.GiaTriBan) - SUM(b.GiaTriTra) -SUM(b.GiamGiaHD), 0) as float ) as DoanhThuThuan,
    	Case When @XemGiaVon = '1' then SUM(b.TongGiaVon) else 0 end as TienVon,
    		Case When @XemGiaVon = '1' then CAST(ROUND(SUM(b.GiaTriBan) - SUM(b.GiamGiaHD) - SUM(b.GiaTriTra) - SUM(b.TongGiaVon), 0) as float) else 0 end as LaiLo    	
    	FROM 
    	(
    SELECT 
    		dvqd.MaHangHoa,
    		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end + 
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    		hh.TenHangHoa_KhongDau,
    		hh.TenHangHoa_KyTuDau,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    		a.ID_LoHang,
    		a.ID_DonViQuiDoi,
    		a.ID_HoaDon,
    		Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		CAST(ROUND(SUM(ISNULL(a.GiamGiaHD, 0)), 0) as float) as GiamGiaHD,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongBan, 0)), 3) as float) as SoLuongBan,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriBan, 0)), 0) as float) as GiaTriBan,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongTra, 0)), 3) as float) as SoLuongTra,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriTra, 0)), 0) as float) as GiaTriTra,
    	CAST(ROUND((SUM(ISNULL(a.SoLuongBan, 0)) - SUM(ISNULL(a.SoLuongTra, 0))) * SUM(ISNULL(a.GiaVon * dvqd.TyLeChuyenDoi, 0)), 0) as float) as TongGiaVon    	
    	FROM
    	(
    		SELECT
    				hdb.ID as ID_HoaDon,
    				hdct.ID_DonViQuiDoi,
    				hdct.ID_LoHang,
					Case when hdb.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdb.TongGiamGia, 0) / ISNULL(hdb.TongTienHang, 0)) end as GiamGiaHD,
    				ISNULL(hdct.SoLuong, 0) as SoLuongBan,
    			ISNULL(hdct.ThanhTien, 0) as GiaTriBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVon,
    				NULL as SoLuongTra,
    			NULL as GiaTriTra
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon 
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    		UNION ALL
    		SELECT
    				hdb.ID as ID_HoaDon,
    				hdct.ID_DonViQuiDoi,
    				hdct.ID_LoHang,
					Case when hdt.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang, 0)) end as GiamGiaHD,
    			NULL as SoLuongBan,
    			NULL as GiaTriBan,
    			NULL as GiaVon,
    				ISNULL(hdct.SoLuong, 0) as SoLuongTra,
    			ISNULL(hdct.ThanhTien, 0) as GiaTriTra
    		FROM BH_HoaDon hdb
    		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    			and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
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
    		where hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    		and (dvqd.MaHangHoa like @MaHH_TV or dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    	GROUP BY a.ID_DonViQuiDoi, a.ID_HoaDon,a.ID_LoHang, hh.ID_NhomHang, dvqd.Xoa, dvqd.MaHangHoa, hh.TenHangHoa, hh.TenHangHoa_KhongDau, hh.TenHangHoa_KyTuDau, dvqd.TenDonViTinh, lh.MaLoHang, ThuocTinh.ThuocTinh_GiaTri
    		) b
    		where b.ID_NhomHang like @ID_NhomHang
    		and b.Xoa like @TrangThai
    		GROUP BY b.ID_DonViQuiDoi, b.ID_LoHang
    		) as HH
    		ORDER BY TySuat DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_NhomHang]", parametersAction: p => new
            {
                TenNhomHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuong, 
    		CAST(ROUND((Sum(a.ThanhTien)), 0) as float) as ThanhTien,
    		CAST(ROUND((Sum(a.SoLuong * a.GiaVon)), 0) as float) as TienVon,
    		CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD)), 0) as float) else 0 end as LaiLo
    	FROM
    	(
    		Select 
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    			Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    			Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    		Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0) else 0 end as GiaVon, 
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
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
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.TenNhomHangHoa_KhongDau like @TenNhomHang or a.TenNhomHangHoa_KyTuDau like @TenNhomHang)
    	Group by a.ID_NhomHang
    		OrDER BY LaiLo DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_TheoKhachHang]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomKhachHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    
    	SELECT
    		Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomKhachHang,
    		c.MaKhachHang, c.TenKhachHang, c.DienThoai, c.SoLuong, c.ThanhTien, c.TienVon, c.GiamGiaHD, c.LaiLo
    	  FROM 
    	(
    SELECT 
    		b.ID_KhachHang,
    	MAX(b.MaKhachHang) as MaKhachHang,
    	MAX(b.TenKhachHang) as TenKhachHang,
    	MAX(b.DienThoai) as DienThoai,
    	MAX(b.SoLuong) as SoLuong, 
    		MAX(b.ThanhTien) as ThanhTien, 
    		MAX(b.TienVon) as TienVon, 
    		MAX(b.GiamGiaHD) as GiamGiaHD, 
    		MAX(b.LaiLo) as LaiLo
    	 FROM
    	(
    		SELECT
    				a.ID_KhachHang,
    					Case When dtn.ID_NhomDoiTuong is null then '00000010-0000-0000-0000-000000000010' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end AS MaKhachHang,
    			case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end AS TenKhachHang,
    			case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end AS TenDoiTuong_KhongDau,
    			case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end AS TenDoiTuong_ChuCaiDau,
    			Case when dt.DienThoai is null then '' else dt.DienThoai end AS DienThoai,
    			a.SoLuong,
    				a.ThanhTien,
    				a.TienVon,
    				a.GiamGiaHD,
    				a.LaiLo
    		FROM
    		(
    			SELECT
    				NCC.ID_KhachHang,
    				CAST(ROUND((SUM(NCC.SoLuong)), 3) as float) as SoLuong, 
    				CAST(ROUND((Sum(NCC.ThanhTien)), 0) as float) as ThanhTien,
    				CAST(ROUND((Sum(NCC.SoLuong * NCC.GiaVon)), 0) as float) as TienVon,
    				CAST(ROUND((Sum(NCC.GiamGiaHD)), 0) as float) as GiamGiaHD,
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.ThanhTien - (NCC.SoLuong * NCC.GiaVon) - NCC.GiamGiaHD)), 0) as float) else 0 end as LaiLo
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_KhachHang,
    				ISNULL(hdct.SoLuong, 0) AS SoLuong,
    					ISNULL(hdct.ThanhTien, 0) AS ThanhTien,
    					Case when @XemGiaVon = '1' then ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0) else 0 end As GiaVon,
    					Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    					Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    					Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    					inner join DonViQuiDoi dvqd on dvqd.ID = hdct.ID_DonViQuiDoi
    					inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 1
    				and (dt.MaDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @maKH or dt.MaDoiTuong is NULL)
    					and hh.LaHangHoa like @LaHangHoa
    					and hh.TheoDoi like @TheoDoi
    			) AS NCC
    				Where NCC.Xoa like @TrangThai
    				and NCC.ID_NhomHang like @ID_NhomHang
    			Group by NCC.ID_KhachHang
    		) a
    		left join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    	) b
    	where ID_NhomDoiTuong in (select * from splitstring(@ID_NhomKhachHang)) and
    		(MaKhachHang like @MaKH_TV or MaKhachHang like @MaKH or TenDoiTuong_KhongDau like @MaKH or TenDoiTuong_ChuCaiDau like @MaKH or DienThoai like @MaKH)
    			Group by b.ID_KhachHang
    		) as c
    		left join DM_DoiTuong dt on c.ID_KhachHang = dt.ID
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
    	ORDER BY LaiLo DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_TheoNhanVien]", parametersAction: p => new
            {
                TenNhanVien = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	SELECT 
    		b.ID_NhanVien,
    	b.TenNhanVien, 
    	SUM(b.SoLuongBan) as SoLuongBan,
    	SUM(b.GiaTriBan) as ThanhTien,
    	SUM(b.SoLuongTra) as SoLuongTra,
    	SUM(b.GiaTriTra) as GiaTriTra,
    		SUM(b.GiamGiaHD) as GiamGiaHD,
    	Case When @XemGiaVon = '1' then SUM(b.TongGiaVon) else 0 end as TienVon,
    		Case When @XemGiaVon = '1' then CAST(ROUND(SUM(b.GiaTriBan) - SUM(b.GiamGiaHD) - SUM(b.GiaTriTra) - SUM(b.TongGiaVon), 0) as float) else 0 end as LaiLo    	
    	FROM 
    	(
    SELECT 
    	a.ID_NhanVien,
    	nv.TenNhanVien, 
    		a.ID_DonViQuiDoi,
    		a.ID_HoaDon,
    		Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		CAST(ROUND(SUM(ISNULL(a.GiamGiaHD, 0)), 0) as float) as GiamGiaHD,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongBan, 0)), 3) as float) as SoLuongBan,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriBan, 0)), 0) as float) as GiaTriBan,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongTra, 0)), 3) as float) as SoLuongTra,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriTra, 0)), 0) as float) as GiaTriTra,
    	CAST(ROUND((SUM(ISNULL(a.SoLuongBan, 0)) - SUM(ISNULL(a.SoLuongTra, 0))) * SUM(ISNULL(a.GiaVon * dvqd.TyLeChuyenDoi, 0)), 0) as float) as TongGiaVon    	
    	FROM
    	(
    		SELECT
    			nv.ID as ID_NhanVien,
    				hdb.ID as ID_HoaDon,
    				hdct.ID_DonViQuiDoi,
					Case when hdb.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdb.TongGiamGia, 0) / ISNULL(hdb.TongTienHang, 0)) end as GiamGiaHD,
    				ISNULL(hdct.SoLuong, 0) as SoLuongBan,
    				ISNULL(hdct.ThanhTien, 0) as GiaTriBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVon,
    				NULL as SoLuongTra,
    			NULL as GiaTriTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon 
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    		--GROUP BY nv.ID
    
    		UNION ALL
    		SELECT
    			nv.ID as ID_NhanVien,
    				hdb.ID as ID_HoaDon,
    				hdct.ID_DonViQuiDoi,
					Case when hdt.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang, 0)) end as GiamGiaHD,
    			NULL as SoLuongBan,
    			NULL as GiaTriBan,
    			NULL as GiaVon,
    				ISNULL(hdct.SoLuong, 0) as SoLuongTra,
    				ISNULL(hdct.ThanhTien, 0) as GiaTriTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
    		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    			and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    	) a
    	left join NS_NhanVien nv on a.ID_NhanVien = nv.ID
    		inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    	GROUP BY a.ID_NhanVien, nv.TenNhanVien, a.ID_DonViQuiDoi, a.ID_HoaDon, hh.ID_NhomHang, dvqd.Xoa
    		) b
    		where b.ID_NhomHang like @ID_NhomHang
    		and b.Xoa like @TrangThai
    		and b.TenNhanVien like @TenNhanVien
    		GROUP BY b.ID_NhanVien, b.TenNhanVien");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_TongHop]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                TenNhomHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    			a.MaHangHoa,
    			Max(a.TenHangHoaFull) as  TenHangHoaFull,
    			Max(a.TenHangHoa) as TenHangHoa,
    		Max(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    			Max(a.TenDonViTinh) as TenDonViTinh,
    			a.TenLoHang,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuong, 
    		CAST(ROUND((Sum(a.ThanhTien)), 0) as float) as ThanhTien,
    		CAST(ROUND((Sum(a.SoLuong * a.GiaVon)), 0) as float) as TienVon,
    			CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD)), 0) as float) else 0 end as LaiLo
    	FROM
    	(
    		Select 
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    			Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    			Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    				hh.TenHangHoa_KhongDau,
    				hh.TenHangHoa_KyTuDau,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    					Case When hdct.ID_LoHang is not null then hdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    			ISNULL(hdct.SoLuong, 0) as SoLuong,
    		ISNULL(hdct.DonGia, 0) - ISNULL(hdct.TienChietKhau, 0) as GiaBan,
    		Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0) else 0 end as GiaVon, 
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
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
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.MaHangHoa like @Text_Search or a.TenHangHoa_KhongDau like @Text_Search or a.TenHangHoa_KyTuDau like @Text_Search or a.TenNhomHangHoa_KhongDau like @TenNhomHang or a.TenNhomHangHoa_KyTuDau like @TenNhomHang or a.MaHangHoa like @MaHH)
    	Group by a.MaHangHoa, a.TenLoHang, a.ID_LoHang
    		OrDER BY LaiLo DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHangChiTiet_TheoNhanVien]", parametersAction: p => new
            {
                ID_NhanVien = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    	SELECT 
    		MAX(b.MaHangHoa) as MaHangHoa,
    		MAX(b.TenHangHoaFull) as TenHangHoaFull,
    		MAX(b.TenHangHoa) as TenHangHoa,
    		MAX(b.TenDonViTinh) as TenDonViTinh,
    		MAX (b.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(b.TenLoHang) as TenLoHang,
    	SUM(b.SoLuongBan) as SoLuongBan,
    	SUM(b.GiaTriBan) as ThanhTien,
    	SUM(b.SoLuongTra) as SoLuongTra,
    	SUM(b.GiaTriTra) as GiaTriTra,
    		SUM(b.GiamGiaHD) as GiamGiaHD,
    	Case When @XemGiaVon = '1' then SUM(b.TongGiaVon) else 0 end as TienVon,
    		Case When @XemGiaVon = '1' then CAST(ROUND(SUM(b.GiaTriBan) - SUM(b.GiamGiaHD) - SUM(b.GiaTriTra) - SUM(b.TongGiaVon), 0) as float) else 0 end as LaiLo    	
    	FROM 
    	(
    SELECT 
    		a.ID_DonViQuiDoi,
    		dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when a.MaLoHang is null then '' else '. Lô: ' + a.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when a.MaLoHang is null then '' else '. Lô: ' + a.MaLoHang end as TenLoHang,
    					Case When a.ID_LoHang is not null then a.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    		Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		CAST(ROUND(SUM(ISNULL(a.GiamGiaHD, 0)), 0) as float) as GiamGiaHD,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongBan, 0)), 3) as float) as SoLuongBan,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriBan, 0)), 0) as float) as GiaTriBan,
    	CAST(ROUND(SUM(ISNULL(a.SoLuongTra, 0)), 3) as float) as SoLuongTra,
    	CAST(ROUND(SUM(ISNULL(a.GiaTriTra, 0)), 0) as float) as GiaTriTra,
    	CAST(ROUND((SUM(ISNULL(a.SoLuongBan, 0)) - SUM(ISNULL(a.SoLuongTra, 0))) * SUM(ISNULL(a.GiaVon * dvqd.TyLeChuyenDoi, 0)), 0) as float) as TongGiaVon    	
    	FROM
    	(
    		SELECT
    				lh.MaLoHang,
    				lh.ID as ID_LoHang,
    			nv.ID as ID_NhanVien,
    				hdb.ID as ID_HoaDon,
    				hdct.ID_DonViQuiDoi,
					Case when hdb.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdb.TongGiamGia, 0) / ISNULL(hdb.TongTienHang, 0)) end as GiamGiaHD,
    				ISNULL(hdct.SoLuong, 0) as SoLuongBan,
    			ISNULL(hdct.ThanhTien, 0) as GiaTriBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVon,
    				NULL as SoLuongTra,
    			NULL as GiaTriTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon 
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    			and hdb.ID_NhanVien = @ID_NhanVien
    		UNION ALL
    		SELECT
    				lh.MaLoHang,
    				lh.ID as ID_LoHang,
    			nv.ID as ID_NhanVien,
    				hdb.ID as ID_HoaDon,
    				hdct.ID_DonViQuiDoi,
					Case when hdt.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hdt.TongGiamGia, 0) / ISNULL(hdt.TongTienHang, 0)) end as GiamGiaHD,
    			NULL as SoLuongBan,
    			NULL as GiaTriBan,
    			NULL as GiaVon,
    				ISNULL(hdct.SoLuong, 0) as SoLuongTra,
    			ISNULL(hdct.ThanhTien, 0) as GiaTriTra
    		FROM
    		NS_NhanVien nv
    		join BH_HoaDon hdb on nv.ID = hdb.ID_NhanVien
    		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    			and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 1
    			and hdb.ID_NhanVien = @ID_NhanVien
    	) a
    	left join NS_NhanVien nv on a.ID_NhanVien = nv.ID
    		inner join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
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
    		where hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    	GROUP BY a.ID_NhanVien, nv.TenNhanVien, a.ID_DonViQuiDoi, a.ID_HoaDon, hh.ID_NhomHang, dvqd.Xoa, a.MaLoHang, dvqd.MaHangHoa, hh.TenHangHoa, ThuocTinh.ThuocTinh_GiaTri, dvqd.TenDonViTinh, a.ID_LoHang
    		) b
    		where b.ID_NhomHang like @ID_NhomHang
    		and b.Xoa like @TrangThai
    		GROUP BY b.ID_DonViQuiDoi, b.ID_LoHang");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDatHang_ChiTiet]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"SELECT 
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
    			a.TenKhachHang,
    			a.MaHangHoa,
    			a.TenHangHoaFull,
    			a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    			a.TenDonViTinh,
    				a.TenLoHang,
    		CAST(ROUND((a.SoLuongDat), 3) as float) as SoLuongDat,
    		CAST(ROUND((a.TongTienHang), 0) as float) as TongTienHang,
    		CAST(ROUND((a.GiamGiaHD), 0) as float) as GiamGiaHD,
    			CAST(ROUND((a.GiaTriDat), 0) as float) as GiaTriDat,
    		CAST(ROUND((ISNULL(a.SoLuongNhan,0)), 3) as float) as SoLuongNhan,
    			a.TenNhanVien
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    			Case when dt.ID is not null then dt.TenDoiTuong else N'Khách lẻ' end as TenKhachHang,
    			Case when dt.ID is not null then dt.TenDoiTuong_KhongDau else N'khach le' end as TenKhachHang_KhongDau,
    			Case when dt.ID is not null then dt.TenDoiTuong_ChuCaiDau else N'kl' end as TenKhachHang_KyTuDau,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    				hh.TenHangHoa_KhongDau,
    				hh.TenHangHoa_KyTuDau,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    			(Select SUM(hdctx.SoLuong) from BH_HoaDon hdx
    		inner join BH_HoaDon_ChiTiet hdctx on hdx.ID = hdctx.ID_HoaDon
    		where hdx.LoaiHoaDon = 1 
    		and hdctx.ID_DonViQuiDoi = dvqd.ID
    		and hdx.ID_HoaDon = hd.ID
    		and hdx.ChoThanhToan = 0
    			and (hdctx.ID_LoHang = hdct.ID_LoHang or (hdctx.ID_LoHang is null and hdct.ID_LoHang is null))
    		group by ID_DonViQuiDoi, ID_LoHang
    		) as SoLuongNhan, 
    			ISNULL(hdct.SoLuong, 0) as SoLuongDat,
    			ISNULL(hdct.ThanhTien, 0) as TongTienHang,
				Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    			--ISNULL(hdct.ThanhTien, 0) * ISNULL(hd.TongGiamGia / hd.TongTienHang, 0) as GiamGiaHD,
    		Case when hd.TongTienHang = 0 then hdct.ThanhTien else ISNULL(hdct.ThanhTien, 0) * (1- ISNULL(hd.TongGiamGia / hd.TongTienHang, 0)) end as GiaTriDat,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    			nv.TenNhanVien,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
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
    		and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.MaHoaDon like @Text_Search or a.MaHangHoa like @Text_Search or a.MaHangHoa like @MaHH or a.TenHangHoa_KhongDau like @MaHH or a.TenHangHoa_KyTuDau like @MaHH or a.TenKhachHang_KhongDau like @MaHH or a.TenKhachHang_KyTuDau like @MaHH)
    	order by a.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDatHang_NhomHang]", parametersAction: p => new
            {
                TenNhomHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuongDat, 
    		CAST(ROUND((Sum(a.ThanhTien)), 0) as float) as ThanhTien,
    		CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) as GiamGiaHD,
    		CAST(ROUND((Sum(a.ThanhTien - a.GiamGiaHD)), 0) as float) as GiaTriDat,
    		CAST(ROUND((SUM(ISNULL(a.SoLuongNhan, 0))), 3) as float) as SoLuongNhan
    	FROM
    	(
    		Select 
    			Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    			Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    			Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    		--hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) as GiamGiaHD,
    			(Select SUM(hdctx.SoLuong) from BH_HoaDon hdx
    		inner join BH_HoaDon_ChiTiet hdctx on hdx.ID = hdctx.ID_HoaDon
    		where hdx.LoaiHoaDon = 1 
    		and hdctx.ID_DonViQuiDoi = dvqd.ID
    		and hdx.ID_HoaDon = hd.ID
    		and hdx.ChoThanhToan = 0
    			and (hdctx.ID_LoHang = hdct.ID_LoHang or (hdctx.ID_LoHang is null and hdct.ID_LoHang is null))
    		group by ID_DonViQuiDoi, ID_LoHang
    		) as SoLuongNhan
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
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
    		and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.TenNhomHangHoa_KhongDau like @TenNhomHang or a.TenNhomHangHoa_KyTuDau like @TenNhomHang)
    	Group by a.ID_NhomHang
    		OrDER BY GiaTriDat DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDatHang_TongHop]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH_TV = p.String(),
                TenNhomHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"SELECT
    	Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    	a.MaHangHoa,
    	Max(a.TenHangHoaFull) as  TenHangHoaFull,
    	Max(a.TenHangHoa) as TenHangHoa,
    	Max(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    	Max(a.TenDonViTinh) as TenDonViTinh,
    	a.TenLoHang,
    		CAST(ROUND((SUM(a.SoLuongDat)), 3) as float) as SoLuongDat, 
    		CAST(ROUND((SUM(a.TongTienHang)), 0) as float) as ThanhTien, 
    		CAST(ROUND((SUM(a.GiamGiaHD)), 0) as float) as GiamGiaHD, 
    		CAST(ROUND((SUM(a.GiaTriDat)), 0) as float) as GiaTriDat,
    		CAST(ROUND((SUM(ISNULL(a.SoLuongNhan, 0))), 3) as float) as SoLuongNhan
    	FROM
    	(
    	SELECT
    		dvqd.ID as ID_DonViQuiDoi,
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    		Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    		Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    		Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    		hh.TenHangHoa_KhongDau,
    		hh.TenHangHoa_KyTuDau,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
    			Case When hdct.ID_LoHang is not null then hdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    		ISNULL(hdct.SoLuong, 0) as SoLuongDat,
    			ISNULL(hdct.ThanhTien, 0) as TongTienHang,
				Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    			--ISNULL(hdct.ThanhTien, 0) * ISNULL(hd.TongGiamGia / hd.TongTienHang, 0) as GiamGiaHD,
    		Case when hd.TongTienHang = 0 then hdct.ThanhTien else ISNULL(hdct.ThanhTien, 0) * (1- ISNULL(hd.TongGiamGia / hd.TongTienHang, 0)) end as GiaTriDat,
    			(Select SUM(hdctx.SoLuong) from BH_HoaDon hdx
    		inner join BH_HoaDon_ChiTiet hdctx on hdx.ID = hdctx.ID_HoaDon
    		where hdx.LoaiHoaDon = 1 
    		and hdctx.ID_DonViQuiDoi = dvqd.ID
    		and hdx.ID_HoaDon = hd.ID
    		and hdx.ChoThanhToan = 0
    			and (hdctx.ID_LoHang = hdct.ID_LoHang or (hdctx.ID_LoHang is null and hdct.ID_LoHang is null))
    		group by ID_DonViQuiDoi, ID_LoHang
    		) as SoLuongNhan
    FROM
    	BH_HoaDon_ChiTiet hdct
    	inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
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
    	and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
    	and hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    	) a
    	where a.ID_NhomHang like @ID_NhomHang
    		and a.Xoa like @TrangThai
    		and (a.MaHangHoa like @MaHH_TV or a.MaHangHoa like @Text_Search or a.TenHangHoa_KhongDau like @Text_Search or a.TenHangHoa_KyTuDau like @Text_Search or a.TenNhomHangHoa_KhongDau like @TenNhomHang or a.TenNhomHangHoa_KyTuDau like @TenNhomHang)
    	GROUP BY a.MaHangHoa, a.TenLoHang, a.ID_LoHang
    	order by mahanghoa, GiaTriDat desc");

            Sql(@"Update DM_NhomHangHoa set TenNhomHangHoa_KyTuDau = '';
                    Update DM_NhomDoiTuong set TenNhomDoiTuong_KyTuDau = '';");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoBanHang_ChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoBanHang_HangTraLai]");
            DropStoredProcedure("[dbo].[BaoCaoBanHang_LoiNhuan]");
            DropStoredProcedure("[dbo].[BaoCaoBanHang_NhomHang]");
            DropStoredProcedure("[dbo].[BaoCaoBanHang_TheoKhachHang]");
            DropStoredProcedure("[dbo].[BaoCaoBanHang_TheoNhanVien]");
            DropStoredProcedure("[dbo].[BaoCaoBanHang_TongHop]");
            DropStoredProcedure("[dbo].[BaoCaoBanHangChiTiet_TheoNhanVien]");
            DropStoredProcedure("[dbo].[BaoCaoDatHang_ChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoDatHang_NhomHang]");
            DropStoredProcedure("[dbo].[BaoCaoDatHang_TongHop]");
        }
    }
}

