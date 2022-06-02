namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180702 : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[PageCountTraCuuHangHoa]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                MaHH = p.String(),
                MaHHCoDau = p.String()
            }, body: @"select dvqd.ID as ID_DonViQuiDoi from DonViQuiDoi dvqd 
    	left join dm_hanghoa hh on dvqd.ID_hangHoa = hh.ID
    	where (hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH or dvqd.MaHangHoa like @MaHH or dvqd.MaHangHoa like @MaHHCoDau) and dvqd.Xoa is null and dvqd.LaDonViChuan = 1 and hh.TheoDoi =1
    		group by dvqd.ID   ");

            CreateStoredProcedure(name: "[dbo].[Report_BanHang_BieuDo]", parametersAction: p => new
            {
                MaHD = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String()
            }, body: @"SELECT 
    		a.NgayLapHoaDon,
    		CAST(ROUND((a.ThanhTien), 0) as float) as ThanhTien,
			a.TenDonVi,
    		a.ID_NhomHang
    	FROM
    	(
			Select
    		hd.NgayLapHoaDon,
			dv.TenDonVi,
    		hh.ID_NhomHang,
    		hdct.ThanhTien
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			inner join DM_DonVi dv on hd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon = 1
    		and hh.LaHangHoa like @LaHangHoa
    		and hd.MaHoaDon like @maHD
    		and hd.TongTienHang > 0
    	) a
    	order by a.NgayLapHoaDon desc");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[Report_BanHang_BieuDo]");
        }
    }
}