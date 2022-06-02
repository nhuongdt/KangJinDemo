namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSP_20190327 : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER PROCEDURE [dbo].[BaoCaoTaiChinh_SoQuyTheoChiNhanh]
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
    	SELECT
			c.ID_DonVi,
			MAX(c.TenDonVi) as TenDonVi,
			CAST(ROUND(SUM(c.ThuTienMat - c.ChiTienMat), 0) as float) as TienMat,
			CAST(ROUND(SUM(c.ThuTienGui - c.ChiTienGui), 0) as float) as TienGui,
			CAST(ROUND(SUM(c.ThuTienMat - c.ChiTienMat + c.ThuTienGui - c.ChiTienGui), 0) as float) as TongThu
    	  FROM 
    		(
    		 SELECT 
				MAX(b.ID_DonVi) as ID_DonVi,
    			MAX(b.TenDonVi) as TenDonVi,
    			MAX (b.ThuTienGui) as ThuTienGui,
    			MAX (b.ChiTienGui) as ChiTienGui, 
    			MAX (b.ThuTienMat) as ThuTienMat,
    			MAX (b.ChiTienMat) as ChiTienMat
    		FROM
    		(
				select 
				a.ID_DonVi, 
				a.TenDonVi,
    			a.HachToanKinhDoanh,
    			a.ID_NhomDoiTuong,
    			a.ID_DoiTuong,
    			a.ID_HoaDon,
    			a.MaHoaDon,
    			a.MaPhieuThu,
    			case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
    			Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
				MAX(qhd.ID_DonVi) as ID_DonVi,
				MAX(dv.TenDonVi) as TenDonVi,
    			MAX(dt.ID) as ID_DoiTuong,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
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
    			dt.DienThoai,
    			qhd.MaHoaDon as MaPhieuThu,
    			Case when qhd.NguoiNopTien is null or qhd.NguoiNopTien = '' then N'Khách lẻ' else qhd.NguoiNopTien end as TenNguoiNop,
    			MAX(ISNULL(qhdct.TienMat,0)) as TienMat,
    			MAX(ISNULL(qhdct.TienGui,0)) as TienGui,
    			MAX(ISNULL(qhdct.TienThu,0)) as TienThu,
    			qhd.NgayLapHoaDon,
    			MAX(qhd.NoiDungThu) as GhiChu,
    			hd.MaHoaDon
    		From Quy_HoaDon qhd 
    			inner join Quy_HoaDon_ChiTiet qhdct on qhd.ID = qhdct.ID_HoaDon
    			left join BH_HoaDon hd on qhdct.ID_HoaDonLienQuan = hd.ID
    			left join DM_DoiTuong dt on qhdct.ID_DoiTuong = dt.ID
    			left join DM_DoiTuong_Nhom dtn on dt.ID = dtn.ID_DoiTuong
    			left join Quy_KhoanThuChi ktc on qhdct.ID_KhoanThuChi = ktc.ID
    			left join DM_TaiKhoanNganHang tknh on qhdct.ID_TaiKhoanNganHang = tknh.ID
    			left join DM_NganHang nh on qhdct.ID_NganHang = nh.ID
				inner join DM_DonVi dv on qhd.ID_DonVi = dv.ID
    		where qhd.NgayLapHoaDon >= @timeStart and qhd.NgayLapHoaDon < @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			Group by b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon
    		) as c
			GROUP BY c.ID_DonVi
END");

            Sql(@"ALTER PROCEDURE [dbo].[selectKhachHang_CongNo]
    @timeEnd [datetime],
    @ID_ChiNhanh [uniqueidentifier]
AS
BEGIN
    SELECT 
    		NEWID() AS ID,
    		*, @ID_ChiNhanh as ID_DonVi, @timeEnd as NgayChotSo
    		FROM
    		(
    			SELECT
    			td.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(td.DoanhThu,0)) + SUM(ISNULL(td.TienChi,0)) - SUM(ISNULL(td.TienThu,0)) - SUM(ISNULL(td.GiaTriTra,0)) AS CongNo
    			FROM
    			(
    			-- Doanh thu tu ban hang
    			SELECT 
    			bhd.ID_DoiTuong,
    			NULL AS GiaTriTra,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '1' OR bhd.LoaiHoaDon = '7' OR bhd.LoaiHoaDon = '19') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			-- gia tri tra tu ban hang
    			UNION All
    			SELECT bhd.ID_DoiTuong AS ID_KhachHang,
    			SUM(ISNULL(bhd.PhaiThanhToan,0)) AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			NULL AS TienChi
    			FROM BH_HoaDon bhd
    			JOIN DM_DoiTuong dt ON dt.ID = bhd.ID_DoiTuong
    			WHERE (bhd.LoaiHoaDon = '6' OR bhd.LoaiHoaDon = '4') AND bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeEnd
    			AND bhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY bhd.ID_DoiTuong
    			
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienThu,
    			NULL AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
				Left join BH_HoaDon bhd on qhdct.ID_HoaDonLienQuan = bhd.ID -- thêm thẻ
    			WHERE qhd.LoaiHoaDon = '11' AND  (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeEnd
				AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
				AND (bhd.LoaiHoaDon is null or bhd.LoaiHoaDon != 22)
    			GROUP BY qhdct.ID_DoiTuong
    
    			
    			UNION ALL
    			SELECT 
    			qhdct.ID_DoiTuong AS ID_KhachHang,
    			NULL AS GiaTriTra,
    			NULL AS DoanhThu,
    			NULL AS TienThu,
    			SUM(ISNULL(qhdct.TienThu,0)) AS TienChi
    			FROM Quy_HoaDon qhd
    			JOIN Quy_HoaDon_ChiTiet qhdct ON qhd.ID = qhdct.ID_HoaDon
    			JOIN DM_DoiTuong dt ON dt.ID = qhdct.ID_DoiTuong
    			WHERE qhd.LoaiHoaDon = '12' AND (qhd.TrangThai != '0' OR qhd.TrangThai is null) AND qhd.NgayLapHoaDon < @timeEnd
				AND (qhd.HachToanKinhDoanh is null or qhd.HachToanKinhDoanh = '1')
    			AND qhd.ID_DonVi = @ID_ChiNhanh
    			GROUP BY qhdct.ID_DoiTuong
    			) AS td
    		    GROUP BY td.ID_DoiTuong
    		) 
    		  AS HangHoa
END");
            
        }
        
        public override void Down()
        {
        }
    }
}
