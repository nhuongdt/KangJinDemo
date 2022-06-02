namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.SqlServer;

    public partial class AddUpdateSP_20200527 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NS_MayChamCong", "TrangThai", c => c.Int(nullable: false));
            AlterColumn("dbo.NS_MayChamCong", "MatMa", c => c.String());
            //AddColumn("dbo.DM_DonVi", "HanSuDung", c => c.DateTime());
            Sql(@"CREATE TYPE [dbo].[ChiTietHoaDonEdit] AS TABLE(
	[ID_HangHoa] [uniqueidentifier] NOT NULL,
	[ID_LoHang] [uniqueidentifier] NULL,
	[ID_DonViQuiDoi] [uniqueidentifier] NOT NULL,
	[TyLeChuyenDoi] [float] NOT NULL
)");

            Sql(@"CREATE FUNCTION [dbo].[Diary_GetInforOldInvoice]
(	
	@ID_HoaDon uniqueidentifier
)
RETURNS nvarchar(max)  
AS
begin 

	Declare @infor nvarchar(max) = (select CONCAT(noidung, REPLACE(REPLACE( noidungct,'&lt;','<'),'&gt;','>')) as abc
	from
		(SELECT 
			CONCAT(
				N'- Mã hóa đơn: ',hd.MaHoaDon, N' , Ngày lập hóa đơn: ', FORMAT(NgayLapHoaDon,'dd/MM/yyyy HH:mm:ss'), ',',
				N' Tổng tiền hàng: ', FORMAT(hd.TongTienHang,'#,0'), N', Tổng giảm giá: ', FORMAT(hd.TongGiamGia,'#,0'),',',
				N' Phải thanh toán: ', FORMAT(hd.PhaiThanhToan,'#,0'), N', Khách hàng: ', dt.TenDoiTuong, N' <br /> - Chi tiết hóa đơn gồm: ') as noidung,
				(select 
					concat(N' <br /> <a style= ""cursor: pointer"" onclick = ""loadHangHoabyMaHH(',qd.MaHangHoa,')"" > ', qd.MaHangHoa ,' </a> ',
					case when lh.MaLoHang is null then '' else N'(Số lô: ' + lh.MaLoHang + ') ' end,
					N' Số lượng: ', ct.SoLuong, N', Đơn giá: ', FORMAT(ct.DonGia, '#,0.0'),',',
					N' Chiết khấu: ', FORMAT(ct.TienChietKhau, '#,0'),',',
					N' Thành tiền:' , FORMAT(ct.ThanhTien, '#,0'))  AS[text()]
					from BH_HoaDon_ChiTiet ct
					join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
					left
					join DM_LoHang lh on ct.ID_LoHang = lh.ID
					where ct.ID_HoaDon = @ID_HoaDon
					for xml path('')
				) noidungct
		from BH_HoaDon hd
		join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		where hd.ID = @ID_HoaDon
	) s)
	return @infor
end
");

            Sql(@"CREATE Function [dbo].[GetDebitCustomer_allBrands]
(
@ID_DoiTuong uniqueidentifier,
@ToDate datetime
)
returns float
as
BEGIN
	declare @NoHienTai float =(
		SELECT     		  			
    		 CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai    		 		
    	FROM DM_DoiTuong dt  			  
    	LEFT JOIN (
    				SELECT 
						tblThuChi.ID_DoiTuong,
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) - SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    					SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    					SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    					SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
    						
    							-- tongban
    						SELECT 
    							bhd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							ISNULL(bhd.PhaiThanhToan,0) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,22) AND bhd.ChoThanhToan = '0'    
							AND bhd.NgayLapHoaDon < @ToDate
							AND bhd.ID_DoiTuong= @ID_DoiTuong
    
    						 union all
    							-- gia tri tr? t? bán hàng
    						SELECT bhd.ID_DoiTuong,
    							ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = '0'  
    							AND bhd.NgayLapHoaDon < @ToDate
								AND bhd.ID_DoiTuong= @ID_DoiTuong
    							
    							 union all
    
    							-- tienthu
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
							AND qhd.NgayLapHoaDon < @ToDate
							AND qhdct.ID_DoiTuong= @ID_DoiTuong
								
							union all
    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    								0 AS SoLanMuaHang
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    							AND qhd.NgayLapHoaDon < @ToDate 
								AND qhdct.ID_DoiTuong= @ID_DoiTuong
					)AS tblThuChi GROUP BY tblThuChi.ID_DoiTuong   						
    		) a on dt.ID = a.ID_DoiTuong  		
			where dt.ID= @ID_DoiTuong
	) 
	return @NoHienTai
END");

            CreateStoredProcedure(name: "[dbo].[SMS_KhachHangGiaoDich]", parametersAction: p => new
            {
                ID_ChiNhanh = p.String(),
                ID_NhomKhachHang = p.String(),
                FromDate = p.DateTime(),
                ToDate = p.DateTime(),
                Where = p.String(),
                CurrentPage = p.Int(),
                PageSize = p.Double()
            }, body: @"SET NOCOUNT ON;
	if @Where='' set @Where= ''
    else set @Where= ' AND '+ @Where 

	declare @from int= @CurrentPage * @PageSize + 1  
    declare @to int= (@CurrentPage + 1) * @PageSize 

	declare @sql1 nvarchar(max)= concat('
    	declare @tblIDNhoms table (ID varchar(36)) 
    	declare @idNhomDT nvarchar(max) = ''', @ID_NhomKhachHang, '''
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh select * from splitstring(''',@ID_ChiNhanh, ''')
    	
    	if @idNhomDT =''%%''
    		begin
				insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')

    			-- check QuanLyKHTheochiNhanh
    			--declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))  
				
				declare @QLTheoCN bit = 0;
				declare @countQL int=0;
				select distinct QuanLyKhachHangTheoDonVi into #temp from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)
				set @countQL = (select COUNT(*) from #temp)
				if	@countQL= 1 
						set @QLTheoCN = (select QuanLyKhachHangTheoDonVi from #temp)
				
    			if @QLTheoCN = 1
    				begin									
    					insert into @tblIDNhoms(ID)
    					select *  from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = 1
    						union all
    						-- get Nhom at this ChiNhanh
    						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) ) tbl
    				end
    			else
    				begin				
    				-- insert all
    				insert into @tblIDNhoms(ID)
    				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    				where LoaiDoiTuong = 1
    				end		
    		end
    	else
    		begin
    			set @idNhomDT = REPLACE( @idNhomDT,''%'','''')
    			insert into @tblIDNhoms(ID) values (@idNhomDT)
    		end',
		';'
		)

	declare @sql2 nvarchar(max)= 
	concat(' WITH Data_CTE ',
    		' AS ',
    		' ( ',
				'SELECT  *
    			 FROM
    			 (
						select hd.MaHoaDon, hd.LoaiHoaDon, hd.NgayLapHoaDon, 
							dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.TenNhomDoiTuongs as TenNhomDT,
							case when dt.IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' else  ISNULL(dt.IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong,
							sms.ThoiGianGui, sms.NoiDung,
							ISNULL(sms.TrangThai,999) as TrangThai,
							nd.TaiKhoan as NguoiGui							
						from BH_HoaDon hd
						join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
						left join HeThong_SMS sms on dt.ID= sms.ID_KhachHang
						left join HT_NguoiDung nd on sms.ID_NguoiGui= nd.ID
						where hd.LoaiHoaDon in (1,3,19)
						and hd.ID_DoiTuong !=''00000000-0000-0000-0000-000000000000''
						and hd.ChoThanhToan is not null
						and ISNULL(dt.DienThoai,'''')!=''''
						and hd.NgayLapHoaDon >''', @FromDate,''' and hd.NgayLapHoaDon < ''',@ToDate, '''', @Where,
				  ')b
				  WHERE 
    			   EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    			),
			Count_CTE ',
    			' AS ',
    			' ( ',
    			' SELECT COUNT(*) AS TotalRow, CEILING(COUNT(*) / CAST(',@PageSize, ' as float )) as TotalPage FROM Data_CTE ',
    			' ) ',
				' SELECT dt.*, cte.TotalPage, cte.TotalRow
				  FROM Data_CTE dt
    			  CROSS JOIN Count_CTE cte
				  ORDER BY NgayLapHoaDon desc',		 
				' OFFSET (', @CurrentPage, ' * ', @PageSize ,') ROWS ',
    			' FETCH NEXT ', @PageSize , ' ROWS ONLY ')

				--print (@sql2)
    		exec (@sql1 + @sql2 )");

            CreateStoredProcedure(name: "[dbo].[SMS_KhachHangSinhNhat]", parametersAction: p => new
            {
                ID_ChiNhanh = p.String(),
                ID_NhomKhachHang = p.String(),
                Where = p.String(),
                CurrentPage = p.Int(),
                PageSize = p.Double()
            }, body: @"SET NOCOUNT ON;
	if @Where='' set @Where= ''
    else set @Where= ' AND '+ @Where 

	declare @from int= @CurrentPage * @PageSize + 1  
    declare @to int= (@CurrentPage + 1) * @PageSize 

	declare @sql1 nvarchar(max)= concat('
    	declare @tblIDNhoms table (ID varchar(36)) 
    	declare @idNhomDT nvarchar(max) = ''', @ID_NhomKhachHang, '''
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh select * from splitstring(''',@ID_ChiNhanh, ''')
    	
    	if @idNhomDT =''%%''
    		begin
				insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')

    			-- check QuanLyKHTheochiNhanh
    			--declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))  
				
				declare @QLTheoCN bit = 0;
				declare @countQL int=0;
				select distinct QuanLyKhachHangTheoDonVi into #temp from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)
				set @countQL = (select COUNT(*) from #temp)
				if	@countQL= 1 
						set @QLTheoCN = (select QuanLyKhachHangTheoDonVi from #temp)
				
    			if @QLTheoCN = 1
    				begin									
    					insert into @tblIDNhoms(ID)
    					select *  from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = 1
    						union all
    						-- get Nhom at this ChiNhanh
    						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) ) tbl
    				end
    			else
    				begin				
    				-- insert all
    				insert into @tblIDNhoms(ID)
    				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    				where LoaiDoiTuong = 1
    				end		
    		end
    	else
    		begin
    			set @idNhomDT = REPLACE( @idNhomDT,''%'','''')
    			insert into @tblIDNhoms(ID) values (@idNhomDT)
    		end',
		';'
		)

	declare @sql2 nvarchar(max)= 
	concat(' WITH Data_CTE ',
    		' AS ',
    		' ( ',
				'SELECT  *
    			 FROM
    			 (
						select dt.ID,
							dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.TenNhomDoiTuongs as TenNhomDT,dt.NgaySinh_NgayTLap,
							case when dt.IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' else  ISNULL(dt.IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong,
							sms.ThoiGianGui,
							ISNULL(sms.NoiDung,'''') as NoiDung,
							ISNULL(sms.TrangThai,999) as TrangThai,
							nd.TaiKhoan	as NguoiGui					
						from DM_DoiTuong dt
						left join HeThong_SMS sms on dt.ID= sms.ID_KhachHang
						left join HT_NguoiDung nd on sms.ID_NguoiGui= nd.ID
						where dt.NgaySinh_NgayTLap is not null and ISNULL(dt.DienThoai,'''') !=''''
						and dt.TheoDoi=0 and dt.LoaiDoiTuong = 1 ',@Where,						
				  ')b
				  WHERE 
    			   EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    			),
			Count_CTE ',
    			' AS ',
    			' ( ',
    			' SELECT COUNT(*) AS TotalRow, CEILING(COUNT(*) / CAST(',@PageSize, ' as float )) as TotalPage FROM Data_CTE ',
    			' ) ',
				' SELECT dt.*, cte.TotalPage, cte.TotalRow
				  FROM Data_CTE dt
    			  CROSS JOIN Count_CTE cte
				  ORDER BY NgaySinh_NgayTLap',		 
				' OFFSET (', @CurrentPage, ' * ', @PageSize ,') ROWS ',
    			' FETCH NEXT ', @PageSize , ' ROWS ONLY ')
			--	print (@sql2)
    		exec (@sql1 + @sql2 )");

            CreateStoredProcedure(name: "[dbo].[UpdateChiTietKiemKe_WhenEditCTHD]", parametersAction: p => new
            {
                IDHoaDonInput = p.Guid(),
                IDChiNhanhInput = p.Guid(),
                NgayLapHDMin = p.DateTime()
            }, body: @"SET NOCOUNT ON;
	declare @tblCTHD ChiTietHoaDonEdit
		INSERT INTO @tblCTHD
		SELECT 
			qd.ID_HangHoa, ct.ID_LoHang, ct.ID_DonViQuiDoi, qd.TyLeChuyenDoi
		FROM BH_HoaDon_ChiTiet ct
		INNER JOIN BH_HoaDon hd ON hd.ID = ct.ID_HoaDon			
		INNER JOIN DonViQuiDoi qd ON qd.ID = ct.ID_DonViQuiDoi			
		INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa    		
		WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1
		GROUP BY qd.ID_HangHoa,ct.ID_DonViQuiDoi,qd.TyLeChuyenDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;	

	-- get cthd KiemKe (LoaiHoaDon = 9) can update
    DECLARE @ChiTietHoaDonUpdate TABLE (IDHoaDon UNIQUEIDENTIFIER,ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, 
    TyLeChuyenDoi FLOAT, TonKho FLOAT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER);
    INSERT INTO @ChiTietHoaDonUpdate
    select hd.ID AS ID_HoaDon, cthd.ID AS ID_ChiTietHoaDon, 
		hd.NgayLapHoaDon, cthd.SoThuTu, cthd.SoLuong, qd.TyLeChuyenDoi,
    	[dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput, hh.ID, cthd.ID_LoHang, hd.NgayLapHoaDon) AS TonKho, qd.ID, cthd.ID_LoHang
	FROM BH_HoaDon hd
    INNER JOIN BH_HoaDon_ChiTiet cthd ON hd.ID = cthd.ID_HoaDon    	
    INNER JOIN DonViQuiDoi qd ON cthd.ID_DonViQuiDoi = qd.ID    	
    INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa    
    INNER JOIN @tblCTHD cthdthemmoiupdate ON cthdthemmoiupdate.ID_HangHoa = hh.ID    	
    WHERE hd.ChoThanhToan = 0 AND hd.LoaiHoaDon = 9 
		and hd.ID_DonVi = @IDChiNhanhInput and hd.NgayLapHoaDon >= @NgayLapHDMin
		AND (cthd.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) 

	-- UPDATE KIEM KE
	-- update TonDauKy(TienChietKhau), SoLuongLech(SoLuong), GiaTriLech(ThanhToan) in ctkiemke
	update ctkiemke
	SET ctkiemke.TienChietKhau = TonDauKy, 
		ctkiemke.SoLuong = ctkiemke.ThanhTien - TonDauKy,
		ctkiemke.ThanhToan = ctkiemke.GiaVon * TonDauKy
	FROM BH_HoaDon_ChiTiet ctkiemke
	join 
		(select ID_ChiTietHoaDon, TonKho/TyLeChuyenDoi as TonDauKy
		from @ChiTietHoaDonUpdate) ctupdate
		on ctkiemke.ID = ctupdate.ID_ChiTietHoaDon

	-- update SoLuongLech tang/giam(TongChiPhi/TongTienHang), SoLuongLech(TongGiamGia), GiaTriLech tang/giam (PhaiThanhToan/TongChietKhau) - todo
    UPDATE hdkkupdate
    SET hdkkupdate.TongTienHang = dshoadonkkupdate.SoLuongGiam, 
		hdkkupdate.TongGiamGia = dshoadonkkupdate.SoLuongLech,
		hdkkupdate.TongChiPhi = dshoadonkkupdate.SoLuongTang
    FROM BH_HoaDon AS hdkkupdate
    INNER JOIN
    	(SELECT ct.ID_HoaDon, 
			SUM(CASE WHEN ct.SoLuong > 0 THEN ct.SoLuong ELSE 0 END) AS SoLuongTang,
    		SUM(CASE WHEN ct.SoLuong < 0 THEN ct.SoLuong ELSE 0 END) AS SoLuongGiam, 
			SUM(SoLuong) AS SoLuongLech
		FROM BH_HoaDon_ChiTiet ct
    	INNER JOIN (SELECT IDHoaDon, IDDonViQuiDoi, ID_LoHang FROM @ChiTietHoaDonUpdate) AS KKHoaDon
    	ON ct.ID_HoaDon = KKHoaDon.IDHoaDon AND ct.ID_DonViQuiDoi = KKHoaDon.IDDonViQuiDoi AND (ct.ID_LoHang = KKHoaDon.ID_LoHang OR KKHoaDon.ID_LoHang IS NULL) 
		GROUP BY ct.ID_HoaDon) AS dshoadonkkupdate
    ON hdkkupdate.ID = dshoadonkkupdate.ID_HoaDon;");

            CreateStoredProcedure(name: "[dbo].[UpdateGiaVon_WhenEditCTHD]", parametersAction: p => new
            {
                IDHoaDonInput = p.Guid(),
                IDChiNhanh = p.Guid(),
                NgayLapHDMin = p.DateTime()
            }, body: @"SET NOCOUNT ON;		
		declare @tblCTHD ChiTietHoaDonEdit
		INSERT INTO @tblCTHD
		SELECT 
			qd.ID_HangHoa, ct.ID_LoHang, ct.ID_DonViQuiDoi, qd.TyLeChuyenDoi
		FROM BH_HoaDon_ChiTiet ct
		INNER JOIN BH_HoaDon hd ON hd.ID = ct.ID_HoaDon			
		INNER JOIN DonViQuiDoi qd ON qd.ID = ct.ID_DonViQuiDoi			
		INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa    		
		WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1
		GROUP BY qd.ID_HangHoa,ct.ID_DonViQuiDoi,qd.TyLeChuyenDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;	

		-- get cthd can update GiaVon
		DECLARE @cthd_NeedUpGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @cthd_NeedUpGiaVon
		select * from 
		(
    	select distinct hdupdate.ID as IDHoaDon, hdupdate.ID_HoaDon, hdupdate.MaHoaDon, hdupdate.LoaiHoaDon, hdctupdate.ID as ID_ChiTietHoaDon, 
    		CASE WHEN hdupdate.YeuCau = '4' AND @IDChiNhanh = hdupdate.ID_CheckIn THEN hdupdate.NgaySua ELSE hdupdate.NgayLapHoaDon END AS NgayLapHoaDon, 				    			    				    							    			
			hdctupdate.SoThuTu, hdctupdate.SoLuong, hdctupdate.DonGia, hdupdate.TongTienHang, hdctupdate.TienChietKhau, hdctupdate.ThanhTien, hdupdate.TongGiamGia, dvqdupdate.TyLeChuyenDoi,
    		[dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanh, hhupdate.ID, hdctupdate.ID_LoHang, 
				CASE WHEN hdupdate.YeuCau = '4' AND @IDChiNhanh = hdupdate.ID_CheckIn THEN hdupdate.NgaySua ELSE hdupdate.NgayLapHoaDon  		    		    			    					
    		END) as TonKho, 	    	
			hdctupdate.GiaVon / dvqdupdate.TyLeChuyenDoi as GiaVon, 
			hdctupdate.GiaVon_NhanChuyenHang / dvqdupdate.TyLeChuyenDoi as GiaVonNhan,
    		hhupdate.ID as ID_HangHoa, hhupdate.LaHangHoa, dvqdupdate.ID as IDDonViQuiDoi, hdctupdate.ID_LoHang, hdctupdate.ID_ChiTietDinhLuong, 
			@IDChiNhanh as IDChiNhanh, hdupdate.ID_CheckIn, hdupdate.YeuCau 
		FROM BH_HoaDon hdupdate
    	INNER JOIN BH_HoaDon_ChiTiet hdctupdate ON hdupdate.ID = hdctupdate.ID_HoaDon    	
    	INNER JOIN DonViQuiDoi dvqdupdate ON hdctupdate.ID_DonViQuiDoi = dvqdupdate.ID   	
    	INNER JOIN DM_HangHoa hhupdate on hhupdate.ID = dvqdupdate.ID_HangHoa   	
    	INNER JOIN @tblCTHD cthdthemmoiupdate  ON cthdthemmoiupdate.ID_HangHoa = hhupdate.ID   
    	WHERE hdupdate.ChoThanhToan = 0 
			AND hdupdate.LoaiHoaDon != 3 AND hdupdate.LoaiHoaDon != 19 
			AND (hdctupdate.ID_LoHang = cthdthemmoiupdate.ID_LoHang OR cthdthemmoiupdate.ID_LoHang IS NULL) 
			AND
    		((hdupdate.ID_DonVi = @IDChiNhanh and hdupdate.NgayLapHoaDon >= @NgayLapHDMin
				and ((hdupdate.YeuCau != '2' and hdupdate.YeuCau != '3') or hdupdate.YeuCau is null))
    		or (hdupdate.YeuCau = '4'  and hdupdate.ID_CheckIn = @IDChiNhanh and hdupdate.NgaySua >= @NgayLapHDMin))
		) as table1
    	order by NgayLapHoaDon, SoThuTu desc, LoaiHoaDon, MaHoaDon;

		--Begin TinhGiaVonTrungBinh
		DECLARE @TinhGiaVonTrungBinh BIT;
		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanh;
		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
		BEGIN
			-- get GiaVon DauKy by ID_QuiDoi
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select
				hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu, hdct.SoLuong, hdct.DonGia, hd.TongTienHang, 
				hdct.TienChietKhau, hdct.ThanhTien, hd.TongGiamGia, 
				dvqd.TyLeChuyenDoi, 0, hdct.GiaVon / dvqd.TyLeChuyenDoi, -- giavon
    			hdct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi, --giavonnhan
    			hh.ID, hh.LaHangHoa, hdct.ID_DonViQuiDoi, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, 
				@IDChiNhanh, hd.ID_CheckIn, hd.YeuCau FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct 	ON hd.ID = hdct.ID_HoaDon    	
    		INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID    		
    		INNER JOIN DM_HangHoa hh on hh.ID = dvqd.ID_HangHoa    		
    		INNER JOIN @tblCTHD cthdthemmoi ON cthdthemmoi.ID_HangHoa = hh.ID    		
    		WHERE hd.ChoThanhToan = 0 AND hd.LoaiHoaDon != 3 AND hd.LoaiHoaDon != 19 
				AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) 
				AND
    				((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon < @NgayLapHDMin and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    					or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua < @NgayLapHDMin))
    		order by NgayLapHoaDon desc, SoThuTu desc, hd.LoaiHoaDon, hd.MaHoaDon;
			
			--select * from @ChiTietHoaDonGiaVon order by NgayLapHoaDon
			--select * from @cthd_NeedUpGiaVon order by NgayLapHoaDon

			-- assign again GiaVon to cthd was edit by ID_HangHoa
    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @BangUpdateGiaVon
    		SELECT *, 
				ROW_NUMBER() OVER (PARTITION BY tableUpdateGiaVon.ID_HangHoa, tableUpdateGiaVon.ID_LoHang ORDER BY tableUpdateGiaVon.NgayLapHoaDon) AS RN 
			FROM
    			(SELECT * 
					FROM @cthd_NeedUpGiaVon
    			UNION ALL
    			SELECT 
					cthdGiaVon.IDHoaDon, cthdGiaVon.IDHoaDonBan, cthdGiaVon.MaHoaDon, cthdGiaVon.LoaiHoaDon, cthdGiaVon.ID_ChiTietHoaDon, cthdGiaVon.NgayLapHoaDon,
					cthdGiaVon.SoThuThu, cthdGiaVon.SoLuong, cthdGiaVon.DonGia, cthdGiaVon.TongTienHang,
    				cthdGiaVon.ChietKhau, cthdGiaVon.ThanhTien, cthdGiaVon.TongGiamGia, cthdGiaVon.TyLeChuyenDoi, cthdGiaVon.TonKho, 
					CASE WHEN cthdGiaVon.GiaVon IS NULL THEN 0 ELSE cthdGiaVon.GiaVon END AS GiaVon, 											
					CASE WHEN cthdGiaVon.GiaVonNhan IS NULL THEN 0 ELSE cthdGiaVon.GiaVonNhan END AS GiaVonNhan,								
					cthd1.ID_HangHoa, cthdGiaVon.LaHangHoa, cthdGiaVon.IDDonViQuiDoi, cthd1.ID_LoHang , cthdGiaVon.ID_ChiTietDinhLuong,
    				@IDChiNhanh, cthdGiaVon.ID_CheckIn, cthdGiaVon.YeuCau 
				FROM @tblCTHD cthd1
				LEFT JOIN 
					(SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
					FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon1 WHERE cthdGiaVon1.RN = 1
					) AS cthdGiaVon
				ON cthd1.ID_HangHoa = cthdGiaVon.ID_HangHoa 
				AND (cthd1.ID_LoHang = cthdGiaVon.ID_LoHang OR cthdGiaVon.ID_LoHang IS NULL)) AS tableUpdateGiaVon;
		--select * from @BangUpdateGiaVon order by NgayLapHoaDon

			-- caculator again GiaVon by ID_HangHoa
			DECLARE @TableTrungGianUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER,IDHangHoa UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, GiaVonNhapHang FLOAT, GiaVonNhanHang FLOAT)
			INSERT INTO @TableTrungGianUpDate
			SELECT 
				IDHoaDon, ID_HangHoa, ID_LoHang, 
				CASE WHEN MAX(TongTienHang) != 0 THEN SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) * (1-(MAX(TongGiamGia) / MAX(TongTienHang))) 
					ELSE SUM(SoLuong * (DonGia - ChietKhau))/ SUM(IIF(SoLuong = 0, 1, SoLuong) * TyLeChuyenDoi) END as GiaVonNhapHang,
				CASE WHEN LoaiHoaDon = 10 THEN SUM(ChietKhau * DonGia)/ SUM(IIF(ChietKhau = 0, 1, ChietKhau) * TyLeChuyenDoi) 
					ELSE 0 END as GiaVonNhanHang
			FROM @BangUpdateGiaVon
			WHERE IDHoaDon = @IDHoaDonInput
				AND ID_HangHoa in (SELECT ID_HangHoa FROM @BangUpdateGiaVon WHERE IDHoaDon = @IDHoaDonInput AND RN = 1)
			GROUP BY ID_HangHoa, ID_LoHang, IDHoaDon,LoaiHoaDon
			
			--select * from @TableTrungGianUpDate 

			DECLARE @RNCheck INT;
			SELECT @RNCheck = MAX(RN) FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang
			IF(@RNCheck = 1)
			BEGIN
				UPDATE @BangUpdateGiaVon SET RN = 2
			END

			-- update GiaVon, GiaVonNhan to @BangUpdateGiaVon if Loai =(4,10), else keep old
    		UPDATE bhctup 
			SET bhctup.GiaVon = 
    			CASE WHEN bhctup.LoaiHoaDon = 4 THEN giavontbup.GiaVonNhapHang	    						
    			ELSE bhctup.GiaVon END,    		
			bhctup.GiaVonNhan = 
				CASE WHEN bhctup.LoaiHoaDon = 10 AND bhctup.YeuCau = '4' AND bhctup.ID_CheckIn = ID_ChiNhanhThemMoi THEN giavontbup.GiaVonNhanHang   		    			
    			ELSE bhctup.GiaVonNhan END  		
			FROM @BangUpdateGiaVon bhctup
			JOIN @TableTrungGianUpDate giavontbup on bhctup.IDHoaDon =giavontbup.IDHoaDon and bhctup.ID_HangHoa = giavontbup.IDHangHoa and (bhctup.ID_LoHang = giavontbup.IDLoHang or giavontbup.IDLoHang is null)
    		WHERE bhctup.IDHoaDon = @IDHoaDonInput AND bhctup.RN = 1;
			--END tính giá vốn trung bình cho lần nhập hàng và chuyển hàng đầu tiền

    		DECLARE @GiaVonCapNhat TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, IDHoaDonCu UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, IDChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoLuong FLOAT, DonGia FLOAT, TongTienHang FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, GiaVonCu FLOAT, IDHangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, IDChiTietDinhLuong UNIQUEIDENTIFIER,
    		IDChiNhanhThemMoi UNIQUEIDENTIFIER, IDCheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @GiaVonCapNhat
    		SELECT 
				tableUpdate.IDHoaDon, tableUpdate.IDHoaDonBan, tableGiaVon.IDHoaDon, tableUpdate.MaHoaDon, tableUpdate.LoaiHoaDon, tableUpdate.ID_ChiTietHoaDon,tableUpdate.NgayLapHoaDon, tableUpdate.SoLuong, tableUpdate.DonGia,
    			tableUpdate.TongTienHang, tableUpdate.ChietKhau, tableUpdate.ThanhTien, tableUpdate.TongGiamGia, tableUpdate.TyLeChuyenDoi, tableUpdate.TonKho, tableUpdate.GiaVon, tableUpdate.GiaVonNhan, tableGiaVon.GiaVon, tableUpdate.ID_HangHoa, tableUpdate.LaHangHoa,
    			tableUpdate.IDDonViQuiDoi, tableUpdate.ID_LoHang, tableUpdate.ID_ChiTietDinhLuong, tableUpdate.ID_ChiNhanhThemMoi, tableUpdate.ID_CheckIn, tableUpdate.YeuCau, tableUpdate.RN 
			FROM @BangUpdateGiaVon tableUpdate
    		LEFT JOIN (SELECT (CASE WHEN ID_CheckIn = ID_ChiNhanhThemMoi THEN GiaVonNhan ELSE GiaVon END) AS GiaVon, ID_HangHoa, IDHoaDon, ID_LoHang, RN + 1 AS RN 
						FROM @BangUpdateGiaVon) AS tableGiaVon
    		ON tableUpdate.ID_HangHoa = tableGiaVon.ID_HangHoa AND tableUpdate.RN = tableGiaVon.RN 
			AND (tableUpdate.ID_LoHang = tableGiaVon.ID_LoHang OR tableUpdate.ID_LoHang IS NULL);

			--select * from @GiaVonCapNhat
			
    		DECLARE @IDHoaDon UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonBan UNIQUEIDENTIFIER;
    		DECLARE @IDHoaDonCu UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX);
    		DECLARE @LoaiHoaDon INT;
    		DECLARE @IDChiTietHoaDon UNIQUEIDENTIFIER;
    		DECLARE @SoLuong FLOAT;
    		DECLARE @DonGia FLOAT;
    		DECLARE @TongTienHang FLOAT;
    		DECLARE @ChietKhau FLOAT;
			DECLARE @ThanhTien FLOAT;
    		DECLARE @TongGiamGia FLOAT;
    		DECLARE @TyLeChuyenDoi FLOAT;
    		DECLARE @TonKho FLOAT;
    		DECLARE @GiaVonCu FLOAT;
    		DECLARE @IDHangHoa UNIQUEIDENTIFIER;
    		DECLARE @IDDonViQuiDoi UNIQUEIDENTIFIER;
    		DECLARE @IDLoHang UNIQUEIDENTIFIER;
    		DECLARE @IDChiNhanhThemMoi UNIQUEIDENTIFIER;
    		DECLARE @IDCheckIn UNIQUEIDENTIFIER;
    		DECLARE @YeuCau NVARCHAR(MAX);
    		DECLARE @RN INT;
			DECLARE @GiaVon FLOAT;
			DECLARE @GiaVonNhan FLOAT;
    		DECLARE @GiaVonMoi FLOAT;
    		DECLARE @GiaVonCuUpdate FLOAT;
    		DECLARE @IDHangHoaUpdate UNIQUEIDENTIFIER;
    		DECLARE @IDLoHangUpdate UNIQUEIDENTIFIER;
			DECLARE @GiaVonHoaDonBan FLOAT;
    		DECLARE @TongTienHangDemo FLOAT;
    		DECLARE @SoLuongDemo FLOAT;
			DECLARE @ThanhTienDemo FLOAT;
			DECLARE @ChietKhauDemo FLOAT;

    		DECLARE CS_GiaVon CURSOR SCROLL LOCAL FOR 
			SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
    			GiaVonCu, IDHangHoa, IDDonViQuiDoi, IDLoHang, IDChiNhanhThemMoi, IDCheckIn, YeuCau, RN, GiaVon, GiaVonNhan 
			FROM @GiaVonCapNhat WHERE RN > 1 and LaHangHoa = 1
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon 
			INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau,@ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
    			iF(@IDHangHoaUpdate = @IDHangHoa AND (@IDLoHangUpdate = @IDLoHang OR @IDLoHang IS NULL))
    			BEGIN
    				SET @GiaVonCu = @GiaVonCuUpdate;
    			END
    			ELSE
    			BEGIN
    				SET @IDHangHoaUpdate = @IDHangHoa;
    				SET @IDLoHangUpdate = @IDLoHang;
					SET @GiaVonCuUpdate = @GiaVonCu;
    			END
				IF(@GiaVonCu IS NOT NULL)
				BEGIN
    				IF(@LoaiHoaDon = 4)
    				BEGIN
    					--SELECT @TongTienHangDemo = SUM(bhctdm.ThanhTien - bhctdm.SoLuong * bhctdm.ChietKhau), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia -  bhctdm.ChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang

						--select @IDHoaDon, @MaHoaDon, @TongTienHangDemo, @SoLuongDemo, @TonKho
    					IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    					BEGIN
    						IF(@TongTienHang != 0)
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + (@TongTienHangDemo* (1-(@TongGiamGia/@TongTienHang))))/(@SoLuongDemo + @TonKho);
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo)/(@SoLuongDemo + @TonKho);
    						END
    					END
    					ELSE
    					BEGIN
					
    						IF(@TongTienHang != 0)
    						BEGIN
    							SET	@GiaVonMoi = (@TongTienHangDemo / @SoLuongDemo) * (1 - (@TongGiamGia / @TongTienHang));
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
    						END
    					END

						--select @GiaVonMoi
    				END
    				ELSE IF (@LoaiHoaDon = 7)
    				BEGIN
    					--select @IDHoaDon, @MaHoaDon, @TongTienHangDemo, @SoLuongDemo, @TonKho
						
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@TonKho - @SoLuongDemo > 0)
    					BEGIN
    						SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) - @TongTienHangDemo)/(@TonKho - @SoLuongDemo);
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
						--select @GiaVonMoi
    				END
    				ELSE IF(@LoaiHoaDon = 10)
    				BEGIN
    					SELECT @TongTienHangDemo = SUM(bhctdm.ChietKhau * bhctdm.DonGia), @SoLuongDemo = SUM(bhctdm.ChietKhau * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang

    					IF(@YeuCau = '1' OR (@YeuCau = '4' AND @IDChiNhanhThemMoi != @IDCheckIn))
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    					ELSE IF (@YeuCau = '4' AND @IDChiNhanhThemMoi = @IDCheckIn)
    					BEGIN
    						IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    						BEGIN
    							SET @GiaVonMoi = (@GiaVonCu * @TonKho + @TongTienHangDemo)/(@TonKho + @SoLuongDemo);
    						END
    						ELSE
    						BEGIN
								IF(@SoLuongDemo = 0)
								BEGIN
									SET @GiaVonMoi = @GiaVonCu;
								END
								ELSE
								BEGIN
    								SET @GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
								END
    						END
    					END
    				END
    				ELSE IF (@LoaiHoaDon = 6)
    				BEGIN
    					SELECT @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    					IF(@IDHoaDonBan IS NOT NULL)
    					BEGIN
    						SET @GiaVonHoaDonBan = -1;
    						SELECT @GiaVonHoaDonBan = GiaVon FROM @GiaVonCapNhat WHERE IDHoaDon = @IDHoaDonBan AND IDDonViQuiDoi = @IDDonViQuiDoi AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL);
						
    						IF(@GiaVonHoaDonBan = -1)
    						BEGIN
							
    							SELECT @GiaVonHoaDonBan = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDonBan AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
						
    						END
    						IF(@TonKho + @SoLuongDemo > 0 AND @TonKho > 0)
    						BEGIN
							
    							SET @GiaVonMoi = (@GiaVonCu * @TonKho + @GiaVonHoaDonBan * @SoLuongDemo) / (@TonKho + @SoLuongDemo);
    						END
    						ELSE
    						BEGIN
    							SET @GiaVonMoi = @GiaVonHoaDonBan;
    						END
    					END
    					ELSE
    					BEGIN
    						SET @GiaVonMoi = @GiaVonCu;
    					END
    				END
    				ELSE IF(@LoaiHoaDon = 18)
					BEGIN
						SELECT @GiaVonMoi = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID_HoaDon = @IDHoaDon AND ID_DonViQuiDoi = @IDDonViQuiDoi AND (ID_LoHang = @IDLoHang OR @IDLoHang IS NULL);
					END
					ELSE
    				BEGIN
    					SET @GiaVonMoi = @GiaVonCu;
    				END
				END
				ELSE
				BEGIN
					IF(@IDCheckIn = @IDChiNhanhThemMoi)
					BEGIN
						SET @GiaVonMoi = @GiaVonNhan
					END
					ELSE
					BEGIN
						SET @GiaVonMoi = @GiaVon
					END
				END

    			IF(@IDHoaDon = @IDHoaDonCu)
    			BEGIN
    				SET @GiaVonMoi = @GiaVonCuUpdate;	
    			END
    			ELSE
    			BEGIN
    				SET @GiaVonCuUpdate = @GiaVonMoi;
    			END
    			IF(@LoaiHoaDon = 10 AND @YeuCau = '4' AND @IDCheckIn = @IDChiNhanhThemMoi)
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVonNhan = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			ELSE
    			BEGIN
    				UPDATE @GiaVonCapNhat SET GiaVon = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN;
    				UPDATE @GiaVonCapNhat SET GiaVonCu = @GiaVonMoi WHERE IDHangHoa = @IDHangHoa AND (IDLoHang = @IDLoHang OR @IDLoHang IS NULL) AND RN = @RN +1;
    			END
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, @TongTienHang, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon			

		--	select * from @GiaVonCapNhat
    		--Update BH_HoaDon_ChiTiet
    		UPDATE hoadonchitiet1
    		SET hoadonchitiet1.GiaVon = _giavoncapnhat1.GiaVon * _giavoncapnhat1.TyLeChuyenDoi,
			hoadonchitiet1.GiaVon_NhanChuyenHang = _giavoncapnhat1.GiaVonNhan * _giavoncapnhat1.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet1
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat1 ON hoadonchitiet1.ID = _giavoncapnhat1.IDChiTietHoaDon   		
    		WHERE _giavoncapnhat1.LoaiHoaDon != 8 AND _giavoncapnhat1.LoaiHoaDon != 18 AND _giavoncapnhat1.LoaiHoaDon != 9 AND _giavoncapnhat1.RN > 1;

			-- update GiaVon to phieu KiemKe
			UPDATE hoadonchitiet4
    		SET hoadonchitiet4.GiaVon = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi, 
			hoadonchitiet4.ThanhToan = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi *hoadonchitiet4.SoLuong
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet4
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat4 ON hoadonchitiet4.ID = _giavoncapnhat4.IDChiTietHoaDon    		
    		WHERE _giavoncapnhat4.LoaiHoaDon = 9 AND _giavoncapnhat4.RN > 1;
    
			-- update GiaVon to phieu XuatKho
    		UPDATE hoadonchitiet2
    		SET hoadonchitiet2.GiaVon = _giavoncapnhat2.GiaVon * _giavoncapnhat2.TyLeChuyenDoi, 
				hoadonchitiet2.ThanhTien = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong* _giavoncapnhat2.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet2
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat2 ON hoadonchitiet2.ID = _giavoncapnhat2.IDChiTietHoaDon    		
    		WHERE _giavoncapnhat2.LoaiHoaDon = 8 AND _giavoncapnhat2.RN > 1;
    
			-- update GiaVon to Loai = 18 (Phieu DieuChinh GiaVon)
    		UPDATE hoadonchitiet3
    		SET hoadonchitiet3.DonGia = _giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi, 
				hoadonchitiet3.PTChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) ELSE 0 END,
    			hoadonchitiet3.TienChietKhau = CASE WHEN hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) > 0 THEN 0 ELSE hoadonchitiet3.GiaVon - (_giavoncapnhat3.GiaVonCu * _giavoncapnhat3.TyLeChuyenDoi) END
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet3
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat3
    		ON hoadonchitiet3.ID = _giavoncapnhat3.IDChiTietHoaDon
    		WHERE _giavoncapnhat3.LoaiHoaDon = 18 AND _giavoncapnhat3.RN > 1;
    
    		UPDATE chitietdinhluong
    		SET chitietdinhluong.GiaVon = gvDinhLuong.GiaVonDinhLuong / iif(chitietdinhluong.SoLuong=0,1,chitietdinhluong.SoLuong)
    		FROM BH_HoaDon_ChiTiet AS chitietdinhluong
    		INNER JOIN
    			(SELECT 
					SUM(ct.GiaVon * ct.SoLuong) AS GiaVonDinhLuong, ct.ID_ChiTietDinhLuong 
				FROM BH_HoaDon_ChiTiet ct
    			INNER JOIN (SELECT IDChiTietDinhLuong FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDChiTietDinhLuong) gv
    			ON (ct.ID_ChiTietDinhLuong = gv.IDChiTietDinhLuong and ct.ID_ChiTietDinhLuong IS NOT NULL)
    			WHERE gv.IDChiTietDinhLuong IS NOT NULL AND ct.ID != ct.ID_ChiTietDinhLuong
    			GROUP BY ct.ID_ChiTietDinhLuong) AS gvDinhLuong
    		ON chitietdinhluong.ID = gvDinhLuong.ID_ChiTietDinhLuong
    		--END Update BH_HoaDon_ChiTiet
    		--Update DM_GiaVon
    		UPDATE _dmGiaVon1
    		SET _dmGiaVon1.GiaVon = ISNULL(_gvUpdateDM1.GiaVon, 0)
    		FROM 
				(SELECT dvqd1.ID AS IDDonViQuiDoi, _giavon1.IDLoHang AS IDLoHang, (CASE WHEN _giavon1.IDCheckIn = _giavon1.IDChiNhanhThemMoi THEN _giavon1.GiaVonNhan ELSE _giavon1.GiaVon END) * dvqd1.TyLeChuyenDoi AS GiaVon, _giavon1.IDChiNhanhThemMoi AS IDChiNhanh 
				FROM @GiaVonCapNhat _giavon1
    			INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN > 1 GROUP BY IDHangHoa,IDLoHang) AS _maxGiaVon1
    			ON _giavon1.IDHangHoa = _maxGiaVon1.IDHangHoa AND _giavon1.RN = _maxGiaVon1.RN AND (_giavon1.IDLoHang = _maxGiaVon1.IDLoHang OR _maxGiaVon1.IDLoHang IS NULL)
    			INNER JOIN DonViQuiDoi dvqd1
    			ON dvqd1.ID_HangHoa = _giavon1.IDHangHoa) AS _gvUpdateDM1
    		LEFT JOIN DM_GiaVon _dmGiaVon1
    		ON _gvUpdateDM1.IDChiNhanh = _dmGiaVon1.ID_DonVi AND _gvUpdateDM1.IDDonViQuiDoi = _dmGiaVon1.ID_DonViQuiDoi AND (_gvUpdateDM1.IDLoHang = _dmGiaVon1.ID_LoHang OR _gvUpdateDM1.IDLoHang IS NULL)
    		WHERE _dmGiaVon1.ID IS NOT NULL;
    
    		INSERT INTO DM_GiaVon (ID, ID_DonVi, ID_DonViQuiDoi, ID_LoHang, GiaVon)
    		SELECT NEWID(), _gvUpdateDM.IDChiNhanh, _gvUpdateDM.IDDonViQuiDoi, _gvUpdateDM.IDLoHang, _gvUpdateDM.GiaVon 
			FROM 
    			(SELECT dvqd2.ID AS IDDonViQuiDoi, _giavon2.IDLoHang AS IDLoHang, 
					(CASE WHEN _giavon2.IDCheckIn = _giavon2.IDChiNhanhThemMoi THEN _giavon2.GiaVonNhan ELSE _giavon2.GiaVon END) * dvqd2.TyLeChuyenDoi AS GiaVon, 
					_giavon2.IDChiNhanhThemMoi AS IDChiNhanh 
				FROM @GiaVonCapNhat _giavon2
    			INNER JOIN (SELECT IDHangHoa,IDLoHang, MAX(RN) AS RN FROM @GiaVonCapNhat WHERE RN >1 GROUP BY IDHangHoa, IDLoHang) AS _maxGiaVon
    		ON _giavon2.IDHangHoa = _maxGiaVon.IDHangHoa AND _giavon2.RN = _maxGiaVon.RN AND (_giavon2.IDLoHang = _maxGiaVon.IDLoHang OR _maxGiaVon.IDLoHang IS NULL)
    		INNER JOIN DonViQuiDoi dvqd2 ON dvqd2.ID_HangHoa = _giavon2.IDHangHoa) AS _gvUpdateDM    		
    		LEFT JOIN DM_GiaVon _dmGiaVon
    		ON _gvUpdateDM.IDChiNhanh = _dmGiaVon.ID_DonVi AND _gvUpdateDM.IDDonViQuiDoi = _dmGiaVon.ID_DonViQuiDoi AND (_gvUpdateDM.IDLoHang = _dmGiaVon.ID_LoHang OR _gvUpdateDM.IDLoHang IS NULL)
    		WHERE _dmGiaVon.ID IS NULL;
    		--End Update DM_GiaVon
		END
		--END TinhGiaVonTrungBinh");

            CreateStoredProcedure(name: "[dbo].[UpdateIDKhachHang_inSoQuy]", parametersAction: p => new
            {
                ID_HoaDonLienQuan = p.Guid()
            }, body: @"declare @IDDoiTuong UNIQUEIDENTIFIER = (select ID_DoiTuong from BH_HoaDon where id= @ID_HoaDonLienQuan)
	-- update ID_KhachHang
	Update Quy_HoaDon_ChiTiet set ID_DoiTuong = @IDDoiTuong where ID_HoaDonLienQuan = @ID_HoaDonLienQuan 
	-- update NguoiNopTien
	Update hd set NguoiNopTien = TenDoiTuong
	from Quy_HoaDon hd
	join Quy_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
	left join DM_DoiTuong dt on ct.ID_DoiTuong= dt.ID
	where ct.ID_HoaDonLienQuan = @ID_HoaDonLienQuan");

			CreateStoredProcedure(name: "[dbo].[UpdateTonLuyKeCTHD_whenUpdate]", parametersAction: p => new
			{
				IDHoaDonInput = p.Guid(),
				IDChiNhanhInput = p.Guid(),
				NgayLapHDOld = p.DateTime()
			}, body: @"SET NOCOUNT ON;

		DECLARE @NgayLapHDNew DATETIME;   
		DECLARE @NgayNhanHang DATETIME;
		declare @tblHoaDonChiTiet ChiTietHoaDonEdit -- table user defined
		DECLARE @IDCheckIn  UNIQUEIDENTIFIER, @YeuCau NVARCHAR(MAX),  @LoaiHoaDon INT, @NgayLapHDMin DATETIME;
		DECLARE @tblChiTiet TABLE (ID_HangHoa UNIQUEIDENTIFIER not null, ID_LoHang UNIQUEIDENTIFIER null, ID_DonViQuiDoi UNIQUEIDENTIFIER not null, TyLeChuyenDoi float not null)
		DECLARE @LuyKeDauKy TABLE(ID_LoHang UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TonLuyKe FLOAT);
		DECLARE @hdctUpdate TABLE(ID UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, TonLuyKe FLOAT, LoaiHoaDon INT, 
		MaHoaDon nvarchar(max), NgayLapHoaDon datetime, YeuCau nvarchar(max));

		--  get NgayLapHD by IDHoaDon: if update HDNhanHang (loai 10, yeucau = 4 --> get NgaySua
		select 
			@NgayLapHDNew = NgayLapHoaDon,
			@NgayNhanHang = NgaySua,
			@LoaiHoaDon = LoaiHoaDon, @YeuCau = YeuCau, @IDCheckIn = ID_CheckIn				
		from (
					select LoaiHoaDon, YeuCau, ID_CheckIn, NgaySua, case when @IDChiNhanhInput = ID_CheckIn and YeuCau !='1' then NgaySua else NgayLapHoaDon end as NgayLapHoaDon
					from BH_HoaDon where ID = @IDHoaDonInput) a

		-- alway get Ngay min --> compare to update TonLuyKe
		IF(@NgayLapHDOld > @NgayLapHDNew)
			SET @NgayLapHDMin = @NgayLapHDNew;
		ELSE
			SET @NgayLapHDMin = @NgayLapHDOld;

		-- get cthd update by IDHoaDon
		INSERT INTO @tblChiTiet
		SELECT 
			qd.ID_HangHoa, ct.ID_LoHang, ct.ID_DonViQuiDoi, qd.TyLeChuyenDoi
		FROM BH_HoaDon_ChiTiet ct
		INNER JOIN BH_HoaDon hd ON hd.ID = ct.ID_HoaDon			
		INNER JOIN DonViQuiDoi qd ON qd.ID = ct.ID_DonViQuiDoi			
		INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa    		
		WHERE hd.ID = @IDHoaDonInput AND hh.LaHangHoa = 1
		GROUP BY qd.ID_HangHoa,ct.ID_DonViQuiDoi,qd.TyLeChuyenDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn, hd.YeuCau, hd.NgaySua, hd.NgayLapHoaDon;	
		insert into @tblHoaDonChiTiet select * from @tblChiTiet			
				
		-- get cthd has KiemKe group by ID_HangHoa, ID_LoHang
		declare @tblHangKiemKe table (NgayKiemKe datetime, ID_HangHoa uniqueidentifier null, ID_LoHang uniqueidentifier null)
		insert into @tblHangKiemKe
		select distinct NgayLapHoaDon, qd.ID_HangHoa, ct.ID_LoHang
			from BH_HoaDon_ChiTiet ct 
			join BH_HoaDon hd ON hd.ID = ct.ID_HoaDon		
			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
			join @tblChiTiet tblct ON qd.ID_HangHoa = tblct.ID_HangHoa AND (ct.ID_LoHang = tblct.ID_LoHang OR ct.ID_LoHang IS NULL)	
			WHERE hd.ChoThanhToan = 0
			and hd.LoaiHoaDon= 9
			and hd.ID_DonVi = @IDChiNhanhInput and NgayLapHoaDon >= @NgayLapHDMin
			group by qd.ID_HangHoa, ct.ID_LoHang, hd.NgayLapHoaDon				
		
		-- get cthd liên quan
		select
			ct.ID, 
			ct.ID_LoHang,
			case when ct.ChatLieu= '5' then 0 else SoLuong end as SoLuong, -- chatlieu = 5 (cthd bi xoa khi updateHD)
			case when ct.ChatLieu= '5' then 0 else TienChietKhau end as TienChietKhau,
			case when ct.ChatLieu= '5' then 0 else ct.ThanhTien end as ThanhTien,-- kiemke bi huy
			case when hd.LoaiHoaDon= 10 and  hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput then ct.TonLuyKe_NhanChuyenHang else ct.TonLuyKe end as TonDauKy,
			qd.ID_HangHoa,
			qd.TyLeChuyenDoi,
			hd.MaHoaDon,
			hd.LoaiHoaDon,
			hd.ID_DonVi,
			hd.ID_CheckIn,
			hd.YeuCau,								
			case when hd.YeuCau = '4' AND hd.ID_CheckIn = @IDChiNhanhInput then hd.NgaySua else hd.NgayLapHoaDon end as NgayLapHoaDon
		into #temp
		from BH_HoaDon_ChiTiet ct
		join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
		join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
		join @tblChiTiet ctupdate on qd.ID_HangHoa = ctupdate.ID_HangHoa AND (ct.ID_LoHang = ctupdate.ID_LoHang OR ct.ID_LoHang IS NULL)	
		WHERE hd.ChoThanhToan = 0					
		AND (hd.ID_DonVi = @IDChiNhanhInput OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4'))

		-- table cthd has ID_HangHoa exist cthd kiemke
		declare @cthdHasKiemKe table (ID_HangHoa uniqueidentifier, ID_LoHang uniqueidentifier)
		declare @tblNgayKiemKe table (NgayKiemKe datetime)

		declare @count float= (select count(*) from @tblHangKiemKe)
		--if @count > 0
			begin						
				declare @ID_HangHoa uniqueidentifier, @ID_LoHang uniqueidentifier				
				DECLARE Cur_tblKiemKe CURSOR SCROLL LOCAL FOR
				select ID_HangHoa, ID_LoHang
				from @tblHangKiemKe
				order by NgayKiemKe

				OPEN Cur_tblKiemKe -- cur 1
    			FETCH FIRST FROM Cur_tblKiemKe
				INTO @ID_HangHoa, @ID_LoHang
				WHILE @@FETCH_STATUS = 0
    				BEGIN	
						if not exists (select * from @cthdHasKiemKe kk where kk.ID_HangHoa= @ID_HangHoa and (kk.ID_LoHang= @ID_LoHang OR kk.ID_LoHang is null))
							begin
								-- get list NgayKiemKe by ID_HangHoa & ID_LoHang								
								declare @NgayKiemKe datetime
								declare @FromDate datetime = @NgayLapHDMin

								-- get cac khoang thoigian kiemke
								insert into @tblNgayKiemKe
								select *
								from
									( select NgayKiemKe 
									from @tblHangKiemKe kk where kk.ID_HangHoa = @ID_HangHoa and (kk.ID_LoHang= @ID_LoHang or kk.ID_LoHang is null)						
									union 
										select GETDATE() as NgayKiemKe
									) b order by NgayKiemKe

								DECLARE Cur_NgayKiemKe CURSOR SCROLL LOCAL FOR								
								select NgayKiemKe from @tblNgayKiemKe

								OPEN Cur_NgayKiemKe -- cur 2
    							FETCH FIRST FROM Cur_NgayKiemKe
								INTO @NgayKiemKe
								WHILE @@FETCH_STATUS = 0
									begin											
										insert into @cthdHasKiemKe values(@ID_HangHoa, @ID_LoHang)
										-- get tondauky 
										if @FromDate = @NgayLapHDMin and @LoaiHoaDon !=9		
											begin
												insert into @LuyKeDauKy
												select 
													ID_LoHang,ID_HangHoa,TonDauKy																		
												from
													(
													select 
														ID_LoHang,ID_HangHoa,TonDauKy,										
														ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
													from #temp
													where NgayLapHoaDon < @FromDate		
													and #temp.ID_HangHoa = @ID_HangHoa AND (#temp.ID_LoHang = @ID_LoHang OR #temp.ID_LoHang IS NULL)										
													) luyke	
												where luyke.RN= 1									
											end
										else
											begin
												insert into @LuyKeDauKy
												select 
													ID_LoHang,ID_HangHoa,TonDauKy
												from
													(
													select 
														ID_LoHang,ID_HangHoa,TonDauKy,
														ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
													from #temp
													where NgayLapHoaDon <=  @FromDate 
													and #temp.ID_HangHoa = @ID_HangHoa AND (#temp.ID_LoHang = @ID_LoHang OR #temp.ID_LoHang IS NULL)		
													) luyke	
												where luyke.RN= 1
											end
		
										--- tinh lai tonluyke
										INSERT INTO @hdctUpdate
										select ID, ID_DonVi, ID_CheckIn,
												ISNULL(lkdk.TonLuyKe, 0) + 
												(SUM(IIF(LoaiHoaDon IN (1, 5, 7, 8), -1 * a.SoLuong* a.TyLeChuyenDoi, 
    											IIF(LoaiHoaDon IN (4, 6, 18), SoLuong * TyLeChuyenDoi, 				
												IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    											IIF(a.LoaiHoaDon = 10 AND a.YeuCau = '4' AND a.ID_CheckIn = @IDChiNhanhInput, a.TienChietKhau* a.TyLeChuyenDoi, 0))))) 
												OVER(PARTITION BY a.ID_HangHoa, a.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe,
												LoaiHoaDon, MaHoaDon,NgayLapHoaDon, YeuCau
										from
											(							
											select distinct
												ID,
												ID_LoHang,
												SoLuong,
												TienChietKhau,
												ThanhTien,
												ID_HangHoa,
												TyLeChuyenDoi,
												MaHoaDon,
												LoaiHoaDon,
												NgayLapHoaDon,
												ID_DonVi,
												ID_CheckIn,
												YeuCau
											from #temp
											where NgayLapHoaDon >= @FromDate
												and NgayLapHoaDon < @NgayKiemKe
												and #temp.ID_HangHoa = @ID_HangHoa AND (#temp.ID_LoHang = @ID_LoHang or #temp.ID_LoHang IS NULL	)						
											) a
										LEFT JOIN @LuyKeDauKy lkdk ON lkdk.ID_HangHoa = a.ID_HangHoa AND (lkdk.ID_LoHang = a.ID_LoHang OR a.ID_LoHang IS NULL)	
						
										-- xóa TonLuyKe trước đó để lấy TonLuyKe mới theo khoảng thời gian		
										set @FromDate = @NgayKiemKe
										--select *, 1 as after1 from @LuyKeDauKy
										delete from @LuyKeDauKy															
										FETCH NEXT FROM Cur_NgayKiemKe INTO @NgayKiemKe
									end
								CLOSE Cur_NgayKiemKe  
								DEALLOCATE Cur_NgayKiemKe 
							end		

						-- delete & assign again in for loop
						delete from @tblNgayKiemKe
						FETCH NEXT FROM Cur_tblKiemKe INTO @ID_HangHoa,@ID_LoHang
					END
				CLOSE Cur_tblKiemKe  
				DEALLOCATE Cur_tblKiemKe 				
			end

			-- get luyke dauky of HangHoa not exist in ctkiemke
			begin
				insert into @LuyKeDauKy
				select 
					ID_LoHang,ID_HangHoa,TonDauKy											
				from
					(
					select 
						ID_LoHang,ID_HangHoa,TonDauKy,
						ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
					from #temp
					where NgayLapHoaDon < @NgayLapHDMin 
						and not exists (select * from @tblHangKiemKe kk where #temp.ID_HangHoa =  kk.ID_HangHoa and (#temp.ID_LoHang = kk.ID_LoHang OR #temp.ID_LoHang is null))
					) luyke	
				where luyke.RN= 1

				-- caculator again TonLuyKe for all cthd 'liên quan'
				INSERT INTO @hdctUpdate
				select ID, ID_DonVi, ID_CheckIn,
						ISNULL(lkdk.TonLuyKe, 0) + 
						(SUM(IIF(LoaiHoaDon IN (1, 5, 7, 8), -1 * a.SoLuong* a.TyLeChuyenDoi, 
    					IIF(LoaiHoaDon IN (4, 6, 18), SoLuong * TyLeChuyenDoi, 				
						IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    					IIF(a.LoaiHoaDon = 10 AND a.YeuCau = '4' AND a.ID_CheckIn = @IDChiNhanhInput, a.TienChietKhau* a.TyLeChuyenDoi, 0))))) 
						OVER(PARTITION BY a.ID_HangHoa, a.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe,
						LoaiHoaDon, MaHoaDon,NgayLapHoaDon,YeuCau
				from
					(
					select distinct
						ID,
						ID_LoHang,
						SoLuong,
						TienChietKhau,
						ThanhTien,
						ID_HangHoa,
						TyLeChuyenDoi,
						MaHoaDon,
						LoaiHoaDon,
						NgayLapHoaDon,
						ID_DonVi,
						ID_CheckIn,
						YeuCau
					from #temp
					where NgayLapHoaDon >= @NgayLapHDMin
					and not exists (select * from @tblHangKiemKe kk where #temp.ID_HangHoa =  kk.ID_HangHoa and (#temp.ID_LoHang = kk.ID_LoHang OR #temp.ID_LoHang is null))
					) a
				LEFT JOIN @LuyKeDauKy lkdk ON lkdk.ID_HangHoa = a.ID_HangHoa AND (lkdk.ID_LoHang = a.ID_LoHang OR a.ID_LoHang IS NULL)					
			end
		
		--select *, 1 as after2 from @LuyKeDauKy
		--select * , @NgayLapHDMin as NgayMin from @hdctUpdate order by NgayLapHoaDon desc

		UPDATE hdct
    	SET hdct.TonLuyKe = IIF(tlkupdate.ID_DonVi = @IDChiNhanhInput, tlkupdate.TonLuyKe, hdct.TonLuyKe), 
		hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput and tlkupdate.LoaiHoaDon = 10, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
    	FROM BH_HoaDon_ChiTiet hdct
    	INNER JOIN @hdctUpdate tlkupdate ON hdct.ID = tlkupdate.ID where tlkupdate.LoaiHoaDon !=9 -- don't update TonLuyKe of HD KiemKe

		-- get TonKho hientai full ID_QuiDoi, ID_LoHang of ID_HangHoa
		DECLARE @tblTonKho1 TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, TonKho FLOAT, ID_LoHang UNIQUEIDENTIFIER)
		INSERT INTO @tblTonKho1
		SELECT qd.ID, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput,qd.ID_HangHoa,ID_LoHang, DATEADD(minute, 2,GETDATE()))/qd.TyLeChuyenDoi as TonKho, ID_LoHang 
		FROM @tblChiTiet ct
		join DonViQuiDoi qd on ct.ID_HangHoa = qd.ID_HangHoa 
		
		--select * from @tblTonKho1

		-- UPDATE TonKho in DM_HangHoa_TonKho
		UPDATE hhtonkho SET hhtonkho.TonKho = ISNULL(cthoadon.TonKho, 0)
		FROM DM_HangHoa_TonKho hhtonkho
		INNER JOIN @tblTonKho1 as cthoadon on hhtonkho.ID_DonViQuyDoi = cthoadon.ID_DonViQuiDoi 
			and (hhtonkho.ID_LoHang = cthoadon.ID_LoHang or cthoadon.ID_LoHang is null) and hhtonkho.ID_DonVi = @IDChiNhanhInput

		-- delete cthd was delete in cthd update
		--delete from BH_HoaDon_ChiTiet where id in (select id from @tblChiTiet where ChatLieu='5') OR(ID_HoaDon = @IDHoaDonInput and ChatLieu='5')	
		delete from BH_HoaDon_ChiTiet where ID_HoaDon = @IDHoaDonInput and ChatLieu='5'

		-- neu update NhanHang --> goi ham update TonKho 2 lan
		-- update GiaVon neu tontai phieu NhapHang,ChuyenHang/NhanHang, DieuChinhGiaVon 
		declare @count2 float = (select count(ID) from @hdctUpdate where LoaiHoaDon in (4,7,10, 18))
		select ISNULL(@count2,0) as UpdateGiaVon, ISNULL(@count,0) as UpdateKiemKe, @NgayLapHDMin as NgayLapHDMin");

            Sql(@"ALTER PROCEDURE [dbo].[DanhMucKhachHang_CongNo_ChotSo_Paging]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @MaKH [nvarchar](max),
    @LoaiKH [int],
    @ID_NhomKhachHang [nvarchar](max),
    @timeStartKH [datetime],
    @timeEndKH [datetime],
    @CurrentPage [int],
    @PageSize [float],
    @Where [nvarchar](max),
    @SortBy [nvarchar](100)
AS
BEGIN
    SET NOCOUNT ON;
    	if @SortBy ='' set @SortBy = ' dt.NgayTao DESC'
    	if @Where='' set @Where= ''
    	else set @Where= ' AND '+ @Where 
    
    	declare @from int= @CurrentPage * @PageSize + 1  
    	declare @to int= (@CurrentPage + 1) * @PageSize 
    
    	declare @sql1 nvarchar(max)= concat('
    	declare @tblIDNhoms table (ID varchar(36)) 
    	declare @idNhomDT nvarchar(max) = ''', @ID_NhomKhachHang, '''
    	
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh select * from splitstring(''',@ID_ChiNhanh,''')
    	
    	if @idNhomDT =''%%''
    		begin    		
    			insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')

				-- check QuanLyKHTheochiNhanh
    			--declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))

				declare @QLTheoCN bit = 0;
				declare @countQL int=0;
				select distinct QuanLyKhachHangTheoDonVi into #temp from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)
				set @countQL = (select COUNT(*) from #temp)
				if	@countQL= 1 
						set @QLTheoCN = (select QuanLyKhachHangTheoDonVi from #temp)
    
    			if @QLTheoCN = 1
    				begin									
    					insert into @tblIDNhoms(ID)
    					select *  from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = ',@LoaiKH ,'
    						union all
    						-- get Nhom at this ChiNhanh
    						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) ) tbl
    				end
    			else
    				begin				
    				-- insert all
    				insert into @tblIDNhoms(ID)
    				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
    				where LoaiDoiTuong = ',@LoaiKH, ' 
    				end		
    		end
    	else
    		begin
    			set @idNhomDT = REPLACE( @idNhomDT,''%'','''')
    			insert into @tblIDNhoms(ID) values (@idNhomDT)
    		end
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    	DECLARE @count int;
    	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](''',@MaKH,''', '' '') where Name!='''';
    	Select @count =  (Select count(*) from @tblSearchString);
    	
    	DECLARE @timeChotSo Datetime = (Select NgayChotSo FROM ChotSo where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))')
    
    	declare @sql2 nvarchar(max)= 
    	concat('; WITH Data_CTE ',
    			' AS ',
    			' ( ',
    
    			'SELECT * 
    			FROM
    			(
    			 SELECT 
    			 dt.ID as ID,
    			 dt.MaDoiTuong, 
    				 dt.ID_TrangThai,
    				 case when dt.IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' else  ISNULL(dt.IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong,
    			  dt.TenDoiTuong,
    			  dt.TenDoiTuong_KhongDau,
    			  dt.TenDoiTuong_ChuCaiDau,
    			  dt.GioiTinhNam,
    			  dt.NgaySinh_NgayTLap,
    			  dt.DienThoai,
    			  dt.Email,
    			  dt.DiaChi,
    			  dt.MaSoThue,
    			  ISNULL(dt.GhiChu,'''') as GhiChu,
    			  dt.NgayTao,
    			  dt.NguoiTao,
    			  dt.DinhDang_NgaySinh,
    			  dt.ID_NguonKhach,
    			  dt.ID_NhanVienPhuTrach,
    			  dt.ID_NguoiGioiThieu,
    				  dt.ID_DonVi,
    			  dt.LaCaNhan,
    			  dt.ID_TinhThanh,
    			  dt.ID_QuanHuyen,
    				  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    				  case when right(rtrim(dt.TenNhomDoiTuongs),1) ='','' then LEFT(Rtrim(dt.TenNhomDoiTuongs), len(dt.TenNhomDoiTuongs)-1) else ISNULL(dt.TenNhomDoiTuongs,N''Nhóm mặc định'') end as TenNhomDT,-- remove last coma
    			  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
    			  CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    			  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    			  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    			  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    			  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
    				  CAST(0 as float) as TongNapThe , 
    				  CAST(0 as float) as SuDungThe , 
    				  CAST(0 as float) as HoanTraTheGiaTri , 
    				  CAST(0 as float) as SoDuTheGiaTri , 
    				  concat(dt.MaDoiTuong,'' '',dt.TenDoiTuong,'' '',ISNULL(dt.DienThoai,''''),'' '', ISNULL(dt.TenDoiTuong_KhongDau,''''))  as Name_Phone
    				 FROM DM_DoiTuong dt ','')
    
    declare @sql3 nvarchar(max)= concat(
    				'LEFT JOIN
    			(
    			  SELECT HangHoa.ID_DoiTuong,
    				SUM(ISNULL(HangHoa.NoHienTai, 0)) as NoHienTai, 
    				SUM(ISNULL(HangHoa.TongBan, 0)) as TongBan,
    				SUM(ISNULL(HangHoa.TongBanTruTraHang, 0)) as TongBanTruTraHang,
    				SUM(ISNULL(HangHoa.TongMua, 0)) as TongMua,
    				SUM(ISNULL(HangHoa.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
    					SELECT
    						td.ID_DoiTuong,
    						SUM(ISNULL(td.CongNo, 0)) + SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
    						0 AS TongBan,
    						0 AS TongBanTruTraHang,
    						0 AS TongMua,
    						0 AS SoLanMuaHang
    					FROM
    					(
    					-- Chốt sổ
    						SELECT 
    							ID_KhachHang As ID_DoiTuong,
    							ISNULL(CongNo, 0) AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM ChotSo_KhachHang
    						where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    						UNION ALL
    						-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    						SELECT 
    							bhd.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = 1 OR bhd.LoaiHoaDon = 7 OR bhd.LoaiHoaDon = 19) AND bhd.ChoThanhToan = ''0'' AND bhd.NgayLapHoaDon >= @timeChotSo
    						AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong,
    							0 AS CongNo,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = ''0''  AND bhd.NgayLapHoaDon >= @timeChotSo
    						AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    						GROUP BY bhd.ID_DoiTuong
    						-- sổ quỹ thu
    						UNION ALL
    						SELECT 
    							qhdct.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    							0 AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    							Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo
    							--AND (qhd.HachToanKinhDoanh = 0 or qhd.HachToanKinhDoanh = ''1'')
    						AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID) 
    							AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    						GROUP BY qhdct.ID_DoiTuong
    							-- So Quy chi
    						UNION ALL
    						SELECT 
    							qhdct.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != ''0'' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo
    							--AND (qhd.HachToanKinhDoanh  = 0 or qhd.HachToanKinhDoanh = ''1'')
    						AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID) 
    						GROUP BY qhdct.ID_DoiTuong							
    
    					) AS td
    						GROUP BY td.ID_DoiTuong ','')
    
    declare @sql4 nvarchar(max) = concat(
    						' UNION ALL
    							-- Tổng bán phát sinh trong khoảng thời gian truy vấn
    						SELECT
    							pstv.ID_DoiTuong ,
    							0 AS NoHienTai,
    							SUM(ISNULL(pstv.DoanhThu,0)) AS TongBan,
    							SUM(ISNULL(pstv.DoanhThu,0)) -  SUM(ISNULL(pstv.GiaTriTra,0)) AS TongBanTruTraHang,
    							SUM(ISNULL(pstv.GiaTriTra,0)) AS TongMua,
    							0 AS SoLanMuaHang
    						FROM
    						(
    						SELECT 
    							bhd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = 1 OR bhd.LoaiHoaDon = 7 OR bhd.LoaiHoaDon = 19) AND bhd.ChoThanhToan = ''0'' 
    							AND bhd.NgayLapHoaDon >= ''',@timeStart,''' AND bhd.NgayLapHoaDon < ''',@timeEnd, ''' 
    						AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = ''0''  
    							AND bhd.NgayLapHoaDon >= ''',@timeStart,''' AND bhd.NgayLapHoaDon < ''',@timeEnd,''' 
    						AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    						GROUP BY bhd.ID_DoiTuong
    						) AS pstv
    						GROUP BY pstv.ID_DoiTuong
    							Union All
    							Select 
    								hd.ID_DoiTuong,
    								0 AS NoHienTai,
    								0 AS TongBan,
    								0 AS TongBanTruTraHang,
    								0 AS TongMua,
    								COUNT(*) AS SoLanMuaHang
    							From BH_HoaDon hd 
    							where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    							and hd.ChoThanhToan = 0
    							AND hd.NgayLapHoaDon >= ''',@timeStart,''' AND hd.NgayLapHoaDon < ''',@timeEnd ,''' 
    							And exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    							GROUP BY hd.ID_DoiTuong
    					)AS HangHoa
    						GROUP BY HangHoa.ID_DoiTuong
    				) a
    					on dt.ID = a.ID_DoiTuong 
    						WHERE  dt.loaidoituong =', @loaiKH  ,					
    						' and dt.NgayTao >= ''',@timeStartKH, ''' and dt.NgayTao < ''',@timeEndKH,
    							''' and ( MaDoiTuong like ''%', @MaKH, '%'' OR  TenDoiTuong like ''%',@MaKH, '%'' or TenDoiTuong_KhongDau like ''%',@MaKH, '%'' or DienThoai like ''%',@MaKH, '%'' or Email like ''%',@MaKH, '%'' )',
    							' AND dt.TheoDoi =0 ',
    						  '', @Where, ')b
    				 where b.ID not like ''%00000000-0000-0000-0000-0000%''
    				 and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    				 ),
    
    			Count_CTE ',
    			' AS ',
    			' ( ',
    			' SELECT COUNT(*) AS TotalRow, CEILING(COUNT(*) /  CAST(',@PageSize, ' as float )) as TotalPage FROM Data_CTE ',
    			' ) ',
    			' SELECT dt.*, cte.TotalPage, cte.TotalRow,
    				  ISNULL(trangthai.TenTrangThai,'''') as TrangThaiKhachHang,
    				  ISNULL(qh.TenQuanHuyen,'''') as PhuongXa,
    				  ISNULL(tt.TenTinhThanh,'''') as KhuVuc,
    				  ISNULL(dv.TenDonVi,'''') as ConTy,
    				  ISNULL(dv.SoDienThoai,'''') as DienThoaiChiNhanh,
    				  ISNULL(dv.DiaChi,'''') as DiaChiChiNhanh,
    				  ISNULL(nk.TenNguonKhach,'''') as TenNguonKhach,
    				  ISNULL(dt2.TenDoiTuong,'''') as NguoiGioiThieu',
    			' FROM Data_CTE dt',
    			' CROSS JOIN Count_CTE cte',
    			' LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID ',
    		' LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID ',
    			' LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID ',
    			' LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID ',
    		' LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID ',
    			' LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID ',
    			' ORDER BY ',@SortBy,
    			' OFFSET (', @CurrentPage, ' * ', @PageSize ,') ROWS ',
    			' FETCH NEXT ', @PageSize , ' ROWS ONLY ')
    
    			--print (@sql4)
    			exec (@sql1 + @sql2 + @sql3 + @sql4)
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetDVTKhacInGiaoDich]
    @ID_DonViQuiDoi [uniqueidentifier],
    @ID_DonVi [uniqueidentifier],
	@ThoiGian DATETIME
AS
BEGIN
    --DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
    --	SELECT @ID_HangHoa = ID_HangHoa FROM DonViQuiDoi WHERE ID = @ID_DonViQuiDoi
    
    	DECLARE @TableDVT TABLE (ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, MaHangHoa NVARCHAR(MAX), GiaNhap FLOAT, GiaBanHH FLOAT, GiaVon FLOAT, TonKho FLOAT, ID_LoHang UNIQUEIDENTIFIER, TyLeChuyenDoi FLOAT) 
		INSERT INTO @TableDVT

		Select distinct dvqd.ID as ID_DonViQuiDoi, dvqd.ID_HangHoa, dvqd.MaHangHoa, 
			CAST(ISNULL(dvqd.GiaNhap, 0) AS FLOAT) as GiaNhap, 
			CAST(ISNULL(dvqd.GiaBan,0) AS FLOAT) as GiaBanHH, 
			CAST(ISNULL(gv.GiaVon,0) AS FLOAT) as GiaVon, 
    		0 AS TonKho,
			case when lo.ID is null then null 
			else case when gv.ID_LoHang is not null then gv.ID_LoHang else lo.ID end end as ID_LoHang,
			dvqd.TyLeChuyenDoi
		from DonViQuiDoi dvqd
		left join DM_LoHang lo on dvqd.ID_HangHoa= lo.ID_HangHoa
    	LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_DonVi
    	where dvqd.ID = @ID_DonViQuiDoi
    
    	SELECT ID_DonViQuiDoi, MaHangHoa, GiaNhap, GiaBanHH, GiaVon,(ISNULL([dbo].FUNC_TinhSLTonKhiTaoHD(@ID_DonVi, ID_HangHoa, ID_LoHang, @ThoiGian),0)) / TyLeChuyenDoi AS TonKho, ID_LoHang FROM @TableDVT
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonBanHang_FindMaHang]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max)
AS
BEGIN
    SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.ID_ViTri,
    	c.ID_NhanVien,
		-- when KH was deleted: assign ID_DoiTuong = null
		case when c.TheoDoi ='1' Then null 
		else c.ID_DoiTuong end as ID_DoiTuong,
    	c.TheoDoi,
    	c.ID_DonVi,
    	c.ID_KhuyenMai,
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.MaHoaDonGoc,
		case when c.LoaiHoaDonGoc =6 then c.TongTienHDTra else cast (0 as float) end as TongTienHDTra,
    	c.NgayLapHoaDon,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
		c.NgaySinh_NgayTLap,
    	ISNULL(c.NguoiTaoHD,'') as NguoiTaoHD,
    	c.DiaChiKhachHang,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TenPhongBan,
    	c.TongTienHang,
		c.TongGiamGia, 
		c.TongTienThue,
		-- neu HD DatHang/HD Ban: PhaiTT = PhaiTT
		-- neu HĐoiTra: PhaiTT = PhaiTT- TongHDTra
		CASE WHEN c.LoaiHoaDonGoc = 3 THEN c.PhaiThanhToan
		ELSE
			case when c.PhaiThanhToan < c.TongTienHDTra then 0
			else c.PhaiThanhToan - c.TongTienHDTra end
		END AS PhaiThanhToan,
		c.ThuTuThe, c.TienMat,c.TienATM, c.ChuyenKhoan,c.TienDoiDiem, c.KhachDaTra, c.TongChietKhau,c.TongChiPhi,
    	c.TrangThai,
    	c.KhuyenMai_GhiChu,
    	c.KhuyeMai_GiamGia,
    	c.LoaiHoaDonGoc,
    	c.LoaiHoaDon,
    	c.DiaChiChiNhanh,
    	c.DienThoaiChiNhanh,
    	c.DiemGiaoDich,
    	c.DiemSauGD,
		c.GiaTriSDDV,
		c.GiamGiaCT,
		c.ThanhTienChuaCK,
		ID_TaiKhoanPos,
		ID_TaiKhoanChuyenKhoan,
    	'' as  HoaDon_HangHoa-- string contail all MaHangHoa,TenHangHoa of HoaDon
    	FROM
    	(
    		select 
    		a.ID as ID,
    		--hdXMLOut.HoaDon_HangHoa,
    		bhhd.ID_DoiTuong,
    			-- Neu theo doi = null --> kiem tra neu la khach le --> theodoi = true, nguoc lai = 1
    			CASE 
    				WHEN dt.TheoDoi IS NULL THEN 
    					CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    				ELSE dt.TheoDoi
    			END AS TheoDoi,
    		bhhd.ID_HoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DonVi,
    		bhhd.ChoThanhToan,
    		bhhd.ID_KhuyenMai,
    		bhhd.KhuyenMai_GhiChu,
    		bhhd.LoaiHoaDon,
    		ISNULL(bhhd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(bhhd.DiemGiaoDich,0) AS DiemGiaoDich,
			ISNULL(gb.ID,N'00000000-0000-0000-0000-000000000000') as ID_BangGia,
			ISNULL(vt.ID,N'00000000-0000-0000-0000-000000000000') as ID_ViTri,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		bhhd.MaHoaDon,
    		Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
    		bhhd.NgayLapHoaDon,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
			dt.NgaySinh_NgayTLap,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		bhhd.TongChietKhau,
			ISNULL(bhhd.TongChiPhi,0) as TongChiPhi,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.TongTienThue, 0) as float) as TongTienThue,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
			ISNULL(hdt.PhaiThanhToan,0) - ISNULL(hdt.TongChiPhi,0) as TongTienHDTra, -- tru ChiPhiTra
    		a.ThuTuThe,
    		a.TienMat,
			a.TienATM,
    		a.ChuyenKhoan,
			a.TienDoiDiem,
			a.GiaTriSDDV,
			a.GiamGiaCT,
			a.ThanhTienChuaCK,
			a.KhachDaTra as KhachDaTra,
			--case when bhhd.PhaiThanhToan <= a.KhachDaTra then bhhd.PhaiThanhToan else a.KhachDaTra end as KhachDaTra,-- truong hop XuLyHD, nhung khong HoanTraTamUng
    		ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
    		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai,
			ID_TaiKhoanPos,
			ID_TaiKhoanChuyenKhoan
    		FROM
    		(
    			Select 
    			b.ID,    			
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
				SUM(ISNULL(b.TienATM, 0)) as TienATM,
    			SUM(ISNULL(b.TienCK, 0)) as ChuyenKhoan,
				SUM(ISNULL(b.TienDoiDiem, 0)) as TienDoiDiem,
				SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienThu, 0)) as KhachDaTra,
				SUM(ISNULL(b.GiaTriSDDV, 0)) as GiaTriSDDV,
				SUM(ISNULL(b.GiamGiaCT, 0)) as GiamGiaCT,
				SUM(ISNULL(b.ThanhTien, 0)) as ThanhTienChuaCK,
				max(b.ID_TaiKhoanPOS) as ID_TaiKhoanPos,
				max(b.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan
    			from
    			(
					-- get TongThu from HDMua (thisHD)
    				Select 
    					bhhd.ID,
    					Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,								  	
						-- pos
						case when qhd.TrangThai= 0 then 0
							else case when qhd.LoaiHoaDon = '11' -- thutien
									then case when TaiKhoanPOS = 1 then ISNULL(hdct.TienGui, 0) else 0 end
								else -- chitien
									case when TaiKhoanPOS = 1 then -ISNULL(hdct.TienGui, 0) else 0 end end end as TienATM,
						--- chuyenkhoan
						case when qhd.TrangThai= 0 then 0
							else case when qhd.LoaiHoaDon = '11' -- thutien
									then case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end
								else -- chitien
									case when TaiKhoanPOS = 0 then -ISNULL(hdct.TienGui, 0) else 0 end end end as TienCK,
					
    					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then 
							case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else ISNULL(hdct.TienThu, 0) end
							else case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else -ISNULL(hdct.TienThu, 0) end end end as TienDoiDiem,
    					Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
    					Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu,
						0 AS GiaTriSDDV,
						0 as GiamGiaCT,
						0 as ThanhTien,
						case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 1 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanPos,
						case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 0 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanChuyenKhoan
					
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID  
    				where bhhd.LoaiHoaDon = '1' 
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
    
    				Union all
						-- get TongThu from HDDatHang: chi get hdXuly first
    					select 
							ID,
							TienMat, TienATM,ChuyenKhoan,
							TienDoiDiem, ThuTuThe, TienThu,
							0 AS GiaTriSDDV,
							0 as GiamGiaCT,
							0 as ThanhTien,
							'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
							'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan

						from
						(	
								Select 
										ROW_NUMBER() OVER(PARTITION BY ID_HoaDonLienQuan ORDER BY NgayLapHoaDon ASC) AS isFirst,						
    									d.ID,
										ID_HoaDonLienQuan,
										d.NgayLapHoaDon,
    									sum(d.TienMat) as TienMat,
    									SUM(ISNULL(d.TienATM, 0)) as TienATM,
    									SUM(ISNULL(d.TienCK, 0)) as ChuyenKhoan,
										SUM(ISNULL(d.TienDoiDiem, 0)) as TienDoiDiem,
										sum(d.ThuTuThe) as ThuTuThe,
    									sum(d.TienThu) as TienThu
    								FROM
    								(
									
											select hd.ID, hd.NgayLapHoaDon, qct.ID_HoaDonLienQuan,
												case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(qct.TienMat, 0) else - ISNULL(qct.TienMat, 0) end end as TienMat,
												case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then case when TaiKhoanPOS = 1 then ISNULL(qct.TienGui, 0) else 0 end else ISNULL(qct.TienGui, 0) * (-1) end end as TienATM,							
												case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then case when TaiKhoanPOS = 0 then ISNULL(qct.TienGui, 0) else 0 end else ISNULL(qct.TienGui, 0) * (-1) end end as TienCK,
												case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then 
														case when ISNULL(qct.DiemThanhToan, 0) = 0 then 0 else ISNULL(qct.ThuTuThe, 0) end
													else case when ISNULL(qct.DiemThanhToan, 0) = 0 then 0 else -ISNULL(qct.ThuTuThe, 0) end end end as TienDoiDiem,
												case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(qct.ThuTuThe, 0) else - ISNULL(qct.ThuTuThe, 0) end end as ThuTuThe,
												case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(qct.TienThu, 0) else - ISNULL(qct.TienThu, 0) end end as TienThu
											from Quy_HoaDon_ChiTiet qct
											join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
											left join DM_TaiKhoanNganHang tk on tk.ID= qct.ID_TaiKhoanNganHang						
											left join BH_HoaDon hdd on hdd.ID= qct.ID_HoaDonLienQuan
											left join BH_HoaDon hd on hd.ID_HoaDon= hdd.ID
											where hdd.LoaiHoaDon = '3' 	
											and hd.NgayLapHoadon >= @timeStart and hd.NgayLapHoaDon < @timeEnd and hd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
    								) d group by d.ID,d.NgayLapHoaDon,ID_HoaDonLienQuan						
						) thuDH
						where isFirst= 1

					union all

					-- tong giatri sudung goiudv
					select 
						ctsd.ID_HoaDon as ID,
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,						
						0 as TienThu,
						ctsd.SoLuong * ct.DonGia  as GiaTriSDDV, -- gtri sudung: tinh theo DonGia (chua chiet khau _ Xuyen)
						0 as GiamGiaCT,
						0 as ThanhTien,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan
					from BH_HoaDon_ChiTiet ctsd
					join BH_HoaDon hd on ctsd.ID_HoaDon= hd.ID
					join BH_HoaDon_ChiTiet ct on ctsd.ID_ChiTietGoiDV= ct.ID
					join BH_HoaDon gdv on ct.ID_HoaDon= gdv.ID
					where hd.LoaiHoaDon = 1 and hd.ChoThanhToan = '0' and gdv.LoaiHoaDon = 19
					and hd.NgayLapHoadon >= @timeStart and hd.NgayLapHoaDon < @timeEnd and hd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
					and (ctsd.ID_ChiTietDinhLuong= ctsd.ID or ctsd.ID_ChiTietDinhLuong is null)-- khong lay TPDinhLuong

					union all
					select 
						hd.ID,
						0 as TienMat,
						0 as TienATM,
						0 as ChuyenKhoan,
						0 as TienDoiDiem,
						0 as ThuTuThe,
						0 as TienThu,
						0 as GiaTriSDDV,
						ct.SoLuong * ct.TienChietKhau as GiamGiaCT,
						ct.SoLuong * ct.DonGia  as ThanhTien,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanPos,
						'00000000-0000-0000-0000-000000000000' as ID_TaiKhoanChuyenKhoan
					from BH_HoaDon_ChiTiet ct
					join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
					where hd.LoaiHoaDon = 1 and hd.ChoThanhToan = '0' 
					and hd.NgayLapHoadon >= @timeStart and hd.NgayLapHoaDon < @timeEnd and hd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))
					and (ct.ID_ChiTietDinhLuong= ct.ID or ct.ID_ChiTietDinhLuong is null)
    			) b group by b.ID
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdt on bhhd.ID_HoaDon = hdt.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
    		--left join 
    		--	(Select distinct hdXML.ID, 
    		--			(
    		--			select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
    		--			from BH_HoaDon_ChiTiet ct
    		--			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    		--			join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
    		--			where ct.ID_HoaDon = hdXML.ID
    		--			For XML PATH ('')
    		--		) HoaDon_HangHoa
    		--	from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
    	--WHERE MaHoaDon like @maHD or TenDoiTuong_KhongDau like @maHD or TenDoiTuong_ChuCaiDau like @maHD or DienThoai like @maHD or MaDoiTuong like @maHD or TenDoiTuong like @maHD
    		--OR HoaDon_HangHoa like @maHD
    	ORDER BY c.NgayLapHoaDon DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonDatHang_FindMaHang]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max)
AS
BEGIN
    SELECT 
    	c.ID,
    	c.MaHoaDon,
    	c.LoaiHoaDon,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
    	c.ID_BangGia,
    	c.ID_DonVi,
    	c.YeuCau,
		'' as MaHoaDonGoc,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	ISNULL(c.NguoiTaoHD,'') as NguoiTaoHD,
    	c.DiaChiKhachHang,
		c.NgaySinh_NgayTLap,
    	c.KhuVuc,
    	c.PhuongXa,
    	c.TenDonVi,
		c.DiaChiChiNhanh,
    	c.DienThoaiChiNhanh,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan, 
		c.TienMat,
		c.TienATM,
		c.ChuyenKhoan,
		c.KhachDaTra,c.TongChietKhau,c.TongTienThue, c.ThuTuThe,
    	c.TrangThai,
    	c.TheoDoi,
    	c.TenPhongBan,
    	c.HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon
    
    	FROM
    	(
    		select 
    		a.ID as ID,
    		bhhd.MaHoaDon,
    		hdXMLOut.HoaDon_HangHoa,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DoiTuong,
    		bhhd.ID_BangGia,
    		bhhd.NgayLapHoaDon,
    		bhhd.YeuCau,
    		bhhd.ID_DonVi,
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
			dt.NgaySinh_NgayTLap,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
			CAST(ROUND(bhhd.TongTienThue, 0) as float) as TongTienThue,
    		a.KhachDaTra,
			a.ThuTuThe,
			a.TienMat,
			a.TienATM,
			a.ChuyenKhoan,
    		bhhd.TongChietKhau,

    		Case When bhhd.YeuCau = '1' then N'Phiếu tạm' when bhhd.YeuCau = '3' then N'Hoàn thành' when bhhd.YeuCau = '2' then N'Đang giao hàng' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			select 
    			b.ID,
				SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
				SUM(ISNULL(b.TienMat, 0)) as TienMat,
				SUM(ISNULL(b.TienATM, 0)) as TienATM,
    			SUM(ISNULL(b.TienCK, 0)) as ChuyenKhoan,
    			SUM(ISNULL(b.KhachDaTra, 0)) as KhachDaTra

    			from
    			(
					-- get infor PhieuThu from HDDatHang (HuyPhieuThu (qhd.TrangThai ='0')
    				Select 
    					bhhd.ID,
						Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = 11 then ISNULL(hdct.ThuTuThe, 0) else -ISNULL(hdct.ThuTuThe, 0) end end as ThuTuThe,
						case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(hdct.TienMat, 0) else -ISNULL(hdct.TienMat, 0) end end as TienMat,
						case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then case when TaiKhoanPOS = 1 then ISNULL(hdct.TienGui, 0) else 0 end else -ISNULL(hdct.TienGui, 0) end end as TienATM,							
						case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end else -ISNULL(hdct.TienGui, 0) end end as TienCK,
    					Case when bhhd.ChoThanhToan is null OR qhd.TrangThai='0' then 0 else case when qhd.LoaiHoaDon = 11 then ISNULL(hdct.Tienthu, 0) else -ISNULL(hdct.Tienthu, 0) end end as KhachDaTra					
   				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID 	
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang					
    				where bhhd.LoaiHoaDon = '3'
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))    
    
    				union all
					-- get infor PhieuThu/Chi from HDXuLy
    				Select
    					hdt.ID,
						Case when bhhd.ChoThanhToan is null or qhd.TrangThai='0' then 0 else Case when qhd.LoaiHoaDon = 11 then ISNULL(hdct.ThuTuThe, 0) else -ISNULL(hdct.ThuTuThe, 0) end end as ThuTuThe,		
						Case when bhhd.ChoThanhToan is null or qhd.TrangThai='0' then 0 else Case when qhd.LoaiHoaDon= 11 then ISNULL(hdct.TienMat, 0) else -ISNULL(hdct.TienMat, 0) end end as TienMat,			
						case when bhhd.ChoThanhToan is null or qhd.TrangThai='0' then 0 else case when qhd.LoaiHoaDon = 11 then case when TaiKhoanPOS = 1 then ISNULL(hdct.TienGui, 0) else 0 end else -ISNULL(hdct.TienGui, 0) end end as TienATM,
						case when bhhd.ChoThanhToan is null or qhd.TrangThai='0' then 0 else case when qhd.LoaiHoaDon = 11 then case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end else -ISNULL(hdct.TienGui, 0) end end as TienCK,
  						Case when bhhd.ChoThanhToan is null or qhd.TrangThai='0' then (Case when qhd.LoaiHoaDon = 11 or qhd.TrangThai='0' then 0 else -ISNULL(hdct.Tienthu, 0) end)
    						else (Case when qhd.LoaiHoaDon = 11 then ISNULL(hdct.Tienthu, 0) else -ISNULL(hdct.Tienthu, 0) end) end as KhachDaTra
    				from BH_HoaDon bhhd
    				inner join BH_HoaDon hdt on (bhhd.ID_HoaDon = hdt.ID and hdt.ChoThanhToan = '0')
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan
    				left join Quy_HoaDon qhd on (hdct.ID_HoaDon = qhd.ID)
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang		
    				where hdt.LoaiHoaDon = '3' 
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
    		left join 
    			(Select distinct hdXML.ID, 
    					(
    					select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
    					from BH_HoaDon_ChiTiet ct
    					join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    					join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
    					where ct.ID_HoaDon = hdXML.ID
    					For XML PATH ('')
    				) HoaDon_HangHoa
    			from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
		where c.NgayLapHoadon >= @timeStart and c.NgayLapHoaDon < @timeEnd
    	ORDER BY c.NgayLapHoaDon DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonTraHang_FindMaHang]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @MaPT [nvarchar](max),
    @MaHD [nvarchar](max),
    @TrangThai [nvarchar](max)
AS
BEGIN
    SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.LoaiHoaDon,
    	c.ID_ViTri,
    	c.ID_DonVi,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
    	c.TongTienHDDoiTra,
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.MaHoaDonGoc, 
		c.LoaiHoaDonGoc,
    	--c.MaPhieuChi,
    	c.NgayLapHoaDon,
    	c.TenDoiTuong,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	ISNULL(c.NguoiTaoHD,'') as NguoiTaoHD,
		c.DienThoai,
		c.Email,
		c.DiaChiKhachHang,
		c.NgaySinh_NgayTLap,
    	c.TenDonVi,
    	c.TenNhanVien,
    	c.DienGiai,
    	c.TenBangGia,
    	c.TongTienHang, c.TongGiamGia, 
		case when c.PhaiThanhToan < c.TongTienHDDoiTra then 0
		else c.PhaiThanhToan- c.TongTienHDDoiTra - c.TongChiPhi
		end as PhaiThanhToan,
		--c.PhaiThanhToan as A,
		c.TongChiPhi, c.KhachDaTra,
		c.ThuTuThe,
		c.TienMat,
		c.ChuyenKhoan,
		c.TongChietKhau,c.TongTienThue,
    	c.TrangThai,
    	c.TheoDoi,
    	c.TenPhongBan,
    	c.DienThoaiChiNhanh,
    	c.DiaChiChiNhanh,
    	c.DiemGiaoDich,
    	c.HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	FROM
    	(
    		select 
    		hdXMLOut.HoaDon_HangHoa,
    		a.ID as ID,
    		bhhd.MaHoaDon,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_BangGia,
    		bhhd.ID_HoaDon,
    		bhhd.ID_ViTri,
    		bhhd.ID_DonVi,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DoiTuong,
    		ISNULL(hddt.PhaiThanhToan,0) as TongTienHDDoiTra,
    		ISNULL(bhhd.DiemGiaoDich,0) as DiemGiaoDich,
    		bhhd.ChoThanhToan,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		Case when hdb.MaHoaDon is null then '' else hdb.MaHoaDon end as MaHoaDonGoc,
			ISNULL(hdb.LoaiHoaDon ,1) as LoaiHoaDonGoc,
    		--a.MaPhieuChi,
    		bhhd.NgayLapHoaDon,
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,

			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
			dt.NgaySinh_NgayTLap,
			ISNULL(dt.Email, N'') as Email,
			ISNULL(dt.DienThoai, N'') as DienThoai,
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
    		CAST(ROUND(bhhd.TongChiPhi, 0) as float) as TongChiPhi,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
			CAST(ROUND(bhhd.TongTienThue, 0) as float) as TongTienThue,
    		a.KhachDaTra,
    		a.ThuTuThe,
    		a.TienMat,
    		a.ChuyenKhoan,
    		bhhd.TongChietKhau,
    
    		Case When bhhd.YeuCau = '4' then N'Đã hủy' else N'Hoàn thành' end as TrangThai
    		FROM
    		(
    			select a1.ID, 
					sum(KhachDaTra) as KhachDaTra,
					sum(ThuTuThe) as ThuTuThe,
					sum(TienMat) as TienMat,-- TraHang: khong POS --> TienGui alway = 0
					sum(TienCK) as ChuyenKhoan
				from (
					Select 
    				bhhd.ID,					
					case when qhd.TrangThai ='0' then 0 else ISNULL(hdct.Tienthu, 0) end as KhachDaTra,
					Case when qhd.TrangThai = 0 then 0 else ISNULL(hdct.ThuTuThe, 0) end as ThuTuThe,
					case when qhd.TrangThai = 0 then 0 else ISNULL(hdct.TienMat, 0) end as TienMat,										
					case when qhd.TrangThai = 0 then 0
						else case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end
					end as TienCK					
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang		
    				where bhhd.LoaiHoaDon = '6'
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
					--group by  bhhd.ID, bhhd.MaHoaDon,qhd.TrangThai
				) a1 group by a1.ID
    		) as a
    		left join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join BH_HoaDon hdb on bhhd.ID_HoaDon = hdb.ID
    		left join BH_HoaDon hddt on bhhd.ID = hddt.ID_HoaDon and hddt.ChoThanhToan is not null		
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID
    			left join 
    				(Select distinct hdXML.ID, 
    					 (
    						select qd.MaHangHoa +', '  AS [text()], hh.TenHangHoa +', '  AS [text()]
    						from BH_HoaDon_ChiTiet ct
    						join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    						join DM_HangHoa hh on  hh.ID= qd.ID_HangHoa
    						where ct.ID_HoaDon = hdXML.ID
    						For XML PATH ('')
    					) HoaDon_HangHoa
    				from BH_HoaDon hdXML) hdXMLOut on a.ID= hdXMLOut.ID
    		) as c
    	ORDER BY c.NgayLapHoaDon DESC
END");

            CreateStoredProcedure(name: "[dbo].[GetList_ServicePackages_ByMaGoi]", parametersAction: p => new
            {
                MaGoiDV = p.String()
            }, body: @"SET NOCOUNT ON;
   select  
		hd.ID as ID_GoiDV, MaHoaDon, hd.ID_DoiTuong, dt.MaDoiTuong,
		convert(varchar,hd.NgayLapHoaDon, 103) as NgayLapHoaDon,
    	convert(varchar,hd.NgayApDungGoiDV, 103) as NgayApDungGoiDV,
    	convert(varchar,hd.HanSuDungGoiDV, 103) as HanSuDungGoiDV,
		ctm.ID as ID_ChiTietGoiDV, ctm.ID_DonViQuiDoi, ctm.ID_LoHang, 
		ISNULL(ctm.ID_TangKem, '00000000-0000-0000-0000-000000000000') as ID_TangKem, ISNULL(ctm.TangKem,'0') as TangKem, 
		ctm.DonGia - ctm.TienChietKhau as GiaBan,
		ctm.SoLuong, 
		ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) as SoLuongMua,
		ISNULL(ctt.SoLuongDung,0) as SoLuongDung,
		ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) - ISNULL(ctt.SoLuongDung,0) as SoLuongConLai,		
		qd.TenDonViTinh,qd.ID_HangHoa,qd.MaHangHoa,ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
		hh.LaHangHoa, hh.TenHangHoa, CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
		ISNULL(hh.ID_NhomHang,'00000000-0000-0000-0000-000000000001') as ID_NhomHangHoa,
		ISNULL(hh.SoPhutThucHien,0) as SoPhutThucHien,
		case when hh.LaHangHoa ='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
		Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
		ISNULL(hh.GhiChu,'') as GhiChuHH
	from BH_HoaDon_ChiTiet ctm
	join BH_HoaDon hd on ctm.ID_HoaDon = hd.ID
	join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
	join DonViQuiDoi qd on ctm.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	left join 
	(
		select a.ID_ChiTietGoiDV,
			SUM(a.SoLuongTra) as SoLuongTra,
			SUM(a.SoLuongDung) as SoLuongDung
		from
			(-- sum soluongtra
			select ct.ID_ChiTietGoiDV,
				SUM(ct.SoLuong) as SoLuongTra,
				0 as SoLuongDung
			from BH_HoaDon_ChiTiet ct 
			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
			where hd.ChoThanhToan='0' and hd.LoaiHoaDon = 6
			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
			group by ct.ID_ChiTietGoiDV

			union all
			-- sum soluong sudung
			select ct.ID_ChiTietGoiDV,
				0 as SoLuongDung,
				SUM(ct.SoLuong) as SoLuongDung
			from BH_HoaDon_ChiTiet ct 
			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
			where hd.ChoThanhToan='0' and hd.LoaiHoaDon = 1
			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
			group by ct.ID_ChiTietGoiDV
			) a group by a.ID_ChiTietGoiDV
	) ctt on ctm.ID = ctt.ID_ChiTietGoiDV
	where hd.LoaiHoaDon = 19 and hd.MaHoaDon like '%'+  @MaGoiDV + '%'
	and hd.ChoThanhToan='0'
	order by hd.NgayLapHoaDon desc");

            Sql(@"ALTER PROCEDURE [dbo].[GetMaHoaDon_Copy]
    @MaHoaDon [nvarchar](50)
AS
BEGIN
    SET NOCOUNT ON;	
    	declare @mahoadongoc varchar(50) = 
				(select top 1 SUBSTRING(@MaHoaDon, CHARINDEX(MaHoaDon,@MaHoaDon), len(@MaHoaDon))
				from BH_HoaDon
    			where  CHARINDEX(MaHoaDon,@MaHoaDon) > 0 and MaHoaDon not like '%copy%' 
				order by CHARINDEX(MaHoaDon,@MaHoaDon) -- get ma gan giong nhat
				)				 
				--select @mahoadongoc
    	if CHARINDEX('copy',@MaHoaDon) = 0
    		select @MaHoaDon as MaxCode
    	else
    		begin
			--select @mahoadongoc
    		declare @count int =
    			(select count(ID) from BH_HoaDon
    			where CHARINDEX(@mahoadongoc, MaHoaDon) > 0 and MaHoaDon like '%copy%')
    		select CONCAT('Copy', @count+1 ,'_', @mahoadongoc) as MaxCode
    		end
END");

            Sql(@"ALTER PROCEDURE [dbo].[import_DanhMucHangHoa_Update]
	@ID_HoaDon [uniqueidentifier],
	@SoThuTu [int],
    @LoaiUpdate [int],
    @ID_HangHoa [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @GiaTriTang [nvarchar](max),
    @GiaTriGiam [nvarchar](max),
    @TongTienLech [nvarchar](max),
    @TongChenhLech [nvarchar](max),
    @SoLuongThucTe [nvarchar](max),
    @SoLuongTang [nvarchar](max),
    @SoLuongGiam [nvarchar](max),
    @TenNhomHangHoaCha [nvarchar](max),
    @TenNhomHangHoaCha_KhongDau [nvarchar](max),
    @TenNhomHangHoaCha_KyTuDau [nvarchar](max),
    @MaNhomHangHoaCha [nvarchar](max),
    @timeCreateNHHCha [datetime],
    @TenNhomHangHoa [nvarchar](max),
    @TenNhomHangHoa_KhongDau [nvarchar](max),
    @TenNhomHangHoa_KyTuDau [nvarchar](max),
    @MaNhomHangHoa [nvarchar](max),
    @timeCreateNHH [datetime],
    @LaHangHoa [bit],
    @timeCreateHH [datetime],
    @TenHangHoa [nvarchar](max),
    @TenHangHoa_KhongDau [nvarchar](max),
    @TenHangHoa_KyTuDau [nvarchar](max),
    @GhiChu [nvarchar](max),
    @QuyCach [nvarchar](max),
    @DuocBanTrucTiep [bit],
    @MaDonViCoBan [nvarchar](max),
    @MaHangHoa [nvarchar](max),
    @TenDonViTinh [nvarchar](max),
    @GiaVon [nvarchar](max),
    @GiaBan [nvarchar](max),
    @timeCreateDVQD [datetime],
    @LaDonViChuan [bit],
    @TyLeChuyenDoi [nvarchar](max),
    @MaHoaDon [nvarchar](max),
    @DienGiai [nvarchar](max),
    @TonKho [nvarchar](max),
    @timeCreateHD [datetime],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHangHoaChaCungLoai [nvarchar](max)
AS
BEGIN
	SET NOCOUNT ON;
    -- Khai báo biến update
    	DECLARE @GiaVonF as float
    		set @GiaVonF = (select CAST(ROUND(@GiaVon, 2) as float))
    	DECLARE @GiaBanF as float
    		set @GiaBanF = (select CAST(ROUND(@GiaBan, 2) as float))
    	DECLARE @QuyCachF as float
    		set @QuyCachF = (select CAST(ROUND(@QuyCach, 2) as float))
    	DECLARE @TyLeChuyenDoiF as float
    		set @TyLeChuyenDoiF = (select CAST(ROUND(@TyLeChuyenDoi, 2) as float))
    	DECLARE @TonKhoF as float
    		set @TonKhoF = (select CAST(ROUND(@TonKho, 2) as float))
    
    		DECLARE @GiaTriTangF as float
    		set @GiaTriTangF = (select CAST(ROUND(@GiaTriTang, 0) as float))
    		DECLARE @GiaTriGiamF as float
    		set @GiaTriGiamF = (select CAST(ROUND(@GiaTriGiam, 0) as float))
    		DECLARE @TongTienLechF as float
    		set @TongTienLechF = (select CAST(ROUND(@TongTienLech, 0) as float))
    		DECLARE @TongChenhLechF as float
    		set @TongChenhLechF = (select CAST(ROUND(@TongChenhLech, 2) as float))
    		DECLARE @SoLuongTangF as float
    		set @SoLuongTangF = (select CAST(ROUND(@SoLuongTang, 2) as float))
    		DECLARE @SoLuongGiamF as float
    		set @SoLuongGiamF = (select CAST(ROUND(@SoLuongGiam, 2) as float))
    		DECLARE @SoLuongThucTeF as float
    		set @SoLuongThucTeF = (select CAST(ROUND(@SoLuongThucTe, 2) as float))
    	-- insert NhomHangHoa parent
    DECLARE @ID_NhomHangHoaCha  as uniqueidentifier
    	set @ID_NhomHangHoaCha = null
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin
    		--SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
			SELECT TOP(1) @ID_NhomHangHoaCha = ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha,@TenNhomHangHoaCha_KhongDau, @TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHHCha, null, 0)
    		End
    	End
    -- insert NhomHangHoa
    	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
    	if (len(@TenNhomHangHoa) > 0)
    	Begin
    		--SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
			SELECT TOP(1) @ID_NhomHangHoa = ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @MaNhomHangHoa, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHH, @ID_NhomHangHoaCha, 0)
    		End
    	End
    -- Update HangHoa
    		DECLARE @LaChaCungLoai  as int
    		set @LaChaCungLoai = 1;
    		DECLARE @ID_HangHoaCungLoai  as uniqueidentifier
    	set @ID_HangHoaCungLoai = newID();
    			if(len(@MaHangHoaChaCungLoai) > 0) -- nếu có thông tin mã hàng hóa cùng loại
    			Begin 
    				set @ID_HangHoaCungLoai = (select TOP(1) ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'); -- lấy ID_HangHoaCungLoai có chung tên khác
					--select TOP(1) @ID_HangHoaCungLoai = ID_HangHoaCungLoai FROM DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'
    				if (len(@ID_HangHoaCungLoai) > 0) 
    				Begin
    					set @LaChaCungLoai = 0;
    				End
    				else
    				Begin 
    					set @ID_HangHoaCungLoai = newID(); 
    				End
    			End
    		-- update HangHoa
    	if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    	Begin
    			update DM_HangHoa set ID_HangHoaCungLoai = @ID_HangHoaCungLoai, LaChaCungLoai = @LaChaCungLoai, ID_NhomHang = @ID_NhomHangHoa, LaHangHoa = @LaHangHoa,
    					NgaySua = @timeCreateHH, NguoiSua='admin', TenHangHoa = @TenHangHoa, TenHangHoa_KhongDau = @TenHangHoa_KhongDau, TenHangHoa_KyTuDau = @TenHangHoa_KyTuDau,
    					TenKhac = @MaHangHoaChaCungLoai, GhiChu = @GhiChu, QuyCach = @QuyCachF, DuocBanTrucTiep = @DuocBanTrucTiep Where ID = @ID_HangHoa
    	end
    -- update DonViQuiDoi
    		update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoiF, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBanF, NguoiSua ='admin', NgaySua =@timeCreateDVQD
    			Where ID = @ID_DonViQuiDoi

    -- insert kiểm kê TonKho
    	if (@LoaiUpdate = 2 and @LaHangHoa = 1)
    	Begin
		IF (@GiaVonF = -1000000000)
		BEGIN
			set @GiaVonF = 0;
			set @TongTienLechF = 0;
		END
    	--DECLARE @ID_HoaDon  as uniqueidentifier
    	--set @ID_HoaDon = newID();
    	--	insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    	--		values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
		DECLARE @TonLuyKeHH FLOAT = 0;
		DECLARE @tblIDDonViQuiDoi TABLE(ID UNIQUEIDENTIFIER);
		INSERT INTO @tblIDDonViQuiDoi
		SELECT ID FROM DonViQuiDoi WHERE ID_HangHoa = @ID_HangHoa AND Xoa = 0;

		SELECT TOP(1) @TonLuyKeHH = ISNULL(hdct.TonLuyKe, 0) FROM BH_HoaDon_ChiTiet hdct
		INNER JOIN @tblIDDonViQuiDoi dvqd
		ON hdct.ID_DonViQuiDoi = dvqd.ID
		WHERE hdct.ID_HoaDon = @ID_HoaDon;
			
		SET @TonLuyKeHH = @TonLuyKeHH + @SoLuongThucTeF * @TyLeChuyenDoiF;

		UPDATE hdct
		SET hdct.TonLuyKe = @TonLuyKeHH
		FROM BH_HoaDon_ChiTiet hdct
		INNER JOIN @tblIDDonViQuiDoi dvqd
		ON dvqd.ID = hdct.ID_DonViQuiDoi
		WHERE ID_HoaDon = @ID_HoaDon;

    	    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi)
    			Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@TonLuyKeHH, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi)
    	End
END");

            Sql(@"ALTER PROCEDURE [dbo].[import_DanhMucHangHoaLoHang_Update]
	@ID_HoaDon [uniqueidentifier],
	@SoThuTu [int],
    @LoaiUpdate [int],
    @ID_HangHoa [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @GiaTriTang [nvarchar](max),
    @GiaTriGiam [nvarchar](max),
    @TongTienLech [nvarchar](max),
    @TongChenhLech [nvarchar](max),
    @SoLuongThucTe [nvarchar](max),
    @SoLuongTang [nvarchar](max),
    @SoLuongGiam [nvarchar](max),
    @TenNhomHangHoaCha [nvarchar](max),
    @TenNhomHangHoaCha_KhongDau [nvarchar](max),
    @TenNhomHangHoaCha_KyTuDau [nvarchar](max),
    @MaNhomHangHoaCha [nvarchar](max),
    @timeCreateNHHCha [datetime],
    @TenNhomHangHoa [nvarchar](max),
    @TenNhomHangHoa_KhongDau [nvarchar](max),
    @TenNhomHangHoa_KyTuDau [nvarchar](max),
    @MaNhomHangHoa [nvarchar](max),
    @timeCreateNHH [datetime],
    @LaHangHoa [bit],
    @timeCreateHH [datetime],
    @TenHangHoa [nvarchar](max),
    @TenHangHoa_KhongDau [nvarchar](max),
    @TenHangHoa_KyTuDau [nvarchar](max),
    @GhiChu [nvarchar](max),
    @QuyCach [nvarchar](max),
    @DuocBanTrucTiep [bit],
    @MaDonViCoBan [nvarchar](max),
    @MaHangHoa [nvarchar](max),
    @TenDonViTinh [nvarchar](max),
    @GiaVon [nvarchar](max),
    @GiaBan [nvarchar](max),
    @timeCreateDVQD [datetime],
    @LaDonViChuan [bit],
    @TyLeChuyenDoi [nvarchar](max),
    @MaHoaDon [nvarchar](max),
    @DienGiai [nvarchar](max),
    @TonKho [nvarchar](max),
    @timeCreateHD [datetime],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHangHoaChaCungLoai [nvarchar](max),
    @MaLoHang [nvarchar](max),
    @NgaySanXuat [datetime],
    @NgayHetHan [datetime]
AS
BEGIN
	SET NOCOUNT ON;
    -- Khai báo biến update
    	DECLARE @GiaVonF as float
    		set @GiaVonF = (select CAST(ROUND(@GiaVon, 2) as float))
    	DECLARE @GiaBanF as float
    		set @GiaBanF = (select CAST(ROUND(@GiaBan, 2) as float))
    	DECLARE @QuyCachF as float
    		set @QuyCachF = (select CAST(ROUND(@QuyCach, 2) as float))
    	DECLARE @TyLeChuyenDoiF as float
    		set @TyLeChuyenDoiF = (select CAST(ROUND(@TyLeChuyenDoi, 2) as float))
    	DECLARE @TonKhoF as float
    		set @TonKhoF = (select CAST(ROUND(@TonKho, 2) as float))
    DECLARE @HangHoaNhieuDonViTinh BIT = 0;
	DECLARE @TonLuyKe FLOAT = 0;
	SET @TonLuyKe = @TonKhoF * @TyLeChuyenDoiF;
    		DECLARE @GiaTriTangF as float
    		set @GiaTriTangF = (select CAST(ROUND(@GiaTriTang, 0) as float))
    		DECLARE @GiaTriGiamF as float
    		set @GiaTriGiamF = (select CAST(ROUND(@GiaTriGiam, 0) as float))
    		DECLARE @TongTienLechF as float
    		set @TongTienLechF = (select CAST(ROUND(@TongTienLech, 0) as float))
    		DECLARE @TongChenhLechF as float
    		set @TongChenhLechF = (select CAST(ROUND(@TongChenhLech, 2) as float))
    		DECLARE @SoLuongTangF as float
    		set @SoLuongTangF = (select CAST(ROUND(@SoLuongTang, 2) as float))
    		DECLARE @SoLuongGiamF as float
    		set @SoLuongGiamF = (select CAST(ROUND(@SoLuongGiam, 2) as float))
    		DECLARE @SoLuongThucTeF as float
    		set @SoLuongThucTeF = (select CAST(ROUND(@SoLuongThucTe, 2) as float))
	SET @TonLuyKe = @SoLuongThucTeF * @TyLeChuyenDoiF;
	DECLARE @DieuKien as int
	set @DieuKien = 1;
    	-- insert NhomHangHoa parent
    DECLARE @ID_NhomHangHoaCha  as uniqueidentifier
    	set @ID_NhomHangHoaCha = null
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin
    		--SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
			select TOP(1) @ID_NhomHangHoaCha = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha, @TenNhomHangHoaCha_KhongDau, @TenNhomHangHoaCha_KyTuDau, @MaNhomHangHoaCha, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHHCha, null, 0)
    		End
    	End
    -- insert NhomHangHoa
    	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
    	if (len(@TenNhomHangHoa) > 0)
    	Begin
    		--SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
			SELECT TOP(1) @ID_NhomHangHoa = ID from DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = @LaHangHoa and (TrangThai is NULL or TrangThai = 0)
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa,TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent, TrangThai)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @TenNhomHangHoa_KhongDau, @TenNhomHangHoa_KyTuDau, @MaNhomHangHoa, '1', '1', '1', @LaHangHoa, 'admin', @timeCreateNHH, @ID_NhomHangHoaCha, 0)
    		End
    	End
    -- Update HangHoa
    		DECLARE @LaChaCungLoai  as int
    		set @LaChaCungLoai = 1;
    		DECLARE @ID_HangHoaCungLoai  as uniqueidentifier
    	set @ID_HangHoaCungLoai = newID();
    			if(len(@MaHangHoaChaCungLoai) > 0) -- nếu có thông tin mã hàng hóa cùng loại
    			Begin 
    				set @ID_HangHoaCungLoai = (select TOP(1) ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'); -- lấy ID_HangHoaCungLoai có chung tên khác
					--select TOP(1) @ID_HangHoaCungLoai = ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'
    				if (len(@ID_HangHoaCungLoai) > 0) 
    				Begin
    					set @LaChaCungLoai = 0;
    				End
    				else
    				Begin 
    					set @ID_HangHoaCungLoai = newID(); 
    				End
    			End
    		-- update HangHoa
    	if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    	Begin
    			update DM_HangHoa set ID_HangHoaCungLoai = @ID_HangHoaCungLoai, LaChaCungLoai = @LaChaCungLoai, ID_NhomHang = @ID_NhomHangHoa, LaHangHoa = @LaHangHoa,
    					NgaySua = @timeCreateHH, NguoiSua='admin', TenHangHoa = @TenHangHoa, TenHangHoa_KhongDau = @TenHangHoa_KhongDau, TenHangHoa_KyTuDau = @TenHangHoa_KyTuDau,
    					TenKhac = @MaHangHoaChaCungLoai, GhiChu = @GhiChu, QuyCach = @QuyCachF, DuocBanTrucTiep = @DuocBanTrucTiep Where ID = @ID_HangHoa
    	end
		ELSE
		BEGIN
			SET @HangHoaNhieuDonViTinh = 1;
		END
    		DECLARE @ID_LoHang  as uniqueidentifier
    	set @ID_LoHang = null
    		if(@MaLoHang != '')
    		Begin
    			update DM_HangHoa set QuanLyTheoLoHang = '1' where ID = @ID_HangHoa;
    			--SET @ID_LoHang =  (Select ID FROM DM_LoHang where MaLoHang = @MaLoHang and ID_HangHoa = @ID_HangHoa);
				SELECT TOP(1) @ID_LoHang = ID from DM_LoHang where MaLoHang= @MaLoHang and ID_HangHoa = @ID_HangHoa
    		if (@ID_LoHang is null or len(@ID_LoHang) = 0)
    		BeGin
    			SET @ID_LoHang = newID();
				SET @DieuKien = 0;
    			insert into DM_LoHang(ID, ID_HangHoa, MaLoHang, TenLoHang, NgaySanXuat, NgayHetHan, NguoiTao, NgayTao, TrangThai)
    			values (@ID_LoHang, @ID_HangHoa, @MaLoHang, @MaLoHang, @NgaySanXuat, @NgayHetHan, 'admin', GETDATE(), '1')
				-- update Tồn kho khi thêm mới lô hàng				
				if (@GiaVonF != -1000000000)
				BEGIN
					if (@LaDonViChuan = 1)
						BEGIN
						exec insert_DM_GiaVon @ID_DonViQuiDoi,@ID_DonVi,@ID_LoHang,@GiaVonF, @ID_NhanVien;
						END
					else
					BEGIN
						insert into DM_GiaVon (ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon)
    					Values (NEWID(), @ID_DonViQuiDoi, @ID_DonVi, @ID_LoHang, @GiaVonF)
					END
				END

				else
				BEGIN
					set @GiaVonF = 0;
					set @TongTienLechF = 0;
				END

				--DECLARE @ID_HoaDonLH  as uniqueidentifier
    			--set @ID_HoaDonLH = newID();
    				--insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    				--	values (@ID_HoaDonLH, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    				insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    					Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@SoLuongThucTeF, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
    			End
    		End
    -- update DonViQuiDoi
    	update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoiF, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBanF, NguoiSua ='admin', NgaySua = @timeCreateDVQD
    			Where ID = @ID_DonViQuiDoi
    -- insert kiểm kê TonKho
		
    	if (@LoaiUpdate = 2 and @LaHangHoa = 1 and @DieuKien = 1)
    	Begin
		--DECLARE @ID_HoaDon  as uniqueidentifier
		If (@GiaVonF = -1000000000)
			BEGIN
			IF(@HangHoaNhieuDonViTinh = 1)
			BEGIN
				SELECT @TonLuyKe = @TonLuyKe + ISNULL(SUM(hdct.ThanhTien * dvqd.TyLeChuyenDoi), 0) FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));

				UPDATE hdct
				SET TonLuyKe = @TonLuyKe
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));
			END
			set @GiaVonF = 0;
			set @TongTienLechF = 0;
    		--set @ID_HoaDon = newID();
    			--insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    			--	values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    			insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    				Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@TonLuyKe, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
			END
		Else
			BEGIN
			IF(@HangHoaNhieuDonViTinh = 1)
			BEGIN
				SELECT @TonLuyKe = @TonLuyKe + ISNULL(SUM(hdct.ThanhTien * dvqd.TyLeChuyenDoi), 0) FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));

				UPDATE hdct
				SET TonLuyKe = @TonLuyKe
				FROM BH_HoaDon_ChiTiet hdct
				INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi
				WHERE hdct.ID_HoaDon = @ID_HoaDon AND dvqd.ID_HangHoa = @ID_HangHoa AND (hdct.ID_LoHang = @ID_LoHang OR (hdct.ID_LoHang IS NULL AND @ID_LoHang IS NULL));
			END
    			--set @ID_HoaDon = newID();
    				--insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    				--	values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, dateadd(hour, 7, GETUTCDATE()), @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    				insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien,TonLuyKe, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    					Values (NEWID(), @ID_HoaDon, @SoThuTu, @TongChenhLechF, '0', @SoLuongThucTeF,@TonLuyKe, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
			END
		END
END");


            Sql(@"ALTER PROCEDURE [dbo].[insert_ChietKhauMacDinhNhanVien]
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHH [nvarchar](max),
    @ChietKhau [float],
    @LaPTChietKhau [bit],
    @TuVan [float],
    @LaPTTuVan [bit],
	@YeuCau [float],
	@LaPTYeuCau [bit],
    @Timezone [int],
	@BanGoi [float],
	@LaPTBanGoi [bit],
    @TheoCKThucHien [int]
AS
BEGIN
    DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
    	SELECT @ID_DonViQuiDoi = ID from DonViQuiDoi where MaHangHoa = @MaHH
	DECLARE @ID_ChietKhauMacDinh Uniqueidentifier;
		SELECT @ID_ChietKhauMacDinh = ID from ChietKhauMacDinh_NhanVien 
		where ID_DonViQuiDoi = @ID_DonViQuiDoi and ID_NhanVien = @ID_NhanVien and ID_DonVi= @ID_DonVi
	if (LEN(@ID_ChietKhauMacDinh) > 0)
	BEGIN
		update ChietKhauMacDinh_NhanVien set ChietKhau = @ChietKhau, LaPhanTram = @LaPTChietKhau, ChietKhau_YeuCau = @YeuCau, LaPhanTram_YeuCau = @LaPTYeuCau,
		ChietKhau_TuVan = @TuVan, LaPhanTram_TuVan = @LaPTTuVan, ChietKhau_BanGoi= @BanGoi, LaPhanTram_BanGoi= @LaPTBanGoi, TheoChietKhau_ThucHien= @TheoCKThucHien where ID = @ID_ChietKhauMacDinh
	END
	else
	BEGIN
		insert into ChietKhauMacDinh_NhanVien(ID, ID_NhanVien, ID_DonVi, ChietKhau, LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan, ID_DonViQuiDoi, NgayNhap, ChietKhau_BanGoi, LaPhanTram_BanGoi, TheoChietKhau_ThucHien)
    			values (newID(), @ID_NhanVien, @ID_DonVi, @ChietKhau, @LaPTChietKhau, @YeuCau, @LaPTYeuCau, @TuVan, @LaPTTuVan, @ID_DonViQuiDoi, dateadd(hour,@Timezone,GETUTCDATE()), @BanGoi, @LaPTBanGoi, @TheoCKThucHien)
	END

    
END");


            Sql(@"ALTER PROCEDURE [dbo].[SP_GetList_ServicePackages_Mua]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select hd.MaHoaDon, 
			hd.ID,
    		convert(varchar,hd.NgayLapHoaDon, 103) as NgayLapHoaDon,
			-- TongTienHang = TongMua - TongTra
    		hd.TongTienHang - (ISNULL(hd.TongGiamGia,0)  + ISNULL(hd.KhuyeMai_GiamGia,0))
    		 - (ISNULL(HDTra.TongTienHang,0) - (ISNULL(HDTra.TongGiamGia,0)  + ISNULL(HDTra.KhuyeMai_GiamGia,0))) as TongTienHang, 
			-- PhaiTT of GDV = MuaGoi - TraGoi
    		hd.PhaiThanhToan - ISNULL(HDTra.PhaiThanhToan,0) - ISNULL(hdgoc.PhaiThanhToan,0) as PhaiThanhToan,
			-- chỉ trừ TongTienThu của HDTra nếu khi Trả hàng, mình chi tiền cho khách
			ISNULL(QuyHDMua.TongTienThu,0) - ISNULL(HDTra.TongTienThu,0) as DaThanhToan,  

    		CASE WHEN (HDTra.NoiDungThu  is null OR HDTra.NoiDungThu ='') THEN hd.DienGiai
    		ELSE HDTra.NoiDungThu end GhiChu
    	from BH_HoaDon hd
    	left join 
				-- get TongThu when MuaGoi
				(select qct.ID_HoaDonLienQuan,SUM(qct.TienThu) as TongTienThu,MAX(ISNULL(qhd.NoiDungThu,'')) as NoiDungThu
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				where qhd.TrangThai ='1' or qhd.TrangThai is null
    			group by qct.ID_HoaDonLienQuan) QuyHDMua on hd.ID = QuyHDMua.ID_HoaDonLienQuan
    	left join (
				-- get infor HDTra: TongThu, TongTien
    				select hdt.ID_HoaDon as ID, 
							MAX(ISNULL(QuyHDTra.NoiDungThu,'')) as NoiDungThu,
    						SUM(ISNULL(QuyHDTra.TongTienThu,0)) as TongTienThu,
    						SUM(hdt.TongTienHang) as TongTienHang,
    						SUM(hdt.TongGiamGia) as TongGiamGia ,
    						SUM(hdt.KhuyeMai_GiamGia) as KhuyeMai_GiamGia, 
    						SUM(hdt.PhaiThanhToan) as PhaiThanhToan
    				from BH_HoaDon hd
    				left join BH_HoaDon hdt on hd.ID = hdt.ID_HoaDon
    				left join 
    					(select qct.ID_HoaDonLienQuan,qhd.TongTienThu, MAX(ISNULL(qhd.NoiDungThu,'')) as NoiDungThu 
						from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
						-- get phieu chi chua bi huy
    					where qhd.LoaiHoaDon =12 and (qhd.TrangThai ='1' OR qhd.TrangThai is null)
    					group by qct.ID_HoaDonLienQuan,qct.ID_HoaDon,qhd.MaHoaDon,qhd.TongTienThu) QuyHDTra on hdt.ID = QuyHDTra.ID_HoaDonLienQuan
    				left join BH_HoaDon hddt on hdt.ID_HoaDon= hddt.ID
    				where hdt.LoaiHoaDon =6 and hdt.ChoThanhToan='0'
    				group by hdt.ID_HoaDon) HDTra on hd.ID= HDTra.ID
					
    	left join BH_HoaDon hdgoc on hd.ID_HoaDon = hdgoc.ID
    	where hd.LoaiHoaDon = 19 and hd.ChoThanhToan='0' and hd.ID_DoiTuong like @ID_DoiTuong
		-- get PhieuThu all ChiNhanh
    	order by hd.NgayLapHoaDon desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetThucDonWait]
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select 
    	hd.MaHoaDon ,
    	hd.ID_ViTri,
    	ISNULL(vt.TenViTri,N'Mang về') as TenPhongBan,
    	ct.ID,
		ct.SoLuong,
		ct.ID_DonViQuiDoi,    
		ct.Bep_SoLuongYeuCau,
		ct.Bep_SoLuongHoanThanh,
		ct.Bep_SoLuongChoCungUng,
		ct.ThoiGian ,    
		hh.TenHangHoa ,
		ct.ID_HoaDon,
		ct.GhiChu ,
		ct.TienThue,
    	qd.ID_HangHoa ,
		qd.MaHangHoa
    
    from BH_HoaDon hd
    join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    left join DM_ViTri vt on hd.ID_ViTri= vt.ID
    join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    left join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
    where ISNULL(ct.Bep_SoLuongChoCungUng,0) > 0 and hd.ChoThanhToan = '1'
    and hd.ID_DonVi like @ID_DonVi and hd.LoaiHoaDon=3
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetThucDonYeuCau]
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select 
    	hd.MaHoaDon ,
    	hd.ID_ViTri,
		ISNULL(vt.TenViTri,N'Mang về') as TenPhongBan,
    	ct.ID,
		ct.SoLuong,
		ct.ID_DonViQuiDoi,    
		ct.Bep_SoLuongYeuCau,
		ct.Bep_SoLuongHoanThanh,
		ct.Bep_SoLuongChoCungUng,
		ct.ThoiGian ,    
		hh.TenHangHoa ,
		ct.ID_HoaDon,
		ct.GhiChu ,
		ct.TienThue,
    	qd.ID_HangHoa ,
		qd.MaHangHoa
    
    from BH_HoaDon hd
    join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
    left join DM_ViTri vt on hd.ID_ViTri= vt.ID
    join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    left join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
    where ISNULL(ct.Bep_SoLuongYeuCau,0) > 0 and hd.ChoThanhToan = '1'
    and hd.ID_DonVi like @ID_DonVi and hd.LoaiHoaDon = 3
END");


            Sql(@"update ct set ID_ViTri = hd.ID_ViTri
from BH_HoaDon_ChiTiet ct
join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
where LoaiHoaDon= 3 and hd.ID_ViTri is not null
and ct.ID_ViTri is null");

        }
        
        public override void Down()
        {
            AlterColumn("dbo.NS_MayChamCong", "MatMa", c => c.Int(nullable: false));
            DropColumn("dbo.NS_MayChamCong", "TrangThai");
            //DropColumn("dbo.DM_DonVi", "HanSuDung");
            DropStoredProcedure("[dbo].[SMS_KhachHangGiaoDich]");
            DropStoredProcedure("[dbo].[SMS_KhachHangSinhNhat]");
            DropStoredProcedure("[dbo].[UpdateChiTietKiemKe_WhenEditCTHD]");
            DropStoredProcedure("[dbo].[UpdateGiaVon_WhenEditCTHD]");
            DropStoredProcedure("[dbo].[UpdateIDKhachHang_inSoQuy]");
            DropStoredProcedure("[dbo].[UpdateTonLuyKeCTHD_whenUpdate]");
            DropStoredProcedure("[dbo].[GetList_ServicePackages_ByMaGoi]");
        }
    }
}
