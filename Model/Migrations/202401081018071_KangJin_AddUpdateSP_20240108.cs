namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KangJin_AddUpdateSP_20240108 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[fnDemSoLanDoiTra]
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
						+ isnull((select dbo.fnTinhSoLanDoiTra(ID_HoaDon)),0) as SoLan									
					from BH_HoaDon hd
					where ID = @ID
				)tbl
	
	RETURN @count

END
");

			CreateStoredProcedure(name: "[dbo].[BaoCaoGoiDichVu_BanDoiTra]", parametersAction: p => new {
				IDChiNhanhs = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				TxtMaHD = p.String(),
				TxtDVMua = p.String(),
				TxtDVDoi = p.String(),
				ThoiHanSuDung = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()

			}, body: @"SET NOCOUNT ON;

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
	drop table #cthdMuaGoc"
			);

			CreateStoredProcedure(name: "[dbo].[PhieuXuatKho_GetNVThucHien_byIDChiTietGDV]", parametersAction: p => new
			{
				IDChiTietGDVs = p.String()
			}, body: @"SET NOCOUNT ON;

	declare @tblID_ChiTietGDV table (ID uniqueidentifier primary key)
	declare @tblIDChiTietHD table (ID uniqueidentifier primary key, ID_ChiTietDinhLuong uniqueidentifier)

	if(isnull(@IDChiTietGDVs,'')!='')
		begin
		insert into @tblID_ChiTietGDV
		select name from dbo.splitstring(@IDChiTietGDVs)


		---- tu IDChiTietGDV --> tim ID cua BH_ChiTiet ---
		---- neu la HH: get ID ---
		---- neu la DV: get ID_ChiTietDinhLuong, sau do, tim den dichvu co ID = IDDinhLuong

		insert into @tblIDChiTietHD
		select distinct ID, ID_ChiTietDinhLuong 
		from BH_HoaDon_ChiTiet ct
		where  exists (select ID from @tblID_ChiTietGDV ctGDV where ctGDV.ID = ct.ID)

		select ctDB.ID 
		into #tblIDChiTiet
		from @tblIDChiTietHD cthd
		join BH_HoaDon_ChiTiet ctDB  on (ctDB.ID = cthd.ID and ctDB.ID_ChiTietDinhLuong is null)
				or (ctDB.ID_ChiTietDinhLuong is not null and ctDB.ID = cthd.ID_ChiTietDinhLuong)

			------	select * from #tblIDChiTiet

		select 
			th.ID_NhanVien,
			th.ID_ChiTietHoaDon,
			th.ThucHien_TuVan,
			th.TienChietKhau,
			th.PT_ChietKhau,
			th.TheoYeuCau,
			th.HeSo,
			th.TinhChietKhauTheo,
			ISNULL(th.TinhHoaHongTruocCK,0) as TinhHoaHongTruocCK,
			nv.TenNhanVien
		from BH_NhanVienThucHien th
		join NS_NhanVien nv on th.ID_NhanVien= nv.ID
		where exists (select ID from #tblIDChiTiet ct where th.ID_ChiTietHoaDon = ct.ID)

		end");

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


					------ update again TonLuyKe: only update HD da huy +  min(NgayLapHoaDon)  ---
					--BEGIN TRY  
					--	declare @Min_IDPhieuXuat uniqueidentifier, @Min_NgayLapPhieuXuat datetime

					--	select top 1 @Min_IDPhieuXuat = ID, @NgayLapHoaDon = NgayLapHoaDon 					
					--	from BH_HoaDon where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35
					--	and ChoThanhToan is null
					--	order by NgayLapHoaDon

					--	if @Min_IDPhieuXuat is not null
					--		exec dbo.UpdateTonLuyKeCTHD_whenUpdate @Min_IDPhieuXuat, @ID_DonVi, @Min_NgayLapPhieuXuat
					--END TRY  
    	--			BEGIN CATCH 
    	--				select ERROR_MESSAGE() as Err
    	--			END CATCH  
		;Enable TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
		
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
    
    						--begin try  
    						--	exec dbo.UpdateTonLuyKeCTHD_whenUpdate @ID_XuatKho, @ID_DonVi, @ngayXuatKho		
    						--	exec dbo.UpdateGiaVon_WhenEditCTHD @ID_XuatKho, @ID_DonVi, @ngayXuatKho		
    						--end try
    						--begin catch
    						--end catch
    						
    					
    				END

					;Enable TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_afterUseAndTra]
 --declare 
 @IDChiNhanhs nvarchar(max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
   @IDNhanViens nvarchar(max) = null,
   @DateFrom datetime = '2023-07-01',
   @DateTo datetime = null,
   @TextSearch nvarchar(max) = 'GDV0000000040',
   @CurrentPage int =0,
   @PageSize int = 10
AS
BEGIN

	if isnull(@CurrentPage,'') =''
			set @CurrentPage = 0
		if isnull(@PageSize,'') =''
		set @PageSize = 30

		if isnull(@DateFrom,'') =''
		begin	
			set @DateFrom = '2016-01-01'		
		end

		if isnull(@DateTo,'') =''
		begin		
			set @DateTo = DATEADD(day, 1, getdate())		
		end
		else
		set @DateTo = DATEADD(day, 1, @DateTo)

			DECLARE @tblChiNhanh table (ID uniqueidentifier primary key)
			if isnull(@IDChiNhanhs,'') !=''
				insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs)		
			else
				set @IDChiNhanhs =''

			DECLARE @tblSearch TABLE (Name [nvarchar](max))
			DECLARE @count int
			INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!=''
			select @count =  (Select count(*) from @tblSearch)

	; with data_cte
	as
	(
			SELECT 
				hd.ID,
				hd.MaHoaDon,
				hd.LoaiHoaDon,
				hd.NgayLapHoaDon,   						
				hd.ID_DoiTuong,	
				hd.ID_HoaDon,
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
				xe.BienSo,
				iif(hd.TongThanhToan =0 or hd.TongThanhToan is null,  hd.PhaiThanhToan, hd.TongThanhToan) as TongThanhToan,
				ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
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
				isnull(hd.PhaiThanhToanBaoHiem,0) as  PhaiThanhToanBaoHiem
			FROM
			(
					select 
						hd.ID			
					from
					(				
						select 
							cthd.ID,
							sum(cthd.SoLuongBan - isnull(cthd.SoLuongTra,0) - isnull(cthd.SoLuongDung,0)) as SoLuongConLai
						from
						(
									------ mua ----
										select 
											ct.ID,
											ct.SoLuong as SoLuongBan,
											0 as SoLuongTra,
											0 as SoLuongDung
										from BH_HoaDon hd
										join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
										where hd.ChoThanhToan=0
										and hd.LoaiHoaDon = 19 ---- khong trahang HDSC
										and hd.NgayLapHoaDon between @DateFrom and @DateTo	
										and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
										and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID) ---- chi get hanghoa + dv
										and (ct.ID_ParentCombo is null OR ct.ID_ParentCombo != ct.ID) ---- khong get parent, get TP combo
						

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
							) cthd
							group by cthd.ID
					)cthConLai
					join BH_HoaDon_ChiTiet ct on cthConLai.ID=  ct.ID
					join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
					where cthConLai.SoLuongConLai > 0
					group by hd.ID
			) tblConLai 
			JOIN BH_HoaDon hd ON tblConLai.ID =	hd.ID	
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID 
			left join Gara_DanhMucXe xe on hd.ID_Xe = xe.ID		
			where ((select count(Name) from @tblSearch b where     			
					hd.MaHoaDon like '%'+b.Name+'%'								
					or dt.MaDoiTuong like '%'+b.Name+'%'		
					or dt.TenDoiTuong like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'		
					or xe.BienSo like '%'+b.Name+'%'			
					)=@count or @count=0)
					)
	, count_cte
	as
	(
		select count (ID) as TotalRow,
		ceiling(count (ID) / cast(@PageSize as float)) as TotalPage
		from data_cte
	),
	tView
	as
	(
	select *
	from data_cte
	cross join count_cte
	order by NgayLapHoaDon desc
	offset (@CurrentPage * @PageSize) rows
	fetch next @PageSize rows only
	)
	----- get row from- to
	select *
	into #tblView
	from tView




		
		select 
				cnLast.*,
				nv.TenNhanVien,			
				cnLast.ConNo1 as ConNo
				
		from
		(
		select 
				tblLast.*,
				tblLast.TongThanhToan 
					--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = 0
					- iif(tblLast.LoaiHoaDonGoc = 6, iif(tblLast.LuyKeTraHang > 0, tblLast.TongGiaTriTra, 
							iif(abs(tblLast.LuyKeTraHang) > tblLast.TongThanhToan, tblLast.TongThanhToan, abs(tblLast.LuyKeTraHang))), tblLast.LuyKeTraHang)
					- tblLast.KhachDaTra  as ConNo1 ---- ConNo = TongThanhToan - GtriBuTru
		from
		(
				select 
					tbl.*,
						isnull(iif(tbl.LoaiHoaDonGoc = 3 or tbl.ID_HoaDon is null,
						iif(tbl.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
							case when tbl.TongGiaTriTra > tbl.KhachNo then tbl.KhachNo						
							else tbl.TongGiaTriTra end),
						(select dbo.BuTruTraHang_HDDoi(tbl.ID_HoaDon,tbl.NgayLapHoaDon,tbl.ID_HoaDonGoc, tbl.LoaiHoaDonGoc))				
					),0) as LuyKeTraHang	
				from
				(
					select hd.*	,
						hdgoc.ID as ID_HoaDonGoc,
						hdgoc.LoaiHoaDon as LoaiHoaDonGoc,
						hdgoc.MaHoaDon as MaHoaDonGoc,

						ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,	
						ISNULL(allTra.NoTraHang,0) as NoTraHang,
						isnull(sqHD.KhachDaTra,0) as KhachDaTra,
						hd.TongThanhToan- isnull(sqHD.KhachDaTra,0) as KhachNo
					from #tblView hd
					left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID
					left join
					(
							select 
								tbUnion.ID_HoaDonLienQuan,
								sum(isnull(tbUnion.KhachDaTra,0)) as KhachDaTra
							from
							(
								------ thu hoadon -----
								select 
									qct.ID_HoaDonLienQuan,
									sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, - qct.TienThu)) as KhachDaTra
								from Quy_HoaDon qhd
								join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon= qhd.ID
								where qhd.TrangThai='1'
								and exists (select hd.ID from #tblView hd where qct.ID_HoaDonLienQuan = hd.ID and  hd.ID_DoiTuong = qct.ID_DoiTuong)
								group by qct.ID_HoaDonLienQuan

								union all

								------ thudathang ---
								select 
									hdFirst.ID,
									hdFirst.KhachDaTra
								from
								(
									select 
										hdxl.ID,
										thuDH.KhachDaTra,
										ROW_NUMBER() over (partition by hdxl.ID_HoaDon order by hdxl.NgayLapHoaDon) as RN
									from BH_HoaDon hdxl
									join 
									(
										select 
											qct.ID_HoaDonLienQuan,
											sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, - qct.TienThu)) as KhachDaTra
										from Quy_HoaDon qhd
										join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
										where qhd.TrangThai='1'
										and exists (select hd.ID from #tblView hd where qct.ID_HoaDonLienQuan = hd.ID_HoaDon and  hd.ID_DoiTuong = qct.ID_DoiTuong)
										group by qct.ID_HoaDonLienQuan
									) thuDH on thuDH.ID_HoaDonLienQuan = hdxl.ID_HoaDon
									where exists (select ID from #tblView hd where hdxl.ID_HoaDon = hd.ID_HoaDon)
									and hdxl.LoaiHoaDon in (1,25)
									and hdxl.ChoThanhToan='0'
								) hdFirst 
								where hdFirst.RN= 1
							) tbUnion group by tbUnion.ID_HoaDonLienQuan
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
				) tbl
		)tblLast
		) cnLast
		left join NS_NhanVien nv on cnLast.ID_NhanVien= nv.ID
		order by NgayLapHoaDon desc
		drop table #tblView
	
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetTonKho_byIDQuyDois]
    @ID_ChiNhanh [uniqueidentifier],
    @ToDate [datetime],
    @IDDonViQuyDois [nvarchar](max),
    @IDLoHangs [nvarchar](max)
AS
BEGIN
	 SET NOCOUNT ON;
    	declare @tblIDQuiDoi table (ID_DonViQuyDoi uniqueidentifier)
    	declare @tblIDLoHang table (ID_LoHang uniqueidentifier)
    
    	insert into @tblIDQuiDoi
    	select Name from dbo.splitstring(@IDDonViQuyDois) 
    	insert into @tblIDLoHang
    	select Name from dbo.splitstring(@IDLoHangs) where Name not like '%null%' and Name !=''

		
		---- get tonluyke theo thoigian 
		---- get tonluyke theo thoigian 
		SELECT 
    		ID_DonViQuiDoi,
    		ID_HangHoa, 		
    		ID_LoHang,		
    		IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, TonLuyKe_NhanChuyenHang, TonLuyKe) AS TonKho, 
    		IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, GiaVon_NhanChuyenHang, GiaVon) AS GiaVon
		into #tblTon
    	FROM (
    		SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang, 
			tbltemp.ID_DonViInput ORDER BY tbltemp.ThoiGian DESC) AS RN 
		
    	FROM (
				select 
						hd.LoaiHoaDon, 
						hd.ID_DonVi,
						qd.ID_HangHoa,
						ct.ID_DonViQuiDoi,
						hd.ID_CheckIn, 					
						@ID_ChiNhanh as ID_DonViInput, 
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
						ct.TonLuyKe_NhanChuyenHang, ct.TonLuyKe) AS TonLuyKe,
    					ct.TonLuyKe_NhanChuyenHang,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
    					ct.GiaVon_NhanChuyenHang, 
    					ct.GiaVon)/ISNULL(qd.TyLeChuyenDoi,1) AS GiaVon,
    					ct.GiaVon_NhanChuyenHang, 
    					ct.ID_LoHang ,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh,
						hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
				
				from 
					(
						--- get all dvquydoi by idhanghoa 
						select qdOut.ID, qdOut.TyLeChuyenDoi,  qdOut.ID_HangHoa
						from DonViQuiDoi qdOut
						where exists (
							select dvqd.ID_HangHoa
							from @tblIDQuiDoi qd
							join DonViQuiDoi dvqd on qd.ID_DonViQuyDoi= dvqd.ID
							where qdOut.ID_HangHoa = dvqd.ID_HangHoa
							)
					) qd
				join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
				join BH_HoaDon_ChiTiet ct on qd.ID = ct.ID_DonViQuiDoi
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID				
				where (hd.ID_DonVi= @ID_ChiNhanh or (hd.ID_CheckIn = @ID_ChiNhanh and hd.YeuCau = '4'))
				and hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10,18, 35,36,37,38,39,40,13,14)
				and (exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= ct.ID_LoHang) Or hh.QuanLyTheoLoHang= 0)    
			) as tbltemp
    	WHERE tbltemp.ThoiGian < @ToDate) tblTonKhoTemp
    	WHERE tblTonKhoTemp.RN = 1;

		---- get giavon tieuchuan theo thoigian
		declare @tblGVTieuChuan table(ID_DonVi uniqueidentifier,
							ID_DonViQuiDoi uniqueidentifier,
							ID_LoHang uniqueidentifier, 
							GiaVonTieuChuan	float)
		insert into @tblGVTieuChuan
		exec GetGiaVonTieuChuan_byTime @ID_ChiNhanh, @ToDate, @IDDonViQuyDois, @IDLoHangs 

	
	select TenHangHoa, 
			lo.MaLoHang,
			qd2.ID_HangHoa,
			qd.ID_DonViQuyDoi as ID_DonViQuiDoi,
			qd2.GiaNhap,
			lo.ID as ID_LoHang, 
			qd2.TyLeChuyenDoi,
			qd2.TenDonViTinh,
			isnull(gvtc.GiaVonTieuChuan,0) as GiaVonTieuChuan,
			isnull(tk.TonKho,0)/ iif(qd2.TyLeChuyenDoi=0 or qd2.TyLeChuyenDoi is null,1, qd2.TyLeChuyenDoi) as TonKho,
			isnull(tk.GiaVon,0) * iif(qd2.TyLeChuyenDoi=0 or qd2.TyLeChuyenDoi is null,1, qd2.TyLeChuyenDoi) as GiaVon
		from @tblIDQuiDoi qd 	
		join DonViQuiDoi qd2 on qd.ID_DonViQuyDoi= qd2.ID 
		join DM_HangHoa hh on hh.ID = qd2.ID_HangHoa
		left join DM_LoHang lo on hh.ID = lo.ID_HangHoa and hh.QuanLyTheoLoHang = 1   
		left join @tblGVTieuChuan gvtc on qd.ID_DonViQuyDoi = gvtc.ID_DonViQuiDoi and (lo.ID = gvtc.ID_LoHang or (gvtc.ID_LoHang is null and lo.ID is null) )
		left join #tblTon tk on hh.ID = tk.ID_HangHoa 
		and (tk.ID_LoHang = lo.ID or hh.QuanLyTheoLoHang =0)
		where (exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= lo.ID) Or hh.QuanLyTheoLoHang= 0)
		order by qd.ID_DonViQuyDoi, lo.ID

		
END");

			Sql(@"ALTER PROCEDURE [dbo].[HuyPhieuXuatKho_WhenUpdateTPDL]
    @ID_CTHoaDon [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    	
    	select ctxk.ID_HoaDon, ID_DonVi, NgayLapHoaDon
		into #tblHuy
    	from
    	(
    		---- get tpdluong of ctOld
    		select ctm.ID
    		from BH_HoaDon_ChiTiet ctm
    		where ctm.ID_ChiTietDinhLuong= @ID_CTHoaDon and ctm.ID!= ctm.ID_ChiTietDinhLuong ---- khong lay dichvu
    	) tpdl
    	join BH_HoaDon_ChiTiet ctxk on tpdl.ID = ctxk.ID_ChiTietGoiDV ---- get ctXuatKho old
    	join BH_HoaDon hdx on ctxk.ID_HoaDon= hdx.ID
    	where  hdx.LoaiHoaDon= 35 and hdx.ChoThanhToan is not null


		---- không chạy hàm tính tồn kho phiếu hủy
		---- vì đã chạy cho hóa đơn gốc ban đầu (ở js)
    	update hd set hd.ChoThanhToan= null
		from BH_HoaDon hd
		where exists (select ID_HoaDon from #tblHuy where  hd.ID = #tblHuy.ID_HoaDon)

END");
        }
        
        public override void Down()
        {
            Sql("DROP FUNCTION [dbo].[fnDemSoLanDoiTra]");
			DropStoredProcedure("[dbo].[BaoCaoGoiDichVu_BanDoiTra]");
			DropStoredProcedure("[dbo].[PhieuXuatKho_GetNVThucHien_byIDChiTietGDV]");
        }
    }
}
