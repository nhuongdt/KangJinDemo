namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20210118 : DbMigration
    {
        public override void Up()
        {
			CreateStoredProcedure(name: "[dbo].[GetListGaraDanhMucXe_v1]", parametersAction: p => new
			{
				IdHangXe = p.Guid(),
				IdLoaiXe = p.Guid(),
				IdMauXe = p.Guid(),
				TrangThais = p.String(),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	declare @tbTrangThai table (GiaTri int)
	insert into @tbTrangThai
	select Name from dbo.splitstring(@TrangThais);
    -- Insert statements for procedure here
	if(@PageSize != 0)
	BEGIN
	with data_cte
	as
	(
	SELECT gx.ID, gx.BienSo, gx.DungTich, gx.GhiChu, gx.HopSo, gx.MauSon, gx.ID_KhachHang, gx.ID_MauXe, gx.NamSanXuat, gx.NgayTao, gx.SoKhung,
	gx.SoMay, gx.TrangThai, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.NgaySinh_NgayTLap, dt.DiaChi, dtt.MaTinhThanh, dtt.TenTinhThanh,
	dqh.MaQuanHuyen, dqh.TenQuanHuyen,
	ghx.TenHangXe, glx.TenLoaiXe, gmx.TenMauXe, gx.NguoiTao FROM Gara_DanhMucXe gx
	INNER JOIN Gara_MauXe gmx ON gx.ID_MauXe = gmx.ID
	INNER JOIN Gara_HangXe ghx ON gmx.ID_HangXe = ghx.ID
	INNER JOIN Gara_LoaiXe glx ON gmx.ID_LoaiXe = glx.ID
	INNER JOIN @tbTrangThai tt ON tt.GiaTri = gx.TrangThai
	LEFT JOIN DM_DoiTuong dt ON dt.ID = gx.ID_KhachHang
	LEFT JOIN DM_TinhThanh dtt ON dt.ID_TinhThanh = dtt.ID
	LEFT JOIN DM_QuanHuyen dqh ON dqh.ID = dt.ID_QuanHuyen
	WHERE (@IdHangXe IS NULL OR ghx.ID = @IdHangXe)
	AND (@IdLoaiXe IS NULL OR glx.ID = @IdLoaiXe)
	AND (@IdMauXe IS NULL OR gmx.ID = @IdMauXe)
	AND ((select count(Name) from @tblSearch b where     			
		gx.BienSo like '%'+b.Name+'%'
		or gx.NamSanXuat like '%'+b.Name+'%'
		or gx.SoMay like '%'+b.Name+'%'
		or gx.SoKhung like '%'+b.Name+'%'
		or gx.MauSon like '%'+b.Name+'%'
		or gx.DungTich like '%'+b.Name+'%'
		or gx.HopSo like '%'+b.Name+'%'
		or gx.GhiChu like '%'+b.Name+'%'
		or dt.MaDoiTuong like '%'+b.Name+'%'		
		or dt.TenDoiTuong like '%'+b.Name+'%'
		or dt.DienThoai like '%'+b.Name+'%'
		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
		or gmx.TenMauXe like '%'+b.Name+'%'
		or glx.TenLoaiXe like '%'+b.Name+'%'
		or ghx.TenHangXe like '%'+b.Name+'%'
		)=@count or @count=0)
		),
		count_cte
		as
		(
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
			from data_cte
		)

		SELECT dt.*, ct.* FROM data_cte dt
		CROSS JOIN count_cte ct
		ORDER BY dt.NgayTao desc
		OFFSET (@CurrentPage * @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY;
		END
		ELSE
		BEGIN
		with data_cte
	as
	(
	SELECT gx.ID, gx.BienSo, gx.DungTich, gx.GhiChu, gx.HopSo, gx.MauSon, gx.ID_KhachHang, gx.ID_MauXe, gx.NamSanXuat, gx.NgayTao, gx.SoKhung,
	gx.SoMay, gx.TrangThai, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.NgaySinh_NgayTLap, dt.DiaChi, dtt.MaTinhThanh, dtt.TenTinhThanh,
	dqh.MaQuanHuyen, dqh.TenQuanHuyen,
	ghx.TenHangXe, glx.TenLoaiXe, gmx.TenMauXe, gx.NguoiTao FROM Gara_DanhMucXe gx
	INNER JOIN Gara_MauXe gmx ON gx.ID_MauXe = gmx.ID
	INNER JOIN Gara_HangXe ghx ON gmx.ID_HangXe = ghx.ID
	INNER JOIN Gara_LoaiXe glx ON gmx.ID_LoaiXe = glx.ID
	INNER JOIN @tbTrangThai tt ON tt.GiaTri = gx.TrangThai
	LEFT JOIN DM_DoiTuong dt ON dt.ID = gx.ID_KhachHang
	LEFT JOIN DM_TinhThanh dtt ON dt.ID_TinhThanh = dtt.ID
	LEFT JOIN DM_QuanHuyen dqh ON dqh.ID = dt.ID_QuanHuyen
	WHERE (@IdHangXe IS NULL OR ghx.ID = @IdHangXe)
	AND (@IdLoaiXe IS NULL OR glx.ID = @IdLoaiXe)
	AND (@IdMauXe IS NULL OR gmx.ID = @IdMauXe)
	AND ((select count(Name) from @tblSearch b where     			
		gx.BienSo like '%'+b.Name+'%'
		or gx.NamSanXuat like '%'+b.Name+'%'
		or gx.SoMay like '%'+b.Name+'%'
		or gx.SoKhung like '%'+b.Name+'%'
		or gx.MauSon like '%'+b.Name+'%'
		or gx.DungTich like '%'+b.Name+'%'
		or gx.HopSo like '%'+b.Name+'%'
		or gx.GhiChu like '%'+b.Name+'%'
		or dt.MaDoiTuong like '%'+b.Name+'%'		
		or dt.TenDoiTuong like '%'+b.Name+'%'
		or dt.DienThoai like '%'+b.Name+'%'
		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
		or gmx.TenMauXe like '%'+b.Name+'%'
		or glx.TenLoaiXe like '%'+b.Name+'%'
		or ghx.TenHangXe like '%'+b.Name+'%'
		)=@count or @count=0)
		)
		SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
			ORDER BY dt.NgayTao desc
		END");

			CreateStoredProcedure(name: "[dbo].[JqAuto_SearchXe]", parametersAction: p => new
			{
				TextSearch = p.String()
			}, body: @"SET NOCOUNT ON;

    select xe.ID, xe.ID_MauXe, xe.BienSo, xe.SoKhung, xe.SoMay, mau.TenMauXe, hang.TenHangXe
	from Gara_DanhMucXe xe
	join Gara_MauXe mau on xe.ID_MauXe= mau.ID
	join Gara_HangXe hang on mau.ID_HangXe= hang.ID
	where xe.BienSo like @TextSearch
	-- chi get xe khong co trong xuong
	and not exists (select ID_Xe from Gara_PhieuTiepNhan tn where xe.ID= tn.ID_Xe 
	and (tn.NgayXuatXuong is null or tn.TrangThai != 0 ))");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetListGaraDanhMucXe_v1]");
			DropStoredProcedure("[dbo].[JqAuto_SearchXe]");
        }
    }
}
