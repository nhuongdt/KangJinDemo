var ModelGroupProduct = function () {
    var self = this;
    self.TenNhomHangHoa = ko.observable();
    self.ID_Parent = ko.observable();
    self.ID = ko.observable();
    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenNhomHangHoa(item.TenNhomHangHoa);
        self.ID_Parent(item.ID_Parent);
    }
};

var ModelProduct = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaHangHoa = ko.observable();
    self.TenHangHoa = ko.observable();
    self.ID_NhomHang = ko.observable();
    self.GiaBan = ko.observable(0);

    self.GiamGia = ko.observable();
    self.SoLuong = ko.observable();
    self.ThanhTien = ko.observable();
    self.TenDonViTinh = ko.observable();
    self.QuanLyTheoLoHang = ko.observable(false);
    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaHangHoa(item.MaHangHoa);
        self.TenHangHoa(item.TenHangHoa);
        self.ID_NhomHang(item.ID_NhomHang);
        self.GiaBan(item.GiaBan);
        self.GiamGia(item.GiamGia);
        self.SoLuong(item.SoLuong);
        self.ThanhTien(item.ThanhTien);
        self.TenDonViTinh(item.TenDonViTinh);
        self.QuanLyTheoLoHang(item.QuanLyTheoLoHang);
    }
}

var FormModel_NewHoaDon = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_HoaDon = ko.observable();
    self.IDRandom = ko.observable();
    self.MaHoaDon = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ID_DoiTuong = ko.observable();
    self.TongTienHang = ko.observable();
    self.TongGiamGia = ko.observable(0); // tien
    self.TongChietKhau = ko.observable(0); // % gg
    self.ChoThanhToan = ko.observable(true);

    self.PhaiThanhToan = ko.observable(0);
    self.NgayLapHoaDon = ko.observable(null);
    self.ID_NhanVien = ko.observable(null);
    self.NguoiTao = ko.observable(VHeader.UserLogin);
    self.DienGiai = ko.observable();
    self.LoaiHoaDon = ko.observable($('#txtLoaiHoaDon').val());
    if (commonStatisJs.CheckNull($('#txtLoaiHoaDon').val())) {
        self.LoaiHoaDon(4);
    }
    self.TenDoiTuong = ko.observable();
    self.DaThanhToan = ko.observable(0);
    self.KhachDaTra = ko.observable(0);// tien da tra truoc khi tamluu
    self.ConThieu = ko.observable(0);
    self.TienMat = ko.observable(0);
    self.TienChuyenKhoan = ko.observable(0);
    self.HoanTra = ko.observable(0);
    self.TongTienHangChuaCK = ko.observable(0);
    self.PTChietKhauHH = ko.observable(0);
    self.TongGiamGiaHang = ko.observable(0);
    self.TongTienThue = ko.observable(0);
    self.PTThueHoaDon = ko.observable(0);
    self.TongThanhToan = ko.observable(0);
    self.TongChiPhi = ko.observable(0);
    self.SoDuDatCoc = ko.observable(0);
    self.TienPOS = ko.observable(0);
    self.TienDatCoc = ko.observable(0);
    self.ID_TaiKhoanPos = ko.observable();
    self.TenTaiKhoanPos = ko.observable();
    self.ID_TaiKhoanChuyenKhoan = ko.observable();
    self.TenTaiKhoanCK = ko.observable();
    self.ID_KhoanThuChi = ko.observable();
    self.isCheckTraLaiCoc = ko.observable(false);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_HoaDon(item.ID_HoaDon);
        self.IDRandom(item.IDRandom);
        self.ID_DonVi(item.ID_DonVi);
        self.ID_DoiTuong(item.ID_DoiTuong);
        self.ID_NhanVien(item.ID_NhanVien);
        self.TenDoiTuong(item.TenDoiTuong);
        self.MaHoaDon(item.MaHoaDon);
        self.TongTienHang(item.TongTienHang);
        self.TongGiamGia(item.TongGiamGia);
        self.TongChietKhau(item.TongChietKhau);
        self.PhaiThanhToan(item.PhaiThanhToan);
        self.DaThanhToan(item.DaThanhToan);
        self.KhachDaTra(item.KhachDaTra);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.NguoiTao(item.NguoiTao);
        self.DienGiai(item.DienGiai);
        self.ChoThanhToan(item.ChoThanhToan);
        self.ConThieu(item.ConThieu);
        self.TienMat(item.TienMat);
        self.TienChuyenKhoan(item.TienChuyenKhoan);
        self.HoanTra(item.HoanTra);
        self.TongTienHangChuaCK(item.TongTienHangChuaCK);
        self.PTChietKhauHH(item.PTChietKhauHH);
        self.TongGiamGiaHang(item.TongGiamGiaHang);
        self.TongTienThue(item.TongTienThue);
        self.PTThueHoaDon(item.PTThueHoaDon);
        self.TongThanhToan(item.TongThanhToan);
        self.TongChiPhi(item.TongChiPhi);
        self.SoDuDatCoc(item.SoDuDatCoc);
        self.ID_KhoanThuChi(item.ID_KhoanThuChi);
        self.LoaiHoaDon(item.LoaiHoaDon);

        if (commonStatisJs.CheckNull(item.TienPOS)) {
            self.TienPOS(0);
        }
        else {
            self.TienPOS(item.TienPOS);
        }
        if (commonStatisJs.CheckNull(item.TienDatCoc)) {
            self.TienDatCoc(0);
        }
        else {
            self.TienDatCoc(item.TienDatCoc);
        }

        if (commonStatisJs.CheckNull(item.ID_TaiKhoanPos)) {
            self.ID_TaiKhoanPos(null);
        }
        else {
            self.ID_TaiKhoanPos(item.ID_TaiKhoanPos);
        }
        if (commonStatisJs.CheckNull(item.TenTaiKhoanPos)) {
            self.TenTaiKhoanPos('');
        }
        else {
            self.TenTaiKhoanPos(item.TenTaiKhoanPos);
        }
        if (commonStatisJs.CheckNull(item.ID_TaiKhoanChuyenKhoan)) {
            self.ID_TaiKhoanChuyenKhoan(null);
        }
        else {
            self.ID_TaiKhoanChuyenKhoan(item.ID_TaiKhoanChuyenKhoan);
        }
        if (commonStatisJs.CheckNull(item.TenTaiKhoanCK)) {
            self.TenTaiKhoanCK('');
        }
        else {
            self.TenTaiKhoanCK(item.TenTaiKhoanCK);
        }
        if (commonStatisJs.CheckNull(item.isCheckTraLaiCoc)) {
            self.isCheckTraLaiCoc(false);
        }
        else {
            self.isCheckTraLaiCoc(item.isCheckTraLaiCoc);
        }
    }
}

var NhapHangChiTiet = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDoiTuongUri = "/api/DanhMuc/DM_DoiTuongAPI/";
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    self.LoaiHoaDonMenu = ko.observable(parseInt($('#txtLoaiHoaDon').val()));

    var lcCTNhapHang = 'lcCTNhapHang';
    var lcHDNhapHang = 'lcHDNhapHang';
    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();

    switch (self.LoaiHoaDonMenu()) {
        case 13:
            lcCTNhapHang = 'ctNhapNoiBo';
            lcHDNhapHang = 'hdNhapNoiBo';
            break;
        case 14:
            lcCTNhapHang = 'ctNhapHangThua';
            lcHDNhapHang = 'hdNhapHangThua';
            break;
        case 31:
            lcCTNhapHang = 'ctDatNCC';
            lcHDNhapHang = 'hdDatNCC';
            break;
    }

    var _idDonVi = VHeader.IdDonVi;
    var _tenDonVi = VHeader.TenDonVi;
    var _userLogin = VHeader.UserLogin;
    var _idNhanVien = VHeader.IdNhanVien;
    var _IDNguoiDung = VHeader.IdNguoiDung;
    var shopCookies = VHeader.IdNganhNgheKinhDoanh;
    self.ID_DonVi = ko.observable(_idDonVi);

    vmThemMoiNCC.inforLogin = {
        ID_NhanVien: VHeader.IdNhanVien,
        ID_User: VHeader.IdNguoiDung,
        UserLogin: VHeader.UserLogin,
        ID_DonVi: VHeader.IdDonVi,
        TenNhanVien: VHeader.TenNhanVien,
    };
    vmThemMoiNhomNCC.inforLogin = vmThemMoiNCC.inforLogin;

    var serverTime = sstime.GetDatetime();
    self.DateHDDefault = ko.observable(moment(serverTime).format('DD/MM/YYYY HH:mm'));

    self.isLoadding = ko.observable(false);
    self.newNhomDoiTuong = ko.observable(new PartialVendorGroup());
    self.newDoiTuong = ko.observable(new FormModel_NewVendor());
    self.newHoaDon = ko.observable(new FormModel_NewHoaDon());
    self.newNhomHangHoa = ko.observable(new ModelGroupProduct());
    self.newHangHoa = ko.observable(new ModelProduct());

    self.NhanViens = ko.observableArray();
    self.DoiTuong = ko.observableArray();
    self.NhomDoiTuongs = ko.observableArray();
    self.HangHoaAfterAdd = ko.observableArray();
    self.ChiTietDoiTuong = ko.observableArray();
    self.HangHoas = ko.observableArray();
    self.ListLot_ofProduct = ko.observableArray();
    self.ListLot_ofProductAll = ko.observableArray();
    self.ID_DonViQuiDoiBG = ko.observable();
    self.BangGiaBans = ko.observableArray();
    self.BangGiaBanOfAll = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    self.CongTy = ko.observableArray();
    self.ChiNhanh = ko.observableArray();
    self.DoiTuong_Old = ko.observableArray();
    self.TaiKhoansPOS = ko.observableArray();
    self.TaiKhoansCK = ko.observableArray();
    self.IsNhapNhanh = ko.observable(true);
    self.ItemChosing = ko.observable();
    self.loiExcel = ko.observableArray();
    self.numbersPrintHD = ko.observable(1);
    self.ConTonKho = ko.observable(0);// 1.an hang het tonkho
    self.CTHoaDonPrint = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    self.turnOnPrint = ko.observable(true);

    self.filterNHH = ko.observable();
    self.selectedHH = ko.observable();
    self.filterNH = ko.observable();
    self.selectIDNHCK = ko.observable();
    self.fileNameExcel = ko.observable();

    self.soLuongMatHang = ko.observable(0);
    self.TongSoLuongHH = ko.observable(0);
    self.Quyen_NguoiDung = ko.observableArray();
    self.NhapHang_ThayDoiThoiGian = ko.observable();
    self.TraHangNhap_ThayDoiThoiGian = ko.observable();
    self.NhapHang_ThayDoiNhanVien = ko.observable();
    self.TraHangNhap_ThayDoiNhanVien = ko.observable();
    self.role_XacNhan_NhapHangKhachThua = ko.observable(true);
    self.HangHoa_GiaBan = ko.observable();
    self.HangHoa_GiaNhap = ko.observable();
    self.HangHoa_ThemMoi = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable(true);
    self.HoaDon_ThemMoi = ko.observable();
    modelTypeSearchProduct.TypeSearch(1);// jqAutoProduct

    self.isGara = ko.observable(false);
    self.shopCookies = ko.observable(shopCookies.toLowerCase());

    self.role_NhapKhoNoiBo = ko.observable(VHeader.Quyen.indexOf('NhapKhoNoiBo') > -1);
    self.role_NhapHangKhachThua = ko.observable(VHeader.Quyen.indexOf('NhapHangKhachThua') > -1);
    self.role_InsertSoQuy = ko.observable(VHeader.Quyen.indexOf('SoQuy_ThemMoi') > -1);

    self.ListTypePurchase = ko.observableArray([
        { ID: 4, Text: 'Nhập hàng nhà cung cấp' },
        //{ ID: 13, Text: 'Nhập kho nội bộ' },
        //{ ID: 14, Text: 'Nhập hàng khách thừa' },
    ])

    if (self.role_NhapKhoNoiBo()) {
        self.ListTypePurchase.push({ ID: 13, Text: 'Nhập kho nội bộ' })
    }
    if (self.role_NhapHangKhachThua()) {
        self.ListTypePurchase.push({ ID: 14, Text: 'Nhập hàng khách thừa' })
    }

    if (self.shopCookies() === 'c16edda0-f6d0-43e1-a469-844fab143014') {
        self.isGara(true);
    }

    function PageLoad() {
        UpdateProperties_ifUndefined();
        GetListNhanVien();
        GetHT_Quyen_ByNguoiDung();
        Check_QuyenXemGiaVon();
        GetListNhomDT();
        GetListTinhThanh();
        GetTree_NhomHangHoa();
        getAllTaiKhoanNganHang();
        GetInforCongTy();
        GetInforChiNhanh();
        Check_OnOffPrint();
        GetAllQuy_KhoanThuChi();
    }
    console.log(1)

    PageLoad();

    function UpdateProperties_ifUndefined() {
        let idRandom = CreateIDRandom('HD_');
        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (commonStatisJs.CheckNull(hd[i].IDRandom)) {
                    hd[i].IDRandom = idRandom;
                }
                if (hd[i].HoanTra === undefined) {
                    hd[i].HoanTra = 0;
                }
                if (hd[i].TongGiamGiaHang === undefined) {
                    hd[i].TongGiamGiaHang = tongGiamGiaHang;
                }
                if (hd[i].TongTienHangChuaCK === undefined) {
                    hd[i].TongTienHangChuaCK = tongtienchuaCK;
                }
                if (hd[i].TongThanhToan === undefined) {
                    hd[i].TongThanhToan = hd[i].PhaiThanhToan;
                }
                if (hd[i].TongTienThue === undefined) {
                    hd[i].TongTienThue = 0;
                }
                if (commonStatisJs.CheckNull(hd[i].ID_KhoanThuChi)) {
                    hd[i].ID_KhoanThuChi = null;
                }
                if (commonStatisJs.CheckNull(hd[i].PTThueHoaDon)) {
                    if (!commonStatisJs.CheckNull(hd[i].PTThue)) {
                        hd[i].PTThueHoaDon = hd[i].PTThue;
                    }
                    else {
                        hd[i].PTThueHoaDon = 0;
                    }
                }
                if (hd[i].PTChietKhauHH === undefined) {
                    hd[i].PTChietKhauHH = 0;
                }
                if (hd[i].SoDuDatCoc === undefined) {
                    hd[i].SoDuDatCoc = 0;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }

        // caculator cthd
        var tongtienchuaCK = 0, tongGiamGiaHang = 0;
        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                let itFor = cthd[i];
                if (commonStatisJs.CheckNull(itFor.IDRandomHD)) {
                    cthd[i].IDRandomHD = idRandom;// because old code: has 1 hd & cthd old always override: so only assign 1 IDRandomHD for same
                }
                if (cthd[i].TienThue === undefined) {
                    cthd[i].TienThue = 0;
                }
                if (cthd[i].PTThue === undefined) {
                    cthd[i].PTThue = 0;
                }
                if (cthd[i].ThanhToan === undefined) {
                    cthd[i].ThanhToan = cthd[i].ThanhTien;
                }
                if (commonStatisJs.CheckNull(cthd[i].SoLuongConLai)) {
                    cthd[i].SoLuongConLai = 0;
                }
                if (commonStatisJs.CheckNull(cthd[i].ID_ChiTietGoiDV)) {
                    cthd[i].ID_ChiTietGoiDV = null;
                }
                tongtienchuaCK += formatNumberToFloat(itFor.SoLuong) * formatNumberToFloat(itFor.DonGia);
                tongGiamGiaHang += formatNumberToFloat(itFor.SoLuong) * formatNumberToFloat(itFor.TienChietKhau);

                for (let j = 1; j < cthd[i].DM_LoHang.length; j++) {
                    tongtienchuaCK += itFor.DM_LoHang[j].DonGia * itFor.DM_LoHang[j].SoLuong;
                    tongGiamGiaHang += itFor.DM_LoHang[j].TienChietKhau * itFor.DM_LoHang[j].SoLuong;

                    if (commonStatisJs.CheckNull(cthd[i].DM_LoHang[j].IDRandomHD)) {
                        cthd[i].DM_LoHang[j].IDRandomHD = idRandom;
                    }

                    if (cthd[i].DM_LoHang[j].ThanhToan === undefined) {
                        cthd[i].DM_LoHang[j].ThanhToan = cthd[i].DM_LoHang[j].ThanhTien;
                    }
                    if (cthd[i].DM_LoHang[j].PTThue === undefined) {
                        cthd[i].DM_LoHang[j].PTThue = 0;
                    }
                    if (cthd[i].DM_LoHang[j].TienThue === undefined) {
                        cthd[i].DM_LoHang[j].TienThue = 0;
                    }
                    if (commonStatisJs.CheckNull(cthd[i].DM_LoHang[j].SoLuongConLai)) {
                        cthd[i].DM_LoHang[j].SoLuongConLai = 0;
                    }
                    if (commonStatisJs.CheckNull(cthd[i].DM_LoHang[j].ID_ChiTietGoiDV)) {
                        cthd[i].DM_LoHang[j].ID_ChiTietGoiDV = null;
                    }
                }
                for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                    tongtienchuaCK += itFor.HangCungLoais[j].SoLuong * itFor.HangCungLoais[j].DonGia;
                    tongGiamGiaHang += itFor.HangCungLoais[j].TienChietKhau * itFor.HangCungLoais[j].SoLuong;

                    if (commonStatisJs.CheckNull(cthd[i].HangCungLoais[j].IDRandomHD)) {
                        cthd[i].HangCungLoais[j].IDRandomHD = idRandom;
                    }
                    if (cthd[i].HangCungLoais[j].ThanhToan === undefined) {
                        cthd[i].HangCungLoais[j].ThanhToan = cthd[i].HangCungLoais[j].ThanhTien;
                    }
                    if (cthd[i].HangCungLoais[j].PTThue === undefined) {
                        cthd[i].HangCungLoais[j].PTThue = 0;
                    }
                    if (cthd[i].HangCungLoais[j].TienThue === undefined) {
                        cthd[i].HangCungLoais[j].TienThue = 0;
                    }
                    if (commonStatisJs.CheckNull(cthd[i].HangCungLoais[j].SoLuongConLai)) {
                        cthd[i].HangCungLoais[j].SoLuongConLai = 0;
                    }
                    if (commonStatisJs.CheckNull(cthd[i].HangCungLoais[j].ID_ChiTietGoiDV)) {
                        cthd[i].HangCungLoais[j].ID_ChiTietGoiDV = null;
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
        }
    }
    self.ChangeCheckTonKho = function () {
        var tk = parseInt(self.ConTonKho());
        if (tk === 1) {
            self.ConTonKho(0);
        }
        else {
            self.ConTonKho(1);
        }
        localStorage.setItem('nhaphang_isTonKho', self.ConTonKho());
        modelTypeSearchProduct.ConTonKho(self.ConTonKho());
    }

    function CheckSaoChep_EditPhieuNhap() {

        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
        }
        else {
            hd = [];
        }

        let ctNhap = localStorage.getItem(lcCTNhapHang);
        if (ctNhap !== null) {
            ctNhap = JSON.parse(ctNhap);
        }
        else {
            ctNhap = [];
        }

        let cthd = localStorage.getItem('lc_CTSaoChep');
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            let ngaylapHD = cthd[0].NgayLapHoaDon;
            if (!commonStatisJs.CheckNull(ngaylapHD)) {
                ngaylapHD = moment(ngaylapHD).format('DD/MM/YYYY HH:mm');
            }
            let idHDGoc = cthd[0].ID_HoaDonGoc;
            if (commonStatisJs.CheckNull(idHDGoc)) {
                idHDGoc = null;
            }

            let idRanDomHD = CreateIDRandom('HD_');
            let objHD = {
                ID: cthd[0].ID_HoaDon,
                ID_HoaDon: idHDGoc,
                ID_DonVi: commonStatisJs.CheckNull(cthd[0].ID_DonVi) ? _idDonVi : cthd[0].ID_DonVi,
                LoaiHoaDon: commonStatisJs.CheckNull(cthd[0].LoaiHoaDon) ? 4 : cthd[0].LoaiHoaDon,
                IDRandom: idRanDomHD,
                ID_DoiTuong: cthd[0].ID_DoiTuong,
                ID_NhanVien: cthd[0].ID_NhanVien,
                MaHoaDon: cthd[0].MaHoaDon,
                NgayLapHoaDon: ngaylapHD,
                TongTienHang: cthd[0].TongTienHang,
                TongTienThue: cthd[0].TongTienThue,
                TongTienHangChuaCK: cthd[0].TongTienHangChuaCK,
                TongGiamGiaHang: cthd[0].TongGiamGiaHang,
                PTChietKhauHH: cthd[0].PTChietKhauHH,
                PTThueHoaDon: cthd[0].PTThueHD,
                TongGiamGia: cthd[0].TongGiamGia,
                TongChietKhau: cthd[0].TongChietKhau,
                TongChiPhi: formatNumberToFloat(cthd[0].TongChiPhi),
                PhaiThanhToan: cthd[0].PhaiThanhToan,
                TongThanhToan: cthd[0].PhaiThanhToan,
                KhachDaTra: cthd[0].KhachDaTra,
                DaThanhToan: cthd[0].DaThanhToan,
                HoanTra: 0,
                DienGiai: cthd[0].DienGiai,
                TenDoiTuong: cthd[0].TenDoiTuong,
                NguoiTao: _userLogin,
                SoDuDatCoc: 0,
                ConThieu: cthd[0].PhaiThanhToan - cthd[0].KhachDaTra,
                TienMat: 0,
                TienPOS: 0,
                TienChuyenKhoan: 0,
                TienDatCoc: 0,
            };

            let isSaoChep = localStorage.getItem('isSaoChep') === 'true';
            let isEdit = localStorage.getItem('isEditNH') === 'true';
            if (isSaoChep || isEdit) {

                let hdEx = $.grep(hd, function (x) {
                    return x.MaHoaDon === objHD.MaHoaDon;
                });
                if (hdEx.length > 0) {
                    idRanDomHD = hdEx[0].IDRandom;
                }
                else {
                    hd.push(objHD);
                }

                // chỉ add nếu hd not exist
                if (hdEx.length === 0) {
                    for (let i = 0; i < cthd.length; i++) {
                        cthd[i].IDRandomHD = idRanDomHD;
                        ctNhap.push(cthd[i]);
                    }
                }
            }
            else {
                let typeCacheNhapHang = localStorage.getItem('typeCacheNhapHang');
                if (typeCacheNhapHang !== null) {
                    typeCacheNhapHang = parseInt(typeCacheNhapHang);
                    switch (typeCacheNhapHang) {
                        case 3:
                            objHD.ID = const_GuidEmpty;
                            objHD.MaHoaDon = '';
                            objHD.ID_DoiTuong = null;
                            objHD.ID_NhanVien = _idNhanVien;
                            objHD.NgayLapHoaDon = null;
                            objHD.TongTienThue = 0;
                            objHD.TongTienHangChuaCK = 0;
                            objHD.TongGiamGiaHang = 0;
                            objHD.PTChietKhauHH = 0;
                            objHD.PTThueHoaDon = 0;
                            objHD.TongGiamGia = 0;
                            objHD.TongChietKhau = 0;
                            objHD.TongGiamGia = 0;
                            objHD.KhachDaTra = 0;
                            objHD.DaThanhToan = 0;
                            objHD.HoanTra = 0;
                            objHD.TenDoiTuong = '';
                            objHD.ConThieu = objHD.PhaiThanhToan;

                            hd.push(objHD);

                            // always add cthd
                            for (let i = 0; i < cthd.length; i++) {
                                cthd[i].IDRandomHD = idRanDomHD;
                                ctNhap.push(cthd[i]);
                            }
                            break;
                        case 4:// nhaphang from PO
                            objHD.ID = const_GuidEmpty;
                            objHD.ID_HoaDon = cthd[0].ID_HoaDon;
                            hd.push(objHD);

                            // always add cthd
                            for (let i = 0; i < cthd.length; i++) {
                                cthd[i].IDRandomHD = idRanDomHD;
                                ctNhap.push(cthd[i]);
                            }
                            break;
                    }
                }
            }

            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
            localStorage.setItem(lcCTNhapHang, JSON.stringify(ctNhap));

            BindHD_byIDRandom(idRanDomHD);
            BindCTHD_byIDRandomHD(idRanDomHD);
            Caculator_AmountProduct(idRanDomHD);

            if (!commonStatisJs.CheckNull(objHD.ID_DoiTuong)) {
                getChiTietNCCByID(objHD.ID_DoiTuong);
            }

            localStorage.removeItem('isSaoChep');
            localStorage.removeItem('isEditNH');
            localStorage.removeItem('lc_CTSaoChep');
            localStorage.removeItem('typeCacheNhapHang');
        }
        else {
            CheckExistCacheHD();
        }
    }

    function BindHD_byIDRandom(idRandom = null) {
        if (commonStatisJs.CheckNull(idRandom)) {
            idRandom = self.newHoaDon().IDRandom();
        }
        let hdEx = [];
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hdEx = $.grep(hd, function (x) {
                return x.IDRandom === idRandom;
            });
        }

        if (hdEx.length > 0) {
            self.newHoaDon().SetData(hdEx[0]);
        }
        else {
            ResetInforHD();
        }
    }

    function BindCTHD_byIDRandomHD(idRandom = null) {
        if (commonStatisJs.CheckNull(idRandom)) {
            idRandom = self.newHoaDon().IDRandom();
        }
        let cthdEx = [];
        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            cthdEx = $.grep(cthd, function (x) {
                return x.IDRandomHD === idRandom;
            });
        }
        self.HangHoaAfterAdd(cthdEx);
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                vmThanhToan.listData.KhoanThuChis = x.data;
            }
        })
    }

    function CheckExistCacheHD() {
        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd != null) {
            cthd = JSON.parse(cthd);
            if (cthd.length > 0) {
                dialogConfirm_OKCancel('Thông báo', 'Hệ thống tìm được 1 bản nháp chưa được lưu lên máy chủ. Bạn có muốn tiếp tục làm việc với bản nháp này?', function () {
                    var hd = localStorage.getItem(lcHDNhapHang);
                    if (hd !== null) {
                        hd = JSON.parse(hd);

                        let hdLast = hd[hd.length - 1];
                        let idRandom = hdLast.IDRandom;
                        BindHD_byIDRandom(idRandom);
                        BindCTHD_byIDRandomHD(idRandom);
                        Caculator_AmountProduct(idRandom);

                        if (!commonStatisJs.CheckNull(hdLast.ID_DoiTuong)) {
                            getChiTietNCCByID(hdLast.ID_DoiTuong, true);
                        }

                        // bind nhanvien
                        let nvien = $.grep(self.NhanViens(), function (x) {
                            return x.ID === self.newHoaDon().ID_NhanVien();
                        });
                        if (nvien.length > 0) {
                            self.textSearch(nvien[0].TenNhanVien);
                        }
                    }
                }, function () {
                    RemoveCache();
                    ResetInforHD();
                });
            }
            else {
                RemoveCache();
                ResetInforHD();
            }
        }
        else {
            RemoveCache();
            ResetInforHD();
        }
    }

    function GetListNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + _idDonVi, 'GET').done(function (data) {
            self.NhanViens(data);
            self.ListNVienSearch(data.slice(0, 20));

            CheckSaoChep_EditPhieuNhap();

            // find nvlogin
            let nvien = $.grep(self.NhanViens(), function (x) {
                return x.ID === self.newHoaDon().ID_NhanVien();
            });
            if (nvien.length > 0) {
                self.textSearch(nvien[0].TenNhanVien);
            }
        });
    }

    function GetListNhomDT() {
        ajaxHelper("/api/DanhMuc/DM_nhomDoiTuongAPI/" + "GetDM_NhomDoiTuong?loaiDoiTuong=2", 'GET').done(function (data) {
            if (data != null) {
                self.NhomDoiTuongs(data);
            }
            var newObj = {
                ID: const_GuidEmpty,
                TenNhomDoiTuong: 'Nhóm mặc định'
            }
            self.NhomDoiTuongs.unshift(newObj);
            vmThemMoiNCC.listData.NhomKhachs = self.NhomDoiTuongs();
        });
    }

    function GetListTinhThanh() {
        ajaxHelper(DMDoiTuongUri + "GetListTinhThanh", 'GET').done(function (x) {
            if (x.res === true) {
                var province = x.data.map(function (p) {
                    return {
                        ID: p.ID,
                        val2: p.TenTinhThanh
                    }
                });
                vmThemMoiNCC.listData.TinhThanhs = province;
                vmThemMoiNCC.listData.ListTinhThanhSearch = province;
            }
        });
    }

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        if (role.length > 0) {
            return true;
        }
        return false;
    }

    function GetHT_Quyen_ByNguoiDung() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
            if (data !== "" && data.length > 0) {
                self.Quyen_NguoiDung(data);
                self.HangHoa_GiaBan(CheckQuyenExist('HangHoa_GiaBan'));
                self.HangHoa_GiaNhap(CheckQuyenExist('HangHoa_GiaNhap'));
                self.HangHoa_ThemMoi(CheckQuyenExist('HangHoa_ThemMoi'));

                switch (self.LoaiHoaDonMenu()) {
                    case 4:
                        self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('NhapHang_ThayDoiThoiGian'));
                        self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('NhapHang_ThayDoiNhanVien'));
                        break;
                    case 14:
                        self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('NhapHangKhachThua_ThayDoiThoiGian'));
                        self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('NhapHangKhachThua_ThayDoiNhanVien'));
                        self.role_XacNhan_NhapHangKhachThua(CheckQuyenExist('NhapHangKhachThua_XacNhanNhapKho'));
                        break;
                    case 31:
                        self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('DatHangNCC_ThayDoiThoiGian'));
                        self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('DatHangNCC_ThayDoiNhanVien'));
                        break;
                    case 13:
                        self.NhapHang_ThayDoiThoiGian(CheckQuyenExist('NhapNoiBo_ThayDoiThoiGian'));
                        self.NhapHang_ThayDoiNhanVien(CheckQuyenExist('NhapNoiBo_ThayDoiNhanVien'));
                        break;
                }

                var insertNCC = CheckQuyenExist('NhaCungCap_ThemMoi');
                if (insertNCC) {
                    $('#hiddenShowaddNCC,#btnLuuNhomDoiTuong').show();

                }
                else {
                    $('#hiddenShowaddNCC,#btnLuuNhomDoiTuong').hide();
                }
            }
            else {
                ShowMessage_Danger('Không có quyền');
            }
        });
    }

    function Check_QuyenXemGiaVon() {
        ajaxHelper('/api/DanhMuc/ReportAPI/' + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDNguoiDung + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            if (data !== '') {
                self.HangHoa_XemGiaVon(true);
            }
            else {
                self.HangHoa_XemGiaVon(false);
            }
        });
    }

    function GetTree_NhomHangHoa() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetTree_NhomHangHoa', 'GET').done(function (obj) {
                if (obj.res === true) {
                    let data = obj.data;
                    if (data.length > 0) {
                        data = data.sort((a, b) => a.text.localeCompare(b.text, undefined, { caseFirst: "upper" }));
                    }
                    self.NhomHangHoas(data);
                }
            })
        }
    }
    self.AllAccountBank = ko.observableArray();

    function getAllTaiKhoanNganHang() {
        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + _idDonVi, 'GET').done(function (x) {
            if (x.res === true) {
                let data = x.data;
                self.AllAccountBank(data);
                for (var i = 0; i < data.length; i++) {
                    if (data[i].TaiKhoanPOS === true) {
                        self.TaiKhoansPOS.push(data[i]);
                    }
                    else {
                        self.TaiKhoansCK.push(data[i]);
                    }
                }
            }
        })
    }

    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data != null) {
                self.CongTy(data);
            }
        });
    }

    function GetInforChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + 'GetDM_DonVi/' + _idDonVi, 'GET').done(function (data) {
            if (data != null) {
                self.ChiNhanh(data);
            }
        });
    }

    function newCTNhap(addCungLoai, itemHH, soluong) {
        let dongia = itemHH.DonGia;
        //if (self.newHoaDon().LoaiHoaDon() === 14) {
        //    dongia = 0;
        //}
        let ptThue = self.newHoaDon().PTThueHoaDon() > 0 ? self.newHoaDon().PTThueHoaDon() : 0;
        let ptCKHangHoa = self.newHoaDon().PTChietKhauHH() > 0 ? self.newHoaDon().PTChietKhauHH() : 0;
        let tienCK = ptCKHangHoa * dongia / 100;
        let tienThue = ptThue * (dongia - tienCK) / 100;
        let idRandomHD = self.newHoaDon().IDRandom();

        let lotParent = itemHH.QuanLyTheoLoHang ? true : false;
        if (addCungLoai) {
            return {
                ID_HangHoa: itemHH.ID_HangHoa,
                SoThuTu: 1,
                IDRandom: CreateIDRandom('CTHD_'),
                IDRandomHD: itemHH.IDRandomHD,
                ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
                MaHangHoa: itemHH.MaHangHoa,
                TenHangHoa: itemHH.TenHangHoa,
                TenDonViTinh: itemHH.TenDonViTinh,
                DonViTinh: itemHH.DonViTinh,
                TyLeChuyenDoi: itemHH.TyLeChuyenDoi,
                ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
                QuanLyTheoLoHang: itemHH.QuanLyTheoLoHang,
                TonKho: itemHH.TonKho,
                SrcImage: itemHH.SrcImage,
                DonGia: dongia,
                GiaBanHH: itemHH.GiaBanHH,// used to get GiaBan when save DB
                GiaVon: itemHH.GiaVon,
                SoLuong: 1,
                TienChietKhau: tienCK,
                PTChietKhau: ptCKHangHoa,
                DVTinhGiam: ptCKHangHoa > 0 || tienCK == 0 ? '%' : 'VND',
                ThanhTien: dongia - (ptCKHangHoa * dongia) / 100,// sauCK
                ThanhToan: dongia - tienCK + tienThue,// sauthue
                GhiChu: '',
                ID_LoHang: itemHH.ID_LoHang,
                MaLoHang: '',
                NgaySanXuat: '',
                NgayHetHan: '',
                DM_LoHang: [],
                HangCungLoais: [],
                LotParent: false,
                LaConCungLoai: addCungLoai,
                PTThue: ptThue,
                TienThue: tienThue,
                SoLuongConLai: 0,
                ID_ChiTietGoiDV: null,
            }
        }
        else {
            var ngaysx = moment(itemHH.NgaySanXuat).format('DD/MM/YYYY');
            var hethan = moment(itemHH.NgayHetHan).format('DD/MM/YYYY');
            if (ngaysx === 'Invalid date') {
                ngaysx = '';
            }

            if (hethan === 'Invalid date') {
                hethan = '';
            }
            let gianhap = dongia;
            gianhap = gianhap == 0 ? itemHH.GiaVon : gianhap;
            //if (self.newHoaDon().LoaiHoaDon() === 14) {
            //    gianhap = 0;
            //}
            tienCK = ptCKHangHoa * gianhap / 100;
            tienThue = ptThue * (gianhap - tienCK) / 100;
            return {
                ID_HangHoa: itemHH.ID_HangHoa,
                SoThuTu: 1,
                IDRandom: CreateIDRandom('CTHD_'),
                IDRandomHD: idRandomHD,
                ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
                MaHangHoa: itemHH.MaHangHoa,
                TenHangHoa: itemHH.TenHangHoa,
                TenDonViTinh: itemHH.TenDonViTinh,
                DonViTinh: itemHH.DonViTinh,
                TyLeChuyenDoi: itemHH.TyLeChuyenDoi,
                ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
                QuanLyTheoLoHang: itemHH.QuanLyTheoLoHang,
                TonKho: itemHH.TonKho,
                SrcImage: itemHH.SrcImage,
                DonGia: gianhap,// get gianhap old
                GiaBanHH: itemHH.GiaBan,
                GiaVon: itemHH.GiaVon,
                SoLuong: soluong,
                TienChietKhau: tienCK,
                PTChietKhau: ptCKHangHoa,
                DVTinhGiam: ptCKHangHoa > 0 || tienCK == 0 ? '%' : 'VND',
                ThanhTien: (gianhap - tienCK) * soluong,
                ThanhToan: (gianhap - tienCK + tienThue) * soluong,
                GhiChu: '',
                ID_LoHang: itemHH.ID_LoHang,
                MaLoHang: itemHH.MaLoHang,
                NgaySanXuat: ngaysx,
                NgayHetHan: hethan,
                DM_LoHang: [],
                HangCungLoais: [],
                LotParent: lotParent,
                LaConCungLoai: addCungLoai,
                PTThue: ptThue,
                TienThue: tienThue,
                SoLuongConLai: 0,
                ID_ChiTietGoiDV: null,
            }
        }
    }

    function newLot(itemHH, parent) {
        parent = parent || false;
        let idRandom = parent ? itemHH.IDRandom : CreateIDRandom('CTHD_');
        var ngaysx = '';
        var hethan = '';
        if (itemHH.NgaySanXuat !== null && itemHH.NgaySanXuat !== undefined) {
            if (itemHH.NgaySanXuat.split('/').length === 0) {
                ngaysx = moment(itemHH.NgaySanXuat).format('DD/MM/YYYY');
            }
            else {
                ngaysx = itemHH.NgaySanXuat;
            }
        }
        if (itemHH.NgayHetHan !== null && itemHH.NgayHetHan !== undefined) {
            if (itemHH.NgayHetHan.split('/').length === 0) {
                hethan = moment(itemHH.NgayHetHan).format('DD/MM/YYYY');
            }
            else {
                hethan = itemHH.NgayHetHan;
            }
        }

        if (ngaysx === 'Invalid date') {
            ngaysx = '';
        }

        if (hethan === 'Invalid date') {
            hethan = '';
        }

        var dongia = itemHH.DonGia;
        //if (self.newHoaDon().LoaiHoaDon() === 14) {
        //    dongia = 0;
        //}
        var ptThue = self.newHoaDon().PTThueHoaDon() > 0 ? self.newHoaDon().PTThueHoaDon() : 0;
        var ptCKHangHoa = self.newHoaDon().PTChietKhauHH() > 0 ? self.newHoaDon().PTChietKhauHH() : 0;
        var tienCK = ptCKHangHoa * dongia / 100;
        var tienThue = ptThue * (dongia - tienCK) / 100;

        return {
            SoThuTu: 1,
            IDRandom: idRandom,
            IDRandomHD: self.newHoaDon().IDRandom(),
            ID_HangHoa: itemHH.ID_HangHoa,// used to save DB
            ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
            ID_LoHang: itemHH.ID_LoHang,
            MaLoHang: itemHH.MaLoHang,
            NgaySanXuat: ngaysx,
            NgayHetHan: hethan,
            TonKho: itemHH.TonKho,
            DonGia: dongia,
            GiaBanHH: itemHH.GiaBanHH,
            GiaVon: itemHH.GiaVon,
            SoLuong: parent ? itemHH.SoLuong : 1,
            TienChietKhau: tienCK,
            PTChietKhau: ptCKHangHoa,
            DVTinhGiam: ptCKHangHoa > 0 || tienCK == 0 ? '%' : 'VND',
            ThanhTien: dongia - tienCK,
            ThanhToan: dongia - tienCK + tienThue,
            GhiChu: '',
            LotParent: parent ? true : false,
            LaConCungLoai: false,
            QuanLyTheoLoHang: true,
            NguoiTao: _userLogin,// used to save DB
            TenHangHoa: itemHH.TenHangHoa,
            MaHangHoa: itemHH.MaHangHoa,
            TenDonViTinh: itemHH.TenDonViTinh,
            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
            PTThue: ptThue,
            TienThue: tienThue,
            SoLuongConLai: 0,
            ID_ChiTietGoiDV: null,
        }
    }

    function FindCTHD_isDoing(item) {
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var concungloai = item.LaConCungLoai;
        var idRandom = item.IDRandom;

        var lstCTHD = localStorage.getItem(lcCTNhapHang);
        if (lstCTHD !== null) {
            lstCTHD = JSON.parse(lstCTHD);
            if (quanLiTheoLo) {
                if (item.LotParent) {
                    for (let i = 0; i < lstCTHD.length; i++) {
                        if (lstCTHD[i].IDRandom === idRandom) {
                            return lstCTHD[i];
                        }
                    }
                }
                else {
                    for (let i = 0; i < lstCTHD.length; i++) {
                        for (let j = 0; j < lstCTHD[i].DM_LoHang.length; j++) {
                            if (lstCTHD[i].DM_LoHang[j].IDRandom === idRandom) {
                                return lstCTHD[i].DM_LoHang[j];
                            }
                        }
                    }
                }
            }
            else {
                if (concungloai) {
                    for (let i = 0; i < lstCTHD.length; i++) {
                        for (let j = 0; j < lstCTHD[i].HangCungLoais.length; j++) {
                            if (lstCTHD[i].HangCungLoais[j].IDRandom === idRandom) {
                                return lstCTHD[i].HangCungLoais[j];
                            }
                        }
                    }
                }
                else {
                    for (let i = 0; i < lstCTHD.length; i++) {
                        if (lstCTHD[i].IDRandom === idRandom) {
                            return lstCTHD[i];
                        }
                    }
                }
            }
        }
        return null;
    }

    function SetValueInput_whenEdit(type, idRandom, thanhtien, dongia) {
        switch (type) {
            case 1:// thanhtien
                $('#thanhtien_' + idRandom).val(formatNumber3Digit(thanhtien, 2));
                break;
            case 2:// dongia
                $('#giavon_' + idRandom).val(formatNumber3Digit(dongia, 2));
                break;
        }
    }

    function updateCTHDLe(arr, ctDoing) {
        var quanlytheolo = ctDoing.QuanLyTheoLoHang;
        var concungloai = ctDoing.LaConCungLoai;
        var lotParent = ctDoing.LotParent;
        var idRandom = ctDoing.IDRandom;

        var newSoLuong = ctDoing.SoLuong;
        var newDonGia = ctDoing.DonGia;
        var tienGiam = ctDoing.TienChietKhau;
        var ptGiam = ctDoing.PTChietKhau;
        var tienThue = ctDoing.TienThue;
        var ptThue = ctDoing.PTThue;
        var thanhtien = ctDoing.ThanhTien;
        var thanhtoan = ctDoing.ThanhToan;

        if (lotParent || (concungloai === false && quanlytheolo === false)) {
            for (let i = 0; i < arr.length; i++) {
                if (arr[i].IDRandom === idRandom) {
                    arr[i].ThanhTien = thanhtien;
                    arr[i].ThanhToan = thanhtoan;

                    arr[i].SoLuong = newSoLuong;
                    arr[i].DonGia = newDonGia;
                    arr[i].PTChietKhau = ptGiam;
                    arr[i].TienChietKhau = tienGiam;
                    arr[i].PTThue = ptThue;
                    arr[i].TienThue = tienThue;

                    if (lotParent) {
                        arr[i].DM_LoHang[0].SoLuong = newSoLuong;
                        arr[i].DM_LoHang[0].DonGia = newDonGia;
                        arr[i].DM_LoHang[0].PTChietKhau = ptGiam;
                        arr[i].DM_LoHang[0].TienChietKhau = tienGiam;
                        arr[i].DM_LoHang[0].PTThue = ptThue;
                        arr[i].DM_LoHang[0].TienThue = tienThue;
                        arr[i].DM_LoHang[0].ThanhTien = arr[i].ThanhTien;
                        arr[i].DM_LoHang[0].ThanhToan = arr[i].ThanhToan;
                    }
                    break;
                }
            }
        }
        else {
            if (quanlytheolo) {
                for (let i = 0; i < arr.length; i++) {
                    for (let j = 0; j < arr[i].DM_LoHang.length; j++) {
                        if (arr[i].DM_LoHang[j].IDRandom === idRandom) {
                            arr[i].DM_LoHang[j].ThanhTien = thanhtien;
                            arr[i].DM_LoHang[j].ThanhToan = thanhtoan;
                            arr[i].DM_LoHang[j].SoLuong = newSoLuong;
                            arr[i].DM_LoHang[j].DonGia = newDonGia;
                            arr[i].DM_LoHang[j].PTChietKhau = ptGiam;
                            arr[i].DM_LoHang[j].TienChietKhau = tienGiam;
                            arr[i].DM_LoHang[j].PTThue = ptThue;
                            arr[i].DM_LoHang[j].TienThue = tienThue;
                            i = arr.length;// used to esc out for loop
                            break;
                        }
                    }
                }
            }
            else {
                for (let i = 0; i < arr.length; i++) {
                    for (let j = 0; j < arr[i].HangCungLoais.length; j++) {
                        if (arr[i].HangCungLoais[j].IDRandom === idRandom) {
                            arr[i].HangCungLoais[j].ThanhTien = thanhtien;
                            arr[i].HangCungLoais[j].ThanhToan = thanhtoan;
                            arr[i].HangCungLoais[j].SoLuong = newSoLuong;
                            arr[i].HangCungLoais[j].DonGia = newDonGia;
                            arr[i].HangCungLoais[j].PTChietKhau = ptGiam;
                            arr[i].HangCungLoais[j].TienChietKhau = tienGiam;
                            arr[i].HangCungLoais[j].PTThue = ptThue;
                            arr[i].HangCungLoais[j].TienThue = tienThue;
                            i = arr.length;
                            break;
                        }
                    }
                }
            }
        }
        return arr;
    }

    function XoaHangHoa_CheckCungLoai(cthd, lotParent, quanlytheolo, concungloai, idRandom) {
        if (lotParent || (concungloai === false && quanlytheolo === false)) {
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === idRandom) {
                    cthd.splice(i, 1);
                    break;
                }
            }
        }
        else {
            if (quanlytheolo) {
                for (let i = 0; i < cthd.length; i++) {
                    for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                        if (cthd[i].DM_LoHang[j].IDRandom === idRandom) {
                            cthd[i].DM_LoHang.splice(j, 1);
                            i = cthd.length;  // esc for loop
                            break;
                        }
                    }
                }
            }
            else {
                for (let i = 0; i < cthd.length; i++) {
                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                        if (cthd[i].HangCungLoais[j].IDRandom === idRandom) {
                            cthd[i].HangCungLoais.splice(j, 1);
                            i = cthd.length;
                            break;
                        }
                    }
                }
            }
        }
        return cthd;
    }

    self.LenCTHD = ko.computed(function () {
        return self.HangHoaAfterAdd().length;
    });

    function Caculator_AmountProduct(idRandomHD = null) {
        if (commonStatisJs.CheckNull(idRandomHD)) {
            idRandomHD = self.newHoaDon().IDRandom();
        }
        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            let ctThis = $.grep(cthd, function (x) {
                return x.IDRandomHD === idRandomHD;
            })

            var sumQuantity = 0;
            for (let i = 0; i < ctThis.length; i++) {
                sumQuantity += parseFloat(ctThis[i].SoLuong);

                // count Lot in Hang hoa
                for (let k = 1; k < ctThis[i].DM_LoHang.length; k++) {
                    sumQuantity += parseFloat(ctThis[i].DM_LoHang[k].SoLuong);
                }

                // count hangcungloai
                for (let k = 0; k < ctThis[i].HangCungLoais.length; k++) {
                    sumQuantity += parseFloat(ctThis[i].HangCungLoais[k].SoLuong);
                }
            }

            // round number to 3 decimals 
            let numberRound = Math.round(sumQuantity * 1000) / 1000;
            self.TongSoLuongHH(numberRound);
        }
    }

    function UpdateAgain_DonViTinhCTHD(idHangHoa, cthd) {
        let idRandomHD = self.newHoaDon().IDRandom();
        let cthd_sameIDHangHoa = $.grep(cthd, function (x) {
            return x.IDRandomHD === idRandomHD && x.ID_HangHoa === idHangHoa;
        });

        // get all dvt of this hanghoa
        var arrDVT = [];
        for (let i = 0; i < cthd_sameIDHangHoa.length; i++) {
            let itFor = cthd_sameIDHangHoa[i];
            if (arrDVT.filter(x => x.ID_DonViQuiDoi === itFor.ID_DonViQuiDoi).length === 0) {
                arrDVT.push({ ID_DonViQuiDoi: itFor.ID_DonViQuiDoi, TenDonViTinh: itFor.TenDonViTinh, Xoa: false });
            }

            for (let j = 0; j < itFor.DonViTinh.length; j++) {
                // check exist in arrDVT & push
                let itFor2 = itFor.DonViTinh[j];
                if (arrDVT.filter(x => x.ID_DonViQuiDoi === itFor2.ID_DonViQuiDoi).length === 0) {
                    arrDVT.push({ ID_DonViQuiDoi: itFor2.ID_DonViQuiDoi, TenDonViTinh: itFor2.TenDonViTinh, Xoa: false });
                }
            }
        }

        // update again lst DVT to cthd
        let find = 0;
        for (let i = 0; i < cthd.length; i++) {
            let itFor = cthd[i];
            if (itFor.IDRandomHD === idRandomHD && cthd[i].ID_HangHoa === idHangHoa) {
                // get arrQuiDoi exist
                let arrIDQuiDoi = [];
                let arrEx = $.grep(cthd_sameIDHangHoa, function (x) {
                    return x.ID_DonViQuiDoi !== cthd[i].ID_DonViQuiDoi;
                });
                for (let k = 0; k < arrEx.length; k++) {
                    arrIDQuiDoi.push(arrEx[k].ID_DonViQuiDoi);
                }
                cthd[i].DonViTinh = $.grep(arrDVT, function (x) {
                    return $.inArray(x.ID_DonViQuiDoi, arrIDQuiDoi) == -1;
                });
                find = find + 1;
                if (find === cthd_sameIDHangHoa.length) {
                    i = cthd.length;// esc for loop
                }
            }
        }
        return cthd;
    }

    self.JqAutoSelectItem = function (itemChose) {
        if (!commonStatisJs.CheckNull(itemChose.ID_DonViQuiDoi)) {
            ajaxHelper(DMHangHoaUri + 'GetInforProduct_ByIDQuidoi?idQuiDoi=' + itemChose.ID_DonViQuiDoi
                + '&idChiNhanh=' + _idDonVi + '&idLoHang=' + itemChose.ID_LoHang).done(function (x) {
                    if (x.res === true) {
                        let lst = x.data;
                        let item = [];
                        if (itemChose.QuanLyTheoLoHang) {
                            // find in lst
                            let exItem = $.grep(lst, function (o) {
                                return o.ID_LoHang === itemChose.ID_LoHang;
                            });
                            if (exItem.length > 0) {
                                item = exItem[0];
                            }
                        }
                        else {
                            item = lst[0];
                        }
                        item.GiaBanHH = item.GiaBan;
                        item.ID_HangHoa = item.ID;
                        item.DonGia = item.GiaNhap;
                        self.ItemChosing(item);

                        if (self.IsNhapNhanh()) {
                            AddCTHD(item, 1);
                            $('jqauto-product ._jsInput').val(item.MaHangHoa);
                            $(function () {
                                $('jqauto-product ._jsInput').focus().select();
                            })
                        }
                        else {
                            $('#txtSoLuongHang').focus();
                        }
                    }
                });
        }
    }

    self.JqAutoSelect_Enter = function () {// not use
        if (self.IsNhapNhanh()) {
            let mahh = $('jqauto-product ._jsInput').val();
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByMaHangHoa?mahh=" + mahh + '&iddonvi=' + _idDonVi, 'GET').done(function (data) {
                console.log(data);
                if (data.length > 0) {
                    data = data.filter(p => p.LaHangHoa === true);
                    if (data.length > 0) {
                        data[0].GiaBanHH = data[0].GiaBan;
                        data[0].ID_HangHoa = data[0].ID;
                        data[0].DonGia = data[0].GiaNhap;
                        self.ItemChosing(data[0]);
                        AddCTHD(data[0], 1);
                    }
                }
                else {
                    ShowMessage_Danger('Mã hàng không tồn tại');
                }
            });
        }
        else {
            $('#txtSoLuongHang').focus();
        }
    }

    function CreateNewHoaDon() {
        var idNhanVien = self.newHoaDon().ID_NhanVien();
        if (commonStatisJs.CheckNull(idNhanVien)) {
            idNhanVien = _idNhanVien;
        }
        return {
            ID: const_GuidEmpty,
            IDRandom: CreateIDRandom('HD_'),
            LoaiHoaDon: self.LoaiHoaDonMenu(),
            MaHoaDon: '',
            ID_DoiTuong: null,
            ID_DonVi: _idDonVi,
            ID_NhanVien: idNhanVien,
            NguoiTao: VHeader.UserLogin,
            NgayLapHoaDon: null,
            TongTienHangChuaCK: 0,
            TongGiamGiaHang: 0,
            TongTienHang: 0,
            PTThueHoaDon: 0,
            TongTienThue: 0,
            PTChietKhauHH: 0,
            TongGiamGia: 0,
            TongChietKhau: 0,
            PhaiThanhToan: 0,
            TongThanhToan: 0,
            DaThanhToan: 0,
            HoanTra: 0,
            ConThieu: 0,
            KhachDaTra: 0,
            TienMat: 0,
            TienPOS: 0,
            TienChuyenKhoan: 0,
            TienDatCoc: 0,
            SoDuDatCoc: 0,
            TenTaiKhoanPos: '',
            TenTaiKhoanCK: '',
            ID_TaiKhoanChuyenKhoan: null,
            ID_TaiKhoanPos: null,
            ID_KhoanThuChi: null,
            YeuCau: self.LoaiHoaDonMenu() == 31 ? '1' : '',
        };
    }

    function AddCTHD(item, soluong) {
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
        }
        else {
            hd = [];
        }

        var lstCT = localStorage.getItem(lcCTNhapHang);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
        }
        else {
            lstCT = [];
        }

        let idRandomHD = self.newHoaDon().IDRandom();
        if (commonStatisJs.CheckNull(idRandomHD)) {
            let obj = CreateNewHoaDon();
            idRandomHD = obj.IDRandom;
            hd.push(obj);
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
            self.newHoaDon().IDRandom(idRandomHD);
        }

        var itemEx = $.grep(lstCT, function (x) {
            return x.IDRandomHD === idRandomHD && x.ID_DonViQuiDoi === item.ID_DonViQuiDoi;
        });
        if (itemEx.length > 0) {
            for (let i = 0; i < lstCT.length; i++) {
                let itFor = lstCT[i];
                if (itFor.IDRandomHD === idRandomHD && itFor.ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                    if (itFor.QuanLyTheoLoHang) {
                        let exLo = $.grep(itFor.DM_LoHang, function (o) {
                            return o.ID_LoHang === item.ID_LoHang;
                        });
                        if (exLo.length > 0) {
                            for (let j = 0; j < itFor.DM_LoHang.length; j++) {
                                if (itFor.DM_LoHang[j].ID_LoHang === item.ID_LoHang) {
                                    // if lot parent
                                    if (itFor.ID_LoHang === item.ID_LoHang) {
                                        lstCT[i].SoLuong = itFor.SoLuong + soluong;
                                        lstCT[i].ThanhTien = lstCT[i].SoLuong * (lstCT[i].DonGia - lstCT[i].TienChietKhau);
                                        lstCT[i].ThanhToan = lstCT[i].SoLuong * (lstCT[i].DonGia - lstCT[i].TienChietKhau + lstCT[i].TienThue);
                                    }
                                    lstCT[i].DM_LoHang[j].SoLuong = lstCT[i].DM_LoHang[j].SoLuong + soluong;
                                    lstCT[i].DM_LoHang[j].ThanhTien = lstCT[i].DM_LoHang[j].SoLuong * (lstCT[i].DM_LoHang[j].DonGia - lstCT[i].DM_LoHang[j].TienChietKhau);
                                    lstCT[i].DM_LoHang[j].ThanhToan = lstCT[i].DM_LoHang[j].SoLuong
                                        * (lstCT[i].DM_LoHang[j].DonGia - lstCT[i].DM_LoHang[j].TienChietKhau + lstCT[i].DM_LoHang[j].TienThue);

                                    i = lstCT.length;
                                    break;
                                }
                            }
                        }
                        else {
                            if (item.NgaySanXuat !== null && item.NgaySanXuat !== undefined) {
                                item.NgaySanXuat = moment(item.NgaySanXuat).format('DD/MM/YYYY');
                            }
                            if (item.NgayHetHan !== null && item.NgayHetHan !== undefined) {
                                item.NgayHetHan = moment(item.NgayHetHan).format('DD/MM/YYYY');
                            }
                            let obj = newLot(item);
                            obj.SoLuong = soluong;
                            obj.ThanhTien = soluong * obj.DonGia;
                            obj.ThanhToan = soluong * (obj.DonGia - obj.TienThue);
                            lstCT[i].DM_LoHang.push(obj);
                        }
                    }
                    else {
                        lstCT[i].SoLuong = itFor.SoLuong + soluong;
                        lstCT[i].ThanhTien = lstCT[i].SoLuong * (lstCT[i].DonGia - lstCT[i].TienChietKhau);
                        lstCT[i].ThanhToan = lstCT[i].SoLuong * (lstCT[i].DonGia - lstCT[i].TienChietKhau + lstCT[i].TienThue);
                    }
                    break;
                }
            }
        }
        else {
            let newCT = newCTNhap(false, item, soluong);
            if (item.QuanLyTheoLoHang) {
                let objLo = newLot(newCT, true);
                newCT.DM_LoHang.push(objLo);
            }
            lstCT.unshift(newCT);
        }
        lstCT = UpdateAgain_DonViTinhCTHD(item.ID_HangHoa, lstCT);
        localStorage.setItem(lcCTNhapHang, JSON.stringify(lstCT));

        UpdateSoThuTu_CTHD();

        Bind_UpdateHD();
        BindCTHD_byIDRandomHD();
        BindHD_byIDRandom();
        Caculator_AmountProduct();
    }

    function Focus_InputTienTraHD() {
        $('#txtPaid').focus().select();
    }

    function UpdateSoThuTu_CTHD() {
        let idRandomHD = self.newHoaDon().IDRandom();
        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            // get arrHD with MaHoaDon
            let arrHD = $.grep(cthd, function (item) {
                return item.IDRandomHD === idRandomHD;
            });
            // update again SoThuTu for CTHD with MaHoaDon
            let stt = 1;
            for (let i = arrHD.length - 1; i >= 0; i--) {
                arrHD[i].SoThuTu = stt;
                stt = stt + 1;
            }
            for (let i = cthd.length - 1; i >= 0; i--) {
                if (cthd[i].IDRandomHD === idRandomHD) {
                    for (let j = 0; j < arrHD.length; j++) {
                        if (cthd[i].IDRandom === arrHD[j].IDRandom) {
                            cthd[i].SoThuTu = arrHD[j].SoThuTu;
                        }
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
        }
    }

    function Enter_CTHD(itemCT, e, charStart) {
        var key = e.keyCode || e.which;

        if (key === 13) {
            var cthd = JSON.parse(localStorage.getItem(lcCTNhapHang));
            var idRandomFocus = null;
            var lenCTHD = cthd.length;
            for (let i = 0; i < lenCTHD; i++) {
                if (cthd[i].ID_DonViQuiDoi === itemCT.ID_DonViQuiDoi) {
                    if (cthd[i].DM_LoHang.length > 0) {
                        if (cthd[i].IDRandom === itemCT.IDRandom) {
                            if (cthd[i].DM_LoHang.length > 1) {
                                idRandomFocus = cthd[i].DM_LoHang[1].IDRandom;
                            }
                            else {
                                if (i < cthd.length - 1) {
                                    idRandomFocus = cthd[i + 1].IDRandom;
                                }
                                else {
                                    Focus_InputTienTraHD();
                                    return false;
                                }
                            }
                        }
                        else {
                            // find LoHang
                            for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                if (cthd[i].DM_LoHang[j].IDRandom === itemCT.IDRandom) {
                                    if (j < cthd[i].DM_LoHang.length - 1) {
                                        // focus in next Lot
                                        idRandomFocus = cthd[i].DM_LoHang[j + 1].IDRandom;
                                    }
                                    else {
                                        // find li next
                                        if (i < cthd.length - 1) {
                                            // focus in next cthd
                                            idRandomFocus = cthd[i + 1].IDRandom;
                                        }
                                        else {
                                            Focus_InputTienTraHD();
                                            return false;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        // find li next
                        if (i < cthd.length - 1) {
                            if (cthd[i].IDRandom === itemCT.IDRandom) {
                                // find in HangCungLoai
                                if (cthd[i].HangCungLoais.length > 0) {
                                    // focus HangCungLoai first
                                    idRandomFocus = cthd[i].HangCungLoais[0].IDRandom;
                                }
                                else {
                                    // focus in next Lot
                                    idRandomFocus = cthd[i + 1].IDRandom;
                                }
                            }
                            else {
                                // find in HangCungLoai
                                if (cthd[i].HangCungLoais.length > 0) {
                                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                                        if (cthd[i].HangCungLoais[j].IDRandom === itemCT.IDRandom) {
                                            if (j < cthd[i].HangCungLoais.length - 1) {
                                                // focus in next cungloai
                                                idRandomFocus = cthd[i].HangCungLoais[j + 1].IDRandom;
                                                i = lenCTHD;
                                                break;
                                            }
                                            else {
                                                // focus in next cthd
                                                idRandomFocus = cthd[i + 1].IDRandom;
                                                i = lenCTHD;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else {
                                    Focus_InputTienTraHD();
                                    return false;
                                }
                            }
                        }
                        else {
                            if (cthd[i].IDRandom === itemCT.IDRandom) {
                                if (cthd[i].HangCungLoais.length > 0) {
                                    // focus HangCungLoai first
                                    idRandomFocus = cthd[i].HangCungLoais[0].IDRandom;
                                }
                                else {
                                    // focus in next Lot
                                    Focus_InputTienTraHD();
                                    return false;
                                }
                            }
                            else {
                                // find HangCungLoai
                                if (cthd[i].HangCungLoais.length > 0) {
                                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                                        if (cthd[i].HangCungLoais[j].IDRandom === itemCT.IDRandom) {
                                            if (j < cthd[i].HangCungLoais.length - 1) {
                                                // focus in next cungloai
                                                idRandomFocus = cthd[i].HangCungLoais[j + 1].IDRandom;
                                                i = lenCTHD;
                                                break;
                                            }
                                            else {
                                                // focus in next cthd
                                                if (i < cthd.length - 1) {
                                                    idRandomFocus = cthd[i + 1].IDRandom;
                                                    i = lenCTHD;
                                                    break;
                                                }
                                                else {
                                                    Focus_InputTienTraHD();
                                                    return false;
                                                }
                                            }
                                        }
                                    }
                                }
                                else {
                                    Focus_InputTienTraHD();
                                    return false;
                                }
                            }
                        }
                    }
                    break;
                }
            }
            if (idRandomFocus !== null) {
                $('#' + charStart + idRandomFocus).focus().select();
            }
        }
    }

    function Shift_CTHD(itemCT, e, charStart) {
        var key = e.keyCode || e.which;
        // 16.shift
        if (key === 16) {
            // get all CTHD of HD opening
            var cthd = JSON.parse(localStorage.getItem(lcCTNhapHang));
            var idRandomFocus = null;

            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].ID_DonViQuiDoi === itemCT.ID_DonViQuiDoi) {
                    if (cthd[i].DM_LoHang.length > 0) {
                        if (cthd[i].IDRandom === itemCT.IDRandom) {
                            if (i - 1 >= 0) {
                                let lstLoHang = cthd[i - 1].DM_LoHang;
                                if (lstLoHang.length > 0) {
                                    // focus LoHang last
                                    idRandomFocus = lstLoHang[lstLoHang.length - 1].IDRandom;
                                }
                                else {
                                    // focus in prev li
                                    lstLoHang = cthd[i - 1].HangCungLoais;
                                    if (lstLoHang.length > 0) {
                                        idRandomFocus = lstLoHang[lstLoHang.length - 1].IDRandom;
                                    }
                                    else {
                                        idRandomFocus = cthd[i - 1].IDRandom;
                                    }
                                }
                            }
                            else {
                                Focus_InputTienTraHD();
                                return false;
                            }
                        }
                        else {
                            // find LoHang
                            for (var j = 1; j < cthd[i].DM_LoHang.length; j++) {
                                if (cthd[i].DM_LoHang[j].IDRandom === itemCT.IDRandom) {
                                    if (j - 1 === 0) {
                                        idRandomFocus = cthd[i].IDRandom;
                                    }
                                    else {
                                        idRandomFocus = cthd[i].DM_LoHang[j - 1].IDRandom;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        if (cthd[i].HangCungLoais.length > 0) {
                            // find hangcungloai
                            if (cthd[i].IDRandom === itemCT.IDRandom) {
                                if (i - 1 >= 0) {
                                    let lstLoHang = cthd[i - 1].DM_LoHang;
                                    if (lstLoHang.length > 0) {
                                        // focus LoHang last
                                        idRandomFocus = lstLoHang[lstLoHang.length - 1].IDRandom;
                                    }
                                    else {
                                        // focus in prev li
                                        lstLoHang = cthd[i - 1].HangCungLoais;
                                        if (lstLoHang.length > 0) {
                                            idRandomFocus = lstLoHang[lstLoHang.length - 1].IDRandom;
                                        }
                                        else {
                                            idRandomFocus = cthd[i - 1].IDRandom;
                                        }
                                    }
                                }
                                else {
                                    Focus_InputTienTraHD();
                                    return false;
                                }
                            }
                            else {
                                for (var j = 0; j < cthd[i].HangCungLoais.length; j++) {
                                    if (cthd[i].HangCungLoais[j].IDRandom === itemCT.IDRandom) {
                                        if (j - 1 >= 0) {
                                            idRandomFocus = cthd[i].HangCungLoais[j - 1].IDRandom;
                                        }
                                        else {
                                            idRandomFocus = cthd[i].IDRandom;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else {
                            // find hangcungloai
                            if (cthd[i].IDRandom === itemCT.IDRandom) {
                                if (i - 1 >= 0) {
                                    let lstLoHang = cthd[i - 1].DM_LoHang;
                                    if (lstLoHang.length > 0) {
                                        // focus LoHang last
                                        idRandomFocus = lstLoHang[lstLoHang.length - 1].IDRandom;
                                    }
                                    else {
                                        // focus in prev li
                                        lstLoHang = cthd[i - 1].HangCungLoais;
                                        if (lstLoHang.length > 0) {
                                            idRandomFocus = lstLoHang[lstLoHang.length - 1].IDRandom;
                                        }
                                        else {
                                            idRandomFocus = cthd[i - 1].IDRandom;
                                        }
                                    }
                                }
                                else {
                                    Focus_InputTienTraHD();
                                    return false;
                                }
                            }
                            else {
                                Focus_InputTienTraHD();
                                return false;
                            }
                        }
                    }
                    break;
                }
            }
            if (idRandomFocus !== null) {
                $('#' + charStart + idRandomFocus).focus().select();
            }
        }
    }

    self.deleteChiTietHD = function (item) {
        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            let dvtDeleted = { ID_DonViQuiDoi: item.ID_DonViQuiDoi, TenDonViTinh: item.TenDonViTinh, Xoa: false };

            cthd = XoaHangHoa_CheckCungLoai(cthd, item.LotParent, item.QuanLyTheoLoHang, item.LaConCungLoai, item.IDRandom);
            // push dvt was delete into cthd (if same ID_HangHoa)
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandomHD === self.newHoaDon().IDRandom()
                    && cthd[i].ID_HangHoa === item.ID_HangHoa) {
                    cthd[i].DonViTinh.push(dvtDeleted);
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));

            Bind_UpdateHD();
            BindCTHD_byIDRandomHD();
            BindHD_byIDRandom();
            Caculator_AmountProduct();
        }
    }

    self.EditGiamGiaCTHD = function (item) {
        var sumTemp = 0;
        var idRandom = item.IDRandom;

        var thisObj = $(event.currentTarget); // input nhap giam gia
        var priceOld = item.DonGia;
        var tienGiam = 0;
        var ptGiam = 0;

        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing !== null) {
            priceOld = ctDoing.DonGia;
            ptGiam = ctDoing.PTChietKhau;
            tienGiam = ctDoing.TienChietKhau;
            soluong = ctDoing.SoLuong;
            dvtGiam = ctDoing.DVTinhGiam;

            // neu gia cu = 0 => khong cho phep nhap giam gia
            if (priceOld === 0) {
                thisObj.val(0);
            }
            else {
                // format 000,000,000
                formatNumberObj(thisObj);

                var valThis = thisObj.val();
                if (valThis === '') {
                    valThis = 0;
                }

                if ($('#noVnd_' + idRandom).hasClass('gb')) {
                    // neu giam gia > 100 %
                    if (formatNumberToFloat(valThis) > 100) {
                        thisObj.val(100);
                        valThis = 100;
                    }
                    tienGiam = priceOld * parseFloat(valThis) / 100;
                    ptGiam = parseFloat(valThis);
                }
                else {
                    // neu giam gia > gia cu
                    if (formatNumberToInt(valThis) > priceOld) {
                        thisObj.val(formatNumber3Digit(priceOld, 2));
                        valThis = priceOld;
                    }
                    tienGiam = formatNumberToInt(valThis);
                    dvtGiam = 'VND';
                }
                var giasauCK = priceOld - tienGiam;
                if (ctDoing.PTThue > 0) {
                    ctDoing.TienThue = ctDoing.PTThue * giasauCK / 100;
                }
                sumTemp = (priceOld - tienGiam + ctDoing.TienThue) * ctDoing.SoLuong;
            }

            ctDoing.PTChietKhau = ptGiam;
            ctDoing.TienChietKhau = tienGiam;
            ctDoing.ThanhToan = sumTemp;
            ctDoing.ThanhTien = (priceOld - tienGiam) * ctDoing.SoLuong;
            $('#thanhtien_' + idRandom).val(formatNumber3Digit(sumTemp, 2));
            $('#giamgia_' + idRandom).val(formatNumber3Digit(tienGiam, 2));
            $('#tax_' + idRandom).val(formatNumber3Digit(ctDoing.TienThue, 2));

            var cthd = localStorage.getItem(lcCTNhapHang);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = updateCTHDLe(cthd, ctDoing);
                localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                Bind_UpdateHD(cthd, 3);
            }
        }
    }


    self.EditSoLuong = function (item) {
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);
        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing !== null) {
            let soluong = formatNumberToFloat($(thisObj).val());
            let keyCode = event.keyCode || event.which;

            switch (keyCode) {
                case 38:// up
                    soluong = soluong + 1;
                    $(thisObj).val(formatNumber(soluong));
                    break;
                case 40:// down
                    if (soluong > 0) {
                        soluong = soluong - 1;
                    }
                    $(thisObj).val(formatNumber(soluong));
                    break;
            }

            if (!commonStatisJs.CheckNull(item.ID_ChiTietGoiDV)) {
                if (item.SoLuongConLai < soluong) {
                    $(thisObj).val(formatNumber(ctDoing.SoLuongConLai));
                    ShowMessage_Danger('Vui lòng không nhập quá số lượng đặt là ' + item.SoLuongConLai);
                    return;
                }
            }

            ctDoing.SoLuong = soluong;
            ctDoing.ThanhTien = soluong * (ctDoing.DonGia - ctDoing.TienChietKhau);
            ctDoing.ThanhToan = soluong * (ctDoing.DonGia - ctDoing.TienChietKhau + ctDoing.TienThue);
            $('#thanhtien_' + ctDoing.IDRandom).val(formatNumber3Digit(ctDoing.ThanhToan, 2));
            $('#lblthanhtien_' + ctDoing.IDRandom).text(formatNumber3Digit(ctDoing.ThanhToan, 2));
            var lstCT = localStorage.getItem(lcCTNhapHang);
            if (lstCT !== null) {
                lstCT = JSON.parse(lstCT);
                lstCT = updateCTHDLe(lstCT, ctDoing);
                localStorage.setItem(lcCTNhapHang, JSON.stringify(lstCT));
                Caculator_AmountProduct();
                Bind_UpdateHD(lstCT);
                Enter_CTHD(item, event, 'soluong_');
                Shift_CTHD(item, event, 'soluong_');
            }
        }
    }

    self.EditThanhTien = function (item) {
        var thisObj = event.currentTarget;
        var idRandom = item.IDRandom;
        formatNumberObj(thisObj);
        var thanhtien = formatNumberToFloat($(thisObj).val());
        var gianhap = 0;
        var soluong = 0;
        // caculator again giaban, tienchietkhau
        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing !== null) {
            gianhap = ctDoing.DonGia;
            soluong = ctDoing.SoLuong;

            if (soluong === 0) {
                // keep GiaBan, caculator again SoLuong
                soluong = thanhtien / gianhap;
                $('#soluong_' + idRandom).val(formatNumber(soluong));
            }
            else {
                gianhap = Caculator_Price_byTienGiam(0, soluong, thanhtien);
            }
        }
        // reset tienchietkhau, thue
        ctDoing.PTChietKhau = 0;
        ctDoing.TienChietKhau = 0;
        ctDoing.TienThue = 0;
        ctDoing.PTThue = 0;
        ctDoing.SoLuong = soluong;
        ctDoing.DonGia = gianhap;
        ctDoing.ThanhToan = thanhtien;
        ctDoing.ThanhTien = thanhtien;

        // reset ptthue hd
        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].PTThueHoaDon = 0;
                    hd[i].PTChietKhauHH = 0;
                    self.newHoaDon().PTThueHoaDon(0);
                    self.newHoaDon().PTChietKhauHH(0);
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }

        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            cthd = updateCTHDLe(cthd, ctDoing);
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
            Bind_UpdateHD(cthd);
        }
        $('#giavon_' + idRandom).val(formatNumber3Digit(gianhap, 2));
        $('#tax_' + idRandom).val(0);
        $('#giamgia_' + idRandom).val(0);
        Enter_CTHD(item, event, 'thanhtien_');
        Shift_CTHD(item, event, 'thanhtien_');
    }

    self.EditGiaNhap = function (item) {
        var idRandom = item.IDRandom;
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);
        var giaNhap = formatNumberToFloat($(thisObj).val());

        var ctDoing = FindCTHD_isDoing(item, false);
        if (ctDoing !== null) {
            var tienGiam = ctDoing.TienChietKhau;
            var tienThue = ctDoing.TienThue;
            var thanhtoan = 0;

            if (giaNhap === 0) {
                giaNhap = 0;
                tienGiam = 0;
                tienThue = 0;
                ctDoing.PTChietKhau = self.newHoaDon().PTChietKhauHH();
                ctDoing.PTThue = self.newHoaDon().PTThueHoaDon();;
            }

            if (ctDoing.PTChietKhau > 0) {
                tienGiam = ctDoing.PTChietKhau * giaNhap / 100;
            }
            if (ctDoing.PTThue > 0) {
                tienThue = ctDoing.PTThue * (giaNhap - tienGiam) / 100;
            }
            thanhtoan = parseFloat(ctDoing.SoLuong) * (giaNhap - tienGiam + tienThue);

            ctDoing.DonGia = giaNhap;
            ctDoing.TienChietKhau = tienGiam;
            ctDoing.TienThue = tienThue;
            ctDoing.ThanhToan = thanhtoan;
            ctDoing.ThanhTien = (giaNhap - tienGiam) * ctDoing.SoLuong;
            var cthd = localStorage.getItem(lcCTNhapHang);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = updateCTHDLe(cthd, ctDoing);
                localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                Bind_UpdateHD(cthd);
            }
            $('#tax_' + idRandom).val(formatNumber3Digit(tienThue, 2));
            $('#giamgia_' + idRandom).val(formatNumber3Digit(tienGiam, 2));
            $('#thanhtien_' + idRandom).val(formatNumber3Digit(ctDoing.ThanhToan, 2));
            Enter_CTHD(item, event, 'giavon_');
            Shift_CTHD(item, event, 'giavon_');
        }
    }

    self.UpdateGhiChu_CTHD = function (item) {
        var idRandom = item.IDRandom;
        var quanlyTheoLo = item.QuanLyTheoLoHang;
        var ghichu = $(event.currentTarget).val();
        var concungloai = item.LaConCungLoai;

        var lcCTHD = localStorage.getItem(lcCTNhapHang);
        if (lcCTHD !== null) {
            lcCTHD = JSON.parse(lcCTHD);
            if (item.LotParent || (concungloai === false && quanlyTheoLo === false)) {
                for (let i = 0; i < lcCTHD.length; i++) {
                    if (lcCTHD[i].IDRandom === idRandom) {
                        lcCTHD[i].GhiChu = ghichu;
                        if (item.LotParent) {
                            lcCTHD[i].DM_LoHang[0].GhiChu = ghichu;
                        }
                        break;
                    }
                }
            }
            else {
                if (quanlyTheoLo) {
                    for (let i = 0; i < lcCTHD.length; i++) {
                        for (let j = 0; j < lcCTHD[i].DM_LoHang.length; j++) {
                            if (lcCTHD[i].DM_LoHang[j].IDRandom === idRandom) {
                                lcCTHD[i].DM_LoHang[j].GhiChu = ghichu;
                                if (lcCTHD[i].ID_LoHang === item.ID_LoHang) {
                                    lcCTHD[i].GhiChu = ghichu;
                                }
                                i = lcCTHD.length;  // esc for loop
                                break;
                            }
                        }
                    }
                }
                else {
                    for (let i = 0; i < lcCTHD.length; i++) {
                        for (let j = 0; j < lcCTHD[i].HangCungLoais.length; j++) {
                            if (lcCTHD[i].HangCungLoais[j].IDRandom === idRandom) {
                                lcCTHD[i].HangCungLoais[j].GhiChu = ghichu;
                                i = lcCTHD.length;
                                break;
                            }
                        }
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(lcCTHD));
        }
    }

    self.ShowDivSaleCTHD = function (item) {
        var idRandom = item.IDRandom;
        var thisObj = $(event.currentTarget);
        var nextEl = $(thisObj).next();
        var ptGiam = 0;
        var tienGiam = 0;

        var ctDoing = FindCTHD_isDoing(item, false);
        if (ctDoing !== null) {
            ptGiam = ctDoing.PTChietKhau;
            tienGiam = ctDoing.TienChietKhau;
            dvtGiam = ctDoing.DVTinhGiam;

            if (ptGiam > 0 || (tienGiam === 0 && ptGiam === 0)) {
                nextEl.find('.jsPTram').addClass('gb');
                nextEl.find('.jsVND').removeClass('gb');
                $('#popnumber_' + idRandom).val(ptGiam);
            }
            else {
                nextEl.find('.jsVND').addClass('gb');
                nextEl.find('.jsPTram').removeClass('gb');
                $('#popnumber_' + idRandom).val(formatNumber3Digit(tienGiam));
            }
        }
        nextEl.show();
        nextEl.find('input').focus().select();
    }

    function updatePTVND(arr, ctDoing, loai, gtri, dvt) {
        var quanlytheolo = ctDoing.QuanLyTheoLoHang;
        var concungloai = ctDoing.LaConCungLoai;
        var lotParent = ctDoing.LotParent;
        var idRandom = ctDoing.IDRandom;

        if (lotParent || (concungloai === false && quanlytheolo === false)) {
            for (let i = 0; i < arr.length; i++) {
                if (arr[i].IDRandom === idRandom) {
                    switch (loai) {
                        case 1:// ck
                            arr[i].PTChietKhau = gtri;
                            arr[i].DVTinhGiam = dvt;
                            break;
                        case 2://thue
                            arr[i].PTThue = gtri;
                            break;
                    }
                    if (lotParent) {
                        arr[i].DM_LoHang[0].PTChietKhau = arr[i].PTChietKhau;
                        arr[i].DM_LoHang[0].DVTinhGiam = arr[i].DVTinhGiam;
                        arr[i].DM_LoHang[0].PTThue = arr[i].PTThue;
                    }
                    break;
                }
            }
        }
        else {
            if (quanlytheolo) {
                for (let i = 0; i < arr.length; i++) {
                    for (let j = 0; j < arr[i].DM_LoHang.length; j++) {
                        if (arr[i].DM_LoHang[j].IDRandom === idRandom) {
                            switch (loai) {
                                case 1:// ck
                                    arr[i].DM_LoHang[j].PTChietKhau = gtri;
                                    arr[i].DM_LoHang[j].DVTinhGiam = dvt;
                                    break;
                                case 2://thue
                                    arr[i].DM_LoHang[j].PTThue = gtri;
                                    break;
                            }
                            i = arr.length;// used to esc out for loop
                            break;
                        }
                    }
                }
            }
            else {
                for (let i = 0; i < arr.length; i++) {
                    for (let j = 0; j < arr[i].HangCungLoais.length; j++) {
                        if (arr[i].HangCungLoais[j].IDRandom === idRandom) {
                            switch (loai) {
                                case 1:// ck
                                    arr[i].HangCungLoais[j].PTChietKhau = gtri;
                                    arr[i].HangCungLoais[j].DVTinhGiam = dvt;
                                    break;
                                case 2://thue
                                    arr[i].HangCungLoais[j].PTThue = gtri;
                                    break;
                            }
                            i = arr.length;
                            break;
                        }
                    }
                }
            }
        }
        return arr;
    }

    self.ClickVND = function (item) {
        var idRandom = item.IDRandom;
        var $this = $(event.currentTarget);
        var objSale = $('#popnumber_' + idRandom); // input giam gia

        if ($this.hasClass('gb')) {
            $this.removeClass('gb');
            $('#noVnd_' + idRandom).addClass('gb');
        }
        else {
            $this.addClass('gb');
            $('#noVnd_' + idRandom).removeClass('gb');
        }

        var ctDoing = FindCTHD_isDoing(item, false);
        if (ctDoing !== null) {
            var priceOld = ctDoing.DonGia;
            var ptGiam = ctDoing.PTChietKhau;
            var tienGiam = ctDoing.TienChietKhau;
            var dvtGiam = ctDoing.DVTinhGiam;

            // if priceOld =0 (chua nhap gia von ban dau o hang hoa)
            if (priceOld !== 0) {
                if ($this.hasClass('gb')) {
                    objSale.val(formatNumber3Digit(tienGiam, 2));
                    dvtGiam = 'VND';
                    ptGiam = 0;
                }
                else {
                    // caculator again %
                    ptGiam = tienGiam / priceOld * 100;
                    objSale.val(ptGiam);
                    dvtGiam = '%';
                }

                var cthd = localStorage.getItem(lcCTNhapHang);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    cthd = updatePTVND(cthd, ctDoing, 1, ptGiam, dvtGiam);
                    localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                }
            }
            $(function () {
                $(objSale).focus();
            });
        }
    }

    self.ShowDiv_ThueHangHoa = function (item) {
        var idRandom = item.IDRandom;
        var thisObj = $(event.currentTarget);
        var nextEl = $(thisObj).next();
        var ptThue = 0;
        var tienThue = 0;

        var ctDoing = FindCTHD_isDoing(item, false);
        if (ctDoing !== null) {
            ptThue = ctDoing.PTThue;
            tienThue = ctDoing.TienThue;
            dvtGiam = ctDoing.DVTinhGiam;

            if (ptThue > 0 || (tienThue === 0 && ptThue === 0)) {
                nextEl.find('.jsPTram').addClass('picked');
                nextEl.find('.jsVND').removeClass('picked');
                $('#ipTaxCT_' + idRandom).val(ptThue);
            }
            else {
                nextEl.find('.jsVND').addClass('picked');
                nextEl.find('.jsPTram').removeClass('picked');
                $('#ipTaxCT_' + idRandom).val(formatNumber3Digit(tienThue));
            }
        }
        nextEl.show();
        nextEl.find('input').focus().select();
    }

    self.cthd_clickThue = function (item, clickPtram) {
        var idRandom = item.IDRandom;
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        quanLiTheoLo = (quanLiTheoLo === null ? false : quanLiTheoLo);
        var $this = $(event.currentTarget);
        $this.parent().find('.picked').removeClass('picked');
        $this.addClass('picked');

        var objSale = $('#ipTaxCT_' + idRandom);
        var tienThue = 0;
        var ptThue = 0;
        var priceOld = 0;
        var giaSauCK = 0;
        var ctDoing = FindCTHD_isDoing(item, false);
        if (ctDoing !== null) {
            priceOld = ctDoing.GiaBan;
            ptThue = ctDoing.PTThue;
            tienThue = ctDoing.TienThue;
            giaSauCK = ctDoing.DonGia - ctDoing.TienChietKhau;

            if (priceOld !== 0) {
                if (clickPtram) {
                    ptThue = tienThue / giaSauCK * 100;
                    objSale.val(formatNumber3Digit(ptThue));
                }
                else {
                    objSale.val(formatNumber3Digit(tienThue, 2));
                    ptThue = 0;
                }

                var cthd = localStorage.getItem(lcCTNhapHang);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    cthd = updatePTVND(cthd, ctDoing, 2, ptThue, ctDoing.DVTingGiam);
                    localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                }
            }
        }
    }

    self.cthd_editThue = function (item, e) {
        var sumTemp = 0;
        var idRandom = item.IDRandom;
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        quanLiTheoLo = (quanLiTheoLo === null ? false : quanLiTheoLo);

        var thisObj = $(event.currentTarget);
        var nextEl = $(thisObj).next();
        var priceOld = 0;
        var tienThue = 0;
        var ptThue = 0;
        var giaSauCK = 0;

        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing !== null) {
            priceOld = ctDoing.DonGia;
            ptThue = ctDoing.PTThue;
            giaSauCK = priceOld - ctDoing.TienChietKhau;

            if (priceOld === 0) {
                thisObj.val(0);
            }
            else {
                // format 000,000,000
                formatNumberObj(thisObj);
                var valThis = thisObj.val();
                if (valThis === '') {
                    valThis = 0;
                }
                if (nextEl.hasClass('picked') === false) {
                    ptThue = formatNumberToFloat(valThis);
                    if (ptThue > 100) {
                        ptThue = 100;
                        thisObj.val(100);
                    }
                    tienThue = giaSauCK * ptThue / 100;
                }
                else {
                    tienThue = formatNumberToFloat(valThis);
                }
                sumTemp = (giaSauCK + tienThue) * ctDoing.SoLuong;

                ctDoing.PTThue = ptThue;
                ctDoing.TienThue = tienThue;
                ctDoing.ThanhToan = sumTemp;
                ctDoing.ThanhTien = giaSauCK * ctDoing.SoLuong;
            }
            $('#thanhtien_' + idRandom).val(formatNumber3Digit(sumTemp, 2));
            $('#giamgia_' + idRandom).text(formatNumber3Digit(ctDoing.TienChietKhau, 2));
            $('#tax_' + idRandom).val(formatNumber3Digit(ctDoing.TienThue, 2));

            var cthd = localStorage.getItem(lcCTNhapHang);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = updateCTHDLe(cthd, ctDoing);
                localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                Bind_UpdateHD(cthd, 4);
            }
        }
    }

    self.ClickNoVND = function (item) {
        var idRandom = item.IDRandom;
        var $this = $(event.currentTarget);
        var objSale = $('#popnumber_' + idRandom); // input giam gia

        if ($this.hasClass('gb')) {
            $this.removeClass('gb');
            $('#vnd_' + idRandom).addClass('gb');
        }
        else {
            $this.addClass('gb');
            $('#vnd_' + idRandom).removeClass('gb');
        }

        var ctDoing = FindCTHD_isDoing(item, false);
        if (ctDoing !== null) {
            var priceOld = ctDoing.DonGia;
            var ptGiam = ctDoing.PTChietKhau;
            var tienGiam = ctDoing.TienChietKhau;
            var dvtGiam = ctDoing.DVTinhGiam;

            // if priceOld =0 (chua nhap gia von ban dau o hang hoa)
            if (priceOld !== 0) {
                if ($this.hasClass('gb')) {
                    // caculator agin %
                    ptGiam = tienGiam / priceOld * 100;
                    objSale.val(formatNumber3Digit(ptGiam));
                    dvtGiam = '%';
                }
                else {
                    objSale.val(formatNumber3Digit(tienGiam, 2));
                    dvtGiam = 'VND';
                    ptGiam = 0;
                }

                var cthd = localStorage.getItem(lcCTNhapHang);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    cthd = updatePTVND(cthd, ctDoing, 1, ptGiam, dvtGiam);
                    localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                }
            }
        }
        $(function () {
            $(objSale).focus();
        });
    }

    function ResetPTChietKhauHH_PTThue() {
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].PTChietKhauHH = 0;
                    hd[i].PTThueHoaDon = 0;
                    self.newHoaDon().PTChietKhauHH(0);
                    self.newHoaDon().PTThueHoaDon(0);
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
    }

    self.ChangeDonViTinh = function (item, parent) {
        let newIDQuiDoi = item.ID_DonViQuiDoi;
        let oldIDQuiDoi = parent.ID_DonViQuiDoi;
        let idRandomHD = self.newHoaDon().IDRandom();

        if (newIDQuiDoi !== oldIDQuiDoi) {
            let dvtOld = { ID_DonViQuiDoi: oldIDQuiDoi, TenDonViTinh: parent.TenDonViTinh, Xoa: false };

            var cthd = localStorage.getItem(lcCTNhapHang);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                // update lst DonViTinh for cthd
                for (let i = 0; i < cthd.length; i++) {
                    let itFor = cthd[i];
                    if (itFor.IDRandomHD === idRandomHD && itFor.ID_HangHoa === parent.ID_HangHoa) {
                        if (itFor.ID_DonViQuiDoi !== oldIDQuiDoi) {
                            cthd[i].DonViTinh.push(dvtOld);
                            cthd[i].DonViTinh = cthd[i].DonViTinh.filter(x => x.ID_DonViQuiDoi !== newIDQuiDoi);
                        }
                    }
                }

                // update DonGia, GiaVon, TonKho with new dvt
                ajaxHelper(DMHangHoaUri + 'GetDonViTinhKhacGiaoDich?iddvqd=' + newIDQuiDoi + '&iddv=' + _idDonVi, 'GET').done(function (data) {
                    let giavon = data[0].GiaVon;  // if LoHang not exist in DM_GiaVon --> get giavon of parent
                    let gianhap = data[0].GiaNhap;
                    if (parent.QuanLyTheoLoHang) {
                        if (data.filter(p => p.ID_LoHang === null).length === 0) {
                            data = data.filter(p => p.ID_LoHang === parent.ID_LoHang);
                            giavon = data[0].GiaVon;
                            gianhap = data[0].GiaNhap;
                        }
                    }
                    let mahang = data[0].MaHangHoa;
                    let giaban = data[0].GiaBanHH;
                    let tonkho = data[0].TonKho;
                    //if (self.newHoaDon().LoaiHoaDon() === 14) {
                    //    gianhap = 0;
                    //}

                    for (let i = 0; i < cthd.length; i++) {
                        let itFor = cthd[i];
                        if (itFor.IDRandomHD === idRandomHD && itFor.ID_DonViQuiDoi === oldIDQuiDoi) {
                            cthd[i].MaHangHoa = mahang;
                            cthd[i].DonGia = gianhap;
                            cthd[i].GiaVon = giavon;
                            cthd[i].GiaBanHH = giaban;
                            cthd[i].TonKho = tonkho;
                            cthd[i].TienChietKhau = 0;
                            cthd[i].PTChietKhau = 0;
                            cthd[i].PTThue = 0;
                            cthd[i].TienThue = 0;
                            cthd[i].DVTinhGiam = '%';
                            cthd[i].TenDonViTinh = item.TenDonViTinh;
                            cthd[i].ID_DonViQuiDoi = newIDQuiDoi;
                            cthd[i].ThanhTien = gianhap * cthd[i].SoLuong;
                            cthd[i].ThanhToan = cthd[i].ThanhTien;

                            for (let j = 0; j < itFor.DM_LoHang.length; j++) {
                                cthd[i].DM_LoHang[j].ID_DonViQuiDoi = newIDQuiDoi;
                                cthd[i].DM_LoHang[j].TenDonViTinh = item.TenDonViTinh;
                                cthd[i].DM_LoHang[j].MaHangHoa = mahang;
                                cthd[i].DM_LoHang[j].DonGia = gianhap;
                                cthd[i].DM_LoHang[j].GiaVon = giavon;
                                cthd[i].DM_LoHang[j].GiaBanHH = giaban;
                                cthd[i].DM_LoHang[j].TonKho = tonkho;
                                cthd[i].DM_LoHang[j].PTChietKhau = 0;
                                cthd[i].DM_LoHang[j].TienChietKhau = 0;
                                cthd[i].DM_LoHang[j].PTThue = 0;
                                cthd[i].DM_LoHang[j].TienThue = 0;
                                cthd[i].DM_LoHang[j].DVTinhGiam = '%';
                                cthd[i].DM_LoHang[j].ThanhTien = gianhap * cthd[i].DM_LoHang[j].SoLuong;
                                cthd[i].DM_LoHang[j].ThanhToan = cthd[i].DM_LoHang[j].ThanhTien;
                            }

                            for (let j = 0; j < itFor.HangCungLoais.length; j++) {
                                cthd[i].HangCungLoais[j].ID_DonViQuiDoi = newIDQuiDoi;
                                cthd[i].HangCungLoais[j].TenDonViTinh = item.TenDonViTinh;
                                cthd[i].HangCungLoais[j].MaHangHoa = mahang;
                                cthd[i].HangCungLoais[j].DonGia = gianhap;
                                cthd[i].HangCungLoais[j].GiaVon = giavon;
                                cthd[i].HangCungLoais[j].GiaBanHH = giaban;
                                cthd[i].HangCungLoais[j].TonKho = tonkho;
                                cthd[i].HangCungLoais[j].PTChietKhau = 0;
                                cthd[i].HangCungLoais[j].TienChietKhau = 0;
                                cthd[i].HangCungLoais[j].PTThue = 0;
                                cthd[i].HangCungLoais[j].TienThue = 0;
                                cthd[i].HangCungLoais[j].DVTinhGiam = '%';
                                cthd[i].HangCungLoais[j].ThanhTien = gianhap * cthd[i].HangCungLoais[j].SoLuong;
                                cthd[i].HangCungLoais[j].ThanhToan = cthd[i].HangCungLoais[j].ThanhTien;
                            }
                            break;
                        }
                    }
                    localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));

                    ResetPTChietKhauHH_PTThue();
                    Bind_UpdateHD(cthd);
                    BindHD_byIDRandom();
                    BindCTHD_byIDRandomHD();
                });
            }
        }
    }

    self.ClickThemLo = function (item) {
        let quanlytheolo = item.QuanLyTheoLoHang;
        let idQuiDoi = item.ID_DonViQuiDoi;
        let idRandomHD = self.newHoaDon().IDRandom();

        if (item.ID_ChiTietGoiDV !== null) {
            ShowMessage_Danger('Phiếu nhập từ PO. Vui lòng không thêm hàng');
            return;
        }

        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (quanlytheolo) {
                // add lo
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].IDRandomHD === idRandomHD && cthd[i].ID_DonViQuiDoi === idQuiDoi) {
                        let obj = newLot(cthd[i]);
                        obj.ID_LoHang = null;
                        obj.MaLoHang = '';
                        obj.NgaySanXuat = '';
                        obj.NgayHetHan = '';
                        cthd[i].DM_LoHang.push(obj);
                        break;
                    }
                }
            }
            else {
                // add hangcungloai
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].IDRandomHD === idRandomHD && cthd[i].ID_DonViQuiDoi === idQuiDoi) {
                        let obj = newCTNhap(true, cthd[i], 1);
                        cthd[i].HangCungLoais.push(obj);
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));

            Caculator_AmountProduct();
            Bind_UpdateHD();
            BindCTHD_byIDRandomHD();
            BindHD_byIDRandom();
        }
    }

    self.ResetLo = function (item) {
        var thisObj = $(event.currentTarget);
        var idRandom = item.IDRandom;
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var concungloai = item.LaConCungLoai;
        var lotParent = item.LotParent;

        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (lotParent || (concungloai === false && quanLiTheoLo === false)) {
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].IDRandom === idRandom) {
                        cthd[i].ID_LoHang = null;
                        cthd[i].MaLoHang = '';
                        cthd[i].NgaySanXuat = '';
                        cthd[i].NgayHetHan = '';

                        if (lotParent) {
                            cthd[i].DM_LoHang[0].ID_LoHang = null;
                            cthd[i].DM_LoHang[0].MaLoHang = '';
                            cthd[i].DM_LoHang[0].NgaySanXuat = '';
                            cthd[i].DM_LoHang[0].NgayHetHan = '';
                        }
                        break;
                    }
                }
            }
            else {
                if (quanLiTheoLo) {
                    for (let i = 0; i < cthd.length; i++) {
                        for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                            if (cthd[i].DM_LoHang[j].IDRandom === idRandom) {
                                cthd[i].DM_LoHang[j].ID_LoHang = null;
                                cthd[i].DM_LoHang[j].MaLoHang = '';
                                cthd[i].DM_LoHang[j].NgaySanXuat = '';
                                cthd[i].DM_LoHang[j].NgayHetHan = '';
                                i = cthd.length;// used to esc out for loop
                                break;
                            }
                        }
                    }
                }
                else {
                    for (let i = 0; i < cthd.length; i++) {
                        for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                            if (cthd[i].HangCungLoais[j].IDRandom === idRandom) {
                                cthd[i].HangCungLoais[j].ID_LoHang = null;
                                cthd[i].HangCungLoais[j].MaLoHang = '';
                                cthd[i].HangCungLoais[j].NgaySanXuat = '';
                                cthd[i].HangCungLoais[j].NgayHetHan = '';
                                i = cthd.length;
                                break;
                            }
                        }
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
        }
        thisObj.closest('.op-js-nhaphang-lo').find('input').val('');// empty malo
        thisObj.closest('.op-js-nhaphang-lo').find('input.js-ngaynhaplo').show();// show input ngaysx, hethan
        thisObj.closest('.op-js-nhaphang-lo').find('div.js-ngaynhaplo').hide();// hide div ngaysx, hethan
    }

    self.LoadListLoHang = function (item) {
        ajaxHelper(DMHangHoaUri + 'GetInforProduct_ByIDQuidoi?idQuiDoi=' + item.ID_DonViQuiDoi + '&idChiNhanh=' + _idDonVi).done(function (x) {
            if (x.res === true) {
                var lst = x.data;
                self.ListLot_ofProduct(lst.filter(x => x.ID_LoHang != null));
                self.ListLot_ofProductAll(self.ListLot_ofProduct());
            }
        });
    }

    self.SearchLoHang = function (item) {
        var malo = $('#AddNewLo' + item.IDRandom).val();
        var lst = [];
        if (malo !== "") {
            for (let i = 0; i < self.ListLot_ofProductAll().length; i++) {
                if (self.ListLot_ofProductAll()[i].MaLoHang.toLowerCase().includes(malo) === true) {
                    lst.push(self.ListLot_ofProductAll()[i]);
                }
            }
            self.ListLot_ofProduct(lst);
        }
        else {
            self.ListLot_ofProduct(self.ListLot_ofProductAll());
        }
    }

    self.ChangeLoHang = function (item) {
        var $this = $(event.currentTarget);
        var ngaysx = moment(item.NgaySanXuat).format('DD/MM/YYYY');
        var hethan = moment(item.NgayHetHan).format('DD/MM/YYYY');
        if (ngaysx === 'Invalid date') {
            ngaysx = '';
        }
        if (hethan === 'Invalid date') {
            hethan = '';
        }

        var idLoHang = item.ID_LoHang;
        var maLoHang = item.MaLoHang;
        let gianhap = item.GiaNhap;
        //if (self.newHoaDon().LoaiHoaDon() === 14) {
        //    gianhap = 0;
        //}
        var idRandom = $($this.closest('.js-IDlohang')).find('span').eq(0).attr('id');

        var ptThue = self.newHoaDon().PTThueHoaDon();
        var ptCKHang = self.newHoaDon().PTChietKhauHH();

        // update TonKho, GiaVon, GiaBan, Lo
        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                    let itFor = cthd[i].DM_LoHang[j];
                    // if parent
                    var tienthue = 0, tienchietkhau = 0;
                    if (cthd[i].IDRandom === idRandom) {
                        cthd[i].ID_LoHang = idLoHang;
                        cthd[i].MaLoHang = maLoHang;
                        cthd[i].NgaySanXuat = ngaysx;
                        cthd[i].NgayHetHan = hethan;
                        cthd[i].DonGia = gianhap;

                        if (ptCKHang > 0) {
                            tienchietkhau = gianhap * ptCKHang / 100;
                        }
                        if (ptThue > 0) {
                            tienthue = (gianhap - tienchietkhau) * ptThue / 100;
                        }
                        cthd[i].ThanhTien = (gianhap - tienchietkhau) * cthd[i].SoLuong;
                        cthd[i].ThanhToan = (gianhap - tienchietkhau + tienthue) * cthd[i].SoLuong;
                        cthd[i].TienChietKhau = tienchietkhau;
                        cthd[i].PTChietKhau = ptCKHang;
                        cthd[i].PTThue = ptThue;
                        cthd[i].TienThue = tienthue;
                        cthd[i].DVTinhGiam = ptCKHang > 0 || tienchietkhau === 0 ? '%' : 'VND';
                        cthd[i].GiaVon = item.GiaVon;
                        cthd[i].GiaBanHH = item.GiaBan;
                    }
                    if (itFor.IDRandom === idRandom) {
                        cthd[i].DM_LoHang[j].ID_LoHang = idLoHang;
                        cthd[i].DM_LoHang[j].MaLoHang = maLoHang;
                        cthd[i].DM_LoHang[j].NgaySanXuat = ngaysx;
                        cthd[i].DM_LoHang[j].NgayHetHan = hethan;
                        cthd[i].DM_LoHang[j].DonGia = gianhap;
                        if (ptCKHang > 0) {
                            tienchietkhau = gianhap * ptCKHang / 100;
                        }
                        if (ptThue > 0) {
                            tienthue = (gianhap - tienchietkhau) * ptThue / 100;
                        }
                        cthd[i].DM_LoHang[j].TienChietKhau = tienchietkhau;
                        cthd[i].DM_LoHang[j].PTChietKhau = ptCKHang;
                        cthd[i].DM_LoHang[j].PTThue = ptThue;
                        cthd[i].DM_LoHang[j].TienThue = tienthue;
                        cthd[i].DM_LoHang[j].ThanhTien = (gianhap - tienchietkhau) * cthd[i].DM_LoHang[j].SoLuong;
                        cthd[i].DM_LoHang[j].ThanhToan = (gianhap - tienchietkhau + tienthue) * cthd[i].DM_LoHang[j].SoLuong;
                        cthd[i].DM_LoHang[j].DVTinhGiam = ptCKHang > 0 || tienchietkhau === 0 ? '%' : 'VND';
                        cthd[i].DM_LoHang[j].GiaVon = item.GiaVon;
                        cthd[i].DM_LoHang[j].GiaBanHH = item.GiaBan;
                        i = cthd.length;
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));

            Bind_UpdateHD();
            BindCTHD_byIDRandomHD();
            BindHD_byIDRandom();
        }
        self.ListLot_ofProduct([]);
    }

    self.CheckNewLo = function (type, item) {
        var idRandom = item.IDRandom;
        var malo = $('#AddNewLo' + idRandom).val();
        var nsx = $('#nsx' + idRandom).val();
        var hsd = $('#hd' + idRandom).val();

        if (CheckChar_Special(malo)) {
            ShowMessage_Danger('Mã lô hàng không được chứa kí tự đặc biệt');

            return false;
        }
        if (type == 1) {
            nsx = '';
            hsd = '';
        }
        ajaxHelper(DMHangHoaUri + 'CheckMaLoHangTrung?malohang=' + malo + '&idhanghoa=' + item.ID_HangHoa, 'GET').done(function (x) {
            if (x === true) {
                ShowMessage_Danger('Mã lô hàng đã tồn tại');
            }
            else {
                if (Remove_LastComma(nsx) !== '') {
                    if (Remove_LastComma(hsd) !== '') {
                        let nsxCompare = moment(nsx, 'DD/MM/YYYY').format('YYYYMMDD');
                        let hsdCompare = moment(hsd, 'DD/MM/YYYY').format('YYYYMMDD');
                        if (nsxCompare > hsdCompare) {
                            ShowMessage_Danger('Ngày hết hạn không được bé hơn ngày sản xuất');
                            return false;
                        }
                    }
                }

                // add newlo to cthd
                var cthd = localStorage.getItem(lcCTNhapHang);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].QuanLyTheoLoHang) {
                            for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                let itFor = cthd[i].DM_LoHang[j];
                                if (cthd[i].IDRandom === idRandom) {
                                    cthd[i].MaLoHang = malo;
                                    cthd[i].NgaySanXuat = nsx;
                                    cthd[i].NgayHetHan = hsd;
                                    cthd[i].ID_LoHang = null;
                                }
                                if (itFor.IDRandom === idRandom) {
                                    cthd[i].DM_LoHang[j].MaLoHang = malo;
                                    cthd[i].DM_LoHang[j].NgaySanXuat = nsx;
                                    cthd[i].DM_LoHang[j].NgayHetHan = hsd;
                                    cthd[i].DM_LoHang[j].ID_LoHang = null;
                                    i = cthd.length;
                                    break;
                                }
                            }
                        }
                    }
                    localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));

                    BindCTHD_byIDRandomHD();
                }
            }
        });
    }

    self.ClickNgay_Lo = function (type, item) {
        var idRandom = item.IDRandom;
        var $this = $(event.currentTarget);
        $this.datetimepicker(
            {
                format: "d/m/Y",
                mask: true,
                timepicker: false,
                onChangeDateTime: function (dp, $input) {
                    var ngay = $this.val();
                    var cthd = localStorage.getItem(lcCTNhapHang);
                    if (cthd !== null) {
                        cthd = JSON.parse(cthd);
                        for (let i = 0; i < cthd.length; i++) {
                            if (cthd[i].QuanLyTheoLoHang) {
                                if (cthd[i].IDRandom === idRandom) {
                                    if (type === 1) {
                                        cthd[i].NgaySanXuat = ngay;
                                        $('#nsx' + idRandom).val(ngay);
                                    }
                                    else {
                                        cthd[i].NgayHetHan = ngay;
                                        $('#hd' + idRandom).val(ngay);
                                    }
                                }
                                for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                    if (cthd[i].DM_LoHang[j].IDRandom === idRandom) {
                                        if (type === 1) {
                                            cthd[i].DM_LoHang[j].NgaySanXuat = ngay;
                                            $('#nsx' + idRandom).val(ngay);
                                        }
                                        else {
                                            cthd[i].DM_LoHang[j].NgayHetHan = ngay;
                                            $('#hd' + idRandom).val(ngay);
                                        }
                                        i = cthd.length;
                                        break;
                                    }
                                }
                            }
                        }
                        localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                    }
                }
            });
    }

    self.showpopupbanggia = function (item) {
        self.ID_DonViQuiDoiBG(item.ID_DonViQuiDoi);
        $('#modalpopupbanggia').modal('show');
        ajaxHelper(BH_HoaDonUri + 'getAllGiaBanByIDDVQD?iddvqd=' + item.ID_DonViQuiDoi + '&iddonvi=' + _idDonVi, 'GET').done(function (data) {
            if (self.BangGiaBanOfAll().filter(p => p.ID_DonViQuiDoi == item.ID_DonViQuiDoi).length === 0) {
                var obj = {
                    ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                    lst: data
                }
                self.BangGiaBanOfAll.push(obj);
            }
            var c = $.extend(true, [], self.BangGiaBanOfAll().filter(p => p.ID_DonViQuiDoi === item.ID_DonViQuiDoi)[0].lst);
            self.BangGiaBans(c);

            var gbChung = item.GiaBanHH;
            var exBGChung = false;
            var bgCache = localStorage.getItem('CacheBangGiaNhapHang');
            if (bgCache !== null) {
                bgCache = JSON.parse(bgCache);
                var ex = $.grep(bgCache, function (x) {
                    return x.ID_DonViQuiDoi === self.ID_DonViQuiDoiBG();
                })
                if (ex.length > 0) {
                    // find bgchung
                    var ex2 = $.grep(ex[0].lst, function (x2) {
                        return x2.ID_GiaBan === const_GuidEmpty;
                    });
                    if (ex2.length > 0) {
                        exBGChung = true;
                        gbChung = ex2[0].GiaBan;
                    }
                }
            }

            var bgChung = {
                ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                TenGiaBan: 'Bảng giá chung',
                ID_GiaBan: '00000000-0000-0000-0000-000000000000',
                GiaBan: gbChung,
            }
            if (!exBGChung) {
                self.BangGiaBans.unshift(bgChung);
            }
        });
    }

    self.FillGiaBan = function (item) {
        let gia = formatNumberToInt($('#gb_' + item.ID_GiaBan + '_' + item.ID_DonViQuiDoi).val());
        if (isNaN(gia)) {
            gia = null;
        }
        for (let i = 0; i < self.BangGiaBans().length; i++) {
            if (self.BangGiaBans()[i].ID_GiaBan === item.ID_GiaBan) {
                self.BangGiaBans()[i].GiaBan = gia;
                if (event.keyCode === 13) {
                    // focus next banggia if enter
                    if (i < self.BangGiaBans().length - 1) {
                        $('#gb_' + self.BangGiaBans()[i + 1].ID_GiaBan + '_' + item.ID_DonViQuiDoi).focus().select();
                    }
                }
                break;
            }
        }
    }

    self.LuuTamGiaBanOfTungBangGia = function () {
        self.BangGiaBanOfAll().filter(p => p.ID_DonViQuiDoi === self.ID_DonViQuiDoiBG())[0].lst = self.BangGiaBans();
        localStorage.setItem('CacheBangGiaNhapHang', JSON.stringify(self.BangGiaBanOfAll()));
        $('#modalpopupbanggia').modal('hide');
    }

    self.ClickNhapNhanh_Thuong = function () {
        $(".number-fast").toggle();
        if ($('.number-fast').css('display') === 'block') {
            self.IsNhapNhanh(false);
            ShowMessage_Success("Chế độ nhập thường!");
            $('#txtSoLuongHang').val(1);
        } else {
            self.IsNhapNhanh(true);
            ShowMessage_Success("Chế độ nhập nhanh!");
        }
        $('jqauto-product ._jsInput').focus().select();
    }

    // chiet khau hanghoa + thue
    function Update_TienThue_forCTHD() {
        let idRandomHD = self.newHoaDon().IDRandom();
        let ptThue = self.newHoaDon().TongTienThue() / (self.newHoaDon().TongTienHang()) * 100;
        let ptThueHD = self.newHoaDon().PTThueHoaDon();
        ptThueHD = ptThueHD > 0 ? ptThueHD : 0;

        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandomHD === idRandomHD) {
                    cthd[i].PTThue = ptThueHD;
                    cthd[i].TienThue = ptThue * (cthd[i].DonGia - cthd[i].TienChietKhau) / 100;
                    cthd[i].ThanhToan = cthd[i].SoLuong * (cthd[i].DonGia - cthd[i].TienChietKhau + cthd[i].TienThue);

                    for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                        cthd[i].DM_LoHang[j].PTThue = ptThueHD;
                        cthd[i].DM_LoHang[j].TienThue = ptThue * (cthd[i].DM_LoHang[j].DonGia - cthd[i].DM_LoHang[j].TienChietKhau) / 100;
                        cthd[i].DM_LoHang[j].ThanhToan = cthd[i].DM_LoHang[j].SoLuong *
                            (cthd[i].DM_LoHang[j].DonGia - cthd[i].DM_LoHang[j].TienChietKhau + cthd[i].DM_LoHang[j].TienThue);
                    }
                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                        cthd[i].HangCungLoais[j].PTThue = ptThueHD;
                        cthd[i].HangCungLoais[j].TienThue = ptThue * (cthd[i].HangCungLoais[j].DonGia - cthd[i].HangCungLoais[j].TienChietKhau) / 100;
                        cthd[i].HangCungLoais[j].ThanhToan = cthd[i].HangCungLoais[j].SoLuong
                            * (cthd[i].HangCungLoais[j].DonGia - cthd[i].HangCungLoais[j].TienChietKhau + cthd[i].HangCungLoais[j].TienThue);
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
        }
    }

    function Update_ChietKhau_forCTHD() {
        let idRandomHD = self.newHoaDon().IDRandom();
        let ptCK = self.newHoaDon().TongGiamGiaHang() / self.newHoaDon().TongTienHangChuaCK() * 100;
        let ptCKHH = self.newHoaDon().PTChietKhauHH();
        ptCKHH = ptCKHH > 0 ? ptCKHH : 0;

        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandomHD === idRandomHD) {
                    cthd[i].PTChietKhau = ptCKHH;
                    cthd[i].TienChietKhau = ptCK * cthd[i].DonGia / 100;
                    if (cthd[i].PTThue > 0) {
                        cthd[i].TienThue = cthd[i].PTThue * (cthd[i].DonGia - cthd[i].TienChietKhau) / 100;
                    }
                    cthd[i].ThanhTien = cthd[i].SoLuong * (cthd[i].DonGia - cthd[i].TienChietKhau);
                    cthd[i].ThanhToan = cthd[i].SoLuong * (cthd[i].DonGia - cthd[i].TienChietKhau + cthd[i].TienThue);

                    for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                        cthd[i].DM_LoHang[j].PTChietKhau = ptCKHH;
                        cthd[i].DM_LoHang[j].TienChietKhau = ptCK * cthd[i].DM_LoHang[j].DonGia / 100;
                        if (cthd[i].DM_LoHang[j].PTThue > 0) {
                            cthd[i].DM_LoHang[j].TienThue = cthd[i].DM_LoHang[j].PTThue
                                * (cthd[i].DM_LoHang[j].DonGia - cthd[i].DM_LoHang[j].TienChietKhau) / 100;
                        }
                        cthd[i].DM_LoHang[j].ThanhTien = cthd[i].DM_LoHang[j].SoLuong *
                            (cthd[i].DM_LoHang[j].DonGia - cthd[i].DM_LoHang[j].TienChietKhau);
                        cthd[i].DM_LoHang[j].ThanhToan = cthd[i].DM_LoHang[j].SoLuong *
                            (cthd[i].DM_LoHang[j].DonGia - cthd[i].DM_LoHang[j].TienChietKhau + cthd[i].DM_LoHang[j].TienThue);
                    }
                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                        cthd[i].HangCungLoais[j].PTChietKhau = ptCKHH;
                        cthd[i].HangCungLoais[j].TienChietKhau = ptCK * cthd[i].HangCungLoais[j].DonGia / 100;
                        if (cthd[i].HangCungLoais[j].PTThue > 0) {
                            cthd[i].HangCungLoais[j].TienThue = cthd[i].HangCungLoais[j].PTThue
                                * (cthd[i].HangCungLoais[j].DonGia - cthd[i].HangCungLoais[j].TienChietKhau) / 100;
                        }
                        cthd[i].HangCungLoais[j].ThanhTien = cthd[i].HangCungLoais[j].SoLuong
                            * (cthd[i].HangCungLoais[j].DonGia - cthd[i].HangCungLoais[j].TienChietKhau);
                        cthd[i].HangCungLoais[j].ThanhToan = cthd[i].HangCungLoais[j].SoLuong
                            * (cthd[i].HangCungLoais[j].DonGia - cthd[i].HangCungLoais[j].TienChietKhau + cthd[i].HangCungLoais[j].TienThue);
                    }
                }
            }
            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
        }
    }

    function HD_UpdatePtramThue() {
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].PTThueHoaDon = self.newHoaDon().PTThueHoaDon();
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
    }

    function HD_UpdatePtramChietKhauHhangHoa() {
        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].PTChietKhauHH = self.newHoaDon().PTChietKhauHH();
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
    }

    function CheckNhapMua_fromPO() {
        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            let ctNhapMua = $.grep(cthd, function (x) {
                return x.IDRandomHD === self.newHoaDon().IDRandom() && x.ID_ChiTietGoiDV !== null;
            })
            if (ctNhapMua.length > 0) {
                ShowMessage_Danger('Phiếu nhập từ PO. Vui lòng không thay đổi thông tin');
                return false;
            }
        }
        return true;
    }

    self.ShowDiv_ChietKhauHangHoa = function () {
        let check = CheckNhapMua_fromPO();
        if (!check) {
            return;
        }
        var $thisNext = $(event.currentTarget).next();
        $thisNext.show();
        $thisNext.find('.picked').removeClass('picked');
        if (self.newHoaDon().PTChietKhauHH() > 0 || (self.newHoaDon().TongGiamGiaHang() === 0 && self.newHoaDon().TongGiamGiaHang() === 0)) {
            $('#ckHang_Ptram').addClass("picked");
        }
        else {
            $('#ckHang_Ptram').prev().addClass("picked");
        }
        $(function () {
            $thisNext.children().eq(0).find('input').select().focus();
        })
    }

    self.ChietKhauHangHoa_ClickTaxVND = function () {
        var $this = $(event.currentTarget);
        $this.next().removeClass('picked');

        if (!$this.hasClass('picked')) {
            $this.addClass("picked");
        }
        if (self.newHoaDon().TongGiamGiaHang() > 0) {
            if (self.newHoaDon().PTChietKhauHH() > 0) {
                self.newHoaDon().PTChietKhauHH(0);
            }
            HD_UpdatePtramChietKhauHhangHoa();
        }
        $(function () {
            $this.closest('div').find('input').select().focus();
        })
    }

    self.ChietKhauHangHoa_ClickTaxPtram = function () {
        var $this = $(event.currentTarget);
        $this.prev().removeClass('picked');
        if (!$this.hasClass('picked')) {
            $this.addClass("picked");
        }
        if (self.newHoaDon().TongGiamGiaHang() > 0) {
            let ptCK = self.newHoaDon().TongGiamGiaHang() / self.newHoaDon().TongTienHangChuaCK() * 100;
            self.newHoaDon().PTChietKhauHH(ptCK);
            HD_UpdatePtramChietKhauHhangHoa();
        }
        $(function () {
            $this.closest('div').find('input').select().focus();
        })
    }

    self.ChietKhauHangHoa_Edit = function () {
        var $this = $(event.currentTarget);
        var ptCK = self.newHoaDon().PTChietKhauHH();
        var tongCKHang = 0;

        if ($this.val() === '') {
            ptCK = tongCKHang = 0;
        }
        else {
            if ($('#ckHang_Ptram').hasClass('picked')) {
                // %
                if (formatNumberToFloat($this.val()) > 100) {
                    ptCK = 100;
                    $this.val(100);
                }
                else {
                    ptCK = formatNumberToFloat($this.val());
                }
                tongCKHang = ptCK * self.newHoaDon().TongTienHangChuaCK() / 100;
            }
            else {
                // vnd
                formatNumberObj($this);
                tongCKHang = formatNumberToFloat($this.val());
            }
        }
        self.newHoaDon().TongGiamGiaHang(tongCKHang);
        self.newHoaDon().PTChietKhauHH(ptCK);

        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].PTChietKhauHH = ptCK;
                    hd[i].TongGiamGiaHang = tongCKHang;
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
        Update_ChietKhau_forCTHD();
        Bind_UpdateHD(null, 1);
        BindCTHD_byIDRandomHD();
    }

    self.ShowDivTax_HD = function () {
        let check = CheckNhapMua_fromPO();
        if (!check) {
            return;
        }
        var $thisNext = $(event.currentTarget).next();
        $thisNext.show();
        $thisNext.find('.picked').removeClass('picked')
        if (self.newHoaDon().PTThueHoaDon() > 0 || (self.newHoaDon().PTThueHoaDon() === 0 && self.newHoaDon().TongTienThue() === 0)) {
            $('#hd_TaxPtram').addClass("picked");
        }
        else {
            $('#hd_TaxPtram').prev().addClass("picked");
        }
        $(function () {
            $thisNext.find('input').select().focus();
        })
    }

    self.editTax = function () {
        var $this = $(event.currentTarget);
        var ptThue = self.newHoaDon().PTThueHoaDon();
        var tienThue = self.newHoaDon().TongTienThue();
        var tiensauCK = self.newHoaDon().TongTienHang();

        if ($this.val() === '') {
            ptThue = tienThue = 0;
        }
        else {
            if ($('#hd_TaxPtram').hasClass('picked')) {
                // %
                if (formatNumberToFloat($this.val()) > 100) {
                    ptThue = 100;
                    $this.val(100);
                }
                else {
                    ptThue = formatNumberToFloat($this.val());
                }
                tienThue = ptThue * tiensauCK / 100;
            }
            else {
                // vnd
                formatNumberObj($this);
                tienThue = formatNumberToFloat($this.val());
            }
        }
        self.newHoaDon().TongTienThue(tienThue);
        self.newHoaDon().PTThueHoaDon(ptThue);

        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].PTThueHoaDon = ptThue;
                    hd[i].TongTienThue = tienThue;
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }

        Update_TienThue_forCTHD();// !!importtan update sau khi bind inforHD
        Bind_UpdateHD(null, 2);
        BindCTHD_byIDRandomHD();
    }

    self.HD_ClickTaxVND = function () {
        var $this = $(event.currentTarget);
        $this.next().removeClass('picked');

        if (!$this.hasClass('picked')) {
            $this.addClass("picked");
        }
        if (self.newHoaDon().TongTienHang() > 0) {
            if (self.newHoaDon().PTThueHoaDon() > 0) {
                self.newHoaDon().PTThueHoaDon(0);
            }
            HD_UpdatePtramThue();
        }
        $(function () {
            $this.closest('div').find('input').select().focus();
        })
    }

    self.HD_ClickTaxPtram = function () {
        var $this = $(event.currentTarget);
        $this.prev().removeClass('picked');
        if (!$this.hasClass('picked')) {
            $this.addClass("picked");
        }
        if (self.newHoaDon().TongTienHang() > 0) {
            let ptThue = self.newHoaDon().TongTienThue() / (self.newHoaDon().TongTienHang()) * 100;
            self.newHoaDon().PTThueHoaDon(ptThue);
            HD_UpdatePtramThue();
        }
        $(function () {
            $this.closest('div').find('input').select().focus();
        })
    }

    shortcut.add("F3", function () {
        $('jqauto-product ._jsInput').focus();
    });

    shortcut.add("F4", function () {
        $('#txtAutoDoiTuong').focus();
    });

    shortcut.add("F6", function () {
        self.ClickNhapNhanh_Thuong();
    });

    shortcut.add("F8", function () {
        self.ShowDivSale_HD();
    });

    shortcut.add("F9", function () {
        self.showPayBill();
    });

    shortcut.add("F10", function () {
        self.SaveInvoice(0);
    });

    $('#txtSoLuongHang').keypress(function (e) {
        if (e.keyCode === 13) {
            if (self.IsNhapNhanh() === false) {
                AddCTHD(self.ItemChosing(), formatNumberToFloat($(this).val()));
            }
        }
    })

    // form add new product
    self.ShowForm_NewProduct = function () {
        $('#partialAddProduct').modal('show');
        partialProduct.AddProductSuccess(false);
        partialProduct.ID_NhanVien(_idNhanVien);
        partialProduct.ID_DonVi(_idDonVi);
        partialProduct.UserLogin(_userLogin);
        partialProduct.newProduct(new NewProduct());
        $('.tenNhomHH').text('---Chọn nhóm---');
        $('#divgiavon,#divtonkho,#divCheckLaHH').css('display', 'none');
        $('#divLo').show();
        $('#title-partialproduct').text('Thêm mới hàng hóa');

        var arr = $.grep(self.NhomHangHoas(), function (x) {
            return x.LaNhomHangHoa === true;
        });
        partialProduct.NhomHangHoas_ByKind(arr);
    }

    $('#partialAddProduct').on('hidden.bs.modal', function () {
        if (partialProduct.AddProductSuccess()) {
            var productNew = partialProduct.ProductAfterAdd();
            productNew.GiaNhap = 0;
            productNew.DonGia = 0;
            productNew.DonViTinh = [];
            if (productNew.TenDonViTinh !== '') {
                productNew.DonViTinh = [{ TenDonViTinh: productNew.TenDonViTinh, ID_DonViQuiDoi: productNew.ID_DonViQuiDoi, Xoa: false }];
            }
            productNew.GiaBanHH = productNew.GiaBan;
            productNew.ID_HangHoa = productNew.ID;
            self.ItemChosing(productNew);
            AddCTHD(productNew, 1);
            $('jqauto-product ._jsInput').val(productNew.MaHangHoa);
        }
    })

    self.sLoaiChungTu = ko.computed(function () {
        let txt = 'Nhập hàng nhà cung cấp';
        switch (parseInt(self.newHoaDon().LoaiHoaDon())) {
            case 4:
                txt = 'Nhập hàng nhà cung cấp';
                break;
            case 13:
                txt = 'Nhập kho nội bộ';
                break;
            case 14:
                txt = 'Nhập hàng thừa của khách';
                break;
            case 31:
                txt = 'Đặt hàng nhà cung cấp';
                break;
        }
        return txt;
    })

    self.ChangeLoaiHoaDon = function (item) {
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
        }
        else {
            hd = [];
        }

        let isChangeLoai = false, change14_toOther = false;
        let idRandomHD = self.newHoaDon().IDRandom();
        if (commonStatisJs.CheckNull(idRandomHD)) {
            let obj = CreateNewHoaDon();
            obj.LoaiHoaDon = item.ID;
            hd.push(obj);
            idRandomHD = obj.IDRandom;
            self.newHoaDon().IDRandom(idRandomHD);
        }
        else {
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === idRandomHD) {
                    if (parseInt(hd[i].LoaiHoaDon) !== item.ID) {
                        isChangeLoai = true;
                        // neu loaiHoaDon cu = 14, va chuyen sang loai khac: get again GiaNhap of all HangHoa
                        if (hd[i].LoaiHoaDon === 14) {
                            change14_toOther = true;
                        }
                    }
                    hd[i].LoaiHoaDon = item.ID;
                    break;
                }
            }
        }
        self.newHoaDon().LoaiHoaDon(item.ID);
        localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));

        if (isChangeLoai) {
            nhaphangthua_Reset();

            if (change14_toOther) {
                cthd_getGiaNhapfromDB();
            }
            else {
                BindCTHD_byIDRandomHD();
                BindHD_byIDRandom();
            }
        }
    }

    function nhaphangthua_Reset() {
        let idRandomHD = self.newHoaDon().IDRandom();
        let loaiHoaDon = self.newHoaDon().LoaiHoaDon();
        if (loaiHoaDon === 14) {

            let hd = localStorage.getItem(lcHDNhapHang);
            if (hd !== null) {
                hd = JSON.parse(hd);
                for (let i = 0; i < hd.length; i++) {
                    if (hd[i].IDRandom === idRandomHD) {
                        hd[i].TongTienHangChuaCK = 0;
                        hd[i].TongGiamGiaHang = 0;
                        hd[i].TongTienHang = 0;
                        hd[i].PTThueHoaDon = 0;
                        hd[i].TongTienThue = 0;
                        hd[i].PTChietKhauHH = 0;
                        hd[i].TongGiamGia = 0;
                        hd[i].TongChietKhau = 0;
                        hd[i].PhaiThanhToan = 0;
                        hd[i].DaThanhToan = 0;
                        hd[i].HoanTra = 0;
                        hd[i].ConThieu = 0;
                        hd[i].KhachDaTra = 0;
                        hd[i].TienMat = 0;
                        hd[i].TienPOS = 0;
                        hd[i].TienChuyenKhoan = 0;
                        hd[i].TienDatCoc = 0;
                        hd[i].TongChiPhi = 0;
                        break;
                    }
                }
                localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
            }

            let cthd = localStorage.getItem(lcCTNhapHang);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);

                for (let i = 0; i < cthd.length; i++) {
                    let itFor = cthd[i];
                    if (itFor.IDRandomHD === idRandomHD) {
                        itFor.DonGia = 0;
                        itFor.PTChietKhau = 0;
                        itFor.TienChietKhau = 0;
                        itFor.PTThue = 0;
                        itFor.TienThue = 0;
                        itFor.ThanhTien = 0;
                        itFor.ThanhToan = 0;

                        for (let j = 1; j < cthd[i].DM_LoHang.length; j++) {
                            cthd[i].DM_LoHang[j].DonGia = 0;
                            cthd[i].DM_LoHang[j].PTChietKhau = 0;
                            cthd[i].DM_LoHang[j].TienChietKhau = 0;
                            cthd[i].DM_LoHang[j].PTThue = 0;
                            cthd[i].DM_LoHang[j].ThanhTien = 0;
                            cthd[i].DM_LoHang[j].ThanhToan = 0;
                        }
                        for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                            cthd[i].HangCungLoais[j].DonGia = 0;
                            cthd[i].HangCungLoais[j].PTChietKhau = 0;
                            cthd[i].HangCungLoais[j].TienChietKhau = 0;
                            cthd[i].HangCungLoais[j].PTThue = 0;
                            cthd[i].HangCungLoais[j].ThanhTien = 0;
                            cthd[i].HangCungLoais[j].ThanhToan = 0;
                        }
                    }
                }
                localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
            }
        }
    }

    function cthd_getGiaNhapfromDB() {
        var arrIDQuiDoi = [], arrIDLoHang = [];

        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
        }
        else {
            cthd = [];
        }

        for (let i = 0; i < cthd.length; i++) {
            let itFor = cthd[i];
            arrIDQuiDoi.push(itFor.ID_DonViQuiDoi);
            arrIDLoHang.push(itFor.ID_LoHang);

            for (let j = 1; j < itFor.DM_LoHang.length; j++) {
                let forLot = itFor.DM_LoHang[j];
                arrIDLoHang.push(forLot.ID_LoHang);
            }
        }
        arrIDLoHang = $.grep(arrIDLoHang, function (x) {
            return x !== null;
        })

        var ngayKK = moment($('#datetimepicker_mask').val(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        var obj = {
            ID_ChiNhanh: _idDonVi,
            ToDate: ngayKK,
            ListIDQuyDoi: arrIDQuiDoi,
            ListIDLoHang: arrIDLoHang
        }

        if (arrIDQuiDoi.length > 0) {
            ajaxHelper(DMHangHoaUri + 'GetTonKho_byIDQuyDois', 'POST', obj).done(function (x) {
                if (x.res) {
                    console.log('param ', obj, x.data)
                    for (let i = 0; i < cthd.length; i++) {
                        let forOut = cthd[i];
                        for (let j = 0; j < x.data.length; j++) {
                            let forIn = x.data[j];
                            if (forIn.ID_DonViQuiDoi === forOut.ID_DonViQuiDoi) {
                                if (forOut.QuanLyTheoLoHang === false) {
                                    cthd[i].DonGia = forIn.GiaNhap;
                                    cthd[i].ThanhTien = cthd[i].SoLuong * forIn.GiaNhap;
                                    cthd[i].ThanhToan = cthd[i].ThanhTien;

                                    for (let k = 0; k < forOut.HangCungLoais.length; k++) {
                                        let itLot = forOut.HangCungLoais[k];
                                        cthd[i].HangCungLoais[k].DonGia = forIn.GiaNhap;
                                        cthd[i].HangCungLoais[k].ThanhTien = itLot.SoLuong * forIn.GiaNhap;
                                        cthd[i].HangCungLoais[k].ThanhToan = cthd[i].HangCungLoais[k].ThanhTien;
                                    }
                                    break;
                                }
                                else {
                                    for (let k = 0; k < forOut.DM_LoHang.length; k++) {
                                        let itLot = forOut.DM_LoHang[k];
                                        if (forIn.ID_LoHang === itLot.ID_LoHang) {
                                            cthd[i].DM_LoHang[k].DonGia = forIn.GiaNhap;
                                            cthd[i].DM_LoHang[k].ThanhTien = itLot.SoLuong * forIn.GiaNhap;
                                            cthd[i].DM_LoHang[k].ThanhToan = cthd[i].DM_LoHang[k].ThanhTien;

                                            //update for parent
                                            if (forOut.LotParent && forOut.ID_LoHang === forIn.ID_LoHang) {
                                                cthd[i].DonGia = forIn.GiaNhap;
                                                cthd[i].ThanhTien = cthd[i].SoLuong * forIn.GiaNhap;
                                                cthd[i].ThanhToan = cthd[i].ThanhTien;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));

                    Bind_UpdateHD();
                    BindCTHD_byIDRandomHD();
                    BindHD_byIDRandom();
                }
                else {
                    ShowMessage_Danger(x.mes);
                }
            })
        }
    }

    self.textSearch = ko.observable();
    self.indexFocus = ko.observable(0);
    self.ListNVienSearch = ko.observableArray();

    self.SearchNhanVien = function () {
        var self = this;
        var txt = locdau(self.textSearch());
        var keyCode = event.keyCode;

        if ($.inArray(keyCode, [13, 38, 40]) === -1) {
            let arr = [];
            if (txt === '') {
                // reset nhanvien if clear
                var hd = localStorage.getItem(lcHDNhapHang);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    hd[0].ID_NhanVien = null;
                    localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
                }
                arr = self.NhanViens().slice(0, 20);
                self.ListNVienSearch(arr);
                return;
            }
            arr = $.grep(self.NhanViens(), function (x) {
                x.NameFull = x.MaNhanVien.concat(' ', x.TenNhanVien, ' ', x.SoDienThoai);
                return locdau(x.NameFull).indexOf(txt) > -1;
            })
            self.ListNVienSearch(arr);
        }
        else {
            switch (keyCode) {
                case 13:
                    var chosing = $.grep(self.ListNVienSearch(), function (item, index) {
                        return index === self.indexFocus;
                    });
                    if (chosing.length > 0) {
                        self.ChoseNhanVien(chosing[0]);
                    }
                    break;
                case 38:
                    if (self.indexFocus() < 0) {
                        self.indexFocus(0);
                    }
                    else {
                        self.indexFocus(self.indexFocus() - 1);
                    }
                    break;
                case 40:
                    if (self.indexFocus() > self.ListNVien_BanGoi().length) {
                        self.indexFocus(0);
                    }
                    else {
                        self.indexFocus(self.indexFocus() + 1);
                    }
                    break;
            }
        }
    }

    self.ChoseNhanVien = function (item) {
        self.textSearch(item.TenNhanVien);
        self.newHoaDon().ID_NhanVien(item.ID);

        let exHD = false;
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].ID_NhanVien = item.ID;
                    exHD = true;
                    break;
                }
            }
        }
        else {
            hd = [];
        }

        if (exHD === false) {
            let obj = CreateNewHoaDon();
            hd.push(obj);
            self.newHoaDon().IDRandom(obj.IDRandom);
        }
        localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
    }

    self.editMaHoaDon = function () {
        let $this = $(event.currentTarget);
        let exHD = false;
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].MaHoaDon = $this.val();
                    exHD = true;
                    break;
                }
            }
        }
        else {
            hd = [];
        }

        if (exHD === false) {
            let obj = CreateNewHoaDon();
            obj.MaHoaDon = $this.val()
            hd.push(obj);
            self.newHoaDon().IDRandom(obj.IDRandom);
        }
        localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
    }

    self.ChangeCus = function (item) {
        if (commonStatisJs.CheckNull(item.ID)) {
            $('#lblHideNCC').text('Nhà cung cấp chưa xác định');
        }
        else {
            getChiTietNCCByID(item.ID, false);
            self.newHoaDon().ID_DoiTuong(item.ID);
            $('#lblHideNCC').text('');
        }
    }

    self.OnOffPrint = function () {
        self.turnOnPrint(!self.turnOnPrint())
        localStorage.setItem('CheckInWhenHT', self.turnOnPrint());
    }

    function Check_OnOffPrint() {
        var isPrint = localStorage.getItem('CheckInWhenHT');
        console.log('isPrint ', isPrint)
        if (isPrint !== null) {
            if (isPrint === 'true') {
                self.turnOnPrint(true);
            }
            else {
                self.turnOnPrint(false);
            }
        }
        else {
            self.turnOnPrint(true);
        }
    }

    function getChiTietNCCByID(id, firstLoad = true) {
        ajaxHelper(DMDoiTuongUri + "GetDM_DoiTuong/" + id, 'GET').done(function (data) {
            self.ChiTietDoiTuong(data);
            SetIDDoiTuong_toCacheHD(data.ID);
            GetTienDatCoc(id, firstLoad);
        });
    }

    function GetTienDatCoc(idDoiTuong, firstLoad) {
        ajaxHelper(DMDoiTuongUri + "GetTienDatCoc_ofDoiTuong?idDoiTuong=" + idDoiTuong
            + '&idDonVi=' + _idDonVi, 'GET').done(function (x) {
                if (x.res && x.dataSoure.length > 0) {
                    var soduDatCoc = x.dataSoure[0].SoDuTheGiaTri;
                    soduDatCoc = soduDatCoc < 0 ? 0 : soduDatCoc;
                    self.newHoaDon().SoDuDatCoc(soduDatCoc);
                    var hd = localStorage.getItem(lcHDNhapHang);
                    if (hd !== null) {
                        hd = JSON.parse(hd);
                        for (let i = 0; i < hd.length; i++) {
                            if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                                hd[i].SoDuDatCoc = soduDatCoc;
                                break;
                            }
                        }
                        localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
                    }
                    if (firstLoad === false) {
                        var cthd = localStorage.getItem(lcCTNhapHang);
                        if (cthd !== null) {
                            cthd = JSON.parse(cthd);
                            Bind_UpdateHD(cthd);
                        }
                    }
                }
            });
    }

    self.deleteNCC = function (item) {
        $('jqauto-customer ._jsInput').val('');
        self.ChiTietDoiTuong([]);
        self.newHoaDon().ID_DoiTuong(undefined);
        self.newHoaDon().SoDuDatCoc(0);
        SetIDDoiTuong_toCacheHD(null);

        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd != null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    if (!commonStatisJs.CheckNull(hd[i].ID) && hd[i].ID !== const_GuidEmpty) {
                        hd[i].KhachDaTra = 0;
                        hd[i].HoanTra = 0;
                        hd[i].PhaiThanhToan = hd[i].TongTienHang + hd[i].TongTienThue - hd[i].TongGiamGia + hd[i].TongChiPhi;
                        self.newHoaDon().KhachDaTra(0);
                        self.newHoaDon().HoanTra(0);
                        self.newHoaDon().PhaiThanhToan(0);
                        self.newHoaDon().ConThieu(hd[i].PhaiThanhToan);
                    }
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
    }

    function SetIDDoiTuong_toCacheHD(idNCC) {
        let idRandomHD = self.newHoaDon().IDRandom();

        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd != null) {
            hd = JSON.parse(hd);
        }
        else {
            hd = [];
        }

        if (commonStatisJs.CheckNull(idRandomHD)) {
            let obj = CreateNewHoaDon();
            obj.ID_DoiTuong = idNCC;
            hd.push(obj);
            idRandomHD = obj.IDRandom;
            self.newHoaDon().IDRandom(idRandomHD);
        }
        else {
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === idRandomHD) {
                    if (idNCC !== hd[i].ID_DoiTuong && hd[i].ID != null && hd[i].ID !== const_GuidEmpty) {
                        hd[i].KhachDaTra = 0;
                        hd[i].HoanTra = 0;
                        self.newHoaDon().KhachDaTra(0);
                        self.newHoaDon().HoanTra(0);
                    }
                    hd[i].ID_DoiTuong = idNCC;
                    if (idNCC === null) {
                        hd[i].SoDuDatCoc = 0;
                    }
                    break;
                }
            }
        }
        localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
    }

    $('.txtNgayLapHD').datetimepicker({
        timepicker: true,
        mask: true,
        format: 'd/m/Y H:i',
        maxDate: new Date(),
        onChangeDateTime: function (dp, $input) {
            let ok = CheckNgayLapHD_format($input.val());
            if (ok) {
                let hd = localStorage.getItem(lcHDNhapHang);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                }
                else {
                    hd = [];
                }

                let idRandomHD = self.newHoaDon().IDRandom();
                if (commonStatisJs.CheckNull(idRandomHD)) {
                    let obj = CreateNewHoaDon();
                    obj.NgayLapHoaDon = $input.val();
                    hd.push(obj);
                    idRandomHD = obj.IDRandom;
                    self.newHoaDon().IDRandom(idRandomHD);
                }
                else {
                    for (let i = 0; i < hd.length; i++) {
                        if (hd[i].IDRandom === idRandomHD) {
                            hd[i].NgayLapHoaDon = $input.val();
                            break;
                        }
                    }
                }
                localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
            }
        }
    });

    function CheckNgayLapHD_format(valDate) {

        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        if (valDate === '') {
            ShowMessage_Danger('Vui lòng nhập ngày lập hóa đơn');
            return false;
        }

        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger('Ngày lập hóa đơn chưa đúng định dạng');
            return false;
        }

        if (ngayLapHD > dateNow) {
            ShowMessage_Danger('Ngày lập hóa đơn vượt quá thời gian hiện tại');
            return false;
        }

        let chotSo = VHeader.CheckKhoaSo(moment(ngayLapHD, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'));
        if (chotSo) {
            ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            return false;
        }
        return true;
    }

    // 0. don't edit thue,ck, 1.editCKPr, 2.isEditThuePr, 3.editCK_chitiet, 4.editThue_chitiet
    function Bind_UpdateHD(ct = null, typeEdit = 0) {

        let hd = self.newHoaDon();
        let idRandomHD = hd.IDRandom();
        let ptChietKhauHH = hd.PTChietKhauHH();
        let sum = 0, tongtienchuaCK = 0, tienthue = 0, tongGiamGiaHang = 0;

        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            for (let i = 0; i < cthd.length; i++) {
                let itFor = cthd[i];
                if (itFor.IDRandomHD === idRandomHD) {
                    sum += formatNumberToFloat(cthd[i].ThanhTien);
                    tongtienchuaCK += formatNumberToFloat(itFor.SoLuong) * formatNumberToFloat(itFor.DonGia);
                    tongGiamGiaHang += formatNumberToFloat(itFor.SoLuong) * formatNumberToFloat(itFor.TienChietKhau);
                    tienthue += itFor.TienThue * itFor.SoLuong;

                    for (let j = 1; j < cthd[i].DM_LoHang.length; j++) {
                        sum += cthd[i].DM_LoHang[j].ThanhTien;
                        tienthue += itFor.DM_LoHang[j].TienThue * itFor.DM_LoHang[j].SoLuong;
                        tongtienchuaCK += itFor.DM_LoHang[j].DonGia * itFor.DM_LoHang[j].SoLuong;
                        tongGiamGiaHang += itFor.DM_LoHang[j].TienChietKhau * itFor.DM_LoHang[j].SoLuong;
                    }
                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                        sum += cthd[i].HangCungLoais[j].ThanhTien;
                        tienthue += itFor.HangCungLoais[j].SoLuong * itFor.HangCungLoais[j].TienThue;
                        tongtienchuaCK += itFor.HangCungLoais[j].SoLuong * itFor.HangCungLoais[j].DonGia;
                        tongGiamGiaHang += itFor.HangCungLoais[j].TienChietKhau * itFor.HangCungLoais[j].SoLuong;
                    }
                }
            }
        }

        switch (typeEdit) {
            case 1: // edit ckhanghoa at parent
                self.newHoaDon().TongTienHang(tongtienchuaCK - hd.TongGiamGiaHang());
                if (hd.PTThueHoaDon() > 0) {
                    tienthue = hd.PTThueHoaDon() * (tongtienchuaCK - hd.TongGiamGiaHang()) / 100;
                }
                self.newHoaDon().TongTienThue(tienthue);
                break;
            case 2: // edit thue at parent
                break;
            case 3: // editCK_chitiet
                ptChietKhauHH = 0;
                self.newHoaDon().TongTienHangChuaCK(tongtienchuaCK);
                self.newHoaDon().TongGiamGiaHang(tongGiamGiaHang);
                self.newHoaDon().TongTienHang(sum);
                self.newHoaDon().PTChietKhauHH(0);
                if (hd.PTThueHoaDon() > 0) {
                    tienthue = hd.PTThueHoaDon() * sum / 100;
                }
                self.newHoaDon().TongTienThue(tienthue);
                break;
            case 4: // editThue_chitiet
                self.newHoaDon().PTThueHoaDon(0);
                self.newHoaDon().TongTienHangChuaCK(tongtienchuaCK);
                self.newHoaDon().TongGiamGiaHang(tongGiamGiaHang);
                self.newHoaDon().TongTienHang(sum);
                self.newHoaDon().TongTienThue(tienthue);
                break;
            default:
                self.newHoaDon().TongTienHangChuaCK(tongtienchuaCK);
                self.newHoaDon().TongGiamGiaHang(tongGiamGiaHang);
                self.newHoaDon().TongTienHang(sum);
                self.newHoaDon().TongTienThue(tienthue);
                if (hd.PTThueHoaDon() > 0) {
                    tienthue = hd.PTThueHoaDon() * sum / 100;
                }
                self.newHoaDon().TongTienThue(tienthue);
                break;
        }

        let ggHD = self.newHoaDon().TongGiamGia();
        if (self.newHoaDon().TongChietKhau() > 0) {
            ggHD = (self.newHoaDon().TongTienHang() + tienthue) * self.newHoaDon().TongChietKhau() / 100;
        }

        let chiphi = formatNumberToFloat(hd.TongChiPhi());
        let daTT = self.newHoaDon().DaThanhToan();
        let khachdatra = self.newHoaDon().KhachDaTra();
        khachdatra = khachdatra === undefined ? 0 : khachdatra;
        let datcoc = self.newHoaDon().SoDuDatCoc();
        datcoc = datcoc === undefined ? 0 : datcoc;
        let phaiTT = self.newHoaDon().TongTienHang() + tienthue - ggHD + chiphi;
        if (phaiTT - khachdatra < 0) {
            self.newHoaDon().HoanTra(Math.abs(phaiTT - khachdatra));
        }
        else {
            self.newHoaDon().HoanTra(0);
        }
        self.newHoaDon().PhaiThanhToan(phaiTT);
        self.newHoaDon().TongGiamGia(ggHD);
        self.newHoaDon().TongThanhToan(phaiTT);
        self.newHoaDon().ConThieu(phaiTT - daTT - khachdatra);

        let lstHD = localStorage.getItem(lcHDNhapHang);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
        }
        else {
            lstHD = [];
        }

        for (let i = 0; i < lstHD.length; i++) {
            let itFor = lstHD[i];
            if (itFor.IDRandom === idRandomHD) {
                itFor.TongTienHangChuaCK = self.newHoaDon().TongTienHangChuaCK();
                itFor.TongGiamGiaHang = self.newHoaDon().TongGiamGiaHang();
                itFor.TongTienHang = self.newHoaDon().TongTienHang();
                itFor.PTThueHoaDon = self.newHoaDon().PTThueHoaDon();
                itFor.TongTienThue = tienthue;
                itFor.PTChietKhauHH = ptChietKhauHH;
                itFor.TongGiamGia = self.newHoaDon().TongGiamGia();
                itFor.TongChietKhau = self.newHoaDon().TongChietKhau();
                itFor.PhaiThanhToan = phaiTT;
                itFor.TongThanhToan = phaiTT;
                itFor.DaThanhToan = self.newHoaDon().DaThanhToan();
                itFor.HoanTra = self.newHoaDon().HoanTra();
                itFor.ConThieu = self.newHoaDon().ConThieu();
                break;
            }
        }
        localStorage.setItem(lcHDNhapHang, JSON.stringify(lstHD));
    }

    self.showPopupNCC = function () {
        vmThemMoiNCC.showModalAdd();
    };

    self.showPopupKH = function () {
    };

    self.showPopupEditNCC = function (item) {
        if (CheckQuyenExist('NhaCungCap_CapNhat')) {
            item.TenNhomKhachs = item.NhomDoiTuong;
            vmThemMoiNCC.showModalUpdate(item);
        }
        else {
            ShowMessage_Danger('Không có quyền cập nhật nhà cung cấp');
        }
    };


    function Enable_btnSave() {
        self.isLoadding(false);
    }

    self.SaveInvoice = function (status) {
        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            let idRandom = self.newHoaDon().IDRandom();

            // only get hd current
            cthd = $.grep(cthd, function (x) {
                return x.IDRandomHD === idRandom;
            });

            if (cthd.length > 0) {
                var arrNewLo = [];
                var arrCT = [];
                var err = '';
                vmErr.ListErr = [];

                let dtNow = moment(sstime.GetDatetime()).format('YYYY-MM-DD');
                //if (CheckChar_Special(self.newHoaDon().MaHoaDon())) {
                //    ShowMessage_Danger('Mã hóa đơn không được chứa kí tự đặc biệt');
                //    Enable_btnSave();
                //    return false;
                //}
                // get cthd & get cthd has soluong = 0
                for (let i = 0; i < cthd.length; i++) {
                    let itOut = cthd[i];
                    itOut.SoThuTu = arrCT.length + 1;
                    arrCT.push(itOut);

                    if (formatNumberToFloat(itOut.SoLuong) === 0) {
                        err += itOut.TenHangHoa + ', ';
                    }
                    for (let j = 0; j < itOut.DM_LoHang.length; j++) {
                        let itFor = itOut.DM_LoHang[j];
                        if (itFor.ID_LoHang === null || itFor.ID_LoHang === const_GuidEmpty) {
                            arrNewLo.push(itFor);
                        }
                        if (j !== 0) {
                            itFor.SoThuTu = arrCT.length + 1;
                            arrCT.push(itFor);
                            if (formatNumberToFloat(itFor.SoLuong) === 0) {
                                err += itFor.TenHangHoa + ', ';
                            }
                        }
                        if (!commonStatisJs.CheckNull(itFor.NgayHetHan)
                            && moment(itFor.NgayHetHan, 'DD/MM/YYYY').format('YYYY-MM-DD') < dtNow) {
                            let obj = {
                                TieuDe: itFor.MaHangHoa,
                                NoiDung: ''.concat(itFor.TenHangHoa, ' <b> (Lô: ', itFor.MaLoHang, ') </b> đã quá hạn'),
                            };
                            vmErr.ListErr.push(obj);
                        }
                    }
                    for (let k = 0; k < itOut.HangCungLoais.length; k++) {
                        itOut.HangCungLoais[k].SoThuTu = arrCT.length + 1;
                        arrCT.push(itOut.HangCungLoais[k]);
                        if (formatNumberToFloat(itOut.HangCungLoais[k].SoLuong) === 0) {
                            err += itOut.HangCungLoais[k].TenHangHoa + ', ';
                        }
                    }
                }
                err = Remove_LastComma(err);
                for (let i = 0; i < arrCT.length; i++) {
                    delete arrCT[i]["DM_LoHang"];
                }
                // check MaLo
                var lohangNull = arrNewLo.filter(x => x.MaLoHang === null || x.MaLoHang === '');
                if (lohangNull.length > 0) {
                    ShowMessage_Danger('Vui lòng nhập mã lô hàng');
                    Enable_btnSave();
                    return false;
                }

                if (err !== '') {
                    ShowMessage_Danger('Vui lòng nhập số lượng cho ' + err);
                    Enable_btnSave();
                    return false;
                }

                let checkDate = CheckNgayLapHD_format($('.txtNgayLapHD').val());
                if (!checkDate) {
                    Enable_btnSave();
                    return false;
                }

                let objHD = [];
                var hd = localStorage.getItem(lcHDNhapHang);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    objHD = $.grep(hd, function (x) {
                        return x.IDRandom === idRandom;
                    });
                }

                if (objHD.length === 0) {
                    ShowMessage_Danger('cache hoa don null');
                    Enable_btnSave();
                    return false;
                }

                if (objHD[0].LoaiHoaDon === 13) {
                    // nhập nội bộ, nhưng không chọn NCC --> bắt buộc chọn nhân viên
                    if (commonStatisJs.CheckNull(objHD[0].ID_DoiTuong)) {
                        if (commonStatisJs.CheckNull(objHD[0].ID_NhanVien)) {
                            ShowMessage_Danger('Vui lòng chọn nhân viên cung cấp');
                            Enable_btnSave();
                            return false;
                        }
                    }
                }

                objHD[0].NgayLapHoaDon = GetNgayLapHD_withTimeNow(objHD[0].NgayLapHoaDon);
                objHD[0].ChoThanhToan = status === 0 ? false : true;
                objHD[0].ID_NhanVien = self.newHoaDon().ID_NhanVien();
                objHD[0].MaHoaDon = self.newHoaDon().MaHoaDon();
                objHD[0].DienGiai = self.newHoaDon().DienGiai();

                var arrMaLoNew = [];
                if (arrNewLo.length > 0) {
                    for (let i = 0; i < arrNewLo.length; i++) {
                        let newLo = arrNewLo[i];
                        if (arrMaLoNew.filter(x => x.ID_HangHoa === newLo.ID_HangHoa && x.MaLoHang == newLo.MaLoHang).length > 0) {
                            ShowMessage_Danger('Mã lô ' + newLo.MaLoHang + ' bị trùng lặp');
                            Enable_btnSave();
                            return false;
                        }
                        arrMaLoNew.push(newLo);
                    }
                }
                // return;
                self.isLoadding(true);

                if (arrNewLo.length > 0) {
                    // format ngaysx, hsd
                    for (let i = 0; i < arrNewLo.length; i++) {
                        let newLo = arrNewLo[i];
                        if (newLo.NgaySanXuat !== '') {
                            arrNewLo[i].NgaySanXuat = moment(newLo.NgaySanXuat, 'DD/MM/YYYY').format('YYYY-MM-DD');
                        }
                        if (newLo.NgayHetHan !== '') {
                            arrNewLo[i].NgayHetHan = moment(newLo.NgayHetHan, 'DD/MM/YYYY').format('YYYY-MM-DD');
                        }
                    }
                }

                // show bang baoloi
                if (status === 0 && vmErr.ListErr.length > 0) {
                    let myData = {
                        objHoaDon: objHD[0],
                        objCTHoaDon: arrCT,
                        objLoHang: arrNewLo,
                    }
                    vmErr.showModal(myData);
                }
                else {
                    if (arrNewLo.length > 0) {
                        let myData = {
                            objLoHang: arrNewLo,
                        }

                        ajaxHelper(DMHangHoaUri + 'PostDM_LoHang_AddList', 'post', myData).done(function (x) {
                            if (x.res === true) {
                                // update ID_LoHang to arrCTSave
                                for (let i = 0; i < arrCT.length; i++) {
                                    if (arrCT[i].QuanLyTheoLoHang && arrCT[i].ID_LoHang === null) {
                                        let loDB = $.grep(x.data, function (x) {
                                            return x.MaLoHang === arrCT[i].MaLoHang && x.ID_HangHoa === arrCT[i].ID_HangHoa;
                                        });
                                        if (loDB.length > 0) {
                                            arrCT[i].ID_LoHang = loDB[0].ID;
                                        }
                                    }
                                }
                                SaveHoaDon(objHD[0], arrCT);
                            }
                        });
                    }
                    else {
                        SaveHoaDon(objHD[0], arrCT);
                    }
                }
            }
            else {
                Enable_btnSave();
                ShowMessage_Danger('Vui lòng nhập chi tiết hóa đơn');
            }
        }
        else {
            ShowMessage_Danger('Vui lòng nhập chi tiết hóa đơn');
        }
    }

    self.clickBtnHuyHD = function () {
        if (self.HangHoaAfterAdd() != null && self.HangHoaAfterAdd().length > 0) {
            dialogConfirm('Xác nhận hủy', 'Bạn có chắc chắn muốn hủy hóa đơn đang thực hiện không', function () {
                RemoveCache(self.newHoaDon().IDRandom());
                GotoPageListNhapHang();
            });
        }
        else {
            RemoveCache(self.newHoaDon().IDRandom());
            GotoPageListNhapHang();
        }
    }

    function GotoPageListNhapHang() {
        switch (self.LoaiHoaDonMenu()) {
            case 4:
                window.location.href = '/#/PurchaseOrder';
                break;
            case 31:
                window.location.href = '/#/DatHangNCC';
                break;
            case 13:
                window.location.href = '/#/NhapNoiBo';
                break;
        }
    }

    function GetNgayLapHD_withTimeNow(ngaylapHD) {
        let servertime = sstime.GetDatetime();
        if (commonStatisJs.CheckNull(ngaylapHD)) {
            ngaylapHD = moment(servertime).format('YYYY-MM-DD HH:mm:ss');
        }
        else {
            let giay = servertime.getSeconds();
            let miligiay = servertime.getMilliseconds();
            let ddMMyyyy = ngaylapHD.split('/');
            if (ddMMyyyy.length > 1) {
                ngaylapHD = moment(ngaylapHD, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
            }
            ngaylapHD = moment(ngaylapHD).add(giay, 'seconds').add(miligiay, 'milliseconds').format('YYYY-MM-DD HH:mm:ss');
            // else keep value YYYY-MM-DD
        }
        return ngaylapHD;
    }

    function SaveHoaDon(hd, cthd) {
        var arrBangGia = [];
        // gan bang gia chung if setup
        let sDiaryBGChung = '';
        let cacheBG = localStorage.getItem('CacheBangGiaNhapHang');
        if (cacheBG !== null) {
            cacheBG = JSON.parse(cacheBG);

            let arrQD = [];
            for (let i = 0; i < cacheBG.length; i++) {
                let itFor = cacheBG[i];
                for (let j = 0; j < itFor.lst.length; j++) {
                    if (itFor.lst[j].GiaBan !== null && itFor.lst[j].GiaBan !== "") {
                        arrBangGia.push(itFor.lst[j]);
                    }
                }

                for (let k = 0; k < cthd.length; k++) {
                    if (itFor.ID_DonViQuiDoi === cthd[k].ID_DonViQuiDoi) {
                        let bgchung = $.grep(itFor.lst, function (x) {
                            return x.ID_GiaBan === const_GuidEmpty;
                        });
                        if (bgchung.length > 0 && commonStatisJs.CheckNull(bgchung[0].GiaBan) === false) {
                            cthd[k].GiaBanHH = bgchung[0].GiaBan;

                            if ($.inArray(cthd[k].ID_DonViQuiDoi, arrQD) === -1) {
                                sDiaryBGChung += ' <br /> - '.concat(cthd[k].TenHangHoa, ' (', cthd[k].MaHangHoa,
                                    ') - Giá bán: ', formatNumber3Digit(cthd[k].GiaBanHH));
                                arrQD.push(cthd[k].ID_DonViQuiDoi);
                            }
                            // don't break because 1Hang - nhieulo: update GiaBan all lô
                        }
                    }
                }
            }
        }
        arrBangGia = arrBangGia.filter(x => x.ID_GiaBan !== const_GuidEmpty);

        hd.NguoiTao = _userLogin;
        hd.TenDoiTuong = 'Nhà cung cấp lẻ';
        hd.DiaChiKhachHang = '';
        hd.DienThoaiKhachHang = '';

        if (self.ChiTietDoiTuong() != null && self.ChiTietDoiTuong() !== undefined) {
            hd.TenDoiTuong = self.ChiTietDoiTuong().TenDoiTuong;
            hd.DiaChiKhachHang = self.ChiTietDoiTuong().DiaChi;
            hd.DienThoaiKhachHang = self.ChiTietDoiTuong().DienThoai;

            if (hd.TenDoiTuong === undefined) {
                hd.TenDoiTuong = 'Nhà cung cấp lẻ';
                hd.DiaChiKhachHang = '';
                hd.DienThoaiKhachHang = '';
            }
        }

        let myData = {};
        myData.objHoaDon = hd;
        myData.objCTHoaDon = cthd;
        myData.objBangGia = arrBangGia;
        myData.idNhanVien = _idNhanVien;

        console.log(myData)

        let diaryBG = {};
        if (sDiaryBGChung !== '') {
            diaryBG = {
                ID_DonVi: VHeader.IdDonVi,
                ID_NhanVien: VHeader.IdNhanVien,
                LoaiNhatKy: 1,
                ChucNang: 'Nhập hàng',
                NoiDung: 'Nhập hàng - Cài đặt giá bán',
                NoiDungChiTiet: 'Cài đặt giá bán cho hàng hóa'.concat('<br /> Người tạo: ', VHeader.UserLogin,
                    '<br /> <b> Nội dung chi tiết: </b>', sDiaryBGChung),
            }
        }

        if (myData.objBangGia.length > 0) {
            dialogConfirm_OKCancel('Thông báo', 'Hệ thống sẽ cập nhật bảng giá mới cho những sản phẩm đang nhập. Bạn có chắc chắn muốn lưu không?', function () {
                Insert_UpdatePhieuNhap(myData);
                if (!$.isEmptyObject(diaryBG)) {
                    Insert_NhatKyThaoTac_1Param(diaryBG);
                }
            }, function () {
                myData.objBangGia = null;
                Insert_UpdatePhieuNhap(myData);
                if (!$.isEmptyObject(diaryBG)) {
                    Insert_NhatKyThaoTac_1Param(diaryBG);
                }
            });
        }
        else {
            myData.objBangGia = null;
            Insert_UpdatePhieuNhap(myData);
            if (!$.isEmptyObject(diaryBG)) {
                Insert_NhatKyThaoTac_1Param(diaryBG);
            }
        }
    }

    function ResetInforHD() {
        self.HangHoaAfterAdd([]);
        self.TongSoLuongHH(0);
        self.newHoaDon(new FormModel_NewHoaDon());
        self.ChiTietDoiTuong([]);
        self.BangGiaBanOfAll([]);
        self.textSearch('');
        let arrNV = self.NhanViens().slice(0, 20);
        self.ListNVienSearch(arrNV);
        $('#tenloaitien').text('(Tiền mặt)');
        $('jqauto-customer ._jsInput').val('');
    }

    function RemoveCache(idRandom = null) {
        if (!commonStatisJs.CheckNull(idRandom)) {
            let hd = localStorage.getItem(lcHDNhapHang);
            if (hd !== null) {
                hd = JSON.parse(hd);
                hd = $.grep(hd, function (x) {
                    return x.IDRandom !== idRandom;
                });
                localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
            }
            let cthd = localStorage.getItem(lcCTNhapHang);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = $.grep(cthd, function (x) {
                    return x.IDRandomHD !== idRandom;
                });
                localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
            }
        }
        else {
            localStorage.removeItem(lcHDNhapHang);
            localStorage.removeItem(lcCTNhapHang);
        }

        localStorage.removeItem('CacheBangGiaNhapHang');
    }

    function Insert_UpdatePhieuNhap(myData) {
        var idHoaDon = myData.objHoaDon.ID;
        var phaiTT = formatNumberToFloat(myData.objHoaDon.TongTienHang)
            + formatNumberToFloat(myData.objHoaDon.TongTienThue)
            - formatNumberToFloat(myData.objHoaDon.TongGiamGia)
            + formatNumberToFloat(myData.objHoaDon.TongChiPhi);
        myData.objHoaDon.PhaiThanhToan = phaiTT;
        myData.objHoaDon.TongThanhToan = phaiTT;

        if (idHoaDon !== null && idHoaDon !== undefined && idHoaDon !== const_GuidEmpty) {
            myData.objHoaDon.NguoiSua = VHeader.UserLogin;
            Put_NhapHang(myData);
        }
        else {
            Post_NhapHang(myData);
        }
    }

    function Post_NhapHang(myData) {
        ajaxHelper(BH_HoaDonUri + 'Post_NhapHang', 'post', myData).done(function (x) {
            if (x.res === true) {
                RemoveCache(myData.objHoaDon.IDRandom);
                ShowMessage_Success('Tạo phiếu nhập thành công');

                var dataDB = x.data;
                myData.objHoaDon.MaHoaDon = dataDB.MaHoaDon;
                myData.objHoaDon.ID = dataDB.ID;

                CheckXuLyHet_DonDatHang(myData.objHoaDon.LoaiHoaDon, dataDB.ID, myData.objHoaDon.ID_HoaDon);

                if (myData.objHoaDon.ChoThanhToan === false) {
                    AssignValueHoaDon_ToPhieuThu(myData.objHoaDon);
                    vmThanhToan.SavePhieuThu();
                }

                myData.objHoaDon.MaHoaDon = x.data.MaHoaDon;
                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
                ResetInforHD();
            }
            else {
                ShowMessage_Danger(x.mes);
            }
            Enable_btnSave();
        }).always(function () {
            vmThanhToan.PhieuThuKhach = vmThanhToan.newPhieuThu(2);
        });
    }

    function Put_NhapHang(myData) {
        // update again phaiTT
        var phaiTT = formatNumberToFloat(myData.objHoaDon.TongTienHang) - formatNumberToFloat(myData.objHoaDon.TongGiamGia)
            + formatNumberToFloat(myData.objHoaDon.TongTienThue)
            + formatNumberToFloat(myData.objHoaDon.TongChiPhi);
        myData.objHoaDon.PhaiThanhToan = phaiTT;

        console.log('Update_NhapHang ', myData)
        ajaxHelper(BH_HoaDonUri + 'Update_NhapHang', 'post', myData).done(function (x) {
            if (x.res === true) {
                RemoveCache(myData.objHoaDon.IDRandom);
                ShowMessage_Success('Cập nhật phiếu nhập hàng thành công');

                var dataDB = x.data;
                myData.objHoaDon.MaHoaDon = dataDB.MaHoaDon;
                myData.objHoaDon.ID = dataDB.ID;

                CheckXuLyHet_DonDatHang(myData.objHoaDon.LoaiHoaDon, dataDB.ID, myData.objHoaDon.ID_HoaDon);

                if (myData.objHoaDon.ChoThanhToan === false) {
                    AssignValueHoaDon_ToPhieuThu(myData.objHoaDon);
                    vmThanhToan.SavePhieuThu();
                }

                ResetInforHD();
                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
            }
            else {
                ShowMessage_Danger(x.mes);
            }
            Enable_btnSave();
        }).always(function () {
            vmThanhToan.PhieuThuKhach = vmThanhToan.newPhieuThu(2);
        });
    }

    function CheckXuLyHet_DonDatHang(loaiHoaDon, idHoaDon, idDatHang) {
        if (loaiHoaDon === 4 && !commonStatisJs.CheckNull(idDatHang)) {
            ajaxHelper(BH_HoaDonUri + 'CheckXuLyHet_DonDathang?idHoaDon=' + idHoaDon + '&idDatHang=' + idDatHang, 'GET').done(function (x) {
                if (x === true) {
                    UpdateStatudHD(idDatHang, 3);
                }
                else {
                    UpdateStatudHD(idDatHang, 2);
                }
            });
        }
    }

    function UpdateStatudHD(idDatHang, status) {
        if (!commonStatisJs.CheckNull(idDatHang)) {
            ajaxHelper(BH_HoaDonUri + 'Update_StatusHD?id=' + idDatHang + '&Status=' + status, 'POST').done(function (data) {
                if (data === '') {
                }
            }).fail(function () {
            });
        }
    }

    self.InHoaDon = function (cthd, hd) {
        if (!self.turnOnPrint()) {
            return;
        }
        var cthdFormat = GetCTHDPrint_Format(cthd);
        self.CTHoaDonPrint(cthdFormat);

        var sluongNhap = 0;
        for (var i = 0; i < cthd.length; i++) {
            sluongNhap += formatNumberToFloat(cthd[i].SoLuong);
        }
        var itemHDFormat = GetInforHDPrint(hd);
        itemHDFormat.TongSoLuongHang = sluongNhap;
        self.InforHDprintf(itemHDFormat);

        ajaxHelper('/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + TeamplateMauIn + '&idDonVi=' + _idDonVi, 'GET').done(function (result) {
            var data = '<script src="/Scripts/knockout-3.4.2.js"></script>';
            data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                + "; var item4 =[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
            data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
            PrintExtraReport(result, data, self.numbersPrintHD(), 0);
        });
    }

    function GetCTHDPrint_Format(arrCTHD) {
        for (var i = 0; i < arrCTHD.length; i++) {
            arrCTHD[i].SoThuTu = i + 1;
            arrCTHD[i].TenHangHoa = arrCTHD[i].TenHangHoa.split('(')[0] + (arrCTHD[i].TenDonViTinh !== "" && arrCTHD[i].TenDonViTinh !== null ? "(" + arrCTHD[i].TenDonViTinh + ")" : "") + (arrCTHD[i].ThuocTinh_GiaTri !== null ? arrCTHD[i].ThuocTinh_GiaTri : "") + (arrCTHD[i].MaLoHang !== "" && arrCTHD[i].MaLoHang !== null ? "(Lô: " + arrCTHD[i].MaLoHang + ")" : "");
            arrCTHD[i].DonGia = formatNumber3Digit(arrCTHD[i].DonGia);
            arrCTHD[i].GiaBan = formatNumber3Digit(formatNumberToFloat(arrCTHD[i].DonGia) - formatNumberToFloat(arrCTHD[i].TienChietKhau));
            arrCTHD[i].TienChietKhau = formatNumber3Digit(arrCTHD[i].TienChietKhau);
            arrCTHD[i].SoLuong = formatNumber3Digit(arrCTHD[i].SoLuong);
            arrCTHD[i].ThanhTien = formatNumber3Digit(arrCTHD[i].ThanhTien);
            arrCTHD[i].ThanhToan = formatNumber3Digit(arrCTHD[i].ThanhToan, 2);
            arrCTHD[i].TienThue = formatNumber3Digit(arrCTHD[i].TienThue, 2);
        }
        return arrCTHD;
    }

    function GetInforHDPrint(objHD) {
        var hdPrint = $.extend({}, objHD);
        var daThanhToan = 0;
        var conno = 0;
        var khachdatra = hdPrint.KhachDaTra;
        khachdatra = khachdatra === undefined ? 0 : khachdatra;
        var phaiThanhToan = formatNumberToFloat(hdPrint.PhaiThanhToan);
        if (hdPrint.ChoThanhToan === false) {
            daThanhToan = formatNumberToFloat(vmThanhToan.PhieuThuKhach.DaThanhToan);
        }
        hdPrint.TienMat = formatNumber3Digit(vmThanhToan.PhieuThuKhach.TienMat, 2);
        hdPrint.TienATM = formatNumber3Digit(vmThanhToan.PhieuThuKhach.TienPOS, 2);
        hdPrint.ChuyenKhoan = formatNumber3Digit(vmThanhToan.PhieuThuKhach.TienCK, 2);
        hdPrint.TTBangTienCoc = formatNumber3Digit(vmThanhToan.PhieuThuKhach.TienDatCoc, 2);

        let pthuc = '';
        if (formatNumberToFloat(vmThanhToan.PhieuThuKhach.TienMat) > 0) {
            pthuc = 'Tiền mặt, ';
        }
        if (formatNumberToFloat(vmThanhToan.PhieuThuKhach.TienPOS) > 0) {
            pthuc += 'POS, ';
        }
        if (formatNumberToFloat(vmThanhToan.PhieuThuKhach.TienCK) > 0) {
            pthuc += 'Chuyển khoản, ';
        }
        if (formatNumberToFloat(vmThanhToan.PhieuThuKhach.TienDatCoc) > 0) {
            pthuc += 'Tiền cọc, ';
        }
        hdPrint.PhuongThucTT = Remove_LastComma(pthuc);

        conno = phaiThanhToan - daThanhToan - khachdatra;
        let ngaylap = hdPrint.NgayLapHoaDon;
        hdPrint.NgayLapHoaDon = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
        hdPrint.Ngay = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('DD');
        hdPrint.Thang = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('MM');
        hdPrint.Nam = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('YYYY');
        hdPrint.TongTienHangChuaCK = formatNumber3Digit(hdPrint.TongTienHangChuaCK, 2);
        hdPrint.TongGiamGiaHang = formatNumber3Digit(hdPrint.TongGiamGiaHang, 2);
        hdPrint.TongTienHang = formatNumber3Digit(hdPrint.TongTienHang, 2);
        hdPrint.TongTienThue = formatNumber3Digit(hdPrint.TongTienThue, 2);
        hdPrint.TongThanhToan = formatNumber3Digit(hdPrint.TongThanhToan, 2);
        hdPrint.TongGiamGia = formatNumber3Digit(hdPrint.TongGiamGia, 2);
        hdPrint.PhaiThanhToan = formatNumber3Digit(phaiThanhToan, 2);
        hdPrint.DaThanhToan = formatNumber3Digit(daThanhToan, 2);
        hdPrint.TienBangChu = DocSo(phaiThanhToan);
        hdPrint.NoTruoc = 0;
        hdPrint.NoSau = formatNumber3Digit(conno, 2);
        hdPrint.TongCong = formatNumber3Digit(phaiThanhToan, 2);
        hdPrint.ChiPhiNhap = 0;
        hdPrint.KhuyeMai_GiamGia = 0;

        hdPrint.NguoiTaoHD = _userLogin;
        hdPrint.NhanVienBanHang = $('#selectedNV :selected').text();

        // cong ty, chi nhanh
        hdPrint.TenChiNhanh = _tenDonVi;
        hdPrint.DienThoaiChiNhanh = self.ChiNhanh().SoDienThoai;
        hdPrint.DiaChiChiNhanh = self.ChiNhanh().DiaChi;

        hdPrint.LogoCuaHang = '';
        if (self.CongTy().length > 0) {
            hdPrint.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            hdPrint.TenCuaHang = self.CongTy()[0].TenCongTy;
            hdPrint.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            hdPrint.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }
        return hdPrint;
    }

    // if show confirm but not click OK/Cancel, click out popup
    $('#modalPopuplgDelete').on('hidden.bs.modal', function () {
        Enable_btnSave();
        if (self.HangHoaAfterAdd().length > 0 && self.TongSoLuongHH() === 0) {
            var hd = localStorage.getItem(lcHDNhapHang);
            if (hd !== null) {
                hd = JSON.parse(hd);
                // neu click ben ngoai modal delete --> bind hd
                self.newHoaDon().SetData(hd[0]);
                if (hd[0].ID_DoiTuong !== null && hd[0].ID_DoiTuong !== undefined) {
                    getChiTietNCCByID(hd[0].ID_DoiTuong, false);
                }
                $('#selectedNV').val(hd[0].ID_NhanVien);
                Caculator_AmountProduct();
            }
        }
    });

    self.editTongChiPhi = function () {
        let $this = $(event.currentTarget);
        formatNumberObj($this);

        let idRandomHD = self.newHoaDon().IDRandom();
        let hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === idRandomHD) {
                    hd[i].TongChiPhi = formatNumberToFloat($this.val());
                    self.newHoaDon().TongChiPhi(hd[i].TongChiPhi);
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }

        let cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            Bind_UpdateHD(cthd, 0);
        }
    }

    self.EditDaThanhToan = function () {
        var $this = $(event.currentTarget);
        formatNumberObj($this);
        var phaiTT = self.newHoaDon().TongTienHang()
            + formatNumberToFloat(self.newHoaDon().TongTienThue())
            - self.newHoaDon().TongGiamGia();
        var daTT = formatNumberToFloat($this.val());
        self.newHoaDon().DaThanhToan(formatNumberToFloat($this.val()));

        var conthieu = 0;
        var daTT_coc = self.newHoaDon().KhachDaTra() + self.newHoaDon().SoDuDatCoc();
        if (phaiTT > daTT_coc) {
            conthieu = phaiTT - daTT_coc - daTT;
        }
        else {
            conthieu = self.newHoaDon().HoanTra() - daTT;
        }
        conthieu = RoundDecimal(conthieu, 3)
        self.newHoaDon().ConThieu(conthieu);

        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd != null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].DaThanhToan = self.newHoaDon().DaThanhToan();
                    hd[i].TienMat = self.newHoaDon().DaThanhToan();
                    hd[i].ConThieu = conthieu;
                    hd[i].TienPOS = 0;
                    hd[i].TienChuyenKhoan = 0;
                    hd[i].TienDatCoc = 0;
                    hd[i].ID_TaiKhoanPos = null;
                    hd[i].TenTaiKhoanPos = '';
                    hd[i].ID_TaiKhoanChuyenKhoan = null;
                    hd[i].TenTaiKhoanCK = '';
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
        var tienmat = phaiTT - daTT;
        tienmat = tienmat > 0 ? daTT : phaiTT;
        $('#tenloaitien').html('(Tiền mặt)');

        var hoantraPT = 0;
        if (self.newHoaDon().HoanTra() > 0) {
            hoantraPT = daTT;
        }

        // reset phieuthu (if gara)
        vmThanhToan.PhieuThuKhach.TienMat = tienmat;
        vmThanhToan.PhieuThuKhach.TienPOS = 0;
        vmThanhToan.PhieuThuKhach.TienCK = 0;
        vmThanhToan.PhieuThuKhach.TienDatCoc = 0;
        vmThanhToan.PhieuThuKhach.TienTheGiaTri = 0;
        vmThanhToan.PhieuThuKhach.DaThanhToan = tienmat;
        vmThanhToan.PhieuThuKhach.ID_TaiKhoanPos = null;
        vmThanhToan.PhieuThuKhach.ID_TaiKhoanChuyenKhoan = null;
        vmThanhToan.PhieuThuKhach.HoanTraTamUng = hoantraPT;
    }

    self.EditHoanTra = function () {
        var $this = $(event.currentTarget);
        formatNumberObj($this);
        self.newHoaDon().HoanTra(formatNumberToFloat($this.val()));

        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd != null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].HoanTra = self.newHoaDon().HoanTra();
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
    }

    self.ShowDivSale_HD = function () {
        $('.import-dropbox').hide();
        var $this = $(event.currentTarget);
        $this.next().show();
        $(function () {
            $("#saleNumber").select().focus();
        });

        $this.mouseup(function () {
            return false;
        });
        if (self.newHoaDon().TongChietKhau() > 0 // ptGG
            || (self.newHoaDon().TongGiamGia() === 0 && self.newHoaDon().TongChietKhau() === 0)) {
            $('#vnd').removeClass('picked');
            $('#noVnd').addClass('picked');
        }
        else {
            $('#vnd').addClass('picked');
            $('#noVnd').removeClass('picked');
        }
    }

    self.EditGiamGia_HD = function () {
        var $this = $(event.currentTarget);
        var gtri = formatNumberToFloat($this.val());
        var tongGG = 0;
        if ($('#noVnd').hasClass('picked')) {
            tongGG = gtri * (self.newHoaDon().TongTienHang() + self.newHoaDon().TongTienThue()) / 100;
            self.newHoaDon().TongGiamGia(tongGG);
            self.newHoaDon().TongChietKhau(gtri);
        }
        else {
            tongGG = gtri;
            self.newHoaDon().TongGiamGia(gtri);
            self.newHoaDon().TongChietKhau(0);
        }

        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd != null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].TongGiamGia = self.newHoaDon().TongGiamGia();
                    hd[i].TongChietKhau = self.newHoaDon().TongChietKhau();
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }

        var cthd = localStorage.getItem(lcCTNhapHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            Bind_UpdateHD(cthd, 0);
        }
    }

    self.UpdateGhiChuHD = function () {
        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].DienGiai = $(event.currentTarget).val();
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
    }

    self.clickVND_HD = function () {
        var $this = $(event.currentTarget);
        $this.addClass('picked');
        $('#noVnd').removeClass('picked');
        $(function () {
            $("#saleNumber").select().focus();
        });

        let ptGG = self.newHoaDon().TongChietKhau();
        if (ptGG > 0) {
            self.newHoaDon().TongChietKhau(0);

            let hd = localStorage.getItem(lcHDNhapHang);
            if (hd !== null) {
                hd = JSON.parse(hd);
                for (let i = 0; i < hd.length; i++) {
                    if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                        hd[i].TongChietKhau = 0;
                        break;
                    }
                }
                localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
            }
        }
    }

    self.clickPtram_HD = function () {
        var $this = $(event.currentTarget);
        $this.addClass('picked');
        $('#vnd').removeClass('picked');
        $(function () {
            $("#saleNumber").select().focus();
        });

        var ptGG = self.newHoaDon().TongChietKhau();
        var tongGG = self.newHoaDon().TongGiamGia();
        if (ptGG === 0) {
            ptGG = tongGG / self.newHoaDon().TongTienHang() * 100;
            self.newHoaDon().TongChietKhau(ptGG);
            $("#saleNumber").val(formatNumber3Digit(ptGG));

            let hd = localStorage.getItem(lcHDNhapHang);
            if (hd !== null) {
                hd = JSON.parse(hd);
                for (let i = 0; i < hd.length; i++) {
                    if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                        hd[i].TongChietKhau = ptGG;
                        break;
                    }
                }
                localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
            }
        }
    }

    self.showPayBill = function () {
        vmThanhToan.isGara = self.isGara();
        self.ShowModalThanhToan();
    }

    function AssignValueHoaDon_ToPhieuThu(hd) {
        var cusDoing = self.ChiTietDoiTuong();
        var maKH = 'NCCL';
        var tenKH = 'Nhà Cung Cấp Lẻ';
        var idCus = null;
        if ($.type(cusDoing) === 'object') {
            maKH = cusDoing.MaDoiTuong;
            tenKH = cusDoing.TenDoiTuong;
            idCus = cusDoing.ID;
        }

        var obj = {
            ID: hd.ID,
            LoaiDoiTuong: 2,// 1.kh, 2.ncc, 3.bh
            LoaiHoaDon: hd.LoaiHoaDon,
            MaHoaDon: hd.MaHoaDon,
            HoanTra: hd.HoanTra,
            PhaiThanhToan: hd.PhaiThanhToan,
            TongThanhToan: hd.TongThanhToan,
            ThucThu: hd.DaThanhToan,
            ConNo: 0,
            DienGiai: hd.DienGiai,
            ID_DoiTuong: idCus,
            MaDoiTuong: maKH,
            TenDoiTuong: tenKH,
            ID_KhoanThuChi: hd.ID_KhoanThuChi,
            NgayLapHoaDon: hd.NgayLapHoaDon,
            ID_NhanVien: hd.ID_NhanVien,
            ID_DonVi: hd.ID_DonVi,
            NguoiTao: _userLogin,
            DaThanhToan: hd.DaThanhToan, // used to don't click btnThanhToan
            SoDuDatCoc: formatNumberToFloat(self.newHoaDon().SoDuDatCoc()),
        }
        if (!vmThanhToan.saveOK) {
            // if don't click show modal thanhtoan: set default gtri tien thanhtoan
            let tienmat = 0, tienPOS = 0, tienCK = 0;
            let tiendatcoc = 0, datt = 0, cantt = 0;
            let hoantra = hd.HoanTra;

            if (!commonStatisJs.CheckNull(hd.TienMat)) {
                tienmat = hd.TienMat;
            }
            if (!commonStatisJs.CheckNull(hd.TienPOS)) {
                tienPOS = hd.TienPOS;
            }
            if (!commonStatisJs.CheckNull(hd.TienChuyenKhoan)) {
                tienCK = hd.TienChuyenKhoan;
            }
            if (!commonStatisJs.CheckNull(hd.TienDatCoc)) {
                tiendatcoc = hd.TienDatCoc;
            }

            datt = tienmat + tienPOS + tienCK + tiendatcoc;
            if (datt === 0) {
                if (hoantra > 0) {
                    // todo: có nên tạo phiếu thu hoàn trả tiền??                    
                }
                else {
                    if (formatNumberToFloat(obj.DaThanhToan) > 0) {
                        datt = formatNumberToFloat(obj.DaThanhToan);
                        tienmat = datt;
                    }
                    tiendatcoc = obj.SoDuDatCoc;
                    cantt = hd.PhaiThanhToan - obj.SoDuDatCoc;
                }
                vmThanhToan.PhieuThuKhach.HoanTraTamUng = hoantra;
                vmThanhToan.PhieuThuKhach.PhaiThanhToan = cantt;
            }
            else {
                if (hoantra > 0) {
                    cantt = hoantra;
                    // soducoc > 0, nhung khong hoan tra lai tien
                    if (hd.PhaiThanhToan > datt || obj.SoDuDatCoc > hd.PhaiThanhToan) {
                        vmThanhToan.PhieuThuKhach.TienThua = datt - hd.PhaiThanhToan;
                        vmThanhToan.PhieuThuKhach.HoanTraTamUng = 0;// HoanTraTamUng # inforHoaDon.HoanTra (xảy ra khi số dưu cọc > phải TT, nhưng không sử dụng tiền cọc để TT)
                        vmThanhToan.PhieuThuKhach.PhaiThanhToan = hd.PhaiThanhToan - tiendatcoc;
                    }
                    else {
                        vmThanhToan.PhieuThuKhach.HoanTraTamUng = hoantra;
                        vmThanhToan.PhieuThuKhach.PhaiThanhToan = cantt;
                    }
                }
                else {
                    cantt = hd.PhaiThanhToan - obj.SoDuDatCoc;
                    vmThanhToan.PhieuThuKhach.HoanTraTamUng = hoantra;
                    vmThanhToan.PhieuThuKhach.PhaiThanhToan = cantt;
                }
            }

            vmThanhToan.PhieuThuKhach.TienDatCoc = formatNumber3Digit(tiendatcoc, 2);
            vmThanhToan.PhieuThuKhach.TienMat = formatNumber3Digit(tienmat, 2);
            vmThanhToan.PhieuThuKhach.TienPOS = formatNumber3Digit(tienPOS, 2);
            vmThanhToan.PhieuThuKhach.TienCK = formatNumber3Digit(tienCK, 2);
            vmThanhToan.PhieuThuKhach.ID_TaiKhoanPos = hd.ID_TaiKhoanPos;
            vmThanhToan.PhieuThuKhach.ID_TaiKhoanChuyenKhoan = hd.ID_TaiKhoanChuyenKhoan;
            vmThanhToan.PhieuThuKhach.DaThanhToan = datt;
            vmThanhToan.PhieuThuKhach.TTBangDiem = 0;
        }
        console.log('obj', obj)
        vmThanhToan.inforHoaDon = obj;
    }

    self.ShowModalThanhToan = function () {
        var hd = self.newHoaDon();
        var cusDoing = self.ChiTietDoiTuong();
        var maKH = 'NCCL';
        var tenKH = 'Nhà Cung Cấp Lẻ';
        var idCus = null;
        if ($.type(cusDoing) === 'object') {
            maKH = cusDoing.MaDoiTuong;
            tenKH = cusDoing.TenDoiTuong;
            idCus = cusDoing.ID;
        }

        var phaiTT =
            formatNumberToFloat(hd.TongTienHang())
            - formatNumberToFloat(hd.TongGiamGia())
            + formatNumberToFloat(hd.TongTienThue())
            + formatNumberToFloat(hd.TongChiPhi());

        var obj = {
            IDRandom: self.newHoaDon().IDRandom(),
            MaHoaDon: '',
            LoaiDoiTuong: 2,// 1.kh, 2.ncc, 3.bh
            LoaiHoaDon: hd.LoaiHoaDon(),
            SoDuDatCoc: hd.SoDuDatCoc() ? hd.SoDuDatCoc() : 0,
            HoanTra: hd.HoanTra(),
            KhachDaTra: hd.KhachDaTra(),
            PhaiThanhToan: phaiTT,
            TongThanhToan: hd.TongThanhToan(),
            ThucThu: phaiTT,
            ConNo: 0,
            DienGiai: hd.DienGiai(),
            ID_DoiTuong: idCus,
            MaDoiTuong: maKH,
            TenDoiTuong: tenKH,
            NgayLapHoaDon: hd.NgayLapHoaDon(),
            ID_NhanVien: _idNhanVien,
            ID_DonVi: _idDonVi,
            NguoiTao: _userLogin,
            DaThanhToan: hd.DaThanhToan(), // used to don't click btnThanhToan
            isCheckTraLaiCoc: hd.isCheckTraLaiCoc(),
        }
        vmThanhToan.listData.AccountBanks = self.AllAccountBank();
        vmThanhToan.listData.NhanViens = self.NhanViens();
        vmThanhToan.showModalThanhToan(obj);
    }

    self.AgreePay = function () {
        var phieuThu = vmThanhToan.PhieuThuKhach;
        var tienmat = formatNumberToFloat(phieuThu.TienMat);
        var pos = formatNumberToFloat(phieuThu.TienPOS);
        var ck = formatNumberToFloat(phieuThu.TienCK);
        var loaitien = '';
        if (tienmat > 0) {
            loaitien = 'Tiền mặt, ';
        }
        if (pos > 0) {
            loaitien += 'POS, ';
        }
        if (ck > 0) {
            loaitien += 'Chuyển khoản, ';
        }
        loaitien = Remove_LastComma(loaitien);
        $('#tenloaitien').html('(' + loaitien + ')');

        self.newHoaDon().DaThanhToan(phieuThu.DaThanhToan);
        self.newHoaDon().ConThieu(-phieuThu.TienThua);
        self.newHoaDon().isCheckTraLaiCoc(vmThanhToan.isCheckTraLaiCoc);// used to click again F9

        var hd = localStorage.getItem(lcHDNhapHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].DaThanhToan = self.newHoaDon().DaThanhToan();
                    hd[i].ConThieu = self.newHoaDon().ConThieu();
                    hd[i].TienMat = tienmat;
                    hd[i].TienChuyenKhoan = ck;
                    hd[i].TienPOS = pos;
                    hd[i].TienDatCoc = formatNumberToFloat(phieuThu.TienDatCoc);
                    hd[i].ID_TaiKhoanPos = phieuThu.ID_TaiKhoanPos;
                    hd[i].TenTaiKhoanPos = phieuThu.TenTaiKhoanPos;
                    hd[i].ID_TaiKhoanChuyenKhoan = phieuThu.ID_TaiKhoanChuyenKhoan;
                    hd[i].TenTaiKhoanCK = phieuThu.TenTaiKhoanCK;
                    hd[i].ID_KhoanThuChi = phieuThu.ID_KhoanThuChi;
                    hd[i].isCheckTraLaiCoc = vmThanhToan.isCheckTraLaiCoc;
                    break;
                }
            }
            localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
        }
    }

    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadForm').value = "";
    }

    self.refreshFileSelect = function () {
        self.importNhapHang();
    }

    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        self.loiExcel([]);
    }

    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangNhap.xls";
        window.location.href = url;
    }

    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangNhap.xlsx";
        window.location.href = url;
    }

    self.importNhapHang = function () {
        $('.choose-file').gridLoader({
            style: "top: 120px;"
        });
        let formData = new FormData();
        let totalFiles = document.getElementById("imageUploadForm").files.length;
        for (let i = 0; i < totalFiles; i++) {
            let file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: DMHangHoaUri + "ImfortExcelNhapHang",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                self.loiExcel(item);
                if (self.loiExcel().length > 0) {
                    $(".BangBaoLoi").show();
                    $(".btnImportExcel").hide();
                    $(".refreshFile").show();
                    $(".deleteFile").hide();
                    $('.choose-file').gridLoader({ show: false });
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: DMHangHoaUri + "getList_DanhSachHangNhap?iddonvi=" + _idDonVi,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (item) {
                            self.deleteFileSelect();

                            let hd = localStorage.getItem(lcHDNhapHang);
                            if (hd !== null) {
                                hd = JSON.parse(hd);
                            }
                            else {
                                hd = [];
                            }

                            let idRandomHD = self.newHoaDon().IDRandom();
                            if (commonStatisJs.CheckNull(idRandomHD)) {
                                let obj = CreateNewHoaDon();
                                hd.push(obj);
                                idRandomHD = obj.IDRandom;
                                self.newHoaDon().IDRandom(idRandomHD);
                                localStorage.setItem(lcHDNhapHang, JSON.stringify(hd));
                            }

                            let arrCTsort = item;
                            let arrIDQuiDoi = [];
                            let cthdLoHang = [];
                            for (let i = 0; i < arrCTsort.length; i++) {
                                let ctNew = $.extend({}, arrCTsort[i]);
                                ctNew.IDRandom = CreateIDRandom('CTHD_');
                                ctNew.IDRandomHD = idRandomHD;
                                ctNew.ID_HangHoa = ctNew.ID;
                                ctNew.GiaBanHH = ctNew.GiaBan;
                                ctNew.SoLuongTra = ctNew.SoLuong;
                                ctNew.TonKho = ctNew.TonKho;
                                ctNew.GiaTraLai = ctNew.DonGia;
                                ctNew.LaConCungLoai = false;
                                ctNew.SoLuongConLai = 0;
                                ctNew.GhiChu = '';
                                ctNew.DVTinhGiam = 'VND';
                                ctNew.ID_ChiTietGoiDV = null;
                                if (ctNew.PTThue > 0) {
                                    ctNew.TienThue = (ctNew.DonGia - ctNew.TienChietKhau) * ctNew.PTThue / 100;
                                }
                                ctNew.ThanhToan = ctNew.SoLuong * (ctNew.DonGia - ctNew.TienChietKhau + ctNew.TienThue);

                                let idLoHang = ctNew.ID_LoHang;
                                let quanLiTheoLo = ctNew.QuanLyTheoLoHang;
                                ctNew.QuanLyTheoLoHang = quanLiTheoLo;
                                ctNew.DM_LoHang = [];
                                ctNew.HangCungLoais = [];
                                ctNew.ID_LoHang = idLoHang;
                                ctNew.LotParent = quanLiTheoLo;
                                ctNew.SoThuTu = cthdLoHang.length + 1;

                                let ngaysx = commonStatisJs.CheckNull(arrCTsort[i].NgaySanXuat) ? '' : moment(ctNew.NgaySanXuat).format('DD/MM/YYYY');
                                let hethan = commonStatisJs.CheckNull(arrCTsort[i].NgayHetHan) ? '' : moment(ctNew.NgayHetHan).format('DD/MM/YYYY');
                                ctNew.NgaySanXuat = ngaysx;
                                ctNew.NgayHetHan = hethan;

                                if ($.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                                    arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                                    ctNew.IDRandom = CreateIDRandom('CTHD_');
                                    if (quanLiTheoLo) {
                                        // push DM_Lo
                                        let objLot = $.extend({}, ctNew);
                                        objLot.HangCungLoais = [];
                                        objLot.DM_LoHang = [];
                                        ctNew.DM_LoHang.push(objLot);
                                    }
                                    cthdLoHang.push(ctNew);
                                }
                                else {
                                    for (let j = 0; j < cthdLoHang.length; j++) {
                                        if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                                            if (quanLiTheoLo) {
                                                // push DM_Lo
                                                let objLot = $.extend({}, ctNew);
                                                objLot.LotParent = false;
                                                objLot.HangCungLoais = [];
                                                objLot.DM_LoHang = [];
                                                objLot.IDRandom = CreateIDRandom('RandomCT_');
                                                cthdLoHang[j].DM_LoHang.push(objLot);
                                            }
                                            else {
                                                ctNew.IDRandom = CreateIDRandom('RandomCT_');
                                                ctNew.LaConCungLoai = true;
                                                cthdLoHang[j].HangCungLoais.push(ctNew);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            let cthd = localStorage.getItem(lcCTNhapHang);
                            if (cthd !== null) {
                                cthd = JSON.parse(cthd);
                            }
                            else {
                                cthd = [];
                            }

                            for (let i = 0; i < cthdLoHang.length; i++) {
                                // set giabanHH (if hangcungloai or nhieulo)
                                let gbNew = cthdLoHang[i].GiaBanHH;
                                let exGB = $.grep(arrCTsort, function (x) {
                                    return x.ID_DonViQuiDoi === cthdLoHang[i].ID_DonViQuiDoi && x.ChangeGiaBan === 1;
                                });
                                if (exGB.length > 0) {
                                    gbNew = exGB[0].GiaBan;
                                }

                                cthdLoHang[i].GiaBanHH = gbNew;
                                for (let k = 1; k < cthdLoHang[i].DM_LoHang.length; k++) {
                                    cthdLoHang[i].DM_LoHang[k].GiaBanHH = gbNew;
                                }
                                for (let k = 0; k < cthdLoHang[i].HangCungLoais.length; k++) {
                                    cthdLoHang[i].HangCungLoais[k].GiaBanHH = gbNew;
                                }
                                cthd.unshift(cthdLoHang[i]);
                            }
                            localStorage.setItem(lcCTNhapHang, JSON.stringify(cthd));
                            ShowMessage_Success('Import file thành công')

                            Bind_UpdateHD();
                            BindHD_byIDRandom();
                            BindCTHD_byIDRandomHD();
                        },
                        error: function (x) {
                        },
                        complete: function (jqXHR, textStatus, errorThrown) {
                            $('.choose-file').gridLoader({ show: false });
                        },
                    });
                }
            },
        });
    }

    // paging cthd
    self.CurrentPage = ko.observable(0);
    self.PageSize = ko.observable(1500);
    self.FromItem = ko.observable(0);
    self.ToItem = ko.observable(0);

    self.HangHoaAfterAdd_View = ko.computed(function () {
        let first = self.CurrentPage() * self.PageSize();
        if (self.HangHoaAfterAdd() !== null) {
            let cthd = localStorage.getItem(lcCTNhapHang);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = $.grep(cthd, function (x) {
                    return x.IDRandomHD === self.newHoaDon().IDRandom();
                });
                self.HangHoaAfterAdd(cthd);
            }
            return self.HangHoaAfterAdd().slice(first, first + self.PageSize());
        }
    });

    self.PageCount = ko.computed(function () {
        return Math.ceil(self.LenCTHD() / self.PageSize());
    });

    self.PageListAll = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCount();
        var currentPage = self.CurrentPage();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.CurrentPage()) + 1;
            }
            else {
                i = self.CurrentPage();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumber: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }

        self.FromItem((self.CurrentPage() * self.PageSize()) + 1);
        if (((self.CurrentPage() + 1) * self.PageSize()) > self.HangHoaAfterAdd().length) {
            var fromItem = (self.CurrentPage() + 1) * self.PageSize();
            if (fromItem < self.LenCTHD()) {
                self.ToItem((self.CurrentPage() + 1) * self.PageSize());
            }
            else {
                self.ToItem(self.LenCTHD());
            }
        } else {
            self.ToItem((self.CurrentPage() * self.PageSize()) + self.PageSize());
        }

        return arrPage;
    });

    self.GoToPageAll = function (page) {
        if (page.pageNumber !== '.') {
            self.CurrentPage(page.pageNumber - 1);
        }
    };

    self.GetClassAll = function (page) {
        return ((page.pageNumber - 1) === self.CurrentPage()) ? "click" : "";
    };
    self.VisibleStartPageAll = ko.computed(function () {
        if (self.PageListAll().length > 0) {
            return self.PageListAll()[0].pageNumber !== 1;
        }
    });

    self.VisibleEndPageAll = ko.computed(function () {
        if (self.PageListAll().length > 0) {
            return self.PageListAll()[self.PageListAll().length - 1].pageNumber !== self.PageCount();
        }
    });
    self.StartPageAll = function () {
        self.CurrentPage(0);
    }

    self.BackPageAll = function () {
        if (self.CurrentPage() > 1) {
            self.CurrentPage(self.CurrentPage() - 1);
        }
    }

    self.GoToNextPageAll = function () {
        if (self.CurrentPage() < self.PageCount() - 1) {
            self.CurrentPage(self.CurrentPage() + 1);
        }
    }

    self.EndPageAll = function () {
        if (self.CurrentPage() < self.PageCount() - 1) {
            self.CurrentPage(self.PageCount() - 1);
        }
    }

    $('#ThongTinThanhToanNCC').on('hidden.bs.modal', function () {
        if (vmThanhToan.saveOK) {
            self.AgreePay();
        }
        else {
            // get data from cache (neu truoc do Agree, chi show modal kiemtra lai)
            let hd = localStorage.getItem(lcHDNhapHang);
            if (hd !== null) {
                hd = JSON.parse(hd);
                for (let i = 0; i < hd.length; i++) {
                    if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                        vmThanhToan.PhieuThuKhach.DaThanhToan = hd[i].DaThanhToan;
                        vmThanhToan.PhieuThuKhach.TienMat = hd[i].TienMat;
                        vmThanhToan.PhieuThuKhach.TienPOS = hd[i].TienPOS;
                        vmThanhToan.PhieuThuKhach.TienCK = hd[i].TienChuyenKhoan;
                        vmThanhToan.PhieuThuKhach.TienDatCoc = hd[i].TienDatCoc;
                        break;
                    }
                }
            }
        }
    })

    $('#vmThemMoiNCC').on('hidden.bs.modal', function () {
        if (vmThemMoiNCC.saveOK) {
            self.ChiTietDoiTuong(vmThemMoiNCC.newVendor);
            self.newHoaDon().ID_DoiTuong(vmThemMoiNCC.newVendor.ID);
            $('#txtAutoDoiTuong').val('');
            SetIDDoiTuong_toCacheHD(vmThemMoiNCC.newVendor.ID);
        }
    });

    $('#vmErr').on('hidden.bs.modal', function () {
        if (vmErr.saveOK) {
            let arrNewLo = vmErr.ObjSave.objLoHang;
            let hd = vmErr.ObjSave.objHoaDon;
            let arrCTHD = vmErr.ObjSave.objCTHoaDon;

            if (arrNewLo.length > 0) {
                let myData = {
                    objLoHang: arrNewLo,
                }

                ajaxHelper(DMHangHoaUri + 'PostDM_LoHang_AddList', 'post', myData).done(function (x) {
                    if (x.res === true) {
                        // update ID_LoHang to arrCTSave
                        for (let i = 0; i < arrCTHD.length; i++) {
                            if (arrCTHD[i].QuanLyTheoLoHang && arrCTHD[i].ID_LoHang === null) {
                                let loDB = $.grep(x.data, function (x) {
                                    return x.MaLoHang === arrCTHD[i].MaLoHang && x.ID_HangHoa === arrCTHD[i].ID_HangHoa;
                                });
                                if (loDB.length > 0) {
                                    arrCTHD[i].ID_LoHang = loDB[0].ID;
                                }
                            }
                        }
                        SaveHoaDon(hd, arrCTHD);
                    }
                });
            }
            else {
                SaveHoaDon(hd, arrCTHD);
            }
        }
        else {
            Enable_btnSave();
        }
    })
}

var modelGiaoDich = new NhapHangChiTiet();
ko.applyBindings(modelGiaoDich, document.getElementById('divPage'));
var partialProduct = new PartialAddProduct();
ko.applyBindings(partialProduct, document.getElementById('partial_product_group'));


function jqAutoSelectItem(item) {
    modelGiaoDich.JqAutoSelectItem(item);
}

function keypressEnterSelected(e) {
    if (e.keyCode == 13) {
        modelGiaoDich.JqAutoSelect_Enter();
    }
}

$(window.document).on('shown.bs.modal', '.modal', function () {
    window.setTimeout(function () {
        $('[autofocus]', this).focus();
        $('[autofocus]').select();
    }.bind(this), 100);
});
