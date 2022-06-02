namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190118 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetAllDinhLuongDichVu]", parametersAction: p => new
            {
                ID_DonViQuiDoi = p.Guid(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT dvqd4.ID as ID_DonViQuiDoi,dvqd4.TyLeChuyenDoi,dvqd4.MaHangHoa,dvqd4.ID_HangHoa as ID, hh4.TenHangHoa,hh4.LaHangHoa, Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri, dvqd4.TenDonViTinh, hh4.QuanLyTheoLoHang,ISNULL(TonKhoTable.GiaVon,0) as GiaVon,
    	 dvqd4.GiaBan, dvqd4.GiaNhap, TonKho, ID_LoHang,MaLoHang, NgaySanXuat, NgayHetHan FROM
		DinhLuongDichVu dldv 
    	LEFT JOIN DonViQuiDoi dvqd4 on dldv.ID_DonViQuiDoi = dvqd4.ID
    	left join DM_HangHoa hh4 on dvqd4.ID_HangHoa = hh4.ID
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
    					) as ThuocTinh on dvqd4.ID_HangHoa = ThuocTinh.id_hanghoa
    	 left join(
    	
    	SELECT tableTon.ID_DonViQuiDoi, MaHangHoa,TenHangHoa, TenDonViTinh, QuanLyTheoLoHang, CAST(ROUND(gv.GiaVon,0) as float) as GiaVon,
    	GiaBan, GiaNhap, TonKho, tableTon.ID_LoHang, MaLoHang, NgaySanXuat, NgayHetHan FROM
    	(
    	SELECT Top(20) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHangHoa,
    			dvqd3.TenDonViTinh,
    			a.QuanLyTheoLoHang, 
    		CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan,
    			CAST(ROUND((dvqd3.GiaNhap), 0) as float) as GiaNhap,   
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonKho,
    			ISNULL(an.URLAnh,'/Content/images/iconbepp18.9/gg-37.png') as SrcImage,
    				a.ID_LoHang,
    				a.MaLoHang,
    				a.NgaySanXuat,
    				a.NgayHetHan
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
    				MAX(dhh.TenHangHoa) As TenHangHoa,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    			dhh.QuanLyTheoLoHang,
    			MAX(Case when lh.ID is null or dhh.QuanLyTheoLoHang = '0' then null else lh.ID end)  As ID_LoHang,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then null else lh.MaLoHang end) As MaLoHang,
    			MAX(lh.NgaySanXuat)  As NgaySanXuat,
    			MAX(lh.NgayHetHan)  As NgayHetHan,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
				DinhLuongDichVu dldv1 
    			LEFT JOIN DonViQuiDoi dvqd on dldv1.ID_DonViQuiDoi = dvqd.ID 
    			left join
    		(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					--and dvqd.MaHangHoa = @MaHH
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					--and dvqd.MaHangHoa = @MaHH
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					--and dvqd.MaHangHoa = @MaHH
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    					INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    					
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    					--and dvqd.MaHangHoa = @MaHH
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID and lh.TrangThai = 1
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1 and dldv1.ID_DichVu = @ID_DonViQuiDoi
    		GROUP BY dhh.ID, dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang
    ) a
    	LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    	Where ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
    order by a.NgayHetHan
    	) as tableTon
    	left join DM_GiaVon gv on (tableTon.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tableTon.ID_LoHang = gv.ID_LoHang or tableTon.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    	) as TonKhoTable on TonKhoTable.ID_DonViQuiDoi = dvqd4.ID
    	where dvqd4.Xoa is null and dldv.ID_DichVu = @ID_DonViQuiDoi");

            CreateStoredProcedure(name: "[dbo].[SP_UpdateTrangThaiKhachHang]", parametersAction: p => new
            {
                ID_TrangThai = p.String()
            }, body: @"Update DM_DoiTuong set ID_TrangThai = null where ID in (select ID from DM_DoiTuong where ID_TrangThai  like @ID_TrangThai)");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetAllDinhLuongDichVu]");
            DropStoredProcedure("[dbo].[SP_UpdateTrangThaiKhachHang]");
        }
    }
}
