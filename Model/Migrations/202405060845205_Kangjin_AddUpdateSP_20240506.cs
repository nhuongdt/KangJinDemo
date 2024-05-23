namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Kangjin_AddUpdateSP_20240506 : DbMigration
    {
        public override void Up()
        {
            Sql("DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon");

			Sql(@"CREATE FUNCTION [dbo].[fnGetAllHangHoa_NeedUpdateTonKhoGiaVon]
(	
	@IDHoaDon uniqueidentifier
)
RETURNS TABLE 
AS
RETURN 
(
	
			select distinct
				qdOut.ID as ID_DonViQuiDoi,
				qdOut.ID_HangHoa,
				qdIn.ID_LoHang,
				qdOut.TyLeChuyenDoi,				
				hh.LaHangHoa

			from
			(
				------ chi get idLohang from bh_chitiet ---
				select 
					qd.ID_HangHoa,
					lo.ID as ID_LoHang
				from
				(
					select ct.ID_DonViQuiDoi, ct.ID_LoHang
					from BH_HoaDon_ChiTiet ct
					where ct.ID_HoaDon = @IDHoaDon 
				)ct
			join DonViQuiDoi qd on qd.ID = ct.ID_DonViQuiDoi
			left join DM_LoHang lo on ct.ID_LoHang = lo.ID or lo.ID is null and ct.ID_LoHang is null
			)qdIn
			join DonViQuiDoi qdOut on qdIn.ID_HangHoa = qdOut.ID_HangHoa
			join DM_HangHoa hh on qdOut.ID_HangHoa = hh.ID and qdIn.ID_HangHoa = hh.ID 
			where LaHangHoa='1'
)
");

			Sql(@"ALTER FUNCTION [dbo].[fnDemSoLanDoiTra]
(
	@ID uniqueidentifier
)
RETURNS int
AS
BEGIN

	DECLARE @count int = 0

			select @count = sum(SoLan) 					
				from
				(
					select 	
						iif(ID_HoaDon is null,0,1)
						+ isnull((select dbo.fnDemSoLanDoiTra(ID_HoaDon)),0) as SoLan									
					from BH_HoaDon hd
					where ID = @ID
				)tbl
	
	RETURN @count

END
");

			Sql(@"ALTER FUNCTION [dbo].[GetTongTra_byIDHoaDon]
(
	@ID_HoaDon uniqueidentifier,
	@NgayLapHoaDon datetime
)
RETURNS float
AS
BEGIN
		DECLARE @Gtri float=0

		---- get all hdTra of hdgoc with ngaylap < ngaylap of HDTra current --
		declare @tblHDTra table (ID uniqueidentifier, PhaiThanhToan float)
		insert into @tblHDTra
		select ID, PhaiThanhToan
		from BH_HoaDon
		where LoaiHoaDon = 6
		and ChoThanhToan='0'
		and ID_HoaDon= @ID_HoaDon
		and NgayLapHoaDon < @NgayLapHoaDon ---- hd$root: get allHDTra (don't check NgayLap)


		declare @tblHDDoi_fromHdTra table (ID uniqueidentifier, ID_HoaDon uniqueidentifier, PhaiThanhToan float)
		insert into @tblHDDoi_fromHdTra
		select hdDoi.ID,
			hdDoi.ID_HoaDon,
			hdDoi.PhaiThanhToan
		from BH_HoaDon hdDoi
		where LoaiHoaDon in (1,19)
		and ChoThanhToan='0'
		and exists (select id from @tblHDTra hdTra where hdDoi.ID_HoaDon = hdTra.ID)
		and hdDoi.NgayLapHoaDon < @NgayLapHoaDon

	set @Gtri = (
					select 					
						sum(PhaiThanhToan + DaTraKhach + NoHDDoi) as NoKhach
					from
					(
						select ID,
							-PhaiThanhToan as PhaiThanhToan, 
							0 as DaTraKhach,
							0 as NoHDDoi
						from @tblHDTra					

						union all
						---- get phieuchi hdTra ----
						select 
							hdt.ID,
							0 as PhaiThanhToan,
							iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu) as DaTraKhach,
							0 as NoHDDoi
						from @tblHDTra hdt
						join Quy_HoaDon_ChiTiet qct on hdt.ID = qct.ID_HoaDonLienQuan
						join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
						where qhd.TrangThai='1'
						and qhd.NgayLapHoaDon < @NgayLapHoaDon

						union all
						---- get all HDDoifrom hdTra ----						
						select 
							hdDoi.ID_HoaDon,	
							0 as PhaiThanhToan,
							0 as DaTraKhach,
							sum(hdDoi.PhaiThanhToan) - sum(ISNULL(DaTraKhach,0)) as NoHDDoi
						from @tblHDDoi_fromHdTra hdDoi
						left join
						(
							---- all phieuthu of hdDoi ---
							select 
								hdDoi.ID,
								iif(qhd.LoaiHoaDon=11, qct.TienThu, -qct.TienThu) as DaTraKhach
							from @tblHDDoi_fromHdTra hdDoi
							join Quy_HoaDon_ChiTiet qct on hdDoi.ID = qct.ID_HoaDonLienQuan
							join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
							where qhd.TrangThai='1'
						)sq on hdDoi.ID = sq.ID
						group by hdDoi.ID_HoaDon											

					) tblThuChi
		)
	RETURN @Gtri 

END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKhachHang_TanSuat]
	@IDChiNhanhs nvarchar(max),	
	@IDNhomKhachs nvarchar(max),
	@LoaiChungTus varchar(20),
	@TrangThaiKhach varchar(10),
	@FromDate datetime,
	@ToDate datetime,
	@NgayGiaoDichFrom datetime,
	@NgayGiaoDichTo datetime,
	@NgayTaoKHFrom datetime,
	@NgayTaoKHTo datetime,
	@DoanhThuTu float,	
	@DoanhThuDen float,
	@SoLanDenFrom int,
	@SoLanDenTo int,
	@TextSearch nvarchar(max),
	@CurrentPage int,
	@PageSize int,
	@ColumnSort varchar(200),
	@TypeSort varchar(20)
AS
BEGIN
	
	SET NOCOUNT ON;
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
	select Name from dbo.splitstring(@IDNhomKhachs);

	
	if @DoanhThuTu is null
		set @DoanhThuTu = -10000000

	if @DoanhThuDen is null
		set @DoanhThuDen = 9999999999
	if @SoLanDenTo is null
		set @SoLanDenTo = 9999999;

	with data_cte
	as(
	select dt.ID as ID_KhachHang, dt.MaDoiTuong as MaKhachHang, dt.TenDoiTuong as TenKhachHang,
		dt.DienThoai as DienThoai1, --- kangjin yêu cầu bảo mật SDT khách hàng ở full bao cáo (DienThoai1 chỉ lấy ra để where thôi)
		dt.DiaChi, dt.TenNhomDoiTuongs,
		tblMaxGD.NgayGiaoDichGanNhat,
		hd1.SoLanDen,
		GiaTriMua, 
		GiaTriTra,
		DoanhThu
	from DM_DoiTuong dt  
	left join (
		select hd.ID_DoiTuong,
			max(hd.NgayLapHoaDon) as NgayGiaoDichGanNhat
		from BH_HoaDon hd
		where hd.ChoThanhToan is not null
		group by hd.ID_DoiTuong
	)tblMaxGD on dt.id = tblMaxGD.ID_DoiTuong
	join (		
			select hd.ID_DoiTuong, 
			count(hd.ID) as SoLanDen, 					
			sum(isnull(hd.GiaTriTra,0)) as GiaTriTra,
			sum(hd.GiaTriMua) as GiaTriMua,
			sum(hd.GiaTriMua - hd.GiaTriTra) as DoanhThu
			from(
				-- doanhthu: khong tinh napthe (chi tinh luc su dung)
				-- vi BC chi tiet theo khachhang: khong lay duoc dichvu/hanghoa khi napthe
				select  hd.ID_DoiTuong, hd.ID,
					iif(hd.LoaiHoaDon= 6, hd.TongTienHang - hd.TongGiamGia,0) as GiaTriTra,
					case hd.LoaiHoaDon
						when 6 then 0
						when 36 then 0
						else hd.TongTienHang - hd.TongGiamGia - isnull(hd.KhuyeMai_GiamGia,0)
					end as GiaTriMua			
				from BH_HoaDon hd
				where hd.ChoThanhToan = 0
				and hd.LoaiHoaDon != 22
				and hd.ID_DoiTuong is not null
				and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon <= @ToDate
				and exists (select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
				and exists (select * from dbo.splitstring(@LoaiChungTus) ct where hd.LoaiHoaDon= ct.Name)
			) hd group by hd.ID_DoiTuong			
	) hd1 on dt.ID = hd1.ID_DoiTuong
    where 
		 exists (select nhom1.Name 
			from dbo.splitstring(iif(dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs ='','00000000-0000-0000-0000-000000000000',dt.IDNhomDoiTuongs)) nhom1 
		 join @tblNhomDT nhom2 on nhom1.Name = nhom2.ID) 
	and
		dt.TheoDoi like @TrangThaiKhach
	and tblMaxGD.NgayGiaoDichGanNhat >= @NgayGiaoDichFrom and tblMaxGD.NgayGiaoDichGanNhat < @NgayGiaoDichTo
	and ((dt.NgayTao >= @NgayTaoKHFrom and dt.NgayTao < @NgayTaoKHTo) or ( dt.ID ='00000000-0000-0000-0000-000000000000'))
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
		sum(GiaTriMua) as TongMua,
		sum(GiaTriTra) as TongTra,
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
				when 'GiaTriMua' then cast(dt.GiaTriMua as float)
				when 'GiaTriTra' then cast(dt.GiaTriTra as float)
				when 'NgayGiaoDichGanNhat' then cast(dt.NgayGiaoDichGanNhat as float)
				end
			 END ASC,
		CASE WHEN @TypeSort = 'DESC'  THEN 
			case @ColumnSort
				when 'SoLanDen' then cast(dt.SoLanDen as float)
				when 'DoanhThu' then cast(dt.DoanhThu as float)
				when 'GiaTriMua' then cast(dt.GiaTriMua as float)
				when 'GiaTriTra' then cast(dt.GiaTriTra as float)
				when 'NgayGiaoDichGanNhat' then cast(dt.NgayGiaoDichGanNhat as float)
			end
		END DESC
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMuc_KhachHangNhaCungCap]
--declare
	@IDChiNhanhs [nvarchar](max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @LoaiDoiTuong [int] = 1,
    @IDNhomKhachs [nvarchar](max) ='',
    @TongBan_FromDate [datetime] ='',
    @TongBan_ToDate [datetime]='',
    @NgayTao_FromDate [datetime] ='',
    @NgayTao_ToDate [datetime]='',
    @TextSearch [nvarchar](max)='KN536',
    @Where [nvarchar](max)='',
    @ColumnSort [nvarchar](40)='',
    @SortBy [nvarchar](40)='DESC',
    @CurrentPage [int]=0,
    @PageSize [int] = 20
AS
BEGIN
    SET NOCOUNT ON;
    	declare @whereCus nvarchar(max), @whereInvoice nvarchar(max), @whereLast nvarchar(max), 
    	@whereNhomKhach nvarchar(max),	@whereChiNhanh nvarchar(max), @whereNgayLapHD nvarchar(max),
    	@sql nvarchar(max) , @sql1 nvarchar(max), @sql2 nvarchar(max), @sql3 nvarchar(max),@sql4 nvarchar(max),
    	@paramDefined nvarchar(max)
    
    		declare @tblDefined nvarchar(max) = concat(N' declare @tblChiNhanh table (ID uniqueidentifier) ',	
    												   N' declare @tblIDNhoms table (ID uniqueidentifier) ',
    												   N' declare @tblSearch table (Name nvarchar(max))'    											 
    												   )
    
    
    		set @whereInvoice =' where 1 = 1 and hd.ChoThanhToan = 0 '
    		set @whereCus =' where 1 = 1 and dt.LoaiDoiTuong = @LoaiDoiTuong_In '		
    		set @whereLast = N' where tbl.ID not like ''00000000-0000-0000-0000-000000000%'' '
    		set @whereNhomKhach =' ' 
    		set @whereChiNhanh =' where 1 = 1 ' 
			set @whereNgayLapHD =' ' --- because quyHoaDon = @where chinhanh + @where ngaylapHD
    
    		if isnull(@CurrentPage,'')=''
    			set @CurrentPage =0
    		if isnull(@PageSize,'')=''
    			set @PageSize = 10
    
    		if isnull(@ColumnSort,'')=''
    			set @ColumnSort = 'NgayTao'
    		if isnull(@SortBy,'')=''
    			set @SortBy = 'DESC'
    
    		set @sql1= 'declare @count int = 0'
    
    		declare @QLTheoCN bit = '0'
    		if ISNULL(@IDChiNhanhs,'')!=''
    			begin								
    				set @QLTheoCN = (select max(cast(QuanLyKhachHangTheoDonVi as int)) from HT_CauHinhPhanMem 
    					where exists (select * from dbo.splitstring(@IDChiNhanhs) cn where ID_DonVi= cn.Name))
    
    				set @sql1 = concat(@sql1,
    				N' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In)')
    
    				set @whereChiNhanh= CONCAT(@whereChiNhanh, ' and exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)')
    				set @whereInvoice= CONCAT(@whereInvoice, ' and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)')
    			end
    		
    
    		if ISNULL(@IDNhomKhachs,'')='' ---- idNhom = empty
    			begin			
    				set @sql1 = concat(@sql1,
    				N' insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')')
    
    				if @QLTheoCN = 1
    					begin
    						set @sql1 = concat(@sql1, N' insert into @tblIDNhoms(ID)
    						select * 
    						from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select ID from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = @LoaiDoiTuong_In
    						union all
    						-- get Nhom at this ChiNhanh
    						select ID_NhomDoiTuong  from NhomDoiTuong_DonVi ', @whereChiNhanh,
    						N' ) tbl ')	
    						
    						set @whereNhomKhach  = CONCAT(@whereNhomKhach,
    						N' and EXISTS(SELECT Name FROM splitstring(tbl.ID_NhomDoiTuong) lstFromtbl 
    								inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID where lstFromtbl.Name!='''' )')	
    					end										
    			end
    		else
    		begin
    			set @sql1=  CONCAT(@sql1, N' insert into @tblIDNhoms values ( CAST(@IDNhomKhachs_In as uniqueidentifier) ) ')
    			set @whereNhomKhach  = CONCAT(@whereNhomKhach,
    			N' and EXISTS(SELECT Name FROM splitstring(tbl.ID_NhomDoiTuong) lstFromtbl 
    					inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID where lstFromtbl.Name!='''' )')			
    		end
    
    		if isnull(@TextSearch,'') !=''
    			begin
    				set @sql1= CONCAT(@sql1, N' 
    				INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''';
    			Select @count =  (Select count(*) from @tblSearch);')
    
    				set @whereLast = CONCAT(@whereLast,
    				 N' and ((select count(Name) from @tblSearch b where 				
    				 tbl.Name_Phone like ''%''+b.Name+''%''    		
    				)=@count or @count=0)')
    			end
    
    		if isnull(@NgayTao_FromDate,'') !=''
    			if isnull(@NgayTao_ToDate,'') !=''
    				begin
    					set @whereCus = CONCAT(@whereCus, N' and dt.NgayTao between @NgayTao_FromDate_In and @NgayTao_ToDate_In')
    				end
    
    		if isnull(@TongBan_FromDate,'') !=''
    			if isnull(@TongBan_ToDate,'') !=''
    				begin
    					set @whereInvoice = CONCAT(@whereInvoice, N' and hd.NgayLapHoaDon between @TongBan_FromDate_In and @TongBan_ToDate_In')
						set @whereNgayLapHD = N' and NgayLapHoaDon between @TongBan_FromDate_In and @TongBan_ToDate_In' ---- !important: only {NgayLapHoaDon}
    				end			
    
    		if ISNULL(@Where,'')!=''
    			begin
    				set @Where = CONCAT(@whereLast, @whereNhomKhach, ' and ', @Where)
    			end
    		else
    			begin
    				set @Where = concat(@whereLast, @whereNhomKhach)
    			end
    		
    	set @sql2 = concat(
    		N'
    	;with data_cte
    	as
    	(
    		select 
				tbl.ID,
    			tbl.MaDoiTuong,
    			tbl.TenDoiTuong,
    			tbl.TenDoiTuong_KhongDau,
    			tbl.TenDoiTuong_ChuCaiDau,
    			tbl.LoaiDoiTuong,
    			tbl.ID_TrangThai,
    			tbl.ID_NguonKhach,
    			tbl.ID_NhanVienPhuTrach,
    			tbl.ID_NguoiGioiThieu,
    			tbl.ID_DonVi,
    			tbl.ID_TinhThanh,
    			tbl.ID_QuanHuyen,
				tbl.TheoDoi,
    			tbl.LaCaNhan,				
    			tbl.GioiTinhNam,
    			tbl.NgaySinh_NgayTLap,
    			tbl.DinhDang_NgaySinh,
    			tbl.NgayGiaoDichGanNhat,
    			tbl.TaiKhoanNganHang,
    			tbl.TenNhomDT,
    			tbl.NgayTao,
    			tbl.TrangThai_TheGiaTri,
    			tbl.TongTichDiem,
    			tbl.DienThoai,
    			tbl.Email,
    			tbl.DiaChi,
    			tbl.MaSoThue,
    			tbl.GhiChu,
    			tbl.NguoiTao,
    			tbl.ID_NhomDoiTuong,
				tbl.Name_Phone,
				TongThuKhachHang,
				TongChiKhachHang,
				GiaTriDVHoanTra,
				DieuChinhSoDuTGT,
				GiaTriDVSuDung,
				NoHienTai,
				TongBan,
				TongBanTruTraHang,
				SoLanMuaHang,
				PhiDichVu,
				NapCoc,
				SuDungCoc,
				SoDuCoc,
				NapTienTGT,
				SuDungTGT,
				ThanhToanGDV,
				HoanTraThe,
				SuDungGDV,
				iif(tbl.SoTienChuaSD <0,0,tbl.SoTienChuaSD) as SoTienChuaSD
    		from
    		(
    		select 
    			dt.*,
				isnull(tblMaxGD.NgayGiaoDichGanNhat,null) as NgayGiaoDichGanNhat,
				isnull(a.TongThuKhachHang,0) as TongThuKhachHang,
				isnull(a.TongChiKhachHang,0) as TongChiKhachHang,
				isnull(traGDV.GiaTriHoanTraGDV,0) as GiaTriDVHoanTra,
				isnull(a.DieuChinhSoDuTGT,0) as DieuChinhSoDuTGT,
				isnull(a.NapTienTGT,0)  as NapTienTGT,
				isnull(a.SuDungTGT,0) as SuDungTGT,
				isnull(a.ThanhToanGDV,0) as ThanhToanGDV,
				isnull(tblSuDung.SuDungGDV,0) as SuDungGDV,
				isnull(a.ChiTuGDV,0) as ChiTuGDV,
				isnull(a.TienKhach_biGiamTru,0) as TienKhach_biGiamTru,
				isnull(a.HoanTraThe,0) as HoanTraThe,
				----- === giatrisudung = sudung (tu hoadonle + gdv) -----
				isnull(tblSuDung.GiaTriSuDung,0)  as GiaTriDVSuDung,

				
				----- === tiền còn lại chưa dùng = số dư TGT + tiền GDV chưa dùng (không liên quan đến  hdLẻ) ----
				isnull(a.NapTienTGT,0) - isnull(a.SuDungTGT,0) + isnull(a.HoanTraThe,0) + isnull(a.DieuChinhSoDuTGT,0 )
				+ isnull(a.ThanhToanGDV,0) - isnull(a.TienKhach_biGiamTru,0)  - isnull(a.ChiTuGDV,0) - isnull(tblSuDung.SuDungGDV,0) 
					as SoTienChuaSD,

    			isnull(a.NoHienTai,0) as NoHienTai,
    			isnull(a.TongBan,0) as TongBan,
    			isnull(a.TongMua,0) as TongMua,
    			isnull(a.TongBanTruTraHang,0) as TongBanTruTraHang,
    			cast(isnull(a.SoLanMuaHang,0) as float) as SoLanMuaHang,
    			isnull(a.PhiDichVu,0) as PhiDichVu,
				isnull(a.NapCoc,0) as NapCoc,
				isnull(a.SuDungCoc,0) as SuDungCoc,
				isnull(a.SoDuCoc,0) as SoDuCoc,
				SUBSTRING(DienThoai1,len(DienThoai1) -2 , 3) as DienThoai,
    			CONCAT(dt.MaDoiTuong,'' '', dt.TenDoiTuong, '' '', dt.DienThoai1, '' '', dt.TenDoiTuong_KhongDau) as Name_Phone
    		from (
    			select 
    				dt.ID,
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				dt.TenDoiTuong_KhongDau,
    				dt.TenDoiTuong_ChuCaiDau,
    				dt.LoaiDoiTuong,
    				dt.ID_TrangThai,
    				dt.ID_NguonKhach,
    				dt.ID_NhanVienPhuTrach,
    				dt.ID_NguoiGioiThieu,
    				dt.ID_DonVi,
    				dt.ID_TinhThanh,
    				dt.ID_QuanHuyen,
    				isnull(dt.TheoDoi,''0'') as TheoDoi,
    				dt.LaCaNhan,				
    				dt.GioiTinhNam,
    				dt.NgaySinh_NgayTLap,
    				dt.DinhDang_NgaySinh,
    			
    				dt.TaiKhoanNganHang,
    				isnull(dt.TenNhomDoiTuongs,N''Nhóm mặc định'') as TenNhomDT,
    				dt.NgayTao,
    				isnull(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    				isnull(dt.TongTichDiem,0) as TongTichDiem,
    				----isnull(dt.TheoDoi,''0'') as TrangThaiXoa,
    				isnull(dt.DienThoai,'''') as DienThoai1,
    				isnull(dt.Email,'''') as Email,
    				isnull(dt.DiaChi,'''') as DiaChi,
    				isnull(dt.MaSoThue,'''') as MaSoThue,
    				isnull(dt.GhiChu,'''') as GhiChu,
    				ISNULL(dt.NguoiTao,'''') as NguoiTao,
    				iif(dt.IDNhomDoiTuongs='''' or dt.IDNhomDoiTuongs is null,''00000000-0000-0000-0000-000000000000'', dt.IDNhomDoiTuongs) as ID_NhomDoiTuong
    			from DM_DoiTuong dt ', @whereCus, N' )  dt
				left join
				(
					select 
						hd.ID_DoiTuong,
						max(hd.NgayLapHoaDon) as NgayGiaoDichGanNhat
					from BH_HoaDon hd
					where hd.ChoThanhToan =0				
					group by hd.ID_DoiTuong
				)tblMaxGD on dt.ID = tblMaxGD.ID_DoiTuong
				left join 
				(
				 ----- Hoàn dịch vụ: chỉ lấy phiếu chi trả hàng từ hóa đơn lẻ ---
					 select 
						qct.ID_DoiTuong,
						sum(qct.TienThu) as GiaTriHoanTraGDV
					 from
					 (
						 select 
							hd.ID					
						 from BH_HoaDon hd
						 join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon',
						 @whereInvoice,
						' and ct.ChatLieu = ''1''  and hd.LoaiHoaDon = 6
						  and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.id)
						 group by hd.ID
					 )hdTra
					 join Quy_HoaDon_ChiTiet qct on hdTra.ID = qct.ID_HoaDonLienQuan
					 join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
					 where qhd.TrangThai = 1
					 group by qct.ID_DoiTuong
				) traGDV on dt.ID = traGDV.ID_DoiTuong

				left join 
				(			
				 	 ----- giatri sudung DV: sudung buoi le/ sudung tu GDV ---
				  select 
						tbl.ID_DoiTuong,
						sum(SuDungGDV) as SuDungGDV,
						sum(isnull(SuDungHDLe,0)) as SuDungHDLe,
						sum(SuDungGDV) + sum(isnull(SuDungHDLe,0)) as GiaTriSuDung
					 from
					 (
					 select 
								hd.ID_DoiTuong,					
								iif(ctsd.ChatLieu =5,0,
									iif(gdv.ID is not null, 
										---- sudung GDV --
										ctsd.SoLuong * (ctm.DonGia - ctm.TienChietKhau) * (1 -  gdv.TongGiamGia/iif(gdv.TongTienHang =0,1,gdv.TongTienHang)),
										0)) as SuDungGDV,
								iif(ctsd.ChatLieu =5,0,
									iif(gdv.ID is null, 
										---- sudung hdle --
										iif(hd.TongTienHang =0,ctsd.ThanhTien,
											ctsd.ThanhTien * (1- hd.TongGiamGia/hd.TongTienHang)),							
										0)) as SuDungHDLe

							 from BH_HoaDon hd
							 join BH_HoaDon_ChiTiet ctsd on hd.ID = ctsd.ID_HoaDon and hd.LoaiHoaDon= 1
							 left join BH_HoaDon_ChiTiet ctm on ctsd.ID_ChiTietGoiDV = ctm.ID 
								and (ctsd.ID_ChiTietDinhLuong is null or ctsd.ID_ChiTietDinhLuong = ctsd.id) ----- khong lay tpdinhluonh
							 left join BH_HoaDon gdv on ctm.ID_HoaDon = gdv.ID and gdv.LoaiHoaDon = 19',
							 @whereInvoice,
					N' )tbl group by tbl.ID_DoiTuong				 
				) tblSuDung on dt.ID = tblSuDung.ID_DoiTuong

    			left join
    			(
    			select 
    				 tblThuChi.ID_DoiTuong,
					 -----NapTienTGT,DieuChinhSoDuTGT:  2 trường này dùng để tính số tiền còn lại chưa dùng ---
					 SUM(isnull(tblThuChi.NapTienTGT,0)) as NapTienTGT,	
					 SUM(isnull(tblThuChi.SuDungTGT,0)) as SuDungTGT,
					  SUM(isnull(tblThuChi.ThanhToanGDV,0)) as ThanhToanGDV,
					  SUM(ISNULL(tblThuChi.HoanTraThe,0)) as  HoanTraThe,
					   SUM(ISNULL(tblThuChi.ChiTuGDV,0) )as ChiTuGDV,
					 SUM(isnull(tblThuChi.TienKhach_biGiamTru,0)) as TienKhach_biGiamTru,
					 SUM(isnull(tblThuChi.DieuChinhSoDuTGT,0)) as DieuChinhSoDuTGT,
					 SUM(ISNULL(tblThuChi.ThuHoaDon,0)) as TongThuKhachHang,
					 SUM(ISNULL(tblThuChi.ChiTuGDV,0)) - SUM(ISNULL(tblThuChi.HoanTraThe,0)) as TongChiKhachHang,
    				 SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.HoanTraThe,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) 
							+ sum(ISNULL(tblThuChi.DoanhThuThe,0))
    						- sum(isnull(tblThuChi.PhiDichVu,0)) 
    						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
    				sum(ISNULL(tblThuChi.DoanhThuThe,0)) as DoanhThuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.TraHangGDV,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang,
    				sum(isnull(tblThuChi.PhiDichVu,0)) as PhiDichVu,
					sum(isnull(tblThuChi.NapCoc,0)) as NapCoc,
					sum(isnull(tblThuChi.SuDungCoc,0)) as SuDungCoc,
					sum(isnull(tblThuChi.NapCoc,0)) -sum(isnull(tblThuChi.SuDungCoc,0))  as SoDuCoc ')
    		set @sql3=concat( N' from
    			(
    				---- chiphi dv ncc ----
    				select 
    					cp.ID_NhaCungCap as ID_DoiTuong,
    					0 as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as NapTienTGT,
						0 as SuDungTGT, 
						0 as ThanhToanGDV, 
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					0 as DoanhThuThe,
    					sum(cp.ThanhTien) as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				from BH_HoaDon_ChiPhi cp
    				join BH_HoaDon hd on cp.ID_HoaDon= hd.ID
    				', @whereChiNhanh,
    				N' and hd.ChoThanhToan = 0',
    				 N' group by cp.ID_NhaCungCap

					 union all
					
					 ---- hoantra sodu TGT cho khach (giam sodu TGT)

					SELECT 
    						bhd.ID_DoiTuong,    	
							0 as GiaTriTra,
							0 as TraHangGDV,
							0 as TienKhach_biGiamTru,
    						0 as DoanhThu,
							0 AS TienThu,
    						0 AS TienChi, 
							0 as ThuHoaDon,
							0 as NapTienTGT,
							0 as SuDungTGT, 
							0 as ThanhToanGDV, 
							0 as ChiTuGDV,
    						0 AS SoLanMuaHang,
							0 as DoanhThuThe,
							0 as PhiDichVu,
							----- Loai = 23: (TongTienHang: chênh lệch giữa số dư cũ và số dư sau khi điều chỉnh (+/-) ---
							----- neu loai = 32: (TongGiamGia: chi phí hoàn trả: không ảnh hưởng đến số sư thẻ ----
							----- PhaiThanhToan: số tiền phải thanh toán sau khi trừ phí)
							----- lấy dấu âm (-TongGiamGia): vì TongChiKhachHang = - sum(HoanTraThe): trừ trừ thành cộng
							sum(iif(LoaiHoaDon = 23, bhd.TongTienHang, -bhd.TongGiamGia)) as DieuChinhSoDuTGT, 
							-sum(iif(LoaiHoaDon = 32, bhd.PhaiThanhToan,0)) as HoanTraThe,
							0 as NapCoc,
							0 as SuDungCoc
    				FROM BH_HoaDon bhd ',
					 @whereChiNhanh,
					 @whereNgayLapHD,
					N' and bhd.LoaiHoaDon in (23,32) and bhd.ChoThanhToan = 0 
					and exists (select * from @tblChiNhanh cn where bhd.ID_DonVi= cn.ID)
					group by bhd.ID_DoiTuong
    
    				union all
    				----- tongban ----
    				SELECT 
    					hd.ID_DoiTuong,    	
    					0 as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					hd.PhaiThanhToan as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as NapTienTGT,
						0 as SuDungTGT, 
						0 as ThanhToanGDV, 
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					0 as DoanhThuThe,
    					0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ', @whereInvoice, N'  and hd.LoaiHoaDon in (1,7,19,25) ',
    
    				N' union all
    				---- doanhthu tuthe
    				SELECT 
    					hd.ID_DoiTuong,    	
    					0 as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as NapTienTGT,
						0 as SuDungTGT, 
						0 as ThanhToanGDV, 
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					iif(hd.LoaiHoaDon = 42, - hd.PhaiThanhToan, hd.PhaiThanhToan) as DoanhThuThe,
    					0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ', @whereInvoice , N' and hd.LoaiHoaDon in (22,42) ', 
    
    					N' union all
    				------ gia tri trả từ bán hàng + gdv ----
    				SELECT 
    					hd.ID_DoiTuong,    	
    					hd.PhaiThanhToan as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as NapTienTGT,
						0 as SuDungTGT, 
						0 as ThanhToanGDV, 
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					0 as DoanhThuThe,
    					0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ',  @whereInvoice, N'  and hd.LoaiHoaDon in (6,4) ',


					------ get giatri trahang tu GDV ----> tính vào Tổng bán trừ Trả hàng

					N' union all 
						 select 
							hd.ID_DoiTuong,    	
							0 as GiaTriTra,
							----- chiết khấu hàng trả: không ảnh hưởng đến giá trị GDV mua ban đầu --
    						sum(ct.DonGia * ct.SoLuong) as TraHangGDV,
							---- nhưng, khách vẫn bị giảm trừ tiền, cửa hàng được thêm tiền thôi + 	 chiphi trahàng ---
							sum(ct.SoLuong * ct.TienChietKhau) + 
								sum(iif(hd.TongTienHang = 0 or hd.TongChiPhi = 0, 0, ct.ThanhTien * hd.TongChiPhi/hd.TongTienHang)) as TienKhach_biGiamTru,
    						0 as DoanhThu,
    						0 AS TienThu,
    						0 AS TienChi, 
							0 as ThuHoaDon,
							0 as NapTienTGT,
							0 as SuDungTGT, 
							0 as ThanhToanGDV, 
							0 as ChiTuGDV,						
    						0 AS SoLanMuaHang,
    						0 as DoanhThuThe,
    						0 as PhiDichVu,
							0 as DieuChinhSoDuTGT,
							0 as HoanTraThe,
							0 as NapCoc,
							0 as SuDungCoc				
						 from BH_HoaDon hd
						 join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon',
						 @whereInvoice,
						' and ct.ChatLieu = ''2''  and hd.LoaiHoaDon = 6
						  and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.id)
						 group by hd.ID_DoiTuong

					',					
    				
    				N' union all
    				----- tienthu/chi ---
    				SELECT 
    				qct.ID_DoiTuong,						
    				0 AS GiaTriTra,
					0 as TraHangGDV,
					0 as TienKhach_biGiamTru,
    				0 AS DoanhThu,
    				iif(qhd.LoaiHoaDon=11,qct.TienThu,0) AS TienThu,
    				iif(qhd.LoaiHoaDon=12,qct.TienThu,0) AS TienChi,
					------ThuHoaDon: khônglấy tiền thu/chi từ TGT ----
					iif(qhd.LoaiHoaDon=11, iif(qct.HinhThucThanhToan = 4,0, qct.TienThu),0) as ThuHoaDon,
					iif(hd.LoaiHoaDon= 22, iif(qhd.LoaiHoaDon=11, qct.TienThu, 0),0) as NapTienTGT,
					iif(qhd.LoaiHoaDon= 11, iif(qct.HinhThucThanhToan = 4,qct.TienThu, 0),0) as SuDungTGT,
					iif(hd.LoaiHoaDon= 19, iif(qhd.LoaiHoaDon=11, qct.TienThu, 0),0) as ThanhToanGDV,
					0 as ChiTuGDV,
    				0 AS SoLanMuaHang,
    				0 as DoanhThuThe,
    				0 as PhiDichVu,
					0 as DieuChinhSoDuTGT,
					0 as HoanTraThe,
					0 as NapCoc,
					0 as SuDungCoc
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qct ON qhd.ID = qct.ID_HoaDon 
				join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID',
    				@whereInvoice, 
    				N' and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
					and qct.HinhThucThanhToan not in (6) ----- khong lấy phiếu chi nạp cọc ---
    				and (qct.LoaiThanhToan is null or qct.LoaiThanhToan != 3) ',

					------ hoancoc: chỉ lấy tiền hoàn lại khi mua GDV/hoặc hoàn cọc TGT ----
					N' union all
    				----- tienthu/chi ---
    				SELECT 
    				qct.ID_DoiTuong,						
    				0 AS GiaTriTra,
					0 as TraHangGDV,
					0 as TienKhach_biGiamTru,
    				0 AS DoanhThu,
					0 as TienThu,
					0 as TienChi,
					0 as ThuHoaDon,			
					0 as NapTienTGT,
					0 as SuDungTGT, 
					0 as ThanhToanGDV, 
					iif(qct.ID_HoaDonLienQuan is null,0, 
						iif(qhd.LoaiHoaDon=12,iif(qct.HinhThucThanhToan = 4,0, qct.TienThu),0)) as ChiTuGDV,
    				0 AS SoLanMuaHang,
    				0 as DoanhThuThe,
    				0 as PhiDichVu,
					0 as DieuChinhSoDuTGT,
					0 as HoanTraThe,
					0 as NapCoc,
					0 as SuDungCoc
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qct ON qhd.ID = qct.ID_HoaDon',
    				@whereChiNhanh, 
					@whereNgayLapHD,
    				N' and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
					and qct.HinhThucThanhToan not in (6) ----- khong lấy phiếu chi nạp cọc ---
    				and (qct.LoaiThanhToan is null or qct.LoaiThanhToan != 3) 
					and exists (select hdTra.id from BH_HoaDon hdTra 
						join BH_HoaDon_ChiTiet ctTra on hdTra.ID = ctTra.ID_HoaDon
						where hdTra.LoaiHoaDon = 6
						and ctTra.ChatLieu = ''2''
						and qct.ID_HoaDonLienQuan =  hdTra.ID) ',
    				
					---- NapCoc NCC----	

					N' union all
					
    				SELECT 
    					qhdct.ID_DoiTuong,						
    					0 AS GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as ThuHoaDon,
						0 as NapTienTGT,
						0 as SuDungTGT, 
						0 as ThanhToanGDV, 
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
						0 as DoanhThuThe,
						0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu) as NapCocNCC,
						0 as SuDungCoc
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon ',
					@whereChiNhanh, 
    				N' and (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
					and qhdct.LoaiThanhToan = 1',

					---- sudungcoc ----
						' union all
									
    				SELECT 
    					qhdct.ID_DoiTuong,						
    					0 AS GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as ThuHoaDon,
						0 as NapTienTGT,
						0 as SuDungTGT, 
						0 as ThanhToanGDV, 
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
						0 as DoanhThuThe,
						0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu) as SuDungCoc
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon ',
					@whereChiNhanh, 
    				N' and (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
					and qhdct.HinhThucThanhToan = 6 ',					       				
    			N')tblThuChi 
    			GROUP BY tblThuChi.ID_DoiTuong
    		) a on dt.ID= a.ID_DoiTuong 
    		) tbl ', @Where ,
    	'), 
    	count_cte
    	as
    	(
	    		SELECT COUNT(ID) AS TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize_In as float)) as TotalPage,
					SUM(TongBan) as TongBanAll,
    				SUM(TongBanTruTraHang) as TongBanTruTraHangAll,
    				SUM(TongTichDiem) as TongTichDiemAll,
    				SUM(NoHienTai) as NoHienTaiAll,
    				SUM(PhiDichVu) as TongPhiDichVu,
					SUM(TongThuKhachHang) as SumTongThuKhachHang,
					SUM(TongChiKhachHang) as SumTongChiKhachHang,
					SUM(GiaTriDVHoanTra) as SumGiaTriDVHoanTra,
					SUM(GiaTriDVSuDung) as SumGiaTriDVSuDung,
					SUM(SoTienChuaSD) as SumSoTienChuaSD
    		from data_cte
    	),
    	tView
    	as (
    	select *		
    	from data_cte dt
    	cross join count_cte cte
    	ORDER BY ', @ColumnSort, ' ', @SortBy,
    	N' offset (@CurrentPage_In * @PageSize_In) ROWS
    	fetch next @PageSize_In ROWS only
    	)
    	select dt.*,
    		 ISNULL(trangthai.TenTrangThai,'''') as TrangThaiKhachHang,
    	ISNULL(qh.TenQuanHuyen,'''') as PhuongXa,
    	ISNULL(tt.TenTinhThanh,'''') as KhuVuc,
    	ISNULL(dv.TenDonVi,'''') as ConTy,
    	ISNULL(dv.SoDienThoai,'''') as DienThoaiChiNhanh,
    	ISNULL(dv.DiaChi,'''') as DiaChiChiNhanh,
    	ISNULL(nk.TenNguonKhach,'''') as TenNguonKhach,
    	ISNULL(dt2.TenDoiTuong,'''') as NguoiGioiThieu,
    		ISNULL(nvpt.MaNhanVien,'''') as MaNVPhuTrach,
    		ISNULL(nvpt.TenNhanVien,'''') as TenNhanVienPhuTrach
    	from tView dt
    	left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    	LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    	LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
    	LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
    	LEFT join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID
    	LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    	LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID
    	')
    
    		set @sql = CONCAT(@tblDefined, @sql1, @sql2, @sql3)
    
    		set @paramDefined = N'@IDChiNhanhs_In nvarchar(max),
    								@LoaiDoiTuong_In int ,
    								@IDNhomKhachs_In nvarchar(max),
    								@TongBan_FromDate_In datetime,
    								@TongBan_ToDate_In datetime,
    								@NgayTao_FromDate_In datetime,
    								@NgayTao_ToDate_In datetime,
    								@TextSearch_In nvarchar(max),
    								@Where_In nvarchar(max) ,							
    								@ColumnSort_In varchar(40),
    								@SortBy_In varchar(40),
    								@CurrentPage_In int,
    								@PageSize_In int'
    
    		print @sql
    		print @sql2
    		print @sql3
    
    
    		exec sp_executesql @sql, @paramDefined, 
    					@IDChiNhanhs_In = @IDChiNhanhs,
    					@LoaiDoiTuong_In= @LoaiDoiTuong,
    					@IDNhomKhachs_In= @IDNhomKhachs,
    					@TongBan_FromDate_In= @TongBan_FromDate,
    					@TongBan_ToDate_In =@TongBan_ToDate,
    					@NgayTao_FromDate_In =@NgayTao_FromDate ,
    					@NgayTao_ToDate_In = @NgayTao_ToDate,
    					@TextSearch_In = @TextSearch,
    					@Where_In = @Where ,
    					@ColumnSort_In = @ColumnSort,
    					@SortBy_In = @SortBy,
    					@CurrentPage_In = @CurrentPage,
    					@PageSize_In = @PageSize
END

--LoadDanhMuc_KhachHangNhaCungCap");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateChiTietKiemKe_WhenEditCTHD]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanhInput [uniqueidentifier],
    @NgayLapHDMin [datetime]
AS
BEGIN
    SET NOCOUNT ON;
	
	----declare @IDHoaDonInput uniqueidentifier, @IDChiNhanhInput uniqueidentifier,@NgayLapHDMin  datetime 
	----select top 1 @IDHoaDonInput = ID, @IDChiNhanhInput = ID_DonVi,
	----@NgayLapHDMin = NgayLapHoaDon
	----from BH_HoaDon where MaHoaDon='PKK0000000001'
  
		 ------- get all donviquydoi lienquan ---
			declare @tblQuyDoi table (ID_DonViQuiDoi uniqueidentifier, ID_HangHoa uniqueidentifier, 
				ID_LoHang uniqueidentifier, 
				TyLeChuyenDoi float,
				LaHangHoa bit)
			insert into @tblQuyDoi
			select * from dbo.fnGetAllHangHoa_NeedUpdateTonKhoGiaVon(@IDHoaDonInput)

			------ get all ctKiemKe need update ---
			select 
				hd.ID as ID_HoaDon,
				ct.ID as ID_ChiTietHoaDon,
				hd.NgayLapHoaDon,
				ct.SoLuong,
				qd.ID_HangHoa,
				ct.ID_LoHang,
				ct.ID_DonViQuiDoi,
				0 as TonDauKy,
				qd.TyLeChuyenDoi
			into #ctNeed
			from BH_HoaDon hd
			join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
			join  @tblQuyDoi qd on ct.ID_DonViQuiDoi= qd.ID_DonViQuiDoi			
				and (ct.ID_LoHang = qd.ID_LoHang or ct.ID_LoHang is null and qd.ID_LoHang is null)
    		WHERE hd.ChoThanhToan = 0 
			AND hd.LoaiHoaDon = 9 
    		and hd.ID_DonVi = @IDChiNhanhInput 
			and hd.NgayLapHoaDon >= @NgayLapHDMin



				----- get tonLuyKe all cthd LienQuan ----			
					select 
						ID_ChiTietHoaDon,
						MaHoaDon,
						ID_LoHang,
						ID_HangHoa,
						TonLuyKe,
						NgayLapHoaDon
					into #cthdLienQuan
					from
					(
					select 
						ct.ID as ID_ChiTietHoaDon,							
						ct.ID_HoaDon,							
						ct.ID_LoHang,
						qd.ID_HangHoa,			
						hd.MaHoaDon,
						CASE WHEN @IDChiNhanhInput = hd.ID_CheckIn and hd.YeuCau = '4' then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon,
						CASE WHEN @IDChiNhanhInput = hd.ID_CheckIn and hd.YeuCau = '4' THEN ct.TonLuyKe_NhanChuyenHang ELSE ct.TonLuyKe END as TonLuyKe
					from BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd  ON ct.ID_HoaDon = hd.ID  		
					join @tblQuyDoi qd on  ct.ID_DonViQuiDoi = qd.ID_DonViQuiDoi
						and (qd.ID_LoHang = ct.ID_LoHang or (qd.ID_LoHang is null and ct.ID_LoHang is null))
    				WHERE hd.ChoThanhToan = 0    		
						and hd.LoaiHoaDon NOT IN (3, 19, 25,29)					
						and exists (select ctNeed.ID_DonViQuiDoi 
								from #ctNeed ctNeed 
								where ctNeed.ID_HangHoa = qd.ID_HangHoa 
								and (ctNeed.ID_LoHang = ct.ID_LoHang or (ctNeed.ID_LoHang is null and ct.ID_LoHang is null))
								------ chỉ lấy những hóa đơn có ngày lập < ngày kiểm kê (có thể có nhiều khoảng ngày kiểm kê )---
								AND ((hd.ID_DonVi = @IDChiNhanhInput and hd.NgayLapHoaDon <  ctNeed.NgayLapHoaDon and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    							or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanhInput and  hd.NgaySua < ctNeed.NgayLapHoaDon ))
								)
					)cthdLienQuan
		
			
			

			------ sắp xếp theo thứ tự ngày lập ---> get TonDauKy của mỗi hóa đơn ---
			select 
					ID_ChiTietHoaDon,
					ID_LoHang,
					ID_HangHoa,
					TonLuyKe,
					NgayLapHoaDon,					
					ROW_NUMBER() over (partition by ID_HangHoa, ID_LoHang order by NgayLapHoaDon ) as RN
			into #tblNew
			from
			(
					select 
						ID_ChiTietHoaDon,
							ID_LoHang,
							ID_HangHoa,
							TonLuyKe,
							NgayLapHoaDon
					from #cthdLienQuan

					UNION ALL

					select 
						ID_ChiTietHoaDon,
							ID_LoHang,
							ID_HangHoa,
							TonDauKy,
							NgayLapHoaDon
					from #ctNeed
			)tbl
		
		------- update TonDauKy for each cthd ----
		update t1 set t1.TonLuyKe = isnull(t2.TonLuyKe,0)
		from #tblNew t1
		left join #tblNew t2 on t1.ID_HangHoa = t2.ID_HangHoa
			and (t1.ID_LoHang = t2.ID_LoHang or t1.ID_LoHang is null and t2.ID_LoHang is null)
			and t1.RN = t2.RN + 1
					


			---------- update TonkhoDB, SoLuongLech, GiaTriLech to BH_HoaDon_ChiTiet----
			update ctkiemke
			set	ctkiemke.TienChietKhau = ctNeed.TonDauKy, 
    			ctkiemke.SoLuong = ctkiemke.ThanhTien - ctNeed.TonDauKy, ---- soluonglech
    			ctkiemke.ThanhToan = ctkiemke.GiaVon * (ctkiemke.ThanhTien - ctNeed.TonDauKy) --- gtrilech = soluonglech * giavon
			from BH_HoaDon_ChiTiet ctkiemke
			join (
					------- vì TonLuyKe đang tính theo dvqd ---> TonDauKy cũng lấy theo dvqd --
				select ctNeed.ID_ChiTietHoaDon, 					
					ctNew.TonLuyKe/ ctNeed.TyLeChuyenDoi as TonDauKy
				from #ctNeed ctNeed
				join #tblNew ctNew on ctNeed.ID_ChiTietHoaDon = ctNew.ID_ChiTietHoaDon							
			) ctNeed on ctkiemke.ID = ctNeed.ID_ChiTietHoaDon


			------------- update TongChenhLech for BH_HoaDon ----
			-------- TongGiamGia: sum(SoLuonglech),
			-------- TongTienThue = sum(GiaTriLech) ---
			-------- TongChiPhi: Tổng số lượng lệch tăng = sum (SoLuong - chỉ lấy SoLuong > 0)
			-------- TongTienHang: Tổng số lượng lệch giảm = sum (SoLuong - chỉ lấy SoLuong < 0)

			update hdKK set 
				hdKK.TongGiamGia = ctKK.SoLuongLech,
				hdKK.TongTienThue = ctKK.GiaTriLech,
				hdKK.TongChiPhi = ctKK.SoLuongLechTang,
				hdKK.TongTienHang = ctKK.SoLuongLechGiam
			from BH_HoaDon hdKK
			join
			(
				select 
					ct.ID_HoaDon,
					sum(ct.SoLuong) as SoLuongLech,
					sum(iif(ct.SoLuong >0, ct.SoLuong,0)) as SoLuongLechTang,
					sum(iif(ct.SoLuong < 0, ct.SoLuong,0)) as SoLuongLechGiam,
					sum(ct.ThanhToan) as GiaTriLech
				from BH_HoaDon_ChiTiet ct
				where exists (select ctNeed.ID_HoaDon from #ctNeed ctNeed where ctNeed.ID_ChiTietHoaDon = ct.ID)
				group by ct.ID_HoaDon
			)ctKK on hdKK.ID = ctKK.ID_HoaDon		
			
			drop table #ctNeed		
			drop table #cthdLienQuan
			drop table #tblNew

END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateGiaVon_WhenEditCTHD]
   @IDHoaDonInput [uniqueidentifier],
   @IDChiNhanh [uniqueidentifier],
   @NgayLapHDMin [datetime] ----  @NgayLapHDMin: đang lấy ngày lập hóa đơn cũ
AS
BEGIN
    SET NOCOUNT ON;	
		----declare @IDHoaDonInput uniqueidentifier ='35367DCE-98B0-437F-A95F-7A0FF9BEAC08',
		----@IDChiNhanh uniqueidentifier ='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE',
		----@NgayLapHDMin  datetime ='2024-03-14 11:28:00.000'


			declare @NgayLapHDNew DATETIME, @NgayLapHDMinNew DATETIME, @LoaiHoaDonThis int
			select 
    			@NgayLapHDNew = NgayLapHoaDon,
				@LoaiHoaDonThis = LoaiHoaDon
    		from (
    				select
						LoaiHoaDon,
						case when YeuCau = '4' AND @IDChiNhanh = ID_CheckIn then NgaySua else NgayLapHoaDon end as NgayLapHoaDon
    				from BH_HoaDon where ID = @IDHoaDonInput
				) hdupdate
		
			----- nếu ngày cũ > ngày mới: lấy ngày mới ---
			IF(@NgayLapHDMin > @NgayLapHDNew)
    			SET @NgayLapHDMinNew = @NgayLapHDNew;
    		ELSE
				---- else: lấy ngày cũ ---
    			SET @NgayLapHDMinNew = @NgayLapHDMin;

	
		DECLARE @TinhGiaVonTrungBinh BIT =(SELECT top 1 GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanh)  	
		---- khong update giavon cho LoaiHD: 3,19,25 --- vì giá vốn của các loại này = sum(hd sử dụng, hdle, hdxuatkho) ---
		------> giảm bớt số lần gọi 
		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1  and @LoaiHoaDonThis not in (3,19, 25))
		BEGIN ----- beginOut

				declare @NgayLapHDMin_SubMiliSecond datetime = dateadd(MILLISECOND,-3, @NgayLapHDMinNew)

				 ----- get all donviquydoi lienquan ---
				declare @tblQuyDoi table (ID_DonViQuiDoi uniqueidentifier, 
					ID_HangHoa uniqueidentifier, 
					ID_LoHang uniqueidentifier, 
					TyLeChuyenDoi float,
					LaHangHoa bit)
				insert into @tblQuyDoi
				select * from dbo.fnGetAllHangHoa_NeedUpdateTonKhoGiaVon(@IDHoaDonInput)
			

    		------ get @cthd_NeedUpGiaVon (ngayLap >= ngaylapMin): từ hóa đơn hiện tại trở về sau ----
    		DECLARE @cthd_NeedUpGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonGoc UNIQUEIDENTIFIER, 
					MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ChoThanhToan bit,
					ID_ChiTietHoaDon UNIQUEIDENTIFIER,
					NgayLapHoaDon DATETIME,
					SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
					TongTienHang FLOAT, TongChiPhi FLOAT,
    				ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT,
					TonKho FLOAT,  GiaVon FLOAT, GiaVonNhan FLOAT,
					ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, 
					IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    				ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX),
					GiaVonDauKy float,
					ChatLieu varchar(100),
					IdHoaDonTruocDo UNIQUEIDENTIFIER,
					RN int)
    	INSERT INTO @cthd_NeedUpGiaVon  	
		select ctNeed.*,
			------ rn: get sau khi lay ngaylap/ngaysua --
			ROW_NUMBER() over (partition by ID_HangHoa, ID_LoHang order by NgayLapHoaDon) as RN
		from
		(
		select hd.ID as IDHoaDon,
			hd.ID_HoaDon as IDHoaDonGoc, 
			hd.MaHoaDon, 
			hd.LoaiHoaDon,
			hd.ChoThanhToan,
			ct.ID as ID_ChiTietHoaDon, 
    		CASE WHEN hd.YeuCau = '4' AND @IDChiNhanh = hd.ID_CheckIn THEN hd.NgaySua ELSE hd.NgayLapHoaDon END AS NgayLapHoaDon, 				    			    				    							    			
    		ct.SoThuTu,
			iif(ct.ChatLieu='5' or hd.ChoThanhToan is null or hd.ChoThanhToan ='1' ,0,ct.SoLuong) as SoLuong, 
			ct.DonGia, 
			iif(hd.ChoThanhToan is null or hd.ChoThanhToan ='1',0, hd.TongTienHang) as TongTienHang,
			isnull(hd.TongChiPhi,0) as TongChiPhi,
			iif(ct.ChatLieu='5' or hd.ChoThanhToan is null or hd.ChoThanhToan ='1' ,0,ct.TienChietKhau) as TienChietKhau,
			iif(ct.ChatLieu='5' or hd.ChoThanhToan is null or hd.ChoThanhToan ='1' ,0,ct.ThanhTien) as ThanhTien,
			hd.TongGiamGia, 
			qd.TyLeChuyenDoi,
			0 as TonKho, ---- tạm thời gán = 0, và get lại TonKho đầu kỳ sau --- 	    	
    		iif(hd.ChoThanhToan is null,0,ct.GiaVon / qd.TyLeChuyenDoi) as GiaVon, 
    		iif(hd.ChoThanhToan is null,0,ct.GiaVon_NhanChuyenHang / qd.TyLeChuyenDoi) as GiaVon_NhanChuyenHang,
    		qd.ID_HangHoa, 
			qd.LaHangHoa, 
			qd.ID_DonViQuiDoi, 
			ct.ID_LoHang, 
			ct.ID_ChiTietDinhLuong, 
    		@IDChiNhanh as IDChiNhanh,
			hd.ID_CheckIn,
			hd.YeuCau,
			0 as GiaVonDauKy,
			isnull(ct.ChatLieu,'') as ChatLieu,
			hd.ID as IdHoaDonTruocDo			
    	FROM BH_HoaDon_ChiTiet ct 
		join @tblQuyDoi qd ON ct.ID_DonViQuiDoi = qd.ID_DonViQuiDoi and (ct.ID_LoHang = qd.ID_LoHang OR qd.ID_LoHang IS NULL and ct.ID_LoHang is null)
    	INNER JOIN BH_HoaDon hd  ON ct.ID_HoaDon = hd.ID   
			----- chỉ update GiaVon cho hdCurrent/ hoặc hóa đơn chưa hủy có >= ngayLapHoaDon of hdCurrent
    	WHERE hd.ID = @IDHoaDonInput
		or (hd.ChoThanhToan= 0
			and hd.id != @IDHoaDonInput
			and hd.LoaiHoaDon NOT IN (3, 19, 29,25)
				------- yeuCau = 3 & LoaiHD = 10: phiếu chuyển hàng bị hủy --
			and	((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon >= @NgayLapHDMinNew
    				and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    				or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua >= @NgayLapHDMinNew)))			
		)ctNeed

	

				
				------ tonluyke trước đó của từng hóa đơn ----					
				select 					
					tblTonLuyKe.ID_LoHang,
					tblTonLuyKe.ID_HangHoa,
					tblTonLuyKe.TonLuyKe,
					tblTonLuyKe.NgayLapHoaDon
					into #tblTonLuyKe
				from
				(
				
					select ct.ID_HoaDon,					
						ct.ID_LoHang,
						qd.ID_HangHoa,		
						hd.MahoaDon,					
						CASE WHEN @IDChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon,
						CASE WHEN @IDChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' THEN ct.TonLuyKe_NhanChuyenHang ELSE ct.TonLuyKe END as TonLuyKe
					from BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd  ON ct.ID_HoaDon = hd.ID  
					join @tblQuyDoi qd on ct.ID_DonViQuiDoi = qd.ID_DonViQuiDoi
					and (qd.ID_LoHang = ct.ID_LoHang or (qd.ID_LoHang is null and ct.ID_LoHang is null))
    				WHERE hd.ChoThanhToan = 0    		
						and hd.LoaiHoaDon NOT IN (3, 19, 25,29)					
						and exists (select ctNeed.IDDonViQuiDoi 
								from @cthd_NeedUpGiaVon ctNeed 
								where ctNeed.ID_HangHoa = qd.ID_HangHoa 
								and (ctNeed.ID_LoHang = ct.ID_LoHang or (ctNeed.ID_LoHang is null and ct.ID_LoHang is null))
								------ !important: so sánh ngày lập --> để lấy TonLuyke ---
								AND ((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon <  ctNeed.NgayLapHoaDon and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    							or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and  hd.NgaySua < ctNeed.NgayLapHoaDon ))
								)					

					)tblTonLuyKe


			------- nếu @ctNeed có chứa loại = 6, và idHoaDonGoc is not null ----
			------- get all cthd Mua gốc ban đầu (theo hàng hóa)---> để lấy giá vốn lúc mua ---
			declare @tblCTMuaGoc table (ID_HDMuaGoc uniqueidentifier, NgayLapHoaDon datetime, LoaiHoaDon int,
				ID_HangHoa uniqueidentifier, ID_LoHang uniqueidentifier, GiaVon float)

				select ID_ChiTietHoaDon, IDHoaDon,NgayLapHoaDon,
					ID_HangHoa, IDDonViQuiDoi, ID_LoHang
				into #ctTraHang
				from @cthd_NeedUpGiaVon
					where LoaiHoaDon = 6
					and IDHoaDonGoc is not null
				declare @countTraHang  int = (select count(ID_ChiTietHoaDon) from #ctTraHang)
				
			if @countTraHang > 0
				begin
					------- vì HDSC xuất kho nhiều lần: nên nếu Trả hàng từ HDSC --> không thể tính chính xác giá vốn lúc xuất kho ---
					------ (giá vốn dc tính ở trường hợp này chỉ mang tính chất tham khảo: đang lấy GV trước thời điểm tạo HDSC) --
					insert into @tblCTMuaGoc
					select 
						hd.ID,
						CASE WHEN @IDChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon,
						hd.LoaiHoaDon,
						qd.ID_HangHoa,
						ct.ID_LoHang,									
						CASE WHEN @IDChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' then ct.GiaVon_NhanChuyenHang / qd.TyLeChuyenDoi					
    						else ct.GiaVon / qd.TyLeChuyenDoi end as GiaVon
					from BH_HoaDon hd
					join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
					join @tblQuyDoi qd on ct.ID_DonViQuiDoi = qd.ID_DonViQuiDoi
					and (qd.ID_LoHang = ct.ID_LoHang or (qd.ID_LoHang is null and ct.ID_LoHang is null))
					where hd.ChoThanhToan='0'
					and exists (select ctNeed.ID_HangHoa 
								from #ctTraHang ctNeed 
								where ctNeed.ID_HangHoa = qd.ID_HangHoa
								and (ctNeed.ID_LoHang = ct.ID_LoHang or (ctNeed.ID_LoHang is null and ct.ID_LoHang is null))
								AND ((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon <  ctNeed.NgayLapHoaDon and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    							or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and  hd.NgaySua < ctNeed.NgayLapHoaDon ))
								)	 
				end


				

			------ get giavondauky ----
			select 
				gvDK.ID_HangHoa,
				gvDK.ID_LoHang,
				gvDK.GiaVonDauKy
			into #tblGVDauKy
			from
			(
				select 
					tblGV.ID_HangHoa,
					tblGV.ID_LoHang,
					tblGV.GiaVonDauKy,
					ROW_NUMBER() over (partition by tblGV.ID_HangHoa, tblGV.ID_LoHang order by tblGV.NgayLapHoaDon desc) as RN
				from
				(
				select   					
						ct.ID_LoHang,
						qd.ID_HangHoa,				
						CASE WHEN @IDChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon,
						CASE WHEN @IDChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' THEN ct.GiaVon_NhanChuyenHang/qd.TyLeChuyenDoi ELSE ct.GiaVon/qd.TyLeChuyenDoi END as GiaVonDauKy
				FROM BH_HoaDon hd
    			INNER JOIN BH_HoaDon_ChiTiet ct ON hd.ID = ct.ID_HoaDon  
				join @tblQuyDoi qd ON ct.ID_DonViQuiDoi = qd.ID_DonViQuiDoi and (ct.ID_LoHang = qd.ID_LoHang OR qd.ID_LoHang IS NULL and ct.ID_LoHang is null)    		
    			WHERE hd.ChoThanhToan = 0 
					and hd.LoaiHoaDon NOT IN (3, 19, 25,29)
    					AND ((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon < @NgayLapHDMinNew and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    						or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua < @NgayLapHDMinNew))

				) tblGV
			) gvDK where gvDK.RN= 1

		--select * from #tblGVDauKy

			----- update again TonLuyKe for each ctNeedUpdate ----		
				update ctNeed set ctNeed.TonKho = cthd.TonDauKy
				from @cthd_NeedUpGiaVon ctNeed
				join
				(
					select 
						cthdIn.ID_ChiTietHoaDon,
						cthdIn.TonDauKy
					from
					(
					select ctNeed.ID_ChiTietHoaDon, ctNeed.MaHoaDon,
						ctNeed.ID_LoHang,
						ctNeed.ID_HangHoa,
						ctNeed.NgayLapHoaDon,
						ctNeed.IDHoaDon,				
						isnull(tkDK.TonLuyKe,0) as TonDauKy,
						----- Lấy tồn đầu kỳ của từng chi tiết hóa đơn, ưu tiên sắp xếp theo tkDK.NgayLapHoaDon gần nhất (max) ---
						----- vì có thể có nhiều hd < ngaylaphoadon of ctNeed ----
						ROW_NUMBER() over (partition by ctNeed.ID_ChiTietHoaDon order by tkDK.NgayLapHoaDon desc) as RN
					from @cthd_NeedUpGiaVon ctNeed
					left join #tblTonLuyKe tkDK on ctNeed.ID_HangHoa = tkDK.ID_HangHoa 
						and (ctNeed.ID_LoHang = tkDK.ID_LoHang or (ctNeed.ID_LoHang is null and tkDK.ID_LoHang is null)) 
						and tkDK.NgayLapHoaDon < ctNeed.NgayLapHoaDon
					)cthdIn
					where rn = 1
				)cthd on cthd.ID_ChiTietHoaDon = ctNeed.ID_ChiTietHoaDon

		
		
			----- update idHoaDonTruocDo for ctNeed: nếu trong 1 hóa đơn có cùng hàng hóa, giống/# đơn vị tính
			----- giá vốn cuối cùng được update = giá vốn của RN đầu tiên thuộc cùng hóa đơn ---
			update ctNeed 
				set ctNeed.IdHoaDonTruocDo = ctNeed2.IdHoaDonTruocDo
			from @cthd_NeedUpGiaVon ctNeed
			left join  @cthd_NeedUpGiaVon ctNeed2 on ctNeed.RN  = ctNeed2.RN + 1 and ctNeed.NgayLapHoaDon = ctNeed2.NgayLapHoaDon
			and ctNeed.ID_HangHoa = ctNeed2.ID_HangHoa 
								and (ctNeed.ID_LoHang = ctNeed2.ID_LoHang or (ctNeed.ID_LoHang is null and ctNeed2.ID_LoHang is null))


			------ update GiaVonDauKy cho hd co NgayLapMin --: dựa vào giá vốn này để tính GV cho các hóa đơn tiếp theo ----
			update ctNeed 
				set ctNeed.GiaVonDauKy = isnull(gvDK.GiaVonDauKy,0)				
			from @cthd_NeedUpGiaVon ctNeed
			left join #tblGVDauKy gvDK on ctNeed.ID_HangHoa = gvDK.ID_HangHoa 
				and (ctNeed.ID_LoHang = gvDK.ID_LoHang or (ctNeed.ID_LoHang is null and gvDK.ID_LoHang is null)) 
			where ctNeed.RN = 1

		
    		   			
    		DECLARE @IDHoaDon UNIQUEIDENTIFIER, @IDHoaDonGoc UNIQUEIDENTIFIER, @IDHoaDonCu UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX), @LoaiHoaDon INT, @ChoThanhToan bit
    		DECLARE @IDChiTietHoaDon UNIQUEIDENTIFIER;
    		DECLARE @SoLuong FLOAT, @DonGia FLOAT, @ChietKhau FLOAT, @ThanhTien FLOAT;
    		DECLARE @TongTienHang float, @TongChiPhi float, @TongGiamGia FLOAT;
    		DECLARE @TyLeChuyenDoi FLOAT;
    		DECLARE @TonKho FLOAT;
    		DECLARE @IDHangHoa UNIQUEIDENTIFIER, @IDDonViQuiDoi UNIQUEIDENTIFIER, @IDLoHang UNIQUEIDENTIFIER;
    		DECLARE @IDChiNhanhThemMoi UNIQUEIDENTIFIER,  @IDCheckIn UNIQUEIDENTIFIER;
    		DECLARE @YeuCau NVARCHAR(MAX);
    		DECLARE @RN INT;
			DECLARE @GiaVonCu FLOAT, @GiaVon FLOAT, @GiaVonNhan FLOAT;
    		DECLARE @GiaVonMoi FLOAT, @GiaVonCuUpdate FLOAT;
    		DECLARE @GiaVonHoaDonBan FLOAT;
    		DECLARE @TongTienHangDemo FLOAT, @SoLuongDemo FLOAT, @ThanhTienDemo FLOAT,@ChietKhauDemo FLOAT;
			declare @ngayLapHDGoc datetime

				------ duyet cthdNeed update (order by ngaylaphoadon, idhanghoa) ----

				declare @cthdCurrent table (ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER,
					SoLuong float, DonGia float, ChietKhau float, GiaVon float,
					TyLeChuyenDoi float, TongTienHang float, TongChiPhi float, TongGiamGia float)

    		DECLARE CS_GiaVon CURSOR SCROLL LOCAL FOR 
    			SELECT IDHoaDon, IDHoaDonGoc, IdHoaDonTruocDo, MaHoaDon, LoaiHoaDon, ChoThanhToan,
					ID_ChiTietHoaDon, SoLuong, DonGia, TongTienHang, TongChiPhi,
				ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
    			GiaVonDauKy, ID_HangHoa, IDDonViQuiDoi, ID_LoHang, ID_ChiNhanhThemMoi, ID_CheckIn, YeuCau, GiaVon, GiaVonNhan , RN
    			FROM @cthd_NeedUpGiaVon 
				WHERE LaHangHoa = 1 
				ORDER BY  ID_HangHoa, ID_LoHang, RN  ----- chạy lần lượt theo idHangHoa, IdLoHang (ngaylaphd min)
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon 
    			INTO @IDHoaDon, @IDHoaDonGoc, @IDHoaDonCu,  @MaHoaDon, @LoaiHoaDon, @ChoThanhToan,
				@IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau,@ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @GiaVon, @GiaVonNhan, @RN
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
						-------- RN = 1: hd đầu tiên của hàng hóa, giữ nguyên GiaVonCu ban đầu (tức giavondauky) ---								
							if @RN > 1 set @GiaVonCu = @GiaVonCuUpdate
    				
							if @LoaiHoaDon in (4,13,7,10,6,18)
								begin
									---- nếu hd bị hủy/tạm lưu: get giavon cũ trước đó ---
									if @ChoThanhToan is null or @ChoThanhToan ='1' set @GiaVonMoi = @GiaVonCu
									else
										begin ----- begin ChoThanhToan = 0 (hd chưa hủy )--
												----- get cthd current is cursor ----
												insert into @cthdCurrent
												select ID_HangHoa, ID_LoHang, SoLuong, DonGia, ChietKhau, GiaVon, 
													TyLeChuyenDoi, TongTienHang, TongChiPhi, TongGiamGia
												FROM @cthd_NeedUpGiaVon cthd
    											WHERE cthd.IDHoaDon = @IDHoaDon AND cthd.ID_HangHoa = @IDHangHoa 
													AND (cthd.ID_LoHang = @IDLoHang or @IDLoHang is null and cthd.ID_LoHang is null)

												if @LoaiHoaDon in (4,13) ---- 4.nhaphang, 13.nhapnoibo
													begin
														SELECT @TongTienHangDemo = SUM(cthd.SoLuong * (cthd.DonGia -  cthd.ChietKhau)), 
															@SoLuongDemo = SUM(cthd.SoLuong * cthd.TyLeChuyenDoi) 
    													FROM @cthdCurrent cthd
    													 group by cthd.ID_HangHoa, cthd.ID_LoHang

															IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    														BEGIN
    															IF(@TongTienHang != 0)
    															BEGIN
																	------ giavon: tinh sau khi + phi ship---   										
																	SET	@GiaVonMoi = (@GiaVonCu * @TonKho +
																		(@TongTienHangDemo * ( 1 + @TongChiPhi/@TongTienHang)))
																		/(@SoLuongDemo + @TonKho)																													
							
    															END
    															ELSE
    															BEGIN
    																SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo)/(@SoLuongDemo + @TonKho);
    															END
							
    														END
    														ELSE
    														BEGIN	
																------ Tonkho dauky = 0 ----
    															IF(@TongTienHang != 0)
    															BEGIN
																	------ (thanh tien sau giamgia + chi phi VC)/ soluong
							
    																if @SoLuongDemo > 0
																		SET	@GiaVonMoi = (@TongTienHangDemo * (1 + @TongChiPhi/@TongTienHang))/ @SoLuongDemo																		
																	else
																		 SET @GiaVonMoi = @GiaVonCu
								
    															END
    															ELSE
    															BEGIN
																	if @SoLuongDemo > 0
    																	SET	@GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
																	else
																		 SET @GiaVonMoi = @GiaVonCu
    															END
							
    														END
													end

												if @LoaiHoaDon = 7 --- trahang NCC
													begin
														SELECT @TongTienHangDemo = 
															SUM(cthd.SoLuong * cthd.DonGia * ( 1- cthd.TongGiamGia/iif(cthd.TongTienHang=0,1,cthd.TongTienHang))) ,
															@SoLuongDemo = SUM(cthd.SoLuong * cthd.TyLeChuyenDoi) 
    													FROM @cthdCurrent cthd   										
    													GROUP BY cthd.ID_HangHoa, cthd.ID_LoHang
    													IF(@TonKho - @SoLuongDemo > 0)
    													BEGIN										
    														SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) - @TongTienHangDemo)/(@TonKho - @SoLuongDemo);												
    													END
    													ELSE
    													BEGIN
    														SET @GiaVonMoi = @GiaVonCu;
    													END
													end

												if @LoaiHoaDon = 10 ---- dieuchuyen --
													BEGIN
													SELECT @TongTienHangDemo = SUM(cthd.ChietKhau * cthd.DonGia), 
														@SoLuongDemo = SUM(cthd.ChietKhau * cthd.TyLeChuyenDoi) 
    												FROM @cthdCurrent cthd   								
    												GROUP BY cthd.ID_HangHoa, cthd.ID_LoHang 
									
    											IF(@YeuCau = '1' OR (@YeuCau = '4' AND @IDChiNhanhThemMoi != @IDCheckIn))
    											BEGIN
    												SET @GiaVonMoi = @GiaVonCu;
    											END
    											ELSE IF (@YeuCau = '4' AND @IDChiNhanhThemMoi = @IDCheckIn)
    											BEGIN
    												IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    												BEGIN
    													SET @GiaVonMoi = (@GiaVonCu * @TonKho + @TongTienHangDemo)/(@TonKho + @SoLuongDemo);
    												END
    												ELSE
    												BEGIN
    														IF(@SoLuongDemo = 0)
    														BEGIN
    															SET @GiaVonMoi = @GiaVonCu;
    														END
    														ELSE
    														BEGIN
    														SET @GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
    														END
    												END
    											END
											end

												if @LoaiHoaDon = 6 ---- khachang trahang
												begin
														SELECT @SoLuongDemo = SUM(cthd.SoLuong * cthd.TyLeChuyenDoi) 
    													FROM @cthdCurrent cthd    									
    													GROUP BY cthd.ID_HangHoa, cthd.ID_LoHang

    													IF(@IDHoaDonGoc IS NOT NULL)
    													BEGIN
    														SET @GiaVonHoaDonBan = -1;
															set @ngayLapHDGoc = (select top 1 NgayLapHoaDon from @tblCTMuaGoc where ID_HDMuaGoc = @IDHoaDonGoc)
																------ get giavon hangban tại thời điểm mua ---
																select 
																	@GiaVonHoaDonBan = GiaVon
																from
																(
																	select 
																		ctg.NgayLapHoaDon,
																		ctg.GiaVon,
																		ROW_NUMBER() over (partition by ID_HangHoa, ID_LoHang order by NgayLapHoaDon desc) as RN
																	from @tblCTMuaGoc ctg
																	where ctg.LoaiHoaDon not in (3,19,25,29)
																		and ctg.NgayLapHoaDon <= @ngayLapHDGoc
																		and ctg.ID_HangHoa = @IDHangHoa
																		and (ctg.ID_LoHang = @IDLoHang or ctg.ID_LoHang is null and @IDLoHang is null)
																)tblg where Rn = 1

    						
    														IF(@GiaVonHoaDonBan = - 1) ----- tại thời điểm mua: chưa có giá vốn
    														BEGIN    				
    															set @GiaVonHoaDonBan = 0						
    														END
    														IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    														BEGIN    							
    															SET @GiaVonMoi = (@GiaVonCu * @TonKho + @GiaVonHoaDonBan * @SoLuongDemo) / (@TonKho + @SoLuongDemo);
    														END
    														ELSE
    														BEGIN
    															SET @GiaVonMoi = @GiaVonHoaDonBan;
    														END
    													END
    													ELSE
    													BEGIN
															----- nếu trả nhanh (không liên quan đến hóa đơn/gdv nào) ----
    														SET @GiaVonMoi = @GiaVonCu;
    													END
												end

												if @LoaiHoaDon = 18 --- phieu Dieuchinh Giavon
												begin		
													------ nếu phiếu điều chỉnh: gán = GiaVon luôn (không cần quy đổi nữa, vì đã quy đổi rồi) --
    													SELECT @GiaVonMoi = GiaVon
														FROM @cthdCurrent 		

												end

												if @GiaVonMoi is null set @GiaVonMoi = @GiaVonCu

												------ xoa hết @cthdCurrent để insert lại ---
												delete from @cthdCurrent
										end ----- end ChoThanhToan									
								end
							else
								begin ---- LoaiHoaDon #
									SET @GiaVonMoi = @GiaVonCu;
								end  								
					
					-------- update again GiaVonNew for ctNeed -----
					------select @RN, @IDHoaDon as idHoadon,@IDHoaDonCu as idhoadoncu, @GiaVonMoi as gvmoi, @GiaVonCuUpdate as gvcu
    				IF(@LoaiHoaDon = 10 AND @YeuCau = '4' AND @IDCheckIn = @IDChiNhanhThemMoi) ----- nhanhang ----
    				BEGIN --- 	------ nhanhang: update GiaVonNhan ----
						----- nếu hàng thuộc cùng hóa đơn,
						---- giá vốn mới lấy giống với giá vốn của cùng hàng hóa đó sau lần cập nhật đầu tiên ---
						if @IDHoaDon = @IDHoaDonCu set @GiaVonMoi = @GiaVonCuUpdate		

    					UPDATE @cthd_NeedUpGiaVon SET GiaVonNhan = @GiaVonMoi
							WHERE ID_ChiTietHoaDon = @IDChiTietHoaDon;    			
    				END
    				ELSE
    				BEGIN ------ các hdConLai (#nhanhang): update GiaVon  ---		
						if @IDHoaDon = @IDHoaDonCu set @GiaVonMoi = @GiaVonCuUpdate
    					UPDATE @cthd_NeedUpGiaVon SET GiaVon = @GiaVonMoi
						WHERE ID_ChiTietHoaDon = @IDChiTietHoaDon;  
    				END

					------ gán lại GV cũ = giá vốn mới vừa update: để tính lại GiaVon cho các HD tiếp theo----
					set @GiaVonCuUpdate = @GiaVonMoi
										
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonGoc, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @ChoThanhToan,
				@IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau,  @GiaVon, @GiaVonNhan, @RN
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon	

			

			------- update again GiaVonDauKy for cthdNeed: used to tính lại giá trị chênh lệch của phiếu Điều chỉnh GV ----	
			update cthd set cthd.GiaVonDauKy =  isnull(ct.GiaVon,0) * cthd.TyLeChuyenDoi		
			from @cthd_NeedUpGiaVon cthd 
			left join @cthd_NeedUpGiaVon ct
				on ct.ID_HangHoa = cthd.ID_HangHoa 
							and (cthd.ID_LoHang = ct.ID_LoHang or (cthd.ID_LoHang is null and ct.ID_LoHang is null))									
							and cthd.RN = ct.RN + 1
			where cthd.RN > 1 ---- chỉ cập nhật gvDauKy cho Rn bắt đầu từ 2 --
		
			
			

    		---- =========Update BH_HoaDon_ChiTiet ===========
		
			UPDATE hdct
    		SET hdct.GiaVon = gvNew.GiaVon * gvNew.TyLeChuyenDoi,
    			hdct.GiaVon_NhanChuyenHang = iif(gvNew.LoaiHoaDon not in (8,9), gvNew.GiaVonNhan * gvNew.TyLeChuyenDoi, hdct.GiaVon_NhanChuyenHang),
				------ phieuKiemKe: update ThanhToan = GiaVonLech ThanhToan ----
				hdct.ThanhToan = iif(gvNew.LoaiHoaDon = 9,hdct.SoLuong * gvNew.GiaVon * gvNew.TyLeChuyenDoi, hdct.ThanhToan),
				------ phieu XuatKho: update ThanhTien = GiaTriXuat ----
				hdct.ThanhTien = iif(gvNew.LoaiHoaDon = 8, hdct.SoLuong * gvNew.GiaVon * gvNew.TyLeChuyenDoi, hdct.ThanhTien)			
    		FROM BH_HoaDon_ChiTiet hdct
    		JOIN @cthd_NeedUpGiaVon AS gvNew ON hdct.ID = gvNew.ID_ChiTietHoaDon   
			WHERE gvNew.LoaiHoaDon !=18 

    
    		---- ==== update to Phieu DieuChinhGiaVon === ----
			--- DonGia = giavon truoc khi điều chỉnh: không cần quy đổi nữa, vì gvdk đã tính theo đơn vị quy đổi ----
			--- PTChietKhau: giá vốn lêch tăng ---
			--- TienChietKhau: giá vốn lệch giảm --- 
    		UPDATE hdct
    		SET hdct.DonGia = gvNew.GiaVonDauKy, 
    			hdct.PTChietKhau = CASE WHEN hdct.GiaVon - gvNew.GiaVonDauKy > 0 THEN hdct.GiaVon - gvNew.GiaVonDauKy ELSE 0 END,
    			hdct.TienChietKhau = CASE WHEN hdct.GiaVon - gvNew.GiaVonDauKy > 0 THEN 0 ELSE gvNew.GiaVonDauKy - hdct.GiaVon END
    		FROM BH_HoaDon_ChiTiet AS hdct
    		INNER JOIN @cthd_NeedUpGiaVon AS gvNew
    		ON hdct.ID = gvNew.ID_ChiTietHoaDon
    		WHERE gvNew.LoaiHoaDon = 18 
		


    
				------ Update GiaVon DichVu = sum GiaVon (ThanhPhan_DinhLuong) ------	
    		UPDATE hdct
    			SET hdct.GiaVon = iif(hdct.SoLuong = 0,0, isnull(gvDLuong.GiaVonDinhLuong,0) / hdct.SoLuong)
    		FROM BH_HoaDon_ChiTiet hdct
    		JOIN
    				(SELECT ct.ID_ChiTietDinhLuong,
    					SUM(ct.GiaVon * ct.SoLuong) AS GiaVonDinhLuong
    				FROM @cthd_NeedUpGiaVon ct
					where ct.LoaiHoaDon in (1,25,6)
					and ct.ID_ChiTietDinhLuong is not null
					and ct.ChatLieu!='5' --- khonglay dinhluong bi huy ----
    				GROUP BY ID_ChiTietDinhLuong
					) gvDLuong   			
    		ON hdct.ID = gvDLuong.ID_ChiTietDinhLuong

			
    		--=========END Update BH_HoaDon_ChiTiet

    		------ Update DM_GiaVon------
			------  get giavonHienTai (last GiaVon in cthdNeed) && update to DM_GiaVon ---					
			select distinct
				gvCurrent.ID_HangHoa,
				gvCurrent.ID_LoHang,
				gvCurrent.GiaVon * qd.TyLeChuyenDoi as GiaVon,
				qd.ID_DonViQuiDoi
			into #gvHienTai			
			from
			(
			select 
				ctNeed.ID_HangHoa,
				ctNeed.ID_LoHang,
				iif(ctNeed.ID_CheckIn = ctNeed.ID_ChiNhanhThemMoi,ctNeed.GiaVonNhan, ctNeed.GiaVon) as GiaVon,
				ROW_NUMBER() over (partition by ID_HangHoa, ID_LoHang order by NgayLapHoaDon desc) as RN
			from @cthd_NeedUpGiaVon ctNeed
			) gvCurrent
			join @tblQuyDoi qd on gvCurrent.ID_HangHoa = qd.ID_HangHoa 
				and (gvCurrent.ID_LoHang = qd.ID_LoHang or gvCurrent.ID_LoHang is null and qd.ID_LoHang is null)
			where RN = 1

		
			------ only update this chinhanh ---
    		UPDATE gv
    			SET gv.GiaVon = ISNULL(gvHienTai.GiaVon, 0)
    		FROM DM_GiaVon gv
    		join #gvHienTai gvHienTai on gv.ID_DonViQuiDoi = gvHienTai.ID_DonViQuiDoi
				and (gv.ID_LoHang = gvHienTai.ID_LoHang or gvHienTai.ID_LoHang is null and gv.ID_LoHang is null)
			where gv.ID_DonVi = @IDChiNhanh
			
			----- insert to DM_GiaVon if not exists ----
			INSERT INTO DM_GiaVon (ID, ID_DonVi, ID_DonViQuiDoi, ID_LoHang, GiaVon)
			select 
				newID(),
				@IDChiNhanh,
				gvHienTai.ID_DonViQuiDoi,
				gvHienTai.ID_LoHang,
				gvHienTai.GiaVon
			from #gvHienTai gvHienTai
			where not exists (
				select id from DM_GiaVon gv
				where gv.ID_DonViQuiDoi = gvHienTai.ID_DonViQuiDoi
				and (gv.ID_LoHang = gvHienTai.ID_LoHang or gvHienTai.ID_LoHang is null and gv.ID_LoHang is null)
				and gv.ID_DonVi = @IDChiNhanh
			)	  
    		
			

		
			drop table #tblTonLuyKe
			drop table #tblGVDauKy
			drop table #gvHienTai
			drop table #ctTraHang

    		END --- end beginOut

			------ alway remove cthd if has delete ----
			delete from BH_HoaDon_ChiTiet where ID_HoaDon = @IDHoaDonInput and ChatLieu='5'
    		
END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateTonLuyKeCTHD_whenUpdate]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanhInput [uniqueidentifier],
    @NgayLapHDOld [datetime]
AS
BEGIN
    SET NOCOUNT ON;

	----declare @IDHoaDonInput uniqueidentifier, @IDChiNhanhInput uniqueidentifier, @NgayLapHDOld datetime

	----select top 1 @IDHoaDonInput = ID, @IDChiNhanhInput = ID_DonVi, @NgayLapHDOld = NgayLapHoaDon  
	----from BH_HoaDon where MaHoaDon='XH0000003370'


    
    		declare @NgayLapHDNew DATETIME, @NgayLapHDMin DATETIME, @LoaiHoaDon INT;   
    		declare @tblHoaDonChiTiet ChiTietHoaDonEdit -- chỉ dùng để Insert_ThongBaoHetTonKho ---
    
    		-----  get NgayLapHD by IDHoaDon: if update HDNhanHang (loai 10, yeucau = 4 --> get NgaySua
    		select 
    			@NgayLapHDNew = NgayLapHoaDon,
    			@LoaiHoaDon = LoaiHoaDon
    		from (
    					select LoaiHoaDon,
							case when @IDChiNhanhInput = ID_CheckIn and YeuCau !='1' then NgaySua else NgayLapHoaDon end as NgayLapHoaDon
    					from BH_HoaDon where ID = @IDHoaDonInput
				) a
    
    		-- alway get Ngay min --> compare to update TonLuyKe
    		IF(@NgayLapHDOld > @NgayLapHDNew)
    			SET @NgayLapHDMin = @NgayLapHDNew;
    		ELSE
    			SET @NgayLapHDMin = @NgayLapHDOld;
    
			declare @NgayLapHDMin_SubMiliSecond datetime = dateadd(MILLISECOND,-3, @NgayLapHDMin)

		
			 ----- get all donviquydoi lienquan ---
			declare @tblQuyDoi table (ID_DonViQuiDoi uniqueidentifier, ID_HangHoa uniqueidentifier, 
				ID_LoHang uniqueidentifier, 
				TyLeChuyenDoi float,
				LaHangHoa bit)
			insert into @tblQuyDoi
			select * from dbo.fnGetAllHangHoa_NeedUpdateTonKhoGiaVon(@IDHoaDonInput)

    		
    		------ get all cthd need update TonKho (>= ngayLapHoaDon of hdCurrent) -----
    		select
    			ct.ID, 
				ct.ID_HoaDon,
    			ct.ID_LoHang,
				ct.ID_DonViQuiDoi,
				-- chatlieu = 5 (cthd bi xoa khi updateHD), chatlieu =2 (tra gdv  - khong cong lai tonkho)
    			case when hd.LoaiHoaDon in (1,2,3,19,31,36) or ct.ChatLieu in ('5','2') or hd.ChoThanhToan is null or hd.ChoThanhToan ='1' then 0 else SoLuong end as SoLuong,
    			case when ct.ChatLieu= '5' or hd.ChoThanhToan is null or hd.ChoThanhToan ='1' then 0 else TienChietKhau end as TienChietKhau,
    			case when ct.ChatLieu= '5' or hd.ChoThanhToan is null or hd.ChoThanhToan ='1' then 0 else ct.ThanhTien end as ThanhTien,-- kiemke bi huy
				----- chỉ cần lấy TonLuyLe của phiếu kiếm kê, vì các loại khác sẽ bị tính lại TonKho ---
				iif(hd.LoaiHoaDon = 9,iif(hd.ChoThanhToan is null or hd.ChoThanhToan ='1',0, ct.ThanhTien * qd.TyLeChuyenDoi),0) as TonLuyKe,
				qd.ID_HangHoa,
    			qd.TyLeChuyenDoi,
    			hd.MaHoaDon,
    			hd.LoaiHoaDon,
    			hd.ID_DonVi,
    			hd.ID_CheckIn,
    			hd.YeuCau,				
				hd.ChoThanhToan,
    			case when hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon
    		into #temp
    		from BH_HoaDon_ChiTiet ct
			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID   
			join @tblQuyDoi qd on qd.ID_DonViQuiDoi = ct.ID_DonViQuiDoi and (ct.ID_LoHang = qd.ID_LoHang or ct.ID_LoHang is null and qd.ID_LoHang is null)
    		WHERE hd.ID = @IDHoaDonInput
			----- chỉ update TonKho cho hdCurrent/ hoặc hóa đơn chưa hủy có >= ngayLapHoaDon of hdCurrent
			or (hd.ChoThanhToan='0'  
					and ((hd.ID_DonVi = @IDChiNhanhInput and hd.NgayLapHoaDon > @NgayLapHDMin_SubMiliSecond
    				and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    				or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanhInput and hd.NgaySua > @NgayLapHDMin_SubMiliSecond))
					)
					
				
			------ TonDauKy of hanghoa (id_hanghoa, id_lohang) ----			
				select *
				into #tblTonKhoDK
				from
				(
					select 
						tblTonKho.ID_HangHoa,
						tblTonKho.ID_LoHang,					
						tblTonKho.TonLuyKe,
						row_number() over (partition by tblTonKho.ID_HangHoa,tblTonKho.ID_LoHang order by tblTonKho.NgayLapHoaDon desc) as RN	
					from
					(
					select 				
						ct.ID_LoHang,
						qd.ID_HangHoa,				
						CASE WHEN @IDChiNhanhInput = hd.ID_CheckIn and hd.YeuCau = '4' then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon,
						CASE WHEN @IDChiNhanhInput = hd.ID_CheckIn and hd.YeuCau = '4' THEN ct.TonLuyKe_NhanChuyenHang ELSE ct.TonLuyKe END as TonLuyKe
					from BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd  ON ct.ID_HoaDon = hd.ID  
					join @tblQuyDoi qd on ct.ID_DonViQuiDoi = qd.ID_DonViQuiDoi
    				WHERE hd.ChoThanhToan = 0    		
						and hd.LoaiHoaDon NOT IN (3, 19, 25,29) ---- 29. Phiếu khởi tạo phù tùng xe		
						------ chỉ lấy hd trước đó
						and ((hd.ID_DonVi = @IDChiNhanhInput and hd.NgayLapHoaDon < @NgayLapHDMin
    					and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    						or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanhInput and hd.NgaySua < @NgayLapHDMin))
					)tblTonKho
				)tblTonKhoDK where tblTonKhoDK.RN =1

							
				----- get phieuKK theo hanghoa (mỗi hàng hóa có khoảng kiểm kê # nhau) ----
				declare @tblHangKiemKe table (ID_HoaDon uniqueidentifier, NgayKiemKe datetime, ID_HangHoa uniqueidentifier, ID_LoHang uniqueidentifier)
				insert into @tblHangKiemKe
				select ID_HoaDon, NgayLapHoaDon , ID_HangHoa, ID_LoHang
				from #temp 
				where LoaiHoaDon = 9 and ChoThanhToan = 0 
				group by ID_HoaDon, NgayLapHoaDon,ID_HangHoa, ID_LoHang


				declare @countKiemKe float = (select count(*) from @tblHangKiemKe)			

		

				------ get cthd has hangKiemKe ---			
				select *
				into #cthdHasKiemKe
				from #temp ct
				where ct.LoaiHoaDon != 9 			
				and exists 
					(select ID_HangHoa from @tblHangKiemKe hKK 
					where ct.ID_HangHoa = hKK.ID_HangHoa 
							and (ct.ID_LoHang = hKK.ID_LoHang or ct.ID_LoHang is null and hKK.ID_LoHang is null))


				declare @tblCTHDAfter table (ID_ChiTietHD uniqueidentifier, ID_HoaDon uniqueidentifier, LoaiHoaDon int, MaHoaDon nvarchar (100), NgayLapHoaDon datetime,
							ID_DonVi uniqueidentifier, ID_Checkin uniqueidentifier, YeuCau nvarchar(max),
							ID_HangHoa uniqueidentifier, ID_LoHang uniqueidentifier, TonLuyKe float)	

				if @countKiemKe >0
				begin
															
						------ duyet phieuKiemKe  --
						declare @idHoaDon uniqueidentifier, @ngayKiemKe datetime, @idHangHoa uniqueidentifier, @idLoHang  uniqueidentifier
						declare _curKK cursor for
						select ID_HoaDon, NgayKiemKe, ID_HangHoa, ID_LoHang
						from @tblHangKiemKe 
						order by NgayKiemKe
						open _curKK
						FETCH NEXT FROM _curKK
						INTO @idHoaDon, @ngayKiemKe,@idHangHoa,@idLoHang
						WHILE @@FETCH_STATUS = 0
						BEGIN   
						
								-------- get ctKK (with idhanghoa, idlohang) trong khoang thoi gian ---
								declare @ngayKiemKeNext datetime 
									= (select top 1 NgayKiemKe from @tblHangKiemKe 
										where NgayKiemKe > @ngayKiemKe and ID_HangHoa = @idHangHoa
										and (ID_LoHang = @idLoHang or ID_LoHang is null and @idLoHang is null)
										order by NgayKiemKe
										)

							
								----- tinh TonLuyKe theo giai doan kiem ke ---
								insert into @tblCTHDAfter
								select 
									ct.ID, ct.ID_HoaDon,
									ct.LoaiHoaDon, ct.MaHoaDon, ct.NgayLapHoaDon,
									ct.ID_DonVi, ct.ID_CheckIn, ct.YeuCau,
									ct.ID_HangHoa, ct.ID_LoHang,												
    							ISNULL(ctKK.TonLuyKe, 0) + 
    								(SUM(IIF(LoaiHoaDon IN (5, 7, 8,35, 37, 38, 39, 40), -1 * ct.SoLuong* ct.TyLeChuyenDoi, 
    							IIF(LoaiHoaDon IN (4, 6, 18,13,14), SoLuong * TyLeChuyenDoi, 				
    								IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    							IIF(ct.LoaiHoaDon = 10 AND ct.YeuCau = '4' AND ct.ID_CheckIn = @IDChiNhanhInput, ct.TienChietKhau* ct.TyLeChuyenDoi, 0))))) 
    								OVER(PARTITION BY ct.ID_HangHoa, ct.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe  
								from #cthdHasKiemKe ct
								join
								(
									------ get tondauky from phieuKiemKe ----
									select 
										ctAll.ID_HangHoa,
										ctAll.ID_LoHang,
										ctAll.TonLuyKe
									from #temp ctAll 
									where ctAll.ID_HoaDon = @idHoaDon
									and ID_HangHoa = @idHangHoa and (ID_LoHang = @idLoHang or ID_LoHang is null and @idLoHang is null)
								)ctKK on ct.ID_HangHoa = ctKK.ID_HangHoa
								and (ct.ID_LoHang = ctKK.ID_LoHang or ct.ID_LoHang is null and ctKK.ID_LoHang is null)
								where ct.NgayLapHoaDon >= @ngayKiemKe	
								and (@ngayKiemKeNext is null or ct.NgayLapHoaDon < @ngayKiemKeNext)						
		
						FETCH NEXT FROM _curKK
						INTO @idHoaDon, @ngayKiemKe, @idHangHoa,@idLoHang

						END
						CLOSE _curKK;
						DEALLOCATE _curKK;					
				end

				----- hàng thuộc phiếu kk, nhưng có ngày lập < ngày kiểm kê --> tính theo tondauky  ----
				insert into @tblCTHDAfter
				select 
					ct.ID, ct.ID_HoaDon,
					ct.LoaiHoaDon, ct.MaHoaDon, ct.NgayLapHoaDon,
					ct.ID_DonVi, ct.ID_CheckIn, ct.YeuCau,
					ct.ID_HangHoa, ct.ID_LoHang,												
    			ISNULL(lkdk.TonLuyKe, 0) + 
    				(SUM(IIF(LoaiHoaDon IN (5, 7, 8,35, 37, 38, 39, 40), -1 * ct.SoLuong* ct.TyLeChuyenDoi, 
    			IIF(LoaiHoaDon IN (4, 6, 18,13,14), SoLuong * TyLeChuyenDoi, 				
    				IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    			IIF(ct.LoaiHoaDon = 10 AND ct.YeuCau = '4' AND ct.ID_CheckIn = @IDChiNhanhInput, ct.TienChietKhau* ct.TyLeChuyenDoi, 0))))) 
    				OVER(PARTITION BY ct.ID_HangHoa, ct.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe  
				from #cthdHasKiemKe ct 
				left join #tblTonKhoDK lkdk on ct.ID_HangHoa = lkdk.ID_HangHoa 
					and (ct.ID_LoHang = lkdk.ID_LoHang or ct.ID_LoHang is null and lkdk.ID_LoHang is null)
				where not exists (select id from @tblCTHDAfter ctAfter where ct.ID = ctAfter.ID_ChiTietHD)
		

				------ get cthd conLai (not exists hangKiemKe) && tinhton ---
				----- neu khong co phieuKK: đây là ctAll ---
				----- nguoclai: ctALL trừ ctKiemKe
				insert into @tblCTHDAfter
				select 
					ct.ID, ct.ID_HoaDon,
					ct.LoaiHoaDon, ct.MaHoaDon, ct.NgayLapHoaDon,
					ct.ID_DonVi, ct.ID_CheckIn, ct.YeuCau,
					ct.ID_HangHoa, ct.ID_LoHang,												
    			ISNULL(lkdk.TonLuyKe, 0) + 
    				(SUM(IIF(LoaiHoaDon IN (5, 7, 8,35, 37, 38, 39, 40), -1 * ct.SoLuong* ct.TyLeChuyenDoi, 
    			IIF(LoaiHoaDon IN (4, 6, 18,13,14), SoLuong * TyLeChuyenDoi, 				
    				IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    			IIF(ct.LoaiHoaDon = 10 AND ct.YeuCau = '4' AND ct.ID_CheckIn = @IDChiNhanhInput, ct.TienChietKhau* ct.TyLeChuyenDoi, 0))))) 
    				OVER(PARTITION BY ct.ID_HangHoa, ct.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe  
				from #temp ct
				left join #tblTonKhoDK lkdk on ct.ID_HangHoa = lkdk.ID_HangHoa 
					and (ct.ID_LoHang = lkdk.ID_LoHang or ct.ID_LoHang is null and lkdk.ID_LoHang is null)
				where not exists (select id from @tblCTHDAfter ctIn where ct.ID = ctIn.ID_ChiTietHD )
				------ tính lại tồn kho cho phiếu kiểm kê bị hủy ---
				and (ct.LoaiHoaDon !=9 or (ct.LoaiHoaDon = 9 and (ct.ChoThanhToan is null or ChoThanhToan ='1')))



				------- vì @tblCTHDAfter không bao gồm phiếu kiểm kê: phải insert ---> tính Tồn kho hiện tại
					insert into @tblCTHDAfter
					select 
						ct.ID, ct.ID_HoaDon,
						ct.LoaiHoaDon, ct.MaHoaDon, ct.NgayLapHoaDon,
						ct.ID_DonVi, ct.ID_CheckIn, ct.YeuCau,
						ct.ID_HangHoa, ct.ID_LoHang,
						ct.TonLuyKe
					from #temp ct
					where ct.LoaiHoaDon = 9 and ChoThanhToan = 0	

			

				

				------ update again TonLuyKe for HoaDon_ChiTiet  -----					
    			UPDATE hdct
    			SET hdct.TonLuyKe = IIF(tlkupdate.ID_DonVi = @IDChiNhanhInput, tlkupdate.TonLuyKe, hdct.TonLuyKe), 
    				hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput and tlkupdate.LoaiHoaDon = 10, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
    			FROM BH_HoaDon_ChiTiet hdct
    			JOIN @tblCTHDAfter tlkupdate ON hdct.ID = tlkupdate.ID_ChiTietHD 


				----- get TonKho hientai full ID_QuiDoi, ID_LoHang of ID_HangHoa ----
				DECLARE @tblTonKhoNow TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER,ID_LoHang UNIQUEIDENTIFIER, TonKho FLOAT)
				insert into @tblTonKhoNow
				select 
					qd.ID_DonViQuiDoi,
					tkNow.ID_LoHang,					
					tkNow.TonLuyKe / qd.TyLeChuyenDoi as TonLuyKeNow --- tinh TonKuyke theo DVT
				from(
					select ID_HangHoa, ID_LoHang,
						TonLuyKe,
						row_number() over (partition by ID_HangHoa, ID_LoHang order by NgayLapHoaDon desc) as RN	
					from @tblCTHDAfter
				)tkNow
				join @tblQuyDoi qd on tkNow.ID_HangHoa = qd.ID_HangHoa 
					and (tkNow.ID_LoHang = qd.ID_LoHang or tkNow.ID_LoHang is null and qd.ID_LoHang is null)
				where tkNow.RN= 1



				------ UPDATE TonKho in DM_HangHoa_TonKho -----
				UPDATE hhtonkho SET hhtonkho.TonKho = ISNULL(tkNow.TonKho, 0)
    			FROM DM_HangHoa_TonKho hhtonkho
    			JOIN @tblTonKhoNow tkNow on hhtonkho.ID_DonViQuyDoi = tkNow.ID_DonViQuiDoi 
    			and (hhtonkho.ID_LoHang = tkNow.ID_LoHang or tkNow.ID_LoHang is null)
				and hhtonkho.ID_DonVi = @IDChiNhanhInput


				------ insert DM_TonKho if not exist ----
				INSERT INTO DM_HangHoa_TonKho(ID, ID_DonVi, ID_DonViQuyDoi, ID_LoHang, TonKho)
				select 
				newID(),
				@IDChiNhanhInput,
				tkNow.ID_DonViQuiDoi,
				tkNow.ID_LoHang,
				tkNow.TonKho
				from @tblTonKhoNow tkNow
				where not exists (
					select id from DM_HangHoa_TonKho tk
					where tk.ID_DonViQuyDoi = tkNow.ID_DonViQuiDoi
					and (tk.ID_LoHang = tkNow.ID_LoHang or tkNow.ID_LoHang is null and tk.ID_LoHang is null)
					and tk.ID_DonVi = @IDChiNhanhInput
				)

			------- insert Thongbao het tonkho ----
			begin try
				------- get hanghoa ----
				insert into @tblHoaDonChiTiet (ID_DonViQuiDoi, ID_HangHoa, ID_LoHang, TyLeChuyenDoi)
				select ID_DonViQuiDoi, ID_HangHoa, ID_LoHang, TyLeChuyenDoi from @tblQuyDoi
				
				exec Insert_ThongBaoHetTonKho @IDChiNhanhInput, @LoaiHoaDon, @tblHoaDonChiTiet
			end try
			begin catch
			end catch

    
    	
    		 ----- neu update NhanHang --> goi ham update TonKho 2 lan
    		 ---- update GiaVon neu tontai phieu NhapHang,ChuyenHang/NhanHang, DieuChinhGiaVon 
    		declare @count2 float = (select count(ID_HoaDon) from #temp where LoaiHoaDon in (4,7,10, 18))
    		select ISNULL(@count2,0) as UpdateGiaVon, ISNULL(@countKiemKe,0) as UpdateKiemKe, @NgayLapHDMin as NgayLapHDMin


			----drop table #temp
			----drop table #tblTonKhoDK
			----drop table #cthdHasKiemKe
END");

			Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMucHangHoa]
   @IDChiNhanh uniqueidentifier ='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
   @TextSearch nvarchar(max) ='',
   @IDThuocTinhHangs nvarchar(max)='', 
   @TrangThaiKho int=0, 
   @Where nvarchar(max) ='',
   @CurrentPage int = 0,
   @PageSize int = 1000,
   @ColumnSort varchar(100) ='NgayTao',
   @SortBy varchar(20) = 'DESC'
AS
BEGIN
	SET NOCOUNT ON;

	declare @where1 nvarchar(max), @where2 nvarchar(max), 
		@paramDefined nvarchar(max),
    	@sql1 nvarchar(max) ='', @sql2 nvarchar(max) =''
    	declare @tblDefined nvarchar(max) = concat(N' declare @tblThuocTinh table (ID uniqueidentifier) ',	
    											   N' declare @tblSearch table (Name nvarchar(max)) ')


	set @where1 =' where 1 = 1 and qd.LaDonViChuan = 1 '
    set @where2 =' where 1 = 1'
	
	
	if isnull(@Where,'')!=''
		set @Where = CONCAT(' and ',N'', @Where)

	if isnull(@ColumnSort,'')=''
		set @ColumnSort = 'NgayTao'
	if isnull(@SortBy,'')=''
		set @SortBy = 'DESC'
    
	if isnull(@TextSearch,'')!=''
		begin
			set @sql1 = concat(@sql1,
				N'DECLARE @count int;
				INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''';
				Select @count =  (Select count(*) from @tblSearch) ')

			set @where1 = CONCAT(@where1, N' and
						((select count(*) from @tblSearch b where 
    								hh.TenHangHoa_KhongDau like ''%''+b.Name+''%''
    								or hh.TenHangHoa_KyTuDau like ''%''+b.Name+''%'' 
									or hh.TenHangHoa like ''%''+b.Name+''%''
									or hh.GhiChu like ''%'' +b.Name +''%'' 
    								or qd.MaHangHoa like ''%''+b.Name+''%'' )=@count or @count=0)')

		end

	if isnull(@IDThuocTinhHangs,'')!=''
		begin
			set @sql1 = concat(@sql1, N' insert into @tblThuocTinh select name from dbo.splitstring(@IDThuocTinhHangs_In) where Name!='' ''')		
			set @where1 = CONCAT(@where1, N' and exists
									(select * 
									from HangHoa_ThuocTinh tt 
									where hh.ID = tt.ID_HangHoa 
									and exists (select ID from @tblThuocTinh prop where tt.ID = prop.ID) 
									)')
		end
	
	if isnull(@TrangThaiKho,0)!=0
		begin			
			if @TrangThaiKho in (1,5,6)
				set @where2 = CONCAT(@where2, N' and tblOut.TrangThai_TonKho = @TrangThaiKho_In ') 
			if @TrangThaiKho in (3,4)
				set @where2 = CONCAT(@where2, N' and tblOut.TrangThai_DinhMucTon = @TrangThaiKho_In ') 
		end
	
	set @sql2= concat( N'
;with data_cte
 as
 (
	select *,
		tblOut.TheoDoi as TrangThai,
		case tblOut.LoaiHangHoa 
			when 1 then N''Hàng hóa''
			when 2 then N''Dịch vụ''
			when 3 then N''Combo''
		end as sLoaiHangHoa
	from
	(
	select tbl.*,
		cast(tbl.LaChaCungLoai1 as bit) LaChaCungLoai,
		tbl.TenDonViTinh as DonViTinhChuan,
		nhomDV.ID_NhomHangHoa as ID_NhomHoTro,
		nhomHT.TenNhomHangHoa as TenNhomHoTro,	
		iif(tbl.CountCungLoai = 1 or QuanLyTheoLoHang =''1'', tbl.MaHangHoa1, concat(''('',tbl.CountCungLoai, N'') Mã hàng'')) as MaHangHoa,
		iif(tbl.LoaiHangHoa=1, tbl.GiaVon1, dbo.GetGiaVonOfDichVu(@IDChiNhanh_In,tbl.ID_DonViQuiDoi)) as GiaVon	,
		------0.all
		----- 1.tonkho > 0
		----- 2.tonkho <=0 (bo qua cai nay)
		----- 3.Dưới định mức tồn
		----- 4.Vượt định mức tồn
		----- 5.Hàng âm kho
		----- 6.TonKho = 0
		case
			when tbl.TonKho > 0 then 1
			when tbl.TonKho < 0 then 5
		else 6 end as TrangThai_TonKho,
		case
			when tbl.TonKho < tbl.TonToiThieu then 3
			when tbl.TonKho > tbl.TonToiDa then 4
		else 0 end as TrangThai_DinhMucTon
	from
	(
			select 			
				max(tblGr.ID) as ID,		
				max(tblGr.ID_DonViQuiDoi) as ID_DonViQuiDoi,
				max(tblGr.LaChaCungLoai) as LaChaCungLoai1,		
				max(tblGr.MaHangHoa) as MaHangHoa1,
				max(tblGr.NgayTao) as NgayTao,
			
				max(tblGr.TenDonViTinh) as TenDonViTinh,
				max(tblGr.ThuocTinhGiaTri) as ThuocTinhGiaTri,			
				count(tblGr.ID_HangHoaCungLoai) as CountCungLoai,

				tblGr.TenHangHoa,
				tblGr.TenHangHoa_KhongDau,
				tblGr.LaHangHoa,
				tblGr.GhiChu,			
				tblGr.DuocBanTrucTiep,
				tblGr.TheoDoi,
				tblGr.Xoa,
				tblGr.QuanLyTheoLoHang,
				tblGr.TrangThaiKinhDoanh,
				tblGr.TrangThaiHang,
				tblGr.ID_Xe,
			
				tblGr.ID_HangHoaCungLoai,
				tblGr.ID_NhomHangHoa,
				tblGr.NhomHangHoa,
			
				tblGr.LoaiHangHoa,
				tblGr.SoPhutThucHien,
				tblGr.DichVuTheoGio,
				tblGr.ChietKhauMD_NV,
				tblGr.ChietKhauMD_NVTheoPT,
				tblGr.DuocTichDiem,
				tblGr.QuanLyBaoDuong,
				tblGr.LoaiBaoDuong,
				tblGr.SoKmBaoHanh,
				tblGr.HoaHongTruocChietKhau,
				tblGr.TonToiDa,
				tblGr.TonToiThieu,				
				tblGr.GiaBan,
				
				max(tblGr.GiaVon) as GiaVon1,
				sum(tblGr.TonKho) as TonKho		
			from
			(
				select 
					hh.ID,				
					hh.ID_Xe,
					qd.ID as ID_DonViQuiDoi,
					hh.TenHangHoa,
					hh.TenHangHoa_KhongDau,
					qd.MaHangHoa,
					hh.LaHangHoa,
					hh.GhiChu,				
					cast(hh.LaChaCungLoai as int) LaChaCungLoai,
					hh.DuocBanTrucTiep,
					hh.TheoDoi,
					hh.NgayTao,
					hh.ID_HangHoaCungLoai,
					hh.ID_NhomHang as ID_NhomHangHoa,
					nhom.TenNhomHangHoa as NhomHangHoa,
			
					iif(hh.LoaiHangHoa is null,iif(hh.LaHangHoa=''1'',1,2), hh.LoaiHangHoa) as LoaiHangHoa,
					isnull(hh.SoPhutThucHien,0) as SoPhutThucHien,
					isnull(hh.DichVuTheoGio,0) as DichVuTheoGio,	
					isnull(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
					isnull(hh.ChietKhauMD_NVTheoPT,''1'') as ChietKhauMD_NVTheoPT,
					isnull(hh.DuocTichDiem,0) as DuocTichDiem,
					iif(hh.QuanLyTheoLoHang is null,''0'', hh.QuanLyTheoLoHang) as QuanLyTheoLoHang,
					iif(hh.QuanLyBaoDuong is null,0, hh.QuanLyBaoDuong) as QuanLyBaoDuong,
					iif(hh.LoaiBaoDuong is null,0, hh.LoaiBaoDuong) as LoaiBaoDuong,
					iif(hh.SoKmBaoHanh is null,0, hh.SoKmBaoHanh) as SoKmBaoHanh,
					iif(hh.HoaHongTruocChietKhau is null,0, hh.HoaHongTruocChietKhau) as HoaHongTruocChietKhau,		
					isnull(hh.TonToiDa,0) as TonToiDa,
					isnull(hh.TonToiThieu,0) as TonToiThieu,
				
					qd.GiaBan,
					qd.Xoa,
					qd.TenDonViTinh,
					qd.ThuocTinhGiaTri,
					iif(hh.TheoDoi=''1'',1,2) as TrangThaiKinhDoanh, ----- 0.all, 1.dangkinhdoanh, 2.ngungkinhdoanh
					iif(qd.Xoa=''1'',1,0) as TrangThaiHang,
					ISNULL(tk.TonKho,0) as TonKho,
					isnull(gv.GiaVon,0) as GiaVon ',						
					
				N' from DM_HangHoa hh 	
				left join DonViQuiDoi qd on qd.ID_HangHoa= hh.ID
				left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID		
				left join DM_HangHoa_TonKho tk on qd.ID = tk.ID_DonViQuyDoi and tk.ID_DonVi= @IDChiNhanh_In
				left join DM_GiaVon gv on qd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi= @IDChiNhanh_In	
				and (gv.ID_LoHang = tk.ID_LoHang or hh.QuanLyTheoLoHang =''0'')		
				
	', @where1,		
	 N'  ) tblGr
		 group by tblGr.TenHangHoa, ------ hangcungloai: chi lay 1 dong
			tblGr.TenHangHoa_KhongDau,
			tblGr.LaHangHoa,
			tblGr.ID_Xe,
			tblGr.GhiChu,			
			tblGr.DuocBanTrucTiep,
			tblGr.Xoa,
			tblGr.QuanLyTheoLoHang,
			tblGr.TrangThaiKinhDoanh,
			tblGr.TrangThaiHang,
			tblGr.TheoDoi,
			tblGr.ID_HangHoaCungLoai,
			tblGr.ID_NhomHangHoa,
			tblGr.NhomHangHoa,
			tblGr.LoaiHangHoa,
			tblGr.SoPhutThucHien,
			tblGr.DichVuTheoGio,
			tblGr.ChietKhauMD_NV,
			tblGr.ChietKhauMD_NVTheoPT,
			tblGr.DuocTichDiem,
			tblGr.QuanLyBaoDuong,
			tblGr.LoaiBaoDuong,
			tblGr.SoKmBaoHanh,
			tblGr.HoaHongTruocChietKhau,
			tblGr.TonToiDa,
			tblGr.TonToiThieu,
			tblGr.GiaBan
		) tbl		
		left join 
		(
			select distinct Id_NhomHang, Id_DonViQuiDoi
			from NhomHang_ChiTietSanPhamHoTro
			where LaSanPhamNgayThuoc = 2
		) spht on tbl.ID_DonViQuiDoi= spht.Id_DonViQuiDoi 
		left join NhomHangHoa_DonVi nhomDV on spht.Id_NhomHang= nhomDV.ID_NhomHangHoa and nhomDV.ID_DonVi = @IDChiNhanh_In
		left join DM_NhomHangHoa nhomHT on spht.Id_NhomHang= nhomHT.ID
	) tblOut ', @where2, @Where,
	N'),
count_cte
as
(
	select COUNT(ID) as TotalRow,
		ceiling(COUNT(ID)/ cast (@PageSize_In as float)) as TotalPage,
		sum(TonKho) as SumTonKho
	from data_cte
)

select *
from data_cte dt
cross join count_cte
order by 
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''NgayTao'' then NgayTao end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''NgayTao'' then NgayTao end DESC,
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''MaHangHoa'' then MaHangHoa1 end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''MaHangHoa'' then MaHangHoa1 end DESC,
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''TenHangHoa'' then TenHangHoa end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''TenHangHoa'' then TenHangHoa end DESC,
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''TenNhomHang'' then NhomHangHoa end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''TenNhomHang'' then NhomHangHoa end DESC,
	case when @SortBy_In <> ''ASC'' then 0
	when @ColumnSort_In=''GiaBan'' then GiaBan end ASC,
	case when @SortBy_In <> ''DESC'' then 0
	when @ColumnSort_In=''GiaBan'' then GiaBan end DESC,
	case when @SortBy_In <> ''ASC'' then 0
	when @ColumnSort_In=''GiaVon'' then GiaVon1 end ASC,
	case when @SortBy_In <> ''DESC'' then 0
	when @ColumnSort_In=''GiaVon'' then GiaVon1 end DESC,
	case when @SortBy_In <> ''ASC'' then 0
	when @ColumnSort_In=''TonKho'' then TonKho end ASC,
	case when @SortBy_In <> ''DESC'' then 0
	when @ColumnSort_In=''TonKho'' then TonKho end DESC
	OFFSET (@CurrentPage_In* @PageSize_In) ROWS
	FETCH NEXT @PageSize_In ROWS ONLY

'
)
	

	set @paramDefined = N' @IDChiNhanh_In uniqueidentifier,
    								@TextSearch_In nvarchar(max),
    								@IDThuocTinhHangs_In nvarchar(max),
    								@TrangThaiKho_In int,    							
    								@Where_In nvarchar(max),		
    								@CurrentPage_In int,
    								@PageSize_In int,
									@ColumnSort_In varchar(100),
    								@SortBy_In varchar(20)'

	set @sql2 = CONCAT(@tblDefined, @sql1, @sql2)
    
	----print @sql2

	exec sp_executesql @sql2, 
    		@paramDefined,
    		@IDChiNhanh_In = @IDChiNhanh,
    		@TextSearch_In = @TextSearch,
    		@IDThuocTinhHangs_In = @IDThuocTinhHangs,
    		@TrangThaiKho_In = @TrangThaiKho,   	
    		@Where_In = @Where, 	
    		@CurrentPage_In = @CurrentPage,
    		@PageSize_In = @PageSize,
			@ColumnSort_In = @ColumnSort,
			@SortBy_In = @SortBy

			
			

END");

			Sql(@"ALTER PROCEDURE [dbo].[HDTraHang_InsertTPDinhLuong]
    @ID_HoaDon [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    	--- get infor hdTra --> used to update TonLuyKe
    	declare @ID_HoaDonGoc uniqueidentifier, @ID_DonVi uniqueidentifier, @NgayLapHoaDon datetime
    	select @ID_HoaDonGoc = ID_HoaDon, @ID_DonVi = ID_DonVi, @NgayLapHoaDon= NgayLapHoaDon
    	from BH_HoaDon where ID= @ID_HoaDon
    
    	if @ID_HoaDonGoc is not null
    	begin		
    			---- get dluong at hdgoc
    			select ct.ID_ChiTietDinhLuong, ct.ID_DonViQuiDoi, ct.ID_LoHang, 
    					ct.GiaVon, ct.GiaVon_NhanChuyenHang,
    					ct.TonLuyKe, ct.TonLuyKe_NhanChuyenHang,
    				iif(ctDV.SoLuong = 0,0,ct.SoLuong/ctDV.SoLuong) as SoLuongMacDinh,
    				ct.TenHangHoaThayThe
    			into #tmpCTMua
    			from BH_HoaDon_ChiTiet ct		
    			left join (
    				---- get dv parent
    				select dlCha.ID_ChiTietDinhLuong, dlCha.SoLuong, dlCha.ID
    				from BH_HoaDon_ChiTiet dlCha
    				where dlCha.ID_HoaDon= @ID_HoaDonGoc
    				and (dlCha.ID_ChiTietDinhLuong is not null and dlCha.ID =dlCha.ID_ChiTietDinhLuong)
    			) ctDV on ct.ID_ChiTietDinhLuong = ctDV.ID_ChiTietDinhLuong
    			where ct.ID_HoaDon= @ID_HoaDonGoc 
    			and ct.ID_ChiTietDinhLuong is not null and ct.ID!=ct.ID_ChiTietDinhLuong
    
    			---- get ct hdTra
    			select ct.ID, ct.ID_ChiTietGoiDV, ct.SoLuong, ct.ChatLieu -- chatlieu:1.tra hd, 2.tra gdv
    			into #ctTra
    			from BH_HoaDon_ChiTiet ct where ct.ID_HoaDon= @ID_HoaDon
    
    			declare @CTTra_ID uniqueidentifier, @CTTra_IDChiTietGDV uniqueidentifier, @CTTra_SoLuong float, @CTTra_ChatLieu nvarchar(max)
    			declare _cur cursor
    			for
    			select ID, ID_ChiTietGoiDV, SoLuong, ChatLieu
    			from #ctTra
    			open _cur
    			fetch next from _cur
    			into @CTTra_ID, @CTTra_IDChiTietGDV, @CTTra_SoLuong, @CTTra_ChatLieu
    			while @@FETCH_STATUS = 0
    			begin
    
    				declare @countDLuong int = (select count (*) from #tmpCTMua where  ID_ChiTietDinhLuong= @CTTra_IDChiTietGDV)
    				if @countDLuong > 0
    				begin
    					---- insert dinhluong if has at ctmua
    					update BH_HoaDon_ChiTiet set ID_ChiTietDinhLuong= @CTTra_ID where ID= @CTTra_ID
    
    					insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon,ID_ChiTietDinhLuong, SoThuTu, 
    											ID_DonViQuiDoi, ID_LoHang, SoLuong, DonGia, GiaVon, ThanhTien, ThanhToan, 
    										PTChietKhau, TienChietKhau, GiaVon_NhanChuyenHang,  PTChiPhi, TienChiPhi, TienThue, An_Hien, 
    											TonLuyKe, TonLuyKe_NhanChuyenHang, ChatLieu, TenHangHoaThayThe)		
    					select NEWID(), @ID_HoaDon, @CTTra_ID, 0, ID_DonViQuiDoi, ID_LoHang, SoLuongMacDinh * @CTTra_SoLuong, 0, GiaVon, 0, 0,
    								0,0, GiaVon_NhanChuyenHang,0,0,0,0,0,0, @CTTra_ChatLieu, TenHangHoaThayThe
    					from #tmpCTMua where ID_ChiTietDinhLuong= @CTTra_IDChiTietGDV
    				end		
    				fetch next from _cur into @CTTra_ID, @CTTra_IDChiTietGDV, @CTTra_SoLuong, @CTTra_ChatLieu
    			end
    			close _cur
    			deallocate _cur
    
    		
    	end	
END");

			Sql(@"ALTER PROCEDURE [dbo].[CheckThucThu_TongSuDung]
    @ID_DoiTuong [uniqueidentifier],
    @ID_TheGiaTri [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tongthu float= 0, @tongtiendieuchinh float = 0, @tongHoanTra float
    	declare @tongkhuyenmai float= 0, @tongsudung float= 0
    	declare @dateHD datetime, @trangThaiTGT bit='0'
		declare @return bit='1'
    
    	select top 1 @dateHD=  NgayLapHoaDon, @trangThaiTGT= ChoThanhToan from  BH_HoaDon where ID = @ID_TheGiaTri
		if @trangThaiTGT is not null
			begin
				 ------ nếu TGT đã bị hủy: cho phép hủy luôn Phiếu thu của TGT (dùng khi bị lỗi hủy Thẻ nhưng không hủy phiếu thu)----
				 --- sum sotien dieuchinh den thoidiem hientai
					set @tongtiendieuchinh = (select top 1 TongTienThue as TongThu1
					from BH_HoaDon
					where LoaiHoaDon= 23 and ChoThanhToan=0
					and ID_DoiTuong= @ID_DoiTuong
					and NgayLapHoaDon < @dateHD
					order by NgayLapHoaDon desc)
    
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
    				and qhd.NgayLapHoaDon < @dateHD -- chi so sanh den phut
    				group by hd.ID_DoiTuong

					-- get gtrikhuyenmai (vi giatri dc sử dụng của thẻ = gtri khuyến mại + phải thanh toán)
					select 
    					@tongkhuyenmai = sum(hd.TongChietKhau) + sum (TongGiamGia)
    				from BH_HoaDon hd 
    				where hd.ID_DoiTuong= @ID_DoiTuong
    				and hd.ChoThanhToan is not null
    				and hd.LoaiHoaDon= 22
    				and hd.NgayLapHoaDon  < @dateHD 
    				group by hd.ID_DoiTuong
    
    				-- get all tongsudung
    				select 
    					@tongsudung= sum(qct.TienThu)
    				from Quy_HoaDon qhd
    				join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    				where qct.ID_DoiTuong= @ID_DoiTuong    	
    				and qhd.TrangThai='1'
    				and qhd.LoaiHoaDon = 11
    				and qct.HinhThucThanhToan = 4

					---- get hoantraTGT ---
    				select 
    					@tongHoanTra = sum(hd.TongTienHang) 
    				from BH_HoaDon hd 
    				where hd.ID_DoiTuong= @ID_DoiTuong
    				and hd.ChoThanhToan is not null
    				and hd.LoaiHoaDon= 32 and NgayLapHoaDon > @dateHD
					group by hd.ID_DoiTuong
			
       
    				if isnull(@tongtiendieuchinh,0) +  isnull(@tongthu,0) + isnull(@tongkhuyenmai,0)  < isnull(@tongsudung,0) + @tongHoanTra
    					set @return='0'
			end
			
		
    	select @return as Exist
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListTheGiaTri]
    @IDDonVis [nvarchar](max),
	@LoaiHoaDons varchar(20) = null, --- 
    @TextSearch [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TrangThais [nvarchar](10),
    @MucNapFrom [float],
    @MucNapTo [float],
    @KhuyenMaiFrom [float],
    @KhuyenMaiTo [float],
    @KhuyenMaiLaPTram [int],
    @ChietKhauFrom [float],
    @ChietKhauTo [float],
    @ChietKhauLaPTram [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;

	declare @tblLoaiHD table(LoaiHoaDon int)
	insert into @tblLoaiHD
	select Name from dbo.splitstring(@LoaiHoaDons)
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @MucNapMax float= (select max(TongChiPhi) from BH_HoaDon where ChoThanhToan= 0 and LoaiHoaDon= 22 );
		set @MucNapMax = isnull(@MucNapMax,0)
    	if @MucNapTo is null
    		set @MucNapTo= @MucNapMax
    	if @KhuyenMaiTo is null
    		set @KhuyenMaiTo = @MucNapMax
    	if @ChietKhauTo is null
    		set @ChietKhauTo= @MucNapMax;
    	
    	with data_cte
    	as
    	(
    
    	select tblThe.ID,
			tblThe.MaHoaDon,
			tblThe.NgayLapHoaDon,
			tblThe.LoaiHoaDon,
			tblThe.NgayTao,
    		tblThe.TongChiPhi as MucNap,    		
    		tblThe.TongTienHang as TongTienNap,
    		tblThe.TongTienThue as SoDuSauNap,
			tblThe.TongGiamGia  as ChietKhauVND,    	
    		ISNULL(tblThe.DienGiai,'') as GhiChu,
    		tblThe.NguoiTao,
    		ISNULL(tblThe.ID_DoiTuong,'00000000-0000-0000-0000-000000000000') as ID_DoiTuong,
    		tblThe.PhaiThanhToan,
    		tblThe.MaDoiTuong as MaKhachHang,
    		tblThe.TenDoiTuong as TenKhachHang,
    		------tblThe.DienThoai as SoDienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng ---
    		tblThe.DiaChi as DiaChiKhachHang,
    		tblThe.ChoThanhToan,
    		tblThe.ChietKhauPT,
    		tblThe.KhuyenMaiPT,
			tblThe.ID_DonVi,		
			isnull(soquy.TongPhiNganHang,0) as KhuyenMaiVND, -- muontamtruong (trừ phí ngân hàng khi tính hoa hồng NV)
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienMat,0),-ISNULL(soquy.TienMat,0))  as TienMat,
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienPOS,0),-ISNULL(soquy.TienPOS,0))  as TienATM,
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienCK,0),-ISNULL(soquy.TienCK,0))  as TienGui,
    		iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienThu,0),-ISNULL(soquy.TienThu,0)) as KhachDaTra,
			--iif(tblThe.LoaiHoaDon= 22,  tblThe.PhaiThanhToan - ISNULL(soquy.TienThu,0),-ISNULL(soquy.TienThu,0) + tblThe.PhaiThanhToan) as ConNo1,
    		dv.TenDonVi,
    		dv.SoDienThoai as DienThoaiChiNhanh,
    		dv.DiaChi as DiaChiChiNhanh
    	from
    		(
    		select *
    		from
    			(select hd.ID, 
						hd.MaHoaDon,
						hd.LoaiHoaDon,
						hd.NgayLapHoaDon,
						hd.ID_DonVi,
						hd.ID_DoiTuong,
						hd.ID_NhanVien,
						hd.NguoiTao,
						hd.NgayTao,
						hd.TongChiPhi,
						hd.TongTienHang,
						hd.TongChietKhau,
						hd.TongGiamGia,
						hd.TongTienThue,
						hd.ChoThanhToan,		
						hd.PhaiThanhToan,						
						hd.DienGiai,
						----- Loai 32: hoanthe (TongChietKhau = % PhiHoanThe)
						iif(hd.LoaiHoaDon= 32,hd.TongChietKhau, iif(hd.TongChiPhi=0,0, hd.TongGiamGia/hd.TongChiPhi * 100)) as ChietKhauPT,
						iif(hd.TongChiPhi=0,0, hd.TongChietKhau/hd.TongChiPhi * 100) as KhuyenMaiPT,
    					dt.MaDoiTuong, dt.TenDoiTuong,
    					dt.DienThoai, 
    					dt.DiaChi,
    					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai --,
    					--NhanVienThucHien
    				from BH_HoaDon hd
    				join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			
    				where exists (select name from dbo.splitstring(@IDDonVis) dv where hd.ID_DonVi= dv.Name)	
    				and hd.LoaiHoaDon in (select LoaiHoaDon from @tblLoaiHD)
    				and hd.TongChiPhi >= @MucNapFrom and hd.TongChiPhi <= @MucNapTo -- mucnap
    				and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon <=@ToDate
    					AND ((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    						or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    						or dt.DienThoai like '%'+b.Name+'%'			
    					or hd.MaHoaDon like '%' +b.Name +'%' 
    						or hd.NguoiTao like '%' +b.Name +'%' 				
    						)=@count or @count=0)	
    			) the
    			where IIF(@KhuyenMaiLaPTram = 1, the.TongChietKhau, the.KhuyenMaiPT) >= @KhuyenMaiFrom -- khuyenmai
    				and IIF(@KhuyenMaiLaPTram = 1, the.TongChietKhau, the.KhuyenMaiPT) <= @KhuyenMaiTo
    				and IIF(@ChietKhauLaPTram = 1, the.TongGiamGia, the.ChietKhauPT) >= @ChietKhauFrom -- giamgia
    				and IIF(@ChietKhauLaPTram = 1, the.TongGiamGia, the.ChietKhauPT) <= @ChietKhauTo
    				and the.TrangThai like @TrangThais 
    		) tblThe		
    	join DM_DonVi dv on tblThe.ID_DonVi= dv.ID
    	left join ( select quy.ID_HoaDonLienQuan, 
    					sum(quy.TienThu) as TienThu,
    					sum(quy.TienMat) as TienMat,
    					sum(quy.TienPOS) as TienPOS,
    					sum(quy.TienCK) as TienCK,
						sum(quy.TongPhiNganHang) as TongPhiNganHang ----- 
    				from
    				(
    					select qct.ID_HoaDonLienQuan,
							iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) as TienThu,
    						iif(qct.HinhThucThanhToan = 1, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0) as TienMat,					
    						iif(qct.HinhThucThanhToan = 2, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0) as TienPOS,
    						iif(qct.HinhThucThanhToan = 3, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0)  as TienCK,
    						iif(qct.HinhThucThanhToan = 2,iif(qct.LaPTChiPhiNganHang='0',qct.ChiPhiNganHang, 
							qct.TienThu * qct.ChiPhiNganHang/100),0) as TongPhiNganHang ---- apply pos					
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					where qhd.TrangThai= 1 or qhd.TrangThai is null
    				) quy 
    				group by quy.ID_HoaDonLienQuan) soquy on tblThe.ID= soquy.ID_HoaDonLienQuan
    	),
    	count_cte
    	as (
    		select count(ID) as TotalRow,
    			CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    			sum(MucNap) as TongMucNapAll,
    			sum(KhuyenMaiVND) as TongKhuyenMaiAll,
    			sum(TongTienNap) as TongTienNapAll,			
    			sum(ChietKhauVND) as TongChietKhauAll,
    			sum(SoDuSauNap) as SoDuSauNapAll,
    			sum(PhaiThanhToan) as PhaiThanhToanAll,			
    			sum(TienMat) as TienMatAll,
    			sum(TienATM) as TienATMAll,
    			sum(TienGui) as TienGuiAll,
    			sum(KhachDaTra) as KhachDaTraAll
    			from data_cte
    		),
			tView
			as
			(
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
			)
			select *,
				ISNULL(qtCN.GiaTriTatToan,0) as GiaTriTatToan,
				ISNULL(nvThucHien.NhanVienThucHien,'') as NhanVienThucHien,
			 	iif(hd.ChoThanhToan is null,0, hd.PhaiThanhToan - hd.KhachDaTra - ISNULL(qtCN.GiaTriTatToan,0)) as ConNo
			from tView hd
			left join (
    				Select distinct
    					(
    						Select distinct nv.TenNhanVien + ',' AS [text()]
    						From dbo.BH_NhanVienThucHien th
    						join dbo.NS_NhanVien nv on th.ID_NhanVien = nv.ID
    						where th.ID_HoaDon= nvth.ID_HoaDon
    						For XML PATH ('')
    					) NhanVienThucHien, nvth.ID_HoaDon
    					From dbo.BH_NhanVienThucHien nvth
    					) nvThucHien on hd.ID = nvThucHien.ID_HoaDon
			left join
			(
				select hd.ID_HoaDon,
					sum(hd.TongTienHang) as GiaTriTatToan
				from BH_HoaDon hd
				where hd.ChoThanhToan='0'
				and LoaiHoaDon= 42
				group by hd.ID_HoaDon
			) qtCN on hd.ID= qtCN.ID_HoaDon

END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoGoiDichVu_BanDoiTra]
 --declare 
 @IDChiNhanhs [nvarchar](max) ='',
    @FromDate [datetime]='2024-01-01',
    @ToDate [datetime]='2025-01-01',
    @TxtMaHD [nvarchar](max)='KJ11301',
    @TxtDVMua [nvarchar](max)='',
    @TxtDVDoi [nvarchar](max)='',
    @ThoiHanSuDung [nvarchar](max)='',
    @CurrentPage [int]= 0,
    @PageSize [int]=50
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	if isnull(@IDChiNhanhs,'')!=''
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs)
    	else set @IDChiNhanhs =''
    	
    	if isnull(@TxtMaHD,'') !='' set @TxtMaHD = concat(N'%', @TxtMaHD,'%') else set @TxtMaHD ='%%'
    	if isnull(@TxtDVMua,'') !='' set @TxtDVMua = concat(N'%', @TxtDVMua,'%') else set @TxtDVMua ='%%'
    	if isnull(@TxtDVDoi,'') !='' set @TxtDVDoi = concat(N'%', @TxtDVDoi,'%') else set @TxtDVDoi ='%%'
    
    
    
    	------- cthd mua goc ----
    	select hd.*,
    		ctm.ID as IDChiTietHD,
    		ctm.ID_DonViQuiDoi,
    		ctm.SoLuong,
    		ctm.ThanhTien
    	into #cthdMuaGoc
    	from
    	(
    		select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.ID_DoiTuong, hd.LoaiHoaDon	
    		from BH_HoaDon hd
    		where hd.ChoThanhToan='0'
    		and hd.LoaiHoaDon= 19
    		and hd.ID_HoaDon is null ----- chỉ get gdvMua (không đổi) ---
    		and hd.NgayLapHoaDon between @FromDate and @ToDate
    		and (@IDChiNhanhs ='' or exists (select id from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
    	)hd 
    	join BH_HoaDon_ChiTiet ctm on hd.ID = ctm.ID_HoaDon
    	where (ctm.ID_ChiTietDinhLuong is null or ctm.ID_ChiTietDinhLuong = ctm.ID) --- khong lay tpdluong
    	and (ctm.ID_ParentCombo is null or ctm.ID_ParentCombo != ctm.ID) --- khong lay combo (parent)
    
    
    	------- cthdDoi ----
    	select 
    		hd.*,
    		ctDoi.ID as IDChiTietHD,
    		ctDoi.ID_DonViQuiDoi,
    		ctDoi.SoLuong,		
    		ctDoi.ThanhTien
    	into #cthdDoi
    	from
    	(
    	select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.ID_DoiTuong, hd.ID_HoaDon, hd.LoaiHoaDon
    	from BH_HoaDon hd
    	where hd.ChoThanhToan='0'
    	and hd.LoaiHoaDon= 19
    	and hd.ID_HoaDon is not null
    	and hd.NgayLapHoaDon between @FromDate and @ToDate
		and (@IDChiNhanhs ='' or exists (select id from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
    	)hd
    	join BH_HoaDon_ChiTiet ctDoi on hd.ID = ctDoi.ID_HoaDon
    	where (ctDoi.ID_ChiTietDinhLuong is null or ctDoi.ID_ChiTietDinhLuong = ctDoi.ID) --- khong lay tpdluong
    	and (ctDoi.ID_ParentCombo is null or ctDoi.ID_ParentCombo != ctDoi.ID) --- khong lay combo (parent)
    
    	---- cthdTra----
    	select 
    		hd.*,
    		ctTra.ID as IDChiTietHD,
    		ctTra.ID_DonViQuiDoi,
    		ctTra.SoLuong,
    		ctTra.ThanhTien
    	into #cthdTra
    	from
    	(
    		select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.ID_DoiTuong, hd.ID_HoaDon, hd.LoaiHoaDon	
    		from BH_HoaDon hd
    		where hd.ChoThanhToan='0'
    		and hd.LoaiHoaDon= 6
    		and hd.ID_HoaDon is not null
    		and hd.NgayLapHoaDon between @FromDate and @ToDate
			and (@IDChiNhanhs ='' or exists (select id from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
    	)hd
    	join BH_HoaDon_ChiTiet ctTra on hd.ID = ctTra.ID_HoaDon
    	where (ctTra.ID_ChiTietDinhLuong is null or ctTra.ID_ChiTietDinhLuong = ctTra.ID) --- khong lay tpdluong
    	and (ctTra.ID_ParentCombo is null or ctTra.ID_ParentCombo != ctTra.ID) --- khong lay combo (parent)

		
    
    		; with ctDoiTra
    			as(
    			------ join tra - doi ----
    			select 
    				tra.ID as GDVTra_ID,
    				tra.ID_HoaDon as GDVTra_IDHoaDonGoc,
    				tra.ID_DoiTuong,
    				tra.IDChiTietHD as GDVTra_IDChiTietHD,
    				tra.MaHoaDon as  GDVTra_MaHoaDon,
    				tra.NgayLapHoaDon as GDVTra_NgayLapHoaDon,		
    				tra.ID_DonViQuiDoi as GDVTra_ID_DonViQuiDoi,
    				tra.SoLuong as SoLuongTra,
    				tra.ThanhTien as GiaTriTra,
    				
    				doi.ID as GDVDoi_ID,
    				doi.IDChiTietHD as GDVDoi_IDChiTietHD,
    				doi.MaHoaDon as GDVDoi_MaHoaDon,
    				doi.NgayLapHoaDon GDVDoi_NgayLapHoaDon,	
    				doi.ID_DonViQuiDoi as GDVDoi_ID_DonViQuiDoi,
    				isnull(doi.SoLuong,0) as SoLuongDoi,
    				isnull(doi.ThanhTien,0) as GiaTriDoi				
    			from #cthdTra tra
    			left join #cthdDoi doi on tra.ID = doi.ID_HoaDon 
    			),
    			tblSumDoiTra
    			as
    			(
    				select 
    					GDVDoi_ID,
    					GDVTra_ID,
    					sum(GiaTriTra) as TongTra,
    					sum(GiaTriDoi) as TongDoi
    				from
    				(
    					select 
    						GDVDoi_ID,
    						GDVTra_ID,
    						iif(RnTra >1, 0, GiaTriTra) as GiaTriTra,
    						iif(RnDoi >1, 0, GiaTriDoi) as GiaTriDoi
    					from
    					(
    						----- trả 1 đổi N - hoặc trả N đổi 1 --> chỉ lấy dòng đầu tiên theo idchitiet --
    						select 
    							GDVDoi_ID,
    							GDVTra_ID,
    							GiaTriTra,
    							GiaTriDoi,
    							ROW_NUMBER() over (partition by GDVTra_IDChiTietHD order by GDVTra_IDChiTietHD) as RnTra,
    							ROW_NUMBER() over (partition by GDVDoi_IDChiTietHD order by GDVDoi_IDChiTietHD) as RnDoi
    						 from ctDoiTra
    					)tblRn
    				)tblGr
    				group by GDVDoi_ID,	GDVTra_ID
    			),
    			tblSumMuaGoc
    			as
    			(
    				select ID,
    					sum(ThanhTien) as TongMua
    				from #cthdMuaGoc
    				group by ID
    			),
    			tblUnion as
    			(
    				------ union doitra - muagoc --
    					select 
    						ctDoiTra.GDVTra_ID,
    						ctDoiTra.GDVTra_IDHoaDonGoc,
    						ctDoiTra.GDVTra_IDChiTietHD,						
    						ctDoiTra.ID_DoiTuong,
    						ctDoiTra.GDVTra_MaHoaDon,
    						ctDoiTra.GDVTra_NgayLapHoaDon,	
    						ctDoiTra.GDVTra_ID_DonViQuiDoi,					
    						ctDoiTra.SoLuongTra,
    						ctDoiTra.GiaTriTra,
    
    						ctDoiTra.GDVDoi_ID,
    						ctDoiTra.GDVDoi_IDChiTietHD,
    						ctDoiTra.GDVDoi_MaHoaDon,
    						ctDoiTra.GDVDoi_NgayLapHoaDon,
    						ctDoiTra.GDVDoi_ID_DonViQuiDoi,
    						ctDoiTra.SoLuongDoi,
    						ctDoiTra.GiaTriDoi,
    
    						tblSumDoiTra.TongDoi - tblSumDoiTra.TongTra as GiaTriChenhLech
    					from ctDoiTra
    					left join tblSumDoiTra on ctDoiTra.GDVDoi_ID = tblSumDoiTra.GDVDoi_ID and ctDoiTra.GDVTra_ID = tblSumDoiTra.GDVTra_ID
    
    					union all
    
    					select
    						null as GDVTra_ID,
    						null as GDVTra_IDHoaDonGoc,
    						null as GDVTra_IDChiTietHD,
    						mua.ID_DoiTuong,
    						'' as GDVTra_MaHoaDon,
    						null as GDVTra_NgayLapHoaDon,				
    						null as GDVTra_ID_DonViQuiDoi,
    						0 as SoLuongTra,
    						0 as GiaTriTra,				
    
    						mua.ID as GDVDoi_ID,
    						mua.IDChiTietHD as GDVDoi_IDChiTietHD,
    						mua.MaHoaDon as GDVDoi_MaHoaDon,
    						mua.NgayLapHoaDon as GDVDoi_NgayLapHoaDon,					
    						mua.ID_DonViQuiDoi as GDVDoi_ID_DonViQuiDoi,
    						mua.SoLuong as SoLuongDoi,
    						mua.ThanhTien as GiaTriDoi,
    						tblSum.TongMua as GiaTriChenhLech						
    					from #cthdMuaGoc mua
    					join tblSumMuaGoc tblSum on mua.ID = tblSum.ID
    				),
    				tblLast
    				as(
						select *
						from
						(
    					select 
    						dt.MaDoiTuong,
    						dt.TenDoiTuong,
    						tbl.*,
    						isnull(qdTra.MaHangHoa,'') as MaHangHoa,
    						hhTra.TenHangHoa,
    						qdTra.TenDonViTinh,
    
    						qdMua.MaHangHoa as GDVDoi_MaHangHoa,
    						hhMua.TenHangHoa as GDVDoi_TenHangHoa,
    						qdMua.TenDonViTinh as GDVDoi_TenDonViTinh,
    						gdvGoc.MaHoaDon as GDVTra_MaChungTuGoc,
    
    						iif(GDVTra_NgayLapHoaDon is null, GDVDoi_NgayLapHoaDon,GDVTra_NgayLapHoaDon) as NgayLapHoaDon,
    						ROW_NUMBER() over (partition by GDVTra_IDChiTietHD order by GDVTra_IDChiTietHD) as RnTra,
    						ROW_NUMBER() over (partition by GDVDoi_IDChiTietHD order by GDVDoi_IDChiTietHD) as RnDoi,
    						-----nếu chỉ đổi, giá trị chênh lệch chỉ get 1 dòng đầu tiên của GDV đổi --> used to xuất excel ---
    						ROW_NUMBER() over (partition by GDVDoi_ID order by GDVDoi_ID) as RnGDV_Doi
    
    					from tblUnion tbl
    					left join BH_HoaDon gdvGoc on tbl.GDVTra_IDHoaDonGoc = gdvGoc.ID
    					join DM_DoiTuong dt on tbl.ID_DoiTuong = dt.ID
    					left join DonViQuiDoi qdTra on tbl.GDVTra_ID_DonViQuiDoi = qdTra.ID
    					left join DM_HangHoa hhTra on qdTra.ID_HangHoa = hhTra.ID
    					left join DonViQuiDoi qdMua on tbl.GDVDoi_ID_DonViQuiDoi = qdMua.ID
    					left join DM_HangHoa hhMua on qdMua.ID_HangHoa = hhMua.ID
    					where (@TxtMaHD ='%%' 
    						or MaDoiTuong like @TxtMaHD
    						or TenDoiTuong like @TxtMaHD
    						or TenDoiTuong_KhongDau like @TxtMaHD	
    						or GDVTra_MaHoaDon like @TxtMaHD		
    						or GDVDoi_MaHoaDon like @TxtMaHD		
    						or gdvGoc.MaHoaDon  like @TxtMaHD
    						)
    						and (
    							@TxtDVMua ='%%' 
    							or qdTra.MaHangHoa like @TxtDVMua
    							or hhTra.TenHangHoa like @TxtDVMua
    							or hhTra.TenHangHoa_KhongDau like @TxtDVMua	
    
    							or qdMua.MaHangHoa like @TxtDVMua
    							or hhMua.TenHangHoa like @TxtDVMua
    							or hhMua.TenHangHoa_KhongDau like @TxtDVMua		
    						)
							)tblFilterDouple
							where (RnTra < 2 or RnDoi < 2 )
    					),
    					count_cte
    					as(
    						select count(*) as ToTalRow
    						from tblLast
    					)
    					
    					select
    						ToTalRow,
    						tblLast.NgayLapHoaDon,
    						MaDoiTuong,
    						TenDoiTuong,
    						GDVTra_ID,
    						GDVTra_IDChiTietHD,						
    						GDVTra_MaHoaDon,
    						GDVTra_NgayLapHoaDon,		
    						GDVTra_MaChungTuGoc,
    
    						GDVDoi_ID,
    						GDVDoi_IDChiTietHD,
    						GDVDoi_MaHoaDon,
    						GDVDoi_NgayLapHoaDon,
    						GiaTriChenhLech,
    						----- neu chỉ đổi: get chenhlec from GDVDoi, else: get from Tra ---
    						iif(GDVTra_ID is null, iif(RnGDV_Doi > 1, 0, GiaTriChenhLech), iif(RnTra > 1, 0, GiaTriChenhLech)) as GiaTriChenhLechExcel,
    						RnTra,
    						RnDoi,
    						RnGDV_Doi,
    
    						iif(RnTra > 1, '', MaHangHoa) as MaHangHoa,
    						iif(RnTra > 1, '', TenHangHoa) as TenHangHoa,
    						iif(RnTra > 1, '', TenDonViTinh) as TenDonViTinh,
    						iif(RnTra > 1, 0, SoLuongTra) as SoLuongTra,
    						iif(RnTra > 1, 0, GiaTriTra) as GiaTriTra,
    
    						iif(RnDoi > 1, '', GDVDoi_MaHangHoa) as GDVDoi_MaHangHoa,
    						iif(RnDoi > 1, '', GDVDoi_TenHangHoa) as GDVDoi_TenHangHoa,
    						iif(RnDoi > 1, '', GDVDoi_TenDonViTinh) as GDVDoi_TenDonViTinh,
    						iif(RnDoi > 1, 0, SoLuongDoi) as SoLuongDoi,
    						iif(RnDoi > 1, 0, GiaTriDoi) as GiaTriDoi
    						
    					from tblLast 			
    					cross join count_cte
    					order by tblLast.NgayLapHoaDon desc
    					OFFSET (@CurrentPage* @PageSize) ROWS
    					FETCH NEXT @PageSize ROWS ONLY
    		
    
    	drop table #cthdDoi
    	drop table #cthdTra
    	drop table #cthdMuaGoc
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetNhatKySuDung_GDV]
 --declare  
	@IDChiNhanhs [nvarchar](max) = '',
    @IDCustomers [nvarchar](max) = null,  
	@TextSearch nvarchar(max) = '',
	@DateFrom datetime = null,
	@DateTo datetime = null,
	@LoaiHoaDons [nvarchar](max) = null,
    @CurrentPage [int] = 0,
    @PageSize [int] = 50
AS
BEGIN
    SET NOCOUNT ON;
    	declare @sql nvarchar(max) ='', @where nvarchar(max), @paramDefined nvarchar(max)
    	declare @tblDefined nvarchar(max)= N' declare @tblChiNhanh table(ID uniqueidentifier)
    								declare @tblCus table(ID uniqueidentifier)
    								declare @tblCar table(ID uniqueidentifier)'
    
    	set @where = N' where 1 = 1 and hd.LoaiHoaDon in (1,2) and hd.ChoThanhToan = 0  
						and (ct.ID_ChiTietDinhLuong= ct.id OR ct.ID_ChiTietDinhLuong IS NULL) 
						and (ct.ID_ParentCombo != ct.ID OR ct.ID_ParentCombo IS NULL)'
    
    	if isnull(@CurrentPage,'') =''
    		set @CurrentPage = 0
    	if isnull(@PageSize,'') =''
    		set @PageSize = 20

		if isnull(@LoaiHoaDons,'') !=''
			begin
				if @LoaiHoaDons = 19 ---- hoadon dudung GDV
    				set @where = CONCAT(@where, N' and ct.ChatLieu = ''4''')
			end

    	if isnull(@IDChiNhanhs,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblChiNhanh cn where ID_DonVi = cn.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ;')
    		end
    	if isnull(@IDCustomers,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblCus select name from dbo.splitstring(@IDCustomers_In) ;')
    		end
    	
    	if isnull(@DateFrom,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and hd.NgayLapHoaDon > @DateFrom_In')
    			
    		end
		if isnull(@DateTo,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and hd.NgayLapHoaDon < @DateTo_In')    			
    		end

    	if isnull(@TextSearch,'') !=''
    		begin
    			set @where = CONCAT(@where , N' and (hd.MaHoaDon like N''%'' + @TextSearch_In + ''%''  
							--or hdgoc.MaHoaDon like N''%'' + @TextSearch_In + ''%''  
							or dt.MaDoiTuong like N''%'' + @TextSearch_In + ''%'' or  dt.TenDoiTuong like N''%'' + @TextSearch_In + ''%'' 
							or dt.TenDoiTuong_KhongDau like N''%'' + @TextSearch_In + ''%'' or dt.DienThoai like N''%'' + @TextSearch_In + ''%''
							or hh.TenHangHoa like N''%'' + @TextSearch_In + ''%'' or  hh.TenHangHoa_KhongDau like N''%'' + @TextSearch_In + ''%''
							or qd.MaHangHoa like N''%'' + @TextSearch_In + ''%'')' )
    			
    		end
    	
    	set @sql = CONCAT(@tblDefined, @sql, N'
    		;with data_cte
    as (
		SELECT ct.ID as ID_ChiTietGoiDV,
			hd.MaHoaDon, 
			hd.NgayLapHoaDon,
			hd.ID_DoiTuong, 
			dt.MaDoiTuong,
			dt.TenDoiTuong,
    		qd.MaHangHoa,
			isnull(qd.ThuocTinhGiaTri,'''') as ThuocTinh_GiaTri,
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='''', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoa,
    		hh.TenHangHoa_KhongDau,		
			hh.GhiChu as GhiChuHH,
			hh.LaHangHoa,
			case when hh.LaHangHoa = 1 then ''0'' else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
    		Case when hh.LaHangHoa=1 then ''0'' else ISNULL(hh.ChiPhiTinhTheoPT,''0'') end as LaPTPhiDichVu,
			isnull(hh.ID_NhomHang,''00000000-0000-0000-0000-000000000000'') as ID_NhomHangHoa,
			ISNULL(qd.LaDonViChuan,0) as LaDonViChuan, 
			CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
			isnull(hh.QuanLyTheoLoHang,''0'') as QuanLyTheoLoHang,
			ISNULL(hh.ChietKhauMD_NVTheoPT,''1'') as ChietKhauMD_NVTheoPT,
			ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
			CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
    		lo.MaLoHang, 
			lo.NgaySanXuat, 
			lo.NgayHetHan,
			qd.TenDonViTinh,
			qd.ID_HangHoa,
    		ct.SoLuong as SoLuongMua,
			ct.ID_DonViQuiDoi,
			ct.ID_LoHang,
			ct.DonGia - ct.TienChietKhau as GiaBan,	 ---- lay sau CK
			ct.TienChietKhau,
			ct.ThoiGianBaoHanh,
			ct.LoaiThoiGianBH,
			ct.GhiChu,
			ct.SoThuTu,
			--isnull(gv.GiaVon,0) as GiaVon,
			--isnull(tk.TonKho,0) as TonKho,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			nhomhh.TenNhomHangHoa,
			ct.ID_ChiTietGoiDV as ID_ChiTietGoiDVGoc
    	FROM BH_HoaDon_ChiTiet ct
    	join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.id
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID		
		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		left join DM_NhomHangHoa nhomhh on hh.ID_NhomHang = nhomhh.ID
		left join DM_LoHang lo on ct.ID_LoHang = lo.ID
		--left join DM_GiaVon gv on ct.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and gv.ID_DonVi = hd.ID_DonVi
		--left join DM_HangHoa_TonKho tk on ct.ID_DonViQuiDoi = tk.ID_DonViQuyDoi and tk.ID_DonVi = hd.ID_DonVi
    	      
    	', @where, 
    		'),
    		count_cte
    		as (
    			select count(ID_ChiTietGoiDV) as TotalRow,
    				CEILING(COUNT(ID_ChiTietGoiDV) / CAST(@PageSize_In as float ))  as TotalPage,
    				sum(SoLuongMua) as TongSoLuong
    				-- sum(TongChietKhau) as TongHoaHong			
    			from data_cte
    		)
			Select dt.*,
				cte.*,
				hdXMLOut.HDCT_NhanVien as NhanVienThucHien,
    			CT_ChietKhauNV.TongChietKhau
			From data_cte dt
			cross join count_cte cte			
			left join 
    			(Select distinct hdXML.ID_ChiTietGoiDV,
    					(
    					select distinct (nv.TenNhanVien) +'', ''  AS [text()]
    					from data_cte ct2
    					left join BH_NhanVienThucHien nvth on ct2.ID_ChiTietGoiDV = nvth.ID_ChiTietHoaDon
    					left join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    					where ct2.ID_ChiTietGoiDV = hdXML.ID_ChiTietGoiDV 
    					For XML PATH ('''')
    				) HDCT_NhanVien
    			from data_cte hdXML) hdXMLOut on dt.ID_ChiTietGoiDV = hdXMLOut.ID_ChiTietGoiDV
    		 left join 
    			(select ct3.ID_ChiTietGoiDV, 
					SUM(isnull(nvth2.TienChietKhau,0)) as TongChietKhau 
					from data_cte ct3
    				left join BH_NhanVienThucHien nvth2 on ct3.ID_ChiTietGoiDV = nvth2.ID_ChiTietHoaDon
    				group by ct3.ID_ChiTietGoiDV
				) CT_ChietKhauNV on CT_ChietKhauNV.ID_ChiTietGoiDV = dt.ID_ChiTietGoiDV 
				left join BH_HoaDon_ChiTiet ctgoc on dt.ID_ChiTietGoiDVGoc = ctgoc.ID 
						and (ctgoc.ID_ChiTietDinhLuong= ctgoc.id OR ctgoc.ID_ChiTietDinhLuong IS NULL) 
						and (ctgoc.ID_ParentCombo != ctgoc.ID OR ctgoc.ID_ParentCombo IS NULL)
				left join BH_HoaDon hdgoc on ctgoc.ID_HoaDon = hdgoc.ID and hdgoc.LoaiHoaDon in (1,2) and hdgoc.ChoThanhToan = 0
    			order by dt.NgayLapHoaDon desc
    			OFFSET (@CurrentPage_In * @PageSize_In) ROWS
    			FETCH NEXT @PageSize_In ROWS ONLY 
			')
    
    		
    
    		set @paramDefined =N'
    			@IDChiNhanhs_In nvarchar(max),
    			@IDCustomers_In nvarchar(max),
				@TextSearch_In nvarchar(max),
				@DateFrom_In datetime,
				@DateTo_In datetime,
				@LoaiHoaDons_In nvarchar(max),
    			@CurrentPage_In int,
    			@PageSize_In int'
    
    		exec sp_executesql @sql, 
    		@paramDefined,
    		@IDChiNhanhs_In = @IDChiNhanhs,
    		@IDCustomers_In = @IDCustomers,
			@TextSearch_In = @TextSearch,
			@DateFrom_In = @DateFrom,
			@DateTo_In = @DateTo,
			@LoaiHoaDons_In = @LoaiHoaDons,
    		@CurrentPage_In = @CurrentPage,
    		@PageSize_In = @PageSize
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetAllChiTietHoaDon_afterTraHang] 
 @IDChiNhanhs [nvarchar](max) ='a50ef1b8-bfb6-45cc-8977-bdb178d6f0ed',
    @LoaiHoaDon [int] = 19,
    @DateFrom [datetime] ='2024-01-01',
    @DateTo [datetime]='2025-01-01',
    @TextSearch [nvarchar](max)='KH0000032',
    @CurrentPage [int] = 0,
    @PageSize [int] = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    		if isnull(@CurrentPage,'') ='' set @CurrentPage = 0			
    		if isnull(@PageSize,'') ='' set @PageSize = 30
    		
    		if isnull(@DateFrom,'') ='' set @DateFrom = '2016-01-01'	
    		if isnull(@DateTo,'') ='' set @DateTo = DATEADD(day, 1, getdate())				
    		else set @DateTo = DATEADD(day, 1, @DateTo)
    		
    		DECLARE @tblChiNhanh table (ID uniqueidentifier primary key)
    		if isnull(@IDChiNhanhs,'') !=''
    			insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs)		
    		else
    			set @IDChiNhanhs =''
    
    		DECLARE @tblSearch TABLE (Name [nvarchar](max))
    		DECLARE @count int
    		INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!=''
    		select @count =  (Select count(*) from @tblSearch)
    
    		------ getHD -----
    			select 		
    			hd.ID,
    			hd.ID_DoiTuong,
    			hd.TongThanhToan
    		into #hd
    		from BH_HoaDon hd
    		where hd.ChoThanhToan=0
    		and hd.LoaiHoaDon = @LoaiHoaDon
    		and hd.NgayLapHoaDon between @DateFrom and @DateTo	
    		and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
    
    		------ get ctMua
    		select ct.ID, ct.SoLuong, ct.ID_DonViQuiDoi, ct.ID_LoHang
    		into #ctMua
    		from BH_HoaDon_ChiTiet ct
    		where exists (select id from #hd where ct.ID_HoaDon = #hd.ID)
    		and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID) ---- chi get hanghoa + dv
    		and (ct.ID_ParentCombo is null OR ct.ID_ParentCombo != ct.ID)  ---- khong get parent, get TP combo
    
    		
    			select 
    				hd.ID,
    				hd.MaHoaDon,
    				hd.LoaiHoaDon,
    				hd.NgayLapHoaDon,   						
    				hd.ID_DoiTuong,	
    				hd.ID_HoaDon,
    				hd.ID_ViTri,
    				hd.ID_BangGia,
    				hd.ID_NhanVien,
    				hd.ID_DonVi,
    				hd.ID_Xe,
    				hd.ID_PhieuTiepNhan,
    				hd.ID_BaoHiem,
    				hd.NguoiTao,	
    				hd.DienGiai,	
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				iif(hd.TongThanhToan =0 or hd.TongThanhToan is null,  hd.PhaiThanhToan, hd.TongThanhToan) as TongThanhToan,
    				ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
    				ISNULL(hd.KhuyeMai_GiamGia, 0) as KhuyeMai_GiamGia,
    				ISNULL(hd.TongTienHang, 0) as TongTienHang,
    				ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
    				isnull(hd.TongChietKhau,0) as  TongChietKhau,
    				ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich,							
    				ISNULL(hd.TongTienThue, 0) as TongTienThue,						
    				isnull(hd.PTThueHoaDon,0) as  PTThueHoaDon,
    				ISNULL(hd.TongThueKhachHang, 0) as TongThueKhachHang,	
    				isnull(hd.TongTienThueBaoHiem,0) as  TongTienThueBaoHiem,
    				isnull(hd.TongTienBHDuyet,0) as  TongTienBHDuyet,
    				isnull(hd.SoVuBaoHiem,0) as  SoVuBaoHiem,
    				isnull(hd.PTThueBaoHiem,0) as  PTThueBaoHiem,
    				isnull(hd.KhauTruTheoVu,0) as  KhauTruTheoVu,
    				isnull(hd.GiamTruBoiThuong,0) as  GiamTruBoiThuong,
    				isnull(hd.PTGiamTruBoiThuong,0) as  PTGiamTruBoiThuong,
    				isnull(hd.BHThanhToanTruocThue,0) as  BHThanhToanTruocThue,
    				isnull(hd.PhaiThanhToanBaoHiem,0) as  PhaiThanhToanBaoHiem,
    
    				-----gán ID = ID_ChiTietGoiDV để bên ngoài lấy id này luôn ----
    				ctMua.ID as ID_ChiTietGoiDV,
    				ctMua.ID_DonViQuiDoi,
    				ctMua.ID_LoHang,
    				ctMua.ID_TangKem, 
    			ctMua.TangKem, 
    			ctMua.ID_ParentCombo,
    			ctMua.ID_ChiTietDinhLuong,
    				ctMua.SoLuong,
    				ctMua.DonGia,
    				ctMua.TienChietKhau,
    				ctMua.ThanhToan,
    				ctMua.TonLuyKe,				
    				ctMua.GhiChu,
    				ctMua.TienChietKhau as GiamGia,
    
    			CAST(ISNULL(ctMua.TienThue,0) as float) as TienThue,
    				CAST(ISNULL(ctMua.PTThue,0) as float) as PTThue, 
    			CAST(ISNULL(ctMua.ThoiGianBaoHanh,0) as float) as ThoiGianBaoHanh,
    			CAST(ISNULL(ctMua.LoaiThoiGianBH,0) as float) as LoaiThoiGianBH,
    				iif(ctMua.TenHangHoaThayThe is null or ctMua.TenHangHoaThayThe ='', hh.TenHangHoa, ctMua.TenHangHoaThayThe) as TenHangHoaThayThe,
    			Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
    			Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
    				iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
    
    			
    				isnull(lo.MaLoHang,'') as MaLoHang, 
    			isnull(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
    				isnull(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
    			isnull(hh.ChietKhauMD_NVTheoPT,'1') as ChietKhauMD_NVTheoPT,				
    				ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan,
    			CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
    				CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,    			
    			ISNULL(hh.GhiChu,'') as GhiChuHH,
    			ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
    				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    				hh.ID_NhomHang as ID_NhomHangHoa, 
    
    				hh.DichVuTheoGio,
    			hh.DuocTichDiem,
    			hh.SoPhutThucHien,
    				qd.MaHangHoa,
    				hh.TenHangHoa,
    				qd.TenDonViTinh,
    				qd.ID_HangHoa,
    				hh.QuanLyTheoLoHang,
    			hh.LaHangHoa,
    				lo.NgaySanXuat, 
    				lo.NgayHetHan, 
    
    
    				hdgoc.ID as ID_HoaDonGoc,
    				hdgoc.LoaiHoaDon as LoaiHoaDonGoc,
    				hdgoc.MaHoaDon as MaHoaDonGoc,
    
    				ctConLai.SoLuongBan,
    				ctConLai.SoLuongTra,
    				ctConLai.SoLuongDung,
    				ctConLai.SoLuongBan - isnull(ctConLai.SoLuongTra,0) - isnull(ctConLai.SoLuongDung,0) as SoLuongConLai
    			into #ctAll
    			from
    			(
    						select 
    							ctMuaTra.ID,
    							sum(SoLuongBan) as SoLuongBan,
    							sum(SoLuongTra) as SoLuongTra,
    							sum(SoLuongDung) as SoLuongDung
    						from
    						(
    								------ mua ----
    									select 
    										ct.ID,
    										ct.SoLuong as SoLuongBan,
    										0 as SoLuongTra,
    										0 as SoLuongDung
    									from #ctMua ct
    						
    
    										union all
    
    										----- tra ----
    										select ct.ID_ChiTietGoiDV,
    											0 as SoLuongBan,
    											ct.SoLuong as SoLuongTra,
    											0 as SoLuongDung
    										from BH_HoaDon hd
    										join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon  
    										where hd.ChoThanhToan = 0  
    										and hd.LoaiHoaDon = 6
    										and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID)		
    										and exists (select id from #ctMua ctMua where ct.ID_ChiTietGoiDV = ctMua.ID)
    
    										
    							
    
    										union all
    										----- sudung ----
    										select ct.ID_ChiTietGoiDV,
    											0 as SoLuongBan,
    											0 as SoLuongTra,
    											ct.SoLuong as SoLuongDung
    										from BH_HoaDon hd
    										join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon  
    										where hd.ChoThanhToan = 0  
    										and hd.LoaiHoaDon = 1
    										and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID)		
    										and exists (select id from #ctMua ctMua where ct.ID_ChiTietGoiDV = ctMua.ID)
    
    								)ctMuaTra
    								group by ctMuaTra.ID
    								having sum(SoLuongBan) - sum(SoLuongTra) - sum(SoLuongDung) > 0
    			)ctConLai			
    			join BH_HoaDon_ChiTiet ctMua on ctConLai.ID = ctMua.ID
    			join BH_HoaDon hd on ctMua.ID_HoaDon = hd.ID
    			join DonViQuiDoi qd on ctMua.ID_DonViQuiDoi = qd.ID
    			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    			left join DM_LoHang lo on hh.ID = lo.ID_HangHoa
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID			
    			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID		
    			where  ((select count(Name) from @tblSearch b where     			
    					hd.MaHoaDon like '%'+b.Name+'%'								
    					or dt.MaDoiTuong like '%'+b.Name+'%'		
    					or dt.TenDoiTuong like '%'+b.Name+'%'
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    					or dt.DienThoai like '%'+b.Name+'%'		
    					or qd.MaHangHoa like '%'+b.Name+'%'									
    					or hh.TenHangHoa like '%'+b.Name+'%'									
    					or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'	
    					)=@count or @count=0)
    	
    
    	declare @totalRow int= (select count(ID) from #ctAll)
	
		--(
    	select 
    		tblLast.*,
    		----- thanhtien: lấy số luong conlai * dongia sau ck ---
    		tblLast.SoLuongConLai * (tblLast.DonGia - tblLast.TienChietKhau) as ThanhTien,
    		@totalRow as TotalRow,
    		nv.TenNhanVien,
    		tblLast.TongThanhToan 
    			--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = 0   		
				- iif(tblLast.LoaiHoaDonGoc = 6, 
								iif(tblLast.LuyKeTraHang > 0, tblLast.TongGiaTriTra, 
									---- neu LuyKeTrahang < 0 (tức trả hàng > nợ HD cũ): BuTruTrahang = max (TongTienHang)
									iif(abs(tblLast.LuyKeTraHang) > tblLast.TongThanhToan, tblLast.TongThanhToan,
										---- hdDoiTra: tính cả phần trả của hdDoi nữa ---
										iif(KhachNo > 0, iif(abs(tblLast.LuyKeTraHang) + tblLast.TongGiaTriTra > tblLast.KhachNo, 
												tblLast.KhachNo, abs(tblLast.LuyKeTraHang) +  tblLast.TongGiaTriTra),
											abs(tblLast.LuyKeTraHang))
										)
									),
							 tblLast.LuyKeTraHang)
    			- tblLast.KhachDaTra  as ConNo
    		from(
    			select 
    				tbl.*,
    					isnull(iif(tbl.LoaiHoaDonGoc = 3 or tbl.ID_HoaDon is null,
    					iif(tbl.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
    						case when tbl.TongGiaTriTra > tbl.KhachNo then tbl.KhachNo						
    						else tbl.TongGiaTriTra end),
    					(select dbo.BuTruTraHang_HDDoi(tbl.ID_HoaDon,tbl.NgayLapHoaDon,tbl.ID_HoaDonGoc, tbl.LoaiHoaDonGoc))				
    				),0) as LuyKeTraHang	
    			
    			from (
    					select hd.*,
    						ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,	
    						ISNULL(allTra.NoTraHang,0) as NoTraHang,
    						isnull(sqHD.KhachDaTra,0) as KhachDaTra,
    						hd.TongThanhToan- isnull(sqHD.KhachDaTra,0) as KhachNo
    					from
    					(
    						----- get top 10 ----
    						select * from #ctAll
    						order by NgayLapHoaDon desc
    						offset (@CurrentPage * @PageSize) rows
    						fetch next @PageSize rows only
    					) hd
    					left join
    							(							
    									------ thu hoadon -----
    									select 
    										qct.ID_HoaDonLienQuan,
    										sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, - qct.TienThu)) as KhachDaTra
    									from Quy_HoaDon qhd
    									join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon= qhd.ID
    									where qhd.TrangThai='1'
    									and exists (select hd.ID from #hd hd 
    										where qct.ID_HoaDonLienQuan = hd.ID and  hd.ID_DoiTuong = qct.ID_DoiTuong)
    									group by qct.ID_HoaDonLienQuan															
    							) sqHD on sqHD.ID_HoaDonLienQuan = hd.ID
    								left join
    									(
    										------ all trahang of hdThis ---
    										select 					
    											hdt.ID_HoaDon,					
    											sum(hdt.PhaiThanhToan) as TongGtriTra,
    											sum(hdt.PhaiThanhToan - isnull(chiHDTra.DaTraKhach,0)) as NoTraHang
    										from BH_HoaDon hdt	
    										left join
    										(
    											select 
    												qct.ID_HoaDonLienQuan,
    												sum(iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu)) as DaTraKhach
    											from Quy_HoaDon_ChiTiet qct
    											join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    											where qhd.TrangThai='0'					
    											group by qct.ID_HoaDonLienQuan
    										) chiHDTra on hdt.ID = chiHDTra.ID_HoaDonLienQuan
    										where hdt.LoaiHoaDon= 6
    										and hdt.ChoThanhToan='0'
    										group by hdt.ID_HoaDon		
    									) allTra on allTra.ID_HoaDon = hd.ID
    				)tbl
    		)tblLast
    		left join NS_NhanVien nv on tblLast.ID_NhanVien= nv.ID
			order by NgayLapHoaDon desc
    				
    
    		drop table #ctMua
    		drop table #ctAll
    		drop table #hd
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_Where] 
 --declare 
 @timeStart [datetime]='2024-01-01',
    @timeEnd [datetime]='2024-03-01',
    @ID_ChiNhanh [nvarchar](max)='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE',
    @maHD [nvarchar](max)='KH119',
	@ID_NhanVienLogin nvarchar(max) = '',
	@NguoiTao nvarchar(max)='admin',
	@IDViTris nvarchar(max)='',
	@IDBangGias nvarchar(max)='',
	@TrangThai nvarchar(max)='0,1,2',
	@PhuongThucThanhToan nvarchar(max)='',
	@ColumnSort varchar(max)='NgayLapHoaDon',
	@SortBy varchar(max)= 'DESC',
	@CurrentPage int =0 ,
	@PageSize int = 15
AS
BEGIN
	set nocount on;

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	 declare @tblNhanVien table (ID uniqueidentifier)
	 if isnull(@ID_NhanVienLogin,'') !=''
		begin
			insert into @tblNhanVien
			select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'HoaDon_XemDS_PhongBan','HoaDon_XemDS_HeThong');
		end

	declare @tblChiNhanh table (ID uniqueidentifier)
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh);

	declare @tblPhuongThuc table (PhuongThuc int)
	insert into @tblPhuongThuc
	select Name from dbo.splitstring(@PhuongThucThanhToan)
	

	declare @tblTrangThai table (TrangThaiHD tinyint primary key)
	insert into @tblTrangThai
	select Name from dbo.splitstring(@TrangThai);


	declare @tblViTri table (ID varchar(40))
	insert into @tblViTri
	select Name from dbo.splitstring(@IDViTris) where Name!=''

	declare @tblBangGia table (ID varchar(40))
	insert into @tblBangGia
	select Name from dbo.splitstring(@IDBangGias) where Name!=''
	
	if @timeStart='2016-01-01'		
		select @timeStart = min(NgayLapHoaDon) from BH_HoaDon where LoaiHoaDon=19
	;with data_cte
	as
	(
    SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.ID_ViTri,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
		c.ID_Xe,
		xe.BienSo,
		c.ID_PhieuTiepNhan,
    	c.TheoDoi,
    	c.ID_DonVi,
    	c.ID_KhuyenMai,
    	c.ChoThanhToan,
    	c.MaHoaDon,  	
    	c.NgayLapHoaDon,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	c.TenDoiTuong,
    	c.Email,
    	----c.DienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng ---
    	c.NguoiTaoHD,
    	c.DiaChiKhachHang,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TenPhongBan,
    	c.TongTienHang,
		c.TongGiamGia, 
		--c.TongThanhToan,
		c.PhaiThanhToan,		
		c.ThuTuThe, c.TienMat, c.TienATM,c.TienDoiDiem, c.ChuyenKhoan, c.KhachDaTra,c.TongChietKhau,c.TongTienThue,PTThueHoaDon,
		c.TongThueKhachHang,
		ID_TaiKhoanPos,
		ID_TaiKhoanChuyenKhoan,
    	c.TrangThaiText,
    	c.KhuyenMai_GhiChu,
    	c.KhuyeMai_GiamGia,
		c.LoaiHoaDonGoc,
		c.TongGiaTriTra,
		c.KhachNo,
		(select dbo.BuTruTraHang_HDDoi(ID_HoaDon,NgayLapHoaDon,ID_HoaDonGoc, LoaiHoaDonGoc))as Butrugoc,
    	iif(c.TongThanhToan1 =0 and c.PhaiThanhToan> 0, c.PhaiThanhToan, c.TongThanhToan1) as TongThanhToan,
				isnull(iif(c.ID_HoaDon is null,
					iif(c.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
						case when c.TongGiaTriTra > c.KhachNo then c.KhachNo						
						else c.TongGiaTriTra end),
					(select dbo.BuTruTraHang_HDDoi(ID_HoaDon,NgayLapHoaDon,ID_HoaDonGoc, LoaiHoaDonGoc))				
				),0) as LuyKeTraHang,
    	c.LoaiHoaDon,
    	c.DiaChiChiNhanh,
    	c.DienThoaiChiNhanh,
    	c.DiemGiaoDich,
    	c.DiemSauGD, -- add 02.08.2018 (bind InHoaDon)
    	c.HoaDon_HangHoa, -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	CONVERT(nvarchar(10),c.NgayApDungGoiDV,103) as NgayApDungGoiDV,
    	CONVERT(nvarchar(10),c.HanSuDungGoiDV,103) as HanSuDungGoiDV
		
    	FROM
    	(
    		select 
    		a.ID as ID,
    		hdXMLOut.HoaDon_HangHoa,
    		bhhd.ID_DoiTuong,
    			-- Neu theo doi = null --> kiem tra neu la khach le --> theodoi = true, nguoc lai = 1
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,
    		bhhd.ID_HoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DonVi,
			bhhd.ID_Xe,
			bhhd.ID_PhieuTiepNhan,
    		bhhd.ChoThanhToan,
    		bhhd.ID_KhuyenMai,
    		bhhd.KhuyenMai_GhiChu,
    		bhhd.LoaiHoaDon,
			isnull(bhhd.PTThueHoaDon,0) as  PTThueHoaDon,
    		ISNULL(bhhd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(bhhd.DiemGiaoDich,0) AS DiemGiaoDich,
    		ISNULL(gb.ID,N'00000000-0000-0000-0000-000000000000') as ID_BangGia,
			ISNULL(vt.ID,N'00000000-0000-0000-0000-000000000000') as ID_ViTri,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		bhhd.MaHoaDon,   		
    		bhhd.NgayLapHoaDon,
    		bhhd.NgayApDungGoiDV,
    		bhhd.HanSuDungGoiDV,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
			ISNULL(nv.TenNhanVienKhongDau, N'') as TenNhanVienKhongDau,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		bhhd.TongChietKhau,
			bhhd.TongThanhToan as TongThanhToan1,
			ISNULL(bhhd.TongThueKhachHang,0) as TongThueKhachHang,
			ISNULL(bhhd.TongTienThue,0) as TongTienThue,
			bhhd.TongTienHang,
			bhhd.TongGiamGia,
			bhhd.PhaiThanhToan,

			hdgoc.ID_HoaDon as ID_HoaDonGoc,
			isnull(hdgoc.LoaiHoaDon,0) as LoaiHoaDonGoc,
			hdgoc.MaHoaDon as MaHoaDonGoc,

			ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,
			ISNULL(allTra.NoTraHang,0) as NoTraHang,

    		a.ThuTuThe,
    		a.TienMat,
			a.TienATM,
			a.TienDoiDiem,
    		a.ChuyenKhoan,
    		a.KhachDaTra,
			ID_TaiKhoanPos,
			ID_TaiKhoanChuyenKhoan,

			ISNULL(bhhd.PhaiThanhToan,0) - ISNULL(a.KhachDaTra,0) as KhachNo,
    		
			case bhhd.ChoThanhToan
				when 1 then '1'
				when 0 then '0'
			else '4' end as TrangThaiHD,
    		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThaiText,
			case when a.TienMat > 0 then
				case when a.TienATM > 0 then	
					case when a.ChuyenKhoan > 0 then
						case when a.ThuTuThe > 0 then '1,2,3,4' else '1,2,3' end												
						else 
							case when a.ThuTuThe > 0 then  '1,2,4' else '1,2' end end
						else
							case when a.ChuyenKhoan > 0 then 
								case when a.ThuTuThe > 0 then '1,3,4' else '1,3' end
								else 
										case when a.ThuTuThe > 0 then '1,4' else '1' end end end
				else
					case when a.TienATM > 0 then
						case when a.ChuyenKhoan > 0 then
								case when a.ThuTuThe > 0 then '2,3,4' else '2,3' end	
								else 
									case when a.ThuTuThe > 0 then '2,4' else '2' end end
							else 		
								case when a.ChuyenKhoan > 0 then
									case when a.ThuTuThe > 0 then '3,4' else '3' end
									else 
									case when a.ThuTuThe > 0 then '4' else '5' end end end end
									
						as PTThanhToan
    		FROM
    		(
    			Select 
    			b.ID,
    			SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
				SUM(ISNULL(b.TienATM, 0)) as TienATM,
    			SUM(ISNULL(b.TienCK, 0)) as ChuyenKhoan,
				SUM(ISNULL(b.TienDoiDiem, 0)) as TienDoiDiem,
    			SUM(ISNULL(b.TienThu, 0)) as KhachDaTra,
				max(b.ID_TaiKhoanPos) as ID_TaiKhoanPos,
				max(b.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan
    			from
    			(
    				Select 
    				bhhd.ID,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then case when TaiKhoanPOS = 1 then ISNULL(hdct.TienGui, 0) else 0 end else ISNULL(hdct.TienGui, 0) * (-1) end end as TienATM,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end else ISNULL(hdct.TienGui, 0) * (-1) end end as TienCK,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then 
							case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else ISNULL(hdct.Tienthu, 0) end
							else case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else -ISNULL(hdct.Tienthu, 0) end end end as TienDoiDiem,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu,
					case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 1 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanPos,
					case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 0 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanChuyenKhoan
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID  
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang		
    				where bhhd.LoaiHoaDon = '19' and bhhd.NgayLapHoadon between @timeStart and @timeEnd
					and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))    
					and (isnull(@ID_NhanVienLogin,'')='' or exists( select * from @tblNhanVien nv where nv.ID= bhhd.ID_NhanVien) or bhhd.NguoiTao= @NguoiTao)
    			) b
    			group by b.ID 
    		) as a			
    		join BH_HoaDon bhhd on a.ID = bhhd.ID   	
			left join BH_HoaDon hdgoc on bhhd.ID_HoaDon= hdgoc.ID
			left join
			(
				------ all trahang of hdgoc ---
				select 					
					hdt.ID_HoaDon,					
					sum(hdt.PhaiThanhToan) as TongGtriTra,
					sum(hdt.PhaiThanhToan - isnull(chiHDTra.DaTraKhach,0)) as NoTraHang
				from BH_HoaDon hdt	
				left join
				(
					select 
						qct.ID_HoaDonLienQuan,
						sum(iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu)) as DaTraKhach
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qct.ID_HoaDonLienQuan
					where qhd.TrangThai='0'					
					group by qct.ID_HoaDonLienQuan
				) chiHDTra on hdt.ID = chiHDTra.ID_HoaDonLienQuan
				where hdt.LoaiHoaDon= 6
				and hdt.ChoThanhToan='0'
				group by hdt.ID_HoaDon		
			) allTra on allTra.ID_HoaDon = bhhd.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID			
    		left join 
    			(Select distinct hdXML.ID, 
    					(
    					select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
    					from BH_HoaDon_ChiTiet ct
    					join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    					join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
    					where ct.ID_HoaDon = hdXML.ID
    					For XML PATH ('')
    				) HoaDon_HangHoa
    			from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
			left join Gara_DanhMucXe xe on c.ID_Xe= xe.ID
			where (@IDViTris ='' or exists (select ID from @tblViTri vt2 where vt2.ID= c.ID_ViTri))
			and (@IDBangGias ='' or exists (select ID from @tblBangGia bg where bg.ID= c.ID_BangGia))
			and exists (select TrangThaiHD from @tblTrangThai tt where c.TrangThaiHD= tt.TrangThaiHD)
		    and (@PhuongThucThanhToan ='' or exists(SELECT Name FROM splitstring(c.PTThanhToan) pt join @tblPhuongThuc pt2 on pt.Name = pt2.PhuongThuc))
			and	((select count(Name) from @tblSearch b where     			
				c.MaHoaDon like '%'+b.Name+'%'
				or c.NguoiTaoHD like '%'+b.Name+'%'				
				or c.TenNhanVien like '%'+b.Name+'%'
				or c.TenNhanVienKhongDau like '%'+b.Name+'%'
				or c.DienGiai like '%'+b.Name+'%'
				or c.MaDoiTuong like '%'+b.Name+'%'		
				or c.TenDoiTuong like '%'+b.Name+'%'
				or c.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or c.DienThoai like '%'+b.Name+'%'						
				or xe.BienSo like '%'+b.Name+'%'	
				or c.HoaDon_HangHoa like '%'+b.Name+'%'			
				)=@count or @count=0)	
				), 
				tblDebit as
				(
				select 
					cnLast.ID,
					cnLast.TongTienHDTra,					
					cnLast.ConNo
				from
				(

					select tblBuTruLast.ID,
						tblBuTruLast.TongTienHDTra,
						iif(tblBuTruLast.ChoThanhToan is null,0, 
							tblBuTruLast.TongThanhToan - tblBuTruLast.TongTienHDTra - tblBuTruLast.KhachDaTra) as ConNo
					from
					(
						select 
							c.ID,
							c.LoaiHoaDonGoc,
							c.TongGiaTriTra,
							c.ChoThanhToan,
							c.TongThanhToan,
							c.KhachDaTra,
							----- cot TongGiaTriTra: tongTra of hdThis ---
							iif(c.LoaiHoaDonGoc = 6, 
								iif(c.LuyKeTraHang > 0, c.TongGiaTriTra, 
									---- neu LuyKeTrahang < 0 (tức trả hàng > nợ HD cũ): BuTruTrahang = max (TongTienHang)
									iif(abs(c.LuyKeTraHang) > c.TongThanhToan, c.TongThanhToan,
										---- hdDoiTra: tính cả phần trả của hdDoi nữa ---
										iif(KhachNo > 0, iif(abs(c.LuyKeTraHang) + c.TongGiaTriTra > c.KhachNo, 
												c.KhachNo, abs(c.LuyKeTraHang) +  c.TongGiaTriTra),
											abs(c.LuyKeTraHang))
										)
									),
							 c.LuyKeTraHang) as TongTienHDTra								
						from data_cte c
					)tblBuTruLast
					) cnLast 
				),
			count_cte
		as (
			select count(dt.ID) as TotalRow,
				CEILING(COUNT(dt.ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,			
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumKhachDaTra,								
				sum(KhuyeMai_GiamGia) as SumKhuyeMai_GiamGia,								
				sum(PhaiThanhToan) as SumPhaiThanhToan,				
				sum(TongThanhToan) as SumTongThanhToan,
				sum(TienDoiDiem) as SumTienDoiDiem,
				sum(ThuTuThe) as SumThuTuThe,				
				sum(TienMat) as SumTienMat,
				sum(TienATM) as SumPOS,
				sum(ChuyenKhoan) as SumChuyenKhoan,				
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo
			from data_cte dt
			left join tblDebit cn on dt.ID= cn.ID
		)
		select dt.*, cte.*, cn.ConNo, cn.TongTienHDTra	
		from data_cte dt
		left join tblDebit cn on dt.ID= cn.ID
		cross join count_cte cte	
		order by 
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='' then NgayLapHoaDon end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='' then NgayLapHoaDon end DESC,
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end DESC,			
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachCanTra' then PhaiThanhToan end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachCanTra' then PhaiThanhToan end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC	
				
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
    	
END");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonTraHang]
--declare  
@timeStart [datetime] ='2024-01-01',
    @timeEnd [datetime]='2024-04-01',
    @ID_ChiNhanh [nvarchar](max) ='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @maHD [nvarchar](max)='J107054',
	@ID_NhanVienLogin uniqueidentifier=null,
	@NguoiTao nvarchar(max)='',
	@TrangThai nvarchar(max)='0,1,2,3',
	@ColumnSort varchar(max)='NgayLapHoaDon',
	@SortBy varchar(max)='DESC',
	@CurrentPage int = 0,
	@PageSize int = 50
AS
BEGIN
	set nocount on;
	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'TraHang_XemDS_PhongBan','TraHang_XemDS_PhongBan');

	declare @tblChiNhanh table (ID varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch)


	;with data_cte
	as(
	
    SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
		c.ID_Xe,
    	c.LoaiHoaDon,
    	c.ID_ViTri,
    	c.ID_DonVi,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,		
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.BienSo,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	ISNULL(c.NguoiTaoHD,'') as NguoiTaoHD,
		------c.DienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng ---
		c.Email,
		c.DiaChiKhachHang,
		c.NgaySinh_NgayTLap,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TongTienHang, c.TongGiamGia,
		c.KhuyeMai_GiamGia,
		c.PhaiThanhToan,		
		c.TongChiPhi,
		c.KhachDaTra, 
		c.TongThanhToan,
		c.ThuTuThe,
		c.TienMat,
		c.ChuyenKhoan,
		c.TongChietKhau,c.TongTienThue,
    	c.TrangThai,
    	c.TheoDoi,
    	c.TenPhongBan,
    	c.DienThoaiChiNhanh,
    	c.DiaChiChiNhanh,
    	c.DiemGiaoDich,
		c.ID_BaoHiem, c.ID_PhieuTiepNhan,
		c.TongTienBHDuyet, PTThueHoaDon, c.PTThueBaoHiem, c.TongTienThueBaoHiem, c.SoVuBaoHiem,
		c.KhauTruTheoVu, c.PTGiamTruBoiThuong,
		c.GiamTruBoiThuong, c.BHThanhToanTruocThue,
		c.PhaiThanhToanBaoHiem,				
    	'' as HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	FROM
    	(
    		select 
    	
    		a.ID as ID,
    		bhhd.MaHoaDon,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_BangGia,
    		bhhd.ID_HoaDon,
    		bhhd.ID_ViTri,
    		bhhd.ID_DonVi,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DoiTuong,
			

    		ISNULL(bhhd.DiemGiaoDich,0) as DiemGiaoDich,
    		bhhd.ChoThanhToan,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,

    		bhhd.NgayLapHoaDon,
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,

			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
			ISNULL(dt.TenDoiTuong_KhongDau, N'Khách lẻ') as TenDoiTuong_KhongDau,
			dt.NgaySinh_NgayTLap,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
			ISNULL(nv.TenNhanVienKhongDau, N'') as TenNhanVienKhongDau,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
			isnull(bhhd.KhuyeMai_GiamGia,0) as KhuyeMai_GiamGia,
    		CAST(ROUND(bhhd.TongChiPhi, 0) as float) as TongChiPhi,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
			CAST(ROUND(bhhd.TongTienThue, 0) as float) as TongTienThue,
			isnull(bhhd.TongThanhToan, bhhd.PhaiThanhToan) as TongThanhToan,

			bhhd.ID_BaoHiem, bhhd.ID_PhieuTiepNhan,bhhd.ID_Xe,
			xe.BienSo,
			isnull(bhhd.PTThueHoaDon,0) as PTThueHoaDon,
			isnull(bhhd.PTThueBaoHiem,0) as PTThueBaoHiem,
			isnull(bhhd.TongTienThueBaoHiem,0) as TongTienThueBaoHiem,
			isnull(bhhd.SoVuBaoHiem,0) as SoVuBaoHiem,
			isnull(bhhd.KhauTruTheoVu,0) as KhauTruTheoVu,
			isnull(bhhd.TongTienBHDuyet,0) as TongTienBHDuyet,
			isnull(bhhd.PTGiamTruBoiThuong,0) as PTGiamTruBoiThuong,
			isnull(bhhd.GiamTruBoiThuong,0) as GiamTruBoiThuong,
			isnull(bhhd.BHThanhToanTruocThue,0) as BHThanhToanTruocThue,
			isnull(bhhd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,
    		a.KhachDaTra,
    		a.ThuTuThe,
    		a.TienMat,
    		a.ChuyenKhoan,
    		bhhd.TongChietKhau,			
			case bhhd.ChoThanhToan
				when 0 then 0
				when 1 then 1
				else 4 end as TrangThaiHD,   
    		Case When bhhd.ChoThanhToan = 0 then N'Hoàn thành' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			select a1.ID, 
					sum(KhachDaTra) as KhachDaTra,
					sum(ThuTuThe) as ThuTuThe,
					sum(TienMat) as TienMat,
					sum(TienPOS) as TienATM,
					sum(TienCK) as ChuyenKhoan
				from (
					Select 
    				bhhd.ID,					
					case when qhd.TrangThai ='0' then 0 else ISNULL(qct.Tienthu, 0) end as KhachDaTra,
					Case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=4, isnull(qct.TienThu,0),0) end as ThuTuThe,
					case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=1, isnull(qct.TienThu,0),0) end as TienMat,										
					case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=2, isnull(qct.TienThu,0),0) end as TienPOS,
					case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=3, isnull(qct.TienThu,0),0) end as TienCK							
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet qct on bhhd.ID = qct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
    				where bhhd.LoaiHoaDon = 6
					and bhhd.NgayLapHoadon between  @timeStart and @timeEnd 
					and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
				) a1 group by a1.ID
    		) as a
    		left join BH_HoaDon bhhd on a.ID = bhhd.ID	
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID    		
			left join Gara_DanhMucXe xe on bhhd.ID_Xe = xe.ID
    		) as c
			join (select Name from dbo.splitstring(@TrangThai)) tt on c.TrangThaiHD = tt.Name
			--where (exists( select * from @tblNhanVien nv where nv.ID= c.ID_NhanVien) or c.NguoiTaoHD= @NguoiTao)
			where
				((select count(Name) from @tblSearch b where     			
				c.MaHoaDon like '%'+b.Name+'%'
				or c.NguoiTaoHD like '%'+b.Name+'%'
				or c.TenNhanVien like '%'+b.Name+'%'
				or c.TenNhanVienKhongDau like '%'+b.Name+'%'
				or c.DienGiai like '%'+b.Name+'%'
				or c.MaDoiTuong like '%'+b.Name+'%'		
				or c.TenDoiTuong like '%'+b.Name+'%'
				or c.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or c.DienThoai like '%'+b.Name+'%'	
				
				)=@count or @count=0)	
			), 
			count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumKhachDaTra,	
				sum(PhaiThanhToan) as SumPhaiThanhToan,			
				sum(TongChiPhi) as SumTongChiPhi,				
				sum(ThuTuThe) as SumThuTuThe,				
				sum(TienMat) as SumTienMat,			
				sum(ChuyenKhoan) as SumChuyenKhoan,				
				sum(TongTienThue) as SumTongTienThue
			from data_cte
		),
		tblView as
		(
		select dt.*, cte.*		
		from data_cte dt
		cross join count_cte cte	
		order by 
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end DESC,			
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachCanTra' then PhaiThanhToan end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachCanTra' then PhaiThanhToan end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC	
				
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
    	)
		----- select top 10 -----
		select *
		into #tblView
		from tblView

		----- get list ID of top 10
		declare @tblID TblID
		insert into @tblID
		select ID from #tblView
		
		------ get congno of top 10
		declare @tblCongNo table (ID uniqueidentifier, MaHoaDonGoc nvarchar(max), LoaiHoaDonGoc int, HDDoi_PhaiThanhToan float, BuTruHDGoc_Doi float)
		insert into @tblCongNo
		exec TinhCongNo_HDTra @tblID, 6
					
		select *,
			iif(ConNo1 <0,0,ConNo1) as ConNo
		from
		(
		select tView.*,
			cn.MaHoaDonGoc,
			cn.LoaiHoaDonGoc,
			cn.BuTruHDGoc_Doi,
			
			isnull(cn.BuTruHDGoc_Doi,0) as TongTienHDDoiTra,
			---- muontruong: TongTienHDTra => PhaiTraKhach (sau khi butru congno hdGoc & hdDoi) --
			iif(isnull(cn.BuTruHDGoc_Doi,0) > tView.PhaiThanhToan, 0,
					iif(tView.KhachDaTra > 0 and isnull(cn.BuTruHDGoc_Doi,0) > 0, tView.KhachDaTra, tView.PhaiThanhToan - isnull(cn.BuTruHDGoc_Doi,0))
				) as TongTienHDTra, 
	
			---- butru > phaitt: phaiTraKhach = 0, và khachPhaiTra them phàn nay ---
			----iif(isnull(cn.BuTruHDGoc_Doi,0) > tView.PhaiThanhToan, 0,tView.PhaiThanhToan - isnull(cn.BuTruHDGoc_Doi,0))  as TongTienHDTra, 
			iif(isnull(cn.BuTruHDGoc_Doi,0) > tView.PhaiThanhToan, 0, tView.PhaiThanhToan - isnull(cn.BuTruHDGoc_Doi,0) - tView.KhachDaTra)  as ConNo1
			
		from #tblView tView
		left join @tblCongNo cn on tView.ID = cn.ID
		)tbl
		order by tbl.NgayLapHoaDon desc

		drop table #tblView
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_GetHDDebit_ofKhachHang]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max),
	@LoaiDoiTuong int
AS
BEGIN
	if @ID_DonVi='00000000-0000-0000-0000-000000000000'
		begin
			set @ID_DonVi = (select CAST(ID as varchar(40)) + ',' as  [text()] from DM_DonVi  where TrangThai is null or TrangThai='1' for xml path(''))	
			set @ID_DonVi= left(@ID_DonVi, LEN(@ID_DonVi) -1) -- remove last comma ,
		end

		select 
			tblView.*,
			tblView.TongThanhToan - GiaTriTatToan - TongTienHDTra as PhaiThanhToan ----- ~ PhaiThanhToan after butruTraHang ---
		from
		(
		select 
			tbl.*,						
				iif(tbl.LoaiHoaDonGoc = 6, 
								iif(tbl.LuyKeTraHang > 0, tbl.TongGiaTriTra, 
									---- neu LuyKeTrahang < 0 (tức trả hàng > nợ HD cũ): BuTruTrahang = max (TongTienHang)
									iif(abs(tbl.LuyKeTraHang) + tbl.TongGiaTriTra > tbl.TongThanhToan, tbl.TongThanhToan,
										---- hdDoiTra: tính cả phần trả của hdDoi nữa ---
											abs(tbl.LuyKeTraHang) + tbl.TongGiaTriTra
										)
									),
							 tbl.LuyKeTraHang) as TongTienHDTra
		from
		(
			select 						
				hdGocTra.ID,
				hdGocTra.MaHoaDon,
				hdGocTra.NgayLapHoaDon,
				hdGocTra.LoaiHoaDon,
				hdGocTra.TongTienThue,
				hdGocTra.TongThanhToan,
				hdGocTra.ID_HoaDonGoc,
				hdGocTra.LoaiHoaDonGoc ,	
				hdGocTra.TongGiaTriTra,
				NoTraHang,
				isnull(iif(LoaiHoaDonGoc = 3 or hdGocTra.ID_HoaDon is null,								
										case when hdGocTra.TongGiaTriTra > hdGocTra.PhaiThanhToan then hdGocTra.PhaiThanhToan else hdGocTra.TongGiaTriTra end,											
											(select dbo.BuTruTraHang_HDDoi(hdGocTra.ID_HoaDon,NgayLapHoaDon,hdGocTra.ID_HoaDonGoc, hdGocTra.LoaiHoaDonGoc))				
						),0) as LuyKeTraHang,
				isnull(tattoanTGT.GiaTriTatToan,0) as GiaTriTatToan,
				hdGocTra.TinhChietKhauTheo			
			from
			(
			select hd.ID,
				hd.MaHoaDon, 
				hd.NgayLapHoaDon, 
				hd.LoaiHoaDon,
				hd.TongTienThue,
				hd.TongThanhToan,
				hd.PhaiThanhToan,
    			0 as TinhChietKhauTheo,
				hd.ID_HoaDon,
				hdgoc.LoaiHoaDon as LoaiHoaDonGoc,
				hdgoc.ID_HoaDon as ID_HoaDonGoc,
				isnull(NoTraHang,0) as NoTraHang,
				ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra		
    		from BH_HoaDon hd
    		left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID ---- khong check loaiHD: lay ca hdTra + hdDoi
			left join
				(
					------ all trahang of hdgoc ---
					select 					
						hdt.ID_HoaDon,					
						sum(hdt.PhaiThanhToan) as TongGtriTra,
						sum(hdt.PhaiThanhToan - isnull(chiHDTra.DaTraKhach,0)) as NoTraHang
					from BH_HoaDon hdt	
					left join
					(
						select 
							qct.ID_HoaDonLienQuan,
							sum(iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu)) as DaTraKhach
						from Quy_HoaDon_ChiTiet qct
						join Quy_HoaDon qhd on qct.ID_HoaDon= qct.ID_HoaDonLienQuan
						where qhd.TrangThai='0'					
						group by qct.ID_HoaDonLienQuan
					) chiHDTra on hdt.ID = chiHDTra.ID_HoaDonLienQuan
					where hdt.LoaiHoaDon= 6
					and hdt.ChoThanhToan='0'
					and hdt.ID_DoiTuong like @ID_DoiTuong
					group by hdt.ID_HoaDon		
				) allTra on allTra.ID_HoaDon = hd.ID
    	
    		where 
			exists (select Name from dbo.splitstring(@ID_DonVi) where Name= hd.ID_DonVi)
			and hd.ID_DoiTuong like @ID_DoiTuong		
    		and hd.LoaiHoaDon in (1,19,4,22, 25)
    		and hd.ChoThanhToan='0' 
			) hdGocTra			
			left join
			(
				---- khi khách mua TGT nhưng chưa thanh toán hết --> tất toán công nợ ảo ---
				select
					hd.ID_HoaDon, hd.TongThanhToan as GiaTriTatToan
				from BH_HoaDon hd
				where hd.ID_DoiTuong like @ID_DoiTuong
				and hd.ID_HoaDon is not null ---- idThẻgiá trị				
				and hd.ChoThanhToan='0' and hd.LoaiHoaDon= 42
			) tattoanTGT on hdGocTra.ID= tattoanTGT.ID_HoaDon
		) tbl		
		)tblView order by NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[CreateAgainPhieuXuatKho_WhenUpdateTPDL]
    @ID_CTHoaDon [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    		;DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
    	---- ==========  INSERT AGAIN CTXUAT NEW ===========
    			
    			--- get cthd new
    		declare @ctHDNew table (ID uniqueidentifier, ID_ChiTietDinhLuong uniqueidentifier, ID_ChiTietGoiDV  uniqueidentifier, 
    			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    			SoLuong float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max),ThanhTien float,
    			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(100)				
    		)
    		insert into @ctHDNew
    		select ct.ID, ct.ID_ChiTietDinhLuong, ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    					ct.SoLuong, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,
						ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as ThanhTien,---- sudung tu gdv: van lay thanhtien
    					hh.LaHangHoa,
    					hh.TenHangHoa,
    					qd.MaHangHoa
    		from BH_HoaDon_ChiTiet ct 
    		join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    		where ct.ID_ChiTietDinhLuong= @ID_CTHoaDon
    		and (ct.ChatLieu is null or ct.ChatLieu !='5')
    		
    			
    				declare @MaHoaDon varchar(50), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
    				@NgayLapHoaDon datetime, @NguoiTao nvarchar(max),@LoaiHoaDon int = 35 ---- xuatkho nguyenvatlieu (LoaiHoaDon = 35)
    
    				declare @ID_HoaDonMua uniqueidentifier = (select ID_HoaDon from BH_HoaDon_ChiTiet where ID = @ID_CTHoaDon)
    				---- get infor hoadon
    				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
    				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao
    				from BH_HoaDon where id= @ID_HoaDonMua
    
    				declare @count int = (select count (ID) from  @ctHDNew where LaHangHoa = 1)		
    							
    
    				IF @count > 0
    				BEGIN
    
    					declare  @TongGiaTriXuat float = 
    					(select sum(ct.GiaVon * SoLuong)
    					from @ctHDNew ct
    					where ct.ID != ct.ID_ChiTietDinhLuong
    					)
    
    					declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,2,NgayLapHoaDon)) from BH_HoaDon where LoaiHoaDon= 35 and ID_HoaDon = @ID_HoaDonMua)
    					if @maxNgayLap is null 
							set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)
						else
							begin
									---- compare ngaylapHD - ngayxuatkho (if updateHD & change ngaylapHD)
								if FORMAT(@maxNgayLap,'yyyy-MM-dd')!= FORMAT(@NgayLapHoaDon,'yyyy-MM-dd')
										set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)		
							end

    
    						---- INSERT HD XUATKHO ----
    					declare @ID_XuatKho uniqueidentifier = newID()	, @ngayXuatKho datetime= @maxNgayLap ,@maXuatKho nvarchar(max)		

						---- find all PhieuXuatKho by ID_hoadongoc & by dichvu:  used to get mahoadon 
						declare @tblPhieuXuat table (ID uniqueidentifier, MaHoaDon nvarchar(50), NgayTao datetime)
						insert into @tblPhieuXuat
						select distinct hd.ID, hd.MaHoaDon, hd.NgayTao	
						from BH_HoaDon hd 
						join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
						where hd.LoaiHoaDon= 35 and hd.ID_HoaDon = @ID_HoaDonMua
						and ct.ID_ChiTietDinhLuong = (select top 1 ID_DonViQuiDoi from @ctHDNew where ID= @ID_CTHoaDon)
    
    					---- find all PhieuXuatKho by ID_hoadongoc
    					declare @countPhieuXK int = (select count(id) from @tblPhieuXuat)
    					declare @maXuatKhoGoc nvarchar(max) = (select top 1 MaHoaDon from @tblPhieuXuat order by NgayTao)
    					
    				if @countPhieuXK = 0
    						begin
    						---- neu chua co phieuxuat
    								declare @tblMa table (MaHoaDon nvarchar(max)) 	---- get mahoadon xuatkho
    							insert into @tblMa
    							exec GetMaHoaDonMax_byTemp @LoaiHoaDon, @ID_DonVi, @ngayxuatkho
    							select @maXuatKho = MaHoaDon from @tblMa
    						end
    				else 
    						begin
    							---- exist: tang maphieuxuat theo so lan xuat
    							 set @maXuatKho = CONCAT(@maXuatKhoGoc, '_', @countPhieuXK)    						 
    						end
    					
    
    						declare @xuatchoDV nvarchar(max)
    						= (select top 1 CONCAT(N', Dịch vụ: ', TenHangHoa, '(', MaHangHoa,  N'), Thành tiền: ', FORMAT(ThanhTien, 'N0') ) from @ctHDNew where ID= @ID_CTHoaDon)
    
    					
    						insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon, NgayLapHoaDon, ID_DonVi, ID_NhanVien, ID_DoiTuong,
    						TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
    					PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai)
    
    					values (@ID_XuatKho, @LoaiHoaDon, @maXuatKho,@ID_HoaDonMua, @ngayXuatKho, @ID_DonVi,@ID_NhanVien, @ID_DoiTuong,
    						@TongGiaTriXuat,0,0,0,0,0, @TongGiaTriXuat,0,'1',N'Tạm lưu', GETDATE(), @NguoiTao, 
    						concat(N'Cập nhật phiếu xuất nguyên liệu cho hóa đơn ', @MaHoaDon, @xuatchoDV) )
    
    
    							---- INSERT CT XUATKHO ----
    						insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_ChiTietGoiDV, ID_ChiTietDinhLuong, --- !! important save ID_ChiTietDinhLuong --> used to caculator GiaVon for DichVu
    								ID_DonViQuiDoi, ID_LoHang, SoLuong, DonGia, GiaVon, ThanhTien, ThanhToan, 
    							PTChietKhau, TienChietKhau, PTChiPhi, TienChiPhi, TienThue, An_Hien, TonLuyKe, GhiChu,  ChatLieu)		
    						select 
    						NEWid(),
    						@ID_XuatKho,
    						row_number() over( order by (select 1)) as SoThuTu,
    						ctsc.ID_ChiTietGoiDV,
    						ctsc.ID_DichVu,
    						ctsc.ID_DonViQuiDoi,
    						ctsc.ID_LoHang,
    						ctsc.SoLuong, ctsc.GiaVon, ctsc.GiaVon, ctsc.GiaTri, 
    						0,0,0,0,0,0,'1', ctsc.TonLuyKe, ctsc.GhiChu,''
    					from 
    					(
    					---- get infor tp + dichvu
    						select 
    							cttp.ID as ID_ChiTietGoiDV,
    							dv.ID_DonViQuiDoi as ID_DichVu,
    							cttp.ID_DonViQuiDoi, 
    							cttp.ID_LoHang,
    							cttp.SoLuong,
    							cttp.GiaVon,
    							cttp.GiaVon* cttp.SoLuong as GiaTri,			
    							cttp.TonLuyKe,
    							isnull(cttp.GhiChu,'') as GhiChu
    						from @ctHDNew cttp		
    						join @ctHDNew dv on cttp.ID_ChiTietDinhLuong = dv.ID					
    						and cttp.SoLuong > 0		
    						and cttp.LaHangHoa='1'
    						) ctsc
       						    					
    				END				
END");

			Sql(@"ALTER PROCEDURE [dbo].[CreateXuatKho_NguyenVatLieu]
    @ID_HoaDon [uniqueidentifier],
    @TrangThai [bit] = 0
AS
BEGIN
    SET NOCOUNT ON;	
	;DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon

		declare @ctHDNew table (ID uniqueidentifier, ID_ChiTietDinhLuong uniqueidentifier, ID_ChiTietGoiDV  uniqueidentifier, 
    			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    			SoLuong float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max), ThanhTien float,
    			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(100)	, ID_DichVu uniqueidentifier		
    		)
    		
    		insert into @ctHDNew
    		select ct.ID, ct.ID_ChiTietDinhLuong, ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    					ct.SoLuong, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,
						ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as ThanhTien, ---- sudung tu gdv: van lay thanhtien
    					hh.LaHangHoa,
    					hh.TenHangHoa,
    					qd.MaHangHoa,
						ct.ID
    		from BH_HoaDon_ChiTiet ct 
    		join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    		where ct.ID_HoaDon= @ID_HoaDon
    		and ct.ID_ChiTietDinhLuong is not null 

			update tp set tp.ID_DichVu = dv.ID_DonViQuiDoi ---- used to get maphieuXK by dichvu
			from @ctHDNew tp
			join @ctHDNew dv on tp.ID_ChiTietDinhLuong = dv.ID


				update BH_HoaDon set ChoThanhToan= null where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35


					-- ==========  INSERT AGAIN CTXUAT NEW ===========	

					declare @MaHoaDon varchar(50), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
    				@NgayLapHoaDon datetime, @NguoiTao nvarchar(50),@LoaiHoaDon int = 35 ---- xuatkho nguyenvatlieu (LoaiHoaDon = 35)
    				-- get infor hoadon
    				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
    				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao
    				from BH_HoaDon where id= @ID_HoaDon
    
    				declare @count int = (select count (ID) from  @ctHDNew where LaHangHoa = 1)		

					IF @count > 0
    				BEGIN
						declare @ID_DichVu uniqueidentifier, @ID_ChiTietDinhLuong uniqueidentifier, @TongGiaTriXuat float
      			     
    					declare _cur cursor
    					for
    					select ct.ID_DichVu, ct.ID_ChiTietDinhLuong, sum(ct.GiaVon * SoLuong)
    					from @ctHDNew ct
    					where ct.ID != ct.ID_ChiTietDinhLuong
    					group by ct.ID_ChiTietDinhLuong, ct.ID_DichVu ---- group by dichvu (1 dichvu - 1phieuxuat NVL)
    
    					open _cur
    					FETCH NEXT FROM _cur
    					INTO @ID_DichVu, @ID_ChiTietDinhLuong, @TongGiaTriXuat
    					WHILE @@FETCH_STATUS = 0
    					begin

							declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,5,NgayLapHoaDon)) from BH_HoaDon where ID_HoaDon= @ID_HoaDon)
						
    						if @maxNgayLap is null 
								set @maxNgayLap = DATEADD(MILLISECOND,5,@NgayLapHoaDon)
							else
								begin
										-- compare ngaylapHD - ngayxuatkho (if updateHD & change ngaylapHD)
									if FORMAT(@maxNgayLap,'yyyy-MM-dd')!= FORMAT(@NgayLapHoaDon,'yyyy-MM-dd')
											set @maxNgayLap = DATEADD(MILLISECOND,5,@NgayLapHoaDon)											
								end

							-- find all PhieuXuatKho by ID_hoadongoc & by dichvu:  used tang mahoadon theo solanxuat
							declare @tblPhieuXuat table (ID uniqueidentifier, MaHoaDon nvarchar(50), NgayTao datetime)
							insert into @tblPhieuXuat
							select  hd.ID, hd.MaHoaDon, hd.NgayTao	
							from BH_HoaDon hd 
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							where hd.ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35 and ct.ID_ChiTietDinhLuong = @ID_DichVu
							group by hd.ID, hd.MaHoaDon, hd.NgayTao

    						declare @countPhieuXK int = (select count(id) from @tblPhieuXuat)
    						declare @maXuatKhoGoc nvarchar(50) = (select top 1 MaHoaDon from @tblPhieuXuat order by NgayTao)	
    				
    						-- INSERT HD XUATKHO ----
    						 declare @ID_XuatKho uniqueidentifier = newID()	, @ngayXuatKho datetime= getdate(),@maXuatKho nvarchar(max)		
    						 declare @YeuCau nvarchar(max)
    						 if @TrangThai ='1' set @YeuCau =N'Tạm lưu'
    							else set @YeuCau = N'Hoàn thành'
    												
    						set @ngayXuatKho = @maxNgayLap 
    						
    						 -- get mahoadon xuatkho
    						declare @tblMa table (MaHoaDon nvarchar(max)) 	
    						if @countPhieuXK = 0
    						begin
    							insert into @tblMa
    							exec GetMaHoaDonMax_byTemp @LoaiHoaDon, @ID_DonVi, @ngayxuatkho
    							select @maXuatKho = MaHoaDon from @tblMa
    								
    								set @countPhieuXK = 1
    								set @maXuatKhoGoc = @maXuatKho
    							end
    						else
    							begin
    								set @maXuatKho = CONCAT(@maXuatKhoGoc, '_', @countPhieuXK)    	
    								set @countPhieuXK += 1
    							end
    
    						declare @xuatchoDV nvarchar(max)
    						= (select top 1 CONCAT(N', Dịch vụ: ', TenHangHoa, '(', MaHangHoa, N'), Thành tiền: ', FORMAT(ThanhTien, 'N0') ) from @ctHDNew where ID= @ID_ChiTietDinhLuong)
    
    						insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon, NgayLapHoaDon, ID_DonVi, ID_NhanVien, ID_DoiTuong,
    						TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
    					PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai)
    
    					values (@ID_XuatKho, @LoaiHoaDon, @maXuatKho,@ID_HoaDon, @ngayXuatKho, @ID_DonVi,@ID_NhanVien, @ID_DoiTuong,
    						@TongGiaTriXuat,0,0,0,0,0, @TongGiaTriXuat,0, @TrangThai, @YeuCau, GETDATE(), @NguoiTao, 
    						concat(N'Xuất nguyên vật liệu cho hóa đơn ', @MaHoaDon, @xuatchoDV) )
    
    							-- INSERT CT XUATKHO ----
    						insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_ChiTietGoiDV, ID_ChiTietDinhLuong, --- !! important save ID_ChiTietDinhLuong --> used to caculator GiaVon for DichVu
    								ID_DonViQuiDoi, ID_LoHang, SoLuong, DonGia, GiaVon, ThanhTien, ThanhToan, 
    							PTChietKhau, TienChietKhau, PTChiPhi, TienChiPhi, TienThue, An_Hien, TonLuyKe, GhiChu,  ChatLieu)		
    						select 
    						NEWid(),
    						@ID_XuatKho,
    						row_number() over( order by (select 1)) as SoThuTu,
    						ctsc.ID_ChiTietGoiDV,
    						ctsc.ID_DichVu,
    						ctsc.ID_DonViQuiDoi,
    						ctsc.ID_LoHang,
    						ctsc.SoLuong, ctsc.GiaVon, ctsc.GiaVon, ctsc.GiaTri, 
    						0,0,0,0,0,0,'1', ctsc.TonLuyKe, ctsc.GhiChu,''
    					from 
    					(
    					------- ct hoadon banle or hd sudung GDV
    						select 
    							cttp.ID as ID_ChiTietGoiDV,
    							dv.ID_DonViQuiDoi as ID_DichVu,
    							cttp.ID_DonViQuiDoi, 
    							cttp.ID_LoHang,
    							cttp.SoLuong,
    							cttp.GiaVon,
    							cttp.GiaVon* cttp.SoLuong as GiaTri,			
    							cttp.TonLuyKe,
    							isnull(cttp.GhiChu,'') as GhiChu
    						from @ctHDNew cttp		
    						join @ctHDNew dv on cttp.ID_ChiTietDinhLuong = dv.ID
    						where cttp.ID_ChiTietDinhLuong= @ID_ChiTietDinhLuong
    						and cttp.SoLuong > 0		
    						and cttp.LaHangHoa='1'
    						) ctsc
    
    					delete from @tblMa
						delete from @tblPhieuXuat
    				
    					FETCH NEXT FROM _cur
    					INTO @ID_DichVu, @ID_ChiTietDinhLuong, @TongGiaTriXuat						
    					end
    					CLOSE _cur;
    					DEALLOCATE _cur;		
    				END
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[CreatePhieuXuat_FromHoaDon]
    @ID_HoaDon [uniqueidentifier],
    @LoaiXuatKho [int],
    @IsXuatNgayThuoc [bit],
    @TrangThai [bit]
AS
BEGIN
    SET NOCOUNT ON;
		;DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
    
			------- get all phieuXK NVL of hoadon (bao gom xuat le + xuat NVL)  ------
			declare @tblHDXuat table (ID uniqueidentifier,ID_DonVi uniqueidentifier,MaHoaDon nvarchar(50), 
			LoaiHoaDon int, NgayLapHoaDon datetime, NgayTao datetime, ChoThanhToan bit)
			insert into @tblHDXuat
			select ID, ID_DonVi, MaHoaDon, LoaiHoaDon, NgayLapHoaDon, NgayTao, ChoThanhToan
			from dbo.BH_HoaDon where ID_HoaDon= @ID_HoaDon 
    
  
    
    		---- get cthd new 
    		---- TienChietKhau: soluong xuat/1 ngaythuoc, SoLuong: tong sl xuat
    		declare @ctHDNew table (ID uniqueidentifier,
    			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    			SoLuong float, TienChietKhau float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max),
    			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(100)				
    		)
    		if @IsXuatNgayThuoc ='1'
    		begin
    			insert into @ctHDNew
    			select ct.ID, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    						ct.SoLuong, ct.TienChietKhau, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,
    						hh.LaHangHoa,
    						hh.TenHangHoa,
    						qd.MaHangHoa
    			from BH_HoaDon_ChiTiet ct 
    			join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    			where ct.ID_HoaDon= @ID_HoaDon
    			and ct.ChatLieu='6' --- (chi lay SP ngaythuoc)
    			and hh.LaHangHoa='1'
    		end
    		else
    		begin
    			insert into @ctHDNew
    			select ct.ID, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    						ct.SoLuong, ct.TienChietKhau, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,
    						hh.LaHangHoa,
    						hh.TenHangHoa,
    						qd.MaHangHoa
    			from BH_HoaDon_ChiTiet ct 
    			join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    			where ct.ID_HoaDon= @ID_HoaDon
    			and ct.ID_ChiTietDinhLuong is null --- hdle + hdbaohanh
    			and (ct.ChatLieu is null or ct.ChatLieu not in ('5','6'))
    			and hh.LaHangHoa='1'
    		end
    		
    		
			 --- ===== HUY PHIEU XUATKHO OLD ----------
			
				update BH_HoaDon set ChoThanhToan= null where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= @LoaiXuatKho
    		    					
    
    				
    					---- ==========  INSERT AGAIN CTXUAT NEW (only insert if exist ctNew) ===========		
    				if exists (select ID from @ctHDNew)
    				begin
    					declare @YeuCau nvarchar(max)
    					if @TrangThai ='1' set @YeuCau =N'Tạm lưu'
    					else set @YeuCau = N'Hoàn thành'
    
    			
    				declare @MaHoaDon varchar(50), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
    				@NgayLapHoaDon datetime, @NguoiTao nvarchar(50), @IsChuyenPhatNhanh bit='0'
    				---- get infor hoadon
    				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
    				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao, @IsChuyenPhatNhanh = An_Hien
    				from BH_HoaDon where id= @ID_HoaDon
    									
    				
    						---- find all PhieuXuatKho by ID_hoadongoc: used tang mahoadon theo solanxuat
    					declare @countPhieuXK int = (select count(id) from @tblHDXuat where LoaiHoaDon= @LoaiXuatKho)
    					declare @maXuatKhoGoc nvarchar(50) = (select top 1 MaHoaDon from @tblHDXuat where LoaiHoaDon= @LoaiXuatKho order by NgayTao)	
    					declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,2,NgayLapHoaDon)) from dbo.BH_HoaDon where ID_HoaDon = @ID_HoaDon)

    					if @maxNgayLap is null 
							set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)
						else
							begin
								---- compare ngaylapHD - ngayxuatkho (if updateHD & change ngaylapHD)
								if FORMAT(@maxNgayLap,'yyyy-MM-dd')!= FORMAT(@NgayLapHoaDon,'yyyy-MM-dd')
										set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)		
							end
    
    					declare @TongGiaTriXuat float = (select sum(isnull(GiaVon,0) * SoLuong) from @ctHDNew)
    
    				
    						---- INSERT HD XUATKHO ----
    						 declare @ID_XuatKho uniqueidentifier = newID()	, @ngayXuatKho datetime= getdate(),@maXuatKho nvarchar(max)		
    						
    						set @ngayXuatKho = @maxNgayLap --- phieuxuat phai sau hoadon
    
    						 ---- get mahoadon xuatkho
    						declare @tblMa table (MaHoaDon nvarchar(max)) 	
    						if @countPhieuXK = 0
    						begin
    							insert into @tblMa
    							exec GetMaHoaDonMax_byTemp @LoaiXuatKho, @ID_DonVi, @ngayxuatkho
    							select @maXuatKho = MaHoaDon from @tblMa
    								
    								set @countPhieuXK = 1
    								set @maXuatKhoGoc = @maXuatKho
    							end
    						else
    							begin
    								set @maXuatKho = CONCAT(@maXuatKhoGoc, '_', @countPhieuXK)    	
    								set @countPhieuXK += 1
    							end
    					
    
    						declare @nhomHoTro nvarchar(max) = '', @sLoaiXuat nvarchar(max) = ''
    						if @IsXuatNgayThuoc ='1'
    							set @nhomHoTro= concat(N', nhóm dịch vụ ',(select TenNhomHangHoa from BH_HoaDon hd join DM_NhomHangHoa nhom on hd.ID_CheckIn = nhom.ID where hd.ID = @ID_HoaDon))
    
    						if @LoaiXuatKho = 37
    							set @sLoaiXuat = concat(N'Xuất hỗ trợ ngày thuốc, hóa đơn ', @MaHoaDon)
    						if @LoaiXuatKho = 38
    							set @sLoaiXuat = concat(N'Xuất bán lẻ, hóa đơn ', @MaHoaDon)
    						if @LoaiXuatKho = 39
    							set @sLoaiXuat = concat(N'Xuất bảo hành, hóa đơn ', @MaHoaDon)
    						if @LoaiXuatKho = 40
    							set @sLoaiXuat = concat(N'Xuất hỗ trợ chung, hóa đơn ', @MaHoaDon)
    
    						insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon, NgayLapHoaDon, ID_DonVi, ID_NhanVien, ID_DoiTuong,
    						TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
    					PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai, An_Hien) ---- an_hien: 1.chuyenphat nhanh, 0.khong
    
    					values (@ID_XuatKho, @LoaiXuatKho, @maXuatKho,@ID_HoaDon, @ngayXuatKho, @ID_DonVi,@ID_NhanVien, @ID_DoiTuong,
    						@TongGiaTriXuat,0,0,0,0,0, @TongGiaTriXuat,0,@TrangThai,@YeuCau, GETDATE(), @NguoiTao, 
    						concat(@sLoaiXuat,@nhomHoTro) ,@IsChuyenPhatNhanh)
    
    							---- INSERT CT XUATKHO ----
    						insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_ChiTietGoiDV, 
    								ID_DonViQuiDoi, ID_LoHang, SoLuong, TienChietKhau, DonGia, GiaVon, ThanhTien, ThanhToan, 
    							PTChietKhau,  PTChiPhi, TienChiPhi, TienThue, An_Hien, TonLuyKe, GhiChu,  ChatLieu)		
    						select 
    						NEWid(),
    						@ID_XuatKho,
    						row_number() over( order by (select 1)) as SoThuTu,
    						ctsc.ID_ChiTietGoiDV,
    						ctsc.ID_DonViQuiDoi,
    						ctsc.ID_LoHang,
    						ctsc.SoLuong, 
    						ctsc.TienChietKhau, 
    						ctsc.GiaVon, ctsc.GiaVon, ctsc.GiaTri, 
    						0,0,0,0,0,'1', ctsc.TonLuyKe, ctsc.GhiChu,''
    					from 
    					(
    					--- ct hoadon banle or hd sudung GDV
    						select 
    							cttp.ID as ID_ChiTietGoiDV,							
    							cttp.ID_DonViQuiDoi, 
    							cttp.ID_LoHang,
    							cttp.SoLuong,
    							cttp.TienChietKhau,
    							cttp.GiaVon,
    							cttp.GiaVon* cttp.SoLuong as GiaTri,			
    							cttp.TonLuyKe,
    							isnull(cttp.GhiChu,'') as GhiChu
    						from @ctHDNew cttp		
    						where cttp.SoLuong > 0		
    						) ctsc
    
    						
    																
    			end
 
    		
END");
        }
        
        public override void Down()
        {
			Sql("DROP FUNCTION [dbo].[fnGetAllHangHoa_NeedUpdateTonKhoGiaVon]");
        }
    }
}
