function ViewModal() {
    var self = this;
    self.numbersPrintHD = ko.observable(1);
    self.NhomHangHoas = ko.observableArray();
    self.SumNumberPageReportHangHoa = ko.observableArray();
    self.HangHoaDieuChinh = ko.observableArray();
    self.DS_DieuChinh = ko.observableArray();
    self.HangHoas_seach = ko.observableArray();
    self.selectedHH = ko.observable();
    self.MangChiNhanh = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    self.DonVis = ko.observableArray();
    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    self.HoaDons = ko.observableArray();
    self.TenChiNhanh = ko.observable();
    self.checkTamLuu = ko.observable(true);
    self.checkHoanThanh = ko.observable(true);
    self.checkHuy = ko.observable(false);
    self.ColumnsExcel = ko.observableArray();
    self.DieuChinhGiaVon_ThayDoiThoiGian = ko.observable();
    self.TongGV_HienTai = ko.observable(0);
    self.TongGV_Moi = ko.observable(0);
    self.TongGV_ChenhLech = ko.observable(0);
    self.HangHoa_XemDs = ko.observable();
    self.PhieuDieuChinh_XuatFile = ko.observable();
    self.PhieuDieuChinh_ThemMoi = ko.observable();
    self.PhieuDieuChinh_MoPhieu = ko.observable();
    self.PhieuDieuChinh_Xoa = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    self.ThietLap = ko.observableArray();
    self.ChotSo_ChiNhanh = ko.observableArray();
    self.Quyen_NguoiDung = ko.observableArray();
    self.Enable_NgayLapHD = ko.observable(true);

    self.sum_GiaVonTang = ko.observable();
    self.sum_GiaVonGiam = ko.observable();
    self.ChiTiet_HangHoaDieuChinh = ko.observableArray();
    self.TodayBC = ko.observable("Tuần này");
    self.listDM_LoHang = ko.observableArray();
    self.searchLoHang = ko.observableArray();
    self.TongGiaVonTang = ko.observable();
    self.TongGiaVonGiam = ko.observable();
    self.deleteMaHoaDon = ko.observable();
    self.loiExcel = ko.observableArray();
    self.DanhSachHangDieuChinh = ko.observableArray();
    self.ColumnsExcel = ko.observableArray();

    self.SaveDVT = ko.observableArray();
    self.MaHangHoa_Search = ko.observable();
    self.InforHDprintf = ko.observableArray();
    self.CTHoaDonPrint = ko.observableArray();
    self.CongTy = ko.observableArray();
    self.ListTypeMauIn = ko.observableArray();
    self.IDSelectedDV = ko.observableArray();

    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();// lấy ID chi nhánh
    var _maDC_seach = null;
    var PX_PageSize = 0;
    var _pageNumberHangHoa = 1;
    var _pageSizeHangHoa = 10;
    var AllPageHangHoa;
    var _trangthai1 = "Tạm lưu";
    var _trangthai2 = "Hoàn thành";
    var _trangthai3 = "0";
    self.RowsStart_HH = ko.observable('1');
    self.RowsEnd_HH = ko.observable('10');
    self.RowsStart = ko.observable('0');
    self.RowsEnd = ko.observable('10');
    self.filterNgayPhieuHuy = ko.observable('0');
    self.RowsHangHoas = ko.observableArray();
    self.pageHangHoas = ko.observableArray();
    self.pageSizes = [10, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    var nextPage = 1;
    var numberPage = 1;
    var dt1 = new Date();
    var currentWeekDay = dt1.getDay();
    var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
    var timeStart = moment(new Date(dt1.setDate(dt1.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
    var timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 7))).format('YYYY-MM-DD'); // end of week
    self.timeValue = ko.observable('Tuần này');
    var _sohang = 10;
    var AllPage = 1;
    self.currentPageHangHoa = ko.observable(1);
    self.currentPage = ko.observable(1);
    self.SumRowsHangHoaDieuChinh = ko.observable();
    self.SumRowsHangHoaDieuChinh_Page = ko.observable(0);
    self.selectNameNV = ko.observable();
    self.NhanViens = ko.observableArray();
    self.selectIDNV = ko.observable();
    self.lblThongBao = ko.observable();
    var BH_XuatHuyUri = '/api/DanhMuc/BH_XuatHuyAPI/';
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var DieuChinhUri = '/api/DanhMuc/BH_DieuChinh/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _tenDonViSeach = $('#hd_IDdDonVi').val();
    var _idDonVi = VHeader.IdDonVi;
    var _TenNguoiTao = $('#txtTenTaiKhoan').text();
    var _id_NhanVien = $('.idnhanvien').text();
    var _tennhanvien_seach = null;
    //$('#importDieuChinh').hide();();
    $(".month-oll").hide();
    $('.name-lot').attr("disabled", true);
    self.numberPG = ko.observable(1);
    self.SetupPrint = ko.observableArray();
    var _maHoaDon;
    var _ghichu = "";
    var _ID_HoaDon;
    var _ThongBao = "";
    var STT = 0;
    self.GhiChuHD = ko.observable();
    self.MaDieuChinh_Copy = ko.observable();
    $('.pg_PhieuDC').hide();
    $('.ip_DateReport').attr('disabled', 'false');
    self.filterSeach = function (item, inputString) {

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

                    self.PhieuDieuChinh_XuatFile(CheckQuyenExist('PhieuDieuChinh_XuatFile'));
                    self.PhieuDieuChinh_ThemMoi(CheckQuyenExist('PhieuDieuChinh_ThemMoi'));
                    self.PhieuDieuChinh_MoPhieu(CheckQuyenExist('PhieuDieuChinh_MoPhieu'));
                    self.PhieuDieuChinh_Xoa(CheckQuyenExist('PhieuDieuChinh_Xoa'));
                    self.DieuChinhGiaVon_ThayDoiThoiGian(CheckQuyenExist('DieuChinhGiaVon_ThayDoiThoiGian'));
                }
                else {
                    ShowMessage_Danger('Không có quyền');
                }
            });
        }
    }

    function getQuyen_XemGiaVon() {
        ajaxHelper(ReportUri + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDDoiTuong + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            self.HangHoa_XemGiaVon(data);
        })
    };

    function PageLoad() {
        GetHT_Quyen_ByNguoiDung();
        getQuyen_XemGiaVon();
        GetCauHinhHeThong();
        GetInforCongTy();
        getAllNSNhanVien();
        GetAllNhomHH();
        getDonVi();
        loadMauIn();
    }
    PageLoad();

    function GetCauHinhHeThong() {
        ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + 'GetCauHinhHeThong/' + _idDonVi, 'GET').done(function (data) {
            self.ThietLap(data);
        });
    }

    // Khai báo indexDB chứa danh mục hàng hóa
    var indexedDB = window.indexedDB || window.mozIndexedDB || window.webkitIndexedDB || window.msIndexedDB || window.shimIndexedDB;
    var open = indexedDB.open("BanHang24-DieuChinh");
    var db;
    var table = "HangHoa_DieuChinh"
    // khởi tạo table chứa dữ liệu khi chưa có
    open.onupgradeneeded = function () {
        db = open.result;
        var store = db.createObjectStore(table, { keyPath: "ID_DonViQuiDoi" });
        store.createIndex("ID_DonViQuiDoi", "ID_DonViQuiDoi", { unique: false });
        store.createIndex("MaHangHoa", "MaHangHoa", { unique: false });
        store.createIndex("TenHangHoa", "TenHangHoa", { unique: false });
        //store.createIndex("TenHangHoaFull", "TenHangHoaFull", { unique: false });
        store.createIndex("TenDonViTinh", "TenDonViTinh", { unique: false });
        store.createIndex("ThuocTinh_GiaTri", "ThuocTinh_GiaTri", { unique: false });
        store.createIndex("GiaVonHienTai", "GiaVonHienTai", { unique: false });
        store.createIndex("GiaVonMoi", "GiaVonMoi", { unique: false });
        store.createIndex("GiaVonTang", "GiaVonTang", { unique: false });
        store.createIndex("GiaVonGiam", "GiaVonGiam", { unique: false });
        store.createIndex("DM_LoHang", "DM_LoHang", { unique: false });
        store.createIndex("SoThuTu", "SoThuTu", { unique: false });
    };
    // lấy dữ liệu từ table ra
    var lc_DieuChinh; // khai báo cache chứa danh sách hàng hóa trong phiếu điều chỉnh
    open.onsuccess = function () {
        //load setup Print
        self.SetupPrint(JSON.parse(localStorage.getItem('lc_SetupPrint_DieuChinh')));
        if (self.SetupPrint() != null) {
            self.numberPG(self.SetupPrint().numberPG);
            if (self.SetupPrint().autoPrint == true) {
                $('#setautoPrint').addClass('main-hide');
                $('#SetupOfPrint').addClass('flaggOfPrint');
                $('.setNumberPage input').removeAttr('disabled');
            }
            else {
                $('#setautoPrint').removeClass('main-hide');
                $('#SetupOfPrint').removeClass('flaggOfPrint');
                $('.setNumberPage input').attr('disabled', 'false');
            }
        }
        else {
            var obj = {
                autoPrint: false,
                numberPG: 1
            }
            self.SetupPrint(obj);
            localStorage.setItem('lc_SetupPrint_DieuChinh', JSON.stringify(obj));
        }

        db = open.result;
        var objectStore = db.transaction(table, "readonly").objectStore(table); //chỉ cho phép đọc
        var req = objectStore.getAll();
        var lc_CapNhat = JSON.parse(localStorage.getItem('lc_PhieuDCGV'));
        req.onsuccess = function (evt) {
            if (req.result !== null) {
                lc_DieuChinh = req.result;
                if (lc_DieuChinh.length > 0) {
                    if (lc_CapNhat !== null) {
                        self.selectNameNV(lc_CapNhat.NguoiDieuChinh);
                        self.GhiChuHD(lc_CapNhat.GhiChu);
                        self.MaDieuChinh_Copy(lc_CapNhat.MaHoaDon);
                        _maHoaDon = lc_CapNhat.MaHoaDon;
                        _ghichu = lc_CapNhat.DienGiai;
                        _ID_HoaDon = lc_CapNhat.ID_HoaDon;
                        self.selectIDNV(lc_CapNhat.ID_NhanVien);
                        $('#datetimepicker_mask').datetimepicker({
                            timepicker: true,
                            mask: true, // '9999/19/39 29:59' - digit is the maximum possible for a cell
                            format: 'd/m/Y H:i',
                            value: moment(lc_CapNhat.NgayLapHoaDon).format('DD/MM/YYYY HH:mm')
                        });
                        loadData();
                    }
                    else {
                        $('#modalpopuploadDaTaKK').modal('show');
                    }
                }
                else {
                    //$('#importDieuChinh').show();;
                }
            }
        };
    };
    function sortByKey(array, key) {
        return array.sort(function (a, b) {
            return a[key] < b[key];
        });
    }
    function LoadingForm(IsShow) {
        $('.detail-warehouse').gridLoader({ show: IsShow });
    }
    // tăng giảm giá trị
    self.downUpvaluesPrintPG = function () {
        var objsoluong = formatNumberObj($("#txtPagePrint"))
        var soluong = formatNumberToFloat(objsoluong); //
        var keyCode = event.keyCode || event.which;
        // press up
        if (keyCode === 38 || soluong == 0) {
            soluong = soluong + 1;
            $("#txtPagePrint").val(formatNumber3Digit(soluong));
        }
        // press down
        if (keyCode === 40) {
            if (soluong > 1) {
                soluong = soluong - 1;
                $("#txtPagePrint").val(formatNumber3Digit(soluong));
            }
        }
        var obj = {
            autoPrint: self.SetupPrint().autoPrint,
            numberPG: soluong
        };
        self.SetupPrint(obj);
        localStorage.setItem('lc_SetupPrint_DieuChinh', JSON.stringify(obj));
    }
    // load cache
    self.loadData = function () {
        loadData();
    }
    function loadData() {
        var store = db.transaction(table, "readwrite").objectStore(table);
        var reqHH = store.getAll();
        reqHH.onsuccess = function () {
            lc_DieuChinh = reqHH.result;
            self.HangHoaDieuChinh(lc_DieuChinh);
            $('#modalpopuploadDaTaKK').modal('hide');
            //$('#importDieuChinh').hide();();
            if (self.HangHoaDieuChinh().length > 0) {
                PX_PageSize = self.HangHoaDieuChinh().length;
                self.SumRowsHangHoaDieuChinh(PX_PageSize);
                if (PX_PageSize > 10)
                    $('.pg_PhieuDC').show();
                else
                    $('.pg_PhieuDC').hide();
                AllPageHangHoa = PX_PageSize / _pageSizeHangHoa;
                if (AllPageHangHoa > parseInt(AllPageHangHoa)) {
                    AllPageHangHoa = parseInt(AllPageHangHoa) + 1;
                }
                //self.selecPageHangHoa();
                //self.SelectedPageNumberDieuChinh();
            }
            self.resetCache();
        }
    }
    // load nhóm hàng hóa
    var tk = null;
    function GetAllNhomHH() {
        ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].ID_Parent == null) {
                    var objParent = {
                        ID: data[i].ID,
                        TenNhomHangHoa: data[i].TenNhomHang,
                        Childs: [],
                    }
                    for (var j = 0; j < data.length; j++) {
                        if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                            var objChild =
                            {
                                ID: data[j].ID,
                                TenNhomHangHoa: data[j].TenNhomHang,
                                ID_Parent: data[i].ID,
                                Child2s: []
                            };
                            for (var k = 0; k < data.length; k++) {
                                if (data[k].ID_Parent !== null && data[k].ID_Parent === data[j].ID) {
                                    var objChild2 =
                                    {
                                        ID: data[k].ID,
                                        TenNhomHangHoa: data[k].TenNhomHang,
                                        ID_Parent: data[j].ID,
                                    };
                                    objChild.Child2s.push(objChild2);
                                }
                            }
                            objParent.Childs.push(objChild);
                        }
                    }
                    self.NhomHangHoas.push(objParent);
                }
            }
            if (self.NhomHangHoas().length > 10) {
                $('.close-goods').css('display', 'block');
            }
        });
    };

    var time = null
    self.NoteNhomHang = function () {
        clearTimeout(time);
        time = setTimeout(
            function () {
                self.NhomHangHoas([]);
                tk = $('#SeachNhomHang').val();
                if (tk.trim() != '') {
                    ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
                        console.log(data);
                        for (var i = 0; i < data.length; i++) {
                            if (data[i].ID_Parent == null) {
                                var objParent = {
                                    ID: data[i].ID,
                                    TenNhomHangHoa: data[i].TenNhomHang,
                                    Childs: [],
                                }
                                for (var j = 0; j < data.length; j++) {
                                    if (data[j].ID !== data[i].ID && data[j].ID_Parent === data[i].ID) {
                                        var objChild =
                                        {
                                            ID: data[j].ID,
                                            TenNhomHangHoa: data[j].TenNhomHang,
                                            ID_Parent: data[i].ID,
                                            Child2s: []
                                        };
                                        for (var k = 0; k < data.length; k++) {
                                            if (data[k].ID_Parent !== null && data[k].ID_Parent === data[j].ID) {
                                                var objChild2 =
                                                {
                                                    ID: data[k].ID,
                                                    TenNhomHangHoa: data[k].TenNhomHang,
                                                    ID_Parent: data[j].ID,
                                                };
                                                objChild.Child2s.push(objChild2);
                                            }
                                        }
                                        objParent.Childs.push(objChild);
                                    }
                                }
                                self.NhomHangHoas.push(objParent);
                            }
                        }
                        if (self.NhomHangHoas().length > 10) {
                            $('.close-goods').css('display', 'block');
                        }
                    })
                }
                else {
                    GetAllNhomHH();
                }
            }, 700);
    };
    // lựa chọn nhóm hàng
    $("#check_AllNhomHang input").on("click", function (item) {
        $("#check_AllNhomHang input").removeClass('squarevt');
        console.log($("#check_AllNhomHang input").prop('checked'));

    });

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
            ajaxHelper(DieuChinhUri + 'getListHangHoaBy_IDNhomHang', 'POST', param).done(function (x) {
                if (x.res) {
                    for (let i = 0; i < x.dataSoure.length; i++) {
                        let itFor = x.dataSoure[i];
                        itFor.TenLoHang = itFor.MaLoHang;
                        self.addNhomHangHoa(itFor);
                    }
                   
                    GetTonKho_byIDQuyDois(true);
                    self.resetCache();
                }
            })
        }
    })

    var _note_ID_NhomHangHoa_Select;
    self.SelectID_NhomHangHoa = function (item) {
        _note_ID_NhomHangHoa_Select = item.ID;
        console.log(_note_ID_NhomHangHoa_Select);
        var $this = $('#input_NhomHang_' + item.ID + ' input')

        if ($this.prop('checked'))
            $this.removeClass('checkbox_TR');
        else
            $this.addClass('checkbox_TR');
    }
    //Tìm kiếm hàng hóa
    self.SelectedHHEnterkey = function () {
        getChiTietHangHoaLoHang_ByMaHH($('#txtAutoHangHoa').val().toUpperCase());
    }

    function newPhieuDC() {
        return {
            ID_DonVi: VHeader.IdDonVi,
            ID_NhanVien: VHeader.IdNhanVien,
            NguoiDieuChinh: VHeader.TenNhanVien,
            MaHoaDon: '',
            GhiChu: '',
            DienGiai: '',
        }
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
        let chotSo = VHeader.CheckKhoaSo(moment(ngayLapHD, 'YYYY-MM-DD HH:mm').format('YYYY-MM-DD'),_idDonVi);
        if (chotSo) {
            ShowMessage_Danger(VHeader.warning.ChotSo.Update);
            return false;
        }
        return true;
    }

    $('#datetimepicker_mask').on('change.dp', function (e) {
        let dtChose = $(this).val();
        let dateCheck = CheckNgayLapHD_format(dtChose);
        if (!dateCheck) {
            return;
        }
        let lc_CapNhat = localStorage.getItem('lc_PhieuDCGV');
        if (lc_CapNhat === null) {
            lc_CapNhat = newPhieuDC();
        }
        else {
            lc_CapNhat = JSON.parse(lc_CapNhat);
        }
        lc_CapNhat.NgayLapHoaDon = dtChose;
        localStorage.setItem('lc_PhieuDCGV', JSON.stringify(lc_CapNhat));

        GetTonKho_byIDQuyDois();
    });

    function GetTonKho_byIDQuyDois(isImport = false) {
        var arrIDQuiDoi = [], arrIDLoHang = [];

        let trans1 = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        let store1 = trans1.objectStore(table);
        let req1 = store1.getAll();
        req1.onsuccess = function (e) {

            let result = req1.result;
            arrIDQuiDoi = result.map(function (x) {
                return x.ID_DonViQuiDoi;
            });

            let lstLo = result.filter(x => x.QuanLyTheoLoHang);
            for (let i = 0; i < lstLo.length; i++) {
                for (let j = 0; j < lstLo[i].DM_LoHang.length; j++) {
                    arrIDLoHang.push(lstLo[i].DM_LoHang[j].ID_LoHang);
                }
            }

            arrIDLoHang = $.grep(arrIDLoHang, function (x) {
                return x !== null;
            })

            let ngayKK = moment($('#datetimepicker_mask').val(), 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
            let obj = {
                ID_ChiNhanh: _idDonVi,
                ToDate: ngayKK,
                ListIDQuyDoi: arrIDQuiDoi,
                ListIDLoHang: arrIDLoHang
            }

            if (arrIDQuiDoi.length > 0) {
                ajaxHelper(DMHangHoaUri + 'GetTonKho_byIDQuyDois', 'POST', obj).done(function (x) {
                    if (x.res) {
                        let trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
                        let store = trans.objectStore(table);
                        let req = store.getAll();
                        req.onsuccess = function (e) {
                            lc_DieuChinh = req.result;
                            if (lc_DieuChinh == null) { // trả về danh sách trống khi cache rỗng
                                lc_DieuChinh = [];
                            }

                            for (let i = 0; i < x.data.length; i++) {
                                let forOut = x.data[i];
                                for (let j = 0; j < lc_DieuChinh.length; j++) {
                                    let forIn = lc_DieuChinh[j];
                                    if (forOut.ID_DonViQuiDoi === forIn.ID_DonViQuiDoi) {
                                        if (isImport) {
                                            lc_DieuChinh[j].GiaVonMoi = forOut.GiaVon;
                                        }
                                        let chenhlech = parseFloat(forIn.GiaVonMoi) - parseFloat(forOut.GiaVon);
                                        if (forIn.QuanLyTheoLoHang) {
                                            for (let k = 0; k < forIn.DM_LoHang.length; k++) {
                                                let itLo = forIn.DM_LoHang[k];
                                                if (forOut.ID_LoHang === itLo.ID_LoHang) {
                                                    if (isImport) {
                                                        lc_DieuChinh[j].DM_LoHang[k].GiaVonMoi = forOut.GiaVon;
                                                    }
                                                    lc_DieuChinh[j].DM_LoHang[k].GiaVonHienTai = forOut.GiaVon;
                                                    if (chenhlech > 0) {
                                                        lc_DieuChinh[j].DM_LoHang[k].GiaVonTang = chenhlech;
                                                        lc_DieuChinh[j].DM_LoHang[k].GiaVonGiam = 0;
                                                    }
                                                    else {
                                                        lc_DieuChinh[j].DM_LoHang[k].GiaVonGiam = chenhlech;
                                                        lc_DieuChinh[j].DM_LoHang[k].GiaVonTang = 0;
                                                    }

                                                    if (k === 0) {// update parent
                                                        if (isImport) {
                                                            lc_DieuChinh[j].GiaVonMoi = forOut.GiaVon;
                                                        }
                                                        lc_DieuChinh[j].GiaVonHienTai = forOut.GiaVon;
                                                        lc_DieuChinh[j].GiaVonMoi = lc_DieuChinh[j].DM_LoHang[k].GiaVonMoi;
                                                        lc_DieuChinh[j].GiaVonTang = lc_DieuChinh[j].DM_LoHang[k].GiaVonTang;
                                                        lc_DieuChinh[j].GiaVonGiam = lc_DieuChinh[j].DM_LoHang[k].GiaVonGiam;
                                                    }
                                                    store.put(lc_DieuChinh[j]);
                                                }
                                            }
                                        }
                                        else {
                                            lc_DieuChinh[j].GiaVonHienTai = forOut.GiaVon;
                                            if (chenhlech > 0) {
                                                lc_DieuChinh[j].GiaVonTang = chenhlech;
                                                lc_DieuChinh[j].GiaVonGiam = 0;
                                            }
                                            else {
                                                lc_DieuChinh[j].GiaVonGiam = chenhlech;
                                                lc_DieuChinh[j].GiaVonTang = 0;
                                            }
                                            store.put(lc_DieuChinh[j]);
                                            break;
                                        }
                                    }
                                }
                            }

                            self.resetCache();
                            self.selectedHH(undefined);
                            $('#txtAutoHangHoa').val(self.MaHangHoa_Search());
                            $('#txtAuto').focus();
                            $('#txtAutoHangHoa').focus().select();
                        }
                    }
                    else {
                        ShowMessage_Danger(x.mes);
                    }
                })
            }
        }
    }

    function getChiTietHangHoaLoHang_ByMaHH(MaHH) {

        if (dk_check == 1) {
            ajaxHelper(BH_XuatHuyUri + "getListHangHoaLoHangBy_MaHangHoa?maHH=" + MaHH + '&ID_ChiNhanh=' + _id_DonVi, 'GET').done(function (data) {
                if (data.length > 0) {
                    STT = STT + 1;
                    $('#importXuatHuy').hide();
                    var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
                    var store = trans.objectStore(table);
                    var req = store.getAll();
                    req.onsuccess = function (e) {
                        self.MaHangHoa_Search(data[0].MaHangHoa);
                        var found = -1;
                        lc_DieuChinh = req.result;
                        var ob2 = [];
                        if (data[0].QuanLyTheoLoHang == 1) {
                            ob2 = [{
                                ID_DonViQuiDoi: data[0].ID_DonViQuiDoi,
                                ID_LoHang: data[0].ID_LoHang,
                                MaHangHoa: data[0].MaHangHoa,
                                TenLoHang: data[0].TenLoHang,
                                NgaySanXuat: data[0].NgaySanXuat,
                                NgayHetHan: data[0].NgayHetHan,
                                GiaVonHienTai: data[0].GiaVon,
                                GiaVonMoi: data[0].GiaVon,
                                GiaVonTang: 0,
                                GiaVonGiam: 0,
                            }]
                        }
                        var ob1 = {
                            ID_DonViQuiDoi: data[0].ID_DonViQuiDoi,
                            MaHangHoa: data[0].MaHangHoa,
                            TenHangHoa: data[0].TenHangHoa,
                            TenDonViTinh: data[0].TenDonViTinh,
                            ThuocTinh_GiaTri: data[0].ThuocTinh_GiaTri,
                            GiaVonHienTai: data[0].GiaVon,
                            GiaVonMoi: data[0].GiaVon,
                            GiaVonTang: 0,
                            GiaVonGiam: 0,
                            QuanLyTheoLoHang: data[0].QuanLyTheoLoHang,
                            DM_LoHang: ob2,
                            SoThuTu: STT
                        }
                        if (lc_DieuChinh == null) { // trả về danh sách trống khi cache rỗng
                            lc_DieuChinh = [];
                        }
                        else {
                            for (var i = 0; i < lc_DieuChinh.length; i++) {
                                if (lc_DieuChinh[i].MaHangHoa == data[0].MaHangHoa) {
                                    found = i;
                                    break;
                                }
                            }
                        }
                        if (found < 0) {
                            lc_DieuChinh.unshift(ob1); // add hàng hóa được chọn vào
                            store.add(ob1); // lưu indexedDB
                            PX_PageSize = PX_PageSize + 1;
                            self.SumRowsHangHoaDieuChinh(PX_PageSize);
                            if (PX_PageSize > 10)
                                $('.pg_PhieuDC').show();
                            else
                                $('.pg_PhieuDC').hide();
                            AllPageHangHoa = PX_PageSize / _pageSizeHangHoa;
                            if (AllPageHangHoa > parseInt(AllPageHangHoa)) {
                                AllPageHangHoa = parseInt(AllPageHangHoa) + 1;
                            }
                            // self.selecPageHangHoa();
                        }
                        else {
                            for (var i = 0; i < lc_DieuChinh.length; i++) {
                                if (lc_DieuChinh[i].MaHangHoa == data[0].MaHangHoa) {
                                    if (data[0].QuanLyTheoLoHang != 1) {
                                        lc_DieuChinh[i].GiaVonHienTai = data[0].GiaVon;
                                        lc_DieuChinh[i].GiaVonMoi = lc_DieuChinh[i].GiaVonMoi;
                                        var chenhlech = parseFloat(lc_DieuChinh[i].GiaVonMoi) - parseFloat(data[0].GiaVon);
                                        if (chenhlech >= 0) {
                                            lc_DieuChinh[i].GiaVonTang = chenhlech;
                                            lc_DieuChinh[i].GiaVonGiam = 0;
                                        }
                                        else {
                                            lc_DieuChinh[i].GiaVonTang = 0;
                                            lc_DieuChinh[i].GiaVonGiam = chenhlech;
                                        }
                                    }
                                    else {
                                        for (var j = 0; j < lc_DieuChinh[i].DM_LoHang.length; j++) {
                                            if (lc_DieuChinh[i].DM_LoHang[j].ID_LoHang == data[0].ID_LoHang) {
                                                if (j == 0) {
                                                    lc_DieuChinh[i].GiaVonHienTai = data[0].GiaVon;
                                                    lc_DieuChinh[i].GiaVonMoi = lc_DieuChinh[i].GiaVonMoi;
                                                    var chenhlech = parseFloat(lc_DieuChinh[i].GiaVonMoi) - parseFloat(data[0].GiaVon);
                                                    if (chenhlech >= 0) {
                                                        lc_DieuChinh[i].GiaVonTang = chenhlech;
                                                        lc_DieuChinh[i].GiaVonGiam = 0;
                                                    }
                                                    else {
                                                        lc_DieuChinh[i].GiaVonTang = 0;
                                                        lc_DieuChinh[i].GiaVonGiam = chenhlech;
                                                    }
                                                }
                                                lc_DieuChinh[i].DM_LoHang[j].GiaVonHienTai = data[0].GiaVon;
                                                lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi = lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi;
                                                var chenhlech = parseFloat(lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi) - parseFloat(data[0].GiaVon);
                                                if (chenhlech >= 0) {
                                                    lc_DieuChinh[i].DM_LoHang[j].GiaVonTang = chenhlech;
                                                    lc_DieuChinh[i].DM_LoHang[j].GiaVonGiam = 0;
                                                }
                                                else {
                                                    lc_DieuChinh[i].DM_LoHang[j].GiaVonTang = 0;
                                                    lc_DieuChinh[i].DM_LoHang[j].GiaVonGiam = chenhlech;
                                                }
                                                break;
                                            }
                                            if (j == lc_DieuChinh[i].DM_LoHang.length - 1) {
                                                lc_DieuChinh[i].DM_LoHang.push(ob2)
                                            }
                                        }
                                    }
                                    lc_DieuChinh[i].SoThuTu = STT;
                                    store.put(lc_DieuChinh[i]);
                                    break;
                                }
                            }
                        }
                        self.resetCache();
                        self.selectedHH(undefined);
                        $('#txtAutoHangHoa').val(self.MaHangHoa_Search());
                        $('#txtAuto').focus();
                        $('#txtAutoHangHoa').focus().select();
                    }

                    GetTonKho_byIDQuyDois();
                }
            });
        }
        else {
            ajaxHelper(BH_XuatHuyUri + "getListHangHoaLoHangBy_MaHangHoa?maHH=" + MaHH + '&ID_ChiNhanh=' + _id_DonVi, 'GET').done(function (data) {
                item_NhapCham = data[0];
            });
            ajaxHelper(BH_XuatHuyUri + "GetList_TenDonViTinh?maHH=" + MaHH, 'GET').done(function (data) {
                if (data.length > 1) {
                    ///console.log(data);
                    self.TenDonViTinh(data);
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].MaHangHoa == MaHH) {
                            self.selectedTenDonViTinh(data[i].MaHangHoa);
                            break;
                        }
                    }
                    $('#txtSelectHT').removeAttr('disabled');
                    $('#txtSoLuongXH').val(1);
                    $('#txtSoLuongXH').focus().select();
                }
                else {
                    self.TenDonViTinh([{ MaHangHoa: '', TenDonViTinh: "Đơn Vị" }]);
                    $('#txtSelectHT').attr('disabled', 'false');
                    $('#txtSoLuongXH').val(1);
                    $('#txtSoLuongXH').focus().select();
                }
            });
        }
    }
    var dk_check = 1;
    self.ShowFart = function () {
        $(".number-fast").toggle();
        if (dk_check == 1) {
            dk_check = 2;
            $('#txtAutoHangHoa').focus().select();
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Chế độ nhập thường", "success");
        }
        else {
            dk_check = 1;
            $('#txtAutoHangHoa').focus().select();
            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Chế độ nhập nhanh", "success");
        }
    }
    self.xoacache = function () {
        lc_DieuChinh = [];
        self.HangHoaDieuChinh([]);
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        store.clear();
        $('#modalpopuploadDaTaKK').modal('hide');
        ////$('#importDieuChinh').show();;
        PX_PageSize = 0;
        _pageNumberHangHoa = 1;
        _pageSizeHangHoa = 10;
        $('.pg_PhieuDC').hide();
        self.SumRowsHangHoaDieuChinh(0);
    };

    // thay đổi giá trị
    var _TongGiaVonHienTai = 0;
    var _TongGiaVonMoi = 0;
    var _TongGiaVonTang = 0;
    var _TongGiaVonGiam = 0;
    self.resetCache = function () {
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.getAll();
        req.onsuccess = function (evt) {
            if (req.result !== null) {
                lc_DieuChinh = req.result;
                console.log(lc_DieuChinh);
                self.SumRowsHangHoaDieuChinh_Page(lc_DieuChinh.length);
                STT = lc_DieuChinh.length;
                self.HangHoaDieuChinh(lc_DieuChinh);
                var gvm = 0;
                var gvht = 0;
                var cl = 0;
                for (var i = 0; i < lc_DieuChinh.length; i++) {
                    if (lc_DieuChinh[i].QuanLyTheoLoHang == 0) {
                        gvm = gvm + parseFloat(lc_DieuChinh[i].GiaVonMoi);
                        gvht = gvht + parseFloat(lc_DieuChinh[i].GiaVonHienTai);
                    }
                    else {
                        for (var j = 0; j < lc_DieuChinh[i].DM_LoHang.length; j++) {
                            gvm = gvm + parseFloat(lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi);
                            gvht = gvht + parseFloat(lc_DieuChinh[i].DM_LoHang[j].GiaVonHienTai);
                        }
                    }
                }
                _TongGiaVonHienTai = gvht;
                _TongGiaVonMoi = gvm;
                cl = gvm - gvht;
                if (cl >= 0) {
                    _TongGiaVonTang = cl;
                    _TongGiaVonGiam = 0;
                }
                else {
                    _TongGiaVonTang = 0;
                    _TongGiaVonGiam = cl;
                }
                //$('#lblTongGVM').html(formatNumber3Digit(gvm));
                //$('#lblTongGVHT').html(formatNumber3Digit(gvht));
                //$('#lblTongGVCL').html(formatNumber3Digit(cl));
                self.TongGV_HienTai(formatNumber3Digit(gvht));
                self.TongGV_Moi(formatNumber3Digit(gvm));
                self.TongGV_ChenhLech(formatNumber3Digit(cl));
                self.selecPageHangHoa();
                self.SelectedPageNumberDieuChinh();
            }
        };
    }
    self.resetCacheEdit = function () {
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.getAll();
        req.onsuccess = function (evt) {
            if (req.result !== null) {
                lc_DieuChinh = req.result;
                self.HangHoaDieuChinh(lc_DieuChinh);
                var gvm = 0;
                var gvht = 0;
                var cl = 0;
                for (var i = 0; i < lc_DieuChinh.length; i++) {
                    if (lc_DieuChinh[i].QuanLyTheoLoHang == 0) {
                        gvm = gvm + parseFloat(lc_DieuChinh[i].GiaVonMoi);
                        gvht = gvht + parseFloat(lc_DieuChinh[i].GiaVonHienTai);
                    }
                    else {
                        for (var j = 0; j < lc_DieuChinh[i].DM_LoHang.length; j++) {
                            gvm = gvm + parseFloat(lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi);
                            gvht = gvht + parseFloat(lc_DieuChinh[i].DM_LoHang[j].GiaVonHienTai);
                        }
                    }
                }
                _TongGiaVonHienTai = gvht;
                _TongGiaVonMoi = gvm;
                cl = gvm - gvht;
                if (cl >= 0) {
                    _TongGiaVonTang = cl;
                    _TongGiaVonGiam = 0;
                }
                else {
                    _TongGiaVonTang = 0;
                    _TongGiaVonGiam = cl;
                }
                //$('#lblTongGVM').html(formatNumber3Digit(gvm));
                //$('#lblTongGVHT').html(formatNumber3Digit(gvht));
                //$('#lblTongGVCL').html(formatNumber3Digit(cl));
                self.TongGV_HienTai(formatNumber3Digit(gvht));
                self.TongGV_Moi(formatNumber3Digit(gvm));
                self.TongGV_ChenhLech(formatNumber3Digit(cl));
            }
        };
    }
    self.deleteChiTietHD = function (item) {
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        store.delete(item.ID_DonViQuiDoi);
        self.HangHoaDieuChinh.remove(item);
        lc_DieuChinh = self.HangHoaDieuChinh();
        PX_PageSize = PX_PageSize - 1;
        self.SumRowsHangHoaDieuChinh(PX_PageSize);
        AllPageHangHoa = PX_PageSize / _pageSizeHangHoa;
        if (AllPageHangHoa > parseInt(AllPageHangHoa)) {
            AllPageHangHoa = parseInt(AllPageHangHoa) + 1;
        }
        self.resetCache();
        //if (self.HangHoaDieuChinh().length < 1)
            ////$('#importDieuChinh').show();;
        //self.selecPageHangHoa();
    }
    self.putDM_NhomHang = function (item) {
        var guid = (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
        _ID_DonViQuiDoi = item.ID_DonViQuiDoi;
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        var req1 = store.openCursor(item.ID_DonViQuiDoi);
        req1.onsuccess = function (evt) {
            if (req1.result !== null) {
                var lc_Edit = req1.result.value;
                var ob2 = {
                    ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                    ID_LoHang: guid,
                    MaHangHoa: item.MaHangHoa,
                    TenLoHang: "",
                    NgaySanXuat: null,
                    NgayHetHan: null,
                    GiaVonHienTai: 0,
                    GiaVonMoi: 0,
                    GiaVonTang: 0,
                    GiaVonGiam: 0,
                }
                lc_Edit.DM_LoHang.push(ob2)
                store.put(lc_Edit)
                self.resetCache();
            }
        };
    }
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    // phân trang chi tiết HangHoa
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.GetClassHangHoa = function (page) {
        return (page.SoTrang === self.currentPageHangHoa()) ? "click" : "";
    };
    self.selecPageHangHoa = function () {
        self.SumNumberPageReportHangHoa([]);
        if (AllPageHangHoa > 4) {
            self.SumNumberPageReportHangHoa.push({ SoTrang: 1 });
            self.SumNumberPageReportHangHoa.push({ SoTrang: 2 });
            self.SumNumberPageReportHangHoa.push({ SoTrang: 3 });
            self.SumNumberPageReportHangHoa.push({ SoTrang: 4 });
            self.SumNumberPageReportHangHoa.push({ SoTrang: 5 });
        }
        else {
            for (var j = 0; j < AllPageHangHoa; j++) {
                self.SumNumberPageReportHangHoa.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageHangHoa').hide();
        $('#BackPageHangHoa').hide();
        $('#NextPageHangHoa').show();
        $('#EndPageHangHoa').show();
    }
    self.ReserPageHangHoa = function (item) {
        console.log(_pageNumberHangHoa, parseInt(AllPageHangHoa))
        if (_pageNumberHangHoa > 1 && AllPageHangHoa > 5/* && nextPage < AllPageHangHoa - 1*/) {
            if (_pageNumberHangHoa > 3 && _pageNumberHangHoa < parseInt(AllPageHangHoa) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberHangHoa) === parseInt(AllPageHangHoa) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 3 });
                }
            }
            else if (_pageNumberHangHoa == parseInt(AllPageHangHoa)) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 4 });
                }
            }
            else if (_pageNumberHangHoa < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberHangHoa)
                    self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberHangHoa == 1 && parseInt(AllPageHangHoa) >= 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportHangHoa.replace(self.SumNumberPageReportHangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i });
            }
        }
        if (_pageNumberHangHoa > 1) {
            $('#StartPageHangHoa').show();
            $('#BackPageHangHoa').show();
        }
        else {
            $('#StartPageHangHoa').hide();
            $('#BackPageHangHoa').hide();
        }
        if (_pageNumberHangHoa == AllPageHangHoa) {
            $('#NextPageHangHoa').hide();
            $('#EndPageHangHoa').hide();
        }
        else {
            $('#NextPageHangHoa').show();
            $('#EndPageHangHoa').show();
        }
        self.currentPageHangHoa(parseInt(_pageNumberHangHoa));
    }
    self.SelectedPageNumberDieuChinh = function () {
        var store = db.transaction(table, "readwrite").objectStore(table);
        var req = store.getAll();
        req.onsuccess = function (evt) {
            if (req.result !== null) {
                lc_DieuChinh = req.result;
                lc_DieuChinh = sortByKey(lc_DieuChinh, 'SoThuTu');
                lc_DieuChinh = lc_DieuChinh.sort(function (a, b) {
                    var x = a.SoThuTu, y = b.SoThuTu;
                    return x < y ? 1 : x > y ? -1 : 0;
                });

                self.HangHoaDieuChinh(lc_DieuChinh);
                var i = (_pageNumberHangHoa - 1) * _pageSizeHangHoa;
                var j = _pageNumberHangHoa * _pageSizeHangHoa;
                self.RowsStart_HH(i + 1);
                if (_pageNumberHangHoa < AllPageHangHoa)
                    self.RowsEnd_HH(j);
                else
                    self.RowsEnd_HH(PX_PageSize);
                self.DS_DieuChinh(self.HangHoaDieuChinh().slice(i, j));
            }
        };

    }
    self.NextPageHangHoa = function (item) {
        if (_pageNumberHangHoa < AllPageHangHoa) {
            _pageNumberHangHoa = _pageNumberHangHoa + 1;
            self.ReserPageHangHoa();
            self.SumRowsHangHoaDieuChinh(PX_PageSize - (10 * (_pageNumberHangHoa - 1)));
            self.SelectedPageNumberDieuChinh();
        }
    };
    self.BackPageHangHoa = function (item) {
        if (_pageNumberHangHoa > 1) {
            _pageNumberHangHoa = _pageNumberHangHoa - 1;
            self.ReserPageHangHoa();
            self.SumRowsHangHoaDieuChinh(PX_PageSize - (10 * (_pageNumberHangHoa - 1)));
            self.SelectedPageNumberDieuChinh();
        }
    };
    self.EndPageHangHoa = function (item) {
        _pageNumberHangHoa = AllPageHangHoa;
        self.ReserPageHangHoa();
        self.SumRowsHangHoaDieuChinh(PX_PageSize - (10 * (_pageNumberHangHoa - 1)));
        self.SelectedPageNumberDieuChinh();
    };
    self.StartPageHangHoa = function (item) {
        _pageNumberHangHoa = 1;
        self.ReserPageHangHoa();
        self.SumRowsHangHoaDieuChinh(PX_PageSize - (10 * (_pageNumberHangHoa - 1)));
        self.SelectedPageNumberDieuChinh();
    };
    self.gotoNextPageHangHoa = function (item) {
        _pageNumberHangHoa = item.SoTrang;
        self.ReserPageHangHoa();
        self.SumRowsHangHoaDieuChinh(PX_PageSize - (10 * (_pageNumberHangHoa - 1)));
        self.SelectedPageNumberDieuChinh();
    }
    self.selectGV = function (item) {
        var thisGV = "#GiaVon_" + item.ID_DonViQuiDoi;
        $(thisGV).focus().select();
    }
    self.DeleteHH = function (item) {
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        store.delete(item.ID_DonViQuiDoi);
        self.HangHoaDieuChinh.remove(item);
        lc_DieuChinh = self.HangHoaDieuChinh();
        PX_PageSize = PX_PageSize - 1;
        self.SumRowsHangHoaDieuChinh(PX_PageSize);
        AllPageHangHoa = PX_PageSize / _pageSizeHangHoa;
        if (AllPageHangHoa > parseInt(AllPageHangHoa)) {
            AllPageHangHoa = parseInt(AllPageHangHoa) + 1;
        }
        self.SelectedPageNumberDieuChinh();
        console.log(self.HangHoaDieuChinh().length);
        //if (self.HangHoaDieuChinh().length < 1)
            //$('#importDieuChinh').show();;
    }
    var thisGVM;
    self.editGiaVon = function (item) {
        var rowid = item.ID_DonViQuiDoi;
        var objGiaVon = formatNumberObj($("#GiaVon_" + rowid))
        var GiaVonMoi = formatNumberToFloat(objGiaVon);
        thisGVM = GiaVonMoi;
        var GiaVonHienTai = item.GiaVonHienTai // lấy giá vốn hiện tại
        var ChenhLech = GiaVonMoi - GiaVonHienTai;
        $("#GiaVonChenhLech_" + rowid).text(formatNumber3Digit(ChenhLech));
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        var req1 = store.openCursor(rowid);
        req1.onsuccess = function (evt) {
            if (req1.result !== null) {
                var lc_Edit = req1.result.value;
                lc_Edit.GiaVonMoi = GiaVonMoi;
                if (ChenhLech >= 0) {
                    lc_Edit.GiaVonTang = ChenhLech;
                    lc_Edit.GiaVonGiam = 0;
                }
                else {
                    lc_Edit.GiaVonTang = 0;
                    lc_Edit.GiaVonGiam = ChenhLech;
                }
                if (item.QuanLyTheoLoHang == 1) {
                    lc_Edit.DM_LoHang[0].GiaVonMoi = GiaVonMoi;
                    if (ChenhLech >= 0) {
                        lc_Edit.DM_LoHang[0].GiaVonTang = ChenhLech;
                        lc_Edit.DM_LoHang[0].GiaVonGiam = 0;
                    }
                    else {
                        lc_Edit.DM_LoHang[0].GiaVonTang = 0;
                        lc_Edit.DM_LoHang[0].GiaVonGiam = ChenhLech;
                    }
                }
                //ss.update(ss.value);
                req1.result.update(req1.result.value);
            }
            self.resetCacheEdit();
        };

    }
    self.editGiaVonIndex = function (item) {
        var rowid = item.ID_LoHang;
        var objGiaVon = formatNumberObj($("#GiaVon_" + rowid))
        var GiaVonMoi = formatNumberToFloat(objGiaVon);
        thisGVM = GiaVonMoi;
        var GiaVonHienTai = item.GiaVonHienTai // lấy giá vốn hiện tại
        var ChenhLech = GiaVonMoi - GiaVonHienTai;
        $("#GiaVonChenhLech_" + rowid).text(formatNumber3Digit(ChenhLech));
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        var req1 = store.openCursor(item.ID_DonViQuiDoi);
        req1.onsuccess = function (evt) {
            if (req1.result !== null) {
                var lc_Edit = req1.result.value;
                for (var i = 0; i < lc_Edit.DM_LoHang.length; i++) {
                    if (lc_Edit.DM_LoHang[i].ID_LoHang == rowid) {
                        lc_Edit.DM_LoHang[i].GiaVonMoi = GiaVonMoi;
                        if (ChenhLech >= 0) {
                            lc_Edit.DM_LoHang[i].GiaVonTang = ChenhLech;
                            lc_Edit.DM_LoHang[i].GiaVonGiam = 0;
                        }
                        else {
                            lc_Edit.DM_LoHang[i].GiaVonTang = 0;
                            lc_Edit.DM_LoHang[i].GiaVonGiam = ChenhLech;
                        }
                        req1.result.update(req1.result.value);
                        break;
                    }
                }
            }
            self.resetCacheEdit();
        };

    }
    self.LoadThongBao = function (item) {
        var TenThuocTinh = item.TenDonViTinh != "" ? " (" + item.TenDonViTinh + ")" : "";
        if (_ThongBao == "") {
            _ThongBao = "<span>" + item.TenHangHoa + "</span>" +
                "<span style='color: #8abb0f'><span>" + TenThuocTinh + "</span></span>" +
                "<span style='color:#ff6a00'>" + item.ThuocTinh_GiaTri + "</span>" +
                "<span>: " + formatNumber3Digit(thisGVM) + "</span>";
        }
        else
            _ThongBao = "<span>" + item.TenHangHoa + "</span>" +
                "<span style='color: #8abb0f'><span>" + TenThuocTinh + "</span></span>" +
                "<span style='color:#ff6a00'>" + item.ThuocTinh_GiaTri + "</span>" +
                "<span>: " + formatNumber3Digit(thisGVM) + "</span>" + "<br/>" + _ThongBao;
        self.lblThongBao(_ThongBao);
    }

    var timer = null;
    self.NoteNhanVien = function () {
        clearTimeout(timer);
        _tennhanvien_seach = $('#txtAuto').val();
        console.log(_tennhanvien_seach);
        timer = setTimeout(getAllNSNhanVien(), 500);
    }
    // lấy tên nhân viên tạo phiếu
    //lấy về danh sách nhân viên
    function getAllNSNhanVien() {
        ajaxHelper(NhanVienUri + "getListNhanVien_DonVi?ID_ChiNhanh=" + _tenDonViSeach + "&nameNV=" + _tennhanvien_seach, 'GET').done(function (data) {
            self.NhanViens(data);
        });
    }
    self.SelectNhanVien = function (item) {
        self.selectNameNV(item.TenNhanVien);
        self.selectIDNV(item.ID);

        let lc_CapNhat = localStorage.getItem('lc_PhieuDCGV');
        if (lc_CapNhat === null) {
            lc_CapNhat = newPhieuDC();
        }
        else {
            lc_CapNhat = JSON.parse(lc_CapNhat);
        }
        lc_CapNhat.ID_NhanVien = item.ID;
        lc_CapNhat.NguoiDieuChinh = item.TenNhanVien;
        localStorage.setItem('lc_PhieuDCGV', JSON.stringify(lc_CapNhat));
    }

    self.filterNV = function (item, inputString) {
        var itemSearch = locdau(item.TenNhanVien).toLowerCase();
        var locdauInput = locdau(inputString).toLowerCase();
        var arr = itemSearch.split(/\s+/);
        var sThreechars = '';
        for (var i = 0; i < arr.length; i++) {
            sThreechars += arr[i].toString().split('')[0];
        }
        return itemSearch.indexOf(locdauInput) > -1 ||
            sThreechars.indexOf(locdauInput) > -1;
        //console.log(item);
        self.selectNameNV(item.TenNhanVien);
        self.NhanViens(sThreechars);

    }
    self.NoteMaPhieu = function () {
        _maHoaDon = $('#txtNoteMP').val();
        let lc_CapNhat = localStorage.getItem('lc_PhieuDCGV');
        if (lc_CapNhat === null) {
            lc_CapNhat = newPhieuDC();
        }
        else {
            lc_CapNhat = JSON.parse(lc_CapNhat);
        }
        lc_CapNhat.MaHoaDon = _maHoaDon;
        localStorage.setItem('lc_PhieuDCGV', JSON.stringify(lc_CapNhat));
    }
    self.Noteghichu = function () {
        _ghichu = $('#txtNote').val();
        let lc_CapNhat = localStorage.getItem('lc_PhieuDCGV');
        if (lc_CapNhat === null) {
            lc_CapNhat = newPhieuDC();
        }
        else {
            lc_CapNhat = JSON.parse(lc_CapNhat);
        }
        lc_CapNhat.GhiChu = _ghichu;
        lc_CapNhat.DienGiai = _ghichu;
        localStorage.setItem('lc_PhieuDCGV', JSON.stringify(lc_CapNhat));
    }
    // chuyển về trang trước
    self.btntrove = function () {
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        store.clear();
        window.location.href = '/#/CouponAdjustment';
    }
    var _ChoThanhToan;
    self.add_HoanThanh = function () {
        _ChoThanhToan = false;
        self.addHoaDon_DieuChinh(_ChoThanhToan);
    }
    self.add_TamLuu = function () {
        _ChoThanhToan = true;
        self.addHoaDon_DieuChinh(_ChoThanhToan);
    }
    function isValid(str) {
        return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
    };
    self.addHoaDon_DieuChinh = function (ChoThanhToan) {
        $('.phieu-dieu-chinh').gridLoader();
        if (lc_DieuChinh.length > 0) {
            if (!isValid(_maHoaDon)) {
                $('.bgwhite').hide();
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã phiếu không được nhập kí tự đặc biệt!", "danger");
                return false;
            }
            else {
                let dateCheck = CheckNgayLapHD_format($('#datetimepicker_mask').val());
                if (!dateCheck) {
                    return;
                }

                document.getElementById("add_HoanThanh").disabled = true;
                document.getElementById("add_TamLuu").disabled = true;
                if (ChoThanhToan)
                    document.getElementById("add_TamLuu").lastChild.data = " Đang lưu";
                else
                    document.getElementById("add_HoanThanh").lastChild.data = " Đang lưu";

                var HoaDon = {
                    ID_DonVi: _idDonVi,
                    ID_NhanVien: self.selectIDNV(),
                    MaHoaDon: _maHoaDon,
                    TongTienHang: _TongGiaVonHienTai,
                    TongChietKhau: _TongGiaVonMoi,
                    TongTienThue: _TongGiaVonTang,
                    TongChiPhi: _TongGiaVonGiam,
                    ChoThanhToan: ChoThanhToan,
                    DienGiai: _ghichu,
                    NguoiTao: _TenNguoiTao,
                    LoaiHoaDon: 18
                };
                var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
                var store = trans.objectStore(table);
                var req = store.getAll();
                var lcHD_ChiTiet = [];
                req.onsuccess = function (evt) {
                    if (req.result !== null) {
                        lc_DieuChinh = req.result;
                        lc_DieuChinh = sortByKey(lc_DieuChinh, 'SoThuTu');
                        lc_DieuChinh = lc_DieuChinh.sort(function (a, b) {
                            var x = a.SoThuTu, y = b.SoThuTu;
                            return x < y ? 1 : x > y ? -1 : 0;
                        });
                        for (var i = 0; i < lc_DieuChinh.length; i++) {
                            if (lc_DieuChinh[i].DM_LoHang.length > 0) {
                                for (var j = 0; j < lc_DieuChinh[i].DM_LoHang.length; j++) {
                                    if (lc_DieuChinh[i].DM_LoHang[j].TenLoHang == "") {
                                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Mã hàng: " + lc_DieuChinh[i].MaHangHoa + " chưa chọn lô hàng", "danger");
                                        document.getElementById("add_HoanThanh").disabled = false;
                                        document.getElementById("add_TamLuu").disabled = false;
                                        document.getElementById("add_HoanThanh").lastChild.data = " Hoàn thành";
                                        document.getElementById("add_TamLuu").lastChild.data = " Tạm Lưu";
                                        $('.phieu-dieu-chinh').gridLoader({ show: false });
                                        return false;
                                    }
                                    else {
                                        var ob = {
                                            ID_DonViQuiDoi: lc_DieuChinh[i].DM_LoHang[j].ID_DonViQuiDoi,
                                            ID_LoHang: lc_DieuChinh[i].DM_LoHang[j].ID_LoHang,
                                            MaHangHoa: lc_DieuChinh[i].DM_LoHang[j].MaHangHoa,
                                            GiaVonHienTai: lc_DieuChinh[i].DM_LoHang[j].GiaVonHienTai,
                                            GiaVonMoi: lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi,
                                            GiaVonTang: lc_DieuChinh[i].DM_LoHang[j].GiaVonTang,
                                            GiaVonGiam: lc_DieuChinh[i].DM_LoHang[j].GiaVonGiam,
                                            GhiChu: " (" + lc_DieuChinh[i].DM_LoHang[j].TenLoHang + ")"
                                        }
                                        lcHD_ChiTiet.push(ob);
                                    }
                                }
                            }
                            else {
                                var ob = {
                                    ID_DonViQuiDoi: lc_DieuChinh[i].ID_DonViQuiDoi,
                                    ID_LoHang: null,
                                    MaHangHoa: lc_DieuChinh[i].MaHangHoa,
                                    GiaVonHienTai: lc_DieuChinh[i].GiaVonHienTai,
                                    GiaVonMoi: lc_DieuChinh[i].GiaVonMoi,
                                    GiaVonTang: lc_DieuChinh[i].GiaVonTang,
                                    GiaVonGiam: lc_DieuChinh[i].GiaVonGiam,
                                    GhiChu: ""
                                }
                                lcHD_ChiTiet.push(ob);
                            }
                        }
                        var myData = {};
                        myData.objHoaDon = HoaDon;
                        myData.objHoaDonChiTiet = lcHD_ChiTiet;
                        callAjaxAdd(myData);
                    }
                };
            }
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn cần chọn hàng hóa điều chỉnh", "danger");
            $('.phieu-dieu-chinh').gridLoader({ show: false });
        }

    }
    function callAjaxAdd(myData) {
        // localStorage.removeItem('lc_PhieuHuy');
        $.ajax({
            data: myData,
            url: DieuChinhUri + "PostBH_DieuChinh?ID_DonVi=" + _idDonVi + "&ID_NhanVien=" + _id_NhanVien + "&ID_HoaDon=" + _ID_HoaDon + "&dateTaoPhieu=" + $('#datetimepicker_mask').val(),
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                localStorage.removeItem('lc_PhieuDCGV');
                if (_ChoThanhToan == false) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Điều chỉnh giá vốn thành công", "success");
                }
                else
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Tạm lưu phiếu điều chỉnh thành công", "success");
                document.getElementById("add_HoanThanh").disabled = false;
                document.getElementById("add_HoanThanh").lastChild.data = " Hoàn thành";
                document.getElementById("add_TamLuu").disabled = false;
                document.getElementById("add_TamLuu").lastChild.data = " Tạm lưu";

                var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
                var store = trans.objectStore(table);
                store.clear();
                window.location.href = '/#/CouponAdjustment';
                // tự động in
                if (self.SetupPrint().autoPrint == true) {
                    ajaxHelper(DieuChinhUri + "getList_HoaDonDieuChinhChiTiet?ID_HoaDon=" + item.ID).done(function (data) {
                        self.ChiTiet_HangHoaDieuChinh(data.LstData);
                        self.InHoaDon(item);
                    });
                }
                $('.phieu-dieu-chinh').gridLoader({ show: false });
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                    $('.phieu-dieu-chinh').gridLoader({ show: false });
                },
                500: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + item.responseJSON, "danger");
                    $('.phieu-dieu-chinh').gridLoader({ show: false });
                }
            },
            complete: function (item) {
                document.getElementById("add_HoanThanh").disabled = false;
                document.getElementById("add_HoanThanh").lastChild.data = " Hoàn thành";
                document.getElementById("add_TamLuu").disabled = false;
                document.getElementById("add_TamLuu").lastChild.data = " Tạm lưu";
                $('.phieu-dieu-chinh').gridLoader({ show: false });
            }
        })
    }

    self.InHoaDon = function (item) {
        var cthdFormat = GetCTHDPrint_Format(self.ChiTiet_HangHoaDieuChinh());
        self.CTHoaDonPrint(cthdFormat);
        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrintTypeChungTu?maChungTu=' + TeamplateDieuChinh + '&idDonVi=' + _idDonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data1 = '<script src="/Scripts/knockout-3.4.2.js"></script>';
                data1 = data1.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=" + JSON.stringify(self.CTHoaDonPrint())
                    + " ;var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data1 = data1.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(result, data1, self.numberPG());
            }
        });
    }
    self.PrinDieuChinh = function (item, key) {
        var cthdFormat = GetCTHDPrint_Format(self.ChiTiet_HangHoaDieuChinh());
        self.CTHoaDonPrint(cthdFormat);
        var itemHDFormat = GetInforHDPrint(item);
        self.InforHDprintf(itemHDFormat);
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetContentFIlePrint?idMauIn=' + key,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                var data = result;
                data = data.concat('<script src="/Scripts/knockout-3.4.2.js"></script>');
                data = data.concat("<script > var item1=" + JSON.stringify(self.CTHoaDonPrint())
                    + "; var item4=[], item5=[]; var item2=[];var item3=" + JSON.stringify(itemHDFormat) + "; </script>");
                data = data.concat(" <script type='text/javascript' src='/Scripts/Thietlap/MauInTeamplate.js'></script>");
                PrintExtraReport(data);
            }
        });
    };
    function LoadingForm(IsShow) {
        $('.phieu-dieu-chinh').gridLoader({ show: IsShow });
    }
    function GetCTHDPrint_Format(arrCTHD) {
        let arr = [];
        for (let i = 0; i < arrCTHD.length; i++) {
            let itFor = $.extend({}, arrCTHD[i]);
            itFor.TenHangHoa = itFor.TenHangHoaFull;
            itFor.GiaVonHienTai = formatNumber3Digit(arrCTHD[i].GiaVonHienTai);
            itFor.GiaVonMoi = formatNumber3Digit(arrCTHD[i].GiaVonMoi);
            itFor.ChenhLech = formatNumber3Digit(arrCTHD[i].ChenhLech);
            arr.push(itFor);
        }
        return arr;
    }
    function GetInforHDPrint(objHD) {
        let obj = $.extend({}, true, objHD);
        obj.NgayLapHoaDon = moment(objHD.NgayLapHoaDon, 'YYYY-MM-DD HH:mm:ss').format('DD/MM/YYYY HH:mm:ss');
        obj.NguoiTaoHD = objHD.NguoiDieuChinh == null ? self.selectNameNV() : objHD.NguoiDieuChinh;
        obj.GhiChu = objHD.DienGiai;

        // chinhanh
        let cn = $.grep(self.DonVis(), function (x) {
            return x.ID === objHD.ID_DonVi;
        });
        if (cn.length > 0) {
            obj.TenChiNhanh = cn[0].TenDonVi;
            obj.DienThoaiChiNhanh = cn[0].SoDienThoai;
            obj.DiaChiChiNhanh = cn[0].DiaChi;
        }

        if (self.CongTy().length > 0) {
            obj.LogoCuaHang = Open24FileManager.hostUrl + self.CongTy()[0].DiaChiNganHang;
            obj.TenCuaHang = self.CongTy()[0].TenCongTy;
            obj.DiaChiCuaHang = self.CongTy()[0].DiaChi;
            obj.DienThoaiCuaHang = self.CongTy()[0].SoDienThoai;
        }
        return obj;
    }
    function GetInforCongTy() {
        ajaxHelper('/api/DanhMuc/HT_API/' + 'GetHT_CongTy', 'GET').done(function (data) {
            if (data != null) {
                self.CongTy(data);
            }
        });
    }    

    function loadMauIn() {
        $.ajax({
            url: '/api/DanhMuc/ThietLapApi/GetListMauIn?typeChungTu=' + TeamplateDieuChinh + '&idDonVi=' + _idDonVi,
            type: 'GET',
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (result) {
                self.ListTypeMauIn(result);
            }
        });
    }

    self.showPrint = function () {
        $('.export-keys').hide();
        $(".install-notifi").toggle();
        $(".install-notifi").mouseup(function () {
            return false
        });
        $(".import-fast").mouseup(function () {
            return false;
        });
        $(document).mouseup(function () {
            $(".install-notifi").hide();
        });
        $(".function-keys").mouseup(function () {
            return false
        });
    }
    self.choose_autoPrint = function (item) {
        if (self.SetupPrint().autoPrint == true) {
            var obj = {
                autoPrint: false,
                numberPG: $('#txtPagePrint').val()
            };
            self.SetupPrint(obj);
            localStorage.setItem('lc_SetupPrint_DieuChinh', JSON.stringify(obj));
            $('#setautoPrint').removeClass('main-hide');
            $('#SetupOfPrint').removeClass('flaggOfPrint');
            $('.setNumberPage input').attr('disabled', 'false');
        }
        else {
            var obj = {
                autoPrint: true,
                numberPG: $('#txtPagePrint').val()
            };
            self.SetupPrint(obj);
            localStorage.setItem('lc_SetupPrint_DieuChinh', JSON.stringify(obj));
            $('#setautoPrint').addClass('main-hide');
            $('#SetupOfPrint').addClass('flaggOfPrint');
            $('.setNumberPage input').removeAttr('disabled');
        }
    }
    //load đơn vị
    function getDonVi() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + 'GetListDonVi_User?ID_NguoiDung=' + _IDDoiTuong, 'GET').done(function (data) {
            self.DonVis(data);
            self.searchDonVi(data);
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            for (var i = 0; i < self.DonVis().length; i++) {
                if (self.DonVis()[i].ID == _id_DonVi) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                    self.SelectedDonVi(self.DonVis()[i]);
                }
            }
        });
    }  

    $(document).on('click', '.per_ac1 li', function () {
        var ch = $(this).index();
        $(this).remove();
        var li = document.getElementById("selec-person");
        var list = li.getElementsByTagName("li");
        for (var i = 0; i < list.length; i++) {
            $("#selec-person ul li").eq(ch).find(".fa-check").css("display", "none");
        }
        var nameDV = _tenDonViSeach.split('-');
        _tenDonViSeach = null;
        for (var i = 0; i < nameDV.length; i++) {
            if (nameDV[i].trim() != $(this).text().trim()) {
                if (_tenDonViSeach == null) {
                    _tenDonViSeach = nameDV[i];
                }
                else {
                    _tenDonViSeach = nameDV[i] + "-" + _tenDonViSeach;
                }
            }
        }
        console.log(_tenDonViSeach);
        if (_tenDonViSeach.trim() == "null") {
        }
        else {
        }

    })

    self.CloseDonVi = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        _tenDonViSeach = null;
        var TenChiNhanh;
        self.MangChiNhanh.remove(item);
        for (var i = 0; i < self.MangChiNhanh().length; i++) {
            if (_tenDonViSeach == null) {
                _tenDonViSeach = self.MangChiNhanh()[i].ID;
                TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
            }
            else {
                _tenDonViSeach = self.MangChiNhanh()[i].ID + "," + _tenDonViSeach;
                TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
            }
        }
        if (self.MangChiNhanh().length === 0) {
            $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
            TenChiNhanh = 'Tất cả chi nhánh.'
            for (var i = 0; i < self.searchDonVi().length; i++) {
                if (_tenDonViSeach == null)
                    _tenDonViSeach = self.searchDonVi()[i].ID;
                else
                    _tenDonViSeach = self.searchDonVi()[i].ID + "," + _tenDonViSeach;
            }
        }
        self.TenChiNhanh(TenChiNhanh);
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        nextPage = 1;
        getAllHoaDon();
        _SuKienLoad = null;
    }

    self.SelectedDonVi = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        _tenDonViSeach = null;
        var TenChiNhanh;
        var arrIDDonVi = [];
        for (var i = 0; i < self.MangChiNhanh().length; i++) {
            if ($.inArray(self.MangChiNhanh()[i], arrIDDonVi) === -1) {
                arrIDDonVi.push(self.MangChiNhanh()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDDonVi) === -1) {
            self.MangChiNhanh.push(item);
            $('#NoteNameDonVi').removeAttr('placeholder');
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if (_tenDonViSeach == null) {
                    _tenDonViSeach = self.MangChiNhanh()[i].ID;
                    TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                }
                else {
                    _tenDonViSeach = self.MangChiNhanh()[i].ID + "," + _tenDonViSeach;
                    TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                }
            }
            self.TenChiNhanh(TenChiNhanh);
            nextPage = 1;
            getAllHoaDon();
            _SuKienLoad = null;
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //lọc đơn vị
    self.NoteNameDonVi = function () {
        var arrDonVi = [];
        var itemSearch = locdau($('#NoteNameDonVi').val().toLowerCase());
        for (var i = 0; i < self.searchDonVi().length; i++) {
            var locdauInput = locdau(self.searchDonVi()[i].TenDonVi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrDonVi.push(self.searchDonVi()[i]);
            }
        }
        self.DonVis(arrDonVi);
        if ($('#NoteNameDonVi').val() == "") {
            self.DonVis(self.searchDonVi());
        }
    }
    $('#NoteNameDonVi').keypress(function (e) {
        if (e.keyCode == 13 && self.DonVis().length > 0) {
            self.SelectedDonVi(self.DonVis()[0]);
        }
    });
    self.NoteMaHD = function () {
        _maDC_seach = $('#EnterMaXH').val();
    }
    var cacheExcel = true;
    function LoadHtmlGrid() {
        if (window.localStorage) {
            var current = localStorage.getItem('DieuChinh');
            if (!current) {
                current = [];
                cacheExcel = false;
                localStorage.setItem('DieuChinh', JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    $(current[i].NameClass)[0].style.display = 'none';
                    document.getElementById(current[i].NameId).checked = false;
                    if (cacheExcel) {
                        self.addColum(current[i].Value);
                    }
                }
                cacheExcel = false;
            }
        }
    }
    
    function getAllHoaDon() {
        $('.phieu-dieu-chinh').gridLoader();
        $('.line-right').height(0).css("margin-top", "0px");
        var FindHD = localStorage.getItem('FindDieuChinh');
        if (FindHD !== null) {
            var datime = new Date;
            timeStart = '2015-09-26'
            timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.timeValue('Toàn thời gian');
            _maDC_seach = FindHD;
            $('#EnterMaXH').val(FindHD);
            //hidewait_TR('table_h');
            ajaxHelper(DieuChinhUri + "getList_HoaDonDieuChinh?MaHoaDon=" + _maDC_seach + "&PageSize=" + _sohang + "&PageNumber=" + numberPage + "&dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&trangthai1=" + _trangthai1 + "&trangthai2=" + _trangthai2 + "&trangthai3=" + _trangthai3 + "&chinhanh=" + _tenDonViSeach).done(function (data) {
                self.HoaDons(data.LstData);
                self.TongGiaVonTang(data._thanhtien);
                self.TongGiaVonGiam(data._tienvon);
                LoadHtmlGrid();
                if (self.HoaDons().length != 0) {
                    $('#TongCongDieuChinh').show();
                    self.RowsStart((numberPage - 1) * _sohang + 1);
                    self.RowsEnd((numberPage - 1) * _sohang + self.HoaDons().length)
                }
                else {
                    $('#TongCongDieuChinh').hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.pageHangHoas(data.LstPageNumber);
                if (_SuKienLoad == null) {
                    self.selecPage();
                }
                else {
                    self.ReserPage();
                }
                self.RowsHangHoas(data.Rowcount);
                $('.phieu-dieu-chinh').gridLoader({ show: false });
                //$("div[id ^= 'wait']").text("");
            });
        } else {
            //hidewait_TR('table_h');
            $('.phieu-dieu-chinh').gridLoader();
            ajaxHelper(DieuChinhUri + "getList_HoaDonDieuChinh?MaHoaDon=" + _maDC_seach + "&PageSize=" + _sohang + "&PageNumber=" + numberPage + "&dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&trangthai1=" + _trangthai1 + "&trangthai2=" + _trangthai2 + "&trangthai3=" + _trangthai3 + "&chinhanh=" + _tenDonViSeach).done(function (data) {
                self.HoaDons(data.LstData);
                self.TongGiaVonTang(data._thanhtien);
                self.TongGiaVonGiam(data._tienvon);
                if (window.location.hash.split("?")[0].split("/").length < 3)
                    LoadHtmlGrid();

                if (self.HoaDons().length != 0) {
                    $('#TongCongDieuChinh').show();
                    self.RowsStart((numberPage - 1) * _sohang + 1);
                    self.RowsEnd((numberPage - 1) * _sohang + self.HoaDons().length)
                }
                else {
                    $('#TongCongDieuChinh').hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.pageHangHoas(data.LstPageNumber);
                if (_SuKienLoad == null) {
                    self.selecPage();
                }
                else {
                    self.ReserPage();
                }
                self.RowsHangHoas(data.Rowcount);
                $('.phieu-dieu-chinh').gridLoader({ show: false });
                //$("div[id ^= 'wait']").text("");
            });
        }
        localStorage.removeItem('FindDieuChinh');
    }
    function getSelectPagesHD() {
        //hidewait_TR('table_h');
        $('.phieu-dieu-chinh').gridLoader();
        ajaxHelper(DieuChinhUri + "getList_HoaDonDieuChinh?MaHoaDon=" + _maDC_seach + "&PageSize=" + _sohang + "&PageNumber=" + numberPage + "&dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&trangthai1=" + _trangthai1 + "&trangthai2=" + _trangthai2 + "&trangthai3=" + _trangthai3 + "&chinhanh=" + _tenDonViSeach).done(function (data) {
            self.HoaDons(data.LstData);
            LoadHtmlGrid();
            if (self.HoaDons().length != 0) {
                $('#TongCongDieuChinh').show();
                self.RowsStart((numberPage - 1) * _sohang + 1);
                self.RowsEnd((numberPage - 1) * _sohang + self.HoaDons().length)
            }
            else {
                $('#TongCongDieuChinh').hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            self.RowsHangHoas(data.Rowcount);
            self.pageHangHoas(data.LstPageNumber);
            self.ReserPage();
            $('.phieu-dieu-chinh').gridLoader({ show: false });
            //$("div[id ^= 'wait']").text("");
        });
    }

    self.SelectedHHEnterkey_LoHang = function (item) {
        getChiTietHangHoaLoHang(item);
    }
    //lọc Lô hàng
    var dk_lohang = 0;
    var indexLi = 0;
    var idFocus = null;
    self.NoteLoHang = function (item) {
        self.listDM_LoHang([]);
        $(".month-oll li").each(function () {
            $(this).removeClass('k-select');
            $(this).find('i').remove();
        });
        thisLH_U = "#month-oll_" + item.ID_DonViQuiDoi;
        var $this = event.currentTarget; // lấy thẻ đang có sự kiện 
        var txt = $($this).val()
        var arrLoHang = [];
        var itemSearch = locdau(txt.toLowerCase());
        for (var i = 0; i < self.searchLoHang().length; i++) {
            var locdauInput = locdau(self.searchLoHang()[i].TenLoHang).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrLoHang.push(self.searchLoHang()[i]);
            }
        }
        self.listDM_LoHang(arrLoHang);
        if (txt == "") {
            self.listDM_LoHang(self.searchLoHang());
        }

        if (self.listDM_LoHang().length > 0)
            $(thisLH_U).show();
        else
            $(thisLH_U).hide();
        var keyCode = event.keyCode || event.which;
        var li = $($this).parent().next().find('li');

        if (keyCode != 13) {
            // reset again indexLi if is search
            if (keyCode !== 38 && keyCode !== 40) {
                indexLi = 0;
            }
            else {
                // else: if key = 38/40 --> find li focus
                $(li).each(function (index) {
                    if ($(this).hasClass('focusLot')) {
                        indexLi = index;
                        return;
                    }
                })
            }
            ////remove class focus for all li
            li.removeClass('focusLot');
            //// up : len 
            if (keyCode === 38) {
                indexLi = indexLi - 1;
                if (indexLi < 0) {
                    indexLi = 0;
                }
            }
            // down: xuong
            if (keyCode === 40) {
                indexLi = indexLi + 1;
                // neu index vuot qua do dai cua mang Lot
                if (indexLi > self.listDM_LoHang().length - 1) {
                    indexLi = 0;
                }
            }
            $(li).eq(indexLi).addClass('focusLot');
        }
        // enter --> chose Lot and update CTHD
        if (keyCode === 13) {
            idFocus = $(li).eq(indexLi).attr('id');
            var itemFocus = $.grep(self.listDM_LoHang(), function (x) {
                return 'li_lohang' + item.ID_DonViQuiDoi + x.ID_LoHang === idFocus;
            });
            if (itemFocus.length > 0) {
                if (item.DM_LoHang.length > 0) {
                    for (var i = 0; i < item.DM_LoHang.length; i++) {
                        if (item.DM_LoHang[i].ID_LoHang == itemFocus[0].ID_LoHang) {
                            bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Lô hàng '" + itemFocus[0].TenLoHang + "'  đã được chọn", "danger");
                            self.resetCache();
                            break;
                        }
                        if (i == item.DM_LoHang.length - 1) {
                            self.SelectDMLoHang(itemFocus[0]);
                        }
                    }
                }
                else {
                    self.SelectDMLoHang(itemFocus[0]);
                }
            }
        }
        var thisLi = '#li_lohang' + item.ID_DonViQuiDoi + item.DM_LoHang[0].ID_LoHang;
        $(thisLi).addClass('k-select')
    }
    self.NoteLoHangIndex = function (item) {
        self.listDM_LoHang([]);
        thisLH_U = "#month-oll_" + item.ID_DonViQuiDoi + item.ID_LoHang;
        $(".month-oll li").each(function () {
            $(this).removeClass('k-select');
            $(this).find('i').remove();
        });
        var $this = event.currentTarget; // lấy thẻ đang có sự kiện 
        var txt = $($this).val()
        var arrLoHang = [];
        var itemSearch = locdau(txt.toLowerCase());
        for (var i = 0; i < self.searchLoHang().length; i++) {
            var locdauInput = locdau(self.searchLoHang()[i].TenLoHang).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrLoHang.push(self.searchLoHang()[i]);
            }
        }
        self.listDM_LoHang(arrLoHang);
        if (txt == "") {
            self.listDM_LoHang(self.searchLoHang());
        }
        if (self.listDM_LoHang().length > 0)
            $(thisLH_U).show();
        else
            $(thisLH_U).hide();
        var keyCode = event.keyCode || event.which;
        var li = $($this).parent().next().find('li');
        if (keyCode != 13) {
            // reset again indexLi if is search
            if (keyCode !== 38 && keyCode !== 40) {
                indexLi = 0;
            }
            else {
                // else: if key = 38/40 --> find li focus
                $(li).each(function (index) {
                    if ($(this).hasClass('focusLot')) {
                        indexLi = index;
                        return;
                    }
                })
            }
            //remove class focus for all li
            li.removeClass('focusLot');
            // up : len 
            if (keyCode === 38) {
                indexLi = indexLi - 1;
                if (indexLi < 0) {
                    indexLi = 0;
                }
            }
            // down: xuong
            if (keyCode === 40) {
                indexLi = indexLi + 1;
                // neu index vuot qua do dai cua mang Lot
                if (indexLi > self.listDM_LoHang().length - 1) {
                    indexLi = 0;
                }
            }
            $(li).eq(indexLi).addClass('focusLot');
        }
        if (keyCode === 13) {
            idFocus = $(li).eq(indexLi).attr('id');
            var itemFocus = $.grep(self.listDM_LoHang(), function (x) {
                return 'li_lohang' + TenLoHang_Index + x.ID_LoHang === idFocus;
            });
            if (itemFocus.length > 0) {
                self.SelectDMLoHang(itemFocus[0]);
            }
        }
        var thisLi = '#li_lohang' + item.TenLoHang + item.ID_LoHang;
        $(thisLi).addClass('k-select')
        //$(thisLi).find('a').append('<i class="fa fa-check check-after-litr"></i>')
    }
    self.deleteDM_LoHang = function (item) {
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        var req1 = store.openCursor(item.ID_DonViQuiDoi);
        req1.onsuccess = function (evt) {
            if (req1.result !== null) {
                var lc_Edit = req1.result.value;
                for (var i = 0; i < lc_Edit.DM_LoHang.length; i++) {
                    if (lc_Edit.DM_LoHang[i].ID_LoHang == item.ID_LoHang) {
                        lc_Edit.DM_LoHang.splice(i, 1);
                        break;
                    }
                }
                store.put(lc_Edit)
                self.resetCache();
            }
        };
    }
    self.SelectDMLoHang = function (item) {
        console.log(item);
        var lh = $(thisLH).val();
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        var req1 = store.openCursor(_ID_DonViQuiDoi);
        req1.onsuccess = function (evt) {
            if (req1.result !== null) {
                var lc_Edit = req1.result.value;
                for (var i = 0; i < lc_Edit.DM_LoHang.length; i++) {
                    if (lc_Edit.DM_LoHang[i].ID_LoHang == item.ID_LoHang) {
                        $(thisLH).val(lh)
                        bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Lô hàng '" + item.TenLoHang + "'  đã được chọn", "danger");
                        self.resetCache();
                        break;
                    }
                    if (i == lc_Edit.DM_LoHang.length - 1) {
                        for (var i = 0; i < lc_Edit.DM_LoHang.length; i++) {
                            if (lc_Edit.DM_LoHang[i].ID_LoHang == _ID_LoHang) {
                                lc_Edit.DM_LoHang[i].ID_DonViQuiDoi = _ID_DonViQuiDoi;
                                lc_Edit.DM_LoHang[i].ID_LoHang = item.ID_LoHang;
                                lc_Edit.DM_LoHang[i].MaHangHoa = lc_Edit.MaHangHoa + " .Lô " + item.TenLoHang;
                                lc_Edit.DM_LoHang[i].TenLoHang = item.TenLoHang;
                                lc_Edit.DM_LoHang[i].NgaySanXuat = item.NgaySanXuat;
                                lc_Edit.DM_LoHang[i].NgayHetHan = item.NgayHetHan;
                                lc_Edit.DM_LoHang[i].GiaVonHienTai = item.GiaVon;
                                lc_Edit.DM_LoHang[i].GiaVonMoi = lc_Edit.DM_LoHang[i].GiaVonMoi == 0 ? item.GiaVon : lc_Edit.DM_LoHang[i].GiaVonMoi;
                                var chenhlech = parseFloat(lc_Edit.DM_LoHang[i].GiaVonMoi) - parseFloat(item.GiaVon);
                                if (chenhlech >= 0) {
                                    lc_Edit.DM_LoHang[i].GiaVonTang = chenhlech;
                                    lc_Edit.DM_LoHang[i].GiaVonGiam = 0;
                                    lc_Edit.GiaVonTang = chenhlech;
                                    lc_Edit.GiaVonGiam = 0;
                                }
                                else {
                                    lc_DieuChinh[i].GiaVonTang = 0;
                                    lc_DieuChinh[i].GiaVonGiam = chenhlech;
                                    lc_Edit.GiaVonTang = 0;
                                    lc_Edit.GiaVonGiam = chenhlech;
                                }
                                if (i == 0) {
                                    lc_Edit.GiaVonHienTai = item.GiaVon;
                                    lc_Edit.GiaVonMoi = lc_Edit.DM_LoHang[i].GiaVonMoi;
                                    lc_Edit.GiaVonTang = lc_Edit.DM_LoHang[i].GiaVonTang;
                                    lc_Edit.GiaVonGiam = lc_Edit.DM_LoHang[i].GiaVonGiam;
                                }
                                req1.result.update(req1.result.value);
                                break;
                            }
                        }
                        self.resetCache();

                    }
                }
                //self.SelectedPageNumberDieuChinh();
            }
        };
    }

    var _ID_DonViQuiDoi = null;
    var _ID_LoHang;
    var thisLH;
    var thisLH_U
    self.getListDM_LoHang = function (item) {
        indexLi = 0;
        thisLH_U = "#month-oll_" + item.ID_DonViQuiDoi;
        var thisK = thisLH_U + " li";
        $(".month-oll li").each(function () {
            $(this).removeClass('k-select');
            $(this).find('i').remove();
        });
        dk_lohang = 1;
        thisLH = "#lohang_" + item.ID_DonViQuiDoi;
        $(thisLH).focus().select();
        _ID_DonViQuiDoi = item.ID_DonViQuiDoi;
        _ID_LoHang = item.DM_LoHang[0].ID_LoHang;
        self.listDM_LoHang([]);
        ajaxHelper(BH_XuatHuyUri + "getListDM_LoHang?ID_DonViQuiDoi=" + item.ID_DonViQuiDoi + "&ID_ChiNhanh=" + _id_DonVi + "&ID_NhanVien=" + _IDDoiTuong, "GET").done(function (data) {
            self.listDM_LoHang(data.LstData);
            self.searchLoHang(data.LstData);
            if (self.listDM_LoHang().length > 0) {
                $(thisLH_U).show();
            }
            var thisLi = '#li_lohang' + item.ID_DonViQuiDoi + item.DM_LoHang[0].ID_LoHang;
            $(thisLi).addClass('k-select')
        });
    }
    var TenLoHang_Index;
    self.getListDM_LoHangIndex = function (item) {
        TenLoHang_Index = item.TenLoHang;
        dk_lohang = 1;
        indexLi = 0;
        $(".month-oll li").each(function () {
            $(this).removeClass('k-select');
            $(this).find('i').remove();
        });
        thisLH_U = "#month-oll_" + item.ID_DonViQuiDoi + item.ID_LoHang;
        thisLH = "#lohang_" + item.ID_LoHang;
        $(thisLH).focus().select();
        _ID_DonViQuiDoi = item.ID_DonViQuiDoi;
        _ID_LoHang = item.ID_LoHang;
        self.listDM_LoHang([]);
        ajaxHelper(BH_XuatHuyUri + "getListDM_LoHang?ID_DonViQuiDoi=" + item.ID_DonViQuiDoi + "&ID_ChiNhanh=" + _id_DonVi + "&ID_NhanVien=" + _IDDoiTuong, "GET").done(function (data) {
            self.listDM_LoHang(data.LstData);
            self.searchLoHang(data.LstData);
            if (self.listDM_LoHang().length > 0) {
                $(thisLH_U).show();
            }
            var thisLi = '#li_lohang' + item.TenLoHang + item.ID_LoHang;
            $(thisLi).addClass('k-select')
            //$(thisLi).find('a').append('<i class="fa fa-check check-after-litr"></i>')
        });
    }
    var item_NhapCham = null;
    function getChiTietHangHoaLoHang(item) {
        if (dk_check == 1) {
            STT = STT + 1;
            //$('#importDieuChinh').hide();();
            var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
            var store = trans.objectStore(table);
            var req = store.getAll();
            req.onsuccess = function (e) {
                self.MaHangHoa_Search(item.TenLoHang != "" ? item.MaHangHoa + " .LSX: " + item.TenLoHang : item.MaHangHoa);
                var found = -1;
                lc_DieuChinh = req.result;
                var ob2 = [];
                if (item.QuanLyTheoLoHang == 1) {
                    ob2 = [{
                        ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                        ID_LoHang: item.ID_LoHang,
                        MaHangHoa: item.MaHangHoa,
                        TenLoHang: item.TenLoHang,
                        NgaySanXuat: item.NgaySanXuat,
                        NgayHetHan: item.NgayHetHan,
                        GiaVonHienTai: item.GiaVon,
                        GiaVonMoi: item.GiaVon,
                        GiaVonTang: 0,
                        GiaVonGiam: 0,
                    }]
                }
                var ob1 = {
                    ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                    MaHangHoa: item.MaHangHoa,
                    TenHangHoa: item.TenHangHoa,
                    TenDonViTinh: item.TenDonViTinh,
                    ThuocTinh_GiaTri: item.ThuocTinh_GiaTri,
                    GiaVonHienTai: item.GiaVon,
                    GiaVonMoi: item.GiaVon,
                    GiaVonTang: 0,
                    GiaVonGiam: 0,
                    QuanLyTheoLoHang: item.QuanLyTheoLoHang,
                    DM_LoHang: ob2,
                    SoThuTu: STT
                }
                if (lc_DieuChinh == null) { // trả về danh sách trống khi cache rỗng
                    lc_DieuChinh = [];
                }
                else {
                    for (var i = 0; i < lc_DieuChinh.length; i++) {
                        if (lc_DieuChinh[i].MaHangHoa == item.MaHangHoa) {
                            found = i;
                            break;
                        }
                    }
                }
                if (found < 0) {
                    lc_DieuChinh.unshift(ob1); // add hàng hóa được chọn vào
                    store.add(ob1); // lưu indexedDB
                    PX_PageSize = PX_PageSize + 1;
                    self.SumRowsHangHoaDieuChinh(PX_PageSize);
                    if (PX_PageSize > 10)
                        $('.pg_PhieuDC').show();
                    else
                        $('.pg_PhieuDC').hide();
                    AllPageHangHoa = PX_PageSize / _pageSizeHangHoa;
                    if (AllPageHangHoa > parseInt(AllPageHangHoa)) {
                        AllPageHangHoa = parseInt(AllPageHangHoa) + 1;
                    }
                    //self.selecPageHangHoa();
                }
                else {
                    for (var i = 0; i < lc_DieuChinh.length; i++) {
                        if (lc_DieuChinh[i].MaHangHoa == item.MaHangHoa) {
                            if (item.QuanLyTheoLoHang != 1) {
                                lc_DieuChinh[i].GiaVonHienTai = item.GiaVon;
                                lc_DieuChinh[i].GiaVonMoi = lc_DieuChinh[i].GiaVonMoi;
                                var chenhlech = parseFloat(lc_DieuChinh[i].GiaVonMoi) - parseFloat(item.GiaVon);
                                if (chenhlech >= 0) {
                                    lc_DieuChinh[i].GiaVonTang = chenhlech;
                                    lc_DieuChinh[i].GiaVonGiam = 0;
                                }
                                else {
                                    lc_DieuChinh[i].GiaVonTang = 0;
                                    lc_DieuChinh[i].GiaVonGiam = chenhlech;
                                }
                            }
                            else {
                                for (var j = 0; j < lc_DieuChinh[i].DM_LoHang.length; j++) {
                                    if (lc_DieuChinh[i].DM_LoHang[j].ID_LoHang == item.ID_LoHang) {
                                        if (j == 0) {
                                            lc_DieuChinh[i].GiaVonHienTai = item.GiaVon;
                                            lc_DieuChinh[i].GiaVonMoi = lc_DieuChinh[i].GiaVonMoi;
                                            var chenhlech = parseFloat(lc_DieuChinh[i].GiaVonMoi) - parseFloat(item.GiaVon);
                                            if (chenhlech >= 0) {
                                                lc_DieuChinh[i].GiaVonTang = chenhlech;
                                                lc_DieuChinh[i].GiaVonGiam = 0;
                                            }
                                            else {
                                                lc_DieuChinh[i].GiaVonTang = 0;
                                                lc_DieuChinh[i].GiaVonGiam = chenhlech;
                                            }
                                        }
                                        lc_DieuChinh[i].DM_LoHang[j].GiaVonHienTai = item.GiaVon;
                                        lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi = lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi;
                                        var chenhlech = parseFloat(lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi) - parseFloat(item.GiaVon);
                                        if (chenhlech >= 0) {
                                            lc_DieuChinh[i].DM_LoHang[j].GiaVonTang = chenhlech;
                                            lc_DieuChinh[i].DM_LoHang[j].GiaVonGiam = 0;
                                        }
                                        else {
                                            lc_DieuChinh[i].DM_LoHang[j].GiaVonTang = 0;
                                            lc_DieuChinh[i].DM_LoHang[j].GiaVonGiam = chenhlech;
                                        }
                                        break;
                                    }
                                    if (j == lc_DieuChinh[i].DM_LoHang.length - 1) {
                                        var ob2 = {
                                            ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                                            ID_LoHang: item.ID_LoHang,
                                            MaHangHoa: item.MaHangHoa,
                                            TenLoHang: item.TenLoHang,
                                            NgaySanXuat: item.NgaySanXuat,
                                            NgayHetHan: item.NgayHetHan,
                                            GiaVonHienTai: item.GiaVon,
                                            GiaVonMoi: item.GiaVon,
                                            GiaVonTang: 0,
                                            GiaVonGiam: 0,
                                        }
                                        lc_DieuChinh[i].DM_LoHang.push(ob2)
                                    }
                                }
                            }
                            lc_DieuChinh[i].SoThuTu = STT;
                            store.put(lc_DieuChinh[i]);
                            break;
                        }
                    }
                }
                self.resetCache();
                self.selectedHH(undefined);
                $('#txtAutoHangHoa').val(self.MaHangHoa_Search());
                $('#txtAuto').focus();
                $('#txtAutoHangHoa').focus().select();

                GetTonKho_byIDQuyDois();
            }
        }
        else {
            item_NhapCham = item;
            ajaxHelper(BH_XuatHuyUri + "GetList_TenDonViTinh?maHH=" + item.MaHangHoa, 'GET').done(function (data) {
                if (data.length > 1) {
                    ///console.log(data);
                    self.TenDonViTinh(data);
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].MaHangHoa == MaHH) {
                            self.selectedTenDonViTinh(data[i].MaHangHoa);
                            break;
                        }
                    }
                    $('#txtSelectHT').removeAttr('disabled');
                    $('#txtSoLuongXH').val(1);
                    $('#txtSoLuongXH').focus().select();
                }
                else {
                    self.TenDonViTinh([{ MaHangHoa: '', TenDonViTinh: "Đơn Vị" }]);
                    $('#txtSelectHT').attr('disabled', 'false');
                    $('#txtSoLuongXH').val(1);
                    $('#txtSoLuongXH').focus().select();
                }
            });
        }
    }
    //phân trang

    self.selecPage = function () {
        AllPage = self.pageHangHoas().length;
        if (AllPage > 4) {
            for (var i = 3; i < AllPage; i++) {
                self.pageHangHoas.pop(i + 1);
            }
            self.pageHangHoas.push({ SoTrang: '4' });
            self.pageHangHoas.push({ SoTrang: '5' });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.pageHangHoas.pop(i);
            }
            for (var j = 0; j < AllPage; j++) {
                self.pageHangHoas.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage').hide();
        $('#BackPage').hide();
        $('#NextPage').show();
        $('#EndPage').show();
    }
    self.ReserPage = function (item) {
        self.selecPage();
        if (nextPage > 1 && AllPage > 5/* && nextPage < AllPage - 1*/) {
            if (nextPage > 3 && nextPage < AllPage - 1) {
                for (var i = 0; i < 5; i++) {
                    self.pageHangHoas.replace(self.pageHangHoas()[i], { SoTrang: parseInt(nextPage) + i - 2 });
                }
            }
            else if (parseInt(nextPage) === parseInt(AllPage) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.pageHangHoas.replace(self.pageHangHoas()[i], { SoTrang: parseInt(nextPage) + i - 3 });
                }
            }
            else if (nextPage == AllPage) {
                for (var i = 0; i < 5; i++) {
                    self.pageHangHoas.replace(self.pageHangHoas()[i], { SoTrang: parseInt(nextPage) + i - 4 });
                }
            }
        }
        self.currentPage(parseInt(nextPage));
        if (nextPage > 1) {
            $('#StartPage').show();
            $('#BackPage').show();
        }
        else {
            $('#StartPage').hide();
            $('#BackPage').hide();
        }
        if (nextPage == AllPage) {
            $('#NextPage').hide();
            $('#EndPage').hide();
        }
        else {
            $('#NextPage').show();
            $('#EndPage').show();
        }
    }
    self.NextPage = function (item) {
        if (nextPage < AllPage) {
            nextPage = nextPage + 1;
            numberPage = nextPage;
            self.ReserPage();
            getSelectPagesHD();
        }
    };
    self.BackPage = function (item) {
        if (nextPage > 1) {
            nextPage = nextPage - 1;
            numberPage = nextPage;
            self.ReserPage();
            getSelectPagesHD();
        }
    };
    self.EndPage = function (item) {
        nextPage = AllPage;
        numberPage = nextPage;
        self.ReserPage();
        getSelectPagesHD();
    };
    self.StartPage = function (item) {
        nextPage = 1;
        numberPage = nextPage;
        self.ReserPage();
        getSelectPagesHD();
    };
    self.gotoNextPage = function (item) {
        nextPage = item.SoTrang;
        numberPage = item.SoTrang;
        self.ReserPage();
        getSelectPagesHD();
    };
    $('.choseTamLuu input').on('click', function () {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        if ($(this).val() == 1) {
            $(this).val(0);
            _trangthai1 = "0";
        }
        else {
            $(this).val(1);
            _trangthai1 = "Tạm lưu";
        }
        SuKien = null;
        nextPage = 1;
        numberPage = nextPage;
        _page = 1;
        getAllHoaDon();
        //self.getFilterTrangthai();
        self.currentPage(1);
    });
    $('.choseHoanThanh input').on('click', function () {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        if ($(this).val() == 2) {
            $(this).val(0);
            _trangthai2 = "0";
        }
        else {
            $(this).val(2);
            _trangthai2 = "Hoàn thành";
        }
        SuKien = null;
        nextPage = 1;
        numberPage = nextPage;
        _page = 1;
        getAllHoaDon();
        //self.getFilterTrangthai();
        self.currentPage(1);
    });

    $('.choseHuy input').on('click', function () {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        if ($(this).val() == 3) {
            $(this).val(0);
            _trangthai3 = "Hủy bỏ";
        }
        else {
            $(this).val(3);
            _trangthai3 = "0";
        }
        SuKien = null;
        nextPage = 1;
        numberPage = nextPage;
        _page = 1;
        getAllHoaDon();
        //self.getFilterTrangthai();
        self.currentPage(1);
    });
    $("#EnterMaXH").keypress(function (e) {
        if (e.keyCode == 13) {
            _maDC_seach = $(this).val();
            nextPage = 1;
            numberPage = nextPage;
            getAllHoaDon();
            self.ReserPage();
        }
    });
  
    $('.choose_TimeReport input').on('click', function () {
        _rdTime = $(this).val()
        if ($(this).val() == 0) {
            $('.ip_TimeReport').removeAttr('disabled');
            $('.dr_TimeReport').attr("data-toggle", "dropdown");
            $('.ip_DateReport').attr('disabled', 'false');
            self.TodayBC($('.ip_TimeReport').val());
            var _rdoNgayPage = $('.ip_TimeReport').val();
            var datime = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === "Toàn thời gian") {
                timeStart = '2015-09-26'
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            }
            //Tuần này
            else if (_rdoNgayPage === "Tuần này") {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            }
            //Tuần trước
            else if (_rdoNgayPage === "Tuần trước") {
                timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            }
            //7 ngày qua
            else if (_rdoNgayPage === "7 ngày qua") {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            }
            //Tháng trước
            else if (_rdoNgayPage === "Tháng trước") {
                timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            }
            //30 ngày qua
            else if (_rdoNgayPage === "30 ngày qua") {
                timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Quý này
            else if (_rdoNgayPage === "Quý này") {
                timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            // Quý trước
            else if (_rdoNgayPage === "Quý trước") {
                var prevQuarter = moment().quarter() - 1;
                if (prevQuarter === 0) {
                    // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                    let prevYear = moment().year() - 1;
                    timeStart = prevYear + '-' + '10-01';
                    timeEnd = moment().year() + '-' + '01-01';
                }
                else {
                    timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                    timeEnd = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                }
            }
            //Năm này
            else if (_rdoNgayPage === "Năm này") {
                timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //năm trước
            else if (_rdoNgayPage === "Năm trước") {
                var prevYear = moment().year() - 1;
                timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            numberPage = 1;
            nextPage = 1;
            self.currentPage(numberPage);
            getAllHoaDon();
        }
        else if ($(this).val() == 1) {
            $('.ip_DateReport').removeAttr('disabled');
            $('.ip_TimeReport').attr('disabled', 'false');
            $('.dr_TimeReport').removeAttr('data-toggle');
            self.TodayBC($('.ip_DateReport').val())
            if ($('.ip_DateReport').val() != "") {
                thisDate = $('.ip_DateReport').val();
                var t = thisDate.split("-");
                var t1 = t[0].trim().split("/").reverse().join("-")
                var thisDateStart = moment(t1).format('MM/DD/YYYY')
                var t2 = t[1].trim().split("/").reverse().join("-")
                var thisDateEnd = moment(t2).format('MM/DD/YYYY')
                timeStart = moment(new Date(thisDateStart)).format('YYYY-MM-DD');
                var dt = new Date(thisDateEnd);
                timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                numberPage = 1;
                nextPage = 1;
                self.currentPage(numberPage);
                getAllHoaDon();
            }
        }
    })
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        timeStart = picker.startDate.format('YYYY-MM-DD');
        timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        self.TodayBC($(this).val())
        self.currentPage(1);
        if (self.filterNgayPhieuHuy() === '1') {
            _SuKienLoad = null;
            getAllHoaDon();
        }
    });
    $('.choose_txtTime li').on('click', function () {
        self.TodayBC($(this).text())
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 17) {
            console.log('1');
            timeStart = '2015-09-26'
            timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
        }
        //Tuần này
        else if (_rdoNgayPage === 3) {
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
            timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            var newtime = new Date();
            timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Tháng này
        else if (_rdoNgayPage === 6) {
            timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
        }
        //Tháng trước
        else if (_rdoNgayPage === 7) {
            timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
        }
        //30 ngày qua
        else if (_rdoNgayPage === 10) {
            timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            var newtime = new Date();
            timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Quý này
        else if (_rdoNgayPage === 11) {
            timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('quarter'));
            timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        // Quý trước
        else if (_rdoNgayPage === 12) {
            var prevQuarter = moment().quarter() - 1;
            if (prevQuarter === 0) {
                // get quy 4 cua nam truoc 01/10/... -->  31/21/...
                let prevYear = moment().year() - 1;
                timeStart = prevYear + '-' + '10-01';
                timeEnd = moment().year() + '-' + '01-01';
            }
            else {
                timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                timeEnd = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
            }
        }
        //Năm này
        else if (_rdoNgayPage === 13) {
            timeStart = moment().startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('year'));
            timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //năm trước
        else if (_rdoNgayPage === 14) {
            var prevYear = moment().year() - 1;
            timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().year(prevYear).endOf('year'));
            timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        pageNumber = 1;
        getAllHoaDon();
    })
    self.ResetCurrentPage = function () {
        _sohang = self.pageSize();
        nextPage = 1;
        numberPage = 1;
        getAllHoaDon();
        self.currentPage(1);
    };
    var itemDieuChinh;
    var arrayIDHDXH = [];

    self.getList_DieuChinhChiTiet = function (item, e) {
        self.Enable_NgayLapHD(!VHeader.CheckKhoaSo(moment(item.NgayLapHoaDon).format('YYYY-MM-DD'), item.ID_DonVi));

        itemDieuChinh = item;
        localStorage.removeItem('lc_PhieuDCGV');
        if (arrayIDHDXH.filter(arrayIDHDXH => arrayIDHDXH.key === item.ID_HoaDon)[0] === undefined) {
            ajaxHelper(DieuChinhUri + "getList_HoaDonDieuChinhChiTiet?ID_HoaDon=" + item.ID_HoaDon).done(function (data) {
                self.ChiTiet_HangHoaDieuChinh(data.LstData);
                self.sum_GiaVonTang(data._thanhtien);
                self.sum_GiaVonGiam(data._tienvon);
                var model = {
                    key: item.ID_HoaDon,
                    Value: data.LstData,
                    GV_Giam: data._tienvon,
                    GV_Tang: data._thanhtien
                }
                arrayIDHDXH.push(model);
                SetHeightShowDetail($(e.currentTarget));
            });
        }
        else {
            self.ChiTiet_HangHoaDieuChinh(arrayIDHDXH.filter(arrayIDHDXH => arrayIDHDXH.key === item.ID_HoaDon)[0].Value);
            self.sum_GiaVonTang(arrayIDHDXH.filter(arrayIDHDXH => arrayIDHDXH.key === item.ID_HoaDon)[0].GV_Giam);
            self.sum_GiaVonGiam(arrayIDHDXH.filter(arrayIDHDXH => arrayIDHDXH.key === item.ID_HoaDon)[0].GV_Tang);
            SetHeightShowDetail($(e.currentTarget));
        }
    }

    var itemDelete;
    self.modalDelete = function (item) {
        itemDelete = item;
        self.deleteMaHoaDon(item.MaHoaDon);
        $('#modalpopup_deleteHD').modal('show');
    };
    self.DeleteHD_DieuChinh = function () {
        $.ajax({
            data: null,
            url: DieuChinhUri + "deleteHoaDonDieuChinh?ID_DonVi=" + _idDonVi + "&ID_NhanVien=" + _id_NhanVien + "&ID_HoaDon=" + itemDelete.ID_HoaDon,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (ex) {

            },
            statusCode: {
                404: function (ex) {

                    self.error("page not found");
                },
                500: function (ex) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Hủy bỏ phiếu điều chỉnh không thành công", "danger");
                }
            },
            complete: function (ex) {
                $('#modalpopup_deleteHD').modal('hide');
                getAllHoaDon();
            }
        })
    }
    self.openPhieuDieuChinh = function (item) {
        localStorage.setItem('lc_PhieuDCGV', JSON.stringify(item));
        var trans = db.transaction(table, "readwrite");
        var store = trans.objectStore(table);
        store.clear();
        var req = store.getAll();
        req.onsuccess = function (evt) {
            localStorage.setItem('lc_PhieuDCGV', JSON.stringify(item));
            for (var i = 0; i < self.ChiTiet_HangHoaDieuChinh().length; i++) {
                self.UpdatePhieuDieuChinh(self.ChiTiet_HangHoaDieuChinh()[i]);
            }
            window.location.href = '/#/CouponAdjustment/Adjustment';
        }
    }
    self.UpdatePhieuDieuChinh = function (item) {
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        var req = store.getAll();
        req.onsuccess = function (e) {
            var found = -1;
            lc_DieuChinh = req.result;
            var ob2 = [];
            if (item.QuanLyTheoLoHang == 1) {
                ob2 = [{
                    ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                    ID_LoHang: item.ID_LoHang,
                    MaHangHoa: item.MaHangHoa,
                    TenLoHang: item.TenLoHang,
                    NgaySanXuat: item.NgaySanXuat,
                    NgayHetHan: item.NgayHetHan,
                    GiaVonHienTai: item.GiaVonHienTai,
                    GiaVonMoi: item.GiaVonMoi,
                    GiaVonTang: item.GiaVonTang,
                    GiaVonGiam: item.GiaVonGiam,
                }]
            }
            var ob1 = {
                ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                MaHangHoa: item.MaHangHoa,
                TenHangHoa: item.TenHangHoa,
                TenDonViTinh: item.TenDonViTinh,
                ThuocTinh_GiaTri: item.ThuocTinh_GiaTri,
                GiaVonHienTai: item.GiaVonHienTai,
                GiaVonMoi: item.GiaVonMoi,
                GiaVonTang: item.GiaVonTang,
                GiaVonGiam: item.GiaVonGiam,
                QuanLyTheoLoHang: item.QuanLyTheoLoHang,
                DM_LoHang: ob2,
                SoThuTu: item.SoThuTu
            }
            if (lc_DieuChinh == null) { // trả về danh sách trống khi cache rỗng
                lc_DieuChinh = [];
            }
            else {
                for (var i = 0; i < lc_DieuChinh.length; i++) {
                    if (lc_DieuChinh[i].MaHangHoa == item.MaHangHoa) {
                        var ob2 = {
                            ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                            ID_LoHang: item.ID_LoHang,
                            MaHangHoa: item.MaHangHoa,
                            TenLoHang: item.TenLoHang,
                            NgaySanXuat: item.NgaySanXuat,
                            NgayHetHan: item.NgayHetHan,
                            GiaVonHienTai: item.GiaVonHienTai,
                            GiaVonMoi: item.GiaVonMoi,
                            GiaVonTang: item.GiaVonTang,
                            GiaVonGiam: item.GiaVonGiam,
                        }
                        lc_DieuChinh[i].DM_LoHang.push(ob2)
                        store.put(lc_DieuChinh[i]);
                        found = i;
                        break;
                    }
                }
            }
            if (found < 0) {
                store.add(ob1); // lưu indexedDB
            }
        }
    }
    self.ExportExcel = async function () {
        if (self.HoaDons().length != 0) {
            var objDiary = {
                ID_NhanVien: _id_NhanVien,
                ID_DonVi: _id_DonVi,
                ChucNang: " Phiếu điều chỉnh",
                NoiDung: "Xuất danh sách phiếu điều chỉnh",
                NoiDungChiTiet: "Xuất danh sách phiếu điều chỉnh",
                LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
            };

            var columnHide = null;
            for (var i = 0; i < self.ColumnsExcel().length; i++) {
                if (i == 0) {
                    columnHide = self.ColumnsExcel()[i];
                }
                else {
                    columnHide = self.ColumnsExcel()[i] + "_" + columnHide;
                }
            }

            const ok = await commonStatisJs.NPOI_ExportExcel(DieuChinhUri + "Export_HoaDonDieuChinh?MaHoaDon=" + _maDC_seach + "&dayStart=" + timeStart + "&dayEnd=" + timeEnd + "&trangthai1=" + _trangthai1 + "&trangthai2=" + _trangthai2 + "&trangthai3=" + _trangthai3 + "&ID_ChiNhanh=" + _tenDonViSeach + "&ColumnsHide=" + columnHide + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh(), 'GET', null, "PhieuDieuChinhGiaVon.xlsx");
            if (ok) {
                Insert_NhatKyThaoTac_1Param(objDiary);
            }
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel!", "danger");
        }
    };
    self.ExportExcel_ChiTiet = async function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Phiếu điều chỉnh",
            NoiDung: "Xuất danh sách chi tiết phiếu điều chỉnh: " + item.MaHoaDon,
            NoiDungChiTiet: "Xuất danh sách chi tiết hàng hóa theo phiếu điều chỉnh: " + item.MaHoaDon,
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var columnHide = null;
        var url = DieuChinhUri + "Export_HoaDonDieuChinh_ChiTiet?ID_HoaDon=" + item.ID_HoaDon + "&ColumnsHide=" + columnHide + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
        const ok = await commonStatisJs.NPOI_ExportExcel(url, 'GET', null, "ChiTietPhieuDieuChinhGiaVon.xlsx");
        if (ok) {
            Insert_NhatKyThaoTac_1Param(objDiary);
        }
    }

    var arrColumnsExcel = [];
    self.addColum = function (item) {
        if (self.ColumnsExcel().length < 1) {
            self.ColumnsExcel.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcel().length; i++) {
                if (self.ColumnsExcel()[i] === item) {
                    self.ColumnsExcel.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcel().length - 1) {
                    self.ColumnsExcel.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel.sort();
    }
   
    self.addNhomHangHoa = function (item) {
        var trans = db.transaction(table, "readwrite"); // đọc và ghi dữ liệu từ indexedDB
        var store = trans.objectStore(table);
        var req = store.getAll();
        req.onsuccess = function (e) {
            var found = -1;
            lc_DieuChinh = req.result;
            var ob2 = [];
            if (item.QuanLyTheoLoHang == 1) {
                ob2 = [{
                    ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                    ID_LoHang: item.ID_LoHang,
                    MaHangHoa: item.MaHangHoa,
                    TenLoHang: item.TenLoHang,
                    NgaySanXuat: item.NgaySanXuat,
                    NgayHetHan: item.NgayHetHan,
                    GiaVonHienTai: item.GiaVonHienTai,
                    GiaVonMoi: item.GiaVonMoi,
                    GiaVonTang: item.GiaVonTang,
                    GiaVonGiam: item.GiaVonGiam,
                }]
            }
            var ob1 = {
                ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                MaHangHoa: item.MaHangHoa,
                TenHangHoa: item.TenHangHoa,
                TenDonViTinh: item.TenDonViTinh,
                ThuocTinh_GiaTri: item.ThuocTinh_GiaTri,
                GiaVonHienTai: item.GiaVonHienTai,
                GiaVonMoi: item.GiaVonMoi,
                GiaVonTang: item.GiaVonTang,
                GiaVonGiam: item.GiaVonGiam,
                QuanLyTheoLoHang: item.QuanLyTheoLoHang,
                DM_LoHang: ob2,
                SoThuTu: item.SoThuTu
            }
            if (lc_DieuChinh == null) { // trả về danh sách trống khi cache rỗng
                lc_DieuChinh = [];
            }
            else {
                for (var i = 0; i < lc_DieuChinh.length; i++) {
                    if (lc_DieuChinh[i].MaHangHoa == item.MaHangHoa) {
                        found = i;
                        break;
                    }
                }
            }
            if (found < 0) {
                lc_DieuChinh.unshift(ob1); // add hàng hóa được chọn vào
                store.add(ob1); // lưu indexedDB
                PX_PageSize = PX_PageSize + 1;
                self.SumRowsHangHoaDieuChinh(PX_PageSize);
                if (PX_PageSize > 10)
                    $('.pg_PhieuDC').show();
                else
                    $('.pg_PhieuDC').hide();
                AllPageHangHoa = PX_PageSize / _pageSizeHangHoa;
                if (AllPageHangHoa > parseInt(AllPageHangHoa)) {
                    AllPageHangHoa = parseInt(AllPageHangHoa) + 1;
                }
                //self.selecPageHangHoa();
            }
            else {
                for (var i = 0; i < lc_DieuChinh.length; i++) {
                    if (lc_DieuChinh[i].MaHangHoa == item.MaHangHoa) {
                        if (item.QuanLyTheoLoHang != 1) {
                            lc_DieuChinh[i].GiaVonHienTai = item.GiaVonHienTai;
                            lc_DieuChinh[i].GiaVonMoi = lc_DieuChinh[i].GiaVonMoi;
                            var chenhlech = parseFloat(lc_DieuChinh[i].GiaVonMoi) - parseFloat(item.GiaVonHienTai);
                            if (chenhlech >= 0) {
                                lc_DieuChinh[i].GiaVonTang = chenhlech;
                                lc_DieuChinh[i].GiaVonGiam = 0;
                            }
                            else {
                                lc_DieuChinh[i].GiaVonTang = 0;
                                lc_DieuChinh[i].GiaVonGiam = chenhlech;
                            }
                        }
                        else {
                            for (var j = 0; j < lc_DieuChinh[i].DM_LoHang.length; j++) {
                                if (lc_DieuChinh[i].DM_LoHang[j].ID_LoHang == item.ID_LoHang) {
                                    if (j == 0) {
                                        lc_DieuChinh[i].GiaVonHienTai = item.GiaVonHienTai;
                                        lc_DieuChinh[i].GiaVonMoi = lc_DieuChinh[i].GiaVonMoi;
                                        var chenhlech = parseFloat(lc_DieuChinh[i].GiaVonMoi) - parseFloat(item.GiaVonHienTai);
                                        if (chenhlech >= 0) {
                                            lc_DieuChinh[i].GiaVonTang = chenhlech;
                                            lc_DieuChinh[i].GiaVonGiam = 0;
                                        }
                                        else {
                                            lc_DieuChinh[i].GiaVonTang = 0;
                                            lc_DieuChinh[i].GiaVonGiam = chenhlech;
                                        }
                                    }
                                    lc_DieuChinh[i].DM_LoHang[j].GiaVonHienTai = item.GiaVonHienTai;
                                    lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi = lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi;
                                    var chenhlech = parseFloat(lc_DieuChinh[i].DM_LoHang[j].GiaVonMoi) - parseFloat(item.GiaVonHienTai);
                                    if (chenhlech >= 0) {
                                        lc_DieuChinh[i].DM_LoHang[j].GiaVonTang = chenhlech;
                                        lc_DieuChinh[i].DM_LoHang[j].GiaVonGiam = 0;
                                    }
                                    else {
                                        lc_DieuChinh[i].DM_LoHang[j].GiaVonTang = 0;
                                        lc_DieuChinh[i].DM_LoHang[j].GiaVonGiam = chenhlech;
                                    }
                                    break;
                                }
                                if (j == lc_DieuChinh[i].DM_LoHang.length - 1) {
                                    var ob2 = {
                                        ID_DonViQuiDoi: item.ID_DonViQuiDoi,
                                        ID_LoHang: item.ID_LoHang,
                                        MaHangHoa: item.MaHangHoa,
                                        TenLoHang: item.TenLoHang,
                                        NgaySanXuat: item.NgaySanXuat,
                                        NgayHetHan: item.NgayHetHan,
                                        GiaVonHienTai: item.GiaVonHienTai,
                                        GiaVonMoi: item.GiaVonMoi,
                                        GiaVonTang: item.GiaVonTang,
                                        GiaVonGiam: item.GiaVonGiam,
                                    }
                                    lc_DieuChinh[i].DM_LoHang.push(ob2)
                                }
                            }
                        }
                        lc_DieuChinh[i].SoThuTu = STT;
                        store.put(lc_DieuChinh[i]);
                        break;
                    }
                }
            }
        }
    }
    
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
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
        LoadingForm(true);
        $('.phieu-dieu-chinh').gridLoader();
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadForm").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadForm").files[i];
            formData.append("imageUploadForm", file);
        }
        $.ajax({
            type: "POST",
            url: DieuChinhUri + "importExcelDieuChinh",
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
                    LoadingForm(false);
                }
                else {
                    $.ajax({
                        type: "POST",
                        url: DieuChinhUri + "getList_DanhSachHangDieuChinh?ID_ChiNhanh=" + _id_DonVi,
                        data: formData,
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (item) {
                            for (var i = 0; i < item.length; i++) {
                                self.addNhomHangHoa(item[i]);
                                //$('#importDieuChinh').hide();();
                            }
                            GetTonKho_byIDQuyDois();
                            self.resetCache();
                            LoadingForm(false);

                            $('.phieu-dieu-chinh').gridLoader({ show: false });
                        },
                        statusCode: {
                            500: function (item) {
                                LoadingForm(false);
                                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Load danh sách hàng điều chỉnh thất bại!", "danger");
                                $('.phieu-dieu-chinh').gridLoader({ show: false });
                            }
                        }
                    });
                }
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                },
                406: function (item) {
                    LoadingForm(false);
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + item.responseJSON.Message, "danger")
                    $('.phieu-dieu-chinh').gridLoader({ show: false });
                },
                500: function (item) {
                    LoadingForm(false);
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import hàng hóa điều chỉnh thất bại!", "danger");
                    $('.phieu-dieu-chinh').gridLoader({ show: false });
                }
            }
        });
    }
    shortcut.add('F3', function () {
        $('#txtAutoHangHoa').focus().select();
    });
    shortcut.add('F10', function (e) {
        if ($('.bgwhite').css('display') === "none") {
            if (window.location.hash !== "#/CouponAdjustment") {
                $('.bgwhite').show();
                self.add_HoanThanh();
            }
        }
        e.stopImmediatePropagation();
    });
    // Phím tắt
    var ttsl = 0;
    var thisDC;
    self.selectedSL = function (item) {
        thisDC = "#GiaVon_" + item.ID_DonViQuiDoi;
        ttsl = $(thisDC).closest('tr').index() + (_pageNumberHangHoa - 1) * 10;
    }
    $(document).keyup(function (e) {
        var code = (e.keyCode ? e.keyCode : e.which);
        if (lc_DieuChinh.length > 0) {
            lc_DieuChinh = sortByKey(lc_DieuChinh, 'SoThuTu');
            lc_DieuChinh = lc_DieuChinh.sort(function (a, b) {
                var x = a.SoThuTu, y = b.SoThuTu;
                return x < y ? 1 : x > y ? -1 : 0;
            });
            if (code === 36) { // home
                if ((_pageNumberHangHoa - 1) === 0) {
                    ttsl = 0;
                    thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                    $(thisDC).focus().select();
                }
                else {
                    ttsl = 10 + (_pageNumberHangHoa - 1) * 10 - 10;
                    thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                    $(thisDC).focus().select();
                }
            }
            if (code === 35) { // End
                if ((_pageNumberHangHoa - 1) === 0) {
                    ttsl = lc_DieuChinh.length >= 10 ? 9 : lc_DieuChinh.length - 1;
                    thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                    $(thisDC).focus().select();
                }
                else {
                    ttsl = self.RowsEnd_HH() - 1;
                    thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                    $(thisDC).focus().select();
                }
            }
            if (code === 13 && ttsl >= 0 && $(thisDC).is(":focus")) {
                ttsl = ttsl + 1 - (_pageNumberHangHoa - 1) * 10;
                if (lc_DieuChinh.length >= 10) {
                    if ((_pageNumberHangHoa - 1) === 0) {
                        if (ttsl < 10) {
                            thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                            $(thisDC).focus().select();
                        }
                        else {
                            ttsl = 0;
                            thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                            $(thisDC).focus().select();
                        }
                    }
                    else {
                        ttsl = (_pageNumberHangHoa - 1) * 10 + ttsl;
                        if (ttsl < self.RowsEnd_HH()) {
                            thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                            $(thisDC).focus().select();
                        }
                        else {
                            ttsl = (_pageNumberHangHoa - 1) * 10;
                            thisDC = "#GiaVon_" + lc_DieuChinh[(_pageNumberHangHoa - 1) * 10].IDID_DonViQuiDoi
                            $(thisDC).focus().select();
                        }
                    }
                }
                if (lc_DieuChinh.length < 10) {
                    if (ttsl < lc_DieuChinh.length) {
                        thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                        $(thisDC).focus().select();
                    }
                    else {
                        ttsl = 0;
                        thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                        $(thisDC).focus().select();
                    }
                }
            }
            if (code === 16 && ttsl >= 0 && $(thisDC).is(":focus")) {
                ttsl = ttsl - 1 - (_pageNumberHangHoa - 1) * 10;
                console.log(ttsl)
                if ((_pageNumberHangHoa - 1) === 0) {
                    if (ttsl >= 0) {
                        thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                        $(thisDC).focus().select();
                    }
                    else {
                        ttsl = lc_DieuChinh.length >= 10 ? 9 : lc_DieuChinh.length - 1;
                        thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                        $(thisDC).focus().select();
                    }
                }
                else {
                    ttsl = (_pageNumberHangHoa - 1) * 10 + ttsl;
                    if (ttsl >= (_pageNumberHangHoa - 1) * 10) {
                        thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                        $(thisDC).focus().select();
                    }
                    else {
                        ttsl = self.RowsEnd_HH() - 1;
                        thisDC = "#GiaVon_" + lc_DieuChinh[ttsl].ID_DonViQuiDoi;
                        $(thisDC).focus().select();
                    }
                }
            }
        }
    })
    // ẩn hiện cột
    $("#cbMaPhieu").click(function () {
        $(".mahang").toggle();
        addClass(".mahang", "cbMaPhieu", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbThoiGian").click(function () {
        $(".thoigian").toggle();
        addClass(".thoigian", "cbThoiGian", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbTenDonVi").click(function () {
        $(".tendonvi ").toggle();
        addClass(".tendonvi", "cbTenDonVi", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbGhiChu").click(function () {
        $(".ghichu").toggle();
        addClass(".ghichu", "cbGhiChu", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbSoHangHoa").click(function () {
        $(".tongchenhlech ").toggle();
        addClass(".tongchenhlech", "cbSoHangHoa", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbGiaVonTang").click(function () {
        $(".sllechtang").toggle();
        addClass(".sllechtang", "cbGiaVonTang", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbGiaVonGiam").click(function () {
        $(".sllechgiam").toggle();
        addClass(".sllechgiam", "cbGiaVonGiam", $(this).val());
        self.addColum($(this).val())
    });
    $("#cbTrangThai").click(function () {
        $(".trangthai ").toggle();
        addClass(".trangthai", "cbTrangThai", $(this).val());
        self.addColum($(this).val())
    });
    function addClass(name, id, value) {
        var current = localStorage.getItem('DieuChinh');
        if (!current) {
            current = [];
        } else {
            current = JSON.parse(current);
        }
        if (current.length > 0) {
            for (var i = 0; i < current.length; i++) {
                if (current[i].NameId === id.toString()) {
                    current.splice(i, 1);
                    break;
                }
                if (i == current.length - 1) {
                    current.push({
                        NameClass: name,
                        NameId: id,
                        Value: value
                    });
                    break;
                }
            }
        }
        else {
            current.push({
                NameClass: name,
                NameId: id,
                Value: value
            });
        }
        localStorage.setItem('DieuChinh', JSON.stringify(current));
    }
    self.addColum = function (item) {
        if (self.ColumnsExcel().length < 1) {
            self.ColumnsExcel.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcel().length; i++) {
                if (self.ColumnsExcel()[i] === item) {
                    self.ColumnsExcel.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcel().length - 1) {
                    self.ColumnsExcel.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcel.sort();
    }
}
var vmDieuChinh = new ViewModal();
ko.applyBindings(vmDieuChinh);
function hidewait_TR(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
function keypressEnterSelected(e) {
    if (e.keyCode == 13) {
        vmDieuChinh.SelectedHHEnterkey();
    }
}

function itemSelected() {
    vmDieuChinh.SelectedHHEnterkey();
}

function itemSelected_LoHang(item) {
    vmDieuChinh.SelectedHHEnterkey_LoHang(item);
}


var arrID_NhomHang = [];
function SetCheckAllChilds(obj) {
    var thisID = $(obj).attr('id');
    $(obj).removeClass('squarevt');
    if ($(obj).is(':checked')) {// kiểm tra đã check để lựa chọn nhóm hàng
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhomHang) > -1)) {
            arrID_NhomHang.push(thisID);
        }
    }
    else {
        // bỏ chọn nhóm hàng
        $.map(arrID_NhomHang, function (item, i) {
            if (item === thisID) {
                arrID_NhomHang.splice(i, 1);
            }
        })
    }
    // checked checkbox con
    var isChecked = $(obj).is(":checked");
    $(obj).parents("li").find('.checkall input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $(obj).parents("li").find('.checkall input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhomHang) > -1)) {
                arrID_NhomHang.push(thisID);
            }
        })
    }
    else {
        $(obj).parents("li").find('.checkall input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && (jQuery.inArray(thisID, arrID_NhomHang) > -1)) {
                $.map(arrID_NhomHang, function (item, i) {
                    if (item === thisID) {
                        arrID_NhomHang.splice(i, 1);
                    }
                })
            }
        })
    }
}
function SetCheckAllChild2s(obj) {
    var count = 0;
    var countcheck = 0;
    var thisID = $(obj).attr('id');
    var closestUl = $(obj).closest('ul').prev().prev().find('input'); //lấy id cấp trên
    closestUl.addClass('squarevt');
    var NhomHangcheck = closestUl.attr('id');
    $(obj).closest('ul').find('li').each(function () {
        if ($(this).find('input').is(':checked')) {
            countcheck = countcheck + 1;
        }
        count += 1;
    })
    if (count === countcheck) {
        closestUl.removeClass('squarevt');
        closestUl.prop('checked', true);
        if ($.inArray(NhomHangcheck, arrID_NhomHang) === -1) {
            arrID_NhomHang.push(NhomHangcheck);
        }
    }
    if (countcheck === 0) {
        closestUl.removeClass('squarevt');
        closestUl.prop('checked', false);
        $.map(arrID_NhomHang, function (item, i) {
            if (item === NhomHangcheck) {
                arrID_NhomHang.splice(i, 1);
            }
        })
    }
    if (count > countcheck && countcheck !== 0) {
        if ($.inArray(NhomHangcheck, arrID_NhomHang) === -1) {
            arrID_NhomHang.push(NhomHangcheck);
        }
    }
    if ($(obj).is(':checked')) {
        if (thisID !== undefined && !(jQuery.inArray(thisID, arrID_NhomHang) > -1)) {
            arrID_NhomHang.push(thisID);
        }
    }
    else {

        //remove item in arrID_NhomHang
        $.map(arrID_NhomHang, function (item, i) {
            if (item === thisID) {
                arrID_NhomHang.splice(i, 1);
            }
        })
    }
}