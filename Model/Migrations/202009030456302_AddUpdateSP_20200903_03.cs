namespace Model.Migrations
{
    using Model_banhang24vn;
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20200903_03 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[UpdateStatusBangLuong_whenChangeCong]", parametersAction: p => new
            {
                ID_DonVi = p.Guid(),
                NgayChamCong = p.DateTime()
            }, body: @"SET NOCOUNT ON;
	set @NgayChamCong = FORMAT(@NgayChamCong,'yyyy-MM-dd')

	update NS_BangLuong set TrangThai= 2
	where exists (select ID
					from
						(select ID, FORMAT(TuNgay,'yyyy-MM-dd') as TuNgay, FORMAT(DenNgay,'yyyy-MM-dd') as DenNgay
						from NS_BangLuong
						where TrangThai = 1 and ID_DonVi= @ID_DonVi
						) bl
					where bl.TuNgay<= @ngaychamcong and bl.DenNgay >= @ngaychamcong and ID= bl.ID)");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[UpdateStatusBangLuong_whenChangeCong]");
        }
    }
}
