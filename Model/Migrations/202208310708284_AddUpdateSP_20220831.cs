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
