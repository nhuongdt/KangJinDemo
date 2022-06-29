namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20220504 : DbMigration
    {
        public override void Up()
        {
			Sql(@"CREATE FUNCTION [dbo].[GetSumSoGioHoatDongByIdXe]
(
	-- Add the parameters for the function here
	@IdXe UNIQUEIDENTIFIER,
	@ThoiGian DATETIME
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Result INT;

	-- Add the T-SQL statements to compute the return value here
	SELECT @Result = SUM(SoGioHoatDong) FROM Gara_Xe_NhatKyHoatDong nky
	INNER JOIN Gara_Xe_PhieuBanGiao pbg ON pbg.Id = nky.IdPhieuBanGiao
	WHERE pbg.IdXe = @IdXe AND nky.ThoiGianHoatDong >= @ThoiGian AND nky.TrangThai = 1

	-- Return the result of the function
	RETURN @Result

END");

			CreateStoredProcedure(name: "[dbo].[GetChiTietHD_MultipleHoaDon]", parametersAction: p => new
			{
				@lstID_HoaDon = p.String()
			}, body: @"SELECT 
    		cthd.ID,
			cthd.ID_HoaDon,
			cthd.ID_DonViQuiDoi,
			cthd.ID_LoHang,
			dvqd.ID_HangHoa,			
			cthd.DonGia,
			cthd.GiaVon,
			cthd.SoLuong,
			cthd.ThanhTien,
			cthd.ThanhToan,
    		cthd.TienChietKhau AS GiamGia,
			cthd.PTChietKhau,
			cthd.ThoiGian,
			cthd.GhiChu,
			iif(cthd.TenHangHoaThayThe is null or cthd.TenHangHoaThayThe ='', hh.TenHangHoa, cthd.TenHangHoaThayThe) as TenHangHoa,
			isnull(cthd.PTThue,0) as PTThue,
			isnull(cthd.TienThue,0) as TienThue,
			isnull(cthd.ThanhToan,0) as ThanhToan,
			isnull(cthd.DonGiaBaoHiem,0) as DonGiaBaoHiem,
    		(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
    		CAST(SoThuTu AS float) AS SoThuTu,
			cthd.ID_KhuyenMai,
			dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			hh.LaHangHoa,
			hh.QuanLyTheoLoHang,			
    		dvqd.TenDonViTinh,
			dvqd.MaHangHoa,
			dvqd.TyLeChuyenDoi,
			dvqd.ThuocTinhGiaTri, 				
			hh.ID_NhomHang as ID_NhomHangHoa,
			ISNULL(MaLoHang,'') as MaLoHang  ,
			lo.NgaySanXuat, 
			lo.NgayHetHan,
			hd.YeuCau,    
			ISNULL(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
			iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			concat(TenHangHoa ,    	
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when lo.MaLoHang is null then '' else '. Lô: ' + lo.MaLoHang end) as TenHangHoaFull   				    									
    	FROM BH_HoaDon hd
    	JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    	JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    	JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
		LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
		left JOIN DM_NhomHangHoa nhh ON hh.ID_NhomHang= nhh.ID    		
    	WHERE cthd.ID_HoaDon in (Select * from splitstring(@lstID_HoaDon))  
    		and (cthd.ID_ChiTietDinhLuong= cthd.ID OR cthd.ID_ChiTietDinhLuong is null)
			and (cthd.ID_ParentCombo= cthd.ID OR cthd.ID_ParentCombo is null)");

			CreateStoredProcedure(name: "[dbo].[GetChiTietHoaDon_afterTraHang]", parametersAction: p => new
			{
				@ID_HoaDon = p.String()
			}, body: @"set nocount on

	---- get cthdmua
	select ID		
	into #temCTMua
	from BH_HoaDon_ChiTiet ctm where ctm.ID_HoaDon= @ID_HoaDon

	---- get cttra or ctsudung
	select 
		ct.ID_ChiTietGoiDV,
		SUM(ct.SoLuong) as SoLuongTra
	into #tmpHDTra
	from BH_HoaDon_ChiTiet ct 
	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
	where hd.ChoThanhToan='0' and hd.LoaiHoaDon != 8 
	and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
	and exists (select ctm.ID from #temCTMua ctm where ct.ID_ChiTietGoiDV= ctm.ID)
	group by ct.ID_ChiTietGoiDV


	---- get soluong xuatkho of hdsc
		select SUM(ctxk.SoLuong) as SoLuongXuat, ctxk.ID_ChiTietGoiDV
		into #ctxk
		from BH_HoaDon_ChiTiet ctxk 
		join BH_HoaDon hdxk on ctxk.ID_HoaDon = hdxk.ID
		where hdxk.ID_HoaDon = @ID_HoaDon
		and hdxk.LoaiHoaDon = 8 and hdxk.ChoThanhToan='0'		
		group by ctxk.ID_ChiTietGoiDV			
			


select  distinct
		CAST(ctm.SoThuTu as float) as SoThuTu,
		ctm.ID, 
		ctm.ID_DonViQuiDoi,
		ctm.ID_LoHang,
		ctm.ID_TangKem, 
		ctm.TangKem, 
		ctm.ID_ParentCombo,
		ctm.ID_ChiTietDinhLuong,
		ctm.SoLuong,
		ISNULL(ctt.SoLuongTra,0) as SoLuongTra,
		iif(hd.LoaiHoaDon =25 and hh.LaHangHoa ='1', 
		isnull(xk.SoLuongXuat,0) - ISNULL(ctt.SoLuongTra,0) ,ctm.SoLuong - ISNULL(ctt.SoLuongTra,0)) as SoLuongConLai,
		ctm.DonGia, isnull(gv.GiaVon,0) as GiaVon, ctm.ThanhTien, ctm.ThanhToan, 
		ctm.TienChietKhau, 
		ctm.TienChietKhau as GiamGia,
		ctm.ThoiGian, ctm.GhiChu, ctm.PTChietKhau,
		ctm.ID_HoaDon, ctm.ID_ViTri,
		ctm.ID_LichBaoDuong,
		isnull(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
		CAST(ISNULL(ctm.TienThue,0) as float) as TienThue,CAST(ISNULL(ctm.PTThue,0) as float) as PTThue, 
		CAST(ISNULL(ctm.ThoiGianBaoHanh,0) as float) as ThoiGianBaoHanh,
		CAST(ISNULL(ctm.LoaiThoiGianBH,0) as float) as LoaiThoiGianBH,
		Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
			Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
		lo.NgaySanXuat, lo.NgayHetHan, isnull(lo.MaLoHang,'') as MaLoHang, isnull(tk.TonKho,0) as TonKho,
		hh.QuanLyTheoLoHang,
		hh.LaHangHoa,
		iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
		hd.MaHoaDon,
		hh.DichVuTheoGio,
		hh.DuocTichDiem,
		hh.SoPhutThucHien,
		qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
		qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,
		ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan,
		CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
		hh.LaHangHoa, 
		hh.TenHangHoa, 
		CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach, 
		hh.ID_NhomHang as ID_NhomHangHoa, 
		ISNULL(hh.GhiChu,'') as GhiChuHH,
		ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
		isnull(ctm.DonGiaBaoHiem,0) as DonGiaBaoHiem,
		iif(ctm.TenHangHoaThayThe is null or ctm.TenHangHoaThayThe ='', hh.TenHangHoa, ctm.TenHangHoaThayThe) as TenHangHoaThayThe		

	from BH_HoaDon_ChiTiet ctm
	join BH_HoaDon hd on ctm.ID_HoaDon= hd.ID
	join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID 
	join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
	LEFT JOIN DM_LoHang lo ON ctm.ID_LoHang = lo.ID
	left join DM_HangHoa_TonKho tk on (ctm.ID_DonViQuiDoi = tk.ID_DonViQuyDoi and (ctm.ID_LoHang = tk.ID_LoHang or ctm.ID_LoHang is null) and  tk.ID_DonVi = hd.ID_DonVi)
	left join DM_GiaVon gv on (tk.ID_DonViQuyDoi = gv.ID_DonViQuiDoi and (ctm.ID_LoHang = gv.ID_LoHang or ctm.ID_LoHang is null) and gv.ID_DonVi = hd.ID_DonVi) 
	left join  #tmpHDTra ctt  on ctm.ID = ctt.ID_ChiTietGoiDV --or ctm.ID_ParentCombo= ctt.ID_ParentCombo
	left join #ctxk xk on ctm.ID = xk.ID_ChiTietGoiDV
	where ctm.ID_HoaDon= @ID_HoaDon
	and (ctm.ID_ChiTietDinhLuong is null or ctm.ID_ChiTietDinhLuong = ctm.ID)
	and (ctm.ID_ParentCombo is null or ctm.ID_ParentCombo = ctm.ID)
	and (hh.LaHangHoa = 0 or (hh.LaHangHoa = 1 and tk.TonKho is not null))");

			CreateStoredProcedure(name: "[dbo].[GetListHDbyIDs]", parametersAction: p => new
			{
				@IDHoaDons = p.String()
			}, body: @"set nocount on;

	 declare @tblID table(ID uniqueidentifier)
	 insert into @tblID
	 select name from dbo.splitstring(@IDHoaDons)
	

	CREATE TABLE #TempIndex (	
		ID uniqueidentifier PRIMARY KEY, 
    	ID_DoiTuong uniqueidentifier,
    	ID_BaoHiem uniqueidentifier, 
    	ID_HoaDon uniqueidentifier, 
    	NgayLapHoaDon datetime,
    	ChoThanhToan bit
	)
		
	insert into #TempIndex WITH(TABLOCKX)
	select ID, ID_DoiTuong, ID_BaoHiem, ID_HoaDon,NgayLapHoaDon, ChoThanhToan
	from BH_HoaDon hd
	where exists (select ID from @tblID hd1 where hd.ID = hd1.ID)		;		
					

	with data_cte
	as(
	select c.*, iif(c.ChoThanhToan is null, 0,iif( c.ConNo1 - c.TongTienHDTra > 0, c.ConNo1 - c.TongTienHDTra,0)) as ConNo
	from
	(
	select 
			hd.ID,
    		hd.ID_DoiTuong,
    			
    			CASE 
    				WHEN dt.TheoDoi IS NULL THEN 
    					CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    				ELSE dt.TheoDoi
    			END AS TheoDoi,
    		hd.ID_HoaDon,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
			hd.ID_BaoHiem,
			hd.ID_PhieuTiepNhan,
    		hd.ChoThanhToan,
    		hd.ID_KhuyenMai,
    		hd.KhuyenMai_GhiChu,
    		hd.LoaiHoaDon,

			isnull(hd.TongThueKhachHang,0) as  TongThueKhachHang,
			isnull(hd.CongThucBaoHiem,0) as  CongThucBaoHiem,
			isnull(hd.GiamTruThanhToanBaoHiem,0) as  GiamTruThanhToanBaoHiem,
			isnull(hd.PTThueHoaDon,0) as  PTThueHoaDon,
			isnull(hd.TongTienThueBaoHiem,0) as  TongTienThueBaoHiem,
			isnull(hd.TongTienBHDuyet,0) as  TongTienBHDuyet,
			isnull(hd.SoVuBaoHiem,0) as  SoVuBaoHiem,
			isnull(hd.PTThueBaoHiem,0) as  PTThueBaoHiem,
			isnull(hd.KhauTruTheoVu,0) as  KhauTruTheoVu,
			isnull(hd.GiamTruBoiThuong,0) as  GiamTruBoiThuong,
			isnull(hd.PTGiamTruBoiThuong,0) as  PTGiamTruBoiThuong,
			isnull(hd.BHThanhToanTruocThue,0) as  BHThanhToanTruocThue,

    		ISNULL(hd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(hd.DiemGiaoDich,0) AS DiemGiaoDich,
			ISNULL(hd.ID_BangGia,N'00000000-0000-0000-0000-000000000000') as ID_BangGia,
			ISNULL(hd.ID_ViTri,N'00000000-0000-0000-0000-000000000000') as ID_ViTri,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		hd.MaHoaDon,
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
    		hd.NgayLapHoaDon,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
			dt.NgaySinh_NgayTLap,
			dt.MaSoThue,
			dt.TaiKhoanNganHang,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			isnull(bh.MaDoiTuong,'') as MaBaoHiem,
			isnull(bh.TenDoiTuong,'') as TenBaoHiem,
			isnull(bh.DienThoai,'') as BH_SDT,
			isnull(bh.DiaChi,'') as BH_DiaChi,
			isnull(bh.Email,'') as BH_Email,
			isnull(bh.TenDoiTuong_KhongDau,'') as TenBaoHiem_KhongDau,

			iif(hd.ID_BaoHiem is null,'', tn.NguoiLienHeBH) as LienHeBaoHiem,
			iif(hd.ID_BaoHiem is null,'', tn.SoDienThoaiLienHeBH) as SoDienThoaiLienHeBaoHiem,
			
			dt.ID_TinhThanh, 
			dt.ID_QuanHuyen,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
			ISNULL(nv.TenNhanVienKhongDau, N'') as TenNhanVienKhongDau,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		hd.DienGiai,
			
    		hd.NguoiTao as NguoiTaoHD,
			ISNULL(hd.TongChietKhau,0) as TongChietKhau,
			ISNULL(hd.TongTienHang,0) as TongTienHang,
			ISNULL(hd.ChiPhi,0) as TongChiPhi, --- chiphi cuahang phaitra
			ISNULL(hd.TongGiamGia,0) as TongGiamGia,
			ISNULL(hd.TongTienThue,0) as TongTienThue,
			ceiling(ISNULL(hd.PhaiThanhToan,0)) as PhaiThanhToan,
			ceiling(ISNULL(hd.TongThanhToan,0)) as TongThanhToan,
			ISNULL(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,
			iif(hdt.LoaiHoaDon=6,ISNULL(hdt.TongThanhToan,0),0) as TongTienHDTra, -- hdgoc: co the la baogia/hoactrahang
			ceiling(ISNULL(hd.TongThanhToan,0)) - ceiling(ISNULL(a.DaThanhToan,0)) as ConNo1,		
			ISNULL(a.ThuTuThe,0) as ThuTuThe,
			ISNULL(a.TienMat,0) as TienMat,
			ISNULL(a.TienATM,0) as TienATM,
			ISNULL(a.ChuyenKhoan,0) as ChuyenKhoan,
			ISNULL(a.TienDoiDiem,0) as TienDoiDiem,
			ISNULL(a.TienDatCoc,0) as TienDatCoc,
			ISNULL(a.GiaTriSDDV,0) as GiaTriSDDV,
			ISNULL(a.GiamGiaCT,0) as GiamGiaCT,
			ISNULL(a.ThanhTienChuaCK,0) as ThanhTienChuaCK,
			ISNULL(a.KhachDaTra,0) as KhachDaTra,
			ISNULL(a.DaThanhToan,0) as DaThanhToan,
			ISNULL(a.BaoHiemDaTra,0) as BaoHiemDaTra,

			cx.MaDoiTuong as MaChuXe,
			cx.TenDoiTuong as ChuXe,

			ceiling(hd.PhaiThanhToan - ISNULL(a.KhachDaTra,0)) as KhachConNo,
			ceiling(isnull(hd.PhaiThanhToanBaoHiem,0) - ISNULL(a.BaoHiemDaTra,0)) as BHConNo,

			hd.ID_Xe,
			isnull(tn.MaPhieuTiepNhan,'') as MaPhieuTiepNhan,
			isnull(xe.BienSo,'') as BienSo,
    		ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
    		Case When hd.ChoThanhToan = '1' then N'Phiếu tạm' when hd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai,
			case  hd.ChoThanhToan
				when 1 then '1'
				when 0 then '0'
			else '4' end as TrangThaiHD,
			iif(hd.ID_PhieuTiepNhan is null, '0','1') as LaHoaDonSuaChua,
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
									
						as PTThanhToan,
			ID_TaiKhoanPos,
			ID_TaiKhoanChuyenKhoan
	from BH_HoaDon hd
	left join
	(
	
	Select 
    			b.ID,    			
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
				SUM(ISNULL(b.TienATM, 0)) as TienATM,
    			SUM(ISNULL(b.TienCK, 0)) as ChuyenKhoan,
				SUM(ISNULL(b.TienDoiDiem, 0)) as TienDoiDiem,
				SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienThu, 0)) as DaThanhToan, --- = khach + baohiem tra
				SUM(ISNULL(b.KhachDaTra, 0)) as KhachDaTra,
				SUM(ISNULL(b.BaoHiemDaTra, 0)) as BaoHiemDaTra,
				SUM(ISNULL(b.GiaTriSDDV, 0)) as GiaTriSDDV,
				SUM(ISNULL(b.GiamGiaCT, 0)) as GiamGiaCT,
				SUM(ISNULL(b.ThanhTien, 0)) as ThanhTienChuaCK,
				max(b.ID_TaiKhoanPOS) as ID_TaiKhoanPos,
				max(b.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan,
				SUM(ISNULL(b.TienDatCoc, 0)) as TienDatCoc
    			from
    			(
					---- quyct of thishoadon
					select 
						soquyHD.ID_HoaDonLienQuan as ID,
						sum(soquyHD.TienMat) as TienMat,
						sum(soquyHD.TienATM) as TienATM,
						sum(soquyHD.TienCK) as TienCK,
						sum(soquyHD.TienDoiDiem) as TienDoiDiem,
						sum(soquyHD.ThuTuThe) as ThuTuThe,					
						sum(soquyHD.TienThu) as TienThu,
						0 as GiaTriSDDV,
						0 as GiamGiaCT,
						0 as ThanhTien,					
						max(soquyHD.ID_TaiKhoanPos) as ID_TaiKhoanPos,
						max(soquyHD.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan,
						sum(soquyHD.KhachDaTra) as KhachDaTra,
						sum(soquyHD.BaoHiemDaTra) as BaoHiemDaTra,
						sum(soquyHD.TienDatCoc) as TienDatCoc					
				from
				(
						select 
							soquy.ID_HoaDonLienQuan,
							soquy.ID_DoiTuong,					
							sum(isnull(soquy.TienMat,0)) as TienMat,
							sum(isnull(soquy.TienATM,0)) as TienATM,
							sum(isnull(soquy.TienCK,0)) as TienCK,
							sum(isnull(soquy.TienDoiDiem,0)) as TienDoiDiem,
							sum(isnull(soquy.ThuTuThe,0)) as ThuTuThe,
							sum(isnull(soquy.TienDatCoc,0)) as TienDatCoc,
							sum(isnull(soquy.TienThu,0)) as TienThu,
							max(soquy.ID_TaiKhoanPos) as ID_TaiKhoanPos,
							max(soquy.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan,
							iif(soquy.LoaiDoiTuong =1, sum(isnull(soquy.TienThu,0)),0) as KhachDaTra,
							iif(soquy.LoaiDoiTuong =3, sum(isnull(soquy.TienThu,0)),0) as BaoHiemDaTra
						from
						(
								select
									qct.ID_HoaDonLienQuan,
									qct.ID_DoiTuong,
									qct.ID_HoaDon,
									dt.LoaiDoiTuong,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=1, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=1, -qct.TienThu,0) end as TienMat,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=2, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=2, -qct.TienThu,0) end as TienATM,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=3, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=3, -qct.TienThu,0) end as TienCK,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=5, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=5, -qct.TienThu,0) end as TienDoiDiem,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=4, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=4, -qct.TienThu,0) end as ThuTuThe,
									case  when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=6, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=6, -qct.TienThu,0) end as TienDatCoc,
									iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as TienThu,
									iif(TaiKhoanPOS = 1 , qct.ID_TaiKhoanNganHang,  '00000000-0000-0000-0000-000000000000' ) as ID_TaiKhoanPos,
									iif(TaiKhoanPOS = 0 , qct.ID_TaiKhoanNganHang,  '00000000-0000-0000-0000-000000000000' ) as ID_TaiKhoanChuyenKhoan						
								from Quy_HoaDon_ChiTiet qct
								join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
								left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID
								left join DM_TaiKhoanNganHang tk on tk.ID= qct.ID_TaiKhoanNganHang 
								where exists (select ID from @tblID hd where hd.ID = qct.ID_HoaDonLienQuan)	
						) soquy				
						group by soquy.ID_HoaDonLienQuan, soquy.ID_DoiTuong,soquy.LoaiDoiTuong
				) soquyHD 					
				group by soquyHD.ID_HoaDonLienQuan

			Union all
						---- get TongThu from HDDatHang: chi get hdXuly first
    					select 
							ID,
							TienMat, TienATM,ChuyenKhoan,
							TienDoiDiem, ThuTuThe, TienThu,
							0 AS GiaTriSDDV,
							0 as GiamGiaCT,
							0 as ThanhTien,
							'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
							'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan,
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
											join #TempIndex hd on hd.ID_HoaDon= hdd.ID
											where hdd.LoaiHoaDon = '3' 	
											and hd.ChoThanhToan = 0
											and (qhd.TrangThai= 1 Or qhd.TrangThai is null)
    								) d group by d.ID,d.NgayLapHoaDon,ID_HoaDonLienQuan						
						) thuDH
						where isFirst= 1

					union all

					-- tong giatri sudung goiudv
					select 
						ctsd.ID_HoaDon as ID,
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,						
						0 as TienThu,
						ctsd.SoLuong * ct.DonGia  as GiaTriSDDV, -- gtri sudung: tinh theo DonGia (chua chiet khau _ Xuyen)
						0 as GiamGiaCT,
						0 as ThanhTien,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan,
						0 as KhachDaTra,
						0 as BaoHiemDaTra,
						0 as TienDatCoc
					from BH_HoaDon_ChiTiet ctsd
					join BH_HoaDon_ChiTiet ct on ctsd.ID_ChiTietGoiDV= ct.ID
					join BH_HoaDon gdv on ct.ID_HoaDon= gdv.ID
					where  gdv.LoaiHoaDon = 19									
					and 
					 (ctsd.ID_ChiTietDinhLuong= ctsd.ID or ctsd.ID_ChiTietDinhLuong is null)-- khong lay TPDinhLuong
					and exists 
					(				
						select ID from #TempIndex hd where ctsd.ID_HoaDon=  hd.ID
					)

					union all
					---- sum cthd
					select 
						ct.ID_HoaDon,
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,
						0 as TienThu,
						0 as GiaTriSDDV,
						ct.SoLuong * ct.TienChietKhau as GiamGiaCT,
						ct.SoLuong * ct.DonGia  as ThanhTien,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan,
						0 as KhachDaTra,
						0 as BaoHiemDaTra,
						0 as TienDatCoc
					from BH_HoaDon_ChiTiet ct
					where (ct.ID_ChiTietDinhLuong= ct.ID or ct.ID_ChiTietDinhLuong is null)
					and (ct.ID_ParentCombo= ct.ID or ct.ID_ParentCombo is null)
					and exists 
					(
						select ID from #TempIndex hd where ct.ID_HoaDon=  hd.ID
					)				
					
	) b group by b.ID
	) a on hd.ID= a.ID
	left join BH_HoaDon hdt on hd.ID_HoaDon = hdt.ID
    	left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		left join DM_DoiTuong bh on hd.ID_BaoHiem = bh.ID and bh.LoaiDoiTuong = 3
    	left join DM_DonVi dv on hd.ID_DonVi = dv.ID
    	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID 
    	left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    	left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    	left join DM_GiaBan gb on hd.ID_BangGia = gb.ID
    	left join DM_ViTri vt on hd.ID_ViTri = vt.ID    	
		left join Gara_PhieuTiepNhan tn on hd.ID_PhieuTiepNhan = tn.ID
		left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID	
		left join DM_DoiTuong cx on xe.ID_KhachHang= cx.ID	
		where exists (select ID from @tblID hd1 where hd.ID = hd1.ID)	
		
) c
		
),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(100 as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumKhachDaTra,
				sum(DaThanhToan) as SumDaThanhToan,
				sum(BaoHiemDaTra) as SumBaoHiemDaTra,
				sum(KhuyeMai_GiamGia) as SumKhuyeMai_GiamGia,
				sum(TongChiPhi) as SumTongChiPhi,
				sum(TongTienHDTra) as SumTongTongTienHDTra,
				sum(PhaiThanhToan) as SumPhaiThanhToan,
				sum(PhaiThanhToanBaoHiem) as SumPhaiThanhToanBaoHiem,
				sum(TongThanhToan) as SumTongThanhToan,
				sum(TienDoiDiem) as SumTienDoiDiem,
				sum(ThuTuThe) as SumThuTuThe,
				sum(TienDatCoc) as SumTienCoc,
				sum(ThanhTienChuaCK) as SumThanhTienChuaCK,
				sum(GiamGiaCT) as SumGiamGiaCT,
				sum(TienMat) as SumTienMat,
				sum(TienATM) as SumPOS,
				sum(ChuyenKhoan) as SumChuyenKhoan,
				sum(GiaTriSDDV) as TongGiaTriSDDV,
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo,

				sum(TongTienThueBaoHiem) as SumTongTienThueBaoHiem,
				sum(TongTienBHDuyet) as SumTongTienBHDuyet,
				sum(KhauTruTheoVu) as SumKhauTruTheoVu,
				sum(GiamTruBoiThuong) as SumGiamTruBoiThuong,
				sum(BHThanhToanTruocThue) as SumBHThanhToanTruocThue
				
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc							
		OFFSET 0 ROWS
		FETCH NEXT 100 ROWS ONLY; ");

			CreateStoredProcedure(name: "[dbo].[GetListNhatKyByIdPhieuBanGiao_v1]", parametersAction: p => new
			{
				@IdPhieuBanGiao = p.Guid(),
				@TrangThais = p.String(),
				@CurrentPage = p.Int(),
				@PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @tbTrangThai table (GiaTri varchar(2))
    insert into @tbTrangThai
    select Name from dbo.splitstring(@TrangThais);

    IF(@PageSize != 0)
	BEGIN
	with data_cte
	AS
	(SELECT nky.Id, nky.ThoiGianHoatDong, nky.SoGioHoatDong, nky.SoKmHienTai, nky.GhiChu, nky.LaNhanVien, nky.IdKhachHang, 
	dt.MaDoiTuong, dt.TenDoiTuong, nky.IdNhanVienThucHien, '' AS MaNhanVienThucHien, '' AS TenNhanVienThucHien, nky.TrangThai FROM Gara_Xe_NhatKyHoatDong nky
	INNER JOIN DM_DoiTuong dt ON dt.ID = nky.IdKhachHang
	WHERE nky.LaNhanVien = 0 AND nky.IdPhieuBanGiao = @IdPhieuBanGiao AND exists (select GiaTri from @tbTrangThai tt where nky.TrangThai = tt.GiaTri)
	UNION ALL
	SELECT nky.Id, nky.ThoiGianHoatDong, nky.SoGioHoatDong, nky.SoKmHienTai, nky.GhiChu, nky.LaNhanVien, nky.IdKhachHang, 
	'' AS MaDoiTuong, '' AS TenDoiTuong, nky.IdNhanVienThucHien, nv.MaNhanVien AS MaNhanVienThucHien, nv.TenNhanVien, nky.TrangThai AS TenNhanVienThucHien FROM Gara_Xe_NhatKyHoatDong nky
	INNER JOIN NS_NhanVien nv ON nv.ID = nky.IdNhanVienThucHien
	WHERE nky.LaNhanVien = 1 AND nky.IdPhieuBanGiao = @IdPhieuBanGiao AND exists (select GiaTri from @tbTrangThai tt where nky.TrangThai = tt.GiaTri)),
	count_cte AS
	(
		SELECT COUNT(Id) AS TotalRow, CEILING(COUNT(Id)/CAST(@PageSize AS float)) as TotalPage
		FROM data_cte
	)
	SELECT dt.*, ct.* FROM data_cte dt
	CROSS JOIN count_cte ct
	ORDER BY dt.ThoiGianHoatDong DESC
	OFFSET (@CurrentPage * @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY;
	END
	ELSE
	BEGIN
	with data_cte
	AS
	(SELECT nky.Id, nky.ThoiGianHoatDong, nky.SoGioHoatDong, nky.SoKmHienTai, nky.GhiChu, nky.LaNhanVien, nky.IdKhachHang, 
	dt.MaDoiTuong, dt.TenDoiTuong, nky.IdNhanVienThucHien, '' AS MaNhanVienThucHien, '' AS TenNhanVienThucHien, nky.TrangThai FROM Gara_Xe_NhatKyHoatDong nky
	INNER JOIN DM_DoiTuong dt ON dt.ID = nky.IdKhachHang
	WHERE nky.LaNhanVien = 0 AND nky.IdPhieuBanGiao = @IdPhieuBanGiao AND exists (select GiaTri from @tbTrangThai tt where nky.TrangThai = tt.GiaTri)
	UNION ALL
	SELECT nky.Id, nky.ThoiGianHoatDong, nky.SoGioHoatDong, nky.SoKmHienTai, nky.GhiChu, nky.LaNhanVien, nky.IdKhachHang, 
	'' AS MaDoiTuong, '' AS TenDoiTuong, nky.IdNhanVienThucHien, nv.MaNhanVien AS MaNhanVienThucHien, nv.TenNhanVien AS TenNhanVienThucHien, nky.TrangThai FROM Gara_Xe_NhatKyHoatDong nky
	INNER JOIN NS_NhanVien nv ON nv.ID = nky.IdNhanVienThucHien
	WHERE nky.LaNhanVien = 1 AND nky.IdPhieuBanGiao = @IdPhieuBanGiao AND exists (select GiaTri from @tbTrangThai tt where nky.TrangThai = tt.GiaTri))
	SELECT dt.*, 0 AS TotalRow, CAST(0 AS float) AS TotalPage FROM data_cte dt
	ORDER BY dt.ThoiGianHoatDong DESC
	END");

			CreateStoredProcedure(name: "[dbo].[GetListPhieuBanGiao_v1]", parametersAction: p => new
			{
				@NgayGiaoXeFrom = p.DateTime(),
				@NgayGiaoXeTo = p.DateTime(),
				@TrangThais = p.String(),
				@TextSearch = p.String(),
				@CurrentPage = p.Int(),
				@PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

    DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    declare @tbTrangThai table (GiaTri varchar(2))
    insert into @tbTrangThai
    select Name from dbo.splitstring(@TrangThais);
	IF(@PageSize != 0)
	BEGIN
	with data_cte
    	as
    	(
	select pbg.Id, pbg.MaPhieu, pbg.NgayGiaoXe, pbg.SoKmBanGiao, pbg.IdXe, dmx.BienSo, gmx.TenMauXe, ghx.TenHangXe, glx.TenLoaiXe, dmx.SoMay, dmx.SoKhung, 
	dmx.NamSanXuat, dmx.MauSon, pbg.IdKhachHang, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, pbg.LaNhanVien, pbg.IdNhanVien, '' AS MaNhanVien, '' AS TenNhanVien, '' AS DienThoaiNhanVien,
	pbg.IdNhanVienBanGiao ,nvgiao.MaNhanVien AS MaNhanVienGiao, nvgiao.TenNhanVien AS TenNhanVienGiao, 
	pbg.IdNhanVienTiepNhan, nvnhan.MaNhanVien AS MaNhanVienNhan,
	nvnhan.TenNhanVien AS TenNhanVienNhan, pbg.NgayNhanXe, pbg.GhiChuBanGiao, pbg.GhiChuTiepNhan, pbg.TrangThai from gara_xe_phieubangiao pbg
	INNER JOIN Gara_DanhMucXe dmx ON pbg.IdXe = dmx.ID
	LEFT JOIN Gara_MauXe gmx ON gmx.ID = dmx.ID_MauXe
	LEFT JOIN Gara_HangXe ghx ON ghx.ID = gmx.ID_HangXe
	LEFT JOIN Gara_LoaiXe glx ON glx.ID = gmx.ID_LoaiXe
	INNER JOIN DM_DoiTuong dt ON dt.ID = pbg.IdKhachHang
	INNER JOIN NS_NhanVien nvgiao ON pbg.IdNhanVienBanGiao = nvgiao.ID
	LEFT JOIN NS_NhanVien nvnhan ON nvnhan.ID = pbg.IdNhanVienTiepNhan
	WHERE pbg.LaNhanVien = 0 AND exists (select GiaTri from @tbTrangThai tt where pbg.TrangThai = tt.GiaTri)
	AND (@NgayGiaoXeFrom IS NULL OR pbg.NgayGiaoXe BETWEEN @NgayGiaoXeFrom AND @NgayGiaoXeTo)
	AND ((select count(Name) from @tblSearch b where     			
    		pbg.MaPhieu like '%'+b.Name+'%'
    		or dmx.BienSo like '%'+b.Name+'%'
    		or dt.MaDoiTuong like '%'+b.Name+'%'		
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    		or dt.DienThoai like '%'+b.Name+'%'
    		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    		or nvgiao.TenNhanVien like '%'+b.Name+'%'	
    		or nvgiao.MaNhanVien like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVien like '%'+b.Name+'%'	
    		or nvnhan.MaNhanVien like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or pbg.GhiChuBanGiao like '%'+b.Name+'%'	
    		or pbg.GhiChuTiepNhan like '%'+b.Name+'%'
    		)=@count or @count=0)
	UNION ALL
	select pbg.Id, pbg.MaPhieu, pbg.NgayGiaoXe, pbg.SoKmBanGiao, pbg.IdXe, dmx.BienSo, gmx.TenMauXe, ghx.TenHangXe, glx.TenLoaiXe, dmx.SoMay, dmx.SoKhung, 
	dmx.NamSanXuat, dmx.MauSon, pbg.IdKhachHang, '' AS MaDoiTuong, '' AS TenDoiTuong, '' AS DienThoai, pbg.LaNhanVien, pbg.IdNhanVien, nv.MaNhanVien AS MaNhanVien, nv.TenNhanVien AS TenNhanVien, nv.DienThoaiDiDong AS DienThoaiNhanVien,
	pbg.IdNhanVienBanGiao, nvgiao.MaNhanVien AS MaNhanVienGiao, nvgiao.TenNhanVien AS TenNhanVienGiao, 
	pbg.IdNhanVienTiepNhan, nvnhan.MaNhanVien AS MaNhanVienNhan,
	nvnhan.TenNhanVien AS TenNhanVienNhan, pbg.NgayNhanXe, pbg.GhiChuBanGiao, pbg.GhiChuTiepNhan, pbg.TrangThai from gara_xe_phieubangiao pbg
	INNER JOIN Gara_DanhMucXe dmx ON pbg.IdXe = dmx.ID
	LEFT JOIN Gara_MauXe gmx ON gmx.ID = dmx.ID_MauXe
	LEFT JOIN Gara_HangXe ghx ON ghx.ID = gmx.ID_HangXe
	LEFT JOIN Gara_LoaiXe glx ON glx.ID = gmx.ID_LoaiXe
	INNER JOIN NS_NhanVien nv ON nv.ID = pbg.IdNhanVien
	INNER JOIN NS_NhanVien nvgiao ON pbg.IdNhanVienBanGiao = nvgiao.ID
	LEFT JOIN NS_NhanVien nvnhan ON nvnhan.ID = pbg.IdNhanVienTiepNhan
	WHERE pbg.LaNhanVien = 1 AND exists (select GiaTri from @tbTrangThai tt where pbg.TrangThai = tt.GiaTri)
	AND (@NgayGiaoXeFrom IS NULL OR pbg.NgayGiaoXe BETWEEN @NgayGiaoXeFrom AND @NgayGiaoXeTo)
	AND ((select count(Name) from @tblSearch b where     			
    		pbg.MaPhieu like '%'+b.Name+'%'
    		or dmx.BienSo like '%'+b.Name+'%'
    		or nv.MaNhanVien like '%'+b.Name+'%'		
    		or nv.TenNhanVien like '%'+b.Name+'%'
    		or nv.DienThoaiDiDong like '%'+b.Name+'%'
    		or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    		or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    		or nvgiao.TenNhanVien like '%'+b.Name+'%'	
    		or nvgiao.MaNhanVien like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVien like '%'+b.Name+'%'	
    		or nvnhan.MaNhanVien like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or pbg.GhiChuBanGiao like '%'+b.Name+'%'	
    		or pbg.GhiChuTiepNhan like '%'+b.Name+'%'
    		)=@count or @count=0)
			),
    		count_cte
    		as
    		(
    			select count(Id) as TotalRow,
    				CEILING(COUNT(Id) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    		SELECT dt.*, ct.* FROM data_cte dt
    		CROSS JOIN count_cte ct
    		ORDER BY dt.NgayGiaoXe desc
    		OFFSET (@CurrentPage * @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY;
	END
	ELSE
	BEGIN
	with data_cte
    	as
    	(
	select pbg.Id, pbg.MaPhieu, pbg.NgayGiaoXe, pbg.SoKmBanGiao, pbg.IdXe, dmx.BienSo, gmx.TenMauXe, ghx.TenHangXe, glx.TenLoaiXe, dmx.SoMay, dmx.SoKhung, 
	dmx.NamSanXuat, dmx.MauSon, pbg.IdKhachHang, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, pbg.LaNhanVien, pbg.IdNhanVien, '' AS MaNhanVien, '' AS TenNhanVien, '' AS DienThoaiNhanVien,
	pbg.IdNhanVienBanGiao ,nvgiao.MaNhanVien AS MaNhanVienGiao, nvgiao.TenNhanVien AS TenNhanVienGiao, 
	pbg.IdNhanVienTiepNhan, nvnhan.MaNhanVien AS MaNhanVienNhan,
	nvnhan.TenNhanVien AS TenNhanVienNhan, pbg.NgayNhanXe, pbg.GhiChuBanGiao, pbg.GhiChuTiepNhan, pbg.TrangThai from gara_xe_phieubangiao pbg
	INNER JOIN Gara_DanhMucXe dmx ON pbg.IdXe = dmx.ID
	LEFT JOIN Gara_MauXe gmx ON gmx.ID = dmx.ID_MauXe
	LEFT JOIN Gara_HangXe ghx ON ghx.ID = gmx.ID_HangXe
	LEFT JOIN Gara_LoaiXe glx ON glx.ID = gmx.ID_LoaiXe
	INNER JOIN DM_DoiTuong dt ON dt.ID = pbg.IdKhachHang
	INNER JOIN NS_NhanVien nvgiao ON pbg.IdNhanVienBanGiao = nvgiao.ID
	LEFT JOIN NS_NhanVien nvnhan ON nvnhan.ID = pbg.IdNhanVienTiepNhan
	WHERE pbg.LaNhanVien = 0 AND exists (select GiaTri from @tbTrangThai tt where pbg.TrangThai = tt.GiaTri)
	AND (@NgayGiaoXeFrom IS NULL OR pbg.NgayGiaoXe BETWEEN @NgayGiaoXeFrom AND @NgayGiaoXeTo)
	AND ((select count(Name) from @tblSearch b where     			
    		pbg.MaPhieu like '%'+b.Name+'%'
    		or dmx.BienSo like '%'+b.Name+'%'
    		or dt.MaDoiTuong like '%'+b.Name+'%'		
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    		or dt.DienThoai like '%'+b.Name+'%'
    		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    		or nvgiao.TenNhanVien like '%'+b.Name+'%'	
    		or nvgiao.MaNhanVien like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVien like '%'+b.Name+'%'	
    		or nvnhan.MaNhanVien like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or pbg.GhiChuBanGiao like '%'+b.Name+'%'	
    		or pbg.GhiChuTiepNhan like '%'+b.Name+'%'
    		)=@count or @count=0)
	UNION ALL
	select pbg.Id, pbg.MaPhieu, pbg.NgayGiaoXe, pbg.SoKmBanGiao, pbg.IdXe, dmx.BienSo, gmx.TenMauXe, ghx.TenHangXe, glx.TenLoaiXe, dmx.SoMay, dmx.SoKhung, 
	dmx.NamSanXuat, dmx.MauSon, pbg.IdKhachHang, '' AS MaDoiTuong, '' AS TenDoiTuong, '' AS DienThoai, pbg.LaNhanVien, pbg.IdNhanVien, nv.MaNhanVien AS MaNhanVien, nv.TenNhanVien AS TenNhanVien, nv.DienThoaiDiDong AS DienThoaiNhanVien,
	pbg.IdNhanVienBanGiao, nvgiao.MaNhanVien AS MaNhanVienGiao, nvgiao.TenNhanVien AS TenNhanVienGiao, 
	pbg.IdNhanVienTiepNhan, nvnhan.MaNhanVien AS MaNhanVienNhan,
	nvnhan.TenNhanVien AS TenNhanVienNhan, pbg.NgayNhanXe, pbg.GhiChuBanGiao, pbg.GhiChuTiepNhan, pbg.TrangThai from gara_xe_phieubangiao pbg
	INNER JOIN Gara_DanhMucXe dmx ON pbg.IdXe = dmx.ID
	LEFT JOIN Gara_MauXe gmx ON gmx.ID = dmx.ID_MauXe
	LEFT JOIN Gara_HangXe ghx ON ghx.ID = gmx.ID_HangXe
	LEFT JOIN Gara_LoaiXe glx ON glx.ID = gmx.ID_LoaiXe
	INNER JOIN NS_NhanVien nv ON nv.ID = pbg.IdNhanVien
	INNER JOIN NS_NhanVien nvgiao ON pbg.IdNhanVienBanGiao = nvgiao.ID
	LEFT JOIN NS_NhanVien nvnhan ON nvnhan.ID = pbg.IdNhanVienTiepNhan
	WHERE pbg.LaNhanVien = 1 AND exists (select GiaTri from @tbTrangThai tt where pbg.TrangThai = tt.GiaTri)
	AND (@NgayGiaoXeFrom IS NULL OR pbg.NgayGiaoXe BETWEEN @NgayGiaoXeFrom AND @NgayGiaoXeTo)
	AND ((select count(Name) from @tblSearch b where     			
    		pbg.MaPhieu like '%'+b.Name+'%'
    		or dmx.BienSo like '%'+b.Name+'%'
    		or nv.MaNhanVien like '%'+b.Name+'%'		
    		or nv.TenNhanVien like '%'+b.Name+'%'
    		or nv.DienThoaiDiDong like '%'+b.Name+'%'
    		or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    		or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    		or nvgiao.TenNhanVien like '%'+b.Name+'%'	
    		or nvgiao.MaNhanVien like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or nvgiao.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVien like '%'+b.Name+'%'	
    		or nvnhan.MaNhanVien like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
    		or nvnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
    		or pbg.GhiChuBanGiao like '%'+b.Name+'%'	
    		or pbg.GhiChuTiepNhan like '%'+b.Name+'%'
    		)=@count or @count=0)
			)
    		SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
    			ORDER BY dt.NgayGiaoXe desc

	END");

			CreateStoredProcedure(name: "[dbo].[GetListPhuTungTheoDoiByIdXe_v1]", parametersAction: p => new
			{
				@IdXe = p.Guid()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	-- Lấy danh sách phụ tùng đang theo dõi
	DECLARE @tblPhuTungTheoDoi TABLE(Id UNIQUEIDENTIFIER, IdHoaDon UNIQUEIDENTIFIER, IdDonViQuiDoi UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, LoaiHoaDon INT, IdXe UNIQUEIDENTIFIER, 
	IdHangHoa UNIQUEIDENTIFIER, MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), DinhMucBaoDuong FLOAT, ThoiGianHoatDong FLOAT, ThoiGianConLai FLOAT);
	INSERT INTO @tblPhuTungTheoDoi
	select ct.ID, hd.ID, dvqd.ID, hd.NgayLapHoaDon, hd.LoaiHoaDon, hd.ID_Xe, hh.ID AS IdHangHoa, dvqd.MaHangHoa, hh.TenHangHoa, 0, 0, 0 from BH_HoaDon hd
	INNER JOIN BH_HoaDon_ChiTiet ct ON hd.ID = ct.ID_HoaDon
	INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = ct.ID_DonViQuiDoi
	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa AND dvqd.LaDonViChuan = 1
	WHERE hd.ID_Xe = @IdXe AND hd.LoaiHoaDon = 29;

	-- Lấy thời gian của hóa đơn sửa chữa gần nhất
	DECLARE @tblHoaDonSuaChua TABLE(Id UNIQUEIDENTIFIER, IdHoaDon UNIQUEIDENTIFIER, IdXe UNIQUEIDENTIFIER, IdHangHoa UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME,
	MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX));
	INSERT INTO @tblHoaDonSuaChua
	SELECT MAX(ct.ID) AS Id, hd.ID, pttd.IdXe, pttd.IdHangHoa, MAX(hd.NgayLapHoaDon) AS NgayLapHoaDon, dvqd.MaHangHoa, hh.TenHangHoa FROM @tblPhuTungTheoDoi pttd
	INNER JOIN Gara_PhieuTiepNhan ptn ON ptn.ID_Xe = pttd.IdXe
	INNER JOIN BH_HoaDon hd	ON hd.ID_PhieuTiepNhan = ptn.ID
	INNER JOIN BH_HoaDon_ChiTiet ct ON hd.ID = ct.ID_HoaDon
	INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = ct.ID_DonViQuiDoi AND dvqd.ID_HangHoa = pttd.IdHangHoa
	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
	WHERE hh.LoaiHangHoa = 1
	GROUP BY pttd.IdXe, pttd.IdHangHoa, dvqd.MaHangHoa, hh.TenHangHoa, hd.ID;

	-- Cập nhật thời gian bắt đầu tính thời gian hoạt động dựa theo thời gian hóa đơn sửa chữa gần nhất
	UPDATE pttd
	SET pttd.NgayLapHoaDon = hdsc.NgayLapHoaDon, pttd.LoaiHoaDon = 25
	FROM @tblPhuTungTheoDoi pttd
	INNER JOIN @tblHoaDonSuaChua hdsc ON pttd.IdXe = hdsc.IdXe AND pttd.IdHangHoa = hdsc.IdHangHoa;

	INSERT INTO @tblPhuTungTheoDoi
	SELECT hdsc.Id, hdsc.IdHoaDon, dvqd.ID, hdsc.NgayLapHoaDon, 25, hdsc.IdXe, hdsc.IdHangHoa, hdsc.MaHangHoa, hdsc.TenHangHoa, 0, 0, 0 FROM @tblHoaDonSuaChua hdsc
	LEFT JOIN @tblPhuTungTheoDoi pttd ON hdsc.IdXe = pttd.IdXe AND hdsc.IdHangHoa = pttd.IdHangHoa
	INNER JOIN DonViQuiDoi dvqd ON dvqd.ID_HangHoa = hdsc.IdHangHoa AND dvqd.LaDonViChuan = 1
	WHERE pttd.Id IS NULL;


	-- Lấy định mức bảo dưỡng trong danh mục hàng hóa
	DECLARE @tblDinhMucBaoDuong TABLE (Id UNIQUEIDENTIFIER, DinhMucBaoDuong INT);
	INSERT INTO @tblDinhMucBaoDuong
	SELECT pttd.Id, 
	IIF(bdct.LoaiGiaTri = 1, bdct.GiaTri * 24, 
	IIF(bdct.LoaiGiaTri = 2, bdct.GiaTri * 24 * 30, 
	IIF(bdct.LoaiGiaTri = 3, bdct.GiaTri * 24 * 365, 0))) AS SoGioDinhMuc FROM @tblPhuTungTheoDoi pttd
	INNER JOIN DM_HangHoa_BaoDuongChiTiet bdct ON pttd.IdHangHoa = bdct.ID_HangHoa AND bdct.BaoDuongLapDinhKy = 1;

	-- Cập nhật định mức bảo dưỡng
	UPDATE pttd
	SET pttd.DinhMucBaoDuong = dmbd.DinhMucBaoDuong
	FROM @tblPhuTungTheoDoi pttd
	INNER JOIN @tblDinhMucBaoDuong dmbd ON pttd.Id = dmbd.Id;

	-- Cập nhật thời gian hoạt động và thời gian còn lại
	UPDATE @tblPhuTungTheoDoi
	SET ThoiGianHoatDong = ISNULL(dbo.GetSumSoGioHoatDongByIdXe(IdXe, NgayLapHoaDon), 0);
	UPDATE @tblPhuTungTheoDoi
	SET ThoiGianConLai = IIF(DinhMucBaoDuong = 0, 0, DinhMucBaoDuong - ThoiGianHoatDong);

	SELECT * FROM @tblPhuTungTheoDoi ORDER BY ThoiGianConLai;");

			DropStoredProcedure("[dbo].[SP_GetChiTietHD_MultipleHoaDon]");
			DropStoredProcedure("[dbo].[SP_GetChiTietHoaDon_afterTraHang]");
			DropStoredProcedure("[dbo].[SP_GetChiTietHoaDon_ByIDHoaDon]");

			Sql(@"IF  EXISTS (SELECT [name] FROM sys.objects 
            WHERE object_id = OBJECT_ID('DiscountSale_NVBanHang'))
BEGIN
   DROP FUNCTION [DiscountSale_NVBanHang];
END
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create FUNCTION [dbo].[DiscountSale_NVBanHang]
(
	@IDChiNhanhs varchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanVien varchar(40)
)
RETURNS 
@tblChietKhauNVBanHang TABLE 
(
	LoaiNhanVienApDung int,
	ID_NhanVien uniqueidentifier,
	DoanhThu float,
	ThucThu float,
	HoaHongDoanhThu float,
	HoaHongThucThu float,
	IDChiTietCK uniqueidentifier
)
AS
BEGIN
	declare @tblChiNhanh table (ID uniqueidentifier)
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@IDChiNhanhs) 

					---- get phieuthu from - to
				--	declare @tblPhieuThu table (ID_HoaDonLienQuan uniqueidentifier, MaHoaDon nvarchar(max),MaxNgay datetime, MinNgay datetime)		
		
					insert into @tblChietKhauNVBanHang

					select 	
						1 as LoaiNhanVienApDung,
						tblNVBan.ID_NhanVien,
						tblNVBan.DoanhThu,
						tblNVBan.ThucThu,
						case when tblNVBan.LaPhanTram =1 then
    							case when tblNVBan.TinhChietKhauTheo=2 then DoanhThu * GiaTriChietKhau / 100 else 0 end 
    							else case when tblNVBan.TinhChietKhauTheo=2 then GiaTriChietKhau else 0 end end as HoaHongDoanhThu,   
						case when tblNVBan.LaPhanTram =1 then
    							case when tblNVBan.TinhChietKhauTheo=1 then ThucThu * GiaTriChietKhau / 100 else 0 end 
    						else case when tblNVBan.TinhChietKhauTheo=1 then GiaTriChietKhau else 0 end end as HoaHongThucThu   ,
							tblNVBan.ID as IDChiTietCK
					from
					(

					select  b.* ,  
						ckct.GiaTriChietKhau, 
						ckct.LaPhanTram,
						ckct.ID,
						ROW_NUMBER() over (PARTITION  by b.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
					from
					(
					select 
							a.ID_NhanVien,
    						a.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    						sum(a.TongThanhToan) -sum(GiaTriTra)  as DoanhThu, 
    						sum(ThucThu) -sum(TienTraKhach) as ThucThu,						
							a.ID_ChietKhauDoanhThu 
								
						from
						(
					---- doanhthu + thucthu hoadon ban
					select 
									tbl.ID_NhanVien, 
    								tbl.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								sum(tbl.TongThanhToan) as TongThanhToan,
									sum(isnull(tbl.HeSo,1)* isnull(soquy.ThucThu,0)) as ThucThu,
									0 as GiaTriTra,
    								0 as TienTraKhach,
    								tbl.ID_ChietKhauDoanhThu
								from
								(
								select distinct
									hd.ID,
									case when 
										hd.NgayLapHoaDon between @FromDate and @ToDate
										then 
											case when hd.ID_DonVi in (select ID from @tblChiNhanh) then  isnull(nvth.HeSo,1) *     									 
												case when hd.TongThanhToan is null or hd.TongThanhToan= 0 then hd.PhaiThanhToan
												else hd.TongThanhToan - isnull(hd.TongTienThue,0) end else 0 end
										else 0 end as TongThanhToan, 
									ckdtnv.ID_NhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								nvth.HeSo, 									
    								ckdtnv.ID_ChietKhauDoanhThu									
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu			
    							join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
								join BH_HoaDon hd on nvth.ID_HoaDon = hd.ID ---and ckdt.ID_DonVi = hd.ID_DonVi 
								where ckdt.LoaiNhanVienApDung=1
								and ckdt.TrangThai= 1
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
								and nvth.ID_NhanVien like @IDNhanVien								
								and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))	
								and exists (select ID from @tblChiNhanh cn where ckdt.ID_DonVi = cn.ID)
								) tbl
								left join (
									select ID_HoaDonLienQuan,
										sum(ThucThu) as ThucThu
									from dbo.ReportDiscount_GetThucThu(@IDChiNhanhs, @FromDate, @ToDate)
									group by ID_HoaDonLienQuan
								) soquy on tbl.ID = soquy.ID_HoaDonLienQuan
								left join(
								select b.ID_HoaDonLienQuan,
										b.MaHoaDon, 
										max(b.MaxNgay) as MaxNgay,
										min(b.MinNgay) as MinNgay
								from
								(
									select 
										a.ID_HoaDonLienQuan,
										a.MaHoaDon,
										iif(a.NgayLapPhieuThu is null,a.NgayLapHoaDon, iif(a.NgayLapHoaDon > a.NgayLapPhieuThu,a.NgayLapHoaDon, a.NgayLapPhieuThu)) as MaxNgay,
										iif(a.NgayLapPhieuThu is null,a.NgayLapHoaDon, iif(a.NgayLapHoaDon < a.NgayLapPhieuThu,a.NgayLapHoaDon, a.NgayLapPhieuThu)) as MinNgay
									from
									(
										select hd.ID as ID_HoaDonLienQuan, 
											hd.MaHoaDon, qhd.MaHoaDon as MaPhieuThu, 
											hd.NgayLapHoaDon,
											qhd.NgayLapHoaDon as NgayLapPhieuThu
										from BH_HoaDon hd
										left join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
										left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
										where hd.ChoThanhToan='0'
										and hd.LoaiHoaDon in (1,19,22, 25)
										and (qhd.TrangThai= 1 or qhd.TrangThai is null)
										and qhd.NgayLapHoaDon between @FromDate and @ToDate
										and (exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)
											or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
											)							
									) a
								) b
								group by b.ID_HoaDonLienQuan, b.MaHoaDon
								) sq on tbl.ID= sq.ID_HoaDonLienQuan
								where    						
								(sq.MinNgay between @FromDate and @ToDate
									or sq.MaxNgay between @FromDate and @ToDate)
								group by tbl.ID_NhanVien, tbl.TinhChietKhauTheo,  tbl.ID_ChietKhauDoanhThu 


								union all

							--- trahang
								select  ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
									0 as TienThu,
    								hdt.PhaiThanhToan - isnull(hdt.TongTienThue,0) as GiaTriTra,
    								sum(ISNULL(qhdct.TienThu, 0)) as TienTraKhach,
    								ckdtnv.ID_ChietKhauDoanhThu
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
								join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
    							join BH_HoaDon hdt on nvth.ID_HoaDon = hdt.ID 
								and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							Join BH_HoaDon hd on hd.ID = hdt.ID_HoaDon
    							left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    							left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							 exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID) 
    							and ckdt.LoaiNhanVienApDung=1
    							and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
								and hdt.LoaiHoaDon= 6
    							and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
								and (qhd.TrangThai is null or qhd.TrangThai != 0)
								and nvth.ID_NhanVien like @IDNhanVien
								and ckdt.TrangThai= 1
								group by ckdt.TinhChietKhauTheo,  
								hdt.TongThanhToan,  hdt.PhaiThanhToan, hdt.TongTienThue, ckdtnv.ID_ChietKhauDoanhThu, hdt.ID, ckdtnv.ID_NhanVien
					) a
						group by a.ID_ChietKhauDoanhThu, a.ID_NhanVien,  a.TinhChietKhauTheo
					)b
					join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
					and ((b.DoanhThu >= ckct.DoanhThuTu and b.TinhChietKhauTheo = 2) 
								or (b.ThucThu >= ckct.DoanhThuTu and b.TinhChietKhauTheo = 1))
			) tblNVBan where Rn= 1			
	
	RETURN 
END
");

			Sql(@"ALTER FUNCTION [dbo].[Diary_GetInforOldInvoice]
(	
	@ID_HoaDon uniqueidentifier
)
RETURNS nvarchar(max)  
AS
begin 

	Declare @infor nvarchar(max) = (select CONCAT(noidung, REPLACE(REPLACE( noidungct,'&lt;','<'),'&gt;','>')) as abc
	from
		(SELECT 
			CONCAT(
				N'- Mã hóa đơn: ',hd.MaHoaDon, N' , Ngày lập hóa đơn: ', FORMAT(NgayLapHoaDon,'dd/MM/yyyy HH:mm:ss'), 
				 N' , Nhân viên: ', nv.TenNhanVien,' (', nv.MaNhanVien,'), ',
				case hd.LoaiHoaDon
				when 8 then N'<br />- Tổng giá trị xuất: '
				else N' <br /> Tổng tiền hàng: '
				end,
					FORMAT(hd.TongTienHang,'#,0'),
				case hd.LoaiHoaDon
				when 8 then ''
				else CONCAT( N', Tổng giảm giá: ',FORMAT(hd.TongGiamGia,'#,0'), N', Phải thanh toán: ',FORMAT(hd.PhaiThanhToan,'#,0'),
						N', Khách hàng: ', dt.TenDoiTuong) end,
				N' <br /> - Chi tiết hóa đơn gồm: ') as noidung,
				(select 
					concat(N' <br /> <a style= ""cursor: pointer"" onclick = ""loadHangHoabyMaHH(',qd.MaHangHoa,')"" > ', qd.MaHangHoa ,' </a> ',
					case when lh.MaLoHang is null then '' else N'(Số lô: ' + lh.MaLoHang + ') ' end,
					N' Số lượng: ', ct.SoLuong, N', Đơn giá: ', FORMAT(ct.DonGia, '#,0.0'),',',
					N' Chiết khấu: ', FORMAT(ct.TienChietKhau, '#,0'),',',
					N' Tiền thuế: ', FORMAT(ct.TienThue, '#,0'),',',
					N' Thành tiền:' , FORMAT(ct.ThanhTien, '#,0'))  AS[text()]
					from BH_HoaDon_ChiTiet ct
					join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
					left
					join DM_LoHang lh on ct.ID_LoHang = lh.ID
					where ct.ID_HoaDon = @ID_HoaDon
					and(ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
					and(ct.ChatLieu is null or ct.ChatLieu != '5')
					for xml path('')
				) noidungct
		from BH_HoaDon hd
		join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		where hd.ID = @ID_HoaDon
	) s)
	return @infor
end
");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TonKho]
    @ID_DonVi NVARCHAR(max),
    @ThoiGian [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@TonKho INT
AS
BEGIN
	
		SET NOCOUNT ON;
		DECLARE @XemGiaVon as nvarchar
		Set @XemGiaVon = (Select 
				Case when nd.LaAdmin = '1' then '1' else
				Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
				From HT_NguoiDung nd where nd.ID = @ID_NguoiDung)	 				

		DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);

		declare @tblNhomHang table(ID UNIQUEIDENTIFIER)
		insert into @tblNhomHang
		SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)

		DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER, MaDonVi nvarchar(max), TenDonVi nvarchar(max));
		INSERT INTO @tblChiNhanh 
		SELECT dv.id, dv.MaDonVi, dv.TenDonVi 
		FROM splitstring(@ID_DonVi) cn
		join DM_DonVi dv on cn.Name = dv.ID;			

		if FORMAT(@ThoiGian,'yyyyMMdd') = FORMAT(dateadd(DAY,1, GETDATE()),'yyyyMMdd')
		begin
			---- get giavon, tonkho hientai
			select tk.*, gv.GiaVon
			into #tblTonKhoVon
			from 
			(
				select tk.ID_DonVi, tk.ID_DonViQuyDoi, tk.ID_LoHang, tk.TonKho
				from DM_HangHoa_TonKho tk where exists (select ID from @tblChiNhanh cn where tk.ID_DonVi= cn.id)
				group by tk.ID_DonVi, tk.ID_DonViQuyDoi, tk.ID_LoHang,tk.TonKho
			) tk
			left join
			(
				select gv.ID_DonVi, gv.ID_DonViQuiDoi, gv.ID_LoHang,gv.GiaVon
				from DM_GiaVon gv where exists (select ID from @tblChiNhanh cn where gv.ID_DonVi= cn.id)
				group by gv.ID_DonVi, gv.ID_DonViQuiDoi, gv.ID_LoHang,gv.GiaVon
			) gv on tk.ID_DonVi= gv.ID_DonVi and tk.ID_DonViQuyDoi= gv.ID_DonViQuiDoi
			and (tk.ID_LoHang = gv.ID_LoHang or tk.ID_LoHang is null and gv.ID_LoHang is null)

			select 	
				dv.MaDonVi, dv.TenDonVi,
				qd.MaHangHoa,
				hh.TenHangHoa,
				hh.QuyCach,
				lo.ID as ID_LoHang,
				qd.ID as ID_DonViQuyDoi,
				concat(hh.TenHangHoa,ThuocTinhGiaTri) as TenHangHoaFull,
				isnull(ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
				isnull(qd.TenDonViTinh,'') as TenDonViTinh,
				isnull(lo.MaLoHang,'') as TenLoHang,
				isnull(tk.TonKho,0) as TonCuoiKy,
				isnull(tk.GiaVon,0) as GiaVon,
				isnull(tk.TonKho,0) * iif(hh.QuyCach=0 or hh.QuyCach is null,1, hh.QuyCach)  as TonQuyCach,
				iif(@XemGiaVon='0',0,isnull(tk.TonKho,0) * isnull(tk.GiaVon,0))  as GiaTriCuoiKy,			
				isnull(nhom.TenNhomHangHoa,N'Nhóm Hàng Hóa Mặc Định') TenNhomHang
			from DM_HangHoa hh 		
			join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
			left join DM_LoHang lo on hh.ID = lo.ID_HangHoa and hh.QuanLyTheoLoHang = 1    	
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID				
			cross join @tblChiNhanh  dv	
			left join #tblTonKhoVon tk on qd.ID = tk.ID_DonViQuyDoi
			and	((lo.ID= tk.ID_LoHang) or (tk.ID_LoHang is null and hh.QuanLyTheoLoHang = 0 )) and tk.ID_DonVi = dv.ID
			where hh.LaHangHoa= 1 AND hh.TheoDoi LIKE @TheoDoi
			and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
			and exists (SELECT ID FROM @tblNhomHang allnhh where nhom.ID = allnhh.ID)	
			AND ((select count(Name) from @tblSearchString b where 
    			hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    				or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhom.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'
    				or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
					or dv.MaDonVi like '%'+b.Name+'%'
					or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)			
				and IIF(@TonKho = 1, isnull(tk.TonKho,0), 1) > 0		
				and IIF(@TonKho = 2, isnull(tk.TonKho,0), 0) <= 0	
				and IIF(@TonKho = 3, isnull(tk.TonKho,0), -1) < 0	
				order by hh.TenHangHoa, lo.MaLoHang
		end
		else
			begin	
			declare @tkDauKy table (ID_DonVi uniqueidentifier,ID_HangHoa uniqueidentifier,	ID_LoHang uniqueidentifier null, TonKho float,GiaVon float)		
			insert into @tkDauKy
			exec dbo.GetAll_TonKhoDauKy @ID_DonVi, @ThoiGian
					
			select 	
				dv.MaDonVi, dv.TenDonVi,
				qd.MaHangHoa,
				hh.TenHangHoa,
				hh.QuyCach,
				lo.ID as ID_LoHang,
				qd.ID as ID_DonViQuyDoi,
				concat(hh.TenHangHoa,ThuocTinhGiaTri) as TenHangHoaFull,
				isnull(ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
				isnull(qd.TenDonViTinh,'') as TenDonViTinh,
				isnull(lo.MaLoHang,'') as TenLoHang,
				isnull(tkDauKy.TonKho,0) as TonCuoiKy,
				isnull(tkDauKy.GiaVon,0) as GiaVon,
				isnull(tkDauKy.TonKho,0) * iif(hh.QuyCach=0 or hh.QuyCach is null,1, hh.QuyCach)  as TonQuyCach,
				iif(@XemGiaVon='0',0,isnull(tkDauKy.TonKho,0) * isnull(tkDauKy.GiaVon,0))  as GiaTriCuoiKy,			
				isnull(nhom.TenNhomHangHoa,N'Nhóm Hàng Hóa Mặc Định') TenNhomHang
			from DM_HangHoa hh 		
			join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
			left join DM_LoHang lo on hh.ID = lo.ID_HangHoa and hh.QuanLyTheoLoHang = 1    	
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID	
			cross join @tblChiNhanh  dv		
			left join @tkDauKy tkDauKy 		
			on hh.ID = tkDauKy.ID_HangHoa and tkDauKy.ID_DonVi= dv.ID and ((lo.ID= tkDauKy.ID_LoHang) or (tkDauKy.ID_LoHang is null and hh.QuanLyTheoLoHang = 0 ))	
			where hh.LaHangHoa= 1
			AND hh.TheoDoi LIKE @TheoDoi			
			and exists (SELECT ID FROM @tblNhomHang allnhh where nhom.ID = allnhh.ID)		
			and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
				AND ((select count(Name) from @tblSearchString b where 
    			hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    				or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhom.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'
    				or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
					or dv.MaDonVi like '%'+b.Name+'%'
					or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)						
					and IIF(@TonKho = 1, isnull(tkDauKy.TonKho,0), 1) > 0		
					and IIF(@TonKho = 2, isnull(tkDauKy.TonKho,0), 0) <= 0	
					and IIF(@TonKho = 3, isnull(tkDauKy.TonKho,0), -1) < 0		
					order by hh.TenHangHoa, lo.MaLoHang
		end

END");

			Sql(@"ALTER PROCEDURE [dbo].[ChangePTN_updateCus]
    @ID_PhieuTiepNhan [uniqueidentifier],
    @ID_KhachHangOld [uniqueidentifier],
    @ID_BaoHiemOld [uniqueidentifier],
    @Types [nvarchar](20)
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblType table(Loai int)
    	insert into @tblType select name from dbo.splitstring(@Types)
    
    	---- get PTN new
    	declare @PTNNew_IDCusNew uniqueidentifier, @PTNNew_BaoHiem uniqueidentifier
    	select @PTNNew_IDCusNew = ID_KhachHang, @PTNNew_BaoHiem = ID_BaoHiem from Gara_PhieuTiepNhan where ID= @ID_PhieuTiepNhan
    
    	---- get list hoadon of PTN
    	select ID, ID_DoiTuong, ID_BaoHiem
    	into #tblHoaDon
    	from BH_HoaDon
    	where ID_PhieuTiepNhan = @ID_PhieuTiepNhan
    	and ChoThanhToan =0
    	and LoaiHoaDon in (3,25)
    
    	---- update cus
    	if (select count(*) from @tblType where Loai in ('1','3')) > 0
    	begin
    		update hd set ID_DoiTuong= @PTNNew_IDCusNew
    		from BH_HoaDon hd
    		join #tblHoaDon hdCheck on hd.ID= hdCheck.ID

			---- update phieuthu khachhang
				update qct set ID_DoiTuong= @PTNNew_IDCusNew
    			from Quy_HoaDon_ChiTiet qct
    			join #tblHoaDon hdCheck on qct.ID_HoaDonLienQuan= hdCheck.ID
    			where qct.ID_DoiTuong = hdCheck.ID_DoiTuong
    	end

    
    	---- update baohiem
    	if (select count(*) from @tblType where Loai in ('2','4')) > 0
    	begin
    		update hd set ID_BaoHiem= @PTNNew_BaoHiem
    		from BH_HoaDon hd
    		join #tblHoaDon hdCheck on hd.ID= hdCheck.ID
    	
			
				---- update phieuthu baohiem
    		update qct set ID_DoiTuong= @PTNNew_BaoHiem
    		from Quy_HoaDon_ChiTiet qct
    		join #tblHoaDon hdCheck on qct.ID_HoaDonLienQuan= hdCheck.ID
    		where qct.ID_DoiTuong = hdCheck.ID_BaoHiem
    	end

	
END");

			Sql(@"ALTER PROCEDURE [dbo].[Gara_JqAutoHangHoa]
    @ID_ChiNhanh [uniqueidentifier],
    @ID_BangGia [nvarchar](40),
    @TextSearch [nvarchar](200),
    @LaHangHoa [nvarchar](10),
    @QuanLyTheoLo [nvarchar](10),
    @ConTonKho int,
	@Form [int], ---- 1.nhaphang, 0.other
	@CurrentPage [int],
	@PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
	declare @dtNow datetime = DATEADD(SECOND, -1, FORMAT(getdate(),'yyyy-MM-dd'))

	declare @txtSeachUnsign nvarchar(max) = (select dbo.FUNC_ConvertStringToUnsign(@TextSearch));    
	set @TextSearch = CONCAT('%',@TextSearch,'%')
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	with data_cte
	as 
	(
	select tbView.*,
		ISNULL(anh.URLAnh,'') as SrcImage
	from
	(
		select --ROW_NUMBER() OVER(ORDER BY (SELECT 1)) AS RN,
    		ID_DonViQuiDoi,ID,ID_LoHang,ID_NhomHangHoa,LaHangHoa,
    		MaHangHoa,TenHangHoa, TenDonViTinh,TyLeChuyenDoi,MaLoHang, NgaySanXuat, NgayHetHan,
    		ThuocTinhGiaTri,LaDonViChuan,
			LoaiHangHoa,
			TonKho,
			GiaNhap,
			isnull(GiaBan2, GiaBan) as GiaBan,	
			iif(c.LaHangHoa= 1, c.GiaVon, dbo.GetGiaVonOfDichVu(@ID_ChiNhanh,c.ID_DonViQuiDoi)) as GiaVon,   
			isnull(TenNhomHangHoa,N'Nhóm mặc định') as TenNhomHangHoa,
			Case when LaHangHoa='1' then 0 else CAST(ISNULL(ChiPhiThucHien,0) as float) end as PhiDichVu,
    		Case when LaHangHoa='1' then '0' else ISNULL(ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,				
			isnull(QuanLyBaoDuong,0) as QuanLyBaoDuong,
			case when ISNULL(QuyCach,0) = 0 then TyLeChuyenDoi else QuyCach * TyLeChuyenDoi end as QuyCach,
			ISNULL(DonViTinhQuyCach,'0') as DonViTinhQuyCach,
    		ISNULL(QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
    		ISNULL(ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
    		ISNULL(LoaiBaoHanh,0) as LoaiBaoHanh,
    		ISNULL(SoPhutThucHien,0) as SoPhutThucHien, 
    		ISNULL(GhiChu,'') as GhiChuHH ,
    		ISNULL(DichVuTheoGio,0) as DichVuTheoGio, 
    		ISNULL(DuocTichDiem,0) as DuocTichDiem,    
			ISNULL(HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,   
    		CONCAT(MaHangHoa, ' ', lower(MaHangHoa),' ', TenHangHoa, ' ', TenHangHoa_KhongDau,' ',
    		MaLoHang, ' ', GiaBan, ' ', ThuocTinhGiaTri) as Name
    from(
	select a.*, b.GiaBan2
	from
	(
	select 
		tbl.*,
		isnull(tk.TonKho, 0) as TonKho,
    	isnull(gv.GiaVon, 0) as GiaVon
	from
	(
	select 
		
		hh.ID, 
		hh.TenHangHoa,
		hh.ID_NhomHang as ID_NhomHangHoa,
		hh.LaHangHoa,		
		hh.QuanLyBaoDuong,
		hh.QuanLyTheoLoHang,
		hh.TenHangHoa_KhongDau,
		hh.ChiPhiThucHien,
		hh.ChiPhiTinhTheoPT,
		hh.DonViTinhQuyCach,
		hh.QuyCach,
		hh.ThoiGianBaoHanh,
		hh.LoaiBaoHanh,
		hh.SoPhutThucHien,
		hh.GhiChu,
		hh.DichVuTheoGio,
		hh.DuocTichDiem,
		hh.HoaHongTruocChietKhau,
		nhom.TenNhomHangHoa,
		qd.ID as ID_DonViQuiDoi, 
		qd.MaHangHoa,
		qd.TenDonViTinh, 
		qd.ThuocTinhGiaTri, 
		qd.TyLeChuyenDoi, 
		qd.GiaBan, 
		qd.GiaNhap,
		qd.LaDonViChuan,
		lo.ID as ID_LoHang, 
		lo.MaLoHang,
		lo.NgaySanXuat,
		lo.NgayHetHan,
		iif(hh.LoaiHangHoa is null, iif(LaHangHoa = 1,1,2),hh.LoaiHangHoa) as LoaiHangHoa
	from DM_HangHoa hh
	join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa
	left join DM_NhomHangHoa nhom on hh.ID_NhomHang = nhom.ID
	left join DM_LoHang lo on hh.ID= lo.ID_HangHoa
	where  hh.TheoDoi = '1'
			and (qd.xoa ='0'  or qd.Xoa is null)   			
		and (@Form=1 or (lo.NgayHetHan is null or lo.NgayHetHan > @dtNow)) ---- nhaphang
	and (
		hh.TenHangHoa like @TextSearch
		or hh.TenHangHoa like @txtSeachUnsign
		or hh.TenHangHoa_KhongDau like @TextSearch
		or hh.TenHangHoa_KhongDau like @txtSeachUnsign
		or qd.MaHangHoa like @TextSearch
		or qd.MaHangHoa like @txtSeachUnsign
		or lo.MaLoHang like @TextSearch
		or lo.MaLoHang like @txtSeachUnsign
		or 
		(
		(select count(Name) from @tblSearchString b where     			
    					hh.TenHangHoa like '%'+b.Name+'%'
    					or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    					or qd.MaHangHoa like '%'+b.Name+'%'		
    					or lo.MaLoHang like '%'+b.Name+'%'		
						or nhom.TenNhomHangHoa like '%'+b.Name+'%'	
						or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'	
    					)=@count or @count=0
		)
	)	
    ) tbl
	left join DM_HangHoa_TonKho tk on tk.ID_DonViQuyDoi= tbl.ID_DonViQuiDoi and (tbl.ID_LoHang = tk.ID_LoHang or tbl.ID_LoHang is null) and tk.ID_DonVi= @ID_ChiNhanh
    left join DM_GiaVon gv on tbl.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tbl.ID_LoHang = gv.ID_LoHang or tbl.ID_LoHang is null) and gv.ID_DonVi= @ID_ChiNhanh
	where  (@Form = 1 or (tbl.QuanLyTheoLoHang='0' or (tbl.QuanLyTheoLoHang='1'  and tbl.MaLoHang!='')))
	and 
	(
	tbl.LaHangHoa like @LaHangHoa 
		or (case tbl.LoaiHangHoa 
			when 1 then 11
			when 2 then 12
			when 3 then 23 end) like @LaHangHoa)
	) a
	left join
    	(			
    		select ct.ID_DonViQuiDoi, ct.GiaBan as GiaBan2
    		from DM_GiaBan_ChiTiet ct where ct.ID_GiaBan = @ID_BangGia
    	) b on a.ID_DonViQuiDoi= b.ID_DonViQuiDoi
	 where a.LaHangHoa = 0 or a.TonKho > iif(@Contonkho='1', 0, -99999)
   ) c 	
   ) tbView
   left join DM_HangHoa_Anh anh on tbView.ID= anh.ID_HangHoa and anh.SoThuTu = 1
   )
	select dt.*
	from data_cte dt
	order by dt.NgayHetHan
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY; 

END");

			Sql(@"ALTER PROCEDURE [dbo].[GetChiTietCongThuCong]
    @IDChiNhanhs [nvarchar](max),
    @IDNhanViens [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime]
AS
BEGIN
    SET NOCOUNT ON;
	--set @ToDate = DATEADD(day,1,@ToDate)
    	if @IDNhanViens	= ''
    		select null,bs.ID_CaLamViec, ca.TongGioCong as TongGioCong1Ca, ca.TenCa, bs.ID_NhanVien,
    				bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon, bs.Thu
    			from NS_CongBoSung bs
    			join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
    			where NgayCham between @FromDate and @ToDate and bs.ID_DonVi = @IDChiNhanhs
    			and bs.TrangThai in (1,2)-- 1.taomoi, 2.tamluu bangluong
    	else
    	 select null, bs.ID_CaLamViec, ca.TongGioCong as TongGioCong1Ca, ca.TenCa, bs.ID_NhanVien,
    				bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon, bs.Thu
    			from NS_CongBoSung bs
    			join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
    			where NgayCham between @FromDate and @ToDate and bs.ID_DonVi = @IDChiNhanhs
    			and bs.TrangThai in (1,2) 
    			and exists (select Name from dbo.splitstring(@IDNhanViens) nv where bs.ID_NhanVien= nv.Name)
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListGaraDanhMucXe_v1]
    @IdHangXe [uniqueidentifier],
    @IdLoaiXe [uniqueidentifier],
    @IdMauXe [uniqueidentifier],
    @TrangThais [nvarchar](max),
    @TextSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    	declare @tbTrangThai table (GiaTri int)
    	insert into @tbTrangThai
    	select Name from dbo.splitstring(@TrangThais);
    -- Insert statements for procedure here
    	if(@PageSize != 0)
    	BEGIN
    	with data_cte
    	as
    	(
    	SELECT gx.ID, gx.BienSo, gx.DungTich, gx.GhiChu, gx.HopSo, gx.MauSon, gx.ID_KhachHang, gx.ID_MauXe, gx.NamSanXuat, gx.NgayTao, gx.SoKhung,
    	gx.SoMay, gx.TrangThai, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.NgaySinh_NgayTLap, dt.DiaChi, dtt.MaTinhThanh, dtt.TenTinhThanh,
    	dqh.MaQuanHuyen, dqh.TenQuanHuyen,
    	ghx.TenHangXe, glx.TenLoaiXe, gmx.TenMauXe, gx.NguoiTao , gmx.ID_HangXe, gmx.ID_LoaiXe,
		hh.ID as ID_HangHoa, ISNULL(gx.NguoiSoHuu, 0) AS NguoiSoHuu
		FROM Gara_DanhMucXe gx
    	INNER JOIN Gara_MauXe gmx ON gx.ID_MauXe = gmx.ID
    	INNER JOIN Gara_HangXe ghx ON gmx.ID_HangXe = ghx.ID
    	INNER JOIN Gara_LoaiXe glx ON gmx.ID_LoaiXe = glx.ID
    	INNER JOIN @tbTrangThai tt ON tt.GiaTri = gx.TrangThai
    	LEFT JOIN DM_DoiTuong dt ON dt.ID = gx.ID_KhachHang
    	LEFT JOIN DM_TinhThanh dtt ON dt.ID_TinhThanh = dtt.ID
    	LEFT JOIN DM_QuanHuyen dqh ON dqh.ID = dt.ID_QuanHuyen
		LEFT JOIN DM_HangHoa hh ON gx.ID = hh.ID_Xe
    	WHERE (@IdHangXe IS NULL OR ghx.ID = @IdHangXe)
    	AND (@IdLoaiXe IS NULL OR glx.ID = @IdLoaiXe)
    	AND (@IdMauXe IS NULL OR gmx.ID = @IdMauXe)
    	AND ((select count(Name) from @tblSearch b where     			
    		gx.BienSo like '%'+b.Name+'%'
    		or gx.NamSanXuat like '%'+b.Name+'%'
    		or gx.SoMay like '%'+b.Name+'%'
    		or gx.SoKhung like '%'+b.Name+'%'
    		or gx.MauSon like '%'+b.Name+'%'
    		or gx.DungTich like '%'+b.Name+'%'
    		or gx.HopSo like '%'+b.Name+'%'
    		or gx.GhiChu like '%'+b.Name+'%'
    		or dt.MaDoiTuong like '%'+b.Name+'%'		
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    		or dt.DienThoai like '%'+b.Name+'%'
    		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    		or gmx.TenMauXe like '%'+b.Name+'%'
    		or glx.TenLoaiXe like '%'+b.Name+'%'
    		or ghx.TenHangXe like '%'+b.Name+'%'
    		)=@count or @count=0)
    		),
    		count_cte
    		as
    		(
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
    			from data_cte
    		)
    
    		SELECT dt.*, ct.* FROM data_cte dt
    		CROSS JOIN count_cte ct
    		ORDER BY dt.NgayTao desc
    		OFFSET (@CurrentPage * @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY;
    		END
    		ELSE
    		BEGIN
    		with data_cte
    	as
    	(
    	SELECT gx.ID, gx.BienSo, gx.DungTich, gx.GhiChu, gx.HopSo, gx.MauSon, gx.ID_KhachHang, gx.ID_MauXe, gx.NamSanXuat, gx.NgayTao, gx.SoKhung,
    	gx.SoMay, gx.TrangThai, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.NgaySinh_NgayTLap, dt.DiaChi, dtt.MaTinhThanh, dtt.TenTinhThanh,
    	dqh.MaQuanHuyen, dqh.TenQuanHuyen,
    	ghx.TenHangXe, glx.TenLoaiXe, gmx.TenMauXe, gx.NguoiTao , gmx.ID_HangXe, gmx.ID_LoaiXe,
		hh.ID as ID_HangHoa, ISNULL(gx.NguoiSoHuu, 0) AS NguoiSoHuu
		FROM Gara_DanhMucXe gx
    	INNER JOIN Gara_MauXe gmx ON gx.ID_MauXe = gmx.ID
    	INNER JOIN Gara_HangXe ghx ON gmx.ID_HangXe = ghx.ID
    	INNER JOIN Gara_LoaiXe glx ON gmx.ID_LoaiXe = glx.ID
    	INNER JOIN @tbTrangThai tt ON tt.GiaTri = gx.TrangThai
    	LEFT JOIN DM_DoiTuong dt ON dt.ID = gx.ID_KhachHang
    	LEFT JOIN DM_TinhThanh dtt ON dt.ID_TinhThanh = dtt.ID
    	LEFT JOIN DM_QuanHuyen dqh ON dqh.ID = dt.ID_QuanHuyen
		LEFT JOIN DM_HangHoa hh ON gx.ID = hh.ID_Xe
    	WHERE (@IdHangXe IS NULL OR ghx.ID = @IdHangXe)
    	AND (@IdLoaiXe IS NULL OR glx.ID = @IdLoaiXe)
    	AND (@IdMauXe IS NULL OR gmx.ID = @IdMauXe)
    	AND ((select count(Name) from @tblSearch b where     			
    		gx.BienSo like '%'+b.Name+'%'
    		or gx.NamSanXuat like '%'+b.Name+'%'
    		or gx.SoMay like '%'+b.Name+'%'
    		or gx.SoKhung like '%'+b.Name+'%'
    		or gx.MauSon like '%'+b.Name+'%'
    		or gx.DungTich like '%'+b.Name+'%'
    		or gx.HopSo like '%'+b.Name+'%'
    		or gx.GhiChu like '%'+b.Name+'%'
    		or dt.MaDoiTuong like '%'+b.Name+'%'		
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    		or dt.DienThoai like '%'+b.Name+'%'
    		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    		or gmx.TenMauXe like '%'+b.Name+'%'
    		or glx.TenLoaiXe like '%'+b.Name+'%'
    		or ghx.TenHangXe like '%'+b.Name+'%'
    		)=@count or @count=0)
    		)
    		SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
    			ORDER BY dt.NgayTao desc
    		END
END");

			Sql(@"ALTER PROCEDURE [dbo].[JqAuto_SearchXe]
    @TextSearch nvarchar(max) =null,	
	@StatusTN nvarchar(max) = null, ---- 1.chi get xe chua co trong xuong(khi them moi phieu tiep nhan), null. search all
	@IDCustomer nvarchar(max) = null, -- get list xe by chuxe
	@LaHangHoa int = null,
	@NguoiSoHuu int = null--null - all, 0- xe của kh, 1- xe của gara
AS
BEGIN
    SET NOCOUNT ON;

	declare @sql nvarchar(max) ='', @where nvarchar(max) ='', @paramDefined nvarchar(max) =''

	set @where =' where 1= 1 and xe.TrangThai = 1'
	
	if isnull(@TextSearch,'')!=''
		begin
			set @where= CONCAT(@where, ' and BienSo like N''%'' +  @TextSearch_In + ''%''')
		end

	if isnull(@StatusTN,'')!=''
		begin
			set @where= CONCAT(@where, ' and not exists (select ID_Xe from Gara_PhieuTiepNhan tn where xe.ID = tn.ID_Xe 
						and tn.TrangThai in (1,2 ))')
		end

	if isnull(@IDCustomer,'')!=''
		begin
			set @where= CONCAT(@where, ' and ID_KhachHang =  @IDCustomer_In')
		end	
	if isnull(@LaHangHoa,2)!=2 --- 0.khong get xe thuoc DM_HangHoa, 1. chi get xe la HangHoa, 2.all
		begin
			if @LaHangHoa = 1
				set @where= CONCAT(@where, ' and hh.ID_Xe is not null')
			if @LaHangHoa = 0
				set @where= CONCAT(@where, ' and hh.ID_Xe is null') 
		end	
	--lọc sở hưu của gara hay khách hàng cho phần làm phiếu bàn giao xe
	if ISNULL(@NguoiSoHuu, 2) != 2
	begin
		if @NguoiSoHuu = 0
		begin
			set @where = CONCAT(@where, ' and (xe.NguoiSoHuu = 0 or xe.NguoiSoHuu is null)')
		end
		else if @NguoiSoHuu = 1
		begin
			set @where = CONCAT(@where, ' and xe.NguoiSoHuu = 1')
		end
	end

	set @sql= CONCAT(@sql, '  select top 30 xe.ID, xe.ID_MauXe, xe.ID_KhachHang, xe.BienSo, xe.SoKhung, xe.SoMay, xe.HopSo, xe.DungTich, xe.MauSon, xe.NamSanXuat
    	from Gara_DanhMucXe xe
		left join DM_HangHoa hh on xe.ID = hh.ID_Xe
		', @where)

		set @paramDefined = N' @TextSearch_In nvarchar(max), @IDCustomer_In nvarchar(max), @StatusTN_In nvarchar(max)'
    

--	print @sql
	

	exec sp_executesql @sql, @paramDefined,	
	@TextSearch_In = @TextSearch, 	
	@StatusTN_In = @StatusTN,
	@IDCustomer_In = @IDCustomer

   
END");

			Sql(@"ALTER PROCEDURE [dbo].[XuatKhoToanBo_FromHoaDonSC]
    @ID_HoaDon [uniqueidentifier] 
AS
BEGIN
    SET NOCOUNT ON;

	-- count cthd la hanghoa
		declare @tongGtriXuat float, @count int
		select @count = count(ct.ID) , @tongGtriXuat= sum(ct.GiaVon * SoLuong)
			from BH_HoaDon_ChiTiet ct 
			join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			where ct.ID_HoaDon= @ID_HoaDon
			and hh.LaHangHoa = 1
		
		IF @count > 0
		BEGIN
				---- INSERT HD XUATKHO ----
			
			 declare @ID_XuatKho uniqueidentifier = newID()	
			 declare @ngaylapHD datetime, @ID_DonVi uniqueidentifier,  @mahoadon nvarchar(max)
    		select @ngaylapHD = NgayLapHoaDon, @ID_DonVi = ID_DonVi from BH_HoaDon where id= @ID_HoaDon
    		declare @ngayxuatkho datetime = dateadd(millisecond,2,@ngaylapHD)

				---- get mahoadon xuatkho
    			declare @tblMa table (MaHoaDon nvarchar(max))
    			insert into @tblMa
    			exec GetMaHoaDonMax_byTemp 8, @ID_DonVi, @ngaylapHD
    			select @mahoadon = MaHoaDon from @tblMa

				insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon,ID_PhieuTiepNhan, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
    			PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai)
    			select @ID_XuatKho, 8, @mahoadon,@ID_HoaDon,ID_PhieuTiepNhan, @ngayxuatkho, @ID_DonVi,ID_NhanVien, @tongGtriXuat,0,0,0,0,0, @tongGtriXuat,0,0,N'Hoàn thành', GETDATE(), NguoiTao, 
				concat(N'Xuất kho toàn bộ từ hóa đơn ', MaHoaDon)
    			from BH_HoaDon 
    			where id= @ID_HoaDon

    
			---- INSERT CT XUATKHO -----
		
		
			insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_ChiTietDinhLuong, ID_ChiTietGoiDV, 
						ID_DonViQuiDoi, ID_LoHang, SoLuong, DonGia, GiaVon, ThanhTien, ThanhToan, 
    					PTChietKhau, TienChietKhau, PTChiPhi, TienChiPhi, TienThue, An_Hien, TonLuyKe, GhiChu, TenHangHoaThayThe, ChatLieu)				
			select 
				NEWid(),
				@ID_XuatKho,
				row_number() over( order by (select 1)) as SoThuTu,
				ctsc.ID_ChiTietDinhLuong,
				ctsc.ID_ChiTietGoiDV,
				ctsc.ID_DonViQuiDoi,
				ctsc.ID_LoHang,
				ctsc.SoLuong, ctsc.GiaVon, ctsc.GiaVon, ctsc.GiaTri, 
				0,0,0,0,0,0,'1', ctsc.TonLuyKe, ctsc.GhiChu, ctsc.TenHangHoaThayThe, ctsc.ChatLieu
			from 
			(
			--- ct hoadon suachua
				select 
					cttp.ID as ID_ChiTietGoiDV,
					isnull(ctdv.ID_DichVu,cttp.ID_DonViQuiDoi) as ID_ChiTietDinhLuong, -- id_hanghoa/id_dichvu
					cttp.SoLuong,
					cttp.GiaVon,
					cttp.GiaVon* cttp.SoLuong as GiaTri,
					cttp.ID_DonViQuiDoi,
					cttp.ID_LoHang,
					cttp.TonLuyKe,
					isnull(cttp.GhiChu,'') as GhiChu,
					isnull(cttp.TenHangHoaThayThe,'') as TenHangHoaThayThe,
					cttp.ChatLieu -- chatlieu = 4. check sudung gdv
				from BH_HoaDon_ChiTiet cttp
				left join
				(
					select ctm.ID_DonViQuiDoi as ID_DichVu, ctm.ID, ctm.ID_LichBaoDuong
					from BH_HoaDon_ChiTiet ctm where ctm.ID_HoaDon= @ID_HoaDon
					and ctm.SoLuong > 0
				) ctdv on cttp.ID_ChiTietDinhLuong = ctdv.ID
				where cttp.ID_HoaDon= @ID_HoaDon
				and cttp.SoLuong > 0
				and cttp.ID_LichBaoDuong is null ---- khong xuat hang bao duong
				) ctsc
				JOIN DonViQuiDoi qd on ctsc.ID_DonViQuiDoi = qd.ID
				JOIN DM_HangHoa hh on qd.ID_HangHoa = hh.ID
				where hh.LaHangHoa ='1'


			select @ID_XuatKho as ID_HoaDon,  @ngayxuatkho as NgayLapHoaDon ---- get ngaylaphd of hdsc --> insert diary & update tonkho

			exec UpdateTonLuyKeCTHD_whenUpdate @ID_XuatKho,@ID_DonVi,@ngayxuatkho
		END
		
END");

			Sql(@"INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
VALUES (29, N'PTTD', N'Khởi tạo phụ tùng theo dõi', N'Khởi tạo phụ tùng theo dõi bảo dưỡng xe theo hoạt động', 'ssoftvn', GETDATE());
INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
VALUES (30, N'PBGX', N'Phiếu bàn giao xe', N'Phiếu bàn giao xe', 'ssoftvn', GETDATE());");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetChiTietHD_MultipleHoaDon]");
			DropStoredProcedure("[dbo].[GetChiTietHoaDon_afterTraHang]");
			DropStoredProcedure("[dbo].[GetListHDbyIDs]");
			DropStoredProcedure("[dbo].[GetListNhatKyByIdPhieuBanGiao_v1]");
			DropStoredProcedure("[dbo].[GetListPhieuBanGiao_v1]");
			DropStoredProcedure("[dbo].[GetListPhuTungTheoDoiByIdXe_v1]");
			Sql("DROP FUNCTION [dbo].[GetSumSoGioHoatDongByIdXe]");

        }
    }
}
