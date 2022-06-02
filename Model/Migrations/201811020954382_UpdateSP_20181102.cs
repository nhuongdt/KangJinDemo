namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20181102 : DbMigration
    {
        public override void Up()
        {
            
            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoChietKhau_TongHop]
    @MaNV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
	@ID_NhanVien [nvarchar](max),
	@ID_NhanVien_SP [nvarchar](max),
    @ThucHien_TuVan [nvarchar](max)
AS
BEGIN
    SELECT 
    	a.MaNhanVien,
    	MAX(a.TenNhanVien) as TenNhanVien,
    	CAST(ROUND(SUM(a.HoaHongThucHien), 0) as float) as HoaHongThucHien,
    	CAST(ROUND(SUM(a.HoaHongTuVan), 0) as float) as HoaHongTuVan,
		CAST(ROUND(SUM(a.HoaHongBanGoiDV), 0) as float) as HoaHongBanGoiDV,
    	CAST(ROUND(SUM(a.HoaHongThucHien + a.HoaHongTuVan + a.HoaHongBanGoiDV), 0) as float) as Tong
    	FROM
    	(
    	Select 
    	nv.MaNhanVien,
    	nv.TenNhanVien,
    	--Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    	--Case when ck.ThucHien_TuVan = 0 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan
		Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    	Case when ck.ThucHien_TuVan = 0 and ck.TheoYeuCau = 0 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
		Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV
    	from
    	BH_NhanVienThucHien ck
    	inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    	inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    	inner join NS_NhanVien nv on ck.ID_NhanVien = nv.ID
    	Where hd.ChoThanhToan = 0 and 
		(nv.ID like @ID_NhanVien or nv.ID in (SELECT * FROM splitstring(@ID_NhanVien_SP))) 
		AND (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	and ck.ThucHien_TuVan like @ThucHien_TuVan
    	and (nv.MaNhanVien like @MaNV or nv.TenNhanVien like @MaNV)
    	) a
    	GROUP BY a.MaNhanVien
    	ORDER BY Tong DESC
END");
            
        }
        
        public override void Down()
        {
        }
    }
}
