namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180411 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetDonVi_byUser]", parametersAction: p => new
            {
                ID_NguoiDung = p.Guid(),
                TenDonVi = p.String()
            }, body: @"Select 
	dv.ID,
	dv.TenDonVi,
	dv.SoDienThoai
	From Dm_DonVi dv
	inner join NS_QuaTrinhCongTac qtct on dv.ID = qtct.ID_DonVi
	inner join NS_NhanVien nv on qtct.ID_NhanVien = nv.ID
	inner join HT_NguoiDung nd on nv.ID = nd.ID_NhanVien	
	where nd.ID = @ID_NguoiDung and (dv.TenDonVi like @TenDonVi or dv.SoDienThoai like @TenDonVi)
	Order by dv.TenDonVi");

            CreateStoredProcedure(name: "[dbo].[TinhTonFirstDanhMucHangHoa]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"DECLARE @TonKhoTheoChiNhanh TABLE(
    		ID_DonViQuiDoi uniqueidentifier,
    		ID_HangHoaCungLoai uniqueidentifier,
    		GiaVon float,
    		GiaBan float,
    		TonKho float
    	);
    	INSERT INTO @TonKhoTheoChiNhanh exec LoadAllDanhMucHangHoa @ID_ChiNhanh
    	
    DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select aa.ID as ID_DonViQuiDoi, aa.TonToiThieu, aa.TonToiDa,aa.LaChaCungLoai, aa.ID_HangHoaCungLoai, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho INTO #tblTonFirst FROM (
    (select  dvqd.ID, hh.TonToiThieu, hh.TonToiDa, hh.ID_HangHoaCungLoai, hh.LaChaCungLoai, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DM_HangHoa hh
    left join DonViQuiDoi dvqd on hh.ID = dvqd.ID_hangHoa
    left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    where dvqd.xoa is null and dvqd.ladonvichuan = 1 and hh.LaChaCungLoai = 1 and hh.TheoDoi =1) aa
    left join
    (
    SELECT  dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    (
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
    SUM(ISNULL(HangHoa.TonDau,0)) AS TonCuoiKy
    FROM
    (
    SELECT
    td.ID_DonViQuiDoi,
    SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    NULL AS SoLuongNhap,
    NULL AS SoLuongXuat
    FROM
    (
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi 
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    
    		Select 
    			 hhtb2.ID_DonViQuiDoi,
    			 hhtb2.ID_HangHoaCungLoai,
    			 hhtb2.LaChaCungLoai,
    			 hhtb2.TonToiThieu,
    			 hhtb2.TonToiDa,	
    			 CASE
    					WHEN hhtb2.LaChaCungLoai = '1'
    						THEN
    							(SELECT SUM(tmphhtb2.TonKho) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)
    					ELSE
    						hhtb2.TonKho
    				END AS TonKho
    		from #tblTonFirst hhtb2");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_BanHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                MaKH = p.String(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"Select 
	 Case when b.LoaiHoaDon = 1 then N'Bán hàng: ' + CONVERT(nvarchar, Sum(ISNULL(b.SoLuongHD,0))) else
	 Case when b.LoaiHoaDon = 3 then N'Đặt hàng: ' +  CONVERT(nvarchar, Sum(ISNULL(b.SoLuongHD,0))) else N'Nhập trả hàng: ' + CONVERT(nvarchar, Sum(ISNULL(b.SoLuongHD,0))) end end as MaHoaDon,
	 Case when b.LoaiHoaDon = 6 then CAST(ROUND(Sum(ISNULL(b.SoLuong, 0)) * (-1) ,3)as float) else CAST(ROUND(Sum(ISNULL(b.SoLuong, 0)), 3)as float) end  as SoLuongSanPham,
	 Case when b.LoaiHoaDon = 6 then CAST(ROUND(Sum(ISNULL(b.TongTienHang, 0)) * (-1) ,0)as float) else CAST(ROUND(Sum(ISNULL(b.TongTienHang, 0)), 0) as float) end  as TongTienHang,
	 Case when b.LoaiHoaDon = 6 then CAST(ROUND(Sum(ISNULL(b.GiamGiaHD, 0)) * (-1) ,0)as float) else CAST(ROUND(Sum(ISNULL(b.GiamGiaHD, 0)), 0) as float) end  as GiamGiaHD,
	 Case when b.LoaiHoaDon = 6 then CAST(ROUND((Sum(ISNULL(b.TongTienHang, 0))- Sum(ISNULL(b.GiamGiaHD, 0))) *(-1), 0) as float) else CAST(ROUND(Sum(ISNULL(b.TongTienHang, 0))- Sum(ISNULL(b.GiamGiaHD, 0)), 0) as float) end as DoanhThu,
	 Case when b.LoaiHoaDon = 6 then CAST(ROUND(Sum(ISNULL(b.PhiTraHang, 0)) * (-1) ,0)as float) else CAST(ROUND(Sum(ISNULL(b.PhiTraHang, 0)), 0) as float) end  as PhiTraHang,
	 Case when b.LoaiHoaDon = 6 then CAST(ROUND(Sum(ISNULL(b.ThucThu, 0)) * (-1) ,0)as float) else CAST(ROUND(Sum(ISNULL(b.ThucThu, 0)), 0) as float) end as ThucThu,
	 b.LoaiHoaDon
	From
	(
		Select 
		*
		From
		(
			Select 
			hd.ID,
			hd.LoaiHoaDon,
			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
			dt.DienThoai,
			null as SoLuongHD,
			Sum(ISNULL(hdct.SoLuong, 0)) as SoLuong,
			Sum(ISNULL(hdct.ThanhTien, 0)) as TongTienHang,
			null as GiamGiaHD,
			null as PhiTraHang, 
			null as ThucThu
			From BH_HoaDon hd 
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0 
			and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 6)
			Group by hd.ID, hd.LoaiHoaDon, TenDoiTuong, dt.TenDoiTuong_KhongDau, dt.TenDoiTuong_ChuCaiDau, dt.DienThoai, dt.MaDoiTuong
			Union all
			Select 
			hd.ID,
			hd.LoaiHoaDon,
			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
			dt.DienThoai,
			COUNT(*) as SoLuongHD,
			null as SoLuong,
			null as TongTienHang,
			SUM(ISNULL(hd.TongGiamGia, 0)) as GiamGiaHD,
			case When hd.LoaiHoaDon = 6 then SUM(ISNULL(hd.TongChiPhi, 0)) else 0 end as PhiTraHang,
			null as ThucThu
			From BH_HoaDon hd 
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0 
			and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 6)
			Group by hd.ID, hd.LoaiHoaDon, TenDoiTuong, dt.TenDoiTuong_KhongDau, dt.TenDoiTuong_ChuCaiDau, dt.DienThoai, dt.MaDoiTuong
			Union all
			Select 
			hd.ID,
			hd.LoaiHoaDon,
			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
			dt.DienThoai,
			null as SoLuongHD,
			null as SoLuong,
			null as TongTienHang,
			null as GiamGiaHD,
			null as PhiTraHang,
			Sum(ISNULL(qct.TienThu, 0)) as ThucThu
			From BH_HoaDon hd 
			inner join Quy_HoaDon_ChiTiet qct on hd.ID = qct.ID_HoaDonLienQuan
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0 
			and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 6)
			Group by hd.ID, hd.LoaiHoaDon, TenDoiTuong, dt.TenDoiTuong_KhongDau, dt.TenDoiTuong_ChuCaiDau, dt.DienThoai, dt.MaDoiTuong
		) a
		where a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH
	) as b
	Group By b.LoaiHoaDon");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_BanHangChiTiet]", parametersAction: p => new
            {
                LoaiHoaDon = p.Int(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                MaKH = p.String(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"Select 
	 Max(hd.MaHoaDon) as MaHoaDon,
	 Max(hd.NgayLapHoaDon) as NgayLapHoaDon,
	 CAST(ROUND(Sum(ISNULL(b.SoLuong, 0)), 3) as float) as SoLuongSanPham,
	 CAST(ROUND(Sum(ISNULL(b.TongTienHang, 0)), 0) as float) as TongTienHang,
	 CAST(ROUND(Sum(ISNULL(b.GiamGiaHD, 0)), 0) as float) as GiamGiaHD,
	 CAST(ROUND(Sum(ISNULL(b.TongTienHang, 0))- Sum(ISNULL(b.GiamGiaHD, 0)), 0) as float) as DoanhThu,
	 CAST(ROUND(Sum(ISNULL(b.PhiTraHang, 0)), 0) as float) as PhiTraHang,
	 CAST(ROUND(Sum(ISNULL(b.ThucThu, 0)), 0) as float) as ThucThu
	From
	(
		Select 
		*
		From
		(
			Select 
			hd.ID,
			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
			dt.DienThoai,
			null as SoLuongHD,
			case When hd.LoaiHoaDon = 6 then SUM(ISNULL(hdct.SoLuong * (-1), 0)) else SUM(ISNULL(hdct.SoLuong, 0)) end as SoLuong,
			case When hd.LoaiHoaDon = 6 then SUM(ISNULL(hdct.ThanhTien * (-1), 0)) else SUM(ISNULL(hdct.ThanhTien, 0)) end as TongTienHang,
			--Sum(ISNULL(hdct.SoLuong, 0)) as SoLuong,
			--Sum(ISNULL(hdct.ThanhTien, 0)) as TongTienHang,
			null as GiamGiaHD,
			null as PhiTraHang, 
			null as ThucThu
			From BH_HoaDon hd 
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0 
			and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (hd.LoaiHoaDon = @LoaiHoaDon)
			Group by hd.ID,hd.LoaiHoaDon, TenDoiTuong, dt.TenDoiTuong_KhongDau, dt.TenDoiTuong_ChuCaiDau, dt.DienThoai, dt.MaDoiTuong
			Union all
			Select 
			hd.ID,
			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
			dt.DienThoai,
			COUNT(*) as SoLuongHD,
			null as SoLuong,
			null as TongTienHang,
			--SUM(ISNULL(hd.TongGiamGia, 0)) as GiamGiaHD,
			case When hd.LoaiHoaDon = 6 then SUM(ISNULL(hd.TongGiamGia * (-1), 0)) else SUM(ISNULL(hd.TongGiamGia, 0)) end as GiamGiaHD,
			case When hd.LoaiHoaDon = 6 then SUM(ISNULL(hd.TongChiPhi * (-1), 0)) else 0 end as PhiTraHang,
			null as ThucThu
			From BH_HoaDon hd 
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0 
			and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (hd.LoaiHoaDon = @LoaiHoaDon)
			Group by hd.ID, hd.LoaiHoaDon,TenDoiTuong, dt.TenDoiTuong_KhongDau, dt.TenDoiTuong_ChuCaiDau, dt.DienThoai, dt.MaDoiTuong
			Union all
			Select 
			hd.ID,
			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
			dt.DienThoai,
			null as SoLuongHD,
			null as SoLuong,
			null as TongTienHang,
			null as GiamGiaHD,
			null as PhiTraHang,
			--Sum(ISNULL(qct.TienThu, 0)) as ThucThu
			case When hd.LoaiHoaDon = 6 then SUM(ISNULL(qct.TienThu * (-1), 0)) else SUM(ISNULL(qct.TienThu, 0)) end as ThucThu
			From BH_HoaDon hd 
			inner join Quy_HoaDon_ChiTiet qct on hd.ID = qct.ID_HoaDonLienQuan
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0 
			and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (hd.LoaiHoaDon = @LoaiHoaDon)
			Group by hd.ID,hd.LoaiHoaDon, TenDoiTuong, dt.TenDoiTuong_KhongDau, dt.TenDoiTuong_ChuCaiDau, dt.DienThoai, dt.MaDoiTuong
		) a
		where a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH
	) as b
	inner join BH_HoaDon hd on b.ID = hd.ID
	Group By b.ID
	ORDER BY max(hd.NgayLapHoaDon) DESC");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_HangHoa]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
		hh.TenHangHoa,
		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	hh.ID_NhomHang,
    	CAST(ROUND(a.SoLuongban , 3) as float ) as SoLuongBan,
    	CAST(ROUND(a.GiaTriBan , 0) as float ) as GiaTriBan,
    	CAST(ROUND(a.SoLuongTra , 3) as float ) as SoLuongTra,
    	CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
    	CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan
    	FROM
    	(
			Select
				b.ID_DonViQuiDoi,
				Sum(ISNULL(b.SoLuongBan, 0)) as SoLuongBan,
				Sum(ISNULL(b.SoLuongTra, 0)) as SoLuongTra,
				Sum(ISNULL(b.GiaTriBan, 0)) as GiaTriBan,
				Sum(ISNULL(b.GiaTriTra, 0)) as GiaTriTra
			From
			(
    			SELECT 
    			dvqd.ID as ID_DonViQuiDoi,
				Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
				Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
				Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
				dt.DienThoai,
    			SUM(Case when hd.LoaiHoaDon = 1 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then ISNULL(hdct.SoLuong, 0) else 0 end) as SoLuongTra,
    			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
				and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and hh.LaHangHoa like @LaHangHoa
    			GROUP BY dvqd.ID, dt.MaDoiTuong, dt.TenDoiTuong_KhongDau, dt.TenDoiTuong_ChuCaiDau, dt.DienThoai
			) b
			where (b.DienThoai like @MaKH or b.TenKhachHang_ChuCaiDau like @MaKH or b.TenKhachHang_KhongDau like @MaKH or b.MaKhachHang like @MaKH) 
			GROUP by b.ID_DonViQuiDoi
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
    	ORDER BY DoanhThuThuan DESC");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_HangHoaChiTiet]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"SELECT
    	a.MaHoaDon,
		a.NgayLapHoaDon,
		a.TenKhachHang,
		a.SoLuongSanPham,
		a.TongTienHang,
		a.GiamGiaHD,
		a.TongTienHang - a.GiamGiaHD as DoanhThuThuan
    	FROM
    	(
			Select
				*
			From
			(
    			SELECT 
    			hd.MaHoaDon as MaHoaDon,
				hd.NgayLapHoaDon,
				Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
				Case when dt.TenDoiTuong is null then N'khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
				Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
				Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
				dt.DienThoai,
				Case when hd.LoaiHoaDon = 6 then CAST(ROUND(ISNULL(hdct.SoLuong, 0) * (-1) ,3)as float) else CAST(ROUND(ISNULL(hdct.SoLuong, 0), 3)as float) end  as SoLuongSanPham,
				Case when hd.LoaiHoaDon = 6 then CAST(ROUND(ISNULL(hdct.ThanhTien, 0) * (-1) ,0)as float) else CAST(ROUND(ISNULL(hdct.ThanhTien, 0), 0)as float) end  as TongTienHang,
				Case when hd.LoaiHoaDon = 6 then CAST(ROUND(ISNULL(hdct.ThanhTien, 0) * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) * (-1) ,0)as float) else CAST(ROUND(ISNULL(hdct.ThanhTien, 0) * (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)), 0)as float) end  as GiamGiaHD
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
				and hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
    			and dvqd.MaHangHoa like @maHH
			) b
			where (b.DienThoai like @MaKH or b.TenKhachHang_ChuCaiDau like @MaKH or b.TenKhachHang_KhongDau like @MaKH or b.MaKhachHang like @MaKH) 
    	) a
    	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_SoLuongGiaoDich]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"Select 
	b.GiaoDich,
	CAST(ROUND(SUM(ISNULL(b.SoGiaoDich, 0)), 0) as float) as SoGiaoDich,
	CAST(ROUND(SUM(ISNULL(b.SoLanTraTienMat, 0)), 0) as float) as TienMat,
	CAST(ROUND(SUM(ISNULL(b.SoLanChuyenKhoan, 0)), 0) as float)as ChuyenKhoan,
	CAST(ROUND(SUM(ISNULL(b.SoLanTraTienThe, 0)), 0) as float) as TienThe,
	CAST(ROUND(SUM(ISNULL(b.SoLanTraTienDiem, 0)), 0) as float) as TienDiem
	From
	(
		Select 
		Case when a.LoaiHoaDon = 1 then N'Bán hàng' else 
		Case when a.LoaiHoaDon = 3 then N'Đặt hàng'else N'Trả hàng' end end as GiaoDich,
		1 as SoGiaoDich,
		Case when a.TienMat > 0 then 1 else 0 end as SoLanTraTienMat,
		Case when a.TienGui > 0 then 1 else 0 end as SoLanChuyenKhoan,
		Case when a.ThuTuThe > 0 then 1 else 0 end as SoLanTraTienThe,
		Case when a.TienDiem > 0 then 1 else 0 end as SoLanTraTienDiem
		From
		(
			Select 
			hd.ID,
			hd.LoaiHoaDon,
			ISNULL(qhdct.TienMat, 0) as TienMat,
			ISNULL(qhdct.TienGui, 0) as TienGui,
			ISNULL(qhdct.ThuTuThe, 0) as ThuTuThe,
			Case when qhdct.DiemThanhToan is null then 0 else qhdct.TienThu end as TienDiem
			From BH_HoaDon hd
			left join Quy_HoaDon_ChiTiet qhdct on hd.ID = qhdct.ID_HoaDonLienQuan
			where hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0
			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 6)
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		)a
	)b
	Group by b.GiaoDich
	ORDER BY GiaoDich");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_SoLuongHangHoa]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"Select 
	b.GiaoDich,
	CAST(ROUND(SUM(ISNULL(b.SoLuongHangHoa, 0)), 0) as float) as SoLuongHangHoa,
	CAST(ROUND(SUM(ISNULL(b.SoLuongSanPham, 0)), 0) as float) as SoLuongSanPham
	From
	(
		Select 
		Case when a.LoaiHoaDon = 1 then N'Bán hàng' else 
		Case when a.LoaiHoaDon = 3 then N'Đặt hàng'else N'Trả hàng' end end as GiaoDich,
		1 as SoLuongHangHoa,
		SUM(a.SoLuong) as SoLuongSanPham
		From
		(
			Select 
			hd.LoaiHoaDon,
			hdct.ID_DonViQuiDoi,
			ISNULL(hdct.SoLuong, 0) as SoLuong
			From BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			where hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0
			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 6)
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		)a
		GROUP BY a.LoaiHoaDon, a.ID_DonViQuiDoi
	)b
	Group by b.GiaoDich
	ORDER BY GiaoDich");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_ThuChi]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                MaKH = p.String(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String(),
                lstThuChi = p.String()
            }, body: @"select 
	a.MaPhieuThu,
	a.TenNguoiNop,
	Case when a.LoaiThuChi in ('2', '4', '6') then a.Thuchi *(-1) else a.ThuChi end as ThuChi,
	a.NgayLapHoaDon,
	a.MaHoaDon
	From
	(
		select 
			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else 
			Case when qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null then 2 else 
			Case when hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 then 3 else 
			Case when hd.LoaiHoaDon = 6 then 4 else 
			Case when hd.LoaiHoaDon = 7 then 5 else 
			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi,
			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
			dt.DienThoai,
			qhd.MaHoaDon as MaPhieuThu,
			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
			Sum(ISNULL(qhdct.TienThu,0)) as ThuChi,
			qhd.NgayLapHoaDon,
			Case when hd.MaHoaDon is null and qhd.LoaiHoaDon = 11 then N'Phiếu thu khác' else
			Case when hd.MaHoaDon is null and qhd.LoaiHoaDon = 12 then N'Phiếu chi khác' else hd.MaHoaDon end end as MaHoaDon
		From Quy_HoaDon qhd 
		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
		left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
		where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
		and qhd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon
	)a
	where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH)
	and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_TongKetBanHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"Select 
	b.GiaoDich,
	CAST(ROUND(SUM(ISNULL(b.TongTienHang, 0)), 0) as float) as TongTienHang,
	CAST(ROUND(SUM(ISNULL(b.TienMat, 0)), 0) as float) as TienMat,
	CAST(ROUND(SUM(ISNULL(b.ChuyenKhoan, 0)), 0) as float) as ChuyenKhoan,
	CAST(ROUND(SUM(ISNULL(b.TienThe, 0)), 0) as float) as TienThe,
	CAST(ROUND(SUM(ISNULL(b.TienDiem, 0)), 0) as float) as TienDiem,
	CAST(ROUND(SUM(ISNULL(b.TongThuThu, 0)), 0) as float) as TongThuThu
	From
	(
		Select 
		Case when a.LoaiHoaDon = 1 then N'Bán hàng' else 
		Case when a.LoaiHoaDon = 3 then N'Đặt hàng'else N'Trả hàng' end end as GiaoDich,
		Case when a.LoaiHoaDon != 6 then CAST(ROUND(SUM(ISNULL(a.TongTienHang, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.TongTienHang, 0)) * (-1), 0) as float) end as TongTienHang,
		Case when a.LoaiHoaDon != 6 then CAST(ROUND(SUM(ISNULL(a.TienMat, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.TienMat, 0)) * (-1), 0) as float) end as TienMat,
		Case when a.LoaiHoaDon != 6 then CAST(ROUND(SUM(ISNULL(a.TienGui, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.TienGui, 0)) * (-1), 0) as float) end as ChuyenKhoan,
		Case when a.LoaiHoaDon != 6 then CAST(ROUND(SUM(ISNULL(a.ThuTuThe, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.ThuTuThe, 0)) * (-1), 0) as float) end as TienThe,
		Case when a.LoaiHoaDon != 6 then CAST(ROUND(SUM(ISNULL(a.TienDiem, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.TienDiem, 0)) * (-1), 0) as float) end as TienDiem,
		Case when a.LoaiHoaDon != 6 then CAST(ROUND(SUM(ISNULL(a.TienMat, 0)) + SUM(ISNULL(a.TienGui, 0)) + SUM(ISNULL(a.ThuTuThe, 0)) + SUM(ISNULL(a.TienDiem, 0)), 0) as float)
		else CAST(ROUND((SUM(ISNULL(a.TienMat, 0)) + SUM(ISNULL(a.TienGui, 0)) + SUM(ISNULL(a.ThuTuThe, 0)) + SUM(ISNULL(a.TienDiem, 0))) * (-1), 0) as float) end as TongThuThu
		From
		(
			Select 
			hd.ID,
			hd.LoaiHoaDon,
			hd.TongTienHang,
			qhdct.TienMat,
			qhdct.TienGui,
			qhdct.ThuTuThe,
			Case when qhdct.DiemThanhToan is null then 0 else qhdct.TienThu end as TienDiem
			From BH_HoaDon hd
			left join Quy_HoaDon_ChiTiet qhdct on hd.ID = qhdct.ID_HoaDonLienQuan
			where hd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
			and hd.ChoThanhToan = 0
			and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 6)
			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		)a
		Group by a.ID, a.LoaiHoaDon
	)b
	Group by b.GiaoDich
	ORDER BY GiaoDich");

            CreateStoredProcedure(name: "[dbo].[ReportCuoiNgay_TongKetThuChi]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                ID_NhanVien = p.String()
            }, body: @"Select 
	b.ThuChi,
	CAST(ROUND(SUM(ISNULL(b.TienMat, 0)), 0) as float) as TienMat,
	CAST(ROUND(SUM(ISNULL(b.ChuyenKhoan, 0)), 0) as float) as ChuyenKhoan,
	CAST(ROUND(SUM(ISNULL(b.TienThe, 0)), 0) as float) as TienThe,
	CAST(ROUND(SUM(ISNULL(b.TienDiem, 0)), 0) as float) as TienDiem,
	CAST(ROUND(SUM(ISNULL(b.TongThuThu, 0)), 0) as float) as TongThuThu
	From
	(
		Select 
		Case when a.LoaiHoaDon = 11 then N'Tổng thu' else N'Tổng chi' end as ThuChi,
		Case when a.LoaiHoaDon = 11 then CAST(ROUND(SUM(ISNULL(a.TienMat, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.TienMat, 0)) * (-1), 0) as float) end as TienMat,
		Case when a.LoaiHoaDon = 11 then CAST(ROUND(SUM(ISNULL(a.TienGui, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.TienGui, 0)) * (-1), 0) as float) end as ChuyenKhoan,
		Case when a.LoaiHoaDon = 11 then CAST(ROUND(SUM(ISNULL(a.ThuTuThe, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.ThuTuThe, 0)) * (-1), 0) as float) end as TienThe,
		Case when a.LoaiHoaDon = 11 then CAST(ROUND(SUM(ISNULL(a.TienDiem, 0)), 0) as float) else CAST(ROUND(SUM(ISNULL(a.TienDiem, 0)) * (-1), 0) as float) end as TienDiem,
		Case when a.LoaiHoaDon = 11 then CAST(ROUND(SUM(ISNULL(a.TienMat, 0)) + SUM(ISNULL(a.TienGui, 0)) + SUM(ISNULL(a.ThuTuThe, 0)) + SUM(ISNULL(a.TienDiem, 0)), 0) as float)
		else CAST(ROUND((SUM(ISNULL(a.TienMat, 0)) + SUM(ISNULL(a.TienGui, 0)) + SUM(ISNULL(a.ThuTuThe, 0)) + SUM(ISNULL(a.TienDiem, 0))) * (-1), 0) as float) end as TongThuThu
		From
		(
			Select 
			qhd.ID,
			qhd.LoaiHoaDon,
			qhdct.TienMat,
			qhdct.TienGui,
			qhdct.ThuTuThe,
			Case when qhdct.DiemThanhToan is null then 0 else qhdct.TienThu end as TienDiem
			From Quy_HoaDon qhd 
			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
			where qhd.ID_NhanVien in (select * from splitstring(@ID_NhanVien))
			and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
			and (qhd.TrangThai != 0 or qhd.TrangThai is null)
			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
		)a
		Group by a.ID, a.LoaiHoaDon
	)b
	Group by b.ThuChi
	ORDER BY TongThuThu DESC");

            AlterStoredProcedure("[dbo].[ReportHangBan_NhanVien]", parametersAction: p => new
            {
                MaHH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                ID_NhanVien = p.String()
            }, body: @"SELECT
    		nv.ID as ID_NhanVien,
    		nv.TenNhanVien,
    		CAST(ROUND(SUM(ISNULL(hdct.SoLuong * dvqd.TyLeChuyenDoi, 0)), 3) as float) as SoLuong,
    		CAST(ROUND(SUM(ISNULL(hdct.ThanhTien, 0)* (1- (ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)))), 0) as float)  as GiaTri,
    		hh.ID_NhomHang
    		FROM
    		BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		inner join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hd.LoaiHoaDon = 1
    		and hh.LaHangHoa like @LaHangHoa
    		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
			and nv.ID in (Select * from splitstring(@ID_NhanVien))
    		GROUP BY dvqd.ID, nv.ID, hh.ID_NhomHang, nv.TenNhanVien");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetDonVi_byUser]");
            DropStoredProcedure("[dbo].[TinhTonFirstDanhMucHangHoa]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_BanHang]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_BanHangChiTiet]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_HangHoa]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_HangHoaChiTiet]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_SoLuongGiaoDich]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_SoLuongHangHoa]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_ThuChi]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_TongKetBanHang]");
            DropStoredProcedure("[dbo].[ReportCuoiNgay_TongKetThuChi]");
        }
    }
}