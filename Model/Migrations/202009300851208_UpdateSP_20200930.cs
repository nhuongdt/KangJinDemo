namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Net.NetworkInformation;

    public partial class UpdateSP_20200930 : DbMigration
    {
        public override void Up()
        {
			Sql(@"CREATE TRIGGER [dbo].[UpdateNgayGiaoDichGanNhat_DMDoiTuong]
   ON [dbo].[BH_HoaDon]
   after insert, update
AS 
BEGIN

	SET NOCOUNT ON;
	declare @ID_KhachHang uniqueidentifier = (select  ID_DoiTuong from inserted)

	declare @NgayMax datetime = (
				select hdmax.NgayLapHoaDon
				from (
					select hd.ID_DoiTuong, hd.NgayLapHoaDon, 
					ROW_NUMBER() over (  PARTITION BY hd.ID_DoiTuong order by hd.NgayLapHoaDon desc) RowNum 
					from BH_HoaDon hd
					where hd.ChoThanhToan is not null and hd.ID_DoiTuong is not null
					and hd.ID_DoiTuong= @ID_KhachHang
					)hdmax where RowNum= 1
				) 
	
	update dt set NgayGiaoDichGanNhat = @NgayMax -- inserted.NgayLapHoaDon
	from DM_DoiTuong dt
	where ID = @ID_KhachHang
		
END
GO");

			Sql(@"CREATE FUNCTION [dbo].[CheckExist_ThietLapLuong]
(
	@ID_ChiNhanh uniqueidentifier,
	@ID_NhanVien uniqueidentifier,
	@ID_PhuCap uniqueidentifier,
	@ID_LoaiLuong varchar(40),
	@LoaiLuong int,
	@FromDate datetime,
	@ToDate datetime 
)
RETURNS bit
AS
BEGIN
	declare @count int =0, @exist int ='0'
	if @ToDate = '' set @ToDate = null
	if @ID_LoaiLuong = '' set @ID_LoaiLuong = null
	if @LoaiLuong <5
		begin
			if @ToDate is null 
				select @count = count(ID)
				from NS_Luong_PhuCap where ID!=@ID_PhuCap
				and LoaiLuong <5 
				and TrangThai != 0
				and ID_NhanVien= @ID_NhanVien
				and ID_DonVi= @ID_ChiNhanh
				and (NgayKetThuc is null or (NgayKetThuc is not null and @FromDate <= NgayKetThuc))
				
			else
			select @count = count(ID)
				from NS_Luong_PhuCap where ID!=@ID_PhuCap
				and LoaiLuong <5 
				and TrangThai != 0
				and ID_NhanVien= @ID_NhanVien
				and ID_DonVi= @ID_ChiNhanh
				and ((NgayKetThuc is null and @ToDate >= NgayApDung)
				or (NgayKetThuc is not null 
					and ((@FromDate >= NgayApDung and @FromDate <= NgayKetThuc)
						or (@ToDate <= NgayKetThuc and @ToDate >= NgayApDung)))
				)			

		end
	else
		begin
			if @ToDate is null 
				select @count = count(ID)
					from NS_Luong_PhuCap where ID!=@ID_PhuCap
					and TrangThai != 0
					and ID_NhanVien= @ID_NhanVien
					and ID_DonVi= @ID_ChiNhanh
					and ID_LoaiLuong = @ID_LoaiLuong
					and (NgayKetThuc is null or (NgayKetThuc is not null and @FromDate <= NgayKetThuc))
			else
				select @count = count(ID)
				from NS_Luong_PhuCap where ID!=@ID_PhuCap
				and ID_LoaiLuong = @ID_LoaiLuong
				and TrangThai != 0
				and ID_NhanVien= @ID_NhanVien
				and ID_DonVi= @ID_ChiNhanh
				and ((NgayKetThuc is null and @ToDate >= NgayApDung)
				or (NgayKetThuc is not null 
					and ((@FromDate >= NgayApDung and @FromDate <= NgayKetThuc)
						or (@ToDate <= NgayKetThuc and @ToDate >= NgayApDung)))
				)	
		end

		if @count > 0
			set @exist = '1'
	return @exist

END
");

			CreateStoredProcedure(name: "[dbo].[BaoCaoKhachHang_TanSuat]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				IDNhomKhachs = p.String(),
				LoaiChungTus = p.String(20),
				TrangThaiKhach = p.String(10),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				NgayGiaoDichFrom = p.DateTime(),
				NgayGiaoDichTo = p.DateTime(),
				DoanhThuTu = p.Double(),
				DoanhThuDen = p.Double(),
				SoLanDenFrom = p.Int(),
				SoLanDenTo = p.Int(),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int(),
				ColumnSort = p.String(200),
				TypeSort = p.String(20)
			}, body: @"SET NOCOUNT ON;
	if @ColumnSort ='' or @ColumnSort is null set @ColumnSort='MaKhachHang'
	if @TypeSort ='' or @TypeSort is null set @TypeSort='desc'	
	SET @TypeSort = UPPER(@TypeSort)

	 DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tblChiNhanh table (ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select * from dbo.splitstring(@IDChiNhanhs)

	declare @tblNhomDT table (ID varchar(40))
	insert into @tblNhomDT
	select * from dbo.splitstring(@IDNhomKhachs);

	if @DoanhThuDen is null
		set @DoanhThuDen = 9999999999
	if @SoLanDenTo is null
		set @SoLanDenTo = 9999999;

	with data_cte
	as(

	select dt.ID as ID_KhachHang, dt.MaDoiTuong as MaKhachHang, dt.TenDoiTuong as TenKhachHang, dt.DienThoai, dt.DiaChi, dt.TenNhomDoiTuongs,
		dt.NgayGiaoDichGanNhat,
		hd1.SoLanDen, cast(hd1.DoanhThu as float) as DoanhThu
	from DM_DoiTuong dt  
	join (
		select 
			 thu.ID_DoiTuong,
			 sum(thu.SoLanDen) as SoLanDen,
			 sum(thu.DoanhThu) -  sum(thu.SuDungThe) as DoanhThu
		from 
		(
			select hd.ID_DoiTuong, count(hd.ID) as SoLanDen, sum(hd.PhaiThanhToan) as DoanhThu, 0 as SuDungThe
			from(
				-- neu thegiatri: doanhthu = mucnap - tonggiamgia
				select  hd.ID_DoiTuong, hd.ID, iif(hd.LoaiHoaDon =22, hd.TongChiPhi - hd.TongGiamGia, hd.PhaiThanhToan) as PhaiThanhToan
				from BH_HoaDon hd
				where hd.ChoThanhToan = 0
				and hd.ID_DoiTuong is not null
				and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon <= @ToDate
				and exists (select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
				and exists (select * from dbo.splitstring(@LoaiChungTus) ct where hd.LoaiHoaDon= ct.Name)
			) hd group by hd.ID_DoiTuong

			union all

			select qct.ID_DoiTuong, 0 as SoLanDen, 0 as DoanhThu, sum(TienThu) as SuDungThe
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
			where (qhd.TrangThai is null or qhd.TrangThai= 1)
			and qct.ID_DoiTuong is not null
			and qct.ThuTuThe > 0
			and qhd.LoaiHoaDon = 11
			and qhd.NgayLapHoaDon >= @FromDate and qhd.NgayLapHoaDon <= @ToDate
			and exists (select ID_DonVi from @tblChiNhanh dv where qhd.ID_DonVi= dv.ID_DonVi)
			group by qct.ID_DoiTuong
		) thu group by thu.ID_DoiTuong
	) hd1 on dt.ID = hd1.ID_DoiTuong
    where 
		 exists (select nhom1.Name from dbo.splitstring(isnull(dt.IDNhomDoiTuongs,'00000010-0000-0000-0000-000000000010')) nhom1 join @tblNhomDT nhom2 on nhom1.Name = nhom2.ID)
	and 
		dt.TheoDoi like @TrangThaiKhach
	and dt.NgayGiaoDichGanNhat >= @NgayGiaoDichFrom and dt.NgayGiaoDichGanNhat < @NgayGiaoDichTo
	and hd1.SoLanDen >= @SoLanDenFrom and hd1.SoLanDen <= @SoLanDenTo
	and hd1.DoanhThu >= @DoanhThuTu and hd1.DoanhThu <= @DoanhThuDen
	and ((select count(Name) from @tblSearchString b where 
    				dt.MaDoiTuong like '%'+b.Name+'%' 
    				or dt.TenDoiTuong like '%'+b.Name+'%' 
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%' +b.Name +'%' 
    				or dt.DienThoai like '%'+b.Name+'%'
					or dt.DiaChi like '%'+b.Name+'%'
					)=@count or @count=0)
	),
	count_cte
	as(
	select count(ID_KhachHang) as TotalRow,
		CEILING(COUNT(ID_KhachHang) / CAST(@PageSize as float )) as TotalPage,
		sum(dt.SoLanDen) as TongSoLanDen,
		sum(DoanhThu) as TongDoanhThu
	from data_cte dt
	)
	select *
	from data_cte dt
	cross join count_cte cte
	order by	
		-- các cột dữ liệu sắp xếp phải chuyển về cùng 1 kiểu data
		CASE WHEN @TypeSort = 'ASC'  THEN 
			case @ColumnSort
				when 'SoLanDen' then cast(dt.SoLanDen as float)
				when 'DoanhThu' then cast(dt.DoanhThu as float)
				when 'NgayGiaoDichGanNhat' then cast(dt.NgayGiaoDichGanNhat as float)
				end
			 END ASC,
		CASE WHEN @TypeSort = 'DESC'  THEN 
			case @ColumnSort
				when 'SoLanDen' then cast(dt.SoLanDen as float)
				when 'DoanhThu' then cast(dt.DoanhThu as float)
				when 'NgayGiaoDichGanNhat' then cast(dt.NgayGiaoDichGanNhat as float)
			end
		END DESC
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetListPhieuPhanCa_Paging]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				LoaiCa = p.String(20),
				NgayTaoFrom = p.DateTime(),
				NgayTaoTo = p.DateTime(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				TrangThai = p.String(10),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	set @NgayTaoTo = DATEADD(DAY,1,@NgayTaoTo)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	if @LoaiCa =''
		set @LoaiCa ='1,3'
	if @TrangThai =''
		set @TrangThai ='1,2'

	declare @tblDonVi table(ID_DonVi uniqueidentifier)
	insert into @tblDonVi
	select Name from dbo.splitstring(@IDChiNhanhs);

	with data_cte
	as
	(

	select pc.*, nvPhieu.nvien as TenNhanViens,
		case pc.LoaiPhanCa
			when 3 then N'Ca cố định'
			when 1 then N'Ca tuần'
			else ''
			end as LoaiPhieuPhanCa,
		case pc.TrangThai
			when 0 then N'Hủy'
			when 1 then N'Tạo mới'
			when 2 then N'Áp dụng'
			else ''
			end as TrangThaiText
	from NS_PhieuPhanCa pc
	join (
		select distinct ID_PhieuPhanCa,
		(
			select CONCAT(nv.MaNhanVien, ' ', nv.TenNhanVien,' ', nv.TenNhanVienChuCaiDau,' ', nv.TenNhanVienKhongDau, ', ') as [text()] 
			from NS_PhieuPhanCa_NhanVien phieunv join NS_NhanVien nv on phieunv.ID_NhanVien = nv.ID
			where phieunv.ID_PhieuPhanCa = phieuOut.ID_PhieuPhanCa
			for xml path('')
		) nvien
		from NS_PhieuPhanCa_NhanVien phieuOut
		) nvPhieu on pc.ID= nvPhieu.ID_PhieuPhanCa
	where  pc.NgayTao >= @NgayTaoFrom and pc.NgayTao < @NgayTaoTo
	and exists (select ID_DonVi from @tblDonVi dv where dv.ID_DonVi = pc.ID_DonVi)
	and exists (select Name from dbo.splitstring(@LoaiCa) ca where pc.LoaiPhanCa = ca.Name)
	and exists (select Name from dbo.splitstring(@TrangThai) tt where pc.TrangThai = tt.Name)
	and pc.NgayTao >= @NgayTaoFrom and pc.NgayTao < @NgayTaoTo
	and ( 
	(pc.DenNgay is null and pc.TuNgay <= @ToDate) 
	or (@FromDate <= pc.TuNgay and @ToDate >= pc.TuNgay and @ToDate <= pc.DenNgay)
	or (@FromDate <= pc.DenNgay and @ToDate >= pc.DenNgay and @FromDate >= pc.TuNgay)
	or (@FromDate >= pc.TuNgay and @ToDate >= pc.TuNgay and @ToDate <= pc.DenNgay)	
	or (@FromDate <= pc.TuNgay and @ToDate >= pc.TuNgay and @ToDate >= pc.DenNgay)	
	)
	and
		((select count(Name) from @tblSearchString b where     			
		pc.MaPhieu like '%'+b.Name+'%'
		or nvPhieu.nvien like '%'+b.Name+'%'						
		)=@count or @count=0)	
   ),
   count_cte
   as (
	select count(ID) as TotalRow,
		CEILING(count(ID) / cast (@PageSize as float)) as TotalPage
	from data_cte 
   )

   select *
   from data_cte dt
   cross join count_cte cte
   	order by dt.NgayTao
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetNhatKyGiaoDich_ofKhachHang]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				ID_KhachHang = p.String(),
				LoaiChungTu = p.String(10),
				TextSearch = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblChiNhanh table (ID uniqueidentifier)
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@IDChiNhanhs);

	with data_cte
	as(
	select hd.ID, hd.LoaiHoaDon, hd.MaHoaDon, hd.NgayLapHoaDon, hd.NgayApDungGoiDV, hd.HanSuDungGoiDV, 
		iif(hd.LoaiHoaDon=22, hd.TongChiPhi, hd.TongTienHang) as TongTienHang,
		hd.TongTienThue, hd.TongGiamGia, hd.TongChiPhi, hd.TongChietKhau, hd.PhaiThanhToan, 
		dv.TenDonVi,
		TienMat, TienGui, ThuTuThe, TienThu as DaThanhToan,
		case  hd.LoaiHoaDon
			when 1 then N'Hóa đơn bán lẻ'
			when 19 then N'Gói dịch vụ'
			when 22 then N'Thẻ giá trị'
		end as SLoaiHoaDon
	from BH_HoaDon hd
	join DM_DonVi dv on hd.ID_DonVi= dv.ID
	left join (
		select qct.ID_HoaDonLienQuan, 
			sum(qct.TienMat) as TienMat, 
			sum(qct.TienGui) as TienGui, 
			sum(qct.ThuTuThe) as ThuTuThe,
			sum(qct.TienThu) as TienThu
		from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
		where qhd.TrangThai = 1
		group by qct.ID_HoaDonLienQuan
		) quy on quy.ID_HoaDonLienQuan = hd.ID
	where hd.ID_DoiTuong= @ID_KhachHang
	and exists (select Name from dbo.splitstring(@LoaiChungTu) ct where hd.LoaiHoaDon = ct.Name )
	and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
	and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
   ),
   count_cte
   as(
	select COUNT(ID) as TotalRow,
		 CEILING(COUNT(ID)/ CAST(@PageSize as float)) as TotalPage,
		 sum(TongTienHang) as SumTienHang,
		 sum(TongTienThue) as SumTienThue,
		 sum(TongGiamGia) as SumGiamGia,
		 sum(PhaiThanhToan) as SumPhaiThanhToan,
		 sum(TongChiPhi) as SumChiPhi,
		 sum(TongChietKhau) as SumChietKhau,
		 sum(TienMat) as SumTienMat,
		 sum(TienGui) as SumTienGui,
		 sum(ThuTuThe) as SumThuTuThe,
		 sum(DaThanhToan) as SumDaThanhToan
	from data_cte
   )
   select *
   from data_cte dt
   cross join count_cte cte
   order by dt.NgayLapHoaDon desc
   offset (@CurrentPage * @PageSize) rows
   fetch next @PageSize rows only");

			CreateStoredProcedure(name: "[dbo].[GetThietLapLuong_ofNhanVien]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				ID_NhanVien = p.Guid(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	with data_cte
	as(
	select pc.*,
		pc.LoaiLuong as LoaiPhuCap_GiamTru,
		case pc.LoaiLuong
			when 1 then N'Lương cố định'
			when 2 then N'Lương theo ngày'
			when 3 then N'Lương theo ca'
			when 4 then N'Lương theo giờ'
			else isnull(loai.TenLoaiLuong,'')
			end as TenLoaiLuong		
	from NS_Luong_PhuCap pc 
	left join NS_LoaiLuong loai on pc.ID_LoaiLuong= loai.ID
	where pc.ID_DonVi= @ID_ChiNhanh and pc.ID_NhanVien= @ID_NhanVien
	and pc.TrangThai !=0
   ),
   count_cte
   as(
	select count (ID) as TotlaRow,
		 count (ID)/ cast (@PageSize as float) as TotlaPage
	from data_cte
   )
   select * from data_cte dt
   cross join count_cte cte
   order by dt.NgayApDung desc
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[HuyPhieuThu_UpdateCongNoTamUngLuong]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				IDQuyChiTiets = p.String(),
				LaPhieuTamUng = p.Boolean()
			}, body: @"SET NOCOUNT ON;

		declare @tblQuyChiTiet table(ID_QuyChiTiet uniqueidentifier)
    	insert into @tblQuyChiTiet
    	select Name from dbo.splitstring(@IDQuyChiTiets)

		if @LaPhieuTamUng ='1'
			begin
    			declare @sotienTamUng float, @nvTamUng uniqueidentifier, @idKhoanThuChi uniqueidentifier
    			-- get sotien, nhanvien tamung
    			select @sotienTamUng = TienThu, @nvTamUng= ID_NhanVien, @idKhoanThuChi= ID_KhoanThuChi
    			from Quy_HoaDon_ChiTiet qct1 where exists (select top 1 ID from @tblQuyChiTiet qct2 where qct1.ID= qct2.ID_QuyChiTiet)
    
    			declare @giamtruLuong bit= (
    				select TinhLuong
    				from Quy_KhoanThuChi khoan
    				where id= @idKhoanThuChi)
    
    			if @giamtruLuong ='1'   			
    					update NS_CongNoTamUngLuong set CongNo = CongNo - @sotienTamUng    		
    			
    		end
		else
				update tblNoCu set CongNo= tblNoHienTai.NoHienTai
    			from NS_CongNoTamUngLuong tblNoCu
    			join (
    				select congno.ID,congno.CongNo + quy.TruTamUngLuong as NoHienTai
    				from NS_CongNoTamUngLuong congno
    				join (
    					select qct.ID_NhanVien, qct.TruTamUngLuong
    					from Quy_HoaDon_ChiTiet qct 
    					join @tblQuyChiTiet qct2 on qct.ID= qct2.ID_QuyChiTiet			
    					) quy on congno.ID_NhanVien= quy.ID_NhanVien
    				where congno.ID_DonVi= @ID_ChiNhanh
    			) tblNoHienTai on tblNoCu.ID= tblNoHienTai.ID");

			CreateStoredProcedure(name: "[dbo].[RemoveCong_ofNhanVien]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				ID_NhanVien = p.Guid(),
				ID_CaLamViec = p.Guid(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;	
    	
    	update bs set bs.TrangThai = 0  		
    	from NS_CongBoSung bs
    	left join NS_BangLuong_ChiTiet blct on bs.ID_NhanVien= blct.ID_NhanVien
		left join NS_BangLuong bl on blct.ID_BangLuong= bl.ID
    	where bs.ID_NhanVien= @ID_NhanVien
		and bs.ID_DonVi= @ID_ChiNhanh
		and bs.ID_CaLamViec= @ID_CaLamViec
    	and bs.NgayCham>= @FromDate and bs.NgayCham <= @ToDate
		and isnull(bl.TrangThai,1) < 3	");

			CreateStoredProcedure(name: "[dbo].[UpdatePhieuPhanCa_CheckExistCong]", parametersAction: p => new
			{
				ID_PhieuPhanCa = p.Guid()
			}, body: @"SET NOCOUNT ON;

	declare @tblNhanVienOld table (ID_NhanVien uniqueidentifier)
	insert into @tblNhanVienOld
	select ID_NhanVien
	from NS_PhieuPhanCa_NhanVien where ID_PhieuPhanCa = @ID_PhieuPhanCa

    declare @ID_DonVi uniqueidentifier, @FromDateOld datetime, @ToDateOld datetime
	select @ID_DonVi= ID_DonVi , @FromDateOld= TuNgay, @ToDateOld= DenNgay
	from NS_PhieuPhanCa where ID= @ID_PhieuPhanCa

	-- get cong nhanvien old
	select ID, ID_BangLuongChiTiet, ID_NhanVien, ID_CaLamViec
	into #temp
	from NS_CongBoSung bs
	where ID_DonVi = @ID_DonVi
	and TrangThai!=0
	and NgayCham >= @FromDateOld and (NgayCham <= @ToDateOld or @ToDateOld is null)
	and exists (select ID_NhanVien from @tblNhanVienOld nv where bs.ID_NhanVien= nv.ID_NhanVien)	

	declare @countBL int = (select count(ID) from #temp where ID_BangLuongChiTiet is not null)
	if @countBL > 0
		-- nv da taobangluong
		select (
			select CONCAT( tblNV.TenNhanVien, ' (', tblNV.MaNhanVien , '), ') as [text()]			 
			from
				(select distinct nv.MaNhanVien, nv.TenNhanVien
				from #temp tmp				
				join NS_NhanVien nv on tmp.ID_NhanVien= nv.ID
				where tmp.ID_BangLuongChiTiet is not null
			) tblNV
			for xml path ('')
		) TenNhanVien, '1' as MaNhanVien -- muontamtruong MaNhanVien
	else
		-- nv da chamcong
		select ISNull((
			select CONCAT( nv.TenNhanVien, ' (', nv.MaNhanVien , '), ') as [text()]
			from
				(select distinct nv.MaNhanVien, nv.TenNhanVien
				from #temp tmp
				join NS_NhanVien nv on tmp.ID_NhanVien= nv.ID
				) nv
			for xml path ('')
		),'') as TenNhanVien, '0' as MaNhanVien");

			CreateStoredProcedure(name: "[dbo].[UpdatePhieuPhanCa_RemoveCongNhanVien]", parametersAction: p => new
			{
				ID_PhieuPhanCa = p.Guid(),
				IDNhanVienNews = p.String(),
				IDCaNews = p.String(),
				FromDateNew = p.String(14),
				ToDateNew = p.String(14)
			}, body: @"SET NOCOUNT ON;
	declare @tblCaNew table (ID_CalamViec uniqueidentifier)
	insert into @tblCaNew
	select Name from dbo.splitstring(@IDCaNews)

	declare @tblNhanVienNew table (ID_NhanVien uniqueidentifier)
	insert into @tblNhanVienNew
	select Name
	from dbo.splitstring(@IDNhanVienNews)

	update bs set TrangThai= 0
	--select bs.NgayCham, bs2.*
	from NS_CongBoSung bs
	join  (
		select congDelete.*
		from(
			select phieuold.*, phieunew.ID_CalamViec as IDCaNew, phieuold.ID_NhanVien as IDNhanVienNew
			from
			(
				select bs.ID, bs.ID_NhanVien, bs.ID_CaLamViec, bs.NgayCham
				from NS_CongBoSung bs
    			join NS_PhieuPhanCa_NhanVien phieunv on bs.ID_NhanVien = phieunv.ID_NhanVien
    			join NS_PhieuPhanCa_CaLamViec phieuca on phieunv.ID_PhieuPhanCa = phieuca.ID_PhieuPhanCa and  bs.ID_CaLamViec = phieuca.ID_CaLamViec
    			join NS_PhieuPhanCa phieu on phieunv.ID_PhieuPhanCa = phieu.ID
    			where phieu.ID = @ID_PhieuPhanCa
				--and (bs.NgayCham < @FromDateNew or bs.NgayCham > @ToDateNew)
			) phieuold
			left join 
			(
				select *
				from @tblCaNew		
				cross join @tblNhanVienNew
			) phieunew on phieuold.ID_CaLamViec = phieunew.ID_CalamViec and phieuold.ID_NhanVien= phieunew.ID_NhanVien
		) congDelete 
		-- nhanvien not exist Or NgayCham khong nam trong khoang ngay
		where congDelete.IDCaNew is null or (congDelete.IDCaNew is not null  and (congDelete.NgayCham < @FromDateNew or congDelete.NgayCham > @ToDateNew))
	) bs2 on bs.ID= bs2.ID");


			Sql(@"ALTER FUNCTION [dbo].[check_MaNhanVien](@inputVar NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN    
	DECLARE @MaNhanVien NVARCHAR(MAX);
	DECLARE @tab table (MaNhanVien int, SoThuTu int)
	insert into @tab
	Select RIGHT(MaNhanVien, 5), ROW_NUMBER() over(order by MaNhanVien) as MaNhanVien from NS_NhanVien
	WHERE LEN(MaNhanVien)>6 and (TrangThai = 1 OR TrangThai is null) and LEFT(MaNhanVien, 2) = LEFT(@inputVar, 2) AND ISNUMERIC(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 1 AND LEN(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 5
	order by MaNhanVien
	SELECT @MaNhanVien = (select TOP 1 MaNhanVien + 1 from @tab
	order by SoThuTu desc)
	IF (LEN(@MaNhanVien) > 0)
	BEGIN
			SELECT @MaNhanVien = CASE
			WHEN @MaNhanVien > 0 and @MaNhanVien <= 9 THEN 'NV0000' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 9 and @MaNhanVien <= 99 THEN 'NV000' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 99 and @MaNhanVien <= 999 THEN 'NV00' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 999 and @MaNhanVien <= 9999 THEN 'NV0' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 9999 THEN 'NV' + CONVERT(CHAR, @MaNhanVien)
			END
	END
	else
	BEgin
	 SELECT @MaNhanVien = (Select Max(SoThuTu) +1 from @tab)
		IF (LEN(@MaNhanVien) > 0)
		BEGIN
				SELECT @MaNhanVien = CASE
				WHEN @MaNhanVien > 0 and @MaNhanVien <= 9 THEN 'NV0000' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 9 and @MaNhanVien <= 99 THEN 'NV000' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 99 and @MaNhanVien <= 999 THEN 'NV00' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 999 and @MaNhanVien <= 9999 THEN 'NV0' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 9999 THEN 'NV' + CONVERT(CHAR, @MaNhanVien)
				END
		END
		ELSE
		BEGIN
			SELECT @MaNhanVien = @inputVar;
		END
	END
	RETURN @MaNhanVien
END");

			Sql(@"ALTER PROCEDURE [dbo].[DanhMucKhachHang_CongNo_ChotSo]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @MaKH [nvarchar](max),
    @LoaiKH [int],
    @ID_NhomKhachHang [nvarchar](max),
    @timeStartKH [datetime],
    @timeEndKH [datetime]
AS
BEGIN
    set nocount on

	declare @tblIDNhoms table (ID varchar(36))
	if @ID_NhomKhachHang ='%%'
		begin
			-- check QuanLyKHTheochiNhanh
			declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where ID_DonVi like @ID_ChiNhanh)
			insert into @tblIDNhoms(ID) values ('00000000-0000-0000-0000-000000000000')

			if @QLTheoCN = 1
				begin									
					insert into @tblIDNhoms(ID)
					select *  from (
						-- get Nhom not not exist in NhomDoiTuong_DonVi
						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
						and LoaiDoiTuong = @LoaiKH --and (TrangThai is null or TrangThai = 1)
						union all
						-- get Nhom at this ChiNhanh
						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where ID_DonVi like @ID_ChiNhanh) tbl
				end
			else
				begin				
				-- insert all
				insert into @tblIDNhoms(ID)
				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
				where LoaiDoiTuong = @LoaiKH --and (TrangThai is null or TrangThai = 1)
				end

			--select * from @tblIDNhoms
		end
	else
		begin
			set @ID_NhomKhachHang = REPLACE(@ID_NhomKhachHang,'%','')
			insert into @tblIDNhoms(ID) values (@ID_NhomKhachHang)
		end

	DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    		
    		SELECT * 
    		FROM
    		(
    		 SELECT 
    		 dt.ID as ID,
    		 dt.MaDoiTuong, 
			 dt.ID_TrangThai,
			 case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as ID_NhomDoiTuong,
			 --ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') as ID_NhomDoiTuong,
    		  dt.TenDoiTuong,
    		  dt.TenDoiTuong_KhongDau,
    		  dt.TenDoiTuong_ChuCaiDau,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
			   dt.NgayGiaoDichGanNhat,
    		  dt.DienThoai,
    		  dt.Email,
    		  dt.DiaChi,
    		  dt.MaSoThue,
    		  ISNULL(dt.GhiChu,'') as GhiChu,
    		  dt.NgayTao,
    		  dt.NguoiTao,
    		  dt.DinhDang_NgaySinh,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    		  dt.LaCaNhan,
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
			  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
			  ISNULL(trangthai.TenTrangThai,'') as TrangThaiKhachHang,
			  case when right(rtrim(dt.TenNhomDoiTuongs),1) =',' then LEFT(Rtrim(dt.TenNhomDoiTuongs), len(dt.TenNhomDoiTuongs)-1) else ISNULL(dt.TenNhomDoiTuongs,N'Nhóm mặc định') end as TenNhomDT,-- remove last coma
			  --ISNULL(dt.TenNhomDoiTuongs,N'Nhóm mặc định') as TenNhomDT,
    		  ISNULL(dt.TongTichDiem,0) as TongTichDiem,
			  ISNULL(qh.TenQuanHuyen,'') as PhuongXa,
			  ISNULL(tt.TenTinhThanh,'') as KhuVuc,
			  ISNULL(dv.TenDonVi,'') as ConTy,
			  ISNULL(dv.SoDienThoai,'') as DienThoaiChiNhanh,
			  ISNULL(dv.DiaChi,'') as DiaChiChiNhanh,
			  ISNULL(nk.TenNguonKhach,'') as TenNguonKhach,
			  ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
			  CAST(0 as float) as TongNapThe , 
			  CAST(0 as float) as SuDungThe , 
			  CAST(0 as float) as HoanTraTheGiaTri , 
			  CAST(0 as float) as SoDuTheGiaTri , 
			  concat(dt.MaDoiTuong,' ',lower(dt.MaDoiTuong) ,' ',dt.TenDoiTuong,' ',ISNULL(dt.DienThoai,''),' ', ISNULL(dt.TenDoiTuong_KhongDau,''))  as Name_Phone
    	  FROM
    			DM_DoiTuong dt
    			LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    			LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
				LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
				LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
    			LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    			LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID
    			LEFT Join
    		(
    			  SELECT HangHoa.ID_DoiTuong,
    				SUM(ISNULL(HangHoa.NoHienTai, 0)) as NoHienTai, 
    				SUM(ISNULL(HangHoa.TongBan, 0)) as TongBan,
    				SUM(ISNULL(HangHoa.TongBanTruTraHang, 0)) as TongBanTruTraHang,
    				SUM(ISNULL(HangHoa.TongMua, 0)) as TongMua,
    				SUM(ISNULL(HangHoa.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
    					SELECT
    						td.ID_DoiTuong,
    						SUM(ISNULL(td.CongNo, 0)) + SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
    						0 AS TongBan,
    						0 AS TongBanTruTraHang,
    						0 AS TongMua,
    						0 AS SoLanMuaHang
    					FROM
    					(
    					-- Chốt sổ
    						SELECT 
    							ID_KhachHang As ID_DoiTuong,
    							ISNULL(CongNo, 0) AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM ChotSo_KhachHang
    						where ID_DonVi = @ID_ChiNhanh
    						UNION ALL
    						-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    						SELECT 
    							bhd.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong,
    							0 AS CongNo,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- sổ quỹ thu
    						UNION ALL
    						SELECT 
    							qhdct.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    							0 AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
							Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo
							--AND (qhd.HachToanKinhDoanh = 0 or qhd.HachToanKinhDoanh = '1')
    						AND qhd.ID_DonVi = @ID_ChiNhanh
							AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    						GROUP BY qhdct.ID_DoiTuong
							-- So Quy chi
    						UNION ALL
    						SELECT 
    							qhdct.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo
							--AND (qhd.HachToanKinhDoanh  = 0 or qhd.HachToanKinhDoanh = '1')
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong							

    					) AS td
    						GROUP BY td.ID_DoiTuong
    						UNION ALL
    							-- Tổng bán phát sinh trong khoảng thời gian truy vấn
    						SELECT
    							pstv.ID_DoiTuong ,
    							0 AS NoHienTai,
    							SUM(ISNULL(pstv.DoanhThu,0)) AS TongBan,
    							SUM(ISNULL(pstv.DoanhThu,0)) -  SUM(ISNULL(pstv.GiaTriTra,0)) AS TongBanTruTraHang,
    							SUM(ISNULL(pstv.GiaTriTra,0)) AS TongMua,
    							0 AS SoLanMuaHang
    						FROM
    						(
    						SELECT 
    							bhd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						) AS pstv
    						GROUP BY pstv.ID_DoiTuong
    							Union All
    							Select 
    								hd.ID_DoiTuong,
    								0 AS NoHienTai,
    								0 AS TongBan,
    								0 AS TongBanTruTraHang,
    								0 AS TongMua,
    								COUNT(*) AS SoLanMuaHang
    							From BH_HoaDon hd 
    							where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    							and hd.ChoThanhToan = 0
    							AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd 
    							And hd.ID_DonVi = @ID_ChiNhanh 
    							GROUP BY hd.ID_DoiTuong
    					)AS HangHoa
    						GROUP BY HangHoa.ID_DoiTuong
    				) a
    					on dt.ID = a.ID_DoiTuong
    				where (dt.MaDoiTuong LIKE  @MaKH  
					     OR dt.TenDoiTuong_ChuCaiDau LIKE  @MaKH  
						 OR dt.TenDoiTuong_KhongDau LIKE  @MaKH 
						 OR dt.TenDoiTuong LIKE  @MaKH 
    					 OR dt.DienThoai LIKE  @MaKH)

    				and dt.loaidoituong = @loaiKH
    					and dt.NgayTao >= @timeStartKH and dt.NgayTao < @timeEndKH
    						AND dt.TheoDoi =0
    				)b
					--INNER JOIN @tblIDNhoms tblsearch ON CHARINDEX(CONCAT(', ', tblsearch.ID, ', '), CONCAT(', ', b.ID_NhomDoiTuong, ', '))>0
    				where b.ID not like '%00000000-0000-0000-0000-0000%'
					and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl INNER JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    	ORDER BY b.NgayTao desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[import_DanhMucHangHoa_Update]
	@ID_HoaDon [uniqueidentifier],
	@SoThuTu [int],
    @LoaiUpdate [int],
    @ID_HangHoa [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @GiaTriTang [nvarchar](max),
    @GiaTriGiam [nvarchar](max),
    @TongTienLech [nvarchar](max),
    @TongChenhLech [nvarchar](max),
    @SoLuongThucTe [nvarchar](max),
    @SoLuongTang [nvarchar](max),
    @SoLuongGiam [nvarchar](max),
    @TenNhomHangHoaCha [nvarchar](max),
    @TenNhomHangHoaCha_KhongDau [nvarchar](max),
    @TenNhomHangHoaCha_KyTuDau [nvarchar](max),
    @MaNhomHangHoaCha [nvarchar](max),
    @timeCreateNHHCha [datetime],
    @TenNhomHangHoa [nvarchar](max),
    @TenNhomHangHoa_KhongDau [nvarchar](max),
    @TenNhomHangHoa_KyTuDau [nvarchar](max),
    @MaNhomHangHoa [nvarchar](max),
    @timeCreateNHH [datetime],
    @LaHangHoa [bit],
    @timeCreateHH [datetime],
    @TenHangHoa [nvarchar](max),
    @TenHangHoa_KhongDau [nvarchar](max),
    @TenHangHoa_KyTuDau [nvarchar](max),
    @GhiChu [nvarchar](max),
    @QuyCach [nvarchar](max),
    @DuocBanTrucTiep [bit],
    @MaDonViCoBan [nvarchar](max),
    @MaHangHoa [nvarchar](max),
    @TenDonViTinh [nvarchar](max),
    @GiaVon [nvarchar](max),
    @GiaBan [nvarchar](max),
    @timeCreateDVQD [datetime],
    @LaDonViChuan [bit],
    @TyLeChuyenDoi [nvarchar](max),
    @MaHoaDon [nvarchar](max),
    @DienGiai [nvarchar](max),
    @TonKho [nvarchar](max),
    @timeCreateHD [datetime],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHangHoaChaCungLoai [nvarchar](max),
	@DVTQuyCach nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
    -- Khai báo biến update
    	DECLARE @GiaVonF as float
    		set @GiaVonF = (select CAST(ROUND(@GiaVon, 2) as float))
    	DECLARE @GiaBanF as float
    		set @GiaBanF = (select CAST(ROUND(@GiaBan, 2) as float))
    	DECLARE @QuyCachF as float
    		set @QuyCachF = (select CAST(ROUND(@QuyCach, 2) as float))
    	DECLARE @TyLeChuyenDoiF as float
    		set @TyLeChuyenDoiF = (select CAST(ROUND(@TyLeChuyenDoi, 2) as float))
    	DECLARE @TonKhoF as float
    		set @TonKhoF = (select CAST(ROUND(@TonKho, 2) as float))
    
    		DECLARE @GiaTriTangF as float
    		set @GiaTriTangF = (select CAST(ROUND(@GiaTriTang, 0) as float))
    		DECLARE @GiaTriGiamF as float
    		set @GiaTriGiamF = (select CAST(ROUND(@GiaTriGiam, 0) as float))
    		DECLARE @TongTienLechF as float
    		set @TongTienLechF = (select CAST(ROUND(@TongTienLech, 0) as float))
    		DECLARE @TongChenhLechF as float
    		set @TongChenhLechF = (select CAST(ROUND(@TongChenhLech, 2) as float))
    		DECLARE @SoLuongTangF as float
    		set @SoLuongTangF = (select CAST(ROUND(@SoLuongTang, 2) as float))
    		DECLARE @SoLuongGiamF as float
    		set @SoLuongGiamF = (select CAST(ROUND(@SoLuongGiam, 2) as float))
    		DECLARE @SoLuongThucTeF as float
    		set @SoLuongThucTeF = (select CAST(ROUND(@SoLuongThucTe, 2) as float))
    	-- insert NhomHangHoa parent
    DECLARE @ID_NhomHangHoaCha  as uniqueidentifier
    	set @ID_NhomHangHoaCha = null
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin
    		--SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
			SELECT TOP(1) @ID_NhomHangHoaCha = ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha,@TenNhomHangHoaCha_KhongDau, @TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHHCha, null, 0)
    		End
    	End
    -- insert NhomHangHoa
    	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
    	if (len(@TenNhomHangHoa) > 0)
    	Begin
    		--SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
			SELECT TOP(1) @ID_NhomHangHoa = ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @MaNhomHangHoa, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHH, @ID_NhomHangHoaCha, 0)
    		End
    	End
    -- Update HangHoa
    		DECLARE @LaChaCungLoai  as int
    		set @LaChaCungLoai = 1;
    		DECLARE @ID_HangHoaCungLoai  as uniqueidentifier
    	set @ID_HangHoaCungLoai = newID();
    			if(len(@MaHangHoaChaCungLoai) > 0) -- nếu có thông tin mã hàng hóa cùng loại
    			Begin 
    				set @ID_HangHoaCungLoai = (select TOP(1) ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'); -- lấy ID_HangHoaCungLoai có chung tên khác
					--select TOP(1) @ID_HangHoaCungLoai = ID_HangHoaCungLoai FROM DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'
    				if (len(@ID_HangHoaCungLoai) > 0) 
    				Begin
    					set @LaChaCungLoai = 0;
    				End
    				else
    				Begin 
    					set @ID_HangHoaCungLoai = newID(); 
    				End
    			End
    		-- update HangHoa
    	if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    	Begin
    			update DM_HangHoa set ID_HangHoaCungLoai = @ID_HangHoaCungLoai, LaChaCungLoai = @LaChaCungLoai, ID_NhomHang = @ID_NhomHangHoa, LaHangHoa = @LaHangHoa,
    					NgaySua = @timeCreateHH, NguoiSua='admin', TenHangHoa = @TenHangHoa, TenHangHoa_KhongDau = @TenHangHoa_KhongDau, TenHangHoa_KyTuDau = @TenHangHoa_KyTuDau,
    					TenKhac = @MaHangHoaChaCungLoai, GhiChu = @GhiChu, QuyCach = @QuyCachF, DuocBanTrucTiep = @DuocBanTrucTiep, DonViTinhQuyCach= @DVTQuyCach Where ID = @ID_HangHoa
    	end
    -- update DonViQuiDoi
    		update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoiF, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBanF, NguoiSua ='admin', NgaySua =@timeCreateDVQD
    			Where ID = @ID_DonViQuiDoi

    -- insert kiểm kê TonKho
    	if (@LoaiUpdate = 2 and @LaHangHoa = 1)
    	Begin
		IF (@GiaVonF = -1000000000)
		BEGIN
			set @GiaVonF = 0;
			set @TongTienLechF = 0;
		END
    	--DECLARE @ID_HoaDon  as uniqueidentifier
    	--set @ID_HoaDon = newID();
    	--	insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    	--		values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
		DECLARE @TonLuyKeHH FLOAT = 0;
		DECLARE @tblIDDonViQuiDoi TABLE(ID UNIQUEIDENTIFIER);
		INSERT INTO @tblIDDonViQuiDoi
		SELECT ID FROM DonViQuiDoi WHERE ID_HangHoa = @ID_HangHoa AND Xoa = 0;

		SELECT TOP(1) @TonLuyKeHH = ISNULL(hdct.TonLuyKe, 0) FROM BH_HoaDon_ChiTiet hdct
		INNER JOIN @tblIDDonViQuiDoi dvqd
		ON hdct.ID_DonViQuiDoi = dvqd.ID
		WHERE hdct.ID_HoaDon = @ID_HoaDon;
			
		SET @TonLuyKeHH = @TonLuyKeHH + @SoLuongThucTeF * @TyLeChuyenDoiF;

		UPDATE hdct
		SET hdct.TonLuyKe = @TonLuyKeHH
		FROM BH_HoaDon_ChiTiet hdct
		INNER JOIN @tblIDDonViQuiDoi dvqd
		ON dvqd.ID = hdct.ID_DonViQuiDoi
		WHERE ID_HoaDon = @ID_HoaDon;

    	    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi)
    			Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@TonLuyKeHH, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi)
    	End
END");

            Sql(@"ALTER PROCEDURE [dbo].[import_DanhMucHangHoaLoHang_Update]
	@ID_HoaDon [uniqueidentifier],
	@SoThuTu [int],
    @LoaiUpdate [int],
    @ID_HangHoa [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @GiaTriTang [nvarchar](max),
    @GiaTriGiam [nvarchar](max),
    @TongTienLech [nvarchar](max),
    @TongChenhLech [nvarchar](max),
    @SoLuongThucTe [nvarchar](max),
    @SoLuongTang [nvarchar](max),
    @SoLuongGiam [nvarchar](max),
    @TenNhomHangHoaCha [nvarchar](max),
    @TenNhomHangHoaCha_KhongDau [nvarchar](max),
    @TenNhomHangHoaCha_KyTuDau [nvarchar](max),
    @MaNhomHangHoaCha [nvarchar](max),
    @timeCreateNHHCha [datetime],
    @TenNhomHangHoa [nvarchar](max),
    @TenNhomHangHoa_KhongDau [nvarchar](max),
    @TenNhomHangHoa_KyTuDau [nvarchar](max),
    @MaNhomHangHoa [nvarchar](max),
    @timeCreateNHH [datetime],
    @LaHangHoa [bit],
    @timeCreateHH [datetime],
    @TenHangHoa [nvarchar](max),
    @TenHangHoa_KhongDau [nvarchar](max),
    @TenHangHoa_KyTuDau [nvarchar](max),
    @GhiChu [nvarchar](max),
    @QuyCach [nvarchar](max),
    @DuocBanTrucTiep [bit],
    @MaDonViCoBan [nvarchar](max),
    @MaHangHoa [nvarchar](max),
    @TenDonViTinh [nvarchar](max),
    @GiaVon [nvarchar](max),
    @GiaBan [nvarchar](max),
    @timeCreateDVQD [datetime],
    @LaDonViChuan [bit],
    @TyLeChuyenDoi [nvarchar](max),
    @MaHoaDon [nvarchar](max),
    @DienGiai [nvarchar](max),
    @TonKho [nvarchar](max),
    @timeCreateHD [datetime],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHangHoaChaCungLoai [nvarchar](max),
    @MaLoHang [nvarchar](max),
    @NgaySanXuat [datetime],
    @NgayHetHan [datetime],
	@DVTQuyCach nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
    -- Khai báo biến update
    	DECLARE @GiaVonF as float
    		set @GiaVonF = (select CAST(ROUND(@GiaVon, 2) as float))
    	DECLARE @GiaBanF as float
    		set @GiaBanF = (select CAST(ROUND(@GiaBan, 2) as float))
    	DECLARE @QuyCachF as float
    		set @QuyCachF = (select CAST(ROUND(@QuyCach, 2) as float))
    	DECLARE @TyLeChuyenDoiF as float
    		set @TyLeChuyenDoiF = (select CAST(ROUND(@TyLeChuyenDoi, 2) as float))
    	DECLARE @TonKhoF as float
    		set @TonKhoF = (select CAST(ROUND(@TonKho, 2) as float))
    DECLARE @HangHoaNhieuDonViTinh BIT = 0;
	DECLARE @TonLuyKe FLOAT = 0;
	SET @TonLuyKe = @TonKhoF * @TyLeChuyenDoiF;
    		DECLARE @GiaTriTangF as float
    		set @GiaTriTangF = (select CAST(ROUND(@GiaTriTang, 0) as float))
    		DECLARE @GiaTriGiamF as float
    		set @GiaTriGiamF = (select CAST(ROUND(@GiaTriGiam, 0) as float))
    		DECLARE @TongTienLechF as float
    		set @TongTienLechF = (select CAST(ROUND(@TongTienLech, 0) as float))
    		DECLARE @TongChenhLechF as float
    		set @TongChenhLechF = (select CAST(ROUND(@TongChenhLech, 2) as float))
    		DECLARE @SoLuongTangF as float
    		set @SoLuongTangF = (select CAST(ROUND(@SoLuongTang, 2) as float))
    		DECLARE @SoLuongGiamF as float
    		set @SoLuongGiamF = (select CAST(ROUND(@SoLuongGiam, 2) as float))
    		DECLARE @SoLuongThucTeF as float
    		set @SoLuongThucTeF = (select CAST(ROUND(@SoLuongThucTe, 2) as float))
	SET @TonLuyKe = @SoLuongThucTeF * @TyLeChuyenDoiF;
	DECLARE @DieuKien as int
	set @DieuKien = 1;
    	-- insert NhomHangHoa parent
    DECLARE @ID_NhomHangHoaCha  as uniqueidentifier
    	set @ID_NhomHangHoaCha = null
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin
    		--SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
			select TOP(1) @ID_NhomHangHoaCha = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha, @TenNhomHangHoaCha_KhongDau, @TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHHCha, null, 0)
    		End
    	End
    -- insert NhomHangHoa
    	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
    	if (len(@TenNhomHangHoa) > 0)
    	Begin
    		--SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
			SELECT TOP(1) @ID_NhomHangHoa = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @MaNhomHangHoa, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHH, @ID_NhomHangHoaCha, 0)
    		End
    	End
    -- Update HangHoa
    		DECLARE @LaChaCungLoai  as int
    		set @LaChaCungLoai = 1;
    		DECLARE @ID_HangHoaCungLoai  as uniqueidentifier
    	set @ID_HangHoaCungLoai = newID();
    			if(len(@MaHangHoaChaCungLoai) > 0) -- nếu có thông tin mã hàng hóa cùng loại
    			Begin 
    				set @ID_HangHoaCungLoai = (select TOP(1) ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'); -- lấy ID_HangHoaCungLoai có chung tên khác
					--select TOP(1) @ID_HangHoaCungLoai = ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'
    				if (len(@ID_HangHoaCungLoai) > 0) 
    				Begin
    					set @LaChaCungLoai = 0;
    				End
    				else
    				Begin 
    					set @ID_HangHoaCungLoai = newID(); 
    				End
    			End
    		-- update HangHoa
    	if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    	Begin
    			update DM_HangHoa set ID_HangHoaCungLoai = @ID_HangHoaCungLoai, LaChaCungLoai = @LaChaCungLoai, ID_NhomHang = @ID_NhomHangHoa, LaHangHoa = @LaHangHoa,
    					NgaySua = @timeCreateHH, NguoiSua='admin', TenHangHoa = @TenHangHoa, TenHangHoa_KhongDau = @TenHangHoa_KhongDau, TenHangHoa_KyTuDau = @TenHangHoa_KyTuDau,
    					TenKhac = @MaHangHoaChaCungLoai, GhiChu = @GhiChu, QuyCach = @QuyCachF, DuocBanTrucTiep = @DuocBanTrucTiep,
						DonViTinhQuyCach = @DVTQuyCach
						Where ID = @ID_HangHoa
    	end
		ELSE
		BEGIN
			SET @HangHoaNhieuDonViTinh = 1;
		END
    		DECLARE @ID_LoHang  as uniqueidentifier
    	set @ID_LoHang = null
    		if(@MaLoHang != '')
    		Begin
    			update DM_HangHoa set QuanLyTheoLoHang = '1' where ID = @ID_HangHoa;
    			--SET @ID_LoHang =  (Select ID FROM DM_LoHang where MaLoHang = @MaLoHang and ID_HangHoa = @ID_HangHoa);
				SELECT TOP(1) @ID_LoHang = ID from DM_LoHang where MaLoHang= @MaLoHang and ID_HangHoa = @ID_HangHoa
    		if (@ID_LoHang is null or len(@ID_LoHang) = 0)
    		BeGin
    			SET @ID_LoHang = newID();
				SET @DieuKien = 0;
    			insert into DM_LoHang(ID, ID_HangHoa, MaLoHang, TenLoHang, NgaySanXuat, NgayHetHan, NguoiTao, NgayTao, TrangThai)
    			values (@ID_LoHang, @ID_HangHoa, @MaLoHang, @MaLoHang, @NgaySanXuat, @NgayHetHan, 'admin', GETDATE(), '1')
				-- update Tồn kho khi thêm mới lô hàng				
				if (@GiaVonF != -1000000000)
				BEGIN
					if (@LaDonViChuan = 1)
						BEGIN
						exec insert_DM_GiaVon @ID_DonViQuiDoi,@ID_DonVi,@ID_LoHang,@GiaVonF, @ID_NhanVien;
						END
					else
					BEGIN
						insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
    					Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVonF)
					END
				END

				else
				BEGIN
					set @GiaVonF = 0;
					set @TongTienLechF = 0;
				END

				--DECLARE @ID_HoaDonLH  as uniqueidentifier
    			--set @ID_HoaDonLH = newID();
    				--insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    				--	values (@ID_HoaDonLH, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    				insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    					Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@SoLuongThucTeF, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
    			End
    		End
    -- update DonViQuiDoi
    	update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoiF, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBanF, NguoiSua ='admin', NgaySua = @timeCreateDVQD
    			Where ID = @ID_DonViQuiDoi
    -- insert kiểm kê TonKho
		
    	if (@LoaiUpdate = 2 and @LaHangHoa = 1 and @DieuKien = 1)
    	Begin
		--DECLARE @ID_HoaDon  as uniqueidentifier
		If (@GiaVonF = -1000000000)
			BEGIN
			IF(@HangHoaNhieuDonViTinh = 1)
			BEGIN
				SELECT @TonLuyKe = @TonLuyKe + ISNULL(SUM(hdct.ThanhTien * dvqd.TyLeChuyenDoi), 0) FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));

				UPDATE hdct
				SET TonLuyKe = @TonLuyKe
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));
			END
			set @GiaVonF = 0;
			set @TongTienLechF = 0;
    		--set @ID_HoaDon = newID();
    			--insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    			--	values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    			insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    				Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@TonLuyKe, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
			END
		Else
			BEGIN
			IF(@HangHoaNhieuDonViTinh = 1)
			BEGIN
				SELECT @TonLuyKe = @TonLuyKe + ISNULL(SUM(hdct.ThanhTien * dvqd.TyLeChuyenDoi), 0) FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));

				UPDATE hdct
				SET TonLuyKe = @TonLuyKe
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));
			END
    			--set @ID_HoaDon = newID();
    				--insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    				--	values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    				insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    					Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@TonLuyKe, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
			END
		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[Load_DMHangHoa_TonKho_ChotSo]
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);	
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	Select
    		MAX(tr.ID) as ID,
    		tr.QuanLyTheoLoHang,   
			TyLeChuyenDoi,   
			LaDonViChuan,  
			case when MAX(QuyCach) = 0 then TyLeChuyenDoi else MAX(QuyCach) * TyLeChuyenDoi end as QuyCach,			
    		tr.TenHangHoa,
    		tr.MaHangHoa,
			CONCAT(tr.MaHangHoa, ' ' , tr.TenHangHoa, ' ', tr.TenHangHoa_KhongDau,' ',TenHangHoa_KyTuDau,' ',
			MAX(tr.MaLoHang),' ', max(tr.GiaBan)) as Name,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		SUM(tr.TonKho) as TonKho,
    		Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
    		MAX(tr.GiaBan) as GiaBan, 
    		MAX (tr.TenDonViTinh) as TenDonViTinh, 
    		tr.ID_NhomHangHoa,
    		tr.ID_DonViQuiDoi,			
    		MAX(tr.SrcImage) as SrcImage, 
    		tr.LaHangHoa,
    		Case when tr.ID_LoHang is null then NEWID() else tr.ID_LoHang end as ID_LoHang,
    		MAX(tr.MaLoHang) as MaLoHang,
    		MAX(tr.NgaySanXuat) as NgaySanXuat,
    		MAX(tr.NgayHetHan) as NgayHetHan,
			MAX(ISNULL(tr.PhiDichVu,0)) as PhiDichVu,
			MAX(ISNULL(tr.DonViTinhQuyCach,'')) as DonViTinhQuyCach,
			LaPTPhiDichVu,
			ThoiGianBaoHanh,
			LoaiBaoHanh,
			max(SoPhutThucHien) as SoPhutThucHien,
			max(DichVuTheoGio) as DichVuTheoGio,
			max(DuocTichDiem) as DuocTichDiem
    		 FROM
    		(
    			 Select  
    				dvqd1.ID_HangHoa as ID,
					ISNULL(dvqd1.LaDonViChuan,'0') as LaDonViChuan,   
    				ISNULL(dvqd1.TyLeChuyenDoi,1) as TyLeChuyenDoi,  
					CAST(ISNULL(dhh1.QuyCach,1) as float) as QuyCach,
					ISNULL(dhh1.QuanLyTheoLoHang,'0') as QuanLyTheoLoHang, 
    				dhh1.TenHangHoa,
					dvqd1.ThuocTinhGiaTri as ThuocTinh_GiaTri ,
    				--Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				dhh1.TenHangHoa_KhongDau,
    				dhh1.TenHangHoa_KyTuDau,
    				dvqd1.MaHangHoa, 					
    				0 as TonKho,
    				CAST(ROUND((dvqd1.GiaBan), 0) as float) as GiaBan, 
    				dvqd1.TenDonViTinh, 
    				dhh1.ID_NhomHang as ID_NhomHangHoa,
    				dvqd1.ID as ID_DonViQuiDoi,
    				an.URLAnh as SrcImage,
    				dhh1.LaHangHoa,
    				lh1.ID as ID_LoHang,
    				lh1.MaLoHang,
    				lh1.NgaySanXuat,
    				lh1.NgayHetHan,
    				lh1.TrangThai,
    				Case when dhh1.LaHangHoa='1' then 0 else CAST(ISNULL(dhh1.ChiPhiThucHien,0) as float) end as PhiDichVu,
					Case when dhh1.LaHangHoa='1' then '0' else ISNULL(dhh1.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
					ISNULL(dhh1.ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
					ISNULL(dhh1.LoaiBaoHanh,0) as LoaiBaoHanh,
					dhh1.DonViTinhQuyCach as DonViTinhQuyCach,
					ISNULL(dhh1.SoPhutThucHien,0) as SoPhutThucHien,
					ISNULL(dhh1.DichVuTheoGio,0) as DichVuTheoGio,
					ISNULL(dhh1.DuocTichDiem,0) as DuocTichDiem

    			 from
    				 DonViQuiDoi dvqd1 
    				 join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
    				 left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa
    				 LEFT join DM_HangHoa_Anh an on (dvqd1.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    				 --LEFT JOIN (Select Main.id_hanghoa,
    					--	Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    					--	From
    					--	(
    					--	Select distinct hh_tt.id_hanghoa,
    					--		(
    					--			Select tt.GiaTri + ' - ' AS [text()]
    					--			From dbo.hanghoa_thuoctinh tt
    					--			Where tt.id_hanghoa = hh_tt.id_hanghoa
    					--			order by tt.ThuTuNhap 
    					--			For XML PATH ('')
    					--		) hanghoa_thuoctinh
    					--	From dbo.hanghoa_thuoctinh hh_tt
    					--	) Main
    					--) as ThuocTinh on dvqd1.ID_HangHoa = ThuocTinh.id_hanghoa
    				where  dhh1.duocbantructiep = '1'  and dhh1.TheoDoi = '1' and dvqd1.xoa = '0'
    			 Union all
    
    			SELECT 
    				dvqd3.ID_HangHoa as ID,
					ISNULL(dvqd3.LaDonViChuan,'0') as LaDonViChuan,   
    				ISNULL(dvqd3.TyLeChuyenDoi,1) as TyLeChuyenDoi, 
					CAST(ISNULL(dhh.QuyCach,1) as float) as QuyCach,
					ISNULL(dhh.QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
    				dhh.TenHangHoa,
					'' as ThuocTinh_GiaTri,
    				--Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				dhh.TenHangHoa_KhongDau,
    				dhh.TenHangHoa_KyTuDau,
    				dvqd3.MaHangHoa, 
    				Case when dhh.LaHangHoa = 0 then 0 else CAST(ROUND(ISNULL(a.TonCuoiKy / dvqd3.TyLeChuyenDoi, 0), 3) as float) end as TonKho,
    				CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
    				dvqd3.TenDonViTinh, 
    				dhh.ID_NhomHang as ID_NhomHangHoa,
    				dvqd3.ID as ID_DonViQuiDoi,
    				'' as SrcImage,
    				dhh.LaHangHoa,
    				a.ID_LoHang,
    				a.MaLoHang,
    				a.NgaySanXuat,
    				a.NgayHetHan, 
    				a.TrangThai,
    				Case when dhh.LaHangHoa='1' then 0 else CAST(ISNULL(dhh.ChiPhiThucHien,0) as float) end as PhiDichVu,
					Case when dhh.LaHangHoa='1' then '0' else ISNULL(dhh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
					ISNULL(dhh.ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
					ISNULL(dhh.LoaiBaoHanh,0) as LoaiBaoHanh,
					dhh.DonViTinhQuyCach as DonViTinhQuyCach,
					0 as SoPhutThucHien,
					0 as DichVuTheoGio,
					0 as DuocTichDiem
    	FROM 
    		DonViQuiDoi dvqd3
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd3.ID_HangHoa
    		LEFT JOIN 
    		 (
    		SELECT 
    			dhh.ID,
    			MAX(dhh.TenHangHoa)   AS TenHangHoa,
    			MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    			MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    			MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    			MAX(CAST(ISNULL(dhh.QuyCach,1) as float)) as QuyCach,
				ISNULL(dhh.QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
    			MAX(lh.ID)  As ID_LoHang,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = 0 then '' else lh.MaLoHang end) As MaLoHang,
    			MAX(lh.NgaySanXuat)  As NgaySanXuat,
    			MAX(lh.NgayHetHan)  As NgayHetHan,
    			lh.TrangThai,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    				td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    					SELECT 
    						dvqd.ID As ID_DonViQuiDoi,
    						Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    						NULL AS SoLuongNhap,
    						NULL AS SoLuongXuat,
    						SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				where dvqd.ladonvichuan = '1' AND dvqd.Xoa = '0'
    				AND hh.DuocBanTrucTiep = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					NULL AS SoLuongNhap,
    					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and dvqd.xoa = '0'
					and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                                
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					NULL AS SoLuongNhap,
    					SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND dvqd.xoa = '0'
					AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    					null AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9')AND dvqd.xoa = '0' AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    					null AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND dvqd.xoa = '0'
					AND bhd.ChoThanhToan = 0
    					AND bhd.NgayLapHoaDon >= @timeChotSo

    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    		right JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			GROUP BY dhh.ID, dhh.LaHangHoa, dhh.ID_NhomHang,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    on dvqd3.ID_HangHoa = a.ID
    --LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    --	LEFT join DM_HangHoa hh on dvqd3.ID_HangHoa = hh.ID
    --	LEFT JOIN (Select Main.id_hanghoa,
    --						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    --						From
    --						(
    --						Select distinct hh_tt.id_hanghoa,
    --							(
    --								Select tt.GiaTri + ' - ' AS [text()]
    --								From dbo.hanghoa_thuoctinh tt
    --								Where tt.id_hanghoa = hh_tt.id_hanghoa
    --								order by tt.ThuTuNhap 
    --								For XML PATH ('')
    --							) hanghoa_thuoctinh
    --						From dbo.hanghoa_thuoctinh hh_tt
    --						) Main
    --					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where dvqd3.xoa = '0' and dhh.duocbantructiep = '1'  and dhh.TheoDoi = '1'	
    	and ((a.QuanLyTheoLoHang = 1 and a.ID_LoHang is not null) or a.QuanLyTheoLoHang = '0')
    		) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		where tr.TrangThai = 1 or tr.TrangThai is null
    		Group by tr.ID_DonViQuiDoi,tr.ID, tr.ID_LoHang, tr.QuanLyTheoLoHang, gv.ID, gv.GiaVon, tr.ID_NhomHangHoa, tr.LaHangHoa, tr.TyLeChuyenDoi,tr.LaDonViChuan,
					tr.MaHangHoa, tr.TenHangHoa, tr.TenHangHoa_KhongDau, tr.TenHangHoa_KyTuDau, tr.LaPTPhiDichVu, tr.ThoiGianBaoHanh, tr.LoaiBaoHanh
			having convert(varchar, MAX(tr.NgayHetHan),112) >= convert(varchar, getdate(),112) or MAX(tr.NgayHetHan) is null
    		order by tr.MaHangHoa, MAX(tr.NgayHetHan)
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetHT_CauHinh_TichDiem]
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select ct.ID as ID_TichDiem,
    		ISNULL(ad.ID,'00000000-0000-0000-0000-000000000000') as ID_ApDung,
    		ISNULL(ad.ID_NhomDoiTuong,'00000000-0000-0000-0000-000000000000')  as ID_NhomDoiTuong,
    		ct.TyLeDoiDiem,
    		ct.ThanhToanBangDiem,
    		ct.KhoiTaoTichDiem,
    		ISNUll(ct.TichDiemGiamGia,'0') as TichDiemGiamGia,
    		ct.TichDiemHoaDonDiemThuong,
    		ct.TienThanhToan,
    		ct.DiemThanhToan,
    		ct.ToanBoKhachHang,
    		ISNUll(ct.TichDiemHoaDonGiamGia,'0') as TichDiemHoaDonGiamGia,
    		ISNUll(ct.SoLanMua,0) as SoLanMua
    	from HT_CauHinh_TichDiemChiTiet ct
    	left join HT_CauHinh_TichDiemApDung ad on ct.ID= ad.ID_TichDiem
    	join HT_CauHinhPhanMem ch on ct.ID_CauHinh = ch.ID
    	where ch.ID_DonVi like @ID_DonVi
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetThucDonWait]
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select 
    	hd.MaHoaDon ,
    	hd.ID_ViTri,
    	ISNULL(vt.TenViTri,N'Mang về') as TenPhongBan,
    	ct.ID,
		ct.SoLuong,
		ct.ID_DonViQuiDoi,    
		ct.Bep_SoLuongYeuCau,
		ct.Bep_SoLuongHoanThanh,
		ct.Bep_SoLuongChoCungUng,
		ct.ThoiGian ,    
		hh.TenHangHoa ,
		ct.ID_HoaDon,
		ct.GhiChu ,
		ct.TienThue,
    	qd.ID_HangHoa ,
		qd.MaHangHoa,
		hh.LaHangHoa
    
    from BH_HoaDon hd
    join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    left join DM_ViTri vt on hd.ID_ViTri= vt.ID
    join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    left join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
    where ISNULL(ct.Bep_SoLuongChoCungUng,0) > 0 and hd.ChoThanhToan = '1'
    and hd.ID_DonVi like @ID_DonVi and hd.LoaiHoaDon=3
	and (hh.LaHangHoa = '1' or ( hh.LaHangHoa ='0' and isnull(hh.DichVuTheoGio,0) = 0))
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetThucDonYeuCau]
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select 
    	hd.MaHoaDon ,
    	hd.ID_ViTri,
		ISNULL(vt.TenViTri,N'Mang về') as TenPhongBan,
    	ct.ID,
		ct.SoLuong,
		ct.ID_DonViQuiDoi,    
		ct.Bep_SoLuongYeuCau,
		ct.Bep_SoLuongHoanThanh,
		ct.Bep_SoLuongChoCungUng,
		ct.ThoiGian ,    
		hh.TenHangHoa ,
		ct.ID_HoaDon,
		ct.GhiChu ,
		ct.TienThue,
    	qd.ID_HangHoa ,
		qd.MaHangHoa,
		hh.LaHangHoa
    
    from BH_HoaDon hd
    join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    left join DM_ViTri vt on hd.ID_ViTri= vt.ID
    join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    left join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
    where ISNULL(ct.Bep_SoLuongYeuCau,0) > 0 and hd.ChoThanhToan = '1'
    and hd.ID_DonVi like @ID_DonVi and hd.LoaiHoaDon = 3
	and (hh.LaHangHoa = '1' or ( hh.LaHangHoa ='0' and isnull(hh.DichVuTheoGio,0) = 0))
END");

            Sql(@"update dt set NgayGiaoDichGanNhat = hd.NgayLapHoaDon
from DM_DoiTuong dt
join
(
	select hdmax.*
	from (
		select hd.ID_DoiTuong, hd.NgayLapHoaDon, 
		ROW_NUMBER() over (  PARTITION BY hd.ID_DoiTuong order by hd.NgayLapHoaDon desc) RowNum 
		from BH_HoaDon hd
		where hd.ChoThanhToan is not null and hd.ID_DoiTuong is not null
	)hdmax
where RowNum= 1 ) hd on dt.ID = hd.ID_DoiTuong;");

        }
        
        public override void Down()
        {
			DropStoredProcedure("[dbo].[BaoCaoKhachHang_TanSuat]");
			DropStoredProcedure("[dbo].[GetListPhieuPhanCa_Paging]");
			DropStoredProcedure("[dbo].[GetNhatKyGiaoDich_ofKhachHang]");
			DropStoredProcedure("[dbo].[GetThietLapLuong_ofNhanVien]");
			DropStoredProcedure("[dbo].[HuyPhieuThu_UpdateCongNoTamUngLuong]");
			DropStoredProcedure("[dbo].[RemoveCong_ofNhanVien]");
			DropStoredProcedure("[dbo].[UpdatePhieuPhanCa_CheckExistCong]");
			DropStoredProcedure("[dbo].[UpdatePhieuPhanCa_RemoveCongNhanVien]");
        }
    }
}
