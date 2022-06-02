namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_NS_CongViec_20181031 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SelectDanhSachNhanVien]", parametersAction: p => new
            {
                donviID = p.String(),
                phongban = p.String(),
                dantoc = p.String(),
                HK_TT = p.String(),
                HK_QH = p.String(),
                HK_XP = p.String(),
                TT_TT = p.String(),
                TT_QH = p.String(),
                TT_XP = p.String(),
                GioiTinh = p.Int(),
                TrangThai = p.Int(),
                ChinhTri = p.Int(),
                Start = p.String(),
                End = p.String(),
                LoaiHopDong = p.String(),
                ListBaoHiem = p.String(),
                text = p.String()
            }, body: @"DECLARE @donvi_id uniqueidentifier 
	DECLARE @hktt_id uniqueidentifier 
	DECLARE @hkqh_id uniqueidentifier 
	DECLARE @hkxp_id uniqueidentifier 
	DECLARE @tttt_id uniqueidentifier 
	DECLARE @ttqh_id uniqueidentifier 
	DECLARE @ttxp_id uniqueidentifier 
	DECLARE @tungay date 
	DECLARE @denngay date 

	DECLARE @TablePhongBan TABLE( Name [nvarchar](max))
	INSERT INTO @TablePhongBan(Name) select  Name from [dbo].[splitstring](@phongban+',') where Name!='';

	DECLARE @TableDanToc TABLE( Name [nvarchar](max))
	DECLARE @countdantoc int
	INSERT INTO @TableDanToc(Name) select  Name from [dbo].[splitstring](@dantoc+',') where Name!='';
     Select @countdantoc =  (Select count(*) from @TableDanToc);

	 DECLARE @TableBaoHiem TABLE( Name int)
	DECLARE @countbaohiem int
	INSERT INTO @TableBaoHiem(Name) select  Name from [dbo].[splitstring](@ListBaoHiem+',') where Name!='';
     Select @countbaohiem =  (Select count(*) from @TableBaoHiem);

	 DECLARE @TableHopDong TABLE( Name int)
	DECLARE @counthopdong int
	INSERT INTO @TableHopDong(Name) select  Name from [dbo].[splitstring](@LoaiHopDong+',') where Name!='';
     Select @counthopdong =  (Select count(*) from @TableHopDong);

	 IF(LEN(ISNULL(@tungay, '')) >0)
	 set @tungay=CONVERT(datetime, @Start, 103)
	 set @denngay=CONVERT(datetime, @End, 103)

	IF(LEN(ISNULL(@donviId, '')) >0)
		SET @donvi_id = CONVERT(uniqueidentifier, @donviId)

	IF(LEN(ISNULL(@HK_TT, ''))>0)
		SET @hktt_id = CONVERT(uniqueidentifier, @HK_TT)

	IF(LEN(ISNULL(@HK_QH, ''))>0)
		SET @hkqh_id = CONVERT(uniqueidentifier, @HK_QH)

	IF(LEN(ISNULL(@HK_XP, ''))>0)
		SET @hkxp_id = CONVERT(uniqueidentifier, @HK_XP)

	IF(LEN(ISNULL(@TT_TT, ''))>0)
		SET @tttt_id = CONVERT(uniqueidentifier, @TT_TT)

	IF(LEN(ISNULL(@TT_QH, ''))>0)
		SET @ttqh_id = CONVERT(uniqueidentifier, @TT_QH)

	IF(LEN(ISNULL(@TT_XP, ''))>0)
		SET @ttxp_id = CONVERT(uniqueidentifier, @TT_XP)

    SET NOCOUNT ON;  
   select 
    f1.MaNhanVien,
			f1.TenNhanVien,
			CASE 
				WHEN f1.NgaySinh is not null
				THEN convert(varchar, f1.NgaySinh, 103)
				ELSE '' 
			 END as NgaySinh,
			 CASE 
				WHEN f1.GioiTinh =1
				THEN 'Nam'
				ELSE 'Nữ' 
			 END as GioiTinh,
			f1.NoiSinh,
			f1.DienThoaiDiDong,
			f1.Email,
			f1.SoCMND,
			CASE 
				WHEN f1.NgayCap is not null
				THEN convert(varchar, f1.NgayCap, 103)
				ELSE '' 
			 END as NgayCap,
			f1.NoiCap,
			f1.DanTocTonGiao,
			f1.TonGiao,
			f1.TinhTrangHonNhan,
			qg.TenQuocGia,
			f1.DiaChiHKTT,
			xp1.TenXaPhuong as HK_TenXaPhuong, 
			qh1.TenQuanHuyen as HK_TenQuanHuyen,
			tt1.TenTinhThanh as HK_TenTinhThanh,
			f1.DiaChiTT,
			xp2.TenXaPhuong as TT_TenXaPhuong,
			qh2.TenQuanHuyen as TT_TenQuanHuyen,
			tt2.TenTinhThanh as TT_TenTinhThanh,
			fpb.TenPhongBan,
			CASE 
				WHEN f1.NgayVaoLamViec is not null
				THEN convert(varchar, f1.NgayVaoLamViec, 103)
				ELSE '' 
			 END as NgayVaoLamViec,
			 f1.DaNghiViec,
			CASE 
				WHEN f1.NgayVaoDoan is not null
				THEN convert(varchar, f1.NgayVaoDoan, 103)
				ELSE '' 
			 END as NgayVaoDoan,
			f1.NoiVaoDoan,
			CASE 
				WHEN f1.NgayNhapNgu is not null
				THEN convert(varchar, f1.NgayNhapNgu, 103)
				ELSE '' 
			 END as NgayNhapNgu,
			CASE 
				WHEN f1.NgayXuatNgu is not null
				THEN convert(varchar, f1.NgayXuatNgu, 103)
				ELSE '' 
			 END as NgayXuatNgu,
			CASE 
				WHEN f1.NgayVaoDang is not null
				THEN convert(varchar, f1.NgayVaoDang, 103)
				ELSE '' 
			 END as NgayVaoDang,
			 CASE 
				WHEN f1.NgayVaoDangChinhThuc is not null
				THEN convert(varchar, f1.NgayVaoDangChinhThuc, 103)
				ELSE '' 
			 END as NgayVaoDangChinhThuc,
			  CASE 
				WHEN f1.NgayRoiDang is not null
				THEN convert(varchar, f1.NgayRoiDang, 103)
				ELSE '' 
			 END as NgayRoiDang,
			f1.NoiSinhHoatDang,
			f1.GhiChuThongTinChinhTri
   
  from (
   SELECT *
    FROM NS_NhanVien nv
	where (nv.TrangThai not in(0) or nv.TrangThai is null)
							and (nv.ID  in (select ct.ID_NhanVien from NS_QuaTrinhCongTac ct where ct.ID_DonVi=@donvi_id)
								or nv.ID not in (select ct.ID_NhanVien from NS_QuaTrinhCongTac ct ))
							and (LEN(ISNULL(@phongban, '')) = 0 or nv.ID_NSPhongBan in (select pb.Name from @TablePhongBan pb))
							and (LEN(ISNULL(@HK_TT, '')) = 0 or nv.ID_TinhThanhHKTT =@hktt_id)
							and (LEN(ISNULL(@HK_QH, '')) = 0 or nv.ID_QuanHuyenHKTT =@hkqh_id)  
							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_XaPhuongHKTT =@hkxp_id)
							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_TinhThanhHKTT =@tttt_id)
							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_QuanHuyenTT =@ttqh_id)
							and (LEN(ISNULL(@HK_XP, '')) = 0 or nv.ID_XaPhuongTT =@ttxp_id)
							and (LEN(ISNULL(@text, '')) = 0 or nv.MaNhanVien like N'%'+@text+'%' or nv.TenNhanVien like N'%'+@text+'%' or nv.TenNhanVienChuCaiDau like N'%'+@text+'%' or nv.TenNhanVienKhongDau like N'%'+@text+'%')
							and(@countdantoc=0 or (select count(*) from @TableDanToc dt where dt.Name like N'%'+nv.DanTocTonGiao+'%')>0)
							and (@GioiTinh=2 or @GioiTinh=nv.GioiTinh)
							and (@TrangThai=2 or @TrangThai=nv.DaNghiViec)
							and (LEN(ISNULL(@Start, ''))=0 or(nv.NgaySinh>=@tungay and nv.NgaySinh<=@denngay))
							and(@counthopdong=0 or nv.id in (select hd.ID_NhanVien from NS_HopDong hd where hd.LoaiHopDong in (select Name from @TableHopDong)))  
							and(@countbaohiem=0 or nv.id in (select bh.ID_NhanVien from NS_BaoHiem bh where bh.LoaiBaoHiem in (select Name from @TableBaoHiem)))  
							and ((@ChinhTri=0 and nv.NgayVaoDoan is not null) 
								or (@ChinhTri=1 and nv.NgayVaoDang is not null and  nv.NgayVaoDang is not null and nv.NgayRoiDang is null)
								or (@ChinhTri =2 and nv.NgayNhapNgu is not null)
								or @ChinhTri=-1)) f1
			left join DM_QuocGia qg ON qg.ID = f1.ID_QuocGia
			left join DM_TinhThanh  tt1 ON tt1.ID = f1.ID_TinhThanhHKTT  
			left join DM_TinhThanh  tt2 ON tt2.ID = f1.ID_TinhThanhTT  
			left join DM_QuanHuyen  qh1 ON qh1.ID = f1.ID_QuanHuyenHKTT
			left join DM_QuanHuyen  qh2 ON qh2.ID = f1.ID_QuanHuyenTT
			left join DM_XaPhuong  xp1 ON xp1.ID = f1.ID_XaPhuongHKTT
			left join DM_XaPhuong  xp2 ON xp2.ID = f1.ID_XaPhuongTT 
			left join NS_PhongBan  fpb ON fpb.ID = f1.ID_NSPhongBan  
			;  ");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDichVu_NhapXuatTon]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String()
            }, body: @"SELECT
	 a.MaHangHoa as MaHangHoa,
			MAX(a.TenHangHoaFull) as TenHangHoaFull,
    		MAX(a.TenHangHoa) as TenHangHoa,
    		MAX(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(a.TenDonViTinh) as TenDonViTinh,
    		a.TenLoHang as TenLoHang,
   -- 		CAST(ROUND((SUM(a.SoLuongBanDK)), 2) as float) as SoLuongBanDK, 
			--CAST(ROUND((SUM(a.GiaTriBanDK)), 0) as float) as GiaTriBanDK, 
			--	CAST(ROUND((SUM(a.SoLuongSuDungDK)), 2) as float) as SoLuongSuDungDK, 
			--CAST(ROUND((SUM(a.GiaTriSuDungDK)), 0) as float) as GiaTriSuDungDK,
					CAST(ROUND((SUM(a.SoLuongBanDK - a.SoLuongSuDungDK)), 2) as float) as SoLuongConLaiDK,
				CAST(ROUND((SUM(a.GiaTriBanDK - a.GiaTriSuDungDK)), 0) as float) as GiaTriConLaiDK,

				CAST(ROUND((SUM(a.SoLuongBanGK)), 2) as float) as SoLuongBanGK, 
			CAST(ROUND((SUM(a.GiaTriBanGK)), 0) as float) as GiaTriBanGK, 
				CAST(ROUND((SUM(a.SoLuongSuDungGK)), 2) as float) as SoLuongSuDungGK, 
			CAST(ROUND((SUM(a.GiaTriSuDungGK)), 0) as float) as GiaTriSuDungGK,
					CAST(ROUND((SUM(a.SoLuongBanGK - a.SoLuongSuDungGK)), 2) as float) as SoLuongConLaiGK,
				CAST(ROUND((SUM(a.GiaTriBanGK - a.GiaTriSuDungGK)), 0) as float) as GiaTriConLaiGK,

					CAST(ROUND((SUM(a.SoLuongBanDK - a.SoLuongSuDungDK + a.SoLuongBanGK - a.SoLuongSuDungGK)), 2) as float) as SoLuongConLaiCK,
				CAST(ROUND((SUM(a.GiaTriBanDK - a.GiaTriSuDungDK + a.GiaTriBanGK - a.GiaTriSuDungGK)), 0) as float) as GiaTriConLaiCK
	FROM
	(
    Select
    	dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    	Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    	Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
		dt.MaDoiTuong as MaKhachHang,
		Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
		Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
		Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
		dt.DienThoai,
    	ISNULL(td.SoLuongBanDK, 0) as SoLuongBanDK,
		ISNULL(td.SoLuongSuDungDK, 0) as SoLuongSuDungDK,
		ISNULL(td.GiaTriBanDK, 0) as GiaTriBanDK,
		Case when td.SoLuongBanDK is null or td.SoLuongBanDK = 0 then 0 else ISNULL(td.GiaTriBanDK, 0) * (ISNULL(td.SoLuongSuDungDK, 0) / ISNULL(td.SoLuongBanDK, 0)) end as GiaTriSuDungDK,
		ISNULL(td.SoLuongBanGK, 0) as SoLuongBanGK,
		ISNULL(td.SoLuongSuDungGK, 0) as SoLuongSuDungGK,
		ISNULL(td.GiaTriBanGK, 0) as GiaTriBanGK,
		Case when td.SoLuongBanGK is null or td.SoLuongBanGK = 0 then 0 else ISNULL(td.GiaTriBanGK, 0) * (ISNULL(td.SoLuongSuDungGK, 0) / ISNULL(td.SoLuongBanGK, 0)) end as GiaTriSuDungGK,

    	Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    	Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
	FROM
	(
	 -- Đầu kỳ
		Select 
		hd.ID_DoiTuong,
		hd.ID_DonVi,
		hdct.ID_DonViQuiDoi,
		hdct.ID_LoHang,
    	ISNULL(hdct.SoLuong, 0) as SoLuongBanDK,
		ISNULL(hdct.ThanhTien, 0) as GiaTriBanDK,
		ISNULL(hdsd.SoLuong, 0) as SoLuongSuDungDK,
		null as SoLuongBanGK,
		null as GiaTriBanGK,
		null as SoLuongSuDungGK
    	FROM BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
		left join BH_HoaDon_ChiTiet hdsd on hdct.ID = hdsd.ID_ChiTietGoiDV
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	where (hd.NgayLapHoaDon < @timeStart) 
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	and hd.ChoThanhToan = 0 
    	and (hd.LoaiHoaDon = 19)
		and hdct.ID_ChiTietGoiDV is null
		-- Giữa kỳ
		Union all
	
		Select
		hd.ID_DoiTuong, 
		hd.ID_DonVi,
		hdct.ID_DonViQuiDoi,
		hdct.ID_LoHang,
		null as SoLuongBanDK,
		null as GiaTriBanDK,
		null as SoLuongSuDungDK,
    	ISNULL(hdct.SoLuong, 0) as SoLuongBanGK,
		ISNULL(hdct.ThanhTien, 0) as GiaTriBanGK,
		ISNULL(hdsd.SoLuong, 0) as SoLuongSuDungGK
    	FROM BH_HoaDon hd
    	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
		left join BH_HoaDon_ChiTiet hdsd on hdct.ID = hdsd.ID_ChiTietGoiDV
    	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    	where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd)
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    	and hd.ChoThanhToan = 0 
    	and (hd.LoaiHoaDon = 19)
		and hdct.ID_ChiTietGoiDV is null
	)as td
	inner join DonViQuiDoi dvqd on td.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on td.ID_DoiTuong = dt.ID
    		left join DM_LoHang lh on td.ID_LoHang = lh.ID
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
    		where td.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    		and (dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
		) as a
		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
		and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    	and a.Xoa like @TrangThai
		Group by a.MaHangHoa, TenLoHang
		ORDER BY SUM(a.GiaTriBanDK - a.GiaTriSuDungDK + a.GiaTriBanGK - a.GiaTriSuDungGK) DESC");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDichVu_NhatKySuDungChiTiet]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String()
            }, body: @"SELECT 
			a.MaKhachHang,
			a.TenKhachHang,
			a.MaHoaDon,
    		a.NgayLapHoaDon,
    		a.MaHangHoa,
    		a.TenHangHoaFull,
    		a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    		a.TenDonViTinh,
    		a.TenLoHang,
    		CAST(ROUND((a.SoLuong), 2) as float) as SoLuong,
    		a.NhanVienChietKhau
    	FROM
    	(
    		Select gdv.MaHoaDon,
    		hd.NgayLapHoaDon,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
					dt.MaDoiTuong as MaKhachHang,
					Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
					Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
					Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
					dt.DienThoai,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    		nv.NhanVienChietKhau,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietGoiDV is not null)
			left join BH_HoaDon_ChiTiet hdctdv on hdctdv.ID = hdct.ID_ChiTietGoiDV
			left join BH_HoaDon gdv on hdctdv.ID_HoaDon = gdv.ID
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join (Select Main.ID_ChiTietHoaDon,
						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As NhanVienChietKhau
						From
						(
						Select distinct hh_tt.ID_ChiTietHoaDon,
						(
							Select tt.TenNhanVien + ' - ' AS [text()]
							From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) tt
							Where tt.ID_ChiTietHoaDon = hh_tt.ID_ChiTietHoaDon
							For XML PATH ('')
						) hanghoa_thuoctinh
						From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) hh_tt
						) Main) as nv on hdct.ID = nv.ID_ChiTietHoaDon
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and (hd.LoaiHoaDon = 1)
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (gdv.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    		and a.Xoa like @TrangThai
    	order by a.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDichVu_NhatKySuDungTongHop]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String()
            }, body: @"SELECT 
			a.MaKhachHang as MaKhachHang,
			MAX(a.TenKhachHang) as TenKhachHang,
			a.MaHangHoa as MaHangHoa,
			MAX(a.TenHangHoaFull) as TenHangHoaFull,
    		MAX(a.TenHangHoa) as TenHangHoa,
    		MAX(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(a.TenDonViTinh) as TenDonViTinh,
    		a.TenLoHang as TenLoHang,
			CAST(ROUND((SUM(a.SoLuongMua)), 2) as float) as SoLuongMua, 
    		CAST(ROUND((SUM(a.SoLuong)), 2) as float) as SoLuongSuDung,			
			CAST(ROUND((SUM(a.SoLuongMua - a.SoLuong)), 2) as float) as SoLuongConLai
    	FROM
    	(
    		Select 
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
					dt.MaDoiTuong as MaKhachHang,
					Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
					Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
					Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
					dt.DienThoai,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
			ISNULL(hddv.SoLuong, 0) as SoLuongMua,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietGoiDV is not null)
			left join BH_HoaDon_ChiTiet hddv on hdct.ID_ChiTietGoiDV = hddv.ID
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and (hd.LoaiHoaDon = 1)
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    		and a.Xoa like @TrangThai
			Group by a.MaHangHoa, a.MaKhachHang, a.TenLoHang
    	order by SUM(a.SoLuongMua - a.SoLuong) desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDichVu_SoDuChiTiet]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String()
            }, body: @"SELECT 
			a.MaHoaDon,
    		a.NgayLapHoaDon,
			a.MaKhachHang,
			a.TenKhachHang,
    		a.MaHangHoa,
    		a.TenHangHoaFull,
    		a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    		a.TenDonViTinh,
    		a.TenLoHang,
    		CAST(ROUND((a.SoLuong), 2) as float) as SoLuong, 
    		CAST(ROUND((a.GiaBan), 0) as float) as DonGia,
    			CAST(ROUND((a.TienChietKhau), 0) as float) as TienChietKhau,
    			CAST(ROUND((a.GiamGiaHD), 0) as float) as GiamGiaHD,
				CAST(ROUND((a.ThanhTien), 0) as float) as ThanhTien,
				CAST(ROUND((a.SoLuongSuDung), 2) as float) as SoLuongSuDung, 
					CAST(ROUND((a.SoLuong - a.SoLuongSuDung), 2) as float) as SoLuongConLai, 
    		a.NhanVienChietKhau
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
					dt.MaDoiTuong as MaKhachHang,
					Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
					Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
					Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
					dt.DienThoai,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    		ISNULL(hdct.DonGia, 0) as GiaBan,
    		ISNULL(hdct.TienChietKhau, 0) as TienChietKhau,
			ISNULL(hdsd.SoLuong, 0) as SoLuongSuDung,
    		nv.NhanVienChietKhau,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			left join BH_HoaDon_ChiTiet hdsd on hdct.ID = hdsd.ID_ChiTietGoiDV
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join (Select Main.ID_ChiTietHoaDon,
						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As NhanVienChietKhau
						From
						(
						Select distinct hh_tt.ID_ChiTietHoaDon,
						(
							Select tt.TenNhanVien + ' - ' AS [text()]
							From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) tt
							Where tt.ID_ChiTietHoaDon = hh_tt.ID_ChiTietHoaDon
							For XML PATH ('')
						) hanghoa_thuoctinh
						From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) hh_tt
						) Main) as nv on hdct.ID = nv.ID_ChiTietHoaDon
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and (hd.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (hd.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    		and a.Xoa like @TrangThai
    	order by a.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDichVu_SoDuTongHop]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String()
            }, body: @"SELECT 
			a.MaHoaDon,
    		MAX(a.NgayLapHoaDon) as NgayLapHoaDon,
			MAX(a.MaKhachHang) as MaKhachHang,
			MAX(a.TenKhachHang) as TenKhachHang,
    		CAST(ROUND((SUM(a.SoLuong)), 2) as float) as SoLuong, 
				CAST(ROUND((SUM(a.ThanhTien)), 0) as float) as ThanhTien,
				CAST(ROUND((SUM(a.SoLuongSuDung)), 2) as float) as SoLuongSuDung, 
					CAST(ROUND((SUM(a.SoLuong - a.SoLuongSuDung)), 2) as float) as SoLuongConLai, 
    		MAX(a.NgayApDungGoiDV) as NgayApDungGoiDV,
			MAX(a.HanSuDungGoiDV) as HanSuDungGoiDV,
			Case when DATEADD(day,-1,GETDATE()) <= MAX(a.HanSuDungGoiDV) then DATEDIFF(day,DATEADD(day,-1,GETDATE()),MAX(a.HanSuDungGoiDV)) else 0 end as SoNgayConHan,
			Case when DATEADD(day,-1,GETDATE()) > MAX(a.HanSuDungGoiDV) then DATEDIFF(day,DATEADD(day,-1,GETDATE()) ,MAX(a.HanSuDungGoiDV)) * (-1) else 0 end as SoNgayHetHan
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
					dt.MaDoiTuong as MaKhachHang,
					Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
					Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
					Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
					dt.DienThoai,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
    		ISNULL(hdct.DonGia, 0) as GiaBan,
    		ISNULL(hdct.TienChietKhau, 0) as TienChietKhau,
			ISNULL(hdsd.SoLuong, 0) as SoLuongSuDung,
			hd.NgayApDungGoiDV,
			hd.HanSuDungGoiDV,
			--DATEDIFF(day,GETDATE() ,MAX(hd.HanSuDungGoiDV)) as ChenhLenhNgay,
    		nv.NhanVienChietKhau,
    			Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    		hdct.ThanhTien,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) end as GiamGiaHD,
    				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			left join BH_HoaDon_ChiTiet hdsd on hdct.ID = hdsd.ID_ChiTietGoiDV
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join (Select Main.ID_ChiTietHoaDon,
						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As NhanVienChietKhau
						From
						(
						Select distinct hh_tt.ID_ChiTietHoaDon,
						(
							Select tt.TenNhanVien + ' - ' AS [text()]
							From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) tt
							Where tt.ID_ChiTietHoaDon = hh_tt.ID_ChiTietHoaDon
							For XML PATH ('')
						) hanghoa_thuoctinh
						From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) hh_tt
						) Main) as nv on hdct.ID = nv.ID_ChiTietHoaDon
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and (hd.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (hd.MaHoaDon like @Text_Search or dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    		and a.Xoa like @TrangThai
			Group by a.MaHoaDon
    	order by MAX(a.NgayLapHoaDon) desc");

            CreateStoredProcedure(name: "[dbo].[BaoCaoDichVu_TonChuaSuDung]", parametersAction: p => new
            {
                Text_Search = p.String(),
                MaHH = p.String(),
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.String(),
                ID_NhomHang_SP = p.String()
            }, body: @"SELECT 
			a.MaHangHoa as MaHangHoa,
			MAX(a.TenHangHoaFull) as TenHangHoaFull,
    		MAX(a.TenHangHoa) as TenHangHoa,
    		MAX(a.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		MAX(a.TenDonViTinh) as TenDonViTinh,
    		a.TenLoHang as TenLoHang,
    		CAST(ROUND((SUM(a.SoLuong)), 2) as float) as SoLuongBan, 
			CAST(ROUND((SUM(a.GiaTriBan)), 0) as float) as GiaTriBan, 
				CAST(ROUND((SUM(a.SoLuongSuDung)), 2) as float) as SoLuongSuDung, 
				CAST(ROUND((SUM(a.GiaTriBan * (a.SoLuongSuDung / a.SoLuong))), 0) as float) as GiaTriSuDung, 
					CAST(ROUND((SUM(a.SoLuong - a.SoLuongSuDung)), 2) as float) as SoLuongConLai,
				CAST(ROUND((SUM(a.GiaTriBan * (1 - (a.SoLuongSuDung / a.SoLuong)))), 0) as float) as GiaTriConLai
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenHangHoaFull,
    			hh.TenHangHoa,
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				Case when lh.MaLoHang is null then '' else '. Lô: ' + lh.MaLoHang end as TenLoHang,
					dt.MaDoiTuong as MaKhachHang,
					Case when dt.ID is null then N'Khách lẻ' else dt.TenDoiTuong end as TenKhachHang,
					Case when dt.ID is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
					Case when dt.ID is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_KyTuDau,
					dt.DienThoai,
    		ISNULL(hdct.SoLuong, 0) as SoLuong,
			ISNULL(hdct.ThanhTien, 0) as GiaTriBan,
			ISNULL(hdsd.SoLuong, 0) as SoLuongSuDung,
			--ISNULL(hdsd.ThanhTien, 0) as GiaTriSuDung,
    		Case When hh.ID_NhomHang is null then '00000000-0000-0000-0000-000000000000' else hh.ID_NhomHang end as ID_NhomHang,
    	
    		Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			left join BH_HoaDon_ChiTiet hdsd on hdct.ID = hdsd.ID_ChiTietGoiDV
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
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
    		where (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    			and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and (hd.LoaiHoaDon = 19)
			and hdct.ID_ChiTietGoiDV is null
    		and hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    		and (dvqd.MaHangHoa like @Text_Search or dvqd.MaHangHoa like @MaHH or hh.TenHangHoa_KhongDau like @MaHH or hh.TenHangHoa_KyTuDau like @MaHH)
    	) a
    		where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			and (a.TenKhachHang like @MaKH or a.TenKhachHang_KhongDau like @MaKH or TenKhachHang_KyTuDau like @MaKH or a.DienThoai like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV)
    		and a.Xoa like @TrangThai
			Group by a.MaHangHoa, TenLoHang
    	order by SUM(a.GiaTriBan * (1 - (a.SoLuongSuDung / a.SoLuong))) desc");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SelectDanhSachNhanVien]");
            DropStoredProcedure("[dbo].[BaoCaoDichVu_NhapXuatTon]");
            DropStoredProcedure("[dbo].[BaoCaoDichVu_NhatKySuDungChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoDichVu_NhatKySuDungTongHop]");
            DropStoredProcedure("[dbo].[BaoCaoDichVu_SoDuChiTiet]");
            DropStoredProcedure("[dbo].[BaoCaoDichVu_SoDuTongHop]");
            DropStoredProcedure("[dbo].[BaoCaoDichVu_TonChuaSuDung]");
        }
    }
}