namespace Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddUpdateSP_20190422 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getList_SoSanhCungKyThuChi]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"SELECT
    	CAST(ROUND((CAST(m.TienThuThangNay - m.TienThuThangTruoc as float)) / (m.TienThuThangTruoc) *100, 2) as float) as ThuCungKy,
		CAST(ROUND((CAST(m.TienChiThangNay - m.TienChiThangTruoc as float)) / (m.TienChiThangTruoc) *100, 2) as float) as ChiCungKy
    	FROM
    	(
			SELECT
    		SUM(a.TienThuThangNay) as TienThuThangNay,
    		SUM(a.TienChiThangNay) as TienChiThangNay,
    		SUM(a.TienThuThangTruoc) as TienThuThangTruoc,
    		SUM(a.TienChiThangTruoc) as TienChiThangTruoc
    		FROM
    		(
			SELECT
			CAST(ROUND(SUM(k.TienThu_Thu), 0) as float) as TienThuThangNay,
			CAST(ROUND(SUM(k.TienThu_Chi), 0) as float) as TienChiThangNay,
			0 as TienThuThangTruoc,
			0 as TienChiThangTruoc
			FROM
			(
				Select 
				0 as TienThu_Chi,
				SUM(TienMat + TienGui) as TienThu_Thu
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
				and qhd.ID_DonVi = @ID_ChiNhanh
				and qhd.LoaiHoaDon = 11
				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
				Union all -- tiền chi
				select SUM(TienMat + TienGui) as TienThu_Chi,
				0 as TienThu_Thu
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
				and qhd.ID_DonVi = @ID_ChiNhanh
				and qhd.LoaiHoaDon = 12
				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
			) as k
		-- tháng trước
			Union all
			SELECT
			0 as TienThuThangNay,
			0 as TienChiThangNay,
			CAST(ROUND(SUM(k.TienThu_Thu), 0) as float) as TienThuThangTruoc,
			CAST(ROUND(SUM(k.TienThu_Chi), 0) as float) as TienChiThangTruoc 
			FROM
			(
				Select 
				0 as TienThu_Chi,
				SUM(TienMat + TienGui) as TienThu_Thu
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				where qhd.NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and qhd.NgayLapHoaDon < DateAdd(month, -1, @timeEnd) 
				and qhd.ID_DonVi = @ID_ChiNhanh
				and qhd.LoaiHoaDon = 11
				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
				Union all -- tiền chi
				select SUM(TienMat + TienGui) as TienThu_Chi,
				0 as TienThu_Thu
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				where qhd.NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and qhd.NgayLapHoaDon < DateAdd(month, -1, @timeEnd) 
				and qhd.ID_DonVi = @ID_ChiNhanh
				and qhd.LoaiHoaDon = 12
				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
			) as k
			) as a
    	) as m");

            CreateStoredProcedure(name: "[dbo].[getlist_SuKienToDay]", parametersAction: p => new
            {
                ID_DonVi = p.Guid()
            }, body: @"Select 
CAST(ROUND(SUM(a.SinhNhat),0) as float) as SinhNhat,
CAST(ROUND(SUM(a.CongViec),0) as float) as CongViec,
CAST(ROUND(SUM(a.LichHen),0) as float) as LichHen
FROM
(
select Count(*) as SinhNhat, 0 as CongViec, 0 as LichHen from DM_DoiTuong 
where TheoDoi != 1 and NgaySinh_NgayTLap is not null
and DAY(NgaySinh_NgayTLap) = DAY(GETDATE())
and MONTH(NgaySinh_NgayTLap)= MONTH(GETDATE())
Union All
Select 0 as SinhNhat, COUNT(*) as CongViec, 0 as LichHen from ChamSocKhachHangs
where DATEADD(DAY,1, NgayGioKetThuc) > GETDATE()
and NgayGio <= GETDATE()
and NgayGioKetThuc >=  GETDATE()
and TrangThai = 1
and ID_DonVi = @ID_DonVi
and PhanLoai = 4 --4: Công Việc, 3: LichHen
Union ALL
Select 0 as SinhNhat, 0 as CongViec, COUNT(*) as LichHen from ChamSocKhachHangs
where DATEADD(DAY,1, NgayGioKetThuc) > GETDATE()
and NgayGio <= GETDATE()
and NgayGioKetThuc >=  GETDATE()
and TrangThai = 1
and ID_DonVi = @ID_DonVi
and PhanLoai = 3 --4: Công Việc, 3: LichHen
) a");

            CreateStoredProcedure(name: "[dbo].[getList_TongQuanThuChi]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"    -- khach hàng mới
SELECT
CAST(ROUND(SUM(k.TienThu_Thu), 0) as float) as TienThu_Thu,
CAST(ROUND(SUM(k.TienMat_Thu), 0) as float) as TienMat_Thu,
CAST(ROUND(SUM(k.NganHang_Thu), 0) as float) as NganHang_Thu,
CAST(ROUND(SUM(k.TienThu_Chi), 0) as float) as TienThu_Chi,
CAST(ROUND(SUM(k.TienMat_Chi), 0) as float) as TienMat_Chi,
CAST(ROUND(SUM(k.NganHang_Chi), 0) as float) as NganHang_Chi
FROM
(
    Select 
	0 as TienThu_Chi,
	0 as TienMat_Chi,
	0 as NganHang_Chi,
	SUM(TienMat + TienGui) as TienThu_Thu, SUM(TienMat) as TienMat_Thu, Sum(TienGui) as NganHang_Thu
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
	where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
	and qhd.ID_DonVi = @ID_ChiNhanh
	and qhd.LoaiHoaDon = 11
	and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
	Union all -- tiền chi
	select SUM(TienMat + TienGui) as TienThu_Chi, SUM(TienMat) as TienMat_Chi, Sum(TienGui) as NganHang_Chi,
	0 as TienThu_Thu,
	0 as TienMat_Thu,
	0 as NganHang_Thu
	from Quy_HoaDon_ChiTiet qct
	join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
	where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
	and qhd.ID_DonVi = @ID_ChiNhanh
	and qhd.LoaiHoaDon = 12
	and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
) as k");

            Sql(@"ALTER PROCEDURE [dbo].[getList_SoSanhCungKyKhachHang]
    @ID_ChiNhanh [uniqueidentifier],
    @timeStart [datetime],
    @timeEnd [datetime]
AS
BEGIN
    SELECT
    	CAST(ROUND((CAST(m.KhachHangQuayLaiThangNay + m.KhachHangTaoMoiThangNay - m.KhachHangQuayLaiThangTruoc - m.KhachHangTaoMoiThangTruoc as float)) / (m.KhachHangQuayLaiThangTruoc + m.KhachHangTaoMoiThangTruoc) *100, 2) as float) as SoSanhCungKy
    	FROM
    	(
    	SELECT
    	SUM(k.KhachHangTaoMoiThangNay) as KhachHangTaoMoiThangNay,
    	SUM(k.KhachHangQuayLaiThangNay) as KhachHangQuayLaiThangNay,
    	SUM(k.KhachHangTaoMoiThangTruoc) as KhachHangTaoMoiThangTruoc,
    	SUM(k.KhachHangQuayLaiThangTruoc) as KhachHangQuayLaiThangTruoc
    	FROM
    	(
		Select 
		Count (*) as KhachHangTaoMoiThangNay,
    	0 as KhachHangQuayLaiThangNay,
    	0 as KhachHangTaoMoiThangTruoc,
    	0 as KhachHangQuayLaiThangTruoc
		from 
		(select 
		dt.ID, COUNT(*) as solan
		from
		DM_DoiTuong dt 
		left join ChamSocKhachHangs cs on dt.ID = cs.ID_KhachHang
		left join BH_HoaDon bh on bh.ID_DoiTuong = dt.ID
		left join BH_HoaDon hds on bh.ID_DoiTuong = hds.ID_DoiTuong and bh.ID != hds.ID and bh.ID_DonVi = hds.ID_DonVi
		where (dt.TheoDoi is null or dt.TheoDoi = 0)
		and dt.LoaiDoiTuong = 1
		and bh.ID_DonVi = @ID_ChiNhanh
		and ((bh.NgayLapHoaDon >= @timeStart and bh.NgayLapHoaDon < @timeEnd) or (cs.NgayTao >= @timeStart and cs.NgayTao < @timeEnd))
		and hds.ID is null
		GROUP by dt.ID having COUNT(*) = 1
		)a
		Union all
		Select 
		0 as KhachHangTaoMoiThangNay,
    	COUNT (*) as KhachHangQuayLaiThangNay,
    	0 as KhachHangTaoMoiThangTruoc,
    	0 as KhachHangQuayLaiThangTruoc
		from 
		(select 
		dt.ID, COUNT(*) as solan
		from
		DM_DoiTuong dt 
		left join ChamSocKhachHangs cs on dt.ID = cs.ID_KhachHang
		left join BH_HoaDon bh on bh.ID_DoiTuong = dt.ID
		left join BH_HoaDon hds on bh.ID_DoiTuong = hds.ID_DoiTuong and bh.ID != hds.ID and bh.ID_DonVi = hds.ID_DonVi
		where (dt.TheoDoi is null or dt.TheoDoi = 0)
		and dt.LoaiDoiTuong = 1
		and bh.ID_DonVi = @ID_ChiNhanh
		and ((bh.NgayLapHoaDon >= @timeStart and bh.NgayLapHoaDon < @timeEnd) or (cs.NgayTao >= @timeStart and cs.NgayTao < @timeEnd))
		GROUP by dt.ID having COUNT(*) > 1
		)a
		-- tháng trước
		Union all
    	Select 
		0 as KhachHangTaoMoiThangNay,
    	0 as KhachHangQuayLaiThangNay,
    	Count (*) as KhachHangTaoMoiThangTruoc,
    	0 as KhachHangQuayLaiThangTruoc
		from 
		(select 
		dt.ID, COUNT(*) as solan
		from
		DM_DoiTuong dt 
		left join ChamSocKhachHangs cs on dt.ID = cs.ID_KhachHang
		left join BH_HoaDon bh on bh.ID_DoiTuong = dt.ID
		left join BH_HoaDon hds on bh.ID_DoiTuong = hds.ID_DoiTuong and bh.ID != hds.ID and bh.ID_DonVi = hds.ID_DonVi
		where (dt.TheoDoi is null or dt.TheoDoi = 0)
		and dt.LoaiDoiTuong = 1
		and bh.ID_DonVi = @ID_ChiNhanh
		and ((bh.NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and bh.NgayLapHoaDon < DateAdd(month, -1, @timeEnd) ) or (cs.NgayTao >= DateAdd(month, -1, @timeStart) and cs.NgayTao < DateAdd(month, -1, @timeEnd) ))
		and hds.ID is null
		GROUP by dt.ID having COUNT(*) = 1
		)a
		Union all
		Select 
		0 as KhachHangTaoMoiThangNay,
    	0 as KhachHangQuayLaiThangNay,
    	0 as KhachHangTaoMoiThangTruoc,
    	Count (*) as KhachHangQuayLaiThangTruoc
		from 
		(select 
		dt.ID, COUNT(*) as solan
		from
		DM_DoiTuong dt 
		left join ChamSocKhachHangs cs on dt.ID = cs.ID_KhachHang
		left join BH_HoaDon bh on bh.ID_DoiTuong = dt.ID
		left join BH_HoaDon hds on bh.ID_DoiTuong = hds.ID_DoiTuong and bh.ID != hds.ID and bh.ID_DonVi = hds.ID_DonVi
		where (dt.TheoDoi is null or dt.TheoDoi = 0)
		and dt.LoaiDoiTuong = 1
		and bh.ID_DonVi = @ID_ChiNhanh
		and ((bh.NgayLapHoaDon >= DateAdd(month, -1, @timeStart) and bh.NgayLapHoaDon < DateAdd(month, -1, @timeEnd) ) or (cs.NgayTao >= DateAdd(month, -1, @timeStart) and cs.NgayTao < DateAdd(month, -1, @timeEnd) ))
		GROUP by dt.ID having COUNT(*) > 1
		)a
    	) as k
    	) as m
END");
//            Sql(@"INSERT INTO DM_LoaiTuVanLichHen(ID, TenLoaiTuVanLichHen, TuVan_LichHen, NgayTao, NgaySua, NguoiTao, NguoiSua, TrangThai)
//SELECT ID, LoaiCongViec, '4', NgayTao, NgaySua, NguoiTao, NguoiSua, TrangThai FROM NS_CongViec_PhanLoai");

//            Sql(@"INSERT INTO ChamSocKhachHangs(ID, ID_KhachHang, ID_LoaiTuVan, ID_DonVi, Ma_TieuDe, PhanLoai, NgayGio,
//NgayGioKetThuc, NoiDung, TrangThai, NhacNho, MucDoPhanHoi, NgayTao, NgaySua, ID_NhanVien, ID_NhanVienQuanLy, NguoiTao, NguoiSua, ThoiGianHenLai,
//MucDoUuTien, KetQua, ID_LienHe)
//SELECT ID, ID_KhachHang, ID_LoaiCongViec, ID_DonVi, '', '4', ThoiGianTu, ThoiGianDen, NoiDung, TrangThai,
//NhacTruoc, '0', NgayTao, NgaySua, ID_NhanVienQuanLy, ID_NhanVienQuanLy, NguoiTao, NguoiSua, ThoiGianLienHeLai, '2', KetQuaCongViec, ID_LienHe FROM NS_CongViec");

//            Sql(@"INSERT INTO ChamSocKhachHang_NhanVien(ID, ID_ChamSocKhachHang, ID_NhanVien)
//SELECT NEWID(), ID, ID_NhanVienChiaSe FROM NS_CongViec WHERE ID_NhanVienChiaSe IS NOT NULL");
        }

        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_SoSanhCungKyThuChi]");
            DropStoredProcedure("[dbo].[getlist_SuKienToDay]");
            DropStoredProcedure("[dbo].[getList_TongQuanThuChi]");
        }
    }
}
