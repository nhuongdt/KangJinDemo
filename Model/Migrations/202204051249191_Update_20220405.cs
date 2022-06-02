namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_20220405 : DbMigration
    {
        public override void Up()
        {
			CreateStoredProcedure(name: "[dbo].[BaoCaoHoatDongXe_ChiTiet]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.String(),
				ToDate = p.String(),
				IDNhomHangs = p.String(),
				IDNhanViens = p.String(),
				IDNhomPhuTungs = p.String(),
				SoGioFrom = p.Int(),
				SoGioTo = p.Int(),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	 
	declare @sql1 nvarchar(max),  @sql2 nvarchar(max), @sql nvarchar(max),
	@where1 nvarchar(max), @where2 nvarchar(max),@where3 nvarchar(max),
	@param nvarchar(max), @tbl nvarchar(max)

	set @where1 =' where 1 = 1 and hh.ID_Xe is not null '
	set @where2 =' where 1 = 1 and hh.ID_Xe is not null '
	set @where3 =' where 1 = 1 '

	if ISNULL(@CurrentPage,'')='' set @CurrentPage = 0
	if ISNULL(@PageSize,'')='' set @PageSize = 100

	if isnull(@IDChiNhanhs,'')!=''
		begin
			set @tbl= N' declare @tblChiNhanh table (ID uniqueidentifier)
						insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ' 
			set @where1 = CONCAT(@where1, N' and exists (select cn.ID from @tblChiNhanh cn where tn.ID_DonVi = cn.ID)')
		end
	if isnull(@IDNhanViens,'')!=''
		begin
			set @tbl= CONCAT(@tbl, N' declare @tblNhanVien table (ID uniqueidentifier)
						insert into @tblNhanVien select name from dbo.splitstring(@IDNhanViens_In) ' )
			set @where1 = CONCAT(@where1, N' and exists (select nv.ID from @tblNhanVien nv where tn.ID_CoVanDichVu = nv.ID)')
		end
	if isnull(@IDNhomHangs,'')!=''
		begin
			set @tbl= CONCAT(@tbl, N' declare @tblNhomHang table (ID uniqueidentifier)
						insert into @tblNhomHang select name from dbo.splitstring(@IDNhomHangs_In) ' )
			set @where2 = CONCAT(@where2, N' and exists (select nhom1.ID from @tblNhomHang nhom1 where nhomhh.ID = nhom1.ID)')
		end	

	if isnull(@IDNhomPhuTungs,'')!=''
		begin
			set @tbl= CONCAT(@tbl, N' declare @tblNhomPhuTung table (ID uniqueidentifier)
						insert into @tblNhomPhuTung select name from dbo.splitstring(@IDNhomPhuTungs_In) ' )
			set @where2 = CONCAT(@where2, N' and exists (select nhom2.ID from @tblNhomPhuTung nhom2 where nhompt.ID = nhom2.ID)')
		end	


	if isnull(@TextSearch,'')!=''
		begin
			set @tbl= CONCAT(@tbl,N' DECLARE @tblSearch TABLE (Name [nvarchar](max))
					DECLARE @count int;
					INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!=''''
					Select @count =  (Select count(*) from @tblSearch) ' )
			set @where3 = CONCAT(@where3, N' AND ((select count(Name) from @tblSearch b where 
					tbl.BienSo like ''%''+b.Name+''%''
    				or tbl.TenHangHoa like ''%'' + b.Name +''%''
    				or tbl.MaHangHoa like ''%'' + b.Name +''%''    			
    				or tbl.TenHangHoa_KhongDau like ''%'' + b.Name +''%''	
					or tbl.TenPhuTung like ''%'' + b.Name +''%''
    				or tbl.MaPhuTung like ''%'' + b.Name +''%''    			
    				or tbl.TenPhuTung_KhongDau like ''%'' + b.Name +''%''	
					or tbl.MaPhieuTiepNhan like ''%'' + b.Name +''%''		
					)=@count or @count=0)')
		end
	
	if isnull(@FromDate,'')!=''
			set @where1 = CONCAT(@where1, N' and tn.NgayVaoXuong > @FromDate_In')	
	if isnull(@ToDate,'')!=''
			set @where1 = CONCAT(@where1, N' and tn.NgayVaoXuong < @ToDate_In')	
			
	if isnull(@SoGioFrom,'')!=''
			begin
				set @SoGioFrom = @SoGioFrom - 1
				set @where1= CONCAT(@where1, ' and tn.SoKmVao > @SoGioFrom_In')
			end
	if isnull(@SoGioTo,'')!=''
			begin
				set @SoGioTo = @SoGioTo + 1
				set @where1= CONCAT(@where1, ' and tn.SoKmVao < @SoGioTo_In')
			end
	
	set @sql1 = concat( N'		
			select tn.ID, tn.ID_DonVi, tn.ID_Xe, tn.ID_CoVanDichVu, tn.SoKmVao, tn.MaPhieuTiepNhan, tn.NgayVaoXuong, tn.GhiChu
			into #tblHoatDong
			from Gara_PhieuTiepNhan tn	
			left join DM_HangHoa hh on tn.ID_Xe= hh.ID_Xe ', @where1		
			)

	set @sql2 = concat( N'			
	---- get tpdinhluong by idhanghoa
	select hh.ID_Xe, 
		hh.ID as ID_HangHoa, 
		qd.MaHangHoa,
		hh.TenHangHoa,
		hh.TenHangHoa_KhongDau,
		tpdl.MaHangHoa as MaPhuTung, 
		hhdl.TenHangHoa as TenPhuTung,
		hhdl.TenHangHoa_KhongDau as TenPhuTung_KhongDau, 		
		tpdl.TenDonViTinh, 
		hhdl.LoaiBaoHanh, hhdl.ThoiGianBaoHanh,
		case hhdl.LoaiBaoHanh
			when 1 then hhdl.ThoiGianBaoHanh * 24   ---- ngay
			when 2 then hhdl.ThoiGianBaoHanh * 24 * 30   ---- thang
			when 3 then hhdl.ThoiGianBaoHanh * 24 * 365   ---- nam
			when 4 then hhdl.ThoiGianBaoHanh    ---- gio
		end as MocBaoHanh,
		nhomhh.TenNhomHangHoa,
		nhompt.TenNhomHangHoa as TenNhomPhuTung
	into #tblPhuTung
	from DM_HangHoa hh
	join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa
	left join DM_NhomHangHoa nhomhh on hh.ID_NhomHang = nhomhh.ID
	left join DinhLuongDichVu dl on qd.ID = dl.ID_DichVu
	left join DonViQuiDoi tpdl on dl.ID_DonViQuiDoi = tpdl.ID
	left join DM_HangHoa hhdl on tpdl.ID_HangHoa = hhdl.ID 
	left join DM_NhomHangHoa nhompt on hhdl.ID_NhomHang = nhompt.ID 
	', @where2)

	set @sql = CONCAT(@tbl,@sql1, @sql2, 
	N'
	select *,
		iif(tbl.BHConLai <0,N''Bảo hành'','''') as BHTrangThai,
		row_number() over (order by tbl.NgayVaoXuong desc) as RN
	into #tblView
	from
		(
		select dv.TenDonVi, xe.BienSo, hd.SoKmVao as SoGioHoatDong, 
			hd.MaPhieuTiepNhan, hd.NgayVaoXuong, hd.GhiChu,
			hd.ID_Xe, hd.ID_DonVi,
			hd.ID as ID_PhieuTiepNhan,
			pt.ID_HangHoa,
			pt.MaPhuTung, pt.TenPhuTung, pt.TenPhuTung_KhongDau,
			pt.MaHangHoa, pt.TenHangHoa, pt.TenHangHoa_KhongDau, pt.TenDonViTinh,
			pt.TenNhomHangHoa,
			pt.TenNhomPhuTung,
			pt.LoaiBaoHanh,
			pt.ThoiGianBaoHanh,
			pt.MocBaoHanh,
			pt.MocBaoHanh - hd.SoKmVao as BHConLai,
			nv.TenNhanVien
		from #tblHoatDong hd
		left join #tblPhuTung pt on hd.ID_Xe= pt.ID_Xe
		join DM_DonVi dv on hd.ID_DonVi = dv.ID
		join Gara_DanhMucXe xe on pt.ID_Xe= xe.ID 
		left join NS_NhanVien nv on hd.ID_CoVanDichVu= nv.ID 
	) tbl ', @where3 ,' order by tbl.NgayVaoXuong')

	set @sql = CONCAT(@sql, N' declare @TongSoGioHoatDong float, @TotalRow int 
								
								select @TongSoGioHoatDong = sum(a.SoGioHoatDong) , @TotalRow = max(TotalRow)
									from (
										select max(SoGioHoatDong) as SoGioHoatDong, max(RN) as TotalRow from #tblView group by ID_PhieuTiepNhan
										) a ',
								N' select * , 
									@TongSoGioHoatDong as TongSoGioHoatDong,
									@TotalRow as TotalRow
								from #tblView where Rn between (@CurrentPage_In * @PageSize_In) + 1 and @PageSize_In * (@CurrentPage_In + 1)')

	set @param = N'@IDChiNhanhs_In nvarchar(max) = null,
				@FromDate_In nvarchar(max) = null,
				@ToDate_In nvarchar(max) = null,
				@IDNhomHangs_In nvarchar(max) = null,
				@IDNhanViens_In nvarchar(max) = null,
				@IDNhomPhuTungs_In nvarchar(max) = null,
				@SoGioFrom_In int = null,
				@SoGioTo_In int = null,
				@TextSearch_In nvarchar(max) = null,
				@CurrentPage_In int = null,
				@PageSize_In int = null'

				print @sql
	
	exec sp_executesql @sql, @param,
					@IDChiNhanhs_In  = @IDChiNhanhs,
					@FromDate_In  = @FromDate,
					@ToDate_In  = @ToDate,
					@IDNhomHangs_In  = @IDNhomHangs,
					@IDNhanViens_In  = @IDNhanViens,
					@IDNhomPhuTungs_In  = @IDNhomPhuTungs,
					@SoGioFrom_In = @SoGioFrom,
					@SoGioTo_In = @SoGioTo,
					@TextSearch_In  = @TextSearch,
					@CurrentPage_In  = @CurrentPage,
					@PageSize_In  = @PageSize");

			CreateStoredProcedure(name: "[dbo].[BaoCaoHoatDongXe_TongHop]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				ToDate = p.String(),
				IDNhomHangs = p.String(),
				IDNhanViens = p.String(),
				IDNhomPhuTungs = p.String(),
				TrangThai = p.Int(),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	 
	declare @sql1 nvarchar(max),  @sql2 nvarchar(max), @sql nvarchar(max),
	@where1 nvarchar(max), @where2 nvarchar(max),@where3 nvarchar(max),
	@param nvarchar(max), @tbl nvarchar(max)

	set @where1 =' where 1 = 1 and hh.ID_Xe is not null '
	set @where2 =' where 1 = 1 and hh.ID_Xe is not null '
	set @where3 =' where 1 = 1 '

	if ISNULL(@CurrentPage,'')='' set @CurrentPage = 0
	if ISNULL(@PageSize,'')='' set @PageSize = 100

	if isnull(@IDChiNhanhs,'')!=''
		begin
			set @tbl= N' declare @tblChiNhanh table (ID uniqueidentifier)
						insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ' 
			set @where1 = CONCAT(@where1, N' and exists (select cn.ID from @tblChiNhanh cn where tn.ID_DonVi = cn.ID)')
		end
	if isnull(@IDNhanViens,'')!=''
		begin
			set @tbl= CONCAT(@tbl, N' declare @tblNhanVien table (ID uniqueidentifier)
						insert into @tblNhanVien select name from dbo.splitstring(@IDNhanViens_In) ' )
			set @where1 = CONCAT(@where1, N' and exists (select nv.ID from @tblNhanVien nv where tn.ID_NhanVien = nv.ID)')
		end
	if isnull(@IDNhomHangs,'')!=''
		begin
			set @tbl= CONCAT(@tbl, N' declare @tblNhomHang table (ID uniqueidentifier)
						insert into @tblNhomHang select name from dbo.splitstring(@IDNhomHangs_In) ' )
			set @where2 = CONCAT(@where2, N' and exists (select nhom1.ID from @tblNhomHang nhom1 where nhomhh.ID = nhom1.ID)')
		end	

	if isnull(@IDNhomPhuTungs,'')!=''
		begin
			set @tbl= CONCAT(@tbl, N' declare @tblNhomPhuTung table (ID uniqueidentifier)
						insert into @tblNhomPhuTung select name from dbo.splitstring(@IDNhomPhuTungs_In) ' )
			set @where2 = CONCAT(@where2, N' and exists (select nhom2.ID from @tblNhomPhuTung nhom2 where nhompt.ID = nhom2.ID)')
		end	

	if isnull(@TextSearch,'')!=''
		begin
			set @tbl= CONCAT(@tbl,N' DECLARE @tblSearch TABLE (Name [nvarchar](max))
					DECLARE @count int;
					INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!=''''
					Select @count =  (Select count(*) from @tblSearch) ' )
			set @where3 = CONCAT(@where3, N' AND ((select count(Name) from @tblSearch b where 
					tbl.BienSo like ''%''+b.Name+''%''
    				or tbl.TenHangHoa like ''%'' + b.Name +''%''
    				or tbl.MaHangHoa like ''%'' + b.Name +''%''    			
    				or tbl.TenHangHoa_KhongDau like ''%'' + b.Name +''%''	
					or tbl.TenPhuTung like ''%'' + b.Name +''%''
    				or tbl.MaPhuTung like ''%'' + b.Name +''%''    			
    				or tbl.TenPhuTung_KhongDau like ''%'' + b.Name +''%''	
					)=@count or @count=0)')
		end
	if isnull(@ToDate,'')!=''
		begin
			set @ToDate= DATEADD(day,1,@ToDate)
			set @where1 = CONCAT(@where1, N' and tn.NgayVaoXuong < @ToDate_In')	
		end
			
	if isnull(@TrangThai,0)!=0 ---- 0.all, 1.đến hạn bảo hành, 2.ngược lại 1
			begin
				if isnull(@TrangThai,0) = 1
					set @where3 = CONCAT(@where3, N' and tbl.BHConLai < 1')	
				if isnull(@TrangThai,0) = 2
					set @where3 = CONCAT(@where3, N' and tbl.BHConLai > 0')	
			end
	
	set @sql1 = concat( N'
			---- sum sogiothuchien by car
			select tn.ID_DonVi, tn.ID_Xe, sum( isnull(tn.SoKmVao,0)) as SoGioHoatDong
			into #tblHoatDong
			from Gara_PhieuTiepNhan tn	
			left join DM_HangHoa hh on tn.ID_Xe= hh.ID_Xe ', @where1,		
			' group by tn.ID_Xe, tn.ID_DonVi')

	set @sql2 = concat( N'			
	---- get tpdinhluong by idhanghoa
	select hh.ID_Xe,
		hh.ID as ID_HangHoa, 
		qd.MaHangHoa,
		hh.TenHangHoa,
		hh.TenHangHoa_KhongDau,
		tpdl.MaHangHoa as MaPhuTung, 
		hhdl.TenHangHoa as TenPhuTung,
		hhdl.TenHangHoa_KhongDau as TenPhuTung_KhongDau, 		
		tpdl.TenDonViTinh, 
		hhdl.LoaiBaoHanh, hhdl.ThoiGianBaoHanh,
		case hhdl.LoaiBaoHanh
			when 1 then hhdl.ThoiGianBaoHanh * 24   ---- ngay
			when 2 then hhdl.ThoiGianBaoHanh * 24 * 30   ---- thang
			when 3 then hhdl.ThoiGianBaoHanh * 24 * 365   ---- nam
			when 4 then hhdl.ThoiGianBaoHanh    ---- gio
		end as MocBaoHanh,
		nhomhh.TenNhomHangHoa,
		nhompt.TenNhomHangHoa as TenNhomPhuTung
	into #tblPhuTung
	from DM_HangHoa hh
	join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa
	left join DM_NhomHangHoa nhomhh on hh.ID_NhomHang = nhomhh.ID
	left join DinhLuongDichVu dl on qd.ID = dl.ID_DichVu
	left join DonViQuiDoi tpdl on dl.ID_DonViQuiDoi = tpdl.ID
	left join DM_HangHoa hhdl on tpdl.ID_HangHoa = hhdl.ID 
	left join DM_NhomHangHoa nhompt on hhdl.ID_NhomHang = nhompt.ID 
	',@where2)

	set @sql = CONCAT(@tbl,@sql1, @sql2, 
	N'
	select *,
		iif(tbl.BHConLai <0,N''Bảo hành'','''') as BHTrangThai,
		row_number() over (order by tbl.SoGioHoatDong desc) as RN
	into #tblView
	from
		(
		select dv.TenDonVi, xe.BienSo, hd.SoGioHoatDong, 
			hd.ID_Xe, hd.ID_DonVi,
			pt.ID_HangHoa,
			pt.MaPhuTung, pt.TenPhuTung, pt.TenPhuTung_KhongDau,
			pt.MaHangHoa, pt.TenHangHoa, pt.TenHangHoa_KhongDau, pt.TenDonViTinh,
			pt.TenNhomHangHoa,
			pt.TenNhomPhuTung,
			pt.LoaiBaoHanh,
			pt.ThoiGianBaoHanh,
			isnull(pt.MocBaoHanh,0) as MocBaoHanh,
			pt.MocBaoHanh - hd.SoGioHoatDong as BHConLai
		from #tblHoatDong hd
		left join #tblPhuTung pt on hd.ID_Xe= pt.ID_Xe
		join DM_DonVi dv on hd.ID_DonVi = dv.ID
		join Gara_DanhMucXe xe on pt.ID_Xe= xe.ID 
	) tbl ', @where3)

	set @sql = CONCAT(@sql, N' declare @TongSoGioHoatDong float, @TotalRow int 
								
								select @TongSoGioHoatDong = sum(a.SoGioHoatDong) , @TotalRow = max(TotalRow)
									from (
										select max(SoGioHoatDong) as SoGioHoatDong, max(RN) as TotalRow from #tblView group by ID_Xe
										) a ',
								N' select * , 
									@TongSoGioHoatDong as TongSoGioHoatDong,
									@TotalRow as TotalRow
								from #tblView where Rn between (@CurrentPage_In * @PageSize_In) + 1 and @PageSize_In * (@CurrentPage_In + 1)')

	set @param = N'@IDChiNhanhs_In nvarchar(max) = null,
				@ToDate_In nvarchar(max) = null,
				@IDNhomHangs_In nvarchar(max) = null,
				@IDNhanViens_In nvarchar(max) = null,
				@IDNhomPhuTungs_In nvarchar(max) = null,
				@TrangThai_In int = null,
				@TextSearch_In nvarchar(max) = null,
				@CurrentPage_In int = null,
				@PageSize_In int = null'

				print @sql
	
	exec sp_executesql @sql, @param,
					@IDChiNhanhs_In  = @IDChiNhanhs,
					@ToDate_In  = @ToDate,
					@IDNhomHangs_In  = @IDNhomHangs,
					@IDNhanViens_In  = @IDNhanViens,
					@IDNhomPhuTungs_In  = @IDNhomPhuTungs,
					@TrangThai_In = @TrangThai,
					@TextSearch_In  = @TextSearch,
					@CurrentPage_In  = @CurrentPage,
					@PageSize_In  = @PageSize");

			CreateStoredProcedure(name: "[dbo].[GetInforHoaDon_ByID]", parametersAction: p => new
			{
				ID_HoaDon = p.String()
			}, body: @"SET NOCOUNT ON;

declare @IDHDGoc uniqueidentifier, @ID_DoiTuong uniqueidentifier, @ID_BaoHiem uniqueidentifier
select @IDHDGoc = ID_HoaDon,  @ID_DoiTuong = ID_DoiTuong, @ID_BaoHiem = ID_BaoHiem from BH_HoaDon where ID = @ID_HoaDon

    select 
    		hd.ID,
			hd.LoaiHoaDon,
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,
			hd.ID_PhieuTiepNhan, 
    		hd.TongTienHang,
			hd.ChoThanhToan,
    		ISNULL(hd.TongGiamGia,0) + ISNULL(hd.KhuyeMai_GiamGia, 0) as TongGiamGia,
    		CAST(ISNULL(hd.PhaiThanhToan,0) as float)  as PhaiThanhToan,
    		CAST(ISNULL(KhachDaTra,0) as float) as KhachDaTra,	
			isnull(soquy.BaoHiemDaTra,0) as BaoHiemDaTra,

			CAST(ISNULL(TienDoiDiem,0) as float) as TienDoiDiem,	
			CAST(ISNULL(ThuTuThe,0) as float) as ThuTuThe,	
			isnull(soquy.TienMat,0) as TienMat,
			isnull(soquy.TienATM,0) as TienATM,
			isnull(soquy.ChuyenKhoan,0) as ChuyenKhoan,

			dt.MaDoiTuong,
			bh.TenDoiTuong as TenBaoHiem,
			bh.MaDoiTuong as MaBaoHiem,

    		ISNULL(dt.TenDoiTuong,N'Khách lẻ')  as TenDoiTuong,
			ISNULL(bg.TenGiaBan,N'Bảng giá chung') as TenBangGia,
			ISNULL(nv.TenNhanVien,N'')  as TenNhanVien,
			ISNULL(dv.TenDonVi,N'')  as TenDonVi,    
    		case when hd.NgayApDungGoiDV is null then '' else  convert(varchar(14), hd.NgayApDungGoiDV ,103) end  as NgayApDungGoiDV,
    		case when hd.HanSuDungGoiDV is null then '' else  convert(varchar(14), hd.HanSuDungGoiDV ,103) end as HanSuDungGoiDV,
    		hd.NguoiTao as NguoiTaoHD,
    		hd.DienGiai,
    		hd.ID_DonVi,
			hd.TongTienThue,
			isnull(hd.TongTienBHDuyet,0) as TongTienBHDuyet, 
			isnull(hd.PTThueHoaDon,0) as PTThueHoaDon, 
			isnull(hd.PTThueBaoHiem,0) as PTThueBaoHiem, 
			isnull(hd.TongTienThueBaoHiem,0) as TongTienThueBaoHiem, 
			isnull(hd.KhauTruTheoVu,0) as KhauTruTheoVu, 

			isnull(hd.PTGiamTruBoiThuong,0) as PTGiamTruBoiThuong, 
			isnull(hd.GiamTruBoiThuong,0) as GiamTruBoiThuong, 
			isnull(hd.BHThanhToanTruocThue,0) as BHThanhToanTruocThue, 
			isnull(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem, 
    		-- get avoid error at variable at class BH_HoaDonDTO
    		NEWID() as ID_DonViQuiDoi,
			case when hd.ChoThanhToan is null then N'Đã hủy'
			else 
				case hd.LoaiHoaDon
					when 3 then 
						case when hd.YeuCau ='1' then case when hd.ID_PhieuTiepNhan is null then  N'Phiếu tạm' else
						case when hd.ChoThanhToan='0' then N'Đã duyệt' else N'Chờ duyệt' end end
    						else 
    							case when hd.YeuCau ='2' then  N'Đang xử lý'
    							else 
    								case when hd.YeuCau ='3' then N'Hoàn thành'
    								else  N'Đã hủy' end
    							end end
						else N'Hoàn thành'
						end end as TrangThai		   														
    	from BH_HoaDon hd
    	left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    	left join DM_DonVi dv on hd.ID_DonVi= dv.ID
    	left join DM_GiaBan bg on hd.ID_BangGia= bg.ID
		left join DM_DoiTuong bh on hd.ID_BaoHiem= bh.ID
    	left join 
    		(
				select 
					tblThuChi.ID,
					sum(TienMat) as TienMat,
					sum(TienATM) as TienATM,
					sum(ChuyenKhoan) as ChuyenKhoan,
					sum(TienDoiDiem) as TienDoiDiem,
					sum(ThuTuThe) as ThuTuThe,				
					sum(KhachDaTra) as KhachDaTra,
					sum(BaoHiemDaTra) as BaoHiemDaTra
				from
					(
					-- get TongThu from HDDatHang: chi get hdXuly first
    					select 
							ID,
							TienMat, TienATM,ChuyenKhoan,
							TienDoiDiem, ThuTuThe, 				
							TienThu as KhachDaTra,	
							0 as BaoHiemDaTra,
							TienDatCoc
						from
						(	
								Select 
										ROW_NUMBER() OVER(PARTITION BY ID_HoaDonLienQuan ORDER BY NgayLapHoaDon ASC) AS isFirst,						
    									d.ID,
										ID_HoaDonLienQuan,
										d.NgayLapHoaDon,
    									sum(d.TienMat) as TienMat,
    									SUM(ISNULL(d.TienATM, 0)) as TienATM,
    									SUM(ISNULL(d.TienCK, 0)) as ChuyenKhoan,
										SUM(ISNULL(d.TienDoiDiem, 0)) as TienDoiDiem,
										sum(d.ThuTuThe) as ThuTuThe,
    									sum(d.TienThu) as TienThu,
										sum(d.TienDatCoc) as TienDatCoc
									
    								FROM
    								(
									
											select hd.ID, hd.NgayLapHoaDon,
												qct.ID_HoaDonLienQuan,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=1, qct.TienThu, 0), iif(qct.HinhThucThanhToan=1, -qct.TienThu, 0)) as TienMat,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=2, qct.TienThu, 0), iif(qct.HinhThucThanhToan=2, -qct.TienThu, 0)) as TienATM,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=3, qct.TienThu, 0), iif(qct.HinhThucThanhToan=3, -qct.TienThu, 0)) as TienCK,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=5, qct.TienThu, 0), iif(qct.HinhThucThanhToan=5, -qct.TienThu, 0)) as TienDoiDiem,
												iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=4, qct.TienThu, 0), iif(qct.HinhThucThanhToan=4, -qct.TienThu, 0)) as ThuTuThe,
												iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu) as TienThu,
												iif(qct.HinhThucThanhToan=6,qct.TienThu,0) as TienDatCoc
											from Quy_HoaDon_ChiTiet qct
											join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
											join BH_HoaDon hdd on hdd.ID= qct.ID_HoaDonLienQuan
											join BH_HoaDon hd on hd.ID_HoaDon= hdd.ID
											where hdd.LoaiHoaDon = '3' 	
											and hd.ChoThanhToan = 0
											and (qhd.TrangThai= 1 Or qhd.TrangThai is null)
											and hdd.ID= @IDHDGoc											
    								) d group by d.ID,d.NgayLapHoaDon,ID_HoaDonLienQuan						
						) thuDH
						where isFirst= 1

						union all
					---- khach datra
					select qct.ID_HoaDonLienQuan,		
						sum(iif(qct.HinhThucThanhToan= 1,iif(qhd.LoaiHoaDon=12 ,- qct.TienThu, qct.TienThu),0))  as TienMat,
						sum(iif(qct.HinhThucThanhToan= 2,iif(qhd.LoaiHoaDon=12 ,- qct.TienThu, qct.TienThu),0))  as TienATM,
						sum(iif(qct.HinhThucThanhToan= 3,iif(qhd.LoaiHoaDon=12 ,- qct.TienThu, qct.TienThu),0))  as ChuyenKhoan,
						sum(iif(qct.HinhThucThanhToan= 5,iif(qhd.LoaiHoaDon=12 ,- qct.TienThu, qct.TienThu),0))  as TienDoiDiem,
						sum(iif(qct.HinhThucThanhToan= 4,iif(qhd.LoaiHoaDon=12 ,- qct.TienThu, qct.TienThu),0))  as ThuTuThe,					
						sum(iif(qhd.LoaiHoaDon=12 ,- qct.TienThu, qct.TienThu)) as KhachDaTra,
						0 as BaoHiemDaTra,
						0 as TienDatCoc
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
					where qhd.TrangThai= 1
					and qct.ID_DoiTuong= @ID_DoiTuong					
					group by qct.ID_HoaDonLienQuan


					union all
					----- baohiem datra
					select qct.ID_HoaDonLienQuan, 
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,
									
						0 as KhachDaTra, 
						sum(qct.TienThu) as BaoHiemDaTra,
						0 as TienDatCoc
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
					where qhd.TrangThai= 1 
					and qct.ID_DoiTuong= @ID_BaoHiem					
					group by qct.ID_HoaDonLienQuan
				)tblThuChi group by tblThuChi.ID
			) soquy on hd.ID = soquy.ID		
    	where hd.ID like @ID_HoaDon");

			CreateStoredProcedure(name: "[dbo].[HuyTienCoc_CheckVuotHanMuc]", parametersAction: p => new
			{
				ID_PhieuThuChi = p.Guid()
			}, body: @"SET NOCOUNT ON;
	declare @ngaylapPhieu datetime , @ID_DoiTuong uniqueidentifier, @ID_DonVi uniqueidentifier
	select top 1 
			@ngaylapPhieu = NgayLapHoaDon , 
			@ID_DoiTuong = ID_DoiTuong
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
	where qhd.ID= @ID_PhieuThuChi


	declare @tongnap float , @sudung float
	---- get tongtien napcoc den @ThoiGian
	---- ncc: tongnap (-)
	select @tongnap = ABS(sum(iif(qhd.LoaiHoaDon =11, qct.TienThu, -qct.TienThu))) 
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
	where qhd.TrangThai = '1'
	and qct.LoaiThanhToan =  1
	and qhd.NgayLapHoaDon < @ngaylapPhieu
	and qct.ID_DoiTuong= @ID_DoiTuong
	
	---- sudung tiencoc
	select @sudung = ABS(sum(iif(qhd.LoaiHoaDon =11, -qct.TienThu, qct.TienThu))) 
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
	where qhd.TrangThai = '1'
	and qct.HinhThucThanhToan= 6
	and qct.ID_DoiTuong= @ID_DoiTuong

	---- 0.chưa vượt hạn mức
	select CAST( iif( isnull(@tongnap,0) - isnull(@sudung,0) > - 1,'0','1') as bit) as Exist");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_ChiTiet_Page]
    @pageNumber [int],
    @pageSize [int],
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
	@LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
	@LoaiChungTu [nvarchar](max),
	@HanBaoHanh [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	set nocount on;
	---- bo sung timkiem NVthuchien
	set @pageNumber = @pageNumber -1;

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER)
	INSERT INTO @tblChiNhanh
	select Name from splitstring(@ID_ChiNhanh);

	DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)	

	declare @tblCTHD table (
		NgayLapHoaDon datetime,
		MaHoaDon nvarchar(max),
		LoaiHoaDon int,
		ID_DonVi uniqueidentifier,
		ID_PhieuTiepNhan uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID_NhanVien uniqueidentifier,
		TongTienHang float,
		TongGiamGia	float,
		KhuyeMai_GiamGia float,
		ChoThanhToan bit,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		ID_ChiTietGoiDV	uniqueidentifier,
		ID_ChiTietDinhLuong uniqueidentifier,
		ID_ParentCombo uniqueidentifier,
		SoLuong float,
		DonGia float,
		GiaVonfloat float,
		TienChietKhau float,
		TienChiPhi float,
		ThanhTien float,
		ThanhToan float,
		GhiChu nvarchar(max),
		ChatLieu nvarchar(max),
		LoaiThoiGianBH int,
		ThoiGianBaoHanh float,
		TenHangHoaThayThe nvarchar(max),
		TienThue float,	
		GiamGiaHD float,
		GiaVon float,
		TienVon float
		)

	insert into @tblCTHD
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu

	declare @tblChiPhi table (ID_ParentCombo uniqueidentifier,ID_DonViQuiDoi uniqueidentifier, ChiPhi float, 
		ID_NhanVien uniqueidentifier,ID_DoiTuong uniqueidentifier)
	insert into @tblChiPhi
	exec BCBanHang_GetChiPhi @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu
			
		select *
		into #tblView
		from
		(
		select 
			hh.ID, hh.TenHangHoa,
			qd.MaHangHoa,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa,
			concat(hh.TenHangHoa, qd.ThuocTinhGiaTri) as TenHangHoaFull,
			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			ISNULL(nhh.TenNhomHangHoa,  N'Nhóm hàng hóa mặc định') as TenNhomHangHoa,
			lo.MaLoHang as TenLoHang,
			qd.TenDonViTinh,
			cast(c.SoLuong as float) as SoLuong,
			cast(c.DonGia as float) as GiaBan,
			cast(c.TienChietKhau as float) as TienChietKhau,
			cast(c.ThanhTien as float) as ThanhTien,
			cast(c.GiamGiaHD as float) as GiamGiaHD,
			cast(c.TienThue as float) as TienThue,
			iif(@XemGiaVon='1',cast(c.GiaVon as float),0) as GiaVon,
			iif(@XemGiaVon='1',cast(c.TienVon as float),0) as TienVon,
			cast(c.ThanhTien - c.GiamGiaHD as float) as DoanhThu,
			iif(@XemGiaVon='1',cast(c.ThanhTien - c.GiamGiaHD - c.TienVon -c.ChiPhi as float),0) as LaiLo,
			c.NgayLapHoaDon, c.MaHoaDon, c.ID_PhieuTiepNhan, c.ID_DoiTuong, c.ID_NhanVien,
			c.ThoiGianBaoHanh, c.HanBaoHanh, c.TrangThai, c.GhiChu,
			dt.MaDoiTuong as MaKhachHang, 
			dt.TenDoiTuong as TenKhachHang, 
			dt.TenNhomDoiTuongs as NhomKhachHang, 
			dt.DienThoai, dt.GioiTinhNam,
			dt.ID_NguoiGioiThieu, dt.ID_NguonKhach,
			c.TenNhanVien,
			c.ChiPhi,
			c.LoaiHoaDon,
			iif(c.TenHangHoaThayThe is null or c.TenHangHoaThayThe='', hh.TenHangHoa, c.TenHangHoaThayThe) as TenHangHoaThayThe			
		from 
		(
		select 
			b.LoaiHoaDon,b.NgayLapHoaDon, b.MaHoaDon, b.ID_PhieuTiepNhan, b.ID_DoiTuong, b.ID_NhanVien, b.TenNhanVien,
			b.ThoiGianBaoHanh, b.HanBaoHanh, b.TrangThai, b.GhiChu,
			b.SoLuong * isnull(qd.TyLeChuyenDoi,1) as SoLuong,
			b.ThanhTien,
			b.GiaVon,
			b.TienVon,		
			qd.ID_HangHoa,
			b.ID_LoHang,	
			b.GiamGiaHD,
			b.TienThue,					
			b.DonGia,
			b.TienChietKhau,
			b.ChiPhi,
			b.TenHangHoaThayThe
		from (
		select 
			ct.LoaiHoaDon,ct.NgayLapHoaDon, ct.MaHoaDon, ct.ID_PhieuTiepNhan, ct.ID_DoiTuong, ct.ID_NhanVien,	
			nvien.NVThucHien as TenNhanVien,
			ct.TienThue,
			ct.GiamGiaHD,			
			ct.ID,ct.ID_DonViQuiDoi, ct.ID_LoHang,
			ct.TenHangHoaThayThe,
			case ct.LoaiThoiGianBH
				when 1 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + N' ngày'
				when 2 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + ' tháng'
				when 3 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + ' năm'
			else '' end as ThoiGianBaoHanh,
			case ct.LoaiThoiGianBH
				when 1 then DATEADD(DAY, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
				when 2 then DATEADD(MONTH, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
				when 3 then DATEADD(YEAR, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
			end as HanBaoHanh,
			Case when ct.LoaiThoiGianBH = 1 and DATEADD(DAY, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH = 2 and DATEADD(MONTH, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH = 3 and DATEADD(YEAR, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH in (1,2,3) Then N'Còn hạn'
			else '' end as TrangThai,
			ct.GhiChu,
			ct.DonGia,
			ct.TienChietKhau,
			ct.SoLuong,
			ct.ThanhTien,
			iif(ct.SoLuong =0, 0, ct.TienVon/ct.SoLuong) as GiaVon,			
			ct.TienVon,
			isnull(cp.ChiPhi,0) as ChiPhi
	from @tblCTHD ct	
	left join @tblChiPhi cp on ct.ID= cp.ID_ParentCombo
	left join
		(
		-- get nvthuchien of hdbl
			select distinct th.ID_ChiTietHoaDon ,
				 (
						select nv.TenNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						join BH_HoaDon_ChiTiet ct on nvth.ID_ChiTietHoaDon = ct.ID
						join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
						where nvth.ID_ChiTietHoaDon = th.ID_ChiTietHoaDon
						and (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    					and hd.ChoThanhToan = 0 
						and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)						
						For XML PATH ('')
					) NVThucHien
				from BH_NhanVienThucHien th 
		) nvien on ct.ID = nvien.ID_ChiTietHoaDon
		where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
		and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)	
		)b
		join DonViQuiDoi qd on b.ID_DonViQuiDoi= qd.ID		
		) c
		join DM_HangHoa hh on c.ID_HangHoa = hh.ID
		join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan=1
		left join DM_LoHang lo on c.ID_LoHang = lo.ID
		left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
		left join DM_DoiTuong dt on c.ID_DoiTuong = dt.ID		
		where 
		exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhh.ID= allnhh.ID)	
    	and hh.TheoDoi like @TheoDoi
		and qd.Xoa like @TrangThai
		and c.TrangThai like @HanBaoHanh		
		AND
		((select count(Name) from @tblSearchString b where 
				c.MaHoaDon like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'
					or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau  like '%'+b.Name+'%'
					or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.DienThoai  like '%'+b.Name+'%'
					or c.GhiChu like N'%'+b.Name+'%'
					or c.TenNhanVien like N'%'+b.Name+'%'
					or dbo.FUNC_ConvertStringToUnsign(c.GhiChu) like N'%'+b.Name+'%'
    				or qd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
		)a where a.LoaiHangHoa in (select name from dbo.splitstring(@LoaiHangHoa))	
		
		
	
			DECLARE @Rows FLOAT,  @TongSoLuong float, @TongThanhTien float, @TongGiamGiaHD FLOAT, @TongTienVon FLOAT, 
			@TongLaiLo FLOAT, @SumTienThue FLOAT,@TongDoanhThuThuan FLOAT, @TongChiPhi float			
			SELECT @Rows = Count(*), @TongSoLuong = SUM(SoLuong),
			@TongThanhTien = SUM(ThanhTien), @TongGiamGiaHD = SUM(GiamGiaHD),
			@TongTienVon = SUM(TienVon), @TongLaiLo = SUM(LaiLo), @SumTienThue = SUM(TienThue),
			@TongDoanhThuThuan = SUM(DoanhThu),
			@TongChiPhi = SUM(ChiPhi) 
			FROM #tblView;

			select 
				tbl.*,
				ISNULL(nk.TenNguonKhach,'') as TenNguonKhach,
				isnull(gt.TenDoiTuong,'') as NguoiGioiThieu				
			from(
			select *,							
				@Rows as Rowns,
    			@TongSoLuong as TongSoLuong,
    			@TongThanhTien as TongThanhTien,
    			@TongGiamGiaHD as TongGiamGiaHD,
    			@TongTienVon as TongTienVon,
    			@TongLaiLo as TongLaiLo,
				@SumTienThue as TongTienThue,
    			@TongDoanhThuThuan as DoanhThuThuan,
    			@TongChiPhi as TongChiPhi
    		from #tblView tbl
			order by NgayLapHoaDon DESC
			OFFSET (@pageNumber* @pageSize) ROWS
    		FETCH NEXT @pageSize ROWS ONLY	) tbl
			left join DM_NguonKhachHang nk on tbl.ID_NguonKhach= nk.ID
			left join DM_DoiTuong gt on tbl.ID_NguoiGioiThieu= gt.ID 	   
			order by NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_HangTraLai]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang uniqueidentifier,
	@LoaiChungTu [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
set nocount on;
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);
	SELECT 
		a.MaChungTuGoc, 
    	a.MaChungTu, 
    	a.NgayLapHoaDon,
    	a.MaHangHoa,
    	a.TenHangHoaFull,
    	a.TenHangHoa,
    	a.TenDonViTinh,
    	a.ThuocTinh_GiaTri,
    	a.TenLoHang,
    	CAST(ROUND(a.SoLuongTra, 3) as float) as SoLuong,
		CAST(ROUND(a.ThanhTien, 0) as float) as ThanhTien,
		CAST(ROUND(a.GiamGiaHD, 0) as float) as GiamGiaHD,
    	CAST(ROUND(a.ThanhTien - a.GiamGiaHD, 0) as float) as GiaTriTra,
    	a.TenNhanVien,
		a.GhiChu ,
		a.LoaiHoaDon
	FROM 
		(select 
			Case when hdb.ID is null then 1 else hdb.LoaiHoaDon end as LoaiHoaDon,
			Case when hdb.ID is null then N'HĐ trả nhanh' else hdb.MaHoaDon end as MaChungTuGoc,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa,
			hdt.MaHoaDon as MaChungTu,
			hdt.NgayLapHoaDon,
			dvqd.MaHangHoa,
			concat(hh.TenHangHoa , dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
			hh.TenHangHoa,
			dvqd.TenDonViTinh as TenDonViTinh,
			dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			lh.MaLoHang as TenLoHang,
			hdct.SoLuong as SoLuongTra,
			Case when hdt.TongTienHang = 0 then 0 else hdct.ThanhTien * ((hdt.TongGiamGia + hdt.KhuyeMai_GiamGia) / hdt.TongTienHang) end as GiamGiaHD,
			hdct.ThanhTien as ThanhTien,
			nv.TenNhanVien,
			hdct.GhiChu 
			from BH_HoaDon hdt
			LEFT JOIN BH_HoaDon hdb ON hdt.ID_HoaDon = hdb.ID
			INNER JOIN BH_HoaDon_ChiTiet hdct ON hdct.ID_HoaDon = hdt.ID			
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join NS_NhanVien nv on hdt.ID_NhanVien = nv.ID
			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID AND hh.ID = lh.ID_HangHoa
			INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(null)) allnhh
			ON hh.ID_NhomHang = allnhh.ID
			INNER JOIN (select * from splitstring(@ID_ChiNhanh)) lstID_DonVi
			ON lstID_DonVi.Name = hdt.ID_DonVi
			WHERE hdt.LoaiHoaDon = 6
			and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null) 
			and (hdct.ID_ParentCombo = hdct.ID or hdct.ID_ParentCombo is null)
			AND hdt.NgayLapHoaDon >= @timeStart AND hdt.NgayLapHoaDon < @timeEnd
			AND hdt.ChoThanhToan = 0
    		and hh.TheoDoi like @TheoDoi
    		and dvqd.Xoa like @TrangThai
			AND ((select count(Name) from @tblSearchString b where 
			hdt.MaHoaDon like '%'+b.Name+'%' 
    		OR hdb.MaHoaDon like '%'+b.Name+'%' 
    		or dvqd.MaHangHoa like '%'+b.Name+'%' 
    			or hh.TenHangHoa like '%'+b.Name+'%'
    			or hh.TenHangHoa_KhongDau like '%' +b.Name +'%' 
				or hh.TenHangHoa_KyTuDau like '%' +b.Name +'%'
    		or lh.MaLoHang like '%'+b.Name+'%'
			or nv.MaNhanVien like '%'+b.Name+'%'
    			or nv.TenNhanVien like '%'+b.Name+'%'
    			or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
				or dvqd.TenDonViTinh like '%'+b.Name+'%'
				or dvqd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)) a
	WHERE a.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
	and a.LoaiHangHoa in (select name from dbo.splitstring(@LoaiHangHoa))
	ORDER BY a.NgayLapHoaDon DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_TongHop_Page]
    @pageNumber [int],
    @pageSize [int],
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang uniqueidentifier,
	@LoaiChungTu [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET @pageNumber = @pageNumber - 1; --- because @pageNumber from 1
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	

		DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);

		declare @tblCTHD table (
		NgayLapHoaDon datetime,
		MaHoaDon nvarchar(max),
		LoaiHoaDon int,
		ID_DonVi uniqueidentifier,
		ID_PhieuTiepNhan uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID_NhanVien uniqueidentifier,
		TongTienHang float,
		TongGiamGia	float,
		KhuyeMai_GiamGia float,
		ChoThanhToan bit,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		ID_ChiTietGoiDV	uniqueidentifier,
		ID_ChiTietDinhLuong uniqueidentifier,
		ID_ParentCombo uniqueidentifier,
		SoLuong float,
		DonGia float,
		GiaVonfloat float,
		TienChietKhau float,
		TienChiPhi float,
		ThanhTien float,
		ThanhToan float,
		GhiChu nvarchar(max),
		ChatLieu nvarchar(max),
		LoaiThoiGianBH int,
		ThoiGianBaoHanh float,
		TenHangHoaThayThe nvarchar(max),
		TienThue float,	
		GiamGiaHD float,
		GiaVon float,
		TienVon float
		)

	insert into @tblCTHD
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu

	declare @tblChiPhi table (ID_ParentCombo uniqueidentifier,ID_DonViQuiDoi uniqueidentifier, ChiPhi float, 
		ID_NhanVien uniqueidentifier,ID_DoiTuong uniqueidentifier)
	insert into @tblChiPhi
	exec BCBanHang_GetChiPhi @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu
    	
		select *			
		into #tblView
		from
		(
		select 
			hh.ID, hh.TenHangHoa,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa,
			qd.MaHangHoa,			
			concat(hh.TenHangHoa, qd.ThuocTinhGiaTri) as TenHangHoaFull,
			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			ISNULL(nhh.TenNhomHangHoa,  N'Nhóm hàng hóa mặc định') as TenNhomHangHoa,
			lo.MaLoHang as TenLoHang,
			qd.TenDonViTinh,
			cast(c.SoLuong as float) as SoLuong,
			cast(c.ThanhTien as float) as ThanhTien,
			cast(c.GiamGiaHD as float) as GiamGiaHD,
			cast(c.TongTienThue as float) as TongTienThue,
			iif(@XemGiaVon='1',cast(c.TienVon as float),0) as TienVon,
			cast(c.ThanhTien - c.GiamGiaHD as float) as DoanhThuThuan,
			iif(@XemGiaVon='1',cast(c.ThanhTien - c.GiamGiaHD - c.TienVon - c.ChiPhi as float),0) as LaiLo,
			c.ChiPhi
		from 
		(
		select 
			sum(b.SoLuong * isnull(qd.TyLeChuyenDoi,1)) as SoLuong,
			sum(b.ThanhTien) as ThanhTien,
			sum(b.TienVon) as TienVon,
			qd.ID_HangHoa,
			b.ID_LoHang,			
			sum(b.GiamGiaHD) as GiamGiaHD,
			sum(b.TongTienThue) as TongTienThue	,
			sum(ChiPhi) as ChiPhi
		from (
		select 		
			a.ID_LoHang, a.ID_DonViQuiDoi,			
			sum(isnull(a.TienThue,0)) as TongTienThue,
			sum(isnull(a.GiamGiaHD,0)) as GiamGiaHD,
			sum(SoLuong) as SoLuong,
			sum(ThanhTien) as ThanhTien,
			sum(TienVon) as TienVon,
			sum(ChiPhi) as ChiPhi
		from
		(				
		select 
			ct.ID,ct.ID_DonViQuiDoi, ct.ID_LoHang,
			ct.TienThue,
    		ct.GiamGiaHD,
			ct.SoLuong,
			ct.ThanhTien, 	
			ct.TienVon,
			isnull(cp.ChiPhi,0) as ChiPhi
		from @tblCTHD ct
		left join @tblChiPhi cp on cp.ID_ParentCombo= ct.ID
		where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)			 
			and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)	
		) a group by a.ID_LoHang, a.ID_DonViQuiDoi	
		)b
		join DonViQuiDoi qd on b.ID_DonViQuiDoi= qd.ID
		group by qd.ID_HangHoa, b.ID_LoHang					
		) c
		join DM_HangHoa hh on c.ID_HangHoa = hh.ID
		join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan=1
		left join DM_LoHang lo on c.ID_LoHang = lo.ID
		left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID		
		where 
		exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhh.ID= allnhh.ID)		
    	and
		hh.TheoDoi like @TheoDoi
		and qd.Xoa like @TrangThai
		AND
		((select count(Name) from @tblSearchString b where 
    			hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'					
    				or qd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
			) a
			where a.LoaiHangHoa in (select name from dbo.splitstring(@LoaiHangHoa))

			---- sum all column
			DECLARE @Rows FLOAT,  @TongSoLuong float, @TongThanhTien float, @TongGiamGiaHD FLOAT, 
			@TongTienVon FLOAT, @TongLaiLo FLOAT, @SumTienThue FLOAT,@TongDoanhThuThuan FLOAT,@TongChiPhi float		
			SELECT @Rows = Count(*), @TongSoLuong = SUM(SoLuong),
			@TongThanhTien = SUM(ThanhTien), @TongGiamGiaHD = SUM(GiamGiaHD),
			@TongTienVon = SUM(TienVon), @TongLaiLo = SUM(LaiLo), @SumTienThue = SUM(TongTienThue),
			@TongDoanhThuThuan = SUM(DoanhThuThuan) ,
			@TongChiPhi = SUM(ChiPhi)
			FROM #tblView;


			select *,
			@Rows as Rowns,
    		@TongSoLuong as TongSoLuong,
    		@TongThanhTien as TongThanhTien,
    		@TongGiamGiaHD as TongGiamGiaHD,
    		@TongTienVon as TongTienVon,
    		@TongLaiLo as TongLaiLo,
			@SumTienThue as SumTienThue,
    		@TongDoanhThuThuan as TongDoanhThuThuan,
			@TongChiPhi as TongChiPhi
    		from #tblView    	
    		order by TenHangHoa DESC
			OFFSET (@pageNumber* @pageSize) ROWS
    		FETCH NEXT @pageSize ROWS ONLY		
	
    	
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDichVu_TonChuaSuDung]
    @Text_Search [nvarchar](max),
    @MaHH [nvarchar](max),
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
	@ThoiHan [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NhomHang_SP [nvarchar](max)
AS
BEGIN
	set nocount on;

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tblChiNhanh table( ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@ID_ChiNhanh)

	declare @dtNow datetime = getdate()

	declare @tblCTMua table(
		MaHoaDon nvarchar(max),
		NgayLapHoaDon datetime,
		NgayApDungGoiDV datetime,
		HanSuDungGoiDV datetime,
		ID_DonVi uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		SoLuong float,
		DonGia float,
		TienChietKhau float,
		ThanhTien float,
		GiamGiaHD float)
	insert into @tblCTMua
	exec BaoCaoGoiDV_GetCTMua @ID_ChiNhanh,'2016-01-01',@timeEnd

	select 
		b.MaHangHoa,
		b.TenHangHoa,
		concat(b.TenHangHoa,b.ThuocTinh_GiaTri) as TenHangHoaFull,
		b.TenDonViTinh,		
		b.TenNhomHangHoa as TenNhomHang,
		b.ThuocTinh_GiaTri,
		b.SoLuongBan,
		b.GiaTriBan,
		b.SoLuongTra,
		b.GiaTriTra,
		b.SoLuongSuDung,
		b.GiaTriSuDung,		
		b.SoLuongBan - b.SoLuongTra - b.SoLuongSuDung as SoLuongConLai,
		b.GiaTriBan - b.GiaTriTra - b.GiaTriSuDung as GiaTriConLai
	from
	(
	select 
		qd.MaHangHoa,
		hh.TenHangHoa,
		qd.TenDonViTinh,
		isnull(qd.ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
		nhom.TenNhomHangHoa,
		iif(hh.ID_NhomHang is null, '00000000-0000-0000-0000-000000000000',hh.ID_NhomHang) as ID_NhomHang,
		sum(tbl.SoLuong) as SoLuongBan,
		sum(tbl.ThanhTien) as GiaTriBan,
		sum(tbl.SoLuongTra) as SoLuongTra,
		sum(tbl.GiaTriTra) as GiaTriTra,
		sum(tbl.SoLuongSuDung) as SoLuongSuDung,
		sum(tbl.GiaTriSuDung) as GiaTriSuDung		
	from
	(
		select 
			ctm.ID_DonViQuiDoi,
			ctm.ID_LoHang,
			ctm.ID_DoiTuong,
			ctm.SoLuong,
			ctm.ThanhTien,
			ctm.MaHoaDon,
			isnull(tbl.SoLuongSuDung,0) as SoLuongSuDung,
			isnull(tbl.GiaTriSuDung,0) as GiaTriSuDung,
			isnull(tbl.SoLuongTra,0) as SoLuongTra,
			isnull(tbl.GiaTriTra,0) as GiaTriTra ,
			iif(ctm.HanSuDungGoiDV is null,1, iif(@dtNow < HanSuDungGoiDV,1,0)) as ThoiHan
		from  @tblCTMua ctm		
		left join (
							select 
								tblSD.ID_ChiTietGoiDV,
								sum(tblSD.SoLuongTra) as SoLuongTra,
								sum(tblSD.GiaTriTra) as GiaTriTra,
								sum(tblSD.SoLuongSuDung) as SoLuongSuDung,
								sum(tblSD.GiaTriSuDung) as GiaTriSuDung
							from 
							(
								---- hdsudung
								Select 								
									ct.ID_ChiTietGoiDV,														
									0 as SoLuongTra,
									0 as GiaTriTra,
									ct.SoLuong as SoLuongSuDung,
									ct.SoLuong * ct.DonGia as GiaTriSuDung
								FROM BH_HoaDon hd
								join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
								join @tblCTMua ctm on ctm.ID= ct.ID_ChiTietGoiDV
								where hd.ChoThanhToan= 0
								and hd.LoaiHoaDon in (1,25)
								and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
							

								union all
								--- hdtra
								Select 							
									ct.ID_ChiTietGoiDV,															
									ct.SoLuong as SoLuongTra,
									ct.ThanhTien as GiaTriTra,
									0 as SoLuongSuDung,
									0 as GiaVon
								FROM BH_HoaDon hd
								join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
								join @tblCTMua ctm on ctm.ID= ct.ID_ChiTietGoiDV
								where hd.ChoThanhToan= 0
								and hd.LoaiHoaDon = 6
								and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)							
								)tblSD group by tblSD.ID_ChiTietGoiDV
						) tbl on ctm.ID= tbl.ID_ChiTietGoiDV
		) tbl
		left join DonViQuiDoi qd on tbl.ID_DonViQuiDoi = qd.ID
		left join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
		left join DM_LoHang lo on tbl.ID_LoHang= lo.ID
		left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
		left join DM_DoiTuong dt on tbl.ID_DoiTuong = dt.ID		
		where tbl.ThoiHan like @ThoiHan
		and hh.LaHangHoa like @LaHangHoa		
    	and hh.TheoDoi like @TheoDoi
    	and qd.Xoa like @TrangThai
		AND 
		((select count(Name) 
			from @tblSearchString b where 
			tbl.MaHoaDon like '%'+b.Name+'%'
    		or hh.TenHangHoa like '%'+b.Name+'%'
    		or qd.MaHangHoa like '%'+b.Name+'%'
    		or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
			or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'	
			)=@count or @count=0)
			group by qd.ID, qd.MaHangHoa, hh.TenHangHoa,
				qd.TenDonViTinh,
				qd.ThuocTinhGiaTri,
				nhom.TenNhomHangHoa,
				hh.ID_NhomHang
		) b
		where (b.ID_NhomHang like @ID_NhomHang or b.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDoanhThuSuaChuaTongHop]
    @IdChiNhanhs [nvarchar](max),
    @ThoiGianFrom [datetime],
    @ThoiGianTo [datetime],
    @DoanhThuFrom [float],
    @DoanhThuTo [float],
    @LoiNhuanFrom [float],
    @LoiNhuanTo [float],
    @TextSearch [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Insert statements for procedure here
    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    	Select @count =  (Select count(*) from @tblSearch);
    
    	DECLARE @tblHoaDonSuaChua TABLE (IDPhieuTiepNhan UNIQUEIDENTIFIER, MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), 
    	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
    	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, TongTienHang FLOAT, TongChietKhau FLOAT, TongTienThue FLOAT, TongChiPhi FLOAT,
    	TongGiamGia FLOAT, TongThanhToan FLOAT, GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));
    
    	INSERT INTO @tblHoaDonSuaChua
    	SELECT ptn.ID, ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, dmx.BienSo, dt.MaDoiTuong, dt.TenDoiTuong, nv.TenNhanVien, hd.ID,
    	hd.MaHoaDon, hd.NgayLapHoaDon, SUM(hdct.SoLuong* hdct.DonGia), SUM(ISNULL(hdct.TienChietKhau, 0)*hdct.SoLuong), hd.TongTienThue, hd.TongChiPhi,
    	hd.TongGiamGia, hd.TongThanhToan - hd.TongTienThue, hd.DienGiai, dv.MaDonVi, dv.TenDonVi FROM Gara_PhieuTiepNhan ptn
    	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
    	INNER JOIN BH_HoaDon_ChiTiet hdct ON hd.ID = hdct.ID_HoaDon
    	INNER JOIN Gara_DanhMucXe dmx ON ptn.ID_Xe = dmx.ID
    	INNER JOIN DM_DoiTuong dt ON dt.ID = ptn.ID_KhachHang
    	LEFT JOIN NS_NhanVien nv ON ptn.ID_CoVanDichVu = nv.ID
    	INNER JOIN DM_DonVi dv ON dv.ID = ptn.ID_DonVi
    	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
    	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0 AND (hdct.ID_ParentCombo = hdct.ID OR hdct.ID_ParentCombo IS NULL)
    	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
    	AND ((select count(Name) from @tblSearch b where     			
    			ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
    			or dmx.BienSo like '%'+b.Name+'%'
    			or dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or nv.TenNhanVien like '%'+b.Name+'%'
    			or hd.MaHoaDon like '%'+b.Name+'%'
    			or hd.DienGiai like '%'+b.Name+'%'
    			)=@count or @count=0)
    	GROUP BY ptn.ID, ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, dmx.BienSo, dt.MaDoiTuong, dt.TenDoiTuong, nv.TenNhanVien, hd.ID,
    	hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienThue, hd.TongChiPhi,
    	hd.TongGiamGia, hd.TongThanhToan, hd.DienGiai, dv.MaDonVi, dv.TenDonVi;
    
    	DECLARE @tblBaoCaoDoanhThu TABLE(MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), 
    	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
    	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, TongTienHang FLOAT, TongChietKhau FLOAT, TongTienThue FLOAT, TongChiPhi FLOAT,
    	TongGiamGia FLOAT, TongThanhToan FLOAT, GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), GiaVon FLOAT, TienVon FLOAT, LoiNhuan FLOAT)
    
    	INSERT INTO @tblBaoCaoDoanhThu
    	SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, 
    	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
    	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon, hdsc.TongTienHang, hdsc.TongChietKhau, hdsc.TongTienThue, hdsc.TongChiPhi,
    	hdsc.TongGiamGia, hdsc.TongThanhToan, hdsc.GhiChu, hdsc.MaDonVi, hdsc.TenDonVi, SUM(ISNULL(hdsc.GiaVon,0)) AS GiaVon, SUM(ISNULL(hdsc.GiaVon,0)*ISNULL(hdsc.SoLuongxk,0)) AS TienVon,
    	hdsc.TongThanhToan - SUM(ISNULL(hdsc.GiaVon,0)*ISNULL(hdsc.SoLuongxk,0)) AS LoiNhuan
    	FROM (
		SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, 
    	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
    	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon, hdsc.TongTienHang, hdsc.TongChietKhau, hdsc.TongTienThue, hdsc.TongChiPhi,
    	hdsc.TongGiamGia, hdsc.TongThanhToan, hdsc.GhiChu, hdsc.MaDonVi, hdsc.TenDonVi, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk
    	FROM @tblHoaDonSuaChua hdsc
    	LEFT JOIN BH_HoaDon xk ON hdsc.ID = xk.ID_HoaDon AND xk.ChoThanhToan = 0
    	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL
		UNION ALL
		SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo,
    	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
    	NULL, '', null, 0, 0, 0, 0, 0, 0, 
    	'', hdsc.MaDonVi, hdsc.TenDonVi, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk FROM
		(SELECT IDPhieuTiepNhan, MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu, 
		MaDonVi, TenDonVi
    	FROM @tblHoaDonSuaChua GROUP BY IDPhieuTiepNhan, MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu, MaDonVi, 
		TenDonVi)
		hdsc
    	INNER JOIN BH_HoaDon xk ON hdsc.IDPhieuTiepNhan = xk.ID_PhieuTiepNhan
		INNER JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon 
    	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) AND xk.ID_HoaDon IS NULL
		) hdsc
    	GROUP BY hdsc.BienSo, hdsc.CoVanDichVu, hdsc.GhiChu, hdsc.ID, hdsc.MaDoiTuong, hdsc.MaHoaDon, 
    	hdsc.MaPhieuTiepNhan, hdsc.NgayLapHoaDon, hdsc.NgayVaoXuong, hdsc.TenDoiTuong,
    	hdsc.TongChietKhau, hdsc.TongChiPhi, hdsc.TongGiamGia, hdsc.TongThanhToan,
    	hdsc.TongTienHang, hdsc.TongTienThue, hdsc.MaDonVi, hdsc.TenDonVi
		
		DECLARE @tblChiPhi TABLE(IDHoaDon UNIQUEIDENTIFIER, TongChiPhi FLOAT);
		INSERT INTO @tblChiPhi
		SELECT hdcp.ID_HoaDon, SUM(ThanhTien) FROM BH_HoaDon_ChiPhi hdcp
		INNER JOIN @tblBaoCaoDoanhThu bcdt ON hdcp.ID_HoaDon = bcdt.ID
		GROUP BY hdcp.ID_HoaDon;

		UPDATE bcdt
		SET bcdt.TongChiPhi = hdcp.TongChiPhi, bcdt.LoiNhuan = bcdt.LoiNhuan - hdcp.TongChiPhi FROM @tblBaoCaoDoanhThu bcdt
		INNER JOIN @tblChiPhi hdcp ON bcdt.ID = hdcp.IDHoaDon;

    	DECLARE @STongTienHang FLOAT,  @SChietKhau FLOAT, @SThue FLOAT, @SChiPhi FLOAT, @SGiamGia FLOAT, @SDoanhThu FLOAT, @STongTienVon FLOAT, @SLoiNhuan FLOAT
    	SELECT @STongTienHang = SUM(TongTienHang), @SChietKhau = SUM(TongChietKhau), @SThue = SUM(TongTienThue),
    	@SChiPhi = SUM(TongChiPhi), @SGiamGia = SUM(TongGiamGia), @SDoanhThu = SUM(TongThanhToan), @STongTienVon = SUM(TienVon), @SLoiNhuan = SUM(LoiNhuan) 
    	FROM @tblBaoCaoDoanhThu
    
    	SELECT MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu , ID AS IDHoaDon, MaHoaDon,
    	NgayLapHoaDon, ISNULL(TongTienHang, 0) AS TongTienHang, ISNULL(TongChietKhau, 0) AS TongChietKhau, ISNULL(TongTienThue, 0) AS TongTienThue, 
    	ISNULL(TongChiPhi, 0) AS TongChiPhi, ISNULL(TongGiamGia, 0) AS TongGiamGia, 
    	ISNULL(TongThanhToan, 0) AS DoanhThu, ISNULL(Tienvon, 0) AS TienVon, ISNULL(LoiNhuan, 0) AS LoiNhuan, GhiChu, MaDonVi, TenDonVi, ISNULL(@STongTienHang, 0) AS STongTienHang, ISNULL(@SChietKhau,0) AS SChietKhau,
    	ISNULL(@SThue,0) AS SThue, ISNULL(@SChiPhi,0) AS SChiPhi, ISNULL(@SGiamGia,0) AS SGiamGia, ISNULL(@SDoanhThu, 0) AS SDoanhThu, ISNULL(@STongTienVon,0) AS STongTienVon,
    	ISNULL(@SLoiNhuan,0) AS SLoiNhuan
    	FROM @tblBaoCaoDoanhThu
    	WHERE (@DoanhThuFrom IS NULL OR TongThanhToan >= @DoanhThuFrom)
    	AND (@DoanhThuTo IS NULL OR TongThanhToan <= @DoanhThuTo)
    	AND (@LoiNhuanFrom IS NULL OR LoiNhuan >= @LoiNhuanFrom)
    	AND (@LoiNhuanTo IS NULL OR LoiNhuan <= @LoiNhuanTo)
    	ORDER BY NgayLapHoaDon
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapChuyenHangChiTiet]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	  SET NOCOUNT ON;
	 DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	    		
    		where nd.ID = @ID_NguoiDung);


				select 
					CNChuyen.TenDonVi as ChiNhanhChuyen,
					CNnhan.TenDonVi as ChiNhanhNhan,
					tblHD.NgayLapHoaDon,tblHD.MaHoaDon,
					isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
					isnull(lo.MaLoHang,'') as TenLoHang,
					qd.MaHangHoa, qd.TenDonViTinh, 
					isnull(qd.ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
					hh.TenHangHoa,
					CONCAT(hh.TenHangHoa,qd.ThuocTinhGiaTri) as TenHangHoaFull,				
					round(tblHD.SoLuong, 3) as SoLuong,
					iif(@XemGiaVon='1',round(tblHD.DonGia,3),0) as DonGia,
					iif(@XemGiaVon='1',round(tblHD.GiaVon,3),0) as GiaVon,
					iif(@XemGiaVon='1',round(tblHD.ThanhTien,3),0) as ThanhTien,
					iif(@XemGiaVon='1',round(tblHD.GiaTri,3),0) as GiaTri			
				from(
					select 
						qd.ID_HangHoa,tblHD.ID_LoHang, 
						tblHD.ID_DonVi,tblHD.ID_CheckIn, tblHD.NgayLapHoaDon,tblHD.MaHoaDon,
						sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
						max(tblHD.GiaVon / iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as GiaVon,
						max(tblHD.DonGia / iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as DonGia,
						sum(tblHD.ThanhTien) as ThanhTien,
						sum(tblHD.GiaTri) as GiaTri
					from(
					select ct.ID_DonViQuiDoi, ct.ID_LoHang, 
						hd.ID_DonVi, hd.ID_CheckIn, hd.NgayLapHoaDon, hd.MaHoaDon,
						sum(ct.TienChietKhau) as SoLuong,
						max(ct.GiaVon) as GiaVon,
						max(ct.DonGia) as DonGia,
						sum(ct.TienChietKhau * ct.DonGia) as ThanhTien, --- get gtri nhan
						sum(ct.TienChietKhau * ct.GiaVon) as GiaTri
					from BH_HoaDon_ChiTiet ct
					join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
					where hd.ChoThanhToan=0
					and hd.LoaiHoaDon= 10 and (hd.YeuCau='1' or hd.YeuCau='4') --- YeuCau: 1.DangChuyen, 4.DaNhan, 2.PhieuTam, 3.Huy
					and hd.NgaySua >=@timeStart and hd.NgaySua < @timeEnd
					and exists (select ID from @tblIdDonVi dv where hd.ID_CheckIn= dv.ID)
					group by ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn,hd.NgayLapHoaDon, hd.MaHoaDon
					)tblHD
					join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID
					group by qd.ID_HangHoa, tblHD.ID_DonViQuiDoi,tblHD.ID_LoHang, tblHD.ID_DonVi,tblHD.ID_CheckIn,tblHD.NgayLapHoaDon, tblHD.MaHoaDon
				)tblHD
				join DM_DonVi CNChuyen on tblHD.ID_DonVi = CNChuyen.ID
				left join DM_DonVi CNnhan on tblHD.ID_CheckIn= CNnhan.ID
				join DM_HangHoa hh on tblHD.ID_HangHoa= hh.ID
				join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
				left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
				left join DM_LoHang lo on tblHD.ID_LoHang= lo.ID and (lo.ID= tblHD.ID_LoHang or (tblHD.ID_LoHang is null and lo.ID is null))
				where hh.LaHangHoa = 1
				and hh.TheoDoi like @TheoDoi
				and qd.Xoa like @TrangThai
				and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID= allnhh.ID)
				AND ((select count(Name) from @tblSearchString b where 
    			hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa like '%'+b.Name+'%'
    			or lo.MaLoHang like '%' +b.Name +'%' 
				or qd.MaHangHoa like '%'+b.Name+'%'
				or qd.MaHangHoa like '%'+b.Name+'%'
    			or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or qd.TenDonViTinh like '%'+b.Name+'%'
    			or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
				or CNnhan.TenDonVi like '%'+b.Name+'%'
				or CNChuyen.TenDonVi like '%'+b.Name+'%')=@count or @count=0)
				order by tblHD.NgayLapHoaDon desc, hh.TenHangHoa, lo.MaLoHang	 
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTon]
	@ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
    @timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
    @CoPhatSinh [int]
AS
BEGIN

	SET NOCOUNT ON;

    DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER, MaDonVi nvarchar(max), TenDonVi nvarchar(max));
    INSERT INTO @tblChiNhanh 
	SELECT dv.ID, dv.MaDonVi, dv.TenDonVi 
	FROM splitstring(@ID_DonVi) cn
	join DM_DonVi  dv on cn.Name= dv.ID

	declare @dtNow datetime = format(getdate(),'yyyy-MM-dd')  

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung);
	

	declare @tkDauKy table (ID_DonVi uniqueidentifier,ID_HangHoa uniqueidentifier,	ID_LoHang uniqueidentifier null, TonKho float,GiaVon float)		
	insert into @tkDauKy
	exec dbo.GetAll_TonKhoDauKy @ID_DonVi, @timeStart

	

			------ tonkho trongky
			select 			
				qd.ID_HangHoa,
				tkNhapXuat.ID_LoHang,
				tkNhapXuat.ID_DonVi,				
				sum(tkNhapXuat.SoLuongNhap * qd.TyLeChuyenDoi) as SoLuongNhap,
				sum(tkNhapXuat.GiaTriNhap ) as GiaTriNhap,
				sum(tkNhapXuat.SoLuongXuat * qd.TyLeChuyenDoi) as SoLuongXuat,
				sum(tkNhapXuat.GiaTriXuat) as GiaTriXuat
				into #temp
			from
			(
			-- xuat ban, trahang ncc, xuatkho, xuat chuyenhang
				select 
					ct.ID_DonViQuiDoi,
					ct.ID_LoHang,
					hd.ID_DonVi,
					0 AS SoLuongNhap,
					0 AS GiaTriNhap,
					sum(
						case hd.LoaiHoaDon
						when 10 then ct.TienChietKhau
						else ct.SoLuong end ) as SoLuongXuat,
					sum( 
						case hd.LoaiHoaDon
						when 7 then ct.SoLuong* ct.DonGia
						when 10 then ct.TienChietKhau * ct.GiaVon
						else ct.SoLuong* ct.GiaVon end )  AS GiaTriXuat
					FROM BH_HoaDon_ChiTiet ct
				LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
				WHERE hd.ChoThanhToan = 0
				and (hd.LoaiHoaDon in (1,5,7,8) 
					or (hd.LoaiHoaDon = 10  and (hd.YeuCau='1' or hd.YeuCau='4')) )
				AND hd.NgayLapHoaDon between  @timeStart AND   @timeEnd
				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
				GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi								


				UNION ALL
				 ---nhap chuyenhang
				SELECT 
					ct.ID_DonViQuiDoi,
					ct.ID_LoHang,
					hd.ID_CheckIn AS ID_DonVi,
					SUM(ct.TienChietKhau) AS SoLuongNhap,
					SUM(ct.TienChietKhau* ct.DonGia) AS GiaTriNhap, -- lay giatri tu chinhanh chuyen
					0 AS SoLuongXuat,
					0 AS GiaTriXuat
				FROM BH_HoaDon_ChiTiet ct
				LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
				WHERE hd.LoaiHoaDon = 10 and hd.YeuCau = '4' AND hd.ChoThanhToan = 0
				and exists (select ID from @tblChiNhanh dv where hd.ID_CheckIn = dv.ID)
				AND hd.NgaySua between  @timeStart AND   @timeEnd
				GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn

    			UNION ALL
				 ---nhaphang + khach trahang
				SELECT 
					ct.ID_DonViQuiDoi,
					ct.ID_LoHang,
					hd.ID_DonVi,
					SUM(ct.SoLuong) AS SoLuongNhap,
					--- KH trahang: giatrinhap = giavon (khong lay giaban)
					sum(iif(hd.LoaiHoaDon= 6, iif(ctm.GiaVon is null or ctm.ID = ctm.ID_ChiTietDinhLuong, ct.SoLuong * ct.GiaVon, ct.SoLuong *ctm.GiaVon),
					iif( hd.TongTienHang = 0,0, ct.SoLuong* (ct.DonGia - ct.TienChietKhau) * (1- hd.TongGiamGia/hd.TongTienHang))))  AS GiaTriNhap,
					0 AS SoLuongXuat,
					0 AS GiaTriXuat
				FROM BH_HoaDon_ChiTiet ct
				LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
				left join BH_HoaDon_ChiTiet ctm on ct.ID_ChiTietGoiDV = ctm.ID
				WHERE (hd.LoaiHoaDon = '4' or hd.LoaiHoaDon = '6') 
				AND hd.ChoThanhToan = 0
				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
				AND hd.NgayLapHoaDon between  @timeStart AND   @timeEnd
				GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi
    
    			UNION ALL
				-- kiemke
    			SELECT 
					ctkk.ID_DonViQuiDoi, 
					ctkk.ID_LoHang, 
					ctkk.ID_DonVi, 
					sum(isnull(SoLuongNhap,0)) as SoLuongNhap,
					sum(isnull(SoLuongNhap,0) * ctkk.GiaVon) as GiaTriNhap,
					sum(isnull(SoLuongXuat,0)) as SoLuongXuat,
					sum(isnull(SoLuongXuat,0) * ctkk.GiaVon) as GiaTriXuat
				FROM
    			(SELECT 
    				ct.ID_DonViQuiDoi,
    				ct.ID_LoHang,
					hd.ID_DonVi,
					IIF(ct.SoLuong< 0, 0, ct.SoLuong) as SoLuongNhap,
					IIF(ct.SoLuong < 0, - ct.SoLuong, 0) as SoLuongXuat,
					ct.GiaVon
    			FROM BH_HoaDon_ChiTiet ct 
    			LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
    			WHERE hd.LoaiHoaDon = '9' 
    			AND hd.ChoThanhToan = 0
				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
    			AND hd.NgayLapHoaDon between  @timeStart AND   @timeEnd  			
				) ctkk	
    			GROUP BY ctkk.ID_DonViQuiDoi, ctkk.ID_LoHang, ctkk.ID_DonVi
			)tkNhapXuat
			join DonViQuiDoi qd on tkNhapXuat.ID_DonViQuiDoi = qd.ID
			group by qd.ID_HangHoa, tkNhapXuat.ID_LoHang, tkNhapXuat.ID_DonVi


			if	@CoPhatSinh= 2
					begin
							select 
							a.TenNhomHang,
							a.TenHangHoa,
							a.MaHangHoa,
							a.TenDonViTinh,
							a.TenLoHang,
							a.TenDonVi,
							a.MaDonVi,
							concat(a.TenHangHoa,a.ThuocTinhGiaTri) as TenHangHoaFull,
							-- dauky
							isnull(a.TonDauKy,0) as TonDauKy,
							iif(@XemGiaVon='1',isnull(a.GiaTriDauKy,0),0) as GiaTriDauKy,

							--- trongky
							isnull(a.SoLuongNhap,0) as SoLuongNhap,
							iif(@XemGiaVon='1',isnull(a.GiaTriNhap,0),0) as GiaTriNhap,
							isnull(a.SoLuongXuat,0) as SoLuongXuat,
							iif(@XemGiaVon='1',isnull(a.GiaTriXuat,0),0) as GiaTriXuat,

							-- cuoiky
							isnull(a.TonDauKy,0) + isnull(a.SoLuongNhap,0) - isnull(a.SoLuongXuat,0) as TonCuoiKy,
							(isnull(a.TonDauKy,0) + isnull(a.SoLuongNhap,0) - isnull(a.SoLuongXuat,0)) * iif(a.QuyCach=0 or a.QuyCach is null,1, a.QuyCach)  as TonQuyCach,
							iif(@XemGiaVon='1',isnull(a.GiaTriDauKy,0) + isnull(a.GiaTriNhap,0) - isnull(a.GiaTriXuat,0),0)  as GiaTriCuoiKy
						from
						(
							select 
								isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
								hh.TenHangHoa,
								hh.QuyCach,
								qd.MaHangHoa,
								qd.TenDonViTinh,
								isnull(lo.MaLoHang,'') as TenLoHang,
								dv.TenDonVi,
								dv.MaDonVi,
								qd.ThuocTinhGiaTri,
				
								-- dauky	

								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.TonKho, 0) as TonDauKy,		
								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.TonKho, 0) * iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.GiaVon, 0) as GiaTriDauKy,
								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.GiaVon, 0) as GiaVon,	
							
									----trongky
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongNhap, 0) as SoLuongNhap,		
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongXuat, 0) as SoLuongXuat,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.GiaTriNhap, 0) as GiaTriNhap,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.GiaTriXuat, 0) as GiaTriXuat
								
							from #temp tkTrongKy
							left join DM_HangHoa hh on tkTrongKy.ID_HangHoa= hh.ID
							left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
							left join DonViQuiDoi qd on tkTrongKy.ID_HangHoa = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
							left join DM_LoHang lo on tkTrongKy.ID_LoHang= lo.ID or lo.ID is null
							cross join @tblChiNhanh  dv									
							left join @tkDauKy tkDauKy on hh.ID = tkDauKy.ID_HangHoa 
							and tkTrongKy.ID_DonVi= tkDauKy.ID_DonVi and ((lo.ID= tkDauKy.ID_LoHang) or (lo.ID is null and hh.QuanLyTheoLoHang = 0 ))							
							where hh.LaHangHoa= 1
								AND hh.TheoDoi LIKE @TheoDoi	
								and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID = allnhh.ID)	
								and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
									AND ((select count(Name) from @tblSearchString b where 
    								hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    								or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    									or hh.TenHangHoa like '%'+b.Name+'%'
    									or lo.MaLoHang like '%' +b.Name +'%' 
    									or qd.MaHangHoa like '%'+b.Name+'%'
    									or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    									or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    									or nhom.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    									or qd.TenDonViTinh like '%'+b.Name+'%'
    									or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
										or dv.MaDonVi like '%'+b.Name+'%'
										or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)		
							) a	order by TenHangHoa, TenDonVi,TenLoHang		
					end
			else
			begin
			
					select 	
							dv.MaDonVi, dv.TenDonVi,
							qd.MaHangHoa,
							hh.TenHangHoa,
							hh.QuyCach,
							lo.ID as ID_LoHang,
							qd.ID as ID_DonViQuyDoi,
							concat(hh.TenHangHoa,ThuocTinhGiaTri) as TenHangHoaFull,
							isnull(ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
							isnull(qd.TenDonViTinh,'') as TenDonViTinh,
							isnull(lo.MaLoHang,'') as TenLoHang,

							---- dauky
							isnull(tkDauKy.TonKho,0) as TonDauKy,
							isnull(tkDauKy.GiaVon,0) as GiaVon,				
							iif(@XemGiaVon='1',isnull(tkDauKy.TonKho,0) * isnull(tkDauKy.GiaVon,0),0)  as GiaTriDauKy,			
							isnull(nhom.TenNhomHangHoa,N'Nhóm Hàng Hóa Mặc Định') TenNhomHang,

							---- trongky
							isnull(tkTrongKy.SoLuongNhap,0) as SoLuongNhap,
							isnull(tkTrongKy.SoLuongXuat,0) as SoLuongXuat,
							iif(@XemGiaVon='1',isnull(tkTrongKy.GiaTriNhap,0),0) as GiaTriNhap,
							iif(@XemGiaVon='1',isnull(tkTrongKy.GiaTriXuat,0),0) as GiaTriXuat,

							---- cuoiky
							isnull(tkDauKy.TonKho,0) + isnull(tkTrongKy.SoLuongNhap,0) - isnull(tkTrongKy.SoLuongXuat,0) as TonCuoiKy,
							(isnull(tkDauKy.TonKho,0) + isnull(tkTrongKy.SoLuongNhap,0) - isnull(tkTrongKy.SoLuongXuat,0))  * iif(hh.QuyCach=0 or hh.QuyCach is null,1, hh.QuyCach) as TonQuyCach,
							iif(@XemGiaVon='1',isnull(tkDauKy.TonKho,0) * isnull(tkDauKy.GiaVon,0) + isnull(tkTrongKy.GiaTriNhap,0)
							- isnull(tkTrongKy.GiaTriXuat,0),0) as GiaTriCuoiKy
					from DM_HangHoa hh 		
					join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
					left join DM_LoHang lo on hh.ID = lo.ID_HangHoa
					left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
					cross join @tblChiNhanh  dv		
					left join @tkDauKy tkDauKy 
					on qd.ID_HangHoa = tkDauKy.ID_HangHoa and tkDauKy.ID_DonVi= dv.ID and ((lo.ID= tkDauKy.ID_LoHang) or (lo.ID is null ))
					left join #temp tkTrongKy on qd.ID_HangHoa = tkTrongKy.ID_HangHoa and tkTrongKy.ID_DonVi= dv.ID and ((lo.ID= tkTrongKy.ID_LoHang) or (lo.ID is null ))
					where hh.LaHangHoa= 1
					AND hh.TheoDoi LIKE @TheoDoi	
					and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID = allnhh.ID)	
					and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
						AND ((select count(Name) from @tblSearchString b where 
    					hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    					or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    						or hh.TenHangHoa like '%'+b.Name+'%'
    						or lo.MaLoHang like '%' +b.Name +'%' 
    						or qd.MaHangHoa like '%'+b.Name+'%'
    						or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    						or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    						or nhom.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    						or qd.TenDonViTinh like '%'+b.Name+'%'
    						or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
							or dv.MaDonVi like '%'+b.Name+'%'
							or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)		
						order by TenHangHoa, TenDonVi,MaLoHang
			end

			
			
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_XuatDichVuDinhLuong]
    @SearchString [nvarchar](max),
    @LoaiChungTu [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	 DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	   		
    		where nd.ID = @ID_NguoiDung)

			select 
				ctsdQD.MaHoaDon,
				ctsdQD.NgayLapHoaDon,
				ctsdQD.TenLoaiChungTu as LoaiHoaDon,
				dv.TenDonVi,
				dv.MaDonVi,
				isnull(nv.TenNhanVien,'') as TenNhanVien,
				isnull(tn.MaPhieuTiepNhan,'') as MaPhieuTiepNhan,
				isnull(xe.BienSo,'') as BienSo,

				-- dichvu
				qddv.MaHangHoa as MaDichVu,
				qddv.TenDonViTinh as TenDonViDichVu,
				qddv.ThuocTinhGiaTri as ThuocTinhDichVu,
				hhdv.TenHangHoa as TenDichVu,
				ctsdQD.ID_DichVu,
				ctsdQD.SoLuongDichVu,
				ctsdQD.GiaTriDichVu,
				ISNULL(nhomDV.TenNhomHangHoa, N'Nhóm Dịch Vụ Mặc Định') as NhomDichVu,

				-- dinhluong
				ctsdQD.ID_DonViQuiDoi,						
				ctsdQD.SoLuongXuat as SoLuongThucTe,
				iif(@XemGiaVon='1', ctsdQD.GiaTriXuat,0) as GiaTriThucTe,
				ctsdQD.SoLuongDinhLuongBanDau,				
				iif(@XemGiaVon='1', ctsdQD.GiaTriDinhLuongBanDau,0) as GiaTriDinhLuongBanDau,
				qddl.MaHangHoa,
				qddl.TenDonViTinh,
				qddl.ThuocTinhGiaTri,
				ctsdQD.GhiChu,
				hhdl.TenHangHoa,
				concat(hhdl.TenHangHoa, qddl.ThuocTinhGiaTri) as TenHangHoaFull,
				ISNULL(nhomHH.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,


				ctsdQD.SoLuongXuat - ctsdQD.SoLuongDinhLuongBanDau as SoLuongChenhLech,
				iif(@XemGiaVon='1', ctsdQD.GiaTriXuat - ctsdQD.GiaTriDinhLuongBanDau,0) as GiaTriChenhLech,

				Case when ctsdQD.SoLuongXuat = 0 then N'Không xuất'
				when ctsdQD.SoLuongChenhLech < 0 then N'Xuất thiếu'
				when ctsdQD.SoLuongChenhLech = 0 then N'Xuất đủ'
				when (ctsdQD.SoLuongDinhLuongBanDau = 0) and ctsdQD.SoLuongXuat > 0 then N'Xuất thêm'
				else N'Xuất thừa' end as TrangThai

			from
			(select 
				ctsd.MaHoaDon,
				ctsd.NgayLapHoaDon,
				ctsd.ID_DonVi,
				ctsd.ID_NhanVien,
				ctsd.ID_PhieuTiepNhan,

				ctsd.LoaiHoaDon,
				case ctsd.LoaiHoaDon
					when 2 then N'Xuất sử dụng gói dịch vụ'
					when 3 then N'Xuất bán dịch vụ định lượng'
					when 11 then N'Xuất sửa chữa'
					else '' end TenLoaiChungTu,
				ctsd.SoLuongXuat * iif(qddl.TyLeChuyenDoi=0,1, qddl.TyLeChuyenDoi) as SoLuongXuat,
				ctsd.SoLuongDinhLuongBanDau * iif(qddl.TyLeChuyenDoi=0,1, qddl.TyLeChuyenDoi) as SoLuongDinhLuongBanDau,

				ctsd.SoLuongXuat- ctsd.SoLuongDinhLuongBanDau as SoLuongChenhLech,

				ctsd.ID_DonViQuiDoi,
				ctsd.ID_DichVu,
				ctsd.SoLuongDichVu,
				ctsd.GiaTriDichVu,
				ctsd.GiaTriXuat,
				ctsd.GiaTriDinhLuongBanDau,
				ctsd.GhiChu,
				qddl.ID_HangHoa

			from(

			select 
				a.MaHoaDon,
				a.ID_DonVi,
				a.ID_NhanVien,
				a.NgayLapHoaDon,
				a.ID_PhieuTiepNhan,
				a.LoaiHoaDon,
				a.ID_DichVu, 
				a.SoLuongDichVu,
				a.GiaTriDichVu,
				max(SoLuongDinhLuongBanDau) as SoLuongDinhLuongBanDau,			
				max(GiaTriDinhLuongBanDau) as GiaTriDinhLuongBanDau,			
				sum(isnull(SoLuongXuat, 0)) as SoLuongXuat,
				sum(isnull(GiaTriXuat, 0)) as GiaTriXuat,
				(a.ID_DonViQuiDoi) as ID_DonViQuiDoi,
				max(isnull(a.GhiChu,'')) as GhiChu
			from
			(
					--- xuatban, xuatsudung
					select 
						hd.MaHoaDon,
						hd.ID_DonVi,
						hd.ID_NhanVien,
						hd.NgayLapHoaDon,
						hd.ID_PhieuTiepNhan,
    					ctdl.ID_DonViQuiDoi,
						ctdl.ID_LoHang,
						ctdl.GhiChu,					 
    						case when hd.LoaiHoaDon= 25 then 11 -- xuatkho sc
						else
    					Case when ctdl.ID_ChiTietGoiDV is not null
							then case when hd.ID_HoaDon is null then 2 else 3 end
						when ctdl.ID_ChiTietGoiDV is null and ctdl.ID_ChiTietDinhLuong is not null then 3 
						else case when hd.LoaiHoaDon = 8 then 11 else hd.LoaiHoaDon end end end as LoaiHoaDon, 	
    			
						ctmua.ID_DonViQuiDoi as ID_DichVu,		
						ISNULL(ctmua.SoLuong,0) AS SoLuongDichVu,
						ISNULL(ctmua.SoLuong,0)* ctmua.GiaVon AS GiaTriDichVu,

    					iif(hd.LoaiHoaDon=25,isnull(xkhdsc.SoLuongXuat,0), ISNULL(ctdl.SoLuong,0)) AS SoLuongXuat,
						iif(hd.LoaiHoaDon= 25,isnull(xkhdsc.GiaTriXuat,0), ISNULL(ctdl.SoLuong,0) * ctdl.GiaVon) AS GiaTriXuat,

						iif(hd.LoaiHoaDon=25,isnull(ctdl.SoLuong,0),ISNULL(ctdl.SoLuongDinhLuong_BanDau,0)) AS SoLuongDinhLuongBanDau,
						iif(hd.LoaiHoaDon=25,isnull(ctdl.SoLuong,0),ISNULL(ctdl.SoLuongDinhLuong_BanDau,0)) * ctdl.GiaVon AS GiaTriDinhLuongBanDau,
						0 as LaDinhLuongBoSung

					from BH_HoaDon_ChiTiet ctdl
					join BH_HoaDon hd on ctdl.ID_HoaDon = hd.ID 
					left join BH_HoaDon_ChiTiet ctmua on ctmua.ID = ctdl.ID_ChiTietDinhLuong
					left join
					(
						select 
						hd.ID_HoaDon,
						ctxk.ID_ChiTietGoiDV,
						ctxk.ID_DonViQuiDoi,
						max(isnull(ctxk.GhiChu,'')) as GhiChu,					
						sum(ISNULL(ctxk.SoLuong,0)) AS SoLuongXuat,
						sum(ISNULL(ctxk.SoLuong,0)* ctxk.GiaVon) AS GiaTriXuat				
						from BH_HoaDon_ChiTiet ctxk
						join BH_HoaDon hd on ctxk.ID_HoaDon = hd.ID 
						where hd.ChoThanhToan='0' 
						and hd.LoaiHoaDon = 8
						group by hd.ID_HoaDon, ctxk.ID_ChiTietGoiDV,  ctxk.ID_DonViQuiDoi
					) xkhdsc on hd.ID= xkhdsc.ID_HoaDon and xkhdsc.ID_ChiTietGoiDV= ctdl.ID --and xkhdsc.ID_DonViQuiDoi = ctdl.ID_DonViQuiDoi
					where hd.ChoThanhToan='0' 
					and hd.LoaiHoaDon in ( 1,25)
					and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
					AND ctdl.ID_ChiTietDinhLuong is not null -- thành phần định lượng
					AND ctdl.ID_ChiTietDinhLuong != ctdl.ID		

					---- get dinhluong them vao khi tao phieu xuatkho sua chua (ID_ChiTietGoiDV la dichvu)
							union all					

							select 
								hdm.MaHoaDon,
								hdm.ID_DonVi,
								hdm.ID_NhanVien,
								hdm.NgayLapHoaDon,
								hdm.ID_PhieuTiepNhan,
								ctxkThem.ID_DonViQuiDoi,
								ctxkThem.ID_LoHang,
								ctxkThem.GhiChu,					 
    							11 as LoaiHoaDon,
    			
								ctm.ID_DonViQuiDoi as ID_DichVu,		
								ISNULL(ctm.SoLuong,0) AS SoLuongDichVu,
								ISNULL(ctm.SoLuong,0)* ctm.GiaVon AS GiaTriDichVu,

    							isnull(ctxkThem.SoLuong,0) AS SoLuongXuat,
								isnull(ctxkThem.SoLuong * ctxkThem.GiaVon,0) AS GiaTriXuat,

								0 AS SoLuongDinhLuongBanDau,
								0 AS GiaTriDinhLuongBanDau,
								1 as LaDinhLuongBoSung
				
							from BH_HoaDon_ChiTiet ctm
							join BH_HoaDon hdm on ctm.ID_HoaDon= hdm.ID
							left join BH_HoaDon_ChiTiet ctxkThem on ctm.ID = ctxkThem.ID_ChiTietGoiDV
							left join BH_HoaDon hdxk on ctxkThem.ID_HoaDon= hdxk.ID and hdm.ID= hdxk.ID_HoaDon
							where hdm.LoaiHoaDon= 25 and hdxk.LoaiHoaDon= 8
							and hdxk.ChoThanhToan='0'
							and hdm.ChoThanhToan='0'
							and ctm.ID = ctm.ID_ChiTietDinhLuong --- chi get dichvu			
							and hdm.NgayLapHoaDon >= @timeStart and hdm.NgayLapHoaDon < @timeEnd
							and exists (select id from @tblIdDonVi dv2 where hdm.ID_DonVi= dv2.ID)
				) a group by
						a.MaHoaDon,
						a.ID_DonVi,
						a.ID_NhanVien,
						a.NgayLapHoaDon,
						a.ID_PhieuTiepNhan,
						a.LoaiHoaDon,
						a.ID_DichVu, a.SoLuongDichVu, a.GiaTriDichVu ,
						a.ID_DonViQuiDoi, a.ID_LoHang
		) ctsd
		join DonViQuiDoi qddl on ctsd.ID_DonViQuiDoi = qddl.ID
		) ctsdQD
		left join DonViQuiDoi qddv on ctsdQD.ID_DichVu = qddv.ID
		left join DonViQuiDoi qddl on ctsdQD.ID_HangHoa = qddl.ID_HangHoa and qddl.LaDonViChuan= 1
		left join DM_DonVi dv on ctsdQD.ID_DonVi = dv.ID
		left join DM_HangHoa hhdl on qddl.ID_HangHoa = hhdl.ID
		left join DM_HangHoa hhdv on qddv.ID_HangHoa = hhdv.ID 
		left join DM_NhomHangHoa nhomHH on hhdl.ID_NhomHang= nhomHH.ID
		left join DM_NhomHangHoa nhomDV on hhdv.ID_NhomHang= nhomDV.ID
		left join NS_NhanVien nv on ctsdQD.ID_NhanVien= nv.ID	
		left join Gara_PhieuTiepNhan tn on ctsdQD.ID_PhieuTiepNhan= tn.ID
		left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
    	where exists (select Name from splitstring(@LoaiChungTu) ct where ctsdQD.LoaiHoaDon = ct.Name ) 				
    		and
			hhdv.TheoDoi like @TheoDoi			
			and qddv.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhomDV.ID = allnhh.ID )
			and exists (select id from @tblIdDonVi dv2 where dv.ID= dv2.ID)
			and
			 ((select count(Name) from @tblSearchString b where 
    			ctsdQD.MaHoaDon like '%'+b.Name+'%' 
				or ctsdQD.GhiChu like '%'+b.Name+'%'
    			or qddl.MaHangHoa like '%'+b.Name+'%' 
    			or hhdv.TenHangHoa like '%'+b.Name+'%'
    			or hhdv.TenHangHoa_KhongDau like '%' +b.Name +'%' 
				or hhdl.TenHangHoa like '%'+b.Name+'%'
    			or hhdl.TenHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhomDV.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhomHH.TenNhomHangHoa like '%'+b.Name+'%'
    			or qddv.MaHangHoa like '%'+b.Name+'%' 
    			or TenNhanVien like '%'+b.Name+'%'    
				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'    
				or xe.BienSo like '%'+b.Name+'%' 
				)=@count or @count=0)
		ORDER BY NgayLapHoaDon DESC, qddv.MaHangHoa
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhapHang_NhomHang]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
set nocount on;
		

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

			DECLARE @tblSearchString TABLE (Name [nvarchar](max));
			DECLARE @count int;
			INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
			Select @count =  (Select count(*) from @tblSearchString);

		SELECT 
    		Max(a.TenNhomHangHoa) as TenNhomHangHoa,
			sum(a.TienThue) as TienThue,
    		CAST(ROUND((SUM(a.SoLuong)), 3) as float) as SoLuongNhap, 
    		Case When @XemGiaVon = '1' then CAST(ROUND((Sum(a.ThanhTien)), 0) as float) else 0 end as ThanhTien,
    		Case When @XemGiaVon = '1' then  CAST(ROUND((Sum(a.GiamGiaHD)), 0) as float) else 0 end as GiamGiaHD,
    		Case When @XemGiaVon = '1' then  CAST(ROUND((Sum(a.ThanhTien - a.GiamGiaHD)), 0) as float) else 0 end as GiaTriNhap
    	FROM
    	(
    		Select
    		nhh.TenNhomHangHoa as TenNhomHangHoa,
			hh.ID_NhomHang,
    		hdct.SoLuong,
			hdct.SoLuong * hdct.TienThue as TienThue,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where hd.NgayLapHoaDon between @timeStart and  @timeEnd
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon = 4
			and dvqd.Xoa like @TrangThai
    		and hh.TheoDoi like @TheoDoi
			and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where hh.ID_NhomHang= allnhh.ID)) 
			 AND
				((select count(Name) from @tblSearchString b where 
    					nhh.TenNhomHangHoa like '%'+b.Name+'%' 
    					or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'    							
    					)=@count or @count=0)	
    	) a
    	Group by a.ID_NhomHang
    	OrDER BY GiaTriNhap DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhapHang_TheoNhaCungCap]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomNCC [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

			DECLARE @tblSearchString TABLE (Name [nvarchar](max));
			DECLARE @count int;
			INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
			Select @count =  (Select count(*) from @tblSearchString);

    SELECT b.NhomKhachHang,
    	b.MaNhaCungCap,
    	b.TenNhaCungCap,
    	b.DienThoai,
    	b.SoLuongNhap, b.ThanhTien, b.GiamGiaHD, b.GiaTriNhap, b.TienThue
    	 FROM
    	(
    		SELECT
    			Case When DoiTuong_Nhom.ID_NhomDoiTuong is null then '30000000-0000-0000-0000-000000000003' else DoiTuong_Nhom.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			Case when DoiTuong_Nhom.TenNhomDT is not null then DoiTuong_Nhom.TenNhomDT else N'Nhóm mặc định' end as NhomKhachHang,
    			dt.MaDoiTuong AS MaNhaCungCap,
    			dt.TenDoiTuong AS TenNhaCungCap,
    			Case when dt.DienThoai is null then '' else dt.DienThoai end AS DienThoai,
    			a.SoLuongNhap,
    			a.ThanhTien,    				
    			a.GiamGiaHD,
    			a.GiaTriNhap,
				a.TienThue
    		FROM
    		(
    			SELECT
    				NCC.ID_NhaCungCap,
					sum(TienThue) as TienThue,
    				CAST(ROUND((SUM(NCC.SoLuong)), 3) as float) as SoLuongNhap, 
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.ThanhTien)), 0) as float) else 0 end as ThanhTien,
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.GiamGiaHD)), 0) as float) else 0 end as GiamGiaHD,
    				Case When @XemGiaVon = '1' then CAST(ROUND((Sum(NCC.ThanhTien - NCC.GiamGiaHD)), 0) as float) else 0 end as GiaTriNhap
    			FROM
    			(
    				SELECT
    				hd.ID_DoiTuong as ID_NhaCungCap,
					hdct.TienThue * hdct.SoLuong as TienThue,
    				ISNULL(hdct.SoLuong, 0) AS SoLuong,
    				ISNULL(hdct.ThanhTien, 0) AS ThanhTien,
					Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang) end as GiamGiaHD
    				FROM
    				BH_HoaDon hd 
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on dvqd.ID = hdct.ID_DonViQuiDoi
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    				and hd.ChoThanhToan = 0
    				and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    				and hd.LoaiHoaDon = 4
    				and hh.TheoDoi like @TheoDoi
					and dvqd.Xoa like @TrangThai
					and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where hh.ID_NhomHang= allnhh.ID))
					AND
						((select count(Name) from @tblSearchString b where 
    							dt.MaDoiTuong like '%'+b.Name+'%' 
    							or dt.TenDoiTuong like '%'+b.Name+'%' 
    							or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    							or dt.DienThoai like '%' +b.Name +'%' 
    							)=@count or @count=0)								
    			) AS NCC
    			Group by NCC.ID_NhaCungCap
    		) a
    		left join DM_DoiTuong dt on a.ID_NhaCungCap = dt.ID
    		Left join (Select Main.ID as ID_DoiTuong,
    					Left(Main.dt_n,Len(Main.dt_n)-1) As TenNhomDT,
    					Left(Main.id_n,Len(Main.id_n)-1) As ID_NhomDoiTuong
    					From
    					(
    					Select distinct hh_tt.ID,
    					(
    						Select ndt.TenNhomDoiTuong + ', ' AS [text()]
    						From dbo.DM_DoiTuong_Nhom dtn
    						inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    						Where dtn.ID_DoiTuong = hh_tt.ID
    						order by ndt.TenNhomDoiTuong
    						For XML PATH ('')
    					) dt_n,
    					(
    					Select convert(nvarchar(max), ndt.ID)  + ', ' AS [text()]
    					From dbo.DM_DoiTuong_Nhom dtn
    					inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    					Where dtn.ID_DoiTuong = hh_tt.ID
    					order by ndt.TenNhomDoiTuong
    					For XML PATH ('')
    					) id_n
    					From dbo.DM_DoiTuong hh_tt
    					) Main) as DoiTuong_Nhom on dt.ID = DoiTuong_Nhom.ID_DoiTuong
    	) b
    	where (@ID_NhomNCC ='' or b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomNCC)))
    	ORDER BY GiaTriNhap DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhapHang_TraHangNhap]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	set nocount on;
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

			DECLARE @tblSearchString TABLE (Name [nvarchar](max));
			DECLARE @count int;
			INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
			Select @count =  (Select count(*) from @tblSearchString);
    	
    SELECT 
    	a.MaChungTuGoc, 
    	a.MaChungTu, 
    	a.NgayLapHoaDon,
    	a.MaHangHoa,
    	a.TenHangHoaFull,
    	a.TenHangHoa,
    	a.TenDonViTinh,
    	a.ThuocTinh_GiaTri,
    	a.TenLoHang,
    	CAST(ROUND(a.SoLuongTra, 3) as float) as SoLuong,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.ThanhTien, 0) as float) else 0 end as ThanhTien,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.GiamGiaHD, 0) as float) else 0 end as GiamGiaHD,
    	Case When @XemGiaVon = '1' then CAST(ROUND(a.ThanhTien -a.GiamGiaHD, 0) as float) else 0 end as GiaTriTra,
    		a.TenNhanVien
    	FROM
    	(
    		SELECT
    		Case when hdb.ID is null then N'HD trả nhanh' else hdb.MaHoaDon end as MaChungTuGoc,
    		hdt.MaHoaDon as MaChungTu,
    		hdt.NgayLapHoaDon,
    		dvqd.MaHangHoa,
    		hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
    		hh.TenHangHoa,
    			dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		lh.MaLoHang  as TenLoHang,
    		hdct.SoLuong as SoLuongTra,
			Case when hdt.TongTienHang = 0 then 0 else hdct.ThanhTien * (hdt.TongGiamGia / hdt.TongTienHang) end as GiamGiaHD,
    		hdct.ThanhTien,
    		nv.TenNhanVien
    		FROM
    		BH_HoaDon hdt 
    		left join BH_HoaDon hdb on hdt.ID_HoaDon = hdb.ID
    		join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon 
    		left join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join NS_NhanVien nv on hdt.ID_NhanVien = nv.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID    			
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdt.ChoThanhToan = 0
    		and hdt.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdt.LoaiHoaDon = 7    		
    		and hh.TheoDoi like @TheoDoi
			and dvqd.Xoa like @TrangThai
			and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where hh.ID_NhomHang= allnhh.ID))
			AND
						((select count(Name) from @tblSearchString b where 
    							dvqd.MaHangHoa like '%'+b.Name+'%' 
    							or hh.TenHangHoa like '%'+b.Name+'%' 
    							or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    							or hdt.MaHoaDon like '%' +b.Name +'%' 
    							)=@count or @count=0)	
    		
    	) a    	
    	ORDER BY a.NgayLapHoaDon DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_CongNo_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    		IF @timeChotSo != null
    		BEGIN
    			IF @timeChotSo < @timeStart
    			BEGIN
    		 SELECT 
    				MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    			MAX(b.MaKhachHang) as MaDoiTac,
    			MAX(b.TenKhachHang) as TenDoiTac,
    			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    			MAX(b.TongTienChi) as TongTienChi, 
    			MAX(b.TongTienThu) as TongTienThu,
    			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    		  dt.MaDoiTuong AS MaKhachHang, 
    		  dt.TenDoiTuong AS TenKhachHang,
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    		  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    		SUM(HangHoa.GhiNo) as GhiNo,
    		SUM(HangHoa.GhiCo) as GhiCo
    		FROM
    		(
    			SELECT
    				td.ID_DoiTuong AS ID_KhachHang,
    				SUM(td.CongNo) + SUM(td.DoanhThu) + SUM(td.TienChi) - SUM(td.TienThu) - SUM(td.GiaTriTra) AS NoDauKy,
    				0 AS GhiNo,
    				0 AS GhiCo
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    					ID_KhachHang As ID_DoiTuong,
    					CongNo,
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
    				SUM(bhd.TongThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
				--chi phí hóa đơn
					UNION ALL
						SELECT 
    						cp.ID_NhaCungCap,
							0 AS CongNo,
    						SUM(cp.ThanhTien) AS GiaTriTra,
    						0 AS DoanhThu,
    						0 AS TienThu,
    						0 AS TienChi
    					FROM BH_HoaDon bhd
						INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    					WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    					AND bhd.ID_DonVi = @ID_ChiNhanh
    						AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    					GROUP BY cp.ID_NhaCungCap
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    			SELECT
    				pstv.ID_DoiTuong AS ID_KhachHang,
    				0 AS NoDauKy,
    				SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    				SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo
    			FROM
    			(
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.TongThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25)  AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
				--Chi phí hóa đơn sửa chữa
				UNION ALL
				SELECT 
    				cp.ID_NhaCungCap,
    				0 AS GiaTriTra,
    				SUM(cp.ThanhTien) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
				INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    			WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY cp.ID_NhaCungCap
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    		)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi='0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    				LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		ORDER BY MAX(b.MaKhachHang) DESC
    			END
    			ELSE IF @timeChotSo > @timeEnd
    			BEGIN
    				SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
    			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
    			MAX(b.TongTienThu) as TongTienThu,
    			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    		SUM(HangHoa.GhiNo) as GhiNo,
    		SUM(HangHoa.GhiCo) as GhiCo,
    		SUM(HangHoa.NoCuoiKy) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    			0 AS GhiNo,
    			0 AS GhiCo,
    			SUM(td.CongNo) - SUM(td.DoanhThu) - SUM(td.TienChi) + SUM(td.TienThu) + SUM(td.GiaTriTra) AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    			SELECT 
    			ID_KhachHang As ID_DoiTuong,
    			CongNo,
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
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    			SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    			SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    			SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo,
    			0 AS NoCuoiKy
    			FROM
    			(
    			SELECT 
    			bhd.ID_DoiTuong,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi = '0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
    			END
    			ELSE
    			BEGIN
    			SELECT 
    			 MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
    			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
    			MAX(b.TongTienThu) as TongTienThu,
    			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo,0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    			SUM(HangHoa.GhiNo) as GhiNo,
    			SUM(HangHoa.GhiCo) as GhiCo,
    			SUM(HangHoa.NoCuoiKy) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(td.CongNo) - SUM(td.DoanhThu) - SUM(td.TienChi) + SUM(td.TienThu) + SUM(td.GiaTriTra) AS NoDauKy,
    				SUM(td.DoanhThu) + SUM(td.TienChi) AS GhiNo,
    				SUM(td.TienThu) + SUM(td.GiaTriTra) AS GhiCo,
    				0 AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				CongNo AS CongNo,
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
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    				UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    				SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    				SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    				SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo,
    			    SUM(pstv.CongNo) + SUM(pstv.DoanhThu) + SUM(pstv.TienChi) - SUM(pstv.TienThu) - SUM(pstv.GiaTriTra) AS NoCuoiKy
    			FROM
    			(
    				-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				CongNo AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    
    			SELECT 
    			bhd.ID_DoiTuong,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi ='0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
    			END
    		END
    		ELSE
    		BEGIN
    			SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
    		Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    		Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
    		MAX(b.TongTienThu) as TongTienThu,
    		Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    		Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT 
    			  a.ID_KhachHang, 
    		  dt.MaDoiTuong AS MaKhachHang, 
    		  dt.TenDoiTuong AS TenKhachHang,
    		  a.NoDauKy,
    		 --a.GhiNo As TongTienChi,
    		 -- a.GhiCo As TongTienThu,
    			  iif(a.GhiNo<= 0, iif(a.GhiCo < 0, -a.GhiCo, 0 ), a.GhiNo) as TongTienChi,
    			  iif(a.GhiCo <=0, iif(a.GhiNo < 0, -a.GhiNo, 0 ), a.GhiCo) as TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    		  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    			SUM(HangHoa.NoDauKy) as NoDauKy, 
    			SUM(HangHoa.GhiNo) as GhiNo,
    			SUM(HangHoa.GhiCo) as GhiCo,
    			SUM(HangHoa.NoDauKy + HangHoa.GhiNo - HangHoa.GhiCo) as NoCuoiKy
    		FROM
    		(
    			SELECT
    				td.ID_DoiTuong AS ID_KhachHang,
    				SUM(td.DoanhThu) + SUM(td.TienChi) - SUM(td.TienThu) - SUM(td.GiaTriTra) AS NoDauKy,
    				0 AS GhiNo,
    				0 AS GhiCo,
    				0 AS NoCuoiKy
    			FROM
    			(
    
    				---- CÔNG NỢ ĐẦU KỲ
    				---- doanhthu khachhang
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
					--chi phí hóa đơn
					UNION ALL
						SELECT 
    						cp.ID_NhaCungCap,
    						SUM(cp.ThanhTien) AS GiaTriTra,
    						0 AS DoanhThu,
    						0 AS TienThu,
    						0 AS TienChi
    					FROM BH_HoaDon bhd
						INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    					WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeStart
    					AND bhd.ID_DonVi = @ID_ChiNhanh
    						AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    					GROUP BY cp.ID_NhaCungCap
    				union all
    				---- doanhthu baohiem
    			SELECT 
    				bhd.ID_BaoHiem,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToanBaoHiem) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon in (1,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				and bhd.ID_BaoHiem is not null
    			GROUP BY bhd.ID_BaoHiem
    
    			-- trahang of khachhag
    			UNION All
    			SELECT bhd.ID_DoiTuong,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (4,6) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
    
    			-- thucthu khachhang + baohiem + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
				AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND qhdct.ID_DoiTuong is not null
    			
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    				-- phieuchi khachhang + ncc+ baohiem
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon    		
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
				AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_NhanVien is null
					AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND qhdct.ID_DoiTuong is not null
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn (---- CÔNG NỢ TRONG KỲ  ------)
    			SELECT
    				pstv.ID_DoiTuong AS ID_KhachHang,
    				0 AS NoDauKy,
    					SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiCo,
    				SUM(pstv.GiaTriTra) + SUM(pstv.TienThu) AS GhiNo,
    			
    				0 AS NoCuoiKy
    			FROM
    			(
    				-- KhachHang: doanh thu
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
				--chi phí hóa đơn
				UNION ALL
					SELECT 
    					cp.ID_NhaCungCap,
    					SUM(cp.ThanhTien) AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi
    				FROM BH_HoaDon bhd
					INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    				WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				GROUP BY cp.ID_NhaCungCap
    				union all
    				---- doanhthu baohiem
    			SELECT 
    				bhd.ID_BaoHiem,
    					0 AS GiaTriTra,    				
    				sum( bhd.PhaiThanhToanBaoHiem) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon in (1,25) AND bhd.ChoThanhToan = '0'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				and bhd.ID_BaoHiem is not null
    			GROUP BY bhd.ID_BaoHiem
    
    			-- khachhang: trahang
    			UNION All
    			SELECT bhd.ID_DoiTuong,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
    
    			--  phieuthu: kh + bh + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd		
    			AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
				AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					AND qhdct.ID_NhanVien is null
    				AND qhdct.ID_DoiTuong is not null
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    					-- phieuchi: kh + bh + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' 
				AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
				AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					AND qhdct.ID_NhanVien is null
    				AND qhdct.ID_DoiTuong is not null
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    				left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where  dt.TheoDoi ='0' 
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy	
    			ORDER BY MAX(b.MaKhachHang) DESC
    		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_SoQuy_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit],
    @LoaiTien [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    --	tinh ton dau ky
    	Declare @TonDauKy float
    	Set @TonDauKy = (Select
    	CAST(ROUND(SUM(TienThu - TienChi), 0) as float) as TonDauKy
    	FROM
    	(
    		select 
    			case when qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as TienThu,
    			Case when qhd.LoaiHoaDon = 12 then qhdct.TienThu else 0 end as TienChi,
    			Case when qhdct.TienMat > 0 and qhdct.TienGui = 0 then '1' 
    			when qhdct.TienGui > 0 and qhdct.TienMat = 0 then '2'
    			when qhdct.TienGui > 0 and qhdct.TienMat > 0 then '12' else '' end as LoaiTien,
    				qhd.HachToanKinhDoanh as HachToanKinhDoanh
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		where qhd.NgayLapHoaDon < @timeStart
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and qhdct.HinhThucThanhToan not in (4,5,6)
    		) a 
    		where LoaiTien like @LoaiTien
    			and (HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    	) 
    		
    	if (@TonDauKy is null)
    	BeGin
    		Set @TonDauKy = 0;
    	END
    	Declare @tmp table (ID_HoaDon UNIQUEIDENTIFIER,MaPhieuThu nvarchar(max), NgayLapHoaDon datetime, KhoanMuc nvarchar(max), TenDoiTac nvarchar(max),
    	TienMat float, TienGui float, TienThu float, TienChi float, ThuTienMat float, ChiTienMat float, ThuTienGui float, ChiTienGui float, TonLuyKeTienMat float,TonLuyKeTienGui float,TonLuyKe float, SoTaiKhoan nvarchar(max), NganHang nvarchar(max), GhiChu nvarchar(max),
    		IDDonVi UNIQUEIDENTIFIER, TenDonVi NVARCHAR(MAX));
    	Insert INTO @tmp
    		 SELECT 
    				b.ID_HoaDon,
    				b.MaPhieuThu as MaPhieuThu,
    			b.NgayLapHoaDon as NgayLapHoaDon,
    				MAX(b.NoiDungThuChi) as KhoanMuc,
    			MAX(b.TenNguoiNop) as TenDoiTac, 
    			SUM (b.TienMat) as TienMat,
    			SUM (b.TienGui) as TienGui,
    			SUM (b.TienThu) as TienThu,
    			SUM (b.TienChi) as TienChi,
    			SUM (b.ThuTienMat) as ThuTienMat,
    			SUM (b.ChiTienMat) as ChiTienMat, 
    			SUM (b.ThuTienGui) as ThuTienGui,
    			SUM (b.ChiTienGui) as ChiTienGui, 
    				0 as TonLuyKe,
    			0 as TonLuyKeTienMat,
    			0 as TonLuyKeTienGui,
    			MAX(b.SoTaiKhoan) as SoTaiKhoan,
    			MAX(b.NganHang) as NganHang,
    			MAX(b.GhiChu) as GhiChu,
    				dv.ID,
    				dv.TenDonVi
    		FROM
    		(
    				select 
    			a.HachToanKinhDoanh,
    			a.ID_DoiTuong,
    			a.ID_HoaDon,
    			a.MaHoaDon,
    			a.MaPhieuThu,
    			a.NgayLapHoaDon,
    			a.TenNguoiNop,
    			a.TienMat,
    			a.TienGui,
    			case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
    			Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat,
    			case when a.LoaiHoaDon = 11 then a.TienThu else 0 end as TienThu,
    			Case when a.LoaiHoaDon = 12 then a.TienThu else 0 end as TienChi,
    			a.NoiDungThuChi,
    			a.NganHang,
    			a.SoTaiKhoan,
    			a.GhiChu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' else '' end  as LoaiTien,
    				a.ID_DonVi
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			MAX (tknh.SoTaiKhoan) as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else 4 end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    			max(IIF(qhdct.HinhThucThanhToan = 1, qhdct.TienThu, 0)) as TienMat,
    			max(IIF(qhdct.HinhThucThanhToan IN (2,3) , qhdct.TienThu, 0)) as TienGui,
    			max(qhdct.TienThu) as TienThu,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
    				qhd.ID_DonVi
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    				and qhdct.HinhThucThanhToan not in (4,5,6)
    				AND ((select count(Name) from @tblSearch b where     			
    			dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or qhd.MaHoaDon like '%' + b.Name + '%'
    				or qhd.NguoiNopTien like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau,qhdct.ID_NhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, qhd.ID_DonVi, qhdct.ID, qhdct.HinhThucThanhToan
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    			inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    			where LoaiTien like @LoaiTien
    		Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaPhieuThu, b.NgayLapHoaDon, dv.TenDonVi, dv.ID
    		ORDER BY NgayLapHoaDon
    -- tính tồn lũy kế
    	    IF (EXISTS (select * from @tmp))
    		BEGIN
    			DECLARE @Ton float;
    			SET @Ton = @TonDauKy;
    			DECLARE @TonTienMat float;
    			SET @TonTienMat = @TonDauKy;
    			DECLARE @TonTienGui float;
    			SET @TonTienGui = @TonDauKy;
    			
    			DECLARE @TienThu float;
    			DECLARE @TienChi float;
    			DECLARE @ThuTienMat float;
    			DECLARE @ChiTienMat float;
    			DECLARE @ThuTienGui float;
    			DECLARE @ChiTienGui float;
    			DECLARE @TonLuyKe float;
    				DECLARE @ID_HoaDon UNIQUEIDENTIFIER;
    	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT TienThu, TienChi, ThuTienGui, ThuTienMat, ChiTienGui, ChiTienMat, ID_HoaDon FROM @tmp ORDER BY NgayLapHoaDon
    	OPEN CS_ItemUpDate 
    FETCH FIRST FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon
    WHILE @@FETCH_STATUS = 0
    BEGIN
    	SET @Ton = @Ton + @TienThu - @TienChi;
    	SET @TonTienMat = @TonTienMat + @ThuTienMat - @ChiTienMat;
    	SET @TonTienGui = @TonTienGui + @ThuTienGui - @ChiTienGui;
    	UPDATE @tmp SET TonLuyKe = @Ton, TonLuyKeTienMat = @TonTienMat, TonLuyKeTienGui = @TonTienGui WHERE ID_HoaDon = @ID_HoaDon
    	FETCH NEXT FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon
    END
    CLOSE CS_ItemUpDate
    DEALLOCATE CS_ItemUpDate
    	END
    	ELSE
    	BEGIN
    		Insert INTO @tmp
    	SELECT '00000000-0000-0000-0000-000000000000', 'TRINH0001', '1989-04-07','','','0','0','0','0','0','0','0','0', @TonDauKy, @TonDauKy, @TonDauKy, '','','', '00000000-0000-0000-0000-000000000000', ''
    	END
    	Select 
    		ID_HoaDon,
    	MaPhieuThu,
    	NgayLapHoaDon,
    	KhoanMuc,
    	TenDoiTac,
    	@TonDauKy as TonDauKy,
    	TienMat,
    	TienGui,
    	TienThu,
    	TienChi,
    	ThuTienMat,
    	ChiTienMat,
    	ThuTienGui,
    	ChiTienGui,
    	TonLuyKe,
    	TonLuyKeTienMat,
    	TonLuyKeTienGui,
    	SoTaiKhoan, 
    	NganHang, 
    	GhiChu,
    		IDDonVi, TenDonVi
    	 from @tmp order by NgayLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_NhatKyHoatDong]
    @ID_DonVi [uniqueidentifier],
	@TongQuan_XemDS_PhongBan [varchar](max),
	@TongQuan_XemDS_HeThong [varchar](max),
	@ID_NguoiDung [uniqueidentifier]
AS
BEGIN

set nocount on;
	DECLARE @LaAdmin as nvarchar
    Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd where nd.ID = @ID_NguoiDung)
	If (@LaAdmin = 1)
	BEGIN
		SELECT TOP(12)
		MAX(a.TenNhanVien) as TenNhanVien,
		a.MaHoaDon,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien,
		MAX(a.NgayLapHoaDon) as NgayGoc,
    	CONVERT(VARCHAR, MAX(a.NgayLapHoaDon), 22) as NgayLapHoaDon,
		CASE 
			WHEN a.LoaiHoaDon = 1 then N'bán đơn hàng'  
			WHEN a.LoaiHoaDon = 3 then N'tạo báo giá' 
			WHEN a.LoaiHoaDon = 4 then N'nhập kho đơn hàng'  
			WHEN a.LoaiHoaDon = 6 then N'nhận hàng trả'  
			WHEN a.LoaiHoaDon = 7 then N'trả hàng nhà cung cấp'  
			WHEN a.LoaiHoaDon = 8 then N'xuất kho đơn hàng'  
			WHEN a.LoaiHoaDon = 25 then N'tạo hóa đơn sửa chữa'  
			Else N'bán gói dịch vụ'
		END as TenLoaiChungTu,
		a.LoaiHoaDon AS LoaiHoaDon
		FROM
		(
    		SELECT
			hdb.ID as ID_HoaDon,
			hdb.MaHoaDon,
			nv.TenNhanVien,
			hdb.LoaiHoaDon,
			hdb.NgayLapHoaDon,
    		isnull(hdb.PhaiThanhToan, hdb.TongThanhToan) as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join NS_NhanVien nv on hdb.ID_NhanVien = nv.ID
    		where hdb.ID_DonVi = @ID_DonVi
    		and hdb.ChoThanhToan = 0
    		and hdb.LoaiHoaDon in (1,3,4,5,6,7,8,19, 25)
		) a
    	GROUP BY a.ID_HoaDon, a.LoaiHoaDon, a.MaHoaDon
		ORDER BY NgayGoc DESC
	END
	ELSE
	BEGIN
		if (@TongQuan_XemDS_HeThong = 'TongQuan_XemDS_HeThong')
		BEGIN
			SELECT TOP(12)
			MAX(a.TenNhanVien) as TenNhanVien,
			a.MaHoaDon,
			CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien,
			MAX(a.NgayLapHoaDon) as NgayGoc,
    		CONVERT(VARCHAR, MAX(a.NgayLapHoaDon), 22) as NgayLapHoaDon,
			CASE WHEN a.LoaiHoaDon = 1 then N'bán đơn hàng'  
			WHEN a.LoaiHoaDon = 3 then N'tạo báo giá' 
			WHEN a.LoaiHoaDon = 4 then N'nhập kho đơn hàng'  
			WHEN a.LoaiHoaDon = 6 then N'nhận hàng trả'  
			WHEN a.LoaiHoaDon = 7 then N'trả hàng nhà cung cấp'  
			WHEN a.LoaiHoaDon = 8 then N'xuất kho đơn hàng'  
			WHEN a.LoaiHoaDon = 25 then N'tạo hóa đơn sửa chữa'  
			Else N'bán gói dịch vụ'
			END as TenLoaiChungTu,
			a.LoaiHoaDon AS LoaiHoaDon
			FROM
			(
    			SELECT
				hdb.ID as ID_HoaDon,
				hdb.MaHoaDon,
				nv.TenNhanVien,
				hdb.LoaiHoaDon,
				hdb.NgayLapHoaDon,
    			isnull(hdb.PhaiThanhToan, hdb.TongThanhToan) as ThanhTien
    			FROM
    			BH_HoaDon hdb
    			
				join NS_NhanVien nv on hdb.ID_NhanVien = nv.ID
    			where hdb.ID_DonVi = @ID_DonVi
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon in (1,3,4,5,6,7,8,19, 25)
			) a
    		GROUP BY a.ID_HoaDon, a.LoaiHoaDon, a.MaHoaDon
			ORDER BY NgayGoc DESC
		END 
		ELSE 
		BEGIN
			if (@TongQuan_XemDS_PhongBan = 'TongQuan_XemDS_PhongBan')
			BEGIN
				DECLARE @ID_NhanVienPhongBan table (ID_NhanVien uniqueidentifier);
				INSERT INTO @ID_NhanVienPhongBan exec getListID_NhanVienPhongBan @ID_NguoiDung;
				SELECT TOP(12)
				MAX(a.TenNhanVien) as TenNhanVien,
				a.MaHoaDon,
				CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien,
				MAX(a.NgayLapHoaDon) as NgayGoc,
    			CONVERT(VARCHAR, MAX(a.NgayLapHoaDon), 22) as NgayLapHoaDon,
				CASE WHEN a.LoaiHoaDon = 1 then N'bán đơn hàng'  
				WHEN a.LoaiHoaDon = 3 then N'tạo báo giá' 
				WHEN a.LoaiHoaDon = 4 then N'nhập kho đơn hàng'  
				WHEN a.LoaiHoaDon = 6 then N'nhận hàng trả'  
				WHEN a.LoaiHoaDon = 7 then N'trả hàng nhà cung cấp'  
				WHEN a.LoaiHoaDon = 8 then N'xuất kho đơn hàng'  
				WHEN a.LoaiHoaDon = 25 then N'tạo hóa đơn sửa chữa'  
				Else N'bán gói dịch vụ'
				END as TenLoaiChungTu,
				a.LoaiHoaDon AS LoaiHoaDon
				FROM
				(
    				SELECT
					hdb.ID as ID_HoaDon,
					hdb.MaHoaDon,
					nv.TenNhanVien,
					hdb.LoaiHoaDon,
					hdb.NgayLapHoaDon,
					isnull(hdb.PhaiThanhToan, hdb.TongThanhToan) as ThanhTien
    				FROM
    				BH_HoaDon hdb
					join NS_NhanVien nv on hdb.ID_NhanVien = nv.ID
					join @ID_NhanVienPhongBan pb on nv.ID = pb.ID_NhanVien
    				where hdb.ID_DonVi = @ID_DonVi
    				and hdb.ChoThanhToan = 0
    				and hdb.LoaiHoaDon in (1,3,4,5,6,7,8,19, 25)					
				) a
    			GROUP BY a.ID_HoaDon, a.LoaiHoaDon, a.MaHoaDon
				ORDER BY NgayGoc DESC
			END
			else 
			BEGIN
				DECLARE @ID_NhanVienDS table (ID_NhanVien uniqueidentifier);
				INSERT INTO @ID_NhanVienDS exec getListID_NhanVienDS @ID_NguoiDung;
				SELECT TOP(12)
				MAX(a.TenNhanVien) as TenNhanVien,
				a.MaHoaDon,
				CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien,
				MAX(a.NgayLapHoaDon) as NgayGoc,
    			CONVERT(VARCHAR, MAX(a.NgayLapHoaDon), 22) as NgayLapHoaDon,
				CASE WHEN a.LoaiHoaDon = 1 then N'bán đơn hàng'  
				WHEN a.LoaiHoaDon = 3 then N'tạo báo giá' 
				WHEN a.LoaiHoaDon = 4 then N'nhập kho đơn hàng'  
				WHEN a.LoaiHoaDon = 6 then N'nhận hàng trả'  
				WHEN a.LoaiHoaDon = 7 then N'trả hàng nhà cung cấp'  
				WHEN a.LoaiHoaDon = 8 then N'xuất kho đơn hàng'  
				WHEN a.LoaiHoaDon = 25 then N'tạo hóa đơn sửa chữa'  
				Else N'bán gói dịch vụ'
				END as TenLoaiChungTu,
				a.LoaiHoaDon AS LoaiHoaDon
				FROM
				(
    				SELECT
					hdb.ID as ID_HoaDon,
					hdb.MaHoaDon,
					nv.TenNhanVien,
					hdb.LoaiHoaDon,
					hdb.NgayLapHoaDon,
    				isnull(hdb.PhaiThanhToan, hdb.TongThanhToan) as ThanhTien
    				FROM
    				BH_HoaDon hdb   			
					join NS_NhanVien nv on hdb.ID_NhanVien = nv.ID
					join @ID_NhanVienDS pb on nv.ID = pb.ID_NhanVien
    				where hdb.ID_DonVi = @ID_DonVi
    				and hdb.ChoThanhToan = 0
    				and hdb.LoaiHoaDon in (1,3,4,5,6,7,8,19, 25)
				) a
    			GROUP BY a.ID_HoaDon, a.LoaiHoaDon, a.MaHoaDon
				ORDER BY NgayGoc DESC
			END
		END
	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoDuong_InsertListDetail_ByNhomHang]
    @ID_HangHoa [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @ID_NhomHangHoa uniqueidentifier, @QuanLyBaoDuong int , @LoaiBaoDuong int
    	select @ID_NhomHangHoa=  ID_NhomHang, @QuanLyBaoDuong= QuanLyBaoDuong, @LoaiBaoDuong = LoaiBaoDuong
    	from DM_HangHoa where id= @ID_HangHoa
    
    declare @tblNhom table(ID_NhomHang uniqueidentifier)
    	insert into @tblNhom
    	select ID from dbo.GetListNhomHangHoa(@ID_NhomHangHoa)
    
    	-- update quanlybaoduong for all hanghoa by nhom		
    	update hh set hh.QuanLyBaoDuong= @QuanLyBaoDuong , hh.LoaiBaoDuong= @LoaiBaoDuong
    	from DM_HangHoa hh 
    	where exists (
    	select id from @tblNhom nhom where hh.ID_NhomHang= nhom.ID_NhomHang)
		
    	---- get list hanghoa by nhomhang
    	select hh.ID, hh.TenHangHoa
    	into #temp
    	from DM_HangHoa hh
    	where hh.TheoDoi = 1
    	and hh.ID not like @ID_HangHoa
    	and exists (
    	select id from @tblNhom nhom where hh.ID_NhomHang= nhom.ID_NhomHang)
    
    	--- delete all by nhom
    	delete bd	
    	from DM_HangHoa_BaoDuongChiTiet bd
    	where exists (
    	select ID from #temp where bd.ID_HangHoa= #temp.ID
    	)
    
    	--- insert again
    	insert into DM_HangHoa_BaoDuongChiTiet
    	select NEWID(), tblhh.ID, a.LanBaoDuong, a.GiaTri, a.LoaiGiaTri, a.BaoDuongLapDinhKy
    	from #temp tblhh	
    	cross join 
    	(
    		select bd.LanBaoDuong, bd.GiaTri, bd.LoaiGiaTri, bd.BaoDuongLapDinhKy
    		from DM_HangHoa_BaoDuongChiTiet bd
    		where bd.ID_HangHoa= @ID_HangHoa
    	) a
END");

            Sql(@"ALTER PROCEDURE [dbo].[ChiTietTraHang_insertChietKhauNV]
    @ID_HoaDon [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    	insert into BH_NhanVienThucHien (ID, ID_NhanVien, ID_ChiTietHoaDon, TienChietKhau, TheoYeuCau, PT_ChietKhau, ThucHien_TuVan, 
		ID_HoaDon, TinhChietKhauTheo, HeSo, ID_QuyHoaDon, TinhHoaHongTruocCK)
    	select newid(), th.ID_NhanVien, cttra.ID, 
    	-- important: neu chiet khau theo VND --> khong nhan voi HeSo
    	case when th.PT_ChietKhau = 0 then case when ctmua.ThanhTien = 0 then 0 else (th.TienChietKhau / ctmua.ThanhTien ) * cttra.ThanhTien end
		else th.PT_ChietKhau/100 *th.HeSo * cttra.ThanhTien end as TienChietKhau,
    	th.TheoYeuCau, th.PT_ChietKhau, th.ThucHien_TuVan, null, th.TinhChietKhauTheo, th.HeSo, null, isnull(th.TinhHoaHongTruocCK,0)
    	from BH_NhanVienThucHien th
    	join BH_HoaDon_ChiTiet ctmua on th.ID_ChiTietHoaDon = ctmua.id
    	join BH_HoaDon_ChiTiet cttra on ctmua.ID= cttra.ID_ChiTietGoiDV
    	join BH_HoaDon hd on ctmua.ID_HoaDon= hd.ID
    	where hd.ID=@ID_HoaDon 
END");

            Sql(@"ALTER PROCEDURE [dbo].[CTHD_GetChiPhiDichVu]
    @IDHoaDons [nvarchar](max),
    @IDVendors [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @sql nvarchar(max) ='', @where nvarchar(max), @tblDefined nvarchar(max) ='',
    	@paramDefined nvarchar(max) ='@IDHoaDons_In nvarchar(max), @IDVendors_In nvarchar(max)'
    
    	set @where=' where 1 = 1 and (cthd.ID_ParentCombo is null or cthd.ID_ParentCombo != cthd.ID)
    	   and (cthd.ID_ChiTietDinhLuong is null or cthd.ID_ChiTietDinhLuong = cthd.ID)	'
    
    	if isnull(@IDHoaDons,'')!=''
    		begin
    			set @tblDefined = concat(@tblDefined, ' declare @tblHoaDon table (ID uniqueidentifier)
    			insert into @tblHoaDon select name from dbo.splitstring(@IDHoaDons_In)')
    			set @where = concat(@where,' and exists (select hd2.ID from @tblHoaDon hd2 where hd.ID = hd2.ID)') 
    		end
    	if isnull(@IDVendors,'')!=''
    		begin
    			set @tblDefined = concat(@tblDefined, ' declare @tblVendor table (ID uniqueidentifier)
    			insert into @tblVendor select name from dbo.splitstring(@IDVendors_In)')
    			set @where =concat(@where, ' and exists (select ncc.ID from @tblVendor ncc where cp.ID_NhaCungCap = ncc.ID)' )
    		end
    
    	set @sql= CONCAT(N'
    		select 
    			iif(cp.ID is null, ''00000000-0000-0000-0000-000000000000'',cp.ID) as ID,	
    			qd.MaHangHoa,
    			qd.TenDonViTinh,
    			cthd.ID_DonViQuiDoi,	
    			cthd.DonGia as GiaBan,
    			cp.ID_NhaCungCap,
				cp.GhiChu,
    			iif(cp.ID_HoaDon_ChiTiet is null, cthd.ID,cp.ID_HoaDon_ChiTiet) as ID_HoaDon_ChiTiet,
    			iif(cp.ID_HoaDon is null, cthd.ID_HoaDon,cp.ID_HoaDon) as ID_HoaDon,
    			dt.DienThoai,
    			dt.MaDoiTuong as MaNhaCungCap,
    			dt.TenDoiTuong as TenNhaCungCap,
    			iif(cp.SoLuong is null, cthd.SoLuong,cp.SoLuong) as SoLuong,
    			iif(cp.DonGia is null, 0,cp.DonGia) as DonGia,			
    			iif(cp.ThanhTien is null, 0,cp.ThanhTien) as ThanhTien,
    			xe.BienSo,
    			hd.ChiPhi as TongChiPhi,
    			hd.NgayLapHoaDon,
    			hd.MaHoaDon,
    			cthd.Soluong as SoLuongHoaDon, --- soluong max
    			iif(cthd.TenHangHoaThayThe is null or cthd.TenHangHoaThayThe ='''', hh.TenHangHoa, cthd.TenHangHoaThayThe) as TenHangHoaThayThe,
    			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa =''1'',1,2), hh.LoaiHangHoa) as LoaiHangHoa
    		from BH_HoaDon_ChiTiet cthd
    		join BH_HoaDon hd on cthd.ID_HoaDon= hd.ID
    		left join BH_HoaDon_ChiPhi cp on cthd.ID= cp.ID_HoaDon_ChiTiet
    		left join DonViQuiDoi qd on cthd.ID_DonViQuiDoi= qd.ID
    	   left join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    	   left join DM_DoiTuong dt on cp.ID_NhaCungCap= dt.ID
    	   left join Gara_DanhMucXe xe on hd.ID_Xe= xe.ID
    	   ', @where,
    	 ' order by qd.MaHangHoa ')
    
    	 set @sql = concat(@tblDefined, @sql)
    
    	 exec sp_executesql @sql, @paramDefined,
    		@IDHoaDons_In = @IDHoaDons,
    		@IDVendors_In = @IDVendors
END");

            Sql(@"ALTER PROCEDURE [dbo].[Gara_GetListBaoGia]
    @IDChiNhanhs [nvarchar](max),
    @FromDate [nvarchar](14),
    @ToDate [nvarchar](14),
    @ID_PhieuSuaChua [nvarchar](max), --%%
    @TrangThais [nvarchar](20), -- 0,1
    @TextSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	if @FromDate = '2016-01-01' 
    		set @ToDate= (select format(DATEADD(day,1, max(NgayLapHoaDon)),'yyyy-MM-dd') from BH_HoaDon where LoaiHoaDon= 3)
    
    	declare @tblDonVi table (ID_DonVi uniqueidentifier)
    	insert into @tblDonVi
    	select Name from dbo.splitstring(@IDChiNhanhs)
    
    	declare @tbTrangThai table (GiaTri varchar(2))
    	insert into @tbTrangThai
    	select Name from dbo.splitstring(@TrangThais)
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);	
    
    	with data_cte
    	as
    	(
    			select *
    			from
    			(
    			select hd.ID,
					hd.ID_HoaDon,
					hd.ID_DonVi,
					hd.ID_DoiTuong,
					hd.ID_NhanVien,
					hd.NgayLapHoaDon,
					hd.MaHoaDon,
					hd.LoaiHoaDon,
					hd.ChoThanhToan,
					hd.TongTienHang,
					hd.TongGiamGia,
					hd.TongChietKhau,
					hd.TongTienThue,
					hd.TongChiPhi,
					hd.PhaiThanhToan,
					
					hd.YeuCau,
					hd.ID_PhieuTiepNhan,
					hd.ID_BangGia,
					hd.ID_BaoHiem,
					hd.TongThanhToan,
					hd.NguoiTao,
					hd.DienGiai,
					
    				tn.MaPhieuTiepNhan,
					isnull(hd.PTThueHoaDon,0) as PTThueHoaDon,
					isnull(tblQuy.KhachDaTra,0) as KhachDaTra,
					isnull(hd.PTThueBaoHiem,0) as PTThueBaoHiem,
					isnull(hd.TongTienThueBaoHiem,0) as TongTienThueBaoHiem,
					isnull(hd.SoVuBaoHiem,0) as SoVuBaoHiem,
					isnull(hd.KhauTruTheoVu,0) as KhauTruTheoVu,
					isnull(hd.TongTienBHDuyet,0) as TongTienBHDuyet,
					isnull(hd.PTGiamTruBoiThuong,0) as PTGiamTruBoiThuong,
					isnull(hd.GiamTruBoiThuong,0) as GiamTruBoiThuong,
					isnull(hd.BHThanhToanTruocThue,0) as BHThanhToanTruocThue,
					isnull(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,
					iif(hd.ID_BaoHiem is null,'',tn.NguoiLienHeBH) as LienHeBaoHiem,
					iif(hd.ID_BaoHiem is null,'',tn.SoDienThoaiLienHeBH) as SoDienThoaiLienHeBaoHiem,
					isnull(tblQuy.Khach_TienMat,0) as Khach_TienMat,
					isnull(tblQuy.Khach_TienPOS,0) as Khach_TienPOS,
					isnull(tblQuy.Khach_TienCK,0) as Khach_TienCK,
					isnull(tblQuy.Khach_TienDiem,0) as Khach_TienDiem,
					isnull(tblQuy.Khach_TheGiaTri,0) as Khach_TheGiaTri,
					isnull(tblQuy.Khach_TienCoc,0) as Khach_TienCoc,

    				dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, 
					dt.Email,
					dt.DiaChi,
					dt.MaSoThue,
					dt.TaiKhoanNganHang,
    				case hd.ChoThanhToan
    					when 0 then '0'
    					when 1 then '1'
    					else '2' end as TrangThai,
    				case hd.ChoThanhToan
    					when 0 
    						then 
    							case hd.YeuCau
    							when '1' then N'Đã duyệt'
    							when '2' then N'Đang xử lý'
    							when '3' then N'Hoàn thành'
    							end 
    					when 1 then N'Chờ duyệt'
    					else N'Đã hủy'
    					end as TrangThaiText
    			from BH_HoaDon hd
    			join Gara_PhieuTiepNhan tn on tn.ID= hd.ID_PhieuTiepNhan
    			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
				left join(
				select 
    				b.ID,
    				SUM(ISNULL(b.KhachDaTra, 0)) as KhachDaTra,
					SUM(ISNULL(b.Khach_TienMat, 0)) as Khach_TienMat,
					SUM(ISNULL(b.Khach_TienPOS, 0)) as Khach_TienPOS,
					SUM(ISNULL(b.Khach_TienCK, 0)) as Khach_TienCK,
					SUM(ISNULL(b.Khach_TheGiaTri, 0)) as Khach_TheGiaTri,
					SUM(ISNULL(b.Khach_TienDiem, 0)) as Khach_TienDiem,
					SUM(ISNULL(b.Khach_TienCoc, 0)) as Khach_TienCoc
    			from
    			(
    					-- get infor PhieuThu from HDDatHang (HuyPhieuThu (qhd.TrangThai ='0')
    				Select 
    					hdd.ID,		
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=1, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienMat,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=2, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienPOS,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=3, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienCK,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=4, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TheGiaTri,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=5, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienDiem,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=6, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienCoc,
						iif(qhd.TrangThai='0',0, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu)) as KhachDaTra    							
    					from BH_HoaDon hdd
    				left join Quy_HoaDon_ChiTiet hdct on hdd.ID = hdct.ID_HoaDonLienQuan
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID 				
    				where hdd.LoaiHoaDon = '3' and hdd.ChoThanhToan is not null
    					and hdd.NgayLapHoadon between @FromDate and  @ToDate
						and exists (select ID_DonVi from @tblDonVi dv where qhd.ID_DonVi = dv.ID_DonVi)
    
    				union all
    					-- get infor PhieuThu/Chi from HDXuLy
    				Select
    					hdd.ID,			
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=1, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienMat,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=2, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienPOS,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=3, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienCK,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=4, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TheGiaTri,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=5, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienDiem,
						iif(qhd.TrangThai='0',0, iif(hdct.HinhThucThanhToan=6, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu),0)) as Khach_TienCoc,
						iif(qhd.TrangThai='0' or bhhd.ChoThanhToan is null,0, iif(qhd.LoaiHoaDon = 11, hdct.TienThu, - hdct.TienThu)) as KhachDaTra       									
    				from BH_HoaDon bhhd
    				join BH_HoaDon hdd on (bhhd.ID_HoaDon = hdd.ID and hdd.ChoThanhToan = '0')
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
    				left join Quy_HoaDon qhd on (hdct.ID_HoaDon = qhd.ID)
    				where hdd.LoaiHoaDon = '3' 
    					and bhhd.ChoThanhToan='0'
    					and bhhd.NgayLapHoadon between @FromDate and  @ToDate
						and exists (select ID_DonVi from @tblDonVi dv where hdd.ID_DonVi = dv.ID_DonVi)
    			) b
    			group by b.ID 
				) tblQuy on hd.ID= tblQuy.ID
    			where hd.LoaiHoaDon= 3
    			and exists (select ID_DonVi from @tblDonVi dv where tn.ID_DonVi = dv.ID_DonVi)
    			and hd.NgayLapHoaDon between @FromDate and  @ToDate
    			and hd.ID_PhieuTiepNhan like @ID_PhieuSuaChua
    			and
    				((select count(Name) from @tblSearch b where     			
    				hd.MaHoaDon like '%'+b.Name+'%'
    				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'
    				or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong like '%'+b.Name+'%'	
    				or dt.DienThoai like '%'+b.Name+'%'				
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
    				or hd.NguoiTao like '%'+b.Name+'%'										
    				)=@count or @count=0)	
    			) a where exists (select GiaTri from @tbTrangThai tt where a.TrangThai = tt.GiaTri)
    	),
    	count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    		select dt.*, cte.*, 
    			dt.NguoiTao as NguoiTaoHD,
    			nv.TenNhanVien,
				bh.TenDoiTuong as TenBaoHiem,
				bh.MaDoiTuong as MaBaoHiem,
				bh.Email as BH_Email,
				bh.DiaChi as BH_DiaChi,
				bh.DienThoai as DienThoaiBaoHiem
    		from data_cte dt
    		join NS_NhanVien nv on dt.ID_NhanVien= nv.ID
			left join DM_DoiTuong bh on dt.ID_BaoHiem= bh.ID
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetBangLuongChiTiet_ofNhanVien]
    @IDChiNhanhs [nvarchar](max),
    @IDNhanVien [uniqueidentifier],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs);
    
    	with data_cte
    		as(
    
    		select blct.*,
    			isnull(quyhd.DaTra,0) as DaTra ,
    			round(blct.LuongSauGiamTru - isnull(quyhd.DaTra,0),3) as ConLai
    		from
    		(select bl.NgayTao,
    				bl.TuNgay,
    				bl.DenNgay,
    				ct.ID as ID_BangLuong_ChiTiet,
    				ct.MaBangLuongChiTiet,
    				ct.NgayCongThuc,
    				ct.NgayCongChuan,
    				ct.LuongCoBan,
    				ct.TongLuongNhan as LuongChinh, 
    				ct.LuongOT,
    				ct.PhuCapCoBan,
    				ct.PhuCapKhac,
    				ct.KhenThuong,
    				ct.KyLuat,
    				ct.ChietKhau,    					
    				ct.TongLuongNhan +  ct.LuongOT + ct.PhuCapCoBan + ct.PhuCapKhac + ChietKhau  as LuongTruocGiamTru,
    				ct.TongTienPhat,
    			ct.LuongThucNhan as LuongSauGiamTru
    			
    		from NS_BangLuong_ChiTiet ct
    		join NS_BangLuong bl on ct.ID_BangLuong= bl.ID
    		where ct.ID_NhanVien= @IDNhanVien
    		and exists (select ID from @tblChiNhanh dv where bl.ID_DonVi = dv.ID)
    		and bl.TrangThai in (3,4)
    		) blct
    		left join ( select qct.ID_BangLuongChiTiet,
    							sum(qct.TienThu) +sum( isnull(qct.TruTamUngLuong,0)) as DaTra
    					 from Quy_HoaDon_ChiTiet qct 
    					 join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 
    					 where qhd.TrangThai = 1
    					 and qct.ID_NhanVien= @IDNhanVien
    					 and exists (select ID from @tblChiNhanh dv where qhd.ID_DonVi = dv.ID)
    					 group by qct.ID_BangLuongChiTiet) quyhd on blct.ID_BangLuong_ChiTiet = quyhd.ID_BangLuongChiTiet		 
    
    	),
    	count_cte
    		as (
    			select count(ID_BangLuong_ChiTiet) as TotalRow,
    				CEILING(COUNT(ID_BangLuong_ChiTiet) / CAST(@PageSize as float ))  as TotalPage,
    				sum(NgayCongThuc) as TongNgayCongThuc,
    				sum(LuongChinh) as TongLuongChinh,
    				sum(LuongOT) as TongLuongOT,
    				sum(PhuCapCoBan) as TongPhuCapCoBan,
    				sum(PhuCapKhac) as TongPhuCapKhac,
    				sum(KhenThuong) as TongKhenThuong,
    				sum(KyLuat) as TongKyLuat,
    				sum(ChietKhau) as TongChietKhau,
    				sum(LuongTruocGiamTru) as TongLuongTruocGiamTru,
    				sum(TongTienPhat) as TongTienPhatAll,
    				sum(LuongSauGiamTru) as TongLuongSauGiamTru,
    				sum(DaTra) as TongDaTra,
    				sum(ConLai) as TongConLai
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaBangLuongChiTiet desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetDSGoiDichVu_ofKhachHang]
    @IDChiNhanhs [nvarchar](50) = null,
	@IDCustomers [nvarchar](max) = null,
	@IDCars nvarchar(max) = null,
	@TextSearch nvarchar(max) = null,
	@DateFrom datetime = null,
	@DateTo datetime = null    
AS
BEGIN
    SET NOCOUNT ON;
	declare @sql nvarchar(max)='', @where nvarchar(max)='', @paramDefined nvarchar(max)=''
	declare @tbldefined nvarchar(max) =' declare @tblChiNhanh table(ID uniqueidentifier) 
								declare @tblCus table(ID uniqueidentifier)
								declare @tblCar table(ID uniqueidentifier)'

	set @where =' where 1 = 1 and hd.LoaiHoaDon = 19 
    			and hd.ChoThanhToan=0
				and (ctm.ID_ChiTietDinhLuong is null or ctm.ID= ctm.ID_ChiTietDinhLuong) '

	if isnull(@IDChiNhanhs,'')!=''
		begin
			set @sql = CONCAT(@sql, ' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanh_In) ')
			set @where= CONCAT(@where, ' and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)')
		end

	if isnull(@IDCustomers,'')!=''
		begin			
			set @where = CONCAT(@where , ' and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblCus select name from dbo.splitstring(@IDCustomers_In) ;')
		end
	if isnull(@IDCars,'')!=''
		begin
			set @where = CONCAT(@where , ' and exists (select ID from @tblCar car where hd.ID_Xe = car.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblCar select name from dbo.splitstring(@IDCars_In) ;')
		end

	if isnull(@TextSearch,'')!=''
		set @where= CONCAT(@where, ' and (hd.MaHoaDon like N''%'' + @TextSearch_In + ''%'' 
			or qd.MaHangHoa like N''%'' + @TextSearch_In + ''%''  or hh.TenHangHoa like N''%'' + @TextSearch_In + ''%''
			 or hh.TenHangHoa_KhongDau like N''%'' + @TextSearch_In + ''%'')    ')

	if isnull(@DateFrom,'')!=''
		set @where= CONCAT(@where, ' and (hd.HanSuDungGoiDV is null or hd.HanSuDungGoiDV >= @DateFrom_In)   ')

	if isnull(@DateTo,'')!=''
		set @where= CONCAT(@where, ' and (hd.HanSuDungGoiDV is not null and hd.HanSuDungGoiDV < @DateTo_In)   ')

	set @sql = concat(@tbldefined, @sql, '

    select  
    		hd.ID as ID_GoiDV, MaHoaDon, 
			convert(varchar,hd.NgayLapHoaDon, 103) as NgayLapHoaDon,
    		convert(varchar,hd.NgayApDungGoiDV, 103) as NgayApDungGoiDV,
    		convert(varchar,hd.HanSuDungGoiDV, 103) as HanSuDungGoiDV, 		
    		ctm.ID as ID_ChiTietGoiDV, ctm.ID_DonViQuiDoi, ctm.ID_LoHang, 
    		ISNULL(ctm.ID_TangKem, ''00000000-0000-0000-0000-000000000000'') as ID_TangKem, ISNULL(ctm.TangKem,0) as TangKem, 
			ctm.TienChietKhau ,
			ctm.PTChietKhau ,
    		ctm.DonGia  as GiaBan,
    		ctm.SoLuong, 
    		ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) as SoLuongMua,
    		ISNULL(ctt.SoLuongDung,0) as SoLuongDung,
    		round(ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) - ISNULL(ctt.SoLuongDung,0),2) as SoLuongConLai,		
    		qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,ISNULL(qd.LaDonViChuan,0) as LaDonViChuan, CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
    		hh.LaHangHoa, 
			iif(ctm.TenHangHoaThayThe is null or ctm.TenHangHoaThayThe ='''', hh.TenHangHoa, ctm.TenHangHoaThayThe) as TenHangHoa,
			hh.TonToiThieu, CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
    		ISNULL(hh.ID_NhomHang,''00000000-0000-0000-0000-000000000001'') as ID_NhomHangHoa,
    		ISNULL(hh.SoPhutThucHien,0) as SoPhutThucHien,
    		case when hh.LaHangHoa = 1 then ''0'' else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
    		Case when hh.LaHangHoa=1 then ''0'' else ISNULL(hh.ChiPhiTinhTheoPT,''0'') end as LaPTPhiDichVu,
    		ISNULL(hh.GhiChu,'''') as GhiChuHH,
    		ISNULL(ctm.GhiChu,'''') as GhiChu,
    		isnull(hh.QuanLyTheoLoHang,''0'') as QuanLyTheoLoHang,
    		lo.MaLoHang, lo.NgaySanXuat, lo.NgayHetHan, xe.BienSo, hd.ID_Xe,			
			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
			ISNULL(hh.ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
			ISNULL(hh.LoaiBaoHanh,0) as LoaiBaoHanh,
			ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			hh.LaHangHoa,
			ctm.ID_ParentCombo,			
			iif(ctm.ID_ParentCombo = ctm.ID, 0,1) as SoThuTu,
			isnull(ctm.GiaVon,0) as GiaVon
    	from BH_HoaDon_ChiTiet ctm
    	join BH_HoaDon hd on ctm.ID_HoaDon = hd.ID
		left join Gara_DanhMucXe xe on hd.ID_Xe= xe.ID
    	join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
    	left join DM_LoHang lo on ctm.ID_LoHang = lo.ID
    	left join 
    	(
    		select a.ID_ChiTietGoiDV,
    			SUM(a.SoLuongTra) as SoLuongTra,
    			SUM(a.SoLuongDung) as SoLuongDung
    		from
    			(-- sum soluongtra
    			select ct.ID_ChiTietGoiDV,
    				SUM(ct.SoLuong) as SoLuongTra,
    				0 as SoLuongDung
    			from BH_HoaDon_ChiTiet ct 
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    			where hd.ChoThanhToan= 0 and hd.LoaiHoaDon = 6
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			group by ct.ID_ChiTietGoiDV
    
    			union all
    			-- sum soluong sudung
    			select ct.ID_ChiTietGoiDV,
    				0 as SoLuongDung,
    				SUM(ct.SoLuong) as SoLuongDung
    			from BH_HoaDon_ChiTiet ct 
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    			where hd.ChoThanhToan=0 and hd.LoaiHoaDon in (1,25)
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			group by ct.ID_ChiTietGoiDV
    			) a group by a.ID_ChiTietGoiDV
    	) ctt on ctm.ID = ctt.ID_ChiTietGoiDV ', @where, ' order by hd.NgayLapHoaDon desc')

		print @sql
    	
		set @paramDefined =' @IDChiNhanh_In nvarchar(max),
							@IDCustomers_In nvarchar(max),
							@IDCars_In nvarchar(max),
							@TextSearch_in nvarchar(max),
							@DateFrom_In nvarchar(max),
							@DateTo_in nvarchar(max)'
		
			exec sp_executesql @sql, @paramDefined,
							@IDChiNhanh_In = @IDChiNhanhs,
							@IDCustomers_In = @IDCustomers,
							@IDCars_in = @IDCars,
							@TextSearch_In = @TextSearch,
							@DateFrom_In = @DateFrom,
							@DateTo_in = @DateTo

END");

            Sql(@"ALTER PROCEDURE [dbo].[GetDuLieuChamCong]
    @IDChiNhanhs [nvarchar](max),
    @IDPhongBans [nvarchar](max),
    @IDCaLamViecs [nvarchar](max),
    @TextSearch [nvarchar](max),
    @FromDate [nvarchar](10),
    @ToDate [nvarchar](10),
    @CurrentPage [int],
    @PageSize [int],
	@TrangThaiNV varchar(10)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @dtNow datetime = getdate()
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    	declare @tblDonVi table(ID uniqueidentifier)
    	insert into @tblDonVi
    	select name from dbo.splitstring(@IDChiNhanhs)
		
    	declare @tblTrangThaiNV table(TrangThaiNV int)
    	insert into @tblTrangThaiNV
    	select name from dbo.splitstring(@TrangThaiNV)
    
    	declare @tblPhong table(ID uniqueidentifier)
    	if @IDPhongBans=''	
    		insert into @tblPhong
    		select ID from NS_PhongBan
    	else
    		insert into @tblPhong
    		select name from dbo.splitstring(@IDPhongBans)
    
    	declare @tblCaLamViec table(ID uniqueidentifier)
    	if @IDCaLamViecs='%%' OR  @IDCaLamViecs=''
    		insert into @tblCaLamViec
    		select ca.ID from NS_CaLamViec ca
    		join NS_CaLamViec_DonVi dvca on ca.id= dvca.ID_CaLamViec
    		where exists (select ID from @tblDonVi dv where dv.ID= dvca.ID_DonVi)
    	else
    		insert into @tblCaLamViec
    		select name from dbo.splitstring(@IDCaLamViecs);
    
    with data_cte
    	as (
    		select 
    				nv.MaNhanVien,
    				nv.TenNhanVien,
					iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) as TrangThaiNV, -- danghiviec ~ daxoa
    				ca.MaCa,
    				ca.TenCa,
    				format(ca.GioVao,'HH:mm') as GioVao,
    				format(ca.GioRa,'HH:mm') as GioRa,
    				tblView.TuNgay,
    				tblView.DenNgay,
    				tblView.ID_PhieuPhanCa,
    				tblView.ID_NhanVien,
    				tblView.ID_CaLamViec,					
					cast(tblView.TongCongNV as float) as TongCongNV,
					tblView.SoPhutDiMuon,
					tblView.SoPhutOT,					
    				tblView.Thang,
    				tblView.Nam,
    				tblView.Ngay1, Ngay2, Ngay3, ngay4, ngay5, Ngay6, Ngay7, Ngay8, Ngay9, 
    				Ngay10, Ngay11, Ngay12,ngay13, ngay14, Ngay15,ngay16, ngay17, Ngay18,Ngay19,
    				Ngay20, Ngay21, Ngay22,ngay23, ngay24, Ngay25,ngay26, ngay27, Ngay28,Ngay29,
    				Ngay30, Ngay31,

    				case when Format1 >= TuNgay and Format1 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format1)) end else '1' end as DisNgay1,
					case when Format2 >= TuNgay and Format2 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format2)) end else '1' end as DisNgay2,
					case when Format3 >= TuNgay and Format3 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format3)) end else '1' end as DisNgay3,
					case when Format4 >= TuNgay and Format4 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format4)) end else '1' end as DisNgay4,
					case when Format5 >= TuNgay and Format5 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format5)) end else '1' end as DisNgay5,
					case when Format6 >= TuNgay and Format6 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format6)) end else '1' end as DisNgay6,
					case when Format7 >= TuNgay and Format7 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format7)) end else '1' end as DisNgay7,
					case when Format8 >= TuNgay and Format8 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format8)) end else '1' end as DisNgay8,
					case when Format9 >= TuNgay and Format9 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format9)) end else '1' end as DisNgay9,

					case when Format10 >= TuNgay and Format10 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format10)) end else '1' end as DisNgay10,
					case when Format11 >= TuNgay and Format11 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format11)) end else '1' end as DisNgay11,
					case when Format12 >= TuNgay and Format12 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format12)) end else '1' end as DisNgay12,
					case when Format13 >= TuNgay and Format13 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format13)) end else '1' end as DisNgay13,
					case when Format14 >= TuNgay and Format14 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format14)) end else '1' end as DisNgay14,
					case when Format15 >= TuNgay and Format15 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format15)) end else '1' end as DisNgay15,
					case when Format16 >= TuNgay and Format16 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format16)) end else '1' end as DisNgay16,
					case when Format17 >= TuNgay and Format17 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format17)) end else '1' end as DisNgay17,
					case when Format18 >= TuNgay and Format18 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format18)) end else '1' end as DisNgay18,
					case when Format19 >= TuNgay and Format19 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format19)) end else '1' end as DisNgay19,

					case when Format20 >= TuNgay and Format20 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format20)) end else '1' end as DisNgay20,
					case when Format21 >= TuNgay and Format21 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format21)) end else '1' end as DisNgay21,
					case when Format22 >= TuNgay and Format22 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format22)) end else '1' end as DisNgay22,
					case when Format23 >= TuNgay and Format23 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format23)) end else '1' end as DisNgay23,
					case when Format24 >= TuNgay and Format24 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format24)) end else '1' end as DisNgay24,
					case when Format25 >= TuNgay and Format25 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format25)) end else '1' end as DisNgay25,
					case when Format26 >= TuNgay and Format26 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format26)) end else '1' end as DisNgay26,
					case when Format27 >= TuNgay and Format27 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format27)) end else '1' end as DisNgay27,
					case when Format28 >= TuNgay and Format28 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format28)) end else '1' end as DisNgay28,
					case when Format29 >= TuNgay and Format29 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format29)) end else '1' end as DisNgay29,

					case when Format30 >= TuNgay and Format30 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format30)) end else '1' end as DisNgay30,
					case when Format31 >= TuNgay and Format31 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format31)) end else '1' end as DisNgay31
    			from
    				( 
    			select tblRow.*, phieu.LoaiPhanCa,
    				format(phieu.TuNgay,'yyyy-MM-dd') as TuNgay, 
    				format(ISNULL(phieu.DenNgay,dateadd(month,1,getdate())),'yyyy-MM-dd') as DenNgay,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,1) as Format1,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,2) as Format2,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,3) as Format3,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,4) as Format4,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,5) as Format5,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,6) as Format6,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,7) as Format7,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,8) as Format8,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,9) as Format9,
    
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,10) as Format10,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,11) as Format11,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,12) as Format12,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,13) as Format13,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,14) as Format14,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,15) as Format15,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,16) as Format16,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,17) as Format17,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,18) as Format18,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,19) as Format19,
    
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,20) as Format20,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,21) as Format21,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,22) as Format22,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,23) as Format23,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,24) as Format24,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,25) as Format25,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,26) as Format26,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,27) as Format27,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,28) as Format28,
    				--- avoid error Ngay 29-02, 30-02
    				DATEADD(MONTH, (tblRow.Nam - 1900) * 12 + tblRow.Thang - 1 , 28) as Format29, 
    				DATEADD(MONTH, (tblRow.Nam - 1900) * 12 + tblRow.Thang - 1 , 29) as Format30, 
    				DATEADD(MONTH, (tblRow.Nam - 1900) * 12 + tblRow.Thang - 1 , 30) as Format31
    			from
    			(
    
    				select tblunion.ID as ID_PhieuPhanCa, tblunion.ID_NhanVien, tblunion.Nam, tblunion.Thang, tblunion.ID_CaLamViec,
						max(tblunion.TongCongNV) as TongCongNV,
						max(tblunion.SoPhutDiMuon) as SoPhutDiMuon,
						max(tblunion.SoPhutOT) as SoPhutOT,

    					max(Ngay1) as Ngay1,max(Ngay2) as Ngay2,max(Ngay3) as Ngay3,max(Ngay4) as Ngay4,max(Ngay5) as Ngay5,
    					max(Ngay6) as Ngay6,max(Ngay7) as Ngay7,max(Ngay8) as Ngay8,max(Ngay9) as Ngay9, max(Ngay10) as Ngay10,
    					max(Ngay11) as Ngay11,max(Ngay12) as Ngay12,max(Ngay13) as Ngay13,max(Ngay14) as Ngay14,max(Ngay15) as Ngay15,
    					max(Ngay16) as Ngay16,max(Ngay17) as Ngay17,max(Ngay18) as Ngay18,max(Ngay19) as Ngay19, max(Ngay20) as Ngay20,
    					max(Ngay21) as Ngay21,max(Ngay22) as Ngay22,max(Ngay23) as Ngay23,max(Ngay24) as Ngay24,max(Ngay25) as Ngay25,
    					max(Ngay26) as Ngay26,max(Ngay27) as Ngay27,max(Ngay28) as Ngay28,max(Ngay29) as Ngay29, 
    					max(Ngay30) as Ngay30, max(Ngay31) as Ngay31
    
    				from
    				(
    					select phieu.ID,  phieu.Nam, phieu.Thang, nvphieu.ID_NhanVien, ca.ID as ID_CaLamViec ,
    						null as Ngay1, null as Ngay2, null as Ngay3,  null as Ngay4, null as Ngay5, null as Ngay6,null as Ngay7, null as Ngay8, null as Ngay9 , 
    						null as Ngay10, null as Ngay11, null as Ngay12,null as Ngay13, null as Ngay14, null as Ngay15,null as Ngay16, null as Ngay17, null as Ngay18,null as Ngay19,
    						null as Ngay20, null as Ngay21, null as Ngay22,null as Ngay23, null as Ngay24, null as Ngay25,null as Ngay26, null as Ngay27, null as Ngay28,null as Ngay29,
    						null as Ngay30, null as Ngay31, 0 as TongCongNV, 0 as SoPhutDiMuon, 0 as SoPhutOT
    					from NS_PhieuPhanCa_NhanVien nvphieu 				
    					join (select ID, case when DenNgay is null then case when @ToDate < @dtNow then datepart(year,@ToDate) else datepart(year, @dtNow) end
								else
								case when datepart(year,DenNgay) != datepart(year, @dtNow) then datepart(year, @dtNow) else  datepart(year,TuNgay) end end as Nam, 
    							case when DenNgay is null then datepart(month,@FromDate)  else 
    							case when TuNgay < @FromDate then datepart(month,@FromDate) else datepart(month,TuNgay) end end as Thang
    						from  NS_PhieuPhanCa
    						where TrangThai != 0  and ((DenNgay is null and TuNgay <= @ToDate ) 
    							OR ((DenNgay is not null 
    								and ((DenNgay <= @ToDate and DenNgay >=  @FromDate )
    									or (DenNgay >= @ToDate  and TuNgay <= @ToDate)))))
    						) phieu on nvphieu.ID_PhieuPhanCa = phieu.ID						
    					join NS_PhieuPhanCa_CaLamViec caphieu on nvphieu.ID_PhieuPhanCa = caphieu.ID_PhieuPhanCa
    					join NS_CaLamViec ca on ca.ID= caphieu.ID_CaLamViec
    							--where exists (select ID from @tblCaLamViec ca2 where ca.ID= ca2.ID)
    
    					union all
    
						select pivOut.*, congOut.TongCongNV, 
							congOut.SoPhutDiMuon,
							congOut.SoPhutOT

						from
    						(select piv.ID_PhieuPhanCa, piv.Nam,  piv.Thang, piv.ID_NhanVien, piv.ID_CaLamViec, [1] as Ngay1, [2] as Ngay2,[3] as Ngay3, [4] as Ngay4, [5] as Ngay5, [6] as Ngay6,[7] as Ngay7, [8] as Ngay8, [9] as Ngay9,
    							[10] as Ngay10, [11] as Ngay11, [12] as Ngay12,[13] as Ngay13, [14] as Ngay14, [15] as Ngay15, [16] as Ngay16,[17] as Ngay17, [18] as Ngay18, [19] as Ngay19,
    							[20] as Ngay20, [21] as Ngay21, [22] as Ngay22,[23] as Ngay23, [24] as Ngay24, [25] as Ngay25, [26] as Ngay26,[27] as Ngay27, [28] as Ngay28, [29] as Ngay29,
    							[30] as Ngay30, [31] as Ngay31
    						from
    						(
    						select phieu.ID as ID_PhieuPhanCa, bs.ID_NhanVien, bs.ID_CaLamViec, bs.ID_DonVi, DATEPART(DAY, bs.NgayCham) as Ngay,DATEPART(MONTH, bs.NgayCham) as Thang,DATEPART(YEAR, bs.NgayCham) as Nam,
    						bs.KyHieuCong	
    						from NS_CongBoSung bs
    						join NS_PhieuPhanCa_NhanVien phieunv on bs.ID_NhanVien = phieunv.ID_NhanVien
    						join NS_PhieuPhanCa_CaLamViec phieuca on phieunv.ID_PhieuPhanCa = phieuca.ID_PhieuPhanCa and  bs.ID_CaLamViec = phieuca.ID_CaLamViec
    						join NS_PhieuPhanCa phieu on phieunv.ID_PhieuPhanCa = phieu.ID
    						where phieu.TrangThai != 0
    							and ((DenNgay is null and TuNgay <= @ToDate) 
    								OR ((DenNgay is not null 
    									and ((DenNgay <= @ToDate and DenNgay >= @FromDate )
    									or (DenNgay >= @ToDate and TuNgay <= @ToDate )))))
    						and bs.NgayCham >= @FromDate and bs.NgayCham <=@ToDate
							and bs.TrangThai !=0
							and exists (select ID from @tblDonVi dv where dv.ID= bs.ID_DonVi)
    						) a
    						PIVOT (
    						  max(KyHieuCong)
    						  FOR Ngay in ( [1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19], [20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31]) 
    						) piv 
						) pivOut
						join (
							-- sumcong ofnv
							select  
								cong2.ID_NhanVien, cong2.ID_CaLamViec,
								cong2.CongNgayThuong + cong2.CongNgayNghiLe as TongCongNV,
								cong2.SoPhutDiMuon,
								SoGioOT as SoPhutOT								
								from
								(
												select cong.ID_NhanVien, cong.ID_CaLamViec, cong.ID_DonVi,
    												sum(cong.CongNgayThuong) as CongNgayThuong,
    												sum(CongNgayNghiLe) as CongNgayNghiLe,
    												sum(OTNgayThuong) as OTNgayThuong,
    												sum(OTNgayNghiLe) as OTNgayNghiLe,
    												sum(SoPhutDiMuon) as SoPhutDiMuon,
													sum(SoGioOT) as SoGioOT
    											from
    												(select bs.ID_CaLamViec,  bs.ID_NhanVien, bs.ID_DonVi,
    													bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon,
    													IIF(bs.LoaiNgay=0, bs.Cong,0) as CongNgayThuong,
    													IIF(bs.LoaiNgay!=0, bs.Cong,0) as CongNgayNghiLe,
    													IIF(bs.LoaiNgay=0, bs.SoGioOT,0) as OTNgayThuong,
    													IIF(bs.LoaiNgay!=0, bs.SoGioOT,0) as OTNgayNghiLe
    												from NS_CongBoSung bs
    												join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
    												where NgayCham >= @FromDate and NgayCham <= @ToDate
    												and bs.TrangThai !=0    		
													and  exists (select ID from @tblDonVi dv where dv.ID= bs.ID_DonVi) 
    												) cong group by cong.ID_NhanVien, cong.ID_CaLamViec, cong.ID_DonVi
											 ) cong2
						) congOut on pivout.ID_NhanVien= congOut.ID_NhanVien and pivout.ID_CaLamViec= congOut.ID_CaLamViec
    				) tblunion
    				group by  tblunion.ID,tblunion.ID_NhanVien, tblunion.Nam, tblunion.Thang, tblunion.ID_CaLamViec
    			) tblRow
    			join NS_PhieuPhanCa phieu on tblRow.ID_PhieuPhanCa = phieu.ID
    		) tblView 
    	join NS_CaLamViec ca on tblView.ID_CaLamViec = ca.ID
    	join NS_NhanVien nv on tblView.ID_NhanVien= nv.ID 
    	join ( select nv.ID, nv.MaNhanVien, nv.TenNhanVien
    			from NS_NhanVien nv 
    			left join NS_QuaTrinhCongTac ct on nv.ID= ct.ID_NhanVien
    			where exists (select ID from @tblDonVi dv where dv.ID= ct.ID_DonVi) 
    			AND exists (select ID from @tblPhong pb where pb.ID= ct.ID_PhongBan or ct.ID_PhongBan is null) 
    			) nv2 on nv.ID= nv2.ID 
    	where exists (select ID from @tblCaLamViec ca2 where ca.ID= ca2.ID) --- van hien nv daxoa de check bang cong cu
		and exists (select TrangThaiNV from @tblTrangThaiNV tt where iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) = tt.TrangThaiNV)
    	AND ((select count(Name) from @tblSearchString b where    			
    				nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'
    				)=@count or @count=0)	
    	),
    	count_cte
    	as (
    	SELECT COUNT(*) AS TotalRow, 
    			CEILING(COUNT(*) / CAST(@PageSize as float )) as TotalPage ,
				SUM(TongCongNV) as TongCongAll,
				SUM(SoPhutDiMuon) as TongSoPhutDiMuon
    	from data_cte
    	)
    	select dt.*, cte.*
    	from data_cte dt
    	cross join count_cte cte		
    	order by dt.TuNgay
    	OFFSET (@CurrentPage* @PageSize) ROWS
    	FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_Where]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max)
AS
BEGIN
	set nocount on;

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	if @timeStart='2016-01-01'		
		select @timeStart = min(NgayLapHoaDon) from BH_HoaDon where LoaiHoaDon=19
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
    	c.MaHoaDonGoc,
    	c.TongTienHDTra,
    	c.NgayLapHoaDon,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
    	c.NguoiTaoHD,
    	c.DiaChiKhachHang,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TenPhongBan,
    	c.TongTienHang, c.TongGiamGia, 
		c.TongThanhToan,
		case when c.PhaiThanhToan < c.TongTienHDTra then 0 else  c.PhaiThanhToan - c.TongTienHDTra - c.KhachDaTra end as ConNo,
		case when c.PhaiThanhToan < c.TongTienHDTra then 0 else  c.PhaiThanhToan - c.TongTienHDTra end as PhaiThanhToan,
		c.ThuTuThe, c.TienMat, c.TienATM,c.TienDoiDiem, c.ChuyenKhoan, c.KhachDaTra,c.TongChietKhau,c.TongTienThue,PTThueHoaDon,
		c.TongThueKhachHang,
		ID_TaiKhoanPos,
		ID_TaiKhoanChuyenKhoan,
    	c.TrangThai,
    	c.KhuyenMai_GhiChu,
    	c.KhuyeMai_GiamGia,
    	c.LoaiHoaDonGoc,
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
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
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
			bhhd.TongThanhToan,
			ISNULL(bhhd.TongThueKhachHang,0) as TongThueKhachHang,
			ISNULL(bhhd.TongTienThue,0) as TongTienThue,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
    		CAST(ROUND(ISNULL(hdt.PhaiThanhToan,0),0) as float) as TongTienHDTra,
    		a.ThuTuThe,
    		a.TienMat,
			a.TienATM,
			a.TienDoiDiem,
    		a.ChuyenKhoan,
    		a.KhachDaTra,
			ID_TaiKhoanPos,
			ID_TaiKhoanChuyenKhoan,
    		ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
    		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai
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
    				where bhhd.LoaiHoaDon = '19' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd 
					and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))     			
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
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
				or xe.BienSo like '%'+b.Name+'%'	
				or c.HoaDon_HangHoa like '%'+b.Name+'%'			
				)=@count or @count=0)	
    	ORDER BY c.NgayLapHoaDon DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetList_HoaDonNhapHang]
    @TextSearch [nvarchar](max),
    @LoaiHoaDon [int], ---- dùng chung cho nhập hàng + trả hàng nhập
    @IDChiNhanhs [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TrangThais [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int],
    @ColumnSort [nvarchar](max),
    @SortBy [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @tblChiNhanh table (ID varchar(40))
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs)
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    	with data_cte
    	as (
    	select hdQuy.*	, hdQuy.PhaiThanhToan - hdQuy.KhachDaTra as ConNo
    	from
    	(	
    	select hd.id, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hd.DienGiai, hd.PhaiThanhToan, hd.ChoThanhToan,
    	hd.NgayLapHoaDon, hd.ID_NhanVien, hd.ID_BangGia, hd.TongTienHang, hd.TongChietKhau, hd.TongGiamGia, hd.TongChiPhi,
    	hd.TongTienThue, hd.TongThanhToan, hd.ID_DoiTuong, 
		ctHD.ThanhTienChuaCK,
		ctHD.GiamGiaCT,
		iif(@LoaiHoaDon=4, isnull(quy.TienThu,0), - isnull(quy.TienThu,0))  as KhachDaTra	,
		isnull(quy.TienMat,0) as TienMat,
		isnull(quy.ChuyenKhoan,0) as ChuyenKhoan,
		isnull(quy.TienATM,0) as TienATM,
		isnull(quy.TienDatCoc,0) as TienDatCoc,
		hd.NguoiTao, hd.NguoiTao as NguoiTaoHD,
    	dv.TenDonVi,hd.ID_DonVi,
		isnull(hd.PTThueHoaDon,0) as PTThueHoaDon,
    	isnull(dv.SoDienThoai,'') as DienThoaiChiNhanh,
    	isnull(dv.DiaChi,'') as DiaChiChiNhanh,
    	isnull(dt.MaDoiTuong,'') as MaDoiTuong,
    	isnull(dt.TenDoiTuong,'') as TenDoiTuong,
    	isnull(dt.DienThoai,'') as DienThoai,
    	isnull(dt.TenDoiTuong_KhongDau,'') as TenDoiTuong_KhongDau,
    	isnull(nv.MaNhanVien,'') as MaNhanVien,
    	isnull(nv.TenNhanVien,'') as TenNhanVien,
    	isnull(nv.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
    	case  hd.ChoThanhToan
    				when 1 then '1'
    				when 0 then '0'
    			else '2' end as TrangThaiHD
    	from BH_HoaDon hd
    	join DM_DonVi dv on hd.ID_DonVi= dv.ID
    	left join  DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
		left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
		left join (
			select 
				ct.ID_HoaDon,
				sum(ct.SoLuong * ct.TienChietKhau) as GiamGiaCT,			
				sum(ct.SoLuong * ct.DonGia) as ThanhTienChuaCK
			from BH_HoaDon_ChiTiet ct
    		join BH_HoaDon hd on ct.ID_HoaDon= hd.ID   			
    		where hd.LoaiHoaDon= @LoaiHoaDon    		
    		and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    		and  exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
			group by ct.ID_HoaDon
		) ctHD on ctHD.ID_HoaDon = hd.ID
    	left join
    	(
    		select a.ID_HoaDonLienQuan, 
				sum(TienThu) as TienThu,
				sum(a.TienMat) as TienMat,
				sum(a.TienATM) as TienATM,				
				sum(a.ChuyenKhoan) as ChuyenKhoan,
				sum(a.TienDatCoc) as TienDatCoc
    		from(
    			select qct.ID_HoaDonLienQuan,   
				iif(qct.HinhThucThanhToan =1, qct.TienThu, 0) as TienMat,
				iif(qct.HinhThucThanhToan = 2, qct.TienThu,0) as TienATM,
				iif(qct.HinhThucThanhToan = 3, qct.TienThu,0) as ChuyenKhoan,
				iif(qct.HinhThucThanhToan = 6, qct.TienThu,0) as TienDatCoc,
				iif(qhd.LoaiHoaDon = 11,-qct.TienThu, qct.TienThu) as TienThu
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
    			where hd.LoaiHoaDon= @LoaiHoaDon
    			and (qhd.TrangThai= 1 or qhd.TrangThai is null)
    			and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    			and  exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
    			) a group by a.ID_HoaDonLienQuan
    	) quy on hd.id = quy.ID_HoaDonLienQuan
    	where hd.LoaiHoaDon= @LoaiHoaDon and
    	NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    	and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
    ) hdQuy
    where 
    exists (select ID from dbo.splitstring(@TrangThais) tt where hdQuy.TrangThaiHD= tt.Name)	
    	and
    	((select count(Name) from @tblSearch b where     			
    		hdQuy.MaHoaDon like '%'+b.Name+'%'
    		or hdQuy.NguoiTao like '%'+b.Name+'%'				
    		or hdQuy.TenNhanVien like '%'+b.Name+'%'
    		or hdQuy.TenNhanVienKhongDau like '%'+b.Name+'%'
    		or hdQuy.DienGiai like '%'+b.Name+'%'
    		or hdQuy.MaDoiTuong like '%'+b.Name+'%'		
    		or hdQuy.TenDoiTuong like '%'+b.Name+'%'
    		or hdQuy.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or hdQuy.DienThoai like '%'+b.Name+'%'		
    		)=@count or @count=0)	
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
					sum(ThanhTienChuaCK) as SumThanhTienChuaCK,
					sum(GiamGiaCT) as SumGiamGiaCT,
    				sum(TongTienHang) as SumTongTienHang,
    				sum(TongGiamGia) as SumTongGiamGia,
					sum(TienMat) as SumTienMat,	
					sum(TienATM) as SumPOS,	
					sum(ChuyenKhoan) as SumChuyenKhoan,	
					sum(TienDatCoc) as SumTienCoc,	
    				sum(KhachDaTra) as SumDaThanhToan,				
    				sum(TongChiPhi) as SumTongChiPhi,
    				sum(PhaiThanhToan) as SumPhaiThanhToan,
    				sum(TongThanhToan) as SumTongThanhToan,				
    				sum(TongTienThue) as SumTongTienThue,
    				sum(ConNo) as SumConNo
    			from data_cte
    		)
    		select dt.*, cte.*	
    		from data_cte dt
    		cross join count_cte cte
    		order by 
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='ConNo' then ConNo end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='ConNo' then ConNo end DESC,
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
    			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end DESC,
    			case when @SortBy <>'ASC' then 0
    			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC			
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListBaoHiem_v1]
    @IdChiNhanhs [nvarchar](max),
    @NgayTaoFrom [datetime],
    @NgayTaoTo [datetime],
    @TongBanDateFrom [datetime],
    @TongBanDateTo [datetime],
    @TongBanFrom [float],
    @TongBanTo [float],
    @NoFrom [float],
    @NoTo [float],
    @TrangThais [nvarchar](20),
    @TextSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblDonVi table (ID_DonVi  uniqueidentifier);
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    	declare @tbTrangThai table (GiaTri varchar(2));
    	if(@TrangThais != '')
    	BEGIN
    		insert into @tbTrangThai
    		select Name from dbo.splitstring(@TrangThais);
    	END
    	DECLARE @tblResult TABLE(ID UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), DienThoai NVARCHAR(MAX), MaSoThue NVARCHAR(MAX), Email NVARCHAR(MAX), DiaChi NVARCHAR(MAX), ID_TinhThanh UNIQUEIDENTIFIER, 
    	TenTinhThanh NVARCHAR(MAX), ID_QuanHuyen UNIQUEIDENTIFIER, TenQuanHuyen NVARCHAR(MAX),
    	GhiChu NVARCHAR(MAX), ID_DonVi UNIQUEIDENTIFIER, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), NgayTao DATETIME, LoaiDoiTuong INT, NguoiTao NVARCHAR(MAX), NoHienTai FLOAT, TongTienBaoHiem FLOAT, TotalRow INT, TotalPage FLOAT);
    
    
    	DECLARE @tblDoiTuong TABLE(ID UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), DienThoai NVARCHAR(MAX), MaSoThue NVARCHAR(MAX), Email NVARCHAR(MAX), DiaChi NVARCHAR(MAX), ID_TinhThanh UNIQUEIDENTIFIER, 
    	TenTinhThanh NVARCHAR(MAX), ID_QuanHuyen UNIQUEIDENTIFIER, TenQuanHuyen NVARCHAR(MAX),
    	GhiChu NVARCHAR(MAX), ID_DonVi UNIQUEIDENTIFIER, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), NgayTao DATETIME, LoaiDoiTuong INT, NguoiTao NVARCHAR(MAX));
    	DECLARE @tblBaoHiemPhaiThanhToan TABLE(ID UNIQUEIDENTIFIER, PhaiThanhToan FLOAT);
    	DECLARE @tblBaoHiemDaThanhToan TABLE(ID UNIQUEIDENTIFIER, DaThanhToan FLOAT);
    	DECLARE @TotalRow INT;
    	DECLARE @TotalPage FLOAT;
    
    	IF (@TongBanFrom IS NULL AND @TongBanTo IS NULL AND @NoFrom IS NULL AND @NoTo IS NULL)
    	BEGIN
    		
    		INSERT INTO @tblDoiTuong
    		SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
    		dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
    		LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
    		LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
    		inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
    		inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
    		INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
    		WHERE 
    			dt.LoaiDoiTuong  = 3
    			AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
    			AND ((select count(Name) from @tblSearch b where     			
    			dt.GhiChu like '%'+b.Name+'%'
    			or dt.MaDoiTuong like '%'+b.Name+'%'		
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.DienThoai like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.MaSoThue like '%'+b.Name+'%'
    			or dt.DiaChi like '%'+b.Name+'%'
    			or dt.Email like '%'+b.Name+'%'
    			or tt.TenTinhThanh like '%'+b.Name+'%'
    			or qh.TenQuanHuyen like '%'+b.Name+'%'
    			or dv.MaDonVi like '%'+b.Name+'%'
    			or dv.TenDonVi like '%'+b.Name+'%'
    			)=@count or @count=0);
    
    			--SELECT * FROM @tblDoiTuong;
				IF (@PageSize != 0)
				BEGIN
    				INSERT INTO @tblBaoHiemPhaiThanhToan
    				select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
    				INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
    							OFFSET (@CurrentPage * @PageSize) ROWS
    							FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = hd.ID_BaoHiem 
    				inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
    				where hd.LoaiHoaDon = 25 and ChoThanhToan = 0
    				AND ID_BaoHiem IS NOT NULL
    				AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
    				GROUP BY ID_BaoHiem
    				--HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
    				--AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanTo)
    		
    				INSERT INTO @tblBaoHiemDaThanhToan
    				SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
    				INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
    							OFFSET (@CurrentPage * @PageSize) ROWS
    							FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = qhdct.ID_DoiTuong
    				inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
    				WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo) AND qhdct.HinhThucThanhToan != 6
					AND qhd.TrangThai = 1
    				GROUP BY qhdct.ID_DoiTuong
    				--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
    				--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
    				SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
    				INSERT INTO @tblResult
    				SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    				LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    				LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    				ORDER BY dt.NgayTao desc
    				OFFSET (@CurrentPage * @PageSize) ROWS
    				FETCH NEXT @PageSize ROWS ONLY;
				END
				ELSE
				BEGIN
					INSERT INTO @tblBaoHiemPhaiThanhToan
    				select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
    				INNER JOIN @tblDoiTuong dt ON dt.ID = hd.ID_BaoHiem 
    				inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
    				where hd.LoaiHoaDon = 25 and ChoThanhToan = 0
    				AND ID_BaoHiem IS NOT NULL
    				AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
    				GROUP BY ID_BaoHiem
    				--HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
    				--AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanTo)
    		
    				INSERT INTO @tblBaoHiemDaThanhToan
    				SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
    				INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				INNER JOIN @tblDoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    				inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
    				WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo) AND qhdct.HinhThucThanhToan != 6
					AND qhd.TrangThai = 1
    				GROUP BY qhdct.ID_DoiTuong
    				--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
    				--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
    				SELECT @TotalRow = 0, @TotalPage = 0 FROM @tblDoiTuong;
    				INSERT INTO @tblResult
    				SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    				LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    				LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    				ORDER BY dt.NgayTao desc;
				END
    	END
    	ELSE
    	BEGIN
    		IF(@NoFrom IS NULL AND @NoTo IS NULL)
    		BEGIN
    			IF(@TongBanFrom = 0 OR @TongBanTo = 0 OR @TongBanFrom IS NULL)
    			BEGIN
    				INSERT INTO @tblBaoHiemPhaiThanhToan
    				select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
    				inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
    				where hd.LoaiHoaDon = 25 and ChoThanhToan = 0
    				AND ID_BaoHiem IS NOT NULL
    				AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
    				GROUP BY ID_BaoHiem
    				HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
    				AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) <= @TongBanTo)
    
    				INSERT INTO @tblDoiTuong
    				SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
    				dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
    				LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
    				LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
    				inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
    				inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
    				INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
    				LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    				WHERE 
    					dt.LoaiDoiTuong  = 3 AND (ISNULL(ptt.PhaiThanhToan, 0) >= @TongBanFrom OR @TongBanFrom IS NULL) AND (ISNULL(ptt.PhaiThanhToan, 0) <= @TongBanTo OR @TongBanTo IS NULL)
    					AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
    					AND ((select count(Name) from @tblSearch b where     			
    					dt.GhiChu like '%'+b.Name+'%'
    					or dt.MaDoiTuong like '%'+b.Name+'%'		
    					or dt.TenDoiTuong like '%'+b.Name+'%'
    					or dt.DienThoai like '%'+b.Name+'%'
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    					or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    					or dt.MaSoThue like '%'+b.Name+'%'
    					or dt.DiaChi like '%'+b.Name+'%'
    					or dt.Email like '%'+b.Name+'%'
    					or tt.TenTinhThanh like '%'+b.Name+'%'
    					or qh.TenQuanHuyen like '%'+b.Name+'%'
    					or dv.MaDonVi like '%'+b.Name+'%'
    					or dv.TenDonVi like '%'+b.Name+'%'
    					)=@count or @count=0);
					IF (@PageSize != 0)
					BEGIN
    					INSERT INTO @tblBaoHiemDaThanhToan
    					SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
    					INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    					INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
    								OFFSET (@CurrentPage * @PageSize) ROWS
    								FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = qhdct.ID_DoiTuong
    					inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
    					WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo) AND qhdct.HinhThucThanhToan != 6
						AND qhd.TrangThai = 1
    					GROUP BY qhdct.ID_DoiTuong
    					--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
    					--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
    					SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
    					INSERT INTO @tblResult
    					SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    					LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    					LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    					ORDER BY dt.NgayTao desc
    					OFFSET (@CurrentPage * @PageSize) ROWS
    					FETCH NEXT @PageSize ROWS ONLY;
					END
					ELSE
					BEGIN
						INSERT INTO @tblBaoHiemDaThanhToan
    					SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
    					INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    					INNER JOIN @tblDoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    					inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
    					WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo) AND qhdct.HinhThucThanhToan != 6
						AND qhd.TrangThai = 1
    					GROUP BY qhdct.ID_DoiTuong
    					--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
    					--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
    					SELECT @TotalRow = 0, @TotalPage = 0 FROM @tblDoiTuong;
    					INSERT INTO @tblResult
    					SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    					LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    					LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    					ORDER BY dt.NgayTao desc;
					END
    			END
    			ELSE
    			BEGIN
    				INSERT INTO @tblBaoHiemPhaiThanhToan
    				select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
    				inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
    				where hd.LoaiHoaDon = 25 and ChoThanhToan = 0
    				AND ID_BaoHiem IS NOT NULL
    				AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
    				GROUP BY ID_BaoHiem
    				HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
    				AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) <= @TongBanTo)
    
    				INSERT INTO @tblDoiTuong
    				SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
    				dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
    				LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
    				LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
    				inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
    				inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
    				INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
    				INNER JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    				WHERE 
    					dt.LoaiDoiTuong  = 3
    					AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
    					AND ((select count(Name) from @tblSearch b where     			
    					dt.GhiChu like '%'+b.Name+'%'
    					or dt.MaDoiTuong like '%'+b.Name+'%'		
    					or dt.TenDoiTuong like '%'+b.Name+'%'
    					or dt.DienThoai like '%'+b.Name+'%'
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    					or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    					or dt.MaSoThue like '%'+b.Name+'%'
    					or dt.DiaChi like '%'+b.Name+'%'
    					or dt.Email like '%'+b.Name+'%'
    					or tt.TenTinhThanh like '%'+b.Name+'%'
    					or qh.TenQuanHuyen like '%'+b.Name+'%'
    					or dv.MaDonVi like '%'+b.Name+'%'
    					or dv.TenDonVi like '%'+b.Name+'%'
    					)=@count or @count=0);
						IF (@PageSize != 0)
						BEGIN
    						INSERT INTO @tblBaoHiemDaThanhToan
    						SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
    						INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
    									OFFSET (@CurrentPage * @PageSize) ROWS
    									FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = qhdct.ID_DoiTuong
    						inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
    						WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo) and qhdct.HinhThucThanhToan != 6
							AND qhd.TrangThai = 1
    						GROUP BY qhdct.ID_DoiTuong
    						--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
    						--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
    						SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
    						INSERT INTO @tblResult
    						SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    						LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    						LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    						ORDER BY dt.NgayTao desc
    						OFFSET (@CurrentPage * @PageSize) ROWS
    						FETCH NEXT @PageSize ROWS ONLY;
						END
						ELSE
						BEGIN
							INSERT INTO @tblBaoHiemDaThanhToan
    						SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
    						INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						INNER JOIN @tblDoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
    						WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo) and qhdct.HinhThucThanhToan != 6
							AND qhd.TrangThai = 1
    						GROUP BY qhdct.ID_DoiTuong
    						--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
    						--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
    						SELECT @TotalRow = 0, @TotalPage = 0 FROM @tblDoiTuong;
    						INSERT INTO @tblResult
    						SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    						LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    						LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    						ORDER BY dt.NgayTao desc;
						END
    				END
    		END
    		ELSE
    		BEGIN
    			INSERT INTO @tblBaoHiemPhaiThanhToan
    			select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
    			inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
    			where hd.LoaiHoaDon = 25 and ChoThanhToan = 0
    			AND ID_BaoHiem IS NOT NULL
    			AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
    			GROUP BY ID_BaoHiem
    			HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
    			AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) <= @TongBanTo);
    
    			INSERT INTO @tblBaoHiemDaThanhToan
    			SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
    			INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
    			WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo) and qhdct.HinhThucThanhToan != 6
				AND qhd.TrangThai = 1
    			GROUP BY qhdct.ID_DoiTuong;
    
    			INSERT INTO @tblDoiTuong
    			SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
    			dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
    			LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
    			LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
    			inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
    			inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
    			INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
    			LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    			LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    			WHERE 
    				dt.LoaiDoiTuong  = 3
    				AND (ISNULL(ptt.PhaiThanhToan, 0) >= @TongBanFrom OR @TongBanFrom IS NULL) AND (ISNULL(ptt.PhaiThanhToan, 0) <= @TongBanTo OR @TongBanTo IS NULL)
    				AND ((ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0)) >= @NoFrom OR @NoFrom IS NULL) AND ((ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0)) <= @NoTo OR @NoTo IS NULL)
    				AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
    				AND ((select count(Name) from @tblSearch b where     			
    				dt.GhiChu like '%'+b.Name+'%'
    				or dt.MaDoiTuong like '%'+b.Name+'%'		
    				or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.DienThoai like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.MaSoThue like '%'+b.Name+'%'
    				or dt.DiaChi like '%'+b.Name+'%'
    				or dt.Email like '%'+b.Name+'%'
    				or tt.TenTinhThanh like '%'+b.Name+'%'
    				or qh.TenQuanHuyen like '%'+b.Name+'%'
    				or dv.MaDonVi like '%'+b.Name+'%'
    				or dv.TenDonVi like '%'+b.Name+'%'
    				)=@count or @count=0);
					IF(@PageSize != 0)
					BEGIN
    					SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
    					INSERT INTO @tblResult
    					SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    					LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    					LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    					ORDER BY dt.NgayTao desc
    					OFFSET (@CurrentPage * @PageSize) ROWS
    					FETCH NEXT @PageSize ROWS ONLY;
					END
					ELSE
					BEGIN
						SELECT @TotalRow = 0, @TotalPage = 0 FROM @tblDoiTuong;
    					INSERT INTO @tblResult
    					SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
    					LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
    					LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
    					ORDER BY dt.NgayTao desc;
					END
    		END
    	END
    		
    		SELECT ID, MaDoiTuong, TenDoiTuong , DienThoai , MaSoThue , Email , DiaChi , ID_TinhThanh , 
    	TenTinhThanh , ID_QuanHuyen , TenQuanHuyen ,
    	GhiChu , ID_DonVi , MaDonVi , TenDonVi , NgayTao , LoaiDoiTuong , NguoiTao , ROUND(NoHienTai, 0) AS NoHienTai , ROUND(TongTienBaoHiem,0) AS TongTienBaoHiem, TotalRow , TotalPage  FROM @tblResult
		ORDER BY NgayTao desc;
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListCashFlow_Before]
    @IDDonVis [nvarchar](max),
    @ID_NhanVien [nvarchar](40),
    @ID_TaiKhoanNganHang [nvarchar](40),
    @ID_KhoanThuChi [nvarchar](40),
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiSoQuy varchar(15),
    @LoaiChungTu [nvarchar](2),
    @TrangThaiSoQuy [nvarchar](2),
    @TrangThaiHachToan [nvarchar](2),
    @TxtSearch [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;

	declare @tblDonVi table (ID_DonVi uniqueidentifier)
	insert into @tblDonVi
	select name from dbo.splitstring(@IDDonVis)

	declare @tblLoai table (Loai int)
	insert into @tblLoai
	select name from dbo.splitstring(@LoaiSoQuy)

	--declare @nguoitao nvarchar(100) = (select taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
	--declare @tblNhanVien table (ID uniqueidentifier)
	--insert into @tblNhanVien
	--select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDDonVis,'SoQuy_XemDS_PhongBan','SoQuy_XemDS_HeThong');
 
 select 
	ceiling(sum(ThuMat)- sum(ChiMat)) as TongThuMat,  
	ceiling(sum(ThuGui) - sum(ChiGui)) as TongThuCK
 from
 (
 select
	iif(a1.LoaiHoaDon=11, TienMat,0) as ThuMat,
	iif(a1.LoaiHoaDon=12, TienMat,0) as ChiMat,
	iif(a1.LoaiHoaDon=11, TienGui,0) as ThuGui,
	iif(a1.LoaiHoaDon=12, TienGui,0) as ChiGui
 from
 (
    	 select
			tblView.LoaiHoaDon,
			sum(TienMat) as TienMat,
			sum(TienGui) as TienGui
	from
		(select 
			tblQuy.MaHoaDon,		
			tblQuy.LoaiHoaDon,
			ISNUll(tblQuy.TrangThai,'1') as TrangThai,
			tblQuy.NoiDungThu,
			tblQuy.ID_NhanVienPT as ID_NhanVien,			
			TienMat, TienGui, TienMat + TienGui as TienThu,
			TienMat + TienGui as TongTienThu,
			cast(ID_TaiKhoanNganHang as varchar(max)) as ID_TaiKhoanNganHang,
			case when tblQuy.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
    		case when tblQuy.HachToanKinhDoanh = '1' then '11' else '10' end as TrangThaiHachToan,
    		case when tblQuy.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
			case when nv.TenNhanVien is null then  dt.TenDoiTuong  else nv.TenNhanVien end as NguoiNopTien,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong_KhongDau  else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
    		case when nv.MaNhanVien is null then dt.MaDoiTuong else  nv.MaNhanVien end as MaDoiTuong,
    		case when nv.MaNhanVien is null then dt.DienThoai else  nv.DienThoaiDiDong  end as SoDienThoai,
			case when tblQuy.TienMat > 0 then case when tblQuy.TienGui > 0 then '2' else '1' end 
			else case when tblQuy.TienGui > 0 then '0'
				else case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then '0' else '1' end end end as LoaiSoQuy
							
		from
			(select 
				 a.ID_hoaDon, 
				 a.MaHoaDon,			
				 a.LoaiHoaDon,
				 a.HachToanKinhDoanh, 
				 a.NoiDungThu,
				 a.ID_NhanVienPT, a.TrangThai,
				 sum(isnull(a.TienMat, 0)) as TienMat,
				 sum(isnull(a.TienGui, 0)) as TienGui,
				 max(a.ID_DoiTuong) as ID_DoiTuong,
				 max(a.ID_NhanVien) as ID_NhanVien,
				 max(a.ID_TaiKhoanNganHang) as ID_TaiKhoanNganHang			
			from
			(				
					select qhd.MaHoaDon,				
					qhd.LoaiHoaDon,
					qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo, qhd.NoiDungThu,
					qhd.ID_NhanVien as ID_NhanVienPT, qhd.TrangThai,
					qct.ID_HoaDon, 
					iif(qct.HinhThucThanhToan= 1, qct.TienThu,0) as TienMat,
					iif(qct.HinhThucThanhToan= 2 or qct.HinhThucThanhToan = 3, qct.TienThu,0) as TienGui,		
					qct.ID_DoiTuong, qct.ID_NhanVien, 
					ISNULL(qct.ID_TaiKhoanNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
					ISNULL(qct.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
					join @tblDonVi cn on qhd.ID_DonVi= cn.ID_DonVi
					left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang= tk.ID
					left join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi= ktc.ID
					where qhd.NgayLapHoaDon < @DateFrom
					and qct.HinhThucThanhToan not in (4,5,6)
					and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
					and (@ID_TaiKhoanNganHang  ='%%' Or qct.ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang)
					and (@ID_KhoanThuChi ='%%' or qct.ID_KhoanThuChi like @ID_KhoanThuChi)				
			) a
			 group by a.ID_HoaDon, a.MaHoaDon, 
				a.LoaiHoaDon,
				a.HachToanKinhDoanh, a.PhieuDieuChinhCongNo, a.NoiDungThu,
				a.ID_NhanVienPT , a.TrangThai
		) tblQuy
		left join DM_DoiTuong dt on tblQuy.ID_DoiTuong = dt.ID
		left join NS_NhanVien nv on tblQuy.ID_NhanVien= nv.ID
	 ) tblView
	 where tblView.TrangThaiHachToan like '%'+ @TrangThaiHachToan + '%'
	 and tblView.MaHoaDon not like 'CB%'		
    	and tblView.TrangThaiSoQuy like '%'+ @TrangThaiSoQuy + '%'
    	and tblView.LoaiChungTu like '%'+ @LoaiChungTu + '%'
    	and ID_NhanVien like @ID_NhanVien
		and exists (select Loai from @tblLoai loai where LoaiSoQuy= loai.Loai)    
		and (MaHoaDon like @TxtSearch OR MaDoiTuong like @TxtSearch OR NguoiNopTien like @TxtSearch
		OR TenDoiTuong_KhongDau like @TxtSearch OR dbo.FUNC_ConvertStringToUnsign(NoiDungThu) like @TxtSearch)
		group by tblView.LoaiHoaDon
		) a1
		) b
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListCashFlow_Paging]
	@IDDonVis [nvarchar](max),
    @ID_NhanVien [nvarchar](40),
    @ID_NhanVienLogin [uniqueidentifier],
    @ID_TaiKhoanNganHang [nvarchar](40),
    @ID_KhoanThuChi [nvarchar](40),
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiSoQuy [nvarchar](15),	-- mat/nganhang/all
    @LoaiChungTu [nvarchar](2), -- thu/chi
    @TrangThaiSoQuy [nvarchar](2),
    @TrangThaiHachToan [nvarchar](2),
    @TxtSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int],
	@LoaiNapTien [nvarchar](15) -- 11.tiencoc, 10. khongphai tiencoc, 1.all
AS
BEGIN

	SET NOCOUNT ON;

   SET NOCOUNT ON;
	declare @isNullSearch int = 1
	if isnull(@TxtSearch,'')='' OR @TxtSearch ='%%'
		begin
			set @isNullSearch =0 
			set @TxtSearch ='%%'
		end
	else
		set @TxtSearch= CONCAT(N'%',@TxtSearch, '%')

	declare @tblChiNhanh table (ID uniqueidentifier)
    insert into @tblChiNhanh
	select name from dbo.splitstring(@IDDonVis)

	--declare #tblQuyHD table (ID uniqueidentifier, MaHoaDon nvarchar(40), NgayLapHoaDon datetime, ID_DonVi uniqueidentifier,
	--LoaiHoaDon int, NguoiTao nvarchar(100), HachToanKinhDoanh bit, PhieuDieuChinhCongNo int,
	--NoiDungThu nvarchar(max), ID_NhanVienPT uniqueidentifier, TrangThai bit)

	--insert into #tblQuyHD
	select qhd.ID,
		qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.ID_DonVi, qhd.LoaiHoaDon, qhd.NguoiTao,
    	qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo, qhd.NoiDungThu,
    	qhd.ID_NhanVien as ID_NhanVienPT, qhd.TrangThai	
	into #tblQuyHD
	from Quy_HoaDon qhd	
	where qhd.NgayLapHoaDon between  @DateFrom and  @DateTo		
	and qhd.ID_DonVi in (select * from dbo.splitstring(@IDDonVis))
	and(qhd.PhieuDieuChinhCongNo != '1' or qhd.PhieuDieuChinhCongNo is null)


    	declare @nguoitao nvarchar(100) = (select taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDDonVis,'SoQuy_XemDS_PhongBan','SoQuy_XemDS_HeThong');
    	
    	with data_cte
    	as(

    select tblView.*
    	from
    		(
			select 
    			tblQuy.ID,
    			tblQuy.MaHoaDon,
    			tblQuy.NgayLapHoaDon,
    			tblQuy.ID_DonVi,
    			tblQuy.LoaiHoaDon,
    			tblQuy.NguoiTao,
				ISNUll(nv2.TenNhanVien,'') as TenNhanVien,
				ISNUll(nv2.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
			
				ISNUll(dv.TenDonVi,'') as TenChiNhanh,
				ISNUll(dv.SoDienThoai,'') as DienThoaiChiNhanh,
				ISNUll(dv.DiaChi,'') as DiaChiChiNhanh,
				ISNUll(nguon.TenNguonKhach,'') as TenNguonKhach,
    			ISNUll(tblQuy.TrangThai,'1') as TrangThai,
    			tblQuy.NoiDungThu,
				iif(@isNullSearch=0, dbo.FUNC_ConvertStringToUnsign(NoiDungThu), tblQuy.NoiDungThu) as NoiDungThuUnsign,
    			tblQuy.PhieuDieuChinhCongNo,
    			tblQuy.ID_NhanVienPT as ID_NhanVien,
    			iif(LoaiHoaDon=11, TienMat,0) as ThuMat,
    			iif(LoaiHoaDon=12, TienMat,0) as ChiMat,
    			iif(LoaiHoaDon=11, TienGui,0) as ThuGui,
    			iif(LoaiHoaDon=12, TienGui,0) as ChiGui,
    			TienMat + TienGui as TienThu,
    			TienMat + TienGui as TongTienThu,
				TienGui,
				TienMat, 
				ChuyenKhoan, 
				TienPOS,
				TienDoiDiem, 
				TTBangTienCoc,
				TienTheGiaTri,
    			TenTaiKhoanPOS, TenTaiKhoanNOTPOS,
    			cast(ID_TaiKhoanNganHang as varchar(max)) as ID_TaiKhoanNganHang,
    			ID_KhoanThuChi,
    			NoiDungThuChi,
				tblQuy.ID_NhanVienPT,
				dt.ID_NguonKhach,
				isnull(dt.LoaiDoiTuong,0) as LoaiDoiTuong,
    			ISNULL(tblQuy.HachToanKinhDoanh,'1') as HachToanKinhDoanh,
    			case when tblQuy.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
    		case when tblQuy.HachToanKinhDoanh = '1' or tblQuy.HachToanKinhDoanh is null  then '11' else '10' end as TrangThaiHachToan,
    		case when tblQuy.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong  else nv.TenNhanVien end as NguoiNopTien,
			case when nv.TenNhanVien is null then  dt.DiaChi  else nv.DiaChiCoQuan end as DiaChiKhachHang,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong_KhongDau  else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
    		case when nv.MaNhanVien is null then dt.MaDoiTuong else  nv.MaNhanVien end as MaDoiTuong,
    		case when nv.MaNhanVien is null then dt.DienThoai else  nv.DienThoaiDiDong  end as SoDienThoai,
    			case when qct.TienMat > 0 then case when qct.TienGui > 0 then '2' else '1' end 
    			else case when qct.TienGui > 0 then '0'
    				else case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then '0' else '1' end end end as LoaiSoQuy,
    			-- check truong hop tongthu = 0
    		case when qct.TienMat > 0 then case when qct.TienGui > 0 then N'Tiền mặt, chuyển khoản' else N'Tiền mặt' end 
    			else case when qct.TienGui > 0 then N'Chuyển khoản' else 
    				case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then  N'Chuyển khoản' else N'Tiền mặt' end end end as PhuongThuc	
    							
    		from #tblQuyHD tblQuy
			 join 
    			(select 
    				 a.ID_hoaDon,
    				 sum(isnull(a.TienMat, 0)) as TienMat,
    				 sum(isnull(a.TienGui, 0)) as TienGui,
					 sum(isnull(a.TienPOS, 0)) as TienPOS,
					 sum(isnull(a.ChuyenKhoan, 0)) as ChuyenKhoan,
					 sum(isnull(a.TienDoiDiem, 0)) as TienDoiDiem,
					 sum(isnull(a.TienTheGiaTri, 0)) as TienTheGiaTri,
					 sum(isnull(a.TTBangTienCoc, 0)) as TTBangTienCoc,
    				 max(a.TenTaiKhoanPOS) as TenTaiKhoanPOS,
    				 max(a.TenTaiKhoanNOPOS) as TenTaiKhoanNOTPOS,
    				 max(a.ID_DoiTuong) as ID_DoiTuong,
    				 max(a.ID_NhanVien) as ID_NhanVien,
    				 max(a.ID_TaiKhoanNganHang) as ID_TaiKhoanNganHang,
    				 max(a.ID_KhoanThuChi) as ID_KhoanThuChi,
    				 max(a.NoiDungThuChi) as NoiDungThuChi
    			from
    			(
    				select *
    				from(
    					select 
    					qct.ID_HoaDon,
						iif(qct.HinhThucThanhToan= 1, qct.TienThu,0) as TienMat,
						iif(qct.HinhThucThanhToan= 2 or qct.HinhThucThanhToan = 3, qct.TienThu,0) as TienGui,			
						iif(qct.HinhThucThanhToan= 2, qct.TienThu,0) as TienPOS,
						iif(qct.HinhThucThanhToan= 3, qct.TienThu,0) as ChuyenKhoan,
						iif(qct.HinhThucThanhToan= 4, qct.TienThu,0) as TienDoiDiem,
						iif(qct.HinhThucThanhToan= 5, qct.TienThu,0) as TienTheGiaTri,
						iif(qct.HinhThucThanhToan= 6, qct.TienThu,0) as TTBangTienCoc,						
						qct.ID_DoiTuong, qct.ID_NhanVien, 
    					ISNULL(qct.ID_TaiKhoanNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
    					ISNULL(qct.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi,
    					case when tk.TaiKhoanPOS='1' then IIF(tk.TrangThai = 0, '<span style=""color: red; text - decoration: line - through; "" title=""Đã xóa"">' + tk.TenChuThe + '</span>', tk.TenChuThe) else '' end as TenTaiKhoanPOS,
    					case when tk.TaiKhoanPOS = '0' then IIF(tk.TrangThai = 0, '<span style=""color:red;text-decoration: line-through;"" title=""Đã xóa"">' + tk.TenChuThe + '</span>', tk.TenChuThe) else '' end as TenTaiKhoanNOPOS,
    					iif(ktc.NoiDungThuChi is null, '',
                        iif(ktc.TrangThai = 0, concat(ktc.NoiDungThuChi, '{DEL}'), ktc.NoiDungThuChi)) as NoiDungThuChi,
						----11.coc, 13.khongbutru congno, 10.khong coc

                        iif(qct.LoaiThanhToan = 1, '11', iif(qct.LoaiThanhToan = 3, '13', '10')) as LaTienCoc, 
						IIF(qct.ID_HoaDonLienQuan IS NULL AND qct.ID_KhoanThuChi IS NULL, 1, 0) AS LaThuChiMacDinh

                        from #tblQuyHD  qhd		
						left join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon = qhd.ID

                        left
                        join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID


                   left
                        join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi = ktc.ID

                        where qct.HinhThucThanhToan not in (4, 5, 6)-- diem, thegiatri, coc
    					)qct
                    where qct.ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang

                    and(qct.ID_KhoanThuChi like @ID_KhoanThuChi OR(qct.LaThuChiMacDinh = 1 AND @ID_KhoanThuChi = '00000000-0000-0000-0000-000000000001'))

                    and qct.LaTienCoc like '%' + @LoaiNapTien + '%'
    			) a group by a.ID_HoaDon
    		) qct on tblQuy.ID = qct.ID_HoaDon

            left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID

            left join NS_NhanVien nv on qct.ID_NhanVien = nv.ID

        left join DM_DonVi dv on tblQuy.ID_DonVi = dv.ID

        left join NS_NhanVien nv2 on tblQuy.ID_NhanVienPT = nv2.ID

        left join DM_NguonKhachHang nguon on dt.ID_NguonKhach = nguon.ID
    	 ) tblView

         where tblView.TrangThaiHachToan like '%' + @TrangThaiHachToan + '%'


        and tblView.TrangThaiSoQuy like '%' + @TrangThaiSoQuy + '%'


        and tblView.LoaiChungTu like '%' + @LoaiChungTu + '%'



            and tblView.ID_NhanVienPT like @ID_NhanVien


            and(exists(select ID from @tblNhanVien nv where tblView.ID_NhanVienPT = nv.ID) or tblView.NguoiTao like @nguoitao)


        and exists(select Name from dbo.splitstring(@LoaiSoQuy) where LoaiSoQuy= Name)

            and(MaHoaDon like @TxtSearch

            OR MaDoiTuong like @TxtSearch

            OR NguoiNopTien like @TxtSearch

            OR SoDienThoai like @TxtSearch

            OR TenNhanVien like @TxtSearch--nvlap

            OR TenNhanVienKhongDau like @TxtSearch

            OR TenDoiTuong_KhongDau like @TxtSearch-- nguoinoptien

            OR NoiDungThuUnsign like @TxtSearch
            )
    	),
    	count_cte
        as (
        select count(dt.ID) as TotalRow,
    		CEILING(count(dt.ID) / cast(@PageSize as float)) as TotalPage,
    		sum(ThuMat) as TongThuMat,
    		sum(ChiMat) as TongChiMat,
    		sum(ThuGui) as TongThuCK,
    		sum(ChiGui) as TongChiCK


        from data_cte dt
    	)
    	select*
        from data_cte dt


        cross join count_cte
        order by dt.NgayLapHoaDon desc


        OFFSET(@CurrentPage * @PageSize) ROWS
        FETCH NEXT @PageSize ROWS ONLY
END
");

            Sql(@"ALTER PROCEDURE [dbo].[GetListComBo_ofCTHD]
    @ID_HoaDon [uniqueidentifier],
    @IDChiTiet [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @ID_DonVi uniqueidentifier = (select top 1 ID_DonVi from BH_HoaDon where ID= @ID_HoaDon)
    
    	select ctsd.ID_ChiTietGoiDV, sum(SoLuong) as SoLuongSuDung
    	into #tblSDDV 
    	from BH_HoaDon_ChiTiet ctsd
    	where exists (select ID from BH_HoaDon_ChiTiet ct where ct.ID_HoaDon= @ID_HoaDon and ct.ID_ChiTietGoiDV =  ctsd.ID_ChiTietGoiDV)
    	group by ctsd.ID_ChiTietGoiDV
    
    	select DISTINCT tbl.*, 
    		tbl.SoLuong as SoLuongMua,		
    		isnull(ctt.SoLuongTra,0) as SoLuongTra,
    		isnull(ctt.SoLuongDung,0) as SoLuongDVDaSuDung,
    		tbl.SoLuong -isnull(ctt.SoLuongTra,0) - isnull(ctt.SoLuongDung,0) as SoLuongDVConLai,--- use when print
    		tbl.SoLuong -isnull(ctt.SoLuongTra,0) - isnull(ctt.SoLuongDung,0) as SoLuongConLai -- use when trahang
    		
    		FROM 
    		(
    			SELECT
    				ct.ID,ct.ID_HoaDon,DonGia,ct.GiaVon,SoLuong,ThanhTien,ThanhToan,ct.ID_DonViQuiDoi, ct.ID_ChiTietDinhLuong, ct.ID_ChiTietGoiDV,
    				ct.TienChietKhau AS GiamGia,PTChietKhau,ct.GhiChu,ct.TienChietKhau,
    				(ct.DonGia - ct.TienChietKhau) as GiaBan,
					qd.GiaBan as GiaBanHH, --- used to nhaphang from hoadon
    				CAST(SoThuTu AS float) AS SoThuTu,ct.ID_KhuyenMai, ISNULL(ct.TangKem,'0') as TangKem, ct.ID_TangKem,
    					-- replace char enter --> char space
    				(REPLACE(REPLACE(TenHangHoa,CHAR(13),''),CHAR(10),'') +
    				CASE WHEN (qd.ThuocTinhGiaTri is null or qd.ThuocTinhGiaTri = '') then '' else '_' + qd.ThuocTinhGiaTri end +
    				CASE WHEN TenDonVitinh = '' or TenDonViTinh is null then '' else ' (' + TenDonViTinh + ')' end +
    				CASE WHEN MaLoHang is null then '' else '. Lô: ' + MaLoHang end) as TenHangHoaFull,
    				
    				hh.ID AS ID_HangHoa,
					hh.LaHangHoa,
					hh.QuanLyTheoLoHang,
				
					iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa,ct.TenHangHoaThayThe) as TenHangHoa,
    				ISNULL(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
    				ISNULL(ID_NhomHang,'00000000-0000-0000-0000-000000000000') as ID_NhomHangHoa,	
    				TenDonViTinh,MaHangHoa,YeuCau,
    				lo.ID AS ID_LoHang,
    					ISNULL(MaLoHang,'') as MaLoHang,
    					lo.NgaySanXuat, lo.NgayHetHan,
    					qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    					ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, 
    					CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
    					CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
    					CAST(ISNULL(ct.PTThue,0) as float) as PTThue,
    					CAST(ISNULL(ct.TienThue,0) as float) as TienThue,
    					CAST(ISNULL(ct.ThoiGianBaoHanh,0) as float) as ThoiGianBaoHanh,
    					CAST(ISNULL(ct.LoaiThoiGianBH,0) as float) as LoaiThoiGianBH,
    					Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
    					Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end LaPTPhiDichVu,
    					CAST(0 as float) as TongPhiDichVu, -- set default PhiDichVu = 0 (caculator again .js)
    					CAST(ISNULL(ct.Bep_SoLuongYeuCau,0) as float) as Bep_SoLuongYeuCau,
    					CAST(ISNULL(ct.Bep_SoLuongHoanThanh,0) as float) as Bep_SoLuongHoanThanh, -- view in CTHD NhaHang
    					CAST(ISNULL(ct.Bep_SoLuongChoCungUng,0) as float) as Bep_SoLuongChoCungUng,
    					ISNULL(hh.SoPhutThucHien,0) as SoPhutThucHien, -- lay so phut theo cai dat
    					ISNULL(ct.ThoiGianThucHien,0)  as ThoiGianThucHien,-- sophut thuc te thuchien	
    					ISNULL(ct.QuaThoiGian,0)  as QuaThoiGian,
    				
    					case when hh.LaHangHoa='0' then 0 else ISNULL(tk.TonKho,0) end as TonKho,
    					ct.ID_ViTri,
    					ISNULL(vt.TenViTri,'') as TenViTri,			
    					ct.ThoiGian,
						ct.ThoiGianHoanThanh, ISNULL(hh.GhiChu,'') as GhiChuHH,
    					ISNULL(ct.DiemKhuyenMai,0) as DiemKhuyenMai,
    					ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
    					ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
						ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
    					ct.ChatLieu,
    					isnull(ct.DonGiaBaoHiem,0) as DonGiaBaoHiem,
    					isnull(ct.TenHangHoaThayThe,hh.TenHangHoa) as TenHangHoaThayThe,
    					ct.ID_LichBaoDuong,
    					iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
    					ct.ID_ParentCombo
    					
    		FROM BH_HoaDon hd
    		JOIN BH_HoaDon_ChiTiet ct ON hd.ID= ct.ID_HoaDon
    		JOIN DonViQuiDoi qd ON ct.ID_DonViQuiDoi = qd.ID
    		JOIN DM_HangHoa hh ON qd.ID_HangHoa= hh.ID    		
    		left JOIN DM_NhomHangHoa nhh ON hh.ID_NhomHang= nhh.ID    							
    		LEFT JOIN DM_LoHang lo ON ct.ID_LoHang = lo.ID
    			left join DM_HangHoa_TonKho tk on ct.ID_DonViQuiDoi= tk.ID_DonViQuyDoi and tk.ID_DonVi= @ID_DonVi
    			left join DM_ViTri vt on ct.ID_ViTri= vt.ID
    		-- chi get CT khong phai la TP dinh luong
    		WHERE ct.ID_HoaDon = @ID_HoaDon
    				and ct.ID_ParentCombo like @IDChiTiet
    					and ct.ID_ParentCombo is not null
    					and ct.ID_ParentCombo != ct.ID
    					and ((tk.ID_DonVi = hd.ID_DonVi and hh.LaHangHoa='1') 
    					or tk.ID_DonVi is null
    					or (hh.LaHangHoa='0'))
    					and (ct.ID_LoHang= tk.ID_LoHang OR (ct.ID_LoHang is null and tk.ID_LoHang is null)) 
    			) tbl
    			left join 
    		(
    			select a.ID_ChiTietGoiDV,
    				SUM(a.SoLuongTra) as SoLuongTra,
    				SUM(a.SoLuongDung) as SoLuongDung
    			from
    				(-- sum soluongtra
    				select ct.ID_ChiTietGoiDV,
    					SUM(ct.SoLuong) as SoLuongTra,
    					0 as SoLuongDung
    				from BH_HoaDon_ChiTiet ct 
    				join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    				where hd.ChoThanhToan= 0 and hd.LoaiHoaDon = 6
    				and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    				group by ct.ID_ChiTietGoiDV
    
    				union all
    				-- sum soluong sudung
    				select ct.ID_ChiTietGoiDV,
    					0 as SoLuongDung,
    					SUM(ct.SoLuong) as SoLuongDung
    				from BH_HoaDon_ChiTiet ct 
    				join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    				where hd.ChoThanhToan=0 and hd.LoaiHoaDon in (1,25)
    				and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    				group by ct.ID_ChiTietGoiDV
    				) a group by a.ID_ChiTietGoiDV
    	) ctt on tbl.ID = ctt.ID_ChiTietGoiDV
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListCustomer_byIDs]
    @IDCustomers [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ID, MaDoiTuong, TenDoiTuong, DienThoai, Email , ISNULL(TongTichDiem,0) as TongTichDiem
	from DM_DoiTuong
	where exists (select Name from dbo.splitstring(@IDCustomers) where ID= Name)
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListDatLich]
    @IdChiNhanhs [nvarchar](max),
    @ThoiGianFrom [datetime],
    @ThoiGianTo [datetime],
    @TrangThais [nvarchar](20),
    @TextSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    	declare @tbTrangThai table (GiaTri varchar(2))
    	insert into @tbTrangThai
    	select Name from dbo.splitstring(@TrangThais);
    -- Insert statements for procedure here
    	if(@PageSize != 0)
    	BEGIN
    		with data_cte
    	as
    	(
    	SELECT dl.Id, dl.ThoiGian, dl.IDDoiTuong AS IdKhachHang, ISNULL(dt.MaDoiTuong, '') AS MaKhachHang,
    	ISNULL(dt.DienThoai, dl.SoDienThoai) AS SoDienThoai, ISNULL(dt.TenDoiTuong, dl.TenDoiTuong) AS TenKhachHang, ISNULL(dt.DiaChi, dl.DiaChi) AS DiaChi,
    	ISNULL(dt.NgaySinh_NgayTLap, dl.NgaySinh) AS NgaySinh, dl.IDXe, ISNULL(xe.BienSo, dl.BienSo) AS BienSo, dl.LoaiXe AS MauXe, dl.TrangThai, 
		dv.TenDonVi AS TenChiNhanh, dv.ID AS IdDonVi, dv.MaDonVi AS MaChiNhanh
    	FROM CSKH_DatLich dl
    	INNER JOIN @tblDonVi donvi ON dl.IDDonVi = donvi.ID_DonVi
    	INNER JOIN DM_DonVi dv ON donvi.ID_DonVi = dv.ID
    	LEFT JOIN DM_DoiTuong dt ON dt.ID = dl.IDDoiTuong
    	LEFT JOIN Gara_DanhMucXe xe ON xe.ID = dl.IDXe
    	WHERE exists (select GiaTri from @tbTrangThai tt where dl.TrangThai = tt.GiaTri)
    	AND dl.ThoiGian BETWEEN @ThoiGianFrom AND @ThoiGianTo
    	AND ((select count(Name) from @tblSearch b where     			
    		dl.TenDoiTuong like '%'+b.Name+'%'
    		or dl.SoDienThoai like '%'+b.Name+'%'
    		or dl.DiaChi like '%'+b.Name+'%'		
    		or dl.BienSo like '%'+b.Name+'%'
    		or dl.LoaiXe like '%'+b.Name+'%'
    		or dl.NgaySinh like '%'+b.Name+'%'
    		or dv.TenDonVi like '%'+b.Name+'%'
    		)=@count or @count=0)
    			), count_cte
    		as
    		(
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    		SELECT dt.*, ct.* FROM data_cte dt
    		CROSS JOIN count_cte ct
    		ORDER BY dt.ThoiGian desc
    		OFFSET (@CurrentPage * @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY;
    		END
    		ELSE
    		BEGIN
    	with data_cte
    	as
    	(
    	SELECT dl.Id, dl.ThoiGian, dl.IDDoiTuong AS IdKhachHang, ISNULL(dt.MaDoiTuong, '') AS MaKhachHang,
    	ISNULL(dt.DienThoai, dl.SoDienThoai) AS SoDienThoai, ISNULL(dt.TenDoiTuong, dl.TenDoiTuong) AS TenKhachHang, ISNULL(dt.DiaChi, dl.DiaChi) AS DiaChi,
    	ISNULL(dt.NgaySinh_NgayTLap, dl.NgaySinh) AS NgaySinh, dl.IDXe, ISNULL(xe.BienSo, dl.BienSo) AS BienSo, dl.LoaiXe AS MauXe, dl.TrangThai, 
		dv.TenDonVi AS TenChiNhanh, dv.ID AS IdDonVi, dv.MaDonVi AS MaChiNhanh
    	FROM CSKH_DatLich dl
    	INNER JOIN @tblDonVi donvi ON dl.IDDonVi = donvi.ID_DonVi
    	INNER JOIN DM_DonVi dv ON donvi.ID_DonVi = dv.ID
    	LEFT JOIN DM_DoiTuong dt ON dt.ID = dl.IDDoiTuong
    	LEFT JOIN Gara_DanhMucXe xe ON xe.ID = dl.IDXe
    	WHERE exists (select GiaTri from @tbTrangThai tt where dl.TrangThai = tt.GiaTri)
    	AND dl.ThoiGian BETWEEN @ThoiGianFrom AND @ThoiGianTo
    	AND ((select count(Name) from @tblSearch b where     			
    		dl.TenDoiTuong like '%'+b.Name+'%'
    		or dl.SoDienThoai like '%'+b.Name+'%'
    		or dl.DiaChi like '%'+b.Name+'%'		
    		or dl.BienSo like '%'+b.Name+'%'
    		or dl.LoaiXe like '%'+b.Name+'%'
    		or dl.NgaySinh like '%'+b.Name+'%'
    		or dv.TenDonVi like '%'+b.Name+'%'
    		)=@count or @count=0)
    			)
    			SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
    			ORDER BY dt.ThoiGian desc;
    			END
END");

            Sql(@"ALTER PROCEDURE [dbo].[getListHangHoaBy_IDNhomHang]
    @ID_NhomHang [nvarchar](max),
	@ID_DonVi [uniqueidentifier],
	@STT [int]
AS
BEGIN
    select 
		CAST(ROUND(ROW_NUMBER() over (order by dvqd.MaHangHoa), 0) + @STT as float) as SoThuTu,
    	dvqd.ID as ID_DonViQuiDoi,
		lh.ID as ID_LoHang,
    	dvqd.MaHangHoa,
		case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    	hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
    	hh.TenHangHoa,
    	dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd.TenDonViTinh,
		Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
		Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
		Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan,
    	Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVonHienTai,
    	Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVonMoi,
    	cast(0 as float)as GiaVonTang,
    	cast(0 as float) as GiaVonGiam
    	FROM 
    	DonViQuiDoi dvqd 
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on hh.ID = lh.ID_HangHoa
		left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or gv.ID_LoHang is null) and gv.ID_DonVi = @ID_DonVi)
    	where hh.ID_NhomHang in (select * from splitstring(@ID_NhomHang))
    		and dvqd.Xoa = '0'
			and dvqd.LaDonViChuan = 1
    		and hh.TheoDoi = 1
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListHoaDonSuaChua]
    @IDChiNhanhs [nvarchar](max),
    @FromDate [nvarchar](14),
    @ToDate [nvarchar](14),
    @ID_PhieuSuaChua [nvarchar](max),
    @IDXe [uniqueidentifier],
    @TrangThais [nvarchar](20),
    @TextSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	if @FromDate = '2016-01-01' 
    		set @ToDate= (select format(DATEADD(day,1, max(NgayLapHoaDon)),'yyyy-MM-dd') from BH_HoaDon where LoaiHoaDon= 25)
    
    	declare @tblDonVi table (ID_DonVi uniqueidentifier)
    	if(@IDChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IDChiNhanhs)
    	END
    	ELSE
    	BEGIN
    		INSERT INTO @tblDonVi
    		SELECT ID FROM DM_DonVi;
    	END
    
    	declare @tbTrangThai table (GiaTri varchar(2))
    	insert into @tbTrangThai
    	select Name from dbo.splitstring(@TrangThais)
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);	
    	if(@PageSize != 0)
    	BEGIN
    	with data_cte
    	as
    	(
    			select *
    			from
    			(
    			select hd.ID,
					 hd.ID_DonVi,
					 hd.NguoiTao, hd.ID_NhanVien,
					 hd.loaihoadon,hd.MaHoaDon, hd.ID_HoaDon, hd.ID_DoiTuong, hd.NgayLapHoaDon, 

					  hd.SoVuBaoHiem, 
					  isnull(hd.TongTienBHDuyet,0) as TongTienBHDuyet, 
					 isnull(hd.PTThueHoaDon,0) as PTThueHoaDon, 
					  isnull(hd.PTThueBaoHiem,0) as PTThueBaoHiem, 
					  isnull(hd.TongTienThueBaoHiem,0) as TongTienThueBaoHiem, 
					   isnull(hd.KhauTruTheoVu,0) as KhauTruTheoVu, 
					   
					isnull(hd.TongThueKhachHang,0) as  TongThueKhachHang,
					isnull(hd.CongThucBaoHiem,0) as  CongThucBaoHiem,
					isnull(hd.GiamTruThanhToanBaoHiem,0) as  GiamTruThanhToanBaoHiem,

					    isnull(hd.PTGiamTruBoiThuong,0) as PTGiamTruBoiThuong, 
					 isnull(hd.GiamTruBoiThuong,0) as GiamTruBoiThuong, 
					  isnull(hd.BHThanhToanTruocThue,0) as BHThanhToanTruocThue, 
					  isnull(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem, 													
					hd.ID_CheckIn, 
					hd.DienGiai,
					 hd.NgaySua, hd.NgayTao, hd.ID_PhieuTiepNhan,
					hd.ID_BaoHiem, hd.TongTienHang, hd.PhaiThanhToan, hd.TongThanhToan, hd.TongGiamGia,
					hd.TongTienThue,
					isnull(hd.ChiPhi,0) as TongChiPhi ,
					hd.TongChietKhau,
					hd.ChoThanhToan, 
					hd.YeuCau,
					xe.BienSo,
    				tn.MaPhieuTiepNhan,
    				dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai as DienThoaiKhachHang, dt.Email, dt.DiaChi, 
					dt.MaSoThue,
					dt.TaiKhoanNganHang,
					iif(hd.ID_BaoHiem is null,'',tn.NguoiLienHeBH) as LienHeBaoHiem,
					iif(hd.ID_BaoHiem is null,'',tn.SoDienThoaiLienHeBH) as SoDienThoaiLienHeBaoHiem,
    				ISNULL(bg.MaHoaDon,'') as MaBaoGia,

    				case hd.ChoThanhToan
    					when 0 then '0'
    					when 1 then '1'
    					else '2' end as TrangThai,
    				case hd.ChoThanhToan
    					when 0 then N'Hoàn thành'
    					when 1 then N'Phiếu tạm'
    					else N'Đã hủy'
    					end as TrangThaiText
    			from BH_HoaDon hd
    			join Gara_PhieuTiepNhan tn on tn.ID= hd.ID_PhieuTiepNhan
				left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
    			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			left join BH_HoaDon bg on hd.ID_HoaDon= bg.ID and bg.ID_PhieuTiepNhan= tn.ID
    			where hd.LoaiHoaDon= 25
    			and exists (select ID_DonVi from @tblDonVi dv where hd.ID_DonVi = dv.ID_DonVi)
    			and hd.NgayLapHoaDon >=@FromDate and hd.NgayLapHoaDon < @ToDate
    			and hd.ID_PhieuTiepNhan like @ID_PhieuSuaChua
    			and (@IDXe IS NULL OR @IDXe = tn.ID_Xe)
    			and
    				((select count(Name) from @tblSearch b where     			
    				hd.MaHoaDon like '%'+b.Name+'%'
    				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'
    				or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong like '%'+b.Name+'%'	
    				or dt.DienThoai like '%'+b.Name+'%'				
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
    				or hd.NguoiTao like '%'+b.Name+'%'										
    				)=@count or @count=0)	
    			) a
    			where exists (select GiaTri from @tbTrangThai tt where a.TrangThai = tt.GiaTri)
    	),
    	count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    
    		select dt.*, cte.*, 
    			dt.NguoiTao as NguoiTaoHD,
    			nv.TenNhanVien,
    			isnull(KhachDaTra,0) as KhachDaTra,
    			isnull(BaoHiemDaTra,0) as BaoHiemDaTra,
    			isnull(bh.TenDoiTuong,'') as TenBaoHiem,
    			isnull(bh.Email,'') as BH_Email,
    			isnull(bh.DiaChi,'') as BH_DiaChi,
    			isnull(bh.DienThoai,'') as DienThoaiBaoHiem,
				isnull(quy.Khach_TienMat,0) as Khach_TienMat,
				isnull(quy.Khach_TienPOS,0) as Khach_TienPOS,
				isnull(quy.Khach_TienCK,0) as Khach_TienCK,
				isnull(quy.Khach_TheGiaTri,0) as Khach_TheGiaTri,
				isnull(quy.Khach_TienDiem,0) as Khach_TienDiem,
				isnull(quy.Khach_TienCoc,0) as Khach_TienCoc,
				isnull(quy.BH_TienMat,0) as BH_TienMat,
				isnull(quy.BH_TienPOS,0) as BH_TienPOS,
				isnull(quy.BH_TienCK,0) as BH_TienCK,
				isnull(quy.BH_TheGiaTri,0) as BH_TheGiaTri,
				isnull(quy.BH_TienDiem,0) as BH_TienDiem,
				isnull(quy.BH_TienCoc,0) as BH_TienCoc
    		from data_cte dt
    		join NS_NhanVien nv on dt.ID_NhanVien= nv.ID
    		cross join count_cte cte
    		left join DM_DoiTuong bh on bh.ID= dt.ID_BaoHiem
    		left join
    		(
    		select a.ID, 
				sum(KhachDaTra) as KhachDaTra, 
				sum(BaoHiemDaTra) as BaoHiemDaTra,
				sum(Khach_TienMat) as Khach_TienMat, 
				sum(Khach_TienPOS) as Khach_TienPOS,
				sum(Khach_TienCK) as Khach_TienCK, 
				sum(Khach_TheGiaTri) as Khach_TheGiaTri,
				sum(Khach_TienDiem) as Khach_TienDiem,
				sum(Khach_TienCoc) as Khach_TienCoc,
				sum(BH_TienMat) as BH_TienMat, 
				sum(BH_TienPOS) as BH_TienPOS,
				sum(BH_TienCK) as BH_TienCK, 
				sum(BH_TheGiaTri) as BH_TheGiaTri,
				sum(BH_TienDiem) as BH_TienDiem,
				sum(BH_TienCoc) as BH_TienCoc
    		from
    		(
    			select cte.ID, 
					sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, -qct.TienThu)) as KhachDaTra,
					0 as BaoHiemDaTra,
					sum(iif(qct.HinhThucThanhToan=1, qct.TienThu, 0)) as Khach_TienMat,
					sum(iif(qct.HinhThucThanhToan=2, qct.TienThu, 0)) as Khach_TienPOS,
					sum(iif(qct.HinhThucThanhToan=3, qct.TienThu, 0)) as Khach_TienCK,
					sum(iif(qct.HinhThucThanhToan=4, qct.TienThu, 0)) as Khach_TheGiaTri,
					sum(iif(qct.HinhThucThanhToan=5, qct.TienThu, 0)) as Khach_TienDiem,
					sum(iif(qct.HinhThucThanhToan=6, qct.TienThu, 0)) as Khach_TienCoc,				
					0 as BH_TienMat,
					0 as BH_TienPOS,
					0 as BH_TienCK,
					0 as BH_TheGiaTri,
					0 as BH_TienDiem,
					0 as BH_TienCoc
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			join BH_HoaDon cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_DoiTuong
    			where qhd.TrangThai= 1
    			group by cte.ID
    
    			union all

				select 
					thuDH.ID,
					thuDH.TienThu as KhachDaTra,
					0 as BaoHiemDaTra,
					0 as Khach_TienMat,
					0 as Khach_TienPOS,
					0 as Khach_TienCK,
					0 as Khach_TheGiaTri,
					0 as Khach_TienDiem,
					0 as Khach_TienCoc,
					0 as BH_TienMat,
					0 as BH_TienPOS,
					0 as BH_TienCK,
					0 as BH_TheGiaTri,
					0 as BH_TienDiem,
					0 as BH_TienCoc
					from
					(	
						Select 
							ROW_NUMBER() OVER(PARTITION BY ID_HoaDonLienQuan ORDER BY NgayLapHoaDon ASC) AS isFirst,						
    						d.ID,
							ID_HoaDonLienQuan,
							d.NgayLapHoaDon,    								
    						sum(d.TienThu) as TienThu																		
    						FROM
    						(
									
									select cte.ID, cte.NgayLapHoaDon, qct.ID_HoaDonLienQuan,
									iif(qhd.LoaiHoaDon = 11,qct.TienThu, -qct.TienThu) as TienThu												
									from Quy_HoaDon_ChiTiet qct
									join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID											
									join BH_HoaDon hdd on hdd.ID= qct.ID_HoaDonLienQuan
									join data_cte cte on hdd.ID= cte.ID_HoaDon
									where hdd.LoaiHoaDon = '3' 	
									and qhd.TrangThai= 1										
    						) d group by d.ID,d.NgayLapHoaDon,ID_HoaDonLienQuan						
						) thuDH
					where isFirst= 1

				union all
    
    			select cte.ID, 
					0 as KhachDaTra, 
					sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, -qct.TienThu)) as BaoHiemDaTra,
					0 as Khach_TienMat,
					0 as Khach_TienPOS,
					0 as Khach_TienCK,
					0 as Khach_TheGiaTri,
					0 as Khach_TienDiem,
					0 as Khach_TienCoc,
					sum(iif(qct.HinhThucThanhToan=1, qct.TienThu, 0)) as BH_TienMat,
					sum(iif(qct.HinhThucThanhToan=2, qct.TienThu, 0)) as BH_TienPOS,
					sum(iif(qct.HinhThucThanhToan=3, qct.TienThu, 0)) as BH_TienCK,
					sum(iif(qct.HinhThucThanhToan=4, qct.TienThu, 0)) as BH_TheGiaTri,
					sum(iif(qct.HinhThucThanhToan=5, qct.TienThu, 0)) as BH_TienDiem,
					sum(iif(qct.HinhThucThanhToan=6, qct.TienThu, 0)) as BH_TienCoc
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			join data_cte cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_BaoHiem
    			where qhd.TrangThai= 1
    			group by cte.ID
    		) a
    		group by a.ID
    		) quy on dt.ID= quy.ID
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
    	END
    	ELSE
    	BEGIN
    	with data_cte
    	as
    	(
    			select *
    			from
    			(
    			select hd.*,
					xe.BienSo,
    				tn.MaPhieuTiepNhan,
    				dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai as DienThoaiKhachHang, dt.Email, dt.DiaChi,
    				ISNULL(bg.MaHoaDon,'') as MaBaoGia,
    				case hd.ChoThanhToan
    					when 0 then '0'
    					when 1 then '1'
    					else '2' end as TrangThai,
    				case hd.ChoThanhToan
    					when 0 then N'Hoàn thành'
    					when 1 then N'Phiếu tạm'
    					else N'Đã hủy'
    					end as TrangThaiText
    			from BH_HoaDon hd
    			join Gara_PhieuTiepNhan tn on tn.ID= hd.ID_PhieuTiepNhan
				left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
    			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			left join BH_HoaDon bg on hd.ID_HoaDon= bg.ID and bg.ID_PhieuTiepNhan= tn.ID
    			where hd.LoaiHoaDon= 25
    			and exists (select ID_DonVi from @tblDonVi dv where hd.ID_DonVi = dv.ID_DonVi)
    			and hd.NgayLapHoaDon >=@FromDate and hd.NgayLapHoaDon < @ToDate
    			and hd.ID_PhieuTiepNhan like @ID_PhieuSuaChua
    			and (@IDXe IS NULL OR @IDXe = tn.ID_Xe)
    			and
    				((select count(Name) from @tblSearch b where     			
    				hd.MaHoaDon like '%'+b.Name+'%'
    				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'
    				or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong like '%'+b.Name+'%'	
    				or dt.DienThoai like '%'+b.Name+'%'				
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
    				or hd.NguoiTao like '%'+b.Name+'%'										
    				)=@count or @count=0)	
    			) a
    			where exists (select GiaTri from @tbTrangThai tt where a.TrangThai = tt.GiaTri)
    	)
    
    		select dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage,
    			dt.NguoiTao as NguoiTaoHD,
    			nv.TenNhanVien,
    			isnull(KhachDaTra,0) as KhachDaTra,
    			isnull(BaoHiemDaTra,0) as BaoHiemDaTra,
    			isnull(bh.TenDoiTuong,'') as TenBaoHiem,
				isnull(bh.MaDoiTuong,'') as MaBaoHiem,
    			isnull(bh.Email,'') as BH_Email,
    			isnull(bh.DiaChi,'') as BH_DiaChi,
    			isnull(bh.DienThoai,'') as DienThoaiBaoHiem,

				isnull(quy.Khach_TienMat,0) as Khach_TienMat,
				isnull(quy.Khach_TienPOS,0) as Khach_TienPOS,
				isnull(quy.Khach_TienCK,0) as Khach_TienCK,
				isnull(quy.Khach_TheGiaTri,0) as Khach_TheGiaTri,
				isnull(quy.Khach_TienDiem,0) as Khach_TienDiem,
				isnull(quy.Khach_TienCoc,0) as Khach_TienCoc,
				isnull(quy.BH_TienMat,0) as BH_TienMat,
				isnull(quy.BH_TienPOS,0) as BH_TienPOS,
				isnull(quy.BH_TienCK,0) as BH_TienCK,
				isnull(quy.BH_TheGiaTri,0) as BH_TheGiaTri,
				isnull(quy.BH_TienDiem,0) as BH_TienDiem,
				isnull(quy.BH_TienCoc,0) as BH_TienCoc
    		from data_cte dt
    		join NS_NhanVien nv on dt.ID_NhanVien= nv.ID    		
    		left join DM_DoiTuong bh on bh.ID= dt.ID_BaoHiem
    		left join
    		(
    		select a.ID,
				sum(KhachDaTra) as KhachDaTra, 
				sum(BaoHiemDaTra) as BaoHiemDaTra,
				sum(Khach_TienMat) as Khach_TienMat, 
				sum(Khach_TienPOS) as Khach_TienPOS,
				sum(Khach_TienCK) as Khach_TienCK, 
				sum(Khach_TheGiaTri) as Khach_TheGiaTri,
				sum(Khach_TienDiem) as Khach_TienDiem,
				sum(Khach_TienCoc) as Khach_TienCoc,
				sum(BH_TienMat) as BH_TienMat, 
				sum(BH_TienPOS) as BH_TienPOS,
				sum(BH_TienCK) as BH_TienCK, 
				sum(BH_TheGiaTri) as BH_TheGiaTri,
				sum(BH_TienDiem) as BH_TienDiem,
				sum(BH_TienCoc) as BH_TienCoc
    		from
    		(
    			select cte.ID,
					sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, -qct.TienThu)) as KhachDaTra,
					0 as BaoHiemDaTra,
					sum(iif(qct.HinhThucThanhToan=1, qct.TienThu, 0)) as Khach_TienMat,
					sum(iif(qct.HinhThucThanhToan=2, qct.TienThu, 0)) as Khach_TienPOS,
					sum(iif(qct.HinhThucThanhToan=3, qct.TienThu, 0)) as Khach_TienCK,
					sum(iif(qct.HinhThucThanhToan=4, qct.TienThu, 0)) as Khach_TheGiaTri,
					sum(iif(qct.HinhThucThanhToan=5, qct.TienThu, 0)) as Khach_TienDiem,
					sum(iif(qct.HinhThucThanhToan=6, qct.TienThu, 0)) as Khach_TienCoc,				
					0 as BH_TienMat,
					0 as BH_TienPOS,
					0 as BH_TienCK,
					0 as BH_TheGiaTri,
					0 as BH_TienDiem,
					0 as BH_TienCoc
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			join BH_HoaDon cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_DoiTuong
    			where qhd.TrangThai= 1
    			group by cte.ID
    
    			union all
    
    			select cte.ID, 
					0 as KhachDaTra, 
					sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, -qct.TienThu)) as BaoHiemDaTra,
					0 as Khach_TienMat,
					0 as Khach_TienPOS,
					0 as Khach_TienCK,
					0 as Khach_TheGiaTri,
					0 as Khach_TienDiem,
					0 as Khach_TienCoc,
					sum(iif(qct.HinhThucThanhToan=1, qct.TienThu, 0)) as BH_TienMat,
					sum(iif(qct.HinhThucThanhToan=2, qct.TienThu, 0)) as BH_TienPOS,
					sum(iif(qct.HinhThucThanhToan=3, qct.TienThu, 0)) as BH_TienCK,
					sum(iif(qct.HinhThucThanhToan=4, qct.TienThu, 0)) as BH_TheGiaTri,
					sum(iif(qct.HinhThucThanhToan=5, qct.TienThu, 0)) as BH_TienDiem,
					sum(iif(qct.HinhThucThanhToan=6, qct.TienThu, 0)) as BH_TienCoc
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			join data_cte cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_BaoHiem
    			where qhd.TrangThai= 1
    			group by cte.ID
    		) a
    		group by a.ID
    		) quy on dt.ID= quy.ID
    		order by dt.NgayLapHoaDon desc
    		
    	END
END");

            CreateStoredProcedure(name: "[dbo].[GetListImgInvoice_byCus]", parametersAction: p => new
            {
                TextSearch = p.String(),
                ID_Customer = p.String(),
                CurrentPage = p.Int(),
                PageSize = p.Int()
            }, body: @"SET NOCOUNT ON;

	if @TextSearch is null set @TextSearch='%%'
	else set @TextSearch = CONCAT(N'%',@TextSearch,'%');

	---- get listhoadon
	select distinct hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.DienGiai as GhiChu
	into #tblHD
		from BH_HoaDon_Anh anh
		join BH_HoaDon hd on hd.ID= anh.IdHoaDon
		where hd.ID_DoiTuong like @ID_Customer
		and hd.ChoThanhToan='0'
		and 
			(hd.MaHoaDon like @TextSearch
			or hd.DienGiai like @TextSearch);

		

	---- get list gdv
	select ctsd.*, gdv.MaHoaDon
	into #tblGDV
	from
	(
		select ctsd.ID_ChiTietGoiDV, ctsd.ID_HoaDon
		from BH_HoaDon_ChiTiet ctsd	
		where exists(select id from #tblHD hd where ctsd.ID_HoaDon = hd.ID)
	) ctsd
	join BH_HoaDon_ChiTiet ctm on ctsd.ID_ChiTietGoiDV= ctm.ID
	join BH_HoaDon gdv on ctm.ID_HoaDon= gdv.ID;


		with data_cte
		as					
		(
			select distinct hdsd.*, gdv.MaHoaDon as MaPhieuThu ---- muon tam truong
			from #tblHD hdsd
			left join #tblGDV gdv on hdsd.ID= gdv.ID_HoaDon			
	   ),
	   count_cte
	   as
	   (
			select 
				count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
			from data_cte
	   )
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY; ");

            Sql(@"ALTER PROCEDURE [dbo].[GetListPhieuTiepNhan_v2]
    @IdChiNhanhs [nvarchar](max),
    @NgayTiepNhan_From [datetime],
    @NgayTiepNhan_To [datetime],
    @NgayXuatXuongDuKien_From [datetime],
    @NgayXuatXuongDuKien_To [datetime],
    @NgayXuatXuong_From [datetime],
    @NgayXuatXuong_To [datetime],
    @TrangThais [nvarchar](20),
    @TextSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int],
	@BaoHiem int
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    	declare @tbTrangThai table (GiaTri varchar(2))
    	insert into @tbTrangThai
    	select Name from dbo.splitstring(@TrangThais);
    	if(@PageSize != 0)
    	BEGIN
    		with data_cte
    	as
    	(
    	select ptn.ID, ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, ptn.SoKmVao, ptn.NgayXuatXuongDuKien, ptn.NgayXuatXuong, ptn.TrangThai,
    	ptn.SoKmRa, ptn.TenLienHe, ptn.SoDienThoaiLienHe, ptn.GhiChu, ptn.TrangThai AS TrangThaiPhieuTiepNhan,
    	ptn.ID_Xe, dmx.BienSo, dmx.SoMay, dmx.SoKhung, dmx.NamSanXuat, mauxe.TenMauXe, hangxe.TenHangXe, loaixe.TenLoaiXe,
    	ptn.ID_KhachHang, dt.MaDoiTuong, dt.TenDoiTuong, dt.Email, dt.DienThoai AS DienThoaiKhachHang, dt.DiaChi,
    	ptn.ID_CoVanDichVu, ISNULL(nvcovan.TenNhanVien, '') AS CoVanDichVu, ISNULL(nvcovan.MaNhanVien, '') AS MaCoVan, nvcovan.DienThoaiDiDong AS CoVan_SDT,
    	ptn.ID_NhanVien, nvtiepnhan.MaNhanVien AS MaNhanVienTiepNhan, nvtiepnhan.TenNhanVien AS NhanVienTiepNhan,
    	dmx.DungTich, dmx.MauSon, dmx.HopSo,
    	cast(iif(dmx.ID_KhachHang = ptn.ID_KhachHang,'1','0') as bit) as LaChuXe,
		cx.ID as ID_ChuXe,
    	cx.TenDoiTuong as ChuXe,
    	cx.DienThoai as ChuXe_SDT, cx.DiaChi as ChuXe_DiaChi, cx.Email as ChuXe_Email,
		bh.TenDoiTuong as TenBaoHiem, bh.MaDoiTuong as MaBaoHiem, ptn.NguoiLienHeBH, ptn.SoDienThoaiLienHeBH,
    	dv.MaDonVi, dv.TenDonVi,
    	ptn.NgayTao, ptn.ID_BaoHiem, ptn.ID_DonVi
    	from Gara_PhieuTiepNhan ptn
    	inner join Gara_DanhMucXe dmx on ptn.ID_Xe = dmx.ID
    	LEFT join DM_DoiTuong cx on dmx.ID_KhachHang = cx.ID
    	inner join DM_DoiTuong dt on dt.ID = ptn.ID_KhachHang
		left join DM_DoiTuong bh on ptn.ID_BaoHiem = bh.ID
    	left join NS_NhanVien nvcovan on nvcovan.ID = ptn.ID_CoVanDichVu
    	inner join NS_NhanVien nvtiepnhan on nvtiepnhan.ID = ptn.ID_NhanVien
    	inner join Gara_MauXe mauxe on mauxe.ID = dmx.ID_MauXe
    	inner join Gara_HangXe hangxe on hangxe.ID = mauxe.ID_HangXe
    	inner join Gara_LoaiXe loaixe on loaixe.ID = mauxe.ID_LoaiXe
    	inner join DM_DonVi dv on dv.ID = ptn.ID_DonVi
    	inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
    	WHERE exists (select GiaTri from @tbTrangThai tt where ptn.TrangThai = tt.GiaTri)
    		AND (@NgayTiepNhan_From IS NULL OR ptn.NgayVaoXuong BETWEEN @NgayTiepNhan_From AND @NgayTiepNhan_To)
    		AND (@NgayXuatXuongDuKien_From IS NULL OR ptn.NgayXuatXuongDuKien BETWEEN @NgayXuatXuongDuKien_From AND @NgayXuatXuongDuKien_To)
    		AND (@NgayXuatXuong_From IS NULL OR ptn.NgayXuatXuong BETWEEN @NgayXuatXuong_From AND @NgayXuatXuong_To)
			AND ((@BaoHiem = 0 AND 1 = 0) OR (@BaoHiem = 1 AND ptn.ID_BaoHiem IS NOT NULL) OR (@BaoHiem = 2 AND ptn.ID_BaoHiem IS NULL)
			OR @BaoHiem = 3 AND 1 = 1)
    		AND ((select count(Name) from @tblSearch b where     			
    		ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
    		or ptn.GhiChu like '%'+b.Name+'%'
    		or dt.MaDoiTuong like '%'+b.Name+'%'		
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    		or dt.DienThoai like '%'+b.Name+'%'
    		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    		or nvcovan.TenNhanVien like '%'+b.Name+'%'	
    		or nvcovan.MaNhanVien like '%'+b.Name+'%'	
    		or nvcovan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or nvcovan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvtiepnhan.TenNhanVien like '%'+b.Name+'%'	
    		or nvtiepnhan.MaNhanVien like '%'+b.Name+'%'	
    		or nvtiepnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvtiepnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or ptn.TenLienHe like '%'+b.Name+'%'	
    		or ptn.SoDienThoaiLienHe like '%'+b.Name+'%'
    		or dmx.BienSo like '%'+b.Name+'%'
			or mauxe.TenMauXe like '%'+b.Name+'%'
			or hangxe.TenHangXe like '%'+b.Name+'%'
			or loaixe.TenLoaiXe like '%'+b.Name+'%'
			or bh.TenDoiTuong like '%'+b.Name+'%'
			or bh.MaDoiTuong like '%'+b.Name+'%'
    		)=@count or @count=0)
    		),
    		count_cte
    		as
    		(
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    		SELECT dt.*, ct.* FROM data_cte dt
    		CROSS JOIN count_cte ct
    		ORDER BY dt.NgayVaoXuong desc
    		OFFSET (@CurrentPage * @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY;
    	END
    	ELSE
    	BEGIN
    		with data_cte
    		as
    		(
    		select ptn.ID, ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, ptn.SoKmVao, ptn.NgayXuatXuongDuKien, ptn.NgayXuatXuong, ptn.TrangThai,
    		ptn.SoKmRa, ptn.TenLienHe, ptn.SoDienThoaiLienHe, ptn.GhiChu, ptn.TrangThai AS TrangThaiPhieuTiepNhan,
    		ptn.ID_Xe, dmx.BienSo, dmx.SoMay, dmx.SoKhung, dmx.NamSanXuat, mauxe.TenMauXe, hangxe.TenHangXe, loaixe.TenLoaiXe,
    		ptn.ID_KhachHang, dt.MaDoiTuong, dt.TenDoiTuong, dt.Email, dt.DienThoai AS DienThoaiKhachHang, dt.DiaChi,
    		ptn.ID_CoVanDichVu, ISNULL(nvcovan.TenNhanVien, '') AS CoVanDichVu, ISNULL(nvcovan.MaNhanVien, '') AS MaCoVan, nvcovan.DienThoaiDiDong AS CoVan_SDT,
    		ptn.ID_NhanVien, nvtiepnhan.MaNhanVien AS MaNhanVienTiepNhan, nvtiepnhan.TenNhanVien AS NhanVienTiepNhan,
    		dmx.DungTich, dmx.MauSon, dmx.HopSo,
			cast(iif(dmx.ID_KhachHang = ptn.ID_KhachHang,'1','0') as bit) as LaChuXe,
			cx.ID as ID_ChuXe,
    		cx.TenDoiTuong as ChuXe,
    		cx.DienThoai as ChuXe_SDT, cx.DiaChi as ChuXe_DiaChi, cx.Email as ChuXe_Email,
    		dv.MaDonVi, dv.TenDonVi,
			bh.TenDoiTuong as TenBaoHiem, bh.MaDoiTuong as MaBaoHiem, ptn.NguoiLienHeBH, ptn.SoDienThoaiLienHeBH,
    		ptn.NgayTao, ptn.ID_BaoHiem,ptn.ID_DonVi
    		from Gara_PhieuTiepNhan ptn
    		inner join Gara_DanhMucXe dmx on ptn.ID_Xe = dmx.ID
    		LEFT join DM_DoiTuong cx on dmx.ID_KhachHang = cx.ID
    		inner join DM_DoiTuong dt on dt.ID = ptn.ID_KhachHang
			left join DM_DoiTuong bh on ptn.ID_BaoHiem = bh.ID
    		left join NS_NhanVien nvcovan on nvcovan.ID = ptn.ID_CoVanDichVu
    		inner join NS_NhanVien nvtiepnhan on nvtiepnhan.ID = ptn.ID_NhanVien
    		inner join Gara_MauXe mauxe on mauxe.ID = dmx.ID_MauXe
    		inner join Gara_HangXe hangxe on hangxe.ID = mauxe.ID_HangXe
    		inner join Gara_LoaiXe loaixe on loaixe.ID = mauxe.ID_LoaiXe
    		inner join DM_DonVi dv on dv.ID = ptn.ID_DonVi
    		inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
    		WHERE exists (select GiaTri from @tbTrangThai tt where ptn.TrangThai = tt.GiaTri)
    			AND (@NgayTiepNhan_From IS NULL OR ptn.NgayVaoXuong BETWEEN @NgayTiepNhan_From AND @NgayTiepNhan_To)
    			AND (@NgayXuatXuongDuKien_From IS NULL OR ptn.NgayXuatXuongDuKien BETWEEN @NgayXuatXuongDuKien_From AND @NgayXuatXuongDuKien_To)
    			AND (@NgayXuatXuong_From IS NULL OR ptn.NgayXuatXuong BETWEEN @NgayXuatXuong_From AND @NgayXuatXuong_To)
				AND ((@BaoHiem = 0 AND 1 = 0) OR (@BaoHiem = 1 AND ptn.ID_BaoHiem IS NOT NULL) OR (@BaoHiem = 2 AND ptn.ID_BaoHiem IS NULL)
				OR @BaoHiem = 3 AND 1 = 1)
    			AND ((select count(Name) from @tblSearch b where     			
    			ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
    			or ptn.GhiChu like '%'+b.Name+'%'
    			or dt.MaDoiTuong like '%'+b.Name+'%'		
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.DienThoai like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or nvcovan.TenNhanVien like '%'+b.Name+'%'	
    			or nvcovan.MaNhanVien like '%'+b.Name+'%'	
    			or nvcovan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    			or nvcovan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    			or nvtiepnhan.TenNhanVien like '%'+b.Name+'%'	
    			or nvtiepnhan.MaNhanVien like '%'+b.Name+'%'	
    			or nvtiepnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    			or nvtiepnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    			or ptn.TenLienHe like '%'+b.Name+'%'	
    			or ptn.SoDienThoaiLienHe like '%'+b.Name+'%'
    			or dmx.BienSo like '%'+b.Name+'%'
				or mauxe.TenMauXe like '%'+b.Name+'%'
				or hangxe.TenHangXe like '%'+b.Name+'%'
				or loaixe.TenLoaiXe like '%'+b.Name+'%'
				or bh.TenDoiTuong like '%'+b.Name+'%'
				or bh.MaDoiTuong like '%'+b.Name+'%'
    			)=@count or @count=0)
    			)
    			SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
    			ORDER BY dt.NgayVaoXuong desc
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListTheGiaTri]
    @IDDonVis [nvarchar](max),
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
    
    	select tblThe.ID,tblThe.MaHoaDon,tblThe.NgayLapHoaDon,tblThe.NgayTao,
    		tblThe.TongChiPhi as MucNap,
    		tblThe.TongChietKhau as KhuyenMaiVND,
    		tblThe.TongTienHang as TongTienNap,
    		tblThe.TongTienThue as SoDuSauNap,
    		tblThe.TongGiamGia as ChietKhauVND,
    		ISNULL(tblThe.DienGiai,'') as GhiChu,
    		tblThe.NguoiTao,
    		ISNULL(tblThe.ID_DoiTuong,'00000000-0000-0000-0000-000000000000') as ID_DoiTuong,
    		tblThe.PhaiThanhToan,
    		ISNULL(tblThe.NhanVienThucHien,'') as NhanVienThucHien,
    		tblThe.MaDoiTuong as MaKhachHang,
    		tblThe.TenDoiTuong as TenKhachHang,
    		tblThe.DienThoai as SoDienThoai,
    		tblThe.DiaChi as DiaChiKhachHang,
    		tblThe.ChoThanhToan,
    		tblThe.ChietKhauPT,
    		tblThe.KhuyenMaiPT,
			tblThe.ID_DonVi,
    		ISNULL(soquy.TienMat,0) as TienMat,
    		ISNULL(soquy.TienPOS,0) as TienATM,
    		ISNULL(soquy.TienCK,0) as TienGui,
    		ISNULL(soquy.TienThu,0) as KhachDaTra,
    		dv.TenDonVi,
    		dv.SoDienThoai as DienThoaiChiNhanh,
    		dv.DiaChi as DiaChiChiNhanh
    	from
    		(
    		select *
    		from
    			(select hd.*, 
						iif(hd.TongChiPhi=0,0, hd.TongGiamGia/hd.TongChiPhi * 100) as ChietKhauPT,
						iif(hd.TongChiPhi=0,0, hd.TongChietKhau/hd.TongChiPhi * 100) as KhuyenMaiPT,
    					dt.MaDoiTuong, dt.TenDoiTuong,
    					dt.DienThoai, 
    					dt.DiaChi,
    					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai,
    					NhanVienThucHien
    				from BH_HoaDon hd
    				join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
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
    				where exists (select name from dbo.splitstring(@IDDonVis) dv where hd.ID_DonVi= dv.Name)	
    				and hd.LoaiHoaDon = 22
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
    					sum(quy.TienCK) as TienCK
    				from
    				(
    					select qct.ID_HoaDonLienQuan,
    						qct.TienMat,
    						qct.TienThu,
    						case when tk.TaiKhoanPOS = '1' then qct.TienGui else 0 end as TienPOS,
    						case when tk.TaiKhoanPOS = '0' then qct.TienGui else 0 end as TienCK
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang= tk.ID
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
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetLuongOT_ofNhanVien]
    @IDChiNhanhs [uniqueidentifier],
    @IDNhanViens [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @NgayCongChuan [int]
AS
BEGIN
    SET NOCOUNT ON;	
    
    		declare @tblCongThuCong CongThuCong
    		insert into @tblCongThuCong
    		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate
    
    		declare @tblThietLapLuong ThietLapLuong
    		insert into @tblThietLapLuong
    		exec GetNS_ThietLapLuong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate
    
    
    		-- ============= OT Ngay ====================
    		declare @thietlapOTNgay table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
    		insert into @thietlapOTNgay
    		select *	
    		from @tblThietLapLuong pc 		
    		where pc.LoaiLuong = 2
    
    		select  a.*,		
    				case when LaPhanTram = 0 then GiaTri else SoTien/@NgayCongChuan/8 end as Luong1GioCongCoBan ,
    				case when LaPhanTram = 0 then HeSo * GiaTri else (SoTien/@NgayCongChuan/8) * GiaTri * HeSo/100 end as Luong1GioCongQuyDoi				
    			into #temp1					
    			from
    			(
    			select bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
    					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
    					pc.SoTien,
    					pc.LoaiLuong,
    					pc.HeSo,

						case when ngayle.ID is null then -- 0.chunhat, 6.thu7, 1.ngaynghi, 2.ngayle, 3.ngaythuong  			
							case bs.Thu
								when 6 then 6
								when 0 then 0
							else 3 end
						else bs.LoaiNgay end LoaiNgayThuong_Nghi,	

						case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_GiaTri
								when 0 then tlct.ThCN_GiaTri
							else tlct.LuongNgayThuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_GiaTri
								when 2 then  tlct.NgayLe_GiaTri
							else tlct.LuongNgayThuong end
						end as GiaTri,

						case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_LaPhanTramLuong
								when 0 then tlct.CN_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_LaPhanTramLuong
								when 2 then  tlct.NgayLe_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						end as LaPhanTram								
    			from @tblCongThuCong bs
    			join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
    			join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap 
				left join NS_NgayNghiLe ngayle on bs.NgayCham = ngayle.Ngay and ngayle.TrangThai='1'
    			where tlct.LaOT= 1 and pc.LoaiLuong= 2
				and exists (select tl.ID_PhuCap from @tblThietLapLuong tl where pc.ID = tl.ID_PhuCap)
    			) a			
    
    			declare @tblCongOTNgay table (ID_PhuCap uniqueidentifier, ID_NhanVien uniqueidentifier, LoaiLuong int, 
    			Luong1GioCongCoBan float, Luong1GioCongQuyDoi float, HeSoLuong int,NgayApDung datetime, NgayKetThuc datetime,
    			LoaiNgayThuong_Nghi int, LaPhanTram int,
    			SoGioOT float, LuongOT float, NgayCham datetime)	
    
    			declare @otngayID_NhanVien uniqueidentifier, @otngayID_PhuCap uniqueidentifier, @otngayTenLoaiLuong nvarchar(max), 
    			@otngayLoaiLuong int,@otngayLuongCoBan float, @otngayHeSo int, @otngayNgayApDung datetime, @otngayNgayKetThuc datetime
    
    			DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTNgay
    		OPEN curLuong -- cur 1
    	FETCH FIRST FROM curLuong
    	INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    				insert into @tblCongOTNgay
    				select @otngayID_PhuCap,@otngayID_NhanVien, @otngayLoaiLuong,tmp.Luong1GioCongCoBan,tmp.Luong1GioCongQuyDoi, tmp.GiaTri,@otngayNgayApDung, @otngayNgayKetThuc,
    					tmp.LoaiNgayThuong_Nghi, tmp.LaPhanTram,
    					tmp.SoGioOT,
    					tmp.SoGioOT * Luong1GioCongQuyDoi as LuongOT,
    					tmp.NgayCham
    				from #temp1 tmp
    				where tmp.ID_NhanVien = @otngayID_NhanVien and tmp.NgayCham >= @otngayNgayApDung and (@otngayNgayKetThuc is null OR tmp.NgayCham <= @otngayNgayKetThuc )  								
    				FETCH NEXT FROM curLuong 
    				INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
    			END;
    			CLOSE curLuong  
    		DEALLOCATE curLuong 	
    
    
    			-- ============= OT Ca =================
    
    			declare @thietlapOTCa table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
    			insert into @thietlapOTCa
    			select *	
    			from @tblThietLapLuong pc 		
    			where pc.LoaiLuong = 3
    
    			select  a.*,	
    				case when LaPhanTram = 0 then GiaTri else case when LuongTheoCa is null then SoTien/TongGioCong1Ca else LuongTheoCa/TongGioCong1Ca end end Luong1GioCongCoBan,
    				case when LaPhanTram = 0 then GiaTri else case when LuongTheoCa is null then SoTien/TongGioCong1Ca * GiaTri/100 
    				else LuongTheoCa/TongGioCong1Ca* GiaTri/100 end end as Luong1GioCongQuyDoi				
    			into #temp2					
    			from
    				(select bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
    					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
    					pc.SoTien,
    					theoca.LuongTheoCa,
    					pc.LoaiLuong,
    					pc.HeSo,
						case when ngayle.ID is null then -- 0.chunhat, 6.thu7, 1.ngaynghi, 2.ngayle, 3.ngaythuong  			
							case bs.Thu
								when 6 then 6
								when 0 then 0
							else 3 end
						else bs.LoaiNgay end LoaiNgayThuong_Nghi,	

						case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_GiaTri
								when 0 then tlct.ThCN_GiaTri
							else tlct.LuongNgayThuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_GiaTri
								when 2 then  tlct.NgayLe_GiaTri
							else tlct.LuongNgayThuong end
						end as GiaTri,

						case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_LaPhanTramLuong
								when 0 then tlct.CN_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_LaPhanTramLuong
								when 2 then  tlct.NgayLe_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						end as LaPhanTram
    				
    				from @tblCongThuCong bs
    				join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
    				join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap 
					left join NS_NgayNghiLe ngayle on bs.NgayCham = ngayle.Ngay and ngayle.TrangThai='1'
    				left join (select tlca.LuongNgayThuong as LuongTheoCa, tlca.ID_CaLamViec, pca.ID_NhanVien
    						from NS_Luong_PhuCap pca
    						join NS_ThietLapLuongChiTiet tlca on pca.ID= tlca.ID_LuongPhuCap 
    						where tlca.LaOT= 0
    						) theoca on pc.ID_NhanVien= theoca.ID_NhanVien and bs.ID_CaLamViec= theoca.ID_CaLamViec
    				where tlct.LaOT= 1
    				and pc.LoaiLuong = 3
					and exists (select tl.ID_PhuCap from @tblThietLapLuong tl where pc.ID = tl.ID_PhuCap)
    				) a			
    
    		declare @tblCongOTCa table (ID_PhuCap uniqueidentifier, ID_NhanVien uniqueidentifier, LoaiLuong int, 
    		Luong1GioCongCoBan float, Luong1GioCongQuyDoi float, HeSoLuong int,NgayApDung datetime, NgayKetThuc datetime,
    		LoaiNgayThuong_Nghi int, LaPhanTram int,
    		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
    		SoGioOT float, LuongOT float, NgayCham datetime)				
    	
    		-- biến để đọc gtrị cursor
    		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int,@LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime
    
    		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTCa
    		OPEN curLuong
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    				insert into @tblCongOTCa
    				select @ID_PhuCap,@ID_NhanVien, @LoaiLuong,tmp.Luong1GioCongCoBan,tmp.Luong1GioCongQuyDoi, tmp.GiaTri,@NgayApDung, @NgayKetThuc,
    				tmp.LoaiNgayThuong_Nghi, tmp.LaPhanTram,
    					tmp.ID_CaLamViec, 					
    					tmp.TenCa, 
    					tmp.TongGioCong1Ca,
    					tmp.SoGioOT,
    					tmp.SoGioOT * Luong1GioCongQuyDoi as LuongOT,
    					tmp.NgayCham
    				from #temp2 tmp
    				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  								
    				FETCH NEXT FROM curLuong 
    				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
    			END;
    			CLOSE curLuong  
    		DEALLOCATE curLuong 	
    
    			select nv.MaNhanVien, nv.TenNhanVien, ot.ID_NhanVien,
    				LoaiLuong, Luong1GioCongCoBan, 
    				FORMAT(ot.Luong1GioCongQuyDoi,'###,###.###') as Luong1GioCongQuyDoi, 
    				HeSoLuong,
    				LoaiNgayThuong_Nghi, 
    				LaPhanTram,
    				ID_CaLamViec, TenCa, TongGioCong1Ca,
    				cast(SoGioOT as float) as SoGioOT,
    				FORMAT(ot.LuongOT,'###,###.###') as ThanhTien
    			from 
    				(
    				select 
    					ID_NhanVien, LoaiLuong, Luong1GioCongCoBan, Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
    					ID_CaLamViec, TenCa, TongGioCong1Ca,
    					sum(SoGioOT) as SoGioOT,
    					sum(LuongOT) as LuongOT
    				from
    					(select ID_NhanVien, LoaiLuong, Luong1GioCongCoBan, Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
    						'00000000-0000-0000-0000-000000000000' as ID_CaLamViec, '' as TenCa, 8 as TongGioCong1Ca,
    						SoGioOT,
    						LuongOT
    					from @tblCongOTNgay cong				
    
    					union all
    
    					select ID_NhanVien, LoaiLuong, Luong1GioCongCoBan, Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
    						ID_CaLamViec, TenCa,TongGioCong1Ca,
    						SoGioOT,
    						LuongOT
    					from @tblCongOTCa cong	
    					) luongot
    				group by luongot.ID_NhanVien,LoaiLuong, Luong1GioCongCoBan,Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
    						luongot.ID_CaLamViec, TenCa, TongGioCong1Ca
    			)ot
    			join NS_NhanVien nv on ot.ID_NhanVien= nv.ID
    			where ot.LuongOT > 0
    			order by ID_NhanVien, ID_CaLamViec
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetNhatKyTienCoc_OfDoiTuong]
    @ID_DoiTuong [nvarchar](max),
    @IDDonVis [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	
    	declare @tblChiNhanh table(ID_DonVi uniqueidentifier)
    	insert into @tblChiNhanh
    	select name from dbo.splitstring(@IDDonVis)
    
    	declare @LoaiDoiTuong int = (select LoaiDoiTuong from DM_DoiTuong where ID= @ID_DoiTuong)
    	if @FromDate is null
    		set @FromDate ='2020-01-01'
    	if @ToDate is null
    		set @ToDate = DATEADD(DAY,1,GETDATE())
    	
    
    	declare @tblDiary table(
    	 ID_PhieuThu uniqueidentifier, MaPhieuThu nvarchar(max), NgayLapPhieuThu datetime,
    		  ID_HoaDon uniqueidentifier, MaHoaDon nvarchar(max), GiaTri float, sLoaiHoaDon nvarchar(max), 
			  LoaiHoaDon int, LoaiThanhToan int,SoDu float
    	)
    
    	-- phieu naptiencoc
    	insert into @tblDiary
    	select * ,0
    	from
    	(
		
    	select
    		hd.ID as ID_PhieuThu,
    		hd.MaHoaDon as MaPhieuThu,
    		hd.NgayLapHoaDon,
    		null as ID,
    		'' as MaHoaDon,
    		case when @LoaiDoiTuong= 2 then iif(hd.LoaiHoaDon=11,-hd.TongTienThu, hd.TongTienThu)
    		else iif(hd.LoaiHoaDon=11,hd.TongTienThu, -hd.TongTienThu) end as GiaTri,
    		case when hd.LoaiHoaDon= 11 then
    			case when @LoaiDoiTuong= 2 then N'Chi trả cọc' else N'Nạp tiền cọc' end
    		else
    			case when @LoaiDoiTuong= 2 then N'Nạp tiền cọc' else N'Chi trả cọc' end
    		end as sLoaiHoaDon,
			hd.LoaiHoaDon,
			1 as LoaiThanhToan
    	from Quy_HoaDon hd
    	join Quy_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    	where ct.ID_DoiTuong like @ID_DoiTuong
    	and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    	and hd.TrangThai='1'
    	and ct.LoaiThanhToan= 1
    	and exists(select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
		group by hd.ID ,hd.LoaiHoaDon,
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,
			hd.TongTienThu

    	union all
    	-- sudung coc
    	select
    		hd.ID as ID_PhieuThu,
    		hd.MaHoaDon as MaPhieuThu, 
    		hd.NgayLapHoaDon, 
    		hdsd.ID,
    		hdsd.MaHoaDon,
    		-sum(ct.TienThu) as GiaTri,
    		case hdsd.LoaiHoaDon
    			when 1 then N'Hóa đơn bán'
    			when 4 then N'Nhập hàng'
    			when 6 then N'Trả hàng'
    			when 7 then N'Trả hàng nhạp'
    			when 19 then N'Gói dịch vụ'
    			when 25 then N'Hóa đơn sửa chữa'
    	else '' end as sLoaiHoaDon,
		hd.LoaiHoaDon,
		0 as LoaiThanhToan
    	from Quy_HoaDon hd
    	join Quy_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    	join BH_HoaDon hdsd on ct.ID_HoaDonLienQuan = hdsd.ID
    	where ct.ID_DoiTuong like @ID_DoiTuong
    	and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    	and exists(select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
    	and hd.TrangThai='1'
    	and ct.HinhThucThanhToan= 6
    	group by hd.ID ,
    		hd.MaHoaDon ,
			hd.LoaiHoaDon,
    		hd.NgayLapHoaDon, 
    		hdsd.ID,
    		hdsd.MaHoaDon,
    		hdsd.LoaiHoaDon
    	) a 
    	
    	declare @ID_PhieuThu uniqueidentifier, @MaPhieuThu nvarchar(max), @NgayLapPhieuThu datetime,
    		 @ID_HoaDon uniqueidentifier, @MaHoaDon nvarchar(max), @GiaTri float, @sLoaiHoaDon nvarchar(max), @SoDu float, @SoDuSauPhatSinh float
    	
    	set @SoDuSauPhatSinh =0
    	declare _cur cursor
    	for
    	select ID_PhieuThu, MaPhieuThu, NgayLapPhieuThu, ID_HoaDon, MaHoaDon,
			GiaTri, sLoaiHoaDon, SoDu
    	from @tblDiary tmp
    	order by NgayLapPhieuThu 
    	open _cur
    	fetch next from _cur 
    	into @ID_PhieuThu ,@MaPhieuThu , @NgayLapPhieuThu,
    		  @ID_HoaDon, @MaHoaDon , @GiaTri, @sLoaiHoaDon , @SoDu
    	while @@FETCH_STATUS =0
    		begin							
    			set @SoDuSauPhatSinh = @SoDuSauPhatSinh +  @GiaTri
    			update @tblDiary set SoDu = @SoDuSauPhatSinh where ID_PhieuThu = @ID_PhieuThu
    			
    			FETCH NEXT FROM _cur 
    			INTO @ID_PhieuThu ,@MaPhieuThu , @NgayLapPhieuThu,  @ID_HoaDon, @MaHoaDon , @GiaTri, @sLoaiHoaDon , @SoDu 		 
    		end  
    
    		close _cur
    		deallocate _cur;
    
    		with data_cte
    		as
    		(
    		select * 
			from @tblDiary
    		),
    		count_cte
    		as (
    			select count(ID_PhieuThu) as TotalRow,
    				CEILING(COUNT(ID_PhieuThu) / CAST(@PageSize as float ))  as TotalPage    				
    			from data_cte
    		)
    			-- do return class SoQuyDTO at C#
    		select 
    				dt.ID_PhieuThu as ID,
    				dt.MaPhieuThu as MaPhieuChi,
    				dt.NgayLapPhieuThu as NgayLapHoaDon,
    				dt.MaHoaDon,
    				dt.ID_HoaDon as ID_HoaDonGoc,
    				dt.GiaTri as PhaiThanhToan,
    				dt.SoDu as DuNoKH,
    				dt.sLoaiHoaDon as strLoaiHoaDon,		
					dt.LoaiHoaDon,
					dt.LoaiThanhToan,
    				cte.*
    		from data_cte dt    		
    		cross join count_cte cte
    		order by dt.NgayLapPhieuThu desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetQuyChiTiet_byIDQuy]
    @ID [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
	declare @ngaylapPhieuThu datetime = (select top 1 NgayLapHoaDon from Quy_HoaDon where ID= @ID)

	---- get allhoadon lienquan by idSoQuy
	select ID_HoaDonLienQuan  into #tblHoaDon
	from Quy_HoaDon_ChiTiet qct
	where qct.ID_HoaDon = @ID	

	---- get phieuthu/chi lienquan hoadon
		select 
			qct.ID_HoaDonLienQuan,
			qct.ID_DoiTuong,
			sum(qct.TienThu) as DaThuTruoc
	into #tblThuTruoc
	 from Quy_HoaDon qhd
    join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
	where exists
		(select qct2.ID_HoaDonLienQuan from #tblHoaDon qct2 
		where qct.ID_HoaDonLienQuan = qct2.ID_HoaDonLienQuan)
	and qhd.ID != @ID
	and qhd.TrangThai ='1'
	group by qct.ID_HoaDonLienQuan,qct.ID_DoiTuong

	---- if hd xuly from dathang --> get infor hd dathang
	select hdd.ID, hdMua.ID as ID_HoaDonMua, hdMua.NgayLapHoaDon into #tblDat
	from BH_HoaDon hdd
	join
	(
		select hd.ID, hd.ID_HoaDon, hd.NgayLapHoaDon
		from #tblHoaDon tmp
		join BH_HoaDon hd on tmp.ID_HoaDonLienQuan= hd.ID
	) hdMua on hdd.ID = hdMua.ID_HoaDon
	where hdd.LoaiHoaDon = 3 and hdd.ChoThanhToan='0'

	---- get phieuthu from dathang
		select thuDH.ID_HoaDonMua, 
				thuDH.ID_DoiTuong,
				thuDH.ThuDatHang
		into #tblThuDH
			from
			(
				select tblDH.ID_HoaDonMua,
					tblDH.ID_DoiTuong,
					sum(tblDH.TienThu) as ThuDatHang,		
					ROW_NUMBER() OVER(PARTITION BY tblDH.ID ORDER BY tblDH.NgayLapHoaDon ASC) AS isFirst	--- group by hdDat, sort by ngaylap hdxuly
				from
				(			
						select hdd.ID_HoaDonMua, hdd.NgayLapHoaDon,		
							hdd.ID,
							qct.ID_DoiTuong,
							iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu) as TienThu			
						from Quy_HoaDon_ChiTiet qct
						join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
						join #tblDat hdd on hdd.ID= qct.ID_HoaDonLienQuan				
						where (qhd.TrangThai= 1 Or qhd.TrangThai is null)
				) tblDH group by tblDH.ID_HoaDonMua, tblDH.ID,tblDH.NgayLapHoaDon, tblDH.ID_DoiTuong
		) thuDH where thuDH.isFirst= 1 

	---- get chiphi dichvu NCC
	select 
			cp.ID_HoaDon,
			sum(cp.ThanhTien) as PhaiThanhToan
		into #tblChiPhi
		from BH_HoaDon_ChiPhi cp
		left join #tblHoaDon hd on cp.ID_HoaDon = hd.ID_HoaDonLienQuan
		group by cp.ID_HoaDon

    select qhd.id, qct.ID_HoaDon, qhd.MaHoaDon, qhd.NguoiTao, qhd.LoaiHoaDon, qhd.TongTienThu, qhd.ID_NhanVien, qhd.NoiDungThu,
		 qhd.ID_DonVi,	qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo,qhd.NguoiSua, isnull(qhd.TrangThai, '1') as TrangThai,
    	  iif(qct.HinhThucThanhToan=1, qct.TienThu,0) as TienMat, 
		  iif(qct.HinhThucThanhToan=2 or qct.HinhThucThanhToan=3 , qct.TienThu,0) as TienGui, 
			qct.TienThu, qct.DiemThanhToan, qct.ID_TaiKhoanNganHang, qct.ID_KhoanThuChi, 
		   qhd.NgayLapHoaDon as NgayLapPhieuThu,
    	   qct.ID_DoiTuong,
		   qct.ID_BangLuongChiTiet,
    	   qct.ID_HoaDonLienQuan,
    	   qct.ID_NhanVien as ID_NhanVienCT, -- thu/chi cho NV nao
    	   qct.HinhThucThanhToan,
    	   cast(iif(qhd.LoaiHoaDon = 11,'1','0') as bit) as LaKhoanThu,
    	   iif(qct.LoaiThanhToan = 1,1,0) as LaTienCoc,
    	   isnull(hd.MaHoaDon,N'Thu thêm') as MaHoaDonHD,    	  
		   nv.TenNhanVien,
		   dt.MaDoiTuong as MaDoiTuong, 
		   dt.TenDoiTuong as NguoiNopTien, 	
		   dt.DienThoai as SoDienThoai,
    	   case dt.LoaiDoiTuong
    		when 1 then 1
    		when 2 then 2
    		when 3 then 3
    		else 0 end as LoaiDoiTuong,
    		iif(qhd.TrangThai ='0', N'Đã hủy', N'Đã thanh toán') as GhiChu,	  
    	   iif( hd.NgayLapHoaDon is null, qhd.NgayLapHoaDon, hd.NgayLapHoaDon) as NgayLapHoaDon,
    	   case qct.HinhThucThanhToan
    			when 1 then  N'Tiền mặt'
    			when 2 then  N'POS'
    			when 3 then  N'Chuyển khoản'
    			when 4 then  N'Thu từ thẻ'
    			when 5 then  N'Đổi điểm'
    			when 6 then  N'Thu từ cọc'
    		end as PhuongThuc,
			ktc.NoiDungThuChi,
			iif(ktc.LaKhoanThu is null,  IIF(qhd.LoaiHoaDon=11,'1','0'), ktc.LaKhoanThu) as LaKhoanThu,
			iif(tk.TaiKhoanPOS ='1',tk.TenChuThe,'') as TenTaiKhoanPOS,
			iif(tk.TaiKhoanPos ='0',tk.TenChuThe,'') as TenTaiKhoanNOTPOS,	
			isnull(hd.LoaiHoaDon,0) as LoaiHoaDonHD,
			isnull(iif(dt.LoaiDoiTuong =3, hd.TongTienThueBaoHiem,iif(hd.TongThueKhachHang >0, hd.TongThueKhachHang, hd.TongTienThue)),0) as TongTienThue,
			isnull(case dt.LoaiDoiTuong
				when 2 then iif(hd.LoaiHoaDon in (4,7), hd.PhaiThanhToan,cp.PhaiThanhToan)
				when 1 then hd.PhaiThanhToan
				when 3 then hd.PhaiThanhToanBaoHiem
			end,0) as TongThanhToanHD,		
			isnull(thu.DaThuTruoc,0) as DaThuTruoc,
			tk.TaiKhoanPOS
    from Quy_HoaDon qhd
    join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    left join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
	left join #tblChiPhi cp on hd.ID= cp.ID_HoaDon and qct.ID_HoaDonLienQuan = cp.ID_HoaDon
    left join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
	left join NS_NhanVien nv on qhd.ID_NhanVien= nv.ID
	left join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi = ktc.ID
	left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID
	left join (
		select 
			thutruoc.ID_HoaDonLienQuan,
			thutruoc.ID_DoiTuong,
			sum(isnull(DaThuTruoc,0)) as DaThuTruoc
		from
		(
		select tmp.ID_HoaDonLienQuan,tmp.ID_DoiTuong, isnull(tmp.DaThuTruoc,0) as DaThuTruoc
		from #tblThuTruoc tmp 
		union all
		select thuDH.ID_HoaDonMua, thuDH.ID_DoiTuong, isnull(thuDH.ThuDatHang,0) as DaThuTruoc
		from #tblThuDH thuDH 
		) thutruoc group by thutruoc.ID_HoaDonLienQuan, thutruoc.ID_DoiTuong
	) thu on thu.ID_HoaDonLienQuan = qct.ID_HoaDonLienQuan and thu.ID_DoiTuong = qct.ID_DoiTuong
    where qhd.ID= @ID
	order by hd.NgayLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[HuyPhieuThu_UpdateCongNoTamUngLuong]
    @ID_ChiNhanh [uniqueidentifier],
    @IDQuyChiTiets [nvarchar](max),
    @LaPhieuTamUng [bit]
AS
BEGIN
    SET NOCOUNT ON;
    
    		declare @tblQuyChiTiet table(ID_QuyChiTiet uniqueidentifier)
    	insert into @tblQuyChiTiet
    	select Name from dbo.splitstring(@IDQuyChiTiets)
    
    		if @LaPhieuTamUng ='1'
    			begin
    			declare @sotienTamUng float, @nvTamUng uniqueidentifier, @idKhoanThuChi uniqueidentifier
    			-- get sotien, nhanvien tamung
    			select @sotienTamUng = TienThu, @nvTamUng= ID_NhanVien, @idKhoanThuChi= ID_KhoanThuChi
    			from Quy_HoaDon_ChiTiet qct1 where exists (select ID from @tblQuyChiTiet qct2 where qct1.ID= qct2.ID_QuyChiTiet)
					
				-- update khoanthuchi: tamungluong --> khoan khac
				-- or: huy phieu tamungluong
    			declare @giamtruLuong bit= (
    				select TinhLuong
    				from Quy_KhoanThuChi khoan
    				where id= @idKhoanThuChi)
		
    				update NS_CongNoTamUngLuong set CongNo = CongNo - @sotienTamUng where ID_NhanVien = @nvTamUng and ID_DonVi= @ID_ChiNhanh	
    			
    		end
    		else
    				update tblNoCu set CongNo= tblNoHienTai.NoHienTai
    			from NS_CongNoTamUngLuong tblNoCu
    			join (
    				select congno.ID,congno.CongNo + quy.TruTamUngLuong as NoHienTai
    				from NS_CongNoTamUngLuong congno
    				join (
    					select qct.ID_NhanVien, sum(qct.TruTamUngLuong) as TruTamUngLuong --- thanhtoan nhieulan
    					from Quy_HoaDon_ChiTiet qct 
    					join @tblQuyChiTiet qct2 on qct.ID= qct2.ID_QuyChiTiet		
						group by qct.ID_NhanVien
    					) quy on congno.ID_NhanVien= quy.ID_NhanVien
    				where congno.ID_DonVi= @ID_ChiNhanh
    			) tblNoHienTai on tblNoCu.ID= tblNoHienTai.ID
END");

            CreateStoredProcedure(name: "[dbo].[HuyPhieuThu_UpdateTrangThaiCong]", parametersAction: p => new
            {
                ID_QuyChiTiet = p.Guid()
            }, body: @"SET NOCOUNT ON;

	declare @ID_BangLuong_ChiTiet uniqueidentifier = (select top 1 ID_BangLuongChiTiet from Quy_HoaDon_ChiTiet where id = @ID_QuyChiTiet)

	

	if @ID_BangLuong_ChiTiet is not null
	begin

		select qct.ID_BangLuongChiTiet,
			sum(iif(qhd.TrangThai ='0',0, qct.TienThu + isnull(qct.TruTamUngLuong,0))) as DaThanhToan
		into #tblSQuy
		from Quy_HoaDon_ChiTiet qct 
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
		where qct.ID_BangLuongChiTiet = @ID_BangLuong_ChiTiet
		group by qct.ID_BangLuongChiTiet

		update bs set TrangThai = iif(soquy.DaThanhToan > 0 ,4,3)    					   
		from NS_CongBoSung bs
		join #tblSQuy soquy on bs.ID_BangLuongChiTiet = soquy.ID_BangLuongChiTiet
	end");

            Sql(@"ALTER PROCEDURE [dbo].[import_DanhMucHangHoa]
	@isUpdateHang int,---1.update, 0.insert
	@isUpdateTonKho int,--- 1.no, 2.yes
	@ID_DonVi [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],  
    @TenNhomHangHoaCha [nvarchar](max),
    @TenNhomHangHoaCha_KhongDau [nvarchar](max),
    @TenNhomHangHoaCha_KyTuDau [nvarchar](max),
    @MaNhomHangHoaCha [nvarchar](max), 
    @TenNhomHangHoa [nvarchar](max),
    @TenNhomHangHoa_KhongDau [nvarchar](max),
    @TenNhomHangHoa_KyTuDau [nvarchar](max),
    @MaNhomHangHoa [nvarchar](max),  
    @LoaiHangHoa int,
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
	@TonKho [nvarchar](max),
    @LaDonViChuan [bit],
    @TyLeChuyenDoi [nvarchar](max),  
    @MaHangHoaChaCungLoai [nvarchar](max),
	@DVTQuyCach nvarchar(max)
AS
BEGIN
		SET NOCOUNT ON;
		declare @dtNow datetime = getdate()
		declare @LaHangHoa bit ='true'

		if @LoaiHangHoa != 1
			set @LaHangHoa ='false'

		--declare @LoaiHangHoa int = iif(@LaHangHoa='true', 1,2)   

		DECLARE @ID_NhomHangHoaCha  uniqueidentifier = null;
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin		
			SELECT TOP(1) @ID_NhomHangHoaCha = ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		Begin		
				set @ID_NhomHangHoaCha = NEWID()
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha,@TenNhomHangHoaCha_KhongDau, @TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, '1', '1', '1', @LaHangHoa, 'admin', @dtNow, null, 0)
    		End
    	End
  	
		DECLARE @ID_NhomHangHoa  uniqueidentifier = null		
    	if (len(@TenNhomHangHoa) > 0)
    	Begin    	
			SELECT TOP(1) @ID_NhomHangHoa = ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    			Begin
					--- neu chuyen tu hanghoa --> dichvu (hoac nguoc lai)
					select TOP(1) @ID_NhomHangHoa = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa  and (TrangThai is NULL or TrangThai = 0)
					if @ID_NhomHangHoa is null	
					begin
						set @ID_NhomHangHoa = NEWID()
    					insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    					values (@ID_NhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @MaNhomHangHoa, '1', '1', '1', @LaHangHoa, 'admin', @dtNow, @ID_NhomHangHoaCha, 0)
					end
					
    			End
    	End
		else
			begin
				if @LaHangHoa='false' 
					set @ID_NhomHangHoa ='00000000-0000-0000-0000-000000000001'
				else 
					set @ID_NhomHangHoa ='00000000-0000-0000-0000-000000000000'
			end

		DECLARE @LaChaCungLoai  bit  = '1';
		DECLARE @TenCungLoai  nvarchar(max) = ''
    	DECLARE @ID_HangHoaCungLoai  uniqueidentifier  = newID();

    	if(len(@MaHangHoaChaCungLoai) > 0)
    	Begin    		
				select top 1 @ID_HangHoaCungLoai =ID_HangHoaCungLoai, 
							@TenCungLoai = TenHangHoa 
					from DM_HangHoa hh
					join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa
					where qd.MaHangHoa = @MaHangHoaChaCungLoai and LaChaCungLoai = '1';
    		if @ID_HangHoaCungLoai  is null   		
    			set @ID_HangHoaCungLoai = newID();
			else 
				begin
    				set @LaChaCungLoai = '0'; 
					if @TenHangHoa='' set @TenHangHoa = @TenCungLoai						
				end
    	End
		else
		begin
			if @isUpdateHang = 1 
				select top 1 @ID_HangHoaCungLoai = ID_HangHoaCungLoai from DM_HangHoa where ID = @ID_HangHoa
		end

		if @isUpdateHang = 1 
			begin
    			if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    			Begin
    				update DM_HangHoa set ID_HangHoaCungLoai = @ID_HangHoaCungLoai, LaChaCungLoai = @LaChaCungLoai, ID_NhomHang = @ID_NhomHangHoa,
							LaHangHoa = @LaHangHoa, LoaiHangHoa= @LoaiHangHoa,
    						NgaySua = @dtNow, NguoiSua='admin', 
								TenHangHoa = iif(@TenHangHoa='', TenHangHoa, @TenHangHoa), 
								TenHangHoa_KhongDau = iif(@TenHangHoa_KhongDau='', TenHangHoa_KhongDau, @TenHangHoa_KhongDau),   
								TenHangHoa_KyTuDau = iif(@TenHangHoa_KyTuDau='', TenHangHoa_KyTuDau, @TenHangHoa_KyTuDau),
    						TenKhac = @MaHangHoaChaCungLoai, GhiChu = @GhiChu, QuyCach = @QuyCach, DuocBanTrucTiep = @DuocBanTrucTiep, DonViTinhQuyCach= @DVTQuyCach 
					Where ID = @ID_HangHoa
    			end

    			update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoi, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBan, NguoiSua ='admin', NgaySua =@dtNow, GhiChu = @GhiChu
    			Where ID = @ID_DonViQuiDoi

			end
		else
			begin
					if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    				Begin
    					insert into DM_HangHoa (ID, ID_HangHoaCungLoai, LaChaCungLoai, ID_NhomHang, LaHangHoa, NgayTao,NguoiTao, TenHangHoa,TenHangHoa_KhongDau, TenHangHoa_KyTuDau,
    					TenKhac, TheoDoi, GhiChu, ChiPhiThucHien, ChiPhiTinhTheoPT, QuyCach, DuocBanTrucTiep, QuanLyTheoLoHang, DonViTinhQuyCach, LoaiHangHoa)
    					Values (@ID_HangHoa, @ID_HangHoaCungLoai, @LaChaCungLoai ,@ID_NhomHangHoa, @LaHangHoa, @dtNow, 'admin', @TenHangHoa, @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau,
    					@MaHangHoaChaCungLoai, '1', @GhiChu, '0', '1', @QuyCach, @DuocBanTrucTiep, '0',@DVTQuyCach, @LoaiHangHoa)
    				end
    				else
    				Begin
						declare @ID_QDChuan uniqueidentifier 
						select TOP(1) @ID_HangHoa = ID_HangHoa, @ID_QDChuan= ID from DonViQuiDoi where MaHangHoa like @MaDonViCoBan						
    				end

    				insert into DonViQuiDoi (ID, MaHangHoa, TenDonViTinh, ID_HangHoa, TyLeChuyenDoi, LaDonViChuan, GiaVon, GiaNhap, GiaBan, NguoiTao, NgayTao, Xoa, GhiChu)
    				Values (@ID_DonViQuiDoi, @MaHangHoa,@TenDonViTinh, @ID_HangHoa,@TyLeChuyenDoi, @LaDonViChuan, @GiaVon, @GiaVon,@GiaBan, 'admin', @dtNow, '0', @GhiChu)
			end    
	DECLARE @FTonKho FLOAT;
	SET @FTonKho = CAST(@TonKho AS float);
	exec UpdateTonKho_multipleDVT @isUpdateTonKho, @ID_DonVi, @ID_DonViQuiDoi, null, @FTonKho
END");

            Sql(@"ALTER PROCEDURE [dbo].[import_DanhMucHangHoaLoHang]
	@isUpdateHang int,
	@isUpdateTonKho int,
	@ID_DonVi uniqueidentifier,
	@ID_HangHoa uniqueidentifier,
	@ID_DonViQuiDoi [uniqueidentifier],
	@ID_LoHang [uniqueidentifier] = null,
    @TenNhomHangHoaCha [nvarchar](max),
    @TenNhomHangHoaCha_KhongDau [nvarchar](max),
    @TenNhomHangHoaCha_KyTuDau [nvarchar](max),
    @MaNhomHangHoaCha [nvarchar](max),
    @TenNhomHangHoa [nvarchar](max),
    @TenNhomHangHoa_KhongDau [nvarchar](max),
    @TenNhomHangHoa_KyTuDau [nvarchar](max),
    @MaNhomHangHoa [nvarchar](max),
    @LoaiHangHoa int,
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
	@TonKho [nvarchar](max),
    @LaDonViChuan [bit],
    @TyLeChuyenDoi [nvarchar](max),
    @MaHangHoaChaCungLoai [nvarchar](max),
	@DVTQuyCach nvarchar(max),
    @MaLoHang [nvarchar](max),   
	@NgaySanXuat [datetime] =null,
    @NgayHetHan [datetime] = null  
AS
BEGIN
		SET NOCOUNT ON;
		declare @dtNow datetime = getdate()
		declare @LaHangHoa bit = 'true'
		declare @ID_QDChuan uniqueidentifier 
		if @LoaiHangHoa !=1
			set @LaHangHoa ='false'

		declare @QuanLyTheoLoHang bit = 'false'
		IF(@LaHangHoa = 'true' and @MaLoHang!='')  
			SET @QuanLyTheoLoHang = 'true'
  
		DECLARE @ID_NhomHangHoaCha  uniqueidentifier = null;   
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin    		
			select TOP(1) @ID_NhomHangHoaCha = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
				set @ID_NhomHangHoaCha = NEWID()
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha, @TenNhomHangHoaCha_KhongDau, @TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, '1', '1', '1', @LaHangHoa, 'admin', @dtNow, null, 0)
    		End
    	End

    	DECLARE @ID_NhomHangHoa  uniqueidentifier = null; 		
    	if (len(@TenNhomHangHoa) > 0)
    	Begin							
			select TOP(1) @ID_NhomHangHoa = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if @ID_NhomHangHoa is null 
    		BeGin    
				--- neu chuyen tu hanghoa --> dichvu (hoac nguoc lai)
				select TOP(1) @ID_NhomHangHoa = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa  and (TrangThai is NULL or TrangThai = 0)
				if @ID_NhomHangHoa is null	
					begin
						set @ID_NhomHangHoa = NEWID()
    					insert into DM_NhomHangHoa (ID, TenNhomHangHoa, TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    					values (@ID_NhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @MaNhomHangHoa, '1', '1', '1', @LaHangHoa, 'admin', @dtNow, @ID_NhomHangHoaCha, 0)
					end
				--else
					--update DM_NhomHangHoa set LaNhomHangHoa= @LaHangHoa where ID = @ID_NhomHangHoa
    		End
    	End
		else
			begin
				if @LaHangHoa='false' 
					set @ID_NhomHangHoa ='00000000-0000-0000-0000-000000000001'
				else 
					set @ID_NhomHangHoa ='00000000-0000-0000-0000-000000000000'
			end
    
    	DECLARE @LaChaCungLoai  bit  = '1';
		DECLARE @TenCungLoai  nvarchar(max) = ''
    	DECLARE @ID_HangHoaCungLoai  uniqueidentifier  = newID();

    	if(len(@MaHangHoaChaCungLoai) > 0)
    	Begin 
    		select TOP(1) @ID_HangHoaCungLoai = ID_HangHoaCungLoai,
					@TenCungLoai = TenHangHoa  from DM_HangHoa hh
						join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa
						where qd.MaHangHoa = @MaHangHoaChaCungLoai and LaChaCungLoai = '1';
    		if @ID_HangHoaCungLoai  is null   		
    			set @ID_HangHoaCungLoai = newID();
			else 
				begin
    				set @LaChaCungLoai = '0'; 
					if @TenHangHoa='' set @TenHangHoa = @TenCungLoai	
				end
    	End
		begin
			if @isUpdateHang = 1 
				select top 1 @ID_HangHoaCungLoai = ID_HangHoaCungLoai from DM_HangHoa where ID = @ID_HangHoa
		end

		if @isUpdateHang = 1 
			Begin
				if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    				Begin
    					update DM_HangHoa set ID_HangHoaCungLoai = @ID_HangHoaCungLoai, LaChaCungLoai = @LaChaCungLoai, ID_NhomHang = @ID_NhomHangHoa, 
								LaHangHoa = @LaHangHoa, LoaiHangHoa= @LoaiHangHoa,
    							NgaySua = @dtNow, NguoiSua='admin', 
								TenHangHoa = iif(@TenHangHoa='', TenHangHoa, @TenHangHoa), 
								TenHangHoa_KhongDau = iif(@TenHangHoa_KhongDau='', TenHangHoa_KhongDau, @TenHangHoa_KhongDau),   
								TenHangHoa_KyTuDau = iif(@TenHangHoa_KyTuDau='', TenHangHoa_KyTuDau, @TenHangHoa_KyTuDau),
    							TenKhac = @MaHangHoaChaCungLoai, GhiChu = @GhiChu, QuyCach = @QuyCach, DuocBanTrucTiep = @DuocBanTrucTiep, DonViTinhQuyCach= @DVTQuyCach 
						Where ID = @ID_HangHoa
    				end				

    				update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoi, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBan, NguoiSua ='admin', NgaySua =@dtNow, GhiChu= @GhiChu
    				Where ID = @ID_DonViQuiDoi
						
					if(@QuanLyTheoLoHang = 'true')
					begin
						declare @countLo int = (select count(id) from DM_LoHang where id= @ID_LoHang)
						if @countLo = 0
							insert into DM_LoHang (ID, ID_HangHoa, MaLoHang, TenLoHang, NgaySanXuat, NgayHetHan, NguoiTao, NgayTao,TrangThai)
							values (@ID_LoHang, @ID_HangHoa, @MaLoHang, @MaLoHang, @NgaySanXuat, @NgayHetHan, 'admin',@dtNow,1)
						else
							if @NgaySanXuat is not null or @NgayHetHan is not null
								update DM_LoHang set NgaySanXuat = @NgaySanXuat, NgayHetHan = @NgayHetHan where ID = @ID_LoHang																					
					end													
			end
		else
		begin    		
    		if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    			Begin						
    				insert into DM_HangHoa (ID, ID_HangHoaCungLoai, LaChaCungLoai, ID_NhomHang, LaHangHoa, NgayTao,NguoiTao, TenHangHoa,TenHangHoa_KhongDau, TenHangHoa_KyTuDau,
    				TenKhac, TheoDoi, GhiChu, ChiPhiThucHien, ChiPhiTinhTheoPT, QuyCach, DuocBanTrucTiep, QuanLyTheoLoHang, DonViTinhQuyCach, LoaiHangHoa)
    				Values (@ID_HangHoa, @ID_HangHoaCungLoai, @LaChaCungLoai ,@ID_NhomHangHoa, @LaHangHoa, @dtNow, 'admin', @TenHangHoa, @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau,
    				@MaHangHoaChaCungLoai, '1', @GhiChu, '0', '1', @QuyCach, @DuocBanTrucTiep, @QuanLyTheoLoHang, @DVTQuyCach, @LoaiHangHoa)
    
    				if(@MaLoHang != '' and @QuanLyTheoLoHang = 'true')
    				Begin    				    					
    					insert into DM_LoHang(ID, ID_HangHoa, MaLoHang, TenLoHang, NgaySanXuat, NgayHetHan, NguoiTao, NgayTao, TrangThai)
    					values (@ID_LoHang, @ID_HangHoa, @MaLoHang, @MaLoHang, @NgaySanXuat, @NgayHetHan, 'admin', GETDATE(), '1')
    				End
    			end
    		else
    			Begin								
					SELECT TOP(1) @ID_HangHoa = ID_HangHoa, @ID_QDChuan= ID from DonViQuiDoi where MaHangHoa like @MaDonViCoBan
					Select TOP(1) @ID_LoHang = ID FROM DM_LoHang where MaLoHang = @MaLoHang and ID_HangHoa = @ID_HangHoa									
    			end
				
    		insert into DonViQuiDoi (ID, MaHangHoa, TenDonViTinh, ID_HangHoa, TyLeChuyenDoi, LaDonViChuan, GiaVon, GiaNhap, GiaBan, NguoiTao, NgayTao, Xoa, GhiChu)
    		Values (@ID_DonViQuiDoi, @MaHangHoa,@TenDonViTinh, @ID_HangHoa,@TyLeChuyenDoi, @LaDonViChuan, @GiaVon, '0',@GiaBan, 'admin', @dtNow, '0', @GhiChu)			
   		end
	
	exec UpdateTonKho_multipleDVT @isUpdateTonKho, @ID_DonVi, @ID_DonViQuiDoi, @ID_LoHang, @TonKho
END");

            Sql(@"ALTER PROCEDURE [dbo].[import_DoiTuong]
    @MaNhomDoiTuong [nvarchar](max),
    @TenNhomDoiTuong [nvarchar](max),
    @TenNhomDoiTuong_KhongDau [nvarchar](max),
    @TenNhomDoiTuong_KyTuDau [nvarchar](max),
    @MaDoiTuong [nvarchar](max),
    @TenDoiTuong [nvarchar](max),
    @TenDoiTuong_KhongDau [nvarchar](max),
    @TenDoiTuong_ChuCaiDau [nvarchar](max),
    @GioiTinhNam [bit],
    @LoaiDoiTuong [int],
    @LaCaNhan [int],
    @timeCreate [datetime],
    @NgaySinh_NgayTLap [nvarchar](max),
    @DinhDangNgaySinh [nvarchar](max),
    @DiaChi [nvarchar](max),
    @Email [nvarchar](max),
    @Fax [nvarchar](max),
    @web [nvarchar](max),
    @GhiChu [nvarchar](max),
    @DienThoai [nvarchar](max),
    @MaSoThue [nvarchar](max),
    @STK [nvarchar](max),
    @MaHoaDonThu [nvarchar](max),
    @MaHoaDonChi [nvarchar](max),
    @ID_NhanVien [uniqueidentifier],
    @NguoiTao nvarchar(max),
    @ID_DonVi [uniqueidentifier],
    @NoCanThu [nvarchar](max),
    @NoCanTra [nvarchar](max),
	@TongTichDiem [nvarchar](max),
    @MaDieuChinhDiem [nvarchar](max),
	@SoDuThe  [nvarchar](max),
    @MaDieuChinhTheGiaTri [nvarchar](max),
	@TenNguonKhach nvarchar(max),
	@TenTrangThai nvarchar(max)
AS
BEGIN
	set @TenNhomDoiTuong_KhongDau= dbo.FUNC_ConvertStringToUnsign(@TenNhomDoiTuong)
	set @TenDoiTuong_KhongDau= dbo.FUNC_ConvertStringToUnsign(@TenDoiTuong)

	set @TenNhomDoiTuong_KyTuDau= dbo.FUNC_GetStartChar(@TenNhomDoiTuong)
	set @TenDoiTuong_ChuCaiDau= dbo.FUNC_GetStartChar(@TenDoiTuong)

    DECLARE @NoCanThuF as float
    		set @NoCanThuF = (select CAST(ROUND(@NoCanThu, 2) as float))
    DECLARE @NoCanTraF as float
    		set @NoCanTraF = (select CAST(ROUND(@NoCanTra, 2) as float))
	DECLARE @TongTichDiemF as float
    		set @TongTichDiemF = (select CAST(ROUND(@TongTichDiem, 2) as float))
	DECLARE @SoDuTheF as float
    		set @SoDuTheF = (select CAST(ROUND(@SoDuThe, 2) as float))
    DECLARE @NgaySinhTL as Datetime
    	set @NgaySinhTL = null;
    if (len(@NgaySinh_NgayTLap) > 0)
    		Begin
    			Set @NgaySinhTL = (select convert(datetime,@NgaySinh_NgayTLap, 103));
    		end	
    DECLARE @ID_NhomDoiTuong  as uniqueidentifier
    	set @ID_NhomDoiTuong = null
	DECLARE @ID_TrangThai  as uniqueidentifier
    	set @ID_TrangThai = null
	DECLARE @ID_NguonKhach  as uniqueidentifier
    	set @ID_NguonKhach = null
    DECLARE @ID  as uniqueidentifier -- iddoituong
    	SET @ID = NewID();
    DECLARE @ID_QuyHDThu  as uniqueidentifier
    	SET @ID_QuyHDThu = NewID();
    DECLARE @ID_QuyHDTra  as uniqueidentifier
    	SET @ID_QuyHDTra = NewID();
	DECLARE @ID_DieuChinhDiem  as uniqueidentifier
    	SET @ID_DieuChinhDiem = NewID();
	DECLARE @ID_HoaDon  as uniqueidentifier -- dieuchinh thegiatri
    	SET @ID_HoaDon = NewID();


    if (len(@TenNhomDoiTuong) > 0)
    	Begin
    		SET @ID_NhomDoiTuong =  (Select top 1 ID FROM DM_NhomDoiTuong where TenNhomDoiTuong like @TenNhomDoiTuong and LoaiDoiTuong = @LoaiDoiTuong 
			and (TrangThai = 1 or TrangThai is null));
    		if (@ID_NhomDoiTuong is null or len(@ID_NhomDoiTuong) = 0)
    		BeGin
    			SET @ID_NhomDoiTuong = newID();
    			insert into DM_NhomDoiTuong (ID, MaNhomDoiTuong, TenNhomDoiTuong, TenNhomDoiTuong_KhongDau, TenNhomDoiTuong_KyTuDau, LoaiDoiTuong, NguoiTao, NgayTao, TrangThai)
    			values (@ID_NhomDoiTuong, @MaNhomDoiTuong, @TenNhomDoiTuong,@TenDoiTuong_KhongDau, @TenNhomDoiTuong_KyTuDau, @LoaiDoiTuong, @NguoiTao, GETDATE(),1)
    		End
    	End

		if (len(@TenTrangThai) > 0)
    	Begin
    		SET @ID_TrangThai=  (Select top 1 ID FROM DM_DoiTuong_TrangThai where TenTrangThai like  @TenTrangThai)							
    		if (@ID_TrangThai is null or len(@ID_TrangThai) = 0)
    		BeGin
    			SET @ID_TrangThai = newID();
    			insert into DM_DoiTuong_TrangThai (ID, TenTrangThai, NguoiTao, NgayTao)
    			values (@ID_TrangThai, @TenTrangThai, @NguoiTao, GETDATE())
    		End
    	End

		if (len(@TenNguonKhach) > 0)
    	Begin
    		SET @ID_NguonKhach=  (Select top 1 ID FROM DM_NguonKhachHang where TenNguonKhach like   @TenNguonKhach)							
    		if (@ID_NguonKhach is null or len(@ID_NguonKhach) = 0)
    		BeGin
    			SET @ID_NguonKhach = newID();
    			insert into DM_NguonKhachHang (ID, TenNguonKhach, NguoiTao, NgayTao)
    			values (@ID_NguonKhach, @TenNguonKhach, @NguoiTao, GETDATE())
    		End
    	End

    	if (LEN(@DinhDangNgaySinh) < 2)
    	BEGIN
    		set @DinhDangNgaySinh = null;
    	END
    		insert into DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong,TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau,DienThoai, Fax,
    		Email, Website, MaSoThue, TaiKhoanNganHang, GhiChu, NgaySinh_NgayTlap,DinhDang_NgaySinh, chiase, theodoi, DiaChi, GioiTinhNam,
			NguoiTao, NgayTao, ID_DonVi, TongTichDiem, ID_NhanVienPhuTrach, ID_NguonKhach, ID_TrangThai)
    		Values (@ID, @LoaiDoiTuong, @LaCaNhan, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau, @TenDoiTuong_ChuCaiDau, @DienThoai, @Fax,
    		@Email, @web, @MaSoThue,@STK, @GhiChu, @NgaySinhTL,@DinhDangNgaySinh, '0', '0',@DiaChi, @GioiTinhNam,
			@NguoiTao, @timeCreate, @ID_DonVi, @TongTichDiemF,@ID_NhanVien,@ID_NguonKhach , @ID_TrangThai)
    
    		if (@ID_NhomDoiTuong is not null or len (@ID_NhomDoiTuong) > 0)
    		Begin
    			insert into  DM_DoiTuong_Nhom (ID, ID_DoiTuong, ID_NhomDoiTuong)
    			values (NEWID(), @ID, @ID_NhomDoiTuong)
    		End
    	if (@NoCanThuF > 0)
    	Begin
    		insert into Quy_HoaDon (ID, MaHoaDon, NgayLapHoaDon, NgayTao, ID_NhanVien, NguoiNopTien, TongTienThu, ThuCuaNhieuDoiTuong, NguoiTao, ID_DonVi, LoaiHoaDon, HachToanKinhDoanh, PhieuDieuChinhCongNo)
    		values (@ID_QuyHDThu,@MaHoaDonChi,@timeCreate,@timeCreate,@ID_NhanVien,@TenDoiTuong, @NoCanThuF,'0', @NguoiTao, @ID_DonVi, '12','1','1')
    		insert into Quy_HoaDon_ChiTiet (ID, ID_HoaDon, ID_DoiTuong, ThuTuThe, TienMat, TienGui, TienThu, HinhThucThanhToan)
    		values (newID(), @ID_QuyHDThu, @ID, '0', @NoCanThuF, '0', @NoCanThuF, 1)
    	End
		if (@NoCanTraF > 0)
    	Begin
    		insert into Quy_HoaDon (ID, MaHoaDon, NgayLapHoaDon, NgayTao, ID_NhanVien, NguoiNopTien, TongTienThu, ThuCuaNhieuDoiTuong, NguoiTao, ID_DonVi, LoaiHoaDon,HachToanKinhDoanh,PhieuDieuChinhCongNo, TrangThai)
    		values (@ID_QuyHDTra,@MaHoaDonThu,@timeCreate,@timeCreate,@ID_NhanVien,@TenDoiTuong, @NoCanTraF,'0', @NguoiTao, @ID_DonVi, '11','1','1',1)
    		insert into Quy_HoaDon_ChiTiet (ID, ID_HoaDon, ID_DoiTuong, ThuTuThe, TienMat, TienGui, TienThu,HinhThucThanhToan)
    		values (newID(), @ID_QuyHDTra, @ID, '0', @NoCanTraF, '0', @NoCanTraF,1)
    	End
    	if (@TongTichDiemF > 0)
    	Begin
    		insert into Quy_HoaDon (ID, MaHoaDon, NgayLapHoaDon, NgayTao, ID_NhanVien, NguoiNopTien, TongTienThu, ThuCuaNhieuDoiTuong, NguoiTao, ID_DonVi, LoaiHoaDon,NoiDungThu,HachToanKinhDoanh,PhieuDieuChinhCongNo, TrangThai)
    		values (@ID_DieuChinhDiem,@MaDieuChinhDiem,@timeCreate,@timeCreate,@ID_NhanVien,@TenDoiTuong, 0,'0', @NguoiTao, @ID_DonVi, '11',N'Import điều chỉnh điểm','0','1',1)
    		insert into Quy_HoaDon_ChiTiet (ID, ID_HoaDon, ID_DoiTuong, ThuTuThe, TienMat, TienGui, TienThu, DiemThanhToan,GhiChu, HinhThucThanhToan)
    		values (newID(), @ID_DieuChinhDiem, @ID, 0, 0, 0, 0,@TongTichDiemF,N'Import điều chỉnh điểm', 5)
    	End
		if (@SoDuTheF > 0)
    	Begin
    		insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_DoiTuong, ID_DonVi, NgayLapHoaDon, TongTienHang, TongChiPhi, TongGiamGia, TongChietKhau, PhaiThanhToan, TongTienThue, ChoThanhToan, DienGiai)
			values(NEWID(), 23, @MaDieuChinhTheGiaTri, @ID,@ID_DonVi, GETDATE(), @SoDuTheF, @SoDuTheF, 0,0,0,@SoDuTheF,0,N'Import tồn số dư thẻ giá trị')
    	End
		
END");

            Sql(@"ALTER PROCEDURE [dbo].[Insert_LichNhacBaoDuong]
    @ID_HoaDon [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON	
    
    	declare @dtNow datetime = format(DATEADD(day,-30, getdate()),'yyyy-MM-dd')
    
    	declare @SoKmMacDinhNgay int= 30, @countSC int =0;
    	---- getthongtin hoadon	
    	declare @NgayLapHoaDon datetime , @ID_Xe uniqueidentifier, @ID_PhieuTiepNhan uniqueidentifier, @Now_SoKmVao float, @Now_NgayVaoXuong datetime
    	select @ID_PhieuTiepNhan = ID_PhieuTiepNhan, @NgayLapHoaDon = NgayLapHoaDon from BH_HoaDon where id= @ID_HoaDon
    
    		---- get thongtin tiepnhan hientai		
    	select @Now_SoKmVao = isnull(SoKmVao,0), @ID_Xe = ID_Xe, @Now_NgayVaoXuong = NgayVaoXuong
    	from Gara_PhieuTiepNhan
    	where ID = @ID_PhieuTiepNhan
    			
    
    	---- get thongtin tiepnhan gan nhat
    	declare @NgayXuatXuong_GanNhat datetime , @SoKmRa_GanNhat float
    	select top 1 @NgayXuatXuong_GanNhat = isnull(NgayXuatXuong, NgayVaoXuong) ,  @SoKmRa_GanNhat= ISNULL(SoKmRa,0) 
    	from Gara_PhieuTiepNhan where isnull(NgayXuatXuong, NgayVaoXuong) < @Now_NgayVaoXuong and ID_Xe= @ID_Xe 
    	order by NgayVaoXuong
    
    	if @NgayXuatXuong_GanNhat is not null
    		begin
    			set @SoKmMacDinhNgay =  CEILING( iif(@Now_SoKmVao - @SoKmRa_GanNhat=0,1, @Now_SoKmVao -@SoKmRa_GanNhat)/ iif(DATEDIFF(day, @NgayXuatXuong_GanNhat, @Now_NgayVaoXuong)=0,1,DATEDIFF(day, @NgayXuatXuong_GanNhat, @Now_NgayVaoXuong)))
    		end
    
    
    	----- get chitiet phutung thuoc hoadon có cài đặt bảo dưỡng
    	select distinct bd.LoaiBaoDuong,
    			bd.ID_HangHoa, 
    			bd.LanBaoDuong, 
    			iif(bd.BaoDuongLapDinhKy=1, bd.GiaTri,0) as GiaTriLap,
    			(select dbo.BaoDuong_GetTongGiaTriNhac(bd.LanBaoDuong,bd.ID_HangHoa)) as GiaTri,	
    			bd.LoaiGiaTri,
    			bd.BaoDuongLapDinhKy, bd.ID_LichBaoDuong, bd.GhiChu
    	into #tmpPhuTung
    	from 
    	(select hh.LoaiBaoDuong, qd.ID_HangHoa,  bd.LanBaoDuong, 
    		bd.GiaTri,	
    		bd.LoaiGiaTri,
    		bd.BaoDuongLapDinhKy,
    		max(ct.ID_LichBaoDuong) as ID_LichBaoDuong,
    		max(ct.GhiChu) as GhiChu
    		from BH_HoaDon_ChiTiet ct		
    		join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    		join DM_HangHoa_BaoDuongChiTiet bd on hh.ID= bd.ID_HangHoa	
    		where ct.ID_HoaDon= @ID_HoaDon
    		and hh.QuanLyBaoDuong=1
    		and hh.LoaiBaoDuong !=0
    		and (ct.ID_ChiTietDinhLuong is null or ct.ID= ct.ID_ChiTietDinhLuong)
    		group by qd.ID_HangHoa,hh.LoaiBaoDuong, qd.ID_HangHoa,  bd.LanBaoDuong, 
    		bd.GiaTri,	
    		bd.LoaiGiaTri,
    		bd.BaoDuongLapDinhKy
    	) bd
    	order by bd.LanBaoDuong desc ---- nếu cùng 1 phụ tùng (vừa mua mới + bảo dưỡng) --> ưu tiên lấy phụ tùng bảo dưỡng	
    
    	---- get phụ tùng đã có lịch bảo dưỡng, và chưa dc xử lý
    		select lich.ID, lich.LanBaoDuong as LanBaoDuongThu, lich.SoKmBaoDuong, lich.NgayBaoDuongDuKien, pt.*
    		into #tmpLich
    		from #tmpPhuTung pt
    		join Gara_LichBaoDuong lich on lich.ID_Xe= @ID_Xe and lich.ID_HangHoa= pt.ID_HangHoa
    		where lich.TrangThai = 1 
    			
    		---- Nếu phụ tùng được thay mới ----> update lichcu trangthai =0) + insert lịch mới
    		update  lich set lich.TrangThai= 0
    		from Gara_LichBaoDuong lich
    		where exists (
    		select *
    		from #tmpLich tmp
    		where tmp.ID_LichBaoDuong is null and tmp.ID= lich.ID)
    		 
    			insert into Gara_LichBaoDuong (ID, ID_HangHoa, ID_HoaDon, ID_Xe, LanBaoDuong, SoKmBaoDuong, NgayBaoDuongDuKien, TrangThai, NgayTao, GhiChu, LanNhac)
    			select NEWID() as ID, a.ID_HangHoa,@ID_HoaDon, @ID_Xe, a.LanBaoDuong,
    				a.SoKmBaoDuong, a.NgayBaoDuongDuKien, a.TrangThai, a.NgayTao, a.GhiChu, 0
    			from
    			(
    				select pt.ID_HangHoa, pt.LanBaoDuong,
    				case pt.LoaiBaoDuong
    					when 2 then @Now_SoKmVao + pt.GiaTri --- chi lưu cột này nếu loại bảo dưỡng = KM
    					else 0 end as SoKmBaoDuong,
    				case pt.LoaiBaoDuong
    					when 2 then DATEADD(day, CEILING( pt.GiaTri/@SoKmMacDinhNgay), @NgayLapHoaDon) --- get số ngày theo km mặc định + thời gian tiếp nhận
    					when 1 then DATEADD(day, pt.GiaTri, @NgayLapHoaDon)	
    				end as NgayBaoDuongDuKien,
    				1 as TrangThai,
    				GETDATE() as NgayTao,
    				pt.GhiChu
    				from
    				(
    					select distinct  ID_HangHoa, LanBaoDuong, LoaiBaoDuong, GiaTri, LoaiGiaTri, GhiChu
    					from #tmpLich 
    					where ID_LichBaoDuong is null --- phụ tùng có lịch bảo dưỡng, nhưng thay mới
    				) pt
    			) a where a.NgayBaoDuongDuKien >= @dtNow
    		
    			---- Nếu phụ tùng đi bảo dưỡng, nhưng ngày bảo dưỡng (hiện tại) gần sát với ngày nhắc dự kiến lần tiếp theo --> xóa và thêm mới
    			---- (gần sát ở đây dc mặc định là < 1/2 thời gian)
    			select *
    			into #lichSatNgay
    			from
    			(
    				select lichOld.ID, lichOld.LanBaoDuongThu, lichOld.ID_HangHoa,
    					 lichOld.LanBaoDuong, lichOld.LoaiBaoDuong, lichOld.GiaTri, lichOld.LoaiGiaTri,
    					lichOld.GiaTri as GiaTriMin,
    					lichOld.SoKmBaoDuong, @Now_SoKmVao as sokmvaomow, GhiChu,
    					case lichOld.LoaiGiaTri
    						when 4 then lichOld.SoKmBaoDuong - @Now_SoKmVao
    						else
    						DATEDIFF(day,@NgayLapHoaDon, lichOld.NgayBaoDuongDuKien) end as GiaTriLech --- chênh lệch giữa ngày bảo dưỡng hiện tại và ngày dự kiến
    				from #tmpLich lichOld
    				where ID_LichBaoDuong is not null -- phụ tùng đi bảo dưỡng
    				and (lichOld.LanBaoDuong = lichOld.LanBaoDuongThu + 1)				
    			) b where b.GiaTriLech >  b.GiaTriMin/2 --- neu tiepnhanxe voi sokmvao = 0, giatrilech bi am			
    			
    			
    		  ---- Xoa va insert lai
    			delete  lich			
    			from Gara_LichBaoDuong lich
    			where exists (
    			select tmp.ID from #lichSatNgay tmp where  tmp.ID= lich.ID)
    			
    			
    			insert into Gara_LichBaoDuong (ID, ID_HangHoa, ID_HoaDon, ID_Xe, LanBaoDuong, SoKmBaoDuong, NgayBaoDuongDuKien, TrangThai, NgayTao, GhiChu, LanNhac)
    			select NEWID() as ID, a.ID_HangHoa,@ID_HoaDon, @ID_Xe, a.LanBaoDuong,
    				a.SoKmBaoDuong, a.NgayBaoDuongDuKien, a.TrangThai, a.NgayTao, a.GhiChu,0
    				from
    				(
    					select pt.ID_HangHoa, pt.LanBaoDuong, pt.GhiChu,
    					case pt.LoaiBaoDuong
    						when 2 then @Now_SoKmVao + pt.GiaTri --- chi lưu cột này nếu loại bảo dưỡng = KM
    						else 0 end as SoKmBaoDuong,
    					case pt.LoaiBaoDuong
    						when 2 then DATEADD(day, CEILING( pt.GiaTri/@SoKmMacDinhNgay), @NgayLapHoaDon)
    						when 1 then DATEADD(day, pt.GiaTri, @NgayLapHoaDon)										
    					end as NgayBaoDuongDuKien,
    					1 as TrangThai,
    					GETDATE() as NgayTao
    					from 
    					(
    						select distinct  ID_HangHoa, LanBaoDuong, LoaiBaoDuong, GiaTri, LoaiGiaTri, GhiChu
    						from #lichSatNgay 			
    					) pt
    				
    			) a where a.NgayBaoDuongDuKien >= @dtNow
    			
    
    			----- insert phutung nếu chưa có trong lịch bảo dưỡng 
    			insert into Gara_LichBaoDuong (ID, ID_HangHoa, ID_HoaDon, ID_Xe, LanBaoDuong, SoKmBaoDuong, NgayBaoDuongDuKien, TrangThai, NgayTao, GhiChu, LanNhac)
    			select NEWID() as ID, a.ID_HangHoa, @ID_HoaDon, @ID_Xe, a.LanBaoDuong,
    				a.SoKmBaoDuong, a.NgayBaoDuongDuKien, a.TrangThai, a.NgayTao, a.GhiChu,0
    			from
    			(
    				select  pt.ID_HangHoa, pt.LanBaoDuong, pt.GhiChu,
    				case pt.LoaiBaoDuong
    					when 2 then @Now_SoKmVao + pt.GiaTri --- chi lưu cột này nếu loại bảo dưỡng = KM
    					else 0 end as SoKmBaoDuong,
    				case pt.LoaiBaoDuong
    					when 2 then DATEADD(day, CEILING( pt.GiaTri/@SoKmMacDinhNgay), @NgayLapHoaDon)
    					when 1 then DATEADD(day, pt.GiaTri, @NgayLapHoaDon)									
    				end as NgayBaoDuongDuKien,
    				1 as TrangThai,
    				GETDATE() as NgayTao
    				from #tmpPhuTung pt
    				where not exists (
    					select lich.ID from Gara_LichBaoDuong lich
    					where lich.ID_HangHoa= pt.ID_HangHoa and lich.ID_Xe= @ID_Xe 
						and (
							(pt.BaoDuongLapDinhKy =1 and lich.TrangThai !=0)
							or (pt.BaoDuongLapDinhKy =0 )
							)
						
    				) 
    			) a where a.NgayBaoDuongDuKien >= @dtNow --- chi insert neu lich > ngayhientai
    
    			---- insert phutung da colich, nhung cai dat lap lai
    			insert into Gara_LichBaoDuong (ID, ID_HangHoa, ID_HoaDon, ID_Xe, LanBaoDuong, SoKmBaoDuong, NgayBaoDuongDuKien, TrangThai, NgayTao, GhiChu, LanNhac)
    			select NEWID() as ID, a.ID_HangHoa, @ID_HoaDon,  @ID_Xe, a.LanBaoDuongThu,
    				a.SoKmBaoDuong, a.NgayBaoDuongDuKien, a.TrangThai, a.NgayTao, a.GhiChu,0
    			from
    			(
    				select  pt.ID_HangHoa, pt.LanBaoDuongThu, pt.GhiChu,
    					case pt.LoaiBaoDuong
    						when 2 then @Now_SoKmVao + pt.GiaTri --- lay sokmbaoduong o lancuoicung (cua lichbaoduong) + giatri
    						else 0 end as SoKmBaoDuong,
    					case pt.LoaiBaoDuong
    						when 2 then DATEADD(day, CEILING( pt.GiaTri/@SoKmMacDinhNgay), @NgayLapHoaDon) --- get số ngày theo km mặc định + thời gian tiếp nhận
    						when 1 then DATEADD(day, pt.GiaTri, @NgayLapHoaDon)									
    					end as NgayBaoDuongDuKien,
    					1 as TrangThai,
    					GETDATE() as NgayTao
    				from
    					(----- get phutung da co lichbaoduong, va caidat laplai	
    					----- chi insert neu lanbaoduong cuoicung da duoc xuly baoduong (trangthai = 2)
    					select *
    					from (
    						select lich.ID, lich.TrangThai, lich.LanBaoDuong + 1 as LanBaoDuongThu, lich.SoKmBaoDuong, lich.NgayBaoDuongDuKien, pt.*,
    						ROW_NUMBER() over(partition by lich.ID_HangHoa order by lich.LanBaoDuong desc) as RN
    						from #tmpPhuTung pt
    						join Gara_LichBaoDuong lich on lich.ID_Xe= @ID_Xe and lich.ID_HangHoa= pt.ID_HangHoa
    						where pt.BaoDuongLapDinhKy= 1 and lich.TrangThai !=0 ---- khong get lich bi xoa				
    					) b where b.RN= 1 and b.TrangThai = 2			
    				) pt  where pt.RN= 1
    			) a where a.NgayBaoDuongDuKien >= @dtNow --- chi insert neu lich > ngayhientai
END");

            Sql(@"ALTER PROCEDURE [dbo].[insertChotSo_XuatNhapTon]
    @NgayChotSo [datetime],
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
	set nocount on;
	declare @ToDate datetime = dateadd(day,1,@NgayChotSo)
	
	declare @ngayChotOld datetime = (select top 1 NgayChotSo from ChotSo_HangHoa)
	if @ngayChotOld is null or @NgayChotSo < @ngayChotOld --- chot so lui ngay
	begin
		--- chua chot so lan nao	
		set @ngayChotOld = '2016-01-01'
	end	
	
	---- get tonkho Từ đầu - thời gian chốt sổ
	 select 
				tkTrongKy.ID_HangHoa,
				tkTrongKy.ID_LoHang,
				tkTrongKy.SoLuongNhap - tkTrongKy.SoLuongXuat as SoLuongTon,
				tkTrongKy.GiaTriNhap - tkTrongKy.GiaTriXuat as GiaTriTon	
			into #tblTrongKy
			 from
			 (
					select 			
						qd.ID_HangHoa,
						tkNhapXuat.ID_LoHang,
						tkNhapXuat.ID_DonVi,				
						sum(tkNhapXuat.SoLuongNhap * qd.TyLeChuyenDoi) as SoLuongNhap,
						sum(tkNhapXuat.GiaTriNhap ) as GiaTriNhap,
						sum(tkNhapXuat.SoLuongXuat * qd.TyLeChuyenDoi) as SoLuongXuat,
						sum(tkNhapXuat.GiaTriXuat) as GiaTriXuat
					from
					(
					-- xuat ban, trahang ncc, xuatkho, xuat chuyenhang
						select 
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_DonVi,
							0 AS SoLuongNhap,
							0 AS GiaTriNhap,
							sum(
								case hd.LoaiHoaDon
								when 10 then ct.TienChietKhau
								else ct.SoLuong end ) as SoLuongXuat,
							sum( 
								case hd.LoaiHoaDon
								when 7 then ct.SoLuong* ct.DonGia
								when 10 then ct.TienChietKhau * ct.GiaVon
								else ct.SoLuong* ct.GiaVon end )  AS GiaTriXuat
							FROM BH_HoaDon_ChiTiet ct
						LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
						WHERE hd.ChoThanhToan = 0
						and (hd.LoaiHoaDon in (1,5,7,8) 
							or (hd.LoaiHoaDon = 10  and (hd.YeuCau='1' or hd.YeuCau='4')) )
						AND hd.NgayLapHoaDon between  @ngayChotOld AND  @ToDate
						and hd.ID_DonVi= @ID_ChiNhanh
						GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi								


						UNION ALL
						 ---nhap chuyenhang
						SELECT 
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_CheckIn AS ID_DonVi,
							SUM(ct.TienChietKhau) AS SoLuongNhap,
							SUM(ct.TienChietKhau* ct.DonGia) AS GiaTriNhap, -- lay giatri tu chinhanh chuyen
							0 AS SoLuongXuat,
							0 AS GiaTriXuat
						FROM BH_HoaDon_ChiTiet ct
						LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
						WHERE hd.LoaiHoaDon = 10 and hd.YeuCau = '4' AND hd.ChoThanhToan = 0
						and hd.ID_DonVi= @ID_ChiNhanh
						AND hd.NgaySua between  @ngayChotOld AND @ToDate
						GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn

    					UNION ALL
						 ---nhaphang + khach trahang
						SELECT 
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_DonVi,
							SUM(ct.SoLuong) AS SoLuongNhap,
							--- KH trahang: giatrinhap = giavon (khong lay giaban)
							sum(iif(hd.LoaiHoaDon= 6, iif(ctm.GiaVon is null or ctm.ID = ctm.ID_ChiTietDinhLuong, ct.SoLuong * ct.GiaVon, ct.SoLuong *ctm.GiaVon),
							iif( hd.TongTienHang = 0,0, ct.SoLuong* (ct.DonGia - ct.TienChietKhau) * (1- hd.TongGiamGia/hd.TongTienHang))))  AS GiaTriNhap,
							0 AS SoLuongXuat,
							0 AS GiaTriXuat
						FROM BH_HoaDon_ChiTiet ct
						LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
						left join BH_HoaDon_ChiTiet ctm on ct.ID_ChiTietGoiDV = ctm.ID
						WHERE (hd.LoaiHoaDon = '4' or hd.LoaiHoaDon = '6') 
						AND hd.ChoThanhToan = 0
						and hd.ID_DonVi= @ID_ChiNhanh
						AND hd.NgayLapHoaDon between  @ngayChotOld AND @ToDate
						GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi
    
    					UNION ALL
						-- kiemke
    					SELECT 
							ctkk.ID_DonViQuiDoi, 
							ctkk.ID_LoHang, 
							ctkk.ID_DonVi, 
							sum(isnull(SoLuongNhap,0)) as SoLuongNhap,
							sum(isnull(SoLuongNhap,0) * ctkk.GiaVon) as GiaTriNhap,
							sum(isnull(SoLuongXuat,0)) as SoLuongXuat,
							sum(isnull(SoLuongXuat,0) * ctkk.GiaVon) as GiaTriXuat
						FROM
    					(SELECT 
    						ct.ID_DonViQuiDoi,
    						ct.ID_LoHang,
							hd.ID_DonVi,
							IIF(ct.SoLuong< 0, 0, ct.SoLuong) as SoLuongNhap,
							IIF(ct.SoLuong < 0, - ct.SoLuong, 0) as SoLuongXuat,
							ct.GiaVon
    					FROM BH_HoaDon_ChiTiet ct 
    					LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
    					WHERE hd.LoaiHoaDon = '9' 
    					AND hd.ChoThanhToan = 0
						and hd.ID_DonVi= @ID_ChiNhanh
    					AND hd.NgayLapHoaDon between  @ngayChotOld AND @ToDate	
						) ctkk	
    					GROUP BY ctkk.ID_DonViQuiDoi, ctkk.ID_LoHang, ctkk.ID_DonVi
					)tkNhapXuat
					join DonViQuiDoi qd on tkNhapXuat.ID_DonViQuiDoi = qd.ID 
					group by qd.ID_HangHoa,tkNhapXuat.ID_LoHang, tkNhapXuat.ID_DonVi
				) tkTrongKy			
			
			
		---- check hàng chốt sổ bị sai
		select cs.*, tk.SoLuongTon as TonKhoDung, tk.GiaTriTon as GtriTonDung
		into #tblChotSoSai
		from ChotSo_HangHoa cs
		left join #tblTrongKy tk on cs.ID_HangHoa = tk.ID_HangHoa and (cs.ID_LoHang= tk.ID_LoHang or cs.ID_LoHang is null and tk.ID_LoHang is null)
		where cs.ID_DonVi= @ID_ChiNhanh
		and (cs.TonKho != tk.SoLuongTon )--or cs.GiaTriTon != tk.GiaTriTon)


		----- xóa hàng chốt sai && insert again
		delete cs		
		from ChotSo_HangHoa cs
		where exists (
		select ID from #tblChotSoSai tblSai where cs.ID= tblSai.ID
		)

		---- insert again hàng sai
		insert into ChotSo_HangHoa (ID, ID_DonVi, ID_HangHoa, ID_LoHang, NgayChotSo, TonKho) --, GiaTriTon) --- todo giatri ton
		select newID(),
			@ID_ChiNhanh,			
			ID_HangHoa,
			ID_LoHang,
			@NgayChotSo,
			ISNULL(tk.TonKhoDung,0) as TonKho --,
			--ISNULL(tk.GtriTonDung,0)  as GiaTriTon
		from #tblChotSoSai tk
	

		---- insert hàng chưa chốt sổ 
		insert into ChotSo_HangHoa (ID, ID_DonVi, ID_HangHoa, ID_LoHang, NgayChotSo, TonKho) --, GiaTriTon) --- todo giatri ton
		select newID(),
			@ID_ChiNhanh,			
			ID_HangHoa,
			ID_LoHang,
			@NgayChotSo,
			ISNULL(tk.SoLuongTon,0) as TonKho --,
			--ISNULL(tk.GtriTonDung,0)  as GiaTriTon
		from #tblTrongKy tk
	
END");

            Sql(@"ALTER PROCEDURE [dbo].[InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac]
    @ID_LichBaoDuong [uniqueidentifier],
    @TypeUpdate [int]
AS
BEGIN
    SET NOCOUNT ON;
    	declare @maxLanBaoDuong int, @max_soKmBaoDuong int, @now_LanBaoDuong int, @now_NgayBaoDuongDuKien datetime, @now_LanNhac int,
    	@ID_Xe uniqueidentifier, @ID_HoaDon uniqueidentifier, @ID_HangHoa uniqueidentifier
    	--- get infor lich
    	select  @ID_Xe = ID_Xe, @ID_HangHoa= ID_HangHoa, @ID_HoaDon= ID_HoaDon,
    		@now_NgayBaoDuongDuKien = NgayBaoDuongDuKien,
    		@now_LanBaoDuong = LanBaoDuong,
    		@now_LanNhac = LanNhac
    	from Gara_LichBaoDuong
    	where ID= @ID_LichBaoDuong
    	
    	--- get max lanbaoduong
    	select top 1 @maxLanBaoDuong = LanBaoDuong, @max_soKmBaoDuong= SoKmBaoDuong
    	from Gara_LichBaoDuong where ID_Xe= @ID_Xe and ID_HangHoa = @ID_HangHoa and TrangThai !=0 order by LanBaoDuong desc
    
    	
    	---- getcaidat lich
    	select *,
    		iif(LoaiGiaTri < 4, 1, 2) as LoaiBaoDuong --- 1.thoigian, 2.km
    	into #tblSetup
    	from DM_HangHoa_BaoDuongChiTiet where ID_HangHoa= @ID_HangHoa and BaoDuongLapDinhKy = 1
    
    	declare @countRepeater int = (select count(id) from #tblSetup)
    
    	select * from #tblSetup
    
    	
    
    	--- chi insert neu max lanbaoduong va laplai
    	if @now_LanBaoDuong = @maxLanBaoDuong and @countRepeater > 0
    	begin
    		
    		---- getthongtin tiepnhan theohoadon		
    		declare @SoKmMacDinhNgay int= 30, @countSC int =0;
    		declare  @ID_PhieuTiepNhan uniqueidentifier, @Now_SoKmVao float, @Now_NgayVaoXuong datetime
    		select @ID_PhieuTiepNhan = ID_PhieuTiepNhan from BH_HoaDon where id= @ID_HoaDon
    
    			---- get thongtin tiepnhan hientai		
    		select @Now_SoKmVao = isnull(SoKmVao,0), @ID_Xe = ID_Xe, @Now_NgayVaoXuong = NgayVaoXuong
    		from Gara_PhieuTiepNhan
    		where ID = @ID_PhieuTiepNhan
    
    		---- get thongtin tiepnhan gan nhat
    		declare @NgayXuatXuong_GanNhat datetime , @SoKmRa_GanNhat float
    		select top 1 @NgayXuatXuong_GanNhat = isnull(NgayXuatXuong, NgayVaoXuong) ,  @SoKmRa_GanNhat= ISNULL(SoKmRa,0) 
    		from Gara_PhieuTiepNhan where isnull(NgayXuatXuong, NgayVaoXuong) < @Now_NgayVaoXuong and ID_Xe= @ID_Xe 
    		order by NgayVaoXuong 
    
    	if @NgayXuatXuong_GanNhat is not null
    		begin
    			set @SoKmMacDinhNgay =  CEILING( iif(@Now_SoKmVao - @SoKmRa_GanNhat=0,1, @Now_SoKmVao -@SoKmRa_GanNhat)/ iif(DATEDIFF(day, @NgayXuatXuong_GanNhat, @Now_NgayVaoXuong)=0,1,DATEDIFF(day, @NgayXuatXuong_GanNhat, @Now_NgayVaoXuong)))
    		end
    
    	--	select @now_LanBaoDuong as now_LanBaoDuong,@maxLanBaoDuong as maxLanBaoDuong ,
    	--@max_soKmBaoDuong as max_soKmBaoDuong, @countRepeater as countRepeater,
    	--@now_NgayBaoDuongDuKien as now_NgayBaoDuongDuKien,
    	--@SoKmMacDinhNgay as SoKmMacDinhNgay,
    	--@Now_SoKmVao as kmvao,
    	-- @SoKmRa_GanNhat as kmra,
    	-- @NgayXuatXuong_GanNhat as ngayxuat,
    	-- @Now_NgayVaoXuong as ngayvao
    			
    		if @TypeUpdate= 1 --- quahan		
    			goto InsertLichNhac						  
    		
    		if @TypeUpdate= 2 --- đủ số lần nhắc
    		begin
    			declare @countSetup int , @SoLanNhac int
    			select @countSetup = COUNT(ID) OVER (), @SoLanNhac = SoLanLapLai from HT_ThongBao_CatDatThoiGian where LoaiThongBao= 4
    			select @countSetup , @SoLanNhac
    			if @countSetup > 0 and @SoLanNhac = @now_LanNhac
    				goto InsertLichNhac
    		end
    		
    		InsertLichNhac:
    			insert into Gara_LichBaoDuong (ID, ID_HangHoa, ID_HoaDon, ID_Xe, LanBaoDuong, SoKmBaoDuong, NgayBaoDuongDuKien, TrangThai, NgayTao, GhiChu)
    			select newid(), @ID_HangHoa, @ID_HoaDon, @ID_Xe, @maxLanBaoDuong + 1, SoKmBaoDuong, NgayBaoDuongDuKien, 1, GETDATE(),''
    			from(
    			select 
    					case pt.LoaiBaoDuong
    						when 2 then @max_soKmBaoDuong + pt.GiaTri
    						else 0 end as SoKmBaoDuong,
    					case pt.LoaiBaoDuong
    						when 2 then DATEADD(day, CEILING(pt.GiaTri/@SoKmMacDinhNgay), @now_NgayBaoDuongDuKien)																			
    						when 1 then DATEADD(day, pt.GiaTri, @now_NgayBaoDuongDuKien)										
    					end as NgayBaoDuongDuKien				
    				from #tblSetup pt
    				) pt			
    	end  
    	drop table #tblSetup
END");

            Sql(@"ALTER PROCEDURE [dbo].[Load_DMHangHoa_TonKho]
	 @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN

	SET NOCOUNT ON;
	declare @dateNow datetime = FORMAT(getdate(),'yyyyMMdd')
	declare @next10Year Datetime = FORMAT(dateadd(year,10, getdate()),'yyyyMMdd')

		select 
			dhh1.ID,
			dvqd1.ID as ID_DonViQuiDoi,		
			MAX(ROUND(ISNULL(tk.TonKho,0),2)) as TonKho,
			MAX(CAST(ROUND(( ISNULL(gv.GiaVon,0)), 0) as float)) as GiaVon,
			TenHangHoa,
			MaHangHoa,
			LaDonViChuan,  
			LaHangHoa,
			dhh1.TonToiThieu,
			ID_NhomHang as ID_NhomHangHoa,
			ISNULL(QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
			Case when dhh1.LaHangHoa='1' then 0 else CAST(ISNULL(dhh1.ChiPhiThucHien,0) as float) end as PhiDichVu,
			Case when dhh1.LaHangHoa='1' then '0' else ISNULL(dhh1.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
			ISNULL(lh1.ID,  NEWID()) as ID_LoHang,
			case when MAX(ISNULL(QuyCach,0)) = 0 then MAX(TyLeChuyenDoi) else MAX(QuyCach) * MAX(TyLeChuyenDoi) end as QuyCach,	
			MAX(ISNULL(TyLeChuyenDoi,0)) as TyLeChuyenDoi, 		
			isnull(TenHangHoa_KhongDau,'') as TenHangHoa_KhongDau,
			CONCAT(MaHangHoa, ' ' , lower(MaHangHoa) ,' ', TenHangHoa, ' ', TenHangHoa_KhongDau,' ',
			MAX(MaLoHang),' ', Cast(max(GiaBan) as decimal(22,0)), MAX(ISNULL(dvqd1.ThuocTinhGiaTri,''))) as Name,
    		MAX(ISNULL(dvqd1.ThuocTinhGiaTri,'')) as ThuocTinh_GiaTri,
    		MAX(GiaBan) as GiaBan, 
    		MAX (TenDonViTinh) as TenDonViTinh, 	
			case when MAX(ISNULL(an.URLAnh,'')) = '' then '' else 'CssImg' end as CssImg,		
    		MAX(ISNULL(an.URLAnh,'')) as SrcImage, 		
    		MAX(ISNULL(MaLoHang,'')) as MaLoHang,
    		MAX(NgaySanXuat) as NgaySanXuat,
    		MAX(NgayHetHan) as NgayHetHan,
			MAX(ISNULL(DonViTinhQuyCach,'')) as DonViTinhQuyCach,
			MAX(ISNULL(ThoiGianBaoHanh,0)) as ThoiGianBaoHanh,
			MAX(ISNULL(LoaiBaoHanh,0)) as LoaiBaoHanh,
			MAX(ISNULL(SoPhutThucHien,0)) as SoPhutThucHien, 
			MAX(ISNULL(dhh1.GhiChu,'')) as GhiChuHH ,
			MAX(ISNULL(dhh1.DichVuTheoGio,0)) as DichVuTheoGio, 
			MAX(ISNULL(dhh1.DuocTichDiem,0)) as DuocTichDiem, 
			MAX(ISNULL(dhh1.HoaHongTruocChietKhau,0)) as HoaHongTruocChietKhau, 
			0 as SoGoiDV,
			@next10Year as HanSuDungGoiDV_Min,
			'' as BackgroundColor,
			iif(dhh1.LoaiHangHoa is null, iif(dhh1.LaHangHoa='1',1,2), dhh1.LoaiHangHoa) as LoaiHangHoa,
			isnull(nhom.TenNhomHangHoa,N'Nhóm mặc định') as TenNhomHangHoa
		from DonViQuiDoi dvqd1 
		join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
		left join DM_NhomHangHoa nhom on dhh1.ID_NhomHang = nhom.ID
		left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa
		left join DM_HangHoa_TonKho tk on (dvqd1.ID = tk.ID_DonViQuyDoi and (lh1.ID = tk.ID_LoHang or lh1.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		left join DM_HangHoa_Anh an on (dvqd1.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))		
		left join DM_GiaVon gv on (dvqd1.ID = gv.ID_DonViQuiDoi and (lh1.ID = gv.ID_LoHang or lh1.ID is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where (dvqd1.xoa ='0'  or dvqd1.Xoa is null)
		and dhh1.TheoDoi = '1'		 
		and (dhh1.LaHangHoa = 0 or (dhh1.LaHangHoa = 1 and tk.TonKho is not null)) -- chi lay HangHoa neu exsit in DM_TonKho_HangHoa
		and (lh1.NgayHetHan is null or (lh1.NgayHetHan >= @dateNow))		
		group by dhh1.ID, dvqd1.ID, lh1.ID, MaHangHoa,ID_NhomHang,TenHangHoa,TenHangHoa_KhongDau,TenHangHoa_KyTuDau,
		LaDonViChuan,LaHangHoa,ChiPhiThucHien,ChiPhiTinhTheoPT, dhh1.QuanLyTheoLoHang, dhh1.TonToiThieu, dhh1.LoaiHangHoa,nhom.TenNhomHangHoa
		order by MaHangHoa,NgayHetHan
END");

            Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMucHangHoa]
    @MaHH [nvarchar](max),
    @MaHHCoDau [nvarchar](max),
    @ListID_NhomHang [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @KinhDoanhFilter [nvarchar](max),
    @LoaiHangHoas [nvarchar](max),
    @List_ThuocTinh [nvarchar](max)
AS
BEGIN
	SET NOCOUNT ON;
    DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    	DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    		INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHHCoDau+',') where Name!='';
    			  Select @count =  (Select count(*) from @tablename);
    	    Select @countChar =   (Select count(*) from @tablenameChar);

			declare @tblLoaiHang table(LoaiHang int)
			insert into @tblLoaiHang
			select name from dbo.splitstring(@LoaiHangHoas)

    if(@MaHH = '' and @MaHHCoDau = '')
    BEGIN
		
		if(@List_ThuocTinh != '')
		BEGIN
			select *,
			case LoaiHangHoa 
				when 1 then N'Hàng hóa'
				when 2 then N'Dịch vụ'
				when 3 then N'Combo'
			end as sLoaiHangHoa

			into #dmhanghoatable1
			from
			(
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang,hhtt.GiaTri + CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.LaChaCungLoai,
			hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, 
			dnhh.TenNhomHangHoa as NhomHangHoa , hh.TenHangHoa, dvqd.TenDonViTinh, dvqd.ThuocTinhGiaTri,
			isnull(hh.SoPhutThucHien,0) as SoPhutThucHien,
			isnull(hh.DichVuTheoGio,0) as DichVuTheoGio,
			isnull(hh.DuocTichDiem,0) as DuocTichDiem,
			iif(hh.QuanLyBaoDuong is null,0, hh.QuanLyBaoDuong) as QuanLyBaoDuong,
			iif(hh.LoaiBaoDuong is null,0, hh.LoaiBaoDuong) as LoaiBaoDuong,
			iif(hh.SoKmBaoHanh is null,0, hh.SoKmBaoHanh) as SoKmBaoHanh,	
			iif(hh.HoaHongTruocChietKhau is null,0, hh.HoaHongTruocChietKhau) as HoaHongTruocChietKhau,		
			iif(hh.LoaiHangHoa is null,iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			iif(hh.LaHangHoa='1', isnull(gv.GiaVon,0), dbo.GetGiaVonOfDichVu(@ID_ChiNhanh,dvqd.ID)) as GiaVon,					
			dvqd.GiaBan,
			dvqd.Xoa,
			ISNULL(hhtonkho.TonKho,0) as TonKho			
			from DonViQuiDoi dvqd
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			LEFT JOIN HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
			where dvqd.ladonvichuan = 1 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
			and hh.TheoDoi like @KinhDoanhFilter
			) a where exists (select LoaiHang from @tblLoaiHang loai where a.LoaiHangHoa = loai.LoaiHang)

			Select * from #dmhanghoatable1 hhtb2
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
		END
		ELSE
		BEGIN
			select *,
			case LoaiHangHoa 
				when 1 then N'Hàng hóa'
				when 2 then N'Dịch vụ'
				when 3 then N'Combo'
			end as sLoaiHangHoa
			into #dmhanghoatable2
			from
			(
				select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang, hh.LaChaCungLoai,
				hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, 
				dnhh.TenNhomHangHoa as NhomHangHoa , hh.TenHangHoa, dvqd.TenDonViTinh, dvqd.ThuocTinhGiaTri,
				isnull(hh.SoPhutThucHien,0) as SoPhutThucHien,
				isnull(hh.DichVuTheoGio,0) as DichVuTheoGio,
				isnull(hh.DuocTichDiem,0) as DuocTichDiem,
				iif(hh.QuanLyBaoDuong is null,0, hh.QuanLyBaoDuong) as QuanLyBaoDuong,
				iif(hh.LoaiBaoDuong is null,0, hh.LoaiBaoDuong) as LoaiBaoDuong,
				iif(hh.SoKmBaoHanh is null,0, hh.SoKmBaoHanh) as SoKmBaoHanh,
				iif(hh.HoaHongTruocChietKhau is null,0, hh.HoaHongTruocChietKhau) as HoaHongTruocChietKhau,		
				iif(hh.LoaiHangHoa is null,iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,
				iif(hh.LaHangHoa='1', isnull(gv.GiaVon,0), dbo.GetGiaVonOfDichVu(@ID_ChiNhanh,dvqd.ID)) as GiaVon,			
				dvqd.GiaBan, 
				dvqd.Xoa,
				ISNULL(hhtonkho.TonKho,0) as TonKho ,
				hh.ID_Xe,
				xe.BienSo
				from DonViQuiDoi dvqd
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
				left join Gara_DanhMucXe xe on hh.ID_Xe= xe.ID
				where  dvqd.ladonvichuan = 1 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
				and hh.TheoDoi like @KinhDoanhFilter
				) a  where exists (select LoaiHang from @tblLoaiHang loai where a.LoaiHangHoa = loai.LoaiHang)

			Select * from #dmhanghoatable2 hhtb2
		END

    END

    if(@MaHH != '' or @MaHHCoDau != '')
    BEGIN
    	if(@List_ThuocTinh != '')
		BEGIN
			select *,
				case LoaiHangHoa 
					when 1 then N'Hàng hóa'
					when 2 then N'Dịch vụ'
					when 3 then N'Combo'
				end as sLoaiHangHoa
			 into #dmhanghoatable3
			from
			(
				select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang,hhtt.GiaTri + CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.LaChaCungLoai,
				hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, 
				dnhh.TenNhomHangHoa as NhomHangHoa , hh.TenHangHoa, dvqd.TenDonViTinh, dvqd.ThuocTinhGiaTri,
				isnull(hh.SoPhutThucHien,0) as SoPhutThucHien,
				isnull(hh.DichVuTheoGio,0) as DichVuTheoGio,			
				isnull(hh.DuocTichDiem,0) as DuocTichDiem,
				iif(hh.QuanLyBaoDuong is null,0, hh.QuanLyBaoDuong) as QuanLyBaoDuong,
				iif(hh.LoaiBaoDuong is null,0, hh.LoaiBaoDuong) as LoaiBaoDuong,
				iif(hh.SoKmBaoHanh is null,0, hh.SoKmBaoHanh) as SoKmBaoHanh,
				iif(hh.HoaHongTruocChietKhau is null,0, hh.HoaHongTruocChietKhau) as HoaHongTruocChietKhau,		
				iif(hh.LoaiHangHoa is null,iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,
				iif(hh.LaHangHoa='1', isnull(gv.GiaVon,0), dbo.GetGiaVonOfDichVu(@ID_ChiNhanh,dvqd.ID)) as GiaVon,				
				dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho,
				dvqd.Xoa,
				hh.ID_Xe,
				xe.BienSo
				FROM DonViQuiDoi dvqd
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh
				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
				LEFT JOIN HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    			LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    			LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
				left join Gara_DanhMucXe xe on hh.ID_Xe= xe.ID
    			where dvqd.ladonvichuan = 1 and
    			((select count(*) from @tablename b where 
    			hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
				or hh.TenHangHoa like '%'+b.Name+'%'
				or hh.GhiChu like '%' +b.Name +'%' 
				or xe.BienSo like '%' +b.Name +'%' 
    			or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    			and ((select count(*) from @tablenameChar c where
    			hh.TenHangHoa like '%'+c.Name+'%' or hh.GhiChu like '%'+c.Name+'%'
    			or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    			 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
    			and hh.TheoDoi like @KinhDoanhFilter
				) a where exists (select LoaiHang from @tblLoaiHang loai where a.LoaiHangHoa = loai.LoaiHang)

			Select * from #dmhanghoatable3 hhtb2	
    				where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
		END
		ELSE
		BEGIN
		select *,
			case LoaiHangHoa 
				when 1 then N'Hàng hóa'
				when 2 then N'Dịch vụ'
				when 3 then N'Combo'
			end as sLoaiHangHoa
			 into #dmhanghoatable4
			from
			(
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang, hh.LaChaCungLoai,
			hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, 
			dnhh.TenNhomHangHoa as NhomHangHoa , hh.TenHangHoa, dvqd.TenDonViTinh, dvqd.ThuocTinhGiaTri,
			isnull(hh.SoPhutThucHien,0) as SoPhutThucHien,
			isnull(hh.DichVuTheoGio,0) as DichVuTheoGio,
			isnull(hh.DuocTichDiem,0) as DuocTichDiem,
			iif(hh.QuanLyBaoDuong is null,0, hh.QuanLyBaoDuong) as QuanLyBaoDuong,
			iif(hh.LoaiBaoDuong is null,0, hh.LoaiBaoDuong) as LoaiBaoDuong,
			iif(hh.SoKmBaoHanh is null,0, hh.SoKmBaoHanh) as SoKmBaoHanh,
			iif(hh.HoaHongTruocChietKhau is null,0, hh.HoaHongTruocChietKhau) as HoaHongTruocChietKhau,		
			iif(hh.LoaiHangHoa is null,iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			iif(hh.LaHangHoa='1', isnull(gv.GiaVon,0), dbo.GetGiaVonOfDichVu(@ID_ChiNhanh,dvqd.ID)) as GiaVon,
			dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho ,
			dvqd.Xoa,
			hh.ID_Xe,
			xe.BienSo
			FROM DonViQuiDoi dvqd
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh 
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
			left join Gara_DanhMucXe xe on hh.ID_Xe= xe.ID
    		where dvqd.ladonvichuan = 1 and
    		((select count(*) from @tablename b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
			or hh.TenHangHoa like '%'+b.Name+'%'
			or hh.GhiChu like '%' +b.Name +'%' 
			or xe.BienSo like '%' +b.Name +'%' 
    		or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    		and ((select count(*) from @tablenameChar c where
    		hh.TenHangHoa like '%'+c.Name+'%' or hh.GhiChu like '%'+c.Name+'%'
    		or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    		 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
    		and hh.TheoDoi like @KinhDoanhFilter 
			) a where exists (select LoaiHang from @tblLoaiHang loai where a.LoaiHangHoa = loai.LoaiHang)

			Select * from #dmhanghoatable4 hhtb2	
		END
	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[PTN_CheckChangeCus]
    @ID_PhieuTiepNhan [uniqueidentifier],
    @ID_KhachHangNew [uniqueidentifier],
    @ID_BaoHiemNew [nvarchar](40)
AS
BEGIN
    SET NOCOUNT ON;
    
    	if isnull(@ID_BaoHiemNew,'')=''
    		set @ID_BaoHiemNew ='00000000-0000-0000-0000-000000000000'
    
    	declare @tblReturn table(Loai int)
    
    	---- get PTN old
    	declare @PTNOld_IDCus uniqueidentifier, @PTNOld_BaoHiem uniqueidentifier
    	select @PTNOld_IDCus = ID_KhachHang, @PTNOld_BaoHiem = ID_BaoHiem from Gara_PhieuTiepNhan where ID= @ID_PhieuTiepNhan
    
    	---- get list hoadon of PTN
    	select ID, ID_DoiTuong, ID_BaoHiem
    	into #tblHoaDon
    	from BH_HoaDon
    	where ID_PhieuTiepNhan = @ID_PhieuTiepNhan
    	and ChoThanhToan =0
    	and LoaiHoaDon in (3,25)
   
    
    	if @ID_KhachHangNew != @PTNOld_IDCus
    	begin
    		declare @count1 int;
    		select @count1 =count(*)
    		from #tblHoaDon
    		where ID_DoiTuong != @ID_KhachHangNew
    
    		if @count1 > 0
    			insert into @tblReturn values (1)

			---- check exist soquy khachhang
    		declare @countSQKH int
    		select @countSQKH = count(qhd.ID)
    		from #tblHoaDon hd
    		join Quy_HoaDon_ChiTiet qct on hd.ID = qct.ID_HoaDonLienQuan
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    		where (qhd.TrangThai is null or qhd.TrangThai= 1)
			and qct.ID_DoiTuong= @PTNOld_IDCus
    
    		if @countSQKH > 0
    			 insert into @tblReturn values (3)
    	end
    
    	if isnull(@PTNOld_BaoHiem,'00000000-0000-0000-0000-000000000000')='00000000-0000-0000-0000-000000000000'
    		set @PTNOld_BaoHiem ='00000000-0000-0000-0000-000000000000'
    
    
    	if @ID_BaoHiemNew != @PTNOld_BaoHiem
    	begin
    		declare @count2 int;
    		select @count2 =count(*)
    		from #tblHoaDon
    		where isnull(ID_BaoHiem,'00000000-0000-0000-0000-000000000000') != @ID_BaoHiemNew
    		
    		if @count2 > 0
    			insert into @tblReturn values (2)

			---- check exist soquy baohiem
    		declare @countSQBH int
    		select @countSQBH = count(qhd.ID)
    		from #tblHoaDon hd
    		join Quy_HoaDon_ChiTiet qct on hd.ID = qct.ID_HoaDonLienQuan
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    		where (qhd.TrangThai is null or qhd.TrangThai= 1)
			and qct.ID_DoiTuong= @PTNOld_BaoHiem
    
    		if @countSQBH > 0
    			 insert into @tblReturn values (4)
    	end      	
    
    	 select * from @tblReturn
END");

            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinh_ChiPhiSuaChua]
    @Year [int],
    @IdChiNhanh [uniqueidentifier],
    @LoaiBaoCao [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Insert statements for procedure here
    	IF(@LoaiBaoCao = 1)
    	BEGIN
    		SELECT Thang, SUM(ThanhTien) AS ChiPhi FROM
    		(SELECT MONTH(hd.NgayLapHoaDon) AS Thang, hdcp.ThanhTien FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiPhi hdcp ON hd.ID = hdcp.ID_HoaDon
    		WHERE YEAR(hd.NgayLapHoaDon) = @Year AND hd.ID_DonVi = @IdChiNhanh AND hd.ChoThanhToan = 0) AS a
    		GROUP BY Thang
    	END
    	ELSE IF(@LoaiBaoCao = 2)
    	BEGIN
    		SELECT Thang, SUM(ThanhTien) AS ChiPhi FROM
    		(SELECT YEAR(hd.NgayLapHoaDon) AS Thang, hdcp.ThanhTien FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiPhi hdcp ON hd.ID = hdcp.ID_HoaDon
    		WHERE YEAR(hd.NgayLapHoaDon) = @Year AND hd.ID_DonVi = @IdChiNhanh AND hd.ChoThanhToan = 0) AS a
    		GROUP BY Thang
    	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhMonth_SoQuyBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
    SELECT
    	a.ThangLapHoaDon,
    	CAST(ROUND(SUM(a.ThuNhapKhac), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac,
    	CAST(ROUND(SUM(a.PhiTraHangNhap), 0) as float) as PhiTraHangNhap,
    	CAST(ROUND(SUM(a.KhachThanhToan), 0) as float) as KhachThanhToan
    	FROM
    	(
    		Select 
    		MONTH(qhd.NgayLapHoaDon) as ThangLapHoaDon,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12)
			or (hd.LoaiHoaDon in (1,3,19) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when (hd.LoaiHoaDon in (1,3,25)) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @year
    		and (qhd.HachToanKinhDoanh = 1)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0) and qhdct.LoaiThanhToan != 1
    	) as a
    	GROUP BY
    	a.ThangLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhYear_SoQuyBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
--EXEC ReportTaiChinhYear_SoQuyBanHang '2021', 'a31fa9bc-dd97-47f8-9901-f19efc4fa831'
BEGIN
SET NOCOUNT ON;
    SELECT
    	a.NamLapHoaDon,
    	CAST(ROUND(SUM(a.ThuNhapKhac), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac,
    	CAST(ROUND(SUM(a.PhiTraHangNhap), 0) as float) as PhiTraHangNhap,
    	CAST(ROUND(SUM(a.KhachThanhToan), 0) as float) as KhachThanhToan
    	FROM
    	(
    		Select 
    		DATEPART(YEAR, qhd.NgayLapHoaDon) as NamLapHoaDon,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or (hd.LoaiHoaDon in (1,3,19) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when hd.LoaiHoaDon in (1,3,25) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    		and (qhd.HachToanKinhDoanh = 1)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0) and qhdct.LoaiThanhToan != 1
    	) as a
    	GROUP BY
    	a.NamLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[SaoChepThietLapLuong]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @KieuLuongs [nvarchar](50),
    @IDNhanViens [nvarchar](max),
    @UpdateNVSetup [bit],
    @ID_NhanVienLogin [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblKieuLuong table(LoaiLuong int)
    	insert into @tblKieuLuong
    	select Name from dbo.splitstring(@KieuLuongs)
    
    	---- get tlapluong of nhanvien
    	select *
    	into #tempCoBan
    	from NS_Luong_PhuCap pc
    	where pc.ID_NhanVien = @ID_NhanVien and pc.ID_DonVi= @ID_ChiNhanh and pc.TrangThai !=0
    	and exists(select LoaiLuong from @tblKieuLuong loai where pc.LoaiLuong= loai.LoaiLuong)
    
    	---- get tlapluong chi tiet of nhanvien
    	select ct.*
    	into #tempChiTiet
    	from NS_Luong_PhuCap pc
    	join NS_ThietLapLuongChiTiet ct on pc.ID= ct.ID_LuongPhuCap
    	where pc.ID_NhanVien = @ID_NhanVien and pc.ID_DonVi= @ID_ChiNhanh
    	and pc.TrangThai !=0
    	and exists(select LoaiLuong from @tblKieuLuong loai where pc.LoaiLuong= loai.LoaiLuong)
    
    	declare @tblNhanVien table(ID_NhanVien uniqueidentifier)
    	if @UpdateNVSetup = '0'	
    		---- giu nguyen tlapluong cu (chi insert nhung nvien not exist in tlapluong)
    		insert into @tblNhanVien
    		select Name from dbo.splitstring(@IDNhanViens) tbl
   -- 		where not exists (select ID_NhanVien from NS_Luong_PhuCap pc 
			--where tbl.Name= pc.ID_NhanVien and pc.ID_DonVi= @ID_ChiNhanh and pc.TrangThai !=0)

			---- chi insert cac thietlapluong chua dc cai dat

    		
    	else
    		---- capnhat lai thietlapluong
    		begin
    			insert into @tblNhanVien
    			select Name from dbo.splitstring(@IDNhanViens)	where Name !=@ID_NhanVien
    
    			---- xoa thietlapluong exist (chỉ xóa những loại sao chép)
    			delete from NS_ThietLapLuongChiTiet 
    			where ID_LuongPhuCap in
    				(select ID 
    				from NS_Luong_PhuCap pc 
    				where pc.ID_DonVi = @ID_ChiNhanh
    				and exists( select ID_NhanVien from @tblNhanVien nv where pc.ID_NhanVien =nv.ID_NhanVien)
    				and exists(select LoaiLuong from @tblKieuLuong loai where pc.LoaiLuong= loai.LoaiLuong)
    				)
    
    			delete from NS_Luong_PhuCap
    			where ID_DonVi = @ID_ChiNhanh
    			and ID_NhanVien in ( select ID_NhanVien from @tblNhanVien)
    			and LoaiLuong in (select LoaiLuong from @tblKieuLuong)
    		end
    
    	declare @IDNhanVien uniqueidentifier
    	DECLARE curNhanVien CURSOR SCROLL LOCAL FOR
    		select ID_NhanVien
    		from @tblNhanVien 
    	OPEN curNhanVien 
    	FETCH FIRST FROM curNhanVien
    	INTO @IDNhanVien
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    				---- insert tlapcoban (neu 1Nvien co nhieu thietlapluong coban)
    				declare @curIDPhuCap uniqueidentifier
    				DECLARE curPhuCapCB CURSOR SCROLL LOCAL FOR
    				select ID
    				from #tempCoBan 
    				OPEN curPhuCapCB 
    			FETCH FIRST FROM curPhuCapCB
    				INTO @curIDPhuCap
    				WHILE @@FETCH_STATUS = 0
    				begin					
    					declare @ID_PhuCap uniqueidentifier = NEWID()
    					--select  @ID_PhuCap
    					insert into NS_Luong_PhuCap
    					select @ID_PhuCap, @IDNhanVien, ID_LoaiLuong, NgayApDung, NgayKetThuc, SoTien, HeSo, Bac, NoiDung, TrangThai, LoaiLuong, ID_DonVi
    					from #tempCoBan where LoaiLuong not in (51,52,53,61,62,63) and ID= @curIDPhuCap
    
    					---- insert tlapnangcao of luong
    					insert into NS_ThietLapLuongChiTiet (ID, ID_LuongPhuCap, LuongNgayThuong, NgayThuong_LaPhanTramLuong, Thu7_GiaTri, Thu7_LaPhanTramLuong,
    						ThCN_GiaTri, CN_LaPhanTramLuong, NgayNghi_GiaTri, NgayNghi_LaPhanTramLuong, NgayLe_GiaTri, NgayLe_LaPhanTramLuong,
    						LaOT, ID_CaLamViec)
    					select NEWID(), @ID_PhuCap,LuongNgayThuong,NgayThuong_LaPhanTramLuong,Thu7_GiaTri,Thu7_LaPhanTramLuong,
    						ThCN_GiaTri, CN_LaPhanTramLuong, NgayNghi_GiaTri, NgayNghi_LaPhanTramLuong, NgayLe_GiaTri, NgayLe_LaPhanTramLuong,
    						LaOT, ID_CaLamViec
    					from #tempChiTiet where ID_LuongPhuCap= @curIDPhuCap
    
    					FETCH NEXT FROM curPhuCapCB 
    					INTO @curIDPhuCap
    				end
    				CLOSE curPhuCapCB  
    			DEALLOCATE curPhuCapCB 
    
    				-- insert phucap + giamtru
    				insert into NS_Luong_PhuCap
    				select NewID(), @IDNhanVien, ID_LoaiLuong, NgayApDung, NgayKetThuc, SoTien, HeSo, Bac, NoiDung, TrangThai, LoaiLuong, ID_DonVi
    				from #tempCoBan where LoaiLuong in (51,52,53,61,62,63)			
    
    				FETCH NEXT FROM curNhanVien 
    				INTO @IDNhanVien
    			END;
    			CLOSE curNhanVien  
    		DEALLOCATE curNhanVien 
    
    		declare @loailuong nvarchar(200) =''
    		if (select count(*) from @tblKieuLuong where LoaiLuong in(1,2,3,4)) > 0
    			set @loailuong =N'lương,'
    		if (select count(*) from @tblKieuLuong where LoaiLuong like '%5%') > 0
    			set @loailuong = @loailuong + N' phụ cấp,'
    		if (select count(*) from @tblKieuLuong where LoaiLuong like '%6%') > 0
    			set @loailuong = @loailuong + N' giảm trừ'
    
    		declare @tenNhanVien nvarchar(200), @maNhanVien nvarchar(100)
    		select @tenNhanVien = TenNhanVien, @maNhanVien = MaNhanVien from NS_NhanVien where ID= @ID_NhanVien
    
    		declare @nhanvienSetup nvarchar(max) = (
    		select  concat( TenNhanVien ,' (',MaNhanVien, ')') + ', ' as [text()] 
    		from NS_NhanVien nv1 
    		where exists (select ID from @tblNhanVien nv2 where nv1.ID= nv2.ID_NhanVien)
    		for xml path(''))
    		
    		insert into HT_NhatKySuDung (ID, ID_DonVi, ID_NhanVien, LoaiNhatKy, ChucNang, ThoiGian, NoiDung, NoiDungChiTiet)
    		values (NEWID(), @ID_ChiNhanh, @ID_NhanVienLogin, 1, N'Thiết lập lương - Sao chép', GETDATE(),
    		concat(N'Sao chép thiết lập lương ', N'(', @loailuong , N') từ nhân viên <b>', @tenNhanVien , ' (',  @maNhanVien ,' </b>)'),
    		concat(N'Sao chép thiết lập lương ', N'(', @loailuong , N') từ nhân viên <b>', @tenNhanVien , ' (',  @maNhanVien , N'</b>) đến: ', @nhanvienSetup) )
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetAllHoaDon_byIDPhieuThuChi]
    @ID_PhieuThuChi [varchar](50)
AS
BEGIN
	SET NOCOUNT ON;
	-- get list HD with MaPhieuThuChi
    select
		case when qhd.PhieuDieuChinhCongNo='1' then N'Điều chỉnh công nợ' else
			case when sum(qct.TienThu)= 0 and max(qct.DiemThanhToan) > 0 then N'Điều chỉnh điểm' else
					ISNULL(hd.MaHoaDon,N'Thu thêm') end end as MaHoaDon,
			ISNULL(hd.NgayLapHoaDon, max(qhd.NgayLapHoaDon)) as NgayLapHoaDon, 
			case when sum(qct.TienThu)= 0 then max(qct.DiemThanhToan) 
			else case when hd.ID_BaoHiem = max(qct.ID_DoiTuong) then ISNULL(hd.PhaiThanhToanBaoHiem,0)
				else ISNULL(hd.PhaiThanhToan,0) end  end as TongTienThu,
    		sum(qct.TienThu) as TienThu, 
			0 as DaChi,
    		N'Đã thanh toán' as GhiChu,
			max(qhd.NgayLapHoaDon) as NgayLapPhieu,
			max(isnull(qct.ID_DoiTuong,'00000000-0000-0000-0000-000000000000')) as ID_DoiTuong,
			max(isnull(qct.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000')) as ID_KhoanThuChi,
    		MAX(ISNULL(QuyXML.PhuongThucTT,N'Tiền mặt')) as PhuongThuc
		INTO #temp
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qhd.ID = qct.ID_HoaDon
    	left join BH_HoaDon hd  on qct.ID_HoaDonLienQuan = hd.ID -- truong hop thu them (left join)
    	LEFT JOIN 
    	(	
    				Select Main.ID_HoaDonLienQuan,
    				   Left(Main.PThuc_SoQuy,Len(Main.PThuc_SoQuy)-1) As PhuongThucTT
    				From
    				(
    					Select distinct main1.ID_HoaDonLienQuan, 
    						(
    							Select distinct (tbl1.PhuongThuc) + ',' AS [text()]
    							From 
    								(
    								SELECT qct2.ID_HoaDonLienQuan,
										case qct2.HinhThucThanhToan
											when 1 then concat(N'Tiền mặt (', FORMAT(qct2.TienThu, '#,#'),')')
											when 2 then concat(N'POS (',FORMAT(qct2.TienThu, '#,#'),')')
											when 3 then concat(N'Chuyển khoản (',FORMAT(qct2.TienThu, '#,#'),')')
											when 4 then concat(N'Thẻ giá trị (', FORMAT(qct2.TienThu, '#,#') ,')')
											when 5 then concat(N'Đổi điểm (', FORMAT(qct2.TienThu, '#,#'),')')
											when 6 then concat(N'Thu từ cọc (', FORMAT(qct2.TienThu, '#,#'),')')
											end as PhuongThuc											
    								FROM Quy_HoaDon_ChiTiet qct2
    								left join DM_TaiKhoanNganHang tk on qct2.ID_TaiKhoanNganHang = tk.ID
    								where qct2.ID_HoaDon like @ID_PhieuThuChi
    								) tbl1
    							Where tbl1.ID_HoaDonLienQuan = main1.ID_HoaDonLienQuan or (tbl1.ID_HoaDonLienQuan is null and  main1.ID_HoaDonLienQuan is null )
    							For XML PATH ('')
    						) PThuc_SoQuy
    					From Quy_HoaDon_ChiTiet main1 group by main1.ID_HoaDonLienQuan
    				) [Main] 
    		
    	) QuyXML on qct.ID_HoaDonLienQuan = QuyXML.ID_HoaDonLienQuan or (qct.ID_HoaDonLienQuan is null and  QuyXML.ID_HoaDonLienQuan is null )		
		where qct.ID_HoaDon = @ID_PhieuThuChi
    	group by qct.ID_HoaDonLienQuan, hd.MaHoaDon,hd.ID_BaoHiem,hd.PhaiThanhToanBaoHiem, hd.NgayLapHoaDon, hd.PhaiThanhToan, qhd.PhieuDieuChinhCongNo

		-- get infor PhieuThu truoc do
		select hd.MaHoaDon, hd.NgayLapHoaDon, 
			0 as TongTienThu,
    		0 as TienThu,
			sum(qct.TienThu) as DaChi,
    		N'Đã thanh toán' as GhiChu,
			max(qhd.NgayLapHoaDon) as NgayLapPhieu,
			'00000000-0000-0000-0000-000000000000' as ID_DoiTuong,
			'00000000-0000-0000-0000-000000000000' as ID_KhoanThuChi,
    		'' as PhuongThuc
		into #temp2
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qhd.ID = qct.ID_HoaDon
    	join BH_HoaDon hd  on qct.ID_HoaDonLienQuan = hd.ID
		where qct.ID_HoaDonLienQuan in 
				( select qct3.ID_HoaDonLienQuan 
				from Quy_HoaDon qhd3
				join Quy_HoaDon_ChiTiet qct3 on qhd3.ID = qct3.ID_HoaDon
				where qhd3.ID like @ID_PhieuThuChi)
		and convert(varchar, qhd.NgayLapHoaDon, 120) < (select top 1 convert(varchar, NgayLapPhieu, 120) from #temp)
		and qhd.TrangThai='1'
		group by qct.ID_HoaDonLienQuan, hd.MaHoaDon, hd.NgayLapHoaDon, hd.PhaiThanhToan

		-- group and sum
		select  tblView.MaHoaDon, tblView.NgayLapHoaDon, 
				sum(tblView.TongTienThu) as TongTienThu,
				sum(tblView.TienThu) as TienThu,
				sum(tblView.DaChi) as DaChi,
				max(GhiChu) as GhiChu,
				max(PhuongThuc) as PhuongThuc,
				max(ID_DoiTuong) as ID_DoiTuong,
				max(ID_KhoanThuChi) as ID_KhoanThuChi
		from (select * from #temp
			union
			select * from #temp2) tblView
		group by tblView.MaHoaDon, tblView.NgayLapHoaDon
		order by tblView.NgayLapHoaDon desc
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
			else
    			ISNULL(hd.PhaiThanhToan,0)
    		end as GiaTri
    	from BH_HoaDon hd
		join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
    	where hd.ID_DoiTuong like @ID_DoiTuong 
    	and hd.LoaiHoaDon not in (3,23) -- dieu chinh the: khong lien quan cong no
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
		
		SELECT *, 0 as LoaiThanhToan FROM @tblHoaDon
    	union
    	-- get list Quy_HD (không lấy Quy_HoaDon thu từ datcoc)
		select * from
		(
    		select qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon ,
			case when dt.LoaiDoiTuong = 1 OR dt.LoaiDoiTuong = 3 then
				case when qhd.LoaiHoaDon= 11 then -sum(qct.TienThu) else sum(qct.TienThu) end
			else 
    			case when qhd.LoaiHoaDon = 11 then sum(qct.TienThu) else -sum(qct.TienThu) end
    		end as GiaTri,
			iif(qhd.PhieuDieuChinhCongNo='1',2, max(qct.LoaiThanhToan)) as LoaiThanhToan --- 1.coc, 2.dieuchinhcongno, 3.khong butru congno
			
    		from Quy_HoaDon qhd	
    		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    		join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
			join @tblChiNhanh cn on qhd.ID_DonVi= cn.ID
			left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID		
    		where qct.ID_DoiTuong like @ID_DoiTuong
			and qct.HinhThucThanhToan !=6			
			and (qct.LoaiThanhToan is null or qct.LoaiThanhToan !=3) ---- khong get phieuthu/chi khong lienquan congno
			and (qhd.TrangThai is null or qhd.TrangThai='1') -- van phai lay phieu thu tu the --> trừ cong no KH
			group by qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon,dt.LoaiDoiTuong,qhd.PhieuDieuChinhCongNo
		) a where a.GiaTri != 0 -- khong lay phieudieuchinh diem

		select * from
		(
    		select qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon ,
			case when dt.LoaiDoiTuong = 1 OR dt.LoaiDoiTuong = 3 then
				case when qhd.LoaiHoaDon= 11 then -sum(qct.TienThu) else sum(qct.TienThu) end
			else 
    			case when qhd.LoaiHoaDon = 11 then sum(qct.TienThu) else -sum(qct.TienThu) end
    		end as GiaTri,
			iif(qhd.PhieuDieuChinhCongNo='1',2, max(qct.LoaiThanhToan)) as LoaiThanhToan --- 1.coc, 2.dieuchinhcongno, 3.khong butru congno
			
    		from Quy_HoaDon qhd	
    		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    		 join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
			--join @tblChiNhanh cn on qhd.ID_DonVi= cn.ID
			left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID		
    		where qct.ID_DoiTuong like @ID_DoiTuong
			--and qct.HinhThucThanhToan !=6			
			--and (qct.LoaiThanhToan is null or qct.LoaiThanhToan !=3) ---- khong get phieuthu/chi khong lienquan congno
			--and (qhd.TrangThai is null or qhd.TrangThai='1') -- van phai lay phieu thu tu the --> trừ cong no KH
			group by qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon,dt.LoaiDoiTuong,qhd.PhieuDieuChinhCongNo
			) a where a.GiaTri != 0
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetInforKhachHang_ByID]
    @ID_DoiTuong uniqueidentifier,
    @ID_ChiNhanh [nvarchar](max),
    @timeStart [nvarchar](max),
    @timeEnd [nvarchar](max)
AS
BEGIN
	SET NOCOUNT ON;
	declare @LoaiDoiTuong int
	select @LoaiDoiTuong= LoaiDoiTuong from DM_DoiTuong where ID = @ID_DoiTuong

    SELECT 
    			  dt.ID as ID,
    			  dt.MaDoiTuong, 
    			  case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as ID_NhomDoiTuong,
    			  dt.TenDoiTuong,
    			  dt.TenDoiTuong_KhongDau,
    			  dt.TenDoiTuong_ChuCaiDau,
    			  dt.ID_TrangThai,
    			  dt.GioiTinhNam,
    			  dt.NgaySinh_NgayTLap,
    			  ISNULL(dt.DienThoai,'') as DienThoai,
    			  ISNULL(dt.Email,'') as Email,
    			  ISNULL(dt.DiaChi,'') as DiaChi,
    			  ISNULL(dt.MaSoThue,'') as MaSoThue,
    			  ISNULL(dt.GhiChu,'') as GhiChu,
				  dt.TaiKhoanNganHang,
    			  dt.NgayTao,
    			  dt.DinhDang_NgaySinh,
    			  ISNULL(dt.NguoiTao,'') as NguoiTao,
    			  dt.ID_NguonKhach,
    			  dt.ID_NhanVienPhuTrach,
    			  dt.ID_NguoiGioiThieu,
    			  dt.ID_DonVi,
    			  dt.LaCaNhan,
    			  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
				  dt.TenNhomDoiTuongs as TenNhomDT,    			 
    			  dt.ID_TinhThanh,
    			  dt.ID_QuanHuyen,
				  dt.TheoDoi,
    			  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    			  CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    			  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    			  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    			  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    			  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
    			  CAST(0 as float) as TongNapThe , 
    			  CAST(0 as float) as SuDungThe , 
    			  CAST(0 as float) as HoanTraTheGiaTri , 
    			  CAST(0 as float) as SoDuTheGiaTri , 
				  ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
    			  concat(dt.MaDoiTuong,' ',lower(dt.MaDoiTuong) ,' ', dt.TenDoiTuong,' ', dt.DienThoai,' ', dt.TenDoiTuong_KhongDau)  as Name_Phone			
    		  FROM DM_DoiTuong dt
			  left join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
			  LEFT JOIN (
    					SELECT tblThuChi.ID_DoiTuong,
    						
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) - SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
					sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    					FROM
    					(
						-- doanhthu
							SELECT 
    							iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) as ID_DoiTuong,
    							0 AS GiaTriTra,    							
								iif(@LoaiDoiTuong=1,bhd.PhaiThanhToan,isnull(bhd.PhaiThanhToanBaoHiem,0)) as DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
									0 as ThuTuThe
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = 0 
							and iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) = @ID_DoiTuong

    						
    						 union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,    							
								iif(@LoaiDoiTuong=1,bhd.PhaiThanhToan,0)  AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) 
							AND bhd.ChoThanhToan = 0 
							and iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) = @ID_DoiTuong						
    							
    						union all
    
    							-- tienthu
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon    						
    						WHERE qhd.LoaiHoaDon = 11 
							and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
							and qhdct.ID_DoiTuong = @ID_DoiTuong
							and qhdct.HinhThucThanhToan!=6

							
    							
    						 union all    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    								0 AS SoLanMuaHang,
									0 as ThuTuThe
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 12 
							AND (qhd.TrangThai != 0 OR qhd.TrangThai is null)
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
							and qhdct.HinhThucThanhToan!=6
							and qhdct.ID_DoiTuong = @ID_DoiTuong

							--union all
							--Select 
    			--				hd.ID_DoiTuong,
    			--				0 AS GiaTriTra,
    			--				0 AS DoanhThu,
    			--				0 AS TienThu,
    			--				0 as TienChi,
    			--				COUNT(*) AS SoLanMuaHang,
							--	0 as ThuTuThe
    			--			From BH_HoaDon hd 
    			--			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    			--			and hd.ChoThanhToan = 0 and hd.ID_DoiTuong = @ID_DoiTuong				
    			--			GROUP BY hd.ID_DoiTuong  	
    				)AS tblThuChi GROUP BY tblThuChi.ID_DoiTuong   					
    		) a on dt.ID = a.ID_DoiTuong

			where dt.ID= @ID_DoiTuong
END");

            Sql(@"ALTER PROCEDURE [dbo].[TinhLuongOT]
	@NgayCongChuan int,
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN
	
	SET NOCOUNT ON;	

		--declare @IDChiNhanhs varchar(max)='d93b17ea-89b9-4ecf-b242-d03b8cde71de'
		--declare @FromDate datetime='2020-08-01'
		--declare @ToDate datetime='2020-08-31'

		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs,'', @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs,'', @FromDate, @ToDate

		---- ===== OT theo ngay ======================
		declare @thietlapOTNgay table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @thietlapOTNgay
		select *	
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong = 2

		select  a.*,
				case when LaPhanTram= 0 then GiaTri else (SoTien/@NgayCongChuan/8) * GiaTri /100 end as Luong1GioCong			
			into #temp1					
			from
					(select TenNhanVien, bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
						bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
						pc.SoTien,
					
						pc.LoaiLuong,
						pc.HeSo,

						case when ngayle.ID is null then -- 0.chunhat, 6.thu7, 1.ngaynghi, 2.ngayle, 3.ngaythuong  			
							case bs.Thu
								when 6 then 6
								when 0 then 0
							else 3 end
						else bs.LoaiNgay end LoaiNgayThuong_Nghi,	

						case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_GiaTri
								when 0 then tlct.ThCN_GiaTri
							else tlct.LuongNgayThuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_GiaTri
								when 2 then  tlct.NgayLe_GiaTri
							else tlct.LuongNgayThuong end
						end as GiaTri,

						case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_LaPhanTramLuong
								when 0 then tlct.CN_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_LaPhanTramLuong
								when 2 then  tlct.NgayLe_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						end as LaPhanTram														
					from @tblCongThuCong bs
					join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
					join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap 
					join NS_NhanVien nv on pc.ID_NhanVien= nv.ID		
					left join NS_NgayNghiLe ngayle on bs.NgayCham = ngayle.Ngay and ngayle.TrangThai='1'
					where tlct.LaOT= 1 and pc.LoaiLuong = 2
					and exists (select tl.ID_PhuCap from @tblThietLapLuong tl where pc.ID = tl.ID_PhuCap)
				)a

		declare @tblCongOTNgay table (ID_PhuCap uniqueidentifier,  ID_NhanVien uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
		LuongOT float, NgayCham datetime)			
		
		declare @otngayID_NhanVien uniqueidentifier, @otngayID_PhuCap uniqueidentifier, @otngayTenLoaiLuong nvarchar(max),
		@otngayLoaiLuong int,@otngayLuongCoBan float, @otngayHeSo int, @otngayNgayApDung datetime, @otngayNgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTNgay
		OPEN curLuong -- cur 1
    	FETCH FIRST FROM curLuong
    	INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongOTNgay
				select @otngayID_PhuCap,@otngayID_NhanVien, @otngayLoaiLuong,@otngayLuongCoBan,@otngayHeSo,@otngayNgayApDung, @otngayNgayKetThuc,
					tmp.ID_CaLamViec, tmp.TenCa, tmp.TongGioCong1Ca,
					tmp.SoGioOT * Luong1GioCong as LuongOT,
					tmp.NgayCham
				from #temp1 tmp
				where tmp.ID_NhanVien = @otngayID_NhanVien and tmp.NgayCham >= @otngayNgayApDung and (@otngayNgayKetThuc is null OR tmp.NgayCham <= @otngayNgayKetThuc )  								
				FETCH NEXT FROM curLuong 
				INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	


			--- ======= OT Theo Ca =================
		declare @thietlapOTCa table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @thietlapOTCa
		select *	
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong = 3
		
		-- get cong OT ca
		select  a.*,				
				case when LaPhanTram = 0 then GiaTri else case when LuongTheoCa is null then SoTien/TongGioCong1Ca * GiaTri/100
					else LuongTheoCa/TongGioCong1Ca* GiaTri/100 end end as Luong1GioCong
			into #temp2					
			from
				(select bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
					pc.SoTien,
					theoca.LuongTheoCa,
					pc.LoaiLuong,
					pc.HeSo,
					case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_GiaTri
								when 0 then tlct.ThCN_GiaTri
							else tlct.LuongNgayThuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_GiaTri
								when 2 then  tlct.NgayLe_GiaTri
							else tlct.LuongNgayThuong end
						end as GiaTri,

						case when ngayle.ID is null then   			
							case bs.Thu
								when 6 then tlct.Thu7_LaPhanTramLuong
								when 0 then tlct.CN_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						else 
							case bs.LoaiNgay 
								when 1 then tlct.NgayNghi_LaPhanTramLuong
								when 2 then  tlct.NgayLe_LaPhanTramLuong
							else tlct.NgayThuong_LaPhanTramLuong end
						end as LaPhanTram	
				from @tblCongThuCong bs
				join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
				join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap -- luongot
				left join NS_NgayNghiLe ngayle on bs.NgayCham = ngayle.Ngay and ngayle.TrangThai='1'
				left join (select tlca.LuongNgayThuong as LuongTheoCa, tlca.ID_CaLamViec, pca.ID_NhanVien -- get luongcoban
						from NS_Luong_PhuCap pca
						join NS_ThietLapLuongChiTiet tlca on pca.ID= tlca.ID_LuongPhuCap 
						where tlca.LaOT= 0
						) theoca on pc.ID_NhanVien= theoca.ID_NhanVien and bs.ID_CaLamViec= theoca.ID_CaLamViec
				where tlct.LaOT= 1
				and pc.LoaiLuong = 3		
				and exists (select tl.ID_PhuCap from @tblThietLapLuong tl where pc.ID = tl.ID_PhuCap)
				) a			

		declare @tblCongOTCa table (ID_PhuCap uniqueidentifier,  ID_NhanVien uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
		LuongOT float, NgayCham datetime)				
	
		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int,@LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTCa
		OPEN curLuong
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongOTCa
				select @ID_PhuCap,@ID_NhanVien, @LoaiLuong,@LuongCoBan,@HeSo,@NgayApDung, @NgayKetThuc,
					tmp.ID_CaLamViec, tmp.TenCa, tmp.TongGioCong1Ca,
					tmp.SoGioOT * Luong1GioCong as LuongOT,
					tmp.NgayCham
				from #temp2 tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  								
				FETCH NEXT FROM curLuong 
				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	

			--select thu, TenCa, SoGioOT, Luong1GioCong, LuongTheoCa, SoTien, NgayCham from #temp2 where ID_NhanVien='D559BADC-83AE-407C-8A79-BC160DF5C73A' order by NgayCham

		select ID_NhanVien, SUM(LuongOT) as LuongOT
		from
			(select ID_NhanVien, LuongOT from @tblCongOTNgay
			union all
			select ID_NhanVien, LuongOT from @tblCongOTCa
			) luongot group by luongot.ID_NhanVien
			
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
    		DECLARE @cthd_NeedUpGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @cthd_NeedUpGiaVon
    		select * from 
    		(
    	select distinct hdupdate.ID as IDHoaDon, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID as ID_ChiTietHoaDon, 
    		CASE WHEN hdupdate.YeuCau = '4' AND @IDChiNhanh = hdupdate.ID_CheckIn THEN hdupdate.NgaySua ELSE hdupdate.NgayLapHoaDon END AS NgayLapHoaDon, 				    			    				    							    			
    			hdctupdate.SoThuTu, hdctupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, hdctupdate.TienChietKhau, hdctupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		[dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanh, hhupdate.ID, hdctupdate.ID_LoHang, 
    				CASE WHEN hdupdate.YeuCau = '4' AND @IDChiNhanh = hdupdate.ID_CheckIn THEN hdupdate.NgaySua ELSE hdupdate.NgayLapHoaDon  		    		    			    					
    		END) as TonKho, 	    	
    			hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi as GiaVon, 
    			hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi as GiaVonNhan,
    		hhupdate.ID as ID_HangHoa, hhupdate.LaHangHoa, dvqdupdate.ID as IDDonViQuiDoi, hdctupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, 
    			@IDChiNhanh as IDChiNhanh, hdupdate.ID_CheckIn, hdupdate.YeuCau 
    		FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate ON hdupdate.ID = hdctupdate.ID_HoaDon    	
    	INNER JOIN DonViQuiDoi dvqdupdate ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID   	
    	INNER JOIN DM_HangHoa hhupdate on hhupdate.ID = dvqdupdate.ID_HangHoa   	
    	INNER JOIN @tblCTHD cthdthemmoiupdate  ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID   
    	WHERE hdupdate.ChoThanhToan = 0 
    			AND hdupdate.LoaiHoaDon NOT IN (3, 19, 25)
    			AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) 
    			AND
    		((hdupdate.ID_DonVi = @IDChiNhanh and hdupdate.NgayLapHoaDon >= @NgayLapHDMin
    				and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = @IDChiNhanh and hdupdate.NgaySua >= @NgayLapHDMin))
    		) as table1
    	order by NgayLapHoaDon, SoThuTu desc, LoaiHoaDon, MaHoaDon;
    
    		--Begin TinhGiaVonTrungBinh
    		DECLARE @TinhGiaVonTrungBinh BIT;
    		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanh;
    		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
    		BEGIN
    			-- get GiaVon DauKy by ID_QuiDoi
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select
    				hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu, hdct.SoLuong, hdct.DonGia, hd.TongTienHang, 
    				hdct.TienChietKhau, hdct.ThanhTien, hd.TongGiamGia, 
    				dvqd.TyLeChuyenDoi, 0, hdct.GiaVon / dvqd.TyLeChuyenDoi, -- giavon
    			hdct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi, --giavonnhan
    			hh.ID, hh.LaHangHoa, hdct.ID_DonViQuiDoi, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, 
    				@IDChiNhanh, hd.ID_CheckIn, hd.YeuCau FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct 	ON hd.ID = hdct.ID_HoaDon    	
    		INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID    		
    		INNER JOIN DM_HangHoa hh on hh.ID = dvqd.ID_HangHoa    		
    		INNER JOIN @tblCTHD cthdthemmoi ON cthdthemmoi.ID_HangHoa = hh.ID    		
    		WHERE hd.ChoThanhToan = 0 AND hd.LoaiHoaDon NOT IN (3, 19, 25)
    				AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) 
    				AND
    				((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon < @NgayLapHDMin and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    					or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua < @NgayLapHDMin))
    		order by NgayLapHoaDon desc, SoThuTu desc, hd.LoaiHoaDon, hd.MaHoaDon;
    			
    			--select * from @ChiTietHoaDonGiaVon order by NgayLapHoaDon
    			--select * from @cthd_NeedUpGiaVon order by NgayLapHoaDon
    
		DECLARE @ChiTietHoaDonGiaVon1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
			
			INSERT INTO @ChiTietHoaDonGiaVon1
			SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    					FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon1 WHERE cthdGiaVon1.RN = 1;

    			-- assign again GiaVon to cthd was edit by ID_HangHoa
    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @BangUpdateGiaVon
    		SELECT *, 
    				ROW_NUMBER() OVER (PARTITION BY tableUpdateGiaVon.ID_HangHoa, tableUpdateGiaVon.ID_LoHang ORDER BY tableUpdateGiaVon.NgayLapHoaDon) AS RN 
    			FROM
    			(SELECT * 
    					FROM @cthd_NeedUpGiaVon
    			UNION ALL
    			SELECT 
    					cthdGiaVon.IDHoaDon, cthdGiaVon.IDHoaDonBan, cthdGiaVon.MaHoaDon, cthdGiaVon.LoaiHoaDon, cthdGiaVon.ID_ChiTietHoaDon, cthdGiaVon.NgayLapHoaDon,
    					cthdGiaVon.SoThuThu, cthdGiaVon.SoLuong, cthdGiaVon.DonGia, cthdGiaVon.TongTienHang,
    				cthdGiaVon.ChietKhau, cthdGiaVon.ThanhTien, cthdGiaVon.TongGiamGia, cthdGiaVon.TyLeChuyenDoi, cthdGiaVon.TonKho, 
    					CASE WHEN cthdGiaVon.GiaVon IS NULL THEN 0 ELSE cthdGiaVon.GiaVon END AS GiaVon, 											
    					CASE WHEN cthdGiaVon.GiaVonNhan IS NULL THEN 0 ELSE cthdGiaVon.GiaVonNhan END AS GiaVonNhan,								
    					cthd1.ID_HangHoa, cthdGiaVon.LaHangHoa, cthdGiaVon.IDDonViQuiDoi, cthd1.ID_LoHang , cthdGiaVon.ID_ChiTietDinhLuong,
    				@IDChiNhanh, cthdGiaVon.ID_CheckIn, cthdGiaVon.YeuCau 
    				FROM @tblCTHD cthd1
    				LEFT JOIN 
    					@ChiTietHoaDonGiaVon1 AS cthdGiaVon
    				ON cthd1.ID_HangHoa = cthdGiaVon.ID_HangHoa 
    				AND (cthd1.ID_LoHang = cthdGiaVon.ID_LoHang OR cthdGiaVon.ID_LoHang IS NULL)) AS tableUpdateGiaVon;
    		--select * from @BangUpdateGiaVon order by NgayLapHoaDon
    
    			-- caculator again GiaVon by ID_HangHoa
    			DECLARE @TableTrungGianUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER,IDHangHoa UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, GiaVonNhapHang FLOAT, GiaVonNhanHang FLOAT)
    			INSERT INTO @TableTrungGianUpDate
    			SELECT 
    				IDHoaDon, ID_HangHoa, ID_LoHang, 
    				CASE WHEN MAX(TongTienHang) != 0 THEN SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) * (1-(MAX(TongGiamGia) / MAX(TongTienHang))) 
    					ELSE SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) END as GiaVonNhapHang,
    				CASE WHEN LoaiHoaDon = 10 THEN SUM(ChietKhau * DonGia)/ SUM(IIF(ChietKhau = 0, 1, ChietKhau) * TyLeChuyenDoi) 
    					ELSE 0 END as GiaVonNhanHang
    			FROM @BangUpdateGiaVon
    			WHERE IDHoaDon = @IDHoaDonInput
    				AND ID_HangHoa in (SELECT ID_HangHoa FROM @BangUpdateGiaVon WHERE IDHoaDon = @IDHoaDonInput AND RN = 1)
    			GROUP BY ID_HangHoa, ID_LoHang, IDHoaDon,LoaiHoaDon
    			
    			--select * from @TableTrungGianUpDate 
    
    			DECLARE @RNCheck INT;
    			SELECT @RNCheck = MAX(RN) FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang
    			IF(@RNCheck = 1)
    			BEGIN
    				UPDATE @BangUpdateGiaVon SET RN = 2
    			END
    
    			-- update GiaVon, GiaVonNhan to @BangUpdateGiaVon if Loai =(4,10), else keep old
    		UPDATE bhctup 
    			SET bhctup.GiaVon = 
    			CASE WHEN bhctup.LoaiHoaDon = 4 THEN giavontbup.GiaVonNhapHang	    						
    			ELSE bhctup.GiaVon END,    		
    			bhctup.GiaVonNhan = 
    				CASE WHEN bhctup.LoaiHoaDon = 10 AND bhctup.YeuCau = '4' AND bhctup.ID_CheckIn = ID_ChiNhanhThemMoi THEN giavontbup.GiaVonNhanHang   		    			
    			ELSE bhctup.GiaVonNhan END  		
    			FROM @BangUpdateGiaVon bhctup
    			JOIN @TableTrungGianUpDate giavontbup on bhctup.IDHoaDon =giavontbup.IDHoaDon and bhctup.ID_HangHoa = giavontbup.IDHangHoa and (bhctup.ID_LoHang = giavontbup.IDLoHang or giavontbup.IDLoHang is null)
    		WHERE bhctup.IDHoaDon = @IDHoaDonInput AND bhctup.RN = 1;
    			--END tính giá vốn trung bình cho lần nhập hàng và chuyển hàng đầu tiền
    
    		DECLARE @GiaVonCapNhat TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, IDHoaDonCu UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, IDChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, GiaVonCu FLOAT, IDHangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, IDChiTietDinhLuong UNIQUEIDENTIFIER,
    		IDChiNhanhThemMoi UNIQUEIDENTIFIER, IDCheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @GiaVonCapNhat
    		SELECT 
    				tableUpdate.IDHoaDon, tableUpdate.IDHoaDonBan, tableGiaVon.IDHoaDon, tableUpdate.MaHoaDon, tableUpdate.LoaiHoaDon, tableUpdate.ID_ChiTietHoaDon,tableUpdate.NgayLapHoaDon, tableUpdate.SoLuong, tableUpdate.DonGia,
    			tableUpdate.TongTienHang, tableUpdate.ChietKhau, tableUpdate.ThanhTien, tableUpdate.TongGiamGia, tableUpdate.TyLeChuyenDoi, tableUpdate.TonKho, tableUpdate.GiaVon, tableUpdate.GiaVonNhan, tableGiaVon.GiaVon, tableUpdate.ID_HangHoa, tableUpdate.LaHangHoa,
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
    		DECLARE @TongTienHang FLOAT;
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
    			SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
    			GiaVonCu, IDHangHoa, IDDonViQuiDoi, IDLoHang, IDChiNhanhThemMoi, IDCheckIn, YeuCau, RN, GiaVon, GiaVonNhan 
    			FROM @GiaVonCapNhat WHERE RN > 1 and LaHangHoa = 1 ORDER BY IDHangHoa, RN
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon 
    			INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau,@ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
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
    				IF(@LoaiHoaDon = 4)
    				BEGIN
    					--SELECT @TongTienHangDemo = SUM(bhctdm.ThanhTien - bhctdm.SoLuong * bhctdm.ChietKhau), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia -  bhctdm.ChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    
    						--select @IDHoaDon, @MaHoaDon, @TongTienHangDemo, @SoLuongDemo, @TonKho
    					IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    					BEGIN
    						IF(@TongTienHang != 0)
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + (@TongTienHangDemo* (1-(@TongGiamGia/@TongTienHang))))/(@SoLuongDemo + @TonKho);
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo)/(@SoLuongDemo + @TonKho);
    						END
    					END
    					ELSE
    					BEGIN
    					
    						IF(@TongTienHang != 0)
    						BEGIN
    							SET	@GiaVonMoi = (@TongTienHangDemo / @SoLuongDemo) * (1 - (@TongGiamGia / @TongTienHang));
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
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
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon		
			
			update gv set GiaVonCu = isnull(GiaVonCu,0)
			from @GiaVonCapNhat gv 
    
    		--	select * from @GiaVonCapNhat
    		--Update BH_HoaDon_ChiTiet
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
    		--END Update BH_HoaDon_ChiTiet
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
    		--End Update DM_GiaVon
    		END
    		--END TinhGiaVonTrungBinh
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateHD_UpdateLichBaoDuong]
    @ID_HoaDon [uniqueidentifier],
    @IDHangHoas [nvarchar](max),
    @NgayLapHDOld [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @dtNow datetime = format( getdate(),'yyyy-MM-dd')
    	declare @SoKmMacDinhNgay int= 30, @countSC int =0;
    
    	declare @NgayLapHoaDon datetime , @ID_PhieuTiepNhan uniqueidentifier, @SoKmGanNhat float, @ID_Xe uniqueidentifier, @Now_SoKmVao int,  @Now_NgayVaoXuong datetime
    	select @ID_PhieuTiepNhan = ID_PhieuTiepNhan, @NgayLapHoaDon = NgayLapHoaDon from BH_HoaDon where id= @ID_HoaDon
    
    	---- get thongtin tiepnhan hientai
    	select @ID_Xe= ID_Xe, @Now_SoKmVao = isnull(SoKmVao,0), @Now_NgayVaoXuong = NgayVaoXuong from Gara_PhieuTiepNhan where ID = @ID_PhieuTiepNhan
    
    	---- get thongtin tiepnhan gan nhat
    	declare @NgayXuatXuong_GanNhat datetime , @SoKmRa_GanNhat float
    	select top 1 @NgayXuatXuong_GanNhat = isnull(NgayXuatXuong, NgayVaoXuong) ,  @SoKmRa_GanNhat= ISNULL(SoKmRa,0) 
    	from Gara_PhieuTiepNhan where isnull(NgayXuatXuong, NgayVaoXuong) < @Now_NgayVaoXuong and ID_Xe= @ID_Xe 
    	order by NgayVaoXuong 
    
    	
    	if @NgayXuatXuong_GanNhat is not null
    		begin
    			set @SoKmMacDinhNgay = CEILING( iif(@Now_SoKmVao - @SoKmRa_GanNhat=0,1, @Now_SoKmVao -@SoKmRa_GanNhat)/ iif(DATEDIFF(day, @NgayXuatXuong_GanNhat, @Now_NgayVaoXuong)=0,1,DATEDIFF(day, @NgayXuatXuong_GanNhat, @Now_NgayVaoXuong)))
    		end
    
    	---- get hang baoduong nhung bi xoa khi capnhat hoadon
    	declare @tblHH table(ID_HangHoa uniqueidentifier)
    	insert into @tblHH
    	select Name from dbo.splitstring(@IDHangHoas)
    
    	---- get caidat baoduong
    	select 
    		distinct bd.LoaiGiaTri,
    			bd.ID_HangHoa, 
    			bd.LanBaoDuong, 
    			iif(bd.BaoDuongLapDinhKy=1, bd.GiaTri,0) as GiaTriLap,
    			(select dbo.BaoDuong_GetTongGiaTriNhac(bd.LanBaoDuong,bd.ID_HangHoa)) as GiaTri,				
    			bd.BaoDuongLapDinhKy
    	into #tmpPhuTung
    	from DM_HangHoa_BaoDuongChiTiet bd
    	join @tblHH hh on bd.ID_HangHoa= hh.ID_HangHoa
    
    	select *
    		into #tnGanNhat
    		from(
    			select tn.ID_Xe, tn.NgayVaoXuong, tn.SoKmRa, qd.ID_HangHoa, ROW_NUMBER() over( partition by qd.ID_HangHoa order by tn.NgayVaoXuong) as RN		
    			from Gara_PhieuTiepNhan tn
    			join BH_HoaDon hd on tn.ID= hd.ID_PhieuTiepNhan
    			join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    			where hd.ID!=@ID_HoaDon
    			and tn.ID_Xe= @ID_Xe
    			and tn.TrangThai !=0
    			and exists (
    			select ID_HangHoa from @tblHH pt where qd.ID_HangHoa= pt.ID_HangHoa
    			)			
    		) a where RN= 1
    
    
    
    	   ---- get cthd new (after update)
    	   select *
    	   into #ctNew
    	   from(
    		   select qd.ID_HangHoa,
    				ct.ID_LichBaoDuong,
    				ROW_NUMBER() over(partition by qd.ID_HangHoa order by ct.ID_LichBaoDuong desc, qd.ID_HangHoa) as RN
    		   from BH_HoaDon_ChiTiet ct
    		   join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    		   where ct.ID_HoaDon= @ID_HoaDon
    	  ) a where a.RN= 1
    
    	  ---- phụ tùng mua mới, và đã dc setup lịch bảo dưỡng, nhưng bị xóa khi cập nhật hóa đơn
    	  ---- find cthd was delete: update TrangThai = 0 (exist in baoduong, but not exist in cthd)
    	  update lichnew set lichnew.TrangThai= 0
    	  from Gara_LichBaoDuong lichnew
    	  where exists (
    	  select lich.ID
    	  from Gara_LichBaoDuong lich
    	  where lich.ID_HoaDon= @ID_HoaDon
    	  and not exists (select ID_HangHoa from #ctNew ct where lich.ID_HangHoa = ct.ID_HangHoa)
    	  and lich.ID= lichnew.ID
    	  )
    
    
    ---- phụ tùng chuyen tu baoduong ---> khong baoduong
    
    		select *
    		into #lichOld
    		from
    		(
    		---- so sanh ngaylaphdold voi lich nhac baoduong gan nhat
    		---- neu > : xoa + insert
    			select a.*,
    				iif(DATEDIFF(day,a.NgayLapHoaDon,@NgayLapHDOld) > 0,1,0) as isUpdate
    			from
    			(
    			
    				select lich.*, 
    					hd.NgayLapHoaDon,
    					ROW_NUMBER() over (partition by lich.ID_HangHoa order by hd.NgayLapHoaDon desc) as RN
    				from Gara_LichBaoDuong lich
    				join BH_HoaDon hd on lich.ID_HoaDon= hd.ID
    				where lich.ID_Xe = @ID_Xe
    				and lich.TrangThai!=0
    				and lich.ID_HoaDon= @ID_HoaDon
    				and exists (select ID_HangHoa from @tblHH hh where lich.ID_HangHoa= hh.ID_HangHoa) 
    			) a 		
    			where a.RN= 1
    		) b where b.isUpdate= 1
    
    		delete lich
    		from Gara_LichBaoDuong lich
    		where lich.TrangThai= 1
    		and exists (select id from #lichOld old where lich.ID = old.ID)
    
    
    		insert into Gara_LichBaoDuong (ID, ID_HangHoa, ID_HoaDon, ID_Xe, LanBaoDuong, SoKmBaoDuong, NgayBaoDuongDuKien, TrangThai, NgayTao)
    			select NEWID() as ID, a.ID_HangHoa, @ID_HoaDon, @ID_Xe, a.LanBaoDuong,
    				a.SoKmBaoDuong, a.NgayBaoDuongDuKien, a.TrangThai, a.NgayTao
    			from
    			(
    				select  pt.ID_HangHoa, pt.LanBaoDuong,
    				case pt.LoaiGiaTri
    					when 4 then @Now_SoKmVao + pt.GiaTri 
    					else 0 end as SoKmBaoDuong,
    				case when pt.LoaiGiaTri = 4 then DATEADD(day, CEILING( pt.GiaTri/@SoKmMacDinhNgay), @NgayLapHoaDon)				
    					else  DATEADD(day, pt.GiaTri, @NgayLapHoaDon)									
    				end as NgayBaoDuongDuKien,
    				1 as TrangThai,
    				GETDATE() as NgayTao
    				from #tmpPhuTung pt				
    			) a where a.NgayBaoDuongDuKien >= @dtNow --- chi insert neu lich > ngayhientai
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateStatusCongBoSung_WhenCreatBangLuong]
    @ID_BangLuong [uniqueidentifier],
    @FromDate [datetime],
    @ToDate [datetime]
AS
BEGIN
    SET NOCOUNT ON;	
		set @ToDate = DATEADD(day, 1, @ToDate) ---- todate = denngay in NS_BangLuong

		---- get infor bangluong ----
    	declare @statusSalary int, @ID_DonVi uniqueidentifier
			select @statusSalary = TrangThai, @ID_DonVi = ID_DonVi from NS_BangLuong where ID= @ID_BangLuong
    	declare @statusCong int = @statusSalary

		if @statusSalary = 1 set @statusCong = 2

		--- bangluong - 0 (huy) --> cong = 1 (reset ve tao moi)
		--- bangluong - 1 (tamluu) --> cong = 2
		--- bangluong - 2 (cần tính lại) --> cong = 2
		--- bangluong - 3 (da chot) = cong
		--- bangluong - 4 (da thanh toan) = cong
	
    	if	@statusSalary like '[1-3]' -- @statusSalary= 1/2/3
			begin    						
				update bs set TrangThai = @statusCong, 
    						 ID_BangLuongChiTiet = blct.ID
    			from NS_CongBoSung bs
    			join NS_BangLuong_ChiTiet blct on bs.ID_NhanVien= blct.ID_NhanVien
    			where blct.ID_BangLuong= @ID_BangLuong
    			and bs.NgayCham between @FromDate and  @ToDate
				and bs.TrangThai in (1,2) --- taomoi, tamluu
				and bs.ID_DonVi = @ID_DonVi
			end
    	else
			begin
			if @statusSalary = 4
				begin
					----- get list id_bangluongchitiet
					select ID into #tblCTLuong from NS_BangLuong_ChiTiet where ID_BangLuong= @ID_BangLuong
	
					update bs set TrangThai = iif(soquy.DaThanhToan > 0 ,4,3)    					
    				from NS_CongBoSung bs
    				join #tblCTLuong blct on bs.ID_BangLuongChiTiet = blct.ID 						
					join (
						---- get soquy ttluong
						select qct.ID_BangLuongChiTiet,
							sum(iif(qhd.TrangThai ='0',0, qct.TienThu + isnull(qct.TruTamUngLuong,0))) as DaThanhToan
						from Quy_HoaDon_ChiTiet qct 
						join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
						where exists (select ID from #tblCTLuong ctluong where qct.ID_BangLuongChiTiet = ctluong.ID)
						group by qct.ID_BangLuongChiTiet
					) soquy on blct.ID= soquy.ID_BangLuongChiTiet
    				where bs.NgayCham between @FromDate and  @ToDate
				end
				else
					 ---- huybangluong
					begin													   		
							update bs set TrangThai = 1, 
    								ID_BangLuongChiTiet = null
    						from NS_CongBoSung bs
    						join NS_BangLuong_ChiTiet blct on bs.ID_BangLuongChiTiet = blct.ID
    						where blct.ID_BangLuong= @ID_BangLuong
    						and bs.NgayCham between @FromDate and  @ToDate									
					end    
			end   
END");

            Sql(@"ALTER PROCEDURE [dbo].[XoaDuLieuHeThong]
    @CheckHH [int],
    @CheckKH [int]
AS
BEGIN
SET NOCOUNT ON;

				delete from chotso
				delete from BH_HoaDon_ChiPhi
				delete from DM_MauIn
				delete from NS_CongViec
				delete from NS_CongViec_PhanLoai
    			delete from chotso_hanghoa
    			delete from chotso_khachHang
				delete from BH_NhanVienThucHien
    			delete from Quy_HoaDon_ChiTiet
    			delete from Quy_KhoanThuChi
    			delete from Quy_HoaDon
				delete from DM_TaiKhoanNganHang    			
    			delete from BH_HoaDon_ChiTiet
    			delete from BH_HoaDon
    			delete from DM_GiaBan_ApDung
    			delete from DM_GiaBan_ChiTiet
    			delete from DM_GiaBan
    			delete from ChamSocKhachHangs
				delete from HeThong_SMS
				delete from HeThong_SMS_TaiKhoan
				delete from HeThong_SMS_TinMau
				delete from ChietKhauMacDinh_NhanVien
				delete from ChietKhauMacDinh_HoaDon_ChiTiet
				delete from ChietKhauMacDinh_HoaDon
				delete from ChietKhauDoanhThu_NhanVien
				delete from ChietKhauDoanhThu_ChiTiet
				delete from ChietKhauDoanhThu
				delete from NhomDoiTuong_DonVi
				delete from DM_KhuyenMai_ChiTiet
    			delete from DM_KhuyenMai_ApDung
    			delete from DM_KhuyenMai
    			delete from ChietKhauMacDinh_NhanVien   
				 
				delete from Gara_HangMucSuaChua
				delete from Gara_GiayToKemTheo
    			delete from DM_KhuyenMai_ApDung
    			delete from DM_KhuyenMai
    			delete from ChietKhauMacDinh_NhanVien   
				delete from Gara_PhieuTiepNhan
				delete from Gara_DanhMucXe
    			delete from Gara_MauXe where id not like '%00000000-0000-0000-0000-000000000000%'
    			delete from Gara_HangXe where id not like '%00000000-0000-0000-0000-000000000000%'
    			delete from Gara_LoaiXe where id not like '%00000000-0000-0000-0000-000000000000%'
				
    			if(@CheckKH =0)
    			BEGIN
					delete from DM_LienHe_Anh
    				delete from DM_LienHe
					delete from DM_DoiTuong_Anh
					delete from DM_DoiTuong_Nhom
    				delete from DM_DoiTuong WHERE ID != '00000000-0000-0000-0000-000000000002' AND ID != '00000000-0000-0000-0000-000000000000'
					delete from DM_NguonKhachHang
					delete from DM_DoiTuong_TrangThai
    				delete from DM_NhomDoiTuong	  
    									
    			END
    			ELSE 
    			BEGIN
    				UPDATE DM_DoiTuong SET ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE', ID_NhanVienPhuTrach = null, TongTichDiem = 0 
    			END
    		 			
    			if(@CheckHH = 0)
    			BEGIN
						delete from DM_GiaVon
						delete from DM_HangHoa_TonKho
    				   	delete from DinhLuongDichVu
    					delete from DonViQuiDoi
    					delete from HangHoa_ThuocTinh
						delete from DM_HangHoa_ViTri  
    					delete from DM_HangHoa_Anh
    					delete from DM_HangHoa  				
    					delete from DM_ThuocTinh				  				  				
    					delete from DM_NhomHangHoa where ID != '00000000-0000-0000-0000-000000000000' and ID != '00000000-0000-0000-0000-000000000001'
    			END
				ELSE
				BEGIN
					DELETE DM_GiaVon WHERE ID_LoHang is not null
					DELETE DM_GiaVon WHERE ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
					DELETE DM_HangHoa_TonKho WHERE ID_LoHang is not null
					DELETE DM_HangHoa_TonKho WHERE ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
					UPDATE DM_GiaVon SET GiaVon = 0
					UPDATE DM_HangHoa_TonKho SET TonKho = 0
				END
				
    			delete from DM_LoHang
    			delete from DM_ViTri
    			delete from DM_KhuVuc
    			
    			delete from HT_NhatKySuDung where LoaiNhatKy != 20 and LoaiNhatKy != 21
    					
    			delete from CongDoan_DichVu
    			delete from CongNoDauKi
    			delete from DanhSachThi_ChiTiet	
    			delete from DanhSachThi
    			delete from DM_ChucVu
    			delete from DM_HinhThucThanhToan
    			delete from DM_HinhThucVanChuyen
    			delete from DM_KhoanPhuCap
    			delete from DM_LoaiGiaPhong
    			delete from DM_LoaiNhapXuat
    			delete from DM_LoaiPhieuThanhToan
    			delete from DM_LoaiPhong
    			delete from DM_LoaiTuVanLichHen
    			delete from DM_LopHoc
    			delete from DM_LyDoHuyLichHen
    			delete from DM_MaVach
    			delete from DM_MayChamCong
    			delete from DM_NoiDungQuanTam
    			delete from DM_PhanLoaiHangHoaDichVu
    			delete from DM_ThueSuat
    			
    			delete from HT_CauHinh_TichDiemApDung
    			delete from HT_CauHinh_TichDiemChiTiet		
    			delete from DM_TichDiem	
    			delete from NhomDoiTuong_DonVi where ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from NhomHangHoa_DonVi where ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from NS_LuongDoanhThu_ChiTiet 
    			delete from NS_LuongDoanhThu
    			delete from NS_HoSoLuong 
    			delete from The_NhomThe
    			delete from The_TheKhachHang_ChiTiet
    			delete from The_TheKhachHang
    
    			delete from HT_ThongBao
    			delete from HT_ThongBao_CaiDat where ID_NguoiDung != '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' 
    			delete from HT_Quyen_Nhom where ID_NhomNguoiDung IN (select ID From HT_NhomNguoiDung where ID NOT IN (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' AND ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'))
    			--delete from HT_NguoiDung_Nhom where IDNhomNguoiDung IN (select ID From HT_NhomNguoiDung where ID NOT IN (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' AND ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'))
				delete from HT_NguoiDung_Nhom where IDNguoiDung != '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' 
    			delete from HT_NhomNguoiDung where ID NOT IN (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' AND ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE')
    				
    			delete from HT_NguoiDung where ID != '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' 
				
				delete from NS_PhieuPhanCa_CaLamViec
				delete from NS_PhieuPhanCa_NhanVien
				delete from NS_CaLamViec_DonVi
				delete from NS_ThietLapLuongChiTiet
				delete from NS_CongNoTamUngLuong

				delete from NS_CongBoSung
				delete from NS_BangLuong_ChiTiet
				delete from NS_CaLamViec
				delete from NS_BangLuong			
				delete from NS_KyHieuCong
				delete from NS_NgayNghiLe
				delete from NS_PhieuPhanCa

				delete from NS_MienGiamThue
				delete from NS_KhenThuong
				delete from NS_HopDong
				delete from NS_BaoHiem
				delete from NS_Luong_PhuCap
				delete from NS_LoaiLuong
				delete from NS_NhanVien_CongTac
				delete from NS_NhanVien_DaoTao
				delete from NS_NhanVien_GiaDinh
				delete from NS_NhanVien_SucKhoe
				delete from NS_NhanVien_Anh	
    			delete from NS_QuaTrinhCongTac where ID_NhanVien NOT IN (select ID_NhanVien from HT_NguoiDung where ID = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77') or ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
				update NS_NhanVien SET ID_NSPhongBan = null
    			delete from NS_NhanVien where ID NOT IN (select ID_NhanVien from HT_NguoiDung where ID = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77')
    			delete from NS_PhongBan	 where ID_DonVi is not null and ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from Kho_DonVi where ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from DM_Kho where ID NOT IN (select ID_Kho from Kho_DonVi where ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE')
    			delete from DM_DonVi where ID !='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE';
	
END");

            Sql(@"ALTER FUNCTION [dbo].[DiscountSale_NVienDichVu]
(	
	@IDChiNhanhs varchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanVien varchar(40)
)
RETURNS TABLE 
AS
RETURN 
(
	select  2 as LoaiNhanVienApDung, tblNVienDV.ID_NhanVien, tblNVienDV.DoanhThu, 
	case when tblNVienDV.LaPhanTram = 1 then DoanhThu * GiaTriChietKhau / 100 else  GiaTriChietKhau end as HoaHongDoanhThu,
	tblNVienDV.ID
from
(
	select b.ID_NhanVien, b.DoanhThu, ckct.GiaTriChietKhau,ckct.LaPhanTram,ckct.ID,
	ROW_NUMBER() over (PARTITION  by b.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
	from
	(
			select  a.ID_NhanVien, sum(a.DoanhThu) - sum(a.GiaTriTra) as DoanhThu, a.ID_ChietKhauDoanhThu
			from
			(

				select 
					ckdtnv.ID_NhanVien , 
					nvth.ID_ChiTietHoaDon, 
					hd.MaHoaDon,   							
					---- HeSo * hoahong truoc/sau CK - phiDV
					iif(nvth.HeSo is null,1,nvth.HeSo) * (case when iif(nvth.TinhHoaHongTruocCK is null,0,nvth.TinhHoaHongTruocCK) = 1 
							then cthd.SoLuong * cthd.DonGia
							else cthd.SoLuong * (cthd.DonGia - cthd.TienChietKhau)
							end )
					- iif(hd.LoaiHoaDon=19,0, case when hh.ChiPhiTinhTheoPT =1 then cthd.SoLuong * cthd.DonGia * hh.ChiPhiThucHien/100
							else hh.ChiPhiThucHien * cthd.SoLuong end) as DoanhThu,
    				0 as GiaTriTra,
					ckdtnv.ID_ChietKhauDoanhThu, 
					ckdt.ApDungTuNgay,
					ckdt.ApDungDenNgay
				from ChietKhauDoanhThu ckdt
				join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
				join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
				join BH_HoaDon_ChiTiet cthd on nvth.ID_ChiTietHoaDon = cthd.ID 
				join DonViQuiDoi qd on cthd.ID_DonViQuiDoi = qd.ID
				join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
				join BH_HoaDon hd on cthd.ID_HoaDon= hd.ID
				and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
									and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
				where hd.ChoThanhToan= 0 
				and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)
				and hd.LoaiHoaDon in (1,19,22,25)
				and ckdt.LoaiNhanVienApDung= 2
				and hd.NgayLapHoaDon >= @FromDate  and hd.NgayLapHoaDon < @ToDate
				and cthd.ChatLieu!=4
				and nvth.ID_NhanVien like @IDNhanVien
				and ckdt.TrangThai= 1

				union all


				--- trahang
				select  ckdtnv.ID_NhanVien ,
					nvth.ID_ChiTietHoaDon,
					hdt.MaHoaDon,    								
    				0 as DoanhThu, 									
    				cthd.ThanhTien  as GiaTriTra,
    				ckdtnv.ID_ChietKhauDoanhThu,
					ckdt.ApDungTuNgay, 
					ckdt.ApDungDenNgay
    			from ChietKhauDoanhThu ckdt
    			join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
				join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
				join BH_HoaDon_ChiTiet cthd on nvth.ID_ChiTietHoaDon = cthd.ID 
    			join BH_HoaDon hdt on cthd.ID_HoaDon = hdt.ID 
				and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    			left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    			left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    			where 
    			exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hdt.ID_DonVi= dv.Name)
    			and ckdt.LoaiNhanVienApDung=2
    			and hdt.ChoThanhToan = '0' and hdt.LoaiHoaDon = 6
    			and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
				and (qhd.TrangThai is null or qhd.TrangThai != 0)
				and nvth.ID_NhanVien like @IDNhanVien
				and ckdt.TrangThai= 1
			) a group by a.ID_NhanVien, a.ID_ChietKhauDoanhThu
	) b 
join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
and (b.DoanhThu >= ckct.DoanhThuTu) 								
)tblNVienDV where tblNVienDV.Rn= 1
)
");

            Sql(@"ALTER TRIGGER [dbo].[UpdateNgayGiaoDichGanNhat_DMDoiTuong]
   ON [dbo].[BH_HoaDon]
   after insert, update
AS 
BEGIN

	SET NOCOUNT ON;
	declare @ID_KhachHang uniqueidentifier = (select top 1  ID_DoiTuong from inserted)
	DECLARE @NgayMaxTemp datetime;
	SELECT @NgayMaxTemp = NgayGiaoDichGanNhat FROM DM_DoiTuong where ID = @ID_KhachHang
	declare @NgayMax datetime;
	 
			select top 1 @NgayMax = hd.NgayLapHoaDon
			FROM BH_HoaDon hd
			where hd.ChoThanhToan is not null and hd.LoaiHoaDon != 23
				and hd.ID_DoiTuong= @ID_KhachHang and (hd.NgayLapHoaDon > @NgayMaxTemp OR @NgayMaxTemp IS NULL)
				ORDER BY NgayLapHoaDon desc
	if(@NgayMax IS NOT null)
	BEGIN
		update dt set NgayGiaoDichGanNhat = @NgayMax -- inserted.NgayLapHoaDon
		from DM_DoiTuong dt
		where ID = @ID_KhachHang
	END
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoHoatDongXe_ChiTiet]");
			DropStoredProcedure("[dbo].[BaoCaoHoatDongXe_TongHop]");
			DropStoredProcedure("[dbo].[GetInforHoaDon_ByID]");
            DropStoredProcedure("[dbo].[GetListImgInvoice_byCus]");
            DropStoredProcedure("[dbo].[HuyPhieuThu_UpdateTrangThaiCong]");
        }
    }
}
