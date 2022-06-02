namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181212 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getList_QuaTrinhCongTacbyNhanVien]", parametersAction: p => new
            {
                ID_NhanVien = p.Guid()
            }, body: @"select CAST(ROUND(ROW_NUMBER() over(order by ct.LaDonViHienThoi DESC), 0) as float) as ID, ct.ID_DonVi as ID_ChiNhanh, ct.ID_PhongBan, ct.LaDonViHienThoi as LaMacDinh,
	dv.TenDonVi as Text_ChiNhanh, pb.TenPhongBan as Text_PhongBan
	from NS_QuaTrinhCongTac ct
	join DM_DonVi dv on ct.ID_DonVi = dv.ID
	join NS_PhongBan pb on ct.ID_PhongBan = pb.ID
	where ct.ID_NhanVien = @ID_NhanVien 
	order by LaDonViHienThoi DESC");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_QuaTrinhCongTacbyNhanVien] ");
        }
    }
}