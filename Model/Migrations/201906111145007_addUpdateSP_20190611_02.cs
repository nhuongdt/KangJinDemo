namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20190611_02 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER TRIGGER [dbo].[CallUpdateGiaVon] on [dbo].[HT_NhatKySuDung]
FOR INSERT
AS
BEGIN
	--DECLARE @IDHoaDonInput UNIQUEIDENTIFIER;
	--DECLARE @IDChiNhanhInput UNIQUEIDENTIFIER;
	--DECLARE @ThoiGian DATETIME;
	--DECLARE @LoaiHoaDon INT;
	--DECLARE @YeuCau NVARCHAR(MAX);
	--DECLARE @ChoThanhToan BIT;

	--DECLARE @ID_LoHang UNIQUEIDENTIFIER;
	--DECLARE @IDCheckIn UNIQUEIDENTIFIER;
	--DECLARE @ThoiGianChyen DATETIME;

	DECLARE @LoaiNhatKy INT;
	DECLARE @ChucNang NVARCHAR(MAX);

	SELECT @LoaiNhatKy = LoaiNhatKy, @ChucNang = ChucNang FROM inserted
	--SELECT @LoaiHoaDon = LoaiHoaDon, @IDHoaDonInput = ID_HoaDon, @ThoiGian = ThoiGianUpdateGV, @LoaiNhatKy = LoaiNhatKy, @ChucNang = ChucNang FROM inserted
	--IF(@IDHoaDonInput is not null)
	--BEGIN
	------Update Giá vốn
	--SELECT @IDChiNhanhInput = ID_DonVi,@IDCheckIn = ID_CheckIn, @ThoiGianChyen = NgayLapHoaDon , @YeuCau = YeuCau, @ChoThanhToan = ChoThanhToan FROM BH_HoaDon WHERE ID = @IDHoaDonInput
	--	IF(@LoaiHoaDon = 1 OR @LoaiHoaDon = 4 OR @LoaiHoaDon = 6 OR @LoaiHoaDon = 7 OR @LoaiHoaDon = 8 OR @LoaiHoaDon = 9 OR @LoaiHoaDon = 18)
	--	BEGIN
	--		exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian
	--	END
	--	IF(@LoaiHoaDon = 10)
	--	BEGIN
	--		IF(@YeuCau = '1')
	--		BEGIN
	--			exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGian
	--		END
	--		IF(@YeuCau = '4' OR @YeuCau = '3')
	--		BEGIN
	--			SELECT @ThoiGian = NgaySua FROM BH_HoaDon WHERE ID = @IDHoaDonInput
	--			exec UpdateGiaVonVer2 @IDHoaDonInput, @IDCheckIn, @ThoiGian
	--			exec UpdateGiaVonVer2 @IDHoaDonInput, @IDChiNhanhInput, @ThoiGianChyen
	--		END
	--	END
	
	--exec UpdateTonForDM_hangHoa_TonKho @LoaiNhatKy, @LoaiHoaDon,@IDHoaDonInput, @ThoiGian, @ChoThanhToan, @IDChiNhanhInput, @IDCheckIn, @YeuCau;
	----End update giavon
	--END

	IF(@LoaiNhatKy = 5 and @ChucNang like N'Danh mục hàng hóa')
	BEGIN
		exec update_DanhMucHangHoa
	END

END");
        }
        
        public override void Down()
        {
        }
    }
}
