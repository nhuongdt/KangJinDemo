namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20181120_1 : DbMigration
    {
        public override void Up()
        {
            
            Sql(@"ALTER PROCEDURE [dbo].[SP_Insert_ChiTietHoaDon]
    @ID [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @SoThuTu [int],
    @DonGia [float],
    @GiaVon [float],
    @ID_HoaDon [uniqueidentifier],
    @SoLuong [float],
    @ThanhTien [float],
    @ThanhToan [float],
    @PTChietKhau [float],
    @TienChietKhau [float],
    @ThoiGian [datetime],
    @GhiChu [nvarchar](max),
    @TangKem [bit],
    @ID_TangKem [uniqueidentifier],
    @ID_KhuyenMai [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @ID_ChiTietGoiDV [uniqueidentifier],
    @ID_ChiTietDinhLuong [uniqueidentifier],
    @TienThue [float],
    @PTChiPhi [float],
    @TienChiPhi [float],
    @An_Hien [bit]
AS
BEGIN
    INSERT INTO BH_HoaDon_ChiTiet(ID,ID_DonViQuiDoi,SoThuTu,DonGia,GiaVon,ID_HoaDon,SoLuong,ThanhTien,ThanhToan,
    		PTChietKhau,TienChietKhau,ThoiGian,GhiChu,TangKem,ID_TangKem,ID_KhuyenMai,ID_LoHang,ID_ChiTietGoiDV,ID_ChiTietDinhLuong, TienThue,PTChiPhi,TienChiPhi, An_Hien)
    
    		VALUES (@ID, @ID_DonViQuiDoi, @SoThuTu, @DonGia,@GiaVon,@ID_HoaDon,@SoLuong,@ThanhTien,@ThanhToan,
    		@PTChietKhau, @TienChietKhau, @ThoiGian, @GhiChu,@TangKem,@ID_TangKem,@ID_KhuyenMai,@ID_LoHang,@ID_ChiTietGoiDV,@ID_ChiTietDinhLuong,
    		@TienThue, @PTChiPhi, @TienChiPhi, @An_Hien)
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_Insert_HoaDon]
    @ID [uniqueidentifier],
    @MaHoaDon [nvarchar](max),
    @ID_HoaDon [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @ID_ViTri [uniqueidentifier],
    @NguoiTao [nvarchar](max),
    @DienGiai [nvarchar](max),
    @YeuCau [nvarchar](max),
    @ID_DoiTuong [uniqueidentifier],
    @NgayLapHoaDon [datetime],
    @PhaiThanhToan [float],
    @TongGiamGia [float],
    @TongChiPhi [float],
    @TongTienHang [float],
    @ID_DonVi [uniqueidentifier],
    @TyGia [float],
    @LoaiHoaDon [int],
    @ID_BangGia [uniqueidentifier],
    @ChoThanhToan [int],
    @TongChietKhau [float],
    @TongTienThue [float],
    @DiemGiaoDich [float],
    @ID_KhuyenMai [uniqueidentifier],
    @KhuyeMai_GiamGia [float],
    @KhuyenMai_GhiChu [nvarchar](max),
    @NgayApDungGoiDV [datetime],
    @HanSuDungGoiDV [datetime]
AS
BEGIN
    INSERT INTO BH_HoaDon (ID,MaHoaDon,ID_HoaDon,ID_NhanVien,ID_ViTri,NguoiTao,DienGiai,YeuCau,ID_DoiTuong,
    		NgayLapHoaDon,PhaiThanhToan,TongGiamGia,TongChiPhi,TongTienHang,ID_DonVi, TyGia,LoaiHoaDon,ID_BangGia,ChoThanhToan,
    		TongChietKhau,TongTienThue,DiemGiaoDich,ID_KhuyenMai,KhuyeMai_GiamGia,KhuyenMai_GhiChu,NgayApDungGoiDV,HanSuDungGoiDV, NgayTao)
    
    		VALUES (@ID, @MaHoaDon, @ID_HoaDon, @ID_NhanVien,@ID_ViTri,@NguoiTao,@DienGiai,@YeuCau,@ID_DoiTuong,
    		@NgayLapHoaDon,@PhaiThanhToan,@TongGiamGia,@TongChiPhi, @TongTienHang,@ID_DonVi, @TyGia, @LoaiHoaDon,@ID_BangGia,@ChoThanhToan,
    		@TongChietKhau,@TongTienThue,@DiemGiaoDich,@ID_KhuyenMai,@KhuyeMai_GiamGia,@KhuyenMai_GhiChu,@NgayApDungGoiDV,@HanSuDungGoiDV, GETDATE())
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_Insert_NhanVienThucHien]
    @ID_ChiTietHoaDon [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @ThucHien_TuVan [bit],
    @TienChietKhau [float],
    @PT_ChietKhau [float],
    @TheoYeuCau [bit]
AS
BEGIN
    INSERT INTO BH_NhanVienThucHien(ID,ID_ChiTietHoaDon,ID_NhanVien,ThucHien_TuVan,TienChietKhau,PT_ChietKhau,TheoYeuCau)
    
    		VALUES (NEWID(),@ID_ChiTietHoaDon, @ID_NhanVien, @ThucHien_TuVan, @TienChietKhau,@PT_ChietKhau,@TheoYeuCau)
END");

        }
        
        public override void Down()
        {
        }
    }
}
