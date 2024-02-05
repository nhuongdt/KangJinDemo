namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class kangJin_UpdateSP_20240204 : DbMigration
    {
        public override void Up()
        {

			Sql(@"ALTER Function [dbo].[GetDebitCustomer_allBrands]
(
@ID_DoiTuong uniqueidentifier
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
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) + SUM(ISNULL(tblThuChi.HoanTraSoDuTGT,0))
						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
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
    							0 AS SoLanMuaHang,
									0 AS HoanTraSoDuTGT
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = '0'    						
							AND bhd.ID_DoiTuong= @ID_DoiTuong
    
    						 union all
    							-- tongtra
    						SELECT bhd.ID_DoiTuong,
    							ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
									0 AS HoanTraSoDuTGT
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = '0'  
								AND bhd.ID_DoiTuong= @ID_DoiTuong
    							
    							 union all
    
    							-- tienthu
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
									0 AS HoanTraSoDuTGT
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
							AND qhdct.HinhThucThanhToan not in (6)
							AND qhdct.ID_DoiTuong= @ID_DoiTuong
								
							union all
    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    								0 AS SoLanMuaHang,
									0 AS HoanTraSoDuTGT
    						FROM Quy_HoaDon qhd 
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
								AND qhdct.ID_DoiTuong= @ID_DoiTuong
								union all
    							---- hoantra sodu TGT cho khach (giam sodu TGT)
    						SELECT 
    							bhd.ID_DoiTuong,    	
    								0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
    							
    								-sum(bhd.PhaiThanhToan) as HoanTraSoDuTGT
    					FROM BH_HoaDon bhd
    						where bhd.LoaiHoaDon = 32 and bhd.ChoThanhToan = 0 	
    						group by bhd.ID_DoiTuong
					)AS tblThuChi GROUP BY tblThuChi.ID_DoiTuong   						
    		) a on dt.ID = a.ID_DoiTuong  		
			where dt.ID= @ID_DoiTuong
	) 
	return @NoHienTai
END");

            Sql(@"ALTER FUNCTION [dbo].[fnDemSoLanDoiTra]
(
	@ID uniqueidentifier
)
RETURNS int
AS
BEGIN

	DECLARE @count int = 0

			select @count = sum(SoLan) 					
				from
				(
					select 	
						iif(ID_HoaDon is null,0,1)
						+ isnull((select dbo.fnTinhSoLanDoiTra(ID_HoaDon)),0) as SoLan									
					from BH_HoaDon hd
					where ID = @ID
				)tbl
	
	RETURN @count

END
");

			CreateStoredProcedure(name: "[dbo].[GetAllChiTietHoaDon_afterTraHang]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				LoaiHoaDon = p.Int(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

		if isnull(@CurrentPage,'') ='' set @CurrentPage = 0			
		if isnull(@PageSize,'') ='' set @PageSize = 30
		
		if isnull(@DateFrom,'') ='' set @DateFrom = '2016-01-01'	
		if isnull(@DateTo,'') ='' set @DateTo = DATEADD(day, 1, getdate())				
		else set @DateTo = DATEADD(day, 1, @DateTo)
		
		DECLARE @tblChiNhanh table (ID uniqueidentifier primary key)
		if isnull(@IDChiNhanhs,'') !=''
			insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs)		
		else
			set @IDChiNhanhs =''

		DECLARE @tblSearch TABLE (Name [nvarchar](max))
		DECLARE @count int
		INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!=''
		select @count =  (Select count(*) from @tblSearch)

		------ getHD -----
			select 		
			hd.ID,
			hd.ID_DoiTuong,
			hd.TongThanhToan
		into #hd
		from BH_HoaDon hd
		where hd.ChoThanhToan=0
		and hd.LoaiHoaDon = @LoaiHoaDon
		and hd.NgayLapHoaDon between @DateFrom and @DateTo	
		and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))

		------ get ctMua
		select ct.ID, ct.SoLuong, ct.ID_DonViQuiDoi, ct.ID_LoHang
		into #ctMua
		from BH_HoaDon_ChiTiet ct
		where exists (select id from #hd where ct.ID_HoaDon = #hd.ID)
		and (ct.ID_ChiTietDinhLuong is null OR ct.ID_ChiTietDinhLuong = ct.ID) ---- chi get hanghoa + dv
		and (ct.ID_ParentCombo is null OR ct.ID_ParentCombo != ct.ID)  ---- khong get parent, get TP combo

		
			select 
				hd.ID,
				hd.MaHoaDon,
				hd.LoaiHoaDon,
				hd.NgayLapHoaDon,   						
				hd.ID_DoiTuong,	
				hd.ID_HoaDon,
				hd.ID_ViTri,
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
				iif(hd.TongThanhToan =0 or hd.TongThanhToan is null,  hd.PhaiThanhToan, hd.TongThanhToan) as TongThanhToan,
				ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
				ISNULL(hd.KhuyeMai_GiamGia, 0) as KhuyeMai_GiamGia,
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
				isnull(hd.PhaiThanhToanBaoHiem,0) as  PhaiThanhToanBaoHiem,

				-----gán ID = ID_ChiTietGoiDV để bên ngoài lấy id này luôn ----
				ctMua.ID as ID_ChiTietGoiDV,
				ctMua.ID_DonViQuiDoi,
				ctMua.ID_LoHang,
				ctMua.ID_TangKem, 
    			ctMua.TangKem, 
    			ctMua.ID_ParentCombo,
    			ctMua.ID_ChiTietDinhLuong,
				ctMua.SoLuong,
				ctMua.DonGia,
				ctMua.TienChietKhau,
				ctMua.ThanhToan,
				ctMua.TonLuyKe,				
				ctMua.GhiChu,
				ctMua.TienChietKhau as GiamGia,

    			CAST(ISNULL(ctMua.TienThue,0) as float) as TienThue,
				CAST(ISNULL(ctMua.PTThue,0) as float) as PTThue, 
    			CAST(ISNULL(ctMua.ThoiGianBaoHanh,0) as float) as ThoiGianBaoHanh,
    			CAST(ISNULL(ctMua.LoaiThoiGianBH,0) as float) as LoaiThoiGianBH,
				iif(ctMua.TenHangHoaThayThe is null or ctMua.TenHangHoaThayThe ='', hh.TenHangHoa, ctMua.TenHangHoaThayThe) as TenHangHoaThayThe,
    			Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
    			Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
				iif(hh.LoaiHangHoa is null or hh.LoaiHangHoa= 0, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,

    			
				isnull(lo.MaLoHang,'') as MaLoHang, 
    			isnull(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
				isnull(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
    			isnull(hh.ChietKhauMD_NVTheoPT,'1') as ChietKhauMD_NVTheoPT,				
				ISNULL(qd.LaDonViChuan,'0') as LaDonViChuan,
    			CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
				CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,    			
    			ISNULL(hh.GhiChu,'') as GhiChuHH,
    			ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
				qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
				hh.ID_NhomHang as ID_NhomHangHoa, 

				hh.DichVuTheoGio,
    			hh.DuocTichDiem,
    			hh.SoPhutThucHien,
				qd.MaHangHoa,
				hh.TenHangHoa,
				qd.TenDonViTinh,
				qd.ID_HangHoa,
				hh.QuanLyTheoLoHang,
    			hh.LaHangHoa,
				lo.NgaySanXuat, 
				lo.NgayHetHan, 


				hdgoc.ID as ID_HoaDonGoc,
				hdgoc.LoaiHoaDon as LoaiHoaDonGoc,
				hdgoc.MaHoaDon as MaHoaDonGoc,

				ctConLai.SoLuongBan,
				ctConLai.SoLuongTra,
				ctConLai.SoLuongDung,
				ctConLai.SoLuongBan - isnull(ctConLai.SoLuongTra,0) - isnull(ctConLai.SoLuongDung,0) as SoLuongConLai
			into #ctAll
			from
			(
						select 
							ctMuaTra.ID,
							sum(SoLuongBan) as SoLuongBan,
							sum(SoLuongTra) as SoLuongTra,
							sum(SoLuongDung) as SoLuongDung
						from
						(
								------ mua ----
									select 
										ct.ID,
										ct.SoLuong as SoLuongBan,
										0 as SoLuongTra,
										0 as SoLuongDung
									from #ctMua ct
						

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
										and exists (select id from #ctMua ctMua where ct.ID_ChiTietGoiDV = ctMua.ID)

										
							

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
										and exists (select id from #ctMua ctMua where ct.ID_ChiTietGoiDV = ctMua.ID)

								)ctMuaTra
								group by ctMuaTra.ID
								having sum(SoLuongBan) - sum(SoLuongTra) - sum(SoLuongDung) > 0
			)ctConLai			
			join BH_HoaDon_ChiTiet ctMua on ctConLai.ID = ctMua.ID
			join BH_HoaDon hd on ctMua.ID_HoaDon = hd.ID
			join DonViQuiDoi qd on ctMua.ID_DonViQuiDoi = qd.ID
			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			left join DM_LoHang lo on hh.ID = lo.ID_HangHoa
			left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID			
			left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
			left join BH_HoaDon hdgoc on hd.ID_HoaDon= hdgoc.ID		
			where  ((select count(Name) from @tblSearch b where     			
					hd.MaHoaDon like '%'+b.Name+'%'								
					or dt.MaDoiTuong like '%'+b.Name+'%'		
					or dt.TenDoiTuong like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'		
					or qd.MaHangHoa like '%'+b.Name+'%'									
					or hh.TenHangHoa like '%'+b.Name+'%'									
					or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'	
					)=@count or @count=0)
	

	declare @totalRow int= (select count(ID) from #ctAll)
	select 
		tblLast.*,
		----- thanhtien: lấy số luong conlai * dongia sau ck ---
		tblLast.SoLuongConLai * (tblLast.DonGia - tblLast.TienChietKhau) as ThanhTien,
		@totalRow as TotalRow,
		nv.TenNhanVien,
		tblLast.TongThanhToan 
			--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = 0
			- iif(tblLast.LoaiHoaDonGoc = 6, iif(tblLast.LuyKeTraHang > 0, tblLast.TongGiaTriTra, 
					iif(abs(tblLast.LuyKeTraHang) > tblLast.TongThanhToan, tblLast.TongThanhToan, 
					iif(KhachNo >0,abs(tblLast.LuyKeTraHang)  + tblLast.TongGiaTriTra ,abs(tblLast.LuyKeTraHang) ))
					), tblLast.LuyKeTraHang)
			- tblLast.KhachDaTra  as ConNo 
		from(
			select 
				tbl.*,
					isnull(iif(tbl.LoaiHoaDonGoc = 3 or tbl.ID_HoaDon is null,
					iif(tbl.KhachNo <= 0, 0, ---  khachtra thuatien --> công nợ âm
						case when tbl.TongGiaTriTra > tbl.KhachNo then tbl.KhachNo						
						else tbl.TongGiaTriTra end),
					(select dbo.BuTruTraHang_HDDoi(tbl.ID_HoaDon,tbl.NgayLapHoaDon,tbl.ID_HoaDonGoc, tbl.LoaiHoaDonGoc))				
				),0) as LuyKeTraHang	
			
			from (
					select hd.*,
						ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,	
						ISNULL(allTra.NoTraHang,0) as NoTraHang,
						isnull(sqHD.KhachDaTra,0) as KhachDaTra,
						hd.TongThanhToan- isnull(sqHD.KhachDaTra,0) as KhachNo
					from
					(
						----- get top 10 ----
						select * from #ctAll
						order by NgayLapHoaDon desc
						offset (@CurrentPage * @PageSize) rows
						fetch next @PageSize rows only
					) hd
					left join
							(							
									------ thu hoadon -----
									select 
										qct.ID_HoaDonLienQuan,
										sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, - qct.TienThu)) as KhachDaTra
									from Quy_HoaDon qhd
									join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon= qhd.ID
									where qhd.TrangThai='1'
									and exists (select hd.ID from #hd hd 
										where qct.ID_HoaDonLienQuan = hd.ID and  hd.ID_DoiTuong = qct.ID_DoiTuong)
									group by qct.ID_HoaDonLienQuan															
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
				)tbl
		)tblLast
		left join NS_NhanVien nv on tblLast.ID_NhanVien= nv.ID
		order by NgayLapHoaDon desc
				

		drop table #ctMua
		drop table #ctAll
		drop table #hd");

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
			iif(tbl.LoaiHoaDonGoc = 6, iif(tbl.LuyKeTraHang > 0, 0,
							---- nếu hdDoi đã được bù trừ theo lũy kế (giá trị sau bù trừ > 0)
							---- nhưng lại tiếp tục trả hàng, phải tính bù trừ cả phần Trả hàng mới nữa ---
							iif(NoTraHang >	abs(tbl.LuyKeTraHang), NoTraHang ,
							abs(tbl.LuyKeTraHang)  
							)), 					 	
				tbl.LuyKeTraHang) as TongTienHDTra
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
				hdGocTra.TongGiaTriTra,
				NoTraHang,
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
				isnull(NoTraHang,0) as NoTraHang,
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

			Sql(@"ALTER PROCEDURE [dbo].[SP_GetHoaDonAndSoQuy_FromIDDoiTuong]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;

declare @tblChiNhanh table (ID uniqueidentifier)
insert into @tblChiNhanh
select name from dbo.splitstring(@ID_DonVi)


	DECLARE @tblHoaDon TABLE(ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, LoaiHoaDon INT, GiaTri FLOAT);
	DECLARE @LoaiDoiTuong INT;
	SELECT @LoaiDoiTuong = LoaiDoiTuong FROM DM_DoiTuong WHERE ID = @ID_DoiTuong;
	IF(@LoaiDoiTuong = 3)
	BEGIN
		INSERT INTO @tblHoaDon
    	select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.LoaiHoaDon,
			hd.PhaiThanhToanBaoHiem as GiaTri
    	from BH_HoaDon hd
    	where hd.ID_BaoHiem like @ID_DoiTuong and hd.ID_DonVi in (select ID from @tblChiNhanh)
    	and hd.LoaiHoaDon not in (3,23) -- dieu chinh the: khong lien quan cong no
		and hd.ChoThanhToan ='0'
	END
	ELSE
	BEGIN
		INSERT INTO @tblHoaDon
		select *
		from
		(
		select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.LoaiHoaDon,
			case hd.LoaiHoaDon
				when 4 then ISNULL(hd.TongThanhToan,0)
				when 6 then - ISNULL(hd.TongThanhToan,0)
				when 7 then - ISNULL(hd.TongThanhToan,0)
				when 42 then - ISNULL(hd.TongThanhToan,0) --- tattoan congno thegiatri
				when 32 then - ISNULL(hd.TongThanhToan,0) ---- hoantracoc
			--	when 14 then - ISNULL(hd.TongThanhToan,0)
				when 14 then - iif(@LoaiDoiTuong = 2, ISNULL(hd.TongThanhToan,0),0) ---- nhaphang khachthua: không liên quan đến công nợ khách hàng
			else
    			ISNULL(hd.PhaiThanhToan,0)
    		end as GiaTri
    	from BH_HoaDon hd
		join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
    	where hd.ID_DoiTuong like @ID_DoiTuong 
    	and hd.LoaiHoaDon not in (3,23,31,35,37,38,39,40) 
		and hd.ChoThanhToan ='0'

		union all
		---- chiphi dichvu
		select 
			cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon, 125 as LoaiHoaDon,
			sum(cp.ThanhTien) as GiaTri			
		from BH_HoaDon_ChiPhi cp
		join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
		join @tblChiNhanh cn on hd.ID_DonVi= cn.ID
		where hd.ChoThanhToan= 0
		and cp.ID_NhaCungCap= @ID_DoiTuong
		group by cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon,hd.LoaiHoaDon
		)a
	END

	---select * from @tblHoaDon
		
		SELECT *, 0 as LoaiThanhToan
		FROM @tblHoaDon
    	union
    	-- get list Quy_HD (không lấy Quy_HoaDon thu từ datcoc)
		select * from
		(
    		select qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon ,
			case when dt.LoaiDoiTuong = 1 OR dt.LoaiDoiTuong = 3 then
				case when qhd.LoaiHoaDon= 11 then -sum(qct.TienThu) else iif (max(LoaiThanhToan)=4, -sum(qct.TienThu),sum(qct.TienThu)) end
			else 
    			case when qhd.LoaiHoaDon = 11 then sum(qct.TienThu) else -sum(qct.TienThu) end
    		end as GiaTri,
			iif(qhd.PhieuDieuChinhCongNo='1',2, max(qct.LoaiThanhToan)) as LoaiThanhToan --- 1.coc, 2.dieuchinhcongno, 3.khong butru congno			
    		from Quy_HoaDon qhd	
    		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    		join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID			
    		where qct.ID_DoiTuong like @ID_DoiTuong 
			and exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)
			and not exists (select id from BH_HoaDon pthh where qct.ID_HoaDonLienQuan = pthh.ID and pthh.LoaiHoaDon= 41) --- khong lay phieuchi hoahong nguoi GT
			and qct.HinhThucThanhToan !=6			
			and (qct.LoaiThanhToan is null or qct.LoaiThanhToan !=3) ---- LoaiThnahToan = 3. thu/chi khong lienquan congno
			and (qhd.TrangThai is null or qhd.TrangThai='1') -- van phai lay phieu thu tu the --> trừ cong no KH
			group by qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon,dt.LoaiDoiTuong,qhd.PhieuDieuChinhCongNo
		) a where a.GiaTri != 0 -- khong lay phieudieuchinh diem

	
END");

			Sql(@"ALTER PROCEDURE [dbo].[ListHangHoaTheKho]
    @ID_HangHoa [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT ID_HoaDon, 
		MaHoaDon, 
		NgayLapHoaDon,
		LoaiHoaDon, 
		ID_DonVi,
		ID_CheckIn,		
		sum(table1.SoLuong) as SoLuong,
		max(table1.GiaVon) as GiaVon,
		---- lamtron 2 so thapphan --> check lech TonKho - TonLuyKe at js ----
		round(max(table1.TonKho),3) as TonKho,		
		round(sum(sum(table1.SoLuong)) over ( order by NgayLapHoaDon ),3) as LuyKeTonKho,
	case table1.LoaiHoaDon
			when 10 then case when table1.ID_CheckIn = @IDChiNhanh then N'Nhận chuyển hàng' else N'Chuyển hàng' end
			when 40 then N'Xuất hỗ trợ chung'
			when 39 then N'Xuất bảo hành'
			when 38 then N'Xuất bán lẻ'
			when 37 then N'Xuất hỗ trợ ngày thuốc'	
			when 35 then N'Xuất nguyên vật liệu'	
			when 4 then N'Nhập hàng'
			when 6 then N'Khách trả hàng'
			when 7 then N'Trả hàng nhập'
			when 8 then N'Xuất kho'
			when 9 then N'Kiểm hàng'
			when 13 then N'Nhập nội bộ'
			when 14 then N'Nhập hàng thừa'
			when 18 then N'Điều chỉnh giá vốn'		
		end as LoaiChungTu
	FROM
	(
		SELECT hd.ID as ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, 
		CASE WHEN hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4' and hd.LoaiHoaDon = 10 THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon,
		bhct.ThanhTien * dvqd.TyLeChuyenDoi as ThanhTien,
		bhct.TienChietKhau * dvqd.TyLeChuyenDoi TienChietKhau, 
		dvqd.TyLeChuyenDoi,
		hd.YeuCau, 
		hd.ID_CheckIn,
		hd.ID_DonVi,
		hh.QuanLyTheoLoHang,
		dvqd.LaDonViChuan, 
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.TonLuyKe, bhct.TonLuyKe_NhanChuyenHang) as TonKho,
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.GiaVon / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi),bhct.GiaVon_NhanChuyenHang / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi)) as GiaVon,	
		bhct.SoThuTu,
		(case hd.LoaiHoaDon
			when 9 then bhct.SoLuong ---- Số lượng lệch = SLThucTe - TonKhoDB        (-) Giảm  (+): Tăng
			when 10 then
				case when hd.ID_CheckIn= @IDChiNhanh and hd.YeuCau = '4' then bhct.TienChietKhau  ---- da nhanhang
				else iif(hd.YeuCau = '4',- bhct.TienChietKhau,- bhct.SoLuong) end 
			--- xuat
			when 40 then - bhct.SoLuong
			when 39 then - bhct.SoLuong
			when 38 then - bhct.SoLuong
			when 37 then - bhct.SoLuong
			when 35 then - bhct.SoLuong		
			when 7 then - bhct.SoLuong
			when 8 then - bhct.SoLuong		
			--- conlai: nhap
			else bhct.SoLuong end
		) * dvqd.TyLeChuyenDoi as SoLuong
		
	FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	LEFT JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	WHERE hd.ChoThanhToan = 0 and hd.LoaiHoaDon in (6,7,35,37,38,39,40,8,4,7,9,10,13,14,18) 
	and (bhct.ChatLieu is null or bhct.ChatLieu not in ('2','5') ) --- ChatLieu = 2: tra GDV, 5. cthd da xoa
	and  hh.ID = @ID_HangHoa 
	and ((hd.ID_DonVi = @IDChiNhanh and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)) or (hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4'))
	)  table1
	group by ID_HoaDon, MaHoaDon, NgayLapHoaDon,LoaiHoaDon, ID_DonVi, ID_CheckIn
	
	order by NgayLapHoaDon desc




END");

			Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMuc_KhachHangNhaCungCap]
    @IDChiNhanhs [nvarchar](max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @LoaiDoiTuong [int] = 1,
    @IDNhomKhachs [nvarchar](max) ='',
    @TongBan_FromDate [datetime] ='',
    @TongBan_ToDate [datetime]='',
    @NgayTao_FromDate [datetime] ='',
    @NgayTao_ToDate [datetime]='',
    @TextSearch [nvarchar](max)='KH112',
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
				isnull(a.DieuChinhSoDuTGT,0) as DieuChinhSoDuTGT,
				isnull(a.ThuHDLe,0)  as ThuHDLe,
				isnull(tblSuDung.SuDungGDV,0) as SuDungGDV,
				isnull(a.TienKhach_biGiamTru,0) as TienKhach_biGiamTru,
				
				----- === giatrisudung = sudung (tu hoadonle + gdv) -----
				isnull(tblSuDung.GiaTriSuDung,0)  as GiaTriDVSuDung,
				----- === tiền còn lại chưa dùng = số dư TGT + tiền GDV chưa dùng (không liên quan đến  hdLẻ) ----
				isnull(a.TongThuKhachHang,0) + isnull(a.DieuChinhSoDuTGT,0)  - isnull(a.ThuHDLe,0) 
					- isnull(tblSuDung.SuDungGDV,0) - isnull(a.TongChiKhachHang,0)
					- isnull(a.TienKhach_biGiamTru,0) as SoTienChuaSD,

    			isnull(a.NoHienTai,0) as NoHienTai,
    			isnull(a.TongBan,0) as TongBan,
    			isnull(a.TongMua,0) as TongMua,
    			isnull(a.TongBanTruTraHang,0) as TongBanTruTraHang,
    			cast(isnull(a.SoLanMuaHang,0) as float) as SoLanMuaHang,
    			isnull(a.PhiDichVu,0) as PhiDichVu,
				isnull(a.NapCoc,0) as NapCoc,
				isnull(a.SuDungCoc,0) as SuDungCoc,
				isnull(a.SoDuCoc,0) as SoDuCoc,
				SUBSTRING(DienThoai1,len(DienThoai1) -2 , 3) as DienThoai,
    			CONCAT(dt.MaDoiTuong,'' '', dt.TenDoiTuong, '' '', dt.DienThoai1, '' '', dt.TenDoiTuong_KhongDau) as Name_Phone
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
    				isnull(dt.DienThoai,'''') as DienThoai1,
    				isnull(dt.Email,'''') as Email,
    				isnull(dt.DiaChi,'''') as DiaChi,
    				isnull(dt.MaSoThue,'''') as MaSoThue,
    				isnull(dt.GhiChu,'''') as GhiChu,
    				ISNULL(dt.NguoiTao,'''') as NguoiTao,
    				iif(dt.IDNhomDoiTuongs='''' or dt.IDNhomDoiTuongs is null,''00000000-0000-0000-0000-000000000000'', dt.IDNhomDoiTuongs) as ID_NhomDoiTuong
    			from DM_DoiTuong dt ', @whereCus, N' )  dt
				left join 
				(
				 ----- Hoàn dịch vụ: chỉ lấy phiếu chi trả hàng từ hóa đơn lẻ ---
					 select 
						qct.ID_DoiTuong,
						sum(qct.TienThu) as GiaTriHoanTraGDV
					 from
					 (
						 select 
							hd.ID					
						 from BH_HoaDon hd
						 join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon',
						 @whereInvoice,
						' and ct.ChatLieu = ''1''  and hd.LoaiHoaDon = 6
						  and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.id)
						 group by hd.ID
					 )hdTra
					 join Quy_HoaDon_ChiTiet qct on hdTra.ID = qct.ID_HoaDonLienQuan
					 join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
					 where qhd.TrangThai = 1
					 group by qct.ID_DoiTuong
				) traGDV on dt.ID = traGDV.ID_DoiTuong

				left join 
				(			
				 	 ----- giatri sudung DV: sudung buoi le/ sudung tu GDV ---
				  select 
						tbl.ID_DoiTuong,
						sum(SuDungGDV) as SuDungGDV,
						sum(isnull(SuDungHDLe,0)) as SuDungHDLe,
						sum(SuDungGDV) + sum(isnull(SuDungHDLe,0)) as GiaTriSuDung
					 from
					 (
					 select 
								hd.ID_DoiTuong,					
								iif(ctsd.ChatLieu =5,0,
									iif(gdv.ID is not null, 
										---- sudung GDV --
										ctsd.SoLuong * (ctm.DonGia - ctm.TienChietKhau) * (1 -  gdv.TongGiamGia/iif(gdv.TongTienHang =0,1,gdv.TongTienHang)),
										0)) as SuDungGDV,
								iif(ctsd.ChatLieu =5,0,
									iif(gdv.ID is null, 
										---- sudung hdle --
										iif(hd.TongTienHang =0,ctsd.ThanhTien,
											ctsd.ThanhTien * (1- hd.TongGiamGia/hd.TongTienHang)),							
										0)) as SuDungHDLe

							 from BH_HoaDon hd
							 join BH_HoaDon_ChiTiet ctsd on hd.ID = ctsd.ID_HoaDon and hd.LoaiHoaDon= 1
							 left join BH_HoaDon_ChiTiet ctm on ctsd.ID_ChiTietGoiDV = ctm.ID 
								and (ctsd.ID_ChiTietDinhLuong is null or ctsd.ID_ChiTietDinhLuong = ctsd.id) ----- khong lay tpdinhluonh
							 left join BH_HoaDon gdv on ctm.ID_HoaDon = gdv.ID and gdv.LoaiHoaDon = 19',
							 @whereInvoice,
					N' )tbl group by tbl.ID_DoiTuong				 
				) tblSuDung on dt.ID = tblSuDung.ID_DoiTuong

    			left join
    			(
    			select 
    				 tblThuChi.ID_DoiTuong,
					 -----ThuHDLe,DieuChinhSoDuTGT:  2 trường này dùng để tính số tiền còn lại chưa dùng ---
					 SUM(isnull(tblThuChi.ThuHDLe,0)) as ThuHDLe,					 
					 SUM(isnull(tblThuChi.TienKhach_biGiamTru,0)) as TienKhach_biGiamTru,
					 SUM(isnull(tblThuChi.DieuChinhSoDuTGT,0)) as DieuChinhSoDuTGT,
					 SUM(ISNULL(tblThuChi.ThuHoaDon,0)) as TongThuKhachHang,
					 SUM(ISNULL(tblThuChi.ChiTuGDV,0)) - SUM(ISNULL(tblThuChi.HoanTraThe,0)) as TongChiKhachHang,
    				 SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.HoanTraThe,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) 
							+ sum(ISNULL(tblThuChi.DoanhThuThe,0))
    						- sum(isnull(tblThuChi.PhiDichVu,0)) 
    						- SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
    				sum(ISNULL(tblThuChi.DoanhThuThe,0)) as DoanhThuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.TraHangGDV,0)) AS TongBanTruTraHang,
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
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ThuHDLe,
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					0 as DoanhThuThe,
    					sum(cp.ThanhTien) as PhiDichVu,
						0 as DieuChinhSoDuTGT,
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
							0 as TraHangGDV,
							0 as TienKhach_biGiamTru,
    						0 as DoanhThu,
							0 AS TienThu,
    						0 AS TienChi, 
							0 as ThuHoaDon,
							0 as ThuHDLe,
							0 as ChiTuGDV,
    						0 AS SoLanMuaHang,
							0 as DoanhThuThe,
							0 as PhiDichVu,
							----- Loai = 23: (TongTienHang: chênh lệch giữa số dư cũ và số dư sau khi điều chỉnh (+/-) ---
							----- neu loai = 32: (TongGiamGia: chi phí hoàn trả: không ảnh hưởng đến số sư thẻ ----
							----- PhaiThanhToan: số tiền phải thanh toán sau khi trừ phí)
							----- lấy dấu âm (-TongGiamGia): vì TongChiKhachHang = - sum(HoanTraThe): trừ trừ thành cộng
							sum(iif(LoaiHoaDon = 23, bhd.TongTienHang, -bhd.TongGiamGia)) as DieuChinhSoDuTGT, 
							-sum(iif(LoaiHoaDon = 32, bhd.PhaiThanhToan,0)) as HoanTraThe,
							0 as NapCoc,
							0 as SuDungCoc
    				FROM BH_HoaDon bhd ',
					 @whereChiNhanh,
					 @whereNgayLapHD,
					N' and bhd.LoaiHoaDon in (23,32) and bhd.ChoThanhToan = 0 
					and exists (select * from @tblChiNhanh cn where bhd.ID_DonVi= cn.ID)
					group by bhd.ID_DoiTuong
    
    				union all
    				----- tongban ----
    				SELECT 
    					hd.ID_DoiTuong,    	
    					0 as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					hd.PhaiThanhToan as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ThuHDLe,
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					0 as DoanhThuThe,
    					0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ', @whereInvoice, N'  and hd.LoaiHoaDon in (1,7,19,25) ',
    
    				N' union all
    				---- doanhthu tuthe
    				SELECT 
    					hd.ID_DoiTuong,    	
    					0 as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ThuHDLe,
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					iif(hd.LoaiHoaDon = 42, - hd.PhaiThanhToan, hd.PhaiThanhToan) as DoanhThuThe,
    					0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ', @whereInvoice , N' and hd.LoaiHoaDon in (22,42) ', 
    
    					N' union all
    				------ gia tri trả từ bán hàng + gdv ----
    				SELECT 
    					hd.ID_DoiTuong,    	
    					hd.PhaiThanhToan as GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 as DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi, 
						0 as ThuHoaDon,
						0 as ThuHDLe,
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
    					0 as DoanhThuThe,
    					0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						0 as SuDungCoc
    				FROM BH_HoaDon hd ',  @whereInvoice, N'  and hd.LoaiHoaDon in (6,4) ',


					------ get giatri trahang tu GDV ----> tính vào Tổng bán trừ Trả hàng

					N' union all 
						 select 
							hd.ID_DoiTuong,    	
							0 as GiaTriTra,
							----- chiết khấu hàng trả: không ảnh hưởng đến giá trị GDV mua ban đầu --
    						sum(ct.DonGia * ct.SoLuong) as TraHangGDV,
							---- nhưng, khách vẫn bị giảm trừ tiền, cửa hàng được thêm tiền thôi ---
							sum(ct.SoLuong * ct.TienChietKhau) as TienKhach_biGiamTru,
    						0 as DoanhThu,
    						0 AS TienThu,
    						0 AS TienChi, 
							0 as ThuHoaDon,
							0 as ThuHDLe,
							0 as ChiTuGDV,
    						0 AS SoLanMuaHang,
    						0 as DoanhThuThe,
    						0 as PhiDichVu,
							0 as DieuChinhSoDuTGT,
							0 as HoanTraThe,
							0 as NapCoc,
							0 as SuDungCoc				
						 from BH_HoaDon hd
						 join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon',
						 @whereInvoice,
						' and ct.ChatLieu = ''2''  and hd.LoaiHoaDon = 6
						  and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.id)
						 group by hd.ID_DoiTuong

					',

    				
    				N' union all
    				----- tienthu/chi ---
    				SELECT 
    				qct.ID_DoiTuong,						
    				0 AS GiaTriTra,
					0 as TraHangGDV,
					0 as TienKhach_biGiamTru,
    				0 AS DoanhThu,
    				iif(qhd.LoaiHoaDon=11,qct.TienThu,0) AS TienThu,
    				iif(qhd.LoaiHoaDon=12,qct.TienThu,0) AS TienChi,
					------ThuHoaDon: khônglấy tiền thu/chi từ TGT ----
					iif(qhd.LoaiHoaDon=11, iif(qct.HinhThucThanhToan = 4,0, qct.TienThu),0) as ThuHoaDon,
					iif(hd.LoaiHoaDon = 1, iif(qhd.LoaiHoaDon=11, iif(qct.HinhThucThanhToan = 4,0, qct.TienThu),0),0) as ThuHDLe,
					0 as ChiTuGDV,
    				0 AS SoLanMuaHang,
    				0 as DoanhThuThe,
    				0 as PhiDichVu,
					0 as DieuChinhSoDuTGT,
					0 as HoanTraThe,
					0 as NapCoc,
					0 as SuDungCoc
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qct ON qhd.ID = qct.ID_HoaDon 
				join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID',
    				@whereInvoice, 
    				N' and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
					and qct.HinhThucThanhToan not in (6) ----- khong lấy phiếu chi nạp cọc ---
    				and (qct.LoaiThanhToan is null or qct.LoaiThanhToan != 3) ',

					------ hoancoc: chỉ lấy tiền hoàn lại khi mua GDV/hoặc hoàn cọc TGT ----
					N' union all
    				----- tienthu/chi ---
    				SELECT 
    				qct.ID_DoiTuong,						
    				0 AS GiaTriTra,
					0 as TraHangGDV,
					0 as TienKhach_biGiamTru,
    				0 AS DoanhThu,
					0 as TienThu,
					0 as TienChi,
					0 as ThuHoaDon,			
					0 as ThuHDLe,
					iif(qct.ID_HoaDonLienQuan is null,0, iif(qhd.LoaiHoaDon=12,iif(qct.HinhThucThanhToan = 4,0, qct.TienThu),0)) as ChiTuGDV,
    				0 AS SoLanMuaHang,
    				0 as DoanhThuThe,
    				0 as PhiDichVu,
					0 as DieuChinhSoDuTGT,
					0 as HoanTraThe,
					0 as NapCoc,
					0 as SuDungCoc
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qct ON qhd.ID = qct.ID_HoaDon',
    				@whereChiNhanh, 
					@whereNgayLapHD,
    				N' and (qhd.TrangThai != 0 OR qhd.TrangThai is null)
					and qct.HinhThucThanhToan not in (6) ----- khong lấy phiếu chi nạp cọc ---
    				and (qct.LoaiThanhToan is null or qct.LoaiThanhToan != 3) 
					and exists (select hdTra.id from BH_HoaDon hdTra 
						join BH_HoaDon_ChiTiet ctTra on hdTra.ID = ctTra.ID_HoaDon
						where hdTra.LoaiHoaDon = 6
						and ctTra.ChatLieu = ''2''
						and qct.ID_HoaDonLienQuan =  hdTra.ID) ',
    				
					---- NapCoc NCC----	

					N' union all
					
    				SELECT 
    					qhdct.ID_DoiTuong,						
    					0 AS GiaTriTra,
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as ThuHoaDon,
						0 as ThuHDLe,
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
						0 as DoanhThuThe,
						0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
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
						0 as TraHangGDV,
						0 as TienKhach_biGiamTru,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi,
						0 as ThuHoaDon,
						0 as ThuHDLe,
						0 as ChiTuGDV,
    					0 AS SoLanMuaHang,
						0 as DoanhThuThe,
						0 as PhiDichVu,
						0 as DieuChinhSoDuTGT,
						0 as HoanTraThe,
						0 as NapCoc,
						iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu) as SuDungCoc
    				FROM Quy_HoaDon qhd
    				JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon ',
					@whereChiNhanh, 
    				N' and (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
					and qhdct.HinhThucThanhToan = 6 ',					       				
    			N')tblThuChi 
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
					SUM(GiaTriDVSuDung) as SumGiaTriDVSuDung,
					SUM(SoTienChuaSD) as SumSoTienChuaSD
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
    
    		print @sql
    		print @sql2
    		print @sql3
    
    
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

			Sql(@"ALTER PROCEDURE [dbo].[GetInforKhachHang_ByID]
    @ID_DoiTuong [uniqueidentifier],
    @ID_ChiNhanh [nvarchar](max),
    @timeStart [nvarchar](max),
    @timeEnd [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
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
    			where dt.ID= @ID_DoiTuong
END");

			Sql(@"ALTER PROCEDURE [dbo].[getListDanhSachHHImportKiemKe]
    @MaLoHangIP [nvarchar](max),
    @MaHangHoaIP [nvarchar](max),
    @ID_DonViIP [uniqueidentifier],
    @TimeIP [datetime]
AS
BEGIN

		set nocount on;

		select 
			hh.ID,
    		lh.ID as ID_LoHang,
			dvqd.ID as ID_DonViQuiDoi,
			dvqd.MaHangHoa,
    		hh.TenHangHoa,
    		hh.QuanLyTheoLoHang,
    		dvqd.TenDonViTinh,
    		dvqd.TyLeChuyenDoi,
    		dvqd.GiaNhap,
			lh.NgaySanXuat,
			lh.NgayHetHan,
			cast(0 as float) as TonKho, --- get tonkho + giavon at js (other function)
			cast(0 as float) as GiaVon,
			dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			Case when lh.ID is null then '' else lh.MaLoHang end as MaLoHang    				
		from
		(
		select 
			qd.ID,
			qd.ID_HangHoa,
			qd.MaHangHoa,
			qd.TenDonViTinh,
			qd.ThuocTinhGiaTri,
			qd.TyLeChuyenDoi,
			qd.GiaNhap			
		from DonViQuiDoi qd
		where rtrim(ltrim(qd.MaHangHoa)) =  @MaHangHoaIP
		and qd.Xoa='0'
		)dvqd
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    	left join DM_LoHang lh on lh.ID_HangHoa = hh.ID and lh.MaLoHang = @MaLoHangIP     	
    	where hh.TheoDoi = 1 		
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetBaoCaoCongNoChiTiet]
	@IDChiNhanhs [nvarchar](max) = 'd93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @DateFrom [datetime] = '2023-01-01',
    @DateTo [datetime] ='2024-12-01',
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


	----- get hd from- to ----
	select hd.ID, hd.LoaiHoaDon
	into #tblHD
	from BH_HoaDon hd
    where hd.LoaiHoaDon in (1,19,22)
    and hd.ChoThanhToan = '0'
    and hd.TongThanhToan > 0
    and hd.NgayLapHoaDon between @DateFrom and @DateTo
    and exists (select ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID)
    
	select 
		tblLast.*,
			ISNULL(qtCN.GiaTriTatToan,0) as GiaTriTatToan,
			iif(tblLast.LoaiHoaDon=22, tblLast.ConNo1 - ISNULL(qtCN.GiaTriTatToan,0),tblLast.ConNo1) as ConNo,
			iif(tblLast.LoaiHoaDon=22, tblLast.KhachDaTra - ISNULL(qtCN.GiaTriTatToan,0), 
				---- nothucteGDV: khachtra - dasudung - gTriConLai sautra
				iif(tblLast.KhachDaTra >  tblLast.GiaTriSD or tblLast.GiaTriSD = 0, 0,
				----- Nếu có bù trừ: phải trừ đi gtrị bù trừ ---
					iif(tblLast.LuyKeTrahang < 0, tblLast.GiaTriSD - isnull(tblLast.TongTienHDTra,0) - tblLast.KhachDaTra,
					tblLast.GiaTriSD - tblLast.KhachDaTra))				
				) as NoThucTe
	into #tblDebit	
	from
	(
	select tblBuTru.*,
		iif(tblBuTru.ChoThanhToan is null,0, 
						tblBuTru.TongThanhToan - tblBuTru.TongTienHDTra - tblBuTru.KhachDaTra							
							) as ConNo1
	from
	(
	select tblLuyKe.*,
		iif(tblLuyKe.LoaiHoaDonGoc = 6, iif(tblLuyKe.LuyKeTraHang > 0, tblLuyKe.TongGiaTriTra, 
						iif(abs(tblLuyKe.LuyKeTraHang) > tblLuyKe.TongThanhToan, tblLuyKe.TongThanhToan, abs(tblLuyKe.LuyKeTraHang))), tblLuyKe.LuyKeTraHang) as TongTienHDTra	
	from
	(

		select c.* ,
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
    	from #tblHD hd1
		join BH_HoaDon hd	on hd1.ID = hd.ID
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
    						qct.ID_HoaDonLienQuan,
    						iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as TienThu				
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 					
    					where (qhd.TrangThai is null or qhd.TrangThai='1')	
						and exists (select id from #tblHD hd where hd.ID = qct.ID_HoaDonLienQuan)  				    
						------ kangjin: khong co DatHang: nen khoong lay thutuDH
    					
    					
    			) tblUnion
    			group by tblUnion.ID_HoaDonLienQuan
    	) soquy on hd.ID= soquy.ID_HoaDonLienQuan
    	left join(
    		------ sudung gdv
    		select 
    			gdv.ID,
    			sum(ctsd.SoLuong * (ctsd.DonGia - ctsd.TienChietKhau)) as GiaTriSD
    		from #tblHD gdv
    		join BH_HoaDon_ChiTiet ctm on gdv.ID = ctm.ID_HoaDon
    		 join BH_HoaDon_ChiTiet ctsd on ctm.ID= ctsd.ID_ChiTietGoiDV 
    		 join BH_HoaDon hdsd on ctsd.ID_HoaDon= hdsd.ID 
    		where gdv.LoaiHoaDon= 19 --and gdv.ChoThanhToan='0'
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
    	where	
		((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'	
    					or hd.MaHoaDon like '%' +b.Name +'%' 		
    					)=@count or @count=0)	
    	) c	where (@TrangThais ='' or c.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))	 
		)tblLuyKe
    )tblBuTru
	)tblLast
	left join
	(
		select hd.ID_HoaDon,
			sum(hd.PhaiThanhToan) as GiaTriTatToan
		from BH_HoaDon hd
		where hd.ChoThanhToan='0'
		and LoaiHoaDon= 42
		group by hd.ID_HoaDon
	) qtCN on tblLast.ID= qtCN.ID_HoaDon



		declare @totalRow int, @totalPage float, @SumTongThanhToan float, @SumKhachDaTra float, @SumConNo float, @SumNoThucTe float
    		select @totalRow = count(dt.ID),
    			@totalPage = CEILING(COUNT(dt.ID) / CAST(@PageSize as float )),
    			@SumTongThanhToan = sum(TongThanhToan) ,
    			@SumKhachDaTra = sum(KhachDaTra),
    			@SumConNo = sum(ConNo) 		,
				@SumNoThucTe  = sum(NoThucTe) 
    			from #tblDebit dt

 
	select *,
		@totalRow as TotalRow,
		@totalPage as TotalPage,
		@SumTongThanhToan as TongThanhToanAll,
		@SumKhachDaTra as KhachDaTraAll,
		@SumConNo as ConNoAll,
		@SumNoThucTe as NoThucTeAll,
		hd.TongThanhToan - isnull(hd.TongTienHDTra,0) as GiaTriSauTra,		
		tblNV.TenNhanViens
	from #tblDebit hd
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
	 order by NgayLapHoaDon desc
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY

	drop table #tblDebit
	drop table #tblHD

END");

			Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonBanHang]	
 --declare
 @timeStart [datetime] ='2024-01-01',
    @timeEnd [datetime]='2024-10-01',
    @ID_ChiNhanh [nvarchar](max)='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
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

	
		;with tblHD
		as
		(
			select tblDebit.*,
					iif(tblDebit.LoaiHoaDonGoc = 6, 
							iif(tblDebit.LuyKeTraHang > 0, tblDebit.TongGiaTriTra, 
								---- neu LuyKeTrahang < 0 (tức trả hàng > nợ HD cũ): BuTruTrahang = max (TongTienHang)
								iif(abs(tblDebit.LuyKeTraHang) > tblDebit.TongThanhToan, tblDebit.TongThanhToan, abs(tblDebit.LuyKeTraHang))
								),
						 tblDebit.LuyKeTraHang) as TongTienHDTra,					
						iif(tblDebit.ChoThanhToan is null,0, 
							----- hdDoi co congno < tongtra							
							tblDebit.TongThanhToan 
								--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = TongGiaTriTra							
								- iif(tblDebit.LoaiHoaDonGoc = 6, iif(tblDebit.LuyKeTraHang > 0,  tblDebit.TongGiaTriTra, iif(abs(tblDebit.LuyKeTraHang) > tblDebit.TongThanhToan, tblDebit.TongThanhToan, abs(tblDebit.LuyKeTraHang))), tblDebit.LuyKeTraHang)
								- tblDebit.KhachDaTra ) as ConNo ---- ConNo = TongThanhToan - GtriBuTru
			from
					(select 
							c.*,
							isnull(iif(c.ID_HoaDon is null,
								iif(c.ConNo1 <= 0, 0, ---  khachtra thuatien --> công nợ âm
									case when c.TongGiaTriTra > c.ConNo1 then c.ConNo1						
									else c.TongGiaTriTra end),
								(select dbo.BuTruTraHang_HDDoi(c.ID_HoaDon,NgayLapHoaDon,ID_HoaDonGoc, LoaiHoaDonGoc))				
							),0) as LuyKeTraHang
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
							hd.NguoiTao,
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
							hd.DienGiai,
							hd.NguoiTao as NguoiTaoHD,
							ISNULL(hd.TongChietKhau,0) as TongChietKhau,
							ISNULL(hd.TongTienHang,0) as TongTienHang,
							ISNULL(hd.ChiPhi,0) as TongChiPhi, --- chiphi cuahang phaitra
							iif(hd.LoaiHoaDon = 36,0,ISNULL(hd.TongGiamGia,0)) as TongGiamGia,
							iif(hd.LoaiHoaDon=36,ISNULL(hd.TongGiamGia,0),0) as SoNgayThuoc,
							ISNULL(hd.PhaiThanhToan,0) as PhaiThanhToan,							
							ISNULL(hd.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,	
							iif(hd.ID_BaoHiem is null, 2, 1) as SuDungBaoHiem,
							case  hd.ChoThanhToan
								when 1 then '1'
								when 0 then '0'
							else '4' end as TrangThaiHD,
							hdt.ID_HoaDon as ID_HoaDonGoc,
							ISNULL(hdt.LoaiHoaDon,0) as LoaiHoaDonGoc,
							Case when hdt.MaHoaDon is null then '' else hdt.MaHoaDon end as MaHoaDonGoc,
							ISNULL(allTra.TongGtriTra,0) as TongGiaTriTra,
							ISNULL(allTra.NoTraHang,0) as NoTraHang,
							isnull(sq.KhachDaTra,0) as KhachDaTra,
							isnull(sq.DaThanhToan,0) as DaThanhToan,
							isnull(sq.TienMat,0) as TienMat,
							isnull(sq.TienATM,0) as TienATM,
							isnull(sq.ChuyenKhoan,0) as ChuyenKhoan,
							isnull(sq.TienDatCoc,0) as TienDatCoc,
							isnull(sq.TienDoiDiem,0) as TienDoiDiem,
							isnull(sq.ThuTuThe,0) as ThuTuThe,
							isnull(sq.BaoHiemDaTra,0) as BaoHiemDaTra,
							iif(hd.TongThanhToan is null or hd.TongThanhToan =0, hd.PhaiThanhToan, hd.TongThanhToan) as TongThanhToan,
							iif(hd.TongThanhToan is null or hd.TongThanhToan =0, hd.PhaiThanhToan, hd.TongThanhToan) - isnull(sq.DaThanhToan,0) as ConNo1,
							Case When hd.ChoThanhToan = '1' then N'Phiếu tạm' when hd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThaiText,
							case when sq.TienMat > 0 then
							case when sq.TienATM > 0 then	
								case when sq.ChuyenKhoan > 0 then
									case when sq.ThuTuThe > 0 then '1,2,3,4' else '1,2,3' end												
									else 
										case when sq.ThuTuThe > 0 then  '1,2,4' else '1,2' end end
									else
										case when sq.ChuyenKhoan > 0 then 
											case when sq.ThuTuThe > 0 then '1,3,4' else '1,3' end
											else 
													case when sq.ThuTuThe > 0 then '1,4' else '1' end end end
							else
								case when sq.TienATM > 0 then
									case when sq.ChuyenKhoan > 0 then
											case when sq.ThuTuThe > 0 then '2,3,4' else '2,3' end	
											else 
												case when sq.ThuTuThe > 0 then '2,4' else '2' end end
										else 		
											case when sq.ChuyenKhoan > 0 then
												case when sq.ThuTuThe > 0 then '3,4' else '3' end
												else 
												case when sq.ThuTuThe > 0 then '4' else '5' end end end end
									
									as PTThanhToan
						from BH_HoaDon hd
						left join BH_HoaDon hdt on hd.ID_HoaDon= hdt.ID
						left join (
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
									qct.ID_HoaDonLienQuan,	
									case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=1, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=1, -qct.TienThu,0) end as TienMat,
									case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=2, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=2, -qct.TienThu,0) end as TienATM,
									case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=3, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=3, -qct.TienThu,0) end as TienCK,
									case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=5, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=5, -qct.TienThu,0) end as TienDoiDiem,
									case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=4, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=4, -qct.TienThu,0) end as ThuTuThe,
									case when qhd.LoaiHoaDon = 11 then iif(qct.HinhThucThanhToan=6, qct.TienThu,0) else  iif(qct.HinhThucThanhToan=6, -qct.TienThu,0) end as TienDatCoc,
									iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as TienThu,
									iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu) as KhachDaTra,
									0 as ThuDatHang,
									cast (0 as float) as BaoHiemDaTra						
								from Quy_HoaDon_ChiTiet qct
								join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 			
								where (qhd.TrangThai is null or qhd.TrangThai= '1')					
							) soquy group by soquy.ID_HoaDonLienQuan
						) sq on hd.ID= sq.ID_HoaDonLienQuan
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
					where hd.NgayLapHoadon between @timeStart and @timeEnd					
					and exists (select loai.Loai from @tblLoaiHoaDon loai where hd.LoaiHoaDon = loai.Loai)
					and exists (select cn.ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID) 	
				)c
		)tblDebit		
		),
		tblWhere
		as
		(
		select
			hd.*,
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
			-----ISNULL(dt.DienThoai, N'') as DienThoai,  ---- kangjin yêu cầu bảo mật sdt khách hàng ---
			ISNULL(dt.DiaChi, N'') as DiaChiKhachHang,						
			dt.ID_TinhThanh, 
			dt.ID_QuanHuyen,
			ISNULL(nv.TenNhanVien, N'') as TenNhanVien
		from tblHD hd		
		left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
		left join NS_NhanVien nv on hd.ID_NhanVien= nv.ID
		where ((select count(Name) from @tblSearch b where     			
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
		and exists (select hd.ID from @tblTrangThai tt where hd.TrangThaiHD= tt.TrangThaiHD)
		and (@PhuongThucThanhToan ='' or exists(SELECT Name FROM splitstring(hd.PTThanhToan) pt join @tblPhuongThuc pt2 on pt.Name = pt2.PhuongThuc))
		),
		tblSum
		as
		(
			select count(dt.ID) as TotalRow,
				CEILING(COUNT(dt.ID) / CAST(@PageSize as float ))  as TotalPage,
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
				--sum(ThanhTienChuaCK) as SumThanhTienChuaCK,
				--sum(GiamGiaCT) as SumGiamGiaCT,
				sum(TienMat) as SumTienMat,
				sum(TienATM) as SumPOS,
				sum(ChuyenKhoan) as SumChuyenKhoan,
				--sum(GiaTriSDDV) as TongGiaTriSDDV,
				sum(TongTienThue) as SumTongTienThue,
				sum(ConNo) as SumConNo,

				sum(TongTienThueBaoHiem) as SumTongTienThueBaoHiem,
				sum(TongTienBHDuyet) as SumTongTienBHDuyet,
				sum(KhauTruTheoVu) as SumKhauTruTheoVu,
				sum(GiamTruBoiThuong) as SumGiamTruBoiThuong,
				sum(BHThanhToanTruocThue) as SumBHThanhToanTruocThue
				
			from tblWhere dt
		)
		select hd.*,			
			isnull(cthd.GiamGiaCT,0) as GiamGiaCT,
			isnull(cthd.ThanhTienChuaCK,0) as ThanhTienChuaCK,
			isnull(cthd.GiaTriSDDV,0) as GiaTriSDDV,
			ISNULL(tt.TenTinhThanh, N'') as KhuVuc,
			ISNULL(qh.TenQuanHuyen, N'') as PhuongXa,
			ISNULL(dv.TenDonVi, N'') as TenDonVi,
			ISNULL(dv.DiaChi, N'') as DiaChiChiNhanh,
			ISNULL(dv.SoDienThoai, N'') as DienThoaiChiNhanh,
			ISNULL(gb.TenGiaBan,N'Bảng giá chung') AS TenBangGia,
			ISNULL(vt.TenViTri,'') as TenPhongBan,
			cast(iif(hdChuaXK.ID is null,'0','1') as bit) as IsChuaXuatKho
		from (
			select hd.*, cte.*				
			from tblWhere hd
			cross join tblSum cte			
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
			when @ColumnSort='MaKhachHang' then MaDoiTuong end ASC,
			case when @SortBy <>'DESC' then ''
			when @ColumnSort='MaKhachHang' then MaDoiTuong end DESC,
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
			--case when @SortBy <>'ASC' then 0
			--when @ColumnSort='GiaTriSDDV' then GiaTriSDDV end ASC,
			--case when @SortBy <>'DESC' then 0
			--when @ColumnSort='GiaTriSDDV' then GiaTriSDDV end DESC,
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
		)hd 		
		left join DM_DonVi dv on hd.ID_DonVi = dv.ID
		left join DM_TinhThanh tt on hd.ID_TinhThanh = tt.ID
		left join DM_QuanHuyen qh on hd.ID_QuanHuyen = qh.ID
		left join DM_GiaBan gb on hd.ID_BangGia = gb.ID
		left join DM_ViTri vt on hd.ID_ViTri = vt.ID
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
				from tblWhere hd
				join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon					
				where	(ct.ID_ChiTietDinhLuong= ct.ID or ct.ID_ChiTietDinhLuong is null)
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
				where exists (select id from tblWhere hdsd where ctsd.ID_HoaDon = hdsd.ID)
				and	(ctsd.ID_ChiTietDinhLuong= ctsd.ID or ctsd.ID_ChiTietDinhLuong is null)		
				and ctsd.ID_ChiTietGoiDV is not null				
				) cthd group by cthd.ID_HoaDon
			) cthd on hd.ID = cthd.ID_HoaDon
		left join 
		(
			select hd.ID 
			from tblWhere hd
			join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
			join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
			join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
			where hh.LaHangHoa='1' ------ hd co cthd la hanghoa ---
			and hd.LoaiHoaDon in (1,2,36) ---- banle, baohanh, hotro
			and hd.ChoThanhToan = 0
			and not exists
			 ---- hd chua co phieu xuatkho-----
				(select id 
				from BH_HoaDon hdx
				where hdx.ID_HoaDon = hd.ID 
				and hdx.LoaiHoaDon in (35,37,38,39,40)
				and hdx.ChoThanhToan is not null
				and hdx.NgayLapHoaDon > @timeStart
				)	
			group by hd.ID
		) hdChuaXK on hd.ID = hdChuaXK.ID		
		END");

			Sql(@"ALTER PROCEDURE [dbo].[GetInforHoaDon_ByID]
    @ID_HoaDon [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
    declare @IDHDGoc uniqueidentifier, @ID_DoiTuong uniqueidentifier, @LoaiHD int
    select @IDHDGoc = ID_HoaDon,  @ID_DoiTuong = ID_DoiTuong, @LoaiHD = LoaiHoaDon 
	from BH_HoaDon where ID = @ID_HoaDon

   
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
								------ nếu không check chỗ này: nó sẽ get về phiếu chi của hdTra ----
								and exists (select id from BH_HoaDon hdd where  hdd.ID = @IDHDGoc and hdd.LoaiHoaDon = 3)
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
							from BH_HoaDon hdXLy
							join BH_HoaDon hdDat on hdXLy.ID_HoaDon = hdDat.ID 								
							join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDonLienQuan = hdXLy.ID
							join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
							where (qhd.TrangThai= 1 Or qhd.TrangThai is null)		
							and hdXLy.ChoThanhToan = 0
							and hdXLy.ID_HoaDon = @ID_HoaDon 
							and hdDat.LoaiHoaDon = 3	
							and hdXLy.LoaiHoaDon in (1,25, 4)							
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
		------c.DienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng ---
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
			---- muontruong: TongTienHDTra => PhaiTraKhach (sau khi butru congno hdGoc & hdDoi) --
			---- butru > phaitt: phaiTraKhach = 0, và khachPhaiTra them phàn nay ---
			iif(isnull(cn.BuTruHDGoc_Doi,0) > tView.PhaiThanhToan, 0,tView.PhaiThanhToan - isnull(cn.BuTruHDGoc_Doi,0))  as TongTienHDTra, 
			iif(isnull(cn.BuTruHDGoc_Doi,0) > tView.PhaiThanhToan, 0, tView.PhaiThanhToan - isnull(cn.BuTruHDGoc_Doi,0) - tView.KhachDaTra)  as ConNo
			
		from #tblView tView
		left join @tblCongNo cn on tView.ID = cn.ID
		order by tView.NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetNhatKySuDung_GDV]
    @IDChiNhanhs [nvarchar](max) = null,
    @IDCustomers [nvarchar](max) = null,  
	@TextSearch nvarchar(max) = null,
	@DateFrom datetime = null,
	@DateTo datetime = null,
	@LoaiHoaDons [nvarchar](max) = null,
    @CurrentPage [int] = null,
    @PageSize [int] = null
AS
BEGIN
    SET NOCOUNT ON;
    	declare @sql nvarchar(max) ='', @where nvarchar(max), @paramDefined nvarchar(max)
    	declare @tblDefined nvarchar(max)= N' declare @tblChiNhanh table(ID uniqueidentifier)
    								declare @tblCus table(ID uniqueidentifier)
    								declare @tblCar table(ID uniqueidentifier)'
    
    	set @where = N' where 1 = 1 and hd.LoaiHoaDon in (1,2) and hd.ChoThanhToan = 0  
						and (ct.ID_ChiTietDinhLuong= ct.id OR ct.ID_ChiTietDinhLuong IS NULL) 
						and (ct.ID_ParentCombo != ct.ID OR ct.ID_ParentCombo IS NULL)'
    
    	if isnull(@CurrentPage,'') =''
    		set @CurrentPage = 0
    	if isnull(@PageSize,'') =''
    		set @PageSize = 20

		if isnull(@LoaiHoaDons,'') !=''
			begin
				if @LoaiHoaDons = 19 ---- hoadon dudung GDV
    				set @where = CONCAT(@where, N' and ct.ChatLieu = ''4''')
			end

    	if isnull(@IDChiNhanhs,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblChiNhanh cn where ID_DonVi = cn.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblChiNhanh select name from dbo.splitstring(@IDChiNhanhs_In) ;')
    		end
    	if isnull(@IDCustomers,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and exists (select ID from @tblCus cus where hd.ID_DoiTuong = cus.ID)')
    			set @sql = CONCAT(@sql, ' insert into @tblCus select name from dbo.splitstring(@IDCustomers_In) ;')
    		end
    	
    	if isnull(@DateFrom,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and hd.NgayLapHoaDon > @DateFrom_In')
    			
    		end
		if isnull(@DateTo,'') !=''
    		begin
    			set @where = CONCAT(@where , ' and hd.NgayLapHoaDon < @DateTo_In')    			
    		end

    	if isnull(@TextSearch,'') !=''
    		begin
    			set @where = CONCAT(@where , N' and (hd.MaHoaDon like N''%'' + @TextSearch_In + ''%''  
							or hdgoc.MaHoaDon like N''%'' + @TextSearch_In + ''%''  
							or dt.MaDoiTuong like N''%'' + @TextSearch_In + ''%'' or  dt.TenDoiTuong like N''%'' + @TextSearch_In + ''%'' 
							or dt.TenDoiTuong_KhongDau like N''%'' + @TextSearch_In + ''%'' or dt.DienThoai like N''%'' + @TextSearch_In + ''%''
							or hh.TenHangHoa like N''%'' + @TextSearch_In + ''%'' or  hh.TenHangHoa_KhongDau like N''%'' + @TextSearch_In + ''%''
							or qd.MaHangHoa like N''%'' + @TextSearch_In + ''%'')' )
    			
    		end
    	
    	set @sql = CONCAT(@tblDefined, @sql, N'
    		;with data_cte
    as (
		SELECT ct.ID as ID_ChiTietGoiDV,
			hd.MaHoaDon, 
			hd.NgayLapHoaDon,
			hd.ID_DoiTuong, 
			dt.MaDoiTuong,
			dt.TenDoiTuong,
    		qd.MaHangHoa,
			isnull(qd.ThuocTinhGiaTri,'''') as ThuocTinh_GiaTri,
			iif(ct.TenHangHoaThayThe is null or ct.TenHangHoaThayThe ='''', hh.TenHangHoa, ct.TenHangHoaThayThe) as TenHangHoa,
    		hh.TenHangHoa_KhongDau,		
			hh.GhiChu as GhiChuHH,
			hh.LaHangHoa,
			case when hh.LaHangHoa = 1 then ''0'' else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end PhiDichVu,
    		Case when hh.LaHangHoa=1 then ''0'' else ISNULL(hh.ChiPhiTinhTheoPT,''0'') end as LaPTPhiDichVu,
			isnull(hh.ID_NhomHang,''00000000-0000-0000-0000-000000000000'') as ID_NhomHangHoa,
			ISNULL(qd.LaDonViChuan,0) as LaDonViChuan, 
			CAST(ISNULL(qd.TyLeChuyenDoi,1) as float) as TyLeChuyenDoi,
			isnull(hh.QuanLyTheoLoHang,''0'') as QuanLyTheoLoHang,
			ISNULL(hh.ChietKhauMD_NVTheoPT,''1'') as ChietKhauMD_NVTheoPT,
			ISNULL(hh.HoaHongTruocChietKhau,0) as HoaHongTruocChietKhau,
			CAST(ISNULL(hh.QuyCach,1) as float) as QuyCach,
    		lo.MaLoHang, 
			lo.NgaySanXuat, 
			lo.NgayHetHan,
			qd.TenDonViTinh,
			qd.ID_HangHoa,
    		ct.SoLuong as SoLuongMua,
			ct.ID_DonViQuiDoi,
			ct.ID_LoHang,
			ct.DonGia - ct.TienChietKhau as GiaBan,	 ---- lay sau CK
			ct.TienChietKhau,
			ct.ThoiGianBaoHanh,
			ct.LoaiThoiGianBH,
			ct.GhiChu,
			ct.SoThuTu,
			isnull(gv.GiaVon,0) as GiaVon,
			isnull(tk.TonKho,0) as TonKho,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa=1,1,2), hh.LoaiHangHoa) as LoaiHangHoa,
			nhomhh.TenNhomHangHoa,
    		hdXMLOut.HDCT_NhanVien as NhanVienThucHien,
    		CT_ChietKhauNV.TongChietKhau,
			hdgoc.MaHoaDon as MaChungTuGoc
    	FROM BH_HoaDon_ChiTiet ct
    	join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.id
    	join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
		left join BH_HoaDon_ChiTiet ctgoc on ct.ID_ChiTietGoiDV = ctgoc.ID 
						and (ctgoc.ID_ChiTietDinhLuong= ctgoc.id OR ctgoc.ID_ChiTietDinhLuong IS NULL) 
						and (ctgoc.ID_ParentCombo != ctgoc.ID OR ctgoc.ID_ParentCombo IS NULL)
		left join BH_HoaDon hdgoc on ctgoc.ID_HoaDon = hdgoc.ID and hdgoc.LoaiHoaDon in (1,2) and hdgoc.ChoThanhToan = 0
		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		left join DM_NhomHangHoa nhomhh on hh.ID_NhomHang = nhomhh.ID
		left join DM_LoHang lo on ct.ID_LoHang = lo.ID
		left join DM_GiaVon gv on ct.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and gv.ID_DonVi = hd.ID_DonVi
		left join DM_HangHoa_TonKho tk on ct.ID_DonViQuiDoi = tk.ID_DonViQuyDoi and tk.ID_DonVi = hd.ID_DonVi
    	left join 
    			(Select distinct hdXML.ID,
    					(
    					select distinct (nv.TenNhanVien) +'', ''  AS [text()]
    					from BH_HoaDon_ChiTiet ct2
    					left join BH_NhanVienThucHien nvth on ct2.ID = nvth.ID_ChiTietHoaDon
    					left join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
    					where ct2.ID = hdXML.ID 
    					For XML PATH ('''')
    				) HDCT_NhanVien
    			from BH_HoaDon_ChiTiet hdXML) hdXMLOut on ct.ID= hdXMLOut.ID
    	 left join 
    			(select ct3.ID, SUM(isnull(nvth2.TienChietKhau,0)) as TongChietKhau from BH_HoaDon_ChiTiet ct3
    			left join BH_NhanVienThucHien nvth2 on ct3.ID = nvth2.ID_ChiTietHoaDon
    			group by ct3.ID) CT_ChietKhauNV on CT_ChietKhauNV.ID = ct.ID        
    	', @where, 
    		'),
    		count_cte
    		as (
    			select count(ID_ChiTietGoiDV) as TotalRow,
    				CEILING(COUNT(ID_ChiTietGoiDV) / CAST(@PageSize_In as float ))  as TotalPage,
    				sum(SoLuongMua) as TongSoLuong,
    				sum(TongChietKhau) as TongHoaHong			
    			from data_cte
    		)
    	select dt.*,
    		cte.*
    		from data_cte dt			
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage_In * @PageSize_In) ROWS
    		FETCH NEXT @PageSize_In ROWS ONLY ')
    
    		print @sql
    
    		set @paramDefined =N'
    			@IDChiNhanhs_In nvarchar(max),
    			@IDCustomers_In nvarchar(max),
				@TextSearch_In nvarchar(max),
				@DateFrom_In datetime,
				@DateTo_In datetime,
				@LoaiHoaDons_In nvarchar(max),
    			@CurrentPage_In int,
    			@PageSize_In int'
    
    		exec sp_executesql @sql, 
    		@paramDefined,
    		@IDChiNhanhs_In = @IDChiNhanhs,
    		@IDCustomers_In = @IDCustomers,
			@TextSearch_In = @TextSearch,
			@DateFrom_In = @DateFrom,
			@DateTo_In = @DateTo,
			@LoaiHoaDons_In = @LoaiHoaDons,
    		@CurrentPage_In = @CurrentPage,
    		@PageSize_In = @PageSize
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListTheGiaTri]
    @IDDonVis [nvarchar](max),
	@LoaiHoaDons varchar(20) = null, --- 
    @TextSearch [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TrangThais [nvarchar](10),
    @MucNapFrom [float],
    @MucNapTo [float],
    @KhuyenMaiFrom [float],
    @KhuyenMaiTo [float],
    @KhuyenMaiLaPTram [int],
    @ChietKhauFrom [float],
    @ChietKhauTo [float],
    @ChietKhauLaPTram [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;

	declare @tblLoaiHD table(LoaiHoaDon int)
	insert into @tblLoaiHD
	select Name from dbo.splitstring(@LoaiHoaDons)
    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @MucNapMax float= (select max(TongChiPhi) from BH_HoaDon where ChoThanhToan= 0 and LoaiHoaDon= 22 );
    	if @MucNapTo is null
    		set @MucNapTo= @MucNapMax
    	if @KhuyenMaiTo is null
    		set @KhuyenMaiTo = @MucNapMax
    	if @ChietKhauTo is null
    		set @ChietKhauTo= @MucNapMax;
    	
    	with data_cte
    	as
    	(
    
    	select tblThe.ID,
			tblThe.MaHoaDon,
			tblThe.NgayLapHoaDon,
			tblThe.LoaiHoaDon,
			tblThe.NgayTao,
    		tblThe.TongChiPhi as MucNap,    		
    		tblThe.TongTienHang as TongTienNap,
    		tblThe.TongTienThue as SoDuSauNap,
			tblThe.TongGiamGia  as ChietKhauVND,    	
    		ISNULL(tblThe.DienGiai,'') as GhiChu,
    		tblThe.NguoiTao,
    		ISNULL(tblThe.ID_DoiTuong,'00000000-0000-0000-0000-000000000000') as ID_DoiTuong,
    		tblThe.PhaiThanhToan,
    		tblThe.MaDoiTuong as MaKhachHang,
    		tblThe.TenDoiTuong as TenKhachHang,
    		------tblThe.DienThoai as SoDienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng ---
    		tblThe.DiaChi as DiaChiKhachHang,
    		tblThe.ChoThanhToan,
    		tblThe.ChietKhauPT,
    		tblThe.KhuyenMaiPT,
			tblThe.ID_DonVi,		
			isnull(soquy.TongPhiNganHang,0) as KhuyenMaiVND, -- muontamtruong (trừ phí ngân hàng khi tính hoa hồng NV)
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienMat,0),-ISNULL(soquy.TienMat,0))  as TienMat,
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienPOS,0),-ISNULL(soquy.TienPOS,0))  as TienATM,
			iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienCK,0),-ISNULL(soquy.TienCK,0))  as TienGui,
    		iif(tblThe.LoaiHoaDon= 22, ISNULL(soquy.TienThu,0),-ISNULL(soquy.TienThu,0)) as KhachDaTra,
			--iif(tblThe.LoaiHoaDon= 22,  tblThe.PhaiThanhToan - ISNULL(soquy.TienThu,0),-ISNULL(soquy.TienThu,0) + tblThe.PhaiThanhToan) as ConNo1,
    		dv.TenDonVi,
    		dv.SoDienThoai as DienThoaiChiNhanh,
    		dv.DiaChi as DiaChiChiNhanh
    	from
    		(
    		select *
    		from
    			(select hd.ID, 
						hd.MaHoaDon,
						hd.LoaiHoaDon,
						hd.NgayLapHoaDon,
						hd.ID_DonVi,
						hd.ID_DoiTuong,
						hd.ID_NhanVien,
						hd.NguoiTao,
						hd.NgayTao,
						hd.TongChiPhi,
						hd.TongTienHang,
						hd.TongChietKhau,
						hd.TongGiamGia,
						hd.TongTienThue,
						hd.ChoThanhToan,		
						hd.PhaiThanhToan,						
						hd.DienGiai,
						----- Loai 32: hoanthe (TongChietKhau = % PhiHoanThe)
						iif(hd.LoaiHoaDon= 32,hd.TongChietKhau, iif(hd.TongChiPhi=0,0, hd.TongGiamGia/hd.TongChiPhi * 100)) as ChietKhauPT,
						iif(hd.TongChiPhi=0,0, hd.TongChietKhau/hd.TongChiPhi * 100) as KhuyenMaiPT,
    					dt.MaDoiTuong, dt.TenDoiTuong,
    					dt.DienThoai, 
    					dt.DiaChi,
    					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai --,
    					--NhanVienThucHien
    				from BH_HoaDon hd
    				join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    			
    				where exists (select name from dbo.splitstring(@IDDonVis) dv where hd.ID_DonVi= dv.Name)	
    				and hd.LoaiHoaDon in (select LoaiHoaDon from @tblLoaiHD)
    				and hd.TongChiPhi >= @MucNapFrom and hd.TongChiPhi <= @MucNapTo -- mucnap
    				and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon <=@ToDate
    					AND ((select count(Name) from @tblSearchString b where 
    					dt.MaDoiTuong like '%'+b.Name+'%' 
    					or dt.TenDoiTuong like '%'+b.Name+'%' 
    						or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%' 
    					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    						or dt.DienThoai like '%'+b.Name+'%'			
    					or hd.MaHoaDon like '%' +b.Name +'%' 
    						or hd.NguoiTao like '%' +b.Name +'%' 				
    						)=@count or @count=0)	
    			) the
    			where IIF(@KhuyenMaiLaPTram = 1, the.TongChietKhau, the.KhuyenMaiPT) >= @KhuyenMaiFrom -- khuyenmai
    				and IIF(@KhuyenMaiLaPTram = 1, the.TongChietKhau, the.KhuyenMaiPT) <= @KhuyenMaiTo
    				and IIF(@ChietKhauLaPTram = 1, the.TongGiamGia, the.ChietKhauPT) >= @ChietKhauFrom -- giamgia
    				and IIF(@ChietKhauLaPTram = 1, the.TongGiamGia, the.ChietKhauPT) <= @ChietKhauTo
    				and the.TrangThai like @TrangThais 
    		) tblThe		
    	join DM_DonVi dv on tblThe.ID_DonVi= dv.ID
    	left join ( select quy.ID_HoaDonLienQuan, 
    					sum(quy.TienThu) as TienThu,
    					sum(quy.TienMat) as TienMat,
    					sum(quy.TienPOS) as TienPOS,
    					sum(quy.TienCK) as TienCK,
						sum(quy.TongPhiNganHang) as TongPhiNganHang ----- 
    				from
    				(
    					select qct.ID_HoaDonLienQuan,
							iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu) as TienThu,
    						iif(qct.HinhThucThanhToan = 1, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0) as TienMat,					
    						iif(qct.HinhThucThanhToan = 2, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0) as TienPOS,
    						iif(qct.HinhThucThanhToan = 3, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0)  as TienCK,
    						iif(qct.HinhThucThanhToan = 2,iif(qct.LaPTChiPhiNganHang='0',qct.ChiPhiNganHang, 
							qct.TienThu * qct.ChiPhiNganHang/100),0) as TongPhiNganHang ---- apply pos					
    					from Quy_HoaDon_ChiTiet qct
    					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    					where qhd.TrangThai= 1 or qhd.TrangThai is null
    				) quy 
    				group by quy.ID_HoaDonLienQuan) soquy on tblThe.ID= soquy.ID_HoaDonLienQuan
    	),
    	count_cte
    	as (
    		select count(ID) as TotalRow,
    			CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    			sum(MucNap) as TongMucNapAll,
    			sum(KhuyenMaiVND) as TongKhuyenMaiAll,
    			sum(TongTienNap) as TongTienNapAll,			
    			sum(ChietKhauVND) as TongChietKhauAll,
    			sum(SoDuSauNap) as SoDuSauNapAll,
    			sum(PhaiThanhToan) as PhaiThanhToanAll,			
    			sum(TienMat) as TienMatAll,
    			sum(TienATM) as TienATMAll,
    			sum(TienGui) as TienGuiAll,
    			sum(KhachDaTra) as KhachDaTraAll
    			from data_cte
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
				ISNULL(qtCN.GiaTriTatToan,0) as GiaTriTatToan,
				ISNULL(nvThucHien.NhanVienThucHien,'') as NhanVienThucHien,
			 	iif(hd.ChoThanhToan is null,0, hd.PhaiThanhToan - hd.KhachDaTra - ISNULL(qtCN.GiaTriTatToan,0)) as ConNo
			from tView hd
			left join (
    				Select distinct
    					(
    						Select distinct nv.TenNhanVien + ',' AS [text()]
    						From dbo.BH_NhanVienThucHien th
    						join dbo.NS_NhanVien nv on th.ID_NhanVien = nv.ID
    						where th.ID_HoaDon= nvth.ID_HoaDon
    						For XML PATH ('')
    					) NhanVienThucHien, nvth.ID_HoaDon
    					From dbo.BH_NhanVienThucHien nvth
    					) nvThucHien on hd.ID = nvThucHien.ID_HoaDon
			left join
			(
				select hd.ID_HoaDon,
					sum(hd.TongTienHang) as GiaTriTatToan
				from BH_HoaDon hd
				where hd.ChoThanhToan='0'
				and LoaiHoaDon= 42
				group by hd.ID_HoaDon
			) qtCN on hd.ID= qtCN.ID_HoaDon

END");

			Sql(@"ALTER PROCEDURE [dbo].[GetQuyChiTiet_byIDQuy]
    @ID [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
	declare @ngaylapPhieuThu datetime = (select top 1 NgayLapHoaDon from Quy_HoaDon where ID= @ID)

	---- get allhoadon lienquan by idSoQuy
	select distinct ID_HoaDonLienQuan into #tblHoaDon
	from Quy_HoaDon_ChiTiet qct
	where qct.ID_HoaDon = @ID	

	---- get phieuthu/chi lienquan hoadon
		select 
			qct.ID_HoaDonLienQuan,
			qct.ID_DoiTuong,
			sum(qct.TienThu) as DaThuTruoc
	into #tblThuTruoc
	 from Quy_HoaDon qhd
    join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
	where exists
		(select qct2.ID_HoaDonLienQuan from #tblHoaDon qct2 
		where qct.ID_HoaDonLienQuan = qct2.ID_HoaDonLienQuan)
	and qhd.ID != @ID
	and qhd.TrangThai ='1'
	group by qct.ID_HoaDonLienQuan,qct.ID_DoiTuong

	---- if hd xuly from dathang --> get infor hd dathang
	select 
		hdd.ID, hdMua.ID as ID_HoaDonMua, hdMua.NgayLapHoaDon into #tblDat
	from
	 (
		 select hd.ID, hd.ID_HoaDon, hd.NgayLapHoaDon
		from #tblHoaDon tmp
		join BH_HoaDon hd on tmp.ID_HoaDonLienQuan= hd.ID
	 ) hdMua
	 join BH_HoaDon hdd  on hdd.ID = hdMua.ID_HoaDon
	 where hdd.LoaiHoaDon = 3 and hdd.ChoThanhToan='0'

	
	---- get phieuthu from dathang
		select thuDH.ID_HoaDonMua, 
				thuDH.ID_DoiTuong,
				thuDH.ThuDatHang
		into #tblThuDH
			from
			(
				select tblDH.ID_HoaDonMua,
					tblDH.ID_DoiTuong,
					sum(tblDH.TienThu) as ThuDatHang,		
					ROW_NUMBER() OVER(PARTITION BY tblDH.ID ORDER BY tblDH.NgayLapHoaDon ASC) AS isFirst	--- group by hdDat, sort by ngaylap hdxuly
				from
				(			
						select hdd.ID_HoaDonMua, hdd.NgayLapHoaDon,		
							hdd.ID,
							qct.ID_DoiTuong,
							iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu) as TienThu			
						from Quy_HoaDon_ChiTiet qct
						join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
						join #tblDat hdd on hdd.ID= qct.ID_HoaDonLienQuan				
						where (qhd.TrangThai= 1 Or qhd.TrangThai is null)
				) tblDH group by tblDH.ID_HoaDonMua, tblDH.ID,tblDH.NgayLapHoaDon, tblDH.ID_DoiTuong
		) thuDH where thuDH.isFirst= 1 

	---- get chiphi dichvu NCC
	select 
			cp.ID_HoaDon,
			sum(cp.ThanhTien) as PhaiThanhToan
		into #tblChiPhi
		from BH_HoaDon_ChiPhi cp
		where exists (select * from #tblHoaDon hd where cp.ID_HoaDon = hd.ID_HoaDonLienQuan)	
		group by cp.ID_HoaDon

    select qhd.id, qct.ID_HoaDon, qhd.MaHoaDon, qhd.NguoiTao, qhd.LoaiHoaDon, qhd.TongTienThu, qhd.ID_NhanVien, qhd.NoiDungThu,
		 qhd.ID_DonVi,	qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo,qhd.NguoiSua, isnull(qhd.TrangThai, '1') as TrangThai,
    	  iif(qct.HinhThucThanhToan=1, qct.TienThu,0) as TienMat, 
		  iif(qct.HinhThucThanhToan=2 or qct.HinhThucThanhToan=3 , qct.TienThu,0) as TienGui, 
			qct.TienThu, qct.DiemThanhToan,
			qct.ID_TaiKhoanNganHang,
			qct.ID_KhoanThuChi, 
		   qhd.NgayLapHoaDon as NgayLapPhieuThu,
    	   qct.ID_DoiTuong,
		   qct.ID_BangLuongChiTiet,
    	   qct.ID_HoaDonLienQuan,
    	   qct.ID_NhanVien as ID_NhanVienCT, -- thu/chi cho NV nao
    	   qct.HinhThucThanhToan,
    	   cast(iif(qhd.LoaiHoaDon = 11,'1','0') as bit) as LaKhoanThu,
    	   iif(qct.LoaiThanhToan = 1,1,0) as LaTienCoc,
    	   isnull(hd.MaHoaDon,N'Thu thêm') as MaHoaDonHD,    	  
		   nv.TenNhanVien,
		   iif(qct.ID_NhanVien is null,dt.MaDoiTuong, nv2.MaNhanVien) as MaDoiTuong, 
		   iif(qct.ID_NhanVien is null,dt.TenDoiTuong, nv2.TenNhanVien) as NguoiNopTien, 	
		   iif(qct.ID_NhanVien is null, dt.DienThoai, nv2.DienThoaiDiDong) as SoDienThoai,
		   iif(qct.ID_NhanVien is null, dt.LoaiDoiTuong,5) as LoaiDoiTuong,  
    	   iif(qhd.TrangThai ='0', N'Đã hủy', N'Đã thanh toán') as GhiChu,	  
    	   iif(hd.NgayLapHoaDon is null, qhd.NgayLapHoaDon, hd.NgayLapHoaDon) as NgayLapHoaDon,
    	   case qct.HinhThucThanhToan
    			when 1 then  N'Tiền mặt'
    			when 2 then  N'POS'
    			when 3 then  N'Chuyển khoản'
    			when 4 then  N'Thu từ thẻ'
    			when 5 then  N'Đổi điểm'
    			when 6 then  N'Thu từ cọc'
    		end as PhuongThuc,
			ktc.NoiDungThuChi,
			iif(ktc.LaKhoanThu is null,  IIF(qhd.LoaiHoaDon=11,'1','0'), ktc.LaKhoanThu) as LaKhoanThu,
			iif(tk.TaiKhoanPOS ='1',tk.TenChuThe,'') as TenTaiKhoanPOS,
			iif(tk.TaiKhoanPos ='0',tk.TenChuThe,'') as TenTaiKhoanNOTPOS,	
			isnull(hd.LoaiHoaDon,0) as LoaiHoaDonHD,
			isnull(iif(dt.LoaiDoiTuong =3, hd.TongTienThueBaoHiem,iif(hd.TongThueKhachHang >0, hd.TongThueKhachHang, hd.TongTienThue)),0) as TongTienThue,
			iif(dt.LoaiDoiTuong= 3, hd.PhaiThanhToanBaoHiem, 
			iif(hd.LoaiHoaDon = 22, hd.PhaiThanhToan - isnull(phieuTatToanTGT.GiaTriTatToan,0), hd.PhaiThanhToan)) as TongThanhToanHD,			
			isnull(thu.DaThuTruoc,0) as DaThuTruoc,
			tk.TaiKhoanPOS,
			nh.TenNganHang,
			nh.MacDinh,

			----- lấy ra để tính chi phí cà thẻ (hoa hồng NV) ---- 
			qct.ChiPhiNganHang as ChiPhiThanhToan,
			qct.LaPTChiPhiNganHang as TheoPhanTram, 
			qct.ThuPhiTienGui as ThuPhiThanhToan
		
			
    from Quy_HoaDon qhd
    join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    left join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
	left join #tblChiPhi cp on hd.ID= cp.ID_HoaDon and qct.ID_HoaDonLienQuan = cp.ID_HoaDon
	left join 
		(select ID_HoaDon, sum(PhaiThanhToan) as GiaTriTatToan
		from BH_HoaDon where LoaiHoaDon= 42 and ChoThanhToan='0' 
		group by ID_HoaDon
		) phieuTatToanTGT on hd.ID= phieuTatToanTGT.ID_HoaDon
    left join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
	left join NS_NhanVien nv on qhd.ID_NhanVien= nv.ID
	left join NS_NhanVien nv2 on qct.ID_NhanVien= nv2.ID
	left join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi = ktc.ID
	left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID
	left join DM_NganHang nh on tk.ID_NganHang = nh.ID
	left join (
		select 
			thutruoc.ID_HoaDonLienQuan,
			thutruoc.ID_DoiTuong,
			sum(isnull(DaThuTruoc,0)) as DaThuTruoc
		from
		(
		select tmp.ID_HoaDonLienQuan,tmp.ID_DoiTuong, isnull(tmp.DaThuTruoc,0) as DaThuTruoc
		from #tblThuTruoc tmp 
		union all
		select thuDH.ID_HoaDonMua, thuDH.ID_DoiTuong, isnull(thuDH.ThuDatHang,0) as DaThuTruoc
		from #tblThuDH thuDH 
		) thutruoc group by thutruoc.ID_HoaDonLienQuan, thutruoc.ID_DoiTuong
	) thu on thu.ID_HoaDonLienQuan = qct.ID_HoaDonLienQuan and thu.ID_DoiTuong = qct.ID_DoiTuong
    where qhd.ID= @ID
	----- order by taikhoanganhang: thanh toán cùng 1 ngân hàng, nhưng giá trị tiền # nhau --> hiện tách riêng ở  giao diện
	order by hd.NgayLapHoaDon, qct.ID_TaiKhoanNganHang
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
    
			----- bang tam chua DS phieu thu theo Ngay timkiem
			----- không groupby: vì phải lấy chi phí POS theo từng dòng
		select qct.ID_HoaDonLienQuan, 
			qhd.ID,
			qhd.NgayLapHoaDon, 		
			---- thanhtoan = TGT: nhung van chon NV thuchien
			iif(qct.HinhThucThanhToan in (4,5),0, iif( qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu)) as ThucThu,
			---- chi get chiphi with POS ----
			iif(qct.HinhThucThanhToan != 2,0, 1) as CountTaiKhoanPos,	
			iif(qct.HinhThucThanhToan != 2,0, qct.TienThu) as ThuPOS,		
			iif(qct.HinhThucThanhToan != 2,0, iif(qct.LaPTChiPhiNganHang='0', qct.ChiPhiNganHang,  
					qct.ChiPhiNganHang * qct.TienThu/100)) as ChiPhiNganHang					
    	into #tempQuy
    	from Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID 
		where (qhd.TrangThai is null or qhd.TrangThai = '1')
		and qhd.ID_DonVi in (select ID from @tblChiNhanh)
		and qhd.NgayLapHoaDon >= @DateFrom
    	and qhd.NgayLapHoaDon < @DateTo 


			------- tinhtongChiPhiNganHang theo phieuthu -----		
		select 
			qhd.ID,
			sum(ThuPOS) as TongThuPos,
			------ nếu chỉ có 1 taikhoan POS, lấy chính giátrị đó, ngược lại: lấy theo % ---
			iif(sum(CountTaiKhoanPos) =1, max(ChiPhiNganHang),  iif(sum(ThuPOS) = 0, 0,  sum(ChiPhiNganHang)/ sum(ThuPOS) * 100 )) as ChiPhiNganHang
		into #qhdChiPhiPos
		from #tempQuy qhd
		group by qhd.ID


		------- thucthu theo hoadon ----
		select ctquy.*, tblTong.TongThuThucTe
		into #tblQuyThucTe
		from (
			----- chiphipos theo hoadon --- 
			select
				qhd.ID,
				qhd.ID_HoaDonLienQuan,
				qhd.NgayLapHoaDon,
				cpPos.ChiPhiNganHang, ---- % hoặc vnd ---
				sum(ThucThu) as ThucThu,				
				iif(sum(qhd.CountTaiKhoanPos) = 1, cpPos.ChiPhiNganHang, cpPos.ChiPhiNganHang * sum(ThuPOS)/100) as TongChiPhiNganHang
				from #tempQuy qhd
				left join #qhdChiPhiPos cpPos on qhd.ID = cpPos.ID	
			group by qhd.ID_HoaDonLienQuan, qhd.NgayLapHoaDon, qhd.ID,
				cpPos.ChiPhiNganHang
		) ctquy
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
    			----ISNULL(dt.DienThoai,'') as DienThoaiKH,		
				'' as DienThoaiKH,		----- kangjin yêu cầu bảo mật SDT khách hàng --
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

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoGoiDichVu_BanDoiTra]
    @IDChiNhanhs [nvarchar](max),
    @FromDate [datetime],
    @ToDate [datetime],
    @TxtMaHD [nvarchar](max),
    @TxtDVMua [nvarchar](max),
    @TxtDVDoi [nvarchar](max),
    @ThoiHanSuDung [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	if isnull(@IDChiNhanhs,'')!=''
    	insert into @tblChiNhanh
    	select Name from dbo.splitstring(@IDChiNhanhs)
    	else set @IDChiNhanhs =''
    	
    	if isnull(@TxtMaHD,'') !='' set @TxtMaHD = concat(N'%', @TxtMaHD,'%') else set @TxtMaHD ='%%'
    	if isnull(@TxtDVMua,'') !='' set @TxtDVMua = concat(N'%', @TxtDVMua,'%') else set @TxtDVMua ='%%'
    	if isnull(@TxtDVDoi,'') !='' set @TxtDVDoi = concat(N'%', @TxtDVDoi,'%') else set @TxtDVDoi ='%%'
    
    
    
    	------- cthd mua goc ----
    	select hd.*,
    		ctm.ID as IDChiTietHD,
    		ctm.ID_DonViQuiDoi,
    		ctm.SoLuong,
    		ctm.ThanhTien
    	into #cthdMuaGoc
    	from
    	(
    		select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.ID_DoiTuong, hd.LoaiHoaDon	
    		from BH_HoaDon hd
    		where hd.ChoThanhToan='0'
    		and hd.LoaiHoaDon= 19
    		and hd.ID_HoaDon is null ----- chỉ get gdvMua (không đổi) ---
    		and hd.NgayLapHoaDon between @FromDate and @ToDate
    		and (@IDChiNhanhs ='' or exists (select id from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
    	)hd 
    	join BH_HoaDon_ChiTiet ctm on hd.ID = ctm.ID_HoaDon
    	where (ctm.ID_ChiTietDinhLuong is null or ctm.ID_ChiTietDinhLuong = ctm.ID) --- khong lay tpdluong
    	and (ctm.ID_ParentCombo is null or ctm.ID_ParentCombo != ctm.ID) --- khong lay combo (parent)
    
    
    	------- cthdDoi ----
    	select 
    		hd.*,
    		ctDoi.ID as IDChiTietHD,
    		ctDoi.ID_DonViQuiDoi,
    		ctDoi.SoLuong,		
    		ctDoi.ThanhTien
    	into #cthdDoi
    	from
    	(
    	select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.ID_DoiTuong, hd.ID_HoaDon, hd.LoaiHoaDon
    	from BH_HoaDon hd
    	where hd.ChoThanhToan='0'
    	and hd.LoaiHoaDon= 19
    	and hd.ID_HoaDon is not null
    	and hd.NgayLapHoaDon between @FromDate and @ToDate
		and (@IDChiNhanhs ='' or exists (select id from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
    	)hd
    	join BH_HoaDon_ChiTiet ctDoi on hd.ID = ctDoi.ID_HoaDon
    	where (ctDoi.ID_ChiTietDinhLuong is null or ctDoi.ID_ChiTietDinhLuong = ctDoi.ID) --- khong lay tpdluong
    	and (ctDoi.ID_ParentCombo is null or ctDoi.ID_ParentCombo != ctDoi.ID) --- khong lay combo (parent)
    
    	---- cthdTra----
    	select 
    		hd.*,
    		ctTra.ID as IDChiTietHD,
    		ctTra.ID_DonViQuiDoi,
    		ctTra.SoLuong,
    		ctTra.ThanhTien
    	into #cthdTra
    	from
    	(
    		select hd.ID, hd.NgayLapHoaDon, hd.MaHoaDon, hd.ID_DoiTuong, hd.ID_HoaDon, hd.LoaiHoaDon	
    		from BH_HoaDon hd
    		where hd.ChoThanhToan='0'
    		and hd.LoaiHoaDon= 6
    		and hd.ID_HoaDon is not null
    		and hd.NgayLapHoaDon between @FromDate and @ToDate
			and (@IDChiNhanhs ='' or exists (select id from @tblChiNhanh cn where hd.ID_DonVi = cn.ID))
    	)hd
    	join BH_HoaDon_ChiTiet ctTra on hd.ID = ctTra.ID_HoaDon
    	where (ctTra.ID_ChiTietDinhLuong is null or ctTra.ID_ChiTietDinhLuong = ctTra.ID) --- khong lay tpdluong
    	and (ctTra.ID_ParentCombo is null or ctTra.ID_ParentCombo != ctTra.ID) --- khong lay combo (parent)
    
    		; with ctDoiTra
    			as(
    			------ join tra - doi ----
    			select 
    				tra.ID as GDVTra_ID,
    				tra.ID_HoaDon as GDVTra_IDHoaDonGoc,
    				tra.ID_DoiTuong,
    				tra.IDChiTietHD as GDVTra_IDChiTietHD,
    				tra.MaHoaDon as  GDVTra_MaHoaDon,
    				tra.NgayLapHoaDon as GDVTra_NgayLapHoaDon,		
    				tra.ID_DonViQuiDoi as GDVTra_ID_DonViQuiDoi,
    				tra.SoLuong as SoLuongTra,
    				tra.ThanhTien as GiaTriTra,
    				
    				doi.ID as GDVDoi_ID,
    				doi.IDChiTietHD as GDVDoi_IDChiTietHD,
    				doi.MaHoaDon as GDVDoi_MaHoaDon,
    				doi.NgayLapHoaDon GDVDoi_NgayLapHoaDon,	
    				doi.ID_DonViQuiDoi as GDVDoi_ID_DonViQuiDoi,
    				isnull(doi.SoLuong,0) as SoLuongDoi,
    				isnull(doi.ThanhTien,0) as GiaTriDoi				
    			from #cthdTra tra
    			left join #cthdDoi doi on tra.ID = doi.ID_HoaDon 
    			),
    			tblSumDoiTra
    			as
    			(
    				select 
    					GDVDoi_ID,
    					GDVTra_ID,
    					sum(GiaTriTra) as TongTra,
    					sum(GiaTriDoi) as TongDoi
    				from
    				(
    					select 
    						GDVDoi_ID,
    						GDVTra_ID,
    						iif(RnTra >1, 0, GiaTriTra) as GiaTriTra,
    						iif(RnDoi >1, 0, GiaTriDoi) as GiaTriDoi
    					from
    					(
    						----- trả 1 đổi N - hoặc trả N đổi 1 --> chỉ lấy dòng đầu tiên theo idchitiet --
    						select 
    							GDVDoi_ID,
    							GDVTra_ID,
    							GiaTriTra,
    							GiaTriDoi,
    							ROW_NUMBER() over (partition by GDVTra_IDChiTietHD order by GDVTra_IDChiTietHD) as RnTra,
    							ROW_NUMBER() over (partition by GDVDoi_IDChiTietHD order by GDVDoi_IDChiTietHD) as RnDoi
    						 from ctDoiTra
    					)tblRn
    				)tblGr
    				group by GDVDoi_ID,	GDVTra_ID
    			),
    			tblSumMuaGoc
    			as
    			(
    				select ID,
    					sum(ThanhTien) as TongMua
    				from #cthdMuaGoc
    				group by ID
    			),
    			tblUnion as
    			(
    				------ union doitra - muagoc --
    					select 
    						ctDoiTra.GDVTra_ID,
    						ctDoiTra.GDVTra_IDHoaDonGoc,
    						ctDoiTra.GDVTra_IDChiTietHD,						
    						ctDoiTra.ID_DoiTuong,
    						ctDoiTra.GDVTra_MaHoaDon,
    						ctDoiTra.GDVTra_NgayLapHoaDon,	
    						ctDoiTra.GDVTra_ID_DonViQuiDoi,					
    						ctDoiTra.SoLuongTra,
    						ctDoiTra.GiaTriTra,
    
    						ctDoiTra.GDVDoi_ID,
    						ctDoiTra.GDVDoi_IDChiTietHD,
    						ctDoiTra.GDVDoi_MaHoaDon,
    						ctDoiTra.GDVDoi_NgayLapHoaDon,
    						ctDoiTra.GDVDoi_ID_DonViQuiDoi,
    						ctDoiTra.SoLuongDoi,
    						ctDoiTra.GiaTriDoi,
    
    						tblSumDoiTra.TongDoi - tblSumDoiTra.TongTra as GiaTriChenhLech
    					from ctDoiTra
    					left join tblSumDoiTra on ctDoiTra.GDVDoi_ID = tblSumDoiTra.GDVDoi_ID and ctDoiTra.GDVTra_ID = tblSumDoiTra.GDVTra_ID
    
    					union all
    
    					select
    						null as GDVTra_ID,
    						null as GDVTra_IDHoaDonGoc,
    						null as GDVTra_IDChiTietHD,
    						mua.ID_DoiTuong,
    						'' as GDVTra_MaHoaDon,
    						null as GDVTra_NgayLapHoaDon,				
    						null as GDVTra_ID_DonViQuiDoi,
    						0 as SoLuongTra,
    						0 as GiaTriTra,				
    
    						mua.ID as GDVDoi_ID,
    						mua.IDChiTietHD as GDVDoi_IDChiTietHD,
    						mua.MaHoaDon as GDVDoi_MaHoaDon,
    						mua.NgayLapHoaDon as GDVDoi_NgayLapHoaDon,					
    						mua.ID_DonViQuiDoi as GDVDoi_ID_DonViQuiDoi,
    						mua.SoLuong as SoLuongDoi,
    						mua.ThanhTien as GiaTriDoi,
    						tblSum.TongMua as GiaTriChenhLech						
    					from #cthdMuaGoc mua
    					join tblSumMuaGoc tblSum on mua.ID = tblSum.ID
    				),
    				tblLast
    				as(
    					select 
    						dt.MaDoiTuong,
    						dt.TenDoiTuong,
    						tbl.*,
    						isnull(qdTra.MaHangHoa,'') as MaHangHoa,
    						hhTra.TenHangHoa,
    						qdTra.TenDonViTinh,
    
    						qdMua.MaHangHoa as GDVDoi_MaHangHoa,
    						hhMua.TenHangHoa as GDVDoi_TenHangHoa,
    						qdMua.TenDonViTinh as GDVDoi_TenDonViTinh,
    						gdvGoc.MaHoaDon as GDVTra_MaChungTuGoc,
    
    						iif(GDVTra_NgayLapHoaDon is null, GDVDoi_NgayLapHoaDon,GDVTra_NgayLapHoaDon) as NgayLapHoaDon,
    						ROW_NUMBER() over (partition by GDVTra_IDChiTietHD order by GDVTra_IDChiTietHD) as RnTra,
    						ROW_NUMBER() over (partition by GDVDoi_IDChiTietHD order by GDVDoi_IDChiTietHD) as RnDoi,
    						-----nếu chỉ đổi, giá trị chênh lệch chỉ get 1 dòng đầu tiên của GDV đổi --> used to xuất excel ---
    						ROW_NUMBER() over (partition by GDVDoi_ID order by GDVDoi_ID) as RnGDV_Doi
    
    					from tblUnion tbl
    					left join BH_HoaDon gdvGoc on tbl.GDVTra_IDHoaDonGoc = gdvGoc.ID
    					join DM_DoiTuong dt on tbl.ID_DoiTuong = dt.ID
    					left join DonViQuiDoi qdTra on tbl.GDVTra_ID_DonViQuiDoi = qdTra.ID
    					left join DM_HangHoa hhTra on qdTra.ID_HangHoa = hhTra.ID
    					left join DonViQuiDoi qdMua on tbl.GDVDoi_ID_DonViQuiDoi = qdMua.ID
    					left join DM_HangHoa hhMua on qdMua.ID_HangHoa = hhMua.ID
    					where (@TxtMaHD ='%%' 
    						or MaDoiTuong like @TxtMaHD
    						or TenDoiTuong like @TxtMaHD
    						or TenDoiTuong_KhongDau like @TxtMaHD	
    						or GDVTra_MaHoaDon like @TxtMaHD		
    						or GDVDoi_MaHoaDon like @TxtMaHD		
    						or gdvGoc.MaHoaDon  like @TxtMaHD
    						)
    						and (
    							@TxtDVMua ='%%' 
    							or qdTra.MaHangHoa like @TxtDVMua
    							or hhTra.TenHangHoa like @TxtDVMua
    							or hhTra.TenHangHoa_KhongDau like @TxtDVMua	
    
    							or qdMua.MaHangHoa like @TxtDVMua
    							or hhMua.TenHangHoa like @TxtDVMua
    							or hhMua.TenHangHoa_KhongDau like @TxtDVMua		
    						)
    					),
    					count_cte
    					as(
    						select count(*) as ToTalRow
    						from tblLast
    					)
    					
    					select
    						ToTalRow,
    						tblLast.NgayLapHoaDon,
    						MaDoiTuong,
    						TenDoiTuong,
    						GDVTra_ID,
    						GDVTra_IDChiTietHD,						
    						GDVTra_MaHoaDon,
    						GDVTra_NgayLapHoaDon,		
    						GDVTra_MaChungTuGoc,
    
    						GDVDoi_ID,
    						GDVDoi_IDChiTietHD,
    						GDVDoi_MaHoaDon,
    						GDVDoi_NgayLapHoaDon,
    						GiaTriChenhLech,
    						----- neu chỉ đổi: get chenhlec from GDVDoi, else: get from Tra ---
    						iif(GDVTra_ID is null, iif(RnGDV_Doi > 1, 0, GiaTriChenhLech), iif(RnTra > 1, 0, GiaTriChenhLech)) as GiaTriChenhLechExcel,
    						RnTra,
    						RnDoi,
    						RnGDV_Doi,
    
    						iif(RnTra > 1, '', MaHangHoa) as MaHangHoa,
    						iif(RnTra > 1, '', TenHangHoa) as TenHangHoa,
    						iif(RnTra > 1, '', TenDonViTinh) as TenDonViTinh,
    						iif(RnTra > 1, 0, SoLuongTra) as SoLuongTra,
    						iif(RnTra > 1, 0, GiaTriTra) as GiaTriTra,
    
    						iif(RnDoi > 1, '', GDVDoi_MaHangHoa) as GDVDoi_MaHangHoa,
    						iif(RnDoi > 1, '', GDVDoi_TenHangHoa) as GDVDoi_TenHangHoa,
    						iif(RnDoi > 1, '', GDVDoi_TenDonViTinh) as GDVDoi_TenDonViTinh,
    						iif(RnDoi > 1, 0, SoLuongDoi) as SoLuongDoi,
    						iif(RnDoi > 1, 0, GiaTriDoi) as GiaTriDoi
    						
    					from tblLast 			
    					cross join count_cte
    					order by tblLast.NgayLapHoaDon desc
    					OFFSET (@CurrentPage* @PageSize) ROWS
    					FETCH NEXT @PageSize ROWS ONLY
    		
    
    	drop table #cthdDoi
    	drop table #cthdTra
    	drop table #cthdMuaGoc
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoNhapHang_ChiTiet]
    @Text_Search [nvarchar](max),  
    @MaNCC [nvarchar](max),   
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @ID_NhomNCC [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
set nocount on;
if @MaNCC is null set @MaNCC =''

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From
    		HT_NguoiDung nd	
    		where nd.ID = @ID_NguoiDung)

			DECLARE @tblSearchString TABLE (Name [nvarchar](max));
			DECLARE @count int;
			INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';
			Select @count =  (Select count(*) from @tblSearchString);


	   SELECT 
    		a.MaHoaDon,
    		a.NgayLapHoaDon,
    		a.MaNhaCungCap,
    		a.TenNhaCungCap,
    		a.MaHangHoa,
    		a.TenHangHoaFull,
    		a.TenHangHoa,
    		a.ThuocTinh_GiaTri,
    		a.TenDonViTinh,
    		a.TenLoHang,
			a.TienThue,
    		a.SoLuong, 
    		Case When @XemGiaVon = '1' then a.GiaBan else 0 end  as DonGia,
    		Case When @XemGiaVon = '1' then	a.TienChietKhau else 0 end  as TienChietKhau,
    		Case When @XemGiaVon = '1' then a.ThanhTien else 0 end  as ThanhTien,
    		Case When @XemGiaVon = '1' then	a.GiamGiaHD else 0 end  as GiamGiaHD,
    		Case When @XemGiaVon = '1' then a.ThanhTien  else 0 end as GiaTriNhap, 
    		a.TenNhanVien,
			a.GhiChu
    	FROM
    	(
    		Select hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    		Case When dtn.ID_NhomDoiTuong is null then '30000000-0000-0000-0000-000000000003' else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    		dt.MaDoiTuong as MaNhaCungCap,
    		dt.TenDoiTuong as TenNhaCungCap,
    		dvqd.MaHangHoa,
    		concat(hh.TenHangHoa ,' ', dvqd.ThuocTinhGiaTri) as TenHangHoaFull,
    		hh.TenHangHoa,
    		dvqd.TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		lh.MaLoHang as TenLoHang,
    		hdct.SoLuong,
    		hdct.DonGia as GiaBan,
    		hdct.TienChietKhau,
    		nv.TenNhanVien,
			hdct.GhiChu,
    		hdct.ThanhTien,
			hdct.TienThue * hdct.SoLuong as TienThue,
			Case when hd.TongTienHang = 0 then 0 else hdct.ThanhTien * (hd.TongGiamGia / hd.TongTienHang) end as GiamGiaHD
    		FROM BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
    		inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    		inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    		left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    		left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    		left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    		where hd.NgayLapHoaDon between @timeStart and @timeEnd
    		and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    		and hd.ChoThanhToan = 0 
    		and hd.LoaiHoaDon in (4,13,14)
    		and dvqd.Xoa like @TrangThai
    		and hh.TheoDoi like @TheoDoi
			and (@ID_NhomHang is null or exists (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) allnhh where hh.ID_NhomHang= allnhh.ID))    		
			AND
			((select count(Name) from @tblSearchString b where 
    				hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or lh.MaLoHang like '%' +b.Name +'%' 
    				or dvqd.MaHangHoa like '%'+b.Name+'%'
    				or hd.MaHoaDon like '%'+b.Name+'%'
    				or hdct.GhiChu like '%'+b.Name+'%'    									
    				or dvqd.TenDonViTinh like '%'+b.Name+'%'
    				or dvqd.ThuocTinhGiaTri like '%'+b.Name+'%')=@count or @count=0)
    		and  (dt.MaDoiTuong like N'%'+@MaNCC +'%'
					or dt.TenDoiTuong_KhongDau like N'%'+ @MaNCC+'%'
					or dt.TenDoiTuong like N'%'+ @MaNCC+'%'
					or dt.TenDoiTuong_ChuCaiDau like N'%'+ @MaNCC +'%'
					or dt.DienThoai like N'%'+ @MaNCC +'%')
    	) a
    	where (@ID_NhomNCC ='' or a.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomNCC)))
    	order by a.NgayLapHoaDon desc
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
 --   MAX(b.TenNhomDoiTuong) as NhomDoiTuong,
 --   b.MaHoaDon,
 --   MAX(b.MaPhieuThu) as MaPhieuThu,
 --   MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
 --   MAX(b.ManguoiNop) as ManguoiNop, 
 --   MAX(b.TenNguoiNop) as TenNguoiNop, 
	--MAX(b.TienMat) AS TienMat,
	--MAX(b.TienGui) AS TienGui,
	--MAX(b.TienPOS) AS TienPOS,
 --   MAX(b.ThuChi) as ThuChi, 
 --   MAX(b.NoiDungThuChi) as NoiDungThuChi,
 --   MAX(b.GhiChu) as GhiChu,
 --   MAX(b.LoaiThuChi) as LoaiThuChi,
		b.MaHoaDon, 
		b.MaPhieuThu,
		b.NgayLapHoaDon,
		b.ManguoiNop,
		b.TenNguoiNop,
		b.TenNhomDoiTuong as NhomDoiTuong,
		b.TienMat,
		b.TienGui,
		b.TienPOS,
		b.ThuChi,
		b.NoiDungThuChi,
		b.GhiChu,
		b.LoaiThuChi,
    	dv.TenDonVi AS TenChiNhanh,
		b.SoTaiKhoan,
		b.TenNganHang,
		HinhThucThanhToan
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
			a.TenNganHang,
			a.HinhThucThanhToan
    	From
    	(
    		select 
    			qhd.LoaiHoaDon,
				qhd.ID as ID_HoaDon,
				qhd.ID_DonVi,
				qhd.MaHoaDon as MaPhieuThu,
				qhd.NgayLapHoaDon,
				qhd.HachToanKinhDoanh,				
				qhd.NoiDungThu as GhiChu,    			
    			hd.MaHoaDon,    			
				ktc.NoiDungThuChi,
				nh.TenNganHang,
				tknh.SoTaiKhoan as SoTaiKhoan,
				dt.ID as  ID_DoiTuong,
				dt.DienThoai,
				qhdct.HinhThucThanhToan, ---- used to order by LoaiThanhtoan: mat, pos, ck
				--MAX(qhd.NoiDungThu) as GhiChu,
				
    			--MAX(qhd.ID) as ID_HoaDon,
    			--MAX(dt.ID) as ID_DoiTuong,
    			--MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			
    			--MAX (nh.TenNganHang) as NganHang,
    			--Max(dt.TenNhomDoiTuongs) as TenNhomDoiTuong,
    			case when qhdct.ID_NhanVien is not null then N'Nhân viên' else dt.TenNhomDoiTuongs end as TenNhomDoiTuong,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 32) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22 or hd.LoaiHoaDon = 25) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case WHEN qhdct.ID_NhanVien is not null
    				then '00000000-0000-0000-0000-000000000000'    				
    				else 
    				case When dtn.ID_NhomDoiTuong is null 
    				then '00000000-0000-0000-0000-000000000000'  else dtn.ID_NhomDoiTuong 
    				end
    				end as ID_NhomDoiTuong,

				case when qhdct.ID_NhanVien is not null then nv.TenNhanVien else dt.TenDoiTuong end as TenNguoiNop,
    			case when qhdct.ID_NhanVien is not null then nv.MaNhanVien else dt.MaDoiTuong end as ManguoiNop,
				iif(qhdct.HinhThucThanhToan= 1, qhdct.TienThu,0) as TienMat,
				iif(qhdct.HinhThucThanhToan= 2,qhdct.TienThu,0) as TienPOS,
				iif(qhdct.HinhThucThanhToan= 3,qhdct.TienThu,0) as TienGui
				

    --			Sum(qhdct.TienMat) as TienMat,
    --			IIF(tknh.TaiKhoanPOS = 1, 0, Sum(qhdct.TienGui)) as TienGui,
				--IIF(tknh.TaiKhoanPOS = 1, SUM(qhdct.TienGui), 0) AS TienPOS,
    			
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
    	--	Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhdct.ID_NhanVien, dt.MaDoiTuong,dt.LoaiDoiTuong,  nv.MaNhanVien,
    	--		 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID, qhd.ID_DonVi,
				 --tknh.TaiKhoanPOS, tknh.SoTaiKhoan, nh.TenNganHang, nv.TenNhanVien, dt.TenDoiTuong
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) 
    	) b
    		inner join DM_DonVi dv ON dv.ID = b.ID_DonVi
    		where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) OR @ID_NhomDoiTuong = ''
			order by NgayLapHoaDon desc, HinhThucThanhToan 
    	--Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi, dv.TenDonVi, b.SoTaiKhoan, b.TenNganHang
    	--ORDER BY NgayLapHoaDon DESC
END");

			Sql(@"ALTER PROCEDURE [dbo].[TinhCongNo_HDTra]
	@tblID TblID readonly,
	@LoaiHoaDonTra int
AS
BEGIN
	
	SET NOCOUNT ON;

	declare @LoaiThuChi int = iif(@LoaiHoaDonTra=6,12,11)


	select 
		hd.ID,		
		hd.ID_HoaDon as ID_HoaDonGoc,
		hdd.ID as ID_HoaDonDoi,	
		hd.PhaiThanhToan as HDTra_PhaiThanhToan,	
		hdd.PhaiThanhToan as HDDoi_PhaiThanhToan
	into #tblHD
	from BH_HoaDon hd
	left join BH_HoaDon hdd on hd.ID = hdd.ID_HoaDon
	where exists (select ID from @tblID tbl where hd.ID= tbl.ID)
	
	------ get allHDTra by idHDGoc ----
	select hdt.ID 
	into #allHDTra
	from BH_HoaDon hdt
	where hdt.ChoThanhToan='0' and hdt.LoaiHoaDon= @LoaiHoaDonTra
	and exists (select ID from #tblHD tbl where hdt.ID_HoaDon= tbl.ID_HoaDonGoc ) 


	-------- lũy kế all hdTra + all hdDoi (trước thời điểm trả hàng) ------		
	select hdt.ID, 
		hdt.MaHoaDon,
		hdt.NgayLapHoaDon,
		sum(isnull(hdtBefore.PhaiThanhToan,0)) as HDTra_LuyKeGtriTra,
		sum(isnull(sqChi.TienChi,0)) as HDTra_LuyKeChi,

		sum(isnull(hdDoi.PhaiThanhToan,0)) as HDDoi_LuyKeGiatriDoi,
		sum(isnull(thuDoi.TienChi,0)) as HDDoi_LuyKeThuTien
	into #luykeDoiTra
	from BH_HoaDon hdt
	left join BH_HoaDon hdtBefore on hdt.ID_HoaDon= hdtBefore.ID_HoaDon 
			------- lũy kế trả: chỉ lấy những hdTra trước đó -----
			and hdtBefore.NgayLapHoaDon < hdt.NgayLapHoaDon and hdtBefore.ChoThanhToan='0' and hdtBefore.LoaiHoaDon= @LoaiHoaDonTra
	left join (
		----- get all phieuchi trahang theo hdGoc bandau ----
		select 
			qct.ID_HoaDonLienQuan,
			sum(iif(qhd.LoaiHoaDon= @LoaiThuChi, qct.TienThu, - qct.TienThu)) as TienChi
		from Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
		where (qhd.TrangThai is null or qhd.TrangThai='1') 
		and exists (select ID from #allHDTra allTra where allTra.ID= qct.ID_HoaDonLienQuan )
		group by qct.ID_HoaDonLienQuan
	) sqChi on hdtBefore.ID= sqChi.ID_HoaDonLienQuan 
	left join BH_HoaDon hdDoi on hdtBefore.ID = hdDoi.ID_HoaDon and hdDoi.ChoThanhToan='0'
	left join
	(
		----- get all phieuthu hdDoi tu hdTra ----
		select 
			qct.ID_HoaDonLienQuan,
			sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, - qct.TienThu)) as TienChi
		from Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
		where (qhd.TrangThai is null or qhd.TrangThai='1') 
		group by qct.ID_HoaDonLienQuan
	) thuDoi on hdDoi.ID= thuDoi.ID_HoaDonLienQuan
	where exists (select ID from #tblHD tbl where hdt.ID_HoaDon= tbl.ID_HoaDonGoc )
	and hdt.LoaiHoaDon= @LoaiHoaDonTra
	group by hdt.ID, hdt.MaHoaDon, hdt.NgayLapHoaDon
	
  
	  ------ tinhcongno hdgoc (chỉ tính công nợ của chính nó)
	  select 
			hdg.ID,
			hdg.ID_BaoGia,
			max(hdg.MaHoaDon) as MaHoaDon,
			max(hdg.LoaiHoaDon) as LoaiHoaDon,	
			max(hdg.PhaiThanhToan) as HDGoc_PhaiThanhToan,
			sum(iif(hdg.LoaiHoaDon = @LoaiThuChi, -hdg.TienThu, hdg.TienThu)) as ThuHDGoc
	  into #tblHDGoc
	  from
	  (
		  ----- thuhdgoc ----
		  select 	
	  		hdg.ID,		
			hdg.ID_HoaDon as ID_BaoGia,
			hdg.MaHoaDon,
			hdg.LoaiHoaDon,
			hdg.PhaiThanhToan,
			qhd.TrangThai,
			isnull(iif(qhd.TrangThai = 0, 0, qct.TienThu),0) as TienThu	 
		  from BH_HoaDon hdg
		  left join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDonLienQuan = hdg.ID and qct.ID_DoiTuong= hdg.ID_DoiTuong
		  left join Quy_HoaDon qhd  on qhd.ID= qct.ID_HoaDon 
		  where  exists (select ID from #tblHD tblHD where hdg.ID= tblHD.ID_HoaDonGoc)
	 ) hdg
	  group by hdg.ID, hdg.ID_BaoGia


	  ---- thu tu dathang (of HDGoc) ----
	  select 
			thuDH.ID,
			thuDH.TienThu as ThuDatHang,
			isFirst	
		into #thuDatHang
		from
		(
		   select 
				hdfromBG.ID,
				hdfromBG.ID_HoaDon,
				hdfromBG.NgayLapHoaDon,
				ROW_NUMBER() OVER(PARTITION BY hdfromBG.ID_HoaDon ORDER BY hdfromBG.NgayLapHoaDon ASC) AS isFirst,	
				sum(iif(qhd.LoaiHoaDon=11, qct.TienThu, -qct.TienThu)) as TienThu
		   from
		   (
			   select 
					hd.ID,
					hd.ID_HoaDon,
					hd.NgayLapHoaDon
			   from dbo.BH_HoaDon hd
			   join dbo.BH_HoaDon hdd on hd.ID_HoaDon= hdd.ID
			   where exists (select ID_BaoGia from #tblHDGoc tblHD where hdd.ID = tblHD.ID_BaoGia)
			   and hd.ChoThanhToan='0'  
			   and hdd.LoaiHoaDon= 3
		   ) hdfromBG
		   join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDonLienQuan	= hdfromBG.ID_HoaDon	
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID	
			where (qhd.TrangThai is null or qhd.TrangThai='1')
			group by hdfromBG.ID,
					hdfromBG.ID_HoaDon,
					hdfromBG.NgayLapHoaDon
		) thuDH
		where thuDH.isFirst = 1



  ----- Cách tính (Phải trả khách) ----
  ----- TH1. Lũy kế tổng trả <= Nợ hóa đơn gốc: Phải trả khách = 0
  ----- Th2. Lũy kế tổng trả < Nợ hóa đơn gốc: Phải trả khách = Tổng trả - chi trả chính nó - công nợ HĐ gốc - Công nợ HD đổi (của HĐ trả)


		select 
			ID,
			MaHoaDonGoc,
			LoaiHoaDonGoc,
			HDDoi_PhaiThanhToan,
			iif(ID_HoaDonGoc is not null,			
					iif(LuyKeCuoiCung < 0, HDDoi_PhaiThanhToan,
						iif(LuyKeCuoiCung > HDTra_PhaiThanhToan, HDTra_PhaiThanhToan ,
						iif(LuyKeCuoiCung + HDDoi_PhaiThanhToan > HDTra_PhaiThanhToan,HDTra_PhaiThanhToan, LuyKeCuoiCung + HDDoi_PhaiThanhToan)
						)), 
				iif(HDTra_PhaiThanhToan > HDDoi_PhaiThanhToan, HDTra_PhaiThanhToan - HDDoi_PhaiThanhToan,HDTra_PhaiThanhToan)
				) as BuTruHDGoc_Doi
		from
		(
				select 
					a.ID,
					a.ID_HoaDonGoc,
					a.MaHoaDonGoc,
					a.LoaiHoaDonGoc,
					a.HDDoi_PhaiThanhToan,
					a.HDTra_PhaiThanhToan,					
					a.CongNoHDGoc - a.HDTra_CongNoLuyKe + a.HDDoi_CongNoLuyKe as LuyKeCuoiCung 
				from
				(
				select 
					hdt.ID,		
					hdt.ID_HoaDonGoc,
					hdt.HDTra_PhaiThanhToan,		
					
					isnull(hdgoc.CongNoHDGoc,0) as CongNoHDGoc,	
					isnull(hdgoc.MaHoaDon,'') as MaHoaDonGoc,
					isnull(hdgoc.LoaiHoaDon,0) as LoaiHoaDonGoc,
					isnull(hdt.HDDoi_PhaiThanhToan,0) as HDDoi_PhaiThanhToan,
					
					isnull(lkDoiTra.HDDoi_LuyKeGiatriDoi,0) - isnull(lkDoiTra.HDDoi_LuyKeThuTien,0) as HDDoi_CongNoLuyKe,
					isnull(lkDoiTra.HDTra_LuyKeGtriTra,0) - isnull(lkDoiTra.HDTra_LuyKeChi,0) as HDTra_CongNoLuyKe			
				from #tblHD hdt
				left join #luykeDoiTra lkDoiTra on hdt.ID= lkDoiTra.ID
				left join (
					----- congno HDGoc (bao gồm phiếu thu từ báo giá)
					select 
						hdgoc.ID,		
						hdgoc.MaHoaDon,
						hdgoc.LoaiHoaDon,
						hdgoc.HDGoc_PhaiThanhToan - hdgoc.ThuHDGoc - isnull(thuDH.ThuDatHang,0) as CongNoHDGoc
					from #tblHDGoc hdgoc
					left join #thuDatHang thuDH on hdgoc.ID = thuDH.ID
				) hdgoc on hdt.ID_HoaDonGoc = hdgoc.ID				
			 ) a
		) b

END
");

			Sql(@"ALTER PROCEDURE [dbo].[BCBanHang_theoMaDinhDanh]
 --declare 
	@pageNumber [int] = 1,
    @pageSize [int] = 300,
    @SearchString [nvarchar](max)=N'ngày',
    @timeStart [datetime]='2023-11-25',
    @timeEnd [datetime]='2023-12-01',
    @ID_ChiNhanh [nvarchar](max) ='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
    @LoaiHangHoa [nvarchar](max)='%%',
    @TheoDoi [nvarchar](max)='%1%',
    @TrangThai [nvarchar](max)='%0%',
    @ID_NhomHang [uniqueidentifier] = null,
    @LoaiChungTu [nvarchar](max) = '1,35,36'
AS
BEGIN
    SET NOCOUNT ON;
    
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
    
    	

    	select tblLast.*
		into #tblLast
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
    			dt.DienThoai as DienThoai1, ---- kangjin yêu cầu bảo mật SDT khách hàng ở full bao cáo
    			N'Ngày thuốc' as MaHangHoa,
				N'ngày' as TenDonViTinh,
    			N'Ngày thuốc' as TenHangHoa,
    			N'Ngày thuốc' as TenHangHoa_KhongDau,
    			nhom.ID as ID_NhomHang,
    			--nhom.TenNhomHangHoa,
				N'Nhóm ngày thuốc'  as  TenNhomHangHoa,
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
				qd.TenDonViTinh,
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
    				---ct.DonGia,
					---- cot DonGia: lấy sau chietkhau (để cho giống bên audo - c Huyen bao) --
					--- cot ChietKhau: gan = 0
					ct.DonGia - ct.TienChietKhau as DonGia,
    				cast(0 as float) as TienChietKhau,		
					ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as ThanhTien, -- sudung GDV: van lay thanhtien ----
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
    			or tblLast.DienThoai1  like '%'+b.Name+'%'
    				or tblLast.GhiChu like N'%'+b.Name+'%'
    			)=@count or @count=0)

		


				declare @totalRow int =(select count(MaHangHoa) from  #tblLast)

				select 
					tbl.*,
					@totalRow as TotalRow,
					CEILING(@totalRow/ cast (@pageSize as float)) as TotalPage,
					isnull(maNV.NVThucHien,'') as MaNhanVien,
					isnull(tenNV.NVThucHien,'') as TenNhanVien	
				from #tblLast tbl
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
    				from #tblLast tblCT 
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
    				from #tblLast tblCT 
				) maNV on tbl.ID = maNV.ID_ChiTietHD
				order by tbl.NgayLapHoaDon desc, tbl.MaDinhDanh desc
				OFFSET (@pageNumber* @pageSize) ROWS
				FETCH NEXT @pageSize ROWS ONLY

   
    	drop table #hdHoTro
    	drop table #tblLast

END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_HangHoaXuatHuybyID]
--declare	
	@ID_HoaDon [uniqueidentifier] ='42BEC180-165D-4AF1-A57A-CFF868160975',
	@ID_ChiNhanh [uniqueidentifier] ='d93b17ea-89b9-4ecf-b242-d03b8cde71de'
AS
BEGIN
  set nocount on;

		declare @countCTMua int, @ID_HoaDonGoc uniqueidentifier		
		select @ID_HoaDonGoc= ID_HoaDon from BH_HoaDon where ID= @ID_HoaDon


		----- get all ctm goc (ke ca dv) ---
		---- vi se co truong hop capnhat tpdl (co --> null)
		select ctm.ID, ctm.ID_HoaDon,
			ctm.ChatLieu, -- chatlieu = 5: huy
			hdm.ChoThanhToan,
			hdm.MaHoaDon
		into #ctmua
		from
		(
			select hdg.ID, hdg.ChoThanhToan, hdg.MaHoaDon
			from BH_HoaDon hdg
			where hdg.ID = @ID_HoaDonGoc
		) hdm 
		join BH_HoaDon_ChiTiet ctm on hdm.ID = ctm.ID_HoaDon	


	select @countCTMua = COUNT(ID) from #ctmua	
		
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
			ctxk.GhiChu,

			dvqd.ID_HangHoa,
			dvqd.MaHangHoa,
			dvqd.TenDonViTinh as TenDonViTinh,
    		dvqd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			lh.MaLoHang,
			Case when lh.MaLoHang is null then '' else lh.MaLoHang end as TenLoHang,
			lh.NgaySanXuat,
    		lh.NgayHetHan,    			
    		hh.TenHangHoa,
			ROUND(ISNULL(tk.TonKho,0),2) as TonKho,
			Case when hh.QuanLyTheoLoHang is null then 'false' else hh.QuanLyTheoLoHang end as QuanLyTheoLoHang, 
    		concat(hh.TenHangHoa , '', dvqd.ThuocTinhGiaTri) as TenHangHoaFull,	
			cast(iif(@countCTMua > 0 and 
				(select count(ID) 
					from #ctmua 
					where #ctmua.ID = ctxk.ID_ChiTietGoiDV 
					and #ctmua.ChatLieu!='5'
					and #ctmua.ChoThanhToan ='0'
				) =0,1,0) as float) as TrangThaiMoPhieu
				
		from BH_HoaDon_ChiTiet ctxk		
		join DonViQuiDoi dvqd on ctxk.ID_DonViQuiDoi = dvqd.ID
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on ctxk.ID_LoHang = lh.ID
		left join DM_HangHoa_TonKho tk on (dvqd.ID = tk.ID_DonViQuyDoi 
		and (lh.ID = tk.ID_LoHang or lh.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh)
		where ctxk.ID_HoaDon = @ID_HoaDon
		and (hh.LaHangHoa = 1 and tk.TonKho is not null) 
		and (ctxk.ChatLieu is null or ctxk.ChatLieu != '5') 
	
	drop table #ctmua
END

");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCao_DoanhThuKhachHang]
 --declare  
	@IDChiNhanhs [nvarchar](max) ='8f01a137-e8ae-4239-ad96-4de67b2fec25',
    @DateFrom [datetime]= '2023-11-01',
    @DateTo [datetime] ='2023-11-20',
    @TextSearch [nvarchar](max) ='',
    @CurrentPage [int] = 1,
    @PageSize [int] = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    	if @CurrentPage > 0 set @CurrentPage = @CurrentPage - 1;
    
    	if(isnull(@TextSearch,'')='') set @TextSearch =''
    	else set @TextSearch= CONCAT(N'%',@TextSearch,'%')
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    insert into @tblChiNhanh
    select Name from dbo.splitstring(@IDChiNhanhs) where Name!=''
    
    	; with data_cte
    	as
    	(
		select *
		from
		(
    		select 
    			dt.ID as ID_DoiTuong,
    			dt.MaDoiTuong,
    			dt.TenDoiTuong,
    			---- dt.DienThoai as DienThoaiKhachHang, ---- kangjin yêu cầu bảo mật sdt khách hàng ---
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
    		from Quy_HoaDon_ChiTiet qct
    		join BH_HoaDon hd on  qct.ID_HoaDonLienQuan = hd.ID
    		--join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID	
    		--where dt.LoaiDoiTuong=1
    		--and dt.TheoDoi ='0'
    		where (qhd.TrangThai='1' or qhd.TrangThai is null)
    		and exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)
			and qhd.NgayLapHoaDon  between @DateFrom and @DateTo
			and qct.HinhThucThanhToan not in (4,5,6)
    
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
			and qhd.NgayLapHoaDon  between @DateFrom and @DateTo
    	) tblSoQuy
    	group by tblSoQuy.ID_DoiTuong, tblSoQuy.NgayThanhToan
    	) tblThuChi  on dt.ID= tblThuChi.ID_DoiTuong
    	--where NgayThanhToan >= @DateFrom and NgayThanhToan < @DateTo
    	where dt.LoaiDoiTuong= 1 and (@TextSearch='' or
    		dt.MaDoiTuong like @TextSearch 
    		or dt.TenDoiTuong like @TextSearch
    		or dt.TenDoiTuong_KhongDau like @TextSearch 
    		or dt.DienThoai like @TextSearch)
		)tblLast where tblLast.ThuHoaDon > 0 or tblLast.ChiHoaDon > 0 or tblLast.HoanCoc > 0 or tblLast.HoanDichVu > 0
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
    fetch next @PageSize ROWS only
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDichVu_SoDuTongHop]
    @Text_Search [nvarchar](max),
    @MaHH [nvarchar](max),
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
	@ThoiHan [nvarchar](max),
    @ID_NhomHang [nvarchar](max),
    @ID_NhomHang_SP [nvarchar](max),
	@ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	declare @tblChiNhanh table( ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@ID_ChiNhanh)

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';

    Select @count =  (Select count(*) from @tblSearchString);
	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    Case when nd.LaAdmin = '1' then '1' else
    Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    From  HT_NguoiDung nd	   
    where nd.ID = @ID_NguoiDung)

	declare @dtNow datetime = getdate()

	---- get list GDV mua ---
	declare @tblCTMua table(
		MaHoaDon nvarchar(max),
		NgayLapHoaDon datetime,
		NgayApDungGoiDV datetime,
		HanSuDungGoiDV datetime,
		ID_DonVi uniqueidentifier,
		ID_DoiTuong uniqueidentifier,
		ID uniqueidentifier,
		ID_HoaDon uniqueidentifier,
		ID_DonViQuiDoi uniqueidentifier,
		ID_LoHang uniqueidentifier,
		SoLuong float,
		DonGia float,
		TienChietKhau float,
		ThanhTien float,
		GiamGiaHD float)
	insert into @tblCTMua
	exec BaoCaoGoiDV_GetCTMua @ID_ChiNhanh,@timeStart,@timeEnd
		
		Select 
			a.MaHoaDon,
			a.NgayLapHoaDon,
			a.NgayApDungGoiDV,
			a.HanSuDungGoiDV,
			a.MaKhachHang,
			a.TenKhachHang,
			------a.DienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng
			a.GioiTinh,
			a.NhomKhachHang,
			a.TenNguonKhach,
			a.NguoiGioiThieu,
			sum(a.SoLuong) as SoLuong,
			sum(a.ThanhTien) as ThanhTien,
			sum(a.SoLuongTra) as SoLuongTra,
			sum(a.GiaTriTra) as GiaTriTra,
			sum(a.SoLuongSuDung) as SoLuongSuDung,
			iif(@XemGiaVon='0',cast( 0 as float),round( sum(a.GiaVon),2)) as GiaVon,
			round(sum(a.SoLuong) -  sum(a.SoLuongTra) - sum(a.SoLuongSuDung),2) as SoLuongConLai,
			CAST(ROUND(Case when DATEADD(day,-1,GETDATE()) <= MAX(a.HanSuDungGoiDV)
				then DATEDIFF(day,DATEADD(day,-1,GETDATE()),MAX(a.HanSuDungGoiDV)) else 0 end, 0) as float) as SoNgayConHan, 
			CAST(ROUND(Case when DATEADD(day,-1,GETDATE()) > MAX(a.HanSuDungGoiDV) 
			then DATEDIFF(day,DATEADD(day,-1,GETDATE()) ,MAX(a.HanSuDungGoiDV)) * (-1) else 0 end, 0) as float) as SoNgayHetHan			
		From
		(
				---- get by idnhom, thoihan --> check where
				select *
				from
				(
			
					select 
						ctm.ID_HoaDon,
						ctm.MaHoaDon,
						ctm.NgayLapHoaDon,
						ctm.NgayApDungGoiDV,
						ctm.HanSuDungGoiDV,
						dt.MaDoiTuong as MaKhachHang,
						dt.TenDoiTuong as TenKhachHang,
						dt.DienThoai,
						Case when dt.GioiTinhNam = 1 then N'Nam' else N'Nữ' end as GioiTinh,
						gt.TenDoiTuong as NguoiGioiThieu,
						nk.TenNguonKhach,
						isnull(dt.TenNhomDoiTuongs, N'Nhóm mặc định') as NhomKhachHang ,
						iif( hh.ID_NhomHang is null, '00000000-0000-0000-0000-000000000000',hh.ID_NhomHang) as ID_NhomHang,
						iif(@dtNow <=ctm.HanSuDungGoiDV,1,0) as ThoiHan,						
						ctm.SoLuong,
						ctm.ThanhTien,
						isnull(tbl.SoLuongTra,0) as SoLuongTra,
						isnull(tbl.GiaTriTra,0) as GiaTriTra,
						isnull(tbl.SoLuongSuDung,0) as SoLuongSuDung,
						isnull(tbl.GiaVon,0) as GiaVon						
					from @tblCTMua ctm
					inner join DonViQuiDoi dvqd on ctm.ID_DonViQuiDoi = dvqd.ID
					inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
					left join DM_DoiTuong dt on ctm.ID_DoiTuong = dt.ID
					left join DM_DoiTuong gt on dt.ID_NguoiGioiThieu = gt.ID
					left join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
					left join (
						select 
							tblSD.ID_ChiTietGoiDV,
							sum(tblSD.SoLuongTra) as SoLuongTra,
							sum(tblSD.GiaTriTra) as GiaTriTra,
							sum(tblSD.SoLuongSuDung) as SoLuongSuDung,
							sum(tblSD.GiaVon) as GiaVon
						from 
						(
							---- hdsudung
							Select 								
								ct.ID_ChiTietGoiDV,														
								0 as SoLuongTra,
								0 as GiaTriTra,
								ct.SoLuong as SoLuongSuDung,
								ct.SoLuong * ct.GiaVon as GiaVon
							FROM BH_HoaDon hd
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							join @tblCTMua ctm on ct.ID_ChiTietGoiDV= ctm.ID
							where hd.ChoThanhToan= 0
							and hd.LoaiHoaDon in (1,25)
							and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)

							union all
							--- hdtra
							Select 							
								ct.ID_ChiTietGoiDV,															
								ct.SoLuong as SoLuongTra,
								ct.ThanhTien as GiaTriTra,
								0 as SoLuongSuDung,
								0 as GiaVon
							FROM BH_HoaDon hd
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							join @tblCTMua ctm on ct.ID_ChiTietGoiDV= ctm.ID
							where hd.ChoThanhToan= 0
							and hd.LoaiHoaDon = 6
							and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
							)tblSD group by tblSD.ID_ChiTietGoiDV

					) tbl on ctm.ID= tbl.ID_ChiTietGoiDV
				where hh.LaHangHoa like @LaHangHoa
    			and hh.TheoDoi like @TheoDoi
    			and dvqd.Xoa like @TrangThai
				AND ((select count(Name) from @tblSearchString b where 
					ctm.MaHoaDon like '%'+b.Name+'%'
    				or hh.TenHangHoa like '%'+b.Name+'%'
    				or dvqd.MaHangHoa like '%'+b.Name+'%'
    				or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    				or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'
    				or dt.MaDoiTuong like '%'+b.Name+'%'
    				or dt.TenDoituong like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
					)=@count or @count=0)
			) b where b.ThoiHan like @ThoiHan
				and (b.ID_NhomHang like @ID_NhomHang or b.ID_NhomHang in (select * from splitstring(@ID_NhomHang_SP)))
			) a
    	Group by a.MaHoaDon,
			a.NgayLapHoaDon,
			a.NgayApDungGoiDV,
			a.HanSuDungGoiDV,
			a.MaKhachHang,
			a.TenKhachHang,
			a.DienThoai,
			a.GioiTinh,
			a.NhomKhachHang,
			a.TenNguonKhach,
			a.NguoiGioiThieu
    	order by a.NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[ReportValueCard_Balance]
    @TextSearch [nvarchar](max),
    @ID_ChiNhanhs [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),	
    @Status [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;


	set @DateTo = DATEADD(day, 1, @DateTo)
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString)
    
    	;with data_cte
    	as (
    		select 
    				tblView.ID, tblView.MaDoiTuong, tblView.TenDoiTuong, 
    				------ ISNULL(tblView.DienThoai,'') as DienThoaiKhachHang, kangjin yêu cầu bảo mật sdt khách hàng ---
    				ISNULL(tblView.SoDuDauKy,0) as SoDuDauKy,
    				ISNULL(tblView.PhatSinhTang,0) as PhatSinhTang,
    				ISNULL(tblView.PhatSinhGiam,0) as PhatSinhGiam,
    				ISNULL(tblView.SoDuDauKy,0) + ISNULL(tblView.PhatSinhTang,0)- ISNULL(tblView.PhatSinhGiam,0) as SoDuCuoiKy,
    				case when tblView.TrangThai_TheGiaTri is null or tblView.TrangThai_TheGiaTri = 1 then N'Đang hoạt động'
    				else N'Ngừng hoạt động' end as TrangThai_TheGiaTri,
    				TrangThai
    		from 
    		(
    			select 
    				dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, 
    				dt.TrangThai_TheGiaTri,
    				case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
    				else '12' end as TrangThai, -- used to where TrangThai_TheGiaTri (1: all, 11: dang hoat dong, 2. Ngung hoat dong)
    				dt.DienThoai,
    				tblTemp.SoDuDauKy,
    				tblTemp.PhatSinhTang,
    				tblTemp.PhatSinhGiam
    			from DM_DoiTuong dt
    			left join 
    			( 
    				select 
    					ID_DoiTuong,
    					SUM(ISNULL(TongThuTheGiaTri,0)) as TongThuTheGiaTri,
    					SUM(ISNULL(SuDungThe,0)) as SuDungThe,
    					SUM(ISNULL(HoanTraTheGiatri,0)) as HoanTraTheGiaTri,
    					SUM(ISNULL(TongThuTheGiaTri,0))  - SUM(ISNULL(SuDungThe,0)) + SUM(ISNULL(HoanTraTheGiatri,0)) - SUM(ISNULL(TatToanThe,0)) as SoDuDauKy,
    					SUM(ISNULL(PhatSinh_ThuTuThe,0)) + SUM(ISNULL(PhatSinh_HoanTraTheGiatri,0)) + SUM(ISNULL(PhatSinhTang_DieuChinhThe,0)) as PhatSinhTang,
    					SUM(ISNULL(PhatSinh_SuDungThe,0)) + SUM(ISNULL(PhatSinhGiam_DieuChinhThe,0)) + SUM(ISNULL(PhatSinhGiam_TatToanThe,0)) as PhatSinhGiam
    
    				from (
							----- ===== Dau ky =======
    					 ---- thu the gtri trước thời gian tìm kiếm (lấy luôn cả gtrị điều chỉnh)
    							 SELECT hd.ID_DoiTuong,
    								  sum(hd.TongTienHang) as TongThuTheGiaTri,
									  0 as  SuDungThe,
    								  0 as  HoanTraTheGiatri,		
									  0 as  TatToanThe,
    								  0 as  PhatSinh_ThuTuThe,
    								  0 as  PhatSinh_SuDungThe,
    								  0 as  PhatSinh_HoanTraTheGiatri,
    								  0 as  PhatSinhTang_DieuChinhThe,
    								  0 as  PhatSinhGiam_DieuChinhThe,
									  0 as  PhatSinhGiam_TatToanThe
    							 from BH_HoaDon hd    							 
    							 where hd.NgayLapHoaDon < @DateFrom 
    							 and hd.ChoThanhToan='0' 
								 and hd.LoaiHoaDon in (22,23)								 
    							 group by hd.ID_DoiTuong
    						 
    
    					 union all
    					 ---- su dung the giatri    						
    						SELECT qct.ID_DoiTuong,
								0 as  TongThuTheGiaTri,
    							sum(qct.TienThu)  as SuDungThe,
								0 as  HoanTraTheGiatri,		
								0 as  TatToanThe,
    							0 as  PhatSinh_ThuTuThe,
    							0 as  PhatSinh_SuDungThe,
    							0 as  PhatSinh_HoanTraTheGiatri,
    							0 as  PhatSinhTang_DieuChinhThe,
    							0 as  PhatSinhGiam_DieuChinhThe,
								0 as PhatSinhGiam_TatToanThe
    						from Quy_HoaDon_ChiTiet qct
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon < @DateFrom 
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 11
							and qct.HinhThucThanhToan = 4
    						group by qct.ID_DoiTuong
    						 
    
    				 union all
    					  -- hoan tra tien vao the (tang sodu)   						
    						SELECT qct.ID_DoiTuong,
								0 as  TongThuTheGiaTri,
    							0 as  SuDungThe,
    							sum(qct.TienThu) as HoanTraTheGiatri,
								0 as  TatToanThe,
								0 as  PhatSinh_ThuTuThe,
    							0 as  PhatSinh_SuDungThe,
    							0 as  PhatSinh_HoanTraTheGiatri,
    							0 as  PhatSinhTang_DieuChinhThe,
    							0 as  PhatSinhGiam_DieuChinhThe,
								0 as  PhatSinhGiam_TatToanThe
    						from Quy_HoaDon_ChiTiet qct   								
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon < @DateFrom 
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 12
							and qct.HinhThucThanhToan = 4
    						group by qct.ID_DoiTuong
    						
						 union all
    					  -- giam do hoantracoc			
    					 SELECT hd.ID_DoiTuong,
    							null TongThuTheGiaTri,
								sum(hd.TongTienHang) as SuDungThe,
    							0 as  HoanTraTheGiatri,		
								0 as TatToanThe,
    							0 as  PhatSinh_ThuTuThe,
    							0 as  PhatSinh_SuDungThe,
    							0 as  PhatSinh_HoanTraTheGiatri,
    							0 as  PhatSinhTang_DieuChinhThe,
    							0 as  PhatSinhGiam_DieuChinhThe,
								0 as  PhatSinhGiam_TatToanThe
    					from BH_HoaDon hd    							 
    					where hd.NgayLapHoaDon < @DateFrom 
    					and hd.ChoThanhToan='0' 
						and hd.LoaiHoaDon = 32
    					group by hd.ID_DoiTuong

						 union all
    					  -- giam do tat toan congno
    					 SELECT hd.ID_DoiTuong,
    							0 TongThuTheGiaTri,
								0 as SuDungThe,
    							0 as  HoanTraTheGiatri,		
								0 as TatToanThe,
    							0 as  PhatSinh_ThuTuThe,
    							0 as  PhatSinh_SuDungThe,
    							0 as  PhatSinh_HoanTraTheGiatri,
    							0 as  PhatSinhTang_DieuChinhThe,
    							0 as  PhatSinhGiam_DieuChinhThe,
								0 as  PhatSinhGiam_TatToanThe
    					from BH_HoaDon hd    							 
    					where hd.NgayLapHoaDon < @DateFrom 
    					and hd.ChoThanhToan='0' 
						and hd.LoaiHoaDon = 42
    					group by hd.ID_DoiTuong
    
					-----=========== Trong ky ==============
    					 union all
    					   --- thu the gtri tại thời điểm hiện tại
    						SELECT hd.ID_DoiTuong,
    								  0 as  TongThuTheGiaTri,
									  0 as  SuDungThe,
    								  0 as  HoanTraTheGiatri,		
									  0 as TatToanThe,
    								  sum(hd.TongTienHang) as PhatSinh_ThuTuThe,
    								  0 as  PhatSinh_SuDungThe,
    								  0 as  PhatSinh_HoanTraTheGiatri,
    								  0 as  PhatSinhTang_DieuChinhThe,
    								  0 as  PhatSinhGiam_DieuChinhThe,
									  0 as  PhatSinhGiam_TatToanThe
    							 from BH_HoaDon hd    							 
    							 where hd.NgayLapHoaDon between @DateFrom  and @DateTo
    							 and hd.ChoThanhToan='0' 
								 and hd.LoaiHoaDon = 22
    							 group by hd.ID_DoiTuong
    
    				union all
    					 -- su dung the giatri tại thời điểm hiện tại
    						SELECT qct.ID_DoiTuong,
								0 as  TongThuTheGiaTri,
    							null  as SuDungThe,
								0 as  HoanTraTheGiatri,			
								0 as TatToanThe,
    							0 as  PhatSinh_ThuTuThe,
    							sum(qct.TienThu) as PhatSinh_SuDungThe,
    							0 as  PhatSinh_HoanTraTheGiatri,
    							0 as  PhatSinhTang_DieuChinhThe,
    							0 as  PhatSinhGiam_DieuChinhThe,
								0 as  PhatSinhGiam_TatToanThe
    						from Quy_HoaDon_ChiTiet qct
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon between @DateFrom  and @DateTo
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 11
							and qct.HinhThucThanhToan = 4
    						group by qct.ID_DoiTuong
    
							---- tang/giam do dieu chinh the hoac hoantra tiencoc
							 union all
							 SELECT hd.ID_DoiTuong,
    								  0 as  TongThuTheGiaTri,
									  0 as  SuDungThe,
    								  0 as  HoanTraTheGiatri,	
									  0 as TatToanThe,
    								  0 as  PhatSinh_ThuTuThe,
    								  0 as  PhatSinh_SuDungThe,
    								  0 as  PhatSinh_HoanTraTheGiatri,
    								  sum(iif(hd.LoaiHoaDon = 32,0, iif(hd.TongTienHang > 0,hd.TongTienHang,0)))  as PhatSinhTang_DieuChinhThe,
    								  sum(iif(hd.LoaiHoaDon = 32, hd.TongTienHang, iif(hd.TongTienHang < 0,-hd.TongTienHang,0)))  as PhatSinhGiam_DieuChinhThe,
									  0 as  PhatSinhGiam_TatToanThe
    							 from BH_HoaDon hd    							 
    							 where hd.NgayLapHoaDon between @DateFrom  and @DateTo
    							 and hd.ChoThanhToan='0' 
								 and hd.LoaiHoaDon in (23,32)
    							 group by hd.ID_DoiTuong   
    
    					union all
    					  -- hoan tra tien the giatri tại thời điểm hiện tại					
    						SELECT qct.ID_DoiTuong,
								0 as  TongThuTheGiaTri,
    							0 as  SuDungThe,
    							0 as  HoanTraTheGiatri,
								0 as TatToanThe,
								0 as  PhatSinh_ThuTuThe,
    							0 as  PhatSinh_SuDungThe,
    							sum(qct.TienThu) as PhatSinh_HoanTraTheGiatri,
    							0 as  PhatSinhTang_DieuChinhThe,
    							0 as  PhatSinhGiam_DieuChinhThe,
								0 as  PhatSinhGiam_TatToanThe
    						from Quy_HoaDon_ChiTiet qct   								
    						left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    						where  qhd.NgayLapHoaDon between @DateFrom  and @DateTo
    						and (qhd.TrangThai ='1' or qhd.TrangThai is null)
    						and qhd.LoaiHoaDon = 12
							and qct.HinhThucThanhToan = 4
    						group by qct.ID_DoiTuong   
							 union all

    					  -- giam do tat toan congno
    					 SELECT hd.ID_DoiTuong,
    							0 TongThuTheGiaTri,
								0 as SuDungThe,
    							0 as  HoanTraTheGiatri,		
								0 as TatToanThe,
    							0 as  PhatSinh_ThuTuThe,
    							0 as  PhatSinh_SuDungThe,
    							0 as  PhatSinh_HoanTraTheGiatri,
    							0 as  PhatSinhTang_DieuChinhThe,
    							0 as  PhatSinhGiam_DieuChinhThe,
								sum(hd.TongTienHang) as PhatSinhGiam_TatToanThe
    					from BH_HoaDon hd    							 
    					where hd.NgayLapHoaDon between @DateFrom  and @DateTo
    					and hd.ChoThanhToan='0' 
						and hd.LoaiHoaDon = 42
    					group by hd.ID_DoiTuong

    					) tblDoiTuong_The group by tblDoiTuong_The.ID_DoiTuong
						
    			) tblTemp on dt.ID= tblTemp.ID_DoiTuong
    			where (dt.TheoDoi is null or dt.TheoDoi = 0) and dt.LoaiDoiTuong =1
    				and
    					 
    							((select count(Name) from @tblSearchString b where    
								dt.DienThoai like '%'+b.Name+'%'
    							or dt.MaDoiTuong like '%'+b.Name+'%'
    							or dt.TenDoiTuong like '%'+b.Name+'%'
    							or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    							or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'				
    							)=@count or @count=0)	
    
    		) tblView 
    		where tblView.TrangThai like @Status
    		and ISNULL(tblView.SoDuDauKy,0) + ISNULL(tblView.PhatSinhTang,0)- ISNULL(tblView.PhatSinhGiam,0) > 0
    	),
    	count_cte
    	as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(SoDuDauKy) as TongSoDuDauKy,
    				sum(PhatSinhTang) as TongPhatSinhTang,
    				sum(PhatSinhGiam) as TongPhatSinhGiam,
    				sum(SoDuCuoiKy) as TongSoDuCuoiKy
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaDoiTuong
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListCashFlow_Paging]
	@IDDonVis [nvarchar](max),
    @ID_NhanVien [nvarchar](40),
    @ID_NhanVienLogin [uniqueidentifier],
    @ID_TaiKhoanNganHang [nvarchar](40),
    @ID_KhoanThuChi [nvarchar](40),
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiSoQuy [nvarchar](15),	-- mat/nganhang/all
    @LoaiChungTu [nvarchar](2), -- thu/chi
    @TrangThaiSoQuy [nvarchar](2),
    @TrangThaiHachToan [nvarchar](2),
    @TxtSearch [nvarchar](max),
    @CurrentPage [int],
    @PageSize [int],
	@LoaiNapTien [nvarchar](15) -- 11.tiencoc, 10. khongphai tiencoc, 1.all
AS
BEGIN

	SET NOCOUNT ON;

   SET NOCOUNT ON;
	declare @isNullSearch int = 1
	if isnull(@TxtSearch,'')='' OR @TxtSearch ='%%'
		begin
			set @isNullSearch =0 
			set @TxtSearch ='%%'
		end
	else
		set @TxtSearch= CONCAT(N'%',@TxtSearch, '%')

	declare @tblChiNhanh table (ID uniqueidentifier)
    insert into @tblChiNhanh
	select name from dbo.splitstring(@IDDonVis)

	--declare #tblQuyHD table (ID uniqueidentifier, MaHoaDon nvarchar(40), NgayLapHoaDon datetime, ID_DonVi uniqueidentifier,
	--LoaiHoaDon int, NguoiTao nvarchar(100), HachToanKinhDoanh bit, PhieuDieuChinhCongNo int,
	--NoiDungThu nvarchar(max), ID_NhanVienPT uniqueidentifier, TrangThai bit)

	--insert into #tblQuyHD
	select qhd.ID,
		qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.ID_DonVi, qhd.LoaiHoaDon, qhd.NguoiTao,
    	qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo, qhd.NoiDungThu,
    	qhd.ID_NhanVien as ID_NhanVienPT, qhd.TrangThai	
	into #tblQuyHD
	from Quy_HoaDon qhd	
	where qhd.NgayLapHoaDon between  @DateFrom and  @DateTo		
	and qhd.ID_DonVi in (select * from dbo.splitstring(@IDDonVis))
	and(qhd.PhieuDieuChinhCongNo != '1' or qhd.PhieuDieuChinhCongNo is null)


    	declare @nguoitao nvarchar(100) = (select taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDDonVis,'SoQuy_XemDS_PhongBan','SoQuy_XemDS_HeThong');
    	
    	with data_cte
    	as(

    select tblView.*
    	from
    		(
			select 
    			tblQuy.ID,
    			tblQuy.MaHoaDon,
    			tblQuy.NgayLapHoaDon,
    			tblQuy.ID_DonVi,
    			tblQuy.LoaiHoaDon,
    			tblQuy.NguoiTao,
				ISNUll(nv2.TenNhanVien,'') as TenNhanVien,
				ISNUll(nv2.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
			
				ISNUll(dv.TenDonVi,'') as TenChiNhanh,
				ISNUll(dv.SoDienThoai,'') as DienThoaiChiNhanh,
				ISNUll(dv.DiaChi,'') as DiaChiChiNhanh,
				ISNUll(nguon.TenNguonKhach,'') as TenNguonKhach,
    			ISNUll(tblQuy.TrangThai,'1') as TrangThai,
    			tblQuy.NoiDungThu,
				iif(@isNullSearch=0, dbo.FUNC_ConvertStringToUnsign(NoiDungThu), tblQuy.NoiDungThu) as NoiDungThuUnsign,
    			tblQuy.PhieuDieuChinhCongNo,
    			tblQuy.ID_NhanVienPT as ID_NhanVien,
    			iif(LoaiHoaDon=11, TienMat,0) as ThuMat,
    			iif(LoaiHoaDon=12, TienMat,0) as ChiMat,
    			iif(LoaiHoaDon=11, TienGui,0) as ThuGui,
    			iif(LoaiHoaDon=12, TienGui,0) as ChiGui,
    			TienMat + TienGui as TienThu,
    			TienMat + TienGui as TongTienThu,
				TienGui,
				TienMat, 
				ChuyenKhoan, 
				TienPOS,
				TienDoiDiem, 
				TTBangTienCoc,
				TienTheGiaTri,
    			TenTaiKhoanPOS, TenTaiKhoanNOTPOS,
    			cast(ID_TaiKhoanNganHang as varchar(max)) as ID_TaiKhoanNganHang,
    			ID_KhoanThuChi,
    			NoiDungThuChi,
				tblQuy.ID_NhanVienPT,
				dt.ID_NguonKhach,
				iif(qct.ID_NhanVien is null,isnull(dt.LoaiDoiTuong,0),5) as LoaiDoiTuong,
    			ISNULL(tblQuy.HachToanKinhDoanh,'1') as HachToanKinhDoanh,
    			case when tblQuy.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
    		case when tblQuy.HachToanKinhDoanh = '1' or tblQuy.HachToanKinhDoanh is null  then '11' else '10' end as TrangThaiHachToan,
    		case when tblQuy.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong  else nv.TenNhanVien end as NguoiNopTien,
			case when nv.TenNhanVien is null then  dt.DiaChi  else nv.DiaChiCoQuan end as DiaChiKhachHang,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong_KhongDau  else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
    		case when nv.MaNhanVien is null then dt.MaDoiTuong else  nv.MaNhanVien end as MaDoiTuong,
    		case when nv.MaNhanVien is null then dt.DienThoai else  nv.DienThoaiDiDong  end as SoDienThoai1, ---- SoDienThoai1: lấy để where thôi: vì kangjin yêu cầu bảo mật sdt khách hàng ---
    			case when qct.TienMat > 0 then case when qct.TienGui > 0 then '2' else '1' end 
    			else case when qct.TienGui > 0 then '0'
    				else case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then '0' else '1' end end end as LoaiSoQuy,
    			-- check truong hop tongthu = 0
    		case when qct.TienMat > 0 then case when qct.TienGui > 0 then N'Tiền mặt, chuyển khoản' else N'Tiền mặt' end 
    			else case when qct.TienGui > 0 then N'Chuyển khoản' else 
    				case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then  N'Chuyển khoản' else N'Tiền mặt' end end end as PhuongThuc	
    							
    		from #tblQuyHD tblQuy
			 join 
    			(select 
    				 a.ID_hoaDon,
    				 sum(isnull(a.TienMat, 0)) as TienMat,
    				 sum(isnull(a.TienGui, 0)) as TienGui,
					 sum(isnull(a.TienPOS, 0)) as TienPOS,
					 sum(isnull(a.ChuyenKhoan, 0)) as ChuyenKhoan,
					 sum(isnull(a.TienDoiDiem, 0)) as TienDoiDiem,
					 sum(isnull(a.TienTheGiaTri, 0)) as TienTheGiaTri,
					 sum(isnull(a.TTBangTienCoc, 0)) as TTBangTienCoc,
    				 max(a.TenTaiKhoanPOS) as TenTaiKhoanPOS,
    				 max(a.TenTaiKhoanNOPOS) as TenTaiKhoanNOTPOS,
    				 max(a.ID_DoiTuong) as ID_DoiTuong,
    				 max(a.ID_NhanVien) as ID_NhanVien,
    				 max(a.ID_TaiKhoanNganHang) as ID_TaiKhoanNganHang,
    				 max(a.ID_KhoanThuChi) as ID_KhoanThuChi,
    				 max(a.NoiDungThuChi) as NoiDungThuChi
    			from
    			(
    				select *
    				from(
    					select 
    					qct.ID_HoaDon,
						iif(qct.HinhThucThanhToan= 1, qct.TienThu,0) as TienMat,
						iif(qct.HinhThucThanhToan= 2 or qct.HinhThucThanhToan = 3, qct.TienThu,0) as TienGui,			
						iif(qct.HinhThucThanhToan= 2, qct.TienThu,0) as TienPOS,
						iif(qct.HinhThucThanhToan= 3, qct.TienThu,0) as ChuyenKhoan,
						iif(qct.HinhThucThanhToan= 4, qct.TienThu,0) as TienDoiDiem,
						iif(qct.HinhThucThanhToan= 5, qct.TienThu,0) as TienTheGiaTri,
						iif(qct.HinhThucThanhToan= 6, qct.TienThu,0) as TTBangTienCoc,						
						qct.ID_DoiTuong, qct.ID_NhanVien, 
    					ISNULL(qct.ID_TaiKhoanNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
    					ISNULL(qct.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi,
    					case when tk.TaiKhoanPOS='1' then IIF(tk.TrangThai = 0, '<span style=""color: red; text - decoration: line - through; "" title=""Đã xóa"">' + tk.TenChuThe + '</span>', tk.TenChuThe) else '' end as TenTaiKhoanPOS,
    					case when tk.TaiKhoanPOS = '0' then IIF(tk.TrangThai = 0, '<span style=""color:red;text-decoration: line-through;"" title=""Đã xóa"">' + tk.TenChuThe + '</span>', tk.TenChuThe) else '' end as TenTaiKhoanNOPOS,
    					iif(ktc.NoiDungThuChi is null, '',
						iif(ktc.TrangThai = 0, concat(ktc.NoiDungThuChi, '{DEL}'), ktc.NoiDungThuChi)) as NoiDungThuChi,
						----11.coc, 13.khongbutru congno, 10.khong coc


						iif(qct.LoaiThanhToan = 1, '11', iif(qct.LoaiThanhToan = 3, '13', '10')) as LaTienCoc, 
						IIF(qct.ID_HoaDonLienQuan IS NULL AND qct.ID_KhoanThuChi IS NULL, 1, 0) AS LaThuChiMacDinh


						from #tblQuyHD  qhd		
						left join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDon = qhd.ID


						left
						join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID



				   left
						join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi = ktc.ID


						where qct.HinhThucThanhToan not in (4, 5, 6)-- diem, thegiatri, coc
    					)qct
					where qct.ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang


					and(qct.ID_KhoanThuChi like @ID_KhoanThuChi OR(qct.LaThuChiMacDinh = 1 AND @ID_KhoanThuChi = '00000000-0000-0000-0000-000000000001'))


					and qct.LaTienCoc like '%' + @LoaiNapTien + '%'
    			) a group by a.ID_HoaDon
    		) qct on tblQuy.ID = qct.ID_HoaDon


			left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID


			left join NS_NhanVien nv on qct.ID_NhanVien = nv.ID


		left join DM_DonVi dv on tblQuy.ID_DonVi = dv.ID


		left join NS_NhanVien nv2 on tblQuy.ID_NhanVienPT = nv2.ID


		left join DM_NguonKhachHang nguon on dt.ID_NguonKhach = nguon.ID
    	 ) tblView

		 where tblView.TrangThaiHachToan like '%' + @TrangThaiHachToan + '%'



		and tblView.TrangThaiSoQuy like '%' + @TrangThaiSoQuy + '%'



		and tblView.LoaiChungTu like '%' + @LoaiChungTu + '%'




			and tblView.ID_NhanVienPT like @ID_NhanVien



			and(exists(select ID from @tblNhanVien nv where tblView.ID_NhanVienPT = nv.ID) or tblView.NguoiTao like @nguoitao)



		and exists(select Name from dbo.splitstring(@LoaiSoQuy) where LoaiSoQuy= Name)


			and(MaHoaDon like @TxtSearch


			OR MaDoiTuong like @TxtSearch


			OR NguoiNopTien like @TxtSearch


			OR SoDienThoai1 like @TxtSearch


			OR TenNhanVien like @TxtSearch--nvlap


			OR TenNhanVienKhongDau like @TxtSearch


			OR TenDoiTuong_KhongDau like @TxtSearch-- nguoinoptien


			OR NoiDungThuUnsign like @TxtSearch
			)
    	),
    	count_cte
		as (
		select count(dt.ID) as TotalRow,
    		CEILING(count(dt.ID) / cast(@PageSize as float)) as TotalPage,
    		sum(ThuMat) as TongThuMat,
    		sum(ChiMat) as TongChiMat,
    		sum(ThuGui) as TongThuCK,
    		sum(ChiGui) as TongChiCK



		from data_cte dt
    	)
    	select*
		from data_cte dt



		cross join count_cte
		order by dt.NgayLapHoaDon desc



		OFFSET(@CurrentPage * @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
END
");

			Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_Where]
  --declare 
  @timeStart [datetime]='2024-01-01',
    @timeEnd [datetime]='2024-03-01',
    @ID_ChiNhanh [nvarchar](max)='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE',
    @maHD [nvarchar](max)='KH117',
	@ID_NhanVienLogin nvarchar(max) = '',
	@NguoiTao nvarchar(max)='admin',
	@IDViTris nvarchar(max)='',
	@IDBangGias nvarchar(max)='',
	@TrangThai nvarchar(max)='0,1,2',
	@PhuongThucThanhToan nvarchar(max)='',
	@ColumnSort varchar(max)='NgayLapHoaDon',
	@SortBy varchar(max)= 'DESC',
	@CurrentPage int =0 ,
	@PageSize int = 15
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
    	----c.DienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng ---
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
		c.KhachNo,
		(select dbo.BuTruTraHang_HDDoi(ID_HoaDon,NgayLapHoaDon,ID_HoaDonGoc, LoaiHoaDonGoc))as Butrugoc,
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
					cnLast.TongTienHDTra,					
					cnLast.ConNo
						
				from
				(
					select 
						c.ID,
						c.LoaiHoaDonGoc,
						c.TongGiaTriTra,
						----- cot TongGiaTriTra: tongTra of hdThis ---
						iif(c.LoaiHoaDonGoc = 6, 
							iif(c.LuyKeTraHang > 0, c.TongGiaTriTra, 
								---- neu LuyKeTrahang < 0 (tức trả hàng > nợ HD cũ): BuTruTrahang = max (TongTienHang)
								iif(abs(c.LuyKeTraHang) > c.TongThanhToan, c.TongThanhToan,
								iif(KhachNo >0,abs(c.LuyKeTraHang)  + c.TongGiaTriTra ,abs(c.LuyKeTraHang) )
								)
								),
						 c.LuyKeTraHang) as TongTienHDTra,					
						iif(c.ChoThanhToan is null,0, 
							----- hdDoi co congno < tongtra							
							c.TongThanhToan 
								--- neu hddoitra co LuyKeTraHang > 0 , thì gtrị bù trù = TongGiaTriTra							
								- iif(c.LoaiHoaDonGoc = 6, iif(c.LuyKeTraHang > 0,  c.TongGiaTriTra, 										
										iif(abs(c.LuyKeTraHang) > c.TongThanhToan, c.TongThanhToan, 
										
										iif(KhachNo >0,abs(c.LuyKeTraHang)  + c.TongGiaTriTra ,abs(c.LuyKeTraHang) )
										)), c.LuyKeTraHang)
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
			when @ColumnSort='' then NgayLapHoaDon end ASC,
			case when @SortBy <> 'DESC' then 0
			when @ColumnSort='' then NgayLapHoaDon end DESC,
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
    				----dt.DienThoai,	
					'' DienThoai,
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

			Sql(@"ALTER PROCEDURE [dbo].[ReportDiscountProduct_Detail]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [nvarchar](max),
	@DepartmentIDs nvarchar(max),
    @ID_NhomHang [nvarchar](max),
	@LaHangHoas [nvarchar](max),
	@LoaiChungTus [nvarchar](max),
    @TextSearch [nvarchar](max),
    @TextSearchHangHoa [nvarchar](max),
	@TxtCustomer [nvarchar](max),
    @DateFrom [nvarchar](max),
    @DateTo [nvarchar](max),
    @Status_ColumHide [int],
    @StatusInvoice [int],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    set nocount on;
    	set @DateTo = DATEADD(day,1,@DateTo)
    
		declare @tblLoaiHang table (LoaiHang int)
    	insert into @tblLoaiHang
    	select Name from dbo.splitstring(@LaHangHoas)

		declare @tblChungTu table (LoaiChungTu int)
    	insert into @tblChungTu
    	select Name from dbo.splitstring(@LoaiChungTus)

    	declare @tblNhanVienAll table (ID uniqueidentifier)
    	insert into @tblNhanVienAll
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHangHoa_XemDS_PhongBan','BCCKHangHoa_XemDS_HeThong');

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

    
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
		DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    
    	DECLARE @tblSearchHH TABLE (Name [nvarchar](max));	
    INSERT INTO @tblSearchHH(Name) select  Name from [dbo].[splitstringByChar](@TextSearchHangHoa, ' ') where Name!='';
    DECLARE @countHH int =  (Select count(*) from @tblSearchHH);
    
    	declare @tblIDNhom table (ID uniqueidentifier);
    	if @ID_NhomHang='%%' OR @ID_NhomHang =''
    		begin
    			insert into @tblIDNhom
    			select ID from DM_NhomHangHoa
    		end
    	else
    		begin
    			insert into @tblIDNhom
    			select cast(Name as uniqueidentifier) from dbo.splitstring(@ID_NhomHang)
    		end;
    
    		select 
				ID_HoaDon,
				ID_ChiTietHoaDon,
				ID_DonViQuiDoi,
				ID_LoHang,
				ID_NhanVien,
				MaHoaDon, 
				LoaiHoaDon,
    			NgayLapHoaDon,
    			MaHangHoa,
    			MaNhanVien,
    			TenNhanVien,
    			TenNhomHangHoa,
    			ID_NhomHang,
    			TenHangHoa,
    			TenHangHoaFull,
    			TenDonViTinh,
    			TenLoHang,
    			ThuocTinh_GiaTri,
    			HoaHongThucHien,
    			PTThucHien,
    			HoaHongTuVan,
    			PTTuVan,
    			HoaHongBanGoiDV,
    			PTBanGoi,
    			HoaHongThucHien_TheoYC,
    			PTThucHien_TheoYC,
    			SoLuong,
    			ThanhTien,
    			HeSo,
				ThanhTien * HeSo as GtriSauHeSo,
    			ISNULL(MaDoiTuong,'') as MaKhachHang,
    			ISNULL(TenDoiTuong,N'Khách lẻ') as TenKhachHang,
    			--ISNULL(dt.DienThoai,'') as DienThoaiKH,		
				'' as DienThoaiKH,		----- kangjin yêu cầu bảo mật SDT khách hàng --
				dt.ID_NhanVienPhuTrach,
    		case @Status_ColumHide
    					when  1 then cast(0 as float)
    					when  2 then ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  3 then ISNULL(HoaHongBanGoiDV,0.0)
    					when  4 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  5 then ISNULL(HoaHongTuVan,0.0)
    					when  6 then ISNULL(HoaHongThucHien_TheoYC,0.0) + ISNULL(HoaHongTuVan,0.0)
    					when  7 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  8 then ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  9 then ISNULL(HoaHongThucHien,0.0)
    					when  10 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  11 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    					when  12 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  13 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0)
    						when  14 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    					when  15 then ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) 
    		else ISNULL(HoaHongThucHien,0.0) + ISNULL(HoaHongTuVan,0.0) + ISNULL(HoaHongBanGoiDV,0.0) + ISNULL(HoaHongThucHien_TheoYC,0.0)
    		end as TongAll
			into #tblHoaHong
    		from
    		(
    				select 
							tbl.ID as ID_ChiTietHoaDon,
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
    						tbl.MaHoaDon,			
    						tbl.LoaiHoaDon,
    						tbl.NgayLapHoaDon,
    						tbl.ID_DoiTuong,
    						tbl.MaHangHoa,
    						tbl.ID_NhanVien,
    						TenHangHoa,
    						CONCAT(TenHangHoa,ThuocTinh_GiaTri) as TenHangHoaFull ,
    						TenDonViTinh,
    						ThuocTinh_GiaTri,
    						TenLoHang,
    						ID_NhomHang,
    						TenNhomHangHoa,
    						SoLuong,
    						HeSo,
    						TrangThaiHD,
							
    						tbl.GiaTriTinhCK_NotCP - iif(tbl.LoaiHoaDon=19,0,tbl.TongChiPhiDV) as ThanhTien,

    						case when LoaiHoaDon=6 then - HoaHongThucHien else HoaHongThucHien end as HoaHongThucHien,
    						case when LoaiHoaDon=6 then - PTThucHien else PTThucHien end as PTThucHien,
    						case when LoaiHoaDon=6 then - HoaHongTuVan else HoaHongTuVan end as HoaHongTuVan,
    						case when LoaiHoaDon=6 then - PTTuVan else PTTuVan end as PTTuVan,
    						case when LoaiHoaDon=6 then - PTBanGoi else PTBanGoi end as PTBanGoi,
    						case when LoaiHoaDon=6 then - HoaHongBanGoiDV else HoaHongBanGoiDV end as HoaHongBanGoiDV,
    						case when LoaiHoaDon=6 then - HoaHongThucHien_TheoYC else HoaHongThucHien_TheoYC end as HoaHongThucHien_TheoYC,
    						case when LoaiHoaDon=6 then - PTThucHien_TheoYC else PTThucHien_TheoYC end as PTThucHien_TheoYC
    				from
    				(Select 
						hdct.ID_HoaDon,
						hdct.ID,
    					hd.MaHoaDon,			
    					hd.LoaiHoaDon,
    					hd.NgayLapHoaDon,
    					hd.ID_DoiTuong,
    					dvqd.MaHangHoa,
						hdct.ID_DonViQuiDoi,
						hdct.ID_LoHang,
    					ck.ID_NhanVien,
						hdct.SoLuong,
						IIF(hdct.TenHangHoaThayThe is null or hdct.TenHangHoaThayThe='', hh.TenHangHoa, hdct.TenHangHoaThayThe) as TenHangHoa,
						iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,    					
    					ISNULL(hh.ID_NhomHang,N'00000000-0000-0000-0000-000000000000') as ID_NhomHang,
    					ISNULL(nhh.TenNhomHangHoa,N'') as TenNhomHangHoa,

						case when hh.ChiPhiTinhTheoPT =1 then hdct.SoLuong * hdct.DonGia * hh.ChiPhiThucHien/100
							else hh.ChiPhiThucHien * hdct.SoLuong end as TongChiPhiDV,

						---- gtri cthd (truoc/sau CK)
						iif(hd.LoaiHoaDon=36,0,case when iif(ck.TinhHoaHongTruocCK is null,0,ck.TinhHoaHongTruocCK) = 1 
							then hdct.SoLuong * hdct.DonGia
							else hdct.SoLuong * (hdct.DonGia - hdct.TienChietKhau)
							end) as GiaTriTinhCK_NotCP,

    					ISNULL(dvqd.TenDonVitinh,'')  as TenDonViTinh,
    					ISNULL(lh.MaLoHang,'')  as TenLoHang,
    					ck.HeSo,
    					Case when (dvqd.ThuocTinhGiaTri is null or dvqd.ThuocTinhGiaTri ='') then '' else '_' + dvqd.ThuocTinhGiaTri end as ThuocTinh_GiaTri,
    					Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien,
    						Case when ck.ThucHien_TuVan = 1 and TheoYeuCau !=1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongTuVan,
    					Case when ck.ThucHien_TuVan = 0 and (tinhchietkhautheo is null or tinhchietkhautheo!=4) then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTTuVan,
    						Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTBanGoi,
    					Case when ck.TinhChietKhauTheo = 4 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongBanGoiDV,
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.TienChietKhau, 0) else 0 end as HoaHongThucHien_TheoYC,   				
    					Case when ck.TheoYeuCau = 1 then ISNULL(ck.PT_ChietKhau, 0) else 0 end as PTThucHien_TheoYC,
    						case when hd.ChoThanhToan='0' then 1 else 2 end as TrangThaiHD
    			
    																																		
    				from
    				BH_NhanVienThucHien ck
    				inner join BH_HoaDon_ChiTiet hdct on ck.ID_ChiTietHoaDon = hdct.ID
    				inner join BH_HoaDon hd on hd.ID = hdct.ID_HoaDon
    				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
    				left join DM_LoHang lh on hdct.ID_LoHang = lh.ID
    				Where hd.ChoThanhToan is not null
    					and hd.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and hd.NgayLapHoaDon >= @DateFrom 
    					and hd.NgayLapHoaDon < @DateTo   							
    					and (exists (select ID from @tblNhanVien nv where ck.ID_NhanVien = nv.ID))
						and (exists (select LoaiChungTu from @tblChungTu ctu where ctu.LoaiChungTu = hd.LoaiHoaDon))
    						and 
    						((select count(Name) from @tblSearchHH b where     									
    							 dvqd.MaHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa like '%'+b.Name+'%'
    							or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%'
    							or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'	
    							)=@countHH or @countHH=0)
    			) tbl
				where tbl.LoaiHangHoa in (select LoaiHang from @tblLoaiHang)
    			) tblView
    			join NS_NhanVien nv on tblView.ID_NhanVien= nv.ID
    			left join DM_DoiTuong dt on tblView.ID_DoiTuong= dt.ID		
    			where tblView.TrangThaiHD = @StatusInvoice
    			and exists(select ID from @tblIDNhom a where ID_NhomHang= a.ID)
    			and
    				((select count(Name) from @tblSearchString b where     			
    					nv.TenNhanVien like N'%'+b.Name+'%'
    					or nv.TenNhanVienKhongDau like N'%'+b.Name+'%'
    					or nv.TenNhanVienChuCaiDau like N'%'+b.Name+'%'
    					or nv.MaNhanVien like N'%'+b.Name+'%'	
    					or tblView.MaHoaDon like '%'+b.Name+'%'							
    					)=@count or @count=0)	
				and (
				dt.MaDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong like N'%'+ @TxtCustomer +'%'
				or dt.TenDoiTuong_KhongDau like N'%'+  @TxtCustomer +'%'
				or dt.DienThoai like N'%'+  @TxtCustomer +'%'
				)


				declare @TotalRow int, @TotalPage float, 
					@TongHoaHongThucHien float, @TongHoaHongThucHien_TheoYC float,
					@TongHoaHongTuVan float, @TongHoaHongBanGoiDV float,
					@TongAllAll float, @TongSoLuong float,
					@TongThanhTien float, @TongThanhTienSauHS float

				---- count all row		
				select 
					@TotalRow= count(tbl.ID_HoaDon),
    				@TotalPage= CEILING(COUNT(tbl.ID_HoaDon ) / CAST(@PageSize as float )) ,
    				@TongHoaHongThucHien= sum(HoaHongThucHien) ,
    				@TongHoaHongThucHien_TheoYC = sum(HoaHongThucHien_TheoYC),
    				@TongHoaHongTuVan = sum(HoaHongTuVan),
    				@TongHoaHongBanGoiDV = sum(HoaHongBanGoiDV),
					@TongAllAll = sum(TongAll)
				from #tblHoaHong tbl

				---- sum and group by hoadon + idquydoi
				select 
					@TongSoLuong= sum(SoLuong) ,			   				
    				@TongThanhTien = sum(ThanhTien),
					@TongThanhTienSauHS= sum(GtriSauHeSo) 
				from 
				(
					select  
							tbl.ID_HoaDon,
							tbl.ID_DonViQuiDoi,
							tbl.ID_LoHang,
							max(tbl.SoLuong) as SoLuong,
							max(tbl.ThanhTien) as ThanhTien,
							max(tbl.GtriSauHeSo) as GtriSauHeSo
					from #tblHoaHong tbl
					group by tbl.ID_HoaDon , tbl.ID_DonViQuiDoi ,tbl.ID_LoHang	, tbl.ID_ChiTietHoaDon	
				) tbl
				

				select tbl.*, 
					isnull(nvpt.TenNhanVien,'') as TenNVPhuTrach,
					@TotalRow as TotalRow,
					@TotalPage as TotalPage,
					@TongHoaHongThucHien as TongHoaHongThucHien,
					@TongHoaHongThucHien_TheoYC as TongHoaHongThucHien_TheoYC,
					@TongHoaHongTuVan as TongHoaHongTuVan,
					@TongHoaHongBanGoiDV as TongHoaHongBanGoiDV,
					@TongAllAll as TongAllAll,
					@TongSoLuong as TongSoLuong,
					@TongThanhTien as TongThanhTien,
					@TongThanhTienSauHS as TongThanhTienSauHS
				from #tblHoaHong tbl
				left join NS_NhanVien nvpt on tbl.ID_NhanVienPhuTrach= nvpt.ID
				order by tbl.NgayLapHoaDon desc
    			OFFSET (@CurrentPage* @PageSize) ROWS
    			FETCH NEXT @PageSize ROWS ONLY

				
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_TheoKhachHang]
    @SearchString NVARCHAR(MAX),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LoaiHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang uniqueidentifier,
    @ID_NhomKhachHang [nvarchar](max),
	@LoaiChungTu [nvarchar](max),
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN	

	declare @tblChiNhanh table(ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh 
	select * from dbo.splitstring(@ID_ChiNhanh)
	
	DECLARE @LoadNhomMacDinh BIT = 0;
	IF(CHARINDEX('00000000-0000-0000-0000-000000000000', @ID_NhomKhachHang) > 0)
	BEGIN
		SET @LoadNhomMacDinh = 1;
	END
	
	DECLARE @tblIDNhoms TABLE (ID NVARCHAR(MAX))
	INSERT INTO @tblIDNhoms
	SELECT Name FROM splitstring(@ID_NhomKhachHang)

	DECLARE @tblIDNhomHang TABLE (ID NVARCHAR(MAX))
	INSERT INTO @tblIDNhomHang
	SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)

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
	
	DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select 
    	Case when nd.LaAdmin = '1' then '1' else
    	Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    	From
    	HT_NguoiDung nd	
    	where nd.ID = @ID_NguoiDung);

		DECLARE @tblSearchString TABLE (Name [nvarchar](max));
		DECLARE @count int;
		INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
		Select @count =  (Select count(*) from @tblSearchString);

	
	select 	distinct
	tblV.ID_DoiTuong as ID_KhachHang, 
	tblV.TenNhomDoiTuongs as NhomKhachHang,
	tblV.MaDoiTuong as  MaKhachHang,
	tblV.TenDoiTuong as TenKhachHang,
	----tblV.DienThoai, ---- kangjin yêu cầu bảo mật SDT khách hàng ở full bao cáo 
	round(tblV.SoLuongMua,3) as SoLuongMua,
	round(tblV.SoLuongTra,3) as SoLuongTra,
	round(tblV.GiaTriMua,3) as GiaTriMua,
	round(tblV.GiaTriTra,3) as GiaTriTra,
	round(tblV.SoLuong,3) as SoLuong,
	round(tblV.ThanhTien,3) as ThanhTien,
	round(iif(@XemGiaVon='1',tblV.TienVon,0),3) as TienVon,
	round(tblV.ThanhTien - tblV.GiamGiaHD,3) as DoanhThu,
	round(tblV.GiamGiaHD,3) as GiamGiaHD, 
	tblV.TongTienThue,
	round(iif(@XemGiaVon='1',tblV.ThanhTien - tblV.TienVon - tblV.GiamGiaHD - tblV.ChiPhi,0),3) as LaiLo,
	tblV.NguoiGioiThieu, tblV.NguoiQuanLy,
	ISNULL(tblV.TenNguonKhach,'') as TenNguonKhach,
	 tblV.ChiPhi
from(
select 
	tbldt.*,
	isnull(nvql.TenNhanVien,'') AS NguoiQuanLy,
	ISNULL(nvql.MaNhanVien,'') as MaNVQuanLy,
	isnull(dtgt.TenDoiTuong,'') AS NguoiGioiThieu,
	isnull(dtgt.MaDoiTuong,'') AS MaNguoiGioiThieu,
	isnull(dtgt.TenDoiTuong_KhongDau,'') AS NguoiGioiThieu_KhongDau,
	isnull(nk.TenNguonKhach,'') AS TenNguonKhach,
	isnull(cp.ChiPhi,0) as ChiPhi	
from 
(
Select 
	tbl.ID_DoiTuong,
	dt.MaDoiTuong,
	dt.TenDoiTuong,
	dt.TenDoiTuong_KhongDau,
	dt.TenDoiTuong_ChuCaiDau,
	dt.DienThoai,
	dt.TenNhomDoiTuongs,
	dt.ID_NguoiGioiThieu,
	dt.ID_NhanVienPhuTrach,
	dt.ID_NguonKhach,
	iif(dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs ='','00000000-0000-0000-0000-000000000000',dt.IDNhomDoiTuongs)  as IDNhomDoiTuongs,
	isnull(tbl.SoLuongMua,0) as SoLuongMua,
	isnull(tbl.SoLuongTra,0) as SoLuongTra,
	isnull(tbl.SoLuongMua,0) - isnull(tbl.SoLuongTra,0) as SoLuong,
	isnull(tbl.GiaTriTra,0) - isnull(GiamGiaHangTra,0) as GiaTriTra,
	isnull(tbl.GiaTriMua,0) - isnull(GiamGiaHangMua,0) as GiaTriMua,
	isnull(tbl.GiaTriMua,0) - isnull(tbl.GiaTriTra,0) as ThanhTien,
	isnull(tbl.GiamGiaHangMua,0) - isnull(tbl.GiamGiaHangTra,0) as GiamGiaHD,
	isnull(tbl.TongThueHangMua,0) - isnull(tbl.TongThueHangTra ,0) as TongTienThue,
	isnull(tbl.GiaVonHangMua,0) - isnull(tbl.GiaVonHangTra,0) as TienVon
	

from (

	select 
		tblMuaTra.ID_DoiTuong, 
		sum(SoLuongMua  * isnull(qd.TyLeChuyenDoi,1)) as SoLuongMua,
		sum(GiaTriMua) as GiaTriMua,
		sum(TongThueHangMua) as TongThueHangMua,
		sum(GiamGiaHangMua) as GiamGiaHangMua,
		sum(GiaVonHangMua) as GiaVonHangMua,
		sum(SoLuongTra  * isnull(qd.TyLeChuyenDoi,1)) as SoLuongTra,
		sum(GiaTriTra) as GiaTriTra,
		sum(TongThueHangTra) as TongThueHangTra,
		sum(GiamGiaHangTra) as GiamGiaHangTra,
		sum(GiaVonHangTra) as GiaVonHangTra
	from
		(
		select 
				tbl.ID_DoiTuong,tbl.ID_DonViQuiDoi, tbl.ID_LoHang,
				sum(tbl.SoLuong) as SoLuongMua,
				sum(tbl.ThanhTien) as GiaTriMua,
				sum(tbl.TienThue) as TongThueHangMua,
				sum(tbl.GiamGiaHD) as GiamGiaHangMua,
				sum(tbl.TienVon) as GiaVonHangMua,
				0 as SoLuongTra,
				0 as GiaTriTra,
				0 as TongThueHangTra,
				0 as GiamGiaHangTra,
				0 as GiaVonHangTra
		from
		(
		---- giatrimua + giavon hangmua
			select 
				ct.ID_DoiTuong,ct.ID_DonViQuiDoi, ct.ID_LoHang,
				ct.SoLuong,
				ct.ThanhTien,
				ct.TienThue,
				ct.GiamGiaHD,
				ct.TienVon
			from @tblCTHD CT			
			where (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)		
			and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)			
			) tbl
			group by tbl.ID_DoiTuong,tbl.ID_DonViQuiDoi, tbl.ID_LoHang

			---- giatritra + giavon hangtra
		union all
		select 
			hd.ID_DoiTuong,ct.ID_DonViQuiDoi, ct.ID_LoHang,
			0 as SoLuongMua,
			0 as GiaTriMua,
			0 as TongThueHangMua,
			0 as GiamGiaHangMua,
			0 as GiaVonHangMua,
			sum(SoLuong) as SoLuongTra,
			sum(ThanhTien) as GiaTriTra,
			sum(ct.TienThue * ct.SoLuong) as TienThueHangTra,
			sum(iif(hd.TongTienHang=0,0, ct.ThanhTien  * hd.TongGiamGia /hd.TongTienHang)) as GiamGiaHangTra,
			sum(ct.SoLuong * ct.GiaVon) as GiaVonHangTra
		from BH_HoaDon hd
		join BH_HoaDon_ChiTiet ct on hd.id= ct.ID_HoaDon 
		where hd.ChoThanhToan= 0
		and (ct.ID_ChiTietDinhLuong = ct.ID or ct.ID_ChiTietDinhLuong is null)
		and (ct.ID_ParentCombo is null or ct.ID_ParentCombo= ct.ID)
		and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
		and exists (select ID_DonVi from @tblChiNhanh dv where hd.ID_DonVi= dv.ID_DonVi)
		and hd.LoaiHoaDon =6
		and (ct.ChatLieu is null or ct.ChatLieu !='4') ---- khong lay ct sudung dichvu
		group by hd.ID_DoiTuong,ct.ID_DonViQuiDoi, ct.ID_LoHang
	) tblMuaTra 
	join DonViQuiDoi qd on tblMuaTra.ID_DonViQuiDoi= qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
	where 
	exists (SELECT ID FROM @tblIDNhomHang nhomhh where hh.ID_NhomHang= nhomhh.ID)
	and hh.TheoDoi like @TheoDoi
	and qd.Xoa like @TrangThai
	and iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) in (select name from dbo.splitstring(@LoaiHangHoa))
	group by tblMuaTra.ID_DoiTuong
) tbl 
join DM_DoiTuong dt on tbl.ID_DoiTuong= dt.ID
) tbldt
LEFT JOIN NS_NhanVien nvql ON nvql.ID = tbldt.ID_NhanVienPhuTrach
LEFT JOIN DM_DoiTuong dtgt ON dtgt.ID = tbldt.ID_NguoiGioiThieu
left join DM_NguonKhachHang nk on tbldt.ID_NguonKhach = nk.ID
left join (
		select ID_DoiTuong, sum(ChiPhi) as ChiPhi from @tblChiPhi group by ID_DoiTuong
		) cp on tbldt.ID_DoiTuong = cp.ID_DoiTuong
where 
 exists (select nhom1.Name 
			from dbo.splitstring(tbldt.IDNhomDoiTuongs) nhom1 
		 join @tblIDNhoms nhom2 on nhom1.Name = nhom2.ID) 
) tblV

where (select count(Name) from @tblSearchString b where 
				tblV.MaDoiTuong like '%'+b.Name+'%' 
    			OR tblV.TenDoiTuong like '%'+b.Name+'%' 
    			or tblV.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%' 
    			or tblV.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    			or tblV.TenNhomDoiTuongs like '%' +b.Name +'%' 
				or tblV.DienThoai like '%' +b.Name +'%'
    			or tblV.TenDoiTuong like '%'+b.Name+'%'
				or tblV.MaDoiTuong like '%'+b.Name+'%'
    				or tblV.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    				or tblV.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or tblV.MaNVQuanLy like '%'+b.Name+'%'
					or tblV.NguoiQuanLy like '%'+b.Name+'%'
					or tblV.MaNguoiGioiThieu like '%'+b.Name+'%'
					or tblV.NguoiGioiThieu like '%'+b.Name+'%'
					or tblV.NguoiGioiThieu_KhongDau like '%'+b.Name+'%'
    				)=@count or @count=0		


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
	select dt.ID as ID_KhachHang, dt.MaDoiTuong as MaKhachHang, dt.TenDoiTuong as TenKhachHang,
		dt.DienThoai as DienThoai1, --- kangjin yêu cầu bảo mật SDT khách hàng ở full bao cáo (DienThoai1 chỉ lấy ra để where thôi)
		dt.DiaChi, dt.TenNhomDoiTuongs,
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

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoBanHang_ChiTiet_Page] --GDV0000000068
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
			hh.ID,
			hh.TenHangHoa,
			qd.MaHangHoa,
			iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa = '1', 1, 2), hh.LoaiHangHoa) as LoaiHangHoa,
			concat(hh.TenHangHoa, qd.ThuocTinhGiaTri) as TenHangHoaFull,
			qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
			ISNULL(nhh.TenNhomHangHoa,  N'Nhóm hàng hóa mặc định') as TenNhomHangHoa,
			lo.MaLoHang as TenLoHang,
			qd.TenDonViTinh,
			c.IDChiTietHD,
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
			'' as DienThoai, ---- kangjin yêu cầu bảo mật SDT khách hàng ở full bao cáo
			dt.GioiTinhNam,
			dt.ID_NguoiGioiThieu, dt.ID_NguonKhach,
			c.ChiPhi,
			c.LoaiHoaDon,
			c.ID_ChiTietGoiDV,
			iif(c.TenHangHoaThayThe is null or c.TenHangHoaThayThe='', hh.TenHangHoa, c.TenHangHoaThayThe) as TenHangHoaThayThe			
		from 
		(
		select 
			b.IDChiTietHD,
			b.ID_ChiTietGoiDV,
			b.LoaiHoaDon,b.NgayLapHoaDon, b.MaHoaDon, b.ID_PhieuTiepNhan, b.ID_DoiTuong, b.ID_NhanVien,-- b.TenNhanVien,
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
			ct.TienThue,
			ct.GiamGiaHD,			
			ct.ID as IDChiTietHD,
			ct.ID_DonViQuiDoi, ct.ID_LoHang,
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
			ct.SoLuong * (ct.DonGia - ct.TienChietKhau) as ThanhTien, ----- !important: kangjin: sử dung từ GDV: vẫn lấy Thành tiền từ GDV mua
			iif(ct.SoLuong =0, 0, ct.TienVon/ct.SoLuong) as GiaVon,			
			ct.TienVon,
			isnull(cp.ChiPhi,0) as ChiPhi,
			ct.ID_ChiTietGoiDV
		from @tblCTHD ct	
		left join @tblChiPhi cp on ct.ID= cp.ID_ParentCombo	
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
				--	or c.TenNhanVien like N'%'+b.Name+'%'
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
				isnull(gt.TenDoiTuong,'') as NguoiGioiThieu	,
				isnull(maNV.NVThucHien,'') as MaNhanVien,
				isnull(tenNV.NVThucHien,'') as TenNhanVien,
				isnull(ctmOut.MaHoaDon,'') as MaGoiDichVu
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
    		FETCH NEXT @pageSize ROWS ONLY	
			) tbl
			left join DM_NguonKhachHang nk on tbl.ID_NguonKhach= nk.ID
			left join DM_DoiTuong gt on tbl.ID_NguoiGioiThieu= gt.ID 	
			left join
			(
			-- get TenNV thuchien of cthd
			select tblCT.IDChiTietHD as ID_ChiTietHD ,
				 (
						select nv.TenNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						where nvth.ID_ChiTietHoaDon = tblCT.IDChiTietHD
										
						For XML PATH ('')
					) NVThucHien
				from #tblView tblCT 
			) tenNV on tbl.IDChiTietHD = tenNV.ID_ChiTietHD
			left join
			(
			-- get MaNV nvthuchien of cthd
			select tblCT.IDChiTietHD as ID_ChiTietHD ,
				 (
						select nv.MaNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						where nvth.ID_ChiTietHoaDon = tblCT.IDChiTietHD										
						For XML PATH ('')
					) NVThucHien
				from #tblView tblCT 
			) maNV on tbl.IDChiTietHD = maNV.ID_ChiTietHD
			left join
			(
			----- get maGDV: sudung tu GDV nao ----
				select gdv.MaHoaDon, ctMua.ID_ChiTietMua
				from
				(
					select ctm.ID_HoaDon, ctm.ID as ID_ChiTietMua
					from BH_HoaDon_ChiTiet ctm
					where exists (select id from #tblView cthd 
					where cthd.ID_ChiTietGoiDV is not null and ctm.ID = cthd.ID_ChiTietGoiDV) ---- chi lay HD sudung tu GDV
				)ctMua
				join BH_HoaDon gdv on ctMua.ID_HoaDon = gdv.ID
			) ctmOut on ctmOut.ID_ChiTietMua = tbl.ID_ChiTietGoiDV
			order by NgayLapHoaDon desc
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoDichVu_NhatKySuDungChiTiet]
    @Text_Search [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
	@ThoiHan [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	declare @dtNow datetime = format(dateadd(day,1, getdate()),'yyyy-MM-dd')

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@Text_Search, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tblChiNhanh table (ID_DonVi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@ID_ChiNhanh)

	---- ctsdung ----
select 
	hd.MaHoaDon,
	hd.ID_DoiTuong,
	hd.ID_NhanVien,
	hd.NgayLapHoaDon,
	hd.ID_Xe,
	ct.SoLuong,	
	ct.DonGia,
	ct.GiaVon * ct.SoLuong as TienVon,
	ct.GhiChu,
	ct.ID_DonViQuiDoi,
	ct.ID_LoHang,
	ct.ID_ChiTietDinhLuong,
	ct.ID_ChiTietGoiDV,
	ct.ID
into #tmpCTSD
from BH_HoaDon_ChiTiet ct
join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
where hd.ChoThanhToan = 0
and hd.NgayLapHoaDon between @timeStart and @timeEnd
and exists (select cn.ID_DonVi from @tblChiNhanh cn where cn.ID_DonVi = hd.ID_DonVi)
and hd.LoaiHoaDon in (1,6,25)
and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
and ct.ChatLieu ='4'
order by hd.NgayLapHoaDon desc

---- ctmua ----
select *
into #GDV
from
(
select 
	hd.MaHoaDon,
	hd.NgayLapHoaDon,
	hd.NgayApDungGoiDV,
	hd.HanSuDungGoiDV,	
	iif(hd.HanSuDungGoiDV is null,'1', iif(@dtNow > hd.HanSuDungGoiDV,'0','1')) as ThoiHan,
	ct.ID
from BH_HoaDon_ChiTiet ct
join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
where hd.LoaiHoaDon= 19
and exists (select ctsd.ID from #tmpCTSD ctsd where ctsd.ID_ChiTietGoiDV = ct.ID )
)a 	WHERE a.ThoiHan like @ThoiHan

---- ctxuatkho (hdle or suachua)---
select dluong.ID_ChiTietDinhLuong,
	sum(isnull(dluong.GiaVon,0) * dluong.SoLuong) as TienVon
into #tblGiaVon
from
(
select iif(ct.ID_ChiTietDinhLuong is null, ct.ID,  ct.ID_ChiTietDinhLuong) as ID_ChiTietDinhLuong,
	ct.SoLuong, ct.GiaVon
from BH_HoaDon_ChiTiet ct
where exists (select ctsd.ID from #tmpCTSD ctsd where ctsd.ID = ct.ID_ChiTietDinhLuong )
and ct.ID != ct.ID_ChiTietDinhLuong
) dluong
group by dluong.ID_ChiTietDinhLuong

----- tblview
select *,
CONCAT( b.TenHangHoa , b.ThuocTinh_GiaTri) as TenHangHoaFull
from
(
select 
	ctm.MaHoaDon as MaGoiDV,
	ctsd.MaHoaDon,
	ctsd.NgayLapHoaDon,
	qd.MaHangHoa,
	hh.TenHangHoa,
	qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
	qd.TenDonViTinh,
	lo.MaLoHang as TenLoHang,
	ctsd.SoLuong,
	ctsd.GhiChu,
	ctsd.SoLuong * ctsd.DonGia as GiaTriSD,
	ceiling(iif(hh.LaHangHoa ='1', ctsd.TienVon, isnull(gv.TienVon,0))) as TienVon,
	dt.MaDoiTuong as MaKhachHang,
	dt.TenDoiTuong as TenKhachHang,
	iif(dt.GioiTinhNam='1', N'Nam', N'Nữ') as GioiTinh,
	----- dt.DienThoai, ---- kangjin yêu cầu bảo mật sdt khách hàng
	gt.TenDoiTuong as NguoiGioiThieu,
	nk.TenNguonKhach,
	iif(dt.TenNhomDoiTuongs is null or dt.TenNhomDoiTuongs='',N'Nhóm mặc định', dt.TenNhomDoiTuongs) as NhomKhachHang,
	iif(hh.ID_NhomHang is null, '00000000-0000-0000-0000-000000000000', hh.ID_NhomHang)  as ID_NhomHang,
	nhh.TenNhomHangHoa as TenNhomHang,
	ISNULL(nv.NhanVienChietKhau,'') as NhanVienChietKhau,
	xe.BienSo,
	chuxe.MaDoiTuong as MaChuXe,
	chuxe.TenDoiTuong as TenChuXe
from #tmpCTSD ctsd
left join #tblGiaVon gv on ctsd.ID = gv.ID_ChiTietDinhLuong
left join #GDV ctm on ctsd.ID_ChiTietGoiDV = ctm.ID
join DonViQuiDoi qd on ctsd.ID_DonViQuiDoi= qd.ID
left join DM_LoHang lo on ctsd.ID_LoHang= lo.ID
left join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
left join DM_NhomHangHoa nhh on hh.ID_NhomHang = nhh.ID
left join DM_DoiTuong dt on ctsd.ID_DoiTuong= dt.ID
left join DM_DoiTuong gt on dt.ID_NguoiGioiThieu = gt.ID
left join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
left join Gara_DanhMucXe xe on ctsd.ID_Xe= xe.ID
left join DM_DoiTuong chuxe on xe.ID_KhachHang= chuxe.ID
left join (Select Main.ID_ChiTietHoaDon,
							Main.nhanvienchietkhau,
							Main.nhanvienchietkhau_khongdau,
							Main.nhanvienchietkhau_chucaidau    		
    						From
    						(
    						Select distinct hh_tt.ID_ChiTietHoaDon,
    						(
    							Select tt.TenNhanVien + ' - ' AS [text()]
    							From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
    							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) tt
    							Where tt.ID_ChiTietHoaDon = hh_tt.ID_ChiTietHoaDon
    							For XML PATH ('')
    						) nhanvienchietkhau,
							(
    							Select tt.TenNhanVienKhongDau + ' - ' AS [text()]
    							From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVienKhongDau from BH_NhanVienThucHien nvth 
    							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) tt
    							Where tt.ID_ChiTietHoaDon = hh_tt.ID_ChiTietHoaDon
    							For XML PATH ('')
    						) nhanvienchietkhau_khongdau,
							(
    							Select tt.TenNhanVienChuCaiDau + ' - ' AS [text()]
    							From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVienChuCaiDau from BH_NhanVienThucHien nvth 
    							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) tt
    							Where tt.ID_ChiTietHoaDon = hh_tt.ID_ChiTietHoaDon
    							For XML PATH ('')
    						) nhanvienchietkhau_chucaidau
    						From (select nvth.ID_ChiTietHoaDon, nv.TenNhanVien from BH_NhanVienThucHien nvth 
    							inner join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID) hh_tt
    						) Main) as nv on ctsd.ID = nv.ID_ChiTietHoaDon
where  hh.LaHangHoa like @LaHangHoa
    		and hh.TheoDoi like @TheoDoi
    		and qd.Xoa like @TrangThai
			AND ((select count(Name) from @tblSearchString b 
			where hh.TenHangHoa_KhongDau like '%'+b.Name+'%'    		
    			or hh.TenHangHoa_KyTuDau like '%'+b.Name+'%' 
    			or hh.TenHangHoa like '%'+b.Name+'%'
    			or lo.MaLoHang like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KhongDau like '%'+b.Name+'%'
    			or nhh.TenNhomHangHoa_KyTuDau like '%'+b.Name+'%'
    			or qd.TenDonViTinh like '%'+b.Name+'%'
    			or qd.ThuocTinhGiaTri like '%'+b.Name+'%'
				or dt.DienThoai like '%'+b.Name+'%'
				or dt.TenDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'
				or ctsd.MaHoaDon like '%'+b.Name+'%'
				or ctm.MaHoaDon like '%'+b.Name+'%'
				or ISNULL(nv.NhanVienChietKhau,'') like '%'+b.Name+'%'
				or nv.NhanVienChietKhau_ChuCaiDau like '%'+b.Name+'%'
				or nv.NhanVienChietKhau_KhongDau like '%'+b.Name+'%'
				or xe.BienSo like '%'+b.Name+'%'
				or chuxe.MaDoiTuong like '%'+b.Name+'%'
				or chuxe.TenDoiTuong like '%'+b.Name+'%'
				or chuxe.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				)=@count or @count=0)
				
) b where exists (select ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang) nhom where b.ID_NhomHang = nhom.ID)
order by b.NgayLapHoaDon desc
			
END");
        }
        
        public override void Down()
        {
			DropStoredProcedure("[dbo].[GetAllChiTietHoaDon_afterTraHang]");
        }
    }
}
