namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20210614 : DbMigration
    {
        public override void Up()
        {
			CreateStoredProcedure(name: "[dbo].[CTHD_GetDichVubyDinhLuong]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid(),
				ID_DonViQuiDoi = p.Guid(),
				ID_LoHang = p.Guid()
			}, body: @"SET NOCOUNT ON;	

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
				hh.LaHangHoa, hh.TenHangHoa, 
				hh.ID_NhomHang as ID_NhomHangHoa, ISNULL(hh.GhiChu,'') as GhiChuHH,
				ctsc.IDChiTietDichVu ---- ~ id chitietHD of dichvu
			from 
			(
				--- ct hoadon suachua
				select 
					cttp.ID as ID_ChiTietGoiDV,
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
			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID");

			CreateStoredProcedure(name: "[dbo].[GetCTHDSuaChua_afterXuatKho]", parametersAction: p => new
			{
				ID_HoaDon = p.String()
			}, body: @"set nocount on

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
				 max(ctsc.ID_DonVi) as ID_DonVi,
				 sum(SoLuong) as SoLuongMua,
				 sum(SoLuongXuat) as SoLuongXuat,
				 sum(SoLuong) - isnull(sum(SoLuongXuat),0) as SoLuong
			from
			(
			select sum(ct.SoLuong) as SoLuong,
				0 as SoLuongXuat,
				ct.ID_DonViQuiDoi,
				ct.ID_LoHang,
				ct.ID_HoaDon,
				hd.ID_DonVi
			from BH_HoaDon_ChiTiet ct
			join BH_HoaDon hd on hd.ID= ct.ID_HoaDon
			where ct.ID_HoaDon= @ID_HoaDon
			and (ct.ID_ChiTietDinhLuong != ct.ID or ct.ID_ChiTietDinhLuong is null)
			group by ct.ID_DonViQuiDoi, ct.ID_LoHang,ct.ID_HoaDon,hd.ID_DonVi

			union all
			-- get cthd daxuat kho
			select 0 as SoLuong,
				sum(ct.SoLuong) as SoLuongXuat,
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
	where hh.LaHangHoa= 1");

			CreateStoredProcedure(name: "[dbo].[HDSC_GetChiTietXuatKho]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid(),
				IDChiTietHD = p.Guid(),
				LoaiHang = p.Int()
			}, body: @"SET NOCOUNT ON;

	if	@LoaiHang = 1 -- hanghoa
		begin
		select 
			qd.MaHangHoa, qd.TenDonViTinh,
			hh.TenHangHoa,
			lo.MaLoHang,
			pxk.SoLuong,
			round(pxk.GiaVon * pxk.SoLuong,3) as GiaVon,
			pxk.MaHoaDon,
			pxk.NgayLapHoaDon,
			pxk.GhiChu
		from(
			select 
				hd.MaHoaDon,
				hd.NgayLapHoaDon,
				ctxk.ID_DonViQuiDoi,
				ctxk.ID_LoHang,
				ctxk.SoLuong,
				ctxk.SoLuong * ctxk.GiaVon as GiaVon,
				ctxk.GhiChu
			from BH_HoaDon_ChiTiet ctxk
			join BH_HoaDon hd on ctxk.ID_HoaDon= hd.ID
			where ctxk.ID_ChiTietGoiDV = @IDChiTietHD		
			and hd.ChoThanhToan='0'
		) pxk
		join DonViQuiDoi qd on pxk.ID_DonViQuiDoi= qd.ID
		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		left join DM_LoHang lo on pxk.ID_LoHang= lo.ID

		end
	else
	begin

			select 
				hh.TenHangHoa,
				qd.MaHangHoa, qd.TenDonViTinh, qd.ThuocTinhGiaTri,
				isnull(lo.MaLoHang,'') as MaLoHang,
				tpdl.SoLuongDinhLuong_BanDau,
				round(tpdl.GiaTriDinhLuong_BanDau,3) as GiaTriDinhLuong_BanDau ,
				tpdl.MaHoaDon,
				tpdl.NgayLapHoaDon	,
				tpdl.SoLuongXuat as SoLuong,
				round(tpdl.GiaTriXuat,3) as GiaVon,
				tpdl.GhiChu,
				tpdl.LaDinhLuongBoSung
			from
			(
						---- get tpdl ban dau
						select 	
							ctxk.MaHoaDon,
							ctxk.NgayLapHoaDon,
							ct.SoLuong as SoLuongDinhLuong_BanDau,
							ct.SoLuong * ct.GiaVon as GiaTriDinhLuong_BanDau,
							ct.ID_DonViQuiDoi, 
							ct.ID_LoHang,
							isnull(ctxk.SoLuongXuat,0) as SoLuongXuat,
							isnull(ctxk.GiaTriXuat,0) as GiaTriXuat,
							isnull(ctxk.GhiChu,'') as GhiChu,
							0 as LaDinhLuongBoSung
						from BH_HoaDon_ChiTiet ct
						left join
						(
							---- get tpdl when xuatkho (ID_ChiTietGoiDV la hanghoa)
							select 
				
									hd.MaHoaDon,
									hd.NgayLapHoaDon	,
									ct.SoLuong as SoLuongXuat,
									round(ct.SoLuong * ct.GiaVon,3) as GiaTriXuat,
									ct.GhiChu,
									ct.ID_ChiTietGoiDV
							from BH_HoaDon_ChiTiet ct
							join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
							where hd.ChoThanhToan='0'
						) ctxk on ct.ID= ctxk.ID_ChiTietGoiDV
						where ct.ID_ChiTietDinhLuong= @IDChiTietHD
						and ct.ID != ct.ID_ChiTietDinhLuong				

						---- get dinhluong them vao khi tao phieu xuatkho (ID_ChiTietGoiDV la dichvu)
						union all

						select 
							hd.MaHoaDon,
							hd.NgayLapHoaDon,
							ct.SoLuong as SoLuongDinhLuong_BanDau,
							ct.SoLuong * ct.GiaVon as GiaTriDinhLuong_BanDau,
							ct.ID_DonViQuiDoi, 
							ct.ID_LoHang,
							isnull(ctxk.SoLuongXuat,0) as SoLuongXuat,
							isnull(ctxk.GiaTriXuat,0) as GiaTriXuat,
							isnull(ct.GhiChu,'') as GhiChu,
							1 as LaDinhLuongBoSung
						from BH_HoaDon_ChiTiet ct
						join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
						left join
						(
							---- sum soluongxuat cua chinh no
							select 
									sum(ct.SoLuong) as SoLuongXuat,
									sum(round(ct.SoLuong * ct.GiaVon,3)) as GiaTriXuat,
									ct.ID_DonViQuiDoi
							from BH_HoaDon_ChiTiet ct
							join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
							where hd.ChoThanhToan='0'
							and hd.LoaiHoaDon= 8 
							and ct.ID_ChiTietGoiDV= @IDChiTietHD
							group by ct.ID_DonViQuiDoi
						) ctxk on ct.ID_DonViQuiDoi= ctxk.ID_DonViQuiDoi
						where hd.ChoThanhToan='0'
						and hd.LoaiHoaDon= 8 
						and ct.ID_ChiTietGoiDV= @IDChiTietHD

			) tpdl
			join DonViQuiDoi qd on qd.ID= tpdl.ID_DonViQuiDoi
			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			left join DM_LoHang lo on tpdl.ID_LoHang= lo.ID
			order by tpdl.NgayLapHoaDon desc
		
	end");

			CreateStoredProcedure(name: "[dbo].[InsertChietKhauTraHang_TheoThucThu]", parametersAction: p => new
			{
				ID_HoaDonTra = p.Guid(),
				ID_PhieuChi = p.Guid()
			}, body: @"SET NOCOUNT ON;

	declare @ID_DonVi uniqueidentifier, @ID_HoaDonGoc uniqueidentifier , @TongChi float
	select @ID_DonVi= ID_DonVi, @ID_HoaDonGoc = ID_HoaDon from BH_HoaDon where ID= @ID_HoaDonTra
	select @TongChi = TongTienThu from Quy_HoaDon where ID= @ID_PhieuChi


		---- check xem co cai dat chiet khau TraHang khong
		declare @count_CKTraHang int   	
    	select @count_CKTraHang = count(hd.ID)
    	from ChietKhauMacDinh_HoaDon hd
    	where hd.ID_DonVi like @ID_DonVi
    	and hd.TrangThai !='0' and  hd.ChungTuApDung like '%6%'

		if	@count_CKTraHang > 0
    		begin		  		 			
    		
			insert into BH_NhanVienThucHien (ID, ID_NhanVien,  PT_ChietKhau, TinhChietKhauTheo, HeSo,ID_HoaDon,ThucHien_TuVan, TheoYeuCau, TienChietKhau, ID_QuyHoaDon)									
				select 
					NewID() as ID,
					ID_NhanVien,
					PT_ChietKhau,
					TinhChietKhauTheo,
					HeSo,
					@ID_HoaDonTra,
					ThucHien_TuVan,
					TheoYeuCau,
					(@TongChi * PT_ChietKhau *HeSo)/100 as TienChietKhau	,
					@ID_PhieuChi
				from(
					select 
						th.*, ROW_NUMBER() over (partition by th.ID_NhanVien order by th.ID_NhanVien, TienChietKhau desc) as Rn
					from BH_NhanVienThucHien th
					where ID_HoaDon like @ID_HoaDonGoc
					and TinhChietKhauTheo =1 -- thucthu: neu thanhtoan nhieulan, lay ck lon nhat
				) a where Rn= 1
			
    		end");

			CreateStoredProcedure(name: "[dbo].[UpdateIDCTNew_forCTOld]", parametersAction: p => new
			{
				Pair_IDNewIDOld = p.String()
			}, body: @"SET NOCOUNT ON;

	declare @PairNewOld nvarchar(max)
	declare _cur Cursor
	for
	 select Name from dbo.splitstringByChar(@Pair_IDNewIdOld,';')

	 open _cur
	 fetch next from _cur into @PairNewOld
	 while @@FETCH_STATUS = 0  
	 begin	
		if @PairNewOld!=''
		begin
			select cast (name as uniqueidentifier) as ID, 
			row_number() over( order by (select 1)) as Rn 
			into #temp
			from dbo.splitstring(@PairNewOld)

			update ct set ct.ID_ChiTietGoiDV = (select top 1 ID from #temp where Rn= 1) -- idnew
			from BH_HoaDon_ChiTiet ct
			where ct.ID_ChiTietGoiDV = (select top 1 ID from #temp where Rn= 2) --idold

			drop table #temp
		end	

		FETCH NEXT FROM _cur INTO @PairNewOld 
	 end
	 close _cur
	 deallocate _cur");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDoanhThuSuaChuaTheoCoVan]
    @IdChiNhanhs [nvarchar](max),
    @ThoiGianFrom [datetime],
    @ThoiGianTo [datetime],
    @SoLanTiepNhanFrom [float],
    @SoLanTiepNhanTo [float],
    @SoLuongHoaDonFrom [float],
    @SoLuongHoaDonTo [float],
    @DoanhThuFrom [float],
    @DoanhThuTo [float],
    @LoiNhuanFrom [float],
    @LoiNhuanTo [float],
    @TextSearch [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Insert statements for procedure here
    	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
    	if(@IdChiNhanhs != '')
    	BEGIN
    		insert into @tblDonVi
    		select Name from dbo.splitstring(@IdChiNhanhs);
    	END
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    	Select @count =  (Select count(*) from @tblSearch);
    
    	DECLARE @tblHoaDonSuaChua TABLE (IDCoVan UNIQUEIDENTIFIER, MaNhanVien NVARCHAR(MAX), TenNhanVien NVARCHAR(MAX), 
    	IDPhieuTiepNhan UNIQUEIDENTIFIER, IDHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, DoanhThu FLOAT, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));
    
    	INSERT INTO @tblHoaDonSuaChua
    	SELECT nv.ID, nv.MaNhanVien, nv.TenNhanVien, ptn.ID, hd.ID, hd.NgayLapHoaDon, hd.TongThanhToan - hd.TongTienThue, dv.MaDonVi, dv.TenDonVi
    	FROM Gara_PhieuTiepNhan ptn
    	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
    	INNER JOIN NS_NhanVien nv ON nv.ID = ptn.ID_CoVanDichVu
    	INNER JOIN DM_DonVi dv ON dv.ID = ptn.ID_DonVi
    	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
    	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0
    	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
    	AND ((select count(Name) from @tblSearch b where     			
    			nv.MaNhanVien like '%'+b.Name+'%'
    			or nv.TenNhanVien like '%'+b.Name+'%'
    			)=@count or @count=0);
    
    	DECLARE @tblTienVon TABLE(IDCoVan UNIQUEIDENTIFIER, TienVon FLOAT);
    
    	INSERT INTO @tblTienVon
    	SELECT hdsc.IDCoVan, SUM(ISNULL(hdsc.GiaVon,0)*ISNULL(hdsc.SoLuongxk,0)) AS TienVon
    	FROM (
			SELECT hdsc.IDCoVan, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk
    		FROM @tblHoaDonSuaChua hdsc
    		LEFT JOIN BH_HoaDon xk ON hdsc.IDHoaDon = xk.ID_HoaDon
    		LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
    		WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL
			UNION ALL
			SELECT hdsc.IDCoVan, ISNULL(xkct.GiaVon,0) AS GiaVon, ISNULL(xkct.SoLuong,0) AS SoLuongxk
    		FROM (SELECT IDCoVan, IDPhieuTiepNhan FROM @tblHoaDonSuaChua GROUP BY IDCoVan, IDPhieuTiepNhan ) hdsc
    		INNER JOIN BH_HoaDon xk ON hdsc.IDPhieuTiepNhan = xk.ID_PhieuTiepNhan
    		INNER JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
    		WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) AND xk.ID_HoaDon IS NULL
		) hdsc
    	GROUP BY hdsc.IDCoVan
    
    	DECLARE @SSoLanTiepNhan FLOAT, @SSoLuongHoaDon FLOAT, @STongDoanhThu FLOAT, @STienVon FLOAT, @SLoiNhuan FLOAT;
    
    	DECLARE @tblBaoCaoDoanhThu TABLE(IDCoVan UNIQUEIDENTIFIER, MaNhanVien NVARCHAR(MAX), TenNhanVien NVARCHAR(MAX),
    	SoLanTiepNhan FLOAT, SoLuongHoaDon FLOAT, TongDoanhThu FLOAT, TongTienVon FLOAT, LoiNhuan FLOAT, NgayGiaoDichGanNhat DATETIME, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX))
    	
    	INSERT INTO @tblBaoCaoDoanhThu
    	SELECT hd.IDCoVan, hd.MaNhanVien, hd.TenNhanVien, hd.SoLanTiepNhan, hd.SoLuongHoaDon,
    	ISNULL(hd.TongDoanhThu,0) AS TongDoanhThu, ISNULL(tv.TienVon,0) AS TongTienVon, ISNULL(hd.TongDoanhThu,0) - ISNULL(tv.TienVon,0) AS LoiNhuan, hd.NgayGiaoDichGanNhat, hd.MaDonVi, hd.TenDonVi
    	FROM
    	(
    	SELECT IDCoVan, MaNhanVien, TenNhanVien, MaDonVi, TenDonVi, COUNT(DISTINCT IDPhieuTiepNhan) AS SoLanTiepNhan, COUNT(IDHoaDon) AS SoLuongHoaDon, SUM(DoanhThu) AS TongDoanhThu,
    	MAX(NgayLapHoaDon) AS NgayGiaoDichGanNhat
    	FROM @tblHoaDonSuaChua
    	GROUP BY IDCoVan, MaNhanVien, TenNhanVien, MaDonVi, TenDonVi) AS hd
    	INNER JOIN @tblTienVon tv ON hd.IDCoVan = tv.IDCoVan
    	WHERE (@SoLanTiepNhanFrom IS NULL OR hd.SoLanTiepNhan >= @SoLanTiepNhanFrom)
    	AND (@SoLanTiepNhanTo IS NULL OR hd.SoLanTiepNhan <= @SoLanTiepNhanTo)
    	AND (@SoLuongHoaDonFrom IS NULL OR hd.SoLuongHoaDon >= @SoLuongHoaDonFrom)
    	AND (@SoLuongHoaDonTo IS NULL OR hd.SoLuongHoaDon <= @SoLuongHoaDonTo)
    	AND (@DoanhThuFrom IS NULL OR hd.TongDoanhThu >= @DoanhThuFrom)
    	AND (@DoanhThuTo IS NULL OR hd.TongDoanhThu <= @DoanhThuTo)
    	AND (@LoiNhuanFrom IS NULL OR hd.TongDoanhThu - tv.TienVon >= @LoiNhuanFrom)
    	AND (@LoiNhuanTo IS NULL OR hd.TongDoanhThu - tv.TienVon <= @LoiNhuanTo)
    
    	SELECT @SSoLanTiepNhan = SUM(SoLanTiepNhan), @SSoLuongHoaDon = SUM(SoLuongHoaDon), @STongDoanhThu = SUM(TongDoanhThu), @STienVon = SUM(TongTienVon), @SLoiNhuan = SUM(LoiNhuan) FROM @tblBaoCaoDoanhThu
    
    	SELECT *, CAST(@SSoLanTiepNhan AS FLOAT) AS SSoLanTiepNhan, @SSoLuongHoaDon AS SSoLuongHoaDon, @STongDoanhThu AS STongDoanhThu, @STienVon AS STienVon, @SLoiNhuan AS SLoiNhuan FROM @tblBaoCaoDoanhThu
    	ORDER BY TenNhanVien
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_BieuDoDoanhThuToDay]
    @timeStart [datetime],
    @timeEnd [datetime],
	@ID_NguoiDung [uniqueidentifier],
	@ID_DonVi nvarchar (max)
AS
BEGIN
	 DECLARE @LaAdmin as nvarchar
    	Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd	where nd.ID = @ID_NguoiDung)
	 IF(@LaAdmin = 1)
	 BEGIN
		SELECT 
			a.NgayLapHoaDon,
			a.TenChiNhanh,
			CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
			FROM
			(
				-- tongmua
    			SELECT
    			hdb.ID as ID_HoaDon,
				DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
    			hdb.PhaiThanhToan - isnull(hdb.TongTienThue,0) as ThanhTien 
    			FROM
    			BH_HoaDon hdb
				join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    			where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    			and hdb.ChoThanhToan = 0
    			and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
				and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

				union all
				-- tongtra
				SELECT
    			hdb.ID as ID_HoaDon,
				DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
    			- hdb.PhaiThanhToan - isnull(hdb.TongTienThue,0) as ThanhTien 
    			FROM
    			BH_HoaDon hdb
				join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    			where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
				and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))				
			) a
    		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
			ORDER BY NgayLapHoaDon
	END
	ELSE
	BEGIN
		SELECT 
			a.NgayLapHoaDon,
			a.TenChiNhanh,
			CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
			FROM
			(
				-- tongmua
    			SELECT
    			hdb.ID as ID_HoaDon,
				DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
    			hdb.PhaiThanhToan - isnull(hdb.TongTienThue,0) as ThanhTien
    			FROM
    			BH_HoaDon hdb
				join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    			where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    			and hdb.ChoThanhToan = 0
    			and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    			and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
				and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

				union all
				-- tongtra
    			SELECT
    			hdb.ID as ID_HoaDon,
				DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
    			-hdb.PhaiThanhToan - isnull(hdb.TongTienThue,0) as ThanhTien
    			FROM
    			BH_HoaDon hdb
				join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    			where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    			and hdb.ChoThanhToan = 0
    			and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    			and hdb.LoaiHoaDon = 6
				and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))
			) a
    		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
			ORDER BY NgayLapHoaDon
	END

END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_DoanhThuChiNhanh]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	DECLARE @LaAdmin as nvarchar
    	Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd	where nd.ID = @ID_NguoiDung)
	 IF(@LaAdmin = 1)
	 BEGIN
		SELECT 
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
			-- tongmua
    		SELECT
			dv.TenDonVi as TenChiNhanh,
    		hdb.PhaiThanhToan - isnull(hdb.TongTienThue,0) as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)

			union all
			-- tongtra
    		SELECT
			dv.TenDonVi as TenChiNhanh,
    		-hdb.PhaiThanhToan  - isnull(hdb.TongTienThue,0)as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.LoaiHoaDon = 6
		) a
    	GROUP BY a.TenChiNhanh
	END
	ELSE
	BEGIN
		SELECT 
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
			--tongban
    		SELECT
			dv.TenDonVi as TenChiNhanh,
    		hdb.PhaiThanhToan - isnull(hdb.TongTienThue,0) as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
			and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)

			union all
			-- tongtra
			SELECT
			dv.TenDonVi as TenChiNhanh,
    		- hdb.PhaiThanhToan  - isnull(hdb.TongTienThue,0)as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
			and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    		and hdb.LoaiHoaDon = 6
		) a
    	GROUP BY a.TenChiNhanh
	END
END");

			Sql(@"ALTER PROCEDURE [dbo].[getListHangHoaLoHang_EnTer]
    @MaHH [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    	Select TOP(40)
		dvqd1.ID as ID_DonViQuiDoi,
    	dhh1.ID,
		dvqd1.MaHangHoa,
    	dhh1.TenHangHoa,
		dvqd1.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    	dvqd1.TenDonViTinh,
		dhh1.QuanLyTheoLoHang,
		dvqd1.TyLeChuyenDoi,
		dhh1.LaHangHoa,
		Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
		dvqd1.GiaBan,
		dvqd1.GiaNhap,
		ISNULL(hhtonkho.TonKho,0) as TonKho,
		ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
		Case when lh1.ID is null then null else lh1.ID end as ID_LoHang,
		lh1.MaLoHang,
    	lh1.NgaySanXuat,
		lh1.NgayHetHan,
		case when ISNULL(dhh1.QuyCach,0) = 0 then dvqd1.TyLeChuyenDoi else dhh1.QuyCach * dvqd1.TyLeChuyenDoi end as QuyCach
    	from
    	DonViQuiDoi dvqd1
    	left join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
		LEFT join DM_HangHoa_Anh an on (dvqd1.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    	left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa and (lh1.TrangThai = 1 or lh1.TrangThai is null)
		left join DM_GiaVon gv on dvqd1.ID = gv.ID_DonViQuiDoi and (lh1.ID = gv.ID_LoHang or lh1.ID is null) and gv.ID_DonVi = @ID_ChiNhanh
		left join DM_HangHoa_TonKho hhtonkho on dvqd1.ID = hhtonkho.ID_DonViQuyDoi and (hhtonkho.ID_LoHang = lh1.ID or lh1.ID is null) and hhtonkho.ID_DonVi = @ID_ChiNhanh
		where dvqd1.Xoa = 0 and dvqd1.MaHangHoa = @MaHH
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListPhieuNhapXuatKhoByIDPhieuTiepNhan]
    @IDPhieuTiepNhan [uniqueidentifier],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Insert statements for procedure here
    	IF(@PageSize != 0)
    	BEGIN
    		with data_cte
    		as
    		(select pxk.ID, pxk.LoaiHoaDon, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.ID AS ID_HoaDonSuaChua, hdsc.MaHoaDon AS HoaDonSuaChua, hdsc.ChoThanhToan AS TrangThaiHoaDonSuaChua, SUM(pxkct.SoLuong) AS SoLuong, SUM(pxkct.ThanhTien) AS GiaTri from BH_HoaDon pxk
    		LEFT JOIN BH_HoaDon hdsc ON pxk.ID_HoaDon = hdsc.ID
    		INNER JOIN BH_HoaDon_ChiTiet pxkct ON pxk.ID = pxkct.ID_HoaDon
    		where pxk.ID_PhieuTiepNhan = @IDPhieuTiepNhan
    		AND pxk.LoaiHoaDon = '8' AND pxk.ChoThanhToan = 0
    		GROUP BY pxk.ID, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.MaHoaDon, pxk.LoaiHoaDon, hdsc.ID, hdsc.ChoThanhToan),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    
    		SELECT * FROM data_cte dt
    		CROSS JOIN count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
    	END
    	ELSE
    	BEGIN
    		with data_cte
    		as
    		(select pxk.ID, pxk.LoaiHoaDon, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.ID AS ID_HoaDonSuaChua, hdsc.MaHoaDon AS HoaDonSuaChua, hdsc.ChoThanhToan AS TrangThaiHoaDonSuaChua, SUM(pxkct.SoLuong) AS SoLuong, SUM(pxkct.ThanhTien) AS GiaTri from BH_HoaDon pxk
    		LEFT JOIN BH_HoaDon hdsc ON pxk.ID_HoaDon = hdsc.ID
    		INNER JOIN BH_HoaDon_ChiTiet pxkct ON pxk.ID = pxkct.ID_HoaDon
    		where pxk.ID_PhieuTiepNhan = @IDPhieuTiepNhan
    		AND pxk.LoaiHoaDon = '8' AND pxk.ChoThanhToan = 0
    		GROUP BY pxk.ID, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.MaHoaDon, pxk.LoaiHoaDon, hdsc.ID, hdsc.ChoThanhToan),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    
    		SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
    		order by dt.NgayLapHoaDon desc
    	END
END");

			Sql(@"ALTER PROCEDURE [dbo].[PhieuTiepNhan_GetThongTinChiTiet]
    @ID_PhieuTiepNhan [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;	
    	
    	select tn.*,		
    		xe.BienSo, xe.SoKhung, xe.SoMay,
    		xe.DungTich, xe.HopSo, xe.MauSon, xe.NamSanXuat, 
    		nvlap.TenNhanVien as NhanVienTiepNhan,
    		nvlap.MaNhanVien as MaNVTiepNhan,
    		ISNULL(nv.TenNhanVien,'') as CoVanDichVu,
    		ISNULL(cv.DienThoaiDiDong,'') as CoVan_SDT,
    		ISNULL(nv.MaNhanVien,'') as MaCoVan,
    		ISNULL(nv.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
    		ISNULL(dt.MaDoiTuong,'') as MaDoiTuong,
    		isnull(dt.TenDoiTuong,'') as TenDoiTuong,
    		isnull(dt.TenDoiTuong_KhongDau,'') as TenDoiTuong_KhongDau,
    		dt.DienThoai as DienThoaiKhachHang,
    		dt.DiaChi,
    		dt.Email,
    		cast(iif(xe.ID_KhachHang = tn.ID_KhachHang,'1','0') as bit) as LaChuXe,
    		cx.TenDoiTuong as ChuXe,
    		cx.DienThoai as ChuXe_SDT,
    		cx.Email as ChuXe_Email,
    		cx.DiaChi as ChuXe_DiaChi,
    		mau.TenMauXe,
    		hang.TenHangXe,
    		loai.TenLoaiXe,
			tn.ID_BaoHiem,
			tn.NguoiLienHeBH,
			tn.SoDienThoaiLienHeBH,
			ISNULL(bh.TenDoituong,'') as TenBaoHiem,
			ISNULL(bh.MaDoiTuong,'') as MaBaoHiem   	
    	from Gara_PhieuTiepNhan tn
    	join Gara_DanhMucXe xe on tn.ID_Xe = xe.ID
    	join Gara_MauXe mau on xe.ID_MauXe = mau.ID
    	join Gara_HangXe hang on mau.ID_HangXe= hang.ID
    	join Gara_LoaiXe loai on mau.ID_LoaiXe= loai.ID
    	join NS_NhanVien nvlap on tn.ID_NhanVien= nvlap.ID
    	left join NS_NhanVien cv on tn.ID_CoVanDichVu= cv.ID
    	left join DM_DoiTuong dt on tn.ID_KhachHang = dt.ID
    	left join DM_DoiTuong cx on xe.ID_KhachHang = cx.ID
		left join DM_DoiTuong bh on tn.ID_BaoHiem= bh.ID
    	left join NS_NhanVien nv on tn.ID_CoVanDichVu= nv.ID
    	where tn.id= @ID_PhieuTiepNhan
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_InsertChietKhauHoaDonTraHang_NhanVien]
    @ID_HoaDon [nvarchar](max),
    @TongTienTra [float],
    @ID_HoaDonTra [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
set nocount on;
    declare @count_CKTraHang int
    	--- check xem co cai dat chiet khau TraHang khong
    	select @count_CKTraHang = count(hd.ID)
    	from ChietKhauMacDinh_HoaDon hd
    	where hd.ID_DonVi like @ID_DonVi
    	and hd.TrangThai !='0' and  hd.ChungTuApDung like '%6%'
    
    	if	@count_CKTraHang > 0
    		begin		
    			-- get PhaiThanhToan from HDMua --> chia % de tinh lai ChietKhau (theo VND)
    			declare @PhaiThanhToan float
    			select @PhaiThanhToan = TongThanhToan - isnull(TongTienThue,0) from BH_HoaDon where ID like @ID_HoaDon
    			
    			-- copy data from BH_NhanVienThucHien (HDMua) to BH_NhanVienThucHien (HDTra) with new {ID_HoaDon, TienChietKhau}
			insert into BH_NhanVienThucHien (ID, ID_NhanVien,  PT_ChietKhau, TinhChietKhauTheo, HeSo,ID_HoaDon,ThucHien_TuVan, TheoYeuCau, TienChietKhau)	
				
				select 
					NewID() as ID,
					ID_NhanVien,
					PT_ChietKhau,
					TinhChietKhauTheo,
					HeSo,
					@ID_HoaDonTra,
					ThucHien_TuVan,
					TheoYeuCau,
					case when TinhChietKhauTheo !=3 then (@TongTienTra * PT_ChietKhau *HeSo)/100
    				else (TienChietKhau/@PhaiThanhToan) * @TongTienTra end  as TienChietKhau
				from BH_NhanVienThucHien th
				where ID_HoaDon like @ID_HoaDon
				and TinhChietKhauTheo!=1 -- khong lay ck theothucthu

				--select 
				--	NewID() as ID,
				--	ID_NhanVien,
				--	PT_ChietKhau,
				--	TinhChietKhauTheo,
				--	HeSo,
				--	@ID_HoaDonTra,
				--	ThucHien_TuVan,
				--	TheoYeuCau,
				--	case when TinhChietKhauTheo !=3 then (@TongTienTra * PT_ChietKhau * HeSo)/100
    --				else (TienChietKhau/@PhaiThanhToan) * @TongTienTra end  as TienChietKhau
				--from
				--(
				--select 
				--	ID_NhanVien,PT_ChietKhau,TinhChietKhauTheo,HeSo,ThucHien_TuVan,TheoYeuCau,
				--	case when TinhChietKhauTheo !=3 then (@TongTienTra * PT_ChietKhau *HeSo)/100
    --				else (TienChietKhau/@PhaiThanhToan) * @TongTienTra end  as TienChietKhau
				--from BH_NhanVienThucHien th
				--where ID_HoaDon like @ID_HoaDon
				--and TinhChietKhauTheo!=1 -- khong lay ck theothucthu

				----union all
				----select 
				----	ID_NhanVien,PT_ChietKhau,TinhChietKhauTheo,HeSo,ThucHien_TuVan,TheoYeuCau,TienChietKhau
				----from(
				----select 
				----	th.*, ROW_NUMBER() over (partition by th.ID_NhanVien order by th.ID_NhanVien, TienChietKhau desc) as Rn
				----from BH_NhanVienThucHien th
				----where ID_HoaDon like @ID_HoaDon
				----and TinhChietKhauTheo =1 -- thucthu: neu thanhtoan nhieulan, lay ck lon nhat
				----) a where Rn= 1
				--) b
    		end
END");

            Sql(@"ALTER FUNCTION [dbo].[GetIDNhanVien_inPhongBan] (
	@ID_NhanVien UNIQUEIDENTIFIER , 
	@IDDonVi varchar(max), 
	@MaQuyenXemPhongBan varchar(100),
	@MaQuyenXemHeThong varchar(100))
RETURNS
 @tblNhanVien TABLE (ID UNIQUEIDENTIFIER)
AS
BEGIN
	
	DECLARE @tblDonVi TABLE (ID UNIQUEIDENTIFIER);
	insert into @tblDonVi
	select Name from dbo.splitstring(@IDDonVi)

	declare @LaAdmin bit=( select LaAdmin from HT_NguoiDung where ID_NhanVien = @ID_NhanVien)

	declare @countAll int = (SELECT count(*)
	FROM HT_NguoiDung_Nhom nnd
	JOIN HT_Quyen_Nhom qn on nnd.IDNhomNguoiDung = qn.ID_NhomNguoiDung
	JOIN HT_NguoiDung htnd on nnd.IDNguoiDung= htnd.ID
	where htnd.ID_NhanVien= @ID_NhanVien and qn.MaQuyen= @MaQuyenXemHeThong 
	and  exists (select ID from @tblDonVi dv where nnd.ID_DonVi = dv.ID) )

	-- get list phongban congtac
	DECLARE @tblPhongBan TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblPhongBan
	select ID_PhongBan
	from NS_QuaTrinhCongTac ct where ID_NhanVien= @ID_NhanVien 
	and exists (select ID from @tblDonVi dv where ct.ID_DonVi = dv.ID)

	DECLARE @tblPhongBanTemp TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblPhongBanTemp
	select ID_PhongBan from NS_QuaTrinhCongTac ct where ID_NhanVien= @ID_NhanVien 
	and exists (select ID from @tblDonVi dv where ct.ID_DonVi = dv.ID)

	if @LaAdmin ='1' or @countAll > 0
	begin
		-- phongban in hethong
		INSERT INTO @tblPhongBan
		SELECT ID FROM NS_PhongBan pb 
		where (exists (select ID from @tblDonVi dv where pb.ID_DonVi = dv.ID) or ID_DonVi is null); 

		-- get allNVien (lay ca NV khong thuoc chi nhanh)
		INSERT INTO @tblNhanVien
		select distinct ct.ID_NhanVien
		from NS_QuaTrinhCongTac ct

	end
	else	
		begin
			declare @countByPhong int = (SELECT count(*)
			FROM HT_NguoiDung_Nhom nnd
			JOIN HT_Quyen_Nhom qn on nnd.IDNhomNguoiDung = qn.ID_NhomNguoiDung
			JOIN HT_NguoiDung htnd on nnd.IDNguoiDung= htnd.ID
			where htnd.ID_NhanVien= @ID_NhanVien and qn.MaQuyen= @MaQuyenXemPhongBan 
			and  exists (select ID from @tblDonVi dv where nnd.ID_DonVi = dv.ID) )

			if @countByPhong > 0
				begin
					DECLARE @intFlag INT;
					SET @intFlag = 1;
					WHILE (@intFlag != 0)
					BEGIN
						SELECT @intFlag = COUNT(ID) FROM NS_PhongBan pb
						WHERE ID_PhongBanCha IN (SELECT ID FROM @tblPhongBanTemp) 
						IF(@intFlag != 0)
						BEGIN
							-- get phongban con
							INSERT INTO @tblPhongBanTemp
							SELECT ID FROM NS_PhongBan pb WHERE ID_PhongBanCha IN (SELECT ID FROM @tblPhongBanTemp) 
							DELETE FROM @tblPhongBanTemp WHERE ID IN (SELECT ID FROM @tblPhongBan);
							INSERT INTO @tblPhongBan
							SELECT ID FROM @tblPhongBanTemp
						END
					END

					INSERT INTO @tblNhanVien
					select distinct ct.ID_NhanVien 
					from NS_QuaTrinhCongTac ct					
					where exists (select ID from @tblPhongBan pb where pb.ID= ct.ID_PhongBan)
				end
			else
				INSERT INTO @tblNhanVien values (@ID_NhanVien)
		end		
	RETURN
END");


        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[CTHD_GetDichVubyDinhLuong]");
			DropStoredProcedure("[dbo].[GetCTHDSuaChua_afterXuatKho]");
			DropStoredProcedure("[dbo].[HDSC_GetChiTietXuatKho]");
			DropStoredProcedure("[dbo].[InsertChietKhauTraHang_TheoThucThu]");
			DropStoredProcedure("[dbo].[UpdateIDCTNew_forCTOld]");
        }
    }
}
