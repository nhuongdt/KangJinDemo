namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_Quy_HoaDon : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure(name: "[dbo].[deleteHoaDonDieuChinh]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid()
            }, body: @"Update BH_HoaDon set YeuCau = N'Hủy bỏ', ChoThanhToan = 1 where ID = @ID_HoaDon");

            CreateStoredProcedure(name: "[dbo].[getList_HangHoabyMaHH]", parametersAction: p => new
            {
                MaHangHoa = p.String()
            }, body: @"select 
	dvqd.ID,
	hh.ID as ID_HangHoa,
	dvqd.MaHangHoa,
	hh.TenHangHoa +
    	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
	hh.TenHangHoa,
	Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
	dvqd.TenDonViTinh,
	dvqd.GiaVon
	FROM 
	DonViQuiDoi dvqd 
	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join 
    		(
    			Select Main.id_hanghoa,
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
    		) tt on hh.ID = tt.id_hanghoa
    	where dvqd.MaHangHoa = @MaHangHoa 
		and dvqd.Xoa is null
		and hh.TheoDoi = 1");

            CreateStoredProcedure(name: "[dbo].[getList_HoaDonDieuChinh]", parametersAction: p => new
            {
                ID_ChiNhanh = p.String(),
                MaHoaDon = p.String(),
                timeStart = p.DateTime(),
                timeEnd = p.DateTime(),
                TrangThai1 = p.String(),
                TrangThai2 = p.String(),
                TrangThai3 = p.String()
            }, body: @"Select 
	hd.ID as ID_HoaDon,
	hd.ID_NhanVien,
	Max(hd.MaHoaDon) as MaHoaDon,
	hd.ChoThanhToan,
	Max(hd.NgayLapHoaDon) as NgayLapHoaDon,
	COUNT(*) as SoLuongHangHoa,
	SUM(hdct.PTChietKhau) as TongGiaVonTang,
	SUM (hdct.TienChietKhau) as TongGiaVonGiam,
	Max(dv.TenDonVi) as TenDonVi,
	Max (hd.DienGiai) as GhiChu,
	MAX (hd.NguoiTao) as NguoiTao,
	MAx(nv.TenNhanVien) as NguoiDieuChinh,
	Case when hd.YeuCau = N'Tạm lưu' then N'Phiếu tạm' else
	Case when hd.YeuCau = N'Hoàn thành' then N'Đã điều chỉnh' else N'Đã hủy' end end as TrangThai
	FROM BH_HoaDon hd
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	inner join DM_DonVi dv on hd.ID_DonVi = dv.ID
	left join NS_NhanVien nv on hd.ID_NhanVien = nv.ID
	where hd.LoaiHoaDon = 18
	and hd.ID_DonVi in (select * from splitstring(@ID_ChiNhanh))
	and (hd.YeuCau like @TrangThai1 or hd.YeuCau like @TrangThai2 or hd.YeuCau like @TrangThai3)
	and hd.MaHoaDon like @MaHoaDon
	and hd.NgayLapHoaDon >= @timeStart and hd.NgayLapHoaDon < @timeEnd
	GROUP BY hd.ID, hd.YeuCau, hd.ChoThanhToan, hd.ID_NhanVien
	ORDER BY MAX(hd.NgayLapHoaDon) DESC");

            CreateStoredProcedure(name: "[dbo].[getList_HoaDonDieuChinh_ChiTiet]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid()
            }, body: @"Select 
	dvqd.ID as ID_DonViQuiDoi,
		dvqd.MaHangHoa,
    	hh.TenHangHoa +
    	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
    	hh.TenHangHoa,
    	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
    		Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenDonViTinh,
    	hdct.DonGia as GiaVonHienTai,
		hdct.GiaVon as GiaVonMoi,
		hdct.PTChietKhau as GiaVonTang,
		hdct.TienChietKhau as GiaVonGiam
	From BH_HoaDon hd
	inner join BH_HoaDon_ChiTiet hdct on hd.ID = hdct.ID_HoaDon
	inner join DonViQuiDoi dvqd on hdct.ID_DonViQuiDoi = dvqd.ID
	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join 
			(
    			Select Main.id_hanghoa,
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
    		) tt on hh.ID = tt.id_hanghoa
	where hd.ID = @ID_HoaDon
	ORDER BY hdct.SoThuTu");

            CreateStoredProcedure(name: "[dbo].[getListHangHoaBy_IDNhomHang]", parametersAction: p => new
            {
                ID_NhomHang = p.String()
            }, body: @"select 
	dvqd.ID as ID_DonViQuiDoi,
	dvqd.MaHangHoa,
	hh.TenHangHoa +
    	Case when (tt.ThuocTinh_GiaTri is null) then '' else '_' + tt.ThuocTinh_GiaTri end +
    	Case when dvqd.TenDonVitinh = '' or dvqd.TenDonViTinh is null then '' else ' (' + dvqd.TenDonViTinh + ')' end as TenHangHoaFull,
	hh.TenHangHoa,
	Case when tt.ThuocTinh_GiaTri is null then '' else '_' + tt.ThuocTinh_GiaTri end as ThuocTinh_GiaTri,
	dvqd.TenDonViTinh,
	dvqd.GiaVon as GiaVonHienTai,
	dvqd.GiaVon as GiaVonMoi,
	0 as GiaVonTang,
	0 as GiaVonGiam
	FROM 
	DonViQuiDoi dvqd 
	inner join DM_HangHoa hh on dvqd.ID_HangHoa = hh.ID
	left join 
    		(
    			Select Main.id_hanghoa,
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
    		) tt on hh.ID = tt.id_hanghoa
    	where hh.ID_NhomHang in (select * from splitstring(@ID_NhomHang))
		and dvqd.Xoa is null
		and hh.TheoDoi = 1");

            CreateStoredProcedure(name: "[dbo].[insert_DieuChinhGiaVon]", parametersAction: p => new
            {
                ID = p.Guid(),
                ID_DonVi = p.Guid(),
                ID_NhanVien = p.Guid(),
                MaHoaDon = p.String(),
                TongGiaVonHienTai = p.Double(),
                TongGiaVonMoi = p.Double(),
                TongGiaVonTang = p.Double(),
                TongGiaVonGiam = p.Double(),
                ChoThanhToan = p.Boolean(),
                DienGiai = p.String(),
                NguoiTao = p.String(),
                loaiInsert = p.Int(),
                YeuCau = p.String()
            }, body: @"if (@loaiInsert = 1)
		BEGIN
			Insert into BH_HoaDon (ID, MaHoaDon, LoaiHoaDon, ChoThanhToan, ID_DonVi, ID_NhanVien, NgayLapHoaDon, TongTienHang, TongChietKhau, TongTienThue, TongChiPhi,TongGiamGia,PhaiThanhToan, DienGiai, YeuCau, NguoiTao, NgayTao)
			Values(@ID, @MaHoaDon, '18', @ChoThanhToan, @ID_DonVi, @ID_NhanVien, GETDATE(), @TongGiaVonHienTai, @TongGiaVonMoi, @TongGiaVonTang, @TongGiaVonGiam,'0','0', @DienGiai, @YeuCau, @NguoiTao, GETDATE())
		END
	else
		BEGin
			update BH_HoaDon set MaHoaDon = @MaHoaDon, ChoThanhToan = @ChoThanhToan, ID_NhanVien = @ID_NhanVien, TongTienHang = @TongGiaVonHienTai, TongChietKhau = @TongGiaVonMoi,
			TongTienThue = @TongGiaVonTang, TongChiPhi = @TongGiaVonGiam, DienGiai = @DienGiai, YeuCau = @YeuCau, NgayLapHoaDon = GETDATE(), NguoiTao = @NguoiTao, NguoiSua = @NguoiTao, NgaySua = GETDATE()
			where ID = @ID
		END");

            CreateStoredProcedure(name: "[dbo].[insert_DieuChinhGiaVon_ChiTiet]", parametersAction: p => new
            {
                ID_HoaDon = p.Guid(),
                ID_DonViQuiDoi = p.Guid(),
                SoThuTu = p.Int(),
                GiaVonHienTai = p.Double(),
                GiaVonMoi = p.Double(),
                GiaVonTang = p.Double(),
                GiaVonGiam = p.Double(),
                loaiInsert = p.Int(),
                ChoThanhToan = p.Boolean()
            }, body: @"IF (@loaiInsert = 1)
		BEGIN
			Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi)
			Values(NEWID(), @ID_HoaDon, @SoThuTu,'0',@GiaVonHienTai, @GiaVonMoi, @GiaVonTang, @GiaVonGiam, '0','0','0','0','0','0', @ID_DonViQuiDoi)
			if (@ChoThanhToan = 0)
			BEGIN
				update DonViQuiDoi set GiaVon = @GiaVonMoi where ID = @ID_DonViQuiDoi
			END
		END
	ElSE
		BEGIN
			delete BH_HoaDon_ChiTiet where ID_HoaDon = @ID_HoaDon
			Insert into BH_HoaDon_ChiTiet (ID, ID_HoaDon, SoThuTu, SoLuong, DonGia, GiaVon,PTChietKhau, TienChietKhau,TienThue, PTChiPhi, TienChiPhi, ThanhToan, ThanhTien,An_Hien, ID_DonViQuiDoi)
			Values(NEWID(), @ID_HoaDon, @SoThuTu,'0',@GiaVonHienTai, @GiaVonMoi, @GiaVonTang, @GiaVonGiam, '0','0','0','0','0','0', @ID_DonViQuiDoi)
			if (@ChoThanhToan = 0)
			BEGIN
				update DonViQuiDoi set GiaVon = @GiaVonMoi where ID = @ID_DonViQuiDoi
			END
		END");
        }
        
        public override void Down()
        {
            DropStoredProcedure("[dbo].[deleteHoaDonDieuChinh]");
            DropStoredProcedure("[dbo].[getList_HangHoabyMaHH]");
            DropStoredProcedure("[dbo].[getList_HoaDonDieuChinh]");
            DropStoredProcedure("[dbo].[getList_HoaDonDieuChinh_ChiTiet]");
            DropStoredProcedure("[dbo].[getListHangHoaBy_IDNhomHang]");
            DropStoredProcedure("[dbo].[insert_DieuChinhGiaVon]");
            DropStoredProcedure("[dbo].[insert_DieuChinhGiaVon_ChiTiet]");
        }
    }
}
