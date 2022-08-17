var FormModel_NewHoaDon = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_HoaDon = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ID_NhanVien = ko.observable(VHeader.IdNhanVien);
    self.IDRandom = ko.observable();
    self.NguoiTao = ko.observable(VHeader.UserLogin);
    self.LoaiHoaDon = ko.observable($('#txtLoaiHoaDon').val());
    self.MaHoaDon = ko.observable();
    self.NgayLapHoaDon = ko.observable(null);
    self.TongTienHang = ko.observable(0);
    self.PhaiThanhToan = ko.observable(0);
    self.TongGiamGia = ko.observable(0);
    self.DaThanhToan = ko.observable(0);
    self.KhachDaTra = ko.observable(0);
    self.TongTienThue = ko.observable(0);
    self.TongChiPhi = ko.observable(0);
    self.TongChietKhau = ko.observable(0);
    self.DienGiai = ko.observable('');

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_HoaDon(item.ID_HoaDon);
        self.ID_DonVi(item.ID_DonVi);
        self.ID_NhanVien(item.ID_NhanVien);
        self.IDRandom(item.IDRandom);
        self.NguoiTao(item.NguoiTao);
        self.LoaiHoaDon(item.LoaiHoaDon);
        self.MaHoaDon(item.MaHoaDon);
        self.NgayLapHoaDon(item.NgayLapHoaDon);
        self.TongTienHang(item.TongTienHang);
        self.PhaiThanhToan(item.PhaiThanhToan);
        self.TongGiamGia(item.TongGiamGia);
        self.DaThanhToan(item.DaThanhToan);
        self.KhachDaTra(item.KhachDaTra);
        self.TongTienThue(item.TongTienThue);
        self.DienGiai(item.DienGiai);
        self.TongChietKhau(item.TongChietKhau);
    }
}

var PhieuDieuChinhChiTiet = function () {
    var self = this;
    var BH_DieuChinhUri = '/api/DanhMuc/BH_DieuChinh/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';

    var lcCTDieuChinh = '';
    var lcHDDieuChinh = '';
    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();

    self.LoaiHoaDonMenu = parseInt($('#txtLoaiHoaDon').val());// use ko-component
    self.LoaiHoaDon = ko.observable(self.LoaiHoaDonMenu); // used check bind view

    switch (self.LoaiHoaDonMenu) {
        case 16:
            lcCTDieuChinh = 'ctGVTieuChuan';
            lcHDDieuChinh = 'hdGVTieuChuan';
            break;
        case 18:
            lcCTDieuChinh = 'lcCTDieuChinh';
            lcHDDieuChinh = 'lcHDDieuChinh';
            break;
    }

    var _idDonVi = VHeader.IdDonVi;
    var _tenDonVi = VHeader.TenDonVi;
    var _userLogin = VHeader.UserLogin;
    var _idNhanVien = VHeader.IdNhanVien;
    var _IDNguoiDung = VHeader.IdNguoiDung;
    var shopCookies = VHeader.IdNganhNgheKinhDoanh;
    self.ID_DonVi = ko.observable(_idDonVi);

    self.isLoadding = ko.observable(false);
    self.newHoaDon = ko.observable(new FormModel_NewHoaDon());

    var serverTime = sstime.GetDatetime();
    self.DateHDDefault = ko.observable(moment(serverTime).format('DD/MM/YYYY HH:mm'));

    self.NhanViens = ko.observableArray();
    self.ListNVienSearch = ko.observableArray();
    self.HangHoaAfterAdd = ko.observableArray();
    self.HangHoas = ko.observableArray();
    self.ListLot_ofProduct = ko.observableArray();
    self.ListLot_ofProductAll = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    self.CongTy = ko.observableArray();
    self.ChiNhanh = ko.observableArray();
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
    self.PhieuDieuChinh_ThayDoiThoiGian = ko.observable();
    self.PhieuDieuChinh_ThayDoiNhanVien = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable(true);

    // search and paging CTHD
    self.filterHangHoa_ChiTietHD = ko.observable('');
    self.PageSize_CTHD = ko.observable(10);
    self.currentPage_CTHD = ko.observable(0);
    self.fromitem_CTHD = ko.observable(1);
    self.toitem_CTHD = ko.observable();
    self.PageCount_CTHD = ko.observable();
    self.TotalRecord_CTHD = ko.observable(0);

    self.isGara = ko.observable(false);
    self.shopCookies = ko.observable(shopCookies.toLowerCase());

    function PageLoad() {
        UpdateProperties_ifUndefined();
        GetListNhanVien();
        GetHT_Quyen_ByNguoiDung();
        Check_QuyenXemGiaVon();
        GetInforCongTy();
        GetInforChiNhanh();
        Check_OnOffPrint();
    }
    console.log(1)

    PageLoad();

    function UpdateProperties_ifUndefined() {
        let idRandom = CreateIDRandom('HD_');
        var hd = localStorage.getItem(lcHDDieuChinh);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (commonStatisJs.CheckNull(hd[i].IDRandom)) {
                    hd[i].IDRandom = idRandom;
                }
            }
            localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
        }

        var cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                let itFor = cthd[i];
                if (commonStatisJs.CheckNull(itFor.IDRandomHD)) {
                    cthd[i].IDRandomHD = idRandom;
                }
            }
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));
        }
    }

    function CheckSaoChep_EditPhieuDieuChinh() {
        let hd = localStorage.getItem(lcHDDieuChinh);
        if (hd !== null) {
            hd = JSON.parse(hd);
        }
        else {
            hd = [];
        }

        let ctDC = localStorage.getItem(lcCTDieuChinh);
        if (ctDC !== null) {
            ctDC = JSON.parse(ctDC);
        }
        else {
            ctDC = [];
        }

        let cthd = localStorage.getItem('danhmuc_ctDieuChinh');
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            let ngaylapHD = cthd[0].NgayLapHoaDon;
            if (!commonStatisJs.CheckNull(ngaylapHD)) {
                ngaylapHD = moment(ngaylapHD).format('DD/MM/YYYY HH:mm');
            }

            let idRanDomHD = CreateIDRandom('HD_');
            let objHD = {
                ID: cthd[0].ID_HoaDon,
                ID_DonVi: commonStatisJs.CheckNull(cthd[0].ID_DonVi) ? _idDonVi : cthd[0].ID_DonVi,
                LoaiHoaDon: self.LoaiHoaDonMenu,
                IDRandom: idRanDomHD,
                ID_NhanVien: cthd[0].ID_NhanVien,
                MaHoaDon: cthd[0].MaHoaDon,
                NgayLapHoaDon: ngaylapHD,
                DienGiai: cthd[0].DienGiai,
                NguoiTao: _userLogin,
            };

            let type = localStorage.getItem('dieuchinh_typeCache');
            if (type !== null) {
                let hdEx = $.grep(hd, function (x) {
                    return x.MaHoaDon === objHD.MaHoaDon;
                });
                if (hdEx.length > 0) {
                    idRanDomHD = hdEx[0].IDRandom;
                }

                switch (type) {
                    case 0:// saochep
                        switch (self.LoaiHoaDonMenu) {
                            case 16:
                                objHD.ID = const_GuidEmpty;
                                objHD.TongTienHang = 0;
                                objHD.TongTienThue = 0;
                                objHD.TongGiamGia = 0;
                                objHD.TongChietKhau = 0;
                                objHD.TongChiPhi = 0;
                                hd.push(objHD);
                                break;
                        }
                        break;
                    case 1:// update
                        switch (self.LoaiHoaDonMenu) {
                            case 16:
                                objHD.TongTienHang = 0;
                                objHD.TongTienThue = 0;
                                objHD.TongGiamGia = 0;
                                objHD.TongChietKhau = 0;
                                objHD.TongChiPhi = 0;
                                hd.push(objHD);
                                break;
                        }
                        break;
                }

                // chỉ add nếu hd not exist
                if (hdEx.length === 0) {

                    hd.push(objHD);

                    for (let i = 0; i < cthd.length; i++) {
                        cthd[i].IDRandomHD = idRanDomHD;
                        ctDC.push(cthd[i]);
                    }
                }
            }

            localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(ctDC));

            UpdateTonKhoGiaVon_byIDQuyDois(idRanDomHD);

            localStorage.removeItem('danhmuc_ctDieuChinh');
            localStorage.removeItem('dieuchinh_typeCache');
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
        let hd = localStorage.getItem(lcHDDieuChinh);
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

    function UpdateTonKhoGiaVon_byIDQuyDois(idRandom = null) {
        var arrIDQuiDoi = [], arrIDLoHang = [];

        var cthd = localStorage.getItem(lcCTDieuChinh);
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

                            let dongia = 0;
                            switch (self.LoaiHoaDonMenu) {
                                case 16:
                                    dongia = forIn.GiaVonTieuChuan;
                                    break;
                                case 18:
                                    dongia = forIn.GiaVon;
                                    thanhtien = 0;
                                    break;
                            }
                            if (forIn.ID_DonViQuiDoi === forOut.ID_DonViQuiDoi) {
                                if (forOut.QuanLyTheoLoHang === false) {
                                    cthd[i].TonKho = forIn.TonKho;
                                    cthd[i].GiaVon = forIn.GiaVon;
                                    cthd[i].DonGia = dongia;
                                    break;
                                }
                                else {
                                    for (let k = 0; k < forOut.DM_LoHang.length; k++) {
                                        let itLot = forOut.DM_LoHang[k];
                                        if (forIn.ID_LoHang === itLot.ID_LoHang) {
                                            cthd[i].DM_LoHang[k].TonKho = forIn.TonKho;
                                            cthd[i].DM_LoHang[k].GiaVon = forIn.GiaVon;
                                            cthd[i].DM_LoHang[k].DonGia = dongia;

                                            //update for parent
                                            if (forOut.LotParent && forOut.ID_LoHang === forIn.ID_LoHang) {
                                                cthd[i].TonKho = forIn.TonKho;
                                                cthd[i].GiaVon = forIn.GiaVon;
                                                cthd[i].DonGia = dongia;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));

                    if (!commonStatisJs.CheckNull(idRandom)) {
                        BindHD_byIDRandom(idRandom);
                        BindCTHD_byIDRandomHD(idRandom);
                    }
                }
            })
        }
    }

    function BindCTHD_byIDRandomHD(idRandom = null) {
        if (commonStatisJs.CheckNull(idRandom)) {
            idRandom = self.newHoaDon().IDRandom();
        }
        let cthdEx = [];
        let cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            cthdEx = $.grep(cthd, function (x) {
                return x.IDRandomHD === idRandom;
            });
        }
        self.HangHoaAfterAdd(cthdEx);
    }

    function CheckExistCacheHD() {
        var cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd != null) {
            cthd = JSON.parse(cthd);
            if (cthd.length > 0) {
                dialogConfirm_OKCancel('Thông báo', 'Hệ thống tìm được 1 bản nháp chưa được lưu lên máy chủ. Bạn có muốn tiếp tục làm việc với bản nháp này?', function () {
                    var hd = localStorage.getItem(lcHDDieuChinh);
                    if (hd !== null) {
                        hd = JSON.parse(hd);

                        let hdLast = hd[hd.length - 1];
                        let idRandom = hdLast.IDRandom;

                        UpdateTonKhoGiaVon_byIDQuyDois(idRandom);// update gain gaivon tieuchuan + giavon hientai

                        // bind nhanvien
                        let nvien = $.grep(self.NhanViens(), function (x) {
                            return x.ID === hdLast.ID_NhanVien;
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

            CheckSaoChep_EditPhieuDieuChinh();

            // find nvlogin
            let nvien = $.grep(self.NhanViens(), function (x) {
                return x.ID === self.newHoaDon().ID_NhanVien();
            });
            if (nvien.length > 0) {
                self.textSearch(nvien[0].TenNhanVien);
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
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.Quyen_NguoiDung(data);

                    switch (self.LoaiHoaDonMenu) {
                        case 16:
                            self.PhieuDieuChinh_ThayDoiThoiGian(CheckQuyenExist('PhieuDieuChinh_ThayDoiThoiGian'));
                            self.PhieuDieuChinh_ThayDoiNhanVien(CheckQuyenExist('PhieuDieuChinh_ThayDoiNhanVien'));
                            break;
                        case 18:
                            self.PhieuDieuChinh_ThayDoiThoiGian(CheckQuyenExist('PhieuDieuChinh_ThayDoiThoiGian'));
                            self.PhieuDieuChinh_ThayDoiNhanVien(CheckQuyenExist('PhieuDieuChinh_ThayDoiNhanVien'));
                            break;
                    }
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

    function newCTHD(itemHH) {
        let idRandomHD = self.newHoaDon().IDRandom();
        let lotParent = itemHH.QuanLyTheoLoHang ? true : false;

        var ngaysx = moment(itemHH.NgaySanXuat).format('DD/MM/YYYY');
        var hethan = moment(itemHH.NgayHetHan).format('DD/MM/YYYY');
        if (ngaysx === 'Invalid date') {
            ngaysx = '';
        }

        if (hethan === 'Invalid date') {
            hethan = '';
        }

        let dongia = 0, thanhtien = formatNumberToFloat(itemHH.ThanhTien);
        switch (self.LoaiHoaDonMenu) {
            case 16:
                dongia = formatNumberToFloat(itemHH.GiaVonTieuChuan);
                break;
            case 18:
                dongia = itemHH.GiaVon;
                thanhtien = 0;
                break;
        }
        return {
            ID_HangHoa: itemHH.ID_HangHoa,
            SoThuTu: 1,
            IDRandom: CreateIDRandom('CTHD_'),
            IDRandomHD: idRandomHD,
            ID_DonViQuiDoi: itemHH.ID_DonViQuiDoi,
            MaHangHoa: itemHH.MaHangHoa,
            TenHangHoa: itemHH.TenHangHoa,
            TenDonViTinh: itemHH.TenDonViTinh,
            DonViTinh: [],
            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
            QuanLyTheoLoHang: itemHH.QuanLyTheoLoHang,
            TonKho: itemHH.TonKho,
            DonGia: dongia,
            GiaVon: itemHH.GiaVon,
            SoLuong: 0,
            TienChietKhau: 0,
            ThanhTien: thanhtien,
            ThanhToan: 0,
            GhiChu: '',
            ID_LoHang: itemHH.ID_LoHang,
            MaLoHang: itemHH.MaLoHang,
            NgaySanXuat: ngaysx,
            NgayHetHan: hethan,
            DM_LoHang: [],
            HangCungLoais: [],
            LotParent: lotParent,
            LaConCungLoai: false,
            PTThue: 0,
            TienThue: 0,
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
            DonGia: itemHH.GiaVon,
            GiaVon: itemHH.GiaVon,
            SoLuong: 0,
            TienChietKhau: 0,
            ThanhTien: 0,
            ThanhToan: 0,
            GhiChu: '',
            LotParent: parent ? true : false,
            LaConCungLoai: false,
            QuanLyTheoLoHang: true,
            NguoiTao: _userLogin,// used to save DB
            TenHangHoa: itemHH.TenHangHoa,
            MaHangHoa: itemHH.MaHangHoa,
            TenDonViTinh: itemHH.TenDonViTinh,
            DonViTinh: [],
            ThuocTinh_GiaTri: itemHH.ThuocTinh_GiaTri,
            PTThue: 0,
            TienThue: 0,
        }
    }

    function FindCTHD_isDoing(item) {
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var concungloai = item.LaConCungLoai;
        var idRandom = item.IDRandom;

        var lstCTHD = localStorage.getItem(lcCTDieuChinh);
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

    function updateCTHDLe(arr, ctDoing) {
        var quanlytheolo = ctDoing.QuanLyTheoLoHang;
        var concungloai = ctDoing.LaConCungLoai;
        var lotParent = ctDoing.LotParent;
        var idRandom = ctDoing.IDRandom;

        var newDonGia = ctDoing.DonGia;
        var tienGiam = ctDoing.TienChietKhau;
        var ptGiam = ctDoing.PTChietKhau;

        if (lotParent || (concungloai === false && quanlytheolo === false)) {
            for (let i = 0; i < arr.length; i++) {
                if (arr[i].IDRandom === idRandom) {
                    arr[i].DonGia = newDonGia;
                    arr[i].PTChietKhau = ptGiam;
                    arr[i].TienChietKhau = tienGiam;

                    if (lotParent) {
                        arr[i].DM_LoHang[0].DonGia = newDonGia;
                        arr[i].DM_LoHang[0].PTChietKhau = ptGiam;
                        arr[i].DM_LoHang[0].TienChietKhau = tienGiam;
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
                            arr[i].DM_LoHang[j].DonGia = newDonGia;
                            arr[i].DM_LoHang[j].PTChietKhau = ptGiam;
                            arr[i].DM_LoHang[j].TienChietKhau = tienGiam;
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
                            arr[i].HangCungLoais[j].DonGia = newDonGia;
                            arr[i].HangCungLoais[j].PTChietKhau = ptGiam;
                            arr[i].HangCungLoais[j].TienChietKhau = tienGiam;
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

    self.JqAutoSelectItem = function (itemChose) {
        console.log(3, itemChose)

        if (commonStatisJs.CheckNull(itemChose.ID)) {
            return;
        }

        itemChose.ID_HangHoa = itemChose.ID;

        let arrLo = [];
        if (!commonStatisJs.CheckNull(itemChose.ID_LoHang)) {
            arrLo = itemChose.ID_LoHang;
        }
        let ngayKK = moment($('#datetimepicker_mask').val(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        let obj = {
            ID_ChiNhanh: _idDonVi,
            ToDate: ngayKK,
            ListIDQuyDoi: [itemChose.ID_DonViQuiDoi],
            ListIDLoHang: arrLo,
        }
        ajaxHelper(DMHangHoaUri + 'GetTonKho_byIDQuyDois', 'POST', obj).done(function (x) {
            if (x.res) {
                if (x.data.length > 0) {
                    itemChose.GiaVonTieuChuan = x.data[0].GiaVonTieuChuan;
                    itemChose.TonKho = x.data[0].TonKho;
                    itemChose.GiaVon = x.data[0].GiaVon;

                    switch (self.LoaiHoaDonMenu) {
                        case 16:
                            itemChose.ThanhTien = itemChose.GiaVonTieuChuan;
                            break;
                    }
                }
            }
            self.ItemChosing(itemChose);
            AddCTHD(itemChose, 1);
        })
    }

    function CreateNewHoaDon() {
        var idNhanVien = self.newHoaDon().ID_NhanVien();
        if (commonStatisJs.CheckNull(idNhanVien)) {
            idNhanVien = _idNhanVien;
        }
        return {
            ID: const_GuidEmpty,
            IDRandom: CreateIDRandom('HD_'),
            LoaiHoaDon: self.LoaiHoaDonMenu,
            MaHoaDon: '',
            ID_DonVi: _idDonVi,
            ID_NhanVien: idNhanVien,
            NguoiTao: VHeader.UserLogin,
            NgayLapHoaDon: null,
            TongTienHang: 0,
            TongTienThue: 0,
            TongGiamGia: 0,
            TongChietKhau: 0,
            PhaiThanhToan: 0,
            TongThanhToan: 0,
        };
    }

    function AddCTHD(item) {
        let hd = localStorage.getItem(lcHDDieuChinh);
        if (hd !== null) {
            hd = JSON.parse(hd);
        }
        else {
            hd = [];
        }

        var cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
        }
        else {
            cthd = [];
        }

        let idRandomHD = self.newHoaDon().IDRandom();
        if (commonStatisJs.CheckNull(idRandomHD)) {
            let obj = CreateNewHoaDon();
            idRandomHD = obj.IDRandom;
            hd.push(obj);
            localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
            self.newHoaDon().IDRandom(idRandomHD);
        }

        var itemEx = $.grep(cthd, function (x) {
            return x.IDRandomHD === idRandomHD && x.ID_DonViQuiDoi === item.ID_DonViQuiDoi;
        });
        if (itemEx.length > 0) {
            let idRandom = itemEx[0].IDRandom;

            if (itemEx[0].QuanLyTheoLoHang) {
                let exLo = $.grep(itemEx[0].DM_LoHang, function (x) {
                    return x.ID_LoHang === item.ID_LoHang;
                });
                if (exLo.length > 0) {
                    // delete & add again
                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].IDRandom === idRandom) {
                            cthd.splice(i, 1);
                            break;
                        }
                    }
                    cthd.unshift(itemEx[0]);
                }
                else {
                    // add newlo
                    let obj = newLot(item);
                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].IDRandom === idRandom) {
                            cthd[i].DM_LoHang.push(obj);
                            break;
                        }
                    }
                }
            }
            else {
                // delete & add again
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].IDRandom === idRandom) {
                        cthd.splice(i, 1);
                        break;
                    }
                }
                cthd.unshift(itemEx[0]);
            }
        }
        else {
            let newCT = newCTHD(item);
            if (item.QuanLyTheoLoHang) {
                let objLo = newLot(newCT, true);
                newCT.DM_LoHang.push(objLo);
            }
            cthd.unshift(newCT);
        }
        localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));

        UpdateSoThuTu_CTHD();

        Bind_UpdateHD();
        BindCTHD_byIDRandomHD();
        BindHD_byIDRandom();
    }

    function UpdateSoThuTu_CTHD() {
        let idRandomHD = self.newHoaDon().IDRandom();
        let cthd = localStorage.getItem(lcCTDieuChinh);
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
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));
        }
    }

    self.editGiaVon = function (item) {
        let $this = $(event.currentTarget);
        formatNumberObj($this);
        let gtri = formatNumberToFloat($this.val());

        let idRandom = item.IDRandom;
        let cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === idRandom) {
                    let gvOld = formatNumberToFloat(cthd[i].DonGia);
                    let chenhlech = gtri - gvOld;
                    if (chenhlech < 0) {
                        cthd[i].TienChietKhau = chenhlech;// lechgiam
                        cthd[i].PTChietKhau = 0;
                    }
                    else {
                        cthd[i].PTChietKhau = chenhlech;// lechtang
                        cthd[i].TienChietKhau = 0;
                    }
                    switch (self.LoaiHoaDonMenu) {
                        case 16:
                            cthd[i].ThanhTien = formatNumberToFloat(gtri);
                            break;
                        case 18:
                            cthd[i].GiaVon = formatNumberToFloat(gtri);
                            break;
                    }
                    $('#GiaVonChenhLech_' + idRandom).html(formatNumber3Digit(chenhlech));
                    break;
                }
            }
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));
        }

        Enter_CTHD(item, event, 'GiaVon_');
    }

    function Enter_CTHD(itemCT, e, charStart) {
        var key = e.keyCode || e.which;

        if (key === 13) {
            var cthd = JSON.parse(localStorage.getItem(lcCTDieuChinh));
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

    self.deleteChiTietHD = function (item) {
        let cthd = localStorage.getItem(lcCTDieuChinh);
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
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));

            Bind_UpdateHD();
            BindCTHD_byIDRandomHD();
            BindHD_byIDRandom();
        }
    }

    self.UpdateGhiChu_CTHD = function (item) {
        var idRandom = item.IDRandom;
        var quanlyTheoLo = item.QuanLyTheoLoHang;
        var ghichu = $(event.currentTarget).val();
        var concungloai = item.LaConCungLoai;

        var lcCTHD = localStorage.getItem(lcCTDieuChinh);
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
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(lcCTHD));
        }
    }

    self.ClickThemLo = function (item) {
        let quanlytheolo = item.QuanLyTheoLoHang;
        let idQuiDoi = item.ID_DonViQuiDoi;
        let idRandomHD = self.newHoaDon().IDRandom();

        let cthd = localStorage.getItem(lcCTDieuChinh);
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
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));

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

        var cthd = localStorage.getItem(lcCTDieuChinh);
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
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));
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
        let idRandom = null;
        var $thisID = $($this.closest('ul')).attr('id');
        if (!commonStatisJs.CheckNull($thisID)) {
            idRandom = $thisID.replace('month-oll_', '');
        }
        console.log('idRandom ', idRandom)

        let dongia = 0, thanhtien = 0;
        switch (self.LoaiHoaDonMenu) {
            case 16:
                dongia = item.GiaVonTieuChuan;
                thanhtien = item.GiaVonTieuChuan;
                break;
            case 18:
                dongia = itemHH.GiaVon;
                thanhtien = 0;
                break;
        }

        // update TonKho, GiaVon, GiaBan, Lo
        var cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                    let itFor = cthd[i].DM_LoHang[j];
                    if (cthd[i].IDRandom === idRandom) {
                        cthd[i].ID_LoHang = idLoHang;
                        cthd[i].MaLoHang = maLoHang;
                        cthd[i].NgaySanXuat = ngaysx;
                        cthd[i].NgayHetHan = hethan;
                        cthd[i].DonGia = dongia;
                        cthd[i].ThanhTien = thanhtien;
                        cthd[i].GiaVon = item.GiaVon;
                        cthd[i].TienChietKhau = 0;
                        cthd[i].PTChietKhau = 0;
                    }
                    if (itFor.IDRandom === idRandom) {
                        cthd[i].DM_LoHang[j].ID_LoHang = idLoHang;
                        cthd[i].DM_LoHang[j].MaLoHang = maLoHang;
                        cthd[i].DM_LoHang[j].NgaySanXuat = ngaysx;
                        cthd[i].DM_LoHang[j].NgayHetHan = hethan;
                        cthd[i].DM_LoHang[j].DonGia = dongia;
                        cthd[i].DM_LoHang[j].ThanhTien = thanhtien;
                        cthd[i].DM_LoHang[j].GiaVon = item.GiaVon;
                        cthd[i].DM_LoHang[j].TienChietKhau = 0;
                        cthd[i].DM_LoHang[j].PTChietKhau = 0;
                        i = cthd.length;
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));

            Bind_UpdateHD();
            BindCTHD_byIDRandomHD();
            BindHD_byIDRandom();
        }
        self.ListLot_ofProduct([]);
    }

    self.sLoaiChungTu = ko.computed(function () {
        let txt = '';
        switch (parseInt(self.newHoaDon().LoaiHoaDon())) {
            case 16:
                txt = 'Giá vốn tiêu chuẩn';
                break;
            case 18:
                txt = 'Điều chỉnh giá vốn';
                break;
        }
        return txt;
    })

    self.textSearch = ko.observable();
    self.indexFocus = ko.observable(0);

    self.SearchNhanVien = function () {
        var self = this;
        var txt = locdau(self.textSearch());
        var keyCode = event.keyCode;

        if ($.inArray(keyCode, [13, 38, 40]) === -1) {
            let arr = [];
            if (txt === '') {
                // reset nhanvien if clear
                var hd = localStorage.getItem(lcHDDieuChinh);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    hd[0].ID_NhanVien = null;
                    localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
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
        let hd = localStorage.getItem(lcHDDieuChinh);
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
        localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
    }

    self.editMaHoaDon = function () {
        let $this = $(event.currentTarget);
        let exHD = false;
        let hd = localStorage.getItem(lcHDDieuChinh);
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
        localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
    }

    self.UpdateGhiChuHD = function () {
        var hd = localStorage.getItem(lcHDDieuChinh);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === self.newHoaDon().IDRandom()) {
                    hd[i].DienGiai = $(event.currentTarget).val();
                    break;
                }
            }
            localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
        }
    }

    self.OnOffPrint = function () {
        self.turnOnPrint(!self.turnOnPrint())
        localStorage.setItem('CheckInWhenHT', self.turnOnPrint());
    }

    function Check_OnOffPrint() {
        var isPrint = localStorage.getItem('CheckInWhenHT');
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

    $('#datetimepicker_mask').datetimepicker({
        timepicker: true,
        mask: true,
        format: 'd/m/Y H:i',
        maxDate: new Date(),
        onChangeDateTime: function (dp, $input) {
            let ok = CheckNgayLapHD_format($input.val());
            if (ok) {
                let hd = localStorage.getItem(lcHDDieuChinh);
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
                localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));

                UpdateTonKhoGiaVon_byIDQuyDois(idRandomHD);
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

    function Bind_UpdateHD() {
        let hd = self.newHoaDon();
        let idRandomHD = hd.IDRandom();
        let tongGVmoi = 0, tongGVcu = 0;

        let cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            for (let i = 0; i < cthd.length; i++) {
                let itFor = cthd[i];
                if (itFor.IDRandomHD === idRandomHD) {
                    tongGVmoi += formatNumberToFloat(itFor.GiaVon);
                    tongGVcu += formatNumberToFloat(itFor.DonGia);

                    for (let j = 1; j < itFor.DM_LoHang.length; j++) {
                        let forLot = itFor.DM_LoHang[j];
                        tongGVmoi += formatNumberToFloat(forLot.GiaVon);
                        tongGVcu += formatNumberToFloat(forLot.DonGia);
                    }
                }
            }
        }

        let lstHD = localStorage.getItem(lcHDDieuChinh);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
        }
        else {
            lstHD = [];
        }

        for (let i = 0; i < lstHD.length; i++) {
            let itFor = lstHD[i];
            if (itFor.IDRandom === idRandomHD) {
                lstHD[i].TongTienThue = tongGVmoi;
                lstHD[i].TongChietKhau = tongGVcu;
                break;
            }
        }
        localStorage.setItem(lcHDDieuChinh, JSON.stringify(lstHD));
    }


    function Enable_btnSave() {
        self.isLoadding(false);
    }

    self.SaveInvoice = function (status) {
        var cthd = localStorage.getItem(lcCTDieuChinh);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            let idRandom = self.newHoaDon().IDRandom();

            // only get hd current
            cthd = $.grep(cthd, function (x) {
                return x.IDRandomHD === idRandom;
            });

            if (cthd.length > 0) {
                let arrCT = [];
                // assign TienChietKhau, PTChietKhau
                switch (self.LoaiHoaDonMenu) {
                    case 16:
                        for (let i = 0; i < cthd.length; i++) {
                            let itOut = cthd[i];
                            itOut.SoThuTu = arrCT.length + 1;
                            let chenhlech = formatNumberToFloat(arrCT.ThanhTien) - formatNumberToFloat(arrCT.DonGia);
                            if (chenhlech > 0) {
                                itOut.PTChietKhau = chenhlech;
                                itOut.TienChietKhau = 0;
                            }
                            else {
                                itOut.PTChietKhau = 0;
                                itOut.TienChietKhau = chenhlech;
                            }
                            arrCT.push(itOut);

                            for (let j = 1; j < itOut.DM_LoHang.length; j++) {
                                let itFor = itOut.DM_LoHang[j];
                                let chenhlech2 = formatNumberToFloat(itFor.ThanhTien) - formatNumberToFloat(itFor.DonGia);
                                if (chenhlech2 > 0) {
                                    itFor.PTChietKhau = chenhlech2;
                                    itFor.TienChietKhau = 0;
                                }
                                else {
                                    itFor.PTChietKhau = 0;
                                    itFor.TienChietKhau = chenhlech2;
                                }
                                arrCT.push(itFor);
                            }
                        }
                        break;
                    case 18:
                        for (let i = 0; i < cthd.length; i++) {
                            let itOut = cthd[i];
                            itOut.SoThuTu = arrCT.length + 1;
                            let chenhlech = formatNumberToFloat(arrCT.GiaVon) - formatNumberToFloat(arrCT.DonGia);
                            if (chenhlech > 0) {
                                itOut.PTChietKhau = chenhlech;
                                itOut.TienChietKhau = 0;
                            }
                            else {
                                itOut.PTChietKhau = 0;
                                itOut.TienChietKhau = chenhlech;
                            }
                            arrCT.push(itOut);

                            for (let j = 1; j < itOut.DM_LoHang.length; j++) {
                                let itFor = itOut.DM_LoHang[j];
                                let chenhlech2 = formatNumberToFloat(itFor.GiaVon) - formatNumberToFloat(itFor.DonGia);
                                if (chenhlech2 > 0) {
                                    itFor.PTChietKhau = chenhlech2;
                                    itFor.TienChietKhau = 0;
                                }
                                else {
                                    itFor.PTChietKhau = 0;
                                    itFor.TienChietKhau = chenhlech2;
                                }
                                arrCT.push(itFor);
                            }
                        }
                        break;
                }

                for (let i = 0; i < arrCT.length; i++) {
                    delete arrCT[i]["DM_LoHang"];
                }

                let checkDate = CheckNgayLapHD_format($('#datetimepicker_mask').val());
                if (!checkDate) {
                    Enable_btnSave();
                    return false;
                }

                let objHD = [];
                var hd = localStorage.getItem(lcHDDieuChinh);
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

                objHD[0].NgayLapHoaDon = GetNgayLapHD_withTimeNow(objHD[0].NgayLapHoaDon);
                objHD[0].ChoThanhToan = status === 0 ? false : true;
                objHD[0].ID_NhanVien = self.newHoaDon().ID_NhanVien();
                objHD[0].MaHoaDon = self.newHoaDon().MaHoaDon();
                objHD[0].DienGiai = self.newHoaDon().DienGiai();

                self.isLoadding(true);

                let myData = {
                    objHoaDon: objHD[0],
                    objCTHoaDon: cthd,
                };

                var idHoaDon = myData.objHoaDon.ID;
                if (!commonStatisJs.CheckNull(idHoaDon) && idHoaDon !== const_GuidEmpty) {
                    Put_PhieuDieuChinh(myData);
                }
                else {
                    Post_PhieuDieuChinh(myData);
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
                GotoPageListPhieuDieuChinh();
            });
        }
        else {
            RemoveCache(self.newHoaDon().IDRandom());
            GotoPageListPhieuDieuChinh();
        }
    }

    function GotoPageListPhieuDieuChinh() {
        switch (self.LoaiHoaDonMenu) {
            case 16:
                window.location.href = '/#/PurchaseOrder';
                break;
            case 18:
                window.location.href = '/#/CouponAdjustment';
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

    function ResetInforHD() {
        self.HangHoaAfterAdd([]);
        self.newHoaDon(new FormModel_NewHoaDon());
        self.textSearch('');
        let arrNV = self.NhanViens().slice(0, 20);
        self.ListNVienSearch(arrNV);
    }

    function RemoveCache(idRandom = null) {
        if (!commonStatisJs.CheckNull(idRandom)) {
            let hd = localStorage.getItem(lcHDDieuChinh);
            if (hd !== null) {
                hd = JSON.parse(hd);
                hd = $.grep(hd, function (x) {
                    return x.IDRandom !== idRandom;
                });
                localStorage.setItem(lcHDDieuChinh, JSON.stringify(hd));
            }
            let cthd = localStorage.getItem(lcCTDieuChinh);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = $.grep(cthd, function (x) {
                    return x.IDRandomHD !== idRandom;
                });
                localStorage.setItem(lcCTDieuChinh, JSON.stringify(cthd));
            }
        }
        else {
            localStorage.removeItem(lcHDDieuChinh);
            localStorage.removeItem(lcCTDieuChinh);
        }
    }

    function Post_PhieuDieuChinh(myData) {
        console.log('myData ', myData)
        $.ajax({
            data: myData,
            url: BH_DieuChinhUri + 'Post_PhieuDieuChinh?idNhanVien=' + _idNhanVien,
            type: 'POST',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        }).done(function (x) {
            if (x.res === true) {
                RemoveCache(myData.objHoaDon.IDRandom);
                ShowMessage_Success('Tạo phiếu điều chỉnh thành công');

                let dataDB = x.data;
                myData.objHoaDon.MaHoaDon = dataDB.MaHoaDon;
                myData.objHoaDon.ID = dataDB.ID;
                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
                ResetInforHD();
            }
            else {
                ShowMessage_Danger(x.mes);
            }
        }).always(function () {
            Enable_btnSave();
        });
    }

    function Put_PhieuDieuChinh(myData) {
        console.log('Update_PhieuDieuChinh ', myData)
        ajaxHelper(BH_DieuChinhUri + 'Update_PhieuDieuChinh?idNhanVien=' + _idNhanVien, 'post', myData).done(function (x) {
            if (x.res === true) {
                RemoveCache(myData.objHoaDon.IDRandom);
                ShowMessage_Success('Cập nhật phiếu điều chỉnh thành công');

                let dataDB = x.data;
                myData.objHoaDon.MaHoaDon = dataDB.MaHoaDon;
                myData.objHoaDon.ID = dataDB.ID;
                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
                ResetInforHD();
            }
            else {
                ShowMessage_Danger(x.mes);
            }
        }).always(function () {
            Enable_btnSave();
        });
    }

    self.InHoaDon = function (cthd, hd) {
        if (!self.turnOnPrint()) {
            return;
        }
        var cthdFormat = GetCTHDPrint_Format(cthd);
        self.CTHoaDonPrint(cthdFormat);

        var itemHDFormat = GetInforHDPrint(hd);
        self.InforHDprintf(itemHDFormat);

        ajaxHelper('/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=DCGV&idDonVi=' + _idDonVi, 'GET').done(function (result) {
            var data = '<script src="/Scripts/knockout-3.4.2.js"></script>';
            data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                + "; var item4 =[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
            data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
            PrintExtraReport(result, data, self.numbersPrintHD(), 0);
        });
    }

    function GetCTHDPrint_Format(arrCTHD) {
        switch (self.LoaiHoaDonMenu) {
            case 16:
                for (var i = 0; i < arrCTHD.length; i++) {
                    arrCTHD[i].SoThuTu = i + 1;
                    arrCTHD[i].DonGia = formatNumber3Digit(arrCTHD[i].DonGia);
                    arrCTHD[i].GiaVon = formatNumber3Digit(arrCTHD[i].GiaVon);
                    arrCTHD[i].GiaVonHienTai = formatNumber3Digit(arrCTHD[i].DonGia);
                    arrCTHD[i].GiaVonMoi = formatNumber3Digit(arrCTHD[i].ThanhTien);
                    arrCTHD[i].TienChietKhau = formatNumber3Digit(arrCTHD[i].TienChietKhau);
                    arrCTHD[i].ThanhTien = formatNumber3Digit(arrCTHD[i].ThanhTien);
                    arrCTHD[i].ChenhLech = formatNumber3Digit(arrCTHD[i].TienChietKhau);
                }
                break;
            case 18:
                for (var i = 0; i < arrCTHD.length; i++) {
                    arrCTHD[i].SoThuTu = i + 1;
                    arrCTHD[i].DonGia = formatNumber3Digit(arrCTHD[i].DonGia);
                    arrCTHD[i].GiaVon = formatNumber3Digit(arrCTHD[i].GiaVon);
                    arrCTHD[i].GiaVonHienTai = formatNumber3Digit(arrCTHD[i].DonGia);
                    arrCTHD[i].GiaVonMoi = formatNumber3Digit(arrCTHD[i].GiaVon);
                    arrCTHD[i].TienChietKhau = formatNumber3Digit(arrCTHD[i].TienChietKhau);
                    arrCTHD[i].ThanhTien = formatNumber3Digit(arrCTHD[i].ThanhTien);
                    arrCTHD[i].ChenhLech = formatNumber3Digit(arrCTHD[i].TienChietKhau, 2);
                }
                break;
        }
        return arrCTHD;
    }

    function GetInforHDPrint(objHD) {
        var hdPrint = $.extend({}, objHD);

        let ngaylap = hdPrint.NgayLapHoaDon;
        hdPrint.NgayLapHoaDon = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
        hdPrint.Ngay = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('DD');
        hdPrint.Thang = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('MM');
        hdPrint.Nam = moment(ngaylap, 'YYYY-MM-DD HH:mm:ss').format('YYYY');
        hdPrint.TongTienHang = formatNumber3Digit(hdPrint.TongTienHang, 2);
        hdPrint.TongTienThue = formatNumber3Digit(hdPrint.TongTienThue, 2);
        hdPrint.TongGiamGia = formatNumber3Digit(hdPrint.TongGiamGia, 2);

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
        if (self.HangHoaAfterAdd().length > 0) {
            var hd = localStorage.getItem(lcHDDieuChinh);
            if (hd !== null) {
                hd = JSON.parse(hd);
                // neu click ben ngoai modal delete --> bind hd
                self.newHoaDon().SetData(hd[0]);
                $('#selectedNV').val(hd[0].ID_NhanVien);
            }
        }
    });

    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangDieuChinh.xls";
        window.location.href = url;
    }

    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachHangDieuChinh.xlsx";
        window.location.href = url;
    }

    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();
    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadForm').value = "";
    }
    self.refreshFileSelect = function () {
        self.importDieuChinh();
    }
    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        self.loiExcel([]);
    }

    self.importDieuChinh = function () {
        $('#divPage').gridLoader({ show: true });

        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: BH_DieuChinhUri + "importExcelDieuChinh",
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
        }).done(function (item) {
            self.loiExcel(item);
            if (self.loiExcel().length > 0) {
                $(".BangBaoLoi").show();
                $(".btnImportExcel").hide();
                $(".refreshFile").show();
                $(".deleteFile").hide();
            }
            else {
                $.ajax({
                    type: "POST",
                    url: BH_DieuChinhUri + "getList_DanhSachHangDieuChinh?ID_ChiNhanh=" + _idDonVi,
                    data: formData,
                    dataType: 'json',
                    contentType: false,
                    processData: false,
                }).done(function (item) {
                    switch (self.LoaiHoaDonMenu) {
                        case 16:
                            for (let i = 0; i < item.length; i++) {
                                let itFor = item[i];
                                itFor.ThanhTien = itFor.GiaVonMoi;
                                AddCTHD(itFor);
                            }
                            break;
                    }
                    UpdateTonKhoGiaVon_byIDQuyDois();

                }).fail(function () {
                }).always(function () {
                    $('#divPage').gridLoader({ show: false });
                })
            }
        }).always(function (x) {
            $(".btnImportExcel").hide();
            $(".filterFileSelect").hide();
        });
    }

    self.PageResult_CTHoaDons = ko.computed(function (item) {
        // filter
        let filter = self.filterHangHoa_ChiTietHD();
        let arrFilter = ko.utils.arrayFilter(self.HangHoaAfterAdd(), function (prod) {
            let chon = true;
            let ipLodau = locdau(filter);
            let maHH = locdau(prod.MaHangHoa);
            let tenHH = locdau(prod.TenHangHoa);
            let maLoHang = locdau(prod.MaLoHang);
            let kitudau = GetChartStart(tenHH);
            if (chon && filter) {
                chon = maHH.indexOf(ipLodau) > -1 || tenHH.indexOf(ipLodau) > -1
                    || maLoHang.indexOf(ipLodau) > -1 || kitudau.indexOf(ipLodau) > -1
                    ;
            }
            return chon;
        });
        let lenData = arrFilter.length;
        self.PageCount_CTHD(Math.ceil(lenData / self.PageSize_CTHD()));
        self.TotalRecord_CTHD(lenData);
        // paging
        let first = self.currentPage_CTHD() * self.PageSize_CTHD();
        if (arrFilter !== null) {
            return arrFilter.slice(first, first + self.PageSize_CTHD());
        }
    })
    self.PageList_CTHD = ko.computed(function () {
        let arrPage = [];
        let allPage = self.PageCount_CTHD();
        let currentPage = self.currentPage_CTHD();
        if (allPage > 4) {
            let i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage_CTHD()) + 1;
            }
            else {
                i = self.currentPage_CTHD();
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
                    if (currentPage === 1) {
                        for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            let obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
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
        if (self.PageResult_CTHoaDons().length > 0) {
            self.fromitem_CTHD((self.currentPage_CTHD() * self.PageSize_CTHD()) + 1);
            if (((self.currentPage_CTHD() + 1) * self.PageSize_CTHD()) > self.PageResult_CTHoaDons().length) {
                let ss = (self.currentPage_CTHD() + 1) * self.PageSize_CTHD();
                let fromItem = (self.currentPage_CTHD() + 1) * self.PageSize_CTHD();
                if (fromItem < self.TotalRecord_CTHD()) {
                    self.toitem_CTHD((self.currentPage_CTHD() + 1) * self.PageSize_CTHD());
                }
                else {
                    self.toitem_CTHD(self.TotalRecord_CTHD());
                }
            } else {
                self.toitem_CTHD((self.currentPage_CTHD() * self.PageSize_CTHD()) + self.PageSize_CTHD());
            }
        }
        return arrPage;
    });
    self.VisibleStartPage_CTHD = ko.computed(function () {
        if (self.PageList_CTHD().length > 0) {
            return self.PageList_CTHD()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPage_CTHD = ko.computed(function () {
        if (self.PageList_CTHD().length > 0) {
            return self.PageList_CTHD()[self.PageList_CTHD().length - 1].pageNumber !== self.PageCount_CTHD();
        }
    })
    self.ResetCurrentPage_CTHD = function () {
        self.currentPage_CTHD(0);
    };
    self.GoToPage_CTHD = function (page) {
        self.currentPage_CTHD(page.pageNumber - 1);
    };
    self.GetClass_CTHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage_CTHD()) ? "click" : "";
    };
    self.StartPage_CTHD = function () {
        self.currentPage_CTHD(0);
    }
    self.BackPage_CTHD = function () {
        if (self.currentPage_CTHD() > 1) {
            self.currentPage_CTHD(self.currentPage_CTHD() - 1);
        }
    }
    self.GoToNextPage_CTHD = function () {
        if (self.currentPage_CTHD() < self.PageCount_CTHD() - 1) {
            self.currentPage_CTHD(self.currentPage_CTHD() + 1);
        }
    }
    self.EndPage_CTHD = function () {
        if (self.currentPage_CTHD() < self.PageCount_CTHD() - 1) {
            self.currentPage_CTHD(self.PageCount_CTHD() - 1);
        }
    }

    self.showPopNhomHang = function () {
        vmApplyGroupProduct.showModal();
    }

    $('#vmApplyGroupProduct').on('hidden.bs.modal', function () {
        if (vmApplyGroupProduct.saveOK) {
            // get all product by nhom
            let param = {
                ID_Donvi: _idDonVi,
                IDNhomHangs: vmApplyGroupProduct.arrIDNhomChosed,
                LoaiHangHoas: '1',// chi get hanghoa
            }
            ajaxHelper(BH_DieuChinhUri + 'getListHangHoaBy_IDNhomHang', 'POST', param).done(function (x) {
                if (x.res) {
                    let data = x.dataSoure;
                    switch (self.LoaiHoaDonMenu) {
                        case 16:
                            for (let i = 0; i < data.length; i++) {
                                let itFor = data[i];
                                itFor.ThanhTien = itFor.GiaVonMoi;
                                AddCTHD(itFor);
                            }
                            break;
                    }
                    UpdateTonKhoGiaVon_byIDQuyDois();
                }
            })
        }
    })
}
var modelGiaoDich = new PhieuDieuChinhChiTiet();
ko.applyBindings(modelGiaoDich, document.getElementById('divPage'));
