namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSP_20180511 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[delete_DoiTuong_Nhom]", parametersAction: p => new
            {
                ID_NhomDoiTuong = p.Guid()
            }, body: @"DELETE from DM_DoiTuong_Nhom where ID_NhomDoiTuong = @ID_NhomDoiTuong");

            CreateStoredProcedure(name: "[dbo].[delete_NhomDoiTuongChiTiet]", parametersAction: p => new
            {
                ID_NhomDoiTuong = p.Guid()
            }, body: @"Delete from DM_NhomDoiTuong_ChiTiet where ID_NhomDoiTuong = @ID_NhomDoiTuong");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc1]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'SELECT b.ID_DoiTuong
	FROM
	(
		SELECT
		a.ID_DoiTuong,
		CAST(ROUND(a.GiaTriBan , 0) as float ) as GiaTriBan,
		CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
		CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan
		FROM
		(
			SELECT 
			hd.ID_DoiTuong as ID_DoiTuong,
    			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
			and hd.ChoThanhToan = 0
			and hd.ID_DoiTuong is not null
			GROUP BY hd.ID_DoiTuong
		) a
	) b
	where DoanhThuThuan'
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc10]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'select dt.ID as ID_DoiTuong from DM_DoiTuong dt
				inner join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
				where tt.ID_VungMien ';
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc2]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'SELECT b.ID_DoiTuong
	FROM
	(
		SELECT
		a.ID_DoiTuong,
		CAST(ROUND(a.GiaTriBan , 0) as float ) as GiaTriBan,
		CAST(ROUND(a.GiaTriTra * (-1), 0) as float ) as GiaTriTra,
		CAST(ROUND(a.GiaTriBan - a.GiaTriTra , 0) as float ) as DoanhThuThuan
		FROM
		(
			SELECT 
			hd.ID_DoiTuong as ID_DoiTuong,
    			SUM(Case when hd.LoaiHoaDon = 1 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriBan,
    			SUM(Case when hd.LoaiHoaDon = 6 then (ISNULL(hdct.ThanhTien, 0)) *(1- ISNULL(hd.TongGiamGia, 0) / ISNULL(hd.TongTienHang, 0)) else 0 end) as GiaTriTra
			FROM
			BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6)
			and hd.ChoThanhToan = 0
			and hd.ID_DoiTuong is not null
			GROUP BY hd.ID_DoiTuong
		) a
	) b
	where GiaTriBan'
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc3]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	declare @sql3  [nvarchar](max);
	set @sql = 'select hd.ID_DoiTuong from BH_HoaDon hd
				where hd.ChoThanhToan = 0
				and hd.LoaiHoaDon = 1
				and hd.ID_DoiTuong is not null
				and NgayLapHoaDon';
	set @sql2 = ' GROUP by hd.ID_DoiTuong'
	set @sql3 = @sql + @SqlQuery + @sql2;
	exec (@sql3);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc4]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'select a.ID_DoiTuong from
				(
				select hd.ID_DoiTuong, Count(*) as SoLanMuaHang from BH_HoaDon hd
				where hd.ChoThanhToan = 0
				and hd.LoaiHoaDon = 1
				and hd.ID_DoiTuong is not null
				GROUP by hd.ID_DoiTuong
				) a
				where SoLanMuaHang';
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc5]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'SELECT a.ID_DoiTuong
				FROM
				(
				SELECT
				td.ID_DoiTuong,
				SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoCuoiKy --(+ khách nợ, - nợ khách)
				FROM
				(
				-- Doanh thu từ bán hàng 
				SELECT 
				bhd.ID_DoiTuong,
				NULL AS GiaTriTra,
				SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
				NULL AS TienThu,
				NULL AS TienChi
				FROM BH_HoaDon bhd
				JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
				WHERE bhd.LoaiHoaDon = ''1''  AND bhd.ChoThanhToan = ''false'' AND bhd.NgayLapHoaDon < DATEADD(day, 1, GETDATE())
				AND dt.LoaiDoiTuong = 1
				GROUP BY bhd.ID_DoiTuong
				-- gia tri trả từ bán hàng
				UNION All
				SELECT bhd.ID_DoiTuong,
				SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
				NULL AS DoanhThu,
				NULL AS TienThu,
				NULL AS TienChi
				FROM BH_HoaDon bhd
				JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
				WHERE bhd.LoaiHoaDon = ''6'' AND bhd.ChoThanhToan = ''false'' AND bhd.NgayLapHoaDon < DATEADD(day, 1, GETDATE())
				AND dt.LoaiDoiTuong = 1
				GROUP BY bhd.ID_DoiTuong
				-- sổ quỹ
				UNION ALL
				SELECT 
				qhdct.ID_DoiTuong,
				NULL AS GiaTriTra,
				NULL AS DoanhThu,
				SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
				NULL AS TienChi
				FROM Quy_HoaDon qhd
				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
				JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				WHERE qhd.LoaiHoaDon = ''11'' AND  (qhd.TrangThai  != ''0'' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon <  DATEADD(day, 1, GETDATE())
				AND dt.LoaiDoiTuong = 1
				GROUP BY qhdct.ID_DoiTuong
    
				UNION ALL
				SELECT 
				qhdct.ID_DoiTuong,
				NULL AS GiaTriTra,
				NULL AS DoanhThu,
				NULL AS TienThu,
				SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
				FROM Quy_HoaDon qhd
				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
				JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				WHERE qhd.LoaiHoaDon = ''12'' AND (qhd.TrangThai  != ''0'' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < DATEADD(day, 1, GETDATE())
				AND dt.LoaiDoiTuong = 1
				GROUP BY qhdct.ID_DoiTuong
				) AS td
				GROUP BY td.ID_DoiTuong
				) a
				where NoCuoiKy ';
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc6]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'Select a.ID_DoiTuong from
				(
				select ID as ID_DoiTuong, MONTH(NgaySinh_NgayTLap) as ThangSinh from DM_DoiTuong where LoaiDoiTuong = 1
				and NgaySinh_NgayTLap is not null
				and DinhDang_NgaySinh like ''%mm%''
				) a
				where ThangSinh ';
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc7]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'Select a.ID_DoiTuong from
				(
				select ID as ID_DoiTuong,  NgaySinh_NgayTLap from DM_DoiTuong where LoaiDoiTuong = 1
				and NgaySinh_NgayTLap is not null
				and DinhDang_NgaySinh like ''%y%''
				) a
				Where NgaySinh_NgayTLap ';
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc8]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'select ID as ID_DoiTuong from DM_DoiTuong where LoaiDoiTuong = 1
				and GioiTinhNam ';
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getlist_DoiTuong_HinhThuc9]", parametersAction: p => new
            {
                SqlQuery = p.String()
            }, body: @"declare @sql  [nvarchar](max);
	declare @sql2  [nvarchar](max);
	set @sql = 'select ID as ID_DoiTuong from DM_DoiTuong dt
				where ID_TinhThanh is not null
				and ID_TinhThanh ';
	set @sql2 = @sql + @SqlQuery;
	exec (@sql2);");

            CreateStoredProcedure(name: "[dbo].[getList_HangHoaXuatHuybyID]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid()
            }, body: @"select hdct.ID,
	hdct.ID_HoaDon,
	hdct.ID_DonViQuiDoi,
	hd.MaHoaDon,
	nv.TenNhanVien,
	dvqd.MaHangHoa,
	hh.TenHangHoa,
	hh.TenHangHoa +
	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end +
    Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
	Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
	CAST(ROUND(1, 0) as float) as TonKho,
	CAST(ROUND(3, 0) as float) as TrangThaiMoPhieu,
	CAST(ROUND(hdct.SoLuong, 3) as float) as SoLuong,
	CAST(ROUND(hdct.SoLuong, 3) as float) as SoLuongXuatHuy,
	CAST(ROUND(hdct.DonGia, 0) as float) as DonGia,
	CAST(ROUND(hdct.GiaVon, 0) as float) as GiaVon,
	CAST(ROUND(hdct.ThanhTien, 0) as float) as GiaTriHuy, 
	CAST(ROUND(hdct.TienChietKhau, 0) as float) as GiamGia,
	hd.NgayLapHoaDon,
	hd.ID_NhanVien,
	hdct.GhiChu as DienGiai
	from BH_HoaDon_ChiTiet hdct
	inner join BH_HoaDon hd on hdct.ID_HoaDon = hd.ID
	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
	left join 
    		(
    			Select Main.id_hanghoa,
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
    		) tt on hh.ID = tt.id_hanghoa
	where hdct.ID_HoaDon = @ID_HoaDon
	ORDER BY hdct.SoThuTu");

            CreateStoredProcedure(name: "[dbo].[getList_NhomDoiTuongChiTiet]", parametersAction: p => new
            {
                ID_NhomDoiTuong = p.Guid()
            }, body: @"Select 
	ct.LoaiDieuKien as IDHT,
	ct.STT as IDDK,
	Case when ct.LoaiDieuKien = 1 then N'Tống mua (trừ trả hàng)' else
	Case when ct.LoaiDieuKien = 2 then N'Tống mua' else
	Case when ct.LoaiDieuKien = 3 then N'Thời gian mua hàng' else
	Case when ct.LoaiDieuKien = 4 then N'Số lần mua hàng' else
	Case when ct.LoaiDieuKien = 5 then N'Công nợ hiện tại' else
	Case when ct.LoaiDieuKien = 6 then N'Tháng sinh' else
	Case when ct.LoaiDieuKien = 7 then N'Tuổi' else
	Case when ct.LoaiDieuKien = 8 then N'Giới tính' else
	Case when ct.LoaiDieuKien = 9 then N'Khu vực' else N'Vùng miền' end end end end end end end end end as HinhThuc,
	ct.LoaiDieuKien as LoaiHinhThuc,
	Case when ct.LoaiSoSanh = 1 then N'>' else
	Case when ct.LoaiSoSanh = 2 then N'>=' else
	Case when ct.LoaiSoSanh = 3 then N'=' else
	Case when ct.LoaiSoSanh = 4 then N'<=' else
	Case when ct.LoaiSoSanh = 5 then N'<' else N'Khác' end end end end end as LoaiSoSanh,
	ct.LoaiSoSanh as SoSanh,
	CAST(ROUND(ISNULL(ct.GiaTriSo, 0), 0) as float) as GiaTri,
	ct.GiaTriThoiGian as TimeBy,
	Case when ct.GiaTriBool is not null then ct.GiaTriBool else 'true' end as GioiTinh,
	CAST(ROUND(ISNULL(ct.GiaTriSo, 0), 0) as float) as ThangSinh,
	ct.GiaTriKhuVuc as ID_KhuVuc,
	(select TenTinhThanh from DM_TinhThanh where ID = ct.GiaTriKhuVuc) as KhuVuc,
	ct.GiaTriVungMien as ID_VungMien,
	(select TenVung from DM_VungMien where ID = ct.GiaTriVungMien) as VungMien
	From DM_NhomDoiTuong_ChiTiet ct
	where ct.ID_NhomDoiTuong = @ID_NhomDoiTuong
	order by ct.STT");

            CreateStoredProcedure(name: "[dbo].[insert_DM_NhomDoiTuong_ChiTiet]", parametersAction: p => new
            {
                ID_NhomDoiTuong = p.Guid(),
                LoaiDieuKien = p.Int(),
                LoaiSoSang = p.Int(),
                GiaTriSo = p.Double(),
                GiaTriBool = p.Boolean(),
                GiaTriThoiGian = p.DateTime(),
                GiaTriKhuVuc = p.Guid(),
                GiaTriVungMien = p.Guid(),
                STT = p.Int()
            }, body: @"if (@STT = 1)
	BEGIN
		Delete from DM_NhomDoiTuong_ChiTiet where ID_NhomDoiTuong = @ID_NhomDoiTuong
	END
	if (@LoaiDieuKien = 1 or @LoaiDieuKien = 2 or @LoaiDieuKien = 4 or @LoaiDieuKien = 5 or @LoaiDieuKien = 6 or @LoaiDieuKien = 7)
	BEGIN
		INSERT INTO DM_NhomDoiTuong_ChiTiet (ID, ID_NhomDoiTuong, LoaiDieuKien, LoaiSoSanh, GiaTriSo, STT)
		VALUES (NEWID(), @ID_NhomDoiTuong, @LoaiDieuKien, @LoaiSoSang, @GiaTriSo, @STT)
	END
	if (@LoaiDieuKien = 3)
	BEGIN
		INSERT INTO DM_NhomDoiTuong_ChiTiet (ID, ID_NhomDoiTuong, LoaiDieuKien, LoaiSoSanh, GiaTriThoiGian, STT)
		VALUES (NEWID(), @ID_NhomDoiTuong, @LoaiDieuKien, @LoaiSoSang, @GiaTriThoiGian, @STT)
	END
	if (@LoaiDieuKien = 8)
	BEGIN
		INSERT INTO DM_NhomDoiTuong_ChiTiet (ID, ID_NhomDoiTuong, LoaiDieuKien,LoaiSoSanh, GiaTriBool, STT)
		VALUES (NEWID(), @ID_NhomDoiTuong, @LoaiDieuKien,'3', @GiaTriBool, @STT)
	END
	if (@LoaiDieuKien = 9)
	BEGIN
		INSERT INTO DM_NhomDoiTuong_ChiTiet (ID, ID_NhomDoiTuong, LoaiDieuKien, LoaiSoSanh, GiaTriKhuVuc, STT)
		VALUES (NEWID(), @ID_NhomDoiTuong, @LoaiDieuKien, @LoaiSoSang, @GiaTriKhuVuc, @STT)
	END
	if (@LoaiDieuKien = 10)
	BEGIN
		INSERT INTO DM_NhomDoiTuong_ChiTiet (ID, ID_NhomDoiTuong, LoaiDieuKien, LoaiSoSanh, GiaTriVungMien, STT)
		VALUES (NEWID(), @ID_NhomDoiTuong, @LoaiDieuKien, @LoaiSoSang, @GiaTriVungMien, @STT)
	END	");

            CreateStoredProcedure(name: "[dbo].[insert_DoiTuong_Nhom]", parametersAction: p => new
            {
                LoaiCapNhat = p.Int(),
                DK_Xoa = p.Int(),
                ID_DoiTuong = p.Guid(),
                ID_NhomDoiTuong = p.Guid()
            }, body: @"IF (@LoaiCapNhat = 1)
	BEGIN
		if (@DK_xoa = 1)
		begin
			DELETE from DM_DoiTuong_Nhom where ID_NhomDoiTuong = @ID_NhomDoiTuong
			SET @DK_xoa = @DK_xoa + 1;
		end
		Insert into DM_DoiTuong_Nhom (ID, ID_DoiTuong, ID_NhomDoiTuong)
		Values (NEWID(), @ID_DoiTuong, @ID_NhomDoiTuong)
	END
	ELSE -- thêm mới khách hàng chưa thuộc nhóm
	BEGIN
		Declare @DK [int];
		set @DK = (select COUNT(*) from DM_DoiTuong_Nhom where ID_DoiTuong = @ID_DoiTuong and ID_NhomDoiTuong = @ID_NhomDoiTuong);
		if (@DK = 0)
		BEGIN  
			Insert into DM_DoiTuong_Nhom (ID, ID_DoiTuong, ID_NhomDoiTuong)
			Values (NEWID(), @ID_DoiTuong, @ID_NhomDoiTuong)
		END
	END");

            CreateStoredProcedure(name: "[dbo].[insert_NhomDoiTuong]", parametersAction: p => new
            {
                LoaiDieuKien = p.Int(),
                ID = p.Guid(),
                LoaiDoiTuong = p.Int(),
                MaNhomDoiTuong = p.String(),
                TenNhomDoiTuong = p.String(),
                GhiChu = p.String(),
                GiamGia = p.Double(),
                GiamGiaTheoPhanTram = p.Boolean(),
                NguoiTao = p.String(),
                TimeCreate = p.DateTime()
            }, body: @"IF (@LoaiDieuKien = 1)
	BEGIN
		if (@GiamGia >= 0)
		BEGIN
			INSERT INTO DM_NhomDoiTuong (ID, LoaiDoiTuong, MaNhomDoiTuong, TenNhomDoiTuong, GhiChu, GiamGia, GiamGiaTheoPhanTram, NguoiTao, NgayTao)
			VALUES (@ID, @LoaiDoiTuong, @MaNhomDoiTuong, @TenNhomDoiTuong, @GhiChu, @GiamGia, @GiamGiaTheoPhanTram, @NguoiTao, @TimeCreate)
		END
		else
		BEGIN
			INSERT INTO DM_NhomDoiTuong (ID, LoaiDoiTuong, MaNhomDoiTuong, TenNhomDoiTuong, GhiChu, GiamGia, GiamGiaTheoPhanTram, NguoiTao, NgayTao)
			VALUES (@ID, @LoaiDoiTuong, @MaNhomDoiTuong, @TenNhomDoiTuong, @GhiChu, null, @GiamGiaTheoPhanTram, @NguoiTao, @TimeCreate)
		END
	END
	IF (@LoaiDieuKien = 2)
	BEGIN
		if (@GiamGia >= 0)
		BEGIN
			UPDATE DM_NhomDoiTuong set TenNhomDoiTuong = @TenNhomDoiTuong, GhiChu = @GhiChu, GiamGia = @GiamGia, GiamGiaTheoPhanTram = @GiamGiaTheoPhanTram
			Where ID = @ID
		END
		else
		BEGIN
			UPDATE DM_NhomDoiTuong set TenNhomDoiTuong = @TenNhomDoiTuong, GhiChu = @GhiChu, GiamGia = null, GiamGiaTheoPhanTram = @GiamGiaTheoPhanTram
			Where ID = @ID
		END
	END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[delete_DoiTuong_Nhom]");
            DropStoredProcedure("[dbo].[delete_NhomDoiTuongChiTiet]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc1]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc10]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc2]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc3]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc4]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc5]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc6]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc7]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc8]");
            DropStoredProcedure("[dbo].[getlist_DoiTuong_HinhThuc9]");
            DropStoredProcedure("[dbo].[getList_HangHoaXuatHuybyID]");
            DropStoredProcedure("[dbo].[getList_NhomDoiTuongChiTiet]");
            DropStoredProcedure("[dbo].[insert_DM_NhomDoiTuong_ChiTiet]");
            DropStoredProcedure("[dbo].[insert_DoiTuong_Nhom]");
            DropStoredProcedure("[dbo].[insert_NhomDoiTuong]");
        }
    }
}