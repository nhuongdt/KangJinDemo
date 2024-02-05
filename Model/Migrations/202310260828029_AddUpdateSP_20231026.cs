namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20231026 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[GetInforKhachHang_ByID]", parametersAction: p => new
            {
                ID_DoiTuong = p.Guid(),
                ID_ChiNhanh = p.String(),
                timeStart = p.String(),
                timeEnd = p.String()
            }, body: @"SET NOCOUNT ON;
    SELECT 
    			  dt.ID as ID,
    			  dt.MaDoiTuong, 
				  dt.LoaiDoiTuong,
    			  case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as ID_NhomDoiTuong,
    			  dt.TenDoiTuong,
    			  dt.TenDoiTuong_KhongDau,
    			  dt.TenDoiTuong_ChuCaiDau,
    			  dt.ID_TrangThai,
    			  dt.GioiTinhNam,
    			  dt.NgaySinh_NgayTLap,
    			  ISNULL(dt.DienThoai,'') as DienThoai,
    			  ISNULL(dt.Email,'') as Email,
    			  ISNULL(dt.DiaChi,'') as DiaChi,
    			  ISNULL(dt.MaSoThue,'') as MaSoThue,
    			  ISNULL(dt.GhiChu,'') as GhiChu,
				  dt.TaiKhoanNganHang,
    			  dt.NgayTao,
    			  dt.DinhDang_NgaySinh,
    			  ISNULL(dt.NguoiTao,'') as NguoiTao,
    			  dt.ID_NguonKhach,
    			  dt.ID_NhanVienPhuTrach,
    			  dt.ID_NguoiGioiThieu,
    			  dt.ID_DonVi,
    			  dt.LaCaNhan,
    			  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
				  dt.TenNhomDoiTuongs as TenNhomDT,    			 
    			  dt.ID_TinhThanh,
    			  dt.ID_QuanHuyen,
				  dt.TheoDoi,
				  dt.NgayGiaoDichGanNhat,
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
				  ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
    			  concat(dt.MaDoiTuong,' ',lower(dt.MaDoiTuong) ,' ', dt.TenDoiTuong,' ', dt.DienThoai,' ', dt.TenDoiTuong_KhongDau)  as Name_Phone			
    			  FROM DM_DoiTuong dt
				  left join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
				  LEFT JOIN (
    					SELECT tblThuChi.ID_DoiTuong,   						
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) + SUM(ISNULL(tblThuChi.HoanTraSoDuTGT,0)) +
						+ SUM(ISNULL(tblThuChi.ThuTuThe,0))
						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
					sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    					FROM
    					(
						-- doanhthu
							SELECT 
    							bhd.ID_DoiTuong as ID_DoiTuong,
    							0 AS GiaTriTra,    							
								bhd.PhaiThanhToan as DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,25) 
							AND bhd.ChoThanhToan = 0 
							and bhd.ID_DoiTuong = @ID_DoiTuong


							union all

							SELECT 
    							bhd.ID_DoiTuong as ID_DoiTuong,
    							0 AS GiaTriTra,    							
								0 as DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								iif(bhd.LoaiHoaDon=22, bhd.PhaiThanhToan, - bhd.PhaiThanhToan) as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (22,42) 
							AND bhd.ChoThanhToan = 0 
							and bhd.ID_DoiTuong = @ID_DoiTuong

    						
    						 union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,    							
								bhd.PhaiThanhToan  AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) 
							AND bhd.ChoThanhToan = 0 
							and bhd.ID_DoiTuong = @ID_DoiTuong						
    							
    						union all
    
    							-- tienthu
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon    						
    						WHERE qhd.LoaiHoaDon = 11 
							and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
							and qhdct.ID_DoiTuong = @ID_DoiTuong
							and qhdct.HinhThucThanhToan!=6
							and not exists(select ID from BH_HoaDon pthh where qhdct.ID_HoaDonLienQuan = pthh.ID and pthh.LoaiHoaDon= 41)

    							
    						 union all    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 12 
							AND (qhd.TrangThai != 0 OR qhd.TrangThai is null)
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
							and qhdct.HinhThucThanhToan!=6
							and qhdct.ID_DoiTuong = @ID_DoiTuong
							and not exists(select ID from BH_HoaDon pthh where qhdct.ID_HoaDonLienQuan = pthh.ID and pthh.LoaiHoaDon= 41)

							union all
							---- hoantra sodu TGT cho khach (giam sodu TGT)
						SELECT 
    							bhd.ID_DoiTuong,    	
								0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								-sum(bhd.PhaiThanhToan) as HoanTraSoDuTGT
    					FROM BH_HoaDon bhd
						where bhd.LoaiHoaDon = 32 and bhd.ChoThanhToan = 0 	
						group by bhd.ID_DoiTuong
    				)AS tblThuChi GROUP BY tblThuChi.ID_DoiTuong   					
    		) a on dt.ID = a.ID_DoiTuong
			where dt.ID= @ID_DoiTuong");

            CreateStoredProcedure(name: "[dbo].[BaoCao_DoanhThuKhachHang]", parametersAction: p => new
            {
                IDChiNhanhs = p.String(),
                DateFrom = p.DateTime(),
                DateTo = p.DateTime(),
                TextSearch = p.String(),
                CurrentPage = p.Int(),
                PageSize = p.Int()
            }, body: @"SET NOCOUNT ON;

	if @CurrentPage > 0 set @CurrentPage = @CurrentPage - 1;

	if(isnull(@TextSearch,'')='') set @TextSearch =''
	else set @TextSearch= CONCAT(N'%',@TextSearch,'%')

	declare @tblChiNhanh table (ID uniqueidentifier)
    insert into @tblChiNhanh
    select Name from dbo.splitstring(@IDChiNhanhs) where Name!=''

	; with data_cte
	as
	(
		select 
			dt.ID as ID_DoiTuong,
			dt.MaDoiTuong,
			dt.TenDoiTuong,
			dt.DienThoai as DienThoaiKhachHang,
			tblThuChi.NgayThanhToan,
			isnull(tblThuChi.ThuHoaDon,0) as ThuHoaDon,
			isnull(tblThuChi.ChiHoaDon,0) as ChiHoaDon,
			isnull(tblThuChi.HoanCoc,0) as HoanCoc,
			isnull(tblThuChi.HoanDichVu,0) as HoanDichVu
		from DM_DoiTuong dt
		left join
		(

		select 
			tblSoQuy.ID_DoiTuong,
			tblSoQuy.NgayThanhToan,
			sum(tblSoQuy.ThuHoaDon) as ThuHoaDon,
			sum(tblSoQuy.ChiHoaDon) as ChiHoaDon,
			sum(tblSoQuy.HoanCoc) as HoanCoc,
			sum(tblSoQuy.HoanDichVu) as HoanDichVu
		from
		(
		select 		
			qct.ID_DoiTuong,
			format(qhd.NgayLapHoaDon,'yyyy-MM-dd') as NgayThanhToan, 
			iif(hd.LoaiHoaDon = 32, iif(qhd.LoaiHoaDon= 12, qct.TienThu,0),0) as HoanCoc, --- chi lay phieuchi tu HTTGT
			iif(qhd.LoaiHoaDon= 11, qct.TienThu,0) as ThuHoaDon, ---- baogom thu TGT
			iif(qhd.LoaiHoaDon= 12 and hd.LoaiHoaDon != 32, qct.TienThu,0) as ChiHoaDon, ---- khong bao baogom chi hoantra TGT
			0 as HoanDichVu
		from DM_DoiTuong dt
		join BH_HoaDon hd on dt.ID = hd.ID_DoiTuong
		join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID	
		where dt.LoaiDoiTuong=1
		and dt.TheoDoi ='0'
		and (qhd.TrangThai='1' or qhd.TrangThai is null)
		and exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)

		union all

		----- hoandichvu: phieuchi khong lienquan hoadon ----
		select 
			qct.ID_DoiTuong,
			format(qhd.NgayLapHoaDon,'yyyy-MM-dd') as NgayThanhToan, 
			0 as HoanCoc,
			0 as ThuHoaDon,
			0 as ChiHoaDon,
			qct.TienThu as HoanDichVu
		from Quy_HoaDon qhd 
		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
		where (qhd.TrangThai='1' or qhd.TrangThai is null)
		and qct.ID_HoaDonLienQuan is null
		and qhd.LoaiHoaDon= 12			
		and exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)
	) tblSoQuy
	group by tblSoQuy.ID_DoiTuong, tblSoQuy.NgayThanhToan
	) tblThuChi  on dt.ID= tblThuChi.ID_DoiTuong
	where NgayThanhToan >= @DateFrom and NgayThanhToan < @DateTo
	and (@TextSearch='' or
		dt.MaDoiTuong like @TextSearch 
		or dt.TenDoiTuong like @TextSearch
		or dt.TenDoiTuong_KhongDau like @TextSearch 
		or dt.DienThoai like @TextSearch)
	),
	tblSum
	as
	(
		select 
			count(ID_DoiTuong) as TotalRow,
			sum(ThuHoaDon) as SumTongThanhToan,
			sum(HoanCoc) as SumHoanCoc,
			sum(HoanDichVu) as SumHoanDichVu
		from data_cte
	)
   select *, 
	ThuHoaDon as TongThanhToan
   from data_cte dt
  cross join tblSum tbgr 
  ORDER BY dt.NgayThanhToan desc
  offset @CurrentPage * @PageSize ROWS
  fetch next @PageSize ROWS only");

			CreateStoredProcedure(name: "[dbo].[BCBanHang_theoMaDinhDanh]", parametersAction: p => new
			{
				pageNumber = p.Int(),
				pageSize = p.Int(),
				SearchString = p.String(),
				timeStart = p.DateTime(),
				timeEnd = p.DateTime(),
				ID_ChiNhanh = p.String(),
				LoaiHangHoa = p.String(),
				TheoDoi = p.String(),
				TrangThai = p.String(),
				ID_NhomHang = p.Guid(),
				LoaiChungTu = p.String()
			}, body: @"SET NOCOUNT ON;

	set @pageNumber = @pageNumber -1;

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER)
	INSERT INTO @tblChiNhanh
	select Name from splitstring(@ID_ChiNhanh);

	DECLARE @tblLoaiHoaDon TABLE(LoaiHoaDon int)
    INSERT INTO @tblLoaiHoaDon
    select Name from splitstring(@LoaiChungTu);


	----- get cthd hotro ---
	select 
		hd.NgayLapHoaDon,
		hd.MaHoaDon,
		hd.LoaiHoaDon,
		hd.TongGiamGia, ---- songaythuoc ---	
		hd.ID_DoiTuong,
		hd.ID_CheckIn as IdNhom_ApdungHotro,
		ct.ID,
		ct.ID_HoaDon,
		ct.ID_DonViQuiDoi,
		ct.SoLuong,
		ct.DonGia,
		ct.TienChietKhau, 
		ct.ThanhTien,
		ct.GhiChu,
		hd.DienGiai,
		0 as TienThue,
		0 as GiamGiaHD
	into #hdHoTro
	from BH_HoaDon hd
	join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
	where hd.ChoThanhToan= 0 and hd.LoaiHoaDon= 36	
	and ct.ChatLieu='6'
	and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
	and hd.NgayLapHoaDon between @timeStart and @timeEnd
    and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)
	and exists (select LoaiHoaDon from @tblLoaiHoaDon loai where loai.LoaiHoaDon = hd.LoaiHoaDon)

	
	
	
	; with data_cte
	as
	(

	select tblLast.*
	from
	(
		------ get nhom apdung hotro ---
		select 
			null as ID, ---- chỉ có tác dụng để union ----
			hd.ID_HoaDon,
			hd.LoaiHoaDon,
			hd.MaHoaDon,
			hd.NgayLapHoaDon,
			dt.MaDoiTuong,
			dt.TenDoiTuong,
			dt.TenDoiTuong_KhongDau,
			dt.DienThoai,
			concat(nhom.TenNhomHangHoa, N' - ngày thuốc') as MaHangHoa,
			concat(nhom.TenNhomHangHoa, N' - ngày thuốc') as TenHangHoa,
			concat(nhom.TenNhomHangHoa, N' - ngày thuốc') as TenHangHoa_KhongDau,
			nhom.ID as ID_NhomHang,
			nhom.TenNhomHangHoa,
			hd.TongGiamGia as SoLuong,
			0 as DonGia,
			0 as TienChietKhau,
			0 as ThanhTien,
			hd.DienGiai as GhiChu,
			hd.MaHoaDon as MaDinhDanh,
			2 as LoaiHangHoa
		from (
			select hd.ID_HoaDon,
				hd.LoaiHoaDon,
				hd.IdNhom_ApdungHotro,
				hd.ID_DoiTuong,
				hd.MaHoaDon,
				hd.NgayLapHoaDon,
				hd.TongGiamGia,
				hd.DienGiai
			from  #hdHoTro hd
			where exists (select * from BH_ChiTiet_DinhDanh dd where hd.ID= dd.IdHoaDonChiTiet)
			group by hd.ID_HoaDon,
				hd.LoaiHoaDon,
				hd.IdNhom_ApdungHotro,
				hd.ID_DoiTuong,
				hd.MaHoaDon,
				hd.NgayLapHoaDon,
				hd.TongGiamGia,
				hd.DienGiai
		) hd
		join DM_NhomHangHoa nhom on hd.IdNhom_ApdungHotro = nhom.ID
		left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
	

		union all
	
		------ tblUnion thong tin chitiet ---
		select 
			tblUnion.ID, --- used to get NVThucHien ----
			tblUnion.ID_HoaDon,
			tblUnion.LoaiHoaDon,
			tblUnion.MaHoaDon,
			tblUnion.NgayLapHoaDon,
			dt.MaDoiTuong,
			dt.TenDoiTuong,
			dt.TenDoiTuong_KhongDau,
			dt.DienThoai,
			qd.MaHangHoa,
			hh.TenHangHoa,	
			hh.TenHangHoa_KhongDau,
			hh.ID_NhomHang,
			nhom.TenNhomHangHoa,
			tblUnion.SoLuong,
			tblUnion.DonGia,
			tblUnion.TienChietKhau,
			tblUnion.ThanhTien,
			tblUnion.GhiChu,
			cast(isnull(dinhdanh.MaDinhDanh,0) as varchar(max)) as MaDinhDanh,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa
		from
		(
			----- sp hotro ngaythuoc ----
			select 
				hd.NgayLapHoaDon,
				hd.MaHoaDon,
				hd.LoaiHoaDon,
				hd.TongGiamGia, ---- songaythuoc ---	
				hd.ID_DoiTuong,		
				hd.ID,
				hd.ID_HoaDon,
				hd.ID_DonViQuiDoi,
				hd.SoLuong,
				hd.DonGia,
				0 as TienChietKhau, 
				hd.ThanhTien,
				hd.GhiChu,
				hd.DienGiai,
				0 as TienThue,
				0 as GiamGiaHD
			from #hdHoTro hd

	
			union all

			---- get cthd (hdle, baohanh) --
			select 
    				hd.NgayLapHoaDon,
					hd.MaHoaDon,
					hd.LoaiHoaDon,    	
					hd.TongGiamGia,   	
					hd.ID_DoiTuong,    			
    				ct.ID,
					ct.ID_HoaDon,
					ct.ID_DonViQuiDoi,     		
    				ct.SoLuong,
					ct.DonGia,
    				ct.TienChietKhau,		
    				ct.ThanhTien, 
    				ct.GhiChu,    	
					hd.DienGiai,
    				ct.TienThue * ct.SoLuong  as TienThue,
    				Case when hd.TongTienHang = 0 then 0 else ct.ThanhTien * ((hd.TongGiamGia + isnull(hd.KhuyeMai_GiamGia,0)) / hd.TongTienHang) end as GiamGiaHD
    			from BH_HoaDon_ChiTiet ct
    			join BH_HoaDon hd on ct.ID_HoaDon = hd.ID	
			where hd.ChoThanhToan=0 and hd.LoaiHoaDon !=36
			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
			and hd.NgayLapHoaDon between @timeStart and @timeEnd
			and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)
			and exists (select LoaiHoaDon from @tblLoaiHoaDon loai where loai.LoaiHoaDon = hd.LoaiHoaDon)
		)tblUnion	
	join DonViQuiDoi qd on tblUnion.ID_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	join BH_ChiTiet_DinhDanh dinhdanh on tblUnion.ID = dinhdanh.IdHoaDonChiTiet or tblUnion.ID is null  ---- Chỉ lấy hóa đơn có mã định danh từ khi bắt đầu làm tính năng mới này ---
	left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
	left join DM_DoiTuong dt on tblUnion.ID_DoiTuong = dt.ID	
	where hh.TheoDoi like @TheoDoi		
	) tblLast
	where 	
		(@LoaiHangHoa ='%%' or tblLast.LoaiHangHoa in (select name from dbo.splitstring(@LoaiHangHoa)))	
		and 
		(@ID_NhomHang is null or exists  (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where tblLast.ID_NhomHang = allnhh.ID))
		AND
		((select count(Name) from @tblSearchString b where 
				tblLast.MaHoaDon like '%'+b.Name+'%' 
    			or tblLast.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or tblLast.TenHangHoa like '%'+b.Name+'%'
    			or tblLast.MaHangHoa like '%'+b.Name+'%'
    			or tblLast.TenNhomHangHoa like '%'+b.Name+'%'
				or tblLast.TenDoiTuong like '%'+b.Name+'%'
    			or tblLast.TenDoiTuong_KhongDau  like '%'+b.Name+'%'
				or tblLast.MaDoiTuong like '%'+b.Name+'%'
    			or tblLast.DienThoai  like '%'+b.Name+'%'
				or tblLast.GhiChu like N'%'+b.Name+'%'
    			)=@count or @count=0)
	),
	count_cte
	as
	(
		select  count (*)  as TotalRow,
			CEILING(count (*)/ cast (@pageSize as float)) as TotalPage
		from data_cte
	),	
	tView
	as
	(
		----- get row from - to ---
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@pageNumber* @pageSize) ROWS
		FETCH NEXT @pageSize ROWS ONLY
	) 	
    select tbl.*,
		isnull(maNV.NVThucHien,'') as MaNhanVien,
		isnull(tenNV.NVThucHien,'') as TenNhanVien	
	from tView tbl
	left join
	(
	-- get TenNV thuchien of cthd
	select distinct tblCT.ID as ID_ChiTietHD ,
			(
				select nv.TenNhanVien +', '  AS [text()]
				from BH_NhanVienThucHien nvth
				join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
				where nvth.ID_ChiTietHoaDon = tblCT.Id										
				For XML PATH ('')
			) NVThucHien
		from tView tblCT 
	) tenNV on tbl.ID = tenNV.ID_ChiTietHD
	left join
	(
	-- get MaNV nvthuchien of cthd
	select distinct tblCT.ID as ID_ChiTietHD ,
			(
				select nv.MaNhanVien +', '  AS [text()]
				from BH_NhanVienThucHien nvth
				join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
				where nvth.ID_ChiTietHoaDon = tblCT.ID										
				For XML PATH ('')
			) NVThucHien
		from tView tblCT 
	) maNV on tbl.ID = maNV.ID_ChiTietHD

	drop table #hdHoTro");

			Sql(@"CREATE FUNCTION [dbo].[BuTruTraHang_HDDoi]
(
	@ID_HoaDon uniqueidentifier,
	@NgayLapHoaDon datetime,
	@ID_HoaDonGoc uniqueidentifier = null,
	@LoaiHDGoc int =  0	
)
RETURNS float
AS
BEGIN

			
	DECLARE @Gtri float=0


	----- neu gtributru > 0 --> khong butru
	set @Gtri = (		
				select 
					sum(isnull(PhaiThanhToan,0) + isnull(DaThanhToan,0)) as BuTruTra
				from
				(
				select 		
					hd.ID,					
					iif(hd.LoaiHoaDon =6, -hd.PhaiThanhToan, hd.PhaiThanhToan)  -- 0  as PhaiThanhToan
						+ isnull((select dbo.BuTruTraHang_HDDoi(hd.ID_HoaDon, hd.NgayLapHoaDon, hdgoc.ID_HoaDon, hdgoc.LoaiHoaDon)),0) --- hdTra tructiep cua hdDoi + hdGoc
						+ isnull((select dbo.GetTongTra_byIDHoaDon(hd.ID_HoaDon, hd.NgayLapHoaDon)),0) --- allHDTra + chilienquan					
						as PhaiThanhToan,
					0 as DaThanhToan
				from BH_HoaDon hd
				left join BH_HoaDon hdgoc on hd.ID_HoaDon = hdgoc.ID
				where hd.ChoThanhToan='0'
				and hd.LoaiHoaDon in (1,6,19,25)
				and hd.ID= @ID_HoaDon
				and hd.NgayLapHoaDon < @NgayLapHoaDon

				

				union all

				 ------ get phieuthu/chi of hd (tra/hdgoc) duoc truyen vao ---		
				 ------ neu HDxuly from baogia,  not get phieuthu of baogia
				select 
					qct.ID_HoaDonLienQuan,
					0 as PhaiThanhToan, 
					iif(@LoaiHDGoc=3,0,	iif(qhd.LoaiHoaDon=12,  qct.TienThu, -qct.TienThu)) as DaThanhToan
				from Quy_HoaDon_ChiTiet qct				
				join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
				where qhd.TrangThai='1'
				and qct.ID_HoaDonLienQuan= @ID_HoaDon				
			
				
				union all

				---- thu dathang hdgoc ---
				select 
					allXL.ID,
					0 as PhaiThanhToan, 
					allXL.ThuDatHang
				from
				(
						select hdXl.ID, 
							hdXl.ID_HoaDon, 
							hdXl.NgayLapHoaDon ,
							hdXl.PhaiThanhToan,
							ROW_NUMBER() over (order by hdXl.NgayLapHoaDon) as RN,
							isnull(ThuDatHang,0) as ThuDatHang
						from BH_HoaDon hdXl
						left join(
							select 
								@ID_HoaDonGoc as ID_HoaDonLienQuan,
								sum(iif(qhd.LoaiHoaDon =12, qct.TienThu,  -qct.TienThu)) as ThuDatHang
							from Quy_HoaDon qhd
							join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
							where qhd.TrangThai='1'
							and qct.ID_HoaDonLienQuan= @ID_HoaDonGoc
						) thuDH on thuDH.ID_HoaDonLienQuan = hdXl.ID_HoaDon
						where  hdXl.ID_HoaDon= @ID_HoaDonGoc ---- get all HDxuly from baogia						
						and hdXl.LoaiHoaDon in (1,25)
						and hdXl.ChoThanhToan= '0'
					) allXL where allXL.RN= 1
					and allXL.ID= @ID_HoaDon
			) tbl
		)
	
	RETURN @Gtri
END");

			Sql(@"CREATE FUNCTION [dbo].[GetTongTra_byIDHoaDon]
(
	@ID_HoaDon uniqueidentifier,
	@NgayLapHoaDon datetime
)
RETURNS float
AS
BEGIN
		DECLARE @Gtri float=0

		---- get all hdTra of hdgoc with ngaylap < ngaylap of HDTra current --
		declare @tblHDTra table (ID uniqueidentifier, PhaiThanhToan float)
		insert into @tblHDTra
		select ID, PhaiThanhToan
		from BH_HoaDon
		where LoaiHoaDon = 6
		and ChoThanhToan='0'
		and ID_HoaDon= @ID_HoaDon
		and NgayLapHoaDon < @NgayLapHoaDon ---- hd$root: get allHDTra (don't check NgayLap)

	set @Gtri = (
					select 					
						sum(PhaiThanhToan + DaTraKhach) as NoKhach
					from
					(
						select ID, -PhaiThanhToan as PhaiThanhToan, 0 as DaTraKhach
						from @tblHDTra					

						union all
						---- get phieuchi hdTra ----
						select 
							hdt.ID,
							0 as PhaiThanhToan,
							iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu) as DaTraKhach
						from @tblHDTra hdt
						join Quy_HoaDon_ChiTiet qct on hdt.ID = qct.ID_HoaDonLienQuan
						join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
						where qhd.TrangThai='1'
						and qhd.NgayLapHoaDon < @NgayLapHoaDon
					) tblThuChi
		)
	RETURN @Gtri 

END");

			Sql(@"ALTER TRIGGER [dbo].[UpdateNgayGiaoDichGanNhat_DMDoiTuong]
   ON [dbo].[BH_HoaDon]
   after insert, update
AS 
BEGIN

	SET NOCOUNT ON;
	declare @ID_KhachHang uniqueidentifier, @NgayInsert DATETIME, @LoaiHoaDon INT, @ChoThanhToan INT;
	select top 1 @ID_KhachHang = ID_DoiTuong, @NgayInsert = NgayLapHoaDon, @LoaiHoaDon = LoaiHoaDon, @ChoThanhToan = ChoThanhToan from inserted;
	DECLARE @NgayMaxTemp datetime;
	SELECT @NgayMaxTemp = NgayGiaoDichGanNhat FROM DM_DoiTuong where ID = @ID_KhachHang
	IF (@NgayInsert > @NgayMaxTemp or @NgayMaxTemp is null) AND @LoaiHoaDon IN (1,2,19,22,25,36) AND @ChoThanhToan IS NOT NULL
	BEGIN
		update dt set NgayGiaoDichGanNhat = @NgayInsert -- inserted.NgayLapHoaDon
		from DM_DoiTuong dt
		where ID = @ID_KhachHang
	END
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetChiTietHoaDon_ByIDHoaDon]
@ID_HoaDon [uniqueidentifier]	='45e70231-c64e-4d55-b279-2ea04413db1c'
AS
BEGIN
  set nocount on;

  declare @ID_DonVi uniqueidentifier, @loaiHD int, @ID_HoaDonGoc uniqueidentifier		
	select top 1 @ID_DonVi= ID_DonVi, @ID_HoaDonGoc= ID_HoaDon, @loaiHD= LoaiHoaDon from BH_HoaDon where ID= @ID_HoaDon

  if @loaiHD= 8 or @loaiHD= 35
		begin
		select 
			ctxk.ID,ctxk.ID_DonViQuiDoi,ctxk.ID_LoHang,
			ctxk.SoLuong,
			ctxk.SoLuong as SoLuongXuatHuy,
			ctxk.DonGia,
			ctxk.GiaVon, 
			ctxk.GiaTriHuy as ThanhTien, 
			ctxk.GiaTriHuy as ThanhToan, 
			ctxk.TienChietKhau as GiamGia,
			ctxk.GhiChu,
			cast(ctxk.SoThuTu as float) as SoThuTu,
			hd.MaHoaDon,
			hd.NgayLapHoaDon,
			hd.ID_NhanVien,
    		nv.TenNhanVien,
			lh.NgaySanXuat,
    		lh.NgayHetHan,    			
    		dvqd.MaHangHoa,
    		hh.TenHangHoa,
			hh.TenHangHoa as TenHangHoaThayThe,
			Case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang, 
    		concat(hh.TenHangHoa , '', dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
    		dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
    		CAST(ROUND(3, 0) as float) as TrangThaiMoPhieu,
			ROUND(ISNULL(TonKho,0),2) as TonKho
		from 
		(
		--- get ct if has tpdinhluong
		select max(ct.ID) as ID,
			max(ct.SoThuTu) as SoThuTu,
			ct.ID_DonViQuiDoi,
			ct.ID_LoHang,
			@ID_HoaDon as ID_HoaDon,
			sum(ct.SoLuong) as SoLuong, 
			max(ct.DonGia) as DonGia,
			max(ct.DonGia) as GiaVon,
			sum(ct.SoLuong * ct.DonGia) as GiaTriHuy,			
			max(ct.TienChietKhau) as TienChietKhau,
			max(ct.GhiChu) as GhiChu
		from BH_HoaDon_ChiTiet ct
		where ct.ID_HoaDon= @ID_HoaDon		
		and (ct.ChatLieu is null or ct.ChatLieu!='5')
		group by ct.ID_DonViQuiDoi, ct.ID_LoHang		
		)ctxk
		join BH_HoaDon hd on hd.ID= ctxk.ID_HoaDon
		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_DonVi)
		where (hh.LaHangHoa = 1 and tk.TonKho is not null)
		end
	else
	begin
		if @loaiHD in (1,3,2,25,36) ---- 1.hoadonle, 2.hoadon baohanh, 3. baogia, 25. hoadon suachua
		begin
			select ctsd.ID_ChiTietGoiDV, sum(SoLuong) as SoLuongSuDung
			into #tblSDDV 
			from BH_HoaDon_ChiTiet ctsd
			join BH_HoaDon hd on ctsd.ID_HoaDon= hd.ID
			where exists (select ID from BH_HoaDon_ChiTiet ct where ct.ID_HoaDon= @ID_HoaDon and ct.ID_ChiTietGoiDV =  ctsd.ID_ChiTietGoiDV)
			and hd.ChoThanhToan= 0
			AND (ctsd.ID_ChiTietDinhLuong IS NULL OR ctsd.ID_ChiTietDinhLuong = ctsd.ID) --- khong get tpdinhluong khi sudung GDV
			group by ctsd.ID_ChiTietGoiDV

					select DISTINCT tbl.*, 
					isnull(hdXK.SoLuongXuat,0) as SoLuongXuat,
					isnull(hdmua.SoLuongMua,0) as SoLuongMua,
					isnull(hdmua.SoLuongMua,0) - isnull(hdmua.SoLuongDVDaSuDung,0) as SoLuongDVConLai,
					isnull(hdmua.SoLuongDVDaSuDung,0) as SoLuongDVDaSuDung
					FROM 
						 (SELECT
    							cthd.ID,cthd.ID_HoaDon,DonGia,cthd.GiaVon,SoLuong,ThanhTien,ThanhToan,cthd.ID_DonViQuiDoi, cthd.ID_ChiTietDinhLuong, cthd.ID_ChiTietGoiDV,
    							cthd.TienChietKhau AS GiamGia,PTChietKhau,cthd.GhiChu,cthd.TienChietKhau,
    							(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
								qd.GiaBan as GiaBanHH, ---- used to nhaphang from hoadon
    							CAST(SoThuTu AS float) AS SoThuTu,cthd.ID_KhuyenMai, ISNULL(cthd.TangKem,'0') as TangKem, cthd.ID_TangKem,
								-- replace char enter --> char space
    							(REPLACE(REPLACE(TenHangHoa,CHAR(13),''),CHAR(10),'') +
    							CASE WHEN (qd.ThuocTinhGiaTri is null or qd.ThuocTinhGiaTri = '') then '' else '_' + qd.ThuocTinhGiaTri end +
    							CASE WHEN TenDonVitinh = '' or TenDonViTinh is null then '' else ' (' + TenDonViTinh + ')' end +
    							CASE WHEN MaLoHang is null then '' else '. Lô: ' + MaLoHang end) as TenHangHoaFull,
    				
    							hh.ID AS ID_HangHoa,
								hh.LaHangHoa,
								hh.QuanLyTheoLoHang,
								hh.TenHangHoa, 
								isnull(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
								ISNULL(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
								ISNULL(ID_NhomHang,'00000000-0000-0000-0000-000000000000') as ID_NhomHangHoa,	
    							TenDonViTinh,MaHangHoa,
    							lo.ID AS ID_LoHang,
								ISNULL(MaLoHang,'') as MaLoHang,
								lo.NgaySanXuat, lo.NgayHetHan,
								qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
								ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, 
								CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
								CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
								CAST(ISNULL(cthd.PTThue,0) as float) as PTThue,
								CAST(ISNULL(cthd.TienThue,0) as float) as TienThue,
								CAST(ISNULL(cthd.ThoiGianBaoHanh,0) as float) as ThoiGianBaoHanh,
								CAST(ISNULL(cthd.LoaiThoiGianBH,0) as float) as LoaiThoiGianBH,
								Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
								Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end LaPTPhiDichVu,
								CAST(0 as float) as TongPhiDichVu, -- set default PhiDichVu = 0 (caculator again .js)
								CAST(ISNULL(cthd.Bep_SoLuongYeuCau,0) as float) as Bep_SoLuongYeuCau,
								CAST(ISNULL(cthd.Bep_SoLuongHoanThanh,0) as float) as Bep_SoLuongHoanThanh, -- view in CTHD NhaHang
								CAST(ISNULL(cthd.Bep_SoLuongChoCungUng,0) as float) as Bep_SoLuongChoCungUng,
								ISNULL(hh.SoPhutThucHien,0) as SoPhutThucHien, -- lay so phut theo cai dat
								ISNULL(cthd.ThoiGianThucHien,0)  as ThoiGianThucHien,-- sophut thuc te thuchien	
								ISNULL(cthd.QuaThoiGian,0)  as QuaThoiGian,
				
								case when hh.LaHangHoa='0' then 0 else ISNULL(tk.TonKho,0) end as TonKho,
								cthd.ID_ViTri,
								ISNULL(vt.TenViTri,'') as TenViTri,			
								ThoiGian,cthd.ThoiGianHoanThanh, ISNULL(hh.GhiChu,'') as GhiChuHH,
								ISNULL(cthd.DiemKhuyenMai,0) as DiemKhuyenMai,
								ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
								ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
								ISNULL(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
								ISNULL(hh.ChietKhauMD_NVTheoPT,'0') as ChietKhauMD_NVTheoPT,
								cthd.ChatLieu,
								isnull(cthd.DonGiaBaoHiem,0) as DonGiaBaoHiem,
								iif(cthd.TenHangHoaThayThe is null or cthd.TenHangHoaThayThe ='',hh.TenHangHoa, cthd.TenHangHoaThayThe) as TenHangHoaThayThe,					
								cthd.ID_LichBaoDuong,
								iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
								cthd.ID_ParentCombo,
								qd.GiaNhap
					
    					FROM BH_HoaDon_ChiTiet cthd
    					left JOIN DonViQuiDoi qd ON cthd.ID_DonViQuiDoi = qd.ID
    					left JOIN DM_HangHoa hh ON qd.ID_HangHoa= hh.ID    		
    					left JOIN DM_NhomHangHoa nhh ON hh.ID_NhomHang= nhh.ID    							
    					LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
						left join DM_HangHoa_TonKho tk on cthd.ID_DonViQuiDoi= tk.ID_DonViQuyDoi and tk.ID_DonVi= @ID_DonVi
						left join DM_ViTri vt on cthd.ID_ViTri= vt.ID
    					-- chi get CT khong phai la TP dinh luong
    					WHERE cthd.ID_HoaDon = @ID_HoaDon
								and (cthd.ChatLieu is null or cthd.ChatLieu!='5')
								AND (cthd.ID_ChiTietDinhLuong IS NULL OR cthd.ID_ChiTietDinhLuong = cthd.ID)
								and ((tk.ID_DonVi = @ID_DonVi and hh.LaHangHoa='1') 
								or tk.ID_DonVi is null
								or (hh.LaHangHoa='0'))
								and (cthd.ID_LoHang= tk.ID_LoHang OR (cthd.ID_LoHang is null and tk.ID_LoHang is null)) 
								and (cthd.ID_ParentCombo IS NULL OR cthd.ID_ParentCombo = cthd.ID) --- khong get tpcombo
						) tbl
						left join
						(
							select ctm.ID as ID_ChiTietGoiDV, ctm.SoLuong as SoLuongMua, isnull(ctsd.SoLuongSuDung,0) as SoLuongDVDaSuDung
							from BH_HoaDon_ChiTiet ctm
							join #tblSDDV ctsd  on ctm.ID= ctsd.ID_ChiTietGoiDV			
						) hdmua on tbl.ID_ChiTietGoiDV = hdmua.ID_ChiTietGoiDV
						left join 
						(
						--- soluongxuatkho
							select SUM(ctxk.SoLuong) as SoLuongXuat, ctxk.ID_ChiTietGoiDV
							from BH_HoaDon_ChiTiet ctxk 
							join BH_HoaDon hdxk on ctxk.ID_HoaDon = hdxk.ID
							where hdxk.ID_HoaDon = @ID_HoaDon
							and hdxk.LoaiHoaDon = 8 and hdxk.ChoThanhToan='0'
							group by ctxk.ID_ChiTietGoiDV			
						) hdXK on tbl.ID = hdXK.ID_ChiTietGoiDV 
						order by tbl.SoThuTu
		end
		else
			if @loaiHD= 4 and @ID_HoaDonGoc is not null
			begin	
				SELECT 
    				cthd.ID,
					cthd.ID_HoaDon, 
					cthd.ID_ParentCombo,
					cthd.ID_ChiTietDinhLuong,
					cthd.ID_ChiTietGoiDV, ---- used to update cthd (check nhapmua from PO)
					cthd.DonGia, 
					cthd.GiaVon, 
					isnull(cthd.TonLuyKe,0) as TonLuyKe, --- tonkho tai thoidiem xxx cua {NgayLapHoaDon}: used to print PO
					cast(cthd.SoThuTu as float) as SoThuTu,
					SoLuong, 
					isnull(ctConLai.SoLuongConLai,0) as SoLuongConLai,
					cthd.ThanhTien, 
					TienChietKhau, 
					cthd.ThanhToan, 
					cthd.TienThue, 
					isnull(cthd.PTThue,0) as PTThue, 
					dvqd.ID as ID_DonViQuiDoi,
    				dvqd.ID_HangHoa, dvqd.TenDonViTinh, dvqd.MaHangHoa,
					TienChietKhau as GiamGia, PTChietKhau,
					(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
					cthd.GhiChu,
    				cthd.ID_KhuyenMai,			
					lo.NgaySanXuat, lo.NgayHetHan,
    				dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
					concat(TenHangHoa ,
    				dvqd.ThuocTinhGiaTri, 
    				Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    				Case when lo.MaLoHang is null then '' else '. Lô: ' + lo.MaLoHang end) as TenHangHoaFull,
    				LaHangHoa, QuanLyTheoLoHang,
					TenHangHoa,		
					hh.TenHangHoa as TenHangHoaThayThe,
    				TyLeChuyenDoi,
    				lo.ID AS ID_LoHang, ISNULL(MaLoHang,'') as MaLoHang, 			
					ISNULL(hhtonkho.TonKho, 0) as TonKho, 	
					dvqd.GiaNhap, 
					dvqd.GiaBan as GiaBanMaVach, hh.ID_NhomHang as ID_NhomHangHoa,
					dvqd.LaDonViChuan, hh.ChiPhiThucHien as PhiDichVu, cast(ISNULL(hh.QuyCach,1) as float) as QuyCach, 
					Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
					dvqd.GiaBan, dvqd.GiaBan as GiaBanHH, -- use to get banggiachung  of cthd (at NhapHangChiTiet),
					ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
					ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
					ISNULL(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
					ISNULL(hh.ChietKhauMD_NVTheoPT,'0') as ChietKhauMD_NVTheoPT,
					ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
					cthd.ID_LichBaoDuong,
					iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa
    			FROM BH_HoaDon_ChiTiet cthd
				left join
				(
						----- get sl conlai saukhi xuly nhapmua
					select ctpo.ID, ctpo.SoLuong - isnull(ctXL.SoLuong,0) as SoLuongConLai
					from BH_HoaDon_ChiTiet ctpo
					left join
					(
						---- ctxuly != hd current
						select sum(ctxl.SoLuong) as SoLuong, ctxl.ID_ChiTietGoiDV
						from BH_HoaDon_ChiTiet ctxl
						join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
						where hdxl.ID_HoaDon= @ID_HoaDonGoc and hdxl.ChoThanhToan='0' and hdxl.LoaiHoaDon= 4
						and hdxl.ID != @ID_HoaDon
						group by ctxl.ID_ChiTietGoiDV
					) ctXL on ctpo.ID= ctXL.ID_ChiTietGoiDV
				) ctConLai on cthd.ID_ChiTietGoiDV= ctConLai.ID
    			left JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    			left JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
    			LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi
				and (hhtonkho.ID_LoHang = cthd.ID_LoHang or cthd.ID_LoHang is null) and hhtonkho.ID_DonVi = @ID_DonVi				
    			WHERE cthd.ID_HoaDon = @ID_HoaDon 
				and (cthd.ID_ChiTietDinhLuong = cthd.ID or cthd.ID_ChiTietDinhLuong is null)
				and (cthd.ID_ParentCombo IS NULL OR cthd.ID_ParentCombo = cthd.ID)
				and (cthd.ChatLieu is null or cthd.ChatLieu!='5')
				order by cthd.SoThuTu desc			
			end
			else

			begin
				SELECT 
    			cthd.ID,
				cthd.ID_HoaDon, 
				cthd.ID_ParentCombo,
				cthd.ID_ChiTietDinhLuong,
				cthd.ID_ChiTietGoiDV, ---- used to update cthd (check nhapmua from PO)
				cthd.DonGia, 
				cthd.GiaVon, 
				isnull(cthd.TonLuyKe,0) as TonLuyKe, --- tonkho tai thoidiem xxx cua {NgayLapHoaDon}: used to print PO
				cast(cthd.SoThuTu as float) as SoThuTu,
				SoLuong, 
				cthd.ThanhTien, 
				TienChietKhau, 
				cthd.ThanhToan, 
				cthd.TienThue, 
				isnull(cthd.PTThue,0) as PTThue, 
				dvqd.ID as ID_DonViQuiDoi,
    			dvqd.ID_HangHoa, dvqd.TenDonViTinh, dvqd.MaHangHoa,
				TienChietKhau as GiamGia, PTChietKhau,
				(cthd.DonGia - cthd.TienChietKhau) as GiaBan,
				cthd.GhiChu,
    			cthd.ID_KhuyenMai,			
				lo.NgaySanXuat, lo.NgayHetHan,
    			dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				concat(TenHangHoa ,
    			dvqd.ThuocTinhGiaTri, 
    			Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    			Case when lo.MaLoHang is null then '' else '. Lô: ' + lo.MaLoHang end) as TenHangHoaFull,
    			LaHangHoa, QuanLyTheoLoHang,
				TenHangHoa,		
				hh.TenHangHoa as TenHangHoaThayThe,
    			TyLeChuyenDoi,
    			lo.ID AS ID_LoHang, ISNULL(MaLoHang,'') as MaLoHang, 			
				ISNULL(hhtonkho.TonKho, 0) as TonKho, 
				dvqd.GiaNhap, 
				dvqd.GiaBan as GiaBanMaVach, hh.ID_NhomHang as ID_NhomHangHoa,
				dvqd.LaDonViChuan, hh.ChiPhiThucHien as PhiDichVu, cast(ISNULL(hh.QuyCach,1) as float) as QuyCach, 
				Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
				dvqd.GiaBan, dvqd.GiaBan as GiaBanHH, -- use to get banggiachung  of cthd (at NhapHangChiTiet),
				ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio,
				ISNULL(hh.DuocTichDiem,0) as DuocTichDiem,
				ISNULL(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
				ISNULL(hh.ChietKhauMD_NVTheoPT,'0') as ChietKhauMD_NVTheoPT,
				ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
				cthd.ID_LichBaoDuong,
				iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa
    			FROM BH_HoaDon_ChiTiet cthd
    			JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    			JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
    			LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
				LEFT JOIN DM_HangHoa_TonKho hhtonkho on dvqd.ID = hhtonkho.ID_DonViQuyDoi
				and (hhtonkho.ID_LoHang = cthd.ID_LoHang or cthd.ID_LoHang is null) and hhtonkho.ID_DonVi = @ID_DonVi
    			WHERE cthd.ID_HoaDon = @ID_HoaDon 
				and (cthd.ID_ChiTietDinhLuong = cthd.ID or cthd.ID_ChiTietDinhLuong is null)
				and (cthd.ID_ParentCombo IS NULL OR cthd.ID_ParentCombo = cthd.ID)
				and (cthd.ChatLieu is null or cthd.ChatLieu!='5')
				order by cthd.SoThuTu desc
			end
	end
    END");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonDatHang]	
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max),
	@ID_NhanVienLogin uniqueidentifier,
	@NguoiTao nvarchar(max),
	@TrangThai nvarchar(max),
	@ColumnSort varchar(max),
	@SortBy varchar(max),
	@CurrentPage int,
	@PageSize int,
	@LaHoaDonSuaChua nvarchar(10)
AS
BEGIN
	set nocount on;

	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'DatHang_XemDS_PhongBan','DatHang_XemDS_HeThong');

	declare @tblChiNhanh table (ID varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
	

with data_cte
as
(
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
		c.ID_BaoHiem,
    	c.ID_DonVi,
		c.ID_PhieuTiepNhan,
		c.TongThanhToan,
    	c.YeuCau,
		'' as MaHoaDonGoc,
		c.MaSoThue,
		c.TaiKhoanNganHang,
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
    	c.TongTienHang, c.TongGiamGia, c.PhaiThanhToan, c.ConNo,
		c.TienMat,
		c.TienATM,
		c.ChuyenKhoan,
		c.KhachDaTra,c.TongChietKhau,c.TongTienThue, c.ThuTuThe, c.TongChiPhi,
    	c.TrangThaiText,
		c.TrangThaiHD,
    	c.TheoDoi,
    	c.TenPhongBan,
		c.ChoThanhToan,
		c.ChiPhi_GhiChu,
    	'' as HoaDon_HangHoa, -- string contail all MaHangHoa,TenHangHoa of HoaDon
		 c.MaPhieuTiepNhan,
		c.BienSo,
		c.MaBaoHiem, c.TenBaoHiem, c.BH_SDT,
		c.LienHeBaoHiem, c.SoDienThoaiLienHeBaoHiem,
		c.PhaiThanhToanBaoHiem, c.PTThueBaoHiem,
		TongTienBHDuyet, c.PTThueHoaDon, c.TongTienThueBaoHiem, SoVuBaoHiem,KhauTruTheoVu, 
		PTGiamTruBoiThuong, GiamTruBoiThuong, BHThanhToanTruocThue

    	FROM
    	(
    		select 
    		a.ID as ID,
    		bhhd.MaHoaDon,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DoiTuong,
    		bhhd.ID_BangGia,
    		bhhd.NgayLapHoaDon,
    		bhhd.YeuCau,
    		bhhd.ID_DonVi,
			bhhd.ID_BaoHiem,
			
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,
			dt.MaDoiTuong,
			dt.MaSoThue,
			dt.TaiKhoanNganHang,
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
			ISNULL(nv.TenNhanVienKhongDau, N'') as TenNhanVienKhongDau,
    		ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
    		ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
    		bhhd.DienGiai,
    		bhhd.NguoiTao as NguoiTaoHD,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
			ceiling(isnull(bhhd.TongTienHang,0)) as TongTienHang,
    		ceiling(isnull(bhhd.TongGiamGia,0)) as TongGiamGia,    		
			ceiling(isnull(bhhd.PhaiThanhToan,0)) as PhaiThanhToan,
			CAST(ROUND(bhhd.TongTienThue, 0) as float) as TongTienThue,
			isnull(bhhd.TongChiPhi,	0) as TongChiPhi,
			bhhd.ChiPhi_GhiChu,
    		a.KhachDaTra,
			a.ThuTuThe,
			a.TienMat,
			a.TienATM,
			a.ChuyenKhoan,
    		bhhd.TongChietKhau,
			bhhd.ID_PhieuTiepNhan,					
			bhhd.TongThanhToan,
			bhhd.TongThanhToan - a.KhachDaTra as ConNo,
			bhhd.ChoThanhToan,

			isnull(bhhd.PTThueHoaDon,0) as PTThueHoaDon,			
			isnull(bhhd.PTThueBaoHiem,0) as PTThueBaoHiem,
			isnull(bhhd.TongTienThueBaoHiem,0) as TongTienThueBaoHiem,
			isnull(bhhd.SoVuBaoHiem,0) as SoVuBaoHiem,
			isnull(bhhd.KhauTruTheoVu,0) as KhauTruTheoVu,
			isnull(bhhd.TongTienBHDuyet,0) as TongTienBHDuyet,
			isnull(bhhd.PTGiamTruBoiThuong,0) as PTGiamTruBoiThuong,
			isnull(bhhd.GiamTruBoiThuong,0) as GiamTruBoiThuong,
			isnull(bhhd.BHThanhToanTruocThue,0) as BHThanhToanTruocThue,
			isnull(bhhd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,

			isnull(bh.TenDoiTuong,'') as TenBaoHiem,
			isnull(bh.MaDoiTuong,'') as MaBaoHiem,
			isnull(bh.DienThoai,'') as BH_SDT,
			iif(bhhd.ID_BaoHiem is null,'',tn.NguoiLienHeBH) as LienHeBaoHiem,
			iif(bhhd.ID_BaoHiem is null,'',tn.SoDienThoaiLienHeBH) as SoDienThoaiLienHeBaoHiem,
			isnull(tn.MaPhieuTiepNhan,'') as MaPhieuTiepNhan,
			isnull(xe.BienSo,'') as BienSo,
			iif(bhhd.ID_PhieuTiepNhan is null, '0','1') as LaHoaDonSuaChua,
			case bhhd.ChoThanhToan
				when 1 then N'Phiếu tạm' 
				when 0 then 
					case bhhd.YeuCau
						when '2' then  N'Đang giao hàng' 
						when '3' then  N'Hoàn thành' 
						else N'Đã lưu' end
				else  N'Đã hủy'
				end as TrangThaiText,
		 
			
		
			case bhhd.ChoThanhToan
				when 1 then '1'
				when 0 then 
					case bhhd.YeuCau
						when 2 then  '2'
						when 3 then '3'
						else '0' end
				else '4' end as TrangThaiHD -- used to where
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
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd 
					and exists (select ID from @tblChiNhanh cn where bhhd.ID_DonVi = cn.ID)
    
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
					and bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd 
					and exists (select ID from @tblChiNhanh cn where bhhd.ID_DonVi = cn.ID)
    			) b
    			group by b.ID 
    		) as a
    		inner join BH_HoaDon bhhd on a.ID = bhhd.ID
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
			left join DM_DoiTuong bh on bhhd.ID_BaoHiem = bh.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID    
			left join Gara_PhieuTiepNhan tn on bhhd.ID_PhieuTiepNhan = tn.ID
			left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID	
			where bhhd.LoaiHoaDon = 3 
			and
			bhhd.NgayLapHoadon >= @timeStart and bhhd.NgayLapHoaDon < @timeEnd 
    		) as c
			where exists( select * from @tblNhanVien nv where nv.ID= c.ID_NhanVien or c.NguoiTaoHD= @NguoiTao)
			and exists( select Name from dbo.splitstring(@TrangThai) tt where  c.TrangThaiHD  = tt.Name)
			and exists (select Name from dbo.splitstring(@LaHoaDonSuaChua) tt where c.LaHoaDonSuaChua = tt.Name)
			and
				((select count(Name) from @tblSearch b where     			
				c.MaHoaDon like '%'+b.Name+'%'
				or c.NguoiTaoHD like '%'+b.Name+'%'
				or c.TenNhanVien like '%'+b.Name+'%'
				or c.TenNhanVienKhongDau like '%'+b.Name+'%'
				or c.DienGiai like '%'+b.Name+'%'
				or c.MaDoiTuong like '%'+b.Name+'%'		
				or c.TenDoiTuong like '%'+b.Name+'%'
				or c.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or c.DienThoai like '%'+b.Name+'%'	
				or c.MaPhieuTiepNhan like '%'+b.Name+'%'
				or c.BienSo like '%'+b.Name+'%'	
				)=@count or @count=0)	
		
    ),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,
				sum(TongGiamGia) as SumTongGiamGia,
					sum(TongChiPhi) as SumTongChiPhi,
				sum(KhachDaTra) as SumKhachDaTra,	
				sum(PhaiThanhToan) as SumPhaiThanhToan,			
				sum(TongThanhToan) as SumTongThanhToan,				
				sum(ThuTuThe) as SumThuTuThe,				
				sum(TienMat) as SumTienMat,
				sum(TienATM) as SumPOS,
				sum(ChuyenKhoan) as SumChuyenKhoan,				
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo
			from data_cte
		)
		select dt.*, cte.*		
		from data_cte dt
		cross join count_cte cte	
		order by 
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaHoaDon' then MaHoaDon end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end DESC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaPhieuTiepNhan' then dt.MaPhieuTiepNhan end DESC,
			case when @SortBy <>'ASC' then ''
			when @ColumnSort='MaPhieuTiepNhan' then dt.MaPhieuTiepNhan end ASC,
			case when @SortBy <> 'ASC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='TongTienHang' then TongTienHang end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiamGia' then TongGiamGia end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachCanTra' then PhaiThanhToan end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachCanTra' then PhaiThanhToan end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC	
				
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_HoaDonNhapHang]
    @TextSearch [nvarchar](max),
    @LoaiHoaDon varchar(50), ---- dùng chung cho nhập hàng + trả hàng nhập + nhập kho nội bộ
    @IDChiNhanhs [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TrangThais [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int],
    @ColumnSort [nvarchar](max),
    @SortBy [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @tblChiNhanh table (ID varchar(40))
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs)

		declare @tblLoaiHD table (Loai int)
    	insert into @tblLoaiHD
    	select Name from dbo.splitstring(@LoaiHoaDon)
    
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

		----- get list hd from - to
		select hd.ID, hd.ID_HoaDon, hd.ID_DoiTuong, hd.NgayLapHoaDon, hd.ChoThanhToan				
		into #tmpHoaDon
		from BH_HoaDon hd
		where hd.NgayLapHoadon between @FromDate and @ToDate					
		and exists (select loai.Loai from @tblLoaiHD loai where hd.LoaiHoaDon = loai.Loai)
		and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)

    
    	;with data_cte
    	as (
    	select hdQuy.*	, hdQuy.PhaiThanhToan - hdQuy.KhachDaTra as ConNo
    	from
    	(	
    	select hd.id, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hd.DienGiai, hd.PhaiThanhToan, hd.ChoThanhToan,
    	hd.NgayLapHoaDon, hd.ID_NhanVien, hd.ID_BangGia, hd.TongTienHang, hd.TongChietKhau, hd.TongGiamGia, hd.TongChiPhi,
    	hd.TongTienThue, hd.TongThanhToan, hd.ID_DoiTuong, 
		ctHD.ThanhTienChuaCK,
		ctHD.GiamGiaCT,	
		iif(@LoaiHoaDon='7', -isnull(quy.TienThu,0),  isnull(quy.TienThu,0))  as KhachDaTra,
		isnull(quy.TienMat,0) as TienMat,
		isnull(quy.ChuyenKhoan,0) as ChuyenKhoan,
		isnull(quy.TienATM,0) as TienATM,
		isnull(quy.TienDatCoc,0) as TienDatCoc,
		hd.NguoiTao, hd.NguoiTao as NguoiTaoHD,
    	dv.TenDonVi,hd.ID_DonVi,
		isnull(hd.PTThueHoaDon,0) as PTThueHoaDon,
    	isnull(dv.SoDienThoai,'') as DienThoaiChiNhanh,
    	isnull(dv.DiaChi,'') as DiaChiChiNhanh,
		iif(hd.LoaiHoaDon = 13, iif(hd.ID_DoiTuong='00000000-0000-0000-0000-000000000002',4,dt.LoaiDoiTuong), dt.LoaiDoiTuong) as LoaiDoiTuong,
		---- nhapnoibo: lay nhacc/nhanvien chung 1 cot
		iif(hd.LoaiHoaDon = 13, iif(hd.ID_DoiTuong='00000000-0000-0000-0000-000000000002',nv.MaNhanVien,dt.MaDoiTuong), dt.MaDoiTuong) as MaDoiTuong,
    	iif(hd.LoaiHoaDon = 13, iif(hd.ID_DoiTuong='00000000-0000-0000-0000-000000000002',nv.TenNhanVien,dt.TenDoiTuong), dt.TenDoiTuong) as TenDoiTuong,		
    	isnull(dt.DienThoai,'') as DienThoai,
    	isnull(dt.TenDoiTuong_KhongDau,'') as TenDoiTuong_KhongDau,
    	isnull(nv.MaNhanVien,'') as MaNhanVien,	
    	isnull(nv.TenNhanVien,'') as TenNhanVien,	
    	isnull(nv.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
		po.MaHoaDon as MaHoaDonGoc,
		isnull(po.LoaiHoaDon,0) as LoaiHoaDonGoc,
		hd.YeuCau,
		case hd.LoaiHoaDon
			when 4 then N'Nhập hàng'
			when 13 then N'Nhập kho nội bộ'
			when 14 then N'Nhập hàng khách thừa'
			when 31 then N'Đặt hàng nhà cung cấp'
		else 'Nhập hàng' end as strLoaiHoaDon,
		case hd.ChoThanhToan			
				when 1 then N'Tạm lưu' 
				when 0 then 
					case hd.YeuCau
						when '1' then  N'Đã lưu' 
						when '2' then  N'Đang xử lý' 
						when '3' then  N'Hoàn thành' 
						when '4' then  N'Đã hủy' 
						else iif(hd.LoaiHoaDon = 31, N'Đã lưu' , N'Đã nhập hàng') end
				else  N'Đã hủy'
				end as TrangThaiText,
		case hd.ChoThanhToan			
			when 1 then N'0' 
			when 0 then 
				case hd.YeuCau
					when '1' then '1'
					when '2' then '2'
					when '3' then '3'
					when '4' then '4'
					else '1' end
			else '4' end as TrangThaiHD				    	
    	from BH_HoaDon hd
    	join DM_DonVi dv on hd.ID_DonVi= dv.ID
		left join BH_HoaDon po on hd.ID_HoaDon= po.ID
    	left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
		left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
		left join (
			select 
				ct.ID_HoaDon,
				sum(ct.SoLuong * ct.TienChietKhau) as GiamGiaCT,			
				sum(ct.SoLuong * ct.DonGia) as ThanhTienChuaCK
			from BH_HoaDon_ChiTiet ct
    		join BH_HoaDon hd on ct.ID_HoaDon= hd.ID   			
    		where exists (select Loai from @tblLoaiHD loaiHD where hd.LoaiHoaDon= loaiHD.Loai)	
    		and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    		and  exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
			group by ct.ID_HoaDon
		) ctHD on ctHD.ID_HoaDon = hd.ID
    	left join
    	(
				select 
					tblTongChi.ID_HoaDonLienQuan,
					sum(tblTongChi.TienThu) as TienThu,
					sum(tblTongChi.TienMat) as TienMat,
					sum(tblTongChi.TienATM) as TienATM,				
					sum(tblTongChi.ChuyenKhoan) as ChuyenKhoan,
					sum(tblTongChi.TienDatCoc) as TienDatCoc
				from
				(   					
						----- thu/chi chinh no ----
    					select qct.ID_HoaDonLienQuan,   
							sum(iif(qct.HinhThucThanhToan = 1, qct.TienThu, 0)) as TienMat,
							sum(iif(qct.HinhThucThanhToan = 2, qct.TienThu,0)) as TienATM,
							sum(iif(qct.HinhThucThanhToan = 3, qct.TienThu,0)) as ChuyenKhoan,
							sum(iif(qct.HinhThucThanhToan = 6, qct.TienThu,0)) as TienDatCoc,
							sum(iif(qhd.LoaiHoaDon = 11,-qct.TienThu, qct.TienThu)) as TienThu
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID   					
    					where exists (select hd.ID from #tmpHoaDon hd where hd.ID= qct.ID_HoaDonLienQuan)	
    					and (qhd.TrangThai= 1 or qhd.TrangThai is null)    	
						group by qct.ID_HoaDonLienQuan
    				

						union all

						----  get infor PhieuThu/Chi from HDXuLy (if PO)
    					Select
							hdXly.ID_HoaDon,
    						iif(qct.HinhThucThanhToan = 1, qct.TienThu, 0) as TienMat,
							iif(qct.HinhThucThanhToan = 2, qct.TienThu,0) as TienATM,
							iif(qct.HinhThucThanhToan = 3, qct.TienThu,0) as ChuyenKhoan,
							iif(qct.HinhThucThanhToan = 6, qct.TienThu,0) as TienDatCoc,
							iif(qhd.LoaiHoaDon = 11,-qct.TienThu, qct.TienThu) as TienThu
    					from Quy_HoaDon_ChiTiet qct
    					left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
						left join BH_HoaDon hdXly on qct.ID_HoaDonLienQuan = hdXly.ID and hdXly.ChoThanhToan='0'
    					where (qhd.TrangThai= 1 or qhd.TrangThai is null) 
						and exists (
							select ID from #tmpHoaDon hdgoc where hdgoc.ID = hdXly.ID_HoaDon
						)


						union all

							select 
									hdXuLyOut.ID,
									isnull(thuDH.TienMat,0) as TienMat,
									isnull(thuDH.TienPOS,0) as TienATM,
									isnull(thuDH.TienCK,0) as TienCK,
									isnull(thuDH.TienCoc,0) as TienCoc,
									isnull(thuDH.TienThu,0) as TienThu
							from
							(
								------- 1. get list hoadon duoc xuly tu PO ----
								select hdXLy.Id,
									hdXLy.ID_HoaDon,
									hdXLy.NgayLapHoaDon,
									ROW_NUMBER() OVER(PARTITION BY hdXLy.ID_HoaDon ORDER BY hdXLy.NgayLapHoaDon ASC) AS isFirst	
								from BH_HoaDon hdXLy
								where hdXLy.ChoThanhToan='0'
								and exists (select ID_HoaDon from #tmpHoaDon hd where hdXLy.ID_HoaDon = hd.ID_HoaDon)
							)hdXuLyOut
							left join
							(
								----2. get thuDathang PO----
								select 
									qct.ID_HoaDonLienQuan as ID_DatHang,
									sum(iif(qct.HinhThucThanhToan=1, qct.TienThu, 0)) as TienMat,
									sum(iif(qct.HinhThucThanhToan=2, qct.TienThu, 0)) as TienPOS,
									sum(iif(qct.HinhThucThanhToan=3, qct.TienThu, 0)) as TienCK,
									sum(iif(qct.HinhThucThanhToan=6, qct.TienThu, 0)) as TienCoc,		
									sum(iif(qhd.LoaiHoaDon = 11,-qct.TienThu, qct.TienThu)) as TienThu	
								from Quy_HoaDon qhd
								join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon = qhd.ID
								where (qhd.TrangThai= 1 Or qhd.TrangThai is null)		
								and exists (select ID_HoaDon from #tmpHoaDon hd where qct.ID_HoaDonLienQuan = hd.ID_HoaDon)
								group by qct.ID_HoaDonLienQuan
							)thuDH on hdXuLyOut.ID_HoaDon = thuDH.ID_DatHang
							where hdXuLyOut.isFirst = 1						
					)tblTongChi
					group by tblTongChi.ID_HoaDonLienQuan
    	) quy on hd.id = quy.ID_HoaDonLienQuan
    	where exists (select Loai from @tblLoaiHD loaiHD where hd.LoaiHoaDon= loaiHD.Loai)	
		and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    	and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
    ) hdQuy
    where 
    exists (select ID from dbo.splitstring(@TrangThais) tt where hdQuy.TrangThaiHD= tt.Name)	
    	and
    	((select count(Name) from @tblSearch b where     			
    		hdQuy.MaHoaDon like '%'+b.Name+'%'
    		or hdQuy.NguoiTao like '%'+b.Name+'%'				
    		or hdQuy.TenNhanVien like '%'+b.Name+'%'
    		or hdQuy.TenNhanVienKhongDau like '%'+b.Name+'%'
    		or hdQuy.DienGiai like '%'+b.Name+'%'
    		or hdQuy.MaDoiTuong like '%'+b.Name+'%'		
    		or hdQuy.TenDoiTuong like '%'+b.Name+'%'
    		or hdQuy.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		or hdQuy.DienThoai like '%'+b.Name+'%'		
    		)=@count or @count=0)	
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
					sum(ThanhTienChuaCK) as SumThanhTienChuaCK,
					sum(GiamGiaCT) as SumGiamGiaCT,
    				sum(TongTienHang) as SumTongTienHang,
    				sum(TongGiamGia) as SumTongGiamGia,
					sum(TienMat) as SumTienMat,	
					sum(TienATM) as SumPOS,	
					sum(ChuyenKhoan) as SumChuyenKhoan,	
					sum(TienDatCoc) as SumTienCoc,	
    				sum(KhachDaTra) as SumDaThanhToan,				
    				sum(TongChiPhi) as SumTongChiPhi,
    				sum(PhaiThanhToan) as SumPhaiThanhToan,
    				sum(TongThanhToan) as SumTongThanhToan,				
    				sum(TongTienThue) as SumTongTienThue,
    				sum(ConNo) as SumConNo
    			from data_cte
    		)
    		select dt.*, cte.*	
    		from data_cte dt
    		cross join count_cte cte
    		order by 
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='NgayLapHoaDon' then NgayLapHoaDon end DESC,
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='ConNo' then ConNo end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='ConNo' then ConNo end DESC,
    			case when @SortBy <>'ASC' then ''
    			when @ColumnSort='MaHoaDon' then MaHoaDon end ASC,
    			case when @SortBy <>'DESC' then ''
    			when @ColumnSort='MaHoaDon' then MaHoaDon end DESC,
    			case when @SortBy <>'ASC' then ''
    			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end ASC,
    			case when @SortBy <>'DESC' then ''
    			when @ColumnSort='MaKhachHang' then dt.MaDoiTuong end DESC,
    			case when @SortBy <> 'ASC' then 0
    			when @ColumnSort='TongTienHang' then TongTienHang end ASC,
    			case when @SortBy <> 'DESC' then 0
    			when @ColumnSort='TongTienHang' then TongTienHang end DESC,
    			case when @SortBy <>'ASC' then 0
    			when @ColumnSort='GiamGia' then TongGiamGia end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='GiamGia' then TongGiamGia end DESC,
    			case when @SortBy <>'ASC' then 0
    			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='PhaiThanhToan' then PhaiThanhToan end DESC,
    			case when @SortBy <>'ASC' then 0
    			when @ColumnSort='KhachDaTra' then KhachDaTra end ASC,
    			case when @SortBy <>'DESC' then 0
    			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC			
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetTPDinhLuong_ofCTHD]
	@ID_CTHD [uniqueidentifier] = 'd106cf67-1858-4da2-bd0b-04de42a4eb6d',
	@LoaiHoaDon int = null
AS
BEGIN
    SET NOCOUNT ON;

	if @LoaiHoaDon is null
	begin
		
		select  MaHangHoa, TenHangHoa, ID_DonViQuiDoi, TenDonViTinh, SoLuong, ct.GiaVon, ct.ID_HoaDon,ID_ChiTietGoiDV, ct.ID_LoHang,
			ct.SoLuongDinhLuong_BanDau,
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoaThayThe,
    		case when ISNULL(hh.QuyCach,0) = 0 then ISNULL(qd.TyLeChuyenDoi,1) else ISNULL(hh.QuyCach,0) * ISNULL(qd.TyLeChuyenDoi,1) end as QuyCach,
    		qd.TenDonViTinh as DonViTinhQuyCach, ct.GhiChu	,
			ceiling(qd.GiaNhap) as GiaNhap, qd.GiaBan as GiaBanHH, lo.MaLoHang, lo.NgaySanXuat, lo.NgayHetHan ---- used to nhaphang tu hoadon
    	from BH_HoaDon_ChiTiet ct		
    	left Join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	left join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		left join DM_LoHang lo on ct.ID_LoHang = lo.ID
    	where ID_ChiTietDinhLuong = @ID_CTHD and ct.ID != @ID_CTHD 
		and ct.SoLuong > 0
		and (ct.ChatLieu is null or ct.ChatLieu !='5')

		
	end
	else
		-- hdxuatkho co Tpdinhluong
		begin	
		
			-- get thongtin hang xuatkho
			declare @ID_DonViQuiDoi uniqueidentifier, @ID_LoHang uniqueidentifier,  @ID_HoaDonXK uniqueidentifier
			select @ID_DonViQuiDoi= ID_DonViQuiDoi, @ID_LoHang= ID_LoHang, @ID_HoaDonXK = ct.ID_HoaDon
			from BH_HoaDon_ChiTiet ct 
			where ct.ID = @ID_CTHD 


			-- chi get dinhluong thuoc phieu xuatkho nay
			select ct.ID_ChiTietDinhLuong,ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
				ct.SoLuong, ct.DonGia, ct.GiaVon, ct.ThanhTien,ct.ID_HoaDon, ct.GhiChu, ct.ChatLieu,
				qd.MaHangHoa, qd.TenDonViTinh,
				lo.MaLoHang,lo.NgaySanXuat, lo.NgayHetHan,
				hh.TenHangHoa,
				qd.GiaBan,
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				qd.TenDonViTinh,
				qd.ID_HangHoa,
				hh.QuanLyTheoLoHang,
				hh.LaHangHoa,
				hh.DichVuTheoGio,
				hh.DuocTichDiem,
				ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan, 
				CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
				hh.ID_NhomHang as ID_NhomHangHoa, 
				ISNULL(hh.GhiChu,'') as GhiChuHH,
				iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoaThayThe
			from BH_HoaDon_ChiTiet ct
			Join DonViQuiDoi qd on ct.ID_ChiTietDinhLuong = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			left join DM_LoHang lo on ct.ID_LoHang= lo.ID
			where ct.ID_DonViQuiDoi= @ID_DonViQuiDoi 
			and ct.ID_HoaDon = @ID_HoaDonXK
			and ((ct.ID_LoHang = @ID_LoHang) or (ct.ID_LoHang is null and @ID_LoHang is null))			
			and (ct.ChatLieu is null or ct.ChatLieu !='5')
		end		
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[HDSC_GetChiTietXuatKho]
	@ID_HoaDon [uniqueidentifier] ='8312d0b9-54fd-491e-b105-89697eb6f66c',
    @IDChiTietHD [uniqueidentifier] = '1c15f83f-9ee4-41b0-ae25-5f57bf11640b',
    @LoaiHang [int] = 0
AS
BEGIN
    SET NOCOUNT ON;
	-- get loaihoadon
	declare @LoaiHoaDon int = (select top 1 LoaiHoaDon from BH_HoaDon where ID= @ID_HoaDon)
	if @LoaiHoaDon in (1,2,6)
	begin
		if	@LoaiHang = 1 -- hanghoa
		begin
			select 
    			qd.MaHangHoa, qd.TenDonViTinh,
    			hh.TenHangHoa,
				iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoaThayThe,
    			lo.MaLoHang,
    			ct.SoLuong,
    			ct.SoLuong* round(ct.GiaVon ,3) as GiaVon, ---- giatrixuat
    			hd.MaHoaDon,
    			hd.NgayLapHoaDon,
    			ct.GhiChu,
				ct.ChatLieu
    		from BH_HoaDon_ChiTiet ct
			join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    		left join DM_LoHang lo on ct.ID_LoHang= lo.ID
			WHERE ct.id= @IDChiTietHD
		end
		else
		begin
			----- xuat nguyenvatlieu
			select 
    				hh.TenHangHoa,
					hh.TenHangHoa as TenHangHoaThayThe,					
    				qd.MaHangHoa, qd.TenDonViTinh, qd.ThuocTinhGiaTri,
    				isnull(lo.MaLoHang,'') as MaLoHang,
    				tpdl.SoLuongDinhLuong_BanDau,
    				round(tpdl.GiaTriDinhLuong_BanDau,3) as GiaTriDinhLuong_BanDau ,
    				tpdl.MaHoaDon,
    				tpdl.NgayLapHoaDon	,
    				tpdl.SoLuongXuat as SoLuong,
    				round(tpdl.GiaTriXuat,3) as GiaVon,
    				tpdl.GhiChu,
    				tpdl.LaDinhLuongBoSung,
					tpdl.ChatLieu
    			from
    			(
						---- get tpdl ban dau
    						select 	
    							ctxk.MaHoaDon,
    							ctxk.NgayLapHoaDon,
    							ct.SoLuong as SoLuongDinhLuong_BanDau,
    							ct.SoLuong * ct.GiaVon as GiaTriDinhLuong_BanDau,
    							ct.ID_DonViQuiDoi, 
    							ct.ID_LoHang,
    							isnull(ctxk.SoLuongXuat,0) as SoLuongXuat,
    							isnull(ctxk.GiaTriXuat,0) as GiaTriXuat,
    							isnull(ctxk.GhiChu,'') as GhiChu,
    							0 as LaDinhLuongBoSung,
								ct.ChatLieu
    						from BH_HoaDon_ChiTiet ct
    						left join
    						(
    							---- get tpdl when xuatkho (ID_ChiTietGoiDV la hanghoa)
    							select 
    				
    									hd.MaHoaDon,
    									hd.NgayLapHoaDon,
    									ct.SoLuong as SoLuongXuat,
    									round(ct.SoLuong * ct.GiaVon,3) as GiaTriXuat,
    									ct.GhiChu,
    									ct.ID_ChiTietGoiDV
    							from BH_HoaDon_ChiTiet ct
    							join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    							where hd.ChoThanhToan='0' and hd.ID_HoaDon= @ID_HoaDon
								and hd.LoaiHoaDon in (8,35) --- sudung khi tra combo hdle
    						) ctxk on ct.ID= ctxk.ID_ChiTietGoiDV
    						where ct.ID_ChiTietDinhLuong= @IDChiTietHD
    						and ct.ID != ct.ID_ChiTietDinhLuong	
							
    			) tpdl
				left join DonViQuiDoi qd on qd.ID= tpdl.ID_DonViQuiDoi ----test thấy left join nhanh hơn nên dùng thôi
				left join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
				left join DM_LoHang lo on tpdl.ID_LoHang= lo.ID   		
    			order by tpdl.NgayLapHoaDon desc
		end
	end
	else
		begin	   
    		if	@LoaiHang = 1 -- hanghoa
    		begin
    		select 
    			qd.MaHangHoa, qd.TenDonViTinh,
    			hh.TenHangHoa,
				iif(pxk.TenHangHoaThayThe is null or pxk.TenHangHoaThayThe ='', hh.TenHangHoa, pxk.TenHangHoaThayThe) as TenHangHoaThayThe,
    			lo.MaLoHang,
    			pxk.SoLuong,
    			round(pxk.GiaVon ,3) as GiaVon,
    			pxk.MaHoaDon,
    			pxk.NgayLapHoaDon,
    			pxk.GhiChu,
				pxk.ChatLieu
    		from(
    			select 
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon,
    				ctxk.ID_DonViQuiDoi,
    				ctxk.ID_LoHang,
    				ctxk.SoLuong,
    				ctxk.SoLuong * ctxk.GiaVon as GiaVon,
    				ctxk.GhiChu,
					ctxk.TenHangHoaThayThe,
					ctxk.ChatLieu
    			from BH_HoaDon_ChiTiet ctxk
    			join BH_HoaDon hd on ctxk.ID_HoaDon= hd.ID
    			where (ctxk.ID_ChiTietGoiDV = @IDChiTietHD	
					or (ctxk.ID= @IDChiTietHD and ctxk.ChatLieu='1'))
    			and hd.ChoThanhToan='0'
    		) pxk
    		join DonViQuiDoi qd on pxk.ID_DonViQuiDoi= qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    		left join DM_LoHang lo on pxk.ID_LoHang= lo.ID  
    		end
    	else
    	begin    
    			select 
    				hh.TenHangHoa,
					hh.TenHangHoa as TenHangHoaThayThe,					
    				qd.MaHangHoa, qd.TenDonViTinh, qd.ThuocTinhGiaTri,
    				isnull(lo.MaLoHang,'') as MaLoHang,
    				tpdl.SoLuongDinhLuong_BanDau,
    				round(tpdl.GiaTriDinhLuong_BanDau,3) as GiaTriDinhLuong_BanDau ,
    				tpdl.MaHoaDon,
    				tpdl.NgayLapHoaDon	,
    				tpdl.SoLuongXuat as SoLuong,
    				round(tpdl.GiaTriXuat,3) as GiaVon,
    				tpdl.GhiChu,
    				tpdl.LaDinhLuongBoSung,
					tpdl.ChatLieu
    			from
    			(
    						---- get tpdl ban dau
    						select 	
    							ctxk.MaHoaDon,
    							ctxk.NgayLapHoaDon,
    							ct.SoLuong as SoLuongDinhLuong_BanDau,
    							ct.SoLuong * ct.GiaVon as GiaTriDinhLuong_BanDau,
    							ct.ID_DonViQuiDoi, 
    							ct.ID_LoHang,
    							isnull(ctxk.SoLuongXuat,0) as SoLuongXuat,
    							isnull(ctxk.GiaTriXuat,0) as GiaTriXuat,
    							isnull(ctxk.GhiChu,'') as GhiChu,
    							0 as LaDinhLuongBoSung,
								ct.ChatLieu
    						from BH_HoaDon_ChiTiet ct
    						left join
    						(
    							---- get tpdl when xuatkho (ID_ChiTietGoiDV la hanghoa)
    							select 
    				
    									hd.MaHoaDon,
    									hd.NgayLapHoaDon,
    									ct.SoLuong as SoLuongXuat,
    									round(ct.SoLuong * ct.GiaVon,3) as GiaTriXuat,
    									ct.GhiChu,
    									ct.ID_ChiTietGoiDV
    							from BH_HoaDon_ChiTiet ct
    							join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    							where hd.ChoThanhToan='0' and ct.ID_ChiTietGoiDV is not null
								and hd.LoaiHoaDon in (1,6,8,35,37,38,39,40) --- sudung khi tra combo hdle
    						) ctxk on ct.ID= ctxk.ID_ChiTietGoiDV
    						where ct.ID_ChiTietDinhLuong= @IDChiTietHD
    						and ct.ID != ct.ID_ChiTietDinhLuong				
    
    						---- get dinhluong them vao khi tao phieu xuatkho (ID_ChiTietGoiDV la dichvu)
    						union all
    
    						select 
    							hd.MaHoaDon,
    							hd.NgayLapHoaDon,
    							ct.SoLuong as SoLuongDinhLuong_BanDau,
    							ct.SoLuong * ct.GiaVon as GiaTriDinhLuong_BanDau,
    							ct.ID_DonViQuiDoi, 
    							ct.ID_LoHang,
    							isnull(ctxk.SoLuongXuat,0) as SoLuongXuat,
    							isnull(ctxk.GiaTriXuat,0) as GiaTriXuat,
    							isnull(ct.GhiChu,'') as GhiChu,
    							1 as LaDinhLuongBoSung,
								ct.ChatLieu
    						from BH_HoaDon_ChiTiet ct
    						join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    						left join
    						(
    							---- sum soluongxuat cua chinh no
    							select 
    									sum(ct.SoLuong) as SoLuongXuat,
    									sum(round(ct.SoLuong * ct.GiaVon,3)) as GiaTriXuat,
    									ct.ID_DonViQuiDoi
    							from BH_HoaDon_ChiTiet ct
    							join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    							where hd.ChoThanhToan='0'
    							and hd.LoaiHoaDon= 8 
    							and ct.ID_ChiTietGoiDV= @IDChiTietHD
    							group by ct.ID_DonViQuiDoi
    						) ctxk on ct.ID_DonViQuiDoi= ctxk.ID_DonViQuiDoi
    						where hd.ChoThanhToan='0'
    						and hd.LoaiHoaDon= 8 
    						and ct.ID_ChiTietGoiDV= @IDChiTietHD
    
    			) tpdl
    			join DonViQuiDoi qd on qd.ID= tpdl.ID_DonViQuiDoi
    			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    			left join DM_LoHang lo on tpdl.ID_LoHang= lo.ID
    			order by tpdl.NgayLapHoaDon desc
    		
    	end
		end
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhomHoTro]
    @IDChiNhanhs [nvarchar](max),
    @DateFrom [datetime],
    @DateTo [datetime],
    @IDNhomHoTros [nvarchar](max),
    @TextSearch [nvarchar](max),
	@IsVuotMuc tinyint, --1.all, 10. khong vuot, 11.vuot
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs) where Name!=''
    
    		declare @tblNhomHoTro table (ID uniqueidentifier)
    		if ISNULL(@IDNhomHoTros,'')!=''
    			insert into @tblNhomHoTro
    		select Name from dbo.splitstring(@IDNhomHoTros) where Name!=''
    		else
    			set @IDNhomHoTros =''
    
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    		DECLARE @count int;
    		if isnull(@TextSearch,'')!=''
    			INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    			Select @count =  (Select count(*) from @tblSearch);
    
    		----- giavon tieuchuan cua dichvu/sanpham da caidat ----
    		declare @tblGVTC table(ID_DonVi uniqueidentifier, ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    		GiaVonTieuChuan float, NgayLapHoaDon datetime)
    
    		insert into @tblGVTC		
    		select hd.ID_DonVi,
    			ct.ID_DonViQuiDoi, 
    			ct.ID_LoHang, 
    		ct.ThanhTien as GiaVonTieuChuan,    	
    		hd.NgayLapHoaDon
    		from BH_HoaDon_ChiTiet ct 
    		join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    		where hd.ChoThanhToan=0
    		and  hd.ID_DonVi in (select ID from @tblChiNhanh)
    		and hd.LoaiHoaDon= 16
    		and hd.NgayLapHoaDon < @DateTo	
    
    	
    
    			----- get khoang apdung hotro ----
    		select 
    			nhom.ID,
    			nhomdv.ID_DonVi,
    			khoangAD.GiaTriSuDungTu,
    			khoangAD.GiaTriSuDungDen,
    			khoangAD.GiaTriHoTro,
    			khoangAD.KieuHoTro
    		into #tblApDung
    		from DM_NhomHangHoa nhom
    		join NhomHangHoa_DonVi nhomdv on nhom.ID = nhomdv.ID_NhomHangHoa
    		join NhomHang_KhoangApDung khoangAD on nhom.ID= khoangAD.Id_NhomHang
    		where exists (select * from @tblChiNhanh cn where nhomdv.ID_DonVi = cn.ID)
    		and (nhom.TrangThai is null or nhom.TrangThai='0') ---- trangthainhom (0.đang dùng, 1.đã xóa)
    
    			------- get all sp thuocnhom hotro
    			select distinct
    				spht.Id_DonViQuiDoi,
    				spht.Id_LoHang,
    				spht.Id_NhomHang
    			into #tmpSPhamHT
    			from NhomHang_ChiTietSanPhamHoTro spht			
    			join NhomHangHoa_DonVi nhomCN on spht.Id_NhomHang = nhomCN.ID_NhomHangHoa
    			where spht.LaSanPhamNgayThuoc = 2
    			and exists (select ID from @tblChiNhanh cn where nhomCN.ID_DonVi = cn.ID)
    			and (@IDNhomHoTros='' or exists (select ID from @tblNhomHoTro nhomHT where nhomHT.ID = spht.Id_NhomHang))
    
    			
    			------ get all khachhang co phat sinh giao dịch from - to ----
    			select 	distinct		
    				hd.ID_DoiTuong,
    				hd.ID_DonVi,				
    				dt.ID_NhanVienPhuTrach,
    				dt.MaDoiTuong,
    				dt.TenDoiTuong
    			into #tblHDCus
    			from BH_HoaDon hd
    			join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			where hd.ChoThanhToan= 0
    			and hd.LoaiHoaDon in (1,2,6,19,22,23,36)
    			and hd.NgayLapHoaDon between @DateFrom and @DateTo
    			and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)	
    			and (@TextSearch ='' 
    				or
    					(select count(Name) from @tblSearch b where 
    						dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%' 
    						or dt.DienThoai like '%'+b.Name+'%' 
    					)=@count
    					or @count=0)
    		
    
    			----- get giatri sudung of cus (all time) ----
    			select 
    				hd.ID,
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon,
    				hd.ID_DoiTuong,
    				hd.ID_DonVi,
    				ct.ID_DonViQuiDoi,
    				spht.Id_NhomHang as ID_NhomHoTro,
    				ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as  GiaTriSuDung				
    			into #tblSuDung
    			from BH_HoaDon hd
    			join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
    			join #tmpSPhamHT spht on ct.ID_DonViQuiDoi = spht.Id_DonViQuiDoi  ----- chi get sp thuoc nhom hotro
    				and (ct.ID_LoHang = spht.Id_LoHang or ct.ID_LoHang is null and spht.Id_LoHang is null)
    			where exists (select ID_DoiTuong from #tblHDCus cus where hd.ID_DoiTuong = cus.ID_DoiTuong)
    			and hd.LoaiHoaDon in (1)
    			and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
    
    		
    		 
    
    		------ get GVTC of hoa don ------
    		select 
    			gvTC.*
    		into #tblHoTro
    		from
    		(
    				------ get gvtc theo khoang thoigian ----
    				select 
    					hd.ID as ID_HoaDon,
    					hd.ID_DoiTuong,
    					hd.ID_DonVi,
    					hd.ID_CheckIn as ID_NhomHoTro,		
    					hd.MaHoaDon,
    					hd.NgayLapHoaDon,
    					gv.NgayLapHoaDon as NgayDieuChinh,
    					isnull(gv.GiaVonTieuChuan,0) as GiaVonTieuChuan,
    					ISNULL(ct.SoLuong,0) * isnull(gv.GiaVonTieuChuan,0) as GiaTriDichVu,						
    				ISNULL(ct.SoLuong,0) AS SoLuongXuat,		
    					----- nếu có nhiều khoảng GVTC: ưu tiên lấy NgayDieuChinhGV gần nhất ----
    					ROW_NUMBER() over (partition by ct.ID order by gv.NgayLapHoaDon desc) as RN
    	
    			from BH_HoaDon hd 		
    			left join BH_HoaDon_ChiTiet ct on ct.ID_HoaDon= hd.ID		
    			left join @tblGVTC gv on hd.ID_DonVi= gv.ID_DonVi and ct.ID_DonViQuiDoi= gv.ID_DonViQuiDoi 
    				and (ct.ID_LoHang = gv.ID_LoHang or (ct.ID_LoHang is null and gv.ID_LoHang is null))
    			where hd.ChoThanhToan=0	
    			and hd.LoaiHoaDon = 36			
    			and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
    			and (hd.NgayLapHoaDon > gv.NgayLapHoaDon or gv.NgayLapHoaDon is null)		
    			and exists (select ID_DoiTuong from #tblHDCus cus where hd.ID_DoiTuong = cus.ID_DoiTuong)
    			and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
    			) gvTC
    			where gvTC.RN= 1 
    			and	(@IDNhomHoTros='' or exists (select ID from @tblNhomHoTro nhom where gvTC.ID_NhomHoTro = nhom.ID))			
    			order by gvTC.NgayLapHoaDon desc
    
    	
    
    		;with data_cte
    		as 
    		(
				select *
				from
				(
					select *,
						iif(PTramHoTro>100,'11','10') as IsVuotMuc
					from
					(
    					select    			
    						cus.MaDoiTuong,
    						cus.TenDoiTuong,
    						dv.TenDonVi,
    						nvpt.TenNhanVien,
    						tView.*,
    						--case 
    						--	when tView.GtriHoTroVND = 0 then tView.DaHoTro
    						--else tView.DaHoTro/tView.GtriHoTroVND *100
    						--end as PTramHoTro,
							case 
    							when tView.GtriHoTroVND = 0 then tView.DaHoTro
    						else tView.DaHoTro/tView.GiaTriSuDung *100
    						end as PTramHoTro,
    						tView.GiaTriHoTro as GtriHoTro_theoQuyDinh
    					from #tblHDCus cus
    					join (
    					select 
    						cusHT.*, 
    						nhom.TenNhomHangHoa as TenNhomHoTro,
    						kAD.GiaTriSuDungTu, 
    						kAD.GiaTriSuDungDen, 
    						kAD.GiaTriHoTro,
    						case kAD.KieuHoTro
    							when 1 then isnull(kAD.GiaTriHoTro,0) * cusHT.GiaTriSuDung/100
    							when 0 then isnull(kAD.GiaTriHoTro,0)
    						else 0 
    						end as GtriHoTroVND
    					from
    					(
    						select sd.ID_DoiTuong,
    							sd.ID_DonVi,		
    							sd.ID_NhomHoTro,			
    							sd.GiaTriSuDung,
    							isnull(ht.DaHoTro,0) as DaHoTro
    						from (
    							select ID_DoiTuong,
    								ID_DonVi,			
    								ID_NhomHoTro,
    								sum(GiaTriSuDung) as GiaTriSuDung
    							from #tblSuDung
    							group by ID_DoiTuong, ID_DonVi, ID_NhomHoTro
    						)  sd
    						left join  (
    							select ID_DoiTuong,		
    									ID_DonVi,				
    									ID_NhomHoTro,
    								sum(GiaTriDichVu) as DaHoTro
    							from #tblHoTro
    							group by ID_DoiTuong,ID_NhomHoTro,ID_DonVi	 
    						)ht on sd.ID_DoiTuong= ht.ID_DoiTuong and sd.ID_NhomHoTro = ht.ID_NhomHoTro and sd.ID_DonVi = ht.ID_DonVi
    					) cusHT
    					left join #tblApDung kAD on cusHT.ID_NhomHoTro = kAD.ID 
    						and cusHT.ID_DonVi = kAD.ID_DonVi 
    						and cusHT.GiaTriSuDung between kAD.GiaTriSuDungTu and kAD.GiaTriSuDungDen --- !!important: get khoang hotro
    					join DM_NhomHangHoa nhom on cusHT.ID_NhomHoTro = nhom.ID
    					) tView on cus.ID_DoiTuong = tView.ID_DoiTuong and cus.ID_DonVi= tView.ID_DonVi
    					left join DM_DonVi dv on tView.ID_DonVi = dv.ID
    					left join NS_NhanVien nvpt on cus.ID_NhanVienPhuTrach = nvpt.ID
						where tView.DaHoTro > 0 ---- chi lay khach  hotro
					) tblVuotMuc
				)tbLast where IsVuotMuc like concat('%' , @IsVuotMuc ,'%')
    		),
    		count_cte
    		as
    		(
    			select count(*) as TotalRow,
    				ceiling(count(*)/ CAST(@PageSize as float)) as TotalPage,
    				sum(GiaTriSuDung) as SumGiaTriSuDung,
    				sum(DaHoTro) as SumGiaTriHoTro
    			from data_cte
    		)
    		select *
    		from data_cte dt
    		cross join count_cte
    		order by dt.MaDoiTuong desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
    			
    
    			drop table #tblSuDung
    			drop table #tmpSPhamHT
    			drop table #tblHoTro
    			drop table #tblApDung
    			drop table #tblHDCus
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_HoaHongNhanVien]
    @ID_DonVi [uniqueidentifier],
	@ID_NhanVien [uniqueidentifier],
	@IDNhomHangs nvarchar(max),
	@TextSearch nvarchar(max),
	@CurrentPage int,
	@PageSize int
AS
BEGIN
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblNhomHang TABLE (ID uniqueidentifier);
	if @IDNhomHangs=''
		insert into @tblNhomHang
		select ID from DM_NhomHangHoa
	else
		insert into @tblNhomHang
		select Name from dbo.splitstring(@IDNhomHangs);
		
		with data_cte
		as (
			Select * ,
				concat(TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh) as TenHangHoaFull
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
    				hh.TenHangHoa,
    				hh.TenHangHoa_KhongDau,
    				hh.TenHangHoa_KyTuDau,				

					iif(dvqd.TenDonVitinh is null or dvqd.TenDonVitinh ='','', CONCAT('_',dvqd.TenDonViTinh)) as TenDonViTinh,
					iif(dvqd.ThuocTinhGiaTri is null or dvqd.ThuocTinhGiaTri ='','', CONCAT('_',dvqd.ThuocTinhGiaTri)) as ThuocTinh_GiaTri,

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
				AND ((select count(Name) from @tblSearchString b where 
    				hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
					or hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
					or dvqd.MaHangHoa like '%'+b.Name+'%'			   		
					)=@count or @count=0)	   	
    			) a
				where exists (select ID from @tblNhomHang nhom where a.ID_NhomHang = nhom.ID)
			),
			count_cte
			as (
				select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
				from data_cte
			)
			select *
			from data_cte dt
			cross join count_cte 
			order by dt.MaHangHoa
			OFFSET (@CurrentPage* @PageSize) ROWS
			FETCH NEXT @PageSize ROWS ONLY
    		
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[GetInforKhachHang_ByID]");
            DropStoredProcedure("[dbo].[BaoCao_DoanhThuKhachHang]");
            DropStoredProcedure("[dbo].[BCBanHang_theoMaDinhDanh]");
			Sql("DROP FUNCTION [dbo].[BuTruTraHang_HDDoi]");
			Sql("DROP FUNCTION [dbo].[GetTongTra_byIDHoaDon]");
        }
    }
}
