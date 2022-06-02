namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDefaultValue_20190611 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[SP_GetAll_DMLoHang_TonKho]
    @ID_ChiNhanh [uniqueidentifier],
    @timeChotSo [datetime]
AS
BEGIN
SET NOCOUNT ON;
    Select
    		lh.ID,
    		dvqd.ID as ID_DonViQuiDoi,
    		lh.ID_HangHoa,
			dhh.TenHangHoa,	
			dvqd.MaHangHoa,
    		lh.TenLoHang as TenLoHangFull,
    		lh.MaLoHang,
    		lh.NgaySanXuat as NgaySanXuat,
    		lh.NgayHetHan,
    		lh.TrangThai,
    		round(tk.TonKho,2) as TonKho,
			dvqd.xoa
    	FROM  DM_LoHang lh 
		join DM_HangHoa dhh on lh.ID_HangHoa = dhh.ID
		join DonViQuiDoi dvqd on dvqd.ID_HangHoa = lh.ID_HangHoa
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)	
		left join DM_GiaVon gv on (dvqd.ID = gv.ID_DonViQuiDoi and (lh.ID = gv.ID_LoHang or lh.ID is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where lh.ID is not null	 
		and (dhh.LaHangHoa = 0 or (dhh.LaHangHoa = 1 and tk.TonKho is not null)) -- chi lay HangHoa neu exsit in DM_TonKho_HangHoa
		-- get all: lấy cả Lô hết hạn và hàng hóa đã bị xóa --> use when TraHang
		order by lh.ID
END");
        }
        
        public override void Down()
        {
        }
    }
}
