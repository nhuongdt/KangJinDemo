namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20200904_01 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetNS_ThietLapLuong]", parametersAction: p => new
            {
                IDChiNhanhs = p.String(),
                IDNhanViens = p.String(),
                FromDate = p.DateTime(),
                ToDate = p.DateTime()
            }, body: @"SET NOCOUNT ON;
	if @IDNhanViens	= ''
		select pc.ID_NhanVien,pc.ID, loai.TenLoaiLuong, pc.LoaiLuong, pc.SoTien,  pc.HeSo, pc.NgayApDung, pc.NgayKetThuc			
			from NS_Luong_PhuCap pc 
			left join NS_LoaiLuong loai on pc.ID_LoaiLuong= loai.ID
			where (NgayApDung <= @FromDate or  NgayApDung < @ToDate)			
			and (NgayKetThuc is null or NgayKetThuc > @FromDate)
			and exists(select Name from dbo.splitstring(@IDChiNhanhs) kieu where pc.ID_DonVi= kieu.Name)
			and pc.TrangThai!=0
	else
		select pc.ID_NhanVien,pc.ID, loai.TenLoaiLuong, pc.LoaiLuong, pc.SoTien,  pc.HeSo, pc.NgayApDung, pc.NgayKetThuc			
			from NS_Luong_PhuCap pc 
			left join NS_LoaiLuong loai on pc.ID_LoaiLuong= loai.ID
			where (NgayApDung <= @FromDate or  NgayApDung < @ToDate)			
			and (NgayKetThuc is null or NgayKetThuc > @FromDate)
			and exists(select Name from dbo.splitstring(@IDChiNhanhs) kieu where pc.ID_DonVi= kieu.Name)
			and pc.TrangThai!=0
			and exists (select Name from dbo.splitstring(@IDNhanViens) nv where pc.ID_NhanVien= nv.Name)");

			CreateStoredProcedure(name: "[dbo].[TinhLuongNhanVien]", parametersAction: p => new
			{
				IDChiNhanhs = p.Guid(),
				IDNhanViens = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				KieuLuongs = p.String(20),
				CurrentPage = p.Int(),
				PageSize = p.Double()
			}, body: @"SET NOCOUNT ON;			

		-- used to get report discount
		declare @IDNhanVienLogin uniqueidentifier= (select top 1 ID_NhanVien from HT_NguoiDung where LaAdmin='1')

		declare @ngaycongchuan float = (select dbo.TinhNgayCongChuan(@FromDate,@ToDate,@IDChiNhanhs))
		
		declare @tblCong CongThuCong
		insert into @tblCong
		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		declare @tblThietLapLuong ThietLapLuong
		insert into @tblThietLapLuong
		exec GetNS_ThietLapLuong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		declare @tblLuong table (LoaiLuong int, ID_NhanVien uniqueidentifier, LuongCoBan float, SoNgayDiLam float, LuongChinh float)				
		insert into @tblLuong		
		exec TinhLuongCoBan @ngaycongchuan, @tblCong, @tblThietLapLuong

		declare @tblLuongOT table (ID_NhanVien uniqueidentifier, LuongOT float)				
		insert into @tblLuongOT		
		exec TinhLuongOT @ngaycongchuan, @tblCong, @tblThietLapLuong
		
		declare @tblPhuCap table (ID_NhanVien uniqueidentifier, PhuCapCoDinh float, PhuCapTheoNgayCong float)
		insert into @tblPhuCap
		exec TinhPhuCapLuong @tblCong, @tblThietLapLuong

		declare @tblPhuCapTheoPtram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  PhuCapCoDinh float, HeSo float,
		NgayApDung datetime, NgayKetThuc datetime, SoNgayDiLam float)
		insert into @tblPhuCapTheoPtram
		exec GetPhuCapCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%',@tblCong, @tblThietLapLuong

		declare @tblGiamTru table (ID_NhanVien uniqueidentifier, GiamTruCoDinhVND float, GiamTruTheoLan float, SoLanDiMuon float)
		insert into @tblGiamTru
		exec TinhGiamTruLuong @tblCong, @tblThietLapLuong	

		-- get phucap codinh theo %luongchinh
	declare @tblLuongPC table (ID_NhanVien uniqueidentifier,LoaiLuong int, LuongCoBan float, SoNgayDiLam float, LuongChinh float,PhuCapCoDinh_TheoPtramLuong float)						
	insert into @tblLuongPC	
	select 
		pcluong.ID_NhanVien, pcluong.LoaiLuong, pcluong.LuongCoBan, pcluong.SoNgayDiLam, pcluong.LuongChinh, 
		sum(PhuCapCoDinh_TheoPtramLuong) as PhuCapCoDinh_TheoPtramLuong
	from
		(select luong.ID_NhanVien, LoaiLuong, LuongCoBan, luong.SoNgayDiLam, LuongChinh,
			case when PhuCapCoDinh is null then 0 else LuongChinh * PhuCapCoDinh * HeSo/100 end as PhuCapCoDinh_TheoPtramLuong
		from @tblLuong luong
		left join @tblPhuCapTheoPtram pc on luong.ID_NhanVien= pc.ID_NhanVien
		) pcluong 
		group by pcluong.ID_NhanVien, pcluong.LuongChinh, pcluong.LoaiLuong, pcluong.LuongCoBan,pcluong.SoNgayDiLam

		--select * from  @tblLuongPC


	-- hoahong		
	DECLARE @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
	Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float) 
	INSERT INTO @tab_DoanhSo exec getList_ChietKhauNhanVienTheoDoanhSo @IDChiNhanhs,@IDNhanVienLogin, '', @FromDate, @ToDate,'%%',0,1000;

	DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
	TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)
	INSERT INTO @tab_HoaDon exec SP_ReportDiscountInvoice @IDChiNhanhs,@IDNhanVienLogin,'%%', @FromDate, @ToDate, 8,1,0,0,100

	DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
	TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, TongHoaHongBanGoiDV float, TongAll float)
	INSERT INTO @tab_HangHoa exec SP_ReportDiscountProduct_General @IDChiNhanhs, @IDNhanVienLogin,'%%', @FromDate, @ToDate, 16,1,0,100

	declare @tblHoaHong table (ID_NhanVien uniqueidentifier, TongDoanhThu float, HoaHong float, HoaHongHangHoa float, HoaHongHoaDon float, HoaHongDoanhThu float)
	insert into  @tblHoaHong
	SELECT a.ID_NhanVien, sum(TongDoanhThu) as TongDoanhThu,		
		SUM(TongDoanhSo + TongHoaDon + TongHangHoa) as HoaHong,
		SUM(TongHangHoa) as HoaHongHangHoa,
		SUM(TongHoaDon) as HoaHongHoaDon,
		SUM(TongDoanhSo) as HoaHongDoanhThu
	FROM 
	(
		select ID_NhanVien,	Tong as TongHangHoa,0 as TongHoaDon, 0 as TongDoanhSo, 0 as TongDoanhThu					
		from @tab_HangHoa
		UNION ALL
		Select ID_NhanVien,	0 as TongHangHoa,TongAll as TongHoaDon,	0 as TongDoanhSo, 0 as TongDoanhThu						
		from @tab_HoaDon
		UNION ALL
		Select ID_NhanVien,	0 as TongHangHoa,0 as TongHoaDon,TongAll as TongDoanhSo, TongDoanhThu
		from @tab_DoanhSo
	) as a
	GROUP BY a.ID_NhanVien

	-- giamtru codinh %tongluongnhan
	declare @tblGiamTruTheoPTram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  GiamTruCoDinh float, HeSo float,
		NgayApDung datetime, NgayKetThuc datetime, SoLanDiMuon float)
	insert into @tblGiamTruTheoPTram
	exec GetGiamTruCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%',@tblCong, @tblThietLapLuong	

	select nv.MaNhanVien, nv.TenNhanVien, 
			luong.*,
			cast(PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong as float) as PhuCap,
			cast(PhatDiMuon + GiamTruCoDinhVND as float) as TongTienPhat,
			cast(LuongChinh as float)  as TongLuongNhan, -- save to DB
			cast(LuongChinh + LuongOT +  PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong + KhenThuong + ChietKhau - PhatDiMuon - GiamTruCoDinhVND as float) as LuongThucNhan
	into #tblluong
		from
			(
			select 
				tbl.ID_NhanVien, 
				max(tbl.LoaiLuong) as LoaiLuong,
				max(tbl.LuongCoBan) as LuongCoBan,
				sum(tbl.LuongChinh) as LuongChinh,
				sum(LuongOT) as LuongOT,	
				sum(PhuCapCoDinh_TheoPtramLuong) as PhuCapCoDinh_TheoPtramLuong,				
				sum(PhuCapCoDinh) as PhuCapCoBan,
				sum(PhuCapTheoNgay) as PhuCapKhac,
				sum(GiamTruCoDinhVND) as GiamTruCoDinhVND,
				sum(GiamTruTheoLan) as PhatDiMuon,
				sum(HoaHong) as ChietKhau,
				sum(HoaHongHangHoa) as HoaHongHangHoa,
				sum(HoaHongHoaDon) as HoaHongHoaDon,
				sum(HoaHongDoanhThu) as HoaHongDoanhThu,
				sum(TongDoanhThu) as TongDoanhThu,
				sum(SoNgayDiLam) as NgayCongThuc,
				sum(SoGioOT) as SoGioOT,
				sum(SoLanDiMuon) as SoLanDiMuon,
				sum(KhenThuong) as KhenThuong,
				@ngaycongchuan as  NgayCongChuan
			from 
				(select 
					ID_NhanVien, LoaiLuong, LuongCoBan, LuongChinh,
					cast(0 as float) as LuongOT, 
					SoNgayDiLam, cast(0 as float) as SoGioOT, 
					PhuCapCoDinh_TheoPtramLuong,
					cast(0 as float) as PhuCapCoDinh, cast(0 as float) as PhuCapTheoNgay,
					cast(0 as float) as GiamTruCoDinhVND, cast(0 as float) as GiamTruTheoLan, cast(0 as float) as SoLanDiMuon,
					cast(0 as float) as HoaHong,cast(0 as float) as HoaHongHangHoa, cast(0 as float) as HoaHongHoaDon, cast(0 as float) as HoaHongDoanhThu, 0 as TongDoanhThu,
					cast(0 as float) as KhenThuong
				from @tblLuongPC

				union all

				select 
					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan, 0 as LuongChinh,
					LuongOT,
					0 as SoNgayDiLam,0 as SoGioOT,
					0 as PhuCapCoDinh_TheoPtramLuong,
					0 as PhuCapCoDinh, 0 as PhuCapTheoNgay,
					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
					0 as HoaHong, 0 as HoaHongHangHoa,0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
					0 as KhenThuong
				from @tblLuongOT

				union all
				select 
					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan, 0 as LuongChinh, 0 as LuongOT, 
					0 as SoNgayDiLam, 0 as SoGioOT,
					0 as PhuCapCoDinh_TheoPtramLuong,
					PhuCapCoDinh, PhuCapTheoNgayCong,
					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
					0 as KhenThuong
				from @tblPhuCap

				union all
				select 
					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
					0 as SoNgayDiLam, 0 as SoGioOT,
					0 as PhuCapCoDinh_TheoPtramLuong,
					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
					GiamTruCoDinhVND, GiamTruTheoLan, SoLanDiMuon,
					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu, 0 as TongDoanhThu,
					0 as KhenThuong
				from @tblGiamTru

				union all
				select 
					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
					0 as SoNgayDiLam, 0 as SoGioOT, 
					0 as PhuCapCoDinh_TheoPtramLuong,
					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
					HoaHong, HoaHongHangHoa, HoaHongHoaDon, HoaHongDoanhThu, TongDoanhThu,
					0 as KhenThuong
				from @tblHoaHong
				) tbl group by tbl.ID_NhanVien
			) luong
		join NS_NhanVien nv on luong.ID_NhanVien= nv.ID
		where LuongChinh > 0 OR LuongOT> 0
		
		if @IDNhanViens='' or 	@IDNhanViens='%%'	
			select luong.* ,
				ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as GiamTruCoDinh_TheoPTram
			from #tblluong luong
			left join @tblGiamTruTheoPTram gt on luong.ID_NhanVien= gt.ID_NhanVien
			where exists(select Name from dbo.splitstring(@KieuLuongs) kl where luong.LoaiLuong= kl.Name)
			order by luong.MaNhanVien
		else
			-- search by nhanvien
			select luong.* ,
				ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as GiamTruCoDinh_TheoPTram
			from #tblluong luong
			left join @tblGiamTruTheoPTram gt on luong.ID_NhanVien= gt.ID_NhanVien
			where exists(select Name from dbo.splitstring(@KieuLuongs) kl where luong.LoaiLuong= kl.Name)
			and exists(select Name from dbo.splitstring(@IDNhanViens) nv where luong.ID_NhanVien= nv.Name)
			order by luong.MaNhanVien");

			Sql(@"ALTER PROCEDURE [dbo].[importNS_NhanVien_DanhSach]
    @MaNhanVien [nvarchar](max),
    @TenNhanVien [nvarchar](max),
    @TenNhanVienKhongDau [nvarchar](max),
    @TenNhanVienKyTuDau [nvarchar](max),
    @GioiTinh [bit],
    @NgaySinh [nvarchar](max),
    @DienThoai [nvarchar](max),
    @Email [nvarchar](max),
    @NoiSinh [nvarchar](max),
    @CMND [nvarchar](max),
    @SoBaoHiem [nvarchar](max),
    @GhiChu [nvarchar](max),
    @TrangThai [bit],
	@ID_DonVi [uniqueidentifier]
AS
BEGIN
	DECLARE @ID_PhongBan uniqueidentifier
    	Set @ID_PhongBan = (select TOP 1 ID from NS_PhongBan where ID_DonVi is NULL)
	DECLARE @ID_NhanVien uniqueidentifier
		Set @ID_NhanVien = NEWID();
    insert into NS_NhanVien(ID, MaNhanVien, TenNhanVien, TenNhanVienKhongDau,TenNhanVienChuCaiDau, GioiTinh, NgaySinh, DienThoaiDiDong, Email, NoiSinh,NguyenQuan, SoCMND,SoBHXH, GhiChu, NguoiTao,NgayTao, DaNghiViec, TrangThai)
    values(@ID_NhanVien, @MaNhanVien, @TenNhanVien, @TenNhanVienKhongDau,@TenNhanVienKyTuDau, @GioiTinh, @NgaySinh, @DienThoai, @Email, @NoiSinh, @NoiSinh, @CMND,@SoBaoHiem, @GhiChu, 'admin',GETDATE(), @TrangThai,'1');

	--- insert NS_QuaTrinhCongTac with current ChiNhanh + phongban macdinh
	insert into NS_QuaTrinhCongTac (ID, ID_NhanVien, ID_DonVi, NgayApDung, LaChucVuHienThoi, LaDonViHienThoi, ID_PhongBan)
			values (NEWID(), @ID_NhanVien, @ID_DonVi, GETDATE(), '0', '1', @ID_PhongBan)

END");

			Sql(@"ALTER FUNCTION [dbo].[check_MaNhanVien](@inputVar NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN    
	DECLARE @MaNhanVien NVARCHAR(MAX);
	DECLARE @tab table (MaNhanVien int, SoThuTu int)
	insert into @tab
	Select RIGHT(MaNhanVien, 5), ROW_NUMBER() over(order by MaNhanVien) as MaNhanVien from NS_NhanVien
	WHERE LEN(MaNhanVien)>6 and (TrangThai = 1 OR TrangThai is null) and LEFT(MaNhanVien, 2) = LEFT(@inputVar, 2) AND ISNUMERIC(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 1 AND LEN(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 5
	order by MaNhanVien
	SELECT @MaNhanVien = (select TOP 1 SoThuTu + 1 from @tab
	--where MaNhanVien != SoThuTu
	order by MaNhanVien desc)
	IF (LEN(@MaNhanVien) > 0)
	BEGIN
			SELECT @MaNhanVien = CASE
			WHEN @MaNhanVien > 0 and @MaNhanVien <= 9 THEN 'NV0000' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 9 and @MaNhanVien <= 99 THEN 'NV000' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 99 and @MaNhanVien <= 999 THEN 'NV00' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 999 and @MaNhanVien <= 9999 THEN 'NV0' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien > 9999 THEN 'NV' + CONVERT(CHAR, @MaNhanVien)
			END
	END
	else
	BEgin
	 SELECT @MaNhanVien = (Select Max(SoThuTu) +1 from @tab)
		IF (LEN(@MaNhanVien) > 0)
		BEGIN
				SELECT @MaNhanVien = CASE
				WHEN @MaNhanVien > 0 and @MaNhanVien <= 9 THEN 'NV0000' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 9 and @MaNhanVien <= 99 THEN 'NV000' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 99 and @MaNhanVien <= 999 THEN 'NV00' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 999 and @MaNhanVien <= 9999 THEN 'NV0' + CONVERT(CHAR, @MaNhanVien)
				WHEN @MaNhanVien > 9999 THEN 'NV' + CONVERT(CHAR, @MaNhanVien)
				END
		END
		ELSE
		BEGIN
			SELECT @MaNhanVien = @inputVar;
		END
	END
	RETURN @MaNhanVien
END");

			Sql(@"IF NOT EXISTS (SELECT * FROM DM_LoaiChungTu where ID = '24')
BEGIN
	insert into DM_LoaiChungTu values (24,'BL',N'Bảng lương','','ssoftvn',GETDATE(),null,null)
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetNS_ThietLapLuong]");
            DropStoredProcedure("[dbo].[TinhLuongNhanVien]");
        }
    }
}
