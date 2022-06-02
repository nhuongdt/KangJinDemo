namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190916 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NS_KyHieuCong", "ID_KyTinhCong", c => c.Guid());
            AddColumn("dbo.NS_ChamCong_ChiTiet", "SoGioOT", c => c.Double(nullable: false));
            CreateIndex("dbo.NS_KyHieuCong", "ID_KyTinhCong");
            AddForeignKey("dbo.NS_KyHieuCong", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID");

            CreateStoredProcedure(name: "[dbo].[SP_BaoCaoKhuyenMai]", parametersAction: p => new
            {
                SearchString = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                LaHangHoa = p.String(),
                TheoDoi = p.String(),
                TrangThai = p.String(),
                ID_NhomHang = p.Guid(),
                LoaiChungTu = p.String(),
                ID_NguoiDung = p.Guid()
            }, body: @"SET NOCOUNT ON;

	DECLARE @tblSearchString TABLE (Name [nvarchar](max));
	DECLARE @count int;
	INSERT INTO @tblSearchString(Name) select  Name from [dbo].[splitstringByChar](@SearchString, ' ') where Name!='';
	Select @count =  (Select count(*) from @tblSearchString);


	select * from 
	(
		select km.MaKhuyenMai, km.TenKhuyenMai,km.HinhThuc, hd.ID_DonVi, hd.MaHoaDon,hd.LoaiHoaDon, hd.NgayLapHoaDon, hd.TongTienHang, hd.NguoiTao, 
			 ''as MaHangHoa, '' as TenHangHoa,0 as SoLuong,
			 case when km.HinhThuc = 14 then DiemGiaoDich else hd.KhuyeMai_GiamGia end as GiaTriKM, 
			ISNULL(dt.MaDoiTuong, '') as MaDoiTuong,ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong,ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau, 
			'' as TenDonViTinh,'' as LaHangHoa, '1' as TheoDoi, '0' as Xoa,
			null as TenHangHoa_KhongDau, null as TenHangHoa_KyTuDau,
			nv.MaNhanVien, nv.TenNhanVien,TenNhanVienChuCaiDau,TenNhanVienKhongDau, '00000000-0000-0000-0000-000000000000' as ID_NhomHang,
			case HinhThuc
				when 11 then N'Hóa đơn - Giảm hóa đơn'
				when 14 then N'Hóa đơn - Tặng điểm'
				end as sHinhThuc
		from BH_HoaDon hd
		join DM_KhuyenMai km on hd.ID_KhuyenMai = km.ID
		left join DM_DoiTuong dt on hd.ID_DoiTuong= dt.ID
		left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
		INNER JOIN (select * from splitstring(@ID_ChiNhanh)) lstID_DonVi ON lstID_DonVi.Name = hd.ID_DonVi			
		where hd.ID_KhuyenMai is not null
		and hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
		and hd.ChoThanhToan = 0

		union all

		select MaKhuyenMai,TenKhuyenMai,HinhThuc,kmhh.ID_DonVi, MaHoaDon,LoaiHoaDon, NgayLapHoaDon, TongTienHang,kmhh.NguoiTao, MaHangHoa,TenHangHoa,SoLuong, GiaTriKM,
			ISNULL(dt.MaDoiTuong, '') as MaDoiTuong, ISNULL(dt.TenDoiTuong, N'Khách lẻ') as TenDoiTuong, ISNULL(dt.TenDoiTuong_KhongDau, N'khach le') as TenDoiTuong_KhongDau, 
			TenDonViTinh, LaHangHoa,kmhh.TheoDoi,kmhh.Xoa,TenHangHoa_KhongDau,TenHangHoa_KyTuDau,
			MaNhanVien,TenNhanVien,TenNhanVienChuCaiDau,TenNhanVienKhongDau, ID_NhomHang,
			case HinhThuc
				when 12 then N'Hóa đơn - Tặng hàng'
				when 13 then N'Hóa đơn - Giảm giá hàng'
				when 21 then N'Hàng hóa - Giảm giá hàng'
				when 22 then N'Hàng hóa - Tặng hàng'
				when 23 then N'Hàng hóa - Tặng điểm'
				when 24 then N'Hàng hóa - Giá bán theo số lượng mua'
				end as sHinhThuc
		from (
		select hd.ID, hd.LoaiHoaDon, hd.ID_DoiTuong, hd.ID_NhanVien, hd.ID_DonVi, km.MaKhuyenMai, km.TenKhuyenMai, km.HinhThuc, hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienHang, qd.MaHangHoa, 
			--case when ctkm.TangKem ='1' then ctkm.SoLuong * ctkm.DonGia else ctkm.SoLuong * ctkm.TienChietKhau end as GiaTriKM,		
			case HinhThuc
				when 12 then ctkm.SoLuong * ctkm.DonGia
				when 22 then ctkm.SoLuong * ctkm.DonGia
				when 21 then ctkm.SoLuong *  ctkm.TienChietKhau
				when 13 then ctkm.SoLuong * ctkm.TienChietKhau
				when 24 then ctkm.SoLuong * ctkm.TienChietKhau
				when 23 then 0
				end as GiaTriKM,
			 hh.TenHangHoa, ctkm.SoLuong, hd.NguoiTao,
			 ctkm.ID_TangKem, ctkm.TangKem, qd.TenDonViTinh, hh.TheoDoi, qd.Xoa,
			 hh.TenHangHoa_KhongDau, hh.TenHangHoa_KyTuDau,hh.LaHangHoa,hh.ID_NhomHang
		from BH_HoaDon_ChiTiet ct
		join BH_HoaDon hd on ct.ID_HoaDon = hd.ID
		join DM_KhuyenMai km on ct.ID_KhuyenMai = km.ID
		join BH_HoaDon_ChiTiet ctkm on ct.ID_DonViQuiDoi = ctkm.ID_TangKem and ct.ID_HoaDon = ctkm.ID_HoaDon and (ctkm.ID_TangKem is not null or ctkm.Tangkem ='0')
		join DonViQuiDoi qd on ctkm.ID_DonViQuiDoi = qd.ID
		join DM_HangHoa hh on qd.ID_HangHoa= hh.ID
		INNER JOIN (select * from splitstring(@ID_ChiNhanh)) lstID_DonVi ON lstID_DonVi.Name = hd.ID_DonVi	
		where ct.ID_KhuyenMai is not null
		and hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd
		and hd.ChoThanhToan = 0) kmhh
		left join DM_DoiTuong dt on kmhh.ID_DoiTuong = dt.ID
		left join NS_NhanVien nv on kmhh.ID_NhanVien = nv.ID
		where kmhh.ID_TangKem is not null or TangKem ='1'
	) tbl
	inner join (SELECT ID FROM dbo.GetListNhomHangHoa(@ID_NhomHang)) allnhh ON tbl.ID_NhomHang = allnhh.ID		
	where tbl.LaHangHoa like @LaHangHoa
		and tbl.TheoDoi like @TheoDoi
    	and tbl.Xoa like @TrangThai
		and tbl.LoaiHoaDon in (select * from splitstring(@LoaiChungTu))
		AND ((select count(Name) from @tblSearchString b where 
		tbl.MaHoaDon like '%'+b.Name+'%' 
    	OR tbl.MaHoaDon like '%'+b.Name+'%' 
    	or tbl.MaHangHoa like '%'+b.Name+'%' 
    	or tbl.TenHangHoa like '%'+b.Name+'%'
    	or tbl.TenHangHoa_KhongDau like '%' +b.Name +'%' 
		or tbl.TenHangHoa_KyTuDau like '%' +b.Name +'%'
		or tbl.MaNhanVien like '%'+b.Name+'%'
		or tbl.TenNhanVien like '%'+b.Name+'%'
		or tbl.TenNhanVienChuCaiDau like '%'+b.Name+'%'
		or tbl.TenNhanVienKhongDau like '%'+b.Name+'%'
		or tbl.TenDonViTinh like '%'+b.Name+'%'
		or tbl.MaKhuyenMai like '%'+b.Name+'%'
		or tbl.TenKhuyenMai like '%'+b.Name+'%'
		or tbl.MaDoiTuong like '%'+b.Name+'%'
		or tbl.TenDoiTuong like '%'+b.Name+'%'
		or tbl.TenDoiTuong_KhongDau like '%'+b.Name+'%'
		or tbl.sHinhThuc like '%'+b.Name+'%'
		or dbo.FUNC_ConvertStringToUnsign(sHinhThuc) like '%'+b.Name+'%'
		)=@count or @count=0)
	order by NgayLapHoaDon desc");

            Sql(@"ALTER FUNCTION [dbo].[splitstring] ( @stringToSplit NVARCHAR(MAX) )
RETURNS
 @returnList TABLE ([Name] [nvarchar] (500))
AS
BEGIN

 DECLARE @name NVARCHAR(255)
 DECLARE @pos INT

 WHILE CHARINDEX(',', @stringToSplit) > 0
 BEGIN
  SELECT @pos  = CHARINDEX(',', @stringToSplit)  
  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)

  INSERT INTO @returnList 
  SELECT @name

  SELECT @stringToSplit = rtrim(ltrim(SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)))
 END

 INSERT INTO @returnList
 SELECT @stringToSplit

 RETURN
END");

            Sql(@"ALTER trigger [dbo].[trg_DMDoiTuong] on [dbo].[DM_DoiTuong]
for insert, update
as 
	DECLARE @IDDoiTuong UNIQUEIDENTIFIER
	SELECT @IDDoiTuong = i.ID
	FROM inserted i
	IF EXISTS(SELECT ID FROM Dm_DoiTuong
	WHERE ID =@IDDoiTuong)
		exec UpdateNhomDoiTuongs_ByID @IDDoiTuong
");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_CongNoI]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @loaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max)
AS
BEGIN
    DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    		 SELECT 
				MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    			MAX(b.MaKhachHang) as MaDoiTac,
    			MAX(b.TenKhachHang) as TenDoiTac,
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
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    		  Case When dtn.ID_NhomDoiTuong is null then
    		  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeStart
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    		)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi='0' and (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
    				) b
    				LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    		ORDER BY MAX(b.MaKhachHang) DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_CongNoII]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max)
AS
BEGIN
    DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    		 SELECT 
			 MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
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
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo,0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
    		  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    			SUM(bhd.PhaiThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			SUM(bhd.PhaiThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi ='0' and  (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_CongNoIII]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max)
AS
BEGIN
    DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    		 SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
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
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    	  Case When dtn.ID_NhomDoiTuong is null then
    	  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    			SUM(bhd.PhaiThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeEnd AND bhd.NgayLapHoaDon < @timeChotSo
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeEnd AND qhd.NgayLapHoaDon < @timeChotSo
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			SUM(bhd.PhaiThanhToan) AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    			0 AS DoanhThu,
    			0 AS TienThu,
    			0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    					left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where dt.TheoDoi = '0' and (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy
    			ORDER BY MAX(b.MaKhachHang) DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_CongNoIV]
    @MaKH [nvarchar](max),
    @MaKH_TV [nvarchar](max),
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @LoaiKH [nvarchar](max),
    @ID_NhomDoiTuong [nvarchar](max),
	@ID_NhomDoiTuong_SP [nvarchar](max)
AS
BEGIN
    	SELECT 
    		MAX(dt.TenNhomDoiTuongs) as NhomDoiTac,
    		MAX(b.MaKhachHang) as MaDoiTac,
    		MAX(b.TenKhachHang) as TenDoiTac,
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
    		  a.NoDauKy,
    		  a.GhiNo As TongTienChi,
    		  a.GhiCo As TongTienThu,
    		  CAST(ROUND(a.NoDauKy + a.GhiNo - a.GhiCo, 0) as float) as NoCuoiKy,
    		  Case When dtn.ID_NhomDoiTuong is null then
    		  Case When dt.LoaiDoiTuong = 1 then '00000010-0000-0000-0000-000000000010' else '30000000-0000-0000-0000-000000000003' end else dtn.ID_NhomDoiTuong end as ID_NhomDoiTuong
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
    			SELECT 
    				bhd.ID_DoiTuong,
    				0 AS CongNo,
    				0 AS GiaTriTra,
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeStart
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
				--and (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeStart
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				--and (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
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
    				SUM(bhd.PhaiThanhToan) AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri trả từ bán hàng
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    				SUM(bhd.PhaiThanhToan) AS GiaTriTra,
    				0 AS DoanhThu,
    				0 AS TienThu,
    				0 AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--and (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
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
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai  != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeStart AND qhd.NgayLapHoaDon < @timeEnd
				--and (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND dt.ID not in ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000002')
    			GROUP BY qhdct.ID_DoiTuong
    			) AS pstv
    		    GROUP BY pstv.ID_DoiTuong
    			)AS HangHoa
    			  	-- LEFT join DM_DoiTuong dt on HangHoa.ID_KhachHang = dt.ID
    				GROUP BY HangHoa.ID_KhachHang
    				) a
    				LEFT join DM_DoiTuong dt on a.ID_KhachHang = dt.ID
    				left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    				where  dt.TheoDoi ='0' and (dt.MaDoiTuong like @MaKH_TV or dt.MaDoiTuong like @maKH or dt.TenDoiTuong_ChuCaiDau like @maKH or dt.TenDoiTuong_KhongDau like @maKH or dt.DienThoai like @MaKH)
    				and dt.loaidoituong like @loaiKH
    				) b
    			LEFT JOin DM_DoiTuong dt on b.ID_KhachHang = dt.ID
				where b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong
    			Group by b.ID_KhachHang, dt.LoaiDoiTuong, b.NoDauKy, b.TongTienChi, b.TongTienThu, b.NoCuoiKy	
    			ORDER BY MAX(b.MaKhachHang) DESC
END");

            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTongQuan_BieuDoDoanhThuToHour]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_NguoiDung [uniqueidentifier],
	@ID_DonVi nvarchar (max)
AS
BEGIN
	 DECLARE @LaAdmin as nvarchar
     Set @LaAdmin = (Select nd.LaAdmin From HT_NguoiDung nd	where nd.ID = @ID_NguoiDung)
	 IF(@LaAdmin = 1)
	 BEGIN
		SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
			-- tongmua
    		SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		hdb.PhaiThanhToan as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19 Or hdb.LoaiHoaDon = 22)
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

			union all
			-- tongtra
			SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		- hdb.PhaiThanhToan as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

			union all
			-- dieuchinhthe
			SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		hdb.TongChiPhi as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.LoaiHoaDon = 23 
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

		) a
    	GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon
		END
	ELSE
	BEGIN
	SELECT 
		a.NgayLapHoaDon,
		a.TenChiNhanh,
		CAST(ROUND(SUM(a.ThanhTien), 0) as float) as ThanhTien
		FROM
		(
			-- tongmua
    		SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		hdb.PhaiThanhToan as ThanhTien
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    		and (hdb.LoaiHoaDon = 1 Or hdb.LoaiHoaDon = 19 or hdb.LoaiHoaDon = 22)
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

			union all
			-- tongtra
			SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		- hdb.PhaiThanhToan as ThanhTien 
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    		and hdb.LoaiHoaDon = 6
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))

			union all
			-- dieuchinhthe
			SELECT
    		hdb.ID as ID_HoaDon,
			DAY(hdb.NgayLapHoaDon) as NgayLapHoaDon,
			dv.TenDonVi as TenChiNhanh,
    		hdb.TongChiPhi as ThanhTien 
    		FROM
    		BH_HoaDon hdb
			join DM_DonVi dv on hdb.ID_DonVi = dv.ID
    		where hdb.NgayLapHoaDon >= @timeStart and hdb.NgayLapHoaDon < @timeEnd
    		and hdb.ChoThanhToan = 0
    		and hdb.ID_DonVi in (select ct.ID_DonVi from HT_NguoiDung nd 
									join NS_NhanVien nv on nv.ID = nd.ID_NhanVien 
									join NS_QuaTrinhCongTac ct on ct.ID_NhanVien = nv.ID 
									 where nd.ID = @ID_NguoiDung)
    		and hdb.LoaiHoaDon = 23
			and hdb.ID_DonVi in (select * from splitstring(@ID_DonVi))
		) a
    	GROUP BY a.NgayLapHoaDon, a.TenChiNhanh
		ORDER BY NgayLapHoaDon
	END
END

--BaoCaoTongQuan_BieuDoDoanhThuToHour '2019-09-13','2019-09-14','28FEF5A1-F0F2-4B94-A4AD-081B227F3B77','D93B17EA-89B9-4ECF-B242-D03B8CDE71DE'");

            Sql(@"ALTER PROCEDURE [dbo].[DanhMucKhachHang_CongNo_ChotSo]
    @timeStart [datetime],
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier],
    @MaKH [nvarchar](max),
    @LoaiKH [int],
    @ID_NhomKhachHang [nvarchar](max),
    @timeStartKH [datetime],
    @timeEndKH [datetime]
AS
BEGIN
    set nocount on

	declare @tblIDNhoms table (ID varchar(36))
	if @ID_NhomKhachHang ='%%'
		begin
			-- check QuanLyKHTheochiNhanh
			declare @QLTheoCN bit = (select QuanLyKhachHangTheoDonVi from HT_CauHinhPhanMem where ID_DonVi like @ID_ChiNhanh)
			insert into @tblIDNhoms(ID) values ('00000000-0000-0000-0000-000000000000')

			if @QLTheoCN = 1
				begin									
					insert into @tblIDNhoms(ID)
					select *  from (
						-- get Nhom not not exist in NhomDoiTuong_DonVi
						select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
						where not exists (select ID_NhomDoiTuong from NhomDoiTuong_DonVi where ID_NhomDoiTuong= nhom.ID) 
						and LoaiDoiTuong = @LoaiKH --and (TrangThai is null or TrangThai = 1)
						union all
						-- get Nhom at this ChiNhanh
						select convert(varchar(36),ID_NhomDoiTuong)  from NhomDoiTuong_DonVi where ID_DonVi like @ID_ChiNhanh) tbl
				end
			else
				begin				
				-- insert all
				insert into @tblIDNhoms(ID)
				select convert(varchar(36),ID) as ID_NhomDoiTuong from DM_NhomDoiTuong nhom  
				where LoaiDoiTuong = @LoaiKH --and (TrangThai is null or TrangThai = 1)
				end

			--select * from @tblIDNhoms
		end
	else
		begin
			set @ID_NhomKhachHang = REPLACE(@ID_NhomKhachHang,'%','')
			insert into @tblIDNhoms(ID) values (@ID_NhomKhachHang)
		end

	DECLARE @timeChotSo Datetime
    	Select @timeChotSo =  (Select NgayChotSo FROM ChotSo where ID_DonVi = @ID_ChiNhanh);
    		
    		SELECT * 
    		FROM
    		(
    		 SELECT 
    		 dt.ID as ID,
    		 dt.MaDoiTuong, 
			 dt.ID_TrangThai,
			 case when dt.IDNhomDoiTuongs='' then '00000000-0000-0000-0000-000000000000' else  ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') end as ID_NhomDoiTuong,
			 --ISNULL(dt.IDNhomDoiTuongs,'00000000-0000-0000-0000-000000000000') as ID_NhomDoiTuong,
    		  dt.TenDoiTuong,
    		  dt.TenDoiTuong_KhongDau,
    		  dt.TenDoiTuong_ChuCaiDau,
    		  dt.GioiTinhNam,
    		  dt.NgaySinh_NgayTLap,
    		  dt.DienThoai,
    		  dt.Email,
    		  dt.DiaChi,
    		  dt.MaSoThue,
    		  ISNULL(dt.GhiChu,'') as GhiChu,
    		  dt.NgayTao,
    		  dt.NguoiTao,
    		  dt.DinhDang_NgaySinh,
    		  dt.ID_NguonKhach,
    		  dt.ID_NhanVienPhuTrach,
    		  dt.ID_NguoiGioiThieu,
    		  dt.LaCaNhan,
    		  dt.ID_TinhThanh,
    		  dt.ID_QuanHuyen,
			  ISNULL(dt.TrangThai_TheGiaTri,1) as TrangThai_TheGiaTri,
			  ISNULL(trangthai.TenTrangThai,'') as TrangThaiKhachHang,
			  case when right(rtrim(dt.TenNhomDoiTuongs),1) =',' then LEFT(Rtrim(dt.TenNhomDoiTuongs), len(dt.TenNhomDoiTuongs)-1) else ISNULL(dt.TenNhomDoiTuongs,N'Nhóm mặc định') end as TenNhomDT,-- remove last coma
			  --ISNULL(dt.TenNhomDoiTuongs,N'Nhóm mặc định') as TenNhomDT,
    		  ISNULL(dt.TongTichDiem,0) as TongTichDiem,
			  ISNULL(qh.TenQuanHuyen,'') as PhuongXa,
			  ISNULL(tt.TenTinhThanh,'') as KhuVuc,
			  ISNULL(dv.TenDonVi,'') as ConTy,
			  ISNULL(dv.SoDienThoai,'') as DienThoaiChiNhanh,
			  ISNULL(dv.DiaChi,'') as DiaChiChiNhanh,
			  ISNULL(nk.TenNguonKhach,'') as TenNguonKhach,
			  ISNULL(dt2.TenDoiTuong,'') as NguoiGioiThieu,
    	      CAST(ROUND(ISNULL(a.NoHienTai,0), 0) as float) as NoHienTai,
    		  CAST(ROUND(ISNULL(a.TongBan,0), 0) as float) as TongBan,
    		  CAST(ROUND(ISNULL(a.TongBanTruTraHang,0), 0) as float) as TongBanTruTraHang,
    		  CAST(ROUND(ISNULL(a.TongMua,0), 0) as float) as TongMua,
    		  CAST(ROUND(ISNULL(a.SoLanMuaHang,0), 0) as float) as SoLanMuaHang,
			  CAST(0 as float) as TongNapThe , 
			  CAST(0 as float) as SuDungThe , 
			  CAST(0 as float) as HoanTraTheGiaTri , 
			  CAST(0 as float) as SoDuTheGiaTri , 
			  concat(dt.MaDoiTuong,' ',dt.TenDoiTuong,' ',ISNULL(dt.DienThoai,''),' ', ISNULL(dt.TenDoiTuong_KhongDau,''))  as Name_Phone
    	  FROM
    			DM_DoiTuong dt
    			LEFT join DM_TinhThanh tt on dt.ID_TinhThanh = tt.ID
    			LEFT join DM_QuanHuyen qh on dt.ID_QuanHuyen = qh.ID
				LEFT join DM_NguonKhachHang nk on dt.ID_NguonKhach = nk.ID
				LEFT join DM_DoiTuong dt2 on dt.ID_NguoiGioiThieu = dt2.ID
    			LEFT join DM_DonVi dv on dt.ID_DonVi = dv.ID
    			LEFT join DM_DoiTuong_TrangThai trangthai on dt.ID_TrangThai = trangthai.ID
    			LEFT Join
    		(
    			  SELECT HangHoa.ID_DoiTuong,
    				SUM(ISNULL(HangHoa.NoHienTai, 0)) as NoHienTai, 
    				SUM(ISNULL(HangHoa.TongBan, 0)) as TongBan,
    				SUM(ISNULL(HangHoa.TongBanTruTraHang, 0)) as TongBanTruTraHang,
    				SUM(ISNULL(HangHoa.TongMua, 0)) as TongMua,
    				SUM(ISNULL(HangHoa.SoLanMuaHang, 0)) as SoLanMuaHang
    				FROM
    				(
    					SELECT
    						td.ID_DoiTuong,
    						SUM(ISNULL(td.CongNo, 0)) + SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS NoHienTai,
    						0 AS TongBan,
    						0 AS TongBanTruTraHang,
    						0 AS TongMua,
    						0 AS SoLanMuaHang
    					FROM
    					(
    					-- Chốt sổ
    						SELECT 
    							ID_KhachHang As ID_DoiTuong,
    							ISNULL(CongNo, 0) AS CongNo,
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
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeChotSo
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong,
    							0 AS CongNo,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeChotSo
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- sổ quỹ thu
    						UNION ALL
    						SELECT 
    							qhdct.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    							0 AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
							Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    						WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null)  AND qhd.NgayLapHoaDon >= @timeChotSo
							--AND (qhd.HachToanKinhDoanh = 0 or qhd.HachToanKinhDoanh = '1')
    						AND qhd.ID_DonVi = @ID_ChiNhanh
							AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    						GROUP BY qhdct.ID_DoiTuong
							-- So Quy chi
    						UNION ALL
    						SELECT 
    							qhdct.ID_DoiTuong,
    							0 AS CongNo,
    							0 AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    						FROM Quy_HoaDon qhd
    						JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    						WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon >= @timeChotSo
							--AND (qhd.HachToanKinhDoanh  = 0 or qhd.HachToanKinhDoanh = '1')
    						AND qhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY qhdct.ID_DoiTuong							

    					) AS td
    						GROUP BY td.ID_DoiTuong
    						UNION ALL
    							-- Tổng bán phát sinh trong khoảng thời gian truy vấn
    						SELECT
    							pstv.ID_DoiTuong ,
    							0 AS NoHienTai,
    							SUM(ISNULL(pstv.DoanhThu,0)) AS TongBan,
    							SUM(ISNULL(pstv.DoanhThu,0)) -  SUM(ISNULL(pstv.GiaTriTra,0)) AS TongBanTruTraHang,
    							SUM(ISNULL(pstv.GiaTriTra,0)) AS TongMua,
    							0 AS SoLanMuaHang
    						FROM
    						(
    						SELECT 
    							bhd.ID_DoiTuong,
    							0 AS GiaTriTra,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						-- gia tri trả từ bán hàng
    						UNION All
    						SELECT bhd.ID_DoiTuong,
    							SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    							0 AS DoanhThu,
    							0 AS TienThu,
    							0 AS TienChi
    						FROM BH_HoaDon bhd
    						WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false'  AND bhd.NgayLapHoaDon >= @timeStart AND bhd.NgayLapHoaDon < @timeEnd
    						AND bhd.ID_DonVi = @ID_ChiNhanh
    						GROUP BY bhd.ID_DoiTuong
    						) AS pstv
    						GROUP BY pstv.ID_DoiTuong
    							Union All
    							Select 
    								hd.ID_DoiTuong,
    								0 AS NoHienTai,
    								0 AS TongBan,
    								0 AS TongBanTruTraHang,
    								0 AS TongMua,
    								COUNT(*) AS SoLanMuaHang
    							From BH_HoaDon hd 
    							where (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 19)
    							and hd.ChoThanhToan = 0
    							AND hd.NgayLapHoaDon >= @timeStart AND hd.NgayLapHoaDon < @timeEnd 
    							And hd.ID_DonVi = @ID_ChiNhanh 
    							GROUP BY hd.ID_DoiTuong
    					)AS HangHoa
    						GROUP BY HangHoa.ID_DoiTuong
    				) a
    					on dt.ID = a.ID_DoiTuong
    				where (dt.MaDoiTuong LIKE  @MaKH  
					     OR dt.TenDoiTuong_ChuCaiDau LIKE  @MaKH  
						 OR dt.TenDoiTuong_KhongDau LIKE  @MaKH 
						 OR dt.TenDoiTuong LIKE  @MaKH 
    					 OR dt.DienThoai LIKE  @MaKH)

    				and dt.loaidoituong = @loaiKH
    					and dt.NgayTao >= @timeStartKH and dt.NgayTao < @timeEndKH
    						AND dt.TheoDoi =0
    				)b
					--INNER JOIN @tblIDNhoms tblsearch ON CHARINDEX(CONCAT(', ', tblsearch.ID, ', '), CONCAT(', ', b.ID_NhomDoiTuong, ', '))>0
    				where b.ID not like '%00000000-0000-0000-0000-0000%'
					and EXISTS(SELECT Name FROM splitstring(b.ID_NhomDoiTuong) lstFromtbl INNER JOIN @tblIDNhoms tblsearch ON lstFromtbl.Name = tblsearch.ID)
    	ORDER BY b.NgayTao desc
END

--DanhMucKhachHang_CongNo_ChotSo '2016-01-01','2019-12-30','d93b17ea-89b9-4ecf-b242-d03b8cde71de','%%',1,'%%','2016-01-01','2019-12-30'
-- DanhMucKhachHang_CongNo_ChotSo_Old '2016-01-01','2019-12-30','d93b17ea-89b9-4ecf-b242-d03b8cde71de','%%',1,'%%','2016-01-01','2019-12-30'
--select TenDoiTuong_KhongDau from DM_DoiTuong

");
            Sql(@"ALTER PROCEDURE [dbo].[GetMaHoaDon_AuTo]
    @LoaiHoaDon [int]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @MaHoaDon varchar(5);
    	DECLARE @Return float
    
    if @LoaiHoaDon = 22 --Phieu thẻ giá trị --
    	set @MaHoaDon ='TGT'

	if @LoaiHoaDon = 23 --Phieu dc thẻ giá trị --
    	set @MaHoaDon ='DCGT'			

    if @LoaiHoaDon = 4 --Phiếu nhập --
    	set @MaHoaDon ='PNK'	
    if @LoaiHoaDon = 7 --Trả hàng NCC --
    	set @MaHoaDon ='THNCC'
	
	if @LoaiHoaDon = 8 --Xuất hủy --
    	set @MaHoaDon ='XH'

	if @LoaiHoaDon = 18 --Xuất hủy --
    	set @MaHoaDon ='DCGV'

	if @LoaiHoaDon = 10 --Chuyển hàng--
    	set @MaHoaDon ='CH'

	if @LoaiHoaDon = 9 --Kiểm hàng --
    	set @MaHoaDon ='PKK'

    	if @LoaiHoaDon = 88 --Hàng hóa --
    	set @MaHoaDon ='HH0'
    
    	if @LoaiHoaDon = 99 --Dịch vụ --
    	set @MaHoaDon ='DV0'
    
    	if(@LoaiHoaDon != 99 and @LoaiHoaDon != 88)
    	BEGIN
    	SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaHoaDon) AS float))
    	FROM BH_HoaDon WHERE CHARINDEX(@MaHoaDon,MaHoaDon) > 0 and CHARINDEX('Copy',MaHoaDon)= 0 and CHARINDEX('_',MaHoaDon)= 0
    
    		if	@Return is null 
    			select Cast(0 as float) as MaxCode
    		else 
    			select @Return as MaxCode
    	END
    	ELSE
    	BEGIN
    		SELECT @Return = MAX(CAST (dbo.udf_GetNumeric(MaHangHoa) AS float))
    	FROM DonViQuiDoi WHERE CHARINDEX(@MaHoaDon,MaHangHoa) > 0 and CHARINDEX('Copy',MaHangHoa)= 0 and CHARINDEX('_',MaHangHoa)= 0
    
    		if	@Return is null 
    			select Cast(0 as float) as MaxCode
    		else 
    			select @Return as MaxCode
    	END
END");

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NS_KyHieuCong", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropIndex("dbo.NS_KyHieuCong", new[] { "ID_KyTinhCong" });
            DropColumn("dbo.NS_ChamCong_ChiTiet", "SoGioOT");
            DropColumn("dbo.NS_KyHieuCong", "ID_KyTinhCong");
            DropStoredProcedure("[dbo].[SP_BaoCaoKhuyenMai]");
        }
    }
}
