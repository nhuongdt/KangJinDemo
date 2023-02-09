namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Kangjin_AddUpdateSP_20230208 : DbMigration
    {
        public override void Up()
        {
            Sql(@"create type TblID as table
(
	ID uniqueidentifier not null primary key
)");

			CreateStoredProcedure(name: "[dbo].[BaoCaoNhomHoTro]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				IDNhomHoTros = p.String(),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

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
			where spht.LaSanPhamNgayThuoc= 2
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
			----and hd.NgayLapHoaDon < @DateTo
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
		select 
			
			cus.MaDoiTuong,
			cus.TenDoiTuong,
			dv.TenDonVi,
			nvpt.TenNhanVien,
			tView.*,
			case 
				when tView.GtriHoTroVND = 0 then tView.DaHoTro
			else tView.DaHoTro/tView.GtriHoTroVND * 100
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
			drop table #tblHDCus");

			CreateStoredProcedure(name: "[dbo].[GetHangCungLoai_byID]", parametersAction: p => new
			{
				ID_HangCungLoai = p.Guid(),
				IDChiNhanh = p.Guid()
			}, body: @"SET NOCOUNT ON;

		select 
			hh.*,
			qd.ID as ID_DonViQuiDoi,
			qd.MaHangHoa,
			nhom.TenNhomHangHoa as NhomHangHoa,					
			qd.Xoa,
			qd.TenDonViTinh,
			qd.ThuocTinhGiaTri,
			qd.GiaBan,
			ISNULL(tk.TonKho,0) as TonKho,
			isnull(gv.GiaVon,0) as GiaVon,
			case hh.LoaiHangHoa 
			when 1 then N'Hàng hóa'
			when 2 then N'Dịch vụ'
			when 3 then N'Combo'
		end as sLoaiHangHoa
		from
		(
		  select 
				hh.ID,				
				hh.ID_Xe,				
				hh.TenHangHoa,					
				hh.LaHangHoa,
				hh.GhiChu,
				hh.LaChaCungLoai,
				hh.DuocBanTrucTiep,
				hh.TheoDoi,
				hh.NgayTao,
				hh.ID_HangHoaCungLoai,
				hh.ID_NhomHang as ID_NhomHangHoa,
					
				iif(hh.LoaiHangHoa is null,iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa,
				isnull(hh.SoPhutThucHien,0) as SoPhutThucHien,
				isnull(hh.DichVuTheoGio,0) as DichVuTheoGio,	
				isnull(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
				isnull(hh.ChietKhauMD_NVTheoPT,'1') as ChietKhauMD_NVTheoPT,
				isnull(hh.DuocTichDiem,0) as DuocTichDiem,
				iif(hh.QuanLyBaoDuong is null,0, hh.QuanLyBaoDuong) as QuanLyBaoDuong,
				iif(hh.LoaiBaoDuong is null,0, hh.LoaiBaoDuong) as LoaiBaoDuong,
				iif(hh.SoKmBaoHanh is null,0, hh.SoKmBaoHanh) as SoKmBaoHanh,
				iif(hh.HoaHongTruocChietKhau is null,0, hh.HoaHongTruocChietKhau) as HoaHongTruocChietKhau,		
				isnull(hh.TonToiDa,0) as TonToiDa,
				isnull(hh.TonToiThieu,0) as TonToiThieu
		   from DM_HangHoa hh 
		   where ID_HangHoaCungLoai= @ID_HangCungLoai
		) hh
		left join DonViQuiDoi qd on qd.ID_HangHoa= hh.ID
		left join DM_NhomHangHoa nhom on hh.ID_NhomHangHoa= nhom.ID	
		left join DM_HangHoa_TonKho tk on qd.ID = tk.ID_DonViQuyDoi and tk.ID_DonVi= @IDChiNhanh
		left join DM_GiaVon gv on qd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi= @IDChiNhanh
		where qd.Xoa='0'
		order by qd.MaHangHoa");

			CreateStoredProcedure(name: "[dbo].[GetInfor_PhieuTatToanTheGiaTri]", parametersAction: p => new
			{
				ID = p.Guid()
			}, body: @"SET NOCOUNT ON;

	declare @ID_HoaDon uniqueidentifier = (select top 1 ID_HoaDon from BH_HoaDon where ID= @ID)

	select 
		ptt.*,
		dt.MaDoiTuong,
		dt.TenDoiTuong,
		dt.DienThoai,

		tgt.MaHoaDon as MaTheGiaTri,
		tgt.PhaiThanhToan as GiaTriNap,
		isnull(thuTGT.TienThu,0) as KhachDaTra,
		tgt.PhaiThanhToan - isnull(thuTGT.TienThu,0) as ConNo
	from
	(
    select 
		hd.ID,
		hd.ID_DoiTuong,
		hd.ID_DonVi,
		hd.ID_NhanVien,
		hd.ID_HoaDon,
		hd.LoaiHoaDon,
		hd.MaHoaDon,
		hd.NgayLapHoaDon,
		hd.TongTienHang,
		hd.PhaiThanhToan,
		hd.TongThanhToan,
		hd.DienGiai,
		hd.NguoiTao,
		hd.ChoThanhToan
	from BH_HoaDon hd
    where hd.ID= @ID	
	) ptt
	left join DM_DoiTuong dt on ptt.ID_DoiTuong= dt.ID
	left join BH_HoaDon tgt on ptt.ID_HoaDon= tgt.ID
	left join
	(
		select qct.ID_HoaDonLienQuan,
		sum(qct.TienThu) as TienThu
		from Quy_HoaDon qhd
		join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
		where qct.ID_HoaDonLienQuan= @ID_HoaDon
		and (qhd.TrangThai is null or qhd.TrangThai='1')
		group by qct.ID_HoaDonLienQuan
	) thuTGT on tgt.ID = thuTGT.ID_HoaDonLienQuan");

			CreateStoredProcedure(name: "[dbo].[TGT_GetNhatKyTatToanCongNo]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				ID_DoiTuong = p.Guid(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID_DonVi uniqueidentifier)
		if isnull(@IDChiNhanhs,'')!=''
    		insert into @tblChiNhanh
    		select name from dbo.splitstring(@IDChiNhanhs)
		else
			set @IDChiNhanhs =''
    
     
    	;with data_cte
    		as(		   		
    		   select hd.ID, 
    					hd.ID_DonVi,
    					hd.ID_DoiTuong,
    					hd.ID_NhanVien,
    					hd.MaHoaDon, 
    					hd.LoaiHoaDon,
    					hd.NgayLapHoaDon, 			
    					hd.TongTienHang,
    					hd.PhaiThanhToan,
    					hd.TongThanhToan,
    					hd.DienGiai,   					
    					hd.NguoiTao,   					
    					hd.ChoThanhToan,   				
    					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai,
    					case when hd.ChoThanhToan is null then N'Đã hủy' else N'Hoàn thành' end as STrangThai
    			from BH_HoaDon hd   		
    			where LoaiHoaDon = 42
    			and hd.ID_DoiTuong= @ID_DoiTuong
    			and (@IDChiNhanhs='' or exists (select ID_DonVi from @tblChiNhanh cn where hd.ID_DonVi= cn.ID_DonVi))
    			),
    			count_cte
    		as (
    			select count(ID) as TotalRow,
    				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    				sum(TongTienHang) as SumTongTienHang
    			from data_cte
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY");

			Sql(@"CREATE PROCEDURE [dbo].[TinhCongNo_HDTra]
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
					iif(LuyKeCuoiCung > HDTra_PhaiThanhToan, HDTra_PhaiThanhToan ,
						iif(LuyKeCuoiCung + HDDoi_PhaiThanhToan > HDTra_PhaiThanhToan,HDTra_PhaiThanhToan, LuyKeCuoiCung + HDDoi_PhaiThanhToan)
						), 
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

END");

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
			dt.DienThoai, dt.GioiTinhNam,
			dt.ID_NguoiGioiThieu, dt.ID_NguonKhach,
		--	c.TenNhanVien,
			c.ChiPhi,
			c.LoaiHoaDon,
			iif(c.TenHangHoaThayThe is null or c.TenHangHoaThayThe='', hh.TenHangHoa, c.TenHangHoaThayThe) as TenHangHoaThayThe			
		from 
		(
		select 
			b.IDChiTietHD,
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
		--	nvien.NVThucHien as TenNhanVien,
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
			ct.ThanhTien,
			iif(ct.SoLuong =0, 0, ct.TienVon/ct.SoLuong) as GiaVon,			
			ct.TienVon,
			isnull(cp.ChiPhi,0) as ChiPhi
	from @tblCTHD ct	
	left join @tblChiPhi cp on ct.ID= cp.ID_ParentCombo
	--left join
	--	(
	--	-- get nvthuchien of hdbl
	--		select distinct th.ID_ChiTietHoaDon ,
	--			 (
	--					select nv.TenNhanVien +', '  AS [text()]
	--					from BH_NhanVienThucHien nvth
	--					join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
	--					join BH_HoaDon_ChiTiet ct on nvth.ID_ChiTietHoaDon = ct.ID
	--					join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
	--					where nvth.ID_ChiTietHoaDon = th.ID_ChiTietHoaDon
	--					and (hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd) 
 --   					and hd.ChoThanhToan = 0 
	--					and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID)						
	--					For XML PATH ('')
	--				) NVThucHien
	--			from BH_NhanVienThucHien th 
	--	) nvien on ct.ID = nvien.ID_ChiTietHoaDon
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
				isnull(tenNV.NVThucHien,'') as TenNhanVien				
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
			order by NgayLapHoaDon desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_CongNo_v2]
    @TextSearch [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);
    
    DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    		IF @timeChotSo != null
    		BEGIN
    			IF @timeChotSo < @timeStart
    			BEGIN
    		 SELECT 
    			MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    			MAX(b.MaKhachHang) as MaDoiTac,
    			MAX(b.TenKhachHang) as TenDoiTac,
				MAX(b.MaNhanVien) as MaNhanVien,
    			MAX(b.TenNhanVien) as TenNhanVien,
    			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    			MAX(b.TongTienChi) as TongTienChi, 
    			MAX(b.TongTienThu) as TongTienThu,
    			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    		  dt.MaDoiTuong AS MaKhachHang, 
    		  dt.TenDoiTuong AS TenKhachHang,
			  nv.MaNhanVien,
			  nv.TenNhanVien,
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    		  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    		SUM(HangHoa.GhiNo) as GhiNo,
    		SUM(HangHoa.GhiCo) as GhiCo
    		FROM
    		(
    			SELECT
    				td.ID_DoiTuong AS ID_KhachHang,
    				SUM(td.CongNo) + SUM(td.DoanhThu) + SUM(td.TienChi) - SUM(td.TienThu) - SUM(td.GiaTriTra) AS NoDauKy,
    				0 AS GhiNo,
    				0 AS GhiCo
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    					ID_KhachHang As ID_DoiTuong,
    					CongNo,
    					0 AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    			-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				SUM(bhd.TongThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
				--chi phí hóa đơn
					UNION ALL
						SELECT 
    						cp.ID_NhaCungCap,
							0 AS CongNo,
    						SUM(cp.ThanhTien) AS GiaTriTra,
    						0 AS DoanhThu,
    						0 AS TienThu,
    						0 AS TienChi
    					FROM BH_HoaDon bhd
						INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    					WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    					AND bhd.ID_DonVi = @ID_ChiNhanh
    						AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    					GROUP BY cp.ID_NhaCungCap
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' 
				AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) 
				AND qhd.NgayLapHoaDon between  @timeChotSo AND  @timeStart
				AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    			SELECT
    				pstv.ID_DoiTuong AS ID_KhachHang,
    				0 AS NoDauKy,
    				SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    				SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo
    			FROM
    			(
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.TongThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25)  AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
				--Chi phí hóa đơn sửa chữa
				UNION ALL
				SELECT 
    				cp.ID_NhaCungCap,
    				0 AS GiaTriTra,
    				SUM(cp.ThanhTien) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
				INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    			WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY cp.ID_NhaCungCap
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    		)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    				left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
					left join NS_NhanVien nv on dt.ID_NhanVienPhuTrach = nv.ID
    				where dt.TheoDoi='0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    				LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		ORDER BY MAX(b.MaKhachHang) DESC
    			END
    			ELSE IF @timeChotSo > @timeEnd
    			BEGIN
    				SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
			MAX(b.MaNhanVien) as MaNhanVien,
    			MAX(b.TenNhanVien) as TenNhanVien,
    			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
    			MAX(b.TongTienThu) as TongTienThu,
    			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
			 nv.MaNhanVien,
			  nv.TenNhanVien,
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    		SUM(HangHoa.GhiNo) as GhiNo,
    		SUM(HangHoa.GhiCo) as GhiCo,
    		SUM(HangHoa.NoCuoiKy) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    			0 AS GhiNo,
    			0 AS GhiCo,
    			SUM(td.CongNo) - SUM(td.DoanhThu) - SUM(td.TienChi) + SUM(td.TienThu) + SUM(td.GiaTriTra) AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    			SELECT 
    			ID_KhachHang As ID_DoiTuong,
    			CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM ChotSo_KhachHang
    			where ID_DonVi = @ID_ChiNhanh
    			UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    			SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    			SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    			SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo,
    			0 AS NoCuoiKy
    			FROM
    			(
    			SELECT 
    			bhd.ID_DoiTuong,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
						left join NS_NhanVien nv on dt.ID_NhanVienPhuTrach = nv.ID
    				where dt.TheoDoi = '0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
    			END
    			ELSE
    			BEGIN
    			SELECT 
    			 MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
			MAX(b.MaNhanVien) as MaNhanVien,
    			MAX(b.TenNhanVien) as TenNhanVien,
    			Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    			Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
    			MAX(b.TongTienThu) as TongTienThu,
    			Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    			Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT a.ID_KhachHang, 
    	  dt.MaDoiTuong AS MaKhachHang, 
    	  dt.TenDoiTuong AS TenKhachHang,
		  nv.MaNhanVien,
			  nv.TenNhanVien,
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo,0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    		SUM(HangHoa.NoDauKy) as NoDauKy, 
    			SUM(HangHoa.GhiNo) as GhiNo,
    			SUM(HangHoa.GhiCo) as GhiCo,
    			SUM(HangHoa.NoCuoiKy) as NoCuoiKy
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(td.CongNo) - SUM(td.DoanhThu) - SUM(td.TienChi) + SUM(td.TienThu) + SUM(td.GiaTriTra) AS NoDauKy,
    				SUM(td.DoanhThu) + SUM(td.TienChi) AS GhiNo,
    				SUM(td.TienThu) + SUM(td.GiaTriTra) AS GhiCo,
    				0 AS NoCuoiKy
    			FROM
    			(
    			-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				CongNo AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    				-- Doanh thu từ bán hàng từ ngày chốt sổ tới thời gian bắt đầu
    			SELECT 
    			bhd.ID_DoiTuong,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    				UNION ALL
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn
    				SELECT
    			pstv.ID_DoiTuong AS ID_KhachHang,
    			0 AS NoDauKy,
    				SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiNo,
    				SUM(pstv.TienThu) + SUM(pstv.GiaTriTra) AS GhiCo,
    			    SUM(pstv.CongNo) + SUM(pstv.DoanhThu) + SUM(pstv.TienChi) - SUM(pstv.TienThu) - SUM(pstv.GiaTriTra) AS NoCuoiKy
    			FROM
    			(
    				-- Chốt sổ
    				SELECT 
    				ID_KhachHang As ID_DoiTuong,
    				CongNo AS CongNo,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    				FROM ChotSo_KhachHang
    				where ID_DonVi = @ID_ChiNhanh
    				UNION ALL
    
    			SELECT 
    			bhd.ID_DoiTuong,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			SUM(bhd.TongThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,25) AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- sổ quỹ
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			SUM(qhdct.TienThu) AS TienThu,
    			0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS CongNo,
    			0 AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
    				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
					AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
						left join NS_NhanVien nv on dt.ID_NhanVienPhuTrach = nv.ID
    				where dt.TheoDoi ='0'
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
    			END
    		END
    		ELSE
    		BEGIN
    			SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
			MAX(b.MaNhanVien) as MaNhanVien,
    			MAX(b.TenNhanVien) as TenNhanVien,
    		Case when (b.NoDauKy >= 0) then b.NoDauKy else 0 end as PhaiThuDauKy,
    		Case when (b.NoDauKy < 0) then b.NoDauKy *(-1) else 0 end as PhaiTraDauKy,
    		MAX(b.TongTienChi) as TongTienChi, 
    		MAX(b.TongTienThu) as TongTienThu,
    		Case when (b.NoCuoiKy >= 0) then b.NoCuoiKy else 0 end as PhaiThuCuoiKy,
    		Case when (b.NoCuoiKy < 0) then b.NoCuoiKy *(-1) else 0 end as PhaiTraCuoiKy
    	 FROM
    	(
    	  SELECT 
    			  a.ID_KhachHang, 
    		  dt.MaDoiTuong AS MaKhachHang, 
    		  dt.TenDoiTuong AS TenKhachHang,
			  nv.MaNhanVien,
			  nv.TenNhanVien,
    		  a.NoDauKy,
    		 --a.GhiNo As TongTienChi,
    		 -- a.GhiCo As TongTienThu,
    			  iif(a.GhiNo<= 0, iif(a.GhiCo < 0, -a.GhiCo, 0 ), a.GhiNo) as TongTienChi,
    			  iif(a.GhiCo <=0, iif(a.GhiNo < 0, -a.GhiNo, 0 ), a.GhiCo) as TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    		  Case When dtn.ID_NhomDoiTuong is null then
    		  '00000000-0000-0000-0000-000000000000'
    			  else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			  dt.LoaiDoiTuong
    	  FROM
    	  (
    	  SELECT HangHoa.ID_KhachHang,
    			SUM(HangHoa.NoDauKy) as NoDauKy, 
    			SUM(HangHoa.GhiNo) as GhiNo,
    			SUM(HangHoa.GhiCo) as GhiCo,
    			SUM(HangHoa.NoDauKy + HangHoa.GhiNo - HangHoa.GhiCo) as NoCuoiKy
    		FROM
    		(
    			SELECT
    				td.ID_DoiTuong AS ID_KhachHang,
    				SUM(td.DoanhThu) + SUM(td.TienChi) - SUM(td.TienThu) - SUM(td.GiaTriTra) AS NoDauKy,
    				0 AS GhiNo,
    				0 AS GhiCo,
    				0 AS NoCuoiKy
    			FROM
    			(
    
    				---- CÔNG NỢ ĐẦU KỲ
    				---- doanhthu khachhang
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
					--chi phí hóa đơn
					UNION ALL
						SELECT 
    						cp.ID_NhaCungCap,
    						SUM(cp.ThanhTien) AS GiaTriTra,
    						0 AS DoanhThu,
    						0 AS TienThu,
    						0 AS TienChi
    					FROM BH_HoaDon bhd
						INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    					WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeStart
    					AND bhd.ID_DonVi = @ID_ChiNhanh
    						AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    					GROUP BY cp.ID_NhaCungCap
    				union all
    				---- doanhthu baohiem
    			SELECT 
    				bhd.ID_BaoHiem,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToanBaoHiem) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon in (1,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				and bhd.ID_BaoHiem is not null
    			GROUP BY bhd.ID_BaoHiem
    
    			-- trahang of khachhag
    			UNION All
    			SELECT bhd.ID_DoiTuong,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (4,6) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
    
    			-- thucthu khachhang + baohiem + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
				AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND qhdct.ID_DoiTuong is not null
    			
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    				-- phieuchi khachhang + ncc+ baohiem
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong AS ID_KhachHang,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon    		
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
				AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_NhanVien is null
					AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND qhdct.ID_DoiTuong is not null
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    			UNION ALL
    
    				-- Công nợ phát sinh trong khoảng thời gian truy vấn (---- CÔNG NỢ TRONG KỲ  ------)
    			SELECT
    				pstv.ID_DoiTuong AS ID_KhachHang,
    				0 AS NoDauKy,
    					SUM(pstv.DoanhThu) + SUM(pstv.TienChi) AS GhiCo,
    				SUM(pstv.GiaTriTra) + SUM(pstv.TienThu) AS GhiNo,
    			
    				0 AS NoCuoiKy
    			FROM
    			(
    				-- KhachHang: doanh thu
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon  in (1,7,19,22,25) AND bhd.ChoThanhToan = '0' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
				--chi phí hóa đơn
				UNION ALL
					SELECT 
    					cp.ID_NhaCungCap,
    					SUM(cp.ThanhTien) AS GiaTriTra,
    					0 AS DoanhThu,
    					0 AS TienThu,
    					0 AS TienChi
    				FROM BH_HoaDon bhd
					INNER JOIN BH_HoaDon_ChiPhi cp ON cp.ID_HoaDon = bhd.ID
    				WHERE bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					AND cp.ID_NhaCungCap not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				GROUP BY cp.ID_NhaCungCap
    				union all
    				---- doanhthu baohiem
    			SELECT 
    				bhd.ID_BaoHiem,
    					0 AS GiaTriTra,    				
    				sum( bhd.PhaiThanhToanBaoHiem) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE bhd.LoaiHoaDon in (1,25) AND bhd.ChoThanhToan = '0'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				and bhd.ID_BaoHiem is not null
    			GROUP BY bhd.ID_BaoHiem
    
    			-- khachhang: trahang
    			UNION All
    			SELECT bhd.ID_DoiTuong,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    				AND bhd.ID_DoiTuong is not null
    			GROUP BY bhd.ID_DoiTuong
    
    			--  phieuthu: kh + bh + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				SUM(qhdct.TienThu) AS TienThu,
    				0 AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd		
    			AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
				AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					AND qhdct.ID_NhanVien is null
    				AND qhdct.ID_DoiTuong is not null
    				--AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    
    					-- phieuchi: kh + bh + ncc
    			UNION ALL
    			SELECT 
    				qhdct.ID_DoiTuong,
    				0 AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				SUM(qhdct.TienThu) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			WHERE qhd.LoaiHoaDon = '12' 
				AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
    			AND (qhd.PhieuDieuChinhCongNo != 3 OR qhd.PhieuDieuChinhCongNo IS NULL)
				AND qhd.ID_DonVi = @ID_ChiNhanh
    				AND qhdct.ID_DoiTuong not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
					AND qhdct.ID_NhanVien is null
    				AND qhdct.ID_DoiTuong is not null
					and qhdct.HinhThucThanhToan != 6
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    				left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
					left join NS_NhanVien nv on dt.ID_NhanVienPhuTrach = nv.ID
    				where  dt.TheoDoi ='0' 
    				and dt.loaidoituong in (select * from splitstring(@loaiKH)) 
    					AND ((select count(Name) from @tblSearch b where     			
    		dt.MaDoiTuong like '%'+b.Name+'%'
    		or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
    				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong)) or b.LoaiDoiTuong = 3 or @ID_NhomDoiTuong = ''
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy	
    			ORDER BY MAX(b.MaKhachHang) DESC
    		END
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
    	   where hdx.ChoThanhToan = 0
		   and hdx.LoaiHoaDon !=2 --- khong lay HD baohanh
		   AND exists (
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

            Sql(@"ALTER PROCEDURE [dbo].[BCThuChi_TheoLoaiTien]
    @IDChiNhanhs [nvarchar](max) = '',
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiDoiTuongs [nvarchar](max) = '1',
    @KhoanMucThuChis [nvarchar](max) = ''
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	if isnull(@IDChiNhanhs,'') !=''
    		insert into @tblChiNhanh
    		select Name from dbo.splitstring(@IDChiNhanhs);
    
    	declare @tblKhoanThuChi table (ID uniqueidentifier)
    	if isnull(@KhoanMucThuChis,'') !=''
    		insert into @tblKhoanThuChi
    		select Name from dbo.splitstring(@KhoanMucThuChis);
    	
    	;with data_cte
    	as(
    	select 
    		FORMAT(cast (tbl.NgayLapYYYYMMDD as date),'dd/MM/yyyy') as NgayLapYYYYMMDD,
    		sum(ThuTienMat) as ThuTienMat,
    		sum(ThuTienPOS) as ThuTienPOS,
    		sum(ThuChuyenKhoan) as ThuChuyenKhoan,
    		sum(TongThu) as TongThu	
    	from
    	(
    	select 		
    		format(qhd.NgayLapHoaDon,'yyyyMMdd') as NgayLapYYYYMMDD,	
    		qct.TienThu as TongThu,
    		iif(qct.HinhThucThanhToan = 1, qct.TienThu,0) as ThuTienMat,
    		iif(qct.HinhThucThanhToan = 2, qct.TienThu,0) as ThuTienPOS,
    		iif(qct.HinhThucThanhToan = 3, qct.TienThu,0) as ThuChuyenKhoan
    	
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon	
    	left join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
    	where qhd.NgayLapHoaDon between @DateFrom and @DateTo
    	and qhd.LoaiHoaDon = 11
    	and (qhd.TrangThai is null or qhd.TrangThai='1')
    	and (qhd.HachToanKinhDoanh = '1' or qhd.HachToanKinhDoanh is null)
    	and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where qhd.ID_DonVi = cn.ID))
    	and (@KhoanMucThuChis ='' or exists (select km.ID from @tblKhoanThuChi km where qct.ID_KhoanThuChi= km.ID))	
    	and qct.ID_HoaDonLienQuan is not null --- chi lay phieuthu/chi lien quan hoadon
    	) tbl		
    	group by tbl.NgayLapYYYYMMDD
    	),
    	count_cte
    	as(
    		select 
    			sum(ThuTienMat) as TongThuTienMat,
    			sum(ThuTienPOS) as TongThuTienPOS,
    			sum(ThuChuyenKhoan) as TongThuChuyenKhoan,
    			sum(TongThu) as TongThuAll
    					
    		from data_cte dt
    	)
    	select *
    	from data_cte dt
    	cross join count_cte
END");

            Sql(@"ALTER PROCEDURE [dbo].[CheckThucThu_TongSuDung]
    @ID_DoiTuong [uniqueidentifier],
    @ID_TheGiaTri [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tongthu float= 0, @tongtiendieuchinh float = 0
    	declare @tongkhuyenmai float= 0
    	declare @tongsudung float= 0
    
    	declare @dateHD datetime = (select NgayLapHoaDon from  BH_HoaDon where ID = @ID_TheGiaTri)


		--- sum sotien dieuchinh den thoidiem hientai
		set @tongtiendieuchinh = (select top 1 TongTienThue as TongThu1
		from BH_HoaDon
		where LoaiHoaDon= 23 and ChoThanhToan=0
		and ID_DoiTuong= @ID_DoiTuong
		and NgayLapHoaDon < @dateHD
		order by NgayLapHoaDon desc)
    
    	-- get tongthu den thoi diem hientai
    	select 
    		@tongthu = sum(qct.TienThu)
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    	join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
    	where qct.ID_DoiTuong= @ID_DoiTuong
    	and hd.ChoThanhToan is not null
    	and hd.LoaiHoaDon= 22
    	and qhd.TrangThai='1'
    	and qhd.NgayLapHoaDon < @dateHD -- chi so sanh den phut
    	group by hd.ID_DoiTuong

		-- get gtrikhuyenmai (vi giatri dc sử dụng của thẻ = gtri khuyến mại + phải thanh toán)
		select 
    		@tongkhuyenmai = sum(hd.TongChietKhau) + sum (TongGiamGia)
    	from BH_HoaDon hd 
    	where hd.ID_DoiTuong= @ID_DoiTuong
    	and hd.ChoThanhToan is not null
    	and hd.LoaiHoaDon= 22
    	and hd.NgayLapHoaDon  < @dateHD 
    	group by hd.ID_DoiTuong
    
    	-- get all tongsudung
    	select 
    		@tongsudung= sum(qct.TienThu)
    	from Quy_HoaDon qhd
    	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
    	where qct.ID_DoiTuong= @ID_DoiTuong    	
    	and qhd.TrangThai='1'
    	and qhd.LoaiHoaDon = 11
    	and qct.HinhThucThanhToan = 4
    	
    
    	declare @return bit='1'
    	if isnull(@tongtiendieuchinh,0) +  isnull(@tongthu,0) + isnull(@tongkhuyenmai,0) < isnull(@tongsudung,0)
    		set @return='0'
    	select @return as Exist
		
END");

            Sql(@"ALTER PROCEDURE [dbo].[CreateAgainPhieuXuatKho_WhenUpdateTPDL]
    @ID_CTHoaDon [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    		;DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
    	---- ==========  INSERT AGAIN CTXUAT NEW ===========
    			
    			--- get cthd new
    		declare @ctHDNew table (ID uniqueidentifier, ID_ChiTietDinhLuong uniqueidentifier, ID_ChiTietGoiDV  uniqueidentifier, 
    			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    			SoLuong float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max),ThanhTien float,
    			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(max)				
    		)
    		insert into @ctHDNew
    		select ct.ID, ct.ID_ChiTietDinhLuong, ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    					ct.SoLuong, ct.GiaVon, ct.TonLuyKe, ct.GhiChu,ct.ThanhTien,
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
    					if @maxNgayLap is null 
							set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)
						else
							begin
									---- compare ngaylapHD - ngayxuatkho (if updateHD & change ngaylapHD)
								if FORMAT(@maxNgayLap,'yyyy-MM-dd')!= FORMAT(@NgayLapHoaDon,'yyyy-MM-dd')
										set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)		
							end

    
    						---- INSERT HD XUATKHO ----
    					declare @ID_XuatKho uniqueidentifier = newID()	, @ngayXuatKho datetime= @maxNgayLap ,@maXuatKho nvarchar(max)		

						---- find all PhieuXuatKho by ID_hoadongoc & by dichvu:  used to get mahoadon 
						declare @tblPhieuXuat table (ID uniqueidentifier, MaHoaDon nvarchar(max), NgayTao datetime)
						insert into @tblPhieuXuat
						select distinct hd.ID, hd.MaHoaDon, hd.NgayTao	
						from BH_HoaDon hd 
						join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
						where hd.LoaiHoaDon= 35 and hd.ID_HoaDon = @ID_HoaDonMua
						and ct.ID_ChiTietDinhLuong = (select top 1 ID_DonViQuiDoi from @ctHDNew where ID= @ID_CTHoaDon)
    
    					---- find all PhieuXuatKho by ID_hoadongoc
    					declare @countPhieuXK int = (select count(id) from @tblPhieuXuat)
    					declare @maXuatKhoGoc nvarchar(max) = (select top 1 MaHoaDon from @tblPhieuXuat order by NgayTao)
    					
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
    						= (select top 1 CONCAT(N', Dịch vụ: ', TenHangHoa, '(', MaHangHoa,  N'), Thành tiền: ', FORMAT(ThanhTien, 'N0') ) from @ctHDNew where ID= @ID_CTHoaDon)
    
    					
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
    						
    					
    				END

					;Enable TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[CreatePhieuXuat_FromHoaDon]
    @ID_HoaDon [uniqueidentifier],
    @LoaiXuatKho [int],
    @IsXuatNgayThuoc [bit],
    @TrangThai [bit]
AS
BEGIN
    SET NOCOUNT ON;
		;DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
    
			------- get all phieuXK NVL of hoadon (bao gom xuat le + xuat NVL)  ------
			declare @tblHDXuat table (ID uniqueidentifier,ID_DonVi uniqueidentifier,MaHoaDon nvarchar(50), 
			LoaiHoaDon int, NgayLapHoaDon datetime, NgayTao datetime, ChoThanhToan bit)
			insert into @tblHDXuat
			select ID, ID_DonVi, MaHoaDon, LoaiHoaDon, NgayLapHoaDon, NgayTao, ChoThanhToan
			from dbo.BH_HoaDon where ID_HoaDon= @ID_HoaDon 
    
  
    
    		---- get cthd new 
    		---- TienChietKhau: soluong xuat/1 ngaythuoc, SoLuong: tong sl xuat
    		declare @ctHDNew table (ID uniqueidentifier,
    			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    			SoLuong float, TienChietKhau float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max),
    			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(100)				
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
    		
    		
			 --- ===== HUY PHIEU XUATKHO OLD ----------
			
				update BH_HoaDon set ChoThanhToan= null where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= @LoaiXuatKho
    		    					
    
    				
    					---- ==========  INSERT AGAIN CTXUAT NEW (only insert if exist ctNew) ===========		
    				if exists (select ID from @ctHDNew)
    				begin
    					declare @YeuCau nvarchar(max)
    					if @TrangThai ='1' set @YeuCau =N'Tạm lưu'
    					else set @YeuCau = N'Hoàn thành'
    
    			
    				declare @MaHoaDon varchar(50), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
    				@NgayLapHoaDon datetime, @NguoiTao nvarchar(50), @IsChuyenPhatNhanh bit='0'
    				---- get infor hoadon
    				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
    				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao, @IsChuyenPhatNhanh = An_Hien
    				from BH_HoaDon where id= @ID_HoaDon
    									
    				
    						---- find all PhieuXuatKho by ID_hoadongoc: used tang mahoadon theo solanxuat
    					declare @countPhieuXK int = (select count(id) from @tblHDXuat where LoaiHoaDon= @LoaiXuatKho)
    					declare @maXuatKhoGoc nvarchar(50) = (select top 1 MaHoaDon from @tblHDXuat where LoaiHoaDon= @LoaiXuatKho order by NgayTao)	
    					declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,2,NgayLapHoaDon)) from dbo.BH_HoaDon where ID_HoaDon = @ID_HoaDon)

    					if @maxNgayLap is null 
							set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)
						else
							begin
								---- compare ngaylapHD - ngayxuatkho (if updateHD & change ngaylapHD)
								if FORMAT(@maxNgayLap,'yyyy-MM-dd')!= FORMAT(@NgayLapHoaDon,'yyyy-MM-dd')
										set @maxNgayLap = DATEADD(MILLISECOND,2,@NgayLapHoaDon)		
							end
    
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
    
    						
    																
    			end
    		
				------ update again TonLuyKe: only update with min(NgayLapHoaDon) ---
					--BEGIN TRY  
					--	declare @Min_IDPhieuXuat uniqueidentifier, @Min_NgayLapPhieuXuat datetime

					--	select top 1 @Min_IDPhieuXuat = ID, @NgayLapHoaDon = NgayLapHoaDon 					
					--	from BH_HoaDon where ID_HoaDon= @ID_HoaDon 
					--	and LoaiHoaDon= @LoaiXuatKho 
					--	and ChoThanhToan is null
					--	order by NgayLapHoaDon

					--	if @Min_IDPhieuXuat is not null
					--	 exec dbo.UpdateTonLuyKeCTHD_whenUpdate @Min_IDPhieuXuat, @ID_DonVi, @Min_NgayLapPhieuXuat
					--END TRY  
    	--			BEGIN CATCH 
    	--				select ERROR_MESSAGE() as Err
    	--			END CATCH  
    		;ENABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong  ON dbo.BH_HoaDon	 
    		
END");

            Sql(@"ALTER PROCEDURE [dbo].[CreateXuatKho_NguyenVatLieu]
    @ID_HoaDon [uniqueidentifier],
    @TrangThai [bit] = 0
AS
BEGIN
    SET NOCOUNT ON;	
	;DISABLE TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon

		declare @ctHDNew table (ID uniqueidentifier, ID_ChiTietDinhLuong uniqueidentifier, ID_ChiTietGoiDV  uniqueidentifier, 
    			ID_DonViQuiDoi uniqueidentifier, ID_LoHang uniqueidentifier,
    			SoLuong float, GiaVon float, TonLuyKe float, GhiChu nvarchar(max), ThanhTien float,
    			LaHangHoa bit,TenHangHoa nvarchar(max), MaHangHoa nvarchar(100)	, ID_DichVu uniqueidentifier		
    		)
    		
    		insert into @ctHDNew
    		select ct.ID, ct.ID_ChiTietDinhLuong, ct.ID_ChiTietGoiDV, ct.ID_DonViQuiDoi, ct.ID_LoHang,
    					ct.SoLuong, ct.GiaVon, ct.TonLuyKe, ct.GhiChu, ct.ThanhTien,
    					hh.LaHangHoa,
    					hh.TenHangHoa,
    					qd.MaHangHoa,
						ct.ID
    		from BH_HoaDon_ChiTiet ct 
    		join DonViQuiDoi qd on ct.ID_DonViQuiDoi = qd.ID
    		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    		where ct.ID_HoaDon= @ID_HoaDon
    		and ct.ID_ChiTietDinhLuong is not null 

			update tp set tp.ID_DichVu = dv.ID_DonViQuiDoi ---- used to get maphieuXK by dichvu
			from @ctHDNew tp
			join @ctHDNew dv on tp.ID_ChiTietDinhLuong = dv.ID


				update BH_HoaDon set ChoThanhToan= null where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35


					---- ==========  INSERT AGAIN CTXUAT NEW ===========	

					declare @MaHoaDon varchar(50), @ID_DonVi uniqueidentifier, @ID_NhanVien uniqueidentifier, @ID_DoiTuong uniqueidentifier,
    				@NgayLapHoaDon datetime, @NguoiTao nvarchar(50),@LoaiHoaDon int = 35 ---- xuatkho nguyenvatlieu (LoaiHoaDon = 35)
    				---- get infor hoadon
    				select @MaHoaDon= MaHoaDon, @ID_DonVi = ID_DonVi ,@ID_NhanVien = ID_NhanVien,@ID_DoiTuong = ID_DoiTuong, 
    				@NgayLapHoaDon= NgayLapHoaDon, @NguoiTao= NguoiTao
    				from BH_HoaDon where id= @ID_HoaDon
    
    				declare @count int = (select count (ID) from  @ctHDNew where LaHangHoa = 1)		

					IF @count > 0
    				BEGIN
						declare @ID_DichVu uniqueidentifier, @ID_ChiTietDinhLuong uniqueidentifier, @TongGiaTriXuat float
      			     
    					declare _cur cursor
    					for
    					select ct.ID_DichVu, ct.ID_ChiTietDinhLuong, sum(ct.GiaVon * SoLuong)
    					from @ctHDNew ct
    					where ct.ID != ct.ID_ChiTietDinhLuong
    					group by ct.ID_ChiTietDinhLuong, ct.ID_DichVu ---- group by dichvu (1 dichvu - 1phieuxuat NVL)
    
    					open _cur
    					FETCH NEXT FROM _cur
    					INTO @ID_DichVu, @ID_ChiTietDinhLuong, @TongGiaTriXuat
    					WHILE @@FETCH_STATUS = 0
    					begin

							declare @maxNgayLap datetime = (select max(DATEADD(MILLISECOND,5,NgayLapHoaDon)) from BH_HoaDon where ID_HoaDon= @ID_HoaDon)
						
    						if @maxNgayLap is null 
								set @maxNgayLap = DATEADD(MILLISECOND,5,@NgayLapHoaDon)
							else
								begin
										---- compare ngaylapHD - ngayxuatkho (if updateHD & change ngaylapHD)
									if FORMAT(@maxNgayLap,'yyyy-MM-dd')!= FORMAT(@NgayLapHoaDon,'yyyy-MM-dd')
											set @maxNgayLap = DATEADD(MILLISECOND,5,@NgayLapHoaDon)											
								end

							---- find all PhieuXuatKho by ID_hoadongoc & by dichvu:  used tang mahoadon theo solanxuat
							declare @tblPhieuXuat table (ID uniqueidentifier, MaHoaDon nvarchar(50), NgayTao datetime)
							insert into @tblPhieuXuat
							select  hd.ID, hd.MaHoaDon, hd.NgayTao	
							from BH_HoaDon hd 
							join BH_HoaDon_ChiTiet ct on hd.ID = ct.ID_HoaDon
							where hd.ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35 and ct.ID_ChiTietDinhLuong = @ID_DichVu
							group by hd.ID, hd.MaHoaDon, hd.NgayTao

    						declare @countPhieuXK int = (select count(id) from @tblPhieuXuat)
    						declare @maXuatKhoGoc nvarchar(50) = (select top 1 MaHoaDon from @tblPhieuXuat order by NgayTao)	
    				
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
						delete from @tblPhieuXuat
    				
    					FETCH NEXT FROM _cur
    					INTO @ID_DichVu, @ID_ChiTietDinhLuong, @TongGiaTriXuat						
    					end
    					CLOSE _cur;
    					DEALLOCATE _cur;		
    				END


					-------- update again TonLuyKe: only update HD da huy +  min(NgayLapHoaDon)  ---
					--BEGIN TRY  
					--	declare @Min_IDPhieuXuat uniqueidentifier, @Min_NgayLapPhieuXuat datetime

					--	select top 1 @Min_IDPhieuXuat = ID, @NgayLapHoaDon = NgayLapHoaDon 					
					--	from BH_HoaDon where ID_HoaDon= @ID_HoaDon and LoaiHoaDon= 35
					--	and ChoThanhToan is null
					--	order by NgayLapHoaDon

					--	if @Min_IDPhieuXuat is not null
					--		exec dbo.UpdateTonLuyKeCTHD_whenUpdate @Min_IDPhieuXuat, @ID_DonVi, @Min_NgayLapPhieuXuat
					--END TRY  
    	--			BEGIN CATCH 
    	--				select ERROR_MESSAGE() as Err
    	--			END CATCH  
		;Enable TRIGGER dbo.UpdateNgayGiaoDichGanNhat_DMDoiTuong ON dbo.BH_HoaDon
		
END");

            Sql(@"ALTER PROCEDURE [dbo].[DanhMucKhachHang_CongNo_Paging]
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
    set nocount on
    	
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
    	Select @count =  (Select count(*) from @tblSearchString);')
    
    	declare @sql2 nvarchar(max)= concat(' WITH Data_CTE ',
    									' AS ',
    									' ( ',
    
    N'SELECT  * 
    		FROM
    		(  
    			SELECT 
    		  dt.ID as ID,
    		  dt.MaDoiTuong, 
    			  case when dt.IDNhomDoiTuongs='''' then ''00000000-0000-0000-0000-000000000000'' 
				  else  ISNULL(dt.IDNhomDoiTuongs,''00000000-0000-0000-0000-000000000000'') end as ID_NhomDoiTuong,
    	      dt.TenDoiTuong,
    		  dt.TenDoiTuong_KhongDau,
    		  dt.TenDoiTuong_ChuCaiDau,
    			  dt.ID_TrangThai,
				   dt.TheoDoi,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
			   dt.NgayGiaoDichGanNhat,
    			  ISNULL(dt.DienThoai,'''') as DienThoai,
    			  ISNULL(dt.Email,'''') as Email,
    			  ISNULL(dt.DiaChi,'''') as DiaChi,
    			  ISNULL(dt.MaSoThue,'''') as MaSoThue,
				  dt.TaiKhoanNganHang,
    		  ISNULL(dt.GhiChu,'''') as GhiChu,
    		  dt.NgayTao,
    		  dt.DinhDang_NgaySinh,
    		  ISNULL(dt.NguoiTao,'''') as NguoiTao,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    			  dt.ID_DonVi, --- Collate Vietnamese_CI_AS
    		  dt.LaCaNhan,
    		  CAST(ISNULL(dt.TongTichDiem,0) as float) as TongTichDiem,
    			 case when right(rtrim(dt.TenNhomDoiTuongs),1) ='','' then LEFT(Rtrim(dt.TenNhomDoiTuongs),
				  len(dt.TenNhomDoiTuongs)-1) else				  
				  ISNULL(dt.TenNhomDoiTuongs, N''Nhóm mặc định'') end   as TenNhomDT,-- remove last coma
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
    			ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
			  a.PhiDichVu,
    			  CAST(0 as float) as TongNapThe , 
    			  CAST(0 as float) as SuDungThe , 
    			  CAST(0 as float) as HoanTraTheGiaTri , 
    			  CAST(0 as float) as SoDuTheGiaTri , 
				  isnull(a.NapCoc,0) as NapCoc,
				  isnull(a.SuDungCoc,0) as SuDungCoc,
				  isnull(a.SoDuCoc,0) as SoDuCoc,
				 ---- isnull(a.NapCoc,0) - isnull(a.SuDungCoc,0) as SoDuCoc,
    			  concat(dt.MaDoiTuong,'' '',lower(dt.MaDoiTuong) ,'' '', dt.TenDoiTuong,'' '', dt.DienThoai,'' '', dt.TenDoiTuong_KhongDau)  as Name_Phone			
    		  FROM DM_DoiTuong dt  ','')
    	
    	declare @sql3 nvarchar(max)= concat('
    				LEFT JOIN (
    					SELECT tblThuChi.ID_DoiTuong,
    						SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.HoanTraThe,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) + sum(ISNULL(tblThuChi.ThuTuThe,0))
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
						sum(isnull(tblThuChi.NapCoc,0)) -sum(isnull(tblThuChi.SuDungCoc,0))  as SoDuCoc
    				FROM
    				(
    					select 
							cp.ID_NhaCungCap as ID_DoiTuong,
							0 as GiaTriTra,
    						0 as DoanhThu,
							0 AS TienThu,
    						0 AS TienChi, 
    						0 AS SoLanMuaHang,
							0 as ThuTuThe,
							sum(cp.ThanhTien) as PhiDichVu,
							0 as HoanTraThe, --- hoantra lai sodu co trong TGT cho khach
							0 as NapCoc,
							0 as SuDungCoc
						from BH_HoaDon_ChiPhi cp
						join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
						where hd.ChoThanhToan = 0
						and exists (select * from @tblChiNhanh cn where hd.ID_DonVi= cn.ID)
						group by cp.ID_NhaCungCap

						union all

						---- hoantra sodu TGT cho khach (giam sodu TGT)
						SELECT 
    							bhd.ID_DoiTuong,    	
								0 as GiaTriTra,
    							0 as DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								-sum(bhd.PhaiThanhToan) as HoanTraThe,
								0 as NapCoc,
								0 as SuDungCoc
    					FROM BH_HoaDon bhd
						where bhd.LoaiHoaDon = 32 and bhd.ChoThanhToan = 0 
						and exists (select * from @tblChiNhanh cn where bhd.ID_DonVi= cn.ID)
						group by bhd.ID_DoiTuong

						union all
    						---- tongban
    						SELECT 
    							bhd.ID_DoiTuong,    	
								0 as GiaTriTra,
    							bhd.PhaiThanhToan as DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe,
								0 as NapCoc,
								0 as SuDungCoc
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,25) AND bhd.ChoThanhToan = ''0'' 
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 

							
							union all
							---- doanhthu tuthe
							select 
								bhd.ID_DoiTuong,
								0 as GiaTriTra,
    							0 AS DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								iif(bhd.LoaiHoaDon =22, bhd.PhaiThanhToan, - bhd.PhaiThanhToan) as ThuTuThe, ----- napTGT + tat toan congno TGT
								0 as PhiDichVu,
								0 as HoanTraThe,
								0 as NapCoc,
								0 as SuDungCoc
							from BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (22,42)
							AND bhd.ChoThanhToan = ''0'' 
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 						

						
    							 union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,
								bhd.PhaiThanhToan AS GiaTriTra,    							
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe,
								0 as NapCoc,
								0 as SuDungCoc
    						FROM BH_HoaDon bhd   						
    						WHERE bhd.LoaiHoaDon in (6,4, 13,14) AND bhd.ChoThanhToan = ''0''  
    							AND bhd.NgayLapHoaDon between ''', @timeStart ,''' AND ''',@timeEnd,		
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
							   							
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
								0 as PhiDichVu,
								0 as HoanTraThe,
								0 as NapCoc,
								0 as SuDungCoc
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon   						
    						WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.HinhThucThanhToan!= 6
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
							and not exists (select id from BH_HoaDon pthh where qhdct.ID_HoaDonLienQuan = pthh.ID and pthh.LoaiHoaDon= 41) --- khong lay phieuchi hoahong nguoi GT
    						AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID) 
    							AND qhd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd  ,

									---- NapCoc ----
								''' union all
									
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe,
								iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu) as NapCocNCC,
								0 as SuDungCoc
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon 							
    						WHERE (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.LoaiThanhToan = 1
							AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)',
    							
								---- sudungcoc ----
								' union all
									
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe,
								0 as NapCoc,
								iif(qhd.LoaiHoaDon=12,qhdct.TienThu,-qhdct.TienThu) as SuDungCoc
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon 							
    						WHERE (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.HinhThucThanhToan = 6
							AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID) ',
    							
    							' union all
    
    							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe,
								0 as NapCoc,
								0 as SuDungCoc
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon 							
    						WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
							and qhdct.HinhThucThanhToan!= 6
							and not exists (select id from BH_HoaDon pthh where qhdct.ID_HoaDonLienQuan = pthh.ID and pthh.LoaiHoaDon= 41) --- khong lay phieuchi hoahong nguoi GT
							and (qhdct.LoaiThanhToan is null or qhdct.LoaiThanhToan != 3)
    							AND qhd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd  ,
    						''' AND exists (select * from @tblChiNhanh cn where qhd.ID_DonVi= cn.ID)' )
    
    declare @sql4 nvarchar(max)= concat(' Union All
    							-- solan mua hang
    						Select 
    							hd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    								0 as TienChi,
    							COUNT(*) AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as PhiDichVu,
								0 as HoanTraThe,
								0 as NapCoc,
								0 as SuDungCoc
    						From BH_HoaDon hd 
    						where hd.LoaiHoaDon in (1,19,25)
    						and hd.ChoThanhToan = 0
    						AND hd.NgayLapHoaDon between ''',@timeStart,''' AND ''',@timeEnd ,
    						''' AND exists (select * from @tblChiNhanh cn where ID_DonVi= cn.ID) 
    							 GROUP BY hd.ID_DoiTuong  	
    							)AS tblThuChi
    						GROUP BY tblThuChi.ID_DoiTuong
    				) a on dt.ID = a.ID_DoiTuong  					
    						WHERE  dt.loaidoituong =', @loaiKH  ,					
    						' and dt.NgayTao between ''',@timeStartKH, ''' and ''',@timeEndKH,
    							''' and ( MaDoiTuong like ''%', @MaKH, '%'' OR  TenDoiTuong like ''%',@MaKH, '%'' or TenDoiTuong_KhongDau like ''%',@MaKH, '%'' or DienThoai like ''%',@MaKH, '%'' or Email like ''%',@MaKH, '%'' )',
    						
    						  '', @Where, ')b
    				 where b.ID not like ''%00000000-0000-0000-0000-0000%''
    				 and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl inner JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    				  ),
    			Count_CTE ',
    			' AS ',
    			' ( ',
    			' SELECT COUNT(*) AS TotalRow, CEILING(COUNT(*) / CAST(',@PageSize, ' as float )) as TotalPage ,
					SUM(TongBan) as TongBanAll,
					SUM(TongBanTruTraHang) as TongBanTruTraHangAll,
					SUM(TongTichDiem) as TongTichDiemAll,
					SUM(NoHienTai) as NoHienTaiAll,
					SUM(PhiDichVu) as TongPhiDichVu,
					SUM(NapCoc) as NapCocAll,
					SUM(SuDungCoc) as SuDungCocAll,
					SUM(SoDuCoc) as SoDuCocAll
					FROM Data_CTE ',
    			' ) ',
    			' SELECT dt.*, cte.TotalPage, cte.TotalRow, 
					cte.TongBanAll, 
					cte.TongBanTruTraHangAll,
					cte.TongTichDiemAll,
					cte.NoHienTaiAll,
					cte.TongPhiDichVu,
    				  ISNULL(trangthai.TenTrangThai,'''') as TrangThaiKhachHang,
    				  ISNULL(qh.TenQuanHuyen,'''') as PhuongXa,
    				  ISNULL(tt.TenTinhThanh,'''') as KhuVuc,
    				  ISNULL(dv.TenDonVi,'''') as ConTy,
    				  ISNULL(dv.SoDienThoai,'''') as DienThoaiChiNhanh,
    				  ISNULL(dv.DiaChi,'''') as DiaChiChiNhanh,
    				  ISNULL(nk.TenNguonKhach,'''') as TenNguonKhach,
    				  ISNULL(dt2.TenDoiTuong,'''') as NguoiGioiThieu,
					  ISNULL(nvpt.MaNhanVien,'''') as MaNVPhuTrach,
					 ISNULL(nvpt.TenNhanVien,'''') as TenNhanVienPhuTrach',
    			' FROM Data_CTE dt',
    			' CROSS JOIN Count_CTE cte',
    			' LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID ',
    		' LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID ',
    			' LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID ',
    			' LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID ',
				' LEFT join NS_NhanVien nvpt on dt.ID_NhanVienPhuTrach = nvpt.ID ',
    		' LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID ',
    			' LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID ',
    			' ORDER BY ',@SortBy,
    			' OFFSET (', @CurrentPage, ' * ', @PageSize ,') ROWS ',
    			' FETCH NEXT ', @PageSize , ' ROWS ONLY ')
    
    			


    			exec (@sql1 + @sql2 + @sql3 + @sql4)
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetBangCongNhanVien]
    @IDChiNhanhs [nvarchar](max),
    @ID_NhanVienLogin [uniqueidentifier],
    @IDPhongBans [nvarchar](max),
    @IDCaLamViecs [nvarchar](max),
    @TextSearch [nvarchar](max),
    @FromDate [nvarchar](10),
    @ToDate [nvarchar](10),
    @CurrentPage [int],
    @PageSize [int],
	@TrangThaiNV varchar(10)
AS
BEGIN
    SET NOCOUNT ON;
	

    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);
    
    	declare @tblNhanVien table (ID uniqueidentifier)
    	insert into @tblNhanVien
    	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDChiNhanhs,'BangCong_XemDS_PhongBan','BangCong_XemDS_HeThong');
    
	declare @tblTrangThaiNV table(TrangThaiNV int)
    	insert into @tblTrangThaiNV
    	select name from dbo.splitstring(@TrangThaiNV)

    	declare @tblPhong table(ID uniqueidentifier)
    	if @IDPhongBans=''	
    		insert into @tblPhong
    		select ID from NS_PhongBan
    	else
    		insert into @tblPhong
    		select name from dbo.splitstring(@IDPhongBans)
    
    	declare @tblca table(ID_CaLamViec uniqueidentifier)
    	if @IDCaLamViecs ='%%'
    		insert into @tblca
    		select ID from NS_CaLamViec
    	else
    		insert into @tblca
    		select Name from dbo.splitstring(@IDCaLamViecs);
    
    		with data_cte
    		as(
    
    		select nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien,
			iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) as TrangThaiNV,
    			cast(congnv.CongNgayThuong as float) as CongChinh, cast(congnv.CongNgayNghiLe as float) as CongLamThem,
    			cast(congnv.OTNgayThuong as float) as OTNgayThuong, 
    			congnv.OTNgayNghiLe as OTNgayNghiLe,
    			cast(congnv.OTNgayThuong + congnv.OTNgayNghiLe as float) as SoGioOT,
    			cast(congnv.SoPhutDiMuon as float) as SoPhutDiMuon
    		from
    			(select cong.ID_NhanVien,
    				sum(cong.CongNgayThuong) as CongNgayThuong,
    				sum(CongNgayNghiLe) as CongNgayNghiLe,
    				sum(OTNgayThuong) as OTNgayThuong,
    				sum(OTNgayNghiLe) as OTNgayNghiLe,
    				sum(SoPhutDiMuon) as SoPhutDiMuon
    			from
    				(select bs.ID_ChamCongChiTiet, bs.ID_CaLamViec, ca.TongGioCong as TongGioCong1Ca, ca.TenCa, bs.ID_NhanVien,
    					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon,
    					IIF(bs.LoaiNgay=0, bs.Cong,0) as CongNgayThuong,
    					IIF(bs.LoaiNgay!=0, bs.Cong,0) as CongNgayNghiLe,
    					IIF(bs.LoaiNgay=0, bs.SoGioOT,0) as OTNgayThuong,
    					IIF(bs.LoaiNgay!=0, bs.SoGioOT,0) as OTNgayNghiLe
    				from NS_CongBoSung bs
    				join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
    				where NgayCham >= @FromDate and NgayCham <= @ToDate
    				and bs.TrangThai !=0
    				and exists(select ID from @tblNhanVien nv where bs.ID_NhanVien= nv.ID)
    				and (@IDCaLamViecs ='%%' or exists(select ID_CaLamViec from @tblca ca where bs.ID_CaLamViec= ca.ID_CaLamViec))
    				and exists(select Name from dbo.splitstring(@IDChiNhanhs) dv where bs.ID_DonVi= dv.Name)
    				) cong
    			group by cong.ID_NhanVien
    			) congnv
    			join NS_NhanVien nv on congnv.ID_NhanVien= nv.ID 
    		
    			WHERE ((select count(Name) from @tblSearchString b 
    				where nv.TenNhanVien like '%'+b.Name+'%'  						
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'
    				)=@count or @count=0)	
				and exists (select TrangThaiNV from @tblTrangThaiNV tt
							where iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) = tt.TrangThaiNV)
				and (@IDPhongBans ='' or 
				----- NV từng chấm công ở CN1, nay chuyển công tác sang CN2
					exists (select pb.ID from @tblPhong pb 
					join NS_QuaTrinhCongTac ct on pb.ID =  ct.ID_PhongBan
					where exists(select Name from dbo.splitstring(@IDChiNhanhs) dv where ct.ID_DonVi= dv.Name)))
    		),
    		count_cte
    		as
    		( SELECT COUNT(*) AS TotalRow, 
    			CEILING(COUNT(*) / CAST(@PageSize as float )) as TotalPage ,
    			cast(sum(CongChinh) as float) as TongCong,
    			cast(sum(CongLamThem) as float)as TongCongNgayNghi,
    			cast(sum(SoGioOT) as float) as TongOT,
    			cast(sum(SoPhutDiMuon) as float) as TongDiMuon
    
    		FROM Data_CTE
    		)
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.MaNhanVien
    		OFFSET (@CurrentPage * @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetBaoCaoCongNoChiTiet]
    @IDChiNhanhs [nvarchar](max) = '',
    @DateFrom [datetime],
    @DateTo [datetime],
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
    	select 
    		tbl.ID,
    		tbl.MaHoaDon,
    		tbl.LoaiHoaDon,
    		tbl.NgayLapHoaDon,		
    		tbl.DienGiai,
    		tbl.MaDoiTuong,
    		tbl.TenDoiTuong,
    		tbl.TenDonVi,
    	--	tbl.TenNhanViens,
			tbl.NVPhuTrach,
    		tbl.KhachDaTra,
    		tbl.TongThanhToan- tbl.TongTra as TongThanhToan,
    		tbl.TongThanhToan - tbl.TongTra - tbl.KhachDaTra as ConNo1,
    		iif(tbl.LoaiHoaDon!=19, tbl.TongThanhToan - tbl.TongTra - tbl.KhachDaTra, 
    		iif(tbl.KhachDaTra - tbl.GiaTriSD > 0,0, tbl.GiaTriSD - tbl.KhachDaTra)) as NoThucTe1
    	from
    	(
    	select  
    		hd.ID,
    		hd.MaHoaDon,
    		hd.LoaiHoaDon,
    		hd.NgayLapHoaDon,
    		hd.TongThanhToan,
    		hd.DienGiai,
    		dt.MaDoiTuong,
    		dt.TenDoiTuong,
    		dv.TenDonVi,
			nvpt.TenNhanVien as NVPhuTrach,    	
    		isnull(soquy.KhachDaTra,0) as KhachDaTra,
    		isnull(sdGDV.GiaTriSD,0) as GiaTriSD,
    		isnull(traGDV.TongTra,0) as TongTra,
    		iif(hd.TongThanhToan - isnull(soquy.KhachDaTra,0) > 0,1,0) as TrangThaiCongNo
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
    		left join(
    		------ tra gdv----
    		select 
    			hdt.ID_HoaDon,
    			sum(hdt.PhaiThanhToan) as TongTra
    		from BH_HoaDon hdt
    		where hdt.LoaiHoaDon = 6 and hdt.ChoThanhToan ='0'	
    		group by hdt.ID_HoaDon
    	) traGDV on hd.ID = traGDV.ID_HoaDon
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
    	) tbl where (@TrangThais ='' or tbl.TrangThaiCongNo in (select name from dbo.splitstring(@TrangThais)))
    					
    	),
    	count_cte as
    	 (
    		select count(ID) as TotalRow,
    			CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
    			sum(TongThanhToan) as TongThanhToanAll,
    			sum(KhachDaTra) as KhachDaTraAll,
    			sum(ConNo1) as ConNoAll,
    			sum(NoThucTe1) as NoThucTeAll
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
		iif(hd.LoaiHoaDon=22, hd.ConNo1 - ISNULL(qtCN.GiaTriTatToan,0),hd.ConNo1) as ConNo,
		iif(hd.LoaiHoaDon=22, hd.NoThucTe1 - ISNULL(qtCN.GiaTriTatToan,0),hd.NoThucTe1) as NoThucTe,
		tblNV.TenNhanViens
	from tView hd
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

            Sql(@"ALTER PROCEDURE [dbo].[GetChiTietHoaHongGioiThieu_byID]
    @ID [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
    	select 
    		hd.MaHoaDon,
    		hd.NgayLapHoaDon,
    		hd.TongThanhToan,
    		hd.ID_CheckIn, --- id nguoi GT
    		dt.MaDoiTuong,
    		dt.TenDoiTuong,
    		dt.DienThoai,
    		hd.ID_DoiTuong,
    		ct.ID_ParentCombo as ID_HoaDon_DuocCK,	
    		ct.PTChietKhau,
    		ct.TienChietKhau,	
			ct.PTChiPhi as DaTrich, ---- số tiền đã trích trước đó
			ct.TienChiPhi as ConLai, ---- Số tiền còn lại được trích		
    		ct.TienThue as SoTienTrich, ----- Số tiền thực tế tính chiết khấu cho khách (TienChietKhau * 100/ PTChietKhau)
    		cast(ct.PTThue as int) as TrangThai
    	from BH_HoaDon_ChiTiet ct
    join BH_HoaDon hd on ct.ID_ParentCombo = hd.ID
    join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    where ct.ID_HoaDon= @ID
    	 order by dt.MaDoiTuong, hd.NgayLapHoaDon desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetDuLieuChamCong]
    @IDChiNhanhs [nvarchar](max),
    @IDPhongBans [nvarchar](max),
    @IDCaLamViecs [nvarchar](max),
    @TextSearch [nvarchar](max),
    @FromDate [nvarchar](10),
    @ToDate [nvarchar](10),
    @CurrentPage [int],
    @PageSize [int],
	@TrangThaiNV varchar(10)
AS
BEGIN
    SET NOCOUNT ON;
    	declare @dtNow datetime = getdate()
    	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);
    
    	declare @tblDonVi table(ID uniqueidentifier)
    	insert into @tblDonVi
    	select name from dbo.splitstring(@IDChiNhanhs)
		
    	declare @tblTrangThaiNV table(TrangThaiNV int)
    	insert into @tblTrangThaiNV
    	select name from dbo.splitstring(@TrangThaiNV)
    
    	declare @tblPhong table(ID uniqueidentifier)
    	if @IDPhongBans=''	
    		insert into @tblPhong
    		select ID from NS_PhongBan
    	else
    		insert into @tblPhong
    		select name from dbo.splitstring(@IDPhongBans)
    
    	declare @tblCaLamViec table(ID uniqueidentifier)
    	if @IDCaLamViecs='%%' OR  @IDCaLamViecs=''
			begin
				set  @IDCaLamViecs =''
    			insert into @tblCaLamViec
    			select ca.ID from NS_CaLamViec ca
    			join NS_CaLamViec_DonVi dvca on ca.id= dvca.ID_CaLamViec
    			where exists (select ID from @tblDonVi dv where dv.ID= dvca.ID_DonVi)
			end
    	else
    		insert into @tblCaLamViec
    		select name from dbo.splitstring(@IDCaLamViecs);
    
    with data_cte
    	as (
    		select 
    				nv.MaNhanVien,
    				nv.TenNhanVien,
					iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) as TrangThaiNV, -- danghiviec ~ daxoa
    				ca.MaCa,
    				ca.TenCa,
    				format(ca.GioVao,'HH:mm') as GioVao,
    				format(ca.GioRa,'HH:mm') as GioRa,
    				tblView.TuNgay,
    				tblView.DenNgay,
    				tblView.ID_PhieuPhanCa,
    				tblView.ID_NhanVien,
    				tblView.ID_CaLamViec,					
					cast(tblView.TongCongNV as float) as TongCongNV,
					tblView.SoPhutDiMuon,
					tblView.SoPhutOT,					
    				tblView.Thang,
    				tblView.Nam,
    				tblView.Ngay1, Ngay2, Ngay3, ngay4, ngay5, Ngay6, Ngay7, Ngay8, Ngay9, 
    				Ngay10, Ngay11, Ngay12,ngay13, ngay14, Ngay15,ngay16, ngay17, Ngay18,Ngay19,
    				Ngay20, Ngay21, Ngay22,ngay23, ngay24, Ngay25,ngay26, ngay27, Ngay28,Ngay29,
    				Ngay30, Ngay31,

    				case when Format1 >= TuNgay and Format1 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format1)) end else '1' end as DisNgay1,
					case when Format2 >= TuNgay and Format2 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format2)) end else '1' end as DisNgay2,
					case when Format3 >= TuNgay and Format3 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format3)) end else '1' end as DisNgay3,
					case when Format4 >= TuNgay and Format4 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format4)) end else '1' end as DisNgay4,
					case when Format5 >= TuNgay and Format5 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format5)) end else '1' end as DisNgay5,
					case when Format6 >= TuNgay and Format6 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format6)) end else '1' end as DisNgay6,
					case when Format7 >= TuNgay and Format7 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format7)) end else '1' end as DisNgay7,
					case when Format8 >= TuNgay and Format8 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format8)) end else '1' end as DisNgay8,
					case when Format9 >= TuNgay and Format9 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format9)) end else '1' end as DisNgay9,

					case when Format10 >= TuNgay and Format10 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format10)) end else '1' end as DisNgay10,
					case when Format11 >= TuNgay and Format11 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format11)) end else '1' end as DisNgay11,
					case when Format12 >= TuNgay and Format12 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format12)) end else '1' end as DisNgay12,
					case when Format13 >= TuNgay and Format13 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format13)) end else '1' end as DisNgay13,
					case when Format14 >= TuNgay and Format14 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format14)) end else '1' end as DisNgay14,
					case when Format15 >= TuNgay and Format15 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format15)) end else '1' end as DisNgay15,
					case when Format16 >= TuNgay and Format16 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format16)) end else '1' end as DisNgay16,
					case when Format17 >= TuNgay and Format17 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format17)) end else '1' end as DisNgay17,
					case when Format18 >= TuNgay and Format18 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format18)) end else '1' end as DisNgay18,
					case when Format19 >= TuNgay and Format19 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format19)) end else '1' end as DisNgay19,

					case when Format20 >= TuNgay and Format20 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format20)) end else '1' end as DisNgay20,
					case when Format21 >= TuNgay and Format21 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format21)) end else '1' end as DisNgay21,
					case when Format22 >= TuNgay and Format22 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format22)) end else '1' end as DisNgay22,
					case when Format23 >= TuNgay and Format23 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format23)) end else '1' end as DisNgay23,
					case when Format24 >= TuNgay and Format24 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format24)) end else '1' end as DisNgay24,
					case when Format25 >= TuNgay and Format25 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format25)) end else '1' end as DisNgay25,
					case when Format26 >= TuNgay and Format26 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format26)) end else '1' end as DisNgay26,
					case when Format27 >= TuNgay and Format27 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format27)) end else '1' end as DisNgay27,
					case when Format28 >= TuNgay and Format28 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format28)) end else '1' end as DisNgay28,
					case when Format29 >= TuNgay and Format29 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format29)) end else '1' end as DisNgay29,

					case when Format30 >= TuNgay and Format30 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format30)) end else '1' end as DisNgay30,
					case when Format31 >= TuNgay and Format31 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
						(select dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec,Format31)) end else '1' end as DisNgay31
    			from
    				( 
    			select tblRow.*, phieu.LoaiPhanCa,
    				format(phieu.TuNgay,'yyyy-MM-dd') as TuNgay, 
    				format(ISNULL(phieu.DenNgay,dateadd(month,1,getdate())),'yyyy-MM-dd') as DenNgay,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,1) as Format1,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,2) as Format2,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,3) as Format3,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,4) as Format4,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,5) as Format5,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,6) as Format6,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,7) as Format7,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,8) as Format8,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,9) as Format9,
    
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,10) as Format10,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,11) as Format11,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,12) as Format12,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,13) as Format13,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,14) as Format14,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,15) as Format15,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,16) as Format16,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,17) as Format17,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,18) as Format18,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,19) as Format19,
    
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,20) as Format20,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,21) as Format21,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,22) as Format22,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,23) as Format23,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,24) as Format24,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,25) as Format25,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,26) as Format26,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,27) as Format27,
    				DATEFROMPARTS (tblRow.Nam,tblRow.Thang,28) as Format28,
    				--- avoid error Ngay 29-02, 30-02
    				DATEADD(MONTH, (tblRow.Nam - 1900) * 12 + tblRow.Thang - 1 , 28) as Format29, 
    				DATEADD(MONTH, (tblRow.Nam - 1900) * 12 + tblRow.Thang - 1 , 29) as Format30, 
    				DATEADD(MONTH, (tblRow.Nam - 1900) * 12 + tblRow.Thang - 1 , 30) as Format31
    			from
    			(
    
    				select tblunion.ID as ID_PhieuPhanCa, tblunion.ID_NhanVien, tblunion.Nam, tblunion.Thang, tblunion.ID_CaLamViec,
						max(tblunion.TongCongNV) as TongCongNV,
						max(tblunion.SoPhutDiMuon) as SoPhutDiMuon,
						max(tblunion.SoPhutOT) as SoPhutOT,

    					max(Ngay1) as Ngay1,max(Ngay2) as Ngay2,max(Ngay3) as Ngay3,max(Ngay4) as Ngay4,max(Ngay5) as Ngay5,
    					max(Ngay6) as Ngay6,max(Ngay7) as Ngay7,max(Ngay8) as Ngay8,max(Ngay9) as Ngay9, max(Ngay10) as Ngay10,
    					max(Ngay11) as Ngay11,max(Ngay12) as Ngay12,max(Ngay13) as Ngay13,max(Ngay14) as Ngay14,max(Ngay15) as Ngay15,
    					max(Ngay16) as Ngay16,max(Ngay17) as Ngay17,max(Ngay18) as Ngay18,max(Ngay19) as Ngay19, max(Ngay20) as Ngay20,
    					max(Ngay21) as Ngay21,max(Ngay22) as Ngay22,max(Ngay23) as Ngay23,max(Ngay24) as Ngay24,max(Ngay25) as Ngay25,
    					max(Ngay26) as Ngay26,max(Ngay27) as Ngay27,max(Ngay28) as Ngay28,max(Ngay29) as Ngay29, 
    					max(Ngay30) as Ngay30, max(Ngay31) as Ngay31
    
    				from
    				(
    					select phieu.ID, phieu.Nam, phieu.Thang, nvphieu.ID_NhanVien, ca.ID as ID_CaLamViec ,
    						null as Ngay1, null as Ngay2, null as Ngay3,  null as Ngay4, null as Ngay5, null as Ngay6,null as Ngay7, null as Ngay8, null as Ngay9 , 
    						null as Ngay10, null as Ngay11, null as Ngay12,null as Ngay13, null as Ngay14, null as Ngay15,null as Ngay16, null as Ngay17, null as Ngay18,null as Ngay19,
    						null as Ngay20, null as Ngay21, null as Ngay22,null as Ngay23, null as Ngay24, null as Ngay25,null as Ngay26, null as Ngay27, null as Ngay28,null as Ngay29,
    						null as Ngay30, null as Ngay31, 0 as TongCongNV, 0 as SoPhutDiMuon, 0 as SoPhutOT
    					from NS_PhieuPhanCa_NhanVien nvphieu 				
    					join (select ID, 
								case when DenNgay is null then case when @ToDate < @dtNow then datepart(year,@ToDate) else datepart(year, @dtNow) end
									else
										case when datepart(year,DenNgay) != datepart(year, @dtNow) 
											then datepart(year, @dtNow) else iif(TuNgay < @FromDate, datepart(year, @FromDate), datepart(year,TuNgay)) end end as Nam, 
    							case when DenNgay is null then datepart(month,@FromDate)  else 
    							case when TuNgay < @FromDate then datepart(month,@FromDate) else datepart(month,TuNgay) end end as Thang
    						from  NS_PhieuPhanCa
    						where TrangThai != 0  and ((DenNgay is null and TuNgay <= @ToDate ) 
    							OR ((DenNgay is not null 
    								and ((DenNgay <= @ToDate and DenNgay >=  @FromDate )
    									or (DenNgay >= @ToDate  and TuNgay <= @ToDate)))))
    						) phieu on nvphieu.ID_PhieuPhanCa = phieu.ID						
    					join NS_PhieuPhanCa_CaLamViec caphieu on nvphieu.ID_PhieuPhanCa = caphieu.ID_PhieuPhanCa
    					join NS_CaLamViec ca on ca.ID= caphieu.ID_CaLamViec
    
    					union all
    
						select pivOut.*, congOut.TongCongNV, 
							congOut.SoPhutDiMuon,
							congOut.SoPhutOT

						from
    						(select piv.ID_PhieuPhanCa, piv.Nam,  piv.Thang, piv.ID_NhanVien, piv.ID_CaLamViec, [1] as Ngay1, [2] as Ngay2,[3] as Ngay3, [4] as Ngay4, [5] as Ngay5, [6] as Ngay6,[7] as Ngay7, [8] as Ngay8, [9] as Ngay9,
    							[10] as Ngay10, [11] as Ngay11, [12] as Ngay12,[13] as Ngay13, [14] as Ngay14, [15] as Ngay15, [16] as Ngay16,[17] as Ngay17, [18] as Ngay18, [19] as Ngay19,
    							[20] as Ngay20, [21] as Ngay21, [22] as Ngay22,[23] as Ngay23, [24] as Ngay24, [25] as Ngay25, [26] as Ngay26,[27] as Ngay27, [28] as Ngay28, [29] as Ngay29,
    							[30] as Ngay30, [31] as Ngay31
    						from
    						(
    						select phieu.ID as ID_PhieuPhanCa, bs.ID_NhanVien, bs.ID_CaLamViec, bs.ID_DonVi, DATEPART(DAY, bs.NgayCham) as Ngay,DATEPART(MONTH, bs.NgayCham) as Thang,DATEPART(YEAR, bs.NgayCham) as Nam,
    						bs.KyHieuCong	
    						from NS_CongBoSung bs
    						join NS_PhieuPhanCa_NhanVien phieunv on bs.ID_NhanVien = phieunv.ID_NhanVien
    						join NS_PhieuPhanCa_CaLamViec phieuca on phieunv.ID_PhieuPhanCa = phieuca.ID_PhieuPhanCa and  bs.ID_CaLamViec = phieuca.ID_CaLamViec
    						join NS_PhieuPhanCa phieu on phieunv.ID_PhieuPhanCa = phieu.ID
    						where phieu.TrangThai != 0
    							and ((DenNgay is null and TuNgay <= @ToDate) 
    								OR ((DenNgay is not null 
    									and ((DenNgay <= @ToDate and DenNgay >= @FromDate )
    									or (DenNgay >= @ToDate and TuNgay <= @ToDate )))))
    						and bs.NgayCham >= @FromDate and bs.NgayCham <=@ToDate
							and bs.TrangThai !=0
							and exists (select ID from @tblDonVi dv where dv.ID= bs.ID_DonVi)
    						) a
    						PIVOT (
    						  max(KyHieuCong)
    						  FOR Ngay in ( [1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19], [20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31]) 
    						) piv 
						) pivOut
						join (
							-- sumcong ofnv
							select  
								cong2.ID_NhanVien, cong2.ID_CaLamViec,
								cong2.CongNgayThuong + cong2.CongNgayNghiLe as TongCongNV,
								cong2.SoPhutDiMuon,
								SoGioOT as SoPhutOT								
								from
								(
												select cong.ID_NhanVien, cong.ID_CaLamViec, cong.ID_DonVi,
    												sum(cong.CongNgayThuong) as CongNgayThuong,
    												sum(CongNgayNghiLe) as CongNgayNghiLe,
    												sum(OTNgayThuong) as OTNgayThuong,
    												sum(OTNgayNghiLe) as OTNgayNghiLe,
    												sum(SoPhutDiMuon) as SoPhutDiMuon,
													sum(SoGioOT) as SoGioOT
    											from
    												(select bs.ID_CaLamViec,  bs.ID_NhanVien, bs.ID_DonVi,
    													bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon,
    													IIF(bs.LoaiNgay=0, bs.Cong,0) as CongNgayThuong,
    													IIF(bs.LoaiNgay!=0, bs.Cong,0) as CongNgayNghiLe,
    													IIF(bs.LoaiNgay=0, bs.SoGioOT,0) as OTNgayThuong,
    													IIF(bs.LoaiNgay!=0, bs.SoGioOT,0) as OTNgayNghiLe
    												from NS_CongBoSung bs
    												join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
    												where NgayCham >= @FromDate and NgayCham <= @ToDate
    												and bs.TrangThai !=0    		
													and  exists (select ID from @tblDonVi dv where dv.ID= bs.ID_DonVi) 
    												) cong group by cong.ID_NhanVien, cong.ID_CaLamViec, cong.ID_DonVi
											 ) cong2
						) congOut on pivout.ID_NhanVien= congOut.ID_NhanVien and pivout.ID_CaLamViec= congOut.ID_CaLamViec
    				) tblunion
    				group by  tblunion.ID,tblunion.ID_NhanVien, tblunion.Nam, tblunion.Thang, tblunion.ID_CaLamViec
    			) tblRow
    			join NS_PhieuPhanCa phieu on tblRow.ID_PhieuPhanCa = phieu.ID
    		) tblView 
    	join NS_CaLamViec ca on tblView.ID_CaLamViec = ca.ID
    	join NS_NhanVien nv on tblView.ID_NhanVien= nv.ID     	
    	where exists (select ID from @tblCaLamViec ca2 where ca.ID= ca2.ID) --- van hien nv daxoa de check bang cong cu
		and exists (select TrangThaiNV from @tblTrangThaiNV tt where iif(nv.DaNghiViec='1', 0,isnull(nv.TrangThai,1)) = tt.TrangThaiNV)
		and (@IDPhongBans ='' or 
				----- NV từng chấm công ở CN1, nay chuyển công tác sang CN2
					exists (select pb.ID from @tblPhong pb 
					join NS_QuaTrinhCongTac ct on pb.ID =  ct.ID_PhongBan
					where exists(select Name from dbo.splitstring(@IDChiNhanhs) dv where ct.ID_DonVi= dv.Name)))

    	AND ((select count(Name) from @tblSearchString b where    			
    				nv.TenNhanVien like '%'+b.Name+'%'
    				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
    				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
    				or nv.MaNhanVien like '%'+b.Name+'%'
    				)=@count or @count=0)	
    	),
    	count_cte
    	as (
    	SELECT COUNT(*) AS TotalRow, 
    			CEILING(COUNT(*) / CAST(@PageSize as float )) as TotalPage ,
				SUM(TongCongNV) as TongCongAll,
				SUM(SoPhutDiMuon) as TongSoPhutDiMuon
    	from data_cte
    	)
    	select dt.*, cte.*
    	from data_cte dt
    	cross join count_cte cte		
    	order by dt.TuNgay
    	OFFSET (@CurrentPage* @PageSize) ROWS
    	FETCH NEXT @PageSize ROWS ONLY
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
						when 42 then -hd.TongTienHang
						when 23 then iif(hd.TongTienHang < 0, hd.TongTienHang,0)
					end as PhatSinhGiam,
    				case hd.LoaiHoaDon
						when 22 then N'Nạp thẻ'
						when 32 then N'Hoàn trả cọc'
						when 42 then N'Tất toán công nợ'
						when 23 then iif(hd.TongTienHang > 0,  N'Điều chỉnh tăng', N'Điều chỉnh giảm')
					end as SLoaiHoaDon
    			from BH_HoaDon hd
    			join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
    			join DM_DonVi dv on hd.ID_DonVi= dv.ID
    			where LoaiHoaDon in (22,23, 32, 42) 
				and ChoThanhToan='0' and ID_DoiTuong= @ID_DoiTuong
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

            Sql(@"ALTER PROCEDURE [dbo].[GetList_GoiDichVu_afterUseAndTra]
   @IDChiNhanhs nvarchar(max) = null,
   @IDNhanViens nvarchar(max) = null,
   @DateFrom datetime = null,
   @DateTo datetime = null,
   @TextSearch nvarchar(max) = null,
   @CurrentPage int =null,
   @PageSize int = null
AS
BEGIN

	set nocount on;

	if isnull(@CurrentPage,'') =''
		set @CurrentPage = 0
	if isnull(@PageSize,'') =''
		set @PageSize = 10


	DECLARE @tblNhanVien TABLE (ID UNIQUEIDENTIFIER, TenNhanVien NVARCHAR(MAX));
	IF(ISNULL(@IDNhanViens, '') != '' )
	BEGIN
		INSERT INTO @tblNhanVien
		SELECT nv.ID, nv.TenNhanVien FROM NS_NhanVien nv
		INNER JOIN (SELECT name from dbo.splitstring(@IDNhanViens)) a ON a.Name = nv.ID;
	END
	ELSE
	BEGIN
		INSERT INTO @tblNhanVien
		SELECT ID, TenNhanVien FROM NS_NhanVien;
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max))
	DECLARE @count int
	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!=''
	select @count =  (Select count(*) from @tblSearch)

	DECLARE @VDateFrom DATETIME;
	IF(ISNULL(@DateFrom, '') != '')
	BEGIN
		SET @VDateFrom = @DateFrom;
	END
	ELSE
	BEGIN
		SET @VDateFrom = '1999-01-01';
	END
	DECLARE @VDateTo DATETIME;
	IF(ISNULL(@DateTo, '') != '')
	BEGIN
		SET @VDateTo = @DateTo;
	END
	ELSE
	BEGIN
		SET @VDateTo = DATEADD(DAY, 10, GETDATE());
	END

	DECLARE @tblHoaDonGoiDichVu TABLE(ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), ID_HoaDon UNIQUEIDENTIFIER, ID_BangGia UNIQUEIDENTIFIER,
	ID_NhanVien UNIQUEIDENTIFIER, ID_DonVi UNIQUEIDENTIFIER, NguoiTao NVARCHAR(MAX), ID_Xe UNIQUEIDENTIFIER, DienGiai NVARCHAR(MAX), NgayLapHoaDon DATETIME, TenNhanVien NVARCHAR(MAX), 
	ID_DoiTuong UNIQUEIDENTIFIER, TenDoiTuong NVARCHAR(MAX), MaDoiTuong NVARCHAR(MAX), BienSo NVARCHAR(MAX), PhaiThanhToan FLOAT, TongTienHang FLOAT, TongGiamGia FLOAT, DiemGiaoDich FLOAT,
	LoaiHoaDon INT, TongTienThue FLOAT, TongThueKhachHang FLOAT)

	INSERT INTO @tblHoaDonGoiDichVu
	SELECT 
	hd.ID, 
	hd.MaHoaDon,
	hd.ID_HoaDon,
	hd.ID_BangGia,
	hd.ID_NhanVien,
	hd.ID_DonVi,
	hd.NguoiTao,
	hd.ID_Xe,
	hd.DienGiai,
	hd.NgayLapHoaDon,
	ISNULL(nv.TenNhanVien,'' ) as TenNhanVien,
	hd.ID_DoiTuong,
	ISNULL(dt.TenDoiTuong,N'Khách lẻ' ) as TenDoiTuong,
	ISNULL(dt.MaDoiTuong,'kl' ) as MaDoiTuong,
	xe.BienSo,
	ISNULL(hd.PhaiThanhToan, 0) as PhaiThanhToan,
	ISNULL(hd.TongTienHang, 0) as TongTienHang,
	ISNULL(hd.TongGiamGia, 0) as TongGiamGia,
	ISNULL(hd.DiemGiaoDich, 0) as DiemGiaoDich,
	hd.LoaiHoaDon,
	ISNULL(hd.TongTienThue, 0) as TongTienThue ,
	ISNULL(hd.TongThueKhachHang, 0) as TongThueKhachHang
	--isnull(thuchi.TienThu,0) as KhachDaTra,
	--hd.PhaiThanhToan - isnull(thuchi.TienThu,0) as ConNo
	FROM BH_HoaDon hd
	INNER JOIN (SELECT name FROM dbo.splitstring(ISNULL(@IDChiNhanhs, ''))) dv ON dv.Name = hd.ID_DonVi
	LEFT JOIN @tblNhanVien nv ON hd.ID_NhanVien = nv.ID
	LEFT JOIN Gara_DanhMucXe xe ON xe.ID = hd.ID_Xe
	LEFT JOIN DM_DoiTuong dt ON hd.ID_DoiTuong = dt.ID
	--left join (
	--				select 
	--					tblQuy.ID_HoaDonLienQuan,
	--					sum(tblQuy.ThuTuThe) as ThuTuThe,
	--					sum(tblQuy.TienThu) as TienThu
	--				from
	--				(
	--					---- Thu tu HDMua
	--					select qct.ID_HoaDonLienQuan,
	--						case when qhd.TrangThai= 0 then 0 else SUM(iif(qct.HinhThucThanhToan= 4, qct.TienThu,0)) end as ThuTuThe,							
	--						Case when qhd.TrangThai = 0 then 0 else 
	--							Case when qhd.LoaiHoaDon = 11 then SUM(ISNULL(qct.Tienthu, 0)) else - SUM(ISNULL(qct.Tienthu, 0)) end end as TienThu
	--					from Quy_HoaDon_ChiTiet qct					
	--					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID												
	--				group by qct.ID_HoaDonLienQuan, qhd.TrangThai,qhd.LoaiHoaDon
	--				) tblQuy group by tblQuy.ID_HoaDonLienQuan
	--) thuchi on hd.ID= thuchi.ID_HoaDonLienQuan
	WHERE hd.LoaiHoaDon = 19 and hd.ChoThanhToan = 0 AND hd.NgayLapHoaDon BETWEEN @VDateFrom AND @VDateTo
	and 
	((select count(Name) from @tblSearch b where     			
	hd.MaHoaDon like '%'+b.Name+'%'
	or hd.NguoiTao like '%'+b.Name+'%'
	or xe.BienSo like '%'+b.Name+'%'	
	or dt.MaDoiTuong like '%'+b.Name+'%'		
	or dt.TenDoiTuong like '%'+b.Name+'%'
	or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
	or dt.DienThoai like '%'+b.Name+'%'		
	)=@count or @count=0)

	-- Hóa đơn mua
	DECLARE @tblTonGoiDichVuChiTiet TABLE(IDGoiDV UNIQUEIDENTIFIER, IDDonViQuiDoi UNIQUEIDENTIFIER, IDChiTietGoiDichVu UNIQUEIDENTIFIER, SoLuongTon FLOAT);
	INSERT INTO @tblTonGoiDichVuChiTiet
	select 
		hd.ID as ID_GoiDV,
		hdct.ID_DonViQuiDoi,
		hdct.ID,
		hdct.SoLuong
	from @tblHoaDonGoiDichVu hd
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	where hdct.ID_ChiTietDinhLuong = hdct.ID  OR hdct.ID_ChiTietDinhLuong is null;

	UPDATE tdv
	SET tdv.SoLuongTon = tdv.SoLuongTon - hdsd.SoLuongSuDung
	FROM @tblTonGoiDichVuChiTiet tdv
	INNER JOIN
	(select ct2.ID_ChiTietGoiDV, SUM(ct2.SoLuong) AS SoLuongSuDung	
	from BH_HoaDon_ChiTiet ct2 
	join BH_HoaDon hd2 on ct2.ID_HoaDon =hd2.ID
	where hd2.ChoThanhToan= 0 and hd2.LoaiHoaDon = 1  and (ct2.ID_ChiTietDinhLuong is null or ct2.ID_ChiTietDinhLuong =ct2.ID)
	GROUP BY ct2.ID_ChiTietGoiDV
	) hdsd
	ON hdsd.ID_ChiTietGoiDV = tdv.IDChiTietGoiDichVu;

	DECLARE @tblTonGoiDichVu TABLE(IDGoiDV UNIQUEIDENTIFIER, SoLuongTon FLOAT);
	INSERT INTO @tblTonGoiDichVu
	SELECT IDGoiDV, SUM(SoLuongTon) FROM @tblTonGoiDichVuChiTiet
	GROUP BY IDGoiDV;
	-- Hóa đơn trả
	UPDATE tdv
	SET tdv.SoLuongTon = tdv.SoLuongTon - hdt.SoLuongTra
	FROM @tblTonGoiDichVu tdv
	INNER JOIN
	(select 
		hd3.ID_HoaDon as ID_HoaDonGoc, 
		SUM(ISNULL(ct3.SoLuong, 0)) AS SoLuongTra
	from BH_HoaDon_ChiTiet ct3
	INNER join BH_HoaDon hd3 on ct3.ID_HoaDon = hd3.ID 
	where hd3.ChoThanhToan =0 AND hd3.ID_HoaDon IS NOT NULL
	group by hd3.ID_HoaDon) hdt ON hdt.ID_HoaDonGoc = tdv.IDGoiDV

	select tblView.*,
		isnull(soquy.TienThu,0) as HDGoc_DaTra,
		tblView.PhaiThanhToan -  isnull(soquy.TienThu,0) as ConNoHDGoc,
		Isnull(hdt.PhaiThanhToan,0) as TongTra
	into #tblX
	from
	(
		SELECT R.ID, R.MaHoaDon, R.ID_HoaDon, R.ID_BangGia, R.ID_NhanVien, R.ID_DonVi, R.NguoiTao, R.ID_Xe,
		R.DienGiai, R.NgayLapHoaDon, R.TenNhanVien, R.ID_DoiTuong, R.TenDoiTuong, R.MaDoiTuong,
		R.BienSo, R.PhaiThanhToan, R.TongTienHang, R.TongGiamGia, R.DiemGiaoDich, R.LoaiHoaDon, R.TongTienThue, R.TongThueKhachHang,
		R.TotalRow, R.SoLuongTon AS SoLuongConLai FROM
		(SELECT row_number() over (order by gdv.NgayLapHoaDon desc) as Rn,
					COUNT(gdv.ID) OVER () as TotalRow, * FROM @tblHoaDonGoiDichVu gdv
		INNER JOIN @tblTonGoiDichVu tdv ON gdv.ID = tdv.IDGoiDV
		WHERE tdv.SoLuongTon > 0) R
		WHERE R.Rn BETWEEN (@CurrentPage * @PageSize) + 1 AND @PageSize * (@CurrentPage + 1)
	) tblView
	left join (
					select 
						tblQuy.ID_HoaDonLienQuan,
						sum(tblQuy.ThuTuThe) as ThuTuThe,
						sum(tblQuy.TienThu) as TienThu
					from
					(
						---- Thu tu HDMua
						select qct.ID_HoaDonLienQuan,
							case when qhd.TrangThai= 0 then 0 else SUM(iif(qct.HinhThucThanhToan= 4, qct.TienThu,0)) end as ThuTuThe,							
							Case when qhd.TrangThai = 0 then 0 else 
								Case when qhd.LoaiHoaDon = 11 then SUM(ISNULL(qct.Tienthu, 0)) else - SUM(ISNULL(qct.Tienthu, 0)) end end as TienThu
						from Quy_HoaDon_ChiTiet qct					
						join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID												
					group by qct.ID_HoaDonLienQuan, qhd.TrangThai,qhd.LoaiHoaDon
					) tblQuy group by tblQuy.ID_HoaDonLienQuan
	) soquy on tblView.ID = soquy.ID_HoaDonLienQuan
	left join BH_HoaDon hdt on tblView.ID= hdt.ID_HoaDon and hdt.ChoThanhToan='0'
	
	
	select tblView.*,
		isnull(tbl.TongGoc,0) as TongGoc,
		isnull(tbl.TongThuGoc,0) as TongThuGoc,
		isnull(tbl.TongTra,0) as TongTra,
		case when tblView.ID_HoaDon is null	
			then iif(tblView.TongTra >= tblView.ConNoHDGoc,0, tblView.ConNoHDGoc - tblView.TongTra)
		else tblView.ConNoHDGoc + isnull(tbl.TongGoc,0) -isnull(tbl.TongThuGoc,0) -isnull(tbl.TongTra,0) end as ConNo
	from #tblX tblView
	left join
	(
		----- get hdGoc + hdTra
		select hdTra.ID, tbl.ID_HoaDon,			
			ISNULL(hdGoc.PhaiThanhToan, 0) AS TongGoc,
			isnull(soquy.TienThu,0) as TongThuGoc,
			hdTra.PhaiThanhToan as TongTra
		from BH_HoaDon hdTra
		join #tblX tbl on hdTra.ID = tbl.ID_HoaDon
		left join BH_HoaDon hdGoc on hdTra.ID_HoaDon = hdGoc.ID
		left join (
						select 
							tblQuy.ID_HoaDonLienQuan,
							sum(tblQuy.ThuTuThe) as ThuTuThe,
							sum(tblQuy.TienThu) as TienThu
						from
						(
							---- Thu tu HDGoc
							select qct.ID_HoaDonLienQuan,
								case when qhd.TrangThai= 0 then 0 else SUM(iif(qct.HinhThucThanhToan= 4, qct.TienThu,0)) end as ThuTuThe,							
								Case when qhd.TrangThai = 0 then 0 else 
									Case when qhd.LoaiHoaDon = 11 then SUM(ISNULL(qct.Tienthu, 0)) else - SUM(ISNULL(qct.Tienthu, 0)) end end as TienThu
							from Quy_HoaDon_ChiTiet qct					
							join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID												
						group by qct.ID_HoaDonLienQuan, qhd.TrangThai,qhd.LoaiHoaDon
						) tblQuy group by tblQuy.ID_HoaDonLienQuan
		) soquy on hdGoc.ID = soquy.ID_HoaDonLienQuan		
	) tbl on  tblView.ID_HoaDon  = tbl.ID_HoaDon
	order by tblView.NgayLapHoaDon desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[getlist_HoaDonBanHang]	
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @maHD [nvarchar](max),
	@ID_NhanVienLogin uniqueidentifier,
	@NguoiTao nvarchar(max),
	@IDViTris nvarchar(max),
	@IDBangGias nvarchar(max),
	@TrangThai nvarchar(max),
	@PhuongThucThanhToan nvarchar(max),
	@ColumnSort varchar(max),
	@SortBy varchar(max),
	@CurrentPage int,
	@PageSize int,
	@LaHoaDonSuaChua nvarchar(10),
	@BaoHiem int
AS
BEGIN

  set nocount on;
 declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanh,'HoaDon_XemDS_PhongBan','HoaDon_XemDS_HeThong');

	declare @tblChiNhanh table (ID varchar(40))
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@ID_ChiNhanh);

	declare @tblPhuongThuc table (PhuongThuc varchar(4))
	insert into @tblPhuongThuc
	select Name from dbo.splitstring(@PhuongThucThanhToan)

	declare @tblTrangThai table (TrangThaiHD varchar(40))
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
						Case When hd.ChoThanhToan = '1' then N'Phiếu tạm' when hd.ChoThanhToan = '0' then N'Hoàn thành' else N'Đã hủy' end as TrangThai,
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
							iif(qhd.TrangThai='0',0,iif(dt.LoaiDoiTuong =1, iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu),0)) as KhachDaTra,
							0 as ThuDatHang,
							iif(qhd.TrangThai='0',0,iif(dt.LoaiDoiTuong =3, iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu),0)) as BaoHiemDaTra						
						from BH_HoaDon hd
						left join Quy_HoaDon_ChiTiet qct on hd.ID = qct.ID_HoaDonLienQuan	
						left join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 
						left join DM_DoiTuong dt on qct.ID_DoiTuong = dt.ID				
						where hd.NgayLapHoadon between @timeStart and @timeEnd					
						and exists (select loai.Loai from @tblLoaiHoaDon loai where hd.LoaiHoaDon = loai.Loai)
						and exists (select cn.ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID) 
					

						union all

						Select
							thuDH.ID,
							thuDH.TienMat,
							thuDH.TienATM,
							thuDH.ChuyenKhoan,
							thuDH.TienDoiDiem,
							thuDH.ThuTuThe,
							thuDH.TienDatCoc,
							thuDH.TienThu,
							thuDH.TienThu as KhachDaTra,
							thuDH.TienThu as ThuDatHang,
							0 as BaoHiemDaTra
						FROM
						(
							Select 
									ROW_NUMBER() OVER(PARTITION BY d.ID_HoaDon ORDER BY d.NgayLapHoaDon ASC) AS isFirst,						
    								d.ID,
									d.ID_HoaDon,
									d.NgayLapHoaDon,
    								sum(d.TienMat) as TienMat,
    								SUM(ISNULL(d.TienATM, 0)) as TienATM,
    								SUM(ISNULL(d.TienCK, 0)) as ChuyenKhoan,
									SUM(ISNULL(d.TienDoiDiem, 0)) as TienDoiDiem,
									sum(d.ThuTuThe) as ThuTuThe,
    								sum(d.TienThu) as TienThu,
									sum(d.TienDatCoc) as TienDatCoc
							FROM 
							(
								select 
									hd.ID,
									hd.NgayLapHoaDon,
									hdd.ID as ID_HoaDon,
									iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=1, qct.TienThu, 0), iif(qct.HinhThucThanhToan=1, -qct.TienThu, 0)) as TienMat,
									iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=2, qct.TienThu, 0), iif(qct.HinhThucThanhToan=2, -qct.TienThu, 0)) as TienATM,
									iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=3, qct.TienThu, 0), iif(qct.HinhThucThanhToan=3, -qct.TienThu, 0)) as TienCK,
									iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=5, qct.TienThu, 0), iif(qct.HinhThucThanhToan=5, -qct.TienThu, 0)) as TienDoiDiem,
									iif(qhd.LoaiHoaDon = 11, iif(qct.HinhThucThanhToan=4, qct.TienThu, 0), iif(qct.HinhThucThanhToan=4, -qct.TienThu, 0)) as ThuTuThe,
									iif(qhd.LoaiHoaDon = 11, qct.TienThu, -qct.TienThu) as TienThu,
									iif(qct.HinhThucThanhToan=6,qct.TienThu,0) as TienDatCoc	
								from
								(
									select 
										ID
									from BH_HoaDon where LoaiHoaDon= 3
									) hdd
									join BH_HoaDon hd on hd.ID_HoaDon= hdd.ID					
									join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDonLienQuan = hdd.ID
									join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID					
									where (qhd.TrangThai= 1 Or qhd.TrangThai is null)										
									and	hd.NgayLapHoadon between @timeStart and @timeEnd					
									and exists (select loai.Loai from @tblLoaiHoaDon loai where hd.LoaiHoaDon = loai.Loai)
									and exists (select cn.ID from @tblChiNhanh cn where hd.ID_DonVi = cn.ID) 
							)  d group by d.ID,d.NgayLapHoaDon,ID_HoaDon		
						) thuDH where isFirst= 1
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
			and ((select count(Name) from @tblSearch b where     			
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
			ISNULL(vt.TenViTri,'') as TenPhongBan
		from tView hd
		left join DM_DonVi dv on hd.ID_DonVi = dv.ID
		left join DM_TinhThanh tt on hd.ID_TinhThanh = tt.ID
		left join DM_QuanHuyen qh on hd.ID_QuanHuyen = qh.ID
		left join DM_GiaBan gb on hd.ID_BangGia = gb.ID
		left join DM_ViTri vt on hd.ID_ViTri = vt.ID

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
	
	if isnull(@CurrentPage,'') ='' set @CurrentPage= 0
	if isnull(@PageSize,'') ='' set @PageSize= 10

	declare @tblChiNhanh table(ID uniqueidentifier)
	if isnull(@IDChiNhanhs,'') !=''
	begin	
		insert into @tblChiNhanh 
		select name from dbo.splitstring(@IDChiNhanhs)
	end
	else
		set @IDChiNhanhs =''
	
	declare @tblLoai table(Loai int)
	if isnull(@LoaiHoaDons,'') !=''
	begin		
		insert into @tblLoai 
		select name from dbo.splitstring(@LoaiHoaDons)
	end
	else set @LoaiHoaDons =''

	declare @tblTrangThai table(TrangThai int)
	if isnull(@TrangThais,'') !=''
	begin		
		insert into @tblTrangThai 
		select name from dbo.splitstring(@TrangThais)	
	end
	else set @TrangThais =''

	DECLARE @tblSearchString TABLE (Name [nvarchar](max))
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch,' ') where  Name!=''
	Select @count =  (Select count(*) from @tblSearchString)


								

	; with data_cte
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
					
					hdsc.MaHoaDon as MaHoaDonGoc,
							
					dv.TenDonVi,
					nv.TenNhanVien,
					tbl.TenDoiTuong,
					tbl.MaDoiTuong,
					cast(tbl.TrangThai as varchar(2)) as TrangThai
			from  
			(
				select 
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
						when 8 then iif(hd.ChoThanhToan='0',hd.NgayLapHoaDon, hd.NgaySua)
					else hd.NgaySua end as NgaySua,
					isnull(hd.An_Hien,'0') as IsChuyenPhatNhanh,
					case hd.ChoThanhToan
						when 1 then 1
						when 0 then 2
					else 3 end as TrangThai,
					case hd.ChoThanhToan
						when 1 then N'Tạm lưu'
						when 0 then N'Hoàn thành'
					else N'Hủy bỏ' end as YeuCau,    
					---- 1.sudung gdv, 2.xuat banle, 3.xuat suachua, 8.xuatkho thuong, 12.xuatbaohanh, 35. xuat NVL, 36. xuat hotro chung, 37. xuat hotro ngaythuoc
					case hd.LoaiHoaDon			
						when 2 then 12
					else hd.LoaiHoaDon end LoaiHoaDon,
					case hd.LoaiHoaDon
						when 8 then N'Phiếu xuất kho'					
						when 40 then N'Xuất hỗ trợ chung' 
						when 39 then N'Xuất bảo hành'
						when 38 then N'Xuất bán lẻ'
						when 37 then N'Xuất hỗ trợ ngày thuốc'
						when 35 then N'Xuất nguyên vật liệu'
					else N'Xuất bán lẻ' end LoaiPhieu,

					case hd.LoaiHoaDon
						when 8 then ''
						when 40 then dt.TenDoiTuong
						when 39 then dt.TenDoiTuong
						when 38 then dt.TenDoiTuong
						when 37 then dt.TenDoiTuong
						when 35 then dt.TenDoiTuong
					else '' end TenDoiTuong,
					case hd.LoaiHoaDon
						when 8 then ''
						when 40 then dt.TenDoiTuong_KhongDau
						when 39 then dt.TenDoiTuong_KhongDau
						when 38 then dt.TenDoiTuong_KhongDau
						when 37 then dt.TenDoiTuong_KhongDau
						when 35 then dt.TenDoiTuong_KhongDau
					else '' end TenDoiTuong_KhongDau,
					case hd.LoaiHoaDon
						when 8 then ''
						when 40 then dt.MaDoiTuong
						when 39 then dt.MaDoiTuong
						when 38 then dt.MaDoiTuong
						when 37 then dt.MaDoiTuong
						when 35 then dt.MaDoiTuong
					else '' end MaDoiTuong
			from BH_HoaDon hd
			join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon			
			left join DM_DoiTuong dt on hd.ID_Doituong= dt.ID
			where (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID))
				and hd.NgayLapHoaDon between @DateFrom and @DateTo
			) tbl
			join DM_DonVi dv on tbl.ID_DonVi = dv.ID
			left join BH_HoaDon hdsc on tbl.ID_HoaDon= hdsc.ID		
    		left join NS_NhanVien nv on tbl.ID_NhanVien = nv.ID
			where  (@LoaiHoaDons='' or  exists (select Loai from @tblLoai loai where tbl.LoaiHoaDon= loai.Loai))
				and (@TrangThais='' or exists (select TrangThai from @tblTrangThai tt where tbl.TrangThai= tt.TrangThai))
				and ((select count(Name) from @tblSearchString b 
									where tbl.MaHoaDon like '%' +b.Name +'%' 										
										or tbl.NguoiTaoHD like '%' +b.Name +'%' 
										or tbl.DienGiai like N'%'+b.Name+'%'
										or nv.TenNhanVien like '%'+b.Name+'%'
										or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
										or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
										or nv.MaNhanVien like '%'+b.Name+'%'									
										or hdsc.MaHoaDon like '%'+b.Name+'%'
										or tbl.TenDoiTuong like '%'+b.Name+'%'
										or tbl.TenDoiTuong_KhongDau like '%'+b.Name+'%'
										or tbl.MaDoiTuong like '%'+b.Name+'%'																		
										or tbl.DienGiaiUnsign like N'%'+b.Name+'%'
										)=@count or @count=0)
			 group by  
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
					
					hdsc.MaHoaDon,
							
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
			OFFSET (@CurrentPage * @PageSize) ROWS
			FETCH NEXT @PageSize ROWS ONLY; 
			

			--print @sql
		
	
				
	
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListCashFlow_Before]
    @IDDonVis [nvarchar](max),
    @ID_NhanVien [nvarchar](40),
    @ID_TaiKhoanNganHang [nvarchar](40),
    @ID_KhoanThuChi [nvarchar](40),
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiSoQuy varchar(15),
    @LoaiChungTu [nvarchar](2),
    @TrangThaiSoQuy [nvarchar](2),
    @TrangThaiHachToan [nvarchar](2),
    @TxtSearch [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;

	declare @tblDonVi table (ID_DonVi uniqueidentifier)
	insert into @tblDonVi
	select name from dbo.splitstring(@IDDonVis)

	declare @tblLoai table (Loai int)
	insert into @tblLoai
	select name from dbo.splitstring(@LoaiSoQuy)

	--declare @nguoitao nvarchar(100) = (select taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
	--declare @tblNhanVien table (ID uniqueidentifier)
	--insert into @tblNhanVien
	--select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDDonVis,'SoQuy_XemDS_PhongBan','SoQuy_XemDS_HeThong');
 
 select 
	ceiling(sum(ThuMat)- sum(ChiMat)) as TongThuMat,  
	ceiling(sum(ThuGui) - sum(ChiGui)) as TongThuCK
 from
 (
 select
	iif(a1.LoaiHoaDon=11, TienMat,0) as ThuMat,
	iif(a1.LoaiHoaDon=12, TienMat,0) as ChiMat,
	iif(a1.LoaiHoaDon=11, TienGui,0) as ThuGui,
	iif(a1.LoaiHoaDon=12, TienGui,0) as ChiGui
 from
 (
    	 select
			tblView.LoaiHoaDon,
			sum(TienMat) as TienMat,
			sum(TienGui) as TienGui
	from
		(select 
			tblQuy.MaHoaDon,		
			tblQuy.LoaiHoaDon,
			ISNUll(tblQuy.TrangThai,'1') as TrangThai,
			tblQuy.NoiDungThu,
			tblQuy.ID_NhanVienPT as ID_NhanVien,			
			TienMat, TienGui, TienMat + TienGui as TienThu,
			TienMat + TienGui as TongTienThu,
			cast(ID_TaiKhoanNganHang as varchar(max)) as ID_TaiKhoanNganHang,
			case when tblQuy.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
    		case when tblQuy.HachToanKinhDoanh = '1' then '11' else '10' end as TrangThaiHachToan,
    		case when tblQuy.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
			case when nv.TenNhanVien is null then  dt.TenDoiTuong  else nv.TenNhanVien end as NguoiNopTien,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong_KhongDau  else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
    		case when nv.MaNhanVien is null then dt.MaDoiTuong else  nv.MaNhanVien end as MaDoiTuong,
    		case when nv.MaNhanVien is null then dt.DienThoai else  nv.DienThoaiDiDong  end as SoDienThoai,
			case when tblQuy.TienMat > 0 then case when tblQuy.TienGui > 0 then '2' else '1' end 
			else case when tblQuy.TienGui > 0 then '0'
				else case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then '0' else '1' end end end as LoaiSoQuy
							
		from
			(select 
				 a.ID_hoaDon, 
				 a.MaHoaDon,			
				 a.LoaiHoaDon,
				 a.HachToanKinhDoanh, 
				 a.NoiDungThu,
				 a.ID_NhanVienPT, a.TrangThai,
				 sum(isnull(a.TienMat, 0)) as TienMat,
				 sum(isnull(a.TienGui, 0)) as TienGui,
				 max(a.ID_DoiTuong) as ID_DoiTuong,
				 max(a.ID_NhanVien) as ID_NhanVien,
				 max(a.ID_TaiKhoanNganHang) as ID_TaiKhoanNganHang			
			from
			(				
					select qhd.MaHoaDon,				
					qhd.LoaiHoaDon,
					qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo, qhd.NoiDungThu,
					qhd.ID_NhanVien as ID_NhanVienPT, qhd.TrangThai,
					qct.ID_HoaDon, 
					iif(qct.HinhThucThanhToan= 1, qct.TienThu,0) as TienMat,
					iif(qct.HinhThucThanhToan= 2 or qct.HinhThucThanhToan = 3, qct.TienThu,0) as TienGui,		
					qct.ID_DoiTuong, qct.ID_NhanVien, 
					ISNULL(qct.ID_TaiKhoanNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
					ISNULL(qct.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
					join @tblDonVi cn on qhd.ID_DonVi= cn.ID_DonVi
					left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang= tk.ID
					left join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi= ktc.ID
					where qhd.NgayLapHoaDon < @DateFrom
					and qct.HinhThucThanhToan not in (4,5,6)
					and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
					and (@ID_TaiKhoanNganHang  ='%%' Or qct.ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang)
					and (@ID_KhoanThuChi ='%%' or qct.ID_KhoanThuChi like @ID_KhoanThuChi)				
			) a
			 group by a.ID_HoaDon, a.MaHoaDon, 
				a.LoaiHoaDon,
				a.HachToanKinhDoanh, a.PhieuDieuChinhCongNo, a.NoiDungThu,
				a.ID_NhanVienPT , a.TrangThai
		) tblQuy
		left join DM_DoiTuong dt on tblQuy.ID_DoiTuong = dt.ID
		left join NS_NhanVien nv on tblQuy.ID_NhanVien= nv.ID
	 ) tblView
	 where tblView.TrangThaiHachToan like '%'+ @TrangThaiHachToan + '%'
	 and tblView.MaHoaDon not like 'CB%'		
    	and tblView.TrangThaiSoQuy like '%'+ @TrangThaiSoQuy + '%'
    	and tblView.LoaiChungTu like '%'+ @LoaiChungTu + '%'
    	and ID_NhanVien like @ID_NhanVien
		and exists (select Loai from @tblLoai loai where LoaiSoQuy= loai.Loai)    
		and (MaHoaDon like @TxtSearch OR MaDoiTuong like @TxtSearch OR NguoiNopTien like @TxtSearch
		OR TenDoiTuong_KhongDau like @TxtSearch OR dbo.FUNC_ConvertStringToUnsign(NoiDungThu) like @TxtSearch)
		group by tblView.LoaiHoaDon
		) a1
		) b
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
			hh.ID_NhomHang as ID_NhomHangHoa,
			nhom.TenNhomHangHoa,
			Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
			Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
			Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan
    	
    	FROM DonViQuiDoi dvqd     	
    	join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID		
		left join DM_LoHang lh on hh.ID = lh.ID_HangHoa		
		left join DM_NhomHangHoa nhom on hh.ID_NhomHang = nhom.ID
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
			hh.ID_NhomHang as ID_NhomHangHoa,
			nhom.TenNhomHangHoa,
			Case when lh.ID is null then '' else lh.MaLoHang end as TenLoHang,
			Case when lh.ID is null then '' else lh.NgaySanXuat end as NgaySanXuat,
			Case when lh.ID is null then '' else lh.NgayHetHan end as NgayHetHan    	
		FROM DonViQuiDoi dvqd     	
		join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
		left join DM_LoHang lh on hh.ID = lh.ID_HangHoa	
		left join DM_NhomHangHoa nhom on hh.ID_NhomHang = nhom.ID
		where hh.ID_NhomHang in (select * from splitstring(@IDNhomHangs))
    		and dvqd.Xoa = '0'
			and dvqd.LaDonViChuan = 1
    		and hh.TheoDoi = 1
			and IIF(hh.LoaiHangHoa is not null, hh.LoaiHangHoa, iif(hh.LaHangHoa=0,2,1)) in (select Loaihang from @tblLoaiHang)
	end
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListHoaDon_byIDCus]
    @ID_NguoiGioiThieu [uniqueidentifier],
    @ID_DoiTuong [uniqueidentifier],
    @IDChiNhanhs [nvarchar](max),
    @DateFrom [datetime],
    @DateTo [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @tblChiNhanh table (ID uniqueidentifier)
    	if isnull(@IDChiNhanhs,'') !=''
    		insert into @tblChiNhanh
    		select name from dbo.splitstring(@IDChiNhanhs)
    
    
    	select *,
    	tblView.KhachDaTra - tblView.DaTrich as ConLai,
    	iif(tblView.KhachDaTra > tblView.DaTrich, 0,1) as TrangThai --- 0.chua trichdu, 1.da trich du
    	from
    	(
    		select 
    			allHD.*,
    			isnull(tblDaTrich.DaTrich,0) as DaTrich,
    			tblDaTrich.ID_NguoiGioiThieu
    			
    		from
    		(
    			------ get allhoadon of cus & soquy dathanhtoan
    			select hd.ID as ID_HoaDon_DuocCK,				
    					hd.MaHoaDon,
    					hd.NgayLapHoaDon,
    					hd.TongThanhToan,
    					hd.ID_DoiTuong,
    					isnull(sum(iif(qhd.LoaiHoaDon = 11, qct.TienThu, - qct.TienThu)),0) as KhachDaTra
    			from
    			(
    				---- only get hd have TongThanhToan > 0
    				select hd.ID, hd.ID_DoiTuong, hd.LoaiHoaDon, hd.MaHoaDon, hd.NgayLapHoaDon,hd.TongThanhToan
    				from BH_HoaDon hd
    				where hd.ID_DoiTuong= @ID_DoiTuong
    				and hd.TongThanhToan > 0
    				and hd.LoaiHoaDon in (1,19,22)
    				and hd.ChoThanhToan='0'
    				and hd.NgayLapHoaDon between @DateFrom and @DateTo
    				and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where hd.ID_DonVi= cn.ID))
    			) hd
    			join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan 
    			join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 	
    			where (qhd.TrangThai is null or qhd.TrangThai='1') --- chi get hd dathanhtoan (vi hoahong tinh theo thuc thu)
    			and qct.HinhThucThanhToan not in (4,5,6)
    			group by hd.ID, hd.LoaiHoaDon, hd.ID_DoiTuong, hd.MaHoaDon, hd.NgayLapHoaDon,hd.TongThanhToan
    		) allHD
    		left join
    		(
    			----- get all hoadon da duoc trich hoahong of cus
    			select 
    				hd.ID,		
    				pthh.ID_CheckIn as ID_NguoiGioiThieu,
    				pthh.TongChietKhau as LoaiDoiTuong,
    				sum(isnull(cthh.TienChiPhi,0)) as DaTrich
    			from BH_HoaDon_ChiTiet cthh
    			join BH_HoaDon pthh on cthh.ID_HoaDon = pthh.ID
    			join BH_HoaDon hd on cthh.ID_ParentCombo= hd.ID
    			where pthh.LoaiHoaDon = 41 
    			and pthh.ChoThanhToan='0'
    			and pthh.ID_CheckIn =  @ID_NguoiGioiThieu
    			and hd.ID_DoiTuong= @ID_DoiTuong
    			and hd.ChoThanhToan='0'
    			and hd.NgayLapHoaDon between @DateFrom and @DateTo
    			and cthh.TienChietKhau > 0
    			and (@IDChiNhanhs ='' or exists (select ID from @tblChiNhanh cn where pthh.ID_DonVi= cn.ID))
    			group by  hd.ID, pthh.ID_CheckIn, pthh.TongChietKhau
    		
    		) tblDaTrich on allHD.ID_HoaDon_DuocCK = tblDaTrich.ID		
    	) tblView
    	order by tblView.NgayLapHoaDon 
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListNhomHang_SetupHoTro]
    @IDDonVis [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
    
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
			order by nhom.NgayTao desc
    	  end
    	  else
    	  begin
    		select  nhom.TenNhomHangHoa, ap.Id_NhomHang, ap.GiaTriSuDungTu, ap.GiaTriSuDungDen, ap.GiaTriHoTro, ap.KieuHoTro, ap.STT
    		from NhomHangHoa_DonVi nhomdv
    		join NhomHang_KhoangApDung ap on nhomdv.ID_NhomHangHoa = ap.Id_NhomHang
    		join DM_NhomHangHoa nhom on ap.Id_NhomHang= nhom.ID		
    	    where nhom.TrangThai = 0		
			order by nhom.NgayTao desc
    	  end
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetListSuDungThe]
    @ID_DoiTuong [nvarchar](max),
	@FromDate datetime,
	@ToDate datetime,
	@CurrentPage int,
	@PageSize int
AS
BEGIN

		 ;with data_cte
		 as
		 (
    			select
    				dt.ID,
    				hd.MaHoaDon,
    				qhd.MaHoaDon as MaHoaDonSQ,
    				qhd.NgayLapHoaDon,
    				hd.DienGiai,
    				hd.LoaiHoaDon,
					qhd.LoaiHoaDon as LoaiHoaDonSQ,
					qct.TienThu as TienThe,
    				iif(qhd.LoaiHoaDon =11, qct.TienThu,0) as PhatSinhGiam,
    				iif(qhd.LoaiHoaDon =12, qct.TienThu,0) as PhatSinhTang,
					case hd.LoaiHoaDon
					when 1 then N'Bán hàng'
					when 3 then N'Báo giá'
					when 6 then N'Trả hàng'
					when 19 then N'Gói dịch vụ'
					when 25 then N'Sửa chữa'
					else '' end as SLoaiHoaDon
    			
    				from DM_DoiTuong dt
    				left join Quy_HoaDon_ChiTiet qct on dt.ID = qct.ID_DoiTuong
    				left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    				left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    				where hd.ID_DoiTuong = @ID_DoiTuong 
					and qct.HinhThucThanhToan = 4 and hd.ChoThanhToan = 0
					and qhd.NgayLapHoaDon between @FromDate and @ToDate	
    	),
		count_cte
		as
		(	
			select count(ID) as TotalRow,
					CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
					sum(PhatSinhTang) as TongTienTang,
					sum(PhatSinhGiam) as TongTienGiam					
				from data_cte
		)
			select dt.*, cte.*
			from data_cte dt
			cross join count_cte cte
			order by dt.NgayLapHoaDon desc
			OFFSET (@CurrentPage* @PageSize) ROWS
			FETCH NEXT @PageSize ROWS ONLY
    	

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
    		--ISNULL(tblThe.NhanVienThucHien,'') as NhanVienThucHien,
    		tblThe.MaDoiTuong as MaKhachHang,
    		tblThe.TenDoiTuong as TenKhachHang,
    		tblThe.DienThoai as SoDienThoai,
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
						iif(hd.TongChiPhi=0,0, hd.TongGiamGia/hd.TongChiPhi * 100) as ChietKhauPT,
						iif(hd.TongChiPhi=0,0, hd.TongChietKhau/hd.TongChiPhi * 100) as KhuyenMaiPT,
    					dt.MaDoiTuong, dt.TenDoiTuong,
    					dt.DienThoai, 
    					dt.DiaChi,
    					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai --,
    					--NhanVienThucHien
    				from BH_HoaDon hd
    				join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    				--left join (
    				--		Select distinct
    				--			(
    				--				Select distinct nv.TenNhanVien + ',' AS [text()]
    				--				From dbo.BH_NhanVienThucHien th
    				--				join dbo.NS_NhanVien nv on th.ID_NhanVien = nv.ID
    				--				where th.ID_HoaDon= nvth.ID_HoaDon
    				--				For XML PATH ('')
    				--			) NhanVienThucHien, nvth.ID_HoaDon
    				--			From dbo.BH_NhanVienThucHien nvth
    				--			) nvThucHien on hd.ID = nvThucHien.ID_HoaDon
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
    						iif(qct.HinhThucThanhToan = 3, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0) as TienPOS,
    						iif(qct.HinhThucThanhToan = 3, iif(qhd.LoaiHoaDon=11, qct.TienThu,-qct.TienThu),0)  as TienCK,
    						iif(qct.HinhThucThanhToan = 2,iif(qct.LaPTChiPhiNganHang='0',qct.ChiPhiNganHang, 
							qct.TienThu * qct.ChiPhiNganHang/1000),0) as TongPhiNganHang ---- apply pos					
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

            Sql(@"ALTER PROCEDURE [dbo].[GetMaHoaDonMax_byTemp]
	@LoaiHoaDon int,
	@ID_DonVi uniqueidentifier,
	@NgayLapHoaDon datetime
AS
BEGIN	
	----- sử dụng IX_LoaiHoaDon (LoaiHoaDon) in dbo.BH_HoaDon để tối ưu
	SET NOCOUNT ON;

	DECLARE @ReturnVC NVARCHAR(MAX);
	declare @NgayLapHoaDon_Compare datetime = dateadd(month, -1, @NgayLapHoaDon)	
	declare @ngaytaoHDMax datetime
	
	declare @tblLoaiHD table (LoaiHD int)
	if @LoaiHoaDon = 4 or @LoaiHoaDon = 13 or @LoaiHoaDon = 14 ---- nhapkho noibo + nhaphangthua = dùng chung mã với nhập kho nhà cung cấp
		begin
			set @LoaiHoaDon = 4
			insert into @tblLoaiHD values (4),(13),(14)			
		end
	else 
		begin
			insert into @tblLoaiHD values (@LoaiHoaDon)	
		end
	
	
	DECLARE @Return float = 1
	declare @lenMaMax int = 0
	declare @kihieuchungtu varchar(10),  @lenMaChungTu int =0

	DECLARE @isDefault bit = (select top 1 SuDungMaChungTu from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi)-- co/khong thiet lap su dung Ma MacDinh
	DECLARE @isSetup int = (select top 1 ID_LoaiChungTu from HT_MaChungTu where ID_LoaiChungTu = @LoaiHoaDon)-- da ton tai trong bang thiet lap chua

	if @isDefault='1' and @isSetup is not null
		begin
			DECLARE @machinhanh varchar(15) = (select MaDonVi from DM_DonVi where ID= @ID_DonVi)
			
			DECLARE @isUseMaChiNhanh varchar(15), @kituphancach1 varchar(1),  @kituphancach2 varchar(1),  @kituphancach3 varchar(1),
			 @dinhdangngay varchar(8), @dodaiSTT INT

			 select @isUseMaChiNhanh = SuDungMaDonVi, 
				@kituphancach1= KiTuNganCach1,
				@kituphancach2 = KiTuNganCach2,
				@kituphancach3= KiTuNganCach3,
				@dinhdangngay = NgayThangNam,
				@dodaiSTT = CAST(DoDaiSTT AS INT),
				@kihieuchungtu = MaLoaiChungTu
			 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon 

			
			
			DECLARE @namthangngay varchar(10) = convert(varchar(10), @NgayLapHoaDon, 112)
			DECLARE @year varchar(4) = Left(@namthangngay,4)
			DECLARE @date varchar(4) = right(@namthangngay,2)
			DECLARE @month varchar(4) = substring(@namthangngay,5,2)
			DECLARE @datecompare varchar(10)='';
			
			if	@isUseMaChiNhanh='0'
			begin 
				set @machinhanh=''
				set @kituphancach1 =''													
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

			DECLARE @sMaFull varchar(50) = concat(@machinhanh,@kituphancach1,@kihieuchungtu,@kituphancach2, @datecompare, @kituphancach3)	

			declare @sCompare varchar(30) = @sMaFull
			if @sMaFull= concat(@kihieuchungtu,'_') set @sCompare = concat(@kihieuchungtu,'[_]') -- like %_% không nhận kí tự _ nên phải [_] theo quy tắc của sql
			
			set @lenMaChungTu = Len(@sMaFull); ----- !!important: chỉ lấy những kí tự nằm bên phải mã full

				----- don't check ID_DonVi (because MaHoaDon like @sCompare contains (MaChiNhanh)
				select @ngaytaoHDMax = max(NgayTao)
				from BH_HoaDon hd				
				where MaHoaDon like @sCompare +'%'
				and exists (select * from @tblLoaiHD loai where hd.LoaiHoaDon= loai.LoaiHD)

				if @NgayLapHoaDon_Compare  > @ngaytaoHDMax set @NgayLapHoaDon_Compare = format(@ngaytaoHDMax,'yyyy-MM-dd')

				SET @Return = 
				(
					select  
						max(CAST(dbo.udf_GetNumeric(RIGHT(MaHoaDon,LEN(MaHoaDon)- @lenMaChungTu))AS float)) as MaxNumber
					from
					(
						select MaHoaDon
						from BH_HoaDon hd				
						where MaHoaDon like @sCompare +'%'
						and exists (select * from @tblLoaiHD loai where hd.LoaiHoaDon= loai.LoaiHD)
						and NgayTao > @NgayLapHoaDon_Compare									
					) a
					where ISNUMERIC(RIGHT(MaHoaDon,LEN(MaHoaDon)- @lenMaChungTu)) = 1
				)

				
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
					set @ReturnVC = FORMAT(@Return + 1, 'F0');
					set @lenMaMax = len(@ReturnVC)

					-- neu @Return là 1 số quá lớn --> mã bị chuyển về dạng e+10
					declare @madai nvarchar(max)= CONCAT(@sMaFull, CONVERT(numeric(22,0), @ReturnVC))
					select 
						case @lenMaMax							
							when 1 then CONCAT(@sMaFull,left(@strstt,@lenSst-1),@ReturnVC)
							when 2 then case when @lenSst - 2 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-2), @ReturnVC) else @madai end
							when 3 then case when @lenSst - 3 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-3), @ReturnVC) else @madai end
							when 4 then case when @lenSst - 4 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-4), @ReturnVC) else @madai end
							when 5 then case when @lenSst - 5 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-5), @ReturnVC) else @madai end
							when 6 then case when @lenSst - 6 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-6), @ReturnVC) else @madai end
							when 7 then case when @lenSst - 7 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-7), @ReturnVC) else @madai end
							when 8 then case when @lenSst - 8 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-8), @ReturnVC) else @madai end
							when 9 then case when @lenSst - 9 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-9), @ReturnVC) else @madai end
							when 10 then case when @lenSst - 10 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-10), @ReturnVC) else @madai end
						else 
							case when  @lenMaMax > 10
								 then iif(@lenSst - 10 > -1, CONCAT(@sMaFull, left(@strstt,@lenSst-10), @ReturnVC),  @madai)
								 else '' end
						end as MaxCode					
				end 
		end
	else
		begin		
			set @kihieuchungtu = (select top 1 MaLoaiChungTu from DM_LoaiChungTu where ID= @LoaiHoaDon)
			set @lenMaChungTu  = LEN(@kihieuchungtu)
						
			IF @LoaiHoaDon = 30
			BEGIN

				----- phieu bangiaoxe (not use)
				DECLARE @MaHoaDonMax NVARCHAR(MAX);
				
				select TOP 1 @MaHoaDonMax = MaPhieu
				from Gara_Xe_PhieuBanGiao 
				where SUBSTRING(MaPhieu, 1, len(@kihieuchungtu)) = @kihieuchungtu 
				and CHARINDEX('O',MaPhieu) = 0 
				AND LEN(MaPhieu) = 10 + @lenMaChungTu 
				AND ISNUMERIC(RIGHT(MaPhieu,LEN(MaPhieu)- @lenMaChungTu)) = 1
				ORDER BY MaPhieu DESC;

				SET @Return = CAST(dbo.udf_GetNumeric(RIGHT(@MaHoaDonMax,LEN(@MaHoaDonMax)- @lenMaChungTu))AS float);
			END
			ELSE
			BEGIN
				select @ngaytaoHDMax = max(NgayTao)
				from BH_HoaDon hd				
				where MaHoaDon like @kihieuchungtu +'%'
				and exists (select * from @tblLoaiHD loai where hd.LoaiHoaDon= loai.LoaiHD)

				if @NgayLapHoaDon_Compare  > @ngaytaoHDMax set @NgayLapHoaDon_Compare = format(@ngaytaoHDMax,'yyyy-MM-dd')			

				SET @Return = 
				(
					select  
						max(CAST(dbo.udf_GetNumeric(RIGHT(MaHoaDon,LEN(MaHoaDon)- @lenMaChungTu))AS float)) as MaxNumber
					from
					(
						select MaHoaDon
						from dbo.BH_HoaDon	hd						
						where MaHoaDon like @kihieuchungtu +'%'
						and NgayTao > @NgayLapHoaDon_Compare
						and exists (select * from @tblLoaiHD loai where hd.LoaiHoaDon= loai.LoaiHD)
					) a
					where ISNUMERIC(RIGHT(MaHoaDon,LEN(MaHoaDon)- @lenMaChungTu)) = 1
				)
				
			END
		
			-- do dai STT (toida = 10)
			if	@Return is null 
					select
						case when @lenMaChungTu = 2 then CONCAT(@kihieuchungtu, '00000000',1)
							when @lenMaChungTu = 3 then CONCAT(@kihieuchungtu, '0000000',1)
							when @lenMaChungTu = 4 then CONCAT(@kihieuchungtu, '000000',1)
							when @lenMaChungTu = 5 then CONCAT(@kihieuchungtu, '00000',1)
						else CONCAT(@kihieuchungtu,'000000',1)
						end as MaxCode
			else 
				begin
					set @ReturnVC = FORMAT(@Return + 1, 'F0');
					set @lenMaMax = len(@ReturnVC)
					select 
						case @lenMaMax
							when 1 then CONCAT(@kihieuchungtu,'000000000',@ReturnVC)
							when 2 then CONCAT(@kihieuchungtu,'00000000',@ReturnVC)
							when 3 then CONCAT(@kihieuchungtu,'0000000',@ReturnVC)
							when 4 then CONCAT(@kihieuchungtu,'000000',@ReturnVC)
							when 5 then CONCAT(@kihieuchungtu,'00000',@ReturnVC)
							when 6 then CONCAT(@kihieuchungtu,'0000',@ReturnVC)
							when 7 then CONCAT(@kihieuchungtu,'000',@ReturnVC)
							when 8 then CONCAT(@kihieuchungtu,'00',@ReturnVC)
							when 9 then CONCAT(@kihieuchungtu,'0',@ReturnVC)								
						else CONCAT(@kihieuchungtu,@ReturnVC) end as MaxCode						
				end 
		end
		
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
			qct.TienThu, qct.DiemThanhToan, qct.ID_TaiKhoanNganHang, qct.ID_KhoanThuChi, 
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
			nh.ChiPhiThanhToan,
			nh.MacDinh,
			nh.TheoPhanTram,
			nh.ThuPhiThanhToan
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
	order by hd.NgayLapHoaDon
END");

            Sql(@"ALTER PROCEDURE [dbo].[GetSoDuTheGiaTri_ofKhachHang]
    @ID_DoiTuong [uniqueidentifier],
    @DateTime [datetime]
AS
BEGIN
    SET NOCOUNT ON;
    	set @DateTime= DATEADD(DAY,1,@DateTime)

		----- get hoadon (nap, dieuchinh, hoanthe)
		select 
			hd.ID,
			hd.ID_DoiTuong,
			hd.ID_BaoHiem,
			hd.LoaiHoaDon,
			hd.PhaiThanhToan,
			hd.TongTienHang,
			hd.NgayLapHoaDon
		into #tblHD
		from dbo.BH_HoaDon hd
		where hd.ChoThanhToan = 0 
		and hd.ID_DoiTuong = @ID_DoiTuong	
		and hd.LoaiHoaDon in (22,23,32)
		and hd.NgayLapHoaDon  < @DateTime

		-----  sudungthe + trahang hoantien
		select 
			qhd.LoaiHoaDon,
			qct.ID,
			qct.ID_HoaDon,
			qct.TienThu,
			qct.HinhThucThanhToan,
			qct.ID_HoaDonLienQuan
		into #tblQCT
		from dbo.Quy_HoaDon_ChiTiet qct
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
		where qct.ID_DoiTuong= @ID_DoiTuong
		and qct.HinhThucThanhToan = 4
		and (qhd.TrangThai is null or qhd.TrangThai='1')
		and qhd.NgayLapHoaDon < @DateTime

    	select 
    		TongThuTheGiaTri,
			TraLaiSoDu,
			SuDungThe, 
			HoanTraTheGiatri,
    		ThucThu,
			PhaiThanhToan,
			SoDuTheGiaTri,
			TongDieuChinh,
    		iif(CongNoThe<0,0,CongNoThe) as CongNoThe
    	from
    	(
    	select 		
    		sum(TongThuTheGiaTri) - sum(TraLaiSoDu) as TongThuTheGiaTri, 
			sum(TongDieuChinh) as TongDieuChinh,
			sum(TraLaiSoDu) as TraLaiSoDu,
    		sum(SuDungThe) as SuDungThe,
    		sum(HoanTraTheGiatri) as HoanTraTheGiatri,
    		sum(ThucThu) as ThucThu,
    		sum(PhaiThanhToan) as PhaiThanhToan,
    		SUM(ThucThu) + sum(TongDieuChinh) - sum(TraLaiSoDu)  - SUM(SuDungThe) + SUM(HoanTraTheGiatri) as SoDuTheGiaTri, --- kangjin: soddu = tongthuthucte - sudung + hoantra
    		sum(PhaiThanhToan) - sum(TraLaiSoDu) - sum(ThucThu) as CongNoThe
    	from (
		---- dieuchinh sodu (tang/giam)					
    		SELECT 
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu,
				SUM(TongTienHang) as TongDieuChinh
    		FROM #tblHD hd
    		where LoaiHoaDon = 23

			union all
    		------ napthe
    		SELECT 
    			TongTienHang as TongThuTheGiaTri,
    			0 as SuDungThe,
    			0 as HoanTraTheGiatri,
    			0 as ThucThu,
    			hd.PhaiThanhToan, -- dieu chinh the (khong lien quan den cong no)
				0 as TraLaiSoDu,
				0 as TongDieuChinh
    		FROM #tblHD hd
    		where LoaiHoaDon = 22

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
    		FROM #tblHD hd
    		where hd.LoaiHoaDon= 32 
    		    
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
    		FROM #tblQCT qct    		
    		WHERE LoaiHoaDon = 11 
    	
	   	
    		union all
    		-- trahang: hoàn trả tiền vào TGT ---> tăng số dư
    		SELECT
    			0 as TongThuTheGiaTri,
    			0 as SuDungThe,
    			SUM(qct.TienThu) as HoanTraTheGiatri,
    			0 as ThucThu,
    			0 as PhaiThanhToan,
				0 as TraLaiSoDu,
				0 as TongDieuChinh
    		FROM #tblQCT qct    	
    		WHERE LoaiHoaDon = 12

    
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
    		from #tblHD hd
			join Quy_HoaDon_ChiTiet qct on hd.ID= qct.ID_HoaDonLienQuan
    		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID    		
    		where hd.LoaiHoaDon = 22 
			and qhd.NgayLapHoaDon < @DateTime
    		and (qhd.TrangThai= '1' or qhd.TrangThai  is  null)
    		
    		) tbl  
    		) tbl2
END");

            Sql(@"ALTER PROCEDURE [dbo].[HuyPhieuXuatKho_WhenUpdateTPDL]
    @ID_CTHoaDon [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    
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

		begin try
			exec UpdateTonLuyKeCTHD_whenUpdate @ID_HDHuy, @ID_DonViHuy, @NgayLapHoaDonHuy
		end try
		begin catch
		end catch
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
		max(table1.TonKho) as TonKho,		
		sum(sum(table1.SoLuong)) over ( order by NgayLapHoaDon ) as LuyKeTonKho,
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

            Sql(@"ALTER PROCEDURE [dbo].[ListHangHoaTheKhoTheoLoHang]
    @ID_HangHoa [uniqueidentifier],
    @IDChiNhanh [uniqueidentifier],
    @ID_LoHang [uniqueidentifier]
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
		max(table1.TonKho) as TonKho,
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
    	SELECT hd.ID as ID_HoaDon, 
		hd.MaHoaDon, 
		hd.LoaiHoaDon, 
		hd.YeuCau, 
		hd.ID_CheckIn, 
		hd.ID_DonVi, 
		bhct.ThanhTien * dvqd.TyLeChuyenDoi as ThanhTien,
		bhct.TienChietKhau * dvqd.TyLeChuyenDoi TienChietKhau, 
		CASE WHEN hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4' and hd.LoaiHoaDon = 10 THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon,    		
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.TonLuyKe, bhct.TonLuyKe_NhanChuyenHang) as TonKho,
		iif(hd.ID_DonVi = @IDChiNhanh, bhct.GiaVon / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi),bhct.GiaVon_NhanChuyenHang / iif(dvqd.TyLeChuyenDoi=0,1,dvqd.TyLeChuyenDoi)) as GiaVon,	
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
    	WHERE bhct.ID_LoHang = @ID_LoHang 
		and hh.ID = @ID_HangHoa 
		and hd.ChoThanhToan = 0 
		and hd.LoaiHoaDon in (6,7,35,37,38,39,40,8,4,7,9,10,13,14,18) 			
		and (bhct.ChatLieu is null or bhct.ChatLieu not in ('2','5') )		
		and ((hd.ID_DonVi = @IDChiNhanh 
		and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null)) or (hd.ID_CheckIn = @IDChiNhanh and hd.YeuCau = '4'))
	) as table1
    group by ID_HoaDon, MaHoaDon, NgayLapHoaDon,LoaiHoaDon, ID_DonVi, ID_CheckIn
	ORDER BY NgayLapHoaDon desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[LoadDanhMucHangHoa]
   @IDChiNhanh uniqueidentifier ='d93b17ea-89b9-4ecf-b242-d03b8cde71de',
   @TextSearch nvarchar(max) ='',
   @IDThuocTinhHangs nvarchar(max)='', 
   @TrangThaiKho int=0, 
   @Where nvarchar(max) ='',
   @CurrentPage int = 0,
   @PageSize int = 1000,
   @ColumnSort varchar(100) ='NgayTao',
   @SortBy varchar(20) = 'DESC'
AS
BEGIN
	SET NOCOUNT ON;

	declare @where1 nvarchar(max), @where2 nvarchar(max), 
		@paramDefined nvarchar(max),
    	@sql1 nvarchar(max) ='', @sql2 nvarchar(max) =''
    	declare @tblDefined nvarchar(max) = concat(N' declare @tblThuocTinh table (ID uniqueidentifier) ',	
    											   N' declare @tblSearch table (Name nvarchar(max)) ')


	set @where1 =' where 1 = 1 and qd.LaDonViChuan = 1 '
    set @where2 =' where 1 = 1'
	
	
	if isnull(@Where,'')!=''
		set @Where = CONCAT(' and ',N'', @Where)

	if isnull(@ColumnSort,'')=''
		set @ColumnSort = 'NgayTao'
	if isnull(@SortBy,'')=''
		set @SortBy = 'DESC'
    
	if isnull(@TextSearch,'')!=''
		begin
			set @sql1 = concat(@sql1,
				N'DECLARE @count int;
				INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch_In, '' '') where Name!='''';
				Select @count =  (Select count(*) from @tblSearch) ')

			set @where1 = CONCAT(@where1, N' and
						((select count(*) from @tblSearch b where 
    								hh.TenHangHoa_KhongDau like ''%''+b.Name+''%''
    								or hh.TenHangHoa_KyTuDau like ''%''+b.Name+''%'' 
									or hh.TenHangHoa like ''%''+b.Name+''%''
									or hh.GhiChu like ''%'' +b.Name +''%'' 
    								or qd.MaHangHoa like ''%''+b.Name+''%'' )=@count or @count=0)')

		end

	if isnull(@IDThuocTinhHangs,'')!=''
		begin
			set @sql1 = concat(@sql1, N' insert into @tblThuocTinh select name from dbo.splitstring(@IDThuocTinhHangs_In) where Name!='' ''')		
			set @where1 = CONCAT(@where1, N' and exists
									(select * 
									from HangHoa_ThuocTinh tt 
									where hh.ID = tt.ID_HangHoa 
									and exists (select ID from @tblThuocTinh prop where tt.ID = prop.ID) 
									)')
		end
	
	if isnull(@TrangThaiKho,0)!=0
		begin			
			if @TrangThaiKho in (1,5,6)
				set @where2 = CONCAT(@where2, N' and tblOut.TrangThai_TonKho = @TrangThaiKho_In ') 
			if @TrangThaiKho in (3,4)
				set @where2 = CONCAT(@where2, N' and tblOut.TrangThai_DinhMucTon = @TrangThaiKho_In ') 
		end
	
	set @sql2= concat( N'
;with data_cte
 as
 (
	select *,
		tblOut.TheoDoi as TrangThai,
		case tblOut.LoaiHangHoa 
			when 1 then N''Hàng hóa''
			when 2 then N''Dịch vụ''
			when 3 then N''Combo''
		end as sLoaiHangHoa
	from
	(
	select tbl.*,
		cast(tbl.LaChaCungLoai1 as bit) LaChaCungLoai,
		tbl.TenDonViTinh as DonViTinhChuan,
		nhomDV.ID_NhomHangHoa as ID_NhomHoTro,
		nhomHT.TenNhomHangHoa as TenNhomHoTro,	
		iif(tbl.CountCungLoai = 1 or QuanLyTheoLoHang =''1'', tbl.MaHangHoa1, concat(''('',tbl.CountCungLoai, N'') Mã hàng'')) as MaHangHoa,
		iif(tbl.LoaiHangHoa=1, tbl.GiaVon1, dbo.GetGiaVonOfDichVu(@IDChiNhanh_In,tbl.ID_DonViQuiDoi)) as GiaVon	,
		------0.all
		----- 1.tonkho > 0
		----- 2.tonkho <=0 (bo qua cai nay)
		----- 3.Dưới định mức tồn
		----- 4.Vượt định mức tồn
		----- 5.Hàng âm kho
		----- 6.TonKho = 0
		case
			when tbl.TonKho > 0 then 1
			when tbl.TonKho < 0 then 5
		else 6 end as TrangThai_TonKho,
		case
			when tbl.TonKho < tbl.TonToiThieu then 3
			when tbl.TonKho > tbl.TonToiDa then 4
		else 0 end as TrangThai_DinhMucTon
	from
	(
			select 			
				max(tblGr.ID) as ID,		
				max(tblGr.ID_DonViQuiDoi) as ID_DonViQuiDoi,
				max(tblGr.LaChaCungLoai) as LaChaCungLoai1,		
				max(tblGr.MaHangHoa) as MaHangHoa1,
				max(tblGr.NgayTao) as NgayTao,
			
				max(tblGr.TenDonViTinh) as TenDonViTinh,
				max(tblGr.ThuocTinhGiaTri) as ThuocTinhGiaTri,			
				count(tblGr.ID_HangHoaCungLoai) as CountCungLoai,

				tblGr.TenHangHoa,
				tblGr.TenHangHoa_KhongDau,
				tblGr.LaHangHoa,
				tblGr.GhiChu,			
				tblGr.DuocBanTrucTiep,
				tblGr.TheoDoi,
				tblGr.Xoa,
				tblGr.QuanLyTheoLoHang,
				tblGr.TrangThaiKinhDoanh,
				tblGr.TrangThaiHang,
				tblGr.ID_Xe,
			
				tblGr.ID_HangHoaCungLoai,
				tblGr.ID_NhomHangHoa,
				tblGr.NhomHangHoa,
			
				tblGr.LoaiHangHoa,
				tblGr.SoPhutThucHien,
				tblGr.DichVuTheoGio,
				tblGr.ChietKhauMD_NV,
				tblGr.ChietKhauMD_NVTheoPT,
				tblGr.DuocTichDiem,
				tblGr.QuanLyBaoDuong,
				tblGr.LoaiBaoDuong,
				tblGr.SoKmBaoHanh,
				tblGr.HoaHongTruocChietKhau,
				tblGr.TonToiDa,
				tblGr.TonToiThieu,				
				tblGr.GiaBan,
				
				max(tblGr.GiaVon) as GiaVon1,
				sum(tblGr.TonKho) as TonKho		
			from
			(
				select 
					hh.ID,				
					hh.ID_Xe,
					qd.ID as ID_DonViQuiDoi,
					hh.TenHangHoa,
					hh.TenHangHoa_KhongDau,
					qd.MaHangHoa,
					hh.LaHangHoa,
					hh.GhiChu,				
					cast(hh.LaChaCungLoai as int) LaChaCungLoai,
					hh.DuocBanTrucTiep,
					hh.TheoDoi,
					hh.NgayTao,
					hh.ID_HangHoaCungLoai,
					hh.ID_NhomHang as ID_NhomHangHoa,
					nhom.TenNhomHangHoa as NhomHangHoa,
			
					iif(hh.LoaiHangHoa is null,iif(hh.LaHangHoa=''1'',1,2), hh.LoaiHangHoa) as LoaiHangHoa,
					isnull(hh.SoPhutThucHien,0) as SoPhutThucHien,
					isnull(hh.DichVuTheoGio,0) as DichVuTheoGio,	
					isnull(hh.ChietKhauMD_NV,0) as ChietKhauMD_NV,
					isnull(hh.ChietKhauMD_NVTheoPT,''1'') as ChietKhauMD_NVTheoPT,
					isnull(hh.DuocTichDiem,0) as DuocTichDiem,
					iif(hh.QuanLyTheoLoHang is null,''0'', hh.QuanLyTheoLoHang) as QuanLyTheoLoHang,
					iif(hh.QuanLyBaoDuong is null,0, hh.QuanLyBaoDuong) as QuanLyBaoDuong,
					iif(hh.LoaiBaoDuong is null,0, hh.LoaiBaoDuong) as LoaiBaoDuong,
					iif(hh.SoKmBaoHanh is null,0, hh.SoKmBaoHanh) as SoKmBaoHanh,
					iif(hh.HoaHongTruocChietKhau is null,0, hh.HoaHongTruocChietKhau) as HoaHongTruocChietKhau,		
					isnull(hh.TonToiDa,0) as TonToiDa,
					isnull(hh.TonToiThieu,0) as TonToiThieu,
				
					qd.GiaBan,
					qd.Xoa,
					qd.TenDonViTinh,
					qd.ThuocTinhGiaTri,
					iif(hh.TheoDoi=''1'',1,2) as TrangThaiKinhDoanh, ----- 0.all, 1.dangkinhdoanh, 2.ngungkinhdoanh
					iif(qd.Xoa=''1'',1,0) as TrangThaiHang,
					ISNULL(tk.TonKho,0) as TonKho,
					isnull(gv.GiaVon,0) as GiaVon ',						
					
				N' from DM_HangHoa hh 	
				left join DonViQuiDoi qd on qd.ID_HangHoa= hh.ID
				left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID		
				left join DM_HangHoa_TonKho tk on qd.ID = tk.ID_DonViQuyDoi and tk.ID_DonVi= @IDChiNhanh_In
				left join DM_GiaVon gv on qd.ID = gv.ID_DonViQuiDoi and gv.ID_DonVi= @IDChiNhanh_In	
				
	', @where1,		
	 N'  ) tblGr
		 group by tblGr.TenHangHoa, ------ hangcungloai: chi lay 1 dong
			tblGr.TenHangHoa_KhongDau,
			tblGr.LaHangHoa,
			tblGr.ID_Xe,
			tblGr.GhiChu,			
			tblGr.DuocBanTrucTiep,
			tblGr.Xoa,
			tblGr.QuanLyTheoLoHang,
			tblGr.TrangThaiKinhDoanh,
			tblGr.TrangThaiHang,
			tblGr.TheoDoi,
			tblGr.ID_HangHoaCungLoai,
			tblGr.ID_NhomHangHoa,
			tblGr.NhomHangHoa,
			tblGr.LoaiHangHoa,
			tblGr.SoPhutThucHien,
			tblGr.DichVuTheoGio,
			tblGr.ChietKhauMD_NV,
			tblGr.ChietKhauMD_NVTheoPT,
			tblGr.DuocTichDiem,
			tblGr.QuanLyBaoDuong,
			tblGr.LoaiBaoDuong,
			tblGr.SoKmBaoHanh,
			tblGr.HoaHongTruocChietKhau,
			tblGr.TonToiDa,
			tblGr.TonToiThieu,
			tblGr.GiaBan
		) tbl		
		left join 
		(
			select distinct Id_NhomHang, Id_DonViQuiDoi
			from NhomHang_ChiTietSanPhamHoTro
			where LaSanPhamNgayThuoc = 2
		) spht on tbl.ID_DonViQuiDoi= spht.Id_DonViQuiDoi 
		left join NhomHangHoa_DonVi nhomDV on spht.Id_NhomHang= nhomDV.ID_NhomHangHoa and nhomDV.ID_DonVi = @IDChiNhanh_In
		left join DM_NhomHangHoa nhomHT on spht.Id_NhomHang= nhomHT.ID
	) tblOut ', @where2, @Where,
	N'),
count_cte
as
(
	select COUNT(ID) as TotalRow,
		ceiling(COUNT(ID)/ cast (@PageSize_In as float)) as TotalPage,
		sum(TonKho) as SumTonKho
	from data_cte
)

select *
from data_cte dt
cross join count_cte
order by 
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''NgayTao'' then NgayTao end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''NgayTao'' then NgayTao end DESC,
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''MaHangHoa'' then MaHangHoa1 end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''MaHangHoa'' then MaHangHoa1 end DESC,
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''TenHangHoa'' then TenHangHoa end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''TenHangHoa'' then TenHangHoa end DESC,
	case when @SortBy_In <> ''ASC'' then ''''
	when @ColumnSort_In=''TenNhomHang'' then NhomHangHoa end ASC,
	case when @SortBy_In <> ''DESC'' then ''''
	when @ColumnSort_In=''TenNhomHang'' then NhomHangHoa end DESC,
	case when @SortBy_In <> ''ASC'' then 0
	when @ColumnSort_In=''GiaBan'' then GiaBan end ASC,
	case when @SortBy_In <> ''DESC'' then 0
	when @ColumnSort_In=''GiaBan'' then GiaBan end DESC,
	case when @SortBy_In <> ''ASC'' then 0
	when @ColumnSort_In=''GiaVon'' then GiaVon1 end ASC,
	case when @SortBy_In <> ''DESC'' then 0
	when @ColumnSort_In=''GiaVon'' then GiaVon1 end DESC,
	case when @SortBy_In <> ''ASC'' then 0
	when @ColumnSort_In=''TonKho'' then TonKho end ASC,
	case when @SortBy_In <> ''DESC'' then 0
	when @ColumnSort_In=''TonKho'' then TonKho end DESC
	OFFSET (@CurrentPage_In* @PageSize_In) ROWS
	FETCH NEXT @PageSize_In ROWS ONLY

'
)
	

	set @paramDefined = N' @IDChiNhanh_In uniqueidentifier,
    								@TextSearch_In nvarchar(max),
    								@IDThuocTinhHangs_In nvarchar(max),
    								@TrangThaiKho_In int,    							
    								@Where_In nvarchar(max),		
    								@CurrentPage_In int,
    								@PageSize_In int,
									@ColumnSort_In varchar(100),
    								@SortBy_In varchar(20)'

	set @sql2 = CONCAT(@tblDefined, @sql1, @sql2)
    
	----print @sql2

	exec sp_executesql @sql2, 
    		@paramDefined,
    		@IDChiNhanh_In = @IDChiNhanh,
    		@TextSearch_In = @TextSearch,
    		@IDThuocTinhHangs_In = @IDThuocTinhHangs,
    		@TrangThaiKho_In = @TrangThaiKho,   	
    		@Where_In = @Where, 	
    		@CurrentPage_In = @CurrentPage,
    		@PageSize_In = @PageSize,
			@ColumnSort_In = @ColumnSort,
			@SortBy_In = @SortBy

			
			

END");

            Sql(@"ALTER PROCEDURE [dbo].[NhomHang_GetListSanPhamHoTro]
    @ID_NhomHang [uniqueidentifier]
AS
BEGIN
    SET NOCOUNT ON;
    	
    	select 
			nhomsp.ID,
    		nhomsp.Id_DonViQuiDoi,
    		nhomsp.Id_LoHang,
    		nhomsp.Id_NhomHang, ---- thuộc nhóm hỗ trợ
    		nhomsp.SoLuong,
    		nhomsp.LaSanPhamNgayThuoc,
    		hh.TenHangHoa,
    		qd.MaHangHoa,
    		qd.TenDonViTinh,
    		lo.MaLoHang,
			hh.ID_NhomHang as ID_NhomHangHoa,---- thuộc nhóm hàng hóa 
			nhom.TenNhomHangHoa,
    		iif(hh.LoaiHangHoa is null, iif(hh.LaHangHoa='1',1,2), hh.LoaiHangHoa) as LoaiHangHoa
    	from (
    		select *
    		from NhomHang_ChiTietSanPhamHoTro 
    		where Id_NhomHang= @ID_NhomHang
    	) nhomsp	
    	join DonViQuiDoi qd on nhomsp.Id_DonViQuiDoi = qd.ID
    	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
    	left join DM_LoHang lo on nhomsp.Id_LoHang = lo.ID
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
    		MaKhachHang nvarchar(max), TenKhachHang nvarchar(max), DienThoaiKH nvarchar(max), TongAll float)
    
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
    		select dt.*, cte.*
    		from data_cte dt
    		cross join count_cte cte
    		order by dt.NgayLapHoaDon desc
    		OFFSET (@CurrentPage* @PageSize) ROWS
    		FETCH NEXT @PageSize ROWS ONLY
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
    				ISNULL(tblView.DienThoai,'') as DienThoaiKhachHang,
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

            Sql(@"ALTER PROCEDURE [dbo].[Search_DMHangHoa_TonKho]
    @MaHH [nvarchar](max),
    @MaHH_TV [nvarchar](max),
    @ID_ChiNhanh [uniqueidentifier],
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
SET NOCOUNT ON;
    DECLARE @XemGiaVon as nvarchar
    Set @XemGiaVon = (Select top 1
    						Case when nd.LaAdmin = '1' then '1' else
    						Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    						From
    						HT_NguoiDung nd	
    						where nd.ID = @ID_NguoiDung)
		DECLARE @tablename TABLE(Name [nvarchar](max))	 
    	DECLARE @tablenameChar TABLE(  Name [nvarchar](max))
  
    	DECLARE @count int
    	DECLARE @countChar int
    	INSERT INTO @tablename(Name) select  Name from [dbo].[splitstring](@MaHH+',') where Name!='';
    	INSERT INTO @tablenameChar(Name) select  Name from [dbo].[splitstring](@MaHH_TV+',') where Name!='';
    	Select @count =  (Select count(*) from @tablename);
    	Select @countChar =   (Select count(*) from @tablenameChar);

select qd.ID as ID_DonViQuiDoi,
		MaHangHoa, TenHangHoa, TenHangHoa_KhongDau, TenHangHoa_KyTuDau,TenDonViTinh, ThuocTinhGiaTri as ThuocTinh_GiaTri,
		CONCAT(TenHangHoa,' ', ThuocTinhGiaTri,' ', case when TenDonViTinh='' or TenDonViTinh is null then '' else ' (' + TenDonViTinh + ')' end) as TenHangHoaFull,
		ISNULL(tk.TonKho,0) as TonCuoiKy,
		CAST(ROUND((qd.GiaBan), 0) as float) as GiaBan,		
		case when @XemGiaVon= '1' then CAST(ROUND((ISNULL(gv.GiaVon,0)), 0) as float) else 0 end as GiaVon,
		case when @XemGiaVon= '1' then	
			case when hh.LaHangHoa='1' then CAST(ROUND((ISNULL(gv.GiaVon,0)), 0) as float)
			else CAST(ROUND((ISNULL(tblDVu.GiaVon,0)), 0) as float) end
		else 0 end as GiaVon,
		gv.ID_DonVi, hh.LaHangHoa
	from DonViQuiDoi qd 
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	left join DM_HangHoa_TonKho tk on qd.ID= tk.ID_DonViQuyDoi 	and ((tk.ID_DonVi = @ID_ChiNhanh and hh.LaHangHoa='1') or hh.LaHangHoa=0)
	left join DM_GiaVon gv on qd.id= gv.ID_DonViQuiDoi and ((gv.ID_DonVi= @ID_ChiNhanh) or gv.ID_DonVi is null )
	left join (select qd2.ID,sum(dl.SoLuong *  ISNULL(gv.GiaVon,0)) as GiaVon
				from DonViQuiDoi qd2
				join DinhLuongDichVu dl on qd2.ID= dl.ID_DichVu
				left join DM_GiaVon gv on dl.ID_DonViQuiDoi= gv.ID_DonViQuiDoi
				where gv.ID_DonVi=@ID_ChiNhanh 
				group by qd2.ID
				) tblDVu on qd.ID= tblDVu.ID
	where qd.Xoa= 0 and hh.TheoDoi=1	
	and	(
		(select count(*) from @tablename b where 
    		qd.MaHangHoa like '%'+b.Name+'%' 
			or hh.TenHangHoa like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 			    		
			)=@count or @count=0
			)  
	and	(
		(select count(*) from @tablenameChar b where 
    		qd.MaHangHoa like '%'+b.Name+'%' 
			or hh.TenHangHoa like '%'+b.Name+'%' 
    		or hh.TenHangHoa_KhongDau like '%'+b.Name+'%' 			    		
			)=@countChar or @countChar=0
			) 
	order by tk.TonKho desc
END");

            Sql(@"ALTER PROCEDURE [dbo].[SearchHangHoa_withGiaVonTieuChuan]
    @ID_DonVi [uniqueidentifier],
    @TextSearch [nvarchar](max),
    @DateTo [datetime],
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;
    
    	declare @txtSeachUnsign nvarchar(max) = (select dbo.FUNC_ConvertStringToUnsign(@TextSearch));    
    	set @TextSearch = CONCAT('%',@TextSearch,'%')
    
    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString)
    
    
    	select 
    		hh.ID,
    		qd.ID as ID_DonViQuiDoi,
    		lo.ID as ID_LoHang,
    		qd.MaHangHoa,
    		hh.TenHangHoa, 
    		isnull(qd.TenDonViTinh,'') as TenDonViTinh, 
    		qd.ThuocTinhGiaTri as ThuocTinh_GiaTri,
    		hh.QuanLyTheoLoHang,
    		hh.LaHangHoa,
    		lo.MaLoHang,
    		lo.NgaySanXuat,
    		lo.NgayHetHan,
    		isnull(tk.TonKho,0) as TonKho, --- tonkho hientai
    		isnull(gv.GiaVon,0) as GiaVon -- giavon hientai
    	from DonViQuiDoi qd 
    	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
    	left join DM_LoHang lo on hh.ID= lo.ID_HangHoa	
    	left join DM_HangHoa_TonKho tk on qd.ID= tk.ID_DonViQuyDoi and (lo.ID= tk.ID_LoHang or hh.QuanLyTheoLoHang = 0) and tk.ID_DonVi= @ID_DonVi
    	left join DM_GiaVon gv on qd.ID= gv.ID_DonViQuiDoi and (lo.ID= gv.ID_LoHang or hh.QuanLyTheoLoHang = 0) and gv.ID_DonVi= @ID_DonVi
    	where (select count(Name) from @tblSearchString b where     			
    					hh.TenHangHoa like '%'+b.Name+'%'
    					or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    					or qd.MaHangHoa like '%'+b.Name+'%'		
    					or lo.MaLoHang like '%'+b.Name+'%'		
    					)=@count or @count=0
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
		-- get hoadon all chinhanh

		select *
		from
		(
		select *
		from
		(
			select 
			a.ID, a.MaHoaDon, a.NgayLapHoaDon,	a.LoaiHoaDon,
			a.TongThanhToan,
			a.TongTienThue,
			TinhChietKhauTheo,
			iif(@LoaiDoiTuong=3, a.PhaiThanhToanBaoHiem,a.PhaiThanhToan) - ISNULL(b.TongTra,0) -  isnull(tattoanTGT.GiaTriTatToan,0) as PhaiThanhToan
			from
			(
			select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.LoaiHoaDon,
				hd.TongTienThue,
				hd.TongThanhToan,
    			ISNULL(hd.PhaiThanhToan,0) - ISNULL(hdt.PhaiThanhToan,0) as PhaiThanhToan,
				ISNULL(hd.PhaiThanhToanBaoHiem,0) - ISNULL(hdt.PhaiThanhToanBaoHiem,0) as PhaiThanhToanBaoHiem,
    			ISNULL(TinhChietKhauTheo,1) as TinhChietKhauTheo
    		from BH_HoaDon hd
    		left join BH_HoaDon hdt on hd.ID_HoaDon= hdt.ID and hdt.LoaiHoaDon= 6
    		left join 
    				(select ID_HoaDon, min(TinhChietKhauTheo) as TinhChietKhauTheo
    				from BH_NhanVienThucHien nvth
    				where nvth.ID_HoaDon is not null
    				group by ID_HoaDon) tblNV on hd.ID = tblNV.ID_HoaDon
    		where 
			exists (select Name from dbo.splitstring(@ID_DonVi) where Name= hd.ID_DonVi)
			and iif(@LoaiDoiTuong=3, hd.ID_BaoHiem, hd.ID_DoiTuong) like @ID_DoiTuong		
    		and hd.LoaiHoaDon in (1,19,4,22, 25)
    		and hd.ChoThanhToan='0' 
			) a
			left join
			(
				-- get hoadon trahang of khachhang
				select
					hd.ID_HoaDon, hd.TongThanhToan as TongTra
				from BH_HoaDon hd
				where hd.ID_DoiTuong like @ID_DoiTuong
				and hd.ID_HoaDon is not null
				and hd.LoaiHoaDon= 6		
				and hd.ChoThanhToan='0'
			) b on a.ID= b.ID_HoaDon
			left join
			(
				-- get hoadon trahang of khachhang
				select
					hd.ID_HoaDon, hd.TongThanhToan as GiaTriTatToan
				from BH_HoaDon hd
				where hd.ID_DoiTuong like @ID_DoiTuong
				and hd.ID_HoaDon is not null				
				and hd.ChoThanhToan='0'
			) tattoanTGT on a.ID= tattoanTGT.ID_HoaDon
		) tbl
		

		union all

		select 
			cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon,hd.LoaiHoaDon,
			sum(cp.ThanhTien) as TongThanhToan,
			0 as TongTienThue,
			0 as TinhChietKhauTheo,
			sum(cp.ThanhTien) as PhaiThanhToan
		from BH_HoaDon_ChiPhi cp
		join BH_HoaDon hd on cp.ID_HoaDon = hd.ID
		where hd.ChoThanhToan= 0
		and cp.ID_NhaCungCap= @ID_DoiTuong
		group by cp.ID_HoaDon, hd.MaHoaDon, hd.NgayLapHoaDon,	hd.LoaiHoaDon
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
				when 14 then - ISNULL(hd.TongThanhToan,0) ---- nhaphang khachthua
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
			and not exists (select id from BH_HoaDon pthh where qct.ID_HoaDonLienQuan = pthh.ID and pthh.LoaiHoaDon= 41) --- khong lay phieuchi hoahong nguoi GT
			and qct.HinhThucThanhToan !=6			
			and (qct.LoaiThanhToan is null or qct.LoaiThanhToan !=3) ---- LoaiThnahToan = 3. thu/chi khong lienquan congno
			and (qhd.TrangThai is null or qhd.TrangThai='1') -- van phai lay phieu thu tu the --> trừ cong no KH
			group by qhd.ID, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon,dt.LoaiDoiTuong,qhd.PhieuDieuChinhCongNo
		) a where a.GiaTri != 0 -- khong lay phieudieuchinh diem

	
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetInforKhachHang_ByID]
    @ID_DoiTuong uniqueidentifier,
    @ID_ChiNhanh [nvarchar](max),
    @timeStart [nvarchar](max),
    @timeEnd [nvarchar](max)
AS
BEGIN
	SET NOCOUNT ON;
	declare @LoaiDoiTuong int
	select @LoaiDoiTuong= LoaiDoiTuong from DM_DoiTuong where ID = @ID_DoiTuong

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
    							iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) as ID_DoiTuong,
    							0 AS GiaTriTra,    							
								iif(@LoaiDoiTuong=1,bhd.PhaiThanhToan,isnull(bhd.PhaiThanhToanBaoHiem,0)) as DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi,
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM BH_HoaDon bhd
    						WHERE bhd.LoaiHoaDon in (1,7,19,25) 
							AND bhd.ChoThanhToan = 0 
							and iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) = @ID_DoiTuong


							union all

							SELECT 
    							iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) as ID_DoiTuong,
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
							and iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) = @ID_DoiTuong

    						
    						 union all
    							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,    							
								iif(@LoaiDoiTuong=1,bhd.PhaiThanhToan,0)  AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								0 as ThuTuThe,
								0 as HoanTraSoDuTGT
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) 
							AND bhd.ChoThanhToan = 0 
							and iif(@LoaiDoiTuong=1,bhd.ID_DoiTuong,bhd.ID_BaoHiem) = @ID_DoiTuong						
    							
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
			TongTienHang FLOAT, TongChiPhi FLOAT,
    	ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER, 
    	ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    	INSERT INTO @cthd_NeedUpGiaVon  	
		select hd.ID as IDHoaDon,
			hd.ID_HoaDon as IDHoaDonGoc, 
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
    
    			---- BEGIN get giavon dauky by id_quidoi =========
    		DECLARE @TinhGiaVonTrungBinh BIT;
    		SELECT @TinhGiaVonTrungBinh = GiaVonTrungBinh FROM HT_CauHinhPhanMem WHERE ID_DonVi = @IDChiNhanh;
    		IF(@TinhGiaVonTrungBinh IS NOT NULL AND @TinhGiaVonTrungBinh = 1)
    		BEGIN
    		
    		DECLARE @ChiTietHoaDonGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT,
			TongTienHang FLOAT, TongChiPhi float,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX));
    		INSERT INTO @ChiTietHoaDonGiaVon
    		select
    				hd.ID, hd.ID_HoaDon, hd.MaHoaDon, hd.LoaiHoaDon, hdct.ID, hd.NgayLapHoaDon, hdct.SoThuTu,
					iif(hdct.ChatLieu='5',0, hdct.SoLuong) as SoLuong, 
					hdct.DonGia, 
					hd.TongTienHang, 
					isnull(hd.TongChiPhi,0) as TongChiPhi,
    				iif(hdct.ChatLieu='5',0,hdct.TienChietKhau) as TienChietKhau, 
					hdct.ThanhTien, hd.TongGiamGia, 
    				dvqd.TyLeChuyenDoi, 0, hdct.GiaVon / dvqd.TyLeChuyenDoi as GiaVon, 
    			hdct.GiaVon_NhanChuyenHang / dvqd.TyLeChuyenDoi  as GiaVonNhan, 
    			hh.ID, hh.LaHangHoa, hdct.ID_DonViQuiDoi, hdct.ID_LoHang, hdct.ID_ChiTietDinhLuong, 
    				@IDChiNhanh, hd.ID_CheckIn, hd.YeuCau
			FROM BH_HoaDon hd
    		INNER JOIN BH_HoaDon_ChiTiet hdct ON hd.ID = hdct.ID_HoaDon    	
    		INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID    		
    		INNER JOIN DM_HangHoa hh on hh.ID = dvqd.ID_HangHoa    		
    		INNER JOIN @tblCTHD cthdthemmoi ON cthdthemmoi.ID_HangHoa = hh.ID    		
    		WHERE hd.ChoThanhToan = 0 
				and hd.LoaiHoaDon NOT IN (3, 19, 25,29)
    				AND (hdct.ID_LoHang = cthdthemmoi.ID_LoHang OR cthdthemmoi.ID_LoHang IS NULL) 
    				AND
    				((hd.ID_DonVi = @IDChiNhanh and hd.NgayLapHoaDon < @NgayLapHDMin and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
    					or (hd.YeuCau = '4'  and hd.ID_CheckIn = @IDChiNhanh and hd.NgaySua < @NgayLapHDMin))
    	   			

		DECLARE @ChiTietHoaDonGiaVon1 TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT,
			TongTienHang FLOAT, TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,  
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
			
			INSERT INTO @ChiTietHoaDonGiaVon1
			SELECT * FROM (SELECT *, ROW_NUMBER() OVER (PARTITION BY ID_HangHoa, ID_LoHang ORDER BY NgayLapHoaDon DESC) AS RN 
    					FROM @ChiTietHoaDonGiaVon) AS cthdGiaVon1 WHERE cthdGiaVon1.RN = 1;
			---- === END giavon dauky ========= ---

    		------ union 2 table: ctEdit + ctNeddUpdateGV: partition by idhang,idlohang order by NgayLapHoaDon
    		DECLARE @BangUpdateGiaVon TABLE (IDHoaDon UNIQUEIDENTIFIER, IDHoaDonBan UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), LoaiHoaDon INT, ID_ChiTietHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, SoThuThu INT, SoLuong FLOAT, DonGia FLOAT,
			TongTienHang FLOAT, TongChiPhi FLOAT,
    		ChietKhau FLOAT, ThanhTien FLOAT, TongGiamGia FLOAT, TyLeChuyenDoi FLOAT, TonKho FLOAT, GiaVon FLOAT, GiaVonNhan FLOAT, ID_HangHoa UNIQUEIDENTIFIER, LaHangHoa BIT, IDDonViQuiDoi UNIQUEIDENTIFIER, ID_LoHang UNIQUEIDENTIFIER, ID_ChiTietDinhLuong UNIQUEIDENTIFIER,
    		ID_ChiNhanhThemMoi UNIQUEIDENTIFIER, ID_CheckIn UNIQUEIDENTIFIER, YeuCau NVARCHAR(MAX), RN INT);
    		INSERT INTO @BangUpdateGiaVon
    		SELECT *, 
    				ROW_NUMBER() OVER (PARTITION BY tableUpdateGiaVon.ID_HangHoa, tableUpdateGiaVon.ID_LoHang ORDER BY tableUpdateGiaVon.NgayLapHoaDon) AS RN 
    			FROM
    			(
					SELECT * 
    					FROM @cthd_NeedUpGiaVon
    				UNION ALL
    					SELECT 
    						gvDauKy.IDHoaDon, gvDauKy.IDHoaDonBan, gvDauKy.MaHoaDon, gvDauKy.LoaiHoaDon, gvDauKy.ID_ChiTietHoaDon, gvDauKy.NgayLapHoaDon,
    						gvDauKy.SoThuThu, gvDauKy.SoLuong, gvDauKy.DonGia, gvDauKy.TongTienHang, gvDauKy.TongChiPhi,
    						gvDauKy.ChietKhau, gvDauKy.ThanhTien, gvDauKy.TongGiamGia, gvDauKy.TyLeChuyenDoi, gvDauKy.TonKho, 
    						CASE WHEN gvDauKy.GiaVon IS NULL THEN 0 ELSE gvDauKy.GiaVon END AS GiaVon, 											
    						CASE WHEN gvDauKy.GiaVonNhan IS NULL THEN 0 ELSE gvDauKy.GiaVonNhan END AS GiaVonNhan,								
    						cthd1.ID_HangHoa, gvDauKy.LaHangHoa, gvDauKy.IDDonViQuiDoi, cthd1.ID_LoHang , gvDauKy.ID_ChiTietDinhLuong,
    					@IDChiNhanh as ID_ChiNhanh, gvDauKy.ID_CheckIn, gvDauKy.YeuCau 
    					FROM @tblCTHD cthd1
    					LEFT JOIN @ChiTietHoaDonGiaVon1 AS gvDauKy ON cthd1.ID_HangHoa = gvDauKy.ID_HangHoa   					   				
    					AND (cthd1.ID_LoHang = gvDauKy.ID_LoHang OR gvDauKy.ID_LoHang IS NULL)				
			 ) tableUpdateGiaVon
    
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
    			
    			
    
    			--DECLARE @RNCheck INT;
    			--SELECT @RNCheck = MAX(RN) FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang
    			--IF(@RNCheck = 1)
    			--BEGIN
    			--	UPDATE @BangUpdateGiaVon SET RN = 2
    			--END

			update gv set gv.RN = 2
				 from @BangUpdateGiaVon gv
				 join 
						(select  MAX(RN) as RN, ID_HangHoa, ID_LoHang FROM @BangUpdateGiaVon GROUP BY ID_HangHoa, ID_LoHang 
				) tbl on gv.ID_HangHoa = tbl.ID_HangHoa and (gv.ID_LoHang = tbl.ID_LoHang or gv.ID_LoHang is null and tbl.ID_LoHang is null)
				 where tbl.RN= 1
    
    			---- update GiaVon, GiaVonNhan to @BangUpdateGiaVon if Loai =(4,10,13), else keep old
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
			TongTienHang FLOAT, TongChiPhi FLOAT,
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
    		DECLARE @TongTienHang float, @TongChiPhi float;
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
    			SELECT IDHoaDon, IDHoaDonBan, IDHoaDonCu, MaHoaDon, LoaiHoaDon, IDChiTietHoaDon, SoLuong, DonGia, TongTienHang, TongChiPhi,
				ChietKhau,ThanhTien, TongGiamGia, TyLeChuyenDoi, TonKho,
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
    					SELECT @TongTienHangDemo = SUM(bhctdm.SoLuong * (bhctdm.DonGia -  bhctdm.ChietKhau)), @SoLuongDemo = SUM(bhctdm.SoLuong * dvqddm.TyLeChuyenDoi) 
    					FROM @GiaVonCapNhat bhctdm
    					left Join DonViQuiDoi dvqddm on bhctdm.IDDonViQuiDoi = dvqddm.ID
    					WHERE bhctdm.IDHoaDon = @IDHoaDon AND dvqddm.ID_HangHoa = @IDHangHoa AND (bhctdm.IDLoHang = @IDLoHang or @IDLoHang is null)
    					GROUP BY dvqddm.ID_HangHoa, bhctdm.IDLoHang
        					
    					IF(@SoLuongDemo + @TonKho > 0 AND @TonKho > 0)
    					BEGIN
    						IF(@TongTienHang != 0)
    						BEGIN
								------ giavon: tinh sau khi tru giam gia hoadon + phi ship
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + (@TongTienHangDemo* (1-(@TongGiamGia/@TongTienHang)))
								+ (@TongTienHangDemo* @TongChiPhi/@TongTienHang))/(@SoLuongDemo + @TonKho);
    						END
    						ELSE
    						BEGIN
    							SET	@GiaVonMoi = ((@GiaVonCu * @TonKho) + @TongTienHangDemo)/(@SoLuongDemo + @TonKho);
    						END
    					END
    					ELSE
    					BEGIN	
							------ Tonkho dauky = 0 ----
    						IF(@TongTienHang != 0)
    						BEGIN
								------ (thanh tien sau giamgia + chi phi VC)/ soluong
    							if @SoLuongDemo > 0
									SET	@GiaVonMoi = (@TongTienHangDemo * (1 - @TongGiamGia / @TongTienHang) 
													+ (@TongTienHangDemo * @TongChiPhi/@TongTienHang)
													)/ @SoLuongDemo
								else
									 SET @GiaVonMoi = @GiaVonCu
    						END
    						ELSE
    						BEGIN
								if @SoLuongDemo > 0
    								SET	@GiaVonMoi = @TongTienHangDemo/@SoLuongDemo;
								else
									 SET @GiaVonMoi = @GiaVonCu
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
    			FETCH NEXT FROM CS_GiaVon INTO @IDHoaDon, @IDHoaDonBan, @IDHoaDonCu, @MaHoaDon, @LoaiHoaDon, @IDChiTietHoaDon, @SoLuong, @DonGia, 
				@TongTienHang, @TongChiPhi, @ChietKhau, @ThanhTien, @TongGiamGia, @TyLeChuyenDoi, @TonKho,
    			@GiaVonCu, @IDHangHoa, @IDDonViQuiDoi, @IDLoHang, @IDChiNhanhThemMoi, @IDCheckIn, @YeuCau, @RN, @GiaVon, @GiaVonNhan
    		END
    		CLOSE CS_GiaVon
    		DEALLOCATE CS_GiaVon		
			
			update gv set GiaVonCu = isnull(GiaVonCu,0)
			from @GiaVonCapNhat gv 
    

    		---- =========Update BH_HoaDon_ChiTiet
			begin try
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
			end try
			begin catch
				select error_number() as ErrNumber,				
					ERROR_LINE() as ErrLine,
					ERROR_MESSAGE() as ErrMsg
			end catch
    		----=========END Update BH_HoaDon_ChiTiet

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
    		
			delete from BH_HoaDon_ChiTiet where ID_HoaDon = @IDHoaDonInput and ChatLieu='5'

    		END
    		--END TinhGiaVonTrungBinh
END");

            Sql(@"ALTER PROCEDURE [dbo].[ValueCard_ServiceUsed]
    @ID_ChiNhanhs [nvarchar](max),
    @TextSearch [nvarchar](max),
    @DateFrom datetime,
    @DateTo datetime,
    @Status [nvarchar](14),
    @CurrentPage [int],
    @PageSize [int]
AS
BEGIN
    SET NOCOUNT ON;   
	declare @tblChiNhanh table (ID_Donvi uniqueidentifier)
	insert into @tblChiNhanh
	select name from dbo.splitstring(@ID_ChiNhanhs)

    DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);

	set @DateTo = DATEADD(day, 1, @DateTo)
    	
    	select 
			hd.ID as ID_HoaDon,
			tblq.ID_HoaDon as ID_PhieuThuChi,
			hd.MaHoaDon,tblq.NgayLapHoaDon,
			ISNULL(dt.MaDoiTuong,'') as MaDoiTuong, 
			ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, 
    		qd.MaHangHoa,
			hh.TenHangHoa,
			ct.SoLuong, 
			ct.DonGia,
			ct.TienChietKhau,
			ct.ThanhTien,  
			ISNULL(tblq.PhatSinhGiam,0) as PhatSinhGiam,
			ISNULL(tblq.PhatSinhTang,0) as PhatSinhTang, 
			tblq.MaHoaDon as MaPhieuThu,		
    		case hd.LoaiHoaDon
    			when 1 then N'Bán hàng'
    			when 3 then N'Đặt hàng'
    			when 6 then N'Trả hàng'
    			when 19 then N'Gói dịch vụ'
    			when 25 then N'Sửa chữa'
    		else '' end as SLoaiHoaDon,
			isnull(maNV.NVThucHien,'') as MaNhanVien,
			isnull(tenNV.NVThucHien,'') as TenNhanVien
    	from BH_HoaDon hd
    	join BH_HoaDon_ChiTiet ct on hd.id= ct.id_hoadon
		join DonViQuiDoi qd on ct.id_donviquidoi= qd.id
    	join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
		join 
		(select 
				qct.ID_HoaDonLienQuan, 
				MaHoaDon,
				NgayLapHoaDon,
				qct.ID_HoaDon,
    		case when qhd.LoaiHoaDon = 11 then SUM(ISNULL(qct.TienThu ,0)) end as PhatSinhGiam,
    		case when qhd.LoaiHoaDon = 12 then SUM(ISNULL(qct.TienThu ,0)) end as PhatSinhTang
    		from Quy_HoaDon_Chitiet qct 
    		join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
    		where qhd.TrangThai ='1' 
    			and qct.HinhThucThanhToan=4
    		and qhd.NgayLapHoaDon  between @DateFrom and @DateTo    		
    		group by qct.ID_HoaDonLienQuan, qct.ID_HoaDon, qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.LoaiHoaDon
		) tblq on hd.ID= tblq.ID_HoaDonLienQuan
    	left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID   	    	
		left join (
			-- get TenNV thuchien of cthd
			select ctOut.ID,
				 (
						select nv.TenNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						where nvth.ID_ChiTietHoaDon = ctOut.ID										
						For XML PATH ('')
					) NVThucHien
				from BH_HoaDon_ChiTiet ctOut 
			) tenNV on ct.ID = tenNV.ID
			left join
			(
			-- get MaNV nvthuchien of cthd
			select distinct ctOut.ID,
				 (
						select nv.MaNhanVien +', '  AS [text()]
						from BH_NhanVienThucHien nvth
						join NS_NhanVien nv on nvth.ID_NhanVien = nv.ID
						where nvth.ID_ChiTietHoaDon = ctOut.ID								
						For XML PATH ('')
					) NVThucHien
				from BH_HoaDon_ChiTiet ctOut 
			) maNV on ct.ID = maNV.ID
    	where hd.LoaiHoaDon in ( 1,3,6,19,25) 
    	and hd.ChoThanhToan ='0'
		and exists (select cn.ID_DonVi from @tblChiNhanh cn where hd.ID_DonVi= cn.ID_Donvi)
    	and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)	
		AND
			((select count(Name) from @tblSearchString b where 
				hd.MaHoaDon like '%'+b.Name+'%' 
    			or tblq.MaHoaDon like '%'+b.Name+'%' 
    			or dt.MaDoiTuong like '%'+b.Name+'%' 
    			or dt.TenDoiTuong like '%'+b.Name+'%'
    			or dt.TenDoiTuong_KhongDau like '%' +b.Name +'%' 
    			or qd.MaHangHoa like '%'+b.Name+'%'
    			or hh.TenHangHoa like '%'+b.Name+'%'
    			or hh.TenHangHoa_KhongDau like '%'+b.Name+'%'
    		)=@count or @count=0)
    		order by hd.NgayLapHoaDon desc
END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoNhomHoTro]");
			DropStoredProcedure("[dbo].[GetHangCungLoai_byID]");
			DropStoredProcedure("[dbo].[GetInfor_PhieuTatToanTheGiaTri]");
			DropStoredProcedure("[dbo].[TGT_GetNhatKyTatToanCongNo]");
			DropStoredProcedure("[dbo].[TinhCongNo_HDTra]");
        }
    }
}
