namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20181112 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetList_GoiDichVu_afterUseAndTra]", parametersAction: p => new
            {
                MaHD = p.String(),
                ID_DonVi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime()
            }, body: @"Select 
    	c.ID,
    	c.MaHoaDon,
    	c.ID_HoaDon,
    	c.ID_BangGia,
    	c.ID_NhanVien,
    	c.ID_DonVi,
    	c.NguoiTao,
    	c.DienGiai,
    	c.NgayLapHoaDon,
    	c.TenNhanVien,
    	c.ID_DoiTuong,
    	c.TenDoiTuong,
    	c.PhaiThanhToan,
    	c.TongTienHang,
    	c.TongGiamGia,
    	c.DiemGiaoDich,
    		c.SoLuongTra,
    		c.SoLuongBan,
    		c.LoaiHoaDon
    	from
    	(
    		Select 
    		hd.ID,
    		hd.MaHoaDon,
    			hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
    		dt.DienThoai,
    		hd.ID_DoiTuong,	
    		hd.ID_HoaDon,
    		hd.ID_BangGia,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
    		hd.NguoiTao,	
    		hd.DienGiai,	
    			b.SoLuongTra,
    			b.SoLuongBan,
    		Case When nv.TenNhanVien != '' then nv.TenNhanVien else '' end as TenNhanVien,
    		Case When dt.TenDoiTuong != '' then dt.TenDoiTuong else N'Khách lẻ' end as TenDoiTuong,
    		Case When dt.TenDoiTuong_KhongDau != '' then dt.TenDoiTuong_KhongDau else 'khach le' end as TenDoiTuong_KhongDau,
    		Case When dt.TenDoiTuong_ChuCaiDau != '' then dt.TenDoiTuong_ChuCaiDau else 'kl' end as TenDoiTuong_ChuCaiDau,
    		ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
    		ISNULL(hd.TongTienHang, 0) as TongTienHang,
    		ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
    		ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich
    
    		from 
    		(
    			Select 
    			a.ID,
    			SUM(ISNULL(a.SoLuongBan, 0)) as SoLuongBan,
    			Sum(ISNULL(a.SoLuongTra, 0)) as SoLuongTra
    			from
    			(
    				select  max(ct1.ID_HoaDon) as ID, max(ct1.SoLuong) as SoLuongBan,sum(ct2.SoLuong) as SoLuongTra
					from BH_HoaDon_ChiTiet ct1
					left join BH_HoaDon_ChiTiet ct2 on ct1.ID= ct2.ID_ChiTietGoiDV
					left join BH_HoaDon hd2 on hd2.ID= ct2.ID_HoaDon 					
					-- khong lay HD da bi huy, khi chua su dung lan nao ct2.SoLuong = null va ChoThanhToan = null
					 where (hd2.ChoThanhToan = '0' OR (hd2.ChoThanhToan is null and ct2.SoLuong is null)) 
					-- khong get TP dinh luong
					and (ct1.ID_ChiTietDinhLuong is null or ct1.ID_ChiTietDinhLuong = ct1.ID)
					and (ct2.ID_ChiTietDinhLuong is null or ct2.ID_ChiTietDinhLuong = ct2.ID)
					group by ct1.ID   				
    			)a
    			Group by a.ID
    		) b
    		inner join BH_HoaDon hd on b.ID = hd.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd 
    		and b.SoLuongBan > b.SoLuongTra
    			and hd.chothanhtoan = '0' AND LoaiHoaDon= 19
    	)c
    	where  MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD
    	order by NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[SP_GetChiTietHoaDonGoiDV_AfterUseAndTra]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid()
            }, body: @"select ct1.ID, ct1.ID_HoaDon, max(ct1.SoLuong)- ISNULL(sum(ct2.SoLuong),0)  as SoLuong, ct1.ID_DonViQuiDoi,
				ct1.DonGia,ct1.GiaVon,ct1.ThanhTien,qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa, max(ISNULL(ct1.TienChietKhau,0)) as GiamGia, 
				ct1.ThoiGian, ct1.GhiChu,ct1.ID_LoHang, CAST((ct1.SoThuTu) as float) as SoThuTu , ct1.TangKem,ct1.ID_TangKem,
				hh.LaHangHoa, hh.TenHangHoa
		from BH_HoaDon_ChiTiet ct1
		join DonViQuiDoi qd on ct1.ID_DonViQuiDoi = qd.ID
		join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
		left join BH_HoaDon_ChiTiet ct2 on ct1.ID= ct2.ID_ChiTietGoiDV
		left join BH_HoaDon hd2 on hd2.ID= ct2.ID_HoaDon 
		-- khong lay HD da bi huy, khi chua su dung lan nao ct2.SoLuong = null va ChoThanhToan = null
		where (hd2.ChoThanhToan = '0' OR (hd2.ChoThanhToan is null AND ct2.SoLuong is null))
		-- khong lay TP dinh luong
		and (ct1.ID_ChiTietDinhLuong is null or ct1.ID_ChiTietDinhLuong = ct1.ID)
		and (ct2.ID_ChiTietDinhLuong is null or ct2.ID_ChiTietDinhLuong = ct2.ID)
		and ct1.ID_HoaDon = @ID_HoaDon
		group by ct1.ID,ct1.ID_DonViQuiDoi,ct1.ID, ct1.ID_HoaDon,ct1.DonGia,ct1.GiaVon,ct1.ThanhTien,qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,
		ct1.ThoiGian, ct1.GhiChu,ct1.ID_LoHang, ct1.SoThuTu, ct1.TangKem,ct1.ID_TangKem,hh.LaHangHoa, hh.TenHangHoa
		-- chi lay Hang co SoLuongConLai > 0
		Having max(ct1.SoLuong) > SUM(ISNULL(ct2.SoLuong,0))");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetList_GoiDichVu_afterUseAndTra]");
            DropStoredProcedure("[dbo].[SP_GetChiTietHoaDonGoiDV_AfterUseAndTra]");
        }
    }
}
