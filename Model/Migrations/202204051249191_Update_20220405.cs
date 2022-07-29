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
