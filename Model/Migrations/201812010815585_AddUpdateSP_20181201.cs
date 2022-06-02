namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20181201 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[SP_GetAllHoaDon_byIDPhieuThuChi]", parametersAction: p => new
            {
                ID_PhieuThuChi = p.String()
            }, body: @"select hd.MaHoaDon, hd.NgayLapHoaDon, 
		hd.TongTienHang as TongTienThu, -- assign TongTienThu used to bind at grid
		sum(qct.TienThu) as DaChi, -- assign DaChi used to bind at grid (DaThu)
		hd.TongTienHang - sum(qct.TienThu) as TienThu,  -- assign TienThu used to bind at grid (ConPhaiThu)
		N'Đã thanh toán' as GhiChu,
		MAX(QuyXML.PhuongThucTT) as PhuongThuc
	from Quy_HoaDon_ChiTiet qct
	join BH_HoaDon hd  on qct.ID_HoaDonLienQuan = hd.ID
	LEFT JOIN 
	(	
				Select Main.ID_HoaDonLienQuan,
				   Left(Main.PThuc_SoQuy,Len(Main.PThuc_SoQuy)-1) As PhuongThucTT
				From
				(
					Select distinct main1.ID_HoaDonLienQuan, 
						(
							Select distinct (tbl1.PhuongThuc) + ',' AS [text()]
							From 
								(
								SELECT qct2.ID_HoaDonLienQuan, tk.TaiKhoanPos,
										case when qct2.TienMat > 0 then
											case when qct2.TienGui > 0  then
												case when tk.TaiKhoanPos is null OR tk.TaiKhoanPos='0' then N'Chuyển khoản'
												else N'POS' end
											else
												N'Tiền mặt' end
										else
											case when qct2.TienGui > 0  then
												case when tk.TaiKhoanPos is null OR tk.TaiKhoanPos='0' then N'Chuyển khoản'
												else N'POS' end
											end
										end as PhuongThuc
								FROM Quy_HoaDon_ChiTiet qct2
								left join DM_TaiKhoanNganHang tk on qct2.ID_TaiKhoanNganHang = tk.ID
								where qct2.ID_HoaDon like @ID_PhieuThuChi
								) tbl1
							Where tbl1.ID_HoaDonLienQuan = main1.ID_HoaDonLienQuan --group by tbl1.id_hoadon, tbl1.PhuongThuc
							For XML PATH ('')
						) PThuc_SoQuy
					From Quy_HoaDon_ChiTiet main1 group by main1.ID_HoaDonLienQuan
				) [Main] 
		
	) QuyXML on qct.ID_HoaDonLienQuan = QuyXML.ID_HoaDonLienQuan
	where qct.ID_HoaDon like @ID_PhieuThuChi
	group by qct.ID_HoaDonLienQuan, hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienHang");

            CreateStoredProcedure(name: "[dbo].[SP_GetInforHoaDon_ByID]", parametersAction: p => new
            {
                ID_HoaDon = p.String()
            }, body: @"select 
		hd.ID,
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hd.TongTienHang,
		ISNULL(hd.TongGiamGia,0) + ISNULL(hd.KhuyeMai_GiamGia, 0) as TongGiamGia,
		CAST(ISNULL(hd.PhaiThanhToan,0) as float)  as PhaiThanhToan,
		CAST(ISNULL(TongThuChi,0) as float) as KhachDaTra,	
		case when hd.ID_DoiTuong is null then N'Khách lẻ' else dt.TenDoiTuong end as TenDoiTuong,
		case when hd.ID_BangGia is null then N'Bảng giá chung' else bg.TenGiaBan end as TenBangGia,
		case when hd.ID_NhanVien is null then N'' else nv.TenNhanVien end as TenNhanVien,
		case when hd.ID_DonVi is null then N'' else dv.TenDonVi end as TenDonVi,
		case when hd.ID_DonVi is null then N'' else dv.TenDonVi end as TenDonVi,	
		case when hd.NgayApDungGoiDV is null then '' else  convert(varchar(14), hd.NgayApDungGoiDV ,103) end  as NgayApDungGoiDV,
		case when hd.HanSuDungGoiDV is null then '' else  convert(varchar(14), hd.HanSuDungGoiDV ,103) end as HanSuDungGoiDV,
		hd.NguoiTao as NguoiTaoHD,
		hd.DienGiai,
		hd.ID_DonVi,
		-- get avoid error at variable at class BH_HoaDonDTO
		NEWID() as ID_DonViQuiDoi,


		case 
			when hd.LoaiHoaDon = 1 OR hd.LoaiHoaDon =6 OR hd.LoaiHoaDon =19 then
									case when hd.ChoThanhToan is null then N'Đã hủy'
									else N'Hoàn thành' end
			when hd.LoaiHoaDon = 3 then
									case when hd.YeuCau ='1' then N'Phiếu tạm'
									else 
										case when hd.YeuCau ='2' then  N'Đang giao hàng'
										else 
											case when hd.YeuCau ='3' then N'Hoàn thành'
											else  N'Đã hủy' end
										end
									end
		end as TrangThai													
	from BH_HoaDon hd
	left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
	left join DM_DonVi dv on hd.ID_DonVi= dv.ID
	left join DM_GiaBan bg on hd.ID_BangGia= bg.ID
	left join 
		(select qct.ID_HoaDonLienQuan, SUM(ISNULL(TongTienThu,0)) as TongThuChi
		from Quy_HoaDon_ChiTiet qct
		left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID where qhd.TrangThai ='1'
		group by qct.ID_HoaDonLienQuan) soquy on hd.ID = soquy.ID_HoaDonLienQuan		
	where hd.ID like @ID_HoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_GetInforSoQuy_ByID]", parametersAction: p => new
            {
                ID_PhieuThuChi = p.String()
            }, body: @"select 
		qhd.ID,
		qhd.MaHoaDon,
		qhd.NgayLapHoaDon,
		MAX(qhd.LoaiHoaDon) as LoaiHoaDon ,
		MAX(CAST(ISNULL(qhd.TongTienThu,0) as float)) as TongTienThu,
		MAX(ISNULL(qhd.NoiDungThu,'')) as NoiDungThu,
		MAX(ISNULL(nv.TenNhanVien,'')) as TenNhanVien,
		MAX(ISNULL(dt.TenDoiTuong,'')) as NguoiNopTien
	from Quy_HoaDon qhd
	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
	left join NS_NhanVien nv on qhd.ID_NhanVien= nv.ID
	left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID
	where qhd.ID like @ID_PhieuThuChi
	group by qhd.ID, qhd.MaHoaDon,qhd.NgayLapHoaDon");

            Sql(@"ALTER PROCEDURE [dbo].[GetListNhanVienEdit]
    @ID_NhanVien [uniqueidentifier]
AS
BEGIN
    select * from NS_NhanVien nv where nv.TrangThai is null and nv.DaNghiViec != 1 and nv.ID not in (select ID_NhanVien from HT_nguoiDung where ID_NhanVien != @ID_NhanVien)
END");
            Sql(@"ALTER PROCEDURE [dbo].[UpDateGiaVonDMHangHoaKhiTaoHD]
    @IDHD [uniqueidentifier],
	@DateHD [datetime],
	@IDDonVi [uniqueidentifier]
AS
BEGIN
		DECLARE @TableChiTietHHUpdate TABLE (ID_ChiNhanhHD UNIQUEIDENTIFIER, ID_DonViQuiDoiHH UNIQUEIDENTIFIER, ID_LoHangHH UNIQUEIDENTIFIER, DateTimeHD DATETIME, ChoThanhToanHD BIT, YeuCauHD NVARCHAR(Max))
		INSERT INTO @TableChiTietHHUpdate
		SELECT * FROM(
			SELECT @IDDonVi as ID_ChiNhanhHD,cthd.ID_DonViQuiDoi as ID_DonViQuiDoiHH, cthd.ID_LoHang as ID_LoHangHH, @DateHD as DateTimeHD, hd.ChoThanhToan as ChoThanhToanHD, hd.YeuCau as YeuCauHD
			FROM BH_HoaDon_ChiTiet cthd
			left join BH_HoaDon hd on cthd.ID_HoaDon = hd.ID
			where hd.ID = @IDHD
		) as cthh

		DECLARE @ID_ChiNhanhHD UNIQUEIDENTIFIER;
		DECLARE @ID_DonViQuiDoiHH UNIQUEIDENTIFIER;
		DECLARE @ID_LoHangHH UNIQUEIDENTIFIER;
		DECLARE @DateTimeHD DATETIME;
		DECLARE @ChoThanhToanHD BIT;
		DECLARE @YeuCauHD NVARCHAR(MAX);

		DECLARE CS_ItemCTHH CURSOR SCROLL LOCAL FOR SELECT * FROM @TableChiTietHHUpdate
		--foreach tất cả chi tiết của các hàng hóa trong hd cần update
		OPEN CS_ItemCTHH 
		FETCH FIRST FROM CS_ItemCTHH INTO @ID_ChiNhanhHD, @ID_DonViQuiDoiHH,@ID_LoHangHH,@DateTimeHD, @ChoThanhToanHD,@YeuCauHD
		WHILE @@FETCH_STATUS = 0
			BEGIN
					DECLARE @IDHangHoaLayRaListUpdate UNIQUEIDENTIFIER;
    				SELECT @IDHangHoaLayRaListUpdate = ID_HangHoa FROM DonViQuiDoi WHERE ID = @ID_DonViQuiDoiHH
					-- list chi tiet sau ngay xoa
					IF(@ChoThanhToanHD = 'true' or @YeuCauHD = '3')
					BEGIN
						DECLARE @TableListCT TABLE(IDCT UNIQUEIDENTIFIER, NgayLapHoaDonCT DATETIME) INSERT INTO @TableListCT SELECT TOP(1) bhhd.ID as IDCT, bhhd.NgayLapHoaDon as NgayLapHoaDonCT FROM
						BH_HoaDon_ChiTiet bhct
						left join BH_HoaDon bhhd on bhhd.ID = bhct.ID_HoaDon
						LEFT JOIN DonViQuiDoi dvqd on dvqd.ID = bhct.ID_DonViQuiDoi
						where bhhd.LoaiHoaDon != 3 and bhhd.LoaiHoaDon != 19 and ID_HangHoa = @IDHangHoaLayRaListUpdate and bhhd.ChoThanhToan = 'false' and (bhct.ID_LoHang = @ID_LoHangHH OR @ID_LoHangHH IS NULL) and bhhd.NgayLapHoaDon < @DateTimeHD and ((bhhd.ID_DonVi = @ID_ChiNhanhHD and ((bhhd.YeuCau != '2' and bhhd.YeuCau != '3') or bhhd.YeuCau is null)) or (bhhd.YeuCau = '4'  and bhhd.ID_CheckIn = @ID_ChiNhanhHD))
						ORDER BY NgayLapHoaDon desc
						DECLARE @COUNTCT INT;
						SELECT @COUNTCT = COUNT(IDCT) FROM @TableListCT 
						IF(@COUNTCT > 0)
						BEGIN
							SELECT @DateTimeHD = NgayLapHoaDonCT FROM @TableListCT
						END
						DELETE FROM @TableListCT
					END

					DECLARE @GiaVonTrungBinh BIT;
					SELECT @GiaVonTrungBinh = GiaVonTrungBinh from HT_CauHinhPhanMem where ID_DonVi = @ID_ChiNhanhHD

    				DECLARE @TongHopNhapXuat TABLE (ID UNIQUEIDENTIFIER, ID_HoaDonCT UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, ID_DonVi UNIQUEIDENTIFIER,ID_DonViQuiDoi UNIQUEIDENTIFIER, ID_HangHoa UNIQUEIDENTIFIER, TienChietKhau FLOAT,DonGia FLOAT,GiaVon FLOAT, LoaiHoaDon INT, YeuCau nvarchar(max), SoLuong FLOAT, ChoThanhToan bit, ID_LoHang UNIQUEIDENTIFIER, TongGiamGia FLOAT, TongTienHang FLOAT, TyLeChuyenDoi float) 
    					INSERT INTO @TongHopNhapXuat
    				SELECT * from(
    				SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgayLapHoaDon, hd.ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
    				FROM BH_HoaDon_ChiTiet bhct
    				left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
    				left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE hd.LoaiHoaDon != 3 and hd.LoaiHoaDon != 19 and hd.ChoThanhToan = 'false' and dvqd.ID_HangHoa = @IDHangHoaLayRaListUpdate and (bhct.ID_LoHang = @ID_LoHangHH OR @ID_LoHangHH IS NULL) and hd.NgayLapHoaDon >= @DateTimeHD and ((hd.ID_DonVi = @ID_ChiNhanhHD and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)) or (hd.YeuCau = '4'  and hd.ID_CheckIn = @ID_ChiNhanhHD))
    				--order by hd.NgayLapHoaDon
    
    				UNION all
    
    				SELECT hd.ID, bhct.ID as ID_HoaDonCT, hd.NgaySua as NgayLapHoaDon, hd.ID_CheckIn as ID_DonVi,bhct.ID_DonViQuiDoi, dvqd.ID_HangHoa,bhct.TienChietKhau, bhct.DonGia,bhct.GiaVon, hd.LoaiHoaDon,hd.YeuCau, bhct.SoLuong, hd.ChoThanhToan, bhct.ID_LoHang, hd.TongGiamGia, hd.TongTienHang, dvqd.TyLeChuyenDoi
    				FROM BH_HoaDon_ChiTiet bhct
    				left join BH_HoaDon hd on bhct.ID_HoaDon = hd.ID
    				left join DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
    				left join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE hd.ChoThanhToan = 'false' and dvqd.ID_HangHoa = @IDHangHoaLayRaListUpdate and (bhct.ID_LoHang = @ID_LoHangHH OR @ID_LoHangHH IS NULL) and hd.NgaySua >= @DateTimeHD and hd.LoaiHoaDon = 10 and hd.YeuCau = '4' and hd.ID_CheckIn = @ID_ChiNhanhHD
    				) as a
    				order by a.NgayLapHoaDon
    
				DECLARE @ID UNIQUEIDENTIFIER;
				DECLARE @ID_HoaDonCT UNIQUEIDENTIFIER;
				DECLARE @NgayLapHoaDon DATETIME;
				DECLARE @ID_DonVi UNIQUEIDENTIFIER;
				DECLARE @ID_DonViQuiDoi UNIQUEIDENTIFIER;
				DECLARE @ID_HangHoa UNIQUEIDENTIFIER;
				DECLARE @TienChietKhau FLOAT;
				DECLARE @DonGia FLOAT;
				DECLARE @GiaVon FLOAT;
				DECLARE @LoaiHoaDon INT;
				DECLARE @YeuCau NVARCHAR(MAX);
				DECLARE @SoLuong FLOAT;
				DECLARE @ChoThanhToan bit;
				DECLARE @ID_LoHang UNIQUEIDENTIFIER;
				DECLARE @TongGiamGia FLOAT;
				DECLARE @TongTienHang FLOAT;
				DECLARE @TyLeChuyenDoi FLOAT;
    
				DECLARE @GiaVonUpDate FLOAT;	
				DECLARE @CountHDTongHopNhapXuat INT;
				SELECT @CountHDTongHopNhapXuat = COUNT(ID) FROM @TongHopNhapXuat
				IF(@CountHDTongHopNhapXuat > 0)
				BEGIN
					DECLARE CS_Item CURSOR SCROLL LOCAL FOR SELECT * FROM @TongHopNhapXuat ORDER BY NgayLapHoaDon
					--foreach tất cả chi tiết của các hàng hóa trong hd cần update
					OPEN CS_Item 
					FETCH FIRST FROM CS_Item INTO @ID, @ID_HoaDonCT,@NgayLapHoaDon,@ID_DonVi, @ID_DonViQuiDoi,@ID_HangHoa, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong,@ChoThanhToan, @ID_LoHang, @TongGiamGia, @TongTienHang, @TyLeChuyenDoi
					WHILE @@FETCH_STATUS = 0
						BEGIN
    						Declare @ID_DonViQuiDoiDVT nvarchar(max);
    						SELECT @ID_DonViQuiDoiDVT =   
    						SUBSTRING(
    								(
    									SELECT ','+CAST(ST1.ID as nvarchar(max))
    									FROM dbo.DonViQuiDoi ST1
    									WHERE ST1.ID_HangHoa = @ID_HangHoa
    									ORDER BY ST1.ID
    									FOR XML PATH ('')
    								), 2, 1000)
    						FROM DonViQuiDoi ST2 WHERE ST2.ID_HangHoa = @ID_HangHoa
							DECLARE @TonKhoHienTai FLOAT;
							SET @TonKhoHienTai = ISNULL([dbo].FUNC_TinhSLTonKhiTaoHD(@ID_DonVi,@ID_HangHoa,@ID_LoHang, @NgayLapHoaDon),0)
    						SET @GiaVonUpDate = [dbo].FUNC_GiaVon(@ID,@ID_DonViQuiDoi,@ID_DonViQuiDoiDVT, @ID_HangHoa,@ID_LoHang, @ID_DonVi,@NgayLapHoaDon, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong, @TongGiamGia, @TongTienHang,@TyLeChuyenDoi,@TonKhoHienTai)
    	
    						-- UPDATE Giá vốn cho từng hóa đơn chi tiết
    						IF(@LoaiHoaDon = 10)		
    						BEGIN
    							DECLARE @ID_DonViCheckIn [uniqueidentifier];
    							SELECT @ID_DonViCheckIn = ID_CheckIn FROM BH_HoaDon WHERE ID = @ID
								IF(@GiaVonTrungBinh = 1)
								BEGIN
    								IF(@YeuCau = '1' OR (@YeuCau = '4' AND @ID_DonVi != @ID_DonViCheckIn))
    								BEGIN
    									UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate * @TyLeChuyenDoi where ID = @ID_HoaDonCT
    								END
    								IF(@YeuCau = '4' AND @ID_DonVi = @ID_DonViCheckIn)
    								BEGIN
    									UPDATE BH_HoaDon_ChiTiet SET GiaVon_NhanChuyenHang = @GiaVonUpDate * @TyLeChuyenDoi where ID = @ID_HoaDonCT
    								END
								END
    						END
    						ELSE
    						BEGIN
    							IF(@LoaiHoaDon = 18)
    							BEGIN
    								UPDATE BH_HoaDon_ChiTiet SET DonGia = @GiaVonUpDate * @TyLeChuyenDoi, PTChietKhau = (Case When GiaVon - (@GiaVonUpDate * @TyLeChuyenDoi) > 0 then GiaVon - (@GiaVonUpDate * @TyLeChuyenDoi) else 0 end), TienChietKhau = (Case When GiaVon - (@GiaVonUpDate * @TyLeChuyenDoi) > 0 then 0 else GiaVon - (@GiaVonUpDate * @TyLeChuyenDoi) end)  where ID = @ID_HoaDonCT
									SELECT @GiaVonUpDate = GiaVon / @TyLeChuyenDoi FROM BH_HoaDon_ChiTiet WHERE ID = @ID_HoaDonCT
    							END
								ELSE IF(@LoaiHoaDon = 8)
    							BEGIN
    								DECLARE @ThanhTienNew FLOAT;
    								SET @ThanhTienNew = @GiaVonUpDate * @TyLeChuyenDoi * @SoLuong
									IF(@GiaVonTrungBinh = 1)
									BEGIN
    									UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate * @TyLeChuyenDoi, ThanhTien = @ThanhTienNew where ID = @ID_HoaDonCT
									END
    							END
    							ELSE IF(@LoaiHoaDon = 9)
    							BEGIN
									DECLARE @SoLuongNew FLOAT;
									SELECT @SoLuongNew = ThanhTien - @TonKhoHienTai FROM BH_HoaDon_ChiTiet WHERE ID = @ID_HoaDonCT
									IF(@SoLuong < 0)
									BEGIN
										IF(@SoLuongNew < 0)
										BEGIN
											DECLARE @LechGiamNew FLOAT;
											SELECT @LechGiamNew = TongTienHang - (@SoLuong - @SoLuongNew) FROM BH_HoaDon WHERE ID = @ID
											UPDATE BH_HoaDon SET TongTienHang = @LechGiamNew, TongGiamGia = @LechGiamNew + TongChiPhi WHERE ID = @ID
										END
										IF(@SoLuongNew > 0)
										BEGIN
											DECLARE @LechTangNew FLOAT;
											SELECT @LechTangNew = TongChiPhi + @SoLuongNew FROM BH_HoaDon WHERE ID = @ID
											UPDATE BH_HoaDon SET TongChiPhi = @LechTangNew, TongTienHang = TongTienHang - @SoLuong , TongGiamGia = @LechTangNew + (TongTienHang - @SoLuong) WHERE ID = @ID
										END
										IF(@SoLuongNew = 0)
										BEGIN
											UPDATE BH_HoaDon SET TongTienHang = TongTienHang - @SoLuong , TongGiamGia = TongChiPhi + (TongTienHang - @SoLuong) WHERE ID = @ID
										END
									END
									IF(@SoLuong > 0)
									BEGIN
										IF(@SoLuongNew > 0)
										BEGIN
											UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi - (@SoLuong - @SoLuongNew) , TongGiamGia = TongChiPhi - (@SoLuong - @SoLuongNew) + TongTienHang WHERE ID = @ID
										END
										IF(@SoLuongNew < 0)
										BEGIN
											UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi - @SoLuong, TongTienHang = TongTienHang + @SoLuongNew, TongGiamGia = TongChiPhi - @SoLuong + (TongTienHang + @SoLuongNew) WHERE ID = @ID
										END
										IF(@SoLuongNew = 0)
										BEGIN
											UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi - @SoLuong, TongGiamGia = TongChiPhi - @SoLuong + TongTienHang WHERE ID = @ID
										END
									END
									IF(@SoLuong = 0)
									BEGIN
										IF(@SoLuongNew > 0)
										BEGIN
											UPDATE BH_HoaDon SET TongChiPhi = TongChiPhi + @SoLuongNew, TongGiamGia = TongChiPhi + @SoLuongNew + TongTienHang WHERE ID = @ID
										END
										IF(@SoLuongNew < 0)
										BEGIN
											UPDATE BH_HoaDon SET TongTienHang = TongTienHang + @SoLuongNew, TongGiamGia = TongChiPhi + @SoLuongNew + TongTienHang WHERE ID = @ID
										END
									END
									IF(@GiaVonTrungBinh = 1)
									BEGIN
    									UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate * @TyLeChuyenDoi,TienChietKhau = @TonKhoHienTai, SoLuong = ThanhTien - @TonKhoHienTai, ThanhToan = @GiaVonUpDate * @TyLeChuyenDoi * (ThanhTien - @TonKhoHienTai)  where ID = @ID_HoaDonCT
									END
									ELSE
									BEGIN
										UPDATE BH_HoaDon_ChiTiet SET TienChietKhau = @TonKhoHienTai, SoLuong = ThanhTien - @TonKhoHienTai, ThanhToan = @GiaVon * (ThanhTien - @TonKhoHienTai)  where ID = @ID_HoaDonCT
									END
    							END
								ELSE IF(@LoaiHoaDon = 1)
								BEGIN
									IF(@GiaVonTrungBinh = 1)
									BEGIN
    									UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate * @TyLeChuyenDoi where ID = @ID_HoaDonCT
										DECLARE @ID_ChiTietDinhLuong UNIQUEIDENTIFIER;
										DECLARE @GiaVonUpDLDV FLOAT;
										SELECT @ID_ChiTietDinhLuong = ID_ChiTietDinhLuong FROM BH_HoaDon_ChiTiet WHERE ID = @ID_HoaDonCT
										IF(@ID_ChiTietDinhLuong is not null)
										BEGIN
											SELECT @GiaVonUpDLDV = SUM(ISNULL(dlct.SoLuong, 0) * ISNULL(dlct.GiaVon,0)) FROM BH_HoaDon_ChiTiet dlct WHERE ID_ChiTietDinhLuong = @ID_ChiTietDinhLuong AND ID != @ID_ChiTietDinhLuong
											GROUP BY ID_ChiTietDinhLuong;
											BEGIN TRANSACTION
												UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDLDV / SoLuong where ID = @ID_ChiTietDinhLuong;
											COMMIT TRANSACTION
										END
									END
								END
    							ELSE
    							BEGIN
									IF(@GiaVonTrungBinh = 1)
									BEGIN
    									UPDATE BH_HoaDon_ChiTiet SET GiaVon = @GiaVonUpDate * @TyLeChuyenDoi where ID = @ID_HoaDonCT
									END
    							END
    						END
    						-- END update giá vốn cho từng hóa đơn chi tiết
    
    						--Update giá vốn cho từng đơn vi qui đổi theo tỷ lệ chuyển đổi
						IF(@GiaVonTrungBinh = 1 or @LoaiHoaDon = 18)
						BEGIN
    						DECLARE @TableDonViQuiDoi table(ID_DonViQuiDoiGV UNIQUEIDENTIFIER, TyLeChuyenDoiGV FLOAT) insert into @TableDonViQuiDoi 
    						select dvqdgv.ID as ID_DonViQuiDoiGV, dvqdgv.TyLeChuyenDoi as TyLeChuyenDoiGV from DonViQuiDoi dvqdgv where dvqdgv.ID_HangHoa = @ID_HangHoa
    
    						DECLARE @ID_DonViQuiDoiGV UNIQUEIDENTIFIER;
    						DECLARE @TyLeChuyenDoiGV FLOAT;
    		
    							 DECLARE CS_ItemGV CURSOR SCROLL LOCAL FOR SELECT * FROM @TableDonViQuiDoi
    							 -- foreach đơn vị tính để update vào dm_giavon
    							 OPEN CS_ItemGV 
    							 FETCH FIRST FROM CS_ItemGV INTO @ID_DonViQuiDoiGV, @TyLeChuyenDoiGV
    							 WHILE @@FETCH_STATUS = 0
    							 BEGIN
    								DECLARE @GiaVonCheck FLOAT; 
    								select @GiaVonCheck = COUNT(ID) from DM_GiaVon where ID_DonViQuiDoi = @ID_DonViQuiDoiGV and ID_DonVi = @ID_DonVi and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL)
    								IF(@GiaVonCheck = 0)
    								BEGIN
    									INSERT INTO DM_GiaVon(ID, ID_DonViQuiDoi, ID_DonVi, ID_LoHang, GiaVon) values (newID(), @ID_DonViQuiDoiGV, @ID_DonVi,@ID_LoHang, @GiaVonUpDate * @TyLeChuyenDoiGV)
    								END
    								ELSE
    								BEGIN
    									DECLARE @GiaVonNew FLOAT; 
    									SET @GiaVonNew = @GiaVonUpDate * @TyLeChuyenDoiGV
    									UPDATE DM_GiaVon SET GiaVon = @GiaVonNew where ID_DonViQuiDoi = @ID_DonViQuiDoiGV and (ID_LoHang = @ID_LoHang OR @ID_LoHang IS NULL) and ID_DonVi = @ID_DonVi
    								END
    
    								FETCH NEXT FROM CS_ItemGV INTO @ID_DonViQuiDoiGV, @TyLeChuyenDoiGV
    							 END
    							 CLOSE CS_ItemGV
    							 DEALLOCATE CS_ItemGV 
    							 DELETE FROM @TableDonViQuiDoi
    						--end update giá vốn cho từng đơn vị qui đổi
						END 
						FETCH NEXT FROM CS_Item INTO @ID, @ID_HoaDonCT,@NgayLapHoaDon,@ID_DonVi, @ID_DonViQuiDoi,@ID_HangHoa, @TienChietKhau,@DonGia,@GiaVon, @LoaiHoaDon,@YeuCau,@SoLuong,@ChoThanhToan,@ID_LoHang, @TongGiamGia, @TongTienHang,@TyLeChuyenDoi	 
    					END
						CLOSE CS_Item
						DEALLOCATE CS_Item
				END
				ELSE
				BEGIN
					--Update giá vốn cho từng đơn vi qui đổi theo tỷ lệ chuyển đổi
					IF(@GiaVonTrungBinh = 1 or @LoaiHoaDon = 18)
					BEGIN
    					DECLARE @TableDonViQuiDoi1 table(ID_DonViQuiDoiGV1 UNIQUEIDENTIFIER, TyLeChuyenDoiGV1 FLOAT) insert into @TableDonViQuiDoi1 
    					select dvqdgv.ID as ID_DonViQuiDoiGV1, dvqdgv.TyLeChuyenDoi as TyLeChuyenDoiGV1 from DonViQuiDoi dvqdgv where dvqdgv.ID_HangHoa = @IDHangHoaLayRaListUpdate
    					DECLARE @ID_DonViQuiDoiGV1 UNIQUEIDENTIFIER;
    					DECLARE @TyLeChuyenDoiGV1 FLOAT;
    		
    						 DECLARE CS_ItemGV1 CURSOR SCROLL LOCAL FOR SELECT * FROM @TableDonViQuiDoi1
    						 -- foreach đơn vị tính để update vào dm_giavon
    						 OPEN CS_ItemGV1 
    						 FETCH FIRST FROM CS_ItemGV1 INTO @ID_DonViQuiDoiGV1, @TyLeChuyenDoiGV1
    						 WHILE @@FETCH_STATUS = 0
    						 BEGIN
    								UPDATE DM_GiaVon SET GiaVon = 0 where ID_DonViQuiDoi = @ID_DonViQuiDoiGV1 and (ID_LoHang = @ID_LoHangHH OR @ID_LoHangHH IS NULL) and ID_DonVi = @ID_ChiNhanhHD
    								FETCH NEXT FROM CS_ItemGV1 INTO @ID_DonViQuiDoiGV1, @TyLeChuyenDoiGV1
    						 END
    						 CLOSE CS_ItemGV1
    						 DEALLOCATE CS_ItemGV1 
    						 DELETE FROM @TableDonViQuiDoi1
    					--end update giá vốn cho từng đơn vị qui đổi
					END
				END

			FETCH NEXT FROM CS_ItemCTHH INTO @ID_ChiNhanhHD, @ID_DonViQuiDoiHH,@ID_LoHangHH,@DateTimeHD, @ChoThanhToanHD,@YeuCauHD
			END
		CLOSE CS_ItemCTHH
		DEALLOCATE CS_ItemCTHH
END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[SP_GetAllHoaDon_byIDPhieuThuChi]");
            DropStoredProcedure("[dbo].[SP_GetInforHoaDon_ByID]");
            DropStoredProcedure("[dbo].[SP_GetInforSoQuy_ByID]");
        }
    }
}
