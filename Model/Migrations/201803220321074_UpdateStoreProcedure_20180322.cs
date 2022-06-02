namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStoreProcedure_20180322 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getlist_HoaDon_afterTraHang]", parametersAction: p => new
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
    	c.TongGiamGia
    	from
    	(
    		Select 
    		hd.ID,
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    		dt.DienThoai,
    		hd.ID_DoiTuong,	
    		hd.ID_HoaDon,
    		hd.ID_BangGia,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
    		hd.NguoiTao,	
    		hd.DienGiai,	
    
    		Case When nv.TenNhanVien != '' then nv.TenNhanVien else '' end as TenNhanVien,
    		Case When dt.TenDoiTuong != '' then dt.TenDoiTuong else N'Khách lẻ' end as TenDoiTuong,
    		Case When dt.TenDoiTuong_KhongDau != '' then dt.TenDoiTuong_KhongDau else 'khach le' end as TenDoiTuong_KhongDau,
    		Case When dt.TenDoiTuong_ChuCaiDau != '' then dt.TenDoiTuong_ChuCaiDau else 'kl' end as TenDoiTuong_ChuCaiDau,
    		ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
    		ISNULL(hd.TongTienHang, 0) as TongTienHang,
    		ISNULL(hd.TongGiamGia, 0) as TongGiamGia
    		from 
    		(
    			Select 
    			a.ID,
    			Sum(ISNULL(a.SoLuongBan, 0)) as SoLuongBan,
    			Sum(ISNULL(a.SoLuongTra, 0)) as SoLuongTra
    			from
    			(
    				Select 
    				hd.ID as ID,
    				Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongBan,
    				null as SoLuongTra 
    				from
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				where hd.ID_DonVi = @ID_DonVi
    				and hd.loaihoadon = '1'
    				Group by hd.ID
    
    				Union all
    				select 
    				hd.ID_HoaDon as ID,
    				null as SoLuongBan,
    				Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongTra
    				from
    				BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    				where hd.ID_DonVi = @ID_DonVi
    				and hd.loaihoadon = '6'
    				Group by hd.ID_HoaDon
    			)a
    			--where SoLuongTra < SoLuongBan
    			Group by a.ID
    		) b
    		inner join BH_HoaDon hd on b.ID = hd.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd 
    		and b.SoLuongBan > b.SoLuongTra
    	)c
    	where MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD
    	order by NgayLapHoaDon DESC");

            
        }
        
        public override void Down()
        {
            DropStoredProcedure("getlist_HoaDon_afterTraHang");
            
        }
    }
}