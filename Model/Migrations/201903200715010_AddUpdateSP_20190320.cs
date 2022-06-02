namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190320 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[TinhSoDuKHTheoThoiGian]
(
	@ID_DoiTuong [uniqueidentifier],
	@Time [datetime]
)
RETURNS FLOAT
AS
BEGIN
		DECLARE @SoDu AS FLOAT;		
		DECLARE @TongNap AS FLOAT;
		DECLARE @SuDungThe AS FLOAT;
		DECLARE @HoanTraTienThe AS FLOAT;
		DECLARE @TongDieuChinh AS FLOAT;

		SELECT @TongNap = SUM(TongTienHang) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 22 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @TongDieuChinh = SUM(TongChiPhi) FROM BH_HoaDon hd
		where hd.NgayLapHoaDon < @Time and hd.ChoThanhToan ='0' and hd.LoaiHoaDon = 23 and hd.ID_DoiTuong = @ID_DoiTuong;

		SELECT @SuDungThe = SUM(qhdct.ThuTuThe) FROM Quy_HoaDon_ChiTiet qhdct
		INNER JOIN Quy_HoaDon qhd
		ON qhdct.ID_HoaDon = qhd.ID
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 11 and (qhd.TrangThai = 1 or qhd.TrangThai is null);

		SELECT @HoanTraTienThe = SUM(qhdct.ThuTuThe) FROM Quy_HoaDon_ChiTiet qhdct
		INNER JOIN Quy_HoaDon qhd
		ON qhdct.ID_HoaDon = qhd.ID
		WHERE qhdct.ID_DoiTuong = @ID_DoiTuong AND qhd.NgayLapHoaDon < @Time and qhd.LoaiHoaDon = 12 and (qhd.TrangThai = 1 or qhd.TrangThai is null);

		SET @SoDu = ISNULL(@TongNap, 0) +  ISNULL(@TongDieuChinh, 0)  - ISNULL(@SuDungThe, 0) + ISNULL(@HoanTraTienThe,0);
	RETURN @SoDu
END");

            CreateStoredProcedure(name: "[dbo].[GetListSuDungThe]", parametersAction: p => new
            {
                ID_DoiTuong = p.String()
            }, body: @"DECLARE @TableTheGT TABLE(ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), MaHoaDonSQ NVARCHAR(MAX), NgayLapHoaDon DATETIME, DienGiai NVARCHAR(MAX), LoaiHoaDon INT, LoaiHoaDonSQ INT, TienThe FLOAT, SoDu FLOAT, STT INT)
    	INSERT INTO @TableTheGT
    	select 
    		*,
    		ROW_NUMBER() OVER (PARTITION BY ID ORDER BY NgayLapHoaDon) AS STT
    	from (
    	 -- su dung the giatri
    			select
    			ID,
    			MaHoaDon,
    			MaHoaDonSQ,
    			NgayLapHoaDon,
    			DienGiai,
    			LoaiHoaDon,
    			LoaiHoaDonSQ,
    			TienThe,
    			SoDu
    			from (
    				SELECT
    				dt.ID,
    				hd.MaHoaDon,
    				qhd.MaHoaDon as MaHoaDonSQ,
    				qhd.NgayLapHoaDon,
    				hd.DienGiai,
    				hd.LoaiHoaDon,
    				qhd.LoaiHoaDon as LoaiHoaDonSQ,
    				qct.ThuTuThe as TienThe,
    				0 as SoDu
    				from DM_DoiTuong dt
    				left join Quy_HoaDon_ChiTiet qct on dt.ID = qct.ID_DoiTuong
    				left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    				left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    				where qct.ThuTuThe != 0 and hd.ChoThanhToan = 0
    			) tblSuDungThe
    ) tblDoiTuong_The where ID = @ID_DoiTuong
    order by NgayLapHoaDon, MaHoaDonSQ
    	
    	
    			DECLARE @ID UNIQUEIDENTIFIER;
    		DECLARE @MaHoaDon NVARCHAR(MAX);
    			DECLARE @MaHoaDonSQ NVARCHAR(MAX);
    		DECLARE @NgayLapHoaDon DATETIME;
    		DECLARE @DienGiai NVARCHAR(MAX);
    		DECLARE @LoaiHoaDon INT;
    		DECLARE @LoaiHoaDonSQ INT;
    		DECLARE @TienThe FLOAT;
    		DECLARE @SoDu FLOAT;
    			DECLARE @STT INT;
    
    			DECLARE @SoDuLuyKe FLOAT;
    			DECLARE @NgayLapHoaDonSS DATETIME;
    			SET @NgayLapHoaDonSS = GETDATE();
    			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR SELECT ID, MaHoaDon, MaHoaDonSQ, NgayLapHoaDon, DienGiai, LoaiHoaDon, LoaiHoaDonSQ, TienThe, SoDu, STT
    			FROM @TableTheGT
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT INTO @ID, @MaHoaDon, @MaHoaDonSQ, @NgayLapHoaDon, @DienGiai, @LoaiHoaDon, @LoaiHoaDonSQ, @TienThe, @SoDu, @STT
    		WHILE @@FETCH_STATUS = 0
    			BEGIN
    				IF(@NgayLapHoaDonSS != @NgayLapHoaDon)
    				BEGIN
    					SET @SoDuLuyKe = [dbo].[TinhSoDuKHTheoThoiGian](@ID,@NgayLapHoaDon)
    					IF(@LoaiHoaDonSQ = 11)
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe - @TienThe 
    					END
    					ELSE
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe + @TienThe 
    					END
    				END
    				ELSE
    				BEGIN
    					IF(@LoaiHoaDonSQ = 11)
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe - @TienThe 
    					END
    					ELSE
    					BEGIN
    						SET @SoDuLuyKe = @SoDuLuyKe + @TienThe 
    					END
    				END
    				
    				SET @NgayLapHoaDonSS = @NgayLapHoaDon
    				UPDATE @TableTheGT SET SoDu = @SoDuLuyKe WHERE STT = @STT
    
    				FETCH NEXT FROM CS_TheGT INTO @ID, @MaHoaDon, @MaHoaDonSQ, @NgayLapHoaDon, @DienGiai, @LoaiHoaDon, @LoaiHoaDonSQ, @TienThe, @SoDu, @STT
    		END
    		CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT
    
    			SELECT * FROM @TableTheGT order by STT desc");

            CreateStoredProcedure(name: "[dbo].[SP_CheckExist_ChietKhauDoanhThuNhanVien]", parametersAction: p => new
            {
                ID_DonVi = p.String(),
                ID_NhanViens = p.String(),
                ApDungTuNgay = p.String(),
                ApDungDenNgay = p.String(),
                ID_ChietKhauDoanhThu = p.String()
            }, body: @"DECLARE @ID_ChietKhauDTSearch varchar(40) = @ID_ChietKhauDoanhThu
	DECLARE @Count int -- đếm số bản ghi có trong bảng #temp

	SELECT tbl.ID, tbl.ID_NhanVien, tbl.MaNhanVien, tbl.TenNhanVien, '' as TenNhanVien_GC, '' as TenNhanVien_CV into #temp 
	FROM
			(select ck.ID, ct.ID_NhanVien, nv.TenNhanVien, nv.MaNhanVien, ck.ApDungTuNgay				
			from ChietKhauDoanhThu ck
			join ChietKhauDoanhThu_NhanVien ct on ck.id= ct.ID_ChietKhauDoanhThu
			join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
			where ck.ID_DonVi like @ID_DonVi
			and ck.TrangThai !='0'
			and ((ck.ApDungDenNgay is not null and convert(varchar,ck.ApDungDenNgay,112) >= @ApDungTuNgay)
				-- DateTo not null & > DateFrom exist in DB
				AND ( @ApDungDenNgay !='' and (convert(varchar,ck.ApDungTuNgay,112) <= @ApDungDenNgay))
				)
			) tbl
			WHERE tbl.ID_NhanVien in (select * from splitstring(@ID_NhanViens))	
	
	SELECT @Count= COUNT(*) FROM #temp
	
	-- - DateTo null & DateFrom exist --> return data null
	IF @ApDungDenNgay ='' and @Count> 0
		 BEGIN
			SELECT *
			FROM #temp
			WHERE MaNhanVien=''
		 END
	ELSE
		BEGIN
			IF @ID_ChietKhauDTSearch='' OR @ID_ChietKhauDTSearch ='00000000-0000-0000-0000-000000000000'
				BEGIN
					SELECT *
					FROM #temp
				END
			ELSE
				BEGIN
					SELECT *
					FROM #temp
					where #temp.ID NOT LIKE @ID_ChietKhauDTSearch
				END
		END	");

            CreateStoredProcedure(name: "[dbo].[SP_CheckExist_ChietKhauHoaDonNhanVien]", parametersAction: p => new
            {
                ID_DonVi = p.String(),
                ID_NhanViens = p.String(),
                ChungTuApDung = p.String(),
                ID_ChietKhauHoaDon = p.String()
            }, body: @"DECLARE @ID_ChietKhauHoaDonSearch varchar(40) = @ID_ChietKhauHoaDon
    
    	SELECT tbl.ID, tbl.ID_NhanVien, tbl.MaNhanVien, tbl.TenNhanVien, '' as TenNhanVien_GC, '' as TenNhanVien_CV into #temp 
    	FROM
    			(select hd.ID, hd.ChungTuApDung, ct.ID_NhanVien, nv.TenNhanVien, nv.MaNhanVien,
    				CASE WHEN hd.ChungTuApDung like '%19%' THEN '19'  ELSE NULL END AS GoiDV,
    				CASE WHEN hd.ChungTuApDung like '%1%'  THEN '1'  ELSE NULL END AS BanHang,
    				CASE WHEN hd.ChungTuApDung like '%3%' THEN '3'  ELSE NULL END AS DatHang,			
    				CASE WHEN hd.ChungTuApDung like '%6%' THEN '6'  ELSE NULL END AS TraHang
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
    				OR tbl.TraHang in (select * from splitstring(@ChungTuApDung)))
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
    		END");

            CreateStoredProcedure(name: "[dbo].[SP_Get_ChietKhauDoanhThu_byDonVi]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"select ID, TinhChietKhauTheo,ApDungTuNgay, ApDungDenNgay, GhiChu,NgayTao,
		case when TinhChietKhauTheo= 3 then cast('0' as bit) else cast('1' as bit)  end as LaPhanTram
	from ChietKhauDoanhThu
	where ID_DonVi like @ID_DonVi
	and TrangThai !='0'
	order by NgayTao desc");

            CreateStoredProcedure(name: "[dbo].[SP_Get_ChietKhauHoaDon_byDonVi]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"select hd.ID, hd.GiaTriChietKhau, hd.TinhChietKhauTheo, hd.ChungTuApDung, hd.GhiChu,NgayTao,
		case when hd.TinhChietKhauTheo= 3 then cast('0' as bit) else cast('1' as bit)  end as LaPhanTram
	from ChietKhauMacDinh_HoaDon hd
	where hd.ID_DonVi like @ID_DonVi
	and hd.TrangThai !='0'
	order by NgayTao desc");

            CreateStoredProcedure(name: "[dbo].[SP_GetAll_DMGiaBan]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"select gb.ID, gb.TenGiaBan, gb.TuNgay, gb.DenNgay, gb.TatCaDonVi,gb.TatCaDoiTuong,
			gb.TatCaNhanVien,gb.LoaiChungTuApDung, gb.NgayTrongTuan, gb.ApDung
	from DM_GiaBan gb
	left join DM_GiaBan_ApDung ad on gb.ID= ad.ID_GiaBan
	where gb.ApDung='1' and (gb.TatCaDonVi='1'  or ad.ID_DonVi like @ID_DonVi)
	and CONVERT(varchar, getdate(),112) >= CONVERT(varchar, gb.TuNgay,112) 
	and CONVERT(varchar, getdate(),112) <= CONVERT(varchar, gb.DenNgay,112) 
	group by gb.ID, gb.TenGiaBan, gb.TuNgay, gb.DenNgay,gb.TatCaDonVi,gb.TatCaDoiTuong, 
	gb.TatCaNhanVien,gb.LoaiChungTuApDung, gb.NgayTrongTuan, gb.ApDung");

            CreateStoredProcedure(name: "[dbo].[SP_GetChietKhauDoanhThuChiTiet_byID]", parametersAction: p => new
            {
                ID_ChietKhauDoanhThu = p.String()
            }, body: @"SELECT ct.ID,ct.ID_ChietKhauDoanhThu, ct.DoanhThuTu, ct.DoanhThuDen, ct.GiaTriChietKhau, ct.LaPhanTram
	FROM ChietKhauDoanhThu dt
	join ChietKhauDoanhThu_ChiTiet ct on dt.ID = ct.ID_ChietKhauDoanhThu
	where dt.TrangThai !=0 and ct.ID_ChietKhauDoanhThu like @ID_ChietKhauDoanhThu");

            CreateStoredProcedure(name: "[dbo].[SP_GetChietKhauDoanhThuNhanVien_byID]", parametersAction: p => new
            {
                ID_ChietKhauDoanhThu = p.String(),
                ID_DonVi = p.String()
            }, body: @"select nv.ID,nv.MaNhanVien, nv.TenNhanVien, nvpb.NhanVien_PhongBan as TenNhanVien_GC, convert(varchar, NgaySinh,103) as TenNhanVien_CV
	from ChietKhauDoanhThu_NhanVien ct
	join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
	left join (
	Select distinct hdXML.ID,
					 (
						select pb.TenPhongBan +', '  AS [text()]
						from NS_QuaTrinhCongTac qtct
						join NS_PhongBan pb on qtct.ID_PhongBan= pb.ID
						where qtct.ID_NhanVien = hdXML.ID and qtct.ID_DonVi like @ID_DonVi
						For XML PATH ('')
					) NhanVien_PhongBan
				from NS_NhanVien hdXML 

	) nvpb on nvpb.ID= nv.ID where ct.ID_ChietKhauDoanhThu like @ID_ChietKhauDoanhThu");

            CreateStoredProcedure(name: "[dbo].[SP_GetChietKhauHoaDon_ChiTiet]", parametersAction: p => new
            {
                ID_ChietKhauHoaDon = p.String(),
                ID_DonVi = p.String()
            }, body: @"select nv.ID,nv.MaNhanVien, nv.TenNhanVien, nvpb.NhanVien_PhongBan as TenNhanVien_GC, convert(varchar, NgaySinh,103) as TenNhanVien_CV
	from ChietKhauMacDinh_HoaDon_ChiTiet ct
	join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
	left join (
	Select distinct hdXML.ID,
					 (
						select pb.TenPhongBan +', '  AS [text()]
						from NS_QuaTrinhCongTac qtct
						join NS_PhongBan pb on qtct.ID_PhongBan= pb.ID
						where qtct.ID_NhanVien = hdXML.ID and qtct.ID_DonVi like @ID_DonVi
						For XML PATH ('')
					) NhanVien_PhongBan
				from NS_NhanVien hdXML 

	) nvpb on nvpb.ID= nv.ID where ct.ID_ChietKhauHoaDon like @ID_ChietKhauHoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_GetDiaryCard_ofHoaDon]", parametersAction: p => new
            {
                ID_HoaDonLienQuan = p.String()
            }, body: @"select 
		qhd.MaHoaDon, qhd.NgayLapHoaDon,  qhd.NguoiTao as NguoiNopTien, sum(qct.ThuTuThe) as TongTienThu,
		case when qhd.LoaiHoaDon= 12 then N'Hoàn trả tiền thẻ' else N'Sử dụng thẻ' end as LoaiPhieu
	from Quy_HoaDon qhd
	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
	where qct.ID_HoaDonLienQuan like @ID_HoaDonLienQuan
	and ISNULL(qct.ThuTuThe,0) > 0
	group by qhd.MaHoaDon, qhd.NgayLapHoaDon,  qhd.NguoiTao, qhd.LoaiHoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_GetHDDebit_ofKhachHang]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"select hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, 
		ISNULL(hd.PhaiThanhToan,0) - ISNULL(hdt.PhaiThanhToan,0) as PhaiThanhToan,
		ISNULL(TinhChietKhauTheo,1) as TinhChietKhauTheo
	from BH_HoaDon hd
	left join BH_HoaDon hdt on hd.ID_HoaDon= hdt.ID
	left join 
			(select ID_HoaDon, min(TinhChietKhauTheo) as TinhChietKhauTheo
			from BH_NhanVienThucHien nvth
			where nvth.ID_HoaDon is not null
			group by ID_HoaDon) tblNV on hd.ID = tblNV.ID_HoaDon
	where hd.ID_DonVi like @ID_DonVi and hd.ID_DoiTuong like @ID_DoiTuong
	and hd.LoaiHoaDon in (1,19)
	and hd.ChoThanhToan='0' ");

            CreateStoredProcedure(name: "[dbo].[SP_GetHoaDonAndSoQuy_FromIDDoiTuong]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"-- get list HD (not DatHang)
	select ID, MaHoaDon, NgayLapHoaDon, LoaiHoaDon,
		case when LoaiHoaDon= 6 or LoaiHoaDon= 7 then - ISNULL(PhaiThanhToan,0)
		else 
			ISNULL(PhaiThanhToan,0)
		end as GiaTri
	from BH_HoaDon hd
	where hd.ID_DoiTuong like @ID_DoiTuong and hd.ID_DonVi like @ID_DonVi
	and hd.LoaiHoaDon != 22

	union
	-- get list Quy_HD (không lấy Quy_HoaDon thu từ Thẻ giá trị)
	select qhd.ID, MaHoaDon, NgayLapHoaDon, LoaiHoaDon ,
		case when dt.LoaiDoiTuong = 1 then
			case when qhd.LoaiHoaDon= 11 then - qhd.TongTienThu else qhd.TongTienThu end
		else 
			case when qhd.LoaiHoaDon = 11 then qhd.TongTienThu else - qhd.TongTienThu end
		end as GiaTri
	from Quy_HoaDon qhd	
	join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
	join DM_DoiTuong dt on qct.ID_DoiTuong= dt.ID
	where qhd.ID_DonVi like @ID_DonVi and qct.ID_DoiTuong like @ID_DoiTuong
	and qct.ID_HoaDonLienQuan not in (select id from BH_HoaDon where LoaiHoaDon=22)");

            CreateStoredProcedure(name: "[dbo].[SP_GetQuyHoaDon_ofDoiTuong]", parametersAction: p => new
            {
                ID_DoiTuong = p.String(),
                ID_DonVi = p.String()
            }, body: @"select ct.ID_HoaDonLienQuan,
    		case when hd.LoaiHoaDon = 6 then sum(ISNULL(ct.TienThu,0)) else 
    		case when max(qhd.LoaiHoaDon) = 11 then sum(ct.TienThu) else sum(ISNULL(-ct.TienThu,0)) end end TongTienThu
		into #temp1
    	from Quy_HoaDon_ChiTiet ct
    	join Quy_HoaDon qhd on ct.ID_HoaDon = qhd.ID
    	left join BH_HoaDon hd on ct.ID_HoaDonLienQuan = hd.ID
    	where ct.ID_DoiTuong like @ID_DoiTuong and qhd.ID_DonVi like @ID_DonVi
    	and (TrangThai is  null or TrangThai = '1' ) 
    	group by ct.ID_HoaDonLienQuan, hd.LoaiHoaDon,TrangThai

		select hd.ID as ID_HoaDonLienQuan , 
			ISNULL(b.TongTienThu,0) as TongTienThu
		into #temp2
    	from BH_HoaDon hd 
    	left join 
    				(-- get TongThu from HĐatHang
    				select sum(ct.TienThu) as TongTienThu, hdm.ID
    				from Quy_HoaDon_ChiTiet ct
    				join Quy_HoaDon qhd on ct.ID_HoaDon = qhd.ID
    				join BH_HoaDon hdd on ct.ID_HoaDonLienQuan= hdd.ID
    				join BH_HoaDon hdm on hdd.ID = hdm.ID_HoaDon
    				where ct.ID_DoiTuong like @ID_DoiTuong and qhd.ID_DonVi like @ID_DonVi
    				and (TrangThai is not null or TrangThai = '1' ) and hdd.LoaiHoaDon = 3 and hdm.LoaiHoaDon =1
    				group by hdm.ID
    				) b on b.ID = hd.ID
    	where hd.NgayLapHoaDon = (
    					-- chi lay HDDatHang thanh toan cho HD dau tien
    					select min(NgayLapHoaDon)
    					from 
    					(
    						select hdm.NgayLapHoaDon, ct.ID_HoaDonLienQuan
    						from Quy_HoaDon_ChiTiet ct
    						join Quy_HoaDon qhd on ct.ID_HoaDon = qhd.ID
    						join BH_HoaDon hdd on ct.ID_HoaDonLienQuan= hdd.ID
    						join BH_HoaDon hdm on hdd.ID = hdm.ID_HoaDon
    						where ct.ID_DoiTuong like @ID_DoiTuong and qhd.ID_DonVi like @ID_DonVi
    						and (TrangThai is not null or TrangThai = '1' )  and hdd.LoaiHoaDon=3 and hdm.LoaiHoaDon =1
    						group by ct.ID_HoaDonLienQuan, hdm.ID,hdm.NgayLapHoaDon)
    						b group by b.ID_HoaDonLienQuan
    					)

		select #temp1.ID_HoaDonLienQuan, ISNULL(#temp1.TongTienThu,0)+ ISNULL(#temp2.TongTienThu,0) as TongTienThu
		from #temp1
		left join #temp2 on #temp1.ID_HoaDonLienQuan = #temp2.ID_HoaDonLienQuan");

            CreateStoredProcedure(name: "[dbo].[SP_GetSoDuTheGiaTri_ofKhachHang]", parametersAction: p => new
            {
                ID_DoiTuong = p.String()
            }, body: @"declare @ID_Search varchar(40) = @ID_DoiTuong
    	if	@ID_Search ='' or @ID_Search='00000000-0000-0000-0000-000000000000'
    		SET @ID_Search='%%'
    				
    	select 
    		ID_DoiTuong,
    		SUM(ISNULL(TongThuTheGiaTri,0)) as TongThuTheGiaTri,
    		SUM(ISNULL(SuDungThe,0)) as SuDungThe,
    		SUM(ISNULL(HoanTraTheGiatri,0)) as HoanTraTheGiaTri,
    		SUM(ISNULL(TongThuTheGiaTri,0))  - SUM(ISNULL(SuDungThe,0)) + SUM(ISNULL(HoanTraTheGiatri,0)) as SoDuTheGiaTri
    	from (
    	 --- thu the gtri
    		 select ID as ID_DoiTuong, 
    			 null as SuDungThe,
    			 null as HoanTraTheGiatri,
    			 SUM(ISNULL(TongThuTheGiaTri,0)) as TongThuTheGiaTri
    		 from (
    			 SELECT dt.ID, 
    				 case when (hd.LoaiHoaDon=22  or hd.LoaiHoaDon=23) then sum(hd.TongTienHang)
    				 else 0 end as TongThuTheGiaTri
    			 from DM_DoiTuong dt
    			 left join BH_HoaDon hd on hd.ID_DoiTuong = dt.ID
    			 where (dt.TheoDoi is null or dt.TheoDoi=0) and hd.ChoThanhToan ='0' and (dt.LoaiDoiTuong=1 or dt.LoaiDoiTuong is null)
    			 group by dt.ID, hd.LoaiHoaDon
    		 ) tblThuThe group by tblThuThe.ID
    
    	 union all
    	 -- su dung the giatri
    		 select ID_DoiTuong, 
    			 sum(ISNULL(SuDungThe,0)) as SuDungThe,
    			  null as HoanTraTheGiatri,
    			 null as TongThuTheGiaTri
    			
    		 from (
    			 SELECT qct.ID_DoiTuong,
    				case when qhd.LoaiHoaDon= 12 then 0 else sum(qct.ThuTuThe) end as SuDungThe
    			 from Quy_HoaDon_ChiTiet qct
    			 left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    			 left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    			 where (qhd.TrangThai is null or qhd.TrangThai='1') and hd.ChoThanhToan ='0'
    			 group by qct.ID_DoiTuong, qhd.LoaiHoaDon,qhd.TrangThai,hd.ChoThanhToan
    		 ) tblSuDungThe group by tblSuDungThe.ID_DoiTuong
    
    	 union all
    	  -- hoan tra tien the giatri
    		select ID_DoiTuong, 
    			null as SuDungThe,
    			SUM(ISNULL(HoanTraTheGiatri,0)) as HoanTraTheGiatri,
    			null as TongThuTheGiaTri
    		from (
    				SELECT qct.ID_DoiTuong,
    				case when qhd.LoaiHoaDon= 11 then 0 else sum(qct.ThuTuThe) end as HoanTraTheGiatri
    				from Quy_HoaDon_ChiTiet qct
    				left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    				left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
    				where (qhd.TrangThai is null or qhd.TrangThai='1') and hd.ChoThanhToan ='0'
    				group by qct.ID_DoiTuong, qhd.LoaiHoaDon, qhd.TrangThai,hd.ChoThanhToan
    			) tblSuDungThe group by tblSuDungThe.ID_DoiTuong
    ) tblDoiTuong_The where ID_DoiTuong like @ID_Search group by ID_DoiTuong");

            CreateStoredProcedure(name: "[dbo].[SP_InsertChietKhauHoaDonTraHang_NhanVien]", parametersAction: p => new
            {
                ID_HoaDon = p.String(),
                TongTienTra = p.Double(),
                ID_HoaDonTra = p.String(),
                ID_DonVi = p.String()
            }, body: @"declare @count_CKTraHang int
	--- check xem co cai dat chiet khau TraHang khong
	select @count_CKTraHang = count(hd.ID)
	from ChietKhauMacDinh_HoaDon hd
	where hd.ID_DonVi like @ID_DonVi
	and hd.TrangThai !='0' and  hd.ChungTuApDung like '%6%'

	if	@count_CKTraHang > 0
		begin		
			-- get PhaiThanhToan from HDMua --> chia % de tinh lai ChietKhau (theo VND)
			declare @PhaiThanhToan float
			select @PhaiThanhToan = PhaiThanhToan from BH_HoaDon where ID like @ID_HoaDon
			
			-- copy data from BH_NhanVienThucHien (HDMua) to BH_NhanVienThucHien (HDTra) with new {ID_HoaDon, TienChietKhau}
			insert into BH_NhanVienThucHien (ID, ID_NhanVien,  PT_ChietKhau, TinhChietKhauTheo, HeSo,ID_HoaDon,ThucHien_TuVan, TheoYeuCau, TienChietKhau)

			select NewID() as ID, ID_NhanVien, PT_ChietKhau, TinhChietKhauTheo, HeSo,	@ID_HoaDonTra as ID_HoaDon,	ThucHien_TuVan, TheoYeuCau,
				case when TinhChietKhauTheo !=3 then (@TongTienTra * PT_ChietKhau * HeSo)/100
				else (TienChietKhau/@PhaiThanhToan) * @TongTienTra end  as TienChietKhau
				from BH_NhanVienThucHien
			where ID_HoaDon like @ID_HoaDon
		end");

            CreateStoredProcedure(name: "[dbo].[SP_ReportValueCard_Balance]", parametersAction: p => new
            {
                TextSearch = p.String(),
                ID_ChiNhanhs = p.String(),
                DateFrom = p.String(),
                DateTo = p.String(),
                Status = p.String()
            }, body: @"select 
			tblView.ID, tblView.MaDoiTuong, tblView.TenDoiTuong, 
			ISNULL(tblView.DienThoai,'') as DienThoaiKhachHang,
			CAST(ISNULL(tblView.SoDuDauKy,0) as float) as SoDuDauKy,
			CAST(ISNULL(tblView.PhatSinhTang,0) as float) as PhatSinhTang,
			CAST(ISNULL(tblView.PhatSinhGiam,0) as float) as PhatSinhGiam,
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
				SUM(ISNULL(TongThuTheGiaTri,0))  - SUM(ISNULL(SuDungThe,0)) + SUM(ISNULL(HoanTraTheGiatri,0)) as SoDuDauKy,
				SUM(ISNULL(PhatSinh_ThuTuThe,0)) + SUM(ISNULL(PhatSinh_HoanTraTheGiatri,0)) as PhatSinhTang,
				SUM(ISNULL(PhatSinh_SuDungThe,0)) as PhatSinhGiam

			from (
				 --- thu the gtri trước thời gian tìm kiếm
					 select ID as ID_DoiTuong, 
						SUM(ISNULL(TongThuTheGiaTri,0)) as TongThuTheGiaTri,
						 null as SuDungThe,
						 null as HoanTraTheGiatri,						 
						 null as PhatSinh_ThuTuThe,
						 null as PhatSinh_SuDungThe,
						 null as PhatSinh_HoanTraTheGiatri
					 from (
						 SELECT dt.ID, 
							 case when (hd.LoaiHoaDon=22  or hd.LoaiHoaDon=23) then sum(hd.TongTienHang)
							 else 0 end as TongThuTheGiaTri
						 from DM_DoiTuong dt
						 left join BH_HoaDon hd on hd.ID_DoiTuong = dt.ID
						 where CONVERT(varchar,hd.NgayLapHoaDon,112) <= @DateFrom
						 and hd.ChoThanhToan='0' and hd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
						 group by dt.ID, hd.LoaiHoaDon
					 ) tblThuThe group by tblThuThe.ID

				 union all
				 -- su dung the giatri
					 select tblSuDungThe.ID_DoiTuong, 
					  null as TongThuTheGiaTri,
						 sum(ISNULL(SuDungThe,0)) as SuDungThe,
						 null as HoanTraTheGiatri,						
						 null as PhatSinh_ThuTuThe,
						 null as PhatSinh_SuDungThe,
						 null as PhatSinh_HoanTraTheGiatri
			
					 from (
						 SELECT qct.ID_DoiTuong,
							case when qhd.LoaiHoaDon= 12 then 0 else sum(qct.ThuTuThe) end as SuDungThe
						 from Quy_HoaDon_ChiTiet qct
						 left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
						 left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
						 where CONVERT(varchar,qhd.NgayLapHoaDon,112) <= @DateFrom 
						 and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
						 and (qhd.TrangThai ='1' or qhd.TrangThai is null)
						 and hd.ChoThanhToan ='0'
						 group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
					 ) tblSuDungThe group by tblSuDungThe.ID_DoiTuong

			 union all
				  -- hoan tra tien the giatri
					select ID_DoiTuong, 
						null as TongThuTheGiaTri,
						null as SuDungThe,
						SUM(ISNULL(HoanTraTheGiatri,0)) as HoanTraTheGiatri,						
						null as PhatSinh_ThuTuThe,
						null as PhatSinh_SuDungThe,
						null as PhatSinh_HoanTraTheGiatri
					from (
							SELECT qct.ID_DoiTuong,
							case when qhd.LoaiHoaDon= 11 then 0 else sum(qct.ThuTuThe) end as HoanTraTheGiatri
							from Quy_HoaDon_ChiTiet qct
							left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
							left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
							where CONVERT(varchar,qhd.NgayLapHoaDon,112) <= @DateFrom 
							and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
							and (qhd.TrangThai ='1' or qhd.TrangThai is null)
							and hd.ChoThanhToan ='0'
							group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
						) tblSuDungThe group by tblSuDungThe.ID_DoiTuong 

			 union all
				   --- thu the gtri tại thời điểm hiện tại
					 select ID_DoiTuong, 
					 	 null as TongThuTheGiaTri,
						 null as SuDungThe,
						 null as HoanTraTheGiatri,
						 SUM(ISNULL(TongThuTheGiaTri,0)) as PhatSinh_ThuTuThe,
						 null as PhatSinh_SuDungThe,
						 null as PhatSinh_HoanTraTheGiatri
					 from (
						 SELECT hd.ID_DoiTuong, 
							 case when (hd.LoaiHoaDon=22  or hd.LoaiHoaDon=23) then sum(hd.TongTienHang)
							 else 0 end as TongThuTheGiaTri
						 from BH_HoaDon hd 
						 where CONVERT(varchar,hd.NgayLapHoaDon,112) >= @DateFrom and CONVERT(varchar,hd.NgayLapHoaDon,112) <= @Dateto
						 and hd.ChoThanhToan='0' and hd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
						 group by hd.ID_DoiTuong, hd.LoaiHoaDon
					 ) tblThuThe2 group by tblThuThe2.ID_DoiTuong

			union all
				 -- su dung the giatri tại thời điểm hiện tại
					 select tblSuDungThe2.ID_DoiTuong, 
						null as TongThuTheGiaTri,
						 null as SuDungThe,
						 null as HoanTraTheGiatri,						 
						 null as PhatSinh_ThuTuThe,
						 sum(ISNULL(SuDungThe,0)) as PhatSinh_SuDungThe,
						 null as PhatSinh_HoanTraTheGiatri
			
					 from (
						 SELECT qct.ID_DoiTuong,
							case when qhd.LoaiHoaDon= 12 then 0 else sum(qct.ThuTuThe) end as SuDungThe
						 from Quy_HoaDon_ChiTiet qct
						 left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
						 left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
						 where CONVERT(varchar,qhd.NgayLapHoaDon,112) >= @DateFrom and CONVERT(varchar,qhd.NgayLapHoaDon,112) <= @Dateto
						 and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
						 and (qhd.TrangThai ='1' or qhd.TrangThai is null)
						 and hd.ChoThanhToan ='0'
						 group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
					 ) tblSuDungThe2 group by tblSuDungThe2.ID_DoiTuong

			union all
				  -- hoan tra tien the giatri tại thời điểm hiện tại
					select ID_DoiTuong, 
						null as TongThuTheGiaTri,
						null as SuDungThe,
						null as HoanTraTheGiatri,						
						null as PhatSinh_ThuTuThe,
						null as PhatSinh_SuDungThe,
						SUM(ISNULL(HoanTraTheGiatri,0)) as PhatSinh_HoanTraTheGiatri
					from (
							SELECT qct.ID_DoiTuong,
							case when qhd.LoaiHoaDon= 11 then 0 else sum(qct.ThuTuThe) end as HoanTraTheGiatri
							from Quy_HoaDon_ChiTiet qct
							left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
							left join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
							where CONVERT(varchar,qhd.NgayLapHoaDon,112) >= @DateFrom and CONVERT(varchar,qhd.NgayLapHoaDon,112) <= @Dateto
							and qhd.ID_DonVi in (select * from dbo.splitstring (@ID_ChiNhanhs))
							and (qhd.TrangThai ='1' or qhd.TrangThai is null)
							and hd.ChoThanhToan ='0'
							group by qct.ID_DoiTuong, qhd.LoaiHoaDon,hd.ChoThanhToan
						) tblSuDungThe2 group by tblSuDungThe2.ID_DoiTuong 

				) tblDoiTuong_The group by tblDoiTuong_The.ID_DoiTuong
		) tblTemp on dt.ID= tblTemp.ID_DoiTuong
		where (dt.TheoDoi is null or dt.TheoDoi = 0) and dt.LoaiDoiTuong =1

	) tblView 
	where tblView.TrangThai like @Status
	and ISNULL(tblView.SoDuDauKy,0) + ISNULL(tblView.PhatSinhTang,0)- ISNULL(tblView.PhatSinhGiam,0) > 0");

            CreateStoredProcedure(name: "[dbo].[SP_ReportValueCard_DiaryUsed]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                DateFrom = p.String(),
                DateTo = p.String(),
                Status = p.String()
            }, body: @"DECLARE @TblHisCard TABLE(
				STT INT, ID UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(50),TenDoiTuong NVARCHAR(500), MaHoaDon NVARCHAR(50),  LoaiHoaDon INT, 
				MaHoaDonSQ NVARCHAR(MAX),LoaiHoaDonSQ INT, NgayLapHoaDon DATETIME, TienThe FLOAT, ThuChiThe FLOAT, SoDuTruoc FLOAT, SoDuSau FLOAT, TrangThai_TheGiaTri int)
	INSERT INTO @TblHisCard
		select 
			ROW_NUMBER() OVER(ORDER BY qhd.ID, hd.ID) AS STT,
			dt.ID,
			dt.MaDoiTuong,
			dt.TenDoiTuong,
			hd.MaHoaDon,
			hd.LoaiHoaDon,
			qhd.MaHoaDon as MaHoaDonSQ, 
			qhd.LoaiHoaDon as LoaiHoaDonSQ,
			qhd.NgayLapHoaDon,
			SUM(ISNULL(qct.ThuTuThe,0)) as TienThe,
			case when qhd.LoaiHoaDon = 11 then - SUM(ISNULL(qct.ThuTuThe,0)) else SUM(ISNULL(qct.ThuTuThe,0)) end as ThuChiThe,
			0 as SoDuTruoc,
			0 as SoDuSau,
			case when dt.TrangThai_TheGiaTri is null or dt.TrangThai_TheGiaTri = 1 then '11'
			else '12' end as TrangThai -- used to where TrangThai_TheGiaTri (1: all, 11: dang hoat dong, 2. Ngung hoat dong)
		from BH_HoaDon hd
		join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
		join Quy_HoaDon_ChiTiet qct on qct.ID_HoaDonLienQuan = hd.ID and dt.ID= qct.ID_DoiTuong
		join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID 
		where hd.ChoThanhToan ='0' and (qhd.TrangThai =1 or qhd.TrangThai is null)		
		and hd.LoaiHoaDon in (1, 3, 6,19)
		and qct.ThuTuThe > 0
		and CONVERT(varchar, qhd.NgayLapHoaDon, 112) >= @DateFrom 
		and CONVERT(varchar, qhd.NgayLapHoaDon, 112) <= @DateTo
		group by dt.ID,dt.MaDoiTuong, qhd.MaHoaDon, qhd.ID, qhd.LoaiHoaDon, qhd.NgayLapHoaDon,
		hd.ID, hd.MaHoaDon, hd.LoaiHoaDon,dt.TrangThai_TheGiaTri, dt.TenDoiTuong
		
	
			DECLARE @SoDuTruocPhatSinh INT;

			DECLARE @STT INT;
			DECLARE @ID UNIQUEIDENTIFIER;
			DECLARE @MaDoiTuong NVARCHAR(50);
			DECLARE @TenDoiTuong NVARCHAR(500);
			DECLARE @MaHoaDon NVARCHAR(50);
			DECLARE @LoaiHoaDon int;
			DECLARE @MaHoaDonSQ NVARCHAR(50);
			DECLARE @LoaiHoaDonSQ INT;
			DECLARE @NgayLapHoaDon DATETIME;
			DECLARE @TienThe FLOAT;
			DECLARE @ThuChiThe FLOAT;
			DECLARE @SoDuTruoc FLOAT;
			DECLARE @SoDuSau FLOAT;
			DECLARE @TrangThai_TheGiaTri INT;

			DECLARE CS_TheGT CURSOR SCROLL LOCAL FOR SELECT STT, ID, MaDoiTuong, TenDoiTuong, MaHoaDon, LoaiHoaDon, MaHoaDonSQ, LoaiHoaDonSQ, NgayLapHoaDon, TienThe, ThuChiThe, SoDuTruoc, SoDuSau, TrangThai_TheGiaTri
			FROM @TblHisCard
    		OPEN CS_TheGT
    		FETCH FIRST FROM CS_TheGT INTO @STT,@ID, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe,@ThuChiThe, @SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri
    		WHILE @@FETCH_STATUS = 0
			BEGIN
					SET @SoDuTruocPhatSinh = [dbo].[TinhSoDuKHTheoThoiGian](@ID,@NgayLapHoaDon)

					UPDATE @TblHisCard SET SoDuTruoc= @SoDuTruocPhatSinh, SoDuSau = @SoDuTruocPhatSinh + @ThuChiThe
					WHERE STT = @STT

					FETCH NEXT FROM CS_TheGT INTO @STT,@ID, @MaDoiTuong,@TenDoiTuong, @MaHoaDon, @LoaiHoaDon, @MaHoaDonSQ, @LoaiHoaDonSQ, @NgayLapHoaDon, @TienThe, @ThuChiThe,@SoDuTruoc, @SoDuSau, @TrangThai_TheGiaTri

					
			END
			CLOSE CS_TheGT
    		DEALLOCATE CS_TheGT

			SELECT 
				MaDoiTuong, TenDoiTuong, MaHoaDon, MaHoaDonSQ, 
				TrangThai_TheGiaTri,
				NgayLapHoaDon,
				LoaiHoaDonSQ,
				case LoaiHoaDon
					when 1 then N'Bán hàng' 
					when 3 then N'Đặt hàng' 
					when 6 then N'Trả hàng'
					else N'Bán gói dịch vụ'
				end as SLoaiHoaDon,						
				TienThe,
				SoDuTruoc,
				SoDuSau		
			FROM @TblHisCard 
			WHERE TrangThai_TheGiaTri like @Status
			ORDER BY NgayLapHoaDon desc");

            CreateStoredProcedure(name: "[dbo].[UpdateLaiSoDuTheNap]", parametersAction: p => new
            {
                NgayLapHoaDonInput = p.DateTime(),
                IDDoiTuong = p.Guid()
            }, body: @"DECLARE @TableUpdate TABLE(ID UNIQUEIDENTIFIER, TongTienHang FLOAT, NgayLapHoaDon DATETIME) 
	INSERT INTO @TableUpdate
	SELECT hd.ID, hd.TongTienHang, hd.NgayLapHoaDon FROM BH_HoaDon hd where hd.ID_DoiTuong = @IDDoiTuong and hd.ChoThanhToan = 0 and (hd.LoaiHoaDon = 22 or LoaiHoaDon = 23) and hd.NgayLapHoaDon >= @NgayLapHoaDonInput
	ORDER BY NgayLapHoaDon
	
	DECLARE @ID UNIQUEIDENTIFIER;
	DECLARE @TongTienHang FLOAT;
	DECLARE @NgayLapHoaDon DATETIME;

	DECLARE CS_UPD CURSOR SCROLL LOCAL FOR SELECT ID, TongTienHang, NgayLapHoaDon  FROM @TableUpdate ORDER BY NgayLapHoaDon
    		OPEN CS_UPD
    		FETCH FIRST FROM CS_UPD INTO @ID, @TongTienHang, @NgayLapHoaDon
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				DECLARE @SoDuThoiGianInput FLOAT;
				DECLARE @SoDuSauNap FLOAT;
				SET @SoDuThoiGianInput = [dbo].[TinhSoDuKHTheoThoiGian](@IDDoiTuong, @NgayLapHoaDon);
				SET @SoDuSauNap = @SoDuThoiGianInput + @TongTienHang;
				UPDATE BH_HoaDon SET TongTienThue = @SoDuSauNap WHERE ID = @ID
				FETCH NEXT FROM CS_UPD INTO @ID, @TongTienHang, @NgayLapHoaDon
    		END
    		CLOSE CS_UPD
    		DEALLOCATE CS_UPD");

            Sql(@"ALTER PROCEDURE [dbo].[SP_AddChietKhau_ByIDNhom]
    @ID_NhomHangs [nvarchar](max),
    @ID_NhanVien [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    DECLARE @i int = 0
    	DECLARE @IDQuiDoi uniqueidentifier
    	SET @i= 
    		(SELECT COUNT(*) FROM DM_HangHoa hh
    		join DonViQuiDoi qd ON hh.iD= qd.ID_HangHoa
    		WHERE hh.ID_NhomHang in (Select * from splitstring(@ID_NhomHangs)))
    
    	WHILE @i>0
    		BEGIN
    			SELECT @IDQuiDoi= tb.ID FROM (
    				SELECT ROW_NUMBER() OVER (Order by qd.ID) AS RowNumber, qd.ID FROM DM_HangHoa hh
    				join DonViQuiDoi qd ON hh.iD= qd.ID_HangHoa
    				WHERE hh.ID_NhomHang in (Select * from splitstring(@ID_NhomHangs))
					and (qd.Xoa is null OR qd.Xoa='0')
    				and qd.ID not in (select ID_DonViQuiDoi from ChietKhauMacDinh_NhanVien where ID_NhanVien like @ID_NhanVien  and ID_DonVi like @ID_DonVi)
    				) tb
    			WHERE tb.RowNumber = @i
    			SET @i= @i -1;
    
    			if @IDQuiDoi is not null
    				INSERT INTO ChietKhauMacDinh_NhanVien (ID, ID_NhanVien, ID_DonVi, ChietKhau, LaPhanTram, ChietKhau_YeuCau, LaPhanTram_YeuCau, ChietKhau_TuVan, LaPhanTram_TuVan,ID_DonViQuiDoi, NgayNhap)
    				values ( NEWID(),@ID_NhanVien,@ID_DonVi, 0,'0',0,'0',0,'0',@IDQuiDoi,getdate())
    		END
END");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetHoaDonChoThanhToan]
    @LoaiHoaDon [int],
	@ID_DonVi varchar(40)
AS
BEGIN
    select 
    	hd.ID, hd.ID_DoiTuong, hd.ID_BangGia, hd.ID_NhanVien, hd.ID_ViTri, hd.ID_DonVi, hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienThue, hd.TongGiamGia, hd.TongChietKhau, 
    	hd.TongChiPhi, hd.PhaiThanhToan, hd.TongTienHang, ISNULL(hd.ChoThanhToan,'0') as ChoThanhToan,
    	hd.DienGiai,CAST(ISNULL(hd.KhuyeMai_GiamGia, 0) as float) as KhuyeMai_GiamGia , ISNULL(hd.YeuCau,'') as YeuCau, hd.LoaiHoaDon,
    	dv.TenDonVi, ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, ISNULL(nv.TenNhanVien,'') as TenNhanVien,
    	ISNULL(vt.TenViTri,'') as TenViTri, ISNULL(gb.TenGiaBan, N'Bảng giá chung') as TenGiaBan,
    	ISNULL(dt.DienThoai,'') as DienThoai, ISNULL(dt.Email,'') as Email
    from BH_HoaDon hd
    left join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
    left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
    left join DM_DonVi dv on hd.ID_DonVi= dv.ID
    left join DM_ViTri vt on hd.ID_ViTri= vt.ID
    left join DM_GiaBan gb on  hd.ID_BangGia= gb.ID
    left join Quy_HoaDon_ChiTiet qct on hd.ID_HoaDon= qct.ID_HoaDonLienQuan
    where hd.LoaiHoaDon like @LoaiHoaDon and hd.ID_DonVi like @ID_DonVi
	and ChoThanhToan = '1'
END

--SP_GetHoaDonChoThanhToan 3,'d93b17ea-89b9-4ecf-b242-d03b8cde71de'");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetListNhanVien_inDepartment]
    @ID_PhongBans [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
		-- phai khai bao @lstID_PhongBans de check dieu kien OR trong lenh if
    	declare @lstID_PhongBans nvarchar(max) = @ID_PhongBans

		-- create table #temp: get all NhanVien by ID_DonVi (LaDonViHienThoi = true)
		select distinct nv.ID, nv.MaNhanVien, nv.TenNhanVien,  ISNULL(pb.TenPhongBan,'') as TenNhanVien_GC, '' as TenNhanVien_CV, 
    			ISNULL(ct.ID_PhongBan,'00000000-0000-0000-0000-000000000000') as ID_PhongBan, ct.LaDonViHienThoi, nv.TrangThai
		into #temp
		from NS_NhanVien nv
		left join NS_QuaTrinhCongTac ct on nv.ID= ct.ID_NhanVien
		left join NS_PhongBan pb on ct.ID_PhongBan=  pb.ID 
		where ct.ID_DonVi like  @ID_DonVi and (pb.ID_DonVi like @ID_DonVi OR pb.ID_DonVi is null)
		and (nv.DaNghiViec is null or nv.DaNghiViec='0') and (nv.TrangThai is null OR nv.TrangThai !='0')
		--and ct.LaDonViHienThoi= 1

    	if	(@lstID_PhongBans ='' or @lstID_PhongBans ='00000000-0000-0000-0000-000000000000')
    		begin		
    			select * from #temp
				order by ID_PhongBan, LaDonViHienThoi
    		end
    	else
    		begin
    			select * from #temp
				where #temp.ID_PhongBan in (Select * from splitstring(@ID_PhongBans))  
				order by ID_PhongBan,LaDonViHienThoi
    		end
END

--SP_GetListNhanVien_inDepartment '','D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'");

        }
        
        public override void Down()
        {
            Sql("DROP FUNCTION [dbo].[TinhSoDuKHTheoThoiGian]");
            DropStoredProcedure("[dbo].[GetListSuDungThe]");
            DropStoredProcedure("[dbo].[SP_CheckExist_ChietKhauDoanhThuNhanVien]");
            DropStoredProcedure("[dbo].[SP_CheckExist_ChietKhauHoaDonNhanVien]");
            DropStoredProcedure("[dbo].[SP_Get_ChietKhauDoanhThu_byDonVi]");
            DropStoredProcedure("[dbo].[SP_Get_ChietKhauHoaDon_byDonVi]");
            DropStoredProcedure("[dbo].[SP_GetAll_DMGiaBan]");
            DropStoredProcedure("[dbo].[SP_GetChietKhauDoanhThuChiTiet_byID]");
            DropStoredProcedure("[dbo].[SP_GetChietKhauDoanhThuNhanVien_byID]");
            DropStoredProcedure("[dbo].[SP_GetChietKhauHoaDon_ChiTiet]");
            DropStoredProcedure("[dbo].[SP_GetDiaryCard_ofHoaDon]");
            DropStoredProcedure("[dbo].[SP_GetHDDebit_ofKhachHang]");
            DropStoredProcedure("[dbo].[SP_GetHoaDonAndSoQuy_FromIDDoiTuong]");
            DropStoredProcedure("[dbo].[SP_GetQuyHoaDon_ofDoiTuong]");
            DropStoredProcedure("[dbo].[SP_GetSoDuTheGiaTri_ofKhachHang]");
            DropStoredProcedure("[dbo].[SP_InsertChietKhauHoaDonTraHang_NhanVien]");
            DropStoredProcedure("[dbo].[SP_ReportValueCard_Balance]");
            DropStoredProcedure("[dbo].[SP_ReportValueCard_DiaryUsed]");
            DropStoredProcedure("[dbo].[UpdateLaiSoDuTheNap]");
        }
    }
}
