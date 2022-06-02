namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20210117 : DbMigration
    {
        public override void Up()
        {
			Sql(@"ALTER FUNCTION [dbo].[FUNC_TonLuyKeTruocThoiGian]
    (
    @ID_ChiNhanh [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier],
	@ID_LoHang [uniqueidentifier],
	@TimeStart [datetime]
	)
RETURNS FLOAT
AS
    BEGIN
	DECLARE @TonKho AS FLOAT;
	DECLARE @timeStartCS DATETIME;
	Set @timeStartCS =  (select convert(datetime, '2016/01/01'))
	--DECLARE @SQL VARCHAR(254)
 --   Select @SQL = (Select Count(*) FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 --   if (@SQL > 0)
 --   BEGiN
 --   Select @timeStartCS =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh)
 --   END	

	SET @TonKho = 0;
	SELECT TOP(1) @TonKho = TonKho FROM
	(
	SELECT CASE WHEN @ID_ChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' THEN bhct.TonLuyKe_NhanChuyenHang ELSE bhct.TonLuyKe END as TonKho, CASE WHEN @ID_ChiNhanh = hd.ID_CheckIn and hd.YeuCau = '4' THEN hd.NgaySua ELSE hd.NgayLapHoaDon END as NgayLapHoaDon FROM BH_HoaDon hd
	LEFT JOIN BH_HoaDon_ChiTiet bhct on hd.ID = bhct.ID_HoaDon
	LEFT JOIN DonViQuiDoi dvqd on bhct.ID_DonViQuiDoi = dvqd.ID
	 where hd.LoaiHoaDon != 3 AND hd.LoaiHoaDon != 19 and hd.LoaiHoaDon != 25 and dvqd.ID_HangHoa = @ID_HangHoa and (bhct.ID_LoHang = @ID_LoHang or @ID_LoHang is null) and 
	 ((hd.ID_DonVi = @ID_ChiNhanh and hd.NgayLapHoaDon < @TimeStart and hd.NgayLapHoaDon > @timeStartCS and ((hd.YeuCau != '2' and hd.YeuCau != '3') or hd.YeuCau is null))
     or (hd.YeuCau = '4'  and hd.ID_CheckIn = @ID_ChiNhanh and hd.NgaySua < @TimeStart and hd.NgaySua > @timeStartCS)) and ChoThanhToan = 0
	 ) as tbl1
	 order by tbl1.NgayLapHoaDon desc

	RETURN ISNULL(@TonKho, 0);
END");

			Sql(@"CREATE FUNCTION [dbo].[DiscountSale_NVBanHang]
(	
	@IDChiNhanhs varchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanVien varchar(40)
)
RETURNS TABLE 
AS
RETURN 
(
	select 	
	1 as LoaiNhanVienApDung,
	tblNVBan.ID_NhanVien,
	tblNVBan.DoanhThu,
	tblNVBan.ThucThu,
	case when tblNVBan.LaPhanTram =1 then
    		case when tblNVBan.TinhChietKhauTheo=2 then DoanhThu * GiaTriChietKhau / 100 else 0 end 
    		else case when tblNVBan.TinhChietKhauTheo=2 then GiaTriChietKhau else 0 end end as HoaHongDoanhThu,   
    case when tblNVBan.LaPhanTram =1 then
    		case when tblNVBan.TinhChietKhauTheo=1 then ThucThu * GiaTriChietKhau / 100 else 0 end 
    		else case when tblNVBan.TinhChietKhauTheo=1 then GiaTriChietKhau else 0 end end as HoaHongThucThu   ,
			tblNVBan.ID as IDChiTietCK
from
(

select  b.* ,  ckct.GiaTriChietKhau, ckct.LaPhanTram,ckct.ID,
ROW_NUMBER() over (PARTITION  by b.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
from
(
select 
 a.ID_NhanVien,
    								a.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								sum(a.TongThanhToan) -sum(GiaTriTra)  as DoanhThu, 
    								sum(TienThu) -sum(TienTraKhach) as ThucThu,						
									a.ID_ChietKhauDoanhThu 
								
from
(

select 

 ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								hd.TongThanhToan, 
    								0 as TienThu,
    								0 as GiaTriTra,
    								0 as TienTraKhach,
									ckdtnv.ID_ChietKhauDoanhThu, 
									ckdt.ApDungTuNgay,
									ckdt.ApDungDenNgay
from ChietKhauDoanhThu ckdt
join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
join BH_HoaDon hd on nvth.ID_HoaDon = hd.ID and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
where hd.ChoThanhToan= 0 
and  exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)
and hd.LoaiHoaDon in (1,19,22, 25)
and ckdt.LoaiNhanVienApDung= 1
and hd.NgayLapHoaDon >= @FromDate  and hd.NgayLapHoaDon < @ToDate
and nvth.ID_NhanVien like @IDNhanVien

union all



--- trahang
select  ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
									0 as TienThu,
    								hdt.PhaiThanhToan as GiaTriTra,
    								ISNULL(qhdct.TienThu, 0) as TienTraKhach,
    								ckdtnv.ID_ChietKhauDoanhThu,
									ckdt.ApDungTuNgay, 
									ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
								join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
    							join BH_HoaDon hdt on nvth.ID_HoaDon = hdt.ID 
								and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							Join BH_HoaDon hd on hd.ID = hdt.ID_HoaDon
    							left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    							left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							 exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name) 
    							and ckdt.LoaiNhanVienApDung=1
    							and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
    							and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
								and (qhd.TrangThai is null or qhd.TrangThai != 0)
								and nvth.ID_NhanVien like @IDNhanVien

								union all

--- thucthu

select ckdtnv.ID_NhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
    								case when ISNULL(qhdct.ThuTuThe, 0) > 0 or ISNULL(qhdct.DiemThanhToan, 0) > 0 then 0 else ISNULL(qhdct.TienThu, 0) end as TienThu,-- ThucThu(khong lay giatri ThuTuThe or ThanhToan = diem)
    								0 as GiaTriTra,
    								0 as TienTraKhach,
    								ckdtnv.ID_ChietKhauDoanhThu,
									ckdt.ApDungTuNgay, 
									ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu			
    							join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
								join BH_HoaDon hd on nvth.ID_HoaDon = hd.ID and ckdt.ID_DonVi = hd.ID_DonVi 
								and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hd.ID
    							join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							 exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)
    							and ckdt.LoaiNhanVienApDung=1
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
								and nvth.ID_NhanVien like @IDNhanVien
    							and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    							and (qhd.TrangThai is null or qhd.TrangThai != 0)
) a
group by a.ID_ChietKhauDoanhThu, a.ID_NhanVien,  a.TinhChietKhauTheo
)b
join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
and ((b.DoanhThu >= ckct.DoanhThuTu and b.TinhChietKhauTheo = 2) 
								or (b.ThucThu >= ckct.DoanhThuTu and b.TinhChietKhauTheo = 1))
) tblNVBan where Rn= 1
)
");

			Sql(@"CREATE FUNCTION [dbo].[DiscountSale_NVienDichVu]
(	
	@IDChiNhanhs varchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanVien varchar(40)
)
RETURNS TABLE 
AS
RETURN 
(
	select  2 as LoaiNhanVienApDung, tblNVienDV.ID_NhanVien, tblNVienDV.DoanhThu, 
	case when tblNVienDV.LaPhanTram = 1 then DoanhThu * GiaTriChietKhau / 100 else  GiaTriChietKhau end as HoaHongDoanhThu,
	tblNVienDV.ID
from
(
select b.ID_NhanVien, b.DoanhThu, ckct.GiaTriChietKhau,ckct.LaPhanTram,ckct.ID,
ROW_NUMBER() over (PARTITION  by b.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
from
(
select  a.ID_NhanVien, sum(a.DoanhThu) - sum(a.GiaTriTra) as DoanhThu, a.ID_ChietKhauDoanhThu
from
(

select 

 ckdtnv.ID_NhanVien , nvth.ID_ChiTietHoaDon, hd.MaHoaDon,
    							
    								cthd.ThanhTien as DoanhThu,     							
    								0 as GiaTriTra,
									ckdtnv.ID_ChietKhauDoanhThu, 
									ckdt.ApDungTuNgay,
									ckdt.ApDungDenNgay
from ChietKhauDoanhThu ckdt
join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
join BH_HoaDon_ChiTiet cthd on nvth.ID_ChiTietHoaDon = cthd.ID 
join BH_HoaDon hd on cthd.ID_HoaDon= hd.ID
and ckdt.ID_DonVi = hd.ID_DonVi and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
where hd.ChoThanhToan= 0 
and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)
and hd.LoaiHoaDon in (1,19,22)
and ckdt.LoaiNhanVienApDung= 2
and hd.NgayLapHoaDon >= @FromDate  and hd.NgayLapHoaDon < @ToDate
and cthd.ChatLieu!=4
and nvth.ID_NhanVien like @IDNhanVien

union all


--- trahang
select  ckdtnv.ID_NhanVien ,nvth.ID_ChiTietHoaDon,hdt.MaHoaDon,
    								
    								0 as DoanhThu, 									
    								cthd.ThanhTien  as GiaTriTra,
    								ckdtnv.ID_ChietKhauDoanhThu,
									ckdt.ApDungTuNgay, 
									ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu		
								join BH_NhanVienThucHien nvth on ckdtnv.ID_NhanVien = nvth.ID_NhanVien 
								join BH_HoaDon_ChiTiet cthd on nvth.ID_ChiTietHoaDon = cthd.ID 
    							join BH_HoaDon hdt on cthd.ID_HoaDon = hdt.ID 
								and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    							left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hdt.ID_DonVi= dv.Name)
    							and ckdt.LoaiNhanVienApDung=2
    							and hdt.ChoThanhToan = '0' and hdt.LoaiHoaDon = 6
    							and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
								and (qhd.TrangThai is null or qhd.TrangThai != 0)
								and nvth.ID_NhanVien like @IDNhanVien
) a group by a.ID_NhanVien, a.ID_ChietKhauDoanhThu
) b 
join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
and (b.DoanhThu >= ckct.DoanhThuTu) 								
)tblNVienDV where tblNVienDV.Rn= 1
)
");

			Sql(@"CREATE FUNCTION [dbo].[DiscountSale_NVLapHoaDon]
(	
	@IDChiNhanhs varchar(max),
	@FromDate datetime,
	@ToDate datetime,
	@IDNhanVien varchar(40)
)
RETURNS TABLE 
AS
RETURN 
(
	select 3 as LoaiNhanVienApDung, tblMax.ID_NhanVien,
	tblMax.DoanhThu,tblMax.ThucThu,
	case when tblMax.LaPhanTram =1 then
    		case when tblMax.TinhChietKhauTheo=2 then tblMax.DoanhThu * tblMax.GiaTriChietKhau / 100 else 0 end 
    		else case when tblMax.TinhChietKhauTheo=2 then tblMax.GiaTriChietKhau else 0 end end as HoaHongDoanhThu,   
    case when tblMax.LaPhanTram =1 then
    		case when tblMax.TinhChietKhauTheo=1 then tblMax.ThucThu * tblMax.GiaTriChietKhau / 100 else 0 end 
    		else case when tblMax.TinhChietKhauTheo=1 then tblMax.GiaTriChietKhau else 0 end end as HoaHongThucThu ,
			tblMax.ID
from
(
select tblNVLap.ID_NhanVien, tblNVLap.DoanhThu, tblNVLap.ThucThu,tblNVLap.TinhChietKhauTheo, ckct.GiaTriChietKhau,ckct.LaPhanTram, ckct.ID,
ROW_NUMBER() over (PARTITION  by tblNVLap.ID_NhanVien order by ckct.DoanhThuTu desc)as Rn
	
from
(
select a.ID_NhanVien,
	a.ID_ChietKhauDoanhThu,
	a.ApDungTuNgay,
	a.ApDungDenNgay,
	a.TinhChietKhauTheo,
	SUM(a.PhaiThanhToan - a.GiaTriTra) as DoanhThu,
    SUM(a.TienThu - a.TienTraKhach) as ThucThu
from
(
-- doanh thu
					select  ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								hd.TongThanhToan as PhaiThanhToan, 
    								0 as TienThu,
    								0 as GiaTriTra,
    								0 as TienTraKhach,
									ckdt.ID as ID_ChietKhauDoanhThu, 
									ckdt.ApDungTuNgay,
									ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu    													
    							join BH_HoaDon hd on ckdtnv.ID_NhanVien = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi
								and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon 
								and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							where ckdt.LoaiNhanVienApDung=3
								and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)    							    							
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
								and hd.ID_NhanVien like @IDNhanVien
    							and hd.NgayLapHoaDon >= @FromDate  and hd.NgayLapHoaDon < @ToDate

								-- thucthu
								union all
								select  ckdtnv.ID_NhanVien, 
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 
    								case when ISNULL(qhdct.ThuTuThe, 0) > 0 or ISNULL(qhdct.DiemThanhToan, 0) > 0 then 0 else ISNULL(qhdct.TienThu, 0) end as TienThu,-- ThucThu(khong lay giatri ThuTuThe or ThanhToan = diem)
    								0 as GiaTriTra,
    								0 as TienTraKhach,
    								ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu			
    							join BH_HoaDon hd on ckdtnv.ID_NhanVien = hd.ID_NhanVien and ckdt.ID_DonVi = hd.ID_DonVi 
								and (ckdt.ApDungTuNgay <= hd.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hd.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hd.ID
    							join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)    
    							and ckdt.LoaiNhanVienApDung=3
    							and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
    							and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon < @ToDate
    							and (qhd.TrangThai is null or qhd.TrangThai != 0)
								and hd.ID_NhanVien like @IDNhanVien

								union all
								-- hdtra
								select ckdtnv.ID_NhanVien ,
    								ckdt.TinhChietKhauTheo, --1 thực thu, 2 doanh thu
    								0 as PhaiThanhToan, 0 as TienThu,
    								hdt.PhaiThanhToan as GiaTriTra,
    								ISNULL(qhdct.TienThu, 0) as TienTraKhach,
    								ckdt.ID as ID_ChietKhauDoanhThu, ckdt.ApDungTuNgay, ckdt.ApDungDenNgay
    							from ChietKhauDoanhThu ckdt
    							join ChietKhauDoanhThu_NhanVien ckdtnv on ckdt.ID  = ckdtnv.ID_ChietKhauDoanhThu					
    							join BH_HoaDon hdt on ckdtnv.ID_NhanVien = hdt.ID_NhanVien and ckdt.ID_DonVi = hdt.ID_DonVi and (ckdt.ApDungTuNgay <= hdt.NgayLapHoaDon and (Dateadd(day, 1,ckdt.ApDungDenNgay) >= hdt.NgayLapHoaDon or ckdt.ApDungDenNgay is null))
    							Join BH_HoaDon hd on hd.ID = hdt.ID_HoaDon
    							left join Quy_HoaDon_ChiTiet qhdct on qhdct.ID_HoaDonLienQuan = hdt.ID
    							left join Quy_HoaDon qhd on qhdct.ID_HoaDon = qhd.ID
    							where 
    							 exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where hd.ID_DonVi= dv.Name)  
    							and 
								ckdt.LoaiNhanVienApDung=3
    							and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22, 25)
    							and hdt.NgayLapHoaDon >= @FromDate and hdt.NgayLapHoaDon < @ToDate
								and (qhd.TrangThai is null or qhd.TrangThai != 0)
								and hdt.ID_NhanVien like @IDNhanVien
								) a 
								group by a.ID_ChietKhauDoanhThu, a.ID_NhanVien, a.ApDungTuNgay,  a.ApDungDenNgay, a.TinhChietKhauTheo
								) tblNVLap
								join ChietKhauDoanhThu_ChiTiet ckct on
								tblNVLap.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu 
								and ((tblNVLap.DoanhThu >= DoanhThuTu and tblNVLap.TinhChietKhauTheo = 2) 
								or (tblNVLap.ThucThu >= ckct.DoanhThuTu and tblNVLap.TinhChietKhauTheo = 1))
								)tblMax where Rn= 1
)
");

			CreateStoredProcedure(name: "[dbo].[BaoCaoDoanhThuSuaChuaChiTiet]", parametersAction: p => new
			{
				IdChiNhanhs = p.String(),
				ThoiGianFrom = p.DateTime(),
				ThoiGianTo = p.DateTime(),
				DoanhThuFrom = p.Double(),
				DoanhThuTo = p.Double(),
				LoiNhuanFrom = p.Double(),
				LoiNhuanTo = p.Double(),
				TextSearch = p.String()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
	if(@IdChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IdChiNhanhs);
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearch);

	DECLARE @tblHoaDonSuaChua TABLE (MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), 
	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, ID_DonViQuiDoi UNIQUEIDENTIFIER, IDChiTiet UNIQUEIDENTIFIER,
	MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), SoLuong FLOAT, DonGia FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, TienThue FLOAT,
	GiamGia FLOAT, DoanhThu FLOAT,
	GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));

	INSERT INTO @tblHoaDonSuaChua
	SELECT ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, dmx.BienSo, dt.MaDoiTuong, dt.TenDoiTuong, nv.TenNhanVien, hd.ID,
	hd.MaHoaDon, hd.NgayLapHoaDon, hdct.ID_DonViQuiDoi, hdct.ID,
	dvqd.MaHangHoa, hh.TenHangHoa, dvqd.TenDonViTinh, hdct.SoLuong, hdct.DonGia, hdct.TienChietKhau*hdct.SoLuong, hdct.ThanhToan, hdct.TienThue*hdct.SoLuong,
	hdct.ThanhToan * hd.TongGiamGia/(hd.TongThanhToan + hd.TongGiamGia) AS GiamGia, (hdct.ThanhToan - (hdct.ThanhToan * hd.TongGiamGia/(hd.TongThanhToan + hd.TongGiamGia))) AS DoanhThu,
	hdct.GhiChu, dv.MaDonVi, dv.TenDonVi FROM Gara_PhieuTiepNhan ptn
	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
	INNER JOIN BH_HoaDon_ChiTiet hdct ON hd.ID = hdct.ID_HoaDon
	INNER JOIN DonViQuiDoi dvqd ON hdct.ID_DonViQuiDoi = dvqd.ID
	INNER JOIN DM_HangHoa hh ON hh.ID = dvqd.ID_HangHoa
	INNER JOIN Gara_DanhMucXe dmx ON ptn.ID_Xe = dmx.ID
	INNER JOIN DM_DoiTuong dt ON dt.ID = ptn.ID_KhachHang
	LEFT JOIN NS_NhanVien nv ON ptn.ID_CoVanDichVu = nv.ID
	INNER JOIN DM_DonVi dv ON dv.ID = hd.ID_DonVi
	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0 --AND ptn.TrangThai != 0
	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
	AND ((select count(Name) from @tblSearch b where     			
			ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
			or dmx.BienSo like '%'+b.Name+'%'
			or dt.MaDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong like '%'+b.Name+'%'
			or nv.TenNhanVien like '%'+b.Name+'%'
			or hd.MaHoaDon like '%'+b.Name+'%'
			or hd.DienGiai like '%'+b.Name+'%'
			)=@count or @count=0);

	DECLARE @tblBaoCaoDoanhThu TABLE(MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), 
	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, MaHangHoa NVARCHAR(MAX), TenHangHoa NVARCHAR(MAX), TenDonViTinh NVARCHAR(MAX), SoLuong FLOAT, DonGia FLOAT, TienChietKhau FLOAT, ThanhTien FLOAT, 
	TienThue FLOAT,
	GiamGia FLOAT, DoanhThu FLOAT, GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), TienVon FLOAT, LoiNhuan FLOAT)

	INSERT INTO @tblBaoCaoDoanhThu
	SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, 
	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon,
	hdsc.MaHangHoa, hdsc.TenHangHoa, hdsc.TenDonViTinh, hdsc.SoLuong, hdsc.DonGia, hdsc.TienChietKhau, hdsc.ThanhTien, hdsc.TienThue,
	hdsc.GiamGia, hdsc.DoanhThu,
	hdsc.GhiChu, hdsc.MaDonVi, hdsc.TenDonVi, SUM(ISNULL(xkct.GiaVon,0)*ISNULL(xkct.SoLuong,0)) AS TienVon,
	hdsc.DoanhThu - SUM(ISNULL(xkct.GiaVon,0)*ISNULL(xkct.SoLuong,0)) AS LoiNhuan
	FROM @tblHoaDonSuaChua hdsc
	LEFT JOIN BH_HoaDon xk ON hdsc.ID = xk.ID_HoaDon
	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon AND xkct.ID_ChiTietGoiDV = hdsc.IDChiTiet
	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL
	GROUP BY hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, 
	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon,
	hdsc.MaHangHoa, hdsc.TenHangHoa, hdsc.TenDonViTinh, hdsc.SoLuong, hdsc.DonGia, hdsc.TienChietKhau, hdsc.ThanhTien, hdsc.TienThue,
	hdsc.GiamGia, hdsc.DoanhThu,
	hdsc.GhiChu, hdsc.MaDonVi, hdsc.TenDonVi;

	DECLARE @SThanhTien FLOAT,  @SChietKhau FLOAT, @SThue FLOAT, @SGiamGia FLOAT, @SDoanhThu FLOAT, @STongTienVon FLOAT, @SLoiNhuan FLOAT
	SELECT @SThanhTien = SUM(ThanhTien), @SChietKhau = SUM(TienChietKhau), @SThue = SUM(TienThue), @SGiamGia = SUM(GiamGia), @SDoanhThu = SUM(DoanhThu), @STongTienVon = SUM(TienVon), @SLoiNhuan = SUM(LoiNhuan) 
	FROM @tblBaoCaoDoanhThu

	SELECT MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu , ID AS IDHoaDon, MaHoaDon,
	NgayLapHoaDon, MaHangHoa, TenHangHoa, TenDonViTinh, ISNULL(SoLuong, 0) AS SoLuong, ISNULL(DonGia, 0) AS DonGia, ISNULL(TienChietKhau, 0) AS TienChietKhau, 
	ISNULL(TienThue,0) AS TienThue, ISNULL(ThanhTien,0) AS ThanhTien, ISNULL(GiamGia, 0) AS GiamGia, ISNULL(DoanhThu, 0) AS DoanhThu, ISNULL(TienVon,0) AS TienVon, ISNULL(LoiNhuan,0) AS LoiNhuan,
	GhiChu, MaDonVi, TenDonVi, ISNULL(@SThanhTien, 0) AS SThanhTien, ISNULL(@SChietKhau,0) AS SChietKhau,
	ISNULL(@SThue,0) AS SThue, ISNULL(@SGiamGia,0) AS SGiamGia, ISNULL(@SDoanhThu, 0) AS SDoanhThu, ISNULL(@STongTienVon,0) AS STongTienVon,
	ISNULL(@SLoiNhuan,0) AS SLoiNhuan
	FROM @tblBaoCaoDoanhThu
	WHERE (@DoanhThuFrom IS NULL OR DoanhThu >= @DoanhThuFrom)
	AND (@DoanhThuTo IS NULL OR DoanhThu <= @DoanhThuTo)
	AND (@LoiNhuanFrom IS NULL OR LoiNhuan >= @LoiNhuanFrom)
	AND (@LoiNhuanTo IS NULL OR LoiNhuan <= @LoiNhuanTo)
	ORDER BY NgayLapHoaDon");

			CreateStoredProcedure(name: "[dbo].[BaoCaoDoanhThuSuaChuaTheoCoVan]", parametersAction: p => new
			{
				IdChiNhanhs = p.String(),
				ThoiGianFrom = p.DateTime(),
				ThoiGianTo = p.DateTime(),
				SoLanTiepNhanFrom = p.Double(),
				SoLanTiepNhanTo = p.Double(),
				SoLuongHoaDonFrom = p.Double(),
				SoLuongHoaDonTo = p.Double(),
				DoanhThuFrom = p.Double(),
				DoanhThuTo = p.Double(),
				LoiNhuanFrom = p.Double(),
				LoiNhuanTo = p.Double(),
				TextSearch = p.String()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
	if(@IdChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IdChiNhanhs);
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearch);

	DECLARE @tblHoaDonSuaChua TABLE (IDCoVan UNIQUEIDENTIFIER, MaNhanVien NVARCHAR(MAX), TenNhanVien NVARCHAR(MAX), 
	IDPhieuTiepNhan UNIQUEIDENTIFIER, IDHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, DoanhThu FLOAT, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));

	INSERT INTO @tblHoaDonSuaChua
	SELECT nv.ID, nv.MaNhanVien, nv.TenNhanVien, ptn.ID, hd.ID, hd.NgayLapHoaDon, hd.TongThanhToan, dv.MaDonVi, dv.TenDonVi
	FROM Gara_PhieuTiepNhan ptn
	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
	INNER JOIN NS_NhanVien nv ON nv.ID = ptn.ID_CoVanDichVu
	INNER JOIN DM_DonVi dv ON dv.ID = hd.ID_DonVi
	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0
	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
	AND ((select count(Name) from @tblSearch b where     			
			nv.MaNhanVien like '%'+b.Name+'%'
			or nv.TenNhanVien like '%'+b.Name+'%'
			)=@count or @count=0);

	DECLARE @tblTienVon TABLE(IDCoVan UNIQUEIDENTIFIER, TienVon FLOAT);

	INSERT INTO @tblTienVon
	SELECT hdsc.IDCoVan, SUM(ISNULL(xkct.GiaVon,0)*ISNULL(xkct.SoLuong,0)) AS TienVon
	FROM @tblHoaDonSuaChua hdsc
	LEFT JOIN BH_HoaDon xk ON hdsc.IDHoaDon = xk.ID_HoaDon
	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL
	GROUP BY hdsc.IDCoVan

	DECLARE @SSoLanTiepNhan FLOAT, @SSoLuongHoaDon FLOAT, @STongDoanhThu FLOAT, @STienVon FLOAT, @SLoiNhuan FLOAT;

	DECLARE @tblBaoCaoDoanhThu TABLE(IDCoVan UNIQUEIDENTIFIER, MaNhanVien NVARCHAR(MAX), TenNhanVien NVARCHAR(MAX),
	SoLanTiepNhan FLOAT, SoLuongHoaDon FLOAT, TongDoanhThu FLOAT, TongTienVon FLOAT, LoiNhuan FLOAT, NgayGiaoDichGanNhat DATETIME, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX))
	
	INSERT INTO @tblBaoCaoDoanhThu
	SELECT hd.IDCoVan, hd.MaNhanVien, hd.TenNhanVien, hd.SoLanTiepNhan, hd.SoLuongHoaDon,
	ISNULL(hd.TongDoanhThu,0) AS TongDoanhThu, ISNULL(tv.TienVon,0) AS TongTienVon, ISNULL(hd.TongDoanhThu,0) - ISNULL(tv.TienVon,0) AS LoiNhuan, hd.NgayGiaoDichGanNhat, hd.MaDonVi, hd.TenDonVi
	FROM
	(
	SELECT IDCoVan, MaNhanVien, TenNhanVien, MaDonVi, TenDonVi, COUNT(DISTINCT IDPhieuTiepNhan) AS SoLanTiepNhan, COUNT(IDHoaDon) AS SoLuongHoaDon, SUM(DoanhThu) AS TongDoanhThu,
	MAX(NgayLapHoaDon) AS NgayGiaoDichGanNhat
	FROM @tblHoaDonSuaChua
	GROUP BY IDCoVan, MaNhanVien, TenNhanVien, MaDonVi, TenDonVi) AS hd
	INNER JOIN @tblTienVon tv ON hd.IDCoVan = tv.IDCoVan
	WHERE (@SoLanTiepNhanFrom IS NULL OR hd.SoLanTiepNhan >= @SoLanTiepNhanFrom)
	AND (@SoLanTiepNhanTo IS NULL OR hd.SoLanTiepNhan <= @SoLanTiepNhanTo)
	AND (@SoLuongHoaDonFrom IS NULL OR hd.SoLuongHoaDon >= @SoLuongHoaDonFrom)
	AND (@SoLuongHoaDonTo IS NULL OR hd.SoLuongHoaDon <= @SoLuongHoaDonTo)
	AND (@DoanhThuFrom IS NULL OR hd.TongDoanhThu >= @DoanhThuFrom)
	AND (@DoanhThuTo IS NULL OR hd.TongDoanhThu <= @DoanhThuTo)
	AND (@LoiNhuanFrom IS NULL OR hd.TongDoanhThu - tv.TienVon >= @LoiNhuanFrom)
	AND (@LoiNhuanTo IS NULL OR hd.TongDoanhThu - tv.TienVon <= @LoiNhuanTo)

	SELECT @SSoLanTiepNhan = SUM(SoLanTiepNhan), @SSoLuongHoaDon = SUM(SoLuongHoaDon), @STongDoanhThu = SUM(TongDoanhThu), @STienVon = SUM(TongTienVon), @SLoiNhuan = SUM(LoiNhuan) FROM @tblBaoCaoDoanhThu

	SELECT *, CAST(@SSoLanTiepNhan AS FLOAT) AS SSoLanTiepNhan, @SSoLuongHoaDon AS SSoLuongHoaDon, @STongDoanhThu AS STongDoanhThu, @STienVon AS STienVon, @SLoiNhuan AS SLoiNhuan FROM @tblBaoCaoDoanhThu
	ORDER BY TenNhanVien");

			CreateStoredProcedure(name: "[dbo].[BaoCaoDoanhThuSuaChuaTheoXe]", parametersAction: p => new
			{
				IdChiNhanhs = p.String(),
				ThoiGianFrom = p.DateTime(),
				ThoiGianTo = p.DateTime(),
				SoLanTiepNhanFrom = p.Double(),
				SoLanTiepNhanTo = p.Double(),
				SoLuongHoaDonFrom = p.Double(),
				SoLuongHoaDonTo = p.Double(),
				DoanhThuFrom = p.Double(),
				DoanhThuTo = p.Double(),
				LoiNhuanFrom = p.Double(),
				LoiNhuanTo = p.Double(),
				TextSearch = p.String()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
	if(@IdChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IdChiNhanhs);
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearch);

	DECLARE @tblHoaDonSuaChua TABLE (IDXe UNIQUEIDENTIFIER, BienSo NVARCHAR(MAX), SoMay NVARCHAR(MAX), SoKhung NVARCHAR(MAX), MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), DienThoai NVARCHAR(MAX), 
	IDPhieuTiepNhan UNIQUEIDENTIFIER, IDHoaDon UNIQUEIDENTIFIER, NgayLapHoaDon DATETIME, DoanhThu FLOAT, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));

	INSERT INTO @tblHoaDonSuaChua
	SELECT dmx.ID, dmx.BienSo, dmx.SoMay, dmx.SoKhung,
	dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, ptn.ID, hd.ID, hd.NgayLapHoaDon, hd.TongThanhToan, dv.MaDonVi, dv.TenDonVi
	FROM Gara_PhieuTiepNhan ptn
	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
	INNER JOIN Gara_DanhMucXe dmx ON ptn.ID_Xe = dmx.ID
	INNER JOIN DM_DoiTuong dt ON dt.ID = ptn.ID_KhachHang
	INNER JOIN DM_DonVi dv ON dv.ID = hd.ID_DonVi
	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0
	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
	AND ((select count(Name) from @tblSearch b where     			
			dmx.BienSo like '%'+b.Name+'%'
			or dt.MaDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong like '%'+b.Name+'%'
			or dmx.SoMay like '%'+b.Name+'%'
			or dmx.SoKhung like '%'+b.Name+'%'
			or dt.DienThoai like '%'+b.Name+'%'
			or dv.TenDonVi like '%'+b.Name + '%'
			)=@count or @count=0);

	DECLARE @tblTienVon TABLE(IDXe UNIQUEIDENTIFIER, TienVon FLOAT);

	INSERT INTO @tblTienVon
	SELECT hdsc.IDXe, SUM(ISNULL(xkct.GiaVon,0)*ISNULL(xkct.SoLuong,0)) AS TienVon
	FROM @tblHoaDonSuaChua hdsc
	LEFT JOIN BH_HoaDon xk ON hdsc.IDHoaDon = xk.ID_HoaDon
	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL
	GROUP BY hdsc.IDXe

	DECLARE @SSoLanTiepNhan FLOAT, @SSoLuongHoaDon FLOAT, @STongDoanhThu FLOAT, @STienVon FLOAT, @SLoiNhuan FLOAT;

	DECLARE @tblBaoCaoDoanhThu TABLE(IDXe UNIQUEIDENTIFIER, BienSo NVARCHAR(MAX), SoKhung NVARCHAR(MAX), SoMay NVARCHAR(MAX), MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX),
	DienThoai NVARCHAR(MAX), SoLanTiepNhan FLOAT, SoLuongHoaDon FLOAT, TongDoanhThu FLOAT, TongTienVon FLOAT, LoiNhuan FLOAT, NgayGiaoDichGanNhat DATETIME, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX))
	
	INSERT INTO @tblBaoCaoDoanhThu
	SELECT hd.IDXe, hd.BienSo, hd.SoKhung, hd.SoMay, hd.MaDoiTuong, hd.TenDoiTuong, hd.DienThoai, hd.SoLanTiepNhan, hd.SoLuongHoaDon,
	ISNULL(hd.TongDoanhThu,0) AS TongDoanhThu, ISNULL(tv.TienVon,0) AS TongTienVon, ISNULL(hd.TongDoanhThu,0) - ISNULL(tv.TienVon,0) AS LoiNhuan, hd.NgayGiaoDichGanNhat, hd.MaDonVi, hd.TenDonVi
	FROM
	(
	SELECT IDXe, BienSo, SoMay, SoKhung,  MaDoiTuong, TenDoiTuong, DienThoai, MaDonVi, TenDonVi, COUNT(DISTINCT IDPhieuTiepNhan) AS SoLanTiepNhan, COUNT(IDHoaDon) AS SoLuongHoaDon, SUM(DoanhThu) AS TongDoanhThu,
	MAX(NgayLapHoaDon) AS NgayGiaoDichGanNhat
	FROM @tblHoaDonSuaChua
	GROUP BY IDXe, BienSo, SoMay, SoKhung,  MaDoiTuong, TenDoiTuong, DienThoai, MaDonVi, TenDonVi) AS hd
	INNER JOIN @tblTienVon tv ON hd.IDXe = tv.IDXe
	WHERE (@SoLanTiepNhanFrom IS NULL OR hd.SoLanTiepNhan >= @SoLanTiepNhanFrom)
	AND (@SoLanTiepNhanTo IS NULL OR hd.SoLanTiepNhan <= @SoLanTiepNhanTo)
	AND (@SoLuongHoaDonFrom IS NULL OR hd.SoLuongHoaDon >= @SoLuongHoaDonFrom)
	AND (@SoLuongHoaDonTo IS NULL OR hd.SoLuongHoaDon <= @SoLuongHoaDonTo)
	AND (@DoanhThuFrom IS NULL OR hd.TongDoanhThu >= @DoanhThuFrom)
	AND (@DoanhThuTo IS NULL OR hd.TongDoanhThu <= @DoanhThuTo)
	AND (@LoiNhuanFrom IS NULL OR hd.TongDoanhThu - tv.TienVon >= @LoiNhuanFrom)
	AND (@LoiNhuanTo IS NULL OR hd.TongDoanhThu - tv.TienVon <= @LoiNhuanTo)

	SELECT @SSoLanTiepNhan = SUM(SoLanTiepNhan), @SSoLuongHoaDon = SUM(SoLuongHoaDon), @STongDoanhThu = SUM(TongDoanhThu), @STienVon = SUM(TongTienVon), @SLoiNhuan = SUM(LoiNhuan) FROM @tblBaoCaoDoanhThu

	SELECT *, CAST(@SSoLanTiepNhan AS FLOAT) AS SSoLanTiepNhan, @SSoLuongHoaDon AS SSoLuongHoaDon, @STongDoanhThu AS STongDoanhThu, @STienVon AS STienVon, @SLoiNhuan AS SLoiNhuan FROM @tblBaoCaoDoanhThu
	ORDER BY BienSo");

			CreateStoredProcedure(name: "[dbo].[BaoCaoDoanhThuSuaChuaTongHop]", parametersAction: p => new
			{
				IdChiNhanhs = p.String(),
				ThoiGianFrom = p.DateTime(),
				ThoiGianTo = p.DateTime(),
				DoanhThuFrom = p.Double(),
				DoanhThuTo = p.Double(),
				LoiNhuanFrom = p.Double(),
				LoiNhuanTo = p.Double(),
				TextSearch = p.String()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
	if(@IdChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IdChiNhanhs);
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearch);

	DECLARE @tblHoaDonSuaChua TABLE (MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), 
	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, TongTienHang FLOAT, TongChietKhau FLOAT, TongTienThue FLOAT, TongChiPhi FLOAT,
	TongGiamGia FLOAT, TongThanhToan FLOAT, GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX));

	INSERT INTO @tblHoaDonSuaChua
	SELECT ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, dmx.BienSo, dt.MaDoiTuong, dt.TenDoiTuong, nv.TenNhanVien, hd.ID,
	hd.MaHoaDon, hd.NgayLapHoaDon, SUM(hdct.SoLuong* hdct.DonGia), SUM(ISNULL(hdct.TienChietKhau, 0)*hdct.SoLuong), hd.TongTienThue, hd.TongChiPhi,
	hd.TongGiamGia, hd.TongThanhToan, hd.DienGiai, dv.MaDonVi, dv.TenDonVi FROM Gara_PhieuTiepNhan ptn
	INNER JOIN BH_HoaDon hd ON hd.ID_PhieuTiepNhan = ptn.ID
	INNER JOIN BH_HoaDon_ChiTiet hdct ON hd.ID = hdct.ID_HoaDon
	INNER JOIN Gara_DanhMucXe dmx ON ptn.ID_Xe = dmx.ID
	INNER JOIN DM_DoiTuong dt ON dt.ID = ptn.ID_KhachHang
	LEFT JOIN NS_NhanVien nv ON ptn.ID_CoVanDichVu = nv.ID
	INNER JOIN DM_DonVi dv ON dv.ID = hd.ID_DonVi
	INNER JOIN @tblDonVi dvf ON dv.ID = dvf.ID_DonVi
	WHERE hd.LoaiHoaDon = 25 AND hd.ChoThanhToan = 0
	AND (@ThoiGianFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @ThoiGianFrom AND @ThoiGianTo)
	AND ((select count(Name) from @tblSearch b where     			
			ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
			or dmx.BienSo like '%'+b.Name+'%'
			or dt.MaDoiTuong like '%'+b.Name+'%'
			or dt.TenDoiTuong like '%'+b.Name+'%'
			or nv.TenNhanVien like '%'+b.Name+'%'
			or hd.MaHoaDon like '%'+b.Name+'%'
			or hd.DienGiai like '%'+b.Name+'%'
			)=@count or @count=0)
	GROUP BY ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, dmx.BienSo, dt.MaDoiTuong, dt.TenDoiTuong, nv.TenNhanVien, hd.ID,
	hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienThue, hd.TongChiPhi,
	hd.TongGiamGia, hd.TongThanhToan, hd.DienGiai, dv.MaDonVi, dv.TenDonVi;

	DECLARE @tblBaoCaoDoanhThu TABLE(MaPhieuTiepNhan NVARCHAR(MAX), NgayVaoXuong DATETIME, BienSo NVARCHAR(MAX), 
	MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), CoVanDichVu NVARCHAR(MAX),
	ID UNIQUEIDENTIFIER, MaHoaDon NVARCHAR(MAX), NgayLapHoaDon DATETIME, TongTienHang FLOAT, TongChietKhau FLOAT, TongTienThue FLOAT, TongChiPhi FLOAT,
	TongGiamGia FLOAT, TongThanhToan FLOAT, GhiChu NVARCHAR(MAX), MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), GiaVon FLOAT, TienVon FLOAT, LoiNhuan FLOAT)

	INSERT INTO @tblBaoCaoDoanhThu
	SELECT hdsc.MaPhieuTiepNhan, hdsc.NgayVaoXuong, hdsc.BienSo, 
	hdsc.MaDoiTuong, hdsc.TenDoiTuong, hdsc.CoVanDichVu,
	hdsc.ID, hdsc.MaHoaDon, hdsc.NgayLapHoaDon, hdsc.TongTienHang, hdsc.TongChietKhau, hdsc.TongTienThue, hdsc.TongChiPhi,
	hdsc.TongGiamGia, hdsc.TongThanhToan, hdsc.GhiChu, hdsc.MaDonVi, hdsc.TenDonVi, SUM(ISNULL(xkct.GiaVon,0)) AS GiaVon, SUM(ISNULL(xkct.GiaVon,0)*ISNULL(xkct.SoLuong,0)) AS TienVon,
	hdsc.TongThanhToan - SUM(ISNULL(xkct.GiaVon,0)*ISNULL(xkct.SoLuong,0)) AS LoiNhuan
	FROM @tblHoaDonSuaChua hdsc
	LEFT JOIN BH_HoaDon xk ON hdsc.ID = xk.ID_HoaDon
	LEFT JOIN BH_HoaDon_ChiTiet xkct ON xk.ID = xkct.ID_HoaDon
	WHERE (xk.LoaiHoaDon = 8 AND xk.ChoThanhToan = 0) OR xk.ID IS NULL
	GROUP BY hdsc.BienSo, hdsc.CoVanDichVu, hdsc.GhiChu, hdsc.ID, hdsc.MaDoiTuong, hdsc.MaHoaDon, 
	hdsc.MaPhieuTiepNhan, hdsc.NgayLapHoaDon, hdsc.NgayVaoXuong, hdsc.TenDoiTuong,
	hdsc.TongChietKhau, hdsc.TongChiPhi, hdsc.TongGiamGia, hdsc.TongThanhToan,
	hdsc.TongTienHang, hdsc.TongTienThue, hdsc.MaDonVi, hdsc.TenDonVi

	DECLARE @STongTienHang FLOAT,  @SChietKhau FLOAT, @SThue FLOAT, @SChiPhi FLOAT, @SGiamGia FLOAT, @SDoanhThu FLOAT, @STongTienVon FLOAT, @SLoiNhuan FLOAT
	SELECT @STongTienHang = SUM(TongTienHang), @SChietKhau = SUM(TongChietKhau), @SThue = SUM(TongTienThue),
	@SChiPhi = SUM(TongChiPhi), @SGiamGia = SUM(TongGiamGia), @SDoanhThu = SUM(TongThanhToan), @STongTienVon = SUM(TienVon), @SLoiNhuan = SUM(LoiNhuan) 
	FROM @tblBaoCaoDoanhThu

	SELECT MaPhieuTiepNhan, NgayVaoXuong, BienSo, MaDoiTuong, TenDoiTuong, CoVanDichVu , ID AS IDHoaDon, MaHoaDon,
	NgayLapHoaDon, ISNULL(TongTienHang, 0) AS TongTienHang, ISNULL(TongChietKhau, 0) AS TongChietKhau, ISNULL(TongTienThue, 0) AS TongTienThue, 
	ISNULL(TongChiPhi, 0) AS TongChiPhi, ISNULL(TongGiamGia, 0) AS TongGiamGia, 
	ISNULL(TongThanhToan, 0) AS DoanhThu, ISNULL(Tienvon, 0) AS TienVon, ISNULL(LoiNhuan, 0) AS LoiNhuan, GhiChu, MaDonVi, TenDonVi, ISNULL(@STongTienHang, 0) AS STongTienHang, ISNULL(@SChietKhau,0) AS SChietKhau,
	ISNULL(@SThue,0) AS SThue, ISNULL(@SChiPhi,0) AS SChiPhi, ISNULL(@SGiamGia,0) AS SGiamGia, ISNULL(@SDoanhThu, 0) AS SDoanhThu, ISNULL(@STongTienVon,0) AS STongTienVon,
	ISNULL(@SLoiNhuan,0) AS SLoiNhuan
	FROM @tblBaoCaoDoanhThu
	WHERE (@DoanhThuFrom IS NULL OR TongThanhToan >= @DoanhThuFrom)
	AND (@DoanhThuTo IS NULL OR TongThanhToan <= @DoanhThuTo)
	AND (@LoiNhuanFrom IS NULL OR LoiNhuan >= @LoiNhuanFrom)
	AND (@LoiNhuanTo IS NULL OR LoiNhuan <= @LoiNhuanTo)
	ORDER BY NgayLapHoaDon");

			CreateStoredProcedure(name: "[dbo].[DiscountSale_byIDNhanVien]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				ID_NhanVien = p.String(40),
				FromDate = p.DateTime(),
				ToDate = p.DateTime()
			}, body: @"SET NOCOUNT ON;
	set @ToDate = dateadd(day,1, @ToDate) 

	declare @tblSale_NVLapHD table(LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float, IDChiTietCK uniqueidentifier)
	insert into @tblSale_NVLapHD
	select * from dbo.DiscountSale_NVLapHoaDon(@IDChiNhanhs, @FromDate, @ToDate,@ID_NhanVien)

	declare @tblSale_NBanHang table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float,IDChiTietCK uniqueidentifier)
	insert into @tblSale_NBanHang
	select * from dbo.DiscountSale_NVBanHang (@IDChiNhanhs,@FromDate,@ToDate,@ID_NhanVien)

	declare @tblSale_NVDichVu table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, HoaHongDoanhThu float, IDChiTietCK uniqueidentifier)
	insert into @tblSale_NVDichVu
	select * from dbo.DiscountSale_NVienDichVu (@IDChiNhanhs,@FromDate,@ToDate,@ID_NhanVien);

	select 
		case a.LoaiNhanVienApDung
		when 1 then N'Nhân viên bán hàng'
		when 2 then N'Nhân viên thực hiện/tư vấn'
		when 3 then N'Nhân viên lập hóa đơn'
		end as LoaiNVApDung,
		case when dt.TinhChietKhauTheo= 1 then N'Theo doanh thu' else 
		case when a.LoaiNhanVienApDung = 2 then N'Theo doanh thu' else N'Theo thực thu' end end as HinhThuc,
		ct.DoanhThuTu, ct.DoanhThuDen, ct.GiaTriChietKhau, ct.LaPhanTram,
		DoanhThu, ThucThu, HoaHongDoanhThu +  HoaHongThucThu as HoaHong,		
		dt.ApDungTuNgay, dt.ApDungDenNgay
	from
	(
	select * from @tblSale_NVLapHD 
	union all
	select * from  @tblSale_NBanHang 
	union all
	select LoaiNhanVienApDung, ID_NhanVien, DoanhThu, 0 as ThucThu, HoaHongDoanhThu, 0 as HoaHongThucThu, IDChiTietCK from @tblSale_NVDichVu 
	) a
    join ChietKhauDoanhThu_ChiTiet ct on a.IDChiTietCK= ct.ID
	join ChietKhauDoanhThu dt on ct.ID_ChietKhauDoanhThu= dt.ID");

			CreateStoredProcedure(name: "[dbo].[Gara_GetListBaoGia]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.String(14),
				ToDate = p.String(14),
				ID_PhieuSuaChua = p.String(),
				TrangThais = p.String(20),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	if @FromDate = '2016-01-01' 
		set @ToDate= (select format(DATEADD(day,1, max(NgayLapHoaDon)),'yyyy-MM-dd') from BH_HoaDon where LoaiHoaDon= 3)

	declare @tblDonVi table (ID_DonVi uniqueidentifier)
	insert into @tblDonVi
	select Name from dbo.splitstring(@IDChiNhanhs)

	declare @tbTrangThai table (GiaTri varchar(2))
	insert into @tbTrangThai
	select Name from dbo.splitstring(@TrangThais)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);	

	with data_cte
	as
	(
			select *
			from
			(
			select hd.*,
				tn.MaPhieuTiepNhan,
				dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.Email, dt.DiaChi,
				case hd.ChoThanhToan
					when 0 then '0'
					when 1 then '1'
					else '2' end as TrangThai,
				case hd.ChoThanhToan
					when 0 
						then 
							case hd.YeuCau
							when '1' then N'Đã duyệt'
							when '2' then N'Đang xử lý'
							when '3' then N'Hoàn thành'
							end 
					when 1 then N'Chờ duyệt'
					else N'Đã hủy'
					end as TrangThaiText
			from BH_HoaDon hd
			join Gara_PhieuTiepNhan tn on tn.ID= hd.ID_PhieuTiepNhan
			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
			where hd.LoaiHoaDon= 3
			and exists (select ID_DonVi from @tblDonVi dv where tn.ID_DonVi = dv.ID_DonVi)
			and hd.NgayLapHoaDon >=@FromDate and hd.NgayLapHoaDon < @ToDate
			and hd.ID_PhieuTiepNhan like @ID_PhieuSuaChua
			and
				((select count(Name) from @tblSearch b where     			
				hd.MaHoaDon like '%'+b.Name+'%'
				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong like '%'+b.Name+'%'	
				or dt.DienThoai like '%'+b.Name+'%'				
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
				or hd.NguoiTao like '%'+b.Name+'%'										
				)=@count or @count=0)	
			) a where exists (select GiaTri from @tbTrangThai tt where a.TrangThai = tt.GiaTri)
	),
	count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
			from data_cte
		)
		select dt.*, cte.*, 
			dt.NguoiTao as NguoiTaoHD,
			nv.TenNhanVien
		from data_cte dt
		join NS_NhanVien nv on dt.ID_NhanVien= nv.ID
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[Gara_GetListHangHoa_ByIDQuiDoi]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				ID_BangGia = p.String(40),
				IDQuiDois = p.String()
			}, body: @"SET NOCOUNT ON;
	declare @tblIDQuiDoi table (ID_DonViQuiDoi uniqueidentifier)
	insert into @tblIDQuiDoi
	select Name from dbo.splitstring(@IDQuiDois)

	
	select 
		a.ID_DonViQuiDoi,ID,ID_LoHang,ID_NhomHangHoa,LaHangHoa,
		MaHangHoa,TenHangHoa, TenDonViTinh,TyLeChuyenDoi,MaLoHang, NgaySanXuat, NgayHetHan,
		ThuocTinhGiaTri,LaDonViChuan, PhiDichVu, LaPTPhiDichVu,
		QuyCach, DonViTinhQuyCach, QuanLyTheoLoHang, ThoiGianBaoHanh, LoaiBaoHanh,
		SoPhutThucHien, GhiChuHH, DichVuTheoGio, DuocTichDiem, TonKho, GiaVon,
		isnull(b.GiaBan2, GiaBan) as GiaBan,
		CONCAT(MaHangHoa, ' ', lower(MaHangHoa),' ', TenHangHoa, ' ', TenHangHoa_KhongDau,' ',
		MaLoHang, ' ', GiaBan, ' ', ThuocTinhGiaTri) as Name
	from(
    select qd.ID as ID_DonViQuiDoi, qd.MaHangHoa, qd.TenDonViTinh, qd.ThuocTinhGiaTri, qd.TyLeChuyenDoi, qd.GiaBan, qd.LaDonViChuan,
			hh.ID, hh.TenHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, hh.LaHangHoa, hh.TenHangHoa_KhongDau,
			Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
			Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
			case when ISNULL(QuyCach,0) = 0 then TyLeChuyenDoi else QuyCach * TyLeChuyenDoi end as QuyCach,
			ISNULL(hh.DonViTinhQuyCach,'0') as DonViTinhQuyCach,
			ISNULL(QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
			ISNULL(ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
			ISNULL(LoaiBaoHanh,0) as LoaiBaoHanh,
			ISNULL(SoPhutThucHien,0) as SoPhutThucHien, 
			ISNULL(hh.GhiChu,'') as GhiChuHH ,
			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio, 
			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem, 
			isnull(tk.TonKho, 0) as TonKho,
			isnull(gv.GiaVon, 0) as GiaVon,
			lo.ID as ID_LoHang, 
			lo.MaLoHang,
			lo.NgaySanXuat,
			lo.NgayHetHan
		from DonViQuiDoi qd
		join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
		left join DM_LoHang lo on hh.ID= lo.ID_HangHoa
		left join DM_HangHoa_TonKho tk on tk.ID_DonViQuyDoi= qd.ID and (lo.ID = tk.ID_LoHang or lo.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh
		left join DM_GiaVon gv on (qd.ID = gv.ID_DonViQuiDoi and (lo.ID = gv.ID_LoHang or lo.ID is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where (qd.xoa ='0'  or qd.Xoa is null)
		and hh.TheoDoi = '1'	
		and exists (select ID_DonViQuiDoi from @tblIDQuiDoi qd2 where qd.ID= qd2.ID_DonViQuiDoi)
		and (hh.LaHangHoa = 0 or (hh.LaHangHoa = 1 and tk.TonKho is not null) -- chi lay HangHoa neu exsit in DM_TonKho_HangHoa
		and (convert(varchar,lo.NgayHetHan,112) >= convert(varchar, getdate(),112) or lo.NgayHetHan is null)
		and (ISNULL(hh.QuanLyTheoLoHang,'0')='0' or (hh.QuanLyTheoLoHang='1' and MaLoHang!='')))
	) a
	left join
	(			
		select ct.ID_DonViQuiDoi, ct.GiaBan as GiaBan2
		from DM_GiaBan_ChiTiet ct where ct.ID_GiaBan = @ID_BangGia
		) b on a.ID_DonViQuiDoi= b.ID_DonViQuiDoi");

			CreateStoredProcedure(name: "[dbo].[Gara_JqAutoHangHoa]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				ID_BangGia = p.String(40),
				TextSearch = p.String(200),
				LaHangHoa = p.String(10),
				QuanLyTheoLo = p.String(10),
				ConTonKho = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @dtNow varchar(14) = convert(varchar, getdate(),112)
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearchString);

	select 
		ID_DonViQuiDoi,ID,ID_LoHang,ID_NhomHangHoa,LaHangHoa,
		MaHangHoa,TenHangHoa, TenDonViTinh,TyLeChuyenDoi,MaLoHang, NgaySanXuat, NgayHetHan,
		ThuocTinhGiaTri,LaDonViChuan, PhiDichVu, LaPTPhiDichVu,
		QuyCach, DonViTinhQuyCach, QuanLyTheoLoHang, ThoiGianBaoHanh, LoaiBaoHanh,
		SoPhutThucHien, GhiChuHH, DichVuTheoGio, DuocTichDiem, TonKho, GiaVon, 
		isnull(GiaBan2, GiaBan) as GiaBan,
		TenNhomHangHoa,
		CONCAT(MaHangHoa, ' ', lower(MaHangHoa),' ', TenHangHoa, ' ', TenHangHoa_KhongDau,' ',
		MaLoHang, ' ', GiaBan, ' ', ThuocTinhGiaTri) as Name
	from(
	select top 20 a.*, b.GiaBan2
	from
	(
	-- loc dieu kien tonkho> 0
	select *
	from
	(
	select *
	from
	(
		select qd.ID as ID_DonViQuiDoi, qd.MaHangHoa, qd.TenDonViTinh, qd.ThuocTinhGiaTri, qd.TyLeChuyenDoi, qd.GiaBan, qd.LaDonViChuan,
			hh.ID, hh.TenHangHoa, hh.ID_NhomHang as ID_NhomHangHoa, hh.LaHangHoa, hh.TenHangHoa_KhongDau,
			Case when hh.LaHangHoa='1' then 0 else CAST(ISNULL(hh.ChiPhiThucHien,0) as float) end as PhiDichVu,
			Case when hh.LaHangHoa='1' then '0' else ISNULL(hh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
			case when ISNULL(QuyCach,0) = 0 then TyLeChuyenDoi else QuyCach * TyLeChuyenDoi end as QuyCach,
			ISNULL(hh.DonViTinhQuyCach,'0') as DonViTinhQuyCach,
			ISNULL(QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
			ISNULL(ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
			ISNULL(LoaiBaoHanh,0) as LoaiBaoHanh,
			ISNULL(SoPhutThucHien,0) as SoPhutThucHien, 
			ISNULL(hh.GhiChu,'') as GhiChuHH ,
			ISNULL(hh.DichVuTheoGio,0) as DichVuTheoGio, 
			ISNULL(hh.DuocTichDiem,0) as DuocTichDiem, 
			isnull(tk.TonKho, 0) as TonKho,
			isnull(gv.GiaVon, 0) as GiaVon,
			lo.ID as ID_LoHang, 
			lo.MaLoHang,
			lo.NgaySanXuat,
			lo.NgayHetHan,
			isnull(nhh.TenNhomHangHoa,'') as TenNhomHangHoa,
			convert(varchar,isnull(lo.NgayHetHan, @dtNow),112) as NgayHetHanCompare
		from DonViQuiDoi qd
		join DM_HangHoa hh on qd.ID_HangHoa = hh.ID
		left join DM_NhomHangHoa nhh on hh.ID_NhomHang= nhh.ID
		left join DM_LoHang lo on hh.ID= lo.ID_HangHoa
		left join DM_HangHoa_TonKho tk on tk.ID_DonViQuyDoi= qd.ID and (lo.ID = tk.ID_LoHang or lo.ID is null) and  tk.ID_DonVi = @ID_ChiNhanh
		left join DM_GiaVon gv on (qd.ID = gv.ID_DonViQuiDoi and (lo.ID = gv.ID_LoHang or lo.ID is null) and gv.ID_DonVi = @ID_ChiNhanh)
		where (qd.xoa ='0'  or qd.Xoa is null)
		and hh.TheoDoi = '1'	
		and hh.LaHangHoa like @LaHangHoa
		and hh.QuanLyTheoLoHang like @QuanLyTheoLo
		and hh.LaHangHoa like @LaHangHoa
		and (hh.LaHangHoa = 0 or (hh.LaHangHoa = 1 and tk.TonKho is not null)) -- chi lay HangHoa neu exsit in DM_TonKho_HangHoa
		) a1
		--and (convert(varchar,lo.NgayHetHan,112) >= convert(varchar, getdate(),112) or lo.NgayHetHan is null)
		--and (ISNULL(hh.QuanLyTheoLoHang,'0')='0' or (hh.QuanLyTheoLoHang='1' and MaLoHang!='')))
		where
					((select count(Name) from @tblSearchString b where     			
					a1.TenHangHoa like '%'+b.Name+'%'
					or a1.TenHangHoa_KhongDau like '%'+b.Name+'%'
					--or a1.TenHangHoa_KyTuDau like '%'+b.Name+'%'
					or a1.MaHangHoa like '%'+b.Name+'%'		
					or a1.MaLoHang like '%'+b.Name+'%'		
					)=@count or @count=0)	
		and a1.NgayHetHanCompare >= @dtNow
		and (a1.QuanLyTheoLoHang='0' or (a1.QuanLyTheoLoHang='1'  and MaLoHang!=''))
		) b where b.LaHangHoa = 0 or b.TonKho > iif(@Contonkho=1, 0, -99999)
	) a 
	left join
	(			
		select ct.ID_DonViQuiDoi, ct.GiaBan as GiaBan2
		from DM_GiaBan_ChiTiet ct where ct.ID_GiaBan = @ID_BangGia
		) b on a.ID_DonViQuiDoi= b.ID_DonViQuiDoi
	) c");

			CreateStoredProcedure(name: "[dbo].[GetAll_DiscountSale]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				ID_NhanVienLogin = p.String(),
				FromDate = p.DateTime(),
				ToDate = p.DateTime(),
				TextSearch = p.String(),
				Status_DoanhThu = p.String(4),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	set @ToDate = dateadd(day,1, @ToDate) 
	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
    INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    DECLARE @count int =  (Select count(*) from @tblSearchString);

	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDChiNhanhs,'BCCKHoaDon_XemDS_PhongBan','BCCKHoaDon_XemDS_HeThong');
	
	declare @tblSale_NVLapHD table(LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float,IDChiTietCK uniqueidentifier)
	insert into @tblSale_NVLapHD
	select * from dbo.DiscountSale_NVLapHoaDon(@IDChiNhanhs, @FromDate, @ToDate,'%%')

	declare @tblSale_NBanHang table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, ThucThu float, HoaHongDoanhThu float, HoaHongThucThu float,IDChiTietCK uniqueidentifier)
	insert into @tblSale_NBanHang
	select * from dbo.DiscountSale_NVBanHang (@IDChiNhanhs,@FromDate,@ToDate,'%%')

	declare @tblSale_NVDichVu table (LoaiNhanVienApDung int, ID_NhanVien uniqueidentifier , DoanhThu float, HoaHongDoanhThu float,IDChiTietCK uniqueidentifier)
	insert into @tblSale_NVDichVu
	select * from dbo.DiscountSale_NVienDichVu (@IDChiNhanhs,@FromDate,@ToDate,'%%');

	with data_cte
	as (
	select *
	from
	(
	select tblOut.ID_NhanVien,nv.MaNhanVien, nv.TenNhanVien,
		tblOut.DoanhThu, tblOut.ThucThu, 
		tblOut.HoaHongDoanhThu,tblOut.HoaHongThucThu,
		tblOut.TongAll, 
		case when HoaHongDoanhThu > 0 then case when HoaHongThucThu > 0 then '21' else '20' end			
				else case when HoaHongThucThu > 0 then '11' else '10' end end as Status_DoanhThu	
	from
		(select tbl.ID_NhanVien,			
			sum(tbl.DoanhThu) as DoanhThu,
			sum(tbl.ThucThu) as ThucThu,
			sum(tbl.HoaHongDoanhThu) as HoaHongDoanhThu,
			sum(tbl.HoaHongThucThu) as HoaHongThucThu,
			sum(tbl.HoaHongDoanhThu)  + sum(tbl.HoaHongThucThu) as TongAll							
		from
			(select * from @tblSale_NVLapHD 
			union all
			select * from  @tblSale_NBanHang 
			union all
			select LoaiNhanVienApDung, ID_NhanVien, DoanhThu, 0 as ThucThu, HoaHongDoanhThu, 0 as HoaHongThucThu, IDChiTietCK from @tblSale_NVDichVu 
			) tbl 	
			where (exists (select ID from @tblNhanVien nv where tbl.ID_NhanVien = nv.ID))
			group by tbl.ID_NhanVien
		)tblOut
	join NS_NhanVien nv on tblOut.ID_NhanVien= nv.ID
	where
			((select count(Name) from @tblSearchString b where     			
			nv.TenNhanVien like '%'+b.Name+'%'
			or nv.TenNhanVienKhongDau like '%'+b.Name+'%'
			or nv.TenNhanVienChuCaiDau like '%'+b.Name+'%'
			or nv.MaNhanVien like '%'+b.Name+'%'				
			)=@count or @count=0)
			) tblView
	where tblView.Status_DoanhThu like '%'+ @Status_DoanhThu +'%'
	),
	count_cte
	as (
			select count(ID_NhanVien) as TotalRow,
				CEILING(COUNT(ID_NhanVien) / CAST(@PageSize as float ))  as TotalPage,
				sum(DoanhThu) as TongDoanhThu,
				sum(ThucThu) as TongThucThu,
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
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[getlist_SuKienToDay_v2]", parametersAction: p => new
			{
				ID_DonVi = p.Guid(),
				Date = p.DateTime()
			}, body: @"set nocount on;
	DECLARE @SoNgay int;
	DECLARE @DateNow datetime= @Date;
	DECLARE @tblCalendar table (ID uniqueidentifier, ID_DonVi uniqueidentifier, ID_NhanVien uniqueidentifier, NgayGio datetime, PhanLoai int)
	insert into @tblCalendar exec GetListLichHen_FullCalendar_Dashboard @ID_DonVi,'%%',@Date ;
	DECLARE @SinhNhat FLOAT, @CongViec FLOAT, @LichHen FLOAT, @SoLoSapHetHan FLOAT, @SoLoHetHan FLOAT, @XeMoiTiepNhan FLOAT, @XeXuatXuong FLOAT;

	select @SinhNhat = Count(ID) from DM_DoiTuong 
		where TheoDoi != 1 and NgaySinh_NgayTLap is not null
		and DAY(NgaySinh_NgayTLap) = DAY(@DateNow)
		and MONTH(NgaySinh_NgayTLap)= MONTH(@DateNow);

		Select @CongViec = COUNT(CASE WHEN PhanLoai = 4 THEN 1 END),
		@LichHen = COUNT(CASE WHEN PhanLoai = 3 THEN 1 END)
		from @tblCalendar where PhanLoai IN (3, 4);

		Select 
		@SoLoSapHetHan = COUNT(CASE WHEN a.SoNgayConHan < =@SoNgay and a.SoNgayConHan > 0 THEN 1 END),
		@SoLoHetHan = COUNT(CASE WHEN a.SoNgayConHan < 1 THEN 1 END)
		from (
			SELECT DATEDIFF(day,DATEADD(day,-1,@DateNow), lh.NgayHetHan) as SoNgayConHan FROM DM_LoHang lh
			JOIN (Select ID_LoHang, SUM(TonKho) as TonKho from DM_HangHoa_TonKho where ID_DonVi = @ID_DonVi GROUP BY ID_LoHang) tk on lh.ID = tk.ID_LoHang
			where lh.NgayHetHan is not null 
			and tk.TonKho > 0
			) as a;

		SELECT @XeMoiTiepNhan = COUNT(CASE WHEN convert(varchar(10), NgayVaoXuong, 102) 
		= convert(varchar(10), @DateNow, 102) THEN 1 END),
			@XeXuatXuong = COUNT(CASE WHEN convert(varchar(10), NgayXuatXuong, 102) 
		= convert(varchar(10), @DateNow, 102) THEN 1 END)
		FROM Gara_PhieuTiepNhan
		WHERE TrangThai != 0;

		DECLARE @KhachHangMoi FLOAT;
		SELECT @KhachHangMoi = COUNT(ID)
		FROM DM_DoiTuong
		WHERE convert(varchar(10), NgayTao, 102) 
		= convert(varchar(10), @DateNow, 102)
		and TheoDoi= 0

	SELECT @SinhNhat AS SinhNhat, @CongViec AS CongViec, @LichHen AS LichHen, @SoLoSapHetHan AS SoLoSapHetHan, @SoLoHetHan AS SoLoHetHan,
	@XeMoiTiepNhan AS XeMoiTiepNhan, @XeXuatXuong AS XeXuatXuong, @KhachHangMoi AS KhachHangMoi;");

			CreateStoredProcedure(name: "[dbo].[GetListBaoGia_AfterXuLy]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.String(14),
				ToDate = p.String(14),
				TrangThais = p.String(20),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblDonVi table (ID_DonVi uniqueidentifier)
	insert into @tblDonVi
	select Name from dbo.splitstring(@IDChiNhanhs)

	declare @tbTrangThai table (GiaTri varchar(2))
	insert into @tbTrangThai
	select Name from dbo.splitstring(@TrangThais)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);	
	
	with data_cte
	as (

	select 
		c.ID, c.MaHoaDon, c.NgayLapHoaDon,  c.TenDoiTuong, c.PhaiThanhToan,  c.TrangThaiText,
		c.MaPhieuTiepNhan, c.BienSo,
		c.ID_PhieuTiepNhan, c.ID_BangGia, c.TongTienHang, c.TongThanhToan, c.ID_DoiTuong,
		c.TongGiamGia, c.TongTienThue, c.TongChietKhau, c.KhuyeMai_GiamGia, c.YeuCau, c.TongChiPhi
	from
	(

	select 
		ISNULL(xe.BienSo,'') as BienSo, ISNULL(tn.MaPhieuTiepNhan,'') as MaPhieuTiepNhan,
		ISNULL(dt.TenDoiTuong,N'Khách lẻ') as TenDoiTuong,
		ISNULL(dt.TenDoiTuong_KhongDau,N'khach le') as TenDoiTuong_KhongDau,
		ISNULL(dt.MaDoiTuong,N'') as MaDoiTuong,
		ISNULL(dt.DienThoai,N'') as DienThoai,
		b.ID, b.MaHoaDon, b.NgayLapHoaDon, 
		b.ID_DoiTuong, b.ID_BaoHiem, b.PhaiThanhToan, b.PhaiThanhToanBaoHiem, 
		b.ID_PhieuTiepNhan, b.ID_BangGia, b.TongTienHang, b.TongThanhToan,
		b.TongGiamGia, b.TongTienThue, b.TongChietKhau, b.KhuyeMai_GiamGia, b.YeuCau, b.TongChiPhi, 
		case when SoLuong = b.SoLuongConLai then N'Đã duyệt'
		else  N'Đang xử lý' end as TrangThaiText
	from
	(
	select a.ID, a.MaHoaDon, a.NgayLapHoaDon,
		a.ID_DoiTuong, a.ID_BaoHiem, a.PhaiThanhToan, a.PhaiThanhToanBaoHiem, 
		a.ID_PhieuTiepNhan, a.ID_BangGia, a.TongTienHang, a.TongThanhToan, 
		a.TongGiamGia, a.TongTienThue, a.TongChietKhau, a.KhuyeMai_GiamGia, a.YeuCau, a.TongChiPhi, 
		sum(a.SoLuong)  as SoLuong, sum(a.SoLuongConLai) as SoLuongConLai
	from
	(
	select  
		hd.ID, hd.MaHoaDon, hd.NgayLapHoaDon, hd.ID_DoiTuong, hd.ID_BaoHiem, hd.PhaiThanhToan, hd.PhaiThanhToanBaoHiem,
		hd.ID_PhieuTiepNhan, hd.ID_BangGia, hd.TongTienHang, hd.TongThanhToan,
		hd.TongGiamGia, hd.TongTienThue, hd.TongChietKhau, hd.KhuyeMai_GiamGia, hd.YeuCau, hd.TongChiPhi, 
		ctm.SoLuong,
		ctm.SoLuong - ISNULL(ctt.SoLuongTra,0) as SoLuongConLai	
	from BH_HoaDon_ChiTiet ctm
	join BH_HoaDon hd on ctm.ID_HoaDon= hd.ID	
	left join 
	(
		select SUM(ct.SoLuong) as SoLuongTra, ct.ID_ChiTietGoiDV
		from BH_HoaDon_ChiTiet ct 
		join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
		where hd.ChoThanhToan='0' and hd.LoaiHoaDon in (1,25)
		and (ct.ID_ChiTietDinhLuong is null or ct.ID_ChiTietDinhLuong = ct.ID)
		group by ct.ID_ChiTietGoiDV
	) ctt on ctm.ID = ctt.ID_ChiTietGoiDV
	where hd.LoaiHoaDon = 3 and hd.YeuCau !=3 --- yeucau= 3 (capnhat thucong trangthai = hoanthanh mac du chua xuly het)
	and exists (select ID_DonVi from @tblDonVi dv where hd.ID_DonVi = dv.ID_DonVi)
	and hd.ChoThanhToan= '0' 
	and hd.NgayLapHoaDon >= @FromDate and hd.NgayLapHoaDon <= @ToDate
	and (ctm.ID_ChiTietDinhLuong is null or ctm.ID_ChiTietDinhLuong = ctm.ID)	
	) a where a.SoLuongConLai > 0 
	group by a.ID, a.MaHoaDon, a.NgayLapHoaDon, 
		a.ID_DoiTuong, a.ID_BaoHiem, a.PhaiThanhToan, a.PhaiThanhToanBaoHiem, a.ID_PhieuTiepNhan, a.ID_BangGia,
		a.TongTienHang, a.TongThanhToan, a.TongGiamGia, a.TongTienThue, a.TongChietKhau, a.KhuyeMai_GiamGia, a.YeuCau, a.TongChiPhi
	) b
	left join DM_DoiTuong dt on b.ID_DoiTuong = dt.ID
	left join Gara_PhieuTiepNhan tn on b.ID_PhieuTiepNhan = tn.ID
	left join Gara_DanhMucXe xe on tn.ID_Xe = xe.ID
	) c
	where 	
				((select count(Name) from @tblSearch tbls where     			
				c.MaHoaDon like '%'+tbls.Name+'%'
				or c.MaPhieuTiepNhan like '%'+tbls.Name+'%'
				or c.BienSo like '%'+tbls.Name+'%'
				or c.MaPhieuTiepNhan like '%'+tbls.Name+'%'
				or c.MaDoiTuong like '%'+tbls.Name+'%'
				or c.TenDoiTuong like '%'+tbls.Name+'%'	
				or c.DienThoai like '%'+tbls.Name+'%'				
				or c.TenDoiTuong_KhongDau like '%'+tbls.Name+'%'														
				)=@count or @count=0)
	),
	count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage			
			from data_cte
		)
		select dt.*, cte.*, 0 as KhachDaTra
		from data_cte dt
		cross join count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetListBaoHiem_v1]", parametersAction: p => new
			{
				IdChiNhanhs = p.String(),
				NgayTaoFrom = p.DateTime(),
				NgayTaoTo = p.DateTime(),
				TongBanDateFrom = p.DateTime(),
				TongBanDateTo = p.DateTime(),
				TongBanFrom = p.Double(),
				TongBanTo = p.Double(),
				NoFrom = p.Double(),
				NoTo = p.Double(),
				TrangThais = p.String(20),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblDonVi table (ID_DonVi  uniqueidentifier);
	if(@IdChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IdChiNhanhs);
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	declare @tbTrangThai table (GiaTri varchar(2));
	if(@TrangThais != '')
	BEGIN
		insert into @tbTrangThai
		select Name from dbo.splitstring(@TrangThais);
	END
	DECLARE @tblResult TABLE(ID UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), DienThoai NVARCHAR(MAX), MaSoThue NVARCHAR(MAX), Email NVARCHAR(MAX), DiaChi NVARCHAR(MAX), ID_TinhThanh UNIQUEIDENTIFIER, 
	TenTinhThanh NVARCHAR(MAX), ID_QuanHuyen UNIQUEIDENTIFIER, TenQuanHuyen NVARCHAR(MAX),
	GhiChu NVARCHAR(MAX), ID_DonVi UNIQUEIDENTIFIER, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), NgayTao DATETIME, LoaiDoiTuong INT, NguoiTao NVARCHAR(MAX), NoHienTai FLOAT, TongTienBaoHiem FLOAT, TotalRow INT, TotalPage FLOAT);


	DECLARE @tblDoiTuong TABLE(ID UNIQUEIDENTIFIER, MaDoiTuong NVARCHAR(MAX), TenDoiTuong NVARCHAR(MAX), DienThoai NVARCHAR(MAX), MaSoThue NVARCHAR(MAX), Email NVARCHAR(MAX), DiaChi NVARCHAR(MAX), ID_TinhThanh UNIQUEIDENTIFIER, 
	TenTinhThanh NVARCHAR(MAX), ID_QuanHuyen UNIQUEIDENTIFIER, TenQuanHuyen NVARCHAR(MAX),
	GhiChu NVARCHAR(MAX), ID_DonVi UNIQUEIDENTIFIER, MaDonVi NVARCHAR(MAX), TenDonVi NVARCHAR(MAX), NgayTao DATETIME, LoaiDoiTuong INT, NguoiTao NVARCHAR(MAX));
	DECLARE @tblBaoHiemPhaiThanhToan TABLE(ID UNIQUEIDENTIFIER, PhaiThanhToan FLOAT);
	DECLARE @tblBaoHiemDaThanhToan TABLE(ID UNIQUEIDENTIFIER, DaThanhToan FLOAT);
	DECLARE @TotalRow INT;
	DECLARE @TotalPage FLOAT;

	IF (@TongBanFrom IS NULL AND @TongBanTo IS NULL AND @NoFrom IS NULL AND @NoTo IS NULL)
	BEGIN
		
		INSERT INTO @tblDoiTuong
		SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
		dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
		LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
		LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
		inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
		inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
		INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
		WHERE 
			dt.LoaiDoiTuong  = 3
			AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
			AND ((select count(Name) from @tblSearch b where     			
			dt.GhiChu like '%'+b.Name+'%'
			or dt.MaDoiTuong like '%'+b.Name+'%'		
			or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.DienThoai like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
			or dt.MaSoThue like '%'+b.Name+'%'
			or dt.DiaChi like '%'+b.Name+'%'
			or dt.Email like '%'+b.Name+'%'
			or tt.TenTinhThanh like '%'+b.Name+'%'
			or qh.TenQuanHuyen like '%'+b.Name+'%'
			or dv.MaDonVi like '%'+b.Name+'%'
			or dv.TenDonVi like '%'+b.Name+'%'
			)=@count or @count=0);

			--SELECT * FROM @tblDoiTuong;

			INSERT INTO @tblBaoHiemPhaiThanhToan
			select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
			INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
						OFFSET (@CurrentPage * @PageSize) ROWS
						FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = hd.ID_BaoHiem 
			inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
			where hd.LoaiHoaDon = 25
			AND ID_BaoHiem IS NOT NULL
			AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
			GROUP BY ID_BaoHiem
			--HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
			--AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanTo)
		
			INSERT INTO @tblBaoHiemDaThanhToan
			SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
			INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
			INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
						OFFSET (@CurrentPage * @PageSize) ROWS
						FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = qhdct.ID_DoiTuong
			inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
			WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
			GROUP BY qhdct.ID_DoiTuong
			--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
			--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
			SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
			INSERT INTO @tblResult
			SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
			LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
			LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
			ORDER BY dt.NgayTao desc
			OFFSET (@CurrentPage * @PageSize) ROWS
			FETCH NEXT @PageSize ROWS ONLY;
	END
	ELSE
	BEGIN
		IF(@NoFrom IS NULL AND @NoTo IS NULL)
		BEGIN
			IF(@TongBanFrom = 0 OR @TongBanTo = 0 OR @TongBanFrom IS NULL)
			BEGIN
				INSERT INTO @tblBaoHiemPhaiThanhToan
				select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
				inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
				where hd.LoaiHoaDon = 25
				AND ID_BaoHiem IS NOT NULL
				AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
				GROUP BY ID_BaoHiem
				HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
				AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) <= @TongBanTo)

				INSERT INTO @tblDoiTuong
				SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
				dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
				LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
				LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
				inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
				inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
				INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
				LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
				WHERE 
					dt.LoaiDoiTuong  = 3 AND (ISNULL(ptt.PhaiThanhToan, 0) >= @TongBanFrom OR @TongBanFrom IS NULL) AND (ISNULL(ptt.PhaiThanhToan, 0) <= @TongBanTo OR @TongBanTo IS NULL)
					AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
					AND ((select count(Name) from @tblSearch b where     			
					dt.GhiChu like '%'+b.Name+'%'
					or dt.MaDoiTuong like '%'+b.Name+'%'		
					or dt.TenDoiTuong like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
					or dt.MaSoThue like '%'+b.Name+'%'
					or dt.DiaChi like '%'+b.Name+'%'
					or dt.Email like '%'+b.Name+'%'
					or tt.TenTinhThanh like '%'+b.Name+'%'
					or qh.TenQuanHuyen like '%'+b.Name+'%'
					or dv.MaDonVi like '%'+b.Name+'%'
					or dv.TenDonVi like '%'+b.Name+'%'
					)=@count or @count=0);

				INSERT INTO @tblBaoHiemDaThanhToan
				SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
				INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
				INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
							OFFSET (@CurrentPage * @PageSize) ROWS
							FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = qhdct.ID_DoiTuong
				inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
				WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
				GROUP BY qhdct.ID_DoiTuong
				--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
				--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
				SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
				INSERT INTO @tblResult
				SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
				LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
				LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
				ORDER BY dt.NgayTao desc
				OFFSET (@CurrentPage * @PageSize) ROWS
				FETCH NEXT @PageSize ROWS ONLY;
			END
			ELSE
			BEGIN
				INSERT INTO @tblBaoHiemPhaiThanhToan
				select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
				inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
				where hd.LoaiHoaDon = 25
				AND ID_BaoHiem IS NOT NULL
				AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
				GROUP BY ID_BaoHiem
				HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
				AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) <= @TongBanTo)

				INSERT INTO @tblDoiTuong
				SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
				dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
				LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
				LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
				inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
				inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
				INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
				INNER JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
				WHERE 
					dt.LoaiDoiTuong  = 3
					AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
					AND ((select count(Name) from @tblSearch b where     			
					dt.GhiChu like '%'+b.Name+'%'
					or dt.MaDoiTuong like '%'+b.Name+'%'		
					or dt.TenDoiTuong like '%'+b.Name+'%'
					or dt.DienThoai like '%'+b.Name+'%'
					or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
					or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
					or dt.MaSoThue like '%'+b.Name+'%'
					or dt.DiaChi like '%'+b.Name+'%'
					or dt.Email like '%'+b.Name+'%'
					or tt.TenTinhThanh like '%'+b.Name+'%'
					or qh.TenQuanHuyen like '%'+b.Name+'%'
					or dv.MaDonVi like '%'+b.Name+'%'
					or dv.TenDonVi like '%'+b.Name+'%'
					)=@count or @count=0);

					INSERT INTO @tblBaoHiemDaThanhToan
					SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
					INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
					INNER JOIN (SELECT * FROM @tblDoiTuong dtt ORDER BY dtt.NgayTao desc
								OFFSET (@CurrentPage * @PageSize) ROWS
								FETCH NEXT @PageSize ROWS ONLY) dt ON dt.ID = qhdct.ID_DoiTuong
					inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
					WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
					GROUP BY qhdct.ID_DoiTuong
					--HAVING (@TongBanFrom IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanFrom)
					--AND (@TongBanTo IS NULL OR SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) >= @TongBanTo)
					SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
					INSERT INTO @tblResult
					SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
					LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
					LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
					ORDER BY dt.NgayTao desc
					OFFSET (@CurrentPage * @PageSize) ROWS
					FETCH NEXT @PageSize ROWS ONLY;
				END
		END
		ELSE
		BEGIN
			INSERT INTO @tblBaoHiemPhaiThanhToan
			select hd.ID_BaoHiem, SUM(hd.PhaiThanhToanBaoHiem) AS TongBaoHiem from BH_HoaDon hd
			inner join @tblDonVi donvi on donvi.ID_DonVi = hd.ID_DonVi
			where hd.LoaiHoaDon = 25
			AND ID_BaoHiem IS NOT NULL
			AND (@TongBanDateFrom IS NULL OR hd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
			GROUP BY ID_BaoHiem
			HAVING (@TongBanFrom IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) >= @TongBanFrom)
			AND (@TongBanTo IS NULL OR SUM(hd.PhaiThanhToanBaoHiem) <= @TongBanTo);

			INSERT INTO @tblBaoHiemDaThanhToan
			SELECT qhdct.ID_DoiTuong, SUM(CASE WHEN qhd.LoaiHoaDon = 11 THEN qhdct.TienThu ELSE -1 * qhdct.TienThu END) AS DaThanhToan FROM Quy_HoaDon qhd
			INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
			inner join @tblDonVi donvi on donvi.ID_DonVi = qhd.ID_DonVi
			WHERE (@TongBanDateFrom IS NULL OR qhd.NgayLapHoaDon BETWEEN @TongBanDateFrom AND @TongBanDateTo)
			GROUP BY qhdct.ID_DoiTuong;

			INSERT INTO @tblDoiTuong
			SELECT dt.ID, dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai, dt.MaSoThue, dt.Email, dt.DiaChi, dt.ID_TinhThanh, tt.TenTinhThanh, dt.ID_QuanHuyen, qh.TenQuanHuyen,
			dt.GhiChu, dt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, dt.NgayTao, dt.LoaiDoiTuong, dt.NguoiTao FROM DM_DoiTuong dt
			LEFT JOIN DM_TinhThanh tt ON dt.ID_TinhThanh = tt.ID
			LEFT JOIN DM_QuanHuyen qh ON dt.ID_QuanHuyen = qh.ID
			inner join DM_DonVi dv on dv.ID = dt.ID_DonVi
			inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
			INNER JOIN @tbTrangThai tth ON dt.TheoDoi = tth.GiaTri
			LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
			LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
			WHERE 
				dt.LoaiDoiTuong  = 3
				AND (ISNULL(ptt.PhaiThanhToan, 0) >= @TongBanFrom OR @TongBanFrom IS NULL) AND (ISNULL(ptt.PhaiThanhToan, 0) <= @TongBanTo OR @TongBanTo IS NULL)
				AND ((ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0)) >= @NoFrom OR @NoFrom IS NULL) AND ((ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0)) <= @NoTo OR @NoTo IS NULL)
				AND (@NgayTaoFrom IS NULL OR dt.NgayTao BETWEEN @NgayTaoFrom AND @NgayTaoTo)
				AND ((select count(Name) from @tblSearch b where     			
				dt.GhiChu like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'		
				or dt.TenDoiTuong like '%'+b.Name+'%'
				or dt.DienThoai like '%'+b.Name+'%'
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
				or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
				or dt.MaSoThue like '%'+b.Name+'%'
				or dt.DiaChi like '%'+b.Name+'%'
				or dt.Email like '%'+b.Name+'%'
				or tt.TenTinhThanh like '%'+b.Name+'%'
				or qh.TenQuanHuyen like '%'+b.Name+'%'
				or dv.MaDonVi like '%'+b.Name+'%'
				or dv.TenDonVi like '%'+b.Name+'%'
				)=@count or @count=0);

				SELECT @TotalRow = COUNT(ID), @TotalPage = CEILING(COUNT(ID) / CAST(@PageSize as float )) FROM @tblDoiTuong;
				INSERT INTO @tblResult
				SELECT dt.*, ISNULL(ptt.PhaiThanhToan, 0) - ISNULL(dtt.DaThanhToan, 0) AS NoHienTai, ISNULL(ptt.PhaiThanhToan, 0) AS TongTienBaoHiem, @TotalRow AS TotalRow, @TotalPage AS TotalPage FROM @tblDoiTuong dt
				LEFT JOIN @tblBaoHiemPhaiThanhToan ptt ON ptt.ID = dt.ID
				LEFT JOIN @tblBaoHiemDaThanhToan dtt ON dtt.ID = dt.ID
				ORDER BY dt.NgayTao desc
				OFFSET (@CurrentPage * @PageSize) ROWS
				FETCH NEXT @PageSize ROWS ONLY;
		END
	END
		
		SELECT ID, MaDoiTuong, TenDoiTuong , DienThoai , MaSoThue , Email , DiaChi , ID_TinhThanh , 
	TenTinhThanh , ID_QuanHuyen , TenQuanHuyen ,
	GhiChu , ID_DonVi , MaDonVi , TenDonVi , NgayTao , LoaiDoiTuong , NguoiTao , ROUND(NoHienTai, 0) AS NoHienTai , ROUND(TongTienBaoHiem,0) AS TongTienBaoHiem, TotalRow , TotalPage  FROM @tblResult;");

			CreateStoredProcedure(name: "[dbo].[GetListCashFlow_Paging]", parametersAction: p => new
			{
				IDDonVis = p.String(),
				ID_NhanVien = p.String(40),
				ID_NhanVienLogin = p.Guid(),
				ID_TaiKhoanNganHang = p.String(40),
				ID_KhoanThuChi = p.String(40),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime(),
				LoaiSoQuy = p.String(15),
				LoaiChungTu = p.String(2),
				TrangThaiSoQuy = p.String(2),
				TrangThaiHachToan = p.String(2),
				TxtSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @nguoitao nvarchar(100) = (select taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDDonVis,'SoQuy_XemDS_PhongBan','SoQuy_XemDS_HeThong');
	
	with data_cte
	as(
    select tblView.*
	from
		(select 
			tblQuy.ID_HoaDon as ID,
			tblQuy.MaHoaDon,
			tblQuy.NgayLapHoaDon,
			tblQuy.ID_DonVi,
			tblQuy.LoaiHoaDon,
			tblQuy.NguoiTao,
			ISNUll(tblQuy.TrangThai,'1') as TrangThai,
			tblQuy.NoiDungThu,
			tblQuy.PhieuDieuChinhCongNo,
			tblQuy.ID_NhanVienPT as ID_NhanVien,
			iif(LoaiHoaDon=11, TienMat,0) as ThuMat,
			iif(LoaiHoaDon=12, TienMat,0) as ChiMat,
			iif(LoaiHoaDon=11, TienGui,0) as ThuGui,
			iif(LoaiHoaDon=12, TienGui,0) as ChiGui,
			TienMat, TienGui, TienMat + TienGui as TienThu,
			TienMat + TienGui as TongTienThu,
			TenTaiKhoanPOS, TenTaiKhoanNOTPOS,
			cast(ID_TaiKhoanNganHang as varchar(max)) as ID_TaiKhoanNganHang,
			ID_KhoanThuChi,
			NoiDungThuChi,
			isnull(nguon.TenNguonKhach,'') as TenNguonKhach,
			ISNULL(tblQuy.HachToanKinhDoanh,'1') as HachToanKinhDoanh,
			case when tblQuy.LoaiHoaDon = 11 then '11' else '12' end as LoaiChungTu,
    		case when tblQuy.HachToanKinhDoanh = '1' or tblQuy.HachToanKinhDoanh is null  then '11' else '10' end as TrangThaiHachToan,
    		case when tblQuy.TrangThai = '0' then '10' else '11' end as TrangThaiSoQuy,
			case when nv.TenNhanVien is null then  dt.TenDoiTuong  else nv.TenNhanVien end as NguoiNopTien,
    		case when nv.TenNhanVien is null then  dt.TenDoiTuong_KhongDau  else nv.TenNhanVienKhongDau end as TenDoiTuong_KhongDau,
    		case when nv.MaNhanVien is null then dt.MaDoiTuong else  nv.MaNhanVien end as MaDoiTuong,
    		case when nv.MaNhanVien is null then dt.DienThoai else  nv.DienThoaiDiDong  end as SoDienThoai,
			ISNULL(nv2.TenNhanVien,'') as TenNhanVien,
			ID_TaiKhoanNganHang as ddd,
			case when tblQuy.TienMat > 0 then case when tblQuy.TienGui > 0 then '2' else '1' end 
			else case when tblQuy.TienGui > 0 then '0'
				else case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then '0' else '1' end end end as LoaiSoQuy,
			-- check truong hop tongthu = 0
    		case when tblQuy.TienMat > 0 then case when tblQuy.TienGui > 0 then N'Tiền mặt, chuyển khoản' else N'Tiền mặt' end 
			else case when tblQuy.TienGui > 0 then N'Chuyển khoản' else 
				case when ID_TaiKhoanNganHang!='00000000-0000-0000-0000-000000000000' then  N'Chuyển khoản' else N'Tiền mặt' end end end as PhuongThuc	
							
		from
			(select 
				 a.ID_hoaDon, a.MaHoaDon, a.NguoiTao,
				 a.NgayLapHoaDon, a.ID_DonVi, a.LoaiHoaDon,
				 a.HachToanKinhDoanh, a.PhieuDieuChinhCongNo, a.NoiDungThu,
				 a.ID_NhanVienPT, a.TrangThai,
				 sum(isnull(a.TienMat, 0)) as TienMat,
				 sum(isnull(a.TienGui, 0)) as TienGui,
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
					select qhd.MaHoaDon, qhd.NgayLapHoaDon, qhd.ID_DonVi, qhd.LoaiHoaDon, qhd.NguoiTao,
					qhd.HachToanKinhDoanh, qhd.PhieuDieuChinhCongNo, qhd.NoiDungThu,
					qhd.ID_NhanVien as ID_NhanVienPT, qhd.TrangThai,
					qct.ID_HoaDon, qct.TienMat,qct.TienGui, qct.ID_DoiTuong, qct.ID_NhanVien, 
					ISNULL(qct.ID_TaiKhoanNganHang,'00000000-0000-0000-0000-000000000000') as ID_TaiKhoanNganHang,
					ISNULL(qct.ID_KhoanThuChi,'00000000-0000-0000-0000-000000000000') as ID_KhoanThuChi,
					case when tk.TaiKhoanPOS='1' then tk.TenChuThe else '' end as TenTaiKhoanPOS,
					case when tk.TaiKhoanPOS='0' then tk.TenChuThe else '' end as TenTaiKhoanNOPOS,
					ISNULL(ktc.NoiDungThuChi,'') as NoiDungThuChi
					from Quy_HoaDon_ChiTiet qct
					join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
					left join DM_TaiKhoanNganHang tk on qct.ID_TaiKhoanNganHang= tk.ID
					left join Quy_KhoanThuChi ktc on qct.ID_KhoanThuChi= ktc.ID
					where qhd.NgayLapHoaDon >= @DateFrom and qhd.NgayLapHoaDon < @DateTo
					and (qct.ThuTuThe is null OR qct.ThuTuThe = 0) and (qct.DiemThanhToan is null OR qct.DiemThanhToan = 0)
					)qct
				where qct.ID_TaiKhoanNganHang like @ID_TaiKhoanNganHang
				and qct.ID_KhoanThuChi like @ID_KhoanThuChi
			) a
			 group by a.ID_HoaDon, a.MaHoaDon, a.NgayLapHoaDon, a.ID_DonVi, a.LoaiHoaDon,
				a.HachToanKinhDoanh, a.PhieuDieuChinhCongNo, a.NoiDungThu,
				a.ID_NhanVienPT , a.TrangThai,a.NguoiTao
		) tblQuy
		left join DM_DoiTuong dt on tblQuy.ID_DoiTuong = dt.ID
		left join NS_NhanVien nv on tblQuy.ID_NhanVien= nv.ID
		left join NS_NhanVien nv2 on tblQuy.ID_NhanVienPT= nv2.ID
		left join DM_NguonKhachHang nguon on dt.ID_NguonKhach = nguon.ID
	 ) tblView
	 where tblView.TrangThaiHachToan like '%'+ @TrangThaiHachToan + '%'
		and tblView.ID_DonVi in (select * from dbo.splitstring(@IDDonVis))	
    	and tblView.TrangThaiSoQuy like '%'+ @TrangThaiSoQuy + '%'
    	and tblView.LoaiChungTu like '%'+ @LoaiChungTu + '%'
    	and (tblView.PhieuDieuChinhCongNo ='0' or PhieuDieuChinhCongNo is null)
		and tblView.ID_NhanVien like @ID_NhanVien
		and (exists (select ID from @tblNhanVien nv where tblView.ID_NhanVien = nv.ID) or tblView.NguoiTao like @nguoitao)
    	and exists (select Name from dbo.splitstring(@LoaiSoQuy) where LoaiSoQuy= Name)
		and (MaHoaDon like @TxtSearch OR MaDoiTuong like @TxtSearch OR NguoiNopTien like @TxtSearch
		OR TenDoiTuong_KhongDau like @TxtSearch OR dbo.FUNC_ConvertStringToUnsign(NoiDungThu) like @TxtSearch)
	),
	count_cte
	as(
	select count(dt.ID) as TotalRow,
		CEILING( count(dt.ID) / cast (@PageSize as float)) as TotalPage,
		max(dv.TenDonVi) as TenChiNhanh,	
		sum(ThuMat) as TongThuMat,
		sum(ChiMat) as TongChiMat,
		sum(ThuGui) as TongThuCK,
		sum(ChiGui) as TongChiCK
	from data_cte dt
	join DM_DonVi dv on dt.ID_DonVi= dv.ID
	)
	select *
	from data_cte dt
	cross join count_cte
	order by dt.NgayLapHoaDon desc
	OFFSET (@CurrentPage* @PageSize) ROWS
	FETCH NEXT @PageSize ROWS ONLY");

			CreateStoredProcedure(name: "[dbo].[GetListHoaDonSuaChua]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				FromDate = p.String(14),
				ToDate = p.String(14),
				ID_PhieuSuaChua = p.String(),
				IDXe = p.Guid(),
				TrangThais = p.String(20),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;
	if @FromDate = '2016-01-01' 
		set @ToDate= (select format(DATEADD(day,1, max(NgayLapHoaDon)),'yyyy-MM-dd') from BH_HoaDon where LoaiHoaDon= 25)

	declare @tblDonVi table (ID_DonVi uniqueidentifier)
	if(@IDChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IDChiNhanhs)
	END
	ELSE
	BEGIN
		INSERT INTO @tblDonVi
		SELECT ID FROM DM_DonVi;
	END

	declare @tbTrangThai table (GiaTri varchar(2))
	insert into @tbTrangThai
	select Name from dbo.splitstring(@TrangThais)

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);	
	if(@PageSize != 0)
	BEGIN
	with data_cte
	as
	(
			select *
			from
			(
			select hd.*,
				tn.MaPhieuTiepNhan,
				dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai as DienThoaiKhachHang, dt.Email, dt.DiaChi,
				ISNULL(bg.MaHoaDon,'') as MaBaoGia,
				case hd.ChoThanhToan
					when 0 then '0'
					when 1 then '1'
					else '2' end as TrangThai,
				case hd.ChoThanhToan
					when 0 then N'Hoàn thành'
					when 1 then N'Phiếu tạm'
					else N'Đã hủy'
					end as TrangThaiText
			from BH_HoaDon hd
			join Gara_PhieuTiepNhan tn on tn.ID= hd.ID_PhieuTiepNhan
			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
			left join BH_HoaDon bg on hd.ID_HoaDon= bg.ID and bg.ID_PhieuTiepNhan= tn.ID
			where hd.LoaiHoaDon= 25
			and exists (select ID_DonVi from @tblDonVi dv where hd.ID_DonVi = dv.ID_DonVi)
			and hd.NgayLapHoaDon >=@FromDate and hd.NgayLapHoaDon < @ToDate
			and hd.ID_PhieuTiepNhan like @ID_PhieuSuaChua
			and (@IDXe IS NULL OR @IDXe = tn.ID_Xe)
			and
				((select count(Name) from @tblSearch b where     			
				hd.MaHoaDon like '%'+b.Name+'%'
				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong like '%'+b.Name+'%'	
				or dt.DienThoai like '%'+b.Name+'%'				
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
				or hd.NguoiTao like '%'+b.Name+'%'										
				)=@count or @count=0)	
			) a
			where exists (select GiaTri from @tbTrangThai tt where a.TrangThai = tt.GiaTri)
	),
	count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
			from data_cte
		)

		select dt.*, cte.*, 
			dt.NguoiTao as NguoiTaoHD,
			nv.TenNhanVien,
			isnull(KhachDaTra,0) as KhachDaTra,
			isnull(BaoHiemDaTra,0) as BaoHiemDaTra,
			isnull(bh.TenDoiTuong,'') as TenBaoHiem,
			isnull(bh.Email,'') as BH_Email,
				isnull(bh.DiaChi,'') as BH_DiaChi,
			isnull(bh.DienThoai,'') as DienThoaiBaoHiem
		from data_cte dt
		join NS_NhanVien nv on dt.ID_NhanVien= nv.ID
		cross join count_cte cte
		left join DM_DoiTuong bh on bh.ID= dt.ID_BaoHiem
		left join
		(
		select a.ID, sum(KhachDaTra) as KhachDaTra, sum(BaoHiemDaTra) as BaoHiemDaTra
		from
		(
			select cte.ID, sum(qct.TienThu) as KhachDaTra, 0 as BaoHiemDaTra
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
			join BH_HoaDon cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_DoiTuong
			where qhd.TrangThai= 1
			group by cte.ID

			union all

			select cte.ID, 0 as KhachDaTra, sum(qct.TienThu) as BaoHiemDaTra
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
			join data_cte cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_BaoHiem
			where qhd.TrangThai= 1
			group by cte.ID
		) a
		group by a.ID
		) quy on dt.ID= quy.ID
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
	END
	ELSE
	BEGIN
	with data_cte
	as
	(
			select *
			from
			(
			select hd.*,
				tn.MaPhieuTiepNhan,
				dt.MaDoiTuong, dt.TenDoiTuong, dt.DienThoai as DienThoaiKhachHang, dt.Email, dt.DiaChi,
				ISNULL(bg.MaHoaDon,'') as MaBaoGia,
				case hd.ChoThanhToan
					when 0 then '0'
					when 1 then '1'
					else '2' end as TrangThai,
				case hd.ChoThanhToan
					when 0 then N'Hoàn thành'
					when 1 then N'Phiếu tạm'
					else N'Đã hủy'
					end as TrangThaiText
			from BH_HoaDon hd
			join Gara_PhieuTiepNhan tn on tn.ID= hd.ID_PhieuTiepNhan
			left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
			left join BH_HoaDon bg on hd.ID_HoaDon= bg.ID and bg.ID_PhieuTiepNhan= tn.ID
			where hd.LoaiHoaDon= 25
			and exists (select ID_DonVi from @tblDonVi dv where hd.ID_DonVi = dv.ID_DonVi)
			and hd.NgayLapHoaDon >=@FromDate and hd.NgayLapHoaDon < @ToDate
			and hd.ID_PhieuTiepNhan like @ID_PhieuSuaChua
			and (@IDXe IS NULL OR @IDXe = tn.ID_Xe)
			and
				((select count(Name) from @tblSearch b where     			
				hd.MaHoaDon like '%'+b.Name+'%'
				or tn.MaPhieuTiepNhan like '%'+b.Name+'%'
				or dt.MaDoiTuong like '%'+b.Name+'%'
				or dt.TenDoiTuong like '%'+b.Name+'%'	
				or dt.DienThoai like '%'+b.Name+'%'				
				or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'			
				or hd.NguoiTao like '%'+b.Name+'%'										
				)=@count or @count=0)	
			) a
			where exists (select GiaTri from @tbTrangThai tt where a.TrangThai = tt.GiaTri)
	)

		select dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage,
			dt.NguoiTao as NguoiTaoHD,
			nv.TenNhanVien,
			isnull(KhachDaTra,0) as KhachDaTra,
			isnull(BaoHiemDaTra,0) as BaoHiemDaTra,
			isnull(bh.TenDoiTuong,'') as TenBaoHiem,
			isnull(bh.Email,'') as BH_Email,
				isnull(bh.DiaChi,'') as BH_DiaChi,
			isnull(bh.DienThoai,'') as DienThoaiBaoHiem
		from data_cte dt
		join NS_NhanVien nv on dt.ID_NhanVien= nv.ID
		
		left join DM_DoiTuong bh on bh.ID= dt.ID_BaoHiem
		left join
		(
		select a.ID, sum(KhachDaTra) as KhachDaTra, sum(BaoHiemDaTra) as BaoHiemDaTra
		from
		(
			select cte.ID, sum(qct.TienThu) as KhachDaTra, 0 as BaoHiemDaTra
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
			join BH_HoaDon cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_DoiTuong
			where qhd.TrangThai= 1
			group by cte.ID

			union all

			select cte.ID, 0 as KhachDaTra, sum(qct.TienThu) as BaoHiemDaTra
			from Quy_HoaDon_ChiTiet qct
			join Quy_HoaDon qhd on qct.ID_HoaDon= qhd.ID
			join data_cte cte on qct.ID_HoaDonLienQuan= cte.ID and qct.ID_DoiTuong= cte.ID_BaoHiem
			where qhd.TrangThai= 1
			group by cte.ID
		) a
		group by a.ID
		) quy on dt.ID= quy.ID
		order by dt.NgayLapHoaDon desc
		
	END");

			CreateStoredProcedure(name: "[dbo].[GetListPhieuNhapXuatKhoByIDPhieuTiepNhan]", parametersAction: p => new
			{
				IDPhieuTiepNhan = p.Guid(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF(@PageSize != 0)
	BEGIN
		with data_cte
		as
		(select pxk.ID, pxk.LoaiHoaDon, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.ID AS ID_HoaDonSuaChua, hdsc.MaHoaDon AS HoaDonSuaChua, hdsc.ChoThanhToan AS TrangThaiHoaDonSuaChua, SUM(pxkct.SoLuong) AS SoLuong, SUM(pxkct.ThanhTien) AS GiaTri from BH_HoaDon pxk
		INNER JOIN BH_HoaDon hdsc ON pxk.ID_HoaDon = hdsc.ID
		INNER JOIN BH_HoaDon_ChiTiet pxkct ON pxk.ID = pxkct.ID_HoaDon
		where pxk.ID_PhieuTiepNhan = @IDPhieuTiepNhan
		AND pxk.LoaiHoaDon = '8' AND pxk.ChoThanhToan = 0
		GROUP BY pxk.ID, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.MaHoaDon, pxk.LoaiHoaDon, hdsc.ID, hdsc.ChoThanhToan),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
			from data_cte
		)

		SELECT * FROM data_cte dt
		CROSS JOIN count_cte cte
		order by dt.NgayLapHoaDon desc
		OFFSET (@CurrentPage* @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY
	END
	ELSE
	BEGIN
		with data_cte
		as
		(select pxk.ID, pxk.LoaiHoaDon, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.ID AS ID_HoaDonSuaChua, hdsc.MaHoaDon AS HoaDonSuaChua, hdsc.ChoThanhToan AS TrangThaiHoaDonSuaChua, SUM(pxkct.SoLuong) AS SoLuong, SUM(pxkct.ThanhTien) AS GiaTri from BH_HoaDon pxk
		INNER JOIN BH_HoaDon hdsc ON pxk.ID_HoaDon = hdsc.ID
		INNER JOIN BH_HoaDon_ChiTiet pxkct ON pxk.ID = pxkct.ID_HoaDon
		where pxk.ID_PhieuTiepNhan = @IDPhieuTiepNhan
		AND pxk.LoaiHoaDon = '8' AND pxk.ChoThanhToan = 0
		GROUP BY pxk.ID, pxk.MaHoaDon, pxk.NgayLapHoaDon, hdsc.MaHoaDon, pxk.LoaiHoaDon, hdsc.ID, hdsc.ChoThanhToan),
		count_cte
		as (
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
			from data_cte
		)

		SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
		order by dt.NgayLapHoaDon desc
	END");

			CreateStoredProcedure(name: "[dbo].[GetListPhieuTiepNhan_v2]", parametersAction: p => new
			{
				IdChiNhanhs = p.String(),
				NgayTiepNhan_From = p.DateTime(),
				NgayTiepNhan_To = p.DateTime(),
				NgayXuatXuongDuKien_From = p.DateTime(),
				NgayXuatXuongDuKien_To = p.DateTime(),
				NgayXuatXuong_From = p.DateTime(),
				NgayXuatXuong_To = p.DateTime(),
				TrangThais = p.String(20),
				TextSearch = p.String(),
				CurrentPage = p.Int(),
				PageSize = p.Int()
			}, body: @"SET NOCOUNT ON;

	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
	if(@IdChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IdChiNhanhs);
	END

	DECLARE @tblSearch TABLE (Name [nvarchar](max));
    DECLARE @count int;
    INSERT INTO @tblSearch(Name) select  Name from [dbo].[splitstringByChar](@TextSearch, ' ') where Name!='';
    Select @count =  (Select count(*) from @tblSearch);

	declare @tbTrangThai table (GiaTri varchar(2))
	insert into @tbTrangThai
	select Name from dbo.splitstring(@TrangThais);
	if(@PageSize != 0)
	BEGIN
		with data_cte
	as
	(
	select ptn.ID, ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, ptn.SoKmVao, ptn.NgayXuatXuongDuKien, ptn.NgayXuatXuong, ptn.TrangThai,
	ptn.SoKmRa, ptn.TenLienHe, ptn.SoDienThoaiLienHe, ptn.GhiChu, ptn.TrangThai AS TrangThaiPhieuTiepNhan,
	ptn.ID_Xe, dmx.BienSo, dmx.SoMay, dmx.SoKhung, dmx.NamSanXuat, mauxe.TenMauXe, hangxe.TenHangXe, loaixe.TenLoaiXe,
	ptn.ID_KhachHang, dt.MaDoiTuong, dt.TenDoiTuong, dt.Email, dt.DienThoai AS DienThoaiKhachHang, dt.DiaChi,
	ptn.ID_CoVanDichVu, ISNULL(nvcovan.TenNhanVien, '') AS CoVanDichVu, ISNULL(nvcovan.MaNhanVien, '') AS MaCoVan,
	ptn.ID_NhanVien, nvtiepnhan.MaNhanVien AS MaNhanVienTiepNhan, nvtiepnhan.TenNhanVien AS NhanVienTiepNhan,
	dmx.DungTich, dmx.MauSon, dmx.HopSo,
	cast(iif(dmx.ID_KhachHang = ptn.ID_KhachHang,'1','0') as bit) as LaChuXe,
	cx.TenDoiTuong as ChuXe,
	cx.DienThoai as ChuXe_SDT, cx.DiaChi as ChuXe_DiaChi, cx.Email as ChuXe_Email,
	dv.MaDonVi, dv.TenDonVi,
	ptn.NgayTao
	from Gara_PhieuTiepNhan ptn
	inner join Gara_DanhMucXe dmx on ptn.ID_Xe = dmx.ID
	LEFT join DM_DoiTuong cx on dmx.ID_KhachHang = cx.ID
	inner join DM_DoiTuong dt on dt.ID = ptn.ID_KhachHang
	left join NS_NhanVien nvcovan on nvcovan.ID = ptn.ID_CoVanDichVu
	inner join NS_NhanVien nvtiepnhan on nvtiepnhan.ID = ptn.ID_NhanVien
	inner join Gara_MauXe mauxe on mauxe.ID = dmx.ID_MauXe
	inner join Gara_HangXe hangxe on hangxe.ID = mauxe.ID_HangXe
	inner join Gara_LoaiXe loaixe on loaixe.ID = mauxe.ID_LoaiXe
	inner join DM_DonVi dv on dv.ID = ptn.ID_DonVi
	inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
	WHERE exists (select GiaTri from @tbTrangThai tt where ptn.TrangThai = tt.GiaTri)
		AND (@NgayTiepNhan_From IS NULL OR ptn.NgayVaoXuong BETWEEN @NgayTiepNhan_From AND @NgayTiepNhan_To)
		AND (@NgayXuatXuongDuKien_From IS NULL OR ptn.NgayXuatXuongDuKien BETWEEN @NgayXuatXuongDuKien_From AND @NgayXuatXuongDuKien_To)
		AND (@NgayXuatXuong_From IS NULL OR ptn.NgayXuatXuong BETWEEN @NgayXuatXuong_From AND @NgayXuatXuong_To)
		AND ((select count(Name) from @tblSearch b where     			
		ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
		or ptn.GhiChu like '%'+b.Name+'%'
		or dt.MaDoiTuong like '%'+b.Name+'%'		
		or dt.TenDoiTuong like '%'+b.Name+'%'
		or dt.DienThoai like '%'+b.Name+'%'
		or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
		or nvcovan.TenNhanVien like '%'+b.Name+'%'	
		or nvcovan.MaNhanVien like '%'+b.Name+'%'	
		or nvcovan.TenNhanVienKhongDau like '%'+b.Name+'%'	
		or nvcovan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
		or nvtiepnhan.TenNhanVien like '%'+b.Name+'%'	
		or nvtiepnhan.MaNhanVien like '%'+b.Name+'%'	
		or nvtiepnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
		or nvtiepnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
		or ptn.TenLienHe like '%'+b.Name+'%'	
		or ptn.SoDienThoaiLienHe like '%'+b.Name+'%'
		or dmx.BienSo like '%'+b.Name+'%'
		)=@count or @count=0)
		),
		count_cte
		as
		(
			select count(ID) as TotalRow,
				CEILING(COUNT(ID) / CAST(@PageSize as float ))  as TotalPage
			from data_cte
		)
		SELECT dt.*, ct.* FROM data_cte dt
		CROSS JOIN count_cte ct
		ORDER BY dt.NgayVaoXuong desc
		OFFSET (@CurrentPage * @PageSize) ROWS
		FETCH NEXT @PageSize ROWS ONLY;
	END
	ELSE
	BEGIN
		with data_cte
		as
		(
		select ptn.ID, ptn.MaPhieuTiepNhan, ptn.NgayVaoXuong, ptn.SoKmVao, ptn.NgayXuatXuongDuKien, ptn.NgayXuatXuong, ptn.TrangThai,
		ptn.SoKmRa, ptn.TenLienHe, ptn.SoDienThoaiLienHe, ptn.GhiChu, ptn.TrangThai AS TrangThaiPhieuTiepNhan,
		ptn.ID_Xe, dmx.BienSo, dmx.SoMay, dmx.SoKhung, dmx.NamSanXuat, mauxe.TenMauXe, hangxe.TenHangXe, loaixe.TenLoaiXe,
		ptn.ID_KhachHang, dt.MaDoiTuong, dt.TenDoiTuong, dt.Email, dt.DienThoai AS DienThoaiKhachHang, dt.DiaChi,
		ptn.ID_CoVanDichVu, ISNULL(nvcovan.TenNhanVien, '') AS CoVanDichVu, ISNULL(nvcovan.MaNhanVien, '') AS MaCoVan,
		ptn.ID_NhanVien, nvtiepnhan.MaNhanVien AS MaNhanVienTiepNhan, nvtiepnhan.TenNhanVien AS NhanVienTiepNhan,
		dmx.DungTich, dmx.MauSon, dmx.HopSo,
		cx.TenDoiTuong as ChuXe,
		cx.DienThoai as ChuXe_SDT, cx.DiaChi as ChuXe_DiaChi, cx.Email as ChuXe_Email,
		dv.MaDonVi, dv.TenDonVi,
		ptn.NgayTao
		from Gara_PhieuTiepNhan ptn
		inner join Gara_DanhMucXe dmx on ptn.ID_Xe = dmx.ID
		LEFT join DM_DoiTuong cx on dmx.ID_KhachHang = cx.ID
		inner join DM_DoiTuong dt on dt.ID = ptn.ID_KhachHang
		left join NS_NhanVien nvcovan on nvcovan.ID = ptn.ID_CoVanDichVu
		inner join NS_NhanVien nvtiepnhan on nvtiepnhan.ID = ptn.ID_NhanVien
		inner join Gara_MauXe mauxe on mauxe.ID = dmx.ID_MauXe
		inner join Gara_HangXe hangxe on hangxe.ID = mauxe.ID_HangXe
		inner join Gara_LoaiXe loaixe on loaixe.ID = mauxe.ID_LoaiXe
		inner join DM_DonVi dv on dv.ID = ptn.ID_DonVi
		inner join @tblDonVi donvi on donvi.ID_DonVi = dv.ID
		WHERE exists (select GiaTri from @tbTrangThai tt where ptn.TrangThai = tt.GiaTri)
			AND (@NgayTiepNhan_From IS NULL OR ptn.NgayVaoXuong BETWEEN @NgayTiepNhan_From AND @NgayTiepNhan_To)
			AND (@NgayXuatXuongDuKien_From IS NULL OR ptn.NgayXuatXuongDuKien BETWEEN @NgayXuatXuongDuKien_From AND @NgayXuatXuongDuKien_To)
			AND (@NgayXuatXuong_From IS NULL OR ptn.NgayXuatXuong BETWEEN @NgayXuatXuong_From AND @NgayXuatXuong_To)
			AND ((select count(Name) from @tblSearch b where     			
			ptn.MaPhieuTiepNhan like '%'+b.Name+'%'
			or ptn.GhiChu like '%'+b.Name+'%'
			or dt.MaDoiTuong like '%'+b.Name+'%'		
			or dt.TenDoiTuong like '%'+b.Name+'%'
			or dt.DienThoai like '%'+b.Name+'%'
			or dt.TenDoiTuong_KhongDau like '%'+b.Name+'%'
			or dt.TenDoiTuong_ChuCaiDau like '%'+b.Name+'%'
			or nvcovan.TenNhanVien like '%'+b.Name+'%'	
			or nvcovan.MaNhanVien like '%'+b.Name+'%'	
			or nvcovan.TenNhanVienKhongDau like '%'+b.Name+'%'	
			or nvcovan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
			or nvtiepnhan.TenNhanVien like '%'+b.Name+'%'	
			or nvtiepnhan.MaNhanVien like '%'+b.Name+'%'	
			or nvtiepnhan.TenNhanVienChuCaiDau like '%'+b.Name+'%'	
			or nvtiepnhan.TenNhanVienKhongDau like '%'+b.Name+'%'	
			or ptn.TenLienHe like '%'+b.Name+'%'	
			or ptn.SoDienThoaiLienHe like '%'+b.Name+'%'
			or dmx.BienSo like '%'+b.Name+'%'
			)=@count or @count=0)
			)
			SELECT dt.*, 0 AS TotalRow, CAST(0 AS FLOAT) AS TotalPage FROM data_cte dt
			ORDER BY dt.NgayVaoXuong desc
	END");

			CreateStoredProcedure(name: "[dbo].[GetMaMaPhieuTiepNhan_byTemp]", parametersAction: p => new
			{
				LoaiHoaDon = p.Int(),
				ID_DonVi = p.Guid(),
				NgayLapHoaDon = p.DateTime()
			}, body: @"SET NOCOUNT ON;
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
			DECLARE @namthangngay varchar(10) = convert(varchar(10), @NgayLapHoaDon, 112)
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
			select top 1 MaPhieuTiepNhan from Gara_PhieuTiepNhan 
			where MaPhieuTiepNhan like @sCompare +'%'  
			order by MaPhieuTiepNhan desc)
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
					select CONCAT(@sMaFull,left(@strstt,@lenSst-1),1) as MaxCode-- left(@strstt,@lenSst-1): bỏ bớt 1 số 0			
			else 
				begin
					set @Return = @Return + 1
					set @lenMaMax =  len(@Return)
					select 
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
						else '' end as MaxCode
				end 
		end
	else
		begin
			declare @machungtu varchar(10) = (select top 1 MaLoaiChungTu from DM_LoaiChungTu where ID= @LoaiHoaDon)
			declare @lenMaChungTu int= LEN(@machungtu)

			select @Return = MAX(CAST(dbo.udf_GetNumeric(RIGHT(MaPhieuTiepNhan,LEN(MaPhieuTiepNhan)- @lenMaChungTu))AS float))
			from Gara_PhieuTiepNhan where SUBSTRING(MaPhieuTiepNhan, 1, len(@machungtu)) = @machungtu and CHARINDEX('O',MaPhieuTiepNhan) = 0 -- not HDO, GDVO, THO, DHO
			
			-- do dai STT (toida = 10)
			if	@Return is null 
					select
						case when @lenMaChungTu = 2 then CONCAT(@machungtu, '00000000',1)
							when @lenMaChungTu = 3 then CONCAT(@machungtu, '0000000',1)
							when @lenMaChungTu = 4 then CONCAT(@machungtu, '000000',1)
							when @lenMaChungTu = 5 then CONCAT(@machungtu, '00000',1)
						else CONCAT(@machungtu,'000000',1)
						end as MaxCode
			else 
				begin
					set @Return = @Return + 1
					set @lenMaMax = len(@Return)
					select 
						case when @lenMaMax = 1 then CONCAT(@machungtu,'000000000',@Return)
							when @lenMaMax = 2 then CONCAT(@machungtu,'00000000',@Return)
							when @lenMaMax = 3 then CONCAT(@machungtu,'0000000',@Return)
							when @lenMaMax = 4 then CONCAT(@machungtu,'000000',@Return)
							when @lenMaMax = 5 then CONCAT(@machungtu,'00000',@Return)
							when @lenMaMax = 6 then CONCAT(@machungtu,'0000',@Return)
							when @lenMaMax = 7 then CONCAT(@machungtu,'000',@Return)
							when @lenMaMax = 8 then CONCAT(@machungtu,'00',@Return)
							when @lenMaMax = 9 then CONCAT(@machungtu,'0',@Return)								
						else CONCAT(@machungtu,CAST(@Return  as decimal(22,0))) end as MaxCode
				end 
		end");

			CreateStoredProcedure(name: "[dbo].[GetTonKho_byIDQuyDois]", parametersAction: p => new
			{
				ID_ChiNhanh = p.Guid(),
				ToDate = p.DateTime(),
				IDDonViQuyDois = p.String(),
				IDLoHangs = p.String()
			}, body: @"SET NOCOUNT ON;
	declare @tblIDQuiDoi table (ID_DonViQuyDoi uniqueidentifier)
	declare @tblIDLoHang table (ID_LoHang uniqueidentifier)

	insert into @tblIDQuiDoi
	select Name from dbo.splitstring(@IDDonViQuyDois) 
	insert into @tblIDLoHang
	select Name from dbo.splitstring(@IDLoHangs) where Name not like '%null%' and Name !=''

	SELECT [dbo].[FUNC_TonLuyKeTruocThoiGian](@ID_ChiNhanh, ID_HangHoa, ID_LoHang, @ToDate) as TonKho, ID_HangHoa, ID_LoHang, ID_DonViQuiDoi, MaHangHoa, MaLoHang,QuanLyTheoLoHang
	from
	(
		select hh.ID as ID_HangHoa, qd.ID as ID_DonViQuiDoi, lo.id as ID_LoHang, MaHangHoa, MaLoHang,hh.QuanLyTheoLoHang
		from DM_HangHoa hh
		join DonViQuiDoi qd on hh.ID= qd.ID_HangHoa
		left join DM_LoHang lo on hh.ID= lo.ID_HangHoa		
		where hh.TheoDoi= 1
		and exists( select ID_DonViQuyDoi from @tblIDQuiDoi qd2 where qd2.ID_DonViQuyDoi= qd.ID)
		and (exists( select ID_LoHang from @tblIDLoHang lo2 where lo2.ID_LoHang= lo.ID) Or hh.QuanLyTheoLoHang= 0)
	) a");

			CreateStoredProcedure(name: "[dbo].[JqAuto_HoaDonSC]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSearch = p.String()
			}, body: @"SET NOCOUNT ON;

	select top 10 hd.ID, hd.MaHoaDon, hd.ID_PhieuTiepNhan, tn.MaPhieuTiepNhan, xe.BienSo
	from BH_HoaDon hd
	join Gara_PhieuTiepNhan tn on hd.ID_PhieuTiepNhan = tn.ID
	join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID
	where 
	exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where dv.Name= tn.ID_DonVi )
	and hd.ID_PhieuTiepNhan is not null
	and hd.LoaiHoaDon= 25
	and hd.ChoThanhToan= 0
	and tn.NgayXuatXuong is null
	and (tn.MaPhieuTiepNhan like @TextSearch 
		or xe.BienSo like @TextSearch 		
		or hd.MaHoaDon like @TextSearch 		
		)
   order by tn.NgayVaoXuong desc ");

			CreateStoredProcedure(name: "[dbo].[JqAuto_PhieuTiepNhan]", parametersAction: p => new
			{
				IDChiNhanhs = p.String(),
				TextSearch = p.String(),
				CustomerID = p.String()
			}, body: @"SET NOCOUNT ON;
	select top 10 tn.ID, tn.MaPhieuTiepNhan, xe.BienSo, tn.ID_KhachHang
	from Gara_PhieuTiepNhan tn
	join Gara_DanhMucXe xe on tn.ID_Xe= xe.ID	
	where tn.ID_KhachHang like @CustomerID	
	and tn.TrangThai !=0
	and tn.NgayXuatXuong is null
	and exists (select Name from dbo.splitstring(@IDChiNhanhs) dv where dv.Name= tn.ID_DonVi )
	and (tn.MaPhieuTiepNhan like @TextSearch 
		or xe.BienSo like @TextSearch 		
		)
   order by tn.NgayVaoXuong desc");

			CreateStoredProcedure(name: "[dbo].[JqAuto_SearchMauXe]", parametersAction: p => new
			{
				TextSearch = p.String()
			}, body: @"SET NOCOUNT ON;
	select top 20 *
	from Gara_MauXe
	where TenMauXe like @TextSearch
	or TenMauXe like @TextSearch COLLATE Vietnamese_CI_AI ");

			CreateStoredProcedure(name: "[dbo].[NangNhom_KhachHangbyID]", parametersAction: p => new
			{
				ID_DoiTuong = p.Guid(),
				ID_ChiNhanh = p.Guid()
			}, body: @"SET NOCOUNT ON;
	declare @ThangSinh int, @Tuoi int, @GioiTinh bit, @ID_TinhThanh uniqueidentifier, @NgaySinh_NgayTLap datetime, @LoaiNgaySinh int 

	-- get ngaysinh, gioitinh, tinhthanh
select @GioiTinh = GioiTinhNam, @ThangSinh = ThangSinh, @Tuoi = Tuoi, @NgaySinh_NgayTLap = NgaySinh_NgayTLap,
@LoaiNgaySinh = LoaiNgaySinh, @ID_TinhThanh = ID_TinhThanh
from
(
select ID_TinhThanh, GioiTinhNam,  DATEDIFF(year, NgaySinh_NgayTLap, 
GETDATE()) as Tuoi, DATEPART(month, NgaySinh_NgayTLap) as ThangSinh, NgaySinh_NgayTLap, DinhDang_NgaySinh,
		case  DinhDang_NgaySinh
		 when 'dd/MM/yyyy' then 1
		 when 'dd/MM' then 2
		 when 'MM/yyyy' then 3
		 when 'yyyy' then 4
		 end as LoaiNgaySinh
from DM_DoiTuong where ID = @ID_DoiTuong
) a

declare @NoHienTai float, @TongBan float, @TongBanTruTraHang float, @SoLanMuaHang int
-- doanhthu, congno,..
select 
@NoHienTai = NoHienTai,
@TongBan = TongBan,
@TongBanTruTraHang = TongBanTruTraHang,
@SoLanMuaHang = SoLanMuaHang
from 
(
SELECT tblThuChi.ID_DoiTuong,
    					SUM(ISNULL(tblThuChi.DoanhThu,0)) + SUM(ISNULL(tblThuChi.TienChi,0)) - 
						SUM(ISNULL(tblThuChi.TienThu,0)) - SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS NoHienTai,
    				SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongBan,
					sum(ISNULL(tblThuChi.ThuTuThe,0)) as ThuTuThe,
    				SUM(ISNULL(tblThuChi.DoanhThu,0)) -  SUM(ISNULL(tblThuChi.GiaTriTra,0)) AS TongBanTruTraHang,
    				SUM(ISNULL(tblThuChi.GiaTriTra, 0)) - SUM(ISNULL(tblThuChi.DoanhThu, 0)) as TongMua,
    				SUM(ISNULL(tblThuChi.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(


	SELECT 
    	bhd.ID_DoiTuong,
    	0 AS GiaTriTra,
    	ISNULL(bhd.PhaiThanhToan,0) AS DoanhThu,
    	0 AS TienThu,
    	0 AS TienChi,
    	0 AS SoLanMuaHang,
		0 as ThuTuThe
    FROM BH_HoaDon bhd
    WHERE bhd.LoaiHoaDon in (1,7,19,22, 25) AND bhd.ChoThanhToan = 0
	AND bhd.ID_DoiTuong = @ID_DoiTuong
    AND bhd.ID_DonVi = @ID_ChiNhanh


		union all
							-- gia tri trả từ bán hàng
    						SELECT bhd.ID_DoiTuong,
    							ISNULL(bhd.PhaiThanhToan,0) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
									0 as ThuTuThe
    						FROM BH_HoaDon bhd   						
    						WHERE (bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = 4) AND bhd.ChoThanhToan = 0
							AND bhd.ID_DoiTuong = @ID_DoiTuong			
    						AND bhd.ID_DonVi = @ID_ChiNhanh

							-- tt= the
							union all
							select 
								hd.ID_DoiTuong,
								0 as GiaTriTra,
    							0 AS DoanhThu,
								0 AS TienThu,
    							0 AS TienChi, 
    							0 AS SoLanMuaHang,
								qct.ThuTuThe
							from Quy_HoaDon qhd 
							join Quy_HoaDon_ChiTiet qct on qhd.ID= qct.ID_HoaDon
							join BH_HoaDon hd on qct.ID_HoaDonLienQuan= hd.ID
							where qhd.TrangThai= 1
							and qct.ThuTuThe > 0
							and qhd.LoaiHoaDon= 11
							and hd.ID_DoiTuong = @ID_DoiTuong			
							AND hd.ID_DonVi = @ID_ChiNhanh


							union all

							-- tienthu
							SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							ISNULL(qhdct.TienThu,0) AS TienThu,
    							0 AS TienChi,
								0 AS SoLanMuaHang,
									0 as ThuTuThe
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
							Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID 
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    						AND qhd.ID_DonVi = @ID_ChiNhanh
							AND qhdct.ID_DoiTuong = @ID_DoiTuong
							
							union all

							-- tienchi
    						SELECT 
    							qhdct.ID_DoiTuong,						
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							ISNULL(qhdct.TienThu,0) AS TienChi,
								0 AS SoLanMuaHang,
									0 as ThuTuThe
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null)
								AND qhdct.ID_DoiTuong = @ID_DoiTuong
    						AND qhd.ID_DonVi = @ID_ChiNhanh

							Union All
							-- solan mua hang
    						Select 
    							hd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
								0 as TienChi,
    							COUNT(*) AS SoLanMuaHang	,
									0 as ThuTuThe
    						From BH_HoaDon hd 
    						where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    						and hd.ChoThanhToan = 0
    						AND hd.ID_DoiTuong = @ID_DoiTuong
    						
							GROUP BY hd.ID_DoiTuong  	
							) tblThuChi group by tblThuChi.ID_DoiTuong
	) a		
	

	

--insert into DM_DoiTuong_Nhom

select NewID(), @ID_DoiTuong, ID, *
from 
(
select *
from
(
select b.ID, b.TenNhomDoiTuong, 
max(TongBanTruTraHang) as TongBanTruTraHang, 
max(b.TongBan) as TongBan,
max(b.SoLanMuaHang) as SoLanMuaHang,
max(b.CongNo) as CongNo, 
max(b.ThangSinh) as ThangSinh, 
max(Tuoi) as Tuoi, 
max(b.GioiTinh) as GioiTinh, 
max(b.TinhThanh) as TinhThanh

from
(
select TenDonVi, dv.id, TenNhomDoiTuong, GiamGia, GiamGiaTheoPhanTram, TuDongCapNhat,
ct.LoaiDieuKien, ct.LoaiSoSanh, ct.GiaTriSo, ct.GiaTriBool, ct.GiaTriThoiGian, ct.GiaTriKhuVuc, ct.GiaTriVungMien,
case ct.LoaiDieuKien
 when 1 then 
	case ct.LoaiSoSanh
		when 1 then iif(@TongBanTruTraHang > ct.GiaTriSo,1,0)
		when 2 then iif(@TongBanTruTraHang >= ct.GiaTriSo,1,0)
		when 3 then iif(@TongBanTruTraHang = ct.GiaTriSo,1,0)
		when 4 then iif(@TongBanTruTraHang <= ct.GiaTriSo,1,0)
		when 5 then iif(@TongBanTruTraHang < ct.GiaTriSo,1,0)
		end 
	else -1 end as TongBanTruTraHang,
	case ct.LoaiDieuKien
 when 2 then 
	case ct.LoaiSoSanh
		when 1 then iif(@TongBan > ct.GiaTriSo, 1,0)
		when 2 then iif(@TongBan >= ct.GiaTriSo, 1,0)
		when 3 then iif(@TongBan = ct.GiaTriSo, 1,0)
		when 4 then iif(@TongBan >= ct.GiaTriSo, 1,0)
		when 5 then iif(@TongBan < ct.GiaTriSo, 1,0)
		end 
	else - 1 end as TongBan,
	case ct.LoaiDieuKien
 when 4 then 
	case ct.LoaiSoSanh
		when 1 then iif(@SoLanMuaHang > ct.GiaTriSo ,1,0)
		when 2 then iif(@SoLanMuaHang >= ct.GiaTriSo ,1,0)
		when 3 then iif(@SoLanMuaHang = ct.GiaTriSo ,1,0)
		when 4 then iif(@SoLanMuaHang <= ct.GiaTriSo ,1,0)
		when 5 then iif(@SoLanMuaHang < ct.GiaTriSo ,1,0)
		end 
	else - 1 end as SoLanMuaHang,
	case ct.LoaiDieuKien
 when 5 then 
	case ct.LoaiSoSanh
		when 1 then iif(@NoHienTai > ct.GiaTriSo ,1,0)
		when 2 then iif(@NoHienTai >= ct.GiaTriSo ,1,0)
		when 3 then iif(@NoHienTai = ct.GiaTriSo ,1,0)
		when 4 then iif(@NoHienTai <= ct.GiaTriSo ,1,0)
		when 5 then iif(@NoHienTai < ct.GiaTriSo ,1,0)
		end 
	else - 1 end as CongNo,

	case ct.LoaiDieuKien
 when 6 then 
	case ct.LoaiSoSanh
		when 1 then iif(@ThangSinh > ct.GiaTriSo ,1,0)
		when 2 then iif(@ThangSinh >= ct.GiaTriSo ,1,0)
		when 3 then iif(@ThangSinh = ct.GiaTriSo ,1,0)
		when 4 then iif(@ThangSinh <= ct.GiaTriSo ,1,0)
		when 5 then iif(@ThangSinh < ct.GiaTriSo ,1,0)
		end 
	else - 1 end as ThangSinh,
	case ct.LoaiSoSanh
	when 7 then 
	case ct.LoaiSoSanh
		when 1 then iif(@Tuoi > ct.GiaTriSo ,1,0)
		when 2 then iif(@Tuoi >= ct.GiaTriSo ,1,0)
		when 3 then iif(@Tuoi = ct.GiaTriSo ,1,0)
		when 4 then iif(@Tuoi <= ct.GiaTriSo ,1,0)
		when 5 then iif(@Tuoi < ct.GiaTriSo ,1,0)
		end 
	else - 1 end as Tuoi,
	case ct.LoaiSoSanh
	when 8 then 
	case ct.LoaiSoSanh		
		when 3 then iif(@GioiTinh = ct.GiaTriBool ,1,0)
		else iif(@GioiTinh != ct.GiaTriBool ,1,0)
		end 
	else - 1 end as GioiTinh,
	case ct.LoaiSoSanh
	when 9 then 
	case ct.LoaiSoSanh
		when 1 then iif(@ID_TinhThanh > ct.GiaTriKhuVuc ,1,0)
		when 2 then iif(@ID_TinhThanh >= ct.GiaTriKhuVuc ,1,0)
		when 3 then iif(@ID_TinhThanh = ct.GiaTriKhuVuc ,1,0)
		when 4 then iif(@ID_TinhThanh <= ct.GiaTriKhuVuc ,1,0)
		when 5 then iif(@ID_TinhThanh < ct.GiaTriKhuVuc ,1,0)
		end 
	else - 1 end as TinhThanh

from DM_NhomDoiTuong nhom
left join DM_NhomDoiTuong_ChiTiet ct on nhom.ID= ct.ID_NhomDoiTuong
left join NhomDoiTuong_DonVi ndv on nhom.ID= ndv.ID_NhomDoiTuong
left join DM_DonVi dv on dv.ID= ndv.ID_DonVi
where ndv.ID_DonVi= @ID_ChiNhanh
)	b group by b.ID, b.TenNhomDoiTuong
) c where TongBanTruTraHang !=-1 or TongBan !=-1 or SoLanMuaHang !=-1 or CongNo !=-1  --- get nhóm có thiết lập điều kiện trong DB
or ThangSinh !=-1 or Tuoi !=-1 or GioiTinh !=-1 or TinhThanh !=-1
)  nhomdudk 
where TongBanTruTraHang in (1,-1)
and TongBan  in (1,-1)
and SoLanMuaHang  in (1,-1)
and CongNo  in (1,-1)
and ThangSinh  in (1,-1)
and Tuoi  in (1,-1)
and GioiTinh  in (1,-1)
and TinhThanh  in (1,-1)
and not exists (select ID_NhomDoiTuong from DM_DoiTuong_Nhom where ID_DoiTuong= @ID_DoiTuong) -- chỉ insert nếu chưa dc add");

			CreateStoredProcedure(name: "[dbo].[PhieuTiepNhan_GetThongTinChiTiet]", parametersAction: p => new
			{
				ID_PhieuTiepNhan = p.Guid()
			}, body: @"SET NOCOUNT ON;	
	
	select tn.*,		
		xe.BienSo, xe.SoKhung, xe.SoMay,
		xe.DungTich, xe.HopSo, xe.MauSon, xe.NamSanXuat, 
		nvlap.TenNhanVien as NhanVienTiepNhan,
		nvlap.MaNhanVien as MaNVTiepNhan,
		ISNULL(nv.TenNhanVien,'') as CoVanDichVu,
		ISNULL(cv.DienThoaiDiDong,'') as CoVan_SDT,
		ISNULL(nv.MaNhanVien,'') as MaCoVan,
		ISNULL(nv.TenNhanVienKhongDau,'') as TenNhanVienKhongDau,
		ISNULL(dt.MaDoiTuong,'') as MaDoiTuong,
		isnull(dt.TenDoiTuong,'') as TenDoiTuong,
		isnull(dt.TenDoiTuong_KhongDau,'') as TenDoiTuong_KhongDau,
		dt.DienThoai as DienThoaiKhachHang,
		dt.DiaChi,
		dt.Email,
		cast(iif(xe.ID_KhachHang = tn.ID_KhachHang,'1','0') as bit) as LaChuXe,
		cx.TenDoiTuong as ChuXe,
		cx.DienThoai as ChuXe_SDT,
		cx.Email as ChuXe_Email,
		cx.DiaChi as ChuXe_DiaChi,
		mau.TenMauXe,
		hang.TenHangXe,
		loai.TenLoaiXe
	
	from Gara_PhieuTiepNhan tn
	join Gara_DanhMucXe xe on tn.ID_Xe = xe.ID
	join Gara_MauXe mau on xe.ID_MauXe = mau.ID
	join Gara_HangXe hang on mau.ID_HangXe= hang.ID
	join Gara_LoaiXe loai on mau.ID_LoaiXe= loai.ID
	join NS_NhanVien nvlap on tn.ID_NhanVien= nvlap.ID
	left join NS_NhanVien cv on tn.ID_CoVanDichVu= cv.ID
	left join DM_DoiTuong dt on tn.ID_KhachHang = dt.ID
	left join DM_DoiTuong cx on xe.ID_KhachHang = cx.ID
	left join NS_NhanVien nv on tn.ID_CoVanDichVu= nv.ID
	where tn.id= @ID_PhieuTiepNhan");

			CreateStoredProcedure(name: "[dbo].[TongQuanBieuDoDoanhThuThuan]", parametersAction: p => new
			{
				LoaBieuDo = p.Int(),
				Thang = p.Int(),
				Nam = p.String(),
				IdChiNhanhs = p.String()
			}, body: @"SET NOCOUNT ON;
	
DECLARE @tblDonVi TABLE(ID UNIQUEIDENTIFIER);
INSERT INTO @tblDonVi
select * from splitstring(@IdChiNhanhs);


DECLARE @tblDoanThu TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, DoanhThuThuan FLOAT);
DECLARE @tblGiaVon TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, GiaVon FLOAT);
DECLARE @tblChiPhi TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, ChiPhi FLOAT);
DECLARE @tblThuNhapKhac TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, ThuNhapKhac FLOAT, ChiPhiKhac FLOAT);
IF(@LoaBieuDo = 1) -- Theo Ngày
BEGIN
	--DoanhThu
	INSERT INTO @tblDoanThu
	SELECT
    	a.Ngay, a.ID_DonVi,
		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		DAY(hd.NgayLapHoaDon) as Ngay,
    		hd.LoaiHoaDon,
			hd.ID_DonVi,
    		Case When (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
			Case when hd.TongTienHang != 0 and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @Nam AND MONTH(hd.NgayLapHoaDon) = @Thang
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Ngay, a.ID_DonVi
		--GiaVon
		INSERT INTO @tblGiaVon
		SELECT 
		b.Ngay,
		b.ID_DonVi,
		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
		FROM
		(
		SELECT
    		a.Ngay,
			a.ID_DonViQuiDoi,
			a.ID_DonVi,
			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			DAY(hd.NgayLapHoaDon) as Ngay,
				hdct.ID_DonViQuiDoi,
				hd.ID_DonVi,
				hdct.SoLuong as SoLuongBan,
				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hd.NgayLapHoaDon) = @Nam AND MONTH(hd.NgayLapHoaDon) = @Thang
    			and hd.ChoThanhToan = 0
				Union all
				Select 
    			DAY(hdb.NgayLapHoaDon) as Ngay,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
				hdct.SoLuong as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hdt.NgayLapHoaDon) = @Nam AND MONTH(hdt.NgayLapHoaDon) = @Thang
    			and hdt.ChoThanhToan = 0
				UNION ALL
    			SELECT
    			DAY(hdb.NgayLapHoaDon) as Ngay,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				0 as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
				hdct.SoLuong as SoLuongTraNhanh,
				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where YEAR(hdb.NgayLapHoaDon) = @Nam AND MONTH(hdb.NgayLapHoaDon) = @Thang
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
				and hdb.ID_HoaDon is null
    		) as a
    		GROUP BY a.ID_DonViQuiDoi, a.Ngay, a.ID_DonVi
			) as b
			GROUP BY b.Ngay, b.ID_DonVi
			--Chiphi
		INSERT INTO @tblChiPhi
		SELECT
    	a.Ngay,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		DAY(hd.NgayLapHoaDon) as Ngay,
			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon = 8
    		and YEAR(hd.NgayLapHoaDon) = @Nam AND MONTH(hd.NgayLapHoaDon) = @Thang
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		DAY(qhd.NgayLapHoaDon) as Ngay,
			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam AND MONTH(qhd.NgayLapHoaDon) = @Thang
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Ngay, a.ID_DonVi;

		--Thu nhap khac
		INSERT INTO @tblThuNhapKhac
		SELECT
    	a.Ngay,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		DAY(qhd.NgayLapHoaDon) as Ngay,
			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3)) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam AND MONTH(qhd.NgayLapHoaDon) = @Thang
    		and (qhd.HachToanKinhDoanh = 1)
			and qhdct.DiemThanhToan is null
    	) as a
    	GROUP BY
    	a.Ngay, a.ID_DonVi
END
ELSE IF (@LoaBieuDo = 2) --Theo tháng
BEGIN
	--DoanhThu
	INSERT INTO @tblDoanThu
	SELECT
    	a.Thang, a.ID_DonVi,
		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		MONTH(hd.NgayLapHoaDon) as Thang,
    		hd.LoaiHoaDon,
			hd.ID_DonVi,
    		Case When (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
			Case when hd.TongTienHang != 0 and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Thang, a.ID_DonVi
		--GiaVon
		INSERT INTO @tblGiaVon
		SELECT 
		b.Thang,
		b.ID_DonVi,
		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
		FROM
		(
		SELECT
    		a.Thang,
			a.ID_DonViQuiDoi,
			a.ID_DonVi,
			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			MONTH(hd.NgayLapHoaDon) as Thang,
				hdct.ID_DonViQuiDoi,
				hd.ID_DonVi,
				hdct.SoLuong as SoLuongBan,
				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hd.NgayLapHoaDon) = @Nam
    			and hd.ChoThanhToan = 0
				Union all
				Select 
    			MONTH(hdb.NgayLapHoaDon) as Thang,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
				hdct.SoLuong as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hdt.NgayLapHoaDon) = @Nam
    			and hdt.ChoThanhToan = 0
				UNION ALL
    			SELECT
    			MONTH(hdb.NgayLapHoaDon) as Thang,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				0 as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
				hdct.SoLuong as SoLuongTraNhanh,
				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where YEAR(hdb.NgayLapHoaDon) = @Nam
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
				and hdb.ID_HoaDon is null
    		) as a
    		GROUP BY a.ID_DonViQuiDoi, a.Thang, a.ID_DonVi
			) as b
			GROUP BY b.Thang, b.ID_DonVi
			--Chiphi
		INSERT INTO @tblChiPhi
		SELECT
    	a.Thang,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		MONTH(hd.NgayLapHoaDon) as Thang,
			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon = 8
    		and YEAR(hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		MONTH(qhd.NgayLapHoaDon) as Thang,
			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Thang, a.ID_DonVi;

		--Thu nhap khac
		INSERT INTO @tblThuNhapKhac
		SELECT
    	a.Thang,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		MONTH(qhd.NgayLapHoaDon) as Thang,
			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3)) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and (qhd.HachToanKinhDoanh = 1)
			and qhdct.DiemThanhToan is null
    	) as a
    	GROUP BY
    	a.Thang, a.ID_DonVi
END
ELSE IF (@LoaBieuDo = 3) -- Theo Quý
BEGIN
	--DoanhThu
	INSERT INTO @tblDoanThu
	SELECT
    	a.Quy, a.ID_DonVi,
		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		DATEPART(QUARTER,hd.NgayLapHoaDon) as Quy,
    		hd.LoaiHoaDon,
			hd.ID_DonVi,
    		Case When (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
			Case when hd.TongTienHang != 0 and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25)
    		and DATEPART(YEAR, hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Quy, a.ID_DonVi
		--GiaVon
		INSERT INTO @tblGiaVon
		SELECT 
		b.Quy,
		b.ID_DonVi,
		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
		FROM
		(
		SELECT
    		a.Quy,
			a.ID_DonViQuiDoi,
			a.ID_DonVi,
			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			DATEPART(QUARTER,hd.NgayLapHoaDon) as Quy,
				hdct.ID_DonViQuiDoi,
				hd.ID_DonVi,
				hdct.SoLuong as SoLuongBan,
				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hd.NgayLapHoaDon) = @Nam
    			and hd.ChoThanhToan = 0
				Union all
				Select 
    			DATEPART(QUARTER,hdb.NgayLapHoaDon) as Quy,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
				hdct.SoLuong as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and YEAR(hdt.NgayLapHoaDon) = @Nam
    			and hdt.ChoThanhToan = 0
				UNION ALL
    			SELECT
    			DATEPART(QUARTER,hdb.NgayLapHoaDon) as Quy,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				0 as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
				hdct.SoLuong as SoLuongTraNhanh,
				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
    			where YEAR(hdb.NgayLapHoaDon) = @Nam
    			and hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
				and hdb.ID_HoaDon is null
    		) as a
    		GROUP BY a.ID_DonViQuiDoi, a.Quy, a.ID_DonVi
			) as b
			GROUP BY b.Quy, b.ID_DonVi
			--Chiphi
		INSERT INTO @tblChiPhi
		SELECT
    	a.Quy,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		DATEPART(QUARTER,hd.NgayLapHoaDon) as Quy,
			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
    		where hd.LoaiHoaDon = 8
    		and YEAR(hd.NgayLapHoaDon) = @Nam
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		DATEPART(QUARTER,qhd.NgayLapHoaDon) as Quy,
			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Quy, a.ID_DonVi;

		--Thu nhap khac
		INSERT INTO @tblThuNhapKhac
		SELECT
    	a.Quy,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		DATEPART(QUARTER,qhd.NgayLapHoaDon) as Quy,
			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3)) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and YEAR(qhd.NgayLapHoaDon) = @Nam
    		and (qhd.HachToanKinhDoanh = 1)
			and qhdct.DiemThanhToan is null
    	) as a
    	GROUP BY
    	a.Quy, a.ID_DonVi
END
ELSE IF (@LoaBieuDo = 4) --Theo năm
BEGIN
	DECLARE @tblNam TABLE(Nam INT);
	INSERT INTO @tblNam
	select * from splitstring(@Nam);
	--DoanhThu
	INSERT INTO @tblDoanThu
	SELECT
    	a.Nam, a.ID_DonVi,
		CAST(ROUND(SUM(a.DoanhThu - (a.GiaTriTra + a.GiamGiaHDB - a.GiamGiaHDT) - GiaVonGDV), 0) AS FLOAT) AS DoanhThuThuan
    	FROM
    	(
    		Select 
    		YEAR(hd.NgayLapHoaDon) as Nam,
    		hd.LoaiHoaDon,
			hd.ID_DonVi,
    		Case When (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25) and hdct.ChatLieu != 4 then hdct.ThanhTien else 0 end as DoanhThu,
			Case When (hd.LoaiHoaDon = 1) and hdct.ID_ChiTietGoiDV is not null then ISNULL(hdct.SoLuong * hdct.GiaVon ,0) else 0 end as GiaVonGDV,
    		Case When hd.LoaiHoaDon = 6 then hdct.ThanhTien else 0 end as GiaTriTra,
			Case when hd.TongTienHang != 0 and (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19) then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDB,
			Case when hd.TongTienHang != 0 and hd.LoaiHoaDon = 6 then hdct.ThanhTien * ((ISNULL(hd.TongGiamGia, 0) + ISNULL(hd.KhuyeMai_GiamGia, 0)) / ISNULL(hd.TongTienHang, 0)) else 0 end as GiamGiaHDT
    		From BH_HoaDon hd
			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
			INNER JOIN @tblNam nam ON nam.Nam = YEAR(hd.NgayLapHoaDon)
    		where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 6 or hd.LoaiHoaDon = 19 OR hd.LoaiHoaDon = 25)
    		and hd.ChoThanhToan = 0
    	) as a
    	GROUP BY
    	a.Nam, a.ID_DonVi
		--GiaVon
		INSERT INTO @tblGiaVon
		SELECT 
		b.Nam,
		b.ID_DonVi,
		SUM(CAST(ROUND(b.TongGiaVonBan - b.TongGiaVonTra, 0) as float)) as GiaVon
		FROM
		(
		SELECT
    		a.Nam,
			a.ID_DonViQuiDoi,
			a.ID_DonVi,
			SUM((a.SoLuongBan - a.SoLuongTra) * a.GiaVonBan) as TongGiaVonBan,
			SUM(a.SoLuongTraNhanh * a.GiaVonTraNhanh) as TongGiaVonTra
    		FROM
    		(
    			Select 
    			YEAR(hd.NgayLapHoaDon) as Nam,
				hdct.ID_DonViQuiDoi,
				hd.ID_DonVi,
				hdct.SoLuong as SoLuongBan,
				ISNULL(hdct.GiaVon, 0) as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hd
    			inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
				INNER JOIN @tblNam nam ON nam.Nam = YEAR(hd.NgayLapHoaDon)
    			where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and hd.ChoThanhToan = 0
				Union all
				Select 
    			YEAR(hdb.NgayLapHoaDon) as Nam,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				ISNULL(ctb.GiaVon, 0) as GiaVonBan,
				hdct.SoLuong as SoLuongTra,
				0 as GiaVonTra,
    			0 as SoLuongTraNhanh,
				0 as GiaVonTraNhanh
    			From BH_HoaDon hdb
				inner join BH_HoaDon hdt on hdb.ID = hdt.ID_HoaDon
    			inner join BH_HoaDon_ChiTiet hdct on hdt.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				join BH_HoaDon_ChiTiet ctb on (ctb.ID_HoaDon =  hdt.ID_HoaDon and ctb.ID_DonViQuiDoi = hdct.ID_DonViQuiDoi and ((ctb.ID_LoHang = hdct.ID_LoHang) or (ctb.ID_LoHang is null and hdct.ID_LoHang is null))) 
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
				INNER JOIN @tblNam nam ON nam.Nam = YEAR(hdb.NgayLapHoaDon)
    			where (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19)
				and hdct.ID_ChiTietGoiDV is null
    			and hdt.ChoThanhToan = 0
				UNION ALL
    			SELECT
    			YEAR(hdb.NgayLapHoaDon) as Nam,
				hdct.ID_DonViQuiDoi,
				hdb.ID_DonVi,
				0 as SoLuongBan,
				0 as GiaVonBan,
				0 as SoLuongTra,
				0 as GiaVonTra,
				hdct.SoLuong as SoLuongTraNhanh,
				ISNULL(hdct.GiaVon, 0) as GiaVonTraNhanh
    			FROM
    			BH_HoaDon hdb
    			join BH_HoaDon_ChiTiet hdct on hdb.ID = hdct.ID_HoaDon and (hdct.ID_ChiTietDinhLuong = hdct.ID or hdct.ID_ChiTietDinhLuong is null)
				inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
				INNER JOIN @tblDonVi AS dv ON dv.ID = hdb.ID_DonVi
				INNER JOIN @tblNam nam ON nam.Nam = YEAR(hdb.NgayLapHoaDon)
    			where hdb.ChoThanhToan = 0
    			and hdb.LoaiHoaDon = 6
				and hdb.ID_HoaDon is null
    		) as a
    		GROUP BY a.ID_DonViQuiDoi, a.Nam, a.ID_DonVi
			) as b
			GROUP BY b.Nam, b.ID_DonVi
			--Chiphi
		INSERT INTO @tblChiPhi
		SELECT
    	a.Nam,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.GiaTriHuy + a.DiemThanhToan), 0) as float) as ChiPhi
    	FROM
    	(
    		Select 
    		YEAR(hd.NgayLapHoaDon) as Nam,
			hd.ID_DonVi,
    		hdct.ThanhTien as GiaTriHuy,
    		0 as DiemThanhToan
    		From BH_HoaDon hd
    		inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = hd.ID_DonVi
			INNER JOIN @tblNam nam ON nam.Nam = YEAR(hd.NgayLapHoaDon)
    		where hd.LoaiHoaDon = 8
    		and hd.ChoThanhToan = 0
    		UNION ALL
    		Select 
    		YEAR(qhd.NgayLapHoaDon) as Nam,
			qhd.ID_DonVi,
    		0 as GiaTriHuy,
    		qhdct.TienThu as DiemThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
			INNER JOIN @tblNam nam ON nam.Nam = YEAR(qhd.NgayLapHoaDon)
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and qhdct.DiemThanhToan > 0
    	) as a
    	GROUP BY
    	a.Nam, a.ID_DonVi;

		--Thu nhap khac
		INSERT INTO @tblThuNhapKhac
		SELECT
    	a.Nam,
		a.ID_DonVi,
    	CAST(ROUND(SUM(a.ThuNhapKhac + a.PhiTraHangNhap), 0) as float) as ThuNhapKhac,
    	CAST(ROUND(SUM(a.ChiPhiKhac), 0) as float) as ChiPhiKhac
    	FROM
    	(
    		Select 
    		YEAR(qhd.NgayLapHoaDon) as Nam,
			qhd.ID_DonVi,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 11) then qhdct.TienThu else 0 end as ThuNhapKhac,
    		Case when (hd.LoaiHoaDon is null and qhd.LoaiHoaDon = 12) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then qhdct.TienThu else 0 end as ChiPhiKhac,
    		Case when (hd.LoaiHoaDon = 7) then qhdct.TienThu else 0 end as PhiTraHangNhap,
    		Case when ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3)) and qhd.LoaiHoaDon = 11 then qhdct.TienThu else 0 end as KhachThanhToan
    		From Quy_HoaDon qhd
    		inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    		left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
			INNER JOIN @tblDonVi AS dv ON dv.ID = qhd.ID_DonVi
			INNER JOIN @tblNam nam ON nam.Nam = YEAR(qhd.NgayLapHoaDon)
    		where (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    		and (qhd.HachToanKinhDoanh = 1)
			and qhdct.DiemThanhToan is null
    	) as a
    	GROUP BY
    	a.Nam, a.ID_DonVi
END
SELECT tbl.ID_DonVi, dv.MaDonVi, dv.TenDonVi, CAST(tbl.ThoiGian AS INT) AS ThoiGian, tbl.DoanhThuThuan, tbl.LoiNhuan FROM
(SELECT IIF(dt.ThoiGian IS NOT NULL, dt.ThoiGian, IIF(gv.ThoiGian IS NOT NULL, gv.ThoiGian, IIF(cp.ThoiGian IS NOT NULL, cp.ThoiGian, tnk.ThoiGian))) AS ThoiGian,
IIF(dt.ID_DonVi IS NOT NULL, dt.ID_DonVi, IIF(gv.ID_DonVi IS NOT NULL, gv.ID_DonVi, IIF(cp.ID_DonVi IS NOT NULL, cp.ID_DonVi, tnk.ID_DonVi))) AS ID_DonVi,
ISNULL(dt.DoanhThuThuan, 0) - ISNULL(gv.GiaVon, 0) + ISNULL(tnk.ThuNhapKhac, 0) - ISNULL(tnk.ChiPhiKhac, 0) AS LoiNhuan,
ISNULL(dt.DoanhThuThuan, 0) AS DoanhThuThuan FROM @tblDoanThu dt
FULL JOIN @tblGiaVon gv ON dt.ThoiGian = gv.ThoiGian AND dt.ID_DonVi = gv.ID_DonVi
FULL JOIN @tblChiPhi cp ON dt.ThoiGian = cp.ThoiGian AND dt.ID_DonVi = cp.ID_DonVi
FULL JOIN @tblThuNhapKhac tnk ON dt.ThoiGian = tnk.ThoiGian AND dt.ID_DonVi = tnk.ID_DonVi) AS tbl
INNER JOIN DM_DonVi dv ON dv.ID = tbl.ID_DonVi");

			CreateStoredProcedure(name: "[dbo].[TongQuanBieuDoThucThu]", parametersAction: p => new
			{
				LoaBieuDo = p.Int(),
				Thang = p.Int(),
				Nam = p.String(),
				IdChiNhanhs = p.String()
			}, body: @"SET NOCOUNT ON;
	
DECLARE @tblDonVi TABLE(ID UNIQUEIDENTIFIER);
INSERT INTO @tblDonVi
select * from splitstring(@IdChiNhanhs);


DECLARE @tblThucThu TABLE(ThoiGian INT, ID_DonVi UNIQUEIDENTIFIER, ThucThu FLOAT);
IF(@LoaBieuDo = 1) -- Theo Ngày
BEGIN
	INSERT INTO @tblThucThu
	SELECT 
	a.Ngay,
	a.ID_DonVi,
	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
	FROM
	(
    	SELECT
		DAY(qhd.NgayLapHoaDon) as Ngay,
		qhd.ID_DonVi,
		qct.TienThu as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
		where qhd.LoaiHoaDon = 11
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.DiemThanhToan is null
		and YEAR(qhd.NgayLapHoaDon) = @Nam and MONTH(qhd.NgayLapHoaDon) = @Thang
		and qhd.HachToanKinhDoanh = 1
	) a
    GROUP BY a.Ngay, a.ID_DonVi
	ORDER BY Ngay
END
ELSE IF (@LoaBieuDo = 2) --Theo tháng
BEGIN
	INSERT INTO @tblThucThu
	SELECT 
	a.Thang,
	a.ID_DonVi,
	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
	FROM
	(
    	SELECT
		MONTH(qhd.NgayLapHoaDon) as Thang,
		qhd.ID_DonVi,
		qct.TienThu as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
		where qhd.LoaiHoaDon = 11
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.DiemThanhToan is null
		and YEAR(qhd.NgayLapHoaDon) = @Nam
		and qhd.HachToanKinhDoanh = 1
	) a
    GROUP BY a.Thang, a.ID_DonVi
	ORDER BY Thang
END
ELSE IF (@LoaBieuDo = 3) -- Theo Quý
BEGIN
	INSERT INTO @tblThucThu
	SELECT 
	a.Quy,
	a.ID_DonVi,
	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
	FROM
	(
    	SELECT
		DATEPART(QUARTER,qhd.NgayLapHoaDon) as Quy,
		qhd.ID_DonVi,
		qct.TienThu as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
		where qhd.LoaiHoaDon = 11
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.DiemThanhToan is null
		and YEAR(qhd.NgayLapHoaDon) = @Nam
		and qhd.HachToanKinhDoanh = 1
	) a
    GROUP BY a.Quy, a.ID_DonVi
	ORDER BY Quy
END
ELSE IF (@LoaBieuDo = 4) --Theo năm
BEGIN
	DECLARE @tblNam TABLE(Nam INT);
	INSERT INTO @tblNam
	select * from splitstring(@Nam);

	INSERT INTO @tblThucThu
	SELECT 
	a.Nam,
	a.ID_DonVi,
	CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
	FROM
	(
    	SELECT
		YEAR(qhd.NgayLapHoaDon) as Nam,
		qhd.ID_DonVi,
		qct.TienThu as ThanhTien
		from Quy_HoaDon_ChiTiet qct
		INNER join Quy_HoaDon qhd on qct.ID_HoaDon = qhd.ID
		INNER JOIN @tblDonVi dv ON dv.ID = qhd.ID_DonVi
		INNER JOIN @tblNam nam ON nam.Nam = YEAR(qhd.NgayLapHoaDon)
		where qhd.LoaiHoaDon = 11
		and (qhd.TrangThai is null or qhd.TrangThai != 0)
		and qct.DiemThanhToan is null
		and qhd.HachToanKinhDoanh = 1
	) a
    GROUP BY a.Nam, a.ID_DonVi
	ORDER BY Nam
END
	SELECT tt.ID_DonVi, dv.MaDonVi, dv.TenDonVi, tt.ThoiGian, tt.ThucThu FROM @tblThucThu tt
	INNER JOIN DM_DonVi dv ON dv.ID = tt.ID_DonVi");

			CreateStoredProcedure(name: "[dbo].[TongQuanDoanhThuCongNo]", parametersAction: p => new
			{
				IdChiNhanhs = p.String(),
				DateFrom = p.DateTime(),
				DateTo = p.DateTime()
			}, body: @"SET NOCOUNT ON;
	declare @tblDonVi table (ID_DonVi  uniqueidentifier)
	if(@IdChiNhanhs != '')
	BEGIN
		insert into @tblDonVi
		select Name from dbo.splitstring(@IdChiNhanhs);
	END
    -- Insert statements for procedure here
	DECLARE @tblHoaDon TABLE(ID UNIQUEIDENTIFIER, LoaiHoaDon INT, TongThanhToan FLOAT, PhaiThanhToan FLOAT);
	INSERT INTO @tblHoaDon
	SELECT ID, LoaiHoaDon, TongThanhToan, PhaiThanhToan FROM BH_HoaDon hd
	INNER JOIN @tblDonVi dv ON hd.ID_DonVi = dv.ID_DonVi
	WHERE hd.LoaiHoaDon IN (1, 4, 6, 7, 19, 25)
	AND hd.ChoThanhToan = 0
	AND hd.NgayLapHoaDon BETWEEN @DateFrom AND @DateTo;

	DECLARE @tblSoQuy TABLE(ID UNIQUEIDENTIFIER, LoaiHoaDon INT, TienMat FLOAT, TienGui FLOAT, IDHoaDonLienQuan UNIQUEIDENTIFIER);
	INSERT INTO @tblSoQuy
	SELECT qhd.ID, qhd.LoaiHoaDon, qhdct.TienMat, qhdct.TienGui, qhdct.ID_HoaDonLienQuan FROM Quy_HoaDon qhd
	INNER JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
	INNER JOIN @tblDonVi dv ON qhd.ID_DonVi = dv.ID_DonVi
	WHERE qhd.NgayLapHoaDon BETWEEN @DateFrom AND @DateTo
	AND qhd.LoaiHoaDon IN (11, 12);

	DECLARE @DoanhThuSuaChua FLOAT,
	@DoanhThuBanHang FLOAT,
	@PhaiTraKhachHang FLOAT,
	@PhaiTraNhaCungCap FLOAT,
	@PhaiThuNhaCungCap FLOAT,
	@Thu_TienMat FLOAT,
	@Thu_NganHang FLOAT,
	@Chi_TienMat FLOAT,
	@Chi_NganHang FLOAT,
	@HoaDonDaThu FLOAT,
	@HoaDonDaChi FLOAT;

	SELECT @DoanhThuSuaChua =SUM(CASE WHEN hd.LoaiHoaDon = 25 THEN hd.TongThanhToan ELSE 0 END), 
		@PhaiTraKhachHang = SUM(CASE WHEN hd.LoaiHoaDon = 6 THEN -hd.TongThanhToan ELSE 0 END),
		@DoanhThuBanHang = SUM(CASE WHEN hd.LoaiHoaDon IN (1, 19)
		THEN
			hd.PhaiThanhToan
			ELSE 0
		END),
	@PhaiTraNhaCungCap = SUM(CASE WHEN hd.LoaiHoaDon = 4 THEN hd.PhaiThanhToan ELSE 0 END),
	@PhaiThuNhaCungCap = SUM(CASE WHEN hd.LoaiHoaDon = 7 THEN hd.PhaiThanhToan ELSE 0 END)
	FROM @tblHoaDon hd;

	SELECT @Thu_TienMat = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.TienMat ELSE 0 END), 
	@Thu_NganHang = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.TienGui ELSE 0 END),
	@Chi_TienMat = SUM(CASE WHEN sq.LoaiHoaDon = 12 THEN sq.TienMat ELSE 0 END),
	@Chi_NganHang = SUM(CASE WHEN sq.LoaiHoaDon = 12 THEN sq.TienGui ELSE 0 END)
	FROM @tblSoQuy sq;

	SELECT @HoaDonDaThu = SUM(CASE WHEN sq.LoaiHoaDon = 11 THEN sq.ThanhToan ELSE 0 END),  
	@HoaDonDaChi = SUM(CASE WHEN sq.LoaiHoaDon = 12 THEN sq.ThanhToan ELSE 0 END) FROM @tblHoaDon hd
	INNER JOIN (SELECT IDHoaDonLienQuan, SUM(TienGui + TienMat) AS ThanhToan, LoaiHoaDon FROM @tblSoQuy GROUP BY IDHoaDonLienQuan, LoaiHoaDon) sq
	ON hd.ID = sq.IDHoaDonLienQuan

	SET @DoanhThuSuaChua = ISNULL(@DoanhThuSuaChua, 0);
	SET @DoanhThuBanHang = ISNULL(@DoanhThuBanHang, 0);
	SET @PhaiTraKhachHang = ISNULL(@PhaiTraKhachHang, 0);
	SET @PhaiTraNhaCungCap = ISNULL(@PhaiTraNhaCungCap, 0);
	SET @PhaiThuNhaCungCap = ISNULL(@PhaiThuNhaCungCap, 0);
	SET @Thu_TienMat = ISNULL(@Thu_TienMat, 0);
	SET @Thu_NganHang = ISNULL(@Thu_NganHang, 0);
	SET @Chi_TienMat = ISNULL(@Chi_TienMat, 0);
	SET @Chi_NganHang = ISNULL(@Chi_NganHang, 0);
	SET @HoaDonDaThu = ISNULL(@HoaDonDaThu, 0);
	SET @HoaDonDaChi = ISNULL(@HoaDonDaChi, 0);

	SELECT ISNULL(@DoanhThuSuaChua, 0) AS DoanhThuSuaChua, @DoanhThuBanHang AS DoanhThuBanHang,
	@DoanhThuBanHang + @DoanhThuSuaChua AS TongDoanhThu, @DoanhThuBanHang + @DoanhThuSuaChua + @PhaiThuNhaCungCap - @HoaDonDaThu AS CongNoPhaiThu,
	@PhaiTraNhaCungCap + @PhaiTraKhachHang - @HoaDonDaChi AS CongNoPhaiTra, 
	(@DoanhThuBanHang + @DoanhThuSuaChua + @PhaiThuNhaCungCap - @HoaDonDaThu) - (@PhaiTraNhaCungCap + @PhaiTraKhachHang - @HoaDonDaChi) AS TongCongNo,
	@Thu_TienMat AS ThuTienMat, @Thu_NganHang AS ThuNganHang, @Thu_TienMat + @Thu_NganHang AS TongTienThu,
	@Chi_TienMat AS ChiTienMat, @Chi_NganHang AS ChiNganHang, @Chi_TienMat + @Chi_NganHang AS TongTienChi;");

			CreateStoredProcedure(name: "[dbo].[XuatKhoToanBo_FromHoaDonSC]", parametersAction: p => new
			{
				ID_HoaDon = p.Guid()
			}, body: @"SET NOCOUNT ON;
	
	declare @ngaylapHD datetime, @ID_DonVi uniqueidentifier,  @mahoadon nvarchar(max)
	select @ngaylapHD = NgayLapHoaDon, @ID_DonVi = ID_DonVi from BH_HoaDon where id= @ID_HoaDon
	declare @ngayxuatkho datetime = dateadd(MINUTE,1,@ngaylapHD)
	
	-- tinh tong giatrixuat (theo GiaVon)
	declare @tongGtriXuat float = (select sum(GiaVon) from BH_HoaDon_ChiTiet where ID_HoaDon= @ID_HoaDon)

	-- get mahoadon xuatkho
	declare @tblMa table (MaHoaDon nvarchar(max))
	insert into @tblMa
	exec GetMaHoaDonMax_byTemp 8, @ID_DonVi, @ngaylapHD
	select @mahoadon = MaHoaDon from @tblMa

	-- insert to BH_HoaDon, HoaDon_ChiTiet
	 declare @ID_XuatKho uniqueidentifier = newID()
	insert into BH_HoaDon (ID, LoaiHoaDon, MaHoaDon, ID_HoaDon,ID_PhieuTiepNhan, NgayLapHoaDon, ID_DonVi, ID_NhanVien, TongTienHang, TongThanhToan, TongChietKhau, TongChiPhi, TongGiamGia, TongTienThue, 
	PhaiThanhToan, PhaiThanhToanBaoHiem, ChoThanhToan, YeuCau, NgayTao, NguoiTao, DienGiai)
	select @ID_XuatKho, 8, @mahoadon,@ID_HoaDon,ID_PhieuTiepNhan, @ngayxuatkho, @ID_DonVi,ID_NhanVien, @tongGtriXuat,0,0,0,0,0, @tongGtriXuat,0,0,N'Hoàn thành', GETDATE(), NguoiTao, DienGiai
	from BH_HoaDon 
	where id= @ID_HoaDon

	insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, ID_DonViQuiDoi, ID_LoHang, SoLuong, DonGia,  ThanhTien, ThanhToan, 
	PTChietKhau, TienChietKhau, PTChiPhi, TienChiPhi, GiaVon, TienThue, An_Hien, TonLuyKe, ID_ChiTietGoiDV)
	select NEWID(), @ID_XuatKho, ct.SothuTu, ct.ID_DonViQuiDoi, ct.ID_LoHang, ct.SoLuong, ct.GiaVon, ct.SoLuong * ct.GiaVon,
	0, 0,0,0,0, ct.GiaVon,0,0, ct.TonLuyKe, ct.ID
	from BH_HoaDon_ChiTiet ct
	join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
	join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
	where ID_HoaDon = @ID_HoaDon
	and hh.LaHangHoa=1
	
	-- update tonkho, giavon
   exec UpdateTonLuyKeCTHD_whenUpdate @ID_XuatKho, @ID_DonVi, @ngaylapHD
   exec UpdateGiaVon_WhenEditCTHD  @ID_XuatKho, @ID_DonVi, @ngaylapHD ");

			Sql(@"CREATE trigger [dbo].[trg_InsertNhomDoiTuongs] on [dbo].[DM_DoiTuong_Nhom]
for insert,update
as 
	set nocount on
	declare @IDDoiTuong UNIQUEIDENTIFIER = (select top 1 ID_DoiTuong from inserted)
	exec UpdateNhomDoiTuongs_ByID @IDDoiTuong");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoNam]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
	@year [int],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [nvarchar](max),
    @LoaiTien [nvarchar](max)
AS
BEGIN
	set nocount on;
		declare @tblNhomDT table(ID_NhomDoiTuong varchar(40))
		insert into @tblNhomDT
		select * from dbo.splitstring(@ID_NhomDoiTuong_SP)
    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	select 
			b.ID_KhoanThuChi,
			b.KhoanMuc,
			sum(b.Thang1) as Thang1,
			sum(b.Thang2) as Thang2,
			sum(b.Thang3) as Thang3,
			sum(b.Thang4) as Thang4,
			sum(b.Thang5) as Thang5,
			sum(b.Thang6) as Thang6,
			sum(b.Thang7) as Thang7,
			sum(b.Thang8) as Thang8,
			sum(b.Thang9) as Thang9,
			sum(b.Thang10) as Thang10,
			sum(b.Thang11) as Thang11,
			sum(b.Thang12) as Thang12,
			max(STT) as STT

	from
	(
		select 
			a.ID_KhoanThuChi,
			case a.LoaiThuChi
			when 3 then N'Thu tiền bán hàng'
			when 5 then N'Thu trả hàng nhà cung cấp'		
			else case when a.ID_KhoanThuChi is null then N'Thu mặc định' else NoiDungThuChi end end as KhoanMuc,
			case when a.ThangLapHoaDon = 1 then tienthu end as Thang1,
			case when a.ThangLapHoaDon = 2 then tienthu end as Thang2,
			case when a.ThangLapHoaDon = 3 then tienthu end as Thang3,
			case when a.ThangLapHoaDon = 4 then tienthu end as Thang4,
			case when a.ThangLapHoaDon = 5 then tienthu end as Thang5,
			case when a.ThangLapHoaDon = 6 then tienthu end as Thang6,
			case when a.ThangLapHoaDon = 7 then tienthu end as Thang7,
			case when a.ThangLapHoaDon = 8 then tienthu end as Thang8,
			case when a.ThangLapHoaDon = 9 then tienthu end as Thang9,
			case when a.ThangLapHoaDon = 10 then tienthu end as Thang10,
			case when a.ThangLapHoaDon = 11 then tienthu end as Thang11,
			case when a.ThangLapHoaDon = 12 then tienthu end as Thang12		,
			ROW_NUMBER() OVER(ORDER BY a.NoiDungThuChi) as STT		
		from
		(
		select 
    			--a1.ID_NhomDoiTuong,
				a1.LoaiThuChi,			
				a1.ID_KhoanThuChi,
				a1.NoiDungThuChi,
    			a1.ThangLapHoaDon,
				Case when @LoaiTien = '%1%' then a1.TienMat
					when @LoaiTien = '%2%' then a1.TienGui else a1.tienmat + a1.TienGui end as TienThu,
    			Case when a1.TienMat > 0 and TienGui = 0 then '1'  
    			 when a1.TienGui > 0 and TienMat = 0 then '2' 
    			 when a1.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
				select    					
					qhdct.ID_KhoanThuChi,
    				ktc.NoiDungThuChi,
    				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    				Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    				 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    				 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    				 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    				 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    				 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
					case when dt.LoaiDoiTuong= 1 then	
					case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end
					else
						case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000002' else dt.IDNhomDoiTuongs end end as ID_NhomDoiTuong,   
					tienmat, tiengui,
    				tienmat +  tiengui as TienThu,
					DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
    				hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan= 0)
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH 
				or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or hd.MaHoaDon like @MaKH or hd.MaHoaDon like @MaKH_TV)
    			and qhd.LoaiHoaDon = 11
    			and qhd.HachToanKinhDoanh like @HachToanKD
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') 
				) a1
				where (EXISTS(SELECT Name FROM splitstring(a1.ID_NhomDoiTuong) dtDB inner JOIN @tblNhomDT dtS ON dtDB.Name = dtS.ID_NhomDoiTuong) 
				or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
				--where  (a1.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
		) a where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) --and a.LoaiTien like @LoaiTien
	) b group by b.ID_KhoanThuChi, b.KhoanMuc
			DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then MAX(b.TienMat)
				when @LoaiTien = '%2%' then MAX(b.TienGui) else
				MAX(b.TienMat + b.TienGui) end as TienThu
				--MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,    		
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or hd.MaHoaDon like @MaKH or hd.MaHoaDon like @MaKH_TV)	
    			and qhd.LoaiHoaDon = 12
    			and qhd.HachToanKinhDoanh like @HachToanKD
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0')
    			Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select *
			from
			(
			select max(ID_KhoanThuChi) as ID_KhoanThuChi, -- deu chi tien nhaphang, nhưng ID_KhoanThuChi # nhau --> thi bi douple, nen chi group tho KhoanMua va lay max (ID_KhoanThuChi)
			KhoanMuc, 
			MAX(STT) as STT,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY KhoanMuc
			) tblview where TongCong > 0
			order by STT
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoQuy]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
	@year [int],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [nvarchar](max),
    @LoaiTien [nvarchar](max)
AS
BEGIN
		set nocount on;
		declare @tblNhomDT table(ID_NhomDoiTuong varchar(40))
		insert into @tblNhomDT
		select * from dbo.splitstring(@ID_NhomDoiTuong_SP)

    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	select 
			b.ID_KhoanThuChi,
			b.KhoanMuc,
			sum(b.Thang1) as Thang1,
			sum(b.Thang2) as Thang2,
			sum(b.Thang3) as Thang3,
			sum(b.Thang4) as Thang4,
			sum(b.Thang5) as Thang5,
			sum(b.Thang6) as Thang6,
			sum(b.Thang7) as Thang7,
			sum(b.Thang8) as Thang8,
			sum(b.Thang9) as Thang9,
			sum(b.Thang10) as Thang10,
			sum(b.Thang11) as Thang11,
			sum(b.Thang12) as Thang12,
			max(STT) as STT

	from
	(
		select 
			a.ID_KhoanThuChi,
			case a.LoaiThuChi
			when 3 then N'Thu tiền bán hàng'
			when 5 then N'Thu trả hàng nhà cung cấp'		
			else case when a.ID_KhoanThuChi is null then N'Thu mặc định' else NoiDungThuChi end end as KhoanMuc,
			case when a.ThangLapHoaDon = 1 then tienthu end as Thang1,
			case when a.ThangLapHoaDon = 2 then tienthu end as Thang2,
			case when a.ThangLapHoaDon = 3 then tienthu end as Thang3,
			case when a.ThangLapHoaDon = 4 then tienthu end as Thang4,
			case when a.ThangLapHoaDon = 5 then tienthu end as Thang5,
			case when a.ThangLapHoaDon = 6 then tienthu end as Thang6,
			case when a.ThangLapHoaDon = 7 then tienthu end as Thang7,
			case when a.ThangLapHoaDon = 8 then tienthu end as Thang8,
			case when a.ThangLapHoaDon = 9 then tienthu end as Thang9,
			case when a.ThangLapHoaDon = 10 then tienthu end as Thang10,
			case when a.ThangLapHoaDon = 11 then tienthu end as Thang11,
			case when a.ThangLapHoaDon = 12 then tienthu end as Thang12		,
			ROW_NUMBER() OVER(ORDER BY a.NoiDungThuChi) as STT		
		from
		(
		select 
    			--a1.ID_NhomDoiTuong,
				a1.LoaiThuChi,			
				a1.ID_KhoanThuChi,
				a1.NoiDungThuChi,
    			a1.ThangLapHoaDon,
				Case when @LoaiTien = '%1%' then a1.TienMat
					when @LoaiTien = '%2%' then a1.TienGui else a1.tienmat + a1.TienGui end as TienThu,
    			Case when a1.TienMat > 0 and TienGui = 0 then '1'  
    			 when a1.TienGui > 0 and TienMat = 0 then '2' 
    			 when a1.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
				select    					
					qhdct.ID_KhoanThuChi,
    				ktc.NoiDungThuChi,
    				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    				Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    				 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    				 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    				 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    				 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    				 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
					case when dt.LoaiDoiTuong= 1 then	
					case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end
					else
						case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000002' else dt.IDNhomDoiTuongs end end as ID_NhomDoiTuong,   
					tienmat, tiengui,
    				tienmat +  tiengui as TienThu,
					DATEPART(MONTH, qhd.NgayLapHoaDon) as ThangLapHoaDon,
    				hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan is null or qhdct.DiemThanhToan= 0)
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH 
				or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or hd.MaHoaDon like @MaKH or hd.MaHoaDon like @MaKH_TV)
    			and qhd.LoaiHoaDon = 11
    			and qhd.HachToanKinhDoanh like @HachToanKD
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') 
				) a1
				where (EXISTS(SELECT Name FROM splitstring(a1.ID_NhomDoiTuong) dtDB inner JOIN @tblNhomDT dtS ON dtDB.Name = dtS.ID_NhomDoiTuong) 
				or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
				--where  (a1.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or a1.ID_NhomDoiTuong like @ID_NhomDoiTuong)
		) a where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) --and a.LoaiTien like @LoaiTien
	) b group by b.ID_KhoanThuChi, b.KhoanMuc
		DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then MAX(b.TienMat)
				when @LoaiTien = '%2%' then MAX(b.TienGui) else
				MAX(b.TienMat + b.TienGui) end as TienThu
				--MAX(b.TienMat + b.TienGui) as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienThu,
				a.TienMat,
				a.TienGui,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case when dt.MaDoiTuong is null then N'khách lẻ' else dt.MaDoiTuong end as MaKhachHang,
    			Case when dt.TenDoiTuong_KhongDau is null then N'khach le' else dt.TenDoiTuong_KhongDau end as TenKhachHang_KhongDau,
    			Case when dt.TenDoiTuong_ChuCaiDau is null then N'kl' else dt.TenDoiTuong_ChuCaiDau end as TenKhachHang_ChuCaiDau,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or hd.MaHoaDon like @MaKH or hd.MaHoaDon like @MaKH_TV)	
    			and qhd.LoaiHoaDon = 12
    			and qhd.HachToanKinhDoanh like @HachToanKD
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') -- DieuChinh CongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID, qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select *
			from
			(
			select max(ID_KhoanThuChi) as ID_KhoanThuChi,
			KhoanMuc, 
			MAX(STT) as STT,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) as Quy1,
			ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) as Quy2,
			ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + ISNULL(SUM(Thang9),0) as Quy3,
			ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as Quy4,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY  KhoanMuc  
			) tblview where TongCong > 0
			order by STT
END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_PhanTichThuChiTheoThang]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
	@year [int],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [nvarchar](max),
    @LoaiTien [nvarchar](max)
AS
BEGIN
    --	tinh ton dau ky
    	Declare @tmp table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		-- thu tiền
    	Insert INTO @tmp
    	SELECT
			ID_KhoanThuChi,
			CASE When c.LoaiThuChi = 3 then N'Thu tiền bán hàng'
			When c.LoaiThuChi = 5 then N'Thu trả hàng nhà cung cấp'
			When ID_KhoanThuChi is null then N'Thu mặc định'
			else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then SUM(b.TienMat)
				when @LoaiTien = '%2%' then SUM(b.TienGui) else
				SUM(b.TienMat + b.TienGui) end as TienThu
    		FROM
    		(
			select 
    		a.ID_NhomDoiTuong,
			a.LoaiThuChi,
			a.ID_HoaDon,
			a.ID_DoiTuong,
			a.ID_KhoanThuChi,
    		a.ThangLapHoaDon,
			a.TienMat,
			a.TienGui,
			a.TienThu,
    		Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			when a.TienGui > 0 and TienMat = 0 then '2' 
    			when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select 
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
				Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			 when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2  -- phiếu chi khác
    			 when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3  -- bán hàng
    			 when hd.LoaiHoaDon = 6  then 4  -- Đổi trả hàng
    			 when hd.LoaiHoaDon = 7 then 5  -- trả hàng NCC
    			 when hd.LoaiHoaDon = 4 then 6 else 7 end as LoaiThuChi, -- nhập hàng NCC
    			--Case When dtn.ID_NhomDoiTuong is null then
    			--Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
				case when dt.IDNhomDoiTuongs is null or dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else dt.IDNhomDoiTuongs end as ID_NhomDoiTuong,
    			max(qhdct.TienMat) as TienMat,
    			max(qhdct.TienGui) as TienGui,
    			max(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon,
    			hd.MaHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			--left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and (qhdct.DiemThanhToan = 0 or qhdct.DiemThanhToan is null)
				and qhd.LoaiHoaDon = 11
    			and qhd.HachToanKinhDoanh like @HachToanKD
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') -- DieuChinh CongNo, khong dau vao BC PhanTichThuChi
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or hd.MaHoaDon like @MaKH or hd.MaHoaDon like @MaKH_TV)
    			Group by qhd.ID, qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi, 
    			qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dt.IDNhomDoiTuongs
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    				and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dkt nvarchar(max);
		set @dkt = (select top(1) KhoanMuc from @tmp)
		if (@dkt is not null)
		BEGIN
		Insert INTO @tmp
		select '00000010-0000-0000-0000-000000000010',
		N'Tổng thu', SUM(Thang1)as Thang1,
		SUM(Thang2) as Thang2,
		SUM(Thang3) as Thang3,
		SUM(Thang4) as Thang4,
		SUM(Thang5) as Thang5,
		SUM(Thang6) as Thang6,
		SUM(Thang7) as Thang7,
		SUM(Thang8) as Thang8,
		SUM(Thang9) as Thang9,
		SUM(Thang10) as Thang10,
		SUM(Thang11) as Thang11,
		SUM(Thang12) as Thang12,
		MAX(STT) + 1 as STT
		from @tmp
		END
		-- chi tiền
		Declare @tmc table (ID_KhoanThuChi uniqueidentifier, KhoanMuc nvarchar(max), Thang1 float, Thang2 float, Thang3 float, Thang4 float, Thang5 float, Thang6 float, Thang7 float, Thang8 float, Thang9 float,Thang10 float,
		Thang11 float, Thang12 float, STT int)
		Insert INTO @tmc
    	SELECT
			ID_KhoanThuChi,
			--CASE When ID_KhoanThuChi is null then N'Chi mặc định' else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When c.LoaiThuChi = 4 then N'Chi đổi trả hàng'
				When c.LoaiThuChi = 6 then N'Chi nhập hàng nhà cung cấp'
				When ID_KhoanThuChi is null then N'Chi mặc định'
				else ktc.NoiDungThuChi end as KhoanMuc,
			CASE When ThangLapHoaDon = 1 then SUM(c.TienThu) END as Thang1,
			CASE When ThangLapHoaDon = 2 then SUM(c.TienThu) END as Thang2,
			CASE When ThangLapHoaDon = 3 then SUM(c.TienThu) END as Thang3,
			CASE When ThangLapHoaDon = 4 then SUM(c.TienThu) END as Thang4,
			CASE When ThangLapHoaDon = 5 then SUM(c.TienThu) END as Thang5,
			CASE When ThangLapHoaDon = 6 then SUM(c.TienThu) END as Thang6,
			CASE When ThangLapHoaDon = 7 then SUM(c.TienThu) END as Thang7,
			CASE When ThangLapHoaDon = 8 then SUM(c.TienThu) END as Thang8,
			CASE When ThangLapHoaDon = 9 then SUM(c.TienThu) END as Thang9,
			CASE When ThangLapHoaDon = 10 then SUM(c.TienThu) END as Thang10,
			CASE When ThangLapHoaDon = 11 then SUM(c.TienThu) END as Thang11,
			CASE When ThangLapHoaDon = 12 then SUM(c.TienThu) END as Thang12,
			ROW_NUMBER() OVER(ORDER BY ktc.NoiDungThuChi ASC) + (select MAX(STT) from @tmp) as STT
    	  FROM 
    		(
    		 SELECT 
				b.ID_KhoanThuChi,
    			b.ThangLapHoaDon,
				b.LoaiThuChi,
				Case when @LoaiTien = '%1%' then MAX(b.TienMat)
				when @LoaiTien = '%2%' then MAX(b.TienGui) else
				MAX(b.TienMat + b.TienGui) end as TienThu
    		FROM
    		(
				select 
    			a.ID_NhomDoiTuong,
				a.LoaiThuChi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.ID_KhoanThuChi,
    			a.ThangLapHoaDon,
				a.TienMat,
				a.TienGui,
				a.TienThu,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' end  as LoaiTien
    		From
    		(
    		select
    			MAX(qhd.ID) as ID_HoaDon,
				qhdct.ID_KhoanThuChi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
    			Case when hd.LoaiHoaDon = 6  then 4 else -- Đổi trả hàng
    			Case when hd.LoaiHoaDon = 7 then 5 else -- trả hàng NCC
    			Case when hd.LoaiHoaDon = 4 then 6 else '' end end end end end end as LoaiThuChi, -- nhập hàng NCC
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			SUM(qhdct.TienMat) as TienMat,
    			SUM(qhdct.TienGui) as TienGui,
    			SUM(qhdct.TienThu) as TienThu,
				MAX(DATEPART(MONTH, qhd.NgayLapHoaDon)) as ThangLapHoaDon
    			From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    			where DATEPART(YEAR, qhd.NgayLapHoaDon) = @year
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or qhd.MaHoaDon like @MaKH or qhd.MaHoaDon like @MaKH_TV)	
				and qhd.LoaiHoaDon = 12
    			and qhd.HachToanKinhDoanh like @HachToanKD
				and (qhd.PhieuDieuChinhCongNo is null or qhd.PhieuDieuChinhCongNo = '0') -- dcCongNo, khong dau vao BC PhanTichThuChi
    			Group by qhd.ID,qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, qhdct.ID_KhoanThuChi,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			and LoaiTien like @LoaiTien
    			Group by b.ID_KhoanThuChi, b.ThangLapHoaDon, b.ID_DoiTuong, b.ID_HoaDon, b.LoaiThuChi
    		) as c
			left join Quy_KhoanThuChi ktc on c.ID_KhoanThuChi = ktc.ID
			Group by c.ID_KhoanThuChi, c.ThangLapHoaDon, ktc.NoiDungThuChi, c.LoaiThuChi
		DECLARE @dk nvarchar(max);
		set @dk = (select top(1) KhoanMuc from @tmc)
		if (@dk is not null)
		BEGIN
		Insert INTO @tmp
			select *
			from @tmc
		Insert INTO @tmp
			select 
			'00000030-0000-0000-0000-000000000030',
			N'Tổng chi', 
			SUM(Thang1)as Thang1,
			SUM(Thang2) as Thang2,
			SUM(Thang3) as Thang3,
			SUM(Thang4) as Thang4,
			SUM(Thang5) as Thang5,
			SUM(Thang6) as Thang6,
			SUM(Thang7) as Thang7,
			SUM(Thang8) as Thang8,
			SUM(Thang9) as Thang9,
			SUM(Thang10) as Thang10,
			SUM(Thang11) as Thang11,
			SUM(Thang12) as Thang12,
			MAX(STT) + 1 as STT
			from @tmc
		END
			select max(ID_KhoanThuChi) as ID_KhoanThuChi, -- deu chi tien nhaphang, nhưng ID_KhoanThuChi # nhau --> thi bi douple, nen chi group tho KhoanMua va lay max (ID_KhoanThuChi)
			KhoanMuc, 
			CAST(ROUND(SUM(Thang1), 0) as float) as Thang1,
			CAST(ROUND(SUM(Thang2), 0) as float) as Thang2,
			CAST(ROUND(SUM(Thang3), 0) as float) as Thang3,
			CAST(ROUND(SUM(Thang4), 0) as float) as Thang4,
			CAST(ROUND(SUM(Thang5), 0) as float) as Thang5,
			CAST(ROUND(SUM(Thang6), 0) as float) as Thang6,
			CAST(ROUND(SUM(Thang7), 0) as float) as Thang7,
			CAST(ROUND(SUM(Thang8), 0) as float) as Thang8,
			CAST(ROUND(SUM(Thang9), 0) as float) as Thang9,
			CAST(ROUND(SUM(Thang10), 0) as float) as Thang10,
			CAST(ROUND(SUM(Thang11), 0) as float) as Thang11,
			CAST(ROUND(SUM(Thang12), 0) as float) as Thang12,
			ISNULL(SUM(Thang1),0) + ISNULL(SUM(Thang2),0) + ISNULL(SUM(Thang3),0) + ISNULL(SUM(Thang4),0) + ISNULL(SUM(Thang5),0) + ISNULL(SUM(Thang6),0) + ISNULL(SUM(Thang7),0) + ISNULL(SUM(Thang8),0) + 
			ISNULL(SUM(Thang9),0) + ISNULL(SUM(Thang10),0) + ISNULL(SUM(Thang11),0) + ISNULL(SUM(Thang12),0) as TongCong
			from @tmp
			GROUP BY  KhoanMuc
			order by MAX(STT)

END");

			Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_ThuChi]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [nvarchar](max),
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max),
    @lstThuChi [nvarchar](max),
    @HachToanKD [nvarchar](max)
AS
BEGIN
SET NOCOUNT ON;
    SELECT 
    MAX(b.TenNhomDoiTuong) as NhomDoiTuong,
    b.MaHoaDon,
    MAX(b.MaPhieuThu) as MaPhieuThu,
    MAX(b.NgayLapHoaDon) as NgayLapHoaDon,
    MAX(b.ManguoiNop) as ManguoiNop, 
    MAX(b.TenNguoiNop) as TenNguoiNop, 
    MAX(b.ThuChi) as ThuChi, 
    MAX(b.NoiDungThuChi) as NoiDungThuChi,
    MAX(b.GhiChu) as GhiChu,
    MAX(b.LoaiThuChi) as LoaiThuChi
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
		a.TienMat + a.TienGui as ThuChi,
    	a.NoiDungThuChi,
    	a.GhiChu,
    	Case when a.LoaiThuChi = 1 then N'Phiếu thu khác'  
    	when a.LoaiThuChi = 2 then N'Phiếu chi khác' 
    	when a.LoaiThuChi = 3 then N'Thu tiền khách trả'  
    	when a.LoaiThuChi = 4 then N'Chi tiền đổi trả hàng'  
    	when a.LoaiThuChi = 5 then N'Thu tiền nhà NCC'  
    	when a.LoaiThuChi = 6 then N'Chi tiền trả NCC' else '' end as LoaiThuChi
    	From
    	(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
    			MAX(ktc.NoiDungThuChi) as NoiDungThuChi,
    			MAX (tknh.SoTaiKhoan) as SoTaiKhoan,
    			MAX (nh.TenNganHang) as NganHang,
				Max(dt.TenNhomDoiTuongs) as TenNhomDoiTuong,
    			qhd.HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 -- phiếu thu khác
    			when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2-- phiếu chi khác
    			when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19 or hd.LoaiHoaDon = 22) and qhd.LoaiHoaDon = 11 then 3 -- bán hàng 
    			when hd.LoaiHoaDon = 6  then 4 -- Đổi trả hàng
    			when hd.LoaiHoaDon = 7 then 5 -- trả hàng NCC
    			when hd.LoaiHoaDon = 4 then 6 else ''end as LoaiThuChi, -- nhập hàng NCC
    			dt.MaDoiTuong as MaKhachHang,
    			Case When dtn.ID_NhomDoiTuong is null then
    			Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong,
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			qhd.NguoiNopTien as TenNguoiNop,
				case when qhdct.ID_NhanVien is not null then nv.MaNhanVien else dt.MaDoiTuong end as ManguoiNop,
    			Sum(qhdct.TienMat) as TienMat,
    			Sum(qhdct.TienGui) as TienGui,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon
    		From Quy_HoaDon qhd 			
    			join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
				join NS_NhanVien nv on qhd.ID_NhanVien= nv.ID
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
    		where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd 
				and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
				and (qhd.PhieuDieuChinhCongNo !='1' or qhd.PhieuDieuChinhCongNo is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
				and (dt.DienThoai like @MaKH or dt.TenDoiTuong_ChuCaiDau like @MaKH or dt.TenDoiTuong_KhongDau like @MaKH or dt.MaDoiTuong like @MaKH or dt.MaDoiTuong like @MaKH_TV or qhd.MaHoaDon like @MaKH or qhd.MaHoaDon like @MaKH_TV)	
    			and qhd.HachToanKinhDoanh like @HachToanKD
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, qhdct.ID_NhanVien, dt.MaDoiTuong,dt.LoaiDoiTuong,  nv.MaNhanVien,
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong,dtn.ID
    		)a
    		where a.LoaiThuChi in (select * from splitstring(@lstThuChi)) 
    	) b
		where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong
    	Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon
    	ORDER BY NgayLapHoaDon DESC
END
");

			Sql(@"ALTER PROCEDURE [dbo].[GetListLichHen_FullCalendar_Dashboard]
    @IDChiNhanhs [nvarchar](max),
    @PhanLoai [nvarchar](20),
	@FromDate datetime
AS
BEGIN
    SET NOCOUNT ON;
    	DECLARE @DateNow datetime= GETDATE()
    	--declare @FromDate datetime= convert(varchar(14), @DateNow,23)
    	declare @ToDate datetime= convert(varchar(14), dateadd(DAY,1,@FromDate),23)
    
    	--DECLARE @tblNhanVien table (ID uniqueidentifier, MaNhanVien nvarchar(max), TenNhanVien nvarchar(max), DienThoaiDiDong nvarchar(max), GioiTinh bit null, ID_DonVi uniqueidentifier)
    	--insert into @tblNhanVien
    	--exec GetInForStaff_Working_byChiNhanh @IDChiNhanhs ;
    
    	declare @tblCalendar table(ID uniqueidentifier, ID_DonVi uniqueidentifier, ID_NhanVien uniqueidentifier, NgayHenGap datetime, TrangThai varchar(10))
    	
    
    --- table LichHen
    select cs.ID, 
    	Ma_TieuDe,
    	cs.ID_DonVi, 
    	ID_KhachHang, 
    	ID_LoaiTuVan,
    	ID_NhanVien,	
    	ID_NhanVienQuanLy,
    	NgayTao,
    	NgayGio,
    	NgayGioKetThuc,	
    	NgayHoanThanh,
    	ISNULL(KieuLap,0) as KieuLap,
    	ISNULL(SoLanLap,0) as SoLanLap,
    	ISNULL(GiaTriLap,'') as GiaTriLap,
    	ISNULL(TuanLap,0) as TuanLap,
    	ISNULL(TrangThaiKetThuc,0) as TrangThaiKetThuc,
    	ISNULL(GiaTriKetThuc,'') as GiaTriKetThuc, 	
    	ISNULL(SoLanDaHen,0) as SoLanDaHen,
    	TrangThai,
    	ISNULL(GhiChu,'') as GhiChu,
    	NguoiTao,
    	2 as MucDoUuTien,
    	KetQua,
    	NhacNho, 
    	ISNULL(KieuNhacNho,0) as KieuNhacNho,
    	cs.ID as ID_Parent,
    	cs.NgayCu
    into #temp
    from ChamSocKhachHangs cs
    left join ( select ISNULL(ID_Parent,'00000000-0000-0000-0000-000000000000') as ID_Parent,
    		count(*) as SoLanDaHen
    		from ChamSocKhachHangs
    		where PhanLoai = 3
    		group by ID_Parent) a on cs.ID= a.ID_Parent
    where KieuLap in (1,2,3,4,0)
    	and (TrangThaiKetThuc = 1 
    	OR (TrangThaiKetThuc = 2 and ISNULL(GiaTriKetThuc,'')  >= CONVERT(varchar, @FromDate,23))
    	OR (TrangThaiKetThuc = 3 and ISNULL(SoLanDaHen,0)  <= ISNULL(GiaTriKetThuc,0)) 
    	)	
    and PhanLoai = 3 
    
    -- get row was update (ID_Parent !=null)
    select ID, ID_Parent, NgayCu into #temp2 from ChamSocKhachHangs where ID_Parent is not null and PhanLoai = 3
    
    set nocount on;
    declare @ID uniqueidentifier, @Ma_TieuDe nvarchar(max), @ID_DonVi uniqueidentifier, @ID_KhachHang uniqueidentifier,@ID_LoaiTuVan uniqueidentifier, 
    		@ID_NhanVien uniqueidentifier,@ID_NhanVienQuanLy uniqueidentifier,
    		@NgayTao datetime,@NgayGio datetime,@NgayGioKetThuc datetime, @NgayHoanThanh datetime,
    		@KieuLap int, @SoLanLap int, @GiaTriLap varchar(max), @TuanLap int, @TrangThaiKetThuc int,@GiaTriKetThuc varchar(max),			
    		@SoLanDaHen int, @TrangThai varchar, @GhiChu nvarchar(max),
    		@NguoiTao nvarchar(max), @MucDoUuTien int, @KetQua nvarchar(max), @NhacNho int, @KieuNhacNho int, @ID_Parent uniqueidentifier, @NgayCu datetime
    
    		--- lap ngay
    		declare _cur cursor
    		for
    			select * from #temp where KieuLap = 1 and SoLanLap > 0	
    				and not exists (select ID from #temp2 where #temp2.ID = #temp.ID) --and TrangThai='1' 
    		open _cur
    		fetch next from _cur
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc, @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc, @SoLanDaHen,@TrangThai,@GhiChu,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin		
    				-- chi add row < @ToDate
    				declare @dateadd datetime = @NgayGio
    				declare @lanlap int = 1			
    				while @dateadd < @ToDate 
    					begin	
    					
    						if @TrangThaiKetThuc= 1 
    							OR (@TrangThaiKetThuc = 2 and  @dateadd < @GiaTriKetThuc )  --- khong bao gio OR KetThuc vao ngay OR sau x lan (todo)
    							OR (@TrangThaiKetThuc= 3 and @lanlap <= @GiaTriKetThuc - @SoLanDaHen)
    							begin
    								set @NgayGioKetThuc = DATEADD(hour,4,@dateadd)
    								declare @newidDay uniqueidentifier = NEWID()
    								declare @count1 int = 0;
    								if @dateadd = @NgayGio set @newidDay = @ID		
    								select @count1 = count(*) from #temp2 where ID_Parent = @ID_Parent 
    									and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@dateadd,23)								
    								if @count1 = 0											
    									insert into @tblCalendar values (@newidDay, @ID_DonVi, @ID_NhanVien,@dateadd, @TrangThai)																			
    							end
    						set @dateadd = DATEADD(day, @SoLanLap, @dateadd)
    						set @lanlap= @lanlap + 1
    					end
    				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap,@TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho,@ID_Parent, @NgayCu
    			end
    		close _cur;
    		deallocate _cur;
    
    		--- lap tuan
    		declare _cur2 cursor
    		for
    			select * from #temp where KieuLap = 2 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID)  --and TrangThai='1' 
    		open _cur2
    		fetch next from _cur2
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin	
    				declare @weekRepeat datetime = @NgayGio				
    				declare @lanlapWeek int = 1
    				while @weekRepeat < @ToDate -- lặp đến khi thuộc khoảng thời gian tìm kiếm
    					begin	
    
    								declare @firstOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 0)) -- lay ngay dau tien cua tuan
    								declare @lastOfWeek datetime = (select  dateadd(WEEK, datediff(WEEK, 0, @weekRepeat), 7)) -- lay ngay cuoi cung cua tuan
    								declare @dateRepeat datetime = @firstOfWeek	
    								while @dateRepeat < @lastOfWeek -- tim kiem trong tuan duoc lap lai
    									begin
    										if dateadd(hour,23, @dateRepeat) >= @NgayGio
    											begin
    												declare @dateOfWeek varchar(1) = cast(DATEPART(WEEKDAY,@dateRepeat) as varchar(1)) -- lấy ngày trong tuần (thứ 2,3,..)	
    												if @dateOfWeek = 1 set @dateOfWeek = 8
    												declare @datefrom datetime = @dateRepeat
    												set @NgayGioKetThuc = DATEADD(hour,2,@datefrom) -- add 2 hour
    
    												if CHARINDEX(@dateOfWeek, @GiaTriLap ) > 0 
    												and  (@TrangThaiKetThuc= 1 OR (@TrangThaiKetThuc = 2 and  @dateRepeat < @GiaTriKetThuc)
    													OR (@TrangThaiKetThuc= 3 and @lanlapWeek <= @GiaTriKetThuc - @SoLanDaHen))
    													begin														
    														declare @newidWeek uniqueidentifier = NEWID()
    														declare @exitDB bit='0'
    														if convert(varchar(20),@dateRepeat,23) = convert(varchar(20),@NgayGio,23) 
    															begin
    																set @newidWeek = @ID
    																set @exitDB ='1'
    															end
    														declare @count2 int=0
    														select @count2 = count(*) from #temp2 where ID_Parent = @ID_Parent 
    																and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@dateRepeat,23)								
    														if @count2 = 0	
    															begin
    																insert into @tblCalendar values (@newidWeek, @ID_DonVi, @ID_NhanVien, @dateRepeat, @TrangThai)	
    																set @lanlapWeek= @lanlapWeek + 1																
    															end
    													end												
    											end										
    										set @dateRepeat = DATEADD(day, 1, @dateRepeat)											
    									end							
    						set @weekRepeat = DATEADD(WEEK, @SoLanLap, @weekRepeat)	-- lap lai x tuan/lan	
    					end			
    				FETCH NEXT FROM _cur2 into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    			end
    		close _cur2;
    		deallocate _cur2;
    
    		--- lap thang
    		declare _cur cursor
    		for
    			select * from #temp where KieuLap = 3 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID)
    		open _cur
    		fetch next from _cur
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin		
    				declare @monthRepeat datetime = @NgayGio	
    				declare @lanlapMonth int = 1
    				while @monthRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
    					begin	
    						if  @monthRepeat >= @FromDate			
    							begin	
    								declare @datefromMonth datetime= @monthRepeat
    								set @NgayGioKetThuc = DATEADD(hour,2,@datefromMonth)
    								 -- hàng tháng vào ngày ..xx..
    								if	@TuanLap = 0 
    									and (@TrangThaiKetThuc = 1 
    									OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
    									OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
    									)
    									begin
    											declare @newidMonth1 uniqueidentifier = NEWID()
    											if @monthRepeat = @NgayGio set @newidMonth1 = @ID
    											declare @count3 int=0
    											select @count3 = count(*) from #temp2 where ID_Parent = @ID_Parent 
    													and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@monthRepeat,23)								
    											if @count3 = 0	
    												insert into @tblCalendar values (@newidMonth1, @ID_DonVi, @ID_NhanVien, @monthRepeat, @TrangThai)	
    									end 
    								else
    									-- hàng tháng vào thứ ..x.. tuần thứ ..y.. của tháng
    									begin
    										declare @dateOfWeek_Month int = DATEPART(WEEKDAY,@monthRepeat) -- thu may trong tuan
    										if @dateOfWeek_Month = 1 set @dateOfWeek_Month = 8
    										declare @weekOfMonth int = DATEDIFF(WEEK, DATEADD(MONTH, DATEDIFF(MONTH, 0, @monthRepeat), 0), @monthRepeat) +1 -- tuan thu may cua thang
    										if @dateOfWeek_Month = @GiaTriLap and @weekOfMonth = @TuanLap 
    										and (@TrangThaiKetThuc = 1 
    											OR (@TrangThaiKetThuc = 2 and @monthRepeat < @GiaTriKetThuc)
    											OR (@TrangThaiKetThuc= 3 and @lanlapMonth <= @GiaTriKetThuc - @SoLanDaHen)
    											)
    											begin
    												declare @newidMonth2 uniqueidentifier = NEWID()
    												if @monthRepeat = @NgayGio set @newidMonth2 = @ID
    												declare @count4 int=0
    												select @count4 = count(*) from #temp2 where ID_Parent = @ID_Parent 
    														and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@monthRepeat,23)								
    												if @count4 = 0	
    													insert into @tblCalendar values (@newidMonth2, @ID_DonVi, @ID_NhanVien, @monthRepeat, @TrangThai)
    											end
    									end						
    							end
    						set @monthRepeat = DATEADD(MONTH, @SoLanLap, @monthRepeat)	-- lap lai x thang/lan	
    						set @lanlapMonth = @lanlapMonth +1
    					end			
    				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    			end
    		close _cur;
    		deallocate _cur;
    
    		--- lap nam
    		declare _cur cursor
    		for
    			select * from #temp where KieuLap = 4 and SoLanLap > 0	and not exists (select ID from #temp2 where #temp2.ID = #temp.ID) 
    		open _cur
    		fetch next from _cur
    		into @ID, @Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan, @ID_NhanVien,@ID_NhanVienQuanLy,
    			@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    			@KieuLap, @SoLanLap, @GiaTriLap,@TuanLap, @TrangThaiKetThuc,@GiaTriKetThuc,  @SoLanDaHen,@TrangThai,@GhiChu,
    			@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    		while @@FETCH_STATUS = 0
    			begin		
    				declare @yearRepeat datetime = @NgayGio	
    				declare @lanlapYear int = 1
    				while @yearRepeat < @ToDate -- lặp trong khoảng thời gian tìm kiếm
    					begin						
    						if  @yearRepeat >= @FromDate			
    							begin	
    								declare @dateOfMonth int = datepart(day,@yearRepeat)
    								declare @monthOfYear int = datepart(MONTH,@yearRepeat)
    								set @NgayGioKetThuc= DATEADD(hour,2, @yearRepeat)
    
    								if @dateOfMonth = @GiaTriLap and @monthOfYear= @TuanLap
    									and (@TrangThaiKetThuc = 1 
    										OR (@TrangThaiKetThuc = 2 and @yearRepeat < @GiaTriKetThuc)
    										OR (@TrangThaiKetThuc = 3 and @lanlapYear <= @GiaTriKetThuc - @SoLanDaHen)
    										)
    									begin
    										declare @newidYear uniqueidentifier = NEWID()										
    										if @yearRepeat = @NgayGio set @newidYear = @ID
    										declare @count5 int=0
    										select @count5 = count(*) from #temp2 where ID_Parent = @ID_Parent 
    												and CONVERT(varchar(14),NgayCu,23) = CONVERT(varchar(14),@yearRepeat,23)								
    										if @count5 = 0	
    											insert into @tblCalendar values (@newidYear, @ID_DonVi,  @ID_NhanVien,@yearRepeat, @TrangThai)
    									end
    							end
    						set @yearRepeat = DATEADD(YEAR, @SoLanLap, @yearRepeat)	-- lap lai x nam/lan	
    						set @lanlapYear = @lanlapYear +1
    					end			
    				FETCH NEXT FROM _cur into @ID,@Ma_TieuDe, @ID_DonVi, @ID_KhachHang,@ID_LoaiTuVan,@ID_NhanVien,@ID_NhanVienQuanLy,
    					@NgayTao, @NgayGio, @NgayGioKetThuc,  @NgayHoanThanh,
    					@KieuLap, @SoLanLap,@GiaTriLap, @TuanLap, @TrangThaiKetThuc, @GiaTriKetThuc, @SoLanDaHen, @TrangThai,@GhiChu,
    					@NguoiTao, @MucDoUuTien, @KetQua, @NhacNho, @KieuNhacNho, @ID_Parent, @NgayCu
    			end
    		close _cur;
    		deallocate _cur;
    	
    	-- add LichHen da duoc update (SoLanLap = 0)
    	insert into @tblCalendar
    	select ID, 
    		ID_DonVi, 
    		ID_NhanVien,	
    		NgayGio,
    		TrangThai
    	from #temp 
    	where (SoLanLap= 0 OR TrangThai !='1'  or KieuLap=0
    		Or exists (select ID from #temp2 where #temp2.ID = #temp.ID)
    		)
    		and NgayGio>= @FromDate and NgayGio < @ToDate
    	
    	drop table #temp 
    	drop table #temp2 
    
    	-- select --> union
    	select b.*
    	from
    		(select *
    		from
    			(-- lichhen
    			select ID,
    				ID_DonVi, 
    				ID_NhanVien,
    				NgayHenGap as NgayGio,
    				3 as PhanLoai
    			from @tblCalendar
    			where exists (select Name from dbo.splitstring(@IDChiNhanhs) where Name = ID_DonVi) 
    			and TrangThai='1'
    			and NgayHenGap >= @FromDate and NgayHenGap < @ToDate
    
    			union all
    			-- cong viec
    			select 
    					cs.ID,
    					cs.ID_DonVi, 
    					ID_NhanVien,
    					NgayGio,
    					4 as PhanLoai
    				from ChamSocKhachHangs cs
    				where exists (select Name from dbo.splitstring(@IDChiNhanhs) where Name = cs.ID_DonVi)
    				and PhanLoai= 4
    				and TrangThai='1'
    				and NgayGio >= @FromDate and NgayGio < @ToDate
    				) a			
    		)b
    		where b.PhanLoai like @PhanLoai
    		order by NgayGio
END");

			Sql(@"ALTER PROCEDURE [dbo].[GetLuongCoDinh_OrLuongNgayCong]
	@NgayCongChuan int,
	@tblCongThuCong CongThuCong readonly,
	@tblThietLapLuong ThietLapLuong readonly
AS
BEGIN

		SET NOCOUNT ON;	
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
				else LuongCoBan/@NgayCongChuan * sum(SoNgayDiLam)
				end as ThanhTien
			from @tblCongCDNgay luongcd		
			group by ID_NhanVien, LoaiLuong,LuongCoBan,NgayApDung, NgayKetThuc
		
END");

			Sql(@"ALTER PROCEDURE [dbo].[insert_HoaDon_ChiTietXuatKho]
    @ID_HoaDon [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @SoLuong [float],
    @DonGia [float],
    @ThanhTien [float],
    @GiaVon [float],
    @LoaiIS [int],
    @SoThuTu [int],
	@GhiChu [nvarchar](max),
	@ID_ChiTietGoiDV uniqueidentifier
AS
BEGIN
    IF (@LoaiIS = 1)
    	BEGIN
    		insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang, GhiChu, ID_ChiTietGoiDV)
    		Values (NEWID(), @ID_HoaDon, @SoThuTu, @SoLuong, @DonGia, @ThanhTien, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi, @ID_LoHang, @GhiChu, @ID_ChiTietGoiDV)
    	END
    	ELSE
    	BEGIN
    		insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, GhiChu, ID_ChiTietGoiDV)
    		Values (NEWID(), @ID_HoaDon, @SoThuTu, @SoLuong, @DonGia, @ThanhTien, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi, @GhiChu, @ID_ChiTietGoiDV)
    	END
END");

			Sql(@"ALTER PROCEDURE [dbo].[insert_HoaDonXuatKho]
    @ID [uniqueidentifier],
    @ID_DonVi [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @MaHoaDon [nvarchar](max),
    @LoaiHoaDon [int],
    @TongTienHang [float],
	@ID_HoaDon uniqueidentifier null,
	@ID_PhieuTiepNhan uniqueidentifier null,
    @timeCreate [datetime],
    @NguoiTao [nvarchar](max),
    @DienGiai [nvarchar](max),
    @YeuCau [nvarchar](max),
    @ChoThanhToan [bit]
AS
BEGIN
    insert into BH_HoaDon (ID, MaHoaDon, NgayLapHoaDon,ID_NhanVien, LoaiHoaDon, ChoThanhToan, TongTienHang, ID_HoaDon, ID_PhieuTiepNhan,
    	TongChietKhau, TongTienThue, TongGiamGia, TongChiPhi, PhaiThanhToan, DienGiai,YeuCau, NguoiTao, NgayTao, ID_DonVi, TyGia)
    Values (@ID, @MaHoaDon, @timeCreate, @ID_NhanVien, @LoaiHoaDon, @ChoThanhToan, @TongTienHang, @ID_HoaDon, @ID_PhieuTiepNhan,
    	 '0', '0', '0', '0', @TongTienHang, @DienGiai,@YeuCau, @NguoiTao, @timeCreate, @ID_DonVi, '1')
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_CheckExist_ChietKhauDoanhThuNhanVien]
    @ID_DonVi varchar(40),
    @ID_NhanViens varchar(max),
    @ApDungTuNgay varchar(10),
    @ApDungDenNgay varchar(10),
    @ID_ChietKhauDoanhThu varchar(40),
	@TinhChietKhauTheo int,
	@LoaiNhanVienApDung int
AS
BEGIN
    DECLARE @ID_ChietKhauDTSearch varchar(40) = @ID_ChietKhauDoanhThu
    	DECLARE @Count int -- đếm số bản ghi có trong bảng #temp
    
    	SELECT tbl.ID, tbl.ID_NhanVien, tbl.MaNhanVien, tbl.TenNhanVien, '' as TenNhanVien_GC, '' as TenNhanVien_CV,ApDungTuNgay,ApDungDenNgay into #temp 
    	FROM
    			(select ck.ID, ct.ID_NhanVien, nv.TenNhanVien, nv.MaNhanVien, ck.ApDungTuNgay, ck.ApDungDenNgay				
    			from ChietKhauDoanhThu ck
    			join ChietKhauDoanhThu_NhanVien ct on ck.id= ct.ID_ChietKhauDoanhThu
    			join NS_NhanVien nv on ct.ID_NhanVien= nv.ID
    			where ck.ID_DonVi like @ID_DonVi
    			and ck.TrangThai !='0'
				and LoaiNhanVienApDung = @LoaiNhanVienApDung
				-- Neu DenNgay(DB)=null --> alway exist
				-- Neu DenNgay(DB)!=null--> compare TuNgay(new) with DenNgay (DB)
				and (ck.ApDungDenNgay is null 
				or ck.ApDungDenNgay is not null 
					and ((convert(varchar,ck.ApDungDenNgay,112) >= @ApDungTuNgay and convert(varchar,ck.ApDungDenNgay,112) <= @ApDungDenNgay)
						or ( convert(varchar,ck.ApDungTuNgay,112) <= @ApDungDenNgay and convert(varchar,ck.ApDungTuNgay,112) >= @ApDungTuNgay))   			
    			)) tbl
    			WHERE tbl.ID_NhanVien in (select * from splitstring(@ID_NhanViens))	
    	    	
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
    		
END");

			Sql(@"ALTER PROCEDURE [dbo].[SP_PhieuThu_ServicePackage]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    Select 
    	MAX(a.MaHoaDon) as MaHoaDon,
    	CONVERT(varchar, MAX(a.NgayLapHoaDon),103) as NgayLapHoaDon,
    	SUM(a.TienMat) as TienMat,
    	SUM(a.TienGui) as TienGui,
    	SUM(a.TienThu) as TongThu,
    	MAX(a.NoiDungThu) as NoiDungThu,
		CASE WHEN MAX(a.LoaiHoaDon) =1 THEN N'Thu tiền bán hàng'
		ELSE N'Thu tiền bán gói dịch vụ'
		END AS LoaiThuChi
    FROM
    (
    	Select MAX(qhd.ID) as ID_QuyHoaDon, 
    		MAX(qhd.MaHoaDon) as MaHoaDon, 
    		MAX(qhd.NgayLapHoaDon) as NgayLapHoaDon,
    		MAX(ISNULL(qct.TienMat, 0)) as TienMat,
    		MAX(ISNULL(qct.TienGui, 0)) as TienGui,
    		MAX(ISNULL(qct.TienThu, 0)) as TienThu, 
    		MAX(qhd.NoiDungThu) as NoiDungThu,
			MAX(hd.LoaiHoaDon) as LoaiHoaDon
    	from Quy_HoaDon qhd 
    	join Quy_HoaDon_ChiTiet qct on qhd.ID = qct.ID_HoaDon
    	join BH_HoaDon hd on qct.ID_HoaDonLienQuan = hd.ID
    	join BH_HoaDon_ChiTiet hdct on hdct.ID_HoaDon = hd.ID
    	where hd.LoaiHoaDon = 19 
		and qhd.TrangThai= 1
    	and hd.ID_DoiTuong like @ID_DoiTuong and hd.ID_DonVi like @ID_DonVi
		--and not exists(select id from BH_HoaDon dathang where hd.ID_HoaDon= dathang.ID)
		--and hdct.ChatLieu='4'
    	group by qct.ID
    )a
    GROUP BY a.ID_QuyHoaDon
END");

			Sql(@"ALTER PROCEDURE [dbo].[update_HoaDon_ChiTietXuatKho]
    @ID_HoaDon [uniqueidentifier],
    @ID_DonViQuiDoi [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @SoLuong [float],
    @DonGia [float],
    @ThanhTien [float],
    @GiaVon [float],
    @LoaiIS [int],
    @SoThuTu [int],
    @DieuKienXoa [int],
	@GhiChu [nvarchar](max),
	@ID_ChiTietGoiDV uniqueidentifier
AS
BEGIN
    IF (@DieuKienXoa = 0)
    	BEGIN
    		delete BH_HoaDon_ChiTiet where ID_HoaDon = @ID_HoaDon
    	END
    	IF (@LoaiIS = 1)
    	BEGIN
    		insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau, TienThue, PTChiPhi, TienChiPhi,
			ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, ID_LoHang, GhiChu, ID_ChiTietGoiDV)
    		Values (NEWID(), @ID_HoaDon, @SoThuTu, @SoLuong, @DonGia, @ThanhTien, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi, @ID_LoHang, @GhiChu, @ID_ChiTietGoiDV)
    	END
    	ELSE
    	BEGIN
    		insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, ThanhTien, PTChietKhau, TienChietKhau,
			TienThue, PTChiPhi, TienChiPhi, ThanhToan, GiaVon, An_Hien, ID_DonViQuiDoi, GhiChu, ID_ChiTietGoiDV)
    		Values (NEWID(), @ID_HoaDon, @SoThuTu, @SoLuong, @DonGia, @ThanhTien, '0', '0','0','0','0','0',@GiaVon,'0',@ID_DonViQuiDoi, @GhiChu, @ID_ChiTietGoiDV)
    	END
END");

			Sql(@"ALTER PROCEDURE [dbo].[update_HoaDonXuatKho]
    @ID [uniqueidentifier],
    @ID_NhanVien [uniqueidentifier],
    @TongTienHang [float],
	@ID_HoaDon uniqueidentifier null,
	@ID_PhieuTiepNhan uniqueidentifier null,
    @timeCreate [datetime],
    @NguoiTao [nvarchar](max),
    @DienGiai [nvarchar](max),
    @YeuCau [nvarchar](max),
    @ChoThanhToan [bit]
AS
BEGIN
    update BH_HoaDon set ID_NhanVien = @ID_NhanVien, TongTienHang = @TongTienHang, NgayLapHoaDon = @timeCreate, NguoiSua = @NguoiTao,
    	DienGiai = @DienGiai, YeuCau = @YeuCau, ChoThanhToan = @ChoThanhToan, NgaySua = GETDATE(),
		ID_HoaDon= @ID_HoaDon, ID_PhieuTiepNhan= @ID_PhieuTiepNhan
    	where ID = @ID
END");

			Sql(@"
IF object_id('[dbo].[KhoiTaoDuLieuLanDau]') IS NULL
	EXEC ('create procedure [dbo].[KhoiTaoDuLieuLanDau] as select 1')
GO
ALTER PROCEDURE [dbo].[KhoiTaoDuLieuLanDau]
	@Subdomain NVARCHAR(MAX)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ThoiGian DATETIME;
	SET @ThoiGian = GETDATE();
	DECLARE @IDNganhNgheKinhDoanh UNIQUEIDENTIFIER;
	DECLARE @TenCuaHang NVARCHAR(max), @DiaChi NVARCHAR(max), @Email NVARCHAR(max), @SoDienThoai NVARCHAR(max), @TenNhanVien NVARCHAR(max),
	@TaiKhoan NVARCHAR(MAX), @MatKhau NVARCHAR(MAX);
	SELECT 
	@TenCuaHang = IIF(TenCuaHang != '', TenCuaHang, 'Open24.vn'), 
	@DiaChi = IIF(DiaChi != '', DiaChi, 'Open24.vn'), 
	@Email = IIF(Email != '', Email, ''),
	@SoDienThoai = IIF(SoDienThoai != '', SoDienThoai, ''),
	@TenNhanVien = IIF(HoTen != '', HoTen, 'Open24.vn'),
	@TaiKhoan = UserKT,
	@MatKhau = MatKhauKT,
	@IDNganhNgheKinhDoanh = ID_NganhKinhDoanh
	FROM BANHANG24..CuaHangDangKy WHERE SubDomain = @Subdomain;
	--INSERT HT_CongTy
	INSERT INTO HT_CongTy (ID, TenCongTy, DiaChi, SoDienThoai, SoFax, MaSoThue, Mail, Website, TenGiamDoc, TenKeToanTruong, Logo, GhiChu, 
	TaiKhoanNganHang, ID_NganHang, DiaChiNganHang, TenVT, DiaChiVT, DangHoatDong, DangKyNhanSu, NgayCongChuan)
	VALUES ('4DE12030-B7FE-487A-B6B2-A99665E8AE7C', @TenCuaHang, @DiaChi, @SoDienThoai, '', '', @Email, '', '', '', NULL, '',
	'', NULL, '', @TenCuaHang, @DiaChi, 1, 0, 26);
	--INSERT DM_DonVi
	DECLARE @IDDonVi UNIQUEIDENTIFIER;
	SET @IDDonVi = 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE';
	INSERT INTO DM_DonVi (ID, MaDonVi, TenDonVi, DiaChi, Website, MaSoThue, SoTaiKhoan, SoDienThoai, SoFax, KiTuDanhMa, HienThi_Chinh, HienThi_Phu,
	NgayTao, NguoiTao, TrangThai)
	VALUES (@IDDonVi, 'CTY', @TenCuaHang, @DiaChi, @Subdomain + '.open24.vn', '', '', @SoDienThoai, '', @Subdomain,1, 1, @ThoiGian, 'ssoftvn', 1);
	--INSERT NS_NhanVien
	DECLARE @IDNhanVien UNIQUEIDENTIFIER;
	SET @IDNhanVien = NEWID();
	INSERT INTO NS_NhanVien (ID, MaNhanVien, TenNhanVien, GioiTinh, DienThoaiDiDong, Email, CapTaiKhoan, DaNghiViec, NguoiTao, NgayTao, TrangThai)
	VALUES (@IDNhanVien, 'NV01', @TenNhanVien, 1, @SoDienThoai, @Email, 1, 0, 'ssoftvn', @ThoiGian, 1);
	--INSERT QuaTrinhCongTac
	INSERT INTO NS_QuaTrinhCongTac (ID, ID_NhanVien, ID_DonVi, NgayApDung, LaChucVuHienThoi, LaDonViHienThoi, NguoiLap, NgayLap)
	VALUES (NEWID(), @IDNhanVien, @IDDonVi, @ThoiGian, 1, 1, 'ssoftvn', @ThoiGian);
	--INSERT HT_NguoiDung
	DECLARE @IDNguoiDung UNIQUEIDENTIFIER;
	SET @IDNguoiDung = '28FEF5A1-F0F2-4B94-A4AD-081B227F3B77';
	INSERT INTO HT_NguoiDung (ID, ID_NhanVien, TaiKhoan, MatKhau, LaNhanVien, LaAdmin, DangHoatDong, IsSystem, NguoiTao, NgayTao, ID_DonVi, XemGiaVon, SoDuTaiKhoan)
	VALUES (@IDNguoiDung, @IDNhanVien, @TaiKhoan, @MatKhau, 1, 1, 1, 1, 'ssoftvn', @ThoiGian, @IDDonVi, 0, 0);
	--INSERT HT_NhomNguoiDung
	DECLARE @IDNhomNguoiDung UNIQUEIDENTIFIER;
	SET @IDNhomNguoiDung = 'EE609285-F6A6-43D8-A517-BDA52F426AE5';
	INSERT INTO HT_NhomNguoiDung (ID, MaNhom, TenNhom, MoTa, NguoiTao, NgayTao)
	VALUES (@IDNhomNguoiDung, 'ADMIN', 'ADMIN', N'Nhóm quản trị', 'ssoftvn', @ThoiGian);
	--INSERT HT_NguoiDung_Nhom
	INSERT INTO HT_NguoiDung_Nhom (ID, IDNguoiDung, IDNhomNguoiDung, ID_DonVi)
	VALUES (NEWID(), @IDNguoiDung, @IDNhomNguoiDung, @IDDonVi);
	--INSERT HT_Quyen
	INSERT INTO HT_Quyen (MaQuyen, TenQuyen, MoTa, QuyenCha, DuocSuDung)
	SELECT q.MaQuyen, q.TenQuyen, q.MoTa, q.QuyenCha, q.DuocSuDung FROM BANHANG24..HT_Quyen q
	INNER JOIN BANHANG24..HT_Quyen_NganhNgheKinhDoanh qn ON q.MaQuyen = qn.MaQuyen
	WHERE qn.ID_NganhKinhDoanh = @IDNganhNgheKinhDoanh;
	--INSERT HT_Quyen_Nhom
	INSERT INTO HT_Quyen_Nhom (ID, ID_NhomNguoiDung, MaQuyen)
	SELECT NEWID(), @IDNhomNguoiDung, MaQuyen FROM HT_Quyen;
	--INSERT HT_CauHinhPhanMem
	INSERT INTO HT_CauHinhPhanMem (ID, ID_DonVi, GiaVonTrungBinh, CoDonViTinh, DatHang, XuatAm, DatHangXuatAm, ThayDoiThoiGianBanHang, SoLuongTrenChungTu,
	TinhNangTichDiem, GioiHanThoiGianTraHang, SanPhamCoThuocTinh, BanVaChuyenKhiHangDaDat, TinhNangSanXuatHangHoa, SuDungCanDienTu, KhoaSo, InBaoGiaKhiBanHang,
	QuanLyKhachHangTheoDonVi, KhuyenMai, LoHang, SuDungMauInMacDinh, ApDungGopKhuyenMai, ThongTinChiTietNhanVien, BanHangOffline, ThoiGianNhacHanSuDungLo,
	SuDungMaChungTu, ChoPhepTrungSoDienThoai)
	VALUES (NEWID(), @IDDonVi, 1, 1, 1, 0, 0, 0, 0,
	0, 0, 1, 0, 0, 0, 1, 0,
	0, 1, 0, 0, 0, 0, 0, 0,
	0, 0);
	--INSERT DM_QuocGia
	INSERT INTO DM_QuocGia (ID, MaQuocGia, TenQuocGia, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaQuocGia, TenQuocGia, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_QuocGia;
	--INSERT DM_VungMien
	INSERT INTO DM_VungMien (ID, MaVung, TenVung, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaVung, TenVung, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_VungMien;
	--INSERT DM_TinhThanh
	INSERT INTO DM_TinhThanh (ID, MaTinhThanh, TenTinhThanh, ID_QuocGia, ID_VungMien, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaTinhThanh, TenTinhThanh, ID_QuocGia, ID_VungMien, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_TinhThanh;
	--INSERT DM_QuanHuyen
	INSERT INTO DM_QuanHuyen (ID, MaQuanHuyen, TenQuanHuyen, ID_TinhThanh, GhiChu, NguoiTao, NgayTao)
	SELECT ID, MaQuanHuyen, TenQuanHuyen, ID_TinhThanh, GhiChu, 'ssoftvn', @ThoiGian FROM BANHANG24..DM_QuanHuyen;
	--INSERT DM_ThueSuat
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'0%', 0, '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'5%', 5, '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_ThueSuat (ID, MaThueSuat, ThueSuat, GhiChu, NguoiTao, NgayTao) VALUES (NEWID(), N'10%', 10, '', 'ssoftvn', @ThoiGian);
	--INSERT DM_TienTe
	DECLARE @IDTienTe UNIQUEIDENTIFIER;
	DECLARE @IDQuocGia UNIQUEIDENTIFIER;
	SET @IDTienTe = '406eed2d-faae-4520-aef2-12912f83dda2';
	SELECT @IDQuocGia = ID FROM DM_QuocGia WHERE MaQuocGia = 'VNI';
	INSERT INTO DM_TienTe (ID, ID_QuocGia, LaNoiTe, MaNgoaiTe, TenNgoaiTe, NguoiTao, NgayTao)
	VALUES (@IDTienTe, @IDQuocGia, 1, N'VND', N'Việt Nam đồng', 'ssoftvn', @ThoiGian);
	--INSERT DM_TyGia
	INSERT INTO DM_TyGia (ID, ID_TienTe, TyGia, NgayTyGia, GhiChu, NguoiTao, NgayTao)
	VALUES (NEWID(), @IDTienTe, 1, @ThoiGian, '', 'ssoftvn', @ThoiGian);
	--INSERT DM_Kho
	DECLARE @IDKho UNIQUEIDENTIFIER;
	SET @IDKho = '01CD02F2-4612-4104-B790-1C0373CBD72D';
	INSERT INTO DM_Kho (ID, MaKho, TenKho, NguoiTao, NgayTao)
	VALUES (@IDKho, N'KHO_CTy', N'Kho tổng', 'ssoftvn', @ThoiGian);
	--INSERT Kho_DonVi
	INSERT INTO Kho_DonVi (ID, ID_DonVi, ID_Kho)
	VALUES (NEWID(), @IDDonVi, @IDKho);
	--INSERT DM_LoaiChungTu
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (1, 'HDBL', N'Hóa đơn bán lẻ', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (2, 'HDB', N'Hóa đơn bán', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (3, 'BG', N'Báo giá', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (4, 'PNK', N'Phiếu nhập kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (5, 'PXK', N'Phiếu xuất kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (6, 'TH', N'Trả hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (7, 'THNCC', N'Trả hàng nhà cung cấp', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (8, 'XH', N'Xuất kho', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (9, 'PKK', N'Phiếu kiểm kê', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (10, 'CH', N'Chuyển hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (11, 'SQPT', N'Phiếu thu', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (12, 'SQPC', N'Phiếu chi', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (13, 'NH', N'Nhận hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (14, 'DH', N'Đặt hàng', '', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (15, 'CB', N'Điều chỉnh', N'Điều chỉnh công nợ khách hàng, nhà cung cấp', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (16, 'KTGV', N'Khởi tạo giá vốn', N'Khởi tạo giá vốn khi tạo hàng hóa', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (17, 'DTH', N'Đổi trả hàng', N'Đổi trả hàng hóa', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (18, 'DCGV', N'Điều chỉnh giá vốn', N'Điều chỉnh giá vốn', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (19, 'GDV', N'Gói dịch vụ', N'Bán gói dịch vụ', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (20, 'TGDV', N'Trả gói dịch vụ', N'Trả gói dịch vụ', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (21, 'IMV', N'Tem - Mã vạch', N'Tem - Mã vạch', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (22, 'TGT', N'Thẻ giá trị', N'Bán, nạp thẻ giá trị', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao)
	VALUES (23, 'DCGT', N'Điều chỉnh thẻ giá trị', N'Điều chỉnh giá trị thẻ giá trị', 'ssoftvn', @ThoiGian);
	--INSERT HT_ThongBao_CaiDat
	INSERT INTO HT_ThongBao_CaiDat (ID, ID_NguoiDung, NhacSinhNhat, NhacCongNo, NhacTonKho, NhacDieuChuyen, NhacLoHang)
	VALUES (NEWID(), @IDNguoiDung, 1, 1, 1, 1, 1);
	--INSERT NS_PhongBan
	DECLARE @IDPhongBan UNIQUEIDENTIFIER;
	SET @IDPhongBan = '6DE963A7-50AF-4E51-91E8-E242D7E7B476';
	INSERT INTO NS_PhongBan (ID, MaPhongBan, TenPhongBan, TrangThai)
	VALUES (@IDPhongBan, 'PB0000', N'Phòng ban mặc định', 1);
	--INSERT DM_NganHang
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'CTG', N'Ngân hàng Công thương Việt Nam (VietinBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIETBANK', N'Ngân hàng Việt Nam Thương Tín (VietBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'OCB', N'Ngân hàng Phương Đông (Orient Commercial Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NAMABANK', N'Ngân hàng Nam Á (Nam A Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SEABANK', N'Ngân hàng Đông Nam Á (SeABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'AGRIBANK', N'Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam (Agribank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'MBB', N'Ngân hàng Quân đội (Military Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'ACB', N'Ngân hàng Á Châu (ACB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIB', N'Ngân hàng Quốc tế (VIBBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'DAF', N'Ngân hàng Đông Á (DAF)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BVB', N'Ngân hàng Bảo Việt (BaoVietBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VDB', N'Ngân hàng Phát triển Việt Nam (VDB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'KIENLONGBANK', N'Ngân hàng Kiên Long (KienLongBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'HDBANK', N'Ngân hàng Phát triển nhà Thành Phố Hồ Chí Minh (HDBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SHB', N'Ngân hàng Sài Gòn - Hà Nội (SHBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'EIB', N'Ngân hàng Xuất Nhập khẩu Việt Nam (Eximbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'CB', N'Ngân hàng Xây dựng (CB)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SGB', N'Ngân hàng Sài Gòn Công Thương (Saigonbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'PVCOMBANK', N'Ngân hàng Đại chúng (PVcom Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'PGBANK', N'Ngân hàng Xăng dầu Petrolimex (Petrolimex Group Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'OCEANBANK', N'Ngân hàng Đại Dương (Oceanbank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'TECHCOMBANK', N'Ngân hàng Kỹ Thương Việt Nam (Techcombank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VAB', N'Ngân hàng Việt Á (VietABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'STB', N'Ngân hàng Sài Gòn Thương Tín (Sacombank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'MSB', N'Ngân hàng Hàng Hải Việt Nam (MaritimeBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VCB', N'Ngân hàng Ngoại thương Việt Nam (Ngoại Thương Việt Nam)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'GPBANK', N'Ngân hàng Dầu khí Toàn Cầu (GPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VIETCAPITALBANK', N'Ngân hàng Bản Việt (VietCapitalBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'SCB', N'Ngân hàng Sài Gòn (Sài Gòn)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'LPB', N'Ngân hàng Bưu điện Liên Việt (LienVietPostBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BID', N'Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'ABBANK', N'Ngân hàng An Bình (ABBANK)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NHCSXH/VBSP', N'Ngân hàng Chính sách xã hội (NHCSXH/VBSP)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'TPBANK', N'Ngân hàng Tiên Phong (TPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'BACABANK', N'Ngân hàng Bắc Á (BacABank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'VPBANK', N'Ngân hàng Việt Nam Thịnh Vượng (VPBank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	INSERT INTO DM_NganHang (ID, MaNganHang, TenNganHang, ChiNhanh, GhiChu, NguoiTao, NgayTao, ChiPhiThanhToan, TheoPhanTram, MacDinh, ThuPhiThanhToan) 
	VALUES (NEWID(), 'NCB', N'Ngân hàng Quốc Dân (National Citizen Bank)', '','','ssoftvn', @ThoiGian, 0, 0, 0, 0);
	--INSERT OptinForm_TruongThongTin
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Chọn chi nhánh', 2, 1, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Giới tính', 1, 3, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số lượng', 2, 5, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Địa chỉ', 1, 7, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Thời gian', 2, 9, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Khách hàng', 2, 3, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Email', 1, 6, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Dịch vụ yêu cầu', 2, 8, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Khách hàng lẻ', 1, 11, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Xưng hô', 2, 2, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Nhân viên thực hiện', 2, 10, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số điện thoại', 2, 6, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Tỉnh thành', 1, 8, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Mã số thuế', 1, 10, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Bạn đi theo nhóm', 2, 4, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ngày sinh', 1, 4, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Quận huyện', 1, 9, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Số điện thoại', 1, 5, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ảnh đại diện', 1, 1, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Ghi chú', 2, 11, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Email', 2, 7, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Người giới thiệu', 1, 12, 1);
	INSERT INTO OptinForm_TruongThongTin (ID, TenTruongThongTin, LoaiTruongThongTin, STT, TrangThai) VALUES (NEWID(), N'Tên khách hàng', 1, 2, 1);
	--INSERT DM_DoiTuong
	INSERT INTO DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong, TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, ChiaSe, TheoDoi,
	ID_DonVi, TenNhomDoiTuongs, NguoiTao, NgayTao)
	VALUES ('00000000-0000-0000-0000-000000000000', 1, 1, 'KL00001', N'Khách lẻ', 'Khach le', 'Kl', 0, 0, @IDDonVi, N'Nhóm mặc định', 'ssoftvn', @ThoiGian);
	INSERT INTO DM_DoiTuong (ID, LoaiDoiTuong, LaCaNhan, MaDoiTuong, TenDoiTuong, TenDoiTuong_KhongDau, TenDoiTuong_ChuCaiDau, ChiaSe, TheoDoi,
	ID_DonVi, TenNhomDoiTuongs, NguoiTao, NgayTao)
	VALUES ('00000000-0000-0000-0000-000000000002', 2, 1, 'NCCL001', N'Nhà cung cấp lẻ', 'Nha cung cap le', 'nccl', 0, 0, @IDDonVi, N'Nhóm mặc định', 'ssoftvn', @ThoiGian);
	--INSERT DM_NhomHangHoa
	INSERT INTO DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa, LaNhomHangHoa, NguoiTao, NgayTao, HienThi_Chinh, HienThi_Phu, HienThi_BanThe,
	TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau)
	VALUES ('00000000-0000-0000-0000-000000000000', 'NHMD00001', N'Nhóm hàng hóa mặc định', 1, 'ssoftvn', @ThoiGian, 1, 1, 1, 'Nhom hang hoa mac dinh', 'Nhhmd');
	INSERT INTO DM_NhomHangHoa (ID, MaNhomHangHoa, TenNhomHangHoa, LaNhomHangHoa, NguoiTao, NgayTao, HienThi_Chinh, HienThi_Phu, HienThi_BanThe,
	TenNhomHangHoa_KhongDau, TenNhomHangHoa_KyTuDau)
	VALUES ('00000000-0000-0000-0000-000000000001', 'DVMD00001', N'Nhóm dịch vụ mặc định', 0, 'ssoftvn', @ThoiGian, 1, 1, 1, 'Nhom dich vu mac dinh', 'Ndvmd');

	--INSERT Gara_HangXe
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('886E75F8-21D8-4519-A32B-19FC75D2FD2C', 'CHEVROLET', N'Chevrolet', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('326502EE-14C5-4398-BA65-90361E3AD6D1', 'FORD', N'Ford', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('CAC7F723-6BEC-4FA4-A18A-CCE47B4D921B', 'HONDA', N'Honda', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('D6CD766E-86A1-46B6-882F-BDD4EAD7EA78', 'HYUNDAI', N'Hyundai', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('D954DD78-0477-43A0-B2D7-495A9549F7E1', 'INFINITI', N'Infiniti', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('3B036EBD-12B9-4D3C-9136-29B7F086F1B7', 'ISUZU', N'Isuzu', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('CBDE7FA8-0F56-43B0-B0E0-BF4C7653AF49', 'KIA', N'Kia', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('48C65217-1943-4EA2-A3E7-75617D45D975', 'LEXUS', N'Lexus', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8B4D5D6B-B2F3-46A3-8C6D-5742A6B1B8AA', 'MASERATI', N'Maserati', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('3DB382EC-3E11-42E2-9AAE-07E113AD7BEE', 'MAZDA', N'Mazda', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('26C1C0A3-3C0C-4E5F-AA5C-1C347DA1E5B2', 'MERCEDES', N'Mercedes', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('36D9D282-7152-416F-AE4A-CA9232186699', 'MITSUBISHI', N'Mitsubishi', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8E6D885B-4B6D-4A18-AA32-FCCE8E4373B9', 'NISSAN', N'Nissan', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('04844286-EDB5-41E2-91E7-A0B06695E31F', 'PEUGEOT', N'Peugeot', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('0788EDD3-AE0E-4BEF-AA98-2FEDD94EC8B1', 'PORSCHE', N'Porsche', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('34A0801B-D517-478F-8172-774E3ED0A7FE', 'RENAULT', N'Renault', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('98A3621E-7D00-436F-B25C-045AC2186AE5', 'SSANGYONG', N'Ssangyong', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('FED3837F-7A4D-4C98-9B85-90F1AE23E753', 'SUBARU', N'Subaru', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('B3CD7C19-67EC-43A9-B56F-A12EE87B53CA', 'SUZUKI', N'Suzuki', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('EA6CA8A9-8B9E-489E-8C07-2F673C6E3EF7', 'TOYOTA', N'Toyota', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('34192C59-2D6E-47E9-AE92-6B00D5A6DA3A', 'VINFAST', N'Vinfast', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8E65F5C2-5D5A-42E8-93FF-33C379B84F4D', 'VOLKSWAGEN', N'Volkswagen', '', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_HangXe (ID, MaHangXe, TenHangXe, Logo, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('39A939F7-1EBF-494C-9BE1-5EB750E73334', 'VOLVO', N'Volvo', '', 1, 'ssoftvn', GETDATE(), '', null);

	--INSERT Gara_LoaiXe
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('8C15086A-0645-437B-BA7A-008CFDC0C737', 'SEDAN', N'Sedan', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('4115FB1B-8978-40E2-87F9-67DB6425AB5E', 'HATCHBACK', N'Hatchback', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('B17B339C-CDF3-4E9E-ACCE-29FB097899F0', 'MPV', N'MPV', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('78FFB367-ECD6-4A55-8E6C-769FC3D3F13B', 'SUV', N'SUV', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('A81F4945-1B3A-4FD1-B8B8-2B35B4AC58C0', 'SUV-COUPE', N'SUV-Coupe', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('7D8190F1-07A9-477D-8B05-EBB8EC154FCC', 'SUV-WAGON', N'SUV-Wagon', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('2BE297DF-B5A5-4657-85DF-EEA7E2E6BB0F', 'PICK-UP', N'Pick-up', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('83583EFC-AFA8-446D-90A9-36397A7F01D5', 'COUPE', N'Coupe', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('83034F8B-B16F-41FB-B75A-2B53AD1410E0', 'COUPE2CUA', N'Coupe 2 cửa', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('3C7BC4F5-E7B4-475C-B29A-FB122C38D1F9', 'COUPE4CUA', N'Coupe 4 cửa', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('6541F0FC-D139-4C44-8EE8-B67045EC842F', 'CROSSOVER', N'Crossover', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('B26E8752-B118-4680-90E0-BC04028AA831', 'CONVERTIBLE', N'Convertible', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('89501335-9457-4BF1-9335-A9E0355C5E63', 'ROADSTER', N'Roadster', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('340C75FF-46F4-4856-B649-983273A152D8', 'SIEUXE', N'Siêu xe', 1, 'ssoftvn', GETDATE(), '', null);
	INSERT INTO Gara_LoaiXe (ID, MaLoaiXe, TenLoaiXe, TrangThai, NguoiTao, NgayTao, NguoiSua, NgaySua) VALUES ('5DF0C056-FC43-4653-A811-44BA3468124F', 'STATION WAGON', N'Station Wagon', 1, 'ssoftvn', GETDATE(), '', null);

END");
			Sql(@"Update ChietKhauDoanhThu set LoaiNhanVienApDung= 3;
update BH_HoaDon
set TongThanhToan = PhaiThanhToan + TongChiPhi + TongTienThue
where (TongThanhToan is null or TongThanhToan = 0);
insert into DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao, NguoiSua, NgaySua)
values ('25', 'HDSC', N'Hóa đơn sửa chữa', '', 'ssoftvn',GETDATE(), '', NULL);
insert into DM_LoaiChungTu (ID, MaLoaiChungTu, TenLoaiChungTu, GhiChu, NguoiTao, NgayTao, NguoiSua, NgaySua)
values ('26', 'PTN', N'Phiếu tiếp nhận', N'Phiếu tiếp nhận gara', 'ssoftvn',GETDATE(), '', NULL);");
        }
        
        public override void Down()
        {
			Sql("DROP FUNCTION [dbo].[DiscountSale_NVBanHang]");
			Sql("DROP FUNCTION [dbo].[DiscountSale_NVienDichVu]");
			Sql("DROP FUNCTION [dbo].[DiscountSale_NVLapHoaDon]");
			DropStoredProcedure("[dbo].[BaoCaoDoanhThuSuaChuaChiTiet]");
			DropStoredProcedure("[dbo].[BaoCaoDoanhThuSuaChuaTheoCoVan]");
			DropStoredProcedure("[dbo].[BaoCaoDoanhThuSuaChuaTheoXe]");
			DropStoredProcedure("[dbo].[BaoCaoDoanhThuSuaChuaTongHop]");
			DropStoredProcedure("[dbo].[DiscountSale_byIDNhanVien]");
			DropStoredProcedure("[dbo].[Gara_GetListBaoGia]");
			DropStoredProcedure("[dbo].[Gara_GetListHangHoa_ByIDQuiDoi]");
			DropStoredProcedure("[dbo].[Gara_JqAutoHangHoa]");
			DropStoredProcedure("[dbo].[GetAll_DiscountSale]");
			DropStoredProcedure("[dbo].[getlist_SuKienToDay_v2]");
			DropStoredProcedure("[dbo].[GetListBaoGia_AfterXuLy]");
			DropStoredProcedure("[dbo].[GetListBaoHiem_v1]");
			DropStoredProcedure("[dbo].[GetListCashFlow_Paging]");
			DropStoredProcedure("[dbo].[GetListHoaDonSuaChua]");
			DropStoredProcedure("[dbo].[GetListPhieuNhapXuatKhoByIDPhieuTiepNhan]");
			DropStoredProcedure("[dbo].[GetListPhieuTiepNhan_v2]");
			DropStoredProcedure("[dbo].[GetMaMaPhieuTiepNhan_byTemp]");
			DropStoredProcedure("[dbo].[GetTonKho_byIDQuyDois]");
			DropStoredProcedure("[dbo].[JqAuto_HoaDonSC]");
			DropStoredProcedure("[dbo].[JqAuto_PhieuTiepNhan]");
			DropStoredProcedure("[dbo].[JqAuto_SearchMauXe]");
			DropStoredProcedure("[dbo].[NangNhom_KhachHangbyID]");
			DropStoredProcedure("[dbo].[PhieuTiepNhan_GetThongTinChiTiet]");
			DropStoredProcedure("[dbo].[TongQuanBieuDoDoanhThuThuan]");
			DropStoredProcedure("[dbo].[TongQuanBieuDoThucThu]");
			DropStoredProcedure("[dbo].[TongQuanDoanhThuCongNo]");
			DropStoredProcedure("[dbo].[XuatKhoToanBo_FromHoaDonSC]");
		}
    }
}