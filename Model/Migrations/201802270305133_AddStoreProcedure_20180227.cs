namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStoreProcedure_20180227 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getList_XuatHuy]", parametersAction: p => new
            {
                maXH = p.String(),
                maHH = p.String(),
                TenNV = p.String(),
                TenNguoiTao = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                TrangThai1 = p.String(),
                TrangThai2 = p.String(),
                TrangThai3 = p.String()
            }, body: @"SELECT *
	FROM
	(
		SELECT DISTINCT
		hd.ID,
		hd.ID_NhanVien as ID_NhanVien,
		hd.ID_DonVi,
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		dv.TenDonVi as TenChiNhanh,
		CAST(ROUND(ISNULL(hd.TongTienHang, 0), 0) as float) as TongTienHang,
		hd.DienGiai,
		hd.YeuCau,
		hd.NguoiTao,
		Case when nv.TenNhanVien != '' then nv.TenNhanVien else '' end as TenNhanVien,
		hd.ChoThanhToan
		--hd.ID_DoiTuong
		FROM
		BH_HoaDon hd
		join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
		join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		join DM_DonVi dv on hd.ID_DonVi = dv.ID
		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		where hd.loaiHoaDon = 8
		and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and (hd.YeuCau like @TrangThai1 or hd.YeuCau like @TrangThai2 or hd.YeuCau like @TrangThai3)
		and hd.MaHoaDon like @maXH
		and hd.NguoiTao like @TenNguoiTao
		and (dvqd.MaHangHoa like @maHH or hh.TenHangHoa_KhongDau like @maHH or hh.TenHangHoa_KyTuDau like @maHH)
	) a
	where a.TenNhanVien like @TenNV
	ORDER BY a.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getList_ChuongTrinhKhuyenMai]", parametersAction: p => new
            {
                maKM = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT 
	km.ID,
	ad.ID_DonVi,
	km.MaKhuyenMai,
	km.TenKhuyenMai,
	km.GhiChu,
	Case when km.TrangThai = 1 then N'Kích hoạt' else N'Chưa áp dụng' end as TrangThai,
	Case when km.HinhThuc = 11 then N'Hóa đơn - Giảm giá hóa đơn' when km.HinhThuc = 12 then N'Hóa đơn - Tặng hàng' when  km.HinhThuc = 13 then N'Hóa đơn - Giảm giá hàng' when km.HinhThuc = 14 then N'Hóa đơn - Tặng Điểm'
	when km.HinhThuc = 21 then N'Hàng hóa - Mua hàng giảm giá hàng' when km.HinhThuc = 22 then N'Hàng hóa - Mua hàng tặng hàng' when  km.HinhThuc = 13 then N'Hàng hóa - Mua hàng tặng điểm' else N'Hàng hóa - Mua hàng giảm giá theo số lượng mua'  end as HinhThuc,
	km.LoaiKhuyenMai,
	km.HinhThuc as KieuHinhThuc,
	km.ThoiGianBatDau,
	km.ThoiGianKetThuc,
	Case when km.NgayApDung = '' then '' else N'Ngày ' + Replace(km.NgayApDung, '_', N', Ngày ') end as NgayApDung,
	Case when km.ThangApDung = '' then '' else N'Tháng ' + Replace(km.ThangApDung, '_', N', Tháng ') end as ThangApDung,
	Replace(Case when km.ThuApDung = '' then '' else N'Thứ ' + Replace(km.ThuApDung, '_', N', Thứ ') end, N'Thứ 8',N'Chủ nhật') as ThuApDung,
	Case when km.GioApDung = '' then '' else Replace(km.GioApDung, '_', N', ') end as GioApDung,
	Case when km.ApDungNgaySinhNhat = 1 then N'Áp dụng vào ngày sinh nhật khách hàng' when km.ApDungNgaySinhNhat = 2 then N'Áp dụng vào tuần sinh nhật khách hàng'
	when km.ApDungNgaySinhNhat = 3 then N'Áp dụng vào tháng sinh nhật khách hàng' else '' end as ApDungNgaySinhNhat,
	km.ApDungNgaySinhNhat as ValueApDungSN,
	km.TatCaDoiTuong,
	km.TatCaDonVi,
	km.TatCaNhanVien,
	km.NguoiTao
	FROM 
	DM_KhuyenMai km
	left join DM_KhuyenMai_ApDung ad on km.ID = ad.ID_KHuyenMai
	where (km.MaKhuyenMai like @maKM or km.TenKhuyenMai like @maKM)
	and km.TrangThai like @TrangThai
	ORDER BY km.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[GetListDoiTuongByLoai]", parametersAction: p => new
            {
                LoaiDoiTuong = p.Int(),
                Search = p.String()
            }, body: @"select TOP(20) ID, TenDoiTuong as NguoiNopTien from DM_DoiTuong where LoaiDoiTuong = @LoaiDoiTuong and TenDoiTuong like @Search");

            CreateStoredProcedure(name: "[dbo].[GetListNhanVienAddSoQuy]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                Search = p.String()
            }, body: @"select TOP(20) nv.ID, nv.TenNhanVien as NguoiNopTien 
	from NS_NhanVien nv
	left join NS_QuaTrinhCongTac qtct on nv.ID = qtct.ID_NhanVien
	where qtct.ID_DonVi = @ID_ChiNhanh and nv.TenNhanVien like @Search");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_XuatHuy]");
            DropStoredProcedure("[dbo].[getList_ChuongTrinhKhuyenMai]");
            DropStoredProcedure("[dbo].[GetListDoiTuongByLoai]");
            DropStoredProcedure("[dbo].[GetListNhanVienAddSoQuy]");
        }
    }
}
