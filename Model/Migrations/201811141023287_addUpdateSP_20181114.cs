namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20181114 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_BieuDoDoanhThuToDay]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
    		SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		ISNULL(hdct.ThanhTien, 0) as ThanhTien
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		--and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
		) a
    	GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_BieuDoDoanhThuToHour]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
    		SELECT
    		hdb.ID as ID_HoaDon,
			DATEPART(HOUR, hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		ISNULL(hdct.ThanhTien, 0) as ThanhTien
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		--and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
		) a
    	GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_DoanhThuChiNhanh]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"SELECT 
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
    		SELECT
			dv.TenDonVi as TenChiNhanh,
    		ISNULL(hdct.ThanhTien, 0) as ThanhTien
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
		) a
    	GROUP BY a.TenChiNhanh");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_DoanhThuToDay]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT *,
		CASE WHEN HH.DoanhThuThangTruoc = 0 then 100 else CAST(ROUND(( CAST(HH.DoanhThuThangNay - HH.DoanhThuThangTruoc as float) / HH.DoanhThuThangTruoc) * 100, 2) as float) end as SoSanhCungKy
    	FROM
    	(
    	SELECT 
			CAST(ROUND(SUM(b.SoLuongBan), 0) as float) as SoLuongBan,
			CAST(ROUND(SUM(b.SoLuongTra), 0) as float) as SoLuongTra,
			CAST(ROUND(SUM(b.GiaTriBan), 0) as float) as ThanhTien,
			CAST(ROUND(SUM(b.GiaTriTra), 0) as float) as GiaTriTra,
			CAST(ROUND(SUM(b.DoanhThuThangNay), 0) as float) as DoanhThuThangNay,
			CAST(ROUND(SUM(b.DoanhThuThangTruoc), 0) as float) as DoanhThuThangTruoc
    	FROM 
    	(
		SELECT 
			ISNULL(a.SoLuongBan, 0) as SoLuongBan,
			ISNULL(a.SoLuongTra, 0) as SoLuongTra,
    		SUM(ISNULL(a.GiaTriBan, 0)) as GiaTriBan,
    		SUM(ISNULL(a.GiaTriTra, 0) + ISNULL(a.GiaTriTraNhanh, 0)) as GiaTriTra,
			Sum(ISNULL(a.GiaTriBan, 0) - ISNULL(a.GiaTriTra, 0) - ISNULL(a.GiaTriTraNhanh, 0)) as DoanhThuThangNay,
			Sum(ISNULL(a.GiaTriBanTruoc, 0) - ISNULL(a.GiaTriTraTruoc, 0) - ISNULL(a.GiaTriTraNhanhTruoc, 0)) as DoanhThuThangTruoc
    	FROM
    	(
		SELECT * from
		(
    		SELECT
    		hdb.ID as ID_HoaDon,
			1 as SoLuongBan,
			NULL as SoLuongTra,
    		ISNULL(hdct.ThanhTien, 0) as GiaTriBan,
    		NULL as GiaTriTra,
    		NULL as GiaTriTraNhanh,
					NULL as GiaTriBanTruoc,
    				NULL as GiaTriTraTruoc,
    				NULL as GiaTriTraNhanhTruoc
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi = @ID_ChiNhanh
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		UNION ALL
    		SELECT
    				hdt.ID as ID_HoaDon,
					NULL as SoLuongBan,
					1 as SoLuongTra,
    				NULL as GiaTriBan,				
    				ISNULL(hdct.ThanhTien, 0) as GiaTriTra,
    				NULL as GiaTriTraNhanh,
					NULL as GiaTriBanTruoc,
    				NULL as GiaTriTraTruoc,
    				NULL as GiaTriTraNhanhTruoc				
    		FROM BH_HoaDon hdb
    		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    			and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi = @ID_ChiNhanh
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			--and hdct.ID_ChiTietGoiDV is null
			UNION ALL
    		SELECT
    				hdb.ID as ID_HoaDon,
					NULL as SoLuongBan,
					1 as SoLuongTra,
    				NULL as GiaTriBan,
    				NULL as GiaTriTra,
    				ISNULL(hdct.ThanhTien, 0) as GiaTriTraNhanh,
					NULL as GiaTriBanTruoc,
    				NULL as GiaTriTraTruoc,
    				NULL as GiaTriTraNhanhTruoc
    		FROM
    		BH_HoaDon hdb
			left join BH_HoaDon hdg on hdb.ID_HoaDon = hdg.ID
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
			and (hdg.LoaiHoaDon = 1 Or hdg.LoaiHoaDon = 19)
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_HoaDon is null

			-- tháng trước
			Union all
			SELECT
    		hdb.ID as ID_HoaDon,
			NULL as SoLuongBan,
			NULL as SoLuongTra,
    		NULL as GiaTriBan,
    		NULL as GiaTriTra,
    		NULL as GiaTriTraNhanh,
			ISNULL(hdct.ThanhTien, 0) as GiaTriBanTruoc,
    		NULL as GiaTriTraTruoc,
    		NULL as GiaTriTraNhanhTruoc
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		where NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi = @ID_ChiNhanh
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		UNION ALL
    		SELECT
    				hdt.ID as ID_HoaDon,
					NULL as SoLuongBan,
					NULL as SoLuongTra,
    				NULL as GiaTriBan,				
    				NULL as GiaTriTra,
    				NULL as GiaTriTraNhanh,
					NULL as GiaTriBanTruoc,				
    				ISNULL(hdct.ThanhTien, 0) as GiaTriTraTruoc,
    				NULL as GiaTriTraNhanhTruoc							
    		FROM BH_HoaDon hdb
    		join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    		where hdt.NgayLapHoaDon >=  DateAdd(month, -1, @timeStart) and hdt.NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
    		and hdb.ChoThanhToan = 0
    			and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi = @ID_ChiNhanh
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			--and hdct.ID_ChiTietGoiDV is null
			UNION ALL
    		SELECT
    				hdb.ID as ID_HoaDon,
					NULL as SoLuongBan,
					NULL as SoLuongTra,
    				NULL as GiaTriBan,
    				NULL as GiaTriTra,
    				NULL as GiaTriTraNhanh,
					NULL as GiaTriBanTruoc,
    				NULL as GiaTriTraTruoc,
    				ISNULL(hdct.ThanhTien, 0) as GiaTriTraNhanhTruoc
    		FROM
    		BH_HoaDon hdb
			left join BH_HoaDon hdg on hdb.ID_HoaDon = hdg.ID
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		where hdb.NgayLapHoaDon >= DateAdd(month, -1, @timeStart)  and hdb.NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
			and (hdg.LoaiHoaDon = 1 Or hdg.LoaiHoaDon = 19)
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_HoaDon is null
			) d

    	) a
    		GROUP BY a.ID_HoaDon, a.SoLuongBan, a.SoLuongTra
    		) b
    		) as HH");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_HangBanTheoDoanhThu]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"SELECT TOP(10)
		a.MaHangHoa,
		a.TenHangHoa,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
    		SELECT
			dvqd.MaHangHoa, 
			hh.TenHangHoa,
    		ISNULL(hdct.ThanhTien, 0) as ThanhTien
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
		) a
    	GROUP BY a.MaHangHoa, a.TenHangHoa
		ORDER BY ThanhTien DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_HangBanTheoSoLuong]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"SELECT TOP(10)
		a.MaHangHoa,
		a.TenHangHoa,
		CAST(ROUND(SUM(a.SoLuong), 0) as float) as SoLuong
		FROM
		(
    		SELECT
			dvqd.MaHangHoa, 
			hh.TenHangHoa,
    		ISNULL(hdct.SoLuong, 0) as SoLuong
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
		) a
    	GROUP BY a.MaHangHoa, a.TenHangHoa
		ORDER BY SoLuong DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_NhatKyHoatDong]", parametersAction: p => new
            {
                ID_DonVi = p.Guid()
            }, body: @"SELECT TOP(12)
		MAX(a.TenNhanVien) as TenNhanVien,
		a.MaHoaDon,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien,
		MAX(a.NgayLapHoaDon) as NgayGoc,
    	CONVERT(VARCHAR, MAX(a.NgayLapHoaDon), 22) as NgayLapHoaDon,
		CASE 
			WHEN a.LoaiHoaDon = 1 then N'bán đơn hàng'  
			WHEN a.LoaiHoaDon = 3 then N'nhập đơn đặt hàng' 
			WHEN a.LoaiHoaDon = 4 then N'nhập kho đơn hàng'  
			WHEN a.LoaiHoaDon = 6 then N'nhận hàng trả'  
			WHEN a.LoaiHoaDon = 7 then N'trả hàng nhà cung cấp'  
			WHEN a.LoaiHoaDon = 8 then N'xuất kho đơn hàng'  
			Else N'bán gói dịch vụ'
		END as TenLoaiChungTu
		FROM
		(
    		SELECT
			hdb.ID as ID_HoaDon,
			hdb.MaHoaDon,
			nv.TenNhanVien,
			hdb.LoaiHoaDon,
			hdb.NgayLapHoaDon,
    		ISNULL(hdct.ThanhTien, 0) as ThanhTien
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			join NS_NhanVien nv on hdb.ID_NhanVien = nv.ID
    		where hdb.ID_DonVi = @ID_DonVi
    		and hdb.ChoThanhToan = 0
    		and hdb.LoaiHoaDon in (1,3,4,5,6,7,8,19)
			and hdct.ID_ChiTietGoiDV is null
		) a
    	GROUP BY a.ID_HoaDon, a.LoaiHoaDon, a.MaHoaDon
		ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[SP_GetChiTietHoaDon_afterTraHang]", parametersAction: p => new
            {
                ID_HoaDon = p.String()
            }, body: @"select ct1.ID, ct1.ID_HoaDon, max(ct1.SoLuong)- MAX(ISNULL(hdt.SoLuong,0))  as SoLuong, ct1.ID_DonViQuiDoi, 
    				ct1.DonGia,ct1.GiaVon,ct1.ThanhTien,qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa, max(ISNULL(ct1.TienChietKhau,0)) as GiamGia, 
    				ct1.ThoiGian, ct1.GhiChu,ct1.ID_LoHang, CAST((ct1.SoThuTu) as float) as SoThuTu , ct1.TangKem,ct1.ID_TangKem,
    				hh.LaHangHoa, hh.TenHangHoa
    		from BH_HoaDon_ChiTiet ct1
    		join DonViQuiDoi qd on ct1.ID_DonViQuiDoi = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
			--- get CTHD Tra
			left join 
					(select hd3.ID_HoaDon as ID_HoaDonGoc, ct3.ID_DonViQuiDoi, SUM(ISNULL(ct3.SoLuong,0)) as SoLuong 
					from BH_HoaDon_ChiTiet ct3
					join BH_HoaDon hd3 on ct3.ID_HoaDon = hd3.ID 
					where hd3.ChoThanhToan ='0'  
					group by hd3.ID_HoaDon, ct3.ID_DonViQuiDoi
					) hdt on ct1.ID_DonViQuiDoi = hdt.ID_DonViQuiDoi and ct1.ID_HoaDon = hdt.ID_HoaDonGoc
    		where ct1.ID_HoaDon = @ID_HoaDon
    		-- khong lay TP dinh luong
    		and (ct1.ID_ChiTietDinhLuong is null or ct1.ID_ChiTietDinhLuong = ct1.ID)
    		group by ct1.ID,ct1.ID_DonViQuiDoi,ct1.ID, ct1.ID_HoaDon,ct1.DonGia,ct1.GiaVon,ct1.ThanhTien,qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,
    		ct1.ThoiGian, ct1.GhiChu,ct1.ID_LoHang, ct1.SoThuTu, ct1.TangKem,ct1.ID_TangKem,hh.LaHangHoa, hh.TenHangHoa
    		-- chi lay Hang co SoLuongConLai > 0
    		Having max(ct1.SoLuong)- MAX(ISNULL(hdt.SoLuong,0)) > 0");

            CreateStoredProcedure(name: "[dbo].[SP_GetMaHDDatHang_Max]",
                body: @"DECLARE @intReturn int;

		SELECT @intReturn = COUNT(MaHoaDon)
    	FROM BH_HoaDon WHERE CHARINDEX('DHO',MaHoaDon) = 0 and CHARINDEX('DH',MaHoaDon) > 0

		IF @intReturn = 0 SELECT 0
		ELSE
			BEGIN
    			SELECT  MAX(CAST (dbo.udf_GetNumeric(MaHoaDon) AS INT)) MaxCode
    			FROM BH_HoaDon WHERE CHARINDEX('DHO',MaHoaDon) = 0 and CHARINDEX('DH',MaHoaDon) > 0
			END	");

            CreateStoredProcedure(name: "[dbo].[SP_GetQuyen_ByIDNguoiDung]", parametersAction: p => new
            {
                ID_NguoiDung = p.String(),
                ID_DonVi = p.String()
            }, body: @"DECLARE @LaAdmin bit

	select @LaAdmin= LaAdmin from HT_NguoiDung where ID like @ID_NguoiDung

	-- LaAdmin: full quyen, assign ID ='00000000-0000-0000-0000-000000000000' --> because class HT_Quyen_NhomDTO {ID, MaQuyen}
	if @LaAdmin	='1'
		select NEWID() as ID,  MaQuyen from HT_Quyen where DuocSuDung = '1'
	else	
		select NEWID() as  ID, MaQuyen 
		from HT_NguoiDung_Nhom nnd
		JOIN HT_Quyen_Nhom qn on nnd.IDNhomNguoiDung = qn.ID_NhomNguoiDung
		where nnd.IDNguoiDung like @ID_NguoiDung and nnd.ID_DonVi like @ID_DonVi");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_BieuDoDoanhThuToDay]");
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_BieuDoDoanhThuToHour]");
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_DoanhThuChiNhanh]");
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_DoanhThuToDay]");
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_HangBanTheoDoanhThu]");
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_HangBanTheoSoLuong]");
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_NhatKyHoatDong]");
            DropStoredProcedure("[dbo].[SP_GetChiTietHoaDon_afterTraHang]");
            DropStoredProcedure("[dbo].[SP_GetMaHDDatHang_Max]");
            DropStoredProcedure("[dbo].[SP_GetQuyen_ByIDNguoiDung]");
        }
    }
}
