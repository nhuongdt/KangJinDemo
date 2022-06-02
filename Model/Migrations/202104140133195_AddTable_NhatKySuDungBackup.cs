namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTable_NhatKySuDungBackup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HT_NhatKySuDung_Backup",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_NhanVien = c.Guid(),
                        ID_DonVi = c.Guid(nullable: false),
                        ChucNang = c.String(),
                        ThoiGian = c.DateTime(nullable: false),
                        NoiDung = c.String(),
                        NoiDungChiTiet = c.String(),
                        LoaiNhatKy = c.Int(nullable: false),
                        ID_HoaDon = c.Guid(),
                        LoaiHoaDon = c.Int(),
                        ThoiGianUpdateGV = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HT_NhatKySuDung_Backup");
        }
    }
}
