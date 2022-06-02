namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateTableNguoiDung_20180816 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getList_DMLoHang_TonKho_byMaLoHang]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                ID_DonViQuiDoi = p.String(),
                timeChotSo = p.DateTime(),
                MaLoHang = p.String()
            }, body: @"SELECT 
    		dvqd.ID as ID_DonViQuiDoi,
			dvqd.GiaVon,
    		CAST(ROUND((ISNULL(HangHoa.TonCuoiKy,0) / dvqd.TyLeChuyenDoi), 3) as float) as TonCuoiKy
    	FROM
    		   DM_LoHang lh
    		   left join
    		(
    		SELECT
    		td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    			td.ID_LoHang,
    		SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			--SUM(ISNULL(td.TonKho,0)) AS TonCuoiKy
    		FROM
    		(
    		-- lấy danh sách hàng hóa tồn kho
    			SELECT 
    			dvqd.ID As ID_DonViQuiDoi,
    				Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    			NULL AS SoLuongNhap,
    			NULL AS SoLuongXuat,
    			SUM(ISNULL(cs.TonKho, 0)) as TonKho
    			FROM DonViQuiDoi dvqd
    			left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			left join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
    				where dvqd.ladonvichuan = '1'
    					and dvqd.ID = @ID_DonViQuiDoi
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
    				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                             
    
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
    				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    			OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    
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
    				LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    			WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
    			AND bhd.NgayLapHoaDon >= @timeChotSo
    				AND dvqd.ID = @ID_DonViQuiDoi
    			GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang
    		) AS td
    			GROUP BY ID_DonViQuiDoi, ID_LoHang
    		) AS HangHoa
    		on lh.ID = HangHoa.ID_LoHang
    		inner join DM_HangHoa hh on lh.ID_HangHoa = hh.ID
    		inner join DonViQuiDoi dvqd on hh.ID = dvqd.ID_HangHoa
    		where dvqd.ID = @ID_DonViQuiDoi
			and lh.MaLoHang = @MaLoHang
    		order by lh.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[GetListDM_LoHangHetHan]", parametersAction: p => new
            {
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.Guid()
            }, body: @"SELECT * FROM
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
		AND bhd.NgayLapHoaDon < @timeEnd
		GROUP BY bhdct.ID_LoHang
    ) AS td 
	right join DM_LoHang dmlo1 on td.ID_LoHang = dmlo1.ID
	left join DonViQuiDoi dvqd on dmlo1.ID_HangHoa = dvqd.ID_HangHoa
	where (DATEPART(day, dmlo1.NgayHetHan)= DATEPART(day, @timeEnd)) and 
                             (DATEPART(month, dmlo1.NgayHetHan)= DATEPART(month, @timeEnd)) and (DATEPART(year, dmlo1.NgayHetHan)= DATEPART(YEAR, @timeEnd)) and dvqd.Xoa is null and dvqd.LaDonViChuan = 1
    GROUP BY dmlo1.ID,dmlo1.MaLoHang, dmlo1.NgaySanXuat, dmlo1.NgayHetHan, dvqd.MaHangHoa
	) as tbDMLo
	where tbDMLo.TonKho > 0");

            CreateStoredProcedure(name: "[dbo].[import_DanhMucHangHoaLoHang]", parametersAction: p => new
            {
                TenNhomHangHoaCha = p.String(),
                MaNhomHangHoaCha = p.String(),
                timeCreateNHHCha = p.DateTime(),
                TenNhomHangHoa = p.String(),
                MaNhomHangHoa = p.String(),
                timeCreateNHH = p.DateTime(),
                LaHangHoa = p.Boolean(),
                timeCreateHH = p.DateTime(),
                TenHangHoa = p.String(),
                TenHangHoa_KhongDau = p.String(),
                TenHangHoa_KyTuDau = p.String(),
                GhiChu = p.String(),
                QuyCach = p.String(),
                DuocBanTrucTiep = p.Boolean(),
                MaDonViCoBan = p.String(),
                MaHangHoa = p.String(),
                TenDonViTinh = p.String(),
                GiaVon = p.String(),
                GiaBan = p.String(),
                timeCreateDVQD = p.DateTime(),
                LaDonViChuan = p.Boolean(),
                TyLeChuyenDoi = p.String(),
                MaHoaDon = p.String(),
                DienGiai = p.String(),
                TonKho = p.String(),
                timeCreateHD = p.DateTime(),
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                MaHangHoaChaCungLoai = p.String(),
                MaLoHang = p.String(),
                NgaySanXuat = p.DateTime(),
                NgayHetHan = p.DateTime(),
                QuanLyTheoLoHang = p.Boolean()
            }, body: @"-- insert NhomHangHoa parent
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
    DECLARE @ID_NhomHangHoaCha  as uniqueidentifier
    	set @ID_NhomHangHoaCha = null
    	if (len(@TenNhomHangHoaCha) > 0)
    	Begin
    		SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha, @MaNhomHangHoaCha, '1', '1', '1', '1', 'admin', @timeCreateNHHCha, null)
    		End
    	End
    -- insert NhomHangHoa
    	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
    		if (len(@TenNhomHangHoa) > 0)
    	Begin
    		SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @MaNhomHangHoa, '1', '1', '1', '1', 'admin', @timeCreateNHH, @ID_NhomHangHoaCha)
    		End
    	End
    -- insert HangHoa
    	DECLARE @ID_HangHoa  as uniqueidentifier
    	set @ID_HangHoa = newID();
    		DECLARE @LaChaCungLoai  as int
    		set @LaChaCungLoai = 1;
    		DECLARE @ID_HangHoaCungLoai  as uniqueidentifier
    	set @ID_HangHoaCungLoai = newID();
    			if(len(@MaHangHoaChaCungLoai) > 0)
    			Begin 
    				set @ID_HangHoaCungLoai = (select ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1');
    				if (len(@ID_HangHoaCungLoai) > 0)
    				Begin
    					set @LaChaCungLoai = 0;
    				End
    				else
    				Begin 
    					set @ID_HangHoaCungLoai = newID(); 
    				End
    			End
				DECLARE @ID_LoHang  as uniqueidentifier
    					set @ID_LoHang = null
		DECLARE @ID_DonViQuiDoi  as uniqueidentifier
    		set @ID_DonViQuiDoi = null;
    		set @ID_DonViQuiDoi = (Select ID from DonViQuiDoi where MaHangHoa = @MaHangHoa);
			if (@ID_DonViQuiDoi is null or len(@ID_DonViQuiDoi) = 0)
			Begin
			set @ID_DonViQuiDoi = NEWID();
    			if (@MaDonViCoBan = '' or len (@MaDonViCoBan) = 0)
    			Begin
    					insert into DM_HangHoa (ID, ID_HangHoaCungLoai, LaChaCungLoai, ID_NhomHang, LaHangHoa, NgayTao,NguoiTao, TenHangHoa,TenHangHoa_KhongDau, TenHangHoa_KyTuDau,
    					TenKhac, TheoDoi, GhiChu, ChiPhiThucHien, ChiPhiTinhTheoPT, QuyCach, DuocBanTrucTiep, QuanLyTheoLoHang)
    					Values (@ID_HangHoa, @ID_HangHoaCungLoai, @LaChaCungLoai ,@ID_NhomHangHoa, @LaHangHoa, @timeCreateHH, 'admin', @TenHangHoa, @TenHangHoa_KhongDau, @TenHangHoa_KyTuDau,
    					@MaHangHoaChaCungLoai, '1', @GhiChu, '0', '1', @QuyCachF, @DuocBanTrucTiep, @QuanLyTheoLoHang)

						if(@MaLoHang != '')
						Begin
							SET @ID_LoHang =  (Select ID FROM DM_LoHang where MaLoHang = @MaLoHang and ID_HangHoa = @ID_HangHoa);
    						if (@ID_LoHang is null or len(@ID_LoHang) = 0)
    						BeGin
    							SET @ID_LoHang = newID();
    							insert into DM_LoHang(ID, ID_HangHoa, MaLoHang, TenLoHang, NgaySanXuat, NgayHetHan, NguoiTao, NgayTao)
    							values (@ID_LoHang, @ID_HangHoa, @MaLoHang, @MaLoHang, @NgaySanXuat, @NgayHetHan, 'admin', GETDATE())
    						End
						End
    			end
    			else
    			Begin
    				set @ID_HangHoa = (Select ID_HangHoa from DonViQuiDoi where MaHangHoa like @MaDonViCoBan);
    			end
			-- insert DonViQuiDoi
    			 insert into DonViQuiDoi (ID, MaHangHoa, TenDonViTinh, ID_HangHoa, TyLeChuyenDoi, LaDonViChuan, GiaVon, GiaNhap, GiaBan, NguoiTao, NgayTao)
    			Values (@ID_DonViQuiDoi, @MaHangHoa,@TenDonViTinh, @ID_HangHoa,@TyLeChuyenDoiF, @LaDonViChuan, @GiaVonF, '0',@GiaBanF, 'admin', @timeCreateDVQD)
			End
	-- insert DM_LoHang
    -- insert TonKho
    	if (@TonKhoF > 0 and @LaHangHoa = 1)
    	Begin
    		DECLARE @ThanhToanF as float
    		set @ThanhToanF = (select CAST(ROUND(@TonKhoF * @GiaVonF, 0) as float))
    	DECLARE @ID_HoaDon  as uniqueidentifier
    	set @ID_HoaDon = newID();
    		insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    			values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, @timeCreateHD, @ID_DonVi, @ID_NhanVien, @TonKhoF, '0',@TonKhoF, '0', '0', '0', '0', '9')
    	    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    			Values (NEWID(), @ID_HoaDon, '1', @TonKhoF, '0', @TonKhoF, '0', '0','0','0','0',@ThanhToanF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
    	End");

            CreateStoredProcedure(name: "[dbo].[import_DanhMucHangHoaLoHang_Update]", parametersAction: p => new
            {
                LoaiUpdate = p.Int(),
                ID_HangHoa = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                GiaTriTang = p.String(),
                GiaTriGiam = p.String(),
                TongTienLech = p.String(),
                TongChenhLech = p.String(),
                SoLuongThucTe = p.String(),
                SoLuongTang = p.String(),
                SoLuongGiam = p.String(),
                TenNhomHangHoaCha = p.String(),
                MaNhomHangHoaCha = p.String(),
                timeCreateNHHCha = p.DateTime(),
                TenNhomHangHoa = p.String(),
                MaNhomHangHoa = p.String(),
                timeCreateNHH = p.DateTime(),
                LaHangHoa = p.Boolean(),
                timeCreateHH = p.DateTime(),
                TenHangHoa = p.String(),
                TenHangHoa_KhongDau = p.String(),
                TenHangHoa_KyTuDau = p.String(),
                GhiChu = p.String(),
                QuyCach = p.String(),
                DuocBanTrucTiep = p.Boolean(),
                MaDonViCoBan = p.String(),
                MaHangHoa = p.String(),
                TenDonViTinh = p.String(),
                GiaVon = p.String(),
                GiaBan = p.String(),
                timeCreateDVQD = p.DateTime(),
                LaDonViChuan = p.Boolean(),
                TyLeChuyenDoi = p.String(),
                MaHoaDon = p.String(),
                DienGiai = p.String(),
                TonKho = p.String(),
                timeCreateHD = p.DateTime(),
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                MaHangHoaChaCungLoai = p.String(),
                MaLoHang = p.String(),
                NgaySanXuat = p.DateTime(),
                NgayHetHan = p.DateTime()
            }, body: @"-- Khai báo biến update
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
    		SET @ID_NhomHangHoaCha =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoaCha and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoaCha is null or len(@ID_NhomHangHoaCha) = 0)
    		BeGin
    			SET @ID_NhomHangHoaCha = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoaCha, @TenNhomHangHoaCha, @MaNhomHangHoaCha, '1', '1', '1', '1', 'admin', @timeCreateNHHCha, null)
    		End
    	End
    -- insert NhomHangHoa
    	DECLARE @ID_NhomHangHoa  as uniqueidentifier
    	set @ID_NhomHangHoa = null
    	if (len(@TenNhomHangHoa) > 0)
    	Begin
    		SET @ID_NhomHangHoa =  (Select ID FROM DM_NhomHangHoa where TenNhomHangHoa like @TenNhomHangHoa and LaNhomHangHoa = '1');
    		if (@ID_NhomHangHoa is null or len(@ID_NhomHangHoa) = 0)
    		BeGin
    			SET @ID_NhomHangHoa = newID();
    			insert into DM_NhomHangHoa (ID, TenNhomHangHoa, MaNhomHangHoa, HienThi_BanThe, HienThi_Chinh, HienThi_Phu, LaNhomHangHoa, NguoiTao, NgayTao, ID_Parent)
    			values (@ID_NhomHangHoa, @TenNhomHangHoa, @MaNhomHangHoa, '1', '1', '1', '1', 'admin', @timeCreateNHH, @ID_NhomHangHoaCha)
    		End
    	End
    -- Update HangHoa
    		DECLARE @LaChaCungLoai  as int
    		set @LaChaCungLoai = 1;
    		DECLARE @ID_HangHoaCungLoai  as uniqueidentifier
    	set @ID_HangHoaCungLoai = newID();
    			if(len(@MaHangHoaChaCungLoai) > 0) -- nếu có thông tin mã hàng hóa cùng loại
    			Begin 
    				set @ID_HangHoaCungLoai = (select ID_HangHoaCungLoai from DM_HangHoa where TenKhac = @MaHangHoaChaCungLoai and LaChaCungLoai = '1'); -- lấy ID_HangHoaCungLoai có chung tên khác
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
		DECLARE @ID_LoHang  as uniqueidentifier
    	set @ID_LoHang = null
		if(@MaLoHang != '')
		Begin
			update DM_HangHoa set QuanLyTheoLoHang = '1' where ID = @ID_HangHoa;
			SET @ID_LoHang =  (Select ID FROM DM_LoHang where MaLoHang = @MaLoHang and ID_HangHoa = @ID_HangHoa);
    		if (@ID_LoHang is null or len(@ID_LoHang) = 0)
    		BeGin
    			SET @ID_LoHang = newID();
    			insert into DM_LoHang(ID, ID_HangHoa, MaLoHang, TenLoHang, NgaySanXuat, NgayHetHan, NguoiTao, NgayTao)
    			values (@ID_LoHang, @ID_HangHoa, @MaLoHang, @MaLoHang, @NgaySanXuat, @NgayHetHan, 'admin', GETDATE())
    		End
		End
    -- update DonViQuiDoi
    		update DonViQuiDoi set TenDonViTinh = @TenDonViTinh, TyLeChuyenDoi = @TyLeChuyenDoiF, LaDonViChuan = @LaDonViChuan, GiaBan = @GiaBanF, NguoiSua ='admin', NgaySua =@timeCreateDVQD
    			Where ID = @ID_DonViQuiDoi
    -- insert kiểm kê TonKho
    	if (@LoaiUpdate = 2 and @LaHangHoa = 1)
    	Begin
    	DECLARE @ID_HoaDon  as uniqueidentifier
    	set @ID_HoaDon = newID();
    		insert into BH_HoaDon (ID, MaHoaDon, NguoiTao, DienGiai, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongChiPhi, TongTienHang, TongGiamGia, PhaiThanhToan, TongChietKhau, TongTienThue, ChoThanhToan, LoaiHoaDon)
    			values (@ID_HoaDon, @MaHoaDon, 'admin', @DienGiai, @timeCreateHD, @ID_DonVi, @ID_NhanVien, @SoLuongTangF, @SoLuongGiamF,@TongChenhLechF, @GiaTriTangF, @GiaTriGiamF, @TongTienLech, '0', '9')
    	    insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang)
    			Values (NEWID(), @ID_HoaDon, '1', @TongChenhLechF, '0', @SoLuongThucTeF, '0', @TonKhoF,'0','0','0',@TongTienLechF, @GiaVonF,'0',@ID_DonViQuiDoi, @ID_LoHang)
    	End");
        }

        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_DMLoHang_TonKho_byMaLoHang]");
            DropStoredProcedure("[dbo].[GetListDM_LoHangHetHan]");
            DropStoredProcedure("[dbo].[import_DanhMucHangHoaLoHang]");
            DropStoredProcedure("[dbo].[import_DanhMucHangHoaLoHang_Update]");
        }
    }
}