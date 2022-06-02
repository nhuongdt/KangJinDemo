namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181217 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[check_MaNhanVien](@inputVar NVARCHAR(MAX) )
RETURNS NVARCHAR(MAX)
AS
BEGIN    
	DECLARE @MaNhanVien NVARCHAR(MAX);
	DECLARE @tab table (MaNhanVien int, SoThuTu int)
	insert into @tab
	Select RIGHT(MaNhanVien, 5), ROW_NUMBER() over(order by MaNhanVien) as MaNhanVien from NS_NhanVien
	WHERE LEFT(MaNhanVien, 2) = LEFT(@inputVar, 2) AND ISNUMERIC(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 1 AND LEN(SUBSTRING(MaNhanVien, 3, LEN(MaNhanVien) - 2)) = 5
	order by MaNhanVien
	SELECT @MaNhanVien = (select TOP 1 SoThuTu from @tab
	where MaNhanVien != SoThuTu
	order by MaNhanVien)
	IF (LEN(@MaNhanVien) > 0)
	BEGIN
			SELECT @MaNhanVien = CASE
			WHEN @MaNhanVien >= 0 and @MaNhanVien < 9 THEN 'NV0000' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien >= 9 and @MaNhanVien < 99 THEN 'NV000' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien >= 99 and @MaNhanVien < 999 THEN 'NV00' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien >= 999 and @MaNhanVien < 9999 THEN 'NV0' + CONVERT(CHAR, @MaNhanVien)
			WHEN @MaNhanVien >= 9999 THEN 'NV' + CONVERT(CHAR, @MaNhanVien)
			END
	END
	else
	BEgin
	 SELECT @MaNhanVien = @inputVar;
	END
	RETURN @MaNhanVien
END");

            CreateStoredProcedure(name: "[dbo].[get_MaNhanVien]", parametersAction: p => new
            {
                MaNhanVien = p.String()
            }, body: @"select dbo.check_MaNhanVien(@MaNhanVien)");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_DoiTuong_HinhThuc2]
    @SqlQuery [nvarchar](max)
AS
BEGIN
    declare @sql  [nvarchar](max);
    	declare @sql2  [nvarchar](max);
    	set @sql = 'SELECT b.ID_DoiTuong
    	FROM
    	(
    		SELECT
    		a.ID_DoiTuong,
    		CAST(ROUND(a.GiaTriBan , 0) as float ) as GiaTriBan,
    		CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
    		CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan
    		FROM
    		(
    			SELECT 
    			hd.ID_DoiTuong as ID_DoiTuong,
    			SUM(Case when (hd.LoaiHoaDon = 1 OR hd.LoaiHoaDon = 19) then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra
    			FROM
    			BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6  or hd.LoaiHoaDon = 19)
    			and hd.ChoThanhToan = 0 and hd.TongTienHang > 0
    			and hd.ID_DoiTuong is not null
    			GROUP BY hd.ID_DoiTuong
    		) a
    	) b
    	where GiaTriBan'
    	set @sql2 = @sql + @SqlQuery;
    	exec (@sql2);
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[get_MaNhanVien]");
        }
    }
}
