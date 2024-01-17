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

			Sql(@"ALTER PROCEDURE [dbo].[BCBanHang_theoMaDinhDanh]
 --declare 
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
    		0 as GiamGiaHD
    	into #hdHoTro
    	from BH_HoaDon hd
    	join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    	where hd.ChoThanhToan= 0 and hd.LoaiHoaDon= 36	
    	and ct.ChatLieu='6'
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
    			dt.DienThoai,
    			N'Ngày thuốc' as MaHangHoa,
				N'ngày' as TenDonViTinh,
    			N'Ngày thuốc' as TenHangHoa,
    			N'Ngày thuốc' as TenHangHoa_KhongDau,
    			nhom.ID as ID_NhomHang,
    			--nhom.TenNhomHangHoa,
				N'Nhóm ngày thuốc'  as  TenNhomHangHoa,
    			hd.TongGiamGia as SoLuong,
    			0 as DonGia,
    			0 as TienChietKhau,
    			0 as ThanhTien,
    			hd.DienGiai as GhiChu,
    			hd.MaHoaDon as MaDinhDanh,
    			2 as LoaiHangHoa
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
    			where exists (select * from BH_ChiTiet_DinhDanh dd where hd.ID= dd.IdHoaDonChiTiet)
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
    			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa
    		from
    		(
    			----- sp hotro ngaythuoc ----
    			select 
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
    				hd.NgayLapHoaDon,
    					hd.MaHoaDon,
    					hd.LoaiHoaDon,    	
    					hd.TongGiamGia,   	
    					hd.ID_DoiTuong,    			
    				ct.ID,
    					ct.ID_HoaDon,
    					ct.ID_DonViQuiDoi,     		
    				ct.SoLuong,
    					ct.DonGia,
    				ct.TienChietKhau,		
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
    			or tblLast.DienThoai  like '%'+b.Name+'%'
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
				order by tbl.NgayLapHoaDon desc, tbl.MaDinhDanh desc
				OFFSET (@pageNumber* @pageSize) ROWS
				FETCH NEXT @pageSize ROWS ONLY

   
    	drop table #hdHoTro
    	drop table #tblLast

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

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_ChiTiet_Page] --GDV0000000068
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
			hh.ID,
			hh.TenHangHoa,
			qd.MaHangHoa,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa,
			concat(hh.TenHangHoa, qd.ThuocTinhGiaTri) as TenHangHoaFull,
			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			ISNULL(nhh.TenNhomHangHoa,  N'Nhóm hàng hóa mặc định') as TenNhomHangHoa,
			lo.MaLoHang as TenLoHang,
			qd.TenDonViTinh,
			c.IDChiTietHD,
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
			c.ChiPhi,
			c.LoaiHoaDon,
			c.ID_ChiTietGoiDV,
			iif(c.TenHangHoaThayThe is null or c.TenHangHoaThayThe='', hh.TenHangHoa, c.TenHangHoaThayThe) as TenHangHoaThayThe			
		from 
		(
		select 
			b.IDChiTietHD,
			b.ID_ChiTietGoiDV,
			b.LoaiHoaDon,b.NgayLapHoaDon, b.MaHoaDon, b.ID_PhieuTiepNhan, b.ID_DoiTuong, b.ID_NhanVien,-- b.TenNhanVien,
			b.ThoiGianBaoHanh, b.HanBaoHanh, b.TrangThai, b.GhiChu,		
			isnull(qd.TyLeChuyenDoi,1) * (case b.LoaiHoaDon
				when 6 then - b.SoLuong
			else b.SoLuong end)  as SoLuong,
			case b.LoaiHoaDon
				when 6 then - b.ThanhTien
			else b.ThanhTien end as ThanhTien,
		
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
			ct.TienThue,
			ct.GiamGiaHD,			
			ct.ID as IDChiTietHD,
			ct.ID_DonViQuiDoi, ct.ID_LoHang,
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
			ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as ThanhTien, ----- !important: kangjin: sử dung từ GDV: vẫn lấy Thành tiền từ GDV mua
			iif(ct.SoLuong =0, 0, ct.TienVon/ct.SoLuong) as GiaVon,			
			ct.TienVon,
			isnull(cp.ChiPhi,0) as ChiPhi,
			ct.ID_ChiTietGoiDV
		from @tblCTHD ct	
		left join @tblChiPhi cp on ct.ID= cp.ID_ParentCombo	
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
				--	or c.TenNhanVien like N'%'+b.Name+'%'
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
				isnull(gt.TenDoiTuong,'') as NguoiGioiThieu	,
				isnull(maNV.NVThucHien,'') as MaNhanVien,
				isnull(tenNV.NVThucHien,'') as TenNhanVien,
				isnull(ctmOut.MaHoaDon,'') as MaGoiDichVu
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
    		FETCH NEXT @pageSize ROWS ONLY	
			) tbl
			left join DM_NguonKhachHang nk on tbl.ID_NguonKhach= nk.ID
			left join DM_DoiTuong gt on tbl.ID_NguoiGioiThieu= gt.ID 	
			left join
			(
			-- get TenNV thuchien of cthd
			select tblCT.IDChiTietHD as ID_ChiTietHD ,
				 (
						select nv.TenNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						where nvth.ID_ChiTietHoaDon = tblCT.IDChiTietHD
										
						For XML PATH ('')
					) NVThucHien
				from #tblView tblCT 
			) tenNV on tbl.IDChiTietHD = tenNV.ID_ChiTietHD
			left join
			(
			-- get MaNV nvthuchien of cthd
			select tblCT.IDChiTietHD as ID_ChiTietHD ,
				 (
						select nv.MaNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						where nvth.ID_ChiTietHoaDon = tblCT.IDChiTietHD										
						For XML PATH ('')
					) NVThucHien
				from #tblView tblCT 
			) maNV on tbl.IDChiTietHD = maNV.ID_ChiTietHD
			left join
			(
			----- get maGDV: sudung tu GDV nao ----
				select gdv.MaHoaDon, ctMua.ID_ChiTietMua
				from
				(
					select ctm.ID_HoaDon, ctm.ID as ID_ChiTietMua
					from BH_HoaDon_ChiTiet ctm
					where exists (select id from #tblView cthd 
					where cthd.ID_ChiTietGoiDV is not null and ctm.ID = cthd.ID_ChiTietGoiDV) ---- chi lay HD sudung tu GDV
				)ctMua
				join BH_HoaDon gdv on ctMua.ID_HoaDon = gdv.ID
			) ctmOut on ctmOut.ID_ChiTietMua = tbl.ID_ChiTietGoiDV
			order by NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoaXuatHuybyID]
--declare	
	@ID_HoaDon [uniqueidentifier] ='42BEC180-165D-4AF1-A57A-CFF868160975',
	@ID_ChiNhanh [uniqueidentifier] ='d93b17ea-89b9-4ecf-b242-d03b8cde71de'
AS
BEGIN
  set nocount on;

		declare @countCTMua int, @ID_HoaDonGoc uniqueidentifier		
		select @ID_HoaDonGoc= ID_HoaDon from BH_HoaDon where ID= @ID_HoaDon


		----- get all ctm goc (ke ca dv) ---
		---- vi se co truong hop capnhat tpdl (co --> null)
		select ctm.ID, ctm.ID_HoaDon,
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


	select @countCTMua = COUNT(ID) from #ctmua	
		
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
			cast(iif(@countCTMua > 0 and 
				(select count(ID) 
					from #ctmua 
					where #ctmua.ID = ctxk.ID_ChiTietGoiDV 
					and #ctmua.ChatLieu!='5'
					and #ctmua.ChoThanhToan ='0'
				) =0,1,0) as float) as TrangThaiMoPhieu
				
		from BH_HoaDon_ChiTiet ctxk		
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi 
		and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		where ctxk.ID_HoaDon = @ID_HoaDon
		and (hh.LaHangHoa = 1 and tk.TonKho is not null) 
		and (ctxk.ChatLieu is null or ctxk.ChatLieu != '5') 
	
	drop table #ctmua
END

");

			Sql(@"ALTER PROCEDURE [dbo].[ListHangHoaTheKho]
    @ID_HangHoa [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID_HoaDon, 
		MaHoaDon, 
		NgayLapHoaDon,
		LoaiHoaDon, 
		ID_DonVi,
		ID_CheckIn,		
		sum(table1.SoLuong) as SoLuong,
		max(table1.GiaVon) as GiaVon,
		---- lamtron 2 so thapphan --> check lech TonKho - TonLuyKe at js ----
		round(max(table1.TonKho),2) as TonKho,		
		round(sum(sum(table1.SoLuong)) over ( order by NgayLapHoaDon ),2) as LuyKeTonKho,
	case table1.LoaiHoaDon
			when 10 then case when table1.ID_CheckIn = @IDChiNhanh then N'Nhận chuyển hàng' else N'Chuyển hàng' end
			when 40 then N'Xuất hỗ trợ chung'
			when 39 then N'Xuất bảo hành'
			when 38 then N'Xuất bán lẻ'
			when 37 then N'Xuất hỗ trợ ngày thuốc'	
			when 35 then N'Xuất nguyên vật liệu'	
			when 4 then N'Nhập hàng'
			when 6 then N'Khách trả hàng'
			when 7 then N'Trả hàng nhập'
			when 8 then N'Xuất kho'
			when 9 then N'Kiểm hàng'
			when 13 then N'Nhập nội bộ'
			when 14 then N'Nhập hàng thừa'
			when 18 then N'Điều chỉnh giá vốn'		
		end as LoaiChungTu
	FROM
	(
		SELECT hd.ID as ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, 
		CASE WHEN hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4' and hd.LoaiHoaDon = 10 THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon,
		bhct.ThanhTien * dvqd.TyLeChuyenDoi as ThanhTien,
		bhct.TienChietKhau * dvqd.TyLeChuyenDoi TienChietKhau, 
		dvqd.TyLeChuyenDoi,
		hd.YeuCau, 
		hd.ID_CheckIn,
		hd.ID_DonVi,
		hh.QuanLyTheoLoHang,
		dvqd.LaDonViChuan, 
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.TonLuyKe, bhct.TonLuyKe_NhanChuyenHang) as TonKho,
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.GiaVon / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi),bhct.GiaVon_NhanChuyenHang / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi)) as GiaVon,	
		bhct.SoThuTu,
		(case hd.LoaiHoaDon
			when 9 then bhct.SoLuong ---- Số lượng lệch = SLThucTe - TonKhoDB        (-) Giảm  (+): Tăng
			when 10 then
				case when hd.ID_CheckIn= @IDChiNhanh and hd.YeuCau = '4' then bhct.TienChietKhau  ---- da nhanhang
				else iif(hd.YeuCau = '4',- bhct.TienChietKhau,- bhct.SoLuong) end 
			--- xuat
			when 40 then - bhct.SoLuong
			when 39 then - bhct.SoLuong
			when 38 then - bhct.SoLuong
			when 37 then - bhct.SoLuong
			when 35 then - bhct.SoLuong		
			when 7 then - bhct.SoLuong
			when 8 then - bhct.SoLuong		
			--- conlai: nhap
			else bhct.SoLuong end
		) * dvqd.TyLeChuyenDoi as SoLuong
		
	FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	WHERE hd.ChoThanhToan = 0 and hd.LoaiHoaDon in (6,7,35,37,38,39,40,8,4,7,9,10,13,14,18) 
	and (bhct.ChatLieu is null or bhct.ChatLieu not in ('2','5') ) --- ChatLieu = 2: tra GDV, 5. cthd da xoa
	and  hh.ID = @ID_HangHoa 
	and ((hd.ID_DonVi = @IDChiNhanh and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)) or (hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4'))
	)  table1
	group by ID_HoaDon, MaHoaDon, NgayLapHoaDon,LoaiHoaDon, ID_DonVi, ID_CheckIn
	
	order by NgayLapHoaDon desc




END");

			Sql(@"ALTER PROCEDURE [dbo].[GetBaoCaoCongNoChiTiet]
    @IDChiNhanhs [nvarchar](max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @DateFrom [datetime] = '2023-08-01',
    @DateTo [datetime] ='2023-12-01',
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
    
    
    	;with data_cte
    	as
    	(
		select c.* ,
				iif(c.KhachDaTra - c.GiaTriSD > 0,0, c.GiaTriSD - c.KhachDaTra) as NoThucTe1,
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
			nvpt.TenNhanVien as NVPhuTrach,    	
			ISNULL(hd.PhaiThanhToan,0) - ISNULL(soquy.KhachDaTra,0) as KhachNo,
    		isnull(soquy.KhachDaTra,0) as KhachDaTra,
    		isnull(sdGDV.GiaTriSD,0) as GiaTriSD,
    		iif(hd.TongThanhToan - isnull(soquy.KhachDaTra,0) > 0,1,0) as TrangThaiCongNo,
			isnull(hdgoc.LoaiHoaDon,0) as LoaiHoaDonGoc,
			hdgoc.ID_HoaDon as ID_HoaDonGoc,					
			hdgoc.MaHoaDon as MaHoaDonGoc,
			ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,
			ISNULL(allTra.NoTraHang,0) as NoTraHang
    	from BH_HoaDon hd	
    	join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	join DM_DonVi dv on hd.ID_DonVi = dv.ID
		left join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID    	
    	left join
    	(
    		Select 
    			tblUnion.ID_HoaDonLienQuan,			
    			SUM(ISNULL(tblUnion.TienThu, 0)) as KhachDaTra			
    			from
    			(		------ thanhtoan itseft ----			
    					Select 
    						hd.ID as ID_HoaDonLienQuan,
    						iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as TienThu				
    					from BH_HoaDon hd
    					join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 					
    					where (qhd.TrangThai is null or qhd.TrangThai='1')			
    					and hd.LoaiHoaDon in (1,19,22)
    					and hd.ChoThanhToan = '0'				
    					and hd.NgayLapHoaDon between @DateFrom and @DateTo
    					and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    
    					
    					Union all
    
    					----- thanhtoan when dathang -----
    					Select
    						thuDH.ID,				
    						thuDH.TienThu
    					FROM
    					(
    						Select 
    								ROW_NUMBER() OVER(PARTITION BY d.ID_HoaDon ORDER BY d.NgayLapHoaDon ASC) AS isFirst,						
    							d.ID,
    								d.ID_HoaDon,
    								d.NgayLapHoaDon,    						
    							sum(d.TienThu) as TienThu
    						FROM 
    						(
    					
    							Select
    							hd.ID,
    							hd.NgayLapHoaDon,
    							hdd.ID as ID_HoaDon,						
    							iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu) as TienThu											
    							from BH_HoaDon hd 
    							join BH_HoaDon hdd on hd.ID_HoaDon= hdd.ID and hdd.LoaiHoaDon= 3
    							join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDonLienQuan = hdd.ID
    							join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
    							where hd.LoaiHoaDon in (1,19,22)
    							and hd.ChoThanhToan = '0'				
    							and hd.NgayLapHoaDon between @DateFrom and @DateTo
    							and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    						
    						)  d group by d.ID,d.NgayLapHoaDon,ID_HoaDon		
    					) thuDH where isFirst= 1
    			) tblUnion
    			group by tblUnion.ID_HoaDonLienQuan
    	) soquy on hd.ID= soquy.ID_HoaDonLienQuan
    	left join(
    		------ sudung gdv
    		select 
    			gdv.ID,
    			sum(ctsd.SoLuong * (ctsd.DonGia - ctsd.TienChietKhau)) as GiaTriSD
    		from BH_HoaDon gdv
    		join BH_HoaDon_ChiTiet ctm on gdv.ID = ctm.ID_HoaDon
    		 join BH_HoaDon_ChiTiet ctsd on ctm.ID= ctsd.ID_ChiTietGoiDV 
    		 join BH_HoaDon hdsd on ctsd.ID_HoaDon= hdsd.ID 
    		where gdv.LoaiHoaDon= 19 and gdv.ChoThanhToan='0'
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
    	where hd.LoaiHoaDon in (1,19,22)
    	and hd.ChoThanhToan = '0'
    	and hd.TongThanhToan > 0
    	and hd.NgayLapHoaDon between @DateFrom and @DateTo
    	and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    	and ((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'	
    					or hd.MaHoaDon like '%' +b.Name +'%' 		
    					)=@count or @count=0)	
    	--) tbl where (@TrangThais ='' or tbl.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))
    	) c	where (@TrangThais ='' or c.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))	 		
    	),
		tblDebit as
		(
			select 
				cnLast.ID,
				cnLast.TongTienHDTra,
				cnLast.ConNo			
						
			from
			(
				select 
					c.ID,
					c.LoaiHoaDonGoc,
					c.TongGiaTriTra,
					iif(c.LoaiHoaDonGoc = 6, iif(c.LuyKeTraHang > 0, c.TongGiaTriTra, 
						iif(abs(c.LuyKeTraHang) > c.TongThanhToan, c.TongThanhToan, abs(c.LuyKeTraHang))), c.LuyKeTraHang) as TongTienHDTra,
					iif(c.ChoThanhToan is null,0, 
						c.TongThanhToan 
							------ neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = 0
							- iif(c.LoaiHoaDonGoc = 6, 
								iif(c.LuyKeTraHang > 0, c.TongGiaTriTra, iif(abs(c.LuyKeTraHang) > c.TongThanhToan, c.TongThanhToan, abs(c.LuyKeTraHang))), 
								c.LuyKeTraHang)
							- c.KhachDaTra - c.BaoHiemDaTra) as ConNo ---- ConNo = TongThanhToan - GtriBuTru
				from data_cte c
			) cnLast 
		),
    	count_cte as
    	 (
    		select count(dt.ID) as TotalRow,
    			CEILING(COUNT(dt.ID) / CAST(@PageSize as float ))  as TotalPage,
    			sum(TongThanhToan) as TongThanhToanAll,
    			sum(KhachDaTra) as KhachDaTraAll,
    			sum(cn.ConNo) as ConNoAll --,
    			--sum(TongThanhToan - cn.NoThucTe1) as NoThucTeAll
    			from data_cte dt
				left join tblDebit cn on dt.ID= cn.ID
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
		hd.TongThanhToan - isnull(cn.TongTienHDTra,0) as GiaTriSauTra,
		ISNULL(qtCN.GiaTriTatToan,0) as GiaTriTatToan,
		iif(hd.LoaiHoaDon=22, cn.ConNo - ISNULL(qtCN.GiaTriTatToan,0),cn.ConNo) as ConNo,
		iif(hd.LoaiHoaDon=22, hd.NoThucTe1 - ISNULL(qtCN.GiaTriTatToan,0),hd.NoThucTe1) as NoThucTe,
		tblNV.TenNhanViens
	from tView hd
	left join tblDebit cn on hd.ID= cn.ID
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
	left join
	(
		select hd.ID_HoaDon,
			sum(hd.PhaiThanhToan) as GiaTriTatToan
		from BH_HoaDon hd
		where hd.ChoThanhToan='0'
		and LoaiHoaDon= 42
		group by hd.ID_HoaDon
	) qtCN on hd.ID= qtCN.ID_HoaDon
	order by NgayLapHoaDon desc
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

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_Where]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max),
	@ID_NhanVienLogin nvarchar(max) = '',
	@NguoiTao nvarchar(max)='',
	@IDViTris nvarchar(max)='',
	@IDBangGias nvarchar(max)='',
	@TrangThai nvarchar(max)='0,1,2',
	@PhuongThucThanhToan nvarchar(max)='',
	@ColumnSort varchar(max)='NgayLapHoaDon',
	@SortBy varchar(max)= 'DESC',
	@CurrentPage int,
	@PageSize int
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
					select 
						c.ID,
						c.LoaiHoaDonGoc,
						c.TongGiaTriTra,
						----- cot TongGiaTriTra: tongTra of hdThis ---
						iif(c.LoaiHoaDonGoc = 6, 
							iif(c.LuyKeTraHang > 0, c.TongGiaTriTra, 
								---- neu LuyKeTrahang < 0 (tức trả hàng > nợ HD cũ): BuTruTrahang = max (TongTienHang)
								iif(abs(c.LuyKeTraHang) > c.TongThanhToan, c.TongThanhToan, abs(c.LuyKeTraHang))
								),
						 c.LuyKeTraHang) as TongTienHDTra,					
						iif(c.ChoThanhToan is null,0, 
							----- hdDoi co congno < tongtra							
							c.TongThanhToan 
								--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = TongGiaTriTra							
								- iif(c.LoaiHoaDonGoc = 6, iif(c.LuyKeTraHang > 0,  c.TongGiaTriTra, iif(abs(c.LuyKeTraHang) > c.TongThanhToan, c.TongThanhToan, abs(c.LuyKeTraHang))), c.LuyKeTraHang)
								- c.KhachDaTra ) as ConNo ---- ConNo = TongThanhToan - GtriBuTru
					from data_cte c
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

			Sql(@"ALTER PROCEDURE [dbo].[getListDanhSachHHImportKiemKe]
    @MaLoHangIP [nvarchar](max),
    @MaHangHoaIP [nvarchar](max),
    @ID_DonViIP [uniqueidentifier],
    @TimeIP [datetime]
AS
BEGIN


 --   DECLARE @TableImport TABLE (ID_DonViQuiDoi UNIQUEIDENTIFIER, ID UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, QuanLyTheoLoHang BIT, MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX),
 --   	ThuocTinh_GiaTri NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), TyLeChuyenDoi FLOAT, GiaNhap FLOAT, MaLoHang NVARCHAR(MAX), GiaVon FLOAT, TonKho FLOAT, NgayHetHan DATETIME) 
	--INSERT INTO @TableImport
    --Select *
    --FROM
    --(
		select 
    		dvqd.ID as ID_DonViQuiDoi,
    		hh.ID as ID,
    		lh.ID as ID_LoHang,
    		case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    		dvqd.MaHangHoa,
    		hh.TenHangHoa,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd.TenDonViTinh,
    		dvqd.TyLeChuyenDoi,
    		dvqd.GiaNhap,
    		Case when lh.ID is null then '' else lh.MaLoHang end as MaLoHang,
    		Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVon,
    		cast(0 as float) as TonKho,
    		Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan
    	FROM DonViQuiDoi dvqd    		
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	left join DM_LoHang lh on lh.ID_HangHoa = hh.ID and lh.MaLoHang = @MaLoHangIP 
    	left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or gv.ID_LoHang is null) and gv.ID_DonVi = @ID_DonViIP)
    	where dvqd.MaHangHoa = @MaHangHoaIP 
    		and dvqd.Xoa = 0
    		and hh.TheoDoi = 1 
    		---	) as p order by NgayHetHan
    

  --  	DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER, @ID_HangHoa UNIQUEIDENTIFIER, @ID_LoHang UNIQUEIDENTIFIER, @TyLeChuyenDoi FLOAT, @TonKhoHienTai FLOAT;

		--select top 1 @ID_DonViQuiDoi = ID_DonViQuiDoi,
		--	@ID_HangHoa= ID,
		--	@ID_LoHang = ID_LoHang,
		--	@TyLeChuyenDoi = TyLeChuyenDoi
		--from @TableImport


		--SET @TonKhoHienTai = ISNULL([dbo].FUNC_TonLuyKeTruocThoiGian(@ID_DonViIP,@ID_HangHoa,@ID_LoHang,@TimeIP),0)
		--UPDATE @TableImport SET TonKho = @TonKhoHienTai / @TyLeChuyenDoi WHERE ID_DonViQuiDoi = @ID_DonViQuiDoi
    	
  --  	SELECT * from @TableImport
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
