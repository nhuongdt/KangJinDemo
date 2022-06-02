namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableQuy_HoaDon_ChiTiet_20210226 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Quy_HoaDon_ChiTiet", "HinhThucThanhToan", c => c.Int());
            AddColumn("dbo.Quy_HoaDon_ChiTiet", "LoaiThanhToan", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Quy_HoaDon_ChiTiet", "LoaiThanhToan");
            DropColumn("dbo.Quy_HoaDon_ChiTiet", "HinhThucThanhToan");
        }
    }
}
