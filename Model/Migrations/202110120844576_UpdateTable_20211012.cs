namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_20211012 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DinhLuongDichVu", "DonGia", c => c.Double());
            AddColumn("dbo.DinhLuongDichVu", "ID_LoHang", c => c.Guid());
            AddColumn("dbo.HT_CongTy", "ZaloAccessToken", c => c.String());
            AddColumn("dbo.HT_CongTy", "ZaloRefreshToken", c => c.String());
            AddColumn("dbo.HT_CongTy", "EmailAccount", c => c.String());
            AddColumn("dbo.HT_CongTy", "EmailPassword", c => c.String());
            AddColumn("dbo.HT_CongTy", "ZaloCodeVerifier", c => c.String());
            AddColumn("dbo.Quy_KhoanThuChi", "TrangThai", c => c.Int());
            CreateIndex("dbo.DinhLuongDichVu", "ID_LoHang");
            AddForeignKey("dbo.DinhLuongDichVu", "ID_LoHang", "dbo.DM_LoHang", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DinhLuongDichVu", "ID_LoHang", "dbo.DM_LoHang");
            DropIndex("dbo.DinhLuongDichVu", new[] { "ID_LoHang" });
            DropColumn("dbo.Quy_KhoanThuChi", "TrangThai");
            DropColumn("dbo.HT_CongTy", "ZaloCodeVerifier");
            DropColumn("dbo.HT_CongTy", "EmailPassword");
            DropColumn("dbo.HT_CongTy", "EmailAccount");
            DropColumn("dbo.HT_CongTy", "ZaloRefreshToken");
            DropColumn("dbo.HT_CongTy", "ZaloAccessToken");
            DropColumn("dbo.DinhLuongDichVu", "ID_LoHang");
            DropColumn("dbo.DinhLuongDichVu", "DonGia");
        }
    }
}
