namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20180530 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[LoadFirstDanhMucHangHoaSort]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid()
            }, body: @"DECLARE @TonKhoTheoChiNhanh TABLE(
    		ID_DonViQuiDoi uniqueidentifier,
    		ID_HangHoaCungLoai uniqueidentifier,
    		GiaVon float,
    		GiaBan float,
    		TonKho float
    	);
    	INSERT INTO @TonKhoTheoChiNhanh exec LoadAllDanhMucHangHoa @ID_ChiNhanh
    	
    DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    Set @timeStart =  (select convert(datetime, '2018/01/01'))
    Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    if (@SQL > 0)
    BEGiN
    Select @timeStart =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
    END
    Select  aa.ID as ID_DonViQuiDoi,aa.ID_HangHoa as ID,aa.TonToiThieu, aa.TonToiDa, aa.LaHangHoa,aa.LaChaCungLoai, aa.DuocBanTrucTiep,aa.TrangThai,aa.NgayTao, aa.ID_HangHoaCungLoai, aa.MaHangHoa, aa.ID_NhomHangHoa, aa.TenNhomHangHoa as NhomHangHoa, aa.TenHangHoa, aa.TenDonViTinh, aa.TenHangHoa_KhongDau, aa.TenHangHoa_KyTuDau, aa.GiaVon, aa.GiaBan, --aa.TonKho As TonKho_ChotSo, ISNULL(bb.TonCuoiKy, 0) as XuatNhapTon, 
    (ISNULL(aa.TonKho, 0) + ISNULL(bb.TonCuoiKy, 0)) as TonKho INTO #tblTonFirst FROM (
    (select dvqd.ID,hh.ID as ID_HangHoa,hh.TonToiThieu, hh.TonToiDa, hh.ID_HangHoaCungLoai,hh.LaHangHoa,hh.LaChaCungLoai, hh.DuocBanTrucTiep,hh.TheoDoi as TrangThai,hh.NgayTao, dvqd.MaHangHoa, hh.TenHanghoa,nhh.ID as ID_NhomHangHoa, nhh.TenNhomHangHoa, hh.TenHangHoa_KhongDau, hh.TenhangHoa_KyTuDau, dvqd.TenDonViTinh, dvqd.GiaVon,dvqd.GiaBan, ISNULL(cs.TonKho / dvqd.tylechuyendoi, 0) as tonkho 
    from DM_HangHoa hh
    left join DonViQuiDoi dvqd on hh.ID = dvqd.ID_hangHoa
    left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID 
    left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    where dvqd.xoa is null and dvqd.ladonvichuan = 1 and hh.LaChaCungLoai = 1 and hh.TheoDoi =1) aa
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
    WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') and bhd.ChoThanhToan = 'false'  and hh.LaHangHoa = 1
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
    --    AND bhd.ID_DonVi = @ID_ChiNhanh
    	AND bhd.NgayLapHoaDon >= @timeStart 
    GROUP BY bhdct.ID_DonViQuiDoi 
    ) AS td 
    GROUP BY td.ID_DonViQuiDoi
    )
    AS HangHoa
    	LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    
    GROUP BY dhh.ID, dhh.TenHangHoa
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	)bb on aa.ID = bb.ID_DonViQuiDoi)
    		Select 
    			 hhtb2.ID_DonViQuiDoi,
    			 hhtb2.ID,
    			 hhtb2.LaHangHoa, 
    			 hhtb2.LaChaCungLoai, 
    			 hhtb2.DuocBanTrucTiep,
    			 hhtb2.TrangThai,
    			 hhtb2.NgayTao, 
    			 hhtb2.ID_HangHoaCungLoai, 
    			 hhtb2.MaHangHoa, 
    			 hhtb2.ID_NhomHangHoa, 
    			 hhtb2.NhomHangHoa, 
    			 hhtb2.TenHangHoa, 
    			 hhtb2.TenDonViTinh, 
    			 hhtb2.TenHangHoa_KhongDau, 
    			 hhtb2.TenHangHoa_KyTuDau, 
    			 hhtb2.TonToiThieu,
    			 hhtb2.TonToiDa, 
    			 CASE
    					WHEN hhtb2.LaChaCungLoai = '1'
    						THEN
    							(SELECT SUM(tmphhtb2.GiaBan) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)/(SELECT Count(tmphhtb2.GiaBan) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)
    					ELSE
    						hhtb2.GiaBan
    				END AS GiaBan,
    			 CASE
    					WHEN hhtb2.LaChaCungLoai = '1'
    						THEN
    							(SELECT SUM(tmphhtb2.GiaVon) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)/(SELECT Count(tmphhtb2.GiaVon) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)
    					ELSE
    						hhtb2.GiaVon
    				END AS GiaVon,
    			 CASE
    					WHEN hhtb2.LaChaCungLoai = '1'
    						THEN
    							(SELECT SUM(tmphhtb2.TonKho) FROM @TonKhoTheoChiNhanh tmphhtb2 WHERE tmphhtb2.ID_HangHoaCungLoai = hhtb2.ID_HangHoaCungLoai)
    					ELSE
    						hhtb2.TonKho
    				END AS TonKho
    		from #tblTonFirst hhtb2");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[LoadFirstDanhMucHangHoaSort]");
        }
    }
}