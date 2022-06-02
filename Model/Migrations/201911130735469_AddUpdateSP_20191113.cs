namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20191113 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[DanhMucKhachHang_CongNo_ChotSo_Paging]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                MaKH = p.String(),
                LoaiKH = p.Int(),
                ID_NhomKhachHang = p.String(),
                timeStartKH = p.DateTime(),
                timeEndKH = p.DateTime(),
                CurrentPage = p.Int(),
                PageSize = p.Double(),
                Where = p.String(),
                SortBy = p.String(100)
            }, body: @"SET NOCOUNT ON;
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
					-- check QuanLyKHTheochiNhanh
			declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))
			insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')

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
			exec (@sql1 + @sql2 + @sql3 + @sql4)");

            CreateStoredProcedure(name: "[dbo].[DanhMucKhachHang_CongNo_Paging]", parametersAction: p => new
            {
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                MaKH = p.String(),
                LoaiKH = p.Int(),
                ID_NhomKhachHang = p.String(),
                timeStartKH = p.DateTime(),
                timeEndKH = p.DateTime(),
                CurrentPage = p.Int(),
                PageSize = p.Double(),
                Where = p.String(),
                SortBy = p.String(100)
            }, body: @"set nocount on
	
	if @SortBy ='' set @SortBy = ' dt.NgayTao DESC'
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
					-- check QuanLyKHTheochiNhanh
			declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID))
			insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')

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
	Select @count =  (Select count(*) from @tblSearchString);')

	declare @sql2 nvarchar(max)= concat(' WITH Data_CTE ',
									' AS ',
									' ( ',

    'SELECT  * 
    		FROM
    		(
			SELECT 
    		  dt.ID as ID,
    		  dt.MaDoiTuong, 
			  case when dt.IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' else  ISNULL(dt.IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
    		  dt.TenDoiTuong_KhongDau,
    		  dt.TenDoiTuong_ChuCaiDau,
			  dt.ID_TrangThai,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
			  ISNULL(dt.DienThoai,'''') as DienThoai,
			  ISNULL(dt.Email,'''') as Email,
			  ISNULL(dt.DiaChi,'''') as DiaChi,
			  ISNULL(dt.MaSoThue,'''') as MaSoThue,
    		  ISNULL(dt.GhiChu,'''') as GhiChu,
    		  dt.NgayTao,
    		  dt.DinhDang_NgaySinh,
    		  ISNULL(dt.NguoiTao,'''') as NguoiTao,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
			  dt.ID_DonVi,
    		  dt.LaCaNhan,
    		  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
			  case when right(rtrim(dt.TenNhomDoiTuongs),1) ='','' then LEFT(Rtrim(dt.TenNhomDoiTuongs), len(dt.TenNhomDoiTuongs)-1) else ISNULL(dt.TenNhomDoiTuongs,N''Nhóm mặc định'') end as TenNhomDT,-- remove last coma
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
			  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
			  CAST(0 as float) as TongNapThe , 
			  CAST(0 as float) as SuDungThe , 
			  CAST(0 as float) as HoanTraTheGiaTri , 
			  CAST(0 as float) as SoDuTheGiaTri , 
			  concat(dt.MaDoiTuong,'' '',lower(dt.MaDoiTuong) ,'' '', dt.TenDoiTuong,'' '', dt.DienThoai,'' '', dt.TenDoiTuong_KhongDau)  as Name_Phone			
    		  FROM DM_DoiTuong dt  ','')
	
	declare @sql3 nvarchar(max)= concat('
				LEFT JOIN (
					SELECT tblThuChi.ID_DoiTuong,
					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) - SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
					-- dieu chinh the
							SELECT 
    							bhd.ID_DoiTuong,							
    							0 AS GiaTriTra,
    							ISNULL(bhd.TongTienHang,0) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
								0 AS SoLanMuaHang
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon = 23 AND bhd.ChoThanhToan = ''0''
    						AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
							AND bhd.NgayLapHoaDon >=''', @timeStart ,''' AND bhd.NgayLapHoaDon < ''',@timeEnd,

							''' union all

							-- doanh thu
    						SELECT 
    							bhd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							ISNULL(bhd.PhaiThanhToan,0) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = 1 OR bhd.LoaiHoaDon = 7 OR bhd.LoaiHoaDon = 19 OR bhd.LoaiHoaDon = 22) AND bhd.ChoThanhToan = ''0'' 
							AND bhd.NgayLapHoaDon >=''', @timeStart ,''' AND bhd.NgayLapHoaDon < ''',@timeEnd,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 

							 union all
							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,
    							ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = ''0''  
							AND bhd.NgayLapHoaDon >=''', @timeStart ,''' AND bhd.NgayLapHoaDon < ''',@timeEnd,		
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
							
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
							Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
    						AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID) 
							AND qhd.NgayLapHoaDon >= ''',@timeStart,''' AND qhd.NgayLapHoaDon < ''',@timeEnd  ,
							
							''' union all

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
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							AND qhd.NgayLapHoaDon >= ''',@timeStart,''' AND qhd.NgayLapHoaDon < ''',@timeEnd  ,
    						''' AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)' )

declare @sql4 nvarchar(max)= concat(' Union All
							-- solan mua hang
    						Select 
    							hd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
								0 as TienChi,
    							COUNT(*) AS SoLanMuaHang								
    						From BH_HoaDon hd 
    						where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    						and hd.ChoThanhToan = 0
    						AND hd.NgayLapHoaDon >= ''',@timeStart,''' AND hd.NgayLapHoaDon < ''',@timeEnd ,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
							 GROUP BY hd.ID_DoiTuong  	
							)AS tblThuChi
    						GROUP BY tblThuChi.ID_DoiTuong
    				) a on dt.ID = a.ID_DoiTuong  					
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
			' SELECT COUNT(*) AS TotalRow, CEILING(COUNT(*) / CAST(',@PageSize, ' as float )) as TotalPage FROM Data_CTE ',
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
			exec (@sql1 + @sql2 + @sql3 + @sql4)");

            CreateStoredProcedure(name: "[dbo].[GetHisChargeValueCard]", parametersAction: p => new
            {
                ID_DoiTuong = p.Guid(),
                IDChiNhanhs = p.String()
            }, body: @"SET NOCOUNT ON;
   select *, ISNULL(b.TienThu,0) as KhachDaTra
   from  
	   (select hd.ID, MaHoaDon, LoaiHoaDon, NgayLapHoaDon, 			
			TongChiPhi as MucNap,
			TongChiPhi as ThanhTien,
			ISNULL(TongTienThue,0) as SoDuSauNap ,
			ISNULL(TongChietKhau,0) as KhuyenMaiVND,
			ISNULL(hd.TongChietKhau,0) / ISNULL(hd.TongChiPhi,1) * 100 as KhuyenMaiPT,
			ISNULL(hd.TongGiamGia,0) / hd.TongChiPhi * 100 as ChietKhauPT,
			TongTienHang as TongTienNap,
			TongGiamGia as ChietKhauVND,
			ISNULL(hd.DienGiai,'') as GhiChu,
			hd.NguoiTao,
			PhaiThanhToan, TenDoiTuong as TenKhachHang, DienThoai as SoDienThoai, TenDonVi
		from BH_HoaDon hd
		join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
		join DM_DonVi dv on hd.ID_DonVi= dv.ID
		where LoaiHoaDon in (22,23) and ChoThanhToan='0' and ID_DoiTuong= @ID_DoiTuong
		) a
	left join (select qct.ID_HoaDonLienQuan, sum(qct.TienThu) as TienThu, MAX(qhd.MaHoaDon) as MaPhieuThu
				from Quy_HoaDon_ChiTiet qct 
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				where qct.ID_DoiTuong= @ID_DoiTuong
				and qhd.TrangThai ='1'
				group by qct.ID_HoaDonLienQuan) b
	on a.ID= b.ID_HoaDonLienQuan");

            CreateStoredProcedure(name: "[dbo].[GetListCashFlow]", parametersAction: p => new
            {
                IDDonVis = p.String(),
                ID_NhanVien = p.String(40),
                ID_TaiKhoanNganHang = p.String(40),
                ID_KhoanThuChi = p.String(40),
                DateFrom = p.DateTime(),
                DateTo = p.DateTime(),
                LoaiSoQuy = p.String(2),
                LoaiChungTu = p.String(2),
                TrangThaiSoQuy = p.String(2),
                TrangThaiHachToan = p.String(2),
                TxtSearch = p.String()
            }, body: @"SET NOCOUNT ON;

	select *
	from
		(select 
		qhd.ID,
		qhd.MaHoaDon,
		qhd.NgayLapHoaDon,
		ISNUll(qhd.TrangThai,'1') as TrangThai,
		qhd.LoaiHoaDon,
		qhd.TongTienThu,
		qhd.NoiDungThu,
		qhd.PhieuDieuChinhCongNo,
		cast (0 as float) as DaChi,
		case when qhd.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
		case when qhd.HachToanKinhDoanh = '1' then '11' else '10' end as TrangThaiHachToan,
		case when qhd.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
		qhd.ID_NhanVien,
		nv.TenNhanVien,
		ISNULL(qhdTK2.TKNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
		ISNULL(a.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi,
		a.NguoiNopTien,
		a.MaDoiTuong,
		a.SoDienThoai,
		a.NoiDungThuChi,
		a.TienMat,
		a.TienGui,
		a.TienMat + a.TienGui as TienThu,
		-- 0. tiengui, 1.tienmat , 2. all, 3. conlai
		case when a.TienMat > 0 then case when a.TienGui > 0 then 2 else 1 end else case when a.TienGui > 0 then 0 else 3 end end as LoaiSoQuy,
		case when a.TienMat > 0 then case when a.TienGui > 0 then N'Tiền mặt, chuyển khoản' else N'Tiền mặt' end else case when a.TienGui > 0 then N'Chuyển khoản' else N'Tiền mặt' end end as HinhThuc					
	from Quy_HoaDon qhd
	join (
			select 
				tblQuy.ID_HoaDon,
				tblQuy.ID_KhoanThuChi,
				case when dt.TenDoiTuong is null then nv.TenNhanVien else dt.TenDoiTuong end as NguoiNopTien,
				case when dt.MaDoiTuong is null then nv.MaNhanVien else dt.MaDoiTuong end as MaDoiTuong,
				case when dt.MaDoiTuong is null then nv.DienThoaiDiDong else dt.DienThoai end as SoDienThoai, 
				isnull(ktc.NoiDungThuChi,N'') as NoiDungThuChi,
				TienMat, TienPOS, ChuyenKhoan, TienPOS + ChuyenKhoan as TienGui
			from
				(select 
					a.ID_HoaDon,
					a.ID_KhoanThuChi,
					a.ID_DoiTuong,
					a.ID_NhanVien,
					sum(TienMat) as TienMat,
					sum(ChuyenKhoan) as ChuyenKhoan,
					sum(TienPOS) as TienPOS
				from
					(
					-- tienmat
					select qct.ID_HoaDon,
						qct.ID_KhoanThuChi,
						qct.ID_DoiTuong, 
						qct.ID_NhanVien,
						sum(isnull(qct.TienMat, 0)) as TienMat,
						0 as TienPOS,
						0 as ChuyenKhoan
					 from Quy_HoaDon_ChiTiet qct
					 join Quy_HoaDon qhd on qhd.ID= qct.ID_HoaDon
					 where qhd.NgayLapHoaDon >= @DateFrom and qhd.NgayLapHoaDon < @DateTo
					 group by qct.ID_HoaDon, qct.ID_KhoanThuChi,qct.ID_DoiTuong, qct.ID_NhanVien

					 union all
					 -- pos
					select qct.ID_HoaDon,
						qct.ID_KhoanThuChi,
						qct.ID_DoiTuong, 
						qct.ID_NhanVien,
						0 as TienMat,
						sum(isnull(qct.TienGui, 0)) as POS,
						0 as ChuyenKhoan
					 from Quy_HoaDon_ChiTiet qct
					 join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID
					 join Quy_HoaDon qhd on qhd.ID= qct.ID_HoaDon
					 where qhd.NgayLapHoaDon >= @DateFrom and qhd.NgayLapHoaDon < @DateTo
					 and tk.TaiKhoanPOS='1'
					 group by qct.ID_HoaDon, qct.ID_KhoanThuChi,qct.ID_DoiTuong, qct.ID_NhanVien

					 union all
					 -- chuyenkhoan
					select qct.ID_HoaDon,
						qct.ID_KhoanThuChi,
						qct.ID_DoiTuong, 
						qct.ID_NhanVien,
						0 as TienMat,
						0 as POS,
						sum(isnull(qct.TienGui, 0)) as ChuyenKhoan
					 from Quy_HoaDon_ChiTiet qct
					 join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID
					 join Quy_HoaDon qhd on qhd.ID= qct.ID_HoaDon
					 where qhd.NgayLapHoaDon >= @DateFrom and qhd.NgayLapHoaDon < @DateTo
					 and tk.TaiKhoanPOS='0'
					 group by qct.ID_HoaDon, qct.ID_KhoanThuChi, qct.ID_DoiTuong, qct.ID_NhanVien
					 ) a 	
					 group by a.ID_HoaDon,a.ID_KhoanThuChi, a.ID_DoiTuong, a.ID_NhanVien
				) tblQuy
				left join DM_DoiTuong dt on tblQuy.ID_DoiTuong = dt.ID
				left join NS_NhanVien nv on tblQuy.ID_NhanVien= nv.ID
				left join Quy_KhoanThuChi ktc on tblQuy.ID_KhoanThuChi= ktc.ID				
		) a on qhd.ID= a.ID_HoaDon
	join NS_NhanVien nv on qhd.ID_NhanVien = nv.ID
	left join (
		-- xml ID_TaiKhoanNganHang
		Select qhdTK.ID, qhdTK.TKNganHang	
		From
		(
			Select distinct qhd.ID, 
				(
					Select cast(qct.ID_TaiKhoanNganHang as varchar(40)) + ',' AS [text()]
					From dbo.Quy_HoaDon_ChiTiet qct
					Where qct.ID_HoaDon = qhd.ID and qct.ID_TaiKhoanNganHang is not null
					For XML PATH ('')
				) TKNganHang
			From dbo.Quy_HoaDon qhd where qhd.NgayLapHoaDon >= @DateFrom and NgayLapHoaDon < @DateTo
		) qhdTK
	) qhdTK2 on qhd.ID= qhdTK2.ID
	where qhd.ID_DonVi in (select * from dbo.splitstring(@IDDonVis))	
		) tblView
	where tblView.TrangThaiHachToan like '%'+ @TrangThaiHachToan + '%'
	and tblView.TrangThaiSoQuy like '%'+ @TrangThaiSoQuy + '%'
	and tblView.LoaiChungTu like '%'+ @LoaiChungTu + '%'
	and (tblView.PhieuDieuChinhCongNo ='0' or PhieuDieuChinhCongNo is null)
	and ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang
	and ID_NhanVien like @ID_NhanVien
	and ID_KhoanThuChi like @ID_KhoanThuChi
	and LoaiSoQuy = @LoaiSoQuy
	order by tblView.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[GetListCashFlow_Before]", parametersAction: p => new
            {
                IDDonVis = p.String(),
                ID_NhanVien = p.String(40),
                ID_TaiKhoanNganHang = p.String(40),
                ID_KhoanThuChi = p.String(40),
                DateFrom = p.DateTime(),
                DateTo = p.DateTime(),
                LoaiSoQuy = p.String(2),
                LoaiChungTu = p.String(2),
                TrangThaiSoQuy = p.String(2),
                TrangThaiHachToan = p.String(2),
                TxtSearch = p.String()
            }, body: @"SET NOCOUNT ON;

	select 
		TienMat, TienGui, TienThu
	from
		(select 
		qhd.ID,
		qhd.MaHoaDon,
		qhd.NgayLapHoaDon,
		qhd.TongTienThu,
		qhd.NoiDungThu,
		qhd.PhieuDieuChinhCongNo,
		cast (0 as float) as DaChi,
		case when qhd.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
		case when qhd.HachToanKinhDoanh = '1' then '11' else '10' end as TrangThaiHachToan,
		case when qhd.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
		qhd.ID_NhanVien,
		nv.TenNhanVien,
		ISNULL(qhdTK2.TKNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
		ISNULL(a.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi,
		a.NguoiNopTien,
		a.MaDoiTuong,
		a.TienMat,
		a.TienGui,
		a.TienMat + a.TienGui as TienThu,
		-- 0. tiengui, 1.tienmat , 2. all, 3. conlai
		case when a.TienMat > 0 then case when a.TienGui > 0 then 2 else 1 end else case when a.TienGui > 0 then 0 else 3 end end as LoaiSoQuy
	from Quy_HoaDon qhd
	join (
			select 
				tblQuy.ID_HoaDon,
				tblQuy.ID_KhoanThuChi,
				case when dt.TenDoiTuong is null then nv.TenNhanVien else dt.TenDoiTuong end as NguoiNopTien,
				case when dt.MaDoiTuong is null then nv.MaNhanVien else dt.MaDoiTuong end as MaDoiTuong,
				TienMat, TienPOS, ChuyenKhoan, TienPOS + ChuyenKhoan as TienGui
			from
				(select 
					a.ID_HoaDon,
					a.ID_KhoanThuChi,
					a.ID_DoiTuong,
					a.ID_NhanVien,
					sum(TienMat) as TienMat,
					sum(ChuyenKhoan) as ChuyenKhoan,
					sum(TienPOS) as TienPOS
				from
					(
					-- tienmat
					select qct.ID_HoaDon,
						qct.ID_KhoanThuChi,
						qct.ID_DoiTuong, 
						qct.ID_NhanVien,
						sum(isnull(qct.TienMat, 0)) as TienMat,
						0 as TienPOS,
						0 as ChuyenKhoan
					 from Quy_HoaDon_ChiTiet qct
					 join Quy_HoaDon qhd on qhd.ID= qct.ID_HoaDon
					 where qhd.NgayLapHoaDon < @DateFrom 
					 group by qct.ID_HoaDon, qct.ID_KhoanThuChi,qct.ID_DoiTuong, qct.ID_NhanVien

					 union all
					 -- pos
					select qct.ID_HoaDon,
						qct.ID_KhoanThuChi,
						qct.ID_DoiTuong, 
						qct.ID_NhanVien,
						0 as TienMat,
						sum(isnull(qct.TienGui, 0)) as POS,
						0 as ChuyenKhoan
					 from Quy_HoaDon_ChiTiet qct
					 join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID
					 join Quy_HoaDon qhd on qhd.ID= qct.ID_HoaDon
					 where qhd.NgayLapHoaDon < @DateFrom 
					 and tk.TaiKhoanPOS='1'
					 group by qct.ID_HoaDon, qct.ID_KhoanThuChi,qct.ID_DoiTuong, qct.ID_NhanVien

					 union all
					 -- chuyenkhoan
					select qct.ID_HoaDon,
						qct.ID_KhoanThuChi,
						qct.ID_DoiTuong, 
						qct.ID_NhanVien,
						0 as TienMat,
						0 as POS,
						sum(isnull(qct.TienGui, 0)) as ChuyenKhoan
					 from Quy_HoaDon_ChiTiet qct
					 join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID
					 join Quy_HoaDon qhd on qhd.ID= qct.ID_HoaDon
					 where qhd.NgayLapHoaDon < @DateFrom 
					 and tk.TaiKhoanPOS='0'
					 group by qct.ID_HoaDon, qct.ID_KhoanThuChi, qct.ID_DoiTuong, qct.ID_NhanVien
					 ) a 	
					 group by a.ID_HoaDon,a.ID_KhoanThuChi, a.ID_DoiTuong, a.ID_NhanVien
				) tblQuy
				left join DM_DoiTuong dt on tblQuy.ID_DoiTuong = dt.ID
				left join NS_NhanVien nv on tblQuy.ID_NhanVien= nv.ID
				left join Quy_KhoanThuChi ktc on tblQuy.ID_KhoanThuChi= ktc.ID				
		) a on qhd.ID= a.ID_HoaDon
	join NS_NhanVien nv on qhd.ID_NhanVien = nv.ID
	left join (
		-- xml ID_TaiKhoanNganHang
		Select qhdTK.ID, qhdTK.TKNganHang	
		From
		(
			Select distinct qhd.ID, 
				(
					Select cast(qct.ID_TaiKhoanNganHang as varchar(40)) + ',' AS [text()]
					From dbo.Quy_HoaDon_ChiTiet qct
					Where qct.ID_HoaDon = qhd.ID and qct.ID_TaiKhoanNganHang is not null
					For XML PATH ('')
				) TKNganHang
			From dbo.Quy_HoaDon qhd where qhd.NgayLapHoaDon < @DateFrom 
		) qhdTK
	) qhdTK2 on qhd.ID= qhdTK2.ID
	where qhd.ID_DonVi in (select * from dbo.splitstring(@IDDonVis))	
		) tblView
	where tblView.TrangThaiHachToan like '%'+ @TrangThaiHachToan + '%'
	and tblView.TrangThaiSoQuy like '%'+ @TrangThaiSoQuy + '%'
	and tblView.LoaiChungTu like '%'+ @LoaiChungTu + '%'
	and (tblView.PhieuDieuChinhCongNo ='0' or PhieuDieuChinhCongNo is null)
	and ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang
	and ID_NhanVien like @ID_NhanVien
	and ID_KhoanThuChi like @ID_KhoanThuChi
	and LoaiSoQuy = @LoaiSoQuy
	order by tblView.NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[GetListIDQuiDoi_SetupHoaHongByNhom]", parametersAction: p => new
            {
                ID_NhomHangs = p.String(),
                ID_NhanVien = p.Guid(),
                ID_DonVi = p.Guid()
            }, body: @"set nocount on;
	declare @date datetime = getdate()
    SELECT newid() as ID, @ID_NhanVien as ID_NhanVien , @ID_DonVi as ID_DonVi, 
		cast(0 as float) as ChietKhau, cast(0 as bit) as LaPhanTram, 
		cast(0 as float) as ChietKhau_BanGoi, cast(0 as bit) as LaPhanTram_BanGoi,
		cast(0 as float) as ChietKhau_YeuCau, cast(0 as bit) as LaPhanTram_YeuCau, 0 as TheoChietKhau_ThucHien, 
		cast(0 as float) as ChietKhau_TuVan, cast(0 as bit) as LaPhanTram_TuVan,
		qd.ID as ID_DonViQuiDoi, @date as NgayNhap
	FROM DM_HangHoa hh
    join DonViQuiDoi qd ON hh.iD= qd.ID_HangHoa
    WHERE hh.ID_NhomHang in (Select * from splitstring(@ID_NhomHangs))
	and (qd.Xoa is null OR qd.Xoa='0')
    and not exists
		(select ID_DonViQuiDoi 
		from ChietKhauMacDinh_NhanVien ck 
		where ck.ID_DonViQuiDoi= qd.ID 
		and  ID_NhanVien like @ID_NhanVien  and ID_DonVi like @ID_DonVi)");

            Sql(@"ALTER PROCEDURE [dbo].[getList_ChuongTrinhKhuyenMai]
	@maKM [nvarchar] (max),    
	@TrangThai [nvarchar] (max)
AS
BEGIN
	SELECT 
	km.ID,
	ad.ID_DonVi,
	km.MaKhuyenMai,
	km.TenKhuyenMai,
	km.GhiChu,
	Case when km.TrangThai = 1 then N'Kích hoạt' else N'Chưa áp dụng' end as TrangThai,
	case km.HinhThuc
		when 11 then N'Hóa đơn - Giảm giá hóa đơn'
		when 12 then N'Hóa đơn - Tặng hàng'
		when 13 then N'Hóa đơn - Giảm giá hàng'
		when 14 then N'Hóa đơn - Tặng Điểm'
		when 21 then N'Hàng hóa - Mua hàng giảm giá hàng'
		when 22 then N'Hàng hóa - Mua hàng tặng hàng'
		when 23 then N'Hàng hóa - Mua hàng tặng điểm'
		when 24 then N'Hàng hóa - Mua hàng giảm giá theo số lượng mua'
	end as HinhThuc,
	km.LoaiKhuyenMai,
	km.HinhThuc as KieuHinhThuc,
	km.ThoiGianBatDau,
	km.ThoiGianKetThuc,
	Case when km.NgayApDung = '' then '' else N'Ngày ' + Replace(km.NgayApDung, '_', N', Ngày ') end as NgayApDung,
	Case when km.ThangApDung = '' then '' else N'Tháng ' + Replace(km.ThangApDung, '_', N', Tháng ') end as ThangApDung,
	Replace(Case when km.ThuApDung = '' then '' else N'Thứ ' + Replace(km.ThuApDung, '_', N', Thứ ') end, N'Thứ 8',N'Chủ nhật') as ThuApDung,
	Case when km.GioApDung = '' then '' else Replace(km.GioApDung, '_', N', ') end as GioApDung,
	Case when km.ApDungNgaySinhNhat = 1 then N'Áp dụng vào ngày sinh nhật khách hàng' when km.ApDungNgaySinhNhat = 2 then N'Áp dụng vào tuần sinh nhật khách hàng'
	when km.ApDungNgaySinhNhat = 3 then N'Áp dụng vào tháng sinh nhật khách hàng' else '' end as ApDungNgaySinhNhat,
	km.ApDungNgaySinhNhat as ValueApDungSN,
	km.TatCaDoiTuong,
	km.TatCaDonVi,
	km.TatCaNhanVien,
	km.NguoiTao
	FROM 
	DM_KhuyenMai km
	left join DM_KhuyenMai_ApDung ad on km.ID = ad.ID_KHuyenMai
	where (km.MaKhuyenMai like @maKM or km.TenKhuyenMai like @maKM)
	and km.TrangThai like @TrangThai
	ORDER BY km.NgayTao DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[getList_HoaHongNhanVien_Excel]
    @ID_NhanVien [uniqueidentifier],
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NhomHang_SP [uniqueidentifier],
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_DonVi [uniqueidentifier]
AS
BEGIN
	set nocount on;

			Select 
				a.TenNhomHangHoa,
				a.MaHangHoa,
				a.TenHangHoaFull,
				case when LaPTChietKhau ='0' then CONCAT(ChietKhau, ' vnđ') else  CONCAT(ChietKhau, ' %') end as ChietKhau,
				case when LaPTYeuCau ='0' then CONCAT(YeuCau, ' vnđ') else  CONCAT(YeuCau, ' %') end as YeuCau,
				case when LaPTTuVan ='0' then CONCAT(TuVan, ' vnđ') else  CONCAT(TuVan, ' %') end as TuVan,
				case when LaPTBanGoi ='0' then CONCAT(BanGoi, ' vnđ') else  CONCAT(BanGoi, ' %') end as BanGoi,
				a.GiaBan
			from
		(
    		SELECT ckmd.ID, ckmd.ID_DonViQuiDoi AS IDQuyDoi,
				ckmd.NgayNhap,
				Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    			Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    			Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    			Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    			Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    			dvqd.MaHangHoa,
    			hh.TenHangHoa +
    			Case when (dvqd.ThuocTinhGiaTri is null) then '' else '_' + dvqd.ThuocTinhGiaTri end +
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    			hh.TenHangHoa,
    			hh.TenHangHoa_KhongDau,
    			hh.TenHangHoa_KyTuDau,
				Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    			Case when (dvqd.ThuocTinhGiaTri is null) then '' else '_' + dvqd.ThuocTinhGiaTri end as ThuocTinh_GiaTri,
    			ckmd.ChietKhau,
				ckmd.ChietKhau_YeuCau AS YeuCau,
				ckmd.ChietKhau_TuVan AS TuVan,
				ISNULL(ckmd.ChietKhau_BanGoi,0) AS BanGoi,
				-- if chietkhau = 0, set LaPhanTram = true
				case when ckmd.LaPhanTram= '0' then case when ChietKhau=0 then '1' else '0' end else ckmd.LaPhanTram end AS LaPTChietKhau,
				case when ckmd.LaPhanTram_YeuCau= '0' then case when ChietKhau_YeuCau=0 then '1' else '0' end else ckmd.LaPhanTram_YeuCau end AS LaPTYeuCau,
				case when ckmd.LaPhanTram_TuVan= '0' then case when ChietKhau_TuVan=0 then '1' else '0' end else ckmd.LaPhanTram_TuVan end AS LaPTTuVan,	   		
				case when ckmd.LaPhanTram_BanGoi= '0' then case when ChietKhau_BanGoi=0 then '1' else '0' end else ckmd.LaPhanTram_BanGoi end AS LaPTBanGoi,	   							   		
    			dvqd.GiaBan,
				ISNULL(ckmd.TheoChietKhau_ThucHien,0)  as TheoChietKhau_ThucHien
    		 from ChietKhauMacDinh_NhanVien ckmd
    		left join DonViQuiDoi dvqd on dvqd.ID = ckmd.ID_DonViQuiDoi
    		left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_NhomHangHoa nhh on nhh.ID = hh.ID_NhomHang
    		where ckmd.ID_NhanVien = @ID_NhanVien AND ckmd.ID_DonVi = @ID_DonVi
    		and hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    		) a
			where (a.ID_NhomHang like @ID_NhomHang or a.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP))) 
			and a.Xoa like @TrangThai
    		and (a.MaHangHoa like @MaHH or a.MaHangHoa like @MaHH_TV or a.TenHangHoa_KhongDau like @MaHH or a.TenHangHoa_KyTuDau like @MaHH)
    		ORDER BY a.NgayNhap DESC

END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_CheckExist_ChietKhauHoaDonNhanVien]
    @ID_DonVi [nvarchar](max),
    @ID_NhanViens [nvarchar](max),
    @ChungTuApDung [nvarchar](max),
    @ID_ChietKhauHoaDon [nvarchar](max)
AS
BEGIN
    DECLARE @ID_ChietKhauHoaDonSearch varchar(40) = @ID_ChietKhauHoaDon
    
    	SELECT tbl.ID, tbl.ID_NhanVien, tbl.MaNhanVien, tbl.TenNhanVien, '' as TenNhanVien_GC, '' as TenNhanVien_CV into #temp 
    	FROM
    			(select hd.ID, hd.ChungTuApDung, ct.ID_NhanVien, nv.TenNhanVien, nv.MaNhanVien,    				
    				CASE WHEN hd.ChungTuApDung like '%19%' THEN '19'  ELSE NULL END AS GoiDV,
    				CASE WHEN hd.ChungTuApDung like '%1,%' or hd.ChungTuApDung  like '%,1'  or hd.ChungTuApDung  ='1' THEN '1'  ELSE NULL END AS BanHang,
    				CASE WHEN hd.ChungTuApDung like '%3%' THEN '3'  ELSE NULL END AS DatHang,			
    				CASE WHEN hd.ChungTuApDung like '%6%' THEN '6'  ELSE NULL END AS TraHang,
    				CASE WHEN hd.ChungTuApDung like '%22%' THEN '22'  ELSE NULL END AS TheGiaTri
    			from ChietKhauMacDinh_HoaDon hd
    			join ChietKhauMacDinh_HoaDon_ChiTiet ct on hd.id= ct.ID_ChietKhauHoaDon
    			join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
    			where hd.ID_DonVi like @ID_DonVi
    			and hd.TrangThai !='0'
    			) tbl
    			WHERE
    				(tbl.GoiDV in (select * from splitstring(@ChungTuApDung))
    				OR tbl.BanHang in (select * from splitstring(@ChungTuApDung))
    				OR tbl.DatHang in (select * from splitstring(@ChungTuApDung))
    				OR tbl.TraHang in (select * from splitstring(@ChungTuApDung))
					OR tbl.TheGiaTri in (select * from splitstring(@ChungTuApDung))
					)
    			AND tbl.ID_NhanVien in (select * from splitstring(@ID_NhanViens))
    
    	IF @ID_ChietKhauHoaDonSearch='' OR @ID_ChietKhauHoaDonSearch ='00000000-0000-0000-0000-000000000000'
    		BEGIN
    			SELECT *
    			FROM #temp
    		END
    	ELSE
    		BEGIN
    			SELECT *
    			FROM #temp
    			where #temp.ID NOT LIKE @ID_ChietKhauHoaDon
    		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[XuatFileDanhMucHH]
    @MaHH [nvarchar](max),
    @MaHHCoDau [nvarchar](max),
    @ListID_NhomHang [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @KinhDoanhFilter [nvarchar](max),
    @LaHangHoaFilter [nvarchar](max),
    @List_ThuocTinh [nvarchar](max)
AS
BEGIN
    DECLARE @timeStart Datetime
    DECLARE @SQL VARCHAR(254)
    	DECLARE @tablename TABLE(
    Name [nvarchar](max))
    	DECLARE @tablenameChar TABLE(
    Name [nvarchar](max))
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHHCoDau+',') where Name!='';
    	Select @count =  (Select count(*) from @tablename);
    	Select @countChar =   (Select count(*) from @tablenameChar);

		if(@MaHH = '' and @MaHHCoDau = '')
    --begin @MaHH = '%%'
    BEGIN
		
		if(@List_ThuocTinh != '')
		BEGIN
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang,hhtt.GiaTri + CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.LaChaCungLoai,
			hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, dnhh.TenNhomHangHoa as NhomHangHoa ,
			concat( hh.TenHangHoa , ' ' , dvqd.ThuocTinhGiaTri) as TenHangHoa, dvqd.TenDonViTinh,
					ISNULL(CASE
						WHEN hh.LaHangHoa = 1
						THEN CAST(ISNULL(gv.GiaVon, 0) as float) 
						ELSE
							(select TOP(1) SUM(dinhluong.SoLuong * ISNULL(giavon1.GiaVon,0)) from DinhLuongDichVu dinhluong
								LEFT JOIN DM_GiaVon giavon1 on dinhluong.ID_DonViQuiDoi = giavon1.ID_DonViQuiDoi
								where dinhluong.ID_DichVu = dvqd.ID AND giavon1.ID_DonVi = @ID_ChiNhanh and giavon1.ID_LoHang is null
								GROUP BY dinhluong.ID_DichVu)
					END, 0) as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable1
			from DonViQuiDoi dvqd
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			LEFT JOIN HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
			where dvqd.xoa = 0 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
			and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter


			Select * from #dmhanghoatable1 hhtb2
    		where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
		END
		ELSE
		BEGIN
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang, hh.LaChaCungLoai,
			hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, dnhh.TenNhomHangHoa as NhomHangHoa ,
			concat(hh.TenHangHoa ,' ' , dvqd.ThuocTinhGiaTri) as TenHangHoa, dvqd.TenDonViTinh,
					ISNULL(CASE
						WHEN hh.LaHangHoa = 1
						THEN CAST(ISNULL(gv.GiaVon, 0) as float) 
						ELSE
							(select TOP(1) SUM(dinhluong.SoLuong * ISNULL(giavon1.GiaVon,0)) from DinhLuongDichVu dinhluong
								LEFT JOIN DM_GiaVon giavon1 on dinhluong.ID_DonViQuiDoi = giavon1.ID_DonViQuiDoi
								where dinhluong.ID_DichVu = dvqd.ID AND giavon1.ID_DonVi = @ID_ChiNhanh and giavon1.ID_LoHang is null
								GROUP BY dinhluong.ID_DichVu)
					END, 0) as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable2
			from DonViQuiDoi dvqd
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
			where dvqd.xoa = 0 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
			and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter
			Select * from #dmhanghoatable2 hhtb2
		END

    END
    --end @MaHH = '%%'
    if(@MaHH != '' or @MaHHCoDau != '')
    --begin @MaHH != '%%'
    BEGIN
    	if(@List_ThuocTinh != '')
		BEGIN
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang,hhtt.GiaTri + CAST(hhtt.ID_ThuocTinh AS NVARCHAR(36)) AS ThuocTinh, hh.LaChaCungLoai,
			hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, dnhh.TenNhomHangHoa as NhomHangHoa ,
			concat( hh.TenHangHoa , ' ' , dvqd.ThuocTinhGiaTri) as TenHangHoa, dvqd.TenDonViTinh,
					ISNULL(CASE
						WHEN hh.LaHangHoa = 1
						THEN CAST(ISNULL(gv.GiaVon, 0) as float) 
						ELSE
							(select TOP(1) SUM(dinhluong.SoLuong * ISNULL(giavon1.GiaVon,0)) from DinhLuongDichVu dinhluong
								LEFT JOIN DM_GiaVon giavon1 on dinhluong.ID_DonViQuiDoi = giavon1.ID_DonViQuiDoi
								where dinhluong.ID_DichVu = dvqd.ID AND giavon1.ID_DonVi = @ID_ChiNhanh and giavon1.ID_LoHang is null
								GROUP BY dinhluong.ID_DichVu)
					END, 0) as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable3
			FROM DonViQuiDoi dvqd
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			LEFT JOIN HangHoa_ThuocTinh hhtt on hh.ID = hhtt.ID_HangHoa
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
			
    		where dvqd.xoa = 0 and
    		((select count(*) from @tablename b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
			or hh.TenHangHoa like '%'+b.Name+'%'
			or hh.GhiChu like '%' +b.Name +'%' 
    		or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    		and ((select count(*) from @tablenameChar c where
    		hh.TenHangHoa like '%'+c.Name+'%' or hh.GhiChu like '%'+c.Name+'%'
    		or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    		 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
    		and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter
			Select * from #dmhanghoatable3 hhtb2	
    				where ThuocTinh COLLATE Latin1_General_CI_AI in (select [name] COLLATE Latin1_General_CI_AI from splitstring(@List_ThuocTinh))
		END
		ELSE
		BEGIN
			select dvqd.ID as ID_DonViQuiDoi, hh.ID, hh.TonToiThieu, hh.TonToiDa, hh.GhiChu, hh.LaHangHoa, hh.QuanLyTheoLoHang, hh.LaChaCungLoai,
			hh.DuocBanTrucTiep, hh.TheoDoi as TrangThai, hh.NgayTao, hh.ID_HangHoaCungLoai, dvqd.MaHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, 
			dnhh.TenNhomHangHoa as NhomHangHoa , 
			concat( hh.TenHangHoa , ' ' , dvqd.ThuocTinhGiaTri) as TenHangHoa, dvqd.TenDonViTinh,
					ISNULL(CASE
						WHEN hh.LaHangHoa = 1
						THEN CAST(ISNULL(gv.GiaVon, 0) as float) 
						ELSE
							(select TOP(1) SUM(dinhluong.SoLuong * ISNULL(giavon1.GiaVon,0)) from DinhLuongDichVu dinhluong
								LEFT JOIN DM_GiaVon giavon1 on dinhluong.ID_DonViQuiDoi = giavon1.ID_DonViQuiDoi
								where dinhluong.ID_DichVu = dvqd.ID AND giavon1.ID_DonVi = @ID_ChiNhanh and giavon1.ID_LoHang is null
								GROUP BY dinhluong.ID_DichVu)
					END, 0) as GiaVon, dvqd.GiaBan, ISNULL(hhtonkho.TonKho,0) as TonKho into #dmhanghoatable4
			FROM DonViQuiDoi dvqd
			LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi and hhtonkho.ID_DonVi = @ID_ChiNhanh 
			LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = hh.ID_NhomHang
    		LEFT JOIN DM_GiaVon gv on dvqd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi = @ID_ChiNhanh and gv.ID_LoHang is null
    		where dvqd.xoa = 0 and
    		((select count(*) from @tablename b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
			or hh.TenHangHoa like '%'+b.Name+'%'
			or hh.GhiChu like '%' +b.Name +'%' 
    		or dvqd.MaHangHoa like '%'+b.Name+'%' )=@count or @count=0)
    		and ((select count(*) from @tablenameChar c where
    		hh.TenHangHoa like '%'+c.Name+'%' or hh.GhiChu like '%'+c.Name+'%'
    		or dvqd.MaHangHoa like '%'+c.Name+'%' )= @countChar or @countChar=0)
    		 and ((select TOP 1 [name] from splitstring(@ListID_NhomHang) ORDER BY [name]) = '' or dnhh.id=(select * from splitstring(@ListID_NhomHang) where [name] like dnhh.ID))
    		and hh.TheoDoi like @KinhDoanhFilter and hh.LaHangHoa like @LaHangHoaFilter
			Select * from #dmhanghoatable4 hhtb2	
		END
	END
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[DanhMucKhachHang_CongNo_ChotSo_Paging]");
            DropStoredProcedure("[dbo].[DanhMucKhachHang_CongNo_Paging]");
            DropStoredProcedure("[dbo].[GetHisChargeValueCard]");
            DropStoredProcedure("[dbo].[GetListCashFlow]");
            DropStoredProcedure("[dbo].[GetListCashFlow_Before]");
            DropStoredProcedure("[dbo].[GetListIDQuiDoi_SetupHoaHongByNhom]");
        }
    }
}
