namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStoreProcedure_20180330 : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure(name: "[dbo].[insert_HoaDon_ChiTiet]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                SoLuong = p.Double(),
                DonGia = p.Double(),
                ThanhTien = p.Double(),
                GiaVon = p.Double()
            }, body: @"DECLARE @SoThuTu  as int
    SET @SoThuTu =  (Select Max(SoThuTu) FROM BH_HoaDon_ChiTiet where ID_HoaDon = @ID_HoaDon);
		if (@SoThuTu is null)
		Begin
			SET @SoThuTu = 1;
		End
		else
		Begin
			SET @SoThuTu = @SoThuTu + 1;
		End
    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi)
    Values (NEWID(), @ID_HoaDon, @SoThuTu, @SoLuong, @DonGia, @ThanhTien, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi)");
        }
        
        public override void Down()
        {

        }
    }
}