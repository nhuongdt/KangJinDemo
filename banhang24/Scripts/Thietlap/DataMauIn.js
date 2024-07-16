ko.observableArray.fn.group = function (prop) {
    var target = this;
    target.index = {};
    target.index[prop] = ko.observable({});
    ko.computed(function () {
        //rebuild index
        var propIndex = {};

        ko.utils.arrayForEach(target(), function (item) {
            var key = ko.utils.unwrapObservable(item[prop]);
            if (key) {
                propIndex[key] = propIndex[key] || [];
                propIndex[key].push(item);
            }
        });

        target.index[prop](propIndex);
    });
    return target;
};

var dataMauIn = function () {
    //===============================
    // Declare parameter
    //===============================
    var self = this;
    var obj = {
        SoThuTu: 1,
        LaHangHoa: true,
        MaHangHoa: "sp000001",
        TenHangHoa: "Sản phẩm 1",
        TenHangHoaThayThe: "Sản phẩm 1 (A1)",
        GiaVonHienTai: '100,000',
        GiaVonMoi: '90,000',
        ChenhLech: '10,000',
        DonViTinh: "Cái",
        DonGia: "10,000",
        GiaBan: "9,000",
        DonGiaBaoHiem: "10,000",
        SoLuong: "2",
        ThanhTienTruocCK: "20,000",
        ThanhTien: "18,000",
        ThanhToan: "28,000",
        BH_ThanhTien: "28,000",
        SoLuongChuyen: '2',
        SoLuongNhan: '2',
        GiaChuyen: '10,000',
        SoLuongHuy: '2',
        GiaVon: '10,000',
        GiaTriHuy: '10,000',

        TonLuyKe: '2',
        TonKho: '10',
        KThucTe: '1',
        SLLech: '-9',
        GiaTriLech: '-54,000',

        PTChietKhau: '10',
        TienChietKhau: '1,000',
        GhiChu_NVThucHien: 'Minh Anh, Quỳnh Anh',
        GhiChu_NVTuVan: 'Lê Nam, E Quân',
        GhiChuHH: 'Hàng dễ vỡ',
        NVTuVanDV_CoCK: 'Lê Nam(10 %), E Quân (20,000)',
        NVThucHienDV_CoCK: 'Quỳnh Anh(5 %), Quỳnh Anh (10%)',
        PTThue: '10',
        TienThue: '10,000',
        HH_ThueTong: '20,000',
        PTChiPhi: '10',
        TienChiPhi: '10,000',
        TongChietKhau: '2,000',

        GhiChu: 'abc',
        ThuocTinh_GiaTri: 'size X',
        SoLuongDVDaSuDung: 1,
        SoLuongDVConLai: 2,
        TenViTri: 'Phòng 1',
        TimeStart: '18:12',
        ThoiGianThucHien: '20 phút',
        QuaThoiGian: '10 phút',
        ID_NhomHangHoa: '1',
        TenNhomHangHoa: 'Nhóm 1',
        SoThuTuNhom: 1,
        SoThuTuNhom_LaMa: 'I',
        ThanhPhanComBo: [],
        MaLoHang: 'Lô 01',
    };
    var obj2 = {
        SoThuTu: 2,
        LaHangHoa: false,
        MaHangHoa: "DV000002",
        TenHangHoa: "Dịch vụ 2",
        TenHangHoaThayThe: "Kiểm tra sản phẩm",
        GiaVonHienTai: '100,000',
        GiaVonMoi: '90,000',
        ChenhLech: '10,000',
        DonViTinh: "Lần",
        DonGia: "10,000",
        GiaBan: "8,000",
        DonGiaBaoHiem: "10,000",
        SoLuong: "1",
        ThanhTienTruocCK: "10,000",
        ThanhTien: "8,000",
        ThanhToan: "8,000",
        BH_ThanhTien: "18,000",
        SoLuongChuyen: '1',
        SoLuongNhan: '1',
        GiaChuyen: '10,000',
        SoLuongHuy: '1',
        GiaVon: '10,000',
        GiaTriHuy: '10,000',

        TonLuyKe: '3',
        TonKho: '10',
        KThucTe: '2',
        SLLech: '-8',
        GiaTriLech: '-48,000',

        PTChiPhi: '10',
        TienChiPhi: '10,000',
        PTThue: '10',
        TienThue: '10,000',
        HH_ThueTong: '10,000',
        PTChietKhau: '5',
        TienChietKhau: '2,000',
        GhiChu_NVThucHien: 'Nguyễn A, Lê Hà',
        GhiChu_NVTuVan: 'Đỗ Hà, Cao Trang',
        GhiChuHH: 'Hàng dễ vỡ',
        NVTuVanDV_CoCK: 'Nguyễn A(10 %), Lê Hà (20,000)',
        NVThucHienDV_CoCK: 'Cao B(5 %), Lê Hà (10%)',
        TongChietKhau: '2,000',

        GhiChu: 'abc',
        ThuocTinh_GiaTri: 'size M',
        TenViTri: 'Phòng 2',
        TimeStart: '18:12',
        SoLuongDVDaSuDung: 1,
        SoLuongDVConLai: 2,
        ThoiGianThucHien: '20 phút',
        QuaThoiGian: '10 phút',
        ID_NhomHangHoa: '1',
        TenNhomHangHoa: 'Nhóm 1',
        SoThuTuNhom: 1,
        SoThuTuNhom_LaMa: 'I',
        ThanhPhanComBo: [
            obj, obj,
        ],
        MaLoHang: 'Lô 02',
    };

    var obj3 = {
        SoThuTu: 2,
        LaHangHoa: false,
        MaHangHoa: "SP0003",
        TenHangHoa: "Sản phẩm 2",
        TenHangHoaThayThe: "Ốc vít",
        GiaVonHienTai: '100,000',
        GiaVonMoi: '90,000',
        ChenhLech: '10,000',
        DonViTinh: "Lần",
        DonGia: "10,000",
        GiaBan: "8,000",
        DonGiaBaoHiem: "10,000",
        SoLuong: "1",
        ThanhTienTruocCK: "10,000",
        ThanhTien: "8,000",
        ThanhToan: "8,000",
        BH_ThanhTien: "18,000",
        SoLuongChuyen: '1',
        SoLuongNhan: '1',
        GiaChuyen: '10,000',
        SoLuongHuy: '1',
        GiaVon: '10,000',
        GiaTriHuy: '10,000',

        TonLuyKe: '4',
        TonKho: '10',
        KThucTe: '2',
        SLLech: '-8',
        GiaTriLech: '-48,000',

        PTChiPhi: '10',
        TienChiPhi: '10,000',
        PTThue: '10',
        TienThue: '10,000',
        HH_ThueTong: '10,000',
        PTChietKhau: '5',
        TienChietKhau: '2,000',
        GhiChu_NVThucHien: 'Nguyễn A, Lê Hà',
        GhiChu_NVTuVan: 'Đỗ Hà, Cao Trang',
        GhiChuHH: 'Hàng dễ vỡ',
        NVTuVanDV_CoCK: 'Nguyễn A(10 %), Lê Hà (20,000)',
        NVThucHienDV_CoCK: 'Cao B(5 %), Lê Hà (10%)',
        TongChietKhau: '2,000',

        GhiChu: 'abc',
        ThuocTinh_GiaTri: 'size M',
        TenViTri: 'Phòng 2',
        TimeStart: '18:12',
        SoLuongDVDaSuDung: 1,
        SoLuongDVConLai: 2,
        ThoiGianThucHien: '20 phút',
        QuaThoiGian: '10 phút',
        ID_NhomHangHoa: '3',
        TenNhomHangHoa: 'Nhóm 3',
        SoThuTuNhom: 1,
        SoThuTuNhom_LaMa: 'I',
        ThanhPhanComBo: [
            obj, obj,
        ],
        MaLoHang: 'Lô 02',
    };
    var obj4 = {
        SoThuTu: 2,
        LaHangHoa: false,
        MaHangHoa: "DV0004",
        TenHangHoa: "Dịch vụ 4",
        TenHangHoaThayThe: "Thay lọc nhớt",
        GiaVonHienTai: '100,000',
        GiaVonMoi: '90,000',
        ChenhLech: '10,000',
        DonViTinh: "Lần",
        DonGia: "10,000",
        GiaBan: "8,000",
        DonGiaBaoHiem: "10,000",
        SoLuong: "1",
        ThanhTienTruocCK: "10,000",
        ThanhTien: "8,000",
        ThanhToan: "8,000",
        BH_ThanhTien: "18,000",
        SoLuongChuyen: '1',
        SoLuongNhan: '1',
        GiaChuyen: '10,000',
        SoLuongHuy: '1',
        GiaVon: '10,000',
        GiaTriHuy: '10,000',

        TonLuyKe: '5',
        TonKho: '10',
        KThucTe: '2',
        SLLech: '-8',
        GiaTriLech: '-48,000',

        PTChiPhi: '10',
        TienChiPhi: '10,000',
        PTThue: '10',
        TienThue: '10,000',
        HH_ThueTong: '10,000',
        PTChietKhau: '5',
        TienChietKhau: '2,000',
        GhiChu_NVThucHien: 'Nguyễn A, Lê Hà',
        GhiChu_NVTuVan: 'Đỗ Hà, Cao Trang',
        GhiChuHH: 'Hàng dễ vỡ',
        NVTuVanDV_CoCK: 'Nguyễn A(10 %), Lê Hà (20,000)',
        NVThucHienDV_CoCK: 'Cao B(5 %), Lê Hà (10%)',
        TongChietKhau: '2,000',

        GhiChu: 'abc',
        ThuocTinh_GiaTri: 'size M',
        TenViTri: 'Phòng 2',
        TimeStart: '18:12',
        SoLuongDVDaSuDung: 1,
        SoLuongDVConLai: 2,
        ThoiGianThucHien: '20 phút',
        QuaThoiGian: '10 phút',
        ID_NhomHangHoa: '4',
        TenNhomHangHoa: 'Nhóm 4',
        SoThuTuNhom: 1,
        SoThuTuNhom_LaMa: 'I',
        ThanhPhanComBo: [
            obj, obj,
        ],
        MaLoHang: 'Lô 04',
    };
    self.CTHoaDonPrint = ko.observableArray([
        obj,
        obj2,
        obj,
        obj2,
    ]);
    self.CTHoaDonPrintMH = ko.observableArray([
        obj,
        obj,
        obj2,
        obj2,
    ]);
    self.TenPhongBan = ko.observable('Phòng 1');
    self.TongTienHoaDonMua = ko.observable('40,000');
    self.PhaiTraKhach = ko.observable('40,000');
    self.PhaiThanhToan = ko.observable('40,000');
    self.GiaChuyen = ko.observable('10,000');
    self.MaHangHoa = ko.observable('sp000001');
    self.SoLuongHuy = ko.observable(4);
    self.GiaVon = ko.observable('10,000');
    self.GiaTriHuy = ko.observable('10,000');
    self.TenCuaHang = ko.observable('Cửa hàng số 1');
    self.TenChiNhanh = ko.observable('Chi nhánh trung tâm')
    self.DienThoaiChiNhanh = ko.observable('0985226332');
    self.DiaChiChiNhanh = ko.observable('Số 12, Phạm Văn Đồng, Cầu Giấy, HN')
    self.LogoCuaHang = ko.observable(logoImage);
    self.NgayLapHoaDon = ko.observable('05/05/2014 09:40');
    self.NgayApDungGoiDV = ko.observable('01/01/2021');
    self.HanSuDungGoiDV = ko.observable('01/01/2023');
    self.MaHoaDon = ko.observable('DH000021');
    self.MaDoiTuong = ko.observable('KH00001');
    self.NgaySinh_NgayTLap = ko.observable('10/10/1990');
    self.TenDoiTuong = ko.observable('Anh Hòa Q.1');
    self.TongTichDiem = ko.observable('123');
    self.DiaChiKhachHang = ko.observable('Số 10, Phổ Quang,Tân Bình, TPHCM');
    self.DienThoaiKhachHang = ko.observable('01635552623');
    self.NhanVienBanHang = ko.observable('Nguyễn Hạnh Phúc');
    self.NguoiTaoHD = ko.observable('Phan Bá Nam');
    self.GhiChu = ko.observable('Ghi chú');
    self.GhiChuNgayThuoc = ko.observable('10 ngày thuốc');
    self.TongTienHang = ko.observable('40,000');
    self.TongTienHDSauGiamGia = ko.observable('30,000');
    self.ThoiGianGiao = ko.observable('05/07/2018 09:40');
    self.DaThanhToan = ko.observable('40,000');
    self.TongGiamGia = ko.observable('10,000');
    self.DiaChiCuaHang = ko.observable('Quận 7, TPHCM');
    self.DienThoaiCuaHang = ko.observable('024345678');
    self.TongChiPhi = ko.observable('0');
    self.TongTienTraHang = ko.observable('40,000');
    self.TongTienTra = ko.observable('15,000');
    self.MaHoaDonTraHang = ko.observable('BG0000014');

    self.TongCong = ko.observable('0');
    self.TongSoLuongHang = ko.observable('40,000');
    self.ChiPhiNhap = ko.observable('40,000');
    self.NoSau = ko.observable('100,000');
    self.NoTruoc = ko.observable('100,000');
    self.BH_NoSau = ko.observable('5,000,000');
    self.BH_NoTruoc = ko.observable('3,000,000');
    self.TienKhachThieu = ko.observable('10,000');
    self.ChiNhanhChuyen = ko.observable('Chi nhánh trung tâm');
    self.GhiChuChiNhanhChuyen = ko.observable();
    self.NguoiChuyen = ko.observable('Người vận chuyển 1');
    self.ChiNhanhNhan = ko.observable('Nhân viên 1');
    self.NguoiNhan = ko.observable('Nguyễn Hạnh Phúc');
    self.TongSoLuongChuyen = ko.observable('4');
    self.SoLuongNhan = ko.observable('4');
    self.TongSoLuongNhan = ko.observable('4');
    self.TongTienChuyen = ko.observable('40,000');
    self.MaSoThue = ko.observable('1800898225');
    self.TaiKhoanNganHang = ko.observable('5852200000');
    self.PTThue = ko.observable('10');
    self.ChiPhi = ko.observable('410,000');
    self.ChiPhi_GhiChu = ko.observable('Chi phí vận chuyển');
    self.IsChuyenPhatNhanh = ko.observable(false);

    self.TongTienNhan = ko.observable('40,000');
    self.MaPhieu = ko.observable('TH000326');

    self.NguoiNopTien = ko.observable('Khách 1');
    self.GiaTriPhieu = ko.observable('40,000');
    self.TienBangChu = ko.observable('Bốn mươi nghìn đồng');
    self.KH_TienBangChu = ko.observable('Bốn mươi nghìn đồng');
    self.NoiDungThu = ko.observable('Nội dung 1');
    self.ChiNhanhBanHang = ko.observable('Chi nhánh trung tâm');
    self.HoaDonLienQuan = ko.observable('HD0001, HD0002');

    self.NguoiCanBang = ko.observable('Bùi Tiến Dũng');
    self.TrangThaiKK = ko.observable(' Đã cân bằng kho');
    self.NgayTao = ko.observable('13/06/2018 09:53');
    self.NgayCanBang = ko.observable('13/06/2018 09:53');
    self.TongThucTe = ko.observable('36,000');
    self.TongLechTang = ko.observable('0');
    self.TongLechGiam = ko.observable('-144,000');
    self.TongChenhLech = ko.observable('-144,000');

    self.TenSanPham = ko.observable('Bánh kem');
    self.MaVach = ko.observable('/Content/icon/ma-vach.JPG');
    self.Gia = ko.observable('144,000 đ');

    self.TienThua = ko.observable('0');
    self.TienMat = ko.observable('5,000,000');
    self.TienGui = ko.observable('5,000,000'); // POS
    self.TienATM = ko.observable('2,000,000'); // ChuyenKhoan
    self.TTBangTienCoc = ko.observable('800,000');
    self.SoDuDatCoc = ko.observable('1,000,000');
    self.TongGiamGiaHang = ko.observable('40,000');
    self.TongTienHangChuaCK = ko.observable('60,000');
    self.TongTienHDSauVAT = ko.observable('50,000');
    self.TongTienThue = ko.observable('20,000');
    self.TenNganHangPOS = ko.observable('Ngân hàng Công Thương Việt Nam (VietinBank)');
    self.TenChuThePOS = ko.observable('Nguyễn B');
    self.SoTaiKhoanPOS = ko.observable('0132454556666');
    self.TenNganHangChuyenKhoan = ko.observable('Ngân hàng Đầu tư và Phát triển Việt Nam (BIDV)');
    self.TenChuTheChuyenKhoan = ko.observable('Đỗ Hoa');
    self.SoTaiKhoanChuyenKhoan = ko.observable('1903555665655');
    self.TongTaiKhoanThe = ko.observable('10,000,000');
    self.TongSuDungThe = ko.observable('3,000,000');
    self.SoDuConLai = ko.observable('7,000,000');
    self.TongChietKhau = ko.observable('5');// % giam gia HD
    self.TongGiamGiaHD_HH = ko.observable('10,000');// % giam gia HD
    self.TenNhomKhach = 'Nhóm VIP';
    self.ChietKhauNVHoaDon_InGtriCK = ko.observable('Lê Huyền (10%), Cao Hải (20%)');
    self.ChietKhauNVHoaDon = ko.observable('Lê Huyền, Cao Hải');
    self.PhuongThucTT = ko.observable('Tiền mặt');

    // ValueCard
    self.TongTien = ko.observable('2,000,000');
    self.TienDoiDiem = ko.observable('20,000');
    self.TienTheGiaTri = ko.observable('2,000,000');
    self.Ngay = ko.observable('08');
    self.Thang = ko.observable('07');
    self.Nam = ko.observable('2019');

    //gara
    self.HangMucSuaChua = ko.observableArray([
        {
            STT: 1, TenHangMuc: 'Lốp xe', TinhTrang: 'Thủng lốp', PhuongAnSuaChua: 'Thay lốp mới',
        },
        {
            STT: 2, TenHangMuc: 'Gương chiếu hậu', TinhTrang: 'Mờ', PhuongAnSuaChua: 'Làm sạch',
        },
    ]);

    self.VatDungKemTheo = ko.observableArray([
        {
            STT: 1, TieuDe: 'Ghế nhựa', SoLuong: 1,
        },
        {
            STT: 2, TieuDe: 'Tượng phật nhỏ', SoLuong: 2,
        },
    ]);

    self.MaPhieuTiepNhan = ko.observable('PTN0002');
    self.NgayVaoXuong = ko.observable('01/12/2020');
    self.NgayXuatXuong = ko.observable('08/12/2020');
    self.NgayXuatXuongDuKien = ko.observable('07/12/2020');

    self.CoVanDichVu = ko.observable('Nguyễn Lê Anh');
    self.CoVan_SDT = ko.observable('0986 456 888');
    self.NhanVienTiepNhan = ko.observable('Nguyễn Mỹ Linh');

    self.BienSo = ko.observable('30A-145.16');
    self.TenMauXe = ko.observable('Model 1');
    self.TenLoaiXe = ko.observable('Suzuki');
    self.TenHangXe = ko.observable('Honda');
    self.HopSo = ko.observable('0005');
    self.DungTich = ko.observable('200 lit');
    self.MauSon = ko.observable('Trắng');
    self.NamSanXuat = ko.observable('2020');
    self.SoKhung = ko.observable('001');
    self.SoMay = ko.observable('002');
    self.SoKmVao = ko.observable('200');
    self.SoKmRa = ko.observable('200');
    self.TongTienDichVu = ko.observable('500,000');
    self.TongTienPhuTung = ko.observable('1,000,000');
    self.TongTienPhuTung_TruocVAT = ko.observable('1,500,000');
    self.TongTienPhuTung_TruocCK = ko.observable('1,400,000');
    self.TongTienDichVu_TruocVAT = ko.observable('1,000,000');
    self.TongTienDichVu_TruocCK = ko.observable('900,000');
    self.PhaiThanhToanBaoHiem = ko.observable('400,000');
    self.TongThanhToan = ko.observable('1,500,000');
    self.PTN_GhiChu = ko.observable('bảo dưỡng');
    self.TongThue_PhuTung = ko.observable('40,000');
    self.TongCK_PhuTung = ko.observable('4,000');
    self.TongThue_DichVu = ko.observable('20,000');
    self.TongCK_DichVu = ko.observable('4,000');
    self.TongSL_PhuTung = ko.observable('4');
    self.TongSL_DichVu = ko.observable('2');

    self.ChuXe = ko.observable('Lê Văn Quyết');
    self.ChuXe_SDT = ko.observable('0946 123 895');
    self.ChuXe_DiaChi = ko.observable('Số nhà 12, đường Láng, HN');
    self.ChuXe_Email = ko.observable('ngant@gmail.com');

    self.LH_Ten = ko.observable('Anh Thắng');
    self.LH_SDT = ko.observable('0985 624 321');

    self.TenBaoHiem = ko.observable('Bảo hiểm nhân thọ');
    self.BH_SDT = ko.observable('024 125 2255');
    self.BH_Email = ko.observable('baohiemnhantho@gmail.com');
    self.BH_DiaChi = ko.observable('Tầng 2, Tòa nhà TimeTower');
    self.BH_TenLienHe = ko.observable('Chị Nga');
    self.BH_SDTLienHe = ko.observable('0962 122 456');
    self.BaoHiemDaTra = ko.observable('300,000');
    self.BH_TienBangChu = ko.observable('Ba trăm nghìn đồng');

    self.TongTienBHDuyet = ko.observable('300,000');
    self.PTThueHoaDon = ko.observable('10');
    self.PTThueBaoHiem = ko.observable('10');
    self.SoVuBaoHiem = ko.observable('2');
    self.KhauTruTheoVu = ko.observable('200,000');
    self.PTGiamTruBoiThuong = ko.observable('5');
    self.GiamTruBoiThuong = ko.observable('300,000');
    self.BHThanhToanTruocThue = ko.observable('500,000');
    self.TongTienThueBaoHiem = ko.observable('200,000');
    self.TongThueKhachHang = ko.observable('00,000');

    self.NgayLapPhieu = ko.observable('08/07/2019');
    self.TenBangLuong = ko.observable('Bảng lương tháng 7');
    self.KyTinhLuong = ko.observable('01/07/2019 - 31/07/2019');
    self.MaBangLuongChiTiet = ko.observable('PL0001');
    self.MaNhanVien = ko.observable('NV0003');
    self.TenNhanVien = ko.observable('Nguyễn Thị Hoa');
    self.NgayCongChuan = ko.observable('24');
    self.NgayCongThuc = ko.observable('26');
    self.LuongCoBan = ko.observable('5,000,000');
    self.LuongChinh = ko.observable('5,200,000');
    self.LuongOT = ko.observable('300,000');
    self.PhuCapCoBan = ko.observable('200,000');
    self.PhuCapKhac = ko.observable('60,000');
    self.ChietKhau = ko.observable('500,000');
    self.TongGiamTru = ko.observable('50,000');
    self.LuongSauGiamTru = ko.observable('5,910,000');
    self.TruTamUngLuong = ko.observable('2,000,000');
    self.ThucLinh = ko.observable('3,910,000');
    self.ThanhToan = ko.observable('3,910,000');
    self.NguoiLapPhieu = ko.observable('Lê Thị Xuân');
    self.KhoanMucThuChi = ko.observable('Thu tiền khách nợ');

    self.CTHoaDonPrint_TheoNhom = ko.observableArray([
        {
            ID_NhomHangHoa: 1, TenNhomHangHoa: 'Nhóm 1', SoThuTuNhom: 1, SoThuTuNhom_LaMa: 'I',
            TongTienTheoNhom: '4,000,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 4,
            TongThueTheoNhom: '40,000',
            TongCKTheoNhom: '4,000',
            HangHoas: [obj, obj]
        },

        {
            ID_NhomHangHoa: 2, TenNhomHangHoa: 'Nhóm 2', SoThuTuNhom: 2, SoThuTuNhom_LaMa: 'II',
            TongTienTheoNhom: '1,500,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 2,
            TongThueTheoNhom: '20,000',
            TongCKTheoNhom: '4,000',
            HangHoas: [obj2, obj2]
        },
    ])

    self.CTHoaDonPrint_DichVu = ko.observableArray([
        {
            ID_NhomHangHoa: 1, TenNhomHangHoa: 'Nhóm 2', SoThuTuNhom: 1, SoThuTuNhom_LaMa: 'II',
            TongTienTheoNhom: '4,000,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 4,
            TongThueTheoNhom: '40,000',
            TongCKTheoNhom: '4,000',
            HangHoas: [obj2]
        },

        {
            ID_NhomHangHoa: 2, TenNhomHangHoa: 'Nhóm 4', SoThuTuNhom: 4, SoThuTuNhom_LaMa: 'IV',
            TongTienTheoNhom: '1,500,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 2,
            TongThueTheoNhom: '20,000',
            TongCKTheoNhom: '4,000',
            HangHoas: [obj4]
        },
    ])
    self.CTHoaDonPrint_VatTu = ko.observableArray([
        {
            ID_NhomHangHoa: 1, TenNhomHangHoa: 'Nhóm 1', SoThuTuNhom: 1, SoThuTuNhom_LaMa: 'I',
            TongTienTheoNhom: '4,000,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 4,
            TongThueTheoNhom: '40,000',
            TongCKTheoNhom: '4,000',
            HangHoas: [obj]
        },

        {
            ID_NhomHangHoa: 2, TenNhomHangHoa: 'Nhóm 3', SoThuTuNhom: 3, SoThuTuNhom_LaMa: 'III',
            TongTienTheoNhom: '1,500,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 2,
            TongThueTheoNhom: '20,000',
            TongCKTheoNhom: '4,000',
            HangHoas: [obj3]
        },
    ])

    self.CTHoaDon_TheoNhom_VaHHDV =  ko.observableArray([
        {
            ID_NhomHangHoa: 1, TenNhomHangHoa: 'Nhóm 1', SoThuTuNhom: 1, SoThuTuNhom_LaMa: 'I',
            TongTienTheoNhom: '4,000,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 4,
            TongThueTheoNhom: '40,000',
            TongCKTheoNhom: '4,000',
            ListHangHoas: {
                HangHoas: [obj, obj2],
                NhomHH_SoLuong: 14,
                NhomHH_TongThue: '44,00',
                NhomHH_ChietKhau: '4,000',
                NhomHH_TruocVAT: '41,00',
                NhomHH_TruocCK: '42,00',
                NhomHH_ThanhToan: '43,00',
            },
            ListDichVus: {
                DichVus: [obj3, obj4],
                NhomDV_SoLuong: 15,
                NhomDV_TongThue: '55,00',
                NhomDV_ChietKhau: '5,000',
                NhomDV_TruocVAT: '51,00',
                NhomDV_TruocCK: '52,00',
                NhomDV_ThanhToan: '53,00',
            },
        },

        {
            ID_NhomHangHoa: 2, TenNhomHangHoa: 'Nhóm 2', SoThuTuNhom: 2, SoThuTuNhom_LaMa: 'II',
            TongTienTheoNhom: '1,500,000',
            TongTienTheoNhom_TruocVAT: '1,500,000',
            TongSLTheoNhom: 2,
            TongThueTheoNhom: '20,000',
            TongCKTheoNhom: '4,000',
            ListHangHoas: {
                HangHoas: [obj, obj3],
                NhomHH_SoLuong: 12,
                NhomHH_TongThue: '20,00',
                NhomHH_ChietKhau: '2,000',
                NhomHH_TruocVAT: '21,00',
                NhomHH_TruocCK: '22,00',
                NhomHH_ThanhToan: '23,000',
            },
            ListDichVus: {
                DichVus: [obj2, obj4],
                NhomDV_SoLuong: 13,
                NhomDV_TongThue: '30,00',
                NhomDV_ChietKhau: '3,000',
                NhomDV_TruocVAT: '31,00',
                NhomDV_TruocCK: '32,00',
                NhomDV_ThanhToan: '33,00',
            },
        },
    ])
};
ko.applyBindings(new dataMauIn());









