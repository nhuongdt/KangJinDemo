namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190419 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_BieuDoThucThuToDay]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_NguoiDung = p.Guid(),
                ID_DonVi = p.String()
            }, body: @"DECLARE @LaAdmin as nvarchar
    	Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd	where nd.ID = @ID_NguoiDung)
	 IF(@LaAdmin = 1)
	 BEGIN
		SELECT 
			a.NgayLapHoaDon,
			a.TenChiNhanh,
			CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
			FROM
			(
    			SELECT
				DAY(qhd.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
				ISNULL(qct.TienThu, 0) as ThanhTien
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				join DM_DonVi dv on qhd.ID_DonVi = dv.ID
				where qhd.LoaiHoaDon = 11
				and (qhd.TrangThai is null or qhd.TrangThai != 0)
				and qct.DiemThanhToan is null
				and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
				and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
			) a
    		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
			ORDER BY NgayLapHoaDon
	END
	ELSE
	BEGIN
		SELECT 
			a.NgayLapHoaDon,
			a.TenChiNhanh,
			CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
			FROM
			(
    			SELECT
				DAY(qhd.NgayLapHoaDon) as NgayLapHoaDon,
				dv.TenDonVi as TenChiNhanh,
				ISNULL(qct.TienThu, 0) as ThanhTien
				from Quy_HoaDon_ChiTiet qct
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				join DM_DonVi dv on qhd.ID_DonVi = dv.ID
				where qhd.LoaiHoaDon = 11
				and (qhd.TrangThai is null or qhd.TrangThai != 0)
				and qct.DiemThanhToan is null
				and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
				and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
				and qhd.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
			) a
    		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
			ORDER BY NgayLapHoaDon
	END");

            CreateStoredProcedure(name: "[dbo].[BaoCaoTongQuan_BieuDoThucThuToHour]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_NguoiDung = p.Guid(),
                ID_DonVi = p.String()
            }, body: @"DECLARE @LaAdmin as nvarchar
     Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd	where nd.ID = @ID_NguoiDung)
	 IF(@LaAdmin = 1)
	 BEGIN
		SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
		SELECT
		DATEPART(HOUR, qhd.NgayLapHoaDon) as NgayLapHoaDon,
		dv.TenDonVi as TenChiNhanh,
		ISNULL(qct.TienThu, 0) as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		join DM_DonVi dv on qhd.ID_DonVi = dv.ID
		where qhd.LoaiHoaDon = 11
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.DiemThanhToan is null
		and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
		and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
		) a
		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon
	END
	ELSE
	BEGIN
		SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
		SELECT
		DATEPART(HOUR, qhd.NgayLapHoaDon) as NgayLapHoaDon,
		dv.TenDonVi as TenChiNhanh,
		ISNULL(qct.TienThu, 0) as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		join DM_DonVi dv on qhd.ID_DonVi = dv.ID
		where qhd.LoaiHoaDon = 11
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.DiemThanhToan is null
		and qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
		and qhd.ID_DonVi in (select * from splitstring(@ID_DonVi))
		and qhd.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
		) a
		GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon
	END");

            CreateStoredProcedure(name: "[dbo].[SP_GetHT_CauHinh_TichDiem]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"select ct.ID as ID_TichDiem,
		ISNULL(ad.ID,'00000000-0000-0000-0000-000000000000') as ID_ApDung,
		ISNULL(ad.ID_NhomDoiTuong,'00000000-0000-0000-0000-000000000000')  as ID_NhomDoiTuong,
		ct.TyLeDoiDiem,
		ct.ThanhToanBangDiem,
		ct.KhoiTaoTichDiem,
		ISNUll(ct.TichDiemGiamGia,'0') as TichDiemGiamGia,
		ct.TichDiemHoaDonDiemThuong,
		ct.TienThanhToan,
		ct.DiemThanhToan,
		ct.ToanBoKhachHang,
		ISNUll(ct.TichDiemHoaDonGiamGia,'0') as TichDiemHoaDonGiamGia,
		ct.SoLanMua
	from HT_CauHinh_TichDiemChiTiet ct
	left join HT_CauHinh_TichDiemApDung ad on ct.ID= ad.ID_TichDiem
	join HT_CauHinhPhanMem ch on ct.ID_CauHinh = ch.ID
	where ch.ID_DonVi like @ID_DonVi");

            CreateStoredProcedure(name: "[dbo].[SP_GetNhanVien_NguoiDung]", parametersAction: p => new
            {
                ID_ChiNhanh = p.String()
            }, body: @"SELECT NS_NhanVien.ID, MaNhanVien,TenNhanVien, ISNULL( HT_NguoiDung.ID,'00000000-0000-0000-0000-000000000000') as ID_NguoiDung, '' as TenNhanVien_GC, '' as TenNhanVien_CV FROM NS_NhanVien
LEFT JOIN NS_QuaTrinhCongTac ON NS_NhanVien.ID = NS_QuaTrinhCongTac.ID_NhanVien
LEFT JOIN HT_NguoiDung ON NS_NhanVien.ID = HT_NguoiDung.ID_NhanVien
WHERE NS_QuaTrinhCongTac.ID_DonVi like '%'+ @ID_ChiNhanh + '%' or NS_QuaTrinhCongTac.ID_DonVi is null");

            CreateStoredProcedure(name: "[dbo].[SP_ReportDiscountProduct_Detail]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                DateFrom = p.String(),
                DateTo = p.String(),
                Status_ColumHide = p.Int()
            }, body: @"select *,
		case @Status_ColumHide
					when  1 then cast(0 as float)
					when  2 then ISNULL(HoaHongBanGoiDV,0.0)
					when  3 then ISNULL(HoaHongTuVan,0.0)
					when  4 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0)
					when  5 then ISNULL(HoaHongThucHien,0.0) 
					when  6 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0)
					when  7 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0)
		else ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0)
		end as TongAll
	from
	(
			Select 
    			hd.MaHoaDon,
    			hd.NgayLapHoaDon,
    			dvqd.MaHangHoa,
				nv.MaNhanVien,
    			nv.TenNhanVien,
				hh.TenHangHoa + Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as TenHangHoaFull,
				hh.TenHangHoa,
				--ISNULL(hdct.ThanhTien,0)as ThanhTien,
				-- tinh gia tri CK (thanhtien- phi dich vu)
				case when hh.ChiPhiThucHien > 0 then
					case when hh.ChiPhiTinhTheoPT =1 then ThanhTien -(ThanhTien * hh.ChiPhiThucHien/100)
					else ThanhTien- hh.ChiPhiThucHien* hdct.SoLuong end 
				else ThanhTien end as ThanhTien,
				ISNULL(dvqd.TenDonVitinh,'')  as TenDonViTinh,
				ISNULL(lh.MaLoHang,'')  as TenLoHang,
				Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    			Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    			Case when ck.ThucHien_TuVan = 0 and ck.TheoYeuCau = 0 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
				Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
				Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien,
    			Case when ck.ThucHien_TuVan = 0 and ck.TheoYeuCau = 0 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTTuVan,
				Case when ck.TheoYeuCau = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTBanGoi				
    		from
    		BH_NhanVienThucHien ck
    		inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    		inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    		inner join NS_NhanVien nv on ck.ID_NhanVien = nv.ID
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
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
    						) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    		Where hd.ChoThanhToan = 0  
			and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
			and CONVERT(varchar, hd.NgayLapHoaDon,23) >= @DateFrom 
			and CONVERT(varchar, hd.NgayLapHoaDon,23) <= @DateTo   		
	) tbl
	OrDER BY NgayLapHoaDon DESC");

            CreateStoredProcedure(name: "[dbo].[SP_ReportDiscountProduct_General]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                DateFrom = p.String(),
                DateTo = p.String(),
                Status_ColumHide = p.Int()
            }, body: @"SELECT 
    		a.MaNhanVien,
    		MAX(a.TenNhanVien) as TenNhanVien,
    		CAST(ROUND(SUM(a.HoaHongThucHien), 0) as float) as HoaHongThucHien,
    		CAST(ROUND(SUM(a.HoaHongTuVan), 0) as float) as HoaHongTuVan,
			CAST(ROUND(SUM(a.HoaHongBanGoiDV), 0) as float) as HoaHongBanGoiDV,
			case @Status_ColumHide
				when  1 then cast(0 as float)
				when  2 then SUM(ISNULL(HoaHongBanGoiDV,0.0))
				when  3 then SUM(ISNULL(HoaHongTuVan,0.0))
				when  4 then SUM(ISNULL(HoaHongBanGoiDV,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
				when  5 then SUM(ISNULL(HoaHongThucHien,0.0)) 
				when  6 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0))
				when  7 then SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0))
			else SUM(ISNULL(HoaHongThucHien,0.0)) + SUM(ISNULL(HoaHongTuVan,0.0)) + SUM(ISNULL(HoaHongBanGoiDV,0.0))
		end as Tong
    	FROM
    	(
    	Select 
    		nv.MaNhanVien,
    		nv.TenNhanVien,
			Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    		Case when ck.ThucHien_TuVan = 0 and ck.TheoYeuCau = 0 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
			Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV
    	from
    	BH_NhanVienThucHien ck
    	inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    	inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    	inner join NS_NhanVien nv on ck.ID_NhanVien = nv.ID
    	Where hd.ChoThanhToan = 0 
		and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
		and CONVERT(varchar, hd.NgayLapHoaDon,23) >= @DateFrom 
		and CONVERT(varchar, hd.NgayLapHoaDon,23) <= @DateTo

    	) a
    	GROUP BY a.MaNhanVien
    	ORDER BY MaNhanVien DESC");

            CreateStoredProcedure(name: "[dbo].[SP_ValueCard_GetListHisUsed]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                ID_KhachHang = p.String(),
                DateFrom = p.String(),
                DateTo = p.String()
            }, body: @"DECLARE @TblHisCard TABLE(
    				STT INT, ID UNIQUEIDENTIFIER, ID_DoiTuong UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(50),TenDoiTuong NVARCHAR(500), MaHoaDon NVARCHAR(500),  SLoaiHoaDon nvarchar(max), 
    				MaHoaDonSQ NVARCHAR(MAX),LoaiHoaDonSQ INT, NgayLapHoaDon DATETIME, TienThe FLOAT, ThuChiThe FLOAT, SoDuTruoc FLOAT, SoDuSau FLOAT, TrangThai_TheGiaTri int)
    	INSERT INTO @TblHisCard
    		SELECT 
				ROW_NUMBER() OVER(ORDER BY qhd.ID) AS STT,
				qhd.ID,
				dt.ID as ID_DoiTuong,-- used to caculator sodutruoc phatsinh
				dt.MaDoiTuong,
				dt.TenDoiTuong,
				MaHoaDons,
				ISNULL(LoaiHoaDons, N'Loại khác, ') as LoaiHoaDons,
				qhd.MaHoaDon as MaHoaDonSQ, 
				qhd.LoaiHoaDon as LoaiHoaDonSQ,
				qhd.NgayLapHoaDon,
				SUM(ISNULL(qct.ThuTuThe,0)) as TienThe,
				case when qhd.LoaiHoaDon = 11 then - SUM(ISNULL(qct.ThuTuThe,0)) else SUM(ISNULL(qct.ThuTuThe,0)) end as ThuChiThe,
				0 as SoDuTruoc,
				0 as SoDuSau,
				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
				else '12' end as TrangThai
			FROM Quy_HoaDon qhd
			join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
			join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
			join (
				--merger text MaHoaDon to 1 row
				Select distinct qhdXML.ID, 
							 (
						select distinct hd.MaHoaDon +', '  AS [text()]
						from BH_HoaDon hd
						join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
						where qct.ID_HoaDon = qhdXML.ID
						and hd.LoaiHoaDon in (1, 3, 6, 19)
						and ChoThanhToan ='0'
						For XML PATH ('')
					) MaHoaDons
				from Quy_HoaDon qhdXML
			) tbl on qhd.ID= tbl.ID
			

			-- get LoaiHoaDon
			join (
				Select distinct qhdXML2.ID, 
					 (
					 -- merger text LoaiHoaDon to 1 row
						select distinct 
							 tbl1.SLoaiHoaDon +', '  AS [text()]
						from 
							(
							-- get text HoaDon by LoaiHoaDon
							select 
								case hd.LoaiHoaDon
									when 1 then N'Bán hàng'
									when 3 then N'Đặt hàng'
									when 6 then N'Trả hàng'
								else 
									N'Gói dịch vụ' end as SLoaiHoaDon
							from BH_HoaDon hd
							join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
							where hd.LoaiHoaDon in (1, 3, 6, 19)
							and ChoThanhToan ='0' 
							and qct.ID_HoaDon = qhdXML2.ID
							) tbl1
						For XML PATH ('')
					) LoaiHoaDons
				from Quy_HoaDon qhdXML2
		) tbl2 on qhd.ID= tbl2.ID

	where (qhd.TrangThai = 1 or qhd.TrangThai is null)		
	and qct.ThuTuThe > 0
	and CONVERT(varchar, qhd.NgayLapHoaDon, 112) >= @DateFrom 
	and CONVERT(varchar, qhd.NgayLapHoaDon, 112) <= @DateTo
	and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
	and dt.ID like @ID_KhachHang
	GROUP BY qhd.ID,
		dt.ID,
		dt.MaDoiTuong,
		dt.TenDoiTuong,
		qhd.MaHoaDon,
		qhd.LoaiHoaDon,
		qhd.NgayLapHoaDon,
		dt.TrangThai_TheGiaTri,
		MaHoaDons,
		LoaiHoaDons
    		    	
    			DECLARE @SoDuTruocPhatSinh INT;
    
    			DECLARE @STT INT;
    			DECLARE @ID UNIQUEIDENTIFIER;
    			DECLARE @ID_DoiTuong UNIQUEIDENTIFIER;
    			DECLARE @MaDoiTuong NVARCHAR(50);
    			DECLARE @TenDoiTuong NVARCHAR(500);
    			DECLARE @MaHoaDon NVARCHAR(500);
    			DECLARE @LoaiHoaDon nvarchar(max);
    			DECLARE @MaHoaDonSQ NVARCHAR(50);
    			DECLARE @LoaiHoaDonSQ INT;
    			DECLARE @NgayLapHoaDon DATETIME;
    			DECLARE @TienThe FLOAT;
    			DECLARE @ThuChiThe FLOAT;
    			DECLARE @SoDuTruoc FLOAT;
    			DECLARE @SoDuSau FLOAT;
    			DECLARE @TrangThai_TheGiaTri INT;
    
    			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR SELECT STT, ID,ID_DoiTuong, MaDoiTuong, TenDoiTuong, MaHoaDon, SLoaiHoaDon, MaHoaDonSQ, LoaiHoaDonSQ, NgayLapHoaDon, TienThe, ThuChiThe, SoDuTruoc, SoDuSau, TrangThai_TheGiaTri
    			FROM @TblHisCard
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe,@ThuChiThe, @SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
    					SET @SoDuTruocPhatSinh = [dbo].[TinhSoDuKHTheoThoiGian](@ID_DoiTuong,@NgayLapHoaDon)
    
    					UPDATE @TblHisCard SET SoDuTruoc= @SoDuTruocPhatSinh, SoDuSau = @SoDuTruocPhatSinh + @ThuChiThe
    					WHERE STT = @STT
    
    					FETCH NEXT FROM CS_TheGT INTO @STT,@ID, @ID_DoiTuong, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe, @ThuChiThe,@SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    
    					
    			END
    		CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT
    
    		SELECT ID, ID_DoiTuong,MaDoiTuong,TenDoiTuong,
				LEFT(MaHoaDon, LEN(MaHoaDon) - 1) as MaHoaDon,
				LEFT(SLoaiHoaDon, LEN(SLoaiHoaDon) - 1) as SLoaiHoaDon,
				MaHoaDonSQ, LoaiHoaDonSQ,NgayLapHoaDon,TienThe,SoDuTruoc, SoDuSau, TrangThai_TheGiaTri				
    		FROM @TblHisCard 
    		WHERE TrangThai_TheGiaTri like '%11%' --11.DangHoatDong
    		ORDER BY NgayLapHoaDon desc");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoChietKhau_ChiTiet]
    @MaNV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
	@ID_NhanVien [nvarchar](max),
	@ID_NhanVien_SP [nvarchar](max),
    @ThucHien_TuVan [nvarchar](max)
AS
BEGIN
    Select 
    	hd.MaHoaDon,
    	hd.NgayLapHoaDon,
    	dvqd.MaHangHoa,
    hh.TenHangHoa + Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as TenHangHoaFull,
    hh.TenHangHoa,
    Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else dvqd.TenDonViTinh end as TenDonViTinh,
    Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    	nv.MaNhanVien,
    	nv.TenNhanVien,
    	Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    	Case when ck.ThucHien_TuVan = 0 and ck.TheoYeuCau = 0 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
		Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
		Case when ck.ThucHien_TuVan = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien,
    	Case when ck.ThucHien_TuVan = 0 and ck.TheoYeuCau = 0 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTTuVan,
		Case when ck.TheoYeuCau = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTBanGoi
    	from
    	BH_NhanVienThucHien ck
    	inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    	inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    	inner join NS_NhanVien nv on ck.ID_NhanVien = nv.ID
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
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
    					) as ThuocTinh on dvqd.ID_HangHoa = ThuocTinh.id_hanghoa
    	Where hd.ChoThanhToan = 0 AND 
		(nv.ID like @ID_NhanVien or nv.ID in (SELECT * FROM splitstring(@ID_NhanVien_SP)))
		AND (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	and ck.ThucHien_TuVan like @ThucHien_TuVan
    	and (nv.MaNhanVien like @MaNV or nv.TenNhanVien like @MaNV)
    	OrDER BY NgayLapHoaDon DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetDonVi_byUser]
    @ID_NguoiDung [uniqueidentifier],
    @TenDonVi [nvarchar](max)
AS
BEGIN
    Select 
    	dv.ID,
    	dv.TenDonVi,
    	dv.SoDienThoai,
		1 as checkSearch
    	From Dm_DonVi dv
    	inner join NS_QuaTrinhCongTac qtct on dv.ID = qtct.ID_DonVi
    	inner join NS_NhanVien nv on qtct.ID_NhanVien = nv.ID
    	inner join HT_NguoiDung nd on nv.ID = nd.ID_NhanVien	
    	where nd.ID = @ID_NguoiDung and (dv.TenDonVi like @TenDonVi or dv.SoDienThoai like @TenDonVi)
    		and (dv.TrangThai = 1 or dv.TrangThai is null)
    	Order by dv.TenDonVi
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetDonVi_byUserSeach]
    @ID_NguoiDung [uniqueidentifier],
    @TenDonVi [nvarchar](max)
AS
BEGIN
    Select 
    	dv.ID,
    	dv.TenDonVi,
    	dv.SoDienThoai,
		1 as checkSearch
    	From Dm_DonVi dv
    	inner join NS_QuaTrinhCongTac qtct on dv.ID = qtct.ID_DonVi
    	inner join NS_NhanVien nv on qtct.ID_NhanVien = nv.ID
    	inner join HT_NguoiDung nd on nv.ID = nd.ID_NhanVien	
    		inner join HT_NguoiDung_Nhom ndn on nd.ID = ndn.IDNguoiDung and dv.ID = ndn.ID_DonVi
    	where nd.ID = @ID_NguoiDung and (dv.TenDonVi like @TenDonVi or dv.SoDienThoai like @TenDonVi)
    		and (dv.TrangThai = 1 or dv.TrangThai is null)
    	Order by dv.TenDonVi
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetSoDuTheGiaTri_ByTime]
    @ID_DoiTuong [uniqueidentifier],
    @Time [datetime]
AS
BEGIN
    select dbo.TinhSoDuKHTheoThoiGian(@ID_DoiTuong,@Time)
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GettAll_BangGiaChiTiet]
AS
BEGIN
	SET NOCOUNT ON
    select gbct.ID,gbct.GiaBan, gbct.ID_DonViQuiDoi, gbct.ID_GiaBan as ID_BangGia, hh.ID as ID_HangHoa
    	from DM_GiaBan_ChiTiet gbct
    	left join DonViQuiDoi qd on gbct.ID_DonViQuiDoi= qd.ID
    	left join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    	where qd.Xoa is null OR qd.Xoa='0'
END");

            CreateStoredProcedure(name: "[dbo].[SP_ReportDiscountInvoice]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                DateFrom = p.String(),
                DateTo = p.String(),
                Status_ColumHide = p.Int()
            }, body: @"select MaNhanVien, 
		 TenNhanVien,
		SUM(ISNULL(HoaHongThucThu,0.0)) as HoaHongThucThu,
		SUM(ISNULL(HoaHongDoanhThu,0.0)) as HoaHongDoanhThu,
		SUM(ISNULL(HoaHongVND,0.0)) as HoaHongVND,

		case @Status_ColumHide
			when  1 then cast(0 as float)
			when  2 then SUM(ISNULL(HoaHongVND,0.0))
			when  3 then SUM(ISNULL(HoaHongThucThu,0.0))
			when  4 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
			when  5 then SUM(ISNULL(HoaHongDoanhThu,0.0)) 
			when  6 then SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
			when  7 then SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0))
		else SUM(ISNULL(HoaHongThucThu,0.0)) + SUM(ISNULL(HoaHongDoanhThu,0.0)) + SUM(ISNULL(HoaHongVND,0.0))
		end as TongAll
	from 
	(
		select MaNhanVien, TenNhanVien,
			--case when TinhChietKhauTheo =1 then TienChietKhau end as HoaHongThucThu,
			case when TinhChietKhauTheo =1 then
				case when tblQuy.TrangThai =0 then 0 else TienChietKhau end
			end as HoaHongThucThu,
			case when TinhChietKhauTheo =2 then TienChietKhau end as HoaHongDoanhThu,
			case when TinhChietKhauTheo =3 then TienChietKhau end as HoaHongVND
			
		from BH_NhanVienThucHien th
		join BH_HoaDon hd on th.ID_HoaDon= hd.ID
		join NS_NhanVien nv on th.ID_NhanVien= nv.ID
		left join (
			select ID_HoaDonLienQuan,TrangThai, sum(qct.ThuTuThe) as TongThu
			from quy_hoadon qhd
			join quy_hoadon_chitiet qct on qhd.ID= qct.ID_HoaDon
			where ISNULL(qct.DiemThanhToan,0)=0 and ISNULL(qct.ThuTuThe,0)=0
			group by qct.ID_HoaDonLienQuan, qhd.TrangThai
			) tblQuy on hd.ID= tblQuy.ID_HoaDonLienQuan
		where th.ID_HoaDon is not null
		and hd.ChoThanhToan =0
		and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
		and CONVERT(varchar, hd.NgayLapHoaDon,23) >= @DateFrom 
		and CONVERT(varchar, hd.NgayLapHoaDon,23) <= @DateTo
	) tbl
	group by MaNhanVien, TenNhanVien");

            CreateStoredProcedure(name: "[dbo].[SP_ReportDiscountInvoice_Detail]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                DateFrom = p.String(),
                DateTo = p.String(),
                Status_ColumHide = p.Int()
            }, body: @"select MaNhanVien, 
		 TenNhanVien,
		NgayLapHoaDon,
		MaHoaDon,
		PhaiThanhToan as DoanhThu,
		ISNULL(ThucThu,0) as ThucThu,
		ISNULL(HeSo,0) as HeSo,
		ISNULL(HoaHongThucThu,0) as HoaHongThucThu,
		ISNULL(HoaHongDoanhThu,0) as HoaHongDoanhThu,
		ISNULL(HoaHongVND,0) as HoaHongVND,
		ISNULL(PTThucThu,0) as PTThucThu,
		ISNULL(PTDoanhThu,0) as PTDoanhThu,
		case @Status_ColumHide
			when  1 then cast(0 as float)
			when  2 then ISNULL(HoaHongVND,0.0)
			when  3 then ISNULL(HoaHongThucThu,0.0)
			when  4 then ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongVND,0.0)
			when  5 then ISNULL(HoaHongDoanhThu,0.0) 
			when  6 then ISNULL(HoaHongDoanhThu,0.0) + ISNULL(HoaHongVND,0.0)
			when  7 then ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongDoanhThu,0.0)
		else ISNULL(HoaHongThucThu,0.0) + ISNULL(HoaHongDoanhThu,0.0) + ISNULL(HoaHongVND,0.0)
		end as TongAll
	from 
	(
		select MaNhanVien, TenNhanVien,hd.NgayLapHoaDon,hd.MaHoaDon, ISNULL(hd.PhaiThanhToan,0) as PhaiThanhToan,
			ThucThu, hd.LoaiHoaDon,
			th.HeSo,
			-- huy PhieuThu --> PTThucThu,HoaHongThucThu = 0
			case when TinhChietKhauTheo =1 then 
				case when ISNULL(ThucThu,0)= 0 then 0 else TienChietKhau end
			end as HoaHongThucThu,		
			case when TinhChietKhauTheo =1 then 
				case when ISNULL(ThucThu,0)= 0 then 0 else PT_ChietKhau end
			end as PTThucThu,			
			case when TinhChietKhauTheo =2 then TienChietKhau end as HoaHongDoanhThu,
			case when TinhChietKhauTheo =3 then TienChietKhau end as HoaHongVND,
			case when TinhChietKhauTheo =2 then PT_ChietKhau end as PTDoanhThu
			
		from BH_NhanVienThucHien th		
		join NS_NhanVien nv on th.ID_NhanVien= nv.ID
		join BH_HoaDon hd on th.ID_HoaDon= hd.ID
		left join (
					select qct.ID_HoaDonLienQuan, sum(ISNULL(qct.TienThu,0)) as ThucThu
					from Quy_HoaDon qhd
					join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
					where (qhd.TrangThai is null or qhd.TrangThai = '1')
					and ISNULL(qct.DiemThanhToan,0) = 0 and  ISNULL(qct.ThuTuThe,0) = 0
					group by qct.ID_HoaDonLienQuan
				) tblQuy on hd.ID= tblQuy.ID_HoaDonLienQuan
		where th.ID_HoaDon is not null
		and hd.ChoThanhToan='0'
		and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
		and CONVERT(varchar, hd.NgayLapHoaDon,23) >= @DateFrom 
		and CONVERT(varchar, hd.NgayLapHoaDon,23) <= @DateTo
	) tbl	order by NgayLapHoaDon desc");

            Sql(@"ALTER PROCEDURE [dbo].[GetListDM_LoHangHetHan]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    SELECT * FROM
    (
    SELECT
    dmlo1.ID as ID_LoHang,@ID_ChiNhanh AS ID_DonVi,dmlo1.MaLoHang,dvqd.MaHangHoa, dmlo1.NgaySanXuat, dmlo1.NgayHetHan,
    ROUND(SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)),2) AS TonKho
    FROM
    (
    		SELECT 
    		bhdct.ID_LoHang,
    		NULL AS SoLuongNhap,
    		SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    		FROM DM_LoHang dmlo
    		Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false' and hh.LaHangHoa =1
    		AND bhd.NgayLapHoaDon < @timeEnd
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		GROUP BY bhdct.ID_LoHang                                                                                                                                                                                                                                                      
    
    		UNION ALL
    		SELECT 
    		bhdct.ID_LoHang,
    		NULL AS SoLuongNhap,
    		SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
    		FROM DM_LoHang dmlo
    		Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    		OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		AND bhd.NgayLapHoaDon < @timeEnd
    		GROUP BY bhdct.ID_LoHang
    
    		UNION ALL
    		SELECT 
    		bhdct.ID_LoHang,
    		SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    		null AS SoLuongXuat
    		FROM DM_LoHang dmlo
    		Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0 and hh.LaHangHoa = 1
    		AND bhd.ID_DonVi = @ID_ChiNhanh
    		AND bhd.NgayLapHoaDon < @timeEnd
    		GROUP BY bhdct.ID_LoHang
    
    		UNION ALL
    		SELECT 
    		bhdct.ID_LoHang,
    		SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    		null AS SoLuongXuat
    		FROM DM_LoHang dmlo
    		Left join BH_HoaDon_ChiTiet bhdct on dmlo.ID = bhdct.ID_LoHang
    		LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    		LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    		WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    		AND bhd.NgaySua < @timeEnd
    		GROUP BY bhdct.ID_LoHang
    ) AS td 
    	right join DM_LoHang dmlo1 on td.ID_LoHang = dmlo1.ID
    	left join DonViQuiDoi dvqd on dmlo1.ID_HangHoa = dvqd.ID_HangHoa
    	where (DATEPART(day, dmlo1.NgayHetHan)= DATEPART(day, @timeEnd)) and 
    (DATEPART(month, dmlo1.NgayHetHan)= DATEPART(month, @timeEnd)) and (DATEPART(year, dmlo1.NgayHetHan)= DATEPART(YEAR, @timeEnd)) and dvqd.Xoa is null and dvqd.LaDonViChuan = 1
    GROUP BY dmlo1.ID,dmlo1.MaLoHang, dmlo1.NgaySanXuat, dmlo1.NgayHetHan, dvqd.MaHangHoa
    	) as tbDMLo
    	where tbDMLo.TonKho > 0
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_BieuDoThucThuToDay]");
            DropStoredProcedure("[dbo].[BaoCaoTongQuan_BieuDoThucThuToHour]");
            DropStoredProcedure("[dbo].[SP_GetHT_CauHinh_TichDiem]");
            DropStoredProcedure("[dbo].[SP_GetNhanVien_NguoiDung]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountProduct_Detail]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountProduct_General]");
            DropStoredProcedure("[dbo].[SP_ValueCard_GetListHisUsed]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountInvoice]");
            DropStoredProcedure("[dbo].[SP_ReportDiscountInvoice_Detail]");
        }
    }
}
