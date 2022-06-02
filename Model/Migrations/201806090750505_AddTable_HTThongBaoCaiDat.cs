namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_HTThongBaoCaiDat : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[ReportKhachHang_BanHangChiTiet]", parametersAction: p => new
            {
                ID_KhachHang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String()
            }, body: @"IF (@ID_KhachHang != '')
    	BEGIN
    		SELECT
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon as ThoiGian,
    			Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.SoLuong, 0)) else SUM(ISNULL(hdct.SoLuong, 0)) * (-1) end as SoLuong,
    			Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.ThanhTien, 0)) else SUM(ISNULL(hdct.ThanhTien, 0)) * (-1) end as TongTienHang,
    			Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang), 0)) else SUM(ISNULL(hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang), 0)) * (-1) end as GiamGiaHD,
    		Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.ThanhTien * (1 - hd.TongGiamGia / hd.TongTienHang), 0)) else SUM(ISNULL(hdct.ThanhTien * (1 - hd.TongGiamGia / hd.TongTienHang), 0)) * (-1) end as DoanhThu
    		FROM
    			BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and hd.ID_DoiTuong = @ID_KhachHang
    			and hd.ChoThanhToan = 0
    			and (hd.loaihoadon = 1 or hd.loaihoadon = 6)
				and hd.TongTienHang != 0
    			GROUP BY hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.LoaiHoaDon
    		ORDER BY hd.NgayLapHoaDon DESC
    	END
    	ELSE
    	BEGIN
    		SELECT
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon as ThoiGian,
    			Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.SoLuong, 0)) else SUM(ISNULL(hdct.SoLuong, 0)) * (-1) end as SoLuong,
    			Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.ThanhTien, 0)) else SUM(ISNULL(hdct.ThanhTien, 0)) * (-1) end as TongTienHang,
    			Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang), 0)) else SUM(ISNULL(hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang), 0)) * (-1) end as GiamGiaHD,
    		Case when hd.LoaiHoaDon = 1 then SUM(ISNULL(hdct.ThanhTien * (1 - hd.TongGiamGia / hd.TongTienHang), 0)) else SUM(ISNULL(hdct.ThanhTien * (1 - hd.TongGiamGia / hd.TongTienHang), 0)) * (-1) end as DoanhThu
    		FROM
    			BH_HoaDon hd
    				inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			Where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and hd.ID_DoiTuong is null
    			and hd.ChoThanhToan = 0
    			and (hd.loaihoadon = 1 or hd.loaihoadon = 6)
				and hd.TongTienHang != 0
    			GROUP BY hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.LoaiHoaDon
    		ORDER BY hd.NgayLapHoaDon DESC
    	END");

            AlterStoredProcedure(name: "[dbo].[insert_DoiTuong]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_DonVi = p.Guid(),
                MaDoiTuong = p.String(),
                TenDoiTuong = p.String(),
                TenDoiTuong_KhongDau = p.String(),
                TenDoiTuong_ChuCaiDau = p.String(),
                GioiTinhNam = p.Boolean(),
                LoaiDoiTuong = p.Int(),
                LaCaNhan = p.Int(),
                timeCreate = p.DateTime()
            }, body: @"insert into DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong,TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, chiase, theodoi, GioiTinhNam, NguoiTao, NgayTao, ID_DonVi)
    Values (@ID, @LoaiDoiTuong, @LaCaNhan, @MaDoiTuong, @TenDoiTuong, @TenDoiTuong_KhongDau, @TenDoiTuong_ChuCaiDau, '0', '0', @GioiTinhNam, 'admin', @timeCreate, @ID_DonVi)");
        }
        
        public override void Down()
        {
            
        }
    }
}