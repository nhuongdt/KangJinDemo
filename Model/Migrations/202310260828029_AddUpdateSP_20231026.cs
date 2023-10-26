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

			Sql(@"ALTER PROCEDURE [dbo].[GetInforHoaDon_ByID]
    @ID_HoaDon [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
    declare @IDHDGoc uniqueidentifier, @ID_DoiTuong uniqueidentifier, @LoaiHD int
    select @IDHDGoc = ID_HoaDon,  @ID_DoiTuong = ID_DoiTuong, @LoaiHD = LoaiHoaDon from BH_HoaDon where ID = @ID_HoaDon

   
    select 
    			hd.ID,
				hd.ID_NhanVien,
				hd.ID_BangGia,
				hd.ID_HoaDon,
				hd.ID_DoiTuong,
				hd.ID_BaoHiem,
				hd.ID_CheckIn, 
				hd.ID_ViTri,
				hd.ID_Xe,
				hd.ID_KhuyenMai,
    			hd.LoaiHoaDon,
    			hd.MaHoaDon,
    			hd.NgayLapHoaDon,
    			hd.ID_PhieuTiepNhan, 
    			hd.TongTienHang,
    			hd.ChoThanhToan,
				hd.YeuCau,
				hd.SoVuBaoHiem,
				hd.DiemGiaoDich,
				hd.TongChietKhau,
				hd.ChiPhi_GhiChu,
				
				ISNULL(hd.KhuyeMai_GiamGia,0) as KhuyeMai_GiamGia,
    			ISNULL(hd.TongGiamGia,0) + ISNULL(hd.KhuyeMai_GiamGia, 0) as TongGiamGia, 
    			ISNULL(hd.PhaiThanhToan,0)   as PhaiThanhToan,
				ISNULL(hd.TongThanhToan,0)  as TongThanhToan,    				    						
				iif(hd.LoaiHoaDon=6 or hd.LoaiHoaDon = 4, isnull(hd.TongChiPhi,0) , isnull(hd.ChiPhi,0)) as TongChiPhi, --- loai = 6: PhiTraHag (khach phai tra)
    
    			CAST(ISNULL(TienDoiDiem,0) as float) as TienDoiDiem,	
    			CAST(ISNULL(ThuTuThe,0) as float) as ThuTuThe,	
    			isnull(soquy.TienMat,0) as TienMat,
    			isnull(soquy.TienATM,0) as TienATM,
    			isnull(soquy.ChuyenKhoan,0) as ChuyenKhoan,
			
				cast(ISNULL(KhachDaTra,0)as float)  as KhachDaTra,

				cast(ISNULL(soquy.KhachDaTra,0) as float) as DaThanhToan,
    
    			dt.MaDoiTuong,
				dt.DienThoai,
				dt.Email,
				dt.DiaChi, 
				dt.MaSoThue,
				dt.TaiKhoanNganHang,

    			bh.TenDoiTuong as TenBaoHiem,
    			bh.MaDoiTuong as MaBaoHiem,
				isnull(bh.Email,'') as BH_Email,
    			isnull(bh.DiaChi,'') as BH_DiaChi,
    			isnull(bh.DienThoai,'') as DienThoaiBaoHiem,

				

    			ISNULL(dt.TenDoiTuong,N'Khách lẻ')  as TenDoiTuong,
    			ISNULL(bg.TenGiaBan,N'Bảng giá chung') as TenBangGia,
    			ISNULL(nv.TenNhanVien,N'')  as TenNhanVien,
    			ISNULL(dv.TenDonVi,N'')  as TenDonVi,    

    			case when hd.NgayApDungGoiDV is null then '' else  convert(varchar(14), hd.NgayApDungGoiDV ,103) end  as NgayApDungGoiDV,
    			case when hd.HanSuDungGoiDV is null then '' else  convert(varchar(14), hd.HanSuDungGoiDV ,103) end as HanSuDungGoiDV,

				---- get 2 trường này chỉ mục đích KhuyenMai thooi daay !!!---
				case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' 
					else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as IDNhomDoiTuongs,
				dt.NgaySinh_NgayTLap, 


    			hd.NguoiTao as NguoiTaoHD,
				hd.NguoiTao,
				hd.NgaySua,
				hd.NgayTao,
    			hd.DienGiai,
    			hd.ID_DonVi,
    			hd.TongTienThue,
    			isnull(hd.TongTienBHDuyet,0) as TongTienBHDuyet, 
    			isnull(hd.PTThueHoaDon,0) as PTThueHoaDon, 
    			isnull(hd.PTThueBaoHiem,0) as PTThueBaoHiem, 
    			isnull(hd.TongTienThueBaoHiem,0) as TongTienThueBaoHiem, 
    			isnull(hd.KhauTruTheoVu,0) as KhauTruTheoVu, 
				isnull(hd.CongThucBaoHiem,0) as  CongThucBaoHiem,
				hd.GiamTruThanhToanBaoHiem as  GiamTruThanhToanBaoHiem,

    			isnull(hd.PTGiamTruBoiThuong,0) as PTGiamTruBoiThuong, 
    			isnull(hd.GiamTruBoiThuong,0) as GiamTruBoiThuong, 
    			isnull(hd.BHThanhToanTruocThue,0) as BHThanhToanTruocThue, 
    			isnull(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem, 
				isnull(hd.TongThueKhachHang,0) as  TongThueKhachHang,

				cpVC.ID_NhaCungCap,
				iif(cpVC.ID_NhaCungCap = hd.ID_DoiTuong,0,isnull(cpVC.DaChi_BenVCKhac,0)) as DaChi_BenVCKhac, ---- nếu chính nó VC, đã chi VC = 0
				isnull(cpVC.TenDoiTuong,'') as TenNCCVanChuyen, 
    			isnull(cpVC.MaDoiTuong,'') as MaNCCVanChuyen, 
				ISNULL(baogia.MaHoaDon,'') as MaBaoGia,

    			
				case hd.ChoThanhToan
    				when 0 then '0'
    				when 1 then '1'
    				else '2' end as TrangThai,

				case hd.LoaiHoaDon
					when 3 then
						case hd.YeuCau
							when '1' then iif( hd.ID_PhieuTiepNhan is null, N'Phiếu tạm', iif(hd.ChoThanhToan='0',  N'Đã duyệt',N'Chờ duyệt'))
							when '2' then  N'Đang xử lý'
							when '3' then N'Hoàn thành'
						else N'Đã hủy' end
					else
						case hd.ChoThanhToan
							when 0 then N'Hoàn thành'
    						when 1 then N'Phiếu tạm'
    						else N'Đã hủy'
						end
				end as TrangThaiText 
    	from BH_HoaDon hd
    	left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    	left join DM_DonVi dv on hd.ID_DonVi= dv.ID
    	left join DM_GiaBan bg on hd.ID_BangGia= bg.ID
    	left join DM_DoiTuong bh on hd.ID_BaoHiem= bh.ID
		left join BH_HoaDon baogia on hd.ID_HoaDon= bg.ID 
		left join
		(
			select cp.ID_NhaCungCap,
				cp.ID_HoaDon,
				ncc.MaDoiTuong, 
				ncc.TenDoiTuong,
				isnull(TienThu,0)  as DaChi_BenVCKhac
			from BH_HoaDon_ChiPhi cp 
			join DM_DoiTuong ncc on cp.ID_NhaCungCap= ncc.ID
			left join
			(
				select 
					qct.ID_HoaDonLienQuan,   	
					qct.ID_DoiTuong,   
					sum(iif(qhd.LoaiHoaDon = 11,-qct.TienThu, qct.TienThu)) as TienThu
				from Quy_HoaDon qhd
				join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon= qhd.ID
				where qct.ID_HoaDonLienQuan = @ID_HoaDon
				and (qhd.TrangThai= 1 or qhd.TrangThai is null)				
				group by qct.ID_HoaDonLienQuan,  qct.ID_DoiTuong		
			) chiVC on chiVC.ID_DoiTuong = cp.ID_NhaCungCap 
			where cp.ID_HoaDon = @ID_HoaDon 
			and cp.ID_HoaDon_ChiTiet is null			
		) cpVC on hd.ID= cpVC.ID_HoaDon
    	left join 
    		(
    				select 
    					hdsq.ID,
    					sum(Khach_TienMat) as TienMat,
    					sum(Khach_TienPOS) as TienATM,
    					sum(Khach_TienCK) as ChuyenKhoan,
    					sum(Khach_TienDiem) as TienDoiDiem,
    					sum(Khach_TheGiaTri) as ThuTuThe,		
						sum(Khach_TienCoc ) as TienDatCoc,				
						sum(hdsq.KhachDaTra) as KhachDaTra,
						sum(hdsq.ThuDatHang) as ThuDatHang
    				from
    					(    									
							select 
								hdFist.ID,
								isnull(thuDH.Khach_TienMat,0) as Khach_TienMat,
								isnull(thuDH.Khach_TienPOS,0) as Khach_TienPOS,
								isnull(thuDH.Khach_TienCK,0) as Khach_TienCK,
								isnull(thuDH.Khach_TheGiaTri,0) as Khach_TheGiaTri,
								isnull(thuDH.Khach_TienDiem,0) as Khach_TienDiem,
								isnull(thuDH.Khach_TienCoc,0) as Khach_TienCoc,
								isnull(thuDH.TienThu,0) as KhachDaTra,
								isnull(thuDH.TienThu,0) as ThuDatHang
							from
							(
								------- 1. get list hoadon duoc xuly tu hdDatHang (neu loaiHD = 1,25) ----
							select hdXLy.Id,
								hdXLy.ID_HoaDon,
								hdXLy.NgayLapHoaDon,
								ROW_NUMBER() OVER(PARTITION BY hdXLy.ID_HoaDon ORDER BY hdXLy.NgayLapHoaDon ASC) AS isFirst	
							from BH_HoaDon hdXLy
							where hdXLy.ID_HoaDon = @IDHDGoc
							and hdXLy.ChoThanhToan='0'
							) hdFist
							left join
							(
								----2. get thuDathang ----
								select 
									qct.ID_HoaDonLienQuan as ID_DatHang,
									sum(iif(qct.HinhThucThanhToan=1, qct.TienThu, 0)) as Khach_TienMat,
									sum(iif(qct.HinhThucThanhToan=2, qct.TienThu, 0)) as Khach_TienPOS,
									sum(iif(qct.HinhThucThanhToan=3, qct.TienThu, 0)) as Khach_TienCK,
									sum(iif(qct.HinhThucThanhToan=4, qct.TienThu, 0)) as Khach_TheGiaTri,
									sum(iif(qct.HinhThucThanhToan=5, qct.TienThu, 0)) as Khach_TienDiem,
									sum(iif(qct.HinhThucThanhToan=6, qct.TienThu, 0)) as Khach_TienCoc,		
									sum(iif(qhd.LoaiHoaDon = 11,qct.TienThu, -qct.TienThu)) as TienThu	
								from Quy_HoaDon qhd
								join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon = qhd.ID
								where qct.ID_HoaDonLienQuan = @IDHDGoc
								and (qhd.TrangThai= 1 Or qhd.TrangThai is null)		
								group by qct.ID_HoaDonLienQuan

							)thuDH on thuDH.ID_DatHang= hdFist.ID_HoaDon
							where hdFist.isFirst = 1

						
							union all

								--- if hdDatHang: get thuChi all hdXuLy ---
							select 
								@ID_HoaDon as ID,
								sum(iif(qct.HinhThucThanhToan=1,iif(qct.ID_DoiTuong = @ID_DoiTuong, qct.TienThu,0), 0)) as Khach_TienMat,
								sum(iif(qct.HinhThucThanhToan=2, iif(qct.ID_DoiTuong = @ID_DoiTuong, qct.TienThu,0), 0)) as Khach_TienPOS,
								sum(iif(qct.HinhThucThanhToan=3, iif(qct.ID_DoiTuong = @ID_DoiTuong, qct.TienThu,0), 0)) as Khach_TienCK,
								sum(iif(qct.HinhThucThanhToan=4, iif(qct.ID_DoiTuong = @ID_DoiTuong, qct.TienThu,0), 0)) as Khach_TheGiaTri,
								sum(iif(qct.HinhThucThanhToan=5, iif(qct.ID_DoiTuong = @ID_DoiTuong, qct.TienThu,0), 0)) as Khach_TienDiem,
								sum(iif(qct.HinhThucThanhToan=6, iif(qct.ID_DoiTuong = @ID_DoiTuong, qct.TienThu,0), 0)) as Khach_TienCoc,		
								sum(iif(qhd.LoaiHoaDon = 11,iif(qct.ID_DoiTuong = @ID_DoiTuong, qct.TienThu,0), iif(qct.ID_DoiTuong = @ID_DoiTuong, -qct.TienThu,0))) as KhachDaTra,
								0 as ThuDatHang							
							from Quy_HoaDon qhd
							join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon = qhd.ID
							where (qhd.TrangThai= 1 Or qhd.TrangThai is null)		
							and exists (
								select hdXLy.Id,							
									hdXLy.NgayLapHoaDon
								from BH_HoaDon hdXLy
								where hdXLy.ID_HoaDon = @ID_HoaDon and qct.ID_HoaDonLienQuan = hdXLy.ID
								and hdXLy.ChoThanhToan='0'
								and hdXLy.LoaiHoaDon in (1,25, 4)
							)
							group by qct.ID_DoiTuong
    
    						union all

    					---- khach datra
    					select qct.ID_HoaDonLienQuan,	
							sum(iif(qct.HinhThucThanhToan=1, qct.TienThu, 0)) as Khach_TienMat,
							sum(iif(qct.HinhThucThanhToan=2, qct.TienThu, 0)) as Khach_TienPOS,
							sum(iif(qct.HinhThucThanhToan=3, qct.TienThu, 0)) as Khach_TienCK,
							sum(iif(qct.HinhThucThanhToan=4, qct.TienThu, 0)) as Khach_TheGiaTri,
							sum(iif(qct.HinhThucThanhToan=5, qct.TienThu, 0)) as Khach_TienDiem,
							sum(iif(qct.HinhThucThanhToan=6, qct.TienThu, 0)) as Khach_TienCoc,	
							sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, -qct.TienThu)) as KhachDaTra,
							0 as ThuDatHang							
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					where qhd.TrangThai= 1
    					and qct.ID_DoiTuong= @ID_DoiTuong and qct.ID_HoaDonLienQuan = @ID_HoaDon					
    					group by qct.ID_HoaDonLienQuan
        
    					
    				)hdsq group by hdsq.ID
    			) soquy on hd.ID = soquy.ID		
    	where hd.ID like @ID_HoaDon
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_Where]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max),
	@ID_NhanVienLogin nvarchar(max) = '',
	@NguoiTao nvarchar(max)='',
	@IDViTris nvarchar(max)='',
	@IDBangGias nvarchar(max)='',
	@TrangThai nvarchar(max)='0,1,2',
	@PhuongThucThanhToan nvarchar(max)='',
	@ColumnSort varchar(max)='NgayLapHoaDon',
	@SortBy varchar(max)= 'DESC',
	@CurrentPage int,
	@PageSize int
AS
BEGIN
	set nocount on;

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	 declare @tblNhanVien table (ID uniqueidentifier)
	 if isnull(@ID_NhanVienLogin,'') !=''
		begin
			insert into @tblNhanVien
			select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'HoaDon_XemDS_PhongBan','HoaDon_XemDS_HeThong');
		end

	declare @tblChiNhanh table (ID uniqueidentifier)
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh);

	declare @tblPhuongThuc table (PhuongThuc int)
	insert into @tblPhuongThuc
	select Name from dbo.splitstring(@PhuongThucThanhToan)
	

	declare @tblTrangThai table (TrangThaiHD tinyint primary key)
	insert into @tblTrangThai
	select Name from dbo.splitstring(@TrangThai);


	declare @tblViTri table (ID varchar(40))
	insert into @tblViTri
	select Name from dbo.splitstring(@IDViTris) where Name!=''

	declare @tblBangGia table (ID varchar(40))
	insert into @tblBangGia
	select Name from dbo.splitstring(@IDBangGias) where Name!=''
	
	if @timeStart='2016-01-01'		
		select @timeStart = min(NgayLapHoaDon) from BH_HoaDon where LoaiHoaDon=19
	;with data_cte
	as
	(
    SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
    	c.ID_ViTri,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,
		c.ID_Xe,
		xe.BienSo,
		c.ID_PhieuTiepNhan,
    	c.TheoDoi,
    	c.ID_DonVi,
    	c.ID_KhuyenMai,
    	c.ChoThanhToan,
    	c.MaHoaDon,  	
    	c.NgayLapHoaDon,
		ISNULL(c.MaDoiTuong,'') as MaDoiTuong,
    	c.TenDoiTuong,
    	c.Email,
    	c.DienThoai,
    	c.NguoiTaoHD,
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
		--c.TongThanhToan,
		c.PhaiThanhToan,		
		c.ThuTuThe, c.TienMat, c.TienATM,c.TienDoiDiem, c.ChuyenKhoan, c.KhachDaTra,c.TongChietKhau,c.TongTienThue,PTThueHoaDon,
		c.TongThueKhachHang,
		ID_TaiKhoanPos,
		ID_TaiKhoanChuyenKhoan,
    	c.TrangThaiText,
    	c.KhuyenMai_GhiChu,
    	c.KhuyeMai_GiamGia,
		c.LoaiHoaDonGoc,
		c.TongGiaTriTra,
    	iif(c.TongThanhToan1 =0 and c.PhaiThanhToan> 0, c.PhaiThanhToan, c.TongThanhToan1) as TongThanhToan,
				isnull(iif(c.ID_HoaDon is null,
					iif(c.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
						case when c.TongGiaTriTra > c.KhachNo then c.KhachNo						
						else c.TongGiaTriTra end),
					(select dbo.BuTruTraHang_HDDoi(ID_HoaDon,NgayLapHoaDon,ID_HoaDonGoc, LoaiHoaDonGoc))				
				),0) as LuyKeTraHang,
    	c.LoaiHoaDon,
    	c.DiaChiChiNhanh,
    	c.DienThoaiChiNhanh,
    	c.DiemGiaoDich,
    	c.DiemSauGD, -- add 02.08.2018 (bind InHoaDon)
    	c.HoaDon_HangHoa, -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	CONVERT(nvarchar(10),c.NgayApDungGoiDV,103) as NgayApDungGoiDV,
    	CONVERT(nvarchar(10),c.HanSuDungGoiDV,103) as HanSuDungGoiDV
		
    	FROM
    	(
    		select 
    		a.ID as ID,
    		hdXMLOut.HoaDon_HangHoa,
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
			bhhd.ID_Xe,
			bhhd.ID_PhieuTiepNhan,
    		bhhd.ChoThanhToan,
    		bhhd.ID_KhuyenMai,
    		bhhd.KhuyenMai_GhiChu,
    		bhhd.LoaiHoaDon,
			isnull(bhhd.PTThueHoaDon,0) as  PTThueHoaDon,
    		ISNULL(bhhd.KhuyeMai_GiamGia,0) AS KhuyeMai_GiamGia,
    		ISNULL(bhhd.DiemGiaoDich,0) AS DiemGiaoDich,
    		ISNULL(gb.ID,N'00000000-0000-0000-0000-000000000000') as ID_BangGia,
			ISNULL(vt.ID,N'00000000-0000-0000-0000-000000000000') as ID_ViTri,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,
    		bhhd.MaHoaDon,   		
    		bhhd.NgayLapHoaDon,
    		bhhd.NgayApDungGoiDV,
    		bhhd.HanSuDungGoiDV,
			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
    		ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau,
			ISNULL(dt.TenDoiTuong_ChuCaiDau, N'kl') as TenDoiTuong_ChuCaiDau,
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
    		bhhd.TongChietKhau,
			bhhd.TongThanhToan as TongThanhToan1,
			ISNULL(bhhd.TongThueKhachHang,0) as TongThueKhachHang,
			ISNULL(bhhd.TongTienThue,0) as TongTienThue,
			bhhd.TongTienHang,
			bhhd.TongGiamGia,
			bhhd.PhaiThanhToan,

			hdgoc.ID_HoaDon as ID_HoaDonGoc,
			isnull(hdgoc.LoaiHoaDon,0) as LoaiHoaDonGoc,
			hdgoc.MaHoaDon as MaHoaDonGoc,

			ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,
			ISNULL(allTra.NoTraHang,0) as NoTraHang,

    		a.ThuTuThe,
    		a.TienMat,
			a.TienATM,
			a.TienDoiDiem,
    		a.ChuyenKhoan,
    		a.KhachDaTra,
			ID_TaiKhoanPos,
			ID_TaiKhoanChuyenKhoan,

			ISNULL(bhhd.PhaiThanhToan,0) - ISNULL(a.KhachDaTra,0) as KhachNo,
    		
			case bhhd.ChoThanhToan
				when 1 then '1'
				when 0 then '0'
			else '4' end as TrangThaiHD,
    		Case When bhhd.ChoThanhToan = '1' then N'Phiếu tạm' when bhhd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThaiText,
			case when a.TienMat > 0 then
				case when a.TienATM > 0 then	
					case when a.ChuyenKhoan > 0 then
						case when a.ThuTuThe > 0 then '1,2,3,4' else '1,2,3' end												
						else 
							case when a.ThuTuThe > 0 then  '1,2,4' else '1,2' end end
						else
							case when a.ChuyenKhoan > 0 then 
								case when a.ThuTuThe > 0 then '1,3,4' else '1,3' end
								else 
										case when a.ThuTuThe > 0 then '1,4' else '1' end end end
				else
					case when a.TienATM > 0 then
						case when a.ChuyenKhoan > 0 then
								case when a.ThuTuThe > 0 then '2,3,4' else '2,3' end	
								else 
									case when a.ThuTuThe > 0 then '2,4' else '2' end end
							else 		
								case when a.ChuyenKhoan > 0 then
									case when a.ThuTuThe > 0 then '3,4' else '3' end
									else 
									case when a.ThuTuThe > 0 then '4' else '5' end end end end
									
						as PTThanhToan
    		FROM
    		(
    			Select 
    			b.ID,
    			SUM(ISNULL(b.ThuTuThe, 0)) as ThuTuThe,
    			SUM(ISNULL(b.TienMat, 0)) as TienMat,
				SUM(ISNULL(b.TienATM, 0)) as TienATM,
    			SUM(ISNULL(b.TienCK, 0)) as ChuyenKhoan,
				SUM(ISNULL(b.TienDoiDiem, 0)) as TienDoiDiem,
    			SUM(ISNULL(b.TienThu, 0)) as KhachDaTra,
				max(b.ID_TaiKhoanPos) as ID_TaiKhoanPos,
				max(b.ID_TaiKhoanChuyenKhoan) as ID_TaiKhoanChuyenKhoan
    			from
    			(
    				Select 
    				bhhd.ID,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.TienMat, 0) else ISNULL(hdct.TienMat, 0) * (-1) end end as TienMat,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then case when TaiKhoanPOS = 1 then ISNULL(hdct.TienGui, 0) else 0 end else ISNULL(hdct.TienGui, 0) * (-1) end end as TienATM,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = '11' then case when TaiKhoanPOS = 0 then ISNULL(hdct.TienGui, 0) else 0 end else ISNULL(hdct.TienGui, 0) * (-1) end end as TienCK,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.ThuTuThe, 0) else ISNULL(hdct.ThuTuThe, 0) * (-1) end end as ThuTuThe,
					case when qhd.TrangThai = 0 then 0 else case when qhd.LoaiHoaDon = 11 then 
							case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else ISNULL(hdct.Tienthu, 0) end
							else case when ISNULL(hdct.DiemThanhToan, 0) = 0 then 0 else -ISNULL(hdct.Tienthu, 0) end end end as TienDoiDiem,
    				Case when qhd.TrangThai = 0 then 0 else Case when qhd.LoaiHoaDon = '11' then ISNULL(hdct.Tienthu, 0) else ISNULL(hdct.Tienthu, 0) * (-1) end end as TienThu,
					case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 1 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanPos,
					case when qhd.TrangThai = 0 then '00000000-0000-0000-0000-000000000000' else case when TaiKhoanPOS = 0 then hdct.ID_TaiKhoanNganHang else '00000000-0000-0000-0000-000000000000' end end as ID_TaiKhoanChuyenKhoan
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet hdct on bhhd.ID = hdct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on hdct.ID_HoaDon = qhd.ID  
					left join DM_TaiKhoanNganHang tk on tk.ID= hdct.ID_TaiKhoanNganHang		
    				where bhhd.LoaiHoaDon = '19' and bhhd.NgayLapHoadon between @timeStart and @timeEnd
					and bhhd.ID_DonVi IN (Select * from splitstring(@ID_ChiNhanh))    
					and (isnull(@ID_NhanVienLogin,'')='' or exists( select * from @tblNhanVien nv where nv.ID= bhhd.ID_NhanVien) or bhhd.NguoiTao= @NguoiTao)
    			) b
    			group by b.ID 
    		) as a			
    		join BH_HoaDon bhhd on a.ID = bhhd.ID   	
			left join BH_HoaDon hdgoc on bhhd.ID_HoaDon= hdgoc.ID
			left join
			(
				------ all trahang of hdgoc ---
				select 					
					hdt.ID_HoaDon,					
					sum(hdt.PhaiThanhToan) as TongGtriTra,
					sum(hdt.PhaiThanhToan - isnull(chiHDTra.DaTraKhach,0)) as NoTraHang
				from BH_HoaDon hdt	
				left join
				(
					select 
						qct.ID_HoaDonLienQuan,
						sum(iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu)) as DaTraKhach
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qct.ID_HoaDonLienQuan
					where qhd.TrangThai='0'					
					group by qct.ID_HoaDonLienQuan
				) chiHDTra on hdt.ID = chiHDTra.ID_HoaDonLienQuan
				where hdt.LoaiHoaDon= 6
				and hdt.ChoThanhToan='0'
				group by hdt.ID_HoaDon		
			) allTra on allTra.ID_HoaDon = bhhd.ID
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
			left join Gara_DanhMucXe xe on c.ID_Xe= xe.ID
			where (@IDViTris ='' or exists (select ID from @tblViTri vt2 where vt2.ID= c.ID_ViTri))
			and (@IDBangGias ='' or exists (select ID from @tblBangGia bg where bg.ID= c.ID_BangGia))
			and exists (select TrangThaiHD from @tblTrangThai tt where c.TrangThaiHD= tt.TrangThaiHD)
		    and (@PhuongThucThanhToan ='' or exists(SELECT Name FROM splitstring(c.PTThanhToan) pt join @tblPhuongThuc pt2 on pt.Name = pt2.PhuongThuc))
			and	((select count(Name) from @tblSearch b where     			
				c.MaHoaDon like '%'+b.Name+'%'
				or c.NguoiTaoHD like '%'+b.Name+'%'				
				or c.TenNhanVien like '%'+b.Name+'%'
				or c.TenNhanVienKhongDau like '%'+b.Name+'%'
				or c.DienGiai like '%'+b.Name+'%'
				or c.MaDoiTuong like '%'+b.Name+'%'		
				or c.TenDoiTuong like '%'+b.Name+'%'
				or c.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or c.DienThoai like '%'+b.Name+'%'						
				or xe.BienSo like '%'+b.Name+'%'	
				or c.HoaDon_HangHoa like '%'+b.Name+'%'			
				)=@count or @count=0)	
				), 
				tblDebit as
				(
				select 
					cnLast.ID,
					---- hdDoi co CongNo  < tongtra --> butru = Luyketrahang + conngno
					iif (cnLast.LoaiHoaDonGoc != 6, cnLast.TongTienHDTra,
						iif(cnLast.TongGiaTriTra > cnLast.ConNo, cnLast.TongTienHDTra + cnLast.ConNo,cnLast.TongTienHDTra)) as TongTienHDTra,
					
					iif (cnLast.LoaiHoaDonGoc != 6, cnLast.ConNo,
						iif(cnLast.TongGiaTriTra > cnLast.ConNo, 0, cnLast.ConNo)) as ConNo
						
				from
				(
					select 
						c.ID,
						c.LoaiHoaDonGoc,
						c.TongGiaTriTra,
						iif(c.LoaiHoaDonGoc = 6, iif(c.LuyKeTraHang > 0, 0, abs(c.LuyKeTraHang)), c.LuyKeTraHang) as TongTienHDTra,
					
						iif(c.ChoThanhToan is null,0, 
							----- hdDoi co congno < tongtra							
							c.TongThanhToan 
								--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = 0							
								- iif(c.LoaiHoaDonGoc = 6, iif(c.LuyKeTraHang > 0, 0, abs(c.LuyKeTraHang)), c.LuyKeTraHang)
								- c.KhachDaTra ) as ConNo ---- ConNo = TongThanhToan - GtriBuTru
					from data_cte c
					) cnLast 
				),
			count_cte
		as (
			select count(dt.ID) as TotalRow,
				CEILING(COUNT(dt.ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,			
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumKhachDaTra,								
				sum(KhuyeMai_GiamGia) as SumKhuyeMai_GiamGia,								
				sum(PhaiThanhToan) as SumPhaiThanhToan,				
				sum(TongThanhToan) as SumTongThanhToan,
				sum(TienDoiDiem) as SumTienDoiDiem,
				sum(ThuTuThe) as SumThuTuThe,				
				sum(TienMat) as SumTienMat,
				sum(TienATM) as SumPOS,
				sum(ChuyenKhoan) as SumChuyenKhoan,				
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo
			from data_cte dt
			left join tblDebit cn on dt.ID= cn.ID
		)
		select dt.*, cte.*, cn.ConNo, cn.TongTienHDTra	
		from data_cte dt
		left join tblDebit cn on dt.ID= cn.ID
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

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonBanHang]	
    @timeStart [datetime] ='2023-08-01',
    @timeEnd [datetime]='2023-10-01',
    @ID_ChiNhanh [nvarchar](max)='8f01a137-e8ae-4239-ad96-4de67b2fec25',
    @maHD [nvarchar](max) ='',
	@ID_NhanVienLogin uniqueidentifier ='451fa98d-34e2-49f3-8abf-f93fd3924b45',
	@NguoiTao nvarchar(max)='admin',
	@IDViTris nvarchar(max)='',
	@IDBangGias nvarchar(max)='',
	@TrangThai nvarchar(max)='0,1,2,3',
	@PhuongThucThanhToan nvarchar(max)='',
	@ColumnSort varchar(max)='NgayLapHoaDon',
	@SortBy varchar(max)='DESC',
	@CurrentPage int=0,
	@PageSize int=10,
	@LaHoaDonSuaChua nvarchar(10)='1,36', ---- le + hdHoTro
	@BaoHiem int=3
AS
BEGIN

  set nocount on;
 --declare @tblNhanVien table (ID uniqueidentifier)
	--insert into @tblNhanVien
	--select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'HoaDon_XemDS_PhongBan','HoaDon_XemDS_HeThong');

	declare @tblChiNhanh table (ID varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh);

	declare @tblPhuongThuc table (PhuongThuc varchar(4))
	insert into @tblPhuongThuc
	select Name from dbo.splitstring(@PhuongThucThanhToan)

	declare @tblTrangThai table (TrangThaiHD varchar(2))
	insert into @tblTrangThai
	select Name from dbo.splitstring(@TrangThai);


	declare @tblViTri table (ID varchar(40))
	insert into @tblViTri
	select Name from dbo.splitstring(@IDViTris) where Name!=''

	declare @tblBangGia table (ID varchar(40))
	insert into @tblBangGia
	select Name from dbo.splitstring(@IDBangGias)where Name!=''

	declare @tblLoaiHoaDon table (Loai varchar(40))
	insert into @tblLoaiHoaDon
	select Name from dbo.splitstring(@LaHoaDonSuaChua)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	with data_cte
	as(
	select *,
		iif(c.ChoThanhToan is null, 0,iif( c.ConNo1 - c.TongTienHDTra > 0, c.ConNo1 - c.TongTienHDTra,0)) as ConNo
		from
		(
	select 
					hd.ID,
					hd.ID_DonVi,
					hd.ID_DoiTuong,
					hd.ID_HoaDon,
					hd.ID_BaoHiem,
					hd.ID_PhieuTiepNhan,
					hd.ID_KhuyenMai,
					hd.ID_NhanVien,
					hd.ID_Xe,
					hd.ChoThanhToan,
					hd.MaHoaDon,
					hd.LoaiHoaDon,
					hd.NgayLapHoaDon,
					hd.KhuyenMai_GhiChu,
					hd.KhuyeMai_GiamGia,
					hd.TongTienThue,
					isnull(hd.DiemGiaoDich,0) as DiemGiaoDich,
					isnull(hd.TongThueKhachHang,0) as  TongThueKhachHang,
					isnull(hd.CongThucBaoHiem,0) as  CongThucBaoHiem,
					isnull(hd.GiamTruThanhToanBaoHiem,0) as  GiamTruThanhToanBaoHiem,
					isnull(hd.PTThueHoaDon,0) as  PTThueHoaDon,
					isnull(hd.TongTienThueBaoHiem,0) as  TongTienThueBaoHiem,
					isnull(hd.TongTienBHDuyet,0) as  TongTienBHDuyet,
					isnull(hd.SoVuBaoHiem,0) as  SoVuBaoHiem,
					isnull(hd.PTThueBaoHiem,0) as  PTThueBaoHiem,
					isnull(hd.KhauTruTheoVu,0) as  KhauTruTheoVu,
					isnull(hd.GiamTruBoiThuong,0) as  GiamTruBoiThuong,
					isnull(hd.PTGiamTruBoiThuong,0) as  PTGiamTruBoiThuong,
					isnull(hd.BHThanhToanTruocThue,0) as  BHThanhToanTruocThue,
					ISNULL(hd.ID_BangGia,N'00000000-0000-0000-0000-000000000000') as ID_BangGia,
					ISNULL(hd.ID_ViTri,N'00000000-0000-0000-0000-000000000000') as ID_ViTri,

					CASE 
    					WHEN dt.TheoDoi IS NULL THEN 
    						CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    					ELSE dt.TheoDoi
    					END AS TheoDoi,

					dt.MaDoiTuong,
					dt.NgaySinh_NgayTLap,
					dt.MaSoThue,
					dt.TaiKhoanNganHang,
					ISNULL(dt.TongTichDiem,0) AS DiemSauGD,
					ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
					ISNULL(dt.Email, N'') as Email,
					ISNULL(dt.DienThoai, N'') as DienThoai,
					ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,
				
		
					dt.ID_TinhThanh, 
					dt.ID_QuanHuyen,
				
					ISNULL(nv.TenNhanVien, N'') as TenNhanVien,
    	

		
					hd.DienGiai,
					hd.NguoiTao as NguoiTaoHD,
					ISNULL(hd.TongChietKhau,0) as TongChietKhau,
					ISNULL(hd.TongTienHang,0) as TongTienHang,
					ISNULL(hd.ChiPhi,0) as TongChiPhi, --- chiphi cuahang phaitra
					iif(hd.LoaiHoaDon = 36,0,ISNULL(hd.TongGiamGia,0)) as TongGiamGia,
					iif(hd.LoaiHoaDon=36,ISNULL(hd.TongGiamGia,0),0) as SoNgayThuoc,
					ISNULL(hd.PhaiThanhToan,0) as PhaiThanhToan,
					ISNULL(hd.TongThanhToan,0) as TongThanhToan,
					ISNULL(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,
	
					iif(hd.ID_BaoHiem is null, 2, 1) as SuDungBaoHiem,
		
					ISNULL(hdSq.TienMat,0) as TienMat,
					ISNULL(hdSq.TienATM,0) as TienATM,
					ISNULL(hdSq.ChuyenKhoan,0) as ChuyenKhoan,
					ISNULL(hdSq.TienDoiDiem,0) as TienDoiDiem,
					ISNULL(hdSq.ThuTuThe,0) as ThuTuThe,
					ISNULL(hdSq.TienDatCoc,0) as TienDatCoc,
					ISNULL(hdSq.KhachDaTra,0) as KhachDaTra,
					ISNULL(hdSq.BaoHiemDaTra,0) as BaoHiemDaTra,
					ISNULL(hdSq.DaThanhToan,0) as DaThanhToan,
					ISNULL(hdSq.ThuDatHang,0) as ThuDatHang,

					ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
					Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
					iif(hdt.LoaiHoaDon=6,ISNULL(hdt.TongThanhToan,0),0) as TongTienHDTra, -- hdgoc: co the la baogia/hoactrahang

					cthd.GiamGiaCT,
					cthd.ThanhTienChuaCK,
					isnull(cthd.GiaTriSDDV,0) as GiaTriSDDV,

					
					

					ISNULL(hd.TongThanhToan,0) - ISNULL(hdSq.DaThanhToan,0) as ConNo1,
						Case When hd.ChoThanhToan = '1' then N'Phiếu tạm' when hd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThaiText,
						case  hd.ChoThanhToan
							when 1 then '1'
							when 0 then '0'
						else '4' end as TrangThaiHD,
						iif(hd.ID_PhieuTiepNhan is null, '0','1') as LaHoaDonSuaChua,
						case when hdSq.TienMat > 0 then
							case when hdSq.TienATM > 0 then	
								case when hdSq.ChuyenKhoan > 0 then
									case when hdSq.ThuTuThe > 0 then '1,2,3,4' else '1,2,3' end												
									else 
										case when hdSq.ThuTuThe > 0 then  '1,2,4' else '1,2' end end
									else
										case when hdSq.ChuyenKhoan > 0 then 
											case when hdSq.ThuTuThe > 0 then '1,3,4' else '1,3' end
											else 
													case when hdSq.ThuTuThe > 0 then '1,4' else '1' end end end
							else
								case when hdSq.TienATM > 0 then
									case when hdSq.ChuyenKhoan > 0 then
											case when hdSq.ThuTuThe > 0 then '2,3,4' else '2,3' end	
											else 
												case when hdSq.ThuTuThe > 0 then '2,4' else '2' end end
										else 		
											case when hdSq.ChuyenKhoan > 0 then
												case when hdSq.ThuTuThe > 0 then '3,4' else '3' end
												else 
												case when hdSq.ThuTuThe > 0 then '4' else '5' end end end end
									
									as PTThanhToan
				from
				(
		
	Select 
    					soquy.ID_HoaDonLienQuan,   				
						SUM(ISNULL(soquy.ThuTuThe, 0)) as ThuTuThe,
						SUM(ISNULL(soquy.TienMat, 0)) as TienMat,
						SUM(ISNULL(soquy.TienATM, 0)) as TienATM,
						SUM(ISNULL(soquy.TienCK, 0)) as ChuyenKhoan,
						SUM(ISNULL(soquy.TienDoiDiem, 0)) as TienDoiDiem,
						SUM(ISNULL(soquy.TienDatCoc, 0)) as TienDatCoc,
						SUM(ISNULL(soquy.TienThu, 0)) as DaThanhToan,
						SUM(ISNULL(soquy.KhachDaTra, 0)) as KhachDaTra,
						SUM(ISNULL(soquy.ThuDatHang, 0)) as ThuDatHang,
						SUM(ISNULL(soquy.BaoHiemDaTra, 0)) as BaoHiemDaTra
    				from
    				(
						Select 
							hd.ID as ID_HoaDonLienQuan,	
							iif(qhd.TrangThai='0',0, case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=1, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=1, -qct.TienThu,0) end) as TienMat,
							iif(qhd.TrangThai='0',0,case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=2, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=2, -qct.TienThu,0) end) as TienATM,
							iif(qhd.TrangThai='0',0,case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=3, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=3, -qct.TienThu,0) end) as TienCK,
							iif(qhd.TrangThai='0',0,case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=5, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=5, -qct.TienThu,0) end) as TienDoiDiem,
							iif(qhd.TrangThai='0',0,case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=4, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=4, -qct.TienThu,0) end) as ThuTuThe,
							iif(qhd.TrangThai='0',0,case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=6, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=6, -qct.TienThu,0) end) as TienDatCoc,
							iif(qhd.TrangThai='0',0,iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu)) as TienThu,
							iif(qhd.TrangThai='0',0,iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu)) as KhachDaTra,
							0 as ThuDatHang,
							cast (0 as float) as BaoHiemDaTra						
						from BH_HoaDon hd
						left join Quy_HoaDon_ChiTiet qct on hd.ID = qct.ID_HoaDonLienQuan	
						left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 			
						where hd.NgayLapHoadon between @timeStart and @timeEnd					
						and exists (select loai.Loai from @tblLoaiHoaDon loai where hd.LoaiHoaDon = loai.Loai)
						and exists (select cn.ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID) 											
					) soquy group by soquy.ID_HoaDonLienQuan
				) hdSq
			join BH_HoaDon hd on hdSq.ID_HoaDonLienQuan = hd.ID
			left join BH_HoaDon hdt on hd.ID_HoaDon = hdt.ID
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID 
			left join
			(
		
				select 
					cthd.ID_HoaDon,
					sum(GiamGiaCT) as GiamGiaCT,
					sum(ThanhTienChuaCK) as ThanhTienChuaCK,
					sum(GiaTriSDDV) as GiaTriSDDV
				from
				(
						------- cthd -----------
				select 
					ct.ID_HoaDon,
					ct.SoLuong * ct.TienChietKhau as GiamGiaCT,
					ct.SoLuong * ct.DonGia  as ThanhTienChuaCK,
					0 as GiaTriSDDV
				from BH_HoaDon hd
				join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon	
				where hd.NgayLapHoadon between @timeStart and @timeEnd					
						and exists (select loai.Loai from @tblLoaiHoaDon loai where hd.LoaiHoaDon = loai.Loai)
						and exists (select cn.ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID) 
					and	(ct.ID_ChiTietDinhLuong= ct.ID or ct.ID_ChiTietDinhLuong is null)
						and (ct.ID_ParentCombo= ct.ID or ct.ID_ParentCombo is null)		

				union all

				------ ctsudung ---
				select 
					ctsd.ID_HoaDon,
					0 as GiamGiaCT,
					0 as ThanhTienChuaCK,
					ctsd.SoLuong * (ct.DonGia - ct.TienChietKhau) * ( 1 -  gdv.TongGiamGia/iif(gdv.TongTienHang =0,1,gdv.TongTienHang))  as GiaTriSDDV
				from BH_HoaDon gdv 
				join BH_HoaDon_ChiTiet ct on ct.ID_HoaDon= gdv.ID and gdv.LoaiHoaDon = 19	
				join BH_HoaDon_ChiTiet ctsd on ctsd.ID_ChiTietGoiDV = ct.ID
				join BH_HoaDon hdsd on ctsd.ID_HoaDon= hdsd.ID
				where hdsd.NgayLapHoadon between @timeStart and @timeEnd					
					and exists (select loai.Loai from @tblLoaiHoaDon loai where hdsd.LoaiHoaDon = loai.Loai)
					and exists (select cn.ID from @tblChiNhanh cn where hdsd.ID_DonVi = cn.ID) 
				and	(ctsd.ID_ChiTietDinhLuong= ctsd.ID or ctsd.ID_ChiTietDinhLuong is null)		
				and ctsd.ID_ChiTietGoiDV is not null
				
				) cthd group by cthd.ID_HoaDon
			) cthd on hd.ID = cthd.ID_HoaDon
			where 
			(@IDViTris ='' or exists (select ID from @tblViTri vt2 where vt2.ID= hd.ID_ViTri))
			and (@IDBangGias ='' or exists (select ID from @tblBangGia bg where bg.ID= hd.ID_BangGia))
			and 
			((select count(Name) from @tblSearch b where     			
				hd.MaHoaDon like '%'+b.Name+'%'
				or hd.NguoiTao like '%'+b.Name+'%'				
				or nv.TenNhanVien like '%'+b.Name+'%'
				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
				or hd.DienGiai like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'		
				or dt.TenDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or dt.DienThoai like '%'+b.Name+'%'		
					
				)=@count or @count=0)	
			) as c
	WHERE (@BaoHiem= 3 or SuDungBaoHiem = @BaoHiem)
	and exists (select ID from @tblTrangThai tt where c.TrangThaiHD= tt.TrangThaiHD)
	and ( @PhuongThucThanhToan ='' or exists(SELECT Name FROM splitstring(c.PTThanhToan) pt join @tblPhuongThuc pt2 on pt.Name = pt2.PhuongThuc))
	),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumKhachDaTra,
				sum(DaThanhToan) as SumDaThanhToan,
				sum(BaoHiemDaTra) as SumBaoHiemDaTra,
				sum(KhuyeMai_GiamGia) as SumKhuyeMai_GiamGia,
				sum(TongChiPhi) as SumTongChiPhi,
				sum(TongTienHDTra) as SumTongTongTienHDTra,
				sum(PhaiThanhToan) as SumPhaiThanhToan,
				sum(PhaiThanhToanBaoHiem) as SumPhaiThanhToanBaoHiem,
				sum(TongThanhToan) as SumTongThanhToan,
				sum(TienDoiDiem) as SumTienDoiDiem,
				sum(ThuTuThe) as SumThuTuThe,
				sum(TienDatCoc) as SumTienCoc,
				sum(ThanhTienChuaCK) as SumThanhTienChuaCK,
				sum(GiamGiaCT) as SumGiamGiaCT,
				sum(TienMat) as SumTienMat,
				sum(TienATM) as SumPOS,
				sum(ChuyenKhoan) as SumChuyenKhoan,
				sum(GiaTriSDDV) as TongGiaTriSDDV,
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo,

				sum(TongTienThueBaoHiem) as SumTongTienThueBaoHiem,
				sum(TongTienBHDuyet) as SumTongTienBHDuyet,
				sum(KhauTruTheoVu) as SumKhauTruTheoVu,
				sum(GiamTruBoiThuong) as SumGiamTruBoiThuong,
				sum(BHThanhToanTruocThue) as SumBHThanhToanTruocThue
				
			from data_cte
		),
		tView
		as
		(
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
			when @ColumnSort='KhachDaTra' then KhachDaTra end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TienMat' then TienMat end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TienMat' then TienMat end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='ChuyenKhoan' then ChuyenKhoan end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='ChuyenKhoan' then ChuyenKhoan end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TienATM' then TienATM end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TienATM' then TienATM end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiaTriSDDV' then GiaTriSDDV end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiaTriSDDV' then GiaTriSDDV end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='ThuTuThe' then ThuTuThe end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='ThuTuThe' then ThuTuThe end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TienDatCoc' then TienDatCoc end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TienDatCoc' then TienDatCoc end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='BaoHiemDaTra' then BaoHiemDaTra end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='BaoHiemDaTra' then BaoHiemDaTra end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='PhaiThanhToanBaoHiem' then PhaiThanhToanBaoHiem end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='PhaiThanhToanBaoHiem' then PhaiThanhToanBaoHiem end DESC ,

			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TongTienThueBaoHiem' then TongTienThueBaoHiem end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TongTienThueBaoHiem' then TongTienThueBaoHiem end DESC,			
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='KhauTruTheoVu' then KhauTruTheoVu end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='KhauTruTheoVu' then KhauTruTheoVu end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='GiamTruBoiThuong' then GiamTruBoiThuong end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='GiamTruBoiThuong' then GiamTruBoiThuong end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='BHThanhToanTruocThue' then BHThanhToanTruocThue end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='BHThanhToanTruocThue' then BHThanhToanTruocThue end DESC,
			case when @SortBy <>'ASC' then 0
			when @ColumnSort='TongTienBHDuyet' then TongTienBHDuyet end ASC,
			case when @SortBy <>'DESC' then 0
			when @ColumnSort='TongTienBHDuyet' then TongTienBHDuyet end DESC					
			
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
		)
		select hd.*,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
			ISNULL(vt.TenViTri,'') as TenPhongBan,
			--cast ('0' as bit) as IsChuaXuatKho
			cast(iif(hdChuaXK.ID is null,'0','1') as bit) as IsChuaXuatKho
		from tView hd
		left join DM_DonVi dv on hd.ID_DonVi = dv.ID
		left join DM_TinhThanh tt on hd.ID_TinhThanh = tt.ID
		left join DM_QuanHuyen qh on hd.ID_QuanHuyen = qh.ID
		left join DM_GiaBan gb on hd.ID_BangGia = gb.ID
		left join DM_ViTri vt on hd.ID_ViTri = vt.ID
		left join 
		(
			select hd.ID , MaHoaDon,NgayLapHoaDon
			from tView hd
			where hd.LoaiHoaDon in (1,2,36) ---- banle, baohanh, hotro
			and hd.ChoThanhToan = 0
			and not exists
			 ---- hd chua co phieu xuatkho-----
				(select id from BH_HoaDon hdx
				where hdx.ID_HoaDon = hd.ID 
				and hdx.LoaiHoaDon in (35,37,38,39,40)
				and hdx.ChoThanhToan is not null
				and hdx.NgayLapHoaDon > @timeStart
				)
				and  exists (
				---- hd co cthd la hanghoa ---
					select * from BH_HoaDon_ChiTiet ct
					join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
					join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
					where  hh.LaHangHoa='1'
					and hd.ID= ct.ID_HoaDon
				)
			
		) hdChuaXK on hd.ID = hdChuaXK.ID

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

			Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoaXuatHuybyID]
    @ID_HoaDon [uniqueidentifier] ='D2D3F3BC-16E1-4946-8A5D-46697D83C2D9',
	@ID_ChiNhanh [uniqueidentifier] ='8F01A137-E8AE-4239-AD96-4DE67B2FEC25'
AS
BEGIN
  set nocount on;

		declare @loaiHD int, @ID_HoaDonGoc uniqueidentifier		
		select @loaiHD = LoaiHoaDon, @ID_HoaDonGoc= ID_HoaDon from BH_HoaDon where ID= @ID_HoaDon
		
		select 	
			 ctxk.ID,		
			ctxk.ID_DonViQuiDoi,
			ctxk.ID_LoHang,
			ctxk.ID_ChiTietGoiDV,
			ctxk.ID_ChiTietDinhLuong,
			ctxk.SoLuong,
			ctxk.DonGia,
			ctxk.GiaVon,
			ctxk.ThanhTien,
			ctxk.ChatLieu,
			ctxk.SoThuTu,
			ctxk.TienChietKhau,		
			ctxk.ID_HoaDon,
			ctxk.ThanhTien as GiaTriHuy,
			 ctxk.SoLuong as SoLuongXuatHuy,
			 ctxk.TienChietKhau as GiamGia,
			 hd.MaHoaDon,
			hd.NgayLapHoaDon,
			hd.ID_NhanVien,
    		--nv.TenNhanVien,
			dvqd.ID_HangHoa,
			dvqd.MaHangHoa,
			dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			lh.MaLoHang,
			Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
			lh.NgaySanXuat,
    		lh.NgayHetHan,    			
    		hh.TenHangHoa,
			Case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang, 
    		concat(hh.TenHangHoa , '', dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
			ctm.CountHD,
			---- duoc huy neu: xuatkho noibo, hdgoc tamluu, huy, hoac hdgoc chua huy nhung xuất kho bị douple
			cast(iif(hdm.ID is null, 0, ---- xuatkho noibo
				case hdm.ChoThanhToan
					when '1' then 1
					when '0' then iif(isnull(ctm.CountHD,0) > 1,1,0) --- capnhat hdmua: huy xuatkho cu --> ID_ChiTietGoiDV bi thay doi
				else 2 end) as float) as TrangThaiMoPhieu,				
			ROUND(ISNULL(tk.TonKho,0),2) as TonKho
		from BH_HoaDon_ChiTiet ctxk		
		join BH_HoaDon hd on hd.ID= ctxk.ID_HoaDon and hd.ID= @ID_HoaDon
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join BH_HoaDon hdm on hd.ID_HoaDon= hdm.ID
		left join 
		(	
			select 
				ctxk.ID_ChiTietGoiDV,
				count(ctxk.ID_ChiTietGoiDV) as CountHD
			from BH_HoaDon_ChiTiet ctxk 
			join BH_HoaDon hdxk on ctxk.ID_HoaDon = hdxk.ID
			join BH_HoaDon hdg on hdxk.ID_HoaDon = hdg.ID
			where hdg.ID = @ID_HoaDonGoc and hdxk.ChoThanhToan is not null
			group by ctxk.ID_ChiTietGoiDV
		) ctm on ctm.ID_ChiTietGoiDV = ctxk.ID_ChiTietGoiDV
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		where (hh.LaHangHoa = 1 and tk.TonKho is not null) 
		and (ctxk.ChatLieu is null or ctxk.ChatLieu != '5') 


	
END

");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonTraHang]
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
	@PageSize int
AS
BEGIN
	set nocount on;
	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'TraHang_XemDS_PhongBan','TraHang_XemDS_PhongBan');

	declare @tblChiNhanh table (ID varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@maHD, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch)


	;with data_cte
	as(
	
    SELECT 
    	c.ID,
    	c.ID_BangGia,
    	c.ID_HoaDon,
		c.ID_Xe,
    	c.LoaiHoaDon,
    	c.ID_ViTri,
    	c.ID_DonVi,
    	c.ID_NhanVien,
    	c.ID_DoiTuong,		
    	c.ChoThanhToan,
    	c.MaHoaDon,
    	c.BienSo,
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
		c.KhuyeMai_GiamGia,
		c.PhaiThanhToan,		
		c.TongChiPhi,
		c.KhachDaTra, 
		c.TongThanhToan,
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
		c.ID_BaoHiem, c.ID_PhieuTiepNhan,
		c.TongTienBHDuyet, PTThueHoaDon, c.PTThueBaoHiem, c.TongTienThueBaoHiem, c.SoVuBaoHiem,
		c.KhauTruTheoVu, c.PTGiamTruBoiThuong,
		c.GiamTruBoiThuong, c.BHThanhToanTruocThue,
		c.PhaiThanhToanBaoHiem,				
    	'' as HoaDon_HangHoa -- string contail all MaHangHoa,TenHangHoa of HoaDon
    	FROM
    	(
    		select 
    	
    		a.ID as ID,
    		bhhd.MaHoaDon,
    		bhhd.LoaiHoaDon,
    		bhhd.ID_BangGia,
    		bhhd.ID_HoaDon,
    		bhhd.ID_ViTri,
    		bhhd.ID_DonVi,
    		bhhd.ID_NhanVien,
    		bhhd.ID_DoiTuong,
			

    		ISNULL(bhhd.DiemGiaoDich,0) as DiemGiaoDich,
    		bhhd.ChoThanhToan,
    		ISNULL(vt.TenViTri,'') as TenPhongBan,

    		bhhd.NgayLapHoaDon,
    		CASE 
    			WHEN dt.TheoDoi IS NULL THEN 
    				CASE WHEN dt.ID IS NULL THEN '0' ELSE '1' END
    			ELSE dt.TheoDoi
    		END AS TheoDoi,

			dt.MaDoiTuong,
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,
			ISNULL(dt.TenDoiTuong_KhongDau, N'Khách lẻ') as TenDoiTuong_KhongDau,
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
    		CAST(ROUND(bhhd.TongTienHang, 0) as float) as TongTienHang,
    		CAST(ROUND(bhhd.TongGiamGia, 0) as float) as TongGiamGia,
			isnull(bhhd.KhuyeMai_GiamGia,0) as KhuyeMai_GiamGia,
    		CAST(ROUND(bhhd.TongChiPhi, 0) as float) as TongChiPhi,
    		CAST(ROUND(bhhd.PhaiThanhToan, 0) as float) as PhaiThanhToan,
			CAST(ROUND(bhhd.TongTienThue, 0) as float) as TongTienThue,
			isnull(bhhd.TongThanhToan, bhhd.PhaiThanhToan) as TongThanhToan,

			bhhd.ID_BaoHiem, bhhd.ID_PhieuTiepNhan,bhhd.ID_Xe,
			xe.BienSo,
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
    		a.KhachDaTra,
    		a.ThuTuThe,
    		a.TienMat,
    		a.ChuyenKhoan,
    		bhhd.TongChietKhau,			
			case bhhd.ChoThanhToan
				when 0 then 0
				when 1 then 1
				else 4 end as TrangThaiHD,   
    		Case When bhhd.ChoThanhToan = 0 then N'Hoàn thành' else N'Đã hủy' end as TrangThai
    		FROM
    		(
    			select a1.ID, 
					sum(KhachDaTra) as KhachDaTra,
					sum(ThuTuThe) as ThuTuThe,
					sum(TienMat) as TienMat,
					sum(TienPOS) as TienATM,
					sum(TienCK) as ChuyenKhoan
				from (
					Select 
    				bhhd.ID,					
					case when qhd.TrangThai ='0' then 0 else ISNULL(qct.Tienthu, 0) end as KhachDaTra,
					Case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=4, isnull(qct.TienThu,0),0) end as ThuTuThe,
					case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=1, isnull(qct.TienThu,0),0) end as TienMat,										
					case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=2, isnull(qct.TienThu,0),0) end as TienPOS,
					case when qhd.TrangThai = 0 then 0 else iif(qct.HinhThucThanhToan=3, isnull(qct.TienThu,0),0) end as TienCK							
    				from BH_HoaDon bhhd
    				left join Quy_HoaDon_ChiTiet qct on bhhd.ID = qct.ID_HoaDonLienQuan	
    				left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
    				where bhhd.LoaiHoaDon = 6
					and bhhd.NgayLapHoadon between  @timeStart and @timeEnd 
					and bhhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
				) a1 group by a1.ID
    		) as a
    		left join BH_HoaDon bhhd on a.ID = bhhd.ID	
    		left join DM_DoiTuong dt on bhhd.ID_DoiTuong = dt.ID
    		left join DM_DonVi dv on bhhd.ID_DonVi = dv.ID
    		left join NS_NhanVien nv on bhhd.ID_NhanVien = nv.ID 
    		left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    		left join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    		left join DM_GiaBan gb on bhhd.ID_BangGia = gb.ID
    		left join DM_ViTri vt on bhhd.ID_ViTri = vt.ID    		
			left join Gara_DanhMucXe xe on bhhd.ID_Xe = xe.ID
    		) as c
			join (select Name from dbo.splitstring(@TrangThai)) tt on c.TrangThaiHD = tt.Name
			where (exists( select * from @tblNhanVien nv where nv.ID= c.ID_NhanVien) or c.NguoiTaoHD= @NguoiTao)
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
				
				)=@count or @count=0)	
			), 
			count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(TongTienHang) as SumTongTienHang,
				sum(TongGiamGia) as SumTongGiamGia,
				sum(KhachDaTra) as SumKhachDaTra,	
				sum(PhaiThanhToan) as SumPhaiThanhToan,			
				sum(TongChiPhi) as SumTongChiPhi,				
				sum(ThuTuThe) as SumThuTuThe,				
				sum(TienMat) as SumTienMat,			
				sum(ChuyenKhoan) as SumChuyenKhoan,				
				sum(TongTienThue) as SumTongTienThue
			from data_cte
		),
		tblView as
		(
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
    	)
		----- select top 10 -----
		select *
		into #tblView
		from tblView

		----- get list ID of top 10
		declare @tblID TblID
		insert into @tblID
		select ID from #tblView
		
		------ get congno of top 10
		declare @tblCongNo table (ID uniqueidentifier, MaHoaDonGoc nvarchar(max), LoaiHoaDonGoc int, HDDoi_PhaiThanhToan float, BuTruHDGoc_Doi float)
		insert into @tblCongNo
		exec TinhCongNo_HDTra @tblID, 6
					
		
		select tView.*,
			cn.MaHoaDonGoc,
			cn.LoaiHoaDonGoc,
			isnull(cn.BuTruHDGoc_Doi,0) as TongTienHDDoiTra,
			tView.PhaiThanhToan - isnull(cn.BuTruHDGoc_Doi,0) as TongTienHDTra, --- muontruong: PhaiTraKhach (sau khi butru congno hdGoc & hdDoi)
			tView.PhaiThanhToan - isnull(cn.BuTruHDGoc_Doi,0) - tView.KhachDaTra as ConNo
		from #tblView tView
		left join @tblCongNo cn on tView.ID = cn.ID
		order by tView.NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetBaoCaoCongNoChiTiet]
    @IDChiNhanhs [nvarchar](max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @DateFrom [datetime] = '2023-08-01',
    @DateTo [datetime] ='2023-12-01',
    @TextSearch [nvarchar](max) = '',
    @TrangThais [nvarchar](4) = '',
    @CurrentPage [int] = 0,
    @PageSize [int] = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID varchar(40))
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs)
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    
    	;with data_cte
    	as
    	(
		select c.* ,
				iif(c.KhachDaTra - c.GiaTriSD > 0,0, c.GiaTriSD - c.KhachDaTra) as NoThucTe1,
					------ những hóa đơn lâu đời, chưa có trường TongThanhToan = 0/null --> assign TongThanhToan = PhaiThanhToan ---
					iif(c.TongThanhToan1 =0 and c.PhaiThanhToan> 0, c.PhaiThanhToan, c.TongThanhToan1) as TongThanhToan,
					isnull(iif(c.LoaiHoaDonGoc = 3 or c.ID_HoaDon is null,
						iif(c.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
							case when c.TongGiaTriTra > c.KhachNo then c.KhachNo						
							else c.TongGiaTriTra end),
						(select dbo.BuTruTraHang_HDDoi(ID_HoaDon,NgayLapHoaDon,ID_HoaDonGoc, LoaiHoaDonGoc))				
					),0) as LuyKeTraHang
			
				from
				(
    	select  
    		hd.ID,
			hd.ID_HoaDon,
    		hd.MaHoaDon,
    		hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
			hd.PhaiThanhToan,
    		hd.TongThanhToan as TongThanhToan1,
    		hd.DienGiai,
    		dt.MaDoiTuong,
    		dt.TenDoiTuong,
    		dv.TenDonVi,
			hd.ChoThanhToan,
			0 as BaoHiemDaTra,
			nvpt.TenNhanVien as NVPhuTrach,    	
			ISNULL(hd.PhaiThanhToan,0) - ISNULL(soquy.KhachDaTra,0) as KhachNo,
    		isnull(soquy.KhachDaTra,0) as KhachDaTra,
    		isnull(sdGDV.GiaTriSD,0) as GiaTriSD,
    		iif(hd.TongThanhToan - isnull(soquy.KhachDaTra,0) > 0,1,0) as TrangThaiCongNo,
			isnull(hdgoc.LoaiHoaDon,0) as LoaiHoaDonGoc,
			hdgoc.ID_HoaDon as ID_HoaDonGoc,					
			hdgoc.MaHoaDon as MaHoaDonGoc,
			ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,
			ISNULL(allTra.NoTraHang,0) as NoTraHang
    	from BH_HoaDon hd	
    	join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    	join DM_DonVi dv on hd.ID_DonVi = dv.ID
		left join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID    	
    	left join
    	(
    		Select 
    			tblUnion.ID_HoaDonLienQuan,			
    			SUM(ISNULL(tblUnion.TienThu, 0)) as KhachDaTra			
    			from
    			(		------ thanhtoan itseft ----			
    					Select 
    						hd.ID as ID_HoaDonLienQuan,
    						iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as TienThu				
    					from BH_HoaDon hd
    					join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 					
    					where (qhd.TrangThai is null or qhd.TrangThai='1')			
    					and hd.LoaiHoaDon in (1,19,22)
    					and hd.ChoThanhToan = '0'				
    					and hd.NgayLapHoaDon between @DateFrom and @DateTo
    					and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    
    					
    					Union all
    
    					----- thanhtoan when dathang -----
    					Select
    						thuDH.ID,				
    						thuDH.TienThu
    					FROM
    					(
    						Select 
    								ROW_NUMBER() OVER(PARTITION BY d.ID_HoaDon ORDER BY d.NgayLapHoaDon ASC) AS isFirst,						
    							d.ID,
    								d.ID_HoaDon,
    								d.NgayLapHoaDon,    						
    							sum(d.TienThu) as TienThu
    						FROM 
    						(
    					
    							Select
    							hd.ID,
    							hd.NgayLapHoaDon,
    							hdd.ID as ID_HoaDon,						
    							iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu) as TienThu											
    							from BH_HoaDon hd 
    							join BH_HoaDon hdd on hd.ID_HoaDon= hdd.ID and hdd.LoaiHoaDon= 3
    							join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDonLienQuan = hdd.ID
    							join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
    							where hd.LoaiHoaDon in (1,19,22)
    							and hd.ChoThanhToan = '0'				
    							and hd.NgayLapHoaDon between @DateFrom and @DateTo
    							and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    						
    						)  d group by d.ID,d.NgayLapHoaDon,ID_HoaDon		
    					) thuDH where isFirst= 1
    			) tblUnion
    			group by tblUnion.ID_HoaDonLienQuan
    	) soquy on hd.ID= soquy.ID_HoaDonLienQuan
    	left join(
    		------ sudung gdv
    		select 
    			gdv.ID,
    			sum(ctsd.SoLuong * (ctsd.DonGia - ctsd.TienChietKhau)) as GiaTriSD
    		from BH_HoaDon gdv
    		join BH_HoaDon_ChiTiet ctm on gdv.ID = ctm.ID_HoaDon
    		 join BH_HoaDon_ChiTiet ctsd on ctm.ID= ctsd.ID_ChiTietGoiDV 
    		 join BH_HoaDon hdsd on ctsd.ID_HoaDon= hdsd.ID 
    		where gdv.LoaiHoaDon= 19 and gdv.ChoThanhToan='0'
    		and hdsd.LoaiHoaDon = 1 and hdsd.ChoThanhToan ='0'
    		and (ctsd.ID_ChiTietDinhLuong is null or ctsd.ID_ChiTietDinhLuong = ctsd.ID)
    		group by gdv.ID
    	) sdGDV on hd.ID = sdGDV.ID    
		left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID
		left join
			(
				------ all trahang of hdgoc ---
				select 					
					hdt.ID_HoaDon,					
					sum(hdt.PhaiThanhToan) as TongGtriTra,
					sum(hdt.PhaiThanhToan - isnull(chiHDTra.DaTraKhach,0)) as NoTraHang
				from BH_HoaDon hdt	
				left join
				(
					select 
						qct.ID_HoaDonLienQuan,
						sum(iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu)) as DaTraKhach
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qct.ID_HoaDonLienQuan
					where qhd.TrangThai='0'					
					group by qct.ID_HoaDonLienQuan
				) chiHDTra on hdt.ID = chiHDTra.ID_HoaDonLienQuan
				where hdt.LoaiHoaDon= 6
				and hdt.ChoThanhToan='0'
				group by hdt.ID_HoaDon		
			) allTra on allTra.ID_HoaDon = hd.ID
    	where hd.LoaiHoaDon in (1,19,22)
    	and hd.ChoThanhToan = '0'
    	and hd.TongThanhToan > 0
    	and hd.NgayLapHoaDon between @DateFrom and @DateTo
    	and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    	and ((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'	
    					or hd.MaHoaDon like '%' +b.Name +'%' 		
    					)=@count or @count=0)	
    	--) tbl where (@TrangThais ='' or tbl.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))
    	) c	where (@TrangThais ='' or c.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))	 		
    	),
		tblDebit as
		(
			select 
				cnLast.ID,
				---- hdDoi co CongNo  < tongtra --> butru = Luyketrahang + conngno
				iif (cnLast.LoaiHoaDonGoc != 6, cnLast.TongTienHDTra,
					iif(cnLast.TongGiaTriTra > cnLast.ConNo, cnLast.TongTienHDTra + cnLast.ConNo,cnLast.TongTienHDTra)) as TongTienHDTra,
					
				iif (cnLast.LoaiHoaDonGoc != 6, cnLast.ConNo,
					iif(cnLast.TongGiaTriTra > cnLast.ConNo, 0, cnLast.ConNo)) as ConNo
						
			from
			(
				select 
					c.ID,
					c.LoaiHoaDonGoc,
					c.TongGiaTriTra,
					iif(c.LoaiHoaDonGoc = 6, iif(c.LuyKeTraHang > 0, 0, abs(c.LuyKeTraHang)), c.LuyKeTraHang) as TongTienHDTra,
					iif(c.ChoThanhToan is null,0, 
						c.TongThanhToan 
							------ neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = 0
							- iif(c.LoaiHoaDonGoc = 6, iif(c.LuyKeTraHang > 0, 0, abs(c.LuyKeTraHang)), c.LuyKeTraHang)
							- c.KhachDaTra - c.BaoHiemDaTra) as ConNo ---- ConNo = TongThanhToan - GtriBuTru
				from data_cte c
			) cnLast 
		),
    	count_cte as
    	 (
    		select count(dt.ID) as TotalRow,
    			CEILING(COUNT(dt.ID) / CAST(@PageSize as float ))  as TotalPage,
    			sum(TongThanhToan) as TongThanhToanAll,
    			sum(KhachDaTra) as KhachDaTraAll,
    			sum(cn.ConNo) as ConNoAll --,
    			--sum(TongThanhToan - cn.NoThucTe1) as NoThucTeAll
    			from data_cte dt
				left join tblDebit cn on dt.ID= cn.ID
    ),
	tView
	as
	(
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
	)
	select *,
		hd.TongThanhToan - isnull(cn.TongTienHDTra,0) as GiaTriSauTra,
		ISNULL(qtCN.GiaTriTatToan,0) as GiaTriTatToan,
		iif(hd.LoaiHoaDon=22, cn.ConNo - ISNULL(qtCN.GiaTriTatToan,0),cn.ConNo) as ConNo,
		iif(hd.LoaiHoaDon=22, hd.NoThucTe1 - ISNULL(qtCN.GiaTriTatToan,0),hd.NoThucTe1) as NoThucTe,
		tblNV.TenNhanViens
	from tView hd
	left join tblDebit cn on hd.ID= cn.ID
	left join
    	(
    			Select distinct
    			(
    				Select concat( nv.TenNhanVien ,' (',th.PT_ChietKhau, '%) ,') AS [text()]
    				From dbo.BH_NhanVienThucHien th
    				join dbo.NS_NhanVien nv on th.ID_NhanVien = nv.ID
    				where th.ID_HoaDon= nvth.ID_HoaDon
    				For XML PATH ('')
    			) TenNhanViens, 
    				nvth.ID_HoaDon
    			From dbo.BH_NhanVienThucHien nvth
    	) tblNV on tblNV.ID_HoaDon = hd.ID
	left join
	(
		select hd.ID_HoaDon,
			sum(hd.PhaiThanhToan) as GiaTriTatToan
		from BH_HoaDon hd
		where hd.ChoThanhToan='0'
		and LoaiHoaDon= 42
		group by hd.ID_HoaDon
	) qtCN on hd.ID= qtCN.ID_HoaDon
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_afterUseAndTra]
 --declare 
 @IDChiNhanhs nvarchar(max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
   @IDNhanViens nvarchar(max) = null,
   @DateFrom datetime = '2023-07-01',
   @DateTo datetime = null,
   @TextSearch nvarchar(max) = 'GDV0000000040',
   @CurrentPage int =0,
   @PageSize int = 10
AS
BEGIN

	if isnull(@CurrentPage,'') =''
			set @CurrentPage = 0
		if isnull(@PageSize,'') =''
		set @PageSize = 30

		if isnull(@DateFrom,'') =''
		begin	
			set @DateFrom = '2016-01-01'		
		end

		if isnull(@DateTo,'') =''
		begin		
			set @DateTo = DATEADD(day, 1, getdate())		
		end
		else
		set @DateTo = DATEADD(day, 1, @DateTo)

			DECLARE @tblChiNhanh table (ID uniqueidentifier primary key)
			if isnull(@IDChiNhanhs,'') !=''
				insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs)		
			else
				set @IDChiNhanhs =''

			DECLARE @tblSearch TABLE (Name [nvarchar](max))
			DECLARE @count int
			INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!=''
			select @count =  (Select count(*) from @tblSearch)

	; with data_cte
	as
	(
			SELECT 
				hd.ID,
				hd.MaHoaDon,
				hd.LoaiHoaDon,
				hd.NgayLapHoaDon,   						
				hd.ID_DoiTuong,	
				hd.ID_HoaDon,
				hd.ID_BangGia,
				hd.ID_NhanVien,
				hd.ID_DonVi,
				hd.ID_Xe,
				hd.ID_PhieuTiepNhan,
				hd.ID_BaoHiem,
				hd.NguoiTao,	
				hd.DienGiai,	
				dt.MaDoiTuong,
				dt.TenDoiTuong,
				xe.BienSo,
				iif(hd.TongThanhToan =0 or hd.TongThanhToan is null,  hd.PhaiThanhToan, hd.TongThanhToan) as TongThanhToan,
				ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
				ISNULL(hd.TongTienHang, 0) as TongTienHang,
				ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
				isnull(hd.TongChietKhau,0) as  TongChietKhau,
				ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich,							
				ISNULL(hd.TongTienThue, 0) as TongTienThue,						
				isnull(hd.PTThueHoaDon,0) as  PTThueHoaDon,
				ISNULL(hd.TongThueKhachHang, 0) as TongThueKhachHang,	
				isnull(hd.TongTienThueBaoHiem,0) as  TongTienThueBaoHiem,
				isnull(hd.TongTienBHDuyet,0) as  TongTienBHDuyet,
				isnull(hd.SoVuBaoHiem,0) as  SoVuBaoHiem,
				isnull(hd.PTThueBaoHiem,0) as  PTThueBaoHiem,
				isnull(hd.KhauTruTheoVu,0) as  KhauTruTheoVu,
				isnull(hd.GiamTruBoiThuong,0) as  GiamTruBoiThuong,
				isnull(hd.PTGiamTruBoiThuong,0) as  PTGiamTruBoiThuong,
				isnull(hd.BHThanhToanTruocThue,0) as  BHThanhToanTruocThue,
				isnull(hd.PhaiThanhToanBaoHiem,0) as  PhaiThanhToanBaoHiem
			FROM
			(
					select 
						hd.ID			
					from
					(				
						select 
							cthd.ID,
							sum(cthd.SoLuongBan - isnull(cthd.SoLuongTra,0) - isnull(cthd.SoLuongDung,0)) as SoLuongConLai
						from
						(
									------ mua ----
										select 
											ct.ID,
											ct.SoLuong as SoLuongBan,
											0 as SoLuongTra,
											0 as SoLuongDung
										from BH_HoaDon hd
										join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
										where hd.ChoThanhToan=0
										and hd.LoaiHoaDon = 19 ---- khong trahang HDSC
										and hd.NgayLapHoaDon between @DateFrom and @DateTo	
										and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
										and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID) ---- chi get hanghoa + dv
										and (ct.ID_ParentCombo is null OR ct.ID_ParentCombo != ct.ID) ---- khong get parent, get TP combo
						

										union all

										----- tra ----
										select ct.ID_ChiTietGoiDV,
											0 as SoLuongBan,
											ct.SoLuong as SoLuongTra,
											0 as SoLuongDung
										from BH_HoaDon hd
										join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon  
										where hd.ChoThanhToan = 0  
										and hd.LoaiHoaDon = 6
										and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID)													
							

										union all
										----- sudung ----
											select ct.ID_ChiTietGoiDV,
											0 as SoLuongBan,
											0 as SoLuongTra,
											ct.SoLuong as SoLuongDung
										from BH_HoaDon hd
										join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon  
										where hd.ChoThanhToan = 0  
										and hd.LoaiHoaDon = 1
										and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID)													
							) cthd
							group by cthd.ID
					)cthConLai
					join BH_HoaDon_ChiTiet ct on cthConLai.ID=  ct.ID
					join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
					where cthConLai.SoLuongConLai > 0
					group by hd.ID
			) tblConLai 
			JOIN BH_HoaDon hd ON tblConLai.ID =	hd.ID	
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID 
			left join Gara_DanhMucXe xe on hd.ID_Xe = xe.ID		
			where ((select count(Name) from @tblSearch b where     			
					hd.MaHoaDon like '%'+b.Name+'%'								
					or dt.MaDoiTuong like '%'+b.Name+'%'		
					or dt.TenDoiTuong like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'		
					or xe.BienSo like '%'+b.Name+'%'			
					)=@count or @count=0)
					)
	, count_cte
	as
	(
		select count (ID) as TotalRow,
		ceiling(count (ID) / cast(@PageSize as float)) as TotalPage
		from data_cte
	),
	tView
	as
	(
	select *
	from data_cte
	cross join count_cte
	order by NgayLapHoaDon desc
	offset (@CurrentPage * @PageSize) rows
	fetch next @PageSize rows only
	)
	----- get row from- to
	select *
	into #tblView
	from tView




		
		select 
				cnLast.*,
				nv.TenNhanVien,					
				iif (cnLast.LoaiHoaDonGoc != 6, cnLast.ConNo1,
					iif(cnLast.TongGiaTriTra > cnLast.ConNo1, 0, cnLast.ConNo1)) as ConNo
		from
		(
		select 
				tblLast.*,
				tblLast.TongThanhToan 
					--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = 0
					- iif(tblLast.LoaiHoaDonGoc = 6, iif(tblLast.LuyKeTraHang > 0, 0, abs(tblLast.LuyKeTraHang)), tblLast.LuyKeTraHang)
					- tblLast.KhachDaTra  as ConNo1 ---- ConNo = TongThanhToan - GtriBuTru
		from
		(
				select 
					tbl.*,
						isnull(iif(tbl.LoaiHoaDonGoc = 3 or tbl.ID_HoaDon is null,
						iif(tbl.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
							case when tbl.TongGiaTriTra > tbl.KhachNo then tbl.KhachNo						
							else tbl.TongGiaTriTra end),
						(select dbo.BuTruTraHang_HDDoi(tbl.ID_HoaDon,tbl.NgayLapHoaDon,tbl.ID_HoaDonGoc, tbl.LoaiHoaDonGoc))				
					),0) as LuyKeTraHang	
				from
				(
					select hd.*	,
						hdgoc.ID as ID_HoaDonGoc,
						hdgoc.LoaiHoaDon as LoaiHoaDonGoc,
						hdgoc.MaHoaDon as MaHoaDonGoc,

						ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,	
						ISNULL(allTra.NoTraHang,0) as NoTraHang,
						isnull(sqHD.KhachDaTra,0) as KhachDaTra,
						hd.TongThanhToan- isnull(sqHD.KhachDaTra,0) as KhachNo
					from #tblView hd
					left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID
					left join
					(
							select 
								tbUnion.ID_HoaDonLienQuan,
								sum(isnull(tbUnion.KhachDaTra,0)) as KhachDaTra
							from
							(
								------ thu hoadon -----
								select 
									qct.ID_HoaDonLienQuan,
									sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, - qct.TienThu)) as KhachDaTra
								from Quy_HoaDon qhd
								join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon= qhd.ID
								where qhd.TrangThai='1'
								and exists (select hd.ID from #tblView hd where qct.ID_HoaDonLienQuan = hd.ID and  hd.ID_DoiTuong = qct.ID_DoiTuong)
								group by qct.ID_HoaDonLienQuan

								union all

								------ thudathang ---
								select 
									hdFirst.ID,
									hdFirst.KhachDaTra
								from
								(
									select 
										hdxl.ID,
										thuDH.KhachDaTra,
										ROW_NUMBER() over (partition by hdxl.ID_HoaDon order by hdxl.NgayLapHoaDon) as RN
									from BH_HoaDon hdxl
									join 
									(
										select 
											qct.ID_HoaDonLienQuan,
											sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, - qct.TienThu)) as KhachDaTra
										from Quy_HoaDon qhd
										join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
										where qhd.TrangThai='1'
										and exists (select hd.ID from #tblView hd where qct.ID_HoaDonLienQuan = hd.ID_HoaDon and  hd.ID_DoiTuong = qct.ID_DoiTuong)
										group by qct.ID_HoaDonLienQuan
									) thuDH on thuDH.ID_HoaDonLienQuan = hdxl.ID_HoaDon
									where exists (select ID from #tblView hd where hdxl.ID_HoaDon = hd.ID_HoaDon)
									and hdxl.LoaiHoaDon in (1,25)
									and hdxl.ChoThanhToan='0'
								) hdFirst 
								where hdFirst.RN= 1
							) tbUnion group by tbUnion.ID_HoaDonLienQuan
					) sqHD on sqHD.ID_HoaDonLienQuan = hd.ID
				left join
					(
						------ all trahang of hdThis ---
						select 					
							hdt.ID_HoaDon,					
							sum(hdt.PhaiThanhToan) as TongGtriTra,
							sum(hdt.PhaiThanhToan - isnull(chiHDTra.DaTraKhach,0)) as NoTraHang
						from BH_HoaDon hdt	
						left join
						(
							select 
								qct.ID_HoaDonLienQuan,
								sum(iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu)) as DaTraKhach
							from Quy_HoaDon_ChiTiet qct
							join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
							where qhd.TrangThai='0'					
							group by qct.ID_HoaDonLienQuan
						) chiHDTra on hdt.ID = chiHDTra.ID_HoaDonLienQuan
						where hdt.LoaiHoaDon= 6
						and hdt.ChoThanhToan='0'
						group by hdt.ID_HoaDon		
					) allTra on allTra.ID_HoaDon = hd.ID
				) tbl
		)tblLast
		) cnLast
		left join NS_NhanVien nv on cnLast.ID_NhanVien= nv.ID
		drop table #tblView
	
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_GetHDDebit_ofKhachHang]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max),
	@LoaiDoiTuong int
AS
BEGIN
	if @ID_DonVi='00000000-0000-0000-0000-000000000000'
		begin
			set @ID_DonVi = (select CAST(ID as varchar(40)) + ',' as  [text()] from DM_DonVi  where TrangThai is null or TrangThai='1' for xml path(''))	
			set @ID_DonVi= left(@ID_DonVi, LEN(@ID_DonVi) -1) -- remove last comma ,
		end

		select 
			tblView.*,
			tblView.TongThanhToan - GiaTriTatToan - TongTienHDTra as PhaiThanhToan ----- ~ ConNo at view ---
		from
		(
		select 
			tbl.*,			
			iif(tbl.LoaiHoaDonGoc = 6, iif(tbl.LuyKeTraHang > 0, 0, abs(tbl.LuyKeTraHang)), tbl.LuyKeTraHang) as TongTienHDTra
		from
		(
			select 						
				hdGocTra.ID,
				hdGocTra.MaHoaDon,
				hdGocTra.NgayLapHoaDon,
				hdGocTra.LoaiHoaDon,
				hdGocTra.TongTienThue,
				hdGocTra.TongThanhToan,
				hdGocTra.ID_HoaDonGoc,
				hdGocTra.LoaiHoaDonGoc ,			
				isnull(iif(LoaiHoaDonGoc = 3 or hdGocTra.ID_HoaDon is null,								
										case when hdGocTra.TongGiaTriTra > hdGocTra.PhaiThanhToan then hdGocTra.PhaiThanhToan else hdGocTra.TongGiaTriTra end,											
											(select dbo.BuTruTraHang_HDDoi(hdGocTra.ID_HoaDon,NgayLapHoaDon,hdGocTra.ID_HoaDonGoc, hdGocTra.LoaiHoaDonGoc))				
						),0) as LuyKeTraHang,
				isnull(tattoanTGT.GiaTriTatToan,0) as GiaTriTatToan,
				hdGocTra.TinhChietKhauTheo			
			from
			(
			select hd.ID,
				hd.MaHoaDon, 
				hd.NgayLapHoaDon, 
				hd.LoaiHoaDon,
				hd.TongTienThue,
				hd.TongThanhToan,
				hd.PhaiThanhToan,
    			0 as TinhChietKhauTheo,
				hd.ID_HoaDon,
				hdgoc.LoaiHoaDon as LoaiHoaDonGoc,
				hdgoc.ID_HoaDon as ID_HoaDonGoc,
				ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra		
    		from BH_HoaDon hd
    		left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID ---- khong check loaiHD: lay ca hdTra + hdDoi
			left join
				(
					------ all trahang of hdgoc ---
					select 					
						hdt.ID_HoaDon,					
						sum(hdt.PhaiThanhToan) as TongGtriTra,
						sum(hdt.PhaiThanhToan - isnull(chiHDTra.DaTraKhach,0)) as NoTraHang
					from BH_HoaDon hdt	
					left join
					(
						select 
							qct.ID_HoaDonLienQuan,
							sum(iif(qhd.LoaiHoaDon=12, qct.TienThu, -qct.TienThu)) as DaTraKhach
						from Quy_HoaDon_ChiTiet qct
						join Quy_HoaDon qhd on qct.ID_HoaDon= qct.ID_HoaDonLienQuan
						where qhd.TrangThai='0'					
						group by qct.ID_HoaDonLienQuan
					) chiHDTra on hdt.ID = chiHDTra.ID_HoaDonLienQuan
					where hdt.LoaiHoaDon= 6
					and hdt.ChoThanhToan='0'
					and hdt.ID_DoiTuong like @ID_DoiTuong
					group by hdt.ID_HoaDon		
				) allTra on allTra.ID_HoaDon = hd.ID
    	
    		where 
			exists (select Name from dbo.splitstring(@ID_DonVi) where Name= hd.ID_DonVi)
			and hd.ID_DoiTuong like @ID_DoiTuong		
    		and hd.LoaiHoaDon in (1,19,4,22, 25)
    		and hd.ChoThanhToan='0' 
			) hdGocTra			
			left join
			(
				---- khi khách mua TGT nhưng chưa thanh toán hết --> tất toán công nợ ảo ---
				select
					hd.ID_HoaDon, hd.TongThanhToan as GiaTriTatToan
				from BH_HoaDon hd
				where hd.ID_DoiTuong like @ID_DoiTuong
				and hd.ID_HoaDon is not null ---- idThẻgiá trị				
				and hd.ChoThanhToan='0' and hd.LoaiHoaDon= 42
			) tattoanTGT on hdGocTra.ID= tattoanTGT.ID_HoaDon
		) tbl		
		)tblView order by NgayLapHoaDon desc
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

			Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMuc_KhachHangNhaCungCap]
    @IDChiNhanhs [nvarchar](max) = '871a3d63-816a-4e68-bef7-216f3efa27dd',
    @LoaiDoiTuong [int] = 1,
    @IDNhomKhachs [nvarchar](max) ='',
    @TongBan_FromDate [datetime] ='',
    @TongBan_ToDate [datetime]='',
    @NgayTao_FromDate [datetime] ='',
    @NgayTao_ToDate [datetime]='',
    @TextSearch [nvarchar](max)='',
    @Where [nvarchar](max)='',
    @ColumnSort [nvarchar](40)='',
    @SortBy [nvarchar](40)='DESC',
    @CurrentPage [int]=0,
    @PageSize [int] = 20
AS
BEGIN
    SET NOCOUNT ON;
    	declare @whereCus nvarchar(max), @whereInvoice nvarchar(max), @whereLast nvarchar(max), 
    	@whereNhomKhach nvarchar(max),	@whereChiNhanh nvarchar(max), @whereNgayLapHD nvarchar(max),
    	@sql nvarchar(max) , @sql1 nvarchar(max), @sql2 nvarchar(max), @sql3 nvarchar(max),@sql4 nvarchar(max),
    	@paramDefined nvarchar(max)
    
    		declare @tblDefined nvarchar(max) = concat(N' declare @tblChiNhanh table (ID uniqueidentifier) ',	
    												   N' declare @tblIDNhoms table (ID uniqueidentifier) ',
    												   N' declare @tblSearch table (Name nvarchar(max))'    											 
    												   )
    
    
    		set @whereInvoice =' where 1 = 1 and hd.ChoThanhToan = 0 '
    		set @whereCus =' where 1 = 1 and dt.LoaiDoiTuong = @LoaiDoiTuong_In '		
    		set @whereLast = N' where tbl.ID not like ''00000000-0000-0000-0000-000000000%'' '
    		set @whereNhomKhach =' ' 
    		set @whereChiNhanh =' where 1 = 1 ' 
			set @whereNgayLapHD =' ' --- because quyHoaDon = @where chinhanh + @where ngaylapHD
    
    		if isnull(@CurrentPage,'')=''
    			set @CurrentPage =0
    		if isnull(@PageSize,'')=''
    			set @PageSize = 10
    
    		if isnull(@ColumnSort,'')=''
    			set @ColumnSort = 'NgayTao'
    		if isnull(@SortBy,'')=''
    			set @SortBy = 'DESC'
    
    		set @sql1= 'declare @count int = 0'
    
    		declare @QLTheoCN bit = '0'
    		if ISNULL(@IDChiNhanhs,'')!=''
    			begin								
    				set @QLTheoCN = (select max(cast(QuanLyKhachHangTheoDonVi as int)) from HT_CauHinhPhanMem 
    					where exists (select * from dbo.splitstring(@IDChiNhanhs) cn where ID_DonVi= cn.Name))
    
    				set @sql1 = concat(@sql1,
    				N' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In)')
    
    				set @whereChiNhanh= CONCAT(@whereChiNhanh, ' and exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID)')
    				set @whereInvoice= CONCAT(@whereInvoice, ' and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)')
    			end
    		
    
    		if ISNULL(@IDNhomKhachs,'')='' ---- idNhom = empty
    			begin			
    				set @sql1 = concat(@sql1,
    				N' insert into @tblIDNhoms(ID) values (''00000000-0000-0000-0000-000000000000'')')
    
    				if @QLTheoCN = 1
    					begin
    						set @sql1 = concat(@sql1, N' insert into @tblIDNhoms(ID)
    						select * 
    						from (
    						-- get Nhom not not exist in NhomDoiTuong_DonVi
    						select ID from DM_NhomDoiTuong nhom  
    						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
    						and LoaiDoiTuong = @LoaiDoiTuong_In
    						union all
    						-- get Nhom at this ChiNhanh
    						select ID_NhomDoiTuong  from NhomDoiTuong_DonVi ', @whereChiNhanh,
    						N' ) tbl ')	
    						
    						set @whereNhomKhach  = CONCAT(@whereNhomKhach,
    						N' and EXISTS(SELECT Name FROM splitstring(tbl.ID_NhomDoiTuong) lstFromtbl 
    								inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID where lstFromtbl.Name!='''' )')	
    					end										
    			end
    		else
    		begin
    			set @sql1=  CONCAT(@sql1, N' insert into @tblIDNhoms values ( CAST(@IDNhomKhachs_In as uniqueidentifier) ) ')
    			set @whereNhomKhach  = CONCAT(@whereNhomKhach,
    			N' and EXISTS(SELECT Name FROM splitstring(tbl.ID_NhomDoiTuong) lstFromtbl 
    					inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID where lstFromtbl.Name!='''' )')			
    		end
    
    		if isnull(@TextSearch,'') !=''
    			begin
    				set @sql1= CONCAT(@sql1, N' 
    				INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''';
    			Select @count =  (Select count(*) from @tblSearch);')
    
    				set @whereLast = CONCAT(@whereLast,
    				 N' and ((select count(Name) from @tblSearch b where 				
    				 tbl.Name_Phone like ''%''+b.Name+''%''    		
    				)=@count or @count=0)')
    			end
    
    		if isnull(@NgayTao_FromDate,'') !=''
    			if isnull(@NgayTao_ToDate,'') !=''
    				begin
    					set @whereCus = CONCAT(@whereCus, N' and dt.NgayTao between @NgayTao_FromDate_In and @NgayTao_ToDate_In')
    				end
    
    		if isnull(@TongBan_FromDate,'') !=''
    			if isnull(@TongBan_ToDate,'') !=''
    				begin
    					set @whereInvoice = CONCAT(@whereInvoice, N' and hd.NgayLapHoaDon between @TongBan_FromDate_In and @TongBan_ToDate_In')
						set @whereNgayLapHD = N' and NgayLapHoaDon between @TongBan_FromDate_In and @TongBan_ToDate_In' ---- !important: only {NgayLapHoaDon}
    				end			
    
    		if ISNULL(@Where,'')!=''
    			begin
    				set @Where = CONCAT(@whereLast, @whereNhomKhach, ' and ', @Where)
    			end
    		else
    			begin
    				set @Where = concat(@whereLast, @whereNhomKhach)
    			end
    		
    	set @sql2 = concat(
    		N'
    	;with data_cte
    	as
    	(
    		select *
    		from
    		(
    		select 
    			dt.*,
				isnull(a.TongThuKhachHang,0) as TongThuKhachHang,
				isnull(a.TongChiKhachHang,0) as TongChiKhachHang,
				isnull(traGDV.GiaTriHoanTraGDV,0) as GiaTriDVHoanTra,
				isnull(tblSuDung.GiaTriSuDung,0) as GiaTriDVSuDung,

    			isnull(a.NoHienTai,0) as NoHienTai,
    			isnull(a.TongBan,0) as TongBan,
    			isnull(a.TongMua,0) as TongMua,
    			isnull(a.TongBanTruTraHang,0) as TongBanTruTraHang,
    			cast(isnull(a.SoLanMuaHang,0) as float) as SoLanMuaHang,
    			isnull(a.PhiDichVu,0) as PhiDichVu,
				isnull(a.NapCoc,0) as NapCoc,
				isnull(a.SuDungCoc,0) as SuDungCoc,
				isnull(a.SoDuCoc,0) as SoDuCoc,
    			CONCAT(dt.MaDoiTuong,'' '', dt.TenDoiTuong, '' '', dt.DienThoai, '' '', dt.TenDoiTuong_KhongDau) as Name_Phone
    		from (
    			select 
    				dt.ID,
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				dt.TenDoiTuong_KhongDau,
    				dt.TenDoiTuong_ChuCaiDau,
    				dt.LoaiDoiTuong,
    				dt.ID_TrangThai,
    				dt.ID_NguonKhach,
    				dt.ID_NhanVienPhuTrach,
    				dt.ID_NguoiGioiThieu,
    				dt.ID_DonVi,
    				dt.ID_TinhThanh,
    				dt.ID_QuanHuyen,
    				isnull(dt.TheoDoi,''0'') as TheoDoi,
    				dt.LaCaNhan,				
    				dt.GioiTinhNam,
    				dt.NgaySinh_NgayTLap,
    				dt.DinhDang_NgaySinh,
    				dt.NgayGiaoDichGanNhat,
    				dt.TaiKhoanNganHang,
    				isnull(dt.TenNhomDoiTuongs,N''Nhóm mặc định'') as TenNhomDT,
    				dt.NgayTao,
    				isnull(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    				isnull(dt.TongTichDiem,0) as TongTichDiem,
    				----isnull(dt.TheoDoi,''0'') as TrangThaiXoa,
    				isnull(dt.DienThoai,'''') as DienThoai,
    				isnull(dt.Email,'''') as Email,
    				isnull(dt.DiaChi,'''') as DiaChi,
    				isnull(dt.MaSoThue,'''') as MaSoThue,
    				isnull(dt.GhiChu,'''') as GhiChu,
    				ISNULL(dt.NguoiTao,'''') as NguoiTao,
    				iif(dt.IDNhomDoiTuongs='''' or dt.IDNhomDoiTuongs is null,''00000000-0000-0000-0000-000000000000'', dt.IDNhomDoiTuongs) as ID_NhomDoiTuong
    			from DM_DoiTuong dt ', @whereCus, N' )  dt
				left join 
				(
				 ----- hoan goidichvu ---
				 select 
					hd.ID_DoiTuong,
					sum(ct.ThanhTien) as GiaTriHoanTraGDV
				 from BH_HoaDon hd
				 join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon',
				 @whereInvoice,
				' and hd.LoaiHoaDon = 6
				 and ct.ChatLieu = 2 ----- chi lay tra GDV ----
				 group by hd.ID_DoiTuong
				) traGDV on dt.ID = traGDV.ID_DoiTuong

				left join 
				(
				 ----- giatri sudung DV ---
				 ----- sudung buoi le/ sudung tu GDV ----
				 select 
					hd.ID_DoiTuong,
					SUM(iif(ctsd.ChatLieu is null or ctsd.ChatLieu not in (4,5), ctsd.ThanhTien * (1 -  hd.TongGiamGia/iif(hd.TongTienHang =0,1,hd.TongTienHang)),
						ctsd.SoLuong * (ctm.DonGia - ctm.TienChietKhau) * (1 -  gdv.TongGiamGia/iif(gdv.TongTienHang =0,1,gdv.TongTienHang))
						))as GiaTriSuDung
				 from BH_HoaDon hd
				 join BH_HoaDon_ChiTiet ctsd on hd.ID = ctsd.ID_HoaDon
				 left join BH_HoaDon_ChiTiet ctm on ctsd.ID_ChiTietGoiDV = ctm.ID
				 left join BH_HoaDon gdv on ctm.ID_HoaDon = gdv.ID and gdv.LoaiHoaDon = 19',
				 @whereInvoice, ' and hd.LoaiHoaDon = 1			
				 group by hd.ID_DoiTuong 			
				) tblSuDung on dt.ID = tblSuDung.ID_DoiTuong

    			left join
    			(
    			select 
    				 tblThuChi.ID_DoiTuong,
					 SUM(ISNULL(tblThuChi.ThuHoaDon,0)) as TongThuKhachHang,
					 SUM(ISNULL(tblThuChi.ChiHoaDon,0)) as TongChiKhachHang,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.HoanTraThe,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) 
							+ sum(ISNULL(tblThuChi.ThuTuThe,0))
    						- sum(isnull(tblThuChi.PhiDichVu,0)) 
    						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
    				sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang,
    				sum(isnull(tblThuChi.PhiDichVu,0)) as PhiDichVu,
					sum(isnull(tblThuChi.NapCoc,0)) as NapCoc,
					sum(isnull(tblThuChi.SuDungCoc,0)) as SuDungCoc,
					sum(isnull(tblThuChi.NapCoc,0)) -sum(isnull(tblThuChi.SuDungCoc,0))  as SoDuCoc ')
    		set @sql3=concat( N' from
    			(
    				---- chiphi dv ncc ----
    				select 
    					cp.ID_NhaCungCap as ID_DoiTuong,
    					0 as GiaTriTra,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ChiHoaDon,
    					0 AS SoLanMuaHang,
    					0 as ThuTuThe,
    					sum(cp.ThanhTien) as PhiDichVu,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				from BH_HoaDon_ChiPhi cp
    				join BH_HoaDon hd on cp.ID_HoaDon= hd.ID
    				', @whereChiNhanh,
    				N' and hd.ChoThanhToan = 0',
    				 N' group by cp.ID_NhaCungCap

					 union all
					
					 ---- hoantra sodu TGT cho khach (giam sodu TGT)

					SELECT 
    						bhd.ID_DoiTuong,    	
							0 as GiaTriTra,
    						0 as DoanhThu,
							0 AS TienThu,
    						0 AS TienChi, 
							0 as ThuHoaDon,
							0 as ChiHoaDon,
    						0 AS SoLanMuaHang,
							0 as ThuTuThe,
							0 as PhiDichVu,
							-sum(bhd.PhaiThanhToan) as HoanTraThe,
							0 as NapCoc,
							0 as SuDungCoc
    				FROM BH_HoaDon bhd ',
					 @whereChiNhanh,
					 @whereNgayLapHD,
					N' and bhd.LoaiHoaDon = 32 and bhd.ChoThanhToan = 0 
					and exists (select * from @tblChiNhanh cn where bhd.ID_DonVi= cn.ID)
					group by bhd.ID_DoiTuong
    
    				union all
    				----- tongban ----
    				SELECT 
    					hd.ID_DoiTuong,    	
    					0 as GiaTriTra,
    					hd.PhaiThanhToan as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ChiHoaDon,
    					0 AS SoLanMuaHang,
    					0 as ThuTuThe,
    					0 as PhiDichVu,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ', @whereInvoice, N'  and hd.LoaiHoaDon in (1,7,19,25) ',
    
    				N' union all
    				---- doanhthu tuthe
    				SELECT 
    					hd.ID_DoiTuong,    	
    					0 as GiaTriTra,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ChiHoaDon,
    					0 AS SoLanMuaHang,
    					hd.PhaiThanhToan as ThuTuThe,
    					0 as PhiDichVu,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ', @whereInvoice , N' and hd.LoaiHoaDon = 22 ', 
    
    					N' union all
    				-- gia tri trả từ bán hàng
    				SELECT 
    					hd.ID_DoiTuong,    	
    					hd.PhaiThanhToan as GiaTriTra,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ChiHoaDon,
    					0 AS SoLanMuaHang,
    					0 as ThuTuThe,
    					0 as PhiDichVu,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ',  @whereInvoice, N'  and hd.LoaiHoaDon in (6,4,42) ',
    				
    				N' union all
    				----- tienthu/chi ---
    				SELECT 
    				qct.ID_DoiTuong,						
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				iif(qhd.LoaiHoaDon=11,qct.TienThu,0) AS TienThu,
    				iif(qhd.LoaiHoaDon=12,qct.TienThu,0) AS TienChi,
					iif(qct.ID_HoaDonLienQuan is null,0, iif(qhd.LoaiHoaDon=11,qct.TienThu,0)) as ThuHoaDon,
					iif(qct.ID_HoaDonLienQuan is null,0, iif(qhd.LoaiHoaDon=12,qct.TienThu,0)) as ChiHoaDon,
    				0 AS SoLanMuaHang,
    				0 as ThuTuThe,
    				0 as PhiDichVu,
					0 as HoanTraThe,
					0 as NapCoc,
					0 as SuDungCoc
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qct ON qhd.ID = qct.ID_HoaDon ',
    				@whereChiNhanh, 
					@whereNgayLapHD,
    				N' and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
    				and qct.HinhThucThanhToan!= 6
    				and (qct.LoaiThanhToan is null or qct.LoaiThanhToan != 3) ',
    				
					---- NapCoc NCC----	

					N' union all
					
    				SELECT 
    					qhdct.ID_DoiTuong,						
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as ThuHoaDon,
						0 as ChiHoaDon,
    					0 AS SoLanMuaHang,
						0 as ThuTuThe,
						0 as PhiDichVu,
						0 as HoanTraThe,
						iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu) as NapCocNCC,
						0 as SuDungCoc
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon ',
					@whereChiNhanh, 
    				N' and (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
					and qhdct.LoaiThanhToan = 1',

					---- sudungcoc ----
						' union all
									
    				SELECT 
    					qhdct.ID_DoiTuong,						
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as ThuHoaDon,
						0 as ChiHoaDon,
    					0 AS SoLanMuaHang,
						0 as ThuTuThe,
						0 as PhiDichVu,
						0 as HoanTraThe,
						0 as NapCoc,
						iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu) as SuDungCoc
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon ',
					@whereChiNhanh, 
    				N' and (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
					and qhdct.HinhThucThanhToan = 6 ',
					   
    				N' union all

    				----- solanmuahang ---
    				Select 
    				hd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 as TienChi,
					0 as ThuHoaDon,
					0 as ChiHoaDon,
    				COUNT(*) AS SoLanMuaHang,
    				0 as ThuTuThe,
    				0 as PhiDichVu,
					0 as HoanTraThe,
					0 as NapCoc,
					0 as SuDungCoc
    			From BH_HoaDon hd ' , @whereInvoice ,  N' and hd.LoaiHoaDon in (1,19,25) ',
    			N' group by hd.ID_DoiTuong
    			)tblThuChi 
    			GROUP BY tblThuChi.ID_DoiTuong
    		) a on dt.ID= a.ID_DoiTuong 
    		) tbl ', @Where ,
    	'), 
    	count_cte
    	as
    	(
	    		SELECT COUNT(ID) AS TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize_In as float)) as TotalPage,
					SUM(TongBan) as TongBanAll,
    				SUM(TongBanTruTraHang) as TongBanTruTraHangAll,
    				SUM(TongTichDiem) as TongTichDiemAll,
    				SUM(NoHienTai) as NoHienTaiAll,
    				SUM(PhiDichVu) as TongPhiDichVu,
					SUM(TongThuKhachHang) as SumTongThuKhachHang,
					SUM(TongChiKhachHang) as SumTongChiKhachHang,
					SUM(GiaTriDVHoanTra) as SumGiaTriDVHoanTra,
					SUM(GiaTriDVSuDung) as SumGiaTriDVSuDung
    		from data_cte
    	),
    	tView
    	as (
    	select *		
    	from data_cte dt
    	cross join count_cte cte
    	ORDER BY ', @ColumnSort, ' ', @SortBy,
    	N' offset (@CurrentPage_In * @PageSize_In) ROWS
    	fetch next @PageSize_In ROWS only
    	)
    	select dt.*,
    		 ISNULL(trangthai.TenTrangThai,'''') as TrangThaiKhachHang,
    	ISNULL(qh.TenQuanHuyen,'''') as PhuongXa,
    	ISNULL(tt.TenTinhThanh,'''') as KhuVuc,
    	ISNULL(dv.TenDonVi,'''') as ConTy,
    	ISNULL(dv.SoDienThoai,'''') as DienThoaiChiNhanh,
    	ISNULL(dv.DiaChi,'''') as DiaChiChiNhanh,
    	ISNULL(nk.TenNguonKhach,'''') as TenNguonKhach,
    	ISNULL(dt2.TenDoiTuong,'''') as NguoiGioiThieu,
    		ISNULL(nvpt.MaNhanVien,'''') as MaNVPhuTrach,
    		ISNULL(nvpt.TenNhanVien,'''') as TenNhanVienPhuTrach
    	from tView dt
    	left join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    	LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
    	LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
    	LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
    	LEFT join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID
    	LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    	LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID
    	')
    
    		set @sql = CONCAT(@tblDefined, @sql1, @sql2, @sql3)
    
    		set @paramDefined = N'@IDChiNhanhs_In nvarchar(max),
    								@LoaiDoiTuong_In int ,
    								@IDNhomKhachs_In nvarchar(max),
    								@TongBan_FromDate_In datetime,
    								@TongBan_ToDate_In datetime,
    								@NgayTao_FromDate_In datetime,
    								@NgayTao_ToDate_In datetime,
    								@TextSearch_In nvarchar(max),
    								@Where_In nvarchar(max) ,							
    								@ColumnSort_In varchar(40),
    								@SortBy_In varchar(40),
    								@CurrentPage_In int,
    								@PageSize_In int'
    
    		--print @sql
    		--print @sql2
    		--print @sql3
    
    
    		exec sp_executesql @sql, @paramDefined, 
    					@IDChiNhanhs_In = @IDChiNhanhs,
    					@LoaiDoiTuong_In= @LoaiDoiTuong,
    					@IDNhomKhachs_In= @IDNhomKhachs,
    					@TongBan_FromDate_In= @TongBan_FromDate,
    					@TongBan_ToDate_In =@TongBan_ToDate,
    					@NgayTao_FromDate_In =@NgayTao_FromDate ,
    					@NgayTao_ToDate_In = @NgayTao_ToDate,
    					@TextSearch_In = @TextSearch,
    					@Where_In = @Where ,
    					@ColumnSort_In = @ColumnSort,
    					@SortBy_In = @SortBy,
    					@CurrentPage_In = @CurrentPage,
    					@PageSize_In = @PageSize
END

--LoadDanhMuc_KhachHangNhaCungCap");

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

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountInvoice_Detail]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs [nvarchar](max),
    @TextSearch [nvarchar](max),
	@TxtCustomer [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @Status_DoanhThu [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
		set @DateTo = DATEADD(day,1,@DateTo)

		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');
		
		DECLARE @tblDepartment TABLE (ID_PhongBan uniqueidentifier)
		if @DepartmentIDs =''
			insert into @tblDepartment
			select distinct ID_PhongBan from NS_QuaTrinhCongTac pb
		else
			insert into @tblDepartment
			select * from splitstring(@DepartmentIDs)

		----- get nv thuoc PB
		declare @tblNhanVien table (ID uniqueidentifier)
		insert into @tblNhanVien
		select nv.ID
		from @tblNhanVienAll nv
		join NS_QuaTrinhCongTac ct on nv.ID = ct.ID_NhanVien
		where exists (select ID_PhongBan from @tblDepartment pb where pb.ID_PhongBan= ct.ID_PhongBan) 

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    		
    	
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @tblDiscountInvoice table (ID uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), NgayLapHoaDon datetime, NgayLapPhieu datetime, NgayLapPhieuThu datetime, MaHoaDon nvarchar(50),
    		DoanhThu float,
			ThucThu float,
			ChiPhiNganHang float,
			TongChiPhiNganHang float,
			ThucThu_ThucTinh float,
			HeSo float, HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, PTThucThu float, PTDoanhThu float, 
    		MaKhachHang nvarchar(max), TenKhachHang nvarchar(max), DienThoaiKH nvarchar(max), ID_NhanVienPhuTrach uniqueidentifier, TongAll float)
    
  --  	-- bang tam chua DS phieu thu theo Ngay timkiem
		select qct.ID_HoaDonLienQuan, 
			qhd.ID,
			qhd.NgayLapHoaDon, 
			max(isnull(qct.ChiPhiNganHang,0)) as ChiPhiNganHang,
			---- thanhtoan = TGT: nhung van chon NV thuchien
			SUM(iif(qct.HinhThucThanhToan in (4,5),0, iif( qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu))) as ThucThu,
			---- chi get chiphi with POS
			sum(iif(qct.HinhThucThanhToan != 2,0, iif(qct.LaPTChiPhiNganHang='0', qct.ChiPhiNganHang,  qct.ChiPhiNganHang * qct.TienThu/100))) as TongChiPhiNganHang					
    	into #tempQuy
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID 
		where (qhd.TrangThai is null or qhd.TrangThai = '1')
		and qhd.ID_DonVi in (select ID from @tblChiNhanh)
		and qhd.NgayLapHoaDon >= @DateFrom
    	and qhd.NgayLapHoaDon < @DateTo 
		--and qct.HinhThucThanhToan not in (4,5)
    	group by  qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID --, qct.LaPTChiPhiNganHang

		---- thucthu theo hoadon
		select ctquy.*, tblTong.TongThuThucTe
		into #tblQuyThucTe
		from #tempQuy ctquy
		left join
		(
		select ID_HoaDonLienQuan,		
			sum(ThucThu) as TongThuThucTe
		from #tempQuy
		group by ID_HoaDonLienQuan
		) tblTong on ctquy.ID_HoaDonLienQuan= tblTong.ID_HoaDonLienQuan
		
    
    		select
				tbl.ID, ---- id of hoadon
				MaNhanVien, 
    			tbl.TenNhanVien,
    			tbl.NgayLapHoaDon,
    			tbl.NgayLapPhieu, -- used to check at where condition
    			tbl.NgayLapPhieuThu,
    			tbl.MaHoaDon,						
    			-- taoHD truoc, PhieuThu sau --> khong co doanh thu
    			case when  tbl.NgayLapHoaDon between @DateFrom and @DateTo then PhaiThanhToan else 0 end as DoanhThu, 
    			ISNULL(ThucThu,0) as ThucThu,
				tbl.ChiPhiNganHang,
				tbl.TongChiPhiNganHang,
				tbl.ThucThu - tbl.TongChiPhiNganHang as ThucThu_ThucTinh,
    			ISNULL(HeSo,0) as HeSo,
    			ISNULL(HoaHongThucThu,0) as HoaHongThucThu,
    			ISNULL(HoaHongDoanhThu,0) as HoaHongDoanhThu,
    			ISNULL(HoaHongVND,0) as HoaHongVND,
    			ISNULL(PTThucThu,0) as PTThucThu,
    			ISNULL(PTDoanhThu,0) as PTDoanhThu,
    			ISNULL(MaDoiTuong,'') as MaKhachHang,
    			ISNULL(TenDoiTuong,N'Khách lẻ') as TenKhachHang,
    			ISNULL(dt.DienThoai,'') as DienThoaiKH,		
				dt.ID_NhanVienPhuTrach,
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
    		into #temp2
    	from 
    	(    		
    				select distinct MaNhanVien, TenNhanVien, 
						nv.TenNhanVienKhongDau, 
						hd.MaHoaDon, 
    					case when hd.LoaiHoaDon= 6 then - TongThanhToan + isnull(TongTienThue,0)
    					else case when hd.ID_DonVi in (select ID from @tblChiNhanh) then
							iif(hd.LoaiHoaDon=22, PhaiThanhToan, TongThanhToan - TongTienThue) else 0 end end as PhaiThanhToan,
    					hd.NgayLapHoaDon,
						tblQuy.ThucThu ,	
						tblQuy.ChiPhiNganHang ,					
						tblQuy.TongChiPhiNganHang,
						hd.LoaiHoaDon,
    					hd.ID_DoiTuong,
						hd.ID,
    					th.HeSo,
    					tblQuy.NgayLapHoaDon as NgayLapPhieuThu,
						

    				-- huy PhieuThu --> PTThucThu,HoaHongThucThu = 0		
    					case when TinhChietKhauTheo =1 
    						then case when LoaiHoaDon in ( 6, 32) then -TienChietKhau else 
    							case when ISNULL(ThucThu,0)= 0 then 0  else TienChietKhau end end end as HoaHongThucThu,
						th.PT_ChietKhau as PTThucThu,
    					--case when TinhChietKhauTheo =1 
    					--	then case when LoaiHoaDon in ( 6, 32) then PT_ChietKhau else 
    					--		case when ISNULL(ThucThu,0)= 0 then 0  else PT_ChietKhau end end end as PTThucThu,			    				
    					case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then
    						case when TinhChietKhauTheo = 2 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end else 0 end as HoaHongDoanhThu,
    					case when TinhChietKhauTheo =3 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end as HoaHongVND,
    					case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then
    						case when TinhChietKhauTheo = 2 then PT_ChietKhau end else 0 end as PTDoanhThu,
    					-- timkiem theo NgayLapHD or NgayLapPhieuThu
    					case when @DateFrom <= hd.NgayLapHoaDon and hd.NgayLapHoaDon < @DateTo then hd.NgayLapHoaDon else tblQuy.NgayLapHoaDon end as NgayLapPhieu ,
    					case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    			
    			from BH_NhanVienThucHien th		
    			join NS_NhanVien nv on th.ID_NhanVien= nv.ID
    			join BH_HoaDon hd on th.ID_HoaDon= hd.ID
    			left join #tblQuyThucTe tblQuy on th.ID_QuyHoaDon = tblQuy.ID and tblQuy.ID_HoaDonLienQuan = hd.ID --- join hoadon (truong hop phieuthu nhieu hoadon)
    			where th.ID_HoaDon is not null
    				and hd.LoaiHoaDon in (1,19,22,6, 25,3, 32)
    				and hd.ChoThanhToan is not null    				
					and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    				and (exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID))
    				--chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
    				and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 
					and ( exists (select ID from #tempQuy where th.ID_QuyHoaDon = #tempQuy.ID) or  LoaiHoaDon=6)))		
    					
    	) tbl
    		left join DM_DoiTuong dt on tbl.ID_DoiTuong= dt.ID
    		where tbl.NgayLapPhieu >= @DateFrom and tbl.NgayLapPhieu < @DateTo and TrangThaiHD = @StatusInvoice
			and 
    				((select count(Name) from @tblSearchString b where     			
    				tbl.TenNhanVien like '%'+b.Name+'%'
    				or tbl.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or tbl.MaNhanVien like '%'+b.Name+'%'	
    				or tbl.MaHoaDon like '%'+b.Name+'%'							
					or dt.DienThoai like '%'+b.Name+'%'
    				)=@count or @count=0)
			and (
				dt.MaDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong_KhongDau like N'%'+  @TxtCustomer +'%'
				or dt.DienThoai like N'%'+  @TxtCustomer +'%'
				)

    
    	if @Status_DoanhThu =0
    		insert into @tblDiscountInvoice
    		select *
    		from #temp2
    	else
    		begin
    				if @Status_DoanhThu= 1
    					insert into @tblDiscountInvoice
    					select *
    					from #temp2 where HoaHongDoanhThu > 0 or HoaHongThucThu != 0
    				else
    					if @Status_DoanhThu= 2
    						insert into @tblDiscountInvoice
    						select *
    						from #temp2 where HoaHongDoanhThu > 0 or HoaHongVND > 0
    					else		
    						if @Status_DoanhThu= 3
    							insert into @tblDiscountInvoice
    							select *
    							from #temp2 where HoaHongDoanhThu > 0
    						else	
    							if @Status_DoanhThu= 4
    								insert into @tblDiscountInvoice
    								select *
    								from #temp2 where HoaHongVND > 0 Or HoaHongThucThu != 0
    							else
    								if @Status_DoanhThu= 5
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2 where  HoaHongThucThu > 0
    								else -- 6
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2  where HoaHongVND > 0
    								
    			end;

				declare @tongDoanhThu float, @tongThucThu float

				select @tongDoanhThu = (select sum (tblDT.DoanhThu)
											from
											(
												select  id, MaHoaDon, NgayLapHoaDon, max(DoanhThu) as DoanhThu
												from @tblDiscountInvoice
												group by ID, MaHoaDon, NgayLapHoaDon
											)tblDT
										)
	
				select @tongThucThu = (select sum(tblTT.ThucThu)
										from
										(
											select sum(ThucThu) as ThucThu
											from
											(
											select  id, MaHoaDon, max(ThucThu)  as ThucThu
											from @tblDiscountInvoice
											group by ID, MaHoaDon, NgayLapPhieuThu
											) tbl2 group by ID, MaHoaDon
										)tblTT
										);
    
    	with data_cte
    		as(
    		select * from @tblDiscountInvoice
    		),
    		count_cte
    		as (
    			select count(*) as TotalRow,
    				CEILING(COUNT(*) / CAST(@PageSize as float ))  as TotalPage,
					@tongDoanhThu as TongDoanhThu,
					@tongThucThu as TongThucThu,
    				sum(HoaHongThucThu) as TongHoaHongThucThu,
    				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
    				sum(HoaHongVND) as TongHoaHongVND,
    				sum(TongAll) as TongAllAll,
					sum(TongChiPhiNganHang) as SumAllChiPhiNganHang,
					@tongThucThu - sum(TongChiPhiNganHang) as SumThucThu_ThucTinh
    			from data_cte
    		)
    		select dt.*, cte.*,
			isnull(nv.MaNhanVien,'') as MaNVPhuTrach,
			isnull(nv.TenNhanVien,'') as TenNVPhuTrach
    		from data_cte dt
			left join NS_NhanVien nv on dt.ID_NhanVienPhuTrach = nv.ID
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
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
