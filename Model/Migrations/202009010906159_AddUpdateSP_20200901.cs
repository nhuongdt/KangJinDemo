namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20200901 : DbMigration
    {
        public override void Up()
        {
			Sql(@"CREATE FUNCTION [dbo].[Diary_BangLuong]
(	
	@ID_BangLuong uniqueidentifier
)
RETURNS nvarchar(max)  
AS
begin 

	Declare @infor nvarchar(max) =
	(select concat(N' <br /> ', tbl.noidung, N' <br /> <b> *) </b> Nội dung chi tiết', REPLACE(REPLACE( noidungct,'&lt;','<'),'&gt;','>'))
from
(
select  concat(N' <br /> Mã bảng lương: <a href=""javascript: void(0) style = ""cursor: pointer"" onclick = ""GotoBangLuongByMa(',bl.MaBangLuong,')"" > ', bl.MaBangLuong, ' </ a > ',
	N' <br /> Tên bảng lương: ', bl.TenBangLuong,
	N' <br /> Kỳ tính lương: ', FORMAT(bl.TuNgay, 'dd-MM-yyyy'), N' đến ', FORMAT(bl.DenNgay, 'dd-MM-yyyy'),
	N' <br /> Ngày công chuẩn: ', (select top 1 NgayCongChuan from NS_BangLuong_ChiTiet where ID_BangLuong = @ID_BangLuong),
	N' <br /> Người tạo: ', nv.TenNhanVien , ' (', nv.MaNhanVien , ')' ,
	N' <br /> Trạng thái: ', IIF(bl.TrangThai = 1, N' Lưu tạm', N' Đã chốt lương'),
	IIF(bl.GhiChu is null or bl.GhiChu = '', '', N',  <br /> Ghi chú: ' + bl.GhiChu)) noidung,	
	
(
		select pluong as [text()]
		from
		(
			select  CONCAT(' <br />  <b> ', ct.MaBangLuongChiTiet, ': </b>', ' <a href=""javascript:void(0)"" >', nv.MaNhanVien, ' </a> - ', nv.TenNhanVien,
						N': Ngày công thực: ', ct.NgayCongThuc,
						N', Lương cơ bản: ', FORMAT(ct.LuongCoBan, '###,###.###'),
						N', Lương chính: ', FORMAT(ct.TongLuongNhan, '###,###.###'),
						N', Lương OT: ', IIF(ct.LuongOT = 0, '0', FORMAT(ct.LuongOT, '###,###.###')),
						N', Phụ cấp cố định: ', IIF(ct.PhuCapCoBan = 0, '0', FORMAT(ct.PhuCapCoBan, '###,###.###')),
						N', Phụ cấp khác: ', IIF(ct.PhuCapKhac = 0, '0', FORMAT(ct.PhuCapKhac, '###,###.###')),
						N', Hoa hồng: ', IIF(ct.ChietKhau = 0, '0', FORMAT(ct.ChietKhau, '###,###.###')),
						N', Giảm trừ: ', IIF(ct.TongTienPhat = 0, '0', FORMAT(ct.TongTienPhat, '###,###.###')),
						N', Tổng lương: ', FORMAT(ct.LuongThucNhan, '###,###.###')) as pluong
			from NS_BangLuong_ChiTiet ct
			join NS_NhanVien nv on ct.ID_NhanVien = nv.ID
			where ct.ID_BangLuong = @ID_BangLuong
		) a
		for xml path('')		
	) noidungct
from NS_BangLuong bl
join NS_NhanVien nv on bl.ID_NhanVienDuyet = nv.ID
where bl.ID = @ID_BangLuong
) tbl)
	return @infor
end");

			Sql(@"CREATE FUNCTION [dbo].[Diary_LuongPhuCap]
(
	@ID_PhuCap uniqueidentifier
)
RETURNS nvarchar(max)
AS
BEGIN
	-- Declare the return variable here
	Declare @infor nvarchar(max) = (
	select CONCAT(N'<br /> - Loại ',txtFirst, ': ',loaipc,
				N' <br /> - Số tiền: ',txtTien,
				N' <br /> - Ngày áp dụng: ',txtNgayApDung,
				IIF(txtNgayKetThuc='','',N' <br /> - Ngày kết thúc:'+ txtNgayKetThuc),
				IIF(NoiDung is null or NoiDung ='','',N' <br /> - Nội dung: '+ NoiDung), 						
				txtFirstNangCao, REPLACE(REPLACE( thietlap,'&lt;','<'),'&gt;','>')) as abc
	from
	(

		select 
			case LoaiLuong
				when 1 then N'lương'
				when 2 then N'lương'
				when 3 then N'lương'
				when 4 then N'lương'
				when 51 then N'phụ cấp'
				when 52 then N'phụ cấp'
				when 53 then N'phụ cấp'
				when 61 then N'giảm trừ'
				when 62 then N'giảm trừ'
				when 63 then N'giảm trừ'
				else'' end as txtFirst,
			case LoaiLuong
				when 1 then N'Luơng cố định'
				when 2 then N'Luơng theo ngày'
				when 3 then N'Luơng theo ca'
				when 4 then N'Luơng theo giờ'
				when 51 then N'Phụ cấp theo ngày'
				when 52 then N'Phụ cấp cố định VND'
				when 53 then N'Phụ cấp cố định theo % lương chính'
				when 61 then N'Giảm trừ theo lần'
				when 62 then N'Giảm trừ cố định VND'
				when 63 then N'Giảm trừ theo cố định theo % tổng lương nhận'
				else '' end as loaipc,
			case when LoaiLuong = 53 or LoaiLuong= 63 then cast(SoTien as varchar(5)) +' %' else FORMAT(SoTien,'###,###,###.###') +N' đ' end as txtTien,
			FORMAT(NgayApDung,'dd/MM/yyyy') as txtNgayApDung,
			case when NgayKetThuc is null then '' else FORMAT(NgayKetThuc,'dd/MM/yyyy') end as txtNgayKetThuc,
			NoiDung,
			case when LoaiLuong in (3,4) then N'<br /> *) Thiết lập nâng cao ' else '' end txtFirstNangCao,
			-- xml content
			(		
				select concat (ThietLapNangCao ,
							 iif(LamThemGio is null or LamThemGio ='','', N'<br /> *) Làm thêm giờ '+ LamThemGio)) as [text()]				
				from
				(
					select distinct LamThemGio,ThietLapNangCao
					from
						(select 
							case when pc.LoaiLuong in (3,4) then 							
									case when ID_CaLamViec is null and ct.LaOT = 0 
										then CONCAT( N' <br /> - Mặc định: Lương cơ bản: ', FORMAT(ct.LuongNgayThuong,'###,###.###'), N'; Thứ 7:', 
											IIF(ct.Thu7_LaPhanTramLuong = 1, concat(Thu7_GiaTri, ' %'), concat(FORMAT(ct.Thu7_GiaTri,'###,###.###'),' VND')),
											 N'; Chủ nhật:',
											IIF(ct.CN_LaPhanTramLuong = 1, concat(ThCN_GiaTri,' %'), concat(FORMAT(ct.ThCN_GiaTri,'###,###.###'),' VND')),
											 N'; Ngày nghỉ:',
											IIF(ct.NgayNghi_LaPhanTramLuong = 1, concat(NgayNghi_GiaTri,' %'), concat(FORMAT(ct.NgayNghi_GiaTri,'###,###.###'),' VND')),
											 N'; Ngày lễ:',
											IIF(ct.NgayLe_LaPhanTramLuong = 1, concat(NgayLe_GiaTri,' %'), concat(FORMAT(ct.NgayLe_GiaTri,'###,###.###'),' VND')))
										else	
											case when ID_CaLamViec is not null and ct.LaOT = 0 
											then CONCAT( N' <br /> - ', ca.TenCa, N': Lương cơ bản: ', FORMAT(ct.LuongNgayThuong,'###,###.###'), N'; Thứ 7:', 
												IIF(ct.Thu7_LaPhanTramLuong = 1,concat(Thu7_GiaTri,' %'), concat(FORMAT(ct.Thu7_GiaTri,'###,###.###'),' VND')),
												 N'; Chủ nhật:',
												IIF(ct.CN_LaPhanTramLuong = 1,concat(ThCN_GiaTri,' %'), concat(FORMAT(ct.Thu7_GiaTri,'###,###.###'),' VND')),
												 N'; Ngày nghỉ:',
												IIF(ct.NgayNghi_LaPhanTramLuong = 1,concat(NgayNghi_GiaTri,' %'), concat(FORMAT(ct.NgayNghi_GiaTri,'###,###.###'),' VND')),
												 N'; Ngày lễ:',
												IIF(ct.NgayLe_LaPhanTramLuong = 1,concat(NgayLe_GiaTri,' %'), concat(FORMAT(ct.NgayLe_GiaTri,'###,###.###'),' VND')))
												end
												end
												end as ThietLapNangCao,
								case when pc.LoaiLuong in (2,3) then 							
									case when ID_CaLamViec is null and ct.LaOT = 1 
										then CONCAT( N' <br /> - Ngày thường: ', 
											IIF(ct.NgayThuong_LaPhanTramLuong = 1, concat(LuongNgayThuong, ' %'), concat(FORMAT(ct.LuongNgayThuong,'###,###.###'),' VND')),
											N', Thứ 7:', 
											IIF(ct.Thu7_LaPhanTramLuong = 1, concat(Thu7_GiaTri, ' %'), concat(FORMAT(ct.Thu7_GiaTri,'###,###.###'),' VND')),
											 N'; Chủ nhật:',
											IIF(ct.CN_LaPhanTramLuong = 1, concat(ThCN_GiaTri,' %'), concat(FORMAT(ct.ThCN_GiaTri,'###,###.###'),' VND')),
											 N'; Ngày nghỉ:',
											IIF(ct.NgayNghi_LaPhanTramLuong = 1, concat(NgayNghi_GiaTri,' %'), concat(FORMAT(ct.NgayNghi_GiaTri,'###,###.###'),' VND')),
											 N'; Ngày lễ:',
											IIF(ct.NgayLe_LaPhanTramLuong = 1, concat(NgayLe_GiaTri,' %'), concat(FORMAT(ct.NgayLe_GiaTri,'###,###.###'),' VND')))
										else	
											''
										end end as LamThemGio		
						from NS_ThietLapLuongChiTiet ct
						join NS_Luong_PhuCap pc on ct.ID_LuongPhuCap = pc.ID
						left join NS_CaLamViec ca on ct.ID_CaLamViec = ca.ID
						where ct.ID_LuongPhuCap = @ID_PhuCap
						) a
					) tlap for xml path('')
				) thietlap
		from NS_Luong_PhuCap 
		where ID= @ID_PhuCap
		) tbl
	)
	RETURN @infor

END");

			Sql(@"CREATE FUNCTION [dbo].[GetIDNhanVien_inPhongBan] (
	@ID_NhanVien UNIQUEIDENTIFIER , 
	@IDDonVi varchar(max), 
	@MaQuyenXemPhongBan varchar(100),
	@MaQuyenXemHeThong varchar(100))
RETURNS
 @tblNhanVien TABLE (ID UNIQUEIDENTIFIER)
AS
BEGIN
	
	DECLARE @tblDonVi TABLE (ID UNIQUEIDENTIFIER);
	insert into @tblDonVi
	select Name from dbo.splitstring(@IDDonVi)

	declare @LaAdmin bit=( select LaAdmin from HT_NguoiDung where ID_NhanVien = @ID_NhanVien)

	declare @countAll int = (SELECT count(*)
	FROM HT_NguoiDung_Nhom nnd
	JOIN HT_Quyen_Nhom qn on nnd.IDNhomNguoiDung = qn.ID_NhomNguoiDung
	JOIN HT_NguoiDung htnd on nnd.IDNguoiDung= htnd.ID
	where htnd.ID_NhanVien= @ID_NhanVien and qn.MaQuyen= @MaQuyenXemHeThong 
	and  exists (select ID from @tblDonVi dv where nnd.ID_DonVi = dv.ID) )

	DECLARE @tblPhongBan TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblPhongBan
	select ID_PhongBan from NS_QuaTrinhCongTac ct where ID_NhanVien= @ID_NhanVien 
	and exists (select ID from @tblDonVi dv where ct.ID_DonVi = dv.ID)

	DECLARE @tblPhongBanTemp TABLE (ID UNIQUEIDENTIFIER);
	INSERT INTO @tblPhongBanTemp
	select ID_PhongBan from NS_QuaTrinhCongTac ct where ID_NhanVien= @ID_NhanVien 
	and exists (select ID from @tblDonVi dv where ct.ID_DonVi = dv.ID)
	if @LaAdmin ='1' or @countAll > 0
	begin
		INSERT INTO @tblPhongBan
		SELECT ID FROM NS_PhongBan pb where TrangThai != 0
		and (exists (select ID from @tblDonVi dv where pb.ID_DonVi = dv.ID) or ID_DonVi is null); 

		INSERT INTO @tblNhanVien
		select distinct ct.ID_NhanVien from NS_QuaTrinhCongTac ct
		join NS_NhanVien nv on ct.ID_NhanVien = nv.ID
		where exists (select ID from @tblPhongBan pb where pb.ID= ct.ID_PhongBan)
		and (nv.TrangThai = 1 or nv.TrangThai is null) and nv.DaNghiViec= 0
		and exists (select ID from @tblDonVi dv where ct.ID_DonVi = dv.ID)
	end
	else	
		begin
			declare @countByPhong int = (SELECT count(*)
			FROM HT_NguoiDung_Nhom nnd
			JOIN HT_Quyen_Nhom qn on nnd.IDNhomNguoiDung = qn.ID_NhomNguoiDung
			JOIN HT_NguoiDung htnd on nnd.IDNguoiDung= htnd.ID
			where htnd.ID_NhanVien= @ID_NhanVien and qn.MaQuyen= @MaQuyenXemPhongBan 
			and  exists (select ID from @tblDonVi dv where nnd.ID_DonVi = dv.ID) )

			if @countByPhong > 0
				begin
					DECLARE @intFlag INT;
					SET @intFlag = 1;
					WHILE (@intFlag != 0)
					BEGIN
						SELECT @intFlag = COUNT(ID) FROM NS_PhongBan pb
						WHERE ID_PhongBanCha IN (SELECT ID FROM @tblPhongBanTemp) and TrangThai != 0
						IF(@intFlag != 0)
						BEGIN
							INSERT INTO @tblPhongBanTemp
							SELECT ID FROM NS_PhongBan pb WHERE ID_PhongBanCha IN (SELECT ID FROM @tblPhongBanTemp) and TrangThai != 0
							DELETE FROM @tblPhongBanTemp WHERE ID IN (SELECT ID FROM @tblPhongBan);
							INSERT INTO @tblPhongBan
							SELECT ID FROM @tblPhongBanTemp
						END
					END

					INSERT INTO @tblNhanVien
					select distinct ct.ID_NhanVien from NS_QuaTrinhCongTac ct
					join NS_NhanVien nv on ct.ID_NhanVien = nv.ID
					where exists (select ID from @tblPhongBan pb where pb.ID= ct.ID_PhongBan)
					and (nv.TrangThai = 1 or nv.TrangThai is null) and nv.DaNghiViec= 0
					and exists (select ID from @tblDonVi dv where ct.ID_DonVi = dv.ID)

				end
			else
				INSERT INTO @tblNhanVien values (@ID_NhanVien)
		end		
	RETURN
END");

			Sql(@"CREATE FUNCTION [dbo].[GetMaBangLuongMax_byTemp]
(
	@ID_DonVi uniqueidentifier
)
RETURNS varchar(50)
AS
BEGIN
	DECLARE @mabangluong varchar(50)
	DECLARE @LoaiHoaDon int = 24

	DECLARE @Return float = 1
	declare @lenMaMax int = 0
	DECLARE @isDefault bit = (select SuDungMaChungTu from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi)-- co/khong thiet lap su dung Ma MacDinh
	DECLARE @isSetup int = (select ID_LoaiChungTu from HT_MaChungTu where ID_LoaiChungTu = @LoaiHoaDon)-- da ton tai trong bang thiet lap chua

	if @isDefault='1' and @isSetup is not null
		begin
			DECLARE @machinhanh varchar(15) = (select MaDonVi from DM_DonVi where ID= @ID_DonVi)
			DECLARE @lenMaCN int = Len(@machinhanh)
			DECLARE @isUseMaChiNhanh varchar(15) = (select SuDungMaDonVi from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon) -- co/khong su dung MaChiNhanh
			DECLARE @kituphancach1 varchar(1) = (select KiTuNganCach1 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @kituphancach2 varchar(1) = (select KiTuNganCach2 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @kituphancach3 varchar(1) = (select KiTuNganCach3 from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @dinhdangngay varchar(8) = (select NgayThangNam from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @dodaiSTT INT = (select CAST(DoDaiSTT AS INT) from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @kihieuchungtu varchar(10) = (select MaLoaiChungTu from HT_MaChungTu where ID_LoaiChungTu=@LoaiHoaDon)
			DECLARE @lenMaKiHieu int = Len(@kihieuchungtu);
			DECLARE @namthangngay varchar(10) = convert(varchar(10), getdate(), 112)
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

			DECLARE @sMaFull varchar(50) = concat(@machinhanh,@kituphancach1,@kihieuchungtu,@kituphancach2, @datecompare, @kituphancach3)	

			declare @sCompare varchar(30) = @sMaFull
			if @sMaFull= concat(@kihieuchungtu,'_') set @sCompare = concat(@kihieuchungtu,'[_]') -- like %_% không nhận kí tự _ nên phải [_] theo quy tắc của sql

			-- lay ma max hien tai
			declare @maxCodeNow varchar(30) = (
			select top 1 MaBangLuong from NS_BangLuong 
			where MaBangLuong like @sCompare +'%'  
			order by MaBangLuong desc)
			select @Return = CAST(dbo.udf_GetNumeric(RIGHT(@maxCodeNow, LEN(@maxCodeNow) -LEN (@sMaFull))) AS float) -- lay chuoi so ben phai
	
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
				set @mabangluong = CONCAT(@sMaFull,left(@strstt,@lenSst-1),1)-- bỏ bớt 1 số 0			
			else 
				begin
					set @Return = @Return + 1
					set @lenMaMax =  len(@Return)
					set @mabangluong = (select 
						case when @lenMaMax = 1 then CONCAT(@sMaFull,left(@strstt,@lenSst-1),@Return)
							when @lenMaMax = 2 then case when @lenSst - 2 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-2), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 3 then case when @lenSst - 3 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-3), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 4 then case when @lenSst - 4 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-4), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 5 then case when @lenSst - 5 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-5), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 6 then case when @lenSst - 6 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-6), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 7 then case when @lenSst - 7 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-7), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 8 then case when @lenSst - 8 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-8), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 9 then case when @lenSst - 9 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-9), @Return) else CONCAT(@sMaFull, @Return) end
							when @lenMaMax = 10 then case when @lenSst - 10 > -1 then CONCAT(@sMaFull, left(@strstt,@lenSst-10), @Return) else CONCAT(@sMaFull, @Return) end
						else '' end)
				end 
		end
	else
		begin
			declare @machungtu varchar(10) = (select top 1 MaLoaiChungTu from DM_LoaiChungTu where ID= @LoaiHoaDon)
			declare @lenMaChungTu int= LEN(@machungtu)

			select @Return = MAX(CAST(dbo.udf_GetNumeric(RIGHT(MaBangLuong,LEN(MaBangLuong)- @lenMaChungTu))AS float))
			from NS_BangLuong where SUBSTRING(MaBangLuong, 1, len(@machungtu)) = @machungtu and CHARINDEX('O',MaBangLuong) = 0 -- not HDO, GDVO, THO, DHO
			
			-- do dai STT (toida = 10)
			if	@Return is null 
					set @mabangluong = (select
						case when @lenMaChungTu = 2 then CONCAT(@machungtu, '00000000',1)
							when @lenMaChungTu = 3 then CONCAT(@machungtu, '0000000',1)
							when @lenMaChungTu = 4 then CONCAT(@machungtu, '000000',1)
							when @lenMaChungTu = 5 then CONCAT(@machungtu, '00000',1)
						else CONCAT(@machungtu,'000000',1)
						end )
			else 
				begin
					set @Return = @Return + 1
					set @lenMaMax = len(@Return)
					set @mabangluong = (select 
						case when @lenMaMax = 1 then CONCAT(@machungtu,'000000000',@Return)
							when @lenMaMax = 2 then CONCAT(@machungtu,'00000000',@Return)
							when @lenMaMax = 3 then CONCAT(@machungtu,'0000000',@Return)
							when @lenMaMax = 4 then CONCAT(@machungtu,'000000',@Return)
							when @lenMaMax = 5 then CONCAT(@machungtu,'00000',@Return)
							when @lenMaMax = 6 then CONCAT(@machungtu,'0000',@Return)
							when @lenMaMax = 7 then CONCAT(@machungtu,'000',@Return)
							when @lenMaMax = 8 then CONCAT(@machungtu,'00',@Return)
							when @lenMaMax = 9 then CONCAT(@machungtu,'0',@Return)								
						else CONCAT(@machungtu,CAST(@Return  as decimal(22,0))) end)
				end 
		end

	RETURN @mabangluong
END");

			Sql(@"CREATE FUNCTION [dbo].[TinhNgayCongChuan]
(
	@FromDate datetime,
	@ToDate datetime,
	@IDChiNhanh uniqueidentifier
)
RETURNS int
AS
BEGIN
	
	DECLARE @ngaycongchuan  int

    declare @tongNgaySearch int = DATEDIFF(DAY, @FromDate, @ToDate) + 1
    set @ngaycongchuan = (select NgayCongChuan from HT_CongTy)
	--declare @setupCongChuan bit = '1'

	if @ngaycongchuan is null
		begin
		--set @setupCongChuan = '0'
		-- đếm số ngày thứ 2, thứ 3 ... đi làm
		declare @i int =0
		declare @mon int =0, @tues int =0, @wed int =0, @thurs int =0, @fri int= 0, @sat int =0, @sun int=0
		declare @dateFor datetime = @FromDate
		declare @dateOfweek int = 0

		while @i < @tongngaySearch
			begin
				set @dateOfweek = DATEPART(WEEKDAY,@dateFor)
				if(@dateOfweek)= 1 set @sun = @sun + 1
				if(@dateOfweek)= 2 set @mon = @mon + 1
				if(@dateOfweek)= 3 set @tues = @tues + 1
				if(@dateOfweek)= 4 set @wed = @wed + 1
				if(@dateOfweek)= 5 set @thurs = @thurs + 1
				if(@dateOfweek)= 6 set @fri = @fri + 1
				if(@dateOfweek)= 7 set @sat = @sat + 1

				set @dateFor = DATEADD(DAY,1,@dateFor)
				set @i = @i + 1
			end

			set @ngaycongchuan = (select SUM(
									IIF(Thu=0,iif(LoaiNgay =0, @sun,0),0) +
									IIF(Thu=1,iif(LoaiNgay =0, @mon,0),0) +
									IIF(Thu=2,iif(LoaiNgay =0, @tues,0),0) +
									IIF(Thu=3,iif(LoaiNgay =0, @wed,0),0) +
									IIF(Thu=4,iif(LoaiNgay =0, @thurs,0),0) +
									IIF(Thu=5,iif(LoaiNgay =0, @fri,0),0) +
									IIF(Thu=6,iif(LoaiNgay =0, @sat,0),0) )
								from NS_NgayNghiLe where Thu !=-1)		

		end
		return @ngaycongchuan
END");

			CreateStoredProcedure(name: "[dbo].[ChangeCong_UpdateNSCongBoSung]", parametersAction: p => new
			{
				ID_DonVi = p.Guid(),
				NgayChamCong = p.DateTime(),
				ID_CalamViec = p.Guid(),
				DateOfWeek = p.Int(),
				LoaiNgay = p.Int(),
				KyHieuCong = p.String(5),
				Cong = p.Double(),
				SoGioOT = p.Double(),
				SoPhutDiMuon = p.Double(),
				GhiChu = p.String(),
				NguoiTao = p.String(50),
				TrangThai = p.Int()
			}, body: @"SET NOCOUNT ON;

	--- not use
	declare @tblCongQuyDoi table(ID_NhanVien uniqueidentifier, ID_CaLamViec uniqueidentifier null, NgayApDung datetime, NgayKetThuc datetime null, CongQuyDoi float, CongOTQuyDoi float)
	insert into @tblCongQuyDoi
	exec GetCongQuyDoi @DateOfWeek,@LoaiNgay, '%%',@ID_DonVi

	update bsc set KyHieuCong = @KyHieuCong, Cong= @Cong, SoGioOT= @SoGioOT, SoPhutDiMuon= @SoPhutDiMuon, 
			GhiChu= @GhiChu, NguoiSua = @NguoiTao, NgaySua = GETDATE(), TrangThai = @TrangThai,
			CongQuyDoi = @Cong * tbl.CongQuyDoi, GioOTQuyDoi = @SoGioOT * CongOTQuyDoi
	from NS_CongBoSung bsc
	join (		
			select bs.ID, 
						case when bs.ID_CaLamViec= qd.ID_CaLamViec then qd.CongQuyDoi
					else (select top 1 CongQuyDoi 
							from @tblCongQuyDoi qd 
							where bs.ID_NhanVien= qd.ID_NhanVien and bs.NgayCham >= qd.NgayApDung and (qd.NgayKetThuc is null or  bs.NgayCham <= qd.NgayKetThuc)) end as CongQuyDoi,
					case when bs.ID_CaLamViec= qd.ID_CaLamViec then CongOTQuyDoi
					else (select top 1 CongOTQuyDoi 
							from @tblCongQuyDoi qd 
							where bs.ID_NhanVien= qd.ID_NhanVien and bs.NgayCham >= qd.NgayApDung and (qd.NgayKetThuc is null or  bs.NgayCham <= qd.NgayKetThuc)) end as CongOTQuyDoi
					from NS_CongBoSung bs
					left join @tblCongQuyDoi qd on bs.ID_NhanVien = qd.ID_NhanVien 
					where bs.NgayCham = @NgayChamCong
					and bs.ID_DonVi = @ID_DonVi
					and qd.NgayApDung <= @NgayChamCong and (qd.NgayKetThuc is null or qd.NgayKetThuc >=@NgayChamCong)
					and bs.ID_CaLamViec= @ID_CaLamViec					
		) tbl on bsc.ID= tbl.ID");

			CreateStoredProcedure(name: "[dbo].[CheckSameTime_CaLamViec]", parametersAction: p => new
			{
				ID_DonVi = p.String(40),
				ID_PhieuPhanCa = p.String(40),
				LoaiPhanCa = p.String(3),
				DateOfWeeks = p.String(50),
				IDNhanViens = p.String(),
				IDCaLamViecs = p.String(),
				TuNgay = p.DateTime(),
				DenNgay = p.DateTime(null)
			}, body: @"SET NOCOUNT ON;

		set @TuNgay = format(@TuNgay,'yyyy-MM-dd');
		if @DenNgay is null
			set @DenNgay = format(DATEADD(year, 1, getdate()),'yyyy-MM-dd');
		else 
			set @DenNgay = format(@DenNgay,'yyyy-MM-dd');

		declare @tblCa table(ID_CaLamViec uniqueidentifier)
		insert into @tblCa
		select name from dbo.splitstring(@IDCaLamViecs)

			select distinct nv.MaNhanVien, nv.TenNhanVien, tbl.ID, tbl.MaPhieu, tbl.LoaiPhanCa
			from
				(select phieu3.*, canv.ID_NhanVien, caphieu.GiaTri
				from
					(select *
					from
						(select phieu.ID, phieu.MaPhieu, phieu.LoaiPhanCa, 
							FORMAT(ISNULL(phieu.TuNgay, DATEADD(year, 1, getdate())),'yyyy-MM-dd') as TuNgay,
							FORMAT(ISNULL(phieu.DenNgay, DATEADD(year, 1, getdate())),'yyyy-MM-dd') as DenNgay
						from NS_PhieuPhanCa phieu 
						where phieu.ID_DonVi like @ID_DonVi
						and phieu.TrangThai !='0'
						) phieu2
						where phieu2.TuNgay <= @DenNgay and @TuNgay <= phieu2.DenNgay
					) phieu3
				join NS_PhieuPhanCa_NhanVien canv on phieu3.ID= canv.ID_PhieuPhanCa
				join NS_PhieuPhanCa_CaLamViec caphieu on canv.ID_PhieuPhanCa = caphieu.ID_PhieuPhanCa
				where exists (select name from dbo.splitstring(@IDNhanViens) tblNV where canv.ID_NhanVien= tblNV.Name)	
				and @ID_PhieuPhanCa != phieu3.ID
				-- check trungca 1.catuan, 3.cacodinh
				and ((@LoaiPhanCa = 3 and (phieu3.LoaiPhanCa= 1
										OR exists( select ID_CaLamViec from @tblCa tblCa where caphieu.ID_CaLamViec = tblCa.ID_CaLamViec))			
					OR (@LoaiPhanCa = 1  
							and (phieu3.LoaiPhanCa = 3
								OR (exists( select ID_CaLamViec from @tblCa tblCa where caphieu.ID_CaLamViec = tblCa.ID_CaLamViec) 
									and exists (select Name from dbo.splitstring(@DateOfWeeks) tblDate where caphieu.GiaTri= tblDate.Name))
									)
						)
					))
			) tbl
			join NS_NhanVien nv on tbl.ID_NhanVien= nv.ID");

			CreateStoredProcedure(name: "[dbo].[ChiTietTraHang_insertChietKhauNV]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid()
			}, body: @"SET NOCOUNT ON;
	insert into BH_NhanVienThucHien
	select newid(), th.ID_NhanVien, cttra.ID, 
	-- important: neu chiet khau theo VND --> khong nhan voi HeSo
	case when PT_ChietKhau = 0 then (th.TienChietKhau / ctmua.ThanhTien ) * cttra.ThanhTien else th.PT_ChietKhau/100 *th.HeSo * cttra.ThanhTien end as TienChietKhau,
	th.TheoYeuCau, th.PT_ChietKhau, th.ThucHien_TuVan, null, th.TinhChietKhauTheo, th.HeSo, null
	from BH_NhanVienThucHien th
	join BH_HoaDon_ChiTiet ctmua on th.ID_ChiTietHoaDon = ctmua.id
	join BH_HoaDon_ChiTiet cttra on ctmua.ID= cttra.ID_ChiTietGoiDV
	join BH_HoaDon hd on ctmua.ID_HoaDon= hd.ID
	where hd.ID=@ID_HoaDon ");

			CreateStoredProcedure(name: "[dbo].[ExportBangCongNhanVien]", parametersAction: p => new
			{
				ID_NhanVien = p.Guid(),
				IDDonVis = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;
	declare @tblDonVi table(ID uniqueidentifier)
	insert into @tblDonVi
	select * from dbo.splitstring(@IDDonVis);
	
			with data_cte
			as(
			select piv.ID_NhanVien,piv.ID_CaLamViec, piv.ID_DonVi,
				[1] as Ngay1, [2] as Ngay2,[3] as Ngay3, [4] as Ngay4, [5] as Ngay5, [6] as Ngay6,[7] as Ngay7, [8] as Ngay8, [9] as Ngay9,
				[10] as Ngay10, [11] as Ngay11, [12] as Ngay12,[13] as Ngay13, [14] as Ngay14, [15] as Ngay15, [16] as Ngay16,[17] as Ngay17, [18] as Ngay18, [19] as Ngay19,
				[20] as Ngay20, [21] as Ngay21, [22] as Ngay22,[23] as Ngay23, [24] as Ngay24, [25] as Ngay25, [26] as Ngay26,[27] as Ngay27, [28] as Ngay28, [29] as Ngay29,
				[30] as Ngay30, [31] as Ngay31
			from
			(
			select bs.ID_NhanVien, bs.ID_CaLamViec, bs.ID_DonVi, 
				DATEPART(DAY, bs.NgayCham) as Ngay,
				bs.KyHieuCong		
			from NS_CongBoSung bs
			where bs.ID_NhanVien= @ID_NhanVien
			and exists (select ID from @tblDonVi dv where bs.ID_DonVi= dv.ID)
			and bs.NgayCham >= @FromDate and bs.NgayCham <=@ToDate
			) a
			PIVOT (
				max(KyHieuCong)
				FOR Ngay in ( [1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19], [20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31]) 
				) piv 
			)

			SELECT cong.MaCa, cong.TenCa, cong.GioVao, cong.GioRa, cte.*, 
				cast(TongCong as float) as TongCong,   
				cast(TongCongNgayNghi as float) as TongCongNgayNghi,  
				cast(TongOT as float) as TongOT,
				cast(TongPhutDiMuon as float) as TongPhutDiMuon
			FROM data_cte cte
			join (select ca.MaCa, ca.TenCa, ca.GioVao, ca.GioRa,  bs.ID_NhanVien, bs.ID_CaLamViec, bs.ID_DonVi,
						sum(IIF(bs.LoaiNgay=0, bs.Cong,0)) as TongCong,
						sum(IIF(bs.LoaiNgay!=0, bs.Cong,0)) as TongCongNgayNghi,
						sum(bs.SoGioOT) as TongOT,
						sum(bs.SoPhutDiMuon) as TongPhutDiMuon
				from NS_CongBoSung bs 
				join NS_CaLamViec ca on bs.ID_CaLamViec= ca.ID
				where bs.ID_NhanVien= @ID_NhanVien
					and exists (select ID from @tblDonVi dv where bs.ID_DonVi= dv.ID)
					and bs.NgayCham >= @FromDate and bs.NgayCham <=@ToDate
				group by bs.ID_NhanVien, bs.ID_CaLamViec, bs.ID_DonVi,ca.MaCa, ca.TenCa,ca.GioVao, ca.GioRa
				) cong on cte.ID_NhanVien = cong.ID_NhanVien and cte.ID_CaLamViec= cong.ID_CaLamViec and cte.ID_DonVi= cong.ID_DonVi
				order by cong.MaCa");

			CreateStoredProcedure(name: "[dbo].[GetAllBangLuong]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TxtSearch = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				TrangThais = p.String(10),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TxtSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	DECLARE @tblChiNhanh TABLE (ID uniqueidentifier)
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@IDChiNhanhs);

	with data_cte
	as (
	select bl.*, soquy.DaTra,soquy.LuongThucNhan, soquy.LuongThucNhan - soquy.DaTra as ConLai
	from NS_BangLuong bl
	join (select ct.ID_BangLuong,
				sum(ct.LuongThucNhan) as LuongThucNhan,
				sum(isnull(soquy.DaTra,0)) as DaTra
			from NS_BangLuong_ChiTiet ct
			left join( select qct.ID_BangLuongChiTiet , sum(qct.TienThu) + sum(ISNULL(qct.TruTamUngLuong,0)) as DaTra
						from Quy_HoaDon_ChiTiet qct  
						join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID where qhd.TrangThai= 1 
						group by  qct.ID_BangLuongChiTiet) soquy on ct.ID= soquy.ID_BangLuongChiTiet					
			group by ct.ID_BangLuong
			) soquy on bl.ID = soquy.ID_BangLuong
	where exists (select Name from dbo.splitstring(@TrangThais) tt where bl.TrangThai = tt.Name)
	and exists (select ID from @tblChiNhanh dv where bl.ID_DonVi= dv.ID)
	and ((bl.TuNgay >= @FromDate and (bl.TuNgay <= @ToDate or bl.DenNgay <= @ToDate))
		or ( bl.DenNgay <= @ToDate and ( bl.DenNgay >= @FromDate or bl.TuNgay >= @FromDate))
			)
	AND ((select count(Name) from @tblSearchString b where 
    					bl.MaBangLuong like '%'+b.Name+'%' 
    					or bl.TenBangLuong like '%'+b.Name+'%' 
						or bl.GhiChu like '%'+b.Name+'%' 
    				
						)=@count or @count=0)	
	),
	count_cte
	as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage,
				sum(LuongThucNhan) as TongPhaiTra,				
				sum(DaTra) as TongDaTra,
				sum(ConLai) as TongConLai
			from data_cte
		)
		select dt.*, cte.*, ISNULL(nv.TenNhanVien,'') as NguoiDuyet
		from data_cte dt
		left join NS_NhanVien nv on dt.ID_NhanVienDuyet = nv.ID
		cross join count_cte cte
		order by dt.NgayTao desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetBangCongChiTiet]", parametersAction: p => new
			{
				ID_NhanVien = p.String(),
				IDChiNhanhs = p.String(),
				IDCaLamViecs = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @tblCa table (ID_CaLamViec uniqueidentifier);
	if @IDCaLamViecs='' or @IDCaLamViecs='%%'
		insert into @tblCa
		select ID from NS_CaLamViec
	else
		insert into @tblCa
		select Name from dbo.splitstring(@IDCaLamViecs);

		with data_cte
		as(
		select bs.ID, bs.ID_CaLamViec as ID_Ca, ca.TongGioCong as TongGioCong1Ca, ca.MaCa, ca.TenCa,  ca.GioVao, ca.GioRa, 
				bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon,
				bs.NgayTao, bs.NguoiTao, bs.GhiChu
			from NS_CongBoSung bs
			join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
			where bs.ID_NhanVien= @ID_NhanVien
			and bs.NgayCham>= @FromDate and bs.NgayCham <=@ToDate
			and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where bs.ID_DonVi= dv.Name)
			--and exists (select ID_CaLamViec from @tblCa ca2 where ct.ID_CaLamViec= ca2.ID_CaLamViec)
			),
			count_cte 
			as(
				select count(id) as TotalRow,
						CEILING(COUNT(id) / CAST(@PageSize as float )) as TotalPage,
						Sum(SoGioOT) as TongSoGioOT,
						cast(Sum(SoPhutDiMuon) as float) as TongSoPhutDiMuon
				from data_cte
			)
			select *
			from data_cte dt
			cross join count_cte ct 
			order by dt.NgayCham
			offset (@CurrentPage * @PageSize) Rows
			fetch next @PageSize Rows only");

			CreateStoredProcedure(name: "[dbo].[GetBangCongNhanVien]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				ID_NhanVienLogin = p.Guid(),
				IDPhongBans = p.String(),
				IDCaLamViecs = p.String(),
				TextSearch = p.String(),
				FromDate = p.String(10),
				ToDate = p.String(10),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDChiNhanhs,'BangCong_XemDS_PhongBan','BangCong_XemDS_HeThong');

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
				and exists(select ID_CaLamViec from @tblNhanVien nv where bs.ID_NhanVien= nv.ID)
				and exists(select ID_CaLamViec from @tblca ca where bs.ID_CaLamViec= ca.ID_CaLamViec)
				and exists(select Name from dbo.splitstring(@IDChiNhanhs) dv where bs.ID_DonVi= dv.Name)
				) cong
			group by cong.ID_NhanVien
			) congnv
			join NS_NhanVien nv on congnv.ID_NhanVien= nv.ID 
			join
				( select nv.ID, nv.MaNhanVien, nv.TenNhanVien
				from NS_NhanVien nv 
				left join NS_QuaTrinhCongTac ct on nv.ID= ct.ID_NhanVien
				where exists (select ID from @tblPhong pb where pb.ID= ct.ID_PhongBan)
				and exists(select Name from dbo.splitstring(@IDChiNhanhs) dv where ct.ID_DonVi= dv.Name)
				) congtac on nv.ID= congtac.ID
			WHERE ((select count(Name) from @tblSearchString b 
				where nv.TenNhanVien like '%'+b.Name+'%'  						
				or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
				or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
				or nv.MaNhanVien like '%'+b.Name+'%'
				)=@count or @count=0)	
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
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetBangLuongChiTiet_ofNhanVien]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				IDNhanVien = p.Guid(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @tblChiNhanh table (ID uniqueidentifier)
	insert into @tblChiNhanh
	select Name from dbo.splitstring(@IDChiNhanhs);

	with data_cte
		as(

		select blct.*,
			isnull(quyhd.DaTra,0) as DaTra ,
			round(blct.LuongSauGiamTru - isnull(quyhd.DaTra,0),3) as ConLai
		from
		(select bl.NgayTao,
				bl.TuNgay,
				bl.DenNgay,
				ct.ID as ID_BangLuong_ChiTiet,
				ct.MaBangLuongChiTiet,
				ct.NgayCongThuc,
				ct.NgayCongChuan,
				ct.LuongCoBan,
				ct.TongLuongNhan as LuongChinh, 
				ct.LuongOT,
				ct.PhuCapCoBan,
				ct.PhuCapKhac,
				ct.KhenThuong,
				ct.KyLuat,
				ct.ChietKhau,    					
				ct.TongLuongNhan +  ct.LuongOT + ct.PhuCapCoBan + ct.PhuCapKhac + ChietKhau  as LuongTruocGiamTru,
				ct.TongTienPhat,
    			ct.LuongThucNhan as LuongSauGiamTru
			
		from NS_BangLuong_ChiTiet ct
		join NS_BangLuong bl on ct.ID_BangLuong= bl.ID
		where ct.ID_NhanVien= @IDNhanVien
		and exists (select ID from @tblChiNhanh dv where bl.ID_DonVi = dv.ID)
		and bl.TrangThai = 4 
		) blct
		left join ( select qct.ID_BangLuongChiTiet,
							sum(qct.TienThu) +sum( isnull(qct.TruTamUngLuong,0)) as DaTra
					 from Quy_HoaDon_ChiTiet qct 
					 join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID 
					 where qhd.TrangThai = 1
					 and qct.ID_NhanVien= @IDNhanVien
					 and exists (select ID from @tblChiNhanh dv where qhd.ID_DonVi = dv.ID)
					 group by qct.ID_BangLuongChiTiet) quyhd on blct.ID_BangLuong_ChiTiet = quyhd.ID_BangLuongChiTiet		 

	),
	count_cte
		as (
			select count(ID_BangLuong_ChiTiet) as TotalRow,
				CEILING(COUNT(ID_BangLuong_ChiTiet) / CAST(@PageSize as float ))  as TotalPage,
				sum(NgayCongThuc) as TongNgayCongThuc,
				sum(LuongChinh) as TongLuongChinh,
				sum(LuongOT) as TongLuongOT,
				sum(PhuCapCoBan) as TongPhuCapCoBan,
				sum(PhuCapKhac) as TongPhuCapKhac,
				sum(KhenThuong) as TongKhenThuong,
				sum(KyLuat) as TongKyLuat,
				sum(ChietKhau) as TongChietKhau,
				sum(LuongTruocGiamTru) as TongLuongTruocGiamTru,
				sum(TongTienPhat) as TongTienPhatAll,
				sum(LuongSauGiamTru) as TongLuongSauGiamTru,
				sum(DaTra) as TongDaTra,
				sum(ConLai) as TongConLai
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.MaBangLuongChiTiet desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetChiTietCongThuCong]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				IDNhanViens = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;
	if @IDNhanViens	= ''
		select null,bs.ID_CaLamViec, ca.TongGioCong as TongGioCong1Ca, ca.TenCa, bs.ID_NhanVien,
				bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon, bs.Thu
			from NS_CongBoSung bs
			join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
			where NgayCham >= @FromDate and NgayCham <= @ToDate and bs.ID_DonVi = @IDChiNhanhs
			and bs.TrangThai in (1,2)-- 1.taomoi, 2.tamluu bangluong
	else
	 select null, bs.ID_CaLamViec, ca.TongGioCong as TongGioCong1Ca, ca.TenCa, bs.ID_NhanVien,
				bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.Cong, bs.CongQuyDoi, bs.SoGioOT, bs.GioOTQuyDoi, bs.SoPhutDiMuon, bs.Thu
			from NS_CongBoSung bs
			join NS_CaLamViec ca on bs.ID_CaLamViec = ca.ID
			where NgayCham >= @FromDate and NgayCham <= @ToDate and bs.ID_DonVi = @IDChiNhanhs
			and bs.TrangThai in (1,2) 
			and exists (select Name from dbo.splitstring(@IDNhanViens) nv where bs.ID_NhanVien= nv.Name)");

			CreateStoredProcedure(name: "[dbo].[GetCongQuyDoi]", parametersAction: p => new
			{
				DateOfWeek = p.Int(),
				LoaiNgay = p.Int(),
				ID_NhanVien = p.String(40),
				ID_DonVi = p.Guid()
			}, body: @"SET NOCOUNT ON;

	select 
				a.ID_NhanVien, a.ID_CaLamViec, 
				a.NgayApDung, a.NgayKetThuc,
				case when a.LoaiLuong = 1 or a.LoaiLuong = 2 then 1
					else case when a.LaPhanTram = 0 then 1 else a.GiaTri/100 end end as CongQuyDoi,				
				case when a.LoaiLuong = 1 or a.LoaiLuong = 4 then 1
					else case when b.LaPhanTram = 0 then 1 else ISNULL(b.GiaTri/100,1) end end as CongOTQuyDoi
			from
				(select 
					pc.LoaiLuong,
					pc.ID_NhanVien, 
					pc.NgayApDung,
					pc.NgayKetThuc,
					ct.LaOT,		
					ISNULL(ct.ID_CaLamViec,'00000000-0000-0000-0000-000000000000') as ID_CaLamViec,				
					case @DateOfWeek
						when 6 then ct.Thu7_GiaTri
						when 0 then ct.ThCN_GiaTri
					else
						case @LoaiNgay 
							when 0 then 100
							when 1 then ct.NgayNghi_GiaTri
							when 2 then ct.NgayLe_GiaTri
							end
						end as GiaTri,
					case @DateOfWeek
						when 6 then ct.Thu7_LaPhanTramLuong
						when 0 then ct.CN_LaPhanTramLuong
					else
						case @LoaiNgay 
							when 0 then 1
							when 1 then ct.NgayNghi_LaPhanTramLuong
							when 2 then ct.NgayLe_LaPhanTramLuong
							end
						end as LaPhanTram		
				from NS_ThietLapLuongChiTiet ct
				join NS_Luong_PhuCap pc on ct.ID_LuongPhuCap= pc.ID
				where  LaOT= 0
				and pc.ID_NhanVien like @ID_NhanVien
				and pc.ID_DonVi= @ID_DonVi
				) a
			left join
				(
				-- get otquydoi
				select 
					pc.LoaiLuong,
					pc.ID_NhanVien, 
					pc.NgayApDung,
					pc.NgayKetThuc,
					ct.LaOT,
					'00000000-0000-0000-0000-000000000000' as ID_CaLamViec,			
					case @dateOfWeek
						when 6 then ct.Thu7_GiaTri
						when 0 then ct.ThCN_GiaTri
					else
						case @LoaiNgay 
							when 0 then ct.LuongNgayThuong
							when 1 then ct.NgayNghi_GiaTri
							when 2 then ct.NgayLe_GiaTri
							end
						end as GiaTri,
					case @DateOfWeek
						when 6 then ct.Thu7_LaPhanTramLuong
						when 0 then ct.CN_LaPhanTramLuong
					else
						case @LoaiNgay 
							when 0 then ct.NgayThuong_LaPhanTramLuong
							when 1 then ct.NgayNghi_LaPhanTramLuong
							when 2 then ct.NgayLe_LaPhanTramLuong
							end
						end as LaPhanTram		
				from NS_ThietLapLuongChiTiet ct
				join NS_Luong_PhuCap pc on ct.ID_LuongPhuCap= pc.ID
				where LaOT= 1
				and pc.ID_DonVi= @ID_DonVi
				and pc.ID_NhanVien like @ID_NhanVien
			) b on a.ID_NhanVien= b.ID_NhanVien and a.NgayApDung= b.NgayApDung and (a.NgayKetThuc= b.NgayKetThuc OR (a.NgayKetThuc is null and b.NgayKetThuc is null))
			order by ID_NhanVien");

			CreateStoredProcedure(name: "[dbo].[GetDanhSachMayChamCongTheoChiNhanh]", parametersAction: p => new
			{
				ListIDChiNhanh = p.String()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF(@ListIDChiNhanh != '')
	BEGIN
		SELECT
		mcc.ID AS ID,
		mcc.MaMCC AS MaMCC,
		mcc.TenMCC AS TenMCC,
		mcc.TenHienThi AS TenHienThi,
		mcc.IP AS IP,
		dv.MaDonVi AS MaChiNhanh,
		dv.TenDonVi AS TenChiNhanh,
		mcc.SoSeries AS SoSeries,
		mcc.GhiChu AS GhiChu,
		mcc.ID_ChiNhanh AS IDChiNhanh,
		mcc.LoaiKetNoi AS LoaiKetNoi,
		mcc.CongCOM AS CongCOM,
		mcc.TocDoCOM AS TocDoCOM,
		mcc.LoaiHinh AS LoaiHinh,
		mcc.MatMa AS MatMa,
		mcc.Port AS Port,
		mcc.SoDangKy AS IDMay
		FROM NS_MayChamCong mcc
		INNER JOIN DM_DonVi dv ON mcc.ID_ChiNhanh = dv.ID
		INNER JOIN splitstring(@ListIDChiNhanh) cnin ON cnin.Name = dv.ID
		ORDER BY TenHienThi
	END
	ELSE
	BEGIN
		SELECT 
		mcc.ID AS ID,
		mcc.MaMCC AS MaMCC,
		mcc.TenMCC AS TenMCC,
		mcc.TenHienThi AS TenHienThi,
		mcc.IP AS IP,
		dv.MaDonVi AS MaChiNhanh,
		dv.TenDonVi AS TenChiNhanh,
		mcc.SoSeries AS SoSeries,
		mcc.GhiChu AS GhiChu,
		mcc.ID_ChiNhanh AS IDChiNhanh,
		mcc.LoaiKetNoi AS LoaiKetNoi,
		mcc.CongCOM AS CongCOM,
		mcc.TocDoCOM AS TocDoCOM,
		mcc.LoaiHinh AS LoaiHinh,
		mcc.MatMa AS MatMa,
		mcc.Port AS Port,
		mcc.SoDangKy AS IDMay
		FROM NS_MayChamCong mcc
		INNER JOIN DM_DonVi dv ON mcc.ID_ChiNhanh = dv.ID
		ORDER BY TenHienThi
	END");

			CreateStoredProcedure(name: "[dbo].[GetDuLieuChamCong]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				IDPhongBans = p.String(),
				IDCaLamViecs = p.String(),
				TextSearch = p.String(),
				FromDate = p.String(10),
				ToDate = p.String(10),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);

	declare @tblDonVi table(ID uniqueidentifier)
	insert into @tblDonVi
	select name from dbo.splitstring(@IDChiNhanhs)

	declare @tblPhong table(ID uniqueidentifier)
	if @IDPhongBans=''	
		insert into @tblPhong
		select ID from NS_PhongBan
	else
		insert into @tblPhong
		select name from dbo.splitstring(@IDPhongBans)

	declare @tblCaLamViec table(ID uniqueidentifier)
	if @IDCaLamViecs='%%' OR  @IDCaLamViecs=''
		insert into @tblCaLamViec
		select ca.ID from NS_CaLamViec ca
		join NS_CaLamViec_DonVi dvca on ca.id= dvca.ID_CaLamViec
		where exists (select ID from @tblDonVi dv where dv.ID= dvca.ID_DonVi)
	else
		insert into @tblCaLamViec
		select name from dbo.splitstring(@IDCaLamViecs);

    with data_cte
	as (
		select 
				nv.MaNhanVien,
				nv.TenNhanVien,
				ca.MaCa,
				ca.TenCa,
				format(ca.GioVao,'HH:mm') as GioVao,
				format(ca.GioRa,'HH:mm') as GioRa,
				tblView.TuNgay,
				tblView.DenNgay,
				tblView.ID_PhieuPhanCa,
				tblView.ID_NhanVien,
				tblView.ID_CaLamViec,
				tblView.Thang,
				tblView.Nam,
				tblView.Ngay1, Ngay2, Ngay3, ngay4, ngay5, Ngay6, Ngay7, Ngay8, Ngay9, 
				Ngay10, Ngay11, Ngay12,ngay13, ngay14, Ngay15,ngay16, ngay17, Ngay18,Ngay19,
				Ngay20, Ngay21, Ngay22,ngay23, ngay24, Ngay25,ngay26, ngay27, Ngay28,Ngay29,
				Ngay30, Ngay31,
				case when Format1 >= TuNgay and Format1 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format1) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay1,
				case when Format2 >= TuNgay and Format2 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format2) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay2,
				case when Format3 >= TuNgay and Format3 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format3) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay3,
				case when Format4 >= TuNgay and Format4 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format4) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay4,
				case when Format5 >= TuNgay and Format5 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format5) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay5,
				case when Format6 >= TuNgay and Format6 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format6) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay6,
				case when Format7 >= TuNgay and Format7 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format7) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay7,
				case when Format8 >= TuNgay and Format8 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format8) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay8,
				case when Format9 >= TuNgay and Format9 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format9) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay9,							
				case when Format10 >= TuNgay and Format10 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format10) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay10,
				case when Format11 >= TuNgay and Format11 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format11) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay11,
				case when Format12 >= TuNgay and Format12 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format12) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay12,
				case when Format13 >= TuNgay and Format13 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format13) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay13,
				case when Format14 >= TuNgay and Format14 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format14) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay14,
				case when Format15 >= TuNgay and Format15 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format15) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay15,
				case when Format16 >= TuNgay and Format16 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format16) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay16,
				case when Format17 >= TuNgay and Format17 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format17) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay17,
				case when Format18 >= TuNgay and Format18 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format18) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay18,
				case when Format19 >= TuNgay and Format19 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format19) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay19, 

	
				case when Format20 >= TuNgay and Format20 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format20) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay20,
				case when Format21 >= TuNgay and Format21 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format21) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay21,
				case when Format22 >= TuNgay and Format22 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format22) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay22,
				case when Format23 >= TuNgay and Format23 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format23) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay23,
				case when Format24 >= TuNgay and Format24 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format24) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay24,
				case when Format25 >= TuNgay and Format25 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format25) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay25,
				case when Format26 >= TuNgay and Format26 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format26) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay26,
				case when Format27 >= TuNgay and Format27 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format27) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay27,
				case when Format28 >= TuNgay and Format28 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format28) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay28,
				case when Format29 >= TuNgay and Format29 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format29) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay29,

				case when Format30 >= TuNgay and Format30 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format30) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay30,
				case when Format31 >= TuNgay and Format31 <= DenNgay then case when LoaiPhanCa= 3 then '0' else 
					case when DATEPART(WEEKDAY,Format31) = dbo.GetDayOfWeek_byPhieuPhanCa(ID_PhieuPhanCa, ID_CaLamViec) then '0' else '1' end end else '1' end as DisNgay31				
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
					max(Ngay1) as Ngay1,max(Ngay2) as Ngay2,max(Ngay3) as Ngay3,max(Ngay4) as Ngay4,max(Ngay5) as Ngay5,
					max(Ngay6) as Ngay6,max(Ngay7) as Ngay7,max(Ngay8) as Ngay8,max(Ngay9) as Ngay9, max(Ngay10) as Ngay10,
					max(Ngay11) as Ngay11,max(Ngay12) as Ngay12,max(Ngay13) as Ngay13,max(Ngay14) as Ngay14,max(Ngay15) as Ngay15,
					max(Ngay16) as Ngay16,max(Ngay17) as Ngay17,max(Ngay18) as Ngay18,max(Ngay19) as Ngay19, max(Ngay20) as Ngay20,
					max(Ngay21) as Ngay21,max(Ngay22) as Ngay22,max(Ngay23) as Ngay23,max(Ngay24) as Ngay24,max(Ngay25) as Ngay25,
					max(Ngay26) as Ngay26,max(Ngay27) as Ngay27,max(Ngay28) as Ngay28,max(Ngay29) as Ngay29, 
					max(Ngay30) as Ngay30, max(Ngay31) as Ngay31

				from
				(
					select phieu.ID,  phieu.Nam, phieu.Thang, nvphieu.ID_NhanVien, ca.ID as ID_CaLamViec ,
						null as Ngay1, null as Ngay2, null as Ngay3,  null as Ngay4, null as Ngay5, null as Ngay6,null as Ngay7, null as Ngay8, null as Ngay9 , 
						null as Ngay10, null as Ngay11, null as Ngay12,null as Ngay13, null as Ngay14, null as Ngay15,null as Ngay16, null as Ngay17, null as Ngay18,null as Ngay19,
						null as Ngay20, null as Ngay21, null as Ngay22,null as Ngay23, null as Ngay24, null as Ngay25,null as Ngay26, null as Ngay27, null as Ngay28,null as Ngay29,
						null as Ngay30, null as Ngay31
					from NS_PhieuPhanCa_NhanVien nvphieu 				
					join (select ID, datepart(year,TuNgay) as Nam, 
							case when DenNgay is null then datepart(month,@FromDate)  else 
							case when TuNgay < @FromDate then datepart(month,@FromDate) else datepart(month,TuNgay) end end as Thang
						from  NS_PhieuPhanCa
						where TrangThai = 1  and ((DenNgay is null and TuNgay <= @ToDate ) 
							OR ((DenNgay is not null 
								and ((DenNgay <= @ToDate and DenNgay >=  @FromDate )
									or (DenNgay >= @ToDate  and TuNgay <= @ToDate)))))
						) phieu on nvphieu.ID_PhieuPhanCa = phieu.ID						
					join NS_PhieuPhanCa_CaLamViec caphieu on nvphieu.ID_PhieuPhanCa = caphieu.ID_PhieuPhanCa
					join NS_CaLamViec ca on ca.ID= caphieu.ID_CaLamViec
							--where exists (select ID from @tblCaLamViec ca2 where ca.ID= ca2.ID)

					union all

					select piv.ID_PhieuPhanCa, piv.Nam,  piv.Thang, piv.ID_NhanVien, piv.ID_CaLamViec, [1] as Ngay1, [2] as Ngay2,[3] as Ngay3, [4] as Ngay4, [5] as Ngay5, [6] as Ngay6,[7] as Ngay7, [8] as Ngay8, [9] as Ngay9,
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
					where phieu.TrangThai = 1 
						and ((DenNgay is null and TuNgay <= @ToDate) 
							OR ((DenNgay is not null 
								and ((DenNgay <= @ToDate and DenNgay >= @FromDate )
								or (DenNgay >= @ToDate and TuNgay <= @ToDate )))))
					and bs.NgayCham >= @FromDate and bs.NgayCham <=@ToDate
					) a
					PIVOT (
					  max(KyHieuCong)
					  FOR Ngay in ( [1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19], [20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31]) 
					) piv 
				) tblunion
				group by  tblunion.ID,tblunion.ID_NhanVien, tblunion.Nam, tblunion.Thang, tblunion.ID_CaLamViec
			) tblRow
			join NS_PhieuPhanCa phieu on tblRow.ID_PhieuPhanCa = phieu.ID
		) tblView 
	join NS_CaLamViec ca on tblView.ID_CaLamViec = ca.ID
	join NS_NhanVien nv on tblView.ID_NhanVien= nv.ID 
	join ( select nv.ID, nv.MaNhanVien, nv.TenNhanVien
			from NS_NhanVien nv 
			left join NS_QuaTrinhCongTac ct on nv.ID= ct.ID_NhanVien
			where exists (select ID from @tblDonVi dv where dv.ID= ct.ID_DonVi) 
			AND exists (select ID from @tblPhong pb where pb.ID= ct.ID_PhongBan) 
			) nv2 on nv.ID= nv2.ID 
	where exists (select ID from @tblCaLamViec ca2 where ca.ID= ca2.ID)
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
			CEILING(COUNT(*) / CAST(@PageSize as float )) as TotalPage 
	from data_cte
	)
	select dt.*, cte.*
	from data_cte dt
	cross join count_cte cte
	order by dt.TuNgay
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetDuLieuCongThoTheoThang]", parametersAction: p => new
			{
				IDMayChamCong = p.Guid(),
				InMonth = p.Int(),
				InYear = p.Int(),
				PageSize = p.Int(),
				PageNum = p.Int()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @MaxRowNum INT;
	WITH DuLieuCongTho AS
	(
		select MaChamCong, ThoiGian, ID_MCC, ROW_NUMBER() OVER (ORDER BY ThoiGian) AS RowNum, 1 AS InnerJoinValue from NS_DuLieuCongTho 
		where MONTH(ThoiGian) = @InMonth AND YEAR(ThoiGian) = @InYear AND ID_MCC = @IDMayChamCong
	)
	SELECT ct.MaChamCong, mcc.TenHienThi AS TenMayChamCong, dv.MaDonVi, dv.TenDonVi, mcc.IP AS IPDomain, ct.ThoiGian, CAST(ct.RowNum AS INT) AS RowNum, CAST(m.MaxRow AS INT) AS MaxRow FROM DuLieuCongTho ct
	INNER JOIN (SELECT MAX(RowNum) AS MaxRow, 1 AS InnerJoinValue FROM DuLieuCongTho) as m ON m.InnerJoinValue = ct.InnerJoinValue
	INNER JOIN NS_MayChamCong mcc ON ct.ID_MCC = mcc.ID
	INNER JOIN DM_DonVi dv ON dv.ID = mcc.ID_ChiNhanh
	WHERE RowNum BETWEEN (@PageNum - 1)* @PageSize + 1 AND @PageNum * @PageSize
	ORDER BY RowNum");

			Sql(@"CREATE TYPE CongThuCong AS TABLE 
(	

	ID_ChamCongChiTiet uniqueidentifier, 
	ID_CaLamViec uniqueidentifier, 
	TongGioCong1Ca float,
	TenCa nvarchar(max), 
	ID_NhanVien uniqueidentifier,	
	NgayCham datetime, LoaiNgay int, 
	KyHieuCong nvarchar(10) , 
	Cong float, 
	CongQuyDoi float, 
	SoGioOT float, 
	GioOTQuyDoi float, 
	SoPhutDiMuon float, 
	Thu int
);

CREATE TYPE ThietLapLuong AS TABLE 
(		
	ID_NhanVien uniqueidentifier, 
	ID_PhuCap uniqueidentifier,
	TenLoaiLuong nvarchar(max), 
	LoaiLuong int,
	SoTien float, 	
	HeSo int,
	NgayApDung datetime , 
	NgayKetThuc datetime
);");
			Sql(@"CREATE PROCEDURE [dbo].[GetGiamTruCoDinh_TheoPtram]
	@IDChiNhanhs nvarchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanViens nvarchar(max),
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

	SET NOCOUNT ON;

	declare @tblIDNhanVien table(ID_NhanVien uniqueidentifier)

	if @IDNhanViens='%%'	
		begin
				insert into @tblIDNhanVien
				select bs.ID_NhanVien 
				from NS_CongBoSung bs				
				where bs.TrangThai in (1,2) and bs.NgayCham >= @FromDate and bs.NgayCham <= @ToDate and bs.ID_DonVi = @IDChiNhanhs
				group by bs.ID_NhanVien

		end
	else	
		begin
			insert into @tblIDNhanVien
			select Name from dbo.splitstring(@IDNhanViens)
		end	
	
	--- get bangcong
	select *
	into #temp
	from @tblCongThuCong ct
	where exists( select ID_NhanVien from @tblIDNhanVien tbl where ct.ID_NhanVien= tbl.ID_NhanVien)

    declare @tblPhuCapCD table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),LoaiLuong int, PhuCapCoDinh float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
	insert into @tblPhuCapCD
	select *
	from @tblThietLapLuong pc
	where pc.LoaiLuong = 63	
	and exists( select ID_NhanVien from @tblIDNhanVien tbl where pc.ID_NhanVien= tbl.ID_NhanVien)

	--select * from @tblPhuCapCD

		declare @tblCong1 table ( ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), PhuCapCoDinh float, HeSo int,
		NgayApDung datetime, NgayKetThuc datetime, SoLanDiMuon float)
		declare @cd_IDNhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_PhuCapCoDinh float, @cdLoaiPhuCap int, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblPhuCapCD
		OPEN curPhuCap -- cur 1
    	FETCH FIRST FROM curPhuCap
    	INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_PhuCapCoDinh, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong1
				select @cd_IDNhanVien,  @cd_IDPhuCap, @cd_TenPhuCap,@cd_PhuCapCoDinh, @cd_HeSo,
					@cd_NgayApDung, @cd_NgayKetThuc,
					SUM(IIF(SoPhutDiMuon >0,1,0))  as SoLanDiMuon
				from #temp tmp
				where tmp.ID_NhanVien = @cd_IDNhanVien and tmp.NgayCham >= @cd_NgayApDung and (@cd_NgayKetThuc is null OR tmp.NgayCham <= @cd_NgayKetThuc )  
				group by tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_PhuCapCoDinh, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	

			select *
			from @tblCong1
			where SoLanDiMuon > 0

END");

			CreateStoredProcedure(name: "[dbo].[GetGiamTruLuongChiTiet]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				IDNhanViens = p.String()
			}, body: @"SET NOCOUNT ON;

		 declare @tblIDNhanVien table(ID_NhanVien uniqueidentifier)
		insert into @tblIDNhanVien
		select Name from dbo.splitstring(@IDNhanViens)

		declare @tblCong CongThuCong
		insert into @tblCong
		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		declare @tblThietLapLuong ThietLapLuong
		insert into @tblThietLapLuong
		exec GetNS_ThietLapLuong @IDChiNhanhs, @IDNhanViens, @FromDate, @ToDate

		select *
		into #temp
		from @tblCong ct
		where exists( select ID_NhanVien from @tblIDNhanVien tbl where ct.ID_NhanVien= tbl.ID_NhanVien)

		 -- ==== GIAM TRU CODINH ====
		 -- get giamtru codinh VND trong khoang thoigian
		declare @tblGiamTruCD table (ID_NhanVien uniqueidentifier, IDPhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, GiamTruCoDinh float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblGiamTruCD
		select *			
		from @tblThietLapLuong pc 		
		where exists( select ID_NhanVien from @tblIDNhanVien tbl where pc.ID_NhanVien= tbl.ID_NhanVien)
		and pc.LoaiLuong = 62

		declare @tblCong1 table (ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, GiamTruCoDinh float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoLanDiMuon float, NgayApDung datetime, NgayKetThuc datetime)
		declare @cd_ID_NhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_LoaiLuong int, @cd_GiamTruCoDinh float, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblGiamTruCD
		OPEN curPhuCap 
    	FETCH FIRST FROM curPhuCap
    	INTO  @cd_ID_NhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_GiamTruCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong1
				select @cd_ID_NhanVien, tmp.ID_ChamCongChiTiet, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_GiamTruCoDinh, @cd_HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(SoPhutDiMuon>0,1,0)) as SoLanDiMuon,
					@cd_NgayApDung, @cd_NgayKetThuc
				from #temp tmp
				where tmp.ID_NhanVien = @cd_ID_NhanVien and tmp.NgayCham >= @cd_NgayApDung and (@cd_NgayKetThuc is null OR tmp.NgayCham <= @cd_NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @cd_ID_NhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_GiamTruCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	
		

		 -- ==== GIAM TRU THEO SO LAN ====

		-- get giamtru theosolan trong khoang thoigian
		declare @tblGiamTruLan table ( ID_NhanVien uniqueidentifier, IDPhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, GiamTruTheoLan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblGiamTruLan
		select *		
		from @tblThietLapLuong pc 
		where exists( select ID_NhanVien from @tblIDNhanVien tbl where pc.ID_NhanVien= tbl.ID_NhanVien)
		and pc.LoaiLuong = 61

		-- bảng tính số lần đi muộn, về sớm theo phiếu phân ca, ca làm việc
		declare @tblCong2 table ( ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, GiamTruTheoLan float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), SoLanDiMuon float, NgayApDung datetime, NgayKetThuc datetime)

		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @IDPhuCap uniqueidentifier, @TenPhuCap nvarchar(max), @LoaiLuong int, @GiamTruTheoLan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblGiamTruLan
		OPEN curPhuCap -- cur 1
    	FETCH FIRST FROM curPhuCap
    	INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong2
				select @ID_NhanVien, tmp.ID_ChamCongChiTiet, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(SoPhutDiMuon >0,1,0))  as SoLanDiMuon,
					@NgayApDung, @NgayKetThuc
				from #temp tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 
				
				FETCH NEXT FROM curPhuCap INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	

				-- get giamtru codinh theo % tongluongnhan
			declare @tblGiamTruTheoPTram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  GiamTruCoDinh float, HeSo float,
				NgayApDung datetime, NgayKetThuc datetime, SoLanDiMuon float)
			insert into @tblGiamTruTheoPTram
			exec GetGiamTruCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, @IDNhanViens, @tblCong, @tblThietLapLuong			

			select 
					tbl.ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien,
					TenPhuCap, NgayApDung, NgayKetThuc,
					FORMAT(tbl.GiamTruCoDinh,'###,###.###') as GiamTruCoDinh,
					FORMAT(tbl.GiamTruCoDinh,'###,###.###') as ThanhTien,
					FORMAT(tbl.GiamTruTheoLan,'###,###.###') as GiamTruTheoLan,
					tbl.SoLanDiMuon,
					FORMAT(tbl.ThanhTienGiamTruTheoLan,'###,###.###') as ThanhTienGiamTruTheoLan,
					LoaiGiamTru
				from
					(select 
						gtru.ID_NhanVien, TenPhuCap, NgayApDung, NgayKetThuc,
						GiamTruCoDinh,
						gtru.GiamTruTheoLan,					
						sum(gtru.SoLanDiMuon) as SoLanDiMuon,
						sum(gtru.ThanhTienGiamTruTheoLan) as ThanhTienGiamTruTheoLan,
						LoaiLuong as LoaiGiamTru
					from 
						(
						-- giamtru codinh vnd
						select ID_NhanVien,
							GiamTruCoDinh * HeSo as GiamTruCoDinh, 
							sum(SoLanDiMuon) as SoLanDiMuon,
							0 as GiamTruTheoLan, 0 as ThanhTienGiamTruTheoLan,
							ID_PhuCap, TenPhuCap, NgayApDung, NgayKetThuc,
							LoaiLuong
						from @tblCong1 			
						where SoLanDiMuon > 0
						group by ID_NhanVien, ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc, GiamTruCoDinh, HeSo,LoaiLuong

						union all

							-- giamtru codinh %luongnhan
						select ID_NhanVien,
							GiamTruCoDinh * HeSo as GiamTruCoDinh, 
							sum(SoLanDiMuon) as SoLanDiMuon,
							0 as GiamTruTheoLan, 0 as ThanhTienGiamTruTheoLan,
							ID_PhuCap, TenPhuCap, NgayApDung, NgayKetThuc,
							63 as LoaiLuong
						from @tblGiamTruTheoPTram 			
						group by ID_NhanVien, ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc, GiamTruCoDinh, HeSo

						union all

						-- giamtru theolan
						select ID_NhanVien, 
							0 as GiamTruCoDinh, 
							SoLanDiMuon, 
							GiamTruTheoLan * HeSo as GiamTruTheoLan,
							GiamTruTheoLan * HeSo * SoLanDiMuon as ThanhTienGiamTruTheoLan ,
							ID_PhuCap, TenPhuCap, NgayApDung, NgayKetThuc,
							LoaiLuong
						from @tblCong2
						) gtru group by gtru.ID_NhanVien, gtru.ID_PhuCap, TenPhuCap, NgayApDung, NgayKetThuc, GiamTruTheoLan, GiamTruCoDinh, LoaiLuong					
					) tbl
					join NS_NhanVien nv on tbl.ID_NhanVien= nv.ID
					where tbl.GiamTruCoDinh > 0 OR ThanhTienGiamTruTheoLan > 0
					order by tbl.NgayApDung");

			CreateStoredProcedure(name: "[dbo].[GetListCaLamViec_ofDonVi]", parametersAction: p => new
			{
				ID_DonVi = p.Guid()
			}, body: @"SET NOCOUNT ON;
	declare @dateNow varchar(14)= format(getdate(),'yyyyMMdd')

	-- get all ca of nhan vien (not check time)
	select ca.ID as ID_CaLamViec, ca.MaCa, ca.TenCa, canv.ID_NhanVien
	from
		(select ca.ID, ca.MaCa, ca.TenCa
		from NS_CaLamViec ca
		join NS_CaLamViec_DonVi cadv on ca.ID= cadv.ID_CaLamViec
		where cadv.ID_DonVi= @ID_DonVi
		) ca
	join NS_PhieuPhanCa_CaLamViec caphieu on ca.ID= caphieu.ID_CaLamViec
	join NS_PhieuPhanCa_NhanVien canv on caphieu.ID_PhieuPhanCa= canv.ID_PhieuPhanCa
	join ( select ID, 
			format(TuNgay,'yyyyMMdd') as TuNgay,  
			format(ISNULL(DenNgay, getdate()),'yyyyMMdd') as DenNgay
		from NS_PhieuPhanCa) phieu on canv.ID_PhieuPhanCa= phieu.ID
	--where phieu.TuNgay <= @dateNow and phieu.DenNgay >= @dateNow
	group by ca.ID, ca.MaCa, ca.TenCa, canv.ID_NhanVien
	order by canv.ID_NhanVien");

			CreateStoredProcedure(name: "[dbo].[GetListDebitSalaryDetail]", parametersAction: p => new
			{
				ID_BangLuong = p.Guid(),
				TextSearch = p.String(),
				Currentpage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @idChiNhanh uniqueidentifier = (select top 1 ID_DonVi from NS_BangLuong where ID= @ID_BangLuong);
	with data_cte
	as (
	select nv.TenNhanVien, quy.*, 
		case when quy.TamUngLuong > quy.LuongThucNhan - quy.DaTra then round(quy.LuongThucNhan - quy.DaTra,0) else quy.TamUngLuong end as TienTamUngFloat,	
		round(quy.LuongThucNhan - quy.DaTra,0) as ConCanTra,
		iif(quy.LuongThucNhan - quy.TamUngLuong - quy.DaTra> 0, round(quy.LuongThucNhan - TamUngLuong - DaTra,3),0) as CanTraSauTamUng
	from
	(
		select 
			quy_tamung.ID_NhanVien, max(ID_BangLuongChiTiet) as ID_BangLuongChiTiet,
			max(MaBangLuongChiTiet) as MaBangLuongChiTiet,
			sum(LuongThucNhan) as LuongThucNhan,
			sum(DaTra) as DaTra,
			sum(TamUngLuong) as TamUngLuong
		from
			(select quyhd.ID_NhanVien, quyhd.ID_BangLuongChiTiet, quyhd.MaBangLuongChiTiet, quyhd.LuongThucNhan , 
				sum(quyhd.DaTra) as DaTra, 0 as TamUngLuong
			from
			(
				-- get tienluong datra
				select blct.ID_NhanVien, blct.ID as ID_BangLuongChiTiet, blct.MaBangLuongChiTiet, blct.LuongThucNhan , 
					case when qhd.TrangThai= 0 then 0 else sum(isnull(qct.TienThu,0)) + sum(isnull(qct.TruTamUngLuong,0)) end as DaTra		--	
				from NS_BangLuong_ChiTiet blct
				left join Quy_HoaDon_ChiTiet qct on blct.ID= qct.ID_BangLuongChiTiet
				left join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
				where blct.ID_BangLuong= @ID_BangLuong
				group by  blct.ID, blct.ID_BangLuong, blct.ID_NhanVien, blct.MaBangLuongChiTiet, blct.LuongThucNhan , qhd.TrangThai
			) quyhd group by  quyhd.ID_NhanVien, quyhd.ID_BangLuongChiTiet, quyhd.MaBangLuongChiTiet, quyhd.LuongThucNhan

			union all
			-- todo: get from NS_TamUngLuong (column NoHienTai of NV)
			select ID_NhanVien,
				'00000000-0000-0000-0000-000000000000' as ID_BangLuongChiTiet, '' as MaBangLuongChiTiet, 0 as LuongThucNhan, 0 as DaTra, 
				CongNo as TamUngLuong
			from NS_CongNoTamUngLuong 
			where ID_DonVi= @idChiNhanh
			) quy_tamung group by quy_tamung.ID_NhanVien
	) quy
	join NS_NhanVien nv on quy.ID_NhanVien= nv.ID
	where round(quy.LuongThucNhan - DaTra,0) > 0
	),
	count_cte
	as(
		select count(ID_NhanVien) as TotalRow,
			CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float )) as TotalPage,
			Sum(LuongThucNhan) as TongLuongNhan,
			Sum(DaTra) as TongDaTra,
			Sum(ConCanTra) as TongCanTra,
			Sum(TamUngLuong) as TongTamUng,
			Sum(TienTamUngFloat) as TongTruTamUngThucTe,		
			Sum(CanTraSauTamUng) as TongCanTraSauTamUng		
		from data_cte
	)
	select *,
		FORMAT(TienTamUngFloat,'###,###.###') as TienTamUng,
		FORMAT(CanTraSauTamUng,'###,###.###') as TienTra
	from data_cte dt
	cross join count_cte ct 
	order by dt.MaBangLuongChiTiet
	offset (@CurrentPage * @PageSize) Rows
	fetch next @PageSize Rows only");

			CreateStoredProcedure(name: "[dbo].[GetListTheGiaTri]", parametersAction: p => new
			{
				IDDonVis = p.String(),
				TextSearch = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				TrangThais = p.String(10),
				MucNapFrom = p.Double(),
				MucNapTo = p.Double(),
				KhuyenMaiFrom = p.Double(),
				KhuyenMaiTo = p.Double(),
				KhuyenMaiLaPTram = p.Int(),
				ChietKhauFrom = p.Double(),
				ChietKhauTo = p.Double(),
				ChietKhauLaPTram = p.Int(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

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

	select tblThe.ID,tblThe.MaHoaDon,tblThe.NgayLapHoaDon,tblThe.NgayTao,
		tblThe.TongChiPhi as MucNap,
		tblThe.TongChietKhau as KhuyenMaiVND,
		tblThe.TongTienHang as TongTienNap,
		tblThe.TongTienThue as SoDuSauNap,
		tblThe.TongGiamGia as ChietKhauVND,
		ISNULL(tblThe.DienGiai,'') as GhiChu,
		tblThe.NguoiTao,
		ISNULL(tblThe.ID_DoiTuong,'00000000-0000-0000-0000-000000000000') as ID_DoiTuong,
		tblThe.PhaiThanhToan,
		ISNULL(tblThe.NhanVienThucHien,'') as NhanVienThucHien,
		tblThe.MaDoiTuong as MaKhachHang,
		tblThe.TenDoiTuong as TenKhachHang,
		tblThe.DienThoai as DienThoaiKhachHang,
		tblThe.DiaChi as DiaChiKhachHang,
		tblThe.ChoThanhToan,
		tblThe.ChietKhauPT,
		tblThe.KhuyenMaiPT,
		ISNULL(soquy.TienMat,0) as TienMat,
		ISNULL(soquy.TienPOS,0) as TienATM,
		ISNULL(soquy.TienCK,0) as TienGui,
		ISNULL(soquy.TienThu,0) as KhachDaTra,
		dv.TenDonVi,
		dv.SoDienThoai as DienThoaiChiNhanh,
		dv.DiaChi as DiaChiChiNhanh
	from
		(
		select *
		from
			(select hd.*, 
					hd.TongGiamGia/hd.TongChiPhi * 100 as ChietKhauPT,
					hd.TongChietKhau/hd.TongChiPhi * 100 as KhuyenMaiPT,
					dt.MaDoiTuong, dt.TenDoiTuong,
					dt.DienThoai, 
					dt.DiaChi,
					case when hd.ChoThanhToan is null then '10' else '12' end as TrangThai,
					NhanVienThucHien
				from BH_HoaDon hd
				join DM_DoiTuong dt on hd.ID_DoiTuong = dt.ID
				left join (
						Select distinct
							(
								Select nv.TenNhanVien + ',' AS [text()]
    							From dbo.BH_NhanVienThucHien th
    							join dbo.NS_NhanVien nv on th.ID_NhanVien = nv.ID
								where th.ID_HoaDon= nvth.ID_HoaDon
								For XML PATH ('')
							) NhanVienThucHien, nvth.ID_HoaDon
							From dbo.BH_NhanVienThucHien nvth
							) nvThucHien on hd.ID = nvThucHien.ID_HoaDon
				where exists (select name from dbo.splitstring(@IDDonVis) dv where hd.ID_DonVi= dv.Name)	
				and hd.LoaiHoaDon = 22
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
					sum(quy.TienCK) as TienCK
				from
				(
					select qct.ID_HoaDonLienQuan,
						qct.TienMat,
						qct.TienThu,
						case when tk.TaiKhoanPOS = '1' then qct.TienGui else 0 end as TienPOS,
						case when tk.TaiKhoanPOS = '0' then qct.TienGui else 0 end as TienCK
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
					left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang= tk.ID
					where qhd.TrangThai= 1 
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
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetLuongChinh_ofNhanVien]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				IDNhanViens = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				NgayCongChuan = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblCongThuCong CongThuCong
	insert into @tblCongThuCong
	exec dbo.GetChiTietCongThuCong @ID_ChiNhanh,@IDNhanViens, @FromDate, @ToDate

	declare @tblThietLapLuong ThietLapLuong
	insert into @tblThietLapLuong
	exec GetNS_ThietLapLuong @ID_ChiNhanh,@IDNhanViens, @FromDate, @ToDate

	declare @tblLuongCDNgay table(ID_NhanVien uniqueidentifier, LoaiLuong int, LuongCoBan float, NgayApDung datetime, NgayKetThuc datetime, SoNgayDiLam float, NgayCongChuan int, ThanhTien float)
	insert into @tblLuongCDNgay
	exec GetLuongCoDinh_OrLuongNgayCong @NgayCongChuan,@tblCongThuCong,@tblThietLapLuong

	declare @tblLuongCaGio table(ID_NhanVien uniqueidentifier,LoaiLuong int,LoaiNgayThuong_Nghi int, LuongCoBan float, LuongCoBanQuyDoi float, HeSoLuong int, LaPhanTram int,
	NgayApDung datetime, NgayKetThuc datetime, ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, SoNgayDiLam float, ThanhTien float)
	insert into @tblLuongCaGio
	exec GetLuongChinhTheoCaGio @tblCongThuCong,@tblThietLapLuong	

	select nv.MaNhanVien, nv.TenNhanVien, 
		ID_NhanVien, LoaiLuong, 
		FORMAT(luongchinh.LuongCoBan,'###,###.###') as LuongCoBan,
		FORMAT(luongchinh.LuongCoBanQuyDoi,'###,###.###') as LuongCoBanQuyDoi,
		NgayApDung, NgayKetThuc, SoNgayDiLam, NgayCongChuan,
		ThanhTien,
		ID_CaLamViec, TenCa, TongGioCong1Ca,LoaiNgayThuong_Nghi, HeSoLuong , LaPhanTram
	from
		(select ID_NhanVien, LoaiLuong, LuongCoBan, LuongCoBanQuyDoi,  NgayApDung, NgayKetThuc, SoNgayDiLam, 0 as NgayCongChuan, ThanhTien,
			ID_CaLamViec, TenCa, TongGioCong1Ca,LoaiNgayThuong_Nghi, HeSoLuong , LaPhanTram
		from @tblLuongCaGio

		union all
		select ID_NhanVien, LoaiLuong, LuongCoBan, 0 as LuongCoBanQuyDoi, NgayApDung, NgayKetThuc, SoNgayDiLam, NgayCongChuan, ThanhTien,
			'00000000-0000-0000-0000-000000000000' as ID_CaLamViec, '' as TenCa, 0 as TongGioCong1Ca, 0 as LoaiNgayThuong_Nghi,	0 as HeSoLuong, 0 as LaPhanTram
		from @tblLuongCDNgay
		)luongchinh
	join NS_NhanVien nv on luongchinh.ID_NhanVien= nv.ID");

			Sql(@"CREATE PROCEDURE [dbo].[GetLuongChinhTheoCaGio]
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

	SET NOCOUNT ON;	

		--declare @IDChiNhanhs uniqueidentifier='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
		--declare @FromDate datetime='2020-08-01'
		--declare @ToDate datetime='2020-08-31'

		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs,'', @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs,'', @FromDate, @ToDate

		-- get caidatluong (ca + gio)
		declare @tblLuongCaGio table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, 
		LuongCoBan float, HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblLuongCaGio
		select *
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong in (3,4) 
		order by NgayApDung	

		-- bảng tính công theo phiếu phân ca, ca làm việc
		declare @tblCong table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, LoaiLuong float, 
		LoaiNgayThuong_Nghi int, LuongCoBan float,LuongCoBanQuyDoi float, HeSoLuong int, LaPhanTram int, NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, TongCong float,
		ThanhTien float)				
	
		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int,
		@LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblLuongCaGio 
		OPEN curLuong 
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan,@HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				select ct.*
				into #tmpca
				from NS_ThietLapLuongChiTiet ct
				where ID_LuongPhuCap= @ID_PhuCap and LaOT= 0
				
				insert into @tblCong
				select @ID_NhanVien, @ID_PhuCap, @LoaiLuong, 
					tbl.LoaiNgayThuong_Nghi, tbl.LuongCoBan, tbl.LuongCoBanQuyDoi, tbl.HeSoLuong, tbl.LaPhanTram,
					@NgayApDung, @NgayKetThuc,
					tbl.ID_CaLamViec, tbl.TenCa, tbl.TongGioCong1Ca,
					tbl.TongCong,
					case when @LoaiLuong = 3 then LuongCoBanQuyDoi * tbl.Cong else (tbl.Cong * TongGioCong1Ca - SoPhutDiMuon * 1.0/60 +  SoGioOT) * LuongCoBanQuyDoi  end as ThanhTien						
				from
					(
					select 
							luong.ID_CaLamViec, luong.TenCa, luong.TongGioCong1Ca,
							luong.Cong, luong.SoPhutDiMuon, luong.SoGioOT, 
							luong.LuongNgayThuong, luong.LoaiNgayThuong_Nghi,
							luong.GiaTri as HeSoLuong,luong.LaPhanTram,
							case when @LoaiLuong= 3 then luong.Cong else luong.Cong * luong.TongGioCong1Ca - luong.SoPhutDiMuon * 1.0/60 +  luong.SoGioOT end as TongCong,	
							case when luong.LaPhanTram= 1 then @HeSo * luong.LuongNgayThuong else @HeSo * luong.GiaTri end as LuongCoBan,
							case when luong.LaPhanTram= 1 then @HeSo * luong.LuongNgayThuong * luong.GiaTri/100 else @HeSo * luong.GiaTri end as LuongCoBanQuyDoi
					
						from 
						(
							select 
								qd.ID_CaLamViec, qd.TenCa, qd.TongGioCong1Ca,
								qd.Cong, qd.SoPhutDiMuon, qd.SoGioOT,
								isnull(qd.LuongNgayThuong, LuongNgayThuongNull) as LuongNgayThuong,
								case when qd.LoaiNgay= 0 then case when qd.Thu not in (0,6) then 3 else qd.Thu end								
									else case when qd.Thu in (0,6) then qd.Thu else qd.LoaiNgay end end as LoaiNgayThuong_Nghi,
								case qd.Thu
									when 6 then case when Thu7_GiaTri is not null then qd.Thu7_GiaTri else Thu7_GiaTriNull end
									when 0 then case when ThCN_GiaTri is not null then qd.ThCN_GiaTri else ThCN_GiaTriNull end
								else	
									case qd.LoaiNgay
										when 0 then case when qd.LuongNgayThuong is not null then LuongNgayThuong else LuongNgayThuongNull end
										when 1 then case when qd.NgayNghi_GiaTri is not null then NgayNghi_GiaTri else NgayNghi_GiaTriNull end
										when 0 then case when qd.NgayLe_GiaTri is not null then NgayLe_GiaTri else NgayLe_GiaTriNull end
									end
								end
								as GiaTri,

								case qd.Thu
									when 6 then case when Thu7_LaPhanTramLuong is not null then qd.Thu7_LaPhanTramLuong else Thu7_LaPhanTramLuongNull end
									when 0 then case when CN_LaPhanTramLuong is not null then qd.CN_LaPhanTramLuong else CN_LaPhanTramLuongNull end
								else	
									case qd.LoaiNgay
										when 0 then 0
										when 1 then case when qd.NgayNghi_LaPhanTramLuong is not null then NgayNghi_LaPhanTramLuong else NgayNghi_LaPhanTramLuongNull end
										when 0 then case when qd.NgayLe_LaPhanTramLuong is not null then NgayLe_LaPhanTramLuong else NgayLe_LaPhanTramLuongNull end
									end
								end
								as LaPhanTram
							
							from
							(
								select tmp.*, 
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) LuongNgayThuong,
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) LuongNgayThuongNull,

									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_GiaTri,
									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_GiaTriNull,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_LaPhanTramLuong,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_LaPhanTramLuongNull,
						
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) CN_LaPhanTramLuong,
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) CN_LaPhanTramLuongNull,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) ThCN_GiaTri,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) ThCN_GiaTriNull,

									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_GiaTri,
									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_GiaTriNull,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_LaPhanTramLuong,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_LaPhanTramLuongNull,

									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_GiaTri,
									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_GiaTriNull,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_LaPhanTramLuong,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_LaPhanTramLuongNull
						
								from @tblCongThuCong tmp
								where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
							) qd
						)luong
					)tbl
					drop table #tmpca
										
				FETCH NEXT FROM curLuong 
				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	


			select ID_NhanVien, LoaiLuong, LoaiNgayThuong_Nghi, LuongCoBan, LuongCoBanQuyDoi,  HeSoLuong, LaPhanTram,
				NgayApDung, NgayKetThuc,
				ID_CaLamViec,TenCa, TongGioCong1Ca,	
				sum(TongCong) as SoNgayDiLam, 
				sum(ThanhTien) as ThanhTien		
			from @tblCong
			group by ID_NhanVien,LoaiLuong, LoaiNgayThuong_Nghi, LuongCoBan,LuongCoBanQuyDoi, HeSoLuong, LaPhanTram,
				NgayApDung, NgayKetThuc,TenCa, TongGioCong1Ca,ID_CaLamViec
				order by ID_CaLamViec
END");

			Sql(@"CREATE PROCEDURE [dbo].[GetLuongCoDinh_OrLuongNgayCong]
	@NgayCongChuan int,
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

		SET NOCOUNT ON;	

		--declare @IDChiNhanhs uniqueidentifier='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
		--declare @FromDate datetime='2020-08-01'
		--declare @ToDate datetime='2020-08-31'

		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs,'', @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs,'', @FromDate, @ToDate


		-- get thietlapluong (codinh + ngay)
		declare @tblLuongCDNgay table (ID_NhanVien uniqueidentifier,ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblLuongCDNgay
		select *		
		from @tblThietLapLuong pc 
		where  pc.LoaiLuong in (1,2)

		declare @tblCongCDNgay table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), SoNgayDiLam float, NgayApDung datetime, NgayKetThuc datetime)
		
		declare @cdID_NhanVien uniqueidentifier, @cdID uniqueidentifier, @cdTenLoaiLuong nvarchar(max), @cdLoaiLuong int,@cdLuongCoBan float, @cdHeSo int, @cdNgayApDung datetime, @cdNgayKetThuc datetime
		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblLuongCDNgay
		OPEN curLuong -- cur 1
    	FETCH FIRST FROM curLuong
    	INTO @cdID_NhanVien, @cdID, @cdTenLoaiLuong, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, @cdNgayApDung, @cdNgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongCDNgay
				select @cdID_NhanVien, @cdID, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam,
					@cdNgayApDung,@cdNgayKetThuc
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @cdID_NhanVien and tmp.NgayCham >= @cdNgayApDung and (@cdNgayKetThuc is null OR tmp.NgayCham <= @cdNgayKetThuc )  
				group by tmp.ID_NhanVien, tmp.ID_CaLamViec, tmp.TenCa		
				FETCH NEXT FROM curLuong INTO @cdID_NhanVien, @cdID, @cdTenLoaiLuong, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, @cdNgayApDung, @cdNgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 

			select luongcd.ID_NhanVien, luongcd.LoaiLuong, LuongCoBan, NgayApDung, NgayKetThuc,
				sum(SoNgayDiLam) as SoNgayDiLam,
				@NgayCongChuan as NgayCongChuan,
				case when LoaiLuong = 1 then LuongCoBan
				else case when sum(SoNgayDiLam) > @NgayCongChuan then LuongCoBan else LuongCoBan/@NgayCongChuan * sum(SoNgayDiLam) end
				end as ThanhTien
			from @tblCongCDNgay luongcd		
			group by ID_NhanVien, LoaiLuong,LuongCoBan,NgayApDung, NgayKetThuc
		
END");

			CreateStoredProcedure(name: "[dbo].[GetLuongOT_ofNhanVien]", parametersAction: p => new
			{
				IDChiNhanhs = p.Guid(),
				IDNhanViens = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				NgayCongChuan = p.Int()
			}, body: @"SET NOCOUNT ON;	

		declare @tblCongThuCong CongThuCong
		insert into @tblCongThuCong
		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		declare @tblThietLapLuong ThietLapLuong
		insert into @tblThietLapLuong
		exec GetNS_ThietLapLuong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate


		-- ============= OT Ngay ====================
		declare @thietlapOTNgay table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @thietlapOTNgay
		select *	
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong = 2

		select  a.*,		
				case when LaPhanTram = 0 then GiaTri else SoTien/@NgayCongChuan/8 end as Luong1GioCongCoBan ,
				case when LaPhanTram = 0 then HeSo * GiaTri else (SoTien/@NgayCongChuan/8) * GiaTri * HeSo/100 end as Luong1GioCongQuyDoi				
			into #temp1					
			from
			(
			select bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
					pc.SoTien,
					pc.LoaiLuong,
					pc.HeSo,
					case when bs.LoaiNgay= 0 then
						case when bs.Thu not in (0,6) then 3 else bs.Thu end -- ngaythuong:3, thu7: 6, cn:0
					else case when bs.Thu in (0,6) then bs.Thu else bs.LoaiNgay end end as LoaiNgayThuong_Nghi, 
					case bs.Thu
							when 6 then tlct.Thu7_GiaTri
							when 0 then tlct.ThCN_GiaTri
						else
							case bs.LoaiNgay 
								when 0 then LuongNgayThuong
								when 1 then tlct.NgayNghi_GiaTri
								when 2 then tlct.NgayLe_GiaTri
								end
							end as GiaTri,
					case bs.Thu
						when 6 then tlct.Thu7_LaPhanTramLuong
						when 0 then tlct.CN_LaPhanTramLuong
					else
					case bs.LoaiNgay  
						when 0 then tlct.NgayThuong_LaPhanTramLuong
						when 1 then tlct.NgayNghi_LaPhanTramLuong
						when 2 then tlct.NgayLe_LaPhanTramLuong
						end
					end as LaPhanTram
			from @tblCongThuCong bs
			join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
			join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap 
			where tlct.LaOT= 1 and pc.LoaiLuong= 2
			) a			

			declare @tblCongOTNgay table (ID_PhuCap uniqueidentifier, ID_NhanVien uniqueidentifier, LoaiLuong int, 
			Luong1GioCongCoBan float, Luong1GioCongQuyDoi float, HeSoLuong int,NgayApDung datetime, NgayKetThuc datetime,
			LoaiNgayThuong_Nghi int, LaPhanTram int,
			SoGioOT float, LuongOT float, NgayCham datetime)	

			declare @otngayID_NhanVien uniqueidentifier, @otngayID_PhuCap uniqueidentifier, @otngayTenLoaiLuong nvarchar(max), 
			@otngayLoaiLuong int,@otngayLuongCoBan float, @otngayHeSo int, @otngayNgayApDung datetime, @otngayNgayKetThuc datetime

			DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTNgay
		OPEN curLuong -- cur 1
    	FETCH FIRST FROM curLuong
    	INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongOTNgay
				select @otngayID_PhuCap,@otngayID_NhanVien, @otngayLoaiLuong,tmp.Luong1GioCongCoBan,tmp.Luong1GioCongQuyDoi, tmp.GiaTri,@otngayNgayApDung, @otngayNgayKetThuc,
					tmp.LoaiNgayThuong_Nghi, tmp.LaPhanTram,
					tmp.SoGioOT,
					tmp.SoGioOT * Luong1GioCongQuyDoi as LuongOT,
					tmp.NgayCham
				from #temp1 tmp
				where tmp.ID_NhanVien = @otngayID_NhanVien and tmp.NgayCham >= @otngayNgayApDung and (@otngayNgayKetThuc is null OR tmp.NgayCham <= @otngayNgayKetThuc )  								
				FETCH NEXT FROM curLuong 
				INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	


			-- ============= OT Ca =================

			declare @thietlapOTCa table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
			insert into @thietlapOTCa
			select *	
			from @tblThietLapLuong pc 		
			where pc.LoaiLuong = 3

			select  a.*,	
				case when LaPhanTram = 0 then GiaTri else case when LuongTheoCa is null then SoTien/TongGioCong1Ca else LuongTheoCa/TongGioCong1Ca end end Luong1GioCongCoBan,
				case when LaPhanTram = 0 then GiaTri else case when LuongTheoCa is null then SoTien/TongGioCong1Ca * GiaTri/100 
				else LuongTheoCa/TongGioCong1Ca* GiaTri/100 end end as Luong1GioCongQuyDoi				
			into #temp2					
			from
				(select bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
					pc.SoTien,
					theoca.LuongTheoCa,
					pc.LoaiLuong,
					pc.HeSo,
					case when bs.LoaiNgay= 0 then
						case when bs.Thu not in (0,6) then 3 else bs.Thu end -- ngaythuong:3, thu7: 6, cn:0
					else case when bs.Thu in (0,6) then bs.Thu else bs.LoaiNgay end end as LoaiNgayThuong_Nghi, 
					case bs.Thu
							when 6 then tlct.Thu7_GiaTri
							when 0 then tlct.ThCN_GiaTri
						else
							case bs.LoaiNgay 
								when 0 then LuongNgayThuong
								when 1 then tlct.NgayNghi_GiaTri
								when 2 then tlct.NgayLe_GiaTri
								end
							end as GiaTri,
					case bs.Thu
						when 6 then tlct.Thu7_LaPhanTramLuong
						when 0 then tlct.CN_LaPhanTramLuong
					else
					case bs.LoaiNgay  
						when 0 then tlct.NgayThuong_LaPhanTramLuong
						when 1 then tlct.NgayNghi_LaPhanTramLuong
						when 2 then tlct.NgayLe_LaPhanTramLuong
						end
					end as LaPhanTram
				from @tblCongThuCong bs
				join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
				join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap 
				left join (select tlca.LuongNgayThuong as LuongTheoCa, tlca.ID_CaLamViec, pca.ID_NhanVien
						from NS_Luong_PhuCap pca
						join NS_ThietLapLuongChiTiet tlca on pca.ID= tlca.ID_LuongPhuCap 
						where tlca.LaOT= 0
						) theoca on pc.ID_NhanVien= theoca.ID_NhanVien and bs.ID_CaLamViec= theoca.ID_CaLamViec
				where tlct.LaOT= 1
				and pc.LoaiLuong = 3
				) a			

		declare @tblCongOTCa table (ID_PhuCap uniqueidentifier, ID_NhanVien uniqueidentifier, LoaiLuong int, 
		Luong1GioCongCoBan float, Luong1GioCongQuyDoi float, HeSoLuong int,NgayApDung datetime, NgayKetThuc datetime,
		LoaiNgayThuong_Nghi int, LaPhanTram int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
		SoGioOT float, LuongOT float, NgayCham datetime)				
	
		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int,@LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTCa
		OPEN curLuong
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongOTCa
				select @ID_PhuCap,@ID_NhanVien, @LoaiLuong,tmp.Luong1GioCongCoBan,tmp.Luong1GioCongQuyDoi, tmp.GiaTri,@NgayApDung, @NgayKetThuc,
				tmp.LoaiNgayThuong_Nghi, tmp.LaPhanTram,
					tmp.ID_CaLamViec, 					
					tmp.TenCa, 
					tmp.TongGioCong1Ca,
					tmp.SoGioOT,
					tmp.SoGioOT * Luong1GioCongQuyDoi as LuongOT,
					tmp.NgayCham
				from #temp2 tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  								
				FETCH NEXT FROM curLuong 
				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	

			select nv.MaNhanVien, nv.TenNhanVien, ot.ID_NhanVien,
				LoaiLuong, Luong1GioCongCoBan, 
				FORMAT(ot.Luong1GioCongQuyDoi,'###,###.###') as Luong1GioCongQuyDoi, 
				HeSoLuong,
				LoaiNgayThuong_Nghi, 
				LaPhanTram,
				ID_CaLamViec, TenCa, TongGioCong1Ca,
				cast(SoGioOT as float) as SoGioOT,
				FORMAT(ot.LuongOT,'###,###.###') as ThanhTien
			from 
				(
				select 
					ID_NhanVien, LoaiLuong, Luong1GioCongCoBan, Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
					ID_CaLamViec, TenCa, TongGioCong1Ca,
					sum(SoGioOT) as SoGioOT,
					sum(LuongOT) as LuongOT
				from
					(select ID_NhanVien, LoaiLuong, Luong1GioCongCoBan, Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
						'00000000-0000-0000-0000-000000000000' as ID_CaLamViec, '' as TenCa, 8 as TongGioCong1Ca,
						SoGioOT,
						LuongOT
					from @tblCongOTNgay cong				

					union all

					select ID_NhanVien, LoaiLuong, Luong1GioCongCoBan, Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
						ID_CaLamViec, TenCa,TongGioCong1Ca,
						SoGioOT,
						LuongOT
					from @tblCongOTCa cong	
					) luongot
				group by luongot.ID_NhanVien,LoaiLuong, Luong1GioCongCoBan,Luong1GioCongQuyDoi, HeSoLuong,LoaiNgayThuong_Nghi, LaPhanTram,
						luongot.ID_CaLamViec, TenCa, TongGioCong1Ca
			)ot
			join NS_NhanVien nv on ot.ID_NhanVien= nv.ID
			where ot.LuongOT > 0
			order by ID_NhanVien, ID_CaLamViec");

			CreateStoredProcedure(name: "[dbo].[GetNhanVien_coBangLuong]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;

    select nv.ID, nv.MaNhanVien, nv.TenNhanVien, 
	CONCAT(nv.MaNhanVien,' ', nv.TenNhanVien, ' ', nv.TenNhanVienKhongDau, ' ', nv.TenNhanVienChuCaiDau) as NameFull
	from
		(select bs.ID_NhanVien 
		from NS_CongBoSung bs
		where bs.TrangThai in (1,2) and bs.NgayCham >= @FromDate and bs.NgayCham <= @ToDate and bs.ID_DonVi =@ID_ChiNhanh
		group by bs.ID_NhanVien) cc
	join NS_NhanVien nv on cc.ID_NhanVien = nv.ID");

			Sql(@"CREATE PROCEDURE [dbo].[GetPhuCapCoDinh_TheoPtram]
	@IDChiNhanhs nvarchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanViens nvarchar(max),
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

	SET NOCOUNT ON;

	declare @tblIDNhanVien table(ID_NhanVien uniqueidentifier)

	--declare @tblCongThuCong CongThuCong
	--	insert into @tblCongThuCong
	--	exec dbo.GetChiTietCongThuCong @IDChiNhanhs,'', @FromDate, @ToDate

	--	declare @tblThietLapLuong ThietLapLuong
	--	insert into @tblThietLapLuong
	--	exec GetNS_ThietLapLuong @IDChiNhanhs,'', @FromDate, @ToDate

	if @IDNhanViens='%%' or @IDNhanViens =''
		begin
				insert into @tblIDNhanVien
				select bs.ID_NhanVien 
				from NS_CongBoSung bs 
				where bs.TrangThai in (1,2) and bs.NgayCham >= @FromDate and bs.NgayCham <= @ToDate and bs.ID_DonVi = @IDChiNhanhs
				group by bs.ID_NhanVien

		end
	else	
		begin
			insert into @tblIDNhanVien
			select Name from dbo.splitstring(@IDNhanViens)
		end

	--- get bangcong
	select *
	into #temp
	from @tblCongThuCong ct	
	where exists( select ID_NhanVien from @tblIDNhanVien tbl where ct.ID_NhanVien= tbl.ID_NhanVien)

    declare @tblPhuCapCD table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiPhuCap int, PhuCapCoDinh float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
	insert into @tblPhuCapCD
	select *
	from @tblThietLapLuong pc 
	where pc.LoaiLuong = 53	
	and exists( select ID_NhanVien from @tblIDNhanVien tbl where pc.ID_NhanVien= tbl.ID_NhanVien)

		declare @tblCong1 table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), PhuCapCoDinh float, HeSo int,
		NgayApDung datetime, NgayKetThuc datetime, SoNgayDiLam float)
		declare @cd_IDNhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_LoaiPhuCap int, @cd_PhuCapCoDinh float, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblPhuCapCD
		OPEN curPhuCap -- cur 1
    	FETCH FIRST FROM curPhuCap
    	INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiPhuCap, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong1
				select @cd_IDNhanVien,  @cd_IDPhuCap, @cd_TenPhuCap,@cd_PhuCapCoDinh, @cd_HeSo,
					@cd_NgayApDung, @cd_NgayKetThuc,
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam
				from #temp tmp
				where tmp.ID_NhanVien = @cd_IDNhanVien and tmp.NgayCham >= @cd_NgayApDung and (@cd_NgayKetThuc is null OR tmp.NgayCham <= @cd_NgayKetThuc )  
				group by tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiPhuCap, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	

			select *
			from @tblCong1

END");

			CreateStoredProcedure(name: "[dbo].[GetPhuCapLuongChiTiet]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				IDNhanViens = p.String()
			}, body: @"SET NOCOUNT ON;

		declare @tblIDNhanVien table(ID_NhanVien uniqueidentifier)
		insert into @tblIDNhanVien
		select Name from dbo.splitstring(@IDNhanViens)

		declare @tblCong CongThuCong
		insert into @tblCong
		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		declare @tblThietLapLuong ThietLapLuong
		insert into @tblThietLapLuong
		exec GetNS_ThietLapLuong @IDChiNhanhs, @IDNhanViens, @FromDate, @ToDate

		select *
		into #temp
		from @tblCong ct		
		where exists( select ID_NhanVien from @tblIDNhanVien tbl where ct.ID_NhanVien= tbl.ID_NhanVien)

	-- get phucapcodinh vnd trong khoang thoigian
		declare @tblPhuCapCD table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, PhuCapCoDinh float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblPhuCapCD
		select *			
		from @tblThietLapLuong pc 
		where pc.LoaiLuong = 52	
		and exists( select ID_NhanVien from @tblIDNhanVien tbl where pc.ID_NhanVien= tbl.ID_NhanVien)

		declare @tblCong1 table ( ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, PhuCapCoDinh float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoNgayDiLam float, NgayApDung datetime, NgayKetThuc datetime)
		declare @cd_IDNhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_LoaiLuong int, @cd_PhuCapCoDinh float, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblPhuCapCD
		OPEN curPhuCap -- cur 1
    	FETCH FIRST FROM curPhuCap
    	INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong1
				select @cd_IDNhanVien, tmp.ID_ChamCongChiTiet, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_PhuCapCoDinh, @cd_HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam,
					@cd_NgayApDung, @cd_NgayKetThuc
				from #temp tmp
				where tmp.ID_NhanVien = @cd_IDNhanVien and tmp.NgayCham >= @cd_NgayApDung and (@cd_NgayKetThuc is null OR tmp.NgayCham <= @cd_NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 			

		-- get phucaptheongay trong khoang thoigian
		declare @tblPhuCap table (ID_NhanVien uniqueidentifier, IDPhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, PhuCapTheoNgay float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblPhuCap
		select *			
		from @tblThietLapLuong pc 
		where pc.LoaiLuong = 51
		and exists( select ID_NhanVien from @tblIDNhanVien tbl where pc.ID_NhanVien= tbl.ID_NhanVien)
	
			-- bảng tính số ngày đi làm theo phiếu phân ca, ca làm việc
		declare @tblCong2 table (ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, 
		PhuCapTheoNgay float, HeSo int, ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoNgayDiLam float,		
		NgayApDung datetime, NgayKetThuc datetime)

		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @IDPhuCap uniqueidentifier, @TenPhuCap nvarchar(max), @PhuCapTheoNgay float, @LoaiLuong int, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblPhuCap
		OPEN curPhuCap 
    	FETCH FIRST FROM curPhuCap
    	INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong,@PhuCapTheoNgay, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong2
				select @ID_NhanVien, tmp.ID_ChamCongChiTiet, @IDPhuCap, @TenPhuCap, @LoaiLuong, @PhuCapTheoNgay, @HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam,
					@NgayApDung, @NgayKetThuc
				from #temp tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong,@PhuCapTheoNgay, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	

			-- get phucapluong codinh theo % luong
			declare @tblPhuCapTheoPtram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  PhuCapCoDinh float, HeSo float,
			NgayApDung datetime, NgayKetThuc datetime, SoNgayDiLam float)
			insert into @tblPhuCapTheoPtram
			exec GetPhuCapCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, @IDNhanViens, @tblCong, @tblThietLapLuong
			
			select *
			into #temp2
			from
			(select ID_NhanVien,
					PhuCapCoDinh * HeSo as PhuCapCoDinh, 
					0 as PhuCapTheoNgay, 
					0 as ThanhTienPC_TheoNgay,
					sum(SoNgayDiLam) as SoNgayDiLam,
					ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc, 
					LoaiLuong
				from @tblCong1 			
				where SoNgayDiLam > 0
				group by ID_NhanVien, ID_PhuCap, PhuCapCoDinh, HeSo, NgayApDung, NgayKetThuc, TenPhuCap,LoaiLuong

				union all

				select ID_NhanVien, 
					0 as PhuCapCoDinh,
					PhuCapTheoNgay  * HeSo as PhuCapTheoNgay ,
					PhuCapTheoNgay * HeSo * SoNgayDiLam as ThanhTienPC_TheoNgay,
					SoNgayDiLam,
					ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc,
					LoaiLuong
				from @tblCong2

				union all

				select ID_NhanVien,
					PhuCapCoDinh * HeSo as PhuCapCoDinh,
					0 as PhuCapTheoNgay,
					0 as ThanhTienPC_TheoNgay,
					SoNgayDiLam,
					ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc, 
					53 as LoaiLuong
				from @tblPhuCapTheoPtram
				) pc	


			select 
				tblview.ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien,
				FORMAT(tblview.PhuCapCoDinh,'###,###.###') as PhuCapCoDinh,
				FORMAT(tblview.PhuCapCoDinh,'###,###.###') as ThanhTien,
				FORMAT(tblview.PhuCapTheoNgay,'###,###.###') as PhuCapTheoNgay,
				SoNgayDiLam,
				FORMAT(tblview.ThanhTienPC_TheoNgay,'###,###.###') as ThanhTienPC_TheoNgay,
				TenPhuCap, NgayApDung, NgayKetThuc, LoaiLuong as LoaiPhuCap
			from
				(select ID_NhanVien ,
						PhuCapCoDinh,
						PhuCapTheoNgay,
						SUM(SoNgayDiLam) as SoNgayDiLam,
						SUM(pc.ThanhTienPC_TheoNgay) as ThanhTienPC_TheoNgay,
						TenPhuCap, NgayApDung, NgayKetThuc, LoaiLuong
				from #temp2 pc
				group by ID_NhanVien, ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc, PhuCapTheoNgay,LoaiLuong,PhuCapCoDinh		
				) tblview
			join NS_NhanVien nv on tblview.ID_NhanVien= nv.ID
		order by tblview.NgayApDung ");

			CreateStoredProcedure(name: "[dbo].[GetQuyDoi_ofCongBoSung]", parametersAction: p => new
			{
				ID_PhuCap = p.Guid(),
				ID_NhanVien = p.Guid(),
				ID_DonVi = p.Guid()
			}, body: @"SET NOCOUNT ON;

			declare @ngayapdung datetime, @ngayketthuc datetime
			select @ngayapdung= NgayApDung, @ngayketthuc = NgayKetThuc from NS_Luong_PhuCap where ID= @ID_PhuCap

			-- tlluong.ID_NhanVien is null: chua thietlapluongchitiet
			-- tlluong.ID_CaLamViec is null: quydoi macdinh
			select bs.ID as ID_CongBoSung, 
					ISNULL(tlluong.ID_CaLamViec,'00000000-0000-0000-0000-000000000000') as ID_CaLamViec,
					case bs.Thu
						when 0 then tlluong.ThCN_GiaTri 
						when 6 then tlluong.Thu7_GiaTri
					else
						case bs.LoaiNgay 
							when 0 then 100
							when 1 then tlluong.NgayNghi_GiaTri
							when 2 then tlluong.NgayLe_GiaTri
						else 100
					end end as CongQuyDoi,

					case bs.Thu
						when 0 then tlluong.ThCN_GiaTriOT
						when 6 then tlluong.Thu7_GiaTriOT
					else
						case bs.LoaiNgay 
							when 0 then tlluong.LuongNgayThuongOT
							when 1 then tlluong.NgayNghi_GiaTriOT
							when 2 then tlluong.NgayLe_GiaTriOT
						else 100
					end end as CongOTQuyDoi	,
					
					case bs.Thu
						when 0 then tlluong.CN_LaPhanTramLuong 
						when 6 then tlluong.Thu7_LaPhanTramLuong
					else
						case bs.LoaiNgay 
							when 0 then 1
							when 1 then tlluong.NgayNghi_LaPhanTramLuong
							when 2 then tlluong.NgayLe_LaPhanTramLuong
						else 100
					end end as LaPTCongQuyDoi,

					case bs.Thu
						when 0 then tlluong.CN_LaPhanTramLuongOT
						when 6 then tlluong.Thu7_LaPhanTramLuongOT
					else
						case bs.LoaiNgay 
							when 0 then tlluong.NgayThuong_LaPhanTramLuongOT
							when 1 then tlluong.NgayNghi_LaPhanTramLuongOT
							when 2 then tlluong.NgayLe_LaPhanTramLuongOT
						else 100
					end end as LaPhanTramOTQuyDoi

			from NS_CongBoSung bs
			left join
			(
			select 
				pc.ID, 
				pc.NgayApDung,
				pc.NgayKetThuc,
				pc.LoaiLuong,
				pc.ID_NhanVien, 
				ct.LuongNgayThuong,
				ct.Thu7_LaPhanTramLuong,
				ct.Thu7_GiaTri,
				ct.CN_LaPhanTramLuong,
				ct.ThCN_GiaTri,
				ct.NgayNghi_LaPhanTramLuong,
				ct.NgayNghi_GiaTri,
				ct.NgayLe_LaPhanTramLuong,
				ct.NgayLe_GiaTri,
				ct.ID_CaLamViec,
				ISNULL(ot.LuongNgayThuong,100) as LuongNgayThuongOT,
				ISNULL(ot.NgayThuong_LaPhanTramLuong,1) as NgayThuong_LaPhanTramLuongOT,
				ISNULL(ot.Thu7_LaPhanTramLuong,100) as Thu7_LaPhanTramLuongOT,
				ISNULL(ot.Thu7_GiaTri,100) as Thu7_GiaTriOT,
				ISNULL(ot.CN_LaPhanTramLuong,1) as CN_LaPhanTramLuongOT,
				ISNULL(ot.ThCN_GiaTri,100) as ThCN_GiaTriOT,
				ISNULL(ot.NgayNghi_LaPhanTramLuong,1) as NgayNghi_LaPhanTramLuongOT,
				ISNULL(ot.NgayNghi_GiaTri,100) as NgayNghi_GiaTriOT,
				ISNULL(ot.NgayLe_LaPhanTramLuong,1) as NgayLe_LaPhanTramLuongOT,
				ISNULL(ot.NgayLe_GiaTri,100) as NgayLe_GiaTriOT
			from NS_ThietLapLuongChiTiet ct
			join NS_Luong_PhuCap pc on ct.ID_LuongPhuCap= pc.ID
			left join (select ct2.* from NS_ThietLapLuongChiTiet ct2 where ID_LuongPhuCap = @ID_PhuCap and LaOT = 1 ) ot on pc.ID= ot.ID_LuongPhuCap
			where ct.LaOT= 0
			and pc.ID  like @ID_PhuCap
		) tlluong on bs.ID_NhanVien= tlluong.ID_NhanVien
		where bs.ID_NhanVien like @ID_NhanVien
		and bs.NgayCham >= @ngayapdung and (@ngayketthuc is null or bs.NgayCham <= @ngayketthuc)
		and bs.TrangThai in (1,2)");

			CreateStoredProcedure(name: "[dbo].[KhoiTaoDuLieuChamCong]", parametersAction: p => new
			{
				ID_DonVi = p.Guid(),
				NguoiTao = p.String(50)
			}, body: @"SET NOCOUNT ON;

		declare @tblDonVi table(ID_DonVi uniqueidentifier)
		insert into @tblDonVi
		select ID from DM_DonVi where TrangThai= 1 or TrangThai is null

		declare @countKyHieu int= (select count(*) from NS_KyHieuCong)
		if @countKyHieu = 0
		begin
			insert into NS_KyHieuCong (ID, KyHieu, MoTa, CongQuyDoi, ID_DonVi, LayGioMacDinh, TrangThai, TrangThaiCong)
			select *
			from
			(
				select NEWID() as ID, 'X' as KyHieu, N'Một ngày công' as MoTa,1 as CongQuyDoi, ID_DonVi, 1 as LayGioMacDinh, 1 as TrangThai, '' as TrangThaiCong
				from @tblDonVi

				union all
				select NEWID(), 'X/2', N'Nữa ngày công', 0.5, ID_DonVi, 1, 1, ''
				from @tblDonVi
		
				union all
				select NEWID(), 'N', N'Nghỉ', 0, ID_DonVi, 1, 1, ''
				from @tblDonVi
			) a			
		end

		-- insert ngaythuong
		declare @i int = 6
		while @i > 0
		begin
			insert into NS_NgayNghiLe (ID, Thu, LoaiNgay,TrangThai, NguoiTao, NgayTao)
			select NEWID(), @i, 0, 1, @NguoiTao, GETDATE()
			set @i= @i- 1
		end
		 -- chunhat
		insert into NS_NgayNghiLe (ID, Thu, LoaiNgay,TrangThai, NguoiTao, NgayTao)
		select NEWID(), 0, 1, 1, @NguoiTao, GETDATE()

		-- insert DMCaLamViec
		declare @idca uniqueidentifier = newid()
		insert into NS_CaLamViec (ID, MaCa, TenCa, GioVao, GioRa, NghiGiuaCaTu, NghiGiuaCaDen, TongGioCong, SoGioOTToiThieu, CachLayGioCong, LaCaDem, TrangThai, NguoiTao, NgayTao,
			ThoiGianDiMuonVeSom, SoPhutDiMuon,SoPhutVeSom, CaLamViec_KhongDau, CaLamViec_ChuCaiDau)
		values(@idca, 'CA000013', N'Ca cố định', '2020-01-01 08:00','2020-01-01 17:30','2020-01-01 12:00','2020-01-01 13:30',8,4,1,0,1,'333',GETDATE(),
			0, 0, 0, 'ca co dinh','ccd')

		insert into NS_CaLamViec_DonVi
		values( NEWID(),@idca,@ID_DonVi)");

			CreateStoredProcedure(name: "[dbo].[SaoChepThietLapLuong]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				ID_NhanVien = p.Guid(),
				KieuLuongs = p.String(50),
				IDNhanViens = p.String(),
				UpdateNVSetup = p.Boolean(),
				ID_NhanVienLogin = p.Guid()
			}, body: @"SET NOCOUNT ON;

	declare @tblKieuLuong table(LoaiLuong int)
	insert into @tblKieuLuong
	select Name from dbo.splitstring(@KieuLuongs)

	---- get tlapluong of nhanvien
	select *
	into #tempCoBan
	from NS_Luong_PhuCap pc
	where pc.ID_NhanVien = @ID_NhanVien and pc.ID_DonVi= @ID_ChiNhanh and pc.TrangThai !=0
	and exists(select LoaiLuong from @tblKieuLuong loai where pc.LoaiLuong= loai.LoaiLuong)

	---- get tlapluong chi tiet of nhanvien
	select ct.*
	into #tempChiTiet
	from NS_Luong_PhuCap pc
	join NS_ThietLapLuongChiTiet ct on pc.ID= ct.ID_LuongPhuCap
	where pc.ID_NhanVien = @ID_NhanVien and pc.ID_DonVi= @ID_ChiNhanh
	and pc.TrangThai !=0
	and exists(select LoaiLuong from @tblKieuLuong loai where pc.LoaiLuong= loai.LoaiLuong)

	declare @tblNhanVien table(ID_NhanVien uniqueidentifier)
	if @UpdateNVSetup = '0'	
		---- giu nguyen tlapluong cu (chi insert nhung nvien not exist in tlapluong)
		insert into @tblNhanVien
		select Name from dbo.splitstring(@IDNhanViens) tbl
		where not exists (select ID_NhanVien from NS_Luong_PhuCap pc where tbl.Name= pc.ID_NhanVien and pc.ID_DonVi= @ID_ChiNhanh and pc.TrangThai !=0)
		
	else
		---- capnhat lai thietlapluong
		begin
			insert into @tblNhanVien
			select Name from dbo.splitstring(@IDNhanViens)	where Name !=@ID_NhanVien

			---- xoa thietlapluong exist (chỉ xóa những loại sao chép)
			delete from NS_ThietLapLuongChiTiet 
			where ID_LuongPhuCap in
				(select ID 
				from NS_Luong_PhuCap pc 
				where pc.ID_DonVi = @ID_ChiNhanh
				and exists( select ID_NhanVien from @tblNhanVien nv where pc.ID_NhanVien =nv.ID_NhanVien)
				and exists(select LoaiLuong from @tblKieuLuong loai where pc.LoaiLuong= loai.LoaiLuong)
				)

			delete from NS_Luong_PhuCap
			where ID_DonVi = @ID_ChiNhanh
			and ID_NhanVien in ( select ID_NhanVien from @tblNhanVien)
			and LoaiLuong in (select LoaiLuong from @tblKieuLuong)
		end

	declare @IDNhanVien uniqueidentifier
	DECLARE curNhanVien CURSOR SCROLL LOCAL FOR
		select ID_NhanVien
		from @tblNhanVien 
	OPEN curNhanVien 
    	FETCH FIRST FROM curNhanVien
    	INTO @IDNhanVien
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				---- insert tlapcoban (neu 1Nvien co nhieu thietlapluong coban)
				declare @curIDPhuCap uniqueidentifier
				DECLARE curPhuCapCB CURSOR SCROLL LOCAL FOR
				select ID
				from #tempCoBan 
				OPEN curPhuCapCB 
    			FETCH FIRST FROM curPhuCapCB
				INTO @curIDPhuCap
				WHILE @@FETCH_STATUS = 0
				begin					
					declare @ID_PhuCap uniqueidentifier = NEWID()
					--select  @ID_PhuCap
					insert into NS_Luong_PhuCap
					select @ID_PhuCap, @IDNhanVien, ID_LoaiLuong, NgayApDung, NgayKetThuc, SoTien, HeSo, Bac, NoiDung, TrangThai, LoaiLuong, ID_DonVi
					from #tempCoBan where LoaiLuong not in (51,52,53,61,62,63) and ID= @curIDPhuCap

					---- insert tlapnangcao of luong
					insert into NS_ThietLapLuongChiTiet (ID, ID_LuongPhuCap, LuongNgayThuong, NgayThuong_LaPhanTramLuong, Thu7_GiaTri, Thu7_LaPhanTramLuong,
						ThCN_GiaTri, CN_LaPhanTramLuong, NgayNghi_GiaTri, NgayNghi_LaPhanTramLuong, NgayLe_GiaTri, NgayLe_LaPhanTramLuong,
						LaOT, ID_CaLamViec)
					select NEWID(), @ID_PhuCap,LuongNgayThuong,NgayThuong_LaPhanTramLuong,Thu7_GiaTri,Thu7_LaPhanTramLuong,
						ThCN_GiaTri, CN_LaPhanTramLuong, NgayNghi_GiaTri, NgayNghi_LaPhanTramLuong, NgayLe_GiaTri, NgayLe_LaPhanTramLuong,
						LaOT, ID_CaLamViec
					from #tempChiTiet where ID_LuongPhuCap= @curIDPhuCap

					FETCH NEXT FROM curPhuCapCB 
					INTO @curIDPhuCap
				end
				CLOSE curPhuCapCB  
    			DEALLOCATE curPhuCapCB 

				-- insert phucap + giamtru
				insert into NS_Luong_PhuCap
				select NewID(), @IDNhanVien, ID_LoaiLuong, NgayApDung, NgayKetThuc, SoTien, HeSo, Bac, NoiDung, TrangThai, LoaiLuong, ID_DonVi
				from #tempCoBan where LoaiLuong in (51,52,53,61,62,63)			

				FETCH NEXT FROM curNhanVien 
				INTO @IDNhanVien
			END;
			CLOSE curNhanVien  
    		DEALLOCATE curNhanVien 

		declare @loailuong nvarchar(200) =''
		if (select count(*) from @tblKieuLuong where LoaiLuong in(1,2,3,4)) > 0
			set @loailuong =N'lương,'
		if (select count(*) from @tblKieuLuong where LoaiLuong like '%5%') > 0
			set @loailuong = @loailuong + N' phụ cấp,'
		if (select count(*) from @tblKieuLuong where LoaiLuong like '%6%') > 0
			set @loailuong = @loailuong + N' giảm trừ'

		declare @tenNhanVien nvarchar(200), @maNhanVien nvarchar(100)
		select @tenNhanVien = TenNhanVien, @maNhanVien = MaNhanVien from NS_NhanVien where ID= @ID_NhanVien

		declare @nhanvienSetup nvarchar(max) = (
		select  concat( TenNhanVien ,' (',MaNhanVien, ')') + ', ' as [text()] 
		from NS_NhanVien nv1 
		where exists (select ID from @tblNhanVien nv2 where nv1.ID= nv2.ID_NhanVien)
		for xml path(''))
		
		insert into HT_NhatKySuDung (ID, ID_DonVi, ID_NhanVien, LoaiNhatKy, ChucNang, ThoiGian, NoiDung, NoiDungChiTiet)
		values (NEWID(), @ID_ChiNhanh, @ID_NhanVienLogin, 1, N'Thiết lập lương - Sao chép', GETDATE(),
		concat(N'Sao chép thiết lập lương ', N'(', @loailuong , N') từ nhân viên <b>', @tenNhanVien , ' (',  @maNhanVien ,' </b>)'),
		concat(N'Sao chép thiết lập lương ', N'(', @loailuong , N') từ nhân viên <b>', @tenNhanVien , ' (',  @maNhanVien , N'</b>) đến: ', @nhanvienSetup) )");

			Sql(@"CREATE PROCEDURE [dbo].[TinhGiamTruLuong]
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

	SET NOCOUNT ON;

		 -- ==== GIAM TRU CODINH ====
		declare @tblGiamTruCD table (ID_NhanVien uniqueidentifier, IDPhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, GiamTruTheoLuong float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblGiamTruCD
		select *			
		from @tblThietLapLuong pc
		where pc.LoaiLuong = 62	

		declare @tblCong1 table (ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, GiamTruTheoLuong float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoLanDiMuon float, NgayApDung datetime, NgayKetThuc datetime)
		declare @cd_ID_NhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_LoaiLuong int, @cd_GiamTruTheoLuong float, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblGiamTruCD
		OPEN curPhuCap 
    	FETCH FIRST FROM curPhuCap
    	INTO  @cd_ID_NhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_GiamTruTheoLuong, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong1
				select @cd_ID_NhanVien, tmp.ID_ChamCongChiTiet, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_GiamTruTheoLuong, @cd_HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(SoPhutDiMuon>0,1,0)) as SoLanDiMuon,
					@cd_NgayApDung, @cd_NgayKetThuc
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @cd_ID_NhanVien and tmp.NgayCham >= @cd_NgayApDung and (@cd_NgayKetThuc is null OR tmp.NgayCham <= @cd_NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @cd_ID_NhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_GiamTruTheoLuong, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	
		

		 -- ==== GIAM TRU THEO SO LAN ====

		-- get giamtru theosolan trong khoang thoigian
		declare @tblGiamTruLan table (IDPhuCap uniqueidentifier, ID_NhanVien uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, GiamTruTheoLan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblGiamTruLan
		select *			
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong = 61		

		-- bảng tính số lần đi muộn, về sớm theo phiếu phân ca, ca làm việc
		declare @tblCong2 table ( ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, GiamTruTheoLan float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), SoLanDiMuon float, NgayApDung datetime, NgayKetThuc datetime)

		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @IDPhuCap uniqueidentifier, @TenPhuCap nvarchar(max), @LoaiLuong int, @GiamTruTheoLan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblGiamTruLan
		OPEN curPhuCap -- cur 1
    	FETCH FIRST FROM curPhuCap
    	INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong2
				select @ID_NhanVien, tmp.ID_ChamCongChiTiet, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(SoPhutDiMuon >0,1,0))  as SoLanDiMuon,
					@NgayApDung, @NgayKetThuc
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 
				
				FETCH NEXT FROM curPhuCap INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong, @GiamTruTheoLan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 		
			
			--select * from @tblCong2
				select pc.ID_NhanVien, 
					SUM(pc.GiamTruTheoLuong) as GiamTruTheoLuong,
					SUM(pc.GiamTruTheoLan) as GiamTruTheoLan,
					SUM(SoLanDiMuon) as SoLanDiMuon				
				from 
					(select ID_NhanVien, GiamTruTheoLuong * HeSo as GiamTruTheoLuong, 0 as SoLanDiMuon, 0 as GiamTruTheoLan
					from @tblCong1 			
					where SoLanDiMuon > 0
					group by ID_NhanVien, ID_PhuCap, GiamTruTheoLuong, HeSo

					union all

					select ID_NhanVien, 0 as GiamTruTheoLuong,SoLanDiMuon, GiamTruTheoLan * HeSo * SoLanDiMuon as GiamTruTheoLan 
					from @tblCong2
					) pc group by pc.ID_NhanVien 

		
END");

			CreateStoredProcedure(name: "[dbo].[UpdateStatusCongBoSung_WhenCreatBangLuong]", parametersAction: p => new
			{
				ID_BangLuong = p.Guid(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;	
	declare @statusSalary int = (select TrangThai from NS_BangLuong where ID= @ID_BangLuong)
	declare @statusCong int = @statusSalary
	if	@statusSalary = 1 -- taomoi
		set @statusCong = @statusSalary + 1
	else
		if	@statusSalary = 0 -- huybangluong
			set @statusCong = 1
	
	update bs set TrangThai = @statusCong, -- 1. taomoi 2.BangLuong TamLuu, 3. BangLuong DaChot, 4. dathanhtoan
				 ID_BangLuongChiTiet = iif(@statusSalary=0,null,blct.ID)
	from NS_CongBoSung bs
	join NS_BangLuong_ChiTiet blct on bs.ID_NhanVien= blct.ID_NhanVien
	where blct.ID_BangLuong= @ID_BangLuong
	and bs.NgayCham>= @FromDate and bs.NgayCham <= @ToDate");

			CreateStoredProcedure(name: "[dbo].[TinhLaiBangLuong]", parametersAction: p => new
			{
				ID_BangLuong = p.Guid(),
				NguoiSua = p.String()
			}, body: @"SET NOCOUNT ON;
	declare @IDNhanVienLogin uniqueidentifier= (select top 1 ID_NhanVien from HT_NguoiDung where LaAdmin='1')

	select bl.TuNgay, bl.DenNgay, bl.ID_DonVi, ct.ID_NhanVien	
	into #tempbangluong
	from NS_BangLuong_ChiTiet ct
	join NS_BangLuong bl on ct.ID_BangLuong= bl.ID
	where bl.ID like @ID_BangLuong

	declare @IDChiNhanhs uniqueidentifier, @FromDate datetime, @ToDate datetime, @KieuLuongs varchar(10)= '1,2,3,4'
	select @IDChiNhanhs = ID_DonVi, @FromDate = TuNgay, @ToDate = DenNgay from (select top 1 * from  #tempbangluong ) a


	declare @IDNhanViens varchar(max) = 	
		(select cast(ID_NhanVien as varchar(40)) +',' AS [text()]
		from #tempbangluong
		for xml path('')
		) 

		drop table #tempbangluong

		set @IDNhanViens = LEFT(@IDNhanViens,LEN(@IDNhanViens)-1) -- remove last comma

		declare @ngaycongchuan float = (select dbo.TinhNgayCongChuan(@FromDate,@ToDate,@IDChiNhanhs))
		
		declare @tblCong CongThuCong
		insert into @tblCong
		exec dbo.GetChiTietCongThuCong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		declare @tblThietLapLuong ThietLapLuong
		insert into @tblThietLapLuong
		exec GetNS_ThietLapLuong @IDChiNhanhs,@IDNhanViens, @FromDate, @ToDate

		declare @tblLuong table (LoaiLuong int, ID_NhanVien uniqueidentifier, LuongCoBan float, SoNgayDiLam float, LuongChinh float)				
		insert into @tblLuong		
		exec TinhLuongCoBan @ngaycongchuan, @tblCong, @tblThietLapLuong

		declare @tblLuongOT table (ID_NhanVien uniqueidentifier, LuongOT float)				
		insert into @tblLuongOT		
		exec TinhLuongOT @ngaycongchuan, @tblCong, @tblThietLapLuong
		
		declare @tblPhuCap table (ID_NhanVien uniqueidentifier, PhuCapCoDinh float, PhuCapTheoNgayCong float)
		insert into @tblPhuCap
		exec TinhPhuCapLuong @tblCong, @tblThietLapLuong

		declare @tblPhuCapTheoPtram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  PhuCapCoDinh float, HeSo float, NgayApDung datetime, NgayKetThuc datetime, SoNgayDiLam float)
		insert into @tblPhuCapTheoPtram
		exec GetPhuCapCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%', @tblCong, @tblThietLapLuong


		declare @tblGiamTru table (ID_NhanVien uniqueidentifier, GiamTruCoDinhVND float, GiamTruTheoLan float, SoLanDiMuon float)
		insert into @tblGiamTru
		exec TinhGiamTruLuong @tblCong, @tblThietLapLuong	

		-- get phucap codinh theo %luongchinh
	declare @tblLuongPC table (ID_NhanVien uniqueidentifier,LoaiLuong int, LuongCoBan float, SoNgayDiLam float, LuongChinh float,PhuCapCoDinh_TheoPtramLuong float)						
	insert into @tblLuongPC	
	select 
		pcluong.ID_NhanVien, pcluong.LoaiLuong, pcluong.LuongCoBan, pcluong.SoNgayDiLam, pcluong.LuongChinh, 
		sum(PhuCapCoDinh_TheoPtramLuong) as PhuCapCoDinh_TheoPtramLuong
	from
		(select luong.ID_NhanVien, LoaiLuong, LuongCoBan, luong.SoNgayDiLam, LuongChinh,
			case when PhuCapCoDinh is null then 0 else LuongChinh * PhuCapCoDinh * HeSo/100 end as PhuCapCoDinh_TheoPtramLuong
		from @tblLuong luong
		left join @tblPhuCapTheoPtram pc on luong.ID_NhanVien= pc.ID_NhanVien
		) pcluong 
		group by pcluong.ID_NhanVien, pcluong.LuongChinh, pcluong.LoaiLuong, pcluong.LuongCoBan,pcluong.SoNgayDiLam

		--select * from  @tblLuongPC


	-- hoahong		
	DECLARE @tab_DoanhSo TABLE (ID_NhanVien uniqueidentifier, MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), TongDoanhThu float, TongThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, TongAll float,
	Status_DoanhThu nvarchar(10), TotalRow int, TotalPage float, TongAllDoanhThu float,TongAllThucThu float, TongHoaHongDoanhThu float, TongHoaHongThucThu float, TongAllAll float) 
	INSERT INTO @tab_DoanhSo exec getList_ChietKhauNhanVienTheoDoanhSo @IDChiNhanhs, @IDNhanVienLogin,'', @FromDate, @ToDate,'%%',0,1000;

	DECLARE @tab_HoaDon TABLE (ID_NhanVien uniqueidentifier,MaNhanVien nvarchar(255), TenNhanVien nvarchar(max), HoaHongThucThu float, HoaHongDoanhThu float, HoaHongVND float, TongAll float,
	TotalRow int, TotalPage float, TongHHDoanhThu float,TongHHThucThu float, TongHHVND float, TongAllAll float)
	INSERT INTO @tab_HoaDon exec SP_ReportDiscountInvoice @IDChiNhanhs,@IDNhanVienLogin,'%%', @FromDate, @ToDate, 8,1,0,0,100

	DECLARE @tab_HangHoa TABLE (MaNhanVien nvarchar(255), TenNhanVien nvarchar(max),ID_NhanVien uniqueidentifier, HoaHongThucHien float, HoaHongThucHien_TheoYC float, HoaHongTuVan float, HoaHongBanGoiDV float, Tong float,
	TotalRow int, TotalPage float, TongHoaHongThucHien float,TongHoaHongThucHien_TheoYC float, TongHoaHongTuVan float, TongHoaHongBanGoiDV float, TongAll float)
	INSERT INTO @tab_HangHoa exec SP_ReportDiscountProduct_General @IDChiNhanhs,@IDNhanVienLogin,'%%', @FromDate, @ToDate, 16,1,0,100

	declare @tblHoaHong table (ID_NhanVien uniqueidentifier, TongDoanhThu float, HoaHong float, HoaHongHangHoa float, HoaHongHoaDon float, HoaHongDoanhThu float)
	insert into  @tblHoaHong
	SELECT a.ID_NhanVien, sum(TongDoanhThu) as TongDoanhThu,				
		SUM(TongDoanhSo + TongHoaDon + TongHangHoa) as HoaHong,
		SUM(TongHangHoa) as HoaHongHangHoa,
		SUM(TongHoaDon) as HoaHongHoaDon,
		SUM(TongDoanhSo) as HoaHongDoanhThu
	FROM 
	(
		select ID_NhanVien,	Tong as TongHangHoa,0 as TongHoaDon, 0 as TongDoanhSo, 0 as TongDoanhThu						
		from @tab_HangHoa
		UNION ALL
		Select ID_NhanVien,	0 as TongHangHoa,TongAll as TongHoaDon,	0 as TongDoanhSo, 0 as TongDoanhThu							
		from @tab_HoaDon
		UNION ALL
		Select ID_NhanVien,	0 as TongHangHoa,0 as TongHoaDon,TongAll as TongDoanhSo, TongDoanhThu
		from @tab_DoanhSo
	) as a
	GROUP BY a.ID_NhanVien

	-- giamtru codinh %tongluongnhan
	declare @tblGiamTruTheoPTram table (ID_NhanVien uniqueidentifier,ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max),  GiamTruCoDinh float, HeSo float,
		NgayApDung datetime, NgayKetThuc datetime, SoLanDiMuon float)
	insert into @tblGiamTruTheoPTram
	exec GetGiamTruCoDinh_TheoPtram @IDChiNhanhs, @FromDate, @ToDate, '%%',@tblCong, @tblThietLapLuong	

	select nv.MaNhanVien, nv.TenNhanVien, 
			luong.*,
			cast(PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong as float) as PhuCap,
			cast(PhatDiMuon + GiamTruCoDinhVND as float) as TongTienPhat,
			cast(LuongChinh as float)  as TongLuongNhan, -- save to DB
			cast(LuongChinh + LuongOT +  PhuCapCoBan + PhuCapKhac + PhuCapCoDinh_TheoPtramLuong + KhenThuong + ChietKhau - PhatDiMuon - GiamTruCoDinhVND as float) as LuongThucNhan
	into #tblluong
		from
			(
			select 
				tbl.ID_NhanVien, 
				max(tbl.LoaiLuong) as LoaiLuong,
				max(tbl.LuongCoBan) as LuongCoBan,
				sum(tbl.LuongChinh) as LuongChinh,
				sum(LuongOT) as LuongOT,	
				sum(PhuCapCoDinh_TheoPtramLuong) as PhuCapCoDinh_TheoPtramLuong,				
				sum(PhuCapCoDinh) as PhuCapCoBan,
				sum(PhuCapTheoNgay) as PhuCapKhac,
				sum(GiamTruCoDinhVND) as GiamTruCoDinhVND,
				sum(GiamTruTheoLan) as PhatDiMuon,
				sum(HoaHong) as ChietKhau,
				sum(HoaHongHangHoa) as HoaHongHangHoa,
				sum(HoaHongHoaDon) as HoaHongHoaDon,
				sum(HoaHongDoanhThu) as HoaHongDoanhThu,
				sum(TongDoanhThu) as TongDoanhThu,
				sum(SoNgayDiLam) as NgayCongThuc,
				sum(SoGioOT) as SoGioOT,
				sum(SoLanDiMuon) as SoLanDiMuon,
				sum(KhenThuong) as KhenThuong,
				@ngaycongchuan as  NgayCongChuan
			from 
				(select 
					ID_NhanVien, LoaiLuong, LuongCoBan, LuongChinh,
					cast(0 as float) as LuongOT, 
					SoNgayDiLam, cast(0 as float) as SoGioOT, 
					PhuCapCoDinh_TheoPtramLuong,
					cast(0 as float) as PhuCapCoDinh, cast(0 as float) as PhuCapTheoNgay,
					cast(0 as float) as GiamTruCoDinhVND, cast(0 as float) as GiamTruTheoLan, cast(0 as float) as SoLanDiMuon,
					cast(0 as float) as HoaHong,cast(0 as float) as HoaHongHangHoa, cast(0 as float) as HoaHongHoaDon, cast(0 as float) as HoaHongDoanhThu, 0 as TongDoanhThu,
					cast(0 as float) as KhenThuong
				from @tblLuongPC

				union all

				select 
					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan, 0 as LuongChinh,
					LuongOT,
					0 as SoNgayDiLam,0 as SoGioOT,
					0 as PhuCapCoDinh_TheoPtramLuong,
					0 as PhuCapCoDinh, 0 as PhuCapTheoNgay,
					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
					0 as HoaHong, 0 as HoaHongHangHoa,0 as HoaHongHoaDon, 0 as HoaHongDoanhThu, 0 as TongDoanhThu,
					0 as KhenThuong
				from @tblLuongOT

				union all
				select 
					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan, 0 as LuongChinh, 0 as LuongOT, 
					0 as SoNgayDiLam, 0 as SoGioOT,
					0 as PhuCapCoDinh_TheoPtramLuong,
					PhuCapCoDinh, PhuCapTheoNgayCong,
					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
					0 as KhenThuong
				from @tblPhuCap

				union all
				select 
					ID_NhanVien, 0 as LoaiLuong,0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
					0 as SoNgayDiLam, 0 as SoGioOT,
					0 as PhuCapCoDinh_TheoPtramLuong,
					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
					GiamTruCoDinhVND, GiamTruTheoLan, SoLanDiMuon,
					0 as HoaHong,0 as HoaHongHangHoa, 0 as HoaHongHoaDon, 0 as HoaHongDoanhThu,0 as TongDoanhThu,
					0 as KhenThuong
				from @tblGiamTru

				union all
				select 
					ID_NhanVien, 0 as LoaiLuong, 0 as LuongCoBan,0 as LuongChinh, 0 as LuongOT, 
					0 as SoNgayDiLam, 0 as SoGioOT, 
					0 as PhuCapCoDinh_TheoPtramLuong,
					0 as PhuCapCoDinh, 0 as PhuCapTheoNgayCong,
					0 as GiamTruCoDinhVND, 0 as GiamTruTheoLan, 0 as SoLanDiMuon,
					HoaHong, HoaHongHangHoa, HoaHongHoaDon, HoaHongDoanhThu, TongDoanhThu,
					0 as KhenThuong
				from @tblHoaHong
				) tbl group by tbl.ID_NhanVien
			) luong
		join NS_NhanVien nv on luong.ID_NhanVien= nv.ID
		where LuongChinh > 0 OR LuongOT> 0

		-- get max maphieuluong 
		declare @maxcodePL varchar(20) = (select ISNULL(MAX(CAST (dbo.udf_GetNumeric(MaBangLuongChiTiet) AS float)),0) 
		from NS_BangLuong_ChiTiet where ID_BangLuong != @ID_BangLuong)

		exec DeleteBangLuongChiTietById @ID_BangLuong
		
		insert into NS_BangLuong_ChiTiet
		select  
			NEWID(), @ID_BangLuong, ID_NhanVien, NgayCongThuc, NgayCongChuan, LuongCoBan, 
			PhuCapCoBan + PhuCapCoDinh_TheoPtramLuong as PhuCapCoBan, -- phucapcoban in db
			PhuCapKhac,
			KhenThuong,
			0, 0, 0,0, -- thue, mienthue, baohiem,kyluat
			PhatDiMuon,
			LuongOT,
			LuongThucNhan - GiamTruCoDinh_TheoPTram, -- luong chinh + ot + hoahong + phucap - phat
			TongTienPhat + GiamTruCoDinh_TheoPTram,
			TongLuongNhan, -- luongchinh
			N'tính lại lương', -- ghichu
			1, -- tranngthai
			@NguoiSua, GETDATE(),
			@NguoiSua, GETDATE(),
			0, -- baohiemcty
			TongDoanhThu, -- doanhso
			ChietKhau,
			MaPhieu
		from
			(
				select *, CONCAT('PL0000', RN + @maxcodePL) as MaPhieu
				from
				(
					select luong.* ,
						ISNULL(ISNULL(gt.GiamTruCoDinh,0) * gt.HeSo * luong.TongLuongNhan/100,0) as GiamTruCoDinh_TheoPTram,
						ROW_NUMBER() over (order by luong.MaNhanVien) RN 
					from #tblluong luong
					left join @tblGiamTruTheoPTram gt on luong.ID_NhanVien= gt.ID_NhanVien
					where exists(select Name from dbo.splitstring(@KieuLuongs) kl where luong.LoaiLuong= kl.Name)
					and exists(select Name from dbo.splitstring(@IDNhanViens) nv where luong.ID_NhanVien= nv.Name)
				) pluong
			) a

		update NS_BangLuong set TrangThai= 1, NguoiSua = @NguoiSua, NgaySua= GETDATE() where id= @ID_BangLuong

		-- update status, id_bangluongchitiet in NS_CongBoSung
		exec UpdateStatusCongBoSung_WhenCreatBangLuong @ID_BangLuong, @FromDate, @ToDate");

			Sql(@"CREATE PROCEDURE [dbo].[TinhLuongCoBan]
	@NgayCongChuan int,
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN
	
	SET NOCOUNT ON;		

		
		--declare @IDChiNhanhs uniqueidentifier='D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'
		--declare @FromDate datetime='2020-08-01'
		--declare @ToDate datetime='2020-08-31'
		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs,'', @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs,'', @FromDate, @ToDate

		-- get thietlapluong (codinh + ngay)
		declare @tblLuongCDNgay table (ID_NhanVien uniqueidentifier,ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblLuongCDNgay
		select *		
		from @tblThietLapLuong pc 
		where  pc.LoaiLuong in (1,2)

		--select * from @tblLuongCDNgay

		declare @tblCongCDNgay table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), SoNgayDiLam float)
		
		declare @cdID_NhanVien uniqueidentifier, @cdID uniqueidentifier, @cdTenLoaiLuong nvarchar(max), @cdLoaiLuong int,@cdLuongCoBan float, @cdHeSo int, @cdNgayApDung datetime, @cdNgayKetThuc datetime
		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblLuongCDNgay
		OPEN curLuong -- cur 1
    	FETCH FIRST FROM curLuong
    	INTO @cdID_NhanVien, @cdID, @cdTenLoaiLuong, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, @cdNgayApDung, @cdNgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongCDNgay
				select @cdID_NhanVien, @cdID, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam				
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @cdID_NhanVien and tmp.NgayCham >= @cdNgayApDung and (@cdNgayKetThuc is null OR tmp.NgayCham <= @cdNgayKetThuc )  
				group by tmp.ID_NhanVien, tmp.ID_CaLamViec, tmp.TenCa		
				FETCH NEXT FROM curLuong INTO @cdID_NhanVien, @cdID, @cdTenLoaiLuong, @cdLoaiLuong, @cdLuongCoBan, @cdHeSo, @cdNgayApDung, @cdNgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 

		-- get caidatluong (ca + gio)
		declare @tblLuongCaGio table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblLuongCaGio
		select *
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong in (3,4)


		-- bảng tính công theo phiếu phân ca, ca làm việc
		declare @tblCong table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
		SoNgayDiLam float, Luong float)				
	
		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int, @LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblLuongCaGio
		OPEN curLuong 
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				select ct.*
				into #tmpca
				from NS_ThietLapLuongChiTiet ct
				where ID_LuongPhuCap= @ID_PhuCap and LaOT= 0
				
				insert into @tblCong
				select 
					 @ID_NhanVien, @ID_PhuCap, @LoaiLuong, @LuongCoBan,@HeSo,@NgayApDung, @NgayKetThuc,
					 tbl.ID_CaLamViec, tbl.TenCa, tbl.TongGioCong1Ca,
					 tbl.TongCong,
					case when @LoaiLuong = 3 then LuongCoBanQuyDoi * tbl.Cong else (tbl.Cong * TongGioCong1Ca - SoPhutDiMuon * 1.0/60 +  SoGioOT) * LuongCoBanQuyDoi  end as ThanhTien						
				from
					(
						select 
							luong.ID_CaLamViec, luong.TenCa, luong.TongGioCong1Ca,
							luong.Cong, luong.SoPhutDiMuon, luong.SoGioOT, 
							luong.LuongNgayThuong, luong.LoaiNgayThuong_Nghi,
							luong.GiaTri as HeSoLuong,luong.LaPhanTram,
							case when @LoaiLuong= 3 then luong.Cong else luong.Cong * luong.TongGioCong1Ca - luong.SoPhutDiMuon * 1.0/60 +  luong.SoGioOT end as TongCong,	
							case when luong.LaPhanTram= 1 then @HeSo * luong.LuongNgayThuong else @HeSo * luong.GiaTri end as LuongCoBan,
							case when luong.LaPhanTram= 1 then @HeSo * luong.LuongNgayThuong * luong.GiaTri/100 else @HeSo * luong.GiaTri end as LuongCoBanQuyDoi
					
						from 
							(
							select 
								qd.ID_CaLamViec, qd.TenCa, qd.TongGioCong1Ca,
								qd.Cong, qd.SoPhutDiMuon, qd.SoGioOT,
								isnull(qd.LuongNgayThuong, LuongNgayThuongNull) as LuongNgayThuong,
								case when qd.LoaiNgay= 0 then case when qd.Thu not in (0,6) then 3 else qd.Thu end								
									else case when qd.Thu in (0,6) then qd.Thu else qd.LoaiNgay end end as LoaiNgayThuong_Nghi,
								case qd.Thu
									when 6 then case when Thu7_GiaTri is not null then qd.Thu7_GiaTri else Thu7_GiaTriNull end
									when 0 then case when ThCN_GiaTri is not null then qd.ThCN_GiaTri else ThCN_GiaTriNull end
								else	
									case qd.LoaiNgay
										when 0 then case when qd.LuongNgayThuong is not null then LuongNgayThuong else LuongNgayThuongNull end
										when 1 then case when qd.NgayNghi_GiaTri is not null then NgayNghi_GiaTri else NgayNghi_GiaTriNull end
										when 0 then case when qd.NgayLe_GiaTri is not null then NgayLe_GiaTri else NgayLe_GiaTriNull end
									end
								end
								as GiaTri,

								case qd.Thu
									when 6 then case when Thu7_LaPhanTramLuong is not null then qd.Thu7_LaPhanTramLuong else Thu7_LaPhanTramLuongNull end
									when 0 then case when CN_LaPhanTramLuong is not null then qd.CN_LaPhanTramLuong else CN_LaPhanTramLuongNull end
								else	
									case qd.LoaiNgay
										when 0 then 0
										when 1 then case when qd.NgayNghi_LaPhanTramLuong is not null then NgayNghi_LaPhanTramLuong else NgayNghi_LaPhanTramLuongNull end
										when 0 then case when qd.NgayLe_LaPhanTramLuong is not null then NgayLe_LaPhanTramLuong else NgayLe_LaPhanTramLuongNull end
									end
								end
								as LaPhanTram
							
							from
							(
								select tmp.*, 
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) LuongNgayThuong,
									(select top 1 LuongNgayThuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) LuongNgayThuongNull,

									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_GiaTri,
									(select top 1 Thu7_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_GiaTriNull,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) Thu7_LaPhanTramLuong,
									(select top 1 Thu7_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) Thu7_LaPhanTramLuongNull,
						
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) CN_LaPhanTramLuong,
									(select top 1 CN_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) CN_LaPhanTramLuongNull,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) ThCN_GiaTri,
									(select top 1 ThCN_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) ThCN_GiaTriNull,

									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_GiaTri,
									(select top 1 NgayNghi_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_GiaTriNull,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayNghi_LaPhanTramLuong,
									(select top 1 NgayNghi_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayNghi_LaPhanTramLuongNull,

									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_GiaTri,
									(select top 1 NgayLe_GiaTri from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_GiaTriNull,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmp.ID_CaLamViec = tmpca.ID_CaLamViec) NgayLe_LaPhanTramLuong,
									(select top 1 NgayLe_LaPhanTramLuong from #tmpca tmpca where tmpca.ID_CaLamViec is null) NgayLe_LaPhanTramLuongNull
						
								from @tblCongThuCong tmp
								where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
							) qd
						) luong
					)tbl

					drop table #tmpca

				FETCH NEXT FROM curLuong 
				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	

		select max(LoaiLuong) as LoaiLuong, 
			ID_NhanVien, 
			max(LuongCoBan) as LuongCoBan,
			sum(SoNgayDiLam) as SoNgayDiLam,
			sum(Luong) as Luong
		from
		(
			select luongcagio.ID_NhanVien, luongcagio.LoaiLuong,
				max(LuongCoBan) as LuongCoBan,
				sum(SoNgayDiLam) as SoNgayDiLam,
				sum(luongcagio.Luong) as Luong
			from @tblCong luongcagio
			group by ID_NhanVien, LoaiLuong

			union all

			select luongcd.ID_NhanVien, luongcd.LoaiLuong,
				max(LuongCoBan) as LuongCoBan,
				sum(SoNgayDiLam) as SoNgayDiLam,
				case when LoaiLuong = 1 then LuongCoBan
				else case when sum(SoNgayDiLam) > @NgayCongChuan then LuongCoBan else LuongCoBan/@NgayCongChuan * sum(SoNgayDiLam) end
				end as Luong
			from @tblCongCDNgay luongcd		
			group by ID_NhanVien, LoaiLuong,LuongCoBan
		) luong
		group by luong.ID_NhanVien
			
END");

			Sql(@"CREATE PROCEDURE [dbo].[TinhLuongOT]
	@NgayCongChuan int,
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN
	
	SET NOCOUNT ON;	

		--declare @IDChiNhanhs varchar(max)='d93b17ea-89b9-4ecf-b242-d03b8cde71de'
		--declare @FromDate datetime='2020-08-01'
		--declare @ToDate datetime='2020-08-31'

		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs,'', @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs,'', @FromDate, @ToDate

		---- ===== OT theo ngay ======================
		declare @thietlapOTNgay table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @thietlapOTNgay
		select *	
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong = 2

		select  a.*,
				case when LaPhanTram= 0 then GiaTri else (SoTien/@NgayCongChuan/8) * GiaTri /100 end as Luong1GioCong			
			into #temp1					
			from
					(select TenNhanVien, bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
						bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
						pc.SoTien,
					
						pc.LoaiLuong,
						pc.HeSo,
						case bs.Thu
								when 6 then tlct.Thu7_GiaTri
								when 0 then tlct.ThCN_GiaTri
							else
								case bs.LoaiNgay 
									when 0 then LuongNgayThuong
									when 1 then tlct.NgayNghi_GiaTri
									when 2 then tlct.NgayLe_GiaTri
									end
								end as GiaTri,
						case bs.Thu
							when 6 then tlct.Thu7_LaPhanTramLuong
							when 0 then tlct.CN_LaPhanTramLuong
						else
						case bs.LoaiNgay  
							when 0 then tlct.NgayThuong_LaPhanTramLuong
							when 1 then tlct.NgayNghi_LaPhanTramLuong
							when 2 then tlct.NgayLe_LaPhanTramLuong
							end
						end as LaPhanTram
					from @tblCongThuCong bs
					join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
					join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap 
					join NS_NhanVien nv on pc.ID_NhanVien= nv.ID				
					where tlct.LaOT= 1 and pc.LoaiLuong = 2
				)a

		declare @tblCongOTNgay table (ID_PhuCap uniqueidentifier,  ID_NhanVien uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
		LuongOT float, NgayCham datetime)			
		
		declare @otngayID_NhanVien uniqueidentifier, @otngayID_PhuCap uniqueidentifier, @otngayTenLoaiLuong nvarchar(max),
		@otngayLoaiLuong int,@otngayLuongCoBan float, @otngayHeSo int, @otngayNgayApDung datetime, @otngayNgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTNgay
		OPEN curLuong -- cur 1
    	FETCH FIRST FROM curLuong
    	INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongOTNgay
				select @otngayID_PhuCap,@otngayID_NhanVien, @otngayLoaiLuong,@otngayLuongCoBan,@otngayHeSo,@otngayNgayApDung, @otngayNgayKetThuc,
					tmp.ID_CaLamViec, tmp.TenCa, tmp.TongGioCong1Ca,
					tmp.SoGioOT * Luong1GioCong as LuongOT,
					tmp.NgayCham
				from #temp1 tmp
				where tmp.ID_NhanVien = @otngayID_NhanVien and tmp.NgayCham >= @otngayNgayApDung and (@otngayNgayKetThuc is null OR tmp.NgayCham <= @otngayNgayKetThuc )  								
				FETCH NEXT FROM curLuong 
				INTO @otngayID_NhanVien, @otngayID_PhuCap, @otngayTenLoaiLuong, @otngayLoaiLuong, @otngayLuongCoBan, @otngayHeSo, @otngayNgayApDung, @otngayNgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	


			--- ======= OT Theo Ca =================
		declare @thietlapOTCa table (ID_NhanVien uniqueidentifier, ID uniqueidentifier, TenLoaiLuong nvarchar(max), LoaiLuong int, LuongCoBan float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @thietlapOTCa
		select *	
		from @tblThietLapLuong pc 		
		where pc.LoaiLuong = 3
		
		-- get cong OT ca
		select  a.*,				
				case when LaPhanTram = 0 then GiaTri else case when LuongTheoCa is null then SoTien/TongGioCong1Ca * GiaTri/100
					else LuongTheoCa/TongGioCong1Ca* GiaTri/100 end end as Luong1GioCong
			into #temp2					
			from
				(select bs.ID_CaLamViec, bs.TenCa, bs.TongGioCong1Ca, bs.ID_NhanVien,
					bs.NgayCham, bs.LoaiNgay, bs.KyHieuCong, bs.SoGioOT, bs.Thu,
					pc.SoTien,
					theoca.LuongTheoCa,
					pc.LoaiLuong,
					pc.HeSo,
					case bs.Thu
							when 6 then tlct.Thu7_GiaTri
							when 0 then tlct.ThCN_GiaTri
						else
							case bs.LoaiNgay 
								when 0 then LuongNgayThuong
								when 1 then tlct.NgayNghi_GiaTri
								when 2 then tlct.NgayLe_GiaTri
								end
							end as GiaTri,
					case bs.Thu
						when 6 then tlct.Thu7_LaPhanTramLuong
						when 0 then tlct.CN_LaPhanTramLuong
					else
					case bs.LoaiNgay  
						when 0 then tlct.NgayThuong_LaPhanTramLuong
						when 1 then tlct.NgayNghi_LaPhanTramLuong
						when 2 then tlct.NgayLe_LaPhanTramLuong
						end
					end as LaPhanTram
				from @tblCongThuCong bs
				join NS_Luong_PhuCap pc  on bs.ID_NhanVien= pc.ID_NhanVien
				join NS_ThietLapLuongChiTiet tlct on pc.ID= tlct.ID_LuongPhuCap -- luongot
				left join (select tlca.LuongNgayThuong as LuongTheoCa, tlca.ID_CaLamViec, pca.ID_NhanVien -- get luongcoban
						from NS_Luong_PhuCap pca
						join NS_ThietLapLuongChiTiet tlca on pca.ID= tlca.ID_LuongPhuCap 
						where tlca.LaOT= 0
						) theoca on pc.ID_NhanVien= theoca.ID_NhanVien and bs.ID_CaLamViec= theoca.ID_CaLamViec
				where tlct.LaOT= 1
				and pc.LoaiLuong = 3		
				) a			

		declare @tblCongOTCa table (ID_PhuCap uniqueidentifier,  ID_NhanVien uniqueidentifier, LoaiLuong float, LuongCoBan float, HeSo int,NgayApDung datetime, NgayKetThuc datetime,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100), TongGioCong1Ca float, 
		LuongOT float, NgayCham datetime)				
	
		declare @ID_NhanVien uniqueidentifier, @ID_PhuCap uniqueidentifier, @TenLoaiLuong nvarchar(max), @LoaiLuong int,@LuongCoBan float, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curLuong CURSOR SCROLL LOCAL FOR
    		select *
    		from @thietlapOTCa
		OPEN curLuong
    	FETCH FIRST FROM curLuong
    	INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCongOTCa
				select @ID_PhuCap,@ID_NhanVien, @LoaiLuong,@LuongCoBan,@HeSo,@NgayApDung, @NgayKetThuc,
					tmp.ID_CaLamViec, tmp.TenCa, tmp.TongGioCong1Ca,
					tmp.SoGioOT * Luong1GioCong as LuongOT,
					tmp.NgayCham
				from #temp2 tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  								
				FETCH NEXT FROM curLuong 
				INTO @ID_NhanVien, @ID_PhuCap, @TenLoaiLuong, @LoaiLuong, @LuongCoBan, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curLuong  
    		DEALLOCATE curLuong 	

			--select thu, TenCa, SoGioOT, Luong1GioCong, LuongTheoCa, SoTien, NgayCham from #temp2 where ID_NhanVien='D559BADC-83AE-407C-8A79-BC160DF5C73A' order by NgayCham

		select ID_NhanVien, SUM(LuongOT) as LuongOT
		from
			(select ID_NhanVien, LuongOT from @tblCongOTNgay
			union all
			select ID_NhanVien, LuongOT from @tblCongOTCa
			) luongot group by luongot.ID_NhanVien
			
END");

			Sql(@"CREATE PROCEDURE [dbo].[TinhPhuCapLuong]
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN
	SET NOCOUNT ON;

		--declare @tblCongThuCong CongThuCong
		--insert into @tblCongThuCong
		--exec dbo.GetChiTietCongThuCong @IDChiNhanhs, @FromDate, @ToDate

		--declare @tblThietLapLuong ThietLapLuong
		--insert into @tblThietLapLuong
		--exec GetNS_ThietLapLuong @IDChiNhanhs, @FromDate, @ToDate

	-- get phucapcodinh vnd trong khoang thoigian
		declare @tblPhuCapCD table (ID_NhanVien uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, PhuCapCoDinh float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblPhuCapCD
		select *
		from @tblThietLapLuong pc
		where pc.LoaiLuong = 52 		

		declare @tblCong1 table ( ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, PhuCapCoDinh float, HeSo int,
		ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoNgayDiLam float, NgayApDung datetime, NgayKetThuc datetime)
		declare @cd_IDNhanVien uniqueidentifier, @cd_IDPhuCap uniqueidentifier, @cd_TenPhuCap nvarchar(max), @cd_LoaiLuong int, @cd_PhuCapCoDinh float, @cd_HeSo int, @cd_NgayApDung datetime, @cd_NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblPhuCapCD
		OPEN curPhuCap -- cur 1
    	FETCH FIRST FROM curPhuCap
    	INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong1
				select @cd_IDNhanVien, tmp.ID_ChamCongChiTiet, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_PhuCapCoDinh, @cd_HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(Cong>0,1,0)) as SoNgayDiLam,
					@cd_NgayApDung, @cd_NgayKetThuc
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @cd_IDNhanVien and tmp.NgayCham >= @cd_NgayApDung and (@cd_NgayKetThuc is null OR tmp.NgayCham <= @cd_NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @cd_IDNhanVien, @cd_IDPhuCap, @cd_TenPhuCap, @cd_LoaiLuong, @cd_PhuCapCoDinh, @cd_HeSo, @cd_NgayApDung, @cd_NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 			

		-- get phucaptheongay trong khoang thoigian
		declare @tblPhuCap table (ID_NhanVien uniqueidentifier, IDPhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong int, PhuCapTheoNgay float,  HeSo int, NgayApDung datetime, NgayKetThuc datetime)
		insert into @tblPhuCap
		select *	
		from @tblThietLapLuong pc 
		where pc.LoaiLuong = 51	
		
			-- bảng tính số ngày đi làm theo phiếu phân ca, ca làm việc
		declare @tblCong2 table (ID_NhanVien uniqueidentifier, ID_ChamCongChiTiet uniqueidentifier, ID_PhuCap uniqueidentifier, TenPhuCap nvarchar(max), LoaiLuong float, 
		PhuCapTheoNgay float, HeSo int, ID_CaLamViec uniqueidentifier, TenCa nvarchar(100),  SoNgayDiLam float,		
		NgayApDung datetime, NgayKetThuc datetime)

		-- biến để đọc gtrị cursor
		declare @ID_NhanVien uniqueidentifier, @IDPhuCap uniqueidentifier, @TenPhuCap nvarchar(max), @PhuCapTheoNgay float, @LoaiLuong int, @HeSo int, @NgayApDung datetime, @NgayKetThuc datetime

		DECLARE curPhuCap CURSOR SCROLL LOCAL FOR
    		select *
    		from @tblPhuCap
		OPEN curPhuCap 
    	FETCH FIRST FROM curPhuCap
    	INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong,@PhuCapTheoNgay, @HeSo, @NgayApDung, @NgayKetThuc
    		WHILE @@FETCH_STATUS = 0
    		BEGIN
				insert into @tblCong2
				select @ID_NhanVien, tmp.ID_ChamCongChiTiet, @IDPhuCap, @TenPhuCap, @LoaiLuong, @PhuCapTheoNgay, @HeSo, tmp.ID_CaLamViec, tmp.TenCa, 
					SUM(IIF(tmp.Cong>0,tmp.Cong,0)) as SoNgayDiLam,
					@NgayApDung, @NgayKetThuc
				from @tblCongThuCong tmp
				where tmp.ID_NhanVien = @ID_NhanVien and tmp.NgayCham >= @NgayApDung and (@NgayKetThuc is null OR tmp.NgayCham <= @NgayKetThuc )  
				group by tmp.ID_ChamCongChiTiet, tmp.ID_CaLamViec,tmp.TenCa, tmp.ID_NhanVien 				
				FETCH NEXT FROM curPhuCap INTO @ID_NhanVien, @IDPhuCap, @TenPhuCap, @LoaiLuong,@PhuCapTheoNgay, @HeSo, @NgayApDung, @NgayKetThuc
			END;
			CLOSE curPhuCap  
    		DEALLOCATE curPhuCap 	

			--select * from #temp where ID_NhanVien='C67E6F17-F506-44F7-B9CD-72675E383500' and ID_CaLamViec='FFE73265-CBEC-4C44-8CFD-B19F128089E7' order by NgayCham
			
			select 
				ID_NhanVien,
				SUM(pc.PhuCapCoDinh) as PhuCapCoDinh,
				SUM(pc.ThanhTienPC_TheoNgay) as ThanhTienPC_TheoNgay
			from
			(select ID_NhanVien,
					PhuCapCoDinh * HeSo as PhuCapCoDinh, 
					0 as PhuCapTheoNgay, 
					0 as ThanhTienPC_TheoNgay,
					0 as SoNgayDiLam,
					ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc, 
					1 as LoaiPhuCap
				from @tblCong1 			
				where SoNgayDiLam > 0
				group by ID_NhanVien, ID_PhuCap, PhuCapCoDinh, HeSo, NgayApDung, NgayKetThuc, TenPhuCap

				union all

				select ID_NhanVien, 
					0 as PhuCapCoDinh,
					PhuCapTheoNgay  * HeSo as PhuCapTheoNgay ,
					PhuCapTheoNgay * HeSo * SoNgayDiLam as ThanhTienPC_TheoNgay,
					SoNgayDiLam,
					ID_PhuCap,TenPhuCap, NgayApDung, NgayKetThuc,
					2 as LoaiPhuCap
				from @tblCong2
				) pc group by ID_NhanVien	
		
END");

			CreateStoredProcedure(name: "[dbo].[UpdateCongNo_TamUngLuong]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				IDQuyChiTiets = p.String(),
				LaPhieuTamUng = p.Boolean()
			}, body: @"SET NOCOUNT ON;

	declare @tblQuyChiTiet table(ID_QuyChiTiet uniqueidentifier)
	insert into @tblQuyChiTiet
	select Name from dbo.splitstring(@IDQuyChiTiets)

	if @LaPhieuTamUng ='1'
		begin
			declare @sotienTamUng float, @nvTamUng uniqueidentifier, @idKhoanThuChi uniqueidentifier
			-- get sotien, nhanvien tamung
			select @sotienTamUng = TienThu, @nvTamUng= ID_NhanVien, @idKhoanThuChi= ID_KhoanThuChi
			from Quy_HoaDon_ChiTiet qct1 where exists (select top 1 ID from @tblQuyChiTiet qct2 where qct1.ID= qct2.ID_QuyChiTiet)

			declare @giamtruLuong bit= (
				select TinhLuong
				from Quy_KhoanThuChi khoan
				where id= @idKhoanThuChi)

			if @giamtruLuong ='1'
			begin
				if (select count(ID) from NS_CongNoTamUngLuong  where ID_NhanVien = @nvTamUng and ID_DonVi = @ID_ChiNhanh) = 0	
					-- neu chua tontai: insert
					insert into NS_CongNoTamUngLuong
					values(NEWID(), @ID_ChiNhanh, @nvTamUng,@sotienTamUng)
				else
					-- update
					update NS_CongNoTamUngLuong set CongNo= CongNo + @sotienTamUng
			end
			
		end
	else
		begin
			update tblNoCu set CongNo= tblNoHienTai.NoHienTai
			from NS_CongNoTamUngLuong tblNoCu
			join (
				select congno.ID,congno.CongNo - quy.TruTamUngLuong as NoHienTai
				from NS_CongNoTamUngLuong congno
				join (
					select qct.ID_NhanVien, qct.TruTamUngLuong
					from Quy_HoaDon_ChiTiet qct 
					join @tblQuyChiTiet qct2 on qct.ID= qct2.ID_QuyChiTiet			
					) quy on congno.ID_NhanVien= quy.ID_NhanVien
				where congno.ID_DonVi= @ID_ChiNhanh
			) tblNoHienTai on tblNoCu.ID= tblNoHienTai.ID
		end");

			Sql(@"ALTER PROCEDURE [dbo].[GetInForStaff_Working_byChiNhanhDaTaoND]
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    SELECT nv.ID,MaNhanVien,TenNhanVien,DienThoaiDiDong, GioiTinh, nv.TenNhanVienKhongDau, TenNhanVienChuCaiDau, nd.TaiKhoan,
		LOWER(CONCAT(nv.MaNhanVien,' ', nv.TenNhanVien, ' ', nv.TenNhanVienKhongDau, ' ', nv.TenNhanVienChuCaiDau)) as NameFull
    	from HT_NguoiDung nd 
    		join NS_NhanVien nv on nv.ID = nd.ID_NhanVien
    	join NS_QuaTrinhCongTac qt on nv.ID = qt.ID_NhanVien
    	where qt.ID_DonVi like @ID_DonVi and nd.DangHoatDong = 1 and nv.DaNghiViec= 0 and (nv.TrangThai is null or nv.TrangThai= 1)
END");

			Sql(@"ALTER PROCEDURE [dbo].[getList_ChietKhauNhanVienTheoDoanhSo]
    @ID_ChiNhanhs [nvarchar](max),
	@ID_NhanVienLogin nvarchar(max),
    @TextSearch nvarchar(max),
    @timeStar [nvarchar](max),
    @timeEnd [nvarchar](max),
	@Status_DoanhThu nvarchar(4),
	@CurrentPage int,
	@PageSize int
AS
BEGIN
	set nocount on;
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);

	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @ID_ChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');

	set @timeEnd = dateadd(day,1, @timeEnd) ;

	with data_cte
	as (
		select *
		from
			(select *,
				case when HoaHongDoanhThu > 0 
					then case when HoaHongThucThu > 0 then '21' else '20' end
				else 
					case when HoaHongThucThu > 0 then '11' else '10' end end as Status_DoanhThu
			from
			(
			 Select 
    			e.ID_NhanVien, 
    			MAX(e.MaNhanVien) as MaNhanVien,
    			MAX(e.TenNhanVien) as TenNhanVien,
    			SUM(e.DoanhThu) as TongDoanhThu,
    			SUM(e.ThucThu) as TongThucThu,
    			CAST(ROUND(SUM(e.HoaHongDoanhThu),0) as float) as HoaHongDoanhThu,
    			CAST(ROUND(SUM(e.HoaHongThucThu),0) as float) as HoaHongThucThu,
    			CAST(ROUND(SUM(e.HoaHongDoanhThu) + SUM(e.HoaHongThucThu),0) as float) as TongAll
			From
			(
    				Select d.* ,
    					case when d.LaPhanTram =1 then
    						case when d.TinhChietKhauTheo=2 then DoanhThu * GiaTriChietKhau / 100 else 0 end 
    					else 
    						case when d.TinhChietKhauTheo=2 then GiaTriChietKhau else 0 end end as HoaHongDoanhThu,
    					case when d.LaPhanTram =1 then
    						case when d.TinhChietKhauTheo=1 then ThucThu * GiaTriChietKhau / 100 else 0 end 
    					else 
    						case when d.TinhChietKhauTheo=1 then GiaTriChietKhau else 0 end end as HoaHongThucThu
    				from
    				(
    					Select
    						c.*,
    						ROW_NUMBER() OVER(PARTITION BY c.MaNhanVien, c.TinhChietKhauTheo, c.ID_ChietKhauDoanhThu 
    													 ORDER BY c.DoanhThuTu DESC) AS rk
    					from
    					(
    						Select b.*, 
    						ckct.ID, ckct.DoanhThuTu, ckct.DoanhThuDen, ckct.GiaTriChietKhau, ckct.LaPhanTram FROM
    						(
    						Select
    							a.ID_NhanVien,
    							a.MaNhanVien,
    							MAX(a.TenNhanVien) as TenNhanVien,
    							a.TinhChietKhauTheo,
    							SUM(a.PhaiThanhToan - a.GiaTriTra) as DoanhThu,
    							SUM(a.TienThu - a.TienTraKhach) as ThucThu,
    							a.ID_ChietKhauDoanhThu,
    							MAX(a.ApDungTuNgay) as ApDungTuNgay,
    							MAX(a.ApDungDenNgay) as ApDungDenNgay
    						 from
    						(
    							-- DoanhThu
    							select DISTINCT nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								hd.PhaiThanhToan, 
    								0 as TienThu,
    								0 as GiaTriTra,
    								0 as TienTraKhach,
    								hd.NgayLapHoaDon,ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu
    							join ChietKhauDoanhThu_ChiTiet ckdtct on ckdt.ID = ckdtct.ID_ChietKhauDoanhThu
    							join NS_NhanVien nv on nv.ID = ckdtnv.ID_NhanVien
    							join BH_HoaDon hd on nv.ID = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							where 
    							ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    							and ckdt.TrangThai=1
    							--and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    							and hd.NgayLapHoaDon >= @timeStar  and hd.NgayLapHoaDon < @timeEnd
								and
									((select count(Name) from @tblSearchString b where     			
									nv.TenNhanVien like '%'+b.Name+'%'
									or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
									or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
									or nv.MaNhanVien like '%'+b.Name+'%'				
									)=@count or @count=0)	
    
    							Union all
    							-- ThucThu
    							select DISTINCT nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
    								case when ISNULL(qhdct.ThuTuThe, 0) > 0 or ISNULL(qhdct.DiemThanhToan, 0) > 0 then 0 else ISNULL(qhdct.TienThu, 0) end as TienThu,-- ThucThu(khong lay giatri ThuTuThe or ThanhToan = diem)
    								0 as GiaTriTra,
    								0 as TienTraKhach,
    								hd.NgayLapHoaDon,ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu
    							join ChietKhauDoanhThu_ChiTiet ckdtct on ckdt.ID = ckdtct.ID_ChietKhauDoanhThu
    							join NS_NhanVien nv on nv.ID = ckdtnv.ID_NhanVien
    							join BH_HoaDon hd on nv.ID = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hd.ID
    							left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    							and ckdt.TrangThai=1
    							--and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    							and hd.NgayLapHoaDon >= @timeStar and hd.NgayLapHoaDon < @timeEnd
    							and (qhd.TrangThai is null or qhd.TrangThai != 0)
								and
									((select count(Name) from @tblSearchString b where     			
									nv.TenNhanVien like '%'+b.Name+'%'
									or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
									or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
									or nv.MaNhanVien like '%'+b.Name+'%'				
									)=@count or @count=0)	
    							and case when ISNULL(qhdct.ThuTuThe, 0) > 0 or ISNULL(qhdct.DiemThanhToan, 0) > 0 then 0 else ISNULL(qhdct.TienThu, 0) end > 0
    
    							Union all
    							-- HDTra
    							select DISTINCT nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 0 as TienThu,
    								hdt.PhaiThanhToan as GiaTriTra,
    								ISNULL(qhdct.TienThu, 0) as TienTraKhach,
    								hd.NgayLapHoaDon,ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu
    							join ChietKhauDoanhThu_ChiTiet ckdtct on ckdt.ID = ckdtct.ID_ChietKhauDoanhThu
    							join NS_NhanVien nv on nv.ID = ckdtnv.ID_NhanVien
    							join BH_HoaDon hdt on nv.ID = hdt.ID_NhanVien and ckdt.ID_DonVi = hdt.ID_DonVi and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							Join BH_HoaDon hd on hd.ID = hdt.ID_HoaDon
    							left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    							left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    							and ckdt.TrangThai=1
    							--and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    							and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    							and hdt.NgayLapHoaDon >= @timeStar and hdt.NgayLapHoaDon < @timeEnd
								and (qhd.TrangThai is null or qhd.TrangThai != 0)
								
								and
									((select count(Name) from @tblSearchString b where     			
									nv.TenNhanVien like '%'+b.Name+'%'
									or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
									or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
									or nv.MaNhanVien like '%'+b.Name+'%'				
									)=@count or @count=0)	  					   
    							) as a
    							GROUP BY a.ID_NhanVien, a.MaNhanVien, a.TinhChietKhauTheo, a.ID_ChietKhauDoanhThu
    						) as b
    						join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu and ((b.DoanhThu >= DoanhThuTu and TinhChietKhauTheo = 2) or (b.ThucThu >= DoanhThuTu and TinhChietKhauTheo = 1))
    					) as c
    				) as d
    				where d.rk = 1
			) e
			GROUP BY e.ID_NhanVien
			) tbl
		) tblView where tblView.Status_DoanhThu like '%'+ @Status_DoanhThu +'%'
		and (exists (select ID from @tblNhanVien nv where tblView.ID_NhanVien = nv.ID))
	),
	count_cte
	as (
		select count(ID_NhanVien) as TotalRow,
			CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float ))  as TotalPage,
			sum(TongDoanhThu) as TongAllDoanhThu,
			sum(TongThucThu) as TongAllThucThu,
			sum(HoaHongDoanhThu) as TongHoaHongDoanhThu,
			sum(HoaHongThucThu) as TongHoaHongThucThu,
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

			Sql(@"ALTER PROCEDURE [dbo].[getList_ChietKhauNhanVienTheoDoanhSobyID]
    @ID_ChiNhanhs [nvarchar](max),
    @ID_NhanVien [nvarchar](max),
    @timeStar [datetime],
    @timeEnd [datetime]
AS
BEGIN
		Select 
    		Case when d.TinhChietKhauTheo = 1 then N'Theo thực thu' else N'Theo doanh thu' end as HinhThuc,
    		d.ApDungTuNgay, d.ApDungDenNgay,
    		DoanhThu,
    		ThucThu,
    		GiaTriChietKhau,
    		LaPhanTram,
    		case when d.LaPhanTram =0 then GiaTriChietKhau				
    		else 
    			case when d.TinhChietKhauTheo=2 then DoanhThu * GiaTriChietKhau / 100 else ThucThu * GiaTriChietKhau / 100 end  end as HoaHong
    	from
    	(
    		Select
    		c.*,
    		ROW_NUMBER() OVER(PARTITION BY c.MaNhanVien, c.TinhChietKhauTheo, c.ID_ChietKhauDoanhThu 
    										 ORDER BY c.DoanhThuTu DESC) AS rk
    		from
    		(
    			Select b.*, 
    			ckct.ID, ckct.DoanhThuTu, ckct.DoanhThuDen, ckct.GiaTriChietKhau, ckct.LaPhanTram FROM
    			(
    				Select
    					a.ID_NhanVien,
    					a.MaNhanVien,
    					MAX(a.TenNhanVien) as TenNhanVien,
    					a.TinhChietKhauTheo,
    					SUM(a.PhaiThanhToan - a.GiaTriTra) as DoanhThu,
    					SUM(a.TienThu - a.TienTraKhach) as ThucThu,
    					a.ID_ChietKhauDoanhThu,
    					MAX(a.ApDungTuNgay) as ApDungTuNgay,
    					MAX(a.ApDungDenNgay) as ApDungDenNgay
    				 from
    				(
    					-- DoanhThu HD
    					select DISTINCT nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien, 
    					ckdt.TinhChietKhauTheo,
    					hd.PhaiThanhToan, 
    					0 as TienThu,					
    					0 as GiaTriTra,
    					0 as TienTraKhach,
    					hd.NgayLapHoaDon,ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    					from ChietKhauDoanhThu ckdt
    					join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu
    					join ChietKhauDoanhThu_ChiTiet ckdtct on ckdt.ID = ckdtct.ID_ChietKhauDoanhThu
    					join NS_NhanVien nv on nv.ID = ckdtnv.ID_NhanVien
    					join BH_HoaDon hd on nv.ID = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    					where nv.ID = @ID_NhanVien
    					and ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and ckdt.TrangThai=1
    					--and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    					and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    					and hd.NgayLapHoaDon >= @timeStar and hd.NgayLapHoaDon < @timeEnd
    					Union all
    
    					-- ThucThu HD
    					select DISTINCT nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien, 
    					ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    					0 as PhaiThanhToan, 
    					case when ISNULL(qhdct.ThuTuThe, 0) > 0 or ISNULL(qhdct.DiemThanhToan, 0) > 0 then 0 else ISNULL(qhdct.TienThu, 0) end as TienThu,-- ThucThu(khong lay giatri ThuTuThe or ThanhToan = diem)
    					0 as GiaTriTra,
    					0 as TienTraKhach,
    					hd.NgayLapHoaDon,ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    					from ChietKhauDoanhThu ckdt
    					join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu
    					join ChietKhauDoanhThu_ChiTiet ckdtct on ckdt.ID = ckdtct.ID_ChietKhauDoanhThu
    					join NS_NhanVien nv on nv.ID = ckdtnv.ID_NhanVien
    					join BH_HoaDon hd on nv.ID = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    					left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hd.ID
    					left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    					where nv.ID = @ID_NhanVien
    					and ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and ckdt.TrangThai=1
    					--and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    					and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    					and hd.NgayLapHoaDon >= @timeStar and hd.NgayLapHoaDon < @timeEnd
    					and (qhd.TrangThai is null or qhd.TrangThai != 0)
    					and case when ISNULL(qhdct.ThuTuThe, 0) > 0 or ISNULL(qhdct.DiemThanhToan, 0) > 0 then 0 else ISNULL(qhdct.TienThu, 0) end > 0
    					Union all
    					-- TongTra
    					select DISTINCT nv.ID as ID_NhanVien, nv.MaNhanVien, nv.TenNhanVien, 
    						ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    						0 as PhaiThanhToan, 0 as TienThu,
    						hdt.PhaiThanhToan as GiaTriTra,
    						ISNULL(qhdct.TienThu, 0) as TienTraKhach,
    						hd.NgayLapHoaDon,ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    					from ChietKhauDoanhThu ckdt
    					join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu
    					join ChietKhauDoanhThu_ChiTiet ckdtct on ckdt.ID = ckdtct.ID_ChietKhauDoanhThu
    					join NS_NhanVien nv on nv.ID = ckdtnv.ID_NhanVien
    					join BH_HoaDon hdt on nv.ID = hdt.ID_NhanVien and ckdt.ID_DonVi = hdt.ID_DonVi and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    					Join BH_HoaDon hd on hd.ID = hdt.ID_HoaDon
    					left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    					left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    					where nv.ID = @ID_NhanVien
    					and ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and ckdt.TrangThai=1
    					--and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    					and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    					and hdt.NgayLapHoaDon >= @timeStar and hdt.NgayLapHoaDon < @timeEnd
    					and (qhd.TrangThai is null or qhd.TrangThai != 0)
    					) as a
    				GROUP BY a.ID_NhanVien, a.MaNhanVien, a.TinhChietKhauTheo, a.ID_ChietKhauDoanhThu
    				) as b
    			join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
    			and ((b.DoanhThu >= DoanhThuTu and TinhChietKhauTheo = 2) or (b.ThucThu >= DoanhThuTu and TinhChietKhauTheo = 1))
    		) as c
    	) as d
    	where d.rk = 1
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
			Select * from
			(
    			SELECT ckmd.ID, ckmd.ID_DonViQuiDoi AS IDQuyDoi,
					ckmd.NgayNhap,
					Case when dvqd.Xoa is null then 0 else dvqd.Xoa end as Xoa,
    				Case when nhh.ID is null then N'Nhóm mặc định' else nhh.TenNhomHangHoa end as TenNhomHangHoa,
    				Case when nhh.ID is null then N'nhom mac dinh' else nhh.TenNhomHangHoa_KhongDau end as TenNhomHangHoa_KhongDau,
    				Case when nhh.ID is null then N'nmd' else nhh.TenNhomHangHoa_KyTuDau end as TenNhomHangHoa_KyTuDau,
    				Case When nhh.ID is null then '00000000-0000-0000-0000-000000000000' else nhh.ID end as ID_NhomHang,
    				dvqd.MaHangHoa,
    				hh.TenHangHoa +
    				Case when (dvqd.ThuocTinhGiaTri is null) then '' else '_' + dvqd.ThuocTinhGiaTri end +
    				Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    				hh.TenHangHoa,
    				hh.TenHangHoa_KhongDau,
    				hh.TenHangHoa_KyTuDau,
					Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    				Case when (dvqd.ThuocTinhGiaTri is null) then '' else '_' + dvqd.ThuocTinhGiaTri end as ThuocTinh_GiaTri,
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

			Sql(@"ALTER PROC [dbo].[getlist_SuKienToDay]
@ID_DonVi uniqueidentifier
AS
BEGIN
	set nocount on;
	DECLARE @SoNgay int
	DECLARE @DateNow datetime= GETDATE()
	DECLARE @tblCalendar table (ID uniqueidentifier, ID_DonVi uniqueidentifier, ID_NhanVien uniqueidentifier, NgayGio datetime, PhanLoai int)
	insert into @tblCalendar exec GetListLichHen_FullCalendar_Dashboard @ID_DonVi,'%%' ;

Select @SoNgay = ThoiGianNhacHanSuDungLo from HT_CauHinhPhanMem where ID_DonVi = @ID_DonVi;
		Select 
			CAST(ROUND(SUM(a.SinhNhat),0) as float) as SinhNhat,
			CAST(ROUND(SUM(a.CongViec),0) as float) as CongViec,
			CAST(ROUND(SUM(a.LichHen),0) as float) as LichHen,
			CAST(ROUND(SUM(a.SoLoSapHetHan),0) as float) as SoLoSapHetHan,
			CAST(ROUND(SUM(a.SoLoHetHan),0) as float) as SoLoHetHan
		FROM
		(
		select Count(*) as SinhNhat, 0 as CongViec, 0 as LichHen, 0 as SoLoHetHan, 0 as SoLoSapHetHan from DM_DoiTuong 
		where TheoDoi != 1 and NgaySinh_NgayTLap is not null
		and DAY(NgaySinh_NgayTLap) = DAY(@DateNow)
		and MONTH(NgaySinh_NgayTLap)= MONTH(@DateNow)
		Union All
		-- cong viec (4)
		Select 0 as SinhNhat, COUNT(*) as CongViec, 0 as LichHen, 0 as SoLoHetHan ,0 as SoLoSapHetHan
		from @tblCalendar where PhanLoai = 4

		Union ALL
		
		Select 0 as SinhNhat, 0 as CongViec, COUNT(*) as LichHen, 0 as SoLoHetHan ,0 as SoLoSapHetHan
		from @tblCalendar where PhanLoai = 3

		Union ALL
		Select 
		0 as SinhNhat, 0 as CongViec, 0 as LichHen,0 as SoLoHetHan ,
		COUNT(*) as SoLoSapHetHan from (
			SELECT DATEDIFF(day,DATEADD(day,-1,@DateNow), lh.NgayHetHan) as SoNgayConHan FROM DM_LoHang lh
			JOIN (Select ID_LoHang, SUM(TonKho) as TonKho from DM_HangHoa_TonKho where ID_DonVi = @ID_DonVi GROUP BY ID_LoHang) tk on lh.ID = tk.ID_LoHang
			where lh.NgayHetHan is not null 
			and tk.TonKho > 0
			) as a
			where  a.SoNgayConHan < =@SoNgay and a.SoNgayConHan > 0

		Union ALL
		Select 
		0 as SinhNhat, 0 as CongViec, 0 as LichHen,
		COUNT(*) as SoLoHetHan, 0 as SoLoSapHetHan from (
			SELECT DATEDIFF(day,DATEADD(day,-1,@DateNow), lh.NgayHetHan) as SoNgayConHan FROM DM_LoHang lh
			JOIN (Select ID_LoHang, SUM(TonKho) as TonKho from DM_HangHoa_TonKho where ID_DonVi = @ID_DonVi GROUP BY ID_LoHang) tk on lh.ID = tk.ID_LoHang
			where lh.NgayHetHan is not null 
			and tk.TonKho > 0
			) as a
			where a.SoNgayConHan < 1
		) a
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetListNhanVienAddSoQuy]
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    select nv.ID, nv.TenNhanVien as NguoiNopTien, nv.MaNhanVien as MaNguoiNop, DienThoaiDiDong as SoDienThoai
    	from NS_NhanVien nv
    	left join NS_QuaTrinhCongTac qtct on nv.ID = qtct.ID_NhanVien
    	where qtct.ID_DonVi = @ID_ChiNhanh
		and (nv.DaNghiViec= 0)
		and (TrangThai= 1 OR TrangThai is null)
END");

			Sql(@"ALTER PROCEDURE [dbo].[importNS_NhanVien_DanhSach]
    @MaNhanVien [nvarchar](max),
    @TenNhanVien [nvarchar](max),
    @TenNhanVienKhongDau [nvarchar](max),
    @TenNhanVienKyTuDau [nvarchar](max),
    @GioiTinh [bit],
    @NgaySinh [nvarchar](max),
    @DienThoai [nvarchar](max),
    @Email [nvarchar](max),
    @NoiSinh [nvarchar](max),
    @CMND [nvarchar](max),
    @SoBaoHiem [nvarchar](max),
    @GhiChu [nvarchar](max),
    @TrangThai [bit],
	@ID_DonVi [uniqueidentifier]
AS
BEGIN
	DECLARE @ID_PhongBan uniqueidentifier
    	Set @ID_PhongBan = (select TOP 1 ID from NS_PhongBan where ID_DonVi is NULL)
	DECLARE @ID_NhanVien uniqueidentifier
		Set @ID_NhanVien = NEWID();
    insert into NS_NhanVien(ID, MaNhanVien, TenNhanVien, TenNhanVienKhongDau,TenNhanVienChuCaiDau, GioiTinh, NgaySinh, DienThoaiDiDong, Email, NoiSinh,NguyenQuan, SoCMND,SoBHXH, GhiChu, NguoiTao,NgayTao, DaNghiViec)
    values(@ID_NhanVien, @MaNhanVien, @TenNhanVien, @TenNhanVienKhongDau,@TenNhanVienKyTuDau, @GioiTinh, @NgaySinh, @DienThoai, @Email, @NoiSinh, @NoiSinh, @CMND,@SoBaoHiem, @GhiChu, 'admin',GETDATE(), @TrangThai);

	--- insert NS_QuaTrinhCongTac with current ChiNhanh + phongban macdinh
	insert into NS_QuaTrinhCongTac (ID, ID_NhanVien, ID_DonVi, NgayApDung, LaChucVuHienThoi, LaDonViHienThoi, ID_PhongBan)
			values (NEWID(), @ID_NhanVien, @ID_DonVi, GETDATE(), '0', '1', @ID_PhongBan)

END");

			Sql(@"ALTER PROCEDURE [dbo].[UpdateChamCongKhiThayDoiHeSo]
	@ID_DonVi uniqueidentifier,
	@ID_KyHieuCong uniqueidentifier,
    @KyHieuCongOld [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;
	declare @KyHieuCongNew nvarchar(10) = (select top 1 KyHieu from NS_KyHieuCong where ID= @ID_KyHieuCong)
	declare @CongQuyDoiNew float = (select top 1 CongQuyDoi from NS_KyHieuCong where ID= @ID_KyHieuCong)

		update bs set bs.Cong = @CongQuyDoiNew, 
					bs.CongQuyDoi = bs.CongQuyDoi/IIF(bs.Cong=0,1,bs.Cong) * @CongQuyDoiNew,  -- congsetup (ol) * cong new
					bs.KyHieuCong= @KyHieuCongNew
		from NS_CongBoSung bs
		where bs.ID_DonVi= @ID_DonVi
		and bs.TrangThai in (1,2)
		and bs.KyHieuCong like @KyHieuCongOld	

END");

			CreateStoredProcedure(name: "[dbo].[GetListPromotion]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSearch = p.String(),
				TypePromotion = p.String(4),
				StatusActive = p.String(4),
				Expired = p.String(4),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	declare @today datetime = format(getdate(),'yyyy-MM-dd HH:mm')
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	with data_cte
		as(
	select *
	from
		(SELECT 
			km.ID,
			ad.ID_DonVi,
			km.MaKhuyenMai,
			km.TenKhuyenMai,
			km.GhiChu,
			Case when km.TrangThai = 1 then N'Kích hoạt' else N'Chưa áp dụng' end as TrangThai,
			case km.HinhThuc
				when 11 then N'Hóa đơn - Giảm giá hóa đơn'
				when 12 then N'Hóa đơn - Tặng hàng'
				when 13 then N'Hóa đơn - Giảm giá hàng'
				when 14 then N'Hóa đơn - Tặng Điểm'
				when 21 then N'Hàng hóa - Mua hàng giảm giá hàng'
				when 22 then N'Hàng hóa - Mua hàng tặng hàng'
				when 23 then N'Hàng hóa - Mua hàng tặng điểm'
				when 24 then N'Hàng hóa - Mua hàng giảm giá theo số lượng mua'
			end as HinhThuc,
			km.LoaiKhuyenMai,
			km.HinhThuc as KieuHinhThuc,
			km.ThoiGianBatDau,
			km.ThoiGianKetThuc,
			Case when km.NgayApDung = '' then '' else N'Ngày ' + Replace(km.NgayApDung, '_', N', Ngày ') end as NgayApDung,
			Case when km.ThangApDung = '' then '' else N'Tháng ' + Replace(km.ThangApDung, '_', N', Tháng ') end as ThangApDung,
			Replace(Case when km.ThuApDung = '' then '' else N'Thứ ' + Replace(km.ThuApDung, '_', N', Thứ ') end, N'Thứ 8',N'Chủ nhật') as ThuApDung,
			Case when km.GioApDung = '' then '' else Replace(km.GioApDung, '_', N', ') end as GioApDung,
			Case when km.ApDungNgaySinhNhat = 1 then N'Áp dụng vào ngày sinh nhật khách hàng' when km.ApDungNgaySinhNhat = 2 then N'Áp dụng vào tuần sinh nhật khách hàng'
			when km.ApDungNgaySinhNhat = 3 then N'Áp dụng vào tháng sinh nhật khách hàng' else '' end as ApDungNgaySinhNhat,
			km.ApDungNgaySinhNhat as ValueApDungSN,
			km.TatCaDoiTuong,
			km.TatCaDonVi,
			km.TatCaNhanVien,
			km.NguoiTao,
			km.NgayTao,
			case when format(ThoiGianBatDau,'yyyy-MM-dd HH:mm') > @today OR format(ThoiGianKetThuc,'yyyy-MM-dd HH:mm') < @today then '1' else '2' end as Expired					
		FROM DM_KhuyenMai km
		left join DM_KhuyenMai_ApDung ad on km.ID = ad.ID_KHuyenMai
		where km.TrangThai like @StatusActive
		and LoaiKhuyenMai like @TypePromotion
		and (km.TatCaDonVi = '1' or exists(select Name from dbo.splitstring(@IDChiNhanhs) where ad.ID_DonVi = Name))
		AND ((select count(Name) from @tblSearchString b where 
    		km.MaKhuyenMai like '%'+b.Name+'%' 
    		or km.TenKhuyenMai like '%'+b.Name+'%' 
    		)=@count or @count=0)
	) a
	where Expired like @Expired
	),
	count_cte
		as (
			select count(*) as TotalRow,
				CEILING(COUNT(*) / CAST(@PageSize as float ))  as TotalPage				
			from data_cte
		)
		select dt.*, cte.*
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayTao desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			Sql(@"--- update ID_NganHang in Quy_ChiTiet
update qct set qct.ID_NganHang = tk.ID_NganHang
from Quy_HoaDon_ChiTiet qct
join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang = tk.ID
where qct.ID_NganHang is null;


----- reset & update TongTichDiem
update BH_HoaDon set DiemGiaoDich= 0 where LoaiHoaDon= 22;
update dt set TongTichDiem = DiemGiaoDich + DiemThanhToan
from DM_DoiTuong dt
join (
select ID, MaDoiTuong, TenDoiTuong, TongTichDiem, DiemGiaoDich, isnull(DiemThanhToan,0) as DiemThanhToan
from DM_DoiTuong dt
join (
	-- diemgiaodich tu hoadon
	select ID_DoiTuong, sum(DiemGiaDich) as DiemGiaoDich
	from(
	select hd.ID_DoiTuong, 
		IIF(hd.LoaiHoaDon=6, - hd.DiemGiaoDich, hd.DiemGiaoDich) as DiemGiaDich
	from BH_HoaDon hd
	where hd.ChoThanhToan= 0
	and hd.LoaiHoaDon in (1,19,6)
	and DiemGiaoDich > 0
	) diemHD group by diemHD.ID_DoiTuong
) a on dt.ID= a.ID_DoiTuong
left join (
	select quy3.ID_DoiTuong, sum(DiemThanhToan) as DiemThanhToan
	from
	(
		-- diemdieuchinh
		select quy1.ID_DoiTuong, sum(DiemThanhToan) as DiemThanhToan
		from
			(select qct.ID_DoiTuong, iif(qhd.LoaiHoaDon= 11, DiemThanhToan, -DiemThanhToan) as DiemThanhToan
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.id
			where qct.DiemThanhToan > 0  and qct.TienThu = 0
			and qhd.TrangThai= 1
			) quy1 group by quy1.ID_DoiTuong

		union all
		-- thanhtoan = diem
		select quy2.ID_DoiTuong, sum(DiemThanhToan) as DiemThanhToan
		from
			(
			select qct.ID_DoiTuong, iif(qhd.LoaiHoaDon= 11, -DiemThanhToan, DiemThanhToan) as DiemThanhToan
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.id
			where qct.DiemThanhToan > 0  and qct.TienThu > 0
			and qhd.TrangThai= 1
		)quy2 group by quy2.ID_DoiTuong
	) quy3 group by quy3.ID_DoiTuong
	) quyout on dt.ID= quyout.ID_DoiTuong	
where dt.TongTichDiem != DiemGiaoDich + isnull(DiemThanhToan,0)
) dtupdate on dt.ID= dtupdate.ID;");

		}
        
        public override void Down()
        {
			Sql(@"DROP FUNCTION [dbo].[Diary_BangLuong]");
			Sql(@"DROP FUNCTION [dbo].[Diary_LuongPhuCap]");
			Sql(@"DROP FUNCTION [dbo].[GetIDNhanVien_inPhongBan]");
			Sql(@"DROP FUNCTION [dbo].[GetMaBangLuongMax_byTemp]");
			Sql(@"DROP FUNCTION [dbo].[TinhNgayCongChuan]");
			DropStoredProcedure("[dbo].[ChangeCong_UpdateNSCongBoSung]");
			DropStoredProcedure("[dbo].[CheckSameTime_CaLamViec]");
			DropStoredProcedure("[dbo].[ChiTietTraHang_insertChietKhauNV]");
			DropStoredProcedure("[dbo].[ExportBangCongNhanVien]");
			DropStoredProcedure("[dbo].[GetAllBangLuong]");
			DropStoredProcedure("[dbo].[GetBangCongChiTiet]");
			DropStoredProcedure("[dbo].[GetBangCongNhanVien]");
			DropStoredProcedure("[dbo].[GetBangLuongChiTiet_ofNhanVien]");
			DropStoredProcedure("[dbo].[GetChiTietCongThuCong]");
			DropStoredProcedure("[dbo].[GetCongQuyDoi]");
			DropStoredProcedure("[dbo].[GetDanhSachMayChamCongTheoChiNhanh]");
			DropStoredProcedure("[dbo].[GetDuLieuChamCong]");
			DropStoredProcedure("[dbo].[GetDuLieuCongThoTheoThang]");
			DropStoredProcedure("[dbo].[GetGiamTruCoDinh_TheoPtram]");
			DropStoredProcedure("[dbo].[GetGiamTruLuongChiTiet]");
			DropStoredProcedure("[dbo].[GetListCaLamViec_ofDonVi]");
			DropStoredProcedure("[dbo].[GetListDebitSalaryDetail]");
			DropStoredProcedure("[dbo].[GetListTheGiaTri]");
			DropStoredProcedure("[dbo].[GetLuongChinh_ofNhanVien]");
			DropStoredProcedure("[dbo].[GetLuongChinhTheoCaGio]");
			DropStoredProcedure("[dbo].[GetLuongCoDinh_OrLuongNgayCong]");
			DropStoredProcedure("[dbo].[GetLuongOT_ofNhanVien]");
			DropStoredProcedure("[dbo].[GetNhanVien_coBangLuong]");
			DropStoredProcedure("[dbo].[GetPhuCapCoDinh_TheoPtram]");
			DropStoredProcedure("[dbo].[GetPhuCapLuongChiTiet]");
			DropStoredProcedure("[dbo].[GetQuyDoi_ofCongBoSung]");
			DropStoredProcedure("[dbo].[KhoiTaoDuLieuChamCong]");
			DropStoredProcedure("[dbo].[SaoChepThietLapLuong]");
			DropStoredProcedure("[dbo].[TinhGiamTruLuong]");
			DropStoredProcedure("[dbo].[UpdateStatusCongBoSung_WhenCreatBangLuong]");
			DropStoredProcedure("[dbo].[TinhLaiBangLuong]");
			DropStoredProcedure("[dbo].[TinhLuongCoBan]");
			DropStoredProcedure("[dbo].[TinhLuongOT]");
			DropStoredProcedure("[dbo].[TinhPhuCapLuong]");
			DropStoredProcedure("[dbo].[UpdateCongNo_TamUngLuong]");
			DropStoredProcedure("[dbo].[GetListPromotion]");
			
		}
    }
}
