namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStoreProcedure_20180321_1639 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getlist_HoaDonTraHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaPT = p.String(),
                MaHD = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT 
	c.ID,
	c.ID_BangGia,
	c.ID_HoaDon,
	c.ID_ViTri,
	c.ID_DonVi,
	c.ID_NhanVien,
	c.ID_DoiTuong,
    c.ChoThanhToan,
	c.MaHoaDon,
	c.MaHoaDonGoc, 
	c.MaPhieuChi,
	c.NgayLapHoaDon,
	c.TenDoiTuong,
	c.NguoiTaoHD,
	c.TenDonVi,
	c.TenNhanVien,
	c.DienGiai,
	c.TenBangGia,
	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan,c.TongChiPhi, c.KhachDaTra,c.TongChietKhau,
	c.TrangThai
	FROM
	(
		select 
		a.ID as ID,
		bhhd.MaHoaDon,
		bhhd.ID_BangGia,
		bhhd.ID_HoaDon,
		bhhd.ID_ViTri,
		bhhd.ID_DonVi,
		bhhd.ID_NhanVien,
		bhhd.ID_DoiTuong,
		bhhd.ChoThanhToan,
		Case when hdb.MaHoaDon is null then '' else hdb.MaHoaDon end as MaHoaDonGoc,
		a.MaPhieuChi,
		bhhd.NgayLapHoaDon,
		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
		Case when dv.TenDonVi is null then N'' else dv.TenDonVi end as TenDonVi,
		Case when nv.TenNhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
		bhhd.DienGiai,
		bhhd.NguoiTao as NguoiTaoHD,
		Case when gb.TenGiaBan is null then N'Bảng giá chung' else gb.TenGiaBan end as TenBangGia,
		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
		CAST(ROUND(bhhd.TongChiPhi, 0) as float) as TongChiPhi,
		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
		a.KhachDaTra as KhachDaTra,
		bhhd.TongChietKhau,

		Case When bhhd.YeuCau = '4' then N'Đã hủy' else N'Hoàn thành' end as TrangThai
		FROM
		(
			Select 
			bhhd.ID,
			qhd.MaHoaDon as MaPhieuChi,
			ISNULL(hdct.Tienthu, 0) as KhachDaTra
			from BH_HoaDon bhhd
			left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
			left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID
			where bhhd.LoaiHoaDon = '6' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi = @ID_ChiNhanh
		) as a
		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
		left join BH_HoaDon hdb on bhhd.ID_HoaDon = hdb.ID
		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
		) as c
	WHERE MaHoaDon like @maPT
	and MaHoaDonGoc like @maHD
	and TrangThai like @TrangThai
	ORDER BY c.NgayLapHoaDon DESC");

            DropStoredProcedure("[dbo].[getList_HoaDonsTraHang]");

            CreateStoredProcedure(name: "[dbo].[Report_DatHang_GiaoDich]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaKH = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                LaHangHoa = p.String()
            }, body: @"SELECT 
	hh.ID as ID_HoaDon,
	hh.MaHoaDon,
	hh.NgayLapHoaDon,
	hh.TenKhachHang,
	hh.SoLuongDat, 
	hh.GiaTriDat,
	hh.ID_NhomHang,
	hh.ID_NhanVien
	FROM
	(
		SELECT
			hd.ID,
    		hd.MaHoaDon,
			hd.NgayLapHoaDon,
			Case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.MaDoiTuong end as MaDoiTuong,
			Case when dt.MaDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
			Case when dt.MaDoiTuong is null then 'khachle' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
			Case when dt.MaDoiTuong is null then 'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
    		a.SoLuongDat,
    		a.GiaTriDat,
    		a.ID_NhomHang,
    		hd.ID_NhanVien
    		FROM
    		(
    			SELECT
    			hd.ID as ID_HoaDon,
				dvqd.ID_HangHoa,
				hd.ID_DoiTuong,
				hh.ID_NhomHang,
    			SUM(ISNULL(hdct.SoLuong, 0)) as SoLuongDat,
    			SUM(ISNULL(hdct.ThanhTien, 0) * (1- ISNULL(hd.TongGiamGia / hd.TongTienHang, 0))) as GiaTriDat
    			FROM
    			BH_HoaDon_ChiTiet hdct
    			inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
    			inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    			inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ChoThanhToan = 0
    			and hd.ID_DonVi = @ID_ChiNhanh
    			and (hd.LoaiHoaDon = 3 and hd.yeucau != '4')
    			and hh.LaHangHoa like @LaHangHoa
    			and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
    			and (dt.MaDoiTuong like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.MaDoiTuong is null)
    			GROUP BY hd.MaHoaDon, hd.ID, dvqd.ID_HangHoa, hd.ID_DoiTuong, hh.ID_NhomHang
    		) a
			left join BH_HoaDon hd on a.ID_HoaDon = hd.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		) as hh
		where MaDoiTuong like @maKH or TenDoiTuong_KhongDau like @maKH or TenDoiTuong_ChuCaiDau like @maKH
		ORDER BY hh.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getListSQ_NhanVien]", parametersAction: p => new
            {
                ID_NhanVien = p.Guid()
            }, body: @"Select 
	MaHoaDon,
	NgayLapHoaDon, NguoiNopTien, 
	Case when loaihoadon =  '11' then N'Phiếu thu' else N'Phiếu chi' end as LoaiPhieu,
	CAST (Round(ISNULL(TongTienThu, 0), 0) as float) as TongTienThu
	from
	Quy_HoaDon
	where ID_NhanVien = @ID_NhanVien
	ORDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getList_LichSuKhuyenMai]", parametersAction: p => new
            {
                ID_KhuyenMai = p.Guid()
            }, body: @"Select 
	a.ID,
	a.MaHoaDon,
	a.NgayLapHoaDon, 
	a.TenNhanVien,
	Cast(round(a.DoanhThu, 0) as float) as DoanhThu,
	Cast(round(a.GiaTriKhuyenMai, 0) as float) as GiaTriKhuyenMai
	From
	(
		Select 
		bhhd.ID,
		bhhd.MaHoaDon,
		bhhd.NgayLapHoaDon,
		ns.TenNhanVien,
		bhhd.PhaiThanhToan as DoanhThu,
		bhhd.KhuyeMai_GiamGia as GiaTriKhuyenMai
		from 
		BH_HoaDon bhhd 
		left join NS_NhanVien ns on bhhd.ID_NhanVien = ns.ID
		--inner join DM_KhuyenMai dmkm on bhhd.ID_KhuyenMai = dmkm.ID
		where ID_KhuyenMai =  @ID_KhuyenMai and chothanhtoan = '0'

		union all
		Select 
		bhhd.ID,
		bhhd.MaHoaDon,
		bhhd.NgayLapHoaDon,
		ns.TenNhanVien,
		bhhd.PhaiThanhToan as DoanhThu,
		hdct.TienChietKhau as GiaTriKhuyenMai
		from
		BH_HoaDon bhhd 
		inner join BH_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDon
		inner join DM_KhuyenMai dmkm on hdct.ID_KhuyenMai = dmkm.ID
		left join NS_NhanVien ns on bhhd.ID_NhanVien = ns.ID
		where hdct.ID_KhuyenMai = @ID_KhuyenMai and chothanhtoan = '0'
	) a
	Order by NgayLapHoaDon DESC");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getlist_HoaDonTraHang]");
            DropStoredProcedure("[dbo].[Report_DatHang_GiaoDich]");
            DropStoredProcedure("[dbo].[getListSQ_NhanVien]");
            DropStoredProcedure("[dbo].[getList_LichSuKhuyenMai]");
        }
    }
}
