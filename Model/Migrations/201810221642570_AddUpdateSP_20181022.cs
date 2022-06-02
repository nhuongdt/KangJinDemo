namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181022 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getList_NhatKySuDung]", parametersAction: p => new
            {
                ID_NhanVien = p.String(),
                ID_ChiNhanh = p.Guid(),
                NoiDung = p.String(),
                ChucNang = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ThaoTac = p.String()
            }, body: @"Select nv.TenNhanVien,
	nk.ChucNang,
	--dbo.fChuyenCoDauThanhKhongDau(nk.ChucNang) as ChucNang_CV,
	--dbo.LayKyTuDau(nk.ChucNang) as ChucNang_GC,
	nk.ThoiGian,
	nk.NoiDung,
	--dbo.fChuyenCoDauThanhKhongDau(nk.NoiDung) as NoiDung_CV,
	--dbo.LayKyTuDau(nk.NoiDung) as NoiDung_GC,
	nk.NoiDungChiTiet,
	nk.LoaiNhatKy
	FROM HT_NhatKySuDung nk
	join NS_NhanVien nv on nk.ID_NhanVien = nv.ID
	where nk.ThoiGian >= @timeStart and nk.ThoiGian < @timeEnd and nk.ID_DonVi = @ID_ChiNhanh and 
	nk.ID_NhanVien in (select * from splitstring(@ID_NhanVien)) and nk.LoaiNhatKy in (select * from splitstring(@ThaoTac))
	and nk.ChucNang like @ChucNang
	and nk.NoiDung like @NoiDung
	order by nk.ThoiGian DESC");

            CreateStoredProcedure(name: "[dbo].[SP_CheckMaDoiTuong_Exist]", parametersAction: p => new
            {
                MaDoiTuong = p.String()
            }, body: @"DECLARE @valReturn bit ='0'
	DECLARE @ID_DoiTuong nvarchar(max);
	SELECT @ID_DoiTuong= ID from DM_DoiTuong WHERE MaDoiTuong like @MaDoiTuong

	IF @ID_DoiTuong IS NULL SET @valReturn= '0'
    ELSE SET @valReturn= '1'

	SELECT @valReturn AS Exist");

            CreateStoredProcedure(name: "[dbo].[SP_CheckSoDienThoaiKH_Exist]", parametersAction: p => new
            {
                DienThoai = p.String()
            }, body: @"DECLARE @valReturn bit ='0'
	DECLARE @ID_DoiTuong nvarchar(max);
	-- TheoDoi = 1: deleted
	SELECT @ID_DoiTuong= ID from DM_DoiTuong WHERE TheoDoi !='1' and DienThoai like @DienThoai

	IF @ID_DoiTuong IS NULL SET @valReturn= '0'
    ELSE SET @valReturn= '1'
	SELECT @valReturn AS Exist");

            Sql(@"ALTER PROCEDURE [dbo].[Search_DMHangHoa_TonKho_ChotSo]
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
    	SELECT Top(20) dvqd3.ID as ID_DonViQuiDoi, dvqd3.MaHangHoa,
			a.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd3.TenDonVitinh = '' or dvqd3.TenDonViTinh is null then '' else ' (' + dvqd3.TenDonViTinh + ')' end as TenHangHoaFull,
    		a.TenHangHoa,
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		dvqd3.TenDonViTinh, 
    	Case When @XemGiaVon = '1' then CAST(ROUND((dvqd3.GiaVon), 0) as float) else 0 end as GiaVon, 
    	CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
    		Case when a.LaHangHoa = 0 then 0 else CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) end as TonCuoiKy,
    			an.URLAnh as SrcImage
    	--CAST(ROUND((a.TonCuoiKy / dvqd3.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM 
    		 (
    		SELECT 
    		dhh.ID,
    			dhh.LaHangHoa,
    		MAX(dhh.TenHangHoa)   AS TenHangHoa,
    		MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    		MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    		MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    			DonViQuiDoi dvqd 
    			left join
    			(
    			SELECT
    			td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    				--SUM(ISNULL(td.TonKho,0)) AS TonCuoiKy
    			FROM
    			(
    			-- lấy danh sách hàng hóa tồn kho
    				SELECT 
    				dvqd.ID As ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				NULL AS SoLuongXuat,
    				SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    					where dvqd.ladonvichuan = '1'
    						GROUP BY dvqd.ID
    				UNION ALL
    
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				NULL AS SoLuongNhap,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    
    				UNION ALL
    				SELECT 
    				bhdct.ID_DonViQuiDoi,
    				SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    				null AS SoLuongXuat,
    				NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    				--AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi
    		) AS HangHoa
    			on dvqd.ID = HangHoa.ID_DonViQuiDoi
    		--LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		INNER JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
    			Where dvqd.Xoa is null and dhh.TheoDoi = 1
    		GROUP BY dhh.ID,dhh.LaHangHoa
    ) a
    LEFT Join DonViQuiDoi dvqd3 on a.ID = dvqd3.ID_HangHoa
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
    		or a.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or a.TenHangHoa_KyTuDau like '%'+b.Name+'%' )=@count or @count=0)
    	and ((select count(*) from @tablenameChar c where
    		dvqd3.MaHangHoa like '%'+c.Name+'%' 
    		or a.TenHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    	--(dvqd3.MaHangHoa like @MaHH or a.TenHangHoa_KhongDau like @MaHH or a.TenHangHoa_KyTuDau like @MaHH or dvqd3.MaHangHoa like @MaHH_TV)
    	and dvqd3.Xoa is null
    order by TonCuoiKy desc
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_NhatKySuDung]");
            DropStoredProcedure("[dbo].[SP_CheckMaDoiTuong_Exist]");
            DropStoredProcedure("[dbo].[SP_CheckSoDienThoaiKH_Exist]");
        }
    }
}