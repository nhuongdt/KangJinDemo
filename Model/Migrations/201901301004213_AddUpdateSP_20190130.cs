namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190130 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SP_GetInforKhachHang_ByID]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_ChiNhanh = p.String(),
                timeStart = p.String(),
                timeEnd = p.String()
            }, body: @"SELECT 
    		c.ID,
    		max(c.MaDoiTuong) as MaDoiTuong,
    		max(c.ID_NhomDoiTuong) as ID_NhomDoiTuong,
    		max(c.TenDoiTuong) as TenDoiTuong,
    		c.GioiTinhNam,
    		max(c.NgaySinh_NgayTLap) as NgaySinh_NgayTLap,
    		max(c.DinhDang_NgaySinh) as DinhDang_NgaySinh,
    		max(c.ID_NhanVienPhuTrach) as ID_NhanVienPhuTrach,
    		max(c.ID_NguoiGioiThieu) as ID_NguoiGioiThieu,
    		c.LaCaNhan as LaCaNhan,
    		max(c.TongTichDiem) as TongTichDiem,
    		max(c.ID_TinhThanh) as ID_TinhThanh,
    		max(c.ID_QuanHuyen) as ID_QuanHuyen,
    		max(c.NoHienTai) as NoHienTai,
    		max(c.TongBan) as TongBan,
    		max(c.TongBanTruTraHang) as TongBanTruTraHang,
    		max(c.TongMua) as TongMua,
    		max(c.SoLanMuaHang) as SoLanMuaHang
    	FROM
    	(
    SELECT * 
    		FROM
    		(
    		  SELECT 
    		  dt.ID as ID,
    		  dt.MaDoiTuong, 
    		  Case when DoiTuong_Nhom.ID_NhomDoiTuong is not null then DoiTuong_Nhom.ID_NhomDoiTuong else N'00000000-0000-0000-0000-000000000000' end as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
    		  dt.DinhDang_NgaySinh,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    		  dt.LaCaNhan,
    		  ISNULL(dt.TongTichDiem,0) as TongTichDiem,
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang
    		  FROM DM_DoiTuong dt
    			
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    			LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    			LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    			LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    			Left join 
					(Select Main.ID as ID_DoiTuong,
    							--Left(Main.dt_n,Len(Main.dt_n)-1) As TenNhomDT,
    							Left(Main.id_n,Len(Main.id_n)-1) As ID_NhomDoiTuong
    							From
    							(
    							Select distinct hh_tt.ID,
    							(
    								Select ndt.TenNhomDoiTuong + ', ' AS [text()]
    								From dbo.DM_DoiTuong_Nhom dtn
    								inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    								Where dtn.ID_DoiTuong = hh_tt.ID
    								order by ndt.TenNhomDoiTuong
    								For XML PATH ('')
    							) dt_n,
    							(
    							Select convert(nvarchar(max), ndt.ID)  + ', ' AS [text()]
    							From dbo.DM_DoiTuong_Nhom dtn
    							inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    							Where dtn.ID_DoiTuong = hh_tt.ID
    							order by ndt.TenNhomDoiTuong
    							For XML PATH ('')
    							) id_n
    							From dbo.DM_DoiTuong hh_tt
    							) Main) as DoiTuong_Nhom on dt.ID = DoiTuong_Nhom.ID_DoiTuong
    			LEFT Join
    			(
    			  SELECT HangHoa.ID_KhachHang,
    				SUM(ISNULL(HangHoa.NoHienTai, 0)) as NoHienTai, 
    				SUM(ISNULL(HangHoa.TongBan, 0)) as TongBan,
    				SUM(ISNULL(HangHoa.TongBanTruTraHang, 0)) as TongBanTruTraHang,
    				SUM(ISNULL(HangHoa.TongMua, 0)) as TongMua,
    					SUM(ISNULL(HangHoa.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
    					SELECT
    					td.ID_DoiTuong AS ID_KhachHang,
    					SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
    							NULL AS TongBan,
    							NULL AS TongBanTruTraHang,
    							NULL AS TongMua,
    						NULL AS SoLanMuaHang
    					FROM
    					(
    						SELECT 
    						bhd.ID_DoiTuong,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false'
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    							NULL AS CongNo,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- sổ quỹ
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    						NULL AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong
    
    						UNION ALL
    						SELECT 
    						qhdct.ID_DoiTuong AS ID_KhachHang,
    						NULL AS CongNo,
    						NULL AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong
    					) AS td
    						GROUP BY td.ID_DoiTuong
    						UNION ALL
    							-- Tổng bán phát sinh trong khoảng thời gian truy vấn
    						SELECT
    						pstv.ID_DoiTuong AS ID_KhachHang,
    						NULL AS NoHienTai,
    						SUM(ISNULL(pstv.DoanhThu,0)) AS TongBan,
    						SUM(ISNULL(pstv.DoanhThu,0)) -  SUM(ISNULL(pstv.GiaTriTra,0)) AS TongBanTruTraHang,
    						SUM(ISNULL(pstv.GiaTriTra,0)) AS TongMua,
    							NULL AS SoLanMuaHang
    						FROM
    						(
    						SELECT 
    						bhd.ID_DoiTuong,
    						NULL AS GiaTriTra,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi,
    							NULL AS SoLanMuaHang
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    						SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    						NULL AS DoanhThu,
    						NULL AS TienThu,
    						NULL AS TienChi, 
    							NULL AS SoLanMuaHang
    						FROM BH_HoaDon bhd
    						JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						) AS pstv
    						GROUP BY pstv.ID_DoiTuong
    			
    							Union All
    							Select 
    							hd.ID_DoiTuong AS ID_KhachHang,
    							NULL AS NoHienTai,
    						NULL AS TongBan,
    						NULL AS TongBanTruTraHang,
    						NULL AS TongMua,
    							COUNT(*) AS SoLanMuaHang
    							From BH_HoaDon hd 
    							where hd.LoaiHoaDon = 1
    							and hd.ChoThanhToan = 0
    							AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd 
    							And hd.ID_DonVi = @ID_ChiNhanh 
    							GROUP BY hd.ID_DoiTuong
    					)AS HangHoa
    						GROUP BY HangHoa.ID_KhachHang
    				) a
    					on dt.ID = a.ID_KhachHang

						WHERE dt.ID like @ID_DoiTuong
    				)b
    				) c GROUP BY c.ID,c.GioiTinhNam, c.LaCaNhan");

            CreateStoredProcedure(name: "[dbo].[SP_GetQuyHoaDon_byDoiTuong]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"select ct.ID_HoaDonLienQuan,
	case when hd.LoaiHoaDon = 6 then sum(ISNULL(ct.TienThu,0)) else 
	case when max(qhd.LoaiHoaDon) = 11 then sum(ct.TienThu) else sum(ISNULL(-ct.TienThu,0)) end end TongTienThu
	from Quy_HoaDon_ChiTiet ct
	join Quy_HoaDon qhd on ct.ID_HoaDon = qhd.ID
	left join BH_HoaDon hd on ct.ID_HoaDonLienQuan = hd.ID
	where ct.ID_DoiTuong like @ID_DoiTuong and qhd.ID_DonVi like @ID_DonVi
	and (TrangThai is not null or TrangThai = '1' ) 
	group by ct.ID_HoaDonLienQuan, hd.LoaiHoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_GetQuyHoaDonDatHang_byDoiTuong]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"select hd.ID as ID_HoaDonLienQuan , ISNULL(b.TongTienThu,0) as TongTienThu
	from BH_HoaDon hd 
	left join 
				(-- get TongThu from HĐatHang
				select sum(ct.TienThu) as TongTienThu, hdm.ID
				from Quy_HoaDon_ChiTiet ct
				join Quy_HoaDon qhd on ct.ID_HoaDon = qhd.ID
				join BH_HoaDon hdd on ct.ID_HoaDonLienQuan= hdd.ID
				join BH_HoaDon hdm on hdd.ID = hdm.ID_HoaDon
				where ct.ID_DoiTuong like @ID_DoiTuong and qhd.ID_DonVi like @ID_DonVi
				and (TrangThai is not null or TrangThai = '1' ) and hdd.LoaiHoaDon = 3 and hdm.LoaiHoaDon =1
				group by hdm.ID
				) b on b.ID = hd.ID
	where hd.NgayLapHoaDon = (
					-- chi lay HDDatHang thanh toan cho HD dau tien
					select min(NgayLapHoaDon)
					from 
					(
						select hdm.NgayLapHoaDon, ct.ID_HoaDonLienQuan
						from Quy_HoaDon_ChiTiet ct
						join Quy_HoaDon qhd on ct.ID_HoaDon = qhd.ID
						join BH_HoaDon hdd on ct.ID_HoaDonLienQuan= hdd.ID
						join BH_HoaDon hdm on hdd.ID = hdm.ID_HoaDon
						where ct.ID_DoiTuong like @ID_DoiTuong and qhd.ID_DonVi like @ID_DonVi
						and (TrangThai is not null or TrangThai = '1' )  and hdd.LoaiHoaDon=3 and hdm.LoaiHoaDon =1
						group by ct.ID_HoaDonLienQuan, hdm.ID,hdm.NgayLapHoaDon)
						b group by b.ID_HoaDonLienQuan
					)");

            CreateStoredProcedure(name: "[dbo].[SP_UpdateCusType_DMDoiTuong]", parametersAction: p => new
            {
                ID_DoiTuong = p.Guid(),
                ID_TrangThai = p.Guid()
            }, body: @"Update DM_DoiTuong set ID_TrangThai= @ID_TrangThai where ID  like @ID_DoiTuong");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetMaHoaDon_Max]
    @LoaiHoaDon [int]
AS
BEGIN
    DECLARE @MaHoaDonOffline varchar(5)='';
	DECLARE @Return float

    if @LoaiHoaDon = 1 
    	set @MaHoaDonOffline ='HDO'
    if @LoaiHoaDon = 3 
    	set @MaHoaDonOffline ='DHO'
    if @LoaiHoaDon = 6 
    	set @MaHoaDonOffline ='THO'
	 if @LoaiHoaDon = 19 
    	set @MaHoaDonOffline ='GDVO'
		
    	-- get list HoaDon not offline
    	SELECT @Return =  MAX(CAST(dbo.udf_GetNumeric(mahoadon)AS float))
    	FROM BH_HoaDon WHERE loaihoadon = @LoaiHoaDon and CHARINDEX(@MaHoaDonOffline,MaHoaDon)=0

		if	@Return is null 
			select Cast(0 as float) as MaxCode
		else 
			select @Return as MaxCode
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetMaQuyHoaDon_Max]
    @LoaiHoaDon [int]
AS
BEGIN
    DECLARE @MaHoaDon varchar(5);
	DECLARE @Return float

    if @LoaiHoaDon = 11 --Phieu Thu --
    	set @MaHoaDon ='SQPT'
    if @LoaiHoaDon = 12 --Phieu Chi --
    	set @MaHoaDon ='SQPC'	
    if @LoaiHoaDon = 15 --Dieu Chinh --
    	set @MaHoaDon ='CB'
    
    	SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaHoaDon) AS float))
    	FROM Quy_HoaDon WHERE CHARINDEX(@MaHoaDon,MaHoaDon) > 0 and CHARINDEX('Copy',MaHoaDon)= 0 and CHARINDEX('_',MaHoaDon)= 0

		if	@Return is null 
			select Cast(0 as float) as MaxCode
		else 
			select @Return as MaxCode
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GetInforKhachHang_ByID]");
            DropStoredProcedure("[dbo].[SP_GetQuyHoaDon_byDoiTuong]");
            DropStoredProcedure("[dbo].[SP_GetQuyHoaDonDatHang_byDoiTuong]");
            DropStoredProcedure("[dbo].[SP_UpdateCusType_DMDoiTuong]");
        }
    }
}
