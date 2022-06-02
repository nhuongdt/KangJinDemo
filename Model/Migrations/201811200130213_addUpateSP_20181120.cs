namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpateSP_20181120 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SP_GettAll_BangGiaChiTiet]", body: @"select gbct.ID,gbct.GiaBan, gbct.ID_DonViQuiDoi as IDQuyDoi, gbct.ID_GiaBan, hh.ID as ID_HangHoa
	from DM_GiaBan_ChiTiet gbct
	left join DonViQuiDoi qd on gbct.ID_DonViQuiDoi= qd.ID
	left join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	where qd.Xoa is null OR qd.Xoa='0'
	order by gbct.ID");

            CreateStoredProcedure(name: "[dbo].[SP_Insert_ChiTietHoaDon]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                SoThuTu = p.Int(),
                DonGia = p.Double(),
                GiaVon = p.Double(),
                ID_HoaDon = p.Guid(),
                SoLuong = p.Double(),
                ThanhTien = p.Double(),
                ThanhToan = p.Double(),
                PTChietKhau = p.Double(),
                TienChietKhau = p.Double(),
                ThoiGian = p.DateTime(),
                GhiChu = p.String(),
                TangKem = p.Boolean(),
                ID_TangKem = p.Guid(),
                ID_KhuyenMai = p.Guid(),
                ID_LoHang = p.Guid(),
                ID_ChiTietGoiDV = p.Guid(),
                ID_ChiTietDinhLuong = p.Guid(),
                TienThue = p.Double(),
                PTChiPhi = p.Double(),
                TienChiPhi = p.Double(),
                An_Hien = p.Boolean()
            }, body: @"INSERT INTO BH_HoaDon_ChiTiet(ID,ID_DonViQuiDoi,SoThuTu,DonGia,GiaVon,ID_HoaDon,SoLuong,ThanhTien,ThanhToan,
		PTChietKhau,TienChietKhau,ThoiGian,GhiChu,TangKem,ID_TangKem,ID_KhuyenMai,ID_LoHang,ID_ChiTietGoiDV,ID_ChiTietDinhLuong, TienThue,PTChiPhi,TienChiPhi, An_Hien)

		VALUES (@ID, @ID_DonViQuiDoi, @SoThuTu, @DonGia,@GiaVon,@ID_HoaDon,@SoLuong,@ThanhTien,@ThanhToan,
		@PTChietKhau, @TienChietKhau, @ThoiGian, @GhiChu,@TangKem,@ID_TangKem,@ID_KhuyenMai,@ID_LoHang,@ID_ChiTietGoiDV,@ID_ChiTietDinhLuong,
		@TienThue, @PTChiPhi, @TienChiPhi, @An_Hien)");

            CreateStoredProcedure(name: "[dbo].[SP_Insert_HoaDon]", parametersAction: p => new
            {
                ID = p.Guid(),
                MaHoaDon = p.String(),
                ID_HoaDon = p.Guid(),
                ID_NhanVien = p.Guid(),
                ID_ViTri = p.Guid(),
                NguoiTao = p.String(),
                DienGiai = p.String(),
                YeuCau = p.String(),
                ID_DoiTuong = p.Guid(),
                NgayLapHoaDon = p.DateTime(),
                PhaiThanhToan = p.Double(),
                TongGiamGia = p.Double(),
                TongChiPhi = p.Double(),
                TongTienHang = p.Double(),
                ID_DonVi = p.Guid(),
                TyGia = p.Double(),
                LoaiHoaDon = p.Int(),
                ID_BangGia = p.Guid(),
                ChoThanhToan = p.Int(),
                TongChietKhau = p.Double(),
                TongTienThue = p.Double(),
                DiemGiaoDich = p.Double(),
                ID_KhuyenMai = p.Guid(),
                KhuyeMai_GiamGia = p.Double(),
                KhuyenMai_GhiChu = p.String(),
                NgayApDungGoiDV = p.DateTime(),
                HanSuDungGoiDV = p.DateTime()
            }, body: @"INSERT INTO BH_HoaDon (ID,MaHoaDon,ID_HoaDon,ID_NhanVien,ID_ViTri,NguoiTao,DienGiai,YeuCau,ID_DoiTuong,
		NgayLapHoaDon,PhaiThanhToan,TongGiamGia,TongChiPhi,TongTienHang,ID_DonVi, TyGia,LoaiHoaDon,ID_BangGia,ChoThanhToan,
		TongChietKhau,TongTienThue,DiemGiaoDich,ID_KhuyenMai,KhuyeMai_GiamGia,KhuyenMai_GhiChu,NgayApDungGoiDV,HanSuDungGoiDV, NgayTao)

		VALUES (@ID, @MaHoaDon, @ID_HoaDon, @ID_NhanVien,@ID_ViTri,@NguoiTao,@DienGiai,@YeuCau,@ID_DoiTuong,
		@NgayLapHoaDon,@PhaiThanhToan,@TongGiamGia,@TongChiPhi, @TongTienHang,@ID_DonVi, @TyGia, @LoaiHoaDon,@ID_BangGia,@ChoThanhToan,
		@TongChietKhau,@TongTienThue,@DiemGiaoDich,@ID_KhuyenMai,@KhuyeMai_GiamGia,@KhuyenMai_GhiChu,@NgayApDungGoiDV,@HanSuDungGoiDV, GETDATE())");

            CreateStoredProcedure(name: "[dbo].[SP_Insert_NhanVienThucHien]", parametersAction: p => new
            {
                ID_ChiTietHoaDon = p.Guid(),
                ID_NhanVien = p.Guid(),
                ThucHien_TuVan = p.Boolean(),
                TienChietKhau = p.Double(),
                PT_ChietKhau = p.Double(),
                TheoYeuCau = p.Boolean()
            }, body: @"INSERT INTO BH_NhanVienThucHien(ID,ID_ChiTietHoaDon,ID_NhanVien,ThucHien_TuVan,TienChietKhau,PT_ChietKhau,TheoYeuCau)

		VALUES (NEWID(),@ID_ChiTietHoaDon, @ID_NhanVien, @ThucHien_TuVan, @TienChietKhau,@PT_ChietKhau,@TheoYeuCau)");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GettAll_BangGiaChiTiet]");
            DropStoredProcedure("[dbo].[SP_Insert_ChiTietHoaDon]");
            DropStoredProcedure("[dbo].[SP_Insert_HoaDon]");
            DropStoredProcedure("[dbo].[SP_Insert_NhanVienThucHien]");
        }
    }
}
