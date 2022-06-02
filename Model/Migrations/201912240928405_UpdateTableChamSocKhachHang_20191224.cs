namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableChamSocKhachHang_20191224 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChamSocKhachHangs", "ID_Parent", c => c.Guid());
            AddColumn("dbo.ChamSocKhachHangs", "KieuLap", c => c.Int());
            AddColumn("dbo.ChamSocKhachHangs", "SoLanLap", c => c.Int());
            AddColumn("dbo.ChamSocKhachHangs", "GiaTriLap", c => c.String());
            AddColumn("dbo.ChamSocKhachHangs", "TuanLap", c => c.Int());
            AddColumn("dbo.ChamSocKhachHangs", "TrangThaiKetThuc", c => c.Int());
            AddColumn("dbo.ChamSocKhachHangs", "GiaTriKetThuc", c => c.String());
            AddColumn("dbo.ChamSocKhachHangs", "KieuNhacNho", c => c.Int());
            AddColumn("dbo.ChamSocKhachHangs", "NgayCu", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChamSocKhachHangs", "NgayCu");
            DropColumn("dbo.ChamSocKhachHangs", "KieuNhacNho");
            DropColumn("dbo.ChamSocKhachHangs", "GiaTriKetThuc");
            DropColumn("dbo.ChamSocKhachHangs", "TrangThaiKetThuc");
            DropColumn("dbo.ChamSocKhachHangs", "TuanLap");
            DropColumn("dbo.ChamSocKhachHangs", "GiaTriLap");
            DropColumn("dbo.ChamSocKhachHangs", "SoLanLap");
            DropColumn("dbo.ChamSocKhachHangs", "KieuLap");
            DropColumn("dbo.ChamSocKhachHangs", "ID_Parent");
        }
    }
}
