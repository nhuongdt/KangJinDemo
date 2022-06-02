namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_20210630 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BH_HoaDon", "TongThueKhachHang", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "CongThucBaoHiem", c => c.Int());
            AddColumn("dbo.BH_HoaDon", "GiamTruThanhToanBaoHiem", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BH_HoaDon", "GiamTruThanhToanBaoHiem");
            DropColumn("dbo.BH_HoaDon", "CongThucBaoHiem");
            DropColumn("dbo.BH_HoaDon", "TongThueKhachHang");
        }
    }
}
