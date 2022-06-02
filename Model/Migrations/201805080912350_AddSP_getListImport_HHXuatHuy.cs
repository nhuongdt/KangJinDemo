namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSP_getListImport_HHXuatHuy : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getListImport_HHXuatHuy]", parametersAction: p => new
            {
                MaHH = p.String()
            }, body: @"select
	dvqd.ID as id,
	dvqd.ID as ID,
	dvqd.ID as ID_DonViQuiDoi,
	dvqd.GiaVon,
	CAST(ROUND(1, 0) as float) as SoLuong,
	CAST(ROUND(1, 0) as float) as SoLuongXuatHuy,
	hh.TenHangHoa,
    Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
	CAST(ROUND(1, 0) as float) as TonKho,
	dvqd.GiaVon as GiaTriHuy,
	dvqd.MaHangHoa,
	CAST(ROUND(2, 0) as float) as TrangThaiMoPhieu
	FROM DonViQuiDoi dvqd
	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
	where dvqd.MaHangHoa like @MaHH");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getListImport_HHXuatHuy]");
        }
    }
}