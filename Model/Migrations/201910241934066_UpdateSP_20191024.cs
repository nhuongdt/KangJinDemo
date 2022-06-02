namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20191024 : DbMigration
    {
        public override void Up()
        {
         
            CreateStoredProcedure(name: "[dbo].[GetHoaDonDatHang_afterXuLy]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                txtSearch = p.String(),
                CurrentPage = p.Int(),
                PageSize = p.Int()
            }, body: @"set nocount on;

    SELECT 
    	c.ID,
    	c.MaHoaDon,
    	c.LoaiHoaDon,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
    	c.DienThoai,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
    	c.ID_BangGia,
    	c.ID_DonVi,
    	c.YeuCau,
		'' as MaHoaDonGoc,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	ISNULL(c.NguoiTaoHD,'') as NguoiTaoHD,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TongTienHang, 
		c.TongGiamGia, 
		c.PhaiThanhToan, 
		c.KhachDaTra,
		c.TongChietKhau,
		c.TongTienThue, 
    	c.TrangThai,
    	c.HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon
    
    	FROM
    	(
    		select 
    		a.ID as ID,
    		bhhd.MaHoaDon,
    		hdXMLOut.HoaDon_HangHoa,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_NhanVien,
    		ISNULL(bhhd.ID_DoiTuong,'00000000-0000-0000-0000-000000000000') as ID_DoiTuong,
    		bhhd.ID_BangGia,
    		bhhd.NgayLapHoaDon,
    		bhhd.YeuCau,
    		bhhd.ID_DonVi,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
			CAST(ROUND(bhhd.TongTienThue, 0) as float) as TongTienThue,
    		a.KhachDaTra,
    		bhhd.TongChietKhau,
			bhhd.ChoThanhToan,
    		Case When bhhd.YeuCau = '1' then N'Phiếu tạm' when bhhd.YeuCau = '3' then N'Hoàn thành' when bhhd.YeuCau = '2' then N'Đang giao hàng' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			select 
    			b.ID,
    			SUM(ISNULL(b.KhachDaTra, 0)) as KhachDaTra
    			from
    			(
					-- get infor PhieuThu from HDDatHang (HuyPhieuThu (qhd.TrangThai ='0')
    				Select 
    					bhhd.ID,						
    					Case when qhd.TrangThai='0' then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(hdct.Tienthu, 0) else -ISNULL(hdct.Tienthu, 0) end end as KhachDaTra,
						0 as SoLuongBan,
						0 as SoLuongTra				
   					from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID 				
    				where bhhd.LoaiHoaDon = '3' and bhhd.ChoThanhToan is not null
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi = @ID_ChiNhanh  
    
    				union all
					-- get infor PhieuThu/Chi from HDXuLy
    				Select
    					hdt.ID,						
  						Case when bhhd.ChoThanhToan is null or qhd.TrangThai='0' then (Case when qhd.LoaiHoaDon = 11 or qhd.TrangThai='0' then 0 else -ISNULL(hdct.Tienthu, 0) end)
    						else (Case when qhd.LoaiHoaDon = 11 then ISNULL(hdct.Tienthu, 0) else -ISNULL(hdct.Tienthu, 0) end) end as KhachDaTra,
						0 as SoLuongBan,
						0 as SoLuongTra
    				from BH_HoaDon bhhd
    				inner join BH_HoaDon hdt on (bhhd.ID_HoaDon = hdt.ID and hdt.ChoThanhToan = '0')
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
    				left join Quy_HoaDon qhd on (hdct.ID_HoaDon = qhd.ID)
    				where hdt.LoaiHoaDon = '3' 
					and bhhd.ChoThanhToan='0'
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi = @ID_ChiNhanh					
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
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
		where c.NgayLapHoadon >= @timeStart and c.NgayLapHoaDon < @timeEnd
		and c.LoaiHoaDon = 3 and c.YeuCau in (1,2) and c.ChoThanhToan is not null		
    	ORDER BY c.NgayLapHoaDon DESC");

            Sql(@"CREATE FUNCTION [dbo].[check_MaNhanSu](@inputVar NVARCHAR(MAX), @LoaiMa int)
RETURNS NVARCHAR(MAX)
AS
BEGIN    
	DECLARE @Ma NVARCHAR(MAX);
	DECLARE @tab table (Ma int, SoThuTu int)
	if (@LoaiMa = 1)
	BEGIN
		insert into @tab
		Select RIGHT(MaCa, 5) as Ma, ROW_NUMBER() over(order by MaCa) as SoThuTu from NS_CaLamViec
		WHERE LEN(MaCa)>6 and  TrangThai!=0 and LEFT(MaCa, 2) = LEFT(@inputVar, 2) AND ISNUMERIC(SUBSTRING(MaCa, 3, LEN(MaCa) - 2)) = 1 AND LEN(SUBSTRING(MaCa, 3, LEN(MaCa) - 2)) = 5
		order by MaCa
	END
	if (@LoaiMa = 2)
	BEGIN
		insert into @tab
		Select RIGHT(MaPhieu, 5) as Ma, ROW_NUMBER() over(order by MaPhieu) as SoThuTu from NS_PhieuPhanCa
		WHERE LEN(MaPhieu)>6 and  TrangThai!=0 and  LEFT(MaPhieu, 2) = LEFT(@inputVar, 2) AND ISNUMERIC(SUBSTRING(MaPhieu, 3, LEN(MaPhieu) - 2)) = 1 AND LEN(SUBSTRING(MaPhieu, 3, LEN(MaPhieu) - 2)) = 5
		order by MaPhieu
	END
	SELECT @Ma = (select TOP 1 SoThuTu from @tab
	where Ma != SoThuTu
	order by Ma)
	IF (LEN(@Ma) > 0)
	BEGIN
			SELECT @Ma = CASE
			WHEN @Ma > 0 and @Ma <= 9 THEN LEFT(@inputVar, 2) + '0000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9 and @Ma <= 99 THEN LEFT(@inputVar, 2) + '000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 99 and @Ma <= 999 THEN LEFT(@inputVar, 2) + '00' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 999 and @Ma <= 9999 THEN LEFT(@inputVar, 2) + '0' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9999 THEN LEFT(@inputVar, 2) + CONVERT(CHAR, @Ma)
			END
	END
	else
	BEgin
	 SELECT @Ma = (Select Max(SoThuTu) +1 from @tab)
		IF (LEN(@Ma) > 0)
		BEGIN
			SELECT @Ma = CASE
			WHEN @Ma > 0 and @Ma <= 9 THEN LEFT(@inputVar, 2) + '0000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9 and @Ma <= 99 THEN LEFT(@inputVar, 2) + '000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 99 and @Ma <= 999 THEN LEFT(@inputVar, 2) + '00' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 999 and @Ma <= 9999 THEN LEFT(@inputVar, 2) + '0' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9999 THEN LEFT(@inputVar, 2) + CONVERT(CHAR, @Ma)
			END
		END
		ELSE
		BEGIN
			SELECT @Ma = @inputVar;
		END
	END
	RETURN @Ma
END");

            Sql(@"ALTER FUNCTION [dbo].[check_MaNhanVien](@inputVar NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN    
	DECLARE @MaNhanVien NVARCHAR(MAX);
	DECLARE @tab table (MaNhanVien int, SoThuTu int)
	insert into @tab
	Select RIGHT(MaNhanVien, 5), ROW_NUMBER() over(order by MaNhanVien) as MaNhanVien from NS_NhanVien
	WHERE LEN(MaNhanVien)>6 and TrangThai!=0 and LEFT(MaNhanVien, 2) = LEFT(@inputVar, 2) AND ISNUMERIC(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 1 AND LEN(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 5
	order by MaNhanVien
	SELECT @MaNhanVien = (select TOP 1 SoThuTu from @tab
	where MaNhanVien != SoThuTu
	order by MaNhanVien)
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

            CreateStoredProcedure(name: "[dbo].[ChotKyTinhCong]", parametersAction: p => new
            {
                ID_KyTinhCong = p.Guid()
            }, body: @"SET NOCOUNT ON;

	DECLARE @dateNow varchar(7)
	set  @dateNow= convert(varchar(7), getdate(), 126);
	 DECLARE @Check int
    BEGIN TRANSACTION;
    SAVE TRANSACTION MySaveChoKyTinhCong;
    BEGIN TRY
			
			

			INSERT INTO NS_KyHieuCong(ID, CongQuyDoi, GioRa,GioVao,KyHieu,LayGioMacDinh,MoTa,TrangThai,TrangThaiCong,ID_KyTinhCong)
			SELECT NEWID(), CongQuyDoi, GioRa,GioVao,KyHieu,LayGioMacDinh,MoTa,TrangThai,TrangThaiCong,@ID_KyTinhCong
			FROM NS_KyHieuCong where   ID_KyTinhCong is null 

			INSERT INTO NS_NgayNghiLe(ID, CongQuyDoi, HeSoLuong,HeSoLuongOT,ID_KyTinhCong,LoaiNgay,MoTa,Ngay,NgaySua,NgayTao,NguoiSua,NguoiTao,Thu,TrangThai)
			SELECT NEWID(), CongQuyDoi, HeSoLuong,HeSoLuongOT,@ID_KyTinhCong,LoaiNgay,MoTa,Ngay,NgaySua,NgayTao,NguoiSua,NguoiTao,Thu,TrangThai
			FROM NS_NgayNghiLe
			where ID_KyTinhCong is null and ( Ngay is null  or convert(varchar(7), Ngay, 126) = @dateNow)

			Update NS_KyTinhCong set TrangThai=2 where ID=@ID_KyTinhCong

        COMMIT TRANSACTION 
		 set  @Check = 1;
    END TRY
  BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION MySaveChoKyTinhCong; -- rollback to MySavePoint
        END
		     set @Check = -1;
    END CATCH
	select @Check;");

            CreateStoredProcedure(name: "[dbo].[DeleteBangLuongChiTietById]", parametersAction: p => new
            {
                IdBangLuong = p.Guid()
            }, body: @"SET NOCOUNT ON;
	
DECLARE @Check int
    BEGIN TRANSACTION;
    SAVE TRANSACTION MySaveDeleteBangLuongChiTiet;
    BEGIN TRY
       
			 Delete from NS_BangLuong_ChiTiet where ID_BangLuong=@IdBangLuong;

        COMMIT TRANSACTION 
		 set  @Check = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION MySaveDeleteBangLuongChiTiet; -- rollback to MySaveDeleteBangLuongChiTiet
        END
		     set @Check = -1;
    END CATCH
	select @Check;");

            CreateStoredProcedure(name: "[dbo].[DeletePhieuPhanCa]", parametersAction: p => new
            {
                IdPhieu = p.Guid()
            }, body: @"DECLARE @Check int
    BEGIN TRANSACTION;
    SAVE TRANSACTION MySavePoint;
    BEGIN TRY
       
			 Delete from NS_PhieuPhanCa_CaLamViec where ID_PhieuPhanCa=@IdPhieu;
			 Delete from NS_PhieuPhanCa_NhanVien where ID_PhieuPhanCa=@IdPhieu;
			 Delete from NS_PhieuPhanCa where ID=@IdPhieu;

        COMMIT TRANSACTION 
		 set  @Check = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION MySavePoint; -- rollback to MySavePoint
        END
		     set @Check = -1;
    END CATCH
	select @Check;");

            CreateStoredProcedure(name: "[dbo].[GetAllBangLuongChiTietById]", parametersAction: p => new
            {
                id = p.Guid()
            }, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
         bl.ID as ID_BangLuong_ChiTiet,
          nv.ID as ID_NhanVien,
        nv.MaNhanVien,
        nv.TenNhanVien,
        ca.TenCa,
        bl.NgayCongThuc,
        bl.NgayCongChuan,
        bl.LuongCoBan,
        bl.PhuCapCoBan,
        bl.KhenThuong,
        bl.KyLuat,
        bl.PhatDiMuon,
        bl.LuongOT,
        bl.LuongThucNhan,
        bl.TongLuongNhan,
        bl.TongTienPhat,
		bl.PhuCapKhac,
		bl.ChietKhau,
		(bl.TongLuongNhan - bl.TongTienPhat) as TongTienNhanSauGiamTru
	
	 from  NS_BangLuong_ChiTiet bl
				join NS_NhanVien nv
				on bl.ID_NhanVien =nv.ID
				join  NS_CaLamViec ca
				on bl.ID_CaLamViec=ca.ID
				where bl.ID_BangLuong=@id ");

            CreateStoredProcedure(name: "[dbo].[GetAllDanhSachCaLamViec]", parametersAction: p => new
            {
                Text = p.String(),
                NgayBatDau = p.String(),
                NgayKetThuc = p.String(),
                ListTrangThai = p.String(),
                TimThoiGian = p.Int(),
                ListDonvi = p.String(),
                ID_NhanVien = p.Guid()
            }, body: @"SET NOCOUNT ON;
		DECLARE @TuNgay date 
    	DECLARE @DenNgay date
		DECLARE @CheckDonVi int
		set @CheckDonVi=1

		DECLARE @TableTrangThai TABLE( Id int)
    	INSERT INTO @TableTrangThai(Id) select  CONVERT(int,Name) from [dbo].[splitstring](@ListTrangThai+',') where Name!='';

		DECLARE @TableDonVi TABLE( Id uniqueidentifier)
    	INSERT INTO @TableDonVi(Id) select   CONVERT(uniqueidentifier,Name) from [dbo].[splitstring](@ListDonvi+',') where Name!='';
		
		IF (select count(*) from @TableDonVi) = 0
			set @CheckDonVi =0

		 IF(@TimThoiGian =1)
    	 set @TuNgay=CONVERT(datetime, @NgayBatDau, 103)
    	 set @DenNgay=CONVERT(datetime, @NgayKetThuc, 103) 
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select 
			DISTINCT
			ca.ID,
			ca.MaCa,
			ca.TenCa,
		CONVERT( VARCHAR(5), ca.GioVao, 108 ) as GioVao,
		CONVERT( VARCHAR(5), ca.GioRa, 108) as GioRa,
		ca.TongGioCong,
		CONVERT( VARCHAR(5), ca.NghiGiuaCaTu, 108 ) as NghiGiuaCaTu,
		CONVERT( VARCHAR(5), ca.NghiGiuaCaDen, 108 ) as NghiGiuaCaDen,
		CONVERT( VARCHAR(5), ca.GioOTBanNgayTu, 108 ) as GioOTBanNgayTu,
		CONVERT( VARCHAR(5), ca.GioOTBanNgayDen, 108 ) as GioOTBanNgayDen,
		CONVERT( VARCHAR(5), ca.GioOTBanDemTu, 108 ) as GioOTBanDemTu,
		CONVERT( VARCHAR(5), ca.GioOTBanDemDen, 108 ) as GioOTBanDemDen,
		ca.SoPhutDiMuon,
		ca.SoPhutVeSom,
		CONVERT( VARCHAR(5), ca.GioVaoTu, 108 ) as GioVaoTu,
		CONVERT( VARCHAR(5), ca.GioVaoDen, 108 ) as GioVaoDen,
		CONVERT( VARCHAR(5), ca.GioRaTu, 108 ) as GioRaTu,
		CONVERT( VARCHAR(5), ca.GioRaDen, 108 ) as GioRaDen,
		CONVERT( VARCHAR(5), ca.TinhOTBanNgayTu, 108 ) as TinhOTBanNgayTu,
		CONVERT( VARCHAR(5), ca.TinhOTBanNgayDen, 108 ) as TinhOTBanNgayDen,
		CONVERT( VARCHAR(5), ca.TinhOTBanDemTu, 108 ) as TinhOTBanDemTu,
		CONVERT( VARCHAR(5), ca.TinhOTBanDemDen, 108 ) as TinhOTBanDemDen,
		CASE 
    				WHEN ca.LaCaDem =1
    				THEN 'Ca đêm'
    				ELSE 'Ca ngày' 
		END as LaCaDem,
		ca.CachLayGioCong,
		ca.SoGioOTToiThieu,
		ca.GhiChuCaLamViec,
		ca.GhiChuTinhGio,
		ca.TrangThai,
		ca.NguoiTao,
		CONVERT( VARCHAR(10), ca.NgayTao, 103 ) as NgayTaoText,
		ca.NgayTao 
	 from NS_CaLamViec ca 
	  join NS_CaLamViec_DonVi cadv 
	 on ca.ID = cadv.ID_CaLamViec
	 join NS_QuaTrinhCongTac ct
	 on cadv.ID_DonVi =ct.ID_DonVi
	 where (						  UPPER(ca.MaCa) like N'%'+@Text+'%' 
									or	UPPER(ca.TenCa) like N'%'+@Text+'%'
									or	UPPER(ca.CaLamViec_ChuCaiDau) like N'%'+@Text+'%'
									or	UPPER(ca.CaLamViec_KhongDau) like N'%'+@Text+'%')
									and (@TimThoiGian=0 or (ca.NgayTao<=@DenNgay and ca.NgayTao>= @TuNgay))
									and ca.TrangThai in (select Id from @TableTrangThai)
									and ((@CheckDonVi=0 and ct.ID_NhanVien=@ID_NhanVien) or cadv.ID_DonVi in (select Id from @TableDonVi))");

            CreateStoredProcedure(name: "[dbo].[GetChietKhauNhanVien]", parametersAction: p => new
            {
                TuNgay = p.String(),
                DenNgay = p.String()
            }, body: @"SET NOCOUNT ON;
	DECLARE @NgayBatDau date 
   DECLARE @NgayKetThuc date
   set @NgayBatDau=CONVERT(datetime, @TuNgay, 103)
   set @NgayKetThuc=CONVERT(datetime, @DenNgay, 103) 
    -- Insert statements for procedure here
	select a.ID_nhanVien,ROUND(sum(a.TienMat + a.TienGui + a.TienChietKhau),1)  as TienChietKhau   from (	
							(select nvth.ID_NhanVien,sum(qhdct.TienMat) as TienMat, sum(qhdct.TienGui)  as TienGui,0 as TienChietKhau 
										from BH_HoaDon hd
										join BH_NhanVienThucHien nvth
										on hd.ID = nvth.ID_HoaDon
										join Quy_HoaDon qhd
										on nvth.ID_QuyHoaDon = qhd.ID
										join Quy_HoaDon_ChiTiet qhdct
										on qhd.ID = qhdct.ID_HoaDon
										 where nvth.ID_QuyHoaDon is not null 
										 and nvth.TinhChietKhauTheo in(1) 
										 and hd.ChoThanhToan=0
										and qhd.NgayLapHoaDon>=@NgayBatDau 
										and  qhd.NgayLapHoaDon<@NgayKetThuc
										 GROUP BY nvth.ID_nhanVien)
										 UNION ALL
							(select nvth.ID_nhanVien,0 as TienMat,0 as TienGui,Sum(nvth.TienChietKhau) as TienChietKhau
										from BH_HoaDon hd
										join BH_NhanVienThucHien nvth
										on hd.ID = nvth.ID_HoaDon where  nvth.TinhChietKhauTheo in(2,3) 
										and hd.ChoThanhToan=0 
										and hd.NgayLapHoaDon>=@NgayBatDau 
										and  hd.NgayLapHoaDon<@NgayKetThuc
														GROUP BY nvth.ID_nhanVien)
														 UNION ALL

							(select  nvth.ID_nhanVien,0 as TienMat,0 as TienGui,Sum(nvth.TienChietKhau) as TienChietKhau 
														 from BH_HoaDon hd
														join BH_HoaDon_ChiTiet bhct
														on hd.ID = bhct.ID_HoaDon
														join BH_NhanVienThucHien nvth
														on bhct.ID=nvth.ID_ChiTietHoaDon
														where hd.ChoThanhToan=0  
														and hd.NgayLapHoaDon>=@NgayBatDau 
														and  hd.NgayLapHoaDon<@NgayKetThuc
														GROUP BY nvth.ID_nhanVien)
														) a 
														GROUP BY a.ID_nhanVien");
            CreateStoredProcedure(name: "[dbo].[GetCongBoSungByListIdCong]", parametersAction: p => new
            {
                ListCongId = p.String()
            }, body: @"SET NOCOUNT ON;
	DECLARE @TableId TABLE( Id uniqueidentifier)
    	INSERT INTO @TableId(Id) select  CONVERT(uniqueidentifier,Name) from [dbo].[splitstring](@ListCongId+',') where Name!='';
    -- Insert statements for procedure here
	select  bs.ID,
			bs.Cong,
			bs.GhiChu,
			bs.KyHieuCong,
			bs.NgayCham,
			bs.NgayTao,
			bs.NguoiTao,
			bs.SoGioOT,
			 bs.SoPhutDiMuon,
			 ca.TenCa,
			 ca.MaCa,
			  ca.ID as ID_Ca,
			  cc.SoGioOT as TongSoGioOT,
			  cc.PhutDiMuon as TongSoPhutDiMuon,
			   ca.GioRa,
			   ca.GioVao
			 from NS_ChamCong_ChiTiet cc
	join NS_CaLamViec ca  on cc.ID_CaLamViec = ca.ID
	join NS_CongBoSung bs on cc.ID =bs.ID_ChamCongChiTiet 
	where cc.TrangThai!=0 and cc.ID in (select Id from @TableId)");

            CreateStoredProcedure(name: "[dbo].[GetDanhSachChamCong]", parametersAction: p => new
            {
                Text = p.String(),
                ID_KyTinhCong = p.Guid(),
                ID_DonVi = p.Guid(),
                ListPhongBan = p.String()
            }, body: @"SET NOCOUNT ON;
		DECLARE @TablePhonBan TABLE( Id uniqueidentifier)
    	INSERT INTO @TablePhonBan(Id) select  CONVERT(uniqueidentifier,Name) from [dbo].[splitstring](@ListPhongBan+',') where Name!='';
		DECLARE @CheckDonVi int
		set @CheckDonVi= (select count(*) from @TablePhonBan);

	select	
					   cc.ID,
                       cc.ID_CaLamViec,
                       cc.ID_KyTinhCong,
                       cc.ID_MaChamCong,
                       cc.ID_NhanVien,
                       cc.LoaiCong,
                       cc.Nam,
                       cc.Thang,
                       cc.Ngay1,
                       cc.Ngay10,
                       cc.Ngay11,
                       cc.Ngay12,
                       cc.Ngay13,
                       cc.Ngay14,
                       cc.Ngay15,
                       cc.Ngay16,
                       cc.Ngay17,
                       cc.Ngay18,
                       cc.Ngay19,
                       cc.Ngay20,
                       cc.Ngay21,
                       cc.Ngay22,
                       cc.Ngay23,
                       cc.Ngay24,
                       cc.Ngay25,
                       cc.Ngay26,
                       cc.Ngay27,
                       cc.Ngay28,
                       cc.Ngay29,
                       cc.Ngay30,
                       cc.Ngay31,
					    cc.Ngay2,
                       cc.Ngay3,
                       cc.Ngay4,
                       cc.Ngay5,
                       cc.Ngay6,
                       cc.Ngay7,
                       cc.Ngay8,
                       cc.Ngay9,
                       cc.NgayTao,
                       cc.NguoiTao,
                       cc.PhutDiMuon,
                       cc.SoNgayNghi,
                       cc.TongCong,
                       cc.TrangThai,
                       us.TenNhanVien,
                       us.MaNhanVien,
                       ca.TenCa,
                       ca.MaCa,
                       cc.GhiChu,
					   cc.SoGioOT,
					   us.NgaySinh,
					   CASE WHEN  cc.TongCongOTNgayNghi IS NULL THEN 0  
							 ELSE cc.TongCongOTNgayNghi                                     
							 END AS  TongCongNgayNghi
			 from NS_ChamCong_ChiTiet cc
			join (select DISTINCT nv.ID,nv.MaNhanVien,NV.TenNhanVien,NV.TenNhanVienChuCaiDau,NV.TenNhanVienKhongDau,nv.NgaySinh  from NS_NhanVien nv
					join NS_QuaTrinhCongTac ct
					on nv.ID = ct.ID_NhanVien
					where (@CheckDonVi=0 and ct.ID_DonVi=@ID_DonVi
							or ct.ID_PhongBan in (select Id from @TablePhonBan))
							and (UPPER(nv.MaNhanVien) like N'%'+@Text+'%' 
								 or UPPER(nv.TenNhanVien) like N'%'+@Text+'%'  
								 or UPPER(nv.TenNhanVienChuCaiDau) like N'%'+@Text+'%'  
								 or UPPER(nv.TenNhanVienKhongDau) like N'%'+@Text+'%' )) us
			on cc.ID_NhanVien = us.ID
		   join NS_CaLamViec ca 
		   on cc.ID_CaLamViec = ca.ID
		   where cc.ID_KyTinhCong=@ID_KyTinhCong 
		   and cc.TrangThai !=0
		   and cc.ID_DonVi =@ID_DonVi
		   order by cc.Nam,cc.Thang,us.MaNhanVien");

            CreateStoredProcedure(name: "[dbo].[GetListPhieuPhanCa]", parametersAction: p => new
            {
                Ma = p.String(),
                NgayBatDau = p.String(),
                NgayKetThuc = p.String(),
                ListTrangThai = p.String(),
                TimThoiGian = p.Int(),
                ListDonvi = p.String(),
                ListLoaiCa = p.String(),
                ID_NhanVien = p.Guid()
            }, body: @"DECLARE @TuNgay date 
    	DECLARE @DenNgay date
		DECLARE @CheckDonVi int
		DECLARE @CheckLoaiCa int
		DECLARE @CheckTrangThai int
		DECLARE @TableTrangThai TABLE( Id int)
    	INSERT INTO @TableTrangThai(Id) select  CONVERT(int,Name) from [dbo].[splitstring](@ListTrangThai+',') where Name!='';

		DECLARE @TableLoaiCa TABLE( Id int)
    	INSERT INTO @TableLoaiCa(Id) select  CONVERT(int,Name) from [dbo].[splitstring](@ListLoaiCa+',') where Name!='';

		DECLARE @TableDonVi TABLE( Id uniqueidentifier)
    	INSERT INTO @TableDonVi(Id) select   CONVERT(uniqueidentifier,Name) from [dbo].[splitstring](@ListDonvi+',') where Name!='';
		
		set @CheckDonVi =(select count(*) from @TableDonVi)

		set @CheckLoaiCa =(select count(*) from @TableLoaiCa)

		set @CheckTrangThai =(select count(*) from @TableTrangThai)

		 IF(@TimThoiGian =1)
    	 set @TuNgay=CONVERT(datetime, @NgayBatDau, 103)
    	 set @DenNgay=CONVERT(datetime, @NgayKetThuc, 103) 

	SET NOCOUNT ON;

    -- Insert statements for procedure here

	select		
		DISTINCT
		ph.MaPhieu,
			CASE 
    				WHEN ph.LoaiPhanCa =1
    				THEN N'Ca Tuần'
					WHEN ph.LoaiPhanCa =3
    				THEN N'Ca cố định'
    				ELSE N'Ca tháng' 
			END as LoaiPhanCaText,
			CASE 
    				WHEN ph.TrangThai =1
    				THEN N'Tạo mới'
    				ELSE N'Áp dụng' 
			END as TrangThaiText,
			CONVERT( VARCHAR(10), ph.TuNgay, 103 ) as TuNgayText,
			CONVERT( VARCHAR(10), ph.DenNgay, 103 ) as DenNgayText,
			CONVERT( VARCHAR(10), ph.NgayTao, 103 ) as NgayTaoText,
			ph.NguoiTao,
			ph.NgayTao,
			ph.GhiChu
	
	 from NS_PHIEUPHANCA ph
		join NS_QuaTrinhCongTac ct
		on ph.ID_DonVi =ct.ID_DonVi
	  where UPPER(ph.MaPhieu) like N'%'+@Ma+'%' 
									and ((@CheckDonVi=0 and ct.ID_NhanVien=@ID_NhanVien) or ph.ID_DonVi in (select Id from @TableDonVi))
									and (@CheckTrangThai=0 or ph.TrangThai in (select Id from @TableTrangThai))
									and  (@TimThoiGian=0 or (ph.NgayTao<=@DenNgay and ph.NgayTao>= @TuNgay))
									and (@CheckLoaiCa=0 or ph.LoaiPhanCa in (select Id from @TableLoaiCa))
									and ph.TrangThai !=0");

            CreateStoredProcedure(name: "[dbo].[InsertHoSoChamCong]", parametersAction: p => new
            {
                KyTinhCong = p.Guid(),
                Thang = p.Int(),
                Nam = p.Int(),
                TaiKhoan = p.String(),
                ListPhieuPhanCa = p.String()
            }, body: @"SET NOCOUNT ON;

	DECLARE @TablePhieu TABLE( Id uniqueidentifier)
    	INSERT INTO @TablePhieu(Id) select  CONVERT(uniqueidentifier,Name) from [dbo].[splitstring](@ListPhieuPhanCa+',') where Name!='';
	DECLARE @TableNhanVienDaCham TABLE( Id [nvarchar](max))
    	INSERT INTO @TableNhanVienDaCham(Id) select CONVERT([nvarchar](max), cc.ID_NhanVien)+'_'+CONVERT([nvarchar](max), cc.ID_CaLamViec) from NS_ChamCong_ChiTiet cc where cc.ID_KyTinhCong=@KyTinhCong;
    -- Insert statements for procedure here
	DECLARE @Check int

    BEGIN TRANSACTION;
    SAVE TRANSACTION MySaveInsertHoSoChamCong;
    BEGIN TRY
				Insert into NS_ChamCong_ChiTiet(ID,ID_MaChamCong,ID_DonVi,ID_NhanVien,ID_CaLamViec,LoaiCong,ID_KyTinhCong,TrangThai,Thang,Nam,NgayTao,NguoiTao,
												SoNgayNghi,TongCong,PhutDiMuon,SoGioOT,TongCongOTNgayNghi,TongGioOTQuyDoi,TongCongQuyDoi,TongCongOTNgayNghiQuyDoi)
				select
					
				 NEWID(),
					null,
					fr.ID_DonVi,
				 fr.ID_NhanVien,
				 fr.ID_CaLamViec,
				 1,
				 @KyTinhCong,
				 1,
				 @Thang,
				 @Nam,
				 GETDATE(),
				 @TaiKhoan ,0,0,0,0,0,0,0,0 from 
					( select DISTINCT
						p.ID_DonVi,
					  nvpc.ID_NhanVien,
					 nvca.ID_CaLamViec
					 from NS_PhieuPhanCa_NhanVien nvpc 
						join NS_PhieuPhanCa p on p.ID = nvpc.ID_PhieuPhanCa
 						join NS_PhieuPhanCa_CaLamViec nvca on p.ID = nvca.ID_PhieuPhanCa
						where p.ID in (select Id from @TablePhieu) and
						CONVERT([nvarchar](max), nvpc.ID_NhanVien) +'_'+ CONVERT([nvarchar](max), nvca.ID_CaLamViec)  not in (select Id from @TableNhanVienDaCham)) fr

				Update NS_PhieuPhanCa set TrangThai = 2 where ID in (select Id from @TablePhieu)
	 COMMIT TRANSACTION 
		 set  @Check = 1;
	END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION MySaveInsertHoSoChamCong; -- rollback to MySavePoint
        END
		     set @Check = -1;
    END CATCH
	select @Check;");

            CreateStoredProcedure(name: "[dbo].[RandomMaBangLuong]", parametersAction: p => new
            {
                inputVar = p.String()
            }, body: @"SET NOCOUNT ON;
DECLARE @Ma NVARCHAR(MAX);
	DECLARE @tab table (Ma int, SoThuTu int)

		insert into @tab
		Select RIGHT(MaBangLuong, 5) as Ma, ROW_NUMBER() over(order by MaBangLuong) as SoThuTu from NS_BangLuong
		WHERE LEN(MaBangLuong)>6 and LEFT(MaBangLuong, 2) = LEFT(@inputVar, 2) AND ISNUMERIC(SUBSTRING(MaBangLuong, 3, LEN(MaBangLuong) - 2)) = 1 AND LEN(SUBSTRING(MaBangLuong, 3, LEN(MaBangLuong) - 2)) = 5
		and TrangThai !=0
		order by MaBangLuong
	SELECT @Ma = (select TOP 1 SoThuTu from @tab
	where Ma != SoThuTu
	order by Ma)
	IF (LEN(@Ma) > 0)
	BEGIN
			SELECT @Ma = CASE
			WHEN @Ma > 0 and @Ma <= 9 THEN LEFT(@inputVar, 2) + '0000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9 and @Ma <= 99 THEN LEFT(@inputVar, 2) + '000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 99 and @Ma <= 999 THEN LEFT(@inputVar, 2) + '00' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 999 and @Ma <= 9999 THEN LEFT(@inputVar, 2) + '0' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9999 THEN LEFT(@inputVar, 3) + CONVERT(CHAR, @Ma)
			END
	END
	else
	BEgin
	 SELECT @Ma = (Select Max(SoThuTu) +1 from @tab)
		IF (LEN(@Ma) > 0)
		BEGIN
			SELECT @Ma = CASE
			WHEN @Ma > 0 and @Ma <= 9 THEN LEFT(@inputVar, 2) + '0000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9 and @Ma <= 99 THEN LEFT(@inputVar, 2) + '000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 99 and @Ma <= 999 THEN LEFT(@inputVar, 2) + '00' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 999 and @Ma <= 9999 THEN LEFT(@inputVar, 2) + '0' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9999 THEN LEFT(@inputVar, 3) + CONVERT(CHAR, @Ma)
			END
		END
		ELSE
		BEGIN
			SELECT @Ma = @inputVar;
		END
	END
	select  @Ma");

            CreateStoredProcedure(name: "[dbo].[RandomMaPhieuPhanCa]", parametersAction: p => new
            {
                inputVar = p.String()
            }, body: @"SET NOCOUNT ON;
	DECLARE @Ma NVARCHAR(MAX);
	DECLARE @tab table (Ma int, SoThuTu int)

		insert into @tab
		Select RIGHT(MaPhieu, 5) as Ma, ROW_NUMBER() over(order by MaPhieu) as SoThuTu from NS_PhieuPhanCa
		WHERE LEN(MaPhieu)>7 and LEFT(MaPhieu, 3) = LEFT(@inputVar, 3) AND ISNUMERIC(SUBSTRING(MaPhieu, 4, LEN(MaPhieu) - 3)) = 1 AND LEN(SUBSTRING(MaPhieu, 4, LEN(MaPhieu) - 3)) = 5
			 and  TrangThai !=0
		order by MaPhieu
	SELECT @Ma = (select TOP 1 SoThuTu from @tab
	where Ma != SoThuTu
	order by Ma)
	IF (LEN(@Ma) > 0)
	BEGIN
			SELECT @Ma = CASE
			WHEN @Ma > 0 and @Ma <= 9 THEN LEFT(@inputVar, 3) + '0000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9 and @Ma <= 99 THEN LEFT(@inputVar, 3) + '000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 99 and @Ma <= 999 THEN LEFT(@inputVar, 3) + '00' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 999 and @Ma <= 9999 THEN LEFT(@inputVar, 3) + '0' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9999 THEN LEFT(@inputVar, 3) + CONVERT(CHAR, @Ma)
			END
	END
	else
	BEgin
	 SELECT @Ma = (Select Max(SoThuTu) +1 from @tab)
		IF (LEN(@Ma) > 0)
		BEGIN
			SELECT @Ma = CASE
			WHEN @Ma > 0 and @Ma <= 9 THEN LEFT(@inputVar, 3) + '0000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9 and @Ma <= 99 THEN LEFT(@inputVar, 3) + '000' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 99 and @Ma <= 999 THEN LEFT(@inputVar, 3) + '00' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 999 and @Ma <= 9999 THEN LEFT(@inputVar, 3) + '0' + CONVERT(CHAR, @Ma)
			WHEN @Ma > 9999 THEN LEFT(@inputVar, 3) + CONVERT(CHAR, @Ma)
			END
		END
		ELSE
		BEGIN
			SELECT @Ma = @inputVar;
		END
	END
	select  @Ma");

            CreateStoredProcedure(name: "[dbo].[UpdateChamCongKhiThayDoiHeSo]", parametersAction: p => new
            {
                KyHieu = p.String(),
                Thu = p.Int(),
                Loai = p.Int()
            }, body: @"SET NOCOUNT ON;
	DECLARE @TableUpdate TABLE(Id uniqueidentifier) 
	if(@Loai=1)
	INSERT INTO @TableUpdate(Id) (select DISTINCT c.ID from NS_KyTinhCong k
								JOIN NS_ChamCong_ChiTiet c
								on k.ID=c.ID_KyTinhCong
								JOIN NS_CongBoSung b
								on c.ID =b.ID_ChamCongChiTiet
								where k.TrangThai !=2 
								and b.KyHieuCong = @KyHieu)
	else
	INSERT INTO @TableUpdate(Id) (select DISTINCT c.ID from NS_KyTinhCong k
								JOIN NS_ChamCong_ChiTiet c
								on k.ID=c.ID_KyTinhCong
								JOIN NS_CongBoSung b
								on c.ID =b.ID_ChamCongChiTiet
								where k.TrangThai !=2 
								and b.Thu = @Thu)
	DECLARE @Check int
    BEGIN TRANSACTION;
    SAVE TRANSACTION MySaveChangeCongChiTiet;
    BEGIN TRY

				Update NS_ChamCong_ChiTiet Set 
							TongCong= case WHEN (select TOP 1 bsc.ID from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay = 0 )is not null then
							   (select SUM(bsc.Cong) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay = 0 ) else 0
							   end , 
							TongCongQuyDoi=case WHEN (select TOP 1 bsc.ID from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay = 0 )is not null then
							(select SUM(bsc.CongQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay = 0 ) else 0
							   end , 
							TongCongOTNgayNghi=case WHEN (select TOP 1 bsc.ID from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay != 0 )is not null then
							(select SUM(bsc.Cong) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay != 0 )else 0
							   end , 
							TongCongOTNgayNghiQuyDoi=case WHEN (select TOP 1 bsc.ID from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay != 0 )is not null then
							(select SUM(bsc.CongQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID and bsc.LoaiNgay != 0 )else 0
							   end , 
							PhutDiMuon=(select SUM(bsc.SoPhutDiMuon) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID),
							SoGioOT=(select SUM(bsc.SoGioOT) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID), 
							TongGioOTQuyDoi=(select SUM(bsc.GioOTQuyDoi) from NS_CongBoSung bsc where bsc.ID_ChamCongChiTiet=NS_ChamCong_ChiTiet.ID) 
							where NS_ChamCong_ChiTiet.ID in( select Id from @TableUpdate)
			  
        COMMIT TRANSACTION 
		 set  @Check = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION MySaveChangeCongChiTiet; -- rollback to MySaveChangeCongChiTiet
        END
		     set @Check = -1;
    END CATCH
	select @Check;");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateTonLuyKeTheoHoaDon]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanhInput [uniqueidentifier],
    @ThoiGian [datetime],
    @Loai [int]
AS
BEGIN
    SET NOCOUNT ON;
		--SELECT * FROM BH_HoaDon where MaHoaDon = 'PNK000253'
		--DECLARE @IDHoaDonInput UNIQUEIDENTIFIER = '6909B4D2-C40E-42F2-A67C-0C126BCF4E71', @IDChiNhanhInput UNIQUEIDENTIFIER = 'D78CF00C-78E2-4832-A33B-4B2FECEA1BBB', @ThoiGian DATETIME = '2019-07-31 16:06:11.810', @Loai INT = 1;
    	DECLARE @ThoiGianHD DATETIME;
    	DECLARE @LoaiHoaDonTruyenVao INT;
    	
		DECLARE @tblChiTiet TABLE (ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME)
		DECLARE @LuyKeDauKy TABLE(ID_LoHang UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TonLuyKe FLOAT);
		DECLARE @LoaiHoaDonInput INT;
		DECLARE @ChoThanhToanKiemKe BIT;
		DECLARE @ChiTietHoaDonUpdateKiemKeUpdate TABLE (ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME);
		DECLARE @ChiTietHoaDonUpdateKiemKe TABLE (ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ThanhTien FLOAT);
		DECLARE @hdctUpdate TABLE(ID UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, TonLuyKe FLOAT, LoaiHoaDon INT);
    	IF(@Loai = 1 OR @Loai = 3 OR @Loai = 4)
		BEGIN
			INSERT INTO @tblChiTiet
			SELECT dvqd.ID_HangHoa, hdct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, CASE 
				WHEN (hd.YeuCau = '4' OR hd.YeuCau = '3')  AND hd.ID_CheckIn = @IDChiNhanhInput
				THEN
					hd.NgaySua
				ELSE
					hd.NgayLapHoaDon
				END AS NgayLapHoaDon FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN DM_HangHoa hh
    		on hh.ID = dvqd.ID_HangHoa
			WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1 AND IIF(hd.LoaiHoaDon != 6, 1, IIF(hdct.ChatLieu != '2' OR hdct.ChatLieu IS NULL, 1, 0)) = 1
			GROUP BY dvqd.ID_HangHoa, hdct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;

			--SELECT * FROM @tblChiTiet
			
			INSERT INTO @ChiTietHoaDonUpdateKiemKe
			SELECT ID_HangHoa, ID_LoHang, NgayLapHoaDon, ThanhTien FROM
			(select *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN from
			(
			select
			CASE 
				WHEN hdupdate.YeuCau = '4' AND @IDChiNhanhInput = hdupdate.ID_CheckIn
				THEN
    				hdupdate.NgaySua
				ELSE
    				hdupdate.NgayLapHoaDon
			END AS NgayLapHoaDon,
			dvqdupdate.ID_HangHoa as ID_HangHoa, cthdthemmoiupdate.ID_LoHang, SUM(hdctupdate.ThanhTien * dvqdupdate.TyLeChuyenDoi) AS ThanhTien FROM BH_HoaDon hdupdate
			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
			ON hdupdate.ID = hdctupdate.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqdupdate
			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
			INNER JOIN @tblChiTiet cthdthemmoiupdate
			ON cthdthemmoiupdate.ID_HangHoa = dvqdupdate.ID_HangHoa
			WHERE hdupdate.ID_DonVi = @IDChiNhanhInput AND hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
			hdupdate.NgayLapHoaDon < cthdthemmoiupdate.NgayLapHoaDon
			GROUP BY dvqdupdate.ID_HangHoa, hdctupdate.ID_LoHang, hdupdate.NgayLapHoaDon, hdupdate.YeuCau, hdupdate.ID_CheckIn, hdupdate.NgaySua, cthdthemmoiupdate.ID_LoHang) as table1) as temp WHERE temp.RN = 1;
			
			
			SELECT @LoaiHoaDonInput = LoaiHoaDon, @ChoThanhToanKiemKe = ChoThanhToan FROM BH_HoaDon WHERE ID = @IDHoaDonInput
			IF(@LoaiHoaDonInput = 9 and @ChoThanhToanKiemKe = 0)
			BEGIN
				INSERT INTO @LuyKeDauKy
				SELECT ID_LoHang, ID_HangHoa, SUM(ThanhTien * TyLeChuyenDoi) AS TonLuyKe FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd
				ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @IDHoaDonInput
				GROUP BY ID_HangHoa, ID_LoHang
			END
			ELSE
			BEGIN
				INSERT INTO @LuyKeDauKy
				SELECT tdktemp.ID_LoHang, tdktemp.ID_HangHoa, tdktemp.TonLuyKe FROM
				(SELECT ID_LoHang, ID_HangHoa, TonLuyKe, ROW_NUMBER() OVER (PARTITION BY tdk.ID_HangHoa, tdk.ID_LoHang ORDER BY tdk.NgayLapHoaDon DESC) AS RN FROM 
				(SELECT ID_LoHang, ID_HangHoa, TonLuyKe, NgayLapHoaDon FROM 
				(SELECT hd.MaHoaDon, IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) AS NgayLapHoaDon, hd.LoaiHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa,ISNULL(ctkiemke.ThanhTien, 0) + (SUM(IIF(hd.LoaiHoaDon IN (1, 5, 7, 8), -1 * hdct.SoLuong * dvqd.TyLeChuyenDoi, 
    						IIF(hd.LoaiHoaDon IN (4, 6, 9, 18), hdct.SoLuong * dvqd.TyLeChuyenDoi, IIF((hd.LoaiHoaDon = 10 AND hd.YeuCau = '1') OR (hd.ID_CheckIn IS NOT NULL AND hd.ID_CheckIn != @IDChiNhanhInput AND hd.LoaiHoaDon = 10 AND hd.YeuCau = '4') AND hd.ID_DonVi = @IDChiNhanhInput, -1 * hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 
    						IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 0))))) OVER(PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon)) AS TonLuyKe, ROW_NUMBER() OVER (PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon DESC) AS RN FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN BH_HoaDon hd
				ON hd.ID = hdct.ID_HoaDon
				INNER JOIN DonViQuiDoi dvqd
				ON dvqd.ID = hdct.ID_DonViQuiDoi
				INNER JOIN @tblChiTiet tblct
				ON tblct.ID_HangHoa = dvqd.ID_HangHoa AND (tblct.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
				LEFT JOIN @ChiTietHoaDonUpdateKiemKe as  ctkiemke
				ON ctkiemke.ID_HangHoa = dvqd.ID_HangHoa AND (ctkiemke.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
				WHERE hd.ChoThanhToan = 0 AND (
				(IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) > ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null) AND IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) < @ThoiGian) AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
				GROUP BY hd.MaHoaDon,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa, ctkiemke.ThanhTien, hd.LoaiHoaDon, hdct.SoLuong, hdct.TienChietKhau, 
				hd.ID_CheckIn, hd.YeuCau, hd.ID_DonVi, hd.LoaiHoaDon, hd.NgaySua, hdct.ID_ChiTietDinhLuong) as temp1 WHERE temp1.RN = 1
				UNION ALL
				SELECT ID_LoHang, ID_HangHoa, ThanhTien, NgayLapHoaDon FROM @ChiTietHoaDonUpdateKiemKe) tdk) as tdktemp
				WHERE tdktemp.RN = 1;
			END
			
			
			INSERT INTO @ChiTietHoaDonUpdateKiemKeUpdate
			SELECT ID_HangHoa, ID_LoHang, NgayLapHoaDon FROM
			(select *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon) AS RN from
			(
			select
			CASE 
				WHEN hdupdate.YeuCau = '4' AND @IDChiNhanhInput = hdupdate.ID_CheckIn
				THEN
    				hdupdate.NgaySua
				ELSE
    				hdupdate.NgayLapHoaDon
			END AS NgayLapHoaDon, 
			hhupdate.ID as ID_HangHoa, cthdthemmoiupdate.ID_LoHang FROM BH_HoaDon hdupdate
			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
			ON hdupdate.ID = hdctupdate.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqdupdate
			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
			INNER JOIN DM_HangHoa hhupdate
			on hhupdate.ID = dvqdupdate.ID_HangHoa
			INNER JOIN @tblChiTiet cthdthemmoiupdate
			ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
			WHERE hdupdate.ID_DonVi = @IDChiNhanhInput AND hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
			hdupdate.NgayLapHoaDon > cthdthemmoiupdate.NgayLapHoaDon
			) as table1) as temp WHERE temp.RN = 1;

			
			INSERT INTO @hdctUpdate
			SELECT ctudhoadon.ID, ctudhoadon.ID_DonVi, ctudhoadon.ID_CheckIn , ctudhoadon.TonLuyKe, ctudhoadon.LoaiHoaDon FROM 
			(SELECT hd.LoaiHoaDon, hd.MaHoaDon, hd.ID_DonVi, hd.ID_CheckIn, hdct.ID, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa,
			ISNULL(lkdk.TonLuyKe, 0) + (SUM(IIF(hd.LoaiHoaDon IN (1, 5, 7, 8), -1 * hdct.SoLuong* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon IN (4, 6, 18), hdct.SoLuong* dvqd.TyLeChuyenDoi, 
			IIF((hd.LoaiHoaDon = 10 AND hd.YeuCau = '1') OR (hd.ID_CheckIn IS NOT NULL AND hd.ID_CheckIn != @IDChiNhanhInput AND hd.LoaiHoaDon = 10 AND hd.YeuCau = '4') AND hd.ID_DonVi = @IDChiNhanhInput, -1 * hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 0))))) 
			OVER(PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon))) AS TonLuyKe 
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN @tblChiTiet tblct
			ON tblct.ID_HangHoa = dvqd.ID_HangHoa AND (tblct.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			LEFT JOIN @LuyKeDauKy lkdk
			ON lkdk.ID_HangHoa = dvqd.ID_HangHoa AND (lkdk.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			LEFT JOIN @ChiTietHoaDonUpdateKiemKeUpdate as  ctkiemkeud
			ON ctkiemkeud.ID_HangHoa = dvqd.ID_HangHoa AND (ctkiemkeud.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			WHERE hd.ChoThanhToan = 0 AND (
			(IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) < ctkiemkeud.NgayLapHoaDon or ctkiemkeud.NgayLapHoaDon is null) AND IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) >= @ThoiGian) AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
			GROUP BY hd.MaHoaDon,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa, hd.LoaiHoaDon, hdct.SoLuong, 
			hdct.TienChietKhau, hd.ID_CheckIn, hd.YeuCau, hd.ID_DonVi, lkdk.TonLuyKe, hdct.ID, hd.NgaySua, hdct.ID_ChiTietDinhLuong) as ctudhoadon;
			--SELECT * FROM @hdctUpdate
			UPDATE hdct
    		SET hdct.TonLuyKe = IIF(tlkupdate.ID_DonVi = @IDChiNhanhInput, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput and tlkupdate.LoaiHoaDon = 10, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
    		FROM BH_HoaDon_ChiTiet hdct
    		INNER JOIN @hdctUpdate tlkupdate ON hdct.ID = tlkupdate.ID;
    	END
		IF(@Loai = 2)
		BEGIN
			SELECT @ThoiGianHD = NgayLapHoaDon, @LoaiHoaDonTruyenVao = LoaiHoaDon FROM BH_HoaDon where ID = @IDHoaDonInput
			DECLARE @TonDauThoiGianLon DATETIME, @TonDauThoiGianNho DATETIME;
			IF(@ThoiGian > @ThoiGianHD)
			BEGIN
				SET @TonDauThoiGianLon = @ThoiGian; 
				SET @TonDauThoiGianNho = @ThoiGianHD;
			END
			ELSE
			BEGIN
				SET @TonDauThoiGianLon = @ThoiGianHD; 
				SET @TonDauThoiGianNho = @ThoiGian;
			END

			INSERT INTO @tblChiTiet
			SELECT dvqd.ID_HangHoa, hdct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, CASE 
				WHEN hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput
				THEN
					hd.NgaySua
				ELSE
					hd.NgayLapHoaDon
				END AS NgayLapHoaDon FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN DM_HangHoa hh
    		on hh.ID = dvqd.ID_HangHoa
			WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1
			GROUP BY dvqd.ID_HangHoa, hdct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;

			--Kiểm kê thời gian nhỏ
			INSERT INTO @ChiTietHoaDonUpdateKiemKe
			SELECT ID_HangHoa, ID_LoHang, NgayLapHoaDon, ThanhTien FROM
			(select *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN from
			(
			select
			CASE 
				WHEN hdupdate.YeuCau = '4' AND @IDChiNhanhInput = hdupdate.ID_CheckIn
				THEN
    				hdupdate.NgaySua
				ELSE
    				hdupdate.NgayLapHoaDon
			END AS NgayLapHoaDon,
			dvqdupdate.ID_HangHoa as ID_HangHoa, cthdthemmoiupdate.ID_LoHang, SUM(hdctupdate.ThanhTien * dvqdupdate.TyLeChuyenDoi) AS ThanhTien FROM BH_HoaDon hdupdate
			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
			ON hdupdate.ID = hdctupdate.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqdupdate
			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
			INNER JOIN @tblChiTiet cthdthemmoiupdate
			ON cthdthemmoiupdate.ID_HangHoa = dvqdupdate.ID_HangHoa
			WHERE hdupdate.ID_DonVi = @IDChiNhanhInput AND hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
			hdupdate.NgayLapHoaDon < @TonDauThoiGianNho
			GROUP BY dvqdupdate.ID_HangHoa, hdctupdate.ID_LoHang, hdupdate.NgayLapHoaDon, hdupdate.YeuCau, hdupdate.ID_CheckIn, hdupdate.NgaySua, cthdthemmoiupdate.ID_LoHang) as table1) as temp WHERE temp.RN = 1;
			--Lũy kế đầu kỳ cho thời gian nhỏ
			
			INSERT INTO @LuyKeDauKy
			SELECT tdktemp.ID_LoHang, tdktemp.ID_HangHoa, tdktemp.TonLuyKe FROM
			(SELECT ID_LoHang, ID_HangHoa, TonLuyKe, ROW_NUMBER() OVER (PARTITION BY tdk.ID_HangHoa, tdk.ID_LoHang ORDER BY tdk.NgayLapHoaDon DESC) AS RN FROM 
			(SELECT ID_LoHang, ID_HangHoa, TonLuyKe, NgayLapHoaDon FROM 
			(SELECT hd.MaHoaDon, IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) AS NgayLapHoaDon, hd.LoaiHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa,ISNULL(ctkiemke.ThanhTien, 0) + (SUM(IIF(hd.LoaiHoaDon IN (1, 5, 7, 8), -1 * hdct.SoLuong * dvqd.TyLeChuyenDoi, 
    					IIF(hd.LoaiHoaDon IN (4, 6, 9, 18), hdct.SoLuong * dvqd.TyLeChuyenDoi, IIF((hd.LoaiHoaDon = 10 AND hd.YeuCau = '1') OR (hd.ID_CheckIn IS NOT NULL AND hd.ID_CheckIn != @IDChiNhanhInput AND hd.LoaiHoaDon = 10 AND hd.YeuCau = '4') AND hd.ID_DonVi = @IDChiNhanhInput, -1 * hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 0))))) OVER(PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon)) AS TonLuyKe, ROW_NUMBER() OVER (PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon DESC) AS RN FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN @tblChiTiet tblct
			ON tblct.ID_HangHoa = dvqd.ID_HangHoa AND (tblct.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			LEFT JOIN @ChiTietHoaDonUpdateKiemKe as  ctkiemke
			ON ctkiemke.ID_HangHoa = dvqd.ID_HangHoa AND (ctkiemke.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			WHERE hd.ChoThanhToan = 0 AND (
			(IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) > ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null) 
			AND IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) < @TonDauThoiGianNho) 
			AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
			GROUP BY hd.MaHoaDon,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa, ctkiemke.ThanhTien, hd.LoaiHoaDon, hdct.SoLuong, hdct.TienChietKhau, 
			hd.ID_CheckIn, hd.YeuCau, hd.ID_DonVi, hd.LoaiHoaDon, hd.NgaySua, hdct.ID_ChiTietDinhLuong) as temp1 WHERE temp1.RN = 1
			UNION ALL
			SELECT ID_LoHang, ID_HangHoa, ThanhTien, NgayLapHoaDon FROM @ChiTietHoaDonUpdateKiemKe) tdk) as tdktemp
			WHERE tdktemp.RN = 1;
			

			--Chi tiết kiểm kê trong khoảng thời gian sửa hóa đơn
			INSERT INTO @ChiTietHoaDonUpdateKiemKeUpdate
			SELECT ID_HangHoa, ID_LoHang, NgayLapHoaDon FROM
			(select *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon) AS RN from
			(
			select
			CASE 
				WHEN hdupdate.YeuCau = '4' AND @IDChiNhanhInput = hdupdate.ID_CheckIn
				THEN
    				hdupdate.NgaySua
				ELSE
    				hdupdate.NgayLapHoaDon
			END AS NgayLapHoaDon, 
			hhupdate.ID as ID_HangHoa, cthdthemmoiupdate.ID_LoHang FROM BH_HoaDon hdupdate
			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
			ON hdupdate.ID = hdctupdate.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqdupdate
			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
			INNER JOIN DM_HangHoa hhupdate
			on hhupdate.ID = dvqdupdate.ID_HangHoa
			INNER JOIN @tblChiTiet cthdthemmoiupdate
			ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
			WHERE hdupdate.ID_DonVi = @IDChiNhanhInput AND hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 
			AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) 
			AND hdupdate.NgayLapHoaDon > @TonDauThoiGianNho AND hdupdate.NgayLapHoaDon < @TonDauThoiGianLon
			) as table1) as temp WHERE temp.RN = 1;

			--Update chi tiết không có phiếu kiểm kê
			INSERT INTO @hdctUpdate
			SELECT ctudhoadon.ID, ctudhoadon.ID_DonVi, ctudhoadon.ID_CheckIn , ctudhoadon.TonLuyKe, ctudhoadon.LoaiHoaDon FROM 
			(SELECT hd.LoaiHoaDon, hd.MaHoaDon, hd.ID_DonVi, hd.ID_CheckIn, hdct.ID, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa,
			ISNULL(lkdk.TonLuyKe, 0) + (SUM(IIF(hd.LoaiHoaDon IN (1, 5, 7, 8), -1 * hdct.SoLuong* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon IN (4, 6, 18), hdct.SoLuong* dvqd.TyLeChuyenDoi, 
			IIF((hd.LoaiHoaDon = 10 AND hd.YeuCau = '1') OR (hd.ID_CheckIn IS NOT NULL AND hd.ID_CheckIn != @IDChiNhanhInput AND hd.LoaiHoaDon = 10 AND hd.YeuCau = '4') AND hd.ID_DonVi = @IDChiNhanhInput, -1 * hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 0))))) 
			OVER(PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon))) AS TonLuyKe
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN @tblChiTiet tblct
			ON tblct.ID_HangHoa = dvqd.ID_HangHoa AND (tblct.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			LEFT JOIN @LuyKeDauKy lkdk
			ON lkdk.ID_HangHoa = dvqd.ID_HangHoa AND (lkdk.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			LEFT JOIN @ChiTietHoaDonUpdateKiemKeUpdate as  ctkiemkeud
			ON ctkiemkeud.ID_HangHoa = dvqd.ID_HangHoa AND (ctkiemkeud.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			WHERE ctkiemkeud.ID_HangHoa IS NULL AND hd.ChoThanhToan = 0 AND (
			(IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) <= @TonDauThoiGianLon) 
			AND IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) >= @TonDauThoiGianNho) 
			AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
			GROUP BY hd.MaHoaDon,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa, hd.LoaiHoaDon, 
			hdct.SoLuong, hdct.TienChietKhau, hd.ID_CheckIn, hd.YeuCau, hd.ID_DonVi, lkdk.TonLuyKe, hdct.ID, hd.NgaySua, hdct.ID_ChiTietDinhLuong) as ctudhoadon;
			--update chi tiết thời gian nhỏ
			INSERT INTO @hdctUpdate
			SELECT ctudhoadon.ID, ctudhoadon.ID_DonVi, ctudhoadon.ID_CheckIn , ctudhoadon.TonLuyKe, ctudhoadon.LoaiHoaDon FROM 
			(SELECT hd.LoaiHoaDon, hd.MaHoaDon, hd.ID_DonVi, hd.ID_CheckIn, hdct.ID, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa,
			ISNULL(lkdk.TonLuyKe, 0) + (SUM(IIF(hd.LoaiHoaDon IN (1, 5, 7, 8), -1 * hdct.SoLuong* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon IN (4, 6, 18), hdct.SoLuong* dvqd.TyLeChuyenDoi, 
			IIF((hd.LoaiHoaDon = 10 AND hd.YeuCau = '1') OR (hd.ID_CheckIn IS NOT NULL AND hd.ID_CheckIn != @IDChiNhanhInput AND hd.LoaiHoaDon = 10 AND hd.YeuCau = '4') AND hd.ID_DonVi = @IDChiNhanhInput, -1 * hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 0))))) 
			OVER(PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon))) AS TonLuyKe
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN @tblChiTiet tblct
			ON tblct.ID_HangHoa = dvqd.ID_HangHoa AND (tblct.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			LEFT JOIN @LuyKeDauKy lkdk
			ON lkdk.ID_HangHoa = dvqd.ID_HangHoa AND (lkdk.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			INNER JOIN @ChiTietHoaDonUpdateKiemKeUpdate as  ctkiemkeud
			ON ctkiemkeud.ID_HangHoa = dvqd.ID_HangHoa AND (ctkiemkeud.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			WHERE hd.ChoThanhToan = 0 AND (
			(IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) < ctkiemkeud.NgayLapHoaDon) 
			AND IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) >= @TonDauThoiGianNho) 
			AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
			GROUP BY hd.MaHoaDon,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa, hd.LoaiHoaDon, 
			hdct.SoLuong, hdct.TienChietKhau, hd.ID_CheckIn, hd.YeuCau, hd.ID_DonVi, lkdk.TonLuyKe, hdct.ID, hd.NgaySua, hdct.ID_ChiTietDinhLuong) as ctudhoadon;

			--Kiểm kê thời gian lớn
			DECLARE @LuyKeDauKyThoiGianLon TABLE(ID_LoHang UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TonLuyKe FLOAT);
			DECLARE @ChiTietHoaDonUpdateKiemKeThoiGianLon TABLE (ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ThanhTien FLOAT);
			INSERT INTO @ChiTietHoaDonUpdateKiemKeThoiGianLon
			SELECT ID_HangHoa, ID_LoHang, NgayLapHoaDon, ThanhTien FROM
			(select *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN from
			(
			select
			CASE 
				WHEN hdupdate.YeuCau = '4' AND @IDChiNhanhInput = hdupdate.ID_CheckIn
				THEN
    				hdupdate.NgaySua
				ELSE
    				hdupdate.NgayLapHoaDon
			END AS NgayLapHoaDon,
			dvqdupdate.ID_HangHoa as ID_HangHoa, cthdthemmoiupdate.ID_LoHang, SUM(hdctupdate.ThanhTien * dvqdupdate.TyLeChuyenDoi) AS ThanhTien FROM BH_HoaDon hdupdate
			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
			ON hdupdate.ID = hdctupdate.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqdupdate
			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
			INNER JOIN @tblChiTiet cthdthemmoiupdate
			ON cthdthemmoiupdate.ID_HangHoa = dvqdupdate.ID_HangHoa
			INNER JOIN @ChiTietHoaDonUpdateKiemKeUpdate kkupdate
			ON kkupdate.ID_HangHoa = dvqdupdate.ID_HangHoa AND (hdctupdate.ID_LoHang IS NULL OR kkupdate.ID_LoHang = hdctupdate.ID_LoHang)
			WHERE hdupdate.ID_DonVi = @IDChiNhanhInput AND hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
			hdupdate.NgayLapHoaDon < @TonDauThoiGianLon
			GROUP BY dvqdupdate.ID_HangHoa, hdctupdate.ID_LoHang, hdupdate.NgayLapHoaDon, hdupdate.YeuCau, hdupdate.ID_CheckIn, hdupdate.NgaySua, cthdthemmoiupdate.ID_LoHang) as table1) as temp WHERE temp.RN = 1;

			--SELECT * FROM @ChiTietHoaDonUpdateKiemKeThoiGianLon
			--Lũy kế đầu kỳ cho thời gian lớn
			
			INSERT INTO @LuyKeDauKyThoiGianLon
			SELECT tdktemp.ID_LoHang, tdktemp.ID_HangHoa, tdktemp.TonLuyKe FROM
			(SELECT ID_LoHang, ID_HangHoa, TonLuyKe, ROW_NUMBER() OVER (PARTITION BY tdk.ID_HangHoa, tdk.ID_LoHang ORDER BY tdk.NgayLapHoaDon DESC) AS RN FROM 
			(SELECT ID_LoHang, ID_HangHoa, TonLuyKe, NgayLapHoaDon FROM 
			(SELECT hd.MaHoaDon, IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) AS NgayLapHoaDon, hd.LoaiHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa,ISNULL(ctkiemke.ThanhTien, 0) + (SUM(IIF(hd.LoaiHoaDon IN (1, 5, 7, 8), -1 * hdct.SoLuong * dvqd.TyLeChuyenDoi, 
    					IIF(hd.LoaiHoaDon IN (4, 6, 9, 18), hdct.SoLuong * dvqd.TyLeChuyenDoi, IIF((hd.LoaiHoaDon = 10 AND hd.YeuCau = '1') OR (hd.ID_CheckIn IS NOT NULL AND hd.ID_CheckIn != @IDChiNhanhInput AND hd.LoaiHoaDon = 10 AND hd.YeuCau = '4') AND hd.ID_DonVi = @IDChiNhanhInput, -1 * hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 0))))) OVER(PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon)) AS TonLuyKe, ROW_NUMBER() OVER (PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY hd.NgayLapHoaDon DESC) AS RN FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN @tblChiTiet tblct
			ON tblct.ID_HangHoa = dvqd.ID_HangHoa AND (tblct.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			INNER JOIN @ChiTietHoaDonUpdateKiemKeUpdate kkupdate
			ON kkupdate.ID_HangHoa = dvqd.ID_HangHoa AND (hdct.ID_LoHang IS NULL OR kkupdate.ID_LoHang = hdct.ID_LoHang)
			LEFT JOIN @ChiTietHoaDonUpdateKiemKeThoiGianLon as ctkiemke
			ON ctkiemke.ID_HangHoa = dvqd.ID_HangHoa AND (ctkiemke.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			WHERE hd.ChoThanhToan = 0 AND (
			(IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) > ctkiemke.NgayLapHoaDon or ctkiemke.NgayLapHoaDon is null) 
			AND IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) < @TonDauThoiGianLon) 
			AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
			GROUP BY hd.MaHoaDon,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa, ctkiemke.ThanhTien, hd.LoaiHoaDon, hdct.SoLuong, hdct.TienChietKhau, 
			hd.ID_CheckIn, hd.YeuCau, hd.ID_DonVi, hd.LoaiHoaDon, hd.NgaySua, hdct.ID_ChiTietDinhLuong) as temp1 WHERE temp1.RN = 1
			UNION ALL
			SELECT ID_LoHang, ID_HangHoa, ThanhTien, NgayLapHoaDon FROM @ChiTietHoaDonUpdateKiemKeThoiGianLon) tdk) as tdktemp
			WHERE tdktemp.RN = 1;

			--SELECT * FROM @LuyKeDauKyThoiGianLon

			--Chi tiết kiểm kê thời gian lớn
			DECLARE @ChiTietHoaDonUpdateKiemKeUpdateThoiGianLon TABLE (ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME);
			INSERT INTO @ChiTietHoaDonUpdateKiemKeUpdateThoiGianLon
			SELECT ID_HangHoa, ID_LoHang, NgayLapHoaDon FROM
			(select *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon) AS RN from
			(
			select
			CASE 
				WHEN hdupdate.YeuCau = '4' AND @IDChiNhanhInput = hdupdate.ID_CheckIn
				THEN
    				hdupdate.NgaySua
				ELSE
    				hdupdate.NgayLapHoaDon
			END AS NgayLapHoaDon, 
			hhupdate.ID as ID_HangHoa, cthdthemmoiupdate.ID_LoHang FROM BH_HoaDon hdupdate
			INNER JOIN BH_HoaDon_ChiTiet hdctupdate
			ON hdupdate.ID = hdctupdate.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqdupdate
			ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
			INNER JOIN DM_HangHoa hhupdate
			on hhupdate.ID = dvqdupdate.ID_HangHoa
			INNER JOIN @tblChiTiet cthdthemmoiupdate
			ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
			WHERE hdupdate.ID_DonVi = @IDChiNhanhInput AND hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 
			AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) 
			AND hdupdate.NgayLapHoaDon > @TonDauThoiGianLon
			) as table1) as temp WHERE temp.RN = 1;

			--update chi tiết thời gian lớn
			INSERT INTO @hdctUpdate
			SELECT ctudhoadon.ID, ctudhoadon.ID_DonVi, ctudhoadon.ID_CheckIn , ctudhoadon.TonLuyKe, ctudhoadon.LoaiHoaDon FROM 
			(SELECT hd.LoaiHoaDon, hd.MaHoaDon, hd.ID_DonVi, hd.ID_CheckIn, hdct.ID, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa,
			ISNULL(lkdk.TonLuyKe, 0) + (SUM(IIF(hd.LoaiHoaDon IN (1, 5, 7, 8), -1 * hdct.SoLuong* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon IN (4, 6, 18), hdct.SoLuong* dvqd.TyLeChuyenDoi, 
			IIF((hd.LoaiHoaDon = 10 AND hd.YeuCau = '1') OR (hd.ID_CheckIn IS NOT NULL AND hd.ID_CheckIn != @IDChiNhanhInput AND hd.LoaiHoaDon = 10 AND hd.YeuCau = '4') AND hd.ID_DonVi = @IDChiNhanhInput, -1 * hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 
    		IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hdct.TienChietKhau* dvqd.TyLeChuyenDoi, 0))))) 
			OVER(PARTITION BY dvqd.ID_HangHoa, hdct.ID_LoHang ORDER BY IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon))) AS TonLuyKe
			FROM BH_HoaDon_ChiTiet hdct
			INNER JOIN BH_HoaDon hd
			ON hd.ID = hdct.ID_HoaDon
			INNER JOIN DonViQuiDoi dvqd
			ON dvqd.ID = hdct.ID_DonViQuiDoi
			INNER JOIN @tblChiTiet tblct
			ON tblct.ID_HangHoa = dvqd.ID_HangHoa AND (tblct.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			LEFT JOIN @LuyKeDauKyThoiGianLon lkdk
			ON lkdk.ID_HangHoa = dvqd.ID_HangHoa AND (lkdk.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			INNER JOIN @ChiTietHoaDonUpdateKiemKeUpdate kkupdate
			ON kkupdate.ID_HangHoa = dvqd.ID_HangHoa AND (hdct.ID_LoHang IS NULL OR kkupdate.ID_LoHang = hdct.ID_LoHang)
			LEFT JOIN @ChiTietHoaDonUpdateKiemKeUpdateThoiGianLon as  ctkiemkeud
			ON ctkiemkeud.ID_HangHoa = dvqd.ID_HangHoa AND (ctkiemkeud.ID_LoHang = hdct.ID_LoHang OR hdct.ID_LoHang IS NULL)
			WHERE hd.ChoThanhToan = 0 AND (
			(IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) < ctkiemkeud.NgayLapHoaDon OR ctkiemkeud.NgayLapHoaDon IS NULL) 
			AND IIF(hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput, hd.NgaySua ,hd.NgayLapHoaDon) >= @TonDauThoiGianLon) 
			AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))
			GROUP BY hd.MaHoaDon,dvqd.TyLeChuyenDoi, hd.NgayLapHoaDon, hdct.ID_LoHang, dvqd.ID_HangHoa, hd.LoaiHoaDon, 
			hdct.SoLuong, hdct.TienChietKhau, hd.ID_CheckIn, hd.YeuCau, hd.ID_DonVi, lkdk.TonLuyKe, hdct.ID, hd.NgaySua, hdct.ID_ChiTietDinhLuong) as ctudhoadon;

			UPDATE hdct
    		SET hdct.TonLuyKe = IIF(tlkupdate.ID_DonVi = @IDChiNhanhInput, tlkupdate.TonLuyKe, hdct.TonLuyKe), hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput and tlkupdate.LoaiHoaDon = 10, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
    		FROM BH_HoaDon_ChiTiet hdct
    		INNER JOIN @hdctUpdate tlkupdate ON hdct.ID = tlkupdate.ID;
		END
END");

            Sql(@"--TRA GOIDV (2) -- KHONG CAN UPDATE ID_CHITIETGOIDV 
update bh_hoadon_chitiet set ChatLieu = 2 where id in 
(select ctsd.ID from bh_hoadon_chitiet ctsd
join bh_hoadon_chitiet ct on ctsd.ID_ChiTietGoiDV = ct.ID
join BH_HoaDon hd on ctsd.id_hoadon = hd.id
where ctsd.id_chitietgoidv is not null
and hd.loaihoadon = 6)

-- SUDUNG GOIDV (4) -- KHONG CAN UPDATE  ID_CHITIETGOIDV
update bh_hoadon_chitiet set ChatLieu = 4 where id in 
(select ctsd.ID from bh_hoadon_chitiet ctsd
join bh_hoadon_chitiet ct on ctsd.ID_ChiTietGoiDV = ct.ID
join BH_HoaDon hd on ctsd.id_hoadon = hd.id
where ctsd.id_chitietgoidv is not null
and hd.loaihoadon = 1)

	-- ============= TRA HD (1) - UPDATE ChatLieu
update bh_hoadon_chitiet set ChatLieu = 1 where id in (
select ctt.id
from bh_hoadon_chitiet ctt
join BH_HoaDon hd on ctt.id_hoadon = hd.id
where hd.ID_HoaDon is not null and id_chitietgoidv is  null
and hd.loaihoadon = 6)

--- update ID_ChiTietGoiDV (Tra HD)
update  ct
set ct.ID_ChiTietGoiDV = a.id
from bh_hoadon_chitiet ct
join 
(
select ctt.id as idt, ctm.id,ctm.id_lohang, ctt.id_donviquidoi as qdt, ctm.ID_DonViQuiDoi, ctt.SoLuong as slt, ctm.SoLuong, hdm.Ngaylaphoadon as n1, hdm.MaHoaDon as m1, hdt.Mahoadon , hdt.NgayLapHoaDon
from bh_hoadon_chitiet ctt
join BH_HoaDon hdt on ctt.id_hoadon = hdt.id
join BH_HoaDon hdm on hdt.ID_HoaDon = hdm.ID
join bh_hoadon_chitiet ctm on hdm.ID= ctm.ID_HoaDon and ctt.id_donviquidoi= ctm.ID_DonViQuiDoi and (ctt.id_lohang = ctm.id_lohang or (ctt.id_lohang is null and ctm.id_lohang is null))
where ctt.id_chitietgoidv is  null
and hdt.loaihoadon = 6
) a on ct.id= a.idt


-- ====== XULY DH (3) -- update ChatLieu
update bh_hoadon_chitiet set ChatLieu = 1 where id in (
select ctt.id
from bh_hoadon_chitiet ctt
join BH_HoaDon hd on ctt.id_hoadon = hd.id
where hd.ID_HoaDon is not null and id_chitietgoidv is  null
and hd.loaihoadon = 1)


-- ====== XULY DH (3) update ID_ChiTietGoiDV 
update ct
set ct.ID_ChiTietGoiDV = a.id
from bh_hoadon_chitiet ct
join 
(
select ctt.id as idt, ctm.id,ctm.id_lohang, ctt.id_donviquidoi as qdt, ctm.ID_DonViQuiDoi, ctt.SoLuong as slt, ctm.SoLuong, hdm.Ngaylaphoadon as n1, hdm.MaHoaDon as m1, hdt.Mahoadon , hdt.NgayLapHoaDon
from bh_hoadon_chitiet ctt
join BH_HoaDon hdt on ctt.id_hoadon = hdt.id
join BH_HoaDon hdm on hdt.ID_HoaDon = hdm.ID
join bh_hoadon_chitiet ctm on hdm.ID= ctm.ID_HoaDon and ctt.id_donviquidoi= ctm.ID_DonViQuiDoi and (ctt.id_lohang = ctm.id_lohang or (ctt.id_lohang is null and ctm.id_lohang is null))
where ctt.id_chitietgoidv is  null
and hdt.loaihoadon = 1) a
ON ct.id= a.idt");

            Sql(@"ALTER FUNCTION [dbo].[GetListNhomHangHoa] ( @IDNhomHang UNIQUEIDENTIFIER )
RETURNS
 @tblNhomHang TABLE (ID UNIQUEIDENTIFIER)
AS
BEGIN
	IF(@IDNhomHang IS NOT NULL)
	BEGIN
		DECLARE @tblNhomHangTemp TABLE (ID UNIQUEIDENTIFIER);
		INSERT INTO @tblNhomHang VALUES (@IDNhomHang);
		INSERT INTO @tblNhomHangTemp VALUES (@IDNhomHang);
		DECLARE @intFlag INT;
		SET @intFlag = 1;
		WHILE (@intFlag != 0)
		BEGIN
			SELECT @intFlag = COUNT(ID) FROM DM_NhomHangHoa WHERE ID_Parent IN (SELECT ID FROM @tblNhomHangTemp);
			IF(@intFlag != 0)
			BEGIN
				INSERT INTO @tblNhomHangTemp
				SELECT ID FROM DM_NhomHangHoa WHERE ID_Parent IN (SELECT ID FROM @tblNhomHangTemp); 
				DELETE FROM @tblNhomHangTemp WHERE ID IN (SELECT ID FROM @tblNhomHang);
				INSERT INTO @tblNhomHang
				SELECT ID FROM @tblNhomHangTemp
			END
		END
	END
	ELSE
	BEGIN
		INSERT INTO @tblNhomHang
		SELECT ID FROM DM_NhomHangHoa
	END
	RETURN
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[ChotKyTinhCong]");
            DropStoredProcedure("[dbo].[DeleteBangLuongChiTietById]");
            DropStoredProcedure("[dbo].[DeletePhieuPhanCa]");
            DropStoredProcedure("[dbo].[GetAllBangLuongChiTietById]");
            DropStoredProcedure("[dbo].[GetAllDanhSachCaLamViec]");
            DropStoredProcedure("[dbo].[GetChietKhauNhanVien]");
            DropStoredProcedure("[dbo].[GetCongBoSungByListIdCong]");
            DropStoredProcedure("[dbo].[GetDanhSachChamCong]");
            DropStoredProcedure("[dbo].[GetListPhieuPhanCa]");
            DropStoredProcedure("[dbo].[InsertHoSoChamCong]");
            DropStoredProcedure("[dbo].[RandomMaBangLuong]");
            DropStoredProcedure("[dbo].[RandomMaPhieuPhanCa]");
            DropStoredProcedure("[dbo].[UpdateChamCongKhiThayDoiHeSo]");
        }
    }
}
