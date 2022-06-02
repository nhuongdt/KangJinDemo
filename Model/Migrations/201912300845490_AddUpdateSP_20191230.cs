namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20191230 : DbMigration
    {
        public override void Up()
        {
			CreateStoredProcedure(name: "[dbo].[CheckThucThu_TongSuDung]", parametersAction: p => new
			{
				ID_DoiTuong = p.Guid(),
				ID_TheGiaTri = p.Guid()
			}, body: @"SET NOCOUNT ON;

	declare @tongthu float= 0
	declare @tongsudung float= 0

	declare @dateHD datetime = (select NgayLapHoaDon from  BH_HoaDon where ID = @ID_TheGiaTri)
	declare @dateHD_add1 datetime = (select top 1 NgayLapHoaDon from  BH_HoaDon where NgayLapHoaDon > @dateHD and LoaiHoaDon= 22 order by NgayLapHoaDon)
	if @dateHD_add1 is null
		set @dateHD_add1 = DATEADD(DAY,1,GETDATE())

	-- get tongthu den thoi diem hientai
	select 
		@tongthu = sum(qct.TienThu)
	from Quy_HoaDon qhd
	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
	join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
	where qct.ID_DoiTuong= @ID_DoiTuong
	and hd.ChoThanhToan is not null
	and hd.LoaiHoaDon= 22
	and qhd.TrangThai='1'
	and qhd.NgayLapHoaDon < @dateHD
	group by hd.ID_DoiTuong

	-- get tongsudung den hientai
	select 
		@tongsudung= sum(qct.TienThu)
	from Quy_HoaDon qhd
	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
	join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
	where qct.ID_DoiTuong= @ID_DoiTuong
	and hd.ChoThanhToan is not null
	and hd.LoaiHoaDon in (1,19)
	and qhd.TrangThai='1'
	and qhd.LoaiHoaDon = 11
	and qhd.NgayLapHoaDon < @dateHD_add1
	and hd.NgayLapHoaDon < @dateHD_add1
	and qct.ThuTuThe > 0
	group by hd.ID_DoiTuong

	declare @return bit='1'
	if @tongthu < @tongsudung 
		set @return='0'
	select @return as Exist");

			CreateStoredProcedure(name: "[dbo].[GetInvoiceUseServive_Newest]", parametersAction: p => new
			{
				ID_DoiTuong = p.Guid()
			}, body: @"SET NOCOUNT ON;

	select a.ID_DonViQuiDoi, qd.MaHangHoa, hh.TenHangHoa--,  a.NgayLapHoaDon, MaHoaDon, ID_ChiTietGoiDV
	from
		(select ct.ID_DonViQuiDoi--, hd.NgayLapHoaDon, hd.MaHoaDon,ct.ID_ChiTietGoiDV
		from
			(select top 1 hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon
			from BH_HoaDon hd	
			join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
			where hd.ID_DoiTuong= @ID_DoiTuong
			and hd.LoaiHoaDon = 1
			and ct.ID_ChiTietGoiDV is not null 
			group by hd.ID,hd.NgayLapHoaDon, hd.MaHoaDon
			order by hd.NgayLapHoaDon desc
			) hd
		join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
		where ct.ID_ChiTietGoiDV is not null and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
		) a
	join DonViQuiDoi qd on qd.id= a.ID_DonViQuiDoi
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID   
	where hh.LaHangHoa='0'");

			CreateStoredProcedure(name: "[dbo].[GetKhachHanghasDienThoai_byIDNhoms]", parametersAction: p => new
			{
				IDNhoms = p.String()
			}, body: @"SET NOCOUNT ON;

    declare @tblIDNhoms table (ID varchar(36))
	insert into @tblIDNhoms
	select Name from dbo.splitstring(@IDNhoms)

	select DienThoai
	from
		(select ID, ISNULL(IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') as IDNhom, DienThoai
		from DM_DoiTuong
		where LoaiDoiTuong = 1 and DienThoai is not null and DienThoai!='') a
	where EXISTS(SELECT Name FROM splitstring(a.IDNhom) nhom INNER JOIN @tblIDNhoms tblsearch ON nhom.Name = tblsearch.ID)");

			CreateStoredProcedure(name: "[dbo].[GetListCustomer_byIDs]", parametersAction: p => new
			{
				IDCustomers = p.String()
			}, body: @"SET NOCOUNT ON;
    SELECT MaDoiTuong, TenDoiTuong, DienThoai from DM_DoiTuong where exists (select Name from dbo.splitstring(@IDCustomers) where ID= Name)");

			CreateStoredProcedure(name: "[dbo].[GetListLichHen_FullCalendar]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				IDLoaiTuVans = p.String(),
				IDNhanVienPhuTrachs = p.String(),
				TrangThaiCVs = p.String(20),
				PhanLoai = p.String(20),
				DoUuTien = p.String(4),
				LoaiDoiTuong = p.String(10),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				IDKhachHang = p.String(40)
			}, body: @"SET NOCOUNT ON;

declare @tblCalendar table(ID uniqueidentifier,Ma_TieuDe nvarchar (max), ID_DonVi uniqueidentifier, ID_KhachHang uniqueidentifier,ID_LoaiTuVan uniqueidentifier null,
	ID_NhanVien uniqueidentifier, ID_NhanVienQuanLy uniqueidentifier null, 
	NgayTao datetime,NgayHenGap datetime, NgayGioKetThuc datetime null,NgayHoanThanh datetime null,
	TrangThai varchar(10), GhiChu nvarchar(max), NguoiTao nvarchar(max),
	MucDoUuTien int, KetQua nvarchar(max), NhacNho int null, KieuNhacNho int null,
	KieuLap int null, SoLanLap int null, GiaTriLap nvarchar(max) null,TuanLap int null, TrangThaiKetThuc int null,GiaTriKetThuc nvarchar(max), 
	ExistDB bit,ID_Parent uniqueidentifier null, NgayCu datetime null)	

--- table LichHen
select cs.ID, 
	Ma_TieuDe,
	cs.ID_DonVi, 
	ID_KhachHang, 
	ID_LoaiTuVan,
	ID_NhanVien,	
	ID_NhanVienQuanLy,
	NgayTao,
	NgayGio,
	NgayGioKetThuc,	
	NgayHoanThanh,
	ISNULL(KieuLap,0) as KieuLap,
	ISNULL(SoLanLap,0) as SoLanLap,
	ISNULL(GiaTriLap,'') as GiaTriLap,
	ISNULL(TuanLap,0) as TuanLap,
	ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
	ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc, 	
	ISNULL(SoLanDaHen,0) as SoLanDaHen,
	TrangThai,
	ISNULL(GhiChu,'') as GhiChu,
	NguoiTao,
	2 as MucDoUuTien,
	KetQua,
	NhacNho, 
	ISNULL(KieuNhacNho,0) as KieuNhacNho,
	cs.ID as ID_Parent,
	cs.NgayCu
into #temp
from ChamSocKhachHangs cs
left join ( select ISNULL(ID_Parent,'00000000-0000-0000-0000-000000000000') as ID_Parent,
		count(*) as SoLanDaHen
		from ChamSocKhachHangs
		where PhanLoai = 3
		group by ID_Parent) a on cs.ID= a.ID_Parent
where KieuLap in (1,2,3,4)
	and (TrangThaiKetThuc = 1 
	OR (TrangThaiKetThuc = 2 and ISNULL(GiaTriKetThuc,'')  >= CONVERT(varchar, @FromDate,23))
	OR (TrangThaiKetThuc = 3 and ISNULL(SoLanDaHen,0)  <= ISNULL(GiaTriKetThuc,0)) 
	)	
and PhanLoai = 3 

-- get row was update (ID_Parent !=null)
select ID, ID_Parent, NgayCu into #temp2 from ChamSocKhachHangs where ID_Parent is not null and PhanLoai = 3

set nocount on;
declare @ID uniqueidentifier, @Ma_TieuDe nvarchar(max), @ID_DonVi uniqueidentifier, @ID_KhachHang uniqueidentifier,@ID_LoaiTuVan uniqueidentifier, 
		@ID_NhanVien uniqueidentifier,@ID_NhanVienQuanLy uniqueidentifier,
		@NgayTao datetime,@NgayGio datetime,@NgayGioKetThuc datetime, @NgayHoanThanh datetime,
		@KieuLap int, @SoLanLap int, @GiaTriLap varchar(max), @TuanLap int, @TrangThaiKetThuc int,@GiaTriKetThuc varchar(max),			
		@SoLanDaHen int, @TrangThai varchar, @GhiChu nvarchar(max),
		@NguoiTao nvarchar(max), @MucDoUuTien int, @KetQua nvarchar(max), @NhacNho int, @KieuNhacNho int, @ID_Parent uniqueidentifier, @NgayCu datetime

		--- lap ngay
		declare _cur cursor
		for
			select * from #temp where KieuLap = 1 and SoLanLap > 0	
				and not exists (select ID from #temp2 where #temp2.ID = #temp.ID) --and TrangThai='1' 
		open _cur
		fetch next from _cur
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc, @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc, @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin		
				-- chi add row < @ToDate
				declare @dateadd datetime = @NgayGio
				declare @lanlap int = 1			
				while @dateadd < @ToDate 
					begin	
					
						if @TrangThaiKetThuc= 1 
							OR (@TrangThaiKetThuc = 2 and  @dateadd < @GiaTriKetThuc )  --- khong bao gio OR KetThuc vao ngay OR sau x lan (todo)
							OR (@TrangThaiKetThuc= 3 and @lanlap <= @GiaTriKetThuc - @SoLanDaHen)
							begin
								set @NgayGioKetThuc = DATEADD(hour,4,@dateadd)
								declare @newidDay uniqueidentifier = NEWID()
								declare @count1 int = 0;
								if @dateadd = @NgayGio set @newidDay = @ID		
								select @count1 = count(*) from #temp2 where ID_Parent = @ID_Parent 
									and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@dateadd,23)								
								if @count1 = 0											
									insert into @tblCalendar values (@newidDay,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
									@NgayTao, @dateadd,@NgayGioKetThuc, @NgayHoanThanh, @TrangThai,@GhiChu,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,
									@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc, IIF(@dateadd = @NgayGio,'1','0'),@ID_Parent, @NgayCu)																			
							end
						set @dateadd = DATEADD(day, @SoLanLap, @dateadd)
						set @lanlap= @lanlap + 1
					end
				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap,@TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,@ID_Parent, @NgayCu
			end
		close _cur;
		deallocate _cur;

		--- lap tuan
		declare _cur2 cursor
		for
			select * from #temp where KieuLap = 2 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID)  --and TrangThai='1' 
		open _cur2
		fetch next from _cur2
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin	
				declare @weekRepeat datetime = @NgayGio				
				declare @lanlapWeek int = 1
				while @weekRepeat < @ToDate -- lặp đến khi thuộc khoảng thời gian tìm kiếm
					begin	
								declare @firstOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 0)) -- lay ngay dau tien cua tuan
								declare @lastOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 7)) -- lay ngay cuoi cung cua tuan
								declare @dateRepeat datetime = @firstOfWeek	
								while @dateRepeat < @lastOfWeek -- tim kiem trong tuan duoc lap lai
									begin
										if dateadd(hour,23, @dateRepeat) >= @NgayGio
											begin
												declare @dateOfWeek varchar(1) = cast(DATEPART(WEEKDAY,@dateRepeat) as varchar(1)) -- lấy ngày trong tuần (thứ 2,3,..)	
												if @dateOfWeek = 1 set @dateOfWeek = 8
												declare @datefrom datetime = @dateRepeat
												set @NgayGioKetThuc = DATEADD(hour,2,@datefrom) -- add 2 hour

												if CHARINDEX(@dateOfWeek, @GiaTriLap ) > 0 
												and  (@TrangThaiKetThuc= 1 OR (@TrangThaiKetThuc = 2 and  @dateRepeat < @GiaTriKetThuc)
													OR (@TrangThaiKetThuc= 3 and @lanlapWeek <= @GiaTriKetThuc - @SoLanDaHen))
													begin														
														declare @newidWeek uniqueidentifier = NEWID()
														declare @exitDB bit='0'
														if convert(varchar(20),@dateRepeat,23) = convert(varchar(20),@NgayGio,23) 
															begin
																set @newidWeek = @ID
																set @exitDB ='1'
															end
														declare @count2 int=0
														select @count2 = count(*) from #temp2 where ID_Parent = @ID_Parent 
																and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@dateRepeat,23)								
														if @count2 = 0	
															begin
																insert into @tblCalendar values (@newidWeek,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
																		@NgayTao, @dateRepeat,@NgayGioKetThuc,  @NgayHoanThanh, 
																		@TrangThai,@GhiChu,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,
																		@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc,@exitDB, @ID_Parent, @NgayCu)
																set @lanlapWeek= @lanlapWeek + 1																
															end
													end												
											end										
										set @dateRepeat = DATEADD(day, 1, @dateRepeat)											
									end							
						set @weekRepeat = DATEADD(WEEK, @SoLanLap, @weekRepeat)	-- lap lai x tuan/lan	
					end			
				FETCH NEXT FROM _cur2 into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
			end
		close _cur2;
		deallocate _cur2;

		--- lap thang
		declare _cur cursor
		for
			select * from #temp where KieuLap = 3 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID)
		open _cur
		fetch next from _cur
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin		
				declare @monthRepeat datetime = @NgayGio	
				declare @lanlapMonth int = 1
				while @monthRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
					begin	
						if  @monthRepeat >= @FromDate			
							begin	
								declare @datefromMonth datetime= @monthRepeat
								set @NgayGioKetThuc = DATEADD(hour,2,@datefromMonth)
								 -- hàng tháng vào ngày ..xx..
								if	@TuanLap = 0 
									and (@TrangThaiKetThuc = 1 
									OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
									OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
									)
									begin
											declare @newidMonth1 uniqueidentifier = NEWID()
											if @monthRepeat = @NgayGio set @newidMonth1 = @ID
											declare @count3 int=0
											select @count3 = count(*) from #temp2 where ID_Parent = @ID_Parent 
													and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@monthRepeat,23)								
											if @count3 = 0	
											insert into @tblCalendar values (@newidMonth1,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
																@NgayTao, @monthRepeat,@NgayGioKetThuc,  @NgayHoanThanh,
																@TrangThai,@GhiChu,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, 
																@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc, IIF(@monthRepeat = @NgayGio,'1','0'), @ID_Parent, @NgayCu)
									end 
								else
									-- hàng tháng vào thứ ..x.. tuần thứ ..y.. của tháng
									begin
										declare @dateOfWeek_Month int = DATEPART(WEEKDAY,@monthRepeat) -- thu may trong tuan
										if @dateOfWeek_Month = 8 set @dateOfWeek_Month = 1
										declare @weekOfMonth int = DATEDIFF(WEEK, DATEADD(MONTH, DATEDIFF(MONTH, 0, @monthRepeat), 0), @monthRepeat) +1 -- tuan thu may cua thang
										if @dateOfWeek_Month = @GiaTriLap and @weekOfMonth = @TuanLap 
										and (@TrangThaiKetThuc = 1 
											OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
											OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
											)
											begin
												declare @newidMonth2 uniqueidentifier = NEWID()
												if @monthRepeat = @NgayGio set @newidMonth2 = @ID
												declare @count4 int=0
												select @count4 = count(*) from #temp2 where ID_Parent = @ID_Parent 
														and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@monthRepeat,23)								
												if @count4 = 0	
												insert into @tblCalendar values (@newidMonth2,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
																@NgayTao, @monthRepeat,@NgayGioKetThuc,  @NgayHoanThanh,
																@TrangThai,@GhiChu,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, 
																@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc,IIF(@monthRepeat = @NgayGio,'1','0'), @ID_Parent, @NgayCu)
											end
									end						
							end
						set @monthRepeat = DATEADD(MONTH, @SoLanLap, @monthRepeat)	-- lap lai x thang/lan	
						set @lanlapMonth = @lanlapMonth +1
					end			
				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
			end
		close _cur;
		deallocate _cur;


		--- lap nam
		declare _cur cursor
		for
			select * from #temp where KieuLap = 4 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID) 
		open _cur
		fetch next from _cur
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin		
				declare @yearRepeat datetime = @NgayGio	
				declare @lanlapYear int = 1
				while @yearRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
					begin						
						if  @yearRepeat >= @FromDate			
							begin	
								declare @dateOfMonth int = datepart(day,@yearRepeat)
								declare @monthOfYear int = datepart(MONTH,@yearRepeat)
								set @NgayGioKetThuc= DATEADD(hour,2, @yearRepeat)

								if @dateOfMonth = @GiaTriLap and @monthOfYear= @TuanLap
									and (@TrangThaiKetThuc = 1 
										OR (@TrangThaiKetThuc = 2 and @yearRepeat < @GiaTriKetThuc)
										OR (@TrangThaiKetThuc = 3 and @lanlapYear <= @GiaTriKetThuc - @SoLanDaHen)
										)
									begin
										declare @newidYear uniqueidentifier = NEWID()										
										if @yearRepeat = @NgayGio set @newidYear = @ID
										declare @count5 int=0
										select @count5 = count(*) from #temp2 where ID_Parent = @ID_Parent 
												and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@yearRepeat,23)								
										if @count5 = 0	
										insert into @tblCalendar values (@newidYear,@Ma_TieuDe, @ID_DonVi,@ID_KhachHang, @ID_LoaiTuVan, @ID_NhanVien, @ID_NhanVienQuanLy, 
															@NgayTao, @yearRepeat,@NgayGioKetThuc, @NgayHoanThanh, 
															@TrangThai,@GhiChu,@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, 
															@KieuLap, @SoLanLap, @GiaTriLap, @TuanLap, @TrangThaiKetThuc ,@GiaTriKetThuc,IIF(@yearRepeat = @NgayGio,'1','0'), @ID_Parent, @NgayCu)
									end
							end
						set @yearRepeat = DATEADD(YEAR, @SoLanLap, @yearRepeat)	-- lap lai x nam/lan	
						set @lanlapYear = @lanlapYear +1
					end			
				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
			end
		close _cur;
		deallocate _cur;
	
	-- add LichHen da duoc update (SoLanLap = 0)
	insert into @tblCalendar
	select ID, 
		Ma_TieuDe,
		ID_DonVi, 
		ID_KhachHang, 
		ID_LoaiTuVan,
		ID_NhanVien,	
		ID_NhanVienQuanLy,
		NgayTao,
		NgayGio,
		NgayGioKetThuc,
		NgayHoanThanh,
		TrangThai,
		GhiChu,
		NguoiTao,
		2 as MucDoUuTien, 
		KetQua,
		NhacNho,
		ISNULL(KieuNhacNho,0) as KieuNhacNho,
		ISNULL(KieuLap,0) as KieuLap,
		ISNULL(SoLanLap,0) as SoLanLap,
		ISNULL(GiaTriLap,'') as GiaTriLap,
		ISNULL(TuanLap,0) as TuanLap,
		ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
		ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc,
		'1' as ExistDB, 
		ID_Parent,
		NgayCu
	from #temp where (SoLanLap= 0 OR TrangThai !='1' 
		Or exists (select ID from #temp2 where #temp2.ID = #temp.ID)
		)
		and NgayGio>= @FromDate and NgayGio < @ToDate
	
	drop table #temp 
	drop table #temp2 

	-- select --> union
	select b.*, 
		case when PhanLoai = 3 then 'rgb(11, 128, 67)' else case when MucDoUuTien = 1 then 'rgb(240, 173, 78)' else 'rgb(3, 155, 229)' end end as color, -- xanhla, cam, xanhtroi
		ISNULL(LoaiDoiTuong,0) as LoaiDoiTuong, 
		ISNULL(MaDoiTuong,'') as MaDoiTuong, 
		ISNULL(TenDoiTuong,'') as TenDoiTuong, 
		ISNULL(DienThoai,'') as DienThoai, 
		TenNhanVien, 
		ISNULL(TenLoaiTuVanLichHen,'') as TenLoaiTuVanLichHen, ISNULL(nk.TenNguonKhach,'') as TenNguonKhach
	from
		(select *
		from
			(-- lichhen
			select ID,
				Ma_TieuDe,
				ID_DonVi, 
				ID_KhachHang, 
				ISNULL(ID_LoaiTuVan,'00000000-0000-0000-0000-000000000000') as ID_LoaiTuVan,
				ID_NhanVien,
				ISNULL(ID_NhanVienQuanLy,'00000000-0000-0000-0000-000000000000') as ID_NhanVienQuanLy,
				NgayTao,
				NgayHenGap as NgayGio,
				NgayGioKetThuc,
				NgayHoanThanh,
				TrangThai,
				GhiChu,
				NguoiTao,
				MucDoUuTien,
				KetQua,
				NhacNho,			
				KieuNhacNho,
				KieuLap,
				SoLanLap, 
				GiaTriLap, 
				TuanLap, 
				TrangThaiKetThuc ,
				GiaTriKetThuc,
				3 as PhanLoai,
				'0' as CaNgay,
				ExistDB, 
				ID_Parent,
				NgayCu
			from @tblCalendar
			where exists (select Name from dbo.splitstring(@IDChiNhanhs) where Name = ID_DonVi) 
			and exists (select Name from dbo.splitstring(@TrangThaiCVs) where Name= TrangThai)
			and NgayHenGap >= @FromDate and NgayHenGap < @ToDate

			union all
			-- cong viec
			select 
					cs.ID,
					Ma_TieuDe,
					cs.ID_DonVi, 
					ID_KhachHang, 
					ISNULL(ID_LoaiTuVan,'00000000-0000-0000-0000-000000000000') as ID_LoaiTuVan,
					ID_NhanVien,
					ISNULL(ID_NhanVienQuanLy,'00000000-0000-0000-0000-000000000000') as ID_NhanVienQuanLy,
					cs.NgayTao,
					NgayGio,
					NgayGioKetThuc,
					NgayHoanThanh,
					cs.TrangThai,
					cs.GhiChu,
					cs.NguoiTao,
					MucDoUuTien,
					KetQua,
					cs.NhacNho,
					ISNULL(KieuNhacNho,0) as KieuNhacNho,
					ISNULL(KieuLap,0) as KieuLap,
					ISNULL(SoLanLap,0) as SoLanLap,
					ISNULL(GiaTriLap,'') as GiaTriLap,
					ISNULL(TuanLap,0) as TuanLap,
					ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
					ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc,
					4 as PhanLoai,
					ISNULL(cs.CaNgay,'0') as CaNgay,
					'1' as ExistDB,
					ID_Parent,
					NgayCu
				from ChamSocKhachHangs cs
				where exists (select Name from dbo.splitstring(@IDChiNhanhs) where Name = cs.ID_DonVi)
				and PhanLoai= 4
				and exists (select Name from dbo.splitstring(@TrangThaiCVs) where Name= TrangThai)
				and NgayGio >= @FromDate and NgayGio < @ToDate
				) a
			where exists (select Name from dbo.splitstring(@IDNhanVienPhuTrachs) where Name= ID_NhanVien)
			and exists (select Name from dbo.splitstring(@IDLoaiTuVans) where Name = ID_LoaiTuVan)
		)b
		left join DM_DoiTuong dt on b.ID_KhachHang= dt.ID
		left join NS_NhanVien nv on b.ID_NhanVien= nv.ID
		left join DM_LoaiTuVanLichHen loai on b.ID_LoaiTuVan= loai.ID
		left join DM_NguonKhachHang nk on dt.ID_NguonKhach= nk.ID
		where (dt.LoaiDoiTuong like @LoaiDoiTuong OR LoaiDoiTuong is null)
		and b.PhanLoai like @PhanLoai
		and b.MucDoUuTien like @DoUuTien
		and ISNULL(dt.ID,'00000000-0000-0000-0000-000000000000') like @IDKhachHang
		order by NgayGio");

			CreateStoredProcedure(name: "[dbo].[GetListLichHen_FullCalendar_Dashboard]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				PhanLoai = p.String(20)
			}, body: @"SET NOCOUNT ON;
	DECLARE @DateNow datetime= GETDATE()
	declare @FromDate datetime= convert(varchar(14), @DateNow,23)
	declare @ToDate datetime= convert(varchar(14), dateadd(DAY,1,@DateNow),23)

	--DECLARE @tblNhanVien table (ID uniqueidentifier, MaNhanVien nvarchar(max), TenNhanVien nvarchar(max), DienThoaiDiDong nvarchar(max), GioiTinh bit null, ID_DonVi uniqueidentifier)
	--insert into @tblNhanVien
	--exec GetInForStaff_Working_byChiNhanh @IDChiNhanhs ;

	declare @tblCalendar table(ID uniqueidentifier, ID_DonVi uniqueidentifier, ID_NhanVien uniqueidentifier, NgayHenGap datetime, TrangThai varchar(10))
	

--- table LichHen
select cs.ID, 
	Ma_TieuDe,
	cs.ID_DonVi, 
	ID_KhachHang, 
	ID_LoaiTuVan,
	ID_NhanVien,	
	ID_NhanVienQuanLy,
	NgayTao,
	NgayGio,
	NgayGioKetThuc,	
	NgayHoanThanh,
	ISNULL(KieuLap,0) as KieuLap,
	ISNULL(SoLanLap,0) as SoLanLap,
	ISNULL(GiaTriLap,'') as GiaTriLap,
	ISNULL(TuanLap,0) as TuanLap,
	ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
	ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc, 	
	ISNULL(SoLanDaHen,0) as SoLanDaHen,
	TrangThai,
	ISNULL(GhiChu,'') as GhiChu,
	NguoiTao,
	2 as MucDoUuTien,
	KetQua,
	NhacNho, 
	ISNULL(KieuNhacNho,0) as KieuNhacNho,
	cs.ID as ID_Parent,
	cs.NgayCu
into #temp
from ChamSocKhachHangs cs
left join ( select ISNULL(ID_Parent,'00000000-0000-0000-0000-000000000000') as ID_Parent,
		count(*) as SoLanDaHen
		from ChamSocKhachHangs
		where PhanLoai = 3
		group by ID_Parent) a on cs.ID= a.ID_Parent
where KieuLap in (1,2,3,4)
	and (TrangThaiKetThuc = 1 
	OR (TrangThaiKetThuc = 2 and ISNULL(GiaTriKetThuc,'')  >= CONVERT(varchar, @FromDate,23))
	OR (TrangThaiKetThuc = 3 and ISNULL(SoLanDaHen,0)  <= ISNULL(GiaTriKetThuc,0)) 
	)	
and PhanLoai = 3 

-- get row was update (ID_Parent !=null)
select ID, ID_Parent, NgayCu into #temp2 from ChamSocKhachHangs where ID_Parent is not null and PhanLoai = 3

set nocount on;
declare @ID uniqueidentifier, @Ma_TieuDe nvarchar(max), @ID_DonVi uniqueidentifier, @ID_KhachHang uniqueidentifier,@ID_LoaiTuVan uniqueidentifier, 
		@ID_NhanVien uniqueidentifier,@ID_NhanVienQuanLy uniqueidentifier,
		@NgayTao datetime,@NgayGio datetime,@NgayGioKetThuc datetime, @NgayHoanThanh datetime,
		@KieuLap int, @SoLanLap int, @GiaTriLap varchar(max), @TuanLap int, @TrangThaiKetThuc int,@GiaTriKetThuc varchar(max),			
		@SoLanDaHen int, @TrangThai varchar, @GhiChu nvarchar(max),
		@NguoiTao nvarchar(max), @MucDoUuTien int, @KetQua nvarchar(max), @NhacNho int, @KieuNhacNho int, @ID_Parent uniqueidentifier, @NgayCu datetime

		--- lap ngay
		declare _cur cursor
		for
			select * from #temp where KieuLap = 1 and SoLanLap > 0	
				and not exists (select ID from #temp2 where #temp2.ID = #temp.ID) --and TrangThai='1' 
		open _cur
		fetch next from _cur
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc, @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc, @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin		
				-- chi add row < @ToDate
				declare @dateadd datetime = @NgayGio
				declare @lanlap int = 1			
				while @dateadd < @ToDate 
					begin	
					
						if @TrangThaiKetThuc= 1 
							OR (@TrangThaiKetThuc = 2 and  @dateadd < @GiaTriKetThuc )  --- khong bao gio OR KetThuc vao ngay OR sau x lan (todo)
							OR (@TrangThaiKetThuc= 3 and @lanlap <= @GiaTriKetThuc - @SoLanDaHen)
							begin
								set @NgayGioKetThuc = DATEADD(hour,4,@dateadd)
								declare @newidDay uniqueidentifier = NEWID()
								declare @count1 int = 0;
								if @dateadd = @NgayGio set @newidDay = @ID		
								select @count1 = count(*) from #temp2 where ID_Parent = @ID_Parent 
									and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@dateadd,23)								
								if @count1 = 0											
									insert into @tblCalendar values (@newidDay, @ID_DonVi, @ID_NhanVien,@dateadd, @TrangThai)																			
							end
						set @dateadd = DATEADD(day, @SoLanLap, @dateadd)
						set @lanlap= @lanlap + 1
					end
				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap,@TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,@ID_Parent, @NgayCu
			end
		close _cur;
		deallocate _cur;

		--- lap tuan
		declare _cur2 cursor
		for
			select * from #temp where KieuLap = 2 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID)  --and TrangThai='1' 
		open _cur2
		fetch next from _cur2
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin	
				declare @weekRepeat datetime = @NgayGio				
				declare @lanlapWeek int = 1
				while @weekRepeat < @ToDate -- lặp đến khi thuộc khoảng thời gian tìm kiếm
					begin	

								declare @firstOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 0)) -- lay ngay dau tien cua tuan
								declare @lastOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 7)) -- lay ngay cuoi cung cua tuan
								declare @dateRepeat datetime = @firstOfWeek	
								while @dateRepeat < @lastOfWeek -- tim kiem trong tuan duoc lap lai
									begin
										if dateadd(hour,23, @dateRepeat) >= @NgayGio
											begin
												declare @dateOfWeek varchar(1) = cast(DATEPART(WEEKDAY,@dateRepeat) as varchar(1)) -- lấy ngày trong tuần (thứ 2,3,..)	
												if @dateOfWeek = 1 set @dateOfWeek = 8
												declare @datefrom datetime = @dateRepeat
												set @NgayGioKetThuc = DATEADD(hour,2,@datefrom) -- add 2 hour

												if CHARINDEX(@dateOfWeek, @GiaTriLap ) > 0 
												and  (@TrangThaiKetThuc= 1 OR (@TrangThaiKetThuc = 2 and  @dateRepeat < @GiaTriKetThuc)
													OR (@TrangThaiKetThuc= 3 and @lanlapWeek <= @GiaTriKetThuc - @SoLanDaHen))
													begin														
														declare @newidWeek uniqueidentifier = NEWID()
														declare @exitDB bit='0'
														if convert(varchar(20),@dateRepeat,23) = convert(varchar(20),@NgayGio,23) 
															begin
																set @newidWeek = @ID
																set @exitDB ='1'
															end
														declare @count2 int=0
														select @count2 = count(*) from #temp2 where ID_Parent = @ID_Parent 
																and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@dateRepeat,23)								
														if @count2 = 0	
															begin
																insert into @tblCalendar values (@newidWeek, @ID_DonVi, @ID_NhanVien, @dateRepeat, @TrangThai)	
																set @lanlapWeek= @lanlapWeek + 1																
															end
													end												
											end										
										set @dateRepeat = DATEADD(day, 1, @dateRepeat)											
									end							
						set @weekRepeat = DATEADD(WEEK, @SoLanLap, @weekRepeat)	-- lap lai x tuan/lan	
					end			
				FETCH NEXT FROM _cur2 into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
			end
		close _cur2;
		deallocate _cur2;

		--- lap thang
		declare _cur cursor
		for
			select * from #temp where KieuLap = 3 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID)
		open _cur
		fetch next from _cur
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin		
				declare @monthRepeat datetime = @NgayGio	
				declare @lanlapMonth int = 1
				while @monthRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
					begin	
						if  @monthRepeat >= @FromDate			
							begin	
								declare @datefromMonth datetime= @monthRepeat
								set @NgayGioKetThuc = DATEADD(hour,2,@datefromMonth)
								 -- hàng tháng vào ngày ..xx..
								if	@TuanLap = 0 
									and (@TrangThaiKetThuc = 1 
									OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
									OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
									)
									begin
											declare @newidMonth1 uniqueidentifier = NEWID()
											if @monthRepeat = @NgayGio set @newidMonth1 = @ID
											declare @count3 int=0
											select @count3 = count(*) from #temp2 where ID_Parent = @ID_Parent 
													and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@monthRepeat,23)								
											if @count3 = 0	
												insert into @tblCalendar values (@newidMonth1, @ID_DonVi, @ID_NhanVien, @monthRepeat, @TrangThai)	
									end 
								else
									-- hàng tháng vào thứ ..x.. tuần thứ ..y.. của tháng
									begin
										declare @dateOfWeek_Month int = DATEPART(WEEKDAY,@monthRepeat) -- thu may trong tuan
										if @dateOfWeek_Month = 1 set @dateOfWeek_Month = 8
										declare @weekOfMonth int = DATEDIFF(WEEK, DATEADD(MONTH, DATEDIFF(MONTH, 0, @monthRepeat), 0), @monthRepeat) +1 -- tuan thu may cua thang
										if @dateOfWeek_Month = @GiaTriLap and @weekOfMonth = @TuanLap 
										and (@TrangThaiKetThuc = 1 
											OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
											OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
											)
											begin
												declare @newidMonth2 uniqueidentifier = NEWID()
												if @monthRepeat = @NgayGio set @newidMonth2 = @ID
												declare @count4 int=0
												select @count4 = count(*) from #temp2 where ID_Parent = @ID_Parent 
														and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@monthRepeat,23)								
												if @count4 = 0	
													insert into @tblCalendar values (@newidMonth2, @ID_DonVi, @ID_NhanVien, @monthRepeat, @TrangThai)
											end
									end						
							end
						set @monthRepeat = DATEADD(MONTH, @SoLanLap, @monthRepeat)	-- lap lai x thang/lan	
						set @lanlapMonth = @lanlapMonth +1
					end			
				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
			end
		close _cur;
		deallocate _cur;

		--- lap nam
		declare _cur cursor
		for
			select * from #temp where KieuLap = 4 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID) 
		open _cur
		fetch next from _cur
		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
		while @@FETCH_STATUS = 0
			begin		
				declare @yearRepeat datetime = @NgayGio	
				declare @lanlapYear int = 1
				while @yearRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
					begin						
						if  @yearRepeat >= @FromDate			
							begin	
								declare @dateOfMonth int = datepart(day,@yearRepeat)
								declare @monthOfYear int = datepart(MONTH,@yearRepeat)
								set @NgayGioKetThuc= DATEADD(hour,2, @yearRepeat)

								if @dateOfMonth = @GiaTriLap and @monthOfYear= @TuanLap
									and (@TrangThaiKetThuc = 1 
										OR (@TrangThaiKetThuc = 2 and @yearRepeat < @GiaTriKetThuc)
										OR (@TrangThaiKetThuc = 3 and @lanlapYear <= @GiaTriKetThuc - @SoLanDaHen)
										)
									begin
										declare @newidYear uniqueidentifier = NEWID()										
										if @yearRepeat = @NgayGio set @newidYear = @ID
										declare @count5 int=0
										select @count5 = count(*) from #temp2 where ID_Parent = @ID_Parent 
												and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@yearRepeat,23)								
										if @count5 = 0	
											insert into @tblCalendar values (@newidYear, @ID_DonVi,  @ID_NhanVien,@yearRepeat, @TrangThai)
									end
							end
						set @yearRepeat = DATEADD(YEAR, @SoLanLap, @yearRepeat)	-- lap lai x nam/lan	
						set @lanlapYear = @lanlapYear +1
					end			
				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
			end
		close _cur;
		deallocate _cur;
	
	-- add LichHen da duoc update (SoLanLap = 0)
	insert into @tblCalendar
	select ID, 
		ID_DonVi, 
		ID_NhanVien,	
		NgayGio,
		TrangThai
	from #temp 
	where (SoLanLap= 0 OR TrangThai !='1' 
		Or exists (select ID from #temp2 where #temp2.ID = #temp.ID)
		)
		and NgayGio>= @FromDate and NgayGio < @ToDate
	
	drop table #temp 
	drop table #temp2 

	-- select --> union
	select b.*
	from
		(select *
		from
			(-- lichhen
			select ID,
				ID_DonVi, 
				ID_NhanVien,
				NgayHenGap as NgayGio,
				3 as PhanLoai
			from @tblCalendar
			where exists (select Name from dbo.splitstring(@IDChiNhanhs) where Name = ID_DonVi) 
			and TrangThai='1'
			and NgayHenGap >= @FromDate and NgayHenGap < @ToDate

			union all
			-- cong viec
			select 
					cs.ID,
					cs.ID_DonVi, 
					ID_NhanVien,
					NgayGio,
					4 as PhanLoai
				from ChamSocKhachHangs cs
				where exists (select Name from dbo.splitstring(@IDChiNhanhs) where Name = cs.ID_DonVi)
				and PhanLoai= 4
				and TrangThai='1'
				and NgayGio >= @FromDate and NgayGio < @ToDate
				) a			
		)b
		where b.PhanLoai like @PhanLoai
		order by NgayGio");
        }
        
        public override void Down()
        {
			DropStoredProcedure("[dbo].[CheckThucThu_TongSuDung]");
			DropStoredProcedure("[dbo].[GetInvoiceUseServive_Newest]");
			DropStoredProcedure("[dbo].[GetKhachHanghasDienThoai_byIDNhoms]");
			DropStoredProcedure("[dbo].[GetListCustomer_byIDs]");
			DropStoredProcedure("[dbo].[GetListLichHen_FullCalendar]");
			DropStoredProcedure("[dbo].[GetListLichHen_FullCalendar_Dashboard]");
        }
    }
}
