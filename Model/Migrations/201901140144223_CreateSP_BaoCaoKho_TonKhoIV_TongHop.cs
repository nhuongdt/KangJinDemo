namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSP_BaoCaoKho_TonKhoIV_TongHop : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_TonKhoIV_TongHop]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh_SP = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
	-- tách list ID
	DECLARE @tab as table(ID_ChiNhanh uniqueidentifier)
	Insert into @tab select * from splitstring(@ID_ChiNhanh_SP)
	DECLARE @ID_ChiNhanh uniqueidentifier;
	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT * FROM @tab
	OPEN CS_ItemUpDate 
	FETCH FIRST FROM CS_ItemUpDate INTO @ID_ChiNhanh
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @tmp as table(ID_ChiNhanh uniqueidentifier, TenChiNhanh nvarchar(max), SoLuong float, GiaTri float)
		Insert into @tmp
		SELECT 
			@ID_ChiNhanh as ID_ChiNhanh,
			(Select TenDonVi from DM_DonVi where ID = @ID_ChiNhanh) as TenChiNhanh,
    		Sum(tr.TonCuoiKy) as SoLuong,
    		Sum(tr.TonCuoiKy * ISNULL(gv.GiaVon, 0)) as GiaTri
    	FROM
    	(
		SELECT 
    	dvqd3.ID as ID_DonViQuiDoi,
    	a.ID_LoHang,
    	a.TenNhomHangHoa,
    	dvqd3.mahanghoa,
    	a.TenHangHoa,
    	CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    		MAX(lh.ID) as ID_LoHang,
    		Case when dhh.ID_NhomHang is not null then dhh.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang,
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		Case when MAx(dnhh.TenNhomHangHoa) is null then N'Nhóm mặc định' else MAX(dnhh.TenNhomHangHoa) end as TenNhomHangHoa,
    				Case when MAx(dnhh.TenNhomHangHoa_KhongDau) is null then N'nhom mac dinh' else MAX(dnhh.TenNhomHangHoa_KhongDau) end as TenNhomHangHoa_KhongDau,
    				Case when MAx(dnhh.TenNhomHangHoa_KyTuDau) is null then N'nmd' else MAX(dnhh.TenNhomHangHoa_KyTuDau) end as TenNhomHangHoa_KyTuDau,
    		SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonCuoiKy,
			Case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach) = 0 then 1 else MAX(dhh.QuyCach) end as QuyCach
    		FROM
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat
    			FROM
    			(
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon < @timeEnd
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang, hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang
    		) 
    		AS HangHoa
    		--LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    		where dhh.LaHangHoa like @LaHangHoa
    			and dhh.TheoDoi like @TheoDoi
    		GROUP BY dhh.ID , dhh.ID_NhomHang, dvqd.Xoa, HangHoa.ID_LoHang
    			) a
    			LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    				left join DM_LoHang lh on a.ID_LoHang = lh.ID
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
    							) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    			where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    				and (MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH) 
    				and LaDonViChuan = 1
    			and a.Xoa like @TrangThai
    			) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		--Group by gv.ID_DonVi 

		UPDATE @tab SET ID_ChiNhanh = NEWID() WHERE ID_ChiNhanh = @ID_ChiNhanh
	FETCH NEXT FROM CS_ItemUpDate INTO @ID_ChiNhanh
	END
	--select MAX(TenChiNhanh) as TenChiNhanh, Sum(SoLuong) as SoLuong, Sum(GiaTri) as GiaTri from @tmp
	--group by TenChiNhanh
	select 
	ID_ChiNhanh,
	TenChiNhanh,
	Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(SoLuong, 0), 3) as float) else 0 end as SoLuong,
	Case When @XemGiaVon = '1' then CAST(ROUND(ISNULL(GiaTri , 0), 0) as float) else 0 end as GiaTri
	 from @tmp
	 order by GiaTri DESC");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoKho_TonKhoIV_TongHop]");
        }
    }
}
