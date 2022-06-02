namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190513 : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE FUNCTION [dbo].[GetFileName]
(
 @fullpath nvarchar(260)
) 
RETURNS nvarchar(260)
AS
BEGIN
DECLARE @charIndexResult int
SET @charIndexResult = CHARINDEX('/', REVERSE(@fullpath))

IF @charIndexResult = 0
    RETURN NULL 

RETURN RIGHT(@fullpath, @charIndexResult -1)
END
");

            CreateStoredProcedure(name: "[dbo].[getList_ChietKhauNhanVienTheoDoanhSo]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                MaNhanVien = p.String(),
                MaNhanVien_TV = p.String(),
                LaDoanhThu = p.String(),
                LaThucThu = p.String(),
                timeStar = p.DateTime(),
                timeEnd = p.DateTime(),
            }, body: @"Select 
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
					and hd.NgayLapHoaDon >= @timeStar and hd.NgayLapHoaDon < @timeEnd

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
					and (qhd.TrangThai is null or qhd.TrangThai != 0)

					) as a
					GROUP BY a.ID_NhanVien, a.MaNhanVien, a.TinhChietKhauTheo, a.ID_ChietKhauDoanhThu
				) as b
				join ChietKhauDoanhThu_ChiTiet ckct on b.ID_ChietKhauDoanhThu = ckct.ID_ChietKhauDoanhThu and ((b.DoanhThu >= DoanhThuTu and TinhChietKhauTheo = 2) or (b.ThucThu >= DoanhThuTu and TinhChietKhauTheo = 1))
			) as c
		) as d
		where d.rk = 1
) e
GROUP BY e.ID_NhanVien");

            CreateStoredProcedure(name: "[dbo].[getList_ChietKhauNhanVienTheoDoanhSobyID]", parametersAction: p => new
            {
                ID_ChiNhanhs = p.String(),
                ID_NhanVien = p.String(),
                LaDoanhThu = p.String(),
                LaThucThu = p.String(),
                timeStar = p.DateTime(),
                timeEnd = p.DateTime(),
            }, body: @"Select 
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
					and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
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
					and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
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
					and (ckdt.TinhChietKhauTheo like @LaDoanhThu or ckdt.TinhChietKhauTheo like @LaThucThu)
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
	where d.rk = 1");

            Sql(@"ALTER PROCEDURE [dbo].[Load_DMHangHoa_TonKho_ChotSo]
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    DECLARE @timeChotSo Datetime
    Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);	
    -- lấy danh sách hàng hóa xuất nhập tồn thời gian chốt sổ trước timeStar
    	Select
    		MAX(tr.ID) as ID,
    		tr.QuanLyTheoLoHang,   
			TyLeChuyenDoi,   
			LaDonViChuan,  
			case when MAX(QuyCach) = 0 then TyLeChuyenDoi else MAX(QuyCach) * TyLeChuyenDoi end as QuyCach,			
    		tr.TenHangHoa,
    		--tr.TenHangHoa_KhongDau,
    		--tr.TenHangHoa_KyTuDau,
    		tr.MaHangHoa,
			CONCAT(tr.MaHangHoa, ' ' , tr.TenHangHoa, ' ', tr.TenHangHoa_KhongDau,' ',TenHangHoa_KyTuDau,' ',
			MAX(tr.MaLoHang),' ', max(tr.GiaBan)) as Name,
    		MAX(tr.ThuocTinh_GiaTri) as ThuocTinh_GiaTri,
    		SUM(tr.TonKho) as TonKho,
    		Case When gv.ID is null then 0 else CAST(ROUND(( gv.GiaVon), 0) as float) end as GiaVon,
    		MAX(tr.GiaBan) as GiaBan, 
    		MAX (tr.TenDonViTinh) as TenDonViTinh, 
    		tr.ID_NhomHangHoa,
    		tr.ID_DonViQuiDoi,			
    		MAX(tr.SrcImage) as SrcImage, 
    		tr.LaHangHoa,
    		Case when tr.ID_LoHang is null then NEWID() else tr.ID_LoHang end as ID_LoHang,
    		MAX(tr.MaLoHang) as MaLoHang,
    		MAX(tr.NgaySanXuat) as NgaySanXuat,
    		MAX(tr.NgayHetHan) as NgayHetHan,
			MAX(ISNULL(tr.PhiDichVu,0)) as PhiDichVu,
			MAX(ISNULL(tr.DonViTinhQuyCach,'')) as DonViTinhQuyCach,
			LaPTPhiDichVu,
			ThoiGianBaoHanh,
			LoaiBaoHanh
    		 FROM
    		(
    			 Select  
    				dvqd1.ID_HangHoa as ID,
					ISNULL(dvqd1.LaDonViChuan,'0') as LaDonViChuan,   
    				ISNULL(dvqd1.TyLeChuyenDoi,1) as TyLeChuyenDoi,  
					CAST(ISNULL(dhh1.QuyCach,1) as float) as QuyCach,
					ISNULL(dhh1.QuanLyTheoLoHang,'0') as QuanLyTheoLoHang, 
    				dhh1.TenHangHoa,
    				Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				dhh1.TenHangHoa_KhongDau,
    				dhh1.TenHangHoa_KyTuDau,
    				dvqd1.MaHangHoa, 					
    				0 as TonKho,
    				CAST(ROUND((dvqd1.GiaBan), 0) as float) as GiaBan, 
    				dvqd1.TenDonViTinh, 
    				dhh1.ID_NhomHang as ID_NhomHangHoa,
    				dvqd1.ID as ID_DonViQuiDoi,
    				an.URLAnh as SrcImage,
    				dhh1.LaHangHoa,
    				lh1.ID as ID_LoHang,
    				lh1.MaLoHang,
    				lh1.NgaySanXuat,
    				lh1.NgayHetHan,
    				lh1.TrangThai,
    				Case when dhh1.LaHangHoa='1' then 0 else CAST(ISNULL(dhh1.ChiPhiThucHien,0) as float) end as PhiDichVu,
					Case when dhh1.LaHangHoa='1' then '0' else ISNULL(dhh1.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
					ISNULL(dhh1.ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
					ISNULL(dhh1.LoaiBaoHanh,0) as LoaiBaoHanh,
					dhh1.DonViTinhQuyCach as DonViTinhQuyCach

    			 from
    				 DonViQuiDoi dvqd1 
    				 join DM_HangHoa dhh1 on dvqd1.ID_HangHoa = dhh1.ID
    				 left join DM_LoHang lh1 on dvqd1.ID_HangHoa = lh1.ID_HangHoa
    				 LEFT join DM_HangHoa_Anh an on (dvqd1.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    				 LEFT JOIN (Select Main.id_hanghoa,
    						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    						From
    						(
    						Select distinct hh_tt.id_hanghoa,
    							(
    								Select tt.GiaTri + ' - ' AS [text()]
    								From dbo.hanghoa_thuoctinh tt
    								Where tt.id_hanghoa = hh_tt.id_hanghoa
    								order by tt.ThuTuNhap 
    								For XML PATH ('')
    							) hanghoa_thuoctinh
    						From dbo.hanghoa_thuoctinh hh_tt
    						) Main
    					) as ThuocTinh on dvqd1.ID_HangHoa = ThuocTinh.id_hanghoa
    						 where dvqd1.xoa is null and dhh1.duocbantructiep = '1'  and dhh1.TheoDoi = '1'
    			 Union all
    
    			SELECT 
    				dvqd3.ID_HangHoa as ID,
					ISNULL(dvqd3.LaDonViChuan,'0') as LaDonViChuan,   
    				ISNULL(dvqd3.TyLeChuyenDoi,1) as TyLeChuyenDoi, 
					CAST(ISNULL(dhh.QuyCach,1) as float) as QuyCach,
					ISNULL(dhh.QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
    				dhh.TenHangHoa,
					'' as ThuocTinh_GiaTri,
    				--Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    				dhh.TenHangHoa_KhongDau,
    				dhh.TenHangHoa_KyTuDau,
    				dvqd3.MaHangHoa, 
    				Case when dhh.LaHangHoa = 0 then 0 else CAST(ROUND(ISNULL(a.TonCuoiKy / dvqd3.TyLeChuyenDoi, 0), 3) as float) end as TonKho,
    				CAST(ROUND((dvqd3.GiaBan), 0) as float) as GiaBan, 
    				dvqd3.TenDonViTinh, 
    				dhh.ID_NhomHang as ID_NhomHangHoa,
    				dvqd3.ID as ID_DonViQuiDoi,
    				'' as SrcImage,
    				dhh.LaHangHoa,
    				a.ID_LoHang,
    				a.MaLoHang,
    				a.NgaySanXuat,
    				a.NgayHetHan, 
    				a.TrangThai,
    				Case when dhh.LaHangHoa='1' then 0 else CAST(ISNULL(dhh.ChiPhiThucHien,0) as float) end as PhiDichVu,
					Case when dhh.LaHangHoa='1' then '0' else ISNULL(dhh.ChiPhiTinhTheoPT,'0') end as LaPTPhiDichVu,
					ISNULL(dhh.ThoiGianBaoHanh,0) as ThoiGianBaoHanh,
					ISNULL(dhh.LoaiBaoHanh,0) as LoaiBaoHanh,
					dhh.DonViTinhQuyCach as DonViTinhQuyCach
    	FROM 
    		DonViQuiDoi dvqd3
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd3.ID_HangHoa
    		LEFT JOIN 
    		 (
    		SELECT 
    			dhh.ID,
    			MAX(dhh.TenHangHoa)   AS TenHangHoa,
    			MAX(dhh.TenHangHoa_KhongDau)   AS TenHangHoa_KhongDau,
    			MAX(dhh.TenHangHoa_KyTuDau)   AS TenHangHoa_KyTuDau,
    			MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
    			MAX(CAST(ISNULL(dhh.QuyCach,1) as float)) as QuyCach,
				--case when MAX(dhh.QuyCach) is null or MAX(dhh.QuyCach)= 0 then 1 else MAX(dhh.QuyCach) end as QuyCach,
				ISNULL(dhh.QuanLyTheoLoHang,'0') as QuanLyTheoLoHang,
    			MAX(lh.ID)  As ID_LoHang,
    			MAX(Case when lh.MaLohang is null or dhh.QuanLyTheoLoHang = 0 then '' else lh.MaLoHang end) As MaLoHang,
    			MAX(lh.NgaySanXuat)  As NgaySanXuat,
    			MAX(lh.NgayHetHan)  As NgayHetHan,
    			lh.TrangThai,
    		SUM(ISNULL(HangHoa.TonCuoiKy,0)) AS TonCuoiKy
    		FROM
    		(
    			SELECT
    				td.ID_DonViQuiDoi AS ID_DonViQuiDoi,
    				td.ID_LoHang,
    			SUM(ISNULL(td.TonKho,0)) + SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonCuoiKy
    			FROM
    			(
    					SELECT 
    						dvqd.ID As ID_DonViQuiDoi,
    						Case when hh.QuanLyTheoLoHang = 1 then cs.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    						NULL AS SoLuongNhap,
    						NULL AS SoLuongXuat,
    						SUM(ISNULL(cs.TonKho, 0)) as TonKho
    				FROM DonViQUiDoi dvqd
    				inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				inner join chotso_hanghoa cs on hh.ID = cs.ID_HangHoa and cs.ID_DonVi = @ID_ChiNhanh
    				where dvqd.ladonvichuan = '1'
    				AND dvqd.Xoa IS NULL
    				AND hh.DuocBanTrucTiep = '1'
    				GROUP BY dvqd.ID, ID_LoHang, hh.QuanLyTheoLoHang
    				UNION ALL
    				-- phát sinh xuất nhập tồn từ khi chốt sổ tới thời gian bắt đầu
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					NULL AS SoLuongNhap,
    					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct   
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    				AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang                                                                                                                                                                                                                                                                
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					NULL AS SoLuongNhap,
    					SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
    				OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4'))AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    					null AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9')AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
    				AND bhd.ID_DonVi = @ID_ChiNhanh
    					AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    
    				UNION ALL
    				SELECT 
    					bhdct.ID_DonViQuiDoi,
    					Case when hh.QuanLyTheoLoHang = 1 then bhdct.ID_LoHang else '10000000-0000-0000-0000-000000000001' end as ID_LoHang,
    					SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
    					null AS SoLuongXuat,
    					NULL AS TonKho
    				FROM BH_HoaDon_ChiTiet bhdct
    				JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
    				JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
    				JOIN DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
    				WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')AND dvqd.Xoa IS NULL AND bhd.ChoThanhToan = 0
    					AND bhd.NgayLapHoaDon >= @timeChotSo
    				GROUP BY bhdct.ID_DonViQuiDoi, ID_LoHang, hh.QuanLyTheoLoHang    
    			) AS td
    			GROUP BY td.ID_DonViQuiDoi, td.ID_LoHang, td.TonKho
    		) AS HangHoa
    		right JOIN DonViQuiDoi dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
    		LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
    			LEFT JOIN DM_LoHang lh on HangHoa.ID_LoHang = lh.ID
    			GROUP BY dhh.ID, dhh.LaHangHoa, dhh.ID_NhomHang,HangHoa.ID_LoHang,dhh.QuanLyTheoLoHang, lh.TrangThai
    ) a
    on dvqd3.ID_HangHoa = a.ID
    --LEFT join DM_HangHoa_Anh an on (dvqd3.ID_HangHoa = an.ID_HangHoa and (an.sothutu = 1 or an.ID is null))
    --	LEFT join DM_HangHoa hh on dvqd3.ID_HangHoa = hh.ID
    --	LEFT JOIN (Select Main.id_hanghoa,
    --						Left(Main.hanghoa_thuoctinh,Len(Main.hanghoa_thuoctinh)-2) As ThuocTinh_GiaTri
    --						From
    --						(
    --						Select distinct hh_tt.id_hanghoa,
    --							(
    --								Select tt.GiaTri + ' - ' AS [text()]
    --								From dbo.hanghoa_thuoctinh tt
    --								Where tt.id_hanghoa = hh_tt.id_hanghoa
    --								order by tt.ThuTuNhap 
    --								For XML PATH ('')
    --							) hanghoa_thuoctinh
    --						From dbo.hanghoa_thuoctinh hh_tt
    --						) Main
    --					) as ThuocTinh on dvqd3.ID_HangHoa = ThuocTinh.id_hanghoa
    where dvqd3.xoa is null and dhh.duocbantructiep = '1'  and dhh.TheoDoi = '1'
    	and ((a.QuanLyTheoLoHang = 1 and a.ID_LoHang is not null) or a.QuanLyTheoLoHang = 0 or a.QuanLyTheoLoHang is null)
    --order by TenHangHoa
    		) tr
    		left join DM_GiaVon gv on (tr.ID_DonViQuiDoi = gv.ID_DonViQuiDoi and (tr.ID_LoHang = gv.ID_LoHang or tr.ID_LoHang is null) and gv.ID_DonVi = @ID_ChiNhanh)
    		where tr.TrangThai = 1 or tr.TrangThai is null
    		Group by tr.ID_DonViQuiDoi,tr.ID, tr.ID_LoHang, tr.QuanLyTheoLoHang, gv.ID, gv.GiaVon, tr.ID_NhomHangHoa, tr.LaHangHoa, tr.TyLeChuyenDoi,tr.LaDonViChuan,
					tr.MaHangHoa, tr.TenHangHoa, tr.TenHangHoa_KhongDau, tr.TenHangHoa_KyTuDau, tr.LaPTPhiDichVu, tr.ThoiGianBaoHanh, tr.LoaiBaoHanh
			having convert(varchar, MAX(tr.NgayHetHan),112) >= convert(varchar, getdate(),112) or MAX(tr.NgayHetHan) is null
    		order by tr.MaHangHoa, MAX(tr.NgayHetHan)
END

--exec Load_DMHangHoa_TonKho_ChotSo'd93b17ea-89b9-4ecf-b242-d03b8cde71de'

--select *, convert(varchar, ngayhethan,112), convert(varchar, getdate(),112) from dm_lohang
--where ngayhethan is null or convert(varchar, ngayhethan,112) >= convert(varchar, getdate(),112)");

            Sql(@"ALTER PROCEDURE [dbo].[SP_Check_LoaiCongViec_Exist]
    @LoaiCongViec [nvarchar](500),
    @ID_LoaiCongViec [varchar](40),
	@LoaiTuVan_LichHen int
AS
BEGIN
    DECLARE @valReturn bit ='0'
    	 DECLARE @ID nvarchar(max);
    
    	 IF @ID_LoaiCongViec='00000000-0000-0000-0000-000000000000' 
    		 SELECT @ID =  ID from DM_LoaiTuVanLichHen WHERE TrangThai != '0'  AND TenLoaiTuVanLichHen like  @LoaiCongViec AND TuVan_LichHen= @LoaiTuVan_LichHen
    	 ELSE
    		 SELECT @ID = ID from DM_LoaiTuVanLichHen WHERE TrangThai != '0'  AND TenLoaiTuVanLichHen like  @LoaiCongViec AND TuVan_LichHen= @LoaiTuVan_LichHen  AND ID not like  @ID_LoaiCongViec 
    
    	IF @ID IS NULL SET @valReturn= '0'
    		ELSE SET @valReturn= '1'
    
    	SELECT @valReturn AS Exist
END

--SP_Check_LoaiCongViec_Exist N'loai3','66552429-2dbb-449a-b8f3-03719ceed495',4");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetListCongViec_ByKhachHang]
	@ID_DonVi varchar(40),
	@ID_DoiTuong varchar(40)

AS
BEGIN
	select cs.ID, cs.ID_KhachHang, cs.ID_LienHe, cs.ID_NhanVien, cs.ID_NhanVienQuanLy, cs.ID_LoaiTuVan, cs.Ma_TieuDe, 
		cs.NgayTao, ISNULL(cs.CaNgay,'0') as CaNgay,
		cs.NgayGio as ThoiGianTu, 
		cs.NgayGioKetThuc as ThoiGianDen, 
		ISNULL(cs.NoiDung,'') as NoiDung, 
		ISNULL(cs.KetQua, '') as KetQua, 
		ISNULL(cs.GhiChu,'') as GhiChu, 
		cs.TrangThai,cs.NguoiTao, cs.MucDoUuTien, cs.NgayHoanThanh, 
		dbo.GetFileName(FileDinhKem) as FileDinhKem,
		TenDoiTuong, ISNULL(lh.TenLienHe, '') as TenLienHe, ISNULL(nv.TenNhanVien,'') as TenNhanVien, TenLoaiTuVanLichHen, dt.ID_TrangThai, 
		ISNULL(NhanVien_PhoiHop.StaffIDs,'') as StaffIDs, 
		ISNULL(NhanVien_PhoiHop.StaffNames,'') as StaffNames
	from ChamSocKhachHangs cs
	join DM_LoaiTuVanLichHen ltv on cs.ID_LoaiTuVan= ltv.ID
	join Dm_DoiTuong dt on cs.ID_KhachHang= dt.ID
	left join DM_LienHe lh on cs.ID_LienHe= lh.ID
	left join NS_NhanVien nv on cs.ID_NhanVien = nv.ID
	left join(Select Main.ID,
    								Left(Main.IDs,Len(Main.IDs)-1) As StaffIDs,
    								Left(Main.Names,Len(Main.Names)-1) As StaffNames
    								From
    								(
    								Select distinct cskh.ID,
    								(
    									Select convert(varchar(max),nvph.ID_NhanVien) + ', ' AS [text()]
    									From ChamSocKhachHang_NhanVien nvph
    									Where nvph.ID_ChamSocKhachHang = cskh.ID
    									For XML PATH ('')
    								) IDs,
    								(
    								Select nv.TenNhanVien  + ', ' AS [text()]
    								From ChamSocKhachHang_NhanVien nvph
									join NS_NhanVien nv on nvph.ID_NhanVien = nv.ID
    								Where nvph.ID_ChamSocKhachHang = cskh.ID
    								order by nv.TenNhanVien
    								For XML PATH ('')
    								) Names
    								From dbo.ChamSocKhachHangs cskh
    								) Main) as NhanVien_PhoiHop on cs.ID = NhanVien_PhoiHop.ID
	where cs.PhanLoai= 4 and cs.TrangThai !=0
	and cs.ID_DonVi like @ID_DonVi
	and cs.ID_KhachHang like @ID_DoiTuong
	order by cs.NgayGio asc,cs.MucDoUuTien desc
END
");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_ChietKhauNhanVienTheoDoanhSo]");
            DropStoredProcedure("[dbo].[getList_ChietKhauNhanVienTheoDoanhSobyID]");
        }
    }
}
