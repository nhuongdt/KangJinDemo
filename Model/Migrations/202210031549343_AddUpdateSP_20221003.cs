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

			Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoaXuatHuybyID]
    @ID_HoaDon [uniqueidentifier],
	@ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
  set nocount on;

		declare @loaiHD int, @ID_HoaDonGoc uniqueidentifier		
		select @loaiHD = LoaiHoaDon, @ID_HoaDonGoc= ID_HoaDon from BH_HoaDon where ID= @ID_HoaDon

		
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
			 hd.MaHoaDon,
			hd.NgayLapHoaDon,
			hd.ID_NhanVien,
    		--nv.TenNhanVien,
			dvqd.ID_HangHoa,
			dvqd.MaHangHoa,
			dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			lh.MaLoHang,
			Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
			lh.NgaySanXuat,
    		lh.NgayHetHan,    			
    		hh.TenHangHoa,
			Case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang, 
    		concat(hh.TenHangHoa , '', dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
			
			cast(iif(hdm.ID is null, 0, ---- xuatkho noibo
				case hdm.ChoThanhToan
					when '1' then 1
					when '0' then iif(ctm.ID is null,1,0) --- capnhat hdmua: huy xuatkho cu --> ID_ChiTietGoiDV bi thay doi
				else 2 end) as float) as TrangThaiMoPhieu,				
			ROUND(ISNULL(TonKho,0),2) as TonKho
		from BH_HoaDon_ChiTiet ctxk		
		join BH_HoaDon hd on hd.ID= ctxk.ID_HoaDon and hd.ID= @ID_HoaDon
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join BH_HoaDon hdm on hd.ID_HoaDon= hdm.ID
		left join BH_HoaDon_ChiTiet ctm on ctxk.ID_ChiTietGoiDV = ctm.ID ----capnhat hdmua: huy xuatkho cu --> ID_ChiTietGoiDV bi thay doi		
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		where (hh.LaHangHoa = 1 and tk.TonKho is not null) 
		and (ctxk.ChatLieu is null or ctxk.ChatLieu != '5') 


	
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
				iif(qct.ID_NhanVien is null,isnull(dt.LoaiDoiTuong,0),5) as LoaiDoiTuong,
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

			Sql(@"ALTER PROCEDURE [dbo].[GetTPDinhLuong_ofCTHD]
    @ID_CTHD [uniqueidentifier],
	@LoaiHoaDon int null
AS
BEGIN
    SET NOCOUNT ON;

	if @LoaiHoaDon is null
	begin
		-- hoadonban
		select  MaHangHoa, TenHangHoa, ID_DonViQuiDoi, TenDonViTinh, SoLuong, ct.GiaVon, ct.ID_HoaDon,ID_ChiTietGoiDV, ct.ID_LoHang,
			ct.SoLuongDinhLuong_BanDau,
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoaThayThe,
    		case when ISNULL(QuyCach,0) = 0 then ISNULL(TyLeChuyenDoi,1) else ISNULL(QuyCach,0) * ISNULL(TyLeChuyenDoi,1) end as QuyCach,
    		qd.TenDonViTinh as DonViTinhQuyCach, ct.GhiChu	,
			ceiling(qd.GiaNhap) as GiaNhap, qd.GiaBan as GiaBanHH, lo.MaLoHang, lo.NgaySanXuat, lo.NgayHetHan ---- used to nhaphang tu hoadon
    	from BH_HoaDon_ChiTiet ct
		join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    	Join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
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
    
    	declare @tblDiscountInvoice table (ID uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), NgayLapHoaDon datetime, NgayLapPhieu datetime, NgayLapPhieuThu datetime, MaHoaDon nvarchar(50),
    		DoanhThu float,
			ThucThu float,
			ChiPhiNganHang float,
			TongChiPhiNganHang float,
			ThucThu_ThucTinh float,
			HeSo float, HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, PTThucThu float, PTDoanhThu float, 
    		MaKhachHang nvarchar(max), TenKhachHang nvarchar(max), DienThoaiKH nvarchar(max), TongAll float)
    
  --  	-- bang tam chua DS phieu thu theo Ngay timkiem
		select qct.ID_HoaDonLienQuan, 
			qhd.ID,
			qhd.NgayLapHoaDon, 
			max(isnull(qct.ChiPhiNganHang,0)) as ChiPhiNganHang,
			SUM(iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu)) as ThucThu,
			---- chi get chiphi with POS
			sum(iif(qct.HinhThucThanhToan != 2,0, iif(qct.LaPTChiPhiNganHang='0', qct.ChiPhiNganHang,  qct.ChiPhiNganHang * qct.TienThu/100))) as TongChiPhiNganHang					
    	into #tempQuy
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID 
		where (qhd.TrangThai is null or qhd.TrangThai = '1')
		and qhd.ID_DonVi in (select ID from @tblChiNhanh)
		and qhd.NgayLapHoaDon >= @DateFrom
    	and qhd.NgayLapHoaDon < @DateTo 
		and qct.HinhThucThanhToan not in (4,5)
    	group by  qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID --, qct.LaPTChiPhiNganHang

		---- thucthu theo hoadon
		select ctquy.*, tblTong.TongThuThucTe
		into #tblQuyThucTe
		from #tempQuy ctquy
		left join
		(
		select ID_HoaDonLienQuan,		
			sum(ThucThu) as TongThuThucTe
		from #tempQuy
		group by ID_HoaDonLienQuan
		) tblTong on ctquy.ID_HoaDonLienQuan= tblTong.ID_HoaDonLienQuan
		
    
    		select
				tbl.ID, ---- id of hoadon
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
    			ISNULL(dt.DienThoai,'') as DienThoaiKH,		
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
    					case when TinhChietKhauTheo =1 
    						then case when LoaiHoaDon in ( 6, 32) then PT_ChietKhau else 
    							case when ISNULL(ThucThu,0)= 0 then 0  else PT_ChietKhau end end end as PTThucThu,			    				
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
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountProduct_Detail]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs nvarchar(max),
    @ID_NhomHang [nvarchar](max),
	@LaHangHoas [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @TextSearch [nvarchar](max),
    @TextSearchHangHoa [nvarchar](max),
	@TxtCustomer [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
    	set @DateTo = DATEADD(day,1,@DateTo)
    
		declare @tblLoaiHang table (LoaiHang int)
    	insert into @tblLoaiHang
    	select Name from dbo.splitstring(@LaHangHoas)

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)

    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHangHoa_XemDS_PhongBan','BCCKHangHoa_XemDS_HeThong');

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

    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    
    	DECLARE @tblSearchHH TABLE (Name [nvarchar](max));	
    INSERT INTO @tblSearchHH(Name) select  Name from [dbo].[splitstringByChar](@TextSearchHangHoa, ' ') where Name!='';
    DECLARE @countHH int =  (Select count(*) from @tblSearchHH);
    
    	declare @tblIDNhom table (ID uniqueidentifier);
    	if @ID_NhomHang='%%' OR @ID_NhomHang =''
    		begin
    			insert into @tblIDNhom
    			select ID from DM_NhomHangHoa
    		end
    	else
    		begin
    			insert into @tblIDNhom
    			select cast(Name as uniqueidentifier) from dbo.splitstring(@ID_NhomHang)
    		end;
    
    		select 
				ID_HoaDon,
				ID_ChiTietHoaDon,
				ID_DonViQuiDoi,
				ID_LoHang,
				ID_NhanVien,
				MaHoaDon, 
				LoaiHoaDon,
    			NgayLapHoaDon,
    			MaHangHoa,
    			MaNhanVien,
    			TenNhanVien,
    			TenNhomHangHoa,
    			ID_NhomHang,
    			TenHangHoa,
    			TenHangHoaFull,
    			TenDonViTinh,
    			TenLoHang,
    			ThuocTinh_GiaTri,
    			HoaHongThucHien,
    			PTThucHien,
    			HoaHongTuVan,
    			PTTuVan,
    			HoaHongBanGoiDV,
    			PTBanGoi,
    			HoaHongThucHien_TheoYC,
    			PTThucHien_TheoYC,
    			SoLuong,
    			ThanhTien,
    			HeSo,
				ThanhTien * HeSo as GtriSauHeSo,
    			ISNULL(MaDoiTuong,'') as MaKhachHang,
    			ISNULL(TenDoiTuong,N'Khách lẻ') as TenKhachHang,
    			ISNULL(dt.DienThoai,'') as DienThoaiKH,		
    		case @Status_ColumHide
    					when  1 then cast(0 as float)
    					when  2 then ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  3 then ISNULL(HoaHongBanGoiDV,0.0)
    					when  4 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  5 then ISNULL(HoaHongTuVan,0.0)
    					when  6 then ISNULL(HoaHongThucHien_TheoYC,0.0) + ISNULL(HoaHongTuVan,0.0)
    					when  7 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  8 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  9 then ISNULL(HoaHongThucHien,0.0)
    					when  10 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  11 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    					when  12 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  13 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  14 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  15 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    		else ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    		end as TongAll
			into #tblHoaHong
    		from
    		(
    				select 
							tbl.ID as ID_ChiTietHoaDon,
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
    						tbl.MaHoaDon,			
    						tbl.LoaiHoaDon,
    						tbl.NgayLapHoaDon,
    						tbl.ID_DoiTuong,
    						tbl.MaHangHoa,
    						tbl.ID_NhanVien,
    						TenHangHoa,
    						CONCAT(TenHangHoa,ThuocTinh_GiaTri) as TenHangHoaFull ,
    						TenDonViTinh,
    						ThuocTinh_GiaTri,
    						TenLoHang,
    						ID_NhomHang,
    						TenNhomHangHoa,
    						SoLuong,
    						HeSo,
    						TrangThaiHD,
							
    						tbl.GiaTriTinhCK_NotCP - iif(tbl.LoaiHoaDon=19,0,tbl.TongChiPhiDV) as ThanhTien,

    						case when LoaiHoaDon=6 then - HoaHongThucHien else HoaHongThucHien end as HoaHongThucHien,
    						case when LoaiHoaDon=6 then - PTThucHien else PTThucHien end as PTThucHien,
    						case when LoaiHoaDon=6 then - HoaHongTuVan else HoaHongTuVan end as HoaHongTuVan,
    						case when LoaiHoaDon=6 then - PTTuVan else PTTuVan end as PTTuVan,
    						case when LoaiHoaDon=6 then - PTBanGoi else PTBanGoi end as PTBanGoi,
    						case when LoaiHoaDon=6 then - HoaHongBanGoiDV else HoaHongBanGoiDV end as HoaHongBanGoiDV,
    						case when LoaiHoaDon=6 then - HoaHongThucHien_TheoYC else HoaHongThucHien_TheoYC end as HoaHongThucHien_TheoYC,
    						case when LoaiHoaDon=6 then - PTThucHien_TheoYC else PTThucHien_TheoYC end as PTThucHien_TheoYC
    				from
    				(Select 
						hdct.ID_HoaDon,
						hdct.ID,
    					hd.MaHoaDon,			
    					hd.LoaiHoaDon,
    					hd.NgayLapHoaDon,
    					hd.ID_DoiTuong,
    					dvqd.MaHangHoa,
						hdct.ID_DonViQuiDoi,
						hdct.ID_LoHang,
    					ck.ID_NhanVien,
						hdct.SoLuong,
						IIF(hdct.TenHangHoaThayThe is null or hdct.TenHangHoaThayThe='', hh.TenHangHoa, hdct.TenHangHoaThayThe) as TenHangHoa,
						iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,    					
    					ISNULL(hh.ID_NhomHang,N'00000000-0000-0000-0000-000000000000') as ID_NhomHang,
    					ISNULL(nhh.TenNhomHangHoa,N'') as TenNhomHangHoa,

						case when hh.ChiPhiTinhTheoPT =1 then hdct.SoLuong * hdct.DonGia * hh.ChiPhiThucHien/100
							else hh.ChiPhiThucHien * hdct.SoLuong end as TongChiPhiDV,

						---- gtri cthd (truoc/sau CK)
						iif(hd.LoaiHoaDon=36,0,case when iif(ck.TinhHoaHongTruocCK is null,0,ck.TinhHoaHongTruocCK) = 1 
							then hdct.SoLuong * hdct.DonGia
							else hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau)
							end) as GiaTriTinhCK_NotCP,

    					ISNULL(dvqd.TenDonVitinh,'')  as TenDonViTinh,
    					ISNULL(lh.MaLoHang,'')  as TenLoHang,
    					ck.HeSo,
    					Case when (dvqd.ThuocTinhGiaTri is null or dvqd.ThuocTinhGiaTri ='') then '' else '_' + dvqd.ThuocTinhGiaTri end as ThuocTinh_GiaTri,
    					Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    						Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTTuVan,
    						Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTBanGoi,
    					Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien_TheoYC,   				
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien_TheoYC,
    						case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    			
    																																		
    				from
    				BH_NhanVienThucHien ck
    				inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    				inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    					left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
    				left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    				Where hd.ChoThanhToan is not null
    					and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and hd.NgayLapHoaDon >= @DateFrom 
    					and hd.NgayLapHoaDon < @DateTo   							
    					and (exists (select ID from @tblNhanVien nv where ck.ID_NhanVien = nv.ID))
						and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    						and 
    						((select count(Name) from @tblSearchHH b where     									
    							 dvqd.MaHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
    							or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'	
    							)=@countHH or @countHH=0)
    			) tbl
				where tbl.LoaiHangHoa in (select LoaiHang from @tblLoaiHang)
    			) tblView
    			join NS_NhanVien nv on tblView.ID_NhanVien= nv.ID
    			left join DM_DoiTuong dt on tblView.ID_DoiTuong= dt.ID		
    			where tblView.TrangThaiHD = @StatusInvoice
    			and exists(select ID from @tblIDNhom a where ID_NhomHang= a.ID)
    			and
    				((select count(Name) from @tblSearchString b where     			
    					nv.TenNhanVien like N'%'+b.Name+'%'
    					or nv.TenNhanVienKhongDau like N'%'+b.Name+'%'
    					or nv.TenNhanVienChuCaiDau like N'%'+b.Name+'%'
    					or nv.MaNhanVien like N'%'+b.Name+'%'	
    					or tblView.MaHoaDon like '%'+b.Name+'%'							
    					)=@count or @count=0)	
				and (
				dt.MaDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong_KhongDau like N'%'+  @TxtCustomer +'%'
				or dt.DienThoai like N'%'+  @TxtCustomer +'%'
				)


				declare @TotalRow int, @TotalPage float, 
					@TongHoaHongThucHien float, @TongHoaHongThucHien_TheoYC float,
					@TongHoaHongTuVan float, @TongHoaHongBanGoiDV float,
					@TongAllAll float, @TongSoLuong float,
					@TongThanhTien float, @TongThanhTienSauHS float

				---- count all row		
				select 
					@TotalRow= count(tbl.ID_HoaDon),
    				@TotalPage= CEILING(COUNT(tbl.ID_HoaDon ) / CAST(@PageSize as float )) ,
    				@TongHoaHongThucHien= sum(HoaHongThucHien) ,
    				@TongHoaHongThucHien_TheoYC = sum(HoaHongThucHien_TheoYC),
    				@TongHoaHongTuVan = sum(HoaHongTuVan),
    				@TongHoaHongBanGoiDV = sum(HoaHongBanGoiDV),
					@TongAllAll = sum(TongAll)
				from #tblHoaHong tbl

				---- sum and group by hoadon + idquydoi
				select 
					@TongSoLuong= sum(SoLuong) ,			   				
    				@TongThanhTien = sum(ThanhTien),
					@TongThanhTienSauHS= sum(GtriSauHeSo) 
				from 
				(
					select  
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
							max(tbl.SoLuong) as SoLuong,
							max(tbl.ThanhTien) as ThanhTien,
							max(tbl.GtriSauHeSo) as GtriSauHeSo
					from #tblHoaHong tbl
					group by tbl.ID_HoaDon , tbl.ID_DonViQuiDoi ,tbl.ID_LoHang	, tbl.ID_ChiTietHoaDon	
				) tbl
				

				select tbl.*, 
					@TotalRow as TotalRow,
					@TotalPage as TotalPage,
					@TongHoaHongThucHien as TongHoaHongThucHien,
					@TongHoaHongThucHien_TheoYC as TongHoaHongThucHien_TheoYC,
					@TongHoaHongTuVan as TongHoaHongTuVan,
					@TongHoaHongBanGoiDV as TongHoaHongBanGoiDV,
					@TongAllAll as TongAllAll,
					@TongSoLuong as TongSoLuong,
					@TongThanhTien as TongThanhTien,
					@TongThanhTienSauHS as TongThanhTienSauHS
				from #tblHoaHong tbl
				order by tbl.NgayLapHoaDon desc
    			OFFSET (@CurrentPage* @PageSize) ROWS
    			FETCH NEXT @PageSize ROWS ONLY

				
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
