namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20200220 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetInforProduct_ByIDQuiDoi]", parametersAction: p => new
            {
                IDQuiDoi = p.Guid(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SET NOCOUNT ON;

		Select 
			qd.ID as ID_DonViQuiDoi,
    		hh.ID,
			qd.MaHangHoa,
    		hh.TenHangHoa,
			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		qd.TenDonViTinh,
			hh.QuanLyTheoLoHang,
			qd.TyLeChuyenDoi,
			hh.LaHangHoa,
			Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
			qd.GiaBan,
			qd.GiaNhap,
			CAST(ROUND(ISNULL(hhtonkho.TonKho, 0), 3) as float) as TonKho,
			ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
			Case when lh.ID is null then null else lh.ID end as ID_LoHang,
			lh.MaLoHang,
    		lh.NgaySanXuat,
			lh.NgayHetHan
    	from DonViQuiDoi qd    	
    	left join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
		LEFT join DM_HangHoa_Anh an on (qd.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    	left join DM_LoHang lh on qd.ID_HangHoa = lh.ID_HangHoa and (lh.TrangThai = 1 or lh.TrangThai is null)
		left join DM_GiaVon gv on qd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or lh.ID is null) and gv.ID_DonVi = @ID_ChiNhanh
		left join DM_HangHoa_TonKho hhtonkho on qd.ID = hhtonkho.ID_DonViQuyDoi and (hhtonkho.ID_LoHang = lh.ID or lh.ID is null)
		and hhtonkho.ID_DonVi = @ID_ChiNhanh
		where qd.ID = @IDQuiDoi");

            CreateStoredProcedure(name: "[dbo].[GetListDichVu_inLichHen_ByEventID]", parametersAction: p => new
            {
                EventID = p.Guid()
            }, body: @"SET NOCOUNT ON;
	declare @names nvarchar(max)

	select @names = NoiDung from ChamSocKhachHangs
	where ID= @EventID

	select qd.ID ID_DonViQuiDoi, qd.MaHangHoa, hh.TenHangHoa
	from DM_HangHoa hh
	join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa
	where qd.MaHangHoa in (select * from dbo.splitstring(@names))
	and qd.Xoa='0'");

			CreateStoredProcedure(name: "[dbo].[GetMaHoaDon_Copy]", parametersAction: p => new
			{
				MaHoaDon = p.String(30)
			}, body: @"SET NOCOUNT ON;
	declare @mahoadongoc varchar(30) = (select top 1 MaHoaDon from BH_HoaDon
			where CHARINDEX(MaHoaDon,@MaHoaDon) > 0 and MaHoaDon not like '%copy%')
	if CHARINDEX('copy',@MaHoaDon) = 0
		select @MaHoaDon as MaxCode
	else
		begin
		declare @count int =
			(select count(ID) from BH_HoaDon
			where CHARINDEX(MaHoaDon,@MaHoaDon) > 0 and MaHoaDon like '%copy%')
		select CONCAT('Copy', @count+1 ,'_', @mahoadongoc) as MaxCode
		end");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetAll_DMLoHang_TonKho]
    @ID_ChiNhanh [uniqueidentifier],
    @timeChotSo [datetime]
AS
BEGIN
SET NOCOUNT ON;
    Select
    		lh.ID,
    		dvqd.ID as ID_DonViQuiDoi,
    		lh.ID_HangHoa,
			dhh.TenHangHoa,	
			dvqd.MaHangHoa,
    		--lh.TenLoHang as TenLoHangFull,
    		lh.MaLoHang,
    		lh.NgaySanXuat as NgaySanXuat,
    		lh.NgayHetHan,
    		lh.TrangThai,
    		round(tk.TonKho,2) as TonKho,
			dvqd.xoa
    	FROM  DM_LoHang lh 
		join DM_HangHoa dhh on lh.ID_HangHoa = dhh.ID
		join DonViQuiDoi dvqd on dvqd.ID_HangHoa = lh.ID_HangHoa
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)	
		left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or lh.ID is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where lh.ID is not null	 
		and (dhh.LaHangHoa = 0 or (dhh.LaHangHoa = 1 and tk.TonKho is not null)) -- chi lay HangHoa neu exsit in DM_TonKho_HangHoa
		-- get all: lấy cả Lô hết hạn và hàng hóa đã bị xóa --> use when TraHang
		order by lh.ID
END

--SP_GetAll_DMLoHang_TonKho 'e49575bf-e4f7-489b-b8b0-b0d9db97ca13','2016-01-01'");

            Sql(@"update ChamSocKhachHangs 
set NoiDung = (select max(qd.MaHangHoa) +', '  AS [text()] 
				from DM_HangHoa hh
				join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa
				where hh.TenHangHoa in (select * from dbo.splitstring(ChamSocKhachHangs.Ma_TieuDe))
				and qd.Xoa='0'
				group by TenHangHoa For XML PATH ('')
	)
where PhanLoai= 3 
and (NoiDung is null or NoiDung='');");
            CreateStoredProcedure(name: "[dbo].[KhoiTaoDuLieuLanDau]", parametersAction: p => new
            {
                Subdomain = p.String()
            }, body: @"SET NOCOUNT ON;
	DECLARE @ThoiGian DATETIME;
	SET @ThoiGian = GETDATE();
	DECLARE @IDNganhNgheKinhDoanh UNIQUEIDENTIFIER;
	DECLARE @TenCuaHang NVARCHAR(max), @DiaChi NVARCHAR(max), @Email NVARCHAR(max), @SoDienThoai NVARCHAR(max), @TenNhanVien NVARCHAR(max),
	@TaiKhoan NVARCHAR(MAX), @MatKhau NVARCHAR(MAX);
	SELECT 
	@TenCuaHang = IIF(TenCuaHang != '', TenCuaHang, 'Open24.vn'), 
	@DiaChi = IIF(DiaChi != '', DiaChi, 'Open24.vn'), 
	@Email = IIF(Email != '', Email, ''),
	@SoDienThoai = IIF(SoDienThoai != '', SoDienThoai, ''),
	@TenNhanVien = IIF(HoTen != '', HoTen, 'Open24.vn'),
	@TaiKhoan = UserKT,
	@MatKhau = MatKhauKT,
	@IDNganhNgheKinhDoanh = ID_NganhKinhDoanh
	FROM BANHANG24..CuaHangDangKy WHERE SubDomain = @Subdomain;
	--INSERT HT_CongTy
	INSERT INTO HT_CongTy (ID, TenCongTy, DiaChi, SoDienThoai, SoFax, MaSoThue, Mail, Website, TenGiamDoc, TenKeToanTruong, Logo, GhiChu, 
	TaiKhoanNganHang, ID_NganHang, DiaChiNganHang, TenVT, DiaChiVT, DangHoatDong, DangKyNhanSu, NgayCongChuan)
	VALUES ('4DE12030-B7FE-487A-B6B2-A99665E8AE7C', @TenCuaHang, @DiaChi, @SoDienThoai, '', '', @Email, '', '', '', NULL, '',
	'', NULL, '', @TenCuaHang, @DiaChi, 1, 0, 26);
	--INSERT DM_DonVi
	DECLARE @IDDonVi UNIQUEIDENTIFIER;
	SET @IDDonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE';
	INSERT INTO DM_DonVi (ID, MaDonVi, TenDonVi, DiaChi, Website, MaSoThue, SoTaiKhoan, SoDienThoai, SoFax, KiTuDanhMa, HienThi_Chinh, HienThi_Phu,
	NgayTao, NguoiTao, TrangThai)
	VALUES (@IDDonVi, 'CTY', @TenCuaHang, @DiaChi, @Subdomain + '.open24.vn', '', '', @SoDienThoai, '', @Subdomain,1, 1, @ThoiGian, 'ssoftvn', 1);
	--INSERT NS_NhanVien
	DECLARE @IDNhanVien UNIQUEIDENTIFIER;
	SET @IDNhanVien = NEWID();
	INSERT INTO NS_NhanVien (ID, MaNhanVien, TenNhanVien, GioiTinh, DienThoaiDiDong, Email, CapTaiKhoan, DaNghiViec, NguoiTao, NgayTao, TrangThai)
	VALUES (@IDNhanVien, 'NV01', @TenNhanVien, 1, @SoDienThoai, @Email, 1, 0, 'ssoftvn', @ThoiGian, 1);
	--INSERT QuaTrinhCongTac
	INSERT INTO NS_QuaTrinhCongTac (ID, ID_NhanVien, ID_DonVi, NgayApDung, LaChucVuHienThoi, LaDonViHienThoi, NguoiLap, NgayLap)
	VALUES (NEWID(), @IDNhanVien, @IDDonVi, @ThoiGian, 1, 1, 'ssoftvn', @ThoiGian);
	--INSERT HT_NguoiDung
	DECLARE @IDNguoiDung UNIQUEIDENTIFIER;
	SET @IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77';
	INSERT INTO HT_NguoiDung (ID, ID_NhanVien, TaiKhoan, MatKhau, LaNhanVien, LaAdmin, DangHoatDong, IsSystem, NguoiTao, NgayTao, ID_DonVi, XemGiaVon, SoDuTaiKhoan)
	VALUES (@IDNguoiDung, @IDNhanVien, @TaiKhoan, @MatKhau, 1, 1, 1, 1, 'ssoftvn', @ThoiGian, @IDDonVi, 0, 0);
	--INSERT HT_NhomNguoiDung
	DECLARE @IDNhomNguoiDung UNIQUEIDENTIFIER;
	SET @IDNhomNguoiDung = 'EE609285-F6A6-43D8-A517-BDA52F426AE5';
	INSERT INTO HT_NhomNguoiDung (ID, MaNhom, TenNhom, MoTa, NguoiTao, NgayTao)
	VALUES (@IDNhomNguoiDung, 'ADMIN', 'ADMIN', N'Nhóm quản trị', 'ssoftvn', @ThoiGian);
	--INSERT HT_NguoiDung_Nhom
	INSERT INTO HT_NguoiDung_Nhom (ID, IDNguoiDung, IDNhomNguoiDung, ID_DonVi)
	VALUES (NEWID(), @IDNguoiDung, @IDNhomNguoiDung, @IDDonVi);
	--INSERT HT_Quyen
	INSERT INTO HT_Quyen (MaQuyen, TenQuyen, MoTa, QuyenCha, DuocSuDung)
	SELECT q.MaQuyen, q.TenQuyen, q.MoTa, q.QuyenCha, q.DuocSuDung FROM BANHANG24..HT_Quyen q
	INNER JOIN BANHANG24..HT_Quyen_NganhNgheKinhDoanh qn ON q.MaQuyen = qn.MaQuyen
	WHERE qn.ID_NganhKinhDoanh = @IDNganhNgheKinhDoanh;
	--INSERT HT_Quyen_Nhom
	INSERT INTO HT_Quyen_Nhom (ID, ID_NhomNguoiDung, MaQuyen)
	SELECT NEWID(), @IDNhomNguoiDung, MaQuyen FROM HT_Quyen;
	--INSERT HT_CauHinhPhanMem
	INSERT INTO HT_CauHinhPhanMem (ID, ID_DonVi, GiaVonTrungBinh, CoDonViTinh, DatHang, XuatAm, DatHangXuatAm, ThayDoiThoiGianBanHang, SoLuongTrenChungTu,
	TinhNangTichDiem, GioiHanThoiGianTraHang, SanPhamCoThuocTinh, BanVaChuyenKhiHangDaDat, TinhNangSanXuatHangHoa, SuDungCanDienTu, KhoaSo, InBaoGiaKhiBanHang,
	QuanLyKhachHangTheoDonVi, KhuyenMai, LoHang, SuDungMauInMacDinh, ApDungGopKhuyenMai, ThongTinChiTietNhanVien, BanHangOffline, ThoiGianNhacHanSuDungLo,
	SuDungMaChungTu, ChoPhepTrungSoDienThoai)
	VALUES (NEWID(), @IDDonVi, 1, 1, 1, 0, 0, 0, 0,
	0, 0, 1, 0, 0, 0, 1, 0,
	0, 1, 0, 0, 0, 0, 0, 0,
	0, 0);
	--INSERT DM_QuocGia
	INSERT INTO DM_QuocGia (ID, MaQuocGia, TenQuocGia, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaQuocGia, TenQuocGia, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_QuocGia;
	--INSERT DM_VungMien
	INSERT INTO DM_VungMien (ID, MaVung, TenVung, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaVung, TenVung, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_VungMien;
	--INSERT DM_TinhThanh
	INSERT INTO DM_TinhThanh (ID, MaTinhThanh, TenTinhThanh, ID_QuocGia, ID_VungMien, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaTinhThanh, TenTinhThanh, ID_QuocGia, ID_VungMien, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_TinhThanh;
	--INSERT DM_QuanHuyen
	INSERT INTO DM_QuanHuyen (ID, MaQuanHuyen, TenQuanHuyen, ID_TinhThanh, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaQuanHuyen, TenQuanHuyen, ID_TinhThanh, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_QuanHuyen;
	--INSERT DM_ThueSuat
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'0%', 0, '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'5%', 5, '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'10%', 10, '', 'ssoftvn', @ThoiGian);
	--INSERT DM_TienTe
	DECLARE @IDTienTe UNIQUEIDENTIFIER;
	DECLARE @IDQuocGia UNIQUEIDENTIFIER;
	SET @IDTienTe = '406eed2d-faae-4520-aef2-12912f83dda2';
	SELECT @IDQuocGia = ID FROM DM_QuocGia WHERE MaQuocGia = 'VNI';
	INSERT INTO DM_TienTe (ID, ID_QuocGia, LaNoiTe, MaNgoaiTe, TenNgoaiTe, NguoiTao, NgayTao)
	VALUES (@IDTienTe, @IDQuocGia, 1, N'VND', N'Việt Nam đồng', 'ssoftvn', @ThoiGian);
	--INSERT DM_TyGia
	INSERT INTO DM_TyGia (ID, ID_TienTe, TyGia, NgayTyGia, GhiChu, NguoiTao, NgayTao)
	VALUES (NEWID(), @IDTienTe, 1, @ThoiGian, '', 'ssoftvn', @ThoiGian);
	--INSERT DM_Kho
	DECLARE @IDKho UNIQUEIDENTIFIER;
	SET @IDKho = '01CD02F2-4612-4104-B790-1C0373CBD72D';
	INSERT INTO DM_Kho (ID, MaKho, TenKho, NguoiTao, NgayTao)
	VALUES (@IDKho, N'KHO_CTy', N'Kho tổng', 'ssoftvn', @ThoiGian);
	--INSERT Kho_DonVi
	INSERT INTO Kho_DonVi (ID, ID_DonVi, ID_Kho)
	VALUES (NEWID(), @IDDonVi, @IDKho);
	--INSERT DM_LoaiChungTu
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (1, 'HDBL', N'Hóa đơn bán lẻ', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (2, 'HDB', N'Hóa đơn bán', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (3, 'BG', N'Báo giá', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (4, 'PNK', N'Phiếu nhập kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (5, 'PXK', N'Phiếu xuất kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (6, 'TH', N'Trả hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (7, 'THNCC', N'Trả hàng nhà cung cấp', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (8, 'XH', N'Xuất kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (9, 'PKK', N'Phiếu kiểm kê', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (10, 'CH', N'Chuyển hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (11, 'SQPT', N'Phiếu thu', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (12, 'SQPC', N'Phiếu chi', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (13, 'NH', N'Nhận hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (14, 'DH', N'Đặt hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (15, 'CB', N'Điều chỉnh', N'Điều chỉnh công nợ khách hàng, nhà cung cấp', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (16, 'KTGV', N'Khởi tạo giá vốn', N'Khởi tạo giá vốn khi tạo hàng hóa', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (17, 'DTH', N'Đổi trả hàng', N'Đổi trả hàng hóa', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (18, 'DCGV', N'Điều chỉnh giá vốn', N'Điều chỉnh giá vốn', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (19, 'GDV', N'Gói dịch vụ', N'Bán gói dịch vụ', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (20, 'TGDV', N'Trả gói dịch vụ', N'Trả gói dịch vụ', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (21, 'IMV', N'Tem - Mã vạch', N'Tem - Mã vạch', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (22, 'TGT', N'Thẻ giá trị', N'Bán, nạp thẻ giá trị', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (23, 'DCGT', N'Điều chỉnh thẻ giá trị', N'Điều chỉnh giá trị thẻ giá trị', 'ssoftvn', @ThoiGian);
	--INSERT HT_ThongBao_CaiDat
	INSERT INTO HT_ThongBao_CaiDat (ID, ID_NguoiDung, NhacSinhNhat, NhacCongNo, NhacTonKho, NhacDieuChuyen, NhacLoHang)
	VALUES (NEWID(), @IDNguoiDung, 1, 1, 1, 1, 1);
	--INSERT NS_PhongBan
	DECLARE @IDPhongBan UNIQUEIDENTIFIER;
	SET @IDPhongBan = '6DE963A7-50AF-4E51-91E8-E242D7E7B476';
	INSERT INTO NS_PhongBan (ID, MaPhongBan, TenPhongBan, TrangThai)
	VALUES (@IDPhongBan, 'PB0000', N'Phòng ban mặc định', 1);
	--INSERT DM_NganHang
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'CTG', N'Ngân hàng Công thương Việt Nam (VietinBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIETBANK', N'Ngân hàng Việt Nam Thương Tín (VietBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'OCB', N'Ngân hàng Phương Đông (Orient Commercial Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NAMABANK', N'Ngân hàng Nam Á (Nam A Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SEABANK', N'Ngân hàng Đông Nam Á (SeABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'AGRIBANK', N'Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam (Agribank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'MBB', N'Ngân hàng Quân đội (Military Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'ACB', N'Ngân hàng Á Châu (ACB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIB', N'Ngân hàng Quốc tế (VIBBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'DAF', N'Ngân hàng Đông Á (DAF)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BVB', N'Ngân hàng Bảo Việt (BaoVietBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VDB', N'Ngân hàng Phát triển Việt Nam (VDB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'KIENLONGBANK', N'Ngân hàng Kiên Long (KienLongBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'HDBANK', N'Ngân hàng Phát triển nhà Thành Phố Hồ Chí Minh (HDBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SHB', N'Ngân hàng Sài Gòn - Hà Nội (SHBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'EIB', N'Ngân hàng Xuất Nhập khẩu Việt Nam (Eximbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'CB', N'Ngân hàng Xây dựng (CB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SGB', N'Ngân hàng Sài Gòn Công Thương (Saigonbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'PVCOMBANK', N'Ngân hàng Đại chúng (PVcom Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'PGBANK', N'Ngân hàng Xăng dầu Petrolimex (Petrolimex Group Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'OCEANBANK', N'Ngân hàng Đại Dương (Oceanbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'TECHCOMBANK', N'Ngân hàng Kỹ Thương Việt Nam (Techcombank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VAB', N'Ngân hàng Việt Á (VietABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'STB', N'Ngân hàng Sài Gòn Thương Tín (Sacombank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'MSB', N'Ngân hàng Hàng Hải Việt Nam (MaritimeBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VCB', N'Ngân hàng Ngoại thương Việt Nam (Ngoại Thương Việt Nam)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'GPBANK', N'Ngân hàng Dầu khí Toàn Cầu (GPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIETCAPITALBANK', N'Ngân hàng Bản Việt (VietCapitalBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SCB', N'Ngân hàng Sài Gòn (Sài Gòn)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'LPB', N'Ngân hàng Bưu điện Liên Việt (LienVietPostBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BID', N'Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'ABBANK', N'Ngân hàng An Bình (ABBANK)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NHCSXH/VBSP', N'Ngân hàng Chính sách xã hội (NHCSXH/VBSP)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'TPBANK', N'Ngân hàng Tiên Phong (TPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BACABANK', N'Ngân hàng Bắc Á (BacABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VPBANK', N'Ngân hàng Việt Nam Thịnh Vượng (VPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NCB', N'Ngân hàng Quốc Dân (National Citizen Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	--INSERT OptinForm_TruongThongTin
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Chọn chi nhánh', 2, 1, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Giới tính', 1, 3, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số lượng', 2, 5, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Địa chỉ', 1, 7, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Thời gian', 2, 9, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Khách hàng', 2, 3, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Email', 1, 6, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Dịch vụ yêu cầu', 2, 8, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Khách hàng lẻ', 1, 11, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Xưng hô', 2, 2, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Nhân viên thực hiện', 2, 10, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số điện thoại', 2, 6, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Tỉnh thành', 1, 8, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Mã số thuế', 1, 10, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Bạn đi theo nhóm', 2, 4, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ngày sinh', 1, 4, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Quận huyện', 1, 9, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số điện thoại', 1, 5, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ảnh đại diện', 1, 1, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ghi chú', 2, 11, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Email', 2, 7, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Người giới thiệu', 1, 12, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Tên khách hàng', 1, 2, 1);
	--INSERT DM_DoiTuong
	INSERT INTO DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong, TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, ChiaSe, TheoDoi,
	ID_DonVi, TenNhomDoiTuongs, NguoiTao, NgayTao)
	VALUES ('00000000-0000-0000-0000-000000000000', 1, 1, 'KL00001', N'Khách lẻ', 'Khach le', 'Kl', 0, 0, @IDDonVi, N'Nhóm mặc định', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong, TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, ChiaSe, TheoDoi,
	ID_DonVi, TenNhomDoiTuongs, NguoiTao, NgayTao)
	VALUES ('00000000-0000-0000-0000-000000000002', 2, 1, 'NCCL001', N'Nhà cung cấp lẻ', 'Nha cung cap le', 'nccl', 0, 0, @IDDonVi, N'Nhóm mặc định', 'ssoftvn', @ThoiGian);
	--INSERT DM_NhomHangHoa
	INSERT INTO DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa, LaNhomHangHoa, NguoiTao, NgayTao, HienThi_Chinh, HienThi_Phu, HienThi_BanThe,
	TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau)
	VALUES ('00000000-0000-0000-0000-000000000000', 'NHMD00001', N'Nhóm hàng hóa mặc định', 1, 'ssoftvn', @ThoiGian, 1, 1, 1, 'Nhom hang hoa mac dinh', 'Nhhmd');
	INSERT INTO DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa, LaNhomHangHoa, NguoiTao, NgayTao, HienThi_Chinh, HienThi_Phu, HienThi_BanThe,
	TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau)
	VALUES ('00000000-0000-0000-0000-000000000001', 'DVMD00001', N'Nhóm dịch vụ mặc định', 0, 'ssoftvn', @ThoiGian, 1, 1, 1, 'Nhom dich vu mac dinh', 'Ndvmd');");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetInforProduct_ByIDQuiDoi]");
            DropStoredProcedure("[dbo].[GetListDichVu_inLichHen_ByEventID]");
            DropStoredProcedure("[dbo].[GetMaHoaDon_Copy]");
            DropStoredProcedure("[dbo].[KhoiTaoDuLieuLanDau]");
        }
    }
}
