namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLoadGiaBanChiTiet_20180210 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[LoadGiaBanChiTiet]", parametersAction: p => new
            {
                ID_BangGia = p.String(),
                maHoaDon = p.String()
            }, body: @"if(@ID_BangGia != '')
	BEGIN
		Select gbct.ID, dvqd.ID_HangHoa,hh.TenHangHoa,hh.NgayTao,dvqd.Xoa, hh.ID_NhomHang, dvqd.TenDonViTinh as DonViTinh, dvqd.GiaNhap as GiaNhapCuoi, dvqd.GiaBan as GiaMoi, dvqd.MaHangHoa,
		dvqd.GiaVon, dvqd.GiaBan, dvqd.ID as IDQuyDoi, gbct.ID_GiaBan,hhtt.GiaTri
		from DM_GiaBan_ChiTiet gbct
		left join DonViQuiDoi dvqd on gbct.ID_DonViQuiDoi = dvqd.ID
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
		where (hh.TenHangHoa_KhongDau like @maHoaDon or hh.TenHangHoa_KyTuDau like @maHoaDon or dvqd.MaHangHoa like @maHoaDon) and CAST(gbct.ID_Giaban AS NVARCHAR(36)) = @ID_BangGia and dvqd.Xoa is null and hh.TheoDoi =1
		order by hh.NgayTao desc
	END
	ELSE
	BEGIN
		Select dvqd.ID, dvqd.ID_HangHoa,hh.TenHangHoa,hh.NgayTao,dvqd.Xoa, hh.ID_NhomHang, dvqd.TenDonViTinh as DonViTinh, dvqd.GiaNhap as GiaNhapCuoi, dvqd.GiaBan as GiaMoi, dvqd.MaHangHoa,
		dvqd.GiaVon, dvqd.GiaBan, dvqd.ID as IDQuyDoi, NEWID() as ID_GiaBan, hhtt.GiaTri
		from DonViQuiDoi dvqd
		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
		where (hh.TenHangHoa_KhongDau like @maHoaDon or hh.TenHangHoa_KyTuDau like @maHoaDon or dvqd.MaHangHoa like @maHoaDon) and dvqd.Xoa is null and hh.TheoDoi =1
		order by hh.NgayTao desc	
		END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[LoadGiaBanChiTiet]");
        }
    }
}
