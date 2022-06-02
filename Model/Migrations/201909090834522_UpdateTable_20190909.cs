namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTable_20190909 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HT_MaChungTu", "ID_DonVi", "dbo.DM_DonVi");
            DropIndex("dbo.HT_MaChungTu", new[] { "ID_DonVi" });
            CreateTable(
                "dbo.NS_NgayNghiLe",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_KyTinhCong = c.Guid(),
                    Thu = c.Int(nullable: false),
                    Ngay = c.DateTime(),
                    LoaiNgay = c.Int(nullable: false),
                    CongQuyDoi = c.Double(nullable: false),
                    HeSoLuong = c.Double(nullable: false),
                    HeSoLuongOT = c.Double(nullable: false),
                    MoTa = c.String(maxLength: 4000),
                    TrangThai = c.Int(nullable: false),
                    NguoiTao = c.String(maxLength: 4000),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(maxLength: 4000),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_KyTinhCong", t => t.ID_KyTinhCong)
                .Index(t => t.ID_KyTinhCong);

            AddColumn("dbo.HT_CauHinhPhanMem", "SuDungMaChungTu", c => c.Int());
            AddColumn("dbo.HT_MaChungTu", "ID_LoaiChungTu", c => c.Int(nullable: false));
            AddColumn("dbo.HT_MaChungTu", "KiTuNganCach1", c => c.String(maxLength: 50));
            AddColumn("dbo.HT_MaChungTu", "MaLoaiChungTu", c => c.String(maxLength: 50));
            AddColumn("dbo.HT_MaChungTu", "KiTuNganCach2", c => c.String(maxLength: 50));
            AddColumn("dbo.HT_MaChungTu", "KiTuNganCach3", c => c.String(maxLength: 50));
            AlterColumn("dbo.HT_MaChungTu", "SuDungMaDonVi", c => c.Boolean(nullable: false));
            CreateIndex("dbo.HT_MaChungTu", "ID_LoaiChungTu");
            AddForeignKey("dbo.HT_MaChungTu", "ID_LoaiChungTu", "dbo.DM_LoaiChungTu", "ID", cascadeDelete: true);
            DropColumn("dbo.HT_MaChungTu", "ID_DonVi");
            DropColumn("dbo.HT_MaChungTu", "LoaiChungTu");
            DropColumn("dbo.HT_MaChungTu", "TienTo");
            DropColumn("dbo.HT_MaChungTu", "NganCach1");
            DropColumn("dbo.HT_MaChungTu", "NganCach2");
            DropColumn("dbo.HT_MaChungTu", "SuDungUserName");

            CreateTable(
                "dbo.NS_CongBoSung",
                c => new
                {
                    ID = c.Guid(nullable: false),
                    ID_ChamCongChiTiet = c.Guid(nullable: false),
                    ID_MayChamCong = c.Guid(),
                    NgayCham = c.DateTime(nullable: false),
                    GioVao = c.DateTime(),
                    GioRa = c.DateTime(),
                    GioVaoOT = c.DateTime(),
                    GioRaOT = c.DateTime(),
                    TrangThai = c.Int(nullable: false),
                    GhiChu = c.String(),
                    KyHieuCong = c.String(maxLength: 4000),
                    Cong = c.Double(nullable: false),
                    SoPhutDiMuon = c.Int(nullable: false),
                    SoGioOT = c.Double(nullable: false),
                    NguoiTao = c.String(maxLength: 4000),
                    NgayTao = c.DateTime(nullable: false),
                    NguoiSua = c.String(maxLength: 4000),
                    NgaySua = c.DateTime(),
                })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.NS_MayChamCong", t => t.ID_MayChamCong)
                .ForeignKey("dbo.NS_ChamCong_ChiTiet", t => t.ID_ChamCongChiTiet, cascadeDelete: true)
                .Index(t => t.ID_ChamCongChiTiet)
                .Index(t => t.ID_MayChamCong);

            AddColumn("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong", c => c.Guid());
            AddColumn("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec", c => c.Guid(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "LoaiCong", c => c.Int(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong", c => c.Guid(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "GhiChu", c => c.String());
            AddColumn("dbo.NS_ChamCong_ChiTiet", "TrangThai", c => c.Int(nullable: false));
            AddColumn("dbo.NS_ChamCong_ChiTiet", "PhutDiMuon", c => c.Double(nullable: false));
            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong");
            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec");
            CreateIndex("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong");
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong", "dbo.NS_MaChamCong", "ID");
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong", "dbo.NS_KyTinhCong", "ID", cascadeDelete: true);
            AddForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec", "ID", cascadeDelete: true);

            CreateStoredProcedure(name: "[dbo].[GetMaHoaDonMax_byTemp]", parametersAction: p => new
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
			-- lay ma max hien tai
			declare @maxCodeNow varchar(30) = (
			select top 1 Mahoadon from BH_HoaDon 
			where MaHoaDon like @sMaFull +'%' and ChoThanhToan='0'
			order by MaHoaDon desc)
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

			select @Return = MAX(CAST(dbo.udf_GetNumeric(RIGHT(MaHoaDon,LEN(MaHoaDon)- @lenMaChungTu))AS float))
			from BH_HoaDon where SUBSTRING(MaHoaDon, 1, len(@machungtu)) = @machungtu and CHARINDEX('O',MaHoaDon) = 0 -- not HDO, GDVO, THO, DHO
			
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

            CreateStoredProcedure(name: "[dbo].[GetMaPhieuThuChiMax_byTemp]", parametersAction: p => new
            {
                LoaiHoaDon = p.Int(),
                ID_DonVi = p.Guid(),
                NgayLapHoaDon = p.DateTime()
            }, body: @"SET NOCOUNT ON;
	DECLARE @Return float = 1
	declare @lenMaMax int = 0
	DECLARE @isDefault bit = (select SuDungMaChungTu from HT_CauHinhPhanMem where ID_DonVi= @ID_DonVi)-- co/khong thiet lap su dung Ma MacDinh
	DECLARE @isSetup int = (select ID_LoaiChungTu from HT_MaChungTu where ID_LoaiChungTu = @LoaiHoaDon)

	if @isSetup='1' and @isSetup is not null
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
			
			-- lay ma max hien tai
			declare @maxCodeNow varchar(30) = (
			select top 1 Mahoadon from Quy_HoaDon 
			where MaHoaDon like @sMaFull +'%' 
			order by MaHoaDon desc)
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

			select @Return = MAX(CAST(dbo.udf_GetNumeric(RIGHT(MaHoaDon,LEN(MaHoaDon)- @lenMaChungTu))AS float))
			from Quy_HoaDon where SUBSTRING(MaHoaDon, 1, len(@machungtu)) = @machungtu and CHARINDEX('_', MaHoaDon)=0
	
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
            Sql(@"ALTER PROCEDURE [dbo].[insert_DonViQuiDoi]
    @ID [uniqueidentifier],
    @ID_HangHoa [uniqueidentifier],
    @ID_DonVi [uniqueidentifier],
    @ID_LoHang [uniqueidentifier],
    @MaHangHoa [nvarchar](max),
    @GiaVon [float],
    @GiaBan [float],
    @timeCreate [datetime],
	@ID_NhanVien [uniqueidentifier]
AS
BEGIN
    insert into DonViQuiDoi (ID, MaHangHoa, ID_HangHoa, TyLeChuyenDoi, LaDonViChuan, GiaVon, GiaNhap, GiaBan, NguoiTao, NgayTao, Xoa, ThuocTinhGiaTri)
    	Values (@ID, @MaHangHoa,@ID_HangHoa, '1', '1', '0', '0', @GiaBan, 'admin', @timeCreate, '0','')
		exec insert_DM_GiaVon @ID ,@ID_DonVi,@ID_LoHang, @GiaVon, @ID_NhanVien;
END");

        }
        
        public override void Down()
        {
            AddColumn("dbo.HT_MaChungTu", "SuDungUserName", c => c.Boolean());
            AddColumn("dbo.HT_MaChungTu", "NganCach2", c => c.String(maxLength: 50));
            AddColumn("dbo.HT_MaChungTu", "NganCach1", c => c.String(maxLength: 50));
            AddColumn("dbo.HT_MaChungTu", "TienTo", c => c.String(maxLength: 50));
            AddColumn("dbo.HT_MaChungTu", "LoaiChungTu", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.HT_MaChungTu", "ID_DonVi", c => c.Guid(nullable: false));
            DropForeignKey("dbo.NS_NgayNghiLe", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.HT_MaChungTu", "ID_LoaiChungTu", "dbo.DM_LoaiChungTu");
            DropIndex("dbo.NS_NgayNghiLe", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.HT_MaChungTu", new[] { "ID_LoaiChungTu" });
            AlterColumn("dbo.HT_MaChungTu", "SuDungMaDonVi", c => c.Boolean());
            DropColumn("dbo.HT_MaChungTu", "KiTuNganCach3");
            DropColumn("dbo.HT_MaChungTu", "KiTuNganCach2");
            DropColumn("dbo.HT_MaChungTu", "MaLoaiChungTu");
            DropColumn("dbo.HT_MaChungTu", "KiTuNganCach1");
            DropColumn("dbo.HT_MaChungTu", "ID_LoaiChungTu");
            DropColumn("dbo.HT_CauHinhPhanMem", "SuDungMaChungTu");
            DropTable("dbo.NS_NgayNghiLe");
            CreateIndex("dbo.HT_MaChungTu", "ID_DonVi");
            AddForeignKey("dbo.HT_MaChungTu", "ID_DonVi", "dbo.DM_DonVi", "ID");
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec", "dbo.NS_CaLamViec");
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong", "dbo.NS_KyTinhCong");
            DropForeignKey("dbo.NS_CongBoSung", "ID_ChamCongChiTiet", "dbo.NS_ChamCong_ChiTiet");
            DropForeignKey("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong", "dbo.NS_MaChamCong");
            DropForeignKey("dbo.NS_CongBoSung", "ID_MayChamCong", "dbo.NS_MayChamCong");
            DropIndex("dbo.NS_CongBoSung", new[] { "ID_MayChamCong" });
            DropIndex("dbo.NS_CongBoSung", new[] { "ID_ChamCongChiTiet" });
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_KyTinhCong" });
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_CaLamViec" });
            DropIndex("dbo.NS_ChamCong_ChiTiet", new[] { "ID_MaChamCong" });
            DropColumn("dbo.NS_ChamCong_ChiTiet", "PhutDiMuon");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "TrangThai");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "GhiChu");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "ID_KyTinhCong");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "LoaiCong");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "ID_CaLamViec");
            DropColumn("dbo.NS_ChamCong_ChiTiet", "ID_MaChamCong");
            DropTable("dbo.NS_CongBoSung");
            DropStoredProcedure("[dbo].[GetMaHoaDonMax_byTemp]");
            DropStoredProcedure("[dbo].[GetMaPhieuThuChiMax_byTemp]");
        }
    }
}
