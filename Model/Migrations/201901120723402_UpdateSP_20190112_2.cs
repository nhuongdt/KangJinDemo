namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190112_2 : DbMigration
    {
        public override void Up()
        {
            
            Sql(@"ALTER PROCEDURE [dbo].[getList_NhatKySuDung]
    @ID_NhanVien [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @NoiDung [nvarchar](max),
    @ChucNang [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ThaoTac [nvarchar](max),
	@NhatKy_XemDS_PhongBan [varchar](max),
	@NhatKy_XemDS_HeThong [varchar](max),
	@ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	DECLARE @LaAdmin as nvarchar
    Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd where nd.ID = @ID_NguoiDung)
	If (@LaAdmin = 1)
	BEGIN
    Select nv.TenNhanVien,
    	nk.ChucNang,
    	nk.ThoiGian,
    	nk.NoiDung,
    	nk.NoiDungChiTiet,
    	nk.LoaiNhatKy
    	FROM HT_NhatKySuDung nk
    	join NS_NhanVien nv on nk.ID_NhanVien = nv.ID
    	where nk.ThoiGian >= @timeStart and nk.ThoiGian < @timeEnd and nk.ID_DonVi = @ID_ChiNhanh and 
    	nk.ID_NhanVien in (select * from splitstring(@ID_NhanVien)) and nk.LoaiNhatKy in (select * from splitstring(@ThaoTac))
    	and nk.ChucNang like @ChucNang
    	and nk.NoiDung like @NoiDung
    	order by nk.ThoiGian DESC
	END
	ELSE
	BEGIN
		if (@NhatKy_XemDS_HeThong = 'NhatKy_XemDS_HeThong')
		BEGIN
			Select nv.TenNhanVien,
    		nk.ChucNang,
    		nk.ThoiGian,
    		nk.NoiDung,
    		nk.NoiDungChiTiet,
    		nk.LoaiNhatKy
    		FROM HT_NhatKySuDung nk
    		join NS_NhanVien nv on nk.ID_NhanVien = nv.ID
    		where nk.ThoiGian >= @timeStart and nk.ThoiGian < @timeEnd and nk.ID_DonVi = @ID_ChiNhanh and 
    		nk.ID_NhanVien in (select * from splitstring(@ID_NhanVien)) and nk.LoaiNhatKy in (select * from splitstring(@ThaoTac))
    		and nk.ChucNang like @ChucNang
    		and nk.NoiDung like @NoiDung
    		order by nk.ThoiGian DESC
		END 
		ELSE 
		BEGIN
			if (@NhatKy_XemDS_PhongBan = 'NhatKy_XemDS_PhongBan')
			BEGIN
				DECLARE @ID_NhanVienPhongBan table (ID_NhanVien uniqueidentifier);
				INSERT INTO @ID_NhanVienPhongBan exec getListID_NhanVienPhongBan @ID_NguoiDung;
				Select nv.TenNhanVien,
    			nk.ChucNang,
    			nk.ThoiGian,
    			nk.NoiDung,
    			nk.NoiDungChiTiet,
    			nk.LoaiNhatKy
    			FROM HT_NhatKySuDung nk
    			join NS_NhanVien nv on nk.ID_NhanVien = nv.ID
				join @ID_NhanVienPhongBan pb on nv.ID = pb.ID_NhanVien
    			where nk.ThoiGian >= @timeStart and nk.ThoiGian < @timeEnd and nk.ID_DonVi = @ID_ChiNhanh and 
    			nk.ID_NhanVien in (select * from splitstring(@ID_NhanVien)) and nk.LoaiNhatKy in (select * from splitstring(@ThaoTac))
    			and nk.ChucNang like @ChucNang
    			and nk.NoiDung like @NoiDung
    			order by nk.ThoiGian DESC
			END
			else 
			BEGIN
				DECLARE @ID_NhanVienDS table (ID_NhanVien uniqueidentifier);
				INSERT INTO @ID_NhanVienDS exec getListID_NhanVienDS @ID_NguoiDung;
				Select nv.TenNhanVien,
    			nk.ChucNang,
    			nk.ThoiGian,
    			nk.NoiDung,
    			nk.NoiDungChiTiet,
    			nk.LoaiNhatKy
    			FROM HT_NhatKySuDung nk
    			join NS_NhanVien nv on nk.ID_NhanVien = nv.ID
				join @ID_NhanVienDS pb on nv.ID = pb.ID_NhanVien
    			where nk.ThoiGian >= @timeStart and nk.ThoiGian < @timeEnd and nk.ID_DonVi = @ID_ChiNhanh and 
    			nk.ID_NhanVien in (select * from splitstring(@ID_NhanVien)) and nk.LoaiNhatKy in (select * from splitstring(@ThaoTac))
    			and nk.ChucNang like @ChucNang
    			and nk.NoiDung like @NoiDung
    			order by nk.ThoiGian DESC
			END
		END
	END
END");

            CreateStoredProcedure(name: "[dbo].[GetDVTKhacInGiaoDich]", parametersAction: p => new
            {
                ID_DonViQuiDoi = p.Guid(),
                ID_DonVi = p.Guid()
            }, body: @"DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
	SELECT @ID_HangHoa = ID_HangHoa FROM DonViQuiDoi WHERE ID = @ID_DonViQuiDoi

	DECLARE @TableDVT TABLE (ID_DonViQuiDoi UNIQUEIDENTIFIER,MaHangHoa NVARCHAR(MAX), GiaNhap FLOAT, GiaVon FLOAT, TonKho FLOAT, ID_LoHang UNIQUEIDENTIFIER, TyLeChuyenDoi FLOAT) INSERT INTO @TableDVT
	Select dvqd.ID as ID_DonViQuiDoi, dvqd.MaHangHoa, CAST(ISNULL(dvqd.GiaNhap, 0) AS FLOAT) as GiaNhap, CAST(ISNULL(gv.GiaVon,0) AS FLOAT) as GiaVon, 
	0 AS TonKho, gv.ID_LoHang, dvqd.TyLeChuyenDoi from DonViQuiDoi dvqd
	LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi
	where dvqd.ID_HangHoa = @ID_HangHoa and gv.ID_DonVi = @ID_DonVi

	SELECT ID_DonViQuiDoi, MaHangHoa, GiaNhap, GiaVon,(ISNULL([dbo].FUNC_TinhSLTonKhiTaoHD(@ID_DonVi, @ID_HangHoa, ID_LoHang, GETDATE()),0)) / TyLeChuyenDoi AS TonKho, ID_LoHang FROM @TableDVT");

            CreateStoredProcedure(name: "[dbo].[getList_TenDonViTinh]", parametersAction: p => new
            {
                ID_DonViQuiDoi = p.Guid(),
                ID_ChiNhanh = p.Guid()
            }, body: @"select gv.ID_LoHang, dvqd2.ID as ID_DonViQuiDoi, dvqd2.MaHangHoa, dvqd2.TenDonViTinh,
CAST(ROUND(ISNULL(gv.GiaVon, 0), 0) as float) as GiaVon,
dvqd2.TyLeChuyenDoi, 1 as TrangThai
 from DonViQuiDoi dvqd
inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
inner join DonViQuiDoi dvqd2 on dvqd.ID_HangHoa = dvqd2.ID_HangHoa
left join DM_GiaVon gv on gv.ID_DonViQuiDoi = dvqd2.ID and gv.ID_DonVi = @ID_ChiNhanh
where (dvqd.Xoa is null or dvqd.Xoa = 0)
and (dvqd2.Xoa is null or dvqd2.Xoa = 0)
and hh.TheoDoi = 1 and DuocBanTrucTiep = 1
and dvqd.TenDonViTinh != ''
and dvqd.ID = @ID_DonViQuiDoi
order by dvqd2.LaDonViChuan DESC, dvqd2.TenDonViTinh");

            CreateStoredProcedure(name: "[dbo].[SP_GetListHoaDons_afterXuly]", parametersAction: p => new
            {
                MaHD = p.String(),
                ID_DonVi = p.Guid(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                loaiHoaDon = p.Int()
            }, body: @"Select 
    	c.ID,
    	c.MaHoaDon,
    	c.ID_HoaDon,
    	c.ID_BangGia,
    	c.ID_NhanVien,
    	c.ID_DonVi,
    	c.NguoiTao,
    	c.DienGiai,
    	c.NgayLapHoaDon,
    	c.ID_DoiTuong,
    	c.SoLuongTra,
    	c.SoLuongBan,
    	c.LoaiHoaDon,
		ISNULL(c.PhaiThanhToan, 0) as PhaiThanhToan,
    	ISNULL(c.TongTienHang, 0) as TongTienHang,
    	ISNULL(c.TongGiamGia, 0) as TongGiamGia,
    	ISNULL(c.DiemGiaoDich, 0) as DiemGiaoDich,
		ISNULL(c.TenNhanVien, '') as TenNhanVien,
    	ISNULL(c.TenDoiTuong, 0) as TenDoiTuong,
    	ISNULL(c.TenDoiTuong_KhongDau, 0) as TenDoiTuong_KhongDau,
    	ISNULL(c.TenDoiTuong_ChuCaiDau, 0) as TenDoiTuong_ChuCaiDau
    	from
    	(
    		Select 
    		hd.ID,
    		hd.MaHoaDon,
    		hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
    		dt.DienThoai,
    		hd.ID_DoiTuong,	
    		hd.ID_HoaDon,
    		hd.ID_BangGia,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
    		hd.NguoiTao,	
    		hd.DienGiai,	
    		b.SoLuongTra,
    		b.SoLuongBan,
			nv.TenNhanVien,
			dt.TenDoiTuong,
			dt.TenDoiTuong_KhongDau,
			dt.TenDoiTuong_ChuCaiDau,
			hd.PhaiThanhToan,
			hd.TongTienHang,
			hd.TongGiamGia,
			hd.DiemGiaoDich
		from 
    		(
    			Select 
    			a.ID,
    			Sum(ISNULL(a.SoLuongBan, 0)) as SoLuongBan,
    			Sum(ISNULL(a.SoLuongTra, 0)) as SoLuongTra
    			from
    			(
    				Select 
    					hd.ID as ID,
    					Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongBan,
    					null as SoLuongTra 
    				from
    					BH_HoaDon hd
    					inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    					where hd.ID_DonVi = @ID_DonVi
    					and hd.loaihoadon = @loaiHoaDon
    					Group by hd.ID
    
    				Union all
    				select 
    					hd.ID_HoaDon as ID,
    					null as SoLuongBan,
    					Sum(ISNULL(hdct.SoLuong, 0)) as SoLuongTra
    				from
    					BH_HoaDon hd
    					inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    					where hd.ID_DonVi = @ID_DonVi
    					and hd.loaihoadon = '1' and hd.ChoThanhToan ='0'
    					
    				Group by hd.ID_HoaDon
    			)a
    			--where SoLuongTra < SoLuongBan
    			Group by a.ID
    		) b
    		inner join BH_HoaDon hd on b.ID = hd.ID
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    		where hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd 
    		and b.SoLuongBan > b.SoLuongTra
    			and hd.chothanhtoan = '0'
    	)c
    	where MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD
    	order by NgayLapHoaDon DESC");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetDVTKhacInGiaoDich]");
            DropStoredProcedure("[dbo].[getList_TenDonViTinh]");
        }
    }
}
