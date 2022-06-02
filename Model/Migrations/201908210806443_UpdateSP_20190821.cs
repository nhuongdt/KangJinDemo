namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190821 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_ChiTiet]
    @Text_Search [nvarchar](max),
    @MaHH [nvarchar](max),
	@MaKH [nvarchar](max),
	@MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
	@ID_NhomHang_SP [nvarchar](max),
	@LoaiChungTu [nvarchar](max),
	@HanBaoHanh [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	SET @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
		SELECT 
			a.MaKhachHang,
			a.TenKhachHang,
			a.NhomKhachHang,
			a.TenNguonKhach,
			a.DienThoai,
			a.GioiTinh,
			a.NguoiGioiThieu,
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
    		a.MaHangHoa,
    		a.TenHangHoaFull,
    		a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    		a.TenDonViTinh,
    		a.TenLoHang,
    		CAST(ROUND((a.SoLuong), 3) as float) as SoLuong, 
    		CAST(ROUND((a.GiaBan), 0) as float) as GiaBan,
    		CAST(ROUND((a.TienChietKhau), 0) as float) as TienChietKhau,
    		CAST(ROUND((a.ThanhTien), 0) as float) as ThanhTien,
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.GiaVon), 0) as float) else 0 end as GiaVon,
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.SoLuong * a.GiaVon), 0) as float) else 0 end as TienVon,
    		CAST(ROUND((a.GiamGiaHD), 0) as float) as GiamGiaHD,
    		Case When @XemGiaVon = '1' then CAST(ROUND((a.ThanhTien - (a.SoLuong * a.GiaVon) - a.GiamGiaHD), 0) as float) else 0 end as LaiLo, 
			a.ThoiGianBaoHanh,
			a.HanBaoHanh,
			a.TrangThai,
			Case when ThoiGianBaoHanh != '' and a.HanBaoHanh < GETDATE() then CAST(ROUND(DATEDIFF(day,GETDATE(),a.HanBaoHanh), 0) as float)
			when ThoiGianBaoHanh != '' and a.HanBaoHanh >= GETDATE() then CAST(ROUND(DATEDIFF(day,GETDATE(),DATEADD(DAY, 1, a.HanBaoHanh)), 0) as float)
			end as SoNgay,
			a.TenNhanVien,
			a.GhiChu
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    		dvqd.MaHangHoa,
			hh.TenHangHoa + dvqd.ThuocTinhGiaTri as TenHangHoaFull,
    		hh.TenHangHoa,
			dvqd.TenDonVitinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		lh.MaLoHang as TenLoHang,
			dt.MaDoiTuong as MaKhachHang,
			dt.TenDoiTuong as TenKhachHang,
			dt.TenNhomDoiTuongs as NhomKhachHang,
			nk.TenNguonKhach TenNguonKhach,
			dt.DienThoai,
			Case when dt.GioiTinhNam = 1 then N'Nam' else N'Nữ' end as GioiTinh,
			gt.TenDoiTuong as NguoiGioiThieu,
    		hdct.SoLuong,
    		hdct.DonGia as GiaBan,
    		hdct.TienChietKhau,
			ISNULL(hdct.GiaVon, 0) as GiaVon,
    		--nv.TenNhanVien,
			LEFT(b.nvth, NULLIF(LEN(b.nvth)-1,-1)) as TenNhanVien,
			CASE when hdct.LoaiThoiGianBH = 1 then CONVERT(varchar(100), hdct.ThoiGianBaoHanh) + N' ngày'
			when hdct.LoaiThoiGianBH = 2 then CONVERT(varchar(100), hdct.ThoiGianBaoHanh) + ' tháng'
			when hdct.LoaiThoiGianBH = 3 then CONVERT(varchar(100), hdct.ThoiGianBaoHanh) + ' năm'
			else '' end as ThoiGianBaoHanh,
			CASE when hdct.LoaiThoiGianBH = 1 then DATEADD(DAY, hdct.ThoiGianBaoHanh, hd.NgayLapHoaDon)
			when hdct.LoaiThoiGianBH = 2 then DATEADD(MONTH, hdct.ThoiGianBaoHanh, hd.NgayLapHoaDon)
			when hdct.LoaiThoiGianBH = 3 then DATEADD(YEAR, hdct.ThoiGianBaoHanh, hd.NgayLapHoaDon)
			end as HanBaoHanh,
			Case when hdct.LoaiThoiGianBH = 1 and DATEADD(DAY, hdct.ThoiGianBaoHanh, hd.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when hdct.LoaiThoiGianBH = 2 and DATEADD(MONTH, hdct.ThoiGianBaoHanh, hd.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when hdct.LoaiThoiGianBH = 3 and DATEADD(YEAR, hdct.ThoiGianBaoHanh, hd.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when hdct.LoaiThoiGianBH in (1,2,3) Then N'Còn hạn'
			else '' end as TrangThai,
			hdct.GhiChu,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((hd.TongGiamGia + hd.KhuyeMai_GiamGia) / hd.TongTienHang) end as GiamGiaHD    		
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
			left join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
			left join DM_DoiTuong gt on dt.ID_NguoiGioiThieu = gt.ID
			left join (Select distinct cthd.ID, 
							(
								Select nv.TenNhanVien + ',' AS [text()]
								from BH_NhanVienThucHien th 
								join NS_NhanVien nv on th.ID_NhanVien= nv.ID
								where th.ID_ChiTietHoaDon is not null and ThucHien_TuVan ='1'
								and th.ID_ChiTietHoaDon= cthd.ID
								For XML PATH ('')
							) nvth
						From dbo.BH_HoaDon_ChiTiet cthd) b on hdct.ID= b.ID	
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
			and hd.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    		and (hd.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or hdct.GhiChu like @Text_Search)
			and (hh.ID_NhomHang like @ID_NhomHang or hh.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (dt.TenDoiTuong like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.DienThoai like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV)
    		and dvqd.Xoa like @TrangThai
    	) a
    		where a.TrangThai like @HanBaoHanh
    	order by a.NgayLapHoaDon desc
END");
           
            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_ChiNhanh]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [nvarchar](max)
AS
BEGIN
	SELECT
	c.ID_DonVi as ID, c.TenDonVi,
	SUM (c.ThuTienMat -c.ChiTienMat) as TonTienMat,
    SUM (c.ThuTienGui - c.ChiTienGui) as TonTienGui,
	SUM (c.TongThuChi) as TongThuChi
	FROM
	(
    		 SELECT 
    			b.ID_DonVi,
				MAX (b.TenDonVi) as TenDonVi,
				MAX (b.ThuTienMat) as ThuTienMat,
    			MAX (b.ChiTienMat) as ChiTienMat,
    			MAX (b.ThuTienGui) as ThuTienGui,
    			MAX (b.ChiTienGui) as ChiTienGui, 
				MAX (b.ThuTienMat + b.ThuTienGui - b.ChiTienMat - b.ChiTienGui) as TongThuChi
    		FROM
    		(
				select 
				a.ID_DonVi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.MaHoaDon,
				a.TenDonVi,
				a.ID_NhomDoiTuong,
    			case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
    			Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' else '' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
				MAX (dv.ID) as ID_DonVi,
				MAX(dv.TenDonVi) as TenDonVi,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			Sum(ISNULL(qhdct.TienMat,0)) as TienMat,
    			Sum(ISNULL(qhdct.TienGui,0)) as TienGui,
    			qhd.NgayLapHoaDon,
    			hd.MaHoaDon
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
				inner join DM_DonVi dv on qhd.ID_DonVi = dv.ID
    		where qhd.NgayLapHoaDon < @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
				and qhd.HachToanKinhDoanh like @HachToanKD
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or qhd.MaHoaDon like @MaKH or qhd.MaHoaDon like @MaKH_TV)	
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong, dv.ID, dtn.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			Group by  b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi
				) as c
				GROUP BY c.ID_DonVi, c.TenDonVi
				ORDER BY c.TenDonVi
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_SoQuy]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [nvarchar](max),
    @LoaiTien [nvarchar](max)
AS
BEGIN
    --	tinh ton dau ky
    	Declare @TonDauKy float
    	Set @TonDauKy = (Select
    	CAST(ROUND(SUM(TienThu - TienChi), 0) as float) as TonDauKy
    	FROM
    	(
    		select 
    			case when qhd.LoaiHoaDon = 11 then qhdct.TienMat + qhdct.TienGui else 0 end as TienThu,
    			Case when qhd.LoaiHoaDon = 12 then qhdct.TienMat + qhdct.TienGui else 0 end as TienChi,
    			Case when qhdct.TienMat > 0 and qhdct.TienGui = 0 then '1' 
    			when qhdct.TienGui > 0 and qhdct.TienMat = 0 then '2'
    			when qhdct.TienGui > 0 and qhdct.TienMat > 0 then '12' else '' end as LoaiTien,
				qhd.HachToanKinhDoanh as HachToanKinhDoanh
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		where qhd.NgayLapHoaDon < @timeStart
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
			and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and qhdct.DiemThanhToan is null
    		) a 
    		where LoaiTien like @LoaiTien
			and HachToanKinhDoanh like @HachToanKD
    	) 
		
    	if (@TonDauKy is null)
    	BeGin
    		Set @TonDauKy = 0;
    	END
    	Declare @tmp table (MaPhieuThu nvarchar(max), NgayLapHoaDon datetime, KhoanMuc nvarchar(max), TenDoiTac nvarchar(max),
    	TienMat float, TienGui float, TienThu float, TienChi float, ThuTienMat float, ChiTienMat float, ThuTienGui float, ChiTienGui float, TonLuyKeTienMat float,TonLuyKeTienGui float,TonLuyKe float, SoTaiKhoan nvarchar(max), NganHang nvarchar(max), GhiChu nvarchar(max))
    	Insert INTO @tmp
    		 SELECT 
				MAX(b.MaPhieuThu) as MaPhieuThu,
    			MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
				MAX(b.NoiDungThuChi) as KhoanMuc,
    			MAX(b.TenNguoiNop) as TenDoiTac, 
    			MAX (b.TienMat) as TienMat,
    			MAX (b.TienGui) as TienGui,
    			MAX (b.TienThu) as TienThu,
    			MAX (b.TienChi) as TienChi,
    			MAX (b.ThuTienMat) as ThuTienMat,
    			MAX (b.ChiTienMat) as ChiTienMat, 
    			MAX (b.ThuTienGui) as ThuTienGui,
    			MAX (b.ChiTienGui) as ChiTienGui, 
				0 as TonLuyKe,
    			0 as TonLuyKeTienMat,
    			0 as TonLuyKeTienGui,
    			MAX(b.SoTaiKhoan) as SoTaiKhoan,
    			MAX(b.NganHang) as NganHang,
    			MAX(b.GhiChu) as GhiChu
    		FROM
    		(
				select 
    			a.HachToanKinhDoanh,
    			a.ID_NhomDoiTuong,
    			a.ID_DoiTuong,
    			a.ID_HoaDon,
    			a.MaHoaDon,
    			a.MaPhieuThu,
    			a.NgayLapHoaDon,
    			a.TenNguoiNop,
    			a.TienMat,
    			a.TienGui,
    			case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
    			Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat,
    			case when a.LoaiHoaDon = 11 then a.TienThu else 0 end as TienThu,
    			Case when a.LoaiHoaDon = 12 then a.TienThu else 0 end as TienChi,
    			a.NoiDungThuChi,
    			a.NganHang,
    			a.SoTaiKhoan,
    			a.GhiChu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' else '' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			MAX (tknh.SoTaiKhoan) as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    			Sum(qhdct.TienMat) as TienMat,
    			Sum(qhdct.TienGui) as TienGui,
    			Sum(qhdct.TienThu) as TienThu,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or qhd.MaHoaDon like @MaKH or qhd.MaHoaDon like @MaKH_TV)	
    			and qhd.HachToanKinhDoanh like @HachToanKD
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
			where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			and LoaiTien like @LoaiTien
    		Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon
    		ORDER BY NgayLapHoaDon
    -- tính tồn lũy kế
	    IF (EXISTS (select * from @tmp))
		BEGIN
    			DECLARE @Ton float;
    			SET @Ton = @TonDauKy;
    			DECLARE @TonTienMat float;
    			SET @TonTienMat = @TonDauKy;
    			DECLARE @TonTienGui float;
    			SET @TonTienGui = @TonDauKy;
    			DECLARE @MaPhieuThu nvarchar(max);
    			DECLARE @NgayLapHoaDon datetime;
    			DECLARE @KhoanMuc nvarchar(max);
    			DECLARE @TenDoiTac nvarchar(max);
    			DECLARE @TienMat float;
    			DECLARE @TenGui float;
    			DECLARE @TienThu float;
    			DECLARE @TienChi float;
    			DECLARE @ThuTienMat float;
    			DECLARE @ChiTienMat float;
    			DECLARE @ThuTienGui float;
    			DECLARE @ChiTienGui float;
    			DECLARE @TonLuyKe float;
    			DECLARE @TonLuyKeTienMat float;
    			DECLARE @TonLuyKeTienGui float;
    			DECLARE @SoTaiKhoan nvarchar(max);
    			DECLARE @NganHang nvarchar(max);
    			DECLARE @GhiChu nvarchar(max);
    	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT * FROM @tmp ORDER BY NgayLapHoaDon
    	OPEN CS_ItemUpDate 
    FETCH FIRST FROM CS_ItemUpDate INTO @MaPhieuThu,@NgayLapHoaDon,@KhoanMuc,@TenDoiTac, @TienMat,@TenGui, @TienThu,@TienChi,@ThuTienMat,@ChiTienMat,@ThuTienGui,@ChiTienGui, @TonLuyKe,@TonLuyKeTienMat, @TonLuyKeTienGui, @SoTaiKhoan,@NganHang,@GhiChu
    WHILE @@FETCH_STATUS = 0
    BEGIN
    	SET @Ton = @Ton + @TienThu - @TienChi;
    	SET @TonTienMat = @TonTienMat + @ThuTienMat - @ChiTienMat;
    	SET @TonTienGui = @TonTienGui + @ThuTienGui - @ChiTienGui;
    	UPDATE @tmp SET TonLuyKe = @Ton, TonLuyKeTienMat = @TonTienMat, TonLuyKeTienGui = @TonTienGui WHERE MaPhieuThu = @MaPhieuThu
    	FETCH NEXT FROM CS_ItemUpDate INTO @MaPhieuThu,@NgayLapHoaDon,@KhoanMuc,@TenDoiTac, @TienMat,@TenGui, @TienThu,@TienChi,@ThuTienMat,@ChiTienMat,@ThuTienGui,@ChiTienGui,@TonLuyKe,@TonLuyKeTienMat, @TonLuyKeTienGui, @SoTaiKhoan,@NganHang,@GhiChu
    END
    CLOSE CS_ItemUpDate
    DEALLOCATE CS_ItemUpDate
	END
	ELSE
	BEGIN
		Insert INTO @tmp
    	SELECT 'TRINH0001', '1989-04-07','','','0','0','0','0','0','0','0','0', @TonDauKy, @TonDauKy, @TonDauKy, '','',''
	END
    	Select 
    	MaPhieuThu,
    	NgayLapHoaDon,
    	KhoanMuc,
    	TenDoiTac,
    	@TonDauKy as TonDauKy,
    	TienMat,
    	TienGui,
    	TienThu,
    	TienChi,
    	ThuTienMat,
    	ChiTienMat,
    	ThuTienGui,
    	ChiTienGui,
    	TonLuyKe,
    	TonLuyKeTienMat,
    	TonLuyKeTienGui,
    	SoTaiKhoan, 
    	NganHang, 
    	GhiChu
    	 from @tmp order by NgayLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_DoiTuong_HinhThuc1]
    @SqlQuery [nvarchar](max)
AS
BEGIN
    declare @sql  [nvarchar](max);
    	declare @sql2  [nvarchar](max);
    	set @sql = 'SELECT b.ID_DoiTuong
    	FROM
    	(
    		SELECT
    		a.ID_DoiTuong,
    		CAST(ROUND(a.GiaTriBan , 0) as float ) as GiaTriBan,
    		CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
    		CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan
    		FROM
    		(
    			SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			SUM(Case when (hd.LoaiHoaDon = 1 OR hd.LoaiHoaDon = 19) then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6 or hd.LoaiHoaDon = 19)
    			and hd.ChoThanhToan = 0 and hd.TongTienHang > 0
    			and hd.ID_DoiTuong is not null
    			GROUP BY hd.ID_DoiTuong
    		) a
    	) b
    	where DoanhThuThuan'
    	set @sql2 = @sql + @SqlQuery;
    	exec (@sql2);
END");
            
            Sql(@"ALTER PROCEDURE [dbo].[insert_NhomDoiTuong]
    @LoaiDieuKien [int],
    @ID [uniqueidentifier],
    @LoaiDoiTuong [int],
    @MaNhomDoiTuong [nvarchar](max),
    @TenNhomDoiTuong [nvarchar](max),
    @TenNhomDoiTuong_KhongDau [nvarchar](max),
    @TenNhomDoiTuong_KyTuDau [nvarchar](max),
    @GhiChu [nvarchar](max),
    @GiamGia [float],
    @GiamGiaTheoPhanTram [bit],
    @NguoiTao [nvarchar](max),
    @TimeCreate [datetime],
    @TuDongCapNhat [bit]
AS
BEGIN
    IF (@LoaiDieuKien = 1)
    	BEGIN
    		if (@GiamGia >= 0)
    		BEGIN
    			INSERT INTO DM_NhomDoiTuong (ID, LoaiDoiTuong, MaNhomDoiTuong, TenNhomDoiTuong,TenNhomDoiTuong_KhongDau, TenNhomDoiTuong_KyTuDau, GhiChu, GiamGia, GiamGiaTheoPhanTram, NguoiTao, NgayTao, TuDongCapNhat, TrangThai)
    			VALUES (@ID, @LoaiDoiTuong, @MaNhomDoiTuong, @TenNhomDoiTuong,@TenNhomDoiTuong_KhongDau, @TenNhomDoiTuong_KyTuDau, @GhiChu, @GiamGia, @GiamGiaTheoPhanTram, @NguoiTao, @TimeCreate, @TuDongCapNhat, '1')
    		END
    		else
    		BEGIN
    			INSERT INTO DM_NhomDoiTuong (ID, LoaiDoiTuong, MaNhomDoiTuong, TenNhomDoiTuong,TenNhomDoiTuong_KhongDau, TenNhomDoiTuong_KyTuDau, GhiChu, GiamGia, GiamGiaTheoPhanTram, NguoiTao, NgayTao, TuDongCapNhat, TrangThai)
    			VALUES (@ID, @LoaiDoiTuong, @MaNhomDoiTuong, @TenNhomDoiTuong, @TenNhomDoiTuong_KhongDau, @TenNhomDoiTuong_KyTuDau, @GhiChu, null, @GiamGiaTheoPhanTram, @NguoiTao, @TimeCreate, @TuDongCapNhat,'1')
    		END
    	END
    IF (@LoaiDieuKien = 2)
    	BEGIN
			declare @IDSearch varchar(40) = convert(varchar(40),@ID)
			declare @tenNhomOld nvarchar(max) = (select TenNhomDoiTuong from DM_NhomDoiTuong where ID= @ID)

    		if (@GiamGia >= 0)
    		BEGIN
    			UPDATE DM_NhomDoiTuong set TenNhomDoiTuong = @TenNhomDoiTuong, GhiChu = @GhiChu, GiamGia = @GiamGia, GiamGiaTheoPhanTram = @GiamGiaTheoPhanTram, TuDongCapNhat = @TuDongCapNhat, NgaySua = GETDATE(), NguoiSua= @NguoiTao
    			Where ID = @ID
    		END
    		else
    		BEGIN
    			UPDATE DM_NhomDoiTuong set TenNhomDoiTuong = @TenNhomDoiTuong, GhiChu = @GhiChu, GiamGia = null, GiamGiaTheoPhanTram = @GiamGiaTheoPhanTram, TuDongCapNhat = @TuDongCapNhat, NgaySua = GETDATE(), NguoiSua= @NguoiTao
    			Where ID = @ID
    		END
			-- update again TenNhomDoiTuong in DM_DoiTuong
			update DM_DoiTuong set TenNhomDoiTuongs= Replace(TenNhomDoiTuongs,@tenNhomOld,@TenNhomDoiTuong) where IDNhomDoiTuongs like '%'+ @IDSearch +'%'
    	END
END
			");
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
END

--Search_DMHangHoa_TonKho 'goi','goi','d93b17ea-89b9-4ecf-b242-d03b8cde71de','28fef5a1-f0f2-4b94-a4ad-081b227f3b77'");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetChiTietHoaDonGoiDV_AfterUseAndTra]
    @ID_HoaDon [uniqueidentifier]
AS
BEGIN
			select ct1.ID, ct1.ID_HoaDon, 
					-- SoLuong: tách riêng Số lượng còn lại của Hàng KM | Hàng không KM
					case when ct1.ID_TangKem is null then max(ct1.SoLuong)- MAX(ISNULL(hdt.SLTraThuong,0)) -  SUM(ISNULL(ctsd.SoLuong,0))
						 when ct1.ID_TangKem is not null then max(ct1.SoLuong)- MAX(ISNULL(hdt.SLTraKMai,0)) - SUM(ISNULL(ctsd.SoLuong,0))
					 end as SoLuong,
					 max(ct1.SoLuong) as soluongmua,
					 MAX(ISNULL(hdt.SLTraThuong,0)) as soluongtrathuong, 
					 SUM(ISNULL(ctsd.SoLuong,0)) as soluongdung,
					ct1.ID_DonViQuiDoi, 
    				ct1.DonGia,ct1.GiaVon,ct1.ThanhTien,qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa, max(ISNULL(ct1.TienChietKhau,0)) as GiamGia, 
    				ct1.ThoiGian, ct1.GhiChu,ct1.ID_LoHang, CAST((ct1.SoThuTu) as float) as SoThuTu , ct1.TangKem,ct1.ID_TangKem,
    				hh.LaHangHoa, hh.TenHangHoa,
					ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, 
					CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
					CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
					MAX(ISNULL(ct1.PTThue, 0)) as PTThue,
					MAX(ISNULL(ct1.TienThue, 0)) as TienThue,
					MAX(CAST(ISNULL(ct1.ThoiGianBaoHanh,0) as float)) as ThoiGianBaoHanh,
					MAX(CAST(ISNULL(ct1.LoaiThoiGianBH,0) as float)) as LoaiThoiGianBH
					
    		from BH_HoaDon_ChiTiet ct1
    		join DonViQuiDoi qd on ct1.ID_DonViQuiDoi = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
			--- get CTHD Tra (Tra goiDV thi Bh_Chitiet co ID_ChiTietGoiDV = null
			left join 
					(select hd3.ID_HoaDon as ID_HoaDonGoc, ct3.ID_DonViQuiDoi, ct3.ID_TangKem, ct3.ID_LoHang,
							case when ct3.ID_TangKem is null then SUM(ISNULL(ct3.SoLuong,0)) end as SLTraThuong,
							case when ct3.ID_TangKem is not null then SUM(ISNULL(ct3.SoLuong,0)) end as SLTraKMai
					from BH_HoaDon_ChiTiet ct3
					join BH_HoaDon hd3 on ct3.ID_HoaDon = hd3.ID 
					where hd3.ChoThanhToan ='0'and hd3.LoaiHoaDon = 6
					group by hd3.ID_HoaDon, ct3.ID_DonViQuiDoi, ct3.ID_TangKem, ct3.ID_LoHang 
					) hdt on ct1.ID_DonViQuiDoi = hdt.ID_DonViQuiDoi and ct1.ID_HoaDon = hdt.ID_HoaDonGoc
					and  ((ct1.ID_LoHang is null and hdt.ID_LoHang is null) or (ct1.ID_LoHang = hdt.ID_LoHang))

			-- get CTHD su dung thuoc hoa don chua bi huy
    		left join
				(select ct2.ID, ct2.ID_ChiTietGoiDV, ct2.SoLuong, ct2.ID_ChiTietDinhLuong, hd2.MaHoaDon		
				from BH_HoaDon_ChiTiet ct2 
				join BH_HoaDon hd2 on ct2.ID_HoaDon =hd2.ID
				where hd2.ChoThanhToan='0' and hd2.LoaiHoaDon in (1)  and (ct2.ID_ChiTietDinhLuong is null or ct2.ID_ChiTietDinhLuong =ct2.ID)
				 ) ctsd on ctsd.ID_ChiTietGoiDV = ct1.ID

    		where ct1.ID_HoaDon = @ID_HoaDon
    		group by ct1.ID,ct1.ID_DonViQuiDoi,ct1.ID, ct1.ID_HoaDon,ct1.DonGia,ct1.GiaVon,ct1.ThanhTien,qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,
    		ct1.ThoiGian, ct1.GhiChu,ct1.ID_LoHang, ct1.SoThuTu, ct1.TangKem,ct1.ID_TangKem,hh.LaHangHoa, hh.TenHangHoa, qd.TyLeChuyenDoi,qd.LaDonViChuan,hh.QuyCach, hdt.ID_LoHang
    		-- chi lay Hang co SoLuongConLai > 0
    		Having (max(ct1.SoLuong)  - MAX(ISNULL(hdt.SLTraThuong,0)) - SUM(ISNULL(ctsd.SoLuong,0)) > 0)
			-- hoac hang khuyen mai co soluongconali > 0
			 OR (max(ct1.SoLuong) - MAX(ISNULL(hdt.SLTraKMai,0)) - SUM(ISNULL(ctsd.SoLuong,0)) >0  and ct1.TangKem='1')
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateTonForDM_hangHoa_TonKho]
    @IDHoaDonInput [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @LoaiHoaDon INT;
	DECLARE @IDChiNhanhInput  UNIQUEIDENTIFIER;
	DECLARE @IDCheckIn  UNIQUEIDENTIFIER;
	DECLARE @YeuCau NVARCHAR(MAX);
	SELECT @LoaiHoaDon = LoaiHoaDon, @IDChiNhanhInput = ID_DonVi, @IDCheckIn = ID_CheckIn, @YeuCau = YeuCau  FROM BH_HoaDon where ID = @IDHoaDonInput;

    DECLARE @ChiTietHoaDon1 TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TyLeChuyenDoi FLOAT) 
    INSERT INTO @ChiTietHoaDon1 SELECT dvqd2.ID,
    hdct.ID_LoHang, dvqd2.ID_HangHoa, dvqd2.TyLeChuyenDoi 
    FROM BH_HoaDon_ChiTiet hdct
    INNER JOIN DonViQuiDoi dvqd1
    on hdct.ID_DonViQuiDoi = dvqd1.ID
    INNER JOIN DM_HangHoa hh
    on dvqd1.ID_HangHoa = hh.ID
    INNER JOIN DonViQuiDoi dvqd2
    on hh.ID = dvqd2.ID_HangHoa
    WHERE hdct.ID_HoaDon = @IDHoaDonInput and hh.LaHangHoa = 1
    GROUP BY dvqd2.ID, hdct.ID_LoHang, dvqd2.ID_HangHoa,dvqd2.TyLeChuyenDoi
    
	DECLARE @tblTonKho1 TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, TonKho FLOAT, ID_LoHang UNIQUEIDENTIFIER)
	INSERT INTO @tblTonKho1
	SELECT ID_DonViQuiDoi, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput,ID_HangHoa,ID_LoHang, DATEADD(minute, 2,GETDATE()))/TyLeChuyenDoi as TonKho, ID_LoHang FROM @ChiTietHoaDon1

	--SELECT * FROM @tblTonKho1;

    UPDATE hhtonkho SET hhtonkho.TonKho = ISNULL(cthoadon.TonKho, 0)
    FROM DM_HangHoa_TonKho hhtonkho
    INNER JOIN @tblTonKho1 as cthoadon on hhtonkho.ID_DonViQuyDoi = cthoadon.ID_DonViQuiDoi and (hhtonkho.ID_LoHang = cthoadon.ID_LoHang or cthoadon.ID_LoHang is null) and hhtonkho.ID_DonVi = @IDChiNhanhInput

    INSERT INTO DM_HangHoa_TonKho(ID, ID_DonVi, ID_DonViQuyDoi, ID_LoHang, TonKho)
	SELECT NEWID(), @IDChiNhanhInput, cthoadon1.ID_DonViQuiDoi, cthoadon1.ID_LoHang, cthoadon1.TonKho
    FROM @tblTonKho1 AS cthoadon1
    LEFT JOIN DM_HangHoa_TonKho hhtonkho1 on hhtonkho1.ID_DonViQuyDoi = cthoadon1.ID_DonViQuiDoi and (hhtonkho1.ID_LoHang = cthoadon1.ID_LoHang or cthoadon1.ID_LoHang is null) and hhtonkho1.ID_DonVi = @IDChiNhanhInput
	WHERE hhtonkho1.ID IS NULL

    IF(@LoaiHoaDon = 10 and @YeuCau != '1')
    BEGIN
		--Tao bang TonKho nhan hang
		DECLARE @tblTonKho2 TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, TonKho FLOAT, ID_LoHang UNIQUEIDENTIFIER)
		INSERT INTO @tblTonKho2
		SELECT ID_DonViQuiDoi, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDCheckIn,ID_HangHoa,ID_LoHang, DATEADD(minute, 2,GETDATE()))/TyLeChuyenDoi as TonKho, ID_LoHang FROM @ChiTietHoaDon1

		UPDATE hhtonkho2 SET hhtonkho2.TonKho = ISNULL(cthoadon2.TonKho, 0)
		FROM DM_HangHoa_TonKho hhtonkho2
		INNER JOIN @tblTonKho2 as cthoadon2 on hhtonkho2.ID_DonViQuyDoi = cthoadon2.ID_DonViQuiDoi and (hhtonkho2.ID_LoHang = cthoadon2.ID_LoHang or cthoadon2.ID_LoHang is null) and hhtonkho2.ID_DonVi = @IDCheckIn

		INSERT INTO DM_HangHoa_TonKho(ID, ID_DonVi, ID_DonViQuyDoi, ID_LoHang, TonKho)
		SELECT NEWID(), @IDCheckIn, cthoadon3.ID_DonViQuiDoi, cthoadon3.ID_LoHang, cthoadon3.TonKho
		FROM @tblTonKho2 AS cthoadon3
		LEFT JOIN DM_HangHoa_TonKho hhtonkho3 on hhtonkho3.ID_DonViQuyDoi = cthoadon3.ID_DonViQuiDoi and (hhtonkho3.ID_LoHang = cthoadon3.ID_LoHang or cthoadon3.ID_LoHang is null) and hhtonkho3.ID_DonVi = @IDCheckIn
		WHERE hhtonkho3.ID IS NULL
    END
END");

        }
        
        public override void Down()
        {
        }
    }
}
