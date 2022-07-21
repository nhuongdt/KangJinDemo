var FormModel_NewHoaDon = function () {
    var self = this;
    self.ID = ko.observable();
    self.MaHoaDon = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ID_CheckIn = ko.observable();
    self.TongTienHang = ko.observable(0);
    self.NgayLapHoaDon = ko.observable(null);
    self.DienGiai = ko.observable();
    self.YeuCau = ko.observable('1');
    self.NguoiTao = ko.observable(VHeader.UserLogin);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.MaHoaDon(item.MaHoaDon);
        self.ID_DonVi(item.ID_DonVi);
        self.ID_CheckIn(item.ID_CheckIn);
        self.TongTienHang(item.TongTienHang);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.DienGiai(item.DienGiai);
        self.YeuCau(item.YeuCau);
        self.NguoiTao(item.NguoiTao);
    }
}

var ChuyenHangChiTiet = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var lcCTChuyenHang = 'lcCTChuyenHang';
    var lcHDChuyenHang = 'lcHDChuyenHang';
    var _idDonVi = $('#hd_IDdDonVi').val();
    var _tenDonVi = $('#hd_TenDonVi').val();
    var _idNhanVien = $('.idnhanvien').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var _userLogin = $('#txtTenTaiKhoan').text().trim();

    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();

    var serverTime = sstime.GetDatetime();
    self.DateHDDefault = ko.observable(moment(serverTime).format('DD/MM/YYYY HH:mm'));

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
    self.ConTonKho = ko.observable(0);// 1.an hang het tonkho
    self.CTHoaDonPrint = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    self.ThietLap = ko.observableArray();

    self.selectedHHChuyenHang = ko.observable();
    self.fileNameExcel = ko.observable();
    self.selectedDonVi = ko.observable();
    self.DonVis = ko.observableArray();
    self.IsChuyenHang = ko.observable(true); //true. ChuyenHang, false. NhanHang

    self.TongSoLuongHH = ko.observable(0);
    self.Quyen_NguoiDung = ko.observableArray();
    self.ChuyenHang_ThayDoiThoiGian = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    modelTypeSearchProduct.TypeSearch(1);// jqAutoProduct

    // check cache tonkho

    function PageLoad() {
        UpdateProperties_ifUndefined();
        GetHT_Quyen_ByNguoiDung();
        Check_QuyenXemGiaVon();
        GetInforCongTy();
        GetListDonVi();
        GetCauHinhHeThong();
        CheckLocTonKho();
    }
    console.log(1)

    PageLoad();

    function UpdateProperties_ifUndefined() {
        var hd = localStorage.getItem(lcHDChuyenHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (commonStatisJs.CheckNull(hd[i].NguoiTao)) {
                    hd[i].NguoiTao = _userLogin;
                }
            }
            localStorage.setItem(lcHDChuyenHang, JSON.stringify(hd));
        }
    }

    function CheckLocTonKho() {
        var tk = localStorage.getItem('chuyenhang_isTonKho');
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
        localStorage.setItem('chuyenhang_isTonKho', self.ConTonKho());
        modelTypeSearchProduct.ConTonKho(self.ConTonKho());
    }

    function CheckSaoChep_EditPhieuNhap() {
        var createHDfrom = localStorage.getItem('createfrom');
        if (createHDfrom !== null) {
            createHDfrom = parseInt(createHDfrom);
            var cthd = localStorage.getItem('lcCH_EditOpen');
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                if (createHDfrom === 2) {
                    self.IsChuyenHang(false);
                    if (cthd[0].ID_CheckIn.toLowerCase() !== _idDonVi.toLowerCase()) {
                        // mở phiếu để nhận hàng, nhưng thay đổi chi nhánh at header
                        RemoveCache();
                        ResetInforHD();
                        return;
                    }
                }

                let ngayLapHD = cthd[0].NgayLapHoaDon;
                if (!commonStatisJs.CheckNull(ngayLapHD)) {
                    ngayLapHD = moment(ngayLapHD).format('DD/MM/YYYY HH:mm');
                }
                var objHD = [{
                    ID: cthd[0].ID_HoaDon,
                    ID_DonVi: cthd[0].ID_DonVi,
                    ID_CheckIn: cthd[0].ID_CheckIn,
                    LoaiHoaDon: 10,
                    MaHoaDon: createHDfrom === 1 ? 'Copy' + cthd[0].MaHoaDon : cthd[0].MaHoaDon,
                    NgayLapHoaDon: ngayLapHD,
                    TongTienHang: cthd[0].TongTienHang,
                    DienGiai: cthd[0].DienGiai,
                    NguoiTao: _userLogin
                }];

                self.newHoaDon().SetData(objHD[0]);
                localStorage.setItem(lcHDChuyenHang, JSON.stringify(objHD));
                localStorage.setItem(lcCTChuyenHang, JSON.stringify(cthd));
                if (createHDfrom === 2) {
                    self.selectedDonVi(objHD[0].ID_DonVi);
                }
                else {
                    self.selectedDonVi(objHD[0].ID_CheckIn);
                }
                self.HangHoaAfterAdd(cthd);
                Caculator_AmountProduct();
                GetTonKho_byIDQuyDois();
            }
        }
        else {
            CheckExistCacheHD();
        }
    }

    function CheckExistCacheHD() {
        var cthd = localStorage.getItem(lcCTChuyenHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (cthd.length > 0) {
                dialogConfirm_OKCancel('Thông báo', 'Hệ thống tìm được 1 bản nháp chưa được lưu lên máy chủ. Bạn có muốn tiếp tục làm việc với bản nháp này?', function () {
                    let hd = localStorage.getItem(lcHDChuyenHang);
                    if (hd !== null) {
                        hd = JSON.parse(hd);
                        let idCNNhan = hd[0].ID_CheckIn;
                        self.HangHoaAfterAdd(cthd);
                        self.newHoaDon().SetData(hd[0]);
                        console.log('hd[0]', hd[0])
                        Caculator_AmountProduct();
                        if (idCNNhan !== null && idCNNhan !== undefined) {
                            if (idCNNhan === _idDonVi) {// nhanhang: bind chinhanh chuyen
                                self.IsChuyenHang(false);
                                self.selectedDonVi(hd[0].ID_DonVi);
                            }
                            else {
                                self.selectedDonVi(idCNNhan);// chuyenhang: bind chinhanh nhan
                            }
                        }
                        GetTonKho_byIDQuyDois();
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

    function GetTonKho_byIDQuyDois() {
        var arrIDQuiDoi = [], arrIDLoHang = [];

        var cthd = localStorage.getItem(lcCTChuyenHang);
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

        var ngayKK = GetNgayLapHD_withTimeNow(self.newHoaDon().NgayLapHoaDon());
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
                                    // neu nhanhang: lay giavon at chinhanh chuyen ~ giachuyen
                                    if (self.IsChuyenHang()) {
                                        cthd[i].GiaVon = forIn.GiaVon;
                                        cthd[i].ThanhTien = cthd[i].SoLuong * forIn.GiaVon;
                                    }
                                    break;
                                }
                                else {
                                    for (let k = 0; k < forOut.DM_LoHang.length; k++) {
                                        let itLot = forOut.DM_LoHang[k];
                                        if (forIn.ID_LoHang === itLot.ID_LoHang) {
                                            cthd[i].DM_LoHang[k].TonKho = forIn.TonKho;
                                            if (self.IsChuyenHang()) {
                                                cthd[i].DM_LoHang[k].GiaVon = forIn.GiaVon;
                                                cthd[i].DM_LoHang[k].ThanhTien = itLot.SoLuong * forIn.GiaVon;
                                            }

                                            //update for parent
                                            if (forOut.LotParent && forOut.ID_LoHang === forIn.ID_LoHang) {
                                                cthd[i].TonKho = forIn.TonKho;
                                                if (self.IsChuyenHang()) {
                                                    cthd[i].GiaVon = forIn.GiaVon;
                                                    cthd[i].ThanhTien = cthd[i].SoLuong * forIn.GiaVon;
                                                }
                                            }
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    localStorage.setItem(lcCTChuyenHang, JSON.stringify(cthd));
                    self.HangHoaAfterAdd(cthd);
                }
                else {
                    ShowMessage_Danger(x.mes);
                }
            })
        }
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
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.Quyen_NguoiDung(data);
                    self.ChuyenHang_ThayDoiThoiGian(CheckQuyenExist('ChuyenHang_ThayDoiThoiGian'));
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
            CheckSaoChep_EditPhieuNhap();
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
            DonGia: itemHH.GiaNhap,
            GiaVon: itemHH.GiaVon,
            SoLuong: soluong,
            SoLuongChuyen: 0,
            ThanhTien: itemHH.GiaVon * soluong,
            GhiChu: '',
            ID_LoHang: itemHH.ID_LoHang,
            MaLoHang: itemHH.MaLoHang,
            NgaySanXuat: ngaysx,
            NgayHetHan: hethan,
            DM_LoHang: [],
            LotParent: lotParent,
        }
    }

    function newLot(itemHH, parent) {
        parent = parent || false;
        let idRandom = parent ? itemHH.ID_Random : CreateIDRandom('CTHD_');
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
            SoLuongChuyen: 0,
            ThanhTien: parent ? itemHH.ThanhTien : itemHH.GiaNhap,
            GhiChu: '',
            LotParent: parent ? true : false,
            QuanLyTheoLoHang: true,
            NguoiTao: _userLogin,// used to save DB
            TenHangHoa: itemHH.TenHangHoa,
            MaHangHoa: itemHH.MaHangHoa,
            TenDonViTinh: itemHH.TenDonViTinh,
            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
        }
    }

    function FindCTHD_isDoing(item) {
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var idRandom = item.ID_Random;

        var lstCTHD = localStorage.getItem(lcCTChuyenHang);
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
        var cthd = localStorage.getItem(lcCTChuyenHang);
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
        var lstCT = localStorage.getItem(lcCTChuyenHang);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
        }
        else {
            lstCT = [];
        }

        var itemEx = $.grep(lstCT, function (x) {
            return x.ID_DonViQuiDoi === item.ID_DonViQuiDoi;
        });
        if (itemEx.length > 0) {
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
                            obj.ThanhTien = soluong * obj.GiaVon;
                            lstCT[i].DM_LoHang.push(obj);
                        }
                    }
                    else {
                        lstCT[i].SoLuong = itFor.SoLuong + soluong;
                        lstCT[i].ThanhTien = lstCT[i].SoLuong * lstCT[i].GiaVon;
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
            lstCT.unshift(newCT);
        }

        // update stt
        let stt = 0;
        for (let i = lstCT.length - 1; i >= 0; i--) {
            lstCT[i].SoThuTu = stt;
            stt = stt + 1;
        }
        lstCT = UpdateAgain_DonViTinhCTHD(item.ID_HangHoa, lstCT);
        localStorage.setItem(lcCTChuyenHang, JSON.stringify(lstCT));

        self.HangHoaAfterAdd(lstCT);
        Bind_UpdateHD(lstCT);
        Caculator_AmountProduct();
    }

    function Focus_InputTienTraHD() {
        $('#txtPaid').focus().select();
    }

    function Enter_CTHD(itemCT, e, charStart) {
        var key = e.keyCode || e.which;

        if (key === 13) {
            var cthd = JSON.parse(localStorage.getItem(lcCTChuyenHang));
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
            var cthd = JSON.parse(localStorage.getItem(lcCTChuyenHang));
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
        var lstCT = localStorage.getItem(lcCTChuyenHang);
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
            localStorage.setItem(lcCTChuyenHang, JSON.stringify(lstCT));

            self.HangHoaAfterAdd(lstCT);
            Bind_UpdateHD(lstCT);
            Caculator_AmountProduct();
        }
    }

    self.EditSoLuong = function (item) {
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);
        var soluong = formatNumberToFloat($(thisObj).val());
        var keyCode = event.keyCode || event.which;
        switch (keyCode) {
            case 38:// up
                soluong = soluong + 1;
                break;
            case 40:// down
                if (soluong > 0) {
                    soluong = soluong - 1;
                }
                break;
        }

        if (!self.IsChuyenHang() && soluong > item.SoLuongChuyen) {
            $(thisObj).val(formatNumber3Digit(item.SoLuongChuyen));
            ShowMessage_Danger('Số lượng nhận không được lớn hơn số lượng chuyển');
            return;
        }

        var lstCT = localStorage.getItem(lcCTChuyenHang);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
            lstCT = updateCTHDLe(lstCT, item, soluong, 1);
            localStorage.setItem(lcCTChuyenHang, JSON.stringify(lstCT));
            Caculator_AmountProduct();
            Bind_UpdateHD(lstCT);
            Enter_CTHD(item, event, 'soluong_');
            Shift_CTHD(item, event, 'soluong_');
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

        var arr = localStorage.getItem(lcCTChuyenHang);
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
            localStorage.setItem(lcCTChuyenHang, JSON.stringify(arr));
            Bind_UpdateHD(arr);
        }
    }

    self.UpdateGhiChu_CTHD = function (item) {
        var idRandom = item.ID_Random;
        var quanlyTheoLo = item.QuanLyTheoLoHang;
        var ghichu = $(event.currentTarget).val();

        var lcCTHD = localStorage.getItem(lcCTChuyenHang);
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
            localStorage.setItem(lcCTChuyenHang, JSON.stringify(lcCTHD));
        }
    }

    self.ChangeDonViTinh = function (item, parent) {
        var newIDQuiDoi = item.ID_DonViQuiDoi;
        var oldIDQuiDoi = parent.ID_DonViQuiDoi;

        var cthd = localStorage.getItem(lcCTChuyenHang);
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
                localStorage.setItem(lcCTChuyenHang, JSON.stringify(cthd));
                self.HangHoaAfterAdd(cthd);
                Bind_UpdateHD(cthd);
            });
        }
    }

    self.ClickThemLo = function (item) {
        var quanlytheolo = item.QuanLyTheoLoHang;
        var idQuiDoi = item.ID_DonViQuiDoi;

        var cthd = localStorage.getItem(lcCTChuyenHang);
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
            localStorage.setItem(lcCTChuyenHang, JSON.stringify(cthd));
            self.HangHoaAfterAdd(cthd);
            Bind_UpdateHD(cthd);
            Caculator_AmountProduct();
        }
    }

    self.ResetLo = function (item) {
        var thisObj = $(event.currentTarget);
        var idRandom = item.ID_Random;
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var lotParent = item.LotParent;

        var cthd = localStorage.getItem(lcCTChuyenHang);
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
            localStorage.setItem(lcCTChuyenHang, JSON.stringify(cthd));
        }
        thisObj.closest('.op-js-chuyenhang-lo').find('input').val('');// empty malo
        thisObj.closest('.op-js-chuyenhang-lo').find('.op-js-chuyenhang-hanlo input').show();// show input ngaysx, hethan
        thisObj.closest('.op-js-chuyenhang-lo').find('.op-js-chuyenhang-hanlo div').hide();// hide div ngaysx, hethan
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
        if (!self.IsChuyenHang()) {
            ShowMessage_Danger('Là chi nhánh nhận. Vui lòng không thay đổi thông tin lô');
            return;
        }
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
        var idRandom = $($this.closest('.op-js-chuyenhang-malo')).find('span').eq(0).attr('id');

        // update TonKho, GiaVon, GiaBan, Lo
        var cthd = localStorage.getItem(lcCTChuyenHang);
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
                        i = cthd.length;
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTChuyenHang, JSON.stringify(cthd));
            self.HangHoaAfterAdd(cthd);
            Bind_UpdateHD(cthd);
        }
        self.ListLot_ofProduct([]);
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
        var CheckInHD = localStorage.getItem('CheckInWhenHT');
        if (CheckInHD === "true") {
            $('#divSetPrintPay .main-show').addClass("main-hide");
        }
        else {
            $('#divSetPrintPay .main-show').removeClass("main-hide");
        }

        var tk = localStorage.getItem('chuyenhang_isTonKho');
        if (tk !== null) {
            tk = parseInt(tk);
            self.ConTonKho(tk);
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
        localStorage.setItem('CheckInWhenHT', allow);
    });

    $('#datetimepicker').datetimepicker({
        timepicker: true,
        mask: true,
        format: 'd/m/Y H:i',
        maxDate: new Date(),
        onChangeDateTime: function (dp, $input) {
            self.newHoaDon().NgayLapHoaDon($input.val());
            var ok = CheckNgayLapHD_format($input.val());
            if (ok) {
                var hd = localStorage.getItem(lcHDChuyenHang);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    hd[0].NgayLapHoaDon = $input.val();
                    localStorage.setItem(lcHDChuyenHang, JSON.stringify(hd));
                }
            }
        }
    });

    self.selectedDonVi.subscribe(function (val) {
        if (self.IsChuyenHang()) {
            self.newHoaDon().ID_CheckIn(val);
        }

        var hd = localStorage.getItem(lcHDChuyenHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            if (self.IsChuyenHang()) {
                hd[0].ID_CheckIn = val;
                console.log('selectedDonVi ', val);
                localStorage.setItem(lcHDChuyenHang, JSON.stringify(hd));
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

    function Bind_UpdateHD(lstCT) {
        let sum = 0;
        for (let i = 0; i < lstCT.length; i++) {
            sum += lstCT[i].ThanhTien;
            for (let j = 1; j < lstCT[i].DM_LoHang.length; j++) {
                sum += lstCT[i].DM_LoHang[j].ThanhTien;
            }
        }
        self.newHoaDon().TongTienHang(sum);

        var objHD = [{
            ID: self.newHoaDon().ID(),
            MaHoaDon: self.newHoaDon().MaHoaDon(),
            ID_DonVi: self.IsChuyenHang() ? _idDonVi : self.newHoaDon().ID_DonVi(),// if nhanhang: ID_DonVi = chinhanh chuyen
            NgayLapHoaDon: self.newHoaDon().NgayLapHoaDon(),
            TongTienHang: self.newHoaDon().TongTienHang(),
            ID_CheckIn: self.newHoaDon().ID_CheckIn(),
            NguoiTao: _userLogin,
        }];
        localStorage.setItem(lcHDChuyenHang, JSON.stringify(objHD));
    }

    // add ncc
    self.resetTextBox = function () {
        newModel_NCC.newDoiTuong(new FormModel_NewVendor());
        newModel_NCC.newNhomDoiTuong(new PartialVendorGroup());
        $('.hidePhone').css('display', 'none');
        $('.hideCode').css('display', 'none');
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

    function Enable_btnSave() {
        document.getElementById("btnaddHDCHHT").disabled = false;
        document.getElementById("btnaddHDCHHT").lastChild.data = "Lưu (F10)";
    }

    self.SaveInvoice = function (status) {
        var cthd = localStorage.getItem(lcCTChuyenHang);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            if (cthd.length > 0) {
                document.getElementById("btnaddHDCHHT").disabled = true;
                document.getElementById("btnaddHDCHHT").lastChild.data = "Đang Lưu";

                var hd = localStorage.getItem(lcHDChuyenHang);
                if (hd === null) {
                    ShowMessage_Danger('cache hoa don null');
                    Enable_btnSave();
                    return false;
                }
                hd = JSON.parse(hd);

                if (self.IsChuyenHang()) {
                    if (hd[0].ID_CheckIn === null || hd[0].ID_CheckIn === undefined) {
                        ShowMessage_Danger('Vui lòng chọn chi nhánh tới');
                        Enable_btnSave();
                        return false;
                    }
                }

                let checkDate = CheckNgayLapHD_format($('#datetimepicker').val());
                if (!checkDate) {
                    Enable_btnSave();
                    return false;
                }

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

                // get cthd & get cthd has soluong = 0
                for (let i = 0; i < cthd.length; i++) {
                    let itOut = cthd[i];
                    itOut.SoThuTu = arrCT.length + 1;
                    arrCT.push(itOut);

                    for (let j = 0; j < itOut.DM_LoHang.length; j++) {
                        let itFor = itOut.DM_LoHang[j];
                        if (j !== 0) {
                            itFor.SoThuTu = arrCT.length + 1;
                            arrCT.push(itFor);
                        }
                    }
                }
                console.log('cthd ', cthd);
                // check chua chonj lo
                var arrNotlo = $.grep(arrCT, function (x) {
                    return x.QuanLyTheoLoHang && x.ID_LoHang === null;
                });
                if (arrNotlo.length > 0) {
                    ShowMessage_Danger('Vui lòng nhập đầy đủ thông tin lô cho hàng hóa');
                    Enable_btnSave();
                    return false;
                }
                // only check soluong if chuyenhang
                if (self.IsChuyenHang()) {
                    for (let i = 0; i < cthd.length; i++) {
                        let itOut = cthd[i];
                        if (formatNumberToFloat(itOut.SoLuong) === 0) {
                            err += itOut.TenHangHoa + ', ';
                        }
                        // check soluong > tonkho
                        if (idHoaDon === const_GuidEmpty) {
                            if (formatNumberToFloat(itOut.SoLuong) > itOut.TonKho) {
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
                        else {
                            // sua doi hoadonchuyenhang: cộng ngược lại số lượng chuyển trước đó
                            if (formatNumberToFloat(itOut.SoLuong) > itOut.TonKho + itOut.SoLuongChuyen) {
                                errTonKho += itOut.TenHangHoa + ', ';
                            }

                            for (let j = 0; j < itOut.DM_LoHang.length; j++) {
                                let itFor = itOut.DM_LoHang[j];
                                if (j !== 0) {
                                    if (formatNumberToFloat(itFor.SoLuong) === 0) {
                                        err += itFor.TenHangHoa + ', ';
                                    }
                                    if (formatNumberToFloat(itFor.SoLuong) > itFor.TonKho + itFor.SoLuongChuyen) {
                                        errTonKho += itFor.TenHangHoa + ' (Lô: ' + itFor.MaLoHang + ') ,';
                                    }
                                }
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
                        return false;
                    }
                }

                for (let i = 0; i < arrCT.length; i++) {
                    delete arrCT[i]["DM_LoHang"];
                }

                var nhanhang = 0;
                hd[0].NgayLapHoaDon = GetNgayLapHD_withTimeNow(hd[0].NgayLapHoaDon);
                hd[0].TongChiPhi = 0;

                if (self.IsChuyenHang()) {
                    if (status === 0) {
                        hd[0].YeuCau = '1';// themmoi
                    }
                    else {
                        hd[0].YeuCau = '2';//tamluu chuyenhang
                    }
                }
                else {
                    nhanhang = 1;
                    if (status === 0) {
                        hd[0].TongChiPhi = hd[0].TongTienHang;
                        hd[0].YeuCau = '4';// nhanhang
                    }
                    else {
                        hd[0].YeuCau = '2';// tamluu nhanhang
                    }
                }

                hd[0].TongChietKhau = 0;
                hd[0].TongGiamGia = 0;
                hd[0].PhaiThanhToan = 0;
                hd[0].TongTienThue = 0;
                hd[0].LoaiHoaDon = 10;
                hd[0].NguoiTao = _userLogin;
                hd[0].MaHoaDon = self.newHoaDon().MaHoaDon();
                hd[0].DienGiai = self.newHoaDon().DienGiai();

                let myData = {};
                myData.objHoaDon = hd[0];
                myData.objCTHoaDon = arrCT;
                myData.idnhanvien = _idNhanVien;

                console.log(myData)

                if (idHoaDon !== null && idHoaDon !== undefined && idHoaDon !== const_GuidEmpty) {
                    myData.objHoaDon.NguoiSua = _userLogin;
                    myData.IsNhanHang = nhanhang;
                    Put_ChuyenHang(myData);
                }
                else {
                    Post_ChuyenHang(myData);
                }
            }
            else {
                ShowMessage_Danger('Vui lòng nhập chi tiết chuyển hàng');
            }
        }
        else {
            ShowMessage_Danger('Vui lòng nhập chi tiết chuyển hàng');
        }
    }

    self.clickBtnHuyHD = function () {
        if (self.HangHoaAfterAdd() != null && self.HangHoaAfterAdd().length > 0) {
            dialogConfirm('Xác nhận hủy', 'Bạn có chắc chắn muốn hủy hóa đơn đang thực hiện không', function () {
                RemoveCache();
                GotoPageListChuyenHang();
            });
        }
        else {
            RemoveCache();
            GotoPageListChuyenHang();
        }
    }

    function GotoPageListChuyenHang() {
        window.location.href = '/#/Transfers';
    }

    function ResetInforHD() {
        self.HangHoaAfterAdd([]);
        self.TongSoLuongHH(0);
        self.newHoaDon(new FormModel_NewHoaDon());
        $('#tenloaitien').text('(Tiền mặt)');
    }

    function RemoveCache() {
        localStorage.removeItem(lcCTChuyenHang);
        localStorage.removeItem(lcHDChuyenHang);
        localStorage.removeItem('createfrom');
        localStorage.removeItem('lcCH_EditOpen');
    }

    function Post_ChuyenHang(myData) {
        ajaxHelper(BH_HoaDonUri + 'Insert_ChuyenHang', 'POST', myData).done(function (x) {
            if (x.res === true) {
                myData.objHoaDon.MaHoaDon = x.data.MaHoaDon;
                ShowMessage_Success('Tạo phiếu chuyển hàng thành công');

                if (parseInt(myData.objHoaDon.YeuCau) === 1) {
                    Insert_ThongBaoHetTonKho(myData.objCTHoaDon);
                }
                GotoPageListChuyenHang();
                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
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
            console.log(' tbton ', x)
        })
    }

    function Put_ChuyenHang(myData) {
        console.log('Put_ChuyenHang ', myData);
        let url = 'Update_ChuyenHang';
        if (parseInt(localStorage.getItem('createfrom')) === 4) {
            url = 'UpdateAgain_ChuyenHang';
            myData.objHoaDon.ID_NhanVien = _idNhanVien;
        }
        ajaxHelper(BH_HoaDonUri + url, 'post', myData).done(function (x) {
            if (x.res === true) {
                ShowMessage_Success('Cập nhật phiếu chuyển hàng thành công');

                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
                GotoPageListChuyenHang();

                if (parseInt(myData.objHoaDon.YeuCau) === 1) {
                    Insert_ThongBaoHetTonKho(myData.objCTHoaDon);
                }

                Enable_btnSave();
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
        });;
    }

    self.InHoaDon = function (cthd, hd) {
        if (localStorage.getItem('CheckInWhenHT') === "true") {
            let yeucau = parseInt(hd.YeuCau);
            let cthdFormat = GetCTHDPrint_Format(cthd, yeucau);
            self.CTHoaDonPrint(cthdFormat);

            var soluongchuyen = 0;
            let tongTienChuyen = 0;
            for (let i = 0; i < cthd.length; i++) {
                let giachuyen = formatNumberToInt(cthd[i].GiaChuyen);
                soluongchuyen += formatNumberToFloat(cthd[i].SoLuongChuyen);
                tongTienChuyen += cthd[i].SoLuongChuyen * giachuyen;
            }
            let itemHDFormat = GetInforHDPrint(hd);
            if (yeucau === 4) {
                itemHDFormat.TongSoLuongChuyen = formatNumber3Digit(soluongchuyen);
                itemHDFormat.TongSoLuongNhan = formatNumber3Digit(self.TongSoLuongHH());
                itemHDFormat.TongTienChuyen = formatNumber3Digit(tongTienChuyen, 2);
                itemHDFormat.TongTienNhan = formatNumber3Digit(hd.TongTienHang, 2);
            }
            else {
                itemHDFormat.TongSoLuongChuyen = formatNumber3Digit(self.TongSoLuongHH());
                itemHDFormat.TongSoLuongNhan = 0;
                itemHDFormat.TongTienChuyen = formatNumber3Digit(hd.TongTienHang, 2);
                itemHDFormat.TongTienNhan = 0;
            }
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

    function GetCTHDPrint_Format(arrCTHD, yeucau) {
        console.log(arrCTHD)
        for (let i = 0; i < arrCTHD.length; i++) {
            arrCTHD[i].TenHangHoa = arrCTHD[i].TenHangHoa.split('(')[0] + (arrCTHD[i].TenDonViTinh !== "" && arrCTHD[i].TenDonViTinh !== null ? "(" + arrCTHD[i].TenDonViTinh + ")" : "") + (arrCTHD[i].ThuocTinh_GiaTri !== null ? arrCTHD[i].ThuocTinh_GiaTri : "") + (arrCTHD[i].MaLoHang !== "" && arrCTHD[i].MaLoHang !== null ? "(Lô: " + arrCTHD[i].MaLoHang + ")" : "");
            arrCTHD[i].SoLuong = formatNumber3Digit(arrCTHD[i].SoLuong);
            arrCTHD[i].SoLuongChuyen = yeucau === 4 ? formatNumber3Digit(arrCTHD[i].SoLuongChuyen) : arrCTHD[i].SoLuong;
            arrCTHD[i].SoLuongNhan = yeucau === 4 ? arrCTHD[i].SoLuong : 0;
            arrCTHD[i].GiaChuyen = formatNumber3Digit(arrCTHD[i].DonGia, 2);
            arrCTHD[i].ThanhTien = formatNumber3Digit(arrCTHD[i].ThanhTien, 2);
        }
        return arrCTHD;
    }

    function GetInforHDPrint(objHD) {
        var datehoadon = moment(objHD.NgayLapHoaDon, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
        objHD.NgayLapHoaDon = datehoadon;
        var tongtien = formatNumberToFloat(objHD.TongTienHang);
        objHD.TongTienHang = formatNumber3Digit(objHD.TongTienHang, 2);
        objHD.TienBangChu = DocSo(tongtien);
        objHD.TongCong = formatNumber3Digit(tongtien, 2);
        objHD.NguoiTaoHD = _userLogin;

        // cong ty, chi nhanh
        let chinhanhchuyen = $.grep(self.DonVis(), function (x) {
            return x.ID === objHD.ID_DonVi;
        });
        objHD.TenCuaHang = _tenDonVi;
        objHD.DienThoaiChiNhanh = chinhanhchuyen[0].SoDienThoai;
        objHD.DiaChiChiNhanh = chinhanhchuyen[0].DiaChi;
        objHD.ChiNhanhChuyen = _tenDonVi;
        objHD.ChiNhanhNhan = $('#txtAutoDoiTuong option:selected').text();
        objHD.NguoiChuyen = objHD.NguoiTao;
        objHD.NguoiNhan = objHD.NguoiTaoHD;

        objHD.LogoCuaHang = '';
        if (self.CongTy().length > 0) {
            objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        }
        console.log('objHD_print ', objHD);

        return objHD;
    }

    // if show confirm but not click OK/Cancel, click out popup
    $('#modalPopuplgDelete').on('hidden.bs.modal', function () {
        Enable_btnSave();

        if (self.HangHoaAfterAdd().length > 0 && self.TongSoLuongHH() === 0) {
            // neu click ben ngoai modal delete --> bind infor hd
            var hd = localStorage.getItem(lcHDChuyenHang);
            if (hd !== null) {
                hd = JSON.parse(hd);
                let idCNNhan = hd[0].ID_CheckIn;
                self.newHoaDon().SetData(hd[0]);
                if (idCNNhan !== null && idCNNhan !== undefined) {
                    if (idCNNhan === _idDonVi) {// nhanhang: bind chinhanh chuyen
                        self.IsChuyenHang(false);
                        self.selectedDonVi(hd[0].ID_DonVi);
                    }
                    else {
                        self.selectedDonVi(idCNNhan);// chuyenhang: bind chinhanh nhan
                    }
                }
                Caculator_AmountProduct();
            }
        }
    });

    self.UpdateGhiChuHD = function () {
        var hd = localStorage.getItem(lcHDChuyenHang);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hd[0].DienGiai = $(event.currentTarget).val();
            localStorage.setItem(lcHDChuyenHang, JSON.stringify(hd));
        }
    }

    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadForm').value = "";
    }

    self.refreshFileSelect = function () {
        self.importDieuChuyen();
    }

    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        self.loiExcel([]);
    }

    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangDieuChuyen.xls";
        window.location.href = url;
    }

    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangDieuChuyen.xlsx";
        window.location.href = url;
    }

    self.importDieuChuyen = function () {
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
            url: '/api/DanhMuc/BH_XuatHuyAPI/' + "ImfortExcelDieuChuyen",
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
                        url: '/api/DanhMuc/BH_XuatHuyAPI/' + "getList_DanhSachHangDieuChuyen?iddonvi=" + _idDonVi,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (item) {
                            self.deleteFileSelect();

                            var arrCTsort = item;
                            var arrIDQuiDoi = [];
                            var cthdLoHang = [];
                            for (let i = 0; i < arrCTsort.length; i++) {
                                arrCTsort[i].ID_Random = CreateIDRandom('CTHD_');
                                arrCTsort[i].ID_HangHoa = arrCTsort[i].ID;
                                arrCTsort[i].SoLuongChuyen = 0;
                                arrCTsort[i].GhiChu = '';

                                var idLoHang = arrCTsort[i].ID_LoHang;
                                var quanlytheolo = arrCTsort[i].QuanLyTheoLoHang;
                                arrCTsort[i].QuanLyTheoLoHang = quanlytheolo;
                                arrCTsort[i].DM_LoHang = [];
                                arrCTsort[i].ID_LoHang = idLoHang;
                                arrCTsort[i].LotParent = quanlytheolo;
                                arrCTsort[i].SoThuTu = cthdLoHang.length + 1;
                                arrCTsort[i].TienChietKhau = 0;
                                arrCTsort[i].PTChietKhau = 0;
                                arrCTsort[i].PTThue = 0;
                                arrCTsort[i].TienThue = 0;
                                arrCTsort[i].SoThuTu = cthdLoHang.length + 1;

                                let ngaysx = '', hansd = '';
                                if (!commonStatisJs.CheckNull(arrCTsort[i].NgaySanXuat)) {
                                    ngaysx = moment(arrCTsort[i].NgaySanXuat).format('DD/MM/YYYY');
                                }
                                if (!commonStatisJs.CheckNull(arrCTsort[i].NgayHetHan)) {
                                    hansd = moment(arrCTsort[i].NgayHetHan).format('DD/MM/YYYY');
                                }
                                arrCTsort[i].NgaySanXuat = ngaysx;
                                arrCTsort[i].NgayHetHan = hansd;

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
                                                var objLot = newLot(arrCTsort[i], false);
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

                            var cthd = localStorage.getItem(lcCTChuyenHang);
                            if (cthd !== null) {
                                cthd = JSON.parse(cthd);
                            }
                            else {
                                cthd = [];
                            }
                            for (let i = 0; i < cthdLoHang.length; i++) {
                                cthd.unshift(cthdLoHang[i]);
                            }
                            localStorage.setItem(lcCTChuyenHang, JSON.stringify(cthd));
                            self.HangHoaAfterAdd(cthd);
                            Bind_UpdateHD(cthd);
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
    self.PageSize = ko.observable(10);
    self.FromItem = ko.observable(0);
    self.ToItem = ko.observable(0);

    self.HangHoaAfterAdd_View = ko.computed(function () {
        if (self.HangHoaAfterAdd() !== null) {
            var first = self.CurrentPage() * self.PageSize();
            var cthd = localStorage.getItem(lcCTChuyenHang);
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
}
var modelChuyenHangCT = new ChuyenHangChiTiet();
ko.applyBindings(modelChuyenHangCT);

function jqAutoSelectItem(item) {
    modelChuyenHangCT.JqAutoSelectItem(item);
}

function keypressEnterSelected(e) {
    if (e.keyCode === 13) {
        modelChuyenHangCT.JqAutoSelect_Enter();
    }
}