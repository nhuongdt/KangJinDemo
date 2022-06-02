namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190125 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoNam]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                year = p.Int(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                ID_NhomDoiTuong_SP = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.String(),
                LoaiTien = p.String()
            }, body: @"--	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	SELECT
			ID_KhoanThuChi,
			CASE When c.LoaiThuChi = 3 then N'Thu tiền bán hàng'
				When c.LoaiThuChi = 5 then N'Thu trả hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Thu mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    			 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    			 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    			 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    			 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
			and a.LoaiHoaDon = 11
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    				and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
			DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
			and a.LoaiHoaDon = 12
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select ID_KhoanThuChi,
			KhoanMuc, 
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY ID_KhoanThuChi, KhoanMuc
			order by MAX(STT)");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoQuy]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                year = p.Int(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                ID_NhomDoiTuong_SP = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.String(),
                LoaiTien = p.String()
            }, body: @"--	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	SELECT
			ID_KhoanThuChi,
			CASE When c.LoaiThuChi = 3 then N'Thu tiền bán hàng'
				When c.LoaiThuChi = 5 then N'Thu trả hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Thu mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    			 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    			 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    			 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    			 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
			and a.LoaiHoaDon = 11
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    				and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
			and a.LoaiHoaDon = 12
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select ID_KhoanThuChi,
			KhoanMuc, 
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) as Quy1,
			ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) as Quy2,
			ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + ISNULL(SUM(Thang9),0) as Quy3,
			ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as Quy4,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY ID_KhoanThuChi, KhoanMuc
			order by MAX(STT)");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoThang]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                year = p.Int(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                ID_NhomDoiTuong_SP = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.String(),
                LoaiTien = p.String()
            }, body: @"--	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	SELECT
			ID_KhoanThuChi,
			CASE When c.LoaiThuChi = 3 then N'Thu tiền bán hàng'
				When c.LoaiThuChi = 5 then N'Thu trả hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Thu mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienMat,
				a.TienGui,
				a.TienThu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    			 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    			 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    			 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    			 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
			and a.LoaiHoaDon = 11
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    				and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienMat,
				a.TienGui,
				a.TienThu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
			and a.LoaiHoaDon = 12
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select ID_KhoanThuChi,
			KhoanMuc, 
			CAST(ROUND(SUM(Thang1), 0) as float) as Thang1,
			CAST(ROUND(SUM(Thang2), 0) as float) as Thang2,
			CAST(ROUND(SUM(Thang3), 0) as float) as Thang3,
			CAST(ROUND(SUM(Thang4), 0) as float) as Thang4,
			CAST(ROUND(SUM(Thang5), 0) as float) as Thang5,
			CAST(ROUND(SUM(Thang6), 0) as float) as Thang6,
			CAST(ROUND(SUM(Thang7), 0) as float) as Thang7,
			CAST(ROUND(SUM(Thang8), 0) as float) as Thang8,
			CAST(ROUND(SUM(Thang9), 0) as float) as Thang9,
			CAST(ROUND(SUM(Thang10), 0) as float) as Thang10,
			CAST(ROUND(SUM(Thang11), 0) as float) as Thang11,
			CAST(ROUND(SUM(Thang12), 0) as float) as Thang12,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY ID_KhoanThuChi, KhoanMuc
			order by MAX(STT)");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_SoQuyTheoChiNhanh]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                ID_NhomDoiTuong_SP = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.String()
            }, body: @"SELECT
			c.ID_DonVi,
			MAX(c.TenDonVi) as TenDonVi,
			CAST(ROUND(SUM(c.ThuTienMat - c.ChiTienMat), 0) as float) as TienMat,
			CAST(ROUND(SUM(c.ThuTienGui - c.ChiTienGui), 0) as float) as TienGui,
			CAST(ROUND(SUM(c.ThuTienMat - c.ChiTienMat + c.ThuTienGui - c.ChiTienGui), 0) as float) as TongThu
    	  FROM 
    		(
    		 SELECT 
				MAX(b.ID_DonVi) as ID_DonVi,
    			MAX(b.TenDonVi) as TenDonVi,
    			MAX (b.ThuTienGui) as ThuTienGui,
    			MAX (b.ChiTienGui) as ChiTienGui, 
    			MAX (b.ThuTienMat) as ThuTienMat,
    			MAX (b.ChiTienMat) as ChiTienMat
    		FROM
    		(
				select 
				a.ID_DonVi, 
				a.TenDonVi,
    			a.HachToanKinhDoanh,
    			a.ID_NhomDoiTuong,
    			a.ID_DoiTuong,
    			a.ID_HoaDon,
    			a.MaHoaDon,
    			a.MaPhieuThu,
    			case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
    			Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				MAX(qhd.ID_DonVi) as ID_DonVi,
				MAX(dv.TenDonVi) as TenDonVi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
				inner join DM_DonVi dv on qhd.ID_DonVi = dv.ID
    		where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon
    		) as c
			GROUP BY c.ID_DonVi");

            
            CreateStoredProcedure(name: "[dbo].[SP_AddChietKhau_ByIDNhom]", parametersAction: p => new
            {
                ID_NhomHangs = p.String(),
                ID_NhanVien = p.String(),
                ID_DonVi = p.String()
            }, body: @"DECLARE @i int = 0
	DECLARE @IDQuiDoi uniqueidentifier
	SET @i= 
		(SELECT COUNT(*) FROM DM_HangHoa hh
		join DonViQuiDoi qd ON hh.iD= qd.ID_HangHoa
		WHERE hh.ID_NhomHang in (Select * from splitstring(@ID_NhomHangs)))

	WHILE @i>0
		BEGIN
			SELECT @IDQuiDoi= tb.ID FROM (
				SELECT ROW_NUMBER() OVER (Order by qd.ID) AS RowNumber, qd.ID FROM DM_HangHoa hh
				join DonViQuiDoi qd ON hh.iD= qd.ID_HangHoa
				WHERE hh.ID_NhomHang in (Select * from splitstring(@ID_NhomHangs))
				and qd.ID not in (select ID_DonViQuiDoi from ChietKhauMacDinh_NhanVien where ID_NhanVien like @ID_NhanVien  and ID_DonVi like @ID_DonVi)
				) tb
			WHERE tb.RowNumber = @i
			SET @i= @i -1;

			if @IDQuiDoi is not null
				INSERT INTO ChietKhauMacDinh_NhanVien (ID, ID_NhanVien, ID_DonVi, ChietKhau, LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan,ID_DonViQuiDoi, NgayNhap)
				values ( NEWID(),@ID_NhanVien,@ID_DonVi, 0,'0',0,'0',0,'0',@IDQuiDoi,getdate())
		END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetInfor_TPDinhLuong]
    @ID_DonVi [nvarchar](max),
    @ID_DichVu [nvarchar](max)
AS
BEGIN
	select ID, ID_DichVu, ID_DonViQuiDoi,TenHangHoa,MaHangHoa,SoLuong,SoLuongMacDinh,SoLuongDinhLuong_BanDau,SoLuongQuyCach,TyLeChuyenDoi,DonViTinhQuyCach,TenDonViTinh, QuyCach,max(GiaVon) as GiaVon
	from (
		select dl.ID, dl.ID_DichVu, dl.ID_DonViQuiDoi,TenHangHoa,MaHangHoa,
			CAST(dl.SoLuong as float) as SoLuong,	
			-- used to first load
			CAST(dl.SoLuong as float) as SoLuongMacDinh,
			-- used to save in CTHD
			CAST(dl.SoLuong as float) as SoLuongDinhLuong_BanDau,
			case when QuyCach is null or QuyCach <=1 then 1
			else QuyCach * TyLeChuyenDoi end as QuyCach,
			-- SoLuongQuyCach = SoLuong * QuyCach * TyLeChuyenDoi
			case when QuyCach is null or QuyCach <=1 then dl.SoLuong
			else dl.SoLuong * QuyCach * TyLeChuyenDoi end as SoLuongQuyCach,
			CAST(ISNULL(TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
			case when gv.ID_DonVi like @ID_DonVi then CAST(gv.GiaVon as float)
			else 0 end as GiaVon,
			ISNULL(hh.DonViTinhQuyCach,'') as DonViTinhQuyCach,
			ISNULL(qd.TenDonViTinh,'') as TenDonViTinh
		from DinhLuongDichVu dl
		left join DM_GiaVon gv on dl.ID_DonViQuiDoi = gv.ID_DonViQuiDoi
		left join DonViQuiDoi qd on qd.ID= dl.ID_DonViQuiDoi
		left join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
		where dl.ID_DichVu like @ID_DichVu
	) dt group by dt.ID, dt.ID_DichVu, dt.ID_DonViQuiDoi,TenHangHoa,MaHangHoa,SoLuong,SoLuongMacDinh, QuyCach,TyLeChuyenDoi, DonViTinhQuyCach, TenDonViTinh, SoLuongQuyCach,SoLuongDinhLuong_BanDau
END

--SP_GetInfor_TPDinhLuong 'da196f82-db4c-41bd-a1cf-4a173223f9c1','16C2529B-2507-4BA4-BC9E-4E8BF87B64CB'

");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoNam]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoQuy]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoThang]");
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_SoQuyTheoChiNhanh]");
            DropStoredProcedure("[dbo].[SP_AddChietKhau_ByIDNhom]");
        }
    }
}
