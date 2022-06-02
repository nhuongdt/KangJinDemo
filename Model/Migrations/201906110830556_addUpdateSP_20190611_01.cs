namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUpdateSP_20190611_01 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROC [dbo].[insert_TonKhoKhoiTaoByInsert]
AS
BEGIN
Update DonViQuiDoi set Xoa = 0 where Xoa is null
DECLARE @ID_DonViInsert Uniqueidentifier;
SET @ID_DonViInsert = (select Top 1 ID_DonVi from BH_HoaDon where SoLanIn = -9 and ChoThanhToan = '0')
--Insert tồn kho vào DM_HangHoa_TonKho
DECLARE @TabTK TABLE (ID UNIQUEIDENTIFIER, ID_DonViQuyDoi UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, TonKho float, ChoThanhToan bit);
DECLARE @TableDonVi TABLE (ID_DonVi UNIQUEIDENTIFIER) INSERT INTO @TableDonVi
SELECT ID FROM DM_DonVi where ID != @ID_DonViInsert
DECLARE @ID_DonVi UNIQUEIDENTIFIER;
DECLARE CS_DonVi CURSOR SCROLL LOCAL FOR SELECT ID_DonVi FROM @TableDonVi
OPEN CS_DonVi
DECLARE @TabID_DonViQuiDoi TABLE (ID_DonViQuiDoi UNIQUEIDENTIFIER, ChoThanhToan bit);
INSERT INTO @TabID_DonViQuiDoi Select DISTINCT ct.ID_DonViQuiDoi, hd.ChoThanhToan FROM BH_HoaDon_ChiTiet ct join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
where hd.SoLanIn = -9

-- insert DM_HangHoa_TonKho cho DV insert
INSERT INTO @TabTK SELECT NEWID(), dvqd.ID, @ID_DonViInsert, dmlo.ID, [dbo].[FUNC_TinhSLTonKhiTaoHD](@ID_DonViInsert, dvqd.ID_HangHoa, dmlo.ID, DATEADD(minute, 1, GETDATE())) / dvqd.TyLeChuyenDoi, dv.ChoThanhToan
		FROM DonViQuiDoi dvqd
		INNER JOIN @TabID_DonViQuiDoi dv on dvqd.ID = dv.ID_DonViQuiDoi
		INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		LEFT JOIN DM_LoHang dmlo on dvqd.ID_HangHoa = dmlo.ID_HangHoa
		where dvqd.Xoa = 0 and hh.LaHangHoa = 1

Update dm set dm.TonKho = tk.TonKho from DM_HangHoa_TonKho dm join @TabTK tk on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) and tk.ChoThanhToan = '0'
Insert into DM_HangHoa_TonKho select NEWID(), tk.ID_DonViQuyDoi, tk.ID_DonVi, tk.ID_LoHang, tk.TonKho from @TabTK tk left join DM_HangHoa_TonKho dm on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = tk.ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) where dm.ID is null
FETCH FIRST FROM CS_DonVi INTO @ID_DonVi
WHILE @@FETCH_STATUS = 0
BEGIN
	Insert into DM_HangHoa_TonKho select NEWID(), tk.ID_DonViQuyDoi, @ID_DonVi as ID_DonVi, tk.ID_LoHang, 0 as TonKho from @TabTK tk 
	left join DM_HangHoa_TonKho dm on dm.ID_DonViQuyDoi = tk.ID_DonViQuyDoi and dm.ID_DonVi = @ID_DonVi and (dm.ID_LoHang = tk.ID_LoHang or (dm.ID_LoHang is null and tk.ID_LoHang is null)) where dm.ID is null
	FETCH NEXT FROM CS_DonVi INTO @ID_DonVi
END
CLOSE CS_DonVi
DEALLOCATE CS_DonVi
END");

        }
        
        public override void Down()
        {
        }
    }
}
