namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181006 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[FUNC_TinhSLTonKhiTaoHD]
    (
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier],
	@ID_LoHang [uniqueidentifier],
	@TimeStart [datetime]
	)
RETURNS FLOAT
AS

    BEGIN
	DECLARE @TonKho AS FLOAT;
	DECLARE @timeStartCS DATETIME;
	Set @timeStartCS =  (select convert(datetime, '2016/01/01'))
	DECLARE @SQL VARCHAR(254)
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStartCS =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    SELECT @TonKho =(a.TonCuoiKy / dvqd3.TyLeChuyenDoi) FROM 
    (
		SELECT 
		dhh.ID,
		SUM(ISNULL(HangHoa.TonDau,0)) AS TonCuoiKy
		FROM
			(
			SELECT
			td.ID_DonViQuiDoi,
			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
			NULL AS SoLuongNhap,
			NULL AS SoLuongXuat
			FROM
			(
				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				where dvqd.ladonvichuan = '1' and dvqd.ID_HangHoa = @ID_HangHoa and (cs.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
    				GROUP BY dvqd.ID
    			UNION ALL

				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false' and hh.LaHangHoa = 1
				AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @timeStartCS
				AND bhd.ID_DonVi = @ID_ChiNhanh
    				And dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				NULL AS SoLuongNhap,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND dvqd.ID_HangHoa = @ID_HangHoa
				AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @timeStartCS and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
				null AS SoLuongXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
				AND bhd.ID_DonVi = @ID_ChiNhanh
				AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @timeStartCS
    				AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi
    
				UNION ALL
				SELECT 
				bhdct.ID_DonViQuiDoi,
				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
				null AS SoLuongXuat,
				NULL AS TonKho
				FROM BH_HoaDon_ChiTiet bhdct
				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0

				AND bhd.NgayLapHoaDon < @TimeStart AND bhd.NgayLapHoaDon > @timeStartCS
    				AND dvqd.ID_HangHoa = @ID_HangHoa and (bhdct.ID_LoHang = @ID_LoHang or @ID_LoHang is null)
				GROUP BY bhdct.ID_DonViQuiDoi
				) AS td
				GROUP BY td.ID_DonViQuiDoi
				) 
				AS HangHoa
				left JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
				LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
				GROUP BY dhh.ID
    )  a
    	left Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	where dvqd3.ladonvichuan = 1
    order by MaHangHoa
	RETURN @TonKho;
END");
            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_ChiTiet_Page]", parametersAction: p => new
            {
                pageNumber = p.Int(),
                pageSize = p.Int(),
                Text_Search = p.String(),
                MaHH = p.String(),
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

	DECLARE @To as int
	DECLARE @From as int
	Set @To = (@pageNumber - 1) * @pageSize + 1;
	Set @From = (@pageNumber * @pageSize);
	Declare @tmp table (RownNum float, Rowns float, TongSoLuong float, TongThanhTien float,TongGiamGiaHD float,TongTienVon float,TongLaiLo float);
	Insert INTO @tmp
	Select 
	MAX(RownNum) as RownNum,
	MAX(Rowns) as Rowns,
	MAX(TongSoLuong) as TongSoLuong,
	MAX(TongThanhTien) as TongThanhTien,
	MAX(TongGiamGiaHD) as TongGiamGiaHD,
	MAX(TongTienVon) as TongTienVon,
	--MAX(TongLaiLo) as TongLaiLo
	MAX(TongThanhTien) - MAX(TongTienVon) - MAX(TongGiamGiaHD) as TongLaiLo
	FROM
	(
    SELECT 
			ROW_NUMBER() OVER (ORDER BY NgayLapHoaDon DESC) as RownNum,
			ROW_NUMBER() OVER (ORDER BY NgayLapHoaDon) as Rowns,
			SUM(CAST(ROUND(SoLuong, 3) as float)) OVER(ORDER BY NgayLapHoaDon) as TongSoLuong, 
			SUM(CAST(ROUND(ThanhTien, 0) as float)) OVER(ORDER BY NgayLapHoaDon) as TongThanhTien, 
			SUM(CAST(ROUND(GiamGiaHD, 0) as float)) OVER(ORDER BY NgayLapHoaDon) as TongGiamGiaHD,
			SUM(CAST(ROUND(a.SoLuong * a.GiaVon, 0) as float)) OVER(ORDER BY NgayLapHoaDon) as TongTienVon
			--SUM(CAST(ROUND(a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD, 0) as float)) OVER(ORDER BY NgayLapHoaDon) as TongLaiLo
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa,			
					dt.MaDoiTuong as MaKhachHang,
					Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
					Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
					Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
					dt.DienThoai,
    		    ISNULL(hdct.SoLuong, 0) as SoLuong,
    			ISNULL(hdct.DonGia, 0) as GiaBan,
    			ISNULL(hdct.TienChietKhau, 0) as TienChietKhau,
			Case when hdct.ID_ChiTietDinhLuong is not null then (select SUM (ct.GiaVon * qd.TyLeChuyenDoi) as GiaVon from BH_HoaDon_ChiTiet ct 
						inner join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
						where ct.ID_ChiTietDinhLuong = hdct.ID_ChiTietDinhLuong
						GROUP BY ct.ID_ChiTietDinhLuong) else ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0)end as GiaVon,
    		nv.TenNhanVien,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    			
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and (hd.LoaiHoaDon = 1 Or hd.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (hd.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    		and a.Xoa like @TrangThai
		) as p
		--WHERE RownNum BETWEEN 1 AND 1

	Select * FROM
	(
    SELECT 
			ROW_NUMBER() OVER (ORDER BY NgayLapHoaDon DESC) as RownNum,
			(select Max(Rowns) from @tmp) as Rowns,
			(select Max(TongSoLuong) from @tmp) as TongSoLuong,
			(select Max(TongThanhTien) from @tmp) as TongThanhTien,
			(select Max(TongGiamGiaHD) from @tmp) as TongGiamGiaHD,
			Case When @XemGiaVon = '1' then (select Max(TongTienVon) from @tmp) else 0 end as TongTienVon,
			Case When @XemGiaVon = '1' then (select Max(TongLaiLo) from @tmp) else 0 end as TongLaiLo,
			a.MaKhachHang,
			a.TenKhachHang,
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
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.GiaVon), 0) as float) else 0 end as GiaVon,
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.SoLuong * a.GiaVon), 0) as float) else 0 end as TienVon,
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
					dt.MaDoiTuong as MaKhachHang,
					Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
					Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
					Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
					dt.DienThoai,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    			ISNULL(hdct.DonGia, 0) as GiaBan,
    			ISNULL(hdct.TienChietKhau, 0) as TienChietKhau,
    		--Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0) else 0 end as GiaVon, 
			Case when hdct.ID_ChiTietDinhLuong is not null then (select SUM (ct.GiaVon * qd.TyLeChuyenDoi) as GiaVon from BH_HoaDon_ChiTiet ct 
						inner join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
						where ct.ID_ChiTietDinhLuong = hdct.ID_ChiTietDinhLuong
						GROUP BY ct.ID_ChiTietDinhLuong) else ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0)end as GiaVon,
    		nv.TenNhanVien,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
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
    		and (hd.LoaiHoaDon = 1 Or hd.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (hd.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    		and a.Xoa like @TrangThai
		) as p
		WHERE RownNum BETWEEN @To AND @From");

            CreateStoredProcedure(name: "[dbo].[BaoCaoBanHang_TongHop_Page]", parametersAction: p => new
            {
                pageNumber = p.Int(),
                pageSize = p.Int(),
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
                ID_NhomHang_SP = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	DECLARE @To as int
	DECLARE @From as int
	Set @To = (@pageNumber - 1) * @pageSize + 1;
	Set @From = (@pageNumber * @pageSize);
	Declare @tmp table(RownNum float, TenNhomHangHoa [nvarchar](max), MaHangHoa  [nvarchar](max), TenHangHoaFull [nvarchar](max), TenHangHoa  [nvarchar](max),ThuocTinh_GiaTri [nvarchar](max),
	TenDonViTinh [nvarchar](max),TenLoHang [nvarchar](max), SoLuong float, ThanhTien float, TienVon float, GiamGiaHD float, LaiLo float);
	Insert INTO @tmp
    SELECT 
			ROW_NUMBER() OVER (ORDER BY CAST(ROUND((Sum(a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD)), 0) as float) DESC) as RownNum,
			--COUNT(*) OVER (ORDER BY CAST(ROUND((Sum(a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD)), 0) as float)) AS Rowns,
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
    			a.MaHangHoa,
    			Max(a.TenHangHoaFull) as  TenHangHoaFull,
    			Max(a.TenHangHoa) as TenHangHoa,
    		Max(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    			Max(a.TenDonViTinh) as TenDonViTinh,
    			a.TenLoHang,
    		SUM(a.SoLuong) as SoLuong, 
    		Sum(a.ThanhTien) as ThanhTien,
    		Sum(a.SoLuong * a.GiaVon) as TienVon,
    		Sum(a.GiamGiaHD) as GiamGiaHD,
    		Case When @XemGiaVon = '1' then Sum(a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD) else 0 end as LaiLo
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
    		--Case When @XemGiaVon = '1' then ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0) else 0 end as GiaVon,
			Case when hdct.ID_ChiTietDinhLuong is not null then (select SUM (ct.GiaVon * qd.TyLeChuyenDoi) as GiaVon from BH_HoaDon_ChiTiet ct 
						inner join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
						where ct.ID_ChiTietDinhLuong = hdct.ID_ChiTietDinhLuong
						GROUP BY ct.ID_ChiTietDinhLuong) else ISNULL(hdct.GiaVon * dvqd.TyLeChuyenDoi, 0)end as GiaVon,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
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
			and (hd.LoaiHoaDon = 1 Or hd.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and a.Xoa like @TrangThai
    		and (a.MaHangHoa like @Text_Search or a.TenHangHoa_KhongDau like @Text_Search or a.TenHangHoa_KyTuDau like @Text_Search or a.TenNhomHangHoa_KhongDau like @TenNhomHang or a.TenNhomHangHoa_KyTuDau like @TenNhomHang or a.MaHangHoa like @MaHH)
    		Group by a.MaHangHoa, a.TenLoHang, a.ID_LoHang
    		--OrDER BY LaiLo DESC
		--) as p
		--WHERE p.RownNum BETWEEN @To AND @From
		select TenNhomHangHoa, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, 
		CAST(ROUND(SoLuong, 3) as float) as SoLuong,
		CAST(ROUND(ThanhTien, 0) as float) as ThanhTien,
		CAST(ROUND(TienVon, 0) as float) as TienVon,
		CAST(ROUND(GiamGiaHD, 0) as float) as GiamGiaHD,
		CAST(ROUND(LaiLo, 0) as float) as LaiLo,
		(select MAX(RownNum) from @tmp) as Rowns,
		(select CAST(ROUND(SUM(SoLuong), 3) as float) from @tmp) as TongSoLuong,
		(select CAST(ROUND(SUM(ThanhTien), 0) as float) from @tmp) as TongThanhTien,
		(select CAST(ROUND(SUM(GiamGiaHD), 0) as float) from @tmp) as TongGiamGiaHD,
		(select CAST(ROUND(SUM(TienVon), 0) as float) from @tmp) as TongTienVon,
		(select CAST(ROUND(SUM(LaiLo), 0) as float) from @tmp) as TongLaiLo
		from @tmp 
		WHERE RownNum BETWEEN @To AND @From
		order by LaiLo DESC");

            CreateStoredProcedure(name: "[dbo].[GetChiTietHoaDon_ByIDHoaDon]", parametersAction: p => new
            {
                IDHoaDon = p.Guid()
            }, body: @"DECLARE @TableCT TABLE(ID UNIQUEIDENTIFIER, ID_HoaDon UNIQUEIDENTIFIER, DonGia FLOAT, GiaVon FLOAT, SoThuTu FLOAT,SoLuong FLOAT, ThanhTien FLOAT, TienChietKhau FLOAT, ThanhToan FLOAT, ID_DonViQuiDoi UNIQUEIDENTIFIER,ID_HangHoa UNIQUEIDENTIFIER, TenDonViTinh NVARCHAR(MAX),
MaHangHoa NVARChAR(MAX), GiamGia FLOAT, PTChietKhau FLOAT, GiaBan FLOAT, ThoiGian DateTime, GhiChu NVARCHAR(MAX), ID_KhuyenMai UNIQUEIDENTIFIER, ThuocTinh_GiaTri NVARCHAR(MAX), LaHangHoa BIT, QuanLyTheoLoHang BIT, TenHangHoa NVARCHAR(MAX), TyLeChuyenDoi FLOAT,
YeuCau NVARCHAR(MAX), ID_LoHang UNIQUEIDENTIFIER, MaLoHang NVARCHAR(MAX), TonKho FLOAT , ID_DonVi UNIQUEIDENTIFIER) INSERT INTO @TableCT
    SELECT 
    		cthd.ID,cthd.ID_HoaDon,ROUND(DonGia, 0),ROUND(cthd.GiaVon, 0),cthd.SoThuTu, SoLuong,ThanhTien, TienChietKhau,ThanhToan, ID_DonViQuiDoi,
    		dvqd.ID_HangHoa,dvqd.TenDonViTinh,dvqd.MaHangHoa, TienChietKhau as GiamGia,PTChietKhau,(cthd.DonGia - cthd.TienChietKhau) as GiaBan,ThoiGian,cthd.GhiChu,
    		cthd.ID_KhuyenMai,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		LaHangHoa,QuanLyTheoLoHang,TenHangHoa,		
    		TyLeChuyenDoi,YeuCau,
    		DM_LoHang.ID AS ID_LoHang,ISNULL(MaLoHang,'') as MaLoHang, 0 as TonKho, hd.ID_DonVi
    		FROM BH_HoaDon hd
    		JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    		JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    		JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
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
					) as ThuocTinh on hh.ID = ThuocTinh.id_hanghoa
    		LEFT JOIN DM_LoHang ON cthd.ID_LoHang = DM_LoHang.ID
    		WHERE cthd.ID_HoaDon = @IDHoaDon
	
	DECLARE @ID UNIQUEIDENTIFIER;
    DECLARE @ID_HoaDon UNIQUEIDENTIFIER;
    DECLARE @DonGia FLOAT;
    DECLARE @GiaVon FLOAT;
    DECLARE @SoThuTu FLOAT;
	DECLARE @SoLuong FLOAT;
    DECLARE @ThanhTien FLOAT;
    DECLARE @TienChietKhau FLOAT;
    DECLARE @ThanhToan FLOAT;
    DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
    DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
    DECLARE @TenDonViTinh NVARCHAR(MAX);
    DECLARE @MaHangHoa NVARCHAR(MAX);
    DECLARE @GiamGia FLOAT;
    DECLARE @PTChietKhau FLOAT;
    DECLARE @GiaBan FLOAT;
    DECLARE @ThoiGian DATETIME;
    DECLARE @GhiChu NVARCHAR(MAX);
	DECLARE @ID_KhuyenMai UNIQUEIDENTIFIER;
	DECLARE @ThuocTinh_GiaTri NVARCHAR(MAX);
	DECLARE @LaHangHoa BIT;
	DECLARE @QuanLyTheoLoHang BIT;
	DECLARE @TenHangHoa NVARCHAR(MAX);
	DECLARE @TyLeChuyenDoi FLOAT;
	DECLARE @YeuCau NVARCHAR(MAX);
	DECLARE @ID_LoHang UNIQUEIDENTIFIER;
	DECLARE @MaLoHang NVARCHAR(MAX);
	DECLARE @TonKho FLOAT;
	DECLARE @ID_DonVi UNIQUEIDENTIFIER;

	DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @TableCT
		--foreach tất cả chi tiết của các hàng hóa trong hd cần update
		OPEN CS_Item 
		FETCH FIRST FROM CS_Item INTO @ID, @ID_HoaDon,@DonGia,@GiaVon, @SoThuTu,@SoLuong, @ThanhTien, @TienChietKhau,@ThanhToan,@ID_DonViQuiDoi, @ID_HangHoa,@TenDonViTinh,@MaHangHoa,@GiamGia, 
		@PTChietKhau, @GiaBan, @ThoiGian, @GhiChu,@ID_KhuyenMai,@ThuocTinh_GiaTri,@LaHangHoa,@QuanLyTheoLoHang,@TenHangHoa,@TyLeChuyenDoi,@YeuCau,@ID_LoHang,@MaLoHang,@TonKho,@ID_DonVi
		WHILE @@FETCH_STATUS = 0
			BEGIN
				DECLARE @TonKhoHienTai FLOAT;
				SET @TonKhoHienTai = ISNULL([dbo].FUNC_TinhSLTonKhiTaoHD(@ID_DonVi,@ID_HangHoa,@ID_LoHang, GetDate()),0)
				UPDATE @TableCT SET TonKho = @TonKhoHienTai WHERE ID = @ID
				FETCH NEXT FROM CS_Item INTO @ID, @ID_HoaDon,@DonGia,@GiaVon, @SoThuTu,@SoLuong ,@ThanhTien, @TienChietKhau,@ThanhToan,@ID_DonViQuiDoi, @ID_HangHoa,@TenDonViTinh,@MaHangHoa,@GiamGia, 
		@PTChietKhau, @GiaBan, @ThoiGian, @GhiChu,@ID_KhuyenMai,@ThuocTinh_GiaTri,@LaHangHoa,@QuanLyTheoLoHang,@TenHangHoa,@TyLeChuyenDoi,@YeuCau,@ID_LoHang,@MaLoHang,@TonKho,@ID_DonVi
    		END
    	CLOSE CS_Item
    	DEALLOCATE CS_Item

		SELECT * from @TableCT order by SoThuTu desc");

            CreateStoredProcedure(name: "[dbo].[GetInForStaff_Working_byChiNhanh]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"SELECT nv.ID,MaNhanVien,TenNhanVien,DienThoaiDiDong
	from NS_NhanVien nv
	join NS_QuaTrinhCongTac qt on nv.ID = qt.ID_NhanVien
	where qt.ID_DonVi like @ID_DonVi and nv.DaNghiViec!='1'");

            CreateStoredProcedure(name: "[dbo].[getList_HangHoabyMaHH_LoHang]", parametersAction: p => new
            {
                MaLoHang = p.String(),
                MaHangHoa = p.String(),
                ID_DonVi = p.Guid()
            }, body: @"Select 
		*
	FROM
	(
    select 
    	dvqd.ID,
    	hh.ID as ID_HangHoa,
		lh.ID as ID_LoHang,
		case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
		Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	dvqd.TenDonViTinh,
		Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
		Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
		Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan,
    	Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVon
    	FROM 
    	DonViQuiDoi dvqd 
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on lh.ID_HangHoa = hh.ID
		left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or gv.ID_LoHang is null) and gv.ID_DonVi = @ID_DonVi)
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
    	where dvqd.MaHangHoa = @MaHangHoa 
    		and dvqd.Xoa is null
    		and hh.TheoDoi = 1
			) as p
			where p.TenLoHang = @MaLoHang");

            CreateStoredProcedure(name: "[dbo].[getlist_HoaDon_afterTraHang_DichVu]", parametersAction: p => new
            {
                MaHD = p.String(),
                ID_DonVi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                loaiHoaDon = p.Int()
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
    	c.DiemGiaoDich,
		c.SoLuongTra,
		c.SoLuongBan,
		c.LoaiHoaDon
    	from
    	(
    		Select 
    		hd.ID,
    		hd.MaHoaDon,
			hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
    		dt.DienThoai,
    		hd.ID_DoiTuong,	
    		hd.ID_HoaDon,
    		hd.ID_BangGia,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
    		hd.NguoiTao,	
    		hd.DienGiai,	
			b.SoLuongTra,
			b.SoLuongBan,
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
    				and hd.loaihoadon = @loaiHoaDon
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
					and hd.YeuCau is null
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

            CreateStoredProcedure(name: "[dbo].[Insert_DMLienHe]", parametersAction: p => new
            {
                MaLienHe = p.String(),
                TenLienHe = p.String(),
                ID_DoiTuong = p.Guid(),
                SoDienThoai = p.String(),
                DienThoaiCoDinh = p.String(),
                NgaySinh = p.String(),
                Email = p.String(),
                ChucVu = p.String(),
                DiaChi = p.String(),
                ID_QuanHuyen = p.Guid(),
                ID_TinhThanh = p.Guid(),
                GhiChu = p.String(),
                NguoiTao = p.String()
            }, body: @"DECLARE @NgaySinhDT as Datetime
    	set @NgaySinhDT = null;
    		if (len(@NgaySinh) > 0)
    		Begin
    			Set @NgaySinhDT = (select convert(datetime,@NgaySinh, 103));
    		end	
		INSERT INTO DM_LienHe(ID, MaLienHe, TenLienHe, ID_DoiTuong, SoDienThoai, DienThoaiCoDinh, NgaySinh, Email, ChucVu, DiaChi, ID_QuanHuyen,ID_TinhThanh, GhiChu, NguoiTao, NgayTao,TrangThai)
    	VALUES (NEWID(),@MaLienHe,@TenLienHe,@ID_DoiTuong,@SoDienThoai,@DienThoaiCoDinh, @NgaySinhDT,@Email,@ChucVu,@DiaChi,@ID_QuanHuyen,@ID_TinhThanh,@GhiChu, @NguoiTao, GETDATE(),1) ");


            CreateStoredProcedure(name: "[dbo].[SP_GetInfor_TPDinhLuong]", parametersAction: p => new
            {
                ID_DonVi = p.String(),
                ID_DichVu = p.String()
            }, body: @"select dl.ID_DonViQuiDoi,CAST(dl.SoLuong as float) as SoLuong , CAST(GiaVon as float) as GiaVon
	from DinhLuongDichVu dl
	left join DM_GiaVon gv on dl.ID_DonViQuiDoi = gv.ID_DonViQuiDoi
	where  gv.ID_DonVi like @ID_DonVi and dl.ID_DichVu like @ID_DichVu");

            CreateStoredProcedure(name: "[dbo].[SP_GetList_ServicePackages_Mua]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"select hd.MaHoaDon, 
		convert(varchar,hd.NgayLapHoaDon, 103) as NgayLapHoaDon,
		hd.TongTienHang - (ISNULL(hd.TongGiamGia,0)  + ISNULL(hd.KhuyeMai_GiamGia,0))
		 - (ISNULL(HDTra.TongTienHang,0) - (ISNULL(HDTra.TongGiamGia,0)  + ISNULL(HDTra.KhuyeMai_GiamGia,0))) as TongTienHang, 
		hd.PhaiThanhToan - ISNULL(HDTra.PhaiThanhToan,0) - ISNULL(hdgoc.PhaiThanhToan,0) as PhaiThanhToan,
		ISNULL(QuyHDMua.TongTienThu,0) - ISNULL(HDTra.TongTienThu,0) 
		-ISNULL(CASE WHEN HDTra.TongTienThu IS NULL OR HDTra.TongTienThu = 0  THEN HDTra.PhaiThanhToan
			ELSE 0 END,0) as DaThanhToan,
		CASE WHEN (HDTra.NoiDungThu  is null OR HDTra.NoiDungThu ='') THEN hd.DienGiai
		ELSE HDTra.NoiDungThu end GhiChu
	from BH_HoaDon hd
	left join 
			(select qct.ID_HoaDonLienQuan,qhd.TongTienThu,MAX(ISNULL(qhd.NoiDungThu,'')) as NoiDungThu
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
			group by qct.ID_HoaDonLienQuan,qct.ID_HoaDon,qhd.MaHoaDon,qhd.TongTienThu) QuyHDMua on hd.ID = QuyHDMua.ID_HoaDonLienQuan
	left join (
				select hdt.ID_HoaDon as ID, 
						ISNULL(QuyHDTra.TongTienThu,0) as TongTienThu,
						ISNULL(QuyHDTra.NoiDungThu,'') as NoiDungThu,
						MAX(hdt.MaHoaDon) as MaHoaDon,
						SUM(hdt.TongTienHang) as TongTienHang,
						SUM(hdt.TongGiamGia) as TongGiamGia ,
						SUM(hdt.KhuyeMai_GiamGia) as KhuyeMai_GiamGia, 
						SUM(hdt.PhaiThanhToan) as PhaiThanhToan
				from BH_HoaDon hd
				left join BH_HoaDon hdt on hd.ID = hdt.ID_HoaDon
				left join 
					(select qct.ID_HoaDonLienQuan,qhd.TongTienThu, MAX(ISNULL(qhd.NoiDungThu,'')) as NoiDungThu from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
					where qhd.LoaiHoaDon =12
					group by qct.ID_HoaDonLienQuan,qct.ID_HoaDon,qhd.MaHoaDon,qhd.TongTienThu) QuyHDTra on hdt.ID = QuyHDTra.ID_HoaDonLienQuan
				left join BH_HoaDon hddt on hdt.ID_HoaDon= hddt.ID
				where hdt.LoaiHoaDon =6 --and hdt.ID_DoiTuong= '5CF11B84-9EE8-4B3A-AB62-45A626B6C65F'
				group by hdt.ID_HoaDon,QuyHDTra.TongTienThu,QuyHDTra.NoiDungThu) HDTra on hd.ID= HDTra.ID
	left join BH_HoaDon hdgoc on hd.ID_HoaDon = hdgoc.ID
	where hd.LoaiHoaDon = 19 and hd.ID_DoiTuong like @ID_DoiTuong and hd.ID_DonVi like @ID_DonVi
	order by hd.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[SP_GetListHoaDon_UseService]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"select 
	hdsd.ID ,hdsd.MaHoaDon,
	convert(varchar,hdsd.NgayLapHoaDon, 103) as NgayLapHoaDon,
	CAST(hdsd.TongTienHang - ISNULL(hdsd.TongGiamGia,0) - ISNULL(hdsd.KhuyeMai_GiamGia,0) as float) as TongTienHang,
	CAST(ISNULL(hdsd.PhaiThanhToan,0) AS float) AS PhaiThanhToan,
	hdsd.DienGiai,
	CAST(ISNULL(tblQuyHD.TongTienThu,0) AS float) AS DaThanhToan

from BH_HoaDon hdsd
join 
-- join voi chinh no voi dieu kien cthd.ID_ChiTietGoiDV is not null
	(select hdb.ID
	 from BH_HoaDon hdb
	join BH_HoaDon_ChiTiet cthd on hdb.ID = cthd.ID_HoaDon where cthd.ID_ChiTietGoiDV is not null
	group by hdb.ID) HoaDonBan on HoaDonBan.ID= hdsd.ID
left join 
		(select qct.ID_HoaDonLienQuan, MAX(qhd.TongTienThu) as TongTienThu from Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
		group by qct.ID_HoaDonLienQuan,qct.ID_HoaDon) tblQuyHD on hdsd.ID = tblQuyHD.ID_HoaDonLienQuan
where hdsd.ID_DoiTuong like @ID_DoiTuong and hdsd.ID_DonVi like @ID_DonVi
order by hdsd.NgayLapHoaDon ");

            CreateStoredProcedure(name: "[dbo].[SP_NhatKySuDung_GoiDV]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"SELECT convert(varchar,hd.NgayLapHoaDon, 103) as NgayLapHoaDon,
	qd.MaHangHoa as MaDichVu,hh.TenHangHoa as TenDichVu, ct.SoLuong,
	hdXMLOut.HDCT_NhanVien as NhanVienThucHien,
	CT_ChietKhauNV.TongChietKhau
	FROM BH_HoaDon_ChiTiet ct
	join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.id
	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
	left join 
    			(Select distinct hdXML.ID,
    					(
    					select  nv.TenNhanVien +', '  AS [text()]
    					from BH_HoaDon_ChiTiet ct2
    					left join BH_NhanVienThucHien nvth on ct2.ID = nvth.ID_ChiTietHoaDon
    					left join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    					where ct2.ID = hdXML.ID and (nvth.ThucHien_TuVan is null OR nvth.ThucHien_TuVan = '1')
    					For XML PATH ('')
    				) HDCT_NhanVien
    			from BH_HoaDon_ChiTiet hdXML) hdXMLOut on ct.ID= hdXMLOut.ID
	 left join 
			(select ct3.ID, SUM(isnull(nvth2.TienChietKhau,0)) as TongChietKhau from BH_HoaDon_ChiTiet ct3
			left join BH_NhanVienThucHien nvth2 on ct3.ID = nvth2.ID_ChiTietHoaDon
			group by ct3.ID) CT_ChietKhauNV on CT_ChietKhauNV.ID = ct.ID


	WHERE ct.ID_ChiTietGoiDV is not null and ct.ID_ChiTietDinhLuong= ct.id 
	and hd.ID_DoiTuong like @ID_DoiTuong and hd.ID_DonVi like @ID_DonVi --and hd.mahoadon='HDBL000393'
	ORDER BY hd.NgayLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_PhieuChi_ServicePackage]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"Select 
	MAX(a.MaHoaDon) as MaHoaDon,
	CONVERT(varchar, MAX(a.NgayLapHoaDon),103) as NgayLapHoaDon,
	SUM(a.TienMat) as TienMat,
	SUM(a.TienGui) as TienGui,
	SUM(a.TienThu) as TongThu,
	MAX(a.NoiDungThu) as NoiDungThu

FROM
(
	Select MAX(qhd.ID) as ID_QuyHoaDon, 
		MAX(qhd.MaHoaDon) as MaHoaDon, 
		MAX(qhd.NgayLapHoaDon) as NgayLapHoaDon,
		MAX(ISNULL(qct.TienMat, 0)) as TienMat,
		MAX(ISNULL(qct.TienGui, 0)) as TienGui,
		MAX(ISNULL(qct.TienThu, 0)) as TienThu, 
		MAX(qhd.NoiDungThu) as NoiDungThu
	from Quy_HoaDon qhd 
	inner join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
	join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
	Inner join BH_HoaDon_ChiTiet hdct on hdct.ID_HoaDon = hd.ID
	where hd.LoaiHoaDon = 6
	and hd.ChoThanhToan = 0  and hd.ID_DoiTuong like @ID_DoiTuong and hd.ID_DonVi like @ID_DonVi
	group by qct.ID
)a
GROUP BY a.ID_QuyHoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_PhieuThu_ServicePackage]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"Select 
	MAX(a.MaHoaDon) as MaHoaDon,
	CONVERT(varchar, MAX(a.NgayLapHoaDon),103) as NgayLapHoaDon,
	SUM(a.TienMat) as TienMat,
	SUM(a.TienGui) as TienGui,
	SUM(a.TienThu) as TongThu,
	MAX(a.NoiDungThu) as NoiDungThu

FROM
(
	Select MAX(qhd.ID) as ID_QuyHoaDon, 
		MAX(qhd.MaHoaDon) as MaHoaDon, 
		MAX(qhd.NgayLapHoaDon) as NgayLapHoaDon,
		MAX(ISNULL(qct.TienMat, 0)) as TienMat,
		MAX(ISNULL(qct.TienGui, 0)) as TienGui,
		MAX(ISNULL(qct.TienThu, 0)) as TienThu, 
		MAX(qhd.NoiDungThu) as NoiDungThu
	from Quy_HoaDon qhd 
	inner join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
	join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
	Inner join BH_HoaDon_ChiTiet hdct on hdct.ID_HoaDon = hd.ID
	where ((hd.LoaiHoaDon = 1 and hdct.ID_ChiTietGoiDV is not null) or hd.LoaiHoaDon = 19)
	and hd.ChoThanhToan = 0  and hd.ID_DoiTuong like @ID_DoiTuong and hd.ID_DonVi like @ID_DonVi
	group by qct.ID
)a
GROUP BY a.ID_QuyHoaDon");

            
            Sql(@"ALTER PROCEDURE [dbo].[deleteHoaDonDieuChinh]
    @ID_HoaDon [uniqueidentifier]
AS
BEGIN
    Update BH_HoaDon set YeuCau = N'Hủy bỏ', ChoThanhToan = null where ID = @ID_HoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[insert_DieuChinhGiaVon]
    @ID [uniqueidentifier],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHoaDon [nvarchar](max),
    @TongGiaVonHienTai [float],
    @TongGiaVonMoi [float],
    @TongGiaVonTang [float],
    @TongGiaVonGiam [float],
    @ChoThanhToan [bit],
    @DienGiai [nvarchar](max),
    @NguoiTao [nvarchar](max),
    @loaiInsert [int],
    @YeuCau [nvarchar](max),
	@timeCreate [datetime]
AS
BEGIN
    if (@loaiInsert = 1)
    		BEGIN
    			Insert into BH_HoaDon (ID, MaHoaDon, LoaiHoaDon, ChoThanhToan, ID_DonVi, ID_NhanVien, NgayLapHoaDon, TongTienHang, TongChietKhau, TongTienThue, TongChiPhi,TongGiamGia,PhaiThanhToan, DienGiai, YeuCau, NguoiTao, NgayTao)
    			Values(@ID, @MaHoaDon, '18', @ChoThanhToan, @ID_DonVi, @ID_NhanVien, @timeCreate, @TongGiaVonHienTai, @TongGiaVonMoi, @TongGiaVonTang, @TongGiaVonGiam,'0','0', @DienGiai, @YeuCau, @NguoiTao, GETDATE())
    		END
    	else
    		BEGin
    			update BH_HoaDon set MaHoaDon = @MaHoaDon, ChoThanhToan = @ChoThanhToan, ID_NhanVien = @ID_NhanVien, TongTienHang = @TongGiaVonHienTai, TongChietKhau = @TongGiaVonMoi,
    			TongTienThue = @TongGiaVonTang, TongChiPhi = @TongGiaVonGiam, DienGiai = @DienGiai, YeuCau = @YeuCau, NgayLapHoaDon = @timeCreate, NguoiTao = @NguoiTao, NguoiSua = @NguoiTao, NgaySua = GETDATE()
    			where ID = @ID
    		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetSoLuongHangMua_ofKhachHang]
AS
BEGIN
    Select 
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
    		where ID_DoiTuong is not null and LoaiHoaDon=1 and ChoThanhToan is not null
    		group by hd.ID_DoiTuong, ct.ID_DonViQuiDoi
    	Union all
    		select ID_DoiTuong, 
    		 ID_DonViQuiDoi,
    		SUM(ct.soluong) as soluongTra, 
    		0 as SoLuongMua
    		from BH_HoaDon hd
    	join BH_HoaDon_ChiTiet ct on  hd.ID = ct.ID_HoaDon
    	where ID_DoiTuong is not null and LoaiHoaDon=6 and ChoThanhToan is not null
    	group by hd.ID_DoiTuong,ct.ID_DonViQuiDoi
    	) as a
    	Group by a.ID_DoiTuong, a.ID_DonViQuiDoi having (SUM(a.soluongMua - a.soluongTra)) >=0
END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoBanHang_ChiTiet_Page]");
            DropStoredProcedure("[dbo].[BaoCaoBanHang_TongHop_Page]");
            DropStoredProcedure("[dbo].[GetChiTietHoaDon_ByIDHoaDon]");
            DropStoredProcedure("[dbo].[GetInForStaff_Working_byChiNhanh]");
            DropStoredProcedure("[dbo].[getList_HangHoabyMaHH_LoHang]");
            DropStoredProcedure("[dbo].[getlist_HoaDon_afterTraHang_DichVu]");
            DropStoredProcedure("[dbo].[Insert_DMLienHe]");
            DropStoredProcedure("[dbo].[SP_GetInfor_TPDinhLuong]");
            DropStoredProcedure("[dbo].[SP_GetList_ServicePackages_Mua]");
            DropStoredProcedure("[dbo].[SP_GetListHoaDon_UseService]");
            DropStoredProcedure("[dbo].[SP_NhatKySuDung_GoiDV]");
            DropStoredProcedure("[dbo].[SP_PhieuChi_ServicePackage]");
            DropStoredProcedure("[dbo].[SP_PhieuThu_ServicePackage]");
        }
    }
}