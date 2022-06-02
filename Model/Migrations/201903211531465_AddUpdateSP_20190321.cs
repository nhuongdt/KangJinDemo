namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190321 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SP_GetInforHoaDon_ByMaHoaDon]", parametersAction: p => new
            {
                MaHoaDon = p.String()
            }, body: @"select 
    		hd.ID,
    		hd.MaHoaDon,
			hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
    		hd.TongTienHang,
    		ISNULL(hd.TongGiamGia,0) + ISNULL(hd.KhuyeMai_GiamGia, 0) as TongGiamGia,
    		CAST(ISNULL(hd.PhaiThanhToan,0) as float)  as PhaiThanhToan,
    		CAST(ISNULL(TongThuChi,0) as float) as KhachDaTra,	
			ISNULL(dt.TenDoiTuong,N'Khách lẻ')  as TenDoiTuong,
			ISNULL(bg.TenGiaBan,N'Bảng giá chung') as TenBangGia,
			ISNULL(nv.TenNhanVien,N'')  as TenNhanVien,
			ISNULL(dv.TenDonVi,N'')  as TenDonVi,    		
    		case when hd.NgayApDungGoiDV is null then '' else  convert(varchar(14), hd.NgayApDungGoiDV ,103) end  as NgayApDungGoiDV,
    		case when hd.HanSuDungGoiDV is null then '' else  convert(varchar(14), hd.HanSuDungGoiDV ,103) end as HanSuDungGoiDV,
    		hd.NguoiTao as NguoiTaoHD,
    		hd.DienGiai,
    		hd.ID_DonVi,
    		-- get avoid error at variable at class BH_HoaDonDTO
    		NEWID() as ID_DonViQuiDoi,
    
    
    		case 
    			when hd.LoaiHoaDon = 1 OR hd.LoaiHoaDon =6 OR hd.LoaiHoaDon =19 then
    									case when hd.ChoThanhToan is null then N'Đã hủy'
    									else N'Hoàn thành' end
    			when hd.LoaiHoaDon = 3 then
    									case when hd.YeuCau ='1' then N'Phiếu tạm'
    									else 
    										case when hd.YeuCau ='2' then  N'Đang giao hàng'
    										else 
    											case when hd.YeuCau ='3' then N'Hoàn thành'
    											else  N'Đã hủy' end
    										end
    									end
    		end as TrangThai													
    	from BH_HoaDon hd
    	left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    	left join DM_DonVi dv on hd.ID_DonVi= dv.ID
    	left join DM_GiaBan bg on hd.ID_BangGia= bg.ID
    	left join 
    		(select qct.ID_HoaDonLienQuan, SUM(ISNULL(qct.TienThu,0)) as TongThuChi
    		from Quy_HoaDon_ChiTiet qct
    		left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID where qhd.TrangThai ='1'
    		group by qct.ID_HoaDonLienQuan) soquy on hd.ID = soquy.ID_HoaDonLienQuan		
    	where hd.MaHoaDon = @MaHoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_GetSoDuTheGiaTri_ByTime]", parametersAction: p => new
            {
                ID_DoiTuong = p.Guid(),
                Time = p.DateTime()
            }, body: @"select dbo.TinhSoDuKHTheoThoiGian(@ID_DoiTuong,@Time)");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GetInforHoaDon_ByMaHoaDon]");
            DropStoredProcedure("[dbo].[SP_GetSoDuTheGiaTri_ByTime]");
        }
    }
}
