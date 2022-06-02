var FormModel_NewHoaDon = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_HoaDon = ko.observable();
    self.MaHoaDon = ko.observable();
    self.ID_DonVi = ko.observable();
    self.ID_DoiTuong = ko.observable();
    self.TongTienHang = ko.observable();
    self.TongGiamGia = ko.observable(0); // tonggiamgia
    self.TongChietKhau = ko.observable(0); // % giamgia
    self.ChoThanhToan = ko.observable(true);
    self.PTThueHoaDon = ko.observable(0);
    self.TongTienThue = ko.observable(0);
    self.TongChiPhi = ko.observable(0);
    self.PTChiPhi = ko.observable(0);
    self.TongThanhToan = ko.observable(0);

    self.PhaiThanhToan = ko.observable(0);
    self.NgayLapHoaDon = ko.observable(moment(new Date()).format('DD/MM/YYYY HH:mm'));
    self.ID_NhanVien = ko.observable();
    self.NguoiTao = ko.observable(VHeader.UserLogin);
    self.DienGiai = ko.observable();
    self.LoaiHoaDon = loaiHoaDon;
    self.TenDoiTuong = ko.observable();
    self.DaThanhToan = ko.observable(0);
    self.KhachDaTra = ko.observable(0);// tien da tra truoc khi tamluu
    self.ConThieu = ko.observable();
    self.ID_TaiKhoanNganHang = ko.observable();
    self.TienMat = ko.observable(0);
    self.TienChuyenKhoan = ko.observable(0);
    self.TienPOS = ko.observable(0);
    self.TienDatCoc = ko.observable(0);
    self.ID_TaiKhoanPos = ko.observable();
    self.TenTaiKhoanPos = ko.observable();
    self.ID_TaiKhoanChuyenKhoan = ko.observable();
    self.TenTaiKhoanCK = ko.observable();
    self.ID_KhoanThuChi = ko.observable();
    self.HoanTra = ko.observable(0);

    self.SetData = function (item) {
        self.ID(item.ID);
        self.ID_HoaDon(item.ID_HoaDon);
        self.ID_DonVi(item.ID_DonVi);
        self.ID_DoiTuong(item.ID_DoiTuong);
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
        self.ID_TaiKhoanNganHang(item.ID_TaiKhoanNganHang);
        self.TienMat(item.TienMat);
        self.TienChuyenKhoan(item.TienChuyenKhoan);
        self.PTThueHoaDon(item.PTThueHoaDon);
        self.TongTienThue(item.TongTienThue);
        self.PTChiPhi(item.PTChiPhi);
        self.TongChiPhi(item.TongChiPhi);
        self.TongThanhToan(item.TongThanhToan);
        self.ID_KhoanThuChi(item.ID_KhoanThuChi);

        if (commonStatisJs.CheckNull(item.HoanTra)) {
            self.HoanTra(0);
        }
        else {
            self.HoanTra(item.HoanTra);
        }
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
    }
}

var TraHangNhapChiTiet = function () {
    var self = this;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DMDoiTuongUri = "/api/DanhMuc/DM_DoiTuongAPI/";
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var lcCTTraHangNhap = 'lcCTTraHangNhap';
    var lcHDTraHangNhap = 'lcHDTraHangNhap';
    $(".btnImportExcel").hide();
    $(".filterFileSelect").hide();

    var _idDonVi = VHeader.IdDonVi;
    var _tenDonVi = VHeader.TenDonVi;
    var _userLogin = VHeader.UserLogin;
    var _idNhanVien = VHeader.IdNhanVien;
    var _IDNguoiDung = VHeader.IdNguoiDung;

    vmThemMoiNCC.inforLogin = {
        ID_NhanVien: VHeader.IdNhanVien,
        ID_User: VHeader.IdNguoiDung,
        UserLogin: VHeader.UserLogin,
        ID_DonVi: VHeader.IdDonVi,
        TenNhanVien: VHeader.TenNhanVien,
    };
    vmThemMoiNhomNCC.inforLogin = vmThemMoiNCC.inforLogin;

    self.newHoaDon = ko.observable(new FormModel_NewHoaDon());
    self.NhanViens = ko.observableArray();
    self.DoiTuong = ko.observableArray();
    self.NhomDoiTuongs = ko.observableArray();
    self.HangHoaAfterAdd = ko.observableArray();
    self.ChiTietDoiTuong = ko.observableArray();
    self.HangHoas = ko.observableArray();
    self.ListLot_ofProduct = ko.observableArray();
    self.ListLot_ofProductAll = ko.observableArray();
    self.CongTy = ko.observableArray();
    self.ChiNhanh = ko.observableArray();
    self.DoiTuong_Old = ko.observableArray();
    self.TaiKhoansPOS = ko.observableArray();
    self.TaiKhoansCK = ko.observableArray();
    self.IsNhapNhanh = ko.observable(true);
    self.ItemChosing = ko.observable();
    self.loiExcel = ko.observableArray();
    self.numbersPrintHD = ko.observable(1);
    self.ConTonKho = ko.observable(0);
    self.CTHoaDonPrint = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    self.ThietLap = ko.observableArray();

    self.selectedHH = ko.observable();
    self.filterNH = ko.observable();
    self.selectIDNHCK = ko.observable();
    self.arrFilterTaiKhoanCK = ko.observableArray();
    self.fileNameExcel = ko.observable();
    self.isLoading = ko.observable(false);

    self.soLuongMatHang = ko.observable(0);
    self.TongSoLuongHH = ko.observable(0);
    self.Quyen_NguoiDung = ko.observableArray();
    self.TraHangNhap_ThayDoiThoiGian = ko.observable();
    self.TraHangNhap_ThayDoiNhanVien = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    modelTypeSearchProduct.TypeSearch(1);// jqAutoProduct

    function PageLoad() {
        UpdateProperties_Undefined();
        GetListNhanVien();
        GetHT_Quyen_ByNguoiDung();
        Check_QuyenXemGiaVon();
        GetListNhomDT();
        GetListTinhThanh();
        getAllTaiKhoanNganHang();
        GetInforCongTy();
        GetInforChiNhanh();
        GetCauHinhHeThong();
        GetAllQuy_KhoanThuChi();
    }
    console.log(1)

    PageLoad();

    function UpdateProperties_Undefined() {
        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (commonStatisJs.CheckNull(cthd[i].PTThue)) {
                    cthd[i].PTThue = 0;
                }
                if (commonStatisJs.CheckNull(cthd[i].TienThue)) {
                    cthd[i].TienThue = 0;
                }
                if (commonStatisJs.CheckNull(cthd[i].ThanhToan)) {
                    cthd[i].ThanhToan = cthd[i].ThanhTien;
                }
            }
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
        }
    }

    function GetAllQuy_KhoanThuChi() {
        ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetQuy_KhoanThuChi', 'GET').done(function (x) {
            if (x.res === true) {
                vmThanhToan.listData.KhoanThuChis = x.data;
            }
        })
    }

    self.ChangeCheckTonKho = function () {
        var tk = parseInt(self.ConTonKho());
        if (tk === 1) {
            self.ConTonKho(0);
        }
        else {
            self.ConTonKho(1);
        }
        localStorage.setItem('trahangnhap_isTonKho', self.ConTonKho());
        modelTypeSearchProduct.ConTonKho(self.ConTonKho());
    }

    function CheckSaoChep_EditFromList() {
        var type = parseInt(localStorage.getItem('THN_Thaotac'));
        if (isNaN(type) === false) {
            var cthd = localStorage.getItem('THN_Chitiet');
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                for (let i = 0; i < cthd.length; i++) {
                    if (commonStatisJs.CheckNull(cthd[i].PTThue)) {
                        cthd[i].PTThue = 0;
                    }
                    if (commonStatisJs.CheckNull(cthd[i].TienThue)) {
                        cthd[i].TienThue = 0;
                    }
                    if (commonStatisJs.CheckNull(cthd[i].ThanhToan)) {
                        cthd[i].ThanhToan = cthd[i].ThanhTien;
                    }
                }

                var tongThueHD = cthd[0].TongTienThue;
                var ptThueHD = cthd[0].PTThueHD;
                if (commonStatisJs.CheckNull(tongThueHD)) {
                    tongThueHD = 0;
                }
                if (commonStatisJs.CheckNull(ptThueHD)) {
                    ptThueHD = 0;
                }
                let phaiTT = cthd[0].PhaiThanhToan - cthd[0].KhachDaTra;
                phaiTT = phaiTT < 0 ? 0 : phaiTT;

                let idHoaDon = null;
                switch (type) {
                    case 3:// trahang from nhaphang
                        idHoaDon = cthd[0].ID_HoaDon;
                        break;
                    case 4:// update again phieu trahang
                        idHoaDon = cthd[0].ID_PhieuNhapHang;
                        break;
                } 
               
                var objHD = [{
                    ID: type === 3 ? const_GuidEmpty : cthd[0].ID_HoaDon,
                    ID_HoaDon: idHoaDon,
                    ID_DonVi: cthd[0].ID_DonVi,
                    LoaiHoaDon: 7,
                    ID_DoiTuong: cthd[0].ID_DoiTuong,
                    ID_NhanVien: cthd[0].ID_NhanVien,
                    MaHoaDon: cthd[0].MaHoaDon,
                    NgayLapHoaDon: moment(cthd[0].NgayLapHoaDon).format('DD/MM/YYYY HH:mm'),
                    TongTienHang: cthd[0].TongTienHang,
                    TongGiamGia: cthd[0].TongGiamGia,
                    TongTienThue: tongThueHD,
                    PTThueHoaDon: ptThueHD,
                    TongChietKhau: cthd[0].TongChietKhau,
                    PhaiThanhToan: phaiTT,
                    TongThanhToan: cthd[0].TongThanhToan,
                    KhachDaTra: cthd[0].KhachDaTra,
                    DaThanhToan: 0,// assign = 0: de khach tu nhap
                    ConThieu: cthd[0].PhaiThanhToan,
                    DienGiai: cthd[0].DienGiai,
                    NguoiTao: _userLogin
                }];

                $('#selectedNV').val(cthd[0].ID_NhanVien);
                self.newHoaDon().SetData(objHD[0]);
                localStorage.setItem(lcHDTraHangNhap, JSON.stringify(objHD));
                localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
                self.HangHoaAfterAdd(cthd);
                Caculator_AmountProduct();
                $('#txtAutoDoiTuong').val('');

                localStorage.removeItem('THN_Thaotac');
                localStorage.removeItem('THN_Chitiet');

                if (!commonStatisJs.CheckNull(objHD[0].ID_DoiTuong)) {
                    getChiTietNCCByID(objHD[0].ID_DoiTuong);
                }
            }
        }
        else {
            CheckExistCacheHD();
        }
    }

    function CheckExistCacheHD() {
        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (cthd.length > 0) {
                dialogConfirm_OKCancel('Thông báo', 'Hệ thống tìm được 1 bản nháp chưa được lưu lên máy chủ. Bạn có muốn tiếp tục làm việc với bản nháp này?', function () {
                    var hd = localStorage.getItem(lcHDTraHangNhap);
                    if (hd !== null) {
                        hd = JSON.parse(hd);
                        self.HangHoaAfterAdd(cthd);
                        self.newHoaDon().SetData(hd[0]);
                        if (hd[0].ID_DoiTuong !== null && hd[0].ID_DoiTuong !== undefined) {
                            getChiTietNCCByID(hd[0].ID_DoiTuong);
                        }
                        $('#selectedNV').val(hd[0].ID_NhanVien);
                        Caculator_AmountProduct();
                        $('#txtAutoDoiTuong').val('');
                    }
                }, function () {
                    RemoveCache();
                    ResetInforHD();
                    $('#selectedNV').val(_idNhanVien);
                });
            }
            else {
                RemoveCache();
                ResetInforHD();
                $('#selectedNV').val(_idNhanVien);
            }
        }
        else {
            RemoveCache();
            ResetInforHD();
            $('#selectedNV').val(_idNhanVien);
        }
    }

    function GetListNhanVien() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + _idDonVi, 'GET').done(function (data) {
            self.NhanViens(data);
            CheckSaoChep_EditFromList();
        });
    }

    function GetListNhomDT() {
        ajaxHelper("/api/DanhMuc/DM_nhomDoiTuongAPI/" + "GetDM_NhomDoiTuong?loaiDoiTuong=2", 'GET').done(function (data) {
            if (data !== null) {
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
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data !== "" && data.length > 0) {
                    self.Quyen_NguoiDung(data);
                    self.TraHangNhap_ThayDoiThoiGian(CheckQuyenExist('TraHangNhap_ThayDoiThoiGian'));
                    self.TraHangNhap_ThayDoiNhanVien(CheckQuyenExist('TraHangNhap_ThayDoiNhanVien'));
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
            if (data !== null) {
                self.CongTy(data);
            }
        });
    }

    function GetInforChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + 'GetDM_DonVi/' + _idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.ChiNhanh(data);
            }
        });
    }

    function GetCauHinhHeThong() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + _idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.ThietLap(data);
            }
        });
    }

    function GetNgaySX_NgayHH(ctDoing) {
        var ngaysx = ctDoing.NgaySanXuat;
        if (!commonStatisJs.CheckNull(ngaysx)) {
            ngaysx = moment(ngaysx).format('DD/MM/YYYY');
        }
        var hansd = ctDoing.NgayHetHan;
        if (!commonStatisJs.CheckNull(hansd)) {
            hansd = moment(hansd).format('DD/MM/YYYY');
        }
        return {
            NgaySanXuat: ngaysx,
            NgayHetHan: hansd,
        }
    }

    function newCTNhap(addCungLoai, ctDoing, soluong) {
        var ptThue = self.newHoaDon().PTThueHoaDon() > 0 ? self.newHoaDon().PTThueHoaDon() : 0;
        var tienThue = ptThue * ctDoing.GiaNhap / 100;

        var lot = GetNgaySX_NgayHH(ctDoing);
        var lotParent = ctDoing.QuanLyTheoLoHang ? true : false;
        return {
            ID_HangHoa: ctDoing.ID_HangHoa,
            SoThuTu: 1,
            ID_Random: CreateIDRandom('CTHD_'),
            ID_DonViQuiDoi: ctDoing.ID_DonViQuiDoi,
            MaHangHoa: ctDoing.MaHangHoa,
            TenHangHoa: ctDoing.TenHangHoa,
            TenDonViTinh: ctDoing.TenDonViTinh,
            DonViTinh: ctDoing.DonViTinh,
            TyLeChuyenDoi: ctDoing.TyLeChuyenDoi,
            ThuocTinh_GiaTri: ctDoing.ThuocTinh_GiaTri,
            QuanLyTheoLoHang: ctDoing.QuanLyTheoLoHang,
            TonKho: ctDoing.TonKho,
            SrcImage: ctDoing.SrcImage,
            GiaNhap: ctDoing.GiaVon,
            DonGia: ctDoing.GiaVon, // = giatralai
            GiaVon: ctDoing.GiaVon,
            SoLuong: soluong,
            TienChietKhau: 0,
            PTChietKhau: 0,
            PTThue: ptThue,
            TienThue: tienThue,
            DVTinhGiam: '%',
            ThanhTien: soluong * ctDoing.GiaVon,
            ThanhToan: soluong * (ctDoing.GiaVon + tienThue),
            GhiChu: '',
            ID_LoHang: ctDoing.ID_LoHang,
            MaLoHang: ctDoing.MaLoHang,
            NgaySanXuat: lot.NgaySanXuat,
            NgayHetHan: lot.NgayHetHan,
            DM_LoHang: [],
            HangCungLoais: [],
            LotParent: lotParent,
            LaConCungLoai: addCungLoai,
        }
    }

    function FindCTHD_isDoing(item) {
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var concungloai = item.LaConCungLoai;
        var idRandom = item.ID_Random;

        var lstCTHD = localStorage.getItem(lcCTTraHangNhap);
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
                if (concungloai) {
                    for (let i = 0; i < lstCTHD.length; i++) {
                        for (let j = 0; j < lstCTHD[i].HangCungLoais.length; j++) {
                            if (lstCTHD[i].HangCungLoais[j].ID_Random === idRandom) {
                                return lstCTHD[i].HangCungLoais[j];
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
        }
        return null;
    }

    function XoaHangHoa_CheckCungLoai(cthd, lotParent, quanlytheolo, concungloai, idRandom) {
        if (lotParent || (concungloai === false && quanlytheolo === false)) {
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
            else {
                for (let i = 0; i < cthd.length; i++) {
                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                        if (cthd[i].HangCungLoais[j].ID_Random === idRandom) {
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

    function Caculator_AmountProduct() {
        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            var sumQuantity = 0;
            for (let i = 0; i < cthd.length; i++) {
                sumQuantity += parseFloat(cthd[i].SoLuong);

                // count Lot in Hang hoa
                for (let k = 1; k < cthd[i].DM_LoHang.length; k++) {
                    sumQuantity += parseFloat(cthd[i].DM_LoHang[k].SoLuong);
                }

                // count hangcungloai
                for (let k = 0; k < cthd[i].HangCungLoais.length; k++) {
                    sumQuantity += parseFloat(cthd[i].HangCungLoais[k].SoLuong);
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
            let mahh = $('#txtAutoHangHoa').val();
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetHangHoa_ByMaHangHoa?mahh=" + mahh + '&iddonvi=' + _idDonVi, 'GET').done(function (data) {
                console.log(data);
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
        var lstCT = localStorage.getItem(lcCTTraHangNhap);
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
                                let forLot = itFor.DM_LoHang[j];
                                if (forLot.ID_LoHang === item.ID_LoHang) {
                                    // if lot parent
                                    if (itFor.ID_LoHang === item.ID_LoHang) {
                                        lstCT[i].SoLuong = itFor.SoLuong + soluong;
                                        lstCT[i].ThanhTien = lstCT[i].SoLuong * (itFor.DonGia - itFor.TienChietKhau);
                                        lstCT[i].ThanhToan = lstCT[i].SoLuong * (itFor.DonGia - itFor.TienChietKhau + itFor.TienThue);
                                    }
                                    lstCT[i].DM_LoHang[j].SoLuong = lstCT[i].DM_LoHang[j].SoLuong + soluong;
                                    lstCT[i].DM_LoHang[j].ThanhTien = lstCT[i].DM_LoHang[j].SoLuong * (forLot.DonGia - forLot.TienChietKhau);
                                    lstCT[i].DM_LoHang[j].ThanhToan = lstCT[i].DM_LoHang[j].SoLuong * (forLot.DonGia - forLot.TienChietKhau + forLot.TienThue);

                                    i = lstCT.length;
                                    break;
                                }
                            }
                        }
                        else {
                            let obj = newCTNhap(true, item, soluong);
                            obj.LotParent = false;
                            obj.HangCungLoais = [];
                            obj.DM_LoHang = [];
                            lstCT[i].DM_LoHang.push(obj);
                        }
                    }
                    else {
                        lstCT[i].SoLuong = itFor.SoLuong + soluong;
                        lstCT[i].ThanhTien = lstCT[i].SoLuong * (itFor.DonGia - itFor.TienChietKhau);
                        lstCT[i].ThanhToan = lstCT[i].SoLuong * (itFor.DonGia - itFor.TienChietKhau + itFor.TienThue);
                    }
                    break;
                }
            }
        }
        else {
            let newCT = newCTNhap(false, item, soluong);
            if (item.QuanLyTheoLoHang) {
                let objLo = $.extend({}, newCT);
                objLo.HangCungLoais = [];
                objLo.DM_LoHang = [];
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
        localStorage.setItem(lcCTTraHangNhap, JSON.stringify(lstCT));

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
            var cthd = JSON.parse(localStorage.getItem(lcCTTraHangNhap));
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
                                // find in HangCungLoai
                                if (cthd[i].HangCungLoais.length > 0) {
                                    // focus HangCungLoai first
                                    idRandomFocus = cthd[i].HangCungLoais[0].ID_Random;
                                }
                                else {
                                    // focus in next Lot
                                    idRandomFocus = cthd[i + 1].ID_Random;
                                }
                            }
                            else {
                                // find in HangCungLoai
                                if (cthd[i].HangCungLoais.length > 0) {
                                    for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                                        if (cthd[i].HangCungLoais[j].ID_Random === itemCT.ID_Random) {
                                            if (j < cthd[i].HangCungLoais.length - 1) {
                                                // focus in next cungloai
                                                idRandomFocus = cthd[i].HangCungLoais[j + 1].ID_Random;
                                                i = lenCTHD;
                                                break;
                                            }
                                            else {
                                                // focus in next cthd
                                                idRandomFocus = cthd[i + 1].ID_Random;
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
                            if (cthd[i].ID_Random === itemCT.ID_Random) {
                                if (cthd[i].HangCungLoais.length > 0) {
                                    // focus HangCungLoai first
                                    idRandomFocus = cthd[i].HangCungLoais[0].ID_Random;
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
                                        if (cthd[i].HangCungLoais[j].ID_Random === itemCT.ID_Random) {
                                            if (j < cthd[i].HangCungLoais.length - 1) {
                                                // focus in next cungloai
                                                idRandomFocus = cthd[i].HangCungLoais[j + 1].ID_Random;
                                                i = lenCTHD;
                                                break;
                                            }
                                            else {
                                                // focus in next cthd
                                                if (i < cthd.length - 1) {
                                                    idRandomFocus = cthd[i + 1].ID_Random;
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
            var cthd = JSON.parse(localStorage.getItem(lcCTTraHangNhap));
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
                                    lstLoHang = cthd[i - 1].HangCungLoais;
                                    if (lstLoHang.length > 0) {
                                        idRandomFocus = lstLoHang[lstLoHang.length - 1].ID_Random;
                                    }
                                    else {
                                        idRandomFocus = cthd[i - 1].ID_Random;
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
                        if (cthd[i].HangCungLoais.length > 0) {
                            // find hangcungloai
                            if (cthd[i].ID_Random === itemCT.ID_Random) {
                                if (i - 1 >= 0) {
                                    let lstLoHang = cthd[i - 1].DM_LoHang;
                                    if (lstLoHang.length > 0) {
                                        // focus LoHang last
                                        idRandomFocus = lstLoHang[lstLoHang.length - 1].ID_Random;
                                    }
                                    else {
                                        // focus in prev li
                                        lstLoHang = cthd[i - 1].HangCungLoais;
                                        if (lstLoHang.length > 0) {
                                            idRandomFocus = lstLoHang[lstLoHang.length - 1].ID_Random;
                                        }
                                        else {
                                            idRandomFocus = cthd[i - 1].ID_Random;
                                        }
                                    }
                                }
                                else {
                                    Focus_InputTienTraHD();
                                    return false;
                                }
                            }
                            else {
                                for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                                    if (cthd[i].HangCungLoais[j].ID_Random === itemCT.ID_Random) {
                                        if (j - 1 >= 0) {
                                            idRandomFocus = cthd[i].HangCungLoais[j - 1].ID_Random;
                                        }
                                        else {
                                            idRandomFocus = cthd[i].ID_Random;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        else {
                            // find hangcungloai
                            if (cthd[i].ID_Random === itemCT.ID_Random) {
                                if (i - 1 >= 0) {
                                    let lstLoHang = cthd[i - 1].DM_LoHang;
                                    if (lstLoHang.length > 0) {
                                        // focus LoHang last
                                        idRandomFocus = lstLoHang[lstLoHang.length - 1].ID_Random;
                                    }
                                    else {
                                        // focus in prev li
                                        lstLoHang = cthd[i - 1].HangCungLoais;
                                        if (lstLoHang.length > 0) {
                                            idRandomFocus = lstLoHang[lstLoHang.length - 1].ID_Random;
                                        }
                                        else {
                                            idRandomFocus = cthd[i - 1].ID_Random;
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
        var lstCT = localStorage.getItem(lcCTTraHangNhap);
        if (lstCT !== null) {
            lstCT = JSON.parse(lstCT);
            let dvtDeleted = { ID_DonViQuiDoi: item.ID_DonViQuiDoi, TenDonViTinh: item.TenDonViTinh, Xoa: false };

            lstCT = XoaHangHoa_CheckCungLoai(lstCT, item.LotParent, item.QuanLyTheoLoHang, item.LaConCungLoai, item.ID_Random);
            // push dvt was delete into cthd (if same ID_HangHoa)
            for (let i = 0; i < lstCT.length; i++) {
                if (lstCT[i].ID_HangHoa === item.ID_HangHoa) {
                    lstCT[i].DonViTinh.push(dvtDeleted);
                }
            }
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(lstCT));

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
                $(thisObj).val(formatNumber3Digit(soluong));
                break;
            case 40:// down
                if (soluong > 0) {
                    soluong = soluong - 1;
                }
                $(thisObj).val(formatNumber3Digit(soluong));
                break;
        }

        var ctDoing = FindCTHD_isDoing(item);
        if (ctDoing !== null) {
            ctDoing.SoLuong = soluong;
            ctDoing.ThanhTien = soluong * (ctDoing.DonGia - ctDoing.TienChietKhau);
            ctDoing.ThanhToan = soluong * (ctDoing.DonGia - ctDoing.TienChietKhau + ctDoing.TienThue);
            $('#thanhtien_' + ctDoing.ID_Random).val(formatNumber3Digit(ctDoing.ThanhToan));

            var lstCT = localStorage.getItem(lcCTTraHangNhap);
            if (lstCT !== null) {
                lstCT = JSON.parse(lstCT);
                lstCT = updateCTHDLe(lstCT, ctDoing);
                localStorage.setItem(lcCTTraHangNhap, JSON.stringify(lstCT));
                Caculator_AmountProduct();
                Bind_UpdateHD(lstCT);
                Enter_CTHD(item, event, 'soluong_');
                Shift_CTHD(item, event, 'soluong_');
            }
        }
    }

    function updateCTHDLe(arr, ctDoing) {
        var quanlytheolo = ctDoing.QuanLyTheoLoHang;
        var concungloai = ctDoing.LaConCungLoai;
        var lotParent = ctDoing.LotParent;
        var idRandom = ctDoing.ID_Random;

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
                if (arr[i].ID_Random === idRandom) {
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
                        if (arr[i].DM_LoHang[j].ID_Random === idRandom) {
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
                        if (arr[i].HangCungLoais[j].ID_Random === idRandom) {
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

    self.EditGiaBan = function (item) {
        var thisObj = event.currentTarget;
        formatNumberObj(thisObj);
        var giaNhap = formatNumberToFloat($(thisObj).val());

        var ctDoing = FindCTHD_isDoing(item, false);
        if (ctDoing !== null) {
            var tienGiam = ctDoing.TienChietKhau;
            var tienThue = ctDoing.TienThue;
            var thanhtoan = 0;
            var idRandom = ctDoing.ID_Random;

            if (giaNhap === 0) {
                giaNhap = 0;
                tienGiam = 0;
                tienThue = 0;
                ctDoing.PTChietKhau = 0;
                ctDoing.PTThue = self.newHoaDon().PTThueHoaDon();
            }

            if (ctDoing.PTChietKhau > 0) {
                tienGiam = ctDoing.PTChietKhau * giaNhap / 100;
            }
            if (ctDoing.PTThue > 0) {
                tienThue = ctDoing.PTThue * (giaNhap - tienGiam) / 100;
            }
            thanhtoan = formatNumberToFloat(ctDoing.SoLuong) * (giaNhap - tienGiam + tienThue);

            ctDoing.DonGia = giaNhap;
            ctDoing.TienChietKhau = tienGiam;
            ctDoing.TienThue = tienThue;
            ctDoing.ThanhToan = thanhtoan;
            ctDoing.ThanhTien = (giaNhap - tienGiam) * ctDoing.SoLuong;
            var cthd = localStorage.getItem(lcCTTraHangNhap);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = updateCTHDLe(cthd, ctDoing);
                localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
                Bind_UpdateHD(cthd);
            }
            $('#tax_' + idRandom).val(formatNumber3Digit(tienThue));
            $('#thanhtien_' + idRandom).val(formatNumber3Digit(ctDoing.ThanhToan));
            Enter_CTHD(item, event, 'dongia_');
            Shift_CTHD(item, event, 'dongia_');
        }
    }

    self.ShowDiv_ThueHangHoa = function (item) {
        var idRandom = item.ID_Random;
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
        var idRandom = item.ID_Random;
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
                    objSale.val(ptThue);
                }
                else {
                    objSale.val(formatNumber3Digit(tienThue));
                    ptThue = 0;
                }

                var cthd = localStorage.getItem(lcCTTraHangNhap);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    cthd = updatePTVND(cthd, ctDoing, 2, ptThue, ctDoing.DVTingGiam);
                    localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
                }
            }
        }
    }

    self.cthd_editThue = function (item, e) {
        var sumTemp = 0;
        var idRandom = item.ID_Random;
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
            $('#thanhtien_' + idRandom).val(formatNumber3Digit(sumTemp));
            $('#tax_' + idRandom).val(formatNumber3Digit(ctDoing.TienThue));

            var cthd = localStorage.getItem(lcCTTraHangNhap);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                cthd = updateCTHDLe(cthd, ctDoing);
                localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
                Bind_UpdateHD(cthd, 4);
            }
        }
    }

    self.UpdateGhiChu_CTHD = function (item) {
        var idRandom = item.ID_Random;
        var quanlyTheoLo = item.QuanLyTheoLoHang;
        var ghichu = $(event.currentTarget).val();
        var concungloai = item.LaConCungLoai;

        var lcCTHD = localStorage.getItem(lcCTTraHangNhap);
        if (lcCTHD !== null) {
            lcCTHD = JSON.parse(lcCTHD);
            if (item.LotParent || (concungloai === false && quanlyTheoLo === false)) {
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
                else {
                    for (let i = 0; i < lcCTHD.length; i++) {
                        for (let j = 0; j < lcCTHD[i].HangCungLoais.length; j++) {
                            if (lcCTHD[i].HangCungLoais[j].ID_Random === idRandom) {
                                lcCTHD[i].HangCungLoais[j].GhiChu = ghichu;
                                i = lcCTHD.length;
                                break;
                            }
                        }
                    }
                }
            }
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(lcCTHD));
        }
    }

    self.ChangeDonViTinh = function (item, parent) {
        var newIDQuiDoi = item.ID_DonViQuiDoi;
        var oldIDQuiDoi = parent.ID_DonViQuiDoi;
        if (newIDQuiDoi !== oldIDQuiDoi) {
            let dvtOld = { ID_DonViQuiDoi: oldIDQuiDoi, TenDonViTinh: parent.TenDonViTinh, Xoa: false };

            var cthd = localStorage.getItem(lcCTTraHangNhap);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                // update lst DonViTinh for cthd
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
                    let tonkho = data[0].TonKho;

                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].ID_DonViQuiDoi === oldIDQuiDoi) {
                            cthd[i].MaHangHoa = mahang;
                            cthd[i].GiaNhap = gianhap;
                            cthd[i].DonGia = giavon;
                            cthd[i].TonKho = tonkho;
                            if (cthd[i].PTThue > 0) {
                                cthd[i].TienThue = cthd[i].PTThue * giavon / 100;
                            }
                            else {
                                cthd[i].TienThue = 0;
                            }
                            cthd[i].ThanhTien = giavon * cthd[i].SoLuong;
                            cthd[i].ThanhToan = (giavon + cthd[i].TienThue) * cthd[i].SoLuong;
                            cthd[i].TenDonViTinh = item.TenDonViTinh;
                            cthd[i].ID_DonViQuiDoi = newIDQuiDoi;

                            for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                cthd[i].DM_LoHang[j].ID_DonViQuiDoi = newIDQuiDoi;
                                cthd[i].DM_LoHang[j].TenDonViTinh = item.TenDonViTinh;
                                cthd[i].DM_LoHang[j].MaHangHoa = mahang;
                                cthd[i].DM_LoHang[j].GiaNhap = gianhap;
                                cthd[i].DM_LoHang[j].DonGia = giavon;
                                cthd[i].DM_LoHang[j].GiaVon = giavon;
                                cthd[i].DM_LoHang[j].TonKho = tonkho;
                                if (cthd[i].DM_LoHang[j].PTThue > 0) {
                                    cthd[i].DM_LoHang[j].TienThue = cthd[i].DM_LoHang[j].PTThue * giavon / 100;
                                }
                                else {
                                    cthd[i].DM_LoHang[j].TienThue = 0;
                                }
                                cthd[i].DM_LoHang[j].ThanhTien = giavon * cthd[i].DM_LoHang[j].SoLuong;
                                cthd[i].DM_LoHang[j].ThanhToan = (giavon + cthd[i].DM_LoHang[j].TienThue) * cthd[i].DM_LoHang[j].SoLuong;
                            }

                            for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                                cthd[i].HangCungLoais[j].ID_DonViQuiDoi = newIDQuiDoi;
                                cthd[i].HangCungLoais[j].TenDonViTinh = item.TenDonViTinh;
                                cthd[i].HangCungLoais[j].MaHangHoa = mahang;
                                cthd[i].HangCungLoais[j].GiaNhap = gianhap;
                                cthd[i].HangCungLoais[j].DonGia = giavon;
                                cthd[i].HangCungLoais[j].GiaVon = giavon;
                                cthd[i].HangCungLoais[j].TonKho = tonkho;
                                if (cthd[i].HangCungLoais[j].PTThue > 0) {
                                    cthd[i].HangCungLoais[j].TienThue = cthd[i].HangCungLoais[j].PTThue * giavon / 100;
                                }
                                else {
                                    cthd[i].HangCungLoais[j].TienThue = 0;
                                }
                                cthd[i].HangCungLoais[j].ThanhTien = giavon * cthd[i].HangCungLoais[j].SoLuong;
                                cthd[i].HangCungLoais[j].ThanhToan = (giavon + cthd[i].HangCungLoais[j].TienThue) * cthd[i].HangCungLoais[j].SoLuong;
                            }
                            break;
                        }
                    }
                    localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
                    self.HangHoaAfterAdd(cthd);
                    Bind_UpdateHD(cthd);
                });
            }
        }
    }

    self.ClickThemLo = function (item) {
        var quanlytheolo = item.QuanLyTheoLoHang;
        var idQuiDoi = item.ID_DonViQuiDoi;

        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (quanlytheolo) {
                // add lo
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].ID_DonViQuiDoi === idQuiDoi) {
                        let lohang = $.extend({}, cthd[i]);
                        let obj = newCTNhap(true, lohang, 1);
                        obj.LotParent = false;
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
                    if (cthd[i].ID_DonViQuiDoi === idQuiDoi) {
                        let obj = newCTNhap(true, cthd[i], 1);
                        cthd[i].HangCungLoais.push(obj);
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
            self.HangHoaAfterAdd(cthd);
            Bind_UpdateHD(cthd);
            Caculator_AmountProduct();
        }
    }

    self.EditThanhToan = function (item) {
        var thisObj = event.currentTarget;
        var idRandom = item.ID_Random;
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
                $('#soluong_' + idRandom).val(formatNumber3Digit(soluong));
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
        var hd = localStorage.getItem(lcHDTraHangNhap);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hd[0].PTThueHoaDon = 0;
            self.newHoaDon().PTThueHoaDon(0);
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
        }

        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            cthd = updateCTHDLe(cthd, ctDoing);
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
            Bind_UpdateHD(cthd);
        }
        $('#dongia_' + idRandom).val(formatNumber3Digit(gianhap));
        $('#tax_' + idRandom).val(0);
        Enter_CTHD(item, event, 'thanhtien_');
        Shift_CTHD(item, event, 'thanhtien_');
    }

    self.ResetLo = function (item) {
        var thisObj = $(event.currentTarget);
        var idRandom = item.ID_Random;
        var quanLiTheoLo = item.QuanLyTheoLoHang;
        var concungloai = item.LaConCungLoai;
        var lotParent = item.LotParent;

        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (lotParent || (concungloai === false && quanLiTheoLo === false)) {
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
                else {
                    for (let i = 0; i < cthd.length; i++) {
                        for (let j = 0; j < cthd[i].HangCungLoais.length; j++) {
                            if (cthd[i].HangCungLoais[j].ID_Random === idRandom) {
                                cthd[i].HangCungLoais[j].ID_LoHang = null;;
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
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
        }
        thisObj.closest('.op-js-thongtinlo').find('input').val('');// empty malo
        thisObj.closest('.op-js-thongtinlo').find('.op-js-thuoctinhlo input').show();// show input ngaysx, hethan
        thisObj.closest('.op-js-thongtinlo').find('.op-js-thuoctinhlo div').hide();// hide div ngaysx, hethan
    }

    self.LoadListLoHang = function (item) {
        ajaxHelper(DMHangHoaUri + 'GetInforProduct_ByIDQuidoi?idQuiDoi=' + item.ID_DonViQuiDoi
            + '&idChiNhanh=' + _idDonVi).done(function (x) {
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
        var idRandom = $($this.closest('.op-js-thuoctinhlo1')).find('span').eq(0).attr('id');

        // update TonKho, GiaVon, GiaBan, Lo
        var cthd = localStorage.getItem(lcCTTraHangNhap);
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
                        cthd[i].TonKho = item.TonKho;
                        cthd[i].GiaNhap = item.GiaNhap;
                        cthd[i].DonGia = item.GiaVon;
                        cthd[i].GiaVon = item.GiaVon;
                        if (cthd[i].PTThue > 0) {
                            cthd[i].TienThue = cthd[i].PTThue * item.GiaVon / 100;
                        }
                        else {
                            cthd[i].TienThue = 0;
                        }
                        cthd[i].ThanhTien = item.GiaVon * cthd[i].SoLuong;
                        cthd[i].ThanhToan = (item.GiaVon + cthd[i].TienThue) * cthd[i].SoLuong;
                        cthd[i].TienChietKhau = 0;
                        cthd[i].PTChietKhau = 0;
                        cthd[i].DVTinhGiam = '%';
                    }
                    if (itFor.ID_Random === idRandom) {
                        cthd[i].DM_LoHang[j].ID_LoHang = idLoHang;
                        cthd[i].DM_LoHang[j].MaLoHang = maLoHang;
                        cthd[i].DM_LoHang[j].NgaySanXuat = ngaysx;
                        cthd[i].DM_LoHang[j].NgayHetHan = hethan;
                        cthd[i].DM_LoHang[j].TonKho = item.TonKho;
                        cthd[i].DM_LoHang[j].GiaNhap = item.GiaNhap;
                        cthd[i].DM_LoHang[j].DonGia = item.GiaVon;
                        cthd[i].DM_LoHang[j].GiaVon = item.GiaVon;
                        if (itFor.PTThue > 0) {
                            cthd[i].DM_LoHang[j].TienThue = itFor.PTThue * item.GiaVon / 100;
                        }
                        else {
                            cthd[i].DM_LoHang[j].TienThue = 0;
                        }
                        cthd[i].DM_LoHang[j].ThanhTien = item.GiaVon * cthd[i].DM_LoHang[j].SoLuong;
                        cthd[i].DM_LoHang[j].ThanhToan = (item.GiaVon + cthd[i].DM_LoHang[j].TienThue) * cthd[i].DM_LoHang[j].SoLuong;
                        cthd[i].DM_LoHang[j].TienChietKhau = 0;
                        cthd[i].DM_LoHang[j].PTChietKhau = 0;
                        cthd[i].DM_LoHang[j].DVTinhGiam = '%';
                        i = cthd.length;
                        break;
                    }
                }
            }
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
            self.HangHoaAfterAdd(cthd);
            Bind_UpdateHD(cthd);
        }
        self.ListLot_ofProduct([]);
    }

    self.CheckNewLo = function (type, item) {
        var $this = $(event.currentTarget);
        var valIput = $this.val();
        var idRandom = item.ID_Random;
        var malo = $('#AddNewLo' + idRandom).val();
        var nsx = $('#nsx' + idRandom).val();
        var hsd = $('#hd' + idRandom).val();

        if (type === 1) {
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
                var cthd = localStorage.getItem(lcCTTraHangNhap);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].QuanLyTheoLoHang) {
                            for (let j = 0; j < cthd[i].DM_LoHang.length; j++) {
                                let itFor = cthd[i].DM_LoHang[j];
                                if (cthd[i].ID_Random === idRandom) {
                                    cthd[i].MaLoHang = malo;
                                    cthd[i].NgaySanXuat = nsx;
                                    cthd[i].NgayHetHan = hsd;
                                    cthd[i].ID_LoHang = null;
                                }
                                if (itFor.ID_Random === idRandom) {
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
                    localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
                    self.HangHoaAfterAdd(cthd);
                }
            }
        });
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
        $('#txtAutoHangHoa').focus().select();
    }

    shortcut.add("F3", function () {
        $('#txtAutoHangHoa').focus();
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

    self.ChangeCus = function (item) {
        console.log('item ', item)
        if (commonStatisJs.CheckNull(item.ID)) {
            $('#lblHideNCC').text('Nhà cung cấp chưa xác định');
        }
        else {
            getChiTietNCCByID(item.ID);
            self.newHoaDon().ID_DoiTuong(item.ID);
            $('#lblHideNCC').text('');
        }
    }

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
        var CheckInHD = localStorage.getItem('CheckInWhenTH');
        if (CheckInHD === "true") {
            $('#divSetPrintPay .main-show').addClass("main-hide");
        }
        else {
            $('#divSetPrintPay .main-show').removeClass("main-hide");
        }
        $('#divSetTonKho').hide();
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
        localStorage.setItem('CheckInWhenTH', allow);
    });

    function getChiTietNCCByID(id) {
        ajaxHelper(DMDoiTuongUri + "GetDM_DoiTuong/" + id, 'GET').done(function (data) {
            self.ChiTietDoiTuong(data);
            SetIDDoiTuong_toCacheHD(data.ID);
        });
    }

    self.deleteNCC = function (item) {
        $('jqauto-customer ._jsInput').val('');
        self.ChiTietDoiTuong([]);
        self.newHoaDon().ID_DoiTuong(undefined);
        SetIDDoiTuong_toCacheHD(null);

        var hd = localStorage.getItem(lcHDTraHangNhap);
        if (hd != null) {
            hd = JSON.parse(hd);
            // if update again trahangnhap
            if (!commonStatisJs.CheckNull(hd[0].ID) && hd[0].ID !== const_GuidEmpty) {
                hd[0].KhachDaTra = 0;
                hd[0].HoanTra = 0;
                hd[0].PhaiThanhToan = hd[0].TongTienHang + hd[0].TongTienThue - hd[0].TongGiamGia;
                self.newHoaDon().KhachDaTra(0);
                self.newHoaDon().HoanTra(0);
                self.newHoaDon().PhaiThanhToan(0);
                self.newHoaDon().ConThieu(hd[0].PhaiThanhToan);
            }
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
        }
    }

    function SetIDDoiTuong_toCacheHD(idNCC) {
        var hd = localStorage.getItem(lcHDTraHangNhap);
        if (hd !== null) {
            hd = JSON.parse(hd);
            if (idNCC !== hd[0].ID_DoiTuong && hd[0].ID != null && hd[0].ID !== const_GuidEmpty) {
                hd[0].KhachDaTra = 0;
                hd[0].HoanTra = 0;
                self.newHoaDon().KhachDaTra(0);
                self.newHoaDon().HoanTra(0);
            }
            hd[0].ID_DoiTuong = idNCC;
            if (idNCC === null) {
                hd[0].SoDuDatCoc = 0;
            }
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
        }
    }

    $('.txtNgayLapHD').datetimepicker({
        timepicker: true,
        mask: true,
        format: 'd/m/Y H:i',
        maxDate: new Date(),
        onChangeDateTime: function (dp, $input) {
            self.newHoaDon().NgayLapHoaDon($input.val());
            var ok = CheckNgayLapHD_format($input.val());
            if (ok) {
                var hd = localStorage.getItem(lcHDTraHangNhap);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    hd[0].NgayLapHoaDon = $input.val();
                    localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
                }
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

    function Bind_UpdateHD(lstCT, typeEdit = 0) {
        let sum = 0, tienthue = 0;
        for (let i = 0; i < lstCT.length; i++) {
            sum += lstCT[i].ThanhTien;
            tienthue += lstCT[i].TienThue * lstCT[i].SoLuong;

            for (let j = 1; j < lstCT[i].DM_LoHang.length; j++) {
                sum += lstCT[i].DM_LoHang[j].ThanhTien;
                tienthue += lstCT[i].DM_LoHang[j].TienThue * lstCT[i].DM_LoHang[j].SoLuong;
            }
            for (let j = 0; j < lstCT[i].HangCungLoais.length; j++) {
                sum += lstCT[i].HangCungLoais[j].ThanhTien;
                tienthue += lstCT[i].HangCungLoais[j].TienThue * lstCT[i].HangCungLoais[j].SoLuong;
            }
        }

        var hd = self.newHoaDon();
        switch (typeEdit) {
            case 4: // editThue_chitiet
                self.newHoaDon().PTThueHoaDon(0);
                self.newHoaDon().TongTienHang(sum);
                self.newHoaDon().TongTienThue(tienthue);
                break;
            case 0:
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
        let daTT = self.newHoaDon().DaThanhToan();
        let khachdatra = self.newHoaDon().KhachDaTra();
        khachdatra = khachdatra === undefined ? 0 : khachdatra;
        let phaiTT = self.newHoaDon().TongTienHang() + tienthue - ggHD - khachdatra;
        if (phaiTT < 0) {
            phaiTT = 0;
        }
        self.newHoaDon().TongGiamGia(ggHD);
        self.newHoaDon().PhaiThanhToan(phaiTT);
        self.newHoaDon().ConThieu(phaiTT - daTT);

        var objHD = [{
            ID: self.newHoaDon().ID(),
            LoaiHoaDon: loaiHoaDon,
            MaHoaDon: self.newHoaDon().MaHoaDon(),
            ID_DoiTuong: self.newHoaDon().ID_DoiTuong(),
            ID_DonVi: _idDonVi,
            ID_NhanVien: $('#selectedNV').val(),
            NgayLapHoaDon: self.newHoaDon().NgayLapHoaDon(),
            TongTienHang: self.newHoaDon().TongTienHang(),
            PTThueHoaDon: self.newHoaDon().PTThueHoaDon(),
            TongTienThue: tienthue,
            TongGiamGia: self.newHoaDon().TongGiamGia(),
            TongChietKhau: self.newHoaDon().TongChietKhau(),
            PhaiThanhToan: phaiTT,
            TongThanhToan: phaiTT,
            DaThanhToan: self.newHoaDon().DaThanhToan(),
            ConThieu: self.newHoaDon().ConThieu(),
            KhachDaTra: self.newHoaDon().KhachDaTra(),// số tiền khách đã TT trước đó (nếu tạm lưu/cập nhật)
            TienMat: self.newHoaDon().TienMat(),
            TienPOS: self.newHoaDon().TienPOS(),
            TienChuyenKhoan: self.newHoaDon().TienChuyenKhoan(),
            TienDatCoc: self.newHoaDon().TienDatCoc(),
            TenTaiKhoanPos: self.newHoaDon().TenTaiKhoanPos(),
            TenTaiKhoanCK: self.newHoaDon().TenTaiKhoanCK(),
            ID_TaiKhoanChuyenKhoan: self.newHoaDon().ID_TaiKhoanChuyenKhoan(),
            ID_TaiKhoanPos: self.newHoaDon().ID_TaiKhoanPos(),
            ID_KhoanThuChi: self.newHoaDon().ID_KhoanThuChi(),
        }];
        localStorage.setItem(lcHDTraHangNhap, JSON.stringify(objHD));
    }


    self.showPopupNCC = function () {
        vmThemMoiNCC.showModalAdd();
    };

    self.showPopupEditNCC = function (item) {
        item.TenNhomKhachs = item.NhomDoiTuong;
        vmThemMoiNCC.showModalUpdate(item);
    };

    function Enable_btnSave() {
        self.isLoading(false);
    }

    self.SaveInvoice = function (status) {
        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            if (cthd.length > 0) {
                var arrCT = [];
                var err = '';
                var errLo = '';
                var errTonKho = '';
                var sumTonKho = 0;
                var sumSoLuong = 0;

                if (CheckChar_Special(self.newHoaDon().MaHoaDon())) {
                    ShowMessage_Danger('Mã hóa đơn không được chứa kí tự đặc biệt');
                    Enable_btnSave();
                    return false;
                }
                // get cthd & check tonkho, soluong
                for (let i = 0; i < cthd.length; i++) {
                    let itOut = cthd[i];
                    let soluong = formatNumberToFloat(itOut.SoLuong);
                    sumTonKho = itOut.TonKho * itOut.TyLeChuyenDoi;
                    sumSoLuong = itOut.SoLuong * itOut.TyLeChuyenDoi;
                    itOut.SoThuTu = arrCT.length + 1;
                    arrCT.push(itOut);

                    if (soluong === 0) {
                        err += itOut.TenHangHoa + ', ';
                    }
                    if (soluong > itOut.TonKho) {
                        errTonKho += itOut.TenHangHoa + ', ';
                    }
                    for (let j = 0; j < itOut.DM_LoHang.length; j++) {
                        let itFor = itOut.DM_LoHang[j];
                        let soluongFor = formatNumberToFloat(itFor.SoLuong);
                        if (itFor.ID_LoHang === null) {
                            errLo += itFor.TenHangHoa + ', ';
                        }
                        if (j !== 0) {
                            itFor.SoThuTu = arrCT.length + 1;
                            arrCT.push(itFor);
                            if (soluongFor === 0) {
                                err += itFor.TenHangHoa + ', ';
                            }
                            if (soluongFor > itFor.TonKho) {
                                errTonKho += itFor.TenHangHoa + ', ';
                            }
                            sumTonKho += itFor.TonKho * itFor.TyLeChuyenDoi;
                            sumSoLuong += soluongFor * itFor.TyLeChuyenDoi;
                        }
                    }
                    for (let k = 0; k < itOut.HangCungLoais.length; k++) {
                        let itFor = itOut.HangCungLoais[k];
                        let soluongFor = formatNumberToFloat(itFor.SoLuong);

                        itOut.HangCungLoais[k].SoThuTu = arrCT.length + 1;
                        arrCT.push(itOut.HangCungLoais[k]);
                        if (soluongFor === 0) {
                            err += itFor.TenHangHoa + ', ';
                        }
                        if (soluongFor > itFor.TonKho) {
                            errTonKho += itFor.TenHangHoa + ', ';
                        }
                        sumTonKho += itFor.TonKho * itFor.TyLeChuyenDoi;
                        sumSoLuong += soluongFor * itFor.TyLeChuyenDoi;
                    }
                }
                err = Remove_LastComma(err);
                for (let i = 0; i < arrCT.length; i++) {
                    delete arrCT[i]["DM_LoHang"];
                }
                if (self.ThietLap().XuatAm === false) {
                    if (errTonKho !== '') {
                        ShowMessage_Danger('Không đủ số lượng Tồn kho cho ' + errTonKho);
                        Enable_btnSave();
                        return false;
                    }
                    console.log('sumTonKho ', sumTonKho, sumSoLuong);
                    if (sumTonKho < sumSoLuong) {
                        ShowMessage_Danger('Tổng số lượng trả của các mặt hàng khác đơn vị tính lớn hơn số lượng tồn kho ');
                        Enable_btnSave();
                        return false;
                    }
                }
                // check MaLo
                if (errLo !== '') {
                    ShowMessage_Danger('Vui lòng nhập mã lô hàng cho ' + errLo);
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

                self.isLoading(true);
                var hd = localStorage.getItem(lcHDTraHangNhap);
                if (hd !== null) {
                    hd = JSON.parse(hd);
                    hd[0].NgayLapHoaDon = moment(hd[0].NgayLapHoaDon, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
                    hd[0].ChoThanhToan = status === 0 ? false : true;
                    hd[0].ID_NhanVien = $('#selectedNV').val();
                    hd[0].MaHoaDon = self.newHoaDon().MaHoaDon();
                    hd[0].DienGiai = self.newHoaDon().DienGiai();
                    hd[0].ID_HoaDon = self.newHoaDon().ID_HoaDon();// used to trahang from nhaphang
                }
                else {
                    Enable_btnSave();
                    return false;
                }
                SaveHoaDon(hd[0], arrCT);
            }
            else {
                ShowMessage_Danger('Vui lòng nhập chi tiết hóa đơn');
            }
        }
        else {
            ShowMessage_Danger('Vui lòng nhập chi tiết hóa đơn');
        }
    }

    self.clickBtnHuyHD = function () {
        if (self.HangHoaAfterAdd() !== null && self.HangHoaAfterAdd().length > 0) {
            dialogConfirm('Xác nhận hủy', 'Bạn có chắc chắn muốn hủy hóa đơn đang thực hiện không', function () {
                RemoveCache();
                GotoPageListTraHangNhap();
            });
        }
        else {
            RemoveCache();
            GotoPageListTraHangNhap();
        }
    }

    function GotoPageListTraHangNhap() {
        window.location.href = '/#/PurchaseReturns';
    }

    function SaveHoaDon(hd, cthd) {
        hd.TenDoiTuong = 'Nhà cung cấp lẻ';
        hd.DiaChiKhachHang = '';
        hd.DienThoaiKhachHang = '';
        hd.NguoiTao = _userLogin;

        var phaiTT = formatNumberToFloat(hd.TongTienHang)
            + formatNumberToFloat(hd.TongTienThue)
            - formatNumberToFloat(hd.TongGiamGia);
        hd.PhaiThanhToan = phaiTT;
        hd.TongThanhToan = phaiTT;

        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong() !== undefined) {
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
        myData.idNhanVien = _idNhanVien;
        var idHoaDon = myData.objHoaDon.ID;
        if (idHoaDon !== null && idHoaDon !== undefined && idHoaDon !== const_GuidEmpty) {
            Put_TraHangNhap(myData);
        }
        else {
            Post_TraHangNhap(myData);
        }
    }

    function ResetInforHD() {
        self.HangHoaAfterAdd([]);
        self.TongSoLuongHH(0);
        self.newHoaDon(new FormModel_NewHoaDon());
        self.ChiTietDoiTuong([]);
        vmThanhToan.PhieuThuKhach = vmThanhToan.newPhieuThu(2);
        $('#tenloaitien').text('(Tiền mặt)');
        $('jqauto-customer ._jsInput').val('');
    }

    function RemoveCache() {
        localStorage.removeItem(lcCTTraHangNhap);
        localStorage.removeItem(lcHDTraHangNhap);
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
            ID: hd.ID,// 1.kh, 2.ncc, 3.bh
            LoaiDoiTuong: 2,// 1.kh, 2.ncc, 3.bh
            LoaiHoaDon: hd.LoaiHoaDon,
            MaHoaDon: hd.MaHoaDon,
            HoanTra: 0,
            PhaiThanhToan: hd.PhaiThanhToan,
            TongThanhToan: hd.TongThanhToan,
            ThucThu: hd.DaThanhToan,
            ConNo: 0,
            DienGiai: hd.DienGiai,
            ID_DoiTuong: idCus,
            MaDoiTuong: maKH,
            TenDoiTuong: tenKH,
            ID_KhoanThuChi: null,
            NgayLapHoaDon: hd.NgayLapHoaDon,
            ID_NhanVien: hd.ID_NhanVien,
            ID_DonVi: hd.ID_DonVi,
            NguoiTao: _userLogin,
            DaThanhToan: hd.DaThanhToan, // used to don't click btnThanhToan
            SoDuDatCoc: 0,
        }
        console.log('obj', obj)
        vmThanhToan.inforHoaDon = obj;
    }

    function Insert_ThongBaoHetTonKho(cthd) {
        var arrQuiDoi = cthd.map(function (x) {
            return x.ID_DonViQuiDoi;
        })
        var arrIDLo = cthd.filter(x => x.ID_LoHang !== null).map(function (x) {
            return x.ID_LoHang;
        })
        if (arrQuiDoi.length > 0) {
            var param = {
                ID_ChiNhanh: _idDonVi,
                ListIDQuyDoi: arrQuiDoi,
                ListIDLoHang: arrIDLo,
            }
            ajaxHelper('/api/DanhMuc/HT_API/Insert_ThongBaoHetTon', 'POST', param).done(function (x) {
            })
        }
    }

    function Post_TraHangNhap(myData) {
        console.log('Post_TraHangNhap ', myData)
        ajaxHelper(BH_HoaDonUri + 'Post_TraHangNhap', 'post', myData).done(function (x) {
            if (x.res === true) {
                RemoveCache();
                ShowMessage_Success('Tạo phiếu trả hàng thành công');

                var status = 'Tạo mới ';
                if (myData.objHoaDon.ChoThanhToan) {
                    status = 'Tạm lưu ';
                }
                var diary = {
                    LoaiNhatKy: 1,
                    ID_Donvi: _idDonVi,
                    ID_NhanVien: _idNhanVien,
                    ID_HoaDon: x.data.ID,
                    ThoiGianUpdateGV: x.data.NgayLapHoaDon,
                    LoaiHoaDon: 7,
                    ChucNang: 'Trả hàng nhập',
                    NoiDung: status.concat('phiếu trả hàng nhập ', x.data.MaHoaDon,
                        ', Thời gian: ', moment(x.data.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss'),
                        ', Tổng trả: ', formatNumber3Digit(myData.objHoaDon.TongTienHang),
                        ', Người tạo: ', _userLogin),
                    NoiDungChiTiet: status.concat('phiếu trả hàng nhập <br />', x.data.Diary)
                }
                Post_NhatKySuDung_UpdateGiaVon(diary);

                // save soquy
                myData.objHoaDon.MaHoaDon = x.data.MaHoaDon;
                myData.objHoaDon.ID = x.data.ID;
                if (myData.objHoaDon.ChoThanhToan === false) {
                    Insert_ThongBaoHetTonKho(myData.objCTHoaDon);
                    AssignValueHoaDon_ToPhieuThu(myData.objHoaDon);
                    vmThanhToan.SavePhieuThu();
                }

                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
                ResetInforHD();
            }
            else {
                ShowMessage_Danger(x.mes);
            }
            Enable_btnSave();
        });
    }

    function Put_TraHangNhap(myData) {
        console.log('Put_TraHangNhap ', myData)
        ajaxHelper(BH_HoaDonUri + 'Update_TraHangNhap', 'post', myData).done(function (x) {
            if (x.res === true) {
                RemoveCache();
                ShowMessage_Success('Cập nhật phiếu trả hàng thành công');

                // save soquy
                myData.objHoaDon.MaHoaDon = x.data.MaHoaDon;
                myData.objHoaDon.ID = x.data.ID;
                if (myData.objHoaDon.ChoThanhToan === false) {
                    AssignValueHoaDon_ToPhieuThu(myData.objHoaDon);
                    vmThanhToan.SavePhieuThu();
                    Insert_ThongBaoHetTonKho(myData.objCTHoaDon);
                }

                var diary = {
                    LoaiNhatKy: 2,
                    ID_Donvi: _idDonVi,
                    ID_NhanVien: _idNhanVien,
                    ID_HoaDon: x.data.ID,
                    ThoiGianUpdateGV: x.data.NgayLapHoaDonOld,
                    LoaiHoaDon: 7,
                    ChucNang: 'Trả hàng nhập',
                    NoiDung: 'Cập nhật '.concat('phiếu trả hàng nhập ', x.data.MaHoaDon,
                        ', Thời gian: ', moment(x.data.NgayLapHoaDon).format('DD/MM/YYYY HH:mm:ss'),
                        ', Tổng trả: ', formatNumber3Digit(myData.objHoaDon.TongTienHang),
                        ', Người sửa: ', _userLogin),
                    NoiDungChiTiet: 'Cập nhật '.concat('phiếu trả hàng nhập ', x.data.Diary,
                        ' <br /> <b> Thông tin cũ </b> <br />', x.data.DiaryOld)
                }
                Post_NhatKySuDung_UpdateGiaVon(diary);

                self.InHoaDon(myData.objCTHoaDon, myData.objHoaDon);
                ResetInforHD();
            }
            else {
                ShowMessage_Danger(x.mes);
            }
            Enable_btnSave();
        });
    }

    self.InHoaDon = function (cthd, hd) {
        if (localStorage.getItem('CheckInWhenTH') === "true") {
            var cthdFormat = GetCTHDPrint_Format(cthd);
            self.CTHoaDonPrint(cthdFormat);

            var sluongNhap = 0;
            for (let i = 0; i < cthd.length; i++) {
                sluongNhap += formatNumberToFloat(cthd[i].SoLuong);
            }
            var itemHDFormat = GetInforHDPrint(hd);
            itemHDFormat.TongSoLuongHang = sluongNhap;
            self.InforHDprintf(itemHDFormat);

            ajaxHelper('/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + TeamplateMauIn + '&idDonVi=' + _idDonVi, 'GET').done(function (result) {
                var data = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                data = data.concat("<script > var item4=[], item5=[]; var item1=" + JSON.stringify(self.CTHoaDonPrint()) + "; var item2=" + JSON.stringify(self.CTHoaDonPrint()) + ";var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data, self.numbersPrintHD());
            });
        }
    }

    function GetCTHDPrint_Format(arrCTHD) {
        for (let i = 0; i < arrCTHD.length; i++) {
            arrCTHD[i].SoThuTu = i + 1;
            arrCTHD[i].TenHangHoa = arrCTHD[i].TenHangHoa.split('(')[0] + (arrCTHD[i].TenDonViTinh !== "" && arrCTHD[i].TenDonViTinh !== null ? "(" + arrCTHD[i].TenDonViTinh + ")" : "") + (arrCTHD[i].ThuocTinh_GiaTri !== null ? arrCTHD[i].ThuocTinh_GiaTri : "") + (arrCTHD[i].MaLoHang !== "" && arrCTHD[i].MaLoHang !== null ? "(Lô: " + arrCTHD[i].MaLoHang + ")" : "");
            arrCTHD[i].DonGia = formatNumber3Digit(arrCTHD[i].DonGia);
            arrCTHD[i].GiaBan = formatNumber3Digit(formatNumberToFloat(arrCTHD[i].DonGia) - formatNumberToFloat(arrCTHD[i].TienChietKhau));
            arrCTHD[i].TienChietKhau = formatNumber3Digit(arrCTHD[i].TienChietKhau);
            arrCTHD[i].SoLuong = formatNumber3Digit(arrCTHD[i].SoLuong);
            arrCTHD[i].ThanhTien = formatNumber3Digit(arrCTHD[i].ThanhTien);
            arrCTHD[i].ThanhToan = formatNumber3Digit(arrCTHD[i].ThanhToan);
            arrCTHD[i].TienThue = formatNumber3Digit(arrCTHD[i].TienThue);
        }
        return arrCTHD;
    }

    function GetInforHDPrint(objHD) {
        var hdPrint = $.extend({}, objHD);
        var phaiThanhToan = formatNumberToFloat(hdPrint.PhaiThanhToan);
        var daThanhToan = daThanhToan = formatNumberToFloat(vmThanhToan.PhieuThuKhach.DaThanhToan);
        var conno = phaiThanhToan - daThanhToan;

        hdPrint.TienMat = formatNumber3Digit(vmThanhToan.PhieuThuKhach.TienMat);
        hdPrint.TienATM = formatNumber3Digit(vmThanhToan.PhieuThuKhach.TienPOS);
        hdPrint.ChuyenKhoan = formatNumber3Digit(vmThanhToan.PhieuThuKhach.TienCK);

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
        hdPrint.PhuongThucTT = Remove_LastComma(pthuc);

        hdPrint.NgayLapHoaDon = moment(hdPrint.NgayLapHoaDon, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
        hdPrint.TongTienHang = formatNumber3Digit(objHD.TongTienHang);
        hdPrint.PhaiThanhToan = formatNumber3Digit(phaiThanhToan);
        hdPrint.TongThanhToan = formatNumber3Digit(objHD.TongThanhToan);
        hdPrint.DaThanhToan = formatNumber3Digit(daThanhToan);
        hdPrint.TienBangChu = DocSo(daThanhToan);
        hdPrint.NoTruoc = 0;
        hdPrint.NoSau = formatNumber3Digit(conno);
        hdPrint.TongCong = formatNumber3Digit(objHD.TongThanhToan);
        hdPrint.ChiPhiNhap = 0;
        hdPrint.KhuyeMai_GiamGia = 0;

        hdPrint.NguoiTaoHD = _userLogin;
        hdPrint.NhanVienBanHang = $('#selectedNV :selected').text();

        // cong ty, chi nhanh
        hdPrint.TenCuaHang = _tenDonVi;
        hdPrint.DienThoaiChiNhanh = self.ChiNhanh().SoDienThoai;
        hdPrint.DiaChiCuaHang = self.ChiNhanh().DiaChi;
        hdPrint.LogoCuaHang = '';
        if (self.CongTy().length > 0) {
            hdPrint.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
        }
        return hdPrint;
    }

    // if show confirm but not click OK/Cancel, click out popup
    $('#modalPopuplgDelete').on('hidden.bs.modal', function () {
        Enable_btnSave();
        if (self.HangHoaAfterAdd().length > 0 && self.TongSoLuongHH() === 0) {
            var hd = localStorage.getItem(lcHDTraHangNhap);
            if (hd !== null) {
                hd = JSON.parse(hd);
                // neu click ben ngoai modal delete --> bind hd
                console.log('hidden ', self.TongSoLuongHH());
                self.newHoaDon().SetData(hd[0]);
                if (hd[0].ID_DoiTuong !== null && hd[0].ID_DoiTuong !== undefined) {
                    getChiTietNCCByID(hd[0].ID_DoiTuong);
                }
                $('#selectedNV').val(hd[0].ID_NhanVien);
                Caculator_AmountProduct();
            }
        }
    });

    self.EditDaThanhToan = function () {
        var $this = $(event.currentTarget);
        formatNumberObj($this);
        var phaiTT = self.newHoaDon().TongTienHang() - self.newHoaDon().TongGiamGia();
        var daTT = formatNumberToFloat($this.val());
        self.newHoaDon().DaThanhToan(formatNumberToFloat($this.val()));
        self.newHoaDon().ConThieu(self.newHoaDon().PhaiThanhToan() - self.newHoaDon().DaThanhToan());

        var hd = localStorage.getItem(lcHDTraHangNhap);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hd[0].DaThanhToan = self.newHoaDon().DaThanhToan();
            hd[0].TienMat = self.newHoaDon().DaThanhToan();
            hd[0].ConThieu = self.newHoaDon().ConThieu();
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
        }
        var tienmat = phaiTT - daTT;
        tienmat = tienmat > 0 ? daTT : phaiTT;

        // reset phieuthu (if gara)
        vmThanhToan.PhieuThuKhach.TienMat = tienmat;
        vmThanhToan.PhieuThuKhach.TienPOS = 0;
        vmThanhToan.PhieuThuKhach.TienCK = 0;
        vmThanhToan.PhieuThuKhach.TienDatCoc = 0;
        vmThanhToan.PhieuThuKhach.TienTheGiaTri = 0;
        vmThanhToan.PhieuThuKhach.DaThanhToan = tienmat;
        vmThanhToan.PhieuThuKhach.ID_TaiKhoanPos = null;
        vmThanhToan.PhieuThuKhach.ID_TaiKhoanChuyenKhoan = null;
        vmThanhToan.PhieuThuKhach.HoanTraTamUng = 0;
        $('#tenloaitien').html('(Tiền mặt)');
    }

    self.ShowDivSale_HD = function () {
        $(".import-dropbox").hide();
        var $this = $('#saleMenuRight');
        $("#editGiamGia").show();
        $(function () {
            $("#saleNumber").select().focus();
        });

        $this.mouseup(function () {
            return false;
        });
        var isPtram = true;
        if (self.newHoaDon().TongChietKhau() > 0) {
            $("#saleNumber").val(formatNumber3Digit(self.newHoaDon().TongChietKhau()));
        }
        else {
            if (self.newHoaDon().TongGiamGia() > 0) {
                isPtram = false;
            }
            $("#saleNumber").val(formatNumber3Digit(self.newHoaDon().TongGiamGia()));
        }
        if (isPtram) {
            $('#vnd').removeClass('gb');
            $('#noVnd').addClass('gb');
        }
        else {
            $('#vnd').addClass('gb');
            $('#noVnd').removeClass('gb');
        }
    }

    self.EditGiamGia_HD = function () {
        var $this = $(event.currentTarget);
        var gtri = formatNumberToFloat($this.val());
        var tongGG = 0;
        if ($('#noVnd').hasClass('gb')) {
            tongGG = gtri * (self.newHoaDon().TongTienHang() + self.newHoaDon().TongTienThue()) / 100;
            self.newHoaDon().TongGiamGia(tongGG);
            self.newHoaDon().TongChietKhau(gtri);
        }
        else {
            tongGG = gtri;
            self.newHoaDon().TongGiamGia(gtri);
            self.newHoaDon().TongChietKhau(0);
        }

        var hd = localStorage.getItem(lcHDTraHangNhap);
        hd = JSON.parse(hd);
        hd[0].TongGiamGia = self.newHoaDon().TongGiamGia();
        hd[0].TongChietKhau = self.newHoaDon().TongChietKhau();
        localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));

        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            Bind_UpdateHD(cthd, 0);
        }
    }

    function Update_TienThue_forCTHD() {
        var ptThue = self.newHoaDon().TongTienThue() / (self.newHoaDon().TongTienHang()) * 100;
        var ptThueHD = self.newHoaDon().PTThueHoaDon();
        ptThueHD = ptThueHD > 0 ? ptThueHD : 0;
        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
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
            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
        }
    }

    function HD_UpdatePtramThue() {
        var lcHD = localStorage.getItem(lcHDTraHangNhap);
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            for (let i = 0; i < lcHD.length; i++) {
                lcHD[i].PTThueHoaDon = self.newHoaDon().PTThueHoaDon();
                break;
            }
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(lcHD));
        }
    }

    self.ShowDivTax_HD = function () {
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
                ptThue = 0;
            }
        }
        self.newHoaDon().TongTienThue(tienThue);
        self.newHoaDon().PTThueHoaDon(ptThue);

        var lcHD = localStorage.getItem(lcHDTraHangNhap);
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            for (let i = 0; i < lcHD.length; i++) {
                lcHD[i].PTThueHoaDon = ptThue;
                lcHD[i].TongTienThue = tienThue;
                break;
            }
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(lcHD));
        }

        Update_TienThue_forCTHD();// !!importtan update sau khi bind inforHD
        var cthd = localStorage.getItem(lcCTTraHangNhap);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            self.HangHoaAfterAdd(cthd);
            Bind_UpdateHD(cthd, 2);
        }
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

    self.UpdateGhiChuHD = function () {
        var hd = localStorage.getItem(lcHDTraHangNhap);
        if (hd !== null) {
            hd = JSON.parse(hd);
            hd[0].DienGiai = $(event.currentTarget).val();
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
        }
    }

    self.clickVND_HD = function () {
        var $this = $(event.currentTarget);
        $this.addClass('gb');
        $('#noVnd').removeClass('gb');
        $(function () {
            $("#saleNumber").select().focus();
        });

        var ptGG = self.newHoaDon().TongChietKhau();
        if (ptGG > 0) {
            self.newHoaDon().TongChietKhau(0);

            var hd = localStorage.getItem(lcHDTraHangNhap);
            hd = JSON.parse(hd);
            hd[0].TongChietKhau = 0;
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
        }
    }

    self.clickPtram_HD = function () {
        var $this = $(event.currentTarget);
        $this.addClass('gb');
        $('#vnd').removeClass('gb');
        $(function () {
            $("#saleNumber").select().focus();
        });

        var ptGG = self.newHoaDon().TongChietKhau();
        var tongGG = self.newHoaDon().TongGiamGia();
        if (ptGG === 0) {
            ptGG = tongGG / self.newHoaDon().TongTienHang() * 100;
            self.newHoaDon().TongChietKhau(ptGG);
            $("#saleNumber").val(formatNumber3Digit(ptGG));

            var hd = localStorage.getItem(lcHDTraHangNhap);
            hd = JSON.parse(hd);
            hd[0].TongChietKhau = ptGG;
            localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
        }
    }

    self.showPayBill = function () {
        vmThanhToan.isGara = false;

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
            + formatNumberToFloat(hd.TongChiPhi())
            - formatNumberToFloat(hd.KhachDaTra())

        var obj = {
            MaHoaDon: '',
            LoaiDoiTuong: 2,// 1.kh, 2.ncc, 3.bh
            LoaiHoaDon: 7,
            SoDuDatCoc: 0,
            HoanTra: 0,
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
        var hd = localStorage.getItem(lcHDTraHangNhap);
        hd = JSON.parse(hd);
        hd[0].DaThanhToan = self.newHoaDon().DaThanhToan();
        hd[0].ConThieu = self.newHoaDon().ConThieu();
        hd[0].TienMat = tienmat;
        hd[0].TienChuyenKhoan = ck;
        hd[0].TienPOS = pos;
        hd[0].TienDatCoc = phieuThu.TienDatCoc;
        hd[0].ID_TaiKhoanPos = phieuThu.ID_TaiKhoanPos;
        hd[0].TenTaiKhoanPos = phieuThu.TenTaiKhoanPos;
        hd[0].ID_TaiKhoanChuyenKhoan = phieuThu.ID_TaiKhoanChuyenKhoan;
        hd[0].TenTaiKhoanCK = phieuThu.TenTaiKhoanCK;
        hd[0].ID_KhoanThuChi = phieuThu.ID_KhoanThuChi;
        localStorage.setItem(lcHDTraHangNhap, JSON.stringify(hd));
    }

    self.deleteFileSelect = function () {
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadForm').value = "";
    }

    self.refreshFileSelect = function () {
        self.importTraHangNhap();
    }

    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".btnImportExcel").show();
        $(".filterFileSelect").show();
        self.loiExcel([]);
    }

    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachTraHangNhap.xls";
        window.location.href = url;
    }

    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "FileImport_DanhSachTraHangNhap.xlsx";
        window.location.href = url;
    }

    self.importTraHangNhap = function () {
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
            url: DMHangHoaUri + "ImfortExcelTraHangNhap",
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
                        url: DMHangHoaUri + "getList_DanhSachTraHangNhap?iddonvi=" + _idDonVi,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (item) {
                            self.deleteFileSelect();

                            console.log('import ', item);
                            var arrCTsort = item;
                            var arrIDQuiDoi = [];
                            var cthdLoHang = [];
                            for (let i = 0; i < arrCTsort.length; i++) {
                                arrCTsort[i].ID_Random = CreateIDRandom('CTHD_');
                                arrCTsort[i].ID_HangHoa = arrCTsort[i].ID;
                                arrCTsort[i].TienChietKhau = 0;
                                arrCTsort[i].DVTinhGiam = '%';
                                arrCTsort[i].PTChietKhau = 0;
                                arrCTsort[i].PTThue = 0;
                                arrCTsort[i].TienThue = 0;
                                arrCTsort[i].GhiChu = '';
                                arrCTsort[i].LaConCungLoai = false;
                                arrCTsort[i].HangCungLoais = [];

                                let idLoHang = arrCTsort[i].ID_LoHang;
                                let quanlytheolo = arrCTsort[i].QuanLyTheoLoHang;
                                arrCTsort[i].QuanLyTheoLoHang = quanlytheolo;
                                arrCTsort[i].DM_LoHang = [];
                                arrCTsort[i].ID_LoHang = idLoHang;
                                arrCTsort[i].LotParent = quanlytheolo;
                                arrCTsort[i].SoThuTu = cthdLoHang.length + 1;

                                let dateLot = GetNgaySX_NgayHH(itemFor);
                                arrCTsort[i].NgaySanXuat = dateLot.NgaySanXuat;
                                arrCTsort[i].NgayHetHan = dateLot.NgayHetHan;

                                if ($.inArray(arrCTsort[i].ID_DonViQuiDoi, arrIDQuiDoi) === -1) {
                                    arrIDQuiDoi.unshift(arrCTsort[i].ID_DonViQuiDoi);
                                    if (quanlytheolo) {
                                        let objLot = $.extend({}, arrCTsort[i]);
                                        objLot.DM_LoHang = [];
                                        objLot.HangCungLoais = [];
                                        arrCTsort.DM_LoHang.push(objLot);
                                    }
                                    cthdLoHang.push(arrCTsort[i]);
                                }
                                else {
                                    for (let j = 0; j < cthdLoHang.length; j++) {
                                        if (cthdLoHang[j].ID_DonViQuiDoi === arrCTsort[i].ID_DonViQuiDoi) {
                                            if (quanlytheolo) {
                                                let objLot = $.extend({}, arrCTsort[i]);
                                                objLot.SoLuong = arrCTsort[i].SoLuong;
                                                objLot.GiaNhap = arrCTsort[i].GiaNhap;
                                                objLot.DonGia = arrCTsort[i].DonGia;
                                                objLot.ThanhTien = objLot.SoLuong * objLot.GiaNhap;
                                                objLot.ThanhToan = objLot.ThanhTien;
                                                objLot.DM_LoHang = [];
                                                objLot.HangCungLoais = [];
                                                cthdLoHang[j].DM_LoHang.push(objLot);
                                                break;
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            var cthd = localStorage.getItem(lcCTTraHangNhap);
                            if (cthd !== null) {
                                cthd = JSON.parse(cthd);
                            }
                            else {
                                cthd = [];
                            }
                            for (let i = 0; i < cthdLoHang.length; i++) {
                                cthd.unshift(cthdLoHang[i]);
                            }
                            localStorage.setItem(lcCTTraHangNhap, JSON.stringify(cthd));
                            self.HangHoaAfterAdd(cthd);
                            Bind_UpdateHD(cthd);
                            ShowMessage_Success('Import file thành công')
                        },
                        error: function (x) {
                            console.log('getList_DanhSachTraHangNhap ', x);
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
    self.PageSize = ko.observable(10);
    self.FromItem = ko.observable(0);
    self.ToItem = ko.observable(0);

    self.HangHoaAfterAdd_View = ko.computed(function () {
        var first = self.CurrentPage() * self.PageSize();
        if (self.HangHoaAfterAdd() !== null) {
            var cthd = localStorage.getItem(lcCTTraHangNhap);
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

    $('#ThongTinThanhToanNCC').on('hidden.bs.modal', function () {
        if (vmThanhToan.saveOK) {
            self.AgreePay();
        }
        else {
            // get data from cache (neu truoc do Agree, chi show modal kiemtra lai)
            let hd = localStorage.getItem(lcHDTraHangNhap);
            if (hd !== null) {
                hd = JSON.parse(hd);

                vmThanhToan.PhieuThuKhach.DaThanhToan = hd[0].DaThanhToan;
                vmThanhToan.PhieuThuKhach.TienMat = hd[0].TienMat;
                vmThanhToan.PhieuThuKhach.TienPOS = hd[0].TienPOS;
                vmThanhToan.PhieuThuKhach.TienCK = hd[0].TienChuyenKhoan;
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
    })
    $('#vmThemNhomNCC').on('hidden.bs.modal', function () {
        var typeUpdate = vmThemMoiNhomNCC.typeUpdate;
        var nhom = vmThemMoiNhomNCC.newGroup;
        if (vmThemMoiNhomNCC.saveOK) {
            for (let i = 0; i < self.NhomDoiTuongs().length; i++) {
                if (self.NhomDoiTuongs()[i].ID === nhom.ID) {
                    self.NhomDoiTuongs.remove(self.NhomDoiTuongs()[i]);
                    break;
                }
            };
            if (typeUpdate !== 0) {
                self.NhomDoiTuongs.unshift(nhom);
            }
        }
    })
}

var modelGiaoDich = new TraHangNhapChiTiet();
ko.applyBindings(modelGiaoDich, document.getElementById('divPage'));

function jqAutoSelectItem(item) {
    modelGiaoDich.JqAutoSelectItem(item);
}

function keypressEnterSelected(e) {
    if (e.keyCode === 13) {
        modelGiaoDich.JqAutoSelect_Enter();
    }
}

$(window.document).on('shown.bs.modal', '.modal', function () {
    window.setTimeout(function () {
        $('[autofocus]', this).focus();
        $('[autofocus]').select();
    }.bind(this), 100);
});
