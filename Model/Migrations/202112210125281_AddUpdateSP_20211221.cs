namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20211221 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Gara_DanhMucXe", "BienSo", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Gara_DanhMucXe", "BienSo", c => c.String(maxLength: 20));
        }
    }
}
