namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180417 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_HoaDonNhapChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"Select 
		hh.ID_NhomHang,
    	bhhd.MaHoaDon,
    	bhhd.NgayLapHoaDon,
		dvqd.MaHangHoa,
		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
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
    	and (bhhd.ID_CheckIn is not null and bhhd.ID_CheckIn = @ID_ChiNhanh and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4')
    	Order by NgayLapHoaDon DESC, ThanhTien DESC");

            CreateStoredProcedure(name: "[dbo].[ReportHangHoa_HoaDonXuatChuyenHang]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"Select 
		hh.ID_NhomHang,
    	bhhd.MaHoaDon,
    	bhhd.NgayLapHoaDon,
		dvqd.MaHangHoa,
		hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
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
    	and ((bhhd.loaihoadon = '10' and bhhd.YeuCau = '1' and bhhd.ID_DonVi = @ID_ChiNhanh) or
    	(bhhd.ID_CheckIn is not null and bhhd.ID_CheckIn != @ID_ChiNhanh and bhhd.LoaiHoaDon = '10' and bhhd.YeuCau = '4' and bhhd.ID_DonVi = @ID_ChiNhanh))
    	Order by NgayLapHoaDon DESC, ThanhTien DESC");

            AlterStoredProcedure(name: "[dbo].[ReportNCC_NhapHangChiTiet]", parametersAction: p => new
            {
                ID_NCC = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"IF (@ID_NCC != '')
    	BEGIN
    		SELECT
    		hd.MaHoaDon as MaPhieu,
    		hd.NgayLapHoaDon,
    		a.SoLuongSanPham,
    		Case when hd.LoaiHoaDon = 4 then ISNULL(hd.PhaiThanhToan, 0) else ISNULL(hd.PhaiThanhToan, 0) * (-1) end as TongGiaTri
    		FROM
    		 (
    			SELECT 
    			hd.ID,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and hd.ID_DoiTuong = @ID_NCC
    			and hd.ChoThanhToan = 0
    			and (hd.loaihoadon = 4 or hd.loaihoadon = 7)
    			GROUP BY hd.ID
    		) a
    		left join BH_HoaDon hd on a.ID = hd.ID
    		ORDER BY hd.NgayLapHoaDon DESC
    	END
    	ELSE
    	BEGIN
    		SELECT
    	hd.MaHoaDon as MaPhieu,
    	hd.NgayLapHoaDon,
    	a.SoLuongSanPham,
    	Case when hd.LoaiHoaDon = 4 then ISNULL(hd.PhaiThanhToan, 0) else ISNULL(hd.PhaiThanhToan, 0) * (-1) end as TongGiaTri
    	FROM
    	 (
    		SELECT 
    		hd.ID,
    		SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongSanPham
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ID_DoiTuong is null
    		and hd.ChoThanhToan = 0
    		and (hd.loaihoadon = 4 or hd.loaihoadon = 7)
    		GROUP BY hd.ID
    	) a
    	left join BH_HoaDon hd on a.ID = hd.ID
    	ORDER BY hd.NgayLapHoaDon DESC
    	END");
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ReportHangHoa_HoaDonNhapChuyenHang]");
            DropStoredProcedure("[dbo].[ReportHangHoa_HoaDonXuatChuyenHang]");
        }
    }
}
