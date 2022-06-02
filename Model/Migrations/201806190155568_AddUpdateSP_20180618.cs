namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180618 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_TraHangNhapChiTiet]", parametersAction: p => new
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
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    	hh.ID_NhomHang,
    	CAST(ROUND(hdct.SoLuong , 3) as float) AS SoLuong,
		CAST(ROUND(hdct.DonGia , 3) as float) AS DonGia,
    	CAST(ROUND(hdct.ThanhTien , 0) as float ) AS GiaTri
    	FROM
    	BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
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
    	and hd.LoaiHoaDon = 7
    	and hd.ChoThanhToan = 0
    	and hd.ID_DonVi  in (select * from splitstring(@ID_ChiNhanh))
    	and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    	and hh.LaHangHoa like @LaHangHoa
    	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_NhapHangNCC]", parametersAction: p => new
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
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    	hh.ID_NhomHang,
    	CAST(ROUND(a.SoLuong , 3) as float) AS SoLuong,
    	CAST(ROUND(a.GiaTri , 0) as float ) AS GiaTri
    	FROM
    	(
    		SELECT 
    		dvqd.ID as ID_DonViQuiDoi,
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
    		GROUP BY dvqd.ID
    	) a
    	left join DonViQuiDoi dvqd on a.ID_DonViQuiDoi = dvqd.ID
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
    	ORDER BY GiaTri DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_NhapHangChiTietNCC]", parametersAction: p => new
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
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    	hh.ID_NhomHang,
    	CAST(ROUND(hdct.SoLuong , 3) as float) AS SoLuong,
		CAST(ROUND(hdct.DonGia , 3) as float) AS DonGia,
    	CAST(ROUND(hdct.ThanhTien , 0) as float ) AS GiaTri
    	FROM
    	BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
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

            AlterStoredProcedure(name: "[dbo].[getlist_HoaDon_afterTraHang]", parametersAction: p => new
            {
                MaHD = p.String(),
                ID_DonVi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"Select 
    	c.ID,
    	c.MaHoaDon,
    	c.ID_HoaDon,
    	c.ID_BangGia,
    	c.ID_NhanVien,
    	c.ID_DonVi,
    	c.NguoiTao,
    	c.DienGiai,
    	c.NgayLapHoaDon,
    	c.TenNhanVien,
    	c.ID_DoiTuong,
    	c.TenDoiTuong,
    	c.PhaiThanhToan,
    	c.TongTienHang,
    	c.TongGiamGia,
    	c.DiemGiaoDich

    	from
    	(
    		Select 
    		hd.ID,
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    		dt.DienThoai,
    		hd.ID_DoiTuong,	
    		hd.ID_HoaDon,
    		hd.ID_BangGia,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
    		hd.NguoiTao,	
    		hd.DienGiai,	
    
    		Case When nv.TenNhanVien != '' then nv.TenNhanVien else '' end as TenNhanVien,
    		Case When dt.TenDoiTuong != '' then dt.TenDoiTuong else N'Khách lẻ' end as TenDoiTuong,
    		Case When dt.TenDoiTuong_KhongDau != '' then dt.TenDoiTuong_KhongDau else 'khach le' end as TenDoiTuong_KhongDau,
    		Case When dt.TenDoiTuong_ChuCaiDau != '' then dt.TenDoiTuong_ChuCaiDau else 'kl' end as TenDoiTuong_ChuCaiDau,
    		ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
    		ISNULL(hd.TongTienHang, 0) as TongTienHang,
    		ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
    		ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich

    		from 
    		(
    			Select 
    			a.ID,
    			Sum(ISNULL(a.SoLuongBan, 0)) as SoLuongBan,
    			Sum(ISNULL(a.SoLuongTra, 0)) as SoLuongTra
    			from
    			(
    				Select 
    				hd.ID as ID,
    				Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongBan,
    				null as SoLuongTra 
    				from
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				where hd.ID_DonVi = @ID_DonVi
    				and hd.loaihoadon = '1'
    				Group by hd.ID
    
    				Union all
    				select 
    				hd.ID_HoaDon as ID,
    				null as SoLuongBan,
    				Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongTra
    				from
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				where hd.ID_DonVi = @ID_DonVi
    				and hd.loaihoadon = '6'
    				Group by hd.ID_HoaDon
    			)a
    			--where SoLuongTra < SoLuongBan
    			Group by a.ID
    		) b
    		inner join BH_HoaDon hd on b.ID = hd.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd 
    		and b.SoLuongBan > b.SoLuongTra
    			and hd.chothanhtoan = '0'
    	)c
    	where MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD
    	order by NgayLapHoaDon DESC");


        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportHangHoa_TraHangNhapChiTiet]");
            DropStoredProcedure("[dbo].[ReportHangHoa_NhapHangNCC]");
            DropStoredProcedure("[dbo].[ReportHangHoa_NhapHangChiTietNCC]");
        }
    }
}
