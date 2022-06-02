namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20210520 : DbMigration
    {
        public override void Up()
        {
            Sql("DROP TRIGGER [dbo].[trg_DMDoiTuong]");
            Sql("DROP TRIGGER [dbo].[trg_UpdateNhomDoiTuongs]");
            Sql(@"CREATE TRIGGER [dbo].[trg_DeleteNhomDoiTuongs] on [dbo].[DM_DoiTuong_Nhom]
after delete
AS 
BEGIN

						SET NOCOUNT ON;
						update cus
							set cus.TenNhomDoiTuongs = isnull(tblUp.TenNhoms, N'Nhóm mặc định'),
							 cus.IDNhomDoiTuongs = isnull( tblUp.IDNhoms,'00000000-0000-0000-0000-000000000000')
							from DM_DoiTuong cus
							join  (
							select  dt.ID, (
    										Select ndt.TenNhomDoiTuong + ', ' AS [text()]
    												From dbo.DM_DoiTuong_Nhom dtn
    												inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    												Where dtn.ID_DoiTuong = dt.ID 
    												order by ndt.TenNhomDoiTuong
    												For XML PATH ('')
    										) TenNhoms,											
										(
    									Select convert(nvarchar(max), ndt.ID)  + ', ' AS [text()]
    										From dbo.DM_DoiTuong_Nhom dtn
    										inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    										Where dtn.ID_DoiTuong = dt.ID 
    										order by ndt.TenNhomDoiTuong
    										For XML PATH ('')
    									) IDNhoms
    									from  dbo.DM_DoiTuong dt
										INNER JOIN Deleted i  ON  i.ID_DoiTuong = dt.ID																				
							) tblUp on cus.ID= tblUp.ID

END");

            Sql(@"ALTER trigger [dbo].[trg_InsertNhomDoiTuongs] on [dbo].[DM_DoiTuong_Nhom]
for insert,update
as 
BEGIN
	set nocount on
	--declare @IDDoiTuong UNIQUEIDENTIFIER = (select top 1 ID_DoiTuong from inserted)
	--exec UpdateNhomDoiTuongs_ByID @IDDoiTuong


							update cus
							set cus.TenNhomDoiTuongs = isnull(tblUp.TenNhoms, N'Nhóm mặc định'),
								cus.IDNhomDoiTuongs = isnull(tblUp.IDNhoms,'00000000-0000-0000-0000-000000000000')
							from DM_DoiTuong cus
							join  (
							select  dt.ID, (
    										Select ndt.TenNhomDoiTuong + ', ' AS [text()]
    												From dbo.DM_DoiTuong_Nhom dtn
    												inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    												Where dtn.ID_DoiTuong = dt.ID 
    												order by ndt.TenNhomDoiTuong
    												For XML PATH ('')
    										) TenNhoms,
											(Select convert(nvarchar(max), ndt.ID)  + ', ' AS [text()]
    										From dbo.DM_DoiTuong_Nhom dtn
    										inner join DM_NhomDoiTuong ndt on dtn.ID_NhomDoiTuong = ndt.ID
    										Where dtn.ID_DoiTuong = dt.ID 
    										order by ndt.TenNhomDoiTuong
    										For XML PATH ('')
    										) IDNhoms
    									from  dbo.DM_DoiTuong dt
										INNER JOIN inserted i  ON  i.ID_DoiTuong = dt.ID																				
							) tblUp on cus.ID= tblUp.ID

END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_XuatChuyenHang]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	 SET NOCOUNT ON;
	 DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	    		
    		where nd.ID = @ID_NguoiDung);

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
					sum(ThanhTien) as ThanhTien
				from(
				select ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi,
					sum(ct.TienChietKhau) as SoLuong,
					sum(ct.TienChietKhau * ct.GiaVon) as ThanhTien
				from BH_HoaDon_ChiTiet ct
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
				where hd.ChoThanhToan=0
				and hd.LoaiHoaDon= 10 and (hd.YeuCau='1' or hd.YeuCau='4') --- YeuCau: 1.DangChuyen, 4.DaNhan, 2.PhieuTam, 3.Huy
				and hd.NgayLapHoaDon >=@timeStart and hd.NgayLapHoaDon <@timeEnd
				and exists (select ID from @tblIdDonVi dv where hd.ID_DonVi= dv.ID)
				group by ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi
				)tblHD
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

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_XuatChuyenHangChiTiet]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	 SET NOCOUNT ON;
	 DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	    		
    		where nd.ID = @ID_NguoiDung);


				select 
					CNChuyen.TenDonVi as ChiNhanhChuyen,
					CNnhan.TenDonVi as ChiNhanhNhan,
					tblHD.NgayLapHoaDon,
					tblHD.MaHoaDon,
					isnull(nhom.TenNhomHangHoa, N'Nhóm Hàng Hóa Mặc Định') as TenNhomHang,
					isnull(lo.MaLoHang,'') as TenLoHang,
					qd.MaHangHoa, qd.TenDonViTinh, 
					isnull(qd.ThuocTinhGiaTri,'') as ThuocTinh_GiaTri,
					hh.TenHangHoa,
					CONCAT(hh.TenHangHoa,qd.ThuocTinhGiaTri) as TenHangHoaFull,				
					round(tblHD.SoLuong, 3) as SoLuong,
					iif(@XemGiaVon='1',round(tblHD.DonGia,3),0) as DonGia,
					iif(@XemGiaVon='1',round(tblHD.GiaVon,3),0) as GiaVon,
					iif(@XemGiaVon='1',round(tblHD.ThanhTien,3),0) as ThanhTien,
					iif(@XemGiaVon='1',round(tblHD.GiaTri,3),0) as GiaTri			
				from(
					select 
						qd.ID_HangHoa,tblHD.ID_LoHang, 
						tblHD.ID_DonVi,tblHD.ID_CheckIn, tblHD.NgayLapHoaDon,tblHD.MaHoaDon,
						sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
						max(tblHD.GiaVon / iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as GiaVon,
						max(tblHD.DonGia / iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as DonGia,
						sum(tblHD.ThanhTien) as ThanhTien,
						sum(tblHD.GiaTri) as GiaTri
					from(
					select ct.ID_DonViQuiDoi, ct.ID_LoHang, 
						hd.ID_DonVi, hd.ID_CheckIn, hd.NgayLapHoaDon,hd.MaHoaDon,
						sum(ct.TienChietKhau) as SoLuong,
						max(ct.GiaVon) as GiaVon,
						max(ct.DonGia) as DonGia,
						sum(ct.DonGia * ct.SoLuong) as ThanhTien,
						sum(ct.TienChietKhau * ct.GiaVon) as GiaTri
					from BH_HoaDon_ChiTiet ct
					join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
					where hd.ChoThanhToan=0
					and hd.LoaiHoaDon= 10 and (hd.YeuCau='1' or hd.YeuCau='4') --- YeuCau: 1.DangChuyen, 4.DaNhan, 2.PhieuTam, 3.Huy
					and hd.NgayLapHoaDon >=@timeStart and hd.NgayLapHoaDon <@timeEnd
					and exists (select ID from @tblIdDonVi dv where hd.ID_DonVi= dv.ID)
					group by ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_DonVi, hd.ID_CheckIn,hd.NgayLapHoaDon, hd.MaHoaDon
					)tblHD
					join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID
					group by qd.ID_HangHoa, tblHD.ID_DonViQuiDoi,tblHD.ID_LoHang, tblHD.ID_DonVi,tblHD.ID_CheckIn,tblHD.NgayLapHoaDon,tblHD.MaHoaDon
				)tblHD
				join DM_DonVi CNChuyen on tblHD.ID_DonVi = CNChuyen.ID
				left join DM_DonVi CNnhan on tblHD.ID_CheckIn= CNnhan.ID
				join DM_HangHoa hh on tblHD.ID_HangHoa= hh.ID
				join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
				left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
				left join DM_LoHang lo on tblHD.ID_LoHang= lo.ID and (lo.ID= tblHD.ID_LoHang or (tblHD.ID_LoHang is null and lo.ID is null))
				where hh.LaHangHoa = 1 ---18510 352
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
				or CNnhan.TenDonVi like '%'+b.Name+'%'
				or CNChuyen.TenDonVi like '%'+b.Name+'%')=@count or @count=0)
				order by CNChuyen.TenDonVi, tblHD.NgayLapHoaDon desc, hh.TenHangHoa, lo.MaLoHang 
END");


            Sql(@"ALTER PROCEDURE [dbo].[delete_NhomDoiTuong]
    @ID_NhomDoiTuong [uniqueidentifier]
AS
BEGIN
	set nocount on

		Update DM_NhomDoiTuong set TrangThai = 0 where ID = @ID_NhomDoiTuong
    	Delete from DM_NhomDoiTuong_ChiTiet where ID_NhomDoiTuong = @ID_NhomDoiTuong    	
    	Delete from NhomDoiTuong_DonVi where ID_NhomDoiTuong = @ID_NhomDoiTuong
		Delete from DM_DoiTuong_Nhom where ID_NhomDoiTuong = @ID_NhomDoiTuong --- auto run trg_DeleteNhomDoiTuongs (update IDNhoms, TenNhoms in DM_DoiTuong)

END");

            Sql(@"ALTER PROCEDURE [dbo].[insert_HoaDon]
    @ID [uniqueidentifier],
    @ID_DoiTuong [uniqueidentifier],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHoaDon [nvarchar](max),
    @LoaiHoaDon [int],
    @TongTienHang [float],
    @timeCreate [datetime]
AS
BEGIN
    insert into BH_HoaDon (ID, MaHoaDon, NgayLapHoaDon,ID_DoiTuong,ID_NhanVien, LoaiHoaDon, ChoThanhToan, TongTienHang, 
    	TongChietKhau, TongTienThue, TongGiamGia,KhuyeMai_GiamGia, TongChiPhi, PhaiThanhToan, DienGiai, NguoiTao, NgayTao, ID_DonVi, TyGia, TongThanhToan)
    Values (@ID, @MaHoaDon, @timeCreate, @ID_DoiTuong, @ID_NhanVien, @LoaiHoaDon, '0', @TongTienHang, '0', '0', '0', '0', '0', @TongTienHang, '', 'admin', @timeCreate, @ID_DonVi, '1', @TongTienHang)
END");

            Sql(@"ALTER PROCEDURE [dbo].[insert_Quy_HoaDon]
    @ID [uniqueidentifier],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHoaDon [nvarchar](max),
    @LoaiHoaDon [int],
    @TongTienThu [float],
    @tiemCreate [datetime]
AS
BEGIN
	SET NOCOUNT ON;
    insert into Quy_HoaDon (ID, MaHoaDon, NgayLapHoaDon, NgayTao, ID_NhanVien, NguoiNopTien, NoiDungThu, TongTienThu, ThuCuaNhieuDoiTuong, Nguoitao, ID_DonVi, LoaiHoaDon, HachToanKinhDoanh, TrangThai)
    Values (@ID, @MaHoaDon, @tiemCreate, @tiemCreate, @ID_NhanVien, '', '',  @TongTienThu, '0', 'admin', @ID_DonVi, @LoaiHoaDon, '1','1')
END");

            Sql(@"ALTER PROCEDURE [dbo].[insert_Quy_HoaDon_ChiTiet]
    @ID_HoaDon [uniqueidentifier],
    @ID_HoaDonLienQuan [uniqueidentifier],
    @ID_DoiTuong [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @TienThu [float]
AS
BEGIN
    insert into Quy_HoaDon_ChiTiet (ID, ID_HoaDon, ID_NhanVien, ID_DoiTuong, ThuTuThe, TienMat, TienGui, TienThu, ID_HoaDonLienQuan, LoaiThanhToan, HinhThucThanhToan)
    Values (NEWID(), @ID_HoaDon,@ID_NhanVien,@ID_DoiTuong, '0', @TienThu, '0', @TienThu, @ID_HoaDonLienQuan,0, 1)
END");

            Sql(@"ALTER PROCEDURE [dbo].[UpdateAgainNhomDT_InDMDoiTuong_AfterChangeDKNangNhom]
    @ID_DoiTuongs [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](36)
AS
BEGIN

    delete from DM_DoiTuong_Nhom where ID_NhomDoiTuong = @ID_NhomDoiTuong 
    	and not exists (select Name from dbo.splitstring(@ID_DoiTuongs) a where a.Name= ID_DoiTuong)

		-- update idnhom, tenhom of KH not exist in list add
		update dt set dt.IDNhomDoiTuongs= dt2.IDNhoms, dt.TenNhomDoiTuongs= dt2.TenNhoms
		from DM_DoiTuong dt
		join (
				select 
					   dt.ID, dt.MaDoiTuong,
						ISNULL((select 
									cast(nhom.ID as varchar(40)) + ', ' AS [text()]
								from DM_DoiTuong_Nhom nhomdt
								join DM_NhomDoiTuong nhom
								on nhomdt.ID_NhomDoiTuong = nhom.ID
								where nhomdt.ID_DoiTuong = dt.ID 
								order by nhom.ID
								For XML PATH ('')),'00000000-0000-0000-0000-000000000000') as IDNhoms,
					   ISNULL((select 
									nhom.TenNhomDoiTuong + ', ' AS [text()]
								from DM_DoiTuong_Nhom nhomdt
								join DM_NhomDoiTuong nhom
								on nhomdt.ID_NhomDoiTuong = nhom.ID
								where nhomdt.ID_DoiTuong = dt.ID 
								order by nhom.ID
								For XML PATH ('')),N'Nhóm mặc định') as TenNhoms
				from DM_DoiTuong  dt 
				where dt.TheoDoi=0 and LoaiDoiTuong= 1				
				and not exists (select Name from dbo.splitstring(@ID_DoiTuongs) a where a.Name= ID) 
		) dt2 on dt.ID= dt2.ID	


END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoKho_NhapChuyenHang]
    @SearchString [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @LaHangHoa [nvarchar](max),
    @TheoDoi [nvarchar](max),
    @TrangThai [nvarchar](max),
    @ID_NhomHang UNIQUEIDENTIFIER,
    @ID_NguoiDung [uniqueidentifier]
AS
BEGIN
	 SET NOCOUNT ON;
	 DECLARE @tblIdDonVi TABLE (ID UNIQUEIDENTIFIER);
	 INSERT INTO @tblIdDonVi
	 SELECT Name FROM [dbo].[splitstring](@ID_ChiNhanh) 

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

    DECLARE @XemGiaVon as nvarchar
    	Set @XemGiaVon = (Select 
    		Case when nd.LaAdmin = '1' then '1' else
    		Case when nd.XemGiaVon is null then '0' else nd.XemGiaVon end end as XemGiaVon
    		From HT_NguoiDung nd	    		
    		where nd.ID = @ID_NguoiDung);
	
	-- because like @TheoDoi was slow (không hiểu tại sao chỉ BC này bị chậm vi like @TheoDoi, các BC khác vẫn bình thường)
	declare @sTrangThai varchar(10) ='0,1'
	  set @TheoDoi= REPLACE(@TheoDoi,'%','')
		if @TheoDoi !=''
		set @sTrangThai= @TheoDoi	

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
					qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_CheckIn,
					sum(tblHD.SoLuong * iif(qd.TyLeChuyenDoi=0,1, qd.TyLeChuyenDoi)) as SoLuong,
					sum(ThanhTien) as ThanhTien
				from(
				select ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn,
					sum(ct.TienChietKhau) as SoLuong,
					sum(ct.TienChietKhau * ct.GiaVon) as ThanhTien
				from BH_HoaDon_ChiTiet ct
				join BH_HoaDon hd on ct.ID_HoaDon= hd.ID
				where hd.ChoThanhToan=0
				and hd.LoaiHoaDon= 10 and (hd.YeuCau='1' or hd.YeuCau='4') --- YeuCau: 1.DangChuyen, 4.DaNhan, 2.PhieuTam, 3.Huy
				and hd.NgaySua >=@timeStart and hd.NgaySua <@timeEnd
				and exists (select ID from @tblIdDonVi dv where hd.ID_CheckIn= dv.ID)
				group by ct.ID_DonViQuiDoi, ct.ID_LoHang, hd.ID_CheckIn
				)tblHD
				join DonViQuiDoi qd on tblHD.ID_DonViQuiDoi= qd.ID
				group by qd.ID_HangHoa,tblHD.ID_LoHang, tblHD.ID_CheckIn
			)tblQD
			join DM_DonVi dv on tblQD.ID_CheckIn = dv.ID
			join DM_HangHoa hh on tblQD.ID_HangHoa= hh.ID
			join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa and qd.LaDonViChuan=1
			left join DM_NhomHangHoa nhom on hh.ID_NhomHang= nhom.ID
			left join DM_LoHang lo on tblQD.ID_LoHang= lo.ID and (lo.ID= tblQD.ID_LoHang or tblQD.ID_LoHang is null and lo.ID is null)
			where hh.LaHangHoa = 1
			and exists (select Name from dbo.splitstring(@sTrangThai) tt where hh.TheoDoi= tt.Name )
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

            CreateStoredProcedure(name: "[dbo].[GetAll_TonKhoDauKy]", parametersAction: p => new
            {
                IDDonVis = p.String(),
                ToDate = p.DateTime()
            }, body: @"SET NOCOUNT ON;
	DECLARE @tblChiNhanh TABLE(ID UNIQUEIDENTIFIER);
	INSERT INTO @tblChiNhanh SELECT Name FROM splitstring(@IDDonVis)

		SELECT 
			ID_DonViInput,
			ID_HangHoa, 		
			ID_LoHang,
			IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, TonLuyKe_NhanChuyenHang, TonLuyKe) AS TonKho, 
			IIF(LoaiHoaDon = 10 AND ID_CheckIn = ID_DonViInput, GiaVon_NhanChuyenHang, GiaVon) AS GiaVon
			FROM (
			SELECT tbltemp.*, ROW_NUMBER() OVER (PARTITION BY tbltemp.ID_HangHoa, tbltemp.ID_LoHang, tbltemp.ID_DonViInput ORDER BY tbltemp.ThoiGian DESC) AS RN 
			FROM (
				SELECT hd.LoaiHoaDon, dvqd.ID_HangHoa, hd.ID_DonVi, hd.ID_CheckIn, lstDv.ID AS ID_DonViInput, 
					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, hdct.TonLuyKe_NhanChuyenHang, hdct.TonLuyKe) AS TonLuyKe,
					hdct.TonLuyKe_NhanChuyenHang,
					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, 
					hdct.GiaVon_NhanChuyenHang, 
					hdct.GiaVon)/ISNULL(dvqd.TyLeChuyenDoi,1) AS GiaVon,
					hdct.GiaVon_NhanChuyenHang, 
					hdct.ID_LoHang ,
					IIF(hd.LoaiHoaDon = 10 AND hd.YeuCau = '4' AND hd.ID_CheckIn = lstDv.ID, hd.NgaySua, hd.NgayLapHoaDon) AS ThoiGian
				FROM BH_HoaDon_ChiTiet hdct
				JOIN BH_HoaDon hd ON hd.ID = hdct.ID_HoaDon				
				JOIN DonViQuiDoi dvqd ON dvqd.ID = hdct.ID_DonViQuiDoi				
				INNER JOIN @tblChiNhanh lstDv ON lstDv.ID = hd.ID_DonVi OR (hd.ID_CheckIn = lstDv.ID and hd.YeuCau = '4')				
				where hd.ChoThanhToan = 0 AND hd.LoaiHoaDon IN (1, 5, 7, 8, 4, 6, 9, 10)
				and exists (select ID from @tblChiNhanh dv where hd.ID_DonVi= dv.ID or (hd.ID_CheckIn= dv.ID and hd.LoaiHoaDon= 10))
				) as tbltemp
		WHERE tbltemp.ThoiGian < @ToDate) tblTonKhoTemp
		WHERE tblTonKhoTemp.RN = 1;");

            CreateStoredProcedure(name: "[dbo].[Insert_ThongBaoHetTonKho]", parametersAction: p => new
            {
                ID_ChiNhanh = p.Guid(),
                IDDonViQuyDois = p.String(),
                IDLoHangs = p.String()
            }, body: @"SET NOCOUNT ON;
	declare @dtNow datetime = format( getdate(),'yyyy-MM-dd')

	declare @tblTonKho table(TonKho float, ID_HangHoa uniqueidentifier, ID_LoHang uniqueidentifier,
	ID_DonViQuiDoi uniqueidentifier, MaHangHoa nvarchar(max), MaLoHang nvarchar(max), QuanLyTheoLoHang bit, TonToiThieu float)
	insert into @tblTonKho
	exec GetTonKho_byIDQuyDois @ID_ChiNhanh, @dtNow,@IDDonViQuyDois, @IDLoHangs


	insert into HT_ThongBao
	select newid(), @ID_ChiNhanh,0, 
	CONCAT(N'<p onclick=""loaddadoc(''key',''')""> Hàng hóa <a onclick=""loadthongbao(''1'', ''', MaHangHoa,''', ''key'')"">',

    '<span class=""blue"">', MaHangHoa, ' </span>', N'</a> đã hết số lượng tồn kho. Vui lòng nhập thêm để tiếp tục kinh doanh </p>'),
	GETDATE(),''

    from @tblTonKho

    where TonKho - TonToiThieu <= 0");

            CreateStoredProcedure(name: "[dbo].[UpdateKhachHang_DuDKNangNhom]", parametersAction: p => new
            {
                ID_NhomDoiTuong = p.Guid(),
                IDChiNhanhs = p.String()
            }, body: @"SET NOCOUNT ON;

	declare @fromDate varchar(20) ='2016-01-01', 
	@dtNow varchar(20) = format(dateadd(day,1,getdate()),'yyyy-MM-dd'),
	@countDK int=0,
	@sql1 nvarchar(max),
	@sql2 nvarchar(max), 
	@sql3 nvarchar(max), 
	@sql4 nvarchar(max), 
	@sql5 nvarchar(max), 
	@checkThoiGianMua int = 0,
	@whereThoiGianMua nvarchar(max)='',
	@where nvarchar(max)=''

	select @countDK = count(ID) from DM_NhomDoiTuong_ChiTiet where ID_NhomDoiTuong like @ID_NhomDoiTuong
	-- chi insert neu nhom co dieu kien nang nhom
	if @countDK > 0
	begin

		-- check thoigian mua hang
			declare @LoaiDieuKien1 int ,@LoaiSoSanh1 int, @GiaTriThoiGian1 datetime
			DECLARE _cur1 CURSOR FOR 
			select 
				ct.LoaiDieuKien, ct.LoaiSoSanh, ct.GiaTriThoiGian
			from DM_NhomDoiTuong_ChiTiet ct
			where ID_NhomDoiTuong like @ID_NhomDoiTuong and ct.LoaiDieuKien  = 3
   
			OPEN _cur1  
			FETCH NEXT FROM _cur1
			INTO  @LoaiDieuKien1, @LoaiSoSanh1,  @GiaTriThoiGian1 

			WHILE @@FETCH_STATUS = 0  
			BEGIN  

				if @whereThoiGianMua !='' and @whereThoiGianMua is not null 
					set @whereThoiGianMua= CONCAT( @whereThoiGianMua, ' AND ')		

				 set @whereThoiGianMua = concat(@whereThoiGianMua, 
				   ' NgayLapHoaDon ',
					case  @LoaiSoSanh1
							when 1 then CONCAT( ' > ''', FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd'), '''')
							when 2 then CONCAT( ' >= ''', FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd'), '''')
							when 3 then CONCAT( ' >= ''',  FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd')
										, ''' AND NgayLapHoaDon < ''', FORMAT(DATEADD(day, 1, @GiaTriThoiGian1),'yyyy-MM-dd'), '''')
							when 4 then CONCAT( ' < ''',  FORMAT(DATEADD(day, 1, @GiaTriThoiGian1),'yyyy-MM-dd'), '''')
							when 5 then CONCAT( ' < ''', FORMAT(@GiaTriThoiGian1,'yyyy-MM-dd'), '''')
							else   CONCAT( ' NgayLapHoaDon > ''',  FORMAT(DATEADD(day, -1, @GiaTriThoiGian1),'yyyy-MM-dd'), ''' AND NgayLapHoaDon < ''' , 
							FORMAT(DATEADD(day, 1, @GiaTriThoiGian1),'yyyy-MM-dd'), '''')
					end 
					)
				  FETCH NEXT FROM _cur1 
				  INTO @LoaiDieuKien1, @LoaiSoSanh1,  @GiaTriThoiGian1 
			END 

			CLOSE _cur1  
			DEALLOCATE _cur1 
		
			-- get list chinhanh
			set @sql1 = concat( 'declare @tblChiNhanh table(ID_Donvi uniqueidentifier)
				insert into @tblChiNhanh
				select Name  from dbo.splitstring(''', @IDChiNhanhs , ''') ')

			-- if: khong co dieukien ve thoigian mua
			if	@whereThoiGianMua!='' 
			begin
				set	@checkThoiGianMua = 1
				set @whereThoiGianMua =concat(' and ', @whereThoiGianMua)
			end
			
			-- check cac dieukien khac
			declare @LoaiDieuKien int ,@LoaiSoSanh int, @GiaTriSo int ,@GiaTriBool bit, @GiaTriThoiGian datetime ,@GiaTriKhuVuc uniqueidentifier, @GiaTriVungMien uniqueidentifier

			DECLARE _cur CURSOR FOR 
			select  ct.LoaiDieuKien, ct.LoaiSoSanh, ct.GiaTriSo, ct.GiaTriBool, ct.GiaTriThoiGian, ct.GiaTriKhuVuc, ct.GiaTriVungMien	
		   from DM_NhomDoiTuong_ChiTiet ct
		   where ID_NhomDoiTuong like @ID_NhomDoiTuong and ct.LoaiDieuKien  != 3
	
			OPEN _cur  
			FETCH NEXT FROM _cur
			INTO  @LoaiDieuKien, @LoaiSoSanh, @GiaTriSo, @GiaTriBool, @GiaTriThoiGian ,@GiaTriKhuVuc , @GiaTriVungMien 

			WHILE @@FETCH_STATUS = 0  
			BEGIN  

				if @where !='' and @where is not null set @where= CONCAT( @where, ' AND ')
				 set @where = concat(@where, 
				 case  @LoaiDieuKien
						when 1 then ' TongBanTruTraHang '
						when 2 then ' TongBan '
						when 4 then ' SoLanMuaHang '
						when 5 then ' NoHienTai '
						when 6 then ' ThangSinh != -1 and ThangSinh  '
						when 7 then ' NgaySinh_NgayTLap is not null  and DinhDang_NgaySinh != ''dd/MM'' and NgaySinh_NgayTLap '
						when 8 then ' GioiTinhNam '
						when 9 then ' ID_TinhThanh '
					end,
					case  @LoaiSoSanh
									when 1 then ' > '
									when 2 then ' >= '
									when 3 then ' = '
									when 4 then ' <= '
									when 5 then ' < '
									else ' != ' 
					end ,
					case  @LoaiDieuKien			
						when 1 then  concat('', @GiaTriSo)
						when 2 then  concat('', @GiaTriSo)
						when 4 then  concat('', @GiaTriSo)
						when 5 then  concat('', @GiaTriSo)
						when 6 then  concat('', @GiaTriSo)
						when 7 then  concat(' DATEADD (year, ', -@GiaTriSo ,' , FORMAT( NgaySinh_NgayTLap,''yyyy-MM-dd'')) ')
						when 8 then  concat('', @GiaTriBool)
						end ,'' )
				  FETCH NEXT FROM _cur 
				  INTO @LoaiDieuKien, @LoaiSoSanh, @GiaTriSo, @GiaTriBool, @GiaTriThoiGian ,@GiaTriKhuVuc , @GiaTriVungMien 
			END 

			CLOSE _cur  
			DEALLOCATE _cur 

			if	@where!='' set @where =concat(' where ', @where)

			set @sql2 = concat('
			-- get DS Khachhang du dk nangnhom
			select tbl.ID, tbl.MaDoiTuong
			into #temp 
			from
			(
		   SELECT 
    				  dt.ID,
    				  dt.MaDoiTuong,      	   		
    				  dt.GioiTinhNam,
					  iif(dt.NgaySinh_NgayTLap is null,-1, datepart(month,dt.NgaySinh_NgayTLap)) as ThangSinh,
    				  dt.NgaySinh_NgayTLap,
    				  dt.DinhDang_NgaySinh,
     				  dt.ID_TinhThanh,
    				  dt.ID_QuanHuyen,
    				  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    				  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    				  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    				  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
					   CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai
    				  FROM DM_DoiTuong dt  
			
    						LEFT JOIN (
    							SELECT tblThuChi.ID_DoiTuong,
    							SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) - SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
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
    									0 AS SoLanMuaHang
    								FROM BH_HoaDon bhd
    								WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = ''0''
									and bhd.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    									AND bhd.NgayLapHoaDon >= ''',@fromDate , 
										''' AND bhd.NgayLapHoaDon < ''',@dtNow ,
										''' AND  exists (select ID_DonVi from @tblChiNhanh  dv where  bhd.ID_DonVi = dv.ID_DonVi) ',
							
    
    								' union all
    									-- tongtra
    								SELECT bhd.ID_DoiTuong,
    									ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    									0 AS DoanhThu,
    									0 AS TienThu,
    									0 AS TienChi, 
    									0 AS SoLanMuaHang
    								FROM BH_HoaDon bhd   						
    								WHERE (bhd.LoaiHoaDon = 6 OR bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = ''0'' 
										and	bhd.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    									AND bhd.NgayLapHoaDon >= ''',@fromDate ,''' AND bhd.NgayLapHoaDon < ''', @dtNow ,
										''' AND exists (select ID_DonVi from @tblChiNhanh  dv where  bhd.ID_DonVi = dv.ID_DonVi) ')
    							
		   set @sql3= concat(' union all
    
    									-- tienthu
    									SELECT 
    									qhdct.ID_DoiTuong,						
    									0 AS GiaTriTra,
    									0 AS DoanhThu,
    									ISNULL(qhdct.TienThu,0) AS TienThu,
    									0 AS TienChi,
    										0 AS SoLanMuaHang
    								FROM Quy_HoaDon qhd
    								JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    									Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    								WHERE qhd.LoaiHoaDon = 11 AND  (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
									and	qhdct.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
									and (qhdct.HinhThucThanhToan is null or qhdct.HinhThucThanhToan !=6)
    								AND exists (select ID_DonVi from @tblChiNhanh  dv where  qhd.ID_DonVi = dv.ID_DonVi)  
    								AND qhd.NgayLapHoaDon >= ''', @fromDate , ''' AND qhd.NgayLapHoaDon < ''', @dtNow, 
								
									'''	union all
    
    									-- tienchi
    								SELECT 
    									qhdct.ID_DoiTuong,						
    									0 AS GiaTriTra,
    									0 AS DoanhThu,
    									0 AS TienThu,
    									ISNULL(qhdct.TienThu,0) AS TienChi,
    										0 AS SoLanMuaHang
    								FROM Quy_HoaDon qhd
    								JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    								WHERE qhd.LoaiHoaDon = 12 AND (qhd.TrangThai != ''0'' OR qhd.TrangThai is null)
										and	qhdct.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
										and (qhdct.HinhThucThanhToan is null or qhdct.HinhThucThanhToan !=6)
    									AND qhd.NgayLapHoaDon >= ''', @fromDate, 
										''' AND qhd.NgayLapHoaDon < ''', @dtNow, ''' 
										AND exists (select ID_DonVi from @tblChiNhanh  dv where  qhd.ID_DonVi = dv.ID_DonVi) ')

		 set @sql4= 	concat(	' Union All
    									-- solan mua hang
    								Select 
    									hd.ID_DoiTuong,
    									0 AS GiaTriTra,
    									0 AS DoanhThu,
    									0 AS TienThu,
    										0 as TienChi,
    									COUNT(*) AS SoLanMuaHang								
    								From BH_HoaDon hd 
    								where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 25 or hd.LoaiHoaDon = 19)
    								and hd.ChoThanhToan = 0
									and	hd.ID_DoiTuong not like ''%00000000-0000-0000-0000-0000%''
    								AND hd.NgayLapHoaDon >= ''', @fromDate ,''' AND hd.NgayLapHoaDon < ''', @dtNow,
									''' AND exists (select ID_DonVi from @tblChiNhanh  dv where  hd.ID_DonVi = dv.ID_DonVi) 
    									 GROUP BY hd.ID_DoiTuong  	
    									)AS tblThuChi
    								GROUP BY tblThuChi.ID_DoiTuong
    						) a on dt.ID = a.ID_DoiTuong  					
    								WHERE dt.TheoDoi= 0 and  dt.loaidoituong = 1 and dt.ID not like ''%00000000-0000-0000-0000-0000%''
									AND ( ', @checkThoiGianMua,  ' = 0    OR 
										exists ( select ID_DoiTuong from BH_HoaDon hd
											where hd.LoaiHoaDon in (1,19,22,25)
											and hd.ID_DoiTuong = dt.ID
											and hd.ID_DoiTuong not like ''%00000000%''', @whereThoiGianMua, '
											 ) )
		 ) tbl ', @where)

		 -- 1. get DS khachhang khongdu dieukien nangnhom & delete
		 -- 2. insert again khachhang du dieukien
		 set @sql5 = concat(' delete  dtn from DM_DoiTuong_Nhom dtn where not exists (select ID from #temp tmp where dtn.ID_DoiTuong = tmp.ID)',
		 ' and dtn.ID_NhomDoiTuong=''', @ID_NhomDoiTuong, '''',

		 '	insert into DM_DoiTuong_Nhom
			select NEWID(),  tmp.ID, ''', @ID_NhomDoiTuong,'''
			from #temp tmp where
		    not exists (select ID from DM_DoiTuong_Nhom dtn where tmp.ID = dtn.ID_DoiTuong and dtn.ID_NhomDoiTuong = ''', @ID_NhomDoiTuong,''')')

	exec ( @sql1 + @sql2 + @sql3+ @sql4 + @sql5)
	---- print @sql5
	end");
        }
        
        public override void Down()
        {
            Sql("DROP TRIGGER [dbo].[trg_DeleteNhomDoiTuongs]");
            DropStoredProcedure("[dbo].[GetAll_TonKhoDauKy]");
            DropStoredProcedure("[dbo].[Insert_ThongBaoHetTonKho]");
            DropStoredProcedure("[dbo].[UpdateKhachHang_DuDKNangNhom]");
        }
    }
}
