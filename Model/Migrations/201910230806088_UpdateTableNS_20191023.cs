namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableNS_20191023 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NS_ChamCong_ChiTiet", "ID_DonVi", c => c.Guid(nullable: false));
            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_DonVi");
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_DonVi", "dbo.DM_DonVi", "ID", cascadeDelete: true);
            AddColumn("dbo.NS_BangLuong_ChiTiet", "DoanhSo", c => c.Double(nullable: false));
            AddColumn("dbo.NS_BangLuong_ChiTiet", "ChietKhau", c => c.Double(nullable: false));
            AddColumn("dbo.NS_CongBoSung", "Thu", c => c.Int(nullable: false));
            AlterColumn("dbo.NS_BangLuong_ChiTiet", "BaoHiemCongTyDong", c => c.Double(nullable: false));
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghi", c => c.Double(nullable: false));
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongGioOTQuyDoi", c => c.Double(nullable: false));
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongCongQuyDoi", c => c.Double(nullable: false));
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghiQuyDoi", c => c.Double(nullable: false));
            DropForeignKey("dbo.NS_KhenThuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", "dbo.NS_LoaiKhenThuong");
            DropForeignKey("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", "dbo.NS_LoaiBaoHiem");
            DropIndex("dbo.NS_KhenThuong", new[] { "ID_LoaiKhenThuong" });
            DropIndex("dbo.NS_KhenThuong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_BaoHiem", new[] { "ID_LoaiBaoHiem" });
            AddColumn("dbo.HT_CauHinhPhanMem", "ChoPhepTrungSoDienThoai", c => c.Int());
            AlterColumn("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", c => c.Guid());
            AlterColumn("dbo.NS_KhenThuong", "ID_KyTinhCong", c => c.Guid());
            AlterColumn("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", c => c.Guid());
            CreateIndex("dbo.NS_KhenThuong", "ID_LoaiKhenThuong");
            CreateIndex("dbo.NS_KhenThuong", "ID_KyTinhCong");
            CreateIndex("dbo.NS_BaoHiem", "ID_LoaiBaoHiem");
            AddForeignKey("dbo.NS_KhenThuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID");
            AddForeignKey("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", "dbo.NS_LoaiKhenThuong", "ID");
            AddForeignKey("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", "dbo.NS_LoaiBaoHiem", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_DonVi", "dbo.DM_DonVi");
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_DonVi" });
            DropColumn("dbo.NS_ChamCong_ChiTiet", "ID_DonVi");
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghiQuyDoi", c => c.Double());
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongCongQuyDoi", c => c.Double());
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongGioOTQuyDoi", c => c.Double());
            AlterColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghi", c => c.Double());
            AlterColumn("dbo.NS_BangLuong_ChiTiet", "BaoHiemCongTyDong", c => c.Double());
            DropColumn("dbo.NS_CongBoSung", "Thu");
            DropColumn("dbo.NS_BangLuong_ChiTiet", "ChietKhau");
            DropColumn("dbo.NS_BangLuong_ChiTiet", "DoanhSo");
            DropForeignKey("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", "dbo.NS_LoaiBaoHiem");
            DropForeignKey("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", "dbo.NS_LoaiKhenThuong");
            DropForeignKey("dbo.NS_KhenThuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropIndex("dbo.NS_BaoHiem", new[] { "ID_LoaiBaoHiem" });
            DropIndex("dbo.NS_KhenThuong", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_KhenThuong", new[] { "ID_LoaiKhenThuong" });
            AlterColumn("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", c => c.Guid(nullable: false));
            AlterColumn("dbo.NS_KhenThuong", "ID_KyTinhCong", c => c.Guid(nullable: false));
            AlterColumn("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", c => c.Guid(nullable: false));
            DropColumn("dbo.HT_CauHinhPhanMem", "ChoPhepTrungSoDienThoai");
            CreateIndex("dbo.NS_BaoHiem", "ID_LoaiBaoHiem");
            CreateIndex("dbo.NS_KhenThuong", "ID_KyTinhCong");
            CreateIndex("dbo.NS_KhenThuong", "ID_LoaiKhenThuong");
            AddForeignKey("dbo.NS_BaoHiem", "ID_LoaiBaoHiem", "dbo.NS_LoaiBaoHiem", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_KhenThuong", "ID_LoaiKhenThuong", "dbo.NS_LoaiKhenThuong", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_KhenThuong", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID", cascadeDelete: true);
        }
    }
}
