namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateSP_getlistHangHoa_XNT : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getlistHangHoa_XNT]",
                parametersAction: p => new {
                    MaHH = p.String(20),
                    timeStart = p.DateTime(),
                    timeEnd = p.DateTime(),
                    TenHH = p.String(50),
                    ID_ChiNhanh = p.Guid()
                },
                body: @"SELECT a.* FROM 
(
 SELECT 
 dhh.ID,
 dvqd.MaHangHoa,
 MAX(dhh.TenHangHoa)   AS TenHangHoa,
 MAX(dvqd.TenDonViTinh)   AS TenDonViTinh,
 MAX(dnhh.MaNhomHangHoa)  AS MaNhomHangHoa,
 MAX(dnhh.TenNhomHangHoa) AS TenNhomHangHoa,
 SUM(ISNULL(HangHoa.TonDau,0))  AS TonDau,
 SUM(ISNULL(HangHoa.SoLuongNhap,0))  AS SoLuongNhap,
 SUM(ISNULL(HangHoa.SoLuongXuat,0))  AS SoLuongXuat,
 SUM(ISNULL(HangHoa.TonDau,0)) + SUM(ISNULL(HangHoa.SoLuongNhap,0)) - SUM(ISNULL(HangHoa.SoLuongXuat,0)) AS TonKho
 FROM
 (
  SELECT
  td.ID_DonViQuiDoi,
  SUM(ISNULL(td.SoLuongNhap,0)) - SUM(ISNULL(td.SoLuongXuat,0)) AS TonDau,
  NULL AS SoLuongNhap,
  NULL AS SoLuongXuat
  FROM
  (
   SELECT 
   bhdct.ID_DonViQuiDoi,
   NULL AS SoLuongNhap,
   SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false' AND bhd.NgayLapHoaDon < @timeStart
   AND bhd.ID_DonVi = @ID_ChiNhanh
   GROUP BY bhdct.ID_DonViQuiDoi                                                                                                                                                                                                                                                             
   
   UNION ALL
   SELECT 
   bhdct.ID_DonViQuiDoi,
   NULL AS SoLuongNhap,
   SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
   OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
   AND bhd.ID_DonVi = @ID_ChiNhanh
   AND bhd.NgayLapHoaDon < @timeStart
   GROUP BY bhdct.ID_DonViQuiDoi

   UNION ALL
   SELECT 
   bhdct.ID_DonViQuiDoi,
   SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
   null AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') AND bhd.ChoThanhToan = 0
   AND bhd.ID_DonVi = @ID_ChiNhanh
   AND bhd.NgayLapHoaDon < @timeStart
   GROUP BY bhdct.ID_DonViQuiDoi
   
   UNION ALL
   SELECT 
   bhdct.ID_DonViQuiDoi,
   SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
   null AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = '10' and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
   AND bhd.ID_DonVi = @ID_ChiNhanh
   AND bhd.NgayLapHoaDon < @timeStart
   GROUP BY bhdct.ID_DonViQuiDoi
  ) AS td
  GROUP BY td.ID_DonViQuiDoi
  
  UNION ALL
  SELECT
  pstk.ID_DonViQuiDoi,
  NULL AS TonDau,
  SUM(pstk.SoLuongNhap) AS SoLuongNhap,
  SUM(pstk.SoLuongXuat) AS SoLuongXuat
  FROM 
  (
   SELECT 
   bhdct.ID_DonViQuiDoi,
   NULL AS SoLuongNhap,
   SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE (bhd.LoaiHoaDon = '1' or bhd.LoaiHoaDon = '5' or bhd.LoaiHoaDon = '7' or bhd.LoaiHoaDon = '8') AND dvqd.Xoa IS NULL and bhd.ChoThanhToan = 'false'
   AND bhd.ID_DonVi = @ID_ChiNhanh
   AND (bhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd)
   GROUP BY bhdct.ID_DonViQuiDoi

   UNION ALL
   SELECT 
   bhdct.ID_DonViQuiDoi,
   NULL AS SoLuongNhap,
   SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE ((bhd.LoaiHoaDon = '10' and bhd.yeucau = '1') 
   OR (bhd.ID_CheckIn is not null and bhd.ID_CheckIn != @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4')) AND bhd.ChoThanhToan = 0
   AND bhd.ID_DonVi = @ID_ChiNhanh
   AND (bhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd)
   GROUP BY bhdct.ID_DonViQuiDoi

   UNION ALL
   SELECT 
   bhdct.ID_DonViQuiDoi,
   SUM(ISNULL(bhdct.SoLuong,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
   null AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE (bhd.LoaiHoaDon = '4' or bhd.LoaiHoaDon = '6' or bhd.LoaiHoaDon = '9') 
   AND bhd.ChoThanhToan = 0
   AND bhd.ID_DonVi = @ID_ChiNhanh
   AND (bhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd)
   GROUP BY bhdct.ID_DonViQuiDoi
   
   UNION ALL
   SELECT 
   bhdct.ID_DonViQuiDoi,
   SUM(ISNULL(bhdct.tienchietkhau,0)* dvqd.TyLeChuyenDoi) AS SoLuongNhap,
   null AS SoLuongXuat
   FROM BH_HoaDon_ChiTiet bhdct
   LEFT JOIN BH_HoaDon bhd ON bhdct.ID_HoaDon = bhd.ID
   LEFT JOIN DonViQuiDoi dvqd ON dvqd.ID = bhdct.ID_DonViQuiDoi
   WHERE (bhd.ID_CheckIn is not null and bhd.ID_CheckIn = @ID_ChiNhanh and bhd.LoaiHoaDon = 10 and bhd.YeuCau = '4') AND bhd.ChoThanhToan = 0
   AND bhd.ID_DonVi = @ID_ChiNhanh
   AND (bhd.NgayLapHoaDon BETWEEN @timeStart AND @timeEnd)
   GROUP BY bhdct.ID_DonViQuiDoi
  ) AS pstk
  GROUP BY pstk.ID_DonViQuiDoi
 ) 
 AS HangHoa
 LEFT JOIN (SELECT * FROM DonViQuiDoi dvqd2 WHERE dvqd2.LaDonViChuan =1) dvqd ON dvqd.ID = HangHoa.ID_DonViQuiDoi
 LEFT JOIN DM_HangHoa dhh ON dhh.ID = dvqd.ID_HangHoa
 LEFT JOIN DM_NhomHangHoa dnhh ON dnhh.ID = dhh.ID_NhomHang
 WHERE 
 DHH.TheoDoi = 1 AND dhh.LaHangHoa = 1
 GROUP BY dhh.ID, dvqd.MaHangHoa
) a
where MaHangHoa like @MaHH or TenHangHoa like @TenHH
order by TonKho desc");
        }
        
        public override void Down()
        {
        }
    }
}
