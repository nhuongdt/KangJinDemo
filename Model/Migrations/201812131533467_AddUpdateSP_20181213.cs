namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181213 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getListID_NhanVienDS]", parametersAction: p => new
            {
                ID_NguoiDung = p.Guid()
            }, body: @"Select DISTINCT ctpb.ID_NhanVien from
	HT_NguoiDung nd 
	join NS_NhanVien nv on nd.ID_NhanVien = nv.ID
	join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_PhongBan pbc on ct.ID_PhongBan = pbc.ID_PhongBanCha
	left join NS_PhongBan pbch on pbc.ID = pbch.ID_PhongBanCha
	left join NS_QuaTrinhCongTac ctpb on (ct.ID_NhanVien = ctpb.ID_NhanVien or pbc.ID = ctpb.ID_PhongBan or pbch.ID = ctpb.ID_PhongBan)
	where nd.ID = @ID_NguoiDung");

            CreateStoredProcedure(name: "[dbo].[getListID_NhanVienPhongBan]", parametersAction: p => new
            {
                ID_NguoiDung = p.Guid()
            }, body: @"Select DISTINCT ctpb.ID_NhanVien from
	HT_NguoiDung nd 
	join NS_NhanVien nv on nd.ID_NhanVien = nv.ID
	join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
	left join NS_PhongBan pbc on ct.ID_PhongBan = pbc.ID_PhongBanCha
	left join NS_PhongBan pbch on pbc.ID = pbch.ID_PhongBanCha
	left join NS_QuaTrinhCongTac ctpb on (ct.ID_PhongBan = ctpb.ID_PhongBan or pbc.ID = ctpb.ID_PhongBan or pbch.ID = ctpb.ID_PhongBan)
	where nd.ID = @ID_NguoiDung");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getListID_NhanVienDS]");
            DropStoredProcedure("[dbo].[getListID_NhanVienPhongBan]");
        }
    }
}
