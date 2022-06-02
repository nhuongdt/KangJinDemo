namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUpdateSP_20200903 : DbMigration
    {
        public override void Up()
        {
			Sql(@"ALTER PROCEDURE [dbo].[GetListCashFlow]
    @IDDonVis [nvarchar](max),
    @ID_NhanVien [nvarchar](40),
	@ID_NhanVienLogin uniqueidentifier,
    @ID_TaiKhoanNganHang [nvarchar](40),
    @ID_KhoanThuChi [nvarchar](40),
    @DateFrom [datetime],
    @DateTo [datetime],
    @LoaiSoQuy varchar(15),
    @LoaiChungTu varchar(2),
    @TrangThaiSoQuy varchar(2),
    @TrangThaiHachToan varchar(2),
    @TxtSearch [nvarchar](max)
AS
BEGIN
    SET NOCOUNT ON;

	declare @nguoitao nvarchar(100) = (select taiKhoan from HT_NguoiDung where ID_NhanVien= @ID_NhanVienLogin)
	declare @tblNhanVien table (ID uniqueidentifier)
	insert into @tblNhanVien
	select * from dbo.GetIDNhanVien_inPhongBan(@ID_NhanVienLogin, @IDDonVis,'SoQuy_XemDS_PhongBan','SoQuy_XemDS_HeThong');
	
    select tblView.*, dv.TenDonVi as TenChiNhanh
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
			TienMat, TienGui, TienMat + TienGui as TienThu,
			TienMat + TienGui as TongTienThu,
			TenTaiKhoanPOS, TenTaiKhoanNOTPOS,
			cast(ID_TaiKhoanNganHang as varchar(max)) as ID_TaiKhoanNganHang,
			ID_KhoanThuChi,
			NoiDungThuChi,
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
	 ) tblView
	 join DM_DonVi dv on tblView.ID_DonVi = dv.ID
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
		order by tblView.NgayLapHoaDon desc
	
END");
        }
        
        public override void Down()
        {
        }
    }
}
