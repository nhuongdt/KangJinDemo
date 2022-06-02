namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190112 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetListChiTietHoaDonKiemKhoXuatFile]", parametersAction: p => new
            {
                IDHoaDon = p.Guid()
            }, body: @"SELECT 
    		dvqd.MaHangHoa,
    			TenHangHoa +
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as TenHangHoa,
			dvqd.TenDonViTinh,
			DM_LoHang.MaLoHang as MaLoHang,
			TienChietKhau,ThanhTien, SoLuong,ThanhToan
    		FROM BH_HoaDon hd
    		JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    		JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    		JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
    		LEFT JOIN (Select Main.id_hanghoa,
    					Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    					From
    					(
    					Select distinct hh_tt.id_hanghoa,
    					(
    					Select tt.GiaTri + ' - ' AS [text()]
    					From dbo.hanghoa_thuoctinh tt
    					Where tt.id_hanghoa = hh_tt.id_hanghoa
    					order by tt.ThuTuNhap 
    					For XML PATH ('')
    					) hanghoa_thuoctinh
    					From dbo.hanghoa_thuoctinh hh_tt
    					) Main
    					) as ThuocTinh on hh.ID = ThuocTinh.id_hanghoa
    		LEFT JOIN DM_LoHang ON cthd.ID_LoHang = DM_LoHang.ID
    		WHERE cthd.ID_HoaDon = @IDHoaDon
    		order by SoThuTu desc");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetListChiTietHoaDonKiemKhoXuatFile]");
        }
    }
}