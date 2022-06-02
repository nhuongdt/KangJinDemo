namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20190218 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[BaoCaoTaiChinh_ChiNhanh]", parametersAction: p => new
            {
                MaKH = p.String(),
                MaKH_TV = p.String(),
                timeEnd = p.DateTime(),
                ID_ChiNhanh = p.String(),
                loaiKH = p.String(),
                ID_NhomDoiTuong = p.String(),
                ID_NhomDoiTuong_SP = p.String(),
                lstThuChi = p.String(),
                HachToanKD = p.String()
            }, body: @"SELECT
	c.ID_DonVi as ID, c.TenDonVi,
	SUM (c.ThuTienMat -c.ChiTienMat) as TonTienMat,
    SUM (c.ThuTienGui - c.ChiTienGui) as TonTienGui,
	SUM (c.TongThuChi) as TongThuChi
	FROM
	(
    		 SELECT 
    			b.ID_DonVi,
				MAX (b.TenDonVi) as TenDonVi,
				MAX (b.ThuTienMat) as ThuTienMat,
    			MAX (b.ChiTienMat) as ChiTienMat,
    			MAX (b.ThuTienGui) as ThuTienGui,
    			MAX (b.ChiTienGui) as ChiTienGui, 
				MAX (b.ThuTienMat + b.ThuTienGui - b.ChiTienMat - b.ChiTienGui) as TongThuChi
    		FROM
    		(
				select 
				a.ID_DonVi,
				a.ID_HoaDon,
				a.ID_DoiTuong,
				a.MaHoaDon,
				a.TenDonVi,
				a.ID_NhomDoiTuong,
    			case when a.LoaiHoaDon = 11 then a.TienGui else 0 end as ThuTienGui,
    			Case when a.LoaiHoaDon = 12 then a.TienGui else 0 end as ChiTienGui,
    			case when a.LoaiHoaDon = 11 then a.TienMat else 0 end as ThuTienMat,
    			Case when a.LoaiHoaDon = 12 then a.TienMat else 0 end as ChiTienMat,
    			Case when a.TienMat > 0 and TienGui = 0 then '1'  
    			 when a.TienGui > 0 and TienMat = 0 then '2' 
    			 when a.TienGui > 0 and TienMat > 0 then '12' else '' end  as LoaiTien
    		From
    		(
    		select 
    			qhd.LoaiHoaDon,
    			MAX(qhd.ID) as ID_HoaDon,
    			MAX(dt.ID) as ID_DoiTuong,
				MAX (dv.ID) as ID_DonVi,
				MAX(dv.TenDonVi) as TenDonVi,
    			Case when qhd.HachToanKinhDoanh is null then 1 else qhd.HachToanKinhDoanh end as HachToanKinhDoanh,
    			Case when qhd.LoaiHoaDon = 11 and hd.LoaiHoaDon is null then 1 else -- phiếu thu khác
    			Case when (qhd.LoaiHoaDon = 12 and hd.LoaiHoaDon is null) or ((hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 12) then 2 else -- phiếu chi khác
    			Case when (hd.LoaiHoaDon = 1 or hd.LoaiHoaDon = 3 or hd.LoaiHoaDon = 19) and qhd.LoaiHoaDon = 11 then 3 else -- bán hàng
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
    			Sum(ISNULL(qhdct.TienMat,0)) as TienMat,
    			Sum(ISNULL(qhdct.TienGui,0)) as TienGui,
    			qhd.NgayLapHoaDon,
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
    		where qhd.NgayLapHoaDon < @timeEnd
    			and (qhd.TrangThai != '0' OR qhd.TrangThai is null)
    			and (dt.loaidoituong like @loaiKH or dt.LoaiDoiTuong is null)
    			and qhd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
    			and qhdct.DiemThanhToan is null
    		Group by qhd.LoaiHoaDon, hd.LoaiHoaDon, dt.MaDoiTuong,dt.LoaiDoiTuong, dt.TenDoiTuong_ChuCaiDau, dt.TenDoiTuong_KhongDau, 
    			 qhd.HachToanKinhDoanh, dt.DienThoai, qhd.MaHoaDon, qhd.NguoiNopTien, qhd.NgayLapHoaDon, hd.MaHoaDon, dtn.ID_NhomDoiTuong, dv.ID, dtn.ID
    		)a
    		where (a.DienThoai like @MaKH or a.TenKhachHang_ChuCaiDau like @MaKH or a.TenKhachHang_KhongDau like @MaKH or a.MaKhachHang like @MaKH or a.MaKhachHang like @MaKH_TV or a.MaPhieuThu like @MaKH or a.MaPhieuThu like @MaKH_TV)	
    		and a.LoaiThuChi in (select * from splitstring(@lstThuChi))
    		and a.HachToanKinhDoanh like @HachToanKD
    		) b
				where (b.ID_NhomDoiTuong in (select * from splitstring(@ID_NhomDoiTuong_SP)) or b.ID_NhomDoiTuong like @ID_NhomDoiTuong)
    			Group by  b.ID_HoaDon, b.ID_DoiTuong, b.MaHoaDon, b.ID_DonVi
				) as c
				GROUP BY c.ID_DonVi, c.TenDonVi
				ORDER BY c.TenDonVi");
            CreateStoredProcedure(name: "[dbo].[SP_GetHoaDonChoThanhToan]", parametersAction: p => new
            {
                LoaiHoaDon = p.Int()
            }, body: @"select 
	hd.ID, hd.ID_DoiTuong, hd.ID_BangGia, hd.ID_NhanVien, hd.ID_ViTri, hd.MaHoaDon, hd.NgayLapHoaDon, hd.TongTienThue, hd.TongGiamGia, hd.TongChietKhau, 
	hd.TongChiPhi, hd.PhaiThanhToan, ISNULL(hd.ChoThanhToan,'0') as ChoThanhToan,
	hd.DienGiai, hd.ChoThanhToan, hd.KhuyeMai_GiamGia, ISNULL(hd.YeuCau,'') as YeuCau, hd.LoaiHoaDon,
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
where hd.LoaiHoaDon like @LoaiHoaDon");

            CreateStoredProcedure(name: "[dbo].[SP_GetListNhanVien_inDepartment]", parametersAction: p => new
            {
                ID_PhongBans = p.String(),
                ID_DonVi = p.String()
            }, body: @"-- phai khai bao @lstID_PhongBans de check dieu kien OR trong lenh if
	declare @lstID_PhongBans nvarchar(max) = @ID_PhongBans
	if	(@lstID_PhongBans ='' or @lstID_PhongBans ='00000000-0000-0000-0000-000000000000')
		begin		
			select distinct nv.ID, nv.MaNhanVien, nv.TenNhanVien,  ISNULL(pb.TenPhongBan,'') as TenNhanVien_GC, '' as TenNhanVien_CV, 
				ISNULL(ct.ID_PhongBan,'00000000-0000-0000-0000-000000000000') as ID_PhongBan
			from NS_NhanVien nv
			left join NS_QuaTrinhCongTac ct on nv.ID= ct.ID_NhanVien
			left join NS_PhongBan pb on ct.ID_PhongBan=  pb.ID 
			where ct.ID_DonVi like  @ID_DonVi and (pb.ID_DonVi like @ID_DonVi OR pb.ID_DonVi is null)
			and (nv.DaNghiViec is null or nv.DaNghiViec='0') and (nv.TrangThai is null OR nv.TrangThai ='0')
			order by TenNhanVien_GC desc
		end
	else
		begin
			select distinct nv.ID, nv.MaNhanVien, nv.TenNhanVien, ISNULL(pb.TenPhongBan,'') as TenNhanVien_GC, '' as TenNhanVien_CV,
				ISNULL(ct.ID_PhongBan,'00000000-0000-0000-0000-000000000000') as ID_PhongBan
			from NS_NhanVien nv
			left join NS_QuaTrinhCongTac ct on nv.ID= ct.ID_NhanVien 
			left join NS_PhongBan pb on ct.ID_PhongBan=  pb.ID
			-- nvien co the chua thuoc phong nao
			where ct.ID_DonVi like  @ID_DonVi and (pb.ID_DonVi like @ID_DonVi OR pb.ID_DonVi is null)
			and ct.ID_PhongBan in (Select * from splitstring(@ID_PhongBans))
			and (nv.DaNghiViec is null or nv.DaNghiViec='0') and (nv.TrangThai is null OR nv.TrangThai ='0')
			order by TenNhanVien_GC desc
		end");

            CreateStoredProcedure(name: "[dbo].[SP_GetThucDonWait]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"select 
	hd.MaHoaDon ,
	hd.ID_ViTri,
	vt.TenViTri as TenPhongBan,
	ct.ID,
    ct.SoLuong,
    ct.ID_DonViQuiDoi,    
    ct.Bep_SoLuongYeuCau,
    ct.Bep_SoLuongHoanThanh,
    ct.Bep_SoLuongChoCungUng,
    ct.ThoiGian ,    
    hh.TenHangHoa ,
    ct.ID_HoaDon,
    ct.GhiChu ,
    ct.TienThue,
	qd.ID_HangHoa ,
    qd.MaHangHoa

from BH_HoaDon hd
join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
left join DM_ViTri vt on hd.ID_ViTri= vt.ID
join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
left join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
where ISNULL(ct.Bep_SoLuongChoCungUng,0) > 0 and hd.ChoThanhToan = '1'
and hd.ID_DonVi like @ID_DonVi");

            CreateStoredProcedure(name: "[dbo].[SP_GetThucDonYeuCau]", parametersAction: p => new
            {
                ID_DonVi = p.String()
            }, body: @"select 
	hd.MaHoaDon ,
	hd.ID_ViTri,
	vt.TenViTri as TenPhongBan,
	ct.ID,
    ct.SoLuong,
    ct.ID_DonViQuiDoi,    
    ct.Bep_SoLuongYeuCau,
    ct.Bep_SoLuongHoanThanh,
    ct.Bep_SoLuongChoCungUng,
    ct.ThoiGian ,    
    hh.TenHangHoa ,
    ct.ID_HoaDon,
    ct.GhiChu ,
    ct.TienThue,
	qd.ID_HangHoa ,
    qd.MaHangHoa

from BH_HoaDon hd
join BH_HoaDon_ChiTiet ct on hd.ID= ct.ID_HoaDon
left join DM_ViTri vt on hd.ID_ViTri= vt.ID
join DonViQuiDoi qd on ct.ID_DonViQuiDoi= qd.ID
left join DM_HangHoa hh on hh.ID= qd.ID_HangHoa
where ISNULL(ct.Bep_SoLuongYeuCau,0) > 0 and hd.ChoThanhToan = '1'
and hd.ID_DonVi like @ID_DonVi");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetMaHDDatHang_Max]
AS
BEGIN
    DECLARE @intReturn float;
    
    		SELECT @intReturn = COUNT(MaHoaDon)
    	FROM BH_HoaDon WHERE CHARINDEX('DHO',MaHoaDon) = 0 and CHARINDEX('DH',MaHoaDon) > 0
    
    		IF @intReturn = 0 SELECT 0
    		ELSE
    			BEGIN
    			SELECT  MAX(CAST (dbo.udf_GetNumeric(MaHoaDon) AS float)) MaxCode
    			FROM BH_HoaDon WHERE CHARINDEX('DHO',MaHoaDon) = 0 and CHARINDEX('DH',MaHoaDon) > 0
    			END	
END

--SP_GetMaHDDatHang_Max");

            Sql(@"ALTER PROCEDURE [dbo].[SP_GetQuyHoaDon_byDoiTuong]
    @ID_DoiTuong [nvarchar](max),
    @ID_DonVi [nvarchar](max)
AS
BEGIN
    select ct.ID_HoaDonLienQuan,TrangThai,
    	case when hd.LoaiHoaDon = 6 then sum(ISNULL(ct.TienThu,0)) else 
    	case when max(qhd.LoaiHoaDon) = 11 then sum(ct.TienThu) else sum(ISNULL(-ct.TienThu,0)) end end TongTienThu
    	from Quy_HoaDon_ChiTiet ct
    	join Quy_HoaDon qhd on ct.ID_HoaDon = qhd.ID
    	left join BH_HoaDon hd on ct.ID_HoaDonLienQuan = hd.ID
    	where ct.ID_DoiTuong like @ID_DoiTuong and qhd.ID_DonVi like @ID_DonVi
    	and (TrangThai is  null or TrangThai = '1' ) 
    	group by ct.ID_HoaDonLienQuan, hd.LoaiHoaDon,TrangThai
END");

        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[BaoCaoTaiChinh_ChiNhanh]");
            DropStoredProcedure("[dbo].[SP_GetHoaDonChoThanhToan]");
            DropStoredProcedure("[dbo].[SP_GetListNhanVien_inDepartment]");
            DropStoredProcedure("[dbo].[SP_GetThucDonWait]");
            DropStoredProcedure("[dbo].[SP_GetThucDonYeuCau]");
        }
    }
}
