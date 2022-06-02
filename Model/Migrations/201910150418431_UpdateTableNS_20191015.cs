namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableNS_20191015 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NS_BangLuong_ChiTiet", "BaoHiemCongTyDong", c => c.Double());
            AddColumn("dbo.NS_BangLuong", "LaBangLuongBoSung", c => c.Boolean(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghi", c => c.Double());
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongGioOTQuyDoi", c => c.Double());
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongCongQuyDoi", c => c.Double());
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghiQuyDoi", c => c.Double());
            AddColumn("dbo.NS_CongBoSung", "CongQuyDoi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_CongBoSung", "LoaiNgay", c => c.Int(nullable: false));
            AddColumn("dbo.NS_CongBoSung", "GioOTQuyDoi", c => c.Double(nullable: false));
            AddColumn("dbo.NS_KhenThuong", "SoTien", c => c.Double());
            AddColumn("dbo.NS_LoaiBaoHiem", "GhiChu", c => c.String());
            AlterColumn("dbo.NS_Luong_PhuCap", "LoaiLuong", c => c.Int());


            Sql(@"ALTER PROCEDURE [dbo].[SP_GetInfor_TPDinhLuong]
    @ID_DonVi [nvarchar](max),
    @ID_DichVu [nvarchar](max)
AS
BEGIN
	set nocount on;
	select dl.ID, dl.ID_DichVu, dl.ID_DonViQuiDoi,TenHangHoa,qd.MaHangHoa,
		CAST(dl.SoLuong as float) as SoLuong,	
		-- used to first load
		CAST(dl.SoLuong as float) as SoLuongMacDinh,
		-- used to save in CTHD
		CAST(dl.SoLuong as float) as SoLuongDinhLuong_BanDau,
		case when QuyCach is null or QuyCach <=1 then 1
		else QuyCach * qd.TyLeChuyenDoi end as QuyCach,
		-- SoLuongQuyCach = SoLuong * QuyCach * TyLeChuyenDoi
		case when QuyCach is null or QuyCach <=1 then dl.SoLuong
		else dl.SoLuong * QuyCach * qd.TyLeChuyenDoi end as SoLuongQuyCach,
		CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
		CAST(ISNULL(gv.GiaVon,0) as float) as GiaVon,
		CAST(ISNULL(tk.TonKho,0) as float) as TonKho,								
		ISNULL(hh.DonViTinhQuyCach,'') as DonViTinhQuyCach,
		ISNULL(qd.TenDonViTinh,'') as TenDonViTinh
	from DinhLuongDichVu dl
	join DonViQuiDoi qd on dl.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
	left join DM_GiaVon gv on dl.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_DonVi
	left join DM_HangHoa_TonKho tk on qd.ID = tk.ID_DonViQuyDoi and  tk.ID_DonVi = @ID_DonVi
	where dl.ID_DichVu like @ID_DichVu	and qd.Xoa='0'
END");

        }
        
        public override void Down()
        {
            AlterColumn("dbo.NS_Luong_PhuCap", "LoaiLuong", c => c.Int(nullable: false));
            DropColumn("dbo.NS_LoaiBaoHiem", "GhiChu");
            DropColumn("dbo.NS_KhenThuong", "SoTien");
            DropColumn("dbo.NS_CongBoSung", "GioOTQuyDoi");
            DropColumn("dbo.NS_CongBoSung", "LoaiNgay");
            DropColumn("dbo.NS_CongBoSung", "CongQuyDoi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghiQuyDoi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongCongQuyDoi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongGioOTQuyDoi");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TongCongOTNgayNghi");
            DropColumn("dbo.NS_BangLuong", "LaBangLuongBoSung");
            DropColumn("dbo.NS_BangLuong_ChiTiet", "BaoHiemCongTyDong");
        }
    }
}
