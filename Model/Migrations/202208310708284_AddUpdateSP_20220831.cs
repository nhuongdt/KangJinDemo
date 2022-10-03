namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20220831 : DbMigration
    {
        public override void Up()
        {
			CreateStoredProcedure(name: "[dbo].[CreateAgainPhieuXuatKho_WhenUpdateTPDL]", parametersAction: p => new
			{
				ID_CTHoaDon = p.Guid()
			}, body: @"SET NOCOUNT ON;

		
   	---- ==========  INSERT AGAIN CTXUAT NEW ===========
			
			--- get cthd new
		declare @ctHDNew table (ID uniqueidentifier, ID_ChiTietDinhLuong uniqueidentifier, ID_ChiTietGoiDV  uniqueidentifier, 
			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
			SoLuong float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max),
			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(max)				
		)
		insert into @ctHDNew
		select ct.ID, ct.ID_ChiTietDinhLuong, ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
					ct.SoLuong, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,
					hh.LaHangHoa,
					hh.TenHangHoa,
					qd.MaHangHoa
		from BH_HoaDon_ChiTiet ct 
		join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		where ct.ID_ChiTietDinhLuong= @ID_CTHoaDon
		and (ct.ChatLieu is null or ct.ChatLieu !='5')
		
			
				declare @MaHoaDon varchar(max), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
				@NgayLapHoaDon datetime, @NguoiTao nvarchar(max),@LoaiHoaDon int = 35 ---- xuatkho nguyenvatlieu (LoaiHoaDon = 35)

				declare @ID_HoaDonMua uniqueidentifier = (select ID_HoaDon from BH_HoaDon_ChiTiet where ID = @ID_CTHoaDon)
				---- get infor hoadon
				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao
				from BH_HoaDon where id= @ID_HoaDonMua

				declare @count int = (select count (ID) from  @ctHDNew where LaHangHoa = 1)		
							

				IF @count > 0
				BEGIN

					declare  @TongGiaTriXuat float = 
					(select sum(ct.GiaVon * SoLuong)
					from @ctHDNew ct
					where ct.ID != ct.ID_ChiTietDinhLuong
					)

					declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,2,NgayLapHoaDon)) from BH_HoaDon where LoaiHoaDon= 35 and ID_HoaDon = @ID_HoaDonMua)
					if @maxNgayLap is null set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)

						---- INSERT HD XUATKHO ----
					declare @ID_XuatKho uniqueidentifier = newID()	, @ngayXuatKho datetime= @maxNgayLap ,@maXuatKho nvarchar(max)		

					---- find all PhieuXuatKho by ID_hoadongoc
					declare @countPhieuXK int = (select count(id) from BH_HoaDon where LoaiHoaDon= 35 and ID_HoaDon= @ID_HoaDonMua)
					declare @maXuatKhoGoc nvarchar(max) = (select top 1 MaHoaDon from BH_HoaDon where LoaiHoaDon= 35 and ID_HoaDon= @ID_HoaDonMua order by NgayTao)
					
    				if @countPhieuXK = 0
						begin
    						---- neu chua co phieuxuat
								declare @tblMa table (MaHoaDon nvarchar(max)) 	---- get mahoadon xuatkho
    							insert into @tblMa
    							exec GetMaHoaDonMax_byTemp @LoaiHoaDon, @ID_DonVi, @ngayxuatkho
    							select @maXuatKho = MaHoaDon from @tblMa
						end
    				else 
						begin
							---- exist: tang maphieuxuat theo so lan xuat
							 set @maXuatKho = CONCAT(@maXuatKhoGoc, '_', @countPhieuXK)    						 
						end
					

						declare @xuatchoDV nvarchar(max)
						= (select top 1 CONCAT(N', Dịch vụ: ', TenHangHoa, '(', MaHangHoa, ')') from @ctHDNew where ID= @ID_CTHoaDon)

					
						insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon, NgayLapHoaDon, ID_DonVi, ID_NhanVien, ID_DoiTuong,
						TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
    					PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai)

    					values (@ID_XuatKho, @LoaiHoaDon, @maXuatKho,@ID_HoaDonMua, @ngayXuatKho, @ID_DonVi,@ID_NhanVien, @ID_DoiTuong,
						@TongGiaTriXuat,0,0,0,0,0, @TongGiaTriXuat,0,'1',N'Tạm lưu', GETDATE(), @NguoiTao, 
						concat(N'Cập nhật phiếu xuất nguyên liệu cho hóa đơn ', @MaHoaDon, @xuatchoDV) )


							---- INSERT CT XUATKHO ----
						insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_ChiTietGoiDV, ID_ChiTietDinhLuong, --- !! important save ID_ChiTietDinhLuong --> used to caculator GiaVon for DichVu
								ID_DonViQuiDoi, ID_LoHang, SoLuong, DonGia, GiaVon, ThanhTien, ThanhToan, 
    							PTChietKhau, TienChietKhau, PTChiPhi, TienChiPhi, TienThue, An_Hien, TonLuyKe, GhiChu,  ChatLieu)		
						select 
						NEWid(),
						@ID_XuatKho,
						row_number() over( order by (select 1)) as SoThuTu,
						ctsc.ID_ChiTietGoiDV,
						ctsc.ID_DichVu,
						ctsc.ID_DonViQuiDoi,
						ctsc.ID_LoHang,
						ctsc.SoLuong, ctsc.GiaVon, ctsc.GiaVon, ctsc.GiaTri, 
						0,0,0,0,0,0,'1', ctsc.TonLuyKe, ctsc.GhiChu,''
					from 
					(
					---- get infor tp + dichvu
						select 
							cttp.ID as ID_ChiTietGoiDV,
							dv.ID_DonViQuiDoi as ID_DichVu,
							cttp.ID_DonViQuiDoi, 
							cttp.ID_LoHang,
							cttp.SoLuong,
							cttp.GiaVon,
							cttp.GiaVon* cttp.SoLuong as GiaTri,			
							cttp.TonLuyKe,
							isnull(cttp.GhiChu,'') as GhiChu
						from @ctHDNew cttp		
						join @ctHDNew dv on cttp.ID_ChiTietDinhLuong = dv.ID					
						and cttp.SoLuong > 0		
						and cttp.LaHangHoa='1'
						) ctsc

						--begin try  
						--	exec dbo.UpdateTonLuyKeCTHD_whenUpdate @ID_XuatKho, @ID_DonVi, @ngayXuatKho		
						--	exec dbo.UpdateGiaVon_WhenEditCTHD @ID_XuatKho, @ID_DonVi, @ngayXuatKho		
						--end try
						--begin catch
						--end catch
						
					
				END");

			CreateStoredProcedure(name: "[dbo].[CreatePhieuXuat_FromHoaDon]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid(),
				LoaiXuatKho = p.Int(),
				IsXuatNgayThuoc = p.Boolean(),
				TrangThai = p.Boolean()
			}, body: @"SET NOCOUNT ON;



		---- get ctxuatkho old
		declare @ctXuatOld table (ID uniqueidentifier, ID_ChiTietGoiDV uniqueidentifier, 
		ID_DonViQuiDoi uniqueidentifier, ID_LoHang  uniqueidentifier, SoLuong float, TienChietKhau float)
		insert into @ctXuatOld
		select ctx.ID, ctx.ID_ChiTietGoiDV, ctx.ID_DonViQuiDoi, ctx.ID_LoHang, ctx.SoLuong, ctx.TienChietKhau
		from BH_HoaDon_ChiTiet ctx 	
		join BH_HoaDon hdx on ctx.ID_HoaDon= hdx.ID
		where hdx.ID_HoaDon= @ID_HoaDon and hdx.LoaiHoaDon= @LoaiXuatKho and hdx.ChoThanhToan is not null

		---- get cthd new 
		---- TienChietKhau: soluong xuat/1 ngaythuoc, SoLuong: tong sl xuat
		declare @ctHDNew table (ID uniqueidentifier,
			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
			SoLuong float, TienChietKhau float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max),
			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(max)				
		)
		if @IsXuatNgayThuoc ='1'
		begin
			insert into @ctHDNew
			select ct.ID, ct.ID_DonViQuiDoi, ct.ID_LoHang,
						ct.SoLuong, ct.TienChietKhau, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,
						hh.LaHangHoa,
						hh.TenHangHoa,
						qd.MaHangHoa
			from BH_HoaDon_ChiTiet ct 
			join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			where ct.ID_HoaDon= @ID_HoaDon
			and ct.ChatLieu='6' --- (chi lay SP ngaythuoc)
			and hh.LaHangHoa='1'
		end
		else
		begin
			insert into @ctHDNew
			select ct.ID, ct.ID_DonViQuiDoi, ct.ID_LoHang,
						ct.SoLuong, ct.TienChietKhau, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,
						hh.LaHangHoa,
						hh.TenHangHoa,
						qd.MaHangHoa
			from BH_HoaDon_ChiTiet ct 
			join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			where ct.ID_HoaDon= @ID_HoaDon
			and ct.ID_ChiTietDinhLuong is null --- hdle + hdbaohanh
			and (ct.ChatLieu is null or ct.ChatLieu not in ('5','6'))
			and hh.LaHangHoa='1'
		end
		
		---- compare ctdinhluong old & new --> find cthd same
		----- 1. ctxuatkho left join ctNew --> check if not exist ctXuatKho
		declare @tblSame table (ID uniqueidentifier, ID_CTMuaNew uniqueidentifier, isSame int)
		insert into @tblSame
		select 
			ctold.ID,
			ctNew.ID as ID_CTMua,
			iif(ctNew.ID_DonViQuiDoi is null, 0, iif(ctOld.SoLuong = ctnew.SoLuong,1,0)) as Same
		from @ctXuatOld ctold	
		left join @ctHDNew ctNew 		
		on  ctold.ID_DonViQuiDoi = ctNew.ID_DonViQuiDoi
			and (ctold.ID_LoHang= ctNew.ID_LoHang or (ctold.ID_LoHang is null and ctNew.ID_LoHang is null))

			------ 2. left join ctNew & ctXuatkho (~ exist in ctNew but not exist ctXuatkho)
			declare @count_TPNew int = 
			(
				select count (*)
				from
				(
				select 
					ctNew.ID,
					iif(ctold.ID_DonViQuiDoi is null, 0, iif(ctOld.SoLuong = ctnew.SoLuong,1,0)) as isSame
				from @ctHDNew ctNew 				
				left join @ctXuatOld ctold	on ctold.ID_DonViQuiDoi = ctNew.ID_DonViQuiDoi
				and (ctold.ID_LoHang= ctNew.ID_LoHang or (ctold.ID_LoHang is null and ctNew.ID_LoHang is null))
			)tbl where isSame = 0) 

		
			
		if not exists (select isSame from @tblSame) 
		or (select count (*) from @tblSame where isSame = 0) > 0 --- if find elm not same: huy + insert again
		or @count_TPNew > 0
		begin			
		
					--- ===== HUY PHIEU XUATKHO OLD + CHAY LAI TONKHO ======
					update BH_HoaDon set ChoThanhToan= null where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= @LoaiXuatKho
					declare @ID_HDHuy uniqueidentifier, @ID_DonViHuy uniqueidentifier, @NgayLapHoaDonHuy datetime
					declare _curHuy cursor
					for
					select ID, ID_DonVi, NgayLapHoaDon from BH_HoaDon where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= @LoaiXuatKho order by NgayLapHoaDon 
					open _curHuy
					FETCH NEXT FROM _curHuy
					INTO @ID_HDHuy, @ID_DonViHuy,@NgayLapHoaDonHuy
					WHILE @@FETCH_STATUS = 0
					begin						
						BEGIN TRY  
							exec dbo.UpdateTonLuyKeCTHD_whenUpdate @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy
							exec dbo.UpdateGiaVon_WhenEditCTHD @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy
						end try
						begin catch
						end catch

						FETCH NEXT FROM _curHuy
						INTO  @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy		
					end
					CLOSE _curHuy;
					DEALLOCATE _curHuy;
					

				
					---- ==========  INSERT AGAIN CTXUAT NEW (only insert if exist ctNew) ===========		
				if exists (select ID from @ctHDNew)
				begin
					declare @YeuCau nvarchar(max)
					if @TrangThai ='1' set @YeuCau =N'Tạm lưu'
					else set @YeuCau = N'Hoàn thành'

			
				declare @MaHoaDon varchar(max), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
				@NgayLapHoaDon datetime, @NguoiTao nvarchar(max), @IsChuyenPhatNhanh bit='0'
				---- get infor hoadon
				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao, @IsChuyenPhatNhanh = An_Hien
				from BH_HoaDon where id= @ID_HoaDon
									
				
						---- find all PhieuXuatKho by ID_hoadongoc: used tang mahoadon theo solanxuat
					declare @countPhieuXK int = (select count(id) from BH_HoaDon where LoaiHoaDon= @LoaiXuatKho and ID_HoaDon = @ID_HoaDon)
					declare @maXuatKhoGoc nvarchar(max) = (select top 1 MaHoaDon from BH_HoaDon where LoaiHoaDon= @LoaiXuatKho and ID_HoaDon = @ID_HoaDon order by NgayTao)	
					declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,5,NgayLapHoaDon)) from BH_HoaDon where ID_HoaDon = @ID_HoaDon)
					if @maxNgayLap is null set @maxNgayLap = DATEADD(MILLISECOND,5,@NgayLapHoaDon)

					declare @TongGiaTriXuat float = (select sum(isnull(GiaVon,0) * SoLuong) from @ctHDNew)

				
						---- INSERT HD XUATKHO ----
						 declare @ID_XuatKho uniqueidentifier = newID()	, @ngayXuatKho datetime= getdate(),@maXuatKho nvarchar(max)		
						
						set @ngayXuatKho = @maxNgayLap --- phieuxuat phai sau hoadon

						 ---- get mahoadon xuatkho
						declare @tblMa table (MaHoaDon nvarchar(max)) 	
						if @countPhieuXK = 0
    						begin
    							insert into @tblMa
    							exec GetMaHoaDonMax_byTemp @LoaiXuatKho, @ID_DonVi, @ngayxuatkho
    							select @maXuatKho = MaHoaDon from @tblMa
								
								set @countPhieuXK = 1
								set @maXuatKhoGoc = @maXuatKho
							end
						else
							begin
								set @maXuatKho = CONCAT(@maXuatKhoGoc, '_', @countPhieuXK)    	
								set @countPhieuXK += 1
							end
					

						declare @nhomHoTro nvarchar(max) = '', @sLoaiXuat nvarchar(max) = ''
						if @IsXuatNgayThuoc ='1'
							set @nhomHoTro= concat(N', nhóm dịch vụ ',(select TenNhomHangHoa from BH_HoaDon hd join DM_NhomHangHoa nhom on hd.ID_CheckIn = nhom.ID where hd.ID = @ID_HoaDon))

						if @LoaiXuatKho = 37
							set @sLoaiXuat = concat(N'Xuất hỗ trợ ngày thuốc, hóa đơn ', @MaHoaDon)
						if @LoaiXuatKho = 38
							set @sLoaiXuat = concat(N'Xuất bán lẻ, hóa đơn ', @MaHoaDon)
						if @LoaiXuatKho = 39
							set @sLoaiXuat = concat(N'Xuất bảo hành, hóa đơn ', @MaHoaDon)
						if @LoaiXuatKho = 40
							set @sLoaiXuat = concat(N'Xuất hỗ trợ chung, hóa đơn ', @MaHoaDon)

						insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon, NgayLapHoaDon, ID_DonVi, ID_NhanVien, ID_DoiTuong,
						TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
    					PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai, An_Hien) ---- an_hien: 1.chuyenphat nhanh, 0.khong

    					values (@ID_XuatKho, @LoaiXuatKho, @maXuatKho,@ID_HoaDon, @ngayXuatKho, @ID_DonVi,@ID_NhanVien, @ID_DoiTuong,
						@TongGiaTriXuat,0,0,0,0,0, @TongGiaTriXuat,0,@TrangThai,@YeuCau, GETDATE(), @NguoiTao, 
						concat(@sLoaiXuat,@nhomHoTro) ,@IsChuyenPhatNhanh)
    
							---- INSERT CT XUATKHO ----
						insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_ChiTietGoiDV, 
								ID_DonViQuiDoi, ID_LoHang, SoLuong, TienChietKhau, DonGia, GiaVon, ThanhTien, ThanhToan, 
    							PTChietKhau,  PTChiPhi, TienChiPhi, TienThue, An_Hien, TonLuyKe, GhiChu,  ChatLieu)		
						select 
						NEWid(),
						@ID_XuatKho,
						row_number() over( order by (select 1)) as SoThuTu,
						ctsc.ID_ChiTietGoiDV,
						ctsc.ID_DonViQuiDoi,
						ctsc.ID_LoHang,
						ctsc.SoLuong, 
						ctsc.TienChietKhau, 
						ctsc.GiaVon, ctsc.GiaVon, ctsc.GiaTri, 
						0,0,0,0,0,'1', ctsc.TonLuyKe, ctsc.GhiChu,''
					from 
					(
					--- ct hoadon banle or hd sudung GDV
						select 
							cttp.ID as ID_ChiTietGoiDV,							
							cttp.ID_DonViQuiDoi, 
							cttp.ID_LoHang,
							cttp.SoLuong,
							cttp.TienChietKhau,
							cttp.GiaVon,
							cttp.GiaVon* cttp.SoLuong as GiaTri,			
							cttp.TonLuyKe,
							isnull(cttp.GhiChu,'') as GhiChu
						from @ctHDNew cttp		
						where cttp.SoLuong > 0		
						) ctsc

						--begin try  
						--	exec dbo.UpdateTonLuyKeCTHD_whenUpdate @ID_XuatKho, @ID_DonVi, @ngayXuatKho		
						--	exec dbo.UpdateGiaVon_WhenEditCTHD @ID_XuatKho, @ID_DonVi, @ngayXuatKho		
						--end try
						--begin catch
						--end catch
																
			end
		end
		else
		begin
			--- dichvu cungloai (same dinhluong, same soluong (todo)
			select ct.ID,
				ROW_NUMBER() over (partition by ct.ID order by ct.ID) as RN
			from @tblSame ct
			
			---- update id_ctgoidv, id_ctdinhluong for ctxuatkho old
			update ctXuat set 
				ctXuat.ID_ChiTietGoiDV = ctSame.ID_CTMuaNew
			from BH_HoaDon_ChiTiet ctXuat
			join @tblSame ctSame on ctXuat.ID = ctSame.ID
		end");

			CreateStoredProcedure(name: "[dbo].[CreateXuatKho_NguyenVatLieu]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid(),
				TrangThai = p.Boolean(false)
			}, body: @"SET NOCOUNT ON;
    
    		--- get ctxuatkho old
    		declare @ctXuatOld table (ID uniqueidentifier, ID_ChiTietDinhLuong uniqueidentifier, ID_DonViQuiDoi uniqueidentifier, ID_LoHang  uniqueidentifier, SoLuong float)
    		insert into @ctXuatOld
    		select ctx.ID, ctx.ID_ChiTietDinhLuong, ctx.ID_DonViQuiDoi, ctx.ID_LoHang, ctx.SoLuong
    		from BH_HoaDon_ChiTiet ctx 	
    		join BH_HoaDon hdx on ctx.ID_HoaDon= hdx.ID
    		where  hdx.ID_HoaDon= @ID_HoaDon and hdx.LoaiHoaDon= 35 and hdx.ChoThanhToan is not null
    
    		--- get cthd new
    		declare @ctHDNew table (ID uniqueidentifier, ID_ChiTietDinhLuong uniqueidentifier, ID_ChiTietGoiDV  uniqueidentifier, 
    			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    			SoLuong float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max), ThanhTien float,
    			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(max)				
    		)
    		insert into @ctHDNew
    		select ct.ID, ct.ID_ChiTietDinhLuong, ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    					ct.SoLuong, ct.GiaVon, ct.TonLuyKe, ct.GhiChu, ct.ThanhTien,
    					hh.LaHangHoa,
    					hh.TenHangHoa,
    					qd.MaHangHoa
    		from BH_HoaDon_ChiTiet ct 
    		join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    		where ct.ID_HoaDon= @ID_HoaDon
    		and ct.ID_ChiTietDinhLuong is not null --- chi lay dichvu dinhluong		
    
    
    		---- compare ctdinhluong old & new --> find cthd same
    		----- 1. ctxuatkho left join ctNew --> check if not exist ctXuatKho
    		declare @tblSame table (ID uniqueidentifier, ID_DichVu uniqueidentifier, ID_CTMuaNew uniqueidentifier, isSame int)
    		insert into @tblSame
    		select 
    			ctold.ID,
    			ctNew.ID_DichVu,
    			ctNew.ID_CTMua,
    			iif(ctNew.ID_DonViQuiDoi is null, 0, iif(ctOld.Soluong = ctnew.SoLuong,1,0)) as Same
    		from @ctXuatOld ctold	
    		left join 
    		(
    			--- cthd new
    			select dv.ID_DonViQuiDoi as ID_DichVu,
    				ct.ID as ID_CTMua,
    				ct.ID_DonViQuiDoi, 
    				ct.ID_LoHang,
    				ct.SoLuong			
    			from @ctHDNew ct 	
    			join @ctHDNew dv on ct.ID_ChiTietDinhLuong = dv.ID
    			where ct.ID != ct.ID_ChiTietDinhLuong
    		) ctNew 
    		on ctold.ID_ChiTietDinhLuong = ctNew.ID_DichVu
    		and ctold.ID_DonViQuiDoi = ctNew.ID_DonViQuiDoi
    			and (ctold.ID_LoHang= ctNew.ID_LoHang or (ctold.ID_LoHang is null and ctNew.ID_LoHang is null))
    
    			------ 2. left join ctNew & ctXuatkho (~ exist in ctNew but not exist ctXuatkho)
    			declare @count_TPNew int = 
    			(
    				select count (*)
    				from
    				(
    				select 
    					ctold.ID,
    					ctNew.ID_DichVu,
    					ctNew.ID_CTMua,			
    					iif(ctold.ID_DonViQuiDoi is null, 0, iif(ctOld.Soluong = ctnew.SoLuong,1,0)) as isSame
    				from
    				(
    					--- cthd new
    				select dv.ID_DonViQuiDoi as ID_DichVu,
    					ct.ID as ID_CTMua,
    					ct.ID_DonViQuiDoi, 
    					ct.ID_LoHang,
    					ct.SoLuong			
    				from @ctHDNew ct 	
    				join @ctHDNew dv on ct.ID_ChiTietDinhLuong = dv.ID
    				where ct.ID != ct.ID_ChiTietDinhLuong
    				)ctNew
    				left join @ctXuatOld ctold	on ctold.ID_ChiTietDinhLuong = ctNew.ID_DichVu
    			and ctold.ID_DonViQuiDoi = ctNew.ID_DonViQuiDoi
    				and (ctold.ID_LoHang= ctNew.ID_LoHang or (ctold.ID_LoHang is null and ctNew.ID_LoHang is null))
    		)tbl where isSame = 0) 
    
    		if not exists (select isSame from @tblSame) 
    		or (select count (*) from @tblSame where isSame = 0) > 0 --- if find elm not same: huy + insert again
    		or @count_TPNew > 0
    		begin			
    		
    					--- ===== HUY PHIEU XUATKHO OLD + CHAY LAI TONKHO ======
    					update BH_HoaDon set ChoThanhToan= null where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35
    					declare @ID_HDHuy uniqueidentifier, @ID_DonViHuy uniqueidentifier, @NgayLapHoaDonHuy datetime
    					declare _curHuy cursor
    					for
    					select ID, ID_DonVi, NgayLapHoaDon from BH_HoaDon where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35
    					open _curHuy
    					FETCH NEXT FROM _curHuy
    					INTO @ID_HDHuy, @ID_DonViHuy,@NgayLapHoaDonHuy
    					WHILE @@FETCH_STATUS = 0
    					begin
    						BEGIN TRY  
    						exec dbo.UpdateTonLuyKeCTHD_whenUpdate @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy
    						exec dbo.UpdateGiaVon_WhenEditCTHD @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy
    						END TRY  
    						BEGIN CATCH 
    							select ERROR_MESSAGE() as Err
    						END CATCH  
    						FETCH NEXT FROM _curHuy
    						INTO  @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy		
    					end
    					CLOSE _curHuy;
    					DEALLOCATE _curHuy;
    					
    
    					---- ==========  INSERT AGAIN CTXUAT NEW ===========							
    				if exists (select ID from @ctHDNew)
    				begin
    				declare @MaHoaDon varchar(max), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
    				@NgayLapHoaDon datetime, @NguoiTao nvarchar(max),@LoaiHoaDon int = 35 ---- xuatkho nguyenvatlieu (LoaiHoaDon = 35)
    				---- get infor hoadon
    				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
    				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao
    				from BH_HoaDon where id= @ID_HoaDon
    
    				declare @count int = (select count (ID) from  @ctHDNew where LaHangHoa = 1)							
    
    				IF @count > 0
    				BEGIN
    						---- find all PhieuXuatKho by ID_hoadongoc: used tang mahoadon theo solanxuat
    					declare @countPhieuXK int = (select count(id) from BH_HoaDon where LoaiHoaDon= 35 and ID_HoaDon = @ID_HoaDon)
    					declare @maXuatKhoGoc nvarchar(max) = (select top 1 MaHoaDon from BH_HoaDon where LoaiHoaDon= 35 and ID_HoaDon = @ID_HoaDon order by NgayTao)	
    
    					declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,2,NgayLapHoaDon)) from BH_HoaDon where ID_HoaDon = @ID_HoaDon)
    					if @maxNgayLap is null set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)
    
    					declare @ID_ChiTietDinhLuong uniqueidentifier, @TongGiaTriXuat float
    
    					declare _cur cursor
    					for
    					select ct.ID_ChiTietDinhLuong, sum(ct.GiaVon * SoLuong)
    					from @ctHDNew ct
    					where ct.ID != ct.ID_ChiTietDinhLuong
    					group by ct.ID_ChiTietDinhLuong ---- group by dichvu (1 dichvu - 1phieuxuat NVL)
    
    					open _cur
    					FETCH NEXT FROM _cur
    					INTO @ID_ChiTietDinhLuong, @TongGiaTriXuat
    					WHILE @@FETCH_STATUS = 0
    					begin
    				
    						---- INSERT HD XUATKHO ----
    						 declare @ID_XuatKho uniqueidentifier = newID()	, @ngayXuatKho datetime= getdate(),@maXuatKho nvarchar(max)		
    						 declare @YeuCau nvarchar(max)
    						 if @TrangThai ='1' set @YeuCau =N'Tạm lưu'
    							else set @YeuCau = N'Hoàn thành'
    												
    						set @ngayXuatKho = @maxNgayLap 
    						
    						 ---- get mahoadon xuatkho
    						declare @tblMa table (MaHoaDon nvarchar(max)) 	
    						if @countPhieuXK = 0
    						begin
    							insert into @tblMa
    							exec GetMaHoaDonMax_byTemp @LoaiHoaDon, @ID_DonVi, @ngayxuatkho
    							select @maXuatKho = MaHoaDon from @tblMa
    								
    								set @countPhieuXK = 1
    								set @maXuatKhoGoc = @maXuatKho
    							end
    						else
    							begin
    								set @maXuatKho = CONCAT(@maXuatKhoGoc, '_', @countPhieuXK)    	
    								set @countPhieuXK += 1
    							end
    
    						declare @xuatchoDV nvarchar(max)
    						= (select top 1 CONCAT(N', Dịch vụ: ', TenHangHoa, '(', MaHangHoa, N'), Thành tiền: ', FORMAT(ThanhTien, 'N0') ) from @ctHDNew where ID= @ID_ChiTietDinhLuong)
    
    						insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon, NgayLapHoaDon, ID_DonVi, ID_NhanVien, ID_DoiTuong,
    						TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
    					PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai)
    
    					values (@ID_XuatKho, @LoaiHoaDon, @maXuatKho,@ID_HoaDon, @ngayXuatKho, @ID_DonVi,@ID_NhanVien, @ID_DoiTuong,
    						@TongGiaTriXuat,0,0,0,0,0, @TongGiaTriXuat,0, @TrangThai, @YeuCau, GETDATE(), @NguoiTao, 
    						concat(N'Xuất nguyên vật liệu cho hóa đơn ', @MaHoaDon, @xuatchoDV) )
    
    							---- INSERT CT XUATKHO ----
    						insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_ChiTietGoiDV, ID_ChiTietDinhLuong, --- !! important save ID_ChiTietDinhLuong --> used to caculator GiaVon for DichVu
    								ID_DonViQuiDoi, ID_LoHang, SoLuong, DonGia, GiaVon, ThanhTien, ThanhToan, 
    							PTChietKhau, TienChietKhau, PTChiPhi, TienChiPhi, TienThue, An_Hien, TonLuyKe, GhiChu,  ChatLieu)		
    						select 
    						NEWid(),
    						@ID_XuatKho,
    						row_number() over( order by (select 1)) as SoThuTu,
    						ctsc.ID_ChiTietGoiDV,
    						ctsc.ID_DichVu,
    						ctsc.ID_DonViQuiDoi,
    						ctsc.ID_LoHang,
    						ctsc.SoLuong, ctsc.GiaVon, ctsc.GiaVon, ctsc.GiaTri, 
    						0,0,0,0,0,0,'1', ctsc.TonLuyKe, ctsc.GhiChu,''
    					from 
    					(
    					--- ct hoadon banle or hd sudung GDV
    						select 
    							cttp.ID as ID_ChiTietGoiDV,
    							dv.ID_DonViQuiDoi as ID_DichVu,
    							cttp.ID_DonViQuiDoi, 
    							cttp.ID_LoHang,
    							cttp.SoLuong,
    							cttp.GiaVon,
    							cttp.GiaVon* cttp.SoLuong as GiaTri,			
    							cttp.TonLuyKe,
    							isnull(cttp.GhiChu,'') as GhiChu
    						from @ctHDNew cttp		
    						join @ctHDNew dv on cttp.ID_ChiTietDinhLuong = dv.ID
    						where cttp.ID_ChiTietDinhLuong= @ID_ChiTietDinhLuong
    						and cttp.SoLuong > 0		
    						and cttp.LaHangHoa='1'
    						) ctsc
    
    					delete from @tblMa
    					--BEGIN TRY  
    					--	exec dbo.UpdateTonLuyKeCTHD_whenUpdate @ID_XuatKho, @ID_DonVi, @ngayXuatKho
    					--	exec dbo.    @ID_XuatKho, @ID_DonVi, @ngayXuatKho
    					--end try
    					--begin catch
    					--end catch
    					FETCH NEXT FROM _cur
    					INTO @ID_ChiTietDinhLuong, @TongGiaTriXuat						
    					end
    					CLOSE _cur;
    					DEALLOCATE _cur;		
    				END
    			
    		end
    		end
    		else
    		begin
    			--- dichvu cungloai (same dinhluong, same soluong (todo)
    			select ct.ID,
    				ROW_NUMBER() over (partition by ct.ID order by ct.ID) as RN
    			from @tblSame ct
    			
    			---- update id_ctgoidv, id_ctdinhluong for ctxuatkho old
    			update ctXuat set 
    				ctXuat.ID_ChiTietDinhLuong = ctSame.ID_DichVu,
    				ctXuat.ID_ChiTietGoiDV = ctSame.ID_CTMuaNew
    			from BH_HoaDon_ChiTiet ctXuat
    			join @tblSame ctSame on ctXuat.ID = ctSame.ID
    		end");

			CreateStoredProcedure(name: "[dbo].[GetInfor_HDHoTro]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid()
			}, body: @"SET NOCOUNT ON;

	select hd.*,
		ct.ID as ID_ChiTietHD,	
		ct.ID_DonViQuiDoi,
		ct.ID_LoHang,
		ct.SoLuong,
		ct.TienChietKhau,
		ct.TienChietKhau,
		ct.ID_ChiTietDinhLuong,
		ct.ID_ChiTietGoiDV,
		isnull(ct.ChatLieu,'') as ChatLieu,
		qd.MaHangHoa,
		qd.TenDonViTinh,
		hh.TenHangHoa,
		iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa ='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa
	from
	(
	select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.NguoiTao,
		hd.ID_DonVi,
		hd.ID_DoiTuong,
		hd.DienGiai,
		hd.TongGiamGia as SoNgayThuoc,
		hd.ID_CheckIn as Id_NhomHang,
		hd.An_Hien as IsChuyenPhatNhanh,
		ISNULL(nhomhh.TenNhomHangHoa,'') as TenNhomHangHoa,
		dt.MaDoiTuong,
		dt.TenDoiTuong
	from BH_HoaDon hd
	join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
	left join DM_NhomHangHoa nhomhh on hd.ID_CheckIn= nhomhh.ID
	where hd.ID= @ID_HoaDon
	) hd
	join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
	join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
   	left join DM_LoHang lo on ct.ID_LoHang= lo.ID
	where (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong= ct.ID)");

			CreateStoredProcedure(name: "[dbo].[GetListGiaVonTieuChuan_ChiTiet]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSearch = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				TrangThais = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblChiNhanh table(ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@IDChiNhanhs)

	declare @tblTrangThai table(TrangThai int)
	insert into @tblTrangThai
	select name from dbo.splitstring(@TrangThais)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString)

	;with data_cte
	as (
	select *
	from
	(
	 select 
		hd.ID,
		ct.ID as ID_ChiTiet,		
		hd.MaHoaDon, 
		hd.NgayLapHoaDon,
		isnull(nhom.TenNhomHangHoa,'') as TenNhomHangHoa,
		qd.MaHangHoa,
		hh.TenHangHoa,
		qd.GiaBan,
		qd.TenDonViTinh,
		qd.ThuocTinhGiaTri,
		hh.QuanLyTheoLoHang,
		lo.MaLoHang,
		ct.DonGia,
		ct.ThanhTien,
		ct.GiaVon,
		ct.ID_DonViQuiDoi,
		ct.ID_LoHang,
		qd.ID_HangHoa,
		lo.NgaySanXuat,
		lo.NgayHetHan,
		case hd.ChoThanhToan
			when '0' then 0
			when '1' then 1			
		else 2 end as TrangThaiHD
	from BH_HoaDon_ChiTiet ct 
	join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
	join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	left join DM_LoHang lo on ct.ID_LoHang= lo.ID or  (hh.QuanLyTheoLoHang= 0 and lo.ID is null)
	left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
	where hd.LoaiHoaDon= 16
	and hd.ID_DonVi in (select ID_DonVi from @tblChiNhanh)
	and hd.NgayLapHoaDon between @DateFrom and @DateTo
	AND
		((select count(Name) from @tblSearchString b where 
				hd.MaHoaDon like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa like '%'+b.Name+'%'
    			or lo.MaLoHang like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    			or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'    				
    			or ct.GhiChu like '%'+b.Name+'%'					
    			or qd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
	) tbl
	where tbl.TrangThaiHD in (select TrangThai from @tblTrangThai)
	),
	count_cte as (
		select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
		from data_cte
	)
	select dt.*, cte.*
	from data_cte dt
	cross join count_cte cte
	order by dt.NgayLapHoaDon desc
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY; ");

			CreateStoredProcedure(name: "[dbo].[GetListGiaVonTieuChuan_TongHop]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSearch = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				TrangThais = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblChiNhanh table(ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@IDChiNhanhs)

	declare @tblTrangThai table(TrangThai int)
	insert into @tblTrangThai
	select name from dbo.splitstring(@TrangThais)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString)

	;with data_cte
	as (
	select *,
		case tbl.TrangThaiHD
			when 1 then N'Phiếu tạm'
			when 0 then N'Đã điều chỉnh'
			when 2 then N'Đã hủy'
		else '' end as TrangThai
	from
	(
	 select 
		hd.ID,	
		hd.ID_DonVi,
		hd.ID_NhanVien,
		hd.MaHoaDon, 
		hd.NgayLapHoaDon,
		hd.NguoiTao,		
		dv.TenDonVi,
		hd.DienGiai,
		hd.ChoThanhToan,
		case hd.ChoThanhToan
			when '0' then 0
			when '1' then 1			
		else 2 end as TrangThaiHD
	from BH_HoaDon hd 
	join DM_DonVi dv on hd.ID_DonVi= dv.ID	
	where hd.LoaiHoaDon= 16
	and hd.ID_DonVi in (select ID_DonVi from @tblChiNhanh)
	and hd.NgayLapHoaDon between @DateFrom and @DateTo
	AND
		((select count(Name) from @tblSearchString b where 
				hd.MaHoaDon like '%'+b.Name+'%' 
    			or dv.MaDonVi like '%'+b.Name+'%' 
    			or hd.DienGiai like '%'+b.Name+'%' 
    			or hd.NguoiTao like '%'+b.Name+'%'
    		)=@count or @count=0)
	) tbl
	where tbl.TrangThaiHD in (select TrangThai from @tblTrangThai)
	),
	count_cte as (
		select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
		from data_cte
	),
	cthd_cte as
	(
		select hd.ID, count(ct.ID) as TongSLMatHang 
		from data_cte hd
		join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
		group by hd.ID
	)
	select dt.*, cte.*, ct.TongSLMatHang
	from data_cte dt
	left join cthd_cte ct on dt.ID = ct.ID
	cross join count_cte cte
	order by dt.NgayLapHoaDon desc
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY; ");

			CreateStoredProcedure(name: "[dbo].[GetListNhomHang_SetupHoTro]", parametersAction: p => new
			{
				IDDonVis = p.String(defaultValue: null)
			}, body: @"SET NOCOUNT ON;
  
	  if ISNULL(@IDDonVis,'')!=''
	  begin
		declare @tblDonVi table(ID_DonVi uniqueidentifier)
		insert into @tblDonVi
		select name from dbo.splitstring(@IDDonVis)
	
		select nhom.TenNhomHangHoa, ap.Id_NhomHang, ap.GiaTriSuDungTu, ap.GiaTriSuDungDen, ap.GiaTriHoTro, ap.KieuHoTro, ap.STT
		from NhomHangHoa_DonVi nhomdv
		join NhomHang_KhoangApDung ap on nhomdv.ID_NhomHangHoa = ap.Id_NhomHang
		join DM_NhomHangHoa nhom on ap.Id_NhomHang= nhom.ID		
	    where nhom.TrangThai = 0
		and exists (select dv.ID_DonVi from @tblDonVi dv where nhomdv.ID_DonVi= dv.ID_DonVi)
	  end
	  else
	  begin
		select  nhom.TenNhomHangHoa, ap.Id_NhomHang, ap.GiaTriSuDungTu, ap.GiaTriSuDungDen, ap.GiaTriHoTro, ap.KieuHoTro, ap.STT
		from NhomHangHoa_DonVi nhomdv
		join NhomHang_KhoangApDung ap on nhomdv.ID_NhomHangHoa = ap.Id_NhomHang
		join DM_NhomHangHoa nhom on ap.Id_NhomHang= nhom.ID		
	    where nhom.TrangThai = 0		
	  end");

			CreateStoredProcedure(name: "[dbo].[GetTongGiaTriSuDung_ofKhachHang]", parametersAction: p => new
			{
				IDDonVis = p.String(defaultValue: null),
				ID_KhachHang = p.String(defaultValue: null),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;
	declare @tblNhom table(TenNhomHangHoa nvarchar(max), ID_NhomHangHoa uniqueidentifier, GiaTriSuDungTu float, GiaTriSuDungDen float,
	GiaTriHoTro float, KieuHoTro int, STT int)
	insert into @tblNhom
	exec dbo.GetListNhomHang_SetupHoTro @IDDonVis

	if ISNULL(@IDDonVis,'')!=''
	begin
		declare @tblDonVi table(ID_DonVi uniqueidentifier)
		insert into @tblDonVi
		select name from dbo.splitstring(@IDDonVis)		

		--- get cthd thuoc nhom apdung hotros	
		select hh.ID_NhomHang,  sum( ct.SoLuong * (ct.DonGia-ct.TienChietKhau)) as TongGiaTriSuDung
		from BH_HoaDon hd
		join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
		join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		where hd.ChoThanhToan='0'
		and hd.LoaiHoaDon in (1,2)
		and exists (select dv.ID_DonVi from @tblDonVi dv where hd.ID_DonVi= dv.ID_DonVi)
		and hd.ID_DoiTuong= @ID_KhachHang
		and hd.NgayLapHoaDon < @ToDate
		and exists (select ID_NhomHangHoa from @tblNhom nhom where nhom.ID_NhomHangHoa= hh.ID_NhomHang)
		group by hh.ID_NhomHang
		
	end
	else
	begin

		--- get cthd thuoc nhom apdung hotros			
		select hh.ID_NhomHang, sum(ct.SoLuong * (ct.DonGia-ct.TienChietKhau)) as TongGiaTriSuDung
		from BH_HoaDon hd
		join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
		join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID		
		where hd.ChoThanhToan='0'
		and hd.LoaiHoaDon in (1,2)	
		and hd.ID_DoiTuong= @ID_KhachHang
		and hd.NgayLapHoaDon < @ToDate
		and exists (select ID_NhomHangHoa from @tblNhom nhom where nhom.ID_NhomHangHoa= hh.ID_NhomHang)
		group by hh.ID_NhomHang
		
	end");

			CreateStoredProcedure(name: "[dbo].[HuyPhieuXuatKho_WhenUpdateTPDL]", parametersAction: p => new
			{
				ID_CTHoaDon = p.Guid()
			}, body: @"SET NOCOUNT ON;

   ----- get infor phieuxuatkho ---> used to huyphieu
	declare @ID_HDHuy uniqueidentifier, @ID_DonViHuy uniqueidentifier, @NgayLapHoaDonHuy datetime
	select top 1 @ID_HDHuy = ctxk.ID_HoaDon, @ID_DonViHuy = ID_DonVi, @NgayLapHoaDonHuy = NgayLapHoaDon
	from
	(
		---- get tpdluong of ctOld
		select ctm.ID
		from BH_HoaDon_ChiTiet ctm
		where ctm.ID_ChiTietDinhLuong= @ID_CTHoaDon and ctm.ID!= ctm.ID_ChiTietDinhLuong ---- khong lay dichvu
	) tpdl
	join BH_HoaDon_ChiTiet ctxk on tpdl.ID = ctxk.ID_ChiTietGoiDV ---- get ctXuatKho old
	join BH_HoaDon hdx on ctxk.ID_HoaDon= hdx.ID
	where  hdx.LoaiHoaDon= 35

	
	----- huy + chay tonkho
	update BH_HoaDon set ChoThanhToan= null where ID = @ID_HDHuy

	exec UpdateTonLuyKeCTHD_whenUpdate @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy");

			CreateStoredProcedure(name: "[dbo].[NhomHang_GetListSanPhamHoTro]", parametersAction: p => new
			{
				ID_NhomHang = p.Guid()
			}, body: @"SET NOCOUNT ON;
	
	select 
		nhomsp.Id_DonViQuiDoi,
		nhomsp.Id_LoHang,
		nhomsp.Id_NhomHang,
		nhomsp.SoLuong,
		nhomsp.LaSanPhamNgayThuoc,
		hh.TenHangHoa,
		qd.MaHangHoa,
		qd.TenDonViTinh,
		lo.MaLoHang,
		iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa
	from (
		select *
		from NhomHang_ChiTietSanPhamHoTro 
		where Id_NhomHang= @ID_NhomHang
	) nhomsp	
	join DonViQuiDoi qd on nhomsp.Id_DonViQuiDoi = qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	left join DM_LoHang lo on nhomsp.Id_LoHang = lo.ID");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_ChiTiet_Page]
    @pageNumber [int],
    @pageSize [int],
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
	@LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
	@LoaiChungTu [nvarchar](max),
	@HanBaoHanh [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	set nocount on;
	---- bo sung timkiem NVthuchien
	set @pageNumber = @pageNumber -1;

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER)
	INSERT INTO @tblChiNhanh
	select Name from splitstring(@ID_ChiNhanh);

	DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)	

	declare @tblCTHD table (
		NgayLapHoaDon datetime,
		MaHoaDon nvarchar(max),
		LoaiHoaDon int,
		ID_DonVi uniqueidentifier,
		ID_PhieuTiepNhan uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID_NhanVien uniqueidentifier,
		TongTienHang float,
		TongGiamGia	float,
		KhuyeMai_GiamGia float,
		ChoThanhToan bit,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		ID_ChiTietGoiDV	uniqueidentifier,
		ID_ChiTietDinhLuong uniqueidentifier,
		ID_ParentCombo uniqueidentifier,
		SoLuong float,
		DonGia float,
		GiaVonfloat float,
		TienChietKhau float,
		TienChiPhi float,
		ThanhTien float,
		ThanhToan float,
		GhiChu nvarchar(max),
		ChatLieu nvarchar(max),
		LoaiThoiGianBH int,
		ThoiGianBaoHanh float,
		TenHangHoaThayThe nvarchar(max),
		TienThue float,	
		GiamGiaHD float,
		GiaVon float,
		TienVon float
		)

	insert into @tblCTHD
	exec BCBanHang_GetCTHD @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu

	declare @tblChiPhi table (ID_ParentCombo uniqueidentifier,ID_DonViQuiDoi uniqueidentifier, ChiPhi float, 
		ID_NhanVien uniqueidentifier,ID_DoiTuong uniqueidentifier)
	insert into @tblChiPhi
	exec BCBanHang_GetChiPhi @ID_ChiNhanh, @timeStart, @timeEnd, @LoaiChungTu
			
		select *
		into #tblView
		from
		(
		select 
			hh.ID, hh.TenHangHoa,
			qd.MaHangHoa,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa,
			concat(hh.TenHangHoa, qd.ThuocTinhGiaTri) as TenHangHoaFull,
			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			ISNULL(nhh.TenNhomHangHoa,  N'Nhóm hàng hóa mặc định') as TenNhomHangHoa,
			lo.MaLoHang as TenLoHang,
			qd.TenDonViTinh,
			cast(c.SoLuong as float) as SoLuong,
			cast(c.DonGia as float) as GiaBan,
			cast(c.TienChietKhau as float) as TienChietKhau,
			cast(c.ThanhTien as float) as ThanhTien,
			cast(c.GiamGiaHD as float) as GiamGiaHD,
			cast(c.TienThue as float) as TienThue,
			iif(@XemGiaVon='1',cast(c.GiaVon as float),0) as GiaVon,
			iif(@XemGiaVon='1',cast(c.TienVon as float),0) as TienVon,
			cast(c.ThanhTien - c.GiamGiaHD as float) as DoanhThu,
			iif(@XemGiaVon='1',cast(c.ThanhTien - c.GiamGiaHD - c.TienVon -c.ChiPhi as float),0) as LaiLo,
			c.NgayLapHoaDon, c.MaHoaDon, c.ID_PhieuTiepNhan, c.ID_DoiTuong, c.ID_NhanVien,
			c.ThoiGianBaoHanh, c.HanBaoHanh, c.TrangThai, c.GhiChu,
			dt.MaDoiTuong as MaKhachHang, 
			dt.TenDoiTuong as TenKhachHang, 
			dt.TenNhomDoiTuongs as NhomKhachHang, 
			dt.DienThoai, dt.GioiTinhNam,
			dt.ID_NguoiGioiThieu, dt.ID_NguonKhach,
			c.TenNhanVien,
			c.ChiPhi,
			c.LoaiHoaDon,
			iif(c.TenHangHoaThayThe is null or c.TenHangHoaThayThe='', hh.TenHangHoa, c.TenHangHoaThayThe) as TenHangHoaThayThe			
		from 
		(
		select 
			b.LoaiHoaDon,b.NgayLapHoaDon, b.MaHoaDon, b.ID_PhieuTiepNhan, b.ID_DoiTuong, b.ID_NhanVien, b.TenNhanVien,
			b.ThoiGianBaoHanh, b.HanBaoHanh, b.TrangThai, b.GhiChu,		
			isnull(qd.TyLeChuyenDoi,1) * (case b.LoaiHoaDon
				when 6 then - b.SoLuong
			else b.SoLuong end)  as SoLuong,
			case b.LoaiHoaDon
				when 6 then - b.ThanhTien
			else b.ThanhTien end as ThanhTien,
		
			b.GiaVon,
			b.TienVon,		
			qd.ID_HangHoa,
			b.ID_LoHang,	
			b.GiamGiaHD,
			b.TienThue,					
			b.DonGia,
			b.TienChietKhau,
			b.ChiPhi,
			b.TenHangHoaThayThe
		from (
		select 
			ct.LoaiHoaDon,ct.NgayLapHoaDon, ct.MaHoaDon, ct.ID_PhieuTiepNhan, ct.ID_DoiTuong, ct.ID_NhanVien,	
			nvien.NVThucHien as TenNhanVien,
			ct.TienThue,
			ct.GiamGiaHD,			
			ct.ID,ct.ID_DonViQuiDoi, ct.ID_LoHang,
			ct.TenHangHoaThayThe,
			case ct.LoaiThoiGianBH
				when 1 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + N' ngày'
				when 2 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + ' tháng'
				when 3 then CONVERT(varchar(100), ct.ThoiGianBaoHanh) + ' năm'
			else '' end as ThoiGianBaoHanh,
			case ct.LoaiThoiGianBH
				when 1 then DATEADD(DAY, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
				when 2 then DATEADD(MONTH, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
				when 3 then DATEADD(YEAR, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)
			end as HanBaoHanh,
			Case when ct.LoaiThoiGianBH = 1 and DATEADD(DAY, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH = 2 and DATEADD(MONTH, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH = 3 and DATEADD(YEAR, ct.ThoiGianBaoHanh, ct.NgayLapHoaDon)  < GETDATE() then N'Hết hạn'
			when ct.LoaiThoiGianBH in (1,2,3) Then N'Còn hạn'
			else '' end as TrangThai,
			ct.GhiChu,
			ct.DonGia,
			ct.TienChietKhau,
			ct.SoLuong,
			ct.ThanhTien,
			iif(ct.SoLuong =0, 0, ct.TienVon/ct.SoLuong) as GiaVon,			
			ct.TienVon,
			isnull(cp.ChiPhi,0) as ChiPhi
	from @tblCTHD ct	
	left join @tblChiPhi cp on ct.ID= cp.ID_ParentCombo
	left join
		(
		-- get nvthuchien of hdbl
			select distinct th.ID_ChiTietHoaDon ,
				 (
						select nv.TenNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						join BH_HoaDon_ChiTiet ct on nvth.ID_ChiTietHoaDon = ct.ID
						join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
						where nvth.ID_ChiTietHoaDon = th.ID_ChiTietHoaDon
						and (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
    					and hd.ChoThanhToan = 0 
						and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)						
						For XML PATH ('')
					) NVThucHien
				from BH_NhanVienThucHien th 
		) nvien on ct.ID = nvien.ID_ChiTietHoaDon
		where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
		and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)	
		)b
		join DonViQuiDoi qd on b.ID_DonViQuiDoi= qd.ID		
		) c
		join DM_HangHoa hh on c.ID_HangHoa = hh.ID
		join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan=1
		left join DM_LoHang lo on c.ID_LoHang = lo.ID
		left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
		left join DM_DoiTuong dt on c.ID_DoiTuong = dt.ID		
		where 
		exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhh.ID= allnhh.ID)	
    	and hh.TheoDoi like @TheoDoi
		and qd.Xoa like @TrangThai
		and c.TrangThai like @HanBaoHanh		
		AND
		((select count(Name) from @tblSearchString b where 
				c.MaHoaDon like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lo.MaLoHang like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    				or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    				or qd.TenDonViTinh like '%'+b.Name+'%'
					or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau  like '%'+b.Name+'%'
					or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.DienThoai  like '%'+b.Name+'%'
					or c.GhiChu like N'%'+b.Name+'%'
					or c.TenNhanVien like N'%'+b.Name+'%'
					or dbo.FUNC_ConvertStringToUnsign(c.GhiChu) like N'%'+b.Name+'%'
    				or qd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
		)a where a.LoaiHangHoa in (select name from dbo.splitstring(@LoaiHangHoa))	
		
		
	
			DECLARE @Rows FLOAT,  @TongSoLuong float, @TongThanhTien float, @TongGiamGiaHD FLOAT, @TongTienVon FLOAT, 
			@TongLaiLo FLOAT, @SumTienThue FLOAT,@TongDoanhThuThuan FLOAT, @TongChiPhi float			
			SELECT @Rows = Count(*), @TongSoLuong = SUM(SoLuong),
			@TongThanhTien = SUM(ThanhTien), @TongGiamGiaHD = SUM(GiamGiaHD),
			@TongTienVon = SUM(TienVon), @TongLaiLo = SUM(LaiLo), @SumTienThue = SUM(TienThue),
			@TongDoanhThuThuan = SUM(DoanhThu),
			@TongChiPhi = SUM(ChiPhi) 
			FROM #tblView;

			select 
				tbl.*,
				ISNULL(nk.TenNguonKhach,'') as TenNguonKhach,
				isnull(gt.TenDoiTuong,'') as NguoiGioiThieu				
			from(
			select *,							
				@Rows as Rowns,
    			@TongSoLuong as TongSoLuong,
    			@TongThanhTien as TongThanhTien,
    			@TongGiamGiaHD as TongGiamGiaHD,
    			@TongTienVon as TongTienVon,
    			@TongLaiLo as TongLaiLo,
				@SumTienThue as TongTienThue,
    			@TongDoanhThuThuan as DoanhThuThuan,
    			@TongChiPhi as TongChiPhi
    		from #tblView tbl
			order by NgayLapHoaDon DESC
			OFFSET (@pageNumber* @pageSize) ROWS
    		FETCH NEXT @pageSize ROWS ONLY	) tbl
			left join DM_NguonKhachHang nk on tbl.ID_NguonKhach= nk.ID
			left join DM_DoiTuong gt on tbl.ID_NguoiGioiThieu= gt.ID 	   
			order by NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKhachHang_TanSuat]
	@IDChiNhanhs nvarchar(max),	
	@IDNhomKhachs nvarchar(max),
	@LoaiChungTus varchar(20),
	@TrangThaiKhach varchar(10),
	@FromDate datetime,
	@ToDate datetime,
	@NgayGiaoDichFrom datetime,
	@NgayGiaoDichTo datetime,
	@NgayTaoKHFrom datetime,
	@NgayTaoKHTo datetime,
	@DoanhThuTu float,	
	@DoanhThuDen float,
	@SoLanDenFrom int,
	@SoLanDenTo int,
	@TextSearch nvarchar(max),
	@CurrentPage int,
	@PageSize int,
	@ColumnSort varchar(200),
	@TypeSort varchar(20)
AS
BEGIN
	
	SET NOCOUNT ON;
	if @ColumnSort ='' or @ColumnSort is null set @ColumnSort='MaKhachHang'
	if @TypeSort ='' or @TypeSort is null set @TypeSort='desc'
	SET @TypeSort = UPPER(@TypeSort)
	 

	 DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tblChiNhanh table (ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select * from dbo.splitstring(@IDChiNhanhs)
	
	declare @tblNhomDT table (ID varchar(40))
	insert into @tblNhomDT
	select Name from dbo.splitstring(@IDNhomKhachs);

	
	if @DoanhThuTu is null
		set @DoanhThuTu = -10000000

	if @DoanhThuDen is null
		set @DoanhThuDen = 9999999999
	if @SoLanDenTo is null
		set @SoLanDenTo = 9999999;

	with data_cte
	as(
	select dt.ID as ID_KhachHang, dt.MaDoiTuong as MaKhachHang, dt.TenDoiTuong as TenKhachHang, dt.DienThoai, dt.DiaChi, dt.TenNhomDoiTuongs,
		dt.NgayGiaoDichGanNhat,
		hd1.SoLanDen,
		GiaTriMua, 
		GiaTriTra,
		DoanhThu
	from DM_DoiTuong dt  
	join (		
			select hd.ID_DoiTuong, 
			count(hd.ID) as SoLanDen, 					
			sum(isnull(hd.GiaTriTra,0)) as GiaTriTra,
			sum(hd.GiaTriMua) as GiaTriMua,
			sum(hd.GiaTriMua - hd.GiaTriTra) as DoanhThu
			from(
				-- doanhthu: khong tinh napthe (chi tinh luc su dung)
				-- vi BC chi tiet theo khachhang: khong lay duoc dichvu/hanghoa khi napthe
				select  hd.ID_DoiTuong, hd.ID,
					iif(hd.LoaiHoaDon= 6, hd.TongTienHang - hd.TongGiamGia,0) as GiaTriTra,
					case hd.LoaiHoaDon
						when 6 then 0
						when 36 then 0
						else hd.TongTienHang - hd.TongGiamGia - isnull(hd.KhuyeMai_GiamGia,0)
					end as GiaTriMua			
				from BH_HoaDon hd
				where hd.ChoThanhToan = 0
				and hd.LoaiHoaDon != 22
				and hd.ID_DoiTuong is not null
				and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon <= @ToDate
				and exists (select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
				and exists (select * from dbo.splitstring(@LoaiChungTus) ct where hd.LoaiHoaDon= ct.Name)
			) hd group by hd.ID_DoiTuong			
	) hd1 on dt.ID = hd1.ID_DoiTuong
    where 
		 exists (select nhom1.Name 
			from dbo.splitstring(iif(dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs ='','00000000-0000-0000-0000-000000000000',dt.IDNhomDoiTuongs)) nhom1 
		 join @tblNhomDT nhom2 on nhom1.Name = nhom2.ID) 
	and
		dt.TheoDoi like @TrangThaiKhach
	and dt.NgayGiaoDichGanNhat >= @NgayGiaoDichFrom and dt.NgayGiaoDichGanNhat < @NgayGiaoDichTo
	and ((dt.NgayTao >= @NgayTaoKHFrom and dt.NgayTao < @NgayTaoKHTo) or ( dt.ID ='00000000-0000-0000-0000-000000000000'))
	and hd1.SoLanDen >= @SoLanDenFrom and hd1.SoLanDen <= @SoLanDenTo
	and hd1.DoanhThu >= @DoanhThuTu and hd1.DoanhThu <= @DoanhThuDen
	and ((select count(Name) from @tblSearchString b where 
    				dt.MaDoiTuong like '%'+b.Name+'%' 
    				or dt.TenDoiTuong like '%'+b.Name+'%' 
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%' +b.Name +'%' 
    				or dt.DienThoai like '%'+b.Name+'%'
					or dt.DiaChi like '%'+b.Name+'%'
					)=@count or @count=0)
	
	),
	count_cte
	as(
	select count(ID_KhachHang) as TotalRow,
		CEILING(COUNT(ID_KhachHang) / CAST(@PageSize as float )) as TotalPage,
		sum(dt.SoLanDen) as TongSoLanDen,
		sum(GiaTriMua) as TongMua,
		sum(GiaTriTra) as TongTra,
		sum(DoanhThu) as TongDoanhThu
	from data_cte dt
	)
	select *
	from data_cte dt
	cross join count_cte cte
	order by	
		-- các cột dữ liệu sắp xếp phải chuyển về cùng 1 kiểu data
		CASE WHEN @TypeSort = 'ASC'  THEN 
			case @ColumnSort		
				when 'SoLanDen' then cast(dt.SoLanDen as float)
				when 'DoanhThu' then cast(dt.DoanhThu as float)
				when 'GiaTriMua' then cast(dt.GiaTriMua as float)
				when 'GiaTriTra' then cast(dt.GiaTriTra as float)
				when 'NgayGiaoDichGanNhat' then cast(dt.NgayGiaoDichGanNhat as float)
				end
			 END ASC,
		CASE WHEN @TypeSort = 'DESC'  THEN 
			case @ColumnSort
				when 'SoLanDen' then cast(dt.SoLanDen as float)
				when 'DoanhThu' then cast(dt.DoanhThu as float)
				when 'GiaTriMua' then cast(dt.GiaTriMua as float)
				when 'GiaTriTra' then cast(dt.GiaTriTra as float)
				when 'NgayGiaoDichGanNhat' then cast(dt.NgayGiaoDichGanNhat as float)
			end
		END DESC
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_ChiTietHangNhap]
    @ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
	@timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@LoaiChungTu [nvarchar](max)
AS
BEGIN

    SET NOCOUNT ON;
	
	DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblIdDonVi
	SELECT donviinput.Name FROM [dbo].[splitstring](@ID_DonVi) donviinput

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblHoaDon TABLE(MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, DienGiai NVARCHAR(max), TenNhomHang NVARCHAR(MAX), 
	MaHangHoa NVARCHAR(MAX), TenHangHoaFull NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), ThuocTinh_GiaTri NVARCHAR(MAX),
	TenDonViTinh NVARCHAR(MAX), TenLoHang NVARCHAR(MAX),
	ID_DonVi UNIQUEIDENTIFIER,
	LoaiHoaDon INT, TyLeChuyenDoi FLOAT,
	SoLuong FLOAT, GiaNHap FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, GiaVon FLOAT, YeuCau NVARCHAR(MAX), GiamGiaHDPT FLOAT, GhiChu nvarchar(max),
	NguoiTao nvarchar(max),MaDoiTuong nvarchar(max), TenDoiTuong nvarchar(max));

	INSERT INTO @tblHoaDon
	select 
		MaHoaDon,
		NgayLapHoaDon,
		DienGiai,
		TenNhomHang,
		MaHangHoa,
		TenHangHoaFull,
		TenHangHoa,
		ThuocTinh_GiaTri, 
		TenDonViTinh,
		TenLoHang,
		ID_DonVi,
		LoaiHoaDon,
		TyLeChuyenDoi,
		SoLuong,
		GiaNhap,
		TienChietKhau,
		ThanhTien,
		GiaVon,
		YeuCau,
		GianGiaHD,
		GhiChu,
		tbl.NguoiTao,
		tbl.MaDoiTuong, 
		tbl.TenDoiTuong
	from
	(
	SELECT bhd.MaHoaDon, IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) AS NgayLapHoaDon,
		bhd.DienGiai,
		nhh.TenNhomHangHoa AS TenNhomHang, 
		nhh.TenNhomHangHoa_KhongDau AS TenNhomHangHoa_KhongDau, 
		dvqdChuan.MaHangHoa, 
		concat(hh.TenHangHoa , dvqd.ThuocTinhGiaTri) AS TenHangHoaFull,
		hh.TenHangHoa, 
		ISNULL(dvqd.ThuocTinhGiaTri, '') AS ThuocTinh_GiaTri, 
		dvqdChuan.TenDonViTinh,
		lh.MaLoHang AS TenLoHang, 
		iif(bhd.LoaiHoaDon=10, bhd.ID_CheckIn,bhd.ID_DonVi) as ID_DonVi,	
		bhd.LoaiHoaDon, 
		dvqd.TyLeChuyenDoi, 
		bhdct.SoLuong,
		case bhd.LoaiHoaDon
			when 4 then bhdct.DonGia - bhdct.TienChietKhau
			when 13 then  bhdct.DonGia - bhdct.TienChietKhau
			when 14 then  bhdct.DonGia - bhdct.TienChietKhau
			when 10 then case when YeuCau='4' then bhdct.GiaVon_NhanChuyenHang else bhdct.GiaVon end
		else bhdct.GiaVon end as GiaNhap,
		bhdct.TienChietKhau,
		bhdct.ThanhTien, 
		bhd.NguoiTao,
		case bhd.LoaiHoaDon
			when 4 then dt.MaDoiTuong
			when 14 then dt.MaDoiTuong
			when 6 then dt.MaDoiTuong
			when 13 then iif(dt.ID='00000000-0000-0000-0000-000000000002' or dt.ID is null, nv.MaNhanVien, dt.MaDoiTuong)
		else nv.MaNhanVien end as MaDoiTuong,
		case bhd.LoaiHoaDon
			when 4 then dt.TenDoiTuong
			when 14 then dt.TenDoiTuong
			when 6 then dt.TenDoiTuong
			when 13 then iif(dt.ID='00000000-0000-0000-0000-000000000002' or dt.ID is null, nv.TenNhanVien, dt.TenDoiTuong)
		else nv.TenNhanVien end as TenDoiTuong,
		case bhd.LoaiHoaDon
			when 4 then dt.TenDoiTuong_KhongDau
			when 14 then dt.TenDoiTuong_KhongDau
			when 6 then dt.TenDoiTuong_KhongDau
			when 13 then iif(dt.ID='00000000-0000-0000-0000-000000000002' or dt.ID is null, nv.TenNhanVienKhongDau, dt.TenDoiTuong_KhongDau)
		else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
		case bhd.LoaiHoaDon
			when 6 then case when ctm.GiaVon is null or bhdct.ID_ChiTietDinhLuong is null then bhdct.GiaVon else ctm.GiaVon end
			when 10 then case when bhd.ID_CheckIn= dv.ID then bhdct.GiaVon_NhanChuyenHang else bhdct.GiaVon end
		else bhdct.GiaVon end as GiaVon,			
		bhd.YeuCau,
		IIF(bhd.TongTienHang = 0, 0, bhd.TongGiamGia / bhd.TongTienHang) as GianGiaHD,
		bhdct.GhiChu,
		hh.TenHangHoa_KhongDau,
		hh.TenHangHoa_KyTuDau,
		iif(@SearchString='',bhdct.GhiChu, dbo.FUNC_ConvertStringToUnsign(bhdct.GhiChu)) as GhiChuUnsign,
		iif(@SearchString='',bhd.DienGiai, dbo.FUNC_ConvertStringToUnsign(bhd.DienGiai)) as DienGiaiUnsign
    FROM BH_HoaDon_ChiTiet bhdct
	left join BH_HoaDon_ChiTiet ctm on bhdct.ID_ChiTietGoiDV = ctm.ID
    INNER JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
	left join DM_DoiTuong dt on bhd.ID_DoiTuong= dt.ID
	left join NS_NhanVien nv on bhd.ID_NhanVien= nv.ID
	join @tblIdDonVi dv on (bhd.ID_DonVi = dv.ID and bhd.LoaiHoaDon!=10) or (bhd.ID_CheckIn = dv.ID and bhd.YeuCau='4')
    INNER JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
	INNER JOIN DonViQuiDoi dvqdChuan ON dvqdChuan.ID_HangHoa = dvqd.ID_HangHoa AND dvqdChuan.LaDonViChuan = 1
	INNER JOIN (select Name from splitstring(@LoaiChungTu)) lhd ON bhd.LoaiHoaDon = lhd.Name
	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
	INNER JOIN DM_NhomHangHoa nhh  ON nhh.ID = hh.ID_NhomHang   
    INNER JOIN (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh  ON nhh.ID = allnhh.ID   
	LEFT JOIN DM_LoHang lh   ON lh.ID = bhdct.ID_LoHang OR (bhdct.ID_LoHang IS NULL AND lh.ID IS NULL)
    WHERE bhd.ChoThanhToan = 0
	and (bhdct.ChatLieu is null or bhdct.ChatLieu !='2')
    AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) >= @timeStart
	AND IIF(bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4', bhd.NgaySua, bhd.NgayLapHoaDon) < @timeEnd
	AND hh.LaHangHoa = 1 AND hh.TheoDoi LIKE @TheoDoi AND dvqd.Xoa LIKE @TrangThai 
	) tbl 
	where ((select count(Name) from @tblSearchString b where 
    		tbl.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    			or tbl.NguoiTao like '%'+b.Name+'%'
				or tbl.MaDoiTuong like '%'+b.Name+'%'
				or tbl.TenDoiTuong like '%'+b.Name+'%'
				or tbl.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    			or tbl.TenHangHoa like '%'+b.Name+'%'
    			or tbl.TenLoHang like '%' +b.Name +'%' 
    			or tbl.MaHangHoa like '%'+b.Name+'%'
				or tbl.MaHangHoa like '%'+b.Name+'%'
    			or tbl.TenNhomHang like '%'+b.Name+'%'
    			or tbl.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or tbl.TenDonViTinh like '%'+b.Name+'%'
    			or tbl.ThuocTinh_GiaTri like '%'+b.Name+'%'
				or tbl.DienGiai like N'%'+b.Name+'%'
				or tbl.MaHoaDon like N'%'+b.Name+'%'
    			or tbl.GhiChu like N'%'+b.Name+'%'
				or tbl.GhiChuUnsign like '%'+b.Name+'%'
				or tbl.DienGiaiUnsign like '%'+b.Name+'%'
				)=@count or @count=0);	

	SELECT ct.TenLoaiChungTu,
		pstk.LoaiHoaDon,
		pstk.MaHoaDon, pstk.NgayLapHoaDon, pstk.TenNhomHang, pstk.MaHangHoa, pstk.TenHangHoaFull,
	pstk.TenHangHoa, pstk.ThuocTinh_GiaTri, pstk.TenDonViTinh, pstk.TenLoHang,DienGiai, pstk.GhiChu,
	dv.TenDonVi, dv.MaDonVi, 
	pstk.SoLuongNhap AS SoLuong,
	IIF(@XemGiaVon = '1',pstk.GiaNhap,0) as GiaNhap,
	IIF(@XemGiaVon = '1',pstk.GiaTriNhap,0) as ThanhTien,
	pstk.NguoiTao,
	iif(LoaiHoaDon = 10, dv.MaDonVi, pstk.MaDoiTuong) as MaDoiTuong, 
	iif(LoaiHoaDon = 10, dv.TenDonVi, pstk.TenDoiTuong) as TenDoiTuong
	FROM 
	(
    SELECT 
    MaHoaDon, NgayLapHoaDon, TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	LoaiHoaDon, DienGiai, GhiChu,
	NguoiTao,
	MaDoiTuong, 
	TenDoiTuong,
	IIF(LoaiHoaDon = 10 and YeuCau = '4', TienChietKhau* TyLeChuyenDoi, SoLuong * TyLeChuyenDoi) AS SoLuongNhap,
	GiaNhap,
	IIF(LoaiHoaDon = 10 and YeuCau = '4' ,TienChietKhau* GiaVon, iif(LoaiHoaDon = 6, SoLuong * GiaVon,   ThanhTien*(1-GiamGiaHDPT))) AS GiaTriNhap
    FROM @tblHoaDon WHERE LoaiHoaDon != 9

	UNION ALL
    SELECT 
    MaHoaDon, NgayLapHoaDon, TenNhomHang, MaHangHoa, TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,
	LoaiHoaDon,DienGiai, GhiChu,
	NguoiTao,
	MaDoiTuong, 
	TenDoiTuong,
	sum(SoLuong * TyLeChuyenDoi) as SoLuongNhap,	
	max(GiaVon) as GiaNhap,
	SUM(SoLuong * TyLeChuyenDoi * GiaVon) AS GiaTriNhap
    FROM @tblHoaDon
    WHERE LoaiHoaDon = 9 and SoLuong > 0 
    GROUP BY LoaiHoaDon, MaHoaDon, NgayLapHoaDon,TenNhomHang, MaHangHoa, 
	TenHangHoaFull, TenHangHoa, ThuocTinh_GiaTri, TenDonViTinh, TenLoHang, ID_DonVi,DienGiai, GhiChu,NguoiTao,MaDoiTuong, 
	TenDoiTuong
	) pstk
	join DM_DonVi dv on pstk.ID_DonVi= dv.ID
	INNER JOIN DM_LoaiChungTu ct on pstk.LoaiHoaDon = ct.ID
	order by pstk.NgayLapHoaDon desc

END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_ChiTietHangXuat]
    @ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
	@timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@LoaiChungTu [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
	 DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER);
    INSERT INTO @tblChiNhanh SELECT Name FROM splitstring(@ID_DonVi)

	declare @tblLoaiHD table(LoaiHD int)
	insert into @tblLoaiHD
	select name from dbo.splitstring(@LoaiChungTu)

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
	
			select 
				dv.MaDonVi, dv.TenDonVi, nv.TenNhanVien,
				tblQD.NgayLapHoaDon, tblQD.MaHoaDon,
				tblQD.BienSo,
				isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
				isnull(lo.MaLoHang,'') as TenLoHang,
				qd.MaHangHoa, qd.TenDonViTinh, 
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				hh.TenHangHoa,
				CONCAT(hh.TenHangHoa,qd.ThuocTinhGiaTri) as TenHangHoaFull,
				tblQD.GhiChu,
				tblQD.DienGiai,
				round(SoLuong,3) as SoLuong,
				iif(@XemGiaVon='1', round(GiaTriXuat,3),0) as ThanhTien,
				case tblQD.LoaiXuatKho				
					when 7 then N'Trả hàng nhà cung cấp'
					when 8 then N'Xuất kho'
					when 9 then N'Phiếu kiểm kê'
					when 10 then N'Chuyển hàng'		
					when 12 then N'Xuất bảo hành'	
					when 35 then N'Xuất nguyên vật liệu'
					when 37 then N'Xuất ngày thuốc'
					when 38 then N'Xuất bán lẻ'
					when 39 then N'Xuất bảo hành'
					when 40 then N'Xuất hỗ trợ chung'
				end as TenLoaiChungTu,
				tblQD.LoaiHoaDon,
				tblQD.NguoiTao,
				case when tblQD.LoaiHoaDon in (7,35,37,38,39,40) then dt.MaDoiTuong else nv.MaNhanVien end as MaDoiTuong,
				case when tblQD.LoaiHoaDon in (7,35,37,38,39,40) then dt.TenDoiTuong else nv.TenNhanVien end as TenDoiTuong
			from
			(
					select 
						qd.ID_HangHoa,
						tblHD.ID_LoHang, 
						tblHD.ID_DonVi,
						tblHD.ID_CheckIn,
						tblHD.NgayLapHoaDon,
						tblHD.MaHoaDon, 
						tblHD.ID_NhanVien,
						tblHD.LoaiHoaDon,
						tblHD.LoaiXuatKho,
						tblHD.BienSo,
						tblHD.DienGiai,
						tblHD.NguoiTao,
						tblHD.ID_DoiTuong,
						iif(@SearchString='',tblHD.DienGiai,dbo.FUNC_ConvertStringToUnsign(tblHD.DienGiai)) as DienGiaiUnsign,
						max(tblHD.GhiChu) as GhiChu,
						iif(@SearchString='',max(tblHD.GhiChu),max(dbo.FUNC_ConvertStringToUnsign(tblHD.GhiChu))) as GhiChuUnsign,
						sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
						sum(tblHD.GiaTriXuat) as GiaTriXuat
					from
					(
						select 
							hd.NgayLapHoaDon, 
							hd.MaHoaDon,
							hd.LoaiHoaDon,
							hd.ID_HoaDon,
							hd.ID_CheckIn,
							hd.ID_NhanVien, 
							hd.DienGiai, 
							xe.BienSo,
							hd.NguoiTao,
							hd.ID_DoiTuong, 
							case hd.LoaiHoaDon
								when 2 then 12 --- xuat baohanh (Vì loai = 2 đã dùng cho xuat sudung gdv)
								when 8 then 8								
								else hd.LoaiHoaDon end as LoaiXuatKho, -- xuat khac: traNCC, chuyenang,..
							ct.ID_ChiTietGoiDV,
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_DonVi,
							case hd.LoaiHoaDon
							when 9 then iif(ct.SoLuong < 0, -ct.SoLuong, 0)
							when 10 then ct.TienChietKhau else ct.SoLuong end as SoLuong,
							ct.TienChietKhau,
							ct.GiaVon,
							ct.GhiChu,
							case hd.LoaiHoaDon
								when 7 then ct.SoLuong * ct.DonGia
								when 10 then ct.TienChietKhau * ct.GiaVon
								when 9 then iif(ct.SoLuong < 0, -ct.SoLuong, 0) * ct.GiaVon
								else ct.SoLuong* ct.GiaVon end as GiaTriXuat
						from BH_HoaDon_ChiTiet ct
						join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
						left join Gara_PhieuTiepNhan tn on hd.ID_PhieuTiepNhan = tn.ID
						left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
						WHERE hd.ChoThanhToan = 0 
						AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
						and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)						
						and exists (select LoaiHD from @tblLoaiHD loai where hd.LoaiHoaDon= loai.LoaiHD)
						and iif(hd.LoaiHoaDon=9,ct.SoLuong, -1) < 0 -- phieukiemke: chi lay neu soluong < 0 (~xuatkho)
					) tblHD
				join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID							
				group by qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_DonVi,
				 tblHD.ID_CheckIn,tblHD.NgayLapHoaDon, tblHD.MaHoaDon, tblHD.ID_NhanVien, tblHD.ID_DoiTuong,
				 tblHD.LoaiHoaDon, tblHD.LoaiXuatKho, tblHD.DienGiai, tblHD.BienSo, tblHD.NguoiTao	
			)tblQD
			join DM_DonVi dv on tblQD.ID_DonVi = dv.ID
			join DM_LoaiChungTu ct on tblQD.LoaiHoaDon = ct.ID
			left join DM_DoiTuong dt on tblQD.ID_DoiTuong= dt.ID
			left join NS_NhanVien nv on tblQD.ID_NhanVien= nv.ID
			join DM_HangHoa hh on tblQD.ID_HangHoa= hh.ID
			join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
			left join DM_LoHang lo on tblQD.ID_LoHang= lo.ID and (lo.ID= tblQD.ID_LoHang or (tblQD.ID_LoHang is null and lo.ID is null))
			where hh.LaHangHoa = 1
			and hh.TheoDoi like @TheoDoi
			and qd.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID= allnhh.ID)
			AND ((select count(Name) from @tblSearchString b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa like '%'+b.Name+'%'
    		or lo.MaLoHang like '%' +b.Name +'%' 
			or qd.MaHangHoa like '%'+b.Name+'%'
			or qd.MaHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    		or qd.TenDonViTinh like '%'+b.Name+'%'
    		or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
			or dv.MaDonVi like '%'+b.Name+'%'
			or dv.TenDonVi like '%'+b.Name+'%'
			or nv.TenNhanVien like '%'+b.Name+'%'
			or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
			or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
			or nv.MaNhanVien like '%'+b.Name+'%'
			or dt.MaDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
			or tblQD.GhiChu like N'%'+b.Name+'%'
			or tblQD.MaHoaDon like N'%'+b.Name+'%'
			or tblQD.BienSo like N'%'+b.Name+'%'
			or tblQD.GhiChuUnsign like '%'+b.Name+'%'
			or tblQD.DienGiai like N'%'+b.Name+'%'
			or tblQD.DienGiaiUnsign like N'%'+b.Name+'%'
			)=@count or @count=0)
			order by tblQD.NgayLapHoaDon desc


END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTon]
	@ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
    @timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
    @CoPhatSinh [int]
AS
BEGIN

	SET NOCOUNT ON;

    DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER, MaDonVi nvarchar(max), TenDonVi nvarchar(max));
    INSERT INTO @tblChiNhanh 
	SELECT dv.ID, dv.MaDonVi, dv.TenDonVi 
	FROM splitstring(@ID_DonVi) cn
	join DM_DonVi  dv on cn.Name= dv.ID

	declare @dtNow datetime = format(getdate(),'yyyy-MM-dd')  

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung);
	

	declare @tkDauKy table (ID_DonVi uniqueidentifier,ID_HangHoa uniqueidentifier,	ID_LoHang uniqueidentifier null, TonKho float,GiaVon float)		
	insert into @tkDauKy
	exec dbo.GetAll_TonKhoDauKy @ID_DonVi, @timeStart

	

			------ tonkho trongky
			select 			
				qd.ID_HangHoa,
				tkNhapXuat.ID_LoHang,
				tkNhapXuat.ID_DonVi,				
				sum(tkNhapXuat.SoLuongNhap * qd.TyLeChuyenDoi) as SoLuongNhap,
				sum(tkNhapXuat.GiaTriNhap ) as GiaTriNhap,
				sum(tkNhapXuat.SoLuongXuat * qd.TyLeChuyenDoi) as SoLuongXuat,
				sum(tkNhapXuat.GiaTriXuat) as GiaTriXuat
				into #temp
			from
			(
			-- xuat ban, trahang ncc, xuatkho, xuat chuyenhang
				select 
					ct.ID_DonViQuiDoi,
					ct.ID_LoHang,
					hd.ID_DonVi,
					0 AS SoLuongNhap,
					0 AS GiaTriNhap,
					sum(
						case hd.LoaiHoaDon
						when 10 then ct.TienChietKhau
						else ct.SoLuong end ) as SoLuongXuat,
					sum( 
						case hd.LoaiHoaDon
						when 7 then ct.SoLuong* ct.DonGia
						when 10 then ct.TienChietKhau * ct.GiaVon
						else ct.SoLuong* ct.GiaVon end )  AS GiaTriXuat
					FROM BH_HoaDon_ChiTiet ct
				LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
				WHERE hd.ChoThanhToan = 0
				and (hd.LoaiHoaDon in (7,8,35,37,38,39,40) 
					or (hd.LoaiHoaDon = 10  and (hd.YeuCau='1' or hd.YeuCau='4')) )
				AND hd.NgayLapHoaDon between  @timeStart AND   @timeEnd
				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
				GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi								


				UNION ALL
				 ---nhap chuyenhang
				SELECT 
					ct.ID_DonViQuiDoi,
					ct.ID_LoHang,
					hd.ID_CheckIn AS ID_DonVi,
					SUM(ct.TienChietKhau) AS SoLuongNhap,
					SUM(ct.TienChietKhau* ct.DonGia) AS GiaTriNhap, -- lay giatri tu chinhanh chuyen
					0 AS SoLuongXuat,
					0 AS GiaTriXuat
				FROM BH_HoaDon_ChiTiet ct
				LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
				WHERE hd.LoaiHoaDon = 10 and hd.YeuCau = '4' AND hd.ChoThanhToan = 0
				and exists (select ID from @tblChiNhanh dv where hd.ID_CheckIn = dv.ID)
				AND hd.NgaySua between  @timeStart AND   @timeEnd
				GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn

    			UNION ALL
				 ---nhaphang + khach trahang
				SELECT 
					ct.ID_DonViQuiDoi,
					ct.ID_LoHang,
					hd.ID_DonVi,
					SUM(ct.SoLuong) AS SoLuongNhap,
					sum(case hd.LoaiHoaDon
						when 6 then iif(ctm.GiaVon is null or ctm.ID = ctm.ID_ChiTietDinhLuong, ct.SoLuong * ct.GiaVon, ct.SoLuong *ctm.GiaVon)
						when 4 then iif( hd.TongTienHang = 0,0, ct.SoLuong* (ct.DonGia - ct.TienChietKhau) * (1- hd.TongGiamGia/hd.TongTienHang))
					else ct.SoLuong * ct.GiaVon end) as GiaTriNhap,
					0 AS SoLuongXuat,
					0 AS GiaTriXuat
				FROM BH_HoaDon_ChiTiet ct
				LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
				left join BH_HoaDon_ChiTiet ctm on ct.ID_ChiTietGoiDV = ctm.ID
				WHERE hd.LoaiHoaDon in (4,6,13,14)
				AND hd.ChoThanhToan = 0
				and (ct.ChatLieu is null or ct.ChatLieu !='2')
				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
				AND hd.NgayLapHoaDon between  @timeStart AND   @timeEnd
				GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi
    
    			UNION ALL
				-- kiemke
    			SELECT 
					ctkk.ID_DonViQuiDoi, 
					ctkk.ID_LoHang, 
					ctkk.ID_DonVi, 
					sum(isnull(SoLuongNhap,0)) as SoLuongNhap,
					sum(isnull(SoLuongNhap,0) * ctkk.GiaVon) as GiaTriNhap,
					sum(isnull(SoLuongXuat,0)) as SoLuongXuat,
					sum(isnull(SoLuongXuat,0) * ctkk.GiaVon) as GiaTriXuat
				FROM
    			(SELECT 
    				ct.ID_DonViQuiDoi,
    				ct.ID_LoHang,
					hd.ID_DonVi,
					IIF(ct.SoLuong< 0, 0, ct.SoLuong) as SoLuongNhap,
					IIF(ct.SoLuong < 0, - ct.SoLuong, 0) as SoLuongXuat,
					ct.GiaVon
    			FROM BH_HoaDon_ChiTiet ct 
    			LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
    			WHERE hd.LoaiHoaDon = '9' 
    			AND hd.ChoThanhToan = 0
				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
    			AND hd.NgayLapHoaDon between  @timeStart AND   @timeEnd  			
				) ctkk	
    			GROUP BY ctkk.ID_DonViQuiDoi, ctkk.ID_LoHang, ctkk.ID_DonVi
			)tkNhapXuat
			join DonViQuiDoi qd on tkNhapXuat.ID_DonViQuiDoi = qd.ID
			group by qd.ID_HangHoa, tkNhapXuat.ID_LoHang, tkNhapXuat.ID_DonVi


			if	@CoPhatSinh= 2
					begin
							select 
							a.TenNhomHang,
							a.TenHangHoa,
							a.MaHangHoa,
							a.TenDonViTinh,
							a.TenLoHang,
							a.TenDonVi,
							a.MaDonVi,
							concat(a.TenHangHoa,a.ThuocTinhGiaTri) as TenHangHoaFull,
							-- dauky
							isnull(a.TonDauKy,0) as TonDauKy,
							iif(@XemGiaVon='1',isnull(a.GiaTriDauKy,0),0) as GiaTriDauKy,

							--- trongky
							isnull(a.SoLuongNhap,0) as SoLuongNhap,
							iif(@XemGiaVon='1',isnull(a.GiaTriNhap,0),0) as GiaTriNhap,
							isnull(a.SoLuongXuat,0) as SoLuongXuat,
							iif(@XemGiaVon='1',isnull(a.GiaTriXuat,0),0) as GiaTriXuat,

							-- cuoiky
							isnull(a.TonDauKy,0) + isnull(a.SoLuongNhap,0) - isnull(a.SoLuongXuat,0) as TonCuoiKy,
							(isnull(a.TonDauKy,0) + isnull(a.SoLuongNhap,0) - isnull(a.SoLuongXuat,0)) * iif(a.QuyCach=0 or a.QuyCach is null,1, a.QuyCach)  as TonQuyCach,
							iif(@XemGiaVon='1',isnull(a.GiaTriDauKy,0) + isnull(a.GiaTriNhap,0) - isnull(a.GiaTriXuat,0),0)  as GiaTriCuoiKy
						from
						(
							select 
								isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
								hh.TenHangHoa,
								hh.QuyCach,
								qd.MaHangHoa,
								qd.TenDonViTinh,
								isnull(lo.MaLoHang,'') as TenLoHang,
								dv.TenDonVi,
								dv.MaDonVi,
								qd.ThuocTinhGiaTri,
				
								-- dauky	

								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.TonKho, 0) as TonDauKy,		
								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.TonKho, 0) * iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.GiaVon, 0) as GiaTriDauKy,
								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.GiaVon, 0) as GiaVon,	
							
									----trongky
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongNhap, 0) as SoLuongNhap,		
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongXuat, 0) as SoLuongXuat,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.GiaTriNhap, 0) as GiaTriNhap,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.GiaTriXuat, 0) as GiaTriXuat
								
							from #temp tkTrongKy
							left join DM_HangHoa hh on tkTrongKy.ID_HangHoa= hh.ID
							left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
							left join DonViQuiDoi qd on tkTrongKy.ID_HangHoa = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
							left join DM_LoHang lo on tkTrongKy.ID_LoHang= lo.ID or lo.ID is null
							cross join @tblChiNhanh  dv									
							left join @tkDauKy tkDauKy on hh.ID = tkDauKy.ID_HangHoa 
							and tkTrongKy.ID_DonVi= tkDauKy.ID_DonVi and ((lo.ID= tkDauKy.ID_LoHang) or (lo.ID is null and hh.QuanLyTheoLoHang = 0 ))							
							where hh.LaHangHoa= 1
								AND hh.TheoDoi LIKE @TheoDoi	
								and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID = allnhh.ID)	
								and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
									AND ((select count(Name) from @tblSearchString b where 
    								hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    								or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    									or hh.TenHangHoa like '%'+b.Name+'%'
    									or lo.MaLoHang like '%' +b.Name +'%' 
    									or qd.MaHangHoa like '%'+b.Name+'%'
    									or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    									or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    									or nhom.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    									or qd.TenDonViTinh like '%'+b.Name+'%'
    									or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
										or dv.MaDonVi like '%'+b.Name+'%'
										or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)		
							) a	order by TenHangHoa, TenDonVi,TenLoHang		
					end
			else
			begin
			
					select 	
							dv.MaDonVi, dv.TenDonVi,
							qd.MaHangHoa,
							hh.TenHangHoa,
							hh.QuyCach,
							lo.ID as ID_LoHang,
							qd.ID as ID_DonViQuyDoi,
							concat(hh.TenHangHoa,ThuocTinhGiaTri) as TenHangHoaFull,
							isnull(ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
							isnull(qd.TenDonViTinh,'') as TenDonViTinh,
							isnull(lo.MaLoHang,'') as TenLoHang,

							---- dauky
							isnull(tkDauKy.TonKho,0) as TonDauKy,
							isnull(tkDauKy.GiaVon,0) as GiaVon,				
							iif(@XemGiaVon='1',isnull(tkDauKy.TonKho,0) * isnull(tkDauKy.GiaVon,0),0)  as GiaTriDauKy,			
							isnull(nhom.TenNhomHangHoa,N'Nhóm Hàng Hóa Mặc Định') TenNhomHang,

							---- trongky
							isnull(tkTrongKy.SoLuongNhap,0) as SoLuongNhap,
							isnull(tkTrongKy.SoLuongXuat,0) as SoLuongXuat,
							iif(@XemGiaVon='1',isnull(tkTrongKy.GiaTriNhap,0),0) as GiaTriNhap,
							iif(@XemGiaVon='1',isnull(tkTrongKy.GiaTriXuat,0),0) as GiaTriXuat,

							---- cuoiky
							isnull(tkDauKy.TonKho,0) + isnull(tkTrongKy.SoLuongNhap,0) - isnull(tkTrongKy.SoLuongXuat,0) as TonCuoiKy,
							(isnull(tkDauKy.TonKho,0) + isnull(tkTrongKy.SoLuongNhap,0) - isnull(tkTrongKy.SoLuongXuat,0))  * iif(hh.QuyCach=0 or hh.QuyCach is null,1, hh.QuyCach) as TonQuyCach,
							iif(@XemGiaVon='1',isnull(tkDauKy.TonKho,0) * isnull(tkDauKy.GiaVon,0) + isnull(tkTrongKy.GiaTriNhap,0)
							- isnull(tkTrongKy.GiaTriXuat,0),0) as GiaTriCuoiKy
					from DM_HangHoa hh 		
					join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
					left join DM_LoHang lo on hh.ID = lo.ID_HangHoa
					left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
					cross join @tblChiNhanh  dv		
					left join @tkDauKy tkDauKy 
					on qd.ID_HangHoa = tkDauKy.ID_HangHoa and tkDauKy.ID_DonVi= dv.ID and ((lo.ID= tkDauKy.ID_LoHang) or (lo.ID is null ))
					left join #temp tkTrongKy on qd.ID_HangHoa = tkTrongKy.ID_HangHoa and tkTrongKy.ID_DonVi= dv.ID and ((lo.ID= tkTrongKy.ID_LoHang) or (lo.ID is null ))
					where hh.LaHangHoa= 1
					AND hh.TheoDoi LIKE @TheoDoi	
					and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID = allnhh.ID)	
					and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
						AND ((select count(Name) from @tblSearchString b where 
    					hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    					or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    						or hh.TenHangHoa like '%'+b.Name+'%'
    						or lo.MaLoHang like '%' +b.Name +'%' 
    						or qd.MaHangHoa like '%'+b.Name+'%'
    						or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    						or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    						or nhom.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    						or qd.TenDonViTinh like '%'+b.Name+'%'
    						or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
							or dv.MaDonVi like '%'+b.Name+'%'
							or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)		
						order by TenHangHoa, TenDonVi,MaLoHang
			end

			
			
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapXuatTonChiTiet]
    @ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
    @timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
    @CoPhatSinh [int]
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));
	INSERT INTO @tblIdDonVi
	SELECT donviinput.Name, dv.MaDonVi, dv.TenDonVi FROM [dbo].[splitstring](@ID_DonVi) donviinput
	INNER JOIN DM_DonVi dv
	ON dv.ID = donviinput.Name;

	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER);
    INSERT INTO @tblChiNhanh SELECT Name FROM splitstring(@ID_DonVi)

    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tkDauKy table (ID_DonVi uniqueidentifier,ID_HangHoa uniqueidentifier,	ID_LoHang uniqueidentifier null, TonKho float,GiaVon float)		
	insert into @tkDauKy
	exec dbo.GetAll_TonKhoDauKy @ID_DonVi, @timeStart

	---- phatsinh trongky
	select 
		qd.ID_HangHoa,	
		tblQD.ID_LoHang,
		tblQD.ID_DonVi,
		sum(SoLuongNhap_NCC * qd.TyLeChuyenDoi) as SoLuongNhap_NCC,
		sum(SoLuongNhap_Kiem * qd.TyLeChuyenDoi) as SoLuongNhap_Kiem,
		sum(SoLuongNhap_Tra * qd.TyLeChuyenDoi) as SoLuongNhap_Tra,
		sum(SoLuongNhap_Chuyen * qd.TyLeChuyenDoi) as SoLuongNhap_Chuyen,

		sum(SoLuongXuat_Ban * qd.TyLeChuyenDoi) as SoLuongXuat_Ban,
		sum(SoLuongXuat_Huy * qd.TyLeChuyenDoi) as SoLuongXuat_Huy,
		sum(SoLuongXuat_NCC * qd.TyLeChuyenDoi) as SoLuongXuat_NCC,
		sum(SoLuongXuat_Kiem * qd.TyLeChuyenDoi) as SoLuongXuat_Kiem,
		sum(SoLuongXuat_Chuyen * qd.TyLeChuyenDoi) as SoLuongXuat_Chuyen,		
		sum(GiaTri) as GiaTri
		into #temp
	from
	(
		select 
			tblHD.ID_DonViQuiDoi,
			tblHD.ID_LoHang,
			tblHD.ID_DonVi,
			SUM(tblHD.SoLuongNhap_NCC) AS SoLuongNhap_NCC,
			SUM(tblHD.SoLuongNhap_Kiem) AS SoLuongNhap_Kiem,
			SUM(tblHD.SoLuongNhap_Tra) AS SoLuongNhap_Tra,
			SUM(tblHD.SoLuongNhap_Chuyen) AS SoLuongNhap_Chuyen,
		
			SUM(tblHD.SoLuongXuat_Ban) AS SoLuongXuat_Ban,
			SUM(tblHD.SoLuongXuat_Huy) AS SoLuongXuat_Huy,
			SUM(tblHD.SoLuongXuat_NCC) AS SoLuongXuat_NCC,
			SUM(tblHD.SoLuongXuat_Kiem) AS SoLuongXuat_Kiem,
			SUM(tblHD.SoLuongXuat_Chuyen) AS SoLuongXuat_Chuyen,		
			SUM(tblHD.GiaTri) AS GiaTri
		from
		(
				-----  banhang, xuatkho, trancc, kiem -, chuyenhang
				-- xuatban
					SELECT 
						ct.ID_DonViQuiDoi,
						ct.ID_LoHang,
						hd.ID_DonVi,
						0 AS SoLuongNhap_NCC,
						0 AS SoLuongNhap_Kiem,
						0 AS SoLuongNhap_Tra,
						0 AS SoLuongNhap_Chuyen,
						
						SUM(ct.SoLuong) AS SoLuongXuat_Ban,
						0 AS SoLuongXuat_Huy,
						0 AS SoLuongXuat_NCC,
						0 AS SoLuongXuat_Kiem,
						0 AS SoLuongXuat_Chuyen,					
						SUM(ct.SoLuong * ct.GiaVon) * (-1) AS GiaTri
					FROM BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
					WHERE hd.LoaiHoaDon in (35,37,38,39,40)
					and hd.ChoThanhToan = 0 
					AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
					and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
					GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi

					union all
					-- xuatkho
					SELECT 
						ct.ID_DonViQuiDoi,
						ct.ID_LoHang,
						hd.ID_DonVi,
						0 AS SoLuongNhap_NCC,
						0 AS SoLuongNhap_Kiem,
						0 AS SoLuongNhap_Tra,
						0 AS SoLuongNhap_Chuyen,
						
						0 AS SoLuongXuat_Ban,
						SUM(ct.SoLuong) AS SoLuongXuat_Huy,
						0 AS SoLuongXuat_NCC,
						0 AS SoLuongXuat_Kiem, 
						0 AS SoLuongXuat_Chuyen,					
						SUM(ct.SoLuong * ct.GiaVon) * (-1) AS GiaTri
					FROM BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
					WHERE hd.LoaiHoaDon in (8)
					and hd.ChoThanhToan = 0 
					AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
					and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
					GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi

					union all
					-- xuat traNCC
					SELECT 
						ct.ID_DonViQuiDoi,
						ct.ID_LoHang,
						hd.ID_DonVi,
						0 AS SoLuongNhap_NCC,
						0 AS SoLuongNhap_Kiem,
						0 AS SoLuongNhap_Tra,
						0 AS SoLuongNhap_Chuyen,
						
						0 AS SoLuongXuat_Ban,
						0 AS SoLuongXuat_Huy,
						SUM(ct.SoLuong) AS SoLuongXuat_NCC,
						0 AS SoLuongXuat_Kiem, 
						0 AS SoLuongXuat_Chuyen,						
						SUM(ct.SoLuong * ct.DonGia) * (-1) AS GiaTri
					FROM BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
					WHERE hd.LoaiHoaDon in (7)
					and hd.ChoThanhToan = 0 
					AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
					and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
					GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi

					union all
					-- xuat/nhap kiemke
					SELECT 
						ct.ID_DonViQuiDoi,
						ct.ID_LoHang,
						hd.ID_DonVi,
						 0 AS SoLuongNhap_NCC,
    					sum(IIF(ct.SoLuong >= 0, ct.SoLuong, 0)) AS SoLuongNhap_Kiem, 
						0 AS SoLuongNhap_Tra,
						0 AS SoLuongNhap_Chuyen,
						
						0 AS SoLuongXuat_Ban,
						0 AS SoLuongXuat_Huy,
						0 AS SoLuongXuat_NCC,
    					sum(IIF(ct.SoLuong < 0, ct.SoLuong *(-1), 0)) AS SoLuongXuat_Kiem,
						0 AS SoLuongXuat_Chuyen,						
						sum((ct.ThanhTien - ct.TienChietKhau) * ct.GiaVon) as GiaTri -- soluongthucte - soluongDB (if > 0:nhap else xuat)
					FROM BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
					WHERE hd.LoaiHoaDon in (9)
					and hd.ChoThanhToan = 0 
					AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
					and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
					GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi

				
					union all
					-- xuat chuyenhang
					SELECT 
						ct.ID_DonViQuiDoi,
						ct.ID_LoHang,
						hd.ID_DonVi,
						0 AS SoLuongNhap_NCC,
						0 AS SoLuongNhap_Kiem,
						0 AS SoLuongNhap_Tra,
						0 AS SoLuongNhap_Chuyen,
						
						0 AS SoLuongXuat_Ban,
						0 AS SoLuongXuat_Huy,
						0 AS SoLuongXuat_NCC,
						0 AS SoLuongXuat_Kiem, 
						SUM(ct.TienChietKhau) AS SoLuongXuat_Chuyen,						
						SUM(ct.TienChietKhau * ct.GiaVon) * (-1) AS GiaTri
					FROM BH_HoaDon_ChiTiet ct
					JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
					WHERE hd.ChoThanhToan = 0 and
						((hd.LoaiHoaDon = 10 and hd.yeucau = '1') 
						OR (hd.LoaiHoaDon = 10 and hd.YeuCau = '4'))  
					and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
					AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
					GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi

						---- nhaphang, kiemhang, kh tra, nhanhang
					union all
					-- nhaphang
					SELECT 
						ct.ID_DonViQuiDoi,
						ct.ID_LoHang,
						hd.ID_DonVi,
						SUM(ct.SoLuong) AS SoLuongNhap_NCC,
						0 AS SoLuongNhap_Kiem,
						0 AS SoLuongNhap_Tra,
						0 AS SoLuongNhap_Chuyen,
						
						0 AS SoLuongXuat_Ban,
						0 AS SoLuongXuat_Huy,
						0 AS SoLuongXuat_NCC,
						0 AS SoLuongXuat_Kiem, 
						0 AS SoLuongXuat_Chuyen,		
						sum(case hd.LoaiHoaDon						
							when 4 then iif( hd.TongTienHang = 0,0, ct.SoLuong* (ct.DonGia - ct.TienChietKhau) * (1- hd.TongGiamGia/hd.TongTienHang)) -- sum(ThanhTien) cthd -  TongGiamGia hd
							else ct.SoLuong * ct.GiaVon end) as GiaTri
						FROM BH_HoaDon_ChiTiet ct   
						JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
						WHERE hd.LoaiHoaDon in (4,13,14)
						and hd.ChoThanhToan = 0 
						AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
						and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
						GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi


						union all
						-- kh trahang
						SELECT 
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_DonVi,
							0 AS SoLuongNhap_NCC,
							0 AS SoLuongNhap_Kiem,
							SUM(ct.SoLuong) AS SoLuongNhap_Tra,
							0 AS SoLuongNhap_Chuyen,
							
							0 AS SoLuongXuat_Ban,
							0 AS SoLuongXuat_Huy,
							0 AS SoLuongXuat_NCC,
							0 AS SoLuongXuat_Kiem, 
							0 AS SoLuongXuat_Chuyen,							
							SUM(ct.SoLuong * iif(ctm.GiaVon is null, ct.GiaVon,ctm.GiaVon))  AS GiaTri
						FROM BH_HoaDon_ChiTiet ct   
						JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
						left join BH_HoaDon_ChiTiet ctm on ct.ID_ChiTietGoiDV = ctm.ID
						WHERE hd.LoaiHoaDon = 6  and hd.ChoThanhToan = 0
						and (ct.ChatLieu is null or ct.ChatLieu !='2')
						AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
						and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
						GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi

						union all
						-- nhan chuyenhang
						select
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_CheckIn,
							0 AS SoLuongNhap_NCC,
							0 AS SoLuongNhap_Kiem,
							0 AS SoLuongNhap_Tra,
							SUM(ct.TienChietKhau) AS SoLuongNhap_Chuyen,
							
							0 AS SoLuongXuat_Ban,
							0 AS SoLuongXuat_Huy,
							0 AS SoLuongXuat_NCC,
							0 AS SoLuongXuat_Kiem, 
							0 AS SoLuongXuat_Chuyen,							
							SUM(ct.TienChietKhau * ct.DonGia) AS GiaTri -- lay dongia cua ben chuyen
					FROM BH_HoaDon_ChiTiet ct
					LEFT JOIN BH_HoaDon hd ON ct.ID_HoaDon = hd.ID
					WHERE hd.LoaiHoaDon = 10 and hd.YeuCau = '4' AND hd.ChoThanhToan = 0
					and exists (select ID from @tblChiNhanh dv where hd.ID_CheckIn = dv.ID)
					AND hd.NgaySua >= @timeStart AND hd.NgaySua < @timeEnd
					GROUP BY ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn
			)tblHD
			group by tblHD.ID_DonViQuiDoi,tblHD.ID_LoHang,tblHD.ID_DonVi
		)tblQD
		join DonViQuiDoi qd on tblQD.ID_DonViQuiDoi= qd.ID
		group by qd.ID_HangHoa, tblQD.ID_LoHang, tblQD.ID_DonVi
	
				if @CoPhatSinh= 2
					begin
						select 
							a.TenNhomHang,
							a.TenHangHoa,
							a.MaHangHoa,
							a.TenDonViTinh,
							a.TenLoHang,
							a.TenDonVi,
							a.MaDonVi,

							---- dauky
							isnull(a.TonDauKy,0) as TonDauKy,
							iif(@XemGiaVon='1',isnull(a.GiaTriDauKy,0),0) as GiaTriDauKy,

							----- trongky
							isnull(a.SoLuongNhap_NCC,0) as SoLuongNhap_NCC,
							isnull(a.SoLuongNhap_Kiem,0) as SoLuongNhap_Kiem,
							isnull(a.SoLuongNhap_Tra,0) as SoLuongNhap_Tra,
							isnull(a.SoLuongNhap_Chuyen,0) as SoLuongNhap_Chuyen,

							isnull(a.SoLuongXuat_Ban,0) as SoLuongXuat_Ban,
							isnull(a.SoLuongXuat_Huy,0) as SoLuongXuat_Huy,
							isnull(a.SoLuongXuat_NCC,0) as SoLuongXuat_NCC,
							isnull(a.SoLuongXuat_Kiem,0) as SoLuongXuat_Kiem,
							isnull(a.SoLuongXuat_Chuyen,0) as SoLuongXuat_Chuyen,

							--cuoiky
							isnull(a.TonDauKy,0) + ( a.SoLuongNhap_NCC + a.SoLuongNhap_Kiem + SoLuongNhap_Tra + SoLuongNhap_Chuyen)
								- (SoLuongXuat_Ban + SoLuongXuat_Huy + SoLuongXuat_NCC + SoLuongXuat_Kiem + SoLuongXuat_Chuyen) as TonCuoiKy,
							iif(@XemGiaVon='1',isnull(a.GiaTriDauKy,0) + isnull(a.GiaTri,0),0) as GiaTriCuoiKy

						from
						(
						select 
								isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
								hh.TenHangHoa,
								qd.MaHangHoa,
								concat(hh.TenHangHoa,ThuocTinhGiaTri) as TenHangHoaFull,
								isnull(ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
								qd.TenDonViTinh,
								isnull(lo.MaLoHang,'') as TenLoHang,
								dv.TenDonVi,
								dv.MaDonVi,
				
								-- dauky											
								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.TonKho, 0) as TonDauKy,		
								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.TonKho, 0) * iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.GiaVon, 0) as GiaTriDauKy,
								iif(tkDauKy.ID_DonVi = dv.ID, tkDauKy.GiaVon, 0) as GiaVon,	

									----trongky
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongNhap_NCC, 0) as SoLuongNhap_NCC,		
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongNhap_Kiem, 0) as SoLuongNhap_Kiem,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongNhap_Tra, 0) as SoLuongNhap_Tra,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongNhap_Chuyen, 0) as SoLuongNhap_Chuyen,

								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongXuat_Ban, 0) as SoLuongXuat_Ban,		
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongXuat_Huy, 0) as SoLuongXuat_Huy,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongXuat_NCC, 0) as SoLuongXuat_NCC,	
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongXuat_Kiem, 0) as SoLuongXuat_Kiem,
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.SoLuongXuat_Chuyen, 0) as SoLuongXuat_Chuyen,
								iif(tkTrongKy.ID_DonVi = dv.ID, tkTrongKy.GiaTri, 0) as GiaTri


							from #temp tkTrongKy
							left join DM_HangHoa hh on tkTrongKy.ID_HangHoa= hh.ID
							left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
							left join DonViQuiDoi qd on tkTrongKy.ID_HangHoa = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
							left join DM_LoHang lo on tkTrongKy.ID_LoHang= lo.ID or lo.ID is null
							left join @tkDauKy tkDauKy 
							on hh.ID = tkDauKy.ID_HangHoa and tkDauKy.ID_DonVi= tkTrongKy.ID_DonVi and ((lo.ID= tkDauKy.ID_LoHang) or (lo.ID is null ))
							cross join DM_DonVi  dv									
							where hh.LaHangHoa= 1
								AND hh.TheoDoi LIKE @TheoDoi	
								and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID = allnhh.ID)	
								and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
									AND ((select count(Name) from @tblSearchString b where 
    								hh.TenHangHoa_KhongDau like N'%'+b.Name+'%' 
    								or hh.TenHangHoa_KyTuDau like N'%'+b.Name+'%' 
    									or hh.TenHangHoa like N'%'+b.Name+'%'
    									or lo.MaLoHang like N'%' +b.Name +'%' 
    									or qd.MaHangHoa like N'%'+b.Name+'%'
    									or nhom.TenNhomHangHoa like N'%'+b.Name+'%'
    									or nhom.TenNhomHangHoa_KhongDau like N'%'+b.Name+'%'
    									or nhom.TenNhomHangHoa_KyTuDau like N'%'+b.Name+'%'
    									or qd.TenDonViTinh like N'%'+b.Name+'%'
    									or qd.ThuocTinhGiaTri like N'%'+b.Name+'%'
										or dv.MaDonVi like N'%'+b.Name+'%'
										or dv.TenDonVi like N'%'+b.Name+'%')=@count or @count=0)		
						) a	order by TenHangHoa, TenDonVi,TenLoHang
	end
				else
				begin
				select 	
							dv.MaDonVi, dv.TenDonVi,
							qd.MaHangHoa,
							hh.TenHangHoa,
							hh.QuyCach,
							lo.ID as ID_LoHang,
							qd.ID as ID_DonViQuyDoi,
							concat(hh.TenHangHoa,ThuocTinhGiaTri) as TenHangHoaFull,
							isnull(ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
							isnull(qd.TenDonViTinh,'') as TenDonViTinh,
							isnull(lo.MaLoHang,'') as TenLoHang,

							---- dauky
							isnull(tkDauKy.TonKho,0) as TonDauKy,
							isnull(tkDauKy.GiaVon,0) as GiaVon,
							iif(@XemGiaVon ='1',isnull(tkDauKy.TonKho,0) * isnull(tkDauKy.GiaVon,0),0)  as GiaTriDauKy,			
							isnull(nhom.TenNhomHangHoa,N'Nhóm Hàng Hóa Mặc Định') TenNhomHang,

							---- trongky
							isnull(SoLuongNhap_NCC,0) as SoLuongNhap_NCC,
							isnull(SoLuongNhap_Kiem,0) as SoLuongNhap_Kiem,
							isnull(SoLuongNhap_Tra,0) as SoLuongNhap_Tra,
							isnull(SoLuongNhap_Chuyen,0) as SoLuongNhap_Chuyen,

							isnull(SoLuongXuat_Ban,0) as SoLuongXuat_Ban,
							isnull(SoLuongXuat_Huy,0) as SoLuongXuat_Huy,
							isnull(SoLuongXuat_NCC,0) as SoLuongXuat_NCC,
							isnull(SoLuongXuat_Kiem,0) as SoLuongXuat_Kiem,
							isnull(SoLuongXuat_Chuyen,0) as SoLuongXuat_Chuyen,

							---- cuoiky
							isnull(tkDauKy.TonKho,0) + isnull(SoLuongNhap_NCC,0) +isnull(SoLuongNhap_Kiem,0) +  isnull(SoLuongNhap_Tra,0) + isnull(SoLuongNhap_Chuyen,0)
							- (isnull(SoLuongXuat_Ban,0) + isnull(SoLuongXuat_Huy,0) + isnull(SoLuongXuat_NCC,0) + isnull(SoLuongXuat_Kiem,0) +  isnull(SoLuongXuat_Chuyen,0)) as TonCuoiKy,
							iif(@XemGiaVon ='1',isnull(tkDauKy.TonKho,0) * isnull(tkDauKy.GiaVon,0) + isnull(tkTrongKy.GiaTri,0),0)  as GiaTriCuoiKy
					from DM_HangHoa hh 		
					join DonViQuiDoi qd on hh.ID = qd.ID_HangHoa and qd.LaDonViChuan='1' and qd.Xoa like @TrangThai
					left join DM_LoHang lo on hh.ID = lo.ID_HangHoa
					left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
					cross join DM_DonVi  dv		
					left join @tkDauKy tkDauKy 
					on qd.ID_HangHoa = tkDauKy.ID_HangHoa and tkDauKy.ID_DonVi= dv.ID and ((lo.ID= tkDauKy.ID_LoHang) or (lo.ID is null ))
					left join #temp tkTrongKy on qd.ID_HangHoa = tkTrongKy.ID_HangHoa and tkTrongKy.ID_DonVi= dv.ID and ((lo.ID= tkTrongKy.ID_LoHang) or (lo.ID is null ))
					where hh.LaHangHoa= 1
					AND hh.TheoDoi LIKE @TheoDoi	
					and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID = allnhh.ID)	
					and exists (select ID from @tblChiNhanh cn where dv.ID= cn.id)
						AND ((select count(Name) from @tblSearchString b where 
    					hh.TenHangHoa_KhongDau like N'%'+b.Name+'%' 
    					or hh.TenHangHoa_KyTuDau like N'%'+b.Name+'%' 
    						or hh.TenHangHoa like N'%'+b.Name+'%'
    						or lo.MaLoHang like N'%' +b.Name +'%' 
    						or qd.MaHangHoa like N'%'+b.Name+'%'
    						or nhom.TenNhomHangHoa like N'%'+b.Name+'%'
    						or nhom.TenNhomHangHoa_KhongDau like N'%'+b.Name+'%'
    						or nhom.TenNhomHangHoa_KyTuDau like N'%'+b.Name+'%'
    						or qd.TenDonViTinh like N'%'+b.Name+'%'
    						or qd.ThuocTinhGiaTri like N'%'+b.Name+'%'
							or dv.MaDonVi like N'%'+b.Name+'%'
							or dv.TenDonVi like N'%'+b.Name+'%')=@count or @count=0)		
						order by TenHangHoa, TenDonVi,MaLoHang
				end	
 
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_TongHopHangXuat]
    @ID_DonVi NVARCHAR(MAX),
    @timeStart [datetime],
	@timeEnd [datetime],
    @SearchString [nvarchar](max),
    @ID_NhomHang [uniqueidentifier],
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier],
	@LoaiChungTu [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
	 DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER);
    INSERT INTO @tblChiNhanh SELECT Name FROM splitstring(@ID_DonVi)

	
	declare @tblLoaiHD table(LoaiHD int)
	insert into @tblLoaiHD
	select name from dbo.splitstring(@LoaiChungTu)

	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From
    HT_NguoiDung nd	
    where nd.ID = @ID_NguoiDung)
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

			select 
				dv.MaDonVi, dv.TenDonVi,
				isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
				isnull(lo.MaLoHang,'') as TenLoHang,
				qd.MaHangHoa, qd.TenDonViTinh, 
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				hh.TenHangHoa,
				CONCAT(hh.TenHangHoa,qd.ThuocTinhGiaTri) as TenHangHoaFull,
				round(SoLuong,3) as SoLuong,
				iif(@XemGiaVon='1', round(ThanhTien,3),0) as ThanhTien
			from
			(				
				select 
					qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_DonVi,
					sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
					sum(GiaTriXuat) as ThanhTien
				from
				(
						select 
							hd.NgayLapHoaDon, 
							hd.MaHoaDon,
							hd.LoaiHoaDon,
							hd.ID_HoaDon,
							case hd.LoaiHoaDon
								when 8 then case when hd.ID_PhieuTiepNhan is not null then case when ct.ChatLieu= 4 then 2 else 11 end -- xuat suachua
									else 8 end
								when 1 then case when hd.ID_HoaDon is null and ct.ID_ChiTietGoiDV is not null then 2 --- xuat sudung gdv
									else case when ct.ID_ChiTietDinhLuong is not null then 3 --- xuat ban dinhluong
									else 1 end end -- xuat banle
								else hd.LoaiHoaDon end as LoaiXuatKho, -- xuat khac: traNCC, chuyenang,..
							ct.ID_ChiTietGoiDV,
							ct.ID_DonViQuiDoi,
							ct.ID_LoHang,
							hd.ID_DonVi,
							case hd.LoaiHoaDon
							when 9 then iif(ct.SoLuong < 0, -ct.SoLuong, 0)
							when 10 then ct.TienChietKhau else ct.SoLuong end as SoLuong,
							ct.TienChietKhau,
							ct.GiaVon,
							case hd.LoaiHoaDon
								when 7 then ct.SoLuong * ct.DonGia
								when 10 then ct.TienChietKhau * ct.GiaVon
								when 9 then iif(ct.SoLuong < 0, -ct.SoLuong, 0) * ct.GiaVon
								else ct.SoLuong* ct.GiaVon end as GiaTriXuat
						from BH_HoaDon_ChiTiet ct
						join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
						WHERE hd.ChoThanhToan = 0 
						AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
						and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi = dv.ID)
						and exists (select LoaiHD from @tblLoaiHD loai where hd.LoaiHoaDon= loai.LoaiHD)
						and iif(hd.LoaiHoaDon=9,ct.SoLuong, -1) < 0 -- phieukiemke: chi lay neu soluong < 0 (~xuatkho)
				) tblHD
				join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID										
				group by qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_DonVi		
			)tblQD
			join DM_DonVi dv on tblQD.ID_DonVi = dv.ID
			join DM_HangHoa hh on tblQD.ID_HangHoa= hh.ID
			join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
			left join DM_LoHang lo on tblQD.ID_LoHang= lo.ID and (lo.ID= tblQD.ID_LoHang or (tblQD.ID_LoHang is null and lo.ID is null))
			where hh.LaHangHoa = 1
			and hh.TheoDoi like @TheoDoi
			and qd.Xoa like @TrangThai
			and exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where nhom.ID= allnhh.ID)
			AND ((select count(Name) from @tblSearchString b where 
    		hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    		or hh.TenHangHoa like '%'+b.Name+'%'
    		or lo.MaLoHang like '%' +b.Name +'%' 
			or qd.MaHangHoa like '%'+b.Name+'%'
			or qd.MaHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa like '%'+b.Name+'%'
    		or nhom.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    		or qd.TenDonViTinh like '%'+b.Name+'%'
    		or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
			or dv.MaDonVi like '%'+b.Name+'%'
			or dv.TenDonVi like '%'+b.Name+'%')=@count or @count=0)
		order by dv.TenDonVi, hh.TenHangHoa, lo.MaLoHang	


END");

			Sql(@"ALTER PROCEDURE [dbo].[BCBanHang_GetCTHD]
    @IDChiNhanhs [nvarchar](max),
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiChungTus [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
    	
    	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER)
    INSERT INTO @tblChiNhanh
    select Name from splitstring(@IDChiNhanhs);
    
    	DECLARE @tblLoaiHoaDon TABLE(LoaiHoaDon int)
    INSERT INTO @tblLoaiHoaDon
    select Name from splitstring(@LoaiChungTus);
    
    
    	--- hdmua
    	select 
    		hd.NgayLapHoaDon, hd.MaHoaDon,hd.LoaiHoaDon,
    		hd.ID_DonVi, hd.ID_PhieuTiepNhan, hd.ID_DoiTuong, hd.ID_NhanVien,	
    		hd.TongTienHang, 
			iif(hd.LoaiHoaDon= 36,0,hd.TongGiamGia) as TongGiamGia,
			hd.KhuyeMai_GiamGia,
    		hd.ChoThanhToan,
    		ct.ID, ct.ID_HoaDon, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    		ct.ID_ChiTietGoiDV, ct.ID_ChiTietDinhLuong, ct.ID_ParentCombo,
    		ct.SoLuong, ct.DonGia,  ct.GiaVon,
    		iif(hd.LoaiHoaDon= 36,0, ct.TienChietKhau) as TienChietKhau, 
			ct.TienChiPhi,
    		ct.ThanhTien, ct.ThanhToan,
    		ct.GhiChu, ct.ChatLieu,
    		ct.LoaiThoiGianBH, ct.ThoiGianBaoHanh,		
			ct.TenHangHoaThayThe,
    		Case when hd.TongTienThueBaoHiem > 0 
    			then case when hd.TongThueKhachHang = 0 or hd.TongThueKhachHang is null
    				then ct.ThanhTien * (hd.TongTienThue / hd.TongTienHang) 
    				else ct.TienThue * ct.SoLuong end
    		else ct.TienThue * ct.SoLuong end as TienThue,
    		Case when hd.TongTienHang = 0 then 0 else ct.ThanhTien * ((hd.TongGiamGia + isnull(hd.KhuyeMai_GiamGia,0)) / hd.TongTienHang) end as GiamGiaHD
    	into #cthdMua
    	from BH_HoaDon_ChiTiet ct
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID	
    where hd.ChoThanhToan=0
    and hd.NgayLapHoaDon between @DateFrom and @DateTo
    and exists (select ID from @tblChiNhanh cn where cn.ID = hd.ID_DonVi)
    and exists (select LoaiHoaDon from @tblLoaiHoaDon loai where loai.LoaiHoaDon = hd.LoaiHoaDon)
	--and hd.LoaiHoaDon!=6
    --	and (ct.ChatLieu is null or ct.ChatLieu !='4')
    
    
    	---- === GIA VON HD LE ===
    	select 
    		b.IDComBo_Parent,
    		sum(b.GiaVon) as GiaVon,
    		sum(b.TienVon) as TienVon
    	INTO #gvLe
    	from
    	(
    	 select dluongParent.*,
    			 iif(ctm.ID_ParentCombo is not null, ctm.ID_ParentCombo, ctm.ID) as IDComBo_Parent
    		 from
    		 (
    	select 
    		---- khong get ID_ChiTietGoiDV neu xu ly dathang
    		iif(ctAll.ID_ChiTietGoiDV is not null and ctAll.ID_HoaDon is null, ctAll.ID_ChiTietGoiDV,ctAll.ID) as ID_ChiTietGoiDV,
    		child.GiaVon,
    		child.TienVon
    		from
    		(
    			select 
    				ct.ID_ComBo,
    				sum(ct.GiaVon) as GiaVon,
    				sum(ct.TienVon) as TienVon
    			from
    			(
    			select 
    				iif(ctLe.ID_ParentCombo is not null, ctLe.ID_ParentCombo, 
    								iif(ctLe.ID_ChiTietDinhLuong is not null, ctLe.ID_ChiTietDinhLuong, ctLe.ID)) as ID_ComBo,
    				iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID, 0,  ctLe.GiaVon) as GiaVon,
    				iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID, 0, ctLe.SoLuong * ctLe.GiaVon) as TienVon
    			from #cthdMua ctLe
    			where LoaiHoaDon= 1 
    			) ct group by ct.ID_ComBo
    		) child
    		join #cthdMua ctAll on child.ID_ComBo = ctAll.ID
    		) dluongParent join #cthdMua ctm on dluongParent.ID_ChiTietGoiDV= ctm.ID
    	) b group by b.IDComBo_Parent
    	
    
    ---- xuatkho or sudung gdv
    select hdx.MaHoaDon, hdx.LoaiHoaDon,
    	ctx.ID,	ctx.ID_ChiTietDinhLuong, ctx.ID_ParentCombo,ctx.ID_ChiTietGoiDV,
    	ctx.ID_DonViQuiDoi,
    	ctx.SoLuong, ctx.GiaVon, ctx.ThanhTien
    	into #tblAll
    from BH_HoaDon_ChiTiet ctx
    join BH_HoaDon hdx on ctx.ID_HoaDon= hdx.ID
    	   where hdx.ChoThanhToan = 0 AND exists (
    	   select ID
    	   from #cthdMua ctm where ctx.ID_ChiTietGoiDV = ctm.ID
    )
    
    select xksdGDV.ID_ChiTietGoiDV, xksdGDV.GiaVon, xksdGDV.SoLuong  *  xksdGDV.GiaVon as TienVon
    into #xksdGDV
    from BH_HoaDon_ChiTiet xksdGDV
    where exists (
    	   select ID
    	   from #tblAll ctsc where xksdGDV.ID_ChiTietGoiDV = ctsc.ID)
    
    
    				---- === GIAVON XUATKHO SUA CHUA ===
    	
    				select 
    					c.ID_Parent,
    					sum(c.GiaVon) as GiaVon,
    					sum(c.TienVon) as TienVon
    				into  #xuatSC
    				from
    				(
    				select 
    					iif(ctm2.ID_ParentCombo is not null, ctm2.ID_ParentCombo, ctm2.ID) as ID_Parent,
    					b.GiaVon,
    					b.TienVon
    				from
    				(
    				select 
    					gvXK.ID_Combo,
    					sum(gvXK.GiaVon) as GiaVon,
    					sum(gvXK.TienVon) as TienVon			
    				from
    				(
    				select 
    					IIF(ctm.ID_ParentCombo is not null, ctm.ID_ParentCombo,
    					iif(ctm.ID_ChiTietDinhLuong is not null, ctm.ID_ChiTietDinhLuong, ctm.ID)) as ID_Combo,
    					b.GiaVon,
    					b.TienVon
    				from
    				(		
    				   select 
    					gvComBo.ID_ChiTietGoiDV,
    					sum(GiaVon) as GiaVon,
    					sum(TienVon) as TienVon
    				
    				   from
    				   (
    				   select 
    						iif(ctXuat.ID_ChiTietGoiDV is not null, ctXuat.ID_ChiTietGoiDV, ctXuat.ID) as ID_ChiTietGoiDV,
    						0 as GiaVon,
    						ctXuat.SoLuong * ctXuat.GiaVon as TienVon
    					   from #tblAll ctXuat
    					   where ctXuat.LoaiHoaDon = 8
    					) gvComBo group by gvComBo.ID_ChiTietGoiDV
    				) b
    				join #cthdMua ctm on b.ID_ChiTietGoiDV= ctm.ID
    				) gvXK 
    				group by gvXK.ID_Combo
    				) b
    				join #cthdMua ctm2 on b.ID_Combo = ctm2.ID
    				) c group by c.ID_Parent
    				
    
    
    				----  === GIAVON XUAT SUDUNG ===
    		select gvSD.IDComBo_Parent,
    			sum(gvSD.GiaVon) as GiaVon,
    			sum(gvSD.TienVon) as TienVon
    		into #gvSD
    		from
    		(
    			 ---- group combo at parent
    			 select 
    				iif(ctm2.ID_ParentCombo is not null, ctm2.ID_ParentCombo, ctm2.ID) as IDComBo_Parent,
    				b.GiaVon,
    				b.TienVon
    			 from(
    					select c.*,
    						iif(ctm.ID_ChiTietDinhLuong is not null, ctm.ID_ChiTietDinhLuong, ctm.ID) as IDDLuong_Parent
    					from
    					(
    					---- group dinhluong at parent by id_ctGoiDV
    						select 
    						iif(ctAll.ID_ChiTietGoiDV is not null, ctAll.ID_ChiTietGoiDV,ctAll.ID) as ID_ChiTietGoiDV,
    						child.GiaVon,
    						child.TienVon
    						from
    						(
    						
    							---- xuat sudung gdv le
    							select 
    								gvComBo.ID_ComBo,
    								sum(GiaVon) as GiaVon,
    								sum(TienVon) as TienVon
    							from
    							(
    							select 
    								iif(ctLe.ID_ParentCombo is not null, ctLe.ID_ParentCombo, 
    									iif(ctLe.ID_ChiTietDinhLuong is not null, ctLe.ID_ChiTietDinhLuong, ctLe.ID)) as ID_ComBo,
    								iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID , 0,  ctLe.GiaVon) as GiaVon,
    								iif(ctLe.ID_ParentCombo = ctLe.ID or ctLe.ID_ChiTietDinhLuong = ctLe.ID  , 0, ctLe.SoLuong * ctLe.GiaVon) as TienVon
    							from #tblAll ctLe
    							where ctLe.LoaiHoaDon in (1)
    
    							---- xuat sudung goi baoduong
    							union all
    							select *
    							from #xksdGDV
    							
    							) gvComBo group by gvComBo.ID_ComBo
    						) child
    						join #tblAll ctAll on child.ID_ComBo = ctAll.ID
    				) c join #cthdMua ctm on c.ID_ChiTietGoiDV= ctm.ID
    			) b join #cthdMua ctm2 on b.IDDLuong_Parent = ctm2.ID
    		) gvSD
    		group by gvSD.IDComBo_Parent
    	
    					
    	--select *
    	--	from #xuatSC
    
    	--	select *
    	--	from #gvSD
    
    select 
    	ctmua.*,
    	isnull(gv.GiaVon,0) as GiaVon,
    	isnull(gv.TienVon,0) as TienVon
    from #cthdMua ctmua
    left join
    (
    	
    		---- giavon hdle
    		select *
    		from #gvLe
    
    		union all
    		--- giavon xuatkho sc
    		select *
    		from #xuatSC
    
    		union all
    		--- giavon xuatkho sudung gdv
    		select *
    		from #gvSD				
    	) gv on ctmua.ID = gv.IDComBo_Parent	
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetAll_TonKhoDauKy]
    @IDDonVis [nvarchar](max),
    @ToDate [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER);
    	INSERT INTO @tblChiNhanh SELECT Name FROM splitstring(@IDDonVis)
    
    		--SELECT 
    		--	ID_DonViInput,
    		--	ID_HangHoa, 		
    		--	ID_LoHang,
    		--	IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, TonLuyKe_NhanChuyenHang, TonLuyKe) AS TonKho, 
    		--	IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, GiaVon_NhanChuyenHang, GiaVon) AS GiaVon
    		--	FROM (
    		--	SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang, tbltemp.ID_DonViInput ORDER BY tbltemp.ThoiGian DESC) AS RN 
    		--	FROM (
    		--		SELECT hd.LoaiHoaDon, dvqd.ID_HangHoa, hd.ID_DonVi, hd.ID_CheckIn, lstDv.ID AS ID_DonViInput, 
    		--			IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, hdct.TonLuyKe_NhanChuyenHang, hdct.TonLuyKe) AS TonLuyKe,
    		--			hdct.TonLuyKe_NhanChuyenHang,
    		--			IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, 
    		--			hdct.GiaVon_NhanChuyenHang, 
    		--			hdct.GiaVon)/ISNULL(dvqd.TyLeChuyenDoi,1) AS GiaVon,
    		--			hdct.GiaVon_NhanChuyenHang, 
    		--			hdct.ID_LoHang ,
    		--			IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
    		--		FROM BH_HoaDon_ChiTiet hdct
    		--		JOIN BH_HoaDon hd ON hd.ID = hdct.ID_HoaDon				
    		--		JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi				
    		--		INNER JOIN @tblChiNhanh lstDv ON lstDv.ID = hd.ID_DonVi OR (hd.ID_CheckIn = lstDv.ID and hd.YeuCau = '4')				
    		--		where hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10,18, 13,14)
    		--		and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID or (hd.ID_CheckIn= dv.ID and hd.LoaiHoaDon= 10))
    		--		) as tbltemp
    		--WHERE tbltemp.ThoiGian < @ToDate) tblTonKhoTemp
    		--WHERE tblTonKhoTemp.RN = 1;



		select 
				ID_DonViInput,			
				ID_HangHoa,		
    			ID_LoHang,
				TonLuyKe,
				GiaVon
		from
		(
		select 
				tblUnion.ID_DonViInput, 
				tblUnion.LoaiHoaDon,
				tblUnion.NgayLapHoaDon,
				dvqd.ID_HangHoa,
				tblUnion.ID_LoHang,
				tblUnion.TonLuyKe,
				tblUnion.GiaVon / ISNULL(dvqd.TyLeChuyenDoi,1) as GiaVon,
				ROW_NUMBER() OVER (PARTITION BY dvqd.ID_HangHoa, tblUnion.ID_LoHang, tblUnion.ID_DonViInput ORDER BY tblUnion.NgayLapHoaDon DESC) AS RN
		from
		(
			select *
			from
			(
				SELECT hd.ID_DonVi as ID_DonViInput, 					
						hd.LoaiHoaDon,
						hd.NgayLapHoaDon,					
						hdct.ID_DonViQuiDoi,
						hdct.ID_LoHang,
						hdct.TonLuyKe,
						hdct.GiaVon
    			FROM BH_HoaDon_ChiTiet hdct
    			JOIN BH_HoaDon hd ON hd.ID = hdct.ID_HoaDon	
    			where hd.ChoThanhToan = 0  
				AND hd.LoaiHoaDon != 10
    			and hd.ID_DonVi in (select ID from @tblChiNhanh dv)
				and hd.NgayLapHoaDon < @ToDate

				union all

				SELECT
						cn.ID as ID_DonViInput,
						hd.LoaiHoaDon,
						IIF(ID_CheckIn = cn.ID and hd.YeuCau='4', hd.NgaySua, hd.NgayLapHoaDon) as NgayLapHoaDon,
						hdct.ID_DonViQuiDoi,
						hdct.ID_LoHang,
						IIF(ID_CheckIn = cn.ID and hd.YeuCau='4', hdct.TonLuyKe_NhanChuyenHang, hdct.TonLuyKe) AS TonKho, 
    					IIF(ID_CheckIn = cn.ID and hd.YeuCau='4', hdct.GiaVon_NhanChuyenHang, hdct.GiaVon) AS GiaVon
    			FROM BH_HoaDon_ChiTiet hdct
    			JOIN BH_HoaDon hd ON hd.ID = hdct.ID_HoaDon
				JOIN @tblChiNhanh cn ON cn.ID = hd.ID_DonVi OR (hd.ID_CheckIn = cn.ID and hd.YeuCau = '4')		
    			where hd.ChoThanhToan = 0 
				AND hd.LoaiHoaDon = 10
    			and (hd.ID_CheckIn in (select ID from @tblChiNhanh)
				or (hd.ID_DonVi in (select ID from @tblChiNhanh )))
				and (hd.NgayLapHoaDon < @ToDate or hd.NgaySua < @ToDate)
			) tblCheck where tblCheck.NgayLapHoaDon < @ToDate
		) tblUnion
		JOIN DonViQuiDoi dvqd ON tblUnion.ID_DonViQuiDoi = dvqd.ID	
		) tblRN where tblRN.RN = 1
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetChiTietHoaDon_ByIDHoaDon]
    @ID_HoaDon [uniqueidentifier]	
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
    							TenDonViTinh,MaHangHoa,YeuCau,
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
					
    					FROM BH_HoaDon hd
    					JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    					JOIN DonViQuiDoi qd ON cthd.ID_DonViQuiDoi = qd.ID
    					JOIN DM_HangHoa hh ON qd.ID_HangHoa= hh.ID    		
    					left JOIN DM_NhomHangHoa nhh ON hh.ID_NhomHang= nhh.ID    							
    					LEFT JOIN DM_LoHang lo ON cthd.ID_LoHang = lo.ID
						left join DM_HangHoa_TonKho tk on cthd.ID_DonViQuiDoi= tk.ID_DonViQuyDoi and tk.ID_DonVi= @ID_DonVi
						left join DM_ViTri vt on cthd.ID_ViTri= vt.ID
    					-- chi get CT khong phai la TP dinh luong
    					WHERE cthd.ID_HoaDon = @ID_HoaDon
								and (cthd.ChatLieu is null or cthd.ChatLieu!='5')
								AND (cthd.ID_ChiTietDinhLuong IS NULL OR cthd.ID_ChiTietDinhLuong = cthd.ID)
								and ((tk.ID_DonVi = hd.ID_DonVi and hh.LaHangHoa='1') 
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
					hd.NgayLapHoaDon as ThoiGian, cthd.GhiChu,
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
    				TyLeChuyenDoi, YeuCau,
    				lo.ID AS ID_LoHang, ISNULL(MaLoHang,'') as MaLoHang, 			
					ISNULL(hhtonkho.TonKho, 0) as TonKho, 
					hd.ID_DonVi, dvqd.GiaNhap, 
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
    			FROM BH_HoaDon hd
    			JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
				left join
				(
						----- get sl conlai saukhi xuly nhapmua
					select ctpo.ID, ctpo.SoLuong - isnull(ctXL.SoLuong,0) as SoLuongConLai
					from BH_HoaDon_ChiTiet ctpo
					left join
					(
						---- ctxuly != hd current
						select ctxl.SoLuong, ctxl.ID_ChiTietGoiDV
						from BH_HoaDon_ChiTiet ctxl
						join BH_HoaDon hdxl on ctxl.ID_HoaDon= hdxl.ID
						where hdxl.ID_HoaDon= @ID_HoaDonGoc and hdxl.ChoThanhToan='0' and hdxl.LoaiHoaDon= 4
						and hdxl.ID != @ID_HoaDon
					) ctXL on ctpo.ID= ctXL.ID_ChiTietGoiDV
				) ctConLai on cthd.ID_ChiTietGoiDV= ctConLai.ID
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
				hd.NgayLapHoaDon as ThoiGian, cthd.GhiChu,
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
    			TyLeChuyenDoi, YeuCau,
    			lo.ID AS ID_LoHang, ISNULL(MaLoHang,'') as MaLoHang, 			
				ISNULL(hhtonkho.TonKho, 0) as TonKho, 
				hd.ID_DonVi, dvqd.GiaNhap, 
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
    			FROM BH_HoaDon hd
    			JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
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

			Sql(@"ALTER PROCEDURE [dbo].[GetGiaVonTieuChuan_byTime]
    @IDChiNhanhs [uniqueidentifier],
    @ToDate [datetime],
    @IDDonViQuyDois [nvarchar](max),
    @IDLoHangs [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
    		declare @tblChiNhanh table (ID_DonVi uniqueidentifier)		    
    		insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs) 
    	
    
    	
    		declare @tblIDQuiDoi table (ID_DonViQuyDoi uniqueidentifier)
    	declare @tblIDLoHang table (ID_LoHang uniqueidentifier)
    
    		insert into @tblIDQuiDoi
    	select Name from dbo.splitstring(@IDDonViQuyDois) 
    	insert into @tblIDLoHang
    	select Name from dbo.splitstring(@IDLoHangs) where Name not like '%null%' and Name !=''
    
    		select 
    			tbl.ID_DonVi,
    			tbl.ID_DonViQuiDoi,
    			tbl.ID_LoHang, 
    			tbl.GiaVonTieuChuan			
    		from
    			(
    			select 
    				gvGanNhat.ID_DonVi, 
    				gvGanNhat.ID_DonViQuiDoi, 
    				gvGanNhat.ID_LoHang, 
    				gvGanNhat.GiaVonTieuChuan,
    				ROW_NUMBER() over (partition by gvGanNhat.ID_DonVi, gvGanNhat.ID_DonViQuiDoi, gvGanNhat.ID_LoHang order by gvGanNhat.NgayLapHoaDon desc) as RN
    			from
    			(
    				select ct.ID_DonViQuiDoi, ct.ID_LoHang, 
    					ct.ThanhTien as GiaVonTieuChuan,
    					hd.ID_DonVi,
    					hd.NgayLapHoaDon
    				from @tblIDQuiDoi qd
    				join BH_HoaDon_ChiTiet ct on qd.ID_DonViQuyDoi = ct.ID_DonViQuiDoi
    				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
    				where hd.ChoThanhToan=0
    				and  hd.ID_DonVi in (select ID_DonVi from @tblChiNhanh)
    				and hd.LoaiHoaDon= 16
    				and hd.NgayLapHoaDon < @ToDate
    				and (ct.ID_LoHang is null or ct.ID_LoHang in (select ID_LoHang from @tblIDLoHang))
    			 ) gvGanNhat   
    		 ) tbl where tbl.RN= 1
    	
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetHisChargeValueCard]
    @ID_DoiTuong [uniqueidentifier],
    @IDChiNhanhs [nvarchar](max),
	@FromDate datetime,
	@ToDate datetime,
	@CurrentPage int,
	@PageSize int
AS
BEGIN
    SET NOCOUNT ON;
	with data_cte
		as(
		select a.*, 
			b.MaPhieuThu,
			b.ID_SoQuy,
			ISNULL(b.TienThu,0) as KhachDaTra			
		from  
    		   (select hd.ID, 
					hd.ID_DonVi,
					MaHoaDon, 
					LoaiHoaDon,
					NgayLapHoaDon, 			
    				hd.TongTienHang,
					hd.TongGiamGia,
					hd.TongChietKhau,
					case hd.LoaiHoaDon
						when 22 then hd.TongTienHang
						when 32 then 0
						when 23 then iif(hd.TongTienHang > 0, hd.TongTienHang,0)
					end as PhatSinhTang,
					case hd.LoaiHoaDon
						when 22 then 0
						when 32 then hd.TongTienHang
						when 23 then iif(hd.TongTienHang < 0, hd.TongTienHang,0)
					end as PhatSinhGiam,
    				case hd.LoaiHoaDon
						when 22 then N'Nạp thẻ'
						when 32 then N'Hoàn trả cọc'
						when 23 then iif(hd.TongTienHang > 0,  N'Điều chỉnh tăng', N'Điều chỉnh giảm')
					end as SLoaiHoaDon
    			from BH_HoaDon hd
    			join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			join DM_DonVi dv on hd.ID_DonVi= dv.ID
    			where LoaiHoaDon in (22,23, 32) and ChoThanhToan='0' and ID_DoiTuong= @ID_DoiTuong
				and hd.NgayLapHoaDon between @FromDate and @ToDate
    			) a
    		left join (select qct.ID_HoaDonLienQuan, 
								max(qct.ID_HoaDon) as ID_SoQuy,
								sum(qct.TienThu) as TienThu, MAX(qhd.MaHoaDon) as MaPhieuThu
    					from Quy_HoaDon_ChiTiet qct 
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    					where qct.ID_DoiTuong= @ID_DoiTuong
    					and qhd.TrangThai ='1'
    					group by qct.ID_HoaDonLienQuan) b
    		on a.ID= b.ID_HoaDonLienQuan
			),
			count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(PhatSinhTang) as TongTang,
				sum(PhatSinhGiam) as TongGiam
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROC [dbo].[getList_ChietKhauNhanVienTongHop]
@ID_ChiNhanhs [nvarchar](max),
@ID_NhanVienLogin nvarchar(max),
@DepartmentIDs [nvarchar](max),
@TextSearch [nvarchar](500),
@DateFrom [nvarchar](max),
@DateTo [nvarchar](max),
@CurrentPage int,
@PageSize int
AS
BEGIN
set nocount on;
	
	declare @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
	Status_DoanhThu varchar(4),	TotalRow int, TotalPage float, TongAllDoanhThu float, TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float)
	INSERT INTO @tab_DoanhSo exec GetAll_DiscountSale @ID_ChiNhanhs,@ID_NhanVienLogin, @DepartmentIDs, @DateFrom, @DateTo, @TextSearch, '', 0,500;	

	DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
	TotalRow int, TotalPage float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongHoaHongVND float, TongAllAll float)
	INSERT INTO @tab_HoaDon exec ReportDiscountInvoice @ID_ChiNhanhs,@ID_NhanVienLogin, @DepartmentIDs, @TextSearch,'0,1,6,19,22,25,32,36', @DateFrom, @DateTo, 8, 1, 0, 0,500;

	DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
	TotalRow int, TotalPage float, TongHoaHongThucHien float, TongHoaHongThucHien_TheoYC float,  TongHoaHongTuVan float, TongHoaHongBanGoiDV float, TongAll float)
	INSERT INTO @tab_HangHoa exec ReportDiscountProduct_General @ID_ChiNhanhs,@ID_NhanVienLogin, @DepartmentIDs, @TextSearch,'0,1,6,19,22,25,2,36', @DateFrom, @DateTo, 16,1, 0,500;	

	with data_cte
	as
	 (
		SELECT a.ID_NhanVien, a.MaNhanVien, a.TenNhanVien, 
			SUM(HoaHongThucHien) as HoaHongThucHien,
			SUM(HoaHongThucHien_TheoYC) as HoaHongThucHien_TheoYC,
			SUM(HoaHongTuVan) as HoaHongTuVan,
			SUM(HoaHongBanGoiDV) as HoaHongBanGoiDV,
			SUM(TongHangHoa) as TongHangHoa,
			SUM(HoaHongDoanhThuHD) as HoaHongDoanhThuHD,
			SUM(HoaHongThucThuHD) as HoaHongThucThuHD,
			SUM(HoaHongVND) as HoaHongVND,
			SUM(TongHoaDon) as TongHoaDon,
			SUM(DoanhThu) as DoanhThu,
			SUM(ThucThu) as ThucThu,
			SUM(HoaHongDoanhThuDS) as HoaHongDoanhThuDS,
			SUM(HoaHongThucThuDS) as HoaHongThucThuDS,
			SUM(TongDoanhSo) as TongDoanhSo,
			SUM(TongDoanhSo + TongHoaDon + TongHangHoa) as Tong
		FROM 
		(
		select ID_NhanVien, MaNhanVien, TenNhanVien, 
		HoaHongThucHien, 
		HoaHongThucHien_TheoYC, 
		HoaHongTuVan, 
		HoaHongBanGoiDV, 
		Tong as TongHangHoa,
		0 as HoaHongDoanhThuHD,
		0 as HoaHongThucThuHD,
		0 as HoaHongVND,
		0 as TongHoaDon,
		0 as DoanhThu,
		0 as ThucThu,
		0 as HoaHongDoanhThuDS,
		0 as HoaHongThucThuDS,
		0 as TongDoanhSo
		from @tab_HangHoa
		UNION ALL
		Select ID_NhanVien, MaNhanVien, TenNhanVien, 
		0 as HoaHongThucHien,
		0 as HoaHongThucHien_TheoYC,
		0 as HoaHongTuVan,
		0 as HoaHongBanGoiDV,
		0 as TongHangHoa,
		HoaHongDoanhThu as HoaHongDoanhThuHD,
		HoaHongThucThu as HoaHongThucThuHD,
		HoaHongVND,
		TongAll as TongHoaDon,
		0 as DoanhThu,
		0 as ThucThu,
		0 as HoaHongDoanhThuDS,
		0 as HoaHongThucThuDS,
		0 as TongDoanhSo
		from @tab_HoaDon
		UNION ALL
		Select ID_NhanVien, MaNhanVien, TenNhanVien, 
		0 as HoaHongThucHien,
		0 as HoaHongThucHien_TheoYC,
		0 as HoaHongTuVan,
		0 as HoaHongBanGoiDV,
		0 as TongHangHoa,
		0 as HoaHongDoanhThuHD,
		0 as HoaHongThucThuHD,
		0 as HoaHongVND,
		0 as TongHoaDon,
		TongDoanhThu as DoanhThu,
		TongThucThu as ThucThu,
		HoaHongDoanhThu as HoaHongDoanhThuDS,
		HoaHongThucThu as HoaHongThucThuDS,
		TongAll as TongDoanhSo
		from @tab_DoanhSo
		) as a
		GROUP BY a.ID_NhanVien, a.MaNhanVien, a.TenNhanVien
	),
	count_cte
	as (
		select count(ID_NhanVien) as TotalRow,
			CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float ))  as TotalPage,
			sum(HoaHongThucHien) as TongHoaHongThucHien,
			sum(HoaHongThucHien_TheoYC) as TongHoaHongThucHien_TheoYC,
			sum(HoaHongTuVan) as TongHoaHongTuVan,
			sum(HoaHongBanGoiDV) as TongHoaHongBanGoiDV,
			sum(TongHangHoa) as TongHoaHong_TheoHangHoa,

			sum(HoaHongDoanhThuHD) as TongHoaHongDoanhThu,
			sum(HoaHongThucThuHD) as TongHoaHongThucThu,
			sum(HoaHongVND) as TongHoaHongVND,
			sum(TongHoaDon) as TongHoaHong_TheoHoaDon,
			sum(DoanhThu) as TongDoanhThu,
			sum(ThucThu) as TongThucThu,

			sum(HoaHongDoanhThuDS) as TongHoaHongDoanhThuDS,
			sum(HoaHongThucThuDS) as TongHoaHongThucThuDS,
			sum(TongDoanhSo) as TongHoaHong_TheoDoanhSo,
			sum(Tong) as TongHoaHongAll

		from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.MaNhanVien
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_XuatHuy]
    @IDChiNhanhs nvarchar(max)= null,
	@DateFrom datetime= null,
	@DateTo datetime= null,
	@LoaiHoaDons nvarchar(max)= null,
	@TrangThais nvarchar(max)= null,
	@TextSearch nvarchar(max)=null,
	@CurrentPage int= null,
	@PageSize int = null
AS
BEGIN
	set nocount on;
	declare @sqlSub nvarchar(max) ='', @whereSub nvarchar(max) ='',@tblSub varchar(max) ='',
	@sql nvarchar(max) ='', @where nvarchar(max) ='', @tblOut varchar(max) ='',
	@paramDefined nvarchar(max) =''

	declare @paramIn nvarchar(max) =' declare @textSeach_isNull int = 1'

	set @where =' where 1 = 1 '

	set @whereSub =' where 1 = 1 '


	
	set @paramDefined = N'@IDChiNhanhs_In nvarchar(max)= null,
							@DateFrom_In datetime= null,
							@DateTo_In datetime= null,
							@LoaiHoaDons_In nvarchar(max)= null,
							@TrangThais_In nvarchar(max)= null,
							@TextSearch_In nvarchar(max)= null,
							@CurrentPage_In int= null,
							@PageSize_In int = null '


	if isnull(@CurrentPage,'') ='' set @CurrentPage= 0
	if isnull(@PageSize,'') ='' set @PageSize= 10

	if isnull(@IDChiNhanhs,'') !=''
	begin
		set @tblSub = CONCAT(@tblSub ,N'  declare @tblChiNhanh table(ID uniqueidentifier)
										 insert into @tblChiNhanh 
										 select name from dbo.splitstring(@IDChiNhanhs_In); ')
		set @whereSub = CONCAT(@whereSub, N' and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)' )
	end
	
	if isnull(@DateFrom,'') !=''
		begin
			set @whereSub = CONCAT(@whereSub, N' and hd.NgayLapHoaDon >= @DateFrom_In' )
		end
	if isnull(@DateTo,'') !=''
		begin
			set @whereSub = CONCAT(@whereSub, N' and hd.NgayLapHoaDon < @DateTo_In' )
		end

	if isnull(@LoaiHoaDons,'') !=''
	begin
		set @tblOut = CONCAT(@tblOut ,N'  declare @tblLoai table(Loai int)
									 insert into @tblLoai 
									 select name from dbo.splitstring(@LoaiHoaDons_In) ;')
		set @where = CONCAT(@where, N' and exists (select Loai from @tblLoai loai where tbl.LoaiHoaDon= loai.Loai)' )
	end

	if isnull(@TrangThais,'') !=''
	begin
		set @tblOut = CONCAT(@tblOut ,N'  declare @tblTrangThai table(TrangThai int)
									 insert into @tblTrangThai 
									 select name from dbo.splitstring(@TrangThais_In) ;')
		set @where = CONCAT(@where, N' and exists (select TrangThai from @tblTrangThai tt where tbl.TrangThai= tt.TrangThai)' )
	end

	if isnull(@TextSearch,'') !=''
	begin
		set @paramIn = concat(@paramIn, N' set @textSeach_isNull =0 ')
		set @tblOut = CONCAT(@tblOut ,N'  DECLARE @tblSearchString TABLE (Name [nvarchar](max));
									DECLARE @count int;
									INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''';
									Select @count =  (Select count(*) from @tblSearchString) ;')
		set @where = CONCAT(@where, N' and ((select count(Name) from @tblSearchString b 
									where tbl.MaHoaDon like ''%'' +b.Name +''%'' 										
										or tbl.NguoiTaoHD like ''%'' +b.Name +''%'' 
										or tbl.DienGiai like N''%''+b.Name+''%''
										or nv.TenNhanVien like ''%''+b.Name+''%''
										or nv.TenNhanVienKhongDau like ''%''+b.Name+''%''
										or nv.TenNhanVienChuCaiDau like ''%''+b.Name+''%''
										or nv.MaNhanVien like ''%''+b.Name+''%''
										or tn.MaPhieuTiepNhan like ''%''+b.Name+''%''
										or hdsc.MaHoaDon like ''%''+b.Name+''%''
										or tbl.TenDoiTuong like ''%''+b.Name+''%''
										or tbl.TenDoiTuong_KhongDau like ''%''+b.Name+''%''
										or tbl.MaDoiTuong like ''%''+b.Name+''%''
										or xe.BienSo like ''%''+b.Name+''%''									
										or tbl.DienGiaiUnsign like N''%''+b.Name+''%''
										)=@count or @count=0)' )
	end

	set @sqlSub = CONCAT(
	N' select 
			hd.ID,
    		hd.ID_NhanVien,
    		hd.ID_DonVi,
			hd.ID_HoaDon,
			hd.ID_PhieuTiepNhan,			
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,   	
    		ISNULL(hdct.SoLuong * hdct.GiaVon, 0) as TongTienHang,
    		hd.DienGiai,
			hd.DienGiai as DienGiaiUnsign,
			hd.ChoThanhToan,
			hd.NguoiTao as NguoiTaoHD, 		
			case hd.LoaiHoaDon
				when 8 then iif(hd.ChoThanhToan=''0'',hd.NgayLapHoaDon, hd.NgaySua)
			else hd.NgaySua end as NgaySua,
			isnull(hd.An_Hien,''0'') as IsChuyenPhatNhanh,
			case hd.ChoThanhToan
				when 1 then 1
				when 0 then 2
			else 3 end as TrangThai,
			case hd.ChoThanhToan
				when 1 then N''Tạm lưu''
				when 0 then N''Hoàn thành''
			else N''Hủy bỏ'' end as YeuCau,    
			---- 1.sudung gdv, 2.xuat banle, 3.xuat suachua, 8.xuatkho thuong, 12.xuatbaohanh, 35. xuat NVL, 36. xuat hotro chung, 37. xuat hotro ngaythuoc
			case hd.LoaiHoaDon			
				when 2 then 12
			else hd.LoaiHoaDon end LoaiHoaDon,
			case hd.LoaiHoaDon
				when 8 then N''Phiếu xuất kho''					
				when 40 then N''Xuất hỗ trợ chung'' 
				when 39 then N''Xuất bảo hành''
				when 38 then N''Xuất bán lẻ''
				when 37 then N''Xuất hỗ trợ ngày thuốc''
				when 35 then N''Xuất nguyên vật liệu''
			else N''Xuất bán lẻ'' end LoaiPhieu,

			case hd.LoaiHoaDon
				when 8 then '''' 	
				when 40 then dt.TenDoiTuong
				when 39 then dt.TenDoiTuong
				when 38 then dt.TenDoiTuong
				when 37 then dt.TenDoiTuong
				when 35 then dt.TenDoiTuong
			else '''' end TenDoiTuong,
			case hd.LoaiHoaDon
				when 8 then '''' 	
				when 40 then dt.TenDoiTuong_KhongDau
				when 39 then dt.TenDoiTuong_KhongDau
				when 38 then dt.TenDoiTuong_KhongDau
				when 37 then dt.TenDoiTuong_KhongDau
				when 35 then dt.TenDoiTuong_KhongDau
			else '''' end TenDoiTuong_KhongDau,
			case hd.LoaiHoaDon
				when 8 then '''' 	
				when 40 then dt.MaDoiTuong
				when 39 then dt.MaDoiTuong
				when 38 then dt.MaDoiTuong
				when 37 then dt.MaDoiTuong
				when 35 then dt.MaDoiTuong
			else '''' end MaDoiTuong

			into #hdXK
			from BH_HoaDon hd
			join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
			left join DM_DoiTuong dt on hd.ID_Doituong= dt.ID
									', @whereSub)

		set @sql= CONCAT(@paramIn, @tblSub, @tblOut, @sqlSub, '; ',
			N' with data_cte
			as(
				select tbl.ID,
					tbl.MaHoaDon,
					tbl.NgayLapHoaDon,
					tbl.NgaySua,
					tbl.ID_HoaDon,
					tbl.ID_PhieuTiepNhan,
					tbl.ID_NhanVien,
					tbl.ID_DonVi,				
					sum(tbl.TongTienHang) as TongTienHang,
					tbl.ChoThanhToan,
					tbl.LoaiHoaDon,
					tbl.LoaiPhieu,
					tbl.NguoiTaoHD,
					tbl.YeuCau,
					tbl.DienGiai,
					tbl.IsChuyenPhatNhanh,
					tn.MaPhieuTiepNhan,
					hdsc.MaHoaDon as MaHoaDonGoc,
					xe.BienSo,					
					dv.TenDonVi,
					nv.TenNhanVien,
					tbl.TenDoiTuong,
					tbl.MaDoiTuong,
					cast(tbl.TrangThai as varchar(2)) as TrangThai
			from #hdXK tbl 
			join DM_DonVi dv on tbl.ID_DonVi = dv.ID
			left join BH_HoaDon hdsc on tbl.ID_HoaDon= hdsc.ID
			left join Gara_PhieuTiepNhan tn on tbl.ID_PhieuTiepNhan= tn.ID
			left join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
    		left join NS_NhanVien nv on tbl.ID_NhanVien = nv.ID
			', @where ,
			N' group by 
					tbl.ID,
					tbl.MaHoaDon,
					tbl.NgayLapHoaDon,
					tbl.NgaySua,
					tbl.ID_HoaDon,
					tbl.ID_PhieuTiepNhan,
					tbl.ID_NhanVien,
					tbl.ID_DonVi,		
					tbl.ChoThanhToan,
					tbl.LoaiHoaDon,
					tbl.LoaiPhieu,
					tbl.NguoiTaoHD,
					tbl.YeuCau,
					tbl.DienGiai,
					tn.MaPhieuTiepNhan,
					hdsc.MaHoaDon,
					xe.BienSo,					
					dv.TenDonVi,
					nv.TenNhanVien,
					tbl.TenDoiTuong,
					tbl.MaDoiTuong,
					tbl.TrangThai,
					tbl.IsChuyenPhatNhanh
			),
			count_cte
			as (
			select count(ID) as TotalRow,
				sum(TongTienHang) as SumTongTienHang
			from data_cte
			)
			select dt.*, cte.*
			from data_cte dt
			cross join count_cte cte
			order by dt.NgayLapHoaDon desc
			OFFSET (@CurrentPage_In* @PageSize_In) ROWS
			FETCH NEXT @PageSize_In ROWS ONLY; 
			')

			--print @sql
		
			
			exec sp_executesql @sql, @paramDefined,
					 @IDChiNhanhs_In = @IDChiNhanhs,
					@DateFrom_In = @DateFrom,
					@DateTo_In = @DateTo,
					@LoaiHoaDons_In = @LoaiHoaDons,
					@TrangThais_In = @TrangThais,
					@TextSearch_In =@TextSearch,
					@CurrentPage_In = @CurrentPage,
					@PageSize_In = @PageSize
				
	
END");

			Sql(@"ALTER PROCEDURE [dbo].[getListHangHoaBy_IDNhomHang]
	@ID_DonVi uniqueidentifier,
    @IDNhomHangs nvarchar(max) = null,
	@LoaiHangHoas nvarchar(max) = '1,2,3'
AS
BEGIN
	set nocount on;
	declare @tblLoaiHang table(LoaiHang int)
	insert into @tblLoaiHang
	select name from dbo.splitstring(@LoaiHangHoas)

	if ISNULL(@IDNhomHangs,'')=''
	begin
		 select 
			hh.ID,
    		dvqd.ID as ID_DonViQuiDoi,
			lh.ID as ID_LoHang,
    		dvqd.MaHangHoa,
			IIF(hh.LoaiHangHoa is not null, hh.LoaiHangHoa, iif(hh.LaHangHoa=0,2,1)) as LoaiHangHoa,
			case when hh.QuanLyTheoLoHang is null then '0' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    		concat(hh.TenHangHoa , dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
    		hh.TenHangHoa,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd.TenDonViTinh,
			lh.MaLoHang,
			Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
			Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
			Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan
    	
    	FROM DonViQuiDoi dvqd     	
    	join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on hh.ID = lh.ID_HangHoa		
    	where dvqd.Xoa = '0'
    		and dvqd.Xoa = '0'
			and dvqd.LaDonViChuan = 1
    		and hh.TheoDoi = 1
			and IIF(hh.LoaiHangHoa is not null, hh.LoaiHangHoa, iif(hh.LaHangHoa=0,2,1)) in (select Loaihang from @tblLoaiHang)
	end
	else
	begin
		select 	
			hh.ID,
    		dvqd.ID as ID_DonViQuiDoi,
			lh.ID as ID_LoHang,
    		dvqd.MaHangHoa,
			IIF(hh.LoaiHangHoa is not null, hh.LoaiHangHoa, iif(hh.LaHangHoa=0,2,1)) as LoaiHangHoa,
			case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang,
    		concat(hh.TenHangHoa , dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
    		hh.TenHangHoa,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		dvqd.TenDonViTinh,
			lh.MaLoHang,
			Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
			Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
			Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan    	
		FROM DonViQuiDoi dvqd     	
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on hh.ID = lh.ID_HangHoa	
		where hh.ID_NhomHang in (select * from splitstring(@IDNhomHangs))
    		and dvqd.Xoa = '0'
			and dvqd.LaDonViChuan = 1
    		and hh.TheoDoi = 1
			and IIF(hh.LoaiHangHoa is not null, hh.LoaiHangHoa, iif(hh.LaHangHoa=0,2,1)) in (select Loaihang from @tblLoaiHang)
	end
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListHoaDon_ChuaPhanBoCK]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
    @TextSearch [nvarchar](max),
    @LoaiChungTus [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    	set @DateTo = dateadd(day,1, @DateTo) 
    
    		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');
    
    
    		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    		DECLARE @count int;
    		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    		Select @count =  (Select count(*) from @tblSearchString);
    		
    
    	declare @tblHoaDon table (ID uniqueidentifier, ID_DonVi uniqueidentifier, LoaiHoaDon int,
    			MaHoaDon  nvarchar(max), NgayLapHoaDon datetime,
    			IDSoQuy uniqueidentifier, MaPhieuThu nvarchar(max), NgayLapPhieuThu datetime,
    			MaDoiTuong nvarchar(max), TenDoiTuong nvarchar(max), DienThoai nvarchar(max),			
    		DoanhThu float, ThucThu float)
    		insert into @tblHoaDon
    			select hd.ID, 
    				hd.ID_DonVi,
    				hd.LoaiHoaDon,
    				hd.MaHoaDon,
    				hd.NgayLapHoaDon, 			
    				sq.ID as IDSoQuy,
    				sq.MaHoaDon as MaPhieuThu,
    				sq.NgayLapHoaDon as NgayLapPhieuThu,				
    				dt.MaDoiTuong,
    				dt.TenDoiTuong,
    				dt.DienThoai,	
    				case hd.LoaiHoaDon
    					when 6 then - hd.PhaiThanhToan
    					when 32 then - hd.PhaiThanhToan
    				else hd.PhaiThanhToan end as PhaiThanhToan,				
    				isnull(ThucThu,0) as ThucThu
    			from BH_HoaDon hd
    			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			left join (
    				select qct.ID_HoaDonLienQuan, 
    						qhd.ID,
    						qhd.MaHoaDon,
    						 qhd.NgayLapHoaDon,
    					SUM(iif(qhd.LoaiHoaDon=11, qct.TienThu,- qct.TienThu)) as ThucThu
    			from Quy_HoaDon_ChiTiet qct
    			join  Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    				where (qhd.TrangThai is null or qhd.TrangThai = '1')
    				and qhd.ID_DonVi in (select ID from @tblChiNhanh)				
    				and (qct.HinhThucThanhToan is null or qct.HinhThucThanhToan != 4)
    				group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID,qhd.MaHoaDon
    			) sq on hd.ID= sq.ID_HoaDonLienQuan
    			where hd.ChoThanhToan='0'
    			and hd.NgayLapHoaDon between @DateFrom and @DateTo	
				and hd.PhaiThanhToan > 0
    			and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    			and hd.LoaiHoaDon in (select LoaiChungTu from @tblChungTu)
    			and not exists
    				(select ID_HoaDon 
    				from BH_NhanVienThucHien th 
    				where hd.ID= th.ID_HoaDon and th.TienChietKhau > 0)
    			and
    				((select count(Name) from @tblSearchString b where     			
    				dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or dt.DienThoai like '%'+b.Name+'%'			
    					or hd.MaHoaDon like '%'+b.Name+'%'
    				)=@count or @count=0)	
    	
    		declare @tongDoanhThu float
    		select @tongDoanhThu = sum(hd.DoanhThu)
    		from (select distinct hd.ID,  hd.DoanhThu
    		from @tblHoaDon hd) hd		
    	
    			
    		;with data_cte
    		as(
    		select * from @tblHoaDon
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(ThucThu) as TongThucThu,
    					@tongDoanhThu as TongDoanhThu
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetMaDoiTuongMax_byTemp]
	@LoaiHoaDon int,
	@ID_DonVi uniqueidentifier
AS
BEGIN	
	SET NOCOUNT ON;

	declare @LoaiDoiTuong int
	if @LoaiHoaDon = 33 set @LoaiDoiTuong = 1
	if @LoaiHoaDon = 34 set @LoaiDoiTuong = 2


	DECLARE @Return float = 1
	declare @lenMaMax int = 0
	DECLARE @isDefault bit = (select top 1 SuDungMaChungTu from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi)-- co/khong thiet lap su dung Ma MacDinh
	DECLARE @isSetup int = (select top 1 ID_LoaiChungTu from HT_MaChungTu where ID_LoaiChungTu = @LoaiHoaDon)-- da ton tai trong bang thiet lap chua

	if @isDefault='1' and @isSetup is not null
		begin
			DECLARE @machinhanh varchar(15) = (select MaDonVi from DM_DonVi where ID= @ID_DonVi)
			DECLARE @lenMaCN int = Len(@machinhanh)

			DECLARE @isUseMaChiNhanh varchar(15), @kituphancach1 varchar(1),  @kituphancach2 varchar(1),  @kituphancach3 varchar(1),
			 @dinhdangngay varchar(8), @dodaiSTT INT, @kihieuchungtu varchar(10)

			 select @isUseMaChiNhanh = SuDungMaDonVi, 
				@kituphancach1= KiTuNganCach1,
				@kituphancach2 = KiTuNganCach2,
				@kituphancach3= KiTuNganCach3,
				@dinhdangngay = NgayThangNam,
				@dodaiSTT = CAST(DoDaiSTT AS INT),
				@kihieuchungtu = MaLoaiChungTu
			 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon 

		
			DECLARE @lenMaKiHieu int = Len(@kihieuchungtu);
			DECLARE @namthangngay varchar(10) = convert(varchar(10), getDate(), 112) ---- get ngayhientai
			DECLARE @year varchar(4) = Left(@namthangngay,4)
			DECLARE @date varchar(4) = right(@namthangngay,2)
			DECLARE @month varchar(4) = substring(@namthangngay,5,2)
			DECLARE @datecompare varchar(10)='';
			
			if	@isUseMaChiNhanh='0'
				begin 
					set @machinhanh=''
					set @lenMaCN=0
				end

			if @dinhdangngay='ddMMyyyy'
				set @datecompare = CONCAT(@date,@month,@year)
			else	
				if @dinhdangngay='ddMMyy'
					set @datecompare = CONCAT(@date,@month,right(@year,2))
				else 
					if @dinhdangngay='MMyyyy'
						set @datecompare = CONCAT(@month,@year)
					else	
						if @dinhdangngay='MMyy'
							set @datecompare = CONCAT(@month,right(@year,2))
						else
							if @dinhdangngay='yyyyMMdd'
								set @datecompare = CONCAT(@year,@month,@date)
							else 
								if @dinhdangngay='yyMMdd'
									set @datecompare = CONCAT(right(@year,2),@month,@date)
								else	
									if @dinhdangngay='yyyyMM'
										set @datecompare = CONCAT(@year,@month)
									else	
										if @dinhdangngay='yyMM'
											set @datecompare = CONCAT(right(@year,2),@month)
										else 
											if @dinhdangngay='yyyy'
												set @datecompare = @year	
												
			if @LoaiDoiTuong= 1 set @kihieuchungtu ='' --- không lấy kí tự theo mã khách (chỉ lấy mã chi nhánh) !!important with kangjjin

			DECLARE @sMaFull varchar(50) = concat(@machinhanh,@kituphancach1,@kihieuchungtu,@kituphancach2, @datecompare, @kituphancach3)	

			declare @sCompare varchar(30) = @sMaFull
			if @sMaFull= concat(@kihieuchungtu,'_') set @sCompare = concat(@kihieuchungtu,'[_]') -- like %_% không nhận kí tự _ nên phải [_] theo quy tắc của sql				


			SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaDoiTuong) AS float))
    		FROM DM_DoiTuong 
			WHERE LoaiDoiTuong = @LoaiDoiTuong 
			and MaDoiTuong like @sCompare +'%'  AND MaDoiTuong not like  @sCompare +'00%' -- not like 'KH00%'


			-- lay chuoi 000
			declare @stt int =0;
			declare @strstt varchar (10) ='0'
			while @stt < @dodaiSTT- 1
				begin
					set @strstt= CONCAT('0',@strstt)
					SET @stt = @stt +1;
				end 
			declare @lenSst int = len(@strstt)
			if	@Return is null 
					select CONCAT(@sMaFull,left(@strstt,@lenSst-1),1) as MaxCode-- left(@strstt,@lenSst-1): bỏ bớt 1 số 0			
			else 
				begin
					set @Return =  @Return + 1
					set @lenMaMax =  len(@Return)

					-- neu @Return là 1 số quá lớn --> mã bị chuyển về dạng e+10
					declare @madai nvarchar(max)= CONCAT(@sMaFull, CONVERT(numeric(22,0), @Return))
					select 
						case @lenMaMax							
							when 1 then CONCAT(@sMaFull,left(@strstt,@lenSst-1),@Return)
							when 2 then case when @lenSst - 2 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-2), @Return) else @madai end
							when 3 then case when @lenSst - 3 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-3), @Return) else @madai end
							when 4 then case when @lenSst - 4 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-4), @Return) else @madai end
							when 5 then case when @lenSst - 5 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-5), @Return) else @madai end
							when 6 then case when @lenSst - 6 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-6), @Return) else @madai end
							when 7 then case when @lenSst - 7 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-7), @Return) else @madai end
							when 8 then case when @lenSst - 8 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-8), @Return) else @madai end
							when 9 then case when @lenSst - 9 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-9), @Return) else @madai end
							when 10 then case when @lenSst - 10 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return) else @madai end
						else 
							case when  @lenMaMax > 10
								 then iif(@lenSst - 10 > -1, CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return),  @madai)
								 else '' end
						end as MaxCode					
				end 
		end
	else
		begin
			declare @machungtu varchar(10) = (select top 1 MaLoaiChungTu from DM_LoaiChungTu where ID= @LoaiHoaDon)			
			declare @lenMaChungTu int= LEN(@machungtu)
			
			
				declare @maOffline nvarchar(max) =''
				if @LoaiDoiTuong= 1 set @maOffline='KHO'
				if @LoaiDoiTuong= 2 set @maOffline='NCCO'				
				
				SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaDoiTuong) AS float))
    			FROM DM_DoiTuong 
				WHERE LoaiDoiTuong = @LoaiDoiTuong 
				and MaDoiTuong like @machungtu +'%'  AND MaDoiTuong not like @maOffline + '%'	

				
			-- do dai STT (toida = 9)
			if	@Return is null set @Return = 1				
			else set @Return = @Return + 1
																							
				set @lenMaMax = len(@Return)
				declare @max int =0
				declare @str0 nvarchar(10) =''
				while @max < 9 - (@lenMaMax + @lenMaChungTu)
					begin
						set @str0+='0'
						set @max+=1
					end					
				select CONCAT(@machungtu,@str0, CAST(@Return  as decimal(22,0)))  as MaxCode
		end
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetNhatKyGiaoDich_ofKhachHang]
    @IDChiNhanhs [nvarchar](max),
    @ID_KhachHang [nvarchar](max),
    @LoaiChungTu [nvarchar](10),
    @TextSearch [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs);

		declare @tblThuDH table (ID uniqueidentifier,
				TienMat float, 
				TienATM float, 
				ChuyenKhoan float, 
				TienDoiDiem float,  
				ThuTuThe float,  									
				TienDatCoc float, 
				TienThu	float)
		insert into @tblThuDH
		exec GetTongThu_fromHDDatHang @IDChiNhanhs,@ID_KhachHang,@FromDate, @ToDate;

    
    	with data_cte
    	as(
    	select hd.ID, hd.LoaiHoaDon, hd.MaHoaDon, hd.NgayLapHoaDon, hd.NgayApDungGoiDV, hd.HanSuDungGoiDV, 
    		iif(hd.LoaiHoaDon=22, hd.TongChiPhi, hd.TongTienHang) as TongTienHang,
    		hd.TongTienThue, hd.TongGiamGia, hd.TongChiPhi, hd.TongChietKhau, hd.PhaiThanhToan, 
    		dv.TenDonVi,
    		TienMat, TienGui, ThuTuThe, TienThu as DaThanhToan,
    		case  hd.LoaiHoaDon
    			when 1 then N'Hóa đơn bán lẻ'
    			when 19 then N'Gói dịch vụ'
    			when 22 then N'Thẻ giá trị'
				when 25 then N'Hóa đơn sửa chữa'
    		end as SLoaiHoaDon
    	from BH_HoaDon hd
    	join DM_DonVi dv on hd.ID_DonVi= dv.ID
    	left join (
			
			select 
				thuchi.ID_HoaDonLienQuan,
				sum(TienMat) as TienMat,
				sum(TienGui) as TienGui,
				sum(ThuTuThe) as ThuTuThe,
				sum(TienThu) as TienThu
			from
			(				
				--- thongthu of hoadon
    			select qct.ID_HoaDonLienQuan, 
    				sum(qct.TienMat) as TienMat, 
    				sum(qct.TienGui) as TienGui, 
    				sum(qct.ThuTuThe) as ThuTuThe,
    				sum(iif(qhd.LoaiHoaDon= 11, qct.TienThu, -qct.TienThu)) as TienThu
    			from Quy_HoaDon_ChiTiet qct
    			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			where qhd.TrangThai = 1			
				and qct.ID_DoiTuong= @ID_KhachHang
    			group by qct.ID_HoaDonLienQuan

				union all

				---- tongthu from dathang
				select 
					ID,
					TienMat,
					TienATM,
					ThuTuThe,
					TienThu
				from @tblThuDH		
				) thuchi group by thuchi.ID_HoaDonLienQuan
    		) quy on quy.ID_HoaDonLienQuan = hd.ID
    	where hd.ID_DoiTuong= @ID_KhachHang
    	and exists (select Name from dbo.splitstring(@LoaiChungTu) ct where hd.LoaiHoaDon = ct.Name )
    	and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)
    	and hd.NgayLapHoaDon between  @FromDate and  @ToDate
    ),
    count_cte
    as(
    	select COUNT(ID) as TotalRow,
    		 CEILING(COUNT(ID)/ CAST(@PageSize as float)) as TotalPage,
    		 sum(TongTienHang) as SumTienHang,
    		 sum(TongTienThue) as SumTienThue,
    		 sum(TongGiamGia) as SumGiamGia,
    		 sum(PhaiThanhToan) as SumPhaiThanhToan,
    		 sum(TongChiPhi) as SumChiPhi,
    		 sum(TongChietKhau) as SumChietKhau,
    		 sum(TienMat) as SumTienMat,
    		 sum(TienGui) as SumTienGui,
    		 sum(ThuTuThe) as SumThuTuThe,
    		 sum(DaThanhToan) as SumDaThanhToan
    	from data_cte
    )
    select *
    from data_cte dt
    cross join count_cte cte
    order by dt.NgayLapHoaDon desc
    offset (@CurrentPage * @PageSize) rows
    fetch next @PageSize rows only
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetSoDuTheGiaTri_ofKhachHang]
    @ID_DoiTuong [uniqueidentifier],
    @DateTime [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	set @DateTime= DATEADD(DAY,1,@DateTime)
    	select 
    		TongThuTheGiaTri,
			TraLaiSoDu,
			SuDungThe, 
			HoanTraTheGiatri,
    		ThucThu,
			PhaiThanhToan,
			SoDuTheGiaTri,
    		iif(CongNoThe<0,0,CongNoThe) as CongNoThe
    	from
    	(
    	select 		
    		sum(TongThuTheGiaTri) - sum(TraLaiSoDu) as TongThuTheGiaTri, 
			sum(TraLaiSoDu) as TraLaiSoDu,
    		cast(sum(SuDungThe) as float) as SuDungThe,
    		cast(sum(HoanTraTheGiatri) as float) as HoanTraTheGiatri,
    		cast(sum(ThucThu) as float) as ThucThu,
    		cast(sum(PhaiThanhToan) as float) as PhaiThanhToan,
    		cast(SUM(ThucThu) + sum(TongDieuChinh) - sum(TraLaiSoDu)  - SUM(SuDungThe) + SUM(HoanTraTheGiatri) as float) as SoDuTheGiaTri, --- kangjin: soddu = tongthuthucte - sudung + hoantra
    		cast(sum(PhaiThanhToan) - sum(TraLaiSoDu) - sum(ThucThu) as float) as CongNoThe
    	from (
		---- dieuchinh sodu (tang/giam)					
    		SELECT 
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu,
				SUM(TongChiPhi) as TongDieuChinh
    		FROM BH_HoaDon hd
    		where hd.ID_DoiTuong like @ID_DoiTuong and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 23
    		and hd.NgayLapHoaDon  < @DateTime

			union all
    		-- so du nap the va thuc te phai thanh toan
    		SELECT 
    			TongTienHang as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			hd.PhaiThanhToan, -- dieu chinh the (khong lien quan den cong no)
				0 as TraLaiSoDu,
				0 as TongDieuChinh
    		FROM BH_HoaDon hd
    		where hd.ID_DoiTuong like @ID_DoiTuong and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22
    		and hd.NgayLapHoaDon  < @DateTime
    
    		union all
    		-- su dung the
    		SELECT 
    			0 as TongThuTheGiaTri,
    			SUM(qct.TienThu) as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu,
				0 as TongDieuChinh
    		FROM Quy_HoaDon_ChiTiet qct
    		INNER JOIN Quy_HoaDon qhd
    		ON qct.ID_HoaDon = qhd.ID
    		WHERE qct.ID_DoiTuong like @ID_DoiTuong AND qhd.NgayLapHoaDon  < @DateTime and qhd.LoaiHoaDon = 11 
    		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    		and qct.HinhThucThanhToan=4

			
    		union all
    		---- hoàn trả số dư còn trong TGT cho khách --> giảm số dư
    		SELECT
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				SUM(hd.TongTienHang) as TraLaiSoDu,
				0 as TongDieuChinh
    		FROM BH_HoaDon hd
    		where hd.LoaiHoaDon= 32 and hd.ChoThanhToan= 0
			and hd.ID_DoiTuong like @ID_DoiTuong
    	
    		union all
    		-- hoàn trả tiền vào TGT ---> tăng số dư
    		SELECT
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			SUM(qct.TienThu) as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu,
				0 as TongDieuChinh
    		FROM Quy_HoaDon_ChiTiet qct
    		INNER JOIN Quy_HoaDon qhd
    		ON qct.ID_HoaDon = qhd.ID
    		WHERE qct.ID_DoiTuong like @ID_DoiTuong AND qhd.NgayLapHoaDon  < @DateTime and qhd.LoaiHoaDon = 12
    		and (qhd.TrangThai = 1 or qhd.TrangThai is null)
    			and qct.HinhThucThanhToan=4
    
    		union all
    		-- thuc thu thegiatri
    		SELECT
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			qct.TienThu as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu,
				0 as TongDieuChinh
    		from Quy_HoaDon_ChiTiet qct
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    		join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    		where hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and qhd.NgayLapHoaDon < @DateTime and qct.ID_DoiTuong like @ID_DoiTuong
    		and (qhd.PhieuDieuChinhCongNo= 0 or PhieuDieuChinhCongNo  is  null)
			and (qhd.TrangThai = 1 or qhd.TrangThai is null)

    		-- thucthu do dieuchinh congno khachhang
    		union all
    		select
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			qct.TienThu as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu,
				0 as TongDieuChinh
    		from Quy_HoaDon_ChiTiet qct
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    		where qhd.PhieuDieuChinhCongNo= 1 and qhd.LoaiHoaDon= 11
    		and (qhd.TrangThai= 1 or qhd.TrangThai is null)
    		and qct.ID_DoiTuong like @ID_DoiTuong
    		) tbl  
    		) tbl2
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetTonKho_byIDQuyDois]
    @ID_ChiNhanh [uniqueidentifier],
    @ToDate [datetime],
    @IDDonViQuyDois [nvarchar](max),
    @IDLoHangs [nvarchar](max)
AS
BEGIN
	 SET NOCOUNT ON;
    	declare @tblIDQuiDoi table (ID_DonViQuyDoi uniqueidentifier)
    	declare @tblIDLoHang table (ID_LoHang uniqueidentifier)
    
    	insert into @tblIDQuiDoi
    	select Name from dbo.splitstring(@IDDonViQuyDois) 
    	insert into @tblIDLoHang
    	select Name from dbo.splitstring(@IDLoHangs) where Name not like '%null%' and Name !=''

		
		---- get tonluyke theo thoigian 
		---- get tonluyke theo thoigian 
		SELECT 
    		ID_DonViQuiDoi,
    		ID_HangHoa, 		
    		ID_LoHang,		
    		IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, TonLuyKe_NhanChuyenHang, TonLuyKe) AS TonKho, 
    		IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, GiaVon_NhanChuyenHang, GiaVon) AS GiaVon
		into #tblTon
    	FROM (
    		SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang, 
			tbltemp.ID_DonViInput ORDER BY tbltemp.ThoiGian DESC) AS RN 
		
    	FROM (
				select 
						hd.LoaiHoaDon, 
						hd.ID_DonVi,
						qd.ID_HangHoa,
						ct.ID_DonViQuiDoi,
						hd.ID_CheckIn, 					
						@ID_ChiNhanh as ID_DonViInput, 
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
						ct.TonLuyKe_NhanChuyenHang, ct.TonLuyKe) AS TonLuyKe,
    					ct.TonLuyKe_NhanChuyenHang,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh, 
    					ct.GiaVon_NhanChuyenHang, 
    					ct.GiaVon)/ISNULL(qd.TyLeChuyenDoi,1) AS GiaVon,
    					ct.GiaVon_NhanChuyenHang, 
    					ct.ID_LoHang ,
    					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = @ID_ChiNhanh,
						hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
				
				from 
					(
						--- get all dvquydoi by idhanghoa 
						select qdOut.ID, qdOut.TyLeChuyenDoi,  qdOut.ID_HangHoa
						from DonViQuiDoi qdOut
						where exists (
							select dvqd.ID_HangHoa
							from @tblIDQuiDoi qd
							join DonViQuiDoi dvqd on qd.ID_DonViQuyDoi= dvqd.ID
							where qdOut.ID_HangHoa = dvqd.ID_HangHoa
							)
					) qd
				join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
				join BH_HoaDon_ChiTiet ct on qd.ID = ct.ID_DonViQuiDoi
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID				
				where (hd.ID_DonVi= @ID_ChiNhanh or (hd.ID_CheckIn = @ID_ChiNhanh and hd.YeuCau = '4'))
				and hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10,18)
				and (exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= ct.ID_LoHang) Or hh.QuanLyTheoLoHang= 0)    
			) as tbltemp
    	WHERE tbltemp.ThoiGian < @ToDate) tblTonKhoTemp
    	WHERE tblTonKhoTemp.RN = 1;

		---- get giavon tieuchuan theo thoigian
		declare @tblGVTieuChuan table(ID_DonVi uniqueidentifier,
							ID_DonViQuiDoi uniqueidentifier,
							ID_LoHang uniqueidentifier, 
							GiaVonTieuChuan	float)
		insert into @tblGVTieuChuan
		exec GetGiaVonTieuChuan_byTime @ID_ChiNhanh, @ToDate, @IDDonViQuyDois, @IDLoHangs 

	
	select TenHangHoa, 
			lo.MaLoHang,
			qd2.ID_HangHoa,
			qd.ID_DonViQuyDoi as ID_DonViQuiDoi,
			qd2.GiaNhap,
			lo.ID as ID_LoHang, 
			qd2.TyLeChuyenDoi,
			qd2.TenDonViTinh,
			isnull(gvtc.GiaVonTieuChuan,0) as GiaVonTieuChuan,
			isnull(tk.TonKho,0)/ iif(qd2.TyLeChuyenDoi=0 or qd2.TyLeChuyenDoi is null,1, qd2.TyLeChuyenDoi) as TonKho,
			isnull(tk.GiaVon,0) * iif(qd2.TyLeChuyenDoi=0 or qd2.TyLeChuyenDoi is null,1, qd2.TyLeChuyenDoi) as GiaVon
		from @tblIDQuiDoi qd 	
		join DonViQuiDoi qd2 on qd.ID_DonViQuyDoi= qd2.ID 
		join DM_HangHoa hh on hh.ID = qd2.ID_HangHoa
		left join DM_LoHang lo on hh.ID = lo.ID_HangHoa and hh.QuanLyTheoLoHang = 1   
		left join @tblGVTieuChuan gvtc on qd.ID_DonViQuyDoi = gvtc.ID_DonViQuiDoi and (lo.ID = gvtc.ID_LoHang or (gvtc.ID_LoHang is null and lo.ID is null) )
		left join #tblTon tk on hh.ID = tk.ID_HangHoa 
		and (tk.ID_LoHang = lo.ID or hh.QuanLyTheoLoHang =0)
		where (exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= lo.ID) Or hh.QuanLyTheoLoHang= 0)
		order by qd.ID_DonViQuyDoi, lo.ID

		
END");

			Sql(@"ALTER PROCEDURE [dbo].[HDSC_GetChiTietXuatKho]
    @ID_HoaDon [uniqueidentifier],
    @IDChiTietHD [uniqueidentifier],
    @LoaiHang [int]
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
    			join DonViQuiDoi qd on qd.ID= tpdl.ID_DonViQuiDoi
    			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
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

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountInvoice]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs [nvarchar](max),
    @TextSearch [nvarchar](max),
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
    	set @DateTo = dateadd(day,1, @DateTo) 

		declare @tblChiNhanh table (ID uniqueidentifier)
    	insert into @tblChiNhanh
    	select * from dbo.splitstring(@ID_ChiNhanhs)
    
    	declare @nguoitao nvarchar(100) = (select top 1 taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
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
    
    	declare @tblDiscountInvoice table (ID uniqueidentifier, MaNhanVien nvarchar(50), TenNhanVien nvarchar(max), 
    		HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float)
    	
    	-- bang tam chua DS phieu thu theo Ngay timkiem
    	select qct.ID_HoaDonLienQuan, SUM(qct.TienThu) as ThucThu, qhd.NgayLapHoaDon, qhd.ID
    	into #temp
    	from Quy_HoaDon_ChiTiet qct
    	join (
    			select qhd.ID, qhd.NgayLapHoaDon
    			from Quy_HoaDon qhd
    			join BH_NhanVienThucHien th on qhd.ID= th.ID_QuyHoaDon
    			where (qhd.TrangThai is null or qhd.TrangThai = '1')
    			and qhd.ID_DonVi in (select ID from @tblChiNhanh)
    			and qhd.NgayLapHoaDon >= @DateFrom
    			and qhd.NgayLapHoaDon < @DateTo
    			group by qhd.ID, qhd.NgayLapHoaDon) qhd on qct.ID_HoaDon = qhd.ID
    	where (qct.HinhThucThanhToan is null or qct.HinhThucThanhToan != 4)
    	group by qct.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID;
    	
    	select ID, MaNhanVien, 
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
    		into #temp2
    	from 
    	(
    		select nv.ID, MaNhanVien, TenNhanVien,
    			case when TinhChietKhauTheo =1 then case when hd.LoaiHoaDon in (6,32) then -TienChietKhau else TienChietKhau  end end as HoaHongThucThu,
    				case when TinhChietKhauTheo =3 then case when hd.LoaiHoaDon in (6,32) then -TienChietKhau else TienChietKhau end end as HoaHongVND,
    				-- neu HD tao thang truoc, nhung PhieuThu thuoc thang nay: HoaHongDoanhThu = 0
    				case when hd.NgayLapHoaDon between @DateFrom and @DateTo and hd.ID_DonVi in (select ID from @tblChiNhanh) then
    					case when TinhChietKhauTheo = 2 then case when hd.LoaiHoaDon = 6 then -TienChietKhau else TienChietKhau end end else 0 end as HoaHongDoanhThu,
    				-- timkiem theo NgayLapHD or NgayLapPhieuThu
    				case when @DateFrom <= hd.NgayLapHoaDon and  hd.NgayLapHoaDon < @DateTo then hd.NgayLapHoaDon else tblQuy.NgayLapHoaDon end as NgayLapHoaDon,
    				case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    		from BH_NhanVienThucHien th
    		join BH_HoaDon hd on th.ID_HoaDon= hd.ID
    		join NS_NhanVien nv on th.ID_NhanVien= nv.ID
    			left join #temp tblQuy on hd.ID= tblQuy.ID_HoaDonLienQuan and (th.ID_QuyHoaDon= tblQuy.ID)	
    		where th.ID_HoaDon is not null
    		and hd.ChoThanhToan  is not null
    		and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))  
				and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    			and (@DepartmentIDs ='' or exists (select ID from @tblNhanVien nv where th.ID_NhanVien = nv.ID))
    			-- chi lay CKDoanhThu hoac CKThucThu/VND exist in Quy_HoaDon or (not exist QuyHoaDon but LoaiHoaDon =6 )
    			and (th.TinhChietKhauTheo != 1 or (th.TinhChietKhauTheo =1 and ( exists (select ID from #temp where th.ID_QuyHoaDon = #temp.ID) or  LoaiHoaDon=6)))
    			and
    				((select count(Name) from @tblSearchString b where     			
    				nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'				
    				)=@count or @count=0)	
    	) tbl
    		where tbl.NgayLapHoaDon >= @DateFrom and tbl.NgayLapHoaDon < @DateTo and TrangThaiHD = @StatusInvoice
    	group by MaNhanVien, TenNhanVien, ID
    		having SUM(ISNULL(HoaHongThucThu,0)) + SUM(ISNULL(HoaHongDoanhThu,0)) + SUM(ISNULL(HoaHongVND,0)) > 0 -- chi lay NV co CK > 0
    		
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
    						from #temp2 where HoaHongDoanhThu > 0 or HoaHongVND != 0
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
    									from #temp2 where  HoaHongThucThu != 0
    								else -- 6
    									insert into @tblDiscountInvoice
    									select *
    									from #temp2  where HoaHongVND > 0
    								
    			end;
    			
    		with data_cte
    		as(
    		select * from @tblDiscountInvoice
    		),
    		count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
    				sum(HoaHongThucThu) as TongHoaHongThucThu,
    				sum(HoaHongVND) as TongHoaHongVND,
    				sum(TongAll) as TongAllAll
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaNhanVien
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateGiaVon_WhenEditCTHD]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier],
    @NgayLapHDMin [datetime]
AS
BEGIN
    SET NOCOUNT ON;	
	

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
    		DECLARE @cthd_NeedUpGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT,TongChiPhi FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT,  TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @cthd_NeedUpGiaVon
   
	select hd.ID as IDHoaDon,
			hd.ID_HoaDon, 
			hd.MaHoaDon, 
			hd.LoaiHoaDon,
			ct.ID as ID_ChiTietHoaDon, 
    		CASE WHEN hd.YeuCau = '4' AND @IDChiNhanh = hd.ID_CheckIn THEN hd.NgaySua ELSE hd.NgayLapHoaDon END AS NgayLapHoaDon, 				    			    				    							    			
    		ct.SoThuTu,
			iif(ct.ChatLieu='5',0,ct.SoLuong) as SoLuong, 
			ct.DonGia, 
			hd.TongTienHang, 
			isnull(hd.TongChiPhi,0) as TongChiPhi,
			ct.TienChietKhau,
			ct.ThanhTien, 
			hd.TongGiamGia, 
			qd.TyLeChuyenDoi,
			[dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanh, hh.ID, ct.ID_LoHang, 
			CASE WHEN hd.YeuCau = '4' AND @IDChiNhanh = hd.ID_CheckIn THEN hd.NgaySua ELSE hd.NgayLapHoaDon END) as TonKho, 	    	 	    	
    		ct.GiaVon / qd.TyLeChuyenDoi as GiaVon, 
    		ct.GiaVon_NhanChuyenHang / qd.TyLeChuyenDoi as GiaVonNhan,
    		hh.ID as ID_HangHoa, 
			hh.LaHangHoa, 
			qd.ID as IDDonViQuiDoi, 
			ct.ID_LoHang, 
			ct.ID_ChiTietDinhLuong, 
    		@IDChiNhanh as IDChiNhanh,
			hd.ID_CheckIn,
			hd.YeuCau 
    	FROM BH_HoaDon_ChiTiet ct 
    	INNER JOIN DonViQuiDoi qd ON ct.ID_DonViQuiDoi = qd.ID   	
    	INNER JOIN DM_HangHoa hh on hh.ID = qd.ID_HangHoa   	
    	INNER JOIN BH_HoaDon hd  ON ct.ID_HoaDon = hd.ID   
    	WHERE hd.ChoThanhToan = 0    	
			and hd.LoaiHoaDon NOT IN (3, 19, 25,29)
			---- dont join ctEdit because douple row (only check exists ID_QuiDoi, ID_Lo)
			and exists (select ID_HangHoa from @tblCTHD ctNew where ctNew.ID_HangHoa = qd.ID_HangHoa  and (ct.ID_LoHang = ctNew.ID_LoHang OR ctNew.ID_LoHang IS NULL))
    			AND
    			((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon >= @NgayLapHDMin
    				and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    				or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua >= @NgayLapHDMin))
    
    		--Begin TinhGiaVonTrungBinh
    		DECLARE @TinhGiaVonTrungBinh BIT;
    		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanh;
    		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
    		BEGIN
    			-- get GiaVon DauKy by ID_QuiDoi
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT, TongChiPhi float,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select
    				hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu, 
					iif(hdct.ChatLieu='5',0, hdct.SoLuong) as SoLuong, 
					hdct.DonGia, 
					hd.TongTienHang, isnull(hd.TongChiPhi,0) as TongChiPhi,
    				iif(hdct.ChatLieu='5',0,hdct.TienChietKhau) as TienChietKhau, 
					hdct.ThanhTien, hd.TongGiamGia, 
    				dvqd.TyLeChuyenDoi, 0, hdct.GiaVon / dvqd.TyLeChuyenDoi as GiaVon, 
    				hdct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi as GiaVonNhan, 
    				hh.ID, hh.LaHangHoa, hdct.ID_DonViQuiDoi, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, 
    				@IDChiNhanh, hd.ID_CheckIn, hd.YeuCau 
			FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct 	ON hd.ID = hdct.ID_HoaDon    	
    		INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID    		
    		INNER JOIN DM_HangHoa hh on hh.ID = dvqd.ID_HangHoa    		
    		INNER JOIN @tblCTHD cthdthemmoi ON cthdthemmoi.ID_HangHoa = hh.ID    		
    		WHERE hd.ChoThanhToan = 0 
				and hd.LoaiHoaDon NOT IN (3, 19, 25,29)
    				AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) 
    				AND
    				((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon < @NgayLapHDMin and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    					or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua < @NgayLapHDMin))

    			
    			--select * from @ChiTietHoaDonGiaVon order by NgayLapHoaDon
    			--select * from @cthd_NeedUpGiaVon order by NgayLapHoaDon
    
		DECLARE @ChiTietHoaDonGiaVon1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT, TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
			
			INSERT INTO @ChiTietHoaDonGiaVon1
			SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    					FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon1 WHERE cthdGiaVon1.RN = 1;

    			-- assign again GiaVon to cthd was edit by ID_HangHoa
    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT, 
				TongTienHang FLOAT,TongChiPhi FLOAT,
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
    					cthdGiaVon.SoThuThu, cthdGiaVon.SoLuong, cthdGiaVon.DonGia, cthdGiaVon.TongTienHang,cthdGiaVon.TongChiPhi,
    				cthdGiaVon.ChietKhau, cthdGiaVon.ThanhTien, cthdGiaVon.TongGiamGia, cthdGiaVon.TyLeChuyenDoi, cthdGiaVon.TonKho, 
    					CASE WHEN cthdGiaVon.GiaVon IS NULL THEN 0 ELSE cthdGiaVon.GiaVon END AS GiaVon, 											
    					CASE WHEN cthdGiaVon.GiaVonNhan IS NULL THEN 0 ELSE cthdGiaVon.GiaVonNhan END AS GiaVonNhan,								
    					cthd1.ID_HangHoa, cthdGiaVon.LaHangHoa, cthdGiaVon.IDDonViQuiDoi, cthd1.ID_LoHang , cthdGiaVon.ID_ChiTietDinhLuong,
    				@IDChiNhanh, cthdGiaVon.ID_CheckIn, cthdGiaVon.YeuCau 
    				FROM @tblCTHD cthd1
    				LEFT JOIN @ChiTietHoaDonGiaVon1 AS cthdGiaVon   					
    				ON cthd1.ID_HangHoa = cthdGiaVon.ID_HangHoa 
    				AND (cthd1.ID_LoHang = cthdGiaVon.ID_LoHang OR cthdGiaVon.ID_LoHang IS NULL)) AS tableUpdateGiaVon;
    		--select * from @BangUpdateGiaVon order by NgayLapHoaDon
    
    			-- caculator again GiaVon by ID_HangHoa
    			DECLARE @TableTrungGianUpDate TABLE(IDHoaDon UNIQUEIDENTIFIER,IDHangHoa UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, GiaVonNhapHang FLOAT, GiaVonNhanHang FLOAT)
    			INSERT INTO @TableTrungGianUpDate
    			SELECT 
    				IDHoaDon, ID_HangHoa, ID_LoHang, 
					sum(GiaVon) as GiaVonNhapHang,
					sum(GiaVonNhan) as GiaVonNhanHang
    			FROM @BangUpdateGiaVon
    			WHERE IDHoaDon = @IDHoaDonInput and RN= 1
    			GROUP BY ID_HangHoa, ID_LoHang, IDHoaDon,LoaiHoaDon
    			
    			--select * from @TableTrungGianUpDate 


				update gv set gv.RN = 2
				 from @BangUpdateGiaVon gv
				 join 
						(select  MAX(RN) as RN, ID_HangHoa, ID_LoHang FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang 
				) tbl on gv.ID_HangHoa = tbl.ID_HangHoa and (gv.ID_LoHang = tbl.ID_LoHang or gv.ID_LoHang is null and tbl.ID_LoHang is null)
				 where tbl.RN= 1
    
    			-- update GiaVon, GiaVonNhan to @BangUpdateGiaVon if Loai =(4,10), else keep old
    		UPDATE bhctup 
    			SET bhctup.GiaVon = 
    			CASE WHEN bhctup.LoaiHoaDon in (4,13) THEN giavontbup.GiaVonNhapHang	    						
    			ELSE bhctup.GiaVon END,    		
    			bhctup.GiaVonNhan = 
    				CASE WHEN bhctup.LoaiHoaDon = 10 AND bhctup.YeuCau = '4' AND bhctup.ID_CheckIn = ID_ChiNhanhThemMoi THEN giavontbup.GiaVonNhanHang   		    			
    			ELSE bhctup.GiaVonNhan END  		
    			FROM @BangUpdateGiaVon bhctup
    			JOIN @TableTrungGianUpDate giavontbup on bhctup.IDHoaDon =giavontbup.IDHoaDon and bhctup.ID_HangHoa = giavontbup.IDHangHoa and (bhctup.ID_LoHang = giavontbup.IDLoHang or giavontbup.IDLoHang is null)
    		WHERE bhctup.IDHoaDon = @IDHoaDonInput AND bhctup.RN = 1;
    			--END tính giá vốn trung bình cho lần nhập hàng và chuyển hàng đầu tiền
    
    		DECLARE @GiaVonCapNhat TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, IDHoaDonCu UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, IDChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoLuong FLOAT, DonGia FLOAT, 
			TongTienHang FLOAT,TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, GiaVonCu FLOAT, IDHangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, IDLoHang UNIQUEIDENTIFIER, IDChiTietDinhLuong UNIQUEIDENTIFIER,
    		IDChiNhanhThemMoi UNIQUEIDENTIFIER, IDCheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @GiaVonCapNhat
    		SELECT 
    				tableUpdate.IDHoaDon, tableUpdate.IDHoaDonBan, tableGiaVon.IDHoaDon, tableUpdate.MaHoaDon, tableUpdate.LoaiHoaDon, tableUpdate.ID_ChiTietHoaDon,tableUpdate.NgayLapHoaDon, tableUpdate.SoLuong, tableUpdate.DonGia,
    			tableUpdate.TongTienHang, tableUpdate.TongChiPhi,
				tableUpdate.ChietKhau, tableUpdate.ThanhTien, tableUpdate.TongGiamGia, tableUpdate.TyLeChuyenDoi, tableUpdate.TonKho, tableUpdate.GiaVon, tableUpdate.GiaVonNhan, tableGiaVon.GiaVon, tableUpdate.ID_HangHoa, tableUpdate.LaHangHoa,
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
    		DECLARE @TongTienHang FLOAT, @TongChiPhi float;
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
    			SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, TongChiPhi, ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
    			GiaVonCu, IDHangHoa, IDDonViQuiDoi, IDLoHang, IDChiNhanhThemMoi, IDCheckIn, YeuCau, RN, GiaVon, GiaVonNhan 
    			FROM @GiaVonCapNhat WHERE RN > 1 and LaHangHoa = 1 ORDER BY IDHangHoa, RN
    		OPEN CS_GiaVon
    		FETCH FIRST FROM CS_GiaVon 
    			INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau,@ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
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
    				IF(@LoaiHoaDon in (4,13))
    				BEGIN
    					
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia -  bhctdm.ChietKhau)),  ---- cot thanhtien bhct
								@SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
    
    						--select @IDHoaDon, @MaHoaDon, @TongTienHangDemo, @SoLuongDemo, @TonKho
    					IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    					BEGIN
    						IF(@TongTienHang != 0)
    						BEGIN
								---- giavon: tinh sau khi tru giam gia hoadon + phi ship
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo ----  (@TongTienHangDemo* (1-(@TongGiamGia/@TongTienHang))) GiamGiaHoaDon - khong tinh vao GiaVon
									+ (@TongTienHangDemo* @TongChiPhi/@TongTienHang))/(@SoLuongDemo + @TonKho);
								
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
    							SET	@GiaVonMoi = (@TongTienHangDemo / @SoLuongDemo)  ---- * (1 - (@TongGiamGia / @TongTienHang))
								+ (@TongTienHangDemo *(@TongChiPhi / @TongTienHang));
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
    						
    					SELECT @TongTienHangDemo = 
							SUM(bhctdm.SoLuong * bhctdm.DonGia * ( 1- bhctdm.TongGiamGia/iif(bhctdm.TongTienHang=0,1,bhctdm.TongTienHang))) ,
						@SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
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
    				ELSE IF(@LoaiHoaDon = 18) ----phieu dieuchinh giavon
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
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon		
			
			update gv set GiaVonCu = isnull(GiaVonCu,0)
			from @GiaVonCapNhat gv 
    
    		--	select * from @GiaVonCapNhat
    		--Update BH_HoaDon_ChiTiet
    		UPDATE hoadonchitiet1
    		SET hoadonchitiet1.GiaVon = _giavoncapnhat1.GiaVon * _giavoncapnhat1.TyLeChuyenDoi,
    			hoadonchitiet1.GiaVon_NhanChuyenHang = _giavoncapnhat1.GiaVonNhan * _giavoncapnhat1.TyLeChuyenDoi
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet1
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat1 ON hoadonchitiet1.ID = _giavoncapnhat1.IDChiTietHoaDon   		
    		WHERE _giavoncapnhat1.LoaiHoaDon != 8 AND _giavoncapnhat1.LoaiHoaDon != 18 AND _giavoncapnhat1.LoaiHoaDon != 9 AND _giavoncapnhat1.RN > 1;
    
    		---- update GiaVon to phieu KiemKe
    			UPDATE hoadonchitiet4
    		SET hoadonchitiet4.GiaVon = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi, 
    			hoadonchitiet4.ThanhToan = _giavoncapnhat4.GiaVon * _giavoncapnhat4.TyLeChuyenDoi *hoadonchitiet4.SoLuong
    		FROM BH_HoaDon_ChiTiet AS hoadonchitiet4
    		INNER JOIN @GiaVonCapNhat AS _giavoncapnhat4 ON hoadonchitiet4.ID = _giavoncapnhat4.IDChiTietHoaDon    		
    		WHERE _giavoncapnhat4.LoaiHoaDon = 9 AND _giavoncapnhat4.RN > 1;
    
    			-- update GiaVon to phieu XuatKho
    		UPDATE hoadonchitiet2
    		SET hoadonchitiet2.GiaVon = _giavoncapnhat2.GiaVon * _giavoncapnhat2.TyLeChuyenDoi, 
				--hoadonchitiet2.DonGia = _giavoncapnhat2.GiaVon * hoadonchitiet2.SoLuong* _giavoncapnhat2.TyLeChuyenDoi,
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
    		----End Update DM_GiaVon

			delete from BH_HoaDon_ChiTiet where ID_HoaDon = @IDHoaDonInput and ChatLieu='5'
    		END
    		--END TinhGiaVonTrungBinh
END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateTonLuyKeCTHD_whenUpdate]
    @IDHoaDonInput [uniqueidentifier],
    @IDChiNhanhInput [uniqueidentifier],
    @NgayLapHDOld [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    
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
    					select LoaiHoaDon, YeuCau, ID_CheckIn, ID_DonVi, NgaySua, 
						case when @IDChiNhanhInput = ID_CheckIn and YeuCau !='1' then NgaySua else NgayLapHoaDon end as NgayLapHoaDon
    					from BH_HoaDon where ID = @IDHoaDonInput) a
    
    		-- alway get Ngay min --> compare to update TonLuyKe
    		IF(@NgayLapHDOld > @NgayLapHDNew)
    			SET @NgayLapHDMin = @NgayLapHDNew;
    		ELSE
    			SET @NgayLapHDMin = @NgayLapHDOld;

			declare @NgayLapHDMin_SubMiliSecond datetime = dateadd(MILLISECOND,-2, @NgayLapHDMin)
    
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
    		select NgayLapHoaDon, qd.ID_HangHoa, ct.ID_LoHang
    			from BH_HoaDon_ChiTiet ct 
    			join BH_HoaDon hd ON hd.ID = ct.ID_HoaDon		
    			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
    			WHERE hd.ChoThanhToan = 0
    			and hd.LoaiHoaDon= 9
    			and hd.ID_DonVi = @IDChiNhanhInput and NgayLapHoaDon > @NgayLapHDMin_SubMiliSecond
				and exists (select * from @tblChiTiet tblct where qd.ID_HangHoa = tblct.ID_HangHoa AND (ct.ID_LoHang = tblct.ID_LoHang OR ct.ID_LoHang IS NULL)	)
    			group by qd.ID_HangHoa, ct.ID_LoHang, hd.NgayLapHoaDon				
    		
    		-- get cthd liên quan (chi get from ngyahientai)
    		select
    			ct.ID, 
    			ct.ID_LoHang,
				case when hd.LoaiHoaDon in (1,2,3,19,31,36) or ct.ChatLieu in ('5','2') then 0 else ct.SoLuong end as SoLuong,
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
    		WHERE hd.ChoThanhToan = 0 
    		AND ((hd.ID_DonVi = @IDChiNhanhInput ) 
				OR (hd.ID_CheckIn = @IDChiNhanhInput AND hd.YeuCau = '4') )
			and exists (select * from @tblChiTiet ctupdate where qd.ID_HangHoa = ctupdate.ID_HangHoa AND (ct.ID_LoHang = ctupdate.ID_LoHang OR ct.ID_LoHang IS NULL))
		

    		-- table cthd has ID_HangHoa exist cthd kiemke
    		declare @cthdHasKiemKe table (ID_HangHoa uniqueidentifier, ID_LoHang uniqueidentifier)
    		declare @tblNgayKiemKe table (NgayKiemKe datetime)
    
    		declare @count float= (select count(*) from @tblHangKiemKe)
    			begin						
    				declare @ID_HangHoa uniqueidentifier, @ID_LoHang uniqueidentifier				
    				DECLARE Cur_tblKiemKe CURSOR SCROLL LOCAL FOR
    				select ID_HangHoa, ID_LoHang
    				from @tblHangKiemKe ----- read from tblKiemKe
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
    												(SUM(IIF(LoaiHoaDon IN (5, 7, 8,35, 37, 38, 39, 40), -1 * a.SoLuong* a.TyLeChuyenDoi, 
    											IIF(LoaiHoaDon IN (4, 6, 18,13,14), SoLuong * TyLeChuyenDoi, 				
    												IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    											IIF(a.LoaiHoaDon = 10 AND a.YeuCau = '4' AND a.ID_CheckIn = @IDChiNhanhInput, a.TienChietKhau* a.TyLeChuyenDoi, 0))))) 
    												OVER(PARTITION BY a.ID_HangHoa, a.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe,
    												LoaiHoaDon, MaHoaDon,NgayLapHoaDon, YeuCau
    										from
    											(							
    											select 
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
    						(SUM(IIF(LoaiHoaDon IN (5, 7, 8,35, 37, 38, 39, 40), -1 * a.SoLuong* a.TyLeChuyenDoi, 
    					IIF(LoaiHoaDon IN (4, 6, 18,13,14), SoLuong * TyLeChuyenDoi, 				
    						IIF((LoaiHoaDon = 10 AND YeuCau = '1') OR (ID_CheckIn IS NOT NULL AND ID_CheckIn != @IDChiNhanhInput AND LoaiHoaDon = 10 AND YeuCau = '4') AND ID_DonVi = @IDChiNhanhInput, -1 * TienChietKhau* TyLeChuyenDoi, 				
    					IIF(a.LoaiHoaDon = 10 AND a.YeuCau = '4' AND a.ID_CheckIn = @IDChiNhanhInput, a.TienChietKhau* a.TyLeChuyenDoi, 0))))) 
    						OVER(PARTITION BY a.ID_HangHoa, a.ID_LoHang ORDER BY NgayLapHoaDon)) AS TonLuyKe,
    						LoaiHoaDon, MaHoaDon,NgayLapHoaDon,YeuCau
    				from
    					(
    					select 
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
    					where NgayLapHoaDon > @NgayLapHDMin_SubMiliSecond
    					and not exists (select * from @tblHangKiemKe kk where #temp.ID_HangHoa =  kk.ID_HangHoa and (#temp.ID_LoHang = kk.ID_LoHang OR #temp.ID_LoHang is null))
    					) a
    				LEFT JOIN @LuyKeDauKy lkdk ON lkdk.ID_HangHoa = a.ID_HangHoa AND (lkdk.ID_LoHang = a.ID_LoHang OR a.ID_LoHang IS NULL)					
    			end
    		
    		--select *, 1 as after2 from @LuyKeDauKy
    		--select * , @NgayLapHDMin as NgayMin from @hdctUpdate order by NgayLapHoaDon desc
    
		 begin try
    		UPDATE hdct
    		SET hdct.TonLuyKe = IIF(tlkupdate.ID_DonVi = @IDChiNhanhInput, tlkupdate.TonLuyKe, hdct.TonLuyKe), 
    			hdct.TonLuyKe_NhanChuyenHang = IIF(tlkupdate.ID_CheckIn = @IDChiNhanhInput and tlkupdate.LoaiHoaDon = 10, tlkupdate.TonLuyKe, hdct.TonLuyKe_NhanChuyenHang)
    		FROM BH_HoaDon_ChiTiet hdct
    		INNER JOIN @hdctUpdate tlkupdate ON hdct.ID = tlkupdate.ID where tlkupdate.LoaiHoaDon !=9 -- don't update TonLuyKe of HD KiemKe
    
    		-- get TonKho hientai full ID_QuiDoi, ID_LoHang of ID_HangHoa
    		DECLARE @tblTonKho1 TABLE(ID_DonViQuiDoi UNIQUEIDENTIFIER, TonKho FLOAT, ID_LoHang UNIQUEIDENTIFIER)
    		INSERT INTO @tblTonKho1
    		SELECT qd.ID, [dbo].[FUNC_TonLuyKeTruocThoiGian](@IDChiNhanhInput,qd.ID_HangHoa,ID_LoHang, DATEADD(HOUR, 1,GETDATE()))/qd.TyLeChuyenDoi as TonKho, ID_LoHang 
    		FROM @tblChiTiet ct
    		join DonViQuiDoi qd on ct.ID_HangHoa = qd.ID_HangHoa 
    		
    		--select * from @tblTonKho1
    
    		-- UPDATE TonKho in DM_HangHoa_TonKho
    		UPDATE hhtonkho SET hhtonkho.TonKho = ISNULL(cthoadon.TonKho, 0)
    		FROM DM_HangHoa_TonKho hhtonkho
    		INNER JOIN @tblTonKho1 as cthoadon on hhtonkho.ID_DonViQuyDoi = cthoadon.ID_DonViQuiDoi 
    			and (hhtonkho.ID_LoHang = cthoadon.ID_LoHang or cthoadon.ID_LoHang is null) and hhtonkho.ID_DonVi = @IDChiNhanhInput

		--- insert row into DM_HangHoa_TonKho if not exists
			INSERT INTO DM_HangHoa_TonKho(ID, ID_DonVi, ID_DonViQuyDoi, ID_LoHang, TonKho)
			SELECT NEWID(), @IDChiNhanhInput, cthoadon1.ID_DonViQuiDoi, cthoadon1.ID_LoHang, cthoadon1.TonKho
			FROM @tblTonKho1 AS cthoadon1
			LEFT JOIN DM_HangHoa_TonKho hhtonkho1 on hhtonkho1.ID_DonViQuyDoi = cthoadon1.ID_DonViQuiDoi 
			and (hhtonkho1.ID_LoHang = cthoadon1.ID_LoHang or cthoadon1.ID_LoHang is null) and hhtonkho1.ID_DonVi = @IDChiNhanhInput
			WHERE hhtonkho1.ID IS NULL
		end try
		begin catch
		end catch

	
			begin try
				exec Insert_ThongBaoHetTonKho @IDChiNhanhInput, @LoaiHoaDon, @tblHoaDonChiTiet
			end try
			begin catch
			end catch



    
    		-- neu update NhanHang --> goi ham update TonKho 2 lan
    		-- update GiaVon neu tontai phieu NhapHang,ChuyenHang/NhanHang, DieuChinhGiaVon 
    		declare @count2 float = (select count(ID) from @hdctUpdate where LoaiHoaDon in (4,7,10, 18,13,14))
    		select ISNULL(@count2,0) as UpdateGiaVon, ISNULL(@count,0) as UpdateKiemKe, @NgayLapHDMin as NgayLapHDMin

		
	


END");

			Sql(@"ALTER PROCEDURE [dbo].[XoaDuLieuHeThong]
    @CheckHH [int],
    @CheckKH [int]
AS
BEGIN
SET NOCOUNT ON;

				delete from chotso
				delete from BH_HoaDon_ChiPhi
				delete from DM_MauIn
				delete from NS_CongViec
				delete from NS_CongViec_PhanLoai
    			delete from chotso_hanghoa
    			delete from chotso_khachHang
				delete from BH_NhanVienThucHien
    			delete from Quy_HoaDon_ChiTiet
    			delete from Quy_KhoanThuChi
    			delete from Quy_HoaDon
				delete from DM_TaiKhoanNganHang    			
    			delete from BH_HoaDon_ChiTiet
    			delete from BH_HoaDon
    			delete from DM_GiaBan_ApDung
    			delete from DM_GiaBan_ChiTiet
    			delete from DM_GiaBan
    			delete from ChamSocKhachHangs
				delete from HeThong_SMS
				delete from HeThong_SMS_TaiKhoan
				delete from HeThong_SMS_TinMau
				delete from ChietKhauMacDinh_NhanVien
				delete from ChietKhauMacDinh_HoaDon_ChiTiet
				delete from ChietKhauMacDinh_HoaDon
				delete from ChietKhauDoanhThu_NhanVien
				delete from ChietKhauDoanhThu_ChiTiet
				delete from ChietKhauDoanhThu
				delete from NhomDoiTuong_DonVi
				delete from DM_KhuyenMai_ChiTiet
    			delete from DM_KhuyenMai_ApDung
    			delete from DM_KhuyenMai
    			delete from ChietKhauMacDinh_NhanVien   
				 
				delete from Gara_HangMucSuaChua
				delete from Gara_GiayToKemTheo
    			delete from DM_KhuyenMai_ApDung
    			delete from DM_KhuyenMai
    			delete from ChietKhauMacDinh_NhanVien   
				delete from Gara_PhieuTiepNhan
				delete from Gara_DanhMucXe
    			delete from Gara_MauXe where id not like '%00000000-0000-0000-0000-000000000000%'
    			delete from Gara_HangXe where id not like '%00000000-0000-0000-0000-000000000000%'
    			delete from Gara_LoaiXe where id not like '%00000000-0000-0000-0000-000000000000%'
				
    			if(@CheckKH =0)
    			BEGIN
					delete from DM_LienHe_Anh
    				delete from DM_LienHe
					delete from DM_DoiTuong_Anh
					delete from DM_DoiTuong_Nhom
    				delete from DM_DoiTuong WHERE ID != '00000000-0000-0000-0000-000000000002' AND ID != '00000000-0000-0000-0000-000000000000'
					delete from DM_NguonKhachHang
					delete from DM_DoiTuong_TrangThai
    				delete from DM_NhomDoiTuong	  
    									
    			END
    			ELSE 
    			BEGIN
    				UPDATE DM_DoiTuong SET ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE', ID_NhanVienPhuTrach = null, TongTichDiem = 0 
    			END
    		 			
    			if(@CheckHH = 0)
    			BEGIN
						delete from DM_GiaVon
						delete from DM_HangHoa_TonKho
    				   	delete from DinhLuongDichVu
    					delete from DonViQuiDoi
    					delete from HangHoa_ThuocTinh
						delete from DM_HangHoa_ViTri  
    					delete from DM_HangHoa_Anh
    					delete from DM_HangHoa  				
    					delete from DM_ThuocTinh				  				  				
    					delete from DM_NhomHangHoa where ID != '00000000-0000-0000-0000-000000000000' and ID != '00000000-0000-0000-0000-000000000001'
    			END
				ELSE
				BEGIN
					DELETE DM_GiaVon WHERE ID_LoHang is not null
					DELETE DM_GiaVon WHERE ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
					DELETE DM_HangHoa_TonKho WHERE ID_LoHang is not null
					DELETE DM_HangHoa_TonKho WHERE ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
					UPDATE DM_GiaVon SET GiaVon = 0
					UPDATE DM_HangHoa_TonKho SET TonKho = 0
				END
				
				delete from NhomHang_KhoangApDung
				delete from NhomHang_ChiTietSanPhamHoTro
    			delete from DM_LoHang
    			delete from DM_ViTri
    			delete from DM_KhuVuc
    			
    			delete from HT_NhatKySuDung where LoaiNhatKy != 20 and LoaiNhatKy != 21
    					
    			delete from CongDoan_DichVu
    			delete from CongNoDauKi
    			delete from DanhSachThi_ChiTiet	
    			delete from DanhSachThi
    			delete from DM_ChucVu
    			delete from DM_HinhThucThanhToan
    			delete from DM_HinhThucVanChuyen
    			delete from DM_KhoanPhuCap
    			delete from DM_LoaiGiaPhong
    			delete from DM_LoaiNhapXuat
    			delete from DM_LoaiPhieuThanhToan
    			delete from DM_LoaiPhong
    			delete from DM_LoaiTuVanLichHen
    			delete from DM_LopHoc
    			delete from DM_LyDoHuyLichHen
    			delete from DM_MaVach
    			delete from DM_MayChamCong
    			delete from DM_NoiDungQuanTam
    			delete from DM_PhanLoaiHangHoaDichVu
    			delete from DM_ThueSuat
    			
    			delete from HT_CauHinh_TichDiemApDung
    			delete from HT_CauHinh_TichDiemChiTiet		
    			delete from DM_TichDiem	
    			delete from NhomDoiTuong_DonVi where ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from NhomHangHoa_DonVi where ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from NS_LuongDoanhThu_ChiTiet 
    			delete from NS_LuongDoanhThu
    			delete from NS_HoSoLuong 
    			delete from The_NhomThe
    			delete from The_TheKhachHang_ChiTiet
    			delete from The_TheKhachHang
    
    			delete from HT_ThongBao
    			delete from HT_ThongBao_CaiDat where ID_NguoiDung != '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' 
    			delete from HT_Quyen_Nhom where ID_NhomNguoiDung IN (select ID From HT_NhomNguoiDung where ID NOT IN (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' AND ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'))
    			--delete from HT_NguoiDung_Nhom where IDNhomNguoiDung IN (select ID From HT_NhomNguoiDung where ID NOT IN (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' AND ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'))
				delete from HT_NguoiDung_Nhom where IDNguoiDung != '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' 
    			delete from HT_NhomNguoiDung where ID NOT IN (select IDNhomNguoiDung from HT_NguoiDung_Nhom where IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' AND ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE')
    				
    			delete from HT_NguoiDung where ID != '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77' 
				
				delete from NS_PhieuPhanCa_CaLamViec
				delete from NS_PhieuPhanCa_NhanVien
				delete from NS_CaLamViec_DonVi
				delete from NS_ThietLapLuongChiTiet
				delete from NS_CongNoTamUngLuong

				delete from NS_CongBoSung
				delete from NS_BangLuong_ChiTiet
				delete from NS_CaLamViec
				delete from NS_BangLuong			
				delete from NS_KyHieuCong
				delete from NS_NgayNghiLe
				delete from NS_PhieuPhanCa

				delete from NS_MienGiamThue
				delete from NS_KhenThuong
				delete from NS_HopDong
				delete from NS_BaoHiem
				delete from NS_Luong_PhuCap
				delete from NS_LoaiLuong
				delete from NS_NhanVien_CongTac
				delete from NS_NhanVien_DaoTao
				delete from NS_NhanVien_GiaDinh
				delete from NS_NhanVien_SucKhoe
				delete from NS_NhanVien_Anh	
    			delete from NS_QuaTrinhCongTac where ID_NhanVien NOT IN (select ID_NhanVien from HT_NguoiDung where ID = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77') or ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
				update NS_NhanVien SET ID_NSPhongBan = null
    			delete from NS_NhanVien where ID NOT IN (select ID_NhanVien from HT_NguoiDung where ID = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77')
    			delete from NS_PhongBan	 where ID_DonVi is not null and ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from Kho_DonVi where ID_DonVi != 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
    			delete from DM_Kho where ID NOT IN (select ID_Kho from Kho_DonVi where ID_DonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE')
    			delete from DM_DonVi where ID !='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE';
	
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_ThuChi_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit]
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    SELECT 
    MAX(b.TenNhomDoiTuong) as NhomDoiTuong,
    b.MaHoaDon,
    MAX(b.MaPhieuThu) as MaPhieuThu,
    MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    MAX(b.ManguoiNop) as ManguoiNop, 
    MAX(b.TenNguoiNop) as TenNguoiNop, 
	MAX(b.TienMat) AS TienMat,
	MAX(b.TienGui) AS TienGui,
	MAX(b.TienPOS) AS TienPOS,
    MAX(b.ThuChi) as ThuChi, 
    MAX(b.NoiDungThuChi) as NoiDungThuChi,
    MAX(b.GhiChu) as GhiChu,
    MAX(b.LoaiThuChi) as LoaiThuChi,
    	dv.TenDonVi AS TenChiNhanh,
		b.SoTaiKhoan, b.TenNganHang
    FROM
    (
    	  select 
    		a.ID_DoiTuong,
    		a.ID_HoaDon,
    		a.TenNhomDoiTuong,
    		a.ID_NhomDoiTuong,
    	a.MaHoaDon,
    		a.MaPhieuThu,
    	a.NgayLapHoaDon,
    		a.MaNguoiNop,
    	a.TenNguoiNop,
    	--a.ThuChi,
		a.TienMat,
		a.TienGui,
		a.TienPOS,
    		a.TienMat + a.TienGui + a.TienPOS as ThuChi,
    	a.NoiDungThuChi,
    	a.GhiChu,
    	Case when a.LoaiThuChi = 1 then N'Phiếu thu khác'  
    	when a.LoaiThuChi = 2 then N'Phiếu chi khác' 
    	when a.LoaiThuChi = 3 then N'Thu tiền khách trả'  
    	when a.LoaiThuChi = 4 then N'Chi tiền đổi trả hàng'  
    	when a.LoaiThuChi = 5 then N'Thu tiền nhà NCC'  
    	when a.LoaiThuChi = 6 then N'Chi tiền trả NCC' else '' end as LoaiThuChi,
    		a.ID_DonVi,
			a.SoTaiKhoan,
			a.TenNganHang
    	From
    	(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			tknh.SoTaiKhoan as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
    				--Max(dt.TenNhomDoiTuongs) as TenNhomDoiTuong,
    				case when qhdct.ID_NhanVien is not null then N'Nhân viên' else MAX(dt.TenNhomDoiTuongs) end as TenNhomDoiTuong,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 32) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case WHEN qhdct.ID_NhanVien is not null
    				then
    				'00000000-0000-0000-0000-000000000000' 
    				else 
    				case When dtn.ID_NhomDoiTuong is null 
    					
    				then '00000000-0000-0000-0000-000000000000'  else dtn.ID_NhomDoiTuong 
    				end
    				end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    				case when qhdct.ID_NhanVien is not null then nv.MaNhanVien else dt.MaDoiTuong end as ManguoiNop,
    			Sum(qhdct.TienMat) as TienMat,
    			IIF(tknh.TaiKhoanPOS = 1, 0, Sum(qhdct.TienGui)) as TienGui,
				IIF(tknh.TaiKhoanPOS = 1, SUM(qhdct.TienGui), 0) AS TienPOS,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
    				qhd.ID_DonVi,
				nh.TenNganHang
    		From Quy_HoaDon qhd 			
    			join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    				left join NS_NhanVien nv on qhdct.ID_NhanVien= nv.ID
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on tknh.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart and @timeEnd 
    				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, dt.loaidoituong) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
				and qhdct.HinhThucThanhToan NOT IN (4, 5, 6)
    				AND ((select count(Name) from @tblSearch b where     			
    			dt.MaDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or qhd.MaHoaDon like '%' + b.Name + '%'
    				or hd.MaHoaDon like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhdct.ID_NhanVien, dt.MaDoiTuong,dt.LoaiDoiTuong,  nv.MaNhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID, qhd.ID_DonVi,
				 tknh.TaiKhoanPOS, tknh.SoTaiKhoan, nh.TenNganHang
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) 
    	) b
    		inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    		where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = ''
    	Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi, dv.TenDonVi, b.SoTaiKhoan, b.TenNganHang
    	ORDER BY NgayLapHoaDon DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_SoQuy_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [bit],
    @LoaiTien [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;

	--DECLARE @TextSearch [nvarchar](max),
 --   @timeStart [datetime],
 --   @timeEnd [datetime],
 --   @ID_ChiNhanh [nvarchar](max),
 --   @loaiKH [nvarchar](max),
 --   @ID_NhomDoiTuong [nvarchar](max),
 --   @lstThuChi [nvarchar](max),
 --   @HachToanKD [bit],
 --   @LoaiTien [nvarchar](max);
	--SET @TextSearch = '';
	--SET @timeStart = '2022-01-01';
	--SET @timeEnd = '2023-01-01';
	--SET @ID_ChiNhanh = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de';
	--SET @loaiKH = '1,2,4';
	--SET @ID_NhomDoiTuong = '';
	--SET @lstThuChi = '1,2,3,4,5,6';
	--SET @HachToanKD = 'true';
	--SET @LoaiTien = '%%'

    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    --	tinh ton dau ky
    	Declare @TonDauKy float
    	Set @TonDauKy = (Select
    	CAST(ROUND(SUM(TienThu - TienChi), 0) as float) as TonDauKy
    	FROM
    	(
    		select 
    			case when qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as TienThu,
    			Case when qhd.LoaiHoaDon = 12 then qhdct.TienThu else 0 end as TienChi,
    			Case when qhdct.TienMat > 0 and qhdct.TienGui = 0 then '1' 
    			when qhdct.TienGui > 0 and qhdct.TienMat = 0 then '2'
    			when qhdct.TienGui > 0 and qhdct.TienMat > 0 then '12' else '' end as LoaiTien,
    				qhd.HachToanKinhDoanh as HachToanKinhDoanh
    		From Quy_HoaDon qhd 
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		where qhd.NgayLapHoaDon < @timeStart
    		and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    		and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and qhdct.HinhThucThanhToan not in (4,5,6)
    		) a 
    		where LoaiTien like @LoaiTien
    			and (HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    	) 
    		
    	if (@TonDauKy is null)
    	BeGin
    		Set @TonDauKy = 0;
    	END
    	Declare @tmp table (ID_HoaDon UNIQUEIDENTIFIER,MaPhieuThu nvarchar(max), NgayLapHoaDon datetime, KhoanMuc nvarchar(max), TenDoiTac nvarchar(max),
    	TienMat float, TienGui float, TienThu float, TienChi float, ThuTienMat float, ChiTienMat float, ThuTienGui float, ThuTienPOS FLOAT,
		ChiTienGui float, ChiTienPOS FLOAT, TonLuyKeTienMat float,TonLuyKeTienGui float,TonLuyKe float, SoTaiKhoan nvarchar(max), NganHang nvarchar(max), GhiChu nvarchar(max),
    		IDDonVi UNIQUEIDENTIFIER, TenDonVi NVARCHAR(MAX), RN INT);
    	Insert INTO @tmp
    		 SELECT 
    				b.ID_HoaDon,
    				b.MaPhieuThu as MaPhieuThu,
    			b.NgayLapHoaDon as NgayLapHoaDon,
    				MAX(b.NoiDungThuChi) as KhoanMuc,
    			MAX(b.TenNguoiNop) as TenDoiTac, 
    			SUM (b.TienMat) as TienMat,
    			SUM (b.TienGui) as TienGui,
    			SUM (b.TienThu) as TienThu,
    			SUM (b.TienChi) as TienChi,
    			SUM (b.ThuTienMat) as ThuTienMat,
    			SUM (b.ChiTienMat) as ChiTienMat, 
    			SUM (b.ThuTienGui) as ThuTienGui,
				SUM (b.ThuTienPOS) as ThuTienPOS,
    			SUM (b.ChiTienGui) as ChiTienGui, 
				SUM (b.ChiTienPOS) as ChiTienPOS, 
    				0 as TonLuyKe,
    			0 as TonLuyKeTienMat,
    			0 as TonLuyKeTienGui,
    			MAX(b.SoTaiKhoan) as SoTaiKhoan,
    			MAX(b.NganHang) as NganHang,
    			MAX(b.GhiChu) as GhiChu,
    				dv.ID,
    				dv.TenDonVi,
					ROW_NUMBER() OVER (ORDER BY b.NgayLapHoaDon) AS RN
    		FROM
    		(
    				select 
    			a.HachToanKinhDoanh,
    			a.ID_DoiTuong,
    			a.ID_HoaDon,
    			a.MaHoaDon,
    			a.MaPhieuThu,
    			a.NgayLapHoaDon,
    			a.TenNguoiNop,
    			a.TienMat,
    			a.TienGui,
				IIF(a.LoaiHoaDon = 11, IIF(a.TaiKhoanPOS = 1, 0, a.TienGui) , 0) AS ThuTienGui,
				IIF(a.LoaiHoaDon = 11, IIF(a.TaiKhoanPOS = 1, a.TienGui, 0) , 0) AS ThuTienPOS,
    			--case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
				IIF(a.LoaiHoaDon = 12, IIF(a.TaiKhoanPOS = 1, 0, a.TienGui) , 0) AS ChiTienGui,
				IIF(a.LoaiHoaDon = 12, IIF(a.TaiKhoanPOS = 1, a.TienGui, 0) , 0) AS ChiTienPOS,
    			--Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat,
    			case when a.LoaiHoaDon = 11 then a.TienThu else 0 end as TienThu,
    			Case when a.LoaiHoaDon = 12 then a.TienThu else 0 end as TienChi,
    			a.NoiDungThuChi,
    			a.NganHang,
    			a.SoTaiKhoan,
    			a.GhiChu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' else '' end  as LoaiTien,
    				a.ID_DonVi
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			tknh.SoTaiKhoan as SoTaiKhoan,
    			nh.TenNganHang as NganHang,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else 4 end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
    			max(IIF(qhdct.HinhThucThanhToan = 1, qhdct.TienThu, 0)) as TienMat,
    			max(IIF(qhdct.HinhThucThanhToan IN (2,3) , qhdct.TienThu, 0)) as TienGui,
    			max(qhdct.TienThu) as TienThu,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon,
    				qhd.ID_DonVi,
					tknh.TaiKhoanPOS
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on tknh.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (IIF(qhdct.ID_NhanVien is not null, 4, IIF(dt.loaidoituong IS NULL, 1, dt.LoaiDoiTuong)) in (select * from splitstring(@loaiKH)))
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null OR qhdct.DiemThanhToan = 0)
    			and (qhd.HachToanKinhDoanh = @HachToanKD OR @HachToanKD IS NULL)
    				and (dtn.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = '')
    				and qhdct.HinhThucThanhToan not in (4,5,6)
    				AND ((select count(Name) from @tblSearch b where     			
    			dt.TenDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    				or qhd.MaHoaDon like '%' + b.Name + '%'
    				or qhd.NguoiNopTien like '%' + b.Name + '%'
    			)=@count or @count=0)
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau,qhdct.ID_NhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, qhd.ID_DonVi, qhdct.ID, qhdct.HinhThucThanhToan,
				 tknh.TaiKhoanPOS, tknh.SoTaiKhoan, nh.TenNganHang
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
    			inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    			where LoaiTien like @LoaiTien
    		Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaPhieuThu, b.NgayLapHoaDon, dv.TenDonVi, dv.ID, b.SoTaiKhoan
    		ORDER BY NgayLapHoaDon

			--SELECT * FROM @tmp
    -- tính tồn lũy kế
    	    IF (EXISTS (select * from @tmp))
    		BEGIN
    			DECLARE @Ton float;
    			SET @Ton = @TonDauKy;
    			DECLARE @TonTienMat float;
    			SET @TonTienMat = @TonDauKy;
    			DECLARE @TonTienGui float;
    			SET @TonTienGui = @TonDauKy;
    			
    			DECLARE @TienThu float;
    			DECLARE @TienChi float;
    			DECLARE @ThuTienMat float;
    			DECLARE @ChiTienMat float;
    			DECLARE @ThuTienGui float;
    			DECLARE @ChiTienGui float;
    			DECLARE @TonLuyKe float;
				DECLARE @ThuTienPOS float;
				DECLARE @ChiTienPOS float;
    				DECLARE @ID_HoaDon UNIQUEIDENTIFIER;
    	DECLARE CS_ItemUpDate CURSOR SCROLL LOCAL FOR SELECT TienThu, TienChi, ThuTienGui, ThuTienMat, ChiTienGui, ChiTienMat, ID_HoaDon, ThuTienPOS, ChiTienPOS FROM @tmp ORDER BY RN
    	OPEN CS_ItemUpDate 
    FETCH FIRST FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon, @ThuTienPOS, @ChiTienPOS
    WHILE @@FETCH_STATUS = 0
    BEGIN
    	SET @Ton = @Ton + @ThuTienMat + @ThuTienGui + @ThuTienPOS - @ChiTienMat - @ChiTienGui - @ChiTienPOS;
    	SET @TonTienMat = @TonTienMat + @ThuTienMat - @ChiTienMat;
    	SET @TonTienGui = @TonTienGui + @ThuTienGui - @ChiTienGui + @ThuTienPOS - @ChiTienPOS;
    	UPDATE @tmp SET TonLuyKe = @Ton, TonLuyKeTienMat = @TonTienMat, TonLuyKeTienGui = @TonTienGui WHERE ID_HoaDon = @ID_HoaDon
		AND ThuTienMat = @ThuTienMat AND ThuTienGui = @ThuTienGui AND ThuTienPOS = @ThuTienPOS
		AND ChiTienMat = @ChiTienMat AND ChiTienGui = @ChiTienGui AND ChiTienPOS = @ChiTienPOS
    	FETCH NEXT FROM CS_ItemUpDate INTO @TienThu, @TienChi, @ThuTienGui, @ThuTienMat, @ChiTienGui, @ChiTienMat, @ID_HoaDon, @ThuTienPOS, @ChiTienPOS
    END
    CLOSE CS_ItemUpDate
    DEALLOCATE CS_ItemUpDate
    	END
    	ELSE
    	BEGIN
    		Insert INTO @tmp
    	SELECT '00000000-0000-0000-0000-000000000000', 'TRINH0001', '1989-04-07','','','0','0','0','0','0','0','0','0', '0', '0', @TonDauKy, @TonDauKy, @TonDauKy, '','','', '00000000-0000-0000-0000-000000000000', '', 0
    	END
    	Select 
    		ID_HoaDon,
    	MaPhieuThu,
    	NgayLapHoaDon,
    	KhoanMuc,
    	TenDoiTac,
    	@TonDauKy as TonDauKy,
    	TienMat,
    	TienGui,
    	TienThu,
    	TienChi,
    	ThuTienMat,
    	ChiTienMat,
    	ThuTienGui,
		ThuTienPOS,
    	ChiTienGui,
		ChiTienPOS,
    	TonLuyKe,
    	TonLuyKeTienMat,
    	TonLuyKeTienGui,
    	SoTaiKhoan, 
    	NganHang, 
    	GhiChu,
    		IDDonVi, TenDonVi
    	 from @tmp order by RN DESC
END");

			Sql(@"declare @ID_HoaDon uniqueidentifier
		declare _curOut cursor
		for

		select tbl.ID
		from
		(
			select distinct hd.ID , hd.NgayLapHoaDon, hd.MaHoaDon
			from BH_HoaDon_ChiTiet ct
			join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
			and hd.ChoThanhToan='0'
			and hd.LoaiHoaDon in (1,2)
			and ct.ID_ChiTietDinhLuong is not null
			and not exists (
						select hdx.ID from BH_HoaDon hdx
						where hdx.LoaiHoaDon= 35 and hdx.ChoThanhToan='0' and hdx.ID_HoaDon= hd.ID
						)
			and hd.NgayLapHoaDon < '2022-09-01'
		) tbl
		order by tbl.NgayLapHoaDon

		open _curOut
		fetch next from _curOut
		into @ID_HoaDon
		while @@FETCH_STATUS = 0
		begin
				
			exec dbo.CreateXuatKho_NguyenVatLieu @ID_HoaDon,'0'

			fetch next from _curOut
			into @ID_HoaDon
		end
		close _curOut
		deallocate _curOut");

			Sql(@"declare @ID_HoaDon uniqueidentifier, @LoaiXuatKho int, @IsXuatNgayThuoc bit ='0'
		declare _curOut1 cursor
		for

		select tbl.ID,
			case tbl.LoaiHoaDon
			when 1 then 38
			when 2 then 39 end as LoaiXuatKho
		from
		(
			select distinct hd.ID , hd.NgayLapHoaDon, hd.MaHoaDon, hd.LoaiHoaDon
			from BH_HoaDon_ChiTiet ct
			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
			and hd.ChoThanhToan='0'
			and hd.LoaiHoaDon in (1,2)
			and ct.ID_ChiTietDinhLuong is null
			and hh.LaHangHoa='1'
			and hd.NgayLapHoaDon < '2022-09-01'
		) tbl
		order by tbl.NgayLapHoaDon

		open _curOut1
		fetch next from _curOut1
		into @ID_HoaDon, @LoaiXuatKho
		while @@FETCH_STATUS = 0
		begin				
			exec dbo.CreatePhieuXuat_FromHoaDon @ID_HoaDon,@LoaiXuatKho,@IsXuatNgayThuoc,'0' --- ChothanhToan ='0': xuatkho cho HD cu

			fetch next from _curOut1
			into @ID_HoaDon, @LoaiXuatKho
		end
		close _curOut1
		deallocate _curOut1");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[CreateAgainPhieuXuatKho_WhenUpdateTPDL]");
			DropStoredProcedure("[dbo].[CreatePhieuXuat_FromHoaDon]");
			DropStoredProcedure("[dbo].[CreateXuatKho_NguyenVatLieu]");
			DropStoredProcedure("[dbo].[GetInfor_HDHoTro]");
			DropStoredProcedure("[dbo].[GetListGiaVonTieuChuan_ChiTiet]");
			DropStoredProcedure("[dbo].[GetListGiaVonTieuChuan_TongHop]");
			DropStoredProcedure("[dbo].[GetListNhomHang_SetupHoTro]");
			DropStoredProcedure("[dbo].[GetTongGiaTriSuDung_ofKhachHang]");
			DropStoredProcedure("[dbo].[HuyPhieuXuatKho_WhenUpdateTPDL]");
			DropStoredProcedure("[dbo].[NhomHang_GetListSanPhamHoTro]");
        }
    }
}
