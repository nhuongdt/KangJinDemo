var FormModel_NewHoaDon = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaHoaDon = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ID_HoaDon = ko.observable();
    self.ID_NhanVien = ko.observable(VHeader.IdNhanVien);
    self.NguoiTao = ko.observable(VHeader.UserLogin);
    self.ID_PhieuTiepNhan = ko.observable();
    self.MaHoaDonSuaChua = ko.observable();
    self.TongTienHang = ko.observable(0);
    self.NgayLapHoaDon = ko.observable(moment(new Date()).format('DD/MM/YYYY HH:mm'));
    self.DienGiai = ko.observable();
    self.HasTPDinhLuong = ko.observable(false);
    self.MaPhieuTiepNhan = ko.observable();

    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaHoaDon(item.MaHoaDon);
        self.ID_DonVi(item.ID_DonVi);
        self.ID_HoaDon(item.ID_HoaDon);
        self.ID_NhanVien(item.ID_NhanVien);
        self.NguoiTao(item.NguoiTao);
        self.ID_PhieuTiepNhan(item.ID_PhieuTiepNhan);
        self.MaHoaDonSuaChua(item.MaHoaDonSuaChua);
        self.TongTienHang(item.TongTienHang);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.DienGiai(item.DienGiai);
        self.HasTPDinhLuong(item.HasTPDinhLuong);
        let maTN = item.MaPhieuTiepNhan;
        if (commonStatisJs.CheckNull(maTN)) {
            maTN = '';
        }
        self.MaPhieuTiepNhan(maTN);

        if (item.MaHoaDonSuaChua !== '') {
            modelXuatKhoCT.textSearchHDSC(item.MaHoaDonSuaChua);
        }
        else {
            modelXuatKhoCT.textSearchHDSC('');
        }
        if (commonStatisJs.CheckNull(item.BienSo)) {
            modelXuatKhoCT.textSearchPhieuTN('');
        }
        else {
            modelXuatKhoCT.textSearchPhieuTN(maTN.concat('_', item.BienSo));
        }
    }
}

var XuatKhoChiTiet = function () {
    var self = this;
    var BH_XuatHuyUri = '/api/DanhMuc/BH_XuatHuyAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var lcCTXuatKho = 'lcCTXuatKho';
    var lcHDXuatKho = 'lcHDXuatKho';
    var _idDonVi = $('#hd_IDdDonVi').val();
    var _tenDonVi = $('#hd_TenDonVi').val();
    var _idNhanVien = $('.idnhanvien').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var _userLogin = $('#txtTenTaiKhoan').text().trim();

    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();
    $(".BangBaoLoi").hide();

    self.newHoaDon = ko.observable(new FormModel_NewHoaDon());
    self.HangHoaAfterAdd = ko.observableArray();
    self.HangHoas = ko.observableArray();
    self.ListLot_ofProduct = ko.observableArray();
    self.ListLot_ofProductAll = ko.observableArray();
    self.CongTy = ko.observableArray();
    self.ChiNhanh = ko.observableArray();
    self.IsNhapNhanh = ko.observable(true);
    self.ItemChosing = ko.observable();
    self.loiExcel = ko.observableArray();
    self.numbersPrintHD = ko.observable(1);
    self.ConTonKho = ko.observable(0);
    self.CTHoaDonPrint = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    self.ThietLap = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.isLoading = ko.observable(false);

    self.selectedHHXuatKho = ko.observable();
    self.fileNameExcel = ko.observable();
    self.DonVis = ko.observableArray();

    self.TongSoLuongHH = ko.observable(0);
    self.Quyen_NguoiDung = ko.observableArray();
    self.XuatKho_ThayDoiThoiGian = ko.observable();
    self.XuatKho_XacNhanXuat = ko.observable(false);
    self.HangHoa_XemGiaVon = ko.observable();
    modelTypeSearchProduct.TypeSearch(1);// jqAutoProduct

    function PageLoad() {
        UpdateProperties_Undefined();
        GetHT_Quyen_ByNguoiDung();
        Check_QuyenXemGiaVon();
        GetInforCongTy();
        GetListDonVi();
        GetCauHinhHeThong();
        GetAllNhanVien();
        CheckLocTonKho();
    }
    console.log(1)

    PageLoad();

    function UpdateProperties_Undefined() {
        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (commonStatisJs.CheckNull(cthd[i].CssWarning)) {
                    cthd[i].CssWarning = false;
                }
                if (commonStatisJs.CheckNull(cthd[i].ThanhPhan_DinhLuong)) {
                    cthd[i].ThanhPhan_DinhLuong = [];
                    cthd[i].HasTPDinhLuong = false;

                    for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                        if (commonStatisJs.CheckNull(cthd[i].DM_LoHang[j].ThanhPhan_DinhLuong)) {
                            cthd[i].DM_LoHang[j].ThanhPhan_DinhLuong = [];
                            cthd[i].DM_LoHang[j].HasTPDinhLuong = false;
                        }
                    }
                }
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
        }
        var hd = localStorage.getItem(lcHDXuatKho);
        if (hd !== null) {
            hd = JSON.parse(hd);
            if (commonStatisJs.CheckNull(hd[0].HasTPDinhLuong)) {
                hd[0].HasTPDinhLuong = false;
            }
            localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
        }
    }

    function CheckLocTonKho() {
        var tk = localStorage.getItem('xuatkho_isTonKho');
        if (tk !== null) {
            tk = parseInt(tk);
            self.ConTonKho(tk);
        }
        modelTypeSearchProduct.ConTonKho(parseInt(self.ConTonKho()));
    }

    self.ChangeCheckTonKho = function () {
        var tk = parseInt(self.ConTonKho());
        if (tk === 1) {
            self.ConTonKho(0);
        }
        else {
            self.ConTonKho(1);
        }
        localStorage.setItem('xuatkho_isTonKho', self.ConTonKho());
        modelTypeSearchProduct.ConTonKho(self.ConTonKho());
    }

    function CheckSaoChep_EditPhieuNhap() {
        var createHDfrom = localStorage.getItem('XK_createfrom');
        if (createHDfrom !== null) {
            createHDfrom = parseInt(createHDfrom);
            if (createHDfrom === 0) {
                RemoveCache();
                ResetInforHD();
            }
            else {
                var cthd = localStorage.getItem('lcXK_EditOpen');
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);

                    // xuatkho from banlamviec
                    if (createHDfrom === 3 && cthd.length > 0) {
                        self.ChoseHoaDonSC(cthd[0]);
                    }
                    else {
                        for (let i = 0; i < cthd.length; i++) {
                            if (commonStatisJs.CheckNull(cthd[i].ThanhPhan_DinhLuong)) {
                                cthd[i].ThanhPhan_DinhLuong = [];
                                cthd[i].HasTPDinhLuong = false;

                                for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                    if (commonStatisJs.CheckNull(cthd[i].DM_LoHang[j].ThanhPhan_DinhLuong)) {
                                        cthd[i].DM_LoHang[j].ThanhPhan_DinhLuong = [];
                                        cthd[i].DM_LoHang[j].HasTPDinhLuong = false;
                                    }
                                }
                            }
                        }
                        var exTPDL = $.grep(cthd, function (x) {
                            return x.ThanhPhan_DinhLuong.length > 0
                        })
                        var mahoadon = '';
                        var idHoaDon = const_GuidEmpty;
                        var ngaylaphoadon = moment(new Date()).format('DD/MM/YYYY HH:mm');
                        switch (createHDfrom) {
                            case 1:
                                mahoadon = 'Copy' + cthd[0].MaHoaDon;
                                break;
                            case 2:// update phieu XK
                            case 4:
                                mahoadon = cthd[0].MaHoaDon;
                                idHoaDon = cthd[0].ID_HoaDon;
                                ngaylaphoadon = moment(cthd[0].NgayLapHoaDon).format('DD/MM/YYYY HH:mm');
                                break;
                        }
                        var objHD = [{
                            ID: idHoaDon,
                            ID_DonVi: cthd[0].ID_DonVi,
                            ID_NhanVien: cthd[0].ID_NhanVien,
                            ID_HoaDon: cthd[0].ID_HoaDonSC,
                            ID_PhieuTiepNhan: cthd[0].ID_PhieuTiepNhan,
                            MaHoaDonSuaChua: cthd[0].MaHoaDonSuaChua,
                            DienGiai: cthd[0].DienGiai,
                            BienSo: cthd[0].BienSo,// bienso xe
                            NgayLapHoaDon: ngaylaphoadon,
                            TongTienHang: cthd[0].TongTienHang,
                            LoaiHoaDon: 8,
                            MaHoaDon: mahoadon,
                            MaPhieuTiepNhan: cthd[0].MaPhieuTiepNhan,
                            NguoiTao: _userLogin,
                            HasTPDinhLuong: exTPDL.length > 0
                        }];

                        self.newHoaDon().SetData(objHD[0]);
                        localStorage.setItem(lcHDXuatKho, JSON.stringify(objHD));
                        localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));

                        self.HangHoaAfterAdd(cthd);
                        Caculator_AmountProduct();
                    }
                }
            }
        }
        else {
            CheckExistCacheHD();
        }
    }

    function CheckExistCacheHD() {
        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (cthd.length > 0) {
                dialogConfirm_OKCancel('Thông báo', 'Hệ thống tìm được 1 bản nháp chưa được lưu lên máy chủ. Bạn có muốn tiếp tục làm việc với bản nháp này?', function () {
                    let hd = localStorage.getItem(lcHDXuatKho);
                    if (hd !== null) {
                        hd = JSON.parse(hd);

                        if (hd.length > 0) {
                            if (commonStatisJs.CheckNull(hd[0].HasTPDinhLuong)) {
                                hd[0].HasTPDinhLuong = false;
                            }
                            self.HangHoaAfterAdd(cthd);
                            self.newHoaDon().SetData(hd[0]);
                            Caculator_AmountProduct();

                            GetTonKho_byIDQuyDois();
                        }
                        else {
                            RemoveCache();
                            ResetInforHD();
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

    function CheckQuyenExist(maquyen) {
        var role = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === maquyen;
        });
        return role.length > 0;
    }

    function GetHT_Quyen_ByNguoiDung() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.Quyen_NguoiDung(data);
                    self.XuatKho_ThayDoiThoiGian(CheckQuyenExist('XuatHuy_ThayDoiThoiGian'));
                    self.XuatKho_XacNhanXuat(CheckQuyenExist('XuatHuy_XacNhanXuat'));
                }
                else {
                    ShowMessage_Danger('Không có quyền');
                }
            });
        }
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

    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data !== null) {
                self.CongTy(data);
            }
        });
    }

    function GetListDonVi() {
        ajaxHelper("/api/DanhMuc/DM_DonViAPI/" + "GetListDonVi1", 'GET').done(function (data) {
            self.DonVis(data);
        });
    }

    function GetCauHinhHeThong() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + _idDonVi, 'GET').done(function (data) {
                if (data !== null) {
                    self.ThietLap(data);
                }
            });
        }
    }

    function GetAllNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + 'GetNhanVien_NguoiDung', 'GET').done(function (x) {
            if (x.res === true) {
                let lstNV_byDonVi = $.grep(x.data, function (x) {
                    return x.ID_DonVi === _idDonVi;
                });
                self.NhanViens(lstNV_byDonVi);
                self.ListNVienSearch(lstNV_byDonVi.slice(0, 20));

                CheckSaoChep_EditPhieuNhap();

                // find nvlogin
                let nvien = $.grep(self.NhanViens(), function (x) {
                    return x.ID === self.newHoaDon().ID_NhanVien();
                });
                if (nvien.length > 0) {
                    self.textSearch(nvien[0].TenNhanVien);
                }
            }
        })
    }

    function newCTHD(itemHH, soluong) {
        var lotParent = itemHH.QuanLyTheoLoHang ? true : false;
        var ngaysx = moment(itemHH.NgaySanXuat).format('DD/MM/YYYY');
        var hethan = moment(itemHH.NgayHetHan).format('DD/MM/YYYY');
        if (ngaysx === 'Invalid date') {
            ngaysx = '';
        }

        if (hethan === 'Invalid date') {
            hethan = '';
        }
        return {
            ID_HangHoa: itemHH.ID_HangHoa,
            SoThuTu: 1,
            ID_Random: CreateIDRandom('CTHD_'),
            ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
            MaHangHoa: itemHH.MaHangHoa,
            TenHangHoa: itemHH.TenHangHoa,
            TenDonViTinh: itemHH.TenDonViTinh,
            DonViTinh: itemHH.DonViTinh,
            TyLeChuyenDoi: itemHH.TyLeChuyenDoi,
            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
            QuanLyTheoLoHang: itemHH.QuanLyTheoLoHang,
            TonKho: itemHH.TonKho,
            DonGia: itemHH.GiaVon,
            GiaVon: itemHH.GiaVon,
            SoLuong: soluong,
            SoLuongConLai: 0,
            ThanhTien: itemHH.GiaVon * soluong,
            GhiChu: '',
            ID_LoHang: itemHH.ID_LoHang,
            MaLoHang: itemHH.MaLoHang,
            NgaySanXuat: ngaysx,
            NgayHetHan: hethan,
            DM_LoHang: [],
            LotParent: lotParent,
            ID_ChiTietGoiDV: null,
            ThanhPhan_DinhLuong: [],
            HasTPDinhLuong: false,// used to check if hanghoa la TPdinhluong (at gara- hdsc)
            CssWarning: RoundDecimal(itemHH.TonKho, 3) < RoundDecimal(soluong),
        }
    }

    function newLot(itemHH, parent) {
        parent = parent || false;
        let idRandom = parent ? itemHH.ID_Random : CreateIDRandom('CTHD_');
        var ngaysx = '';
        var hethan = '';
        if (itemHH.NgaySanXuat !== null && itemHH.NgaySanXuat !== undefined) {
            ngaysx = itemHH.NgaySanXuat
        }
        if (itemHH.NgayHetHan !== null && itemHH.NgayHetHan !== undefined) {
            hethan = itemHH.NgayHetHan;
        }

        if (ngaysx === 'Invalid date') {
            ngaysx = '';
        }

        if (hethan === 'Invalid date') {
            hethan = '';
        }
        return {
            SoThuTu: 1,
            ID_Random: idRandom,
            ID_HangHoa: itemHH.ID_HangHoa,// used to save DB
            ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
            ID_LoHang: itemHH.ID_LoHang,
            MaLoHang: itemHH.MaLoHang,
            NgaySanXuat: ngaysx,
            NgayHetHan: hethan,
            TonKho: itemHH.TonKho,
            GiaVon: itemHH.GiaVon,
            SoLuong: parent ? itemHH.SoLuong : 1,
            SoLuongConLai: 0,
            ThanhTien: parent ? itemHH.ThanhTien : itemHH.GiaNhap,
            GhiChu: '',
            LotParent: parent ? true : false,
            QuanLyTheoLoHang: true,
            NguoiTao: _userLogin,// used to save DB
            TenHangHoa: itemHH.TenHangHoa,
            MaHangHoa: itemHH.MaHangHoa,
            TenDonViTinh: itemHH.TenDonViTinh,
            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
            ID_ChiTietGoiDV: null,
            ThanhPhan_DinhLuong: [],
            HasTPDinhLuong: false,
            CssWarning: RoundDecimal(itemHH.TonKho, 3) < RoundDecimal(itemHH.SoLuong, 3),
        }
    }

    function FindCTHD_isDoing(item) {
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var idRandom = item.ID_Random;

        var lstCTHD = localStorage.getItem(lcCTXuatKho);
        if (lstCTHD !== null) {
            lstCTHD = JSON.parse(lstCTHD);
            if (quanLiTheoLo) {
                if (item.LotParent) {
                    for (let i = 0; i < lstCTHD.length; i++) {
                        if (lstCTHD[i].ID_Random === idRandom) {
                            return lstCTHD[i];
                        }
                    }
                }
                else {
                    for (let i = 0; i < lstCTHD.length; i++) {
                        for (let j = 0; j < lstCTHD[i].DM_LoHang.length; j++) {
                            if (lstCTHD[i].DM_LoHang[j].ID_Random === idRandom) {
                                return lstCTHD[i].DM_LoHang[j];
                            }
                        }
                    }
                }
            }
            else {
                for (let i = 0; i < lstCTHD.length; i++) {
                    if (lstCTHD[i].ID_Random === idRandom) {
                        return lstCTHD[i];
                    }
                }
            }
        }
        return null;
    }

    function updateCTHDLe(arr, ctDoing, newSoLuong, type) {
        type = type || 0;
        var quanlytheolo = ctDoing.QuanLyTheoLoHang;
        var lotParent = ctDoing.LotParent;
        var idRandom = ctDoing.ID_Random;

        if (lotParent || quanlytheolo === false) {
            for (let i = 0; i < arr.length; i++) {
                if (arr[i].ID_Random === idRandom) {
                    let thanhtien = newSoLuong * arr[i].GiaVon;
                    arr[i].ThanhTien = thanhtien;
                    arr[i].SoLuong = newSoLuong;

                    if (lotParent) {
                        arr[i].DM_LoHang[0].SoLuong = newSoLuong;
                        arr[i].DM_LoHang[0].ThanhTien = thanhtien;
                    }
                    if (type !== 0) {
                        $('#thanhtien_' + idRandom).val(formatNumber3Digit(thanhtien, 2));
                    }
                    break;
                }
            }
        }
        else {
            if (quanlytheolo) {
                for (let i = 0; i < arr.length; i++) {
                    for (let j = 0; j < arr[i].DM_LoHang.length; j++) {
                        if (arr[i].DM_LoHang[j].ID_Random === idRandom) {
                            let thanhtien = arr[i].DM_LoHang[j].GiaVon * newSoLuong;
                            arr[i].DM_LoHang[j].ThanhTien = thanhtien;
                            arr[i].DM_LoHang[j].SoLuong = newSoLuong;
                            i = arr.length;// used to esc out for loop
                            if (type !== 0) {
                                $('#thanhtien_' + idRandom).val(formatNumber3Digit(thanhtien, 2));
                            }
                            break;
                        }
                    }
                }
            }
        }
        return arr;
    }

    function XoaHangHoa_CheckCungLoai(cthd, lotParent, quanlytheolo, idRandom) {
        if (lotParent || quanlytheolo === false) {
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].ID_Random === idRandom) {
                    cthd.splice(i, 1);
                    break;
                }
            }
        }
        else {
            if (quanlytheolo) {
                for (let i = 0; i < cthd.length; i++) {
                    for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                        if (cthd[i].DM_LoHang[j].ID_Random === idRandom) {
                            cthd[i].DM_LoHang.splice(j, 1);
                            i = cthd.length;  // esc for loop
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

    function Caculator_AmountProduct() {
        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            var sumQuantity = 0;
            for (let i = 0; i < cthd.length; i++) {
                sumQuantity += parseFloat(cthd[i].SoLuong);

                // count Lot in Hang hoa
                for (let k = 1; k < cthd[i].DM_LoHang.length; k++) {
                    sumQuantity += parseFloat(cthd[i].DM_LoHang[k].SoLuong);
                }
            }
            // round number to 3 decimals 
            var numberRound = Math.round(sumQuantity * 1000) / 1000;
            self.TongSoLuongHH(numberRound);
        }
    }

    function UpdateAgain_DonViTinhCTHD(idHangHoa, cthd) {
        let cthd_sameIDHangHoa = $.grep(cthd, function (x) {
            return x.ID_HangHoa === idHangHoa;
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
                    arrDVT.push({ ID_DonViQuiDoi: itFor2.ID_DonViQuiDoi, TenDonViTinh: itFor2.TenDonViTinh, Xoa: itFor2.Xoa });
                }
            }
        }

        // update again lst DVT to cthd
        let find = 0;
        for (let i = 0; i < cthd.length; i++) {
            if (cthd[i].ID_HangHoa === idHangHoa) {
                // get arrQuiDoi exist
                let arrIDQuiDoi = [];
                let arrEx = $.grep(cthd_sameIDHangHoa, function (x) {
                    return x.ID_DonViQuiDoi !== cthd[i].ID_DonViQuiDoi;
                });
                for (let k = 0; k < arrEx.length; k++) {
                    arrIDQuiDoi.push(arrEx[k].ID_DonViQuiDoi);
                }
                cthd[i].DonViTinh = $.grep(arrDVT, function (x) {
                    return $.inArray(x.ID_DonViQuiDoi, arrIDQuiDoi) === -1;
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
        $(".BangBaoLoi").hide();

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
                    item.ID_HangHoa = item.ID;
                    self.ItemChosing(item);

                    if (self.IsNhapNhanh()) {
                        AddCTHD(item, 1);
                    }
                    else {
                        $('#txtSoLuongHang').focus();
                    }
                }
            });
    }

    self.JqAutoSelect_Enter = function () {
        if (self.IsNhapNhanh()) {
            let mahh = $('#txtAutoHangHoa1').val();
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByMaHangHoa?mahh=" + mahh + '&iddonvi=' + _idDonVi, 'GET').done(function (data) {
                if (data.length > 0) {
                    data = data.filter(p => p.LaHangHoa === true);
                    if (data.length > 0) {
                        data[0].ID_HangHoa = data[0].ID;
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

    function AddCTHD(item, soluong) {
        var lstCT = localStorage.getItem(lcCTXuatKho);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
        }
        else {
            lstCT = [];
        }

        var itemDoing = null;
        var itemEx = $.grep(lstCT, function (x) {
            return x.ID_DonViQuiDoi === item.ID_DonViQuiDoi;
        });

        //  nếu hàng có định lượng, nhưng click xóa, sau đó chọn lại --> không set lại tpdl cũ, vì có thẻ có nhiều dịch vụ

        if (itemEx.length > 0) {
            if (itemEx[0].ThanhPhan_DinhLuong.length > 1) {
                ShowMessage_Danger('Hàng thuộc định lượng của dịch vụ. Vui lòng thay đổi ở từng dịch vụ')
                return;
            }
            for (let i = 0; i < lstCT.length; i++) {
                if (lstCT[i].ID_DonViQuiDoi === item.ID_DonViQuiDoi) {
                    // check quanlytheolo
                    let itFor = lstCT[i];
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
                                        lstCT[i].ThanhTien = lstCT[i].SoLuong * lstCT[i].GiaVon;
                                    }
                                    lstCT[i].DM_LoHang[j].SoLuong = lstCT[i].DM_LoHang[j].SoLuong + soluong;
                                    lstCT[i].DM_LoHang[j].ThanhTien = lstCT[i].DM_LoHang[j].SoLuong * lstCT[i].DM_LoHang[j].GiaVon;

                                    itemDoing = lstCT[i].DM_LoHang[j];
                                    i = lstCT.length;
                                    break;
                                }
                            }
                        }
                        else {
                            if (!commonStatisJs.CheckNull(item.NgaySanXuat)) {
                                item.NgaySanXuat = moment(item.NgaySanXuat).format('DD/MM/YYYY');
                            }
                            if (!commonStatisJs.CheckNull(item.NgayHetHan)) {
                                item.NgayHetHan = moment(item.NgayHetHan).format('DD/MM/YYYY');
                            }
                            let obj = newLot(item);
                            obj.SoLuong = soluong;
                            obj.ThanhTien = soluong * obj.GiaVon;
                            lstCT[i].DM_LoHang.push(obj);
                            itemDoing = obj;
                        }
                    }
                    else {
                        lstCT[i].SoLuong = itFor.SoLuong + soluong;
                        lstCT[i].ThanhTien = lstCT[i].SoLuong * lstCT[i].GiaVon;
                        itemDoing = lstCT[i];
                    }
                    break;
                }
            }
        }
        else {
            let newCT = newCTHD(item, soluong);
            if (item.QuanLyTheoLoHang) {
                let objLo = newLot(newCT, true);
                newCT.DM_LoHang.push(objLo);
            }
            itemDoing = newCT;
            lstCT.unshift(newCT);
        }

        // update stt
        let stt = 0;
        for (let i = lstCT.length - 1; i >= 0; i--) {
            lstCT[i].SoThuTu = stt;
            stt = stt + 1;
        }
        lstCT = UpdateAgain_DonViTinhCTHD(item.ID_HangHoa, lstCT);
        localStorage.setItem(lcCTXuatKho, JSON.stringify(lstCT));

        UpdateSoLuong_inTPDL(itemDoing);

        self.HangHoaAfterAdd(lstCT);

        UpDate_InforHD(lstCT);
        Caculator_AmountProduct();
        $('#txtAutoHangHoa1').val(item.MaHangHoa).focus().select();
    }

    function Focus_InputTienTraHD() {
        $('#txtPaid').focus().select();
    }

    function Enter_CTHD(itemCT, e, charStart) {
        var key = e.keyCode || e.which;

        if (key === 13) {
            var cthd = JSON.parse(localStorage.getItem(lcCTXuatKho));
            var idRandomFocus = null;
            var lenCTHD = cthd.length;
            for (let i = 0; i < lenCTHD; i++) {
                if (cthd[i].ID_DonViQuiDoi === itemCT.ID_DonViQuiDoi) {
                    if (cthd[i].DM_LoHang.length > 0) {
                        if (cthd[i].ID_Random === itemCT.ID_Random) {
                            if (cthd[i].DM_LoHang.length > 1) {
                                idRandomFocus = cthd[i].DM_LoHang[1].ID_Random;
                            }
                            else {
                                if (i < cthd.length - 1) {
                                    idRandomFocus = cthd[i + 1].ID_Random;
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
                                if (cthd[i].DM_LoHang[j].ID_Random === itemCT.ID_Random) {
                                    if (j < cthd[i].DM_LoHang.length - 1) {
                                        // focus in next Lot
                                        idRandomFocus = cthd[i].DM_LoHang[j + 1].ID_Random;
                                    }
                                    else {
                                        // find li next
                                        if (i < cthd.length - 1) {
                                            // focus in next cthd
                                            idRandomFocus = cthd[i + 1].ID_Random;
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
                            if (cthd[i].ID_Random === itemCT.ID_Random) {
                                // focus in next Lot
                                idRandomFocus = cthd[i + 1].ID_Random;
                            }
                            else {
                                // find in HangCungLoai
                                Focus_InputTienTraHD();
                                return false;
                            }
                        }
                        else {
                            // find HangCungLoai
                            Focus_InputTienTraHD();
                            return false;
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
            var cthd = JSON.parse(localStorage.getItem(lcCTXuatKho));
            var idRandomFocus = null;

            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].ID_DonViQuiDoi === itemCT.ID_DonViQuiDoi) {
                    if (cthd[i].DM_LoHang.length > 0) {
                        if (cthd[i].ID_Random === itemCT.ID_Random) {
                            if (i - 1 >= 0) {
                                let lstLoHang = cthd[i - 1].DM_LoHang;
                                if (lstLoHang.length > 0) {
                                    // focus LoHang last
                                    idRandomFocus = lstLoHang[lstLoHang.length - 1].ID_Random;
                                }
                                else {
                                    // focus in prev li
                                    idRandomFocus = cthd[i - 1].ID_Random;
                                }
                            }
                            else {
                                Focus_InputTienTraHD();
                                return false;
                            }
                        }
                        else {
                            // find LoHang
                            for (let j = 1; j < cthd[i].DM_LoHang.length; j++) {
                                if (cthd[i].DM_LoHang[j].ID_Random === itemCT.ID_Random) {
                                    if (j - 1 === 0) {
                                        idRandomFocus = cthd[i].ID_Random;
                                    }
                                    else {
                                        idRandomFocus = cthd[i].DM_LoHang[j - 1].ID_Random;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    else {
                        if (cthd[i].ID_Random === itemCT.ID_Random) {
                            if (i - 1 >= 0) {
                                let lstLoHang = cthd[i - 1].DM_LoHang;
                                if (lstLoHang.length > 0) {
                                    // focus LoHang last
                                    idRandomFocus = lstLoHang[lstLoHang.length - 1].ID_Random;
                                }
                                else {
                                    idRandomFocus = cthd[i - 1].ID_Random;
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
                    break;
                }
            }
            if (idRandomFocus !== null) {
                $('#' + charStart + idRandomFocus).focus().select();
            }
        }
    }

    self.deleteChiTietHD = function (item) {
        var lstCT = localStorage.getItem(lcCTXuatKho);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
            let dvtDeleted = { ID_DonViQuiDoi: item.ID_DonViQuiDoi, TenDonViTinh: item.TenDonViTinh, Xoa: false };

            lstCT = XoaHangHoa_CheckCungLoai(lstCT, item.LotParent, item.QuanLyTheoLoHang, item.ID_Random);
            // push dvt was delete into cthd (if same ID_HangHoa)
            for (let i = 0; i < lstCT.length; i++) {
                if (lstCT[i].ID_HangHoa === item.ID_HangHoa) {
                    lstCT[i].DonViTinh.push(dvtDeleted);
                }
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(lstCT));

            self.HangHoaAfterAdd(lstCT);

            //if (lstCT.length === 0) {
            //    var hd = localStorage.getItem(lcHDXuatKho);
            //    if (hd !== null) {
            //        hd = JSON.parse(hd);
            //        hd[0].ID_HoaDon = null;
            //        //hd[0].ID_PhieuTiepNhan = null;
            //        hd[0].MaPhieuTiepNhan = null;
            //        //hd[0].MaHoaDonSuaChua = '';
            //        //hd[0].BienSo = '';
            //        localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
            //    }
            //    //RemoveCache();
            //}
            UpDate_InforHD(lstCT);
            Caculator_AmountProduct();
        }
    }

    function CTHD_UpdateWarning(ctDoing) {
        if (ctDoing !== null) {
            let quanlytheolo = ctDoing.QuanLyTheoLoHang;
            let lotParent = ctDoing.LotParent;
            let idRandom = ctDoing.ID_Random;

            var cthd = localStorage.getItem(lcCTXuatKho);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);

                if (lotParent || quanlytheolo === false) {
                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].ID_Random === idRandom) {
                            cthd[i].CssWarning = RoundDecimal(formatNumberToFloat(cthd[i].SoLuong)) > RoundDecimal(formatNumberToFloat(cthd[i].TonKho));
                            break;
                        }
                    }
                }
                else {
                    if (quanlytheolo) {
                        for (let i = 0; i < cthd.length; i++) {
                            for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                if (cthd[i].DM_LoHang[j].ID_Random === idRandom) {
                                    cthd[i].DM_LoHang[j].CssWarning = RoundDecimal(formatNumberToFloat(cthd[i].DM_LoHang[j].SoLuong))
                                        > RoundDecimal(formatNumberToFloat(cthd[i].DM_LoHang[j].TonKho));
                                    break;
                                }
                            }
                        }
                    }
                }
                localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
            }
        }
    }

    function UpdateSoLuong_inTPDL(ctDoing) {
        if (ctDoing !== null) {
            var quanlytheolo = ctDoing.QuanLyTheoLoHang;
            var lotParent = ctDoing.LotParent;
            var idRandom = ctDoing.ID_Random;

            var cthd = localStorage.getItem(lcCTXuatKho);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);

                if (lotParent || quanlytheolo === false) {
                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].ID_Random === idRandom) {
                            if (cthd[i].ThanhPhan_DinhLuong.length === 1) {
                                cthd[i].ThanhPhan_DinhLuong[0].SoLuong = cthd[i].SoLuong;
                                cthd[i].ThanhPhan_DinhLuong[0].ThanhTien = cthd[i].SoLuong * cthd[i].GiaVon;
                            }
                            break;
                        }
                    }
                }
                else {
                    if (quanlytheolo) {
                        for (let i = 0; i < cthd.length; i++) {
                            for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                if (cthd[i].DM_LoHang[j].ID_Random === idRandom) {
                                    if (cthd[i].DM_LoHang[j].ThanhPhan_DinhLuong.length === 1) {
                                        cthd[i].DM_LoHang[j].ThanhPhan_DinhLuong[0].SoLuong = cthd[i].DM_LoHang[j].SoLuong;
                                        cthd[i].DM_LoHang[j].ThanhPhan_DinhLuong[0].ThanhTien = cthd[i].DM_LoHang[j].SoLuong * cthd[i].DM_LoHang[j].GiaVon;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
            }
        }
    }

    self.EditSoLuong = function (item) {
        if (item.ThanhPhan_DinhLuong.length > 1) {
            ShowMessage_Danger('Hàng thuộc định lượng của dịch vụ. Vui lòng thay đổi ở từng dịch vụ');
            let ctDoing = FindCTHD_isDoing(item);
            if (ctDoing !== null) {
                $('#soluong_' + ctDoing.ID_Random).val(formatNumber3Digit(ctDoing.SoLuong));
            }
            return false;
        }
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);
        var soluong = formatNumberToFloat($(thisObj).val());
        var keyCode = event.keyCode || event.which;
        switch (keyCode) {
            case 38:// up
                soluong = soluong + 1;
                $(thisObj).val(formatNumber3Digit(soluong));
                break;
            case 40:// down
                if (soluong > 0) {
                    soluong = soluong - 1;
                }
                $(thisObj).val(formatNumber3Digit(soluong));
                break;
        }

        if (item.ID_ChiTietGoiDV !== null && item.ID_ChiTietGoiDV !== undefined) {
            if (item.SoLuongConLai < soluong) {
                ShowMessage_Danger('Chỉ được nhập tối đa ' + item.SoLuongConLai);
                return false;
            }
        }

        var lstCT = localStorage.getItem(lcCTXuatKho);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
            lstCT = updateCTHDLe(lstCT, item, soluong, 1);
            localStorage.setItem(lcCTXuatKho, JSON.stringify(lstCT));
            Caculator_AmountProduct();
            UpDate_InforHD(lstCT);
            UpdateSoLuong_inTPDL(item);
            CTHD_UpdateWarning(item)
            Enter_CTHD(item, event, 'soluong_');
            Shift_CTHD(item, event, 'soluong_');
        }
    }

    self.TangSoLuong = function (item) {
        var input = $(event.currentTarget).parent().find('input');
        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing.ThanhPhan_DinhLuong.length > 1) {
            ShowMessage_Danger('Hàng thuộc định lượng của dịch vụ. Vui lòng thay đổi ở từng dịch vụ')
            return false;
        }

        var soluong = ctDoing.SoLuong + 1;
        if (item.ID_ChiTietGoiDV !== null && item.ID_ChiTietGoiDV !== undefined) {
            if (item.SoLuongConLai < soluong) {
                ShowMessage_Danger('Chỉ được nhập tối đa ' + item.SoLuongConLai);
                return false;
            }
        }

        $(input).val(formatNumber3Digit(soluong));
        var lstCT = localStorage.getItem(lcCTXuatKho);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
            lstCT = updateCTHDLe(lstCT, item, soluong, 1);
            localStorage.setItem(lcCTXuatKho, JSON.stringify(lstCT));
            Caculator_AmountProduct();
            UpDate_InforHD(lstCT);
            UpdateSoLuong_inTPDL(item);
            CTHD_UpdateWarning(item);
            Enter_CTHD(item, event, 'soluong_');
            Shift_CTHD(item, event, 'soluong_');
        }
    }

    self.GiamSoLuong = function (item) {
        var input = $(event.currentTarget).parent().find('input');
        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing !== null) {
            if (ctDoing.ThanhPhan_DinhLuong.length > 1) {
                ShowMessage_Danger('Hàng thuộc định lượng của dịch vụ. Vui lòng thay đổi ở từng dịch vụ')
                return false;
            }
            if (ctDoing.SoLuong > 0) {
                var soluong = ctDoing.SoLuong - 1;
                $(input).val(formatNumber3Digit(soluong));
                var lstCT = localStorage.getItem(lcCTXuatKho);
                if (lstCT !== null) {
                    lstCT = JSON.parse(lstCT);
                    lstCT = updateCTHDLe(lstCT, item, soluong, 1);
                    localStorage.setItem(lcCTXuatKho, JSON.stringify(lstCT));
                    Caculator_AmountProduct();
                    UpDate_InforHD(lstCT);
                    UpdateSoLuong_inTPDL(item);
                    CTHD_UpdateWarning(item)
                    Enter_CTHD(item, event, 'soluong_');
                    Shift_CTHD(item, event, 'soluong_');
                }
            }
        }
    }

    self.EditGiaVon = function (item) {
        var thisObj = event.currentTarget;
        var idRandom = item.ID_Random;
        var quanlytheolo = item.QuanLyTheoLoHang;
        var lotParent = item.LotParent;
        formatNumberObj(thisObj);
        var gianhap = formatNumberToFloat($(thisObj).val());
        var newSoLuong = formatNumberToFloat($('#soluong_' + idRandom).val());
        let thanhtien = newSoLuong * gianhap;

        var arr = localStorage.getItem(lcCTXuatKho);
        if (arr !== null) {
            arr = JSON.parse(arr);
            if (lotParent || quanlytheolo === false) {
                for (let i = 0; i < arr.length; i++) {
                    if (arr[i].ID_Random === idRandom) {
                        arr[i].GiaVon = gianhap;
                        arr[i].ThanhTien = thanhtien;

                        if (lotParent) {
                            arr[i].DM_LoHang[0].GiaVon = gianhap;
                            arr[i].DM_LoHang[0].ThanhTien = thanhtien;
                        }
                        $('#thanhtien_' + idRandom).val(formatNumber3Digit(thanhtien, 2));
                        break;
                    }
                }
            }
            else {
                if (quanlytheolo) {
                    for (let i = 0; i < arr.length; i++) {
                        for (let j = 0; j < arr[i].DM_LoHang.length; j++) {
                            if (arr[i].DM_LoHang[j].ID_Random === idRandom) {
                                arr[i].DM_LoHang[j].GiaVon = gianhap;
                                arr[i].DM_LoHang[j].ThanhTien = thanhtien;
                                i = arr.length;// used to esc out for loop
                                $('#thanhtien_' + idRandom).val(formatNumber3Digit(thanhtien, 2));
                                break;
                            }
                        }
                    }
                }
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(arr));
            UpDate_InforHD(arr);
        }
    }

    self.UpdateGhiChu_CTHD = function (item) {
        var idRandom = item.ID_Random;
        var quanlyTheoLo = item.QuanLyTheoLoHang;
        var ghichu = $(event.currentTarget).val();

        var lcCTHD = localStorage.getItem(lcCTXuatKho);
        if (lcCTHD !== null) {
            lcCTHD = JSON.parse(lcCTHD);
            if (item.LotParent || quanlyTheoLo === false) {
                for (let i = 0; i < lcCTHD.length; i++) {
                    if (lcCTHD[i].ID_Random === idRandom) {
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
                            if (lcCTHD[i].DM_LoHang[j].ID_Random === idRandom) {
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
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(lcCTHD));
        }
    }

    self.ChangeDonViTinh = function (item, parent) {
        var newIDQuiDoi = item.ID_DonViQuiDoi;
        var oldIDQuiDoi = parent.ID_DonViQuiDoi;

        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            let dvtOld = { ID_DonViQuiDoi: oldIDQuiDoi, TenDonViTinh: parent.TenDonViTinh, Xoa: false };

            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].ID_HangHoa === parent.ID_HangHoa) {
                    if (cthd[i].ID_DonViQuiDoi !== oldIDQuiDoi) {
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
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].ID_DonViQuiDoi === oldIDQuiDoi) {
                        cthd[i].MaHangHoa = mahang;
                        cthd[i].DonGia = gianhap;
                        cthd[i].GiaVon = giavon;
                        cthd[i].TonKho = tonkho;
                        cthd[i].ThanhTien = giavon * cthd[i].SoLuong;
                        cthd[i].TenDonViTinh = item.TenDonViTinh;
                        cthd[i].ID_DonViQuiDoi = newIDQuiDoi;

                        for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                            cthd[i].DM_LoHang[j].ID_DonViQuiDoi = newIDQuiDoi;
                            cthd[i].DM_LoHang[j].TenDonViTinh = item.TenDonViTinh;
                            cthd[i].DM_LoHang[j].MaHangHoa = mahang;
                            cthd[i].DM_LoHang[j].DonGia = gianhap;
                            cthd[i].DM_LoHang[j].GiaVon = giavon;
                            cthd[i].DM_LoHang[j].TonKho = tonkho;
                            cthd[i].DM_LoHang[j].ThanhTien = gianhap * cthd[i].DM_LoHang[j].SoLuong;
                            cthd[i].DM_LoHang[j].PTChietKhau = 0;
                            cthd[i].DM_LoHang[j].TienChietKhau = 0;
                            cthd[i].DM_LoHang[j].DVTinhGiam = '%';
                        }
                        break;
                    }
                }
                localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
                self.HangHoaAfterAdd(cthd);
                UpDate_InforHD(cthd);
            });
        }
    }

    self.ClickThemLo = function (item) {
        var quanlytheolo = item.QuanLyTheoLoHang;
        var idQuiDoi = item.ID_DonViQuiDoi;

        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (quanlytheolo) {
                // add lo
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].ID_DonViQuiDoi === idQuiDoi) {
                        let obj = newLot(cthd[i]);
                        obj.ID_LoHang = null;
                        obj.MaLoHang = '';
                        obj.NgaySanXuat = '';
                        obj.NgayHetHan = '';
                        obj.DonGia = cthd[i].DonGia;
                        obj.ThanhTien = cthd[i].GiaVon;
                        cthd[i].DM_LoHang.push(obj);
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
            self.HangHoaAfterAdd(cthd);
            UpDate_InforHD(cthd);
            Caculator_AmountProduct();
        }
    }

    self.ResetLo = function (item) {
        var thisObj = $(event.currentTarget);
        var idRandom = item.ID_Random;
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var lotParent = item.LotParent;

        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (lotParent || quanLiTheoLo === false) {
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].ID_Random === idRandom) {
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
                            if (cthd[i].DM_LoHang[j].ID_Random === idRandom) {
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
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
        }
        thisObj.closest('.op-js-xuatkho-lo').find('input').val('');// empty malo
        thisObj.closest('.op-js-xuatkho-lo').find('.op-js-xuatkho-hanlo input').show();// show input ngaysx, hethan
        thisObj.closest('.op-js-xuatkho-lo').find('.op-js-xuatkho-hanlo div').hide();// hide div ngaysx, hethan
    }

    self.LoadListLoHang = function (item) {
        ajaxHelper(DMHangHoaUri + 'GetInforProduct_ByIDQuidoi?idQuiDoi=' + item.ID_DonViQuiDoi + '&idChiNhanh=' + _idDonVi).done(function (x) {
            if (x.res === true) {
                var lst = x.data;
                self.ListLot_ofProduct(lst.filter(x => x.ID_LoHang !== null));
                self.ListLot_ofProductAll(self.ListLot_ofProduct());
            }
        });
    }

    self.SearchLoHang = function (item) {
        var malo = $('#AddNewLo' + item.ID_Random).val();
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
        var idRandom = $($this.closest('.op-js-xuatkho-malo')).find('span').eq(0).attr('id');

        // update TonKho, GiaVon, GiaBan, Lo
        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                    let itFor = cthd[i].DM_LoHang[j];
                    // if parent
                    if (cthd[i].ID_Random === idRandom) {
                        cthd[i].ID_LoHang = idLoHang;
                        cthd[i].MaLoHang = maLoHang;
                        cthd[i].NgaySanXuat = ngaysx;
                        cthd[i].NgayHetHan = hethan;
                        cthd[i].DonGia = item.GiaNhap;
                        cthd[i].ThanhTien = item.GiaVon * cthd[i].SoLuong;
                        cthd[i].GiaVon = item.GiaVon;
                        cthd[i].TonKho = item.TonKho;
                        cthd[i].CssWarning = RoundDecimal(item.TonKho) < RoundDecimal(cthd[i].SoLuong);
                    }
                    if (itFor.ID_Random === idRandom) {
                        cthd[i].DM_LoHang[j].ID_LoHang = idLoHang;
                        cthd[i].DM_LoHang[j].MaLoHang = maLoHang;
                        cthd[i].DM_LoHang[j].NgaySanXuat = ngaysx;
                        cthd[i].DM_LoHang[j].NgayHetHan = hethan;
                        cthd[i].DM_LoHang[j].DonGia = item.GiaNhap;
                        cthd[i].DM_LoHang[j].ThanhTien = item.GiaVon * cthd[i].DM_LoHang[j].SoLuong;
                        cthd[i].DM_LoHang[j].GiaVon = item.GiaVon;
                        cthd[i].DM_LoHang[j].TonKho = item.TonKho;
                        cthd[i].DM_LoHang[j].CssWarning = RoundDecimal(item.TonKho) < RoundDecimal(cthd[i].DM_LoHang[j].SoLuong);
                        i = cthd.length;
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
            self.HangHoaAfterAdd(cthd);
            UpDate_InforHD(cthd);
        }
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
        $('#txtAutoHangHoa1').focus().select();
    }

    shortcut.add("F3", function () {
        $('#txtAutoHangHoa1').focus();
    });

    shortcut.add("F6", function () {
        self.ClickNhapNhanh_Thuong();
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

    self.showPrint = function () {
        $(".install-notifi").toggle();
        $(".install-notifi").mouseup(function () {
            return false;
        });
        $(".import-fast").mouseup(function () {
            return false;
        });
        $(document).mouseup(function () {
            $(".install-notifi").hide();
        });
        var CheckInHD = localStorage.getItem('CheckInWhenXK');
        if (CheckInHD === "true") {
            $('#divSetPrintPay .main-show').addClass("main-hide");
        }
        else {
            $('#divSetPrintPay .main-show').removeClass("main-hide");
        }
    };

    $(".main-show").click(function () {
        var obj = $('#divSetPrintPay .main-show');
        $(obj).toggleClass("main-hide");
        var allow = false;

        if (obj.hasClass('main-hide')) {
            allow = true;
            $('#colorHDNTH').addClass('flaggOfPrint');
        }
        else {
            $('#colorHDNTH').removeClass('flaggOfPrint');
        }
        localStorage.setItem('CheckInWhenXK', allow);
    });

    function GetTonKho_byIDQuyDois() {
        var arrIDQuiDoi = [], arrIDLoHang = [];

        var cthd = localStorage.getItem(lcCTXuatKho);
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

        var ngayKK = moment(self.newHoaDon().NgayLapHoaDon(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
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
                                    cthd[i].TonKho = forIn.TonKho;
                                    cthd[i].GiaVon = forIn.GiaVon;
                                    cthd[i].ThanhTien = cthd[i].SoLuong * forIn.GiaVon;
                                    cthd[i].CssWarning = RoundDecimal(cthd[i].SoLuong, 3) > RoundDecimal(forIn.TonKho, 3);
                                    break;
                                }
                                else {
                                    for (let k = 0; k < forOut.DM_LoHang.length; k++) {
                                        let itLot = forOut.DM_LoHang[k];
                                        if (forIn.ID_LoHang === itLot.ID_LoHang) {
                                            cthd[i].DM_LoHang[k].TonKho = forIn.TonKho;
                                            cthd[i].DM_LoHang[k].GiaVon = forIn.GiaVon;
                                            cthd[i].DM_LoHang[k].ThanhTien = itLot.SoLuong * forIn.GiaVon;
                                            cthd[i].DM_LoHang[k].CssWarning = RoundDecimal(itLot.SoLuong, 3) > RoundDecimal(forIn.TonKho, 3);

                                            //update for parent
                                            if (forOut.LotParent && forOut.ID_LoHang === forIn.ID_LoHang) {
                                                cthd[i].TonKho = forIn.TonKho;
                                                cthd[i].GiaVon = forIn.GiaVon;
                                                cthd[i].ThanhTien = cthd[i].SoLuong * forIn.GiaVon;
                                                cthd[i].CssWarning = RoundDecimal(cthd[i].SoLuong, 3) > RoundDecimal(forIn.TonKho, 3);
                                            }
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
                    self.HangHoaAfterAdd(cthd);
                }
                else {
                    ShowMessage_Danger(x.mes);
                }
            })
        }
    }

    self.editMaHoaDon = function () {
        var hd = localStorage.getItem(lcHDXuatKho);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hd[0].MaHoaDon = self.newHoaDon().MaHoaDon();
            localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
        }
        else {
            CreateNewHoaDon();
        }
    }

    $('#datetimepicker').datetimepicker({
        timepicker: true,
        mask: true,
        format: 'd/m/Y H:i',
        maxDate: new Date(),
        onChangeDateTime: function (dp, $input) {
            self.newHoaDon().NgayLapHoaDon($input.val());

            var ok = CheckNgayLapHD_format($input.val());
            if (ok) {
                var hd = localStorage.getItem(lcHDXuatKho);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    if ($input.val() !== hd[0].NgayLapHoaDon) {
                        GetTonKho_byIDQuyDois();
                    }
                    hd[0].NgayLapHoaDon = $input.val();
                    localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
                }
                else {
                    CreateNewHoaDon();
                }
            }
        }
    });

    function CreateNewHoaDon() {
        var idNhanVien = self.newHoaDon().ID_NhanVien();
        if (commonStatisJs.CheckNull(idNhanVien)) {
            idNhanVien = _idNhanVien;
        }
        let objHD = [{
            ID: const_GuidEmpty,
            MaHoaDon: self.newHoaDon().MaHoaDon(),
            ID_DonVi: _idDonVi,
            ID_NhanVien: idNhanVien,
            NgayLapHoaDon: moment(new Date()).format('DD/MM/YYYY HH:mm'),
            TongTienHang: 0,
            ID_HoaDon: null,
            ID_PhieuTiepNhan: null,
            MaHoaDonSuaChua: '',
            MaPhieuTiepNhan: '',
            BienSo: '',
            NguoiTao: _userLogin,
            LoaiHoaDon: 8,
            DienGiai: '',
        }];
        localStorage.setItem(lcHDXuatKho, JSON.stringify(objHD));
    }

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

    function UpDate_InforHD(lstCT) {
        let sum = 0;
        for (let i = 0; i < lstCT.length; i++) {
            sum += lstCT[i].ThanhTien;
            for (let j = 1; j < lstCT[i].DM_LoHang.length; j++) {
                sum += lstCT[i].DM_LoHang[j].ThanhTien;
            }
        }

        var hd = localStorage.getItem(lcHDXuatKho);
        if (hd !== null) {
            hd = JSON.parse(hd);
        }
        else {
            // create new if nots exist
            var objHD = [{
                ID: const_GuidEmpty,
                MaHoaDon: '',
                ID_DonVi: _idDonVi,
                ID_NhanVien: _idNhanVien,
                NgayLapHoaDon: moment(new Date()).format('DD/MM/YYYY HH:mm'),
                TongTienHang: 0,
                ID_HoaDon: null,
                ID_PhieuTiepNhan: null,
                MaHoaDonSuaChua: '',
                MaPhieuTiepNhan: '',
                BienSo: '',
                NguoiTao: _userLogin,
                LoaiHoaDon: 8,
                DienGiai: '',
            }];
            hd = objHD;
        }
        hd[0].TongTienHang = sum;
        self.newHoaDon().SetData(hd[0]);
        localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
    }

    // add ncc
    self.resetTextBox = function () {
        newModel_NCC.newDoiTuong(new FormModel_NewVendor());
        newModel_NCC.newNhomDoiTuong(new PartialVendorGroup());
        $('.hidePhone').css('display', 'none');
        $('.hideCode').css('display', 'none');
    }

    function Enable_btnSave() {
        self.isLoading(false);
    }

    self.SaveInvoice = function (status) {
        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            if (cthd.length > 0) {
                var hd = localStorage.getItem(lcHDXuatKho);
                if (hd === null) {
                    ShowMessage_Danger('cache hoa don null');
                    Enable_btnSave();
                    return false;
                }
                hd = JSON.parse(hd);

                var idHoaDon = hd[0].ID;
                var arrCT = [];
                var err = '';
                var errTonKho = '';
                if (idHoaDon === null || idHoaDon === undefined || idHoaDon === const_GuidEmpty) {
                    idHoaDon = const_GuidEmpty;
                }

                if (CheckChar_Special(self.newHoaDon().MaHoaDon())) {
                    ShowMessage_Danger('Mã hóa đơn không được chứa kí tự đặc biệt');
                    Enable_btnSave();
                    return false;
                }

                let dateCheck = CheckNgayLapHD_format($('#datetimepicker').val());
                if (!dateCheck) {
                    Enable_btnSave();
                    return;
                }

                // get cthd & get cthd has soluong = 0
                for (let i = 0; i < cthd.length; i++) {
                    let itOut = cthd[i];

                    itOut.TienThue = 0;
                    itOut.PTThue = 0;

                    // avoid error Guid cthd.ID
                    delete itOut["ID"];
                    if (itOut.ThanhPhan_DinhLuong.length > 0) {
                        // nếu chi tiết xuất chứa TPDinhLuong --> chỉ lưu TPDinhLuong (không lưu parent)
                        for (let k = 0; k < itOut.ThanhPhan_DinhLuong.length; k++) {
                            let tpdl = itOut.ThanhPhan_DinhLuong[k];
                            delete tpdl["ID"];
                            if (tpdl.SoLuong > 0) {
                                tpdl.TienThue = 0;
                                tpdl.PTThue = 0;

                                tpdl.SoThuTu = arrCT.length + 1;
                                arrCT.push(tpdl);
                            }
                        }
                    }
                    else {
                        itOut.SoThuTu = arrCT.length + 1;
                        arrCT.push(itOut);
                    }

                    for (let j = 0; j < itOut.DM_LoHang.length; j++) {
                        let itFor = itOut.DM_LoHang[j];
                        delete itFor["ID"];
                        delete itFor["ID_HoaDon"];
                        // tpdl of lohang
                        if (itFor.ThanhPhan_DinhLuong.length > 0) {
                            for (let k = 0; k < itFor.ThanhPhan_DinhLuong.length; k++) {
                                let tpdl = itFor.ThanhPhan_DinhLuong[k];
                                delete tpdl["ID"];
                                delete tpdl["ThanhPhan_DinhLuong"];

                                if (tpdl.SoLuong > 0) {
                                    tpdl.TienThue = 0;
                                    tpdl.PTThue = 0;

                                    tpdl.SoThuTu = arrCT.length + 1;
                                    arrCT.push(tpdl);
                                }
                            }
                        }
                        else {
                            if (j !== 0) {
                                itFor.TienThue = 0;
                                itFor.PTThue = 0;
                                itFor.SoThuTu = arrCT.length + 1;
                                arrCT.push(itFor);
                            }
                        }
                    }
                }

                for (let i = 0; i < cthd.length; i++) {
                    let itOut = cthd[i];
                    let sluong = RoundDecimal(formatNumberToFloat(itOut.SoLuong),3);
                    let tkho = RoundDecimal(formatNumberToFloat(itOut.TonKho), 3);

                    if (sluong === 0) {
                        err += itOut.TenHangHoa + ', ';
                    }
                  
                    if (sluong > tkho) {
                        errTonKho += itOut.TenHangHoa + ', ';
                    }
                    for (let j = 0; j < itOut.DM_LoHang.length; j++) {
                        let itFor = itOut.DM_LoHang[j];
                        if (j !== 0) {
                            if (formatNumberToFloat(itFor.SoLuong) === 0) {
                                err += itFor.TenHangHoa + ', ';
                            }
                            if (formatNumberToFloat(itFor.SoLuong) > itFor.TonKho) {
                                errTonKho += itFor.TenHangHoa + ' (Lô: ' + itFor.MaLoHang + ') ,';
                            }
                        }
                    }
                }
                err = Remove_LastComma(err);
                errTonKho = Remove_LastComma(errTonKho);

                if (err !== '') {
                    ShowMessage_Danger('Vui lòng nhập số lượng cho ' + err);
                    Enable_btnSave();
                    return false;
                }
                if (self.ThietLap().XuatAm === false) {
                    if (errTonKho !== '') {
                        ShowMessage_Danger('Không đủ số lượng tồn kho cho ' + errTonKho);
                        Enable_btnSave();
                        self.HangHoaAfterAdd(cthd);
                        return false;
                    }
                }

                for (let i = 0; i < arrCT.length; i++) {
                    if (arrCT[i].ID_LoHang === const_GuidEmpty) {
                        arrCT[i].ID_LoHang = null;
                    }
                    delete arrCT[i]["DM_LoHang"];
                }

                // check tpdinhluong = null
                for (let i = 0; i < arrCT.length; i++) {
                    if (commonStatisJs.CheckNull(arrCT[i].ThanhPhan_DinhLuong)) {
                        arrCT[i].ThanhPhan_DinhLuong = [];
                    }
                }

                hd[0].NgayLapHoaDon = moment(hd[0].NgayLapHoaDon, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
                hd[0].TongChiPhi = 0;
                hd[0].ChoThanhToan = status;
                hd[0].TongChietKhau = 0;
                hd[0].TongGiamGia = 0;
                hd[0].PhaiThanhToan = 0;
                hd[0].TongTienThue = 0;
                hd[0].LoaiHoaDon = 8;
                hd[0].NguoiTao = _userLogin;
                hd[0].MaHoaDon = self.newHoaDon().MaHoaDon();
                hd[0].DienGiai = self.newHoaDon().DienGiai();

                if (commonStatisJs.CheckNull(hd[0].ID_NhanVien)) {
                    hd[0].ID_NhanVien = _idNhanVien;
                }

                let myData = {};
                myData.objHoaDon = hd[0];
                myData.objCTHoaDon = arrCT;

                self.isLoading(true);

                if (idHoaDon !== null && idHoaDon !== undefined && idHoaDon !== const_GuidEmpty) {
                    myData.objHoaDon.NguoiSua = _userLogin;
                    Put_XuatKho(myData);
                }
                else {
                    Post_XuatKho(myData);
                }
            }
            else {
                ShowMessage_Danger('Vui lòng nhập chi tiết xuất kho');
            }
        }
        else {
            ShowMessage_Danger('Vui lòng nhập chi tiết xuất kho');
        }
    }

    self.clickBtnHuyHD = function () {
        if (self.HangHoaAfterAdd() !== null && self.HangHoaAfterAdd().length > 0) {
            dialogConfirm('Thông báo', 'Bạn có chắc chắn muốn hủy hóa đơn này không', function () {
                RemoveCache();
                GotoPageListXuatKho();
            })
        }
        else {
            RemoveCache();
            GotoPageListXuatKho();
        }
    }

    function GotoPageListXuatKho() {
        window.location.href = '/#/DamageItems';
    }

    function ResetInforHD() {
        self.HangHoaAfterAdd([]);
        self.TongSoLuongHH(0);
        self.newHoaDon(new FormModel_NewHoaDon());
        self.textSearchHDSC('');
        self.textSearchPhieuTN('');
        self.textSearch(VHeader.TenNhanVien);
        let arrNV = self.NhanViens().slice(0, 20);
        self.ListNVienSearch(arrNV);
    }

    function RemoveCache() {
        localStorage.removeItem(lcCTXuatKho);
        localStorage.removeItem(lcHDXuatKho);
        localStorage.removeItem('cacheTPDL');
        localStorage.removeItem('XK_createfrom');
        localStorage.removeItem('lcXK_EditOpen');
    }

    function Insert_ThongBaoHetTonKho(cthd) {
        var arrQuiDoi = cthd.map(function (x) {
            return x.ID_DonViQuiDoi;
        })
        var arrIDLo = cthd.filter(x => x.ID_LoHang !== null).map(function (x) {
            return x.ID_LoHang;
        })
        var param = {
            ID_ChiNhanh: _idDonVi,
            ListIDQuyDoi: arrQuiDoi,
            ListIDLoHang: arrIDLo,
        }
        ajaxHelper('/api/DanhMuc/HT_API/Insert_ThongBaoHetTon', 'POST', param).done(function (x) {
        })
    }

    function getcthd_atView() {
        var cthd = localStorage.getItem(lcCTXuatKho);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
        }
        else {
            cthd = [];
        }
        return cthd;
    }

    function Post_XuatKho(myData) {
        ajaxHelper(BH_XuatHuyUri + "PostBH_XuatKho?ID_NhanVien=" + _idNhanVien,
            'POST', myData).done(function (x) {
                if (x.res) {
                    myData.objHoaDon.ID = x.data.ID;
                    myData.objHoaDon.MaHoaDon = x.data.MaHoaDon;
                    myData.objHoaDon.NgayLapHoaDon = x.data.NgayLapHoaDon;// used to save diary
                    myData.objHoaDon.NgayLapHoaDonOld = x.data.NgayLapHoaDon;

                    ShowMessage_Success('Tạo phiếu xuất kho thành công');

                    var cthdView = getcthd_atView();
                    SaveDiary(myData.objHoaDon, cthdView);

                    self.InHoaDon(cthdView, myData.objHoaDon);

                    RemoveCache();
                    ResetInforHD();
                }
                else {
                    ShowMessage_Danger(x.mes);
                }
            }).fail(function (x) {
                ShowMessage_Danger(x);
            }).always(function () {
                Enable_btnSave();
            });
    }

    function Put_XuatKho(myData) {
        console.log('Put_XuatKho ', myData);
        let url = BH_XuatHuyUri + "PutBH_XuatKho?ID_NhanVien=" + _idNhanVien;
        if (parseInt(localStorage.getItem('XK_createfrom')) === 4) {
            url = BH_XuatHuyUri + 'UpdateAgain_XuatKho?idNhanVien=' + _idNhanVien;
        }
        ajaxHelper(url, 'post', myData).done(function (x) {
            if (x.res === true) {
                ShowMessage_Success('Cập nhật phiếu xuất kho thành công');

                myData.objHoaDon.ID = x.data.ID;
                myData.objHoaDon.MaHoaDon = x.data.MaHoaDon;
                myData.objHoaDon.NgayLapHoaDon = x.data.NgayLapHoaDon;
                myData.objHoaDon.NgayLapHoaDonOld = x.data.NgayLapHoaDonOld;

                var ctOls = '';
                if (parseInt(localStorage.getItem('XK_createfrom')) === 4) {
                    ctOls = x.data.ChiTietOld;
                }
                myData.objHoaDon.ChiTietOld = ctOls;

                var cthdView = getcthd_atView();
                SaveDiary(myData.objHoaDon, cthdView);

                self.InHoaDon(cthdView, myData.objHoaDon);
                Insert_ThongBaoHetTonKho(cthdView);
              
                Enable_btnSave();
                GotoPageListXuatKho();
                RemoveCache();
                ResetInforHD();
            }
            else {
                Enable_btnSave();
                ShowMessage_Danger(x.mes);
            }
        }).fail(function (x) {
            ShowMessage_Danger(x);
        }).always(function () {
            Enable_btnSave();
        });
    }

    function SaveDiary(hd, cthd) {
        var sStatus = 'Tạo mới', sCTOld = '';
        var loai = 1;
        if (parseInt(localStorage.getItem('XK_createfrom')) === 4) {
            sStatus = 'Cập nhật';
            sCTOld = ' <br /> ' + hd.ChiTietOld;
            loai = 2;
        }
        else {
            if (parseInt(hd.ChoThanhToan) === 1) {
                sStatus = 'Tạm lưu';
            }
        }

        var noidungCT = '';
        var sHD = sStatus.concat(' phiếu xuất kho ', hd.MaHoaDon,
            ', Ngày xuất: ', moment(hd.NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
            ', Số lượng xuất: ', formatNumber3Digit(self.TongSoLuongHH()),
            ', Giá trị xuất: ', formatNumber3Digit(self.newHoaDon().TongTienHang(), 2));

        let style1 = '<a style= \"cursor: pointer\" onclick = \"';
        let style2 = "('";
        let style3 = "')\" >";
        let style4 = "</a>";

        for (let i = 0; i < cthd.length; i++) {
            let itFor = cthd[i];
            let lo = '', sCTHD = '';
            if (itFor.QuanLyTheoLoHang) {
                lo = ' (Lô: '.concat(itFor.MaLoHang, ')');
            }
            sCTHD = ' <br /> '.concat(' - ', style1, "loadHangHoabyMaHH",
                style2, itFor.MaHangHoa, style3, itFor.MaHangHoa, style4,
                lo, ': ', formatNumber3Digit(itFor.SoLuong),
                ' Giá trị: ', formatNumber3Digit(itFor.ThanhTien));
            noidungCT = noidungCT.concat(sCTHD);

            for (let k = 1; k < itFor.DM_LoHang.length; k++) {
                let forLot = itFor.DM_LoHang[k];
                lo = ' (Lô: '.concat(forLot.MaLoHang, ')');
                sCTHD = ' <br /> '.concat(' - ', style1, "loadHangHoabyMaHH",
                    style2, forLot.MaHangHoa, style3, forLot.MaHangHoa, style4,
                    lo, ': ', formatNumber3Digit(forLot.SoLuong),
                    ' Giá trị: ', formatNumber3Digit(forLot.ThanhTien));
                noidungCT = noidungCT.concat(sCTHD)
            }
        }
        noidungCT = sHD.concat(' bao gồm: ', noidungCT, sCTOld);

        var diary = {
            ID_DonVi: hd.ID_DonVi,
            ID_NhanVien: _idNhanVien,
            LoaiNhatKy: loai,
            LoaiHoaDon: 8,
            ChucNang: 'Xuất kho',
            NoiDung: sHD,
            NoiDungChiTiet: noidungCT,
            ID_HoaDon: parseInt(hd.ChoThanhToan) === 1 ? null : hd.ID,
            ThoiGianUpdateGV: parseInt(hd.ChoThanhToan) === 1 ? null : hd.NgayLapHoaDonOld,
        }
        Post_NhatKySuDung_UpdateGiaVon(diary);
    }

    self.InHoaDon = function (cthd, hd) {
        if (localStorage.getItem('CheckInWhenXK') === "true") {
            let yeucau = parseInt(hd.YeuCau);
            let cthdFormat = GetCTHDPrint_Format(cthd, yeucau);
            self.CTHoaDonPrint(cthdFormat);

            var itemHDFormat = GetInforHDPrint(hd);
            self.InforHDprintf(itemHDFormat);

            ajaxHelper('/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + TeamplateMauIn + '&idDonVi=' + _idDonVi, 'GET').done(function (result) {
                let data = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                    + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data, self.numbersPrintHD());
            });
        }
    }

    function GetCTHDPrint_Format(arrCTHD) {
        var arrReturn = [];
        for (let i = 0; i < arrCTHD.length; i++) {
            let itFor = arrCTHD[i];
            itFor.SoThuTu = arrReturn.length + 1;
            itFor.TenHangHoa = itFor.TenHangHoa.split('(')[0] + (itFor.TenDonViTinh !== "" && itFor.TenDonViTinh !== null ? "(" + itFor.TenDonViTinh + ")" : "") + (itFor.ThuocTinh_GiaTri !== null ? itFor.ThuocTinh_GiaTri : "") + (itFor.MaLoHang !== "" && itFor.MaLoHang !== null ? "(Lô: " + itFor.MaLoHang + ")" : "");
            itFor.SoLuong = formatNumber3Digit(itFor.SoLuong);
            itFor.SoLuongHuy = formatNumber3Digit(itFor.SoLuong);
            itFor.GiaVon = formatNumber3Digit(itFor.GiaVon, 2);
            itFor.GiaTriHuy = formatNumber3Digit(itFor.ThanhTien, 2);
            arrReturn.push(itFor);

            for (let k = 1; k < itFor.DM_LoHang.length; k++) {
                let forIn = itFor.DM_LoHang[k];
                forIn.SoThuTu = arrReturn.length + 1;
                forIn.TenHangHoa = forIn.TenHangHoa.split('(')[0] + (forIn.TenDonViTinh !== "" && forIn.TenDonViTinh !== null ? "(" + forIn.TenDonViTinh + ")" : "") + (forIn.ThuocTinh_GiaTri !== null ? forIn.ThuocTinh_GiaTri : "") + (forIn.MaLoHang !== "" && forIn.MaLoHang !== null ? "(Lô: " + forIn.MaLoHang + ")" : "");
                forIn.SoLuong = formatNumber3Digit(forIn.SoLuong);
                forIn.SoLuongHuy = formatNumber3Digit(forIn.SoLuong);
                forIn.GiaVon = formatNumber3Digit(forIn.GiaVon, 2);
                forIn.GiaTriHuy = formatNumber3Digit(forIn.ThanhTien, 2);
                arrReturn.push(forIn);
            }
        }
        return arrReturn;
    }

    function GetInforHDPrint(objHD) {
        var datehoadon = moment(objHD.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss');
        objHD.NgayLapHoaDon = datehoadon;
        var tongtien = formatNumberToFloat(objHD.TongTienHang);
        objHD.TongTienHang = formatNumber3Digit(objHD.TongTienHang);
        objHD.TongSoLuongHang = formatNumber3Digit(self.TongSoLuongHH());
        objHD.TongCong = formatNumber3Digit(tongtien, 2);
        objHD.NguoiTaoHD = _userLogin;
        objHD.MaHoaDonTraHang = objHD.MaHoaDonSuaChua;

        // cong ty, chi nhanh
        let chinhanhchuyen = $.grep(self.DonVis(), function (x) {
            return x.ID === objHD.ID_DonVi;
        });
        objHD.TenChiNhanh = _tenDonVi;
        objHD.DienThoaiChiNhanh = chinhanhchuyen[0].SoDienThoai;
        objHD.DiaChiChiNhanh = chinhanhchuyen[0].DiaChi;
        objHD.ChiNhanhChuyen = _tenDonVi;
        objHD.ChiNhanhNhan = $('#txtAutoDoiTuong option:selected').text();
        objHD.NguoiChuyen = objHD.NguoiTao;
        objHD.NguoiNhan = objHD.NguoiTaoHD;

        // find nvlap hd
        var nvLap = '';
        let nvien = $.grep(self.NhanViens(), function (x) {
            return x.ID === objHD.ID_NhanVien;
        });
        if (nvien.length > 0) {
            nvLap = nvien[0].TenNhanVien;
        }
        objHD.NhanVienBanHang = nvLap;

        objHD.LogoCuaHang = '';
        if (self.CongTy().length > 0) {
            objHD.TenCuaHang = self.CongTy()[0].TenCongTy;
            objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            objHD.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }
        return objHD;
    }

    // if show confirm but not click OK/Cancel, click out popup
    $('#modalPopuplgDelete').on('hidden.bs.modal', function () {
        Enable_btnSave();

        if (self.HangHoaAfterAdd().length > 0 && self.TongSoLuongHH() === 0) {
            // neu click ben ngoai modal delete --> bind infor hd
            var hd = localStorage.getItem(lcHDXuatKho);
            if (hd !== null) {
                hd = JSON.parse(hd);
                self.newHoaDon().SetData(hd[0]);
                Caculator_AmountProduct();
            }
        }
    });

    self.UpdateGhiChuHD = function () {
        var hd = localStorage.getItem(lcHDXuatKho);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hd[0].DienGiai = $(event.currentTarget).val();
            localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
        }
    }

    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        $(".BangBaoLoi").hide();
        document.getElementById('imageUploadForm').value = "";
    }

    self.refreshFileSelect = function () {
        $(".BangBaoLoi").hide();
        self.importPhieuXuatKho();
    }

    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        $(".BangBaoLoi").hide();
        self.loiExcel([]);
    }

    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangXuatKho.xls";
        window.location.href = url;
    }

    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangXuatKho.xlsx";
        window.location.href = url;
    }
    

    self.importPhieuXuatKho = function () {
        $('.choose-file').gridLoader({
            style: "top: 120px;"
        });
     
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (let i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: '/api/DanhMuc/BH_XuatHuyAPI/' + "ImfortExcelXuatHuy",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                console.log(item)

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
                        url: '/api/DanhMuc/BH_XuatHuyAPI/' + "getList_DanhSachHangXuatHuy?ID_ChiNhanh=" + _idDonVi,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (item) {
                            self.deleteFileSelect();
                            console.log('import ', item)                       
                            var arrCTsort = item;
                            var arrIDQuiDoi = [];
                            var cthdLoHang = [];
                            for (let i = 0; i < arrCTsort.length; i++) {
                                arrCTsort[i].ID_Random = CreateIDRandom('CTHD_');
                                arrCTsort[i].ID_HangHoa = arrCTsort[i].ID;
                                arrCTsort[i].GhiChu = '';
                                arrCTsort[i].DonViTinh = [];

                                let idLoHang = arrCTsort[i].ID_LoHang;
                                let quanlytheolo = arrCTsort[i].QuanLyTheoLoHang;
                                arrCTsort[i].QuanLyTheoLoHang = quanlytheolo;
                                arrCTsort[i].MaLoHang = arrCTsort[i].TenLoHang;
                                arrCTsort[i].DM_LoHang = [];
                                arrCTsort[i].ID_LoHang = idLoHang;
                                arrCTsort[i].LotParent = quanlytheolo;
                                arrCTsort[i].SoThuTu = cthdLoHang.length + 1;
                                arrCTsort[i].ThanhTien = arrCTsort[i].SoLuong * arrCTsort[i].GiaVon;
                                arrCTsort[i].CssWarning = arrCTsort[i].SoLuong > arrCTsort[i].TonKho;

                                let ngaysx = arrCTsort[i].NgaySanXuat;
                                let hansd = arrCTsort[i].NgayHetHan;
                                if (!commonStatisJs.CheckNull(ngaysx)) {
                                    ngaysx = moment(ngaysx).format('DD/MM/YYYY');
                                }
                                if (!commonStatisJs.CheckNull(hansd)) {
                                    hansd = moment(hansd).format('DD/MM/YYYY');
                                }
                                arrCTsort[i].NgaySanXuat = ngaysx;
                                arrCTsort[i].NgayHetHan = hansd;
                                arrCTsort[i].ThanhPhan_DinhLuong = [];

                                if ($.inArray(arrCTsort[i].ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                                    arrIDQuiDoi.unshift(arrCTsort[i].ID_DonViQuiDoi);

                                    if (quanlytheolo) {
                                        // push DM_Lo
                                        let objLot = newLot(arrCTsort[i], true);
                                        arrCTsort[i].DM_LoHang.push(objLot);
                                    }
                                    cthdLoHang.push(arrCTsort[i]);
                                }
                                else {
                                    for (let j = 0; j < cthdLoHang.length; j++) {
                                        if (cthdLoHang[j].ID_DonViQuiDoi === arrCTsort[i].ID_DonViQuiDoi) {
                                            if (arrCTsort[i].MaLoHang !== '') {
                                                // addlo
                                                let objLot = newLot(arrCTsort[i], false);
                                                objLot.SoLuong = arrCTsort[i].SoLuong;
                                                objLot.GiaVon = arrCTsort[i].GiaVon;
                                                objLot.ThanhTien = objLot.SoLuong * objLot.GiaVon;
                                                cthdLoHang[j].DM_LoHang.push(objLot);
                                                break;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            var cthd = localStorage.getItem(lcCTXuatKho);
                            if (cthd !== null) {
                                cthd = JSON.parse(cthd);
                            }
                            else {
                                cthd = [];
                            }
                            for (let i = 0; i < cthdLoHang.length; i++) {
                                cthd.unshift(cthdLoHang[i]);
                            }
                            localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));

                            GetTonKho_byIDQuyDois();

                            UpDate_InforHD(cthd);

                            ShowMessage_Success('Import file thành công')
                        },
                        error: function (x) {
                            console.log('getList_DanhSachHangDieuChuyen', x);
                        },
                        complete: function (jqXHR, textStatus, errorThrown) {
                            $('.choose-file').gridLoader({ show: false });
                        },
                    });
                }
            },
            complete: function (e) {
                $('.choose-file').gridLoader({ show: false });
            }
        });
    }

    // paging cthd
    self.CurrentPage = ko.observable(0);
    self.PageSize = ko.observable(100);
    self.FromItem = ko.observable(0);
    self.ToItem = ko.observable(0);

    self.HangHoaAfterAdd_View = ko.computed(function () {
        if (self.HangHoaAfterAdd() !== null) {
            var first = self.CurrentPage() * self.PageSize();
            var cthd = localStorage.getItem(lcCTXuatKho);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
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

            let i = 0;
            if (currentPage === 0) {
                i = parseInt(self.CurrentPage()) + 1;
            }
            else {
                i = self.CurrentPage();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (let i = allPage - 5; i < allPage; i++) {
                        let obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage === 1) {
                        for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            let obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (let j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            let obj = {
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
                        let obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        let obj = {
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
                for (let i = 0; i < allPage; i++) {
                    let obj = {
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
        var hd = localStorage.getItem(lcHDXuatKho);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hd[0].ID_NhanVien = item.ID;
            localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
        }
        else {
            CreateNewHoaDon();
        }
    }

    // xuatkho tu hoadon suachua
    self.ListHoaDonSC = ko.observableArray();
    self.textSearchHDSC = ko.observable();
    self.indexFocus_HDSC = ko.observable(0);
    var GaraAPI = '/api/DanhMuc/GaraAPI/';
    var hdsc_delayTimer = null;

    // 1.reset hdsc, 2.reset PTN
    function ResetHD_ifResetHDSC(type = 1) {
        var hd = localStorage.getItem(lcHDXuatKho);
        if (hd !== null) {
            hd = JSON.parse(hd);

            var cthd = localStorage.getItem(lcCTXuatKho);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
            }
            else {
                cthd = [];
            }

            if (type === 2) {
                hd[0].ID_PhieuTiepNhan = null;
                hd[0].MaPhieuTiepNhan = '';
                self.textSearchPhieuTN('');
                self.newHoaDon().ID_PhieuTiepNhan(null);
            }

            if (commonStatisJs.CheckNull(hd[0].ID_PhieuTiepNhan)) {
                cthd = [];
                self.TongSoLuongHH(0);
                self.newHoaDon().TongTienHang(0);
            }
            else {
                // only reset tpdinhluong if not hdsc
                for (let i = 0; i < cthd.length; i++) {
                    cthd[i].ThanhPhan_DinhLuong = [];
                    cthd[i].HasTPDinhLuong = false;

                    for (let k = 0; k < cthd[i].DM_LoHang.length; k++) {
                        cthd[i].DM_LoHang[k].ThanhPhan_DinhLuong = [];
                        cthd[i].DM_LoHang[k].HasTPDinhLuong = false;
                    }
                }
            }
            localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
            self.HangHoaAfterAdd(cthd);

            hd[0].MaHoaDonSuaChua = '';
            hd[0].ID_HoaDon = null;
            hd[0].HasTPDinhLuong = false;
            localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
        }
        self.textSearchHDSC('');
        self.newHoaDon().ID_HoaDon(null);
        return;
    }

    self.searchHoaDonSC = function () {
        var keyCode = event.keyCode || event.which;
        clearTimeout(hdsc_delayTimer);

        switch (keyCode) {
            case 38:
                if (self.indexFocus_HDSC() < 0) {
                    self.indexFocus_HDSC(0);
                }
                else {
                    self.indexFocus_HDSC(self.indexFocus_HDSC() - 1);
                }
                break;
            case 40:
                if (self.indexFocus_HDSC() < 0) {
                    self.indexFocus_HDSC(0);
                }
                else {
                    self.indexFocus_HDSC(self.indexFocus_HDSC() - 1);
                }
                break;
            default:
                if (keyCode !== 13) {
                    self.indexFocus_HDSC(0);
                }
                hdsc_delayTimer = setTimeout(function () {
                    self.HDSC_searchDB(keyCode);
                }, 300);
                break;
        }
    }

    self.HDSC_keyEnter = function () {
        let itChose = $.grep(self.ListHoaDonSC(), function (x, index) {
            return index === self.indexFocus_HDSC();
        });
        if (itChose.length > 0) {
            self.ChoseHoaDonSC(itChose[0]);
        }
    }

    self.HDSC_searchDB = function (keyCode) {
        var self = this;
        var txt = locdau(self.textSearchHDSC()).trim();
        if (txt === '') {
            self.ListHoaDonSC([]);
            ResetHD_ifResetHDSC(1);
            return;
        }
        if (keyCode === 13 && self.ListHoaDonSC().length > 0) {
            self.HDSC_keyEnter();
        }
        else {
            var idPhieuTN = '%%';
            if (!commonStatisJs.CheckNull(self.newHoaDon().ID_PhieuTiepNhan())) {
                idPhieuTN = self.newHoaDon().ID_PhieuTiepNhan();
            }
            var param = {
                LstIDChiNhanh: [_idDonVi],
                TextSearch: txt,
                ID_HangXe: idPhieuTN,
            }
            ajaxHelper(GaraAPI + "JqAuto_HoaDonSC", 'POST', param).done(function (x) {
                if (x.res) {
                    self.ListHoaDonSC(x.dataSoure);

                    if (keyCode === 13) {
                        self.HDSC_keyEnter();
                    }
                    if (x.dataSoure.length > 0) {
                        self.showListHDSC();
                    }
                    else {
                        self.hideListHDSC();
                    }
                }
            })
        }
    }

    self.ChoseHoaDonSC = function (item) {
        ajaxHelper('/api/DanhMuc/BH_HoaDonAPI/' + 'GetCTHDSuaChua_afterXuatKho?idHoaDon=' + item.ID).done(function (x) {
            if (x.res) {
                var data = x.dataSoure;
                console.log('hdsc ', data)
                data.map(function (x, i) {
                    x['SoThuTu'] = i + 1;
                })

                var cthd = data;
                var arrIDQuiDoi = [];
                var cthdLoHang = [];
                var lstTPCache = [];

                for (let i = 0; i < cthd.length; i++) {
                    let ctNew = $.extend({}, cthd[i]);

                    delete cthd[i]["ID_HoaDon"];  // delete avoid error
                    let ngaysx = moment(cthd[i].NgaySanXuat).format('DD/MM/YYYY');
                    let hethan = moment(cthd[i].NgayHetHan).format('DD/MM/YYYY');
                    if (ngaysx === 'Invalid date') {
                        ngaysx = '';
                    }
                    if (hethan === 'Invalid date') {
                        hethan = '';
                    }
                    ctNew.NgaySanXuat = ngaysx;
                    ctNew.NgayHetHan = hethan;
                    // lo hang
                    let quanlytheolo = ctNew.QuanLyTheoLoHang;
                    ctNew.DM_LoHang = [];
                    ctNew.LotParent = quanlytheolo;
                    ctNew.ID_ChiTietGoiDV = null;
                    ctNew.DonViTinh = [];
                    ctNew.LaConCungLoai = false;
                    ctNew.LotParent = quanlytheolo;
                    ctNew.GiaTriHuy = ctNew.ThanhTien;
                    ctNew.GiaTriHuy = ctNew.ThanhTien;
                    ctNew.CssWarning = ctNew.SoLuong > ctNew.TonKho;

                    ctNew.HasTPDinhLuong = false;
                    if (ctNew.ThanhPhan_DinhLuong.length > 0) {
                        ctNew.HasTPDinhLuong = true;

                        // save cache tpdinhluong of hoadon to cache --> used to delete all ctxk
                        for (let k = 0; k < ctNew.ThanhPhan_DinhLuong.length; k++) {
                            ctNew.ThanhPhan_DinhLuong[k].GiaVon = ctNew.GiaVon;
                            ctNew.ThanhPhan_DinhLuong[k].SoLuong = ctNew.ThanhPhan_DinhLuong[k].SoLuongConLai;
                            ctNew.ThanhPhan_DinhLuong[k].QuyCach = commonStatisJs.CheckNull(ctNew.QuyCach) ? 1 : ctNew.QuyCach;

                            let itCache = $.extend({}, ctNew.ThanhPhan_DinhLuong[k]);
                            itCache.ID_PhieuTiepNhan = item.ID_PhieuTiepNhan;
                            itCache.ID_HoaDon = item.ID;
                            lstTPCache.push(itCache);
                        }
                    }
                    // check exist in cthdLoHang
                    if (ctNew.SoLuong > 0 && $.inArray(ctNew.ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                        arrIDQuiDoi.unshift(ctNew.ID_DonViQuiDoi);
                        // push CTHD
                        ctNew.SoThuTu = cthdLoHang.length + 1;
                        ctNew.ID_Random = CreateIDRandom('RandomCT_');
                        if (quanlytheolo) {
                            let objLot = $.extend({}, ctNew);
                            objLot.HangCungLoais = [];
                            objLot.DM_LoHang = [];
                            objLot.ThanhPhan_DinhLuong = [];
                            ctNew.DM_LoHang.push(objLot);
                        }
                        cthdLoHang.unshift(ctNew);
                    }
                    else {
                        // find in cthdLoHang with same ID_QuiDoi
                        for (let j = 0; j < cthdLoHang.length; j++) {
                            let itFor = cthdLoHang[j];
                            if (cthdLoHang[j].ID_DonViQuiDoi === ctNew.ID_DonViQuiDoi) {
                                if (quanlytheolo) {
                                    let exLo = $.grep(itFor.DM_LoHang, function (o) {
                                        return o.ID_LoHang === ctNew.ID_LoHang;
                                    });
                                    if (exLo.length > 0) {
                                        for (let k = 0; k < itFor.DM_LoHang.length; k++) {
                                            if (itFor.DM_LoHang[k].ID_LoHang === ctNew.ID_LoHang) {
                                                // if lot parent
                                                if (itFor.ID_LoHang === ctNew.ID_LoHang) {
                                                    cthdLoHang[j].SoLuong = itFor.SoLuong + ctNew.SoLuong;
                                                    cthdLoHang[j].ThanhTien = cthdLoHang[j].SoLuong * cthdLoHang[j].GiaVon;
                                                }
                                                cthdLoHang[j].DM_LoHang[k].SoLuong = cthdLoHang[j].DM_LoHang[k].SoLuong + ctNew.SoLuong;
                                                cthdLoHang[j].DM_LoHang[k].ThanhTien = cthdLoHang[j].DM_LoHang[k].SoLuong * cthdLoHang[j].DM_LoHang[k].GiaVon;
                                                break;
                                            }
                                        }
                                    }
                                    else {
                                        let obj = $.extend({}, ctNew);
                                        obj.HangCungLoais = [];
                                        obj.DM_LoHang = [];
                                        obj.LotParent = false;
                                        obj.ID_Random = CreateIDRandom('RandomCT_');
                                        cthdLoHang[j].DM_LoHang.push(obj);
                                    }
                                }
                                else {
                                    cthdLoHang[j].SoLuong = itFor.SoLuong + ctNew.SoLuong;
                                    cthdLoHang[j].ThanhTien = itFor.SoLuong * itFor.GiaVon;
                                }
                                break;
                            }
                        }
                    }
                }
                cthdLoHang = cthdLoHang.filter(x => x.SoLuong > 0);
                localStorage.setItem(lcCTXuatKho, JSON.stringify(cthdLoHang));

                localStorage.setItem('cacheTPDL', JSON.stringify(lstTPCache));

                self.HangHoaAfterAdd(cthdLoHang);
                Caculator_AmountProduct();

                // check if exist tpdl in cthd
                var exTPDL = $.grep(data, function (o) {
                    return o.ThanhPhan_DinhLuong.length > 0;
                })
                var hd = localStorage.getItem(lcHDXuatKho);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    hd[0].MaHoaDon = '';
                    hd[0].ID = const_GuidEmpty;
                    hd[0].ID_PhieuTiepNhan = item.ID_PhieuTiepNhan;
                    hd[0].ID_DonVi = _idDonVi;
                    hd[0].MaHoaDonSuaChua = item.MaHoaDon;
                    hd[0].BienSo = item.BienSo;
                    hd[0].ID_HoaDon = item.ID;
                    hd[0].HasTPDinhLuong = exTPDL.length > 0;
                    hd[0].NgayLapHoaDon = moment(new Date()).format('DD/MM/YYYY HH:mm');

                    if (commonStatisJs.CheckNull(hd[0].MaPhieuTiepNhan)) {
                        hd[0].MaPhieuTiepNhan = item.MaPhieuTiepNhan;
                        self.textSearchPhieuTN(item.MaPhieuTiepNhan.concat('_', item.BienSo));
                    }
                }
                else {
                    let obj = {
                        ID: const_GuidEmpty,
                        MaHoaDon: '',
                        ID_DonVi: _idDonVi,
                        ID_NhanVien: _idNhanVien,
                        NgayLapHoaDon: moment(new Date()).format('DD/MM/YYYY HH:mm'),
                        TongTienHang: 0,
                        ID_HoaDon: item.ID,
                        ID_PhieuTiepNhan: item.ID_PhieuTiepNhan,
                        MaHoaDonSuaChua: item.MaHoaDon,
                        MaPhieuTiepNhan: item.MaPhieuTiepNhan,
                        BienSo: item.BienSo,
                        NguoiTao: _userLogin,
                        LoaiHoaDon: 8,
                        DienGiai: '',
                        HasTPDinhLuong: exTPDL.length > 0
                    }
                    hd = [obj];
                }
                localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));

                UpDate_InforHD(cthdLoHang);
            }
            self.hideListHDSC();
        })
    }
    self.hideListHDSC = function () {
        $('#jqAutoHoaDonSC .gara-search-dropbox').hide();
    }
    self.showListHDSC = function () {
        $('#jqAutoHoaDonSC .gara-search-dropbox').show();
    }
    self.showModalChiTietHoaDon = function () {
        if (self.newHoaDon().ID_HoaDon()) {
            vmChiTietHoaDon.showModalChiTietHoaDon(self.newHoaDon().ID_HoaDon(), 2);
        }
        else {
            ShowMessage_Danger('Vui lòng chọn phiếu tiếp nhận');
        }
    }

    // phieu tiep nhan
    self.ThongTinPhieuTiepNhan = ko.observable();
    self.HangMucSuaChuas = ko.observableArray();
    self.VatDungKemTheos = ko.observableArray();
    self.ListPhieuTiepNhan = ko.observableArray();
    self.textSearchPhieuTN = ko.observable();
    self.indexFocus_phieuTN = ko.observable(0);
    var delayTimer = null;

    self.PTN_keyEnter = function () {
        var itChose = $.grep(self.ListPhieuTiepNhan(), function (x, index) {
            return index === self.indexFocus_phieuTN();
        });
        if (itChose.length > 0) {
            self.ChosePhieuTiepNhan(itChose[0]);
        }
    }

    self.searchPhieuTN_inDB = function (keyCode) {
        var self = this;
        var txt = locdau(self.textSearchPhieuTN()).trim();
        if (txt === '') {
            self.ListPhieuTiepNhan([]);
            ResetHD_ifResetHDSC(2);
            return;
        }
        if (keyCode === 13 && self.ListPhieuTiepNhan().length > 0) {
            self.PTN_keyEnter();
        }
        else {
            var param = {
                LstIDChiNhanh: [_idDonVi],
                TextSearch: txt,
                ID_HangXe: '%%',// muontamtruong (idCustomer)
            }
            ajaxHelper(GaraAPI + "JqAuto_PhieuTiepNhan", 'POST', param).done(function (x) {
                if (x.res) {
                    self.ListPhieuTiepNhan(x.dataSoure);

                    if (keyCode === 13) {
                        self.PTN_keyEnter();
                    }
                    if (x.dataSoure.length > 0) {
                        self.showListPhieuTN();
                    }
                    else {
                        self.hideListPhieuTN();
                    }
                }
            })
        }
    }

    self.searchPhieuTiepNhan = function () {
        var keyCode = event.keyCode || event.which;
        clearTimeout(delayTimer);

        switch (keyCode) {
            case 38:
                if (self.indexFocus_phieuTN() < 0) {
                    self.indexFocus_phieuTN(0);
                }
                else {
                    self.indexFocus_phieuTN(self.indexFocus_phieuTN() - 1);
                }
                break;
            case 40:
                if (self.indexFocus_phieuTN() < 0) {
                    self.indexFocus_phieuTN(0);
                }
                else {
                    self.indexFocus_phieuTN(self.indexFocus_phieuTN() - 1);
                }
                break;
            default:
                if (keyCode !== 13) {
                    self.indexFocus_phieuTN(0);
                }
                delayTimer = setTimeout(function () {
                    self.searchPhieuTN_inDB(keyCode);
                }, 300);
                break;
        }
    }

    self.ChosePhieuTiepNhan = function (itChose) {
        self.ListPhieuTiepNhan([]);
        self.textSearchPhieuTN(itChose.MaPhieuTiepNhan + '_' + itChose.BienSo);

        var param = {
            LstIDChiNhanh: [_idDonVi],
            TextSearch: '%%',
            ID_HangXe: itChose.ID,
        }
        ajaxHelper(GaraAPI + 'JqAuto_HoaDonSC', 'POST', param).done(function (x) {
            if (x.res) {
                self.ListHoaDonSC(x.dataSoure);
                if (x.dataSoure.length > 0) {
                    self.ChoseHoaDonSC(x.dataSoure[0]);
                }
                else {
                    self.textSearchHDSC('');
                    // nếu chỉ xuất kho theo phiếu tiếp nhận (vd: xuất kho trc, rồi tạo hóa đơn sau)
                    var hd = localStorage.getItem(lcHDXuatKho);
                    if (hd !== null) {
                        hd = JSON.parse(hd);
                        hd[0].ID_PhieuTiepNhan = itChose.ID;
                        hd[0].BienSo = itChose.BienSo;
                        hd[0].MaPhieuTiepNhan = itChose.MaPhieuTiepNhan;
                        hd[0].NgayLapHoaDon = moment(new Date()).format('DD/MM/YYYY HH:mm');
                        hd[0].MaHoaDonSuaChua = '';
                        hd[0].ID_HoaDon = null;
                        hd[0].HasTPDinhLuong = false;

                        // reset ID_ChiTietGDV for cthd old
                        var cthd = localStorage.getItem(lcCTXuatKho);
                        if (cthd !== null) {
                            cthd = JSON.parse(cthd);

                            for (let i = 0; i < cthd.length; i++) {
                                cthd[i].ID_ChiTietGoiDV = null;
                                cthd[i].ID_ChiTietDinhLuong = null;

                                for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                    cthd[i].DM_LoHang[j].ID_ChiTietGoiDV = null;
                                    cthd[i].DM_LoHang[j].ID_ChiTietDinhLuong = null;
                                }
                                // reset tpdinhluong
                                cthd[i].ThanhPhan_DinhLuong = [];
                            }
                            localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
                        }
                    }
                    else {
                        var obj = {
                            ID: const_GuidEmpty,
                            MaHoaDon: '',
                            ID_DonVi: _idDonVi,
                            ID_NhanVien: _idNhanVien,
                            NgayLapHoaDon: moment(new Date()).format('DD/MM/YYYY HH:mm'),
                            TongTienHang: 0,
                            ID_HoaDon: null,
                            ID_PhieuTiepNhan: itChose.ID,
                            MaHoaDonSuaChua: '',
                            MaPhieuTiepNhan: itChose.MaPhieuTiepNhan,
                            BienSo: itChose.BienSo,
                            NguoiTao: _userLogin,
                            LoaiHoaDon: 8,
                            DienGiai: '',
                        }
                        hd = [obj];
                    }
                    localStorage.setItem(lcHDXuatKho, JSON.stringify(hd));
                    self.newHoaDon().SetData(hd[0]);
                }
            }
        })
        self.hideListPhieuTN();
    }

    self.hideListPhieuTN = function () {
        $('#jqAutoPhieuTN .gara-search-dropbox').hide();
    }
    self.showListPhieuTN = function () {
        $('#jqAutoPhieuTN .gara-search-dropbox').show();
    }

    function GetInfor_PhieuTiepNhan(isView = true) {
        var id = self.newHoaDon().ID_PhieuTiepNhan();
        $.getJSON(GaraAPI + 'PhieuTiepNhan_GetThongTinChiTiet?id=' + id).done(function (x) {
            if (x.res && x.dataSoure.length > 0) {
                self.ThongTinPhieuTiepNhan(x.dataSoure[0]);
                $.getJSON(GaraAPI + "PhieuTiepNhan_GetTinhTrangXe?id=" + id).done(function (o) {
                    if (o.res) {
                        if (isView) {
                            vmTiepNhanXe.UpdatePhieuTiepNhan(x.dataSoure[0], o.dataSoure.hangmuc, o.dataSoure.vatdung)
                        }
                    }
                });
            }
        })
    }

    self.XemThongTinPhieuTiepNhan = function () {
        GetInfor_PhieuTiepNhan(true);
    }

    self.ShowPop_ChangeTPDinhLuong = function (item) {
        // get list dichvu of tpdl
        var arrDV = [];
        var arrIDQuiDoi = [];

        var lstTP = localStorage.getItem('cacheTPDL');
        if (lstTP !== null) {
            lstTP = JSON.parse(lstTP);
            for (let i = 0; i < lstTP.length; i++) {
                let tpdl = lstTP[i];
                if (!tpdl.LaHangHoa) {
                    if ($.inArray(tpdl.ID_ChiTietDinhLuong, arrIDQuiDoi) === -1) {
                        arrDV.push(tpdl);
                        tpdl.ID_DonViQuiDoi = item.ID_DonViQuiDoi;// Nếu thêm hàng ngoài: gán id_quydoi new = chính hàng ngoài
                        tpdl.DonGia = item.GiaVon;// nếu là hang hóa và tpdinhluong = chính nó ---> gán đơn giá = giá vốn (used to update giavon)
                        tpdl.ID_LoHang = item.ID_LoHang;
                        arrIDQuiDoi.push(tpdl.ID_ChiTietDinhLuong);
                    }
                }
            }
        }
        vmDichVu_ofThanhPhanDL.ListAll_DichVu = arrDV;

        // get again cache TPDinhluong of ctdoing
        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing !== null) {
            vmDichVu_ofThanhPhanDL.showModal(ctDoing);
        }
    }

    $('#vmDichVu_ofThanhPhanDL').on('hidden.bs.modal', function () {
        if (vmDichVu_ofThanhPhanDL.saveOK) {
            var ctUpdate = [];
            for (let i = 0; i < self.HangHoaAfterAdd().length; i++) {
                let ct = self.HangHoaAfterAdd()[i];
                if (ct.ID_Random === vmDichVu_ofThanhPhanDL.DichVu_isDoing.ID_Random) {

                    self.HangHoaAfterAdd()[i].ThanhPhan_DinhLuong = vmDichVu_ofThanhPhanDL.Grid_TPDinhLuongChosed;

                    let soluongTP = 0;
                    for (let j = 0; j < vmDichVu_ofThanhPhanDL.Grid_TPDinhLuongChosed.length; j++) {
                        let tp = vmDichVu_ofThanhPhanDL.Grid_TPDinhLuongChosed[j];
                        soluongTP += formatNumberToFloat(tp.SoLuong);
                    }
                    self.HangHoaAfterAdd()[i].SoLuong = soluongTP;
                    self.HangHoaAfterAdd()[i].ThanhTien = ct.GiaVon * soluongTP;
                    ctUpdate = self.HangHoaAfterAdd()[i];
                    break;
                }
            }

            var cthd = localStorage.getItem(lcCTXuatKho);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].ID_Random === vmDichVu_ofThanhPhanDL.DichVu_isDoing.ID_Random) {
                        cthd[i] = ctUpdate;
                        break;
                    }
                }
                localStorage.setItem(lcCTXuatKho, JSON.stringify(cthd));
                UpDate_InforHD(cthd);
                self.HangHoaAfterAdd(cthd);
            }
        }
    });
}
var modelXuatKhoCT = new XuatKhoChiTiet();
ko.applyBindings(modelXuatKhoCT);

function jqAutoSelectItem(item) {
    modelXuatKhoCT.JqAutoSelectItem(item);
}

function keypressEnterSelected(e) {
    if (e.keyCode === 13) {
        modelXuatKhoCT.JqAutoSelect_Enter();
    }
}