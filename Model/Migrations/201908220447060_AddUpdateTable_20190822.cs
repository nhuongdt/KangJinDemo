namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateTable_20190822 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.NS_PhieuPhanCa_ChiTiet", newName: "NS_PhieuPhanCa_NhanVien");
            DropForeignKey("dbo.NS_PhanCaChiTiet", "ID_PhieuPhanCaChiTiet", "dbo.NS_PhieuPhanCa_ChiTiet");
            DropForeignKey("dbo.NS_PhanCaChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropIndex("dbo.NS_PhanCaChiTiet", new[] { "ID_PhieuPhanCaChiTiet" });
            DropIndex("dbo.NS_PhanCaChiTiet", new[] { "ID_CaLamViec" });
            CreateTable(
                "dbo.NS_PhieuPhanCa_CaLamViec",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_PhieuPhanCa = c.Guid(nullable: false),
                        ID_CaLamViec = c.Guid(nullable: false),
                        GiaTri = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_PhieuPhanCa", t => t.ID_PhieuPhanCa, cascadeDelete: true)
                .ForeignKey("dbo.NS_CaLamViec", t => t.ID_CaLamViec, cascadeDelete: true)
                .Index(t => t.ID_PhieuPhanCa)
                .Index(t => t.ID_CaLamViec);
            
            AddColumn("dbo.HT_NguoiDung", "SoDuTaiKhoan", c => c.Double());
            AddColumn("dbo.HeThong_SMS_TaiKhoan", "TrangThai", c => c.Int(nullable: false));
            DropColumn("dbo.NS_PhieuPhanCa_NhanVien", "GhiChu");
            DropTable("dbo.NS_PhanCaChiTiet");

            Sql(@"ALTER PROCEDURE [dbo].[insert_SaoChepCaiDatHoaHong]
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @ID_NhanVien_new [nvarchar](max),
    @PhuongThuc [int]
AS
BEGIN
    if (@PhuongThuc = 1) -- update hàng hóa đã cài đặt
    	Begin
    		update ChietKhauMacDinh_NhanVien Set
    		ChietKhau = src.ChietKhau, LaPhanTram = src.LaPhanTram, ChietKhau_TuVan = src.ChietKhau_TuVan, LaPhanTram_TuVan = src.LaPhanTram_TuVan, ChietKhau_YeuCau = src.ChietKhau_YeuCau, LaPhanTram_YeuCau = src.LaPhanTram_YeuCau,
			LaPhanTram_BanGoi = src.LaPhanTram_BanGoi, ChietKhau_BanGoi = src.ChietKhau_BanGoi, TheoChietKhau_ThucHien = src.TheoChietKhau_ThucHien
    		From ChietKhauMacDinh_NhanVien cknv inner join 
    		(
    			select b.ID,b.ID_NhanVien, b.ID_DonVi, b.ChietKhau, b.LaPhanTram, b.ChietKhau_YeuCau, b.LaPhanTram_YeuCau, b.ChietKhau_TuVan, b.LaPhanTram_TuVan,b.NgayNhap, b.ID_DonViQuiDoi,
				b.LaPhanTram_BanGoi, b.ChietKhau_BanGoi, b.TheoChietKhau_ThucHien,
				 ck.ID as ID_update
    			FROM
    			(
    				select a.ID, a.ID_DonVi, Name as ID_NhanVien, ChietKhau, LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan, ID_DonViQuiDoi, a.NgayNhap,
					 a.LaPhanTram_BanGoi,a.ChietKhau_BanGoi,a.TheoChietKhau_ThucHien
    				from splitstring(@ID_NhanVien_new)
    				Cross join
    				(
    				select NEWID() as ID, ID_DonVi, ChietKhau,
    				LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan, ID_DonViQuiDoi, NgayNhap, LaPhanTram_BanGoi, ChietKhau_BanGoi,TheoChietKhau_ThucHien
    				from ChietKhauMacDinh_NhanVien where ID_NhanVien = @ID_NhanVien
    				) as a
    			) as b
    			left join ChietKhauMacDinh_NhanVien ck on b.ID_DonVi = ck.ID_DonVi and b.ID_NhanVien = ck.ID_NhanVien and b.ID_DonViQuiDoi = ck.ID_DonViQuiDoi
    			where ck.ID is not null and b.ID_DonVi= @ID_DonVi
    		) src
    		on cknv.ID = src.ID_update
    	End
    
    	INSERT INTO ChietKhauMacDinh_NhanVien (ID, ID_NhanVien, ID_DonVi, ChietKhau, LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan, ID_DonViQuiDoi, NgayNhap, ChietKhau_BanGoi, LaPhanTram_BanGoi, TheoChietKhau_ThucHien)
    	select b.ID,b.ID_NhanVien, b.ID_DonVi, b.ChietKhau, b.LaPhanTram, b.ChietKhau_YeuCau, b.LaPhanTram_YeuCau, b.ChietKhau_TuVan, b.LaPhanTram_TuVan, b.ID_DonViQuiDoi, b.NgayNhap,b.ChietKhau_BanGoi, b.LaPhanTram_BanGoi,  b.TheoChietKhau_ThucHien
    	FROM
    	(
    		select a.ID, a.ID_DonVi, Name as ID_NhanVien, ChietKhau, LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan, ID_DonViQuiDoi, NgayNhap, ChietKhau_BanGoi, LaPhanTram_BanGoi, TheoChietKhau_ThucHien
    		from splitstring(@ID_NhanVien_new)
    		Cross join
    		(
    		select NEWID() as ID,ID_DonVi, ChietKhau,
    		LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan, ID_DonViQuiDoi, NgayNhap,LaPhanTram_BanGoi, ChietKhau_BanGoi, TheoChietKhau_ThucHien
    		from ChietKhauMacDinh_NhanVien where ID_NhanVien = @ID_NhanVien --and ID_DonVi= @ID_DonVi
    		) as a
    	) as b
    	left join ChietKhauMacDinh_NhanVien ck on b.ID_DonVi = ck.ID_DonVi and b.ID_NhanVien = ck.ID_NhanVien and b.ID_DonViQuiDoi = ck.ID_DonViQuiDoi
    	where b.ID_DonVi= @ID_DonVi and ck.ID is null
    	order by b.ID_NhanVien
END");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NS_PhanCaChiTiet",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ID_PhieuPhanCaChiTiet = c.Guid(nullable: false),
                        ID_CaLamViec = c.Guid(nullable: false),
                        GiaTri = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.NS_PhieuPhanCa_NhanVien", "GhiChu", c => c.String());
            DropForeignKey("dbo.NS_PhieuPhanCa_CaLamViec", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.NS_PhieuPhanCa_CaLamViec", "ID_PhieuPhanCa", "dbo.NS_PhieuPhanCa");
            DropIndex("dbo.NS_PhieuPhanCa_CaLamViec", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_PhieuPhanCa_CaLamViec", new[] { "ID_PhieuPhanCa" });
            DropColumn("dbo.HeThong_SMS_TaiKhoan", "TrangThai");
            DropColumn("dbo.HT_NguoiDung", "SoDuTaiKhoan");
            DropTable("dbo.NS_PhieuPhanCa_CaLamViec");
            CreateIndex("dbo.NS_PhanCaChiTiet", "ID_CaLamViec");
            CreateIndex("dbo.NS_PhanCaChiTiet", "ID_PhieuPhanCaChiTiet");
            AddForeignKey("dbo.NS_PhanCaChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_PhanCaChiTiet", "ID_PhieuPhanCaChiTiet", "dbo.NS_PhieuPhanCa_ChiTiet", "ID", cascadeDelete: true);
            RenameTable(name: "dbo.NS_PhieuPhanCa_NhanVien", newName: "NS_PhieuPhanCa_ChiTiet");
        }
    }
}
