namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDatabase_20210602 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BH_HoaDon", "SoVuBaoHiem", c => c.Int());
            AddColumn("dbo.BH_HoaDon", "KhauTruTheoVu", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "GiamTruBoiThuong", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "PTGiamTruBoiThuong", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "TongTienThueBaoHiem", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "PTThueBaoHiem", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "PTThueHoaDon", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "BHThanhToanTruocThue", c => c.Double());
            AddColumn("dbo.BH_HoaDon", "TongTienBHDuyet", c => c.Double());
            AddColumn("dbo.BH_HoaDon_ChiTiet", "TenHangHoaThayThe", c => c.String());
            AddColumn("dbo.BH_HoaDon_ChiTiet", "DonGiaBaoHiem", c => c.Double());
            AddColumn("dbo.DM_HangHoa", "ID_Xe", c => c.Guid());
            AddColumn("dbo.DM_TaiKhoanNganHang", "TrangThai", c => c.Int());
            AddColumn("dbo.Gara_PhieuTiepNhan", "ID_BaoHiem", c => c.Guid());
            AddColumn("dbo.Gara_PhieuTiepNhan", "NguoiLienHeBH", c => c.String());
            AddColumn("dbo.Gara_PhieuTiepNhan", "SoDienThoaiLienHeBH", c => c.String(maxLength: 20));
            AddColumn("dbo.Gara_DanhMucXe", "LoaiApDung", c => c.Int());
            AddColumn("dbo.Gara_HangMucSuaChua", "Anh", c => c.String());
            CreateIndex("dbo.DM_HangHoa", "ID_Xe");
            CreateIndex("dbo.Gara_PhieuTiepNhan", "ID_BaoHiem");
            AddForeignKey("dbo.DM_HangHoa", "ID_Xe", "dbo.Gara_DanhMucXe", "ID");
            AddForeignKey("dbo.Gara_PhieuTiepNhan", "ID_BaoHiem", "dbo.DM_DoiTuong", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Gara_PhieuTiepNhan", "ID_BaoHiem", "dbo.DM_DoiTuong");
            DropForeignKey("dbo.DM_HangHoa", "ID_Xe", "dbo.Gara_DanhMucXe");
            DropIndex("dbo.Gara_PhieuTiepNhan", new[] { "ID_BaoHiem" });
            DropIndex("dbo.DM_HangHoa", new[] { "ID_Xe" });
            DropColumn("dbo.Gara_HangMucSuaChua", "Anh");
            DropColumn("dbo.Gara_DanhMucXe", "LoaiApDung");
            DropColumn("dbo.Gara_PhieuTiepNhan", "SoDienThoaiLienHeBH");
            DropColumn("dbo.Gara_PhieuTiepNhan", "NguoiLienHeBH");
            DropColumn("dbo.Gara_PhieuTiepNhan", "ID_BaoHiem");
            DropColumn("dbo.DM_TaiKhoanNganHang", "TrangThai");
            DropColumn("dbo.DM_HangHoa", "ID_Xe");
            DropColumn("dbo.BH_HoaDon_ChiTiet", "DonGiaBaoHiem");
            DropColumn("dbo.BH_HoaDon_ChiTiet", "TenHangHoaThayThe");
            DropColumn("dbo.BH_HoaDon", "TongTienBHDuyet");
            DropColumn("dbo.BH_HoaDon", "BHThanhToanTruocThue");
            DropColumn("dbo.BH_HoaDon", "PTThueHoaDon");
            DropColumn("dbo.BH_HoaDon", "PTThueBaoHiem");
            DropColumn("dbo.BH_HoaDon", "TongTienThueBaoHiem");
            DropColumn("dbo.BH_HoaDon", "PTGiamTruBoiThuong");
            DropColumn("dbo.BH_HoaDon", "GiamTruBoiThuong");
            DropColumn("dbo.BH_HoaDon", "KhauTruTheoVu");
            DropColumn("dbo.BH_HoaDon", "SoVuBaoHiem");
        }
    }
}
