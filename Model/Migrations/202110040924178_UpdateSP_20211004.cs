namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20211004 : DbMigration
    {
        public override void Up()
        {
			Sql(@"CREATE FUNCTION [dbo].[BaoDuong_GetTongGiaTriNhac]
(
	@LanBaoDuong int,
	@ID_HangHoa uniqueidentifier
)
RETURNS float
AS
BEGIN
	DECLARE @TongGiaTri float

	declare @tmp table(ID uniqueidentifier, ID_HangHoa uniqueidentifier, LanBaoDuong int, GiaTri float, LoaiGiaTri int, LapDinhKy int)
			insert into @tmp
			select *
			from DM_HangHoa_BaoDuongChiTiet where ID_HangHoa= @ID_HangHoa
		
			declare @tblGiaTri table (GiaTri float)	
			declare @flag int= @LanBaoDuong;
			
			while @flag!=0
			begin
				insert into @tblGiaTri 
				select 
					case LoaiGiaTri 
						when 4 then GiaTri
						when 3 then 365* GiaTri
						when 2 then 30 * GiaTri
						when 1 then GiaTri
						end
				from @tmp where LanBaoDuong= @flag
				set @flag = @flag - 1
			end
			
		select @TongGiaTri= sum(GiaTri)
		from @tblGiaTri

	RETURN @TongGiaTri

END
");

			Sql(@"CREATE FUNCTION [dbo].[GetGiaVonDichVu_ofCTHD]
(
	@ID_CTHoaDon uniqueidentifier
)
RETURNS float
AS
BEGIN
	DECLARE @SumGiaVon float = 0

	declare @Level1_CTHoaDon uniqueidentifier, @Level1_IDParent uniqueidentifier,@Level1_ChiTietDinhLuong uniqueidentifier, 
		@SoLuong nvarchar(max), @GiaVon float
	declare _cur1 cursor for
   select ct.ID,
		ct.ID_ParentCombo,
		ct.ID_ChiTietDinhLuong, 
		ct.SoLuong,
		ct.GiaVon
	from BH_HoaDon_ChiTiet ct
	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
	where hd.LoaiHoaDon = 1
	and hd.ChoThanhToan= 0
	and (ct.ID_ParentCombo= @ID_CTHoaDon or ct.ID_ChiTietDinhLuong= @ID_CTHoaDon)
	and ct.ID!=@ID_CTHoaDon
	
	open _cur1
	fetch next from _cur1 into @Level1_CTHoaDon, @Level1_IDParent, @Level1_ChiTietDinhLuong, @SoLuong, @GiaVon
	while @@FETCH_STATUS =0
	begin
		if @Level1_IDParent is null 
			begin		
				set @SumGiaVon += @SoLuong * @GiaVon		
			end
		else
			begin	
				if @Level1_ChiTietDinhLuong is null 
					set @SumGiaVon += @SoLuong * @GiaVon	
				else
					set @sumGiaVon +=  (select dbo.GetGiaVonDichVu_ofCTHD(@Level1_ChiTietDinhLuong))
			end	
		fetch next from _cur1 into @Level1_CTHoaDon, @Level1_IDParent, @Level1_ChiTietDinhLuong, @SoLuong, @GiaVon
	end
	close _cur1
	deallocate _cur1
	RETURN @SumGiaVon

END
");

			Sql(@"CREATE FUNCTION [dbo].[GetGiaVonOfDichVu]
(
	@ID_DonVi uniqueidentifier,
	@ID_DichVu uniqueidentifier	
)
RETURNS float
AS
BEGIN
	
	DECLARE @SumGiaVon float = 0

	declare @Level1_IDDichVu uniqueidentifier, @LaHangHoa bit, @MaHangHoa nvarchar(max), @SoLuong float
	declare _cur1 cursor for
    select 
		dl.ID_DonViQuiDoi,
		hh.LaHangHoa,
		qd.MaHangHoa,
		dl.SoLuong
	from DinhLuongDichVu dl
	join DonViQuiDoi qd on dl.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	where dl.ID_DichVu= @ID_DichVu
	and dl.ID_DonViQuiDoi !=@ID_DichVu -- tránh trường hợp thành phần dịch vụ là chính nó
	
	open _cur1
	fetch next from _cur1 into @Level1_IDDichVu, @LaHangHoa, @MaHangHoa, @SoLuong
	while @@FETCH_STATUS =0
	begin
		if @LaHangHoa='0'
			begin		
				set @SumGiaVon += @SoLuong *  (select dbo.GetGiaVonOfDichVu(@ID_DonVi, @Level1_IDDichVu))				
			end
		else
			begin				
				set @sumGiaVon += @SoLuong *  isnull((select top 1 GiaVon from DM_GiaVon where ID_DonVi= @ID_DonVi and ID_DonViQuiDoi = @Level1_IDDichVu),0)	
			end	
		fetch next from _cur1 into @Level1_IDDichVu, @LaHangHoa,@MaHangHoa, @SoLuong
	end
	close _cur1
	deallocate _cur1
	RETURN @SumGiaVon

END");

			CreateStoredProcedure(name: "[dbo].[BaoDuong_InsertListDetail_ByNhomHang]", parametersAction: p => new
			{
				ID_HangHoa = p.Guid()
			}, body: @"SET NOCOUNT ON;

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
	) a");

			CreateStoredProcedure(name: "[dbo].[BCBanHang_GetCTHD]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				LoaiChungTus = p.String()
			}, body: @"SET NOCOUNT ON;

	
	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER)
    INSERT INTO @tblChiNhanh
    select Name from splitstring(@IDChiNhanhs);

	DECLARE @tblLoaiHoaDon TABLE(LoaiHoaDon int)
    INSERT INTO @tblLoaiHoaDon
    select Name from splitstring(@LoaiChungTus);


	--- hdmua
	select 
		hd.NgayLapHoaDon, hd.MaHoaDon,hd.LoaiHoaDon,
		hd.ID_DonVi, hd.ID_PhieuTiepNhan, hd.ID_DoiTuong, hd.ID_NhanVien,	
		hd.TongTienHang, hd.TongGiamGia,hd.KhuyeMai_GiamGia,
		hd.ChoThanhToan,
		ct.ID, ct.ID_HoaDon, ct.ID_DonViQuiDoi, ct.ID_LoHang,
		ct.ID_ChiTietGoiDV, ct.ID_ChiTietDinhLuong, ct.ID_ParentCombo,
		ct.SoLuong, ct.DonGia,  ct.GiaVon,
		ct.TienChietKhau, ct.TienChiPhi,
		ct.ThanhTien, ct.ThanhToan,
		ct.GhiChu, ct.ChatLieu,
		ct.LoaiThoiGianBH, ct.ThoiGianBaoHanh,		
		Case when hd.TongTienThueBaoHiem > 0 
			then case when hd.TongThueKhachHang = 0 or hd.TongThueKhachHang is null
				then ct.ThanhTien * (hd.TongTienThue / hd.TongTienHang) 
				else ct.TienThue * ct.SoLuong end
		else ct.TienThue * ct.SoLuong end as TienThue,
		Case when hd.TongTienHang = 0 then 0 else ct.ThanhTien * ((hd.TongGiamGia + isnull(hd.KhuyeMai_GiamGia,0)) / hd.TongTienHang) end as GiamGiaHD
	into #cthdMua
	from BH_HoaDon_ChiTiet ct
	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID	
   where hd.ChoThanhToan=0
   and hd.NgayLapHoaDon between @DateFrom and @DateTo
   and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)
   and exists (select LoaiHoaDon from @tblLoaiHoaDon loai where loai.LoaiHoaDon = hd.LoaiHoaDon)
	and (ct.ChatLieu is null or ct.ChatLieu !='4')


	---- === GIA VON HD LE ===
	select 
		b.IDComBo_Parent,
		sum(b.GiaVon) as GiaVon,
		sum(b.TienVon) as TienVon
	INTO #gvLe
	from
	(
	 select dluongParent.*,
			 iif(ctm.ID_ParentCombo is not null, ctm.ID_ParentCombo, ctm.ID) as IDComBo_Parent
		 from
		 (
	select 
		---- khong get ID_ChiTietGoiDV neu xu ly dathang
		iif(ctAll.ID_ChiTietGoiDV is not null and ctAll.ID_HoaDon is null, ctAll.ID_ChiTietGoiDV,ctAll.ID) as ID_ChiTietGoiDV,
		child.GiaVon,
		child.TienVon
		from
		(
			select 
				ct.ID_ComBo,
				sum(ct.GiaVon) as GiaVon,
				sum(ct.TienVon) as TienVon
			from
			(
			select 
				iif(ctLe.ID_ParentCombo is not null, ctLe.ID_ParentCombo, 
								iif(ctLe.ID_ChiTietDinhLuong is not null, ctLe.ID_ChiTietDinhLuong, ctLe.ID)) as ID_ComBo,
				iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID, 0,  ctLe.GiaVon) as GiaVon,
				iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID, 0, ctLe.SoLuong * ctLe.GiaVon) as TienVon
			from #cthdMua ctLe
			where LoaiHoaDon= 1 
			) ct group by ct.ID_ComBo
		) child
		join #cthdMua ctAll on child.ID_ComBo = ctAll.ID
		) dluongParent join #cthdMua ctm on dluongParent.ID_ChiTietGoiDV= ctm.ID
	) b group by b.IDComBo_Parent
	

   ---- xuatkho or sudung gdv
   select hdx.MaHoaDon, hdx.LoaiHoaDon,
	ctx.ID,	ctx.ID_ChiTietDinhLuong, ctx.ID_ParentCombo,ctx.ID_ChiTietGoiDV,
	ctx.ID_DonViQuiDoi,
	ctx.SoLuong, ctx.GiaVon, ctx.ThanhTien
	into #tblAll
   from BH_HoaDon_ChiTiet ctx
   join BH_HoaDon hdx on ctx.ID_HoaDon= hdx.ID
	   where exists (
	   select ID
	   from #cthdMua ctm where ctx.ID_ChiTietGoiDV = ctm.ID
   )

   select xksdGDV.ID_ChiTietGoiDV, xksdGDV.GiaVon, xksdGDV.SoLuong  *  xksdGDV.GiaVon as TienVon
   into #xksdGDV
   from BH_HoaDon_ChiTiet xksdGDV
     where exists (
	   select ID
	   from #tblAll ctsc where xksdGDV.ID_ChiTietGoiDV = ctsc.ID)


   				---- === GIAVON XUATKHO SUA CHUA ===
	
				select 
					c.ID_Parent,
					sum(c.GiaVon) as GiaVon,
					sum(c.TienVon) as TienVon
				into  #xuatSC
				from
				(
				select 
					iif(ctm2.ID_ParentCombo is not null, ctm2.ID_ParentCombo, ctm2.ID) as ID_Parent,
					b.GiaVon,
					b.TienVon
				from
				(
				select 
					gvXK.ID_Combo,
					sum(gvXK.GiaVon) as GiaVon,
					sum(gvXK.TienVon) as TienVon			
				from
				(
				select 
					IIF(ctm.ID_ParentCombo is not null, ctm.ID_ParentCombo,
					iif(ctm.ID_ChiTietDinhLuong is not null, ctm.ID_ChiTietDinhLuong, ctm.ID)) as ID_Combo,
					b.GiaVon,
					b.TienVon
				from
				(		
				   select 
					gvComBo.ID_ChiTietGoiDV,
					sum(GiaVon) as GiaVon,
					sum(TienVon) as TienVon
				
				   from
				   (
				   select 
						iif(ctXuat.ID_ChiTietGoiDV is not null, ctXuat.ID_ChiTietGoiDV, ctXuat.ID) as ID_ChiTietGoiDV,
						ctXuat.GiaVon as GiaVon,
						ctXuat.SoLuong * ctXuat.GiaVon as TienVon
					   from #tblAll ctXuat
					   where ctXuat.LoaiHoaDon = 8
					) gvComBo group by gvComBo.ID_ChiTietGoiDV
				) b
				join #cthdMua ctm on b.ID_ChiTietGoiDV= ctm.ID
				) gvXK 
				group by gvXK.ID_Combo
				) b
				join #cthdMua ctm2 on b.ID_Combo = ctm2.ID
				) c group by c.ID_Parent
				


				----  === GIAVON XUAT SUDUNG ===
		select gvSD.IDComBo_Parent,
			sum(gvSD.GiaVon) as GiaVon,
			sum(gvSD.TienVon) as TienVon
		into #gvSD
		from
		(
			 ---- group combo at parent
			 select 
				iif(ctm2.ID_ParentCombo is not null, ctm2.ID_ParentCombo, ctm2.ID) as IDComBo_Parent,
				b.GiaVon,
				b.TienVon
			 from(
					select c.*,
						iif(ctm.ID_ChiTietDinhLuong is not null, ctm.ID_ChiTietDinhLuong, ctm.ID) as IDDLuong_Parent
					from
					(
					---- group dinhluong at parent by id_ctGoiDV
						select 
						iif(ctAll.ID_ChiTietGoiDV is not null, ctAll.ID_ChiTietGoiDV,ctAll.ID) as ID_ChiTietGoiDV,
						child.GiaVon,
						child.TienVon
						from
						(
						
							---- xuat sudung gdv le
							select 
								gvComBo.ID_ComBo,
								sum(GiaVon) as GiaVon,
								sum(TienVon) as TienVon
							from
							(
							select 
								iif(ctLe.ID_ParentCombo is not null, ctLe.ID_ParentCombo, 
									iif(ctLe.ID_ChiTietDinhLuong is not null, ctLe.ID_ChiTietDinhLuong, ctLe.ID)) as ID_ComBo,
								iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID , 0,  ctLe.GiaVon) as GiaVon,
								iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID  , 0, ctLe.SoLuong * ctLe.GiaVon) as TienVon
							from #tblAll ctLe
							where ctLe.LoaiHoaDon in (1)

							---- xuat sudung goi baoduong
							union all
							select *
							from #xksdGDV
							
							) gvComBo group by gvComBo.ID_ComBo
						) child
						join #tblAll ctAll on child.ID_ComBo = ctAll.ID
				) c join #cthdMua ctm on c.ID_ChiTietGoiDV= ctm.ID
			) b join #cthdMua ctm2 on b.IDDLuong_Parent = ctm2.ID
		) gvSD
		group by gvSD.IDComBo_Parent
	
					
	--select *
	--	from #xuatSC

	--	select *
	--	from #gvSD

  select 
	ctmua.*,
	isnull(gv.GiaVon,0) as GiaVon,
	isnull(gv.TienVon,0) as TienVon
  from #cthdMua ctmua
  left join
  (
	
		---- giavon hdle
		select *
		from #gvLe

		union all
		--- giavon xuatkho sc
		select *
		from #xuatSC

		union all
		--- giavon xuatkho sudung gdv
		select *
		from #gvSD				
	) gv on ctmua.ID = gv.IDComBo_Parent	");

			CreateStoredProcedure(name: "[dbo].[CapNhatThongBaoBaoDuongXe]", body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @dateFrom DATETIME, @dateNow DATETIME, @dateFromTemp DATETIME;
	DECLARE @dateTo DATETIME, @ThoiGianNhacTruoc INT, @LoaiThoiGianNhacTruoc INT, @SoLanLapLai INT, @LoaiThoiGianLapLai INT;
	SELECT @ThoiGianNhacTruoc = NhacTruocThoiGian, @LoaiThoiGianNhacTruoc = NhacTruocLoaiThoiGian, @SoLanLapLai = SoLanLapLai,
	@LoaiThoiGianLapLai = LoaiThoiGianLapLai FROM HT_ThongBao_CatDatThoiGian WHERE LoaiThongBao = 4;

	SET @dateNow = dateadd(day, datediff(day, 0, getdate()), 0);

	SET @dateFrom = IIF(@LoaiThoiGianNhacTruoc = 3, DATEADD(DAY, @ThoiGianNhacTruoc, @dateNow), 
	IIF(@LoaiThoiGianNhacTruoc = 4, DATEADD(MONTH, @ThoiGianNhacTruoc, @dateNow), 
	IIF(@LoaiThoiGianNhacTruoc = 5, DATEADD(YEAR, @ThoiGianNhacTruoc, @dateNow), @dateNow)));
	SET @dateTo = dateadd(day, datediff(day, 0, @dateFrom)+1, 0);
	
	DECLARE @tblLichBaoDuong TABLE(ID UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, ID_HoaDon UNIQUEIDENTIFIER, 
	LanBaoDuong INT, SoKmBaoDuong INT, NgayBaoDuongDuKien DATETIME, NgayTao DATETIME, TrangThai INT, ID_Xe UNIQUEIDENTIFIER,
	GhiChu NVARCHAR(MAX), LanNhac INT);
	--PRINT @dateFrom
	INSERT INTO @tblLichBaoDuong
	SELECT * FROM Gara_LichBaoDuong WHERE NgayBaoDuongDuKien BETWEEN @dateFrom AND @dateTo AND TrangThai = 1;

	SET @dateFromTemp = @dateFrom;
	DECLARE @intFlag INT
	SET @intFlag = 1
	WHILE (@intFlag < @SoLanLapLai)
	BEGIN
		SET @dateFrom = IIF(@LoaiThoiGianLapLai = 3, DATEADD(DAY, @intFlag*-1, @dateFromTemp), 
		IIF(@LoaiThoiGianLapLai = 4, DATEADD(MONTH, @intFlag*-1, @dateFromTemp), 
		IIF(@LoaiThoiGianLapLai = 5, DATEADD(YEAR, @intFlag*-1, @dateFromTemp), @dateFromTemp)));
		SET @dateTo = dateadd(day, datediff(day, 0, @dateFrom)+1, 0);
	
		INSERT INTO @tblLichBaoDuong
		SELECT * FROM Gara_LichBaoDuong WHERE NgayBaoDuongDuKien BETWEEN @dateFrom AND @dateTo AND TrangThai = 1;

		SET @intFlag = @intFlag + 1;
	END
	
	DECLARE @tblXeBaoDuong TABLE(IDXe UNIQUEIDENTIFIER, BienSo NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX));

	INSERT INTO @tblXeBaoDuong
	SELECT dmx.ID, dmx.BienSo, hh.TenHangHoa FROM @tblLichBaoDuong lbd
	INNER JOIN Gara_DanhMucXe dmx ON lbd.ID_Xe = dmx.ID
	INNER JOIN DM_HangHoa hh ON lbd.ID_HangHoa = hh.ID;

	DECLARE @tblNoiDungThongBao TABLE(IDThongBao UNIQUEIDENTIFIER, BienSo NVARCHAR(MAX), TenHangHoaBaoDuong NVARCHAR(MAX), SoLuongHangBaoDuong INT);
	INSERT INTO @tblNoiDungThongBao
	SELECT NEWID(),a.BienSo,
	STUFF((SELECT  N', ' +  TenHangHoa [text()]
		  FROM @tblXeBaoDuong b WHERE a.IDXe=b.IDXe
		  for XML PATH (N''),TYPE).
		  value(N'.',N'NVARCHAR(MAX)'),1,2,N'') AS HangHoaBaoDuong, COUNT(TenHangHoa) AS SoLuongHangBaoDuong
	FROM @tblXeBaoDuong as a
	GROUP BY a.IDXe,a.BienSo;

	INSERT INTO HT_ThongBao
	SELECT IDThongBao, 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE', 4, 
	'<p onclick=""loaddadoc('''+ convert(nvarchar(50), IDThongBao) +N''')"">Biển số xe <a href="" /#/Gara_LichNhacBaoDuong?s='+ BienSo + '&t=1"">' + BienSo + N'</a> có '+ convert(nvarchar(50), SoLuongHangBaoDuong) + N' hàng hóa: '+ TenHangHoaBaoDuong + N' đến lịch bảo dưỡng.</p>', 
	GETDATE(), '' FROM @tblNoiDungThongBao; ");

			CreateStoredProcedure(name: "[dbo].[GetCustomer_haveBirthday]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime()
			}, body: @"SET NOCOUNT ON;

	declare @where nvarchar(max), @paramDefined nvarchar(max), @sql1 nvarchar(max) ='', @sql2 nvarchar(max) = ''
	
	declare @tblDefined nvarchar(max) = N' declare @tblChiNhanh table (ID uniqueidentifier) declare @tblIDNhoms table (ID varchar(36)) '

	set @sql1 = concat(@sql1, N'declare @monthFrom int = 0, @dayFrom int = 0, @monthTo int = 0, @dayTo int = 0 ')
		
	set @where =' where 1 = 1'

	if ISNULL(@IDChiNhanhs,'') !=''
		begin
			set @sql1 = concat(@sql1, ' insert into @tblChiNhanh select * from splitstring(@IDChiNhanhs_In) 
						insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'') 
						declare @QLTheoCN bit = 0;
				declare @countQL int=0;
				select distinct QuanLyKhachHangTheoDonVi into #temp from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)
				set @countQL = (select COUNT(*) from #temp)
				if	@countQL= 1 
						set @QLTheoCN = (select QuanLyKhachHangTheoDonVi from #temp)
				
    			if @QLTheoCN = 1
    				begin									
    					insert into @tblIDNhoms(ID)
    					select *  from (
    						---- get Nhom not not exist in NhomDoiTuong_DonVi
    						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = 1
    						union all
    						-- get Nhom at this ChiNhanh
    						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) ) tbl
    				end
    			else
    				begin				
    				---- insert all
    				insert into @tblIDNhoms(ID)
    				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    				where LoaiDoiTuong = 1
    				end	')
			set @where = CONCAT(@where, ' and EXISTS(SELECT Name FROM splitstring(ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID) ')
			
		end

	if ISNULL(@DateFrom,'') !=''
		begin
			set @sql1 = concat(@sql1, N' set @monthFrom = datepart(month,@DateFrom_In) 
			set @dayFrom = datepart(day,@DateFrom_In) ')

			if isnull(@DateTo,'') !=''
				begin
					set @sql1 = concat(@sql1, N' set @monthTo = datepart(month,@DateTo_In) 
					set @dayTo = datepart(day,@DateTo_In) ')

					set @where = CONCAT(@where, N' and ( (ThangSinh between @monthFrom + 1 and @monthTo - 1) ',
											' or (@monthFrom = @monthTo and @monthFrom = ThangSinh and NgaySinh >= @dayFrom and NgaySinh <= @dayTo)
												or ( @monthFrom != @monthTo and((ThangSinh = @monthFrom and NgaySinh >= @dayFrom) or (ThangSinh = @monthTo and NgaySinh <= @dayTo)))																															
												)') 
				end
			else ---- @dateto is null
				begin
					SET @where = CONCAT(@where, N' and (ThangSinh > @monthFrom 
									 or ( ThangSinh = @monthFrom AND NgaySinh >= @dayFrom)
										)')
				end
		end
	else
		begin ---datefrom is null
				if isnull(@DateTo,'') !=''
					begin
						set @sql1 = concat(@sql1, N' set @monthTo = datepart(month,@DateTo_In) 
									set @dayTo = datepart(day,@DateTo_In) ')
						set @where = CONCAT(@where, N' and (ThangSinh < @monthTo
								 or ( ThangSinh = @monthTo AND (NgaySinh <= @dayTo
								 or DinhDang_NgaySinh =''MM/yyyy'')))')
					end
		end
			
		
		set @paramDefined = N'@IDChiNhanhs_In nvarchar(max),
							@DateFrom_In datetime,
							@DateTo_In datetime'


		set @sql2 = CONCAT (@tblDefined, @sql1,'
						select ID, 
							MaDoiTuong as MaNguoiNop, 
							TenDoiTuong as NguoiNopTien,
							DienThoai as SoDienThoai,
							Email, DiaChi,
							NgaySinh_NgayTLap,
							DinhDang_NgaySinh,
							NgaySinh, ThangSinh,
							ID_NhomDoiTuong as IDNhomDoiTuongs
						from
						(select ID, MaDoiTuong, 
								TenDoiTuong, 
								DienThoai,
								DiaChi,
								Email, NgaySinh_NgayTLap, DinhDang_NgaySinh,
								DATEPART(month,NgaySinh_NgayTLap) as ThangSinh,
								DATEPART(DAY,NgaySinh_NgayTLap) as NgaySinh,
							case when IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' 
							else  ISNULL(IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong
						from DM_DoiTuong
						where NgaySinh_NgayTLap is not null	
						and TheoDoi = 0 and LoaiDoiTuong = 1
						and DinhDang_NgaySinh !=''yyyy''
						)a ', @where ,' order by ThangSinh, NgaySinh ')
					
					print @sql2
	
	exec sp_executesql @sql2,@paramDefined,
	@IDChiNhanhs_In= @IDChiNhanhs,
	@DateFrom_In= @DateFrom,
	@DateTo_In= @DateTo");

			CreateStoredProcedure(name: "[dbo].[GetCustomer_haveTransaction]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime()
			}, body: @"SET NOCOUNT ON;

	declare @where nvarchar(max), @paramDefined nvarchar(max), @sql nvarchar(max) =''
	
	declare @tblDefined nvarchar(max) = N' declare @tblChiNhanh table (ID uniqueidentifier) '
	
		
	set @where =' where 1 = 1 and LoaiHoaDon in (1,3,6,22,19,25) and ChoThanhToan = 0'

	if isnull(@IDChiNhanhs,'') !=''
		begin
			set @sql = concat(@sql,' insert into @tblChiNhanh select * from splitstring(@IDChiNhanhs_In)') 
			set @where = concat(@where,' and exists (select ID from @tblChiNhanh cn where ID_DonVi = cn.ID)') 
		end
	
	if isnull(@DateFrom,'') !=''
		begin
			set @where = concat(@where,' and NgayLapHoaDon >= @DateFrom_In') 
		end

	if isnull(@DateTo,'') !=''
		begin
			set @DateTo = DATEADD(day,1,@DateTo)
			set @where = concat(@where,' and NgayLapHoaDon < @DateTo_In') 
		end

	set @sql = CONCAT(@tblDefined, @sql, ' select dt.ID,
								dt.MaDoiTuong as MaNguoiNop, 
								dt.TenDoiTuong as NguoiNopTien,
								dt.DienThoai as SoDienThoai,
								dt.Email, dt.DiaChi							
								from (select distinct ID_DoiTuong
									from BH_HoaDon ', @where, 
								') tbl 
								join DM_DoiTuong dt on tbl.ID_DoiTuong = dt.ID
								where dt.TheoDoi = 0
								and dt.LoaiDoiTuong = 1 ')
							
		set @paramDefined = '@IDChiNhanhs_In nvarchar(max),
							@DateFrom_In datetime,
							@DateTo_In datetime'

exec sp_executesql @sql,@paramDefined,
				@IDChiNhanhs_In= @IDChiNhanhs,
				@DateFrom_In = @DateFrom,
				@DateTo_In = @DateTo");

			CreateStoredProcedure(name: "[dbo].[GetLichNhacBaoDuong]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSeach = p.String(),
				NgayBaoDuongFrom = p.DateTime(),
				NgayBaoDuongTo = p.DateTime(),
				NgayNhacFrom = p.DateTime(),
				NgayNhacTo = p.DateTime(),
				IDNhanVienPhuTrachs = p.String(),
				IDNhomHangs = p.String(),
				ID_Xe = p.String(40),
				ID_PhieuTiepNhan = p.String(40),
				LanNhacs = p.String(20),
				TrangThais = p.String(20),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @where1 nvarchar(max), @where2 nvarchar(max), @paramDefined nvarchar(max),
	@sql1 nvarchar(max), @sql2 nvarchar(max), @sqlSub nvarchar(max)
	declare @tblDefined nvarchar(max) = concat(N' declare @tblChiNhanh table (ID uniqueidentifier) ',	
												N' declare @tblNVPhuTrach table (ID_NhanVien uniqueidentifier) ',												
												N' declare @tblNhomHang table (ID uniqueidentifier) ',
												N' declare @tblPhuTung table (ID_HangHoa uniqueidentifier) ')

	

	set @where1 =' where 1 = 1'
	set @where2 =' where 1 = 1'

	if isnull(@CurrentPage,'') ='' 
		set @CurrentPage= 0
	if isnull(@PageSize,'') ='' 
		set @PageSize= 1000
	
	if isnull(@ID_PhieuTiepNhan,'') !=''
	begin
		set @sql1 = concat(N' declare @soKMVao int = 0 ',
		N' select @soKMVao = SoKmVao from Gara_PhieuTiepNhan where ID = @ID_PhieuTiepNhan_In ',
		----- get list phutung theo phieu tiep nhan
		N'insert into @tblPhuTung
		select distinct qd.ID_HangHoa
		from
			(select ct.ID_DonViQuiDoi 
			from BH_HoaDon_ChiTiet ct
			join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
			where hd.ChoThanhToan=0
			and hd.LoaiHoaDon in (3,25)
			and hd.ID_PhieuTiepNhan= @ID_PhieuTiepNhan_In
			and (exists (
				select * from Gara_LichBaoDuong lich where hd.ID= lich.ID_HoaDon)	
				or ct.ID_LichBaoDuong is not null)
			) ct
		join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID')

		set @where2= CONCAT(@where2, 'and not exists (select * from @tblPhuTung pt where lich.ID_HangHoa = pt.ID_HangHoa) ') --- chi lay neu phutung chua dc bao duong
	end

	if isnull(@ID_Xe,'') !=''
	begin
		set @where1 = CONCAT(@where1 , N' and tn.ID_Xe= @IDXe_In')
		set @where2 = CONCAT(@where2 , N' and lich.ID_Xe= @IDXe_In')
	end
	
	if isnull(@IDChiNhanhs,'') !=''
		begin			
			set @sql1 = concat(@sql1, N' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ')		
			set @where1 = CONCAT(@where1 , N' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)')
			
		end
    
	if isnull(@TextSeach,'') !=''
		begin
			set @TextSeach= '%'+ @TextSeach +'%'
			set @where2 = CONCAT(@where2, N' and (MaHangHoa like @TextSeach_In or TenHangHoa like @TextSeach_In or TenHangHoa_KhongDau like @TextSeach_In
				or TenDoiTuong like  @TextSeach_In or MaDoiTuong like @TextSeach_In or TenDoiTuong_KhongDau like @TextSeach_In or DienThoai like @TextSeach_In
				or BienSo like @TextSeach_In)')
		end

	 
	if isnull(@NgayBaoDuongFrom,'') !='' and isnull(@NgayBaoDuongTo,'') !=''
		begin		
			if isnull(@ID_PhieuTiepNhan,'') !=''				
				set @where2 = CONCAT(@where2, N' and (NgayBaoDuongDuKien between @NgayBaoDuongFrom_In and @NgayBaoDuongTo_In ',
					'or (lich.SoKmBaoDuong > 0 and lich.SoKmBaoDuong between @soKMVao - 100 and @soKMVao + 100) )')
			else
				set @where2 = CONCAT(@where2, N' and NgayBaoDuongDuKien between @NgayBaoDuongFrom_In and @NgayBaoDuongTo_In ')
		end

	if isnull(@NgayNhacFrom,'') !='' and isnull(@NgayNhacTo,'') !=''
		begin	
			set @where2 = CONCAT(@where2 , N' and (NgayNhacBatDau is null or 
				(
				(@NgayNhacFrom_In <= NgayNhacBatDau and @NgayNhacTo_In <= NgayNhacKetThuc and  @NgayNhacTo_In >= NgayNhacBatDau)
				or (@NgayNhacFrom_In >= NgayNhacBatDau and @NgayNhacFrom_In <= NgayNhacKetThuc and  @NgayNhacTo_In >= NgayNhacBatDau)
				or (@NgayNhacFrom_In >= NgayNhacBatDau and @NgayNhacTo_In <= NgayNhacKetThuc and  @NgayNhacTo_In >= NgayNhacBatDau)
				or (@NgayNhacFrom_In <= NgayNhacBatDau and  @NgayNhacTo_In >= NgayNhacKetThuc)
				))')				
		end

	if isnull(@IDNhanVienPhuTrachs,'') !=''
		begin						
			set @sql2 = concat(@sql2, N' insert into @tblNVPhuTrach
			select name from dbo.splitstring(@IDNhanVienPhuTrachs_In) ')
			set @where2 = CONCAT(@where2 , N' and exists (select ID from @tblNVPhuTrach nv where ID_NhanVienPhuTrach = nv.ID_NhanVien)')			
		end
	

	if isnull(@IDNhomHangs,'') !=''
		begin						
			set @sql2 = concat(@sql2, N' insert into @tblNhomHang
			select ID from GetListNhomHangHoa(@IDNhomHangs_In) ')
			set @where2 = CONCAT(@where2 , N' and exists (select ID from @tblNhomHang nhomhh where lich.ID_NhomHang = nhomhh.ID)')			
		end

	if isnull(@LanNhacs,'') !=''
		begin						
			set @where2 = CONCAT(@where2 , N' and LanNhac in (select name from dbo.splitstring(@LanNhacs_In))')			
		end
	if isnull(@TrangThais,'') !=''
		begin						
			set @where2 = CONCAT(@where2 , N' and lich.TrangThai in (select name from dbo.splitstring(@TrangThais_In))')			
		end

	set @sqlSub =concat(N'

		select *
		from		
			(select ct.ID_HoaDon, 				
				 hd.NgayLapHoaDon,
				 qd.ID_HangHoa,				 
				ct.LoaiThoiGianBH,
				ct.ThoiGianBaoHanh,
				row_number() over(partition by qd.ID_HangHoa order by hd.NgayLapHoaDon desc) as RN
		 from BH_HoaDon_ChiTiet ct	
		  join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
		  join Gara_PhieuTiepNhan tn on hd.ID_PhieuTiepNhan= tn.ID
		  join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID 
		  ', @where1, ' and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
		  and hd.LoaiHoaDon= 25				 
		 )ctbh where RN= 1 ')

	set @sql1= CONCAT(@sql1, '
		declare @NhacTruoc int = 0 , @LoaiNhacTruoc int = 0, @SoLanNhac int= 0, @LapLai int= 0, @LoaiLapLai int= 0
		select top 1 @NhacTruoc = NhacTruocThoiGian, @LoaiNhacTruoc = NhacTruocLoaiThoiGian,
			@SoLanNhac= SoLanLapLai, @LapLai = SoLanLapLai, @LoaiLapLai= LoaiThoiGianLapLai
		from HT_ThongBao_CatDatThoiGian
		where LoaiThongBao= 4 ')

	 set @sql2 = concat(@sql2, N' 
		  select *,
			case tbl.TrangThai
				when 0 then N''Đã hủy''
				when 1 then N''Chưa xử lý''
				when 2 then N''Đã xử lý''
				when 3 then N''Đã nhắc''
				when 4 then N''Đã hủy''
				when 5 then N''Quá hạn''
			else '''' end as sTrangThai
		  from (
				select *,
					row_number() over (order by NgayBaoDuongDuKien) as Rn,
					COUNT(lich.ID) OVER () as TotalRow
				from
				(
					select 			
						a.*,
						case @LoaiLapLai
								when 1 then DATEADD(MINUTE, @LapLai - 1, a.NgayNhacBatDau)
								when 2 then DATEADD(HOUR,  @LapLai - 1, a.NgayNhacBatDau)
								when 3 then DATEADD(DAY,  @LapLai - 1, a.NgayNhacBatDau)
								when 4 then DATEADD(MONTH,  @LapLai - 1, a.NgayNhacBatDau)
								when 5 then DATEADD(YEAR,  @LapLai - 1, a.NgayNhacBatDau)
						end as NgayNhacKetThuc
					from
					(
							select								
							lich.ID,
							lich.ID_HangHoa,
							lich.ID_Xe,
							qd.MaHangHoa,
							hh.TenHangHoa,
							lich.LanBaoDuong,
							lich.SoKmBaoDuong,
							lich.NgayBaoDuongDuKien,
							iif(lich.TrangThai=4,0, lich.TrangThai) as TrangThai,
							lich.GhiChu,
							isnull(lich.LanNhac,0) as LanNhac,
							dt.ID as ID_DoiTuong,
							dt.TenDoiTuong,
							dt.MaDoiTuong,
							dt.DienThoai,
							dt.Email,
							dt.TenDoiTuong_KhongDau,
							xe.BienSo,
							hh.ID_NhomHang,
							nhom.TenNhomHangHoa,	
							hh.TenHangHoa_KhongDau,							
							case @LoaiNhacTruoc
								when 1 then DATEADD(MINUTE, - @NhacTruoc, lich.NgayBaoDuongDuKien)
								when 2 then DATEADD(HOUR, - @NhacTruoc, lich.NgayBaoDuongDuKien)
								when 3 then DATEADD(DAY, - @NhacTruoc, lich.NgayBaoDuongDuKien)
								when 4 then DATEADD(MONTH, - @NhacTruoc, lich.NgayBaoDuongDuKien)
								when 5 then DATEADD(YEAR, - @NhacTruoc, lich.NgayBaoDuongDuKien)
							end as NgayNhacBatDau,
						case when ct.ThoiGianBaoHanh !=0 then
						case ct.LoaiThoiGianBH
							when 1 then CONCAT(ct.ThoiGianBaoHanh, N'' ngày'')
							when 2 then CONCAT(ct.ThoiGianBaoHanh, N'' tháng'')
							when 3 then CONCAT(ct.ThoiGianBaoHanh, N'' năm'')
						else N''Không bảo hành'' end end as sThoiGianBaoHanh,
						case when ct.ThoiGianBaoHanh !=0
						 then
							case ct.LoaiThoiGianBH
							when 1 then DATEADD(DAY, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
							when 2 then DATEADD(MONTH, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
							when 3 then DATEADD(YEAR, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
						end end as HanBaoHanh
					 from Gara_LichBaoDuong lich
					 join DM_HangHoa hh on lich.ID_HangHoa= hh.ID	',
					 'left join ( ', @sqlSub, 
					' ) ct on ct.ID_HangHoa= lich.ID_HangHoa
					  join DonViQuiDoi qd on ct.ID_HangHoa = qd.ID_HangHoa and lich.ID_HangHoa= qd.ID_HangHoa and qd.LaDonViChuan= 1 
					  join Gara_DanhMucXe xe on lich.ID_Xe = xe.ID
					  left join DM_DoiTuong dt on xe.ID_KhachHang = dt.ID	
					  left join DM_NhomHangHoa nhom on hh.ID_NhomHang = nhom.ID
					) a
			) lich
	  ', @where2 , ') tbl where Rn between (@CurrentPage_In * @PageSize_In) + 1 and @PageSize_In * (@CurrentPage_In + 1) ')
	  

	set @sql2 = CONCAT(@tblDefined, @sql1, @sql2)

	

	set @paramDefined = N' @IDChiNhanhs_In nvarchar(max),
								@TextSeach_In nvarchar(max),
								@NgayBaoDuongFrom_In datetime,
								@NgayBaoDuongTo_In datetime,
								@NgayNhacFrom_In datetime, 
								@NgayNhacTo_In datetime,
								@IDNhanVienPhuTrachs_In nvarchar(max),	
								@IDNhomHangs_In nvarchar(max),
								@IDXe_In varchar(40),
								@ID_PhieuTiepNhan_In varchar(40),
								@LanNhacs_In varchar(20),
								@TrangThais_In varchar(20),
								@CurrentPage_In int,
								@PageSize_In int'

					---	print @sql2

	exec sp_executesql @sql2, 
		@paramDefined,
		@IDChiNhanhs_In = @IDChiNhanhs,
		@TextSeach_In = @TextSeach,
		@NgayBaoDuongFrom_In = @NgayBaoDuongFrom,
		@NgayBaoDuongTo_In = @NgayBaoDuongTo,
		@NgayNhacFrom_In = @NgayNhacFrom, 
		@NgayNhacTo_In = @NgayNhacTo,
		@IDNhanVienPhuTrachs_In = @IDNhanVienPhuTrachs,	
		@IDNhomHangs_In = @IDNhomHangs,
		@IDXe_In = @ID_Xe,
		@ID_PhieuTiepNhan_In = @ID_PhieuTiepNhan,
		@LanNhacs_In = @LanNhacs,
		@TrangThais_In = @TrangThais,
		@CurrentPage_In = @CurrentPage,
		@PageSize_In = @PageSize");

			CreateStoredProcedure(name: "[dbo].[GetListComBo_ofCTHD]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid(),
				IDChiTiet = p.String()
			}, body: @"SET NOCOUNT ON;
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
    				CAST(SoThuTu AS float) AS SoThuTu,ct.ID_KhuyenMai, ISNULL(ct.TangKem,'0') as TangKem, ct.ID_TangKem,
					-- replace char enter --> char space
    				(REPLACE(REPLACE(TenHangHoa,CHAR(13),''),CHAR(10),'') +
    				CASE WHEN (qd.ThuocTinhGiaTri is null or qd.ThuocTinhGiaTri = '') then '' else '_' + qd.ThuocTinhGiaTri end +
    				CASE WHEN TenDonVitinh = '' or TenDonViTinh is null then '' else ' (' + TenDonViTinh + ')' end +
    				CASE WHEN MaLoHang is null then '' else '. Lô: ' + MaLoHang end) as TenHangHoaFull,
    				
    				hh.ID AS ID_HangHoa,LaHangHoa,QuanLyTheoLoHang,TenHangHoa, 
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
					ThoiGian,ct.ThoiGianHoanThanh, ISNULL(hh.GhiChu,'') as GhiChuHH,
					ISNULL(ct.DiemKhuyenMai,0) as DiemKhuyenMai,
					ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
					ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
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
    	) ctt on tbl.ID = ctt.ID_ChiTietGoiDV");

			CreateStoredProcedure(name: "[dbo].[GetListNhatKyBaoDuongTheoXe]", parametersAction: p => new
			{
				IdXe = p.Guid(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	with data_cte
	AS
	(
	select hdct.ID, hd.MaHoaDon, hd.NgayLapHoaDon, ptn.SoKmVao, dvqd.MaHangHoa, hh.TenHangHoa, hdct.SoLuong, lbd.LanBaoDuong, lbd.TrangThai from Gara_LichBaoDuong lbd
	INNER JOIN BH_HoaDon_ChiTiet  hdct ON hdct.ID_LichBaoDuong = lbd.ID
	INNER JOIN BH_HoaDon hd ON hd.ID = hdct.ID_HoaDon
	INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
	INNER JOIN DM_HangHoa hh ON dvqd.ID_HangHoa = hh.ID
	INNER JOIN Gara_PhieuTiepNhan ptn ON hd.ID_PhieuTiepNhan = ptn.ID
	WHERE lbd.TrangThai IN (2, 5)
	AND lbd.ID_Xe = @IdXe
	and hd.ChoThanhToan IS NOT NULL
	AND hd.LoaiHoaDon = 25),
    count_cte
    as
    (
    	select count(ID) as TotalRow,
    		CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    	from data_cte
    )
    SELECT dt.*, ct.* FROM data_cte dt
    CROSS JOIN count_cte ct
    ORDER BY dt.NgayLapHoaDon desc, dt.MaHoaDon, dt.MaHangHoa desc
    OFFSET (@CurrentPage * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY;");

			CreateStoredProcedure(name: "[dbo].[GetListThongBao]", parametersAction: p => new
			{
				IdDonVi = p.Guid(),
				IdNguoiDung = p.String(),
				NhacSinhNhat = p.Boolean(),
				NhacTonKho = p.Boolean(),
				NhacDieuChuyen = p.Boolean(),
				NhacLoHang = p.Boolean(),
				NhacBaoDuong = p.Boolean(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @tblCaiDat TABLE (GiaTri INT);
	IF @NhacBaoDuong = 1
	BEGIN
		INSERT INTO @tblCaiDat VALUES (4);
	END
	IF @NhacDieuChuyen = 1
	BEGIN
		INSERT INTO @tblCaiDat VALUES (1);
	END
	IF @NhacLoHang = 1
	BEGIN
		INSERT INTO @tblCaiDat VALUES (2);
	END
	IF @NhacSinhNhat = 1
	BEGIN
		INSERT INTO @tblCaiDat VALUES (3);
	END
	IF @NhacTonKho = 1
	BEGIN
		INSERT INTO @tblCaiDat VALUES (0);
	END

	DECLARE @CountChuaDoc INT;
	SELECT @CountChuaDoc = COUNT(ID) FROM HT_ThongBao tb
	INNER JOIN @tblCaiDat cd ON tb.LoaiThongBao = cd.GiaTri
	WHERE ID_DonVi = @IdDonVi
	AND NguoiDungDaDoc NOT LIKE '%' + @IdNguoiDung + '%'

	;with data_cte
	AS
	(SELECT tb.ID, tb.ID_DonVi, tb.LoaiThongBao, tb.NoiDungThongBao, tb.NgayTao, tb.NguoiDungDaDoc FROM HT_ThongBao tb
	INNER JOIN @tblCaiDat cd ON tb.LoaiThongBao = cd.GiaTri
	WHERE ID_DonVi = @IdDonVi),
	count_cte
	as
	(
		select count(ID) as TotalRow,
    		CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
		from data_cte
	)
	SELECT dt.*, @CountChuaDoc AS ChuaDoc FROM data_cte dt
	CROSS JOIN count_cte ct
	ORDER BY dt.NgayTao desc
	OFFSET (@CurrentPage * @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY;");

			CreateStoredProcedure(name: "[dbo].[GetNhatKyGiaoDich_ofCus]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				IDCustomers = p.String(),
				IDCars = p.String(),
				LoaiHoaDons = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @sql nvarchar(max) ='', @where nvarchar(max), @paramDefined nvarchar(max)
	declare @sqlSoQuy nvarchar(max), @where2 nvarchar(max),  @sqlHDTra nvarchar(max)
	declare @tblDefined nvarchar(max)= N' declare @tblChiNhanh table(ID uniqueidentifier)
								declare @tblCus table(ID uniqueidentifier)
								declare @tblCar table(ID uniqueidentifier)'

	set @where = N' where 1 = 1 '
	set @where2 = N' where 1 = 1  '

	if isnull(@CurrentPage,'') =''
		set @CurrentPage = 0
	if isnull(@PageSize,'') =''
		set @PageSize = 20

	if isnull(@LoaiHoaDons,'') =''
		set @LoaiHoaDons = '1,19,15'
	if isnull(@LoaiHoaDons,'') !=''
	begin
		set @where = CONCAT(@where , ' and hd.LoaiHoaDon in (select name from dbo.splitstring(@LoaiHoaDons_In))')		
	end
	if isnull(@IDChiNhanhs,'') !=''
		begin
			set @where = CONCAT(@where , ' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ;')			
		end
	if isnull(@IDCustomers,'') !=''
		begin
			set @where = CONCAT(@where , ' and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblCus select name from dbo.splitstring(@IDCustomers_In) ;')
			set @where2 = CONCAT(@where2 , ' and exists (select ID from @tblCus cus where ID_DoiTuong = cus.ID)')
		end
	
	if isnull(@IDCars,'') !=''
		begin
			set @where = CONCAT(@where , ' and exists (select ID from @tblCar car where hd.ID_Xe = car.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblCar select name from dbo.splitstring(@IDCars_In) ;')
		end

		set @sqlSoQuy = CONCAT(N'
				select qct.ID_HoaDonLienQuan,
					SUM(qct.TienThu) as TongTienThu,
					max(qhd.MaHoaDon) as MaPhieuThu
				into #tmpThuChi
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				', @where2, 'and (qhd.TrangThai = 1 or qhd.TrangThai is null) 
    			group by qct.ID_HoaDonLienQuan; ')

		set @sqlHDTra = CONCAT(N'
			select hdt.ID_HoaDon, 
    			SUM(hdt.TongTienHang) as TongTienHang,
    			SUM(hdt.TongGiamGia) as TongGiamGia ,
    			SUM(hdt.KhuyeMai_GiamGia) as KhuyeMai_GiamGia, 
    			SUM(hdt.PhaiThanhToan) as PhaiThanhToan,
				SUM(ISNULL(thuchi.TongTienThu,0)) as TongTienThu
    	from BH_HoaDon hdt	
		left join #tmpThuChi thuchi on thuchi.ID_HoaDonLienQuan = hdt.ID
		', @where2, ' and hdt.LoaiHoaDon= 6 and hdt.ChoThanhToan = 0 group by hdt.ID_HoaDon ' )


		set @sql = CONCAT(@tblDefined, @sql, @sqlSoQuy ,  '
		with data_cte
		as (
				select hd.MaHoaDon, 
					hd.ID,
					hd.ID_Xe, 
					xe.BienSo,
    				hd.NgayLapHoaDon,
					hd.PhaiThanhToan as TongMua,
					HDTra.PhaiThanhToan as TongTra,
					ISNULL(thuchi.MaPhieuThu,'''') as MaPhieuThu,
					hd.PhaiThanhToan - ISNULL(thuchi.TongTienThu,0) - (ISNULL(HDTra.PhaiThanhToan,0) - ISNULL(HDTra.TongTienThu,0)) as ConNo,
					-- PhaiTT of GDV = MuaGoi - TraGoi
    				hd.PhaiThanhToan - ISNULL(HDTra.PhaiThanhToan,0) as PhaiThanhToan,
					-- chỉ trừ TongTienThu của HDTra nếu khi Trả hàng, mình chi tiền cho khách
					ISNULL(thuchi.TongTienThu,0) - ISNULL(HDTra.TongTienThu,0) as DaThanhToan,  
					hd.DienGiai as GhiChu   		
    	from BH_HoaDon hd
		left join Gara_DanhMucXe xe on hd.ID_Xe= xe.ID
		left join #tmpThuChi thuchi on thuchi.ID_HoaDonLienQuan = hd.ID
		left join ( ', @sqlHDTra , ') HDTra on hd.ID= HDTra.ID_HoaDon ',
		@where, 'and hd.ChoThanhToan = 0 ),
		count_cte
		as (
			select count(ID) as TotalRow,
				sum(TongMua) as SumTongMua,
				sum(TongTra) as SumTongTra,							
				sum(PhaiThanhToan) as SumPhaiThanhToan,
				sum(DaThanhToan) as SumDaThanhToan,
				sum(ConNo) as SumConNo
			from data_cte
		)
    	select dt.*,			
		cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage_In * @PageSize_In) ROWS
		FETCH NEXT @PageSize_In ROWS ONLY 
		')

	set @paramDefined =N'
			@IDChiNhanhs_In nvarchar(max) = null,
			@IDCustomers_In nvarchar(max) = null,
			@IDCars_In nvarchar(max) = null,
			@LoaiHoaDons_In nvarchar(max) = null,
			@CurrentPage_In int =null,
			@PageSize_In int =null'

			print @sql


		exec sp_executesql @sql, 
		@paramDefined,
		@IDChiNhanhs_In = @IDChiNhanhs,
		@IDCustomers_In = @IDCustomers,
		@IDCars_In = @IDCars,
		@LoaiHoaDons_In = @LoaiHoaDons,
		@CurrentPage_In = @CurrentPage,
		@PageSize_In = @PageSize");

			CreateStoredProcedure(name: "[dbo].[GetNhatKyBaoDuong_byCar]", parametersAction: p => new
			{
				ID_Xe = p.Guid()
			}, body: @"SET NOCOUNT ON;

		 select hd.MaHoaDon, 				
				 hd.NgayLapHoaDon,
				 qd.MaHangHoa,
				 qd.TenDonViTinh,
				hh.TenHangHoa,
				dt.MaDoiTuong,
				dt.TenDoiTuong,
				ct.SoLuong,
				ct.GhiChu,
				lich.LanBaoDuong,
				lich.SoKmBaoDuong,
				lich.TrangThai,
				case lich.TrangThai
					when 0 then N'Đã hủy'
					when 1 then N'Chưa xử lý'
					when 2 then N'Đã xử lý'
					when 3 then N'Đã nhắc'
					when 4 then N'Đã hủy'
					when 5 then N'Quá hạn'
				end as sTrangThai,
				nv.NVThucHiens
		 from BH_HoaDon_ChiTiet ct	
		  join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
		  join Gara_PhieuTiepNhan tn on hd.ID_PhieuTiepNhan= tn.ID
		  left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
		  join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID 
		  join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		  join Gara_LichBaoDuong lich on ct.ID_LichBaoDuong= lich.ID
		  left join 
		  (
			select distinct thout.ID_ChiTietHoaDon,
			 (
			  select distinct nv.TenNhanVien + ', ' as [text()]
			  from BH_NhanVienThucHien th
			  join NS_NhanVien nv on th.ID_NhanVien= nv.ID
			  where th.ID_ChiTietHoaDon = thout.ID_ChiTietHoaDon
			  for xml path('')
			  ) NVThucHiens
			  from BH_NhanVienThucHien thout
		  ) nv on ct.ID = nv.ID_ChiTietHoaDon
		  where (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
		  and hd.LoaiHoaDon= 25
		  and hd.ChoThanhToan= 0
		  and tn.ID_Xe= @ID_Xe
		  and ct.ID_LichBaoDuong is not null
		  order by hd.NgayLapHoaDon	desc, qd.MaHangHoa");

			CreateStoredProcedure(name: "[dbo].[GetNhatKySuDung_GDV]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				IDCustomers = p.String(),
				IDCars = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @sql nvarchar(max) ='', @where nvarchar(max), @paramDefined nvarchar(max)
	declare @tblDefined nvarchar(max)= N' declare @tblChiNhanh table(ID uniqueidentifier)
								declare @tblCus table(ID uniqueidentifier)
								declare @tblCar table(ID uniqueidentifier)'

	set @where = N' where 1 = 1 and hd.ChoThanhToan =0 and ct.ChatLieu= 4 and ct.ID_ChiTietGoiDV is not null and (ct.ID_ChiTietDinhLuong= ct.id OR ct.ID_ChiTietDinhLuong IS NULL) '

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
	
	if isnull(@IDCars,'') !=''
		begin
			set @where = CONCAT(@where , ' and exists (select ID from @tblCar car where hd.ID_Xe = car.ID)')
			set @sql = CONCAT(@sql, ' insert into @tblCar select name from dbo.splitstring(@IDCars_In) ;')
		end
	
	set @sql = CONCAT(@tblDefined, @sql, N'
		with data_cte
as (
    SELECT hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon,
    	qd.MaHangHoa, hh.TenHangHoa, 
		hh.TenHangHoa_KhongDau,
		ct.SoLuong,
    	hdXMLOut.HDCT_NhanVien as NhanVienThucHien,
    	CT_ChietKhauNV.TongChietKhau
    	FROM BH_HoaDon_ChiTiet ct
    	join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.id
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    	left join 
    			(Select distinct hdXML.ID,
    					(
    					select distinct (nv.TenNhanVien) +'', ''  AS [text()]
    					from BH_HoaDon_ChiTiet ct2
    					left join BH_NhanVienThucHien nvth on ct2.ID = nvth.ID_ChiTietHoaDon
    					left join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    					where ct2.ID = hdXML.ID 
    					For XML PATH ('''')
    				) HDCT_NhanVien
    			from BH_HoaDon_ChiTiet hdXML) hdXMLOut on ct.ID= hdXMLOut.ID
    	 left join 
    			(select ct3.ID, SUM(isnull(nvth2.TienChietKhau,0)) as TongChietKhau from BH_HoaDon_ChiTiet ct3
    			left join BH_NhanVienThucHien nvth2 on ct3.ID = nvth2.ID_ChiTietHoaDon
    			group by ct3.ID) CT_ChietKhauNV on CT_ChietKhauNV.ID = ct.ID        
    	', @where, 
		'),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize_In as float ))  as TotalPage,
				sum(SoLuong) as TongSoLuong,
				sum(TongChietKhau) as TongHoaHong			
			from data_cte
		)
    	select dt.*,
		cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage_In * @PageSize_In) ROWS
		FETCH NEXT @PageSize_In ROWS ONLY ')

		print @sql

		set @paramDefined =N'
			@IDChiNhanhs_In nvarchar(max),
			@IDCustomers_In nvarchar(max),
			@IDCars_In nvarchar(max),
			@CurrentPage_In int,
			@PageSize_In int'

		exec sp_executesql @sql, 
		@paramDefined,
		@IDChiNhanhs_In = @IDChiNhanhs,
		@IDCustomers_In = @IDCustomers,
		@IDCars_In = @IDCars,
		@CurrentPage_In = @CurrentPage,
		@PageSize_In = @PageSize");

			CreateStoredProcedure(name: "[dbo].[GetTPDinhLuong_ofHoaDon]", parametersAction: p => new
			{
				ID_HoaDons = p.String()
			}, body: @"SET NOCOUNT ON;
	declare @tblHoaDon table (ID uniqueidentifier)
	insert into @tblHoaDon
	select name from dbo.splitstring(@ID_HoaDons)


	---- get tpdl
	select ID 
	into #tblIDCT 
	from BH_HoaDon_ChiTiet ct
	where exists (select ID from @tblHoaDon hd where ct.ID_HoaDon = hd.ID)

    select ct.ID_HoaDon, 
			ct.ID as ID_ChiTietGoiDV, --- used to combo long combo (tpldl la combo)
			ct.ID_DonViQuiDoi,
			ct.ID_LoHang, ct.ID_ChiTietDinhLuong,
			ct.SoLuong,
			qd.GiaBan, --- used to combo long combo (tpldl la combo)
			isnull(gv.GiaVon, 0) as GiaVon,
			MaHangHoa, TenHangHoa,  TenDonViTinh, 
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoaThayThe,
			hh.LaHangHoa,
			ct.GhiChu,
			SoLuongDinhLuong_BanDau,
    		qd.TenDonViTinh as DonViTinhQuyCach, 
    		case when ISNULL(QuyCach,0) = 0 then ISNULL(TyLeChuyenDoi,1) else ISNULL(QuyCach,0) * ISNULL(TyLeChuyenDoi,1) end as QuyCach,
			ct.SoLuong - isnull(ctt.SoLuongDung,0) - isnull(ctt.SoLuongTra,0)  as SoLuongConLai
    	from BH_HoaDon_ChiTiet ct
		join BH_HoaDon hdm on ct.ID_HoaDon= hdm.ID
    	Join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
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
				and exists (select ID from #tblIDCT ctm where ct.ID_ChiTietGoiDV = ctm.ID)
    			group by ct.ID_ChiTietGoiDV
    
    			union all
    			-- sum soluong sudung
    			select ct.ID_ChiTietGoiDV,
    				0 as SoLuongDung,
    				SUM(ct.SoLuong) as SoLuongDung
    			from BH_HoaDon_ChiTiet ct 
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    			where hd.ChoThanhToan=0 and hd.LoaiHoaDon in (1,25)
    			and exists (select ID from #tblIDCT ctm where ct.ID_ChiTietGoiDV = ctm.ID)
				and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			group by ct.ID_ChiTietGoiDV
    			) a group by a.ID_ChiTietGoiDV
    	) ctt on ct.ID = ctt.ID_ChiTietGoiDV
		left join DM_GiaVon gv on ct.ID_DonViQuiDoi= gv.ID_DonViQuiDoi and hdm.ID_DonVi= gv.ID_DonVi
			and (ct.ID_LoHang= gv.ID_LoHang or ct.ID_LoHang is null)
    	where ct.ID_ChiTietDinhLuong is not null
		and (ct.ID_ChiTietDinhLuong != ct.ID)
		and ct.SoLuong > 0
		and exists (select ID from @tblHoaDon hd where hdm.id = hd.ID)");

			CreateStoredProcedure(name: "[dbo].[HDTraHang_InsertTPDinhLuong]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid()
			}, body: @"SET NOCOUNT ON;
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

		exec UpdateTonLuyKeCTHD_whenUpdate @ID_HoaDon, @ID_DonVi, @NgayLapHoaDon
		exec UpdateGiaVon_WhenEditCTHD @ID_HoaDon, @ID_DonVi, @NgayLapHoaDon
	end	");

			CreateStoredProcedure(name: "[dbo].[Insert_LichNhacBaoDuong]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid()
			}, body: @"SET NOCOUNT ON	

	declare @dtNow datetime = format(DATEADD(day,-30, getdate()),'yyyy-MM-dd')

	declare @SoKmMacDinhNgay int= 10, @countSC int =0;
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
	order by NgayVaoXuong desc

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
						when 2 then pt.SoKmBaoDuong + pt.GiaTriLap --- lay sokmbaoduong o lancuoicung (cua lichbaoduong) + giatrilap
						else 0 end as SoKmBaoDuong,
					case pt.LoaiBaoDuong
						when 2 then DATEADD(day, CEILING( pt.GiaTriLap/@SoKmMacDinhNgay), @NgayLapHoaDon) --- get số ngày theo km mặc định + thời gian tiếp nhận
						when 1 then DATEADD(day, pt.GiaTriLap, @NgayLapHoaDon)									
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
			) a where a.NgayBaoDuongDuKien >= @dtNow --- chi insert neu lich > ngayhientai");

			CreateStoredProcedure(name: "[dbo].[InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac]", parametersAction: p => new
			{
				ID_LichBaoDuong = p.Guid(),
				TypeUpdate = p.Int()
			}, body: @"SET NOCOUNT ON;
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
		declare @SoKmMacDinhNgay int= 10, @countSC int =0;
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
		order by NgayVaoXuong desc

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
	drop table #tblSetup");

			CreateStoredProcedure(name: "[dbo].[SuDungBaoDuong_UpdateStatus]", parametersAction: p => new
			{
				IDLichNhacs = p.String(),
				Status = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @tblLich table(ID uniqueidentifier)
	insert into @tblLich
	select Name from dbo.splitstring(@IDLichNhacs)

	update lich set lich.TrangThai= @Status, 
		lich.LanNhac= iif(@Status =3 and (lich.LanNhac = 0 or lich.LanNhac is null),1, lich.LanNhac) --- neu trangthai= danhac va solannhac =0, set LanNhac = 1
	from Gara_LichBaoDuong lich
	where exists (select id from @tblLich tbl where lich.ID= tbl.ID)

	if @Status = 2 or @Status = 5
		---- capnhat lichnhac cho nhung lan truoc do --> quá hạn (TrangThai = 5)
		---- lay lich same ID_hanghoa, same id_xe
		update lichnew set lichnew.TrangThai= 5
		from Gara_LichBaoDuong lichnew
		where exists (
			select lich1.ID 
			from Gara_LichBaoDuong lich1
			join(
				select lich.*
				from Gara_LichBaoDuong lich
				join @tblLich tbl on lich.ID= tbl.ID
			) b on lich1.ID_Xe= b.ID_Xe and lich1.ID_HangHoa= b.ID_HangHoa
			where lich1.LanBaoDuong < b.LanBaoDuong
			and lich1.TrangThai in (1,3)
			and lichnew.ID= lich1.ID
		)");

			CreateStoredProcedure(name: "[dbo].[UpdateHD_UpdateLichBaoDuong]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid(),
				IDHangHoas = p.String(),
				NgayLapHDOld = p.DateTime()
			}, body: @"SET NOCOUNT ON;

	declare @dtNow datetime = format( getdate(),'yyyy-MM-dd')
	declare @SoKmMacDinhNgay int= 10, @countSC int =0;

	declare @NgayLapHoaDon datetime , @ID_PhieuTiepNhan uniqueidentifier, @SoKmGanNhat float, @ID_Xe uniqueidentifier, @Now_SoKmVao int,  @Now_NgayVaoXuong datetime
	select @ID_PhieuTiepNhan = ID_PhieuTiepNhan, @NgayLapHoaDon = NgayLapHoaDon from BH_HoaDon where id= @ID_HoaDon

	---- get thongtin tiepnhan hientai
	select @ID_Xe= ID_Xe, @Now_SoKmVao = isnull(SoKmVao,0), @Now_NgayVaoXuong = NgayVaoXuong from Gara_PhieuTiepNhan where ID = @ID_PhieuTiepNhan

	---- get thongtin tiepnhan gan nhat
	declare @NgayXuatXuong_GanNhat datetime , @SoKmRa_GanNhat float
	select top 1 @NgayXuatXuong_GanNhat = isnull(NgayXuatXuong, NgayVaoXuong) ,  @SoKmRa_GanNhat= ISNULL(SoKmRa,0) 
	from Gara_PhieuTiepNhan where isnull(NgayXuatXuong, NgayVaoXuong) < @Now_NgayVaoXuong and ID_Xe= @ID_Xe 
	order by NgayVaoXuong desc

	
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
			) a where a.NgayBaoDuongDuKien >= @dtNow --- chi insert neu lich > ngayhientai");

			CreateStoredProcedure(name: "[dbo].[UpdateLichBD_whenChangeNgayLapHD]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid(),
				NgayLapHDOld = p.DateTime(),
				NgayLapNew = p.DateTime()
			}, body: @"SET NOCOUNT ON;
    declare @chenhlech int= DATEDIFF(day, @NgayLapHDOld, @NgayLapNew)
	if @chenhlech!=0
		begin
			
			--- update if lich exist
			update lich set lich.NgayBaoDuongDuKien= DATEADD(day, @chenhlech,lich.NgayBaoDuongDuKien)			
			from Gara_LichBaoDuong lich
			where lich.ID_HoaDon= @ID_HoaDon
			and lich.TrangThai = 1
			
			-- insert if not exist
			exec Insert_LichNhacBaoDuong @ID_HoaDon		

		end");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_HangBanTheoDoanhThu]
	@IDChiNhanhs nvarchar(max)  = null,
    @DateFrom [datetime] = null,
    @DateTo [datetime]  = null
AS
BEGIN

	set nocount on
	declare @tblChiNhanh table(ID uniqueidentifier) 
	if isnull(@IDChiNhanhs,'')!=''
		begin
			insert into @tblChiNhanh 
			select name from dbo.splitstring(@IDChiNhanhs)
		end

		SELECT TOP(10)
		a.MaHangHoa,
		a.TenHangHoa,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
    		SELECT
			dvqd.MaHangHoa, 
			hh.TenHangHoa,
    		hdct.ThanhTien as ThanhTien
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon 
			join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hdb.ChoThanhToan = 0    	
			and exists (select cn.ID from @tblChiNhanh cn where hdb.ID_DonVi= cn.ID)    
			and hdb.NgayLapHoaDon between @DateFrom and @DateTo
			and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			and (hdct.ID_ParentCombo = hdct.ID or hdct.ID_ParentCombo is null)
			and hdb.LoaiHoaDon in (1,19,25)
			and hdct.ChatLieu !=4
		) a
    	GROUP BY a.MaHangHoa, a.TenHangHoa
		ORDER BY ThanhTien DESC 				
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_HangBanTheoSoLuong]
    @IDChiNhanhs nvarchar(max) = null,
    @DateFrom [datetime] = null,
    @DateTo [datetime] = null
AS
BEGIN

	set nocount on

	declare @tblChiNhanh table(ID uniqueidentifier) 
	if isnull(@IDChiNhanhs,'')!=''
		begin
			insert into @tblChiNhanh 
			select name from dbo.splitstring(@IDChiNhanhs)
		end

	
		SELECT TOP(10)
		a.MaHangHoa,
		a.TenHangHoa,
		CAST(ROUND(SUM(a.SoLuong), 0) as float) as SoLuong
		FROM
		(
    		SELECT
			dvqd.MaHangHoa, 
			hh.TenHangHoa,
    		hdct.SoLuong as SoLuong
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon
			join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		where hdb.ChoThanhToan = 0    		
			and exists (select cn.ID from @tblChiNhanh cn where hdb.ID_DonVi= cn.ID)
			and hdb.NgayLapHoaDon between @DateFrom and @DateTo
			and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			and (hdct.ID_ParentCombo = hdct.ID or hdct.ID_ParentCombo is null)
    		and hdb.LoaiHoaDon in (1,19,25)
			and hdct.ChatLieu != 4
		) a
    	GROUP BY a.MaHangHoa, a.TenHangHoa
		ORDER BY SoLuong DESC 

		
					
END");

			Sql(@"ALTER PROCEDURE [dbo].[CTHD_GetDichVubyDinhLuong]
    @ID_HoaDon [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @ID_LoHang [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;	
    
    			select 
    				ctsc.ID_ChiTietGoiDV, --- ~ id of thanhphan
    				ctsc.ID_ChiTietDinhLuong, --- ~ id_quidoi of dichvu
    				ctsc.SoLuong,
    				ctsc.SoLuong - isnull(ctxk.SoLuongXuat,0) as SoLuongConLai,
    				ctsc.ID_DonViQuiDoi, ---- ~ id hanghoa,
    				ctsc.ID_LoHang,
    				hh.QuanLyTheoLoHang,
    				hh.LaHangHoa,
    				hh.DichVuTheoGio,
    				hh.DuocTichDiem,
    				qd.GiaBan,
    				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    				qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,
    				ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, 
    				CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
    				hh.LaHangHoa, 
					hh.TenHangHoa, 
					hh.TenHangHoa as TenHangHoaThayThe,
    				hh.ID_NhomHang as ID_NhomHangHoa, ISNULL(hh.GhiChu,'') as GhiChuHH,
    				ctsc.IDChiTietDichVu, ---- ~ id chitietHD of dichvu
					ctsc.ChatLieu
    			from 
    			(
    				--- ct hoadon suachua
    				select 
    					cttp.ID as ID_ChiTietGoiDV,
						cttp.ChatLieu,
    					isnull(ctdv.ID_DichVu,cttp.ID_DonViQuiDoi) as ID_ChiTietDinhLuong, -- id_hanghoa/id_dichvu
    					cttp.SoLuong,
    					cttp.ID_DonViQuiDoi,
    					cttp.ID_LoHang,
    					cttp.ID_ChiTietDinhLuong AS IDChiTietDichVu --- used to xuất kho hàng ngoài
    				from BH_HoaDon_ChiTiet cttp
    				left join
    				(
    					select ctm.ID_DonViQuiDoi as ID_DichVu, ctm.ID
    					from BH_HoaDon_ChiTiet ctm where ctm.ID_HoaDon= @ID_HoaDon
    				) ctdv on cttp.ID_ChiTietDinhLuong = ctdv.ID
    				where cttp.ID_DonViQuiDoi = @ID_DonViQuiDoi
    				and ((cttp.ID_LoHang = @ID_LoHang) or (cttp.ID_LoHang is null and @ID_LoHang is null))
    				and cttp.ID_HoaDon= @ID_HoaDon
    			) ctsc
    			left join
    			(
    					---- ct xuatkho 
    				select sum(ct.SoLuong) as SoLuongXuat,
    					ct.ID_ChiTietGoiDV
    				from BH_HoaDon_ChiTiet ct
    				join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    				where hd.ChoThanhToan='0' and hd.LoaiHoaDon=8
    				and hd.ID_HoaDon= @ID_HoaDon
    				and ct.ID_DonViQuiDoi= @ID_DonViQuiDoi 
    				and ((ct.ID_LoHang = @ID_LoHang) or (ct.ID_LoHang is null and @ID_LoHang is null))
    				group by ct.ID_ChiTietGoiDV
    			) ctxk on ctsc.ID_ChiTietGoiDV = ctxk.ID_ChiTietGoiDV
    			join DonViQuiDoi qd on ctsc.ID_ChiTietDinhLuong= qd.ID
    			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID	
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetAllBangLuongChiTietById]
    @id [uniqueidentifier],
	@CurrentPage int,
	@PageSize int
AS
BEGIN
    SET NOCOUNT ON;

	with data_cte
	as (
    	SELECT 
			bl.ID as ID_BangLuong_ChiTiet,
			bl.MaBangLuongChiTiet,
			nv.ID as ID_NhanVien,
			nv.DaNghiViec,
			nv.MaNhanVien,
			nv.TenNhanVien,
			bl.NgayCongThuc,
			bl.NgayCongChuan,
			bl.LuongCoBan,
			round(bl.TongLuongNhan,0) as LuongChinh, 
			round(bl.LuongOT,0) as LuongOT,
			bl.PhuCapCoBan,
			round(bl.PhuCapKhac,0) as PhuCapKhac,
			bl.KhenThuong,
			bl.KyLuat,
			round(bl.ChietKhau,0) as ChietKhau,    					
			round(bl.TongLuongNhan +  bl.LuongOT + bl.PhuCapCoBan + bl.PhuCapKhac + ChietKhau,0)  as LuongTruocGiamTru,
			round(bl.TongTienPhat,0) as TongTienPhat,
    		round(bl.LuongThucNhan,0)  as LuongSauGiamTru,
			isnull(quyhd.TruTamUngLuong,0) as TruTamUngLuong,
			isnull(quyhd.TienThu,0) as ThanhToan,
			isnull(quyhd.TienThu,0) + isnull(quyhd.TruTamUngLuong,0) as DaTra ,
			round(bl.LuongThucNhan - isnull(quyhd.TienThu,0) - isnull(quyhd.TruTamUngLuong,0),0) as ConLai,
			quyhd.NgayThanhToan
    	 from NS_BangLuong_ChiTiet bl
    	 join NS_NhanVien nv on bl.ID_NhanVien =nv.ID			
		 left join ( select qct.ID_BangLuongChiTiet,
						max(qhd.NgayLapHoaDon) as NgayThanhToan,
						sum( isnull(qct.TruTamUngLuong,0)) as TruTamUngLuong,
						sum(qct.TienThu) as TienThu
				 from Quy_HoaDon_ChiTiet qct 
				 join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 
				 where qhd.TrangThai = 1
				 group by qct.ID_BangLuongChiTiet) quyhd on bl.ID = quyhd.ID_BangLuongChiTiet		 
    	 where bl.ID_BangLuong=@id 
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
				sum(ThanhToan) as TongThanhToan,
				sum(TruTamUngLuong) as TongTamUng,
				sum(DaTra) as TongDaTra,
				sum(ConLai) as TongConLai
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.MaBangLuongChiTiet
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetCongQuyDoi]
    @DateOfWeek [int],	
    @LoaiNgay [int],
	@NgayCham nvarchar(20),
    @ID_NhanVien [nvarchar](40),
    @ID_DonVi [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;

	---- nếu @NgayCham không thuộc DS ngày nghỉ lễ --> chỉ lấy CongQuyDoi là thứ 7/chủ nhật
	declare @isNgayNghiLe bit= '0', @countNgayLe int
	select @countNgayLe = count(id) from NS_NgayNghiLe 
	where Ngay is not null 
	and TrangThai='1'
	and Ngay= @NgayCham

	if @countNgayLe > 0 set @isNgayNghiLe ='1'
   
    	select 
    				a.ID_NhanVien, a.ID_CaLamViec, 
    				a.NgayApDung, a.NgayKetThuc,
    				case when a.LoaiLuong = 1 or a.LoaiLuong = 2 then 1
    					else case when a.LaPhanTram = 0 then 1 else a.GiaTri/100 end end as CongQuyDoi,				
    				case when a.LoaiLuong = 1 or a.LoaiLuong = 4 then 1
    					else case when b.LaPhanTram = 0 then 1 else ISNULL(b.GiaTri/100,1) end end as CongOTQuyDoi
    			from
    				(select 
    					pc.LoaiLuong,
    					pc.ID_NhanVien, 
    					pc.NgayApDung,
    					pc.NgayKetThuc,
    					ct.LaOT,		
    					ISNULL(ct.ID_CaLamViec,'00000000-0000-0000-0000-000000000000') as ID_CaLamViec,				
   				
						-- uu tien theo loaingay: ngayle, ngaynghi, 
						case @LoaiNgay
							when 2 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayLe_GiaTri else iif(@DateOfWeek=6,ct.Thu7_GiaTri,ct.ThCN_GiaTri) end
							when 1 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayNghi_GiaTri else iif(@DateOfWeek=6,ct.Thu7_GiaTri,ct.ThCN_GiaTri) end
						else
							case @DateOfWeek
    							when 6 then ct.Thu7_GiaTri
    							when 0 then ct.ThCN_GiaTri
							else 100
							end end as GiaTri,
						case @LoaiNgay
							when 2 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayLe_LaPhanTramLuong else iif(@DateOfWeek=6,ct.Thu7_LaPhanTramLuong,ct.CN_LaPhanTramLuong) end
							when 1 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayNghi_LaPhanTramLuong else iif(@DateOfWeek=6,ct.Thu7_LaPhanTramLuong,ct.CN_LaPhanTramLuong) end
						else
							case @DateOfWeek
    							when 6 then ct.Thu7_LaPhanTramLuong
    							when 0 then ct.CN_LaPhanTramLuong
							else 1
							end end as LaPhanTram	
	
    				from NS_ThietLapLuongChiTiet ct
    				join NS_Luong_PhuCap pc on ct.ID_LuongPhuCap= pc.ID
    				where  LaOT= 0
    				and pc.ID_NhanVien like @ID_NhanVien
    				and pc.ID_DonVi= @ID_DonVi
    				) a
    			left join
    				(
    				-- get otquydoi
    				select 
    					pc.LoaiLuong,
    					pc.ID_NhanVien, 
    					pc.NgayApDung,
    					pc.NgayKetThuc,
    					ct.LaOT,
    					'00000000-0000-0000-0000-000000000000' as ID_CaLamViec,		

						-- uu tien theo loaingay: ngayle, ngaynghi, 
						case @LoaiNgay
							when 2 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayLe_GiaTri else iif(@DateOfWeek=6,ct.Thu7_GiaTri,ct.ThCN_GiaTri) end
							when 1 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayNghi_GiaTri else iif(@DateOfWeek=6,ct.Thu7_GiaTri,ct.ThCN_GiaTri) end
						else
							case @DateOfWeek
    							when 6 then ct.Thu7_GiaTri
    							when 0 then ct.ThCN_GiaTri
							else ct.LuongNgayThuong 
							end end as GiaTri,

						case @LoaiNgay
							when 2 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayLe_LaPhanTramLuong else iif(@DateOfWeek=6,ct.Thu7_LaPhanTramLuong,ct.CN_LaPhanTramLuong) end
							when 1 then case when @isNgayNghiLe='1' or @DateOfWeek not in (0,6) then ct.NgayNghi_LaPhanTramLuong else iif(@DateOfWeek=6,ct.Thu7_LaPhanTramLuong,ct.CN_LaPhanTramLuong) end
						else
							case @DateOfWeek
    							when 6 then ct.Thu7_LaPhanTramLuong
    							when 0 then ct.CN_LaPhanTramLuong
							else ct.NgayThuong_LaPhanTramLuong
							end end as LaPhanTram	
    				from NS_ThietLapLuongChiTiet ct
    				join NS_Luong_PhuCap pc on ct.ID_LuongPhuCap= pc.ID
					where LaOT= 1
    				and pc.ID_DonVi= @ID_DonVi
    				and pc.ID_NhanVien like @ID_NhanVien
    			) b on a.ID_NhanVien= b.ID_NhanVien and a.NgayApDung= b.NgayApDung and (a.NgayKetThuc= b.NgayKetThuc OR (a.NgayKetThuc is null and b.NgayKetThuc is null))
    			order by ID_NhanVien
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetCTHDSuaChua_afterXuatKho]
    @ID_HoaDon [nvarchar](max)
AS
BEGIN
    set nocount on
    
    	--- get cthd of hdsc
    	select cthd.*,
    		cthd.SoLuong * isnull(gv.GiaVon,0) as ThanhTien,
    		isnull(gv.GiaVon,0) as GiaVon,
    		isnull(tk.TonKho,0) as TonKho,
    		isnull(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
    		lo.NgaySanXuat, lo.NgayHetHan, isnull(lo.MaLoHang,'') as MaLoHang, 
    		hh.QuanLyTheoLoHang,
    		hh.LaHangHoa,
    		hh.DichVuTheoGio,
    		hh.DuocTichDiem,
    		qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
    		hh.LaHangHoa, hh.TenHangHoa, CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach, hh.ID_NhomHang as ID_NhomHangHoa, ISNULL(hh.GhiChu,'') as GhiChuHH
    	from
    	(
    			select 
    				 ctsc.ID_DonViQuiDoi, ctsc.ID_LoHang,ctsc.ID_HoaDon,
					 max(ctsc.ChatLieu) as ChatLieu,
    				 max(ctsc.ID_DonVi) as ID_DonVi,
    				 sum(SoLuong) as SoLuongMua,
    				 sum(SoLuongXuat) as SoLuongXuat,
    				 sum(SoLuong) - isnull(sum(SoLuongXuat),0) as SoLuong
    			from
    			(
    			select sum(ct.SoLuong) as SoLuong,
    				0 as SoLuongXuat,
					max(ct.ChatLieu) as ChatLieu,
    				ct.ID_DonViQuiDoi,
    				ct.ID_LoHang,
    				ct.ID_HoaDon,
    				hd.ID_DonVi
    			from BH_HoaDon_ChiTiet ct
    			join BH_HoaDon hd on hd.ID= ct.ID_HoaDon
    			where ct.ID_HoaDon= @ID_HoaDon
    			and (ct.ID_ChiTietDinhLuong != ct.ID or ct.ID_ChiTietDinhLuong is null)
				and ct.ID_LichBaoDuong is null ---- khong xuat hang bao duong
    			group by ct.ID_DonViQuiDoi, ct.ID_LoHang,ct.ID_HoaDon,hd.ID_DonVi
    
    			union all
    			-- get cthd daxuat kho
    			select 0 as SoLuong,
    				sum(ct.SoLuong) as SoLuongXuat,
					max(ct.ChatLieu) as ChatLieu,
    				ct.ID_DonViQuiDoi,
    				ct.ID_LoHang,
    				@ID_HoaDon as ID_HoaDon,
    				'00000000-0000-0000-0000-000000000000' as ID_DonVi
    			from BH_HoaDon_ChiTiet ct
    			join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    			where hd.ID_HoaDon= @ID_HoaDon
    			and hd.ChoThanhToan='0'
    			group by ct.ID_DonViQuiDoi, ct.ID_LoHang
    			)ctsc
    			group by ctsc.ID_DonViQuiDoi, ctsc.ID_LoHang,ctsc.ID_HoaDon
    	) cthd
    	join DonViQuiDoi qd on cthd.ID_DonViQuiDoi= qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    	left join DM_LoHang lo on cthd.ID_LoHang= lo.ID and hh.ID= lo.ID_HangHoa
    	left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    	left join DM_HangHoa_TonKho tk on (qd.ID = tk.ID_DonViQuyDoi and (lo.ID = tk.ID_LoHang or lo.ID is null) and  tk.ID_DonVi = cthd.ID_DonVi)
    	left join DM_GiaVon gv on (qd.ID = gv.ID_DonViQuiDoi and (lo.ID = gv.ID_LoHang or lo.ID is null) and gv.ID_DonVi = cthd.ID_DonVi) -- lay giavon hientai --> xuatkho gara tu hdsc
    	where hh.LaHangHoa= 1
END");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonTraHang]
     @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max),
	@ID_NhanVienLogin uniqueidentifier,
	@NguoiTao nvarchar(max),
	@TrangThai nvarchar(max),
	@ColumnSort varchar(max),
	@SortBy varchar(max),
	@CurrentPage int,
	@PageSize int
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
    Select @count =  (Select count(*) from @tblSearch);

		with data_cte
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
    	c.TongTienHDDoiTra,
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.MaHoaDonGoc, 
		c.LoaiHoaDonGoc,
    	c.BienSo,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	ISNULL(c.NguoiTaoHD,'') as NguoiTaoHD,
		c.DienThoai,
		c.Email,
		c.DiaChiKhachHang,
		c.NgaySinh_NgayTLap,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TongTienHang, c.TongGiamGia, c.KhuyeMai_GiamGia,
		case when c.PhaiThanhToan < c.TongTienHDDoiTra then 0
		else c.PhaiThanhToan- c.TongTienHDDoiTra
		end as PhaiThanhToan,
		--c.PhaiThanhToan as A,
		c.TongChiPhi, c.KhachDaTra, c.TongThanhToan,
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
		case when c.PhaiThanhToan < c.TongTienHDDoiTra then 0
		else c.PhaiThanhToan- c.TongTienHDDoiTra - c.TongChiPhi - c.KhachDaTra
		end as ConNo,
		
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
			
    		ISNULL(hddt.PhaiThanhToan,0) as TongTienHDDoiTra,
    		ISNULL(bhhd.DiemGiaoDich,0) as DiemGiaoDich,
    		bhhd.ChoThanhToan,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		Case when hdb.MaHoaDon is null then '' else hdb.MaHoaDon end as MaHoaDonGoc,
			ISNULL(hdb.LoaiHoaDon ,1) as LoaiHoaDonGoc,
    		--a.MaPhieuChi,
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
			cast (0 as float) as ConNo,
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
					sum(TienMat) as TienMat,-- TraHang: khong POS --> TienGui alway = 0
					sum(TienCK) as ChuyenKhoan
				from (
					Select 
    				bhhd.ID,					
					case when qhd.TrangThai ='0' then 0 else ISNULL(hdct.Tienthu, 0) end as KhachDaTra,
					Case when qhd.TrangThai = 0 then 0 else ISNULL(hdct.ThuTuThe, 0) end as ThuTuThe,
					case when qhd.TrangThai = 0 then 0 else ISNULL(hdct.TienMat, 0) end as TienMat,										
					case when qhd.TrangThai = 0 then 0
						else case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end
					end as TienCK					
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang		
    				where bhhd.LoaiHoaDon = '6'
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
					--group by  bhhd.ID, bhhd.MaHoaDon,qhd.TrangThai
				) a1 group by a1.ID
    		) as a
    		left join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdb on bhhd.ID_HoaDon = hdb.ID
    		left join BH_HoaDon hddt on bhhd.ID = hddt.ID_HoaDon and hddt.ChoThanhToan is not null		
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
			where (exists( select * from @tblNhanVien nv where nv.ID= c.ID_NhanVien) or c.NguoiTaoHD= @NguoiTao)
			and
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

			Sql(@"ALTER PROCEDURE [dbo].[GetListHoaDon_UseService]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    select 
    	hdsd.ID ,hdsd.MaHoaDon,
    	convert(varchar,hdsd.NgayLapHoaDon, 103) as NgayLapHoaDon,
    	CAST(hdsd.TongTienHang - ISNULL(hdsd.TongGiamGia,0) - ISNULL(hdsd.KhuyeMai_GiamGia,0) as float) as TongTienHang,
    	CAST(ISNULL(hdsd.PhaiThanhToan,0) AS float) AS PhaiThanhToan,
    	hdsd.DienGiai as GhiChu,
    	CAST(ISNULL(tblQuyHD.TongTienThu,0) AS float) AS DaThanhToan,
		hdsd.ID_Xe, xe.BienSo
    from BH_HoaDon hdsd
	join Gara_DanhMucXe xe on hdsd.ID_Xe= xe.ID
    join 
    
    	(select hdb.ID
    	 from BH_HoaDon hdb
    	join BH_HoaDon_ChiTiet cthd on hdb.ID = cthd.ID_HoaDon where cthd.ID_ChiTietGoiDV is not null and cthd.ChatLieu='4'
    	group by hdb.ID) HoaDonBan on HoaDonBan.ID= hdsd.ID
    left join 
    		(select qct.ID_HoaDonLienQuan, MAX(qhd.TongTienThu) as TongTienThu from Quy_HoaDon qhd
    		join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    			where qct.ID_DoiTuong like @ID_DoiTuong 
    			and qhd.ID_DonVi like @ID_DonVi
    		group by qct.ID_HoaDonLienQuan,qct.ID_HoaDon) tblQuyHD on hdsd.ID = tblQuyHD.ID_HoaDonLienQuan
    where hdsd.ID_DoiTuong like @ID_DoiTuong 
    	and hdsd.ID_DonVi like @ID_DonVi and hdsd.ChoThanhToan='0'
    order by hdsd.NgayLapHoaDon 
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetSoDuDatCoc_ofDoiTuong]
    @ID_DoiTuong [uniqueidentifier],
    @ID_DonVi [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    	
		if @ID_DoiTuong not like '%00000000%'
		begin
					declare @LoaiDoiTuong int = (select LoaiDoiTuong from DM_DoiTuong where id= @ID_DoiTuong)
					declare @LoaiThuCoc int =11, @LoaiChiCoc int = 12
    				if @LoaiDoiTuong = 2 
    				begin
    					set @LoaiThuCoc = 12
    					set @LoaiChiCoc = 11 -- NCC tra lai tiencoc
    				end
    
    				select 		
    					cast(sum(TongNap) as float) as TongThuTheGiaTri, 
    					cast(sum(isnull(SuDung,0)) as float) as SuDungThe,
    					cast(sum(TraCoc) as float) as HoanTraTheGiatri,
    					cast(SUM(TongNap)  - SUM(isnull(SuDung,0)) - SUM(TraCoc) as float) as SoDuTheGiaTri
    				from (
    					-- tong nap (hay thucthu) tien coc
    					SELECT 
    						sum(qct.TienThu) as TongNap,
    						0 as SuDung,
    						0 as TraCoc
    					FROM Quy_HoaDon qhd
    					join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    					where qhd.LoaiHoaDon = @LoaiThuCoc and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    					and qct.ID_DoiTuong like @ID_DoiTuong 
    					and qct.LoaiThanhToan = 1
    					and qhd.ID_DonVi = @ID_DonVi	
    
    					union all
    						-- tra lai tiencoc
    					SELECT 
    						0 as TongNap,
    						0 as SuDung,
    						sum(qct.TienThu) as TraCoc
    					FROM Quy_HoaDon qhd
    					join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    					where qhd.LoaiHoaDon = @LoaiChiCoc and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    					and qct.ID_DoiTuong like @ID_DoiTuong 
    					and qct.LoaiThanhToan = 1	
    					and qhd.ID_DonVi = @ID_DonVi	
    
    					union all
    					-- su dung coc
    					SELECT 
    						0 as TongNap,
    						SUM(qct.TienThu) as SuDungThe,
    						0 as HoanTraTheGiatri
    					FROM Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd ON qct.ID_HoaDon = qhd.ID		
    					WHERE qct.ID_DoiTuong like @ID_DoiTuong 
    					and qct.HinhThucThanhToan= 6
    					and qhd.ID_DonVi = @ID_DonVi	
    					and qhd.LoaiHoaDon = @LoaiThuCoc and (qhd.TrangThai = 1 or qhd.TrangThai is null)   	
    	
    					) tbl  
		end
    	
END");

			Sql(@"ALTER PROCEDURE [dbo].[KhoiTaoDuLieuLanDau]
	@Subdomain NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ThoiGian DATETIME;
	SET @ThoiGian = GETDATE();
	DECLARE @IDNganhNgheKinhDoanh UNIQUEIDENTIFIER;
	DECLARE @TenCuaHang NVARCHAR(max), @DiaChi NVARCHAR(max), @Email NVARCHAR(max), @SoDienThoai NVARCHAR(max), @TenNhanVien NVARCHAR(max),
	@TaiKhoan NVARCHAR(MAX), @MatKhau NVARCHAR(MAX);
	SELECT 
	@TenCuaHang = IIF(TenCuaHang != '', TenCuaHang, 'Open24.vn'), 
	@DiaChi = IIF(DiaChi != '', DiaChi, 'Open24.vn'), 
	@Email = IIF(Email != '', Email, ''),
	@SoDienThoai = IIF(SoDienThoai != '', SoDienThoai, ''),
	@TenNhanVien = IIF(HoTen != '', HoTen, 'Open24.vn'),
	@TaiKhoan = UserKT,
	@MatKhau = MatKhauKT,
	@IDNganhNgheKinhDoanh = ID_NganhKinhDoanh
	FROM BANHANG24..CuaHangDangKy WHERE SubDomain = @Subdomain;
	--INSERT HT_CongTy
	INSERT INTO HT_CongTy (ID, TenCongTy, DiaChi, SoDienThoai, SoFax, MaSoThue, Mail, Website, TenGiamDoc, TenKeToanTruong, Logo, GhiChu, 
	TaiKhoanNganHang, ID_NganHang, DiaChiNganHang, TenVT, DiaChiVT, DangHoatDong, DangKyNhanSu, NgayCongChuan)
	VALUES ('4DE12030-B7FE-487A-B6B2-A99665E8AE7C', @TenCuaHang, @DiaChi, @SoDienThoai, '', '', @Email, '', '', '', NULL, '',
	'', NULL, '', @TenCuaHang, @DiaChi, 1, 0, 26);
	--INSERT DM_DonVi
	DECLARE @IDDonVi UNIQUEIDENTIFIER;
	SET @IDDonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE';
	INSERT INTO DM_DonVi (ID, MaDonVi, TenDonVi, DiaChi, Website, MaSoThue, SoTaiKhoan, SoDienThoai, SoFax, KiTuDanhMa, HienThi_Chinh, HienThi_Phu,
	NgayTao, NguoiTao, TrangThai)
	VALUES (@IDDonVi, 'CTY', @TenCuaHang, @DiaChi, @Subdomain + '.open24.vn', '', '', @SoDienThoai, '', @Subdomain,1, 1, @ThoiGian, 'ssoftvn', 1);
	--INSERT NS_NhanVien
	DECLARE @IDNhanVien UNIQUEIDENTIFIER;
	SET @IDNhanVien = NEWID();
	INSERT INTO NS_NhanVien (ID, MaNhanVien, TenNhanVien, GioiTinh, DienThoaiDiDong, Email, CapTaiKhoan, DaNghiViec, NguoiTao, NgayTao, TrangThai)
	VALUES (@IDNhanVien, 'NV01', @TenNhanVien, 1, @SoDienThoai, @Email, 1, 0, 'ssoftvn', @ThoiGian, 1);
	--INSERT QuaTrinhCongTac
	INSERT INTO NS_QuaTrinhCongTac (ID, ID_NhanVien, ID_DonVi, NgayApDung, LaChucVuHienThoi, LaDonViHienThoi, NguoiLap, NgayLap)
	VALUES (NEWID(), @IDNhanVien, @IDDonVi, @ThoiGian, 1, 1, 'ssoftvn', @ThoiGian);
	--INSERT HT_NguoiDung
	DECLARE @IDNguoiDung UNIQUEIDENTIFIER;
	SET @IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77';
	INSERT INTO HT_NguoiDung (ID, ID_NhanVien, TaiKhoan, MatKhau, LaNhanVien, LaAdmin, DangHoatDong, IsSystem, NguoiTao, NgayTao, ID_DonVi, XemGiaVon, SoDuTaiKhoan)
	VALUES (@IDNguoiDung, @IDNhanVien, @TaiKhoan, @MatKhau, 1, 1, 1, 1, 'ssoftvn', @ThoiGian, @IDDonVi, 0, 0);
	--INSERT HT_NhomNguoiDung
	DECLARE @IDNhomNguoiDung UNIQUEIDENTIFIER;
	SET @IDNhomNguoiDung = 'EE609285-F6A6-43D8-A517-BDA52F426AE5';
	INSERT INTO HT_NhomNguoiDung (ID, MaNhom, TenNhom, MoTa, NguoiTao, NgayTao)
	VALUES (@IDNhomNguoiDung, 'ADMIN', 'ADMIN', N'Nhóm quản trị', 'ssoftvn', @ThoiGian);
	--INSERT HT_NguoiDung_Nhom
	INSERT INTO HT_NguoiDung_Nhom (ID, IDNguoiDung, IDNhomNguoiDung, ID_DonVi)
	VALUES (NEWID(), @IDNguoiDung, @IDNhomNguoiDung, @IDDonVi);
	--INSERT HT_Quyen
	INSERT INTO HT_Quyen (MaQuyen, TenQuyen, MoTa, QuyenCha, DuocSuDung)
	SELECT q.MaQuyen, q.TenQuyen, q.MoTa, q.QuyenCha, q.DuocSuDung FROM BANHANG24..HT_Quyen q
	INNER JOIN BANHANG24..HT_Quyen_NganhNgheKinhDoanh qn ON q.MaQuyen = qn.MaQuyen
	WHERE qn.ID_NganhKinhDoanh = @IDNganhNgheKinhDoanh;
	--INSERT HT_Quyen_Nhom
	INSERT INTO HT_Quyen_Nhom (ID, ID_NhomNguoiDung, MaQuyen)
	SELECT NEWID(), @IDNhomNguoiDung, MaQuyen FROM HT_Quyen;
	--INSERT HT_CauHinhPhanMem
	INSERT INTO HT_CauHinhPhanMem (ID, ID_DonVi, GiaVonTrungBinh, CoDonViTinh, DatHang, XuatAm, DatHangXuatAm, ThayDoiThoiGianBanHang, SoLuongTrenChungTu,
	TinhNangTichDiem, GioiHanThoiGianTraHang, SanPhamCoThuocTinh, BanVaChuyenKhiHangDaDat, TinhNangSanXuatHangHoa, SuDungCanDienTu, KhoaSo, InBaoGiaKhiBanHang,
	QuanLyKhachHangTheoDonVi, KhuyenMai, LoHang, SuDungMauInMacDinh, ApDungGopKhuyenMai, ThongTinChiTietNhanVien, BanHangOffline, ThoiGianNhacHanSuDungLo,
	SuDungMaChungTu, ChoPhepTrungSoDienThoai)
	VALUES (NEWID(), @IDDonVi, 1, 1, 1, 0, 0, 0, 0,
	0, 0, 1, 0, 0, 0, 1, 0,
	0, 1, 0, 0, 0, 0, 0, 0,
	0, 0);
	--INSERT DM_QuocGia
	INSERT INTO DM_QuocGia (ID, MaQuocGia, TenQuocGia, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaQuocGia, TenQuocGia, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_QuocGia;
	--INSERT DM_VungMien
	INSERT INTO DM_VungMien (ID, MaVung, TenVung, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaVung, TenVung, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_VungMien;
	--INSERT DM_TinhThanh
	INSERT INTO DM_TinhThanh (ID, MaTinhThanh, TenTinhThanh, ID_QuocGia, ID_VungMien, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaTinhThanh, TenTinhThanh, ID_QuocGia, ID_VungMien, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_TinhThanh;
	--INSERT DM_QuanHuyen
	INSERT INTO DM_QuanHuyen (ID, MaQuanHuyen, TenQuanHuyen, ID_TinhThanh, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaQuanHuyen, TenQuanHuyen, ID_TinhThanh, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_QuanHuyen;
	--INSERT DM_XaPhuong
	INSERT INTO DM_XaPhuong (ID, TenXaPhuong, ID_QuanHuyen)
	SELECT ID, Ten, ID_QuanHuyen FROM BANHANG24..DM_XaPhuong
	--INSERT DM_ThueSuat
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'0%', 0, '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'5%', 5, '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'10%', 10, '', 'ssoftvn', @ThoiGian);
	--INSERT DM_TienTe
	DECLARE @IDTienTe UNIQUEIDENTIFIER;
	DECLARE @IDQuocGia UNIQUEIDENTIFIER;
	SET @IDTienTe = '406eed2d-faae-4520-aef2-12912f83dda2';
	SELECT @IDQuocGia = ID FROM DM_QuocGia WHERE MaQuocGia = 'VNI';
	INSERT INTO DM_TienTe (ID, ID_QuocGia, LaNoiTe, MaNgoaiTe, TenNgoaiTe, NguoiTao, NgayTao)
	VALUES (@IDTienTe, @IDQuocGia, 1, N'VND', N'Việt Nam đồng', 'ssoftvn', @ThoiGian);
	--INSERT DM_TyGia
	INSERT INTO DM_TyGia (ID, ID_TienTe, TyGia, NgayTyGia, GhiChu, NguoiTao, NgayTao)
	VALUES (NEWID(), @IDTienTe, 1, @ThoiGian, '', 'ssoftvn', @ThoiGian);
	--INSERT DM_Kho
	DECLARE @IDKho UNIQUEIDENTIFIER;
	SET @IDKho = '01CD02F2-4612-4104-B790-1C0373CBD72D';
	INSERT INTO DM_Kho (ID, MaKho, TenKho, NguoiTao, NgayTao)
	VALUES (@IDKho, N'KHO_CTy', N'Kho tổng', 'ssoftvn', @ThoiGian);
	--INSERT Kho_DonVi
	INSERT INTO Kho_DonVi (ID, ID_DonVi, ID_Kho)
	VALUES (NEWID(), @IDDonVi, @IDKho);
	--INSERT DM_LoaiChungTu
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (1, 'HDBL', N'Hóa đơn bán lẻ', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (2, 'HDB', N'Hóa đơn bán', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (3, 'BG', N'Báo giá', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (4, 'PNK', N'Phiếu nhập kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (5, 'PXK', N'Phiếu xuất kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (6, 'TH', N'Trả hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (7, 'THNCC', N'Trả hàng nhà cung cấp', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (8, 'XH', N'Xuất kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (9, 'PKK', N'Phiếu kiểm kê', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (10, 'CH', N'Chuyển hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (11, 'SQPT', N'Phiếu thu', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (12, 'SQPC', N'Phiếu chi', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (13, 'NH', N'Nhận hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (14, 'DH', N'Đặt hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (15, 'CB', N'Điều chỉnh', N'Điều chỉnh công nợ khách hàng, nhà cung cấp', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (16, 'KTGV', N'Khởi tạo giá vốn', N'Khởi tạo giá vốn khi tạo hàng hóa', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (17, 'DTH', N'Đổi trả hàng', N'Đổi trả hàng hóa', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (18, 'DCGV', N'Điều chỉnh giá vốn', N'Điều chỉnh giá vốn', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (19, 'GDV', N'Gói dịch vụ', N'Bán gói dịch vụ', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (20, 'TGDV', N'Trả gói dịch vụ', N'Trả gói dịch vụ', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (21, 'IMV', N'Tem - Mã vạch', N'Tem - Mã vạch', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (22, 'TGT', N'Thẻ giá trị', N'Bán, nạp thẻ giá trị', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (23, 'DCGT', N'Điều chỉnh thẻ giá trị', N'Điều chỉnh giá trị thẻ giá trị', 'ssoftvn', @ThoiGian);
	--INSERT HT_ThongBao_CaiDat
	INSERT INTO HT_ThongBao_CaiDat (ID, ID_NguoiDung, NhacSinhNhat, NhacCongNo, NhacTonKho, NhacDieuChuyen, NhacLoHang)
	VALUES (NEWID(), @IDNguoiDung, 1, 1, 1, 1, 1);
	--INSERT NS_PhongBan
	DECLARE @IDPhongBan UNIQUEIDENTIFIER;
	SET @IDPhongBan = '6DE963A7-50AF-4E51-91E8-E242D7E7B476';
	INSERT INTO NS_PhongBan (ID, MaPhongBan, TenPhongBan, TrangThai)
	VALUES (@IDPhongBan, 'PB0000', N'Phòng ban mặc định', 1);
	--INSERT DM_NganHang
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'CTG', N'Ngân hàng Công thương Việt Nam (VietinBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIETBANK', N'Ngân hàng Việt Nam Thương Tín (VietBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'OCB', N'Ngân hàng Phương Đông (Orient Commercial Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NAMABANK', N'Ngân hàng Nam Á (Nam A Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SEABANK', N'Ngân hàng Đông Nam Á (SeABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'AGRIBANK', N'Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam (Agribank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'MBB', N'Ngân hàng Quân đội (Military Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'ACB', N'Ngân hàng Á Châu (ACB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIB', N'Ngân hàng Quốc tế (VIBBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'DAF', N'Ngân hàng Đông Á (DAF)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BVB', N'Ngân hàng Bảo Việt (BaoVietBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VDB', N'Ngân hàng Phát triển Việt Nam (VDB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'KIENLONGBANK', N'Ngân hàng Kiên Long (KienLongBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'HDBANK', N'Ngân hàng Phát triển nhà Thành Phố Hồ Chí Minh (HDBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SHB', N'Ngân hàng Sài Gòn - Hà Nội (SHBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'EIB', N'Ngân hàng Xuất Nhập khẩu Việt Nam (Eximbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'CB', N'Ngân hàng Xây dựng (CB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SGB', N'Ngân hàng Sài Gòn Công Thương (Saigonbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'PVCOMBANK', N'Ngân hàng Đại chúng (PVcom Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'PGBANK', N'Ngân hàng Xăng dầu Petrolimex (Petrolimex Group Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'OCEANBANK', N'Ngân hàng Đại Dương (Oceanbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'TECHCOMBANK', N'Ngân hàng Kỹ Thương Việt Nam (Techcombank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VAB', N'Ngân hàng Việt Á (VietABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'STB', N'Ngân hàng Sài Gòn Thương Tín (Sacombank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'MSB', N'Ngân hàng Hàng Hải Việt Nam (MaritimeBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VCB', N'Ngân hàng Ngoại thương Việt Nam (Ngoại Thương Việt Nam)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'GPBANK', N'Ngân hàng Dầu khí Toàn Cầu (GPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIETCAPITALBANK', N'Ngân hàng Bản Việt (VietCapitalBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SCB', N'Ngân hàng Sài Gòn (Sài Gòn)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'LPB', N'Ngân hàng Bưu điện Liên Việt (LienVietPostBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BID', N'Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'ABBANK', N'Ngân hàng An Bình (ABBANK)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NHCSXH/VBSP', N'Ngân hàng Chính sách xã hội (NHCSXH/VBSP)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'TPBANK', N'Ngân hàng Tiên Phong (TPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BACABANK', N'Ngân hàng Bắc Á (BacABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VPBANK', N'Ngân hàng Việt Nam Thịnh Vượng (VPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NCB', N'Ngân hàng Quốc Dân (National Citizen Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	--INSERT OptinForm_TruongThongTin
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Chọn chi nhánh', 2, 1, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Giới tính', 1, 3, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số lượng', 2, 5, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Địa chỉ', 1, 7, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Thời gian', 2, 9, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Khách hàng', 2, 3, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Email', 1, 6, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Dịch vụ yêu cầu', 2, 8, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Khách hàng lẻ', 1, 11, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Xưng hô', 2, 2, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Nhân viên thực hiện', 2, 10, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số điện thoại', 2, 6, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Tỉnh thành', 1, 8, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Mã số thuế', 1, 10, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Bạn đi theo nhóm', 2, 4, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ngày sinh', 1, 4, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Quận huyện', 1, 9, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số điện thoại', 1, 5, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ảnh đại diện', 1, 1, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ghi chú', 2, 11, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Email', 2, 7, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Người giới thiệu', 1, 12, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Tên khách hàng', 1, 2, 1);
	--INSERT DM_DoiTuong
	INSERT INTO DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong, TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, ChiaSe, TheoDoi,
	ID_DonVi, TenNhomDoiTuongs, NguoiTao, NgayTao)
	VALUES ('00000000-0000-0000-0000-000000000000', 1, 1, 'KL00001', N'Khách lẻ', 'Khach le', 'Kl', 0, 0, @IDDonVi, N'Nhóm mặc định', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong, TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, ChiaSe, TheoDoi,
	ID_DonVi, TenNhomDoiTuongs, NguoiTao, NgayTao)
	VALUES ('00000000-0000-0000-0000-000000000002', 2, 1, 'NCCL001', N'Nhà cung cấp lẻ', 'Nha cung cap le', 'nccl', 0, 0, @IDDonVi, N'Nhóm mặc định', 'ssoftvn', @ThoiGian);
	--INSERT DM_NhomHangHoa
	INSERT INTO DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa, LaNhomHangHoa, NguoiTao, NgayTao, HienThi_Chinh, HienThi_Phu, HienThi_BanThe,
	TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau)
	VALUES ('00000000-0000-0000-0000-000000000000', 'NHMD00001', N'Nhóm hàng hóa mặc định', 1, 'ssoftvn', @ThoiGian, 1, 1, 1, 'Nhom hang hoa mac dinh', 'Nhhmd');
	INSERT INTO DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa, LaNhomHangHoa, NguoiTao, NgayTao, HienThi_Chinh, HienThi_Phu, HienThi_BanThe,
	TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau)
	VALUES ('00000000-0000-0000-0000-000000000001', 'DVMD00001', N'Nhóm dịch vụ mặc định', 0, 'ssoftvn', @ThoiGian, 1, 1, 1, 'Nhom dich vu mac dinh', 'Ndvmd');

	--INSERT Gara_HangXe
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('00000000-0000-0000-0000-000000000000', 'CXD', N'Chưa xác định', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('886E75F8-21D8-4519-A32B-19FC75D2FD2C', 'CHEVROLET', N'Chevrolet', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('326502EE-14C5-4398-BA65-90361E3AD6D1', 'FORD', N'Ford', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('CAC7F723-6BEC-4FA4-A18A-CCE47B4D921B', 'HONDA', N'Honda', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('D6CD766E-86A1-46B6-882F-BDD4EAD7EA78', 'HYUNDAI', N'Hyundai', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('D954DD78-0477-43A0-B2D7-495A9549F7E1', 'INFINITI', N'Infiniti', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('3B036EBD-12B9-4D3C-9136-29B7F086F1B7', 'ISUZU', N'Isuzu', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('CBDE7FA8-0F56-43B0-B0E0-BF4C7653AF49', 'KIA', N'Kia', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('48C65217-1943-4EA2-A3E7-75617D45D975', 'LEXUS', N'Lexus', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8B4D5D6B-B2F3-46A3-8C6D-5742A6B1B8AA', 'MASERATI', N'Maserati', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('3DB382EC-3E11-42E2-9AAE-07E113AD7BEE', 'MAZDA', N'Mazda', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('26C1C0A3-3C0C-4E5F-AA5C-1C347DA1E5B2', 'MERCEDES', N'Mercedes', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('36D9D282-7152-416F-AE4A-CA9232186699', 'MITSUBISHI', N'Mitsubishi', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8E6D885B-4B6D-4A18-AA32-FCCE8E4373B9', 'NISSAN', N'Nissan', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('04844286-EDB5-41E2-91E7-A0B06695E31F', 'PEUGEOT', N'Peugeot', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('0788EDD3-AE0E-4BEF-AA98-2FEDD94EC8B1', 'PORSCHE', N'Porsche', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('34A0801B-D517-478F-8172-774E3ED0A7FE', 'RENAULT', N'Renault', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('98A3621E-7D00-436F-B25C-045AC2186AE5', 'SSANGYONG', N'Ssangyong', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('FED3837F-7A4D-4C98-9B85-90F1AE23E753', 'SUBARU', N'Subaru', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('B3CD7C19-67EC-43A9-B56F-A12EE87B53CA', 'SUZUKI', N'Suzuki', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('EA6CA8A9-8B9E-489E-8C07-2F673C6E3EF7', 'TOYOTA', N'Toyota', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('34192C59-2D6E-47E9-AE92-6B00D5A6DA3A', 'VINFAST', N'Vinfast', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8E65F5C2-5D5A-42E8-93FF-33C379B84F4D', 'VOLKSWAGEN', N'Volkswagen', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('39A939F7-1EBF-494C-9BE1-5EB750E73334', 'VOLVO', N'Volvo', '', 1, 'ssoftvn', GETDATE(), '', null);

	--INSERT Gara_LoaiXe
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('00000000-0000-0000-0000-000000000000', 'CXD', N'Chưa xác định', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8C15086A-0645-437B-BA7A-008CFDC0C737', 'SEDAN', N'Sedan', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('4115FB1B-8978-40E2-87F9-67DB6425AB5E', 'HATCHBACK', N'Hatchback', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('B17B339C-CDF3-4E9E-ACCE-29FB097899F0', 'MPV', N'MPV', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('78FFB367-ECD6-4A55-8E6C-769FC3D3F13B', 'SUV', N'SUV', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('A81F4945-1B3A-4FD1-B8B8-2B35B4AC58C0', 'SUV-COUPE', N'SUV-Coupe', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('7D8190F1-07A9-477D-8B05-EBB8EC154FCC', 'SUV-WAGON', N'SUV-Wagon', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('2BE297DF-B5A5-4657-85DF-EEA7E2E6BB0F', 'PICK-UP', N'Pick-up', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('83583EFC-AFA8-446D-90A9-36397A7F01D5', 'COUPE', N'Coupe', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('83034F8B-B16F-41FB-B75A-2B53AD1410E0', 'COUPE2CUA', N'Coupe 2 cửa', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('3C7BC4F5-E7B4-475C-B29A-FB122C38D1F9', 'COUPE4CUA', N'Coupe 4 cửa', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('6541F0FC-D139-4C44-8EE8-B67045EC842F', 'CROSSOVER', N'Crossover', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('B26E8752-B118-4680-90E0-BC04028AA831', 'CONVERTIBLE', N'Convertible', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('89501335-9457-4BF1-9335-A9E0355C5E63', 'ROADSTER', N'Roadster', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('340C75FF-46F4-4856-B649-983273A152D8', 'SIEUXE', N'Siêu xe', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('5DF0C056-FC43-4653-A811-44BA3468124F', 'STATION WAGON', N'Station Wagon', 1, 'ssoftvn', GETDATE(), '', null);

	INSERT INTO Gara_MauXe (ID, TenMauXe, ID_HangXe, ID_LoaiXe, GhiChu, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua)
	VALUES ('00000000-0000-0000-0000-000000000000', N'Chưa xác định', '00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', '', 1, 'ssoftvn', GETDATE(), '', null);
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_BaoCaoKhuyenMai]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @LoaiChungTu [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    	Select @count =  (Select count(*) from @tblSearchString);
       
    	select * from 
    	(
    		select km.MaKhuyenMai, km.TenKhuyenMai,km.HinhThuc, hd.ID_DonVi, hd.MaHoaDon,hd.LoaiHoaDon, hd.NgayLapHoaDon, hd.TongTienHang, hd.NguoiTao, 
    			 ''as MaHangHoa, '' as TenHangHoa,
				 case when km.HinhThuc = 12 or km.HinhThuc = 13 then ct.SoLuong else 0 end as SoLuong, 
    			 case km.HinhThuc
					 when 11 then hd.KhuyeMai_GiamGia
					 when 12 then ct.SoLuong * ct.DonGia -- tanghang
					 when 13 then  ct.SoLuong * ct.TienChietKhau -- gghang
					 when 14 then hd.DiemKhuyenMai
					 else 0 end as GiaTriKM, 
    			ISNULL(dt.MaDoiTuong, '') as MaDoiTuong,ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau, 
    			'' as TenDonViTinh,'' as LaHangHoa, '1' as TheoDoi, '0' as Xoa,
    			null as TenHangHoa_KhongDau, null as TenHangHoa_KyTuDau,
    			nv.MaNhanVien, nv.TenNhanVien,TenNhanVienChuCaiDau,TenNhanVienKhongDau, '00000000-0000-0000-0000-000000000000' as ID_NhomHang,
    			case HinhThuc
    				when 11 then N'Hóa đơn - Giảm hóa đơn'
    				when 12 then N'Hóa đơn -Tặng hàng'
    				when 13 then N'Hóa đơn - Giảm giá hàng'
    				when 14 then N'Hóa đơn - Tặng điểm'
    				end as sHinhThuc
    		from BH_HoaDon hd
    		join DM_KhuyenMai km on hd.ID_KhuyenMai = km.ID
			left join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon and hd.ID_KhuyenMai = ct.ID_KhuyenMai and ct.TangKem = '1' -- used to hoadon - gghang, hoadon - tanghang
    		left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		INNER JOIN (select * from splitstring(@ID_ChiNhanh)) lstID_DonVi ON lstID_DonVi.Name = hd.ID_DonVi			
    		where hd.ID_KhuyenMai is not null
    		and hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
    		and hd.ChoThanhToan = 0
    
    		union all
    
    		select MaKhuyenMai,TenKhuyenMai,HinhThuc,kmhh.ID_DonVi, MaHoaDon,LoaiHoaDon, NgayLapHoaDon, TongTienHang,kmhh.NguoiTao, MaHangHoa,TenHangHoa,SoLuong, GiaTriKM,
    			ISNULL(dt.MaDoiTuong, '') as MaDoiTuong, ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau, 
    			TenDonViTinh, LaHangHoa,kmhh.TheoDoi,kmhh.Xoa,TenHangHoa_KhongDau,TenHangHoa_KyTuDau,
    			MaNhanVien,TenNhanVien,TenNhanVienChuCaiDau,TenNhanVienKhongDau, ID_NhomHang,
    			case HinhThuc
    				when 21 then N'Hàng hóa - Giảm giá hàng'
    				when 22 then N'Hàng hóa - Tặng hàng'
    				when 23 then N'Hàng hóa - Tặng điểm'
    				when 24 then N'Hàng hóa - Giá bán theo số lượng mua'
    				end as sHinhThuc
    		from (			
    			select hd.ID, hd.LoaiHoaDon, hd.ID_DoiTuong, hd.ID_NhanVien, hd.ID_DonVi, km.MaKhuyenMai, km.TenKhuyenMai,
				km.HinhThuc, hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienHang, qd.MaHangHoa,     				
    				case HinhThuc
    					when 21 then ctkm.SoLuong *  ctkm.TienChietKhau
    					when 22 then ctkm.SoLuong * ctkm.DonGia
						when 23 then 0
    					when 24 then ctkm.SoLuong * ctkm.TienChietKhau    				
    					end as GiaTriKM,
    				 hh.TenHangHoa, ctkm.SoLuong, hd.NguoiTao,
    				 ctkm.ID_TangKem, ctkm.TangKem, qd.TenDonViTinh, hh.TheoDoi, qd.Xoa,
    				 hh.TenHangHoa_KhongDau, hh.TenHangHoa_KyTuDau,hh.LaHangHoa,hh.ID_NhomHang,
					 iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa
    			from BH_HoaDon_ChiTiet ct
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    			join DM_KhuyenMai km on ct.ID_KhuyenMai = km.ID
    			join BH_HoaDon_ChiTiet ctkm on ct.ID_DonViQuiDoi = ctkm.ID_TangKem and ct.ID_HoaDon = ctkm.ID_HoaDon and (ctkm.ID_TangKem is not null or ctkm.Tangkem ='0')
    			join DonViQuiDoi qd on ctkm.ID_DonViQuiDoi = qd.ID
    			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    			INNER JOIN (select * from splitstring(@ID_ChiNhanh)) lstID_DonVi ON lstID_DonVi.Name = hd.ID_DonVi	
    			where ct.ID_KhuyenMai is not null
    			and hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
    			and hd.ChoThanhToan = 0
				) kmhh
    			left join DM_DoiTuong dt on kmhh.ID_DoiTuong = dt.ID
    			left join NS_NhanVien nv on kmhh.ID_NhanVien = nv.ID
    			where (kmhh.ID_TangKem is not null or TangKem ='1')
				and kmhh.LoaiHangHoa in (select name from dbo.splitstring(@LoaiHangHoa))
    	) tbl
    	inner join (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh ON tbl.ID_NhomHang = allnhh.ID		
    	where  tbl.TheoDoi like @TheoDoi    		
		and tbl.Xoa like @TrangThai
    		and tbl.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    		AND ((select count(Name) from @tblSearchString b where 
    		tbl.MaHoaDon like '%'+b.Name+'%' 
    	OR tbl.MaHoaDon like '%'+b.Name+'%' 
    	or tbl.MaHangHoa like '%'+b.Name+'%' 
    	or tbl.TenHangHoa like '%'+b.Name+'%'
    	or tbl.TenHangHoa_KhongDau like '%' +b.Name +'%' 
    		or tbl.TenHangHoa_KyTuDau like '%' +b.Name +'%'
    		or tbl.MaNhanVien like '%'+b.Name+'%'
    		or tbl.TenNhanVien like '%'+b.Name+'%'
    		or tbl.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    		or tbl.TenNhanVienKhongDau like '%'+b.Name+'%'
    		or tbl.TenDonViTinh like '%'+b.Name+'%'
    		or tbl.MaKhuyenMai like '%'+b.Name+'%'
    		or tbl.TenKhuyenMai like '%'+b.Name+'%'
    		or tbl.MaDoiTuong like '%'+b.Name+'%'
    		or tbl.TenDoiTuong like '%'+b.Name+'%'
    		or tbl.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or tbl.sHinhThuc like '%'+b.Name+'%'
    		or dbo.FUNC_ConvertStringToUnsign(sHinhThuc) like '%'+b.Name+'%'
    		)=@count or @count=0)
    	order by NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_GetChietKhauHoaDon_ChiTiet]
    @ID_ChietKhauHoaDon [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select nv.ID,nv.MaNhanVien, nv.TenNhanVien, nvpb.NhanVien_PhongBan as TenNhanVien_GC, convert(varchar, NgaySinh,103) as TenNhanVien_CV
    	from ChietKhauMacDinh_HoaDon_ChiTiet ct
    	join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
    	left join (
    	Select distinct hdXML.ID,
    					 (
    						select pb.TenPhongBan +', '  AS [text()]
    						from NS_QuaTrinhCongTac qtct
    						join NS_PhongBan pb on qtct.ID_PhongBan= pb.ID
    						where qtct.ID_NhanVien = hdXML.ID and qtct.ID_DonVi like @ID_DonVi
    						For XML PATH ('')
    					) NhanVien_PhongBan
    				from NS_NhanVien hdXML 
    
    	) nvpb on nvpb.ID= nv.ID 
		where ct.ID_ChietKhauHoaDon like @ID_ChietKhauHoaDon
		and nv.DaNghiViec = 0
		and (nv.TrangThai = 1 or nv.TrangThai is null)
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_NhatKySuDung_GoiDV]
    @idDoiTuongs nvarchar(max),
    @ID_DonVi [nvarchar](max),
	@CurrentPage int,
	@PageSize int
AS
BEGIN
with data_cte
as (
    SELECT hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon as NgayLapHD, hd.ID_Xe,
    	qd.MaHangHoa as MaDichVu,hh.TenHangHoa as TenDichVu, ct.SoLuong,
    	hdXMLOut.HDCT_NhanVien as NhanVienThucHien,
    	CT_ChietKhauNV.TongChietKhau
    	FROM BH_HoaDon_ChiTiet ct
    	join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.id
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    	left join 
    			(Select distinct hdXML.ID,
    					(
    					select distinct (nv.TenNhanVien) +', '  AS [text()]
    					from BH_HoaDon_ChiTiet ct2
    					left join BH_NhanVienThucHien nvth on ct2.ID = nvth.ID_ChiTietHoaDon
    					left join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    					where ct2.ID = hdXML.ID 
    					For XML PATH ('')
    				) HDCT_NhanVien
    			from BH_HoaDon_ChiTiet hdXML) hdXMLOut on ct.ID= hdXMLOut.ID
    	 left join 
    			(select ct3.ID, SUM(isnull(nvth2.TienChietKhau,0)) as TongChietKhau from BH_HoaDon_ChiTiet ct3
    			left join BH_NhanVienThucHien nvth2 on ct3.ID = nvth2.ID_ChiTietHoaDon
    			group by ct3.ID) CT_ChietKhauNV on CT_ChietKhauNV.ID = ct.ID        
    	WHERE ct.ID_ChiTietGoiDV is not null and (ct.ID_ChiTietDinhLuong= ct.id OR ct.ID_ChiTietDinhLuong IS NULL)
		and hd.ChoThanhToan ='0' 
    	and exists (select * from dbo.splitstring(@idDoiTuongs) tbl where hd.ID_DoiTuong= tbl.Name)
		and ct.ChatLieu='4'
		),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(SoLuong) as TongSoLuong,
				sum(TongChietKhau) as TongHoaHong			
			from data_cte
		)
    	select dt.*, convert(varchar,dt.NgayLapHD, 103) as NgayLapHoaDon,
		cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHD desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateIDKhachHang_inSoQuy]
    @ID_HoaDonLienQuan [uniqueidentifier]
AS
BEGIN
    declare @ID_DonVi UNIQUEIDENTIFIER, @ID_NhanVien  UNIQUEIDENTIFIER, @MaHoaDon nvarchar(max), @MaPhieuThus nvarchar(max)
	select @ID_NhanVien = ID_NhanVien, @ID_DonVi = ID_DonVi ,@MaHoaDon = MaHoaDon
	from BH_HoaDon where id= @ID_HoaDonLienQuan

	declare @sDetail nvarchar(max) =concat(N'Cập nhật hóa đơn ', @MaHoaDon , N': hủy phiếu thu ', @MaPhieuThus , N' của khách hàng cũ')

	----- get list quyhoadon old	
		select @MaPhieuThus =
		(
		select distinct qhd.MaHoaDon + ', '  as  [text()]
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
			where qct.ID_HoaDonLienQuan= @ID_HoaDonLienQuan
			and qhd.TrangThai = 1
			for xml path ('')
		) 
	
		--- update satatus = 0 (huy) if change customer of hoadon
		update qhd set qhd.TrangThai=0, qhd.NoiDungThu = CONCAT(qhd.NoiDungThu, ' ', @sDetail)
		from Quy_HoaDon qhd
		where exists (
		select qct.ID from Quy_HoaDon_ChiTiet qct where qct.ID_HoaDonLienQuan= @ID_HoaDonLienQuan and qhd.ID= qct.ID_HoaDon
		)

		insert into HT_NhatKySuDung(ID, ID_DonVi, ID_NhanVien, LoaiNhatKy, ChucNang, NoiDung, NoiDungChiTiet, ThoiGian)
		values (NEWID(), @ID_DonVi, @ID_NhanVien, 2, N'Cập nhật hóa đơn - thay đổi khách hàng ', 
		concat(N'Cập nhật hóa đơn ', @MaHoaDon , N' thay đổi khách hàng'),
		concat(N'Cập nhật hóa đơn ', @MaHoaDon , N': hủy phiếu thu ', @MaPhieuThus , N' của khách hàng cũ'),
		GETDATE())
		
END");

			DropStoredProcedure("[dbo].[BCBanHang_GetGiaVonHDSC]");
			DropStoredProcedure("[dbo].[GetList_ServicePackages_ByMaGoi]");
			DropStoredProcedure("[dbo].[SP_GetInfor_TPDinhLuong]");
			DropStoredProcedure("[dbo].[SP_GetList_ServicePackages_Mua]");
			DropStoredProcedure("[dbo].[SP_PhieuChi_ServicePackage]");
			DropStoredProcedure("[dbo].[SP_PhieuThu_ServicePackage]");
        }
        
        public override void Down()
        {
			DropStoredProcedure("[dbo].[BaoDuong_InsertListDetail_ByNhomHang]");
			DropStoredProcedure("[dbo].[BCBanHang_GetCTHD]");
			DropStoredProcedure("[dbo].[CapNhatThongBaoBaoDuongXe]");
			DropStoredProcedure("[dbo].[GetCustomer_haveBirthday]");
			DropStoredProcedure("[dbo].[GetCustomer_haveTransaction]");
			DropStoredProcedure("[dbo].[GetLichNhacBaoDuong]");
			DropStoredProcedure("[dbo].[GetListComBo_ofCTHD]");
			DropStoredProcedure("[dbo].[GetListNhatKyBaoDuongTheoXe]");
			DropStoredProcedure("[dbo].[GetListThongBao]");
			DropStoredProcedure("[dbo].[GetNhatKyGiaoDich_ofCus]");
			DropStoredProcedure("[dbo].[GetNhatKyBaoDuong_byCar]");
			DropStoredProcedure("[dbo].[GetNhatKySuDung_GDV]");
			DropStoredProcedure("[dbo].[GetTPDinhLuong_ofHoaDon]");
			DropStoredProcedure("[dbo].[HDTraHang_InsertTPDinhLuong]");
			DropStoredProcedure("[dbo].[Insert_LichNhacBaoDuong]");
			DropStoredProcedure("[dbo].[InsertLichNhacBaoDuong_whenQuaHan_orEnoughLanNhac]");
			DropStoredProcedure("[dbo].[SuDungBaoDuong_UpdateStatus]");
			DropStoredProcedure("[dbo].[UpdateHD_UpdateLichBaoDuong]");
			DropStoredProcedure("[dbo].[UpdateLichBD_whenChangeNgayLapHD]");
        }
    }
}
