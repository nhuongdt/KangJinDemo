namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpUpdate_20210414 : DbMigration
    {
		public override void Up()
		{
			CreateStoredProcedure(name: "[dbo].[CheckMaDoiTuong_Exist]", parametersAction: p => new {
				MaDoiTuong = p.String()
			}, body: @"DECLARE @valReturn bit ='0'
    	DECLARE @ID_DoiTuong nvarchar(max);
    	SELECT @ID_DoiTuong= ID from DM_DoiTuong WHERE MaDoiTuong like @MaDoiTuong
    
    	IF @ID_DoiTuong IS NULL SET @valReturn= '0'
    ELSE SET @valReturn= '1'
    
    	SELECT @valReturn AS Exist");

			CreateStoredProcedure(name: "[dbo].[GetDSGoiDichVu_ofKhachHang]", parametersAction: p => new
			{
				ID_KhachHang = p.String(),
				ID_DonVi = p.String(50)
			}, body: @"SET NOCOUNT ON;
   select  
		hd.ID as ID_GoiDV, MaHoaDon, 
		convert(varchar,hd.NgayLapHoaDon, 103) as NgayLapHoaDon,
    	convert(varchar,hd.NgayApDungGoiDV, 103) as NgayApDungGoiDV,
    	convert(varchar,hd.HanSuDungGoiDV, 103) as HanSuDungGoiDV,
		ctm.ID as ID_ChiTietGoiDV, ctm.ID_DonViQuiDoi, ctm.ID_LoHang, 
		ISNULL(ctm.ID_TangKem, '00000000-0000-0000-0000-000000000000') as ID_TangKem, ISNULL(ctm.TangKem,'0') as TangKem, 
		ctm.DonGia - ctm.TienChietKhau as GiaBan,
		ctm.SoLuong, 
		ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) as SoLuongMua,
		ISNULL(ctt.SoLuongDung,0) as SoLuongDung,
		ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) - ISNULL(ctt.SoLuongDung,0) as SoLuongConLai,		
		qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
		hh.LaHangHoa, hh.TenHangHoa, CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
		ISNULL(hh.ID_NhomHang,'00000000-0000-0000-0000-000000000001') as ID_NhomHangHoa,
		ISNULL(hh.SoPhutThucHien,0) as SoPhutThucHien,
		case when hh.LaHangHoa ='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
		Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
		ISNULL(hh.GhiChu,'') as GhiChuHH,
		hh.QuanLyTheoLoHang,
		lo.MaLoHang, lo.NgaySanXuat, lo.NgayHetHan
	from BH_HoaDon_ChiTiet ctm
	join BH_HoaDon hd on ctm.ID_HoaDon = hd.ID
	join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	left join DM_LoHang lo on ctm.ID_LoHang = lo.ID
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
			where hd.ChoThanhToan='0' and hd.LoaiHoaDon = 6
			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
			group by ct.ID_ChiTietGoiDV

			union all
			-- sum soluong sudung
			select ct.ID_ChiTietGoiDV,
				0 as SoLuongDung,
				SUM(ct.SoLuong) as SoLuongDung
			from BH_HoaDon_ChiTiet ct 
			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
			where hd.ChoThanhToan='0' and hd.LoaiHoaDon = 1
			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
			group by ct.ID_ChiTietGoiDV
			) a group by a.ID_ChiTietGoiDV
	) ctt on ctm.ID = ctt.ID_ChiTietGoiDV
	where hd.LoaiHoaDon = 19 and hd.ID_DoiTuong = @ID_KhachHang
	and hd.ChoThanhToan='0'
	order by hd.NgayLapHoaDon desc");

			CreateStoredProcedure(name: "[dbo].[GetInforBasic_DoiTuongByID]", parametersAction: p => new
			{
				ID_DoiTuong = p.String()
			}, body: @"SET NOCOUNT ON;
    select dt.ID,
			dt.MaDoiTuong, 
			dt.TenDoiTuong, 
			dt.TenDoiTuong_KhongDau,
			dt.TenDoiTuong_ChuCaiDau,
			dt.GioiTinhNam,
			dt.NgaySinh_NgayTLap,
			dt.DienThoai,
			dt.Email,
			dt.DiaChi,
			dt.MaSoThue,
			dt.GhiChu,
			dt.NgayTao,
			dt.NguoiTao,
			dt.ID_NguonKhach,
			dt.ID_NhanVienPhuTrach,
			dt.ID_NguoiGioiThieu,
			dt.LaCaNhan,
			dt.ID_TinhThanh,
			dt.ID_QuanHuyen,
			dt.ID_TrangThai,
    		ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
    		ISNULL(tt.TenTinhThanh,'') as KhuVuc,
    		ISNULL(qh.TenQuanHuyen,'') as PhuongXa,
    		ISNULL(nk.TenNguonKhach,'') as TenNguonKhach,
    		dt.TenNhomDoiTuongs as TenNhomDT
    	from DM_DoiTuong dt
    	left join DM_TinhThanh tt on  dt.ID_TinhThanh = tt.ID
    	left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    	left join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
    	left join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID   	
    	where dt.ID like @ID_DoiTuong");

            CreateStoredProcedure(name: "[dbo].[GetInforMuaHang_ofKhachHang]", parametersAction: p => new
            {
                ID = p.Guid()
            }, body: @"SET NOCOUNT ON;
    SELECT 
    		--c.ID,
    		max(c.MaDoiTuong) as MaDoiTuong,
    		max(c.ID_NhomDoiTuong) as ID_NhomDoiTuong,
    		max(c.TenDoiTuong) as TenDoiTuong,
    		max(c.TongTichDiem) as TongTichDiem,
    		max(c.NoHienTai) as NoHienTai,
    		max(c.TongBan) as TongBan,
    		max(c.TongBanTruTraHang) as TongBanTruTraHang,
    		max(c.TongMua) as TongMua,
    		max(c.SoLanMuaHang) as SoLanMuaHang
    	FROM
    	(
    SELECT * 
    		FROM
    		(
    		  SELECT 
    		  dt.ID as ID,
    		  dt.MaDoiTuong, 
			  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
    		  ISNULL(dt.TongTichDiem,0) as TongTichDiem,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang
    		  FROM
    			DM_DoiTuong dt
    				left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				left join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    			    LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    			LEFT Join
    			(
    			  SELECT HangHoa.ID_KhachHang,
    				SUM(ISNULL(HangHoa.NoHienTai, 0)) as NoHienTai, 
    				SUM(ISNULL(HangHoa.TongBan, 0)) as TongBan,
    				SUM(ISNULL(HangHoa.TongBanTruTraHang, 0)) as TongBanTruTraHang,
    				SUM(ISNULL(HangHoa.TongMua, 0)) as TongMua,
    					SUM(ISNULL(HangHoa.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
    					SELECT
    					td.ID_DoiTuong AS ID_KhachHang,
    					SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
    							NULL AS TongBan,
    							NULL AS TongBanTruTraHang,
    							NULL AS TongMua,
    						NULL AS SoLanMuaHang
    					FROM
    					(
    						SELECT 
    						bhd.ID_DoiTuong,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false'
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    							NULL AS CongNo,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'
    						GROUP BY bhd.ID_DoiTuong
    						-- sổ quỹ
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    						NULL AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    						GROUP BY qhdct.ID_DoiTuong
    
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    						GROUP BY qhdct.ID_DoiTuong
    					) AS td
    						GROUP BY td.ID_DoiTuong
    						UNION ALL
    							-- Tổng bán phát sinh trong khoảng thời gian truy vấn
    						SELECT
    						pstv.ID_DoiTuong AS ID_KhachHang,
    						NULL AS NoHienTai,
    						SUM(ISNULL(pstv.DoanhThu,0)) AS TongBan,
    						SUM(ISNULL(pstv.DoanhThu,0)) -  SUM(ISNULL(pstv.GiaTriTra,0)) AS TongBanTruTraHang,
    						SUM(ISNULL(pstv.GiaTriTra,0)) AS TongMua,
    							NULL AS SoLanMuaHang
    						FROM
    						(
    						SELECT 
    						bhd.ID_DoiTuong,
    						NULL AS GiaTriTra,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi,
    							NULL AS SoLanMuaHang
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' 
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi, 
    							NULL AS SoLanMuaHang
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'
    						GROUP BY bhd.ID_DoiTuong
    						) AS pstv
    						GROUP BY pstv.ID_DoiTuong
    			
    							Union All
    							Select 
    							hd.ID_DoiTuong AS ID_KhachHang,
    							NULL AS NoHienTai,
    						NULL AS TongBan,
    						NULL AS TongBanTruTraHang,
    						NULL AS TongMua,
    							COUNT(*) AS SoLanMuaHang
    							From BH_HoaDon hd 
    							where hd.LoaiHoaDon = 1
    							and hd.ChoThanhToan = 0
    
    							GROUP BY hd.ID_DoiTuong
    					)AS HangHoa
    						GROUP BY HangHoa.ID_KhachHang
    				) a
    					on dt.ID = a.ID_KhachHang
    				)b
    				where ID like @ID
    				) c");

            CreateStoredProcedure(name: "[dbo].[GetInforSoQuy_ByID]", parametersAction: p => new
            {
                ID_PhieuThuChi = p.String()
            }, body: @"SET NOCOUNT ON;
    select 
    		qhd.ID,
    		qhd.MaHoaDon,
    		qhd.NgayLapHoaDon,
			qhd.PhieuDieuChinhCongNo,
			qhd.TrangThai,
			case when sum(qct.TienThu)=0 and sum(ISNULL(qct.DiemThanhToan,0)) > 0 then 1 else 0 end as PhieuDieuChinhDiem,
    		MAX(qhd.LoaiHoaDon) as LoaiHoaDon ,
			case when sum(isnull(qct.TienThu,0))=0 then sum(ISNULL(qct.DiemThanhToan,0)) else sum(isnull(qct.TienThu,0)) end as TongTienThu,
    		MAX(ISNULL(qhd.NoiDungThu,'')) as NoiDungThu,
    		MAX(ISNULL(nv.TenNhanVien,'')) as TenNhanVien,
    		MAX(ISNULL(dt.TenDoiTuong,'')) as NguoiNopTien
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    	left join NS_NhanVien nv on qhd.ID_NhanVien= nv.ID
    	left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID
    	where qhd.ID like @ID_PhieuThuChi 
    	group by qhd.ID, qhd.MaHoaDon,qhd.NgayLapHoaDon,qhd.LoaiHoaDon,qhd.PhieuDieuChinhCongNo, qhd.TrangThai");

            CreateStoredProcedure(name: "[dbo].[GetListHoaDon_UseService]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"SET NOCOUNT ON;
    select 
    	hdsd.ID ,hdsd.MaHoaDon,
    	convert(varchar,hdsd.NgayLapHoaDon, 103) as NgayLapHoaDon,
    	CAST(hdsd.TongTienHang - ISNULL(hdsd.TongGiamGia,0) - ISNULL(hdsd.KhuyeMai_GiamGia,0) as float) as TongTienHang,
    	CAST(ISNULL(hdsd.PhaiThanhToan,0) AS float) AS PhaiThanhToan,
    	hdsd.DienGiai as GhiChu,
    	CAST(ISNULL(tblQuyHD.TongTienThu,0) AS float) AS DaThanhToan
    
    from BH_HoaDon hdsd
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
    order by hdsd.NgayLapHoaDon ");

            CreateStoredProcedure(name: "[dbo].[GetMaDoiTuong_Max]", parametersAction: p => new
            {
                LoaiDoiTuong = p.Int()
            }, body: @"DECLARE @MaDTuongOffline varchar(5);
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

	-- get list DoiTuong not offline
	SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaDoiTuong) AS float))
	FROM DM_DoiTuong WHERE LoaiDoiTuong = @LoaiDoiTuong and CHARINDEX(@MaDTuongOffline,MaDoiTuong)=0 AND CHARINDEX(@MaDTuongTemp,MaDoiTuong)=1 

	if	@Return is null 
		select Cast(0 as float) as MaxCode
	else 
		select @Return as MaxCode");

            CreateStoredProcedure(name: "[dbo].[GetMaLienHe_Max]", parametersAction: p => new
            {

            }, body: @"set nocount on;
		DECLARE @Return float

		SELECT @Return =  MAX(CAST (dbo.udf_GetNumeric(MaLienHe) AS float))
    	FROM DM_LienHe WHERE CHARINDEX('NLH',MaLienHe)= 1

		if	@Return is null 
			select Cast(0 as float) as MaxCode
		else 
			select @Return as MaxCode");

            CreateStoredProcedure(name: "[dbo].[GetSoDuTheGiaTri_ofKhachHang]", parametersAction: p => new
            {
                ID_DoiTuong = p.Guid(),
                DateTime = p.DateTime()
            }, body: @"SET NOCOUNT ON;
	set @DateTime= DATEADD(DAY,1,@DateTime)
	select 
		TongThuTheGiaTri, SuDungThe, HoanTraTheGiatri,
		ThucThu,PhaiThanhToan,SoDuTheGiaTri,
		iif(CongNoThe<0,0,CongNoThe) as CongNoThe
	from
	(
	select 		
		cast(sum(TongThuTheGiaTri) as float) as TongThuTheGiaTri, 
		cast(sum(SuDungThe) as float) as SuDungThe,
		cast(sum(HoanTraTheGiatri) as float) as HoanTraTheGiatri,
		cast(sum(ThucThu) as float) as ThucThu,
		cast(sum(PhaiThanhToan) as float) as PhaiThanhToan,
		cast(SUM(TongThuTheGiaTri)  - SUM(SuDungThe) + SUM(HoanTraTheGiatri) as float) as SoDuTheGiaTri,
		cast(sum(PhaiThanhToan)- sum(ThucThu) as float) as CongNoThe
	from (
		-- so du nap the va thuc te phai thanh toan
		SELECT 
			TongTienHang as TongThuTheGiaTri,
			0 as SuDungThe,
			0 as HoanTraTheGiatri,
			0 as ThucThu,
			hd.PhaiThanhToan -- dieu chinh the (khong lien quan den cong no)
		FROM BH_HoaDon hd
		where hd.ID_DoiTuong like @ID_DoiTuong and hd.ChoThanhToan ='0' and hd.LoaiHoaDon in (22,23) 
		and hd.NgayLapHoaDon  < @DateTime

		union all
		-- su dung the
		SELECT 
			0 as TongThuTheGiaTri,
			SUM(qct.TienThu) as SuDungThe,
			0 as HoanTraTheGiatri,
			0 as ThucThu,
			0 as PhaiThanhToan			 
		FROM Quy_HoaDon_ChiTiet qct
		INNER JOIN Quy_HoaDon qhd
		ON qct.ID_HoaDon = qhd.ID
		WHERE qct.ID_DoiTuong like @ID_DoiTuong AND qhd.NgayLapHoaDon  < @DateTime and qhd.LoaiHoaDon = 11 
		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
		and qct.HinhThucThanhToan=4
    	
		union all
		-- hoan tra the
		SELECT
			0 as TongThuTheGiaTri,
			0 as SuDungThe,
			SUM(qct.TienThu) as HoanTraTheGiatri,
			0 as ThucThu,
			0 as PhaiThanhToan	
		FROM Quy_HoaDon_ChiTiet qct
		INNER JOIN Quy_HoaDon qhd
		ON qct.ID_HoaDon = qhd.ID
		WHERE qct.ID_DoiTuong like @ID_DoiTuong AND qhd.NgayLapHoaDon  < @DateTime and qhd.LoaiHoaDon = 12
		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
			and qct.HinhThucThanhToan=4

		union all
		-- thuc thu thegiatri
		SELECT
			0 as TongThuTheGiaTri,
			0 as SuDungThe,
			0 as HoanTraTheGiatri,
			qct.TienThu as ThucThu,
			0 as PhaiThanhToan	
		from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
		join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
		where hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and qhd.NgayLapHoaDon < @DateTime and qct.ID_DoiTuong like @ID_DoiTuong
		and (qhd.PhieuDieuChinhCongNo= 0 or PhieuDieuChinhCongNo  is  null)

		-- thucthu do dieuchinh congno khachhang
		union all
		select
			0 as TongThuTheGiaTri,
			0 as SuDungThe,
			0 as HoanTraTheGiatri,
			qct.TienThu as ThucThu,
			0 as PhaiThanhToan	
		from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
		where qhd.PhieuDieuChinhCongNo= 1 and qhd.LoaiHoaDon= 11
		and (qhd.TrangThai= 1 or qhd.TrangThai is null)
		and qct.ID_DoiTuong like @ID_DoiTuong
		) tbl  
		) tbl2");

			CreateStoredProcedure(name: "[dbo].[ReportDiscountInvoice]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				ID_NhanVienLogin = p.String(),
				TextSearch = p.String(),
				DateFrom = p.String(),
				DateTo = p.String(),
				Status_ColumHide = p.Int(),
				StatusInvoice = p.Int(),
				Status_DoanhThu = p.Int(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	set @DateTo = dateadd(day,1, @DateTo) 

	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tblDiscountInvoice table (ID uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), 
		HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float)
	
	-- bang tam chua DS phieu thu theo Ngay timkiem
	select qct.ID_HoaDonLienQuan, SUM(qct.TienThu) as ThucThu, qhd.NgayLapHoaDon, qhd.ID
	into #temp
	from Quy_HoaDon_ChiTiet qct
	join (
			select qhd.ID, qhd.NgayLapHoaDon
			from Quy_HoaDon qhd
			join BH_NhanVienThucHien th on qhd.ID= th.ID_QuyHoaDon
			where (qhd.TrangThai is null or qhd.TrangThai = '1')
			and qhd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
			and qhd.NgayLapHoaDon >= @DateFrom
			and qhd.NgayLapHoaDon < @DateTo
			group by qhd.ID, qhd.NgayLapHoaDon) qhd on qct.ID_HoaDon = qhd.ID
	where ISNULL(qct.DiemThanhToan,0) = 0 and  ISNULL(qct.ThuTuThe,0) = 0
	group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID;
	
	select ID, MaNhanVien, 
    			TenNhanVien,
    		SUM(ISNULL(HoaHongThucThu,0.0)) as HoaHongThucThu,
    		SUM(ISNULL(HoaHongDoanhThu,0.0)) as HoaHongDoanhThu,
    		SUM(ISNULL(HoaHongVND,0.0)) as HoaHongVND,			
    		case @Status_ColumHide
    			when  1 then cast(0 as float)
    			when  2 then SUM(ISNULL(HoaHongVND,0.0))
    			when  3 then SUM(ISNULL(HoaHongThucThu,0.0))
    			when  4 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    			when  5 then SUM(ISNULL(HoaHongDoanhThu,0.0)) 
    			when  6 then SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    			when  7 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0))
    		else SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
    		end as TongAll
		into #temp2
    	from 
    	(
    		select nv.ID, MaNhanVien, TenNhanVien,
    			case when TinhChietKhauTheo =1 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau  end end as HoaHongThucThu,
				case when TinhChietKhauTheo =3 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end as HoaHongVND,
				-- neu HD tao thang truoc, nhung PhieuThu thuoc thang nay: HoaHongDoanhThu = 0
				case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then
					case when TinhChietKhauTheo = 2 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end else 0 end as HoaHongDoanhThu,
				-- timkiem theo NgayLapHD or NgayLapPhieuThu
				case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then hd.NgayLapHoaDon else tblQuy.NgayLapHoaDon end as NgayLapHoaDon,
				case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    		from BH_NhanVienThucHien th
    		join BH_HoaDon hd on th.ID_HoaDon= hd.ID
    		join NS_NhanVien nv on th.ID_NhanVien= nv.ID
			left join #temp tblQuy on hd.ID= tblQuy.ID_HoaDonLienQuan and (th.ID_QuyHoaDon= tblQuy.ID)	
    		where th.ID_HoaDon is not null
    		and hd.ChoThanhToan  is not null
    		and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))  
			and (exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID) or hd.NguoiTao like @nguoitao)
			-- chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
			and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 and ( exists (select ID from #temp where th.ID_QuyHoaDon = #temp.ID) or  LoaiHoaDon=6)))
			and
				((select count(Name) from @tblSearchString b where     			
				nv.TenNhanVien like '%'+b.Name+'%'
				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
				or nv.MaNhanVien like '%'+b.Name+'%'				
				)=@count or @count=0)	
    	) tbl
		where tbl.NgayLapHoaDon >= @DateFrom and tbl.NgayLapHoaDon < @DateTo and TrangThaiHD = @StatusInvoice
    	group by MaNhanVien, TenNhanVien, ID
		having SUM(ISNULL(HoaHongThucThu,0)) + SUM(ISNULL(HoaHongDoanhThu,0)) + SUM(ISNULL(HoaHongVND,0)) > 0 -- chi lay NV co CK > 0
		
		if @Status_DoanhThu =0
			insert into @tblDiscountInvoice
			select *
			from #temp2 
		else
			begin
				if @Status_DoanhThu= 1
					insert into @tblDiscountInvoice
					select *
					from #temp2 where HoaHongDoanhThu > 0 or HoaHongThucThu > 0
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
								from #temp2 where HoaHongVND > 0 Or HoaHongThucThu > 0
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
			
		with data_cte
		as(
		select * from @tblDiscountInvoice
		),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
				sum(HoaHongThucThu) as TongHoaHongThucThu,
				sum(HoaHongVND) as TongHoaHongVND,
				sum(TongAll) as TongAllAll
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.MaNhanVien
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[ReportDiscountInvoice_Detail]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				ID_NhanVienLogin = p.String(),
				TextSearch = p.String(),
				DateFrom = p.String(),
				DateTo = p.String(),
				Status_ColumHide = p.Int(),
				StatusInvoice = p.Int(),
				Status_DoanhThu = p.Int(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');

	set @DateTo = DATEADD(day,1,@DateTo)
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tblDiscountInvoice table (MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), NgayLapHoaDon datetime, NgayLapPhieu datetime, NgayLapPhieuThu datetime, MaHoaDon nvarchar(50),
		DoanhThu float, ThucThu float, HeSo float, HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, PTThucThu float, PTDoanhThu float, 
		MaKhachHang nvarchar(max), TenKhachHang nvarchar(max), DienThoaiKH nvarchar(max), TongAll float)

	-- bang tam chua DS phieu thu theo Ngay timkiem
	select qct.ID_HoaDonLienQuan, SUM(qct.TienThu) as ThucThu, qhd.NgayLapHoaDon, qhd.ID
	into #temp
	from Quy_HoaDon_ChiTiet qct
	join (
			select qhd.ID, qhd.NgayLapHoaDon
			from Quy_HoaDon qhd
			join BH_NhanVienThucHien th on qhd.ID= th.ID_QuyHoaDon
			where (qhd.TrangThai is null or qhd.TrangThai = '1')
			and qhd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
			and qhd.NgayLapHoaDon >= @DateFrom
			and qhd.NgayLapHoaDon <= @DateTo 
			group by qhd.ID, qhd.NgayLapHoaDon) qhd on qct.ID_HoaDon = qhd.ID
	where HinhThucThanhToan not in (4,5) -- tt = diem/thegiatri
	group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID;

		select MaNhanVien, 
    		TenNhanVien,
    		NgayLapHoaDon,
			NgayLapPhieu, -- used to check at where condition
			NgayLapPhieuThu,
    		MaHoaDon,
			-- taoHD truoc, PhieuThu sau --> khong co doanh thu
			case when @DateFrom <= tbl.NgayLapHoaDon and  tbl.NgayLapHoaDon < @DateTo then PhaiThanhToan else 0 end as DoanhThu, 
    		ISNULL(ThucThu,0) as ThucThu,
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
				select MaNhanVien, TenNhanVien, hd.MaHoaDon, 
					case when hd.LoaiHoaDon= 6 then - TongThanhToan + isnull(TongTienThue,0)
					else iif(hd.LoaiHoaDon=22, PhaiThanhToan, TongThanhToan - TongTienThue) end as PhaiThanhToan,
					hd.NgayLapHoaDon,
    				ThucThu, hd.LoaiHoaDon,
					hd.ID_DoiTuong,
    				th.HeSo,
					tblQuy.NgayLapHoaDon as NgayLapPhieuThu,
    				-- huy PhieuThu --> PTThucThu,HoaHongThucThu = 0		
					case when TinhChietKhauTheo =1 
						then case when LoaiHoaDon= 6 then -TienChietKhau else 
							case when ISNULL(ThucThu,0)= 0 then 0  else TienChietKhau end end end as HoaHongThucThu,
					case when TinhChietKhauTheo =1 
						then case when LoaiHoaDon= 6 then PT_ChietKhau else 
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
    			left join #temp tblQuy on hd.ID= tblQuy.ID_HoaDonLienQuan and (th.ID_QuyHoaDon= tblQuy.ID)			 
    			where th.ID_HoaDon is not null
				and hd.LoaiHoaDon in (1,19,22,6, 25,3)
    			and hd.ChoThanhToan is not null
    			and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
				and (exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID))
				--chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
				and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 and ( exists (select ID from #temp where th.ID_QuyHoaDon = #temp.ID) or  LoaiHoaDon=6)))		
				and
				((select count(Name) from @tblSearchString b where     			
				nv.TenNhanVien like '%'+b.Name+'%'
				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
				or nv.MaNhanVien like '%'+b.Name+'%'	
				or hd.MaHoaDon like '%'+b.Name+'%'								
				)=@count or @count=0)	
    	) tbl
		left join DM_DoiTuong dt on tbl.ID_DoiTuong= dt.ID
		where tbl.NgayLapPhieu >= @DateFrom and tbl.NgayLapPhieu < @DateTo and TrangThaiHD = @StatusInvoice
		--order by NgayLapHoaDon desc

	if @Status_DoanhThu =0
		insert into @tblDiscountInvoice
		select *
		from #temp2
	else
		begin
				if @Status_DoanhThu= 1
					insert into @tblDiscountInvoice
					select *
					from #temp2 where HoaHongDoanhThu > 0 or HoaHongThucThu > 0
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
								from #temp2 where HoaHongVND > 0 Or HoaHongThucThu > 0
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

	with data_cte
		as(
		select * from @tblDiscountInvoice
		),
		count_cte
		as (
			select count(*) as TotalRow,
				CEILING(COUNT(*) / CAST(@PageSize as float ))  as TotalPage,
				sum(DoanhThu) as TongDoanhThu,
				sum(ThucThu) as TongThucThu,
				sum(HoaHongThucThu) as TongHoaHongThucThu,
				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
				sum(HoaHongVND) as TongHoaHongVND,
				sum(TongAll) as TongAllAll
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[ReportDiscountProduct_Detail]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				ID_NhanVienLogin = p.String(),
				ID_NhomHang = p.String(),
				TextSearch = p.String(),
				TextSearchHangHoa = p.String(),
				DateFrom = p.String(),
				DateTo = p.String(),
				Status_ColumHide = p.Int(),
				StatusInvoice = p.Int(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"set nocount on;
	set @DateTo = DATEADD(day,1,@DateTo)

	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHangHoa_XemDS_PhongBan','BCCKHangHoa_XemDS_HeThong');

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

	with data_cte
	as (

		select MaHoaDon, LoaiHoaDon,
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
    		from
    		(
				select 
						tbl.MaHoaDon,			
						tbl.LoaiHoaDon,
    					tbl.NgayLapHoaDon,
						tbl.ID_DoiTuong,
    					tbl.MaHangHoa,
						tbl.ID_NhanVien,
						TenHangHoa,
						TenHangHoaFull,
						TenDonViTinh,
						ThuocTinh_GiaTri,
						TenLoHang,
						ID_NhomHang,
						TenNhomHangHoa,
						SoLuong,
						ThanhTien,
						HeSo,
						TrangThaiHD,
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
    					hd.MaHoaDon,			
						hd.LoaiHoaDon,
    					hd.NgayLapHoaDon,
						hd.ID_DoiTuong,
    					dvqd.MaHangHoa,
						ck.ID_NhanVien,
    					hh.TenHangHoa + Case when (dvqd.ThuocTinhGiaTri is null or dvqd.ThuocTinhGiaTri = '') then '' else '_' + dvqd.ThuocTinhGiaTri end as TenHangHoaFull,
    					hh.TenHangHoa,
						hdct.SoLuong,
						ISNULL(hh.ID_NhomHang,N'00000000-0000-0000-0000-000000000000') as ID_NhomHang,
						ISNULL(nhh.TenNhomHangHoa,N'') as TenNhomHangHoa,
						-- mua goi, khong tinh phidv
						-- su dung goi: ThanhTien =0 --> lay soluong * dongia
						case when hd.LoaiHoaDon =19 then ThanhTien
						else 
							case when hh.ChiPhiThucHien > 0 then 
								case when hh.ChiPhiTinhTheoPT =1 then hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau) -(hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau) * hh.ChiPhiThucHien/100)
    							else hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau)- hh.ChiPhiThucHien * hdct.SoLuong end
							else hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau) end
						end as ThanhTien,

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
						and 
						((select count(Name) from @tblSearchHH b where     									
							 dvqd.MaHangHoa like '%'+b.Name+'%'
							or hh.TenHangHoa like '%'+b.Name+'%'
							or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
							or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'	
							)=@countHH or @countHH=0)
    			) tbl
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
    	),
		count_cte
		as (
			select count(*) as TotalRow,
				CEILING(COUNT(*) / CAST(@PageSize as float ))  as TotalPage,
				sum(SoLuong) as TongSoLuong,
				sum(HoaHongThucHien) as TongHoaHongThucHien,
				sum(HoaHongThucHien_TheoYC) as TongHoaHongThucHien_TheoYC,
				sum(HoaHongTuVan) as TongHoaHongTuVan,
				sum(HoaHongBanGoiDV) as TongHoaHongBanGoiDV,
				sum(TongAll) as TongAllAll,
				sum(ThanhTien) as TongThanhTien
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[ReportDiscountProduct_General]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				ID_NhanVienLogin = p.String(),
				TextSearch = p.String(),
				DateFrom = p.String(),
				DateTo = p.String(),
				Status_ColumHide = p.Int(),
				StatusInvoice = p.Int(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"set nocount on;
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHangHoa_XemDS_PhongBan','BCCKHangHoa_XemDS_HeThong');

--	select * from @tblNhanVien


	set @DateTo = DATEADD(day,1,@DateTo);

	with data_cte
	as (

	select nv.MaNhanVien, nv.TenNhanVien, b.*
	from
	(
		SELECT 
    			a.ID_NhanVien,
    			CAST(ROUND(SUM(a.HoaHongThucHien), 0) as float) as HoaHongThucHien,
				CAST(ROUND(SUM(a.HoaHongThucHien_TheoYC), 0) as float) as HoaHongThucHien_TheoYC,
    			CAST(ROUND(SUM(a.HoaHongTuVan), 0) as float) as HoaHongTuVan,
    			CAST(ROUND(SUM(a.HoaHongBanGoiDV), 0) as float) as HoaHongBanGoiDV,   		
    			case @Status_ColumHide
    				when  1 then cast(0 as float)
    				when  2 then SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  3 then SUM(ISNULL(HoaHongBanGoiDV,0.0))
    				when  4 then SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  5 then SUM(ISNULL(HoaHongTuVan,0.0))
    				when  6 then SUM(ISNULL(HoaHongThucHien_TheoYC,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
    				when  7 then SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
					when  8 then SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  9 then SUM(ISNULL(HoaHongThucHien,0.0))
    				when  10 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  11 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0)) 
    				when  12 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  13 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
					when  14 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    				when  15 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0))
    			else SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0))+  SUM(ISNULL(HoaHongThucHien_TheoYC,0.0))
    			end as Tong
    		FROM
    		(
				select ckout.ID_NhanVien,
					ckout.TrangThaiHD,
					case when ckout.LoaiHoaDon= 6 then - HoaHongThucHien else HoaHongThucHien end as HoaHongThucHien,
					case when ckout.LoaiHoaDon= 6 then - HoaHongTuVan else HoaHongTuVan end as HoaHongTuVan,
					case when ckout.LoaiHoaDon= 6 then - HoaHongThucHien_TheoYC else HoaHongThucHien_TheoYC end as HoaHongThucHien_TheoYC,
					case when ckout.LoaiHoaDon= 6 then - HoaHongBanGoiDV else HoaHongBanGoiDV end as HoaHongBanGoiDV				
				from
    				(Select 
						ck.ID_NhanVien,
						hd.LoaiHoaDon,
    					Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien_TheoYC,
    					Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
						case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    				from
    				BH_NhanVienThucHien ck
    				inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    				inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    				Where hd.ChoThanhToan is not null
    					and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and hd.NgayLapHoaDon >= @DateFrom 
    					and hd.NgayLapHoaDon < @DateTo   
						and (exists (select ID from @tblNhanVien nv where ck.ID_NhanVien = nv.ID))
					) ckout
    		) a where a.TrangThaiHD = @StatusInvoice
    		GROUP BY a.ID_NhanVien
    	) b
		join NS_NhanVien nv on b.ID_NhanVien= nv.ID
		where 
			((select count(Name) from @tblSearchString b where     			
				nv.TenNhanVien like '%'+b.Name+'%'
				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
				or nv.MaNhanVien like '%'+b.Name+'%'				
				)=@count or @count=0)		
		),
		count_cte
		as (
			select count(ID_NhanVien) as TotalRow,
				CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float ))  as TotalPage,
				sum(HoaHongThucHien) as TongHoaHongThucHien,
				sum(HoaHongThucHien_TheoYC) as TongHoaHongThucHien_TheoYC,
				sum(HoaHongTuVan) as TongHoaHongTuVan,
				sum(HoaHongBanGoiDV) as TongHoaHongBanGoiDV,
				sum(Tong) as TongAll
			from data_cte
			)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.MaNhanVien
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[ReportValueCard_Balance]", parametersAction: p => new
			{
				TextSearch = p.String(),
				ID_ChiNhanhs = p.String(),
				DateFrom = p.String(),
				DateTo = p.String(),
				Status = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"set nocount on;
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
							dt.MaDoiTuong like '%'+b.Name+'%'
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
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[ReportValueCard_DiaryUsed]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				TextSearch = p.String(),
				DateFrom = p.String(14),
				DateTo = p.String(14),
				Status = p.String(5),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"set nocount on;
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);

    DECLARE @TblHisCard TABLE(
    				STT INT, ID UNIQUEIDENTIFIER, ID_DoiTuong UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(50),TenDoiTuong NVARCHAR(500), MaHoaDon NVARCHAR(500),  SLoaiHoaDon nvarchar(max), 
    				MaHoaDonSQ NVARCHAR(MAX),LoaiHoaDonSQ INT, NgayLapHoaDon DATETIME, TienThe FLOAT, ThuChiThe FLOAT, SoDuTruoc FLOAT, SoDuSau FLOAT, TrangThai_TheGiaTri int)
    	INSERT INTO @TblHisCard
    		SELECT 
				ROW_NUMBER() OVER(ORDER BY qhd.ID) AS STT,
				qhd.ID,
				dt.ID as ID_DoiTuong,-- used to caculator sodutruoc phatsinh
				dt.MaDoiTuong,
				dt.TenDoiTuong,
				MaHoaDons,
				ISNULL(LoaiHoaDons, N'Loại khác, ') as LoaiHoaDons,
				qhd.MaHoaDon as MaHoaDonSQ, 
				qhd.LoaiHoaDon as LoaiHoaDonSQ,
				qhd.NgayLapHoaDon,
				SUM(ISNULL(qct.TienThu,0)) as TienThe,
				case when qhd.LoaiHoaDon = 11 then - SUM(ISNULL(qct.TienThu,0)) else SUM(ISNULL(qct.TienThu,0)) end as ThuChiThe,
				0 as SoDuTruoc,
				0 as SoDuSau,
				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
				else '12' end as TrangThai
			FROM Quy_HoaDon qhd
			join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
			join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
			join (
				--merger text MaHoaDon to 1 row
				Select distinct qhdXML.ID, 
							 (
						select distinct hd.MaHoaDon +', '  AS [text()]
						from BH_HoaDon hd
						join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
						where qct.ID_HoaDon = qhdXML.ID
						and hd.LoaiHoaDon in (1, 3, 6, 19,25)
						and ChoThanhToan ='0'
						For XML PATH ('')
					) MaHoaDons
				from Quy_HoaDon qhdXML
			) tbl on qhd.ID= tbl.ID

			-- get LoaiHoaDon
			join (
				Select distinct qhdXML2.ID, 
					 (
					 -- merger text LoaiHoaDon to 1 row
						select distinct 
							 tbl1.SLoaiHoaDon +', '  AS [text()]
						from 
							(
							-- get text HoaDon by LoaiHoaDon
							select 
								case hd.LoaiHoaDon
									when 1 then N'Bán hàng'
									when 3 then N'Đặt hàng'
									when 6 then N'Trả hàng'
									when 25 then N'Sửa chữa'
								else 
									N'Gói dịch vụ' end as SLoaiHoaDon
							from BH_HoaDon hd
							join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
							where hd.LoaiHoaDon in (1, 3, 6, 19, 25)
							and ChoThanhToan ='0' 
							and qct.ID_HoaDon = qhdXML2.ID
							) tbl1
						For XML PATH ('')
					) LoaiHoaDons
				from Quy_HoaDon qhdXML2
		) tbl2 on qhd.ID= tbl2.ID

	where (qhd.TrangThai = 1 or qhd.TrangThai is null)		
	and qct.HinhThucThanhToan = 4
	and FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') >= @DateFrom 
	and FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') <= @DateTo
	and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
	and				 
		((select count(Name) from @tblSearchString b where     			
		dt.MaDoiTuong like '%'+b.Name+'%'
		or dt.TenDoiTuong like '%'+b.Name+'%'
		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
		or dt.DienThoai like '%'+b.Name+'%'			
		)=@count or @count=0)	
	GROUP BY qhd.ID,
		dt.ID,
		dt.MaDoiTuong,
		dt.TenDoiTuong,
		qhd.MaHoaDon,
		qhd.LoaiHoaDon,
		qhd.NgayLapHoaDon,
		dt.TrangThai_TheGiaTri,
		MaHoaDons,
		LoaiHoaDons
    		    	
    			DECLARE @SoDuTruocPhatSinh INT;
    
    			DECLARE @STT INT;
    			DECLARE @ID UNIQUEIDENTIFIER;
    			DECLARE @ID_DoiTuong UNIQUEIDENTIFIER;
    			DECLARE @MaDoiTuong NVARCHAR(50);
    			DECLARE @TenDoiTuong NVARCHAR(500);
    			DECLARE @MaHoaDon NVARCHAR(500);
    			DECLARE @LoaiHoaDon nvarchar(max);
    			DECLARE @MaHoaDonSQ NVARCHAR(50);
    			DECLARE @LoaiHoaDonSQ INT;
    			DECLARE @NgayLapHoaDon DATETIME;
    			DECLARE @TienThe FLOAT;
    			DECLARE @ThuChiThe FLOAT;
    			DECLARE @SoDuTruoc FLOAT;
    			DECLARE @SoDuSau FLOAT;
    			DECLARE @TrangThai_TheGiaTri INT;
    
    			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR SELECT STT, ID,ID_DoiTuong, MaDoiTuong, TenDoiTuong, MaHoaDon, SLoaiHoaDon, MaHoaDonSQ, LoaiHoaDonSQ, NgayLapHoaDon, TienThe, ThuChiThe, SoDuTruoc, SoDuSau, TrangThai_TheGiaTri
    			FROM @TblHisCard
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe,@ThuChiThe, @SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
    					SET @SoDuTruocPhatSinh = [dbo].[TinhSoDuKHTheoThoiGian](@ID_DoiTuong,@NgayLapHoaDon)
    
    					UPDATE @TblHisCard SET SoDuTruoc= @SoDuTruocPhatSinh, SoDuSau = @SoDuTruocPhatSinh + @ThuChiThe
    					WHERE STT = @STT
    
    					FETCH NEXT FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe, @ThuChiThe,@SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    
    					
    			END
    		CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT;

			with data_cte
			as (    
    				SELECT ID, ID_DoiTuong,MaDoiTuong,TenDoiTuong,
						LEFT(MaHoaDon, LEN(MaHoaDon) - 1) as MaHoaDon,
						LEFT(SLoaiHoaDon, LEN(SLoaiHoaDon) - 1) as SLoaiHoaDon,
						MaHoaDonSQ, LoaiHoaDonSQ,NgayLapHoaDon,TienThe,SoDuTruoc, 
						IIF(LoaiHoaDonSQ = 12, TienThe, 0) AS PhatSinhTang,
						IIF(LoaiHoaDonSQ = 11, TienThe, 0) AS PhatSinhGiam, 
						SoDuSau, TrangThai_TheGiaTri				
    				FROM @TblHisCard 
    				WHERE TrangThai_TheGiaTri like @Status
				),
			count_cte
		as (
				select count(ID) as TotalRow,
					CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
					sum(SoDuTruoc) as TongSoDuDauKy,
					sum(PhatSinhTang) as TongPhatSinhTang,
					sum(PhatSinhGiam) as TongPhatSinhGiam,
					sum(SoDuSau) as TongSoDuCuoiKy
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[ValueCard_GetListHisUsed]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				ID_KhachHang = p.String(),
				DateFrom = p.String(),
				DateTo = p.String()
			}, body: @"SET NOCOUNT ON;
    DECLARE @TblHisCard TABLE(
    				STT INT, ID UNIQUEIDENTIFIER, ID_DoiTuong UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(50),TenDoiTuong NVARCHAR(500), MaHoaDon NVARCHAR(500),  SLoaiHoaDon nvarchar(max), 
    				MaHoaDonSQ NVARCHAR(MAX),LoaiHoaDonSQ INT, NgayLapHoaDon DATETIME, TienThe FLOAT, ThuChiThe FLOAT, SoDuTruoc FLOAT, SoDuSau FLOAT, TrangThai_TheGiaTri int)
    	INSERT INTO @TblHisCard
    		SELECT 
    				ROW_NUMBER() OVER(ORDER BY qhd.ID) AS STT,
    				qhd.ID,
    				dt.ID as ID_DoiTuong,-- used to caculator sodutruoc phatsinh
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				MaHoaDons,
    				ISNULL(LoaiHoaDons, N'Loại khác, ') as LoaiHoaDons,
    				qhd.MaHoaDon as MaHoaDonSQ, 
    				qhd.LoaiHoaDon as LoaiHoaDonSQ,
    				qhd.NgayLapHoaDon,
    				SUM(ISNULL(qct.ThuTuThe,0)) as TienThe,
    				case when qhd.LoaiHoaDon = 11 then - SUM(ISNULL(qct.ThuTuThe,0)) else SUM(ISNULL(qct.ThuTuThe,0)) end as ThuChiThe,
    				0 as SoDuTruoc,
    				0 as SoDuSau,
    				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
    				else '12' end as TrangThai
    			FROM Quy_HoaDon qhd
    			join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    			join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
    			join (
    				--merger text MaHoaDon to 1 row
    				Select distinct qhdXML.ID, 
    							 (
    						select distinct hd.MaHoaDon +', '  AS [text()]
    						from BH_HoaDon hd
    						join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    						where qct.ID_HoaDon = qhdXML.ID
    						and hd.LoaiHoaDon in (1, 3, 6, 19)
    						and ChoThanhToan ='0'
    						For XML PATH ('')
    					) MaHoaDons
    				from Quy_HoaDon qhdXML
    			) tbl on qhd.ID= tbl.ID
    			
    
    			-- get LoaiHoaDon
    			join (
    				Select distinct qhdXML2.ID, 
    					 (
    					 -- merger text LoaiHoaDon to 1 row
    						select distinct 
    							 tbl1.SLoaiHoaDon +', '  AS [text()]
    						from 
    							(
    							-- get text HoaDon by LoaiHoaDon
    							select 
    								case hd.LoaiHoaDon
    									when 1 then N'Bán hàng'
    									when 3 then N'Đặt hàng'
    									when 6 then N'Trả hàng'
    								else 
    									N'Gói dịch vụ' end as SLoaiHoaDon
    							from BH_HoaDon hd
    							join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    							where hd.LoaiHoaDon in (1, 3, 6, 19)
    							and ChoThanhToan ='0' 
    							and qct.ID_HoaDon = qhdXML2.ID
    							) tbl1
    						For XML PATH ('')
    					) LoaiHoaDons
    				from Quy_HoaDon qhdXML2
    		) tbl2 on qhd.ID= tbl2.ID
    
    	where (qhd.TrangThai = 1 or qhd.TrangThai is null)		
		and qct.HinhThucThanhToan=4
    	and CONVERT(varchar, qhd.NgayLapHoaDon, 112) >= @DateFrom 
    	and CONVERT(varchar, qhd.NgayLapHoaDon, 112) <= @DateTo
    	and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
    	and dt.ID like @ID_KhachHang
    	GROUP BY qhd.ID,
    		dt.ID,
    		dt.MaDoiTuong,
    		dt.TenDoiTuong,
    		qhd.MaHoaDon,
    		qhd.LoaiHoaDon,
    		qhd.NgayLapHoaDon,
    		dt.TrangThai_TheGiaTri,
    		MaHoaDons,
    		LoaiHoaDons
    		    	
    			DECLARE @SoDuTruocPhatSinh INT;
    
    			DECLARE @STT INT;
    			DECLARE @ID UNIQUEIDENTIFIER;
    			DECLARE @ID_DoiTuong UNIQUEIDENTIFIER;
    			DECLARE @MaDoiTuong NVARCHAR(50);
    			DECLARE @TenDoiTuong NVARCHAR(500);
    			DECLARE @MaHoaDon NVARCHAR(500);
    			DECLARE @LoaiHoaDon nvarchar(max);
    			DECLARE @MaHoaDonSQ NVARCHAR(50);
    			DECLARE @LoaiHoaDonSQ INT;
    			DECLARE @NgayLapHoaDon DATETIME;
    			DECLARE @TienThe FLOAT;
    			DECLARE @ThuChiThe FLOAT;
    			DECLARE @SoDuTruoc FLOAT;
    			DECLARE @SoDuSau FLOAT;
    			DECLARE @TrangThai_TheGiaTri INT;
    
    			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR SELECT STT, ID,ID_DoiTuong, MaDoiTuong, TenDoiTuong, MaHoaDon, SLoaiHoaDon, MaHoaDonSQ, LoaiHoaDonSQ, NgayLapHoaDon, TienThe, ThuChiThe, SoDuTruoc, SoDuSau, TrangThai_TheGiaTri
    			FROM @TblHisCard
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe,@ThuChiThe, @SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
    					SET @SoDuTruocPhatSinh = [dbo].[TinhSoDuKHTheoThoiGian](@ID_DoiTuong,@NgayLapHoaDon)
    
    					UPDATE @TblHisCard SET SoDuTruoc= @SoDuTruocPhatSinh, SoDuSau = @SoDuTruocPhatSinh + @ThuChiThe
    					WHERE STT = @STT
    
    					FETCH NEXT FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe, @ThuChiThe,@SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    
    					
    			END
    		CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT
    
    		SELECT ID, ID_DoiTuong,MaDoiTuong,TenDoiTuong,
    				LEFT(MaHoaDon, LEN(MaHoaDon) - 1) as MaHoaDon,
    				LEFT(SLoaiHoaDon, LEN(SLoaiHoaDon) - 1) as SLoaiHoaDon,
    				MaHoaDonSQ, LoaiHoaDonSQ,NgayLapHoaDon,TienThe,SoDuTruoc, SoDuSau, TrangThai_TheGiaTri				
    		FROM @TblHisCard 
    		WHERE TrangThai_TheGiaTri like '%11%' --11.DangHoatDong
    		ORDER BY NgayLapHoaDon desc");

			CreateStoredProcedure(name: "[dbo].[ValueCard_ServiceUsed]", parametersAction: p => new
			{
				ID_ChiNhanhs = p.String(),
				TextSearch = p.String(),
				DateFrom = p.String(14),
				DateTo = p.String(14),
				Status = p.String(14),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;   
		DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		DECLARE @count int =  (Select count(*) from @tblSearchString);
	
		select hd.ID as ID_HoaDon,tblq.ID_HoaDon as ID_PhieuThuChi, hd.MaHoaDon,tblq.NgayLapHoaDon,ISNULL(dt.MaDoiTuong,'') as MaDoiTuong, ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, 
    	qd.MaHangHoa,hh.TenHangHoa,ct.SoLuong, ct.DonGia, ct.TienChietKhau, ct.ThanhTien,  ISNULL(tblq.PhatSinhGiam,0) as PhatSinhGiam, ISNULL(tblq.PhatSinhTang,0) as PhatSinhTang, tblq.MaHoaDon as MaPhieuThu,		
		case hd.LoaiHoaDon
			when 1 then N'Bán hàng'
			when 3 then N'Đặt hàng'
			when 6 then N'Trả hàng'
			when 19 then N'Gói dịch vụ'
			when 25 then N'Sửa chữa'
		else '' end as SLoaiHoaDon
    	from BH_HoaDon hd
    	join BH_HoaDon_ChiTiet ct on hd.id= ct.id_hoadon
    	left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    	join DonViQuiDoi qd on ct.id_donviquidoi= qd.id
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
    	join (select qct.ID_HoaDonLienQuan, MaHoaDon, NgayLapHoaDon, qct.ID_HoaDon,
				case when qhd.LoaiHoaDon = 11 then SUM(ISNULL(qct.ThuTuThe ,0)) end as PhatSinhGiam,
				case when qhd.LoaiHoaDon = 12 then SUM(ISNULL(qct.ThuTuThe ,0)) end as PhatSinhTang
    		from Quy_HoaDon_Chitiet qct 
    		join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		where qhd.TrangThai ='1' 
			and qct.HinhThucThanhToan=4
    		and FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') >=@DateFrom
    		and FORMAT(qhd.NgayLapHoaDon,'yyyy-MM-dd') <= @DateTo
    		group by qct.ID_HoaDonLienQuan, qct.ID_HoaDon, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon) tblq on hd.ID= tblq.ID_HoaDonLienQuan
    	where hd.LoaiHoaDon in ( 1,3,6,19,25) 
    	and hd.ChoThanhToan ='0'
    	and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)	
		order by hd.NgayLapHoaDon desc");

			Sql(@"ALTER Function [dbo].[GetDebitCustomer_allBrands]
(
@ID_DoiTuong uniqueidentifier
)
returns float
as
BEGIN
	declare @NoHienTai float =(
		SELECT     		  			
    		 CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai    		 		
    	FROM DM_DoiTuong dt  			  
    	LEFT JOIN (
    				SELECT 
						tblThuChi.ID_DoiTuong,
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
    						WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = '0'    						
							AND bhd.ID_DoiTuong= @ID_DoiTuong
    
    						 union all
    							-- tongtra
    						SELECT bhd.ID_DoiTuong,
    							ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = '0'  
								AND bhd.ID_DoiTuong= @ID_DoiTuong
    							
    							 union all
    
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
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
							AND qhdct.HinhThucThanhToan not in (6)
							AND qhdct.ID_DoiTuong= @ID_DoiTuong
								
							union all
    
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
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
								AND qhdct.ID_DoiTuong= @ID_DoiTuong
					)AS tblThuChi GROUP BY tblThuChi.ID_DoiTuong   						
    		) a on dt.ID = a.ID_DoiTuong  		
			where dt.ID= @ID_DoiTuong
	) 
	return @NoHienTai
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhanVien_TongHop]
    @MaNV [nvarchar](max),
    @MaNV_TV [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @timeCreate_start [datetime],
    @timeCreate_end [datetime],
    @ID_PhongBan [nvarchar](max),
    @ID_PhongBan_SP [nvarchar](max),
    @GioiTinh [nvarchar](max),
    @LoaiHopDong [nvarchar](max),
    @timeBirthday_start [datetime],
    @timeBirthday_end [datetime],
    @LoaiChinhTri [nvarchar](max),
    @LoaiBaoHiem [nvarchar](max),
    @LoaiDanToc [nvarchar](max),
    @LoaiDanToc_SP [nvarchar](max),
    @TrangThai [nvarchar](max)
AS
BEGIN
    SELECT DISTINCT 
    	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.DanTocTonGiao, a.SoCMND, a.DienThoaiDiDong, a.DiaChiTT,
    	a.NgayVaoLamViec, a.TenPhongBan, a.GhiChu,
    	a.NgayVaoDoan, a.NoiVaoDoan, a.NgayNhapNgu, a.NgayXuatNgu,
    	a.NgayVaoDang, a.NgayVaoDangChinhThuc, a.NgayRoiDang, a.NoiSinhHoatDang, a.LyDoRoiDang,
    	a.NgayTao
    	FROM
    	(
    	SELECT nv.MaNhanVien, nv.TenNhanVien, 
    	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
    	nv.NgaySinh,
    	nv.NgayTao,
    	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao, nv.SoCMND, nv.DienThoaiDiDong, nv.DiaChiTT,
    	nv.NgayVaoLamViec, pb.TenPhongBan,nv.GhiChu,
    	nv.NgayVaoDoan, nv.NoiVaoDoan,
    	nv.NgayNhapNgu, nv.NgayXuatNgu,
    	nv.NgayVaoDang, nv.NgayVaoDangChinhThuc, nv.NgayRoiDang, nv.NoiSinhHoatDang, nv.LyDoRoiDang,
    	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
    	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
    	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
    	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
    	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
    	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
    	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
    	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
    	FROM
    	NS_NhanVien nv
    	left join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
    	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
    	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
    	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
    	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
    	and nv.GioiTinh like @GioiTinh 
    	and nv.DaNghiViec like @TrangThai
    	and ct.ID_DonVi = @ID_ChiNhanh
    	and (nv.TenNhanVien like @MaNV or  nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV 
		or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)    	
    	) as a
    	where 
		(a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
    	and 
		a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
    	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
    	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
    	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
    	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
    	and
		(a.TrangThai !=0 or a.TrangThai is null)
    	order by a.NgayTao DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_ThuChi_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    SELECT 
    MAX(b.TenNhomDoiTuong) as NhomDoiTuong,
    b.MaHoaDon,
    MAX(b.MaPhieuThu) as MaPhieuThu,
    MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    MAX(b.ManguoiNop) as ManguoiNop, 
    MAX(b.TenNguoiNop) as TenNguoiNop, 
    MAX(b.ThuChi) as ThuChi, 
    MAX(b.NoiDungThuChi) as NoiDungThuChi,
    MAX(b.GhiChu) as GhiChu,
    MAX(b.LoaiThuChi) as LoaiThuChi,
    	dv.TenDonVi AS TenChiNhanh
    FROM
    (
    	  select 
    		a.ID_DoiTuong,
    		a.ID_HoaDon,
    		a.TenNhomDoiTuong,
    		a.ID_NhomDoiTuong,
    	a.MaHoaDon,
    		a.MaPhieuThu,
    	a.NgayLapHoaDon,
    		a.MaNguoiNop,
    	a.TenNguoiNop,
    	--a.ThuChi,
    		a.TienMat + a.TienGui as ThuChi,
    	a.NoiDungThuChi,
    	a.GhiChu,
    	Case when a.LoaiThuChi = 1 then N'Phiếu thu khác'  
    	when a.LoaiThuChi = 2 then N'Phiếu chi khác' 
    	when a.LoaiThuChi = 3 then N'Thu tiền khách trả'  
    	when a.LoaiThuChi = 4 then N'Chi tiền đổi trả hàng'  
    	when a.LoaiThuChi = 5 then N'Thu tiền nhà NCC'  
    	when a.LoaiThuChi = 6 then N'Chi tiền trả NCC' else '' end as LoaiThuChi,
    		a.ID_DonVi
    	From
    	(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			MAX (tknh.SoTaiKhoan) as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
    				--Max(dt.TenNhomDoiTuongs) as TenNhomDoiTuong,
    				case when qhdct.ID_NhanVien is not null then N'Nhân viên' else MAX(dt.TenNhomDoiTuongs) end as TenNhomDoiTuong,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case WHEN qhdct.ID_NhanVien is not null
    				then
    				'00000000-0000-0000-0000-000000000000' 
    				else 
    				case When dtn.ID_NhomDoiTuong is null 
    					
    				then '00000000-0000-0000-0000-000000000000'  else dtn.ID_NhomDoiTuong 
    				end
    				end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    				case when qhdct.ID_NhanVien is not null then nv.MaNhanVien else dt.MaDoiTuong end as ManguoiNop,
    			Sum(qhdct.TienMat) as TienMat,
    			Sum(qhdct.TienGui) as TienGui,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
    				qhd.ID_DonVi
    		From Quy_HoaDon qhd 			
    			join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    				left join NS_NhanVien nv on qhdct.ID_NhanVien= nv.ID
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart and @timeEnd 
    				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, dt.loaidoituong) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and qhdct.HinhThucThanhToan NOT IN (4, 5, 6)
    				AND ((select count(Name) from @tblSearch b where     			
    			dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or qhd.MaHoaDon like '%' + b.Name + '%'
    				or hd.MaHoaDon like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhdct.ID_NhanVien, dt.MaDoiTuong,dt.LoaiDoiTuong,  nv.MaNhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID, qhd.ID_DonVi
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) 
    	) b
    		inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    		where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = ''
    	Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi, dv.TenDonVi
    	ORDER BY NgayLapHoaDon DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_BieuDoThucThuToDay]
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
    			SELECT
				DAY(qhd.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
				qct.TienThu as ThanhTien
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				join DM_DonVi dv on qhd.ID_DonVi = dv.ID
				where qhd.LoaiHoaDon = 11
				and (qhd.TrangThai is null or qhd.TrangThai != 0)
				and qct.HinhThucThanhToan not in (4,5,6)
				--and (qct.DiemThanhToan is null or qct.DiemThanhToan =0)
				and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
				and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
				and qhd.HachToanKinhDoanh = 1
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
    			SELECT
				DAY(qhd.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
				qct.TienThu as ThanhTien
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				join DM_DonVi dv on qhd.ID_DonVi = dv.ID
				where qhd.LoaiHoaDon = 11
				and qhd.HachToanKinhDoanh = 1
				and (qhd.TrangThai is null or qhd.TrangThai != 0)
				and qct.HinhThucThanhToan not in (4,5,6)
				--	and (qct.DiemThanhToan is null or qct.DiemThanhToan =0)
				and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
				and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
				and qhd.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
			) a
    		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
			ORDER BY NgayLapHoaDon
	END

END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_BieuDoThucThuToHour]
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
		SELECT
		DATEPART(HOUR, qhd.NgayLapHoaDon) as NgayLapHoaDon,
		dv.TenDonVi as TenChiNhanh,
		qct.TienThu as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		join DM_DonVi dv on qhd.ID_DonVi = dv.ID
		where qhd.LoaiHoaDon = 11
		and qhd.HachToanKinhDoanh = 1
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.HinhThucThanhToan not in (4,5,6)
		--and (qct.DiemThanhToan is null or qct.DiemThanhToan = 0)
		and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
		and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
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
		SELECT
		DATEPART(HOUR, qhd.NgayLapHoaDon) as NgayLapHoaDon,
		dv.TenDonVi as TenChiNhanh,
		qct.TienThu as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		join DM_DonVi dv on qhd.ID_DonVi = dv.ID
		where qhd.LoaiHoaDon = 11
		and qhd.HachToanKinhDoanh = 1
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.HinhThucThanhToan not in (4,5,6)
		--and (qct.DiemThanhToan is null or qct.DiemThanhToan = 0)
		and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
		and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
		and qhd.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
		) a
		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon
	END
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_DoanhThuToDay]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
select *, CAST (0 as float) AS SoSanhCungKy
	--CASE WHEN b.DoanhThuThangTruoc = 0 then 100 
	--else CAST(ROUND(( CAST(b.DoanhThuThangNay - b.DoanhThuThangTruoc as float) / b.DoanhThuThangTruoc) * 100, 2) as float) end as SoSanhCungKy
from

	(select 
		CAST(SUM(HD_SoLuongBan) as float) as HD_SoLuongBan,
		CAST(SUM(HD_GiaTriBan) as float) as HD_ThanhTien,
		CAST(SUM(GDV_SoLuongBan) as float) as GDV_SoLuongBan,
		CAST(SUM(GDV_GiaTriBan) as float) as GDV_ThanhTien,
		CAST(SUM(SoLuongTra) as float) as SoLuongTra,
		CAST(SUM(GiaTriTra) as float) as GiaTriTra,
		CAST(SUM(GiaTriBanTruoc) as float) as GiaTriBanTruoc,
		CAST(SUM(GiaTriTraTruoc) as float) as GiaTriTraTruoc,
		CAST(SUM(HD_GiaTriBan) + SUM(GDV_GiaTriBan) - SUM(GiaTriTra) as float) as DoanhThuThangNay,
		CAST(SUM(GiaTriBanTruoc) - SUM(GiaTriTraTruoc) as float)  as DoanhThuThangTruoc

	from 
	(
			-- banhang 
			SELECT
				hdb.ID_DonVi,
    			hdb.ID as ID_HoaDon,
    			1 as HD_SoLuongBan,    					
				isnull(hdb.TongThanhToan, hdb.PhaiThanhToan) - isnull(hdb.TongTienThue,0) as HD_GiaTriBan,
				0 as GDV_SoLuongBan,
				0 as GDV_GiaTriBan,
				0 as SoLuongTra,    	
    			0 as GiaTriTra,
    			0 as GiaTriBanTruoc,
    			0 as GiaTriTraTruoc
    		FROM BH_HoaDon hdb    		
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi = @ID_ChiNhanh
    		and hdb.LoaiHoaDon = 1

			-- bangoidv
			union all

			SELECT
				hdb.ID_DonVi,
    			hdb.ID as ID_HoaDon,
				0 as HD_SoLuongBan,
				0 as HD_GiaTriBan,
    			1 as GDV_SoLuongBan,  			
				ISNULL(hdb.PhaiThanhToan, 0)  as GDV_GiaTriBan,
				0 as SoLuongTra,  
    			0 as GiaTriTra,
    			0 as GiaTriBanTruoc,
    			0 as GiaTriTraTruoc
    		FROM BH_HoaDon hdb    		
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi = @ID_ChiNhanh
    		and hdb.LoaiHoaDon = 19

    		UNION ALL
			-- trahang
    		SELECT
				hdt.ID_DonVi,
    			hdt.ID as ID_HoaDon,
    			0 as HD_SoLuongBan,
				0 as HD_GiaTriBan,
    			0 as GDV_SoLuongBan,  		
				0 as GDV_GiaTriBan,  	
				1 as SoLuongTra,
    			ISNULL(hdt.PhaiThanhToan, 0) - isnull(hdt.TongTienThue,0) as GiaTriTra,				  
    			0 as GiaTriBanTruoc,
    			0 as GiaTriTraTruoc			
    		FROM BH_HoaDon hdt    		
    		where hdt.NgayLapHoaDon >= @timeStart and hdt.NgayLapHoaDon < @timeEnd
    		and hdt.ChoThanhToan = 0
    		and hdt.ID_DonVi = @ID_ChiNhanh
    		and hdt.LoaiHoaDon = 6 
    
    			-- tháng trước
    		Union all
			-- banhang + goidv
    		SELECT
				hdb.ID_DonVi,
    			hdb.ID as ID_HoaDon,
    			0 as HD_SoLuongBan,
				0 as HD_GiaTriBan,
    			0 as GDV_SoLuongBan,  		
				0 as GDV_GiaTriBan,  
				0 as SoLuongTra,
    			0 as GiaTriTra,
				ISNULL(hdb.PhaiThanhToan, 0) as GiaTriBanTruoc,
    			0 as GiaTriTraTruoc
    		FROM
    		BH_HoaDon hdb
    		where NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi = @ID_ChiNhanh
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			
    		UNION ALL
			-- trahang
    		SELECT
				hdt.ID_DonVi,
    			hdt.ID as ID_HoaDon,
    			0 as HD_SoLuongBan,
				0 as HD_GiaTriBan,
    			0 as GDV_SoLuongBan,  		
				0 as GDV_GiaTriBan,  
				0 as SoLuongTra,
    			0 as GiaTriTra,
    			0 as GiaTriBanTruoc,				
    			ISNULL(hdt.PhaiThanhToan, 0) as GiaTriTraTruoc							
    		FROM BH_HoaDon hdt
    		where hdt.NgayLapHoaDon >=  DateAdd(month, -1, @timeStart) and hdt.NgayLapHoaDon < DateAdd(month, -1, @timeEnd)
    		and hdt.ChoThanhToan = 0
    		and hdt.ID_DonVi = @ID_ChiNhanh
    		and hdt.LoaiHoaDon = 6
	) a group by a.ID_DonVi
)b
END");

            Sql(@"ALTER PROCEDURE [dbo].[CheckThucThu_TongSuDung]
    @ID_DoiTuong [uniqueidentifier],
    @ID_TheGiaTri [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tongthu float= 0, @tongtiendieuchinh float = 0
    	declare @tongkhuyenmai float= 0
    	declare @tongsudung float= 0
    
    	declare @dateHD datetime = (select NgayLapHoaDon from  BH_HoaDon where ID = @ID_TheGiaTri)


		--- sum sotien dieuchinh den thoidiem hientai
		set @tongtiendieuchinh = (select top 1 TongTienThue as TongThu1
		from BH_HoaDon
		where LoaiHoaDon= 23 and ChoThanhToan=0
		and ID_DoiTuong= @ID_DoiTuong
		and NgayLapHoaDon < @dateHD
		order by NgayLapHoaDon desc)
    
    	-- get tongthu den thoi diem hientai
    	select 
    		@tongthu = sum(qct.TienThu)
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    	join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
    	where qct.ID_DoiTuong= @ID_DoiTuong
    	and hd.ChoThanhToan is not null
    	and hd.LoaiHoaDon= 22
    	and qhd.TrangThai='1'
    	and qhd.NgayLapHoaDon < @dateHD -- chi so sanh den phut
    	group by hd.ID_DoiTuong

		-- get gtrikhuyenmai (vi giatri dc sử dụng của thẻ = gtri khuyến mại + phải thanh toán)
		select 
    		@tongkhuyenmai = sum(hd.TongChietKhau) + sum (TongGiamGia)
    	from BH_HoaDon hd 
    	where hd.ID_DoiTuong= @ID_DoiTuong
    	and hd.ChoThanhToan is not null
    	and hd.LoaiHoaDon= 22
    	and hd.NgayLapHoaDon  < @dateHD 
    	group by hd.ID_DoiTuong
    
    	-- get all tongsudung
    	select 
    		@tongsudung= sum(qct.TienThu)
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    	join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
    	where qct.ID_DoiTuong= @ID_DoiTuong
    	and hd.ChoThanhToan is not null
    	and hd.LoaiHoaDon in (1,19)
    	and qhd.TrangThai='1'
    	and qhd.LoaiHoaDon = 11
    	and qct.HinhThucThanhToan = 4
    	group by hd.ID_DoiTuong
    
    	declare @return bit='1'
    	if isnull(@tongtiendieuchinh,0) +  isnull(@tongthu,0) + isnull(@tongkhuyenmai,0) < isnull(@tongsudung,0)
    		set @return='0'
    	select @return as Exist
		
END");

            Sql(@"ALTER PROCEDURE [dbo].[DiscountSale_byIDNhanVien]
    @IDChiNhanhs [nvarchar](max),
    @ID_NhanVien [nvarchar](40),
    @FromDate [datetime],
    @ToDate [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	set @ToDate = dateadd(day,1, @ToDate) 
    
    	declare @tblSale_NVLapHD table(LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, IDChiTietCK uniqueidentifier)
    	insert into @tblSale_NVLapHD
    	select * from dbo.DiscountSale_NVLapHoaDon(@IDChiNhanhs, @FromDate, @ToDate,@ID_NhanVien)
    
    	declare @tblSale_NBanHang table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float,IDChiTietCK uniqueidentifier)
    	insert into @tblSale_NBanHang
    	select * from dbo.DiscountSale_NVBanHang (@IDChiNhanhs,@FromDate,@ToDate,@ID_NhanVien)
    
    	declare @tblSale_NVDichVu table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, HoaHongDoanhThu float, IDChiTietCK uniqueidentifier)
    	insert into @tblSale_NVDichVu
    	select * from dbo.DiscountSale_NVienDichVu (@IDChiNhanhs,@FromDate,@ToDate,@ID_NhanVien);
    
    	select 
    		case a.LoaiNhanVienApDung
    		when 1 then N'Nhân viên bán hàng'
    		when 2 then N'Nhân viên thực hiện/tư vấn'
    		when 3 then N'Nhân viên lập hóa đơn'
    		end as LoaiNVApDung,
			case dt.TinhChietKhauTheo
				when 1 then N'Theo thực thu'
				when 2 then N'Theo doanh thu'
			else '' end as HinhThuc,    		
    		ct.DoanhThuTu, ct.DoanhThuDen, ct.GiaTriChietKhau, ct.LaPhanTram,
    		DoanhThu, ThucThu, HoaHongDoanhThu +  HoaHongThucThu as HoaHong,		
    		dt.ApDungTuNgay, dt.ApDungDenNgay
    	from
    	(
    	select * from @tblSale_NVLapHD 
    	union all
    	select * from  @tblSale_NBanHang 
    	union all
    	select LoaiNhanVienApDung, ID_NhanVien, DoanhThu, 0 as ThucThu, HoaHongDoanhThu, 0 as HoaHongThucThu, IDChiTietCK from @tblSale_NVDichVu 
    	) a
    join ChietKhauDoanhThu_ChiTiet ct on a.IDChiTietCK= ct.ID
    	join ChietKhauDoanhThu dt on ct.ID_ChietKhauDoanhThu= dt.ID
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetAllKhachHang_NotWhere]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_NhanVienLogin [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    	
    	declare @xemAll bit=( select LaAdmin from HT_NguoiDung where ID_NhanVien = @ID_NhanVienLogin)
    
    	declare @tblNhanVien table (ID_NhanVien uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'KhachHang_XemDS_PhonBan','KhachHang_XemDS_HeThong');
    
    
    	declare @tblIDNhoms table (ID varchar(36))
    	declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where ID_DonVi like @ID_ChiNhanh)
    			insert into @tblIDNhoms(ID) values ('00000000-0000-0000-0000-000000000000')
    
    			if @QLTheoCN = 1
    				begin									
    					insert into @tblIDNhoms(ID)
    					select *  from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = 1 
    						union all
    						-- get Nhom at this ChiNhanh
    						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where ID_DonVi like @ID_ChiNhanh) tbl
    				end
    			else
    				begin				
    				-- insert all
    				insert into @tblIDNhoms(ID)
    				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    				where LoaiDoiTuong = 1
    				end	
    
    				  SELECT  distinct *
    		FROM
    		(
    		  SELECT 
    		  dt.ID as ID,
    			  dt.LoaiDoiTuong,
    		  dt.MaDoiTuong, 
    			  dt.TheoDoi,
    			  case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
    			  case when dt.TenDoiTuong_KhongDau is null then '' else ltrim(dt.TenDoiTuong_KhongDau) end as TenDoiTuong_KhongDau,   		 
    		  dt.TenDoiTuong_ChuCaiDau,
    			  dt.ID_TrangThai,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
    			  dt.NgayGiaoDichGanNhat,
    		  dt.DienThoai,
    		  dt.Email,
    		  dt.DiaChi,
    		  dt.MaSoThue,
    		  ISNULL(dt.GhiChu,'') as GhiChu,
    		  dt.NgayTao,
    		  dt.DinhDang_NgaySinh,
    		  ISNULL(dt.NguoiTao,'') as NguoiTao,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    		  dt.LaCaNhan,
    		  ISNULL(dt.TongTichDiem,0) as TongTichDiem,
    			  case when right(rtrim(dt.TenNhomDoiTuongs),1) =',' then LEFT(Rtrim(dt.TenNhomDoiTuongs), len(dt.TenNhomDoiTuongs)-1) else ISNULL(dt.TenNhomDoiTuongs,N'Nhóm mặc định') end as TenNhomDT,-- remove last coma
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
    			  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri, 
    			  concat(dt.MaDoiTuong,' ',lower(dt.MaDoiTuong) ,' ', dt.TenDoiTuong,' ', dt.DienThoai,' ', dt.TenDoiTuong_KhongDau)  as Name_Phone
    		  FROM DM_DoiTuong dt 
			    left join DM_DoiTuong_Nhom dtn on dt.ID= dtn.ID_DoiTuong
			  where dt.ID not like '%00000000-0000-0000-0000-0000%'	
    				and dt.TheoDoi= 0
    				and dt.LoaiDoiTuong= 1			
			  	and exists (select ID from @tblIDNhoms nhom where dtn.ID_NhomDoiTuong = nhom.ID OR dtn.ID_DoiTuong is null)	
    		)b
    			--where b.ID not like '%00000000-0000-0000-0000-0000%'	
    				--and b.TheoDoi= 0
    				--and b.LoaiDoiTuong= 1
    				--and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    				----and (@xemAll ='1' or 
    				----(EXISTS(SELECT ID_NhanVien from @tblNhanVien nv where b.ID_NhanVienPhuTrach= nv.ID_NhanVien)
    				----or b.ID_NhanVienPhuTrach is null))
    				--order by b.ngaytao desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetBangCongNhanVien]
    @IDChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [uniqueidentifier],
    @IDPhongBans [nvarchar](max),
    @IDCaLamViecs [nvarchar](max),
    @TextSearch [nvarchar](max),
    @FromDate [nvarchar](10),
    @ToDate [nvarchar](10),
    @CurrentPage [int],
    @PageSize [int],
	@TrangThaiNV varchar(10)
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDChiNhanhs,'BangCong_XemDS_PhongBan','BangCong_XemDS_HeThong');
    
	declare @tblTrangThaiNV table(TrangThaiNV int)
    	insert into @tblTrangThaiNV
    	select name from dbo.splitstring(@TrangThaiNV)

    	declare @tblPhong table(ID uniqueidentifier)
    	if @IDPhongBans=''	
    		insert into @tblPhong
    		select ID from NS_PhongBan
    	else
    		insert into @tblPhong
    		select name from dbo.splitstring(@IDPhongBans)
    
    	declare @tblca table(ID_CaLamViec uniqueidentifier)
    	if @IDCaLamViecs ='%%'
    		insert into @tblca
    		select ID from NS_CaLamViec
    	else
    		insert into @tblca
    		select Name from dbo.splitstring(@IDCaLamViecs);
    
    		with data_cte
    		as(
    
    		select nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien,
			iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) as TrangThaiNV,
    			cast(congnv.CongNgayThuong as float) as CongChinh, cast(congnv.CongNgayNghiLe as float) as CongLamThem,
    			cast(congnv.OTNgayThuong as float) as OTNgayThuong, 
    			congnv.OTNgayNghiLe as OTNgayNghiLe,
    			cast(congnv.OTNgayThuong + congnv.OTNgayNghiLe as float) as SoGioOT,
    			cast(congnv.SoPhutDiMuon as float) as SoPhutDiMuon
    		from
    			(select cong.ID_NhanVien,
    				sum(cong.CongNgayThuong) as CongNgayThuong,
    				sum(CongNgayNghiLe) as CongNgayNghiLe,
    				sum(OTNgayThuong) as OTNgayThuong,
    				sum(OTNgayNghiLe) as OTNgayNghiLe,
    				sum(SoPhutDiMuon) as SoPhutDiMuon
    			from
    				(select bs.ID_ChamCongChiTiet, bs.ID_CaLamViec, ca.TongGioCong as TongGioCong1Ca, ca.TenCa, bs.ID_NhanVien,
    					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon,
    					IIF(bs.LoaiNgay=0, bs.Cong,0) as CongNgayThuong,
    					IIF(bs.LoaiNgay!=0, bs.Cong,0) as CongNgayNghiLe,
    					IIF(bs.LoaiNgay=0, bs.SoGioOT,0) as OTNgayThuong,
    					IIF(bs.LoaiNgay!=0, bs.SoGioOT,0) as OTNgayNghiLe
    				from NS_CongBoSung bs
    				join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
    				where NgayCham >= @FromDate and NgayCham <= @ToDate
    				and bs.TrangThai !=0
    				and exists(select ID from @tblNhanVien nv where bs.ID_NhanVien= nv.ID)
    				and exists(select ID_CaLamViec from @tblca ca where bs.ID_CaLamViec= ca.ID_CaLamViec)
    				and exists(select Name from dbo.splitstring(@IDChiNhanhs) dv where bs.ID_DonVi= dv.Name)
    				) cong
    			group by cong.ID_NhanVien
    			) congnv
    			join NS_NhanVien nv on congnv.ID_NhanVien= nv.ID 
    			join
    				( select nv.ID, nv.MaNhanVien, nv.TenNhanVien
    				from NS_NhanVien nv 
    				left join NS_QuaTrinhCongTac ct on nv.ID= ct.ID_NhanVien
    				where exists (select ID from @tblPhong pb where pb.ID= ct.ID_PhongBan)
    				and exists(select Name from dbo.splitstring(@IDChiNhanhs) dv where ct.ID_DonVi= dv.Name)
    				) congtac on nv.ID= congtac.ID
    			WHERE ((select count(Name) from @tblSearchString b 
    				where nv.TenNhanVien like '%'+b.Name+'%'  						
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'
    				)=@count or @count=0)	
				and exists (select TrangThaiNV from @tblTrangThaiNV tt
				where iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) = tt.TrangThaiNV)
    		),
    		count_cte
    		as
    		( SELECT COUNT(*) AS TotalRow, 
    			CEILING(COUNT(*) / CAST(@PageSize as float )) as TotalPage ,
    			cast(sum(CongChinh) as float) as TongCong,
    			cast(sum(CongLamThem) as float)as TongCongNgayNghi,
    			cast(sum(SoGioOT) as float) as TongOT,
    			cast(sum(SoPhutDiMuon) as float) as TongDiMuon
    
    		FROM Data_CTE
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaNhanVien
    		OFFSET (@CurrentPage * @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetInforProduct_ByIDQuiDoi]
    @IDQuiDoi [uniqueidentifier],
    @ID_ChiNhanh [uniqueidentifier],
	@ID_LoHang uniqueidentifier null
AS
BEGIN
    SET NOCOUNT ON;
	if	@ID_LoHang is null
		set @ID_LoHang='00000000-0000-0000-0000-000000000000'

    		Select top 50
    			qd.ID as ID_DonViQuiDoi,
    			hh.ID,
    			qd.MaHangHoa,
    			hh.TenHangHoa,
    			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    			qd.TenDonViTinh,
    			hh.LaHangHoa,
    			Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
    			qd.GiaBan,
    			qd.GiaNhap,
				isnull(tk.TonKho,0) as TonKho,			
    			Case when lh.ID is null then null else lh.ID end as ID_LoHang,
    			lh.MaLoHang,
    			lh.NgaySanXuat,
    			lh.NgayHetHan,
				qd.LaDonViChuan,
				hh.ID_NhomHang as ID_NhomHangHoa,
				Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
				Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
			case when ISNULL(QuyCach,0) = 0 then TyLeChuyenDoi else QuyCach * TyLeChuyenDoi end as QuyCach,
			ISNULL(hh.DonViTinhQuyCach,'0') as DonViTinhQuyCach,
			ISNULL(QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
			ISNULL(ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
			ISNULL(LoaiBaoHanh,0) as LoaiBaoHanh,
			ISNULL(SoPhutThucHien,0) as SoPhutThucHien, 
			ISNULL(hh.GhiChu,'') as GhiChuHH ,
			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio, 
			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem
    	from DonViQuiDoi qd    	
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
    	left join DM_LoHang lh on qd.ID_HangHoa = lh.ID_HangHoa and (lh.TrangThai = 1 or lh.TrangThai is null)
		left join DM_HangHoa_TonKho tk on qd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and tk.ID_DonVi = @ID_ChiNhanh
    	left join DM_GiaVon gv on qd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or lh.ID is null) and gv.ID_DonVi = @ID_ChiNhanh
    	where qd.ID = @IDQuiDoi
		and iif(@ID_LoHang='00000000-0000-0000-0000-000000000000', @ID_LoHang , lh.ID ) = @ID_LoHang
		
END");

            Sql(@"ALTER PROCEDURE [dbo].[getListDanhSachHHImport]
    @MaLoHangIP [nvarchar](max),
    @MaHangHoaIP [nvarchar](max),
    @ID_DonViIP [uniqueidentifier],
    @TimeIP [datetime]
AS
BEGIN
    
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
			dvqd.GiaBan,
    	Case when lh.ID is null then '' else lh.MaLoHang end as MaLoHang,
    	Case when gv.ID is null then 0 else Cast(round(gv.GiaVon, 0) as float) end as GiaVon,
    		hhtonkho.TonKho as TonKho,
    		Case when lh.ID is null then'' else lh.NgayHetHan end as NgayHetHan,
			Case when lh.ID is null then'' else lh.NgaySanXuat end as NgaySanXuat
    	FROM 

    	DonViQuiDoi dvqd 
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_LoHang lh on lh.ID_HangHoa = hh.ID and lh.MaLoHang = @MaLoHangIP 
    		left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or gv.ID_LoHang is null) and gv.ID_DonVi = @ID_DonViIP)
			left join DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and (hhtonkho.ID_LoHang = gv.ID_LoHang or gv.ID_LoHang is null) and hhtonkho.ID_DonVi = @ID_DonViIP
    	where dvqd.MaHangHoa = @MaHangHoaIP 
    		and dvqd.Xoa = 0
    		and hh.TheoDoi = 1 
    	order by NgayHetHan
   
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListSuDungThe]
    @ID_DoiTuong [nvarchar](max),
	@FromDate datetime,
	@ToDate datetime,
	@CurrentPage int,
	@PageSize int
AS
BEGIN
    DECLARE @TableTheGT TABLE(ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), MaHoaDonSQ NVARCHAR(MAX), NgayLapHoaDon DATETIME, DienGiai NVARCHAR(MAX),
		LoaiHoaDon INT, SLoaiHoaDon nvarchar(max), LoaiHoaDonSQ INT, PhatSinhTang float, PhatSinhGiam float, TienThe FLOAT, SoDu FLOAT, STT INT)
    	INSERT INTO @TableTheGT
    	select 
    		*,
    		ROW_NUMBER() OVER (PARTITION BY ID ORDER BY NgayLapHoaDon) AS STT
    	from (
    	 -- su dung the giatri
    			select
    			ID,
    			MaHoaDon,
    			MaHoaDonSQ,
    			NgayLapHoaDon,
    			DienGiai,
    			LoaiHoaDon,
				case LoaiHoaDon
					when 1 then N'Bán hàng'
					when 3 then N'Báo giá'
					when 6 then N'Trả hàng'
					when 19 then N'Gói dịch vuk'
					when 25 then N'Sửa chữa'
				else '' end as SLoaiHoaDon,
    			LoaiHoaDonSQ,
				IIF(LoaiHoaDonSQ=12,TienThe,0) as PhatSinhTang,
				IIF(LoaiHoaDonSQ=11,TienThe,0) as PhatSinhGiam,
    			TienThe,
    			SoDu
    			from (
    				SELECT
    				dt.ID,
    				hd.MaHoaDon,
    				qhd.MaHoaDon as MaHoaDonSQ,
    				qhd.NgayLapHoaDon,
    				hd.DienGiai,
    				hd.LoaiHoaDon,
    				qhd.LoaiHoaDon as LoaiHoaDonSQ,
    				qct.TienThu as TienThe,
    				0 as SoDu
    				from DM_DoiTuong dt
    				left join Quy_HoaDon_ChiTiet qct on dt.ID = qct.ID_DoiTuong
    				left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    				left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    				where qct.HinhThucThanhToan = 4 and hd.ChoThanhToan = 0
					and qhd.NgayLapHoaDon >=@FromDate and qhd.NgayLapHoaDon< @ToDate				
    			) tblSuDungThe
    ) tblDoiTuong_The where ID = @ID_DoiTuong
    order by NgayLapHoaDon, MaHoaDonSQ
    	
    	
    			DECLARE @ID UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX);
    			DECLARE @MaHoaDonSQ NVARCHAR(MAX);
    		DECLARE @NgayLapHoaDon DATETIME;
    		DECLARE @DienGiai NVARCHAR(MAX);
    		DECLARE @LoaiHoaDon INT;
    		DECLARE @LoaiHoaDonSQ INT;
    		DECLARE @TienThe FLOAT;
    		DECLARE @SoDu FLOAT;
    			DECLARE @STT INT;
    
    			DECLARE @SoDuLuyKe FLOAT;
    			DECLARE @NgayLapHoaDonSS DATETIME;
    			SET @NgayLapHoaDonSS = GETDATE();
    			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR SELECT ID, MaHoaDon, MaHoaDonSQ, NgayLapHoaDon, DienGiai, LoaiHoaDon, LoaiHoaDonSQ, TienThe, SoDu, STT
    			FROM @TableTheGT
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT INTO @ID, @MaHoaDon, @MaHoaDonSQ, @NgayLapHoaDon, @DienGiai, @LoaiHoaDon, @LoaiHoaDonSQ, @TienThe, @SoDu, @STT
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
    				IF(@NgayLapHoaDonSS != @NgayLapHoaDon)
    				BEGIN
    					SET @SoDuLuyKe = [dbo].[TinhSoDuKHTheoThoiGian](@ID,@NgayLapHoaDon)
    					IF(@LoaiHoaDonSQ = 11)
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe - @TienThe 
    					END
    					ELSE
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe + @TienThe 
    					END
    				END
    				ELSE
    				BEGIN
    					IF(@LoaiHoaDonSQ = 11)
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe - @TienThe 
    					END
    					ELSE
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe + @TienThe 
    					END
    				END
    				
    				SET @NgayLapHoaDonSS = @NgayLapHoaDon
    				UPDATE @TableTheGT SET SoDu = @SoDuLuyKe WHERE STT = @STT
    
    				FETCH NEXT FROM CS_TheGT INTO @ID, @MaHoaDon, @MaHoaDonSQ, @NgayLapHoaDon, @DienGiai, @LoaiHoaDon, @LoaiHoaDonSQ, @TienThe, @SoDu, @STT
    		END
    		CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT;

			with data_cte
			as(		  
    			SELECT * FROM @TableTheGT 
			),
			count_cte
			as (
				select count(ID) as TotalRow,
					CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
					sum(PhatSinhTang) as TongTienTang,
					sum(PhatSinhGiam) as TongTienGiam					
				from data_cte
			)
			select dt.*, cte.*
			from data_cte dt
			cross join count_cte cte
			order by dt.NgayLapHoaDon desc
			OFFSET (@CurrentPage* @PageSize) ROWS
			FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[insert_DieuChinhGiaVon_ChiTiet]
    @ID_HoaDon [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
	@ID_LoHang [uniqueidentifier],
	@ID_DonVi [uniqueidentifier],
    @SoThuTu [int],
    @GiaVonHienTai [float],
    @GiaVonMoi [float],
    @GiaVonTang [float],
    @GiaVonGiam [float],
    @loaiInsert [int],
    @ChoThanhToan [bit],
    @dk_Xoa [int]
AS
BEGIN
-- @loaiInsert: 1.insert, 2.update (khong chay ham nay)
    IF (@loaiInsert = 1)
    		BEGIN
    			Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang)
    			Values(NEWID(), @ID_HoaDon, @SoThuTu,'0',@GiaVonHienTai, @GiaVonMoi, @GiaVonTang, @GiaVonGiam, '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang)
    			if (@ChoThanhToan = 0)
    			BEGIN
					-- update gianhap in DonViQuiDoi neu dieuchinhgiavon
    					--update DonViQuiDoi set GiaNhap = @GiaVonMoi where ID = @ID_DonViQuiDoi
					 delete DM_GiaVon where ID_DonVi = @ID_DonVi and ID_DonViQuiDoi = @ID_DonViQuiDoi and (ID_LoHang = @ID_LoHang or ID_LoHang is null)
					 insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
    				 Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVonMoi)
    			END
    		END
    	ElSE
    		BEGIN
    			if(@dk_Xoa = 0)
    			BEGIN
    				delete BH_HoaDon_ChiTiet where ID_HoaDon = @ID_HoaDon					
    			END
    		BEGIN
    			Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi, ID_LoHang)
    			Values(NEWID(), @ID_HoaDon, @SoThuTu,'0',@GiaVonHienTai, @GiaVonMoi, @GiaVonTang, @GiaVonGiam, '0','0','0','0','0','0', @ID_DonViQuiDoi, @ID_LoHang)
    			if (@ChoThanhToan = 0)
    			BEGIN
    				 delete DM_GiaVon where ID_DonVi = @ID_DonVi and ID_DonViQuiDoi = @ID_DonViQuiDoi and (ID_LoHang = @ID_LoHang or ID_LoHang is null)
					 insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
    				 Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVonMoi)

					 -- update gianhap in DonViQuiDoi neu dieuchinhgiavon
					 --update DonViQuiDoi set GiaNhap = @GiaVonMoi where ID = @ID_DonViQuiDoi 
    			END
    		END
    		END
END");
                
            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhMonth_GiaVonBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @tblThang TABLE (ThangLapHoaDon INT, TongGiaVonBan FLOAT, TongGiaVonTra FLOAT);
INSERT INTO @tblThang (ThangLapHoaDon, TongGiaVonBan, TongGiaVonTra)
VALUES (1, 0, 0), (2,0,0), (3,0,0), (4,0,0), (5,0,0), (6,0,0), (7,0,0), (8,0,0), (9,0,0), (10,0,0), (11,0,0), (12,0,0);
	SELECT 
	b.ThangLapHoaDon,
	SUM(CAST(ROUND(b.TongGiaVonBan, 0) as float)) as TongGiaVonBan,
	SUM(CAST(ROUND(b.TongGiaVonTra , 0) as float)) as TongGiaVonTra
	FROM
	(
    SELECT
    	a.ThangLapHoaDon,
		SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
		SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    	FROM
    	(
    		Select 
    		DATEPART(MONTH, hd.NgayLapHoaDon) as ThangLapHoaDon,
			hdct.ID_DonViQuiDoi,
			hdct.SoLuong as SoLuongBan,
			ISNULL(hdct.GiaVon, 0) as GiaVonBan,
			0 as SoLuongTra,
			0 as GiaVonTra,
    		0 as SoLuongTraNhanh,
			0 as GiaVonTraNhanh
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		where hd.LoaiHoaDon in (1,19)
			and hdct.ID_ChiTietGoiDV is null
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			Union all
			Select 
    		DATEPART(MONTH, hdb.NgayLapHoaDon) as ThangLapHoaDon,
			hdct.ID_DonViQuiDoi,
			0 as SoLuongBan,
			ISNULL(ctb.GiaVon, 0) as GiaVonBan,
			hdct.SoLuong as SoLuongTra,
			0 as GiaVonTra,
    		0 as SoLuongTraNhanh,
			0 as GiaVonTraNhanh
    		From BH_HoaDon hdb
			inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    		where hdb.LoaiHoaDon in (1,19)
			and hdct.ID_ChiTietGoiDV is null
    		and DATEPART(YEAR, hdt.NgayLapHoaDon) = @year
    		and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			UNION ALL
    		SELECT
    		DATEPART(MONTH, hdb.NgayLapHoaDon) as ThangLapHoaDon,
			hdct.ID_DonViQuiDoi,
			0 as SoLuongBan,
			0 as GiaVonBan,
			0 as SoLuongTra,
			0 as GiaVonTra,
			hdct.SoLuong as SoLuongTraNhanh,
			ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		where DATEPART(YEAR, hdb.NgayLapHoaDon) = @year
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_HoaDon is null
    	) as a
    	GROUP BY a.ID_DonViQuiDoi, a.ThangLapHoaDon
		UNION ALL SELECT * FROM @tblThang)
		as b
		
		GROUP BY b.ThangLapHoaDon
END

");
            
            Sql(@"ALTER PROCEDURE [dbo].[ReportTaiChinhYear_GiaVonBanHang]
    @year [int],
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
DECLARE @tblThang TABLE (NamLapHoaDon INT, TongGiaVonBan FLOAT, TongGiaVonTra FLOAT);
INSERT INTO @tblThang (NamLapHoaDon, TongGiaVonBan, TongGiaVonTra)
VALUES (@year, 0, 0);
	SELECT 
	b.NamLapHoaDon,
	SUM(CAST(ROUND(b.TongGiaVonBan, 0) as float)) as TongGiaVonBan,
	SUM(CAST(ROUND(b.TongGiaVonTra , 0) as float)) as TongGiaVonTra
	FROM
	(
    SELECT
    	a.NamLapHoaDon,
		SUM((a.SoLuongBan - (a.SoLuongTra)) * a.GiaVonBan) as TongGiaVonBan,
		SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    	FROM
    	(
    		Select 
    		DATEPART(YEAR, hd.NgayLapHoaDon) as NamLapHoaDon,
			hdct.ID_DonViQuiDoi,
			hdct.SoLuong as SoLuongBan,
			ISNULL(hdct.GiaVon, 0) as GiaVonBan,
			0 as SoLuongTra,
			0 as GiaVonTra,
    		0 as SoLuongTraNhanh,
			0 as GiaVonTraNhanh
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		where (hd.LoaiHoaDon = 1 Or hd.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @year
    		and hd.ChoThanhToan = 0
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			Union all
			Select 
    		DATEPART(YEAR, hdt.NgayLapHoaDon) as NamLapHoaDon,
			hdct.ID_DonViQuiDoi,
			NULL as SoLuongBan,
			ISNULL(ctb.GiaVon, 0) as GiaVonBan,
			hdct.SoLuong as SoLuongTra,
			0 as GiaVonTra,
    		0 as SoLuongTraNhanh,
			0 as GiaVonTraNhanh
    		From BH_HoaDon hdb
			inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    		inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
			join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    		where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and DATEPART(YEAR, hdt.NgayLapHoaDon) = @year
    		and hdt.ChoThanhToan = 0
    		and hdb.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
			UNION ALL
    		SELECT
    		DATEPART(YEAR, hdb.NgayLapHoaDon) as NamLapHoaDon,
			hdct.ID_DonViQuiDoi,
			0 as SoLuongBan,
			0 as GiaVonBan,
			0 as SoLuongTra,
			0 as GiaVonTra,
			hdct.SoLuong as SoLuongTraNhanh,
			ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    		FROM
    		BH_HoaDon hdb
    		join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		where DATEPART(YEAR, hdb.NgayLapHoaDon) = @year
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_HoaDon is null
    	) as a
    	GROUP BY a.ID_DonViQuiDoi, a.NamLapHoaDon
		UNION ALL SELECT * FROM @tblThang
		) as b
		GROUP BY b.NamLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[Search_DMHangHoa_TonKho]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON;
    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    						Case when nd.LaAdmin = '1' then '1' else
    						Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    						From
    						HT_NguoiDung nd	
    						where nd.ID = @ID_NguoiDung)
    DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    	Select @count =  (Select count(*) from @tablename);
    	Select @countChar =   (Select count(*) from @tablenameChar);

select qd.ID as ID_DonViQuiDoi,
		MaHangHoa, TenHangHoa, TenHangHoa_KhongDau, TenHangHoa_KyTuDau,TenDonViTinh, ThuocTinhGiaTri as ThuocTinh_GiaTri,
		CONCAT(TenHangHoa,' ', ThuocTinhGiaTri,' ', case when TenDonViTinh='' or TenDonViTinh is null then '' else ' (' + TenDonViTinh + ')' end) as TenHangHoaFull,
		ISNULL(tk.TonKho,0) as TonCuoiKy,
		CAST(ROUND((qd.GiaBan), 0) as float) as GiaBan,		
		case when @XemGiaVon= '1' then CAST(ROUND((ISNULL(gv.GiaVon,0)), 0) as float) else 0 end as GiaVon,
		case when @XemGiaVon= '1' then	
			case when hh.LaHangHoa='1' then CAST(ROUND((ISNULL(gv.GiaVon,0)), 0) as float)
			else CAST(ROUND((ISNULL(tblDVu.GiaVon,0)), 0) as float) end
		else 0 end as GiaVon,
		gv.ID_DonVi, hh.LaHangHoa
	from DonViQuiDoi qd 
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	left join DM_HangHoa_TonKho tk on qd.ID= tk.ID_DonViQuyDoi
	left join DM_GiaVon gv on qd.id= gv.ID_DonViQuiDoi
	left join (select qd2.ID,sum(dl.SoLuong *  ISNULL(gv.GiaVon,0)) as GiaVon
				from DonViQuiDoi qd2
				join DinhLuongDichVu dl on qd2.ID= dl.ID_DichVu
				left join DM_GiaVon gv on dl.ID_DonViQuiDoi= gv.ID_DonViQuiDoi
				where gv.ID_DonVi=@ID_ChiNhanh 
				group by qd2.ID
				) tblDVu on qd.ID= tblDVu.ID
	where qd.Xoa= 0 and hh.TheoDoi=1
	and ((tk.ID_DonVi = @ID_ChiNhanh and hh.LaHangHoa='1') or hh.LaHangHoa=0)
	and ((gv.ID_DonVi= @ID_ChiNhanh) or gv.ID_DonVi is null )
	and	((select count(*) from @tablename b where 
    		qd.MaHangHoa like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		--or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
			)=@count or @count=0)	 
    	and	qd.Xoa = 0
	order by tk.TonKho desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_Get_ChietKhauDoanhThu_byDonVi]
    @ID_DonVi [nvarchar](max),
	@LoaiNVApDung varchar(10),
	@TrangThaiConHan varchar(10),
	@CurrentPage int,
	@PageSize int
AS
BEGIN
set nocount on;
declare @today varchar(14) = format(getdate(),'yyyyMMdd');

with data_cte
as
(
select *
from
(
    select ID, TinhChietKhauTheo,ApDungTuNgay, ApDungDenNgay, GhiChu,NgayTao,
    		case when TinhChietKhauTheo= 3 then cast('0' as bit) else cast('1' as bit)  end as LaPhanTram,
			case when ApDungDenNgay is null or format(ApDungDenNgay,'yyyyMMdd') >=@today then '10' else '12' end as TrangThaiConHan,
			ISNULL(LoaiNhanVienApDung, '3') as LoaiNhanVienApDung,
			case LoaiNhanVienApDung
				when 1 then N'Nhân viên bán hàng'
				when 2 then N'Nhân viên thực hiện/ tư vấn dịch vụ'
				when 3 then N'Nhân viên lập hóa đơn'
				end as Text_LoaiNhanVienApDung
    	from ChietKhauDoanhThu
    	where ID_DonVi like @ID_DonVi
		and TrangThai !=0
	) tbl where TrangThaiConHan like @TrangThaiConHan
	and LoaiNhanVienApDung like @LoaiNVApDung 
),
count_cte
as (
select count (ID) as TotalRow,
	 CEILING(count (ID)/ cast (@PageSize as float)) as TotalPage
from data_cte
)
select *
from data_cte dt
cross join count_cte
order by dt.NgayTao desc
OFFSET (@CurrentPage* @PageSize) ROWS
FETCH NEXT @PageSize ROWS ONLY
    	
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_PhieuChi_ServicePackage]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    Select 
    	MAX(a.MaHoaDon) as MaHoaDon,
    	CONVERT(varchar, MAX(a.NgayLapHoaDon),103) as NgayLapHoaDon,
    	SUM(a.TienMat) as TienMat,
    	SUM(a.TienGui) as TienGui,
    	SUM(a.TienThu) as TongThu,
    	MAX(a.NoiDungThu) as NoiDungThu,
		N'Chi tiền trả gói dịch vụ' AS LoaiThuChi
    FROM
    (
    	Select MAX(qhd.ID) as ID_QuyHoaDon, 
    		MAX(qhd.MaHoaDon) as MaHoaDon, 
    		MAX(qhd.NgayLapHoaDon) as NgayLapHoaDon,
    		MAX(ISNULL(qct.TienMat, 0)) as TienMat,
    		MAX(ISNULL(qct.TienGui, 0)) as TienGui,
    		MAX(ISNULL(qct.TienThu, 0)) as TienThu, 
    		MAX(qhd.NoiDungThu) as NoiDungThu,
			MAX(hd.LoaiHoaDon) as LoaiHoaDon
    	from Quy_HoaDon qhd 
    	join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    	join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    	where hd.LoaiHoaDon = 6 and (qhd.TrangThai is null OR qhd.TrangThai='1') --- get Quy_HoaDon chua huy
    	and hd.ChoThanhToan = 0  and hd.ID_DoiTuong like @ID_DoiTuong and hd.ID_DonVi like @ID_DonVi
		and exists (select id from BH_HoaDon hd2 where hd2.LoaiHoaDon= 19 and hd2.ChoThanhToan= 0 and hd.ID_HoaDon= hd2.ID)
    	group by qct.ID
    )a
    GROUP BY a.ID_QuyHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[TongQuanBieuDoDoanhThuThuan]
    @LoaBieuDo [int],
    @Thang [int],
    @Nam [nvarchar](max),
    @IdChiNhanhs [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	
    DECLARE @tblDonVi TABLE(ID UNIQUEIDENTIFIER);
    INSERT INTO @tblDonVi
    select * from splitstring(@IdChiNhanhs);
    
    
    DECLARE @tblDoanThu TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, DoanhThuThuan FLOAT);
    DECLARE @tblGiaVon TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, GiaVon FLOAT);
    DECLARE @tblChiPhi TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, ChiPhi FLOAT);
    DECLARE @tblThuNhapKhac TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, ThuNhapKhac FLOAT, ChiPhiKhac FLOAT);
    IF(@LoaBieuDo = 1) -- Theo Ngày
    BEGIN
    	--DoanhThu
    	INSERT INTO @tblDoanThu
    	SELECT
    	a.Ngay, a.ID_DonVi,
    		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		DAY(hd.NgayLapHoaDon) as Ngay,
    		hd.LoaiHoaDon,
    			hd.ID_DonVi,
    		Case When hd.LoaiHoaDon in (1, 19,25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
    			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon in (1, 19,25) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon in (1,6, 19,25)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @Nam AND MONTH(hd.NgayLapHoaDon) = @Thang
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Ngay, a.ID_DonVi
    		--GiaVon
    		INSERT INTO @tblGiaVon
    		SELECT 
    		b.Ngay,
    		b.ID_DonVi,
    		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
    		FROM
    		(
    		SELECT
    		a.Ngay,
    			a.ID_DonVi,
    			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
    			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			DAY(hd.NgayLapHoaDon) as Ngay,
    				hd.ID_DonVi,
    				hdct.SoLuong as SoLuongBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hd.NgayLapHoaDon) = @Nam AND MONTH(hd.NgayLapHoaDon) = @Thang
    			and hd.ChoThanhToan = 0
    				Union all
    				Select 
    			DAY(hdb.NgayLapHoaDon) as Ngay,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
    				hdct.SoLuong as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
    				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hdt.NgayLapHoaDon) = @Nam AND MONTH(hdt.NgayLapHoaDon) = @Thang
    			and hdt.ChoThanhToan = 0
    				UNION ALL
    			SELECT
    			DAY(hdb.NgayLapHoaDon) as Ngay,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				0 as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    				hdct.SoLuong as SoLuongTraNhanh,
    				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where YEAR(hdb.NgayLapHoaDon) = @Nam AND MONTH(hdb.NgayLapHoaDon) = @Thang
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
    				and hdb.ID_HoaDon is null
					UNION ALL
			select DAY(hdxk.NgayLapHoaDon) AS Ngay, 
			hdsc.ID_DonVi, 
			1 AS SoLuongBan, 
			hdxk.PhaiThanhToan AS GiaVonBan,
			0 AS SoLuongTra,
			0 AS GiaVonTra,
			0 AS SoLuongTraNhanh,
			0 AS GiaVonTraNhanh
			from BH_HoaDon hdsc
			inner join BH_HoaDon hdxk ON hdsc.ID = hdxk.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hdxk.ID_DonVi
			where hdsc.LoaiHoaDon = 25 and hdsc.ChoThanhToan = 0
			and hdxk.ChoThanhToan = 0 AND YEAR(hdxk.NgayLapHoaDon) = @Nam AND MONTH(hdsc.NgayLapHoaDon) = @Thang
    		) as a
    		GROUP BY a.Ngay, a.ID_DonVi
    			) as b
    			GROUP BY b.Ngay, b.ID_DonVi
    			--Chiphi
    		INSERT INTO @tblChiPhi
    		SELECT
    	a.Ngay,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		DAY(hd.NgayLapHoaDon) as Ngay,
    			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon = 8
    		and YEAR(hd.NgayLapHoaDon) = @Nam AND MONTH(hd.NgayLapHoaDon) = @Thang
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		DAY(qhd.NgayLapHoaDon) as Ngay,
    			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam AND MONTH(qhd.NgayLapHoaDon) = @Thang
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Ngay, a.ID_DonVi;
    
    		--Thu nhap khac
    		INSERT INTO @tblThuNhapKhac
    		SELECT
    	a.Ngay,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		DAY(qhd.NgayLapHoaDon) as Ngay,
    			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or (hd.LoaiHoaDon in (1,3,19,25) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when hd.LoaiHoaDon in (1,3,25) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam AND MONTH(qhd.NgayLapHoaDon) = @Thang
    		and (qhd.HachToanKinhDoanh = 1)
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0) and qhdct.LoaiThanhToan != 1
    	) as a
    	GROUP BY
    	a.Ngay, a.ID_DonVi
    END
    ELSE IF (@LoaBieuDo = 2) --Theo tháng
    BEGIN
    	--DoanhThu
    	INSERT INTO @tblDoanThu
    	SELECT
    	a.Thang, a.ID_DonVi,
    		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		MONTH(hd.NgayLapHoaDon) as Thang,
    		hd.LoaiHoaDon,
    			hd.ID_DonVi,
    		Case When hd.LoaiHoaDon in (1, 19,25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
    			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon in (1, 19,25) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon in (1,6, 19,25)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Thang, a.ID_DonVi
    		--GiaVon
    		INSERT INTO @tblGiaVon
    		SELECT 
    		b.Thang,
    		b.ID_DonVi,
    		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
    		FROM
    		(
    		SELECT
    		a.Thang,
    			a.ID_DonVi,
    			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
    			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			MONTH(hd.NgayLapHoaDon) as Thang,
    				hd.ID_DonVi,
    				hdct.SoLuong as SoLuongBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hd.NgayLapHoaDon) = @Nam
    			and hd.ChoThanhToan = 0
    				Union all
    				Select 
    			MONTH(hdb.NgayLapHoaDon) as Thang,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
    				hdct.SoLuong as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
    				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hdt.NgayLapHoaDon) = @Nam
    			and hdt.ChoThanhToan = 0
    				UNION ALL
    			SELECT
    			MONTH(hdb.NgayLapHoaDon) as Thang,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				0 as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    				hdct.SoLuong as SoLuongTraNhanh,
    				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where YEAR(hdb.NgayLapHoaDon) = @Nam
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
    				and hdb.ID_HoaDon is null
					UNION ALL
			select MONTH(hdxk.NgayLapHoaDon) AS Thang, 
			hdsc.ID_DonVi, 
			1 AS SoLuongBan, 
			hdxk.PhaiThanhToan AS GiaVonBan,
			0 AS SoLuongTra,
			0 AS GiaVonTra,
			0 AS SoLuongTraNhanh,
			0 AS GiaVonTraNhanh
			from BH_HoaDon hdsc
			inner join BH_HoaDon hdxk ON hdsc.ID = hdxk.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hdxk.ID_DonVi
			where hdsc.LoaiHoaDon = 25 and hdsc.ChoThanhToan = 0
			and hdxk.ChoThanhToan = 0 AND YEAR(hdxk.NgayLapHoaDon) = @Nam
    		) as a
    		GROUP BY a.Thang, a.ID_DonVi
    			) as b
    			GROUP BY b.Thang, b.ID_DonVi
    			--Chiphi
    		INSERT INTO @tblChiPhi
    		SELECT
    	a.Thang,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		MONTH(hd.NgayLapHoaDon) as Thang,
    			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon = 8
    		and YEAR(hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		MONTH(qhd.NgayLapHoaDon) as Thang,
    			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Thang, a.ID_DonVi;
    
    		--Thu nhap khac
    		INSERT INTO @tblThuNhapKhac
    		SELECT
    	a.Thang,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		MONTH(qhd.NgayLapHoaDon) as Thang,
    			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or (hd.LoaiHoaDon in (1,3,19,25) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when hd.LoaiHoaDon in (1,3,25) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and (qhd.HachToanKinhDoanh = 1)
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0) and qhdct.LoaiThanhToan != 1
    	) as a
    	GROUP BY
    	a.Thang, a.ID_DonVi
    END
    ELSE IF (@LoaBieuDo = 3) -- Theo Quý
    BEGIN
    	--DoanhThu
    	INSERT INTO @tblDoanThu
    	SELECT
    	a.Quy, a.ID_DonVi,
    		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		DATEPART(QUARTER,hd.NgayLapHoaDon) as Quy,
    		hd.LoaiHoaDon,
    			hd.ID_DonVi,
    		Case When hd.LoaiHoaDon in (1, 19,25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
    			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon in (1, 19,25) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon in (1,6, 19,25)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Quy, a.ID_DonVi
    		--GiaVon
    		INSERT INTO @tblGiaVon
    		SELECT 
    		b.Quy,
    		b.ID_DonVi,
    		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
    		FROM
    		(
    		SELECT
    		a.Quy,
    			a.ID_DonVi,
    			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
    			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			DATEPART(QUARTER,hd.NgayLapHoaDon) as Quy,
    				hd.ID_DonVi,
    				hdct.SoLuong as SoLuongBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hd.NgayLapHoaDon) = @Nam
    			and hd.ChoThanhToan = 0
    				Union all
    				Select 
    			DATEPART(QUARTER,hdb.NgayLapHoaDon) as Quy,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
    				hdct.SoLuong as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
    				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hdt.NgayLapHoaDon) = @Nam
    			and hdt.ChoThanhToan = 0
    				UNION ALL
    			SELECT
    			DATEPART(QUARTER,hdb.NgayLapHoaDon) as Quy,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				0 as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    				hdct.SoLuong as SoLuongTraNhanh,
    				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where YEAR(hdb.NgayLapHoaDon) = @Nam
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
    				and hdb.ID_HoaDon is null
					UNION ALL
			select DATEPART(QUARTER,hdxk.NgayLapHoaDon) AS Thang, 
			hdsc.ID_DonVi, 
			1 AS SoLuongBan, 
			hdxk.PhaiThanhToan AS GiaVonBan,
			0 AS SoLuongTra,
			0 AS GiaVonTra,
			0 AS SoLuongTraNhanh,
			0 AS GiaVonTraNhanh
			from BH_HoaDon hdsc
			inner join BH_HoaDon hdxk ON hdsc.ID = hdxk.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hdxk.ID_DonVi
			where hdsc.LoaiHoaDon = 25 and hdsc.ChoThanhToan = 0
			and hdxk.ChoThanhToan = 0 AND YEAR(hdxk.NgayLapHoaDon) = @Nam
    		) as a
    		GROUP BY a.Quy, a.ID_DonVi
    			) as b
    			GROUP BY b.Quy, b.ID_DonVi
    			--Chiphi
    		INSERT INTO @tblChiPhi
    		SELECT
    	a.Quy,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		DATEPART(QUARTER,hd.NgayLapHoaDon) as Quy,
    			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon = 8
    		and YEAR(hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		DATEPART(QUARTER,qhd.NgayLapHoaDon) as Quy,
    			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Quy, a.ID_DonVi;
    
    		--Thu nhap khac
    		INSERT INTO @tblThuNhapKhac
    		SELECT
    	a.Quy,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		DATEPART(QUARTER,qhd.NgayLapHoaDon) as Quy,
    			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or (hd.LoaiHoaDon in (1,3,19,25) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when hd.LoaiHoaDon in (1,3,25) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and (qhd.HachToanKinhDoanh = 1)
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0) and qhdct.LoaiThanhToan != 1
    	) as a
    	GROUP BY
    	a.Quy, a.ID_DonVi
    END
    ELSE IF (@LoaBieuDo = 4) --Theo năm
    BEGIN
    	DECLARE @tblNam TABLE(Nam INT);
    	INSERT INTO @tblNam
    	select * from splitstring(@Nam);
    	--DoanhThu
    	INSERT INTO @tblDoanThu
    	SELECT
    	a.Nam, a.ID_DonVi,
    		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		YEAR(hd.NgayLapHoaDon) as Nam,
    		hd.LoaiHoaDon,
    			hd.ID_DonVi,
    		Case When hd.LoaiHoaDon in (1, 19,25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
    			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon in (1, 19,25) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
    			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			INNER JOIN @tblNam nam ON nam.Nam = YEAR(hd.NgayLapHoaDon)
    		where hd.LoaiHoaDon in (1,6, 19,25)
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Nam, a.ID_DonVi
    		--GiaVon
    		INSERT INTO @tblGiaVon
    		SELECT 
    		b.Nam,
    		b.ID_DonVi,
    		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
    		FROM
    		(
    		SELECT
    		a.Nam,
    			a.ID_DonVi,
    			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
    			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			YEAR(hd.NgayLapHoaDon) as Nam,
    				hd.ID_DonVi,
    				hdct.SoLuong as SoLuongBan,
    				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    				INNER JOIN @tblNam nam ON nam.Nam = YEAR(hd.NgayLapHoaDon)
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and hd.ChoThanhToan = 0
    				Union all
    				Select 
    			YEAR(hdb.NgayLapHoaDon) as Nam,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
    				hdct.SoLuong as SoLuongTra,
    				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
    				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
    				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    				INNER JOIN @tblNam nam ON nam.Nam = YEAR(hdb.NgayLapHoaDon)
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
    				and hdct.ID_ChiTietGoiDV is null
    			and hdt.ChoThanhToan = 0
    				UNION ALL
    			SELECT
    			YEAR(hdb.NgayLapHoaDon) as Nam,
    				hdb.ID_DonVi,
    				0 as SoLuongBan,
    				0 as GiaVonBan,
    				0 as SoLuongTra,
    				0 as GiaVonTra,
    				hdct.SoLuong as SoLuongTraNhanh,
    				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    				INNER JOIN @tblNam nam ON nam.Nam = YEAR(hdb.NgayLapHoaDon)
    			where hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
    				and hdb.ID_HoaDon is null
					UNION ALL
			select YEAR(hdxk.NgayLapHoaDon) AS Nam, 
			hdsc.ID_DonVi, 
			1 AS SoLuongBan, 
			hdxk.PhaiThanhToan AS GiaVonBan,
			0 AS SoLuongTra,
			0 AS GiaVonTra,
			0 AS SoLuongTraNhanh,
			0 AS GiaVonTraNhanh
			from BH_HoaDon hdsc
			inner join BH_HoaDon hdxk ON hdsc.ID = hdxk.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hdxk.ID_DonVi
			INNER JOIN @tblNam nam ON nam.Nam = YEAR(hdxk.NgayLapHoaDon)
			where hdsc.LoaiHoaDon = 25 and hdsc.ChoThanhToan = 0
			and hdxk.ChoThanhToan = 0 
    		) as a
    		GROUP BY a.Nam, a.ID_DonVi
    			) as b
    			GROUP BY b.Nam, b.ID_DonVi
    			--Chiphi
    		INSERT INTO @tblChiPhi
    		SELECT
    	a.Nam,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		YEAR(hd.NgayLapHoaDon) as Nam,
    			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			INNER JOIN @tblNam nam ON nam.Nam = YEAR(hd.NgayLapHoaDon)
    		where hd.LoaiHoaDon = 8
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		YEAR(qhd.NgayLapHoaDon) as Nam,
    			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    			INNER JOIN @tblNam nam ON nam.Nam = YEAR(qhd.NgayLapHoaDon)
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Nam, a.ID_DonVi;
    
    		--Thu nhap khac
    		INSERT INTO @tblThuNhapKhac
    		SELECT
    	a.Nam,
    		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		YEAR(qhd.NgayLapHoaDon) as Nam,
    			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or (hd.LoaiHoaDon in (1,3,19,25) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when hd.LoaiHoaDon in (1,3,25) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    			INNER JOIN @tblNam nam ON nam.Nam = YEAR(qhd.NgayLapHoaDon)
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and (qhd.HachToanKinhDoanh = 1)
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0) and qhdct.LoaiThanhToan != 1
    	) as a
    	GROUP BY
    	a.Nam, a.ID_DonVi
    END
    SELECT tbl.ID_DonVi, dv.MaDonVi, dv.TenDonVi, CAST(tbl.ThoiGian AS INT) AS ThoiGian, tbl.DoanhThuThuan, tbl.LoiNhuan FROM
    (SELECT IIF(dt.ThoiGian IS NOT NULL, dt.ThoiGian, IIF(gv.ThoiGian IS NOT NULL, gv.ThoiGian, IIF(cp.ThoiGian IS NOT NULL, cp.ThoiGian, tnk.ThoiGian))) AS ThoiGian,
    IIF(dt.ID_DonVi IS NOT NULL, dt.ID_DonVi, IIF(gv.ID_DonVi IS NOT NULL, gv.ID_DonVi, IIF(cp.ID_DonVi IS NOT NULL, cp.ID_DonVi, tnk.ID_DonVi))) AS ID_DonVi,
    ISNULL(dt.DoanhThuThuan, 0) - ISNULL(gv.GiaVon, 0) + ISNULL(tnk.ThuNhapKhac, 0) - ISNULL(tnk.ChiPhiKhac, 0) AS LoiNhuan,
    ISNULL(dt.DoanhThuThuan, 0) AS DoanhThuThuan FROM @tblDoanThu dt
    FULL JOIN @tblGiaVon gv ON dt.ThoiGian = gv.ThoiGian AND dt.ID_DonVi = gv.ID_DonVi
    FULL JOIN @tblChiPhi cp ON dt.ThoiGian = cp.ThoiGian AND dt.ID_DonVi = cp.ID_DonVi
    FULL JOIN @tblThuNhapKhac tnk ON dt.ThoiGian = tnk.ThoiGian AND dt.ID_DonVi = tnk.ID_DonVi) AS tbl
    INNER JOIN DM_DonVi dv ON dv.ID = tbl.ID_DonVi
END");

            Sql(@"ALTER PROCEDURE [dbo].[TongQuanBieuDoThucThu]
    @LoaBieuDo [int],
    @Thang [int],
    @Nam [nvarchar](max),
    @IdChiNhanhs [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	
    DECLARE @tblDonVi TABLE(ID UNIQUEIDENTIFIER);
    INSERT INTO @tblDonVi
    select * from splitstring(@IdChiNhanhs);
    
    
    DECLARE @tblThucThu TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, ThucThu FLOAT);
    IF(@LoaBieuDo = 1) -- Theo Ngày
    BEGIN
    	INSERT INTO @tblThucThu
    	SELECT 
    	a.Ngay,
    	a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
    	FROM
    	(
    	SELECT
    		DAY(qhd.NgayLapHoaDon) as Ngay,
    		qhd.ID_DonVi,
    		qct.TienThu  as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
			LEFT JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		where qhd.LoaiHoaDon = 11
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and (qct.DiemThanhToan is null OR qct.DiemThanhToan =0)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam and MONTH(qhd.NgayLapHoaDon) = @Thang
    		and qhd.HachToanKinhDoanh = 1
			and qct.HinhThucThanhToan NOT IN (4, 5)
			and qct.LoaiThanhToan != 1
			UNION ALL
			SELECT DAY(qhd.NgayLapHoaDon) as Ngay,
    		qhd.ID_DonVi,
    		-qct.TienThu  as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
			LEFT JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		where qhd.LoaiHoaDon = 12 AND hd.LoaiHoaDon = 6
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam and MONTH(qhd.NgayLapHoaDon) = @Thang
    		and qhd.HachToanKinhDoanh = 1
    	) a
    GROUP BY a.Ngay, a.ID_DonVi
    	ORDER BY Ngay
    END
    ELSE IF (@LoaBieuDo = 2) --Theo tháng
    BEGIN
    	INSERT INTO @tblThucThu
    	SELECT 
    	a.Thang,
    	a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
    	FROM
    	(
    	SELECT
    		MONTH(qhd.NgayLapHoaDon) as Thang,
    		qhd.ID_DonVi,
    		qct.TienThu as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
			INNER JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		where qhd.LoaiHoaDon = 11
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and (qct.DiemThanhToan is null OR qct.DiemThanhToan =0)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhd.HachToanKinhDoanh = 1
			and qct.HinhThucThanhToan NOT IN (4, 5)
			and qct.LoaiThanhToan != 1

			UNION ALL
			SELECT MONTH(qhd.NgayLapHoaDon) as Thang,
    		qhd.ID_DonVi,
    		-qct.TienThu  as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
			LEFT JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		where qhd.LoaiHoaDon = 12 AND hd.LoaiHoaDon = 6
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhd.HachToanKinhDoanh = 1
    	) a
    GROUP BY a.Thang, a.ID_DonVi
    	ORDER BY Thang
    END
    ELSE IF (@LoaBieuDo = 3) -- Theo Quý
    BEGIN
    	INSERT INTO @tblThucThu
    	SELECT 
    	a.Quy,
    	a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
    	FROM
    	(
    	SELECT
    		DATEPART(QUARTER,qhd.NgayLapHoaDon) as Quy,
    		qhd.ID_DonVi,
    		qct.TienThu as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
			INNER JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
    		where qhd.LoaiHoaDon = 11
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and (qct.DiemThanhToan is null OR qct.DiemThanhToan =0)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhd.HachToanKinhDoanh = 1
			and qct.HinhThucThanhToan NOT IN (4, 5)
			and qct.LoaiThanhToan != 1

			UNION ALL
			SELECT DATEPART(QUARTER,qhd.NgayLapHoaDon) as Quy,
    		qhd.ID_DonVi,
    		-qct.TienThu  as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
			LEFT JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		where qhd.LoaiHoaDon = 12 AND hd.LoaiHoaDon = 6
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhd.HachToanKinhDoanh = 1
    	) a
    GROUP BY a.Quy, a.ID_DonVi
    	ORDER BY Quy
    END
    ELSE IF (@LoaBieuDo = 4) --Theo năm
    BEGIN
    	DECLARE @tblNam TABLE(Nam INT);
    	INSERT INTO @tblNam
    	select * from splitstring(@Nam);
    
    	INSERT INTO @tblThucThu
    	SELECT 
    	a.Nam,
    	a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
    	FROM
    	(
    	SELECT
    		YEAR(qhd.NgayLapHoaDon) as Nam,
    		qhd.ID_DonVi,
    		qct.TienThu as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
			INNER JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		INNER JOIN @tblNam nam ON nam.Nam = YEAR(qhd.NgayLapHoaDon)
    		where qhd.LoaiHoaDon = 11
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and (qct.DiemThanhToan is null OR qct.DiemThanhToan =0)
    		and qhd.HachToanKinhDoanh = 1
			and qct.HinhThucThanhToan NOT IN (4, 5)
			and qct.LoaiThanhToan != 1

			UNION ALL
			SELECT YEAR(qhd.NgayLapHoaDon) as Nam,
    		qhd.ID_DonVi,
    		-qct.TienThu  as ThanhTien
    		from Quy_HoaDon_ChiTiet qct
    		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
			LEFT JOIN BH_HoaDon hd ON hd.ID = qct.ID_HoaDonLienQuan
    		where qhd.LoaiHoaDon = 12 AND hd.LoaiHoaDon = 6
    		and (qhd.TrangThai is null or qhd.TrangThai != 0)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhd.HachToanKinhDoanh = 1
    	) a
    GROUP BY a.Nam, a.ID_DonVi
    	ORDER BY Nam
    END
    	SELECT tt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, tt.ThoiGian, tt.ThucThu FROM @tblThucThu tt
    	INNER JOIN DM_DonVi dv ON dv.ID = tt.ID_DonVi
END");

            DropStoredProcedure("[dbo].[SP_GetInforSoQuy_ByID]");
            DropStoredProcedure("[dbo].[SP_GetMaLienHe_Max]");
            DropStoredProcedure("[dbo].[SP_CheckMaDoiTuong_Exist]");
            DropStoredProcedure("[dbo].[SP_GetInforBasic_DoiTuongByID]");
            DropStoredProcedure("[dbo].[SP_GetListHoaDon_UseService]");
            DropStoredProcedure("[dbo].[SP_GetMaDoiTuong_Max]");
            DropStoredProcedure("[dbo].[SP_GetSoDuTheGiaTri_ofKhachHang]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountInvoice_Detail]");
            DropStoredProcedure("[dbo].[SP_ValueCard_ServiceUsed]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountInvoice]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountProduct_Detail]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountProduct_General]");
            DropStoredProcedure("[dbo].[SP_ReportValueCard_Balance]");
            DropStoredProcedure("[dbo].[SP_ReportValueCard_DiaryUsed]");
            DropStoredProcedure("[dbo].[SP_ValueCard_GetListHisUsed]");
        }
        
        public override void Down()
        {
			DropStoredProcedure("[dbo].[CheckMaDoiTuong_Exist]");
			DropStoredProcedure("[dbo].[GetDSGoiDichVu_ofKhachHang]");
			DropStoredProcedure("[dbo].[GetInforBasic_DoiTuongByID]");
			DropStoredProcedure("[dbo].[GetInforMuaHang_ofKhachHang]");
            DropStoredProcedure("[dbo].[GetInforSoQuy_ByID]");
            DropStoredProcedure("[dbo].[GetListHoaDon_UseService]");
            DropStoredProcedure("[dbo].[GetMaDoiTuong_Max]");
            DropStoredProcedure("[dbo].[GetMaLienHe_Max]");
            DropStoredProcedure("[dbo].[GetSoDuTheGiaTri_ofKhachHang]");
            DropStoredProcedure("[dbo].[ReportDiscountInvoice]");
			DropStoredProcedure("[dbo].[ReportDiscountInvoice_Detail]");
			DropStoredProcedure("[dbo].[ReportDiscountProduct_Detail]");
			DropStoredProcedure("[dbo].[ReportDiscountProduct_General]");
			DropStoredProcedure("[dbo].[ReportValueCard_Balance]");
			DropStoredProcedure("[dbo].[ReportValueCard_DiaryUsed]");
			DropStoredProcedure("[dbo].[ValueCard_GetListHisUsed]");
			DropStoredProcedure("[dbo].[ValueCard_ServiceUsed]");

        }
    }
}
