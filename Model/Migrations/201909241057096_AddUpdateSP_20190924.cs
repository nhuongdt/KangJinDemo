namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190924 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[LoadDanhMucCongViec]", parametersAction: p => new
            {
                ListID_DonVi = p.String(),
                ListID_NVPhoiHop = p.String(),
                ListID_LoaiCongViec = p.String(),
                ListID_NhanVien = p.String(),
                ListID_NhanVienQL = p.String(),
                DayStart = p.DateTime(),
                DayEnd = p.DateTime()
            }, body: @"SET NOCOUNT ON;

	SELECT cv.ID, cv.ID_DonVi,cv.ID_LoaiTuVan, Main.ChuoiNhanVienPhoiHop, cv.ID_NhanVienQuanLy,loaicv.TenLoaiTuVanLichHen as LoaiCongViec, cv.Ma_TieuDe, dt.TenDoiTuong as TenKhachHang,
	dt.MaDoiTuong, lienhe.TenLienHe, nv.TenNhanVien as TenNhanVienPhuTrach, cv.NgayGio, cv.NgayGioKetThuc, cv.NhacNho, cv.NoiDung, dt.LoaiDoiTuong as LoaiDoiTuongCV,
	cv.TrangThai, dt.ID_TrangThai as TrangThaiKhach, cv.NguoiTao, cv.NgayTao, cv.GhiChu,cv.KetQua, cv.MucDoUuTien, cv.FileDinhKem,
	cv.NgayHoanThanh, dt.DienThoai as SoDienThoai, nguonk.TenNguonKhach as NguonKhach FROM ChamSocKhachHangs cv
	inner join DM_LoaiTuVanLichHen loaicv on cv.ID_LoaiTuVan = loaicv.ID
	inner join DM_DoiTuong dt on cv.ID_KhachHang = dt.ID
	left join DM_NguonKhachHang nguonk on dt.ID_NguonKhach = nguonk.ID
	left join DM_LienHe lienhe on cv.ID_LienHe = lienhe.ID
	left join NS_NhanVien nv on cv.ID_NhanVien = nv.ID
	left join (
        Select distinct cs_khnv.ID_ChamSocKhachHang,
            (
                Select nvl.TenNhanVien + ',' AS [text()]
                From dbo.ChamSocKhachHang_NhanVien cskhnv
				inner join dbo.NS_NhanVien nvl on cskhnv.ID_NhanVien = nvl.ID
                Where cskhnv.ID_ChamSocKhachHang = cs_khnv.ID_ChamSocKhachHang
                For XML PATH ('')
            ) ChuoiNhanVienPhoiHop, cs_khnv.ID_NhanVien
        From dbo.ChamSocKhachHang_NhanVien cs_khnv
    ) Main on cv.ID = Main.ID_ChamSocKhachHang
	where cv.PhanLoai = 4 and cv.TrangThai != '0'
	and cv.NgayGio >= @DayStart and cv.NgayGioKetThuc < @DayEnd
	or (((select TOP 1 [name] from splitstring(@ListID_DonVi) ORDER BY [name]) = '' or cv.ID_DonVi=(select * from splitstring(@ListID_DonVi) where [name] like cv.ID_DonVi))
	and ((select TOP 1 [name] from splitstring(@ListID_NVPhoiHop) ORDER BY [name]) = '' or Main.ID_NhanVien=(select * from splitstring(@ListID_DonVi) where [name] like Main.ID_NhanVien))
	and ((select TOP 1 [name] from splitstring(@ListID_LoaiCongViec) ORDER BY [name]) = '' or cv.ID_LoaiTuVan=(select * from splitstring(@ListID_LoaiCongViec) where [name] like cv.ID_LoaiTuVan))
	and ((select TOP 1 [name] from splitstring(@ListID_NhanVien) ORDER BY [name]) = '' or cv.ID_NhanVien=(select * from splitstring(@ListID_NhanVien) where [name] like cv.ID_NhanVien))
	and ((select TOP 1 [name] from splitstring(@ListID_NhanVienQL) ORDER BY [name]) = '' or cv.ID_NhanVienQuanLy=(select * from splitstring(@ListID_NhanVienQL) where [name] like cv.ID_NhanVienQuanLy) or cv.ID_NhanVien=(select * from splitstring(@ListID_NhanVienQL) where [name] like cv.ID_NhanVien))
	)
	order by NgayTao desc");

            CreateStoredProcedure(name: "[dbo].[SP_ValueCard_ServiceUsed]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                DateFrom = p.String(8),
                DateTo = p.String(8),
                Status = p.String(4)
            }, body: @"SET NOCOUNT ON;
	select hd.ID, hd.MaHoaDon,hd.NgayLapHoaDon,ISNULL(dt.MaDoiTuong,'') as MaDoiTuong, ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, 
	qd.MaHangHoa,hh.TenHangHoa,ct.SoLuong, ct.DonGia, ct.TienChietKhau, ct.ThanhTien,  tblq.ThuTuThe
	from BH_HoaDon hd
	join BH_HoaDon_ChiTiet ct on hd.id= ct.id_hoadon
	left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
	join DonViQuiDoi qd on ct.id_donviquidoi= qd.id
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	join (select qct.ID_HoaDonLienQuan, SUM(ISNULL(qct.ThuTuThe ,0)) as ThuTuThe
		from Quy_HoaDon_Chitiet qct 
		join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		where qhd.TrangThai ='1' and qct.ThuTuThe > 0
		and CONVERT(varchar, qhd.NgayLapHoaDon, 112) >=@DateFrom
		and CONVERT(varchar, qhd.NgayLapHoaDon, 112) <= @DateTo
		group by qct.ID_HoaDonLienQuan) tblQ on tblQ.ID_HoaDonLienQuan= hd.ID
	where hd.LoaiHoaDon in ( 1,3,6,19) 
	and hd.ChoThanhToan ='0'
	and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)	
	order by hd.NgayLapHoaDon desc");

            Sql(@"ALTER PROCEDURE [dbo].[getList_LichSuKhuyenMai]
    @ID_KhuyenMai [uniqueidentifier]
AS
BEGIN
    Select 
    	a.ID,
    	a.MaHoaDon,
    	a.NgayLapHoaDon, 
    	a.TenNhanVien,
    	Cast(round(a.DoanhThu, 0) as float) as DoanhThu,
    	Cast(round(a.GiaTriKhuyenMai, 0) as float) as GiaTriKhuyenMai
    	From
    	(
			-- km hoadon
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
    		where ID_KhuyenMai =  @ID_KhuyenMai and chothanhtoan = '0'
    
    		union all
			-- km hanghoa
			select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon,nv.TenNhanVien,
				hd.PhaiThanhToan as DoanhThu,
    			case HinhThuc
    				when 12 then ctkm.SoLuong * ctkm.DonGia
    				when 22 then ctkm.SoLuong * ctkm.DonGia
    				when 21 then ctkm.SoLuong *  ctkm.TienChietKhau
    				when 13 then ctkm.SoLuong * ctkm.TienChietKhau
    				when 24 then ctkm.SoLuong * ctkm.TienChietKhau
    				when 23 then 0
    				end as GiaTriKhuyenMai
    		from BH_HoaDon_ChiTiet ct
    		join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
    		join DM_KhuyenMai km on ct.ID_KhuyenMai = km.ID
    		join BH_HoaDon_ChiTiet ctkm on ct.ID_DonViQuiDoi = ctkm.ID_TangKem and ct.ID_HoaDon = ctkm.ID_HoaDon and (ctkm.ID_TangKem is not null or ctkm.Tangkem ='0')    	
			join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where ct.ID_KhuyenMai = @ID_KhuyenMai
    		and hd.ChoThanhToan = 0
    	) a
    	Order by NgayLapHoaDon DESC
END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[LoadDanhMucCongViec]");
            DropStoredProcedure("[dbo].[SP_ValueCard_ServiceUsed]");
        }
    }
}
