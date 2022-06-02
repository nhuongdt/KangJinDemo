namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSP_20181030 : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[getList_DanhSachNhanVien]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                MaNhanVien_TV = p.String(),
                TrangThai = p.String()
            }, body: @"Select * from 
	(
	select MaNhanVien, TenNhanVien, NgaySinh, 
	Case when GioiTinh = 1 then N'Nam' else N'Nữ' end as GioiTinh, NguyenQuan, DienThoaiDiDong, Email, ThuongTru, SoCMND, SoBHXH, GhiChu, NgayTao,
	Case when DaNghiViec = 0 then N'Đang làm việc' else N'Đã nghỉ việc' end as DaNghiViec,
	Case when TrangThai is null then 1 else TrangThai end as TrangThai from NS_NhanVien
	where (MaNhanVien like @MaNhanVien or MaNhanVien like @MaNhanVien_TV 
	or TenNhanVienChuCaiDau like @MaNhanVien or TenNhanVienKhongDau like @MaNhanVien)
	and DaNghiViec like @TrangThai
	) a
	where a.TrangThai != 0
	order by a.NgayTao DESC");

            CreateStoredProcedure(name: "[dbo].[GetListChiTietHoaDonXuatFile]", parametersAction: p => new
            {
                IDHoaDon = p.Guid()
            }, body: @"SELECT 
    		dvqd.MaHangHoa,
			TenHangHoa +
    		Case when (ThuocTinh.ThuocTinh_GiaTri is null) then '' else '_' + ThuocTinh.ThuocTinh_GiaTri end +
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end +
    		Case when DM_LoHang.MaLoHang is null then '' else '. Lô: ' + DM_LoHang.MaLoHang end as TenHangHoa,SoLuong,ROUND(DonGia, 0) as DonGia,TienChietKhau as GiamGia,ThanhTien
    		FROM BH_HoaDon hd
    		JOIN BH_HoaDon_ChiTiet cthd ON hd.ID= cthd.ID_HoaDon
    		JOIN DonViQuiDoi dvqd ON cthd.ID_DonViQuiDoi = dvqd.ID
    		JOIN DM_HangHoa hh ON dvqd.ID_HangHoa= hh.ID
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
    					) as ThuocTinh on hh.ID = ThuocTinh.id_hanghoa
    		LEFT JOIN DM_LoHang ON cthd.ID_LoHang = DM_LoHang.ID
    		WHERE cthd.ID_HoaDon = @IDHoaDon
    		order by SoThuTu desc");

            CreateStoredProcedure(name: "[dbo].[importNS_NhanVien_DanhSach]", parametersAction: p => new
            {
                MaNhanVien = p.String(),
                TenNhanVien = p.String(),
                TenNhanVienKhongDau = p.String(),
                TenNhanVienKyTuDau = p.String(),
                GioiTinh = p.Boolean(),
                NgaySinh = p.String(),
                DienThoai = p.String(),
                Email = p.String(),
                NoiSinh = p.String(),
                CMND = p.String(),
                SoBaoHiem = p.String(),
                GhiChu = p.String(),
                TrangThai = p.Boolean()
            }, body: @"insert into NS_NhanVien(ID, MaNhanVien, TenNhanVien, TenNhanVienKhongDau,TenNhanVienChuCaiDau, GioiTinh, NgaySinh, DienThoaiDiDong, Email, NoiSinh,NguyenQuan, SoCMND,SoBHXH, GhiChu, NguoiTao,NgayTao, DaNghiViec)
    values(NEWID(), @MaNhanVien, @TenNhanVien, @TenNhanVienKhongDau,@TenNhanVienKyTuDau, @GioiTinh, @NgaySinh, @DienThoai, @Email, @NoiSinh, @NoiSinh, @CMND,@SoBaoHiem, @GhiChu, 'admin',GETDATE(), @TrangThai);");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[getList_DanhSachNhanVien]");
            DropStoredProcedure("[dbo].[GetListChiTietHoaDonXuatFile]");
            DropStoredProcedure("[dbo].[importNS_NhanVien_DanhSach]");
        }
    }
}