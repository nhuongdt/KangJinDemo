namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateStoreProcedure_20180315 : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[LoadFirstPageCountHH]", body: @"select dvqd.ID as ID_DonViQuiDoi from DonViQuiDoi dvqd 
    	left join dm_hanghoa hh on dvqd.ID_hangHoa = hh.ID
    	where dvqd.Xoa is null and dvqd.LaDonViChuan = 1 and hh.LaChaCungLoai =1 and hh.TheoDoi =1
    		group by dvqd.ID");

            AlterStoredProcedure(name: "[dbo].[Update_ChietKhau_YeuCauByID]", parametersAction: p => new
            {
                ID = p.Guid(),
                ChietKhau_YeuCau = p.Double(),
                LaPhanTram_YeuCau = p.Boolean()
            }, body: @"update ChietKhauMacDinh_NhanVien set ChietKhau_YeuCau = @ChietKhau_YeuCau, LaPhanTram_YeuCau = @LaPhanTram_YeuCau where ID = @ID");

            CreateStoredProcedure(name: "[dbo].[getlist_HoaDonBanHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaHD = p.String()
            }, body: @"SELECT 
	c.ID,
	c.ID_BangGia,
	c.ID_ViTri,
	c.ID_NhanVien,
	c.ID_DoiTuong,
	c.ChoThanhToan,
	c.MaHoaDon,
	c.MaHoaDonGoc,
	c.TongTienHDTra,
	c.NgayLapHoaDon,
	c.TenDoiTuong,
	c.Email,
	c.DienThoai,
	c.NguoiTaoHD,
	c.DiaChiKhachHang,
	c.KhuVuc,
	c.PhuongXa,
	c.TenDonVi,
	c.TenNhanVien,
	c.DienGiai,
	c.TenBangGia,
	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan, c.ThuTuThe, c.TienMat, c.ChuyenKhoan, c.KhachDaTra,
	c.TrangThai,
	c.KhuyenMai_GhiChu,
	c.KhuyeMai_GiamGia
	FROM
	(
		select 
		a.ID as ID,
		bhhd.ID_DoiTuong,
		bhhd.ID_NhanVien,
		bhhd.ChoThanhToan,
		bhhd.KhuyenMai_GhiChu,
		bhhd.KhuyeMai_GiamGia,
		Case when gb.ID is not null then gb.ID else N'00000000-0000-0000-0000-000000000000' end as ID_BangGia,
		Case when vt.ID is not null then vt.ID else N'00000000-0000-0000-0000-000000000000' end as ID_ViTri,
		bhhd.MaHoaDon,
		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
		bhhd.NgayLapHoaDon,
		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
		Case when dt.TenDoiTuong is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
		Case when dt.TenDoiTuong is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
		Case when dt.Email is null then N'' else dt.Email end as Email,
		Case when dt.DienThoai is null then N'' else dt.DienThoai end as DienThoai,
		Case when dt.DiaChi is null then N'' else dt.DiaChi end as DiaChiKhachHang,
		Case when tt.TenTinhThanh is null then tt.TenTinhThanh else N'' end as KhuVuc,
		Case when qh.TenQuanHuyen is null then qh.TenQuanHuyen else N'' end as PhuongXa,
		Case when dv.TenDonVi is null then N'' else dv.TenDonVi end as TenDonVi,
		Case when nv.TenNhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
		bhhd.DienGiai,
		bhhd.NguoiTao as NguoiTaoHD,
		Case when gb.TenGiaBan is null then N'Bảng giá chung' else gb.TenGiaBan end as TenBangGia,
		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
		CAST(ROUND(ISNULL(hdt.PhaiThanhToan,0),0) as float) as TongTienHDTra,
		a.ThuTuThe,
		a.TienMat,
		a.ChuyenKhoan,
		a.KhachDaTra as KhachDaTra,
		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai
		FROM
		(
			Select 
			b.ID,
			SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
			SUM(ISNULL(b.TienMat, 0)) as TienMat,
			SUM(ISNULL(b.TienGui, 0)) as ChuyenKhoan,
			SUM(ISNULL(b.TienThu, 0)) as KhachDaTra
			from
			(
				Select 
				bhhd.ID,
				Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end  as TienMat,
				Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienGui, 0) else ISNULL(hdct.TienGui, 0) * (-1) end  as TienGui,
				Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end  as ThuTuThe,
				Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end  as TienThu
				from BH_HoaDon bhhd
				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID 
				where bhhd.LoaiHoaDon = '1' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi = @ID_ChiNhanh

				Union all
				Select
					d.ID,
					Case when RowNumber = 1 then d.TienMat else 0 end as TienMat,
					Case when RowNumber = 1 then d.TienGui else 0 end as TienGui,
					Case when RowNumber = 1 then d.ThuTuThe else 0 end as ThuTuThe,
					Case when RowNumber = 1 then d.TienThu else 0 end as TienThu
				FROM
				(
					SELECT ROW_NUMBER() Over(PARTITION BY ID_DatHang ORDER BY f.NgayLapHoaDon)
					As RowNumber,* FROM 
					(
						Select
						bhhd.ID,
						bhhd.NgayLapHoaDon,
						hdt.ID as ID_DatHang,
						Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end  as TienMat,
						Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienGui, 0) else ISNULL(hdct.TienGui, 0) * (-1) end  as TienGui,
						Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end  as ThuTuThe,
						Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end  as TienThu
						from BH_HoaDon bhhd
						inner join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
						left join Quy_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDonLienQuan
						left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID
						where hdt.LoaiHoaDon = '3' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi = @ID_ChiNhanh
					) f
				) d
			) b
			group by b.ID 
		) as a
		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
		left join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
		) as c
	WHERE MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD or DienThoai like @maHD
	ORDER BY c.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[getlist_HoaDonDatHang]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid(),
                MaHD = p.String()
            }, body: @"SELECT 
	c.ID,
	c.MaHoaDon,
	c.NgayLapHoaDon,
	c.TenDoiTuong,
	c.Email,
	c.DienThoai,
	c.ID_NhanVien,
	c.ID_DoiTuong,
	c.ID_BangGia,
	c.YeuCau,
	c.NguoiTaoHD,
	c.DiaChiKhachHang,
	c.KhuVuc,
	c.PhuongXa,
	c.TenDonVi,
	c.TenNhanVien,
	c.DienGiai,
	c.TenBangGia,
	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan, c.KhachDaTra,
	c.TrangThai
	FROM
	(
		select 
		a.ID as ID,
		bhhd.MaHoaDon,
		bhhd.ID_NhanVien,
		bhhd.ID_DoiTuong,
		bhhd.ID_BangGia,
		bhhd.NgayLapHoaDon,
		bhhd.YeuCau,
		Case when dt.TenDoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
		Case when dt.TenDoiTuong is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenDoiTuong_KhongDau,
		Case when dt.TenDoiTuong is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenDoiTuong_ChuCaiDau,
		Case when dt.Email is null then N'' else dt.Email end as Email,
		Case when dt.DienThoai is null then N'' else dt.DienThoai end as DienThoai,
		Case when dt.DiaChi is null then N'' else dt.DiaChi end as DiaChiKhachHang,
		Case when tt.TenTinhThanh is null then tt.TenTinhThanh else N'' end as KhuVuc,
		Case when qh.TenQuanHuyen is null then qh.TenQuanHuyen else N'' end as PhuongXa,
		Case when dv.TenDonVi is null then N'' else dv.TenDonVi end as TenDonVi,
		Case when nv.TenNhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
		bhhd.DienGiai,
		bhhd.NguoiTao as NguoiTaoHD,
		Case when gb.TenGiaBan is null then N'Bảng giá chung' else gb.TenGiaBan end as TenBangGia,
		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
		a.KhachDaTra as KhachDaTra,
		Case When bhhd.YeuCau = '1' then N'Phiếu tạm' when bhhd.YeuCau = '3' then N'Hoàn thành' when bhhd.YeuCau = '2' then N'Đang giao hàng' else N'Đã hủy' end as TrangThai
		FROM
		(
			select 
			b.ID,
			SUM(ISNULL(b.KhachDaTra, 0)) as KhachDaTra
			from
			(
				Select 
				bhhd.ID,
				ISNULL(hdct.Tienthu, 0) as KhachDaTra
				from BH_HoaDon bhhd
				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
				where bhhd.LoaiHoaDon = '3' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi = @ID_ChiNhanh

				union all
				Select
				hdt.ID,
				Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end  as KhachDaTra
				from BH_HoaDon bhhd
				inner join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID
				where hdt.LoaiHoaDon = '3' and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi = @ID_ChiNhanh
			) b
			group by b.ID 
		) as a
		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
		) as c
	WHERE MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD or DienThoai like @maHD
	ORDER BY c.NgayLapHoaDon DESC");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getlist_HoaDonBanHang]");
            DropStoredProcedure("[dbo].[getlist_HoaDonDatHang]");
        }
    }
}