namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateTable_20210719 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DM_HangHoa_BaoDuongChiTiet",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_HangHoa = c.Guid(nullable: false),
                        LanBaoDuong = c.Int(nullable: false),
                        GiaTri = c.Double(nullable: false),
                        LoaiGiaTri = c.Int(nullable: false),
                        BaoDuongLapDinhKy = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa, cascadeDelete: true)
                .Index(t => t.ID_HangHoa);
            
            CreateTable(
                "dbo.Gara_LichBaoDuong",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_HangHoa = c.Guid(nullable: false),
                        ID_HoaDon = c.Guid(nullable: false),
                        LanBaoDuong = c.Int(nullable: false),
                        SoKmBaoDuong = c.Int(nullable: false),
                        NgayBaoDuongDuKien = c.DateTime(nullable: false),
                        NgayTao = c.DateTime(nullable: false),
                        TrangThai = c.Int(nullable: false),
                        ID_Xe = c.Guid(),
                        GhiChu = c.String(maxLength: 4000),
                        LanNhac = c.Int(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DM_HangHoa", t => t.ID_HangHoa, cascadeDelete: true)
                .ForeignKey("dbo.BH_HoaDon", t => t.ID_HoaDon, cascadeDelete: true)
                .ForeignKey("dbo.Gara_DanhMucXe", t=> t.ID_Xe, cascadeDelete: true)
                .Index(t => t.ID_HangHoa)
                .Index(t => t.ID_HoaDon)
                .Index(t => t.ID_Xe);

            CreateTable(
                "dbo.HT_ThongBao_CatDatThoiGian",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    LoaiThongBao = c.Int(nullable: false),
                    NhacTruocThoiGian = c.Int(nullable: false),
                    NhacTruocLoaiThoiGian = c.Int(nullable: false),
                    SoLanLapLai = c.Int(nullable: false),
                    LoaiThoiGianLapLai = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.ID);

            AddColumn("dbo.BH_HoaDon_ChiTiet", "ID_LichBaoDuong", c => c.Guid());
            AddColumn("dbo.BH_HoaDon_ChiTiet", "ID_ParentCombo", c => c.Guid());
            AddColumn("dbo.DM_HangHoa", "LoaiHangHoa", c => c.Int());
            AddColumn("dbo.DM_HangHoa", "QuanLyBaoDuong", c => c.Int());
            AddColumn("dbo.DM_HangHoa", "LoaiBaoDuong", c => c.Int());
            AddColumn("dbo.DM_HangHoa", "SoKmBaoHanh", c => c.Int());
            AddColumn("dbo.HT_ThongBao_CaiDat", "LichHen", c => c.Int());
            AddColumn("dbo.HT_ThongBao_CaiDat", "GoiDichVu", c => c.Int());
            AddColumn("dbo.HT_ThongBao_CaiDat", "TheGiaTri", c => c.Int());
            CreateIndex("dbo.BH_HoaDon_ChiTiet", "ID_LichBaoDuong");
            AddForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_LichBaoDuong", "dbo.Gara_LichBaoDuong", "ID");
            AddColumn("dbo.BH_HoaDon", "ID_Xe", c => c.Guid());
            CreateIndex("dbo.BH_HoaDon", "ID_Xe");
            AddForeignKey("dbo.BH_HoaDon", "ID_Xe", "dbo.Gara_DanhMucXe", "ID");
            AddColumn("dbo.HT_ThongBao_CaiDat", "BaoDuongXe", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gara_LichBaoDuong", "ID_HoaDon", "dbo.BH_HoaDon");
            DropForeignKey("dbo.Gara_LichBaoDuong", "ID_HangHoa", "dbo.DM_HangHoa");
            DropForeignKey("dbo.Gara_LichBaoDuong", "ID_Xe", "dbo.Gara_DanhMucXe");
            DropForeignKey("dbo.BH_HoaDon_ChiTiet", "ID_LichBaoDuong", "dbo.Gara_LichBaoDuong");
            DropForeignKey("dbo.DM_HangHoa_BaoDuongChiTiet", "ID_HangHoa", "dbo.DM_HangHoa");
            DropIndex("dbo.Gara_LichBaoDuong", new[] { "ID_HoaDon" });
            DropIndex("dbo.Gara_LichBaoDuong", new[] { "ID_HangHoa" });
            DropIndex("dbo.Gara_LichBaoDuong", new[] { "ID_Xe" });
            DropIndex("dbo.DM_HangHoa_BaoDuongChiTiet", new[] { "ID_HangHoa" });
            DropIndex("dbo.BH_HoaDon_ChiTiet", new[] { "ID_LichBaoDuong" });
            DropColumn("dbo.HT_ThongBao_CaiDat", "TheGiaTri");
            DropColumn("dbo.HT_ThongBao_CaiDat", "GoiDichVu");
            DropColumn("dbo.HT_ThongBao_CaiDat", "LichHen");
            DropColumn("dbo.DM_HangHoa", "SoKmBaoHanh");
            DropColumn("dbo.DM_HangHoa", "LoaiBaoDuong");
            DropColumn("dbo.DM_HangHoa", "QuanLyBaoDuong");
            DropColumn("dbo.DM_HangHoa", "LoaiHangHoa");
            DropColumn("dbo.BH_HoaDon_ChiTiet", "ID_ParentCombo");
            DropColumn("dbo.BH_HoaDon_ChiTiet", "ID_LichBaoDuong");
            DropTable("dbo.Gara_LichBaoDuong");
            DropTable("dbo.DM_HangHoa_BaoDuongChiTiet");
            DropForeignKey("dbo.BH_HoaDon", "ID_Xe", "dbo.Gara_DanhMucXe");
            DropIndex("dbo.BH_HoaDon", new[] { "ID_Xe" });
            DropColumn("dbo.BH_HoaDon", "ID_Xe");
            DropColumn("dbo.HT_ThongBao_CaiDat", "BaoDuongXe");
            DropTable("dbo.HT_ThongBao_CatDatThoiGian");
        }
    }
}
