namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20191125 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[getList_ChietKhauNhanVienTheoDoanhSo]
    @ID_ChiNhanhs [nvarchar](max),
    @MaNhanVien [nvarchar](500),
    @MaNhanVien_TV [nvarchar](500),
    @LaDoanhThu varchar(5),
    @LaThucThu varchar(5),
    @timeStar [nvarchar](max),
    @timeEnd [nvarchar](max)
AS
BEGIN
	set nocount on;
	set @timeEnd = dateadd(day,1, @timeEnd) 
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
    					where (nv.MaNhanVien like @MaNhanVien or nv.MaNhanVien like @MaNhanVien_TV or nv.TenNhanVienChuCaiDau like @MaNhanVien or nv.TenNhanVienKhongDau like @MaNhanVien)
    					and ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and ckdt.TrangThai=1
    					and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    					and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    					and hd.NgayLapHoaDon >= @timeStar  and hd.NgayLapHoaDon < @timeEnd
    
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
    					where (nv.MaNhanVien like @MaNhanVien or nv.MaNhanVien like @MaNhanVien_TV or nv.TenNhanVienChuCaiDau like @MaNhanVien or nv.TenNhanVienKhongDau like @MaNhanVien)
    					and ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and ckdt.TrangThai=1
    					and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    					and hd.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    					and hd.NgayLapHoaDon >= @timeStar and hd.NgayLapHoaDon < @timeEnd
						--and CONVERT(varchar, hd.NgayLapHoaDon,23) >= @timeStar  and CONVERT(varchar, hd.NgayLapHoaDon,23) <= @timeEnd
    					and (qhd.TrangThai is null or qhd.TrangThai != 0)
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
    					where (nv.MaNhanVien like @MaNhanVien or nv.MaNhanVien like @MaNhanVien_TV or nv.TenNhanVienChuCaiDau like @MaNhanVien or nv.TenNhanVienKhongDau like @MaNhanVien)
    					and ckdt.ID_DonVi in (select * from dbo.splitstring(@ID_ChiNhanhs))
    					and ckdt.TrangThai=1
    					and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
    					and hdt.ChoThanhToan = '0' and hd.loaihoadon in (1,19,22)
    					and hdt.NgayLapHoaDon >= @timeStar and hdt.NgayLapHoaDon < @timeEnd
						--and CONVERT(varchar, hdt.NgayLapHoaDon,23) >= @timeStar  and CONVERT(varchar, hdt.NgayLapHoaDon,23) <= @timeEnd
    					and (qhd.TrangThai is null or qhd.TrangThai != 0)
    
    					) as a
    					GROUP BY a.ID_NhanVien, a.MaNhanVien, a.TinhChietKhauTheo, a.ID_ChietKhauDoanhThu
    				) as b
    				join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu and ((b.DoanhThu >= DoanhThuTu and TinhChietKhauTheo = 2) or (b.ThucThu >= DoanhThuTu and TinhChietKhauTheo = 1))
    			) as c
    		) as d
    		where d.rk = 1
    ) e
    GROUP BY e.ID_NhanVien
END

-- getList_ChietKhauNhanVienTheoDoanhSo 'D93B17EA-89B9-4ECF-B242-D03B8CDE71DE','%%','%%', '2', '1','2019-05-01','2019-05-31'");

        }
        
        public override void Down()
        {
        }
    }
}
