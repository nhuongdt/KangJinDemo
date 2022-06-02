namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190703 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[UpdateKiemKeTuHoaDon]
	@IDHoaDonInput [uniqueidentifier],
	@IDChiNhanhInput [uniqueidentifier],
    @ThoiGian [datetime]
AS
BEGIN
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

    	DECLARE @ChiTietHoaDonUpdate TABLE (IDHoaDon UNIQUEIDENTIFIER,ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, 
    	TyLeChuyenDoi FLOAT, TonKho FLOAT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER);
    	INSERT INTO @ChiTietHoaDonUpdate
    	select hdupdate.ID AS ID_HoaDon, hdctupdate.ID AS ID_ChiTietHoaDon, 
		hdupdate.NgayLapHoaDon, hdctupdate.SoThuTu, hdctupdate.SoLuong, dvqdupdate.TyLeChuyenDoi,
    	[dbo].[FUNC_TonLuyKeTruocThoiGian](cthdthemmoiupdate.ID_ChiNhanh, hhupdate.ID, hdctupdate.ID_LoHang, hdupdate.NgayLapHoaDon) AS TonKho, dvqdupdate.ID, hdctupdate.ID_LoHang
		FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate
    	ON hdupdate.ID = hdctupdate.ID_HoaDon
    	INNER JOIN DonViQuiDoi dvqdupdate
    	ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID
    	INNER JOIN DM_HangHoa hhupdate
    	on hhupdate.ID = dvqdupdate.ID_HangHoa
    	INNER JOIN @ChiTietHoaDon cthdthemmoiupdate
    	ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID
    	WHERE hdupdate.ChoThanhToan = 0 AND hdupdate.LoaiHoaDon = 9 AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) AND
    	hdupdate.ID_DonVi = cthdthemmoiupdate.ID_ChiNhanh and hdupdate.NgayLapHoaDon >= cthdthemmoiupdate.NgayLapHoaDon

  --  	--Update Kiem ke
		UPDATE ctkiemke
    	SET ctkiemke.TienChietKhau = ctupdatekk.TonKho / ctupdatekk.TyLeChuyenDoi, ctkiemke.SoLuong = ctkiemke.ThanhTien - (ctupdatekk.TonKho /ctupdatekk.TyLeChuyenDoi)
    	FROM BH_HoaDon_ChiTiet ctkiemke
    	INNER JOIN @ChiTietHoaDonUpdate as ctupdatekk
    	on ctkiemke.ID = ctupdatekk.ID_ChiTietHoaDon
    
    	UPDATE hdkkupdate
    	SET hdkkupdate.TongTienHang = dshoadonkkupdate.SoLuongGiam, hdkkupdate.TongGiamGia = dshoadonkkupdate.SoLuongLech, hdkkupdate.TongChiPhi = dshoadonkkupdate.SoLuongTang
    	FROM BH_HoaDon AS hdkkupdate
    	INNER JOIN
    	(SELECT ct.ID_HoaDon, SUM(CASE WHEN ct.SoLuong > 0 THEN ct.SoLuong ELSE 0 END) AS SoLuongTang,
    	SUM(CASE WHEN ct.SoLuong < 0 THEN ct.SoLuong ELSE 0 END) AS SoLuongGiam, SUM(SoLuong) AS SoLuongLech FROM BH_HoaDon_ChiTiet ct
    	INNER JOIN (SELECT IDHoaDon, IDDonViQuiDoi, ID_LoHang FROM @ChiTietHoaDonUpdate) AS KKHoaDon
    	ON ct.ID_HoaDon = KKHoaDon.IDHoaDon AND ct.ID_DonViQuiDoi = KKHoaDon.IDDonViQuiDoi AND (ct.ID_LoHang = KKHoaDon.ID_LoHang OR KKHoaDon.ID_LoHang IS NULL) GROUP BY ct.ID_HoaDon) AS dshoadonkkupdate
    	ON hdkkupdate.ID = dshoadonkkupdate.ID_HoaDon;
END");
        }
        
        public override void Down()
        {
        }
    }
}
