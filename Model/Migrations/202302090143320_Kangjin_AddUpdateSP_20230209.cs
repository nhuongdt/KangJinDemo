namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Kangjin_AddUpdateSP_20230209 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Quy_HoaDon", "NoiDungThu", c => c.String(maxLength: 4000));
			CreateStoredProcedure(name: "[dbo].[LoadDanhMuc_KhachHangNhaCungCap]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				LoaiDoiTuong = p.Int(),
				IDNhomKhachs = p.String(),
				TongBan_FromDate = p.DateTime(),
				TongBan_ToDate = p.DateTime(),
				NgayTao_FromDate = p.DateTime(),
				NgayTao_ToDate = p.DateTime(),
				TextSearch = p.String(),
				Where = p.String(),
				ColumnSort = p.String(maxLength: 40),
				SortBy = p.String(maxLength: 40),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @whereCus nvarchar(max), @whereInvoice nvarchar(max), @whereLast nvarchar(max), 
	@whereNhomKhach nvarchar(max),	@whereChiNhanh nvarchar(max),
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
				set @whereInvoice= CONCAT(@whereInvoice, ' and exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)')
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
		select *
		from
		(
		select 
			dt.*,
			isnull(a.NoHienTai,0) as NoHienTai,
			isnull(a.TongBan,0) as TongBan,
			isnull(a.TongMua,0) as TongMua,
			isnull(a.TongBanTruTraHang,0) as TongBanTruTraHang,
			cast(isnull(a.SoLanMuaHang,0) as float) as SoLanMuaHang,
			isnull(a.PhiDichVu,0) as PhiDichVu,
			CONCAT(dt.MaDoiTuong,'' '', dt.TenDoiTuong, '' '', dt.DienThoai, '' '', dt.TenDoiTuong_KhongDau) as Name_Phone
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
				dt.NgayGiaoDichGanNhat,
				dt.TaiKhoanNganHang,
				isnull(dt.TenNhomDoiTuongs,N''Nhóm mặc định'') as TenNhomDT,
				dt.NgayTao,
				isnull(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
				isnull(dt.TongTichDiem,0) as TongTichDiem,
				----isnull(dt.TheoDoi,''0'') as TrangThaiXoa,
				isnull(dt.DienThoai,'''') as DienThoai,
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
				 tblThuChi.ID_DoiTuong,
    			SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) + sum(ISNULL(tblThuChi.ThuTuThe,0))
						- sum(isnull(tblThuChi.PhiDichVu,0)) 
						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    			SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
				sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    			SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    			SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    			SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang,
				sum(isnull(tblThuChi.PhiDichVu,0)) as PhiDichVu ')
		set @sql3=concat( N' from
			(
				---- chiphi dv ncc ----
				select 
					cp.ID_NhaCungCap as ID_DoiTuong,
					0 as GiaTriTra,
    				0 as DoanhThu,
					0 AS TienThu,
    				0 AS TienChi, 
    				0 AS SoLanMuaHang,
					0 as ThuTuThe,
					sum(cp.ThanhTien) as PhiDichVu
				from BH_HoaDon_ChiPhi cp
				join BH_HoaDon hd on cp.ID_HoaDon= hd.ID
				', @whereChiNhanh,
				N' and hd.ChoThanhToan = 0',
				 N' group by cp.ID_NhaCungCap

				union all
				----- tongban ----
				SELECT 
    				hd.ID_DoiTuong,    	
					0 as GiaTriTra,
    				hd.PhaiThanhToan as DoanhThu,
					0 AS TienThu,
    				0 AS TienChi, 
    				0 AS SoLanMuaHang,
					0 as ThuTuThe,
					0 as PhiDichVu
    			FROM BH_HoaDon hd ', @whereInvoice, N'  and hd.LoaiHoaDon in (1,7,19,25) ',

				N' union all
				---- doanhthu tuthe
				SELECT 
    				hd.ID_DoiTuong,    	
					0 as GiaTriTra,
    				0 as DoanhThu,
					0 AS TienThu,
    				0 AS TienChi, 
    				0 AS SoLanMuaHang,
					hd.PhaiThanhToan as ThuTuThe,
					0 as PhiDichVu
    			FROM BH_HoaDon hd ', @whereInvoice , N' and hd.LoaiHoaDon = 22 ', 

					N' union all
				-- gia tri trả từ bán hàng
				SELECT 
    				hd.ID_DoiTuong,    	
					hd.PhaiThanhToan as GiaTriTra,
    				0 as DoanhThu,
					0 AS TienThu,
    				0 AS TienChi, 
    				0 AS SoLanMuaHang,
					0 as ThuTuThe,
					0 as PhiDichVu
    			FROM BH_HoaDon hd ',  @whereInvoice, N'  and hd.LoaiHoaDon in (6,4) ',
				
				N' union all
				----- tienthu/chi ---
				SELECT 
    				qct.ID_DoiTuong,						
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				iif(qhd.LoaiHoaDon=11,qct.TienThu,0) AS TienThu,
    				iif(qhd.LoaiHoaDon=12,qct.TienThu,0) AS TienChi,
    				0 AS SoLanMuaHang,
					0 as ThuTuThe,
					0 as PhiDichVu
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qct ON qhd.ID = qct.ID_HoaDon ',
				@whereChiNhanh, 
				N' and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
				and qct.HinhThucThanhToan!= 6
				and (qct.LoaiThanhToan is null or qct.LoaiThanhToan != 3)
				
			

				union all
				----- solanmuahang ---
				Select 
    				hd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 as TienChi,
    				COUNT(*) AS SoLanMuaHang,
					0 as ThuTuThe,
					0 as PhiDichVu
    			From BH_HoaDon hd ' , @whereInvoice ,  N' and hd.LoaiHoaDon in (1,19,25) ',
    			N' group by hd.ID_DoiTuong
			)tblThuChi 
			GROUP BY tblThuChi.ID_DoiTuong
		) a on dt.ID= a.ID_DoiTuong 
		) tbl ', @Where ,
	'), 
	count_cte
	as
	(
		SELECT COUNT(ID) AS TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize_In as float)) as TotalPage,
				SUM(TongBanTruTraHang) as TongBanTruTraHangAll,
				SUM(TongTichDiem) as TongTichDiemAll,
				SUM(NoHienTai) as NoHienTaiAll,
				SUM(PhiDichVu) as TongPhiDichVu
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

		--print @sql
		--print @sql2
		--print @sql3


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
					@PageSize_In = @PageSize");
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Quy_HoaDon", "NoiDungThu", c => c.String(maxLength: 500));
            DropStoredProcedure("[dbo].[LoadDanhMuc_KhachHangNhaCungCap]");
        }
    }
}
