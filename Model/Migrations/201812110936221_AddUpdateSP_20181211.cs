namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181211 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_DaoTao]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.TuNgay, a.DenNgay, a.NoiHoc, a.NganhHoc, a.HeDaoTao, a.BangCap,
	a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	dt.TuNgay, dt.DenNgay, dt.NoiHoc, dt.NganhHoc, dt.HeDaoTao, dt.BangCap,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	inner join NS_NhanVien_DaoTao dt on nv.ID = dt.ID_NhanVien
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and dt.TrangThai = 1
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao, a.TuNgay DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_KhenThuong]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.HinhThuc, a.SoQuyetDinh,a.NoiDung, a.NgayBanHang, a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	kt.HinhThuc,kt.SoQuyetDinh, kt.NgayBanHang, kt.NoiDung,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	inner join NS_KhenThuong kt on nv.ID = kt.ID_NhanVien
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and kt.TrangThai = 1
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao, a.NgayBanHang DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_LuongPhuCap]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.TenLoaiLuong, a.NgayApDung, a.NgayKetThuc,a.SoTien, a.HeSo, a.Bac, a.NoiDung, a.TrangThaiLuong,
	a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	ll.TenLoaiLuong, pc.NgayApDung, pc.NgayKetThuc,
	CAST(ROUND(pc.SoTien, 0) as float) as SoTien, 
	CAST(ROUND(pc.HeSo, 3) as float) as HeSo, pc.Bac, pc.NoiDung,
	Case when pc.TrangThai = 0 then N'Đã hủy' 
		when pc.TrangThai = 1 then N'Áp dụng'
		else '' end TrangThaiLuong,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	inner join NS_Luong_PhuCap pc on nv.ID = pc.ID_NhanVien
	inner join NS_LoaiLuong ll on pc.ID_LoaiLuong = ll.ID
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao, a.NgayApDung DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_MienGiamThue]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.KhoanMienGiam, a.NgayApDung, a.NgayKetThuc, a.SoTien, a.GhiChu,
	a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	gt.KhoanMienGiam, gt.NgayApDung, gt.NgayKetThuc, 
	CAST(ROUND(gt.SoTien, 0) as float) as SoTien, 
	gt.GhiChu,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	inner join NS_MienGiamThue gt on nv.ID = gt.ID_NhanVien
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and gt.TrangThai = 1
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao, a.NgayApDung DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_QuaTrinhCongTac]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.TuNgay, a.DenNgay, a.CoQuan, a.ViTri, a.DiaChi,
	a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	congtac.TuNgay, congtac.DenNgay, congtac.CoQuan, congtac.ViTri, congtac.DiaChi,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	inner join NS_NhanVien_CongTac congtac on nv.ID = congtac.ID_NhanVien
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and congtac.TrangThai = 1
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao, a.TuNgay DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_TheoBaoHiem]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.TenLoaiBaoHiem as LoaiBaoHiem, a.SoBaoHiem, a.NgayCap, a.NgayHetHan,a.NoiBaoHiem, a.TrangThaiBaoHiem, a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when bh.LoaiBaoHiem = 0 then N'Bảo hiểm xã hội'
		when bh.LoaiBaoHiem = 1 then N'Bảo hiểm y tế'
		else N'Bảo hiểm thất nghiệp' end as TenLoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	bh.SoBaoHiem,
	bh.NgayCap, bh.NgayHetHan, bh.NoiBaoHiem,
	Case when bh.TrangThai = 0 then N'Đã xóa' 
		when bh.TrangThai = 1 then N'Áp dụng'
		else '' end TrangThaiBaoHiem,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	inner join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao, a.NgayCap DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_TheoDoTuoi]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String(),
                Min = p.Double(),
                Max = p.Double()
            }, body: @"SELECT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	Case when a.Tuoi is null then N'Không xác định' else Convert (varchar(10), a.Tuoi) end as Tuoi,
	a.GhiChu, a.TrangThaiNV
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	DATEDIFF(year, nv.NgaySinh, GETDATE()) as Tuoi,
	nv.GhiChu,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.DaNghiViec = 0 then N'Đang làm việc' else N'Đã nghỉ việc' end as TrangThaiNV,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	and a.Tuoi is not null
	and a.Tuoi >= @Min and a.Tuoi <= @Max
	GROUP by a.MaNhanVien, a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan, a.Tuoi, a.GhiChu, a.TrangThaiNV
	order by a.Tuoi DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_TheoHopDong]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.TenLoaiHopDong as LoaiHopDong, a.SoHopDong, a.NgayKy, a.ThoiHan, a.TrangThaiHopDong, a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.LoaiHopDong = 0 then N'Xác định thời gian'
		when hd.LoaiHopDong = 1 then N'Không xác định thời gian'
		when hd.LoaiHopDong = 2 then N'Thử việc'
		when hd.LoaiHopDong = 3 then N'Học việc'
		else N'Cộng tác viên' end as TenLoaiHopDong,
	hd.LoaiHopDong,
	hd.SoHopDong,
	hd.NgayKy,
	Case when hd.DonViThoiHan = 1 then CONVERT(varchar(10), hd.ThoiHan) + N' Năm'
	else CONVERT(varchar(10), hd.ThoiHan) + N' Tháng' end as ThoiHan,
	Case when hd.TrangThai = 0 then N'Đã xóa' 
		when hd.TrangThai = 1 then N'Áp dụng'
		else '' end TrangThaiHopDong,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	left join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	inner join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao, a.NgayKy DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_ThongTinGiaDinh]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.HoTen, a.NgaySinhGD, a.QuanHe, a.DiaChi,
	a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	gd.HoTen, 
	Case when LEN(gd.NgaySinh) = 4 then CAST(ROUND(gd.NgaySinh,0) as varchar) 
		when LEN(gd.NgaySinh) = 6 then  right(gd.NgaySinh, 2) + '/' + LEFT(gd.NgaySinh, 4) 
		when LEN(gd.NgaySinh) = 8 then CONVERT(varchar, Convert(DATETIME, LEFT(gd.NgaySinh, 8)), 103) 
		else '' end as NgaySinhGD,
	gd.QuanHe, gd.DiaChi,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	inner join NS_NhanVien_GiaDinh gd on nv.ID = gd.ID_NhanVien
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	and gd.TrangThai = 1
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai != 0
	order by a.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_ThongTinSucKhoe]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT
	a.MaNhanVien,a.TenNhanVien, a.GioiTinh, a.NgaySinh, a.TenPhongBan,
	a.NgayKham,
	CAST(ROUND(a.ChieuCao,3) as float) as ChieuCao,
	CAST(ROUND(a.CanNang,3) as float) as CanNang,
	 --a.ChieuCao, a.CanNang, 
	 a.TinhHinhSucKhoe,
	a.NgayTao
	FROM
	(
	SELECT nv.MaNhanVien, nv.TenNhanVien, 
	CASe when nv.GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh,
	nv.NgaySinh,
	nv.NgayTao,
	nv.DienThoaiDiDong,
	nv.NgayVaoLamViec, pb.TenPhongBan,
	Case when nv.DanTocTonGiao is null then '' else nv.DanTocTonGiao end as DanTocTonGiao,
	case when ct.ID_PhongBan is null then NEWID() else ct.ID_PhongBan end as ID_PhongBan,
	Case when nv.NgayVaoDoan is not null then 0 else 3 end as LoaiVaoDoan,
	Case when nv.NgayVaoDang is not null then 1 else 3 end as LoaiVaoDang,
	Case when nv.NgayNhapNgu is not null then 2 else 3 end as LoaiNhapNgu,
	Case when bh.ID is null then 3 else bh.LoaiBaoHiem end as LoaiBaoHiem,
	Case when hd.ID is null then 5 else hd.LoaiHopDong end as LoaiHopDong,
	sk.NgayKham, sk.ChieuCao, sk.CanNang,
	--CAST(ROUND(sk.ChieuCao,3) as float) as ChieuCao,
	--CAST(ROUND(sk.CanNang,3) as float) as CanNang,
	sk.TinhHinhSucKhoe,
	Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
	Case when nv.NgaySinh is not null then nv.NgaySinh else '2016-07-04' end as NgaySinhNhat
	FROM
	NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_BaoHiem bh on bh.ID_NhanVien = nv.ID
	left join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	left join NS_HopDong hd on hd.ID_NhanVien = nv.ID
	inner join NS_NhanVien_SucKhoe sk on nv.ID = sk.ID_NhanVien
	where nv.NgayTao >= @timeCreate_start and nv.NgayTao < @timeCreate_end
	and nv.GioiTinh like @GioiTinh 
	and nv.DaNghiViec like @TrangThai
	and ct.ID_DonVi = @ID_ChiNhanh
	and ct.LaDonViHienThoi = '1'
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	and sk.TrangThai = 1
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai != 0
	order by a.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoNhanVien_TongHop]", parametersAction: p => new
            {
                MaNV = p.String(),
                MaNV_TV = p.String(),
                ID_ChiNhanh = p.Guid(),
                timeCreate_start = p.DateTime(),
                timeCreate_end = p.DateTime(),
                ID_PhongBan = p.String(),
                ID_PhongBan_SP = p.String(),
                GioiTinh = p.String(),
                LoaiHopDong = p.String(),
                timeBirthday_start = p.DateTime(),
                timeBirthday_end = p.DateTime(),
                LoaiChinhTri = p.String(),
                LoaiBaoHiem = p.String(),
                LoaiDanToc = p.String(),
                LoaiDanToc_SP = p.String(),
                TrangThai = p.String()
            }, body: @"SELECT DISTINCT 
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
	and ct.LaDonViHienThoi = '1'
	and (nv.TenNhanVienChuCaiDau like @MaNV or nv.TenNhanVienKhongDau like @MaNV or nv.MaNhanVien like @MaNV or nv.MaNhanVien like @MaNV_TV or DienThoaiDiDong like @MaNV_TV)
	
	) as a
	where (a.ID_PhongBan in (select * from splitstring(@ID_PhongBan_SP)) or a.ID_PhongBan like @ID_PhongBan)
	and a.NgaySinhNhat >= @timeBirthday_start and a.NgaySinhNhat < @timeBirthday_end
	and (a.LoaiVaoDoan in (select * from splitstring(@LoaiChinhTri)) or a.LoaiVaoDang in (select * from splitstring(@LoaiChinhTri)) or a.LoaiNhapNgu in (select * from splitstring(@LoaiChinhTri)))
	and a.LoaiBaoHiem in (select * from splitstring(@LoaiBaoHiem))
	and a.LoaiHopDong in (select * from splitstring(@LoaiHopDong))
	and (a.DanTocTonGiao in (select * from splitstring(@LoaiDanToc_SP)) or a.DanTocTonGiao like @LoaiDanToc)
	and a.TrangThai !=0
	order by a.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[CapNhatPhongBanMacDinhKhiXoa]", parametersAction: p => new
            {
                PhongBanId = p.Guid()
            }, body: @"SET NOCOUNT ON;  
	DECLARE @pPhongBanMacDinhId uniqueidentifier
	set @pPhongBanMacDinhId=(SELECT TOP 1 ID FROM NS_PhongBan where ID_DonVi is null );

	DECLARE @TablePhongBan TABLE( Name uniqueidentifier)
	INSERT INTO @TablePhongBan(Name) select  ID from NS_PhongBan where ID_PhongBanCha =@PhongBanId or ID =@PhongBanId;

	DECLARE @TablePhongBanNew TABLE( Name uniqueidentifier)
	INSERT INTO @TablePhongBanNew(Name) select  ID from NS_PhongBan where ID_PhongBanCha in (select Name from @TablePhongBan) or ID in (select Name from @TablePhongBan);
	
	update NS_PhongBan set TrangThai =0 where ID in  (select Name from @TablePhongBanNew);
	Update NS_QuaTrinhCongTac set ID_PhongBan=@pPhongBanMacDinhId where ID_PhongBan in  (select Name from @TablePhongBanNew) ;");

            CreateStoredProcedure(name: "[dbo].[SP_GetInforBasic_DoiTuongByID]", parametersAction: p => new
            {
                ID_DoiTuong = p.String()
            }, body: @"select dt.*,
		ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
		ISNULL(tt.TenTinhThanh,'') as KhuVuc,
		ISNULL(qh.TenQuanHuyen,'') as PhuongXa,
		ISNULL(nk.TenNguonKhach,'') as TenNguonKhach,
		ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
		nhoms.TenNhomDoiTuongs
	from DM_DoiTuong dt
	left join DM_TinhThanh tt on  dt.ID_TinhThanh = tt.ID
	left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
	left join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt.ID
	left join DM_NguonKhachHang nk on dt.ID_NguonKhach = dt.ID_NguonKhach
	left join (
		Select Main.ID_DoiTuong,
			Left(Main.doituong_nhom,Len(Main.doituong_nhom)-1) As TenNhomDoiTuongs
			From
				(
					Select distinct dtnOut.ID_DoiTuong, 
						(
							Select ndt.TenNhomDoiTuong + ',' AS [text()]
							From dbo.DM_DoiTuong_Nhom dtn
							left join dbo.DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
							Where  dtn.ID_DoiTuong like  @ID_DoiTuong
							For XML PATH ('')
						) doituong_nhom
					From dbo.DM_DoiTuong_Nhom dtnOut
				) [Main] 
		) nhoms on dt.ID= nhoms.ID_DoiTuong
	where dt.ID like @ID_DoiTuong");

            Sql(@"ALTER PROCEDURE [dbo].[getListNhanVien_allDonVi]
    @ID_ChiNhanh [nvarchar](max)
AS
BEGIN
    Select 
    	nv.ID,
    	nv.MaNhanVien,
    nv.TenNhanVien
    	From NS_NhanVien nv
    	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
    	where ct.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh)) 
		and (nv.TrangThai is null or nv.TrangThai = 1) and nv.DaNghiViec = 0
    	GROUP by nv.ID, nv.MaNhanVien, nv.TenNhanVien
END");

            Sql(@"ALTER PROCEDURE [dbo].[SelectDanhSachNhanVien]
    @donviID [nvarchar](max),
    @phongban [nvarchar](max),
    @dantoc [nvarchar](max),
    @HK_TT [nvarchar](max),
    @HK_QH [nvarchar](max),
    @HK_XP [nvarchar](max),
    @TT_TT [nvarchar](max),
    @TT_QH [nvarchar](max),
    @TT_XP [nvarchar](max),
    @GioiTinh [int],
    @TrangThai [int],
    @ChinhTri [int],
    @StartDate [nvarchar](max),
    @EndDate [nvarchar](max),
    @LoaiHopDong [nvarchar](max),
    @ListBaoHiem [nvarchar](max),
    @Text [nvarchar](max)
AS
BEGIN
		DECLARE @donvi_id uniqueidentifier 
    	DECLARE @hktt_id uniqueidentifier 
    	DECLARE @hkqh_id uniqueidentifier 
    	DECLARE @hkxp_id uniqueidentifier 
    	DECLARE @tttt_id uniqueidentifier 
    	DECLARE @ttqh_id uniqueidentifier 
    	DECLARE @ttxp_id uniqueidentifier 
    	DECLARE @tungay date 
    	DECLARE @denngay date 
    
    	DECLARE @TablePhongBan TABLE( Name [nvarchar](max))
    	INSERT INTO @TablePhongBan(Name) select  Name from [dbo].[splitstring](@phongban+',') where Name!='';
    
    	DECLARE @TableDanToc TABLE( Name [nvarchar](max))
    	DECLARE @countdantoc int
    	INSERT INTO @TableDanToc(Name) select  Name from [dbo].[splitstring](@dantoc+',') where Name!='';
		Select @countdantoc =  (Select count(*) from @TableDanToc);
    
    	DECLARE @TableBaoHiem TABLE( Name int)
    	DECLARE @countbaohiem int
    	INSERT INTO @TableBaoHiem(Name) select  Name from [dbo].[splitstring](@ListBaoHiem+',') where Name!='';
		Select @countbaohiem =  (Select count(*) from @TableBaoHiem);
    
    	DECLARE @TableHopDong TABLE( Name int)
    	DECLARE @counthopdong int
    	INSERT INTO @TableHopDong(Name) select  Name from [dbo].[splitstring](@LoaiHopDong+',') where Name!='';
		Select @counthopdong =  (Select count(*) from @TableHopDong);
    
    	 IF(LEN(ISNULL(@StartDate, '')) >0)
    	 set @tungay=CONVERT(datetime, @StartDate, 103)
    	 set @denngay=CONVERT(datetime, @EndDate, 103)
    
    	IF(LEN(ISNULL(@donviId, '')) >0)
    		SET @donvi_id = CONVERT(uniqueidentifier, @donviId)
    
    	IF(LEN(ISNULL(@HK_TT, ''))>0)
    		SET @hktt_id = CONVERT(uniqueidentifier, @HK_TT)
    
    	IF(LEN(ISNULL(@HK_QH, ''))>0)
    		SET @hkqh_id = CONVERT(uniqueidentifier, @HK_QH)
    
    	IF(LEN(ISNULL(@HK_XP, ''))>0)
    		SET @hkxp_id = CONVERT(uniqueidentifier, @HK_XP)
    
    	IF(LEN(ISNULL(@TT_TT, ''))>0)
    		SET @tttt_id = CONVERT(uniqueidentifier, @TT_TT)
    
    	IF(LEN(ISNULL(@TT_QH, ''))>0)
    		SET @ttqh_id = CONVERT(uniqueidentifier, @TT_QH)
    
    	IF(LEN(ISNULL(@TT_XP, ''))>0)
    		SET @ttxp_id = CONVERT(uniqueidentifier, @TT_XP)
    
    SET NOCOUNT ON;  
    select 
    f1.MaNhanVien,
    			f1.TenNhanVien,
    			CASE 
    				WHEN f1.NgaySinh is not null
    				THEN convert(varchar, f1.NgaySinh, 103)
    				ELSE '' 
    			 END as NgaySinh,
    			f1.GioiTinh,
    			f1.NoiSinh,
    			f1.DienThoaiDiDong,
    			f1.Email,
    			f1.SoCMND,
    			CASE 
    				WHEN f1.NgayCap is not null
    				THEN convert(varchar, f1.NgayCap, 103)
    				ELSE '' 
    			 END as NgayCap,
    			f1.NoiCap,
    			f1.DanTocTonGiao,
    			f1.TonGiao,
    			f1.TinhTrangHonNhan,
    			qg.TenQuocGia,
    			f1.NguyenQuan as DiaChiHKTT,
    			xp1.TenXaPhuong as HK_TenXaPhuong, 
    			qh1.TenQuanHuyen as HK_TenQuanHuyen,
    			tt1.TenTinhThanh as HK_TenTinhThanh,
    			f1.ThuongTru as DiaChiTT,
    			xp2.TenXaPhuong as TT_TenXaPhuong,
    			qh2.TenQuanHuyen as TT_TenQuanHuyen,
    			tt2.TenTinhThanh as TT_TenTinhThanh,
    			fpb.TenPhongBan,
    			CASE 
    				WHEN f1.NgayVaoLamViec is not null
    				THEN convert(varchar, f1.NgayVaoLamViec, 103)
    				ELSE '' 
    			 END as NgayVaoLamViec,
    			 f1.DaNghiViec,
    			CASE 
    				WHEN f1.NgayVaoDoan is not null
    				THEN convert(varchar, f1.NgayVaoDoan, 103)
    				ELSE '' 
    			 END as NgayVaoDoan,
    			f1.NoiVaoDoan,
    			CASE 
    				WHEN f1.NgayNhapNgu is not null
    				THEN convert(varchar, f1.NgayNhapNgu, 103)
    				ELSE '' 
    			 END as NgayNhapNgu,
    			CASE 
    				WHEN f1.NgayXuatNgu is not null
    				THEN convert(varchar, f1.NgayXuatNgu, 103)
    				ELSE '' 
    			 END as NgayXuatNgu,
    			CASE 
    				WHEN f1.NgayVaoDang is not null
    				THEN convert(varchar, f1.NgayVaoDang, 103)
    				ELSE '' 
    			 END as NgayVaoDang,
    			 CASE 
    				WHEN f1.NgayVaoDangChinhThuc is not null
    				THEN convert(varchar, f1.NgayVaoDangChinhThuc, 103)
    				ELSE '' 
    			 END as NgayVaoDangChinhThuc,
    			  CASE 
    				WHEN f1.NgayRoiDang is not null
    				THEN convert(varchar, f1.NgayRoiDang, 103)
    				ELSE '' 
    			 END as NgayRoiDang,
    			f1.NoiSinhHoatDang,
    			f1.GhiChuThongTinChinhTri
    
    from (
    SELECT DISTINCT nv.*
    FROM NS_NhanVien nv
	join NS_QuaTrinhCongTac qtct
	on nv.ID = qtct.ID_NhanVien
    	where (nv.TrangThai not in(0) or nv.TrangThai is null)
    							and ((LEN(ISNULL(@phongban, '')) = 0 and qtct.ID_DonVi=@donvi_id)
    								or qtct.ID_PhongBan in (select pb.Name from @TablePhongBan pb))
    							and (LEN(ISNULL(@HK_TT, '')) = 0 or nv.ID_TinhThanhHKTT =@hktt_id)
    							and (LEN(ISNULL(@HK_QH, '')) = 0 or nv.ID_QuanHuyenHKTT =@hkqh_id)  
    							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_XaPhuongHKTT =@hkxp_id)
    							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_TinhThanhHKTT =@tttt_id)
    							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_QuanHuyenTT =@ttqh_id)
    							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_XaPhuongTT =@ttxp_id)
    							and (LEN(ISNULL(@Text, '')) = 0 or nv.MaNhanVien like N'%'+@Text+'%' or nv.TenNhanVien like N'%'+@Text+'%' or nv.TenNhanVienChuCaiDau like N'%'+@Text+'%' or nv.TenNhanVienKhongDau like N'%'+@Text+'%')
    							and(@countdantoc=0 or (select count(*) from @TableDanToc dt where dt.Name like N'%'+nv.DanTocTonGiao+'%')>0)
    							and (@GioiTinh=2 or @GioiTinh=nv.GioiTinh)
    							and (@TrangThai=2 or @TrangThai=nv.DaNghiViec)
    							and (LEN(ISNULL(@StartDate, ''))=0 or(nv.NgaySinh>=@tungay and nv.NgaySinh<=@denngay))
    							and(@counthopdong=0 or nv.id in (select hd.ID_NhanVien from NS_HopDong hd where hd.LoaiHopDong in (select Name from @TableHopDong)))  
    							and(@countbaohiem=0 or nv.id in (select bh.ID_NhanVien from NS_BaoHiem bh where bh.LoaiBaoHiem in (select Name from @TableBaoHiem)))  
    							and ((@ChinhTri=0 and nv.NgayVaoDoan is not null) 
    								or (@ChinhTri=1 and nv.NgayVaoDang is not null and  nv.NgayVaoDang is not null and nv.NgayRoiDang is null)
    								or (@ChinhTri =2 and nv.NgayNhapNgu is not null)
    								or @ChinhTri=-1)
									
									) f1

    			left join DM_QuocGia qg ON qg.ID = f1.ID_QuocGia
    			left join DM_TinhThanh  tt1 ON tt1.ID = f1.ID_TinhThanhHKTT  
    			left join DM_TinhThanh  tt2 ON tt2.ID = f1.ID_TinhThanhTT  
    			left join DM_QuanHuyen  qh1 ON qh1.ID = f1.ID_QuanHuyenHKTT
    			left join DM_QuanHuyen  qh2 ON qh2.ID = f1.ID_QuanHuyenTT
    			left join DM_XaPhuong  xp1 ON xp1.ID = f1.ID_XaPhuongHKTT
    			left join DM_XaPhuong  xp2 ON xp2.ID = f1.ID_XaPhuongTT 
    			left join NS_PhongBan  fpb ON fpb.ID = f1.ID_NSPhongBan  
    			order by f1.NgayTao desc;  
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetAll_UserContact_Where]
    @txtSearch [nvarchar](max)
AS
BEGIN
    DECLARE @where AS nvarchar (max)=N''
    
    IF @txtSearch !=''
    	SET @where = ' WHERE '+ @txtSearch
    ELSE
    	SET @where = ' WHERE MaLienHe LIKE ''%%'''
    
    DECLARE @sqlExc nvarchar(max) = 'SELECT DM_LienHe.ID, DM_LienHe.ID_DoiTuong, MaLienHe,TenLienHe, DM_LienHe.ID_TinhThanh, DM_LienHe.ID_QuanHuyen,DM_LienHe.SoDienThoai,DM_LienHe.NgaySinh,
		dt.ID_NhanVienPhuTrach,
		ISNULL(DM_LienHe.DiaChi,'''') AS DiaChi,
    	ISNULL(TenTinhThanh,'''') AS TenTinhThanh,ISNULL(TenQuanHuyen,'''') AS TenQuanHuyen ,DM_LienHe.Email,DM_LienHe.GhiChu, DM_LienHe.NguoiTao,
    	DM_LienHe.NgayTao, TenDoiTuong, ISNULL(DM_LienHe.ChucVu,'''') AS ChucVu, DienThoaiCoDinh, ISNULL(DM_LienHe.XungHo,0) as XungHo, LoaiDoiTuong as LoaiLienHe
    	FROM DM_LienHe 
    	left join DM_DoiTuong dt ON DM_LienHe.ID_DoiTuong= dt.ID
    	left join DM_TinhThanh tt ON DM_LienHe.ID_TinhThanh = tt.ID
    	left join DM_QuanHuyen qh ON DM_LienHe.ID_QuanHuyen = qh.ID ' + @where + ' ORDER BY DM_LienHe.MaLienHe DESC'
    EXEC sp_executesql  @sqlExc
END

");
            
            CreateStoredProcedure(name: "[dbo].[UpdateGiaVonVer2]", parametersAction: p => new
            {
                IDHoaDonInput = p.Guid(),
                IDChiNhanhInput = p.Guid(),
                ThoiGian = p.DateTime()
            }, body: @"--DECLARE @IDHoaDonInput [uniqueidentifier];
 --   DECLARE @IDChiNhanhInput [uniqueidentifier];
 --   DECLARE @ThoiGian [datetime];
	--SET @IDHoaDonInput = '81D4959B-D099-483A-A0F2-D4FE6756D23B';
 --   SET @IDChiNhanhInput = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE';
 --   SET @ThoiGian = '2018-12-11 17:39:56.997';
    SET NOCOUNT ON;
    	DECLARE @ChiTietHoaDon TABLE (MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_HangHoa UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiNhanh UNIQUEIDENTIFIER);
    	INSERT INTO @ChiTietHoaDon
    	select hdcthd.MaHoaDon, @ThoiGian, hh.ID, hdctcthd.ID_LoHang, @IDChiNhanhInput FROM BH_HoaDon hdcthd
    	INNER JOIN BH_HoaDon_ChiTiet hdctcthd
    	ON hdcthd.ID = hdctcthd.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqd
    	ON hdctcthd.ID_DonViQuiDoi = dvqd.ID
    	INNER JOIN DM_HangHoa hh
    	on hh.ID = dvqd.ID_HangHoa
    	WHERE hdcthd.ID = @IDHoaDonInput
		GROUP BY hh.ID, hdctcthd.ID_LoHang, hdcthd.MaHoaDon;

		--SELECT * FROM @ChiTietHoaDon;

    	DECLARE @ChiTietHoaDonUpdate TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @ChiTietHoaDonUpdate
    	select hdupdate.ID, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID, 
    	CASE 
    		WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END AS NgayLapHoaDon, hdctupdate.SoThuTu, hdctupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, hdctupdate.TienChietKhau, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    	[dbo].[FUNC_TinhSLTonKhiTaoHD](cthdthemmoiupdate.ID_ChiNhanh, hhupdate.ID, hdctupdate.ID_LoHang, CASE
    		WHEN hdupdate.YeuCau = '4' AND cthdthemmoiupdate.ID_ChiNhanh = hdupdate.ID_CheckIn
    		THEN
    			hdupdate.NgaySua
    		ELSE
    			hdupdate.NgayLapHoaDon
    	END), hdctupdate.GiaVon, hdctupdate.GiaVon_NhanChuyenHang,
    	hhupdate.ID, dvqdupdate.ID, hdctupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, cthdthemmoiupdate.ID_ChiNhanh, hdupdate.ID_CheckIn, hdupdate.YeuCau FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    	ON hdupdate.ID = hdctupdate.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqdupdate
    	ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    	INNER JOIN DM_HangHoa hhupdate
    	on hhupdate.ID = dvqdupdate.ID_HangHoa
    	INNER JOIN @ChiTietHoaDon cthdthemmoiupdate
    	ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    	WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    	((hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon >= cthdthemmoiupdate.NgayLapHoaDon and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    	or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgaySua >= cthdthemmoiupdate.NgayLapHoaDon))
    	order by NgayLapHoaDon, SoThuTu desc, hdupdate.LoaiHoaDon, hdupdate.MaHoaDon;

    	--Update Kiem ke
    	UPDATE ctkiemke
    	SET ctkiemke.TienChietKhau = ctupdatekk.TonKho / ctupdatekk.TyLeChuyenDoi, ctkiemke.SoLuong = ctkiemke.ThanhTien - (ctupdatekk.TonKho /ctupdatekk.TyLeChuyenDoi)
    	FROM BH_HoaDon_ChiTiet ctkiemke
    	INNER JOIN @ChiTietHoaDonUpdate ctupdatekk
    	on ctkiemke.ID = ctupdatekk.ID_ChiTietHoaDon
    	WHERE ctupdatekk.LoaiHoaDon = 9;
    
    	UPDATE hdkkupdate
    	SET hdkkupdate.TongTienHang = dshoadonkkupdate.SoLuongGiam, hdkkupdate.TongGiamGia = dshoadonkkupdate.SoLuongLech, hdkkupdate.TongChiPhi = dshoadonkkupdate.SoLuongTang
    	FROM BH_HoaDon AS hdkkupdate
    	INNER JOIN
    	(SELECT ct.ID_HoaDon, SUM(CASE WHEN ct.SoLuong > 0 THEN ct.SoLuong ELSE 0 END) AS SoLuongTang,
    	SUM(CASE WHEN ct.SoLuong < 0 THEN ct.SoLuong ELSE 0 END) AS SoLuongGiam, SUM(SoLuong) AS SoLuongLech FROM BH_HoaDon_ChiTiet ct
    	INNER JOIN (SELECT IDHoaDon FROM @ChiTietHoaDonUpdate WHERE LoaiHoaDon = 9) AS KKHoaDon
    	ON ct.ID_HoaDon = KKHoaDon.IDHoaDon GROUP BY ct.ID_HoaDon) AS dshoadonkkupdate
    	ON hdkkupdate.ID = dshoadonkkupdate.ID_HoaDon;
    	--End update Kiem ke

		--Begin TinhGiaVonTrungBinh
		DECLARE @TinhGiaVonTrungBinh BIT;
		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanhInput;
		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
		BEGIN
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu, hdct.SoLuong, hdct.DonGia, hd.TongTienHang, hdct.TienChietKhau, hd.TongGiamGia, dvqd.TyLeChuyenDoi, 0, hdct.GiaVon, 
    		hdct.GiaVon_NhanChuyenHang,
    		hh.ID, dvqd.ID, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, cthdthemmoi.ID_ChiNhanh, hd.ID_CheckIn, hd.YeuCau FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct
    		ON hd.ID = hdct.ID_HoaDon
    		INNER JOIN DonViQuiDoi dvqd
    		ON hdct.ID_DonViQuiDoi = dvqd.ID
    		INNER JOIN DM_HangHoa hh
    		on hh.ID = dvqd.ID_HangHoa
    		INNER JOIN @ChiTietHoaDon cthdthemmoi
    		ON cthdthemmoi.ID_HangHoa = hh.ID
    		WHERE hd.ChoThanhToan = 0 AND hd.LoaiHoaDon != 3 AND hd.LoaiHoaDon != 19 AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) AND
    		((hd.ID_DonVi = cthdthemmoi.ID_ChiNhanh and hd.NgayLapHoaDon < cthdthemmoi.NgayLapHoaDon and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    		or (hd.YeuCau = '4'  and hd.ID_CheckIn = cthdthemmoi.ID_ChiNhanh and hd.NgaySua < cthdthemmoi.NgayLapHoaDon))
    		order by NgayLapHoaDon desc, SoThuTu desc, hd.LoaiHoaDon, hd.MaHoaDon;
		
    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @BangUpdateGiaVon
    		SELECT *, ROW_NUMBER() OVER (PARTITION BY tableUpdateGiaVon.ID_HangHoa, tableUpdateGiaVon.ID_LoHang ORDER BY tableUpdateGiaVon.NgayLapHoaDon) AS RN FROM
    		(SELECT * FROM @ChiTietHoaDonUpdate
    		UNION ALL
    		SELECT cthdGiaVon.IDHoaDon, cthdGiaVon.IDHoaDonBan, cthdGiaVon.MaHoaDon, cthdGiaVon.LoaiHoaDon, cthdGiaVon.ID_ChiTietHoaDon, cthdGiaVon.NgayLapHoaDon, cthdGiaVon.SoThuThu, cthdGiaVon.SoLuong, cthdGiaVon.DonGia, cthdGiaVon.TongTienHang,
    		cthdGiaVon.ChietKhau, cthdGiaVon.TongGiamGia, cthdGiaVon.TyLeChuyenDoi, cthdGiaVon.TonKho, cthdGiaVon.GiaVon, cthdGiaVon.GiaVonNhan, cthdGiaVon.ID_HangHoa, cthdGiaVon.IDDonViQuiDoi, cthdGiaVon.ID_LoHang , cthdGiaVon.ID_ChiTietDinhLuong,
    		cthdGiaVon.ID_ChiNhanhThemMoi, cthdGiaVon.ID_CheckIn, cthdGiaVon.YeuCau 
    		FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon WHERE cthdGiaVon.RN = 1) AS tableUpdateGiaVon;
    
    		UPDATE @BangUpdateGiaVon SET GiaVon = 
    		CASE
    			WHEN LoaiHoaDon = 4 THEN ((DonGia - ChietKhau) * (1 - (TongGiamGia/TongTienHang))) /TyLeChuyenDoi
    			ELSE GiaVon
    		END, GiaVonNhan =
    		CASE
    			WHEN LoaiHoaDon = 10 AND YeuCau = '4' AND ID_CheckIn = ID_ChiNhanhThemMoi THEN DonGia/TyLeChuyenDoi
    			ELSE GiaVonNhan
    		END
    		WHERE IDHoaDon = @IDHoaDonInput AND RN = 1;

			--SELECT * FROM @BangUpdateGiaVon
    
    		DECLARE @GiaVonCapNhat TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, IDHoaDonCu UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, IDChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, GiaVonCu FLOAT, IDHangHoa UNIQUEIDENTIFIER, IDDonViQuiDoi UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, IDChiTietDinhLuong UNIQUEIDENTIFIER,
    		IDChiNhanhThemMoi UNIQUEIDENTIFIER, IDCheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @GiaVonCapNhat
    		SELECT tableUpdate.IDHoaDon, tableUpdate.IDHoaDonBan, tableGiaVon.IDHoaDon, tableUpdate.MaHoaDon, tableUpdate.LoaiHoaDon, tableUpdate.ID_ChiTietHoaDon,tableUpdate.NgayLapHoaDon, tableUpdate.SoLuong, tableUpdate.DonGia,
    		tableUpdate.TongTienHang, tableUpdate.ChietKhau, tableUpdate.TongGiamGia, tableUpdate.TyLeChuyenDoi, tableUpdate.TonKho, tableUpdate.GiaVon, tableUpdate.GiaVonNhan, tableGiaVon.GiaVon, tableUpdate.ID_HangHoa,
    		tableUpdate.IDDonViQuiDoi, tableUpdate.ID_LoHang, tableUpdate.ID_ChiTietDinhLuong, tableUpdate.ID_ChiNhanhThemMoi, tableUpdate.ID_CheckIn, tableUpdate.YeuCau, tableUpdate.RN FROM @BangUpdateGiaVon tableUpdate
    		LEFT JOIN (SELECT (CASE WHEN ID_CheckIn = ID_ChiNhanhThemMoi THEN GiaVonNhan ELSE GiaVon END) AS GiaVon, ID_HangHoa, IDHoaDon, ID_LoHang, RN + 1 AS RN FROM @BangUpdateGiaVon) AS tableGiaVon
    		ON tableUpdate.ID_HangHoa = tableGiaVon.ID_HangHoa AND tableUpdate.RN = tableGiaVon.RN AND (tableUpdate.ID_LoHang = tableGiaVon.ID_LoHang OR tableUpdate.ID_LoHang IS NULL);
			--SELECT * FROM @GiaVonCapNhat
    		DECLARE @IDHoaDon UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonBan UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonCu UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX);
    		DECLARE @LoaiHoaDon INT;
    		DECLARE @IDChiTietHoaDon UNIQUEIDENTIFIER;
    		DECLARE @SoLuong FLOAT;
    		DECLARE @DonGia FLOAT;
    		DECLARE @TongTienHang FLOAT;
    		DECLARE @ChietKhau FLOAT;
    		DECLARE @TongGiamGia FLOAT;
    		DECLARE @TyLeChuyenDoi FLOAT;
    		DECLARE @TonKho FLOAT;
    		DECLARE @GiaVonCu FLOAT;
    		DECLARE @IDHangHoa UNIQUEIDENTIFIER;
    		DECLARE @IDDonViQuiDoi UNIQUEIDENTIFIER;
    		DECLARE @IDLoHang UNIQUEIDENTIFIER;
    		DECLARE @IDChiNhanhThemMoi UNIQUEIDENTIFIER;
    		DECLARE @IDCheckIn UNIQUEIDENTIFIER;
    		DECLARE @YeuCau NVARCHAR(MAX);
    		DECLARE @RN INT;
    		DECLARE @GiaVonMoi FLOAT;
    		DECLARE @GiaVonCuUpdate FLOAT;
    		DECLARE @IDHangHoaUpdate UNIQUEIDENTIFIER;
    		DECLARE @IDLoHangUpdate UNIQUEIDENTIFIER;
    
    		DECLARE @TongTienHangDemo FLOAT;
    		DECLARE @SoLuongDemo FLOAT;
    		DECLARE CS_GiaVon CURSOR SCROLL LOCAL FOR SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, ChietKhau, TongGiamGia, TyLeChuyenDoi, TonKho,
    		GiaVonCu, IDHangHoa, IDDonViQuiDoi, IDLoHang, IDChiNhanhThemMoi, IDCheckIn, YeuCau, RN FROM @GiaVonCapNhat WHERE RN > 1
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    		@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    			iF(@IDHangHoaUpdate = @IDHangHoa AND (@IDLoHangUpdate = @IDLoHang OR @IDLoHang IS NULL))
    			BEGIN
    				SET @GiaVonCu = @GiaVonCuUpdate;
    			END
    			ELSE
    			BEGIN
    				SET @IDHangHoaUpdate = @IDHangHoa;
    				SET @IDLoHangUpdate = @IDLoHang;
    			END
    			IF(@LoaiHoaDon = 4)
    			BEGIN
    			SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia - bhctdm.ChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    			FROM @GiaVonCapNhat bhctdm
    			left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    			WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    			GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    
    				IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    				BEGIN
    					IF(@TongTienHang != 0)
    					BEGIN
    						SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + (@TongTienHangDemo* (1-(@TongGiamGia/@TongTienHang))))/(@SoLuongDemo + @TonKho);
    					END
    					ELSE
    					BEGIN
    						SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo)/(@SoLuongDemo + @TonKho);
    					END
    				END
    				ELSE
    				BEGIN
    					IF(@TongTienHang != 0)
    					BEGIN
    						SET	@GiaVonMoi = (@TongTienHangDemo / @SoLuongDemo) * (1 - (@TongGiamGia / @TongTienHang));
    					END
    					ELSE
    					BEGIN
    						SET	@GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
    					END
    				END
    			END
    			ELSE IF (@LoaiHoaDon = 7)
    			BEGIN
    				SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    				FROM @GiaVonCapNhat bhctdm
    				left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    				WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    				GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    				IF(@TonKho - @SoLuongDemo > 0)
    				BEGIN
    					SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) - @TongTienHangDemo)/(@TonKho - @SoLuongDemo);
    				END
    				ELSE
    				BEGIN
    					SET @GiaVonMoi = @GiaVonCu;
    				END
    			END
    			ELSE IF(@LoaiHoaDon = 10)
    			BEGIN
    				SELECT @TongTienHangDemo = SUM(bhctdm.ChietKhau * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.ChietKhau * dvqddm.TyLeChuyenDoi) 
    				FROM @GiaVonCapNhat bhctdm
    				left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    				WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    				GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    				IF(@YeuCau = '1' OR (@YeuCau = '4' AND @IDChiNhanhThemMoi != @IDCheckIn))
    				BEGIN
    					SET @GiaVonMoi = @GiaVonCu;
    				END
    				ELSE IF (@YeuCau = '4' AND @IDChiNhanhThemMoi = @IDCheckIn)
    				BEGIN
    					IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    					BEGIN
    						SET @GiaVonMoi = (@GiaVonCu * @TonKho + @TongTienHangDemo)/(@TonKho + @SoLuongDemo);
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
    					END
    				END
    			END
    			ELSE IF (@LoaiHoaDon = 6)
    			BEGIN
    				SELECT @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    				FROM @GiaVonCapNhat bhctdm
    				left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    				WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    				GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    				IF(@IDHoaDonBan IS NOT NULL)
    				BEGIN
    					DECLARE @GiaVonHoaDonBan FLOAT;
    					SELECT @GiaVonHoaDonBan = GiaVon FROM @GiaVonCapNhat WHERE IDHoaDon = @IDHoaDonBan AND IDDonViQuiDoi = @IDDonViQuiDoi AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL);
    					IF(@GiaVonHoaDonBan IS NULL)
    					BEGIN
    						SELECT @GiaVonHoaDonBan = GiaVon FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDonBan AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
    					END
    					IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    					BEGIN
    						SET @GiaVonMoi = (@GiaVonCu * @TonKho + @GiaVonHoaDonBan * @SoLuongDemo) / (@TonKho + @SoLuongDemo);
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonHoaDonBan;
    					END
    				END
    				ELSE
    				BEGIN
    					SET @GiaVonMoi = @GiaVonCu;
    				END
    			END
    			ELSE
    			BEGIN
    				SET @GiaVonMoi = @GiaVonCu;
    			END
    			IF(@IDHoaDon = @IDHoaDonCu)
    			BEGIN
    				SET @GiaVonMoi = @GiaVonCuUpdate;	
    			END
    			ELSE
    			BEGIN
    				SET @GiaVonCuUpdate = @GiaVonMoi;
    			END
    			IF(@LoaiHoaDon = 10 AND @YeuCau = '4' AND @IDCheckIn = @IDChiNhanhThemMoi)
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVonNhan = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			ELSE
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVon = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon
    		--Update BH_HoaDon_ChiTiet
    		UPDATE hoadonchitiet1
    		SET hoadonchitiet1.GiaVon = _giavoncapnhat1.GiaVon, hoadonchitiet1.GiaVon_NhanChuyenHang = _giavoncapnhat1.GiaVonNhan
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet1
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat1
    		ON hoadonchitiet1.ID = _giavoncapnhat1.IDChiTietHoaDon
    		WHERE _giavoncapnhat1.LoaiHoaDon != 8 AND _giavoncapnhat1.LoaiHoaDon != 18;
    
    		UPDATE hoadonchitiet2
    		SET hoadonchitiet2.GiaVon = _giavoncapnhat2.GiaVon, hoadonchitiet2.ThanhTien = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet2
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat2
    		ON hoadonchitiet2.ID = _giavoncapnhat2.IDChiTietHoaDon
    		WHERE _giavoncapnhat2.LoaiHoaDon = 8;
    
    		UPDATE hoadonchitiet3
    		SET hoadonchitiet3.DonGia = _giavoncapnhat3.GiaVon, hoadonchitiet3.PTChietKhau = CASE WHEN hoadonchitiet3.GiaVon - _giavoncapnhat3.GiaVon > 0 THEN hoadonchitiet3.GiaVon - _giavoncapnhat3.GiaVon ELSE 0 END,
    		hoadonchitiet3.TienChietKhau = CASE WHEN hoadonchitiet3.GiaVon - _giavoncapnhat3.GiaVon > 0 THEN 0 ELSE hoadonchitiet3.GiaVon - _giavoncapnhat3.GiaVon END
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet3
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat3
    		ON hoadonchitiet3.ID = _giavoncapnhat3.IDChiTietHoaDon
    		WHERE _giavoncapnhat3.LoaiHoaDon = 18;
    
    		UPDATE chitietdinhluong
    		SET chitietdinhluong.GiaVon = gvDinhLuong.GiaVonDinhLuong / chitietdinhluong.SoLuong
    		FROM BH_HoaDon_ChiTiet AS chitietdinhluong
    		INNER JOIN
    		(SELECT SUM(ct.GiaVon * ct.SoLuong) AS GiaVonDinhLuong, ct.ID_ChiTietDinhLuong FROM BH_HoaDon_ChiTiet ct
    		INNER JOIN (SELECT IDChiTietDinhLuong FROM @GiaVonCapNhat GROUP BY IDChiTietDinhLuong) gv
    		ON ct.ID_ChiTietDinhLuong = gv.IDChiTietDinhLuong
    		WHERE gv.IDChiTietDinhLuong IS NOT NULL AND ct.ID != ct.ID_ChiTietDinhLuong
    		GROUP BY ct.ID_ChiTietDinhLuong) AS gvDinhLuong
    		ON chitietdinhluong.ID = gvDinhLuong.ID_ChiTietDinhLuong
    		--END Update BH_HoaDon_ChiTiet
    		--Update DM_GiaVon
    		UPDATE _dmGiaVon1
    		SET _dmGiaVon1.GiaVon = _gvUpdateDM1.GiaVon
    		FROM (SELECT dvqd1.ID AS IDDonViQuiDoi, _giavon1.IDLoHang AS IDLoHang, (CASE WHEN _giavon1.IDCheckIn = _giavon1.IDChiNhanhThemMoi THEN _giavon1.GiaVonNhan ELSE _giavon1.GiaVon END) * dvqd1.TyLeChuyenDoi AS GiaVon, _giavon1.IDChiNhanhThemMoi AS IDChiNhanh FROM @GiaVonCapNhat _giavon1
    		INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat GROUP BY IDHangHoa,IDLoHang) AS _maxGiaVon1
    		ON _giavon1.IDHangHoa = _maxGiaVon1.IDHangHoa AND _giavon1.RN = _maxGiaVon1.RN AND (_giavon1.IDLoHang = _maxGiaVon1.IDLoHang OR _maxGiaVon1.IDLoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd1
    		ON dvqd1.ID_HangHoa = _giavon1.IDHangHoa) AS _gvUpdateDM1
    		LEFT JOIN DM_GiaVon _dmGiaVon1
    		ON _gvUpdateDM1.IDChiNhanh = _dmGiaVon1.ID_DonVi AND _gvUpdateDM1.IDDonViQuiDoi = _dmGiaVon1.ID_DonViQuiDoi AND (_gvUpdateDM1.IDLoHang = _dmGiaVon1.ID_LoHang OR _dmGiaVon1.ID_LoHang IS NULL)
    		WHERE _dmGiaVon1.ID IS NOT NULL;
    
    		INSERT INTO DM_GiaVon (ID, ID_DonVi, ID_DonViQuiDoi, ID_LoHang, GiaVon)
    		SELECT NEWID(), _gvUpdateDM.IDChiNhanh, _gvUpdateDM.IDDonViQuiDoi, _gvUpdateDM.IDLoHang, _gvUpdateDM.GiaVon FROM 
    		(SELECT dvqd2.ID AS IDDonViQuiDoi, _giavon2.IDLoHang AS IDLoHang, (CASE WHEN _giavon2.IDCheckIn = _giavon2.IDChiNhanhThemMoi THEN _giavon2.GiaVonNhan ELSE _giavon2.GiaVon END) * dvqd2.TyLeChuyenDoi AS GiaVon, _giavon2.IDChiNhanhThemMoi AS IDChiNhanh FROM @GiaVonCapNhat _giavon2
    		INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat GROUP BY IDHangHoa, IDLoHang) AS _maxGiaVon
    		ON _giavon2.IDHangHoa = _maxGiaVon.IDHangHoa AND _giavon2.RN = _maxGiaVon.RN AND (_giavon2.IDLoHang = _maxGiaVon.IDLoHang OR _maxGiaVon.IDLoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd2
    		ON dvqd2.ID_HangHoa = _giavon2.IDHangHoa) AS _gvUpdateDM
    		LEFT JOIN DM_GiaVon _dmGiaVon
    		ON _gvUpdateDM.IDChiNhanh = _dmGiaVon.ID_DonVi AND _gvUpdateDM.IDDonViQuiDoi = _dmGiaVon.ID_DonViQuiDoi AND (_gvUpdateDM.IDLoHang = _dmGiaVon.ID_LoHang OR _dmGiaVon.ID_LoHang IS NULL)
    		WHERE _dmGiaVon.ID IS NULL;
    		--End Update DM_GiaVon
		END
		--END TinhGiaVonTrungBinh");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_DaoTao]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_KhenThuong]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_LuongPhuCap]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_MienGiamThue]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_QuaTrinhCongTac]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_TheoBaoHiem]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_TheoDoTuoi]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_TheoHopDong]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_ThongTinGiaDinh]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_ThongTinSucKhoe]");
            DropStoredProcedure("[dbo].[BaoCaoNhanVien_TongHop]");
            DropStoredProcedure("[dbo].[CapNhatPhongBanMacDinhKhiXoa]");
            DropStoredProcedure("[dbo].[SP_GetInforBasic_DoiTuongByID]");
            DropStoredProcedure("[dbo].[UpdateGiaVonVer2]");
        }
    }
};