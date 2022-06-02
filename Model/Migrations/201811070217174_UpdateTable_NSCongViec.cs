namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_NSCongViec : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getlistNhanVien_CaiDatChietKhau]", parametersAction: p => new
            {
                ID_DonVi = p.Guid(),
                Text_NhanVien = p.String(),
                Text_NhanVien_TV = p.String(),
                TrangThai = p.String()
            }, body: @"Select a.ID as ID_NhanVien,
	a.MaNhanVien, a.TenNhanVien, a.DienThoaiDiDong
	From
	(
		Select DISTINCT nv.ID, nv.MaNhanVien, nv.TenNhanVien,nv.TenNhanVienChuCaiDau, nv.TenNhanVienKhongDau, nv.DienThoaiDiDong, 
		Case when nv.TrangThai is null then 1 else nv.TrangThai end as TrangThai,
		Case when ck.ID is null then 0 else 1 end as CaiDat
		From NS_NhanVien nv
		inner join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
		left join ChietKhauMacDinh_NhanVien ck on nv.ID = ck.ID_NhanVien and ck.ID_DonVi = @ID_DonVi
		where nv.MaNhanVien like @Text_NhanVien or nv.MaNhanVien like @Text_NhanVien_TV 
		or nv.DienThoaiDiDong like @Text_NhanVien_TV or nv.TenNhanVienKhongDau like @Text_NhanVien or nv.TenNhanVienChuCaiDau like @Text_NhanVien
		and DaNghiViec != 1
		and ct.ID_DonVi = @ID_DonVi
	)a
	where a.CaiDat like @TrangThai
	and a.TrangThai != 0
	order by MaNhanVien");

            CreateStoredProcedure(name: "[dbo].[insert_SaoChepCaiDatHoaHong]", parametersAction: p => new
            {
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                ID_NhanVien_new = p.String(),
                PhuongThuc = p.Int()
            }, body: @"if (@PhuongThuc = 1) -- update hàng hóa đã cài đặt
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
    	order by b.ID_NhanVien");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getlistNhanVien_CaiDatChietKhau]");
            DropStoredProcedure("[dbo].[insert_SaoChepCaiDatHoaHong]");
        }
    }
}
