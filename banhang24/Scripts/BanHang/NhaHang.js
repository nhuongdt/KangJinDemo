var hub = $.connection.AlertHub;
var connected = false;
$.connection.hub.start().done(function () {
    connected = true;
});

$.connection.hub.disconnected(function () {
    connected = false;
    setTimeout(function () {
        $.connection.hub.start().done(function () {
            connected = true;
            console.log('connect again hub');
        });
    }, 5000);
});

var NewModel_HoaDon = function () {
    var self = this;
    var DM_PhongbanUri = '/api/DanhMuc/DM_ViTriAPI/';
    var DM_NhomBanUri = '/api/DanhMuc/DM_KhuVucAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DM_HangHoaUri = "/api/DanhMuc/DM_HangHoaAPI/";
    var BH_HoaDonUri = "/api/Danhmuc/BH_HoaDonAPI/";
    var DMNguonKhachUri = '/api/DanhMuc/DM_NguonKhachAPI/';
    var DMNhomDoiTuongUri = '/api/DanhMuc/DM_NhomDoiTuongAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    const lcListHD = 'lstHDTemp';
    const lcListCTHD = 'listAllCTHD';
    const lcPhongBanID = 'phongbanIDCache';
    const lcListKHoffline = 'DM_DoiTuongOffline';
    var tree = '';
    var _maHoaDon = '1';
    var _idDonVi = $('#txtDonVi').val();
    var _idUser = $('#txtIDUser').val();
    var _userLogin = $('#txtUserLogin').val();
    var _idNhanVien = $('#txtIDNhanVien').val();
    var _phongBanID = localStorage.getItem(lcPhongBanID);
    var tlapHeThong = localStorage.getItem('lc_CTThieTLap');
    if (tlapHeThong !== null) {
        _idDonVi = JSON.parse(localStorage.getItem('lc_CTThieTLap')).ID_DonVi;
    }

    $('#inputFocus').focus();
    $('#lblHideNCC').text('Khách lẻ');
    $('input,select').removeAttr('disabled');

    self.selectedGiaBan = ko.observable();
    self.selectedNVien = ko.observable();
    self.selectedPhongBan = ko.observable();
    self.selectedPhongBan_GhepHoaDon = ko.observable();
    self.selectedTheKH = ko.observable();
    self.selectedNCC = ko.observable(null);
    self.selectedNhomHH = ko.observable();
    self.booleanAdd = ko.observable(true);
    self.booleanAddNhomDT = ko.observable(true);

    self.checkEmail = ko.observable(true);
    self.countHDoffilne = ko.observable();
    self.CheckChuyenBan = ko.observable(0);
    self.choseHDKH = ko.observable('');
    self.filterPB = ko.observable();
    self.countTableUsed = ko.observable();
    self.selectedStatus = ko.observable(0);
    self.windowWidth = ko.observable($(window).width());
    self.FileImgs = ko.observableArray();
    self.FilesSelect = ko.observableArray();
    self.selectLoaiChungTu = ko.observable();
    self.NgayLapHoaDon = ko.observable(moment(new Date()).format('DD/MM/YYYY HH:mm'));

    self.DMChungTus = ko.observableArray([
        { MaLoaiChungTu: 0, TenLoaiChungTu: 'Tất cả' },
        { MaLoaiChungTu: 1, TenLoaiChungTu: 'Hóa đơn' },
        { MaLoaiChungTu: 3, TenLoaiChungTu: 'Đặt hàng' },
    ]);

    self.newAccountBank = ko.observable(new FormModel_BankAccount());

    // Phieu Thu
    self.IDDoiTuong_PhieuThu = ko.observable();
    self.ChiTietKH_PhieuThu = ko.observableArray();
    self.NoSau = ko.observable();
    self.ThuTuKhach = ko.observable();
    self.GhiChu_PhieuThu = ko.observable();
    self.NgayLapPhieuThu = ko.observable(moment(new Date()).format('DD/MM/YYYY HH:mm'));
    self.TienMat_PhieuThu = ko.observable();
    self.TienATM_PhieuThu = ko.observable();
    self.TienGui_PhieuThu = ko.observable();
    self.KhachTT_PhieuThu = ko.observable();
    self.TongThanhToan = ko.observable(0);
    self.CongVaoTK = ko.observable(0);
    self.ListHDisDebit = ko.observableArray();
    self.selectID_POSPT = ko.observable();
    self.selectID_ChuyenKhoanPT = ko.observable();
    self.SelectTime = ko.observableArray();
    self.ListTime = ko.observableArray();
    self.DM_MauIn = ko.observableArray();
    self.ListAllDoiTuong = ko.observableArray();
    self.NhanViens_ChiNhanh = ko.observableArray();
    self.PhongBans_ChuyenBan = ko.observableArray();
    self.InforHDprintf = ko.observableArray();
    //phan trang: TableUsed
    self.currTblUsed = ko.observable(0);
    self.currNhomHH = ko.observable(0);
    self.pageSizeHD = ko.observable(4);
    // phan trang DS HoaDon
    if (self.windowWidth() === 768) {
        self.pageSizeHD(3);
    }

    self.ThietLap_TichDiem = ko.observableArray(); // thiet lap tich diem
    self.DienHienTai = ko.observable(0); // bind at popThanhToanThe
    self.TienQuyDoi = ko.observable(0); // so tien quy doi duoc tu Diem

    self.AllPhongBans = ko.observableArray();
    self.PhongBans = ko.observableArray();// phong by nhom
    self.NhomBans = ko.observableArray();
    self.HangHoas = ko.observableArray();
    self.HangHoaAll = ko.observableArray();
    self.HangHoaOlds = ko.observableArray();
    self.HangHoaAfterAdds = ko.observableArray();
    self.PageResults = ko.observableArray();
    self.HoaDons = ko.observable(new FormModel_NewInvoice());
    self.NhanViens = ko.observableArray();
    self.titleNhanvien = ko.observable(0);
    self.titleGiaban = ko.observable(0);

    self.DoiTuongs = ko.observableArray();
    self.AllQuanHuyens = ko.observableArray();
    self.NguonKhachs = ko.observableArray();
    self.NhomDoiTuongDB = ko.observableArray();
    self.NhomDoiTuongDB = ko.observableArray();

    self.ChiTietHangHoa = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    self.GiaBans = ko.observableArray();
    self.AllBangGia = ko.observableArray();
    self.ChiTietDoiTuong = ko.observableArray();
    self.IndexHD_inTable = ko.observable(0);
    self.PhongBanSelected = ko.observableArray(); // DS HoaDon duoc them vao o class slide-auto
    self.HoaDonByIDViTri = ko.observableArray();
    self.HoaDonOffline = ko.observableArray();
    self.TheKhachHang = ko.observableArray();
    self.CongTy = ko.observableArray();
    self.PhongBanUsed = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.ThietLap = ko.observableArray();
    self.AllGiaBanChiTiet = ko.observableArray();
    self.TrangThaiKhachHang = ko.observableArray();
    self.Quyen_NguoiDung = ko.observableArray();
    self.NguonKhachChosed = ko.observableArray();
    self.DM_NhomDoiTuong_ChiTiets = ko.observableArray();
    self.DM_NhomDoiTuong_ChiTiets_Unique = ko.observableArray();
    self.ListGroupSale = ko.observableArray();
    self.currentPage = ko.observable(0);
    self.ChiNhanhs = ko.observableArray();
    self.VungMiens = ko.observableArray();

    // Thanh toan = The, NganHang
    self.ListAccountPOS = ko.observableArray();
    self.ListAccountChuyenKhoan = ko.observableArray();
    self.selectID_POS = ko.observable();
    self.selectID_ChuyenKhoan = ko.observable();
    self.filterAcPOS = ko.observable();
    self.filterAcCK = ko.observable();
    self.shareMoneys = ko.observableArray();

    // Setting print invoice
    self.setLuuTam = ko.observable(false);
    self.setViewSoDoPhong = ko.observable(false);
    self.setNhapQuyCach = ko.observable(false);
    self.setHideProduct = ko.observable();
    self.setPrintDraft = ko.observable(false);
    self.setPrintPay = ko.observable(false);
    self.setPrintTicket = ko.observable(false);
    self.numberOfPrint = ko.observable(1);
    //self.selectedTempPrint = ko.observable();
    self.ListTempPrint = ko.observableArray();
    self.ChotSo_ChiNhanh = ko.observableArray();
    self.roomEmpty = ko.observable(0);
    self.roomBusy = ko.observable(0);

    // pagesize default
    self.PageSizeProductGroup = ko.observable(10);
    self.PageSizeTable = ko.observable(24); // PhongBan
    self.PageSizeProduct = ko.observable(25); // HangHoa
    self.PageSizeTableUse = ko.observable(4);
    self.currentPageProducts = ko.observable(0);

    // ipad
    if (self.windowWidth() <= 768) {
        self.PageSizeProductGroup(1);
        self.PageSizeTable(6);
        self.PageSizeProduct(5);
        self.PageSizeTableUse(3);
    }
    else {
        if (self.windowWidth() <= 1024) {
            // ipad pro
            self.PageSizeProductGroup(3);
            self.PageSizeTable(8);
            self.PageSizeProduct(12);
        }
        else {
            if (self.windowWidth() <= 1400) {
                // desktop
                self.PageSizeProductGroup(7);
                self.PageSizeTable(24);
                self.PageSizeProduct(25);
            }
        }
    }

    var _subdomain = $('#subDomain').val();
    var db = new Dexie(_subdomain);
    db.version(1).stores({
        DM_DoiTuong: 'ID, Name_Phone',
        DM_HangHoa: 'ID, Name',
        NS_NhanVien: '++id',
        DM_GiaBan: 'ID',
        HeThongTichDiem: 'ID_TichDiem',
        DM_MauIn: 'ID',
        Quyen_NguoiDung: '++id',
        HT_NguoiDung: 'ID',
        DM_QuanHuyen: 'ID',
        DM_LoHang: '++id',
        DM_DonVi: 'ID',
        KhachHang_HangHoa: 'ID_DoiTuong',
        ChietKhau_NhanVien: '++id',
        DM_KhuyenMai: 'ID',
        DM_TaiKhoanNganHang: 'ID',
        DM_NhanVienLienQuan: '++id',
        DM_ViTri: 'ID',
        DM_KhuVuc: 'ID',
        DM_DoiTuong_TrangThai: 'ID',
    });

    function WriteData_Dexie(tbl, data) {
        db.transaction("rw", tbl, () => {
            tbl.bulkPut(data);
        }).catch(function (e) {
            //console.log(tbl.name, e);
        });
    }

    function Page_Load() {
        AssignSomeNewProperties_forCTHD();
        UpdateAppCache();
        GetDMGiaBan_GBApDung();
        GetHT_Quyen_ByNguoiDung();
        getAllDMNhomHangHoas();
        GetCauHinhHeThong();
        GetDM_TaiKhoanNganHang();
        GetDM_NhomDoiTuong_ChiTiets();
        Call_ManyFunctionDB_PageLoad();
        GetInforChiNhanh();
        GetNhanVien_NguoiDung();
        getAllDMHangHoas();
        getAllDMDoiTuong();
        GetHT_TichDiem();
        LoadTrangThai();
        Call_ManyFunction_Onsuccess();
    }

    Page_Load();

    function AssignSomeNewProperties_forCTHD() {
        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].DichVuTheoGio === undefined) {
                    cthd[i].DichVuTheoGio = 0;
                    cthd[i].Stop = false;
                }
                if (cthd[i].DuocTichDiem === undefined) {
                    cthd[i].DuocTichDiem = 1;
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
        }
    }

    function GetListPrinter() {
        $.getJSON(BH_HoaDonUri + 'GetListPrinter', function (x) {
            console.log(x)
        });
    }

    function Call_ManyFunctionDB_PageLoad() {
        if (navigator.onLine) {
            ajaxHelper(BH_HoaDonUri + "BanHang_ManyFunctionDB_PageLoad?idChiNhanh=" + _idDonVi, 'GET').done(function (obj) {
                if (obj.res === true) {
                    newModal_AddKhachHang.TinhThanhs(obj.TinhThanh);
                    newModal_AddKhachHang.NguonKhachs(obj.NguonKhach);
                    newModal_AddKhachHang.VungMiens(obj.VungMien);

                    self.CongTy(obj.CongTy);
                    self.ChotSo_ChiNhanh(obj.ChotSo);

                    localStorage.setItem('lc_TinhThanhs', JSON.stringify(obj.TinhThanh));
                    localStorage.setItem('lc_NguonKhachs', JSON.stringify(obj.NguonKhach));
                    localStorage.setItem('lcCongTy', JSON.stringify(obj.CongTy));
                    localStorage.setItem('lc_ChotSo', JSON.stringify(obj.ChotSo));
                    localStorage.setItem('DM_VungMien', JSON.stringify(obj.VungMien));
                }
            });
        }
        else {
            var tinhthanh = localStorage.getItem('lc_TinhThanhs');
            if (tinhthanh !== null) {
                tinhthanh = JSON.parse(tinhthanh);
                newModal_AddKhachHang.TinhThanhs(tinhthanh);
            }

            var nguonkhach = localStorage.getItem('lc_NguonKhachs');
            if (nguonkhach !== null) {
                nguonkhach = JSON.parse(nguonkhach);
                newModal_AddKhachHang.NguonKhachs(nguonkhach);
            }

            var vungmien = localStorage.getItem('DM_VungMien');
            if (vungmien !== null) {
                vungmien = JSON.parse(vungmien);
                newModal_AddKhachHang.VungMiens(vungmien);
            }

            var congty = localStorage.getItem('lcCongTy');
            if (congty !== null) {
                congty = JSON.parse(congty);
                self.CongTy(congty);
            }

            var chotso = localStorage.getItem('lc_ChotSo');
            if (chotso !== null) {
                chotso = JSON.parse(chotso);
                self.ChotSo_ChiNhanh(chotso);
            }
        }
    }

    function Call_ManyFunction_Onsuccess() {
        if (navigator.onLine) {
            ajaxHelper(DMDoiTuongUri + 'Call_ManyFunction_OnsuccessBanHang?idChiNhanh=' + _idDonVi, 'GET').done(function (obj) {
                if (obj.res == true) {
                    self.DM_MauIn(obj.TemPrint);
                    self.BindSettingPrint(true);
                    self.AllQuanHuyens(obj.District);
                    self.NhomBans(obj.NhomBan);
                    for (let i = 0; i < obj.PhongBan.length; i++) {
                        obj.PhongBan[i].DateStart = "0";
                        obj.PhongBan[i].CreateTime = "0";
                        obj.PhongBan[i].WorkingTime = 0;
                        obj.PhongBan[i].SoLuongKhachHang = 0;
                    }
                    var arrSortPB = obj.PhongBan.sort((a, b) => a.TenViTri.localeCompare(b.TenViTri, undefined, { caseFirst: "upper" }));
                    arrSortPB.unshift({
                        ID: '00000000-0000-0000-0000-000000000000',
                        TenViTri: 'Mang về',
                        WorkingTime: 1,
                        TenKhuVuc: '',
                        SoLuongKhachHang: 0,
                        DateStart: '0',
                        CreateTime: "0",
                    })
                    self.PhongBans(arrSortPB);
                    self.AllPhongBans(arrSortPB);

                    GetAllHD_fromDB(true);

                    WriteData_Dexie(db.DM_QuanHuyen, obj.District);
                    WriteData_Dexie(db.DM_MauIn, obj.TemPrint);
                    WriteData_Dexie(db.DM_KhuVuc, obj.NhomBan);
                    WriteData_Dexie(db.DM_ViTri, self.PhongBans());
                }
            })
        }
        else {
            db.DM_QuanHuyen.toArray(function (dt) {
                self.AllQuanHuyens(dt);
            });
            db.DM_MauIn.toArray(function (dt) {
                self.DM_MauIn(dt);
            });
            db.DM_KhuVuc.toArray(function (dt) {
                self.AllNhomBans(dt);
            });
            db.DM_ViTri.toArray(function (dt) {
                self.PhongBans(dt);
                self.AllPhongBans(dt);
            });
        }
    }

    function GetHDOpening_ofTable() {
        var arr = [];
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            arr = $.grep(lstHD, function (x) {
                return x.ID_ViTri === _phongBanID && x.ID_DonVi === _idDonVi && x.TrangThaiHD === 1;
            });
        }
        return arr;
    }

    function BindHD_CTHD(maHoaDon) {
        var arrHD = GetHDOpening_ofTable();
        if (arrHD.length > 0) {
            let hdOpen = [];
            //var index = 0;
            // bind hoadon voi mahoadon duoc gan (sau khi chuyenghep)
            if (maHoaDon !== undefined) {
                _maHoaDon = maHoaDon;
                hdOpen = $.grep(arrHD, function (x, i) {
                    //index = i;
                    return x.MaHoaDon === maHoaDon;
                });
            }

            if (hdOpen.length > 0) {
                hdOpen = hdOpen[0];
            }
            else {
                //index = 0;
                hdOpen = arrHD[0];
            }
            _maHoaDon = hdOpen.MaHoaDon;

            self.PhongBanSelected(arrHD);
            self.SetNhanVien(hdOpen.ID_NhanVien);
            if (self.selectedGiaBan() !== hdOpen.ID_BangGia) {
                UpdateGiaBan_inListHangHoa_byPage(hdOpen.ID_BangGia);
            }
            self.SetBangGia(hdOpen.ID_BangGia);
            getChiTietNCCByID(hdOpen.ID_DoiTuong);
            self.HoaDons().SetData(hdOpen);
            BindCTHD_byIDRandomHD(hdOpen.IDRandom);
            SetText_lblTienMat(hdOpen, 1);
        }
        else {
            _maHoaDon = '1';
            var objEmpty = newHoaDon(_phongBanID, '1');
            self.PhongBanSelected([objEmpty]);
            self.resetInforHD_CTHD();
            $('#lblTienMat').text('(Tiền mặt)');
        }
        StyDSHoaDon_byMaHoaDon(_maHoaDon);
        Enable_DisableNgayLapHD();
    }

    function CheckTrangThaiHD_undefined() {
        var lstHD = localStorage.getItem(lcListHD);
        var lstHDaddDB = [];
        if (lstHD !== null) {
            lstHD = [];
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].TrangThaiHD === undefined) {
                    lstHD[i].TrangThaiHD = 1;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
        }
    }

    function GetAllHD_fromDB(firstLoad) {
        $('#divContent').show();
        $('#divHeaderRight').show();

        var lstHD = localStorage.getItem(lcListHD);
        var lstHDaddDB = [];
        var arrIDRandom = [];
        var arrMaHD = [];
        if (lstHD === null) {
            lstHD = [];
        }
        else {
            lstHD = JSON.parse(lstHD);
            // không xóa hóa đơn có trong DB và dc mở lại để thêm chi tiết
            lstHD = $.grep(lstHD, function (x) {
                return x.ID === const_GuidEmpty || (x.ID !== const_GuidEmpty && x.TrangThaiHD === 1);
            });
            // only get hd of this chinhanh
            lstHD = $.grep(lstHD, function (x) {
                return x.ID_DonVi === _idDonVi;
            });
            // get all IDRandom of HD don't delete
            for (let i = 0; i < lstHD.length; i++) {
                arrIDRandom.push(lstHD[i].IDRandom);
                if (lstHD[i].ID !== const_GuidEmpty) {
                    arrMaHD.push(lstHD[i].MaHoaDon);
                }
            }
        }

        var lstCTHD = localStorage.getItem(lcListCTHD);
        if (lstCTHD === null) {
            lstCTHD = [];
        }
        else {
            lstCTHD = JSON.parse(lstCTHD);
            // delete all CTHD with ID_HoaDon !== Guid.Empty
            lstCTHD = $.grep(lstCTHD, function (x) {
                return $.inArray(x.IDRandomHD, arrIDRandom) > -1;
            });
        }

        if (navigator.onLine) {
            ajaxHelper(BH_HoaDonUri + 'GetListHoaDons_ChoThanhToanNhaBep?loaiHoaDon=3&idDonVi=' + _idDonVi, 'GET').done(function (data) {
                if (data !== null) {
                    let maHDDB = [];
                    for (let i = 0; i < data.length; i++) {
                        maHDDB.push(data[i].MaHoaDon);
                        if ($.inArray(data[i].MaHoaDon, arrMaHD) === -1) {
                            data[i].IDRandom = CreateIDRandom('HD_');

                            for (let j = 0; j < data[i].BH_HoaDon_ChiTiet.length; j++) {
                                let itemCTHD = data[i].BH_HoaDon_ChiTiet[j];
                                itemCTHD.MaHoaDon = data[i].MaHoaDon;
                                itemCTHD.IDRandom = CreateIDRandom('cthd_');
                                itemCTHD.ID_ViTri = data[i].ID_ViTri;
                                itemCTHD.LoaiHoaDon = data[i].LoaiHoaDon;
                                itemCTHD.CssWarning = false;
                                itemCTHD.DVTinhGiam = 'VND';
                                if (itemCTHD.PTChietKhau > 0) {
                                    itemCTHD.DVTinhGiam = '%';
                                }
                                itemCTHD.TienChietKhau = itemCTHD.GiamGia;
                                itemCTHD.ShowProductName = true;
                                itemCTHD.ShowProductPrice = true;
                                itemCTHD.ShowSumPrice = true;
                                itemCTHD.IDRandomHD = data[i].IDRandom;
                                itemCTHD.ID_User = _idUser;
                                itemCTHD.GhiChu_NVTuVan = ''
                                itemCTHD.GhiChu_NVTuVanPrint = '';
                                itemCTHD.GhiChu_NVThucHien = '';
                                itemCTHD.GhiChu_NVThucHienPrint = '';

                                let dvtheogio = itemCTHD.DichVuTheoGio;
                                itemCTHD.Stop = false;
                                itemCTHD.ThoiGianHoanThanh = dvtheogio ? moment(new Date()).format('YYYY-MM-DD HH:mm') : null;
                                itemCTHD.DichVuTheoGio = dvtheogio;

                                // add again CTHD
                                lstCTHD.push(itemCTHD);
                            }
                            // delete key BH_HoaDon_ChiTiet in data
                            delete data[i]['BH_HoaDon_ChiTiet'];
                            data[i].CreateTime = moment(data[i].NgayLapHoaDon).format('HH:mm');
                            data[i].StatusOffline = false;
                            data[i].HoanTraTamUng = 0;
                            data[i].DiemGiaoDich = 0;
                            data[i].TTBangDiem = 0;
                            data[i].DiemHienTai = 0;
                            data[i].DiemQuyDoi = 0; // so diem quy doi tu tien --> save in Quy_HoaDon_ChiTiet
                            data[i].ID_User = _idUser;
                            data[i].GioVao = convertDateTime(data[i].GioVao);
                            data[i].GioRa = convertDateTime(data[i].GioRa);
                            data[i].ID_TaiKhoanPos = null; // set = null, vi co the TT nhieu lan, nhieu ID_TaiKhoan khac nhau
                            data[i].ID_TaiKhoanChuyenKhoan = null;
                            data[i].PTThue = RoundDecimal(data[i].TongTienThue / (data[i].TongTienHang - data[i].TongGiamGia) * 100);
                            data[i].PTThue = isNaN(data[i].PTThue) ? 0 : data[i].PTThue;

                            // Khuyen mai
                            data[i].IsKhuyenMaiHD = false;
                            data[i].PTGiam_KM = 0;
                            data[i].KhuyeMai_GiamGia = 0;
                            data[i].TongGiamGiaKM_HD = data[i].TongGiamGia;//= TongGG vi DatHang khong co KMai
                            data[i].KhuyenMai_GhiChu = '';
                            data[i].IsOpeningKMaiHD = false;
                            // giam gia theo nhom
                            data[i].ID_NhomDTApplySale = null;
                            data[i].TrangThaiHD = 1;

                            lstHD.push(data[i]);
                        }
                    }
                    // find HD with Status= 1 but was delete
                    var arrHDDelete = $.grep(arrMaHD, function (x) {
                        return $.inArray(x, maHDDB) === -1;
                    });
                    lstHD = $.grep(lstHD, function (x) {
                        return $.inArray(x.MaHoaDon, arrHDDelete) === -1;
                    });
                    lstCTHD = $.grep(lstCTHD, function (x) {
                        return $.inArray(x.MaHoaDon, arrHDDelete) === -1;
                    });
                }
                localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));

                UpdateTime_lcPhongBan();
                CountTableUsed(false);
                BindHD_CTHD();
                Bind_andCountHDO();
            });
        }
        else {
            UpdateTime_lcPhongBan();
            CountTableUsed(false);
            BindHD_CTHD();
            Bind_andCountHDO();
        }
    }

    function StyDSHoaDon_byMaHoaDon(maHoaDon) {
        $('.divLstHoaDon span').removeClass('active');
        $(function () {
            $('.divLstHoaDon span.munber-goods').each(function () {
                if ($(this).text() === _maHoaDon) {
                    $(this).parent().parent().addClass('active');
                    return;
                }
            });
        });
    }

    function getListNhomDT() {
        if (navigator.onLine) {
            ajaxHelper(DMDoiTuongUri + 'GetNhomDoiTuong_DonVi?loaiDT=1', 'GET').done(function (obj) {
                if (obj.res === true) {
                    let data = obj.data;
                    for (var i = 0; i < data.length; i++) {
                        let tenNhom = data[i].TenNhomDoiTuong;
                        tenNhom = tenNhom.concat(' ', locdau(tenNhom), ' ', GetChartStart(tenNhom));
                        data[i].Text_Search = tenNhom;
                    }
                    if (self.ThietLap().QuanLyKhachHangTheoDonVi) {
                        // only get Nhom chua cai dat ChiNhanh or in this ChiNhanh
                        var arrNhom = [];
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].NhomDT_DonVi.length > 0) {
                                let ex = $.grep(data[i].NhomDT_DonVi, function (x) {
                                    return x.ID === _idDonVi;
                                });
                                if (ex.length) {
                                    arrNhom.push(data[i]);
                                }
                            }
                            else {
                                arrNhom.push(data[i]);
                            }
                        }
                        newModal_AddKhachHang.NhomDoiTuongDB(arrNhom);
                    }
                    else {
                        newModal_AddKhachHang.NhomDoiTuongDB(data);
                    }
                }
            });
        }
        else {
            var lc_NhomDT = localStorage.getItem('lc_NhomDT');
            if (lc_NhomDT !== null) {
                lc_NhomDT = JSON.parse(lc_NhomDT);
                newModal_AddKhachHang.NhomDoiTuongDB(lc_NhomDT);
            }
        }
    }

    function getAllDMHangHoas() {
        if (navigator.onLine) {
            ajaxHelper(DM_HangHoaUri + "GetListHangHoas_QuyDoiNH_Anh_IEnumerable?iddonvi=" + _idDonVi, 'GET').done(function (data) {
                if (data !== null) {

                    self.HangHoaOlds($.extend(true, [], data));
                    self.HangHoaAll($.extend(true, [], data));
                    self.HangHoas($.extend(true, [], data)); // not change when change ID_NhomHH
                    BindListHangHoa_ByPage();

                    WriteData_Dexie(db.DM_HangHoa, data);

                    // update TonKho in cache CTHD
                    var lstCTHD = localStorage.getItem(lcListCTHD);
                    if (lstCTHD !== null) {
                        lstCTHD = JSON.parse(lstCTHD);
                        for (let i = 0; i < lstCTHD.length; i++) {
                            for (let j = 0; j < data.length; j++) {
                                if (lstCTHD[i].ID_DonViQuiDoi === data[j].ID_DonViQuiDoi) {
                                    lstCTHD[i].TonKho = data[j].TonKho;

                                    // show/ hide warning TonKho at CTHD (OK)
                                    if (self.ThietLap().XuatAm === false && lstCTHD[i].LaHangHoa && lstCTHD[i].TonKho < lstCTHD[i].SoLuong) {
                                        lstCTHD[i].CssWarning = true;
                                        //$('#show_wr_' + lstCTHD[i].IDRandom).css('display', '');
                                    }
                                    else {
                                        lstCTHD[i].CssWarning = false;
                                        //$('#show_wr_' + lstCTHD[i].IDRandom).css('display', 'none');
                                    }

                                }
                            }
                        }
                        localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));
                    }
                }
            });
        }
        else {
            db.DM_HangHoa.toArray(function (dt) {
                self.HangHoaOlds(dt);
                self.HangHoas(dt);
                self.HangHoaAll(dt);
            });
        }
    }

    function GetDataChotSo() {
        // to do check NgayLapHD
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/GetDataChotSo?idChiNhanh=' + _idDonVi, 'GET').done(function (data) {
                if (data !== null && data.length > 0) {
                    self.ChotSo_ChiNhanh(data);
                    localStorage.setItem('lc_ChotSo', JSON.stringify(data));
                }
            })
        }
        else {
            var lcChotSo = localStorage.getItem('lc_ChotSo')
            if (lcChotSo !== null) {
                lcChotSo = JSON.parse(lcChotSo);
                self.ChotSo_ChiNhanh(lcChotSo);
            }
        }
    }



    self.selectted = ko.observable(0);
    var vm = new Vue({
        el: '#el',
        data: function () {
            return {
                query: '',
                filters: [],
            }
        },
        methods: {
            reset: function (item) {
                this.filters = [];
                this.query = '';
            },
            submit: function (event) {
                let keyCode = event.keyCode;
                switch (keyCode) {
                    case 13:
                        var result = this.findBy(this.query);
                        var focus = false;
                        $('#showseachPage li').each(function (i) {
                            if ($(this).hasClass('hoverenabled')) {
                                self.addHangHoa(result[i]);
                                focus = true;
                                $('#inputFocus').val(result[i].MaHangHoa);
                            }
                        });
                        if (result.length > 0 && this.query !== '' && focus === false) {
                            self.addHangHoa(result[0]);
                            $('#inputFocus').val(result[0].MaHangHoa);
                        }
                        $('#inputFocus').focus().select();
                        break;
                    case 37:// left
                    case 38:// up
                        self.selectted(self.selectted() - 1);
                        if (self.selectted() < 0) {
                            self.selectted($("#showseachPage li").length - 1);
                        }
                        this.loadfocus();
                        break;
                    case 39:// right
                    case 40://down
                        self.selectted(self.selectted() + 1);
                        if (self.selectted() >= ($("#showseachPage li").length)) { self.selectted(0); }
                        this.loadfocus();
                        break;
                }
            },
            findBy: function (value) {
                let txt = locdau(value);
                // select top 30
                let lst = self.HangHoaAll().filter(function (item) {
                    return item['Name'].indexOf(txt) > -1;
                });
                return lst;
            },
            loadfocus: function () {
                $('#showseachPage li').each(function (i) {
                    $(this).removeClass('hoverenabled');
                    if (i === self.selectted()) {
                        $(this).addClass('hoverenabled');
                    }
                });
                $('#inputFocus').focus();
            }

        },
        computed: {
            searchResult: function () {
                self.currentPageProducts(0);
                var result = this.findBy(this.query.trim());
                self.HangHoas(result);
                UpdateGiaBan_inListHangHoa_byPage(self.selectedGiaBan());

                $('#showseachPage li').each(function (i) {
                    $(this).removeClass('hoverenabled');
                    if (i === 0) {
                        $(this).addClass('hoverenabled');
                    }
                });
                self.selectted(0);
                $('#inputFocus').focus();
                return result;
            }
        }
    });

    // search auto KhachHang
    var index = -1;
    var model_KH = new Vue({
        el: '#divSearchKH',
        data: function () {
            return {
                query_Kh: '',
                data_kh: []
            }
        },
        methods: {
            reset: function (item) {
                this.data_kh = [];
                this.query_Kh = '';
            },
            click: function (item) {
                self.Change_KhachHang(item.ID);
                $('#showseach_Kh').hide();
            },
            submit: function (event) {
                let keyCode = event.keyCode;
                switch (keyCode) {
                    case 13:
                        var result = this.fillter_KH(this.query_Kh);
                        var focus = false;
                        $('#showseach_Kh ul li').each(function (i) {
                            if ($(this).hasClass('focusLi0')) {
                                self.Change_KhachHang(result[i].ID);
                                $('#showseach_Kh').hide();
                                focus = true;
                            }
                        });
                        if (result.length > 0 && this.query_Kh !== '' && focus === false) {
                            $('#showseach_Kh').hide();
                        }
                        break;
                    case 37:
                    case 38:
                        index = index - 1;
                        if (index < 0) {
                            index = $("#showseach_Kh ul li").length - 1;
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top + 500
                            }, 1000);
                        }
                        else if (index > 0 && index < 10) {
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                    case 39:
                    case 40:
                        index = index + 1;
                        if (index >= ($("#showseach_Kh ul li").length)) {
                            index = 0;
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top - 600
                            }, 1000);
                        }
                        else if (index > 9 && index < $("#showseach_Kh ul li").length) {
                            $('#showseach_Kh').stop().animate({
                                scrollTop: $('#showseach_Kh').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                }
            },
            loadFocus: function () {
                $('#showseach_Kh ul li').each(function (i) {
                    $(this).removeClass('focusLi0');
                    if (index === i) {
                        $(this).addClass('focusLi0');
                    }
                });
            },
            // Tìm kiếm khách hàg
            fillter_KH: function (value) {
                if (value === '') return [];
                let txt = locdau(value);
                return self.DoiTuongs().filter(function (item) {
                    return item['Name_Phone'].indexOf(txt) > -1;
                }).slice(0, 20);
            },
        },
        computed: {
            // Return Khách hàng
            SearchKhachHang: function () {

                var result = this.fillter_KH(this.query_Kh);
                if (result.length < 1 || this.query_Kh === '') {
                    $('#showseach_Kh').hide();
                }
                else {
                    index = 0;
                    $('#showseach_Kh').show();
                }
                $('#showseach_Kh ul li').each(function (i) {
                    if (i === 0) {
                        $(this).addClass('focusLi0');
                    }
                    else {
                        $(this).removeClass('focusLi0');
                    }
                });
                $('#showseach_Kh').stop().animate({
                    scrollTop: $('#showseach_Kh').offset().top - 600
                }, 1000);
                return result;
            }

        }
    });

    // search auto KhachHang in popup PhieuThu
    var indexKH_PhieuThu = -1;
    var model_KHPT = new Vue({
        el: '#divSearchKHPhieuThu',
        data: function () {
            return {
                query_Kh: '',
                data_kh: [],
            }
        },
        methods: {
            reset: function (item) {
                this.data_kh = [];
                this.query_Kh = '';
            },
            click: function (item) {
                self.IDDoiTuong_PhieuThu(item.ID);
                $('#showseach_KhPT').hide();
            },
            submit: function (event) {
                let keyCode = event.keyCode;
                switch (keyCode) {
                    case 13:
                        var result = this.fillter_KH(this.query_Kh);
                        var focus = false;
                        $('#showseach_KhPT ul li').each(function (i) {
                            if ($(this).hasClass('focusLi0')) {
                                self.IDDoiTuong_PhieuThu(result[i].ID)
                                $('#showseach_KhPT').hide();
                                focus = true;
                            }
                        });
                        if (result.length > 0 && this.query_Kh !== '' && focus === false) {
                            self.IDDoiTuong_PhieuThu(result[0].ID);
                            $('#showseach_KhPT').hide();
                        }
                        break;
                    case 37:
                    case 38:
                        indexKH_PhieuThu = indexKH_PhieuThu - 1;
                        if (indexKH_PhieuThu < 0) {
                            indexKH_PhieuThu = $("#showseach_KhPT ul li").length - 1;
                            $('#showseach_KhPT').stop().animate({
                                scrollTop: $('#showseach_KhPT').offset().top + 500
                            }, 1000);
                        }
                        else if (indexKH_PhieuThu > 0 && indexKH_PhieuThu < 10) {
                            $('#showseach_KhPT').stop().animate({
                                scrollTop: $('#showseach_KhPT').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                    case 39:
                    case 40:
                        indexKH_PhieuThu = indexKH_PhieuThu + 1;
                        if (indexKH_PhieuThu >= ($("#showseach_KhPT ul li").length)) {
                            indexKH_PhieuThu = 0;
                            $('#showseach_KhPT').stop().animate({
                                scrollTop: $('#showseach_KhPT').offset().top - 600
                            }, 1000);
                        }
                        else if (indexKH_PhieuThu > 9 && indexKH_PhieuThu < $("#showseach_KhPT ul li").length) {
                            $('#showseach_KhPT').stop().animate({
                                scrollTop: $('#showseach_KhPT').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                }
            },
            loadFocus: function () {
                $('#showseach_KhPT ul li').each(function (i) {
                    $(this).removeClass('focusLi0');
                    if (indexKH_PhieuThu === i) {
                        $(this).addClass('focusLi0');
                    }
                });
            },
            // Tìm kiếm khách hàg
            fillter_KH: function (value) {
                if (value === '') return self.DoiTuongs().slice(0, 20);
                let txt = locdau(value);
                return self.DoiTuongs().filter(function (item) {
                    return item['Name_Phone'].indexOf(txt) > -1;
                }).slice(0, 20);
            },
        },
        computed: {
            // Return Khách hàng
            SearchKhachHangPhieuThu: function () {

                var result = this.fillter_KH(this.query_Kh);
                if (result.length < 1 || this.query_Kh === '') {
                    $('#showseach_KhPT').hide();
                }
                else {
                    indexKH_PhieuThu = 0;
                    $('#showseach_KhPT').show();
                }
                $('#showseach_KhPT ul li').each(function (i) {
                    if (i == 0) {
                        $(this).addClass('focusLi0');
                    }
                    else {
                        $(this).removeClass('focusLi0');
                    }
                });
                $('#showseach_KhPT').stop().animate({
                    scrollTop: $('#showseach_KhPT').offset().top - 600
                }, 1000);
                return result;
            }
        }
    });

    // search phong at chuyenghepban
    var index = -1;
    var model_Phong = new Vue({
        el: '#divSearchPB',
        data: function () {
            return {
                query: '',
                data: []
            }
        },
        methods: {
            reset: function (item) {
                this.data = [];
                this.query = '';
            },
            click: function (item) {
                self.ChuyenGhepBan_ChosePhong(item);
                $('#showseach_PB').hide();
            },
            submit: function (event) {
                let keyCode = event.keyCode;
                switch (keyCode) {
                    case 13:
                        var result = this.fillter(this.query);
                        var focus = false;
                        $('#showseach_PB ul li').each(function (i) {
                            if ($(this).hasClass('focusLi0')) {
                                self.ChuyenGhepBan_ChosePhong(result[i]);
                                $('#showseach_PB').hide();
                                focus = true;
                            }
                        });
                        if (result.length > 0 && this.query !== '' && focus === false) {
                            $('#showseach_PB').hide();
                        }
                        break;
                    case 37:
                    case 38:
                        index = index - 1;
                        if (index < 0) {
                            index = $("#showseach_PB ul li").length - 1;
                            $('#showseach_PB').stop().animate({
                                scrollTop: $('#showseach_PB').offset().top + 500
                            }, 1000);
                        }
                        else if (index > 0 && index < 10) {
                            $('#showseach_PB').stop().animate({
                                scrollTop: $('#showseach_PB').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                    case 39:
                    case 40:
                        index = index + 1;
                        if (index >= ($("#showseach_PB ul li").length)) {
                            index = 0;
                            $('#showseach_PB').stop().animate({
                                scrollTop: $('#showseach_PB').offset().top - 600
                            }, 1000);
                        }
                        else if (index > 9 && index < $("#showseach_PB ul li").length) {
                            $('#showseach_PB').stop().animate({
                                scrollTop: $('#showseach_PB').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                }
            },
            loadFocus: function () {
                $('#showseach_PB ul li').each(function (i) {
                    $(this).removeClass('focusLi0');
                    if (index === i) {
                        $(this).addClass('focusLi0');
                    }
                });
            },
            fillter: function (value) {
                if (value === '') return [];
                let txt = locdau(value);
                return self.PhongBans_ChuyenBan().filter(function (item) {
                    item.FullName = item.TenViTri.concat(' ', item.MaViTri);
                    return locdau(item['FullName']).indexOf(txt) > -1;
                }).slice(0, 20);
            },
        },
        computed: {
            SearchList: function () {
                var result = this.fillter(this.query);
                if (result.length < 1 || this.query === '') {
                    $('#showseach_PB').hide();
                }
                else {
                    index = 0;
                    $('#showseach_PB').show();
                }
                $('#showseach_PB ul li').each(function (i) {
                    if (i === 0) {
                        $(this).addClass('focusLi0');
                    }
                    else {
                        $(this).removeClass('focusLi0');
                    }
                });
                $('#showseach_PB').stop().animate({
                    scrollTop: $('#showseach_PB').offset().top - 600
                }, 1000);
                return result;
            }

        }
    });

    function getAllDMNhomHangHoas() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/DM_NhomHangHoaAPI/' + 'GetTree_NhomHangHoa', 'GET').done(function (obj) {
                if (obj.res === true) {
                    let data = obj.data;
                    if (data.length > 0) {
                        data = data.sort((a, b) => a.text.localeCompare(b.text, undefined, { caseFirst: "upper" }));
                    }
                    self.NhomHangHoas(data);
                    localStorage.setItem('lc_NhomHangHoasNH', JSON.stringify(data));

                    tree = $('#tree').tree({
                        primaryKey: 'id',
                        uiLibrary: 'bootstrap',
                        dataSource: data,
                        checkboxes: true,
                    });
                }
            });
        }
        else {
            var lc_NhomHangHoas = localStorage.getItem('lc_NhomHangHoasNH');
            if (lc_NhomHangHoas !== null) {
                lc_NhomHangHoas = JSON.parse(lc_NhomHangHoas);
                self.NhomHangHoas(lc_NhomHangHoas);
            }
        }
    }

    function GetNhanVien_NguoiDung() {
        if (navigator.onLine) {
            ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + 'GetNhanVien_NguoiDung', 'GET').done(function (x) {
                if (x.res === true) {
                    let data = x.data;
                    self.NhanViens_ChiNhanh(data);

                    var lstNV_byDonVi = $.grep(data, function (x) {
                        return x.ID_DonVi === _idDonVi;
                    })
                    self.NhanViens(lstNV_byDonVi);
                    newModal_AddKhachHang.NhanViens(lstNV_byDonVi);
                    newModal_AddKhachHang.NhanVienAllChiNhanh(data);

                    WriteData_Dexie(db.NS_NhanVien, data);

                    // because if get from DB: --> run after ResetNVien_BGia, so must set NhanVien0 at here

                    // get user from DB
                    ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + 'GetUserCookies', 'GET').done(function (nv_nd) {
                        _idNhanVien = nv_nd.ID_NhanVien;
                        _idUser = nv_nd.ID;
                        _userLogin = nv_nd.TaiKhoan;
                        WriteData_Dexie(db.HT_NguoiDung, [nv_nd]);
                    });
                }
            })
        }
        else {
            db.NS_NhanVien.toArray(function (dt) {
                self.NhanViens_ChiNhanh(dt);

                let lstNV_byDonVi = $.grep(self.NhanViens_ChiNhanh(), function (x) {
                    return x.ID_DonVi === _idDonVi;
                });
                self.NhanViens(lstNV_byDonVi);
                newModal_AddKhachHang.NhanViens(lstNV_byDonVi);
                newModal_AddKhachHang.NhanVienAllChiNhanh(dt);
            });

            db.HT_NguoiDung.toArray(function (dt) {
                _idNhanVien = dt[0].ID_NhanVien;
                _idUser = dt[0].ID;
                _userLogin = dt[0].TaiKhoan;
            });
        }
    }

    function GetDMGiaBan_GBApDung() {
        if (navigator.onLine) {
            ajaxHelper("/api/DanhMuc/DM_GiaBanAPI/" + "GetDMGiaBan_GBApDung_ChiTiet?idDonVi=" + _idDonVi, 'GET').done(function (obj) {
                if (obj.res === true) {
                    let data = obj.data;
                    self.AllBangGia(data);
                    BindLstBangGia_byNhanVien_andDoiTuong(data);
                    WriteData_Dexie(db.DM_GiaBan, data);
                }
            });
        }
        else {
            db.DM_GiaBan.toArray(function (dt) {
                self.AllBangGia(data);
            });
        }
    };

    function BindLstBangGia_byNhanVien_andDoiTuong() {
        // get BangGia if BGia apply for all NhanVien, allDoiTuong
        var arrBGia = [];
        for (let i = 0; i < self.AllBangGia().length; i++) {
            if (self.AllBangGia()[i].TatCaNhanVien && self.AllBangGia()[i].TatCaDoiTuong) {
                arrBGia.push(self.AllBangGia()[i]);
            }
        }

        // get BGia apply for NhanVien is chosing
        for (let i = 0; i < self.AllBangGia().length; i++) {
            let itFor = self.AllBangGia()[i];
            if (itFor.TatCaNhanVien === false || itFor.TatCaDoiTuong == false) {
                if (itFor.TatCaNhanVien === false) {
                    // apply for all DoiTuong
                    if (itFor.TatCaDoiTuong) {
                        for (let j = 0; j < itFor.DM_GiaBan_ApDung.length; j++) {
                            if (itFor.DM_GiaBan_ApDung[j].ID_NhanVien === self.selectedNVien()) {
                                arrBGia.push(itFor);
                                break; // esc for DM_GiaBan_ApDung
                            }
                        }
                    }
                    else {
                        // apply by ID_NhomDoiTuong
                        for (let j = 0; j < itFor.DM_GiaBan_ApDung.length; j++) {
                            if (itFor.DM_GiaBan_ApDung[j].ID_NhanVien === self.selectedNVien()
                                && self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0
                                && self.ChiTietDoiTuong()[0].ID_NhomDoiTuong.toLowerCase().indexOf(itFor.DM_GiaBan_ApDung[j].ID_NhomKhachHang) > -1) {
                                arrBGia.push(itFor);
                                break; // esc for DM_GiaBan_ApDung
                            }
                        }
                    }
                }
                else {

                    // chi con truong hop TatCaDoiTuong == false (OK)
                    if (itFor.TatCaDoiTuong == false) {
                        // apply by ID_NhomDoiTuong
                        for (let j = 0; j < itFor.DM_GiaBan_ApDung.length; j++) {
                            if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0
                                && self.ChiTietDoiTuong()[0].ID_NhomDoiTuong.toLowerCase().indexOf(itFor.DM_GiaBan_ApDung[j].ID_NhomKhachHang) > -1) {
                                arrBGia.push(itFor);
                                break; // esc for DM_GiaBan_ApDung
                            }
                        }
                    }
                }
            }
        }
        arrBGia = FillterBangGia_byNgayTrongTuan(arrBGia);
        self.GiaBans(arrBGia);
        var objBGChung = {
            TenGiaBan: 'Bảng giá chuẩn',
            ID: const_GuidEmpty,
        }
        self.GiaBans.unshift(objBGChung);
    }
    function FillterBangGia_byNgayTrongTuan(lstBangGia) {
        // filter BangGia by NgayLapHD
        var dtNgayLapHD = moment($('#txtNgayTaoHD').val(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD');
        var today = (new Date(dtNgayLapHD)).getDay() + 1; // 1 --> 7 (1: chu nhat)
        // check null NgayTrongTuan
        var arrBGNgayTrongTuan = [];
        for (let i = 0; i < lstBangGia.length; i++) {
            if (lstBangGia[i].NgayTrongTuan !== null && lstBangGia[i].NgayTrongTuan !== undefined) {
                // push BG co loai chung tu = loaiHD
                if (lstBangGia[i].NgayTrongTuan.indexOf(today.toString()) > -1) {
                    arrBGNgayTrongTuan.push(lstBangGia[i]);
                }
            }
        }
        for (let i = 0; i < lstBangGia.length; i++) {
            if (lstBangGia[i].NgayTrongTuan === null) {
                // push BG co loai NgayTrongTuan === null
                arrBGNgayTrongTuan.push(lstBangGia[i]);
            }
        }
        return arrBGNgayTrongTuan;
    }

    function LoadTrangThai() {
        if (navigator.onLine) {
            ajaxHelper(DMDoiTuongUri + 'GetListTrangThaiTimKiem', 'GET').done(function (obj) {
                var data = obj.data;
                if (obj.res === true && data !== undefined) {
                    newModal_AddKhachHang.TrangThaiKhachHang(data);
                    WriteData_Dexie(db.DM_DoiTuong_TrangThai, data);
                }
            });
        }
        else {
            db.DM_DoiTuong_TrangThai.toArray(function (dt) {
                newModal_AddKhachHang.TrangThaiKhachHang(dt);
            });
        }
    };

    function GetInforKhachHang_ByID(cusID) {
        var date = moment(new Date()).format('YYYY-MM-DD');
        if (navigator.onLine && cusID !== null) {
            ajaxHelper(DMDoiTuongUri + "GetInforKhachHang_ByID?idDoiTuong=" + cusID + '&idChiNhanh=' + _idDonVi
                + '&timeStart=' + date + '&timeEnd=' + date + '&wasChotSo=false', 'GET').done(function (data) {
                    if (data !== null) {
                        self.ChiTietDoiTuong(data);
                    }
                });
        }
    }

    function getAllDMDoiTuong() {
        if (navigator.onLine) {
            var _now = new Date();
            var dayEnd = moment(new Date(_now.setDate(_now.getDate() + 1))).format('YYYY-MM-DD');
            var Params_GetListKhachHang = {
                ID_DonVis: [_idDonVi],
                LoaiDoiTuong: 1,
                NgayTao_TuNgay: '2016-01-01',
                NgayTao_DenNgay: dayEnd,
                TongBan_TuNgay: '2016-01-01',
                TongBan_DenNgay: dayEnd,
                NguoiTao: _userLogin,
                ID_NhanVienQuanLys: null,
            }
            ajaxHelper(DMDoiTuongUri + "GetListKH_PassObject_ByNhanVien", 'POST', Params_GetListKhachHang).done(function (data) {
                if (data !== null) {
                    self.DoiTuongs(data);
                    newModal_AddKhachHang.ListAllDoiTuong(data);

                    if (self.ThietLap().BanHangOffline) {
                        WriteData_Dexie(db.DM_DoiTuong, data);
                    }
                }
            });
        }
        else {
            db.DM_DoiTuong.toArray(function (x) {
                self.DoiTuongs(x);
                newModal_AddKhachHang.ListAllDoiTuong(x);
            });
        }
    }

    function getChiTietNCCByID(id) {

        if (id !== const_GuidEmpty && id !== null) {
            // find in lstKH offline
            var customer = [];
            var cusOffline = localStorage.getItem(lcListKHoffline);
            if (cusOffline !== null) {
                cusOffline = JSON.parse(cusOffline);
                customer = $.grep(cusOffline, function (x) {
                    return x.ID === id;
                });
            }
            if (customer.length > 0) {
                self.ChiTietDoiTuong(customer);// KH offline
            }
            else {
                // find in lstDoiTuong
                if (self.DoiTuongs().length > 0) {
                    customer = $.grep(self.DoiTuongs(), function (x) {
                        return x.ID === id;
                    });
                    self.ChiTietDoiTuong(customer);// KH offline
                }
                else {
                    // get from db
                    GetInforKhachHang_ByID(id);
                }
            }
            self.selectedNCC(id);
        }
        else {
            self.selectedNCC(null);
            self.ChiTietDoiTuong([]);
        }
    }

    self.selectGroupkhuVuc = ko.observable();
    function refreshGroupkhuVuc(id) {
        self.modelViTri('');
        self.selectGroupkhuVuc(id);
    }
    self.getPhongBan_ByIDNhom = function (item) {
        $('#homePB').addClass('active');
        $('#homeFoods').removeClass('active');
        $('#infor').removeClass('active');
        $('#home').addClass('active');
        $('#seachHangHoa').hide();
        $('#searchPhongBan').show();
        self.currentPage(0);

        var idKhuVuc = item.ID;
        self.selectGroupkhuVuc(idKhuVuc);
        if (idKhuVuc == undefined) {
            self.PhongBans(self.AllPhongBans());
        }
        else {
            var newArr = $.grep(self.AllPhongBans(), function (x) {
                return x.ID_KhuVuc === idKhuVuc;
            });
            self.PhongBans(newArr);
        }
        GetAllHD_fromDB();
    }

    function ChangeColorImg() {
        // reset all img before set color
        $('img[id^=img_]').attr('src', '/Content/images/iconbepp18.9/banghe.png');
        if (_phongBanID !== null) {
            $('#img_' + _phongBanID).attr('src', '/Content/images/table_red.png');
        }
        else {
            var lcPhongBan = localStorage.getItem('lc_PhongBans');
            if (lcPhongBan !== null && lcPhongBan !== 'null') {
                lcPhongBan = JSON.parse(lcPhongBan);
                var obj = $('#img_' + lcPhongBan[0].ID);
                obj.attr('src', '/Content/images/table_red.png');
                localStorage.setItem(lcPhongBanID, lcPhongBan[0].ID);
            }
        }
    }

    function UpdateTime_lcPhongBan() {
        var lc_PhongBans = self.PhongBans();
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            // get arrID_ViTri in lstHD
            var arrID_ViTri = $.unique($.map(lstHD, function (x) {
                return x.ID_ViTri;
            }).sort());

            for (let i = 0; i < lc_PhongBans.length; i++) {
                lc_PhongBans[i].CreateTime = '0';
                lc_PhongBans[i].WorkingTime = 0;
                lc_PhongBans[i].SoLuongKhachHang = 0;

                if ($.inArray(lc_PhongBans[i].ID, arrID_ViTri) > -1) {

                    // get all HD of PhongBan
                    let arrHD_PB = $.grep(lstHD, function (item) {
                        return item.ID_ViTri === lc_PhongBans[i].ID && item.StatusOffline === false && item.ID_DonVi === _idDonVi;
                    });

                    // sort arrHD_PB with NgayLapHoaDon DESC
                    arrHD_PB.sort(function (a, b) {
                        var date1 = (new Date(a.NgayLapHoaDon));
                        var date2 = (new Date(b.NgayLapHoaDon));
                        return date1 - date2;
                    });

                    // sum soluongkhach
                    let sum = 0;
                    for (let j = 0; j < arrHD_PB.length; j++) {
                        sum += arrHD_PB[j].SoLuongKhachHang;
                    }

                    // get NgayLapHoaDon min
                    if (arrHD_PB.length > 0) {
                        let ngayLapHDmin = arrHD_PB[0].NgayLapHoaDon;

                        // get item HD with ID_Vitri and NgayLapHoaDon min
                        let hdFirst_OfPhong = $.grep(lstHD, function (item2) {
                            return item2.ID_ViTri === lc_PhongBans[i].ID && item2.NgayLapHoaDon === ngayLapHDmin;
                        });

                        // set time for PhongBan
                        if (hdFirst_OfPhong.length > 0) {
                            lc_PhongBans[i].CreateTime = hdFirst_OfPhong[0].CreateTime;
                            lc_PhongBans[i].WorkingTime = getMinutesBetweenDates(ngayLapHDmin);
                        }
                        lc_PhongBans[i].SoLuongKhachHang = sum;
                    }
                }
            }
        }
        else {
            for (let i = 0; i < lc_PhongBans.length; i++) {
                lc_PhongBans[i].CreateTime = '0';
                lc_PhongBans[i].WorkingTime = 0;
                lc_PhongBans[i].SoLuongKhachHang = 0;
            }
        }
        // sort by name
        lc_PhongBans = lc_PhongBans.sort((a, b) => a.TenViTri.localeCompare(b.TenViTri, undefined, { caseFirst: "upper" }));
        self.PhongBans(lc_PhongBans);
        self.PhongBans.refresh();
    }

    function getMinutesBetweenDates(startDate) {

        var start = startDate;

        var diff = (new Date() - new Date(start)) / 1000;

        var hours = Math.floor(diff / 3600);
        var minutes = Math.floor(diff % 3600) / 60;

        if (hours > 24) {
            // lay phan nguyen Math.floor
            return Math.floor(hours / 24) + ' ngày trước'
        }
        else {
            if (hours >= 1) {
                return hours + ' giờ trước';
            }
            else {
                if (minutes > 1) {
                    return Math.floor(minutes) + ' phút trước';
                }
                else {
                    return 'vài giây trước';
                }
            }
        }
    }

    self.showHoaDonOffline = function () {
        $('.bs-example-modal-lg').modal('show');
        $('#btnDongBoHoa').removeAttr('disabled');

        if (navigator.onLine) {
            $('#lblStatusConnect').text("Có internet");
            $('.connect').css('color', 'blue');
            $(".connect i").css('color', 'blue');
            $('#btnDongBoHoa').css('display', '');
        }
        else {
            $('#lblStatusConnect').text("Không có internet");
            $(".connect i").css({ "right": "68px", 'color': 'red' })
            $('.connect').css('color', 'red');
            $('#btnDongBoHoa').css('display', 'none');
        }
    }

    function GetAll_IDNhomChild_ofNhomHH(idNhom) {
        var allID = [];
        if (idNhom !== null) {
            tree.check(tree.getNodeById(idNhom));//check to do get all child
            allID = tree.getCheckedNodes();
        }
        return allID;
    }

    self.getHangHoa_ByIDNhom = function () {
        var id = this.id;
        self.selectedNhomHH(id);

        // active tab THUC DON, remove active tab PHONG BAN
        $('#homePB').removeClass('active');
        $('#homeFoods').addClass('active');
        $('#infor').addClass('active');
        $('#home').removeClass('active');
        $('#seachHangHoa').show();
        $('#searchPhongBan').hide();

        var itemHD = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);
        if (itemHD.length > 0) {
            var idBangGia = itemHD[0].ID_BangGia;
            if (idBangGia !== self.selectedGiaBan()) {
                UpdateGiaBan_inListHangHoa_byPage(idBangGia);
            }
            self.selectedGiaBan(idBangGia);
        }

        if (id == undefined) {
            self.HangHoas(self.HangHoaAll());
        }
        else {
            // get from Cache
            self.currentPageProducts(0);
            tree.uncheckAll();

            var arrIDChilds = GetAll_IDNhomChild_ofNhomHH(id);
            var newArr = $.grep(self.HangHoaAll(), function (item) {
                return $.inArray(item.ID_NhomHangHoa, arrIDChilds) > -1;
            });
            self.HangHoas(newArr);
        }
        BindListHangHoa_ByPage();
    }

    self.showPopupAddKH = function () {
        var roleCus = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === 'KhachHang_ThemMoi';
        })
        if (roleCus.length === 0) {
            ShowMessage_Danger('Bạn không có quyền thêm mới khách hàng');
            return false;
        }

        $('#modalPopuplg_KH').modal('show');
        $('#lblTitleKH').text('Thêm khách hàng');
        $('#btnLuuNhomDoiTuong').show();
        $('#file').val('');

        newModal_AddKhachHang.newDoiTuong(new FormModel_NewKhachHang());
        newModal_AddKhachHang.newNhomDoiTuong(new FormModel_NewNhomDoiTuong());
        newModal_AddKhachHang.newDoiTuong().ID_NhanVienPhuTrach(_idNhanVien);
        newModal_AddKhachHang.booleanAdd(true);
        newModal_AddKhachHang.DoAction(0);
        newModal_AddKhachHang.HaveImage_Select(false);
        newModal_AddKhachHang.FilesSelect([]);

        if (self.ThietLap().QuanLyKhachHangTheoDonVi) {
            let nhomNotDefault = $.grep(newModal_AddKhachHang.NhomDoiTuongDB(), function (x) {
                return x.ID !== 0;
            });
            if (nhomNotDefault.length > 0) {
                let nhomofCN = $.grep(nhomNotDefault, function (x) {
                    return x.NhomDT_DonVi.length === 1 && x.NhomDT_DonVi[0].ID === idDonVi;
                });
                if (nhomofCN.length === 1) {
                    newModal_AddKhachHang.NhomDoiTuongChosed([nhomofCN[0]]);
                    $('#choose_NhomDT input').remove();
                    SetCheckAfter_inArrID('ddlNhomDT', [nhomofCN[0].ID]);
                }
                else {
                    Reset_NhomDoiTuong();
                }
            }
            else {
                Reset_NhomDoiTuong();
            }
        }
        else {
            Reset_NhomDoiTuong();
        }
        Reset_NguonKhach();
    }

    function Reset_NhomDoiTuong() {
        newModal_AddKhachHang.NhomDoiTuongChosed([]);
        ResetDropdown('#choose_NhomDT', '#ddlNhomDT', 'Chọn nhóm');
    }
    function Reset_NguonKhach() {
        newModal_AddKhachHang.NguonKhachChosed([]);
        ResetDropdown('#choose_NguonKhach', '#ddlNguonKhach', 'Chọn nguồn');
    }

    function AddNewKH_indexDB(DM_DoiTuong) {
        db.DM_DoiTuong.put(DM_DoiTuong)
    }

    self.showPopupEditKH = function (item) {
        var role_UpdateKH = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === 'KhachHang_CapNhat';
        });

        if (role_UpdateKH.length > 0) {
            $('#modalPopuplg_KH').modal('show');
            $('#lblTitleKH').text('Cập nhật khách hàng');

            if (item.ID_NhanVienPhuTrach === null || item.ID_NhanVienPhuTrach === const_GuidEmpty) {
                item.ID_NhanVienPhuTrach = idNhanVien;
            }
            newModal_AddKhachHang.newDoiTuong().SetData(item);
            newModal_AddKhachHang.booleanAdd(false);
            newModal_AddKhachHang.DoAction(0);

            // ca nhan/ cong ty
            if (item.LaCaNhan) {
                $(".check-personal").show();
                $(".cheack-conpany").removeClass("tgg")
            }
            else {
                $(".check-personal").hide();
                $(".cheack-conpany").addClass("tgg");
            }

            // bind list Nhom Khachhang
            var arrID_Nhom = [];
            if (item.ID_NhomDoiTuong !== null) {
                arrID_Nhom = item.ID_NhomDoiTuong.split(',');
                arrID_Nhom = $.grep(arrID_Nhom, function (x) {
                    return x.length >= 36;
                });
            }
            var arrTenNhom = item.TenNhomDT.split(',');
            newModal_AddKhachHang.NhomDoiTuongChosed([]);
            for (var i = 0; i < arrID_Nhom.length; i++) {
                if (arrID_Nhom[i] !== '00000000-0000-0000-0000-000000000000') {
                    var objNhom = {
                        ID: arrID_Nhom[i].trim().toLowerCase(),
                        TenNhomDoiTuong: arrTenNhom[i].trim(),
                    };
                    newModal_AddKhachHang.selectManyNhomDT(objNhom);
                }
            }
            if (newModal_AddKhachHang.NhomDoiTuongChosed().length === 0) {
                Reset_NhomDoiTuong();
            }

            // Bind NguonKhach
            var itemNguon = [];
            if (item.ID_NguonKhach !== null) {
                itemNguon = $.grep(newModal_AddKhachHang.NguonKhachs(), function (x) {
                    return x.ID === item.ID_NguonKhach;
                });
            }
            if (itemNguon.length > 0) {
                newModal_AddKhachHang.ChoseNguonKhach(itemNguon);
            }
            else {
                Reset_NguonKhach();
            }

            // img -- todo get img customer at lstDoiTuong
            newModal_AddKhachHang.HaveImage_Select(false);
            if (newModal_AddKhachHang.HaveImage_Select() === false) {
                newModal_AddKhachHang.FilesSelect([]);
                $('#file').val('');
            }
            else {
                newModal_AddKhachHang.FilesSelect(self.FileImgs());
            }
        }
        else {
            ShowMessage_Danger('Không có quyền cập nhật khách hàng');
        }
    }

    // modal insert customer/vendor
    $('#modalPopuplg_KH, #modalPopuplg_NCC').on('hidden.bs.modal', function () {
        if (newModal_AddKhachHang.DoAction() == 1) {
            // insert customer
            var item = newModal_AddKhachHang.CustomerDoing();
            if (item !== []) {
                switch (newModal_AddKhachHang.booleanAdd()) {
                    case true:// insert
                        if (navigator.onLine) {
                            Insert_NhatKyThaoTac(item, [], 2, 1);
                        }
                        else {
                            AddNewKH_indexDB(item);
                        }
                        self.DoiTuongs.unshift(item);
                        self.Change_KhachHang(item.ID);
                        break;
                    case false://update
                        for (var i = 0; i < self.DoiTuongs().length; i++) {
                            if (self.DoiTuongs()[i].ID === item.ID) {
                                self.DoiTuongs.remove(self.DoiTuongs()[i]);
                                break;
                            }
                        }
                        self.DoiTuongs.unshift(item);
                        getChiTietNCCByID(item.ID);

                        if (navigator.onLine) {
                            Insert_NhatKyThaoTac(item, [], 2, 2);
                        }
                        else {
                            AddNewKH_indexDB(item);
                        }
                        break;
                }
            }
        }
    });

    // insert customer group
    $('#modalAddGroup, #modalNhomNCC').on('hidden.bs.modal', function () {
        // get nhomdt chi tiet
        var nhom = newModal_AddKhachHang.GroupDoing();
        switch (newModal_AddKhachHang.DoAction()) {
            case 2:  // insert/update
                // remove avoid duplicate (because same variable NhomDoiTuongs at KhachHang.js & modalThemMoiKH.js)
                for (var i = 0; i < self.NhomDoiTuongDB().length; i++) {
                    if (self.NhomDoiTuongDB()[i].ID === nhom.ID) {
                        self.NhomDoiTuongDB.remove(self.NhomDoiTuongDB()[i]);
                        break;
                    }
                }
                self.NhomDoiTuongDB.unshift(nhom);
                if (newModal_AddKhachHang.booleanAddNhomDT()) {
                    if (newModal_AddKhachHang.IsAddAtModal() === false) {
                        self.selectNhomDT(nhom.ID);
                    }
                }
                GetDM_NhomDoiTuong_ChiTiets();
                break;
            case 5: // delete
                for (var i = 0; i < self.NhomDoiTuongDB().length; i++) {
                    if (self.NhomDoiTuongDB()[i].ID === nhom.ID) {
                        self.NhomDoiTuongDB.remove(self.NhomDoiTuongDB()[i]);
                        break;
                    }
                };
                Insert_NhatKyThaoTac(nhom, 2, 3);
                SearchKhachHang();
                break;
        }
        newModal_AddKhachHang.DoAction(0);
    })

    function AddKHOffline_IndexDB(DM_DoiTuong) {
        db.DM_DoiTuong.put(DM_DoiTuong)
    }

    self.deleteNCC = function (item) {

        self.ChiTietDoiTuong.remove(item);
        self.selectedNCC(undefined);
        ResetTextSearch();
        // update ID_DoiTuong for lstHD
        var idRandomHD = '';
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].ID_ViTri === _phongBanID && lstHD[i].MaHoaDon === _maHoaDon) {
                    lstHD[i].ID_DoiTuong = null;
                    idRandomHD = lstHD[i].IDRandom;
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));

            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
        }
    };

    self.showNoteVitri = function () {
        $(".text_area").css("display", "none");
        $('#text_area' + this.ID).show();
    }

    function SaveHD_RemoveDisable() {
        $('#btnSave, input, select').removeAttr('disabled');
        $('.bgwhite').hide();
        $('#wait').remove();
        $('.bs-example-modal-lg').modal('hide');
    }

    function AssignProperties_forCTHD_whenSave(arr) {
        for (let i = 0; i < arr.length; i++) {
            delete arr[i]["DM_LoHang"];
            if (arr[i].GiaBan > arr[i].DonGia) {
                arr[i].DonGia = arr[i].GiaBan;
            }
            arr[i].ThoiGianThucHien = null;
            if (arr[i].DichVuTheoGio === 1) {
                arr[i].ThoiGianThucHien = arr[i].SoLuong * 60;
            }
        }
        return arr;
    }

    self.saveHoaDon = function () {
        $('.bgwhite').show();
        hidewait('footer-bill');
        $('#btnSave').attr('disabled', 'disabled');

        var idViTri = _phongBanID;
        var _idNVien = self.selectedNVien();

        if (_maHoaDon === '') {
            _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
        }

        var tenDTuong = 'Khách lẻ';

        // tim HoaDon co maHoaDon tuong ung
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            var hdOpening = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);

            if (hdOpening.length === 0) {
                SaveHD_RemoveDisable();
                ShowMessage_Danger('Vui lòng nhập thông tin hóa đơn');
                return false;
            }

            var itemNV = $.grep(self.NhanViens(), function (item) {
                return item.ID === hdOpening[0].ID_NhanVien;
            });
            if (itemNV.length === 0) {
                ShowMessage_Danger('Vui lòng chọn nhân viên lập hóa đơn');
                SaveHD_RemoveDisable()
            }

            // ngaylap HD
            var idRandomHD = hdOpening[0].IDRandom;
            var ngaylapHD = GetNgayLapHD_whenSave(hdOpening[0].NgayLapHoaDon);
            hdOpening[0].NgayLapHoaDon = ngaylapHD;
            hdOpening[0].GioRa = convertDateTime(new Date());

            // check NgayLapHD
            var dateDDMMYYY = moment(hdOpening[0].NgayLapHoaDon, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm');
            var check = CheckNgayLapHD(dateDDMMYYY);
            if (!check) {
                SaveHD_RemoveDisable();
                return false;
            }

            // tim CTHoaDon t/u voi HoaDon
            var objCTAdd = [];
            var cthd = localStorage.getItem(lcListCTHD);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                objCTAdd = $.grep(cthd, function (item) {
                    return item.IDRandomHD === idRandomHD;
                });
            }
            else {
                cthd = [];
            }

            if (objCTAdd.length === 0) {
                ShowMessage_Danger('Vui lòng nhập chi tiết hóa đơn');
                SaveHD_RemoveDisable();
                return;
            }

            // check if SoLuong > TonKho
            if (self.ThietLap().XuatAm === false) {
                var msgErr = "";
                for (let i = 0; i < objCTAdd.length; i++) {
                    if (objCTAdd[i].LaHangHoa && objCTAdd[i].SoLuong > objCTAdd[i].TonKho) {
                        msgErr += objCTAdd[i].TenHangHoa + ", ";
                    }
                }

                if (msgErr !== "") {
                    msgErr = msgErr.substr(0, msgErr.length - 2);
                    ShowMessage_Danger('Không đủ số lượng tồn kho cho sản phẩm ' + msgErr);
                    SaveHD_RemoveDisable();
                    return false;
                }
            }

            // Check Giam gia > tien hang
            if (hdOpening[0].TongGiamGiaKM_HD > hdOpening[0].TongTienHang) {
                ShowMessage_Danger('Giảm giá hóa đơn không được lớn hơn tổng tiền hàng');
                SaveHD_RemoveDisable();
                return false;
            }

            objCTAdd = AssignProperties_forCTHD_whenSave(objCTAdd);

            // assign again loaihoadon before update inforKH
            hdOpening[0].ChoThanhToan = false;
            hdOpening[0].LoaiHoaDon = 1;

            UpdateInforKH_inLstDoiTuongs(hdOpening[0], false);
            NangNhom_KhachHang(hdOpening[0], 2);

            if (navigator.onLine) {
                var myData = {};
                // Neu la HoaDonoffline => giu nguyen ma
                if (hdOpening[0].MaHoaDon.indexOf('HDO') > -1 || hdOpening[0].MaHoaDon.indexOf('DHO') > -1) {
                    hdOpening[0].MaHoaDon = hdOpening[0].MaHoaDon;
                }
                else {
                    hdOpening[0].MaHoaDon = null; // gan ma hoa don = null de khi save sinh MaHoaDon tu dong
                }

                // update all CTHD: Bep_SoLuongYeuCau = 0, Bep_SoLuongChoCungUng =0;
                // vi van cho phep chua goi mon xong ma van ThanhToan
                for (let i = 0; i < objCTAdd.length; i++) {
                    objCTAdd[i].Bep_SoLuongYeuCau = 0;
                    objCTAdd[i].Bep_SoLuongChoCungUng = 0;
                }

                myData.objHoaDon = hdOpening[0];
                myData.objCTHoaDon = objCTAdd;

                var itemDT = [];
                var dtOffline = localStorage.getItem(lcListKHoffline);
                if (dtOffline !== null) {
                    var doituongOfline = JSON.parse(dtOffline);
                    itemDT = $.grep(doituongOfline, function (item) {
                        return item.ID === hdOpening[0].ID_DoiTuong;
                    });
                }

                if (itemDT.length > 0) {
                    AddKhachHang_HoaDon(itemDT[0], myData, false);
                }
                else {
                    PostHD_SoQuy_SaveHD(myData);
                }
            }
            else {
                SaveHD_RemoveDisable();
                SaveHDoffline();
                ShowMessage_Success('Không có kết nối Internet. Giao dịch đã được lưu tạm trên máy');

                // start print offline
                hdOpening[0].MaHoaDon = _maHoaDon;
                var objHDPrint = GetInforHDPrint(hdOpening[0]);
                self.InforHDprintf(objHDPrint);
                var cthdPrint = GetCTHDPrint_Format(objCTAdd);
                self.CTHoaDonPrint(cthdPrint);
                self.InHoaDon('HDBL', false);
                BindHD_CTHD();
                $('.parent-price-1, .price-pay-end, input').removeClass('btn-disable');
            }
        }
        else {
            SaveHD_RemoveDisable();
            ShowMessage_Danger('Vui lòng nhập thông tin hóa đơn');
            return;
        }
    }

    $('.parent-price-1, .price-pay-end, input').removeClass('btn-disable');

    function SaveHDoffline() {

        var idRandom = self.HoaDons().IDRandom();

        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            var itemHD = $.grep(lstHD, function (x) {
                return x.IDRandom === idRandom;
            });

            var maHDon = '';
            if (itemHD.length > 0) {
                if (itemHD[0].StatusOffline === false) {

                    var rd = Math.floor(Math.random() * 10000000 + 1);
                    switch (itemHD[0].LoaiHoaDon) {
                        case 1:
                        case 0:
                            maHDon = 'HDO' + rd;
                            break;
                        case 3:
                            maHDon = 'DHO' + rd;
                    }

                    for (let i = 0; i < lstHD.length; i++) {
                        if (lstHD[i].IDRandom === idRandom) {
                            lstHD[i].GioRa = convertDateTime(new Date());
                            lstHD[i].MaHoaDon = maHDon;
                            lstHD[i].StatusOffline = true;
                            lstHD[i].StatusTBNB = true;
                            lstHD[i].TrangThaiHD = 2;
                            break;
                        }
                    }
                    localStorage.setItem(lcListHD, JSON.stringify(lstHD));

                    var cthd = localStorage.getItem(lcListCTHD);
                    if (cthd !== null) {
                        cthd = JSON.parse(cthd);
                        // change  MaHoaDon = HDO in lstCTHD
                        for (let i = 0; i < cthd.length; i++) {
                            if (cthd[i].IDRandomHD === idRandom) {
                                cthd[i].MaHoaDon = maHDon;
                            }
                        }
                        localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                        UpdateTonKho_forListHangHoa_andCTHD(cthd);
                    }

                    _maHoaDon = maHDon;
                    // use this func to update Diem (# BanLe) --> tru diem khi thanh toan = diem offline
                    UpdateDiemGiaoDich_forHoaDon(idRandom);
                }
                else {
                    // if save again HDOffline
                    maHDon = _maHoaDon;
                }

                // assign again _maHoaDon --> to PrintHoaDon
                _maHoaDon = maHDon;

                // count element offline
                Bind_andCountHDO();
            }
        }
    }

    // update SoLanMua, TongBanTruTraHang, TonBan, NoHienTai, TongTichDiem for KH
    function UpdateInforKH_inLstDoiTuongs(itemHD, isTrahet) {
        console.log('UpdateInforKH ', itemHD);
        var idDoiTuong = itemHD.ID_DoiTuong;
        var itemDTOld = $.grep(self.DoiTuongs(), function (x) {
            return x.ID === idDoiTuong;
        });

        if (itemDTOld.length > 0) {

            var noHienTai = 0;
            var soLanMua = 1;
            var diemHienTai = 0;
            var updateDiem = false;
            var tongTraHang = itemHD.TongGiaGocHangTra - itemHD.TongGiamGiaDB;
            // vi khong co chuc nang tra hang --> khong co thuoc tinh TongGiaGocHangTra, TongGiamGiaDB
            if (isNaN(tongTraHang)) {
                tongTraHang = 0;
            }
            var tongMua = itemHD.TongTienHang - itemHD.TongGiamGiaKM_HD;

            if (itemHD.PhaiTraKhach == 0) {
                noHienTai = itemHD.PhaiThanhToan - itemHD.DaThanhToan;
                noHienTai = noHienTai > 0 ? noHienTai : 0;
            }
            if (isTrahet) {
                soLanMua = -1; // neu tra het hang, tru 1 lan mua hang
            }

            var diemGDCurrent = formatNumberToFloat(itemHD.DiemGiaoDich);
            switch (itemHD.LoaiHoaDon) {
                case 1:
                    diemHienTai = itemDTOld[0].TongTichDiem + diemGDCurrent - itemHD.DiemQuyDoi;
                    // Neu HD khong dc tich diem (vi cai dat: khong TichDiem cho HD giam gia/HD thanh toan = diem), nhung van TT = diem --> tru diem KH
                    if ((diemGDCurrent !== 0) || (diemGDCurrent === 0 && itemHD.DiemQuyDoi > 0)) {
                        updateDiem = true;
                    }
                    break;
                case 6:
                    // if TraHang from HD tich diem --> get DiemGD of TraHang--> tru diem for KH
                    var diemGD = 0;
                    if (itemHD.FromHDTichDiem) {
                        diemGD = GetDiemGiaoDich_HDTraHang(tongTraHang);
                        diemHienTai = itemDTOld[0].TongTichDiem + diemGDCurrent - diemGD; // diem cu of KH + diem GD - diem tra hang
                        updateDiem = true;
                    }
                    else {
                        // khong tra hang tu HD tich diem, nhung co mua moi --> cong diem cho KH
                        if (tongMua > 0) {
                            diemHienTai = diemGDCurrent;
                            updateDiem = true;
                        }
                        else {
                            // khong mua moi
                            diemHienTai = 0; // assign = 0 --> avoid update when UpdateDiemKH_toDB (online)
                        }
                    }
                    break;
            }

            for (let i = 0; i < self.DoiTuongs().length; i++) {
                if (self.DoiTuongs()[i].ID === idDoiTuong) {
                    if (noHienTai > 0) {
                        self.DoiTuongs()[i].NoHienTai = self.DoiTuongs()[i].NoHienTai + noHienTai;
                    }
                    self.DoiTuongs()[i].SoLanMuaHang = self.DoiTuongs()[i].SoLanMuaHang + soLanMua;
                    self.DoiTuongs()[i].TongBan = self.DoiTuongs()[i].TongBan + tongMua;
                    self.DoiTuongs()[i].TongBanTruTraHang = self.DoiTuongs()[i].TongBanTruTraHang + tongMua - tongTraHang;
                    if (diemHienTai >= 0 && updateDiem) {
                        self.DoiTuongs()[i].TongTichDiem = diemHienTai;
                    }
                    break;
                }
            }
        }
    }

    self.goDetail = function () {

        var maHoaDon = this.MaHoaDon;
        var idViTri = this.ID_ViTri;
        var idRandom = this.IDRandom;
        _maHoaDon = maHoaDon;

        _maHoaDonOffline = maHoaDon;
        _phongBanID = idViTri;
        localStorage.setItem(lcPhongBanID, idViTri);
        $('.bs-example-modal-lg').modal('hide');

        // get ThongtinHoaDon
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            var itemExist = $.grep(lstHD, function (item) {
                return item.IDRandom === idRandom;
            });
            if (itemExist.length > 0) {
                // update status = 2
                for (let i = 0; i < lstHD.length; i++) {
                    if (lstHD[i].IDRandom === idRandom) {
                        lstHD[i].TrangThaiHD = 1;
                        break;
                    }
                }
                localStorage.setItem(lcListHD, JSON.stringify(lstHD));

                // load thong tin HoaDon
                getChiTietNCCByID(itemExist[0].ID_DoiTuong);
                self.SetNhanVien(itemExist[0].ID_NhanVien);
                BindLstBangGia_byNhanVien_andDoiTuong();
                if (self.selectedGiaBan() !== itemExist[0].ID_BangGia) {
                    UpdateGiaBan_inListHangHoa_byPage(itemExist[0].ID_BangGia);
                }
                self.SetBangGia(itemExist[0].ID_BangGia);
                self.HoaDons().SetData(itemExist[0]);

                // get arrMaHoaDon;
                //var arrMaHoaDon = [];
                //for (let i = 0; i < self.PhongBanSelected().length; i++) {
                //    arrMaHoaDon.push(self.PhongBanSelected()[i].MaHoaDon);
                //}

                // push if HoaDon isn't opening
                //if ($.inArray(maHoaDon, arrMaHoaDon) < 0) {
                self.PhongBanSelected.push(itemExist[0]);
                //}

                BindCTHD_byIDRandomHD(idRandom);

                // visible textbox, button
                $('.inner-setup, .parent-price-1, .price-pay-end').addClass('btn-disable');
                $('input,select').attr('disabled', 'disabled');
                $('#ddlDMChungTus').removeAttr('disabled');
                Disable_BtnThongBaoNB();
                Disable_BtnGhepBan();

                StyDSHoaDon_byMaHoaDon(maHoaDon);
            }
        }
    }

    self.selectLoaiChungTu.subscribe(function (newVal) {
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            var lstHDoffline = [];
            if (newVal === 0) {
                lstHDoffline = $.grep(lstHD, function (item) {
                    return item.StatusOffline === true && item.ID_User === _idUser && item.ID_DonVi === _idDonVi;
                });
            }
            else {
                lstHDoffline = $.grep(lstHD, function (item) {
                    return item.StatusOffline === true && item.LoaiHoaDon === newVal && item.ID_User === _idUser
                        && item.ID_DonVi === _idDonVi;
                });
            }
            self.HoaDonOffline(lstHDoffline);
        }
    })

    self.DongBoHoa = function () {
        $('#btnDongBoHoa,#btnSave').attr('disabled', 'disabled');

        // getListHoadOn_offline
        var lstHD = localStorage.getItem(lcListHD);
        var lstCTHD = localStorage.getItem(lcListCTHD);
        var dtOffline = localStorage.getItem(lcListKHoffline);
        if (dtOffline !== null) {
            dtOffline = JSON.parse(dtOffline);
        }
        else {
            dtOffline = [];
        }

        var lst_HDoffline = [];
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            // lst HD offline + Dathang offline
            lst_HDoffline = $.grep(lstHD, function (item) {
                return item.StatusOffline === true && item.ID_DonVi == _idDonVi && item.ID_User === _idUser;
            });

            var lstHDO = $.grep(lst_HDoffline, function (item) {
                return item.LoaiHoaDon === 1;
            });

            var lstDHO = $.grep(lst_HDoffline, function (item) {
                return item.LoaiHoaDon === 3;
            });

            var arrID_HoaDon = [];
            var maHoaDon = '';
            var idViTri = '';

            if (lstCTHD !== null) {
                lstCTHD = JSON.parse(lstCTHD);

                // DongBoHoa --> giu nguyen maHD khi offline + assign agin ChoThanhToan = false
                var loaiChungTu = self.selectLoaiChungTu();
                switch (loaiChungTu) {
                    case 0:// all

                        for (let i = 0; i < lst_HDoffline.length; i++) {

                            maHoaDon = lst_HDoffline[i].MaHoaDon;

                            if (lst_HDoffline[i].ID.indexOf('0000') === -1) {
                                arrID_HoaDon.push(lst_HDoffline[i].ID); //--> send to client
                            }

                            // check exist bep truoc khi reset SoLuong_YC,...
                            var arrExistBep = $.grep(lstCTHD, function (x) {
                                return $.inArray(x.ID_HoaDon, arrID_HoaDon) > -1 && (x.Bep_SoLuongYeuCau > 0 || x.Bep_SoLuongChoCungUng > 0);
                            });

                            // reset Bep_SoLuongYeuCau neu LoaiHoaDon = 1
                            if (lst_HDoffline[i].LoaiHoaDon === 1) {
                                for (let j = 0; j < lstCTHD.length; j++) {
                                    if (lstCTHD[j].MaHoaDon === maHoaDon) {
                                        lstCTHD[j].Bep_SoLuongYeuCau = 0;
                                        lstCTHD[j].Bep_SoLuongChoCungUng = 0;
                                    }
                                }
                            }

                            var arrHangHoa = $.grep(lstCTHD, function (x) {
                                return x.MaHoaDon === maHoaDon;
                            })

                            if (arrHangHoa.length > 0) {

                                var myData = {};
                                // ngaylap HD
                                var ngaylapHD = GetNgayLapHD_whenSave(lst_HDoffline[i].NgayLapHoaDon);
                                lst_HDoffline[i].NgayLapHoaDon = ngaylapHD;
                                lst_HDoffline[i].ChoThanhToan = false;

                                myData.objHoaDon = lst_HDoffline[i];
                                myData.objCTHoaDon = arrHangHoa;

                                var idDTuongHD = myData.objHoaDon.ID_DoiTuong;
                                let itemDT = $.grep(dtOffline, function (x) {
                                    return x.ID === idDTuongHD;
                                });

                                if (arrID_HoaDon.length > 0) {
                                    $('#wait').remove();
                                    // send to client
                                    var objSend = {
                                        ID_HoaDons: arrID_HoaDon,
                                        IDRandomHD: '',
                                        ID_User: _idUser,
                                        ID_ViTri: _phongBanID,
                                        IDChiNhanh: _idDonVi,
                                        Func: 4
                                    }
                                    SendData_atThisPage(objSend);

                                    if (arrExistBep.length > 0 && connected) {
                                        hub.server.sendData_ThuNgan_ToNhaBep(objSend);
                                    }
                                }

                                if (itemDT.length > 0) {
                                    // check ID_DoiTuong if KH offline (if KH offline saved)

                                    let itemDToff = $.grep(dtOffline, function (x) {
                                        return x.MaDoiTuong === idDTuongHD;
                                    });

                                    if (itemDToff.length > 0) {
                                        idDTuongHD = itemDToff[0].ID;
                                    }

                                    if (idDTuongHD !== null && idDTuongHD.length >= 36) {
                                        myData.objHoaDon.ID_DoiTuong = idDTuongHD;
                                        PostHD_SoQuy_DongBo(myData);
                                    }
                                    else {
                                        // 1.add DoiTuong, HoaDon
                                        AddKhachHang_HoaDon(itemDT[0], myData, true);
                                    }
                                }
                                else {
                                    // check ID_DoiTuong if KH offline (if KH offline saved)
                                    var itemDToff = $.grep(dtOffline, function (x) {
                                        return x.MaDoiTuong === idDTuongHD;
                                    });

                                    if (itemDToff.length > 0) {
                                        idDTuongHD = itemDToff[0].ID;
                                    }

                                    // assign again ID_DoiTuong 
                                    myData.objHoaDon.ID_DoiTuong = idDTuongHD;
                                    if (idDTuongHD === null || idDTuongHD.length >= 36) {
                                        PostHD_SoQuy_DongBo(myData);
                                    }
                                }
                            }
                        }
                        localStorage.removeItem(lcListKHoffline);

                        // remove all HDOffline
                        lst_HDoffline = $.grep(lst_HDoffline, function (item) {
                            return item.StatusOffline === false;
                        });
                        localStorage.setItem(lcListHD, JSON.stringify(lst_HDoffline));

                        // remove all CTHD with MaHoaDon = HDO
                        lstCTHD = $.grep(lstCTHD, function (x) {
                            return x.MaHoaDon.indexOf('HDO') === -1;
                        })
                        localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));

                        // reset HDO
                        self.countHDoffilne(0);
                        self.HoaDonOffline([]);

                        BindHD_CTHD();
                        break;
                    case 1:
                        for (let i = 0; i < lstHDO.length; i++) {

                            maHoaDon = lstHDO[i].MaHoaDon;

                            if (lstHDO[i].ID.indexOf('0000') === -1) {
                                arrID_HoaDon.push(lstHDO[i].ID); //--> send to client
                            }

                            let arrHangHoa = [];
                            for (let j = 0; j < lstCTHD.length; j++) {
                                if (lstCTHD[j].MaHoaDon === maHoaDon) {
                                    lstCTHD[j].Bep_SoLuongYeuCau = 0;
                                    lstCTHD[j].Bep_SoLuongChoCungUng = 0;
                                    arrHangHoa.push(lstCTHD[j]);
                                }
                            }
                            if (arrHangHoa.length > 0) {

                                let myData = {};
                                // ID_DonVi
                                if (_idDonVi !== '') {
                                    lstHDO[i].ID_DonVi = _idDonVi;
                                }
                                else {
                                    lstHDO[i].ID_DonVi = '00000000-0000-0000-0000-000000000000';
                                }

                                // ngaylap HD
                                var ngaylapHD = GetNgayLapHD_whenSave(lstHDO[i].NgayLapHoaDon);
                                lstHDO[i].NgayLapHoaDon = ngaylapHD;

                                myData.objHoaDon = lstHDO[i];
                                myData.objCTHoaDon = arrHangHoa;


                                let idDTuongHD = myData.objHoaDon.ID_DoiTuong;
                                let itemDT = $.grep(dtOffline, function (item) {
                                    return item.ID === idDTuongHD;
                                });

                                if (arrID_HoaDon.length > 0) {
                                    $('#wait').remove();
                                    // send to client
                                    let objSend = {
                                        ID_HoaDons: arrID_HoaDon,
                                        IDRandomHD: '',
                                        ID_User: _idUser,
                                        ID_ViTri: _phongBanID,
                                        IDChiNhanh: _idDonVi,
                                        Func: 4
                                    }
                                    SendData_atThisPage(objSend);

                                    // check exist bep and sen data
                                    let arrExistBep = $.grep(lstCTHD, function (x) {
                                        return $.inArray(x.ID_HoaDon, arrID_HoaDon) > -1 && (x.Bep_SoLuongYeuCau > 0 || x.Bep_SoLuongChoCungUng > 0);
                                    });
                                    if (arrExistBep.length > 0 && connected) {
                                        hub.server.sendData_ThuNgan_ToNhaBep(objSend);
                                    }
                                }

                                if (itemDT.length > 0) {
                                    // check ID_DoiTuong if KH offline (if KH offline saved)
                                    let itemDToff = $.grep(dtOffline, function (x) {
                                        return x.MaDoiTuong === idDTuongHD;
                                    });

                                    if (itemDToff.length > 0) {
                                        idDTuongHD = itemDToff[0].ID;
                                    }

                                    if (idDTuongHD !== null && idDTuongHD.length >= 36) {
                                        myData.objHoaDon.ID_DoiTuong = idDTuongHD;
                                        PostHD_SoQuy_DongBo(myData);
                                    }
                                    else {
                                        // 1.add DoiTuong, HoaDon
                                        AddKhachHang_HoaDon(itemDT[0], myData, true);
                                    }
                                }
                                else {
                                    // check ID_DoiTuong if KH offline (if KH offline saved)
                                    let itemDToff = $.grep(dtOffline, function (x) {
                                        return x.MaDoiTuong === idDTuongHD;
                                    });

                                    if (itemDToff.length > 0) {
                                        idDTuongHD = itemDToff[0].ID;
                                    }

                                    // assign again ID_DoiTuong
                                    myData.objHoaDon.ID_DoiTuong = idDTuongHD;
                                    if (idDTuongHD === null || idDTuongHD.length >= 36) {
                                        PostHD_SoQuy_DongBo(myData);
                                    }
                                }
                            }
                        }

                        localStorage.removeItem(lcListKHoffline);

                        // remove all HDOffline
                        var lstHDAfter = $.grep(lst_HDoffline, function (item) {
                            return item.LoaiHoaDon !== 1;
                        });
                        localStorage.setItem(lcListHD, JSON.stringify(lstHDAfter));

                        // get arrIDRandom còn lại
                        var arrIDRandom = [];
                        for (let i = 0; i < lstHDAfter.length; i++) {
                            arrIDRandom.push(lstHDAfter[i].IDRandom);
                        }

                        // remove all CTHD with IDRandomHD not exist
                        var lstCTHDAfter = $.grep(lstCTHD, function (x) {
                            return $.inArray(x.IDRandomHD, arrIDRandom) > -1;
                        })
                        localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHDAfter));

                        Bind_andCountHDO();
                        BindHD_CTHD();
                        break;
                    case 3: // loai = 3: khong reset Bep_SoLuongYeuCau
                        for (let i = 0; i < lstDHO.length; i++) {

                            maHoaDon = lstDHO[i].MaHoaDon;

                            if (lstDHO[i].ID.indexOf('0000') === -1) {
                                arrID_HoaDon.push(lstDHO[i].ID); //--> send to client
                            }

                            // check exist bep
                            var arrExistBep = $.grep(lstCTHD, function (x) {
                                return $.inArray(x.ID_HoaDon, arrID_HoaDon) > -1 && (x.Bep_SoLuongYeuCau > 0 || x.Bep_SoLuongChoCungUng > 0);
                            });

                            var arrHangHoa = [];
                            for (let j = 0; j < lstCTHD.length; j++) {
                                if (lstCTHD[j].MaHoaDon === maHoaDon) {
                                    arrHangHoa.push(lstCTHD[j]);
                                }
                            }
                            if (arrHangHoa.length > 0) {

                                var myData = {};
                                // ID_DonVi
                                if (_idDonVi !== '') {
                                    lstDHO[i].ID_DonVi = _idDonVi;
                                }
                                else {
                                    lstDHO[i].ID_DonVi = '00000000-0000-0000-0000-000000000000';
                                }

                                // ngaylap HD
                                var ngaylapHD = GetNgayLapHD_whenSave(lstDHO[i].NgayLapHoaDon);
                                lstDHO[i].NgayLapHoaDon = ngaylapHD;
                                lst_HDoffline[i].ChoThanhToan = false;

                                myData.objHoaDon = lstDHO[i];
                                myData.objCTHoaDon = arrHangHoa;

                                let idDTuongHD = myData.objHoaDon.ID_DoiTuong;
                                let itemDT = $.grep(dtOffline, function (item) {
                                    return item.ID === idDTuongHD;
                                });

                                if (arrID_HoaDon.length > 0) {
                                    $('#wait').remove();
                                    // send to client
                                    var objSend = {
                                        ID_HoaDons: arrID_HoaDon,
                                        IDRandomHD: '',
                                        ID_User: _idUser,
                                        ID_ViTri: _phongBanID,
                                        IDChiNhanh: _idDonVi,
                                        Func: 4
                                    }
                                    SendData_atThisPage(objSend);

                                    if (arrExistBep.length > 0 && connected) {
                                        hub.server.sendData_ThuNgan_ToNhaBep(objSend);
                                    }
                                    //hub.server.sendData_DongBoHoa(JSON.stringify(arrID_HoaDon), localStorage.getItem(lcPhongBanID));
                                }

                                if (itemDT.length > 0) {
                                    // check ID_DoiTuong if KH offline (if KH offline saved)
                                    let itemDToff = $.grep(dtOffline, function (x) {
                                        return x.MaDoiTuong === idDTuongHD;
                                    });

                                    if (itemDToff.length > 0) {
                                        idDTuongHD = itemDToff[0].ID;
                                    }

                                    if (idDTuongHD !== null && idDTuongHD.length >= 36) {
                                        myData.objHoaDon.ID_DoiTuong = idDTuongHD;
                                        PostHD_SoQuy_DongBo(myData);
                                    }
                                    else {
                                        // 1.add DoiTuong, HoaDon
                                        AddKhachHang_HoaDon(itemDT[0], myData, true);
                                    }
                                }
                                else {
                                    // check ID_DoiTuong if KH offline (if KH offline saved)
                                    let itemDToff = $.grep(dtOffline, function (x) {
                                        return x.MaDoiTuong === idDTuongHD;
                                    });

                                    if (itemDToff.length > 0) {
                                        idDTuongHD = itemDToff[0].ID;
                                    }
                                    // assign again ID_DoiTuong 
                                    myData.objHoaDon.ID_DoiTuong = idDTuongHD;
                                    if (idDTuongHD === null || idDTuongHD.length >= 36) {
                                        PostHD_SoQuy_DongBo(myData);
                                    }
                                }
                            }
                        }

                        localStorage.removeItem(lcListKHoffline);

                        // remove all HDOffline
                        var lstHDAfter = $.grep(lst_HDoffline, function (item) {
                            return item.LoaiHoaDon !== 3;
                        });
                        localStorage.setItem(lcListHD, JSON.stringify(lstHDAfter));

                        // get arrIDRandom còn lại
                        var arrIDRandom = [];
                        for (let i = 0; i < lstHDAfter.length; i++) {
                            arrIDRandom.push(lstHDAfter[i].IDRandom);
                        }

                        // remove all CTHD with IDRandomHD not exist
                        var lstCTHDAfter = $.grep(lstCTHD, function (x) {
                            return $.inArray(x.IDRandomHD, arrIDRandom) > -1;
                        })
                        localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHDAfter));

                        Bind_andCountHDO();
                        BindHD_CTHD();
                        break;
                }
            }
        }
        $('.bs-example-modal-lg').modal('hide');
    }

    function Bind_andCountHDO(lstHD) {
        var hd = localStorage.getItem(lcListHD);
        var hdo = [];
        if (hd !== null) {
            hd = JSON.parse(hd);
            hdo = $.grep(hd, function (x) {
                return x.StatusOffline === true && x.ID_DonVi === _idDonVi && x.ID_User === _idUser;
            });
        }
        self.countHDoffilne(hdo.length);
        self.HoaDonOffline(hdo);
    }

    function RemoveKHoffline_afterSave() {

        var arrID_DoiTuong = [];
        // get lstHD after save
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            var arrID_DoiTuong = [];
            for (let i = 0; i < lstHD.length; i++) {
                // get all ID_DoiTuong in lstHD
                arrID_DoiTuong.push(lstHD[i].ID_DoiTuong);
            }

            // delete in KH offline
            var khOffline = localStorage.getItem(lcListKHoffline);
            if (khOffline !== null) {
                khOffline = JSON.parse(khOffline);
                for (let i = 0; i < khOffline.length; i++) {
                    // delete KH offline if (ID && MaDoiTuong) not exist in arrID_DoiTuong (OK)
                    if (($.inArray(khOffline[i].MaDoiTuong, arrID_DoiTuong) === -1) && ($.inArray(khOffline[i].ID, arrID_DoiTuong) === -1)) {
                        khOffline.splice(i, 1);
                    }
                }

                localStorage.setItem(lcListKHoffline, JSON.stringify(khOffline));
                console.log('DM_DoiTuongs_AFTER_DELETE ', JSON.stringify(khOffline))
            }
        }
        else {
            localStorage.removeItem(lcListKHoffline);
        }
    }

    self.showModalChuyenGhepBan = function () {
        // tìm the li dang active
        var liActive = $('.divLstHoaDon span.active');
        var thisMaHoaDon = liActive.find('span:eq(0)').text();

        // get name of PhongBan + remove 1 PhongBan
        var itemPB = $.grep(self.AllPhongBans(), function (item) {
            return item.ID === _phongBanID;
        });
        if (itemPB.length > 0) {
            // reset rdoChuyenGhep, ddlPhongBan
            self.CheckChuyenBan(1);
            self.HoaDonByIDViTri([]);
            $('#txtSearchPB').val('');
            $('#titleChuyenGhepBan').text('Hóa đơn ' + thisMaHoaDon + '_ Bàn ' + itemPB[0].TenViTri);

            var arr = $.grep(self.AllPhongBans(), function (item) {
                return item.ID !== _phongBanID && item.ID !== const_GuidEmpty;
            });
            self.PhongBans_ChuyenBan(arr);

            var cthd = localStorage.getItem(lcListCTHD);
            // check if SoLuong > TonKho
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                var objCTAdd = $.grep(cthd, function (item) {
                    return item.ID_ViTri === _phongBanID && item.MaHoaDon === liActive.find('span:eq(0)').text() && item.ID_DonVi === _idDonVi;
                });

                if (self.ThietLap().XuatAm === false) {
                    var msgErr = "";
                    for (let i = 0; i < objCTAdd.length; i++) {
                        if (objCTAdd[i].LaHangHoa && objCTAdd[i].SoLuong > objCTAdd[i].TonKho) {
                            msgErr += objCTAdd[i].TenHangHoa + ", ";
                        }
                    }

                    if (msgErr !== "") {
                        msgErr = msgErr.substr(0, msgErr.length - 2);
                        ShowMessage_Danger('Không đủ số lượng tồn kho cho sản phẩm ' + msgErr);
                        Enable_BtnGhepBan();
                        return false;
                    }
                    $('#modalChuyenGhepBan').modal('show');
                }
                else {
                    $('#modalChuyenGhepBan').modal('show');
                }
            }
            else {
                ShowMessage_Danger('Vui lòng nhập chi tiết hóa đơn');
                return false;
            }
        }
    }

    self.ChuyenGhepBan_ChosePhong = function (item) {
        if (self.CheckChuyenBan() !== 0) {
            $('#txtSearchPB').val(item.TenViTri);
            self.selectedPhongBan_GhepHoaDon(item.ID);
            var cthd = localStorage.getItem(lcListCTHD);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);

                var lstHD = localStorage.getItem(lcListHD);
                if (lstHD !== null) {
                    lstHD = JSON.parse(lstHD);

                    // get all hd of phong is chosing (not hdopening)
                    let hdOfTable = $.grep(lstHD, function (x) {
                        return x.ID_DonVi === _idDonVi && x.ID_ViTri === item.ID && x.StatusOffline === false;
                    });
                    // neu ghephoadon cung ban
                    if (self.CheckChuyenBan() === 2 && self.selectedPhongBan() === item.ID) {
                        hdOfTable = $.grep(hdOfTable, function (x) {
                            return x.MaHoaDon !== _maHoaDon;
                        });
                    }

                    for (let i = 0; i < hdOfTable.length; i++) {
                        let itFor = hdOfTable[i];
                        let cusName = 'Khách lẻ';
                        let cusItem = $.grep(self.DoiTuongs(), function (x) {
                            return x.ID === itFor.ID_DoiTuong;
                        });
                        if (cusItem.length > 0) {
                            cusName = cusItem[0].TenDoiTuong;
                        }

                        // sum soluong in ct
                        let soluong = 0;
                        for (let j = 0; j < cthd.length; j++) {
                            if (cthd[j].IDRandomHD === itFor.IDRandom) {
                                soluong += cthd[j].SoLuong;
                            }
                        }
                        hdOfTable[i].TenDoiTuong = cusName;
                        hdOfTable[i].SoLuong = soluong;
                    }
                    self.HoaDonByIDViTri(hdOfTable);
                }
            }
        }
    };

    self.CheckChuyenBan.subscribe(function (newVal) {
        var arr = self.AllPhongBans();
        switch (newVal) {
            case 1:// chuyenban
                arr = $.grep(self.AllPhongBans(), function (x) {
                    return x.ID !== _phongBanID;
                });
                break;
        }
        self.PhongBans_ChuyenBan(arr);
        self.HoaDonByIDViTri([]);
        model_Phong.reset();
        $('#showseach_PB').hide();
    });

    // get Vitri cua HoaDon den (GhepHoaDon)
    self.choseHD = function (item) {
        self.selectedPhongBan_GhepHoaDon(item.ID_ViTri);
    };

    function GetHDOpening_byMaHoaDon_andViTri(maHoaDon, idViTri) {
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            var itemEx = $.grep(lstHD, function (x) {
                return x.ID_ViTri === idViTri && x.MaHoaDon === maHoaDon && x.ID_DonVi === _idDonVi;
            });
            if (itemEx.length > 0) {
                return itemEx;
            }
            else {
                return [];
            }
        }
    }

    function GetListHD_NotOffline_byUser_andViTri(idViTri, lstHD) {
        return $.grep(lstHD, function (x) {
            return x.ID_ViTri === idViTri && x.ID_DonVi === _idDonVi && x.StatusOffline === false;
        })
    }

    function CreateObjHub(jsonHD, jsonCTHD, idRandomHD, idViTri, typeFunc, idQuiDoi) {
        return {
            HD: jsonHD,
            CTHD: jsonCTHD,
            IDRandomHD: idRandomHD,
            ID_User: _idUser,
            ID_ViTri: idViTri,
            IDChiNhanh: _idDonVi,
            ID_DonViQuiDoi: idQuiDoi,
            Func: typeFunc
        }
    }
    self.ChuyenGhepBan = function () {
        var thisMaHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
        var lstHD = localStorage.getItem(lcListHD);
        var cthd = localStorage.getItem(lcListCTHD);
        var idViTri = _phongBanID;
        var idViTriCurrent = self.selectedPhongBan_GhepHoaDon();// idVitri after change now
        var tableName = '';
        var tableNew = $.grep(self.AllPhongBans(), function (x) {
            return x.ID === idViTriCurrent;
        });
        if (tableName.length > 0) {
            tableName = tableNew[0].TenViTri;
        }

        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            if (lstHD !== null) {
                lstHD = JSON.parse(lstHD);
            }
            else {
                lstHD = [];
            }

            var hdOpening = GetHDOpening_byMaHoaDon_andViTri(thisMaHoaDon, _phongBanID);
            if (hdOpening.length > 0) {
                var objHDAdd = [];
                var objCTAdd = [];
                var maHDon = thisMaHoaDon;
                var idRandomHD = hdOpening[0].IDRandom;
                _phongBanID = idViTriCurrent;
                localStorage.setItem(lcPhongBanID, _phongBanID);

                switch (self.CheckChuyenBan()) {
                    case 1: // chuyenban
                        if (thisMaHoaDon.indexOf('DH') === -1 && navigator.onLine === false) {
                            maHDon = 'DHO' + Math.floor(Math.random() * 1000000 + 1);
                        }
                        // update hd with newVT
                        for (let i = 0; i < lstHD.length; i++) {
                            if (lstHD[i].IDRandom === idRandomHD) {
                                lstHD[i].MaHoaDon = maHDon;
                                lstHD[i].ID_ViTri = idViTriCurrent;
                                lstHD[i].TenPhongBan = tableName;
                                objHDAdd = lstHD[i];
                                break;
                            }
                        }

                        // update cthd with newVT
                        for (let j = 0; j < cthd.length; j++) {
                            if (cthd[j].IDRandomHD === idRandomHD) {
                                cthd[j].MaHoaDon = maHDon;
                                cthd[j].ID_ViTri = idViTriCurrent;
                                cthd[j].TenPhongBan = tableName;
                                objCTAdd.push(cthd[j]);
                            }
                        }
                        objCTAdd = AssignProperties_forCTHD_whenSave(objCTAdd);

                        if (navigator.onLine) {
                            // save to DB
                            if (maHDon.indexOf('DH') === -1) {
                                objHDAdd.MaHoaDon = null;
                            }
                            objHDAdd.ChoThanhToan = true;
                            objHDAdd.LoaiHoaDon = 3;

                            let myData = {
                                objHoaDon: objHDAdd,
                                objCTHoaDon: objCTAdd,
                                IsSetGiaVonTrungBinh: self.ThietLap().GiaVonTrungBinh,
                            };
                            console.log('ghepban ', myData)
                            ajaxHelper(BH_HoaDonUri + "PostBH_HoaDon_NhaHang", 'POST', myData).done(function (objReturn) {
                                if (objReturn.res === true) {
                                    var item = objReturn.data;
                                    _maHoaDon = item.MaHoaDon;

                                    // update ID, MaHoaDon to hd
                                    for (let i = 0; i < lstHD.length; i++) {
                                        if (lstHD[i].IDRandom === idRandomHD) {
                                            lstHD[i].ID = item.ID;
                                            lstHD[i].MaHoaDon = item.MaHoaDon;
                                            break;
                                        }
                                    }
                                    localStorage.setItem(lcListHD, JSON.stringify(lstHD));

                                    // update id CTHoaDon
                                    for (var j = 0; j < cthd.length; j++) {
                                        if (cthd[j].IDRandomHD === idRandomHD) {
                                            cthd[j].MaHoaDon = item.MaHoaDon;
                                            cthd[j].ID_HoaDon = item.ID;

                                            // update again ID for CTHD
                                            for (let i = 0; i < item.BH_HoaDon_ChiTiet.length; i++) {
                                                let cthdDB = item.BH_HoaDon_ChiTiet[i];
                                                if (cthd[j].ID_DonViQuiDoi === cthdDB.ID_DonViQuiDoi) {
                                                    cthd[j].ID = cthdDB.ID;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
                                    ShowMessage_Success('Ghép bàn thành công');

                                    // bind again list HD, CTHD and send data to client
                                    BindHD_CTHD();
                                    CountTableUsed(false);
                                    UpdateTime_lcPhongBan();

                                    var objSend = CreateObjHub(JSON.stringify(lstHD), JSON.stringify(cthd), idRandomHD, idViTriCurrent, 1, null);
                                    SendData_atThisPage(objSend);

                                    // send to Kitchen and update again ID CTHD
                                    var arrBep = $.grep(cthd, function (x) {
                                        return x.IDRandomHD === idRandomHD
                                            && (x.Bep_SoLuongYeuCau > 0 || x.Bep_SoLuongChoCungUng > 0);
                                    });

                                    if (arrBep.length > 0 && connected) {
                                        ThongBaoBep_SendData(cthd, 5);
                                    }
                                }
                                else {
                                    console.log(objReturn.mes);
                                    ShowMessage_Danger('Ghép bàn thất bại');
                                }
                            });
                        }
                        else {
                            _maHoaDon = maHDon;
                            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                            BindHD_CTHD(_maHoaDon);
                            CountTableUsed(false);
                            UpdateTime_lcPhongBan();
                            ShowMessage_Success('Ghép bàn thành công');
                        }
                        break;
                    case 2: // ghep hd
                        maHDon = self.choseHDKH();
                        if (maHDon === '' || maHDon === undefined) {
                            ShowMessage_Danger('Vui lòng chọn hóa đơn để ghép');
                            return false;
                        }

                        if (navigator.onLine === false) {
                            if (maHDon.indexOf('DH') === -1) {
                                maHDon = 'DHO' + Math.floor(Math.random() * 100000 + 1);
                            }
                        }

                        // delete if exist db
                        if (hdOpening[0].ID !== const_GuidEmpty && navigator.onLine) {
                            ajaxHelper(BH_HoaDonUri + 'DeleteBH_HoaDon/' + hdOpening[0].ID, 'DELETE').done(function () {
                            });
                        }

                        // get hd parent (hd duoc ghep)
                        var hdParent = GetHDOpening_byMaHoaDon_andViTri(maHDon, idViTriCurrent);
                        if (hdParent.length > 0) {
                            let newIDRandom = hdParent[0].IDRandom;

                            // delete hdold
                            lstHD = $.grep(lstHD, function (x) {
                                return x.IDRandom !== idRandomHD;
                            });
                            // update MaHoaDon in hd
                            for (let j = 0; j < lstHD.length; j++) {
                                if (lstHD[j].IDRandom === newIDRandom) {
                                    lstHD[j].MaHoaDon = maHDon;
                                    lstHD[j].SoLuongKhachHang = hdOpening[0].SoLuongKhachHang + lstHD[j].SoLuongKhachHang;
                                    break;
                                }
                            }
                            localStorage.setItem(lcListHD, JSON.stringify(lstHD));

                            // get arrCT of parent --> gop dongia at ctold
                            var ctParent = $.grep(cthd, function (x) {
                                return x.IDRandomHD === newIDRandom;
                            });
                            // update cthd with newIDRandom
                            for (let i = 0; i < cthd.length; i++) {
                                if (cthd[i].IDRandomHD === newIDRandom) {
                                    cthd[i].MaHoaDon = maHDon;
                                }
                                if (cthd[i].IDRandomHD === idRandomHD) {
                                    cthd[i].IDRandomHD = newIDRandom;
                                    cthd[i].MaHoaDon = maHDon;
                                    cthd[i].TenPhongBan = tableName;
                                    cthd[i].ID_ViTri = idViTriCurrent;
                                }
                            }

                            // gop hang hoa if trung
                            //for (let k = 0; k < cthd.length; k++) {
                            //    for (var j = k + 1; j < cthd.length; j++) {
                            //        if (cthd[j].IDRandomHD === newIDRandom && cthd[j].ID_DonViQuiDoi === cthd[k].ID_DonViQuiDoi) {
                            //            let giabanBefore = 0;
                            //            let dongiaBefore = 0;
                            //            let tienCK = 0;
                            //            let ptCK = 0;
                            //            let dvt = 0;

                            //            // get DonGia from HoaDon đích
                            //            for (let h = 0; h < ctParent.length; h++) {
                            //                if (ctParent[h].ID_DonViQuiDoi === cthd[k].ID_DonViQuiDoi) {
                            //                    giabanBefore = ctParent[h].GiaBan;
                            //                    dongiaBefore = ctParent[h].DonGia;
                            //                    tienCK = ctParent[h].TienChietKhau;
                            //                    ptCK = ctParent[h].PTChietKhau;
                            //                    dvt = ctParent[h].DVTinhGiam;
                            //                    break;
                            //                }
                            //            }
                            //            cthd[k].DonGia = dongiaBefore;
                            //            cthd[k].GiaBan = giabanBefore;
                            //            cthd[k].PTChietKhau = ptCK;
                            //            cthd[k].TienChietKhau = tienCK;
                            //            cthd[k].DVTinhGiam = dvt;
                            //            cthd[k].SoLuong = cthd[k].SoLuong + cthd[j].SoLuong;
                            //            cthd[k].ThanhTien = cthd[k].SoLuong * cthd[k].GiaBan;
                            //            cthd.splice(j, 1);
                            //        }
                            //    }
                            //}
                            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                            // update tongtien hd
                            let sum = Sum_ThanhTienCTHD(cthd, newIDRandom);
                            UpdateHD_whenChangeCTHD(sum, newIDRandom);
                            UpdateGiamGiaHD_ByNhomKH();
                            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(newIDRandom);

                            // save DB
                            if (navigator.onLine) {
                                var hdAfter = localStorage.getItem(lcListHD);
                                if (hdAfter !== null) {
                                    hdAfter = JSON.parse(hdAfter);
                                    objHDAdd = $.grep(hdAfter, function (x) {
                                        return x.IDRandom === newIDRandom;
                                    });
                                }

                                objCTAdd = $.grep(cthd, function (x) {
                                    return x.IDRandomHD === newIDRandom;
                                });
                                if (objHDAdd.length > 0) {
                                    let ngaylapHD = GetNgayLapHD_whenSave(objHDAdd[0].NgayLapHoaDon);
                                    objHDAdd[0].NgayLapHoaDon = ngaylapHD;
                                    objHDAdd[0].LoaiHoaDon = 3;
                                    objHDAdd[0].ChoThanhToan = true;
                                    if (objHDAdd[0].ID !== const_GuidEmpty) {
                                        objHDAdd[0].MaHoaDon = maHDon;
                                    }
                                    else {
                                        objHDAdd[0].MaHoaDon = null;
                                    }

                                    let myData = {
                                        objHoaDon: objHDAdd[0],
                                        objCTHoaDon: objCTAdd,
                                        IsSetGiaVonTrungBinh: self.ThietLap().GiaVonTrungBinh
                                    };
                                    console.log('ghepHD_myData ', myData)

                                    ajaxHelper(BH_HoaDonUri + "PostBH_HoaDon_NhaHang", 'POST', myData).done(function (objReturn) {
                                        if (objReturn.res === true) {
                                            var item = objReturn.data;
                                            _maHoaDon = item.MaHoaDon;

                                            // update ID, MaHoaDon from DB -> cache
                                            let arrHDnew = [];
                                            for (let i = 0; i < hdAfter.length; i++) {
                                                if (hdAfter[i].IDRandom === newIDRandom) {
                                                    hdAfter[i].ID = item.ID;
                                                    hdAfter[i].MaHoaDon = item.MaHoaDon;
                                                    arrHDnew = [hdAfter[i]]; // --> to do send client
                                                    break;
                                                }
                                            }
                                            localStorage.setItem(lcListHD, JSON.stringify(hdAfter));

                                            // update MaHoaDon in cache CTHD
                                            for (let j = 0; j < cthd.length; j++) {
                                                if (cthd[j].IDRandomHD === newIDRandom) {
                                                    cthd[j].MaHoaDon = item.MaHoaDon;
                                                    // update again ID for CTHD
                                                    for (let i = 0; i < item.BH_HoaDon_ChiTiet.length; i++) {
                                                        var cthdDB = item.BH_HoaDon_ChiTiet[i];
                                                        if (cthd[j].ID_DonViQuiDoi === cthdDB.ID_DonViQuiDoi) {
                                                            cthd[j].ID = cthdDB.ID;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
                                            ShowMessage_Success('Ghép hóa đơn thành công');

                                            BindHD_CTHD(_maHoaDon);
                                            UpdateTime_lcPhongBan();
                                            CountTableUsed(false);

                                            // get CTHD vừa ghép
                                            let arrCTHDnew = $.grep(cthd, function (x) {
                                                return x.IDRandomHD === newIDRandom; // (item.MaHoaDon)--> get from DB
                                            });
                                            var objSend = CreateObjHub(JSON.stringify(arrHDnew), JSON.stringify(arrCTHDnew), newIDRandom, _phongBanID, 1, null);
                                            SendData_atThisPage(objSend);
                                            var arrBep = jQuery.grep(cthd, function (x) {
                                                return x.IDRandomHD === newIDRandom
                                                    && (x.Bep_SoLuongYeuCau > 0 || x.Bep_SoLuongChoCungUng > 0);
                                            });
                                            if (arrBep.length > 0 && connected) {
                                                hub.server.send(JSON.stringify(lstCTHD), id_DonVi);
                                            }
                                        }
                                        else {
                                            console.log(2, objReturn.mes);
                                            ShowMessage_Danger('Ghép hóa đơn thất bại');
                                        }
                                    });
                                }
                            }
                            else {
                                _maHoaDon = maHDon;
                                BindHD_CTHD(_maHoaDon);
                                UpdateTime_lcPhongBan();
                                CountTableUsed(false);
                            }
                        }
                        break;
                }
            }
        }
    }

    function GetMaxMaHoaDon_ofTable(lstHD) {
        var max = 0;
        var lstHD_ofTable = $.grep(lstHD, function (x) {
            return x.ID_ViTri == _phongBanID && x.StatusOffline === false && x.ID_DonVi === _idDonVi && x.ID === const_GuidEmpty;
        });
        if (lstHD_ofTable.length > 0) {
            max = Math.max.apply(Math, lstHD_ofTable.map(function (item) {
                return item.MaHoaDon.match(/\d+/);
            }));
        }
        else {
            max = 0;
        }
        return max;
    }

    self.addPhongBan = function () {

        $('#lblTienMat').text('(Tiền mặt)');
        self.NgayLapHoaDon(moment(new Date()).format('DD/MM/YYYY HH:mm'));

        Enable_BtnThongBaoNB();
        Enable_DisableNgayLapHD();
        ResetTextSearch();

        // count MaHoaDon max of PhongBan
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
        }
        else {
            lstHD = [];
        }
        var max = GetMaxMaHoaDon_ofTable(lstHD) + 1;
        _maHoaDon = max.toString();;

        // addPhongBan --> addHoaDon
        var newObj = newHoaDon(_phongBanID, _maHoaDon);
        newObj.GioVao = convertDateTime(new Date());
        lstHD.push(newObj);
        localStorage.setItem(lcListHD, JSON.stringify(lstHD));

        var arr = GetHDOpening_ofTable();
        self.PhongBanSelected(arr);
        StyDSHoaDon_byMaHoaDon();
        UpdateTime_lcPhongBan();
        self.resetInforHD_CTHD();
        if (self.IndexHD_inTable() < self.PhongBanSelected().length && self.PhongBanSelected().length > self.pageSizeHD()) {
            self.IndexHD_inTable(self.IndexHD_inTable() + 1);
        }
    }
    function convertDateTime(input) {
        if (input !== null && input !== undefined && input !== '') {
            return moment(input).format("YYYY-MM-DD HH:mm:ss");
        }
        return "";
    }

    function newHoaDon(idViTri, maHoaDon) {
        var tableName = '';
        var table = $.grep(self.AllPhongBans(), function (x) {
            return x.ID === _phongBanID;
        });
        if (table.length > 0) {
            tableName = table[0].TenViTri;
        }
        var ngaylapHD = moment(new Date()).format("YYYY-MM-DD HH:mm");

        return {
            IDRandom: CreateIDRandom('HD_'),
            LoaiHoaDon: 1,
            ID_DoiTuong: null,
            ID_NhanVien: _idNhanVien,
            ID_DonVi: _idDonVi,
            NguoiTao: _userLogin,
            NgayLapHoaDon: ngaylapHD,
            CreateTime: GetTimeNow_HHmm(),
            ID_ViTri: idViTri,
            ID_User: _idUser,
            TongTienHang: 0,
            PhaiThanhToan: 0,
            PTThue: 0,
            TongTienThue: 0,
            TongGiamGia: 0,
            DienGiai: '',
            DVTinhGiam: '%',
            TongChietKhau: 0, // PTGiam
            ID_BangGia: '00000000-0000-0000-0000-000000000000',
            DaThanhToan: 0,
            ChoThanhToan: false,
            MaHoaDon: maHoaDon,
            ID: '00000000-0000-0000-0000-000000000000',
            StatusOffline: false,
            TienMat: 0,
            TienGui: 0,
            TienATM: 0,
            TienTheGiaTri: 0,
            TienThua: 0,
            TrangThaiHD: 1, // open, 2.close
            StatusTBNB: false,
            ID_TaiKhoanPos: null, // in DM_TaiKhoanNganHang
            ID_TaiKhoanChuyenKhoan: null,
            HoanTraTamUng: 0, // use when TraHang
            GioVao: null,
            GioRa: null,
            SoLuongKhachHang: 0,
            // Khuyen mai
            IsKhuyenMaiHD: false,
            PTGiam_KM: 0,
            KhuyeMai_GiamGia: 0,
            TongGiamGiaKM_HD: 0, // KhuyeMai_GiamGia + TongGiamGia of HD
            KhuyenMai_GhiChu: '',
            IsOpeningKMaiHD: false,

            // TichDiem
            DiemGiaoDich: 0, // save in HoaDon
            TTBangDiem: 0, // tien quy doi tu diem --> save in Quy_HoaDon_ChiTiet
            DiemQuyDoi: 0, // so diem quy doi tu tien --> save in Quy_HoaDon_ChiTiet
            DiemHienTai: 0, // = diem hien tai of KH - DiemQuyDoi --> save in DM_DoiTuong

            // giam gia theo nhom
            ID_NhomDTApplySale: null,
            PTChiPhi: 0,
            TongChiPhi: 0,
            DVTinhPhiDV: '%',
            TenPhongBan: tableName,
        }
    }

    self.GetThongTinHoaDon = function (item) {
        ResetTextSearch();
        _maHoaDon = item.MaHoaDon;
        _phongBanID = item.ID_ViTri;

        var itemHD = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);
        if (itemHD.length > 0) {
            var idRandom = itemHD[0].IDRandom;
            if (itemHD[0].StatusOffline) {
                _maHoaDonOffline = thisMaHD;
                Disable_BtnGhepBan();
                $('input,select').attr('disabled', 'disabled');
            }
            else {
                _maHoaDonOffline = '';
                $('input,select').removeAttr('disabled');
                $('#txtNgayTaoHD').attr('disabled', 'disabled');
                Enable_BtnGhepBan();
                Enable_DisableNgayLapHD();
            }

            getChiTietNCCByID(itemHD[0].ID_DoiTuong);
            StyDSHoaDon_byMaHoaDon(_maHoaDon);
            self.SetNhanVien(itemHD[0].ID_NhanVien);
            BindLstBangGia_byNhanVien_andDoiTuong();
            if (self.selectedGiaBan() !== itemHD[0].ID_BangGia) {
                UpdateGiaBan_inListHangHoa_byPage(itemHD[0].ID_BangGia);
            }
            self.SetBangGia(itemHD[0].ID_BangGia);
            BindCTHD_byIDRandomHD(idRandom);
            AssignValue_ofHoaDon(idRandom);
            SetText_lblTienMat(itemHD[0], 1);
        }
        else {
            self.resetInforHD_CTHD();
        }
    }

    self.enabledmangve = ko.observable(true);
    self.ClickImg = function (item, isClickImg) {
        $('input,select').removeAttr('disabled');
        Enable_DisableNgayLapHD();
        ResetTextSearch();

        self.IndexHD_inTable(0);
        _maHoaDonOffline = '';
        _phongBanID = item.ID;
        localStorage.setItem(lcPhongBanID, _phongBanID);
        self.selectedPhongBan(_phongBanID);

        // active tab THUC DON, remove active tab PHONG BAN
        if (isClickImg) {
            // active tab THUC DON, remove active tab PHONG BAN
            $('#homePB').removeClass('active');
            $('#homeFoods').addClass('active');
            $('#home').removeClass('active');
            $('#infor').addClass('active');
            $('#seachHangHoa').show();
            $('#searchPhongBan').hide();
        }

        // get all HoaDon with PhongBan
        BindHD_CTHD();
        CountTableUsed(isClickImg);
        StyDSHoaDon_byMaHoaDon(_maHoaDon);
        ChangeColorImg();
        Enable_BtnGhepBan();

    }
    //phan trang: buton prev, next (class list-table): HangHoa
    self.PageCount = ko.computed(function () {
        var div;
        if (self.PageResults() != null) {
            div = Math.floor(self.HangHoas().length / self.PageSizeProduct());
            div += self.HangHoas().length % self.PageSizeProduct() > 0 ? 1 : 0;
        }
        return div - 1;
    });

    //self.PageResults = ko.computed(function () {
    //    var first = self.currentPageProducts() * self.PageSizeProduct();
    //    if (self.HangHoas() !== null) {
    //        return self.HangHoas().slice(first, first + self.PageSizeProduct());
    //    }
    //});

    self.hasPrevious = ko.computed(function () {
        return self.currentPageProducts() !== 0;
    });
    self.hasNext = ko.computed(function () {
        return self.currentPageProducts() !== self.PageCount() && self.PageCount() > 0;
    });
    self.next = function () {
        if (self.currentPageProducts() < self.PageCount()) {
            self.currentPageProducts(self.currentPageProducts() + 1);
            UpdateGiaBan_inListHangHoa_byPage(self.selectedGiaBan());
        }
    }
    self.previous = function () {
        if (self.currentPageProducts() !== 0) {
            self.currentPageProducts(self.currentPageProducts() - 1);
            UpdateGiaBan_inListHangHoa_byPage(self.selectedGiaBan());
        }
    }

    //phan trang: buton prev, next (class list-table): PhongBan
    self.PageCountPB = ko.computed(function () {

        var div;
        if (self.PhongBans() != null) {
            div = Math.floor(self.PhongBans().length / self.PageSizeTable());
            div += self.PhongBans().length % self.PageSizeTable() > 0 ? 1 : 0;
        }
        return div - 1;
    });

    self.PageResultsPB = ko.computed(function () {
        var first = self.currentPage() * self.PageSizeTable();
        if (self.PhongBans() !== null) {
            let lst = self.PhongBans().filter(o => o.ID !== '00000000-0000-0000-0000-000000000000').slice(first, first + self.PageSizeTable());
            return lst;
        }
        return null;
    });
    self.PageResultsPBMangVe = ko.computed(function () {
        if (self.PhongBans() !== null) {
            return self.PhongBans().filter(o => o.ID === '00000000-0000-0000-0000-000000000000');
        }
        return null;
    });
    self.hasGotoPrev = ko.computed(function () {
        ChangeColorImg();
        return self.currentPage() !== 0;
    });
    self.hasGotoNext = ko.computed(function () {
        ChangeColorImg();
        return self.currentPage() !== self.PageCountPB();
    });
    self.GotoNext = function () {
        if (self.currentPage() < self.PageCountPB()) {
            self.currentPage(self.currentPage() + 1);
        }
    }
    self.GotoPrev = function () {
        if (self.currentPage() != 0) {
            self.currentPage(self.currentPage() - 1);
        }
    }

    self.resetInforHD_CTHD = function () {
        self.HangHoaAfterAdds([]);
        self.HoaDons(new FormModel_NewInvoice());
        self.selectedNCC(null);
        self.ChiTietDoiTuong([]);

        self.SetNhanVien(_idNhanVien);
        BindLstBangGia_byNhanVien_andDoiTuong();
        var idBangGiaOld = self.selectedGiaBan();
        if (idBangGiaOld !== const_GuidEmpty) {
            UpdateGiaBan_inListHangHoa_byPage(const_GuidEmpty);
        }
        self.SetBangGia(const_GuidEmpty);
        $('#ptGiam').text(0);
    }

    self.GotoHome = function () {
        // not permisson
        window.open('/#/DashBoard', '_blank');
    }

    function CheckHoaDonOffline_byMaHoaDon(maHoaDon) {
        if (maHoaDon.indexOf('O') >= 0) {
            ShowMessage_Danger('Hóa đơn đã được thanh toán offline. Không thể thay đổi thông tin');
            return false;
        }
    }

    function newCTHD(productItem, idRandomHD, idHoaDon) {

        var giaVon = productItem.GiaVon;
        var laHangHoa = productItem.LaHangHoa;
        if (!laHangHoa) {
            giaVon = 0;
        }
        var idLoHang = productItem.ID_LoHang;
        var quanlyTheoLo = productItem.QuanLyTheoLoHang;
        if (quanlyTheoLo == false) idLoHang = null;
        var dvtheogio = productItem.DichVuTheoGio;

        return {
            MaHoaDon: _maHoaDon,
            IDRandomHD: idRandomHD,
            ID_ViTri: _phongBanID,
            ID_HoaDon: idHoaDon, // nếu thông báo bếp, sau đó add thêm hàng hóa

            IDRandom: CreateIDRandom('cthd_'),
            SoThuTu: 1,
            ID: const_GuidEmpty,// use when get HD_ChoThanhToan (remove CTHD with ID =const_GuidEmpty)
            ID_HangHoa: productItem.ID,
            ID_LoHang: idLoHang,
            ID_DonViQuiDoi: productItem.ID_DonViQuiDoi,

            SoLuong: dvtheogio === 1 ? 0 : 1,
            DonGia: productItem.GiaBan,
            GiaBan: productItem.GiaBan,
            GiaVon: giaVon,
            TonKho: productItem.TonKho,
            ThanhTien: productItem.GiaBan,
            TienChietKhau: 0,
            PTChietKhau: 0,
            DVTinhGiam: '%',
            PTThue: 0,
            TienThue: 0,
            GhiChu: '',
            Bep_SoLuongYeuCau: 0,
            Bep_SoLuongHoanThanh: 0,
            Bep_SoLuongChoCungUng: 0,
            MaHangHoa: productItem.MaHangHoa,
            TenHangHoa: productItem.TenHangHoa,
            TenDonViTinh: productItem.TenDonViTinh,
            ThuocTinh_GiaTri: productItem.ThuocTinh_GiaTri,
            LaHangHoa: productItem.LaHangHoa,

            Status: 0, // 0: Chua thong bao nha Bep, 1: Dang thong bao, 2: Cho che bien, 3: Da xong
            ThoiGian: moment(new Date()).format('YYYY-MM-DD HH:mm:ss'),
            CssWarning: false,

            // hide/show colum
            ShowSTT: self.show_STT(),
            ShowProductCode: self.show_ProductCode(),
            ShowProductName: self.show_ProductName(),
            ShowProductPrice: self.show_ProductPrice(),
            ShowSumPrice: self.show_SumPrice(),

            GhiChu_NVThucHien: '',
            GhiChu_NVTuVan: '',
            GhiChu_NVThucHienPrint: '',
            GhiChu_NVTuVanPrint: '',
            ThanhPhan_DinhLuong: [],

            DuocTichDiem: productItem.DuocTichDiem,
            Stop: false,// macdinh (false): thoigian dangchay, (true: cho phep thaydoi thoigian)
            DichVuTheoGio: dvtheogio,
            ThoiGianHoanThanh: dvtheogio ? moment(new Date()).format('YYYY-MM-DD HH:mm:ss') : null // if not stop = current time
            // soluong = (new Date() - new Date(ThoiGian))/1000/3600
        }
    }
    function UpdateSoThuTu_CTHD(idRandomHD) {
        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            // get arrHD with MaHoaDon
            var arrHD = $.grep(cthd, function (item) {
                return item.IDRandomHD === idRandomHD;
            });
            // update again SoThuTu for CTHD with MaHoaDon
            var stt = 1;
            for (let i = arrHD.length - 1; i >= 0; i--) {
                arrHD[i].SoThuTu = stt;
                stt = stt + 1;
            }
            for (let i = cthd.length - 1; i >= 0; i--) {
                if (cthd[i].IDRandomHD === idRandomHD) {
                    for (let j = 0; j < arrHD.length; j++) {
                        if (cthd[i].ID_DonViQuiDoi === arrHD[j].ID_DonViQuiDoi && cthd[i].ID_ChiTietGoiDV === arrHD[j].ID_ChiTietGoiDV) {
                            cthd[i].SoThuTu = arrHD[j].SoThuTu;
                        }
                    }
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
        }
    }

    function UpdateWarning_CTHD(idRandomHD) {
        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandomHD === idRandomHD) {
                    if (self.ThietLap().XuatAm === false) {
                        if (cthd[i].LaHangHoa && cthd[i].SoLuong > cthd[i].TonKho) {
                            cthd[i].CssWarning = true;
                        }
                    }
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
        }
    }
    self.addHangHoa = function (item) {
        if (_maHoaDon === '') {
            _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
        };

        // add HangHoa if not HDonOffline
        var isOffline = CheckHoaDonOffline_byMaHoaDon(_maHoaDon);
        if (isOffline === false) {
            return false;
        }

        Enable_BtnThongBaoNB();
        Get_SetHideShowColumCTHD();

        $('.nt2').css('display', ''); // show button Chuyen/Ghep ban
        var idObject = item.ID_DonViQuiDoi; // ID don vi quy doi
        var idRandomHD = '';
        var idHoaDon = const_GuidEmpty;

        // tao HoaDon neu chua co trong cache
        var blExist = false;
        var lstHoaDon = localStorage.getItem(lcListHD);

        if (lstHoaDon === null) {
            lstHoaDon = [];
        }
        else {
            lstHoaDon = JSON.parse(lstHoaDon);
        }

        var itemEx = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);
        var newHD = {};
        if (itemEx.length === 0) {
            newHD = newHoaDon(_phongBanID, _maHoaDon);
            idRandomHD = newHD.IDRandom;
            newHD.GioVao = convertDateTime(new Date());
            lstHoaDon.push(newHD);
            localStorage.setItem(lcListHD, JSON.stringify(lstHoaDon));
        }
        else {
            idHoaDon = itemEx[0].ID;
            idRandomHD = itemEx[0].IDRandom;
        }

        var found = -1;
        var soluongNew;
        var ob1 = newCTHD(item, idRandomHD, idHoaDon);
        ob1.TenViTri = newHD.TenPhongBan;

        var listAllCTHD = localStorage.getItem(lcListCTHD);
        if (listAllCTHD === null) {
            listAllCTHD = [];
            listAllCTHD.push(ob1);
        }
        else {
            // check maHoaDon, maHH
            listAllCTHD = JSON.parse(listAllCTHD);
            for (let i = 0; i < listAllCTHD.length; i++) {
                if (listAllCTHD[i].IDRandomHD === idRandomHD
                    && listAllCTHD[i].ID_DonViQuiDoi == idObject) {
                    found = i;
                    soluongNew = parseFloat(listAllCTHD[i].SoLuong) + 1;
                    break;
                }
            }

            if (found < 0) {
                listAllCTHD.unshift(ob1);
            }
            else {
                // update Soluong, ThanhTien, TienChietKhau, PTChietKhau
                for (let i = 0; i < listAllCTHD.length; i++) {
                    if (listAllCTHD[i].IDRandomHD === idRandomHD
                        && listAllCTHD[i].ID_DonViQuiDoi == idObject) {
                        listAllCTHD[i].SoLuong = soluongNew;
                        listAllCTHD[i].ThanhTien = Math.round(soluongNew * listAllCTHD[i].GiaBan);
                        break;
                    }
                }

            }
        }
        localStorage.setItem(lcListCTHD, JSON.stringify(listAllCTHD));
        UpdateWarning_CTHD(idRandomHD);
        UpdateSoThuTu_CTHD(idRandomHD);
        BindCTHD_byIDRandomHD(idRandomHD);

        UpdateGiamGiaHD_ByNhomKH();
        var sum = Sum_ThanhTienCTHD(listAllCTHD, idRandomHD);
        UpdateHD_whenChangeCTHD(sum, idRandomHD);
        Update_TienThue_CTHD(idRandomHD);
        UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
        AssignValue_ofHoaDon(idRandomHD);
        UpdateTime_lcPhongBan();
    }

    function Sum_ThanhTienCTHD(lstCTHD, idRandomHD) {
        var sum = 0;
        for (let i = 0; i < lstCTHD.length; i++) {
            if (lstCTHD[i].IDRandomHD === idRandomHD) {
                sum += lstCTHD[i].ThanhTien;
            }
        }
        return sum;
    }

    function UpdateHD_whenChangeCTHD(sum, idRandom) {
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].IDRandom === idRandom) {
                    lstHD[i].TongTienHang = sum;
                    if (lstHD[i].PTChiPhi > 0) {
                        lstHD[i].TongChiPhi = parseInt(lstHD[i].PTChiPhi * sum / 100);
                    }
                    if (lstHD[i].TongChietKhau > 0) {
                        lstHD[i].TongGiamGia = parseInt(lstHD[i].TongChietKhau * sum / 100);
                    }
                    if (lstHD[i].PTThue > 0) {
                        lstHD[i].TongTienThue = parseInt(lstHD[i].PTThue * (sum - lstHD[i].TongGiamGia) / 100);
                        lstHD[i].TongTienThue = isNaN(lstHD[i].TongTienThue) ? 0 : lstHD[i].TongTienThue;
                    }
                    lstHD[i].TongGiamGiaKM_HD = formatNumberToFloat(lstHD[i].TongGiamGia) + formatNumberToFloat(lstHD[i].KhuyeMai_GiamGia);

                    var phaiTT = sum + formatNumberToFloat(lstHD[i].TongTienThue) - lstHD[i].TongGiamGiaKM_HD + formatNumberToFloat(lstHD[i].TongChiPhi);
                    lstHD[i].PhaiThanhToan = phaiTT < 0 ? 0 : phaiTT;

                    lstHD[i].DaThanhToan = lstHD[i].PhaiThanhToan;
                    lstHD[i].TienMat = lstHD[i].PhaiThanhToan;
                    lstHD[i].TienGui = 0;
                    lstHD[i].TienATM = 0;
                    lstHD[i].TienThua = 0;
                    lstHD[i].StatusTBNB = false;
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
        }
    }

    self.giamSoLuong = function (item) {
        if (_maHoaDon.indexOf('O') > -1) {
            ShowMessage_Danger('Hóa đơn đã được thanh toán ofline. Không được phép thay đổi');
            return false;
        }

        var ctDoing = FindCTHD_isDoing(item.IDRandom);
        if (ctDoing.DichVuTheoGio === 1 && ctDoing.Stop === false) {
            return;
        }
        var idRandom = item.IDRandom;
        var idRandomHD = item.IDRandomHD;
        var objNumber = $('#munber_' + idRandom);
        var soluongNew = formatNumberToFloat(objNumber.val()) - 1;
        if (soluongNew > 0) {
            objNumber.val(formatNumber(soluongNew));

            var cthd = localStorage.getItem(lcListCTHD);
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === idRandom) {
                    cthd[i].SoLuong = soluongNew;
                    cthd[i].ThanhTien = soluongNew * cthd[i].GiaBan;
                    break;
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
            UpdateWarning_CTHD(idRandomHD);

            var sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
            UpdateHD_whenChangeCTHD(sum, idRandomHD);
            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
            Update_TienThue_CTHD(idRandomHD);
            AssignValue_ofHoaDon(idRandomHD);
            BindCTHD_byIDRandomHD(idRandomHD);
        }

        $(".price-pay-end").focus().select();
    }

    self.tangSoLuong = function (item) {
        if (_maHoaDon.indexOf('O') > -1) {
            ShowMessage_Danger('Hóa đơn đã được thanh toán ofline. Không được phép thay đổi');
            return false;
        }
        var ctDoing = FindCTHD_isDoing(item.IDRandom);
        if (ctDoing.DichVuTheoGio === 1 && ctDoing.Stop === false) {
            return;
        }
        var idRandom = item.IDRandom;
        var idRandomHD = item.IDRandomHD;
        var objNumber = $('#munber_' + idRandom);
        var soluongNew = formatNumberToFloat(objNumber.val()) + 1;
        objNumber.val(formatNumber(soluongNew));

        // update SoLuong, thanhtien in cache
        var cthd = localStorage.getItem(lcListCTHD);
        cthd = JSON.parse(cthd);
        for (let i = 0; i < cthd.length; i++) {
            if (cthd[i].IDRandom === idRandom) {
                cthd[i].SoLuong = soluongNew;
                cthd[i].ThanhTien = soluongNew * cthd[i].GiaBan;
                break;
            }
        }
        localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
        UpdateWarning_CTHD(idRandomHD);

        var sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
        UpdateHD_whenChangeCTHD(sum, idRandomHD);
        UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
        Update_TienThue_CTHD(idRandomHD);
        AssignValue_ofHoaDon(idRandomHD);
        BindCTHD_byIDRandomHD(idRandomHD);
    }

    self.xoaHangHoa = function (item) {
        if (_maHoaDon.indexOf('O') > -1) {
            ShowMessage_Danger('Hóa đơn đã được thanh toán ofline. Không được phép thay đổi');
            return false;
        }
        var idRandom = item.IDRandom;
        var idRandomHD = item.IDRandomHD;

        var cthd = localStorage.getItem(lcListCTHD);
        cthd = JSON.parse(cthd);
        // check exist bep
        var exBep = $.grep(cthd, function (x) {
            return x.IDRandomHD === idRandomHD && (x.Bep_SoLuongYeuCau > 0 || x.Bep_SoLuongChoCungUng > 0);
        });

        for (let i = 0; i < cthd.length; i++) {
            if (cthd[i].IDRandom === idRandom) {
                cthd.splice(i, 1);
                break;
            }
        }
        localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

        //  if cthd = empty: delete hd
        var arrCT = $.grep(cthd, function (x) {
            return x.IDRandomHD === idRandomHD;
        });
        if (arrCT.length === 0) {
            let hd = localStorage.getItem(lcListHD);
            if (hd !== null) {
                hd = JSON.parse(hd);
                hd = $.grep(hd, function (x) {
                    return x.IDRandom !== idRandomHD;
                });
                localStorage.setItem(lcListHD, JSON.stringify(hd));
            }
        }
        else {
            let sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
            UpdateHD_whenChangeCTHD(sum, idRandomHD);
            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
            Update_TienThue_CTHD(idRandomHD);
        }
        BindCTHD_byIDRandomHD(idRandomHD);
        AssignValue_ofHoaDon(idRandomHD);
        UpdateTime_lcPhongBan();

        if (exBep && connected) {
            hub.server.send(JSON.stringify(cthd), _idDonVi);
        }
    }

    function Update_TienThue_CTHD(idRandomHD) {
        var lcHD = localStorage.getItem(lcListHD);
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            var itemHD = $.grep(lcHD, function (x) {
                return x.IDRandom === idRandomHD;
            });

            if (itemHD.length > 0) {
                var ptThueHH = itemHD[0].TongTienThue / (itemHD[0].TongTienHang - itemHD[0].TongGiamGia) * 100;
                ptThueHH = isNaN(ptThueHH) ? 0 : ptThueHH;

                var lstCTHD = localStorage.getItem(lcListCTHD);
                if (lstCTHD !== null) {
                    lstCTHD = JSON.parse(lstCTHD);

                    for (let i = 0; i < lstCTHD.length; i++) {
                        var itemCT = lstCTHD[i];
                        if (itemCT.IDRandomHD === idRandomHD) {
                            lstCTHD[i].PTThue = ptThueHH;
                            lstCTHD[i].TienThue = ptThueHH * lstCTHD[i].ThanhTien / 100;
                        }
                    }
                    localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));
                }
            }
        }
    }

    function AssignValue_ofHoaDon(idRandomHD) {
        // bind infor Hoadon
        var lstHD2 = localStorage.getItem(lcListHD);
        if (lstHD2 !== null) {
            lstHD2 = JSON.parse(lstHD2);
            var itemHD = $.grep(lstHD2, function (item) {
                return item.IDRandom === idRandomHD;
            });

            if (itemHD.length > 0) {
                self.HoaDons().SetData(itemHD[0]);
            }
            else {
                self.HoaDons(new FormModel_NewInvoice());
            }
        };
    }

    self.Change_KhachHang = function (idDoiTuong) {
        self.selectedNCC(idDoiTuong);

        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD === null) {
            lstHD = [];
        }
        else {
            lstHD = JSON.parse(lstHD);
        }
        var idRandomHD = '';
        var isChangeKH = false;
        var itemEx = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);
        if (itemEx.length === 0) {
            // Creat newHoaDon if not exist 
            if (_maHoaDon !== undefined) {
                console.log('change_KH ')
                var newHD = newHoaDon(_phongBanID, _maHoaDon);
                lstHD.push(newHD);
                idRandomHD = newHD.IDRandom;
                itemEx = [newHD];
            }
        }
        else {
            idRandomHD = itemEx[0].IDRandom;
        }
        // update ID_DoiTuong in hd
        for (let i = 0; i < lstHD.length; i++) {
            if (lstHD[i].IDRandom === idRandomHD) {
                if (lstHD[i].ID_DoiTuong !== idDoiTuong) {
                    isChangeKH = true;
                    lstHD[i].ID_DoiTuong = idDoiTuong;
                }
                break;
            }
        }
        localStorage.setItem(lcListHD, JSON.stringify(lstHD));

        // only reset if hd have cthd
        if (isChangeKH && itemEx[0].StatusOffline === false) {
            getChiTietNCCByID(idDoiTuong);
            BindLstBangGia_byNhanVien_andDoiTuong();

            // set default BangGia
            let arrBGNhom = $.grep(self.GiaBans(), function (x) {
                return x.DM_GiaBan_ApDung !== undefined && x.DM_GiaBan_ApDung.length > 0;
            });
            let nhomAD = [];
            if (arrBGNhom.length > 0) {
                for (let i = 0; i < arrBGNhom.length; i++) {
                    for (let j = 0; j < arrBGNhom[i].DM_GiaBan_ApDung.length; j++) {
                        let gbap = arrBGNhom[i].DM_GiaBan_ApDung[j];
                        // chi get banggia cai dat theo nhom
                        if (gbap.ID_NhomKhachHang !== const_GuidEmpty && self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0
                            && self.ChiTietDoiTuong()[0].ID_NhomDoiTuong.toLowerCase().indexOf(gbap.ID_NhomKhachHang) > -1) {
                            nhomAD.push(arrBGNhom[i]);
                            break;
                        }
                    }
                }
            }

            // ưu tiên áp dụng bảng giá theo nhóm
            if (nhomAD.length === 1 && itemEx[0].LoaiHoaDon !== 6) {
                // update again GiaBan cthd
                if (idDoiTuong !== const_GuidEmpty) {
                    self.ChangeBangGia(nhomAD[0]);
                }
                else {
                    let bgc = {
                        TenGiaBan: 'Bảng giá chuẩn',
                        ID: const_GuidEmpty,
                    };
                    self.ChangeBangGia(bgc);
                }
            }
            else {
                // caculator again tongtien after reset Kmai
                var cthd = localStorage.getItem(lcListCTHD);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    var sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
                    UpdateHD_whenChangeCTHD(sum, idRandomHD);
                }
                UpdateGiamGiaHD_ByNhomKH();
                UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);

                AssignValue_ofHoaDon(idRandomHD);
                BindCTHD_byIDRandomHD(idRandomHD);
            }
        }
    }

    self.SetNhanVien = function (idNhanVien) {
        var itemNV = $.grep(self.NhanViens(), function (item) {
            return item.ID === idNhanVien;
        });
        if (itemNV.length > 0) {
            self.titleNhanvien(itemNV[0].TenNhanVien);
        }
        else {
            if (self.NhanViens().length > 0) {
                self.titleNhanvien(self.NhanViens()[0].TenNhanVien);
            }
        }
        self.selectedNVien(idNhanVien);

        $('#ddlNVien i').remove();
        $('#nv_' + idNhanVien).append(element_appendCheck);
    }

    self.SetBangGia = function (idBangGia) {
        var itemBG = $.grep(self.AllBangGia(), function (item) {
            return item.ID === idBangGia;
        });
        if (itemBG.length > 0) {
            self.titleGiaban(itemBG[0].TenGiaBan);
        }
        else {
            self.titleGiaban('Bảng giá chuẩn');
        }
        self.selectedGiaBan(idBangGia);

        $('#ddlGiaBan i').remove();
        $('#bg_' + idBangGia).append(element_appendCheck);
    }

    self.ChangeNVienBan = function (item) {
        $('#txtSearchHH').focus();
        var newValue = item.ID;
        self.SetNhanVien(newValue);

        var idRandomHD = '';
        var itemOpen = [];
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD === null) {
            lstHD = [];
        }
        else {
            lstHD = JSON.parse(lstHD);
        }
        var itemEx = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);
        if (itemEx.length === 0) {
            // Creat newHoaDon if not exist and not HD TraHang
            // _maHoaDon === undefined when delete cacheMaHD
            if (_maHoaDon !== undefined && _maHoaDon.indexOf('Trả') === -1) {
                var newHD = newHoaDon(_maHoaDon);
                lstHD.push(newHD);
                idRandomHD = newHD.IDRandom;
                console.log('change_NVien ', _maHoaDon)
            }
        }
        else {
            idRandomHD = itemEx[0].IDRandom;
        }
        for (let i = 0; i < lstHD.length; i++) {
            if (lstHD[i].IDRandom === idRandomHD) {
                if (lstHD[i].ID_NhanVien !== newValue) {
                    lstHD[i].ID_NhanVien = newValue;
                    itemOpen = lstHD[i];
                }
                break;
            }
        }
        localStorage.setItem(lcListHD, JSON.stringify(lstHD));

        if (itemOpen.StatusOffline === false) {
            BindLstBangGia_byNhanVien_andDoiTuong();
            // check exist ID_BangGia of HD in lst BangGia new
            let itemGB = $.grep(self.GiaBans(), function (item) {
                return item.ID === itemOpen.ID_BangGia;
            });
            if (itemGB.length === 0) {
                // if not exist: reset to BGChuan
                self.ChangeBangGia({ ID: const_GuidEmpty, TenGiaBan: 'Bảng giá chuẩn' });
            }
        }
    }

    self.filterGiaBan = ko.observable('');
    self.filterNVien = ko.observable('');

    self.arrFilterGiaBan = ko.computed(function () {
        var _filter = self.filterGiaBan();
        return arrFilter = ko.utils.arrayFilter(self.GiaBans(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenGiaBan).split(/\s+/);
            var sSearch = '';
            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }
            if (chon && _filter) {
                chon = (locdau(prod.TenGiaBan).indexOf(locdau(_filter)) > -1 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) > -1
                );
            }
            return chon;
        });
    });
    self.arrFilterNhanVien = ko.computed(function () {
        var _filter = self.filterNVien();
        return arrFilter = ko.utils.arrayFilter(self.NhanViens(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenNhanVien).split(/\s+/);
            var sSearch = '';
            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }
            if (chon && _filter) {
                chon = (locdau(prod.TenNhanVien).indexOf(locdau(_filter)) > -1 ||
                    sSearch.indexOf(locdau(_filter)) > -1
                );
            }
            return chon;
        });
    });

    function UpdateGiaBanHangHoa_inList(lst) {
        // get giaban with banggia is chosing
        var arrGB = [];
        var idBangGia = self.selectedGiaBan();
        if (idBangGia !== null && idBangGia !== const_GuidEmpty) {
            arrGB = $.grep(self.AllBangGia(), function (item) {
                return item.ID === idBangGia;
            });
            if (arrGB.length > 0) {
                for (let i = 0; i < lst.length; i++) {
                    let itemGB = $.grep(arrGB[0].GiaBanChiTiet, function (x) {
                        return x.ID_DonViQuiDoi === lst[i].ID_DonViQuiDoi;
                    });
                    if (itemGB.length === 0) {
                        itemGB = $.grep(self.HangHoaOlds(), function (x) {
                            return x.ID_DonViQuiDoi === lst[i].ID_DonViQuiDoi && x.ID_LoHang === lst[i].ID_LoHang;
                        });
                    }
                    lst[i].GiaBan = itemGB.length > 0 ? itemGB[0].GiaBan : 0;
                }
            }
        }
        if (arrGB.length === 0) {
            for (let i = 0; i < lst.length; i++) {
                let itemHH = $.grep(self.HangHoaOlds(), function (x) {
                    return x.ID_DonViQuiDoi === lst[i].ID_DonViQuiDoi && x.ID_LoHang === lst[i].ID_LoHang;
                });
                lst[i].GiaBan = itemHH.length > 0 ? itemHH[0].GiaBan : 0;
            }
        }
        return lst;
    }

    function UpdateGiaBan_inListHangHoa_byPage(idBangGia) {
        var from = self.currentPageProducts() * self.PageSizeProduct();
        var to = from + self.PageSizeProduct();
        if (to > self.HangHoas().length) {
            to = self.HangHoas().length;
        }
        // get giaban with banggia is chosing
        var arrGB = [];
        if (idBangGia !== null && idBangGia !== const_GuidEmpty && idBangGia !== undefined) {
            arrGB = $.grep(self.AllBangGia(), function (item) {
                return item.ID === idBangGia;
            });
            if (arrGB.length > 0) {
                for (let i = from; i < to; i++) {
                    let itemGB = $.grep(arrGB[0].GiaBanChiTiet, function (x) {
                        return x.ID_DonViQuiDoi === self.HangHoas()[i].ID_DonViQuiDoi;
                    });
                    if (itemGB.length === 0) {
                        itemGB = $.grep(self.HangHoaOlds(), function (x) {
                            return x.ID_DonViQuiDoi === self.HangHoas()[i].ID_DonViQuiDoi && x.ID_LoHang === self.HangHoas()[i].ID_LoHang;
                        });
                    }
                    self.HangHoas()[i].GiaBan = itemGB.length > 0 ? itemGB[0].GiaBan : 0;
                }
            }
        }
        if (arrGB.length === 0) {
            for (let i = from; i < to; i++) {
                let itemHH = $.grep(self.HangHoaOlds(), function (x) {
                    return x.ID_DonViQuiDoi === self.HangHoas()[i].ID_DonViQuiDoi && x.ID_LoHang === self.HangHoas()[i].ID_LoHang;
                });
                self.HangHoas()[i].GiaBan = itemHH.length > 0 ? itemHH[0].GiaBan : 0;
            }
        }
        BindListHangHoa_ByPage();
    }

    function BindListHangHoa_ByPage() {
        var first = self.currentPageProducts() * self.PageSizeProduct();
        var lst = self.HangHoas().slice(first, first + self.PageSizeProduct());
        self.PageResults($.extend(true, [], lst));
    }

    self.ChangeBangGia = function (item) {
        $('#btnBangGia').text(item.TenGiaBan);
        var newValue = item.ID;
        self.SetBangGia(newValue);
        UpdateGiaBan_inListHangHoa_byPage(newValue);
        console.log('self.ChangeBangGi', newValue)

        var isChangeBG = false;
        var itemHD = [];
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            let idRandomHD = '';
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].ID_ViTri === _phongBanID && lstHD[i].MaHoaDon == _maHoaDon
                    && lstHD[i].ID_DonVi == _idDonVi) {
                    if (lstHD[i].ID_BangGia !== newValue && lstHD[i].StatusOffline === false) {
                        lstHD[i].ID_BangGia = newValue;
                        idRandomHD = lstHD[i].IDRandom;
                        isChangeBG = true;
                        break;
                    }
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));

            if (isChangeBG) {
                UpdateGiaBan_inCTHD(idRandomHD);
                UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
                Update_TienThue_CTHD(idRandomHD);

                AssignValue_ofHoaDon(idRandomHD);
                BindCTHD_byIDRandomHD(idRandomHD);
            }
        }
    }

    function BindCTHD_byIDRandomHD(idRandomHD) {
        var arr = [];
        var arrDT = [];

        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            arr = $.grep(cthd, function (x) {
                return x.IDRandomHD === idRandomHD;
            });
            self.HangHoaAfterAdds(arr);
        }
        else {
            self.HangHoaAfterAdds([]);
        }
    }

    function UpdateGiaBan_inCTHD(idRandomHD) {
        var sum = 0;
        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let j = 0; j < cthd.length; j++) {
                if (cthd[j].IDRandomHD === idRandomHD) {
                    let product = $.grep(self.HangHoaAll(), function (x) {
                        return x.ID_DonViQuiDoi === cthd[j].ID_DonViQuiDoi;
                    });
                    if (product.length > 0) {
                        cthd[j].PTChietKhau = 0;
                        cthd[j].TienChietKhau = 0;
                        cthd[j].DVTinhGiam = '%';
                        cthd[j].GiaBan = product[0].GiaBan;
                        cthd[j].DonGia = product[0].GiaBan;
                        cthd[j].ThanhTien = Math.round(product[0].GiaBan * cthd[j].SoLuong);
                        sum += cthd[j].ThanhTien;
                    }
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

            UpdateHD_whenChangeCTHD(sum, idRandomHD);
        }
    }

    self.editNote = function () {
        var lcNewHoaDon = localStorage.getItem(lcListHD);
        if (lcNewHoaDon !== null) {
            lcNewHoaDon = JSON.parse(lcNewHoaDon);
            // update note HD
            for (let i = 0; i < lcNewHoaDon.length; i++) {
                if (lcNewHoaDon[i].ID_ViTri === _phongBanID && lcNewHoaDon[i].MaHoaDon === _maHoaDon
                    && lcNewHoaDon[i].ID_DonVi === _idDonVi) {
                    lcNewHoaDon[i].DienGiai = $('#txtNote').val();
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lcNewHoaDon));
        }
    };

    function SumSoKhach_inTable(phongbanID) {
        var lstPB = self.AllPhongBans();
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            for (let i = 0; i < self.PhongBans().length; i++) {
                if (self.PhongBans()[i].ID === phongbanID) {
                    let arr = $.grep(lstHD, function (x) {
                        return x.ID_ViTri === phongbanID && x.StatusOffline === false && x.ID_DonVi === _idDonVi;
                    });
                    let sum = 0;
                    for (let i = 0; i < arr.length; i++) {
                        sum += arr[i].SoLuongKhachHang;
                    }
                    self.PhongBans()[i].SoLuongKhachHang = sum;
                    break;
                }
            }
            self.PhongBans.refresh();
        }
    }

    self.editNumberCustomer = function () {
        var lcNewHoaDon = localStorage.getItem(lcListHD);
        if (lcNewHoaDon !== null) {
            lcNewHoaDon = JSON.parse(lcNewHoaDon);
            // update note HD
            for (let i = 0; i < lcNewHoaDon.length; i++) {
                if (lcNewHoaDon[i].ID_ViTri === _phongBanID && lcNewHoaDon[i].MaHoaDon === _maHoaDon
                    && lcNewHoaDon[i].ID_DonVi === _idDonVi) {
                    lcNewHoaDon[i].SoLuongKhachHang = formatNumberToFloat($('#sokhacden').val());
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lcNewHoaDon));
            SumSoKhach_inTable(_phongBanID);
        }
    };

    self.showPopupThanhToan = function () {
        $('#cart').modal('show');
    }

    var initLoad = false;
    $(document).ready(function () {
        initLoad = true;
    })
    self.GetLst_TheKH = function (value) {
        if (initLoad) {
            $('#trHide').css('display', '');

            switch (value) {
                case 0:
                    $('#lblType,#lblThe').html('Tiền mặt');
                    $('.selectThe').css('display', 'none');
                    break;
                case 1:
                    $('#lblType, #lblThe').html('Thẻ');
                    $('.selectThe').css('display', '');
                    break;
                case 2:
                    $('#lblType,#lblThe').html('Chuyển khoản');
                    $('.selectThe').css('display', '');
                    break;
            }
        }
    }

    self.ThongBaoNhaBep = function () {
        //UpdateCache_ThongBaoNB();
    };

    function UpdateCache_ThongBaoNB() {
        // update Cache CTHD with Satus = 1;
        _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();

        var listAllCTHD = localStorage.getItem(lcListCTHD);
        if (listAllCTHD !== null) {
            listAllCTHD = JSON.parse(listAllCTHD);
            for (let i = 0; i < listAllCTHD.length; i++) {
                if (listAllCTHD[i].MaHoaDon === _maHoaDon) {
                    listAllCTHD[i].Status = 1;
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(listAllCTHD));
            ShowMessage_Success('Cập nhật hóa đơn thành công');
            console.log('PB ' + JSON.stringify(listAllCTHD));
        }
    }

    self.arrFilterPB = function (item, inputString) {
        var itemSearch = locdau(item.TenViTri).toLowerCase();
        var locdauInput = locdau(inputString).toLowerCase();
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (let i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }

        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    };

    function CountTableUsed(isClickImg) {
        var lstHD = localStorage.getItem(lcListHD);
        var arrIDVT_Ex = [];
        var vtNotEx = [];
        var arrIDVT_HD = [];

        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            // get arrID_VT in cacheHD
            var hdHasVT = $.grep(lstHD, function (x) {
                return x.ID_DonVi === _idDonVi && x.ID_ViTri !== null;
            });
            arrIDVT_HD = $.map(hdHasVT, function (x) {
                return x.ID_ViTri;
            });

            // get arrIDVT exist in lstPB
            var arrVTDB = [];
            arrVTDB = $.map(self.AllPhongBans(), function (x) {
                return x.ID;
            });
            // get idVitri exist in lstHD but not exist in DB
            vtNotEx = $.grep(arrIDVT_HD, function (x) {
                return $.inArray(x, arrVTDB) === -1;
            });
            // delete hd if idVitri not exist DB
            lstHD = $.grep(lstHD, function (x) {
                return $.inArray(x.ID_ViTri, vtNotEx) === -1;
            });

            // get arrIDVT after delete
            arrIDVT_Ex = $.map(lstHD, function (x) {
                return x.ID_ViTri;
            });
            arrIDVT_Ex = $.unique(arrIDVT_Ex.sort());
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
        }
        else {
            lstHD = [];
        }

        // check phong was delete --> set new idPhong
        if ($.inArray(_phongBanID, vtNotEx) > -1) {
            if (lstHD.length > 0) {
                _phongBanID = lstHD[0].ID_ViTri;
            }
            else {
                _phongBanID = self.AllPhongBans()[0].ID;
            }
        }
        else {
            if (_phongBanID === null) {
                _phongBanID = self.AllPhongBans()[0].ID;
            }
        }
        if (isClickImg) {
            arrIDVT_Ex.push(_phongBanID);
        }
        else {
            // nếu chọn phòng, nhưng chưa có CTHD (tức chưa tồn tại trong DS hóa đơn)
            if (arrIDVT_HD.length > 0 && $.inArray(_phongBanID, arrIDVT_HD) === -1) {
                _phongBanID = arrIDVT_HD[arrIDVT_HD.length - 1];
            }
        }
        if (arrIDVT_Ex.length === 0) {
            arrIDVT_Ex = [_phongBanID];
        }
        localStorage.setItem(lcPhongBanID, _phongBanID);

        var arrPBUsed = $.grep(self.AllPhongBans(), function (x) {
            return $.inArray(x.ID, arrIDVT_Ex) > -1;
        });
        self.PhongBanUsed(arrPBUsed);
        self.selectedPhongBan(_phongBanID);

        // count empty table by idkhuvuc
        var allPB = self.AllPhongBans();
        var tblBusy = arrIDVT_Ex.length;
        if (self.selectGroupkhuVuc() !== undefined) {
            allPB = $.grep(self.AllPhongBans(), function (x) {
                return x.ID_KhuVuc === self.selectGroupkhuVuc();
            });

            // get table busy in khuvuc
            tblBusy = ($.grep(allPB, function (x) {
                return $.inArray(x.ID, arrIDVT_Ex) > -1;
            })).length;
        }
        self.roomBusy(tblBusy);
        self.roomEmpty(allPB.length - tblBusy);
    }

    self.selectedStatus.subscribe(function (newValue) {
        self.currentPage(0);
    });

    self.filterNhanVien = function (item, inputString) {
        var itemSearch = locdau(item.TenNhanVien).toLowerCase();
        var itemSearch2 = locdau(item.MaNhanVien).toLowerCase();
        var locdauInput = locdau(inputString).toLowerCase();
        // nen bat chat khong cho nhap dau cach o MaKH
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';

        for (let i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 || itemSearch2.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
    }

    function UpdateKH_inforBasic_IndexDB(objUpdate) {
        db.DM_DoiTuong.put(objUpdate);
    }

    self.clickVND = function (item) {
        var idRandom = item.IDRandom;
        var $this = $('#vnd_' + idRandom);
        var objSale = $('#pri-g_' + idRandom);
        $(function () {
            objSale.focus();
        });

        $this.next(".plus").removeClass("gb");
        $this.prev(".plus").removeClass("gb");
        $this.addClass("gb");

        var ctDoing = FindCTHD_isDoing(idRandom);
        if (ctDoing !== null) {
            let tiengiam = ctDoing.TienChietKhau;
            let ptGiam = ctDoing.PTChietKhau;
            let priceOld = ctDoing.DonGia;
            let dvtGiam = ctDoing.DVTinhGiam;

            // neu old = vnd
            if (ptGiam !== 0) {
                dvtGiam = 'VND';
                ptGiam = 0;
                objSale.val(formatNumber(tiengiam));
            }

            // update ptgiam
            let cthd = localStorage.getItem(lcListCTHD);
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === idRandom) {
                    cthd[i].PTChietKhau = ptGiam;
                    cthd[i].DVTinhGiam = 'VND';
                    break;
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

            // bind data
            $('#dvtGiam_' + idRandom).text('VNĐ');
            $('#lblGiamGia_' + idRandom).text(objSale.val());
        }
    }

    self.clickNoVND = function (item) {
        var idRandom = item.IDRandom;
        var $this = $('#novnd_' + idRandom);
        var objSale = $('#pri-g_' + idRandom);
        $this.next(".plus").removeClass("gb");
        $this.prev(".plus").removeClass("gb");
        $this.addClass("gb");
        $(function () {
            objSale.focus();
        });

        var ctDoing = FindCTHD_isDoing(idRandom);
        if (ctDoing !== null) {
            let tiengiam = ctDoing.TienChietKhau;
            let ptGiam = ctDoing.PTChietKhau;
            let priceOld = ctDoing.DonGia;
            let dvtGiam = ctDoing.DVTinhGiam;

            // neu old = vnd
            if (ptGiam === 0) {
                ptGiam = RoundDecimal(tiengiam / priceOld * 100);
                $(objSale).val(ptGiam);
            }

            // update ptgiam
            let cthd = localStorage.getItem(lcListCTHD);
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === idRandom) {
                    cthd[i].PTChietKhau = ptGiam;
                    cthd[i].DVTinhGiam = '%';
                    break;
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

            // bind data
            $('#dvtGiam_' + idRandom).text('%');
            $('#lblGiamGia_' + idRandom).text(objSale.val());
        }
    }

    function FindCTHD_isDoing(idRandom) {
        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandom === idRandom) {
                    return cthd[i];
                }
            }
        }
        return null;
    }

    function Focus_InputTienTraHD() {
        $('#txtTienKhachDua').focus();
    }

    function Enter_CTHD(itemCT, e, charStart) {
        var key = e.keyCode || e.which;

        if (key === 13 && self.windowWidth() > 1023) {
            var cthd = JSON.parse(localStorage.getItem(lcListCTHD));
            var idRandomFocus = null;
            var lenCTHD = cthd.length;
            for (let i = 0; i < lenCTHD; i++) {
                if (cthd[i].IDRandom === itemCT.IDRandom) {
                    // find li next
                    if (i < cthd.length - 1) {
                        idRandomFocus = cthd[i + 1].IDRandom;
                    }
                    else {
                        Focus_InputTienTraHD();
                        return false;
                    }
                    break;
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
            if (idRandomFocus !== null) {
                $('#' + charStart + idRandomFocus).focus().select();
            }
        }
    }

    self.editSoluong = function (item) {
        var $this = $(event.currentTarget);
        formatNumberObj($this);
        var idRandom = item.IDRandom;

        var ctDoing = FindCTHD_isDoing(idRandom);
        if (ctDoing !== null) {
            var objThanhTien = $('#sum-f_' + idRandom);
            var idRandomHD = item.IDRandomHD;
            var newNumber = formatNumberToFloat($this.val());
            var sluongOld = newNumber;
            var keyCode = event.keyCode || event.which;

            if (keyCode === 38) {
                newNumber = sluongOld + 1;
                $this.val(formatNumber(newNumber));
            }
            // press down
            if (keyCode === 40) {
                if (sluongOld > 1) {
                    newNumber = sluongOld - 1;
                }
                $this.val(formatNumber(newNumber));
            }

            // update thanhtien, chietkhau
            let thanhtien = newNumber * ctDoing.GiaBan;
            var cthd = localStorage.getItem(lcListCTHD);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].IDRandomHD === idRandomHD && cthd[i].IDRandom === idRandom) {
                        cthd[i].SoLuong = newNumber;
                        if (cthd[i].PTChietKhau > 0) {
                            cthd[i].TienChietKhau = parseInt(cthd[i].PTChietKhau * thanhtien / 100);
                        }
                        cthd[i].ThanhTien = thanhtien;
                        break;
                    }
                }
                localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
            }

            UpdateWarning_CTHD(idRandomHD);
            var sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
            UpdateHD_whenChangeCTHD(sum, idRandomHD);
            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
            Update_TienThue_CTHD(idRandomHD);
            AssignValue_ofHoaDon(idRandomHD);

            objThanhTien.html(formatNumber3Digit(thanhtien));
            // show warning
            if (self.ThietLap().XuatAm === false) {
                if (ctDoing.LaHangHoa && newNumber > ctDoing.TonKho) {
                    $('#show_wr_' + idRandom).css('display', '');
                }
                else {
                    $('#show_wr_' + idRandom).css('display', 'none');
                }
            }
            else {
                $('#show_wr_' + idRandom).css('display', 'none');
            }
        }

        Enter_CTHD(ctDoing, event, 'munber_');
    }

    self.editPrice = function (item) {
        var obj = $(event.currentTarget);
        formatNumberObj(obj);
        var priceNew = formatNumberToFloat(obj.val());
        var idRandomHD = item.IDRandomHD;
        var idRandom = item.IDRandom;

        var ctDoing = FindCTHD_isDoing(idRandom);
        if (ctDoing !== null) {
            let priceOld = ctDoing.DonGia;
            let tiengiam = ctDoing.TienChietKhau;
            let ptGiam = ctDoing.PTChietKhau;
            let dvt = ctDoing.DVTinhGiam;

            if (priceNew < priceOld) {
                tiengiam = priceOld - priceNew;
                if (dvt === '%') {
                    ptGiam = RoundDecimal(tiengiam / priceOld * 100);
                    $('#lblGiamGia_' + idRandom).text(ptGiam);
                }
                else {
                    $('#lblGiamGia_' + idRandom).text(formatNumber(tiengiam));
                    $('#dvtGiam_' + idRandom).text('VND');
                    dvt = 'VND';
                }
            }
            else {
                tiengiam = 0;
                ptGiam = 0;
                $('#lblGiamGia_' + idRandom).text(0);
            }

            let thanhtien = priceNew * ctDoing.SoLuong;
            var cthd = localStorage.getItem(lcListCTHD);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].IDRandom === idRandom) {
                        cthd[i].PTChietKhau = ptGiam;
                        cthd[i].TienChietKhau = tiengiam;
                        cthd[i].GiaBan = priceNew;
                        cthd[i].ThanhTien = thanhtien;
                        break;
                    }
                }
                localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
            }

            var sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
            UpdateHD_whenChangeCTHD(sum, idRandomHD);
            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
            Update_TienThue_CTHD(idRandomHD);
            AssignValue_ofHoaDon(idRandomHD);

            // bind data
            $('#sum-f_' + idRandom).html(formatNumber(thanhtien));
            $('button[id=' + idRandom + ']').text(formatNumber(priceNew));
            $('#pri-g_' + idRandom).val($('#lblGiamGia_' + idRandom).text());

            if (tiengiam > 0) {
                $('#gg_' + idRandom).css('display', '');
            }
            else {
                $('#gg_' + idRandom).css('display', 'none');
            }
        }
    }

    self.editSale = function (item) {
        var idRandomHD = item.IDRandomHD;
        var idRandom = item.IDRandom;
        var obj = $('#pri-g_' + idRandom);

        var ctDoing = FindCTHD_isDoing(idRandom);
        if (ctDoing !== null) {
            let priceOld = ctDoing.DonGia;
            let tiengiam = ctDoing.TienChietKhau;
            let ptGiam = ctDoing.PTChietKhau;
            let dvt = ctDoing.DVTinhGiam;
            var giamNew = 0;

            // neu gia cu = 0 => khong cho phep nhap giam gia
            if (priceOld === 0) {
                obj.val(0);
            }
            else {
                formatNumberObj(obj);
                giamNew = formatNumberToFloat(obj.val());

                if (dvt === 'VND') {
                    // nhap giamgia > giaban
                    if (giamNew > priceOld) {
                        giamNew = priceOld;
                        tiengiam = priceOld;
                        $(obj).val(formatNumber(priceOld));
                    }
                    else {
                        tiengiam = giamNew;
                    }
                }
                else {
                    if (giamNew > 100) {
                        giamNew = 100;
                        ptGiam = 100;
                        tiengiam = priceOld;
                        $(obj).val(100);
                    }
                    else {
                        ptGiam = giamNew;
                        tiengiam = Math.round(giamNew * priceOld / 100);
                    }
                }

                // update giamgia, dongia, thanhtien
                let priceNew = Math.round(priceOld - tiengiam);
                let thanhtien = Math.round(priceNew * ctDoing.SoLuong);
                var cthd = localStorage.getItem(lcListCTHD);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    for (let i = 0; i < cthd.length; i++) {
                        if (cthd[i].IDRandom === idRandom) {
                            cthd[i].PTChietKhau = ptGiam;
                            cthd[i].TienChietKhau = tiengiam;
                            cthd[i].GiaBan = priceNew;
                            cthd[i].ThanhTien = thanhtien;
                            break;
                        }
                    }
                    localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
                }

                // bind data todo
                $('button[id=' + idRandom + ']').text(formatNumber(priceNew));
                $('input[id=' + idRandom + ']').val(formatNumber(priceNew));
                $('#lblGiamGia_' + idRandom).text(formatNumber(formatNumber(giamNew)));
                $('#sum-f_' + idRandom).html(formatNumber(thanhtien));
                if (tiengiam > 0) {
                    $('#gg_' + idRandom).css('display', '');
                }
                else {
                    $('#gg_' + idRandom).css('display', 'none');
                }
            }

            var sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
            UpdateHD_whenChangeCTHD(sum, idRandomHD);
            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
            Update_TienThue_CTHD(idRandomHD);
            AssignValue_ofHoaDon(idRandomHD);
        }
    }

    self.clickNoVNDTax = function () {
        var $this = $('.senter2');
        $this.next(".plus").removeClass("gb");
        $this.prev(".plus").removeClass("gb");
        $this.addClass("gb");

        var objTongtien = $('.sum-price').html();
        var tongtien = formatNumberToInt(objTongtien);
        var ipTax = $('#txtTax');
        var ipGiamOld = ipTax.val();
        if (ipGiamOld === '') {
            ipGiamOld = '0';
        }
        if (!clickVNDTax) {
            ipTax.val(RoundDecimal(ipGiamOld));
        }
        else {
            var div = formatNumberToInt(ipGiamOld) / tongtien * 100;
            ipTax.val(RoundDecimal(div));
        };

        clickVNDTax = false;
        var lcHD = localStorage.getItem(lcListHD);
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            for (let i = 0; i < lcHD.length; i++) {
                if (lcHD[i].MaHoaDon === _maHoaDon && lcHD[i].ID_ViTri === _phongBanID
                    && lcHD[i].ID_DonVi === _idDonVi) {
                    lcHD[i].PTThue = ipTax.val();
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lcHD));
        }

        $('#ptTax').text(ipTax.val()); // ptThue
        $('#dvtTax').text('%');
    }

    self.clickVNDTax = function () {
        var $this = $('.vnd2');
        $this.next(".plus").removeClass("gb");
        $this.prev(".plus").removeClass("gb");
        $this.addClass("gb");

        var objTongtien = $('.sum-price').html();
        var tongtien = formatNumberToInt(objTongtien);
        var ipTax = $('#txtTax');
        var tongThue = 0;
        var ipGiamOld = ipTax.val();
        if (ipGiamOld === '') {
            ipGiamOld = '0';
        }

        // if click again VND
        if (clickVNDTax) {
            tongThue = formatNumberToInt(ipGiamOld);
        }
        else {
            tongThue = Math.round(parseFloat(ipGiamOld) * tongtien / 100);
        }

        clickVNDTax = true;
        // update ptGiam, DVTGiam, tongThueGia in cache HoaDon
        var lcHD = localStorage.getItem(lcListHD);
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            for (let i = 0; i < lcHD.length; i++) {
                if (lcHD[i].MaHoaDon === _maHoaDon && lcHD[i].ID_ViTri === _phongBanID
                    && lcHD[i].ID_DonVi === _idDonVi) {
                    lcHD[i].PTThue = 0;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lcHD));
        }
        $('#dvtTax').text('VNĐ');
        $('#ptTax').text(0);
        ipTax.val(formatNumber(tongThue));
    }

    self.editTax = function () {
        var ipTax = $('#txtTax');
        var objTongtien = $('.sum-price').html();
        var tongtien = formatNumberToInt(objTongtien);
        var tonggiamHD = formatNumberToInt($('.parent-price-1').text());
        var objDVTGiam = $('#dvtTax');

        var cantra = 0;
        var tongThue = 0;
        var ptThue = 0;

        if ($('.vnd2').hasClass('gb')) {

            if (ipTax.val() === '') {
                $('#ptTax').text(0);
            }
            else {
                // format 000,000,000
                formatNumberObj(ipTax);

                // neu giamgia > tongtien
                if (formatNumberToInt(ipTax.val()) > tongtien) {
                    ipTax.val(formatNumber(tongtien));
                }
                tongThue = formatNumberToInt(ipTax.val());
                ptThue = 0;
                //$('#ptTax').text(ipTax.val());
            }
            objDVTGiam.text('VNĐ');
        }
        else {
            if (ipTax.val() === '') {
                $('#ptTax').text(0);
            }
            else {
                // neu giam gia > 100 %
                if (formatNumberToFloat(ipTax.val()) > 100) {
                    ipTax.val(100);
                }
                tongThue = parseInt(parseFloat(ipTax.val()) * (tongtien - tonggiamHD) / 100);
                $('#ptTax').text(ipTax.val());
                ptThue = ipTax.val();
            }
            objDVTGiam.text('%');
        }

        var idRandom = '';
        var lcHD = localStorage.getItem(lcListHD);
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            for (let i = 0; i < lcHD.length; i++) {
                if (lcHD[i].MaHoaDon === _maHoaDon && lcHD[i].ID_ViTri === _phongBanID
                    && lcHD[i].ID_DonVi === _idDonVi) {
                    lcHD[i].PTThue = ptThue;
                    lcHD[i].TongTienThue = tongThue;
                    idRandom = lcHD[i].IDRandom;
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lcHD));
        }

        UpdateHD_whenChangeHD(idRandom);
        UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandom);
        Update_TienThue_CTHD(idRandom);
        AssignValue_ofHoaDon(idRandom);

        if (event.keyCode === 13) {
            $('.arrow-left3').hide();
        }
    }

    self.editSalePr = function () {
        var ipGiamGia = $('.price-sa');
        var objTongtien = $('.sum-price').html();
        var tongtien = formatNumberToInt(objTongtien);
        var objDVTGiam = $('#dvtGiamParent');
        var thue = $('.parent-tax').text();

        if (thue === '') {
            thue = 0;
        }
        else {
            thue = formatNumberToInt(thue);
        };
        var cantra = 0;
        var tongGiam = 0;

        if ($('.vnd1').hasClass('gb')) {

            if (ipGiamGia.val() === '') {
                $('#ptGiam').text(0);
            }
            else {
                // format 000,000,000
                formatNumberObj(ipGiamGia);

                // neu giamgia > tongtien
                if (formatNumberToInt(ipGiamGia.val()) > tongtien) {
                    ipGiamGia.val(formatNumber(tongtien));
                }
                tongGiam = formatNumberToInt(ipGiamGia.val());

                $('#ptGiam').text(ipGiamGia.val());
            }
            objDVTGiam.text('VNĐ');
        }
        else {
            if (ipGiamGia.val() === '') {
                $('#ptGiam').text(0);
            }
            else {
                // neu giam gia > 100 %
                if (formatNumberToFloat(ipGiamGia.val()) > 100) {
                    ipGiamGia.val(100);
                }
                tongGiam = Math.round(parseFloat(ipGiamGia.val()) * tongtien / 100);
                $('#ptGiam').text(ipGiamGia.val());
            }
            objDVTGiam.text('%');
        }

        // update GiamGia for HD
        var idRandom = '';
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].MaHoaDon === _maHoaDon && lstHD[i].ID_ViTri === _phongBanID
                    && lstHD[i].ID_DonVi === _idDonVi) {
                    lstHD[i].TongGiamGia = tongGiam;
                    if (objDVTGiam.text().indexOf('%') > -1) {
                        lstHD[i].TongChietKhau = formatNumberToFloat($('#ptGiam').text());
                    }
                    else {
                        lstHD[i].TongChietKhau = 0;
                    }
                    lstHD[i].DVTinhGiam = objDVTGiam.text();
                    idRandom = lstHD[i].IDRandom;
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
        }

        UpdateHD_whenChangeHD(idRandom);
        UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandom);
        Update_TienThue_CTHD(idRandom);
        AssignValue_ofHoaDon(idRandom);
    }

    self.clickVNDPr = function () {
        var $this = $('.vnd1');
        $this.next(".plus").removeClass("gb");
        $this.prev(".plus").removeClass("gb");
        $this.addClass("gb");

        var ipGiamGia = $('.price-sa');
        var objDVTGiam = $('#dvtGiamParent');
        var objTongtien = $('.sum-price').html();
        var tongtien = formatNumberToInt(objTongtien);
        var datraObj = formatNumberObj($('.price-pay-end'));
        var datra = formatNumberToInt(datraObj);
        var cantra = 0;
        var giam = 0;
        var ipGiamOld = ipGiamGia.val();
        if (ipGiamOld === '') {
            ipGiamOld = '0';
        }
        // if click again VND
        if (clickVNDpr) {
            giam = formatNumberToInt(ipGiamOld);
        }
        else {
            giam = Math.round(parseFloat(ipGiamOld) * tongtien / 100);
        }

        ipGiamGia.val(formatNumber(giam));
        objDVTGiam.text('VNĐ');
        clickVNDpr = true;

        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].MaHoaDon === _maHoaDon && lstHD[i].ID_ViTri === _phongBanID
                    && lstHD[i].ID_DonVi === _idDonVi) {
                    lstHD[i].DVTinhGiam = 'VND';
                    lstHD[i].TongChietKhau = 0;
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD))
        }
        $('#ptGiam').text(0);
        $('#dvtGiamParent').text('VND');
    }

    self.clickNoVNDPr = function () {
        var $this = $('.senter1');
        $this.next(".plus").removeClass("gb");
        $this.prev(".plus").removeClass("gb");
        $this.addClass("gb");

        var ipGiamGia = $('.price-sa');
        var objTongtien = $('.sum-price').html();
        var tongtien = formatNumberToInt(objTongtien);
        var cantra = 0;
        var ipGiamOld = ipGiamGia.val();
        if (ipGiamOld === '') {
            ipGiamOld = '0';
        }
        if (!clickVNDpr) {
            ipGiamGia.val(RoundDecimal(ipGiamOld));
        }
        else {
            var div = formatNumberToInt(ipGiamOld) / tongtien * 100;
            ipGiamGia.val(RoundDecimal(div));
        };
        clickVNDpr = false;

        var lcHD = localStorage.getItem(lcListHD);
        if (lcHD !== null) {
            lcHD = JSON.parse(lcHD);
            for (let i = 0; i < lcHD.length; i++) {
                if (lcHD[i].MaHoaDon === _maHoaDon && lcHD[i].ID_ViTri === _phongBanID
                    && lcHD[i].ID_DonVi === _idDonVi) {
                    lcHD[i].TongChietKhau = ipGiamGia.val();
                    lcHD[i].DVTinhGiam = '%';
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lcHD));
        }

        $('#ptGiam').text(ipGiamGia.val());
        $('#dvtGiamParent').text('%');
    }

    self.HD_EditPhiDV = function () {
        var obj = $('#txtcharge');
        var idRandom = self.HoaDons().IDRandom();
        var tongtienhang = self.HoaDons().TongTienHang();
        var dvt = self.HoaDons().DVTinhPhiDV();
        var tongChiPhi = self.HoaDons().TongChiPhi();
        var ptChiPhi = self.HoaDons().PTChiPhi();
        var chiphi = 0;

        if (tongtienhang === 0) {
            obj.val(0);
        }
        else {
            formatNumberObj(obj);
            chiphi = formatNumberToFloat(obj.val());

            if (dvt === 'VND') {
                // nhap giamgia > giaban
                if (chiphi > tongtienhang) {
                    chiphi = tongtienhang;
                    $(obj).val(formatNumber(chiphi));
                }

                ptChiPhi = 0;
                tongChiPhi = chiphi;
            }
            else {
                if (chiphi > 100) {
                    chiphi = 100;
                    ptChiPhi = 100;
                    tongChiPhi = tongtienhang;
                    $(obj).val(chiphi);
                }
                else {
                    ptChiPhi = chiphi;
                    tongChiPhi = Math.round(chiphi * tongtienhang / 100);
                }
            }

            var khachcantra = 0;
            // update giamgia, dongia, thanhtien
            var hd = localStorage.getItem(lcListHD);
            if (hd !== null) {
                hd = JSON.parse(hd);
                for (let i = 0; i < hd.length; i++) {
                    if (hd[i].IDRandom === idRandom) {
                        hd[i].PTChiPhi = ptChiPhi;
                        hd[i].TongChiPhi = tongChiPhi;
                        khachcantra = hd[i].TongTienHang + hd[i].TongTienThue - hd[i].TongGiamGiaKM_HD + tongChiPhi;
                        break;
                    }
                }
                localStorage.setItem(lcListHD, JSON.stringify(hd));
            }

            UpdateHD_whenChangeHD(idRandom);
            $('#tongChiPhi').text(formatNumber(tongChiPhi));
            $('#lblKhachCanTra').text(formatNumber(khachcantra));
            $('#txtTienKhachDua').val(formatNumber(khachcantra));
        }
    }

    self.PhiDV_ClickVND = function () {
        var idRandom = self.HoaDons().IDRandom();
        // update ptchiphi
        let hd = localStorage.getItem(lcListHD);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === idRandom) {
                    hd[i].PTChiPhi = 0;
                    hd[i].DVTinhPhiDV = 'VND';
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(hd));
        }
        AssignValue_ofHoaDon(idRandom);
    }

    self.PhiDV_ClickPercent = function () {
        var idRandom = self.HoaDons().IDRandom();
        var ptChiPhi = self.HoaDons().PTChiPhi();
        if (ptChiPhi === 0) {
            ptChiPhi = RoundDecimal(self.HoaDons().TongChiPhi() / self.HoaDons().TongTienHang() * 100);
        }

        // update ptgiam
        let hd = localStorage.getItem(lcListHD);
        if (hd !== null) {
            hd = JSON.parse(hd);
            for (let i = 0; i < hd.length; i++) {
                if (hd[i].IDRandom === idRandom) {
                    hd[i].PTChiPhi = ptChiPhi;
                    hd[i].DVTinhPhiDV = '%';
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(hd));
            AssignValue_ofHoaDon(idRandom);
        }
    }

    self.editDaTra = function () {
        var $this = formatNumberObj($('.price-pay-end'));
        var datra = formatNumberToInt($this);
        var cantra = formatNumberToInt($('.price-pay').text());
        var tienthua = datra - cantra;

        // set again infor HoaDon
        var itemHD = [];
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].MaHoaDon === _maHoaDon && lstHD[i].ID_ViTri === _phongBanID
                    && lstHD[i].ID_DonVi === _idDonVi) {
                    lstHD[i].DaThanhToan = datra;
                    lstHD[i].TienMat = datra;
                    lstHD[i].TienThua = tienthua;
                    itemHD = lstHD[i];
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));

            if (itemHD != []) {
                self.HoaDons().SetData(itemHD);
            }
        }
    }

    self.UpdateCTHD_GhiChu = function (item) {
        var lcCTHD = localStorage.getItem(lcListCTHD);
        var idViTri = item.ID_ViTri;
        if (lcCTHD !== null) {
            lcCTHD = JSON.parse(lcCTHD);
            // update note HD
            for (let i = 0; i < lcCTHD.length; i++) {

                if (lcCTHD[i].ID_ViTri === idViTri && lcCTHD[i].MaHoaDon === _maHoaDon
                    && lcCTHD[i].ID_DonVi === _idDonVi) {
                    if (lcCTHD[i].IDRandom === item.IDRandom) {
                        lcCTHD[i].GhiChu = $('#gc_' + item.IDRandom).val();
                    }
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(lcCTHD));

            Enable_BtnThongBaoNB();
        }
    }

    self.printDraft = function () {
        var idRandomHD = self.HoaDons().IDRandom();

        var hd = localStorage.getItem(lcListHD);
        if (hd !== null) {
            hd = JSON.parse(hd);
            let hdOp = $.grep(hd, function (x) {
                return x.IDRandom === idRandomHD;
            });
            if (hdOp.length > 0) {
                let cthd = localStorage.getItem(lcListCTHD);
                if (cthd !== null) {
                    cthd = JSON.parse(cthd);
                    let ctOp = $.grep(cthd, function (x) {
                        return x.IDRandomHD === idRandomHD;
                    });
                    if (ctOp.length > 0) {
                        ctOp = ctOp.sort(function (a, b) {
                            var x = a.SoThuTu, y = b.SoThuTu;
                            return x > y ? 1 : x < y ? -1 : 0
                        });

                        ctOp = AssignProperties_forCTHD_whenSave(ctOp);
                        let cthdPrint = GetCTHDPrint_Format(ctOp);
                        self.CTHoaDonPrint(cthdPrint);

                        let objHDPrint = GetInforHDPrint(hdOp[0]);
                        self.InforHDprintf(objHDPrint);

                        self.InHoaDon('HDBL', true);
                    }
                    else {
                        ShowMessage_Danger('Vui lòng nhập chi tiết tin hóa đơn');
                    }
                }
                else {
                    ShowMessage_Danger('Vui lòng nhập chi tiết tin hóa đơn');
                }
            }
        }
        else {
            ShowMessage_Danger('Vui lòng nhập thông tin hóa đơn');
        }
    }

    self.PageCountTbl = ko.computed(function () {
        var div;
        if (self.PhongBanUsed() != null) {
            div = Math.floor(self.PhongBanUsed().length / self.PageSizeTableUse());
            div += self.PhongBanUsed().length % self.PageSizeTableUse() > 0 ? 1 : 0;
        }
        return div - 1;
    });
    self.resultTableUsed = ko.computed(function () {
        var first = self.currTblUsed() * self.PageSizeTableUse();
        if (self.PhongBanUsed() !== null) {
            return self.PhongBanUsed.slice(first, first + self.PageSizeTableUse());
        }
        return null;
    });
    self.hasPrevTbl = ko.computed(function () {
        return self.currTblUsed() !== 0;
    });
    self.hasNextTbl = ko.computed(function () {
        return self.currTblUsed() !== self.PageCountTbl();
    });
    self.nextTableUsed = function () {
        if (self.currTblUsed() < self.PageCountTbl()) {
            self.currTblUsed(self.currTblUsed() + 1);
        }
        // set color tableUsed
        $('.bill-bxslide').find('li[id=room_' + _phongBanID + ']').addClass('using');
    }
    self.prevTableUsed = function () {
        if (self.currTblUsed() != 0) {
            self.currTblUsed(self.currTblUsed() - 1);
        }
        // set color tableUsed
        $('.bill-bxslide').find('li[id=room_' + _phongBanID + ']').addClass('using');
    }

    self.PageCountHD = ko.computed(function () {
        var div;
        if (self.PhongBanSelected() != null) {
            div = Math.floor(self.PhongBanSelected().length / self.pageSizeHD());
            div += self.PhongBanSelected().length % self.pageSizeHD() > 0 ? 1 : 0;
        }
        return div - 1;
    });

    function BindInforHD_byIndexHD() {
        var arr = GetHDOpening_ofTable();
        if (arr.length > 0) {
            let hdOpen = $.grep(arr, function (x, i) {
                return i === self.IndexHD_inTable();
            });
            if (hdOpen.length > 0) {
                console.log('hdOpen ', hdOpen)
                hdOpen = hdOpen[0];
            }
            else {
                hdOpen = arr[0];
            }

            _maHoaDon = hdOpen.MaHoaDon;
            self.PhongBanSelected(arr);
            self.SetNhanVien(hdOpen.ID_NhanVien);
            if (self.selectedGiaBan() !== hdOpen.ID_BangGia) {
                UpdateGiaBan_inListHangHoa_byPage(hdOpen.ID_BangGia);
            }
            self.SetBangGia(hdOpen.ID_BangGia);
            getChiTietNCCByID(hdOpen.ID_DoiTuong);
            self.HoaDons().SetData(hdOpen);
            BindCTHD_byIDRandomHD(hdOpen.IDRandom);
            SetText_lblTienMat(hdOpen, 1);
        }
        else {
            _maHoaDon = '1';
            var objEmpty = newHoaDon(_phongBanID, '1');
            self.PhongBanSelected([objEmpty]);
            self.resetInforHD_CTHD();
            $('#lblTienMat').text('(Tiền mặt)');
        }
        StyDSHoaDon_byMaHoaDon(_maHoaDon);
        Enable_DisableNgayLapHD();
    }

    self.listHoaDon_ofPB = ko.computed(function () {
        var first = self.IndexHD_inTable();
        return self.PhongBanSelected().slice(first, first + self.pageSizeHD());
    });
    self.hasPrevHD = ko.computed(function () {
        if (self.IndexHD_inTable() === 0) {
            return false;
        }
        else {
            if (self.PhongBanSelected() !== null) {
                let len = self.PhongBanSelected().length;
                if (len > self.pageSizeHD()) {
                    if (self.IndexHD_inTable() < len) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
                else {
                    return false;
                }
            }
            else {
                return false;
            }
        }
    });
    self.hasNextHD = ko.computed(function () {
        let len = self.PhongBanSelected().length;
        if (self.IndexHD_inTable() + self.pageSizeHD() < len) {
            return true;
        }
        else {
            return false;
        }
    });
    self.nextHoaDon_ofPB = function () {
        if (self.IndexHD_inTable() < self.PhongBanSelected().length) {
            self.IndexHD_inTable(self.IndexHD_inTable() + 1);
            BindInforHD_byIndexHD();
        }
    }
    self.prevHoaDon_ofPB = function () {
        if (self.IndexHD_inTable() !== 0) {
            self.IndexHD_inTable(self.IndexHD_inTable() - 1);
            BindInforHD_byIndexHD();
        }
    }

    // phan trang DS Nhom HH
    self.PageCountNhomHH = ko.computed(function () {
        var div;
        if (self.NhomHangHoas() != null) {
            div = Math.floor(self.NhomHangHoas().length / self.PageSizeProductGroup());
            div += self.NhomHangHoas().length % self.PageSizeProductGroup() > 0 ? 1 : 0;
        }
        return div - 1;
    });
    self.resultNhomHH = ko.computed(function () {
        var first = self.currNhomHH() * self.PageSizeProductGroup();
        if (self.NhomHangHoas() !== null) {
            return self.NhomHangHoas.slice(first, first + self.PageSizeProductGroup());
        }
        return null;
    });
    self.hasPrevNhomHH = ko.computed(function () {
        return self.currNhomHH() !== 0;
    });
    self.hasNextNhomHH = ko.computed(function () {
        return self.currNhomHH() !== self.PageCountNhomHH();
    });
    self.nextNhomHH = function () {
        if (self.currNhomHH() < self.PageCountNhomHH()) {
            self.currNhomHH(self.currNhomHH() + 1);
        }
    }
    self.prevNhomHH = function () {
        if (self.currNhomHH() !== 0) {
            self.currNhomHH(self.currNhomHH() - 1);
        }
    }

    function SendData_atThisPage(obj) {
        if (connected) {
            hub.server.sendData_NhaHang(obj);
        }
    }

    function ThongBaoBep_SendData(cthd, type) {
        if (connected) {
            //var obj = {
            //    MauIn: self.DM_MauIn(),
            //    CTHD: JSON.stringify(cthd),
            //    IDChiNhanh: _idDonVi,
            //    Func: type
            //}
            //hub.server.sendData_ThuNgan_ToNhaBep(obj);
            hub.server.send(JSON.stringify(cthd), _idDonVi)
        }
    }

    hub.client.receiveData_NhaHang = function (objSend) {
        var thisUser = _idUser;
        var thisChiNhanh = _idDonVi;
        var thisIDViTri = localStorage.getItem(lcPhongBanID);

        var loaiFunc = objSend.Func;
        var userSend = objSend.ID_User;
        var idViTriSend = objSend.ID_ViTri;
        var idChiNhanhSend = objSend.IDChiNhanh;
        var idRandomHDSend = objSend.IDRandomHD;

        if (idChiNhanhSend === thisChiNhanh) {
            switch (loaiFunc) {
                case 1:// chuyen ban, ghep HD, thong bao bep
                    var cthd_OtherClient = JSON.parse(objSend.CTHD);
                    var hd_OtherClient = JSON.parse(objSend.HD);

                    // get HD send with ID_Random
                    var itemHDSend = $.grep(hd_OtherClient, function (x) {
                        return x.IDRandom === idRandomHDSend;
                    })
                    // get HD send with ID_Random
                    var cthdPush = $.grep(cthd_OtherClient, function (x) {
                        return x.IDRandomHD === idRandomHDSend;
                    })

                    // check exist in client and remove/ push again
                    var lstHD = localStorage.getItem(lcListHD);
                    if (lstHD !== null) {
                        lstHD = JSON.parse(lstHD);

                        // remove HD with IDRandom
                        lstHD = $.grep(lstHD, function (x) {
                            return x.IDRandom !== idRandomHDSend;
                        });
                        // add again
                        lstHD.push(itemHDSend[0]);
                    }
                    else {
                        lstHD = itemHDSend;
                    }

                    var cthd = localStorage.getItem(lcListCTHD);
                    if (cthd !== null) {
                        cthd = JSON.parse(cthd);

                        // remove all CTHD with IDRandom
                        cthd = $.grep(cthd, function (x) {
                            return x.IDRandomHD !== idRandomHDSend;
                        });

                        // add again
                        for (let j = 0; j < cthdPush.length; j++) {
                            var itemIn = cthdPush[j];
                            if (itemIn.IDRandomHD === idRandomHDSend) {
                                cthd.push(itemIn);
                            }
                        }
                    }
                    else {
                        cthd = cthdPush;
                    }

                    localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
                    localStorage.setItem(lcListHD, JSON.stringify(lstHD));

                    // bind again client if same ID_ViTri
                    if (thisIDViTri === idViTriSend && thisUser !== userSend) {
                        //don't bind again (because other client doing other invoice

                    }
                    break;
                case 2: // closeHoaDon, after saveHoaDon, delete all HH
                    var lstHD = localStorage.getItem(lcListHD);
                    if (lstHD !== null) {
                        lstHD = JSON.parse(lstHD);

                        lstHD = $.grep(lstHD, function (x) {
                            return x.IDRandom !== idRandomHDSend;
                        });

                        var cthd = localStorage.getItem(lcListCTHD);
                        if (cthd !== null) {
                            cthd = JSON.parse(cthd);

                            cthd = $.grep(cthd, function (x) {
                                return x.IDRandomHD !== idRandomHDSend;
                            });
                            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
                        }
                        localStorage.setItem(lcListHD, JSON.stringify(lstHD));

                        // bind again if other client
                        if (thisIDViTri === idViTriSend && thisUser !== userSend) {
                            //var arrHDBind = GetListHD_NotOffline_byUser_andViTri(thisIDViTri, lstHD);
                            //self.PhongBanSelected(arrHDBind);
                            //if (arrHDBind.length > 0) {
                            //    var arrCTBind = $.grep(cthd, function (x) {
                            //        return x.IDRandomHD === arrHDBind[0].IDRandom;
                            //    })
                            //    self.HangHoaAfterAdds(arrCTBind);
                            //}
                        }
                    }
                    break;
                case 3: // remove product
                    var idQuiDoiSend = objSend.ID_DonViQuiDoi;

                    var hdSend = JSON.parse(objSend.HD);
                    var itemHDSend = $.grep(hdSend, function (x) {
                        return x.IDRandom === idRandomHDSend;
                    });

                    if (itemHDSend.length > 0) {
                        // find HD and update again
                        var lstHD = localStorage.getItem(lcListHD);
                        if (lstHD !== null) {
                            lstHD = JSON.parse(lstHD);
                            for (let i = 0; i < lstHD.length; i++) {
                                if (lstHD[i].IDRandom === idRandomHDSend) {
                                    lstHD[i] = itemHDSend[0];
                                }
                            }
                        }
                        localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                    }

                    var cthd = localStorage.getItem(lcListCTHD);
                    if (cthd !== null) {
                        cthd = JSON.parse(cthd);

                        // remove CTHD
                        cthd = $.grep(cthd, function (x) {
                            return x.IDRandomHD !== idRandomHDSend || (x.IDRandomHD === idRandomHDSend && x.ID_DonViQuiDoi !== idQuiDoiSend);
                        })

                        localStorage.setItem(lcListCTHD, JSON.stringify(cthd));
                    }

                    // remove CTDoiTra
                    var cthdDT = localStorage.getItem(lcListCTHD_DoiTra);
                    if (cthdDT !== null && cthdDT !== 'null') {
                        cthdDT = JSON.parse(cthdDT);

                        cthdDT = $.grep(cthdDT, function (x) {
                            return x.IDRandomHD !== idRandomHDSend || (x.IDRandomHD === idRandomHDSend && x.ID_DonViQuiDoi !== idQuiDoiSend);
                        });

                        localStorage.setItem(lcListCTHD_DoiTra, JSON.stringify(cthdDT));
                    }
                    break;

                case 4: // dong bo hoa: send all ID_HoaDon and delete
                    var ID_HoaDons = objSend.ID_HoaDons;

                    var hdClient = localStorage.getItem(lcListHD);
                    var cthdClient = localStorage.getItem(lcListCTHD);

                    if (hdClient !== null) {
                        hdClient = JSON.parse(hdClient);

                        // remove HD with ID send
                        hdClient = $.grep(hdClient, function (x) {
                            return $.inArray(x.ID, ID_HoaDons) === -1
                        })

                        if (cthdClient !== null) {
                            cthdClient = JSON.parse(cthdClient);

                            // remove CTHD with list ID
                            cthdClient = $.grep(cthdClient, function (x) {
                                return $.inArray(x.ID_HoaDon, ID_HoaDons) === -1;
                            });
                            localStorage.setItem(lcListCTHD, JSON.stringify(cthdClient));
                        }
                        localStorage.setItem(lcListHD, JSON.stringify(hdClient));

                    }
                    break
            }
        }
    };

    hub.client.readData_NhaBep_toThuNgan = function (idViTri, maHoaDon, dataDB) {
        var idViTriNow = _phongBanID;

        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            var data = JSON.parse(dataDB);
            var lstCTHD = localStorage.getItem(lcListCTHD);
            if (lstCTHD !== null) {
                lstCTHD = JSON.parse(lstCTHD);
                console.log('arrCTHD_old ', lstCTHD)
                for (let i = 0; i < lstCTHD.length; i++) {
                    if (lstCTHD[i].ID === data.ID) {
                        // get idRandom from ID_HoaDon
                        var itemHD = $.grep(lstHD, function (x) {
                            return x.MaHoaDon === lstCTHD[i].MaHoaDon;
                        });
                        if (itemHD.length > 0) {
                            lstCTHD[i].Bep_SoLuongChoCungUng = data.Bep_SoLuongChoCungUng;
                            lstCTHD[i].Bep_SoLuongHoanThanh = data.Bep_SoLuongHoanThanh;
                            lstCTHD[i].Bep_SoLuongYeuCau = data.Bep_SoLuongYeuCau;
                            lstCTHD[i].IDRandomHD = itemHD[0].IDRandom;
                        }
                        break;
                    }
                }
                localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));

                if (idViTriNow === idViTri && _maHoaDon === maHoaDon) {
                    // bind CTHD
                    var arrCTHD = $.grep(lstCTHD, function (item) {
                        return item.MaHoaDon === maHoaDon;
                    });
                    console.log('arrCTHD_receive ', arrCTHD)
                    self.HangHoaAfterAdds(arrCTHD);
                }
            }
        }
    }

    $('#btnThongBaoNhaBep').click(function () {
        $(this).addClass('btnDisable');
        $(this).children('button').attr('disabled', 'disabled');
        InsertDB_ThongBaoNB(true);
    });

    //$('#btnThucHien').click(function () {
    //    self.ChuyenGhepBan();
    //});

    $('#btnSave').click(function () {
        self.saveHoaDon();
    });

    shortcut.add("F10", function () {
        if ($('.bgwhite').css('display') === "none") {
            $('.bgwhite').show();
            self.saveHoaDon();
        }
    });

    $('#btnDongBoHoa').click(function () {
        self.DongBoHoa();
    })

    $(document).keyup(function (e) {
        var keyCode = event.keyCode || event.which();
    })

    self.CloseHoaDon = function (item) {
        var thisMaHD = item.MaHoaDon;
        var thisIDVT = item.ID_ViTri;

        var itemHD = GetHDOpening_byMaHoaDon_andViTri(thisMaHD, thisIDVT);
        if (itemHD.length > 0) {
            let idRandomHD = itemHD[0].IDRandom;
            let idHoaDonDB = itemHD[0].ID;
            var hd = JSON.parse(localStorage.getItem(lcListHD));

            var cthd = localStorage.getItem(lcListCTHD);
            if (cthd !== null) {
                cthd = JSON.parse(cthd);

                let arrCT = $.grep(cthd, function (x) {
                    return x.IDRandomHD === idRandomHD;
                });

                if (arrCT.length > 0) {
                    let tenphong = $('#room_' + thisIDVT).text();
                    dialogConfirm('Xác nhận xóa',
                        'Bạn có chắc chắn muốn xóa <b> Hóa đơn ' + thisMaHD + ' </b> của bàn <b> ' + tenphong + ' </b> không?', function () {

                            // delete in db
                            if (idHoaDonDB !== const_GuidEmpty) {
                                ajaxHelper(BH_HoaDonUri + 'DeleteBH_HoaDon/' + idHoaDonDB, 'GET').done(function (x) {
                                });
                            }

                            // send to client with IDRandom delete
                            var objHub = CreateObjHub(null, null, idRandomHD, thisIDVT, 2, null);
                            SendData_atThisPage(objHub);

                            var arrBep = $.grep(cthd, function (x) {
                                return x.IDRandomHD === idRandomHD
                                    && (x.Bep_SoLuongYeuCau > 0 || x.Bep_SoLuongChoCungUng > 0);
                            });

                            // delete hd, cthd
                            hd = $.grep(hd, function (x) {
                                return x.IDRandom !== idRandomHD;
                            });
                            localStorage.setItem(lcListHD, JSON.stringify(hd));
                            // delete CTHD
                            cthd = $.grep(cthd, function (x) {
                                return x.IDRandomHD !== idRandomHD;
                            })
                            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                            if (arrBep.length > 0 && connected) {
                                ThongBaoBep_SendData(cthd, 5);
                            }

                            if (self.IndexHD_inTable() !== 0) {
                                self.IndexHD_inTable(self.IndexHD_inTable() - 1);
                            }
                            BindInforHD_byIndexHD();
                            CountTableUsed(true);
                            UpdateTime_lcPhongBan();
                            ChangeColorImg();
                            Enable_BtnGhepBan();
                            Enable_DisableNgayLapHD();
                        });
                }
                else {
                    // delete hoadon in cache server
                    hd = $.grep(hd, function (x) {
                        return x.IDRandom !== idRandomHD;
                    });
                    localStorage.setItem(lcListHD, JSON.stringify(hd));

                    if (self.IndexHD_inTable() !== 0) {
                        self.IndexHD_inTable(self.IndexHD_inTable() - 1);
                    }
                    BindInforHD_byIndexHD();
                    UpdateTime_lcPhongBan();
                    ChangeColorImg();
                    Enable_BtnGhepBan();
                    Enable_DisableNgayLapHD();
                }
            }
        }
    };

    function InsertDB_ThongBaoNB(daThongBao) {
        var idVT = _phongBanID;
        // get name PhongBan
        var tenViTri = '';
        var itemPB = $.grep(self.PhongBans(), function (x) {
            return x.ID === _phongBanID;
        });
        if (itemPB.length > 0) {
            tenViTri = itemPB[0].TenViTri;
        }

        var cthd = localStorage.getItem(lcListCTHD);
        var arrTBao = [];
        if (cthd !== null) {
            cthd = JSON.parse(cthd);

            // get hdopening --> to do get IDRandom
            var hdOpening = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);
            if (hdOpening.length > 0) {
                var idRandomHD = hdOpening[0].IDRandom;
                var idHoaDon = hdOpening[0].ID;

                var lstCTHDofPB = $.grep(cthd, function (item) {
                    return item.IDRandomHD === idRandomHD;
                });

                if (lstCTHDofPB.length === 0) {
                    ShowMessage_Danger('Vui lòng nhập chi tiết tin hóa đơn');
                    Enable_BtnThongBaoNB();
                    return false;
                }

                // check TonKho or not have CTHD
                if (self.ThietLap().XuatAm === false) {
                    var msgErr = "";
                    for (let i = 0; i < lstCTHDofPB.length; i++) {
                        if (lstCTHDofPB[i].LaHangHoa && lstCTHDofPB[i].SoLuong > lstCTHDofPB[i].TonKho) {
                            msgErr += lstCTHDofPB[i].TenHangHoa + ", ";
                        }
                    }

                    if (msgErr !== "") {
                        msgErr = msgErr.substr(0, msgErr.length - 2);
                        ShowMessage_Danger('Không đủ số lượng tồn kho cho sản phẩm ' + msgErr);
                        Enable_BtnThongBaoNB();
                        return false;
                    }
                }

                //update Cache CTHD with Satus = 1;
                for (let i = 0; i < cthd.length; i++) {
                    if (cthd[i].IDRandomHD === idRandomHD) {
                        let _soluongYC = cthd[i].SoLuong;
                        //truong hop sluong ycau da giam di, sau do tang soluong ycau len
                        if (cthd[i].Bep_SoLuongYeuCau > 0) {
                            _soluongYC = _soluongYC - (cthd[i].Bep_SoLuongYeuCau +
                                cthd[i].Bep_SoLuongChoCungUng + cthd[i].Bep_SoLuongHoanThanh) + cthd[i].Bep_SoLuongYeuCau;
                        }
                        if (cthd[i].Bep_SoLuongYeuCau == 0 &&
                            (cthd[i].Bep_SoLuongChoCungUng > 0 || (cthd[i].Bep_SoLuongHoanThanh > 0))) {
                            _soluongYC = _soluongYC - (cthd[i].Bep_SoLuongChoCungUng + cthd[i].Bep_SoLuongHoanThanh);
                        }
                        cthd[i].Bep_SoLuongYeuCau = _soluongYC;
                        cthd[i].TenPhongBan = tenViTri;
                        arrTBao.push(cthd[i]);
                    }
                }
                localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                var lcHD = localStorage.getItem(lcListHD);
                if (lcHD !== null) {
                    lcHD = JSON.parse(lcHD);

                    //chi luu vao DBkhi online
                    if (navigator.onLine) {
                        var myData = {};

                        arrTBao = AssignProperties_forCTHD_whenSave(arrTBao);

                        var ngaylapHD = GetNgayLapHD_whenSave(hdOpening[0].NgayLapHoaDon);
                        hdOpening[0].NgayLapHoaDon = ngaylapHD;
                        hdOpening[0].LoaiHoaDon = 3;
                        hdOpening[0].ChoThanhToan = true;

                        myData.objHoaDon = hdOpening[0];
                        myData.objCTHoaDon = arrTBao;
                        console.log('myDataTBBep ', myData)

                        // if delete all HangHoa of HD --> delete HD in DB
                        if (arrTBao.length === 0 && idHoaDon !== const_GuidEmpty) {
                            ajaxHelper(BH_HoaDonUri + "DeleteBH_HoaDon/" + hdOpening[0].ID, 'GET').done(function (x) {

                                var objSend = CreateObjHub(JSON.stringify(lcHD), JSON.stringify(cthd), idRandomHD, _phongBanID, 2, null);
                                SendData_atThisPage(objSend);
                                if (connected) {
                                    ThongBaoBep_SendData(cthd, 5);
                                }
                                ShowMessage_Success('Cập nhật hóa đơn thành công');
                            }).alway(function () {
                                $('.nt1').attr('disabled', 'disabled');
                            })
                        }
                        else {
                            // insert : POST
                            if (idHoaDon === const_GuidEmpty) {
                                ajaxHelper(BH_HoaDonUri + "Insert_ThongBaoNhabep", 'POST', myData).done(function (item) {
                                    var maHDNew = item.MaHoaDon;
                                    _maHoaDon = item.MaHoaDon;

                                    // if insert ==> update again ID, MaHoaDon
                                    for (let i = 0; i < lcHD.length; i++) {
                                        if (lcHD[i].IDRandom === idRandomHD) {
                                            lcHD[i].ID = item.ID;
                                            lcHD[i].MaHoaDon = maHDNew;
                                            lcHD[i].StatusTBNB = true;
                                            lcHD[i].LoaiHoaDon = 3;
                                            break;
                                        }
                                    }
                                    localStorage.setItem(lcListHD, JSON.stringify(lcHD));

                                    // update ID_CTHD in Cache CTHD 
                                    for (let i = 0; i < item.BH_HoaDon_ChiTiet.length; i++) {
                                        for (let j = 0; j < cthd.length; j++) {
                                            if (cthd[j].IDRandomHD === idRandomHD
                                                && cthd[j].ID_DonViQuiDoi === item.BH_HoaDon_ChiTiet[i].ID_DonViQuiDoi) {
                                                cthd[j].ID = item.BH_HoaDon_ChiTiet[i].ID;
                                                cthd[j].MaHoaDon = maHDNew;
                                                cthd[j].ID_HoaDon = item.ID;
                                            }
                                        }
                                    }
                                    localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                                    if (connected) {
                                        ThongBaoBep_SendData(cthd, 5);
                                    }
                                    var objSend = CreateObjHub(JSON.stringify(lcHD), JSON.stringify(cthd), idRandomHD, idVT, 1, null);
                                    SendData_atThisPage(objSend);

                                    BindHD_CTHD();
                                    ShowMessage_Success('Cập nhật hóa đơn thành công');
                                }).always(function () {
                                    $('.bs-example-modal-lg').modal('hide');
                                    $('.nt1').attr('disabled', 'disabled');
                                });
                            }
                            else {
                                // update
                                ajaxHelper(BH_HoaDonUri + "Update_ThongBaoNhabep", 'POST', myData).done(function (item) {
                                    _maHoaDon = hdOpening[0].MaHoaDon;

                                    // update again id fro cthd
                                    for (let i = 0; i < item.BH_HoaDon_ChiTiet.length; i++) {
                                        for (let j = 0; j < cthd.length; j++) {
                                            if (cthd[j].IDRandomHD === idRandomHD &&
                                                cthd[j].ID_DonViQuiDoi === item.BH_HoaDon_ChiTiet[i].ID_DonViQuiDoi) {
                                                cthd[j].ID = item.BH_HoaDon_ChiTiet[i].ID;
                                            }
                                        }
                                    }
                                    console.log('cthd ', cthd)
                                    localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                                    // update StatusTBND == true
                                    for (let i = 0; i < lcHD.length; i++) {
                                        if (lcHD[i].IDRandom === idRandomHD) {
                                            lcHD[i].StatusTBNB = true;
                                            lcHD[i].ID = item.ID;
                                            break;
                                        }
                                    }
                                    localStorage.setItem(lcListHD, JSON.stringify(lcHD));

                                    var objSend = CreateObjHub(JSON.stringify(lcHD), JSON.stringify(cthd), idRandomHD, _phongBanID, 1, null);
                                    SendData_atThisPage(objSend);
                                    if (connected) {
                                        ThongBaoBep_SendData(cthd, 5);
                                    }
                                    ShowMessage_Success('Cập nhật hóa đơn thành công');

                                }).always(function () {
                                    $('.bs-example-modal-lg').modal('hide');
                                    $('.nt1').attr('disabled', 'disabled');
                                });
                            }
                        }
                    }
                }
            }
        }
    }

    // Shortcut
    shortcut.add("F3", function () {
        $('#inputFocus').focus();
    });

    shortcut.add('F8', function () {
        $('.parent-price-1').trigger('click');
    });

    shortcut.add('F7', function () {
        $('#tdTax').trigger('click');
    });

    function Enable_BtnThongBaoNB() {
        $('#btnThongBaoNhaBep').removeClass('btnDisable');
        $('#btnThongBaoNhaBep').children('button').removeAttr('disabled');
    }

    function Enable_BtnGhepBan() {
        $('#btnGhepBan').removeClass('btnDisable');
        $('#btnGhepBan').children('button').removeAttr('disabled');
    }

    function Disable_BtnThongBaoNB() {
        $('#btnThongBaoNhaBep').addClass('btnDisable');
        $('#btnThongBaoNhaBep').children('button').attr('disabled', 'disabled');
    }

    function Disable_BtnGhepBan() {
        $('#btnGhepBan').addClass('btnDisable');
        $('#btnGhepBan').children('button').attr('disabled', 'disabled');
    }

    // check use ipad
    function isIpad() {
        return navigator.userAgent.match(/iPad/i) !== null;
    }

    $("#modalPopuplgDelete").on('keydown', function (e) {
        var key = e.which || e.keyCode;
        if (key == 13) {
            console.log(1);
            //$('#confirmOkDelete')[0].click(); // <----use the DOM click this way!!!
        }
    });

    function GetInforChiNhanh() {
        if (navigator.onLine) {
            ajaxHelper('/api/DM_DonViAPI/' + 'GetListDonVi1', 'GET').done(function (data) {
                if (data !== null) {
                    self.ChiNhanhs(data);
                    newModal_AddKhachHang.ChiNhanhs(data);
                    WriteData_Dexie(db.DM_DonVi, data);

                    newModal_AddKhachHang.ID_DonVi(_idDonVi);
                    newModal_AddKhachHang.ID_NhanVien(_idNhanVien);
                    newModal_AddKhachHang.ID_User(_idUser);
                    newModal_AddKhachHang.UserLogin(_userLogin);
                }
            });
        }
        else {
            db.DM_DonVi.toArray(function (x) {
                self.ChiNhanhs(x);
            });
        }
    }

    // hover in HoaDon
    self.GetMaHoaDon = function (item) {
        $('.hoverMaHoaDon').text(item.MaHoaDon);
        $('.hoverMaHoaDon').toggle();
    }

    function GetInforHDPrint(objHD) {

        var lcDoiTuong = self.DoiTuongs();
        var tenDTuong = 'Khách lẻ';

        var notruoc = 0;
        var nosau = 0;

        var itemEx = $.grep(lcDoiTuong, function (item) {
            return item.ID === objHD.ID_DoiTuong;
        });

        // get nameDT for print HoaDon
        if (itemEx.length > 0) {
            tenDTuong = itemEx[0].TenDoiTuong;
            notruoc = itemEx[0].NoHienTai === undefined ? 0 : itemEx[0].NoHienTai;
        }
        else {
            // check KH offline
            var khOffline = localStorage.getItem(lcListKHoffline);
            if (khOffline !== null) {
                khOffline = JSON.parse(khOffline);
                var itemKH = $.grep(khOffline, function (item) {
                    return item.MaHoaDon === objHD.MaHoaDon;
                });
                if (itemKH.length > 0) {
                    tenDTuong = itemKH[0].TenDoiTuong;
                }
            }
            notruoc = 0;
        }
        // Start Print
        objHD.TenDoiTuong = tenDTuong;
        objHD.NgayLapHoaDon = moment(objHD.NgayLapHoaDon, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss')
        // dia chi KH, SDT
        var address = '';
        var phone = '';

        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0) {
            address = self.ChiTietDoiTuong()[0].DiaChi;
            phone = self.ChiTietDoiTuong()[0].DienThoai;
        }

        objHD.DiaChiKhachHang = address;
        objHD.DienThoaiKhachHang = phone;

        // nhan vien BH
        var staffName = '';
        var itemNV = $.grep(self.NhanViens(), function (item) {
            return item.ID === objHD.ID_NhanVien;
        });
        if (itemNV.length > 0) {
            staffName = itemNV[0].TenNhanVien;
        }
        objHD.NhanVienBanHang = staffName;
        // ten PB
        var tenPB = '';
        var lcPhongBan = localStorage.getItem('lc_PhongBans');
        if (lcPhongBan !== null && lcPhongBan !== 'null') {
            lcPhongBan = JSON.parse(lcPhongBan);
            var itemPB = $.grep(lcPhongBan, function (item) {
                return item.ID === objHD.ID_ViTri;
            });
            if (itemPB.length > 0) {
                tenPB = itemPB[0].TenViTri;
            }
            objHD.TenPhongBan = tenPB;
        }

        // tong tien
        if (formatNumberToInt(objHD.PhaiTraKhach) > 0) {
            var phaiTraKhach = formatNumberToInt(objHD.PhaiTraKhach);
            objHD.TongCong = formatNumber(phaiTraKhach);
        }
        else {
            var khachCanTra = formatNumberToInt(objHD.PhaiThanhToan);
            objHD.TongCong = formatNumber(khachCanTra);
        }

        var phaiTT = formatNumberToInt(objHD.PhaiThanhToan);
        var noHienTai = phaiTT - formatNumberToInt(objHD.DaThanhToan);

        // khach tra thua tien
        if (noHienTai < 0) {
            noHienTai = 0
        }
        var tiendu = formatNumberToInt(objHD.DaThanhToan) -
            (formatNumberToInt(objHD.TongTienThue) + formatNumberToInt(objHD.TongTienHang) - formatNumberToInt(objHD.TongGiamGia));
        objHD.TongTienTraHang = formatNumber(objHD.TongTienTra);
        objHD.TongChiPhi = formatNumber(objHD.TongChiPhi);
        objHD.TongTienThue = formatNumber(objHD.TongTienThue);
        objHD.PhaiTraKhach = formatNumber(objHD.PhaiTraKhach);
        objHD.DaTraKhach = formatNumber(objHD.DaTraKhach);

        objHD.TienBangChu = DocSo(phaiTT);
        objHD.TongTienHoaDonMua = formatNumber(objHD.TongTienHang);
        objHD.TongGiamGia = formatNumber(objHD.TongGiamGia);
        objHD.PhaiThanhToan = formatNumber(objHD.PhaiThanhToan);
        objHD.DaThanhToan = formatNumber(objHD.DaThanhToan);
        objHD.TongTienHang = objHD.TongTienHoaDonMua;
        objHD.TienThua = formatNumber(tiendu);
        objHD.NoTruoc = formatNumber(notruoc);
        objHD.NoSau = formatNumber(notruoc + noHienTai);
        // chi nhanh
        objHD.DiaChiChiNhanh = '';
        var lcChiNhanh = localStorage.getItem('lcChiNhanh');
        if (lcChiNhanh !== null) {
            lcChiNhanh = JSON.parse(lcChiNhanh);

            if (_idDonVi !== '') {
                var itemChiNhanh = $.grep(lcChiNhanh, function (item) {
                    return item.ID === _idDonVi;
                });
                if (itemChiNhanh.length > 0) {
                    objHD.ChiNhanhBanHang = itemChiNhanh[0].TenDonVi;
                    objHD.DienThoaiChiNhanh = itemChiNhanh[0].SoDienThoai;
                    objHD.TenChiNhanh = itemChiNhanh[0].TenDonVi;
                    objHD.DiaChiChiNhanh = itemChiNhanh[0].DiaChi;
                }
            }
            else {
                objHD.TenChiNhanh = lcChiNhanh[0].TenDonVi;
                objHD.DienThoaiChiNhanh = lcChiNhanh[0].SoDienThoai;
                objHD.ChiNhanhBanHang = lcChiNhanh[0].TenDonVi;
                objHD.DiaChiChiNhanh = lcChiNhanh[0].DiaChi;
            }
        }

        // congty
        if (self.CongTy() !== null && self.CongTy().length > 0) {
            objHD.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            objHD.TenCuaHang = self.CongTy()[0].TenCongTy;
            objHD.DiaChiCuaHang = self.CongTy()[0].DiaChi;
        }

        return objHD;
    }

    function GetCTHDPrint_Format(arrCTHD) {
        for (let i = 0; i < arrCTHD.length; i++) {
            var dongia = formatNumberToInt(arrCTHD[i].DonGia);
            var tienGiam = formatNumberToInt(arrCTHD[i].TienChietKhau);

            arrCTHD[i].DonGia = formatNumber(dongia);
            arrCTHD[i].TienChietKhau = formatNumber(tienGiam);
            arrCTHD[i].GiaBan = formatNumber(dongia - tienGiam);
            arrCTHD[i].SoLuong = formatNumber3Digit(arrCTHD[i].SoLuong, 4);
            arrCTHD[i].ThanhTien = formatNumber3Digit(arrCTHD[i].ThanhTien);
            arrCTHD[i].SoLuongDVDaSuDung = 0;
            arrCTHD[i].SoLuongDVConLai = 0;
        }
        return arrCTHD;
    }

    self.ShowinforTon_Dat = function (item) {
        $(".content-warning").hide();
        $('#wr_' + item.IDRandom).show();
        $(".content-warning").mouseup(function () {
            return false;
        });
        $(".main-warning").mouseup(function () {
            return false;
        });
        $(document).mouseup(function () {
            $(".content-warning").hide();
        });
    }

    function GetCauHinhHeThong() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + _idDonVi, 'GET').done(function (data) {
                self.ThietLap(data);
                localStorage.setItem('CauHinhHeThong', JSON.stringify(data));
                getListNhomDT();
            });
        }
        else {
            var cauhinh = localStorage.getItem('CauHinhHeThong');
            if (cauhinh !== null) {
                cauhinh = JSON.parse(cauhinh);
                self.ThietLap(cauhinh);
            }
        }
    }

    // Phieu Thu
    self.showPopupLapPhieuThu = function () {
        $('#modalPopupLapPhieuThu').modal('show');
        self.RemoveKH_PhieuThu();
    }
    //tra cứu hàng hóa
    self.showPopupTraCuuHH = function () {
        $('#trahanghoamodal').modal('show');
        $('#txtTraCuuHH').val("");
        self.arrTraCuuHangHoa([]);
        self.TotalRecordTK(0);
        self.PageCountTK(0);
    }
    self.arrTraCuuHangHoa = ko.observableArray();
    $('#txtTraCuuHH').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPageTK(0);
            var search = $('#txtTraCuuHH').val();
            if (search !== null) {
                searchTheKho();
            } else {
                self.arrTraCuuHangHoa([]);
            }
        }
    })

    self.pageSizesTK = [10, 20, 30];
    self.pageSizeTK = ko.observable(self.pageSizesTK[0]);
    self.currentPageTK = ko.observable(0);
    self.fromitemtk = ko.observable(1);
    self.toitemtk = ko.observable();
    self.arrPagging = ko.observableArray();
    self.PageCountTK = ko.observable();
    self.TotalRecordTK = ko.observable(0);

    var isGoToNext = false;
    function searchTheKho(isGoToNext) {
        hidewait('table_TraCuu');
        var search = $('#txtTraCuuHH').val();
        ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'TraCuuHangHoa?currentPage=' + self.currentPageTK() + '&pageSize=' + self.pageSizeTK() + '&id_donvi=' + _idDonVi + '&txtSearch=' + search, 'GET').done(function (data) {
            self.arrTraCuuHangHoa(data.data.lstHH);
            self.TotalRecordTK(data.data.Rowcount);
            self.PageCountTK(data.data.pageCount);
            $('#wait').remove();
        });
    };

    self.PageList_Display_TheKho = ko.computed(function () {
        var arrPage = [];

        var allPage = self.PageCountTK();
        var currentPage = self.currentPageTK();

        if (allPage > 4) {

            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageTK()) + 1;
            }
            else {
                i = self.currentPageTK();
            }

            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (let i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberTK: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (let j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberTK: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (let j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberTK: j + 1,
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
                            pageNumberTK: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberTK: i,
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
                    var obj = {
                        pageNumberTK: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });

    self.PageResultTKs = ko.computed(function () {
        if (self.arrTraCuuHangHoa() !== null) {

            self.fromitemtk((self.currentPageTK() * self.pageSizeTK()) + 1);

            if (((self.currentPageTK() + 1) * self.pageSizeTK()) > self.arrTraCuuHangHoa().length) {
                var fromItem = (self.currentPageTK() + 1) * self.pageSizeTK();
                if (fromItem < self.TotalRecordTK()) {
                    self.toitemtk((self.currentPageTK() + 1) * self.pageSizeTK());
                }
                else {
                    self.toitemtk(self.TotalRecordTK());
                }
            } else {
                self.toitemtk((self.currentPageTK() * self.pageSizeTK()) + self.pageSizeTK());
            }
        }
    });

    self.ResetCurrentPageTheKho = function () {
        self.currentPageTK(0);
        searchTheKho();
    };

    self.VisibleStartPageTheKho = ko.computed(function () {
        if (self.PageList_Display_TheKho().length > 0) {
            return self.PageList_Display_TheKho()[0].pageNumberTK !== 1;
        }
    });

    self.VisibleEndPageTheKho = ko.computed(function () {
        if (self.PageList_Display_TheKho().length > 0) {
            return self.PageList_Display_TheKho()[self.PageList_Display_TheKho().length - 1].pageNumberTK !== self.PageCountTK();
        }
    });

    self.GoToPage = function (page) {
        if (page.pageNumberTK !== '.') {
            self.currentPageTK(page.pageNumberTK - 1);
            searchTheKho();
        }
    };
    self.GetClass = function (page) {
        return ((page.pageNumberTK - 1) === self.currentPageTK()) ? "click" : "";
    };

    self.StartPageTheKho = function () {
        self.currentPageTK(0);
        searchTheKho();
    }
    self.BackPageTheKho = function () {
        if (self.currentPageTK() > 1) {
            self.currentPageTK(self.currentPageTK() - 1);
            searchTheKho();
        }
    }
    self.GoToNextPageTheKho = function () {
        if (self.currentPageTK() < self.PageCountTK() - 1) {
            self.currentPageTK(self.currentPageTK() + 1);
            searchTheKho();
        }
    }
    self.EndPageTheKho = function () {
        if (self.currentPageTK() < self.PageCountTK() - 1) {
            self.currentPageTK(self.PageCountTK() - 1);
            searchTheKho();
        }
    }

    self.IDDoiTuong_PhieuThu.subscribe(function (newID) {

        if (newID === undefined) {
            self.ChiTietKH_PhieuThu(null);
            self.ListHDisDebit(null);
        }
        else {
            for (let i = 0; i < self.DoiTuongs().length; i++) {
                if (self.DoiTuongs()[i].ID === newID) {
                    self.ChiTietKH_PhieuThu(self.DoiTuongs()[i]);

                    GetListHD_isDebitOfKH(newID);
                    break;
                }
            }
        }
    });

    self.RemoveKH_PhieuThu = function () {
        self.IDDoiTuong_PhieuThu(undefined);

        self.ThuTuKhach(0);
        self.NoSau(0);
        self.TongThanhToan(0);
        self.CongVaoTK(0);
        self.GhiChu_PhieuThu('');

        // reset TienMat,TienGui da nhap voi PhieuThu truoc do
        self.KhachTT_PhieuThu(undefined);
        self.TienMat_PhieuThu(0);
        self.TienATM_PhieuThu(0);
        self.TienGui_PhieuThu(0);
        $('#txtSearchKH_PhieuThu').val('');
        $('#lblTienMat_PhieuThu').text('(Tiền mặt)');
    }

    self.popupThanhToanThe_PhieuThu = function () {
        var idDoiTuong = self.IDDoiTuong_PhieuThu();
        if (idDoiTuong !== undefined) {
            $('#modalPopup_ThanhToanPhieuThu').modal('show');
            $('#divCard_PhieuThu').css('display', 'none');

            // check xem da nhap tien va click Dong y chua: neu chua thi clear text
            if (self.KhachTT_PhieuThu() === undefined) {
                $('#txtTienMat_PT').val(formatNumber(self.ChiTietKH_PhieuThu().NoHienTai));
                $('#txtTienATM_PT').val(0);
                $('#txtTienGui_PT').val(0);
                $('#lblKhachTT_PT').text(formatNumber(self.TienMat_PhieuThu()));

                self.selectID_POSPT(undefined);
                self.selectID_ChuyenKhoanPT(undefined);
                $('#lstAccountPOS_PhieuThu, #lstAccountCK_PhieuThu li').each(function () {
                    $(this).find('i').remove();
                });
                $('#divAccountPOS_PhieuThu').text('---Chọn tài khoản---');
                $('#divAccountCK_PhieuThu').text('---Chọn tài khoản---');
            }
            else {
                // if was agree --> set text with value
                $('#txtTienMat_PT').val(formatNumber(self.TienMat_PhieuThu()));
                $('#txtTienATM_PT').val(formatNumber(self.TienATM_PhieuThu()));
                $('#txtTienGui_PT').val(formatNumber(self.TienGui_PhieuThu()));
                $('#lblKhachTT_PT').text(formatNumber(self.KhachTT_PhieuThu()));
            }
        }
        else {
            ShowMessage_Danger('Vui lòng chọn khách hàng');
        }
    }

    self.TinhNoSau = function () {
        var noHienTai = self.ChiTietKH_PhieuThu().NoHienTai;
        if (noHienTai === undefined) {
            noHienTai = 0;
        }
        formatNumberObj($('#txtThuTuKhach'));

        var thuTuKhach = formatNumberToInt(self.ThuTuKhach());
        var noSau = Math.round(noHienTai - thuTuKhach);
        self.NoSau(noSau);

        // update TienThu for List HoaDonDebit 
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            if (thuTuKhach <= self.ListHDisDebit()[i].TienMat) {
                self.ListHDisDebit()[i].TienThu = thuTuKhach;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(thuTuKhach));
                if (i === 0 && self.ListHDisDebit().length > 0) {
                    for (let j = 1; j < self.ListHDisDebit().length; j++) {
                        self.ListHDisDebit()[j].TienThu = 0;
                        $('#tienthu_' + self.ListHDisDebit()[j].ID).val("0");
                    }
                }
                break;
            }
            else {
                self.ListHDisDebit()[i].TienThu = self.ListHDisDebit()[i].TienMat;
                thuTuKhach = thuTuKhach - self.ListHDisDebit()[i].TienMat;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(self.ListHDisDebit()[i].TienThu));
            }
        }

        var tongTT = 0;
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            tongTT += self.ListHDisDebit()[i].TienThu;
        }
        self.TongThanhToan(tongTT);

        var valThuTuKhach = formatNumberToInt(self.ThuTuKhach());
        var congvaoTK = valThuTuKhach - tongTT;
        congvaoTK = congvaoTK > 0 ? congvaoTK : 0;
        self.CongVaoTK(congvaoTK);

        // reset all infor TienATM, tienGui da nhap truoc do
        $('#lblTienMat_PhieuThu').text('Tiền mặt');
        self.TienGui_PhieuThu(0);
        self.TienATM_PhieuThu(0);
        self.TienMat_PhieuThu(valThuTuKhach);
        self.shareMoneys([]);
    }

    self.editTienMat_PhieuThu = function () {
        var tongTien = self.ChiTietKH_PhieuThu().NoHienTai;
        if (tongTien === undefined) {
            tongTien = 0;
        }

        var objTienMat = $('#txtTienMat_PT');
        formatNumberObj(objTienMat);
        var tienMat = formatNumberToInt(objTienMat.val());
        var tienATM = 0;
        if (tongTien < 0) {
            tienATM = 0;
        }

        // neu chua chon tai khoan POS
        var havePOS = false;
        var haveChuyenKhoan = false;
        if (self.selectID_POSPT() !== undefined && self.selectID_POSPT() !== null) {
            tienATM = Math.round(tongTien - tienMat);
            havePOS = true;
        }

        if (self.selectID_ChuyenKhoanPT() !== undefined && self.selectID_ChuyenKhoanPT() !== null) {
            tienGui = Math.round(tongTien - (tienMat + tienATM));
            haveChuyenKhoan = true;
        }

        $('#txtTienATM_PT').val(formatNumber(tienATM));
        $('#lblKhachTT_PT').text(formatNumber(tienMat + tienATM));
        $('#txtTienGui_PT').val(0);
        $('#lblTienThua_PT').text(0);

        var key = event.keyCode || event.which;
        if (key === 13) {
            if (havePOS) {
                $('#txtTienATM_PT').select();
            }
            else {
                if (haveChuyenKhoan) {
                    $('#txtTienGui_PT').select();
                }
            }
        }
    }

    self.editTienATM_PhieuThu = function () {
        var objTienATM = $('#txtTienATM_PT');
        formatNumberObj(objTienATM);

        var tongPhaiTT = self.ChiTietKH_PhieuThu().NoHienTai;
        var tienMat = formatNumberToInt($('#txtTienMat_PT').val());
        var tienATM = formatNumberToInt(objTienATM.val());
        var tienGui = 0;

        var haveChuyenKhoan = false;
        if (self.selectID_ChuyenKhoanPT() !== undefined && self.selectID_ChuyenKhoanPT() !== null) {
            tienGui = Math.round(tongPhaiTT - (tienMat + tienATM));
            haveChuyenKhoan = true;
        }

        var khachTT = tienMat + tienATM + tienGui;
        var tienThua = khachTT - tongPhaiTT;
        if (tienThua < 0) {
            tienThua = 0;
        }
        $('#txtTienGui_PT').val(formatNumber(tienGui));
        $('#lblKhachTT_PT').text(formatNumber(khachTT));
        $('#lblTienThua_PT').text(formatNumber(tienThua));

        var key = event.keyCode || event.which;
        if (key === 13 && haveChuyenKhoan) {
            $('#txtTienGui_PT').select();
        }
    }

    self.editTienGui_PhieuThu = function (d) {

        var tongPhaiTT = self.ChiTietKH_PhieuThu().NoHienTai;

        formatNumberObj($('#txtTienGui_PT'));
        var tienATM = formatNumberToInt($('#txtTienATM_PT').val());
        var tienMat = formatNumberToInt($('#txtTienMat_PT').val());
        var tienGui = formatNumberToInt($('#txtTienGui_PT').val());

        var thanhToan = Math.round(tienMat + tienATM + tienGui);
        var tienThua = Math.round(thanhToan - tongPhaiTT);
        if (tienThua < 0) {
            tienThua = 0;
        }
        $('#lblKhachTT_PT').text(formatNumber(thanhToan));
        $('#lblTienThua_PT').text(formatNumber(tienThua));

        var key = event.keyCode || event.which;
        if (key === 13 || key === 9) {
        }
    }

    self.agreePay_PhieuThu = function () {
        $('#modalPopup_ThanhToanPhieuThu').modal('hide');

        var tienMat = formatNumberToInt($('#txtTienMat_PT').val());
        var tienATM = formatNumberToInt($('#txtTienATM_PT').val());
        var tienGui = formatNumberToInt($('#txtTienGui_PT').val());

        // asssign value to do check when show pop TT = the && addSoQuy
        self.TienMat_PhieuThu(tienMat);
        self.TienATM_PhieuThu(tienATM);
        self.TienGui_PhieuThu(tienGui);
        self.shareMoneys([]);

        var thuTuKhach = tienMat + tienATM + tienGui;
        self.ThuTuKhach(thuTuKhach);
        self.KhachTT_PhieuThu(thuTuKhach);

        $('#txtThuTuKhach').val(formatNumber(thuTuKhach));
        // tinh lai NoSau
        var noHienTai = self.ChiTietKH_PhieuThu().NoHienTai;
        if (noHienTai === undefined) {
            noHienTai = 0;
        }
        var noSau = Math.round(noHienTai - thuTuKhach);
        self.NoSau(noSau);

        // tinh tien thu
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            if (thuTuKhach <= self.ListHDisDebit()[i].TienMat) {
                self.ListHDisDebit()[i].TienThu = thuTuKhach;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(thuTuKhach));
                break;
            }
            else {
                self.ListHDisDebit()[i].TienThu = self.ListHDisDebit()[i].TienMat;
                thuTuKhach = thuTuKhach - self.ListHDisDebit()[i].TienMat;
                $('#tienthu_' + self.ListHDisDebit()[i].ID).val(formatNumber(self.ListHDisDebit()[i].TienThu));
            }
        }

        // tinh tong thanh toan (footer)
        var tongTT = 0;
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            tongTT += self.ListHDisDebit()[i].TienThu;
        }
        self.TongThanhToan(tongTT);
        var congvaoTK = thuTuKhach - tongTT;
        congvaoTK = congvaoTK > 0 ? congvaoTK : 0;
        self.CongVaoTK(congvaoTK);

        var objSetText = $('#lblTienMat_PhieuThu');
        if (tienMat > 0) {
            if (tienATM > 0) {
                if (tienGui) {
                    objSetText.text('(Tiền mặt, thẻ, chuyển khoản)');
                }
                else {
                    objSetText.text('(Tiền mặt, thẻ)');
                }
            }
            else {
                if (tienGui) {
                    objSetText.text('(Tiền mặt, chuyển khoản)');
                }
                else {
                    objSetText.text('(Tiền mặt)');
                }
            }
        }
        else {
            if (tienATM > 0) {
                if (tienGui) {
                    objSetText.text('(Thẻ, chuyển khoản)');
                }
                else {
                    objSetText.text('(Thẻ)');
                }
            }
            else {
                if (tienGui) {
                    objSetText.text('(Chuyển khoản)');
                }
                else {
                    objSetText.text('(Tiền mặt)');
                }
            }
        }
    }

    function GetListHD_isDebitOfKH(idDoiTuong) {
        var arrDV = [_idDonVi];
        // get hdDebit all chinhanh
        ajaxHelper(DMDoiTuongUri + 'GetListHD_isDebit?idDoiTuong=' + idDoiTuong
            + '&arrDonVi=' + arrDV + '&loaiDoiTuong=' + 1,
            'POST', arrDV).done(function (x) {
                if (x.res) {
                    var data = x.dataSoure;
                    for (let i = 0; i < data.length; i++) {
                        data[i].TienThu = 0;
                    }
                    self.ListHDisDebit(data);
                }
            })
    }

    // push to self.shareMoneys
    function shareMoney(tienthu, tienmat, tienthe, chuyenkhoan, thegiatri, itemHD) {
        let idHoaDon = itemHD.ID;
        let mahoadon = itemHD.MaHoaDon;
        var GTfistback = 0;
        var fistback = tienthu - tienmat;
        if (fistback >= 0) {
            // nếu tổng tiền cần TT của HD > Tiền mặt đã nhập --> tiếp tục lấy Tiền thẻ để TT cho HD này
            GTfistback = fistback;
            // get giá trị còn lại phải TT sau khi đã trừ tiền mặt
            fistback = fistback - tienthe; // dương: thiếu tiến, âm: thừa tiền
            if (fistback >= 0) {
                GTfistback = fistback;
                // get giá trị còn lại phải TT sau khi đã trừ tiền thẻ
                fistback = fistback - chuyenkhoan;
                if (fistback >= 0) {
                    GTfistback = fistback;
                    fistback = fistback - thegiatri;
                    if (fistback >= 0) {
                        let obj = {
                            TienMat: tienmat,
                            TienPOS: tienthe,
                            ChuyenKhoan: chuyenkhoan,
                            TienTheGiaTri: thegiatri,
                            ID_HoaDonLienQuan: idHoaDon,
                            MaHoaDon: mahoadon,
                        }
                        self.shareMoneys.push(obj);
                    }
                    else {
                        let obj = {
                            TienMat: tienmat,
                            TienPOS: tienthe,
                            ChuyenKhoan: chuyenkhoan,
                            TienTheGiaTri: GTfistback,
                            ID_HoaDonLienQuan: idHoaDon,
                            MaHoaDon: mahoadon,
                        }
                        self.shareMoneys.push(obj);
                    }
                }
                else {
                    let obj = {
                        TienMat: tienmat,
                        TienPOS: tienthe,
                        ChuyenKhoan: GTfistback,
                        TienTheGiaTri: 0,
                        ID_HoaDonLienQuan: idHoaDon,
                        MaHoaDon: mahoadon,
                    }
                    self.shareMoneys.push(obj);
                }
            }
            else {
                let obj = {
                    TienMat: tienmat,
                    TienPOS: GTfistback,
                    ChuyenKhoan: 0,
                    TienTheGiaTri: 0,
                    ID_HoaDonLienQuan: idHoaDon,
                    MaHoaDon: mahoadon,
                }
                self.shareMoneys.push(obj);
            }
        }
        else {
            let obj = {
                TienMat: tienthu,
                TienPOS: 0,
                ChuyenKhoan: 0,
                TienTheGiaTri: 0,
                ID_HoaDonLienQuan: idHoaDon,
                MaHoaDon: mahoadon,
            }
            self.shareMoneys.push(obj);
        }
    };

    function shareMoney_QuyHD(phaiTT, tienDiem, tienmat, tienPOS, chuyenkhoan, thegiatri, itemHD) {
        let idHoaDon = itemHD.ID;
        let mahoadon = itemHD.MaHoaDon;

        if (tienDiem >= phaiTT) {
            return {
                TTBangDiem: phaiTT,
                TienMat: 0,
                TienPOS: 0,
                TienChuyenKhoan: 0,
                TienTheGiaTri: 0,
                ID_HoaDonLienQuan: idHoaDon,
                MaHoaDon: mahoadon,
            }
        }
        else {
            phaiTT = phaiTT - tienDiem;
            if (thegiatri >= phaiTT) {
                return {
                    TTBangDiem: tienDiem,
                    TienMat: 0,
                    TienPOS: 0,
                    TienChuyenKhoan: 0,
                    TienTheGiaTri: Math.abs(phaiTT),
                    ID_HoaDonLienQuan: idHoaDon,
                    MaHoaDon: mahoadon,
                }
            }
            else {
                phaiTT = phaiTT - thegiatri;
                if (tienPOS >= phaiTT) {
                    return {
                        TTBangDiem: tienDiem,
                        TienMat: 0,
                        TienPOS: Math.abs(phaiTT),
                        TienChuyenKhoan: 0,
                        TienTheGiaTri: thegiatri,
                        ID_HoaDonLienQuan: idHoaDon,
                        MaHoaDon: mahoadon,
                    }
                }
                else {
                    phaiTT = phaiTT - tienPOS;
                    if (chuyenkhoan >= phaiTT) {
                        return {
                            TTBangDiem: tienDiem,
                            TienMat: 0,
                            TienPOS: tienPOS,
                            TienChuyenKhoan: Math.abs(phaiTT),
                            TienTheGiaTri: thegiatri,
                            ID_HoaDonLienQuan: idHoaDon,
                            MaHoaDon: mahoadon,
                        }
                    }
                    else {
                        phaiTT = phaiTT - chuyenkhoan;
                        if (tienmat >= phaiTT) {
                            return {
                                TTBangDiem: tienDiem,
                                TienMat: Math.abs(phaiTT),
                                TienPOS: tienPOS,
                                TienChuyenKhoan: chuyenkhoan,
                                TienTheGiaTri: thegiatri,
                                ID_HoaDonLienQuan: idHoaDon,
                                MaHoaDon: mahoadon,
                            }
                        }
                        else {
                            phaiTT = phaiTT - tienmat;
                            return {
                                TTBangDiem: tienDiem,
                                TienMat: tienmat,
                                TienPOS: tienPOS,
                                TienChuyenKhoan: chuyenkhoan,
                                TienTheGiaTri: thegiatri,
                                ID_HoaDonLienQuan: idHoaDon,
                                MaHoaDon: mahoadon,
                            }
                        }
                    }
                }
            }
        }
    }

    self.ChoseAccountPOS_PhieuThu = function (item) {

        $('#lstAccountPOS_PhieuThu li').each(function () {
            $(this).find('i').remove();
        });

        if (item.ID === undefined) {
            self.selectID_POSPT(undefined);
            $('#divAccountPOS_PhieuThu').text('---Chọn tài khoản---');

            // reset again TienAM, KhachTT
            var tienMat = formatNumberToInt($('#txtTienMat_PT').val());
            var tienGui = formatNumberToInt($('#txtTienGui_PT').val())
            $('#txtTienATM_PT').val(0);
            $('#lblKhachTT_PT').text(formatNumber(tienMat + tienGui));
        }
        else {
            self.selectID_POSPT(item.ID);
            $('#divAccountPOS_PhieuThu').text(item.TenChuThe);
            $('span[id=checkAccountPOS_PT_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
        }
    }

    self.ChoseAccountCK_PhieuThu = function (item) {

        $('#lstAccountCK_PhieuThu li').each(function () {
            $(this).find('i').remove();
        });

        if (item.ID === undefined) {
            self.selectID_ChuyenKhoanPT(undefined);
            $('#divAccountCK_PhieuThu').text('---Chọn tài khoản---');

            // reset tienMat, tienATM: get from input because not assign value for self.TienMat if not agree
            var tienMat = formatNumberToInt($('#txtTienMat_PT').val());
            var tienATM = formatNumberToInt($('#txtTienATM_PT').val())
            $('#txtTienGui_PT').val(0);
            $('#lblKhachTT_PT').text(formatNumber(tienMat + tienATM));
        }
        else {
            self.selectID_ChuyenKhoanPT(item.ID);
            $('#divAccountCK_PhieuThu').text(item.TenChuThe);
            $('span[id=checkAccountCK_PT_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
        }
    }

    self.addSoQuy = function (havePrint) {
        var idDoiTuong = self.IDDoiTuong_PhieuThu();
        if (idDoiTuong === undefined) {
            ShowMessage_Danger('Vui lòng chọn khách hàng');
            return false;
        }

        if (self.ThuTuKhach() === 0) {
            ShowMessage_Danger('Vui lòng nhập số tiền');
            return false;
        }

        // get date + time
        var d = new Date();
        var time = d.getSeconds();
        var ngayLap = moment(self.NgayLapPhieuThu(), 'DD/MM/YYYY HH:mm').format("YYYY-MM-DD HH:mm") + ':' + time;
        var tongThu = self.ThuTuKhach();

        var tienMat = self.TienMat_PhieuThu();
        var tienATM = self.TienATM_PhieuThu();
        var tienGui = self.TienGui_PhieuThu();
        var tienTheGiaTri = self.TienTheGiaTri_PhieuThu();
        var tienmatTr = tienMat;
        var tientheTr = tienATM;
        var tienchuyenkhoanTr = tienGui;
        var ghichu = self.GhiChu_PhieuThu();

        console.log(22, self.ChiTietKH_PhieuThu())
        var Quy_HoaDon = {
            NgayLapHoaDon: ngayLap,
            TongTienThu: tongThu,
            NguoiNopTien: self.ChiTietKH_PhieuThu().TenDoiTuong,
            NguoiTao: _userLogin,
            NoiDungThu: ghichu,
            ID_NhanVien: _idNhanVien,
            ID_DonVi: _idDonVi,
            ID_KhoanThuChi: null,
            LoaiHoaDon: 11,

            // used to get save diary 
            MaHoaDonTraHang: null,
            ID_DoiTuong: idDoiTuong,
        }

        var idKhoanThuChi = null;
        var idTaiKhoanPOS = self.selectID_POSPT();
        var idTaiKhoanCK = self.selectID_ChuyenKhoanPT();
        var phuongthucTT = '';
        var sMaHoaDonLienQuan = '';
        var lstQuy_ChiTiet = [];
        if (self.ListHDisDebit().length > 0) {
            for (let i = 0; i < self.ListHDisDebit().length; i++) {
                let itemHDDebit = self.ListHDisDebit()[i];
                // itemHDDebit.TienThu: Tiền thực tế thu
                // itemHDDebit.TienMat: tiền thực tế nợ
                let tienThucTeThu = itemHDDebit.TienThu;
                if (tienThucTeThu > 0) {
                    if (i === 0) {
                        shareMoney(tienThucTeThu, tienmatTr, tientheTr, tienchuyenkhoanTr, tienTheGiaTri, itemHDDebit);
                    }
                    else {
                        let lenArr = self.shareMoneys().length;
                        if (self.shareMoneys().length > 0) {
                            tienmatTr = tienmatTr - self.shareMoneys()[lenArr - 1].TienMat;
                            tientheTr = tientheTr - self.shareMoneys()[lenArr - 1].TienPOS;
                            tienchuyenkhoanTr = tienchuyenkhoanTr - self.shareMoneys()[lenArr - 1].ChuyenKhoan;
                        }
                        shareMoney(tienThucTeThu, tienmatTr, tientheTr, tienchuyenkhoanTr, tienTheGiaTri, itemHDDebit);
                    }
                }
            }

            tongThu = 0;
            for (let i = 0; i < self.shareMoneys().length; i++) {
                let itemFor = self.shareMoneys()[i];
                let idHoaDon = itemFor.ID_HoaDonLienQuan;
                if (itemFor.TienMat > 0) {
                    tongThu += itemFor.TienMat;
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.TienMat,
                        TienMat: itemFor.TienMat,
                        HinhThucThanhToan: 1,
                    });
                    lstQuy_ChiTiet.push(qct);
                }
                if (itemFor.TienPOS > 0) {
                    tongThu += itemFor.TienPOS;
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.TienPOS,
                        TienPOS: itemFor.TienPOS,
                        HinhThucThanhToan: 2,
                        ID_TaiKhoanNganHang: idTaiKhoanPOS,
                    });
                    lstQuy_ChiTiet.push(qct);
                }
                if (itemFor.ChuyenKhoan > 0) {
                    tongThu += itemFor.ChuyenKhoan;
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.ChuyenKhoan,
                        TienChuyenKhoan: itemFor.ChuyenKhoan,
                        HinhThucThanhToan: 3,
                        ID_TaiKhoanNganHang: idTaiKhoanCK,
                    });
                    lstQuy_ChiTiet.push(qct);
                }
                if (itemFor.TienTheGiaTri > 0) {
                    tongThu += itemFor.TienTheGiaTri;
                    let qct = newQuyChiTiet({
                        ID_HoaDonLienQuan: idHoaDon,
                        ID_KhoanThuChi: idKhoanThuChi,
                        ID_DoiTuong: idDoiTuong,
                        GhiChu: ghichu,
                        TienThu: itemFor.TienTheGiaTri,
                        TienTheGiaTri: itemFor.TienTheGiaTri,
                        HinhThucThanhToan: 4,
                    });
                    lstQuy_ChiTiet.push(qct);
                }
                sMaHoaDonLienQuan += itemFor.MaHoaDon + ', ';
            }
            sMaHoaDonLienQuan = Remove_LastComma(sMaHoaDonLienQuan);
        }
        else {
            if (tienMat === undefined) {
                tienMat = tongThu;
                tienGui = 0;
            }
            let idHoaDon = null;
            if (tienMat > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienMat,
                    TienMat: tienMat,
                    HinhThucThanhToan: 1,
                });
                lstQuy_ChiTiet.push(qct);
            }
            if (tienATM > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienATM,
                    TienPOS: tienATM,
                    HinhThucThanhToan: 2,
                    ID_TaiKhoanNganHang: idTaiKhoanPOS,
                });
                lstQuy_ChiTiet.push(qct);
            }
            if (tienGui > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienGui,
                    TienChuyenKhoan: tienGui,
                    HinhThucThanhToan: 3,
                    ID_TaiKhoanNganHang: idTaiKhoanCK,
                });
                lstQuy_ChiTiet.push(qct);
            }
            if (tienTheGiaTri > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienTheGiaTri,
                    TienTheGiaTri: tienTheGiaTri,
                    HinhThucThanhToan: 4,
                });
                lstQuy_ChiTiet.push(qct);
            }
            tongThu = tienMat + tienATM + tienGui + tienTheGiaTri;
        }

        console.log('lstQuy_ChiTiet ', lstQuy_ChiTiet);

        if (tienMat > 0) {
            phuongthucTT = 'Tiền mặt, ';
        }
        if (tienATM > 0) {
            phuongthucTT += 'Thẻ, ';
        }
        if (tienGui > 0) {
            phuongthucTT += 'Chuyển khoản, ';
        }

        phuongthucTT = Remove_LastComma(phuongthucTT);
        Quy_HoaDon.PhuongThucTT = phuongthucTT;

        var myData = {};
        myData.objQuyHoaDon = Quy_HoaDon;
        myData.lstCTQuyHoaDon = lstQuy_ChiTiet;

        // insert Quy_HoaDon, Quy_ChiTiet
        $.ajax({
            data: myData,
            url: '/api/DanhMuc/Quy_HoaDonAPI/' + "PostQuy_HoaDon_LstQuyChiTiet",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (result) {
                if (result.res === true) {
                    var item = result.data;

                    // Chi Nhanh
                    var itemChiNhanh = $.grep(self.ChiNhanhs(), function (x) {
                        return x.ID === _idDonVi;
                    });
                    item.TenChiNhanh = '';
                    item.DienThoaiChiNhanh = '';
                    if (itemChiNhanh.length > 0) {
                        item.ChiNhanhBanHang = itemChiNhanh[0].TenDonVi;
                        item.DienThoaiChiNhanh = itemChiNhanh[0].SoDienThoai;
                    }

                    item.NgayLapHoaDon = moment(ngayLap, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
                    item.NguoiNopTien = Quy_HoaDon.NguoiNopTien;
                    item.NoiDungThu = Quy_HoaDon.NoiDungThu;
                    item.HoaDonLienQuan = sMaHoaDonLienQuan;
                    item.DiaChiKhachHang = self.ChiTietKH_PhieuThu().DiaChi;
                    item.DienThoaiKhachHang = self.ChiTietKH_PhieuThu().DienThoai;
                    item.TienBangChu = DocSo(item.TongTienThu);
                    item.MaPhieu = item.MaHoaDon;
                    item.GiaTriPhieu = formatNumber(item.TongTienThu);

                    if (self.CongTy() !== null && self.CongTy().length > 0) {
                        item.LogoCuaHang = self.CongTy()[0].DiaChiCuaHang;
                        item.TenCuaHang = self.CongTy()[0].TenCongTy;
                        item.DiaChiCuaHang = self.CongTy()[0].DiaChi;
                    }

                    self.InforHDprintf(item);

                    if (havePrint) {
                        self.InHoaDon("SQPT", false);
                    }

                    // assign used to save diary
                    Quy_HoaDon.MaHoaDon = item.MaHoaDon;
                    Insert_NhatKyThaoTac(Quy_HoaDon, [], 3, 1);

                    ShowMessage_Success('Cập nhật phiếu thu thành công');
                }
                else {
                    ShowMessage_Danger(result.mes);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger('Cập nhật phiếu thu thất bại');
            },
            complete: function () {
                $("#modalPopupLapPhieuThu").modal("hide");
            }
        });
    }

    self.ChangeTienThu = function (item) {
        $('#lblTienMat_PhieuThu').text('(Tiền mặt)');
        var $this = $('#tienthu_' + item.ID);
        formatNumberObj($this);
        var tienThu = formatNumberToInt($this.val());
        if (isNaN(tienThu)) {
            tienThu = 0;
        }
        var tongTTHoaDon = 0;
        for (let i = 0; i < self.ListHDisDebit().length; i++) {
            if (self.ListHDisDebit()[i].ID === item.ID) {
                if (tienThu > self.ListHDisDebit()[i].TienMat) {
                    tienThu = self.ListHDisDebit()[i].TienMat;
                    $($this).val(formatNumber(tienThu));
                }
                self.ListHDisDebit()[i].TienThu = formatNumberToInt(tienThu);
            }
            tongTTHoaDon += self.ListHDisDebit()[i].TienThu;
        }
        $('#txtThuTuKhach').val(formatNumber(tongTTHoaDon));
        self.ThuTuKhach(formatNumber(tongTTHoaDon));
        self.TongThanhToan(tongTTHoaDon);
        self.TienMat_PhieuThu(tongTTHoaDon);
        self.TienATM_PhieuThu(0);
        self.TienGui_PhieuThu(0);
        self.TienTheGiaTri_PhieuThu(0);
        self.CongVaoTK(0);
        var noSau = Math.round(formatNumberToInt(self.ChiTietKH_PhieuThu().NoHienTai) - tongTTHoaDon);
        self.NoSau(noSau);
    }

    self.formatDateTime = function () {
        $('.datepicker_mask').datetimepicker(
            {
                format: "d/m/Y H:i",
                mask: true,
                timepicker: true,
                maxDate: new Date(),
            });
    }

    function CheckNgayLapHD(valDate) {

        var dateNow = moment(new Date()).format('YYYY-MM-DD HH:mm');
        var ngayLapHD = moment(valDate, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');

        if (valDate === '') {
            ShowMessage_Danger('Vui lòng nhập ngày lập hóa đơn');

            $('#txtNgayTaoHD').datetimepicker(
                {
                    format: "d/m/Y H:i",
                    defaultDate: new Date(),
                    mask: true,
                    maxDate: new Date(),
                });

            return false;
        }
        if (valDate.indexOf('_') > -1) {
            ShowMessage_Danger('Ngày lập hóa đơn chưa đúng định dạng');
            return false;
        }

        if (ngayLapHD > dateNow) {
            ShowMessage_Danger('Ngày lập hóa đơn  vượt quá thời gian hiện tại');
            return false;
        }

        if (self.ChotSo_ChiNhanh().length > 0) {
            var dtChotSo = moment(self.ChotSo_ChiNhanh()[0].NgayChotSo, 'YYYY-MM-DD HH:mm:ii').format('YYYY-MM-DD HH:mm');
            // if ngayLap < tgian chot so ==> warning
            if (ngayLapHD < dtChotSo) {
                ShowMessage_Danger('Ngày lập hóa đơn' + ' phải sau thời gian chốt sổ ' + moment(dtChotSo, 'YYYY- MM - DD HH:mm').format('DD/ MM / YYYY'));
                return false;
            }
        }

        return true;
    }

    self.ChangeNgayLapHD = function (x) {
        console.log('chanegNgay')

        var dtchange = moment($('#txtNgayTaoHD').val(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        var check = CheckNgayLapHD($('#txtNgayTaoHD').val());
        if (!check) {
            return;
        }

        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            if (_maHoaDon === '') {
                _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
            }

            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].ID_ViTri === _phongBanID && lstHD[i].MaHoaDon === _maHoaDon
                    && lstHD[i].ID_DonVi === _idDonVi) {
                    lstHD[i].NgayLapHoaDon = dtchange;
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
        }
    }

    function Enable_DisableNgayLapHD() {
        var itemRole = $.grep(self.Quyen_NguoiDung(), function (x) {
            return x.MaQuyen === 'HoaDon_ThayDoiThoiGian';
        });
        if (itemRole.length > 0) {
            $('#txtNgayTaoHD').removeAttr('disabled');
        }
        else {
            $('#txtNgayTaoHD').attr('disabled', 'disabled');
        }
    }

    // Tich Diem
    function GetHT_TichDiem() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CauHinh_TichDiemChiTiet?idDonVi=' + _idDonVi, 'GET').done(function (obj) {
                if (obj.res === true) {
                    let data = obj.data;
                    self.ThietLap_TichDiem(data);
                    WriteData_Dexie(db.HeThongTichDiem, data);
                }
            });
        }
        else {
            db.HeThongTichDiem.toArray(function (dt) {
                self.ThietLap_TichDiem(dt);
            });
        }
    }

    // use agreePay
    function UpdateDiemGiaoDich_forHoaDon(idRandomHD) {
        if (self.ThietLap().TinhNangTichDiem) {
            var lstHD = localStorage.getItem(lcListHD);
            if (lstHD !== null) {
                lstHD = JSON.parse(lstHD);

                if (_maHoaDon === '') {
                    _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
                }
                var phongbanID = _phongBanID;
                var itemHD = $.grep(lstHD, function (x) {
                    return x.IDRandom === idRandomHD;
                });

                if (itemHD.length > 0) {

                    var sumCTHD_notsale = 0;
                    var sumCTHD_sale = 0;
                    let sum_cthd0tichdiem = 0;
                    var idNhomDTs = '';
                    // if Khong tich diem cho Sp giam gia
                    if (self.ThietLap_TichDiem() != null && self.ThietLap_TichDiem().length > 0) {

                        var lstCTHD = localStorage.getItem(lcListCTHD);
                        if (lstCTHD !== null) {
                            lstCTHD = JSON.parse(lstCTHD);
                            // sum diem of product ducotichdiem =0
                            let cthd_notTichDiem = $.grep(lstCTHD, function (x) {
                                return x.DuocTichDiem === 0 && x.IDRandomHD === idRandomHD;
                            });
                            for (let i = 0; i < cthd_notTichDiem.length; i++) {
                                sum_cthd0tichdiem += cthd_notTichDiem[i].ThanhTien;
                            }

                            // only get product with duoctichdiem = 1
                            lstCTHD = $.grep(lstCTHD, function (x) {
                                return x.DuocTichDiem === 1;
                            });
                            // tinh tong tien cac hang khong giam gia cua HD Mua
                            for (let i = 0; i < lstCTHD.length; i++) {
                                if (lstCTHD[i].IDRandomHD === idRandomHD) {
                                    if (lstCTHD[i].TienChietKhau === 0) {
                                        sumCTHD_notsale += lstCTHD[i].ThanhTien;
                                    }
                                    if (lstCTHD[i].TienChietKhau > 0) {
                                        sumCTHD_sale += lstCTHD[i].ThanhTien;
                                    }
                                }
                            }
                        }

                        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0) {
                            if (self.ChiTietDoiTuong()[0].ID_NhomDoiTuong === null || self.ChiTietDoiTuong()[0].ID_NhomDoiTuong === undefined) {
                                idNhomDTs = '';
                            }
                            else {
                                idNhomDTs = self.ChiTietDoiTuong()[0].ID_NhomDoiTuong.toLowerCase();
                            }

                            var diemGD_ofHHsale = Math.floor(sumCTHD_sale / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                            let diemGD_cthd0tichdiem = Math.floor(sum_cthd0tichdiem / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                            var diemGD_ofHD = Math.floor((formatNumberToFloat(itemHD[0].TongTienHang) - formatNumberToFloat(itemHD[0].TongGiamGiaKM_HD)) / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                            var diemGiaoDich = diemGD_ofHD - diemGD_cthd0tichdiem;

                            // Tich Diem for all KH
                            if (self.ThietLap_TichDiem()[0].ToanBoKhachHang) {

                                // if Khong tich diem cho Sp giam gia
                                if (self.ThietLap_TichDiem()[0].TichDiemGiamGia) {

                                    // if Khong tich diem cho HD thanh toan = diem
                                    if (self.ThietLap_TichDiem()[0].TichDiemHoaDonDiemThuong) {

                                        // if Khong tich diem cho HD Giam Gia
                                        if (self.ThietLap_TichDiem()[0].TichDiemHoaDonGiamGia) {
                                            // check xem HD co dang duoc giam gia khong
                                            if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                // neu co Giam Gia --> khong update gi ca
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemGiaoDich = 0;
                                                        break;
                                                    }
                                                }
                                            }
                                            else {
                                                // check xem co TT = diem Khong
                                                // neu HD khong TT = diem --> update DiemGiaoDich for HangHoa khong giam gia
                                                if (itemHD[0].DiemQuyDoi === 0) {
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemGiaoDich = Math.floor(sumCTHD_notsale / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                                                            break;
                                                        }
                                                    }
                                                }
                                                else {
                                                    // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemGiaoDich = 0;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            // check xem co HD TT = diem Khong
                                            // neu HD khong TT = diem --> update DiemGiaoDich for HangHoa khong giam gia
                                            if (itemHD[0].DiemQuyDoi === 0) {
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        let diemGD = diemGiaoDich - diemGD_ofHHsale;
                                                        lstHD[i].DiemGiaoDich = diemGD > 0 ? diemGD : 0;
                                                        break;
                                                    }
                                                }
                                            }
                                            else {
                                                // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemGiaoDich = 0;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        if (self.ThietLap_TichDiem()[0].TichDiemHoaDonGiamGia) {
                                            // check Giam Gia in HD
                                            if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                // neu co Giam Gia --> khong update gi ca
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemGiaoDich = 0;
                                                        break;
                                                    }
                                                }
                                            }
                                            else {
                                                //  neu khong co GG --> update DiemQuiDoi +  SP khong giam gia
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemGiaoDich = Math.floor(sumCTHD_notsale / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            // Giong truong hop Khong co GG in HD
                                            // TichDiem cho HD TT = diem + SP khong giam gia (Van Tich diem cho HD tt = diem, nhung chi tich diem cho Sp giam gia)
                                            for (let i = 0; i < lstHD.length; i++) {
                                                if (lstHD[i].IDRandom === idRandomHD) {
                                                    let diemGD = diemGiaoDich - diemGD_ofHHsale;
                                                    lstHD[i].DiemGiaoDich = diemGD > 0 ? diemGD : 0;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else {
                                    // tich diem cho all SP
                                    if (self.ThietLap_TichDiem()[0].TichDiemHoaDonDiemThuong) {

                                        // Khong Tich Diem cho HD Giam Gia
                                        if (self.ThietLap_TichDiem()[0].TichDiemHoaDonGiamGia) {
                                            if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                // neu co Giam Gia --> khong update gi ca
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemGiaoDich = 0;
                                                        break;
                                                    }
                                                }
                                            }
                                            else {
                                                // check xem co TT = diem Khong
                                                // neu HD khong TT = diem --> update DiemGiaoDich for HangHoa khong giam gia
                                                if (itemHD[0].DiemQuyDoi === 0) {
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemGiaoDich = diemGiaoDich;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else {
                                                    // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemGiaoDich = 0;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            // Tich Diem HD Giam Gia = false

                                            if (itemHD[0].DiemQuyDoi === 0) {
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemGiaoDich = diemGiaoDich;
                                                        break;
                                                    }
                                                }
                                            }
                                            else {
                                                // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemGiaoDich = 0;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        // tich diem for all SP + HD tt = diem
                                        for (let i = 0; i < lstHD.length; i++) {
                                            if (lstHD[i].IDRandom === idRandomHD) {
                                                lstHD[i].DiemGiaoDich = diemGiaoDich;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                // Tich Diem Theo Nhom KH

                                for (let i = 0; i < self.ThietLap_TichDiem().length; i++) {
                                    // if KH thuoc nhom duoc tich diem
                                    if (idNhomDTs.indexOf(self.ThietLap_TichDiem()[i].ID_NhomDoiTuong) > -1) {
                                        // if Khong tich diem cho Sp giam gia
                                        if (self.ThietLap_TichDiem()[i].TichDiemGiamGia) {

                                            // if Khong tich diem cho HD thanh toan = diem
                                            if (self.ThietLap_TichDiem()[i].TichDiemHoaDonDiemThuong) {

                                                // if Khong tich diem cho HD Giam Gia
                                                if (self.ThietLap_TichDiem()[i].TichDiemHoaDonGiamGia) {
                                                    // check xem HD co dang duoc giam gia khong
                                                    if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                        // neu co Giam Gia --> khong update gi ca
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                lstHD[j].DiemGiaoDich = 0;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        // check xem co TT = diem Khong
                                                        // neu HD khong TT = diem --> update DiemGiaoDich for HangHoa khong giam gia
                                                        if (itemHD[0].DiemQuyDoi === 0) {
                                                            for (let j = 0; j < lstHD.length; j++) {
                                                                if (lstHD[j].IDRandom === idRandomHD) {
                                                                    lstHD[j].DiemGiaoDich = Math.round(sumCTHD_notsale / self.ThietLap_TichDiem()[i].TyLeDoiDiem);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                            for (let j = 0; j < lstHD.length; j++) {
                                                                if (lstHD[j].IDRandom === idRandomHD) {
                                                                    lstHD[j].DiemGiaoDich = 0;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else {
                                                    // check xem co HD TT = diem Khong
                                                    // neu HD khong TT = diem --> update DiemGiaoDich for HangHoa khong giam gia
                                                    if (itemHD[0].DiemQuyDoi === 0) {
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                let diemGD = diemGiaoDich - diemGD_ofHHsale;
                                                                lstHD[j].DiemGiaoDich = diemGD > 0 ? diemGD : 0;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                lstHD[j].DiemGiaoDich = 0;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else {
                                                if (self.ThietLap_TichDiem()[i].TichDiemHoaDonGiamGia) {
                                                    // check Giam Gia in HD
                                                    if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                        // neu co Giam Gia --> khong update gi ca
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                lstHD[j].DiemGiaoDich = 0;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        //  neu khong co GG --> update DiemQuiDoi +  SP khong giam gia
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                lstHD[j].DiemGiaoDich = Math.round(sumCTHD_notsale / self.ThietLap_TichDiem()[i].TyLeDoiDiem);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                else {
                                                    // Giong truong hop Khong co GG in HD
                                                    // TichDiem cho HD TT = diem + SP khong giam gia (Van Tich diem cho HD tt = diem, nhung chi tich diem cho Sp giam gia)
                                                    for (let j = 0; j < lstHD.length; j++) {
                                                        if (lstHD[j].IDRandom === idRandomHD) {
                                                            let diemGD = diemGiaoDich - diemGD_ofHHsale;
                                                            lstHD[j].DiemGiaoDich = diemGD > 0 ? diemGD : 0;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            // tich diem cho all SP
                                            if (self.ThietLap_TichDiem()[i].TichDiemHoaDonDiemThuong) {

                                                // Khong Tich Diem cho HD Giam Gia
                                                if (self.ThietLap_TichDiem()[i].TichDiemHoaDonGiamGia) {
                                                    if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                        // neu co Giam Gia --> khong update gi ca
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                lstHD[j].DiemGiaoDich = 0;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        // check xem co TT = diem Khong
                                                        // neu HD khong TT = diem --> update DiemGiaoDich for HangHoa khong giam gia
                                                        if (itemHD[0].DiemQuyDoi === 0) {
                                                            for (let j = 0; j < lstHD.length; j++) {
                                                                if (lstHD[j].IDRandom === idRandomHD) {
                                                                    lstHD[j].DiemGiaoDich = diemGiaoDich;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        else {
                                                            // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                            for (let j = 0; j < lstHD.length; j++) {
                                                                if (lstHD[j].IDRandom === idRandomHD) {
                                                                    lstHD[j].DiemGiaoDich = 0;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else {
                                                    // Tich Diem HD Giam Gia = false

                                                    if (itemHD[0].DiemQuyDoi === 0) {
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                lstHD[j].DiemGiaoDich = diemGiaoDich;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else {
                                                        // neu HD tt = diem --> update DiemQuyDoi, not update DiemGiaoDich
                                                        for (let j = 0; j < lstHD.length; j++) {
                                                            if (lstHD[j].IDRandom === idRandomHD) {
                                                                lstHD[j].DiemGiaoDich = 0;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else {
                                                // tich diem for all SP + HD tt = diem
                                                for (let j = 0; j < lstHD.length; j++) {
                                                    if (lstHD[j].IDRandom === idRandomHD) {
                                                        lstHD[j].DiemGiaoDich = diemGiaoDich;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break; // esc if Check ThietLap_TichDiem().ID_NhomDoiTuong === idNhomDTs
                                    }
                                }
                            }
                        }
                        else {
                            // khach le --> khong tich diem
                            for (let j = 0; j < lstHD.length; j++) {
                                if (lstHD[j].IDRandom === idRandomHD) {
                                    lstHD[j].DiemGiaoDich = 0;
                                    break;
                                }
                            }
                        }
                        localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                    }
                }
            }
        }
        $('#lblTienMat').text('(Tiền mặt)');
    }

    // update DiemGiaoDich, but reset DiemQuyDoi = 0, TTBangDiem = 0 (use edit/tang/giam so luong)
    function UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD) {
        if (self.ThietLap().TinhNangTichDiem) {
            var lstHD = localStorage.getItem(lcListHD);
            if (lstHD !== null) {
                lstHD = JSON.parse(lstHD);
                var itemHD = $.grep(lstHD, function (x) {
                    return x.IDRandom === idRandomHD;
                });

                if (itemHD.length > 0) {

                    var sumCTHD_sale = 0;
                    var sumCTHD_notsale = 0;
                    let sum_cthd0tichdiem = 0;
                    var idNhomDTs = '';
                    // if Khong tich diem cho Sp giam gia
                    if (self.ThietLap_TichDiem() != null && self.ThietLap_TichDiem().length > 0) {

                        var lstCTHD = localStorage.getItem(lcListCTHD);
                        if (lstCTHD !== null) {
                            lstCTHD = JSON.parse(lstCTHD);

                            // sum diem of product ducotichdiem =0
                            let cthd_notTichDiem = $.grep(lstCTHD, function (x) {
                                return x.DuocTichDiem === 0;
                            });
                            for (let i = 0; i < cthd_notTichDiem.length; i++) {
                                sum_cthd0tichdiem += cthd_notTichDiem[i].ThanhTien;
                            }

                            // only get product with duoctichdiem = 1
                            lstCTHD = $.grep(lstCTHD, function (x) {
                                return x.DuocTichDiem === 1;
                            });

                            // tinh tong tien cac hang hoa co giam gia cua HD Mua
                            for (let i = 0; i < lstCTHD.length; i++) {
                                if (lstCTHD[i].IDRandomHD === idRandomHD) {
                                    if (lstCTHD[i].TienChietKhau > 0) {
                                        sumCTHD_sale += lstCTHD[i].ThanhTien;
                                    }
                                    if (lstCTHD[i].TienChietKhau === 0) {
                                        sumCTHD_notsale += lstCTHD[i].ThanhTien;
                                    }
                                }
                            }
                        }

                        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0) {
                            if (self.ChiTietDoiTuong()[0].ID_NhomDoiTuong === null || self.ChiTietDoiTuong()[0].ID_NhomDoiTuong === undefined) {
                                idNhomDTs = '';
                            }
                            else {
                                idNhomDTs = self.ChiTietDoiTuong()[0].ID_NhomDoiTuong.toLowerCase();
                            }

                            var diemGD_ofHHsale = Math.floor(sumCTHD_sale / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                            let diemGD_cthd0tichdiem = Math.floor(sum_cthd0tichdiem / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                            // diem GD cua HoaDon
                            var diemGD_ofHD = Math.floor((formatNumberToFloat(itemHD[0].TongTienHang) - formatNumberToFloat(itemHD[0].TongGiamGiaKM_HD)) / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                            // diem GD sau khi tru diem cua ct khong tichdiem
                            var diemGiaoDich = diemGD_ofHD - diemGD_cthd0tichdiem;

                            // Tich Diem for all KH
                            if (self.ThietLap_TichDiem()[0].ToanBoKhachHang) {

                                // if Khong tich diem cho Sp giam gia
                                if (self.ThietLap_TichDiem()[0].TichDiemGiamGia) {
                                    // if Khong tich diem cho HD Giam Gia
                                    if (self.ThietLap_TichDiem()[0].TichDiemHoaDonGiamGia) {
                                        // check xem HD co dang duoc giam gia khong
                                        if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                            // neu co Giam Gia --> khong update gi ca
                                            for (let i = 0; i < lstHD.length; i++) {
                                                if (lstHD[i].IDRandom === idRandomHD) {
                                                    lstHD[i].DiemQuyDoi = 0;
                                                    lstHD[i].TTBangDiem = 0;
                                                    lstHD[i].DiemGiaoDich = 0;
                                                    break;
                                                }
                                            }
                                        }
                                        else {
                                            for (let i = 0; i < lstHD.length; i++) {
                                                if (lstHD[i].IDRandom === idRandomHD) {
                                                    lstHD[i].DiemQuyDoi = 0;
                                                    lstHD[i].TTBangDiem = 0;
                                                    // diemGD = diem cua HH khong co GG (OK)
                                                    lstHD[i].DiemGiaoDich = Math.floor(sumCTHD_notsale / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        // tich diem cho ca HD giam gia --> update DiemGD for HH khong giam gia
                                        for (let i = 0; i < lstHD.length; i++) {
                                            if (lstHD[i].IDRandom === idRandomHD) {
                                                lstHD[i].DiemQuyDoi = 0;
                                                lstHD[i].TTBangDiem = 0;
                                                let diemGD = diemGiaoDich - diemGD_ofHHsale;
                                                lstHD[i].DiemGiaoDich = diemGD > 0 ? diemGD : 0;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else {
                                    // tich diem cho all SP

                                    // Khong Tich Diem cho HD Giam Gia
                                    if (self.ThietLap_TichDiem()[0].TichDiemHoaDonGiamGia) {
                                        if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                            // neu co Giam Gia --> khong update gi ca
                                            for (let i = 0; i < lstHD.length; i++) {
                                                if (lstHD[i].IDRandom === idRandomHD) {
                                                    lstHD[i].DiemQuyDoi = 0;
                                                    lstHD[i].TTBangDiem = 0;
                                                    lstHD[i].DiemGiaoDich = 0;
                                                    break;
                                                }
                                            }
                                        }
                                        else {
                                            // HD khong giam gia: update DiemGiaoDich for all HangHoa
                                            for (let i = 0; i < lstHD.length; i++) {
                                                if (lstHD[i].IDRandom === idRandomHD) {
                                                    lstHD[i].DiemQuyDoi = 0;
                                                    lstHD[i].TTBangDiem = 0;
                                                    // khong can check loaiHD = 6
                                                    lstHD[i].DiemGiaoDich = diemGiaoDich;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else {
                                        // Tich Diem HD Giam Gia = false

                                        for (let i = 0; i < lstHD.length; i++) {
                                            if (lstHD[i].IDRandom === idRandomHD) {
                                                lstHD[i].DiemQuyDoi = 0;
                                                lstHD[i].TTBangDiem = 0;
                                                lstHD[i].DiemGiaoDich = diemGiaoDich;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else {
                                // Tich Diem Theo Nhom KH

                                for (let j = 0; j < self.ThietLap_TichDiem().length; j++) {
                                    // if KH thuoc nhom duoc tich diem
                                    if (idNhomDTs.indexOf(self.ThietLap_TichDiem()[j].ID_NhomDoiTuong) > -1) {

                                        // if Khong tich diem cho Sp giam gia
                                        if (self.ThietLap_TichDiem()[0].TichDiemGiamGia) {

                                            // if Khong tich diem cho HD Giam Gia
                                            if (self.ThietLap_TichDiem()[0].TichDiemHoaDonGiamGia) {
                                                // check xem HD co dang duoc giam gia khong
                                                if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                    // neu co Giam Gia --> khong update gi ca
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemQuyDoi = 0;
                                                            lstHD[i].TTBangDiem = 0;
                                                            lstHD[i].DiemGiaoDich = 0;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else {
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemQuyDoi = 0;
                                                            lstHD[i].TTBangDiem = 0;
                                                            lstHD[i].DiemGiaoDich = Math.floor(sumCTHD_notsale / self.ThietLap_TichDiem()[0].TyLeDoiDiem);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            else {
                                                // tich diem cho ca HD giam gia --> update DiemGD for HH khong giam gia
                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemQuyDoi = 0;
                                                        lstHD[i].TTBangDiem = 0;
                                                        lstHD[i].DiemGiaoDich = diemGiaoDich - diemGD_ofHHsale;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            // tich diem cho all SP

                                            // Khong Tich Diem cho HD Giam Gia
                                            if (self.ThietLap_TichDiem()[0].TichDiemHoaDonGiamGia) {
                                                if (itemHD[0].TongGiamGiaKM_HD > 0) {
                                                    // neu co Giam Gia --> khong update gi ca
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemQuyDoi = 0;
                                                            lstHD[i].TTBangDiem = 0;
                                                            lstHD[i].DiemGiaoDich = 0;
                                                            break;
                                                        }
                                                    }
                                                }
                                                else {
                                                    // HD khong giam gia: update DiemGiaoDich for all HangHoa
                                                    for (let i = 0; i < lstHD.length; i++) {
                                                        if (lstHD[i].IDRandom === idRandomHD) {
                                                            lstHD[i].DiemQuyDoi = 0;
                                                            lstHD[i].TTBangDiem = 0;
                                                            lstHD[i].DiemGiaoDich = diemGiaoDich;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            else {
                                                // Tich Diem HD Giam Gia = false

                                                for (let i = 0; i < lstHD.length; i++) {
                                                    if (lstHD[i].IDRandom === idRandomHD) {
                                                        lstHD[i].DiemQuyDoi = 0;
                                                        lstHD[i].TTBangDiem = 0;
                                                        lstHD[i].DiemGiaoDich = diemGiaoDich;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        break; // esc if Check ThietLap_TichDiem().ID_NhomDoiTuong === idNhomDT
                                    }
                                }
                            }
                        }
                        else {
                            // khach le --> khong tich diem
                            for (let j = 0; j < lstHD.length; j++) {
                                if (lstHD[j].IDRandom === idRandomHD) {
                                    lstHD[j].TTBangDiem = 0;
                                    lstHD[j].DiemQuyDoi = 0;
                                    lstHD[j].DiemGiaoDich = 0;
                                    break;
                                }
                            }
                        }
                        localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                    }
                }
            }
        }
        $('#lblTienMat').text('(Tiền mặt)');
    }

    function ResetTextSearch() {
        $('#inputFocus').val('');
        $('#txtSearchKH').val('');
    }

    self.modelViTri = ko.observable();
    self.filterViTrichanged = function () {
        var value = $(event.target).val();
        self.modelViTri(value);
        FilterViTri();
    }
    function FilterViTri() {
        var value = locdau(self.modelViTri());
        var data1 = self.AllPhongBans().filter(function (item) {
            return SearchTxt_inVue(value.split(" "), locdau(item['TenViTri'])) === true;
        });
        self.PhongBans(data1);
    }

    // Caculator price
    self.itemCTHD_clickGiaBan = ko.observableArray(); // get item CTHD when click GiaBan;
    self.IsClickNewPrice_computer = ko.observable(true); // dang click vao newprice computer, default = true

    self.clickGiaBan = function (item) {
        if (_maHoaDon.indexOf('O') > -1) {
            ShowMessage_Danger('Hóa đơn đã được thanh toán ofline. Không được phép thay đổi');
            return false;
        }
        self.itemCTHD_clickGiaBan(item);

        var idRandom = item.IDRandom;
        var $this = $(event.currentTarget);
        var ipGiam = $('#pri-g_' + idRandom);

        var itemCTHD = FindCTHD_isDoing(idRandom);
        if (itemCTHD !== null) {
            let ptGiam = itemCTHD.PTChietKhau;
            let tienGiam = itemCTHD.TienChietKhau;
            let giaban = itemCTHD.GiaBan;
            let dvt = itemCTHD.DVTinhGiam;

            // bind data
            if (self.windowWidth() >= 1024) {
                $this.next(".arrow-left1").toggle();

                if (dvt === '%') {
                    ipGiam.val(ptGiam);
                    $('#vnd_' + idRandom).removeClass('gb');
                    $('#novnd_' + idRandom).addClass('gb');
                }
                else {
                    ipGiam.val(formatNumber(tienGiam));
                    $('#novnd_' + idRandom).removeClass('gb');
                    $('#vnd_' + idRandom).addClass('gb');
                }
                $('input[id=' + idRandom + ']').val(formatNumber(giaban));
                $('input[id=' + idRandom + ']').focus().select();
            }
            else {
                $('#computersquer').modal('show');
                if (dvt === '%') {
                    $('#computer-gg').val(ptGiam);
                    $('.toogle-report').addClass('active-re');
                }
                else {
                    $('#computer-gg').val(formatNumber(tienGiam));
                    $('.toogle-report').removeClass('active-re');
                }
                $('#computer_newPrice').val(formatNumber(giaban));
            }
        }
    }

    self.editPrice_computer = function () {
        var ctDoing = FindCTHD_isDoing(self.itemCTHD_clickGiaBan().IDRandom);
        if (ctDoing !== null) {
            var thisObj = $('#computer_newPrice');
            var objSale = $('#computer-gg');
            formatNumberObj(thisObj);
            var priceNew = formatNumberToFloat(thisObj.val());
            var priceOld = ctDoing.DonGia;
            var isPTram = $('#divOnOffVND').hasClass('active-re');
            var tienGiam = ctDoing.TienChietKhau;

            if (priceNew < priceOld) {
                tiengiam = priceOld - priceNew;
                if (isPTram) {
                    ptGiam = RoundDecimal(tiengiam / priceOld * 100);
                    objSale.val(ptGiam);
                }
                else {
                    objSale.val(formatNumber(tiengiam));
                }
            }
            else {
                objSale.val(0);
            }
        }
    }

    self.editSale_computer = function () {
        var ctDoing = FindCTHD_isDoing(self.itemCTHD_clickGiaBan().IDRandom);
        if (ctDoing !== null) {
            var thisObj = $('#computer-gg');
            var objPrice = $('#computer_newPrice');
            formatNumberObj(thisObj);

            var priceNew = formatNumberToFloat(thisObj.val());
            var priceOld = ctDoing.DonGia;
            var isPTram = $('#divOnOffVND').hasClass('active-re');
            var tienGiam = ctDoing.TienChietKhau;
            var giamNew = 0;

            // neu gia cu = 0 => khong cho phep nhap giam gia
            if (priceOld === 0) {
                thisObj.val(0);
            }
            else {
                formatNumberObj(thisObj);
                giamNew = formatNumberToFloat(thisObj.val());

                if (isPTram === false) {
                    // nhap giamgia > giaban
                    if (giamNew > priceOld) {
                        tiengiam = priceOld;
                        $(thisObj).val(formatNumber(priceOld));
                    }
                    else {
                        tiengiam = giamNew;
                    }
                }
                else {
                    if (giamNew > 100) {
                        tiengiam = priceOld;
                        $(thisObj).val(100);
                    }
                    else {
                        tiengiam = Math.round(giamNew * priceOld / 100);
                    }
                }

                // update giamgia, dongia, thanhtien
                let priceNew = Math.round(priceOld - tiengiam);
                objPrice.val(formatNumber(priceNew));
            }
        }
    }

    // click input NewPrice_computer
    self.clickNewPrice_computer = function () {
        self.IsClickNewPrice_computer(true);
        $('#computer_newPrice').focus();
        $('#computer_newPrice').select();
    }

    // click input sale_computer
    self.clickSale_computer = function () {
        self.IsClickNewPrice_computer(false);
        $('#computer-gg').focus();
        $('#computer-gg').select();
    }

    // set value for input when click number
    self.clickNumber = function (number) {
        var cpt_newPrice = $('#computer_newPrice');
        var cpt_sale = $('#computer-gg');
        var isPTram = $('#divOnOffVND').hasClass('active-re');

        // get infor from CTHD old
        var priceOld = self.itemCTHD_clickGiaBan().DonGia;
        var priceNew = 0;
        var tienGiam = 0;

        var newVal = '';
        // if is focus newprice
        if (self.IsClickNewPrice_computer()) {
            // get value at input #computer_newPrice, after concat string (nối chuỗi)
            newVal = cpt_newPrice.val() + number;

            // convert price to int
            var price = formatNumberToInt(newVal);

            priceNew = Math.round(price);

            if (priceNew < priceOld) {
                tienGiam = priceOld - priceNew;
            }

            // assign again sale
            if (isPTram === false) {
                cpt_sale.val(formatNumber(tienGiam));
            }
            else {
                cpt_sale.val(RoundDecimal(tienGiam / priceOld * 100));
            }
            cpt_newPrice.val(formatNumber(priceNew));
        }
        else {

            // neu gia cu = 0 => khong cho phep nhap giam gia
            if (priceOld === 0) {
                cpt_sale.val(0);
            }
            else {
                // tim den vi tri chua dau .
                var indexComma = cpt_sale.val().indexOf('.');
                if (indexComma > -1) {
                    // lay do dai chuoi - 1
                    var len = cpt_sale.val().length - 1;

                    // dem so ki tu sau dau .
                    // neu == 3 --> khong cong chuoi nua
                    if (len - indexComma < 3) {

                        // concat string (nối chuỗi)
                        newVal = cpt_sale.val() + number;

                        // 000,000 --> 000 000
                        newVal = newVal.toString().replace(/,/g, '');

                        // if last char of string ='.' --> not format to float
                        var lastChar = newVal[newVal.length - 1];
                        if (lastChar !== '.') {
                            newVal = formatNumberToFloat(newVal);
                        }

                        // assign again value for input
                        cpt_sale.val(formatNumber(newVal));
                    }
                }
                else {
                    // concat string (nối chuỗi)
                    newVal = cpt_sale.val() + number;

                    // 000,000 --> 000 000
                    newVal = newVal.toString().replace(/,/g, '');

                    // if last char of string ='.' --> not format to float
                    var lastChar = newVal[newVal.length - 1];
                    if (lastChar !== '.') {
                        newVal = formatNumberToFloat(newVal);
                    }

                    // assign again value for input
                    cpt_sale.val(formatNumber(newVal));
                }

                // neu clear giamgia ='';
                var valGG = cpt_sale.val();
                if (valGG === '') {
                    valGG = 0;
                }

                // caculator again newPrice (same editSale_computer)
                if (isPTram === false) {
                    // neu giam gia > gia cu
                    if (formatNumberToInt(valGG) > priceOld) {
                        cpt_sale.val(formatNumber(priceOld));
                        tienGiam = priceOld;
                    }
                    else {
                        // round VD: 20.45 --> 20; 20.51 --> 21
                        tienGiam = Math.round(formatNumberToFloat(valGG));
                    }
                }
                else {
                    // neu giam gia > 100 %
                    if (formatNumberToFloat(valGG) > 100) {
                        cpt_sale.val(100);
                        tienGiam = priceOld;
                    }
                    else {
                        tienGiam = Math.round(priceOld * formatNumberToFloat(valGG) / 100);
                    }
                }
                priceNew = priceOld - tienGiam;
                cpt_newPrice.val(formatNumber(priceNew));
            }
        }
    }

    self.deleteNumber = function () {
        var cpt_newPrice = $('#computer_newPrice');
        var cpt_sale = $('#computer-gg');
        var isPTram = $('#divOnOffVND').hasClass('active-re');

        var newVal = '';
        var oldVal = '';
        var priceOld = self.itemCTHD_clickGiaBan().DonGia;
        var priceNew = 0;
        var tienGiam = 0;

        if (self.IsClickNewPrice_computer()) {

            // get old string
            oldVal = cpt_newPrice.val();

            // remove last char of old string
            newVal = oldVal.substr(0, oldVal.length - 1);

            // format again newValue
            newVal = newVal.replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");

            // tinh lai giam gia (same edit price)
            var price = formatNumberToInt(newVal);

            priceNew = Math.round(price);

            if (priceNew < priceOld) {
                tienGiam = priceOld - priceNew;
            }

            // assign again sale
            if (isPTram === false) {
                cpt_sale.val(formatNumber(tienGiam));
            }
            else {
                cpt_sale.val(RoundDecimal(tienGiam / priceOld * 100));
            }
            cpt_newPrice.val(formatNumber(priceNew));
        }
        else {

            oldVal = cpt_sale.val();

            newVal = oldVal.substr(0, oldVal.length - 1);

            newVal = newVal.replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");

            // if last char ='.' --> remove
            var lastChar = newVal[newVal.length - 1];
            if (lastChar === '.') {
                newVal = newVal.substr(0, newVal.length - 1);
            }

            cpt_sale.val(newVal);

            // caculator again newPrice, sale
            // neu clear giamgia ='';
            var valGiamGia = cpt_sale.val();
            if (valGiamGia === '') {
                valGiamGia = 0;
            }

            // clickVND: global variable
            if (isPTram === false) {
                // neu giam gia > gia cu
                if (formatNumberToInt(valGiamGia) > priceOld) {
                    cpt_sale.val(formatNumber(priceOld));
                }

                // round VD: 20.45 --> 20; 20.51 --> 21
                tienGiam = Math.round(formatNumberToInt(valGiamGia));
            }
            else {
                // neu giam gia > 100 %
                if (formatNumberToFloat(valGiamGia) > 100) {
                    cpt_sale.val(100);
                }
                tienGiam = Math.round(priceOld * parseFloat(valGiamGia) / 100);
            }
            priceNew = priceOld - tienGiam;
            cpt_newPrice.val(formatNumber(priceNew));
        }
    }

    self.ChangeDVT_computer = function () {
        var ctDoing = FindCTHD_isDoing(self.itemCTHD_clickGiaBan().IDRandom);
        if (ctDoing !== null) {
            var objSale = $('#computer-gg');
            var priceOld = self.itemCTHD_clickGiaBan().DonGia;
            var giamgia = objSale.val();
            var tienGiam = 0;
            var isPercent = $('#divOnOffVND').hasClass('active-re');

            // truoc do la %
            if (isPercent) {
                tienGiam = Math.round(parseFloat(giamgia) * priceOld / 100);
                objSale.val(formatNumber(tienGiam));
            }
            else {
                // truoc do la VND
                var div = formatNumberToInt(giamgia) / priceOld * 100;
                objSale.val(RoundDecimal(div));
            }
        }
    }

    self.agreeNewPrice_computer = function () {
        var isPTram = $('#divOnOffVND').hasClass('active-re');
        var cthdIsChose = self.itemCTHD_clickGiaBan();

        var newPrice = formatNumberToInt($('#computer_newPrice').val());
        var newSale = $('#computer-gg').val();

        var tienGiam = 0;
        var ptGiam = 0;
        var dvtGiam = 'VND';
        var priceOld = cthdIsChose.DonGia;
        var idRandomHD = cthdIsChose.IDRandomHD;

        if (isPTram === false) {
            tienGiam = formatNumberToInt(newSale);
            ptGiam = 0;
            dvtGiam = 'VND';
        }
        else {
            ptGiam = parseFloat(newSale);
            tienGiam = Math.round(ptGiam * priceOld / 100);
            dvtGiam = '%';
        }

        var lstCTHD = localStorage.getItem(lcListCTHD);
        if (lstCTHD !== null) {
            lstCTHD = JSON.parse(lstCTHD);

            for (let i = 0; i < lstCTHD.length; i++) {
                if (lstCTHD[i].IDRandomHD === idRandomHD
                    && lstCTHD[i].ID_DonViQuiDoi === cthdIsChose.ID_DonViQuiDoi) {
                    // update giaban, giamgia, thanh tien, dvt
                    lstCTHD[i].GiaBan = newPrice;
                    lstCTHD[i].PTChietKhau = ptGiam;
                    lstCTHD[i].TienChietKhau = tienGiam;
                    lstCTHD[i].DVTinhGiam = dvtGiam;
                    lstCTHD[i].ThanhTien = Math.round(lstCTHD[i].SoLuong * newPrice);
                    break;
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));

            // bind CTHD
            var ctHD = $.grep(lstCTHD, function (x) {
                return x.IDRandomHD === idRandomHD;
            });
            self.HangHoaAfterAdds(ctHD);

            UpdateHD_whenChangeHD(idRandomHD);
            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
            Update_TienThue_CTHD(idRandomHD);

            AssignValue_ofHoaDon(idRandomHD);
        }

        $('.price-pay-end').select();
        $('#computersquer').modal('hide');
    }

    // Caculator Number of Product
    self.ShowComputerQuantity = function (item) {
        if (self.windowWidth() < 1024) {
            $('#mdCaculatorQuatity').modal('show');
            $('#txtQuantity').val(formatNumberToFloat(item.SoLuong.toString()));

            self.itemCTHD_clickGiaBan(item);
        }
    }

    self.editNumber_Quantity = function () {

        var thisObj = $('#txtQuantity');

        var keyCode = event.keyCode || event.which;
        // neu dang go phim back
        if (keyCode === 8) {
            var newVal = thisObj.val();
            // if last char ='.' --> remove
            var lastChar = newVal[newVal.length - 1];
            if (lastChar === '.') {
                newVal = newVal.substr(0, newVal.length - 1);
                thisObj.val(newVal);
            }
        }

        else {
            formatNumberObj(thisObj);
        }

        var sluongOld = formatNumberToFloat(thisObj.val());

        // press up
        if (keyCode === 38) {
            newNumber = sluongOld + 1;
            thisObj.val(formatNumber(newNumber));
        }
        // press down
        if (keyCode === 40) {
            if (sluongOld > 1) {
                newNumber = sluongOld - 1;
                thisObj.val(formatNumber(newNumber));
            }
        }
    }

    // tang
    self.increase_Quantity = function () {
        var thisObj = $('#txtQuantity');

        var newQuantity = formatNumberToFloat(thisObj.val()) + 1;

        thisObj.val(formatNumber(newQuantity));
    }
    // giam
    self.reduce_Quantity = function () {
        var thisObj = $('#txtQuantity');

        var newQuantity = formatNumberToFloat(thisObj.val()) - 1;
        if (newQuantity <= 0) {
            thisObj.val(1);
        }
        else {
            thisObj.val(formatNumber(newQuantity));
        }
    }

    self.clear_Quantity = function () {

        var thisObj = $('#txtQuantity');
        var oldVal = thisObj.val();
        var newVal = '';

        newVal = oldVal.substr(0, oldVal.length - 1);

        newVal = newVal.replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");

        // if last char ='.' --> remove
        var lastChar = newVal[newVal.length - 1];
        if (lastChar === '.') {
            newVal = newVal.substr(0, newVal.length - 1);
        }

        thisObj.val(newVal);

        // caculator again newPrice, sale
        // neu clear giamgia ='';
        if (formatNumberToInt(thisObj.val()) === 0) {
            thisObj.val(0);
        }
        else {
            thisObj.val(formatNumber(newVal));
        }
    }

    self.clickNumber_Quantity = function (number) {
        var thisObj = $('#txtQuantity');
        var newVal = '';

        // find at location contain '.'
        var indexComma = thisObj.val().indexOf('.');
        if (indexComma > -1) {
            // get this length - 1
            var len = thisObj.val().length - 1;

            // count chars at before .
            // if count == 3 --> not concat string
            if (len - indexComma < 3) {

                // concat string (nối chuỗi)
                newVal = thisObj.val() + number;

                // 000,000 --> 000 000
                newVal = newVal.toString().replace(/,/g, '');

                // if last char of string ='.' --> not format to float
                var lastChar = newVal[newVal.length - 1];
                if (lastChar !== '.') {
                    newVal = formatNumberToFloat(newVal);
                }

                // assign again value for input
                thisObj.val(formatNumber(newVal));
            }
        }
        else {
            // concat string (nối chuỗi)
            newVal = thisObj.val() + number;

            // 000,000 --> 000 000
            newVal = newVal.toString().replace(/,/g, '');

            // if last char of string ='.' --> not format to float
            var lastChar = newVal[newVal.length - 1];
            if (lastChar !== '.') {
                newVal = formatNumberToFloat(newVal);
            }

            // assign again value for input
            thisObj.val(formatNumber(newVal));
        }
    }

    self.agree_Quantity = function () {
        $('#mdCaculatorQuatity').modal('hide');

        var thisObj = $('#txtQuantity');

        var itemChose = self.itemCTHD_clickGiaBan();
        var idRandomHD = itemChose.IDRandomHD;

        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            var lstCTHD = localStorage.getItem(lcListCTHD);
            if (lstCTHD !== null) {
                lstCTHD = JSON.parse(lstCTHD);

                for (let i = 0; i < lstCTHD.length; i++) {
                    if (lstCTHD[i].IDRandomHD === idRandomHD
                        && lstCTHD[i].ID_DonViQuiDoi === itemChose.ID_DonViQuiDoi) {
                        lstCTHD[i].SoLuong = formatNumberToFloat(thisObj.val());
                        lstCTHD[i].ThanhTien = lstCTHD[i].SoLuong * lstCTHD[i].GiaBan;
                        if (lstCTHD[i].PTThue > 0) {
                            lstCTHD[i].TienThue = lstCTHD[i].PTThue * lstCTHD[i].ThanhTien / 100;
                        }
                        break;
                    }
                }

                localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));
            }

            var sum = Sum_ThanhTienCTHD(lstCTHD, idRandomHD);
            UpdateHD_whenChangeCTHD(sum, idRandomHD);
            UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
            Update_TienThue_CTHD(idRandomHD);

            // bind CTHoaDOn
            var ctHD = $.grep(lstCTHD, function (x) {
                return x.IDRandomHD === idRandomHD;
            });
            self.HangHoaAfterAdds(ctHD);

            // bind infor HoaDon 
            //AssignValue_ofHoaDon(itemChose.MaHoaDon, itemChose.ID_ViTri);
            AssignValue_ofHoaDon(idRandomHD);
        }
    }

    function Insert_NhatKyThaoTac(objHD, objCTHD, chucNang, loaiNhatKy) {
        // chuc nang (1. TaoHD/HuyHD, 2.ThemKH, 3.TaoPhieuThu, )

        var tenChucNang = '';
        var noiDung = '';
        var noiDungChiTiet = '';
        var txtFirst = '';

        var style1 = '<a style= \"cursor: pointer\" onclick = \"';
        var style2 = "('";
        var style3 = "')\" >";
        var style4 = '</a>';

        var funcNameKH = 'LoadKhachHang_byMaKH';
        var funcNameHD = 'LoadHoaDon_byMaHD';
        var funcNameHH = 'loadHangHoabyMaHH';

        var maHD = objHD.MaHoaDon;
        var styleMaHD = ''.concat(style1, funcNameHD, style2, maHD, style3, maHD, style4);

        var maKH = '';
        var sKH = '';
        var styleKH = '';
        var styleHDGoc = '';

        // get MaKhachHang by ID
        var itemKH = $.grep(self.DoiTuongs(), function (x) {
            return x.ID === objHD.ID_DoiTuong;
        });

        if (itemKH.length > 0) {
            maKH = itemKH[0].MaDoiTuong;
            sKH = ', Khách hàng: ' + maKH + ', ';
            styleKH = ', Khách hàng: '.concat(style1, funcNameKH, style2, maKH, style3, maKH, style4, ', ');
        }
        else {
            sKH = ', Khách hàng: Khách lẻ, ';
        }

        var ngaylapHD = moment(GetNgayLapHD_whenSave(objHD.NgayLapHoaDon), 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
        var phaiTT = 0;

        if (chucNang === 1) {

            var maHDGoc = '';
            var sHDGoc = '';
            var tenBG = 'Bảng giá: Bảng giá chung, ';

            // get TenBangGia by ID
            var itemBangGia = $.grep(self.GiaBans(), function (x) {
                return x.ID === objHD.ID_BangGia;
            });

            if (itemBangGia.length > 0) {
                tenBG = 'Bảng giá: ' + itemBangGia[0].TenGiaBan + ', ';
            }

            // get chi tiet hang hoa
            var styleHangHoa = '';
            var lenCTHD = objCTHD.length;
            for (let i = 0; i < lenCTHD; i++) {
                var itemCTHD = objCTHD[i];
                styleHangHoa += '- '.concat(style1, funcNameHH, style2, itemCTHD.MaHangHoa, style3, itemCTHD.MaHangHoa, style4, ': Số lượng: ', itemCTHD.SoLuong, ', Giá bán: ', formatNumber(itemCTHD.GiaBan), '<br />');
                if (i > 61) {
                    styleHangHoa += 'vv.....';
                    break;
                }
            }

            phaiTT = formatNumber(objHD.PhaiThanhToan);

            var styleKhuyenMai = '';
            switch (objHD.LoaiHoaDon) {
                case 1:
                    tenChucNang = 'Bán hàng';
                    txtFirst = 'Tạo hóa đơn: ';

                    if (objHD.ID_HoaDon !== null && objHD.ID_HoaDon !== undefined) {
                        maHDGoc = objHD.MaHoaDonTraHang;

                        // neu HD DoiTra, objHD.ID_HoaDon !== null but maHDGoc = undefined
                        if (maHDGoc !== undefined) {
                            sHDGoc = ' (cho đơn đặt hàng: ' + maHDGoc + ') ';
                            styleHDGoc = ' (cho đơn đặt hàng: '.concat(style1, funcNameHD, style2, maHDGoc, style3, maHDGoc, style4, ') ');
                        }
                    }

                    if (objHD.KhuyenMai_GhiChu !== '' && objHD.KhuyenMai_GhiChu !== undefined) {
                        styleKhuyenMai = '<br /> Khuyến mại <i class="fa fa-gift bg-pink"></i><br />' + objHD.KhuyenMai_GhiChu;
                    }
                    break;
                case 3:
                    tenChucNang = 'Đặt hàng';
                    txtFirst = 'Tạo đơn đặt hàng: ';
                    break;
                case 6:
                    tenChucNang = 'Trả hàng';
                    txtFirst = 'Tạo phiếu trả hàng: ';

                    if (objHD.ID_HoaDon !== null && objHD.ID_HoaDon !== undefined) {
                        maHDGoc = objHD.MaHoaDonTraHang;
                        sHDGoc = ' (cho hóa đơn: ' + maHDGoc + ') ';
                        styleHDGoc = ' (cho hóa đơn: '.concat(style1, funcNameHD, style2, maHDGoc, style3, maHDGoc, style4, ') ');
                    }
                    break;
            }

            noiDung = txtFirst.concat(maHD, sHDGoc, sKH, tenBG, ' Giá trị: ', phaiTT, ', Thời gian: ', ngaylapHD);

            noiDungChiTiet = txtFirst.concat(styleMaHD, styleHDGoc, styleKH,
                tenBG, ' Giá trị: ', phaiTT, ', Thời gian: ', ngaylapHD,
                ' bao gồm: <br />', styleHangHoa, styleKhuyenMai);
        }

        if (chucNang === 2) {
            tenChucNang = 'Khách hàng';
            txtFirst = 'Thêm mới khách hàng: ';
            if (loaiNhatKy === 2) {
                txtFirst = 'Cập nhật khách hàng: ';
            }
            noiDung = txtFirst.concat(objHD.MaDoiTuong, ', tên: ', objHD.TenDoiTuong);
            noiDungChiTiet = txtFirst.concat(style1, funcNameKH, style2, objHD.MaDoiTuong, style3, objHD.MaDoiTuong, style4, ', tên: ', objHD.TenDoiTuong);
        }

        if (chucNang === 3) {
            phaiTT = formatNumber(objHD.TongTienThu);
            maHDGoc = objHD.MaHoaDonTraHang;

            if (maHDGoc !== null) {
                styleHDGoc = ' cho hóa đơn: '.concat(style1, funcNameHD, style2, objHD.MaHoaDonTraHang, style3, objHD.MaHoaDonTraHang, style4, ' ');
            }

            // 11.Thu, 12.Chi
            if (objHD.LoaiHoaDon === 11) {
                tenChucNang = 'Phiếu thu';
                txtFirst = 'Tạo phiếu thu: ';
            }
            else {
                tenChucNang = 'Phiếu chi';
                txtFirst = 'Tạo phiếu chi: ';
            }

            if (maHDGoc !== null) {
                noiDung = txtFirst.concat(maHD, ' cho hóa đơn ', objHD.MaHoaDonTraHang, sKH, ' với giá trị: ', phaiTT, ', Phương thức thanh toán: ', objHD.PhuongThucTT, ', Thời gian: ', ngaylapHD);
            }
            else {
                noiDung = txtFirst.concat(maHD, sKH, ' với giá trị: ', phaiTT, ', Phương thức thanh toán: ', objHD.PhuongThucTT, ', Thời gian: ', ngaylapHD);
            }

            noiDungChiTiet = txtFirst.concat(styleMaHD, styleHDGoc, styleKH,
                '<br/ > Giá trị: ', phaiTT, '<br/ > Phương thức thanh toán: ', objHD.PhuongThucTT, '<br/ > Thời gian: ', ngaylapHD)
        }


        // insert HT_NhatKySuDung
        var objNhatKy = {
            ID_NhanVien: _idNhanVien,
            ID_DonVi: _idDonVi,
            ChucNang: tenChucNang,
            LoaiNhatKy: loaiNhatKy,
            NoiDung: noiDung,
            NoiDungChiTiet: noiDungChiTiet,
        };

        if (chucNang === 1) {
            if (objHD.LoaiHoaDon !== 3) {
                objNhatKy.ID_HoaDon = objHD.ID;
                objNhatKy.LoaiHoaDon = objHD.LoaiHoaDon;

                var timeUpdate = GetNgayLapHD_whenSave(objHD.NgayLapHoaDon);
                objNhatKy.ThoiGianUpdateGV = timeUpdate;
                Post_NhatKySuDung_UpdateGiaVon(objNhatKy);
            }
            else {
                // DatHang: khong update GiaVon
                Insert_NhatKyThaoTac_1Param(objNhatKy);
            }
        }
        else {
            Insert_NhatKyThaoTac_1Param(objNhatKy);
        }
    }

    function Insert_ManyNhom(lstNhom) {

        // get unique in array
        lstNhom = $.unique(lstNhom);
        console.log('lstNhom_Insert ', lstNhom)
        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;

        $.ajax({
            data: myData,
            url: DMDoiTuongUri + "PostDM_DoiTuong_Nhom",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
            },
            statusCode: {

            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger('Thêm mới nhóm khách hàng thất bại');
            },
            complete: function () {
            }
        })
    }

    function Update_ManyNhom(lstNhom) {

        var myData = {};
        myData.lstDM_DoiTuong_Nhom = lstNhom;

        $.ajax({
            data: myData,
            url: DMDoiTuongUri + "PutDM_DoiTuong_Nhom",
            type: 'PUT',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
            },
            statusCode: {
            },
            error: function (jqXHR, textStatus, errorThrown) {
                ShowMessage_Danger('Cập nhật nhóm khách hàng thất bại');
            },
            complete: function () {
            }
        })
    }

    // Thanh Toan _ new inter face
    self.ThanhToanThe = function () {
        // check if dv theogio not stopping
        var ctNotStop = [];
        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            ctNotStop = $.grep(cthd, function (x) {
                return x.IDRandomHD === self.HoaDons().IDRandom() && x.DichVuTheoGio === 1 && x.Stop === false;
            });
        }
        if (ctNotStop.length > 0) {
            ShowMessage_Danger('Vui lòng dừng tính dịch vụ theo giờ trước khi nhập tiền khách đưa');
            return;
        }

        $('#paybill').modal('show');
        $('.tien-goi-y').css('display', 'none');
        // hide set discount, card
        $('#ulTabThanhToan li:eq(1)').css('display', 'none');

        // show/hide div TT Diem
        if (self.ThietLap().TinhNangTichDiem && self.HoaDons().LoaiHoaDon() === 1) {

            if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0 && self.ChiTietDoiTuong()[0].TongTichDiem > 0) {
                self.DienHienTai(self.ChiTietDoiTuong()[0].TongTichDiem);

                // quy doi diem --> Tien (bind at popThanhToanThe)
                var tienQuyDoi = Math.floor(self.DienHienTai() * self.ThietLap_TichDiem()[0].TienThanhToan / self.ThietLap_TichDiem()[0].DiemThanhToan);
                self.TienQuyDoi(tienQuyDoi);

                // neu cho phep TT = Diem && enough SoLanMuaHang
                if (self.ThietLap_TichDiem()[0].ThanhToanBangDiem && self.ChiTietDoiTuong()[0].SoLanMuaHang >= self.ThietLap_TichDiem()[0].SoLanMua) {
                    $('#divTTBangDiem').css('display', '');
                }
                else {
                    $('#divTTBangDiem').css('display', 'none');
                }
            }
            else {
                $('#divTTBangDiem').css('display', 'none');
            }
        }
        else {
            $('#divTTBangDiem').css('display', 'none');
        }

        // khong can check null ID_TaiKhoanPos, ID_TaiKhoanChuyenKhoan because checked at interface
        self.selectID_POS(self.HoaDons().ID_TaiKhoanPos());
        self.selectID_ChuyenKhoan(self.HoaDons().ID_TaiKhoanChuyenKhoan());

        // find Account in list AcccountPOS and set
        var accountPOS = $.grep(self.ListAccountPOS(), function (x) {
            return x.ID === self.selectID_POS();
        });

        if (accountPOS.length > 0) {
            self.ChoseAccountPOS(accountPOS[0]);
        }
        else {
            self.ChoseAccountPOS([]);
        }

        // find Account in list AcccountCK and set
        var accountCK = $.grep(self.ListAccountChuyenKhoan(), function (x) {
            return x.ID === self.selectID_ChuyenKhoan();
        });

        if (accountCK.length > 0) {
            self.ChoseAccountCK(accountCK[0]);
        }
        else {
            self.ChoseAccountCK([]);
        }

        if (_maHoaDon === '') {
            _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
        }
        var itemHD = GetHDOpening_byMaHoaDon_andViTri(_maHoaDon, _phongBanID);
        if (itemHD.length > 0) {
            AssignValue_ofHoaDon(itemHD[0].IDRandom);
        }
    }

    function SetLblTienThua(tienThua) {
        if (tienThua < 0) {
            $('#lblTienThua_HoaDon').text('Tiền khách thiếu');
        }
        else {
            $('#lblTienThua_HoaDon').text('Tiền thừa trả khách');
        }
        $('#lblpopTienThua').text(formatNumber(Math.abs(tienThua)));
    }

    self.editTienMat = function (d, e) {

        formatNumberObj($('#txtTienMat'));

        var tongPhaiTT = formatNumberToInt(self.HoaDons().PhaiThanhToan());
        var tienCK = 0;
        var tienATM = 0;
        var tienMat = formatNumberToInt($('#txtTienMat').val());
        var tienTheGiaTri = formatNumberToInt($('#txtTienTheGiaTri').val());
        var ttBangDiem = formatNumberToInt($('#lblTienQuyDoi').val());
        var tongTTBanDau = Math.round(tienMat + tienTheGiaTri + ttBangDiem);
        // neu chua chon tai khoan POS
        if (self.selectID_POS() === undefined || self.selectID_POS() === null) {
            tienATM = 0;
        }

        var idPOS = self.selectID_POS();
        if (idPOS === const_GuidEmpty || idPOS === null || idPOS === undefined) {
            idPOS = null;
        }
        var idTaiKhoanCK = self.selectID_ChuyenKhoan();
        if (idTaiKhoanCK === null || idTaiKhoanCK === const_GuidEmpty || idTaiKhoanCK === undefined) {
            idTaiKhoanCK = null;
            tienCK = 0;
        }

        if (idPOS === null) {
            tienATM = 0;
            if (idTaiKhoanCK !== null) {
                tienCK = Math.round(tongPhaiTT - tongTTBanDau);
            }
        }
        else {
            tienATM = Math.round(tongPhaiTT - tongTTBanDau);
            if (idTaiKhoanCK !== null) {
                tienCK = formatNumberToInt($('#txtTienGui').val());// giữ nguyên giá trị nhập trc đó
            }
        }

        var thanhToan = Math.round(tienATM + tienCK + tongTTBanDau);
        var tienThua = Math.round(thanhToan - tongPhaiTT);
        $('#txtTienATM').val(formatNumber(tienATM));
        $('#txtTienGui').val(formatNumber(tienCK));
        $('#lblpopThanhToan').text(formatNumber(thanhToan));
        SetLblTienThua(tienThua);

        var key = e.keyCode || e.which;
        // eneter, key tab not use
        if (key === 13) {
            if (idPOS === null) {
                $('#txtTienGui').select();
            }
            else {
                $('#txtTienATM').select();
            }
        }
    }

    self.editTienATM = function (d, e) {

        var tongPhaiTT = formatNumberToInt(self.HoaDons().PhaiThanhToan());

        formatNumberObj($('#txtTienATM'));
        var tienATM = formatNumberToInt($('#txtTienATM').val());
        var tienMat = formatNumberToInt($('#txtTienMat').val());

        var haveTichDiem = false;
        // check if tt = diem
        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0 && self.ChiTietDoiTuong()[0].TongTichDiem > 0) {
            haveTichDiem = true;
        }

        var thanhToan = Math.round(tienMat + tienATM);
        var tienGui = Math.round(tongPhaiTT - thanhToan);

        // if thanhToan > tongPhaiTT --> 
        if (tienGui < 0) {
            tienGui = 0;
        }

        // if not chose TK NganHang
        if (self.selectID_ChuyenKhoan() === undefined || self.selectID_ChuyenKhoan() === null) {
            tienGui = 0;
        }

        var tienThua = Math.round(thanhToan + tienGui - tongPhaiTT);
        $('#txtTienGui').val(formatNumber(tienGui));
        $('#lblpopThanhToan').text(formatNumber(thanhToan + tienGui));
        $('#txtTTBangDiem').val('');
        $('#lblTienQuyDoi').val('');
        SetLblTienThua(tienThua);

        var key = e.keyCode || e.which;
        if (key === 13) {
            $('#txtTienGui').select();
        }
    }

    self.editTienGui = function (d) {

        var tongPhaiTT = formatNumberToInt(self.HoaDons().PhaiThanhToan());

        formatNumberObj($('#txtTienGui'));
        var tienATM = formatNumberToInt($('#txtTienATM').val());
        var tienMat = formatNumberToInt($('#txtTienMat').val());
        var tienGui = formatNumberToInt($('#txtTienGui').val());

        var haveTichDiem = false;
        // check if tt = diem
        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0 && self.ChiTietDoiTuong()[0].TongTichDiem > 0) {
            haveTichDiem = true;
        }

        var thanhToan = Math.round(tienMat + tienATM + tienGui);
        var tienThua = Math.round(thanhToan - tongPhaiTT);

        $('#lblpopThanhToan').text(formatNumber(thanhToan));
        $('#txtTTBangDiem').val('');
        $('#lblTienQuyDoi').val('');
        SetLblTienThua(tienThua);

        var key = event.keyCode || event.which;
        if (key === 13 || key === 9) {
            if (key === 13) {
                if (haveTichDiem) {
                    $('#txtTTBangDiem').select();
                }
            }
            else {
                $('#txtTienGui').select();
            }
        }
    }

    self.editTTBangDiem = function () {
        var thisObj = $('#txtTTBangDiem');
        formatNumberObj(thisObj);

        var ttBangDiem = formatNumberToInt(thisObj.val()); // = diem quy doi

        // quy doi tien --> Diem
        var tienQuyDoi = 0;
        if (self.ThietLap_TichDiem() != null && self.ThietLap_TichDiem().length > 0) {
            tienQuyDoi = Math.floor(ttBangDiem * self.ThietLap_TichDiem()[0].TienThanhToan / self.ThietLap_TichDiem()[0].DiemThanhToan);
        }

        if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0 && self.ChiTietDoiTuong()[0].TongTichDiem < ttBangDiem) {
            ShowMessage_Danger('Vượt quá số điểm hiện tại');
        }

        var phaiTT = formatNumberToInt(self.HoaDons().PhaiThanhToan());
        var tienMat = formatNumberToInt($('#txtTienMat').val());
        var tienATM = formatNumberToInt($('#txtTienATM').val());
        var tienGui = formatNumberToInt($('#txtTienGui').val());

        var thanhToan = Math.round(tienQuyDoi + tienATM + tienMat + tienGui);
        var tienThua = Math.round(thanhToan - phaiTT);
        $('#lblpopThanhToan').text(formatNumber(thanhToan));
        $('#lblTienQuyDoi').val(formatNumber(tienQuyDoi));
        SetLblTienThua(tienThua);
    }

    self.agreePay = function () {
        $(".price-pay-end").focus();
        var tienMat = formatNumberToInt($('#txtTienMat').val());
        var tienATM = formatNumberToInt($('#txtTienATM').val());
        var tienGui = formatNumberToInt($('#txtTienGui').val());
        var diemQuyDoi = formatNumberToInt($('#txtTTBangDiem').val());
        var tienQuyDoi = formatNumberToInt($('#lblTienQuyDoi').val());

        var account_POS = self.selectID_POS();
        var account_ChuyenKhoan = self.selectID_ChuyenKhoan();

        if (account_POS !== undefined) {
            if (tienATM === 0) {
                ShowMessage_Danger('Vui lòng nhập số tiền quẹt thẻ');
                return;
            }
        }
        else {
            account_POS = null;
        }

        if (account_ChuyenKhoan !== undefined) {
            if (tienGui === 0) {
                ShowMessage_Danger('Vui lòng nhập số tiền chuyển khoản');
                return;
            }
        }
        else {
            account_ChuyenKhoan = null;
        }

        if (diemQuyDoi > self.DienHienTai()) {
            ShowMessage_Danger('Số tiền vượt quá giá trị thanh toán bằng điểm cho phép');
            return;
        }

        // load infor HoaDon
        var idVT = localStorage.getItem(lcPhongBanID);
        var itemHD = [];
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            // update TienMat, TienGui
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].ID_ViTri === idVT && lstHD[i].MaHoaDon === _maHoaDon
                    && lstHD[i].ID_DonVi === _idDonVi) {
                    let thanhToan = tienMat + tienGui + tienATM + tienQuyDoi;
                    lstHD[i].DaThanhToan = thanhToan;
                    lstHD[i].TienMat = tienMat;
                    lstHD[i].TienATM = tienATM;
                    lstHD[i].TienGui = tienGui;
                    lstHD[i].TienThua = Math.round(thanhToan - lstHD[i].PhaiThanhToan);
                    lstHD[i].TTBangDiem = tienQuyDoi;
                    lstHD[i].DiemQuyDoi = diemQuyDoi;
                    lstHD[i].ID_TaiKhoanPos = account_POS;
                    lstHD[i].ID_TaiKhoanChuyenKhoan = account_ChuyenKhoan;

                    itemHD = lstHD[i];
                    break;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));

            if (itemHD !== []) {
                UpdateDiemGiaoDich_forHoaDon(itemHD.IDRandom);
                SetText_lblTienMat(itemHD, 1);
                self.HoaDons().SetData(itemHD);
            }
        }

        $('#paybill').modal('hide');
    }

    function SetText_lblTienMat(itemHD, loaiThu) {

        var lblFind = $('#lblTienMat');
        if (loaiThu === 11) {
            lblFind = $('#lblTienMat_PhieuThu');
            tienMat = formatNumberToInt(self.TienMat_PhieuThu());
            tienPOS = formatNumberToInt(self.TienATM_PhieuThu());
            tienCK = formatNumberToInt(self.TienGui_PhieuThu());
            tienTheGTri = formatNumberToInt(self.TienTheGiaTri_PhieuThu());
        }
        else {
            tienMat = formatNumberToInt(itemHD.TienMat);
            tienPOS = formatNumberToInt(itemHD.TienATM);
            tienCK = formatNumberToInt(itemHD.TienGui);
            tienTheGTri = formatNumberToInt(itemHD.TienTheGiaTri);
            diemQuiDoi = formatNumberToInt(itemHD.DiemQuyDoi);
        }

        if (tienMat > 0) {
            if (tienPOS > 0) {
                if (tienCK > 0) {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, POS, CK, thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(Tiền mặt, POS, CK, điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, POS, CK, thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(Tiền mặt, POS, CK)');
                        }
                    }
                }
                else {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, POS, thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(Tiền mặt, POS, điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, POS, thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(Tiền mặt, POS)');
                        }
                    }
                }
            }
            else {
                if (tienCK > 0) {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, CK, thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(Tiền mặt, CK, điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, CK, thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(Tiền mặt, CK)');
                        }
                    }
                }
                else {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(Tiền mặt, điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Tiền mặt, thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(Tiền mặt)');
                        }
                    }
                }
            }
        }
        else {
            if (tienPOS > 0) {
                if (tienCK > 0) {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(POS, CK, thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(POS, CK, điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(POS, CK, thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(POS, CK)');
                        }
                    }
                }
                else {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(POS, thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(POS, điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(POS, thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(POS)');
                        }
                    }
                }
            }
            else {
                if (tienCK > 0) {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(CK, thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(CK, điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(CK, thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(CK)');
                        }
                    }
                }
                else {
                    if (diemQuiDoi > 0) {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Thẻ giá trị, điểm)');
                        }
                        else {
                            lblFind.text('(Điểm)');
                        }
                    }
                    else {
                        if (tienTheGTri !== undefined && tienTheGTri > 0) {
                            lblFind.text('(Thẻ giá trị)');
                        }
                        else {
                            lblFind.text('(Tiền mặt)');
                        }
                    }
                }
            }
        }
    }

    // khi tổng TT > phải TT --> trừ dần vào TienMat, TienPOS, ..
    function GetQuyHoaDonfromHD_andInsert(itemHD) {

        var loaiThuChi = 11;
        var tongThu = 0;
        var tienMat = formatNumberToFloat(itemHD.TienMat);
        var tienGui = formatNumberToFloat(itemHD.TienGui);
        var tienATM = formatNumberToFloat(itemHD.TienATM);
        var tienTheGiaTri = formatNumberToFloat(itemHD.TienTheGiaTri);
        var tienDiem = formatNumberToFloat(itemHD.TTBangDiem);
        var ghichu = itemHD.DienGiai;
        var idDoiTuong = itemHD.ID_DoiTuong;
        var idHoaDon = itemHD.ID;
        var idKhoanThuChi = null;

        var lstQuyChiTiet = [];
        var phaiTT = 0;
        if (self.HoaDons().HoanTraTamUng()) {
            loaiThuChi = 12;
            phaiTT = self.HoaDons().HoanTraTamUng();

            let dataReturn = shareMoney_QuyHD(phaiTT, 0, tienMat, 0, tienGui, tienTheGiaTri, itemHD);
            tienDiem = 0;
            tienMat = dataReturn.TienMat;
            tienATM = 0;
            tienGui = dataReturn.TienChuyenKhoan;
            tienTheGiaTri = dataReturn.TienTheGiaTri;
        }
        else {
            phaiTT = formatNumberToInt(itemHD.PhaiThanhToan);
            // phân bổ và trừ dần TienThua vào TienMat, POS CK ,...
            let dataReturn = shareMoney_QuyHD(phaiTT, tienDiem, tienMat, tienATM, tienGui, tienTheGiaTri, itemHD);
            tienDiem = dataReturn.TTBangDiem;
            tienMat = dataReturn.TienMat;
            tienATM = dataReturn.TienPOS;
            tienGui = dataReturn.TienChuyenKhoan;
            tienTheGiaTri = dataReturn.TienTheGiaTri;
        }
        tongThu = tienDiem + tienMat + tienATM + tienGui + tienTheGiaTri;

        var tenDoiTuong = 'Khách lẻ';
        var arrDoiTuong0 = $.grep(self.DoiTuongs(), function (item) {
            return item.ID === idDoiTuong;
        });
        if (arrDoiTuong0.length > 0) {
            tenDoiTuong = arrDoiTuong0[0].TenDoiTuong;
        }
        // is setup MaChungTu by temp --> assign MaPhieuThu = null
        var phuongthucTT = '';
        var ngaylapHD = GetNgayLapHD_whenSave(itemHD.NgayLapHoaDon);
        if (tongThu > 0) {
            var Quy_HoaDon = {
                LoaiHoaDon: loaiThuChi,
                TongTienThu: tongThu,
                MaHoaDon: 'TT' + itemHD.MaHoaDon,
                NgayLapHoaDon: ngaylapHD,
                NguoiNopTien: tenDoiTuong,
                NguoiTao: _userLogin,
                NoiDungThu: ghichu,
                ID_NhanVien: itemHD.ID_NhanVien,
                ID_DonVi: itemHD.ID_DonVi,
                ID_KhoanThuChi: null,

                // used to get save diary 
                MaHoaDonTraHang: itemHD.MaHoaDon,
                ID_DoiTuong: idDoiTuong,
            }

            // phai tach rieng Quy_HoaDon_ChiTiet thi moi lay dung gia tri cua no
            if (tienMat > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienMat,
                    TienMat: tienMat,
                    HinhThucThanhToan: 1,
                });
                lstQuyChiTiet.push(qct);
                phuongthucTT = 'Tiền mặt, ';
            }
            if (tienATM > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienATM,
                    TienPOS: tienATM,
                    HinhThucThanhToan: 2,
                    ID_TaiKhoanNganHang: itemHD.ID_TaiKhoanPos,
                });
                lstQuyChiTiet.push(qct);
                phuongthucTT += 'POS, ';
            }
            if (tienGui > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienGui,
                    TienChuyenKhoan: tienGui,
                    HinhThucThanhToan: 3,
                    ID_TaiKhoanNganHang: itemHD.ID_TaiKhoanChuyenKhoan,
                });
                lstQuyChiTiet.push(qct);
                phuongthucTT += 'Chuyển khoản,  ';
            }
            if (tienTheGiaTri > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienTheGiaTri,
                    TienTheGiaTri: tienTheGiaTri,
                    HinhThucThanhToan: 4,
                });
                lstQuyChiTiet.push(qct);
                phuongthucTT += 'Thẻ giá trị, ';
            }
            if (tienDiem > 0) {
                let qct = newQuyChiTiet({
                    ID_HoaDonLienQuan: idHoaDon,
                    ID_KhoanThuChi: idKhoanThuChi,
                    ID_DoiTuong: idDoiTuong,
                    GhiChu: ghichu,
                    TienThu: tienDiem,
                    DiemThanhToan: itemHD.DiemQuyDoi,
                    HinhThucThanhToan: 5,
                });
                lstQuyChiTiet.push(qct);
                phuongthucTT += 'Điểm, ';
            }


            Quy_HoaDon.PhuongThucTT = Remove_LastComma(phuongthucTT);
            var myData = {};
            myData.objQuyHoaDon = Quy_HoaDon;
            myData.lstCTQuyHoaDon = lstQuyChiTiet;
            console.log(1, myData)

            ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/PostQuy_HoaDon_DefaultIDDoiTuong', 'POST', myData).done(function (data) {

            });

            Insert_NhatKyThaoTac(Quy_HoaDon, [], 3, 1);
        }
    }

    function UpdateDiemKH_toDB(idDoiTuong) {

        var itemDT = $.grep(self.DoiTuongs(), function (x) {
            return x.ID === idDoiTuong;
        });

        if (itemDT.length > 0) {

            var DM_DoiTuong = {
                ID: itemDT[0].ID,
                TongTichDiem: itemDT[0].TongTichDiem
            }

            var myData = {};
            myData.objDoiTuong = DM_DoiTuong;

            ajaxHelper(DMDoiTuongUri + 'UpdateDiem_DMDoiTuong', 'POST', myData).done(function (err) {
                if (err !== '') {
                    console.log(err);
                }
            });
        }
    }

    self.ChoseAccountPOS = function (item) {

        $('#lstAccountPOS li').each(function () {
            $(this).find('i').remove();
        });

        if (item.ID === undefined) {
            self.selectID_POS(undefined);
            $('#txtTienATM').val(0);
            $('#divAccountPOS').text('---Chọn tài khoản---');
            if (self.selectID_ChuyenKhoan() === null || self.selectID_ChuyenKhoan() === undefined) {
                if (formatNumberToFloat($('#txtTienMat').val()) === 0) {
                    $('#txtTienMat').val(formatNumber3Digit(self.HoaDons().PhaiThanhToan()));
                }
            }
        }
        else {
            self.selectID_POS(item.ID);
            $('#divAccountPOS').text(item.TenChuThe);
            $('span[id=checkAccountPOS_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
        }

        var tienmat = formatNumberToFloat($('#txtTienMat').val());
        var tienck = formatNumberToFloat($('#txtTienGui').val());
        var thanhtoan = tienmat + tienck;
        var tienThua = Math.round(thanhtoan - self.HoaDons().PhaiThanhToan());
        $('#lblpopThanhToan').text(formatNumber(thanhtoan));
        SetLblTienThua(tienThua);
    }

    self.ChoseAccountCK = function (item) {

        $('#lstAccountCK li').each(function () {
            $(this).find('i').remove();
        });

        if (item.ID === undefined) {
            self.selectID_ChuyenKhoan(undefined);
            $('#divAccountCK').text('---Chọn tài khoản---');
            $('#txtTienGui').val(0);
            if (self.selectID_POS() === null || self.selectID_POS() === undefined) {
                if (formatNumberToFloat($('#txtTienMat').val()) === 0) {
                    $('#txtTienMat').val(formatNumber3Digit(self.HoaDons().PhaiThanhToan()));
                }
            }
        }
        else {
            self.selectID_ChuyenKhoan(item.ID);
            $('#divAccountCK').text(item.TenChuThe);
            $('span[id=checkAccountCK_' + item.ID + ']').append('<i class="fa fa-check pull-right my-fa-check" aria-hidden="true" style="display:block"></i>')
        }

        var tienmat = formatNumberToFloat($('#txtTienMat').val());
        var tienpos = formatNumberToFloat($('#txtTienATM').val());
        var thanhtoan = tienmat + tienpos;
        var tienThua = Math.round(thanhtoan - self.HoaDons().PhaiThanhToan());
        $('#lblpopThanhToan').text(formatNumber(thanhtoan));
        SetLblTienThua(tienThua);
    }

    function GetDM_TaiKhoanNganHang() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/Quy_HoaDonAPI/' + 'GetAllTaiKhoanNganHang_ByDonVi?idDonVi=' + _idDonVi, 'GET').done(function (x) {
                if (x.res === true) {
                    var data = x.data;

                    var lstTKPos = $.grep(data, function (x) {
                        return x.TaiKhoanPOS === true;
                    });
                    self.ListAccountPOS(lstTKPos);

                    var lstTKChuyenKhoan = $.grep(data, function (x) {
                        return x.TaiKhoanPOS === false;
                    });
                    self.ListAccountChuyenKhoan(lstTKChuyenKhoan);

                    WriteData_Dexie(db.DM_TaiKhoanNganHang, data);
                }
            })
        }
        else {
            db.DM_TaiKhoanNganHang.toArray(function (dt) {
                var lstTKPos = $.grep(dt, function (x) {
                    return x.TaiKhoanPOS === true;
                });
                self.ListAccountPOS(lstTKPos);
                var lstTKChuyenKhoan = $.grep(dt, function (x) {
                    return x.TaiKhoanPOS === false;
                });
                self.ListAccountChuyenKhoan(lstTKChuyenKhoan);
            });
        }
    }

    self.arrFilterAccountPOS = ko.computed(function () {
        var filter = self.filterAcPOS();

        return arrFilter = ko.utils.arrayFilter(self.ListAccountPOS(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TextSearchAuto).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && filter) {
                chon = locdau(prod.TextSearchAuto).toLowerCase().indexOf(locdau(filter).toLowerCase()) > -1 ||
                    sSearch.indexOf(locdau(filter).toLowerCase()) > -1;
            }
            return chon;
        });
    });

    self.arrFilterAccountChuyenKhoan = ko.computed(function () {
        var filter = self.filterAcCK();

        return arrFilter = ko.utils.arrayFilter(self.ListAccountChuyenKhoan(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TextSearchAuto).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (let i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && filter) {
                chon = locdau(prod.TextSearchAuto).toLowerCase().indexOf(locdau(filter).toLowerCase()) > -1 ||
                    sSearch.indexOf(locdau(filter).toLowerCase()) > -1;
            }
            return chon;
        });
    });

    self.ShowInforAccount_POS = function (isPos, isPhieuThu) {

        var idFind = null;
        var itemAccPOS = [];

        if (isPos) {
            if (isPhieuThu) {
                idFind = self.selectID_POSPT();
            }
            else {
                idFind = self.selectID_POS();
            }

            itemAccPOS = $.grep(self.ListAccountPOS(), function (x) {
                return x.ID === idFind;
            });
        }
        else {
            if (isPhieuThu) {
                idFind = self.selectID_ChuyenKhoanPT();
            }
            else {
                idFind = self.selectID_ChuyenKhoan();
            }

            itemAccPOS = $.grep(self.ListAccountChuyenKhoan(), function (x) {
                return x.ID === idFind;
            });
        }

        if (itemAccPOS.length > 0) {
            self.newAccountBank().Setdata(itemAccPOS[0]);
            $('#modalInforAccount').modal('show');
        }
    }

    function GetHT_Quyen_ByNguoiDung() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetListQuyen_OfNguoiDung", 'GET').done(function (data) {
                if (data != null) {
                    self.Quyen_NguoiDung(data);
                    WriteData_Dexie(db.Quyen_NguoiDung, data);
                }
                else {
                    ShowMessage_Danger('Không có quyền');
                }
            });
        }
        else {
            db.Quyen_NguoiDung.toArray(function (dt) {
                self.Quyen_NguoiDung(dt);
                CheckPermission_andShowTagA();
                if (IsShop_SpaSalon()) {
                    self.ClickTab_PhongBan(false, true);
                }
            })
        }
    }

    function AddKhachHang_HoaDon(DM_DoiTuong, myDataHD, isDongBo) {

        var idNhomDTs = DM_DoiTuong.ID_NhomDoiTuong;
        delete DM_DoiTuong["ID"]; // delete ID because when Offline: save ID = MaDoiTuong
        delete DM_DoiTuong["ID_NhomDoiTuong"]; // delete ID because when Offline: save ID = MaDoiTuong

        console.log('myDataHD ', myDataHD)

        jQuery.ajax({
            data: DM_DoiTuong,
            url: DMDoiTuongUri + "PostDM_DoiTuong",
            type: 'POST',
            async: false, // not save at the same time
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (obj) {
                if (obj.res === true) {
                    var itemDT = obj.data;

                    DM_DoiTuong.ID = itemDT.ID;
                    DM_DoiTuong.Name_Phone = DM_DoiTuong.TenDoiTuong_ChuCaiDau + " " + DM_DoiTuong.TenDoiTuong_KhongDau + " " +
                        DM_DoiTuong.TenDoiTuong + " " + itemDT.MaDoiTuong + " " + DM_DoiTuong.DienThoai;

                    // insert ManyNhom offline
                    if (idNhomDTs !== null) {
                        var lstNhom = [];
                        var arrID_Nhom = idNhomDTs.split(',');
                        for (let i = 0; i < arrID_Nhom.length; i++) {
                            if (arrID_Nhom[i].length >= 36) {
                                var objDoiTuongNhom = {
                                    ID_DoiTuong: itemDT.ID,
                                    ID_nhomDoiTuong: arrID_Nhom[i],
                                };
                                lstNhom.push(objDoiTuongNhom);
                            }
                        }

                        if (lstNhom.length > 0) {
                            Insert_ManyNhom(lstNhom); //(TODO duplicate Nhom)
                        }
                    }

                    // add new KH indexedDB
                    AddNewDoiTuong_andRemoveDoiTuongOfflineOld_IndexDB(DM_DoiTuong);

                    // save HoaDon
                    myDataHD.objHoaDon.ID_DoiTuong = itemDT.ID;
                    if (isDongBo) {
                        PostHD_SoQuy_DongBo(myDataHD);
                    }
                    else {
                        PostHD_SoQuy_SaveHD(myDataHD);
                    }
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            },
            complete: function () {
            }
        });
    }

    function AddNewDoiTuong_andRemoveDoiTuongOfflineOld_IndexDB(DM_DoiTuong) {

        var maKHoffline = DM_DoiTuong.MaDoiTuong;
        // update again ID for DoiTuong (vi da nang nhom roi)
        var khUpdate = [];
        for (let i = 0; i < self.DoiTuongs().length; i++) {
            if (self.DoiTuongs()[i].ID === maKHoffline) {
                self.DoiTuongs()[i].ID = DM_DoiTuong.ID;
                khUpdate = self.DoiTuongs()[i];
                db.DM_DoiTuong.delete(maKHoffline);
                db.DM_DoiTuong.put(khUpdate);
                break;
            }
        }

        // update ID_DoiTuong new for all HD with ID_DoiTuong = MaDoiTuong offline
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].ID_DoiTuong === maKHoffline) {
                    lstHD[i].ID_DoiTuong = DM_DoiTuong.ID;
                }
            }

            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
        }

        // Update new ID for DoiTuong offline
        var doiTuongOffline = localStorage.getItem("DoiTuongHD_Offline");
        if (doiTuongOffline !== null) {
            doiTuongOffline = JSON.parse(doiTuongOffline);

            for (let i = 0; i < doiTuongOffline.length; i++) {
                if (doiTuongOffline[i].ID === maKHoffline) {
                    doiTuongOffline[i].ID = DM_DoiTuong.ID;
                }
            }
            localStorage.setItem("DoiTuongHD_Offline", JSON.stringify(doiTuongOffline));
        }
    }

    function UpdateTonKho_forListHangHoa_andCTHD(lstCTHD) {
        var maHDOld = lstCTHD[0].MaHoaDon;

        // update again TonKho for list HangHoaAll
        for (i = 0; i < lstCTHD.length; i++) {
            var j = 0;
            var lengHH = self.HangHoaAll().length;
            for (j; j < lengHH; j++) {
                if (self.HangHoaAll()[j].ID_DonViQuiDoi === lstCTHD[i].ID_DonViQuiDoi) {
                    // update self.HangHoaAll() --> knockout auto update self.HangHoas
                    self.HangHoaAll()[j].TonKho = self.HangHoaAll()[j].TonKho - lstCTHD[i].SoLuong;
                }
            }
        }

        // update TonKho for lstCTHD con lai
        var lstCTHD = localStorage.getItem(lcListCTHD);
        if (lstCTHD !== null) {
            lstCTHD = JSON.parse(lstCTHD);

            for (i = 0; i < lstCTHD.length; i++) {
                // chi update nhung hang hoa con lai chua luu HoaDon
                if (lstCTHD[i].MaHoaDon !== maHDOld) {
                    var j = 0;
                    var lengHH = self.HangHoaAll().length;
                    for (j; j < lengHH; j++) {
                        if (self.HangHoaAll()[j].ID_DonViQuiDoi === lstCTHD[i].ID_DonViQuiDoi) {
                            lstCTHD[i].TonKho = self.HangHoaAll()[j].TonKho;
                        }
                    }
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));
        }

        // update again TonKho for indexedDB local(TODO)
    }

    // neu KH oline, but update offline --> when save HD, update both infor KH
    function UpdateInforKH_toDB(idDoiTuong) {
        if (idDoiTuong !== null) {
            // check exist in cache lst ID_DoiTuong
            var lstID_DoiTuong = localStorage.getItem('lcID_DoiTuongOnlineNH');
            if (lstID_DoiTuong !== null) {
                lstID_DoiTuong = JSON.parse(lstID_DoiTuong);

                if ($.inArray(idDoiTuong, lstID_DoiTuong) > -1) {
                    var itemDT = $.grep(self.DoiTuongs(), function (x) {
                        return x.ID == idDoiTuong;
                    });

                    if (itemDT.length > 0) {
                        var DM_DoiTuong = {
                            ID: idDoiTuong,
                            MaDoiTuong: itemDT[0].MaDoiTuong,
                            TenDoiTuong: itemDT[0].TenDoiTuong,
                            DienThoai: itemDT[0].DienThoai,
                            Email: itemDT[0].Email,
                            DiaChi: itemDT[0].DiaChi,
                            GioiTinhNam: itemDT[0].GioiTinhNam,
                            NgaySinh_NgayTLap: itemDT[0].NgaySinh_NgayTLap,
                            MaSoThue: itemDT[0].MaSoThue,
                            LoaiDoiTuong: 1,
                            GhiChu: itemDT[0].GhiChu,
                            ID_NguonKhach: itemDT[0].ID_NguonKhach,
                            ID_NguoiGioiThieu: itemDT[0].ID_NguoiGioiThieu,
                            ID_NhanVienPhuTrach: itemDT[0].ID_NhanVienPhuTrach,
                            LaCaNhan: itemDT[0].LaCaNhan,
                            ID_TinhThanh: itemDT[0].ID_TinhThanh,
                            ID_QuanHuyen: itemDT[0].ID_QuanHuyen,
                            ID_DonVi: _idDonVi,
                            NguoiTao: _userLogin, // user dang nhap
                            TenDoiTuong_KhongDau: locdau(itemDT[0].TenDoiTuong),
                            TenDoiTuong_ChuCaiDau: GetChartStart(itemDT[0].TenDoiTuong),
                            DinhDang_NgaySinh: itemDT[0].DinhDang_NgaySinh,
                        };

                        var myData = {};
                        myData.id = idDoiTuong;
                        myData.objDoiTuong = DM_DoiTuong;

                        console.log('DM_DoiTuong_updateOffline', myData)

                        $.ajax({
                            url: DMDoiTuongUri + "PutDM_DoiTuong",
                            type: 'PUT',
                            dataType: 'json',
                            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                            data: myData,
                            success: function (item) {
                                // delete ID in cache
                                lstID_DoiTuong = $.grep(lstID_DoiTuong, function (x) {
                                    return x !== idDoiTuong;
                                });
                                localStorage.setItem('lcID_DoiTuongOnlineNH', JSON.stringify(lstID_DoiTuong));
                            },
                            complete: function () {
                            }
                        })
                    }
                }
            }
        }
    }

    function GetNgayLapHD_whenSave(ngaylapHD) {
        if (ngaylapHD === null || ngaylapHD === 'Invalid date' || ngaylapHD == undefined) {
            ngaylapHD = moment(new Date()).format('YYYY-MM-DD HH:mm:ss');
        }
        else {
            var ddMMyyyy = ngaylapHD.split('/');
            if (ddMMyyyy.length > 1) {
                ngaylapHD = moment(ngaylapHD, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm:ss');
            }
            // else keep value YYYY-MM-DD
        }
        return ngaylapHD;
    }

    function PostHD_SoQuy_SaveHD(myData) {

        myData.IsSetGiaVonTrungBinh = self.ThietLap().GiaVonTrungBinh;

        var objHDAdd = myData.objHoaDon;
        var objCTAdd = myData.objCTHoaDon;
        var idViTri = objHDAdd.ID_ViTri;
        var idRandomHD = objHDAdd.IDRandom;

        console.log('myData ', myData)

        $.ajax({
            data: myData,
            url: BH_HoaDonUri + "PostBH_HoaDon_NhaHang",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (objReturn) {
                if (objReturn.res === true) {
                    var itemDB = objReturn.data;

                    if (objHDAdd.StatusOffline === false) {
                        // update TonKho if HD mua and not offline
                        UpdateTonKho_forListHangHoa_andCTHD(objCTAdd);
                    }

                    // if ThongBaoBep, after saveHoaDon: itemHD.ID_NhanVien = undefined ???
                    if (objHDAdd.ID_NhanVien === undefined) {
                        objHDAdd.ID_NhanVien = itemDB.ID_NhanVien;
                    }

                    // InsertQuy_HoaDon
                    objHDAdd.ID = itemDB.ID;
                    objHDAdd.MaHoaDon = itemDB.MaHoaDon;
                    GetQuyHoaDonfromHD_andInsert(objHDAdd);

                    // Update Diem,Nhom to DB
                    UpdateDiemKH_toDB(objHDAdd.ID_DoiTuong);
                    UpdateNhomKH_DB(objHDAdd.ID_DoiTuong);

                    Insert_NhatKyThaoTac(objHDAdd, objCTAdd, 1, 1);

                    // start : get infor HoaDon before delete --> print
                    var objHDPrint = GetInforHDPrint(objHDAdd);
                    self.InforHDprintf(objHDPrint);

                    var cthdPrint = GetCTHDPrint_Format(objCTAdd);
                    self.CTHoaDonPrint(cthdPrint);
                    self.InHoaDon('HDBL', false);
                    // end Print

                    // remove HoaDon
                    var lstHD = JSON.parse(localStorage.getItem(lcListHD));
                    lstHD = $.grep(lstHD, function (x) {
                        return x.IDRandom !== idRandomHD;
                    })
                    localStorage.setItem(lcListHD, JSON.stringify(lstHD));

                    // check if dang Yeu Cau Bep
                    var blExistBep = false;
                    var arrBep = [];
                    var cthd = localStorage.getItem(lcListCTHD);
                    if (cthd !== null) {
                        // get lstCTHD after update client
                        cthd = JSON.parse(cthd);
                        arrBep = jQuery.grep(cthd, function (itemEx) {
                            return itemEx.IDRandomHD === idRandomHD
                                && (itemEx.Bep_SoLuongYeuCau > 0 || itemEx.Bep_SoLuongChoCungUng > 0);
                        });

                        // remove CTHD
                        cthd = $.grep(cthd, function (x) {
                            return x.IDRandomHD !== idRandomHD;
                        })
                        localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

                        if (arrBep.length > 0 && connected) {
                            ThongBaoBep_SendData(cthd, 5);
                        };

                        var objSend = CreateObjHub(JSON.stringify(lstHD), JSON.stringify(cthd), idRandomHD, idViTri, 2, null);
                        SendData_atThisPage(objSend);


                        RemoveKHoffline_afterSave();
                        BindHD_CTHD();
                        Bind_andCountHDO();
                        ShowMessage_Success('Thêm mới hóa đơn thành công');
                        UpdateTime_lcPhongBan();
                        CountTableUsed(false);
                    }
                }
                else {
                    console.log(objReturn.mes)
                    ShowMessage_Danger('Thêm mới hóa đơn thất bại');
                }
            },
            complete: function () {
                $('#wait').remove();
                _maHoaDonOffline = '';
                SaveHD_RemoveDisable();
                $('.inner-setup, .parent-price-1, .price-pay-end').removeClass('btn-disable');
            }
        });

        UpdateInforKH_toDB(objHDAdd.ID_DoiTuong);
    }

    function PostHD_SoQuy_DongBo(myData) {

        console.log('myData ', myData);

        myData.IsSetGiaVonTrungBinh = self.ThietLap().GiaVonTrungBinh;

        var objHD = myData.objHoaDon;
        $.ajax({
            data: myData,
            url: BH_HoaDonUri + "PostBH_HoaDon_NhaHang",
            type: 'POST',
            async: false,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (objReturn) {
                if (objReturn.res == true) {
                    itemDB = objReturn.data;
                    objHD.MaHoaDon = itemDB.MaHoaDon;
                    objHD.ID = itemDB.ID;

                    // if not get ID_NhanVien --> assign = null --> avoid err when save Quy_HoaDon
                    if (objHD.ID_NhanVien === undefined) {
                        objHD.ID_NhanVien = itemDB.ID_NhanVien;
                    }

                    GetQuyHoaDonfromHD_andInsert(objHD);

                    UpdateDiemKH_toDB(objHD.ID_DoiTuong);
                    UpdateNhomKH_DB(objHD.ID_DoiTuong);

                    // insert HT_NhatKySuDung
                    Insert_NhatKyThaoTac(objHD, myData.objCTHoaDon, 1, 1);
                    ShowMessage_Success('Thêm mới hóa đơn thành công');
                }
                else {
                    ShowMessage_Success('Thêm mới hóa đơn thất bại');
                }
            },
            complete: function () {
                $('#btnDongBoHoa,#btnSave').removeAttr('disabled');
                // if opening HDOffline: after save remove disable
                $('input,select').removeAttr('disabled');
                $('.inner-setup, .parent-price-1, .price-pay-end').removeClass('btn-disable');
                Enable_DisableNgayLapHD();
                _maHoaDonOffline = '';
            }
        });

        UpdateInforKH_toDB(objHD.ID_DoiTuong);
    }

    function UpdateGiamGiaHD_ByNhomKH() {
        if (initLoad) {
            var phongbanID = _phongBanID;
            if (_maHoaDon === '') {
                _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
            }

            var lstHD = localStorage.getItem(lcListHD);
            if (lstHD !== null) {
                lstHD = JSON.parse(lstHD);

                var idNhomDTs = '';
                if (self.ChiTietDoiTuong() !== null && self.ChiTietDoiTuong().length > 0) {
                    if (self.ChiTietDoiTuong()[0].ID_NhomDoiTuong === null || self.ChiTietDoiTuong()[0].ID_NhomDoiTuong === undefined) {
                        idNhomDTs = '';
                    }
                    else {
                        idNhomDTs = self.ChiTietDoiTuong()[0].ID_NhomDoiTuong.toLowerCase();
                    }

                    // order NgayTaoNhom ASC
                    var arrGroupOrder = self.NhomDoiTuongDB();
                    arrGroupOrder = arrGroupOrder.sort(function (a, b) {
                        var x = a.NgayTao,
                            y = b.NgayTao;
                        return x < y ? -1 : x > y ? 1 : 0;
                    });

                    // get all Nhom of DoiTuong have GiamGia
                    var allNhomGG = [];
                    for (let i = 0; i < arrGroupOrder.length; i++) {
                        if (idNhomDTs.indexOf(arrGroupOrder[i].ID) > -1 && arrGroupOrder[i].GiamGia > 0) {
                            allNhomGG.push(arrGroupOrder[i]);
                        }
                    }

                    self.ListGroupSale(allNhomGG);

                    var groupApply = [];
                    // get Nhom first (min NgayTao)
                    if (allNhomGG.length > 0) {
                        groupApply.push(allNhomGG[0]);
                    }

                    if (groupApply.length > 0) {

                        var dvtGiam = 'VND';
                        var ptGiam = groupApply[0].GiamGia;

                        for (let i = 0; i < lstHD.length; i++) {
                            if (lstHD[i].ID_ViTri === phongbanID && lstHD[i].MaHoaDon === _maHoaDon
                                && lstHD[i].ID_DonVi === _idDonVi) {
                                if (groupApply[0].GiamGiaTheoPhanTram) {
                                    dvtGiam = '%';
                                    $('#dvtGiamParent').text('%');
                                    $('.vnd1').removeClass('gb');
                                    $('.senter1').addClass('gb');
                                    lstHD[i].TongChietKhau = ptGiam;
                                    lstHD[i].TongGiamGia = Math.round(lstHD[i].TongTienHang * ptGiam / 100);
                                }
                                else {
                                    $('#dvtGiamParent').text('VND');
                                    $('.senter1').removeClass('gb');
                                    $('.vnd1').addClass('gb');
                                    lstHD[i].TongChietKhau = 0;
                                    lstHD[i].TongGiamGia = ptGiam;
                                }
                                lstHD[i].TongGiamGiaKM_HD = lstHD[i].KhuyeMai_GiamGia + lstHD[i].TongGiamGia;

                                var phaiTT = lstHD[i].TongTienHang - lstHD[i].TongGiamGiaKM_HD;
                                lstHD[i].PhaiThanhToan = phaiTT < 0 ? 0 : phaiTT;

                                lstHD[i].DaThanhToan = lstHD[i].PhaiThanhToan;
                                lstHD[i].TienMat = lstHD[i].PhaiThanhToan;
                                lstHD[i].ID_NhomDTApplySale = allNhomGG[0].ID;
                                $('.price-sa').val(formatNumber(ptGiam));
                                break;
                            }
                        }
                        localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                    }
                    else {
                        self.ListGroupSale([]);

                        for (let i = 0; i < lstHD.length; i++) {
                            // update again PTGiam if trước đó lstHD[i].ID_NhomDTApplySale !== null (OK)
                            if (lstHD[i].ID_ViTri === phongbanID && lstHD[i].MaHoaDon === _maHoaDon && lstHD[i].ID_NhomDTApplySale !== null
                                && lstHD[i].ID_DonVi === _idDonVi) {
                                lstHD[i].TongChietKhau = 0;
                                lstHD[i].TongGiamGia = 0;
                                lstHD[i].TongGiamGiaKM_HD = lstHD[i].KhuyeMai_GiamGia + lstHD[i].TongGiamGia;

                                var phaiTT = lstHD[i].TongTienHang - lstHD[i].TongGiamGiaKM_HD;
                                lstHD[i].PhaiThanhToan = phaiTT < 0 ? 0 : phaiTT;

                                lstHD[i].DaThanhToan = lstHD[i].PhaiThanhToan;
                                lstHD[i].TienMat = lstHD[i].PhaiThanhToan;
                                lstHD[i].ID_NhomDTApplySale = null;
                                break;
                            }
                        }
                        localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                    }
                }
                else {
                    self.ListGroupSale([]);

                    for (let i = 0; i < lstHD.length; i++) {
                        if (lstHD[i].ID_ViTri === phongbanID && lstHD[i].MaHoaDon === _maHoaDon && lstHD[i].ID_NhomDTApplySale !== null
                            && lstHD[i].ID_DonVi === _idDonVi) {
                            lstHD[i].TongChietKhau = 0;
                            lstHD[i].TongGiamGia = 0;
                            lstHD[i].TongGiamGiaKM_HD = lstHD[i].KhuyeMai_GiamGia + lstHD[i].TongGiamGia;

                            var phaiTT = lstHD[i].TongTienHang - lstHD[i].TongGiamGiaKM_HD;
                            lstHD[i].PhaiThanhToan = phaiTT < 0 ? 0 : phaiTT;

                            lstHD[i].DaThanhToan = lstHD[i].PhaiThanhToan;
                            lstHD[i].TienMat = lstHD[i].PhaiThanhToan;
                            lstHD[i].ID_NhomDTApplySale = null;
                            break;
                        }
                    }
                    localStorage.setItem(lcListHD, JSON.stringify(lstHD));
                }
            }
        }
    }

    self.GetlistGroupSale = function () {
        UpdateGiamGiaHD_ByNhomKH();
    }

    self.ChangeID_NhomSale = function (item) {
        var phongbanID = localStorage.getItem(lcPhongBanID);
        if (_maHoaDon === '') {
            _maHoaDon = $('.divLstHoaDon span.active').find('span.munber-goods').text();
        }
        var idRandom = '';

        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);

            var itemNhomChose = $.grep(self.NhomDoiTuongDB(), function (x) {
                return x.ID === item.ID;
            });

            if (itemNhomChose.length > 0) {
                var dvtGiam = 'VND';
                var ptGiam = itemNhomChose[0].GiamGia;

                for (let i = 0; i < lstHD.length; i++) {
                    if (lstHD[i].ID_ViTri === phongbanID && lstHD[i].MaHoaDon === _maHoaDon
                        && lstHD[i].ID_DonVi === _idDonVi) {
                        if (itemNhomChose[0].GiamGiaTheoPhanTram) {
                            dvtGiam = '%';
                            $('#dvtGiamParent').text('%');
                            $('.vnd1').removeClass('gb');
                            $('.senter1').addClass('gb');
                            lstHD[i].TongChietKhau = ptGiam;
                            lstHD[i].TongGiamGia = Math.round(lstHD[i].TongTienHang * ptGiam / 100);
                        }
                        else {
                            $('#dvtGiamParent').text('VND');
                            $('.senter1').removeClass('gb');
                            $('.vnd1').addClass('gb');
                            lstHD[i].TongChietKhau = 0;
                            lstHD[i].TongGiamGia = ptGiam;
                        }
                        lstHD[i].TongGiamGiaKM_HD = lstHD[i].KhuyeMai_GiamGia + lstHD[i].TongGiamGia;
                        lstHD[i].PhaiThanhToan = lstHD[i].TongTienHang - lstHD[i].TongGiamGiaKM_HD;
                        lstHD[i].DaThanhToan = lstHD[i].PhaiThanhToan;
                        lstHD[i].TienMat = lstHD[i].PhaiThanhToan;
                        lstHD[i].ID_NhomDTApplySale = item.ID;
                        $('.price-sa').val(formatNumber(ptGiam));

                        idRandom = lstHD[i].IDRandom;
                        break;
                    }
                }
                localStorage.setItem(lcListHD, JSON.stringify(lstHD));
            }
        }

        UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandom);
        AssignValue_ofHoaDon(idRandom);
    }

    function GetDM_NhomDoiTuong_ChiTiets() {
        if (navigator.onLine) {
            ajaxHelper(DMNhomDoiTuongUri + 'GetDM_NhomDoiTuong_ChiTiets?idDonVi=' + _idDonVi, 'GET').done(function (x) {
                let data = x.data;
                if (data.length > 0) {

                    self.DM_NhomDoiTuong_ChiTiets(data);
                    localStorage.setItem('lcNhomDT_ChiTiet', JSON.stringify(data));

                    var arrIDNhomAdd = [];
                    var objNhom = [];
                    for (let i = 0; i < data.length; i++) {
                        var itemData = data[i];
                        if ($.inArray(itemData.ID_NhomDoiTuong, arrIDNhomAdd) === -1) {

                            arrIDNhomAdd.push(itemData.ID_NhomDoiTuong);

                            var objNew = {
                                ID_NhomDoiTuong: itemData.ID_NhomDoiTuong,
                                TuDongCapNhat: itemData.TuDongCapNhat,
                                GiamGiaTheoPhanTram: itemData.GiamGiaTheoPhanTram,

                                Conditions: [{
                                    LoaiDieuKien: itemData.LoaiDieuKien,
                                    LoaiSoSanh: itemData.LoaiSoSanh,
                                    GiaTriSo: itemData.GiaTriSo,
                                    GiaTriBool: itemData.GiaTriBool,
                                    GiaTriThoiGian: itemData.GiaTriThoiGian,
                                    GiaTriKhuVuc: itemData.GiaTriKhuVuc,
                                    GiaTriVungMien: itemData.GiaTriVungMien,
                                }]
                            }
                            self.DM_NhomDoiTuong_ChiTiets_Unique.push(objNew);
                        }
                        else {
                            for (let j = 0; j < self.DM_NhomDoiTuong_ChiTiets_Unique().length; j++) {
                                if (self.DM_NhomDoiTuong_ChiTiets_Unique()[j].ID_NhomDoiTuong === itemData.ID_NhomDoiTuong) {
                                    var itemDK = {
                                        LoaiDieuKien: itemData.LoaiDieuKien,
                                        LoaiSoSanh: itemData.LoaiSoSanh,
                                        GiaTriSo: itemData.GiaTriSo,
                                        GiaTriBool: itemData.GiaTriBool,
                                        GiaTriThoiGian: itemData.GiaTriThoiGian,
                                        GiaTriKhuVuc: itemData.GiaTriKhuVuc,
                                        GiaTriVungMien: itemData.GiaTriVungMien,
                                    }
                                    self.DM_NhomDoiTuong_ChiTiets_Unique()[j].Conditions.push(itemDK);
                                    break;
                                }
                            }
                        }
                    }
                }
            });
        }
        else {
            var lstNhomChitiet = localStorage.getItem('lcNhomDT_ChiTiet');
            if (lstNhomChitiet !== null) {
                lstNhomChitiet = JSON.parse(lstNhomChitiet);
                self.DM_NhomDoiTuong_ChiTiets(lstNhomChitiet);

                var arrIDNhomAdd = [];
                var objNhom = [];
                for (let i = 0; i < self.DM_NhomDoiTuong_ChiTiets().length; i++) {
                    var itemData = self.DM_NhomDoiTuong_ChiTiets()[i];
                    if ($.inArray(itemData.ID_NhomDoiTuong, arrIDNhomAdd) === -1) {

                        arrIDNhomAdd.push(itemData.ID_NhomDoiTuong);

                        var objNew = {
                            ID_NhomDoiTuong: itemData.ID_NhomDoiTuong,
                            TuDongCapNhat: itemData.TuDongCapNhat,
                            GiamGiaTheoPhanTram: itemData.GiamGiaTheoPhanTram,

                            Conditions: [{
                                LoaiDieuKien: itemData.LoaiDieuKien,
                                LoaiSoSanh: itemData.LoaiSoSanh,
                                GiaTriSo: itemData.GiaTriSo,
                                GiaTriBool: itemData.GiaTriBool,
                                GiaTriThoiGian: itemData.GiaTriThoiGian,
                                GiaTriKhuVuc: itemData.GiaTriKhuVuc,
                                GiaTriVungMien: itemData.GiaTriVungMien,
                            }]
                        }
                        self.DM_NhomDoiTuong_ChiTiets_Unique.push(objNew);
                    }
                    else {
                        for (let j = 0; j < self.DM_NhomDoiTuong_ChiTiets_Unique().length; j++) {
                            if (self.DM_NhomDoiTuong_ChiTiets_Unique()[j].ID_NhomDoiTuong === itemData.ID_NhomDoiTuong) {
                                var itemDK = {
                                    LoaiDieuKien: itemData.LoaiDieuKien,
                                    LoaiSoSanh: itemData.LoaiSoSanh,
                                    GiaTriSo: itemData.GiaTriSo,
                                    GiaTriBool: itemData.GiaTriBool,
                                    GiaTriThoiGian: itemData.GiaTriThoiGian,
                                    GiaTriKhuVuc: itemData.GiaTriKhuVuc,
                                    GiaTriVungMien: itemData.GiaTriVungMien,
                                }
                                self.DM_NhomDoiTuong_ChiTiets_Unique()[j].Conditions.push(itemDK);
                                break;
                            }
                        }
                    }
                }
                console.log(2, self.DM_NhomDoiTuong_ChiTiets_Unique())
            }
        }
    }

    function NangNhom_KhachHang(itemUsing, loaiCheck) {

        // 1. Khachhang, 2. HoaDon, 3. PhieuThu
        var idDoiTuong = '';
        switch (loaiCheck) {
            case 1:
                idDoiTuong = itemUsing.ID;
                break;
            case 2:
                idDoiTuong = itemUsing.ID_DoiTuong;
                break;
            case 3:
                idDoiTuong = itemUsing.ID_DoiTuong;
                break;
        }

        var arrAddDB = [];
        if (initLoad && idDoiTuong !== null && idDoiTuong !== undefined) {

            var itemDT = $.grep(self.DoiTuongs(), function (x) {
                return x.ID === idDoiTuong;
            });

            if (itemDT.length > 0) {

                var thangsinh = 0;
                var ngaySinhFull = null;
                var namsinh = 0;
                var dtNow = new Date();
                var monthNow = (dtNow.getMonth() + 1).toString();
                var dateNow = (dtNow.getDate()).toString();
                var yearNow = (dtNow.getFullYear()).toString();

                if (itemDT[0].NgaySinh_NgayTLap !== null) {
                    ngaySinhFull = new Date(moment(itemDT[0].NgaySinh_NgayTLap).format('YYYY-MM-DD'));
                }

                if (ngaySinhFull !== null && itemDT[0].DinhDang_NgaySinh !== 'yyyy') {
                    if (itemDT[0].DinhDang_NgaySinh !== 'yyyy') {
                        // get ThangSinh of KH (chi get nhung KH co dih dang ngay thang la d/M/y, d/M, M/y)
                        thangsinh = ngaySinhFull.getMonth() + 1;
                    }
                    if (itemDT[0].DinhDang_NgaySinh.indexOf('yyyy') > -1) {
                        // get NamSinh of KH (chi get nhung KH co dih dang ngay thang la d/M/y, M/y, y)
                        namsinh = ngaySinhFull.getFullYear();
                    }
                }

                var arrNhomAdd = [];
                var arrIDNhomChiTiet = [];
                for (let i = 0; i < self.DM_NhomDoiTuong_ChiTiets_Unique().length; i++) {

                    var add = false;
                    var haveCondition = false;
                    var itemFor = self.DM_NhomDoiTuong_ChiTiets_Unique()[i];

                    arrIDNhomChiTiet.push(itemFor.ID_NhomDoiTuong);

                    for (let j = 0; j < itemFor.Conditions.length; j++) {

                        var itemCondition = itemFor.Conditions[j];
                        var gtriSo = itemCondition.GiaTriSo;

                        switch (itemCondition.LoaiDieuKien) {
                            case 1:// TongBanTruTraHang
                                // neu chua co/ hoac thoa man if 1 thi moi check tiep
                                if (haveCondition == false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBanTruTraHang > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBanTruTraHang >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBanTruTraHang === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBanTruTraHang <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBanTruTraHang < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            case 2: // TongBan
                                if (haveCondition == false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBan > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBan >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBan === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBan <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT[0].TongBan < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            // LoaiDieuKien = 3. Thời gian mua hàng (todo)
                            case 4:// SoLanMuaHang
                                if (haveCondition == false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT[0].SoLanMuaHang > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT[0].SoLanMuaHang >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT[0].SoLanMuaHang === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT[0].SoLanMuaHang <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT[0].SoLanMuaHang < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            case 5: // NoHienTai
                                if (haveCondition == false || (haveCondition && add)) {
                                    switch (itemCondition.LoaiSoSanh) {
                                        case 1:
                                            haveCondition = true;
                                            add = (itemDT[0].NoHienTai > gtriSo);
                                            break;
                                        case 2:
                                            haveCondition = true;
                                            add = (itemDT[0].NoHienTai >= gtriSo);
                                            break;
                                        case 3:
                                            haveCondition = true;
                                            add = (itemDT[0].NoHienTai === gtriSo);
                                            break;
                                        case 4:
                                            haveCondition = true;
                                            add = (itemDT[0].NoHienTai <= gtriSo);
                                            break;
                                        case 5:
                                            haveCondition = true;
                                            add = (itemDT[0].NoHienTai < gtriSo);
                                            break;
                                    }
                                }
                                break;
                            case 6: // ThangSinh
                                if (thangsinh > 0) {
                                    if (haveCondition == false || (haveCondition && add)) {
                                        switch (itemCondition.LoaiSoSanh) {
                                            case 1:
                                                haveCondition = true;
                                                add = (thangsinh > gtriSo);
                                                break;
                                            case 2:
                                                haveCondition = true;
                                                add = (thangsinh >= gtriSo);
                                                break;
                                            case 3:
                                                haveCondition = true;
                                                add = (thangsinh === gtriSo);
                                                break;
                                            case 4:
                                                haveCondition = true;
                                                add = (thangsinh <= gtriSo);
                                                break;
                                            case 5:
                                                haveCondition = true;
                                                add = (thangsinh < gtriSo);
                                                break;
                                        }
                                    }
                                }
                                else {
                                    haveCondition = true;
                                    add = false;
                                }
                                break;
                            case 7: // Tuoi
                                if (namsinh !== 0) {
                                    // format again NgaySingFull
                                    ngaySinhFull = moment(ngaySinhFull).format('YYYY-MM-DD');
                                    // get namSinh can thiet cho tuoi
                                    var namsinhSubstract = yearNow - itemCondition.GiaTriSo;
                                    var ngayFullCompare1 = moment((namsinhSubstract - 1) + '-' + monthNow + '-' + dateNow, 'YYYY-MM-DD').format('YYYY-MM-DD');
                                    var ngayFullCompare2 = moment(namsinhSubstract + '-' + monthNow + '-' + dateNow, 'YYYY-MM-DD').format('YYYY-MM-DD');

                                    if (haveCondition == false || (haveCondition && add)) {
                                        switch (itemCondition.LoaiSoSanh) {
                                            case 1:
                                                haveCondition = true;
                                                add = (ngaySinhFull < ngayFullCompare1);
                                                break;
                                            case 2:
                                                haveCondition = true;
                                                add = (ngaySinhFull <= ngayFullCompare2);
                                                break;
                                            case 3:
                                                haveCondition = true;
                                                add = (ngaySinhFull === ngayFullCompare2);
                                                break;
                                            case 4:
                                                haveCondition = true;
                                                add = (ngaySinhFull >= ngayFullCompare2);
                                                break;
                                            case 5:
                                                haveCondition = true;
                                                add = (ngaySinhFull > ngayFullCompare2);
                                                break;
                                        }
                                    }
                                }
                                else {
                                    haveCondition = true;
                                    add = false;
                                }
                                break;
                            case 8: // GioiTinh
                                if (haveCondition == false || (haveCondition && add)) {
                                    haveCondition = true;
                                    add = (itemCondition.GiaTriBool === itemDT[0].GioiTinhNam);
                                }
                                break;
                            case 9: // KhuVuc
                                if (haveCondition == false || (haveCondition && add)) {
                                    haveCondition = true;
                                    add = (itemCondition.GiaTriKhuVuc.trim().toLowerCase() === itemDT[0].ID_TinhThanh.trim().toLowerCase());
                                }
                                break;
                            // LoaiDieuKien = 10  (VungMien_TODO)
                        }

                        if (haveCondition && add === false) {
                            // neu ton tai 1 dieu kien khong thoan man ==> ESC for loop
                            break;
                        }
                    }

                    if (haveCondition && add) {
                        arrNhomAdd.push(self.DM_NhomDoiTuong_ChiTiets_Unique()[i].ID_NhomDoiTuong);
                    }
                }

                var idNhomDTs = '';
                var arrIDNhomOld = '';
                if (itemDT[0].ID_NhomDoiTuong !== null && itemDT[0].ID_NhomDoiTuong !== undefined) {
                    idNhomDTs = itemDT[0].ID_NhomDoiTuong.toLowerCase();
                    arrIDNhomOld = idNhomDTs.split(',');
                    arrIDNhomOld = RemoveItemEmpty(arrIDNhomOld);
                }
                for (let i = 0; i < arrNhomAdd.length; i++) {
                    // neu doituong da thuoc nhom nay --> khong can add nua
                    if ($.inArray(arrNhomAdd[i].trim().toLowerCase(), arrIDNhomOld) === -1) {
                        arrAddDB.push(arrNhomAdd[i]);
                    }
                }

                console.log('arrAddDB ', arrAddDB);

                // REMOVE NHOM 
                var arrIDRemove = [];
                for (let i = 0; i < arrIDNhomOld.length; i++) {
                    // if exist at  arrIDNhomOld(old) && exist in lst arrIDNhomChiTiet , but not exist in arrNhomAdd(new) --> remove
                    if ($.inArray(arrIDNhomOld[i].trim(), arrNhomAdd) === -1
                        && $.inArray(arrIDNhomOld[i].trim(), arrIDNhomChiTiet) > -1) {
                        arrIDRemove.push(arrIDNhomOld[i].trim());
                    }
                }
                console.log('arrIDRemove ', arrIDRemove);

                // get all nhom in arrAddDB, arrIDNhomOld, not exist in arrIDRemove
                var arrIDNhomAfter = [];
                for (let i = 0; i < arrAddDB.length; i++) {
                    if ($.inArray(arrAddDB[i], arrIDRemove) === -1) {
                        arrIDNhomAfter.push(arrAddDB[i].trim());
                    }
                }

                for (let i = 0; i < arrIDNhomOld.length; i++) {
                    if ($.inArray(arrIDNhomOld[i], arrIDRemove) === -1) {
                        arrIDNhomAfter.push(arrIDNhomOld[i].trim());
                    }
                }

                // remove Id_Nhom =''
                arrIDNhomAfter = RemoveItemEmpty(arrIDNhomAfter);
                console.log('arrIDNhomAfter ', arrIDNhomAfter);

                // update again DoiTuongs
                var idNhomsAfter = '';
                var tenNhomsAfter = '';
                for (let i = 0; i < arrIDNhomAfter.length; i++) {
                    var itemNhom = $.grep(self.NhomDoiTuongDB(), function (x) {
                        return x.ID.toString().toLowerCase() === arrIDNhomAfter[i].toLowerCase();
                    });
                    // get id, name group from arrAddDB
                    if (itemNhom.length > 0) {
                        idNhomsAfter += itemNhom[0].ID.toString() + ',';
                        tenNhomsAfter += itemNhom[0].TenNhomDoiTuong + ',';
                    }
                }

                // update ID_Nhom for DoiTuongs
                var itemDTUpdate = [];
                for (let i = 0; i < self.DoiTuongs().length; i++) {
                    if (self.DoiTuongs()[i].ID === idDoiTuong) {
                        self.DoiTuongs()[i].ID_NhomDoiTuong = idNhomsAfter;
                        itemDTUpdate = self.DoiTuongs()[i];
                        console.log('itemDTUpdate ', itemDTUpdate)
                        break;
                    }
                }

                // update again KH indexdb
                db.DM_DoiTuong.put(itemDTUpdate);

                // add thong bao
                var arrGroupUniqueOld = $.unique(arrIDNhomOld.sort());
                var arrGroupUniqueNew = $.unique(arrIDNhomAfter.sort());
                if (arrGroupUniqueOld.length !== arrGroupUniqueNew.length) {
                    ShowMessage_Success("Đã tự động cập nhật nhóm cho khách hàng " + itemDT[0].MaDoiTuong);
                }
            }
        }
    }

    function UpdateNhomKH_DB(idDoiTuong) {
        var itemDT = $.grep(self.DoiTuongs(), function (x) {
            return x.ID === idDoiTuong;
        });

        if (itemDT.length > 0) {
            var lstNhomNang = [];
            var arrIDNhom = itemDT[0].ID_NhomDoiTuong.trim().split(',');
            for (let i = 0; i < arrIDNhom.length; i++) {
                if (arrIDNhom[i].trim() !== '') {
                    var objDTNhom = {
                        ID_DoiTuong: idDoiTuong,
                        ID_NhomDoiTuong: arrIDNhom[i].trim(),
                    }
                    lstNhomNang.push(objDTNhom);
                }
            }

            if (lstNhomNang.length > 0) {
                Update_ManyNhom(lstNhomNang);
            }
            else {
                // delete all nhom of DoiTuon in DB
                ajaxHelper(DMDoiTuongUri + 'DeleteAllNhom_ofDoiTuong?idDoiTuong=' + idDoiTuong, 'PUT').done(function (x) {

                });
            }
        }
    }

    self.InHoaDon = function (maChungTu, isPrintDraft) {

        // reset numberPrint before print (if not reset, get value old --> wrong)
        Reset_SettingPrint();

        var idTemp = $('#ddlTempPrint').val();

        // if set print when pay --> print
        var lcSetPrint = localStorage.getItem('SetPrintNH');
        if (lcSetPrint !== null) {
            lcSetPrint = JSON.parse(lcSetPrint);
            // alway have 1 record
            for (let i = 0; i < lcSetPrint.length; i++) {
                var itemSet = lcSetPrint[i];
                self.setPrintDraft(itemSet.PrintDraft);
                self.setPrintPay(itemSet.PrintPay);
                self.numberOfPrint(formatNumberToInt(itemSet.NumberOfPrint));
                idTemp = itemSet.TempPrint;
                break;
            }
        }

        var print = false;
        // if dang in Nhap --> cho phep in
        if (isPrintDraft === true || maChungTu == 'SQPT') {
            print = true;
        }
        else {
            print = self.setPrintPay();
        }

        if (print) {
            var itemMauIn = [];
            if (idTemp === undefined || idTemp === null) {
                // neu chua cai dat mau in --> get mau mac dinh
                itemMauIn = $.grep(self.DM_MauIn(), function (x) {
                    return x.MaLoaiChungTu === maChungTu && x.LaMacDinh;
                });
            }
            else {
                // get temp by ID is selected
                itemMauIn = $.grep(self.DM_MauIn(), function (x) {
                    return x.ID === idTemp;
                });
            }

            if (itemMauIn.length > 0) {
                var dulieuMauIn = ReplaceString_toData(itemMauIn[0].DuLieuMauIn);
                var dulieuMauIn1 = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                dulieuMauIn1 = dulieuMauIn1.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item2=[], item4 =[], item5 =[];"
                    + "; var item3=" + JSON.stringify(self.InforHDprintf()) + "; </script>");
                dulieuMauIn1 = dulieuMauIn1.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(dulieuMauIn, dulieuMauIn1, self.numberOfPrint());
            }
            else {
                var idDiv = '';
                switch (maChungTu) {
                    case 'HDBL':
                        idDiv = '#content-print-BanLe';
                        break;
                    case 'SQPT':
                        idDiv = '#content-print-PhieuThu';
                        break;
                }
                self.InHoaDon_TextFile(idDiv);
            }
        }
    }

    self.InHoaDon_TextFile = function (idDivPrint) {
        var contents = $(idDivPrint).html();
        contents = contents.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
        PrintExtraReport(contents, '', self.numberOfPrint());
    }


    function Reset_SettingPrint() {
        self.setPrintDraft(false);
        self.setPrintPay(false);
        self.numberOfPrint(1);
    }

    // setting print (new interface)
    self.ChangeSetPrint = function () {
        console.log('change_setPrintPay', self.setPrintDraft(), self.setPrintTicket())

        self.agree_SettingPrint();
    }

    self.BindSettingPrint = function (isFirstLoad = false) {

        // reset before bind (case not yet set print with loaiHoaDon)
        //Reset_SettingPrint();

        // get all temp with maChungTu
        var lstTemp = $.grep(self.DM_MauIn(), function (x) {
            return x.MaLoaiChungTu === 'HDBL';
        });

        // check xem co mau mac dinh chua
        var itemDefault = $.grep(self.DM_MauIn(), function (x) {
            return x.MaLoaiChungTu === 'HDBL' && x.LaMacDinh;
        });

        if (itemDefault.length === 0) {
            // neu chua co -->  push MauMacDinh intto lst Mau
            var tempDefault = {
                ID: 0,
                TenMauIn: 'Mẫu mặc định',
                LaMacDinh: true,
            }
            lstTemp.unshift(tempDefault);
        }

        // sort by LaMacDinh is top
        lstTemp = lstTemp.sort(function (a, b) {
            var x = a.LaMacDinh, y = b.LaMacDinh;
            return (x === y) ? 0 : x ? -1 : 1;
        });

        var itemSet = [];
        var lcSetPrint = localStorage.getItem('SetPrintNH');
        if (lcSetPrint !== null) {
            lcSetPrint = JSON.parse(lcSetPrint);
            // if had set print --> get from cache

            itemSet = $.grep(lcSetPrint, function (x) {
                return x.LoaiHoaDon === 1;
            });
        }

        if (itemSet.length === 0) {
            self.setPrintDraft(false);
            self.setPrintPay(false);
            self.numberOfPrint(1);
            self.ListTempPrint(lstTemp);
        }
        else {
            // find id_temprint in lst
            var miEx = $.grep(lstTemp, function (x) {
                return x.ID === itemSet[0].TempPrint;
            });

            // set value before bind ListTempPrint (because auto call func selectedTempPrint.subscrible)
            self.setPrintDraft(itemSet[0].PrintDraft);
            self.setPrintPay(itemSet[0].PrintPay);
            self.numberOfPrint(itemSet[0].NumberOfPrint);
            self.ListTempPrint(lstTemp);

            if (miEx.length > 0) {
                // don't reset SettingPrint
                // set value for ddlTempPrint after bind ListTempPrint
                $('#ddlTempPrint').val(itemSet[0].TempPrint);
            }
        }
        if (isFirstLoad === false) {
            var divSetPrint = $('.install-notifi');
            if (divSetPrint.is(':hidden')) {
                // hide div 1,2 (setQuyCach, TonKho < 0)
                divSetPrint.find('#setPrint div:lt(3)').hide();
                divSetPrint.show();
            }
            else {
                divSetPrint.hide();
            };
        }
    }

    self.agree_SettingPrint = function () {
        var idTemprint = $('#ddlTempPrint').val();
        var lsSetPrint = localStorage.getItem('SetPrintNH');
        if (lsSetPrint !== null) {
            lsSetPrint = JSON.parse(lsSetPrint);

            // if exist --> update new value
            for (let i = 0; i < lsSetPrint.length; i++) {
                lsSetPrint[i].PrintDraft = self.setPrintDraft();
                lsSetPrint[i].PrintPay = self.setPrintPay();
                lsSetPrint[i].PrintTicket = self.setPrintTicket();
                lsSetPrint[i].NumberOfPrint = self.numberOfPrint();
                lsSetPrint[i].TempPrint = idTemprint;
                break;
            }
        }
        else {
            lsSetPrint = [];
            var objSetPrint = {
                LoaiHoaDon: 1, // 1.HDBL, 3.DH, 6.TH, 4.DTH (use for setting print)
                PrintDraft: self.setPrintDraft(),
                PrintPay: self.setPrintPay(),
                PrintTicket: self.setPrintTicket(),
                NumberOfPrint: self.numberOfPrint(),
                TempPrint: idTemprint,
            };
            lsSetPrint.push(objSetPrint);
        }

        localStorage.setItem('SetPrintNH', JSON.stringify(lsSetPrint));

        $('#thietlapmauin').modal('hide');
    }

    self.increaseNumberPrint = function (isIncrease) {
        var thisObj = $('#txtNumberOfPrint');
        var numberOld = formatNumberToInt(thisObj.val());

        if (isIncrease) {
            numberOld = numberOld + 1;
        }
        else {
            numberOld = numberOld - 1;
            if (numberOld <= 0) {
                numberOld = 1;
            }
        }

        thisObj.val(numberOld);
        self.numberOfPrint(numberOld);

        self.agree_SettingPrint();
    }

    self.editNumberOfPrint = function () {
        var number = formatNumberToInt($('#txtNumberOfPrint').val());
        self.numberOfPrint(number);
        self.agree_SettingPrint();
    }

    // hide/show colum
    self.show_STT = ko.observable(true);
    self.show_ProductCode = ko.observable(true);
    self.show_ProductName = ko.observable(true);
    self.show_ProductPrice = ko.observable(true);
    self.show_SumPrice = ko.observable(true);

    self.BindSet_HideShowColumCTHD = function () {
        $('#HienThi').modal('show');
        var lcSetCTHD = localStorage.getItem('lcHideShowColumCTHD_NH');
        if (lcSetCTHD !== null) {
            lcSetCTHD = JSON.parse(lcSetCTHD);

            for (let i = 0; i < lcSetCTHD.length; i++) {
                self.show_STT(lcSetCTHD[i].ShowSTT);
                self.show_ProductCode(lcSetCTHD[i].ShowProductCode);
                self.show_ProductName(lcSetCTHD[i].ShowProductName);
                self.show_ProductPrice(lcSetCTHD[i].ShowProductPrice);
                self.show_SumPrice(lcSetCTHD[i].ShowSumPrice);
                break;
            }
        }
        else {
            self.show_STT(true);
            self.show_ProductCode(true);
            self.show_ProductName(true);
            self.show_ProductPrice(true);
            self.show_SumPrice(true);
        }

        if (self.show_STT()) {
            $('#divSet_showSTT').removeClass('main-hide');
        }
        else {
            $('#divSet_showSTT').addClass('main-hide');
        }

        if (self.show_ProductCode()) {
            $('#divSet_showProductCode').removeClass('main-hide');
        }
        else {
            $('#divSet_showProductCode').addClass('main-hide');
        }
        if (self.show_ProductName()) {
            $('#divSet_showProductName').removeClass('main-hide');
        }
        else {
            $('#divSet_showProductName').addClass('main-hide');
        }

        if (self.show_ProductPrice()) {
            $('#divSet_showProductPrice').removeClass('main-hide');
        }
        else {
            $('#divSet_showProductPrice').addClass('main-hide');
        }
        if (self.show_SumPrice()) {
            $('#divSet_showSumPrice').removeClass('main-hide');
        }
        else {
            $('#divSet_showSumPrice').addClass('main-hide');
        }
    }

    self.Setting_HideShowColumCTHD = function (idDiv) {

        var obj = $('#' + idDiv);
        obj.toggleClass("main-hide");

        switch (idDiv) {
            case 'divSet_showSTT':
                if (obj.hasClass('main-hide')) {
                    self.show_STT(false);
                }
                else {
                    self.show_STT(true);
                }
                break;
            case 'divSet_showProductCode':
                if (obj.hasClass('main-hide')) {
                    self.show_ProductCode(false);
                }
                else {
                    self.show_ProductCode(true);
                }
                break;
            case 'divSet_showProductName':
                if (obj.hasClass('main-hide')) {
                    self.show_ProductName(false);
                }
                else {
                    self.show_ProductName(true);
                }
                break;
            case 'divSet_showProductPrice':
                if (obj.hasClass('main-hide')) {
                    self.show_ProductPrice(false);
                }
                else {
                    self.show_ProductPrice(true);
                }
                break;
            case 'divSet_showSumPrice':
                if (obj.hasClass('main-hide')) {
                    self.show_SumPrice(false);
                }
                else {
                    self.show_SumPrice(true);
                }
                break;
        }
    }

    self.Agree_SettingHideShowColumCTHD = function () {
        var lcSetCTHD = localStorage.getItem('lcHideShowColumCTHD_NH');
        if (lcSetCTHD !== null) {
            lcSetCTHD = JSON.parse(lcSetCTHD);

            for (let i = 0; i < lcSetCTHD.length; i++) {
                lcSetCTHD[i].ShowSTT = self.show_STT();
                lcSetCTHD[i].ShowProductCode = self.show_ProductCode();
                lcSetCTHD[i].ShowProductName = self.show_ProductName();
                lcSetCTHD[i].ShowProductPrice = self.show_ProductPrice();
                lcSetCTHD[i].ShowSumPrice = self.show_SumPrice();
                break;
            }
        }
        else {
            lcSetCTHD = [];
            var objSet = {
                ShowSTT: self.show_STT(),
                ShowProductCode: self.show_ProductCode(),
                ShowProductName: self.show_ProductName(),
                ShowProductPrice: self.show_ProductPrice(),
                ShowSumPrice: self.show_SumPrice(),
            };
            lcSetCTHD.push(objSet);
        }
        localStorage.setItem('lcHideShowColumCTHD_NH', JSON.stringify(lcSetCTHD));
        $('#HienThi').modal('hide');

        Update_HideShowColum_forCTHD();
        var idViTri = localStorage.getItem(lcPhongBanID);
        BindHD_CTHD();
    }

    function Update_HideShowColum_forCTHD() {
        var lstCTHD = localStorage.getItem(lcListCTHD);
        if (lstCTHD !== null) {
            lstCTHD = JSON.parse(lstCTHD);
            for (let i = 0; i < lstCTHD.length; i++) {
                lstCTHD[i].ShowSTT = self.show_STT();
                lstCTHD[i].ShowProductCode = self.show_ProductCode();
                lstCTHD[i].ShowProductName = self.show_ProductName();
                lstCTHD[i].ShowProductPrice = self.show_ProductPrice();
                lstCTHD[i].ShowSumPrice = self.show_SumPrice();
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(lstCTHD));
        }
    }

    function Get_SetHideShowColumCTHD() {
        var lcSetCTHD = localStorage.getItem('lcHideShowColumCTHD_NH');
        if (lcSetCTHD !== null) {
            lcSetCTHD = JSON.parse(lcSetCTHD);

            for (let i = 0; i < lcSetCTHD.length; i++) {
                self.show_STT(lcSetCTHD[i].ShowSTT);
                self.show_ProductCode(lcSetCTHD[i].ShowProductCode);
                self.show_ProductName(lcSetCTHD[i].ShowProductName);
                self.show_ProductPrice(lcSetCTHD[i].ShowProductPrice);
                self.show_SumPrice(lcSetCTHD[i].ShowSumPrice);
                break;
            }
        }
    }

    function UpdateHD_whenChangeHD(idRandomHD) {
        var lstHD = localStorage.getItem(lcListHD);
        if (lstHD !== null) {
            lstHD = JSON.parse(lstHD);
            for (let i = 0; i < lstHD.length; i++) {
                if (lstHD[i].IDRandom === idRandomHD) {
                    let sum = lstHD[i].TongTienHang;
                    if (lstHD[i].PTChiPhi > 0) {
                        lstHD[i].TongChiPhi = parseInt(lstHD[i].PTChiPhi * sum / 100);
                    }
                    if (lstHD[i].TongChietKhau > 0) {
                        lstHD[i].TongGiamGia = parseInt(lstHD[i].TongChietKhau * sum / 100);
                    }
                    if (lstHD[i].PTThue > 0) {
                        lstHD[i].TongTienThue = parseInt(lstHD[i].PTThue * (sum - lstHD[i].TongGiamGia) / 100);
                        lstHD[i].TongTienThue = isNaN(lstHD[i].TongTienThue) ? 0 : lstHD[i].TongTienThue;
                    }
                    lstHD[i].TongGiamGiaKM_HD = parseInt(lstHD[i].TongGiamGia) + parseInt(lstHD[i].KhuyeMai_GiamGia);

                    var phaiTT = sum + parseInt(lstHD[i].TongTienThue) - parseInt(lstHD[i].TongGiamGiaKM_HD) + formatNumberToFloat(lstHD[i].TongChiPhi);
                    lstHD[i].PhaiThanhToan = phaiTT < 0 ? 0 : phaiTT;

                    lstHD[i].DaThanhToan = lstHD[i].PhaiThanhToan;
                    lstHD[i].TienMat = lstHD[i].PhaiThanhToan;
                    lstHD[i].TienGui = 0;
                    lstHD[i].TienATM = 0;
                    lstHD[i].TienThua = 0;
                    lstHD[i].StatusTBNB = false;
                }
            }
            localStorage.setItem(lcListHD, JSON.stringify(lstHD));
        }
    }

    self.FullScreen = function () {
        toggleFullScreen();

        var elm = $('#ESC-full-screen');
        if ($(elm).css('display') == 'none') {
            $('#full-screen').attr('title', '');
            elm.show();
        }
        else {
            $('#full-screen').attr('title', 'Toàn màn hình');
            elm.hide();
        }
    }

    function toggleFullScreen() {
        if (!document.fullscreenElement &&    // alternative standard method
            !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement) {  // current working methods
            if (document.documentElement.requestFullscreen) {
                document.documentElement.requestFullscreen();
            } else if (document.documentElement.msRequestFullscreen) {
                document.documentElement.msRequestFullscreen();
            } else if (document.documentElement.mozRequestFullScreen) {
                document.documentElement.mozRequestFullScreen();
            } else if (document.documentElement.webkitRequestFullscreen) {
                document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
            }
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            } else if (document.msExitFullscreen) {
                document.msExitFullscreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitExitFullscreen) {
                document.webkitExitFullscreen();
            }
        }
    }

    self.KeyUp_GiamGiaNhomKH = function () {
        var $this = $('#txtGiamGiaNhomKH');
        formatNumberObj($this);

        var valThis = formatNumberToFloat($this.val());
        if ($('#LaPhanTram').hasClass('active-re')) {
            if (valThis > 100) {
                $this.val(100);
            }
        }
    }

    self.Change_MauIn = function () {
        var idChosed = $('#ddlTempPrint').val();
        if (idChosed !== undefined) {
            self.agree_SettingPrint();
        }
    }

    function UpdateAppCache() {
        ajaxHelper(BH_HoaDonUri + 'UpdateAppcache', 'POST').done(function () {
            console.log('UpdateAppcache susscess');
        })
    }

    // tien goi y
    self.MenhGiaTienGoiy = ko.observableArray();
    self.index_NVienBanGoi = ko.observable(0);
    self.ListNVien_BanGoi = ko.observableArray();
    self.GridNVienBanGoi_Chosed = ko.observableArray();
    self.ChietKhauHoaDon = ko.observableArray();
    self.HoaDon_DoanhThu = ko.observable(0);
    self.HoaDon_ThucThu = ko.observable(0);
    self.HoaDon_ConNo = ko.observable(0);
    self.IsShareDiscount = ko.observable('2');
    self.TienTheGiaTri_PhieuThu = ko.observable(0);
    // not use, because used comond with forrm BanHang
    self.HoaHongHD_ClickTabNVBanGoi = function () {

    }
    self.Change_IsShareDiscount = function (x) {
    }
    self.SoDuTheGiaTri = ko.observable(0);
    self.CongNoThe = ko.observable(0);
    self.editTheGiaTri = function () {

    }

    self.ShowInfor_SoDuTheGiaTri = function () {

    }

    self.editTheGiaTri_PhieuThu = function () {

    }


    // dichvu tinh theo thoigian
    self.GetHoursBetweenDates_ToCurrent = function (start, end = null) {
        var diff = 0;
        if (end !== null) {
            diff = (new Date(end) - new Date(start)) / 1000;
        }
        else {
            diff = (new Date() - new Date(start)) / 1000;
        }
        var hours = RoundDecimal(diff / 3600, 3);
        return hours; // because 1hour = soluong 1
    }

    CaculatorTimeCTHD();
    function CaculatorTimeCTHD() {
        var cthd = localStorage.getItem(lcListCTHD);
        if (cthd !== null) {
            cthd = JSON.parse(cthd);
            var idRandomHD = self.HoaDons().IDRandom();
            var countDVTheoGio = 0;
            for (let i = 0; i < cthd.length; i++) {
                if (cthd[i].IDRandomHD === idRandomHD) {
                    if (cthd[i].Stop === false && cthd[i].DichVuTheoGio === 1) {
                        countDVTheoGio += 1;
                        let sVal = self.GetHoursBetweenDates_ToCurrent(cthd[i].ThoiGian);
                        cthd[i].SoLuong = sVal > 0 ? sVal : 0;
                        cthd[i].ThoiGianThucHien = cthd[i].SoLuong;
                        cthd[i].ThanhTien = cthd[i].SoLuong * cthd[i].GiaBan;

                        // đổi giờ ---> giây & cộng lại
                        let seconds = (new Date(cthd[i].ThoiGian)).getMilliseconds() + cthd[i].SoLuong * 3600;
                        let ht = (new Date()).setMilliseconds(seconds);
                        cthd[i].ThoiGianHoanThanh = moment(new Date(ht)).format('YYYY-MM-DD HH:mm');
                        $('#munber_' + cthd[i].IDRandom).val(formatNumber3Digit(sVal));
                        $('#sum-f_' + cthd[i].IDRandom).text(formatNumber3Digit(cthd[i].ThanhTien));
                    }
                }
            }
            localStorage.setItem(lcListCTHD, JSON.stringify(cthd));

            if (countDVTheoGio > 0) {
                let sum = Sum_ThanhTienCTHD(cthd, idRandomHD);
                UpdateHD_whenChangeCTHD(sum, idRandomHD);
                UpdateDiemGiaoDich_andResetDiemQuyDoi_forHoaDon(idRandomHD);
                Update_TienThue_CTHD(idRandomHD);
                AssignValue_ofHoaDon(idRandomHD);
            }
            setTimeout(CaculatorTimeCTHD, 5000);
        }
    }

    self.CTHD_ShowPoupTime = function (item) {
        var ctDoing = FindCTHD_isDoing(item.IDRandom);

        var li = $(event.currentTarget).closest('li');
        var left = 0;
        switch (self.windowWidth()) {
            case 768:
                left = 50;
                break;
            case 1024:
            case 1016:
                left = 570;
                break;
            case 1280:
                left = 800;
                break;
            case 1366:
                left = 900;
                break;
            case 1392:
                left = 950;
                break;
            case 1600:
                left = 1150;
                break;
        }

        $('#popup-timer').css('top', (li.position().top + 145) + 'px');
        $('#popup-timer').css('left', left + 'px'); // width: header_left + tenhanghoa
        popupTimeCTHD.ShowPopup(ctDoing, true, $(event.currentTarget).closest('li'));
    }

    self.CTHD_HidePoupTime = function () {
        var cthdChange = popupTimeCTHD.DichVu_isDoing();
        $('#munber_' + cthdChange.IDRandom).val(formatNumber3Digit(cthdChange.SoLuong));
        $('#sum-f_' + cthdChange.IDRandom).text(formatNumber3Digit(cthdChange.ThanhTien));
        $('#tinggio_' + cthdChange.IDRandom).text(popupTimeCTHD.TimeStart());
        if (popupTimeCTHD.Stopping()) {
            $('#munber_' + cthdChange.IDRandom).prop('disabled', false);
        }
        else {
            $('#munber_' + cthdChange.IDRandom).prop('disabled', true);
        }
    }
};

var newModelBanLe = new NewModel_HoaDon();
ko.applyBindings(newModelBanLe, document.getElementById('divPage'));

var newModal_AddKhachHang = new PartialView_NewKhachHang();
ko.applyBindings(newModal_AddKhachHang, document.getElementById('modalNewCustomer'));

var popupTimeCTHD = new popupTime();
ko.applyBindings(popupTimeCTHD, document.getElementById('popup-timer'));

ko.observableArray.fn.refresh = function () {
    var data = this().slice(0);
    this([]);
    this(data);
};

$('body').on('popuptime_ChangeTime', function () {
    newModelBanLe.CTHD_HidePoupTime(true);
});


var clickVNDpr = true;
$(document).on('click', '.parent-price-1', function () {

    $(this).siblings(".arrow-left2").toggle();

    $(function () {
        $('.price-sa').select();
    })

    $(".arrow-left").mouseup(function () {
        return false
    });
    $(".price-fist-1").mouseup(function () {
        return false
    });
    $(document).mouseup(function () {
        $(".arrow-left2").hide();
    });

    if ($('#ptGiam').text() !== '') {
        $(".price-sa").val($('#ptGiam').text());
    }
    if ($('#dvtGiamParent').text() === '%') {
        $('.vnd1').removeClass('gb');
        $('.senter1').addClass('gb');
        clickVNDpr = false;
    }
    else {
        $('.vnd1').addClass('gb');
        $('.senter1').removeClass('gb');
        clickVNDpr = true;
    }
});

var clickVNDTax = true;
$(document).on('click', '#tdTax', function () {

    $(this).find(".arrow-left3").show();

    $(function () {
        $('#txtTax').select();
    })

    $(".arrow-left2").hide();// hide div giamgia
    // hide div Tax 
    $(document).mouseup(function () {
        $(".arrow-left3").hide();
    });

    $(this).mouseup(function () {
        return false;
    });

    if ($('#ptTax').text() !== '0' && ($('#ptTax').text() !== 0)) {
        $("#txtTax").val($('#ptTax').text());
    }
    else {
        $("#txtTax").val($('.parent-tax').text());
    }

    if ($('#dvtTax').text() === '%') {
        $('.vnd2').removeClass('gb');
        $('.senter2').addClass('gb');
        clickVNDTax = false;
    }
    else {
        // default %
        $('.vnd2').addClass('gb');
        $('.senter2').removeClass('gb');
        clickVNDTax = true;
    }
});

function getNumber(e, obj) {
    var elementAfer = $(obj).next();
    if (!elementAfer.hasClass('active-re')) {
        // chi cho phep nhap so
        return keypressNumber(e);
    }
    else {
        var keyCode = window.event.keyCode || e.which;
        if (keyCode < 48 || keyCode > 57) {
            // cho phep nhap dau .
            if (keyCode === 8 || keyCode === 127 || keyCode === 46) {
                return;
            }
            return false;
        }
    }
}

function GetTimeNow_HHmm() {
    var d = new Date();
    var hh = d.getHours();
    var mm = d.getMinutes();
    if (hh < 10) {
        hh = '0' + hh;
    };
    if (mm < 10) {
        mm = '0' + mm;
    }
    return hh + ":" + mm;
}

function SetActiveHD(obj) {
    $('.divLstHoaDon span.active').each(function () {
        $(this).removeClass('active');
    });
    $(obj).addClass('active');
}

function showFocus($this) {
    $('#showseachPage li').each(function (i) {
        $(this).removeClass('hoverenabled');
        if ($this[0].id === $(this)[0].id) {
            newModelBanLe.selectted(i);
        }
    });
    $this.addClass('hoverenabled');
}

function HideLostFocust() {
    $('#showseach_Kh, #showseach_KhPT').delay(300).hide(0, function () {
    });
}

var DateNgaySinh;

$(window.document).on('shown.bs.modal', '.modal', function () {
    window.setTimeout(function () {
        $('[autofocus]', this).focus();
        $('[autofocus]').select();
    }.bind(this), 100);
    $('.datepicker_mask').datetimepicker({
        timepicker: false,
        mask: false,
        format: 'd/m/Y H:i',
    });
    DateNgaySinh = $("#txtNgaySinh").datepicker({
        showOn: 'focus',
        altFormat: "dd/mm/yy",
        buttonImage: '/Content/images/icon/ngaysinh.png',
        showOn: "button",
        buttonImageOnly: true,
        dateFormat: "dd/mm/yy"
    }).mask('99/99/9999').on("change", function (e) {
        console.log("Date changed: ", e.target.value);
        newModal_AddKhachHang.newDoiTuong().NgaySinh_NgayTLap(e.target.value);
    });
});

$('body').on('click', '.price-fist2', function () {
    $(".arrow-left1").mouseup(function () {
        return false
    });
    $(document).mouseup(function () {
        $(".arrow-left1").hide();
    });
});

$(document).on('click', '#tdPhiDV', function () {

    $(this).find(".arrow-left3").show();
    $(function () {
        $('#txtcharge').select();
    });

    $(".arrow-left2, #tdTax .arrow-left3").hide();// hide div giamgia, thue
    // hide div Tax 
    $(document).mouseup(function () {
        $(".arrow-left3").hide();
    });

    $(this).mouseup(function () {
        return false;
    });
});
