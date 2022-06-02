namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20211010 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN]", parametersAction: p => new
            {
                ID_PhieuTiepNhan = p.Guid(),
                ChenhLech_SoKM = p.Double()
            }, body: @"SET NOCOUNT ON;

	declare @SoKmMacDinhNgay int= 30, @ChenhLechNgay int
	 set @ChenhLechNgay = CEILING(@ChenhLech_SoKM/@SoKmMacDinhNgay)

	 if @ChenhLechNgay != 0
	 begin
		update lichNew set lichNew.SoKmBaoDuong = lichNew.SoKmBaoDuong + @ChenhLech_SoKM,
							lichNew.NgayBaoDuongDuKien = DATEADD(DAY, @ChenhLechNgay,lichNew.NgayBaoDuongDuKien)
		from Gara_LichBaoDuong lichNew
		where exists(
		select lich.ID
		from Gara_LichBaoDuong lich
		join BH_HoaDon hd on lich.ID_HoaDon= hd.ID
		where hd.ID_PhieuTiepNhan= @ID_PhieuTiepNhan
		and lich.SoKmBaoDuong > 0
		and lich.TrangThai= 1
		and lich.ID= lichNew.ID
		)
	end");

            Sql(@"ALTER FUNCTION [dbo].[GetDayOfWeek_byPhieuPhanCa]
(
	@ID_PhieuPhanCa uniqueidentifier,
	@ID_CaLamViec uniqueidentifier,
	@DateCompare datetime
)
RETURNS varchar(2)
AS
BEGIN
	declare @Exist varchar(2) ='1'
	declare @dateOfWeek int = DATEPART(WEEKDAY,@DateCompare)

	declare @count int = (select count(ID)
		from NS_PhieuPhanCa_CaLamViec
		where ID_PhieuPhanCa= @ID_PhieuPhanCa
		and ID_CaLamViec =  @ID_CaLamViec
		and GiaTri + 1 = @dateOfWeek)		

	set @Exist = iif(@count > 0,'0','1')
	return @Exist ---- neu bangnhau: khong disable
	
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoNam_v2]
    @year [int],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit],
    @LoaiTien [nvarchar](max)
AS
BEGIN
    set nocount on;
    		declare @tblNhomDT table(ID_NhomDoiTuong varchar(40))
    		insert into @tblNhomDT
    		select * from dbo.splitstring(@ID_NhomDoiTuong)
    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
    		Thang11 float, Thang12 float, STT int)
    		-- thu tiền
    	Insert INTO @tmp
    	select 
    			b.ID_KhoanThuChi,
    			b.KhoanMuc,
    			sum(b.Thang1) as Thang1,
    			sum(b.Thang2) as Thang2,
    			sum(b.Thang3) as Thang3,
    			sum(b.Thang4) as Thang4,
    			sum(b.Thang5) as Thang5,
    			sum(b.Thang6) as Thang6,
    			sum(b.Thang7) as Thang7,
    			sum(b.Thang8) as Thang8,
    			sum(b.Thang9) as Thang9,
    			sum(b.Thang10) as Thang10,
    			sum(b.Thang11) as Thang11,
    			sum(b.Thang12) as Thang12,
    			max(STT) as STT
    
    	from
    	(
    		select 
    			a.ID_KhoanThuChi,
    			case a.LoaiThuChi
    			when 3 then N'Thu tiền bán hàng'
    			when 5 then N'Thu trả hàng nhà cung cấp'		
    			else case when a.ID_KhoanThuChi is null then N'Thu mặc định' else NoiDungThuChi end end as KhoanMuc,
    			case when a.ThangLapHoaDon = 1 then tienthu end as Thang1,
    			case when a.ThangLapHoaDon = 2 then tienthu end as Thang2,
    			case when a.ThangLapHoaDon = 3 then tienthu end as Thang3,
    			case when a.ThangLapHoaDon = 4 then tienthu end as Thang4,
    			case when a.ThangLapHoaDon = 5 then tienthu end as Thang5,
    			case when a.ThangLapHoaDon = 6 then tienthu end as Thang6,
    			case when a.ThangLapHoaDon = 7 then tienthu end as Thang7,
    			case when a.ThangLapHoaDon = 8 then tienthu end as Thang8,
    			case when a.ThangLapHoaDon = 9 then tienthu end as Thang9,
    			case when a.ThangLapHoaDon = 10 then tienthu end as Thang10,
    			case when a.ThangLapHoaDon = 11 then tienthu end as Thang11,
    			case when a.ThangLapHoaDon = 12 then tienthu end as Thang12		,
    			ROW_NUMBER() OVER(ORDER BY a.NoiDungThuChi) as STT		
    		from
    		(
    		select 
    			--a1.ID_NhomDoiTuong,
    				a1.LoaiThuChi,			
    				a1.ID_KhoanThuChi,
    				a1.NoiDungThuChi,
    			a1.ThangLapHoaDon,
    				Case when @LoaiTien = '%1%' then a1.TienMat
    					when @LoaiTien = '%2%' then a1.TienGui else a1.tienmat + a1.TienGui end as TienThu,
    			Case when a1.TienMat > 0 and TienGui = 0 then '1'  
    			 when a1.TienGui > 0 and TienMat = 0 then '2' 
    			 when a1.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    				select    					
    					qhdct.ID_KhoanThuChi,
    				ktc.NoiDungThuChi,
    				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    				Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    				 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    				 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    				 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    				 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    				 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    					case when dt.LoaiDoiTuong= 1 then	
    					case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end
    					else
    						case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000002' else dt.IDNhomDoiTuongs end end as ID_NhomDoiTuong,   
    					tienmat, tiengui,
    				tienmat +  tiengui as TienThu,
    					DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
    				hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan= 0)
    			and qhd.LoaiHoaDon = 11
    				and qhdct.HinhThucThanhToan != 6
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0' OR qhd.PhieuDieuChinhCongNo = 2) 
    				) a1
    				where (EXISTS(SELECT Name FROM splitstring(a1.ID_NhomDoiTuong) dtDB inner JOIN @tblNhomDT dtS ON dtDB.Name = dtS.ID_NhomDoiTuong) 
    				or @ID_NhomDoiTuong = '')
    				--where  (a1.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    		) a where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) --and a.LoaiTien like @LoaiTien
    	) b group by b.ID_KhoanThuChi, b.KhoanMuc
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
    				Case when @LoaiTien = '%1%' then SUM(b.TienMat)
    				when @LoaiTien = '%2%' then SUM(b.TienGui) else
    				SUM(b.TienMat + b.TienGui) end as TienThu
    				--MAX(b.TienMat + b.TienGui) as TienThu
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
    			MAX(qhd.ID) as ID_HoaDon,
    				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,    		
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
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
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan = 0)
    			and qhd.LoaiHoaDon = 12
    				and qhdct.HinhThucThanhToan != 6
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0' OR qhd.PhieuDieuChinhCongNo = 2)
    				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong, qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    				where LoaiTien like @LoaiTien
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
    			select *
    			from
    			(
    			select max(ID_KhoanThuChi) as ID_KhoanThuChi, -- deu chi tien nhaphang, nhưng ID_KhoanThuChi # nhau --> thi bi douple, nen chi group tho KhoanMua va lay max (ID_KhoanThuChi)
    			KhoanMuc, 
    			MAX(STT) as STT,
    			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
    			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
    			from @tmp
    			GROUP BY KhoanMuc
    			) tblview where TongCong > 0
    			order by STT
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoQuy_v2]
    @year [int],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit],
    @LoaiTien [nvarchar](max)
AS
BEGIN
    set nocount on;
    		declare @tblNhomDT table(ID_NhomDoiTuong varchar(40))
    		insert into @tblNhomDT
    		select * from dbo.splitstring(@ID_NhomDoiTuong)
    
    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
    		Thang11 float, Thang12 float, STT int)
    		-- thu tiền
    	Insert INTO @tmp
    	select 
    			b.ID_KhoanThuChi,
    			b.KhoanMuc,
    			sum(b.Thang1) as Thang1,
    			sum(b.Thang2) as Thang2,
    			sum(b.Thang3) as Thang3,
    			sum(b.Thang4) as Thang4,
    			sum(b.Thang5) as Thang5,
    			sum(b.Thang6) as Thang6,
    			sum(b.Thang7) as Thang7,
    			sum(b.Thang8) as Thang8,
    			sum(b.Thang9) as Thang9,
    			sum(b.Thang10) as Thang10,
    			sum(b.Thang11) as Thang11,
    			sum(b.Thang12) as Thang12,
    			max(STT) as STT
    
    	from
    	(
    		select 
    			a.ID_KhoanThuChi,
    			case a.LoaiThuChi
    			when 3 then N'Thu tiền bán hàng'
    			when 5 then N'Thu trả hàng nhà cung cấp'		
    			else case when a.ID_KhoanThuChi is null then N'Thu mặc định' else NoiDungThuChi end end as KhoanMuc,
    			case when a.ThangLapHoaDon = 1 then tienthu end as Thang1,
    			case when a.ThangLapHoaDon = 2 then tienthu end as Thang2,
    			case when a.ThangLapHoaDon = 3 then tienthu end as Thang3,
    			case when a.ThangLapHoaDon = 4 then tienthu end as Thang4,
    			case when a.ThangLapHoaDon = 5 then tienthu end as Thang5,
    			case when a.ThangLapHoaDon = 6 then tienthu end as Thang6,
    			case when a.ThangLapHoaDon = 7 then tienthu end as Thang7,
    			case when a.ThangLapHoaDon = 8 then tienthu end as Thang8,
    			case when a.ThangLapHoaDon = 9 then tienthu end as Thang9,
    			case when a.ThangLapHoaDon = 10 then tienthu end as Thang10,
    			case when a.ThangLapHoaDon = 11 then tienthu end as Thang11,
    			case when a.ThangLapHoaDon = 12 then tienthu end as Thang12		,
    			ROW_NUMBER() OVER(ORDER BY a.NoiDungThuChi) as STT		
    		from
    		(
    		select 
    			--a1.ID_NhomDoiTuong,
    				a1.LoaiThuChi,			
    				a1.ID_KhoanThuChi,
    				a1.NoiDungThuChi,
    			a1.ThangLapHoaDon,
    				Case when @LoaiTien = '%1%' then a1.TienMat
    					when @LoaiTien = '%2%' then a1.TienGui else a1.tienmat + a1.TienGui end as TienThu,
    			Case when a1.TienMat > 0 and TienGui = 0 then '1'  
    			 when a1.TienGui > 0 and TienMat = 0 then '2' 
    			 when a1.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    				select    					
    					qhdct.ID_KhoanThuChi,
    				ktc.NoiDungThuChi,
    				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    				Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    				 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    				 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    				 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    				 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    				 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    					case when dt.LoaiDoiTuong= 1 then	
    					case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end
    					else
    						case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000002' else dt.IDNhomDoiTuongs end end as ID_NhomDoiTuong,   
    					tienmat, tiengui,
    				tienmat +  tiengui as TienThu,
    					DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
    				hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan= 0)
    			and qhd.LoaiHoaDon = 11
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0' OR qhd.PhieuDieuChinhCongNo = 2) 
    				and qhdct.HinhThucThanhToan != 6
    				) a1
    				where (EXISTS(SELECT Name FROM splitstring(a1.ID_NhomDoiTuong) dtDB inner JOIN @tblNhomDT dtS ON dtDB.Name = dtS.ID_NhomDoiTuong) 
    				or @ID_NhomDoiTuong = '')
    				--where  (a1.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    		) a where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) --and a.LoaiTien like @LoaiTien
    	) b group by b.ID_KhoanThuChi, b.KhoanMuc
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
    				Case when @LoaiTien = '%1%' then SUM(b.TienMat)
    				when @LoaiTien = '%2%' then SUM(b.TienGui) else
    				SUM(b.TienMat + b.TienGui) end as TienThu
    				--MAX(b.TienMat + b.TienGui) as TienThu
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
    			MAX(qhd.ID) as ID_HoaDon,
    				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
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
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and qhd.LoaiHoaDon = 12
    				and qhdct.HinhThucThanhToan != 6
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0' OR qhd.PhieuDieuChinhCongNo = 2) -- DieuChinh CongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID, qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong, qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    				where LoaiTien like @LoaiTien
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
    			select *
    			from
    			(
    			select max(ID_KhoanThuChi) as ID_KhoanThuChi,
    			KhoanMuc, 
    			MAX(STT) as STT,
    			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) as Quy1,
    			ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) as Quy2,
    			ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + ISNULL(SUM(Thang9),0) as Quy3,
    			ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as Quy4,
    			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
    			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
    			from @tmp
    			GROUP BY  KhoanMuc  
    			) tblview where TongCong > 0
    			order by STT
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoThang_v2]
    @year [int],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit],
    @LoaiTien [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    --	tinh ton dau ky
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
    				Case when @LoaiTien = '%1%' then SUM(b.TienMat)
    				when @LoaiTien = '%2%' then SUM(b.TienGui) else
    				SUM(b.TienMat + b.TienGui) end as TienThu
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
    			MAX(qhd.ID) as ID_HoaDon,
    				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    			 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    			 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    			 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    			 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    			--Case When dtn.ID_NhomDoiTuong is null then
    			--Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    				case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end as ID_NhomDoiTuong,
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
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
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan = 0 or qhdct.DiemThanhToan is null)
    				and qhd.LoaiHoaDon = 11
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				AND qhdct.HinhThucThanhToan != 6
    				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0' OR qhd.PhieuDieuChinhCongNo = 2) -- DieuChinh CongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID, qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi, 
    			qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dt.IDNhomDoiTuongs, qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    				where LoaiTien like @LoaiTien OR @LoaiTien = ''
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
    				Case when @LoaiTien = '%1%' then SUM(b.TienMat)
    				when @LoaiTien = '%2%' then SUM(b.TienGui) else
    				SUM(b.TienMat + b.TienGui) end as TienThu
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
    			MAX(qhd.ID) as ID_HoaDon,
    				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
    				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon
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
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    				and qhd.LoaiHoaDon = 12
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				AND qhdct.HinhThucThanhToan != 6
    				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0' OR qhd.PhieuDieuChinhCongNo = 2) -- dcCongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID,qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong,qhdct.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    				where LoaiTien like @LoaiTien OR @LoaiTien = ''
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
    			select max(ID_KhoanThuChi) as ID_KhoanThuChi, -- deu chi tien nhaphang, nhưng ID_KhoanThuChi # nhau --> thi bi douple, nen chi group tho KhoanMua va lay max (ID_KhoanThuChi)
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
    			GROUP BY  KhoanMuc
    			order by MAX(STT)
END");

            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhMonth_ChiPhiBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
    SELECT
    	a.ThangLapHoaDon,
    	CAST(ROUND(SUM(a.GiaTriHuy), 0) as float) as GiaTriHuy,
    	CAST(ROUND(SUM(a.DiemThanhToan), 0) as float) as DiemThanhToan
    	FROM
    	(
    		Select 
    		DATEPART(MONTH, hd.NgayLapHoaDon) as ThangLapHoaDon,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		where hd.LoaiHoaDon = 8 and hd.ID_HoaDon is null and hd.ID_PhieuTiepNhan iS NULL
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		UNION ALL
    		Select 
    		DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.ThangLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhMonth_DoanhThuBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
	SET NOCOUNT ON;
    SELECT
    	a.ThangLapHoaDon,
    	CAST(ROUND(SUM(a.DoanhThu), 0) as float) as DoanhThu,
		CAST(ROUND(SUM(a.GiaVonGDV), 0) as float) as GiaVonGDV,
    	CAST(ROUND(SUM(a.GiaTriTra), 0) as float) as GiaTriTra,
    	CAST(ROUND(SUM(a.GiamGiaHDB - a.GiamGiaHDT), 0) as float) as GiamGiaHD
    	FROM
    	(
    		Select 
    		DATEPART(MONTH, hd.NgayLapHoaDon) as ThangLapHoaDon,
    		hd.LoaiHoaDon,
    		Case When hd.LoaiHoaDon in (1,19,25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon in (1,19,25) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		where hd.LoaiHoaDon in (1,19,25,6)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh)) 
			AND (hdct.ID_ChiTietDinhLuong IS NULL OR hdct.ID_ChiTietDinhLuong = hdct.ID)
			AND (hdct.ID_ParentCombo IS NULL OR hdct.ID_ParentCombo = hdct.ID)
			UNION ALL
			select MONTH(hdxk.NgayLapHoaDon) AS ThangLapHoaDon, 
			25, 
			0 AS DoanhThu, 
			hdxkct.SoLuong * hdxkct.GiaVon AS GiaVonGDV,
			0 AS GiaTriTra,
			0 AS GiamGiaHDB,
			0 AS GiamGiaHDT
			from Gara_PhieuTiepNhan ptn
			inner join BH_HoaDon hdxk ON ptn.ID = hdxk.ID_PhieuTiepNhan
			INNER JOIN BH_HoaDon_ChiTiet hdxkct ON hdxk.ID = hdxkct.ID_HoaDon
			where 
			--hdsc.LoaiHoaDon = 25 and hdsc.ChoThanhToan = 0 and
			hdxk.LoaiHoaDon = 8 AND
			hdxk.ChoThanhToan = 0 AND YEAR(hdxk.NgayLapHoaDon) = @year
			and hdxk.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	) as a
    	GROUP BY
    	a.ThangLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhYear_ChiPhiBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
    SELECT
    	a.NamLapHoaDon,
    	CAST(ROUND(SUM(a.GiaTriHuy), 0) as float) as GiaTriHuy,
    	CAST(ROUND(SUM(a.DiemThanhToan), 0) as float) as DiemThanhToan
    	FROM
    	(
    		Select 
    		DATEPART(YEAR, hd.NgayLapHoaDon) as NamLapHoaDon,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		where hd.LoaiHoaDon = 8 and hd.ID_HoaDon is null AND hd.ID_PhieuTiepNhan IS NULL
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		UNION ALL
    		Select 
    		DATEPART(YEAR, qhd.NgayLapHoaDon) as NamLapHoaDon,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.NamLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhYear_DoanhThuBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
    SELECT
    	a.NamLapHoaDon,
    	CAST(ROUND(SUM(a.DoanhThu), 0) as float) as DoanhThu,
		CAST(ROUND(SUM(a.GiaVonGDV), 0) as float) as GiaVonGDV,
    	CAST(ROUND(SUM(a.GiaTriTra), 0) as float) as GiaTriTra,
    	CAST(ROUND(SUM(a.GiamGiaHDB - a.GiamGiaHDT), 0) as float) as GiamGiaHD
    	FROM
    	(
    		Select 
    		DATEPART(YEAR, hd.NgayLapHoaDon) as NamLapHoaDon,
    		hd.LoaiHoaDon,
    		Case When hd.LoaiHoaDon in (1,19,25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon in (1,19,25) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		where hd.LoaiHoaDon in (1,6,19,25)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			AND (hdct.ID_ChiTietDinhLuong IS NULL OR hdct.ID_ChiTietDinhLuong = hdct.ID)
			AND (hdct.ID_ParentCombo IS NULL OR hdct.ID_ParentCombo = hdct.ID)
			UNION ALL
			select YEAR(hdxk.NgayLapHoaDon) AS ThangLapHoaDon, 
			25, 
			0 AS DoanhThu, 
			hdxkct.SoLuong * hdxkct.GiaVon AS GiaVonGDV,
			0 AS GiaTriTra,
			0 AS GiamGiaHDB,
			0 AS GiamGiaHDT
			from Gara_PhieuTiepNhan ptn
			inner join BH_HoaDon hdxk ON ptn.ID = hdxk.ID_PhieuTiepNhan
			INNER JOIN BH_HoaDon_ChiTiet hdxkct ON hdxk.ID = hdxkct.ID_HoaDon
			where hdxk.LoaiHoaDon = 8
			and hdxk.ChoThanhToan = 0 AND YEAR(hdxk.NgayLapHoaDon) = @year
			and hdxk.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	) as a
    	GROUP BY
    	a.NamLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateStatusBangLuong_whenChangeCong]
    @ID_DonVi [uniqueidentifier],
    @NgayChamCong [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	set @NgayChamCong = FORMAT(@NgayChamCong,'yyyy-MM-dd')
    
    	update bl1 set bl1.TrangThai= 2
		from NS_BangLuong bl1 
    	where exists (select ID
    					from
    						(select ID, FORMAT(TuNgay,'yyyy-MM-dd') as TuNgay, FORMAT(DenNgay,'yyyy-MM-dd') as DenNgay
    						from NS_BangLuong
    						where TrangThai = 1 and ID_DonVi= @ID_DonVi
    						) bl
    					where bl.TuNgay<= @ngaychamcong and bl.DenNgay >= @ngaychamcong and bl1.ID= bl.ID)
END");

            
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[UpdateLichBaoDuong_whenUpdateSoKM_ofPhieuTN]");
        }
    }
}
