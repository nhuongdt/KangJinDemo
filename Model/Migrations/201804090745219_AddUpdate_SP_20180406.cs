namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdate_SP_20180406 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetListNhanVienChuaCoND]", body: @"select * from NS_NhanVien where ID not in (select ID_NhanVien from HT_nguoiDung)");
            CreateStoredProcedure(name: "[dbo].[GetListNhanVienEdit]", parametersAction: p => new
            {
                ID_NhanVien = p.Guid()
            }, body: @"select * from NS_NhanVien where ID not in (select ID_NhanVien from HT_nguoiDung where ID_NhanVien != @ID_NhanVien)");
            CreateStoredProcedure(name: "[dbo].[getList_ChiTietHangHoaXuatHuy]", parametersAction: p => new
            {
                ID_HangHoa = p.Guid()
            }, body: @"Select 
	hh.TenHangHoa,
	Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri
	from DM_HangHoa hh
	left join 
	(
		Select Main.id_hanghoa,
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
	) tt on hh.ID = tt.id_hanghoa
	where hh.ID = @ID_HangHoa");

            CreateStoredProcedure(name: "[dbo].[update_DanhMucHangHoa]", body: @"update DM_HangHoa set TenKhac = null");
            CreateStoredProcedure(name: "[dbo].[import_HangHoaThuocTinh]", parametersAction: p => new
            {
                TenThuocTinh = p.String(),
                GiaTri = p.String(),
                ThuTuNhap = p.Int(),
                MaHangHoa = p.String()
            }, body: @"if (len(@GiaTri) > 0)
	Begin
		DECLARE @ID_ThuocTinh as uniqueidentifier
		set @ID_ThuocTinh = (Select ID from DM_ThuocTinh where TenThuocTinh like @TenThuocTinh);
		if (len(@ID_ThuocTinh) > 0)
		Begin
			DECLARE @ID_ThuocTinh1 as uniqueidentifier
		End
		else
		Begin
			set @ID_ThuocTinh = newID();
			insert into DM_ThuocTinh (ID, TenThuocTinh)
			values (@ID_ThuocTinh, @TenThuocTinh)
		End 
		DECLARE @ID_HangHoa as uniqueidentifier
		set @ID_HangHoa = (Select ID_HangHoa from DonViQuiDoi where MaHangHoa like @MaHangHoa);
		if (len(@ID_HangHoa) > 0)
		Begin
			DECLARE @ID_Check as uniqueidentifier
			set @ID_Check = (Select ID from HangHoa_ThuocTinh where ID_HangHoa = @ID_HangHoa and ID_ThuocTinh = @ID_ThuocTinh);
			if (len(@ID_Check) > 0)
			Begin
				update HangHoa_ThuocTinh set GiaTri = @GiaTri where ID = @ID_Check
			End
			else
			Begin
				insert into HangHoa_ThuocTinh (ID, ID_HangHoa, ID_ThuocTinh, GiaTri, ThuTuNhap)
				values (newID(), @ID_HangHoa, @ID_ThuocTinh, @GiaTri, @ThuTuNhap)
			End
		End
	End");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetListNhanVienChuaCoND]");
            DropStoredProcedure("[dbo].[GetListNhanVienEdit]");
            DropStoredProcedure("[dbo].[getList_ChiTietHangHoaXuatHuy]");
            DropStoredProcedure("[dbo].[update_DanhMucHangHoa]");
            DropStoredProcedure("[dbo].[import_HangHoaThuocTinh]");
        }
    }
}