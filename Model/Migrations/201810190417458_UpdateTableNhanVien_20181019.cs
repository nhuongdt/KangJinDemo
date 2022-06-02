namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableNhanVien_20181019 : DbMigration
    {
        public override void Up()
        {
            
            Sql(@"ALTER PROCEDURE [dbo].[Search_DMHangHoaLoHang_TonKho_ChotSo]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)
    -- lấy ngày chốt sổ
    DECLARE @timeChotSo Datetime
    	DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    	  Select @count =  (Select count(*) from @tablename);
    	    Select @countChar =   (Select count(*) from @tablenameChar);
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	Select Top(20)
    		tr.ID_DonViQuiDoi,
    		MAX(tr.MaHangHoa) as MaHangHoa,
    		MAX(tr.TenHangHoa) as TenHangHoa,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(tr.TenDonViTinh) as TenDonViTinh,
    		tr.QuanLyTheoLoHang,
    		--gv.GiaVon as GiaVon,
    		Case When @XemGiaVon != '1' or gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
    		MAX(tr.GiaBan) as GiaBan,
    		Sum(tr.TonCuoiKy) as TonCuoiKy,
    		MAX(tr.SrcImage) as SrcImage,
    		Case when tr.ID_LoHang is null then NEWID() else tr.ID_LoHang end as ID_LoHang,
    		MAX(tr.TenLoHang) as TenLoHang,
    		MAX(tr.NgaySanXuat) as NgaySanXuat,
    		MAX(tr.NgayHetHan) as NgayHetHan
    		 FROM
    		(
    		SELECT dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
    		a.TenHH AS TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    			a.QuanLyTheoLoHang,
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,
    			an.URLAnh as SrcImage,
    				a.ID_LoHang,
    				a.MaLoHang as TenLoHang,
    				a.NgaySanXuat,
    				a.NgayHetHan,
    				a.TrangThai
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    			 Select  
    			 dhh1.ID,
    			 dhh1.LaHangHoa,
    			 dhh1.TenHangHoa as TenHH,
    			 dhh1.TenHangHoa,
    			 dhh1.TenHangHoa_KhongDau,
    			 dhh1.TenHangHoa_KyTuDau,
    			 dvqd1.TenDonViTinh,
    			 dhh1.QuanLyTheoLoHang,
    			 lh1.ID as ID_LoHang,
    			 lh1.MaLoHang,
    			 lh1.NgaySanXuat,
    			 lh1.NgayHetHan,
    			 lh1.TrangThai,
    			 0 as TonCuoiKy
    			 from
    			 DonViQuiDoi dvqd1 
    			 join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
    			 left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa
    			  where dvqd1.xoa is null and dhh1.TheoDoi = '1'
    			 Union all
    
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
    			MAX(dhh.TenHangHoa) As TenHH,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' + MAX(dhh.TenHangHoa)  AS TenHangHoa,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) + ' ' +  MAX(dhh.TenHangHoa_KhongDau) AS TenHangHoa_KhongDau,
    		MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end)  + ' ' + MAX(dhh.TenHangHoa_KyTuDau) AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    			dhh.QuanLyTheoLoHang,
    			MAX(lh.ID)  As ID_LoHang,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = '0' then '' else lh.MaLoHang end) As MaLoHang,
    			MAX(lh.NgaySanXuat)  As NgaySanXuat,
    			MAX(lh.NgayHetHan)  As NgayHetHan,
    			lh.TrangThai,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
    			left join
    			(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    					where dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo
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
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi,ID_LoHang,hh.QuanLyTheoLoHang         
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1
    		GROUP BY dhh.ID,dhh.LaHangHoa,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    right Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
    	LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
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
    where ((select count(*) from @tablename b where 
    	dvqd3.MaHangHoa like '%'+b.Name+'%' 
    		--or a.MaLoHang like '%'+b.Name+'%' 
    		or a.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or a.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			)=@count or @count=0)
    	and ((select count(*) from @tablenameChar c where
    		dvqd3.MaHangHoa like '%'+c.Name+'%' 
    				--or a.MaLoHang like '%'+c.Name+'%' 
    		or a.TenHangHoa like '%'+c.Name+'%' 
    			)= @countChar or @countChar=0)
    	and dvqd3.Xoa is null
    		and ((a.QuanLyTheoLoHang = 1 and a.MaLoHang != '') or a.QuanLyTheoLoHang = 0)
    		)
    		tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		where tr.TrangThai = 1 or tr.TrangThai is null
    		Group by tr.ID_DonViQuiDoi, tr.ID_LoHang, tr.QuanLyTheoLoHang, gv.ID, gv.GiaVon
    		order by MAX(tr.NgayHetHan)
END");
        }
        
        public override void Down()
        {
            
        }
    }
}
