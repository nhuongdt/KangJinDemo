namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180714 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetDonVi_byUserSeach]", parametersAction: p => new
            {
                ID_NguoiDung = p.Guid(),
                TenDonVi = p.String()
            }, body: @"Select 
    	dv.ID,
    	dv.TenDonVi,
    	dv.SoDienThoai
    	From Dm_DonVi dv
    	inner join NS_QuaTrinhCongTac qtct on dv.ID = qtct.ID_DonVi
    	inner join NS_NhanVien nv on qtct.ID_NhanVien = nv.ID
    	inner join HT_NguoiDung nd on nv.ID = nd.ID_NhanVien	
		inner join HT_NguoiDung_Nhom ndn on nd.ID = ndn.IDNguoiDung and dv.ID = ndn.ID_DonVi
    	where nd.ID = @ID_NguoiDung and (dv.TenDonVi like @TenDonVi or dv.SoDienThoai like @TenDonVi)
    		and (dv.TrangThai = 1 or dv.TrangThai is null)
    	Order by dv.TenDonVi");

            CreateStoredProcedure(name: "[dbo].[getList_SoSanhCungKyHoaDon]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"SELECT 
	CAST(ROUND(( CAST(b.DoanhThuThangNay - b.DoanhThuThangTruoc as float) / b.DoanhThuThangTruoc) * 100, 2) as float) as SoSanhCungKy
	FROM
	(
	SELECT 
	Sum(ISNULL(a.DoanhThuThangNay, 0) - ISNULL(a.TraHangThangNay, 0)) as DoanhThuThangNay,
	Sum(ISNULL(a.DoanhThuThangTruoc, 0) - ISNULL(a.TraHangThangTruoc, 0)) as DoanhThuThangTruoc
	FROM
	(
	select
	Sum(hdct.ThanhTien) as DoanhThuThangNay,
	NULL as DoanhThuThangTruoc,
	NULL as TraHangThangNay,
	NULL as TraHangThangTruoc,
	1 as GR
	from BH_HoaDon hd 
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	where hd.ChoThanhToan = 0 and hd.LoaiHoaDon = 1 and hd.ID_DonVi = @ID_ChiNhanh
	and NgayLapHoaDon >= @timeStart and NgayLapHoaDon < @timeEnd
	Group by DAY(hd.NgayLapHoaDon)
	union all
	select
	NULL as DoanhThuThangNay,
	NULL as DoanhThuThangTruoc,
	Sum(hdct.ThanhTien) as TraHangThangNay,
	NULL as TraHangThangTruoc,
	1 as GR
	from BH_HoaDon hd 
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	where hd.ChoThanhToan = 0 and hd.LoaiHoaDon = 6 and hd.ID_DonVi = @ID_ChiNhanh
	and NgayLapHoaDon >= @timeStart and NgayLapHoaDon < @timeEnd
	Group by DAY(hd.NgayLapHoaDon)
	union all
	select
	NULL as DoanhThuThangNay,
	Sum(hdct.ThanhTien) as DoanhThuThangTruoc,
	NULL as TraHangThangNay,
	NULL as TraHangThangTruoc,
	1 as GR
	from BH_HoaDon hd 
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	where hd.ChoThanhToan = 0 and hd.LoaiHoaDon = 1 and hd.ID_DonVi = @ID_ChiNhanh
	and NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
	Group by DAY(hd.NgayLapHoaDon)
	union all
	select
	NULL as DoanhThuThangNay,
	NULL as DoanhThuThangTruoc,
	NULL as TraHangThangNay,
	Sum(hdct.ThanhTien) as TraHangThangTruoc,
	1 as GR
	from BH_HoaDon hd 
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	where hd.ChoThanhToan = 0 and hd.LoaiHoaDon = 6 and hd.ID_DonVi = @ID_ChiNhanh
	and NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
	Group by DAY(hd.NgayLapHoaDon)
	) as a
	Group by a.GR
	) as b");

            CreateStoredProcedure(name: "[dbo].[getList_SoSanhCungKyKhachHang]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"SELECT
	CAST(ROUND((CAST(m.KhachHangQuayLaiThangNay + m.KhachHangTaoMoiThangNay - m.KhachHangQuayLaiThangTruoc - m.KhachHangTaoMoiThangTruoc as float)) / (m.KhachHangQuayLaiThangTruoc + m.KhachHangTaoMoiThangTruoc) *100, 2) as float) as SoSanhCungKy
	FROM
	(
	SELECT
	SUM(ISNULl(k.KhachHangTaoMoiThangNay, 0)) as KhachHangTaoMoiThangNay,
	SUM(ISNULl(k.KhachHangQuayLaiThangNay, 0)) as KhachHangQuayLaiThangNay,
	SUM(ISNULl(k.KhachHangTaoMoiThangTruoc, 0)) as KhachHangTaoMoiThangTruoc,
	SUM(ISNULl(k.KhachHangQuayLaiThangTruoc, 0)) as KhachHangQuayLaiThangTruoc
	FROM
	(
   -- khach hàng mới
	select 
	1 as GR,
	Count (*) as KhachHangTaoMoiThangNay,
	NULL as KhachHangQuayLaiThangNay,
	NULL as KhachHangTaoMoiThangTruoc,
	NULL as KhachHangQuayLaiThangTruoc
	from
	DM_DoiTuong dt 
	where (dt.TheoDoi is null or dt.TheoDoi = 0)
	and dt.LoaiDoiTuong = 1
	and dt.ID_DonVi = @ID_ChiNhanh
	and dt.NgayTao >= @timeStart and dt.NgayTao < @timeEnd
	-- khách hàng quay lại
	union all
	select 
	1 as GR,
	NULL as KhachHangTaoMoiThangNay,
	COUNT (*) as KhachHangQuayLaiThangNay,
	NULL as KhachHangTaoMoiThangTruoc,
	NULL as KhachHangQuayLaiThangTruoc
	from 
	(
		select DISTINCT dt.ID
		from BH_HoaDon hdt 
		inner join BH_HoaDon hds on hdt.ID_DoiTuong = hds.ID_DoiTuong
		inner join DM_DoiTuong dt on hdt.ID_DoiTuong = dt.ID
		where hdt.NgayLapHoaDon < @timeStart
		and hds.NgayLapHoaDon >= @timeStart and hds.NgayLapHoaDon < @timeEnd
		and (dt.TheoDoi is null or dt.TheoDoi = 0)
		and dt.LoaiDoiTuong = 1
		and  hdt.ChoThanhToan = 0 
		and hdt.LoaiHoaDon = 1
		and hds.LoaiHoaDon = 1
		and hds.ChoThanhToan = 0
		and hdt.ID_DonVi = @ID_ChiNhanh
		and hds.ID_DonVi = @ID_ChiNhanh
	) as a
	-- tháng trước
	union all
	select
	1 as GR,
	NULL as KhachHangTaoMoiThangNay,
	NULL as KhachHangQuayLaiThangNay,
	Count (*) as KhachHangTaoMoiThangTruoc,
	NULL as KhachHangQuayLaiThangTruoc
	from
	DM_DoiTuong dt 
	where (dt.TheoDoi is null or dt.TheoDoi = 0)
	and dt.LoaiDoiTuong = 1
	and dt.ID_DonVi = @ID_ChiNhanh
	and dt.NgayTao >= DateAdd(month, -1, @timeStart) and dt.NgayTao < DateAdd(month, -1, @timeEnd) 
	-- khách hàng quay lại
	union all
	select 
	1 as GR,
	NULL as KhachHangTaoMoiThangNay,
	NULL as KhachHangQuayLaiThangNay,
	NULL as KhachHangTaoMoiThangTruoc,
	Count (*) as KhachHangQuayLaiThangTruoc
	from 
	(
		select DISTINCT dt.ID
		from BH_HoaDon hdt 
		inner join BH_HoaDon hds on hdt.ID_DoiTuong = hds.ID_DoiTuong
		inner join DM_DoiTuong dt on hdt.ID_DoiTuong = dt.ID
		where hdt.NgayLapHoaDon <  DateAdd(month, -1, @timeEnd)
		and hds.NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and hds.NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
		and (dt.TheoDoi is null or dt.TheoDoi = 0)
		and dt.LoaiDoiTuong = 1
		and  hdt.ChoThanhToan = 0 
		and hdt.LoaiHoaDon = 1
		and hds.LoaiHoaDon = 1
		and hds.ChoThanhToan = 0
		and hdt.ID_DonVi = @ID_ChiNhanh
		and hds.ID_DonVi = @ID_ChiNhanh
	) as b
	) as k
	GROUP BY k.GR
	) as m");

            CreateStoredProcedure(name: "[dbo].[getList_TongQuanKhachHang]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"-- khach hàng mới
	SELECT
	CAST(ROUND(SUM(ISNULL(k.KhachHangTaoMoiThangNay, 0)), 0) as float) as KhachHangTaoMoiThangNay,
	CAST(ROUND(SUM(ISNULL(k.KhachHangQuayLaiThangNay, 0)), 0) as float) as KhachHangQuayLaiThangNay
	FROM
	(
	select 
	1 as GR,
	Count (*) as KhachHangTaoMoiThangNay,
	NULL as KhachHangQuayLaiThangNay
	from
	DM_DoiTuong dt 
	where (dt.TheoDoi is null or dt.TheoDoi = 0)
	and dt.LoaiDoiTuong = 1
	and dt.ID_DonVi = @ID_ChiNhanh
	and dt.NgayTao >= @timeStart and dt.NgayTao < @timeEnd
	-- khách hàng quay lại
	union all
	select 
	1 as GR,
	NULL as KhachHangTaoMoiThangNay,
	COUNT (*) as KhachHangQuayLaiThangNay
	from 
	(
		select DISTINCT dt.ID
		from BH_HoaDon hdt 
		inner join BH_HoaDon hds on hdt.ID_DoiTuong = hds.ID_DoiTuong
		inner join DM_DoiTuong dt on hdt.ID_DoiTuong = dt.ID
		where hdt.NgayLapHoaDon < @timeStart
		and hds.NgayLapHoaDon >= @timeStart and hds.NgayLapHoaDon < @timeEnd
		and (dt.TheoDoi is null or dt.TheoDoi = 0)
		and dt.LoaiDoiTuong = 1
		and  hdt.ChoThanhToan = 0 
		and hdt.LoaiHoaDon = 1
		and hds.LoaiHoaDon = 1
		and hds.ChoThanhToan = 0
		and hdt.ID_DonVi = @ID_ChiNhanh
		and hds.ID_DonVi = @ID_ChiNhanh
	) as a
	) as k
	GROUP BY k.GR");

            CreateStoredProcedure(name: "[dbo].[import_DanhMucHangHoa_Update]", parametersAction: p => new
            {
                LoaiUpdate = p.Int(),
                ID_HangHoa = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                GiaTriTang = p.String(),
                GiaTriGiam = p.String(),
                TongTienLech = p.String(),
                TongChenhLech = p.String(),
                SoLuongThucTe = p.String(),
                SoLuongTang = p.String(),
                SoLuongGiam = p.String(),
                TenNhomHangHoaCha = p.String(),
                MaNhomHangHoaCha = p.String(),
                timeCreateNHHCha = p.DateTime(),
                TenNhomHangHoa = p.String(),
                MaNhomHangHoa = p.String(),
                timeCreateNHH = p.DateTime(),
                LaHangHoa = p.Boolean(),
                timeCreateHH = p.DateTime(),
                TenHangHoa = p.String(),
                TenHangHoa_KhongDau = p.String(),
                TenHangHoa_KyTuDau = p.String(),
                GhiChu = p.String(),
                QuyCach = p.String(),
                DuocBanTrucTiep = p.Boolean(),
                MaDonViCoBan = p.String(),
                MaHangHoa = p.String(),
                TenDonViTinh = p.String(),
                GiaVon = p.String(),
                GiaBan = p.String(),
                timeCreateDVQD = p.DateTime(),
                LaDonViChuan = p.Boolean(),
                TyLeChuyenDoi = p.String(),
                MaHoaDon = p.String(),
                DienGiai = p.String(),
                TonKho = p.String(),
                timeCreateHD = p.DateTime(),
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                MaHangHoaChaCungLoai = p.String()
            }, body: @"-- Khai báo biến update
    	DECLARE @GiaVonF as float
    		set @GiaVonF = (select CAST(ROUND(@GiaVon, 2) as float))
    	DECLARE @GiaBanF as float
    		set @GiaBanF = (select CAST(ROUND(@GiaBan, 2) as float))
    	DECLARE @QuyCachF as float
    		set @QuyCachF = (select CAST(ROUND(@QuyCach, 2) as float))
    	DECLARE @TyLeChuyenDoiF as float
    		set @TyLeChuyenDoiF = (select CAST(ROUND(@TyLeChuyenDoi, 2) as float))
    	DECLARE @TonKhoF as float
    		set @TonKhoF = (select CAST(ROUND(@TonKho, 2) as float))

		DECLARE @GiaTriTangF as float
    		set @GiaTriTangF = (select CAST(ROUND(@GiaTriTang, 0) as float))
		DECLARE @GiaTriGiamF as float
    		set @GiaTriGiamF = (select CAST(ROUND(@GiaTriGiam, 0) as float))
		DECLARE @TongTienLechF as float
    		set @TongTienLechF = (select CAST(ROUND(@TongTienLech, 0) as float))
		DECLARE @TongChenhLechF as float
    		set @TongChenhLechF = (select CAST(ROUND(@TongChenhLech, 2) as float))
		DECLARE @SoLuongTangF as float
    		set @SoLuongTangF = (select CAST(ROUND(@SoLuongTang, 2) as float))
		DECLARE @SoLuongGiamF as float
    		set @SoLuongGiamF = (select CAST(ROUND(@SoLuongGiam, 2) as float))
		DECLARE @SoLuongThucTeF as float
    		set @SoLuongThucTeF = (select CAST(ROUND(@SoLuongThucTe, 2) as float))
	-- insert NhomHangHoa parent
    DECLARE @ID_NhomHangHoaCha  as uniqueidentifier
    	set @ID_NhomHangHoaCha = null
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin
    		SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha, @MaNhomHangHoaCha, '1', '1', '1', '1', 'admin', @timeCreateNHHCha, null)
    		End
    	End
    -- insert NhomHangHoa
    	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
    	if (len(@TenNhomHangHoa) > 0)
    	Begin
    		SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @MaNhomHangHoa, '1', '1', '1', '1', 'admin', @timeCreateNHH, @ID_NhomHangHoaCha)
    		End
    	End
    -- Update HangHoa
    		DECLARE @LaChaCungLoai  as int
    		set @LaChaCungLoai = 1;
    		DECLARE @ID_HangHoaCungLoai  as uniqueidentifier
    	set @ID_HangHoaCungLoai = newID();
    			if(len(@MaHangHoaChaCungLoai) > 0) -- nếu có thông tin mã hàng hóa cùng loại
    			Begin 
    				set @ID_HangHoaCungLoai = (select ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'); -- lấy ID_HangHoaCungLoai có chung tên khác
    				if (len(@ID_HangHoaCungLoai) > 0) 
    				Begin
    					set @LaChaCungLoai = 0;
    				End
    				else
    				Begin 
    					set @ID_HangHoaCungLoai = newID(); 
    				End
    			End
		-- update HangHoa
    	if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    	Begin
			update DM_HangHoa set ID_HangHoaCungLoai = @ID_HangHoaCungLoai, LaChaCungLoai = @LaChaCungLoai, ID_NhomHang = @ID_NhomHangHoa, LaHangHoa = @LaHangHoa,
					NgaySua = @timeCreateHH, NguoiSua='admin', TenHangHoa = @TenHangHoa, TenHangHoa_KhongDau = @TenHangHoa_KhongDau, TenHangHoa_KyTuDau = @TenHangHoa_KyTuDau,
					TenKhac = @MaHangHoaChaCungLoai, GhiChu = @GhiChu, QuyCach = @QuyCachF, DuocBanTrucTiep = @DuocBanTrucTiep Where ID = @ID_HangHoa
    	end
    -- update DonViQuiDoi
		update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoiF, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBanF, NguoiSua ='admin', NgaySua =@timeCreateDVQD
			Where ID = @ID_DonViQuiDoi
    -- insert kiểm kê TonKho
    	if (@LoaiUpdate = 2 and @LaHangHoa = 1)
    	Begin
    	DECLARE @ID_HoaDon  as uniqueidentifier
    	set @ID_HoaDon = newID();
    		insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    			values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, @timeCreateHD, @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    	    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi)
    			Values (NEWID(), @ID_HoaDon, '1', @TongChenhLechF, '0', @SoLuongThucTeF, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi)
    	End");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetDonVi_byUserSeach]");
            DropStoredProcedure("[dbo].[getList_SoSanhCungKyHoaDon]");
            DropStoredProcedure("[dbo].[getList_SoSanhCungKyKhachHang]");
            DropStoredProcedure("[dbo].[getList_TongQuanKhachHang]");
            DropStoredProcedure("[dbo].[import_DanhMucHangHoa_Update]");
        }
    }
}