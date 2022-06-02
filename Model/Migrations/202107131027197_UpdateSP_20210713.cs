namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20210713 : DbMigration
    {
        public override void Up()
        {
			CreateStoredProcedure(name: "[dbo].[BCBanHang_GetGiaVonHDSC]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;

		DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER)
		INSERT INTO @tblChiNhanh
		select Name from splitstring(@IDChiNhanhs);


		---- get listhoadon with all cthd have tax =0	
		select hd.ID,
			sum(iif(ct.TienThue=0 or ct.TienThue is null,1,0)) as CountTax0, 
			count (ct.ID) as CountCT
		into #hdTax0
		from BH_HoaDon_ChiTiet ct
		join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
		join DM_DonVi dv on hd.ID_DonVi= dv.ID
		where hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
		and exists (select ID from @tblChiNhanh dv2 where dv.ID= dv2.ID)
		and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
		group by hd.ID


		--- get giavon of hdsc

		select 
			hd.ID_HoaDon,
			ctxk.ID_ChiTietGoiDV,
			ctxk.ID_ChiTietDinhLuong,
			ctxk.ID_DonViQuiDoi,
			max(isnull(ctxk.GhiChu,'')) as GhiChu,		
			max(isnull(ctxk.GiaVon,'')) as GiaVon,		
			sum(ISNULL(ctxk.SoLuong,0)) AS SoLuongXuat,
			sum(ISNULL(ctxk.SoLuong,0)* ctxk.GiaVon) AS GiaTriXuat		
		into #gvxk
		from BH_HoaDon_ChiTiet ctxk
		join BH_HoaDon hd on ctxk.ID_HoaDon = hd.ID 
		where hd.ChoThanhToan='0' 
		and hd.LoaiHoaDon = 8			
		and hd.ID_PhieuTiepNhan is not null
		group by hd.ID_HoaDon, ctxk.ID_ChiTietGoiDV,  ctxk.ID_DonViQuiDoi,ctxk.ID_ChiTietDinhLuong


		select
		ct.ID,
		ct.ID_HoaDon,
		ct.ID_DonViQuiDoi,
		ct.ID_LoHang,
		ct.SoLuong,
		ct.DonGia,
		ct.TienChietKhau,
		ct.ThanhTien,
		ctsc.GiaVon,
		ctsc.GiaTriXuat,
		ct.GhiChu,
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hd.ID_PhieuTiepNhan,
		hd.ID_DoiTuong,
		hd.ID_NhanVien,
		hd.ID_DonVi,
		hd.DienGiai,
		Case when hd.TongTienHang = 0 then 0 else ct.ThanhTien * ((hd.TongGiamGia + isnull(hd.KhuyeMai_GiamGia,0)) / hd.TongTienHang) end as GiamGiaHD ,
		case when hd.TongTienHang= 0 then 0 
			else case when hdTax0.CountTax0 = hdTax0.CountCT then ct.ThanhTien * (hd.TongTienThue / hd.TongTienHang)
		else ct.TienThue * ct.SoLuong end end as TienThue,			
		case ct.LoaiThoiGianBH
			when 1 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + N' ngày'
			when 2 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + ' tháng'
			when 3 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + ' năm'
		else '' end  as ThoiGianBaoHanh,						
		case ct.LoaiThoiGianBH
				when 1 then DATEADD(DAY, ct.ThoiGianBaoHanh, hd.NgayLapHoaDon)
				when 2 then DATEADD(MONTH, ct.ThoiGianBaoHanh, hd.NgayLapHoaDon)
				when 3 then DATEADD(YEAR, ct.ThoiGianBaoHanh, hd.NgayLapHoaDon)				
		end as HanBaoHanh,
		Case when ct.LoaiThoiGianBH = 1 and DATEADD(DAY, ct.ThoiGianBaoHanh, hd.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH = 2 and DATEADD(MONTH, ct.ThoiGianBaoHanh, hd.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH = 3 and DATEADD(YEAR, ct.ThoiGianBaoHanh, hd.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH in (1,2,3) Then N'Còn hạn'
			else '' end as TrangThai
	from BH_HoaDon_ChiTiet ct
	join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
	join #hdTax0 hdTax0 on hd.ID = hdTax0.ID
	join (	
		select 
			hdsc.ID_ChiTietHD, hdsc.ID_QuiDoiMua, 
			sum(GiaVon) as GiaVon,
			sum(GiaTriXuat) as GiaTriXuat
		from
		(
		select 
			IIF(ctm.ID= ctm.ID_ChiTietDinhLuong or ctm.ID_ChiTietDinhLuong is null, ctm.ID, ctm.ID_ChiTietDinhLuong) as ID_ChiTietHD,
			IIF(ctm.ID= ctm.ID_ChiTietDinhLuong or ctm.ID_ChiTietDinhLuong is null, ctm.ID_DonViQuiDoi, 
			(select ID_DonViQuiDoi from BH_HoaDon_ChiTiet ct
			where ct.ID_ChiTietDinhLuong = ctm.ID_ChiTietDinhLuong and ct.ID_ChiTietDinhLuong = ct.ID)) as ID_QuiDoiMua,			
			isnull(ctxk.GiaVon,0) as GiaVon,
			isnull(ctxk.GiaTriXuat,0) as GiaTriXuat
			from BH_HoaDon_ChiTiet ctm
			join BH_HoaDon hdm on ctm.ID_HoaDon= hdm.ID
			left join #gvxk ctxk on  ctm.ID= ctxk.ID_ChiTietGoiDV 	and ctxk.ID_HoaDon= hdm.id	
		and ctxk.ID_HoaDon= hdm.id
		where hdm.LoaiHoaDon= 25
		and hdm.ChoThanhToan=0
		and exists (select ID_DonVi from @tblChiNhanh dv where hdm.ID_DonVi= dv.ID)
		and hdm.NgayLapHoaDon >= @FromDate and hdm.NgayLapHoaDon < @ToDate
		) hdsc group by hdsc.ID_ChiTietHD, hdsc.ID_QuiDoiMua
	) ctsc on ct.ID= ctsc.ID_ChiTietHD
	where ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID");

			CreateStoredProcedure(name: "[dbo].[GetTongThu_fromHDDatHang]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				ID_Khachhang = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;

	declare @tblChiNhanh table (ID_DonVi varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@IDChiNhanhs);

		---- chi get hdXuly dathang first
    		select 
				ID,
				TienMat, 
				TienATM,
				ChuyenKhoan,
				TienDoiDiem, 
				ThuTuThe, 									
				TienDatCoc,
				TienThu	
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
								join DM_DonVi dv on hd.ID_DonVi= dv.ID
								where hdd.LoaiHoaDon = '3' 	
								and hd.ChoThanhToan = 0
								and (qhd.TrangThai= 1 Or qhd.TrangThai is null)
								and hd.ID_DoiTuong like @ID_Khachhang
								and exists (select ID_DonVi from @tblChiNhanh cn2 where cn2.ID_DonVi= dv.ID)
								and hd.NgayLapHoaDon between @FromDate and @ToDate
    					) d group by d.ID,d.NgayLapHoaDon,ID_HoaDonLienQuan						
			) thuDH
			where isFirst= 1");

			Sql(@"ALTER FUNCTION [dbo].[Diary_BangLuong]
(	
	@ID_BangLuong uniqueidentifier
)
RETURNS nvarchar(max)  
AS
begin 

	Declare @infor nvarchar(max) =
	(select concat(N'', tbl.noidung, N'  <br /> <b> *) </b>Nội dung chi tiết', REPLACE(REPLACE( noidungct,'&lt;','<'),'&gt;','>'))
from
(
select  concat(N' <br /> Mã bảng lương: <a href=""javascript: void(0)"" style = ""cursor: pointer"" onclick = ""GotoBangLuongByMa(',bl.MaBangLuong,')"" > ', bl.MaBangLuong, ' </a> ',
	N' <br /> Tên bảng lương: ', bl.TenBangLuong,
	N' <br /> Kỳ tính lương: ', FORMAT(bl.TuNgay, 'dd/MM/yyyy'), N' đến ', FORMAT(bl.DenNgay, 'dd/MM/yyyy'),
	N' <br /> Ngày công chuẩn: ', (select top 1 NgayCongChuan from NS_BangLuong_ChiTiet where ID_BangLuong = @ID_BangLuong),
	N' <br /> Người tạo: ', nv.TenNhanVien , ' (', nv.MaNhanVien , ')' ,
	N' <br /> Trạng thái: ', IIF(bl.TrangThai = 1, N' Lưu tạm', N' Đã chốt lương'),
	IIF(bl.GhiChu is null or bl.GhiChu = '', '', N',  <br /> Ghi chú: ' + bl.GhiChu)) noidung,	
	
(
		select pluong as [text()]
		from
		(
			select  CONCAT(' <br />  <b> ', ct.MaBangLuongChiTiet, ': </b>', ' <a href=""javascript:void(0)"" >', nv.MaNhanVien, ' </a> - ', nv.TenNhanVien,
						N': Ngày công thực: ', ct.NgayCongThuc,
						N', Lương cơ bản: ', FORMAT(ct.LuongCoBan, '###,###.###'),
						N', Lương chính: ', FORMAT(ct.TongLuongNhan, '###,###.###'),
						N', Lương OT: ', IIF(ct.LuongOT = 0, '0', FORMAT(ct.LuongOT, '###,###.###')),
						N', Phụ cấp cố định: ', IIF(ct.PhuCapCoBan = 0, '0', FORMAT(ct.PhuCapCoBan, '###,###.###')),
						N', Phụ cấp khác: ', IIF(ct.PhuCapKhac = 0, '0', FORMAT(ct.PhuCapKhac, '###,###.###')),
						N', Hoa hồng: ', IIF(ct.ChietKhau = 0, '0', FORMAT(ct.ChietKhau, '###,###.###')),
						N', Giảm trừ: ', IIF(ct.TongTienPhat = 0, '0', FORMAT(ct.TongTienPhat, '###,###.###')),
						N', Tổng lương: ', FORMAT(ct.LuongThucNhan, '###,###.###')) as pluong
			from NS_BangLuong_ChiTiet ct
			join NS_NhanVien nv on ct.ID_NhanVien = nv.ID
			where ct.ID_BangLuong = @ID_BangLuong
		) a
		for xml path('')		
	) noidungct
from NS_BangLuong bl
join NS_NhanVien nv on bl.ID_NhanVienDuyet = nv.ID
where bl.ID = @ID_BangLuong
) tbl)
	return @infor
end");

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
	select dt.ID as ID_KhachHang, dt.MaDoiTuong as MaKhachHang, dt.TenDoiTuong as TenKhachHang, dt.DienThoai, dt.DiaChi, dt.TenNhomDoiTuongs,
		dt.NgayGiaoDichGanNhat,
		hd1.SoLanDen,
		GiaTriMua, 
		GiaTriTra,
		DoanhThu
	from DM_DoiTuong dt  
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
					iif(hd.LoaiHoaDon!= 6, hd.TongTienHang - hd.TongGiamGia - isnull(hd.KhuyeMai_GiamGia,0),0) as GiaTriMua					
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
	and dt.NgayGiaoDichGanNhat >= @NgayGiaoDichFrom and dt.NgayGiaoDichGanNhat < @NgayGiaoDichTo
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

			Sql(@"ALTER FUNCTION [dbo].[CheckExist_ThietLapLuong]
(
	@ID_ChiNhanh uniqueidentifier,
	@ID_NhanVien uniqueidentifier,
	@ID_PhuCap uniqueidentifier,
	@ID_LoaiLuong varchar(40),
	@LoaiLuong int,
	@FromDate datetime,
	@ToDate datetime 
)
RETURNS bit
AS
BEGIN
	declare @count int =0, @exist int ='0'
	if @ToDate = '' set @ToDate = null
	if @ID_LoaiLuong = '' set @ID_LoaiLuong = null
	if @LoaiLuong <5
		begin
			if @ToDate is null 
				select @count = count(ID)
				from NS_Luong_PhuCap where ID!=@ID_PhuCap
				and LoaiLuong <5 
				and TrangThai != 0
				and ID_NhanVien= @ID_NhanVien
				and ID_DonVi= @ID_ChiNhanh
				and (NgayKetThuc is null or (NgayKetThuc is not null and @FromDate <= NgayKetThuc))
				
			else
			select @count = count(ID)
				from NS_Luong_PhuCap where ID!=@ID_PhuCap
				and LoaiLuong <5 
				and TrangThai != 0
				and ID_NhanVien= @ID_NhanVien
				and ID_DonVi= @ID_ChiNhanh
				and ((NgayKetThuc is null and @ToDate >= NgayApDung)
				or (NgayKetThuc is not null 
					and ((@FromDate >= NgayApDung and @FromDate <= NgayKetThuc)
						or (@ToDate <= NgayKetThuc and @ToDate >= NgayApDung)))
				)			

		end
	else
		begin
			if @ToDate is null 
				select @count = count(ID)
					from NS_Luong_PhuCap where ID!=@ID_PhuCap
					and LoaiLuong > 5
					and TrangThai != 0
					and ID_NhanVien= @ID_NhanVien
					and ID_DonVi= @ID_ChiNhanh
					and ID_LoaiLuong = @ID_LoaiLuong
					and (NgayKetThuc is null or (NgayKetThuc is not null and @FromDate <= NgayKetThuc))
			else
				select @count = count(ID)
				from NS_Luong_PhuCap where ID!=@ID_PhuCap
				and LoaiLuong > 5
				and ID_LoaiLuong = @ID_LoaiLuong
				and TrangThai != 0
				and ID_NhanVien= @ID_NhanVien
				and ID_DonVi= @ID_ChiNhanh
				and ((NgayKetThuc is null and @ToDate >= NgayApDung)
				or (NgayKetThuc is not null 
					and ((@FromDate >= NgayApDung and @FromDate <= NgayKetThuc)
						or (@ToDate <= NgayKetThuc and @ToDate >= NgayApDung)))
				)	
		end

		if @count > 0
			set @exist = '1'
	return @exist

END
");

			Sql(@"ALTER PROCEDURE [dbo].[DeleteBangLuongChiTietById]
    @IdBangLuong [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;

				update bs set ID_BangLuongChiTiet = null
				from NS_CongBoSung bs
				join NS_BangLuong_ChiTiet blct on bs.ID_BangLuongChiTiet= blct.ID
				where blct.ID_BangLuong= @IdBangLuong


				update qct set ID_BangLuongChiTiet = null
				from Quy_HoaDon_ChiTiet qct
				join NS_BangLuong_ChiTiet blct on qct.ID_BangLuongChiTiet= blct.ID
				where blct.ID_BangLuong= @IdBangLuong

    			 Delete from NS_BangLuong_ChiTiet where ID_BangLuong=@IdBangLuong;
				 
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetAll_TonKhoDauKy]
    @IDDonVis [nvarchar](max),
    @ToDate [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER);
    	INSERT INTO @tblChiNhanh SELECT Name FROM splitstring(@IDDonVis)
    
    		SELECT 
    			ID_DonViInput,
    			ID_HangHoa, 		
    			ID_LoHang,
    			IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, TonLuyKe_NhanChuyenHang, TonLuyKe) AS TonKho, 
    			IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, GiaVon_NhanChuyenHang, GiaVon) AS GiaVon
    			FROM (
    			SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang, tbltemp.ID_DonViInput ORDER BY tbltemp.ThoiGian DESC) AS RN 
    			FROM (
    				SELECT hd.LoaiHoaDon, dvqd.ID_HangHoa, hd.ID_DonVi, hd.ID_CheckIn, lstDv.ID AS ID_DonViInput, 
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, hdct.TonLuyKe_NhanChuyenHang, hdct.TonLuyKe) AS TonLuyKe,
    					hdct.TonLuyKe_NhanChuyenHang,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, 
    					hdct.GiaVon_NhanChuyenHang, 
    					hdct.GiaVon)/ISNULL(dvqd.TyLeChuyenDoi,1) AS GiaVon,
    					hdct.GiaVon_NhanChuyenHang, 
    					hdct.ID_LoHang ,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
    				FROM BH_HoaDon_ChiTiet hdct
    				JOIN BH_HoaDon hd ON hd.ID = hdct.ID_HoaDon				
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi				
    				INNER JOIN @tblChiNhanh lstDv ON lstDv.ID = hd.ID_DonVi OR (hd.ID_CheckIn = lstDv.ID and hd.YeuCau = '4')				
    				where hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10,18)
    				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID or (hd.ID_CheckIn= dv.ID and hd.LoaiHoaDon= 10))
    				) as tbltemp
    		WHERE tbltemp.ThoiGian < @ToDate) tblTonKhoTemp
    		WHERE tblTonKhoTemp.RN = 1;
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetBangCongChiTiet]
    @ID_NhanVien [nvarchar](max),
    @IDChiNhanhs [nvarchar](max),
    @IDCaLamViecs [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	declare @tblCa table (ID_CaLamViec uniqueidentifier);
    	if @IDCaLamViecs='' or @IDCaLamViecs='%%'
    		insert into @tblCa
    		select ID from NS_CaLamViec
    	else
    		insert into @tblCa
    		select Name from dbo.splitstring(@IDCaLamViecs);
    
    		with data_cte
    		as(
    		select bs.ID, bs.ID_CaLamViec as ID_Ca, ca.TongGioCong as TongGioCong1Ca, ca.MaCa, ca.TenCa,  ca.GioVao, ca.GioRa, 
    				bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon,
    				bs.NgayTao, bs.NguoiTao, bs.GhiChu					
    			from NS_CongBoSung bs
    			join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID				
    			where bs.ID_NhanVien= @ID_NhanVien
    			and bs.NgayCham>= @FromDate and bs.NgayCham <=@ToDate
    			and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where bs.ID_DonVi= dv.Name)
    			--and exists (select ID_CaLamViec from @tblCa ca2 where ct.ID_CaLamViec= ca2.ID_CaLamViec)
    			),
    			count_cte 
    			as(
    				select count(id) as TotalRow,
    						CEILING(COUNT(id) / CAST(@PageSize as float )) as TotalPage,
    						Sum(SoGioOT) as TongSoGioOT,
    						cast(Sum(SoPhutDiMuon) as float) as TongSoPhutDiMuon
    				from data_cte
    			)
    			select *
    			from data_cte dt
    			cross join count_cte ct 
    			order by dt.NgayCham
    			offset (@CurrentPage * @PageSize) Rows
    			fetch next @PageSize Rows only
END");
			
			Sql(@"ALTER PROCEDURE [dbo].[getList_TongQuanKhachHang]
    @ID_ChiNhanh [uniqueidentifier],
    @timeStart [datetime],
    @timeEnd [datetime]
AS
BEGIN
   
SELECT
CAST(ROUND(SUM(k.KhachHangTaoMoiThangNay), 0) as float) as KhachHangGiaoDichLanDau,
CAST(ROUND(SUM(k.KhachHangQuayLaiThangNay), 0) as float) as KhachHangQuayLai,
CAST(ROUND(SUM(k.KhachHangTaoMoi), 0) as float) as KhachHangTaoMoi
FROM
(
    Select 
	Count (*) as KhachHangTaoMoiThangNay,
	0 as KhachHangQuayLaiThangNay,
	0 as KhachHangTaoMoi
	from 
	(select 
		dt.ID
	from DM_DoiTuong dt 		
	join BH_HoaDon bh on bh.ID_DoiTuong = dt.ID
	where (dt.TheoDoi is null or dt.TheoDoi = 0)
	and bh.ChoThanhToan='0'
	and dt.LoaiDoiTuong = 1
	and dt.ID not like '%00000000-0000-0000-0000-000000000000%'
	and bh.ID_DonVi = @ID_ChiNhanh
	and bh.NgayLapHoaDon >= @timeStart and bh.NgayLapHoaDon < @timeEnd
	and dt.NgayTao >= @timeStart and dt.NgayTao < @timeEnd  -- taomoi + co giaodich thang nay
	GROUP by dt.ID 
	)a
	Union all
	Select 
	0 as KhachHangTaoMoiThangNay,
	Count (*) as KhachHangQuayLaiThangNay,
	0 as KhachHangTaoMoi
	from 
	(select 
	dt.ID, COUNT(*) as solan
	from DM_DoiTuong dt 	
	left join BH_HoaDon bh on bh.ID_DoiTuong = dt.ID
	where (dt.TheoDoi is null or dt.TheoDoi = 0)
	and bh.ChoThanhToan='0'
	and bh.ID_DoiTuong not like '%00000000-0000-0000-0000-000000000000%'
	and dt.LoaiDoiTuong = 1
	and bh.ID_DonVi = @ID_ChiNhanh
	and (bh.NgayLapHoaDon >= @timeStart and bh.NgayLapHoaDon < @timeEnd and dt.NgayTao < @timeStart) --- giaodich thang nay, nhưng taomoi < thangnay
	GROUP by dt.ID
	)a
	Union all
	Select 
	0 as KhachHangTaoMoiThangNay,
	0 as KhachHangQuayLaiThangNay,
	COUNT(*) as KhachHangTaoMoi -- taomoi thangnay (co the co giao dichh hoac khong)
	from 
	(select 
	dt.ID, COUNT(*) as solan
	from DM_DoiTuong dt 
	where (dt.TheoDoi is null or dt.TheoDoi = 0)
	and dt.LoaiDoiTuong = 1
	and dt.ID_DonVi = @ID_ChiNhanh
	and dt.ID not like '%000000000000%'
	and dt.NgayTao >= @timeStart and dt.NgayTao < @timeEnd
	GROUP by dt.ID
	)a
) as k
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetLuongChinhTheoCaGio]
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

	SET NOCOUNT ON;	

		--declare @IDChiNhanhs uniqueidentifier='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
		--declare @FromDate datetime='2021-05-01'
		--declare @ToDate datetime='2021-06-01'
		--declare @IDNhanViens nvarchar(max) ='E2A78417-084F-44F4-B557-ACC1BB205C78'

		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate


		---- get caidatluong (ca + gio)
		declare @tblLuongCaGio table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, 
		LuongCoBan float, HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblLuongCaGio
		select *
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong in (3,4) 
		order by NgayApDung	

		--select * from @tblLuongCaGio

		-- bảng tính công theo phiếu phân ca, ca làm việc
		declare @tblCong table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, LoaiLuong float, 
		LoaiNgayThuong_Nghi int, LuongCoBan float,LuongCoBanQuyDoi float, HeSoLuong int, LaPhanTram int, NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, TongCong float,
		ThanhTien float)				
	
		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int,
		@LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblLuongCaGio 
		OPEN curLuong 
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan,@HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				select ct.*
				into #tmpca
				from NS_ThietLapLuongChiTiet ct
				where ID_LuongPhuCap= @ID_PhuCap and LaOT= 0			
	
			
				insert into @tblCong
				select @ID_NhanVien, @ID_PhuCap, @LoaiLuong, 
					tbl.LoaiNgayThuong_Nghi, tbl.LuongCoBan, tbl.LuongCoBanQuyDoi, tbl.HeSoLuong, tbl.LaPhanTram,
					@NgayApDung, @NgayKetThuc,
					tbl.ID_CaLamViec, tbl.TenCa, tbl.TongGioCong1Ca,
					tbl.TongCong,
					case when @LoaiLuong = 3 then LuongCoBanQuyDoi * tbl.Cong else (tbl.Cong * TongGioCong1Ca - SoPhutDiMuon * 1.0/60 +  SoGioOT) * LuongCoBanQuyDoi  end as ThanhTien						
				from
					(
					select 
							luong.ID_CaLamViec, luong.TenCa, luong.TongGioCong1Ca,
							luong.Cong, luong.SoPhutDiMuon, luong.SoGioOT, 
							luong.LuongNgayThuong, luong.LoaiNgayThuong_Nghi,
							luong.GiaTri as HeSoLuong,luong.LaPhanTram,
							case when @LoaiLuong= 3 then luong.Cong else luong.Cong * luong.TongGioCong1Ca - luong.SoPhutDiMuon * 1.0/60 +  luong.SoGioOT end as TongCong,	
							@HeSo * luong.LuongNgayThuong as LuongCoBan,
							case when luong.LaPhanTram= 1 then @HeSo * luong.LuongNgayThuong * luong.GiaTri/100 else @HeSo * luong.GiaTri end as LuongCoBanQuyDoi
					
						from 
						(
							select 
								qd.ID_CaLamViec, qd.TenCa, qd.TongGioCong1Ca,
								qd.Cong, qd.SoPhutDiMuon, qd.SoGioOT,
								isnull(qd.LuongNgayThuong, LuongNgayThuongNull) as LuongNgayThuong,

								case when ngayle.ID is null then
									case qd.Thu
										when 6 then 6
										when 0 then 0
									else 3 end
								else qd.LoaiNgay end LoaiNgayThuong_Nghi,
								
								case when ngayle.ID is null then
									case qd.Thu
										when 6 then case when Thu7_GiaTri is not null then qd.Thu7_GiaTri else Thu7_GiaTriNull end
										when 0 then case when ThCN_GiaTri is not null then qd.ThCN_GiaTri else ThCN_GiaTriNull end
									else case when qd.LuongNgayThuong is not null then LuongNgayThuong else LuongNgayThuongNull end
									end
								else 
									case qd.LoaiNgay
										when 1 then case when qd.NgayNghi_GiaTri is not null then NgayNghi_GiaTri else NgayNghi_GiaTriNull end
										when 2 then case when qd.NgayLe_GiaTri is not null then NgayLe_GiaTri else NgayLe_GiaTriNull end
									else case when qd.LuongNgayThuong is not null then LuongNgayThuong else LuongNgayThuongNull end
								end end as GiaTri,

								case when ngayle.ID is null then
									case qd.Thu
										when 6 then case when Thu7_LaPhanTramLuong is not null then qd.Thu7_LaPhanTramLuong else Thu7_LaPhanTramLuongNull end
										when 0 then case when CN_LaPhanTramLuong is not null then qd.CN_LaPhanTramLuong else CN_LaPhanTramLuongNull end
									else 0
									end
								else 
									case qd.LoaiNgay
										when 1 then case when qd.NgayNghi_LaPhanTramLuong is not null then NgayNghi_LaPhanTramLuong else NgayNghi_LaPhanTramLuongNull end
										when 2 then case when qd.NgayLe_LaPhanTramLuong is not null then NgayLe_LaPhanTramLuong else NgayLe_LaPhanTramLuongNull end
									else 0
								end end as LaPhanTram															
							from
							(
								select tmp.*, 
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) LuongNgayThuong,
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) LuongNgayThuongNull,

									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_GiaTri,
									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_GiaTriNull,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_LaPhanTramLuong,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_LaPhanTramLuongNull,
						
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) CN_LaPhanTramLuong,
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) CN_LaPhanTramLuongNull,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) ThCN_GiaTri,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) ThCN_GiaTriNull,

									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_GiaTri,
									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_GiaTriNull,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_LaPhanTramLuong,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_LaPhanTramLuongNull,

									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_GiaTri,
									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_GiaTriNull,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_LaPhanTramLuong,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_LaPhanTramLuongNull
						
								from @tblCongThuCong tmp								
								where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
							) qd
							left join NS_NgayNghiLe ngayle on qd.NgayCham= ngayle.Ngay and ngayle.TrangThai='1'
						)luong
					)tbl
					drop table #tmpca
										
				FETCH NEXT FROM curLuong 
				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	


			select ID_NhanVien, LoaiLuong, LoaiNgayThuong_Nghi, LuongCoBan, LuongCoBanQuyDoi,  HeSoLuong, LaPhanTram,
				NgayApDung, NgayKetThuc,
				ID_CaLamViec,TenCa, TongGioCong1Ca,	
				sum(TongCong) as SoNgayDiLam, 
				sum(ThanhTien) as ThanhTien		
			from @tblCong
			group by ID_NhanVien,LoaiLuong, LoaiNgayThuong_Nghi, LuongCoBan,LuongCoBanQuyDoi, HeSoLuong, LaPhanTram,
				NgayApDung, NgayKetThuc,TenCa, TongGioCong1Ca,ID_CaLamViec
				order by ID_CaLamViec
END

--exec GetLuongChinhTheoCaGio");

			Sql(@"ALTER PROCEDURE [dbo].[GetNhatKyGiaoDich_ofKhachHang]
    @IDChiNhanhs [nvarchar](max),
    @ID_KhachHang [nvarchar](max),
    @LoaiChungTu [nvarchar](10),
    @TextSearch [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs);

		declare @tblThuDH table (ID uniqueidentifier,
				TienMat float, 
				TienATM float, 
				ChuyenKhoan float, 
				TienDoiDiem float,  
				ThuTuThe float,  									
				TienDatCoc float, 
				TienThu	float)
		insert into @tblThuDH
		exec GetTongThu_fromHDDatHang @IDChiNhanhs,@ID_KhachHang,@FromDate, @ToDate;

    
    	with data_cte
    	as(
    	select hd.ID, hd.LoaiHoaDon, hd.MaHoaDon, hd.NgayLapHoaDon, hd.NgayApDungGoiDV, hd.HanSuDungGoiDV, 
    		iif(hd.LoaiHoaDon=22, hd.TongChiPhi, hd.TongTienHang) as TongTienHang,
    		hd.TongTienThue, hd.TongGiamGia, hd.TongChiPhi, hd.TongChietKhau, hd.PhaiThanhToan, 
    		dv.TenDonVi,
    		TienMat, TienGui, ThuTuThe, TienThu as DaThanhToan,
    		case  hd.LoaiHoaDon
    			when 1 then N'Hóa đơn bán lẻ'
    			when 19 then N'Gói dịch vụ'
    			when 22 then N'Thẻ giá trị'
				when 25 then N'Hóa đơn sửa chữa'
    		end as SLoaiHoaDon
    	from BH_HoaDon hd
    	join DM_DonVi dv on hd.ID_DonVi= dv.ID
    	left join (
			
			select 
				thuchi.ID_HoaDonLienQuan,
				sum(TienMat) as TienMat,
				sum(TienGui) as TienGui,
				sum(ThuTuThe) as ThuTuThe,
				sum(TienThu) as TienThu
			from
			(				
				--- thongthu of hoadon
    			select qct.ID_HoaDonLienQuan, 
    				sum(qct.TienMat) as TienMat, 
    				sum(qct.TienGui) as TienGui, 
    				sum(qct.ThuTuThe) as ThuTuThe,
    				sum(iif(qhd.LoaiHoaDon= 11, qct.TienThu, -qct.TienThu)) as TienThu
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			where qhd.TrangThai = 1			
				and qct.ID_DoiTuong= @ID_KhachHang
    			group by qct.ID_HoaDonLienQuan

				union all

				---- tongthu from dathang
				select 
					ID,
					TienMat,
					TienATM,
					ThuTuThe,
					TienThu
				from @tblThuDH		
				) thuchi group by thuchi.ID_HoaDonLienQuan
    		) quy on quy.ID_HoaDonLienQuan = hd.ID
    	where hd.ID_DoiTuong= @ID_KhachHang
    	and exists (select Name from dbo.splitstring(@LoaiChungTu) ct where hd.LoaiHoaDon = ct.Name )
    	and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
    	and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    ),
    count_cte
    as(
    	select COUNT(ID) as TotalRow,
    		 CEILING(COUNT(ID)/ CAST(@PageSize as float)) as TotalPage,
    		 sum(TongTienHang) as SumTienHang,
    		 sum(TongTienThue) as SumTienThue,
    		 sum(TongGiamGia) as SumGiamGia,
    		 sum(PhaiThanhToan) as SumPhaiThanhToan,
    		 sum(TongChiPhi) as SumChiPhi,
    		 sum(TongChietKhau) as SumChietKhau,
    		 sum(TienMat) as SumTienMat,
    		 sum(TienGui) as SumTienGui,
    		 sum(ThuTuThe) as SumThuTuThe,
    		 sum(DaThanhToan) as SumDaThanhToan
    	from data_cte
    )
    select *
    from data_cte dt
    cross join count_cte cte
    order by dt.NgayLapHoaDon desc
    offset (@CurrentPage * @PageSize) rows
    fetch next @PageSize rows only
END");

			Sql(@"ALTER PROCEDURE [dbo].[JqAuto_HoaDonSC]
    @IDChiNhanhs [nvarchar](max),
	@ID_PhieuTiepNhan [nvarchar](max),
    @TextSearch [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;

	-- get list HSSC by chinhah
		select  hd.ID, hd.MaHoaDon, hd.ID_PhieuTiepNhan, hd.NgayLapHoaDon
		into #temp
    	from BH_HoaDon hd
		where hd.LoaiHoaDon= 25
		and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where dv.Name= hd.ID_DonVi )
    	and hd.ID_PhieuTiepNhan is not null
    	and hd.ChoThanhToan= 0
		and hd.ID_PhieuTiepNhan like @ID_PhieuTiepNhan

	select hdsc.*, tn.MaPhieuTiepNhan, xe.BienSo
	from #temp hdsc	
	join 
	(
			select tbl2.id, tbl2.SoLuongConLai
			from(
				select tbl.ID,
					sum(tbl.SoLuongBan) - sum(isnull(tbl.SoLuongTra,0)) as SoLuongConLai
				from
				(
					-- sum (slMua) from hdsc
					Select 
    					hd.ID,
    					Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongBan,
						0 as SoLuongTra
    				from #temp hd    			
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon   
					join DonViQuiDoi qd on hdct.ID_DonViQuiDoi = qd.ID 
					join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
					--where hh.LaHangHoa='1' -- chi get hanghoa
    				Group by hd.ID

					-- sum (slXuatKho) from hdxk
					union all
					select 
    					hdsc.ID,
    					0 as SoLuongBan,						
    					Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongTra
    				from BH_HoaDon hdxk
					join #temp hdsc on hdxk.ID_HoaDon = hdsc.ID
    				join BH_HoaDon_ChiTiet hdct on hdxk.ID = hdct.ID_HoaDon
    				where hdct.ID_ChiTietGoiDV is not null
    				and hdxk.ChoThanhToan='0'    			
    				Group by hdsc.ID
				)tbl group by tbl.ID
				)tbl2
	) hdsCL on hdsc.ID = hdsCL.ID
	join Gara_PhieuTiepNhan tn on hdsc.ID_PhieuTiepNhan = tn.ID
   join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
   where ---- do 1so phieutiepnhan da xuatxuong (TrangThai = 3) nhung khong luu ngayxuatxuong
   tn.TrangThai in (1,2)
    	and (tn.MaPhieuTiepNhan like @TextSearch 
    		or xe.BienSo like @TextSearch 		
    		or hdsc.MaHoaDon like @TextSearch 		
    		)	
	order by tn.NgayVaoXuong desc , hdsc.NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[NangNhom_KhachHangbyID]
    @ID_DoiTuong [uniqueidentifier],
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;

	--declare @ID_DoiTuong uniqueidentifier ='5d51cec8-8b2d-4e48-a5f6-4ce68707387d',
	--@ID_ChiNhanh uniqueidentifier ='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'

	------ xoanhom in DM_Doituong_Nhom by ID_DoiTuong: if autoUpdate = true & add again
	delete dtn
	from DM_DoiTuong_Nhom dtn  
	where ID_DoiTuong= @ID_DoiTuong
	and exists (select ID from DM_NhomDoiTuong nhom where dtn.ID_NhomDoiTuong = nhom.ID and TuDongCapNhat='1')

	commit; ---- commit transation to exec trigger delete in tbl DM_NhomDoiTuong


   declare @ThangSinh int, @Tuoi int, @GioiTinh bit, @ID_TinhThanh uniqueidentifier, @NgaySinh_NgayTLap datetime, @LoaiNgaySinh int 
    
    	-- get ngaysinh, gioitinh, tinhthanh
    select @GioiTinh = GioiTinhNam, @ThangSinh = ThangSinh, @Tuoi = Tuoi, @NgaySinh_NgayTLap = NgaySinh_NgayTLap,
    @LoaiNgaySinh = LoaiNgaySinh, @ID_TinhThanh = ID_TinhThanh
    from
    (
    select ID_TinhThanh, GioiTinhNam,  DATEDIFF(year, NgaySinh_NgayTLap, 
    GETDATE()) as Tuoi, DATEPART(month, NgaySinh_NgayTLap) as ThangSinh, NgaySinh_NgayTLap, DinhDang_NgaySinh,
    		case  DinhDang_NgaySinh
    		 when 'dd/MM/yyyy' then 1
    		 when 'dd/MM' then 2
    		 when 'MM/yyyy' then 3
    		 when 'yyyy' then 4
    		 end as LoaiNgaySinh
    from DM_DoiTuong where ID = @ID_DoiTuong
    ) a
    
    declare @NoHienTai float = 0, @TongBan float =0, @TongBanTruTraHang float =0, @SoLanMuaHang int =0
    -- doanhthu, congno,..
     select 
    @NoHienTai =  NoHienTai,
    @TongBan = TongBan,
    @TongBanTruTraHang = TongBanTruTraHang,
    @SoLanMuaHang = SoLanMuaHang
    from 
    (
    SELECT tblThuChi.ID_DoiTuong,
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) - 
    						SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
    					sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
    
    
    	SELECT 
    	bhd.ID_DoiTuong,
    	0 AS GiaTriTra,
    	ISNULL(bhd.PhaiThanhToan,0) AS DoanhThu,
    	0 AS TienThu,
    	0 AS TienChi,
    	0 AS SoLanMuaHang,
    		0 as ThuTuThe
    FROM BH_HoaDon bhd
    WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = 0
    	AND bhd.ID_DoiTuong = @ID_DoiTuong
    AND bhd.ID_DonVi = @ID_ChiNhanh
    
    
    		union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,
    							ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
    									0 as ThuTuThe
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = 0
    							AND bhd.ID_DoiTuong = @ID_DoiTuong			
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    
    							-- tt= the
    							union all
    							select 
    								hd.ID_DoiTuong,
    								0 as GiaTriTra,
    							0 AS DoanhThu,
    								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
    								qct.ThuTuThe
    							from Quy_HoaDon qhd 
    							join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    							join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
    							where qhd.TrangThai= 1
    							and qct.ThuTuThe > 0
    							and qhd.LoaiHoaDon= 11
    							and hd.ID_DoiTuong = @ID_DoiTuong			
    							AND hd.ID_DonVi = @ID_ChiNhanh
    
    
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
    							Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
							and (qhdct.HinhThucThanhToan is null or qhdct.HinhThucThanhToan !=6)
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    							AND qhdct.ID_DoiTuong = @ID_DoiTuong
    							
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
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
							and (qhdct.HinhThucThanhToan is null or qhdct.HinhThucThanhToan !=6)
    								AND qhdct.ID_DoiTuong = @ID_DoiTuong
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    
    							Union All
    							-- solan mua hang
    						Select 
    							hd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    								0 as TienChi,
    							COUNT(*) AS SoLanMuaHang	,
    									0 as ThuTuThe
    						From BH_HoaDon hd 
    						where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    						and hd.ChoThanhToan = 0
    						AND hd.ID_DoiTuong = @ID_DoiTuong
    						
    							GROUP BY hd.ID_DoiTuong  	
    							) tblThuChi group by tblThuChi.ID_DoiTuong
    	) a		
    	
       
	insert into DM_DoiTuong_Nhom   
    select  NewID(), @ID_DoiTuong, ID_Nhom
    from 
    (
    select *
    from
    (
    select b.ID_Nhom, b.TenNhomDoiTuong, 
   
    sum(b.TongBanTruTraHang) as TongBanTruTraHang,
    sum(b.TongBan) as TongBan,
	sum(b.SoLanMuaHang) as SoLanMuaHang,
    sum(b.CongNo) as CongNo,
	sum(b.ThangSinh) as ThangSinh,
	sum(b.Tuoi) as Tuoi,
	sum(b.GioiTinh) as GioiTinh,
	sum(b.TinhThanh) as TinhThanh,
	count(*) as countDK    
    from
    (

    select TenDonVi, nhom.id as ID_Nhom, TenNhomDoiTuong, GiamGia, GiamGiaTheoPhanTram, TuDongCapNhat,
    ct.LoaiDieuKien, ct.LoaiSoSanh, ct.GiaTriSo, ct.GiaTriBool, ct.GiaTriThoiGian, ct.GiaTriKhuVuc, ct.GiaTriVungMien,
    case ct.LoaiDieuKien
    when 1 then 
    	case ct.LoaiSoSanh
    		when 1 then iif(@TongBanTruTraHang > ct.GiaTriSo,1,-1)
    		when 2 then iif(@TongBanTruTraHang >= ct.GiaTriSo,1,-1)
    		when 3 then iif(@TongBanTruTraHang = ct.GiaTriSo,1,-1)
    		when 4 then iif(@TongBanTruTraHang <= ct.GiaTriSo,1,-1)
    		when 5 then iif(@TongBanTruTraHang < ct.GiaTriSo,1,-1)
    		end 
    	else 0  end as TongBanTruTraHang,
    	case ct.LoaiDieuKien
    when 2 then 
    	case ct.LoaiSoSanh
    		when 1 then iif(@TongBan > ct.GiaTriSo, 1,-1)
    		when 2 then iif(@TongBan >= ct.GiaTriSo, 1,-1)
    		when 3 then iif(@TongBan = ct.GiaTriSo, 1,-1)
    		when 4 then iif(@TongBan >= ct.GiaTriSo, 1,-1)
    		when 5 then iif(@TongBan < ct.GiaTriSo, 1,-1)
    		end 
    	else 0 end as TongBan,
    	case ct.LoaiDieuKien
    when 4 then 
    	case ct.LoaiSoSanh
    		when 1 then iif(@SoLanMuaHang > ct.GiaTriSo ,1,-1)
    		when 2 then iif(@SoLanMuaHang >= ct.GiaTriSo ,1,-1)
    		when 3 then iif(@SoLanMuaHang = ct.GiaTriSo ,1,-1)
    		when 4 then iif(@SoLanMuaHang <= ct.GiaTriSo ,1,-1)
    		when 5 then iif(@SoLanMuaHang < ct.GiaTriSo ,1,-1)
    		end 
    	else 0 end as SoLanMuaHang,
    	case ct.LoaiDieuKien
    when 5 then 
    	case ct.LoaiSoSanh
    		when 1 then iif(@NoHienTai > ct.GiaTriSo ,1,-1)
    		when 2 then iif(@NoHienTai >= ct.GiaTriSo ,1,-1)
    		when 3 then iif(@NoHienTai = ct.GiaTriSo ,1,-1)
    		when 4 then iif(@NoHienTai <= ct.GiaTriSo ,1,-1)
    		when 5 then iif(@NoHienTai < ct.GiaTriSo ,1,-1)
    		end 
    	else 0 end as CongNo,
    
    	case ct.LoaiDieuKien
    when 6 then 
    	case ct.LoaiSoSanh
    		when 1 then iif(@ThangSinh > ct.GiaTriSo ,1,-1)
    		when 2 then iif(@ThangSinh >= ct.GiaTriSo ,1,-1)
    		when 3 then iif(@ThangSinh = ct.GiaTriSo ,1,-1)
    		when 4 then iif(@ThangSinh <= ct.GiaTriSo ,1,-1)
    		when 5 then iif(@ThangSinh < ct.GiaTriSo ,1,-1)
    		end 
    	else 0 end as ThangSinh,
    	case ct.LoaiDieuKien
    	when 7 then 
    	case ct.LoaiSoSanh
    		when 1 then iif(@Tuoi > ct.GiaTriSo ,1,-1)
    		when 2 then iif(@Tuoi >= ct.GiaTriSo ,1,-1)
    		when 3 then iif(@Tuoi = ct.GiaTriSo ,1,-1)
    		when 4 then iif(@Tuoi <= ct.GiaTriSo ,1,-1)
    		when 5 then iif(@Tuoi < ct.GiaTriSo ,1,-1)
    		end 
    	else 0 end as Tuoi,
    	case ct.LoaiDieuKien
    	when 8 then 
    	case ct.LoaiSoSanh		
    		when 3 then iif(@GioiTinh = ct.GiaTriBool ,1,-1)
    		else iif(@GioiTinh != ct.GiaTriBool ,1,-1)
    		end 
    	else 0 end as GioiTinh,
    	case ct.LoaiDieuKien
    	when 9 then 
    	case ct.LoaiSoSanh
    		when 1 then iif(@ID_TinhThanh > ct.GiaTriKhuVuc ,1,-1)
    		when 2 then iif(@ID_TinhThanh >= ct.GiaTriKhuVuc ,1,-1)
    		when 3 then iif(@ID_TinhThanh = ct.GiaTriKhuVuc ,1,-1)
    		when 4 then iif(@ID_TinhThanh <= ct.GiaTriKhuVuc ,1,-1)
    		when 5 then iif(@ID_TinhThanh < ct.GiaTriKhuVuc ,1,-1)
    		end 
    	else 0 end as TinhThanh,
		ndv.ID_DonVi
    
    from DM_NhomDoiTuong nhom
    join DM_NhomDoiTuong_ChiTiet ct on nhom.ID= ct.ID_NhomDoiTuong
    left join NhomDoiTuong_DonVi ndv on nhom.ID= ndv.ID_NhomDoiTuong
    left join DM_DonVi dv on dv.ID= ndv.ID_DonVi
    where ndv.ID_DonVi= @ID_ChiNhanh
	and nhom.TuDongCapNhat='1'
    )	b group by b.ID_Nhom, b.TenNhomDoiTuong
    ) c where TongBanTruTraHang + TongBan +  SoLanMuaHang +  CongNo + ThangSinh + Tuoi + GioiTinh + TinhThanh = countDK
    )  nhomdudk 
    where 
	not exists (select ID_NhomDoiTuong from DM_DoiTuong_Nhom dtn where ID_DoiTuong= @ID_DoiTuong 
	and nhomdudk.ID_Nhom = dtn.ID_NhomDoiTuong) -- chỉ insert nếu chưa dc add
	commit;
end
");

			Sql(@"ALTER PROCEDURE [dbo].[ReportValueCard_Balance]
    @TextSearch [nvarchar](max),
    @ID_ChiNhanhs [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	with data_cte
    	as (
    		select 
    				tblView.ID, tblView.MaDoiTuong, tblView.TenDoiTuong, 
    				ISNULL(tblView.DienThoai,'') as DienThoaiKhachHang,
    				CAST(ISNULL(tblView.SoDuDauKy,0) as float) as SoDuDauKy,
    				CAST(ISNULL(tblView.PhatSinhTang,0) as float) as PhatSinhTang,
    				CAST(ISNULL(tblView.PhatSinhGiam,0) as float) as PhatSinhGiam,
    				ISNULL(tblView.SoDuDauKy,0) + ISNULL(tblView.PhatSinhTang,0)- ISNULL(tblView.PhatSinhGiam,0) as SoDuCuoiKy,
    				case when tblView.TrangThai_TheGiaTri is null or tblView.TrangThai_TheGiaTri = 1 then N'Đang hoạt động'
    				else N'Ngừng hoạt động' end as TrangThai_TheGiaTri,
    				TrangThai
    		from 
    		(
    			select 
    				dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, 
    				dt.TrangThai_TheGiaTri,
    				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
    				else '12' end as TrangThai, -- used to where TrangThai_TheGiaTri (1: all, 11: dang hoat dong, 2. Ngung hoat dong)
    				dt.DienThoai,
    				tblTemp.SoDuDauKy,
    				tblTemp.PhatSinhTang,
    				tblTemp.PhatSinhGiam
    			from DM_DoiTuong dt
    			left join 
    			( 
    				select 
    					ID_DoiTuong,
    					SUM(ISNULL(TongThuTheGiaTri,0)) as TongThuTheGiaTri,
    					SUM(ISNULL(SuDungThe,0)) as SuDungThe,
    					SUM(ISNULL(HoanTraTheGiatri,0)) as HoanTraTheGiaTri,
    					SUM(ISNULL(TongThuTheGiaTri,0))  - SUM(ISNULL(SuDungThe,0)) + SUM(ISNULL(HoanTraTheGiatri,0)) as SoDuDauKy,
    					SUM(ISNULL(PhatSinh_ThuTuThe,0)) + SUM(ISNULL(PhatSinh_HoanTraTheGiatri,0)) + SUM(ISNULL(PhatSinhTang_DieuChinhThe,0)) as PhatSinhTang,
    					SUM(ISNULL(PhatSinh_SuDungThe,0)) + SUM(ISNULL(PhatSinhGiam_DieuChinhThe,0)) as PhatSinhGiam
    
    				from (
    					 --- thu the gtri trước thời gian tìm kiếm
    						 select ID as ID_DoiTuong, 
    							SUM(ISNULL(TongThuTheGiaTri,0)) as TongThuTheGiaTri,
    							 null as SuDungThe,
    							 null as HoanTraTheGiatri,						 
    							 null as PhatSinh_ThuTuThe,
    							 null as PhatSinh_SuDungThe,
    							 null as PhatSinh_HoanTraTheGiatri,
    								 null as PhatSinhTang_DieuChinhThe,
    								 null as PhatSinhGiam_DieuChinhThe
    						 from (
    							 SELECT dt.ID, 
    								 case when (hd.LoaiHoaDon=22  or hd.LoaiHoaDon=23) then sum(hd.TongTienHang)
    								 else 0 end as TongThuTheGiaTri
    							 from DM_DoiTuong dt
    							 left join BH_HoaDon hd on hd.ID_DoiTuong = dt.ID
    							 where  FORMAT(hd.NgayLapHoaDon,'yyyy-MM-dd') < @DateFrom 
    							 and hd.ChoThanhToan='0' --and hd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    							 group by dt.ID, hd.LoaiHoaDon
    						 ) tblThuThe group by tblThuThe.ID
    
    					 union all
    					 -- su dung the giatri
    						 select tblSuDungThe.ID_DoiTuong, 
    						  null as TongThuTheGiaTri,
    							 sum(ISNULL(SuDungThe,0)) as SuDungThe,
    							 null as HoanTraTheGiatri,						
    							 null as PhatSinh_ThuTuThe,
    							 null as PhatSinh_SuDungThe,
    							 null as PhatSinh_HoanTraTheGiatri,
    								 null as PhatSinhTang_DieuChinhThe,
    								 null as PhatSinhGiam_DieuChinhThe
    			
    						 from (
    							 SELECT qct.ID_DoiTuong,
    								case when qhd.LoaiHoaDon= 12 then 0 else sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0)) end as SuDungThe
    							 from Quy_HoaDon_ChiTiet qct
    							 left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    							 left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    							 where  FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') < @DateFrom 
    							 --and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    							 and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    							 and hd.ChoThanhToan ='0'
    							 group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
    						 ) tblSuDungThe group by tblSuDungThe.ID_DoiTuong
    
    				 union all
    					  -- hoan tra tien the giatri
    						select ID_DoiTuong, 
    							null as TongThuTheGiaTri,
    							null as SuDungThe,
    							SUM(ISNULL(HoanTraTheGiatri,0)) as HoanTraTheGiatri,						
    							null as PhatSinh_ThuTuThe,
    							null as PhatSinh_SuDungThe,
    							null as PhatSinh_HoanTraTheGiatri,
    								null as PhatSinhTang_DieuChinhThe,
    								null as PhatSinhGiam_DieuChinhThe
    						from (
    								SELECT qct.ID_DoiTuong,
    								case when qhd.LoaiHoaDon= 11 then 0 else sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0)) end as HoanTraTheGiatri
    								from Quy_HoaDon_ChiTiet qct
    								left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    								left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    								where  FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') < @DateFrom 
    								--and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    								and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    								and hd.ChoThanhToan ='0'
    								group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
    							) tblSuDungThe group by tblSuDungThe.ID_DoiTuong 
    
    				 union all
    					   --- thu the gtri tại thời điểm hiện tại
    						 select ID_DoiTuong, 
    					 		 null as TongThuTheGiaTri,
    							 null as SuDungThe,
    							 null as HoanTraTheGiatri,
    							 SUM(ISNULL(TongThuTheGiaTri,0)) as PhatSinh_ThuTuThe,
    							 null as PhatSinh_SuDungThe,
    							 null as PhatSinh_HoanTraTheGiatri,
    								 null as PhatSinhTang_DieuChinhThe,
    								 null as PhatSinhGiam_DieuChinhThe
    						 from (
    							 SELECT hd.ID_DoiTuong, 
    								 case when (hd.LoaiHoaDon=22) then sum(hd.TongTienHang)
    								 else 0 end as TongThuTheGiaTri
    							 from BH_HoaDon hd 
    							 where  FORMAT(hd.NgayLapHoaDon,'yyyy-MM-dd') >= @DateFrom and  FORMAT(hd.NgayLapHoaDon,'yyyy-MM-dd') <= @Dateto
    							 and hd.ChoThanhToan='0' --and hd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    							 group by hd.ID_DoiTuong, hd.LoaiHoaDon
    						 ) tblThuThe2 group by tblThuThe2.ID_DoiTuong
    
    				union all
    					 -- su dung the giatri tại thời điểm hiện tại
    						 select tblSuDungThe2.ID_DoiTuong, 
    							 null as TongThuTheGiaTri,
    							 null as SuDungThe,
    							 null as HoanTraTheGiatri,						 
    							 null as PhatSinh_ThuTuThe,
    							 sum(ISNULL(SuDungThe,0)) as PhatSinh_SuDungThe,
    							 null as PhatSinh_HoanTraTheGiatri,
    								 null as PhatSinhTang_DieuChinhThe,
    								 null as PhatSinhGiam_DieuChinhThe
    			
    						 from (
    							 SELECT qct.ID_DoiTuong,
    								case when qhd.LoaiHoaDon= 12 then 0 else sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0)) end as SuDungThe
    							 from Quy_HoaDon_ChiTiet qct
    							 left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    							 left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    							 where  FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') >= @DateFrom and  FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') <= @Dateto
    							 --and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    							 and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    							 and hd.ChoThanhToan ='0'
    							 group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
    						 ) tblSuDungThe2 group by tblSuDungThe2.ID_DoiTuong
    
    					 union all
    					 -- phat sinh tang do điều chỉnh
    						select ID_DoiTuong, 
    					 		 null as TongThuTheGiaTri,
    							 null as SuDungThe,
    							 null as HoanTraTheGiatri,
    							 null as PhatSinh_ThuTuThe,
    							 null as PhatSinh_SuDungThe,
    							 null as PhatSinh_HoanTraTheGiatri,
    							 SUM(ISNULL(TongThuTheGiaTri,0)) as PhatSinhTang_DieuChinhThe,
    								 null as PhatSinhTang_DieuChinhThe
    
    						 from (
    							 SELECT hd.ID_DoiTuong, 
    								 case when (hd.LoaiHoaDon=23) then sum(hd.TongTienHang)
    								 else 0 end as TongThuTheGiaTri
    							 from BH_HoaDon hd 
    							 where  FORMAT(hd.NgayLapHoaDon,'yyyy-MM-dd') >= @DateFrom and  FORMAT(hd.NgayLapHoaDon,'yyyy-MM-dd') <= @Dateto
    							 and hd.ChoThanhToan='0' --and hd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    								 and ISNULL(hd.TongTienHang,0) > 0
    							 group by hd.ID_DoiTuong, hd.LoaiHoaDon
    						 ) tblThuThe2 group by tblThuThe2.ID_DoiTuong
    
    					 union all
    					 -- phat sinh giam do điều chỉnh
    						select ID_DoiTuong, 
    					 		 null as TongThuTheGiaTri,
    							 null as SuDungThe,
    							 null as HoanTraTheGiatri,
    							 null as PhatSinh_ThuTuThe,
    							 null as PhatSinh_SuDungThe,
    							 null as PhatSinh_HoanTraTheGiatri,
    								 null as PhatSinhTang_DieuChinhThe,
    							 SUM(ISNULL(TongThuTheGiaTri,0)* -1) as PhatSinhGiam_DieuChinhThe
    
    						 from (
    							 SELECT hd.ID_DoiTuong, 
    								 case when (hd.LoaiHoaDon=23) then sum(hd.TongTienHang)
    								 else 0 end as TongThuTheGiaTri
    							 from BH_HoaDon hd 
    							 where  FORMAT(hd.NgayLapHoaDon,'yyyy-MM-dd') >= @DateFrom and  FORMAT(hd.NgayLapHoaDon,'yyyy-MM-dd') <= @Dateto
    							 and hd.ChoThanhToan='0' --and hd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    								 and ISNULL(hd.TongTienHang,0) < 0
    							 group by hd.ID_DoiTuong, hd.LoaiHoaDon
    						 ) tblThuThe2 group by tblThuThe2.ID_DoiTuong
    
    				union all
    					  -- hoan tra tien the giatri tại thời điểm hiện tại
    						select ID_DoiTuong, 
    							null as TongThuTheGiaTri,
    							null as SuDungThe,
    							null as HoanTraTheGiatri,						
    							null as PhatSinh_ThuTuThe,
    							null as PhatSinh_SuDungThe,
    							SUM(ISNULL(HoanTraTheGiatri,0)) as PhatSinh_HoanTraTheGiatri,
    								null as PhatSinhTang_DieuChinhThe,
    								null as PhatSinhGiam_DieuChinhThe
    						from (
    								SELECT qct.ID_DoiTuong,
    								case when qhd.LoaiHoaDon= 11 then 0 else sum(iif(qct.HinhThucThanhToan=4,qct.TienThu,0)) end as HoanTraTheGiatri
    								from Quy_HoaDon_ChiTiet qct
    								left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    								left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    								where  FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') >= @DateFrom and  FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') <= @Dateto
    								--and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    								and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    								and hd.ChoThanhToan ='0'
    								group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
    							) tblSuDungThe2 group by tblSuDungThe2.ID_DoiTuong 
    
    					) tblDoiTuong_The group by tblDoiTuong_The.ID_DoiTuong
    					
    			) tblTemp on dt.ID= tblTemp.ID_DoiTuong
    			where (dt.TheoDoi is null or dt.TheoDoi = 0) and dt.LoaiDoiTuong =1
    				and
    					 
    							((select count(Name) from @tblSearchString b where    
								dt.DienThoai like '%'+b.Name+'%'
    							or dt.MaDoiTuong like '%'+b.Name+'%'
    							or dt.TenDoiTuong like '%'+b.Name+'%'
    							or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    							or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'				
    							)=@count or @count=0)	
    
    		) tblView 
    		where tblView.TrangThai like @Status
    		and ISNULL(tblView.SoDuDauKy,0) + ISNULL(tblView.PhatSinhTang,0)- ISNULL(tblView.PhatSinhGiam,0) > 0
    	),
    	count_cte
    	as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(SoDuDauKy) as TongSoDuDauKy,
    				sum(PhatSinhTang) as TongPhatSinhTang,
    				sum(PhatSinhGiam) as TongPhatSinhGiam,
    				sum(SoDuCuoiKy) as TongSoDuCuoiKy
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaDoiTuong
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_GetInforHoaDon_ByID]
    @ID_HoaDon [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;

declare @IDHDGoc uniqueidentifier, @ID_DoiTuong uniqueidentifier, @ID_BaoHiem uniqueidentifier
select @IDHDGoc = ID_HoaDon,  @ID_DoiTuong = ID_DoiTuong, @ID_BaoHiem = ID_BaoHiem from BH_HoaDon where ID = @ID_HoaDon

    select 
    		hd.ID,
			hd.LoaiHoaDon,
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,
			hd.ID_PhieuTiepNhan, 
    		hd.TongTienHang,
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
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,
								
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
    	where hd.ID like @ID_HoaDon
END");

			Sql(@"ALTER PROCEDURE [dbo].[TinhGiamTruLuong]
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

	SET NOCOUNT ON;

		
    		--declare @tblCongThuCong CongThuCong
    		--insert into @tblCongThuCong
    		--exec dbo.GetChiTietCongThuCong 'd93b17ea-89b9-4ecf-b242-d03b8cde71de','','2021-06-01','2021-06-30'
    
    		--declare @tblThietLapLuong ThietLapLuong
    		--insert into @tblThietLapLuong
    		--exec GetNS_ThietLapLuong 'd93b17ea-89b9-4ecf-b242-d03b8cde71de','','2021-06-01','2021-06-30'

		 -- ==== GIAM TRU CODINH ====
		declare @tblGiamTruCD table (ID_NhanVien uniqueidentifier, IDPhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, GiamTruTheoLuong float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblGiamTruCD
		select *			
		from @tblThietLapLuong pc
		where pc.LoaiLuong = 62	

		

		declare @tblCong1 table (ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, GiamTruTheoLuong float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoLanDiMuon float, NgayApDung datetime, NgayKetThuc datetime)
		declare @cd_ID_NhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_LoaiLuong int, @cd_GiamTruTheoLuong float, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime


		insert into @tblCong1		
		select gtVND.ID_NhanVien, null as ID_ChamCongChiTiet, gtVND.IDPhuCap, gtVND.TenPhuCap,
			gtVND.LoaiLuong, gtVND.GiamTruTheoLuong, gtVND.HeSo, 
			null as ID_CaLamViec, 
			'' as TenCa,
			0 as SoLanDiMuon,
			gtVND.NgayApDung, gtVND.NgayKetThuc
		from @tblGiamTruCD gtVND
		

		 -- ==== GIAM TRU THEO SO LAN ====

		-- get giamtru theosolan trong khoang thoigian
		declare @tblGiamTruLan table (IDPhuCap uniqueidentifier, ID_NhanVien uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, GiamTruTheoLan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblGiamTruLan
		select *			
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong = 61		

		-- bảng tính số lần đi muộn, về sớm theo phiếu phân ca, ca làm việc
		declare @tblCong2 table ( ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, GiamTruTheoLan float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), SoLanDiMuon float, NgayApDung datetime, NgayKetThuc datetime)

		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @IDPhuCap uniqueidentifier, @TenPhuCap nvarchar(max), @LoaiLuong int, @GiamTruTheoLan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblGiamTruLan
		OPEN curPhuCap -- cur 1
    	FETCH FIRST FROM curPhuCap
    	INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong2
				select @ID_NhanVien, tmp.ID_ChamCongChiTiet, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(SoPhutDiMuon >0,1,0))  as SoLanDiMuon,
					@NgayApDung, @NgayKetThuc
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 
				
				FETCH NEXT FROM curPhuCap INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 		


			
				select pc.ID_NhanVien, 
					SUM(pc.GiamTruTheoLuong) as GiamTruTheoLuong,
					SUM(pc.GiamTruTheoLan) as GiamTruTheoLan,
					SUM(SoLanDiMuon) as SoLanDiMuon				
				from 
					(select ID_NhanVien, GiamTruTheoLuong * HeSo as GiamTruTheoLuong, 0 as SoLanDiMuon, 0 as GiamTruTheoLan
					from @tblCong1 							
					group by ID_NhanVien, ID_PhuCap, GiamTruTheoLuong, HeSo

					union all

					select ID_NhanVien, 0 as GiamTruTheoLuong,SoLanDiMuon, GiamTruTheoLan * HeSo * SoLanDiMuon as GiamTruTheoLan 
					from @tblCong2
					) pc group by pc.ID_NhanVien 

		
END");

			Sql(@"ALTER PROCEDURE [dbo].[TinhLuongCoBan]
	@NgayCongChuan int,
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN
	
	SET NOCOUNT ON;		

		
		--declare @IDChiNhanhs uniqueidentifier='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
		--declare @FromDate datetime='2020-08-01'
		--declare @ToDate datetime='2020-08-31'
		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs,'', @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs,'', @FromDate, @ToDate
		--declare @NgaycongchuanSetup float = (select top 1 NgayCongChuan from HT_CongTy)
		

		-- get thietlapluong (codinh + ngay)
		declare @tblLuongCDNgay table (ID_NhanVien uniqueidentifier,ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblLuongCDNgay
		select *		
		from @tblThietLapLuong pc 
		where  pc.LoaiLuong in (1,2)

		--select * from @tblLuongCDNgay

		declare @tblCongCDNgay table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), SoNgayDiLam float)
		
		declare @cdID_NhanVien uniqueidentifier, @cdID uniqueidentifier, @cdTenLoaiLuong nvarchar(max), @cdLoaiLuong int,@cdLuongCoBan float, @cdHeSo int, @cdNgayApDung datetime, @cdNgayKetThuc datetime
		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblLuongCDNgay
		OPEN curLuong -- cur 1
    	FETCH FIRST FROM curLuong
    	INTO @cdID_NhanVien, @cdID, @cdTenLoaiLuong, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, @cdNgayApDung, @cdNgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongCDNgay
				select @cdID_NhanVien, @cdID, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam				
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @cdID_NhanVien and tmp.NgayCham >= @cdNgayApDung and (@cdNgayKetThuc is null OR tmp.NgayCham <= @cdNgayKetThuc )  
				group by tmp.ID_NhanVien, tmp.ID_CaLamViec, tmp.TenCa		
				FETCH NEXT FROM curLuong INTO @cdID_NhanVien, @cdID, @cdTenLoaiLuong, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, @cdNgayApDung, @cdNgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 

		-- get caidatluong (ca + gio)
		declare @tblLuongCaGio table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblLuongCaGio
		select *
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong in (3,4)


		-- bảng tính công theo phiếu phân ca, ca làm việc
		declare @tblCong table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
		SoNgayDiLam float, Luong float)				
	
		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int, @LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblLuongCaGio
		OPEN curLuong 
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				select ct.*
				into #tmpca
				from NS_ThietLapLuongChiTiet ct
				where ID_LuongPhuCap= @ID_PhuCap and LaOT= 0
				
				insert into @tblCong
				select 
					 @ID_NhanVien, @ID_PhuCap, @LoaiLuong, @LuongCoBan,@HeSo,@NgayApDung, @NgayKetThuc,
					 tbl.ID_CaLamViec, tbl.TenCa, tbl.TongGioCong1Ca,
					 tbl.TongCong,
					case when @LoaiLuong = 3 then LuongCoBanQuyDoi * tbl.Cong else (tbl.Cong * TongGioCong1Ca - SoPhutDiMuon * 1.0/60 +  SoGioOT) * LuongCoBanQuyDoi  end as ThanhTien						
				from
					(
						select 
							luong.ID_CaLamViec, luong.TenCa, luong.TongGioCong1Ca,
							luong.Cong, luong.SoPhutDiMuon, luong.SoGioOT, 
							luong.LuongNgayThuong, luong.LoaiNgayThuong_Nghi,
							luong.GiaTri as HeSoLuong,luong.LaPhanTram,
							case when @LoaiLuong= 3 then luong.Cong else luong.Cong * luong.TongGioCong1Ca - luong.SoPhutDiMuon * 1.0/60 +  luong.SoGioOT end as TongCong,	
							case when luong.LaPhanTram= 1 then @HeSo * luong.LuongNgayThuong else @HeSo * luong.GiaTri end as LuongCoBan,
							case when luong.LaPhanTram= 1 then @HeSo * luong.LuongNgayThuong * luong.GiaTri/100 else @HeSo * luong.GiaTri end as LuongCoBanQuyDoi
					
						from 
							(
							select 
								qd.ID_CaLamViec, qd.TenCa, qd.TongGioCong1Ca,
								qd.Cong, qd.SoPhutDiMuon, qd.SoGioOT,
								isnull(qd.LuongNgayThuong, LuongNgayThuongNull) as LuongNgayThuong,

								case when ngayle.ID is null then
									case qd.Thu
										when 6 then 6
										when 0 then 0
									else 3 end
								else qd.LoaiNgay end LoaiNgayThuong_Nghi, -- 0.chunhat, 6.thu7, 1.ngaynghi, 2.ngayle, 3.ngaythuong  
								
								case when ngayle.ID is null then
									case qd.Thu
										when 6 then case when Thu7_GiaTri is not null then qd.Thu7_GiaTri else Thu7_GiaTriNull end
										when 0 then case when ThCN_GiaTri is not null then qd.ThCN_GiaTri else ThCN_GiaTriNull end
									else case when qd.LuongNgayThuong is not null then LuongNgayThuong else LuongNgayThuongNull end
									end
								else 
									case qd.LoaiNgay
										when 1 then case when qd.NgayNghi_GiaTri is not null then NgayNghi_GiaTri else NgayNghi_GiaTriNull end
										when 2 then case when qd.NgayLe_GiaTri is not null then NgayLe_GiaTri else NgayLe_GiaTriNull end
									else case when qd.LuongNgayThuong is not null then LuongNgayThuong else LuongNgayThuongNull end
								end end as GiaTri,

								case when ngayle.ID is null then
									case qd.Thu
										when 6 then case when Thu7_LaPhanTramLuong is not null then qd.Thu7_LaPhanTramLuong else Thu7_LaPhanTramLuongNull end
										when 0 then case when CN_LaPhanTramLuong is not null then qd.CN_LaPhanTramLuong else CN_LaPhanTramLuongNull end
									else 0
									end
								else 
									case qd.LoaiNgay
										when 1 then case when qd.NgayNghi_LaPhanTramLuong is not null then NgayNghi_LaPhanTramLuong else NgayNghi_LaPhanTramLuongNull end
										when 2 then case when qd.NgayLe_LaPhanTramLuong is not null then NgayLe_LaPhanTramLuong else NgayLe_LaPhanTramLuongNull end
									else 0
								end end as LaPhanTram																								
							from
							(
								select tmp.*, 
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) LuongNgayThuong,
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) LuongNgayThuongNull,

									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_GiaTri,
									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_GiaTriNull,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_LaPhanTramLuong,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_LaPhanTramLuongNull,
						
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) CN_LaPhanTramLuong,
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) CN_LaPhanTramLuongNull,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) ThCN_GiaTri,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) ThCN_GiaTriNull,

									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_GiaTri,
									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_GiaTriNull,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_LaPhanTramLuong,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_LaPhanTramLuongNull,

									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_GiaTri,
									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_GiaTriNull,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_LaPhanTramLuong,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_LaPhanTramLuongNull
						
								from @tblCongThuCong tmp
								where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
							) qd
							left join NS_NgayNghiLe ngayle on qd.NgayCham= ngayle.Ngay and ngayle.TrangThai='1'
						) luong
					)tbl

					drop table #tmpca

				FETCH NEXT FROM curLuong 
				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	

		select max(LoaiLuong) as LoaiLuong, 
			ID_NhanVien, 
			max(LuongCoBan) as LuongCoBan,
			sum(SoNgayDiLam) as SoNgayDiLam,
			sum(Luong) as Luong
		from
		(
			select luongcagio.ID_NhanVien, luongcagio.LoaiLuong,
				max(LuongCoBan) as LuongCoBan,
				sum(SoNgayDiLam) as SoNgayDiLam,
				sum(luongcagio.Luong) as Luong
			from @tblCong luongcagio
			group by ID_NhanVien, LoaiLuong

			union all

			select luongcd.ID_NhanVien, luongcd.LoaiLuong,
				max(LuongCoBan) as LuongCoBan,
				sum(SoNgayDiLam) as SoNgayDiLam,
				case when LoaiLuong = 1 then LuongCoBan
				else LuongCoBan/@NgayCongChuan * sum(SoNgayDiLam) 			
				--else case when sum(SoNgayDiLam) > @NgayCongChuan then LuongCoBan else LuongCoBan/@NgayCongChuan * sum(SoNgayDiLam) end
				end as Luong
			from @tblCongCDNgay luongcd		
			group by ID_NhanVien, LoaiLuong,LuongCoBan
		) luong
		group by luong.ID_NhanVien
			
END");

			Sql(@"ALTER PROCEDURE [dbo].[TinhPhuCapLuong]
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN
	SET NOCOUNT ON;

		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs, @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs, @FromDate, @ToDate

	-- get phucapcodinh vnd trong khoang thoigian
		declare @tblPhuCapCD table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, PhuCapCoDinh float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblPhuCapCD
		select *
		from @tblThietLapLuong pc
		where pc.LoaiLuong = 52 		

		declare @tblCong1 table ( ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, PhuCapCoDinh float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoNgayDiLam float, NgayApDung datetime, NgayKetThuc datetime)
		declare @cd_IDNhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_LoaiLuong int, @cd_PhuCapCoDinh float, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime

		insert into @tblCong1		
		select pcVND.ID_NhanVien, null as ID_ChamCongChiTiet, pcVND.ID_PhuCap, pcVND.TenPhuCap,
			pcVND.LoaiLuong, pcVND.PhuCapCoDinh, pcVND.HeSo, 
			null as ID_CaLamViec, 
			'' as TenCa,
			0 as SoNgayDiLam,
			pcVND.NgayApDung, pcVND.NgayKetThuc
		from @tblPhuCapCD pcVND


		-- get phucaptheongay trong khoang thoigian
		declare @tblPhuCap table (ID_NhanVien uniqueidentifier, IDPhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, PhuCapTheoNgay float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblPhuCap
		select *	
		from @tblThietLapLuong pc 
		where pc.LoaiLuong = 51	
		
			-- bảng tính số ngày đi làm theo phiếu phân ca, ca làm việc
		declare @tblCong2 table (ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, 
		PhuCapTheoNgay float, HeSo int, ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoNgayDiLam float,		
		NgayApDung datetime, NgayKetThuc datetime)

		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @IDPhuCap uniqueidentifier, @TenPhuCap nvarchar(max), @PhuCapTheoNgay float, @LoaiLuong int, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblPhuCap
		OPEN curPhuCap 
    	FETCH FIRST FROM curPhuCap
    	INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong,@PhuCapTheoNgay, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong2
				select @ID_NhanVien, tmp.ID_ChamCongChiTiet, @IDPhuCap, @TenPhuCap, @LoaiLuong, @PhuCapTheoNgay, @HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam,
					@NgayApDung, @NgayKetThuc
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong,@PhuCapTheoNgay, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	
			
			select 
				ID_NhanVien,
				SUM(pc.PhuCapCoDinh) as PhuCapCoDinh,
				SUM(pc.ThanhTienPC_TheoNgay) as ThanhTienPC_TheoNgay
			from
			(select ID_NhanVien,
					PhuCapCoDinh * HeSo as PhuCapCoDinh, 
					0 as PhuCapTheoNgay, 
					0 as ThanhTienPC_TheoNgay,
					0 as SoNgayDiLam,
					ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc, 
					1 as LoaiPhuCap
				from @tblCong1 			
				group by ID_NhanVien, ID_PhuCap, PhuCapCoDinh, HeSo, NgayApDung, NgayKetThuc, TenPhuCap

				union all

				select ID_NhanVien, 
					0 as PhuCapCoDinh,
					PhuCapTheoNgay  * HeSo as PhuCapTheoNgay ,
					PhuCapTheoNgay * HeSo * SoNgayDiLam as ThanhTienPC_TheoNgay,
					SoNgayDiLam,
					ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc,
					2 as LoaiPhuCap
				from @tblCong2
				) pc group by ID_NhanVien	
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateKhachHang_DuDKNangNhom]
    @ID_NhomDoiTuong [uniqueidentifier],
    @IDChiNhanhs [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @fromDate varchar(20) ='2016-01-01', 
    	@dtNow varchar(20) = format(dateadd(day,1,getdate()),'yyyy-MM-dd'),
    	@countDK int=0,
    	@sql1 nvarchar(max),
    	@sql2 nvarchar(max), 
    	@sql3 nvarchar(max), 
    	@sql4 nvarchar(max), 
    	@sql5 nvarchar(max), 
    	@checkThoiGianMua int = 0,
    	@whereThoiGianMua nvarchar(max)='',
    	@where nvarchar(max)=''
    
    	select @countDK = count(ID) from DM_NhomDoiTuong_ChiTiet where ID_NhomDoiTuong like @ID_NhomDoiTuong
    	-- chi insert neu nhom co dieu kien nang nhom
    	if @countDK > 0
    	begin
    
    		-- check thoigian mua hang
    			declare @LoaiDieuKien1 int ,@LoaiSoSanh1 int, @GiaTriThoiGian1 datetime
    			DECLARE _cur1 CURSOR FOR 
    			select 
    				ct.LoaiDieuKien, ct.LoaiSoSanh, ct.GiaTriThoiGian
    			from DM_NhomDoiTuong_ChiTiet ct
    			where ID_NhomDoiTuong like @ID_NhomDoiTuong and ct.LoaiDieuKien  = 3
    
    			OPEN _cur1  
    			FETCH NEXT FROM _cur1
    			INTO  @LoaiDieuKien1, @LoaiSoSanh1,  @GiaTriThoiGian1 
    
    			WHILE @@FETCH_STATUS = 0  
    			BEGIN  
    
    				if @whereThoiGianMua !='' and @whereThoiGianMua is not null 
    					set @whereThoiGianMua= CONCAT( @whereThoiGianMua, ' AND ')		
    
    				 set @whereThoiGianMua = concat(@whereThoiGianMua, 
    				   ' NgayLapHoaDon ',
    					case  @LoaiSoSanh1
    							when 1 then CONCAT( ' > ''', FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd'), '''')
    							when 2 then CONCAT( ' >= ''', FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd'), '''')
    							when 3 then CONCAT( ' >= ''',  FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd')
    										, ''' AND NgayLapHoaDon < ''', FORMAT(DATEADD(day, 1, @GiaTriThoiGian1),'yyyy-MM-dd'), '''')
    							when 4 then CONCAT( ' < ''',  FORMAT(DATEADD(day, 1, @GiaTriThoiGian1),'yyyy-MM-dd'), '''')
    							when 5 then CONCAT( ' < ''', FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd'), '''')
    							else   CONCAT( ' NgayLapHoaDon > ''',  FORMAT(DATEADD(day, -1, @GiaTriThoiGian1),'yyyy-MM-dd'), ''' AND NgayLapHoaDon < ''' , 
    							FORMAT(DATEADD(day, 1, @GiaTriThoiGian1),'yyyy-MM-dd'), '''')
    					end 
    					)
    				  FETCH NEXT FROM _cur1 
    				  INTO @LoaiDieuKien1, @LoaiSoSanh1,  @GiaTriThoiGian1 
    			END 
    
    			CLOSE _cur1  
    			DEALLOCATE _cur1 
    		
    			-- get list chinhanh
    			set @sql1 = concat( 'declare @tblChiNhanh table(ID_Donvi uniqueidentifier)
    				insert into @tblChiNhanh
    				select Name  from dbo.splitstring(''', @IDChiNhanhs , ''') ')
    
    			-- if: khong co dieukien ve thoigian mua
    			if	@whereThoiGianMua!='' 
    			begin
    				set	@checkThoiGianMua = 1
    				set @whereThoiGianMua =concat(' and ', @whereThoiGianMua)
    			end
    			
    			-- check cac dieukien khac
    			declare @LoaiDieuKien int ,@LoaiSoSanh int, @GiaTriSo int ,@GiaTriBool bit, @GiaTriThoiGian datetime ,@GiaTriKhuVuc uniqueidentifier, @GiaTriVungMien uniqueidentifier
    
    			DECLARE _cur CURSOR FOR 
    			select  ct.LoaiDieuKien, ct.LoaiSoSanh, ct.GiaTriSo, ct.GiaTriBool, ct.GiaTriThoiGian, ct.GiaTriKhuVuc, ct.GiaTriVungMien	
    		   from DM_NhomDoiTuong_ChiTiet ct
    		   where ID_NhomDoiTuong like @ID_NhomDoiTuong and ct.LoaiDieuKien  != 3
    	
    			OPEN _cur  
    			FETCH NEXT FROM _cur
    			INTO  @LoaiDieuKien, @LoaiSoSanh, @GiaTriSo, @GiaTriBool, @GiaTriThoiGian ,@GiaTriKhuVuc , @GiaTriVungMien 
    
    			WHILE @@FETCH_STATUS = 0  
    			BEGIN  
    
    				if @where !='' and @where is not null set @where= CONCAT( @where, ' AND ')
    				 set @where = concat(@where, 
    				 case  @LoaiDieuKien
    						when 1 then ' TongBanTruTraHang '
    						when 2 then ' TongBan '
    						when 4 then ' SoLanMuaHang '
    						when 5 then ' NoHienTai '
    						when 6 then ' ThangSinh != -1 and ThangSinh  '
    						when 7 then ' NgaySinh_NgayTLap is not null  and DinhDang_NgaySinh != ''dd/MM'' and NgaySinh_NgayTLap '
    						when 8 then ' GioiTinhNam '
    						when 9 then ' ID_TinhThanh '
    					end,
    					case  @LoaiSoSanh
    									when 1 then ' > '
    									when 2 then ' >= '
    									when 3 then ' = '
    									when 4 then ' <= '
    									when 5 then ' < '
    									else ' != ' 
    					end ,
    					case  @LoaiDieuKien			
    						when 1 then  concat('', @GiaTriSo)
    						when 2 then  concat('', @GiaTriSo)
    						when 4 then  concat('', @GiaTriSo)
    						when 5 then  concat('', @GiaTriSo)
    						when 6 then  concat('', @GiaTriSo)
    						when 7 then  concat(' DATEADD (year, ', -@GiaTriSo ,' , FORMAT( NgaySinh_NgayTLap,''yyyy-MM-dd'')) ')
    						when 8 then  concat('', @GiaTriBool)
							when 9 then  concat('''', @GiaTriKhuVuc,'''')
    						end ,'' )
    				  FETCH NEXT FROM _cur 
    				  INTO @LoaiDieuKien, @LoaiSoSanh, @GiaTriSo, @GiaTriBool, @GiaTriThoiGian ,@GiaTriKhuVuc , @GiaTriVungMien 
    			END 
    
    			CLOSE _cur  
    			DEALLOCATE _cur 
    
    			if	@where!='' set @where =concat(' where ', @where)
    
    			set @sql2 = concat('
    			-- get DS Khachhang du dk nangnhom
    			select tbl.ID, tbl.MaDoiTuong
    			into #temp 
    			from
    			(
    		   SELECT 
    				  dt.ID,
    				  dt.MaDoiTuong,      	   		
    				  dt.GioiTinhNam,
    					  iif(dt.NgaySinh_NgayTLap is null,-1, datepart(month,dt.NgaySinh_NgayTLap)) as ThangSinh,
    				  dt.NgaySinh_NgayTLap,
    				  dt.DinhDang_NgaySinh,
    				  dt.ID_TinhThanh,
    				  dt.ID_QuanHuyen,
    				  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    				  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    				  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    				  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
    					   CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai
    				  FROM DM_DoiTuong dt  
    			
    						LEFT JOIN (
    							SELECT tblThuChi.ID_DoiTuong,
    							SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) - SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    						SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
    						SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    						SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    						SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    						FROM
    						(
    						
    									-- tongban
    						SELECT 
    									bhd.ID_DoiTuong,
    									0 AS GiaTriTra,							
    									ISNULL(bhd.PhaiThanhToan,0) AS DoanhThu,
    									0 AS TienThu,
    									0 AS TienChi,
    									0 AS SoLanMuaHang
    								FROM BH_HoaDon bhd
    								WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = ''0''
    									and bhd.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    									AND bhd.NgayLapHoaDon >= ''',@fromDate , 
    										''' AND bhd.NgayLapHoaDon < ''',@dtNow ,
    										''' AND  exists (select ID_DonVi from @tblChiNhanh  dv where  bhd.ID_DonVi = dv.ID_DonVi) ',
    							
    
    								' union all
    									-- tongtra
    								SELECT bhd.ID_DoiTuong,
    									ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    									0 AS DoanhThu,
    									0 AS TienThu,
    									0 AS TienChi, 
    									0 AS SoLanMuaHang
    								FROM BH_HoaDon bhd   						
    								WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = ''0'' 
    										and	bhd.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    									AND bhd.NgayLapHoaDon >= ''',@fromDate ,''' AND bhd.NgayLapHoaDon < ''', @dtNow ,
    										''' AND exists (select ID_DonVi from @tblChiNhanh  dv where  bhd.ID_DonVi = dv.ID_DonVi) ')
    							
    		   set @sql3= concat(' union all
    
    									-- tienthu
    									SELECT 
    									qhdct.ID_DoiTuong,						
    									0 AS GiaTriTra,
    									0 AS DoanhThu,
    									ISNULL(qhdct.TienThu,0) AS TienThu,
    									0 AS TienChi,
    										0 AS SoLanMuaHang
    								FROM Quy_HoaDon qhd
    								JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    									Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    								WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
    									and	qhdct.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    									and (qhdct.HinhThucThanhToan is null or qhdct.HinhThucThanhToan !=6)
    								AND exists (select ID_DonVi from @tblChiNhanh  dv where  qhd.ID_DonVi = dv.ID_DonVi)  
    								AND qhd.NgayLapHoaDon >= ''', @fromDate , ''' AND qhd.NgayLapHoaDon < ''', @dtNow, 
    								
    									'''	union all
    
    									-- tienchi
    								SELECT 
    									qhdct.ID_DoiTuong,						
    									0 AS GiaTriTra,
    									0 AS DoanhThu,
    									0 AS TienThu,
    									ISNULL(qhdct.TienThu,0) AS TienChi,
    										0 AS SoLanMuaHang
    								FROM Quy_HoaDon qhd
    								JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    								WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
    										and	qhdct.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    										and (qhdct.HinhThucThanhToan is null or qhdct.HinhThucThanhToan !=6)
    									AND qhd.NgayLapHoaDon >= ''', @fromDate, 
    										''' AND qhd.NgayLapHoaDon < ''', @dtNow, ''' 
    										AND exists (select ID_DonVi from @tblChiNhanh  dv where  qhd.ID_DonVi = dv.ID_DonVi) ')
    
    		 set @sql4= 	concat(	' Union All
    									-- solan mua hang
    								Select 
    									hd.ID_DoiTuong,
    									0 AS GiaTriTra,
    									0 AS DoanhThu,
    									0 AS TienThu,
    										0 as TienChi,
    									COUNT(*) AS SoLanMuaHang								
    								From BH_HoaDon hd 
    								where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 25 or hd.LoaiHoaDon = 19)
    								and hd.ChoThanhToan = 0
    									and	hd.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    								AND hd.NgayLapHoaDon >= ''', @fromDate ,''' AND hd.NgayLapHoaDon < ''', @dtNow,
    									''' AND exists (select ID_DonVi from @tblChiNhanh  dv where  hd.ID_DonVi = dv.ID_DonVi) 
    									 GROUP BY hd.ID_DoiTuong  	
    									)AS tblThuChi
    								GROUP BY tblThuChi.ID_DoiTuong
    						) a on dt.ID = a.ID_DoiTuong  					
    								WHERE dt.TheoDoi= 0 and  dt.loaidoituong = 1 and dt.ID not like ''%00000000-0000-0000-0000-0000%''
    									AND ( ', @checkThoiGianMua,  ' = 0    OR 
    										exists ( select ID_DoiTuong from BH_HoaDon hd
    											where hd.LoaiHoaDon in (1,19,22,25)
    											and hd.ID_DoiTuong = dt.ID
    											and hd.ID_DoiTuong not like ''%00000000%''', @whereThoiGianMua, '
    											 ) )
    		 ) tbl ', @where)
    
    		 -- 1. get DS khachhang khongdu dieukien nangnhom & delete
    		 -- 2. insert again khachhang du dieukien
    		 set @sql5 = concat(' delete  dtn from DM_DoiTuong_Nhom dtn where not exists (select ID from #temp tmp where dtn.ID_DoiTuong = tmp.ID)',
    		 ' and dtn.ID_NhomDoiTuong=''', @ID_NhomDoiTuong, '''',
    
    		 '	insert into DM_DoiTuong_Nhom
    			select NEWID(),  tmp.ID, ''', @ID_NhomDoiTuong,'''
    			from #temp tmp where
    		    not exists (select ID from DM_DoiTuong_Nhom dtn where tmp.ID = dtn.ID_DoiTuong and dtn.ID_NhomDoiTuong = ''', @ID_NhomDoiTuong,''')')
    
    	exec ( @sql1 + @sql2 + @sql3+ @sql4 + @sql5)
    	----print @sql4
    	end
END");

			Sql(@"update BH_HoaDon set TongThueKhachHang = TongTienThue - isnull(TongTienThueBaoHiem,0)");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BCBanHang_GetGiaVonHDSC]");
			DropStoredProcedure("[dbo].[GetTongThu_fromHDDatHang]");
        }
    }
}
