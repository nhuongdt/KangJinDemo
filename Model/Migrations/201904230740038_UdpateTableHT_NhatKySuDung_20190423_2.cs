namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UdpateTableHT_NhatKySuDung_20190423_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HT_NhatKySuDung", "ID_HoaDon", c => c.Guid());
            AddColumn("dbo.HT_NhatKySuDung", "LoaiHoaDon", c => c.Int());
            AddColumn("dbo.HT_NhatKySuDung", "ThoiGianUpdateGV", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HT_NhatKySuDung", "ThoiGianUpdateGV");
            DropColumn("dbo.HT_NhatKySuDung", "LoaiHoaDon");
            DropColumn("dbo.HT_NhatKySuDung", "ID_HoaDon");
        }
    }
}
