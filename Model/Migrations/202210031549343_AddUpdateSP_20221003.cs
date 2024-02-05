namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20221003 : DbMigration
    {
        public override void Up()
        {
			CreateStoredProcedure(name: "[dbo].[BCThuChi_TheoLoaiTien]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(defaultValue: ""),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				LoaiDoiTuongs = p.String(defaultValue: "1"),
				KhoanMucThuChis = p.String(defaultValue: "")
			}, body: @"SET NOCOUNT ON;

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
		qct.TienThu as TongThu,
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
	cross join count_cte");

			CreateStoredProcedure(name: "[dbo].[GetAll_ChiTietPhieuTrich]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(defaultValue: ""),
				TextSearch = p.String(defaultValue: ""),
				TextSearch2 = p.String(defaultValue: ""),
				DateFrom = p.DateTime(defaultValue: null),
				DateTo = p.DateTime(defaultValue: null),
				LoaiDoiTuongs = p.String(defaultValue: ""),
				TrangThais = p.String(defaultValue: ""),
				LaHoaDonBoSung = p.Int(defaultValue: 2),
				CurrentPage = p.Int(defaultValue: 0),
				PageSize = p.Int(defaultValue: 10)
			}, body: @"SET NOCOUNT ON;

	declare @tblChiNhanh table (ID uniqueidentifier)
	if(isnull(@IDChiNhanhs,'')!='')
		begin
			insert into @tblChiNhanh
			select name from dbo.splitstring(@IDChiNhanhs)
		end

	----- timkiem theo thongtin phieutrich: maphieu, nguoi gioithieu
	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

		----- timkiem theo hoadon duoc trichhoahong: mahoadon, khachhang
	DECLARE @tblSearch2 TABLE (Name [nvarchar](max));
    DECLARE @count2 int;
    INSERT INTO @tblSearch2(Name) select  Name from [dbo].[splitstringByChar](@TextSearch2, ' ') where Name!='';
    Select @count2 =  (Select count(*) from @tblSearch2);

	declare @tblLoaiDoiTuong table (LoaiDT tinyint)
	if(isnull(@LoaiDoiTuongs,'')!='')
		begin
			insert into @tblLoaiDoiTuong
			select name from dbo.splitstring(@LoaiDoiTuongs)
		end

	declare @tblTrangThai table (TrangThai tinyint)
	if(isnull(@TrangThais,'')!='')
		begin
			insert into @tblTrangThai
			select name from dbo.splitstring(@TrangThais)
		end

	if(isnull(@DateFrom,'')='') set @DateFrom ='2016-01-01'
	if(isnull(@DateTo,'')='') set @DateTo = DATEADD(day,1,getdate())


	;with data_cte
	as(
	select *
	from
	(
	select 
		pt.ID,
		pt.ID_CheckIn,
		pt.MaHoaDon as MaPhieuTrich,
		pt.NgayLapHoaDon as NgayLapPhieuTrich,
		case pt.TongChietKhau
			when 5 then nv.MaNhanVien
		else dt.MaDoiTuong end as MaNguoiGioiThieu,
		case pt.TongChietKhau
			when 5 then nv.TenNhanVien
		else dt.TenDoiTuong end as TenNguoiGioiThieu,
		case pt.TongChietKhau
			when 5 then nv.DienThoaiDiDong
		else dt.DienThoai end as SDTNguoiGioiThieu,
		case pt.TongChietKhau
			when 5 then nv.TenNhanVienKhongDau
		else dt.TenDoiTuong_KhongDau end as TenNguoiGioiThieu_KhongDau,
		case pt.TongChietKhau		
			when 1 then N'Khách hàng'
			when 2 then N'Nhà cung cấp'
			when 3 then N'Bảo hiểm'
			when 4 then N'Khác'
			when 5 then N'Nhân viên'
		end as SLoaiDoiTuong,
		case pt.ChoThanhToan	
			when '1' then 1
			when '0' then 0 
			else 2 
		end as TrangThaiPT,

		hd.ID_DoiTuong,
		hd.LoaiHoaDon,
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hd.TongThanhToan,
		kh.MaDoiTuong,
		kh.TenDoiTuong,
		
		ct.PTChietKhau,
		ct.TienChietKhau,
		cast(ct.PTThue as int) as TrangThai,
		iif(ct.PTChietKhau= 0,0,ct.TienChietKhau * 100/ ct.PTChietKhau) as DaTrich

	from BH_HoaDon pt ----- phieutrich 
	join BH_HoaDon_ChiTiet ct on pt.ID= ct.ID_HoaDon 
    join BH_HoaDon hd on ct.ID_ParentCombo = hd.ID ---- hd dc trich 
	join DM_DoiTuong kh on hd.ID_DoiTuong= kh.ID
    left join DM_DoiTuong dt on pt.ID_CheckIn= dt.ID
    left join NS_NhanVien nv on pt.ID_CheckIn= nv.ID --and hd.TongChietKhau= 5
    where pt.LoaiHoaDon= 41
	and pt.NgayLapHoaDon between @DateFrom and @DateTo
	and (@IDChiNhanhs='' or exists (select ID from @tblChiNhanh cn where pt.ID_DonVi= cn.ID))	
	and (@LoaiDoiTuongs='' or exists (select loai.LoaiDT from @tblLoaiDoiTuong loai where pt.TongChietKhau= loai.LoaiDT))	
	and (@LaHoaDonBoSung= 2 or ct.PTThue = @LaHoaDonBoSung) 
	and
		((select count(Name) from @tblSearch b where     			
		hd.MaHoaDon like '%'+b.Name+'%'			
		or pt.MaHoaDon like '%'+b.Name+'%'			
		or nv.TenNhanVien like '%'+b.Name+'%'
		or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
		or hd.DienGiai like '%'+b.Name+'%'
		or kh.MaDoiTuong like '%'+b.Name+'%'		
		or kh.TenDoiTuong like '%'+b.Name+'%'
		or kh.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or kh.MaDoiTuong like '%'+b.Name+'%'		
		or kh.TenDoiTuong like '%'+b.Name+'%'
		or kh.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or kh.DienThoai like '%'+b.Name+'%'				
		)=@count or @count=0)	
	) tbl 
	where
		(@TrangThais='' or exists (select tt.TrangThai from @tblTrangThai tt where tbl.TrangThaiPT = tt.TrangThai))
	and	((select count(Name) from @tblSearch2 b where     				
		tbl.MaPhieuTrich like '%'+b.Name+'%'			
		or tbl.MaNguoiGioiThieu like'%'+b.Name+'%'	
		or tbl.TenNguoiGioiThieu like '%'+b.Name+'%'			
		or tbl.SDTNguoiGioiThieu like'%'+b.Name+'%'			
		or tbl.TenNguoiGioiThieu_KhongDau like'%'+b.Name+'%'		
		)=@count2 or @count2=0)	
		),
	count_cte
		as
		(
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(DaTrich) as SumDaTrich,
				sum(TongThanhToan) as SumTongTienHang,
				sum(TienChietKhau) as SumTienChietKhau
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapPhieuTrich desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY; ");

			CreateStoredProcedure(name: "[dbo].[GetBaoCaoCongNoChiTiet]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(defaultValue: ""),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				TextSearch = p.String(defaultValue: ""),
				TrangThais = p.String(defaultValue: "", maxLength: 4),
				CurrentPage = p.Int(defaultValue: 0),
				PageSize = p.Int(defaultValue: 10)
			}, body: @"SET NOCOUNT ON;

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
	select 
		tbl.ID,
		tbl.MaHoaDon,
		tbl.LoaiHoaDon,
		tbl.NgayLapHoaDon,		
		tbl.DienGiai,
		tbl.MaDoiTuong,
		tbl.TenDoiTuong,
		tbl.TenDonVi,
		tbl.TenNhanViens,
		tbl.KhachDaTra,
		tbl.TongThanhToan- tbl.TongTra as TongThanhToan,
		tbl.TongThanhToan - tbl.TongTra - tbl.KhachDaTra as ConNo,
		iif(tbl.LoaiHoaDon!=19, tbl.TongThanhToan - tbl.TongTra - tbl.KhachDaTra, 
		iif(tbl.KhachDaTra - tbl.GiaTriSD > 0,0, tbl.GiaTriSD - tbl.KhachDaTra)) as NoThucTe
	from
	(
	select  
		hd.ID,
		hd.MaHoaDon,
		hd.LoaiHoaDon,
		hd.NgayLapHoaDon,
		hd.TongThanhToan,
		hd.DienGiai,
		dt.MaDoiTuong,
		dt.TenDoiTuong,
		dv.TenDonVi,
		tblNV.TenNhanViens,
		isnull(soquy.KhachDaTra,0) as KhachDaTra,
		isnull(sdGDV.GiaTriSD,0) as GiaTriSD,
		isnull(traGDV.TongTra,0) as TongTra,
		iif(hd.TongThanhToan - isnull(soquy.KhachDaTra,0) > 0,1,0) as TrangThaiCongNo
	from BH_HoaDon hd	
	join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
	join DM_DonVi dv on hd.ID_DonVi = dv.ID
	left join
	(
		Select distinct
    			(
    				Select distinct nv.TenNhanVien + ',' AS [text()]
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
		left join(
		------ tra gdv----
		select 
			hdt.ID_HoaDon,
			sum(hdt.PhaiThanhToan) as TongTra
		from BH_HoaDon hdt
		where hdt.LoaiHoaDon = 6 and hdt.ChoThanhToan ='0'	
		group by hdt.ID_HoaDon
	) traGDV on hd.ID = traGDV.ID_HoaDon
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
	) tbl where (@TrangThais ='' or tbl.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))
					
	),
	count_cte as
	 (
    		select count(ID) as TotalRow,
    			CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    			sum(TongThanhToan) as TongThanhToanAll,
    			sum(KhachDaTra) as KhachDaTraAll,
    			sum(ConNo) as ConNoAll,
				sum(NoThucTe) as NoThucTeAll
    			from data_cte
    )
    select dt.*, cte.*
    from data_cte dt
    cross join count_cte cte
    order by dt.NgayLapHoaDon desc
    OFFSET (@CurrentPage* @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetChiTietHoaHongGioiThieu_byID]", parametersAction: p => new
			{
				ID = p.Guid()
			}, body: @"SET NOCOUNT ON;

	select 
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hd.TongThanhToan,
		hd.ID_CheckIn, --- id nguoi GT
		dt.MaDoiTuong,
		dt.TenDoiTuong,
		dt.DienThoai,
		hd.ID_DoiTuong,
		ct.ID_ParentCombo as ID_HoaDon_DuocCK,	
		ct.PTChietKhau,
		ct.TienChietKhau,	
		iif(ct.PTChietKhau= 0,0,ct.TienChietKhau * 100/ ct.PTChietKhau) as DaTrich,
		cast(ct.PTThue as int) as TrangThai
	from BH_HoaDon_ChiTiet ct  	
   join BH_HoaDon hd on ct.ID_ParentCombo = hd.ID
   join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
     where ct.ID_HoaDon= @ID
	 order by dt.MaDoiTuong, hd.NgayLapHoaDon desc");

			CreateStoredProcedure(name: "[dbo].[GetInforContact_byID]", parametersAction: p => new
			{
				ID = p.Guid()
			}, body: @"SET NOCOUNT ON;

	select 
		lh.ID,
		lh.MaLienHe,
		lh.TenLienHe,
		lh.SoDienThoai,
		lh.NgaySinh,
		lh.ID_DoiTuong,
		lh.SoDienThoai,
		lh.DienThoaiCoDinh,
		lh.XungHo,
		lh.DiaChi,
		lh.ChucVu,
		lh.GhiChu,		
		dt.MaDoiTuong,
		dt.TenDoiTuong,
		dt.LoaiDoiTuong as LoaiLienHe,
		tt.TenTinhThanh,
		qh.TenQuanHuyen
	from DM_LienHe lh
	left join DM_DoiTuong dt on lh.ID_DoiTuong= dt.ID
	left join DM_TinhThanh tt on lh.ID_TinhThanh= tt.ID
	left join DM_QuanHuyen qh on lh.ID_QuanHuyen= qh.ID
	where lh.ID = @ID");

			CreateStoredProcedure(name: "[dbo].[GetList_NguoiGioiThieu]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSearch = p.String(),
				TrichHoaHong_DateFrom = p.DateTime(),
				TrichHoaHong_DateTo = p.DateTime(),
				NgayTaoFrom = p.DateTime(),
				NgayTaoTo = p.DateTime(),
				LoaiDoiTuongs = p.String(),
				TrangThais = p.String(),
				CurrentPage = p.Int(defaultValue: 0),
				PageSize = p.Int(defaultValue: 10)
			}, body: @"SET NOCOUNT ON;

	declare @tblLoaiDoiTuong table (LoaiDT tinyint)
	if(isnull(@LoaiDoiTuongs,'')!='')
		begin
			insert into @tblLoaiDoiTuong
			select name from dbo.splitstring(@LoaiDoiTuongs)
		end

	declare @tblChiNhanh table (ID uniqueidentifier)

	declare @QuanLyKH_theoCN bit = '0'
	declare @tblIDNhoms table (ID uniqueidentifier) 
	declare @tblNhanVien table (ID uniqueidentifier)

	if(isnull(@IDChiNhanhs,'')!='')
		begin
			insert into @tblChiNhanh
			select name from dbo.splitstring(@IDChiNhanhs)

				------ lay khachhang chinhanh ------			
				set @QuanLyKH_theoCN  = (select max(cast(QuanLyKhachHangTheoDonVi as int)) from HT_CauHinhPhanMem where ID_DonVi in (select ID from @tblChiNhanh))		
				if @QuanLyKH_theoCN ='1'
					begin
					insert into @tblIDNhoms
					select *  
						from (
    					-- get Nhom not not exist in NhomDoiTuong_DonVi
    					select ID  from DM_NhomDoiTuong nhom  
    					where LoaiDoiTuong in (1,3,4) 						
    					and not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    					union all
    					-- get Nhom at this ChiNhanh
    					select ID_NhomDoiTuong from NhomDoiTuong_DonVi where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) ) tbl
					end


			------ get nhanvien theo chinhanh -----			
			insert into @tblNhanVien
			select ct.ID_NhanVien
			from NS_QuaTrinhCongTac ct
			where exists (select cn.ID from @tblChiNhanh cn where ct.ID_DonVi= cn.ID)
		end

	declare @tblTrangThai table (TrangThai tinyint)
	if(isnull(@TrangThais,'')!='')
		begin
			insert into @tblTrangThai
			select name from dbo.splitstring(@TrangThais)
		end

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch)

	;with data_cte
	as (
	select 
		tbl.ID_CheckIn,
		tbl.SLoaiDoiTuong,
		tbl.MaNguoiGioiThieu,
		tbl.TenNguoiGioiThieu,
		tbl.SDTNguoiGioiThieu,
		tbl.DiaChiNguoiGioiThieu,
		tbl.NgayTao,
		tbl.NguoiTao,
		tbl.TrangThai,
		tbl.LoaiDoiTuong,
		tbl.IDNhomDoiTuongs,
		case tbl.TrangThai
			when 1 then N'Đang theo dõi'
		else N'Đã xóa' end STrangThai,
		sum(tbl.TongTienHang) as TongTienHang			
	from
	(
	   select hd.ID, 
			hd.TongTienHang, 
			hd.ID_CheckIn,
			hd.TongChietKhau as LoaiDoiTuong,
			case hd.TongChietKhau		
				when 1 then N'Khách hàng'
				when 2 then N'Nhà cung cấp'
				when 3 then N'Bảo hiểm'
				when 4 then N'Khác'
				when 5 then N'Nhân viên'
			end as SLoaiDoiTuong,
			iif(hd.TongChietKhau=5,nv.MaNhanVien,dt.MaDoiTuong) as MaNguoiGioiThieu,	
			iif(hd.TongChietKhau=5,nv.Tamtru,dt.DiaChi) as DiaChiNguoiGioiThieu,
			iif(hd.TongChietKhau=1,ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000'),'00000000-0000-0000-0000-000000000000') as IDNhomDoiTuongs,					
			case hd.TongChietKhau			
				when 5 then nv.TenNhanVien			
			else dt.TenDoiTuong
			end as TenNguoiGioiThieu,
			case hd.TongChietKhau			
				when 5 then nv.DienThoaiDiDong			
				else dt.DienThoai
			end as SDTNguoiGioiThieu,
			case hd.TongChietKhau			
				when 5 then nv.TenNhanVienKhongDau			
				else dt.TenDoiTuong_KhongDau
			end as TenNguoiGioiThieu_KhongDau,
			case hd.TongChietKhau		
				when 5 then nv.NgayTao
				else dt.NgayTao
			end as NgayTao,
			case hd.TongChietKhau		
				when 5 then nv.NguoiTao
				else dt.NguoiTao
			end as NguoiTao,
			case hd.TongChietKhau		
				when 5 then isnull(nv.TrangThai,1) ---- 1.danglamviec, 2. daxoa
				else iif(isnull(dt.TheoDoi,'0')='0','1','0') --- get value of TheoDoi & đảo ngược giá trị nhận được (1.theodoi, 0.daxoa)
			end as TrangThai
	   from BH_HoaDon hd
	   left join DM_DoiTuong dt on hd.ID_CheckIn = dt.ID
	   left join NS_NhanVien nv on hd.ID_CheckIn= nv.ID and hd.TongChietKhau= 5
	   where hd.LoaiHoaDon = 41
	   and hd.ChoThanhToan ='0'
	   and (@TrichHoaHong_DateFrom is null or hd.NgayLapHoaDon between @TrichHoaHong_DateFrom and @TrichHoaHong_DateTo)	 
	   and (@LoaiDoiTuongs='' or exists (select loai.LoaiDT from @tblLoaiDoiTuong loai where hd.TongChietKhau= loai.LoaiDT))	
	
   ) tbl
   where 
   (@TrangThais='' or exists (select tt.TrangThai from @tblTrangThai tt where tbl.TrangThai= tt.TrangThai))
    and 
	(@NgayTaoFrom is null or tbl.NgayTao between @NgayTaoFrom and @NgayTaoTo)
	and (@IDChiNhanhs = ''
		or (tbl.LoaiDoiTuong = 5 and exists (select ID from @tblNhanVien nv2 where tbl.ID_CheckIn= nv2.ID))
		or (tbl.LoaiDoiTuong != 5 
			and (@QuanLyKH_theoCN='0' 
					or (@QuanLyKH_theoCN='1' and tbl.LoaiDoiTuong in (2,3,4))
					or (@QuanLyKH_theoCN='1' and exists (SELECT name FROM splitstring(tbl.IDNhomDoiTuongs) nhom join @tblIDNhoms tblsearch ON nhom.Name = tblsearch.ID and nhom.Name!=''))
				)
			)
		)
	and
		((select count(Name) from @tblSearch b where     			
		tbl.MaNguoiGioiThieu like '%'+b.Name+'%'			
		or tbl.TenNguoiGioiThieu like '%'+b.Name+'%'
		or tbl.SDTNguoiGioiThieu like '%'+b.Name+'%'
		or tbl.TenNguoiGioiThieu_KhongDau like '%'+b.Name+'%'		
		)=@count or @count=0)	
	group by tbl.ID_CheckIn,
		tbl.SLoaiDoiTuong,
		tbl.MaNguoiGioiThieu,
		tbl.TenNguoiGioiThieu,
		tbl.SDTNguoiGioiThieu,
		tbl.DiaChiNguoiGioiThieu,
		tbl.NgayTao,
		tbl.NguoiTao,
		tbl.TrangThai,
		tbl.LoaiDoiTuong,
		tbl.IDNhomDoiTuongs
		),
		count_cte
		as (
			select count(ID_CheckIn) as TotalRow,
				CEILING(COUNT(ID_CheckIn) / CAST(@PageSize as float ))  as TotalPage,
				sum(dt.TongTienHang) as SumTongTienHang
			from data_cte dt
		)
		select dt.*, cte.* 
		from data_cte dt	
		cross join count_cte cte
		order by dt.NgayTao desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY;");

			CreateStoredProcedure(name: "[dbo].[GetList_PhieuTrichHoaHong]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSearch = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				LoaiDoiTuongs = p.String(),
				TrangThais = p.String(),
				CurrentPage = p.Int(defaultValue: 0),
				PageSize = p.Int(defaultValue: 10)
			}, body: @"SET NOCOUNT ON;

	declare @tblChiNhanh table (ID uniqueidentifier)
	if(isnull(@IDChiNhanhs,'')!='')
		begin
			insert into @tblChiNhanh
			select name from dbo.splitstring(@IDChiNhanhs)
		end


	declare @tblLoaiDoiTuong table (LoaiDT tinyint)
	if(isnull(@LoaiDoiTuongs,'')!='')
		begin
			insert into @tblLoaiDoiTuong
			select name from dbo.splitstring(@LoaiDoiTuongs)
		end

	declare @tblTrangThai table (TrangThai tinyint)
	if(isnull(@TrangThais,'')!='')
		begin
			insert into @tblTrangThai
			select name from dbo.splitstring(@TrangThais)
		end

	if(isnull(@DateFrom,'')='') set @DateFrom ='2016-01-01'
	if(isnull(@DateTo,'')='') set @DateTo = DATEADD(day,1,getdate())

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	;with data_cte
	as(
	select 
		tbl.*,
		dv.TenDonVi,
		case tbl.TrangThai
			when 0 then N'Hoàn thành'
			when 2 then N'Đã hủy'
			when 1 then N'Tạm lưu'
		end as STrangThai
	from
	(
	select 
		hd.ID,
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hd.TongChietKhau,
		hd.TongTienHang,
		hd.ID_DonVi,
		hd.ID_CheckIn,
		hd.NguoiTao,
		hd.DienGiai,
		isnull(sq.TongChi,0) as KhachDaTra,
		iif(hd.ChoThanhToan ='0',hd.PhaiThanhToan - isnull(sq.TongChi,0),0) as ConNo,
		case hd.TongChietKhau		
			when 1 then N'Khách hàng'
			when 2 then N'Nhà cung cấp'
			when 3 then N'Bảo hiểm'
			when 4 then N'Khác'
			when 5 then N'Nhân viên'
		end as SLoaiDoiTuong,
		case hd.ChoThanhToan	
			when '1' then 1
			when '0' then 0 
			else 2 
		end as TrangThai,
		case hd.TongChietKhau			
			when 1 then dt.MaDoiTuong
			when 2 then dt.MaDoiTuong
			when 3 then dt.MaDoiTuong
			when 4 then dt.MaDoiTuong
			when 5 then nv.MaNhanVien
		end as MaNguoiGioiThieu,
		case hd.TongChietKhau			
			when 1 then dt.TenDoiTuong
			when 2 then dt.TenDoiTuong
			when 3 then dt.TenDoiTuong
			when 4 then dt.TenDoiTuong
			when 5 then nv.TenNhanVien
		end as TenNguoiGioiThieu,
		case hd.TongChietKhau			
			when 1 then dt.DienThoai
			when 2 then dt.DienThoai
			when 3 then dt.DienThoai
			when 4 then dt.DienThoai
			when 5 then nv.DienThoaiDiDong
		end as SDTNguoiGioiThieu
	from BH_HoaDon hd
	left join DM_DoiTuong dt on hd.ID_CheckIn= dt.ID 
	left join NS_NhanVien nv on hd.ID_CheckIn= nv.ID and hd.TongChietKhau= 5
	left join 
	(
		select 
			qct.ID_HoaDonLienQuan,
			sum(iif(qhd.LoaiHoaDon =11, - qct.TienThu, qct.TienThu)) as TongChi
		from Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
		where (qhd.TrangThai is null or qhd.TrangThai='1')
		group by qct.ID_HoaDonLienQuan
	) sq on hd.ID= sq.ID_HoaDonLienQuan
	where hd.LoaiHoaDon= 41
	and hd.NgayLapHoaDon between @DateFrom and @DateTo
	and (@IDChiNhanhs='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID))	
	and (@LoaiDoiTuongs='' or exists (select loai.LoaiDT from @tblLoaiDoiTuong loai where hd.TongChietKhau= loai.LoaiDT))	
	and
		((select count(Name) from @tblSearch b where     			
		hd.MaHoaDon like '%'+b.Name+'%'			
		or nv.TenNhanVien like '%'+b.Name+'%'
		or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
		or hd.DienGiai like '%'+b.Name+'%'
		or dt.MaDoiTuong like '%'+b.Name+'%'		
		or dt.TenDoiTuong like '%'+b.Name+'%'
		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or dt.DienThoai like '%'+b.Name+'%'				
		)=@count or @count=0)	
	) tbl
	left join DM_DonVi dv on tbl.ID_DonVi= dv.ID
	where (@TrangThais='' or exists (select tt.TrangThai from @tblTrangThai tt where tbl.TrangThai= tt.TrangThai))
	),
	count_cte
	as
	(
		select count(ID) as TotalRow,
			CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
			sum(TongTienHang) as SumTongTienHang,
			sum(KhachDaTra) as SumKhachDaTra,
			sum(ConNo) as SumConNo
		from data_cte
	)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY; ");

			CreateStoredProcedure(name: "[dbo].[GetPhieuTrichHoaHong_byID]", parametersAction: p => new
			{
				ID = p.Guid()
			}, body: @"SET NOCOUNT ON;

    select 
		hd.ID,
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hd.TongChietKhau,
		hd.TongTienHang,
		hd.ID_DonVi,
		hd.ID_CheckIn,
		hd.NguoiTao,
		hd.DienGiai,
		isnull(sq.TongChi,0) as KhachDaTra,
		hd.PhaiThanhToan - isnull(sq.TongChi,0) as ConNo,
		case hd.TongChietKhau		
			when 1 then N'Khách hàng'
			when 2 then N'Nhà cung cấp'
			when 3 then N'Bảo hiểm'
			when 4 then N'Khác'
			when 5 then N'Nhân viên'
		end as SLoaiDoiTuong,
		case hd.ChoThanhToan	
			when '1' then 1
			when '0' then 0 
			else 2 
		end as TrangThai,
		case hd.TongChietKhau			
			when 1 then dt.MaDoiTuong
			when 2 then dt.MaDoiTuong
			when 3 then dt.MaDoiTuong
			when 4 then dt.MaDoiTuong
			when 5 then nv.MaNhanVien
		end as MaNguoiGioiThieu,
		case hd.TongChietKhau			
			when 1 then dt.TenDoiTuong
			when 2 then dt.TenDoiTuong
			when 3 then dt.TenDoiTuong
			when 4 then dt.TenDoiTuong
			when 5 then nv.TenNhanVien
		end as TenNguoiGioiThieu,
		case hd.TongChietKhau			
			when 1 then dt.DienThoai
			when 2 then dt.DienThoai
			when 3 then dt.DienThoai
			when 4 then dt.DienThoai
			when 5 then nv.DienThoaiDiDong
		end as SDTNguoiGioiThieu
	from BH_HoaDon hd
	left join DM_DoiTuong dt on hd.ID_CheckIn= dt.ID 
	left join NS_NhanVien nv on hd.ID_CheckIn= nv.ID and hd.TongChietKhau= 5
	left join 
	(
		select 
			qct.ID_HoaDonLienQuan,
			sum(iif(qhd.LoaiHoaDon =11, - qct.TienThu, qct.TienThu)) as TongChi
		from Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
		where (qhd.TrangThai is null or qhd.TrangThai='1')
		group by qct.ID_HoaDonLienQuan
	) sq on hd.ID= sq.ID_HoaDonLienQuan
	where hd.ID= @ID");

			CreateStoredProcedure(name: "[dbo].[GetPhieuTrichHoaHong_byNguoiGioiThieu]", parametersAction: p => new
			{
				ID_NguoiGioiThieu = p.Guid(),
				IDChiNhanhs = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				TrangThais = p.String(),
				CurrentPage = p.Int(defaultValue: 0),
				PageSize = p.Int(defaultValue: 10)
			}, body: @"SET NOCOUNT ON;

		declare @tblChiNhanh table (ID uniqueidentifier)
		if(isnull(@IDChiNhanhs,'')!='')
			insert into @tblChiNhanh
			select name from dbo.splitstring(@IDChiNhanhs)


		declare @tblTrangThai table (TrangThai tinyint)
		if(isnull(@TrangThais,'')!='')
		begin
			insert into @tblTrangThai
			select name from dbo.splitstring(@TrangThais)
		end

	;with data_cte
	as(
			
		select 
			pt.ID,		
			pt.MaHoaDon,
			pt.NgayLapHoaDon,
			pt.PhaiThanhToan,
			sq.MaHoaDon as MaPhieuChi,
			ISNULL(sq.TienChi,0) as DuNoKH		---- muontruong (vi dung chung class SoQuyDTO)	
		from BH_HoaDon pt 
		left join 
		(
			select 
				qhd.ID, 
				qhd.MaHoaDon,
				qct.ID_HoaDonLienQuan,
				sum(iif(qhd.LoaiHoaDon = 11, -qct.TienThu, qct.TienThu)) as TienChi
			from Quy_HoaDon qhd
			join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
			where (qhd.TrangThai is null or qhd.TrangThai='1')
			group by qhd.ID, 
				qhd.MaHoaDon,
				qct.ID_HoaDonLienQuan
		) sq on pt.ID= sq.ID_HoaDonLienQuan
		where pt.ID_CheckIn= @ID_NguoiGioiThieu
		and pt.ChoThanhToan='0'
		and pt.NgayLapHoaDon between @DateFrom and @DateTo
		and (@IDChiNhanhs='' or exists (select ID from @tblChiNhanh cn where pt.ID_DonVi= cn.ID))		
		),
	count_cte
		as
		(
			select count(ID) as TotalRow
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY; ");

			CreateStoredProcedure(name: "[dbo].[TGT_GetNhatKyDieuChinh]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				TextSearch = p.String(),
				TrangThais = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblChiNhanh table (ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@IDChiNhanhs)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    

	;with data_cte
		as(		
		select *
		from
		(
    		   select hd.ID, 
					hd.ID_DonVi,
					hd.ID_DoiTuong,
					hd.ID_NhanVien,
					hd.MaHoaDon, 
					hd.LoaiHoaDon,
					hd.NgayLapHoaDon, 			
    				hd.TongTienHang,
					hd.TongGiamGia,
					hd.TongChietKhau,
					hd.DienGiai,
					dt.MaDoiTuong,
					dt.TenDoiTuong,
					dt.DienThoai,
					hd.NguoiTao,
					dv.TenDonVi,
					hd.ChoThanhToan,
					iif(hd.TongTienHang > 0, hd.TongTienHang,0) as PhatSinhTang,				
					iif(hd.TongTienHang < 0, hd.TongTienHang,0) as PhatSinhGiam,
    				iif(hd.TongTienHang > 0,  N'Điều chỉnh tăng', N'Điều chỉnh giảm') as SLoaiHoaDon,
					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai,
					case when hd.ChoThanhToan is null then N'Đã hủy' else N'Hoàn thành' end as STrangThai
    			from BH_HoaDon hd
    			join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			join DM_DonVi dv on hd.ID_DonVi= dv.ID
    			where LoaiHoaDon = 23
				and hd.NgayLapHoaDon between @FromDate and @ToDate
    			and exists (select ID_DonVi from @tblChiNhanh cn where hd.ID_DonVi= dv.ID)
					AND ((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%'
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    					or dt.DienThoai like '%'+b.Name+'%'			
    					or hd.MaHoaDon like '%' +b.Name +'%'     							
    					)=@count or @count=0)	
				) tbl
			where tbl.TrangThai like @TrangThais 
			),
			count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(PhatSinhTang) as TongTang,
				sum(PhatSinhGiam) as TongGiam
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetListHoaDon_byIDCus]", parametersAction: p => new
			{
				ID_NguoiGioiThieu = p.Guid(),
				ID_DoiTuong = p.Guid(),
				IDChiNhanhs = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime()
			}, body: @"SET NOCOUNT ON;

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
				sum(iif(cthh.PTChietKhau= 0,0,cthh.TienChietKhau * 100/ cthh.PTChietKhau)) as DaTrich
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
	order by tblView.NgayLapHoaDon ");

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

	  declare @tblLoaiHoaDon table (LoaiHD int)
	 insert into @tblLoaiHoaDon
	 select Name from splitstring(@LoaiChungTu)

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


	declare @tblGVTC table(ID_DonVi uniqueidentifier, ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
	GiaVonTieuChuan float, NgayLapHoaDon datetime)

	insert into @tblGVTC		
	select hd.ID_DonVi,
		ct.ID_DonViQuiDoi, ct.ID_LoHang, 
    	ct.ThanhTien as GiaVonTieuChuan,    	
    	hd.NgayLapHoaDon
    from BH_HoaDon_ChiTiet ct 
    join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    where hd.ChoThanhToan=0
    and  hd.ID_DonVi in (select ID from @tblIdDonVi)
    and hd.LoaiHoaDon= 16
    and hd.NgayLapHoaDon < @timeEnd	

	select *
	into #tblCTSD
	from
	(
	select *,
	ROW_NUMBER() over (partition by tbl.ID_DonVi,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,tbl.ID_ChiTietDinhLuong, tbl.ID_ChiTietGoiDV order by tbl.NgayDieuChinh desc) as RN
		
	from
	(
	select 
		ct.ID_HoaDon,   
		hd.MaHoaDon,
		hd.ID_DonVi,
		hd.ID_NhanVien,
		hd.ID_DoiTuong,
		hd.NgayLapHoaDon,
		gv.NgayLapHoaDon as NgayDieuChinh,
		ct.ID_DonViQuiDoi,
		ct.ID_LoHang,     
		ct.ID_ChiTietDinhLuong,
		ct.ID_ChiTietGoiDV,
		ct.GhiChu,			
		hd.LoaiHoaDon,

		isnull(gv.GiaVonTieuChuan,0) as GiaVonTieuChuan,
		ISNULL(ct.SoLuong,0) * isnull(gv.GiaVonTieuChuan,0) as GiaTriDichVu,
						
    	ISNULL(ct.SoLuong,0) AS SoLuongXuat,
		ISNULL(ct.SoLuong,0) * isnull(ct.GiaVon,0) AS GiaTriXuat						
	
    from BH_HoaDon_ChiTiet ct 
    join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
	left join @tblGVTC gv on hd.ID_DonVi= gv.ID_DonVi and ct.ID_DonViQuiDoi= gv.ID_DonViQuiDoi 
		and (ct.ID_LoHang = gv.ID_LoHang or (ct.ID_LoHang is null and gv.ID_LoHang is null))
    where hd.ChoThanhToan=0
	and ct.ID_ChiTietDinhLuong is not null
	and hd.LoaiHoaDon in ( 35,38,39,40) ---- get ctxuatkho of hdBanLe, hdBaoHang + hdHoTro
	and (hd.NgayLapHoaDon > gv.NgayLapHoaDon or gv.NgayLapHoaDon is null)---- giavon tieuchuan (cuahanghoa dinhluong) theo khoang thoigian
	) tbl
	) tblOut where tblOut.RN= 1

	select *,
	case 
			when tbl.SoLuongChenhLech < 0 then N'Xuất thiếu'
			when tbl.SoLuongChenhLech > 0 then N'Xuất thêm'	
		else N'Xuất đủ' end as TrangThai,
		iif(tbl.ThanhTienDichVu=0,100, isnull(tbl.GiaTriDichVu,0)/ isnull(tbl.ThanhTienDichVu,1) *100) as PtramSuDung
	from
	(
	select hdm.MaHoaDon,
		hdm.ID_DonVi,
		hdm.ID_NhanVien,
		hdm.NgayLapHoaDon,
		--hdm.LoaiHoaDon,
		dv.TenDonVi,
		dv.MaDonVi,
		dt.MaDoiTuong,
		dt.TenDoiTuong,
		isnull(nv.TenNhanVien,'') as TenNhanVien,

		ctm.ID_DonViQuiDoi as ID_DichVu,
		ctm.SoLuong as SoLuongDichVu,
		iif(hdm.LoaiHoaDon= 36,0, ctm.DonGia - ctm.TienChietKhau) as GiaBanDichVu,
		iif(hdm.LoaiHoaDon= 36,0,ctm.SoLuong * (ctm.DonGia - ctm.TienChietKhau)) as ThanhTienDichVu,
		isnull(dv2.GiaTriDichVu,0) as GiaTriDichVu,

		tpdl.ID_DonViQuiDoi,	
		tpdl.GhiChu,
		isnull(tpdl.SoLuongXuat,0) as SoLuongThucTe,
		isnull(tpdl.GiaTriXuat,0) as GiaTriXuat,

		hhDV.TenHangHoa as TenDichVu,
		qdDV.MaHangHoa as MaDichVu,
		qdDV.TenDonViTinh as TenDonViDichVu,
		qdDV.ThuocTinhGiaTri as ThuocTinhDichVu,

		hhTP.TenHangHoa,
		qdTP.MaHangHoa,
		qdTP.TenDonViTinh,
		qdTP.ThuocTinhGiaTri,
		concat(hhTP.TenHangHoa, qdTP.ThuocTinhGiaTri) as TenHangHoaFull,

		nhomDV.TenNhomHangHoa as NhomDichVu,
		isnull(nhomHH.TenNhomHangHoa,'') as TenNhomHang,
		isnull(nvth.NVThucHiens,'') as NVThucHiens,
	
		isnull(tpdl.SoLuongDinhLuongBanDau,0) as SoLuongDinhLuongBanDau,
		isnull(tpdl.SoLuongXuat,0) - isnull(tpdl.SoLuongDinhLuongBanDau,0) as SoLuongChenhLech,
		iif(@XemGiaVon='1', isnull(tpdl.GiaTriXuat,0),0) as GiaTriThucTe,	
		iif(@XemGiaVon='1', isnull(tpdl.GiaTriDinhLuongBanDau,0),0) as GiaTriDinhLuongBanDau,			
		iif(@XemGiaVon='1', isnull(tpdl.GiaTriXuat,0) - isnull(tpdl.GiaTriDinhLuongBanDau,0),0) as GiaTriChenhLech,			

		case hdm.LoaiHoaDon
			when 1 then case when ctm.ChatLieu ='4' then 5 else 1 end ---- chatlieu = 4. sudung gdv
		else hdm.LoaiHoaDon end as LoaiHoaDon,

		case hdm.LoaiHoaDon
			when 1 then case when ctm.ChatLieu ='4' then N'Hóa đơn sử dụng' else N'Hóa đơn bán lẻ' end ---- chatlieu = 4. sudung gdv
			when 2 then N'Hóa đơn bảo hành'
			when 36 then N'Hóa đơn hỗ trợ'		
			else '' end TenLoaiChungTu

	from BH_HoaDon_ChiTiet ctm
	left join(
			select distinct thout.ID_ChiTietHoaDon,
					(
						select nv.TenNhanVien +', ' as [text()]
						from Bh_NhanVienThucHien th
						join BH_HoaDon_ChiTiet ct on th.ID_ChiTietHoaDon= ct.ID
						join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
						join NS_NhanVien nv on th.ID_NhanVien = nv.ID
						where hd.ChoThanhToan='0'
						and hd.NgayLapHoaDon between @timeStart and @timeEnd
						and hd.LoaiHoaDon in ( 1,25,2)
						and th.ID_ChiTietHoaDon= thout.ID_ChiTietHoaDon
							For XML PATH ('')
					) NVThucHiens 
				from Bh_NhanVienThucHien thout where thout.ID_ChiTietHoaDon is not null
			) nvth on ctm.ID = nvth.ID_ChiTietHoaDon	
	left join 
	(
	--- get dinhluong of dichvu at xuatkho (get ctxuatthucte + giavon tieuchuan)
	select 
		ctm1.ID_ChiTietDinhLuong,	
		ctx.ID_DonViQuiDoi,
		ctx.SoLuongXuat,
		ctx.GhiChu,
		ctx.GiaTriXuat,
		ctx.SoLuongXuat as SoLuongDinhLuongBanDau, 
		ctx.GiaTriDichVu as GiaTriDinhLuongBanDau
	from #tblCTSD ctx
	join BH_HoaDon_ChiTiet ctm1 on ctx.ID_ChiTietGoiDV= ctm1.ID
	) tpdl on tpdl.ID_ChiTietDinhLuong = ctm.ID
	left join 
	(
	--- sum giavon tieuchuan
	select 
		ctm1.ID_ChiTietDinhLuong,
		sum(ctx.GiaTriDichVu) as GiaTriDichVu	
	from #tblCTSD ctx
	join BH_HoaDon_ChiTiet ctm1 on ctx.ID_ChiTietGoiDV= ctm1.ID group by ctm1.ID_ChiTietDinhLuong
	) dv2 on tpdl.ID_ChiTietDinhLuong = ctm.ID and tpdl.ID_ChiTietDinhLuong = dv2.ID_ChiTietDinhLuong

	join BH_HoaDon hdm on ctm.ID_HoaDon= hdm.ID
	left join DM_DonVi dv on hdm.ID_DonVi = dv.ID
	left join NS_NhanVien nv on hdm.ID_NhanVien= nv.ID	
	left join DM_DoiTuong dt on hdm.ID_DoiTuong = dt.ID

	join DonViQuiDoi qdDV on ctm.ID_DonViQuiDoi = qdDV.ID
	left join DonViQuiDoi qdTP on tpdl.ID_DonViQuiDoi = qdTP.ID

	join DM_HangHoa hhDV on qdDV.ID_HangHoa = hhDV.ID
	left join DM_HangHoa hhTP on qdTP.ID_HangHoa = hhTP.ID

	join DM_NhomHangHoa nhomDV on hhDV.ID_NhomHang = nhomDV.ID
	left join DM_NhomHangHoa nhomHH on hhTP.ID_NhomHang = nhomHH.ID

	where hdm.ChoThanhToan='0' and ctm.ID_ChiTietDinhLuong = ctm.ID
	
	and hdm.NgayLapHoaDon between @timeStart and @timeEnd
	and hhDV.TheoDoi like @TheoDoi			
			and qdDV.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhomDV.ID = allnhh.ID )
			and exists (select id from @tblIdDonVi dv2 where dv.ID= dv2.ID)
			and
			 ((select count(Name) from @tblSearchString b where 
    			hdm.MaHoaDon like '%'+b.Name+'%' 			
    			or qdTP.MaHangHoa like '%'+b.Name+'%' 
    			or hhdv.TenHangHoa like '%'+b.Name+'%'
    			or hhdv.TenHangHoa_KhongDau like '%' +b.Name +'%' 
				or hhTP.TenHangHoa like '%'+b.Name+'%'
    			or hhTP.TenHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhomDV.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhomHH.TenNhomHangHoa like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    			or qddv.MaHangHoa like '%'+b.Name+'%' 
    			or TenNhanVien like '%'+b.Name+'%'    
					)=@count or @count=0)
	) tbl
	where exists (select LoaiHD from @tblLoaiHoaDon loai where tbl.LoaiHoaDon = loai.LoaiHD)
	order by tbl.NgayLapHoaDon desc, tbl.MaDichVu
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetMaDoiTuong_Max]
    @LoaiDoiTuong [int]
AS
BEGIN
    DECLARE @MaDTuongOffline varchar(5);
    DECLARE @MaDTuongTemp varchar(5);
    DECLARE @Return float
    
    if @LoaiDoiTuong = 1 
    	BEGIN
    		SET @MaDTuongOffline ='KHO'
    		SET @MaDTuongTemp='KH'
    	END 
    else if @LoaiDoiTuong = 2 
    	BEGIN
    		SET @MaDTuongOffline ='NCCO'
    		SET @MaDTuongTemp='NCC'
    	END 
    else if @LoaiDoiTuong = 3
    BEGIN
    	SET @MaDTuongOffline ='BHO';
    	SET @MaDTuongTemp='BH';
    END
	  else if @LoaiDoiTuong = 4
    BEGIN
    	SET @MaDTuongOffline ='GTO';
    	SET @MaDTuongTemp='GT';
    END
    
		DECLARE @MaDoiTuongMax NVARCHAR(MAX);
			select TOP 1 @MaDoiTuongMax = MaDoiTuong 
				from DM_DoiTuong
				where LoaiDoiTuong = @LoaiDoiTuong
					and MaDoiTuong like @MaDTuongTemp +'%'  AND MaDoiTuong not like @MaDTuongOffline + '%'
					and len(ISNULL(MaDoiTuong,'')) > 8
				ORDER BY MaDoiTuong desc;
    	
    	SELECT @Return = CAST(dbo.udf_GetNumeric(@MaDoiTuongMax)AS float);
    
    	if	@Return is null 
    		select Cast(0 as float) as MaxCode
    	else 
    		select @Return as MaxCode
END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateIDKhachHang_inSoQuy]
    @ID_HoaDonLienQuan [uniqueidentifier],
	@LoaiDoiTuong int = 1
AS
BEGIN
	set nocount on;
	declare @sLoaiDoiTuong nvarchar(max) =N'khách hàng'
	if @LoaiDoiTuong = 2
		set @sLoaiDoiTuong = N'nhà cung cấp'
	if @LoaiDoiTuong = 3
		set @sLoaiDoiTuong = N'cty bảo hiểm'
	if @LoaiDoiTuong = 4
		set @sLoaiDoiTuong = N'người giới thiệu'
	if @LoaiDoiTuong = 5
		set @sLoaiDoiTuong = N'nhân viên'

    declare @ID_DonVi UNIQUEIDENTIFIER, @ID_NhanVien  UNIQUEIDENTIFIER, @MaHoaDon nvarchar(max), @MaPhieuThus nvarchar(max)
	select @ID_NhanVien = ID_NhanVien, @ID_DonVi = ID_DonVi ,@MaHoaDon = MaHoaDon
	from BH_HoaDon hd where id= @ID_HoaDonLienQuan

	declare @sDetail nvarchar(max) =concat(N'Cập nhật hóa đơn ', @MaHoaDon , N': hủy phiếu thu ', @MaPhieuThus , N' của ', @sLoaiDoiTuong , N' cũ')

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
		values (NEWID(), @ID_DonVi, @ID_NhanVien, 2, N'Cập nhật hóa đơn - thay đổi '+ @sLoaiDoiTuong, 
		concat(N'Cập nhật hóa đơn ', @MaHoaDon , N' thay đổi '+ @sLoaiDoiTuong),
		@sDetail,
		GETDATE())
		
END");


        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BCThuChi_TheoLoaiTien]");
			DropStoredProcedure("[dbo].[GetAll_ChiTietPhieuTrich]");
			DropStoredProcedure("[dbo].[GetBaoCaoCongNoChiTiet]");
			DropStoredProcedure("[dbo].[GetChiTietHoaHongGioiThieu_byID]");
			DropStoredProcedure("[dbo].[GetInforContact_byID]");
			DropStoredProcedure("[dbo].[GetList_NguoiGioiThieu]");
			DropStoredProcedure("[dbo].[GetList_PhieuTrichHoaHong]");
			DropStoredProcedure("[dbo].[GetPhieuTrichHoaHong_byID]");
			DropStoredProcedure("[dbo].[GetPhieuTrichHoaHong_byNguoiGioiThieu]");
			DropStoredProcedure("[dbo].[TGT_GetNhatKyDieuChinh]");
			DropStoredProcedure("[dbo].[GetListHoaDon_byIDCus]");
        }
    }
}
