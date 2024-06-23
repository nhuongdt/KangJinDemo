namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddUpdateStoreProcedure20240614 : DbMigration
    {
        public override void Up()
        {
            Sql(@"DROP PROCEDURE IF EXISTS [dbo].[GetMaDoiTuongMax_byTemp]");
            Sql(@"DROP PROCEDURE IF EXISTS [dbo].[GetGiaTriHoTro_ofCustomer]");
            Sql(@"CREATE PROCEDURE [dbo].[GetMaDoiTuongMax_byTemp]
	@LoaiHoaDon int,
	@ID_DonVi uniqueidentifier
AS
BEGIN	
	SET NOCOUNT ON;

	declare @LoaiDoiTuong int
	if @LoaiHoaDon = 33 set @LoaiDoiTuong = 1
	if @LoaiHoaDon = 34 set @LoaiDoiTuong = 2


	DECLARE @Return float = 1
	declare @lenMaMax int = 0
	DECLARE @isDefault bit = (select top 1 SuDungMaChungTu from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi)-- co/khong thiet lap su dung Ma MacDinh
	DECLARE @isSetup int = (select top 1 ID_LoaiChungTu from HT_MaChungTu where ID_LoaiChungTu = @LoaiHoaDon)-- da ton tai trong bang thiet lap chua

	if @isDefault='1' and @isSetup is not null
		begin
			DECLARE @machinhanh varchar(15) = (select MaDonVi from DM_DonVi where ID= @ID_DonVi)
			DECLARE @lenMaCN int = Len(@machinhanh)

			DECLARE @isUseMaChiNhanh varchar(15), @kituphancach1 varchar(1),  @kituphancach2 varchar(1),  @kituphancach3 varchar(1),
			 @dinhdangngay varchar(8), @dodaiSTT INT, @kihieuchungtu varchar(10)

			 select @isUseMaChiNhanh = SuDungMaDonVi, 
				@kituphancach1= KiTuNganCach1,
				@kituphancach2 = KiTuNganCach2,
				@kituphancach3= KiTuNganCach3,
				@dinhdangngay = NgayThangNam,
				@dodaiSTT = CAST(DoDaiSTT AS INT),
				@kihieuchungtu = MaLoaiChungTu
			 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon 

		
			DECLARE @lenMaKiHieu int = Len(@kihieuchungtu);
			DECLARE @namthangngay varchar(10) = convert(varchar(10), getDate(), 112) ---- get ngayhientai
			DECLARE @year varchar(4) = Left(@namthangngay,4)
			DECLARE @date varchar(4) = right(@namthangngay,2)
			DECLARE @month varchar(4) = substring(@namthangngay,5,2)
			DECLARE @datecompare varchar(10)='';
			
			if	@isUseMaChiNhanh='0'
				begin 
					set @machinhanh=''
					set @lenMaCN=0
				end

			if @dinhdangngay='ddMMyyyy'
				set @datecompare = CONCAT(@date,@month,@year)
			else	
				if @dinhdangngay='ddMMyy'
					set @datecompare = CONCAT(@date,@month,right(@year,2))
				else 
					if @dinhdangngay='MMyyyy'
						set @datecompare = CONCAT(@month,@year)
					else	
						if @dinhdangngay='MMyy'
							set @datecompare = CONCAT(@month,right(@year,2))
						else
							if @dinhdangngay='yyyyMMdd'
								set @datecompare = CONCAT(@year,@month,@date)
							else 
								if @dinhdangngay='yyMMdd'
									set @datecompare = CONCAT(right(@year,2),@month,@date)
								else	
									if @dinhdangngay='yyyyMM'
										set @datecompare = CONCAT(@year,@month)
									else	
										if @dinhdangngay='yyMM'
											set @datecompare = CONCAT(right(@year,2),@month)
										else 
											if @dinhdangngay='yyyy'
												set @datecompare = @year	
												
			if @LoaiDoiTuong= 1 set @kihieuchungtu ='' --- không lấy kí tự theo mã khách (chỉ lấy mã chi nhánh) !!important with kangjjin

			DECLARE @sMaFull varchar(50) = concat(@machinhanh,@kituphancach1,@kihieuchungtu,@kituphancach2, @datecompare, @kituphancach3)	

			declare @sCompare varchar(30) = @sMaFull
			if @sMaFull= concat(@kihieuchungtu,'_') set @sCompare = concat(@kihieuchungtu,'[_]') -- like %_% không nhận kí tự _ nên phải [_] theo quy tắc của sql				


			SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaDoiTuong) AS float))
    		FROM DM_DoiTuong 
			WHERE LoaiDoiTuong = @LoaiDoiTuong 
			and MaDoiTuong like @sCompare +'%'  AND MaDoiTuong not like  @sCompare +'00%' -- not like 'KH00%'


			-- lay chuoi 000
			declare @stt int =0;
			declare @strstt varchar (10) ='0'
			while @stt < @dodaiSTT- 1
				begin
					set @strstt= CONCAT('0',@strstt)
					SET @stt = @stt +1;
				end 
			declare @lenSst int = len(@strstt)
			if	@Return is null 
					select CONCAT(@sMaFull,left(@strstt,@lenSst-1),1) as MaxCode-- left(@strstt,@lenSst-1): bỏ bớt 1 số 0			
			else 
				begin
					set @Return =  @Return + 1
					set @lenMaMax =  len(@Return)

					-- neu @Return là 1 số quá lớn --> mã bị chuyển về dạng e+10
					declare @madai nvarchar(max)= CONCAT(@sMaFull, CONVERT(numeric(22,0), @Return))
					select 
						case @lenMaMax							
							when 1 then CONCAT(@sMaFull,left(@strstt,@lenSst-1),@Return)
							when 2 then case when @lenSst - 2 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-2), @Return) else @madai end
							when 3 then case when @lenSst - 3 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-3), @Return) else @madai end
							when 4 then case when @lenSst - 4 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-4), @Return) else @madai end
							when 5 then case when @lenSst - 5 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-5), @Return) else @madai end
							when 6 then case when @lenSst - 6 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-6), @Return) else @madai end
							when 7 then case when @lenSst - 7 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-7), @Return) else @madai end
							when 8 then case when @lenSst - 8 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-8), @Return) else @madai end
							when 9 then case when @lenSst - 9 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-9), @Return) else @madai end
							when 10 then case when @lenSst - 10 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return) else @madai end
						else 
							case when  @lenMaMax > 10
								 then iif(@lenSst - 10 > -1, CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return),  @madai)
								 else '' end
						end as MaxCode					
				end 
		end
	else
		begin
			declare @machungtu varchar(10) = (select top 1 MaLoaiChungTu from DM_LoaiChungTu where ID= @LoaiHoaDon)			
			declare @lenMaChungTu int= LEN(@machungtu)
			
			
				declare @maOffline nvarchar(max) =''
				if @LoaiDoiTuong= 1 set @maOffline='KHO'
				if @LoaiDoiTuong= 2 set @maOffline='NCCO'				
				
				SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaDoiTuong) AS float))
    			FROM DM_DoiTuong 
				WHERE LoaiDoiTuong = @LoaiDoiTuong 
				and MaDoiTuong like @machungtu +'%'  AND MaDoiTuong not like @maOffline + '%'	

				
			-- do dai STT (toida = 9)
			if	@Return is null set @Return = 1				
			else set @Return = @Return + 1
																							
				set @lenMaMax = len(@Return)
				declare @max int =0
				declare @str0 nvarchar(10) =''
				while @max < 9 - (@lenMaMax + @lenMaChungTu)
					begin
						set @str0+='0'
						set @max+=1
					end					
				select CONCAT(@machungtu,@str0, CAST(@Return  as decimal(22,0)))  as MaxCode
		end
		
END");
            Sql(@"Create PROCEDURE [dbo].[GetGiaTriHoTro_ofCustomer] 
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
    						select sd.ID_DoiTuong,
    							sd.ID_DonVi,		
    							sd.ID_NhomHoTro,			
    							sd.GiaTriSuDung,
    							isnull(ht.DaHoTro,0) as DaHoTro
    						from (
    							select ID_DoiTuong,
    								ID_DonVi,			
    								ID_NhomHoTro,
    								sum(GiaTriSuDung) as GiaTriSuDung
    							from #tblSuDung
    							group by ID_DoiTuong, ID_DonVi,ID_NhomHoTro
    						)  sd
    						left join  (
    							select ID_DoiTuong,		
    									ID_DonVi,				
    									ID_NhomHoTro,
    								sum(GiaTriDichVu) as DaHoTro
    							from #tblHoTro
    							group by ID_DoiTuong,ID_NhomHoTro,ID_DonVi	 
    						)ht on sd.ID_DoiTuong= ht.ID_DoiTuong and sd.ID_NhomHoTro = ht.ID_NhomHoTro and sd.ID_DonVi = ht.ID_DonVi
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
    		--ISNULL(nvpt.MaNhanVien,'''') as MaNVPhuTrach,
    		ISNULL(nvpt.NVPhuTrachs,'''') as TenNhanVienPhuTrach
    	from tView dt
    	left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    	LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    	LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
    	LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
    	--LEFT join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID
    	LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    	LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID
		left join (
			select distinct 
				nvptOut.ID_KhachHang,		
					(select nv.TenNhanVien +  '','' AS [text()]
					from tView kh
					join KH_NVPhuTrach nvpt on kh.ID = nvpt.ID_KhachHang
					join NS_NhanVien nv on nvpt.ID_NhanVienPhuTrach = nv.ID
					where nvpt.ID_KhachHang = nvptOut.ID_KhachHang
					--and exists (select ID from tView kh where nvpt.ID_KhachHang = kh.ID)
					For XML PATH ('''')
					) NVPhuTrachs
				from KH_NVPhuTrach nvptOut
			 )nvpt on dt.ID = nvpt.ID_KhachHang 
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
END");
            Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoaXuatHuybyID]
	@ID_HoaDon [uniqueidentifier] ='C9FDCF44-F405-4376-A49B-CE259D951063',
	@ID_ChiNhanh [uniqueidentifier] ='d93b17ea-89b9-4ecf-b242-d03b8cde71de'
AS
BEGIN
  set nocount on;

		declare @countCTMua int, @ID_HoaDonGoc uniqueidentifier		
		select @ID_HoaDonGoc= ID_HoaDon from BH_HoaDon where ID= @ID_HoaDon


		----- get all ctm goc (ke ca dv) ---
		---- vi se co truong hop capnhat tpdl (co --> null)
		select ctm.ID, 
			ctm.ID_HoaDon,
			ctm.ChatLieu, -- chatlieu = 5: huy
			hdm.ChoThanhToan,
			hdm.MaHoaDon
		into #ctmua
		from
		(
			select hdg.ID, hdg.ChoThanhToan, hdg.MaHoaDon
			from BH_HoaDon hdg
			where hdg.ID = @ID_HoaDonGoc
		) hdm 
		join BH_HoaDon_ChiTiet ctm on hdm.ID = ctm.ID_HoaDon	

	
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
			tkgv.*,
			qd.ID_HangHoa,
			qd.GiaNhap,
			isnull(gvtc.GiaVonTieuChuan,0) as GiaVonTieuChuan
		from
		(
			select 
				tk.ID_DonViQuyDoi as ID_DonViQuiDoi, 
				tk.ID_LoHang,
				tk.TonKho,
				gv.GiaVon
			from DM_HangHoa_TonKho tk
			join DM_GiaVon gv on tk.ID_DonViQuyDoi= gv.ID_DonViQuiDoi and tk.ID_DonVi = gv.ID_DonVi	
			where tk.ID_DonVi = @ID_ChiNhanh
			and exists (select qd.ID_DonViQuyDoi from @tblIDQuiDoi qd where tk.ID_DonViQuyDoi = qd.ID_DonViQuyDoi)
		) tkgv
		join DonViQuiDoi qd on tkgv.ID_DonViQuiDoi = qd.ID
		left join @tblGVTieuChuan gvtc on tkgv.ID_DonViQuiDoi = gvtc.ID_DonViQuiDoi 
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

            Sql(@"ALTER PROCEDURE [dbo].[UpdateChiTietKiemKe_WhenEditCTHD]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanhInput [uniqueidentifier],
    @NgayLapHDMin [datetime]
AS
BEGIN
    SET NOCOUNT ON;
  
		 ------- get all donviquydoi lienquan ---
			declare @tblQuyDoi table (ID_DonViQuiDoi uniqueidentifier, ID_HangHoa uniqueidentifier, 
				ID_LoHang uniqueidentifier, 
				TyLeChuyenDoi float,
				LaHangHoa bit)
			insert into @tblQuyDoi
			select * from dbo.fnGetAllHangHoa_NeedUpdateTonKhoGiaVon(@IDHoaDonInput)

			------ get all ctKiemKe need update ---
			DECLARE @cthdNeed TABLE (ID_HoaDon UNIQUEIDENTIFIER, ID_ChiTietHoaDon UNIQUEIDENTIFIER, 
			NgayLapHoaDon datetime,SoLuong float, ID_HangHoa UNIQUEIDENTIFIER,
			ID_LoHang UNIQUEIDENTIFIER, ID_DonViQuiDoi UNIQUEIDENTIFIER, TonDauKy float, TyLeChuyenDoi float, TienChietKhau float)
			insert into @cthdNeed
			select 
				hd.ID as ID_HoaDon,
				ct.ID as ID_ChiTietHoaDon,
				hd.NgayLapHoaDon,
				ct.SoLuong,
				qd.ID_HangHoa,
				ct.ID_LoHang,
				ct.ID_DonViQuiDoi,
				0 as TonDauKy,
				qd.TyLeChuyenDoi,
				ct.TienChietKhau
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
								from @cthdNeed ctNeed 
								where ctNeed.ID_HangHoa = qd.ID_HangHoa 
								and (ctNeed.ID_LoHang = ct.ID_LoHang or (ctNeed.ID_LoHang is null and ct.ID_LoHang is null))
								------ chỉ lấy những hóa đơn có ngày lập < ngày kiểm kê (có thể có nhiều khoảng ngày kiểm kê )---
								AND ((hd.ID_DonVi = @IDChiNhanhInput and hd.NgayLapHoaDon <  ctNeed.NgayLapHoaDon and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    							or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanhInput and  hd.NgaySua < ctNeed.NgayLapHoaDon ))
								)
					)cthdLienQuan
		
			
			

			update ctNeed set ctNeed.TonDauKy = cthd.TonDauKy
				from @cthdNeed ctNeed
				join
				(
					select 
						cthdIn.ID_ChiTietHoaDon,
						cthdIn.TonDauKy
					from
					(
					select ctNeed.ID_ChiTietHoaDon, 
						ctNeed.ID_LoHang,
						ctNeed.ID_HangHoa,
						ctNeed.NgayLapHoaDon,
									
						isnull(tkDK.TonLuyKe,0) as TonDauKy,
						----- Lấy tồn đầu kỳ của từng chi tiết hóa đơn, ưu tiên sắp xếp theo tkDK.NgayLapHoaDon gần nhất (max) ---
						----- vì có thể có nhiều hd < ngaylaphoadon of ctNeed ----
						ROW_NUMBER() over (partition by ctNeed.ID_ChiTietHoaDon order by tkDK.NgayLapHoaDon desc) as RN
					from @cthdNeed ctNeed
					left join #cthdLienQuan tkDK on ctNeed.ID_HangHoa = tkDK.ID_HangHoa 
						and (ctNeed.ID_LoHang = tkDK.ID_LoHang or (ctNeed.ID_LoHang is null and tkDK.ID_LoHang is null)) 
						and tkDK.NgayLapHoaDon < ctNeed.NgayLapHoaDon
					)cthdIn
					where rn = 1
				)cthd on cthd.ID_ChiTietHoaDon = ctNeed.ID_ChiTietHoaDon
					


			---------- update TonkhoDB, SoLuongLech, GiaTriLech to BH_HoaDon_ChiTiet----
			update ctkiemke
			set	ctkiemke.TienChietKhau = ctLast.TonDauKy, 
    			ctkiemke.SoLuong = ctkiemke.ThanhTien - ctLast.TonDauKy, ---- soluonglech
    			ctkiemke.ThanhToan = ctkiemke.GiaVon * (ctkiemke.ThanhTien - ctLast.TonDauKy) --- gtrilech = soluonglech * giavon		
			from BH_HoaDon_ChiTiet ctkiemke
			join  
			(
				select cthd.ID_ChiTietHoaDon, 
					----- phai quydoi TonKho theo dvt ---
					cthd.TonDauKy/cthd.TyLeChuyenDoi as TonDauKy, 
					cthd.ID_HangHoa, cthd.TyLeChuyenDoi
				from @cthdNeed cthd	
				where ROUND(cthd.TonDauKy/ cthd.TyLeChuyenDoi, 3) !=  ROUND(cthd.TienChietKhau, 3) 
			) ctLast on ctkiemke.ID =  ctLast.ID_ChiTietHoaDon


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
				where exists (select ctNeed.ID_HoaDon from @cthdNeed ctNeed where ctNeed.ID_ChiTietHoaDon = ct.ID)
				group by ct.ID_HoaDon
			)ctKK on hdKK.ID = ctKK.ID_HoaDon		
			
	
			drop table #cthdLienQuan


END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhomHoTro]
    @IDChiNhanhs [nvarchar](max),
    @DateFrom [datetime],
    @DateTo [datetime],
    @IDNhomHoTros [nvarchar](max),
    @TextSearch [nvarchar](max),
	@IsVuotMuc tinyint, --1.all, 10. khong vuot, 11.vuot
    @CurrentPage [int],
    @PageSize [int]
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
    					join (
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
    						select sd.ID_DoiTuong,
    							sd.ID_DonVi,		
    							sd.ID_NhomHoTro,			
    							sd.GiaTriSuDung,
    							isnull(ht.DaHoTro,0) as DaHoTro
    						from (
    							select ID_DoiTuong,
    								ID_DonVi,			
    								ID_NhomHoTro,
    								sum(GiaTriSuDung) as GiaTriSuDung
    							from #tblSuDung
    							group by ID_DoiTuong, ID_DonVi, ID_NhomHoTro
    						)  sd
    						left join  (
    							select ID_DoiTuong,		
    									ID_DonVi,				
    									ID_NhomHoTro,
    								sum(GiaTriDichVu) as DaHoTro
    							from #tblHoTro
    							group by ID_DoiTuong,ID_NhomHoTro,ID_DonVi	 
    						)ht on sd.ID_DoiTuong= ht.ID_DoiTuong and sd.ID_NhomHoTro = ht.ID_NhomHoTro and sd.ID_DonVi = ht.ID_DonVi
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
				delete from NhomHangHoa_DonVi 
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
				
				delete from NhomHang_KhoangApDung
				delete from NhomHang_ChiTietSanPhamHoTro
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
        }

        public override void Down()
        {
            Sql(@"DROP PROCEDURE IF EXISTS [dbo].[GetMaDoiTuongMax_byTemp]");
            Sql(@"DROP PROCEDURE IF EXISTS [dbo].[GetGiaTriHoTro_ofCustomer]");
        }
    }
}
