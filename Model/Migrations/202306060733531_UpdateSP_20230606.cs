namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20230606 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountProduct_Detail]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs nvarchar(max),
    @ID_NhomHang [nvarchar](max),
	@LaHangHoas [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @TextSearch [nvarchar](max),
    @TextSearchHangHoa [nvarchar](max),
	@TxtCustomer [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
    	set @DateTo = DATEADD(day,1,@DateTo)
    
		declare @tblLoaiHang table (LoaiHang int)
    	insert into @tblLoaiHang
    	select Name from dbo.splitstring(@LaHangHoas)

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)

    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHangHoa_XemDS_PhongBan','BCCKHangHoa_XemDS_HeThong');

		DECLARE @tblDepartment TABLE (ID_PhongBan uniqueidentifier)
		if @DepartmentIDs =''
			insert into @tblDepartment
			select distinct ID_PhongBan from NS_QuaTrinhCongTac pb
		else
			insert into @tblDepartment
			select * from splitstring(@DepartmentIDs)

			----- get nv thuoc PB
		declare @tblNhanVien table (ID uniqueidentifier)
		insert into @tblNhanVien
		select nv.ID
		from @tblNhanVienAll nv
		join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
		where exists (select ID_PhongBan from @tblDepartment pb where pb.ID_PhongBan= ct.ID_PhongBan) 

    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    
    	DECLARE @tblSearchHH TABLE (Name [nvarchar](max));	
    INSERT INTO @tblSearchHH(Name) select  Name from [dbo].[splitstringByChar](@TextSearchHangHoa, ' ') where Name!='';
    DECLARE @countHH int =  (Select count(*) from @tblSearchHH);
    
    	declare @tblIDNhom table (ID uniqueidentifier);
    	if @ID_NhomHang='%%' OR @ID_NhomHang =''
    		begin
    			insert into @tblIDNhom
    			select ID from DM_NhomHangHoa
    		end
    	else
    		begin
    			insert into @tblIDNhom
    			select cast(Name as uniqueidentifier) from dbo.splitstring(@ID_NhomHang)
    		end;
    
    		select 
				ID_HoaDon,
				ID_ChiTietHoaDon,
				ID_DonViQuiDoi,
				ID_LoHang,
				ID_NhanVien,
				MaHoaDon, 
				LoaiHoaDon,
    			NgayLapHoaDon,
    			MaHangHoa,
    			MaNhanVien,
    			TenNhanVien,
    			TenNhomHangHoa,
    			ID_NhomHang,
    			TenHangHoa,
    			TenHangHoaFull,
    			TenDonViTinh,
    			TenLoHang,
    			ThuocTinh_GiaTri,
    			HoaHongThucHien,
    			PTThucHien,
    			HoaHongTuVan,
    			PTTuVan,
    			HoaHongBanGoiDV,
    			PTBanGoi,
    			HoaHongThucHien_TheoYC,
    			PTThucHien_TheoYC,
    			SoLuong,
    			ThanhTien,
    			HeSo,
				ThanhTien * HeSo as GtriSauHeSo,
    			ISNULL(MaDoiTuong,'') as MaKhachHang,
    			ISNULL(TenDoiTuong,N'Khách lẻ') as TenKhachHang,
    			ISNULL(dt.DienThoai,'') as DienThoaiKH,		
				dt.ID_NhanVienPhuTrach,
    		case @Status_ColumHide
    					when  1 then cast(0 as float)
    					when  2 then ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  3 then ISNULL(HoaHongBanGoiDV,0.0)
    					when  4 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  5 then ISNULL(HoaHongTuVan,0.0)
    					when  6 then ISNULL(HoaHongThucHien_TheoYC,0.0) + ISNULL(HoaHongTuVan,0.0)
    					when  7 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  8 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  9 then ISNULL(HoaHongThucHien,0.0)
    					when  10 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  11 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    					when  12 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  13 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  14 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  15 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    		else ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    		end as TongAll
			into #tblHoaHong
    		from
    		(
    				select 
							tbl.ID as ID_ChiTietHoaDon,
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
    						tbl.MaHoaDon,			
    						tbl.LoaiHoaDon,
    						tbl.NgayLapHoaDon,
    						tbl.ID_DoiTuong,
    						tbl.MaHangHoa,
    						tbl.ID_NhanVien,
    						TenHangHoa,
    						CONCAT(TenHangHoa,ThuocTinh_GiaTri) as TenHangHoaFull ,
    						TenDonViTinh,
    						ThuocTinh_GiaTri,
    						TenLoHang,
    						ID_NhomHang,
    						TenNhomHangHoa,
    						SoLuong,
    						HeSo,
    						TrangThaiHD,
							
    						tbl.GiaTriTinhCK_NotCP - iif(tbl.LoaiHoaDon=19,0,tbl.TongChiPhiDV) as ThanhTien,

    						case when LoaiHoaDon=6 then - HoaHongThucHien else HoaHongThucHien end as HoaHongThucHien,
    						case when LoaiHoaDon=6 then - PTThucHien else PTThucHien end as PTThucHien,
    						case when LoaiHoaDon=6 then - HoaHongTuVan else HoaHongTuVan end as HoaHongTuVan,
    						case when LoaiHoaDon=6 then - PTTuVan else PTTuVan end as PTTuVan,
    						case when LoaiHoaDon=6 then - PTBanGoi else PTBanGoi end as PTBanGoi,
    						case when LoaiHoaDon=6 then - HoaHongBanGoiDV else HoaHongBanGoiDV end as HoaHongBanGoiDV,
    						case when LoaiHoaDon=6 then - HoaHongThucHien_TheoYC else HoaHongThucHien_TheoYC end as HoaHongThucHien_TheoYC,
    						case when LoaiHoaDon=6 then - PTThucHien_TheoYC else PTThucHien_TheoYC end as PTThucHien_TheoYC
    				from
    				(Select 
						hdct.ID_HoaDon,
						hdct.ID,
    					hd.MaHoaDon,			
    					hd.LoaiHoaDon,
    					hd.NgayLapHoaDon,
    					hd.ID_DoiTuong,
    					dvqd.MaHangHoa,
						hdct.ID_DonViQuiDoi,
						hdct.ID_LoHang,
    					ck.ID_NhanVien,
						hdct.SoLuong,
						IIF(hdct.TenHangHoaThayThe is null or hdct.TenHangHoaThayThe='', hh.TenHangHoa, hdct.TenHangHoaThayThe) as TenHangHoa,
						iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,    					
    					ISNULL(hh.ID_NhomHang,N'00000000-0000-0000-0000-000000000000') as ID_NhomHang,
    					ISNULL(nhh.TenNhomHangHoa,N'') as TenNhomHangHoa,

						case when hh.ChiPhiTinhTheoPT =1 then hdct.SoLuong * hdct.DonGia * hh.ChiPhiThucHien/100
							else hh.ChiPhiThucHien * hdct.SoLuong end as TongChiPhiDV,

						---- gtri cthd (truoc/sau CK)
						iif(hd.LoaiHoaDon=36,0,case when iif(ck.TinhHoaHongTruocCK is null,0,ck.TinhHoaHongTruocCK) = 1 
							then hdct.SoLuong * hdct.DonGia
							else hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau)
							end) as GiaTriTinhCK_NotCP,

    					ISNULL(dvqd.TenDonVitinh,'')  as TenDonViTinh,
    					ISNULL(lh.MaLoHang,'')  as TenLoHang,
    					ck.HeSo,
    					Case when (dvqd.ThuocTinhGiaTri is null or dvqd.ThuocTinhGiaTri ='') then '' else '_' + dvqd.ThuocTinhGiaTri end as ThuocTinh_GiaTri,
    					Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    						Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTTuVan,
    						Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTBanGoi,
    					Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien_TheoYC,   				
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien_TheoYC,
    						case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    			
    																																		
    				from
    				BH_NhanVienThucHien ck
    				inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    				inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
    				left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    				Where hd.ChoThanhToan is not null
    					and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and hd.NgayLapHoaDon >= @DateFrom 
    					and hd.NgayLapHoaDon < @DateTo   							
    					and (exists (select ID from @tblNhanVien nv where ck.ID_NhanVien = nv.ID))
						and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    						and 
    						((select count(Name) from @tblSearchHH b where     									
    							 dvqd.MaHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
    							or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'	
    							)=@countHH or @countHH=0)
    			) tbl
				where tbl.LoaiHangHoa in (select LoaiHang from @tblLoaiHang)
    			) tblView
    			join NS_NhanVien nv on tblView.ID_NhanVien= nv.ID
    			left join DM_DoiTuong dt on tblView.ID_DoiTuong= dt.ID		
    			where tblView.TrangThaiHD = @StatusInvoice
    			and exists(select ID from @tblIDNhom a where ID_NhomHang= a.ID)
    			and
    				((select count(Name) from @tblSearchString b where     			
    					nv.TenNhanVien like N'%'+b.Name+'%'
    					or nv.TenNhanVienKhongDau like N'%'+b.Name+'%'
    					or nv.TenNhanVienChuCaiDau like N'%'+b.Name+'%'
    					or nv.MaNhanVien like N'%'+b.Name+'%'	
    					or tblView.MaHoaDon like '%'+b.Name+'%'							
    					)=@count or @count=0)	
				and (
				dt.MaDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong_KhongDau like N'%'+  @TxtCustomer +'%'
				or dt.DienThoai like N'%'+  @TxtCustomer +'%'
				)


				declare @TotalRow int, @TotalPage float, 
					@TongHoaHongThucHien float, @TongHoaHongThucHien_TheoYC float,
					@TongHoaHongTuVan float, @TongHoaHongBanGoiDV float,
					@TongAllAll float, @TongSoLuong float,
					@TongThanhTien float, @TongThanhTienSauHS float

				---- count all row		
				select 
					@TotalRow= count(tbl.ID_HoaDon),
    				@TotalPage= CEILING(COUNT(tbl.ID_HoaDon ) / CAST(@PageSize as float )) ,
    				@TongHoaHongThucHien= sum(HoaHongThucHien) ,
    				@TongHoaHongThucHien_TheoYC = sum(HoaHongThucHien_TheoYC),
    				@TongHoaHongTuVan = sum(HoaHongTuVan),
    				@TongHoaHongBanGoiDV = sum(HoaHongBanGoiDV),
					@TongAllAll = sum(TongAll)
				from #tblHoaHong tbl

				---- sum and group by hoadon + idquydoi
				select 
					@TongSoLuong= sum(SoLuong) ,			   				
    				@TongThanhTien = sum(ThanhTien),
					@TongThanhTienSauHS= sum(GtriSauHeSo) 
				from 
				(
					select  
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
							max(tbl.SoLuong) as SoLuong,
							max(tbl.ThanhTien) as ThanhTien,
							max(tbl.GtriSauHeSo) as GtriSauHeSo
					from #tblHoaHong tbl
					group by tbl.ID_HoaDon , tbl.ID_DonViQuiDoi ,tbl.ID_LoHang	, tbl.ID_ChiTietHoaDon	
				) tbl
				

				select tbl.*, 
					isnull(nvpt.TenNhanVien,'') as TenNVPhuTrach,
					@TotalRow as TotalRow,
					@TotalPage as TotalPage,
					@TongHoaHongThucHien as TongHoaHongThucHien,
					@TongHoaHongThucHien_TheoYC as TongHoaHongThucHien_TheoYC,
					@TongHoaHongTuVan as TongHoaHongTuVan,
					@TongHoaHongBanGoiDV as TongHoaHongBanGoiDV,
					@TongAllAll as TongAllAll,
					@TongSoLuong as TongSoLuong,
					@TongThanhTien as TongThanhTien,
					@TongThanhTienSauHS as TongThanhTienSauHS
				from #tblHoaHong tbl
				left join NS_NhanVien nvpt on tbl.ID_NhanVienPhuTrach= nvpt.ID
				order by tbl.NgayLapHoaDon desc
    			OFFSET (@CurrentPage* @PageSize) ROWS
    			FETCH NEXT @PageSize ROWS ONLY

				
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

			Sql(@"ALTER PROCEDURE [dbo].[GetListHoaDon_byIDCus]
    @ID_NguoiGioiThieu [uniqueidentifier],
    @ID_DoiTuong [uniqueidentifier],
    @IDChiNhanhs [nvarchar](max),
    @DateFrom [datetime],
    @DateTo [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	if isnull(@IDChiNhanhs,'') !=''
    		insert into @tblChiNhanh
    		select name from dbo.splitstring(@IDChiNhanhs)
    
    
    	select *,
    	tblView.KhachDaTra - tblView.DaTrich as ConLai,
    	iif(tblView.KhachDaTra > tblView.DaTrich, 0,1) as TrangThai --- 0.chua trichdu, 1.da trich du
    	from
    	(
    		select 
    			allHD.*,
    			isnull(tblDaTrich.DaTrich,0) as DaTrich,
    			tblDaTrich.ID_NguoiGioiThieu
    			
    		from
    		(
    			------ get allhoadon of cus & soquy dathanhtoan
    			select hd.ID as ID_HoaDon_DuocCK,				
    					hd.MaHoaDon,
    					hd.NgayLapHoaDon,
    					hd.TongThanhToan,
    					hd.ID_DoiTuong,
    					isnull(sum(iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu)),0) as KhachDaTra
    			from
    			(
    				---- only get hd have TongThanhToan > 0
    				select hd.ID, hd.ID_DoiTuong, hd.LoaiHoaDon, hd.MaHoaDon, hd.NgayLapHoaDon,hd.TongThanhToan
    				from BH_HoaDon hd
    				where hd.ID_DoiTuong= @ID_DoiTuong
    				and hd.TongThanhToan > 0
    				and hd.LoaiHoaDon in (1,19,22)
    				and hd.ChoThanhToan='0'
    				and hd.NgayLapHoaDon between @DateFrom and @DateTo
    				and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID))
    			) hd
    			join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan 
    			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 	
    			where (qhd.TrangThai is null or qhd.TrangThai='1') --- chi get hd dathanhtoan (vi hoahong tinh theo thuc thu)
    			and qct.HinhThucThanhToan not in (4,5,6)
    			group by hd.ID, hd.LoaiHoaDon, hd.ID_DoiTuong, hd.MaHoaDon, hd.NgayLapHoaDon,hd.TongThanhToan
    		) allHD
    		left join
    		(
    			----- get all hoadon da duoc trich hoahong of cus
    			select 
    				hd.ID,		
    				pthh.ID_CheckIn as ID_NguoiGioiThieu,
    				pthh.TongChietKhau as LoaiDoiTuong,
    				sum(isnull(cthh.TienThue,0)) as DaTrich --- sotien thucte da trich cho khach
    			from BH_HoaDon_ChiTiet cthh
    			join BH_HoaDon pthh on cthh.ID_HoaDon = pthh.ID
    			join BH_HoaDon hd on cthh.ID_ParentCombo= hd.ID
    			where pthh.LoaiHoaDon = 41 
    			and pthh.ChoThanhToan='0'
    			and pthh.ID_CheckIn =  @ID_NguoiGioiThieu
    			and hd.ID_DoiTuong= @ID_DoiTuong
    			and hd.ChoThanhToan='0'
    			and hd.NgayLapHoaDon between @DateFrom and @DateTo
    			and cthh.TienChietKhau > 0
    			and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where pthh.ID_DonVi= cn.ID))
    			group by  hd.ID, pthh.ID_CheckIn, pthh.TongChietKhau
    		
    		) tblDaTrich on allHD.ID_HoaDon_DuocCK = tblDaTrich.ID		
    	) tblView
    	order by tblView.NgayLapHoaDon 
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_ThuChi_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    SELECT 
    MAX(b.TenNhomDoiTuong) as NhomDoiTuong,
    b.MaHoaDon,
    MAX(b.MaPhieuThu) as MaPhieuThu,
    MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    MAX(b.ManguoiNop) as ManguoiNop, 
    MAX(b.TenNguoiNop) as TenNguoiNop, 
	MAX(b.TienMat) AS TienMat,
	MAX(b.TienGui) AS TienGui,
	MAX(b.TienPOS) AS TienPOS,
    MAX(b.ThuChi) as ThuChi, 
    MAX(b.NoiDungThuChi) as NoiDungThuChi,
    MAX(b.GhiChu) as GhiChu,
    MAX(b.LoaiThuChi) as LoaiThuChi,
    	dv.TenDonVi AS TenChiNhanh,
		b.SoTaiKhoan, b.TenNganHang
    FROM
    (
    	  select 
    		a.ID_DoiTuong,
    		a.ID_HoaDon,
    		a.TenNhomDoiTuong,
    		a.ID_NhomDoiTuong,
    		a.MaHoaDon,
    		a.MaPhieuThu,
    		a.NgayLapHoaDon,
    		a.MaNguoiNop,
    		a.TenNguoiNop,
    		--a.ThuChi,
			a.TienMat,
			a.TienGui,
			a.TienPOS,
    		a.TienMat + a.TienGui + a.TienPOS as ThuChi,
    		a.NoiDungThuChi,
    		a.GhiChu,
    		Case when a.LoaiThuChi = 1 then N'Phiếu thu khác'  
    		when a.LoaiThuChi = 2 then N'Phiếu chi khác' 
    		when a.LoaiThuChi = 3 then N'Thu tiền khách trả'  
    		when a.LoaiThuChi = 4 then N'Chi tiền đổi trả hàng'  
    		when a.LoaiThuChi = 5 then N'Thu tiền nhà NCC'  
    		when a.LoaiThuChi = 6 then N'Chi tiền trả NCC' else '' end as LoaiThuChi,
    		a.ID_DonVi,
			a.SoTaiKhoan,
			a.TenNganHang
    	From
    	(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			tknh.SoTaiKhoan as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
    				--Max(dt.TenNhomDoiTuongs) as TenNhomDoiTuong,
    				case when qhdct.ID_NhanVien is not null then N'Nhân viên' else MAX(dt.TenNhomDoiTuongs) end as TenNhomDoiTuong,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 32) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case WHEN qhdct.ID_NhanVien is not null
    				then
    				'00000000-0000-0000-0000-000000000000' 
    				else 
    				case When dtn.ID_NhomDoiTuong is null 
    					
    				then '00000000-0000-0000-0000-000000000000'  else dtn.ID_NhomDoiTuong 
    				end
    				end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			--qhd.NguoiNopTien as TenNguoiNop,
				case when qhdct.ID_NhanVien is not null then nv.TenNhanVien else dt.TenDoiTuong end as TenNguoiNop,
    				case when qhdct.ID_NhanVien is not null then nv.MaNhanVien else dt.MaDoiTuong end as ManguoiNop,
    			Sum(qhdct.TienMat) as TienMat,
    			IIF(tknh.TaiKhoanPOS = 1, 0, Sum(qhdct.TienGui)) as TienGui,
				IIF(tknh.TaiKhoanPOS = 1, SUM(qhdct.TienGui), 0) AS TienPOS,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
    				qhd.ID_DonVi,
				nh.TenNganHang
    		From Quy_HoaDon qhd 			
    			join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    				left join NS_NhanVien nv on qhdct.ID_NhanVien= nv.ID
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on tknh.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart and @timeEnd 
    				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, dt.loaidoituong) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and qhdct.HinhThucThanhToan NOT IN (4, 5, 6)
    				AND ((select count(Name) from @tblSearch b where     			
    			dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or qhd.MaHoaDon like '%' + b.Name + '%'
    				or hd.MaHoaDon like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhdct.ID_NhanVien, dt.MaDoiTuong,dt.LoaiDoiTuong,  nv.MaNhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID, qhd.ID_DonVi,
				 tknh.TaiKhoanPOS, tknh.SoTaiKhoan, nh.TenNganHang, nv.TenNhanVien, dt.TenDoiTuong
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) 
    	) b
    		inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    		where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = ''
    	Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi, dv.TenDonVi, b.SoTaiKhoan, b.TenNganHang
    	ORDER BY NgayLapHoaDon DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[CheckThucThu_TongSuDung]
    @ID_DoiTuong [uniqueidentifier],
    @ID_TheGiaTri [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tongthu float= 0, @tongtiendieuchinh float = 0
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
    	
    
    
    				if isnull(@tongtiendieuchinh,0) +  isnull(@tongthu,0) + isnull(@tongkhuyenmai,0) < isnull(@tongsudung,0)
    					set @return='0'
			end
			
		
    	select @return as Exist
		
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateGiaVon_WhenEditCTHD]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier],
    @NgayLapHDMin [datetime]
AS
BEGIN
    SET NOCOUNT ON;	
	

    		declare @tblCTHD ChiTietHoaDonEdit
    		INSERT INTO @tblCTHD
    		SELECT 
    			qd.ID_HangHoa, ct.ID_LoHang, ct.ID_DonViQuiDoi, qd.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet ct
    		INNER JOIN BH_HoaDon hd ON hd.ID = ct.ID_HoaDon			
    		INNER JOIN DonViQuiDoi qd ON qd.ID = ct.ID_DonViQuiDoi			
    		INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa    		
    		WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1
    		GROUP BY qd.ID_HangHoa,ct.ID_DonViQuiDoi,qd.TyLeChuyenDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;	
    
    		-- get cthd can update GiaVon
    		DECLARE @cthd_NeedUpGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT, TongChiPhi FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @cthd_NeedUpGiaVon  	
		select hd.ID as IDHoaDon,
			hd.ID_HoaDon as IDHoaDonGoc, 
			hd.MaHoaDon, 
			hd.LoaiHoaDon,
			ct.ID as ID_ChiTietHoaDon, 
    		CASE WHEN hd.YeuCau = '4' AND @IDChiNhanh = hd.ID_CheckIn THEN hd.NgaySua ELSE hd.NgayLapHoaDon END AS NgayLapHoaDon, 				    			    				    							    			
    		ct.SoThuTu,
			iif(ct.ChatLieu='5',0,ct.SoLuong) as SoLuong, 
			ct.DonGia, 
			hd.TongTienHang, 
			isnull(hd.TongChiPhi,0) as TongChiPhi,
			ct.TienChietKhau,
			ct.ThanhTien, 
			hd.TongGiamGia, 
			qd.TyLeChuyenDoi,
			[dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanh, hh.ID, ct.ID_LoHang, 
			CASE WHEN hd.YeuCau = '4' AND @IDChiNhanh = hd.ID_CheckIn THEN hd.NgaySua ELSE hd.NgayLapHoaDon END) as TonKho, 	 	    	
    		ct.GiaVon / qd.TyLeChuyenDoi as GiaVon, 
    		ct.GiaVon_NhanChuyenHang / qd.TyLeChuyenDoi as GiaVonNhan,
    		hh.ID as ID_HangHoa, 
			hh.LaHangHoa, 
			qd.ID as IDDonViQuiDoi, 
			ct.ID_LoHang, 
			ct.ID_ChiTietDinhLuong, 
    		@IDChiNhanh as IDChiNhanh,
			hd.ID_CheckIn,
			hd.YeuCau 
    	FROM BH_HoaDon_ChiTiet ct 
    	INNER JOIN DonViQuiDoi qd ON ct.ID_DonViQuiDoi = qd.ID   	
    	INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa   	
    	INNER JOIN BH_HoaDon hd  ON ct.ID_HoaDon = hd.ID   
    	WHERE hd.ChoThanhToan = 0    		
			and hd.LoaiHoaDon NOT IN (3, 19, 25,29)
			---- dont join ctEdit because douple row (only check exists ID_QuiDoi, ID_Lo)
			and exists (select ID_HangHoa from @tblCTHD ctNew where ctNew.ID_HangHoa = qd.ID_HangHoa  and (ct.ID_LoHang = ctNew.ID_LoHang OR ctNew.ID_LoHang IS NULL))
    			AND
    			((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon >= @NgayLapHDMin
    				and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    				or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua >= @NgayLapHDMin))
    
    			---- BEGIN get giavon dauky by id_quidoi =========
    		DECLARE @TinhGiaVonTrungBinh BIT;
    		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanh;
    		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
    		BEGIN
    		
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT,
			TongTienHang FLOAT, TongChiPhi float,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select
    				hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu,
					iif(hdct.ChatLieu='5',0, hdct.SoLuong) as SoLuong, 
					hdct.DonGia, 
					hd.TongTienHang, 
					isnull(hd.TongChiPhi,0) as TongChiPhi,
    				iif(hdct.ChatLieu='5',0,hdct.TienChietKhau) as TienChietKhau, 
					hdct.ThanhTien, hd.TongGiamGia, 
    				dvqd.TyLeChuyenDoi, 0, hdct.GiaVon / dvqd.TyLeChuyenDoi as GiaVon, 
    			hdct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi  as GiaVonNhan, 
    			hh.ID, hh.LaHangHoa, hdct.ID_DonViQuiDoi, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, 
    				@IDChiNhanh, hd.ID_CheckIn, hd.YeuCau
			FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct ON hd.ID = hdct.ID_HoaDon    	
    		INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID    		
    		INNER JOIN DM_HangHoa hh on hh.ID = dvqd.ID_HangHoa    		
    		INNER JOIN @tblCTHD cthdthemmoi ON cthdthemmoi.ID_HangHoa = hh.ID    		
    		WHERE hd.ChoThanhToan = 0 
				and hd.LoaiHoaDon NOT IN (3, 19, 25,29)
    				AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) 
    				AND
    				((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon < @NgayLapHDMin and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    					or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua < @NgayLapHDMin))
    	   			

		DECLARE @ChiTietHoaDonGiaVon1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT,
			TongTienHang FLOAT, TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
			
			INSERT INTO @ChiTietHoaDonGiaVon1
			SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    					FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon1 WHERE cthdGiaVon1.RN = 1;
			---- === END giavon dauky ========= ---

    		------ union 2 table: ctEdit + ctNeddUpdateGV: partition by idhang,idlohang order by NgayLapHoaDon
    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT,
			TongTienHang FLOAT, TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @BangUpdateGiaVon
    		SELECT *, 
    				ROW_NUMBER() OVER (PARTITION BY tableUpdateGiaVon.ID_HangHoa, tableUpdateGiaVon.ID_LoHang ORDER BY tableUpdateGiaVon.NgayLapHoaDon) AS RN 
    			FROM
    			(
					SELECT * 
    					FROM @cthd_NeedUpGiaVon
    				UNION ALL
    					SELECT 
    						gvDauKy.IDHoaDon, gvDauKy.IDHoaDonBan, gvDauKy.MaHoaDon, gvDauKy.LoaiHoaDon, gvDauKy.ID_ChiTietHoaDon, gvDauKy.NgayLapHoaDon,
    						gvDauKy.SoThuThu, gvDauKy.SoLuong, gvDauKy.DonGia, gvDauKy.TongTienHang, gvDauKy.TongChiPhi,
    						gvDauKy.ChietKhau, gvDauKy.ThanhTien, gvDauKy.TongGiamGia, gvDauKy.TyLeChuyenDoi, gvDauKy.TonKho, 
    						CASE WHEN gvDauKy.GiaVon IS NULL THEN 0 ELSE gvDauKy.GiaVon END AS GiaVon, 											
    						CASE WHEN gvDauKy.GiaVonNhan IS NULL THEN 0 ELSE gvDauKy.GiaVonNhan END AS GiaVonNhan,								
    						cthd1.ID_HangHoa, gvDauKy.LaHangHoa, gvDauKy.IDDonViQuiDoi, cthd1.ID_LoHang , gvDauKy.ID_ChiTietDinhLuong,
    					@IDChiNhanh as ID_ChiNhanh, gvDauKy.ID_CheckIn, gvDauKy.YeuCau 
    					FROM @tblCTHD cthd1
    					LEFT JOIN @ChiTietHoaDonGiaVon1 AS gvDauKy ON cthd1.ID_HangHoa = gvDauKy.ID_HangHoa   					   				
    					AND (cthd1.ID_LoHang = gvDauKy.ID_LoHang OR gvDauKy.ID_LoHang IS NULL)				
			 ) tableUpdateGiaVon
    
    			-- caculator again GiaVon by ID_HangHoa
    			DECLARE @TableTrungGianUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER,IDHangHoa UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, GiaVonNhapHang FLOAT, GiaVonNhanHang FLOAT)
    			INSERT INTO @TableTrungGianUpDate
    			SELECT 
    				IDHoaDon, ID_HangHoa, ID_LoHang, 
					sum(GiaVon) as GiaVonNhapHang,
					sum(GiaVonNhan) as GiaVonNhanHang
    			FROM @BangUpdateGiaVon
    			WHERE IDHoaDon = @IDHoaDonInput and RN= 1
    			GROUP BY ID_HangHoa, ID_LoHang, IDHoaDon,LoaiHoaDon
    			
    			
    
    			--DECLARE @RNCheck INT;
    			--SELECT @RNCheck = MAX(RN) FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang
    			--IF(@RNCheck = 1)
    			--BEGIN
    			--	UPDATE @BangUpdateGiaVon SET RN = 2
    			--END

			update gv set gv.RN = 2
				 from @BangUpdateGiaVon gv
				 join 
						(select  MAX(RN) as RN, ID_HangHoa, ID_LoHang FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang 
				) tbl on gv.ID_HangHoa = tbl.ID_HangHoa and (gv.ID_LoHang = tbl.ID_LoHang or gv.ID_LoHang is null and tbl.ID_LoHang is null)
				 where tbl.RN= 1
    
    			---- update GiaVon, GiaVonNhan to @BangUpdateGiaVon if Loai =(4,10,13), else keep old
    		UPDATE bhctup 
    			SET bhctup.GiaVon = 
    			CASE WHEN bhctup.LoaiHoaDon in (4,13) THEN giavontbup.GiaVonNhapHang	    						
    			ELSE bhctup.GiaVon END,    		
    			bhctup.GiaVonNhan = 
    				CASE WHEN bhctup.LoaiHoaDon = 10 AND bhctup.YeuCau = '4' AND bhctup.ID_CheckIn = ID_ChiNhanhThemMoi THEN giavontbup.GiaVonNhanHang   		    			
    			ELSE bhctup.GiaVonNhan END  		
    			FROM @BangUpdateGiaVon bhctup
    			JOIN @TableTrungGianUpDate giavontbup on bhctup.IDHoaDon =giavontbup.IDHoaDon and bhctup.ID_HangHoa = giavontbup.IDHangHoa and (bhctup.ID_LoHang = giavontbup.IDLoHang or giavontbup.IDLoHang is null)
    		WHERE bhctup.IDHoaDon = @IDHoaDonInput AND bhctup.RN = 1;
    			--END tính giá vốn trung bình cho lần nhập hàng và chuyển hàng đầu tiền
    
    		DECLARE @GiaVonCapNhat TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, IDHoaDonCu UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, IDChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT, TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, GiaVonCu FLOAT, IDHangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, IDChiTietDinhLuong UNIQUEIDENTIFIER,
    		IDChiNhanhThemMoi UNIQUEIDENTIFIER, IDCheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @GiaVonCapNhat
    		SELECT 
    				tableUpdate.IDHoaDon, tableUpdate.IDHoaDonBan, tableGiaVon.IDHoaDon, tableUpdate.MaHoaDon, tableUpdate.LoaiHoaDon, tableUpdate.ID_ChiTietHoaDon,tableUpdate.NgayLapHoaDon, tableUpdate.SoLuong, tableUpdate.DonGia,
    			tableUpdate.TongTienHang, tableUpdate.TongChiPhi,
				tableUpdate.ChietKhau, tableUpdate.ThanhTien, tableUpdate.TongGiamGia, tableUpdate.TyLeChuyenDoi, tableUpdate.TonKho, tableUpdate.GiaVon, tableUpdate.GiaVonNhan, tableGiaVon.GiaVon, tableUpdate.ID_HangHoa, tableUpdate.LaHangHoa,
    			tableUpdate.IDDonViQuiDoi, tableUpdate.ID_LoHang, tableUpdate.ID_ChiTietDinhLuong, tableUpdate.ID_ChiNhanhThemMoi, tableUpdate.ID_CheckIn, tableUpdate.YeuCau, tableUpdate.RN 
    			FROM @BangUpdateGiaVon tableUpdate
    		LEFT JOIN (SELECT (CASE WHEN ID_CheckIn = ID_ChiNhanhThemMoi THEN GiaVonNhan ELSE GiaVon END) AS GiaVon, ID_HangHoa, IDHoaDon, ID_LoHang, RN + 1 AS RN 
    						FROM @BangUpdateGiaVon) AS tableGiaVon
    		ON tableUpdate.ID_HangHoa = tableGiaVon.ID_HangHoa AND tableUpdate.RN = tableGiaVon.RN 
    			AND (tableUpdate.ID_LoHang = tableGiaVon.ID_LoHang OR tableUpdate.ID_LoHang IS NULL);
    
    			--select * from @GiaVonCapNhat
    			
    		DECLARE @IDHoaDon UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonBan UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonCu UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX);
    		DECLARE @LoaiHoaDon INT;
    		DECLARE @IDChiTietHoaDon UNIQUEIDENTIFIER;
    		DECLARE @SoLuong FLOAT;
    		DECLARE @DonGia FLOAT;
    		DECLARE @TongTienHang float, @TongChiPhi float;
    		DECLARE @ChietKhau FLOAT;
    		DECLARE @ThanhTien FLOAT;
    		DECLARE @TongGiamGia FLOAT;
    		DECLARE @TyLeChuyenDoi FLOAT;
    		DECLARE @TonKho FLOAT;
    		DECLARE @GiaVonCu FLOAT;
    		DECLARE @IDHangHoa UNIQUEIDENTIFIER;
    		DECLARE @IDDonViQuiDoi UNIQUEIDENTIFIER;
    		DECLARE @IDLoHang UNIQUEIDENTIFIER;
    		DECLARE @IDChiNhanhThemMoi UNIQUEIDENTIFIER;
    		DECLARE @IDCheckIn UNIQUEIDENTIFIER;
    		DECLARE @YeuCau NVARCHAR(MAX);
    		DECLARE @RN INT;
    			DECLARE @GiaVon FLOAT;
    			DECLARE @GiaVonNhan FLOAT;
    		DECLARE @GiaVonMoi FLOAT;
    		DECLARE @GiaVonCuUpdate FLOAT;
    		DECLARE @IDHangHoaUpdate UNIQUEIDENTIFIER;
    		DECLARE @IDLoHangUpdate UNIQUEIDENTIFIER;
    			DECLARE @GiaVonHoaDonBan FLOAT;
    		DECLARE @TongTienHangDemo FLOAT;
    		DECLARE @SoLuongDemo FLOAT;
    			DECLARE @ThanhTienDemo FLOAT;
    			DECLARE @ChietKhauDemo FLOAT;
    
    		DECLARE CS_GiaVon CURSOR SCROLL LOCAL FOR 
    			SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, TongChiPhi,
				ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
    			GiaVonCu, IDHangHoa, IDDonViQuiDoi, IDLoHang, IDChiNhanhThemMoi, IDCheckIn, YeuCau, RN, GiaVon, GiaVonNhan 
    			FROM @GiaVonCapNhat WHERE RN > 1 and LaHangHoa = 1 ORDER BY IDHangHoa, RN
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon 
    			INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau,@ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    			iF(@IDHangHoaUpdate = @IDHangHoa AND (@IDLoHangUpdate = @IDLoHang OR @IDLoHang IS NULL))
    			BEGIN
    				SET @GiaVonCu = @GiaVonCuUpdate;
    			END
    			ELSE
    			BEGIN
    				SET @IDHangHoaUpdate = @IDHangHoa;
    				SET @IDLoHangUpdate = @IDLoHang;
    					SET @GiaVonCuUpdate = @GiaVonCu;
    			END
    				IF(@GiaVonCu IS NOT NULL)
    				BEGIN
    				IF(@LoaiHoaDon in (4,13))
    				BEGIN
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia -  bhctdm.ChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
        					
    					IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    					BEGIN
    						IF(@TongTienHang != 0)
    						BEGIN
								------ giavon: tinh sau khi tru phi ship (!! khong tru giamgiaHD)
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo
								+ (@TongTienHangDemo* @TongChiPhi/@TongTienHang))/(@SoLuongDemo + @TonKho);
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
									SET	@GiaVonMoi = (@TongTienHangDemo + (@TongTienHangDemo * @TongChiPhi/@TongTienHang))/ @SoLuongDemo
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
    
    						--select @GiaVonMoi
    				END
    				ELSE IF (@LoaiHoaDon = 7)
    				BEGIN
    					--select @IDHoaDon, @MaHoaDon, @TongTienHangDemo, @SoLuongDemo, @TonKho
    						
    					SELECT @TongTienHangDemo = 
							SUM(bhctdm.SoLuong * bhctdm.DonGia * ( 1- bhctdm.TongGiamGia/iif(bhctdm.TongTienHang=0,1,bhctdm.TongTienHang))) ,
							@SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@TonKho - @SoLuongDemo > 0)
    					BEGIN
    						SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) - @TongTienHangDemo)/(@TonKho - @SoLuongDemo);
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    						--select @GiaVonMoi
    				END
    				ELSE IF(@LoaiHoaDon = 10)
    				BEGIN
    					SELECT @TongTienHangDemo = SUM(bhctdm.ChietKhau * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.ChietKhau * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    
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
    				END
    				ELSE IF (@LoaiHoaDon = 6)
    				BEGIN
    					SELECT @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@IDHoaDonBan IS NOT NULL)
    					BEGIN
    						SET @GiaVonHoaDonBan = -1;
    						SELECT @GiaVonHoaDonBan = GiaVon FROM @GiaVonCapNhat WHERE IDHoaDon = @IDHoaDonBan AND IDDonViQuiDoi = @IDDonViQuiDoi AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL);
    						
    						IF(@GiaVonHoaDonBan = -1)
    						BEGIN
    							
    							SELECT @GiaVonHoaDonBan = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDonBan AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
    						
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
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    				END
    				ELSE IF(@LoaiHoaDon = 18)
    					BEGIN
    						SELECT @GiaVonMoi = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDon AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
    					END
    					ELSE
    				BEGIN
    					SET @GiaVonMoi = @GiaVonCu;
    				END
    				END
    				ELSE
    				BEGIN
    					IF(@IDCheckIn = @IDChiNhanhThemMoi)
    					BEGIN
    						SET @GiaVonMoi = @GiaVonNhan
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVon
    					END
    				END
    
    			IF(@IDHoaDon = @IDHoaDonCu)
    			BEGIN
    				SET @GiaVonMoi = @GiaVonCuUpdate;	
    			END
    			ELSE
    			BEGIN
    				SET @GiaVonCuUpdate = @GiaVonMoi;
    			END
    			IF(@LoaiHoaDon = 10 AND @YeuCau = '4' AND @IDCheckIn = @IDChiNhanhThemMoi)
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVonNhan = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			ELSE
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVon = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon		
			
			update gv set GiaVonCu = isnull(GiaVonCu,0)
			from @GiaVonCapNhat gv 
    

    		---- =========Update BH_HoaDon_ChiTiet
			begin try
    		UPDATE hoadonchitiet1
    		SET hoadonchitiet1.GiaVon = _giavoncapnhat1.GiaVon * _giavoncapnhat1.TyLeChuyenDoi,
    			hoadonchitiet1.GiaVon_NhanChuyenHang = _giavoncapnhat1.GiaVonNhan * _giavoncapnhat1.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet1
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat1 ON hoadonchitiet1.ID = _giavoncapnhat1.IDChiTietHoaDon   		
    		WHERE _giavoncapnhat1.LoaiHoaDon != 8 AND _giavoncapnhat1.LoaiHoaDon != 18 AND _giavoncapnhat1.LoaiHoaDon != 9 AND _giavoncapnhat1.RN > 1;
    
    		---- update GiaVon to phieu KiemKe
    			UPDATE hoadonchitiet4
    		SET hoadonchitiet4.GiaVon = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi, 
    			hoadonchitiet4.ThanhToan = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi *hoadonchitiet4.SoLuong
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet4
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat4 ON hoadonchitiet4.ID = _giavoncapnhat4.IDChiTietHoaDon    		
    		WHERE _giavoncapnhat4.LoaiHoaDon = 9 AND _giavoncapnhat4.RN > 1;
    
    			-- update GiaVon to phieu XuatKho
    		UPDATE hoadonchitiet2
    		SET hoadonchitiet2.GiaVon = _giavoncapnhat2.GiaVon * _giavoncapnhat2.TyLeChuyenDoi, 
				--hoadonchitiet2.DonGia = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong* _giavoncapnhat2.TyLeChuyenDoi,
    			hoadonchitiet2.ThanhTien = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong* _giavoncapnhat2.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet2
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat2 ON hoadonchitiet2.ID = _giavoncapnhat2.IDChiTietHoaDon    		
    		WHERE _giavoncapnhat2.LoaiHoaDon = 8 AND _giavoncapnhat2.RN > 1;
    
    			-- update GiaVon to Loai = 18 (Phieu DieuChinh GiaVon)
    		UPDATE hoadonchitiet3
    		SET hoadonchitiet3.DonGia = _giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi, 
    				hoadonchitiet3.PTChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) ELSE 0 END,
    			hoadonchitiet3.TienChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN 0 ELSE hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) END
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet3
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat3
    		ON hoadonchitiet3.ID = _giavoncapnhat3.IDChiTietHoaDon
    		WHERE _giavoncapnhat3.LoaiHoaDon = 18 AND _giavoncapnhat3.RN > 1;
    
    		UPDATE chitietdinhluong
    		SET chitietdinhluong.GiaVon = gvDinhLuong.GiaVonDinhLuong / iif(chitietdinhluong.SoLuong=0,1,chitietdinhluong.SoLuong)
    		FROM BH_HoaDon_ChiTiet AS chitietdinhluong
    		INNER JOIN
    			(SELECT 
    					SUM(ct.GiaVon * ct.SoLuong) AS GiaVonDinhLuong, ct.ID_ChiTietDinhLuong 
    				FROM BH_HoaDon_ChiTiet ct
    			INNER JOIN (SELECT IDChiTietDinhLuong FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDChiTietDinhLuong) gv
    			ON (ct.ID_ChiTietDinhLuong = gv.IDChiTietDinhLuong and ct.ID_ChiTietDinhLuong IS NOT NULL)
    			WHERE gv.IDChiTietDinhLuong IS NOT NULL AND ct.ID != ct.ID_ChiTietDinhLuong
    			GROUP BY ct.ID_ChiTietDinhLuong) AS gvDinhLuong
    		ON chitietdinhluong.ID = gvDinhLuong.ID_ChiTietDinhLuong
			end try
			begin catch
				select error_number() as ErrNumber,				
					ERROR_LINE() as ErrLine,
					ERROR_MESSAGE() as ErrMsg
			end catch
    		----=========END Update BH_HoaDon_ChiTiet

    		--Update DM_GiaVon
    		UPDATE _dmGiaVon1
    		SET _dmGiaVon1.GiaVon = ISNULL(_gvUpdateDM1.GiaVon, 0)
    		FROM 
    				(SELECT dvqd1.ID AS IDDonViQuiDoi, _giavon1.IDLoHang AS IDLoHang, (CASE WHEN _giavon1.IDCheckIn = _giavon1.IDChiNhanhThemMoi THEN _giavon1.GiaVonNhan ELSE _giavon1.GiaVon END) * dvqd1.TyLeChuyenDoi AS GiaVon, _giavon1.IDChiNhanhThemMoi AS IDChiNhanh 
    				FROM @GiaVonCapNhat _giavon1
    			INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN > 1 GROUP BY IDHangHoa,IDLoHang) AS _maxGiaVon1
    			ON _giavon1.IDHangHoa = _maxGiaVon1.IDHangHoa AND _giavon1.RN = _maxGiaVon1.RN AND (_giavon1.IDLoHang = _maxGiaVon1.IDLoHang OR _maxGiaVon1.IDLoHang IS NULL)
    			INNER JOIN DonViQuiDoi dvqd1
    			ON dvqd1.ID_HangHoa = _giavon1.IDHangHoa) AS _gvUpdateDM1
    		LEFT JOIN DM_GiaVon _dmGiaVon1
    		ON _gvUpdateDM1.IDChiNhanh = _dmGiaVon1.ID_DonVi AND _gvUpdateDM1.IDDonViQuiDoi = _dmGiaVon1.ID_DonViQuiDoi AND (_gvUpdateDM1.IDLoHang = _dmGiaVon1.ID_LoHang OR _gvUpdateDM1.IDLoHang IS NULL)
    		WHERE _dmGiaVon1.ID IS NOT NULL;
    
    		INSERT INTO DM_GiaVon (ID, ID_DonVi, ID_DonViQuiDoi, ID_LoHang, GiaVon)
    		SELECT NEWID(), _gvUpdateDM.IDChiNhanh, _gvUpdateDM.IDDonViQuiDoi, _gvUpdateDM.IDLoHang, _gvUpdateDM.GiaVon 
    			FROM 
    			(SELECT dvqd2.ID AS IDDonViQuiDoi, _giavon2.IDLoHang AS IDLoHang, 
    					(CASE WHEN _giavon2.IDCheckIn = _giavon2.IDChiNhanhThemMoi THEN _giavon2.GiaVonNhan ELSE _giavon2.GiaVon END) * dvqd2.TyLeChuyenDoi AS GiaVon, 
    					_giavon2.IDChiNhanhThemMoi AS IDChiNhanh 
    				FROM @GiaVonCapNhat _giavon2
    			INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDHangHoa, IDLoHang) AS _maxGiaVon
    		ON _giavon2.IDHangHoa = _maxGiaVon.IDHangHoa AND _giavon2.RN = _maxGiaVon.RN AND (_giavon2.IDLoHang = _maxGiaVon.IDLoHang OR _maxGiaVon.IDLoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd2 ON dvqd2.ID_HangHoa = _giavon2.IDHangHoa) AS _gvUpdateDM    		
    		LEFT JOIN DM_GiaVon _dmGiaVon
    		ON _gvUpdateDM.IDChiNhanh = _dmGiaVon.ID_DonVi AND _gvUpdateDM.IDDonViQuiDoi = _dmGiaVon.ID_DonViQuiDoi AND (_gvUpdateDM.IDLoHang = _dmGiaVon.ID_LoHang OR _gvUpdateDM.IDLoHang IS NULL)
    		WHERE _dmGiaVon.ID IS NULL;
    		
			delete from BH_HoaDon_ChiTiet where ID_HoaDon = @IDHoaDonInput and ChatLieu='5'

    		END
    		--END TinhGiaVonTrungBinh
END");

            Sql(@"ALTER PROCEDURE [dbo].[BCThuChi_TheoLoaiTien]
    @IDChiNhanhs [nvarchar](max) = '',
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiDoiTuongs [nvarchar](max) = '1',
    @KhoanMucThuChis [nvarchar](max) = ''
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	if isnull(@IDChiNhanhs,'') !=''
    		insert into @tblChiNhanh
    		select Name from dbo.splitstring(@IDChiNhanhs);
    
    	declare @tblKhoanThuChi table (ID uniqueidentifier)
    	if isnull(@KhoanMucThuChis,'') !=''
    		insert into @tblKhoanThuChi
    		select Name from dbo.splitstring(@KhoanMucThuChis);
    	
    	;with data_cte
    	as(
    	select 
    		FORMAT(cast (tbl.NgayLapYYYYMMDD as date),'dd/MM/yyyy') as NgayLapYYYYMMDD,
    		sum(ThuTienMat) as ThuTienMat,
    		sum(ThuTienPOS) as ThuTienPOS,
    		sum(ThuChuyenKhoan) as ThuChuyenKhoan,
    		sum(TongThu) as TongThu	
    	from
    	(
    	select 		
    		format(qhd.NgayLapHoaDon,'yyyyMMdd') as NgayLapYYYYMMDD,	
    		iif(qct.HinhThucThanhToan != 4,qct.TienThu,0) as TongThu,
    		iif(qct.HinhThucThanhToan = 1, qct.TienThu,0) as ThuTienMat,
    		iif(qct.HinhThucThanhToan = 2, qct.TienThu,0) as ThuTienPOS,
    		iif(qct.HinhThucThanhToan = 3, qct.TienThu,0) as ThuChuyenKhoan
    	
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon	
    	left join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
    	where qhd.NgayLapHoaDon between @DateFrom and @DateTo
    	and qhd.LoaiHoaDon = 11
    	and (qhd.TrangThai is null or qhd.TrangThai='1')
    	and (qhd.HachToanKinhDoanh = '1' or qhd.HachToanKinhDoanh is null)
    	and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi = cn.ID))
    	and (@KhoanMucThuChis ='' or exists (select km.ID from @tblKhoanThuChi km where qct.ID_KhoanThuChi= km.ID))	
    	and qct.ID_HoaDonLienQuan is not null --- chi lay phieuthu/chi lien quan hoadon
    	) tbl		
    	group by tbl.NgayLapYYYYMMDD
    	),
    	count_cte
    	as(
    		select 
    			sum(ThuTienMat) as TongThuTienMat,
    			sum(ThuTienPOS) as TongThuTienPOS,
    			sum(ThuChuyenKhoan) as TongThuChuyenKhoan,
    			sum(TongThu) as TongThuAll
    					
    		from data_cte dt
    	)
    	select *
    	from data_cte dt
    	cross join count_cte
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
    		--ISNULL(tblThe.NhanVienThucHien,'') as NhanVienThucHien,
    		tblThe.MaDoiTuong as MaKhachHang,
    		tblThe.TenDoiTuong as TenKhachHang,
    		tblThe.DienThoai as SoDienThoai,
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
							qct.TienThu * qct.ChiPhiNganHang/1000),0) as TongPhiNganHang ---- apply pos					
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

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetHoaDonAndSoQuy_FromIDDoiTuong]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;

declare @tblChiNhanh table (ID uniqueidentifier)
insert into @tblChiNhanh
select name from dbo.splitstring(@ID_DonVi)


	DECLARE @tblHoaDon TABLE(ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, LoaiHoaDon INT, GiaTri FLOAT);
	DECLARE @LoaiDoiTuong INT;
	SELECT @LoaiDoiTuong = LoaiDoiTuong FROM DM_DoiTuong WHERE ID = @ID_DoiTuong;
	IF(@LoaiDoiTuong = 3)
	BEGIN
		INSERT INTO @tblHoaDon
    	select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.LoaiHoaDon,
			hd.PhaiThanhToanBaoHiem as GiaTri
    	from BH_HoaDon hd
    	where hd.ID_BaoHiem like @ID_DoiTuong and hd.ID_DonVi in (select ID from @tblChiNhanh)
    	and hd.LoaiHoaDon not in (3,23) -- dieu chinh the: khong lien quan cong no
		and hd.ChoThanhToan ='0'
	END
	ELSE
	BEGIN
		INSERT INTO @tblHoaDon
		select *
		from
		(
		select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.LoaiHoaDon,
			case hd.LoaiHoaDon
				when 4 then ISNULL(hd.TongThanhToan,0)
				when 6 then - ISNULL(hd.TongThanhToan,0)
				when 7 then - ISNULL(hd.TongThanhToan,0)
				when 42 then - ISNULL(hd.TongThanhToan,0) --- tattoan congno thegiatri
				when 32 then - ISNULL(hd.TongThanhToan,0) ---- hoantracoc
				when 14 then - ISNULL(hd.TongThanhToan,0) ---- nhaphang khachthua
			else
    			ISNULL(hd.PhaiThanhToan,0)
    		end as GiaTri
    	from BH_HoaDon hd
		join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
    	where hd.ID_DoiTuong like @ID_DoiTuong 
    	and hd.LoaiHoaDon not in (3,23,31,35,37,38,39,40) 
		and hd.ChoThanhToan ='0'

		union all
		---- chiphi dichvu
		select 
			cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon, 125 as LoaiHoaDon,
			sum(cp.ThanhTien) as GiaTri			
		from BH_HoaDon_ChiPhi cp
		join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
		join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
		where hd.ChoThanhToan= 0
		and cp.ID_NhaCungCap= @ID_DoiTuong
		group by cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon,hd.LoaiHoaDon
		)a
	END

	---select * from @tblHoaDon
		
		SELECT *, 0 as LoaiThanhToan
		FROM @tblHoaDon
    	union
    	-- get list Quy_HD (không lấy Quy_HoaDon thu từ datcoc)
		select * from
		(
    		select qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon ,
			case when dt.LoaiDoiTuong = 1 OR dt.LoaiDoiTuong = 3 then
				case when qhd.LoaiHoaDon= 11 then -sum(qct.TienThu) else iif (max(LoaiThanhToan)=4, -sum(qct.TienThu),sum(qct.TienThu)) end
			else 
    			case when qhd.LoaiHoaDon = 11 then sum(qct.TienThu) else -sum(qct.TienThu) end
    		end as GiaTri,
			iif(qhd.PhieuDieuChinhCongNo='1',2, max(qct.LoaiThanhToan)) as LoaiThanhToan --- 1.coc, 2.dieuchinhcongno, 3.khong butru congno			
    		from Quy_HoaDon qhd	
    		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    		join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID			
    		where qct.ID_DoiTuong like @ID_DoiTuong 
			and exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)
			and not exists (select id from BH_HoaDon pthh where qct.ID_HoaDonLienQuan = pthh.ID and pthh.LoaiHoaDon= 41) --- khong lay phieuchi hoahong nguoi GT
			and qct.HinhThucThanhToan !=6			
			and (qct.LoaiThanhToan is null or qct.LoaiThanhToan !=3) ---- LoaiThnahToan = 3. thu/chi khong lienquan congno
			and (qhd.TrangThai is null or qhd.TrangThai='1') -- van phai lay phieu thu tu the --> trừ cong no KH
			group by qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon,dt.LoaiDoiTuong,qhd.PhieuDieuChinhCongNo
		) a where a.GiaTri != 0 -- khong lay phieudieuchinh diem

	
END");
        }
        
        public override void Down()
        {
        }
    }
}
