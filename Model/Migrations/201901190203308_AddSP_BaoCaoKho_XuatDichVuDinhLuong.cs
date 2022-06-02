namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSP_BaoCaoKho_XuatDichVuDinhLuong : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoKho_XuatDichVuDinhLuong]", parametersAction: p => new
            {
                MaHH = p.String(),
                MaHH_TV = p.String(),
                LoaiChungTu = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
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
    	SELECT 
    		c.TenLoaiChungTu as LoaiHoaDon,
    		c.MaHoaDon as MaHoaDon,
    		c.NgayLapHoaDon as NgayLapHoaDon,
			c.NhomDichVu, c.MaDichVu, c.TenDichVu, c.TenDonViDichVu, c.SoLuongDichVu, c.GiaTriDichVu,
    		c.TenNhomHangHoa as TenNhomHang,
    		c.MaHangHoa,
    		c.TenHangHoaFull,
    		c.TenHangHoa,
    		c.ThuocTinh_GiaTri,
    		c.TenDonViTinh,
			c.SoLuongDinhLuongBanDau, 
			Case When @XemGiaVon = '1' then  c.GiaTriDinhLuongBanDau else 0 end as GiaTriDinhLuongBanDau,
			c.SoLuongXuat as SoLuongThucTe,
    		Case When @XemGiaVon = '1' then  c.GiaTriXuat else 0 end as GiaTriThucTe,
    		c.SoLuongChenhLech,
    		Case When @XemGiaVon = '1' then  c.GiaTriChenhLech else 0 end as GiaTriChenhLech,
			c.TrangThai
    	FROM
    	(
		 SELECT 
    		a.TenLoaiChungTu,
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
		
			Case when a.NhomDichVu is null then N'Nhóm mặc định' else a.NhomDichVu end as NhomDichVu,
			a.MaDichVu, a.TenDichVu, a.TenDonViDichVu,a.SoLuongDichVu, a.GiaTriDichVu,
    		Case when a.ID_NhomHang is not null then a.ID_NhomHang else '00000000-0000-0000-0000-000000000000' end as ID_NhomHang, 
    		Case when a.ID_NhomHang is null then N'Nhóm mặc định' else a.TenNhomHangHoa end as TenNhomHangHoa,
    		Case when a.ID_NhomHang is null then N'nhom mac dinh' else a.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    		Case when a.ID_NhomHang is null then N'nmd' else a.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    		dvqd3.mahanghoa,
			a.TenHangHoa + Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as TenHangHoaFull,
    		a.TenHangHoa,
    		a.TenHangHoa_KhongDau,
    		a.TenHangHoa_KyTuDau,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh,
			a.SoLuongDinhLuongBanDau, a.GiaTriDinhLuongBanDau,
			a.SoLuongXuat / dvqd3.TyLeChuyenDoi as SoLuongXuat,
    		a.GiaTriXuat as GiaTriXuat,
			a.SoLuongChenhLech / dvqd3.TyLeChuyenDoi as SoLuongChenhLech,
			a.GiaTriChenhLech,
			Case when a.SoLuongXuat = 0 then N'Không xuất'
				when a.SoLuongChenhLech < 0 then N'Xuất thiếu'
				when a.SoLuongChenhLech = 0 then N'Xuất đủ'
				when (a.SoLuongDinhLuongBanDau = 0) and a.SoLuongXuat > 0 then N'Xuất thêm'
				else N'Xuất thừa' end as TrangThai
    	FROM 
    		(
    		SELECT 
    		dhh.ID,
    		dhh.ID_NhomHang,	
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    		HangHoa.TenLoaiChungTu,
    		HangHoa.MaHoaDon,
    		HangHoa.NgayLapHoaDon AS NgayLapHoaDon,
			dvnh.TenNhomHangHoa AS NhomDichVu,
			qddv.MaHangHoa AS MaDichVu,
			hhdv.TenHangHoa AS TenDichVu,
			qddv.TenDonViTinh AS TenDonViDichVu,
			dhh.TenHangHoa,
    		dhh.TenHangHoa_KhongDau AS TenHangHoa_KhongDau,
    		dhh.TenHangHoa_KyTuDau AS TenHangHoa_KyTuDau,
    		dvqd.TenDonViTinh AS TenDonViTinh,
    		dnhh.TenNhomHangHoa AS TenNhomHangHoa,
    		dnhh.TenNhomHangHoa_KhongDau AS TenNhomHangHoa_KhongDau,
    		dnhh.TenNhomHangHoa_KyTuDau AS TenNhomHangHoa_KyTuDau,
			HangHoa.SoLuongDichVu,
			HangHoa.GiaTriDichVu,
			HangHoa.SoLuongDinhLuongBanDau,
			HangHoa.GiaTriDinhLuongBanDau,
    		HangHoa.SoLuongXuat,
    		HangHoa.GiaTriXuat,
			HangHoa.SoLuongXuat - HangHoa.SoLuongDinhLuongBanDau as SoLuongChenhLech,
			HangHoa.GiaTriXuat - HangHoa.GiaTriDinhLuongBanDau as GiaTriChenhLech
    		FROM
    		(
    				SELECT
    				pstk.ID_DonViQuiDoi,
    				pstk.LoaiHoaDon,
					pstk.ID_DichVu,
    				Case when pstk.LoaiHoaDon = 2 then N'Xuất sử dụng gói dịch vụ'
					when pstk.LoaiHoaDon = 3 then N'Xuất bán dịch vụ định lượng' else
					ct.TenLoaiChungTu end as TenLoaiChungTu,
    				pstk.MaHoaDon,
    				pstk.NgayLapHoaDon AS NgayLapHoaDon,
					CAST(ROUND(pstk.SoLuongDichVu, 3) as float) AS SoLuongDichVu,
    				CAST(ROUND(pstk.GiaTriDichVu, 0) as float) AS GiaTriDichVu,
					CAST(ROUND(pstk.SoLuongXuat, 3) as float) AS SoLuongXuat,
    				CAST(ROUND(pstk.GiaTriXuat, 0) as float) AS GiaTriXuat,
					CAST(ROUND(pstk.SoLuongDinhLuongBanDau, 3) as float) AS SoLuongDinhLuongBanDau,
    				CAST(ROUND(pstk.GiaTriDinhLuongBanDau, 0) as float) AS GiaTriDinhLuongBanDau
    				FROM 
    				(
    				SELECT 
    				bhd.MaHoaDon,
    				bhdct.ID_DonViQuiDoi,
					dvdl.ID_DonViQuiDoi as ID_DichVu,
    				Case when bhdct.ID_ChiTietGoiDV is not null then 2 
					when bhdct.ID_ChiTietGoiDV is null and bhdct.ID_ChiTietDinhLuong is not null then 3 
					else bhd.LoaiHoaDon end as LoaiHoaDon, 	
    				bhd.NgayLapHoaDon AS NgayLapHoaDon,
					ISNULL(dvdl.SoLuong,0) AS SoLuongDichVu,
					ISNULL(dvdl.SoLuong,0)* dvdl.GiaVon AS GiaTriDichVu,

    				ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi AS SoLuongXuat,
					ISNULL(bhdct.SoLuong,0)* bhdct.GiaVon AS GiaTriXuat,

					ISNULL(bhdct.SoLuongDinhLuong_BanDau,0)* dvqd.TyLeChuyenDoi AS SoLuongDinhLuongBanDau,
					ISNULL(bhdct.SoLuongDinhLuong_BanDau,0)* bhdct.GiaVon AS GiaTriDinhLuongBanDau
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				INNER JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
					LEFT JOIN BH_HoaDon_ChiTiet dvdl on bhdct.ID_ChiTietDinhLuong = dvdl.ID
					
    				WHERE bhd.LoaiHoaDon = '1' AND dvqd.Xoa IS NULL 
					and bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
					AND bhdct.ID_ChiTietDinhLuong is not null -- thành phần định lượng
					AND bhdct.ID_ChiTietDinhLuong != bhdct.ID
    				
    			) AS pstk
    				LEFT JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
    			--GROUP BY pstk.ID_DonViQuiDoi, pstk.LoaiHoaDon, ct.TenLoaiChungTu, pstk.MaHoaDon
    		) 
    		AS HangHoa
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang

			Join DonViQuiDoi qddv on HangHoa.ID_DichVu = qddv.ID
			Join DM_HangHoa hhdv on qddv.ID_HangHoa = hhdv.ID
    		LEFT JOIN DM_NhomHangHoa dvnh ON dvnh.ID = hhdv.ID_NhomHang

    		where dhh.LaHangHoa like @LaHangHoa
    			and HangHoa.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
    				and dhh.TheoDoi like @TheoDoi
    		--GROUP BY dhh.ID , dhh.ID_NhomHang, HangHoa.TenLoaiChungTu, HangHoa.MaHoaDon, dvqd.Xoa
    		) a
    		LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
		  where LaDonViChuan = 1
    	and a.Xoa like @TrangThai
    ) c
    		where (c.ID_NhomHang like @ID_NhomHang or c.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
    		and (MaHoaDon like @MaHH_TV or MaHoaDon like @MaHH or MaHangHoa like @MaHH_TV or MaHangHoa like @MaHH or TenHangHoa_KhongDau like @MaHH or TenHangHoa_KyTuDau like @MaHH or TenNhomHangHoa_KhongDau like @MaHH or TenNhomHangHoa_KyTuDau like @MaHH or MaDichVu like @MaHH_TV or TrangThai like @MaHH_TV) 
    	ORDER BY NgayLapHoaDon DESC, MaDichVu");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoKho_XuatDichVuDinhLuong]");
        }
    }
}
