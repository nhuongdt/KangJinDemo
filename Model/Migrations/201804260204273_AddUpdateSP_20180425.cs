namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180425 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getListNhanVien_allDonVi]", parametersAction: p => new
            {
                ID_ChiNhanh = p.String()
            }, body: @"Select 
	nv.ID,
	nv.MaNhanVien,
    nv.TenNhanVien
	From NS_NhanVien nv
	inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	where ct.ID_DonVi in (Select * from splitstring(@ID_ChiNhanh))
	GROUP by nv.ID, nv.MaNhanVien, nv.TenNhanVien");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getListNhanVien_allDonVi]");
        }
    }
}
