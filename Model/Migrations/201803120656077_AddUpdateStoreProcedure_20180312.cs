namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateStoreProcedure_20180312 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[LoadAllDanhMucHangHoa]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select aa.ID as ID_DonViQuiDoi, aa.ID_HangHoaCungLoai,aa.GiaVon, aa.GiaBan,
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho FROM (
    (select dvqd.ID, hh.ID_HangHoaCungLoai,dvqd.GiaVon, dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DM_HangHoa hh
    left join DonViQuiDoi dvqd on hh.ID = dvqd.ID_hangHoa
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    where dvqd.xoa is null and dvqd.ladonvichuan = 1 and hh.TheoDoi =1) aa
    left join
    (
    SELECT  dvqd3.ID as ID_DonViQuiDoi, dvqd3.mahanghoa, a.TenHangHoa, dvqd3.TenDonViTinh, ((a.TonCuoiKy / dvqd3.TyLeChuyenDoi)) as TonCuoiKy FROM 
    (
    SELECT 
    dhh.ID,
    	dhh.TenHangHoa,
    SUM(ISNULL(HangHoa.TonDau,0)) AS TonCuoiKy
    FROM
    (
    SELECT
    td.ID_DonViQuiDoi,
    SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    NULL AS SoLuongNhap,
    NULL AS SoLuongXuat
    FROM
    (
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'
    	AND bhd.NgayLapHoaDon >= @timeStart
    AND bhd.ID_DonVi = @ID_ChiNhanh
    GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                         
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    NULL AS SoLuongNhap,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi 
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart
    GROUP BY bhdct.ID_DonViQuiDoi
    
    UNION ALL
    SELECT
    bhdct.ID_DonViQuiDoi,
    SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    null AS SoLuongXuat
    FROM BH_HoaDon_ChiTiet bhdct
    LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	)bb on aa.ID = bb.ID_DonViQuiDoi)");

            
            CreateStoredProcedure(name: "[dbo].[getList_DiaryHoaDon]", parametersAction: p => new
            {
                ID_DonVi = p.Guid()
            }, body: @"Select Top 12
	nv.TenNhanVien,
	bhhd.MaHoaDon,
	CAST(ROUND(bhhd.TongTienHang, 0) as float) as ThanhTien,
	CONVERT(VARCHAR, bhhd.NgayLapHoaDon, 22) as NgayLapHoaDon,
	Case when bhhd.LoaiHoaDon = 1 then N'bán đơn hàng' when bhhd.LoaiHoaDon = 3 then N'nhập đơn đặt hàng' when bhhd.LoaiHoaDon = 4 then N'nhập kho đơn hàng'
    when bhhd.LoaiHoaDon = 5 then N'xuất kho đơn hàng' when bhhd.LoaiHoaDon = 6 then N'nhận hàng trả'
	when bhhd.LoaiHoaDon = 7 then N'trả hàng nhà cung cấp' else N'xuất hủy đơn hàng' end as TenLoaiChungTu
	from
	BH_HoaDon bhhd
	Join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID
	where bhhd.LoaiHoaDon in (1,3,4,5,6,7,8) and bhhd.ChoThanhToan = 0 
	and bhhd.ID_DonVi = @ID_DonVi
	ORDER BY bhhd.NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[insert_DM_KhuVuc]", parametersAction: p => new
            {
                ID = p.Guid(),
                MaKhuVuc = p.String(),
                TenKhuVuc = p.String(),
                TimeCreate = p.DateTime()
            }, body: @"insert into DM_KhuVuc (ID, MaKhuVuc, TenKhuVuc, NguoiTao, NgayTao)
	values (@ID, @MaKhuVuc, @TenKhuVuc, 'admin',  @TimeCreate)");

            CreateStoredProcedure(name: "[dbo].[insert_DM_ViTri]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_KhuVuc = p.Guid(),
                MaViTri = p.String(),
                TenViTri = p.String()
            }, body: @"insert into DM_ViTri (ID, MaViTri, ID_KhuVuc, TenViTri)
	values (@ID, @MaViTri, @ID_KhuVuc,  @TenViTri)");

            CreateStoredProcedure(name: "[dbo].[Update_HoaDonNHCFB]", body: @"Update BH_HoaDon set ID_ViTri = '62000000-0000-0000-0000-000000000001' where ID in
	('50000000-0000-0000-0000-000000000021', '50000000-0000-0000-0000-000000000022', '50000000-0000-0000-0000-000000000023')
	Update BH_HoaDon set ID_ViTri = '62000000-0000-0000-0000-000000000002' where ID in
	('50000000-0000-0000-0000-000000000024', '50000000-0000-0000-0000-000000000025', '50000000-0000-0000-0000-000000000026')
	Update BH_HoaDon set ID_ViTri = '62000000-0000-0000-0000-000000000003' where ID in
	('50000000-0000-0000-0000-000000000027', '50000000-0000-0000-0000-000000000028', '50000000-0000-0000-0000-000000000029')
	Update BH_HoaDon set ID_ViTri = '62000000-0000-0000-0000-000000000004' where ID in
	('50000000-0000-0000-0000-000000000030', '50000000-0000-0000-0000-000000000031', '50000000-0000-0000-0000-000000000032')
	Update BH_HoaDon set ID_ViTri = '62000000-0000-0000-0000-000000000005' where ID in
	('50000000-0000-0000-0000-000000000033', '50000000-0000-0000-0000-000000000034', '50000000-0000-0000-0000-000000000035')
	Update BH_HoaDon set ID_ViTri = '62000000-0000-0000-0000-000000000006' where ID in
	('50000000-0000-0000-0000-000000000036', '50000000-0000-0000-0000-000000000037', '50000000-0000-0000-0000-000000000038',
	'50000000-0000-0000-0000-000000000039', '50000000-0000-0000-0000-000000000040')");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[LoadAllDanhMucHangHoa]");
            DropStoredProcedure("[dbo].[getList_DiaryHoaDon]");
            DropStoredProcedure("[dbo].[insert_DM_KhuVuc]");
            DropStoredProcedure("[dbo].[insert_DM_ViTri]");
            DropStoredProcedure("[dbo].[Update_HoaDonNHCFB]");
        }
    }
}
