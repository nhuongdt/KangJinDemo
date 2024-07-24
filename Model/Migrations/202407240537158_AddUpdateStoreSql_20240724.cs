namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateStoreSql_20240724 : DbMigration
    {
        public override void Up()
        {
            Sql(@"DROP PROCEDURE IF EXISTS [dbo].[GetChiTiet_NhatKyGiaoDich_UsedBaoHanh]");
            Sql(@"create PROCEDURE [dbo].[GetChiTiet_NhatKyGiaoDich_UsedBaoHanh] 
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
    
    	set @where = N' where 1 = 1 and hd.LoaiHoaDon in (1,2,36) and hd.ChoThanhToan = 0  
						and (ct.ID_ChiTietDinhLuong= ct.id OR ct.ID_ChiTietDinhLuong IS NULL) 
						and (ct.ID_ParentCombo != ct.ID OR ct.ID_ParentCombo IS NULL)'
    
    	if isnull(@CurrentPage,'') =''
    		set @CurrentPage = 0
    	if isnull(@PageSize,'') =''
    		set @PageSize = 20

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
			hd.ID_DonVi,
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
			iif(hd.LoaiHoaDon = 36,0,ct.DonGia - ct.TienChietKhau) as GiaBan,  ---- lay sau CK
			ct.TienChietKhau,
			ct.ThoiGianBaoHanh,
			ct.LoaiThoiGianBH,
			ct.GhiChu,
			ct.SoThuTu,
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
				isnull(tk.TonKho,0) as TonKho,
				hdXMLOut.HDCT_NhanVien as NhanVienThucHien,
    			CT_ChietKhauNV.TongChietKhau
			From data_cte dt
			left join DM_HangHoa_TonKho tk on dt.ID_DonViQuiDoi = tk.ID_DonViQuyDoi and tk.ID_DonVi = dt.ID_DonVi
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

            Sql(@"ALTER PROCEDURE [dbo].[GetGiaTriHoTro_ofCustomer] 
	@IDDoiTuong uniqueidentifier,
	@IDChiNhanhs nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;

    ------ get giatri ho tro of customer -----
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs) where Name!=''

			declare @countHD int =0;

				select @countHD = count(ID)			   				
    			from BH_HoaDon hd    			
    			where hd.ID_DoiTuong= @IDDoiTuong
    			and hd.LoaiHoaDon in (1,2,6,19,22,23,36)    			
    			and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)	

				if @countHD > 0
					begin


					----- get giatri sudung of cus (all time) ----
					select cthd.*,
						spht.Id_NhomHang as ID_NhomHoTro
					into #tblSuDung
					from
					(
    					select 
    				
    						hd.ID_DoiTuong,
    						hd.ID_DonVi,
    			
    						ct.ID_DonViQuiDoi,
    						ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as  GiaTriSuDung				
    			
    					from BH_HoaDon hd
    					join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
    			
    					where hd.ID_DoiTuong = @IDDoiTuong
    					and hd.LoaiHoaDon = 1
    					and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
						and exists (select id from NhomHang_ChiTietSanPhamHoTro spht 
				         where spht.Id_DonViQuiDoi = ct.ID_DonViQuiDoi and spht.LaSanPhamNgayThuoc = 2)
						 ) cthd
						 join NhomHang_ChiTietSanPhamHoTro spht on cthd.ID_DonViQuiDoi = spht.Id_DonViQuiDoi

					  ----- giavon tieuchuan cua dichvu/sanpham da caidat ----
    		declare @tblGVTC table(ID_DonVi uniqueidentifier, ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    		GiaVonTieuChuan float, NgayLapHoaDon datetime)
    
    		insert into @tblGVTC		
    		select hd.ID_DonVi,
    			ct.ID_DonViQuiDoi, 
    			ct.ID_LoHang, 
    		ct.ThanhTien as GiaVonTieuChuan,    	
    		hd.NgayLapHoaDon
    		from BH_HoaDon_ChiTiet ct 
    		join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    		where hd.ChoThanhToan=0
    		and  hd.ID_DonVi in (select ID from @tblChiNhanh)
    		and hd.LoaiHoaDon= 16
    		
    
    	
    
    			----- get khoang apdung hotro ----
    		select 
    			nhom.ID,
    			nhomdv.ID_DonVi,
    			khoangAD.GiaTriSuDungTu,
    			khoangAD.GiaTriSuDungDen,
    			khoangAD.GiaTriHoTro,
    			khoangAD.KieuHoTro
    		into #tblApDung
    		from DM_NhomHangHoa nhom
    		join NhomHangHoa_DonVi nhomdv on nhom.ID = nhomdv.ID_NhomHangHoa
    		join NhomHang_KhoangApDung khoangAD on nhom.ID= khoangAD.Id_NhomHang
    		where exists (select * from @tblChiNhanh cn where nhomdv.ID_DonVi = cn.ID)
    		and (nhom.TrangThai is null or nhom.TrangThai='0') ---- trangthainhom (0.đang dùng, 1.đã xóa)


			------ get GVTC of hoa don ------
    		select 
    			gvTC.*
    		into #tblHoTro
    		from
    		(
    				------ get gvtc theo khoang thoigian ----
    				select 
    					
    					hd.ID_DoiTuong,
    					hd.ID_DonVi,
    					hd.ID_CheckIn as ID_NhomHoTro,		
    					hd.MaHoaDon,
    					hd.NgayLapHoaDon,
    					gv.NgayLapHoaDon as NgayDieuChinh,
    					isnull(gv.GiaVonTieuChuan,0) as GiaVonTieuChuan,
    					ISNULL(ct.SoLuong,0) * isnull(gv.GiaVonTieuChuan,0) as GiaTriDichVu,						
    				ISNULL(ct.SoLuong,0) AS SoLuongXuat,		
    					----- nếu có nhiều khoảng GVTC: ưu tiên lấy NgayDieuChinhGV gần nhất ----
    					ROW_NUMBER() over (partition by ct.ID order by gv.NgayLapHoaDon desc) as RN
    	
    			from BH_HoaDon hd 		
    			left join BH_HoaDon_ChiTiet ct on ct.ID_HoaDon= hd.ID		
    			left join @tblGVTC gv on hd.ID_DonVi= gv.ID_DonVi and ct.ID_DonViQuiDoi= gv.ID_DonViQuiDoi 
    				and (ct.ID_LoHang = gv.ID_LoHang or (ct.ID_LoHang is null and gv.ID_LoHang is null))
    			where hd.ChoThanhToan=0	
    			and hd.LoaiHoaDon = 36			
				and hd.ID_DoiTuong = @IDDoiTuong
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			and (hd.NgayLapHoaDon > gv.NgayLapHoaDon or gv.NgayLapHoaDon is null)		
    		
    			and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
    			) gvTC
    			where gvTC.RN= 1 
    					
    			
    
    		

			
   select *,
						case 
    							when tView.GtriHoTroVND = 0 then tView.DaHoTro
    						else tView.DaHoTro/tView.GiaTriSuDung *100
    						end as PTramHoTro
   from
   (
    					select 
    						cusHT.*, 					
    						kAD.GiaTriHoTro,
							kAD.KieuHoTro,
    						case kAD.KieuHoTro
    							when 1 then isnull(kAD.GiaTriHoTro,0) * cusHT.GiaTriSuDung/100
    							when 0 then isnull(kAD.GiaTriHoTro,0)
    						else 0 
    						end as GtriHoTroVND
    					from
    					(

							select ht.*,
								isnull(sd.GiaTriSuDung,0) as GiaTriSuDung
							from
							(
							select ID_DoiTuong,		
    									ID_DonVi,				
    									ID_NhomHoTro,
    								sum(GiaTriDichVu) as DaHoTro
    							from #tblHoTro
    							group by ID_DoiTuong,ID_NhomHoTro,ID_DonVi	 
							)ht
							left join
							(
								select ID_DoiTuong,
    								ID_DonVi,			
    								ID_NhomHoTro,
    								sum(GiaTriSuDung) as GiaTriSuDung
    							from #tblSuDung
    							group by ID_DoiTuong, ID_DonVi,ID_NhomHoTro
							) sd
							on sd.ID_DoiTuong= ht.ID_DoiTuong and sd.ID_NhomHoTro = ht.ID_NhomHoTro and sd.ID_DonVi = ht.ID_DonVi    						
    					) cusHT
    					left join #tblApDung kAD on cusHT.ID_NhomHoTro = kAD.ID 
    						and cusHT.ID_DonVi = kAD.ID_DonVi 
    						and cusHT.GiaTriSuDung between kAD.GiaTriSuDungTu and kAD.GiaTriSuDungDen --- !!important: get khoang hotro
    					
		
					) tView
    	
    			
				drop table #tblApDung
				drop table #tblHoTro
				drop table #tblSuDung

					end

END");
            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhomHoTro]
	@IDChiNhanhs [nvarchar](max) ='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @DateFrom [datetime]='2024-07-21',
    @DateTo [datetime]='2024-08-01',
    @IDNhomHoTros [nvarchar](max)='',
    @TextSearch [nvarchar](max)='',
	@IsVuotMuc tinyint = 1, --1.all, 10. khong vuot, 11.vuot
    @CurrentPage [int] =0,
    @PageSize [int] =20
AS
BEGIN
    SET NOCOUNT ON;
    
    		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs) where Name!=''
    
    		declare @tblNhomHoTro table (ID uniqueidentifier)
    		if ISNULL(@IDNhomHoTros,'')!=''
    			insert into @tblNhomHoTro
    		select Name from dbo.splitstring(@IDNhomHoTros) where Name!=''
    		else
    			set @IDNhomHoTros =''
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    		DECLARE @count int;
    		if isnull(@TextSearch,'')!=''
    			INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    			Select @count =  (Select count(*) from @tblSearch);
    
    		----- giavon tieuchuan cua dichvu/sanpham da caidat ----
    		declare @tblGVTC table(ID_DonVi uniqueidentifier, ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    		GiaVonTieuChuan float, NgayLapHoaDon datetime)
    
    		insert into @tblGVTC		
    		select hd.ID_DonVi,
    			ct.ID_DonViQuiDoi, 
    			ct.ID_LoHang, 
    		ct.ThanhTien as GiaVonTieuChuan,    	
    		hd.NgayLapHoaDon
    		from BH_HoaDon_ChiTiet ct 
    		join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    		where hd.ChoThanhToan=0
    		and  hd.ID_DonVi in (select ID from @tblChiNhanh)
    		and hd.LoaiHoaDon= 16
    		and hd.NgayLapHoaDon < @DateTo	
    
    	
    
    			----- get khoang apdung hotro ----
    		select 
    			nhom.ID,
    			nhomdv.ID_DonVi,
    			khoangAD.GiaTriSuDungTu,
    			khoangAD.GiaTriSuDungDen,
    			khoangAD.GiaTriHoTro,
    			khoangAD.KieuHoTro
    		into #tblApDung
    		from DM_NhomHangHoa nhom
    		join NhomHangHoa_DonVi nhomdv on nhom.ID = nhomdv.ID_NhomHangHoa
    		join NhomHang_KhoangApDung khoangAD on nhom.ID= khoangAD.Id_NhomHang
    		where exists (select * from @tblChiNhanh cn where nhomdv.ID_DonVi = cn.ID)
    		and (nhom.TrangThai is null or nhom.TrangThai='0') ---- trangthainhom (0.đang dùng, 1.đã xóa)
    
    			------- get all sp thuocnhom hotro
    			select distinct
    				spht.Id_DonViQuiDoi,
    				spht.Id_LoHang,
    				spht.Id_NhomHang
    			into #tmpSPhamHT
    			from NhomHang_ChiTietSanPhamHoTro spht			
    			join NhomHangHoa_DonVi nhomCN on spht.Id_NhomHang = nhomCN.ID_NhomHangHoa
    			where spht.LaSanPhamNgayThuoc = 2
    			and exists (select ID from @tblChiNhanh cn where nhomCN.ID_DonVi = cn.ID)
    			and (@IDNhomHoTros='' or exists (select ID from @tblNhomHoTro nhomHT where nhomHT.ID = spht.Id_NhomHang))

				
    
    			
    			------ get all khachhang co phat sinh giao dịch from - to ----
    			select 	distinct		
    				hd.ID_DoiTuong,
    				hd.ID_DonVi,				
    				dt.ID_NhanVienPhuTrach,
    				dt.MaDoiTuong,
    				dt.TenDoiTuong
    			into #tblHDCus
    			from BH_HoaDon hd
    			join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			where hd.ChoThanhToan= 0
    			and hd.LoaiHoaDon in (1,2,6,19,22,23,36)
    			and hd.NgayLapHoaDon between @DateFrom and @DateTo
    			and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)	
    			and (@TextSearch ='' 
    				or
    					(select count(Name) from @tblSearch b where 
    						dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%' 
    						or dt.DienThoai like '%'+b.Name+'%' 
    					)=@count
    					or @count=0)
    		
    
	
    			----- get giatri sudung of cus (all time) ----
    			select 
    				hd.ID,
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon,
    				hd.ID_DoiTuong,
    				hd.ID_DonVi,
    				ct.ID_DonViQuiDoi,
    				spht.Id_NhomHang as ID_NhomHoTro,
    				ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as  GiaTriSuDung				
    			into #tblSuDung
    			from BH_HoaDon hd
    			join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
    			join #tmpSPhamHT spht on ct.ID_DonViQuiDoi = spht.Id_DonViQuiDoi  ----- chi get sp thuoc nhom hotro
    				and (ct.ID_LoHang = spht.Id_LoHang or ct.ID_LoHang is null and spht.Id_LoHang is null)
    			where exists (select ID_DoiTuong from #tblHDCus cus where hd.ID_DoiTuong = cus.ID_DoiTuong)
    			and hd.LoaiHoaDon in (1)
				and hd.ChoThanhToan ='0'
    			and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
    
    							

    		 
    
    		------ get GVTC of hoa don ------
    		select 
    			gvTC.*
    		into #tblHoTro
    		from
    		(
    				------ get gvtc theo khoang thoigian ----
    				select 
    					hd.ID as ID_HoaDon,
    					hd.ID_DoiTuong,
    					hd.ID_DonVi,
    					hd.ID_CheckIn as ID_NhomHoTro,		
    					hd.MaHoaDon,
    					hd.NgayLapHoaDon,
    					gv.NgayLapHoaDon as NgayDieuChinh,
    					isnull(gv.GiaVonTieuChuan,0) as GiaVonTieuChuan,
    					ISNULL(ct.SoLuong,0) * isnull(gv.GiaVonTieuChuan,0) as GiaTriDichVu,						
    				ISNULL(ct.SoLuong,0) AS SoLuongXuat,		
    					----- nếu có nhiều khoảng GVTC: ưu tiên lấy NgayDieuChinhGV gần nhất ----
    					ROW_NUMBER() over (partition by ct.ID order by gv.NgayLapHoaDon desc) as RN
    	
    			from BH_HoaDon hd 		
    			left join BH_HoaDon_ChiTiet ct on ct.ID_HoaDon= hd.ID		
    			left join @tblGVTC gv on hd.ID_DonVi= gv.ID_DonVi and ct.ID_DonViQuiDoi= gv.ID_DonViQuiDoi 
    				and (ct.ID_LoHang = gv.ID_LoHang or (ct.ID_LoHang is null and gv.ID_LoHang is null))
    			where hd.ChoThanhToan=0	
    			and hd.LoaiHoaDon = 36			
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			and (hd.NgayLapHoaDon > gv.NgayLapHoaDon or gv.NgayLapHoaDon is null)		
    			and exists (select ID_DoiTuong from #tblHDCus cus where hd.ID_DoiTuong = cus.ID_DoiTuong)
    			and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
    			) gvTC
    			where gvTC.RN= 1 
    			and	(@IDNhomHoTros='' or exists (select ID from @tblNhomHoTro nhom where gvTC.ID_NhomHoTro = nhom.ID))			
    			order by gvTC.NgayLapHoaDon desc
    
    	    
    		;with data_cte
    		as 
    		(
				select *
				from
				(
					select *,					
						iif(PTramHoTro > GtriHoTro_theoQuyDinh,'11','10') as IsVuotMuc
					from
					(
    					select    			
    						cus.MaDoiTuong,
    						cus.TenDoiTuong,
    						dv.TenDonVi,
    						nvpt.TenNhanVien,
    						tView.*,
							case 
    							when tView.GtriHoTroVND = 0 then tView.DaHoTro
    						else tView.DaHoTro/tView.GiaTriSuDung *100
    						end as PTramHoTro,
    						tView.GiaTriHoTro as GtriHoTro_theoQuyDinh
    					from #tblHDCus cus
    					left join ( --- 
    					select 
    						cusHT.*, 
    						nhom.TenNhomHangHoa as TenNhomHoTro,
    						kAD.GiaTriSuDungTu, 
    						kAD.GiaTriSuDungDen, 
    						kAD.GiaTriHoTro,
							kAD.KieuHoTro,
    						case kAD.KieuHoTro
    							when 1 then isnull(kAD.GiaTriHoTro,0) * cusHT.GiaTriSuDung/100
    							when 0 then isnull(kAD.GiaTriHoTro,0)
    						else 0 
    						end as GtriHoTroVND
    					from
    					(
								---- có thể có khách chưa sử uụng DV, nhưng vẫn được hỗ trợ)---
								---- vì khách được chuyển từ PM khác sang ---
								select ht.ID_DoiTuong,		
    									ht.ID_DonVi,				
    									ht.ID_NhomHoTro,
    								    ht.DaHoTro as DaHoTro,
									    isnull(sd.GiaTriSuDung,0) as GiaTriSuDung
    							from (
									select 
										ht.ID_DoiTuong,		
    									ht.ID_DonVi,				
    									ht.ID_NhomHoTro,
    								   sum(ht.GiaTriDichVu) as DaHoTro
									from #tblHoTro ht
									group by ht.ID_DoiTuong,ht.ID_NhomHoTro,ht.ID_DonVi	  
								) ht
								left join 
								(
									select ID_DoiTuong,
    								ID_DonVi,			
    								ID_NhomHoTro,
    								sum(GiaTriSuDung) as GiaTriSuDung
    								from #tblSuDung
    								group by ID_DoiTuong, ID_DonVi, ID_NhomHoTro
								) sd on sd.ID_DoiTuong= ht.ID_DoiTuong and sd.ID_NhomHoTro = ht.ID_NhomHoTro and sd.ID_DonVi = ht.ID_DonVi
   						
    					) cusHT
    					left join #tblApDung kAD on cusHT.ID_NhomHoTro = kAD.ID 
    						and cusHT.ID_DonVi = kAD.ID_DonVi 
    						and cusHT.GiaTriSuDung between kAD.GiaTriSuDungTu and kAD.GiaTriSuDungDen --- !!important: get khoang hotro
    					join DM_NhomHangHoa nhom on cusHT.ID_NhomHoTro = nhom.ID
    					) tView on cus.ID_DoiTuong = tView.ID_DoiTuong and cus.ID_DonVi= tView.ID_DonVi
    					left join DM_DonVi dv on tView.ID_DonVi = dv.ID
    					left join NS_NhanVien nvpt on cus.ID_NhanVienPhuTrach = nvpt.ID
						where tView.DaHoTro > 0 ---- chi lay khach  hotro
					) tblVuotMuc
				)tbLast where IsVuotMuc like concat('%' , @IsVuotMuc ,'%')
    		),
    		count_cte
    		as
    		(
    			select count(*) as TotalRow,
    				ceiling(count(*)/ CAST(@PageSize as float)) as TotalPage,
    				sum(GiaTriSuDung) as SumGiaTriSuDung,
    				sum(DaHoTro) as SumGiaTriHoTro
    			from data_cte
    		)
    		select *
    		from data_cte dt
    		cross join count_cte
    		order by dt.MaDoiTuong desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
    			
    
    			drop table #tblSuDung
    			drop table #tmpSPhamHT
    			drop table #tblHoTro
    			drop table #tblApDung
    			drop table #tblHDCus
END");
            Sql(@"ALTER PROCEDURE [dbo].[GetInforProduct_ByIDQuiDoi]
    @IDQuiDoi [uniqueidentifier],
    @ID_ChiNhanh [uniqueidentifier],
	@ID_LoHang uniqueidentifier null
AS
BEGIN
    SET NOCOUNT ON;
	if	@ID_LoHang is null
		set @ID_LoHang='00000000-0000-0000-0000-000000000000'

    		Select top 50
    			qd.ID as ID_DonViQuiDoi,
    			hh.ID,
    			qd.MaHangHoa,
    			hh.TenHangHoa,
    			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    			qd.TenDonViTinh,
    			hh.LaHangHoa,    			
    			qd.GiaBan,
    			qd.GiaNhap,
				qd.Xoa,
				hh.TheoDoi,					  			
    			lh.MaLoHang,
    			lh.NgaySanXuat,
    			lh.NgayHetHan,
				qd.LaDonViChuan,
				hh.ID_NhomHang as ID_NhomHangHoa,
				isnull(tk.TonKho,0) as TonKho,	
				iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1',1,2),hh.LoaiHangHoa) as LoaiHangHoa,
				Case when lh.ID is null then null else lh.ID end as ID_LoHang,
				Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
				Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
				Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
			case when ISNULL(QuyCach,0) = 0 then TyLeChuyenDoi else QuyCach * TyLeChuyenDoi end as QuyCach,
			ISNULL(hh.DonViTinhQuyCach,'0') as DonViTinhQuyCach,
			ISNULL(QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
			ISNULL(ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
			ISNULL(LoaiBaoHanh,0) as LoaiBaoHanh,
			ISNULL(SoPhutThucHien,0) as SoPhutThucHien, 
			ISNULL(hh.GhiChu,'') as GhiChuHH ,
			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio, 
			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem
    	from DonViQuiDoi qd    	
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
    	left join DM_LoHang lh on qd.ID_HangHoa = lh.ID_HangHoa and (lh.TrangThai = 1 or lh.TrangThai is null)
		left join DM_HangHoa_TonKho tk on qd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and tk.ID_DonVi = @ID_ChiNhanh
    	left join DM_GiaVon gv on qd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or lh.ID is null) and gv.ID_DonVi = @ID_ChiNhanh
    	where qd.ID = @IDQuiDoi
		and iif(@ID_LoHang='00000000-0000-0000-0000-000000000000', @ID_LoHang , lh.ID ) = @ID_LoHang
		
END");
            Sql(@"ALTER PROCEDURE [dbo].[GetListHoaDon_ChuaPhanBoCK]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
    @TextSearch [nvarchar](max),
    @LoaiChungTus [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	set @DateTo = dateadd(day,1, @DateTo) 
    
    		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');
    
    
    		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    		DECLARE @count int;
    		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    		Select @count =  (Select count(*) from @tblSearchString);
    		
    
    	declare @tblHoaDon table (ID uniqueidentifier, ID_DonVi uniqueidentifier, LoaiHoaDon int,
    			MaHoaDon  nvarchar(max), NgayLapHoaDon datetime,
    			IDSoQuy uniqueidentifier, MaPhieuThu nvarchar(max), NgayLapPhieuThu datetime,
    			MaDoiTuong nvarchar(max), TenDoiTuong nvarchar(max), DienThoai nvarchar(max),			
    		DoanhThu float, ThucThu float)
    		insert into @tblHoaDon
    			select hd.ID, 
    				hd.ID_DonVi,
    				hd.LoaiHoaDon,
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon, 			
    				sq.ID as IDSoQuy,
    				sq.MaHoaDon as MaPhieuThu,
    				sq.NgayLapHoaDon as NgayLapPhieuThu,				
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				----dt.DienThoai,	
					'' DienThoai,
    				case hd.LoaiHoaDon
    					when 6 then - hd.PhaiThanhToan
    					when 32 then - hd.PhaiThanhToan
    				else hd.PhaiThanhToan end as PhaiThanhToan,				
    				isnull(ThucThu,0) as ThucThu
    			from BH_HoaDon hd
    			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			left join (
    				select qct.ID_HoaDonLienQuan, 
    						qhd.ID,
    						qhd.MaHoaDon,
    						 qhd.NgayLapHoaDon,
    					SUM(iif(qhd.LoaiHoaDon=11, qct.TienThu,- qct.TienThu)) as ThucThu
    				from Quy_HoaDon_ChiTiet qct
    				join  Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    				where (qhd.TrangThai is null or qhd.TrangThai = '1')
    				and qhd.ID_DonVi in (select ID from @tblChiNhanh)				
    				and (qct.HinhThucThanhToan is null or qct.HinhThucThanhToan != 4)
    				group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID,qhd.MaHoaDon
    			) sq on hd.ID= sq.ID_HoaDonLienQuan
    			where hd.ChoThanhToan='0'
    			and hd.NgayLapHoaDon between @DateFrom and @DateTo	
				and hd.PhaiThanhToan > 0
    			and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    			and hd.LoaiHoaDon in (select LoaiChungTu from @tblChungTu)
    			and not exists
    				(select ID_HoaDon 
    				from BH_NhanVienThucHien th 
    				where hd.ID= th.ID_HoaDon 
					and sq.ID = th.ID_QuyHoaDon ---- phiếu thu chưa dc phân bổ hoa hồng ---
					and th.TienChietKhau > 0
					)
    			and
    				((select count(Name) from @tblSearchString b where     			
    				dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or dt.DienThoai like '%'+b.Name+'%'			
    					or hd.MaHoaDon like '%'+b.Name+'%'
    				)=@count or @count=0)	
    	
    		declare @tongDoanhThu float
    		select @tongDoanhThu = sum(hd.DoanhThu)
    		from (select distinct hd.ID,  hd.DoanhThu
    		from @tblHoaDon hd) hd		
    	
    			
    		;with data_cte
    		as(
    		select * from @tblHoaDon
		    where ThucThu > 0 --- chỉ lây hóa đơn đã thu tiền ---
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(ThucThu) as TongThucThu,
    					@tongDoanhThu as TongDoanhThu
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");
            Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMuc_KhachHangNhaCungCap]
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
    	declare @whereCus nvarchar(max), @whereCusTxt nvarchar(max), @whereInvoice nvarchar(max), @whereLast nvarchar(max), 
    	@whereNhomKhach nvarchar(max),	@whereChiNhanh nvarchar(max), @whereNgayLapHD nvarchar(max),
    	@sql nvarchar(max) , @sql1 nvarchar(max), @sql2 nvarchar(max), @sql3 nvarchar(max),@sql4 nvarchar(max),
    	@paramDefined nvarchar(max)
    
    		declare @tblDefined nvarchar(max) = concat(N' declare @tblChiNhanh table (ID uniqueidentifier) ',	
    												   N' declare @tblIDNhoms table (ID uniqueidentifier) ',
    												   N' declare @tblSearch table (Name nvarchar(max))'    											 
    												   )
    
			set @whereCusTxt = ' where 1 = 1'
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
    				
    				set @whereCusTxt = CONCAT(@whereCusTxt,
    				 N' and ((select count(Name) from @tblSearch b where 				
    				 Name_Phone like ''%''+b.Name+''%''    		
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
				select *
				from
				(
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
    						isnull(dt.DienThoai,'''') as DienThoai,
    						isnull(dt.Email,'''') as Email,
    						isnull(dt.DiaChi,'''') as DiaChi,
    						isnull(dt.MaSoThue,'''') as MaSoThue,
    						isnull(dt.GhiChu,'''') as GhiChu,
    						ISNULL(dt.NguoiTao,'''') as NguoiTao,
							CONCAT(dt.MaDoiTuong,'' '', dt.TenDoiTuong, '''', dt.DienThoai, '' '', dt.TenDoiTuong_KhongDau) as Name_Phone,
    						iif(dt.IDNhomDoiTuongs='''' or dt.IDNhomDoiTuongs is null,''00000000-0000-0000-0000-000000000000'', dt.IDNhomDoiTuongs) as ID_NhomDoiTuong
    					from DM_DoiTuong dt', @whereCus, N'
					) dt   ', @whereCusTxt,
					N' ),
					traGDV
					as
					(
						----- Hoàn dịch vụ: chỉ lấy phiếu chi trả hàng từ hóa đơn lẻ ---
						select 
							qct.ID_DoiTuong,
							sum(qct.TienThu) as GiaTriHoanTraGDV
						from BH_HoaDon hd
						join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
						join Quy_HoaDon_ChiTiet qct on hd.id = qct.ID_HoaDonLienQuan
						join Quy_HoaDon qhd on qhd.ID = qct.ID_HoaDon',
						@whereInvoice,
						N' and qhd.TrangThai = 1
						and ct.ChatLieu = 1 --- trahang hdle --
						and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.id)
						and exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)						
						group by qct.ID_DoiTuong					   
					),
					tblSuDung
					as
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
								N' and exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)
						)tbl group by tbl.ID_DoiTuong		
					),
					tblThuChi
					as
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
					sum(isnull(tblThuChi.NapCoc,0)) -sum(isnull(tblThuChi.SuDungCoc,0))  as SoDuCoc
					from
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
    				join BH_HoaDon hd on cp.ID_HoaDon= hd.ID',
    				@whereChiNhanh,
    				N' and hd.ChoThanhToan = 0
					and exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)
    				group by cp.ID_NhaCungCap

					union all
					
					 ---- hoantra sodu TGT cho khach (giam sodu TGT)

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
							0 as DoanhThuThe,
							0 as PhiDichVu,
							----- Loai = 23: (TongTienHang: chênh lệch giữa số dư cũ và số dư sau khi điều chỉnh (+/-) ---
							----- neu loai = 32: (TongGiamGia: chi phí hoàn trả: không ảnh hưởng đến số sư thẻ ----
							----- PhaiThanhToan: số tiền phải thanh toán sau khi trừ phí)
							----- lấy dấu âm (-TongGiamGia): vì TongChiKhachHang = - sum(HoanTraThe): trừ trừ thành cộng
							sum(iif(LoaiHoaDon = 23, hd.TongTienHang, -hd.TongGiamGia)) as DieuChinhSoDuTGT, 
							-sum(iif(LoaiHoaDon = 32, hd.PhaiThanhToan,0)) as HoanTraThe,
							0 as NapCoc,
							0 as SuDungCoc
    				FROM BH_HoaDon hd ',
					@whereChiNhanh,
					@whereNgayLapHD,
					N' and hd.LoaiHoaDon in (23,32) and hd.ChoThanhToan = 0 
					and exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)
					group by hd.ID_DoiTuong

						union all

					----- tongban, doanhthuthe, giatritra ----
    				SELECT 
    					hd.ID_DoiTuong,    	
						iif(hd.LoaiHoaDon in (4,6), hd.PhaiThanhToan,0) as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
						iif(hd.LoaiHoaDon in (1,7,19,25), hd.PhaiThanhToan,0) as DoanhThu, 
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as NapTienTGT,
						0 as SuDungTGT, 
						0 as ThanhToanGDV, 
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,    
						case hd.LoaiHoaDon
							when 22 then hd.PhaiThanhToan
							when 42 then - hd.PhaiThanhToan
						else 0 end as DoanhThuThe,
    					0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd',
					 @whereInvoice,
					 N' and exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)

					  union all 
					 ------ get giatri trahang tu GDV ----> tính vào Tổng bán trừ Trả hàng
					
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
						N' and ct.ChatLieu = 2  and hd.LoaiHoaDon = 6
						  and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.id)
						  and  exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)
						 group by hd.ID_DoiTuong

						 union all
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
    					and (qct.LoaiThanhToan is null or qct.LoaiThanhToan != 3)
						and  exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)


						 union all
    					------ hoancoc: chỉ lấy tiền hoàn lại khi mua GDV/hoặc hoàn cọc TGT ----
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
						and exists (
							select hd.id from BH_HoaDon hd 
								join BH_HoaDon_ChiTiet ctTra on hd.ID = ctTra.ID_HoaDon
								where hd.LoaiHoaDon = 6
								and ctTra.ChatLieu = 2
								and qct.ID_HoaDonLienQuan =  hd.ID
							    and  exists (select id from data_cte dt where hd.ID_DoiTuong = dt.ID)
							)
							

							union all
							---- napcoc, sudung coc --
						
					
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
						iif(qhdct.LoaiThanhToan = 1,iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu),0) as NapCocNCC,
						iif(qhdct.HinhThucThanhToan = 6,iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu),0) as SuDungCoc
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon',
					@whereChiNhanh, 
    				N' and qhd.TrangThai = 1
					 and  exists (select id from data_cte dt where qhdct.ID_DoiTuong = dt.ID)
				 	 )tblThuChi 
    					GROUP BY tblThuChi.ID_DoiTuong
					),
					tblWhereLast
					as(
					select *
					from
					(
						select 
							dt.*,				
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
								SUBSTRING(DienThoai,len(DienThoai) -2 , 3) as DienThoai_3SoCuoi
								from data_cte dt
								left join tblSuDung on dt.ID = tblSuDung.ID_DoiTuong
								left join traGDV on dt.ID = traGDV.ID_DoiTuong
								left join tblThuChi a on dt.ID = a.ID_DoiTuong
						)tbl', @where,
					N' ),
					count_cte
    				as
    				(
						select count(ID) as TotalRow,
						CEILING(COUNT(ID) / CAST(@PageSize_In as float)) as TotalPage
						from tblWhereLast
					)
					select dt.*,
						cte.*,
    				 ISNULL(trangthai.TenTrangThai,'''') as TrangThaiKhachHang,
    				ISNULL(qh.TenQuanHuyen,'''') as PhuongXa,
    				ISNULL(tt.TenTinhThanh,'''') as KhuVuc,
    				ISNULL(dv.TenDonVi,'''') as ConTy,
    				ISNULL(dv.SoDienThoai,'''') as DienThoaiChiNhanh,
    				ISNULL(dv.DiaChi,'''') as DiaChiChiNhanh,
    				ISNULL(nk.TenNguonKhach,'''') as TenNguonKhach,
    				ISNULL(dt2.TenDoiTuong,'''') as NguoiGioiThieu,
    				ISNULL(nvpt.NVPhuTrachs,'''') as TenNhanVienPhuTrach
    			from tblWhereLast dt
    			cross join count_cte cte    	
    			left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    			LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    			LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
    			LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID 
    			LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    			LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID
		left join (
			select distinct 
				nvptOut.ID_KhachHang,		
					(select nv.TenNhanVien +  '', '' AS [text()]
					from data_cte kh
					join KH_NVPhuTrach nvpt on kh.ID = nvpt.ID_KhachHang
					join NS_NhanVien nv on nvpt.ID_NhanVienPhuTrach = nv.ID
					where nvpt.ID_KhachHang = nvptOut.ID_KhachHang
					order by nvpt.VaiTro
					For XML PATH ('''')
					) NVPhuTrachs
				from KH_NVPhuTrach nvptOut
			 )nvpt on dt.ID = nvpt.ID_KhachHang 
			 ORDER BY ', @ColumnSort, ' ', @SortBy,
    		N' offset (@CurrentPage_In * @PageSize_In) ROWS
    		fetch next @PageSize_In ROWS only
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
    
    		print @sql1
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
END");
            Sql(@"ALTER PROCEDURE [dbo].[GetBaoCaoCongNoChiTiet]
	@IDChiNhanhs [nvarchar](max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @DateFrom [datetime] = '2023-01-01',
    @DateTo [datetime] ='2024-12-01',
    @TextSearch [nvarchar](max) = '',
    @TrangThais [nvarchar](4) = '',
    @CurrentPage [int] = 0,
    @PageSize [int] = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID varchar(40))
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs)
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);


	----- get hd from- to ----
	select hd.ID, hd.LoaiHoaDon
	into #tblHD
	from BH_HoaDon hd
    where hd.LoaiHoaDon in (1,19,22)
    and hd.ChoThanhToan = '0'
    and hd.TongThanhToan > 0
    and hd.NgayLapHoaDon between @DateFrom and @DateTo
    and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    
	select 
		tblLast.*,
			ISNULL(qtCN.GiaTriTatToan,0) as GiaTriTatToan,
			iif(tblLast.LoaiHoaDon=22, tblLast.ConNo1 - ISNULL(qtCN.GiaTriTatToan,0),tblLast.ConNo1) as ConNo,
			iif(tblLast.LoaiHoaDon=22, 0, ---- TGT: nợ thực tế: luôn = 0 ---
				---- nothucteGDV: khachtra - dasudung - gTriConLai sautra
				iif(tblLast.KhachDaTra >  tblLast.GiaTriSD or tblLast.GiaTriSD = 0, 0,
				----- Nếu có bù trừ: phải trừ đi gtrị bù trừ ---
					iif(tblLast.LuyKeTrahang < 0, tblLast.GiaTriSD - isnull(tblLast.TongTienHDTra,0) - tblLast.KhachDaTra,
					tblLast.GiaTriSD - tblLast.KhachDaTra))				
				) as NoThucTe
	into #tblDebit	
	from
	(
	select tblBuTru.*,
		iif(tblBuTru.ChoThanhToan is null,0, 
						tblBuTru.TongThanhToan - tblBuTru.TongTienHDTra - tblBuTru.KhachDaTra							
							) as ConNo1
	from
	(
	select tblLuyKe.*,
		iif(tblLuyKe.LoaiHoaDonGoc = 6, iif(tblLuyKe.LuyKeTraHang > 0, tblLuyKe.TongGiaTriTra, 
						iif(abs(tblLuyKe.LuyKeTraHang) > tblLuyKe.TongThanhToan, tblLuyKe.TongThanhToan, abs(tblLuyKe.LuyKeTraHang))), tblLuyKe.LuyKeTraHang) as TongTienHDTra	
	from
	(

		select c.* ,
					------ những hóa đơn lâu đời, chưa có trường TongThanhToan = 0/null --> assign TongThanhToan = PhaiThanhToan ---
					iif(c.TongThanhToan1 =0 and c.PhaiThanhToan> 0, c.PhaiThanhToan, c.TongThanhToan1) as TongThanhToan,
					isnull(iif(c.LoaiHoaDonGoc = 3 or c.ID_HoaDon is null,
						iif(c.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
							case when c.TongGiaTriTra > c.KhachNo then c.KhachNo						
							else c.TongGiaTriTra end),
						(select dbo.BuTruTraHang_HDDoi(ID_HoaDon,NgayLapHoaDon,ID_HoaDonGoc, LoaiHoaDonGoc))				
					),0) as LuyKeTraHang
			
				from
				(
    	select  
    		hd.ID,
			hd.ID_HoaDon,
    		hd.MaHoaDon,
    		hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
			hd.PhaiThanhToan,
    		hd.TongThanhToan as TongThanhToan1,
    		hd.DienGiai,
    		dt.MaDoiTuong,
    		dt.TenDoiTuong,
    		dv.TenDonVi,
			hd.ChoThanhToan,
			0 as BaoHiemDaTra,
			hd.ID_DoiTuong,
			ISNULL(hd.PhaiThanhToan,0) - ISNULL(soquy.KhachDaTra,0) as KhachNo,
    		isnull(soquy.KhachDaTra,0) as KhachDaTra,
    		isnull(sdGDV.GiaTriSD,0) as GiaTriSD,
    		iif(hd.TongThanhToan - isnull(soquy.KhachDaTra,0) > 0,1,0) as TrangThaiCongNo,
			isnull(hdgoc.LoaiHoaDon,0) as LoaiHoaDonGoc,
			hdgoc.ID_HoaDon as ID_HoaDonGoc,					
			hdgoc.MaHoaDon as MaHoaDonGoc,
			ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,
			ISNULL(allTra.NoTraHang,0) as NoTraHang
    	from #tblHD hd1
		join BH_HoaDon hd	on hd1.ID = hd.ID
    	join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	join DM_DonVi dv on hd.ID_DonVi = dv.ID	
    	left join
    	(
    		Select 
    			tblUnion.ID_HoaDonLienQuan,			
    			SUM(ISNULL(tblUnion.TienThu, 0)) as KhachDaTra			
    			from
    			(		------ thanhtoan itseft ----			
    					Select 
    						qct.ID_HoaDonLienQuan,
    						iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as TienThu				
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 					
    					where (qhd.TrangThai is null or qhd.TrangThai='1')	
						and exists (select id from #tblHD hd where hd.ID = qct.ID_HoaDonLienQuan)  				    
						------ kangjin: khong co DatHang: nen khoong lay thutuDH
    					
    					
    			) tblUnion
    			group by tblUnion.ID_HoaDonLienQuan
    	) soquy on hd.ID= soquy.ID_HoaDonLienQuan
    	left join(
    		------ sudung gdv
    		select 
    			gdv.ID,
    			sum(ctsd.SoLuong * (ctsd.DonGia - ctsd.TienChietKhau)) as GiaTriSD
    		from #tblHD gdv
    		join BH_HoaDon_ChiTiet ctm on gdv.ID = ctm.ID_HoaDon
    		 join BH_HoaDon_ChiTiet ctsd on ctm.ID= ctsd.ID_ChiTietGoiDV 
    		 join BH_HoaDon hdsd on ctsd.ID_HoaDon= hdsd.ID 
    		where gdv.LoaiHoaDon= 19 --and gdv.ChoThanhToan='0'
    		and hdsd.LoaiHoaDon = 1 and hdsd.ChoThanhToan ='0'
    		and (ctsd.ID_ChiTietDinhLuong is null or ctsd.ID_ChiTietDinhLuong = ctsd.ID)
    		group by gdv.ID
    	) sdGDV on hd.ID = sdGDV.ID    
		left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID
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
			) allTra on allTra.ID_HoaDon = hd.ID
    	where	
		((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'	
    					or hd.MaHoaDon like '%' +b.Name +'%' 		
    					)=@count or @count=0)	
    	) c	where (@TrangThais ='' or c.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))	 
		)tblLuyKe
    )tblBuTru
	)tblLast
	left join
	(
		select hd.ID_HoaDon,
			sum(hd.PhaiThanhToan) as GiaTriTatToan
		from BH_HoaDon hd
		where hd.ChoThanhToan='0'
		and LoaiHoaDon= 42
		group by hd.ID_HoaDon
	) qtCN on tblLast.ID= qtCN.ID_HoaDon



		declare @totalRow int, @totalPage float, @SumTongThanhToan float, @SumKhachDaTra float, @SumConNo float, @SumNoThucTe float
    		select @totalRow = count(dt.ID),
    			@totalPage = CEILING(COUNT(dt.ID) / CAST(@PageSize as float )),
    			@SumTongThanhToan = sum(TongThanhToan) ,
    			@SumKhachDaTra = sum(KhachDaTra),
    			@SumConNo = sum(ConNo) 		,
				@SumNoThucTe  = sum(NoThucTe) 
    			from #tblDebit dt

 
	select *,
		@totalRow as TotalRow,
		@totalPage as TotalPage,
		@SumTongThanhToan as TongThanhToanAll,
		@SumKhachDaTra as KhachDaTraAll,
		@SumConNo as ConNoAll,
		@SumNoThucTe as NoThucTeAll,
		hd.TongThanhToan - isnull(hd.TongTienHDTra,0) as GiaTriSauTra,		
		tblNV.TenNhanViens,
		isnull(tvchinh.MaNVTuVanChinh,'') as MaNVTuVanChinh,
		isnull(tvchinh.TenNVTuVanChinh,'') as TenNVTuVanChinh,
		isnull(tvphu.MaNVTuVanPhu,'') as MaNVTuVanPhu,
		isnull(tvphu.TenNVTuVanPhu,'') as TenNVTuVanPhu
	from #tblDebit hd
	left join
    	(
    		Select distinct
    		(
    			Select concat( nv.TenNhanVien ,' (',th.PT_ChietKhau, '%) ,') AS [text()]
    			From dbo.BH_NhanVienThucHien th
    			join dbo.NS_NhanVien nv on th.ID_NhanVien = nv.ID
    			where th.ID_HoaDon= nvth.ID_HoaDon
    			For XML PATH ('')
    		) TenNhanViens, 
    			nvth.ID_HoaDon
    		From dbo.BH_NhanVienThucHien nvth
    	) tblNV on tblNV.ID_HoaDon = hd.ID
		left join (
			select ID_KhachHang,
				max(nv.MaNhanVien) as MaNVTuVanChinh,
				max(nv.TenNhanVien) as TenNVTuVanChinh	
			from KH_NVPhuTrach nvpt 
			join NS_NhanVien nv on nvpt.ID_NhanVienPhuTrach = nv.ID
			where isnull(VaiTro,2) = 2
			group by ID_KhachHang
		)tvchinh on hd.ID_DoiTuong = tvchinh.ID_KhachHang
		left join (
			select ID_KhachHang,
				max(nv.MaNhanVien) as MaNVTuVanPhu,
				max(nv.TenNhanVien) as TenNVTuVanPhu
			from KH_NVPhuTrach nvpt
			join NS_NhanVien nv on nvpt.ID_NhanVienPhuTrach = nv.ID
			where VaiTro = 1
			group by ID_KhachHang
		)tvphu on hd.ID_DoiTuong = tvphu.ID_KhachHang
	 order by NgayLapHoaDon desc
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY

	drop table #tblDebit
	drop table #tblHD

END");
            Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountInvoice_Detail]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs [nvarchar](max),
    @TextSearch [nvarchar](max),
	@TxtCustomer [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @Status_DoanhThu [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
		set @DateTo = DATEADD(day,1,@DateTo)

		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');
		
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

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    		
    	
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @tblDiscountInvoice table (ID uniqueidentifier, ID_DoiTuong uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), NgayLapHoaDon datetime, NgayLapPhieu datetime, NgayLapPhieuThu datetime, MaHoaDon nvarchar(50),
    		DoanhThu float,
			ThucThu float,
			ChiPhiNganHang float,
			TongChiPhiNganHang float,
			ThucThu_ThucTinh float,
			HeSo float, HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, PTThucThu float, PTDoanhThu float, 
    		MaKhachHang nvarchar(max), TenKhachHang nvarchar(max), DienThoaiKH nvarchar(max), ID_NhanVienPhuTrach uniqueidentifier, TongAll float)
    
			----- bang tam chua DS phieu thu theo Ngay timkiem
			----- không groupby: vì phải lấy chi phí POS theo từng dòng
		select qct.ID_HoaDonLienQuan, 
			qhd.ID,
			qhd.NgayLapHoaDon, 		
			---- thanhtoan = TGT: nhung van chon NV thuchien
			iif(qct.HinhThucThanhToan in (4,5),0, iif( qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu)) as ThucThu,
			---- chi get chiphi with POS ----
			iif(qct.HinhThucThanhToan != 2,0, 1) as CountTaiKhoanPos,	
			iif(qct.HinhThucThanhToan != 2,0, qct.TienThu) as ThuPOS,		
			iif(qct.HinhThucThanhToan != 2,0, iif(qct.LaPTChiPhiNganHang='0', qct.ChiPhiNganHang,  
					qct.ChiPhiNganHang * qct.TienThu/100)) as ChiPhiNganHang					
    	into #tempQuy
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID 
		where (qhd.TrangThai is null or qhd.TrangThai = '1')
		and qhd.ID_DonVi in (select ID from @tblChiNhanh)
		and qhd.NgayLapHoaDon >= @DateFrom
    	and qhd.NgayLapHoaDon < @DateTo 


			------- tinhtongChiPhiNganHang theo phieuthu -----		
		select 
			qhd.ID,
			sum(ThuPOS) as TongThuPos,
			------ nếu chỉ có 1 taikhoan POS, lấy chính giátrị đó, ngược lại: lấy theo % ---
			iif(sum(CountTaiKhoanPos) =1, max(ChiPhiNganHang),  iif(sum(ThuPOS) = 0, 0,  sum(ChiPhiNganHang)/ sum(ThuPOS) * 100 )) as ChiPhiNganHang
		into #qhdChiPhiPos
		from #tempQuy qhd
		group by qhd.ID


		------- thucthu theo hoadon ----
		select ctquy.*, tblTong.TongThuThucTe
		into #tblQuyThucTe
		from (
			----- chiphipos theo hoadon --- 
			select
				qhd.ID,
				qhd.ID_HoaDonLienQuan,
				qhd.NgayLapHoaDon,
				cpPos.ChiPhiNganHang, ---- % hoặc vnd ---
				sum(ThucThu) as ThucThu,				
				iif(sum(qhd.CountTaiKhoanPos) = 1, cpPos.ChiPhiNganHang, cpPos.ChiPhiNganHang * sum(ThuPOS)/100) as TongChiPhiNganHang
				from #tempQuy qhd
				left join #qhdChiPhiPos cpPos on qhd.ID = cpPos.ID	
			group by qhd.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID,
				cpPos.ChiPhiNganHang
		) ctquy
		left join
		(
		select ID_HoaDonLienQuan,		
			sum(ThucThu) as TongThuThucTe
		from #tempQuy
		group by ID_HoaDonLienQuan
		) tblTong on ctquy.ID_HoaDonLienQuan= tblTong.ID_HoaDonLienQuan
		
    
    		select
				tbl.ID, ---- id of hoadon
				tbl.ID_DoiTuong,
				MaNhanVien, 
    			tbl.TenNhanVien,
    			tbl.NgayLapHoaDon,
    			tbl.NgayLapPhieu, -- used to check at where condition
    			tbl.NgayLapPhieuThu,
    			tbl.MaHoaDon,						
    			-- taoHD truoc, PhieuThu sau --> khong co doanh thu
    			case when  tbl.NgayLapHoaDon between @DateFrom and @DateTo then PhaiThanhToan else 0 end as DoanhThu, 
    			ISNULL(ThucThu,0) as ThucThu,
				tbl.ChiPhiNganHang,
				tbl.TongChiPhiNganHang,
				tbl.ThucThu - tbl.TongChiPhiNganHang as ThucThu_ThucTinh,
    			ISNULL(HeSo,0) as HeSo,
    			ISNULL(HoaHongThucThu,0) as HoaHongThucThu,
    			ISNULL(HoaHongDoanhThu,0) as HoaHongDoanhThu,
    			ISNULL(HoaHongVND,0) as HoaHongVND,
    			ISNULL(PTThucThu,0) as PTThucThu,
    			ISNULL(PTDoanhThu,0) as PTDoanhThu,
    			ISNULL(MaDoiTuong,'') as MaKhachHang,
    			ISNULL(TenDoiTuong,N'Khách lẻ') as TenKhachHang,
    			----ISNULL(dt.DienThoai,'') as DienThoaiKH,		
				'' as DienThoaiKH,		----- kangjin yêu cầu bảo mật SDT khách hàng --
				dt.ID_NhanVienPhuTrach,
    		case @Status_ColumHide
    			when  1 then cast(0 as float)
    			when  2 then ISNULL(HoaHongVND,0.0)
    			when  3 then ISNULL(HoaHongThucThu,0.0)
    			when  4 then ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongVND,0.0)
    			when  5 then ISNULL(HoaHongDoanhThu,0.0) 
    			when  6 then ISNULL(HoaHongDoanhThu,0.0) + ISNULL(HoaHongVND,0.0)
    			when  7 then ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongDoanhThu,0.0)
    		else ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongDoanhThu,0.0) + ISNULL(HoaHongVND,0.0)
    		end as TongAll
    		into #temp2
    	from 
    	(    		
    				select distinct MaNhanVien, TenNhanVien, 
						nv.TenNhanVienKhongDau, 
						hd.MaHoaDon, 
    					case when hd.LoaiHoaDon= 6 then - TongThanhToan + isnull(TongTienThue,0)
    					else case when hd.ID_DonVi in (select ID from @tblChiNhanh) then
							iif(hd.LoaiHoaDon=22, PhaiThanhToan, TongThanhToan - TongTienThue) else 0 end end as PhaiThanhToan,
    					hd.NgayLapHoaDon,
						tblQuy.ThucThu ,	
						tblQuy.ChiPhiNganHang ,					
						tblQuy.TongChiPhiNganHang,
						hd.LoaiHoaDon,
    					hd.ID_DoiTuong,
						hd.ID,
    					th.HeSo,
    					tblQuy.NgayLapHoaDon as NgayLapPhieuThu,
						

    				-- huy PhieuThu --> PTThucThu,HoaHongThucThu = 0		
    					case when TinhChietKhauTheo =1 
    						then case when LoaiHoaDon in ( 6, 32) then -TienChietKhau else 
    							case when ISNULL(ThucThu,0)= 0 then 0  else TienChietKhau end end end as HoaHongThucThu,
						th.PT_ChietKhau as PTThucThu,
    					--case when TinhChietKhauTheo =1 
    					--	then case when LoaiHoaDon in ( 6, 32) then PT_ChietKhau else 
    					--		case when ISNULL(ThucThu,0)= 0 then 0  else PT_ChietKhau end end end as PTThucThu,			    				
    					case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then
    						case when TinhChietKhauTheo = 2 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end else 0 end as HoaHongDoanhThu,
    					case when TinhChietKhauTheo =3 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end as HoaHongVND,
    					case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then
    						case when TinhChietKhauTheo = 2 then PT_ChietKhau end else 0 end as PTDoanhThu,
    					-- timkiem theo NgayLapHD or NgayLapPhieuThu
    					case when @DateFrom <= hd.NgayLapHoaDon and hd.NgayLapHoaDon < @DateTo then hd.NgayLapHoaDon else tblQuy.NgayLapHoaDon end as NgayLapPhieu ,
    					case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    			
    			from BH_NhanVienThucHien th		
    			join NS_NhanVien nv on th.ID_NhanVien= nv.ID
    			join BH_HoaDon hd on th.ID_HoaDon= hd.ID
    			left join #tblQuyThucTe tblQuy on th.ID_QuyHoaDon = tblQuy.ID and tblQuy.ID_HoaDonLienQuan = hd.ID --- join hoadon (truong hop phieuthu nhieu hoadon)
    			where th.ID_HoaDon is not null
    				and hd.LoaiHoaDon in (1,19,22,6, 25,3, 32)
    				and hd.ChoThanhToan is not null    				
					and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    				and (exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID))
    				--chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
    				and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 
					and ( exists (select ID from #tempQuy where th.ID_QuyHoaDon = #tempQuy.ID) or  LoaiHoaDon=6)))		
    					
    	) tbl
    		left join DM_DoiTuong dt on tbl.ID_DoiTuong= dt.ID
    		where tbl.NgayLapPhieu >= @DateFrom and tbl.NgayLapPhieu < @DateTo and TrangThaiHD = @StatusInvoice
			and 
    				((select count(Name) from @tblSearchString b where     			
    				tbl.TenNhanVien like '%'+b.Name+'%'
    				or tbl.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or tbl.MaNhanVien like '%'+b.Name+'%'	
    				or tbl.MaHoaDon like '%'+b.Name+'%'							
					or dt.DienThoai like '%'+b.Name+'%'
    				)=@count or @count=0)
			and (
				dt.MaDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong_KhongDau like N'%'+  @TxtCustomer +'%'
				or dt.DienThoai like N'%'+  @TxtCustomer +'%'
				)

    
    	if @Status_DoanhThu =0
    		insert into @tblDiscountInvoice
    		select *
    		from #temp2
    	else
    		begin
    				if @Status_DoanhThu= 1
    					insert into @tblDiscountInvoice
    					select *
    					from #temp2 where HoaHongDoanhThu > 0 or HoaHongThucThu != 0
    				else
    					if @Status_DoanhThu= 2
    						insert into @tblDiscountInvoice
    						select *
    						from #temp2 where HoaHongDoanhThu > 0 or HoaHongVND > 0
    					else		
    						if @Status_DoanhThu= 3
    							insert into @tblDiscountInvoice
    							select *
    							from #temp2 where HoaHongDoanhThu > 0
    						else	
    							if @Status_DoanhThu= 4
    								insert into @tblDiscountInvoice
    								select *
    								from #temp2 where HoaHongVND > 0 Or HoaHongThucThu != 0
    							else
    								if @Status_DoanhThu= 5
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2 where  HoaHongThucThu > 0
    								else -- 6
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2  where HoaHongVND > 0
    								
    			end;

				declare @tongDoanhThu float, @tongThucThu float

				select @tongDoanhThu = (select sum (tblDT.DoanhThu)
											from
											(
												select  id, MaHoaDon, NgayLapHoaDon, max(DoanhThu) as DoanhThu
												from @tblDiscountInvoice
												group by ID, MaHoaDon, NgayLapHoaDon
											)tblDT
										)
	
				select @tongThucThu = (select sum(tblTT.ThucThu)
										from
										(
											select sum(ThucThu) as ThucThu
											from
											(
											select  id, MaHoaDon, max(ThucThu)  as ThucThu
											from @tblDiscountInvoice
											group by ID, MaHoaDon, NgayLapPhieuThu
											) tbl2 group by ID, MaHoaDon
										)tblTT
										);
    
    	with data_cte
    		as(
    		select * from @tblDiscountInvoice
    		),
    		count_cte
    		as (
    			select count(*) as TotalRow,
    				CEILING(COUNT(*) / CAST(@PageSize as float ))  as TotalPage,
					@tongDoanhThu as TongDoanhThu,
					@tongThucThu as TongThucThu,
    				sum(HoaHongThucThu) as TongHoaHongThucThu,
    				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
    				sum(HoaHongVND) as TongHoaHongVND,
    				sum(TongAll) as TongAllAll,
					sum(TongChiPhiNganHang) as SumAllChiPhiNganHang,
					@tongThucThu - sum(TongChiPhiNganHang) as SumThucThu_ThucTinh
    			from data_cte
    		)
    		select dt.*, cte.*,
				ISNULL(tvChinh.MaNhanVien,'') as MaNVTuVanChinh,				
				ISNULL(tvChinh.TenNhanVien,'') as TenNVTuVanChinh,
				ISNULL(tuvanphu.MaNhanVien,'') as MaNVTuVanPhu,
				ISNULL(tuvanphu.TenNhanVien,'') as TenNVTuVanPhu
    		from data_cte dt
			left join KH_NVPhuTrach nvpt1 on dt.ID_DoiTuong = nvpt1.ID_KhachHang and nvpt1.VaiTro = 2
			left join NS_NhanVien tvChinh on nvpt1.ID_NhanVienPhuTrach = tvChinh.ID
			left join KH_NVPhuTrach nvpt2 on dt.ID_DoiTuong = nvpt2.ID_KhachHang and nvpt2.VaiTro = 1
			left join NS_NhanVien tuvanphu on nvpt2.ID_NhanVienPhuTrach = tuvanphu.ID		
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");
            Sql(@"ALTER PROCEDURE [dbo].[getList_XuatHuy]
   @IDChiNhanhs nvarchar(max)= 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE',
	@DateFrom datetime= '2024-06-01',
	@DateTo datetime=  '2024-12-31',
	@LoaiHoaDons nvarchar(max)= '35,37,8,38,39',
	@TrangThais nvarchar(max)= '1,2',
	@TextSearch nvarchar(max)=null,
	@CurrentPage int= 0,
	@PageSize int = 9999999
AS
BEGIN
	set nocount on;
	
	if isnull(@CurrentPage,'') ='' set @CurrentPage= 0
	if isnull(@PageSize,'') ='' set @PageSize= 10

	declare @tblChiNhanh table(ID uniqueidentifier)
	if isnull(@IDChiNhanhs,'') !=''
	begin	
		insert into @tblChiNhanh 
		select name from dbo.splitstring(@IDChiNhanhs)
	end
	else
		set @IDChiNhanhs =''
	
	declare @tblLoai table(Loai int)
	if isnull(@LoaiHoaDons,'') !=''
	begin		
		insert into @tblLoai 
		select name from dbo.splitstring(@LoaiHoaDons)
	end
	else set @LoaiHoaDons =''

	declare @tblTrangThai table(TrangThai int)
	if isnull(@TrangThais,'') !=''
	begin		
		insert into @tblTrangThai 
		select name from dbo.splitstring(@TrangThais)	
	end
	else set @TrangThais =''

	DECLARE @tblSearchString TABLE (Name [nvarchar](max))
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch,' ') where  Name!=''
	Select @count =  (Select count(*) from @tblSearchString)


								

	; with data_cte
			as(
				select tbl.ID,					
					tbl.MaHoaDon,
					tbl.NgayLapHoaDon,
					tbl.NgaySua,
					tbl.ID_HoaDon,
					tbl.ID_PhieuTiepNhan,
					tbl.ID_NhanVien,
					tbl.ID_DonVi,		
				
					tbl.ChoThanhToan,
					tbl.LoaiHoaDon,
					tbl.LoaiPhieu,
					tbl.NguoiTaoHD,
					tbl.YeuCau,
					tbl.DienGiai,
					tbl.IsChuyenPhatNhanh,
					
					hdsc.MaHoaDon as MaHoaDonGoc,
					hdsc.DienGiai as ChiPhi_GhiChu, ---- mượn trường: lấy ra mẫu in hóa đơn---
					dv.TenDonVi,
					nv.TenNhanVien,
					tbl.TenDoiTuong,
					tbl.MaDoiTuong,
					cast(tbl.TrangThai as varchar(2)) as TrangThai
			from  
			(
				select 
					hd.ID,
    				hd.ID_NhanVien,
    				hd.ID_DonVi,
					hd.ID_HoaDon,
					hd.ID_PhieuTiepNhan,			
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon,   
					
    				hd.DienGiai,
					hd.DienGiai as DienGiaiUnsign,
					hd.ChoThanhToan,
					hd.NguoiTao as NguoiTaoHD, 		
					case hd.LoaiHoaDon
						when 8 then iif(hd.ChoThanhToan='0',hd.NgayLapHoaDon, hd.NgaySua)
					else hd.NgaySua end as NgaySua,
					isnull(hd.An_Hien,'0') as IsChuyenPhatNhanh,
					case hd.ChoThanhToan
						when 1 then 1
						when 0 then 2
					else 3 end as TrangThai,
					case hd.ChoThanhToan
						when 1 then N'Tạm lưu'
						when 0 then N'Hoàn thành'
					else N'Hủy bỏ' end as YeuCau,    
					---- 1.sudung gdv, 2.xuat banle, 3.xuat suachua, 8.xuatkho thuong, 12.xuatbaohanh, 35. xuat NVL, 36. xuat hotro chung, 37. xuat hotro ngaythuoc
					case hd.LoaiHoaDon			
						when 2 then 12
					else hd.LoaiHoaDon end LoaiHoaDon,
					case hd.LoaiHoaDon
						when 8 then N'Phiếu xuất kho'					
						when 40 then N'Xuất hỗ trợ chung' 
						when 39 then N'Xuất bảo hành'
						when 38 then N'Xuất bán lẻ'
						when 37 then N'Xuất hỗ trợ ngày thuốc'
						when 35 then N'Xuất nguyên vật liệu'
					else N'Xuất bán lẻ' end LoaiPhieu,

					case hd.LoaiHoaDon
						when 8 then ''
						when 40 then dt.TenDoiTuong
						when 39 then dt.TenDoiTuong
						when 38 then dt.TenDoiTuong
						when 37 then dt.TenDoiTuong
						when 35 then dt.TenDoiTuong
					else '' end TenDoiTuong,
					case hd.LoaiHoaDon
						when 8 then ''
						when 40 then dt.TenDoiTuong_KhongDau
						when 39 then dt.TenDoiTuong_KhongDau
						when 38 then dt.TenDoiTuong_KhongDau
						when 37 then dt.TenDoiTuong_KhongDau
						when 35 then dt.TenDoiTuong_KhongDau
					else '' end TenDoiTuong_KhongDau,
					case hd.LoaiHoaDon
						when 8 then ''
						when 40 then dt.MaDoiTuong
						when 39 then dt.MaDoiTuong
						when 38 then dt.MaDoiTuong
						when 37 then dt.MaDoiTuong
						when 35 then dt.MaDoiTuong
					else '' end MaDoiTuong
			from BH_HoaDon hd			
			left join DM_DoiTuong dt on hd.ID_Doituong= dt.ID
			where (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID))
				and hd.NgayLapHoaDon between @DateFrom and @DateTo
			) tbl
			join DM_DonVi dv on tbl.ID_DonVi = dv.ID
			left join BH_HoaDon hdsc on tbl.ID_HoaDon= hdsc.ID					
    		left join NS_NhanVien nv on tbl.ID_NhanVien = nv.ID
			where  (@LoaiHoaDons='' or  exists (select Loai from @tblLoai loai where tbl.LoaiHoaDon= loai.Loai))
				and (@TrangThais='' or exists (select TrangThai from @tblTrangThai tt where tbl.TrangThai= tt.TrangThai))
				and ((select count(Name) from @tblSearchString b 
									where tbl.MaHoaDon like '%' +b.Name +'%' 										
										or tbl.NguoiTaoHD like '%' +b.Name +'%' 
										or tbl.DienGiai like N'%'+b.Name+'%'
										or nv.TenNhanVien like '%'+b.Name+'%'
										or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
										or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
										or nv.MaNhanVien like '%'+b.Name+'%'									
										or hdsc.MaHoaDon like '%'+b.Name+'%'
										or tbl.TenDoiTuong like '%'+b.Name+'%'
										or tbl.TenDoiTuong_KhongDau like '%'+b.Name+'%'
										or tbl.MaDoiTuong like '%'+b.Name+'%'																		
										or tbl.DienGiaiUnsign like N'%'+b.Name+'%'
										)=@count or @count=0)
			 
			),
			ctxkThis
			as
			(
				select 
					hd.ID,
					ctxk.ID_ChiTietGoiDV,
					ctxk.SoLuong * ctxk.GiaVon as GiaTriXuat
				from data_cte hd
				join BH_HoaDon_ChiTiet ctxk on hd.ID = ctxk.ID_HoaDon
				where ctxk.ChatLieu is null or ctxk.ChatLieu !=5
			),
			ctNVL
			as(								
				select 
					ctxk.ID_ChiTietGoiDV,
					count(ctxk.ID_ChiTietGoiDV) as CountCTGDV
				from ctxkThis ctxk		
				join BH_HoaDon_ChiTiet ctmua on ctxk.ID_ChiTietGoiDV = ctmua.ID
				join BH_HoaDon hdm on ctmua.ID_HoaDon= hdm.ID
				where hdm.ChoThanhToan is not null
				group by ctxk.ID_ChiTietGoiDV											
			),
			ctxk2
			as(								
				select 
					ctNVL.CountCTGDV,
					ctxkThis.ID as ID_HoaDon
				from ctNVL 
				left join ctxkThis on ctNVL.ID_ChiTietGoiDV = ctxkThis.ID_ChiTietGoiDV
			),
			ctxk
			as(								
				select 
					hd.ID,
					sum(hd.GiaTriXuat) as TongTienHang
				from ctxkThis hd				
				group by hd.ID											
			),
			tblSum
			as (
				select 
					sum(TongTienHang) as SumTongTienHang
				from ctxk
			),
			count_cte
			as (
			select 
				count(ID) as TotalRow
			from data_cte
			),
			tblTop
			as(
			select dt.*,
				cte.TotalRow,
				ctxk.TongTienHang,
				SumTongTienHang
			from data_cte dt
			left join ctxk on dt.ID = ctxk.ID
			cross join count_cte cte
			cross join tblSum 
			order by dt.NgayLapHoaDon desc
			OFFSET (@CurrentPage * @PageSize) ROWS
			FETCH NEXT @PageSize ROWS ONLY
			),
			pXK
			as
			(				
				select hd.ID_HoaDon,
				 max(hd.NgayLapHoaDon) as MaxNgayLapHoaDon
				from tblTop hd
				group by hd.ID_HoaDon
			)
			
				select tblTop.*,
					cast(max(iif(isnull(ctxk2.CountCTGDV,0) > 1 and tblTop.NgayLapHoaDon != pXK.MaxNgayLapHoaDon 
						or (isnull(ctxk2.CountCTGDV,0) = 0 and tblTop.LoaiHoaDon != 8),'1','0')) as bit) as IsChuaXuatKho ---- mượn trường: phiếu xuất thừa
				from tblTop 
				left join ctxk2 on ctxk2.ID_HoaDon = tblTop.ID
				left join pXK on tblTop.ID_HoaDon = pXK.ID_HoaDon
			group by tblTop.ID,					
					tblTop.MaHoaDon,
					tblTop.NgayLapHoaDon,
					tblTop.NgaySua,
					tblTop.ID_HoaDon,
					tblTop.ID_PhieuTiepNhan,
					tblTop.ID_NhanVien,
					tblTop.ID_DonVi,		
				
					tblTop.ChoThanhToan,
					tblTop.LoaiHoaDon,
					tblTop.LoaiPhieu,
					tblTop.NguoiTaoHD,
					tblTop.YeuCau,
					tblTop.DienGiai,
					tblTop.IsChuyenPhatNhanh,
					
					tblTop.MaHoaDonGoc,
					tblTop.ChiPhi_GhiChu,	

					tblTop.TenDonVi,
					tblTop.TenNhanVien,
					tblTop.TenDoiTuong,
					tblTop.MaDoiTuong,
					tblTop.TrangThai,
					tblTop.TongTienHang,
					tblTop.TotalRow,
					tblTop.SumTongTienHang
					order by NgayLapHoaDon desc
	
END");
            Sql(@"ALTER PROCEDURE [dbo].[GetTPDinhLuong_ofCTHD]
	@ID_CTHD [uniqueidentifier] = 'd106cf67-1858-4da2-bd0b-04de42a4eb6d',
	@LoaiHoaDon int = null
AS
BEGIN
    SET NOCOUNT ON;

	if @LoaiHoaDon is null
	begin
		
		select  MaHangHoa, 
			TenHangHoa,
			ID_DonViQuiDoi, 
			TenDonViTinh, 
			SoLuong, 
			ct.GiaVon,
			ct.ID_HoaDon,
			ID_ChiTietGoiDV,
			ct.ID_LoHang,
			ct.SoLuongDinhLuong_BanDau,
			iif(hh.LoaiHangHoa is not null, hh.LoaiHangHoa, iif(hh.LaHangHoa='1',1,2)) as LoaiHangHoa,
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoaThayThe,
    		case when ISNULL(hh.QuyCach,0) = 0 then ISNULL(qd.TyLeChuyenDoi,1) else ISNULL(hh.QuyCach,0) * ISNULL(qd.TyLeChuyenDoi,1) end as QuyCach,
    		qd.TenDonViTinh as DonViTinhQuyCach,
			ct.GhiChu	,
			ceiling(qd.GiaNhap) as GiaNhap,
			qd.GiaBan as GiaBanHH,
			lo.MaLoHang, 
			lo.NgaySanXuat, 
			lo.NgayHetHan ---- used to nhaphang tu hoadon
    	from BH_HoaDon_ChiTiet ct		
    	left Join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	left join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		left join DM_LoHang lo on ct.ID_LoHang = lo.ID
    	where ID_ChiTietDinhLuong = @ID_CTHD and ct.ID != @ID_CTHD 
		and ct.SoLuong > 0
		and (ct.ChatLieu is null or ct.ChatLieu !='5')

		
	end
	else
		-- hdxuatkho co Tpdinhluong
		begin	
		
			-- get thongtin hang xuatkho
			declare @ID_DonViQuiDoi uniqueidentifier, @ID_LoHang uniqueidentifier,  @ID_HoaDonXK uniqueidentifier
			select @ID_DonViQuiDoi= ID_DonViQuiDoi, @ID_LoHang= ID_LoHang, @ID_HoaDonXK = ct.ID_HoaDon
			from BH_HoaDon_ChiTiet ct 
			where ct.ID = @ID_CTHD 


			-- chi get dinhluong thuoc phieu xuatkho nay
			select ct.ID_ChiTietDinhLuong,ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
				ct.SoLuong, ct.DonGia, ct.GiaVon, ct.ThanhTien,ct.ID_HoaDon, ct.GhiChu, ct.ChatLieu,
				qd.MaHangHoa, qd.TenDonViTinh,
				lo.MaLoHang,lo.NgaySanXuat, lo.NgayHetHan,
				hh.TenHangHoa,
				qd.GiaBan,
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				qd.TenDonViTinh,
				qd.ID_HangHoa,
				hh.QuanLyTheoLoHang,
				hh.LaHangHoa,
				hh.DichVuTheoGio,
				hh.DuocTichDiem,
				ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, 
				CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
				hh.ID_NhomHang as ID_NhomHangHoa, 
				ISNULL(hh.GhiChu,'') as GhiChuHH,
				iif(hh.LoaiHangHoa is not null, hh.LoaiHangHoa, iif(hh.LaHangHoa='1',1,2)) as LoaiHangHoa,
				iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoaThayThe
			from BH_HoaDon_ChiTiet ct
			Join DonViQuiDoi qd on ct.ID_ChiTietDinhLuong = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			left join DM_LoHang lo on ct.ID_LoHang= lo.ID
			where ct.ID_DonViQuiDoi= @ID_DonViQuiDoi 
			and ct.ID_HoaDon = @ID_HoaDonXK
			and ((ct.ID_LoHang = @ID_LoHang) or (ct.ID_LoHang is null and @ID_LoHang is null))			
			and (ct.ChatLieu is null or ct.ChatLieu !='5')
		end		
		
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
					----  36. HD hỗ trợ (do HD này hay bị lỗi không tính tồn kho, nên bỏ qua) --
					---- với lại: HD này cũng không bị trừ kho (chỉ khi xuất kho mới bị trừ kho) ----
						and hd.LoaiHoaDon NOT IN (3, 19, 25,29,36) 
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
            Sql(@"ALTER PROCEDURE [dbo].[BCBanHang_theoMaDinhDanh]
	@pageNumber [int] = 1,
    @pageSize [int] = 300,
    @SearchString [nvarchar](max)=N'ngày',
    @timeStart [datetime]='2023-11-25',
    @timeEnd [datetime]='2023-12-01',
    @ID_ChiNhanh [nvarchar](max) ='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @LoaiHangHoa [nvarchar](max)='%%',
    @TheoDoi [nvarchar](max)='%1%',
    @TrangThai [nvarchar](max)='%0%',
    @ID_NhomHang [uniqueidentifier] = null,
    @LoaiChungTu [nvarchar](max) = '1,35,36'
AS
BEGIN
    SET NOCOUNT ON;
    
    	set @pageNumber = @pageNumber -1;
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    	Select @count =  (Select count(*) from @tblSearchString);
    
    	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER)
    	INSERT INTO @tblChiNhanh
    	select Name from splitstring(@ID_ChiNhanh);
    
    	DECLARE @tblLoaiHoaDon TABLE(LoaiHoaDon int)
    INSERT INTO @tblLoaiHoaDon
    select Name from splitstring(@LoaiChungTu);
    
    
    	----- get cthd hotro ---
    	select 
    		hd.NgayLapHoaDon,
    		hd.MaHoaDon,
    		hd.LoaiHoaDon,
    		hd.TongGiamGia, ---- songaythuoc ---	
    		hd.ID_DoiTuong,
    		hd.ID_CheckIn as IdNhom_ApdungHotro,
    		ct.ID,
    		ct.ID_HoaDon,
    		ct.ID_DonViQuiDoi,
    		ct.SoLuong,
    		ct.DonGia,
    		ct.TienChietKhau, 
    		ct.ThanhTien,
    		ct.GhiChu,
    		hd.DienGiai,
    		0 as TienThue,
    		0 as GiamGiaHD,
			ct.SoThuTu
    	into #hdHoTro
    	from BH_HoaDon hd
    	join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    	where hd.ChoThanhToan= 0 and hd.LoaiHoaDon= 36	
    	and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    	and hd.NgayLapHoaDon between @timeStart and @timeEnd
	    and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)
    	and exists (select LoaiHoaDon from @tblLoaiHoaDon loai where loai.LoaiHoaDon = hd.LoaiHoaDon)
    
    	

    	select tblLast.*
		into #tblLast
    	from
    	(
    		------ get nhom apdung hotro ---
    		select 
    			null as ID, ---- chỉ có tác dụng để union ----
    			hd.ID_HoaDon,
    			hd.LoaiHoaDon,
    			hd.MaHoaDon,
    			hd.NgayLapHoaDon,
    			dt.MaDoiTuong,
    			dt.TenDoiTuong,
    			dt.TenDoiTuong_KhongDau,
    			dt.DienThoai as DienThoai1, ---- kangjin yêu cầu bảo mật SDT khách hàng ở full bao cáo
    			N'Ngày thuốc' as MaHangHoa,
				N'ngày' as TenDonViTinh,
    			N'Ngày thuốc' as TenHangHoa,
    			N'Ngày thuốc' as TenHangHoa_KhongDau,
    			nhom.ID as ID_NhomHang,
				N'Nhóm ngày thuốc'  as  TenNhomHangHoa,
    			hd.TongGiamGia as SoLuong,
    			0 as DonGia,
    			0 as TienChietKhau,
    			0 as ThanhTien,
    			hd.DienGiai as GhiChu,
    			hd.MaHoaDon as MaDinhDanh,
    			2 as LoaiHangHoa,
				0 as SoThuTu
    		from (
    			select hd.ID_HoaDon,
    				hd.LoaiHoaDon,
    				hd.IdNhom_ApdungHotro,
    				hd.ID_DoiTuong,
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon,
    				hd.TongGiamGia,
    				hd.DienGiai
    			from  #hdHoTro hd
    			--where exists (select * from BH_ChiTiet_DinhDanh dd where hd.ID= dd.IdHoaDonChiTiet)
    			group by hd.ID_HoaDon,
    				hd.LoaiHoaDon,
    				hd.IdNhom_ApdungHotro,
    				hd.ID_DoiTuong,
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon,
    				hd.TongGiamGia,
    				hd.DienGiai
    		) hd
    		join DM_NhomHangHoa nhom on hd.IdNhom_ApdungHotro = nhom.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	
    
    		union all
    	
    		------ tblUnion thong tin chitiet ---
    		select 
    			tblUnion.ID, --- used to get NVThucHien ----
    			tblUnion.ID_HoaDon,
    			tblUnion.LoaiHoaDon,
    			tblUnion.MaHoaDon,
    			tblUnion.NgayLapHoaDon,
    			dt.MaDoiTuong,
    			dt.TenDoiTuong,
    			dt.TenDoiTuong_KhongDau,
    			dt.DienThoai,
    			qd.MaHangHoa,
				qd.TenDonViTinh,
    			hh.TenHangHoa,	
    			hh.TenHangHoa_KhongDau,
    			hh.ID_NhomHang,
    			nhom.TenNhomHangHoa,
    			tblUnion.SoLuong,
    			tblUnion.DonGia,
    			tblUnion.TienChietKhau,
    			tblUnion.ThanhTien,
    			tblUnion.GhiChu,
    			cast(isnull(dinhdanh.MaDinhDanh,0) as varchar(max)) as MaDinhDanh,
    			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa,
				tblUnion.SoThuTu
    		from
    		(
    			----- sp hotro ngaythuoc ----
    			select 
					hd.SoThuTu,
    				hd.NgayLapHoaDon,
    				hd.MaHoaDon,
    				hd.LoaiHoaDon,
    				hd.TongGiamGia, ---- songaythuoc ---	
    				hd.ID_DoiTuong,		
    				hd.ID,
    				hd.ID_HoaDon,
    				hd.ID_DonViQuiDoi,
    				hd.SoLuong,
    				hd.DonGia,
    				0 as TienChietKhau, 
    				hd.ThanhTien,
    				hd.GhiChu,
    				hd.DienGiai,
    				0 as TienThue,
    				0 as GiamGiaHD					
    			from #hdHoTro hd
    
    	
    			union all
    
    			---- get cthd (hdle, baohanh) --
    			select 
						ct.SoThuTu,
    					hd.NgayLapHoaDon,
    					hd.MaHoaDon,
    					hd.LoaiHoaDon,    	
    					hd.TongGiamGia,   	
    					hd.ID_DoiTuong,    			
    					ct.ID,
    					ct.ID_HoaDon,
    					ct.ID_DonViQuiDoi,     		
    					ct.SoLuong,
						---- cot DonGia: lấy sau chietkhau (để cho giống bên audo - c Huyen bao) --
						--- cot ChietKhau: gan = 0
						ct.DonGia - ct.TienChietKhau as DonGia,
    					cast(0 as float) as TienChietKhau,		
						ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as ThanhTien, -- sudung GDV: van lay thanhtien ----
    					ct.GhiChu,    	
    					hd.DienGiai,
    					ct.TienThue * ct.SoLuong  as TienThue,
    					Case when hd.TongTienHang = 0 then 0 else ct.ThanhTien * ((hd.TongGiamGia + isnull(hd.KhuyeMai_GiamGia,0)) / hd.TongTienHang) end as GiamGiaHD
    			from BH_HoaDon_ChiTiet ct
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID	
    			where hd.ChoThanhToan=0 and hd.LoaiHoaDon !=36
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			and hd.NgayLapHoaDon between @timeStart and @timeEnd
    			and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)
    			and exists (select LoaiHoaDon from @tblLoaiHoaDon loai where loai.LoaiHoaDon = hd.LoaiHoaDon)
    		)tblUnion	
    	join DonViQuiDoi qd on tblUnion.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
    	join BH_ChiTiet_DinhDanh dinhdanh on tblUnion.ID = dinhdanh.IdHoaDonChiTiet or tblUnion.ID is null  ---- Chỉ lấy hóa đơn có mã định danh từ khi bắt đầu làm tính năng mới này ---
    	left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
    	left join DM_DoiTuong dt on tblUnion.ID_DoiTuong = dt.ID	
    	where hh.TheoDoi like @TheoDoi		
    	) tblLast
    	where 	
    		(@LoaiHangHoa ='%%' or tblLast.LoaiHangHoa in (select name from dbo.splitstring(@LoaiHangHoa)))	
    		and 
    		(@ID_NhomHang is null or exists  (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where tblLast.ID_NhomHang = allnhh.ID))
    		AND
    		((select count(Name) from @tblSearchString b where 
    				tblLast.MaHoaDon like '%'+b.Name+'%' 
    			or tblLast.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or tblLast.TenHangHoa like '%'+b.Name+'%'
    			or tblLast.MaHangHoa like '%'+b.Name+'%'
    			or tblLast.TenNhomHangHoa like '%'+b.Name+'%'
    				or tblLast.TenDoiTuong like '%'+b.Name+'%'
    			or tblLast.TenDoiTuong_KhongDau  like '%'+b.Name+'%'
    				or tblLast.MaDoiTuong like '%'+b.Name+'%'
    			or tblLast.DienThoai1  like '%'+b.Name+'%'
    				or tblLast.GhiChu like N'%'+b.Name+'%'
    			)=@count or @count=0)

		


				declare @totalRow int =(select count(MaHangHoa) from  #tblLast)

				select 
					tbl.*,
					@totalRow as TotalRow,
					CEILING(@totalRow/ cast (@pageSize as float)) as TotalPage,
					isnull(maNV.NVThucHien,'') as MaNhanVien,
					isnull(tenNV.NVThucHien,'') as TenNhanVien	
				from #tblLast tbl
				left join
    			(
    			-- get TenNV thuchien of cthd
    			select distinct tblCT.ID as ID_ChiTietHD ,
    					(
    						select nv.TenNhanVien +', '  AS [text()]
    						from BH_NhanVienThucHien nvth
    						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    						where nvth.ID_ChiTietHoaDon = tblCT.Id										
    						For XML PATH ('')
    					) NVThucHien
    				from #tblLast tblCT 
    			) tenNV on tbl.ID = tenNV.ID_ChiTietHD
				left join
    			(
    			-- get MaNV nvthuchien of cthd
    			select distinct tblCT.ID as ID_ChiTietHD ,
    					(
    						select nv.MaNhanVien +', '  AS [text()]
    						from BH_NhanVienThucHien nvth
    						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    						where nvth.ID_ChiTietHoaDon = tblCT.ID										
    						For XML PATH ('')
    					) NVThucHien
    				from #tblLast tblCT 
				) maNV on tbl.ID = maNV.ID_ChiTietHD
				order by tbl.NgayLapHoaDon desc, tbl.SoThuTu asc
				OFFSET (@pageNumber* @pageSize) ROWS
				FETCH NEXT @pageSize ROWS ONLY

   
    	drop table #hdHoTro
    	drop table #tblLast

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
    						ct.SoLuong, ct.TienChietKhau,
							isnull(ct.GiaVon,0) as GiaVon,
							ct.TonLuyKe, ct.GhiChu,
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
    						ct.SoLuong, ct.TienChietKhau,
							isnull(ct.GiaVon,0) as GiaVon,
							ct.TonLuyKe, 
							ct.GhiChu,
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
            Sql(@"ALTER PROCEDURE [dbo].[GetTonKho_byIDQuyDois]
	@ID_ChiNhanh [uniqueidentifier] ='8f01a137-e8ae-4239-ad96-4de67b2fec25',
	@IdHoaDonUpdate uniqueidentifier = null,
    @ToDate [datetime] ='2024-05-18 09:30',
    @IDDonViQuyDois [nvarchar](max)='902fec0e-9a40-4830-99f1-224b589a1978',
    @IDLoHangs [nvarchar](max)=''
AS
BEGIN
	 SET NOCOUNT ON;

		 declare @tblIDQuiDoi table (ID_DonViQuyDoi uniqueidentifier)
    	declare @tblIDLoHang table (ID_LoHang uniqueidentifier)
    
    	insert into @tblIDQuiDoi
    	select Name from dbo.splitstring(@IDDonViQuyDois) 
    	insert into @tblIDLoHang
    	select Name from dbo.splitstring(@IDLoHangs) where Name not like '%null%' and Name !=''

	 declare @toDateFormat varchar(14) = format(@ToDate,'yyyyMMdd HH:mm')
	 declare @dateNow varchar(14) = format(getdate(),'yyyyMMdd HH:mm')

	
	---- get giavon tieuchuan theo thoigian
		declare @tblGVTieuChuan table(ID_DonVi uniqueidentifier,
							ID_DonViQuiDoi uniqueidentifier,
							ID_LoHang uniqueidentifier, 
							GiaVonTieuChuan	float)
		insert into @tblGVTieuChuan
		exec GetGiaVonTieuChuan_byTime @ID_ChiNhanh, @ToDate, @IDDonViQuyDois, @IDLoHangs 


	---- chấp nhận chênh lệch số giây (chỉ so sánh giờ-phút) ---
	 if @toDateFormat = @dateNow
	 begin
			select 
				qd.ID_HangHoa,
				qd.ID as ID_DonViQuiDoi,			
				tkgv.ID_LoHang, ----- vì bên kangjin không quan lý theo lô nên không muốn join DM_LoHang
				qd.GiaNhap,
				isnull(tkgv.TonKho,0) as TonKho,
				isnull(tkgv.GiaVon,0) as GiaVon,
				isnull(gvtc.GiaVonTieuChuan,0) as GiaVonTieuChuan
			from DonViQuiDoi qd 		
			left join (
				select 
					tk.ID_DonViQuyDoi as ID_DonViQuiDoi, 
					tk.ID_LoHang,
					tk.TonKho,
					gv.GiaVon
				from DM_HangHoa_TonKho tk
				left join DM_GiaVon gv on tk.ID_DonViQuyDoi= gv.ID_DonViQuiDoi and tk.ID_DonVi = gv.ID_DonVi	
				where tk.ID_DonVi = @ID_ChiNhanh
				and exists (select qd.ID_DonViQuyDoi from @tblIDQuiDoi qd where tk.ID_DonViQuyDoi = qd.ID_DonViQuyDoi)
			) tkgv on tkgv.ID_DonViQuiDoi = qd.ID		
			left join @tblGVTieuChuan gvtc on tkgv.ID_DonViQuiDoi = gvtc.ID_DonViQuiDoi 
			where exists (select qd2.ID_DonViQuyDoi from @tblIDQuiDoi qd2 where qd2.ID_DonViQuyDoi = qd.ID)
	 end
	 else
	 begin

			 ---1. get all hanghoa by idquydoi ---	 		
			select qd2.ID, qd2.TyLeChuyenDoi,  qd2.ID_HangHoa, qd2.GiaNhap
			into #tblQD
			from DonViQuiDoi qd
			join DonViQuiDoi qd2 on qd.ID_HangHoa = qd2.ID_HangHoa
			where exists (select qdIn.ID_DonViQuyDoi from @tblIDQuiDoi qdIn where qd.ID = qdIn.ID_DonViQuyDoi)



						-- get tonluyke theo thoigian  ---
						select 
							tblLast.*
						into #tblTon
						from
						(
							SELECT qdOut.ID_HangHoa,
								   cthd.ID_LoHang, 									
									cthd.TonLuyKe as TonKho,
									cthd.GiaVon,
									ROW_NUMBER() OVER (PARTITION BY qdOut.ID_HangHoa, cthd.ID_LoHang ORDER BY cthd.ThoiGian DESC) AS RN 						
								from
								(
										--- getcthdlienquan ---
									select 
											ct.ID_LoHang ,
											ct.ID_DonViQuiDoi,
    										IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
											ct.TonLuyKe_NhanChuyenHang, ct.TonLuyKe) AS TonLuyKe,
    										IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
    										ct.GiaVon_NhanChuyenHang, ct.GiaVon) AS GiaVon,   									
    										IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh,
											hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian				
									from BH_HoaDon_ChiTiet ct				
									join BH_HoaDon hd on ct.ID_HoaDon= hd.ID				
									where hd.ChoThanhToan = 0 AND hd.LoaiHoaDon not in (3,19) ---3.dathang, 19.gdv, 30. PO
									--- chỉ lấy dvqd cần lấy ---
									and exists (select id from #tblQD qd where ct.ID_DonViQuiDoi= qd.ID)   
									and ((hd.ID_DonVi = @ID_ChiNhanh and hd.NgayLapHoaDon < @ToDate
    									and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    									or (hd.YeuCau = '4'  and hd.ID_CheckIn = @ID_ChiNhanh and hd.NgaySua < @ToDate))
								)cthd
								join #tblQD qdOut on cthd.ID_DonViQuiDoi = qdOut.ID ---- join để get full dvt ---
						)tblLast where RN= 1
    		
	
			select 
					qd.ID_HangHoa,
					qd.ID as ID_DonViQuiDoi,
					lo.ID as ID_LoHang, 
					qd.GiaNhap,
					isnull(gvtc.GiaVonTieuChuan,0) as GiaVonTieuChuan,
					isnull(tk.TonKho,0)/ iif(qd.TyLeChuyenDoi=0 or qd.TyLeChuyenDoi is null,1, qd.TyLeChuyenDoi) as TonKho,
					isnull(tk.GiaVon,0) * iif(qd.TyLeChuyenDoi=0 or qd.TyLeChuyenDoi is null,1, qd.TyLeChuyenDoi) as GiaVon
				from #tblQD qd 	
				left join DM_LoHang lo on qd.ID_HangHoa = lo.ID_HangHoa 
				left join @tblGVTieuChuan gvtc on qd.ID = gvtc.ID_DonViQuiDoi 
					and (lo.ID = gvtc.ID_LoHang or (gvtc.ID_LoHang is null and lo.ID is null) )
				left join #tblTon tk on qd.ID_HangHoa = tk.ID_HangHoa 
				order by qd.ID

			
		
		 end
		
END");
            Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoaXuatHuybyID]
	@ID_HoaDon [uniqueidentifier] ='a3fc6f98-2862-403e-8923-cd02d8c858de',
	@ID_ChiNhanh [uniqueidentifier] ='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
AS
BEGIN
  set nocount on;

		declare @countCTMua int, @ID_HoaDonGoc uniqueidentifier		
		select @ID_HoaDonGoc= ID_HoaDon from BH_HoaDon where ID= @ID_HoaDon


		----- get ctmgoc---
		select ctm.ID
		into #ctmua
		from BH_HoaDon hdm	
		join BH_HoaDon_ChiTiet ctm on hdm.ID = ctm.ID_HoaDon	
		where hdm.ID = @ID_HoaDonGoc
		and hdm.ChoThanhToan is not null---- nếu hd đã bị hủy --> phiếu xuất kho cũng phải dc hủy 

	
		declare @isHDLast bit ='0'
		---- get all PXK by idHDGoc ----
		select ID,NgayLapHoaDon,LoaiHoaDon into #tblPXK 
		from BH_HoaDon where ID_HoaDon = @ID_HoaDonGoc 
		---- get phieuXK last
		declare @ngaylapHDMax datetime = (select top 1 NgayLapHoaDon from #tblPXK order by NgayLapHoaDon desc)

			---- get infor of pXK current ----
		declare @ngaylapHDThis datetime, @LoaiHoaDonThis int
	    select top 1 @ngaylapHDThis = NgayLapHoaDon, @LoaiHoaDonThis = LoaiHoaDon from #tblPXK where ID = @ID_HoaDon

		----- get all ctxk lien quan ----
			declare @ctxkLienQuan table (ID_ChiTietGoiDV uniqueidentifier, CountCTGDV int)
				insert into @ctxkLienQuan
				select 
					ctxk.ID_ChiTietGoiDV,
					count(ctxk.ID_ChiTietGoiDV) as CountCTGDV
				from BH_HoaDon_ChiTiet ctxk
				where exists (select id from #ctmua ctmg where ctxk.ID_ChiTietGoiDV = ctmg.ID)				
				group by ctxk.ID_ChiTietGoiDV	
							
				if @ngaylapHDMax = @ngaylapHDThis
				  set @isHDLast ='1'
		
		select 	
			 ctxk.ID,		
			ctxk.ID_DonViQuiDoi,
			ctxk.ID_LoHang,
			ctxk.ID_ChiTietGoiDV,
			ctxk.ID_ChiTietDinhLuong,
			ctxk.SoLuong,
			ctxk.DonGia,
			ctxk.GiaVon,
			ctxk.ThanhTien,
			ctxk.ChatLieu,
			ctxk.SoThuTu,
			ctxk.TienChietKhau,		
			ctxk.ID_HoaDon,
			ctxk.ThanhTien as GiaTriHuy,
			ctxk.SoLuong as SoLuongXuatHuy,
			ctxk.TienChietKhau as GiamGia,
			ctxk.GhiChu,

			dvqd.ID_HangHoa,
			dvqd.MaHangHoa,
			dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			lh.MaLoHang,
			Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
			lh.NgaySanXuat,
    		lh.NgayHetHan,    			
    		hh.TenHangHoa,
			ROUND(ISNULL(tk.TonKho,0),2) as TonKho,
			Case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang, 
    		concat(hh.TenHangHoa , '', dvqd.ThuocTinhGiaTri) as TenHangHoaFull,	
			---- ctgdv > 1 và không phải hdlast
			---- hoac khong tontai ctgdv in ctmua --
			cast(iif(isnull(ctlq.CountCTGDV,0) > 1 and @isHDLast ='0' or (@LoaiHoaDonThis !=8 and isnull(ctlq.CountCTGDV,0) = 0),1,0)
			 as float) as TrangThaiMoPhieu
						
		from BH_HoaDon_ChiTiet ctxk		
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join @ctxkLienQuan ctlq on ctxk.ID_ChiTietGoiDV = ctlq.ID_ChiTietGoiDV
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi 
		and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		where ctxk.ID_HoaDon = @ID_HoaDon
		and (hh.LaHangHoa = 1 and tk.TonKho is not null) 
		and (ctxk.ChatLieu is null or ctxk.ChatLieu != '5') 	


END");
            Sql(@"ALTER PROCEDURE [dbo].[GetNhatKySuDung_GDV] 
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
    
    	set @where = N' where 1 = 1 and hd.LoaiHoaDon in (1,2,36) and hd.ChoThanhToan = 0  
						and (ct.ID_ChiTietDinhLuong= ct.id OR ct.ID_ChiTietDinhLuong IS NULL) 
						and (ct.ID_ParentCombo != ct.ID OR ct.ID_ParentCombo IS NULL)'
    
    	if isnull(@CurrentPage,'') =''
    		set @CurrentPage = 0
    	if isnull(@PageSize,'') =''
    		set @PageSize = 20

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
							or dt.MaDoiTuong like N''%'' + @TextSearch_In + ''%'' or  dt.TenDoiTuong like N''%'' + @TextSearch_In + ''%'' 
							or dt.TenDoiTuong_KhongDau like N''%'' + @TextSearch_In + ''%'' or dt.DienThoai like N''%'' + @TextSearch_In + ''%''
							or hh.TenHangHoa like N''%'' + @TextSearch_In + ''%'' or  hh.TenHangHoa_KhongDau like N''%'' + @TextSearch_In + ''%''
							or qd.MaHangHoa like N''%'' + @TextSearch_In + ''%'')' )
    			
    		end
    	
    	set @sql = CONCAT(@tblDefined, @sql, N'
    		;with data_cte
    as (
		SELECT 
			ct.SoThuTu, --- ưu tiên sắp xếp Nhóm hỗ trợ trên cùng ---
			ct.ID as ID_ChiTietGoiDV, ---- để get nvth --
			hd.MaHoaDon, 
			hd.NgayLapHoaDon,			
    		qd.MaHangHoa,
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='''', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoa,
    		hh.TenHangHoa_KhongDau,		---- get để where at js --	
    		ct.SoLuong as SoLuongMua,			
			iif(hd.LoaiHoaDon = 36,0,ct.DonGia - ct.TienChietKhau) as GiaBan,  ---- lay sau CK			
			ct.GhiChu			
    	FROM BH_HoaDon_ChiTiet ct
    	join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.id
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID			
		', @where, 

		N'union all
		---- get nhom ap dung ho tro

		select 
			0 as STT,
			newid() as ID_ChiTietGoiDV,
			hd.MaHoaDon, 
			hd.NgayLapHoaDon,	
			N''Ngày thuốc'' as MaHangHoa,
			N''Ngày thuốc'' as TenHangHoa,
			N''Ngay thuoc'' as TenHangHoa_KhongDau,
			hd.TongGiamGia as SoLuongMua,  ---- số ngày thuốc
			0 as GiaBan,
			hd.DienGiai as GhiChu
		from BH_HoaDon hd
		join DM_NhomHangHoa nhomhh on hd.ID_CheckIn = nhomhh.ID
		where hd.LoaiHoaDon = 36
		and hd.CHoThanhToan = 0  
		and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)
		----and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    	 ),
    		count_cte
    		as (
    			select count(ID_ChiTietGoiDV) as TotalRow,
    				CEILING(COUNT(ID_ChiTietGoiDV) / CAST(@PageSize_In as float ))  as TotalPage,
    				sum(SoLuongMua) as TongSoLuong		
    			from data_cte
    		)
			Select dt.*,
				cte.*,
				hdXMLOut.HDCT_NhanVien as NhanVienThucHien
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
    			order by dt.NgayLapHoaDon desc, dt.SoThuTu
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
            Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountInvoice]
    @ID_ChiNhanhs [nvarchar](max)='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @ID_NhanVienLogin [nvarchar](max) ='1cfe08ef-d1fe-4822-9511-910b08b14623',
	@DepartmentIDs [nvarchar](max)='',
    @TextSearch [nvarchar](max)='',
	@LoaiChungTus [nvarchar](max)='0,1,2,19,22',
    @DateFrom [nvarchar](max)='2024-07-01',
    @DateTo [nvarchar](max)='2024-07-31',
    @Status_ColumHide [int] = 8,
    @StatusInvoice [int] = 1,
    @Status_DoanhThu [int] = 0,
    @CurrentPage [int] = 0,
    @PageSize [int] = 10
AS
BEGIN
    SET NOCOUNT ON;
    	set @DateTo = dateadd(day,1, @DateTo) 

		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');

		DECLARE @tblDepartment TABLE (ID_PhongBan uniqueidentifier)
		if isnull(@DepartmentIDs,'') =''
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

    
		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @tblDiscountInvoice table (ID uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), 
    		HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float)
    	
    	-- bang tam chua DS phieu thu theo Ngay timkiem
    	select qct.ID_HoaDonLienQuan, SUM(qct.TienThu) as ThucThu, qhd.NgayLapHoaDon, qhd.ID
    	into #temp
    	from Quy_HoaDon_ChiTiet qct
    	join (
    			select qhd.ID, qhd.NgayLapHoaDon
    			from Quy_HoaDon qhd
    			join BH_NhanVienThucHien th on qhd.ID= th.ID_QuyHoaDon
    			where (qhd.TrangThai is null or qhd.TrangThai = '1')
    			and qhd.ID_DonVi in (select ID from @tblChiNhanh)
    			and qhd.NgayLapHoaDon >= @DateFrom
    			and qhd.NgayLapHoaDon < @DateTo
    			group by qhd.ID, qhd.NgayLapHoaDon) qhd on qct.ID_HoaDon = qhd.ID
    	where (qct.HinhThucThanhToan is null or qct.HinhThucThanhToan != 4)
    	group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID;
    	
    	select ID, MaNhanVien, 
    			TenNhanVien,
    		SUM(ISNULL(HoaHongThucThu,0.0)) as HoaHongThucThu,
    		SUM(ISNULL(HoaHongDoanhThu,0.0)) as HoaHongDoanhThu,
    		SUM(ISNULL(HoaHongVND,0.0)) as HoaHongVND,			
    		case @Status_ColumHide
    			when  1 then cast(0 as float)
    			when  2 then SUM(ISNULL(HoaHongVND,0.0))
    			when  3 then SUM(ISNULL(HoaHongThucThu,0.0))
    			when  4 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    			when  5 then SUM(ISNULL(HoaHongDoanhThu,0.0)) 
    			when  6 then SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    			when  7 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0))
    		else SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    		end as TongAll
    		into #temp2
    	from 
    	(
    		select nv.ID, MaNhanVien, TenNhanVien,
    			case when TinhChietKhauTheo =1 then case when hd.LoaiHoaDon in (6,32) then -TienChietKhau else TienChietKhau  end end as HoaHongThucThu,
    				case when TinhChietKhauTheo =3 then case when hd.LoaiHoaDon in (6,32) then -TienChietKhau else TienChietKhau end end as HoaHongVND,
    				-- neu HD tao thang truoc, nhung PhieuThu thuoc thang nay: HoaHongDoanhThu = 0
    				case when hd.NgayLapHoaDon between @DateFrom and @DateTo and hd.ID_DonVi in (select ID from @tblChiNhanh) then
    					case when TinhChietKhauTheo = 2 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end else 0 end as HoaHongDoanhThu,
    				-- timkiem theo NgayLapHD or NgayLapPhieuThu
    				case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then hd.NgayLapHoaDon else tblQuy.NgayLapHoaDon end as NgayLapHoaDon,
    				case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    		from BH_NhanVienThucHien th
    		join BH_HoaDon hd on th.ID_HoaDon= hd.ID
    		join NS_NhanVien nv on th.ID_NhanVien= nv.ID
    			left join #temp tblQuy on hd.ID= tblQuy.ID_HoaDonLienQuan and (th.ID_QuyHoaDon= tblQuy.ID)	
    		where th.ID_HoaDon is not null
    		and hd.ChoThanhToan  is not null
    		and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))  
				and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
				and  exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID)
    			-- chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
    			and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 and ( exists (select ID from #temp where th.ID_QuyHoaDon = #temp.ID) or  LoaiHoaDon=6)))
    			and
    				((select count(Name) from @tblSearchString b where     			
    				nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'				
    				)=@count or @count=0)	
    	) tbl
    		where tbl.NgayLapHoaDon >= @DateFrom and tbl.NgayLapHoaDon < @DateTo and TrangThaiHD = @StatusInvoice
    	group by MaNhanVien, TenNhanVien, ID
    		having SUM(ISNULL(HoaHongThucThu,0)) + SUM(ISNULL(HoaHongDoanhThu,0)) + SUM(ISNULL(HoaHongVND,0)) > 0 -- chi lay NV co CK > 0
    		
    		if @Status_DoanhThu =0
    			insert into @tblDiscountInvoice
    			select *
    			from #temp2 
    		else
    			begin
    				if @Status_DoanhThu= 1
    					insert into @tblDiscountInvoice
    					select *
    					from #temp2 where HoaHongDoanhThu > 0 or HoaHongThucThu != 0
    				else
    					if @Status_DoanhThu= 2
    						insert into @tblDiscountInvoice
    						select *
    						from #temp2 where HoaHongDoanhThu > 0 or HoaHongVND != 0
    					else		
    						if @Status_DoanhThu= 3
    							insert into @tblDiscountInvoice
    							select *
    							from #temp2 where HoaHongDoanhThu > 0
    						else	
    							if @Status_DoanhThu= 4
    								insert into @tblDiscountInvoice
    								select *
    								from #temp2 where HoaHongVND > 0 Or HoaHongThucThu != 0
    							else
    								if @Status_DoanhThu= 5
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2 where  HoaHongThucThu != 0
    								else -- 6
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2  where HoaHongVND > 0
    								
    			end;
    			
    		with data_cte
    		as(
    		select * from @tblDiscountInvoice
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
    				sum(HoaHongThucThu) as TongHoaHongThucThu,
    				sum(HoaHongVND) as TongHoaHongVND,
    				sum(TongAll) as TongAllAll
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaNhanVien
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY


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
    	TienMat float, TienGui float, TienThu float, TienChi float, ThuTienMat float, ChiTienMat float, ThuTienGui float, ThuTienPOS FLOAT,
		ChiTienGui float, ChiTienPOS FLOAT, TonLuyKeTienMat float,TonLuyKeTienGui float,TonLuyKe float, SoTaiKhoan nvarchar(max), NganHang nvarchar(max), GhiChu nvarchar(max),
    		IDDonVi UNIQUEIDENTIFIER, TenDonVi NVARCHAR(MAX), RN INT);
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
				SUM (b.ThuTienPOS) as ThuTienPOS,
    			SUM (b.ChiTienGui) as ChiTienGui, 
				SUM (b.ChiTienPOS) as ChiTienPOS, 
    				0 as TonLuyKe,
    			0 as TonLuyKeTienMat,
    			0 as TonLuyKeTienGui,
    			MAX(b.SoTaiKhoan) as SoTaiKhoan,
    			MAX(b.NganHang) as NganHang,
    			MAX(b.GhiChu) as GhiChu,
    				dv.ID,
    				dv.TenDonVi,
					ROW_NUMBER() OVER (ORDER BY b.NgayLapHoaDon) AS RN
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
				IIF(a.LoaiHoaDon = 11, IIF(a.TaiKhoanPOS = 1, 0, a.TienGui) , 0) AS ThuTienGui,
				IIF(a.LoaiHoaDon = 11, IIF(a.TaiKhoanPOS = 1, a.TienGui, 0) , 0) AS ThuTienPOS,
    			--case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
				IIF(a.LoaiHoaDon = 12, IIF(a.TaiKhoanPOS = 1, 0, a.TienGui) , 0) AS ChiTienGui,
				IIF(a.LoaiHoaDon = 12, IIF(a.TaiKhoanPOS = 1, a.TienGui, 0) , 0) AS ChiTienPOS,
    			--Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
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
    			tknh.SoTaiKhoan as SoTaiKhoan,
    			nh.TenNganHang as NganHang,
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
    				qhd.ID_DonVi,
					tknh.TaiKhoanPOS
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on tknh.ID_NganHang = nh.ID
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
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, qhd.ID_DonVi, qhdct.ID, qhdct.HinhThucThanhToan,
				 tknh.TaiKhoanPOS, tknh.SoTaiKhoan, nh.TenNganHang
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    			inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    			where LoaiTien like @LoaiTien
    		Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaPhieuThu, b.NgayLapHoaDon, dv.TenDonVi, dv.ID, b.SoTaiKhoan
    		ORDER BY NgayLapHoaDon

			--SELECT * FROM @tmp
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
				DECLARE @ThuTienPOS float;
				DECLARE @ChiTienPOS float;
    				DECLARE @ID_HoaDon UNIQUEIDENTIFIER;
    	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT TienThu, TienChi, ThuTienGui, ThuTienMat, ChiTienGui, ChiTienMat, ID_HoaDon, ThuTienPOS, ChiTienPOS FROM @tmp ORDER BY RN
    	OPEN CS_ItemUpDate 
    FETCH FIRST FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon, @ThuTienPOS, @ChiTienPOS
    WHILE @@FETCH_STATUS = 0
    BEGIN
    	SET @Ton = @Ton + @ThuTienMat + @ThuTienGui + @ThuTienPOS - @ChiTienMat - @ChiTienGui - @ChiTienPOS;
    	SET @TonTienMat = @TonTienMat + @ThuTienMat - @ChiTienMat;
    	SET @TonTienGui = @TonTienGui + @ThuTienGui - @ChiTienGui + @ThuTienPOS - @ChiTienPOS;
    	UPDATE @tmp SET TonLuyKe = @Ton, TonLuyKeTienMat = @TonTienMat, TonLuyKeTienGui = @TonTienGui WHERE ID_HoaDon = @ID_HoaDon
		AND ThuTienMat = @ThuTienMat AND ThuTienGui = @ThuTienGui AND ThuTienPOS = @ThuTienPOS
		AND ChiTienMat = @ChiTienMat AND ChiTienGui = @ChiTienGui AND ChiTienPOS = @ChiTienPOS
    	FETCH NEXT FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon, @ThuTienPOS, @ChiTienPOS
    END
    CLOSE CS_ItemUpDate
    DEALLOCATE CS_ItemUpDate
    	END
    	ELSE
    	BEGIN
    		Insert INTO @tmp
    	SELECT '00000000-0000-0000-0000-000000000000', 'TRINH0001', '1989-04-07','','','0','0','0','0','0','0','0','0', '0', '0', @TonDauKy, @TonDauKy, @TonDauKy, '','','', '00000000-0000-0000-0000-000000000000', '', 0
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
		ThuTienPOS,
    	ChiTienGui,
		ChiTienPOS,
    	TonLuyKe,
    	TonLuyKeTienMat,
    	TonLuyKeTienGui,
    	SoTaiKhoan, 
    	NganHang, 
    	GhiChu,
    		IDDonVi, TenDonVi
    	 from @tmp order by RN DESC
END");
            Sql(@"ALTER PROCEDURE [dbo].[CheckThucThu_TongSuDung]
   @ID_DoiTuong [uniqueidentifier] ='00089B15-9262-48CB-A978-6D1AB52029F0',
    @ID_TheGiaTri [uniqueidentifier] ='6B9B7FFE-3202-4CB8-B4F1-F365D19AB65D'
AS
BEGIN
    SET NOCOUNT ON;
        	
    	declare @dateHD datetime, @trangThaiTGT bit='0'
		declare @return bit='1'
    
    	select top 1 @dateHD=  NgayLapHoaDon, @trangThaiTGT= ChoThanhToan from  BH_HoaDon where ID = @ID_TheGiaTri

		---select @dateHD, iif('2024-06-29 14:55:57.000' < '2024-06-29 14:56:55.003' 
		if @trangThaiTGT is not null
			begin
						select 
							*,
							ROW_NUMBER() over (order by NgayLapHoaDon) as RN,
							sum(iif(LoaiHoaDon in (11,32), - DaThanhToan, DaThanhToan)) over (order by NgayLapHoaDon) as SoDuLuyKe
						into #tblLuyKe
						from
						(
						----- get all TGT và TongThu tương ứng ---
						select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.LoaiHoaDon, 
							isnull(sq.DaThanhToan,0) as DaThanhToan
						from BH_HoaDon hd
						left join
						(
						 ----- thu tien napthe ---
							select qct.ID_HoaDonLienQuan, 
								sum (qct.TienThu) as DaThanhToan
						    from Quy_HoaDon qhd
							join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
							where qhd.TrangThai ='1'
							and qct.ID_DoiTuong = @ID_DoiTuong
							group by qct.ID_HoaDonLienQuan
						)sq on hd.ID = sq.ID_HoaDonLienQuan
						where hd.ChoThanhToan ='0'
						and hd.LoaiHoaDon = 22
						and hd.ID_DoiTuong = @ID_DoiTuong 
						

						union all

						---- get giatri hoantra ---
						select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.LoaiHoaDon, 
							hd.TongTienHang
						from BH_HoaDon hd
						where hd.LoaiHoaDon = 32
						and hd.ChoThanhToan='0'
						and hd.ID_DoiTuong = @ID_DoiTuong
											

						union all

						---- get hd dieuchinh ---
						select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.LoaiHoaDon, 
							hd.TongTienHang
						from BH_HoaDon hd
						where hd.LoaiHoaDon = 23
						and hd.ChoThanhToan='0'
						and hd.ID_DoiTuong = @ID_DoiTuong
						

						union all
						-- get all tongsudung
    						select qhd.ID,
								qhd.NgayLapHoaDon,
								qhd.MaHoaDon,
								qhd.LoaiHoaDon,
    							sum(qct.TienThu) as DaSuDung
    						from Quy_HoaDon qhd
    						join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    						where qct.ID_DoiTuong= @ID_DoiTuong    	
    						and qhd.TrangThai='1'
    						and qhd.LoaiHoaDon = 11
    						and qct.HinhThucThanhToan = 4
							group by qhd.ID,
								qhd.NgayLapHoaDon,
								qhd.MaHoaDon, qhd.LoaiHoaDon
						)tblunion										


						declare @ngayNapTheNext datetime, @soduLuyKe float, @tongthuThisTGT float

						--- get ngayNapThe lần tiếp theo ---
						select top 1 @ngayNapTheNext = NgayLapHoaDon
						from #tblLuyKe 
						where NgayLapHoaDon > @dateHD
						and LoaiHoaDon = 22
						order by RN

						---- get phieusudung/ hoantra cuối cùng (sau khi nạp thẻ this) ----
						select top 1 @soduLuyKe = SoDuLuyKe
						from #tblLuyKe 
						where LoaiHoaDon in (11,32) 
						and NgayLapHoaDon > @dateHD and (@ngayNapTheNext is null or NgayLapHoaDon < @ngayNapTheNext)
						order by RN desc


						--- nếu số dư còn lại < Tổng thu của Thẻ: không được hủy thẻ --
						select top 1  @tongthuThisTGT = DaThanhToan from #tblLuyKe where ID= @ID_TheGiaTri

						if @soduLuyKe < @tongthuThisTGT 
							set @return = '0'

						--select * from #tblLuyKe

						--drop table #tblLuyKe
										
			end
			
		
    	select @return as Exist
		
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
			and (ct.ChatLieu is null or ct.ChatLieu !='4') --- không trả hd sử dụng GDV
    		
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
        }
        
        public override void Down()
        {
            Sql(@"DROP PROCEDURE IF EXISTS [dbo].[GetChiTiet_NhatKyGiaoDich_UsedBaoHanh]");
        }
    }
}
