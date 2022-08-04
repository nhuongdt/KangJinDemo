var ViewModal = function () {
    var self = this;
    //var _rdTime = 1;
    var tab_TonKho = 1;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    //var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.MoiQuanTam = ko.observable('Báo cáo hàng hóa tồn kho');
    self.TenChiNhanh = ko.observable($('#_txtTenDonVi').text());
    var TenChiNhanh_tab = $('#_txtTenDonVi').text();
    self.LoaiBaoCao = ko.observable('hàng hóa');
    self.TenNVPrint = ko.observable();
    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray([_id_DonVi]);
    var dt1 = new Date();
    var _timeStart = moment(dt1).format('YYYY-MM-DD')
    var _timeEnd = moment(dt1).add(1, 'days').format('YYYY-MM-DD');
    var _tonkhoEnd = _timeEnd;
    var _tonkhoStart = _timeStart;
    self.TodayBC = ko.observable('Thời gian: ' + moment(_timeStart).format('DD/MM/YYYY'));
    self.TodayBC_TK = ko.observable(moment(_tonkhoStart).format('DD/MM/YYYY'));
    var _idChungTuSeach = null;
    //var _selectTabtk = 1;
    var dk_tabtk = 1;
    var dk_tabxk = 1;
    self.check_MoiQuanTam = ko.observable(1);
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.SumNumberPageReport_CT = ko.observableArray();
    self.RowsStart_CT = ko.observable('1');
    self.RowsEnd_CT = ko.observable('10');
    self.NhomHangHoas = ko.observableArray();
    self.Select_Table = ko.observable('1');
    var _pageNumber = 1;
    self.SumRowsHangHoa = ko.observable();
    self.SumRowsHangHoa_CT = ko.observable();
    self.pageSize = ko.observable(10);
    self.pageSizes = [10, 20, 30, 40, 50];

    var AllPage;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    self.pageNumber_TK = ko.observable(1);
    self.pageNumber_TK_TH = ko.observable(1);
    self.pageNumber_NXT = ko.observable(1);
    self.pageNumber_NXTCT = ko.observable(1);
    self.pageNumber_TQXH = ko.observable(1);
    self.pageNumber_CTDC = ko.observable(1);
    self.pageNumber_TQNH = ko.observable(1);
    self.pageNumber_THNHH = ko.observable(1);
    self.pageNumber_THNGD = ko.observable(1);
    self.pageNumber_THXHH = ko.observable(1);
    self.pageNumber_THXGD = ko.observable(1);
    self.pageNumber_XDVDL = ko.observable(1);
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    $('.ip_TimeReport').val("Hôm nay");
    self.Loc_TinhTrangKD = ko.observable('2');
    self.Loc_TrangThai = ko.observable('1');
    self.Loc_TonKho = ko.observable('0');

    var tk = null;
    // TuanDL Cache Show Hide Column Grid
    self.listCheckbox = ko.observableArray();
    self.columnCheckType = ko.observable(1);
    var Key_Form = 'Key_ReportWarehouse';

    self.isGara = ko.observable(VHeader.IdNganhNgheKinhDoanh.toUpperCase() === 'C16EDDA0-F6D0-43E1-A469-844FAB143014');
    console.log('bckho')

    $('#dieuchuyenhanghoa .nav-tabs ').on('click', 'li', function () {
        if (self.Select_Table() !== '1') {
            $('#dieuchuyenhanghoa .radio-button-leff li').each(function (i) {
                if ($(this).data('id') !== null && $(this).data('id') !== undefined)
                    self.columnCheckType($(this).data('id'));
            });
        }
        else {
            self.columnCheckType($(this).data('id'));
        }
        GetListCheckBox();
    });
    $('#tonghophangnhapkho  .nav-tabs ').on('click', 'li', function () {
        self.columnCheckType($(this).data('id'));
        GetListCheckBox();
    });
    $('#tonghophangxuatkho  .nav-tabs ').on('click', 'li', function () {
        self.columnCheckType($(this).data('id'));
        GetListCheckBox();
    });
    $('#home  .nav-tabs ').on('click', 'li', function () {
        self.columnCheckType($(this).data('id'));
        GetListCheckBox();
    });
    function LoadFirstTabs(type) {
        if (type === parseInt($('#ID_dieuchuyenhh').val())) {
            $('#dieuchuyenhanghoa .tab-content .tab-pane').each(function (i) {
                if ($(this).hasClass('active'))
                    self.columnCheckType($(this).data('id'));
            });
        }
        else if (type === parseInt($('#ID_thhangnhapkho').val())) {
            $('#tonghophangnhapkho .tab-content .tab-pane').each(function (i) {
                if ($(this).hasClass('active'))
                    self.columnCheckType($(this).data('id'));
            });
        }
        else if (type === parseInt($('#ID_thhangxuatkho').val())) {
            $('#tonghophangxuatkho .tab-content .tab-pane').each(function (i) {
                if ($(this).hasClass('active'))
                    self.columnCheckType($(this).data('id'));
            });
        }
        else if (type === parseInt($('#ID_tonghop').val())) {
            $('#home .tab-content .tab-pane').each(function (i) {
                if ($(this).hasClass('active'))
                    self.columnCheckType($(this).data('id'));
            });
        }
        else if (type === parseInt($('#ID_xuatkhohhtheodinhluong').val())) {
            $('#table_dichvudinhluong .tab-content .tab-pane').each(function (i) {
                if ($(this).hasClass('active'))
                    self.columnCheckType($(this).data('id'));
            });
        }
        GetListCheckBox();
    }
    var IsLoad = true;

    function addcacheFirst() {
        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#ID_tonghop').val(), [
            {
                NameClass: "Warehouse_soluongquycach",
                Value: 6
            }
        ]);
        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#ID_nhapxuatton').val(), [{
            NameClass: "Warehouse_soluongquycach",
            Value: 12
        }]);
    }
    self.QuanLyTheoLo = ko.observable(false);
    function loadHtmlGrid() {
        var KeyLo = Key_Form + self.columnCheckType() + "_LOHANG";
        if (IsLoad === true) {
            $.getJSON("api/DanhMuc/ThietLapApi/CheckQuanLyLo", function (data) {
                self.QuanLyTheoLo(data);
                var current = localStorage.getItem(KeyLo);
                if (data.toString() !== current) {
                    localStorage.removeItem(Key_Form + self.columnCheckType());
                    addcacheFirst();
                    if (data.toString() === "false") {
                        $('#select-column .dropdown-list ul li').each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("_lohang") >= 0) {
                                LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
                            }
                        });
                    }
                    LocalCaches.RemoveLoHang(Key_Form);
                    localStorage.setItem(LocalCaches.KeyQuanLyLo, data);
                    localStorage.setItem(KeyLo, data);
                }
                else {
                    addcacheFirst();
                }
                var key = Key_Form + self.columnCheckType();
                if (parseInt($('#ID_dieuchuyenchitiet').val()) === self.columnCheckType()) {
                    $('#dieuchuyenhanghoa .nav-tabs li').each(function () {
                        if ($(this).hasClass('active')) {
                            key = key + '_' + $(this).val();
                        }
                    });
                }
                LocalCaches.LoadFirstColumnGrid(key, $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
                Rowspandichvudinhluong();
                setrowpandinhluong();
                //$('.table-reponsive').css('display', 'block');
                IsLoad = false;

            });
        }
        else {
            var current = localStorage.getItem(LocalCaches.KeyQuanLyLo);
            var page = localStorage.getItem(KeyLo);
            if (!current) {
                IsLoadFirst = true;
            }
            else {
                if (!page || page.toString() !== current) {
                    localStorage.removeItem(Key_Form + self.columnCheckType());
                    addcacheFirst();
                    if (current === "false") {
                        $('#select-column .dropdown-list ul li').each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("_lohang") >= 0) {
                                LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
                            }
                        });
                    }
                    localStorage.setItem(KeyLo, current);
                }
            }

            addcacheFirst();
            var key = Key_Form + self.columnCheckType();
            if (parseInt($('#ID_dieuchuyenchitiet').val()) === self.columnCheckType()) {
                $('#dieuchuyenhanghoa .nav-tabs li').each(function () {
                    if ($(this).hasClass('active')) {
                        key = key + '_' + $(this).val();
                    }
                });
            }
            LocalCaches.LoadFirstColumnGrid(key, $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
            Rowspandichvudinhluong();
            setrowpandinhluong();
            //$('.table-reponsive').css('display', 'block');
            IsLoad = false;
        }

    }
    function GetListCheckBox() {
        $.getJSON("api/DanhMuc/ReportAPI/GetChecked?type=" + self.columnCheckType() + "&group=" + $('#ID_loaibaocao').val(), function (data) {
            if (data === null) {
                data = [];
            }
            if (!self.isGara()) {
                switch (parseInt(self.columnCheckType())) {
                    case 42:// bc xuatkho - theo dinhluong
                        data = $.grep(data, function (x) {
                            return $.inArray(x.Key, ['maphieutiepnhan', 'biensoxe']) === -1;
                        });
                        break;
                    case 26:// bc xuatkho - chi tiet
                        data = $.grep(data, function (x) {
                            return $.inArray(x.Key, ['bienso']) === -1;
                        });
                        break;
                }
            }
            self.listCheckbox(data);
            loadHtmlGrid();
        });
    }
    function loadCheckbox(type, Ischeck = true) {
        self.columnCheckType(type);
        if (Ischeck) {
            LoadFirstTabs(type);
            $('.chose_kieubang li').each(function (i) {
                if (self.columnCheckType() === $(this).data('id')) {
                    if (!$(this).hasClass("active"))
                        $(this).addClass("active");
                }
                else {
                    $(this).removeClass("active");
                }
            });
            $('.tab-content .tabmenu').each(function (i) {
                if (self.columnCheckType() === $(this).data('id')) {
                    if (!$(this).hasClass("active"))
                        $(this).addClass("active");
                }
                else {
                    $(this).removeClass("active");
                }
            });
        }
        else {
            GetListCheckBox();
        }

    }
    loadCheckbox(1);
    function checkedCheckBoxColumn(valueCheck, index) {
        var key = Key_Form + self.columnCheckType();
        if (parseInt($('#ID_dieuchuyenchitiet').val()) === self.columnCheckType()) {
            $('#dieuchuyenhanghoa .nav-tabs li').each(function () {
                if ($(this).hasClass('active')) {
                    key = key + '_' + $(this).val();
                }
            });
        }
        LocalCaches.AddColumnHidenGrid(key, valueCheck, index);
        $('.' + valueCheck).toggle();
        setrowpandinhluong();
    }
    $('#select-column').on('change', '.dropdown-list ul li input[type = checkbox]', function () {
        var index = $(this).closest('li').index();
        if ($(this).closest('li').closest('ul').closest('div').hasClass('dropdown-right')) {
            index += $('#select-column .dropdown-list .dropdown-left ul li').length;
        }
        checkedCheckBoxColumn($(this).val(), index);

    });
    $('#select-column').on('click', '.dropdown-list ul li', function (i, e) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var index = $(this).index();
        if ($(this).closest('ul').closest('div').hasClass('dropdown-right')) {
            index += $('#select-column .dropdown-list .dropdown-left ul li').length;
        }
        checkedCheckBoxColumn($(this).find('input[type = checkbox]').val(), index);

    });
    function setrowpandinhluong() {
        if (self.columnCheckType() === parseInt($('#ID_xuatkhohhtheodinhluong').val())) {
            var colspan = 0;

            if (self.isGara()) {
                $('#select-column .dropdown-list ul li input[type = checkbox]').each(function (i) {
                    if ($(this).is(':checked')) {
                        colspan = colspan + 1;
                    }
                    switch (i) {
                        case 9:
                            $('#_dinhluongchungtu').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 19:
                            $('#_dinhluongdichvu').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 26:
                            $('#_dinhluonghanghoa').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 28:
                            $('#_dinhluongthucte').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 31:
                            $('#_dinhluongchenhlech').attr('colspan', colspan);
                            colspan = 0;
                            break;
                    }
                });
            }
            else {
                $('#select-column .dropdown-list ul li input[type = checkbox]').each(function (i) {
                    if ($(this).is(':checked')) {
                        colspan = colspan + 1;
                    }

                    switch (i) {
                        case 7:
                            $('#_dinhluongchungtu').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 17:
                            $('#_dinhluongdichvu').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 24:
                            $('#_dinhluonghanghoa').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 26:
                            $('#_dinhluongthucte').attr('colspan', colspan);
                            colspan = 0;
                            break;
                        case 29:
                            $('#_dinhluongchenhlech').attr('colspan', colspan);
                            colspan = 0;
                            break;
                    }
                });
            }
        }
    }
    function Rowspandichvudinhluong() {
        if (self.columnCheckType() === parseInt($('#ID_xuatkhohhtheodinhluong').val())) {
            var dataid = '';
            var list = $('#table_dichvudinhluong table tbody tr').map(function () {
                return $(this).data('id');
            });
            $('#table_dichvudinhluong table tbody tr').each(function (i) {
                if ($(this).data('id') !== dataid) {
                    dataid = $(this).data('id');
                    var count = list.toArray().filter(o => o === dataid).length;
                    count = count === 0 ? 1 : count;
                    $(this).find('td').each(function (i) {
                        if ($(this).hasClass('vertical-center')) {
                            $(this).attr('rowspan', count);
                        }
                    });

                }
                else {
                    $(this).find('td').each(function (i) {
                        if ($(this).hasClass('vertical-center')) {
                            $(this).remove();
                        }
                    });
                }
            });
        }
    }
    //trinhpv phân quyền
    self.BaoCaoBanHang = ko.observable();
    self.BCK_TonKho = ko.observable();
    self.BCK_NhapXuatTon = ko.observable();
    self.BCK_NhapXuatTonChiTiet = ko.observable();
    self.BCK_DieuChuyenHangHoa = ko.observable();
    self.BCK_TH_HangNhapKho = ko.observable();
    self.BCK_TH_HangXuatKho = ko.observable();
    self.BCK_XuatFile = ko.observable();
    self.shouldTrangThai = ko.observable(false);
    self.shouldTonKho = ko.observable(true);
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    function getQuyen_NguoiDung() {
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCK_TonKho", "GET").done(function (data) {
        //    self.BCK_TonKho(data);
        //    getDonVi();
        //});
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCK_NhapXuatTon", "GET").done(function (data) {
        //    self.BCK_NhapXuatTon(data);
        //});
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCK_NhapXuatTonChiTiet", "GET").done(function (data) {
        //    self.BCK_NhapXuatTonChiTiet(data);
        //});
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCK_DieuChuyenHangHoa", "GET").done(function (data) {
        //    self.BCK_DieuChuyenHangHoa(data);
        //});
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCK_TH_HangNhapKho", "GET").done(function (data) {
        //    self.BCK_TH_HangNhapKho(data);
        //});
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCK_TH_HangXuatKho", "GET").done(function (data) {
        //    self.BCK_TH_HangXuatKho(data);
        //});
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCK_XuatFile", "GET").done(function (data) {
        //    self.BCK_XuatFile(data);
        //});

        if (VHeader.Quyen.indexOf('BCK_TonKho') > -1) {
            self.BCK_TonKho('BCK_TonKho');
            getDonVi();
        }
        else {
            self.BCK_TonKho('false');
        }

        if (VHeader.Quyen.indexOf('BCK_NhapXuatTon') > -1) {
            self.BCK_NhapXuatTon('BCK_NhapXuatTon');
        }
        else {
            self.BCK_NhapXuatTon('false');
        }

        if (VHeader.Quyen.indexOf('BCK_NhapXuatTonChiTiet') > -1) {
            self.BCK_NhapXuatTonChiTiet('BCK_NhapXuatTonChiTiet');
        }
        else {
            self.BCK_NhapXuatTonChiTiet('false');
        }

        if (VHeader.Quyen.indexOf('BCK_DieuChuyenHangHoa') > -1) {
            self.BCK_DieuChuyenHangHoa('BCK_DieuChuyenHangHoa');
        }
        else {
            self.BCK_DieuChuyenHangHoa('false');
        }

        if (VHeader.Quyen.indexOf('BCK_TH_HangNhapKho') > -1) {
            self.BCK_TH_HangNhapKho('BCK_TH_HangNhapKho');
        }
        else {
            self.BCK_TH_HangNhapKho('false');
        }

        if (VHeader.Quyen.indexOf('BCK_TH_HangXuatKho') > -1) {
            self.BCK_TH_HangXuatKho('BCK_TH_HangXuatKho');
        }
        else {
            self.BCK_TH_HangXuatKho('false');
        }

        if (VHeader.Quyen.indexOf('BCK_XuatFile') > -1) {
            self.BCK_XuatFile('BCK_XuatFile');
        }
        else {
            self.BCK_XuatFile('false');
        }
    }
    getQuyen_NguoiDung();
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameDonViSeach, "GET").done(function (data) {
            self.DonVis(data);
            self.searchDonVi(data);
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            for (let i = 0; i < self.DonVis().length; i++) {
                if (self.DonVis()[i].ID === _idDonViSeach) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                    self.SelectedDonVi(self.DonVis()[i]);
                }
            }
            self.ArrDonVi(self.DonVis());
            self.LoadReport();
        });
    }
    var TenChiNhanh;
    //Lua chon don vi
    self.CloseDonVi = function (item) {
        _idDonViSeach = null;
        var arrID = [];
        self.MangChiNhanh.remove(item);

        if (item.ID === '00000000-0000-0000-0000-0000-000000000000') {
            arrID = $.map(self.DonVis(), function (x) {
                return x.ID;
            });
            TenChiNhanh = 'Tất cả chi nhánh';
        }
        else {
            self.ArrDonVi.unshift(item);
            if (self.MangChiNhanh().length === 0) {
                $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
                TenChiNhanh = 'Tất cả chi nhánh.';
                for (let i = 0; i < self.searchDonVi().length; i++) {
                    if (_idDonViSeach === null)
                        _idDonViSeach = self.searchDonVi()[i].ID;
                    else
                        _idDonViSeach = self.searchDonVi()[i].ID + "," + _idDonViSeach;
                }
                arrID = $.map(self.DonVis(), function (x) {
                    return x.ID;
                });
            }
            else {
                for (let i = 0; i < self.MangChiNhanh().length; i++) {
                    if (_idDonViSeach === null) {
                        _idDonViSeach = self.MangChiNhanh()[i].ID;
                        TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                    }
                    else {
                        _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
                        TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                    }
                }
                arrID = $.map(self.MangChiNhanh(), function (x) {
                    return x.ID;
                });
            }
        }
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        self.TenChiNhanh(TenChiNhanh);
        self.LstIDDonVi(arrID);
        self.LoadReport();
    };

    self.SelectedDonVi = function (item, event) {
        if (event !== undefined) {
            event.stopPropagation();
        }
        _idDonViSeach = null;
        var arrIDDonVi = [];
        if (item.ID === undefined) {
            let arrID = $.map(self.DonVis(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);
            arrIDDonVi = self.ArrDonVi().map(function (x) {
                return x.ID;
            });
            // push again lstDV has chosed
            for (let i = 0; i < self.MangChiNhanh().length; i++) {
                if ($.inArray(self.MangChiNhanh()[i].ID, arrIDDonVi) === -1 && self.MangChiNhanh()[i].ID !== '00000000-0000-0000-0000-0000-000000000000') {
                    self.ArrDonVi().unshift(self.MangChiNhanh()[i]);
                }
            }
            self.MangChiNhanh([{
                ID: '00000000-0000-0000-0000-0000-000000000000', TenDonVi: 'Tất cả chi nhánh'
            }]);
            TenChiNhanh = 'Tất cả chi nhánh';
        }
        else {
            for (let i = 0; i < self.MangChiNhanh().length; i++) {
                if ($.inArray(self.MangChiNhanh()[i].ID, arrIDDonVi) === -1) {
                    arrIDDonVi.push(self.MangChiNhanh()[i].ID);
                }
                // if chosed other donvi --> remove item All
                if (self.MangChiNhanh()[i].ID === '00000000-0000-0000-0000-0000-000000000000') {
                    self.MangChiNhanh().splice(i, 1);
                }
            }
            if ($.inArray(item.ID, arrIDDonVi) === -1) {
                self.MangChiNhanh.push(item);
                $('#NoteNameDonVi').removeAttr('placeholder');
                for (let i = 0; i < self.MangChiNhanh().length; i++) {
                    if (_idDonViSeach === null) {
                        _idDonViSeach = self.MangChiNhanh()[i].ID;
                        TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                    }
                    else {
                        _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
                        TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                    }
                }
                self.TenChiNhanh(TenChiNhanh);
            }
            //thêm dấu check vào đối tượng được chọn
            $('#selec-all-DonVi li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>');
                }
            });

            let arrID = $.map(self.MangChiNhanh(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);
        }
        self.TenChiNhanh(TenChiNhanh);
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        if (event !== undefined) {
            event.preventDefault();
        }
        return false;
    };
    //lọc đơn vị
    self.NoteNameDonVi = function () {
        var arrDonVi = [];
        var itemSearch = locdau($('#NoteNameDonVi').val().toLowerCase());
        for (let i = 0; i < self.searchDonVi().length; i++) {
            var locdauInput = locdau(self.searchDonVi()[i].TenDonVi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrDonVi.push(self.searchDonVi()[i]);
            }
        }
        self.DonVis(arrDonVi);
        if ($('#NoteNameDonVi').val() === "") {
            self.DonVis(self.searchDonVi());
        }
    };
    $('#NoteNameDonVi').keypress(function (e) {
        if (e.keyCode === 13 && self.DonVis().length > 0) {
            self.SelectedDonVi(self.DonVis()[0]);
        }
    });

    self.role_NhapKhoNoiBo = ko.observable(VHeader.Quyen.indexOf('NhapKhoNoiBo') > -1);
    self.role_NhapHangKhachThua = ko.observable(VHeader.Quyen.indexOf('NhapHangKhachThua') > -1);

    //load Loai Chung Tu
    self.MangChungTu = ko.observableArray();
    self.ChungTus = ko.observableArray();
    self.searchChungTu = ko.observableArray();
    self.AllLoaiChungTu = ko.observableArray(
        [
            { ID: 1, TenChungTu: 'Hóa đơn bán lẻ' },
            { ID: 7, TenChungTu: 'Trả hàng nhà cung cấp' },
            { ID: 8, TenChungTu: 'Xuất kho' },
            { ID: 9, TenChungTu: 'Phiếu kiểm kê' },
            { ID: 10, TenChungTu: 'Chuyển hàng' },
            { ID: 2, TenChungTu: 'Xuất sử dụng gói dịch vụ' },
            { ID: 3, TenChungTu: 'Xuất bán dịch vụ định lượng' },
            { ID: 12, TenChungTu: 'Xuất bảo hành' },
            { ID: 11, TenChungTu: 'Xuất sửa chữa' },
            { ID: 4, TenChungTu: 'Phiếu nhập kho' },
            { ID: 6, TenChungTu: 'Trả hàng' },
            { ID: 9, TenChungTu: 'Phiếu kiểm kê' },
        ]);

    if (self.role_NhapKhoNoiBo()) {
        self.AllLoaiChungTu.push({ ID: 13, TenChungTu: 'Nhập kho nội bộ' });
    }

    if (self.role_NhapHangKhachThua()) {
        self.AllLoaiChungTu.push({ ID: 14, TenChungTu: 'Nhập hàng khách thừa' });
    }

    self.getListDM_LoaiChungTuNhapKho = function (item) {
        self.MangChungTu([]);
        _idChungTuSeach = '4,6,9,10,13,14';
        let arr = $.grep(self.AllLoaiChungTu(), function (x) {
            return $.inArray(x.ID, [4, 6, 9, 10, 13, 14]) > -1;
        })
        self.ChungTus(arr);
        self.searchChungTu(arr);
    };
    self.getListDM_LoaiChungTuXuatKho = function (item) {
        self.MangChungTu([]);
        _idChungTuSeach = '1,2,3,7,8,9,10,12';// 12.xuat baohanh
        let arr = $.grep(self.AllLoaiChungTu(), function (x) {
            return $.inArray(x.ID, [1, 2, 3, 7, 8, 9, 10, 12]) > -1;
        })
        self.ChungTus(arr);
        if (self.isGara()) {
            _idChungTuSeach += ',11';
            self.ChungTus.push({ ID: 11, TenChungTu: "Xuất sửa chữa" });// id= 11 --> tránh bị trùng với loại chứng từ khác
        }
        self.searchChungTu(arr);
    };
    self.CloseChungTu = function (item) {
        _idChungTuSeach = null;
        self.MangChungTu.remove(item);
        for (let i = 0; i < self.MangChungTu().length; i++) {
            if (_idChungTuSeach === null) {
                _idChungTuSeach = self.MangChungTu()[i].ID;
            }
            else {
                _idChungTuSeach = self.MangChungTu()[i].ID + "," + _idChungTuSeach;
            }
        }
        if (self.MangChungTu().length === 0) {
            $("#NoteNameChungTu").attr("placeholder", "Chọn chứng từ...");
            for (let i = 0; i < self.searchChungTu().length; i++) {
                if (_idChungTuSeach === null)
                    _idChungTuSeach = self.searchChungTu()[i].ID;
                else
                    _idChungTuSeach = self.searchChungTu()[i].ID + "," + _idChungTuSeach;
            }
        }
        $('#selec-all-ChungTu li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    };
    self.SelectedChungTu = function (item) {
        _idChungTuSeach = null;
        var arrIDChungTu = [];
        for (let i = 0; i < self.MangChungTu().length; i++) {
            if ($.inArray(self.MangChungTu()[i], arrIDChungTu) === -1) {
                arrIDChungTu.push(self.MangChungTu()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDChungTu) === -1) {
            self.MangChungTu.push(item);
            $('#NoteNameChungTu').removeAttr('placeholder');
            for (let i = 0; i < self.MangChungTu().length; i++) {
                if (_idChungTuSeach === null) {
                    _idChungTuSeach = self.MangChungTu()[i].ID;
                }
                else {
                    _idChungTuSeach = self.MangChungTu()[i].ID + "," + _idChungTuSeach;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-ChungTu li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>');
            }
        });

    };
    self.NoteNameChungTu = function () {
        var arrChungTu = [];
        var itemSearch = locdau($('#NoteNameChungTu').val().toLowerCase());
        for (let i = 0; i < self.searchChungTu().length; i++) {
            var locdauInput = locdau(self.searchChungTu()[i].TenChungTu).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrChungTu.push(self.searchChungTu()[i]);
            }
        }
        self.ChungTus(arrChungTu);
        if ($('#NoteNameChungTu').val() === "") {
            self.ChungTus(self.searchChungTu());
        }
    };
    $('#NoteNameChungTu').keypress(function (e) {
        if (e.keyCode === 13 && self.ChungTus().length > 0) {
            self.SelectedChungTu(self.ChungTus()[0]);
        }
    });
    //nhóm hàng
    function GetAllNhomHH() {
        ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
            for (let i = 0; i < data.length; i++) {
                if (data[i].ID_Parent === null) {
                    var objParent = {
                        ID: data[i].ID,
                        TenNhomHangHoa: data[i].TenNhomHang,
                        Childs: []
                    };
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
                                        ID_Parent: data[j].ID
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
            //var objParentMD = {
            //    ID: '00000000-0000-0000-0000-000000000000',
            //    TenNhomHangHoa: 'Nhóm mặc định',
            //    Childs: [],
            //}
            //self.NhomHangHoas.unshift(objParentMD);
            if (self.NhomHangHoas().length > 10) {
                $('.close-goods').css('display', 'block');
            }
        });
    }
    GetAllNhomHH();
    var time = null;
    self.NoteNhomHang = function () {
        clearTimeout(time);
        time = setTimeout(
            function () {
                self.NhomHangHoas([]);
                tk = $('#SeachNhomHang').val();
                if (tk.trim() !== '') {
                    ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
                        for (let i = 0; i < data.length; i++) {
                            if (data[i].ID_Parent === null) {
                                var objParent = {
                                    ID: data[i].ID,
                                    TenNhomHangHoa: data[i].TenNhomHang,
                                    Childs: []
                                };
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
                                                    ID_Parent: data[j].ID
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
                }
                else {
                    GetAllNhomHH();
                }
            }, 300);
    };
    self.SelectRepoert_NhomHangHoa = function (item) {
        _ID_NhomHang = item.ID;
        _pageNumber = 1;
        if (item.ID === undefined) {
            $('.li-oo').removeClass("yellow");
            $('#tatcanhh a').css("display", "block");
            $('#tatcanhh').addClass("yellow");
        }
        else {
            $('.ss-li .li-oo').removeClass("yellow");
            $('#tatcanhh').removeClass("yellow");
            $('.li-pp').removeClass("yellow");
            $('#tatcanhh a').css("display", "none");
            $('#' + item.ID).addClass("yellow");
        }
        self.LoadReport();
    };
    $('.SelectALLNhomHang').on('click', function () {
        _ID_NhomHang = null;
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.chose_TinhTrangKD input').on('click', function () {
        TinhTrangHH = $(this).val();
        _pageNumber = 1;
        self.Loc_TinhTrangKD($(this).val());
        self.LoadReport();
    });
    $('.chose_TrangThai input').on('click', function () {
        _trangThai = $(this).val();
        _pageNumber = 1;
        self.Loc_TrangThai($(this).val());
        self.LoadReport();
    });
    $('.chose_TonKho input').on('click', function () {
        _tonKho = $(this).val();
        _pageNumber = 1;
        self.Loc_TonKho($(this).val());
        self.LoadReport();
    });
    self.txt = ko.observable();
    $('.chose_kieubang').on('click', 'li', function () {
        _id_DonVi = $('#hd_IDdDonVi').val();
        self.txt($("#txt_search").val());
        $('.showDateRange').show();
        $('#showTime').show();
        $('.showDate').hide();
        $(".showChungTu").hide();
        $('#btnExport').show();
        $('#select-column').show();

        if ($(this).data('id') === 1) {
            self.shouldTonKho(true);
            self.shouldTrangThai(false);
        }
        else if ($(this).data('id') === 17 || $(this).data('id') === 18 || $(this).data('id') === 19) {
            self.shouldTrangThai(true); self.shouldTonKho(false);
        }
        else {
            self.shouldTrangThai(false); self.shouldTonKho(false);
        }
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        let valNew = parseInt($(this).find('a input').val());
        let valOld = self.check_MoiQuanTam();
        self.check_MoiQuanTam(valNew);

        switch (valNew) {
            case 1:
                $('.showDateRange').hide();
                $('.showDate').show();
                $('#showTime').hide();
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                if (tab_TonKho === 1) {
                    $('#table_TonKho').addClass('active');
                    $('#table_TongHop').removeClass('active');
                    self.LoaiBaoCao('hàng hóa');
                    self.MoiQuanTam('Báo cáo hàng hóa tồn kho');
                }
                else {
                    $('#btnExport').hide();
                    $('#select-column').hide();
                    $('#table_TonKho').removeClass('active');
                    $('#table_TongHop').addClass('active');
                    self.TenChiNhanh(TenChiNhanh);
                    self.LoaiBaoCao('chi nhánh');
                    self.MoiQuanTam('Báo cáo hàng hóa tồn kho theo chi nhánh');
                }
                break;
            case 2:
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                self.LoaiBaoCao('hàng hóa');
                self.MoiQuanTam('Báo cáo hàng hóa nhập xuất tồn');
                break;
            case 3:
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                self.LoaiBaoCao('hàng hóa');
                self.MoiQuanTam('Báo cáo hàng hóa nhập xuất tồn chi tiết');
                break;
            case 4:
                if (dk_tab === 1) {
                    if (_selectTab === 1) {
                        self.LoaiBaoCao('hàng hóa');
                        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                    }
                    else {
                        self.LoaiBaoCao('hàng hóa chi tiết');
                        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng, mã hóa đơn").blur();
                    }
                }
                else {
                    if (_selectTab === 1) {
                        self.LoaiBaoCao('hàng hóa');
                        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                    }
                    else {
                        self.LoaiBaoCao('hàng hóa chi tiết');
                        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng, mã hóa đơn").blur();
                    }
                }
                self.MoiQuanTam('Báo cáo điều chuyển hàng hóa');
                self.TenChiNhanh(TenChiNhanh);
                break;
            case 5:
                $(".showChungTu").show();
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                self.LoaiBaoCao('nhóm hàng hóa');
                self.MoiQuanTam('Báo cáo tổng hợp hàng nhập kho');
                self.getListDM_LoaiChungTuNhapKho("4,6,9,10,13,14");
                break;
            case 6:
                $(".showChungTu").show();
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                self.LoaiBaoCao('nhóm hàng hóa');
                self.MoiQuanTam('Báo cáo tổng hợp hàng xuất kho');
                self.getListDM_LoaiChungTuXuatKho("1,7,8,9,10");
                break;
        }

        if (valNew === valOld) {
            return;
        }
        loadCheckbox($(this).data('id'));
        self.LoadReport();
    });

    $('.choose_txtTime li').on('click', function () {
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeStart = '2015-09-26';
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày nhập: Toàn thời gian');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Tuần này
        else if (_rdoNgayPage === 3) {
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() + 6))).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));

        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() - datimeBC.getDay()))).format('YYYY-MM-DD');
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            let newtime = new Date();
            let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate()))).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Tháng này
        else if (_rdoNgayPage === 6) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Tháng trước
        else if (_rdoNgayPage === 7) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //30 ngày qua
        else if (_rdoNgayPage === 8) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            let newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Quý này
        else if (_rdoNgayPage === 9) {
            _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
            let newtime = new Date(moment().endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        // Quý trước
        else if (_rdoNgayPage === 10) {
            var prevQuarter = moment().quarter() - 1 === 0 ? 1 : moment().quarter() - 1;
            _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
            let newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Năm này
        else if (_rdoNgayPage === 11) {
            _timeStart = moment().startOf('year').format('YYYY-MM-DD');
            let newtime = new Date(moment().endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //năm trước
        else if (_rdoNgayPage === 12) {
            var prevYear = moment().year() - 1;
            _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            let newtime = new Date(moment().year(prevYear).endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        _pageNumber = 1;

        self.LoadReport();
    });
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        let dtBC = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
        if (_timeStart === _timeBC)
            self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.choose_TimeReport input').on('click', function () {
        if (parseInt($(this).val()) === 1) {
            $('.ip_TimeReport').removeAttr('disabled');
            $('.dr_TimeReport').attr("data-toggle", "dropdown");
            $('.ip_DateReport').attr('disabled', 'false');
            var _rdoNgayPage = $('.ip_TimeReport').val();
            var datime = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === "Toàn thời gian") {
                _timeStart = '2015-09-26';
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày nhập: Toàn thời gian');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Tuần này
            else if (_rdoNgayPage === "Tuần này") {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tuần trước
            else if (_rdoNgayPage === "Tuần trước") {
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //7 ngày qua
            else if (_rdoNgayPage === "7 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                let newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tháng trước
            else if (_rdoNgayPage === "Tháng trước") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //30 ngày qua
            else if (_rdoNgayPage === "30 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                let newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Quý này
            else if (_rdoNgayPage === "Quý này") {
                _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                let newtime = new Date(moment().endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            // Quý trước
            else if (_rdoNgayPage === "Quý trước") {
                var prevQuarter = moment().quarter() - 1 === 0 ? 1 : moment().quarter() - 1;
                _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                let newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Năm này
            else if (_rdoNgayPage === "Năm này") {
                _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                let newtime = new Date(moment().endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //năm trước
            else if (_rdoNgayPage === "Năm trước") {
                var prevYear = moment().year() - 1;
                _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                let newtime = new Date(moment().year(prevYear).endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            _pageNumber = 1;
            self.LoadReport();
        }
        else if (parseInt($(this).val()) === 2) {
            $('.ip_DateReport').removeAttr('disabled');
            $('.ip_TimeReport').attr('disabled', 'false');
            $('.dr_TimeReport').removeAttr('data-toggle');
            if ($('.ip_DateReport').val() !== "") {
                thisDate = $('.ip_DateReport').val();
                var t = thisDate.split("-");
                var checktime1 = t[0].trim().split("/");
                var checktime2 = t[1].trim().split("/");
                var t1 = t[0].trim().split("/").reverse().join("-");
                var thisDateStart = moment(t1).format('MM/DD/YYYY');
                var t2 = t[1].trim().split("/").reverse().join("-");
                var thisDateEnd = moment(t2).format('MM/DD/YYYY');
                _timeStart = moment(new Date(thisDateStart)).format('YYYY-MM-DD');
                var dt = new Date(thisDateEnd);
                let dtBC = new Date(thisDateEnd);
                _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
                if (_timeStart === _timeBC)
                    self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    });
    $('#datetimepicker_mask').keypress(function (e) {
        if (e.keyCode === 13) {
            dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-");
            thisDate = moment(t1).format('MM/DD/YYYY');
            _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            _tonkhoEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            if (thisDate !== 'Invalid date') {
                self.LoadReport();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        }
    });
    $('#datetimepicker_mask').on('change.dp', function (e) {
        dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-");
        thisDate = moment(t1).format('MM/DD/YYYY');
        _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _tonkhoEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        if (thisDate !== 'Invalid date') {
            self.LoadReport();
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
        }
    });

    self.Select_Text = function () {
        Text_search = $('#txt_search').val();
    };
    $('#txt_search').keypress(function (e) {
        if (e.keyCode === 13) {
            self.LoadReport();
        }
    });
    self.select_TonKho = function () {
        $('#btnExport').show();
        $('#select-column').show();
        tab_TonKho = 1;
        //$('.showChiNhanh').hide();
        //if (self.DonVis().length < 2)
        //    $('.showChiNhanh').hide();
        //else
        //    $('.showChiNhanh').show();
        self.LoaiBaoCao('hàng hóa');
        self.MoiQuanTam('Báo cáo hàng hóa tồn kho');
        self.TenChiNhanh(TenChiNhanh_tab);
        self.LoadReport();
    };
    self.select_TonKho_TongHop = function () {
        $('#btnExport').hide();
        $('#select-column').hide();
        tab_TonKho = 2;
        //if (self.DonVis().length < 2)
        //    $('.showChiNhanh').hide();
        //else
        //    $('.showChiNhanh').show();
        self.TenChiNhanh(TenChiNhanh);
        self.LoaiBaoCao('chi nhánh');
        self.MoiQuanTam('Báo cáo hàng hóa tồn kho theo chi nhánh');
        self.LoadReport();
    };
    self.LoadTonKho_byChiNhanh = function (item) {
        _id_DonVi = item.ID_ChiNhanh;
        tab_TonKho = 1;
        self.TenChiNhanh(item.TenChiNhanh);
        TenChiNhanh_tab = item.TenChiNhanh;
        //$('.showChiNhanh').hide();
        $('#tabble_TonKhoChiTiet').addClass('active');
        $('#tabble_TonKhoTongHop').removeClass('active');
        $('#table_TonKho').addClass('active');
        $('#table_TongHop').removeClass('active');
        self.LoadReport();
    };
    var _selectTab = 1;
    var dk_tab = 1;
    self.selectXuatChuyenHang = function () {
        $('.table_TongQuan1').hide();
        $('.table_TongQuan2').hide();
        $('.table_ChiTiet').hide();
        dk_tab = 1;
        if (_selectTab === 1) {
            $('.table_TongQuan1').show();
            self.LoaiBaoCao('hàng hóa');
            $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
            self.LoadReport();
        }
        else {
            $('.table_ChiTiet').show();
            self.LoaiBaoCao('hàng hóa chi tiết');
            $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng, mã hóa đơn").blur();
            self.LoadReport();
        }
    };
    self.selectNhapChuyenHang = function () {
        dk_tab = 2;
        $('.table_TongQuan1').hide();
        $('.table_TongQuan2').hide();
        $('.table_ChiTiet').hide();
        if (_selectTab === 1) {
            $('.table_TongQuan2').show();
            self.LoaiBaoCao('hàng hóa');
            $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
            self.LoadReport();
        }
        else {
            $('.table_ChiTiet').show();
            self.LoaiBaoCao('hàng hóa chi tiết');
            $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng, mã hóa đơn").blur();
            self.LoadReport();
        }
    };
    self.selectHangHoaNhapKho = function () {
        dk_tabtk = 1;
        self.LoaiBaoCao('hàng hóa');
        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
        self.LoadReport();
    };
    self.selectGiaoDichNhapKho = function () {
        dk_tabtk = 2;
        self.LoaiBaoCao('hàng hóa chi tiết');
        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng, mã hóa đơn").blur();
        self.LoadReport();
    };
    self.selectHangHoaXuatKho = function () {
        dk_tabxk = 1;
        self.LoaiBaoCao('hàng hóa');
        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
        self.LoadReport();
    };
    self.selectGiaoDichXuatKho = function () {
        dk_tabxk = 2;
        self.LoaiBaoCao('hàng hóa chi tiết');
        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng, mã hóa đơn").blur();
        self.LoadReport();
    };
    self.selectXuatDichVuDinhLuong = function () {
        dk_tabxk = 3;
        self.LoaiBaoCao('Thành phần định lượng');
        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng, mã DV, mã HĐ, trạng thái").blur();
        self.LoadReport();
    };
    $('.Select_TableCH input').on('click', function () {
        if ($(this).val() !== '1') {
            loadCheckbox($(this).closest('li').data('id'), false);
        }
        else {
            $('#dieuchuyenhanghoa .nav-tabs li').each(function () {
                if ($(this).hasClass('active'))
                    loadCheckbox($(this).data('id'), false);
            });
        }
        self.Select_Table($(this).val());
        _selectTab = parseInt($(this).val());
        if (dk_tab === 1)
            self.selectXuatChuyenHang();
        else
            self.selectNhapChuyenHang();
    });
    self.RefreshPrint = function () {
        self.BaoCaoKho_TonKho([]);
        self.BaoCaoKho_NhapXuatTon([]);
        self.BaoCaoKho_NhapXuatTonChiTiet([]);
        self.BaoCaoKho_XuatDieuChuyen([]);
        self.BaoCaoKho_NhapDieuChuyen([]);
        self.BaoCaoKho_DieuChuyenChiTiet([]);
        self.BaoCaoKho_TongHopHangXuat([]);
        self.BaoCaoKho_ChiTietHangXuat([]);
        self.BaoCaoKho_TongHopHangNhap([]);
        self.BaoCaoKho_ChiTietHangNhap([]);
    };

    var Text_search = "";
    var _laHangHoa = 2;
    var TinhTrangHH = 2;
    let _trangThai = 1;
    let _tonKho = 0;
    var _ID_NhomHang = null;
    self.BaoCaoKho_TonKho = ko.observableArray();
    self.BaoCaoKho_TongKho_TongHop = ko.observableArray();
    self.BaoCaoKho_NhapXuatTon = ko.observableArray();
    self.BaoCaoKho_NhapXuatTonChiTiet = ko.observableArray();
    self.BaoCaoKho_XuatDieuChuyen = ko.observableArray();
    self.BaoCaoKho_NhapDieuChuyen = ko.observableArray();
    self.BaoCaoKho_DieuChuyenChiTiet = ko.observableArray();
    self.BaoCaoKho_TongHopHangXuat = ko.observableArray();
    self.BaoCaoKho_XuatDichVuDinhLuong = ko.observableArray();
    self.BaoCaoKho_ChiTietHangXuat = ko.observableArray();
    self.BaoCaoKho_TongHopHangNhap = ko.observableArray();
    self.BaoCaoKho_ChiTietHangNhap = ko.observableArray();
    self.TK_SoLuongTon = ko.observable();
    self.TKTH_SoLuongTon = ko.observable();
    self.TK_TonQuyCach = ko.observable();
    self.TK_GiaTriTon = ko.observable();
    self.TKTH_GiaTriTon = ko.observable();
    self.NXT_TonDauKy = ko.observable();
    self.NXT_GiaTriDauKy = ko.observable();
    self.NXT_SoLuongNhap = ko.observable();
    self.NXT_GiaTriNhap = ko.observable();
    self.NXT_SoLuongXuat = ko.observable();
    self.NXT_GiaTriXuat = ko.observable();
    self.NXT_TonCuoiKy = ko.observable();
    self.NXT_TonQuyCach = ko.observable();
    self.NXT_GiaTriCuoiKy = ko.observable();
    self.NXTCT_TonDauKy = ko.observable();
    self.NXTCT_GiaTriDauKy = ko.observable();
    self.NXTCT_NhapNCC = ko.observable();
    self.NXTCT_NhapKiem = ko.observable();
    self.NXTCT_NhapTra = ko.observable();
    self.NXTCT_NhapChuyen = ko.observable();
    self.NXTCT_NhapSX = ko.observable();
    self.NXTCT_XuatBan = ko.observable();
    self.NXTCT_XuatHuy = ko.observable();
    self.NXTCT_XuatNCC = ko.observable();
    self.NXTCT_XuatKiem = ko.observable();
    self.NXTCT_XuatChuyen = ko.observable();
    self.NXTCT_XuatSX = ko.observable();
    self.NXTCT_TonCuoiKy = ko.observable();
    self.NXTCT_GiaTriCuoiKy = ko.observable();
    self.XDC_SoLuong = ko.observable();
    self.XDC_ThanhTien = ko.observable();
    self.NDC_SoLuong = ko.observable();
    self.NDC_ThanhTien = ko.observable();
    self.DCCT_SoLuong = ko.observable();
    self.DCCT_GiaTri = ko.observable();
    self.DCCT_ThanhTien = ko.observable();

    self.TNH_SoLuongNhap = ko.observable();
    self.TNH_ThanhTien = ko.observable();
    self.TNH_GiaTriNhap = ko.observable();
    self.TNH_GiamGiaHD = ko.observable();
    self.NCC_SoLuongNhap = ko.observable();
    self.NCC_ThanhTien = ko.observable();
    self.NCC_GiaTriNhap = ko.observable();
    self.NCC_GiamGiaHD = ko.observable();
    self.THHN_SoLuong = ko.observable();
    self.THHN_ThanhTien = ko.observable();
    self.CTHN_SoLuong = ko.observable();
    self.CTHN_ThanhTien = ko.observable();
    self.THHX_SoLuong = ko.observable();
    self.THHX_ThanhTien = ko.observable();
    self.CTHX_SoLuong = ko.observable();
    self.CTHX_ThanhTien = ko.observable();
    self.XDVDL_SoLuongDichVu = ko.observable();
    self.XDVDL_GiaTriDichVu = ko.observable();
    self.XDVDL_SoLuongBanDau = ko.observable();
    self.XDVDL_GiaTriBanDau = ko.observable();
    self.XDVDL_SoLuongThucTe = ko.observable();
    self.XDVDL_GiaTriThucTe = ko.observable();
    self.XDVDL_SoLuongChenhLech = ko.observable();
    self.XDVDL_GiaTriChenhLech = ko.observable();

    self.LoadReport = function () {
        $('.table-reponsive').css('display', 'none');
        LoadingForm(true);
        self.RefreshPrint();
        _pageNumber_CT = 1;
        self.pageNumber_TK(1);
        self.pageNumber_NXT(1);
        self.pageNumber_NXTCT(1);
        self.pageNumber_TQNH(1);
        self.pageNumber_TQXH(1);
        self.pageNumber_CTDC(1);
        self.pageNumber_THNHH(1);
        self.pageNumber_THNGD(1);
        self.pageNumber_THXHH(1);
        self.pageNumber_THXGD(1);
        self.pageNumber_XDVDL(1);
        _pageNumber = 1;

        if (!commonStatisJs.CheckNull(Text_search)) {
            Text_search = Text_search.trim();
        }

        var array_Seach = {
            MaHangHoa: Text_search,
            timeStart: self.check_MoiQuanTam() !== 1 ? _timeStart : _tonkhoStart,
            timeEnd: self.check_MoiQuanTam() !== 1 ? _timeEnd : _tonkhoEnd,
            ID_DonVi: _id_DonVi,
            ID_ChiNhanh: _idDonViSeach,
            LaHangHoa: _laHangHoa,
            TinhTrang: TinhTrangHH,
            ID_NhomHang: _ID_NhomHang,
            LoaiChungTu: _idChungTuSeach,
            ID_NguoiDung: _IDDoiTuong,
            columnsHide: null,
            columnsHideCT: null,
            TodayBC: null,
            TenChiNhanh: null,
            TrangThai: _trangThai,
            TonKho: parseInt(self.Loc_TonKho()),
            lstIDChiNhanh: self.LstIDDonVi(),
            HanBaoHanh: _trangThai,
            PageSize: self.pageSize(),
        };

        var toDate = moment(array_Seach.timeEnd, 'YYYY-MM-DD').add('days', -1).format('YYYY-MM-DD');
        if (array_Seach.timeStart === toDate) {
            self.TodayBC_TK(moment(array_Seach.timeStart, 'YYYY-MM-DD').format('DD/MM/YYYY'));
        }
        else {
            self.TodayBC_TK(moment(array_Seach.timeStart, 'YYYY-MM-DD').format('DD/MM/YYYY')
                .concat(' - ', moment(toDate, 'YYYY-MM-DD').format('DD/MM/YYYY')))
        }

        switch (self.check_MoiQuanTam()) {
            case 1:
                if (self.BCK_TonKho() === "BCK_TonKho") {
                    $(".PhanQuyen").hide();
                    if (tab_TonKho === 1) {
                        ajaxHelper(ReportUri + "BaoCaoKho_TonKho", "POST", array_Seach).done(function (data) {
                            self.BaoCaoKho_TonKho(data.LstData);
                            AllPage = data.numberPage;
                            self.selecPage();
                            self.ReserPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.TK_SoLuongTon(data.a1);
                            self.TK_TonQuyCach(data.a2);
                            self.TK_GiaTriTon(data.a3);
                            LoadingForm(false);
                            $('#table-reponsive').css('display', 'block');
                        });
                    }
                    else {
                        ajaxHelper(ReportUri + "BaoCaoKho_TonKho_TongHop", "POST", array_Seach).done(function (data) {
                            self.BaoCaoKho_TongKho_TongHop(data.LstData);
                            AllPage = data.numberPage;
                            self.selecPage();
                            self.ReserPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.TKTH_SoLuongTon(data.a1);
                            self.TKTH_GiaTriTon(data.a2);
                            LoadingForm(false);
                            $('#table_TongHop .table-reponsive').css('display', 'block');
                        });
                    }
                }
                else {
                    $(".PhanQuyen").show();
                    LoadingForm(false);
                }
                break;
            case 2:
                if (self.BCK_NhapXuatTon() === "BCK_NhapXuatTon") {
                    $(".PhanQuyen").hide();
                    array_Seach.XuatKho = false;
                    ajaxHelper(ReportUri + "BaoCaoKho_NhapXuatTon", "POST", array_Seach).done(function (data) {
                        self.BaoCaoKho_NhapXuatTon(data.LstData);
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.NXT_TonDauKy(data.a1);
                        self.NXT_GiaTriDauKy(data.a2);
                        self.NXT_SoLuongNhap(data.a3);
                        self.NXT_GiaTriNhap(data.a4);
                        self.NXT_SoLuongXuat(data.a5);
                        self.NXT_GiaTriXuat(data.a6);
                        self.NXT_TonCuoiKy(data.a7);
                        self.NXT_TonQuyCach(data.a8);
                        self.NXT_GiaTriCuoiKy(data.a9);
                        LoadingForm(false);
                        $('#Detail .table-reponsive').css('display', 'block');
                    });
                }
                else {
                    $(".PhanQuyen").show();
                    LoadingForm(false);
                }
                break;
            case 3:
                if (self.BCK_NhapXuatTonChiTiet() === "BCK_NhapXuatTonChiTiet") {
                    $(".PhanQuyen").hide();
                    array_Seach.XuatKho = false;
                    ajaxHelper(ReportUri + "BaoCaoKho_NhapXuatTonChiTiet", "POST", array_Seach).done(function (data) {
                        self.BaoCaoKho_NhapXuatTonChiTiet(data.LstData);
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.NXTCT_TonDauKy(data.a1);
                        self.NXTCT_GiaTriDauKy(data.a2);
                        self.NXTCT_NhapNCC(data.a3);
                        self.NXTCT_NhapKiem(data.a4);
                        self.NXTCT_NhapTra(data.a5);
                        self.NXTCT_NhapChuyen(data.a6);
                        self.NXTCT_NhapSX(data.a7);
                        self.NXTCT_XuatBan(data.a8);
                        self.NXTCT_XuatHuy(data.a9);
                        self.NXTCT_XuatNCC(data.a10);
                        self.NXTCT_XuatKiem(data.a11);
                        self.NXTCT_XuatChuyen(data.a12);
                        self.NXTCT_XuatSX(data.a13);
                        self.NXTCT_TonCuoiKy(data.a14);
                        self.NXTCT_GiaTriCuoiKy(data.a15);
                        LoadingForm(false);
                        $('#nhapxuattonchitiet .table-reponsive').css('display', 'block');
                    });
                }
                else {
                    $(".PhanQuyen").show();
                    LoadingForm(false);
                }
                break;
            case 4:
                if (self.BCK_DieuChuyenHangHoa() === "BCK_DieuChuyenHangHoa") {
                    $(".PhanQuyen").hide();
                    if (dk_tab === 1) {
                        if (_selectTab === 1) {
                            ajaxHelper(ReportUri + "BaoCaoKho_XuatChuyenHang", "POST", array_Seach).done(function (data) {
                                self.BaoCaoKho_XuatDieuChuyen(data.LstData);
                                AllPage = data.numberPage;
                                self.selecPage();
                                self.ReserPage();
                                self.SumRowsHangHoa(data.Rowcount);
                                self.XDC_SoLuong(data.a1);
                                self.XDC_ThanhTien(data.a2);
                                LoadingForm(false);
                                $('#table_ChuyenHang .table-reponsive').css('display', 'block');
                            });
                        }
                        else {
                            ajaxHelper(ReportUri + "BaoCaoKho_XuatChuyenHangChiTiet", "POST", array_Seach).done(function (data) {
                                self.BaoCaoKho_DieuChuyenChiTiet(data.LstData);
                                AllPage = data.numberPage;
                                self.selecPage();
                                self.ReserPage();
                                self.SumRowsHangHoa(data.Rowcount);
                                self.DCCT_SoLuong(data.a1);
                                self.DCCT_GiaTri(data.a2);
                                self.DCCT_ThanhTien(data.a3);
                                LoadingForm(false);
                                $('#table_ChuyenHangChiTiet .table-reponsive').css('display', 'block');
                            });
                        }
                    }
                    else {
                        if (_selectTab === 1) {
                            ajaxHelper(ReportUri + "BaoCaoKho_NhapChuyenHang", "POST", array_Seach).done(function (data) {
                                self.BaoCaoKho_NhapDieuChuyen(data.LstData);
                                AllPage = data.numberPage;
                                self.selecPage();
                                self.ReserPage();
                                self.SumRowsHangHoa(data.Rowcount);
                                self.NDC_SoLuong(data.a1);
                                self.NDC_ThanhTien(data.a2);
                                LoadingForm(false);
                                $('#table_NhapHang .table-reponsive').css('display', 'block');
                            });
                        }
                        else {
                            ajaxHelper(ReportUri + "BaoCaoKho_NhapChuyenHangChiTiet", "POST", array_Seach).done(function (data) {
                                self.BaoCaoKho_DieuChuyenChiTiet(data.LstData);
                                AllPage = data.numberPage;
                                self.selecPage();
                                self.ReserPage();
                                self.SumRowsHangHoa(data.Rowcount);
                                self.DCCT_SoLuong(data.a1);
                                self.DCCT_GiaTri(data.a2);
                                self.DCCT_ThanhTien(data.a3);
                                LoadingForm(false);
                                $('#table_ChuyenHangChiTiet .table-reponsive').css('display', 'block');
                            });
                        }
                    }
                }
                else {
                    $(".PhanQuyen").show();
                    LoadingForm(false);
                }
                break;
            case 5:
                if (self.BCK_TH_HangNhapKho() === "BCK_TH_HangNhapKho") {
                    $(".PhanQuyen").hide();
                    array_Seach.XuatKho = false;
                    if (dk_tabtk === 1) {
                        ajaxHelper(ReportUri + "BaoCaoKho_TongHopHangNhapKho", "POST", array_Seach).done(function (data) {
                            self.BaoCaoKho_TongHopHangNhap(data.LstData);
                            AllPage = data.numberPage;
                            self.selecPage();
                            self.ReserPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.THHN_SoLuong(data.a1);
                            self.THHN_ThanhTien(data.a2);
                            LoadingForm(false);
                            $('#table_HangHoaNhapKho .table-reponsive').css('display', 'block');
                        });
                    }
                    else
                        ajaxHelper(ReportUri + "BaoCaoKho_ChiTietHangNhapKho", "POST", array_Seach).done(function (data) {
                            self.BaoCaoKho_ChiTietHangNhap(data.LstData);
                            AllPage = data.numberPage;
                            self.selecPage();
                            self.ReserPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.CTHN_SoLuong(data.a1);
                            self.CTHN_ThanhTien(data.a2);
                            LoadingForm(false);
                            $('#table_GiaoDichNhapKho .table-reponsive').css('display', 'block');
                        });
                }
                else {
                    $(".PhanQuyen").show();
                    LoadingForm(false);
                }
                break;
            case 6:
                if (self.BCK_TH_HangXuatKho() === "BCK_TH_HangXuatKho") {
                    $(".PhanQuyen").hide();
                    array_Seach.XuatKho = true;
                    if (dk_tabxk === 1) {
                        ajaxHelper(ReportUri + "BaoCaoKho_TongHopHangXuatKho", "POST", array_Seach).done(function (data) {
                            self.BaoCaoKho_TongHopHangXuat(data.LstData);
                            AllPage = data.numberPage;
                            self.selecPage();
                            self.ReserPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.THHX_SoLuong(data.a1);
                            self.THHX_ThanhTien(data.a2);
                            LoadingForm(false);
                            $('#table_HangHoaXuatKho .table-reponsive').css('display', 'block');
                        });
                    }
                    else if (dk_tabxk === 2) {
                        ajaxHelper(ReportUri + "BaoCaoKho_ChiTietHangXuatKho", "POST", array_Seach).done(function (data) {
                            self.BaoCaoKho_ChiTietHangXuat(data.LstData);
                            AllPage = data.numberPage;
                            self.selecPage();
                            self.ReserPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.CTHX_SoLuong(data.a1);
                            self.CTHX_ThanhTien(data.a2);
                            LoadingForm(false);
                            $('#table_GiaoDichXuatKho .table-reponsive').css('display', 'block');
                        });
                    }
                    else {
                        ajaxHelper(ReportUri + "BaoCaoKho_XuatDichVuDinhLuong", "POST", array_Seach).done(function (data) {
                            self.BaoCaoKho_XuatDichVuDinhLuong(data.LstData);
                            AllPage = data.numberPage;
                            self.selecPage();
                            self.ReserPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.XDVDL_SoLuongDichVu(data.a1);
                            self.XDVDL_GiaTriDichVu(data.a2);
                            self.XDVDL_SoLuongBanDau(data.a3);
                            self.XDVDL_GiaTriBanDau(data.a4);
                            self.XDVDL_SoLuongThucTe(data.a5);
                            self.XDVDL_GiaTriThucTe(data.a6);
                            self.XDVDL_SoLuongChenhLech(data.a7);
                            self.XDVDL_GiaTriChenhLech(data.a8);
                            LoadingForm(false);
                            $('#table_dichvudinhluong .table-reponsive').css('display', 'block');
                        });
                    }
                }
                else {
                    $(".PhanQuyen").show();
                    LoadingForm(false);
                }
                break;
        }
    };
    self.BaoCaoKho_TonKho_Page = ko.computed(function (x) {
        var first = (self.pageNumber_TK() - 1) * self.pageSize();
        if (self.BaoCaoKho_TonKho() !== null) {
            if (self.BaoCaoKho_TonKho().length !== 0) {
                self.RowsStart((self.pageNumber_TK() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_TK() - 1) * self.pageSize() + self.BaoCaoKho_TonKho().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_TonKho().slice(first, first + self.pageSize());
        }
        return null;
    });
    self.BaoCaoKho_TongKho_TongHop_Page = ko.computed(function (x) {
        var first = (self.pageNumber_TK_TH() - 1) * self.pageSize();
        if (self.BaoCaoKho_TongKho_TongHop() !== null) {
            if (self.BaoCaoKho_TongKho_TongHop().length !== 0) {
                self.RowsStart((self.pageNumber_TK_TH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_TK_TH() - 1) * self.pageSize() + self.BaoCaoKho_TongKho_TongHop().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_TongKho_TongHop().slice(first, first + self.pageSize());
        }
        return null;
    });
    self.BaoCaoKho_NhapXuatTon_Page = ko.computed(function (x) {
        var first = (self.pageNumber_NXT() - 1) * self.pageSize();
        if (self.BaoCaoKho_NhapXuatTon() !== null) {
            if (self.BaoCaoKho_NhapXuatTon().length !== 0) {
                self.RowsStart((self.pageNumber_NXT() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_NXT() - 1) * self.pageSize() + self.BaoCaoKho_NhapXuatTon().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_NhapXuatTon().slice(first, first + self.pageSize());
        }
        return null;
    });
    self.BaoCaoKho_NhapXuatTonChiTiet_Page = ko.computed(function (x) {
        var first = (self.pageNumber_NXTCT() - 1) * self.pageSize();
        if (self.BaoCaoKho_NhapXuatTonChiTiet() !== null) {
            if (self.BaoCaoKho_NhapXuatTonChiTiet().length !== 0) {
                self.RowsStart((self.pageNumber_NXTCT() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_NXTCT() - 1) * self.pageSize() + self.BaoCaoKho_NhapXuatTonChiTiet().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_NhapXuatTonChiTiet().slice(first, first + self.pageSize());
        }
        return null;
    });
    self.BaoCaoKho_XuatDieuChuyen_Page = ko.computed(function (x) {
        var first = (self.pageNumber_TQXH() - 1) * self.pageSize();
        if (self.BaoCaoKho_XuatDieuChuyen() !== null) {
            if (self.BaoCaoKho_XuatDieuChuyen().length !== 0) {
                self.RowsStart((self.pageNumber_TQXH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_TQXH() - 1) * self.pageSize() + self.BaoCaoKho_XuatDieuChuyen().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_XuatDieuChuyen().slice(first, first + self.pageSize());
        }
        return null;
    });

    self.BaoCaoKho_DieuChuyenChiTiet_Page = ko.computed(function (x) {
        var first = (self.pageNumber_CTDC() - 1) * self.pageSize();
        if (self.BaoCaoKho_DieuChuyenChiTiet() !== null) {
            if (self.BaoCaoKho_DieuChuyenChiTiet().length !== 0) {
                self.RowsStart((self.pageNumber_CTDC() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_CTDC() - 1) * self.pageSize() + self.BaoCaoKho_DieuChuyenChiTiet().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_DieuChuyenChiTiet().slice(first, first + self.pageSize());
        }
        return null;
    });
    self.BaoCaoKho_NhapDieuChuyen_Page = ko.computed(function (x) {
        var first = (self.pageNumber_TQNH() - 1) * self.pageSize();
        if (self.BaoCaoKho_NhapDieuChuyen() !== null) {
            if (self.BaoCaoKho_NhapDieuChuyen().length !== 0) {
                self.RowsStart((self.pageNumber_TQNH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_TQNH() - 1) * self.pageSize() + self.BaoCaoKho_NhapDieuChuyen().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_NhapDieuChuyen().slice(first, first + self.pageSize());
        }
        return null;
    });

    self.BaoCaoKho_TongHopHangNhap_Page = ko.computed(function (x) {
        var first = (self.pageNumber_THNHH() - 1) * self.pageSize();
        if (self.BaoCaoKho_TongHopHangNhap() !== null) {
            if (self.BaoCaoKho_TongHopHangNhap().length !== 0) {
                self.RowsStart((self.pageNumber_THNHH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_THNHH() - 1) * self.pageSize() + self.BaoCaoKho_TongHopHangNhap().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_TongHopHangNhap().slice(first, first + self.pageSize());
        }
        return null;
    });
    self.BaoCaoKho_ChiTietHangNhap_Page = ko.computed(function (x) {
        var first = (self.pageNumber_THNGD() - 1) * self.pageSize();
        if (self.BaoCaoKho_ChiTietHangNhap() !== null) {
            if (self.BaoCaoKho_ChiTietHangNhap().length !== 0) {
                self.RowsStart((self.pageNumber_THNGD() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_THNGD() - 1) * self.pageSize() + self.BaoCaoKho_ChiTietHangNhap().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_ChiTietHangNhap().slice(first, first + self.pageSize());
        }
        return null;
    });

    self.BaoCaoKho_TongHopHangXuat_Page = ko.computed(function (x) {
        var first = (self.pageNumber_THXHH() - 1) * self.pageSize();
        if (self.BaoCaoKho_TongHopHangXuat() !== null) {
            if (self.BaoCaoKho_TongHopHangXuat().length !== 0) {
                self.RowsStart((self.pageNumber_THXHH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_THXHH() - 1) * self.pageSize() + self.BaoCaoKho_TongHopHangXuat().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_TongHopHangXuat().slice(first, first + self.pageSize());
        }
        return null;
    });
    self.BaoCaoKho_ChiTietHangXuat_Page = ko.computed(function (x) {
        var first = (self.pageNumber_THXGD() - 1) * self.pageSize();
        if (self.BaoCaoKho_ChiTietHangXuat() !== null) {
            if (self.BaoCaoKho_ChiTietHangXuat().length !== 0) {
                self.RowsStart((self.pageNumber_THXGD() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_THXGD() - 1) * self.pageSize() + self.BaoCaoKho_ChiTietHangXuat().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_ChiTietHangXuat().slice(first, first + self.pageSize());
        }
        return null;
    });

    self.BaoCaoKho_XuatDichVuDinhLuong_Page = ko.computed(function (x) {
        var first = (self.pageNumber_XDVDL() - 1) * self.pageSize();
        if (self.BaoCaoKho_XuatDichVuDinhLuong() !== null) {
            if (self.BaoCaoKho_XuatDichVuDinhLuong().length !== 0) {
                self.RowsStart((self.pageNumber_XDVDL() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_XDVDL() - 1) * self.pageSize() + self.BaoCaoKho_XuatDichVuDinhLuong().slice(first, first + self.pageSize()).length);
            }
            else {
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoKho_XuatDichVuDinhLuong().slice(first, first + self.pageSize());
        }
        return null;
    });
    //Download file excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function (item) {
        let url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    };
    // xuất file Excel
    self.ExportExcel = function () {
        LoadingForm(true);
        var keyselected;
        var lisstcolumn = [];
        var arrayColumn = [];
        var arrayColumnCT = [];
        var columnHide = null;
        var columnHideCT = null;
        $('.chose_kieubang li').each(function () {
            if ($(this).hasClass('active'))
                keyselected = $(this).data('id');
        });

        // bc dieuchuyen/nhap/xuat: 1 file excel có 2 sheet (tổng hợp + chi tiết)
        switch (parseInt(keyselected)) {
            case parseInt($('#ID_dieuchuyenhh').val()):
                $('#dieuchuyenhanghoa .tab-content .tab-pane').each(function (i) {
                    if ($(this).hasClass('active')) {
                        var key = Key_Form + $('#ID_dieuchuyenchitiet').val();
                        $('#dieuchuyenhanghoa .nav-tabs li').each(function () {
                            if ($(this).hasClass('active')) {
                                key = key + '_' + $(this).val();
                            }
                        });
                        lisstcolumn.push({ main: 0, detail: LocalCaches.LoadColumnGrid(Key_Form + $(this).data('id')).map(x => x.Value) });
                        lisstcolumn.push({ main: 1, detail: LocalCaches.LoadColumnGrid(key).map(x => x.Value) });
                    }
                });
                arrayColumn = lisstcolumn[0].detail;
                arrayColumnCT = lisstcolumn[1].detail;
                break;
            case parseInt($('#ID_thhangnhapkho').val()):
                $('#tonghophangnhapkho .tab-content .tab-pane').each(function (i) {
                    lisstcolumn.push({ main: i, detail: LocalCaches.LoadColumnGrid(Key_Form + $(this).data('id')).map(x => x.Value) });
                });
                arrayColumn = lisstcolumn[0].detail;
                arrayColumnCT = lisstcolumn[1].detail;
                break;
            case parseInt($('#ID_thhangxuatkho').val()):
                if (self.columnCheckType() === 42) {
                    // bc dichvu dinhluong
                    $("#select-column .dropdown-list ul li").each(function (i) {
                        if (!$(this).find('input').is(':checked')) {
                            arrayColumn.push(i);
                        }
                    });
                }
                else {
                    $('#tonghophangxuatkho .tab-content .tab-pane').each(function (i) {
                        lisstcolumn.push({ main: i, detail: LocalCaches.LoadColumnGrid(Key_Form + $(this).data('id')).map(x => x.Value) });
                    });
                    arrayColumn = lisstcolumn[0].detail;
                    arrayColumnCT = lisstcolumn[1].detail;
                }
                break;
            default:
                $("#select-column .dropdown-list ul li").each(function (i) {
                    if (!$(this).find('input').is(':checked')) {
                        arrayColumn.push(i);
                    }
                });
                break;
        }

        console.log('arrayColumn ', arrayColumnCT, arrayColumn)

        LoadingForm(false);
        arrayColumn.sort();
        for (let i = 0; i < arrayColumn.length; i++) {
            if (i === 0) {
                columnHide = arrayColumn[i];
            }
            else {
                columnHide = arrayColumn[i] + "_" + columnHide;
            }
        }
        arrayColumnCT.sort();
        for (let i = 0; i < arrayColumnCT.length; i++) {
            if (i === 0) {
                columnHideCT = arrayColumnCT[i];
            }
            else {
                columnHideCT = arrayColumnCT[i] + "_" + columnHideCT;
            }
        }
        var diary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo bán hàng",
            NoiDung: "Xuất " + self.MoiQuanTam().toLowerCase(),
            NoiDungChiTiet: "Xuất " + self.MoiQuanTam().toLowerCase(),
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };

        Insert_NhatKyThaoTac_1Param(diary);

        var array_Seach = {
            MaHangHoa: locdau(Text_search),
            timeStart: self.check_MoiQuanTam() !== 1 ? _timeStart : _tonkhoStart,
            timeEnd: self.check_MoiQuanTam() !== 1 ? _timeEnd : _tonkhoEnd,
            ID_DonVi: _id_DonVi,
            ID_ChiNhanh: _idDonViSeach,
            LaHangHoa: _laHangHoa,
            TinhTrang: TinhTrangHH,
            ID_NhomHang: _ID_NhomHang,
            LoaiChungTu: _idChungTuSeach,
            ID_NguoiDung: _IDDoiTuong,
            columnsHide: columnHide,
            columnsHideCT: columnHideCT,
            TodayBC: 'Thời gian: '.concat(self.TodayBC_TK()),
            TenChiNhanh: self.TenChiNhanh(),
            TrangThai: _trangThai,
            TonKho: _tonKho,
            lstIDChiNhanh: self.LstIDDonVi(),
            XuatKho: true,// true: la bc xuatkho
        };

        if (self.BCK_XuatFile() !== "BCK_XuatFile") {
            ShowMessage_Danger('Bạn không có quyền xuất file báo cáo này');
            LoadingForm(false);
            return false;
        }

        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                if (self.BaoCaoKho_TonKho().length > 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCK_TonKho",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                break;
            case 2:
                if (self.BaoCaoKho_NhapXuatTon().length === 0) {
                    ShowMessage_Danger('Không có dữ liệu');
                    return;
                }
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: ReportUri + "Export_BCK_NhapXuatTon",
                    data: { objExcel: array_Seach },
                    success: function (url) {
                        self.DownloadFileTeamplateXLSX(url);
                        LoadingForm(false);
                    }
                });
                break;
            case 3:
                if (self.BaoCaoKho_NhapXuatTonChiTiet().length === 0) {
                    ShowMessage_Danger('Không có dữ liệu');
                    return;
                }

                if ($.inArray(10, arrayColumn) > -1) {
                    // hide column Xuat (contains 5 columns)
                    columnHide += '_13_14_15_16_17';
                    arrayColumn.push(13);
                    arrayColumn.push(14);
                    arrayColumn.push(15);
                    arrayColumn.push(16);
                    arrayColumn.push(17);
                    arrayColumn.filter(x => x !== 10);
                }

                if ($.inArray(9, arrayColumn) > -1) {
                    // hide column Nhap (contains 4 columns)
                    arrayColumn.push(10);
                    arrayColumn.push(11);
                    arrayColumn.push(12);
                }
                console.log('arrayColumn ', arrayColumn)
                let thisC = '';
                for (let i = 0; i < arrayColumn.length; i++) {
                    thisC += arrayColumn[i] + '_';
                }
                array_Seach.columnsHide = thisC;

                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: ReportUri + "Export_BCK_NhapXuatTonChiTiet",
                    data: { objExcel: array_Seach },
                    success: function (url) {
                        self.DownloadFileTeamplateXLSX(url);
                        LoadingForm(false);
                    }
                });
                break;
            case 4:
                if (dk_tab === 1) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCK_XuatChuyenHang",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                else {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCK_NhapChuyenHang",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                break;
            case 5:
                array_Seach.XuatKho = false;
                $.ajax({
                    type: "POST",
                    dataType: "json",
                    url: ReportUri + "Export_BCK_TongHopHangNhapKho",
                    data: { objExcel: array_Seach },
                    success: function (url) {
                        self.DownloadFileTeamplateXLSX(url);
                        LoadingForm(false);
                    }
                });
                break;
            case 6:
                if (dk_tabxk === 3) {
                    // add index of column (because use Gara)
                    let lstColumn = [];
                    if (array_Seach.columnsHide !== null) {
                        lstColumn = array_Seach.columnsHide.toString().split('_');
                    }
                    lstColumn = lstColumn.filter(x => x !== '');
                    let lstAfter = [];
                    if (!self.isGara()) {
                        for (let i = 0; i < lstColumn.length; i++) {
                            let itFor = parseInt(lstColumn[i]);
                            if (itFor > 2) {
                                lstAfter.push(itFor + 2);// cot BienSo (index = 3)
                            }
                            else {
                                lstAfter.push(itFor);
                            }
                        }
                    }
                    else {
                        lstAfter = lstColumn;
                    }
                    let columnHideThis = '';
                    for (let i = 0; i < lstAfter.length; i++) {
                        columnHideThis += lstAfter[i].toString() + '_';
                    }
                    array_Seach.columnsHide = columnHideThis;
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCK_XuatDinhLuongDichVu",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                else {
                    array_Seach.XuatKho = true;
                    if (!self.isGara()) {

                        let lstColumn = [];
                        if (array_Seach.columnsHideCT !== null) {
                            lstColumn = array_Seach.columnsHideCT.toString().split('_');
                        }
                        lstColumn = lstColumn.filter(x => x !== '');
                        let lstAfter = [];
                        for (let i = 0; i < lstColumn.length; i++) {
                            if (lstColumn[i] > 2) {
                                lstAfter.push(parseInt(lstColumn[i]) + 1);
                            }
                            else {
                                lstAfter.push(lstColumn[i]);
                            }
                        }
                        lstAfter.push(3); //column BienSo
                        let columnHideThis = '';
                        for (let i = 0; i < lstAfter.length; i++) {
                            columnHideThis += lstAfter[i].toString() + '_';
                        }
                        array_Seach.columnsHideCT = columnHideThis;
                    }

                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCK_TongHopHangXuatKho",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                break;
        }
    };
    self.ExportChiTietNhanVien = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo bán hàng",
            NoiDung: "Xuất báo cáo danh sách hàng bán theo nhân viên",
            NoiDungChiTiet: "Xuất báo cáo danh sách hàng bán theo nhân viên",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        var myData = {};
        myData.objDiary = objDiary;
        $.ajax({
            url: DiaryUri + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (item) {
                var columnHide = null;
                let url = ReportUri + "Export_BCBHCT_TheoNhanVien?ID_NhanVien=" + _idNhanVienBanHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach + "&LaHangHoa=" + _laHangHoa + "&TinhTrang=" + TinhTrangHH + "&ID_NhomHang=" + _ID_NhomHang + "&ID_NguoiDung=" + _IDDoiTuong + "&columnsHide=" + columnHide + "&TodayBC=" + self.TodayBC() + "&TenChiNhanh=" + self.TenChiNhanh() + "&chitiet=" + _tenNhanVienBanHang;
                window.location.href = url;
            },
            statusCode: {
                404: function () {
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        });
    };

    self.LoadHangHoa_byMaHH = function (item) {
        if (item.MaHangHoa !== '' && item.MaHangHoa !== null) {
            localStorage.setItem('loadMaHang', item.MaHangHoa);
            let url = "/#/Product";
            window.open(url);
        }
    };
    self.LoadHangHoa_byMaDVDL = function (item) {
        if (item.MaDichVu !== '' && item.MaDichVu !== null) {
            localStorage.setItem('loadMaHang', item.MaDichVu);
            let url = "/#/Product";
            window.open(url);
        }
    };
    self.LoadLoHang_byMaLH = function (item) {
        localStorage.setItem('FindLoHang', item.TenLoHang);
        let url = "/#/Shipment";
        window.open(url);
    };
    self.LoadKhachHang_byMaKH = function (item) {
        if (item.MaKhachHang !== '' && item.MaKhachHang !== null) {
            localStorage.setItem('FindKhachHang', item.MaKhachHang);
            let url = "/#/Customers";
            window.open(url);
        }
    };
    self.LoadNhaCungCap_byMaNCC = function (item) {
        if (item.MaNhaCungCap !== '' && item.MaNhaCungCap !== null && item.MaNhaCungCap !== 'NCC Lẻ') {
            localStorage.setItem('FindKhachHang', item.MaNhaCungCap);
            let url = "/#/Suppliers";
            window.open(url);
        }
    };

    self.gotoDanhSachXe = function (item) {
        window.open('/#/DanhSachXe?' + item.BienSo, '_blank');
    }
    self.LoadHoaDon_byMaHD = function (item) {
        var url = '';
        if (item.MaHoaDon !== '' && item.MaHoaDon !== null && item.MaHoaDon !== 'HD trả nhanh') {
            var maHD = item.MaHoaDon;
            if (maHD.indexOf('TT') > -1) {
                localStorage.setItem('FindMaPhieuChi', maHD);
                url = "/#/CashFlow"; // soquy
            }
            else {
                localStorage.setItem('FindHD', maHD);
                if (maHD.indexOf('HD') > -1) {
                    url = "/#/Invoices";
                }
                else if (maHD.indexOf('GDV') > -1) {
                    url = "/#/ServicePackage";
                }
                else if (maHD.indexOf('PNK') > -1) {
                    url = "/#/PurchaseOrder";
                }
                else if (maHD.indexOf('THN') > -1) {
                    url = "/#/PurchaseReturns";
                }
                else if (maHD.indexOf('CH') > -1) {
                    url = "/#/Transfers";
                }
                else if (maHD.indexOf('PKK') > -1) {
                    url = "/#/StockTakes";
                }
                else if (maHD.indexOf('XH') > -1) {
                    url = "/#/DamageItems";
                }
                else {
                    if (maHD.indexOf('TH') > -1) {
                        url = "/#/Returns"; // trahang
                    }
                    else {
                        url = "/#/Order"; // dathang
                    }
                }
            }
            window.open(url);
        }
    };

    //Phân trang
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return page.SoTrang === self.currentPage() ? "click" : "";
    };
    self.selecPage = function () {
        self.SumNumberPageReport([]);
        if (AllPage > 4) {
            self.SumNumberPageReport.push({ SoTrang: 1 });
            self.SumNumberPageReport.push({ SoTrang: 2 });
            self.SumNumberPageReport.push({ SoTrang: 3 });
            self.SumNumberPageReport.push({ SoTrang: 4 });
            self.SumNumberPageReport.push({ SoTrang: 5 });
        }
        else {
            for (var j = 0; j < AllPage; j++) {
                self.SumNumberPageReport.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage').hide();
        $('#BackPage').hide();
        $('#NextPage').show();
        $('#EndPage').show();
    };
    self.ReserPage = function (item) {
        loadHtmlGrid();
        self.SumNumberPageReport([]);
        if (AllPage > 5) {
            if (_pageNumber > 2 && _pageNumber < (AllPage - 2)) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.push({ SoTrang: _pageNumber - 2 + i });
                }
            }
            else if (_pageNumber >= (AllPage - 2)) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.push({ SoTrang: AllPage - 4 + i });
                }
            }
            else {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.push({ SoTrang: 1 + i });
                }
            }
        }
        else {
            if (AllPage !== 0) {
                for (let i = 0; i < AllPage; i++) {
                    self.SumNumberPageReport.push({ SoTrang: 1 + i });
                }
            }
        }

        if (self.SumNumberPageReport().length > 0) {
            if (self.SumNumberPageReport()[0].SoTrang > 1) {
                $('#StartPage').show();
                $('#BackPage').show();
            }
            else {
                $('#StartPage').hide();
                $('#BackPage').hide();
            }
            if (self.SumNumberPageReport()[self.SumNumberPageReport().length - 1].SoTrang < AllPage) {
                $('#NextPage').show();
                $('#EndPage').show();
            }
            else {
                $('#NextPage').hide();
                $('#EndPage').hide();
            }
        }

        self.currentPage(parseInt(_pageNumber));
    };
    self.NextPage = function (item) {
        if (_pageNumber < AllPage) {
            _pageNumber = _pageNumber + 1;
            if (self.check_MoiQuanTam() === 1)
                self.pageNumber_TK(_pageNumber);
            else if (self.check_MoiQuanTam() === 2)
                self.pageNumber_NXT(_pageNumber);
            else if (self.check_MoiQuanTam() === 3)
                self.pageNumber_NXTCT(_pageNumber);
            else if (self.check_MoiQuanTam() === 4) {
                if (dk_tab === 1)
                    if (_selectTab === 1)
                        self.pageNumber_TQXH(_pageNumber);
                    else
                        self.pageNumber_CTDC(_pageNumber);
                else
                    if (_selectTab === 1)
                        self.pageNumber_TQNH(_pageNumber);
                    else
                        self.pageNumber_CTDC(_pageNumber);
            }
            else if (self.check_MoiQuanTam() === 5) {
                if (dk_tabtk === 1)
                    self.pageNumber_THNHH(_pageNumber);
                else
                    self.pageNumber_THNGD(_pageNumber);
            }

            else if (self.check_MoiQuanTam() === 6) {
                if (dk_tabxk === 1)
                    self.pageNumber_THXHH(_pageNumber);
                else if (dk_tabxk === 2)
                    self.pageNumber_THXGD(_pageNumber);
                else
                    self.pageNumber_XDVDL(_pageNumber);
            }
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            if (self.check_MoiQuanTam() === 1)
                self.pageNumber_TK(_pageNumber);
            else if (self.check_MoiQuanTam() === 2)
                self.pageNumber_NXT(_pageNumber);
            else if (self.check_MoiQuanTam() === 3)
                self.pageNumber_NXTCT(_pageNumber);
            else if (self.check_MoiQuanTam() === 4) {
                if (dk_tab === 1)
                    if (_selectTab === 1)
                        self.pageNumber_TQXH(_pageNumber);
                    else
                        self.pageNumber_CTDC(_pageNumber);
                else
                    if (_selectTab === 1)
                        self.pageNumber_TQNH(_pageNumber);
                    else
                        self.pageNumber_CTDC(_pageNumber);
            }
            else if (self.check_MoiQuanTam() === 5) {
                if (dk_tabtk === 1)
                    self.pageNumber_THNHH(_pageNumber);
                else
                    self.pageNumber_THNGD(_pageNumber);
            }

            else if (self.check_MoiQuanTam() === 6) {
                if (dk_tabxk === 1)
                    self.pageNumber_THXHH(_pageNumber);
                else if (dk_tabxk === 2)
                    self.pageNumber_THXGD(_pageNumber);
                else
                    self.pageNumber_XDVDL(_pageNumber);
            }
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        if (self.check_MoiQuanTam() === 1)
            self.pageNumber_TK(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            self.pageNumber_NXT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_NXTCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 4) {
            if (dk_tab === 1)
                if (_selectTab === 1)
                    self.pageNumber_TQXH(_pageNumber);
                else
                    self.pageNumber_CTDC(_pageNumber);
            else
                if (_selectTab === 1)
                    self.pageNumber_TQNH(_pageNumber);
                else
                    self.pageNumber_CTDC(_pageNumber);
        }
        else if (self.check_MoiQuanTam() === 5) {
            if (dk_tabtk === 1)
                self.pageNumber_THNHH(_pageNumber);
            else
                self.pageNumber_THNGD(_pageNumber);
        }

        else if (self.check_MoiQuanTam() === 6) {
            if (dk_tabxk === 1)
                self.pageNumber_THXHH(_pageNumber);
            else if (dk_tabxk === 2)
                self.pageNumber_THXGD(_pageNumber);
            else
                self.pageNumber_XDVDL(_pageNumber);
        }
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        if (self.check_MoiQuanTam() === 1)
            self.pageNumber_TK(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            self.pageNumber_NXT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_NXTCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 4) {
            if (dk_tab === 1)
                if (_selectTab === 1)
                    self.pageNumber_TQXH(_pageNumber);
                else
                    self.pageNumber_CTDC(_pageNumber);
            else
                if (_selectTab === 1)
                    self.pageNumber_TQNH(_pageNumber);
                else
                    self.pageNumber_CTDC(_pageNumber);
        }
        else if (self.check_MoiQuanTam() === 5) {
            if (dk_tabtk === 1)
                self.pageNumber_THNHH(_pageNumber);
            else
                self.pageNumber_THNGD(_pageNumber);
        }

        else if (self.check_MoiQuanTam() === 6) {
            if (dk_tabxk === 1)
                self.pageNumber_THXHH(_pageNumber);
            else if (dk_tabxk === 2)
                self.pageNumber_THXGD(_pageNumber);
            else
                self.pageNumber_XDVDL(_pageNumber);
        }
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        if (self.check_MoiQuanTam() === 1)
            self.pageNumber_TK(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            self.pageNumber_NXT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_NXTCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 4) {
            if (dk_tab === 1)
                if (_selectTab === 1)
                    self.pageNumber_TQXH(_pageNumber);
                else
                    self.pageNumber_CTDC(_pageNumber);
            else
                if (_selectTab === 1)
                    self.pageNumber_TQNH(_pageNumber);
                else
                    self.pageNumber_CTDC(_pageNumber);
        }
        else if (self.check_MoiQuanTam() === 5) {
            if (dk_tabtk === 1)
                self.pageNumber_THNHH(_pageNumber);
            else
                self.pageNumber_THNGD(_pageNumber);
        }

        else if (self.check_MoiQuanTam() === 6) {
            if (dk_tabxk === 1)
                self.pageNumber_THXHH(_pageNumber);
            else if (dk_tabxk === 2)
                self.pageNumber_THXGD(_pageNumber);
            else
                self.pageNumber_XDVDL(_pageNumber);
        }
        self.ReserPage();
    };

    self.ResetCurrentPage = function () {
        _pageNumber = 1;
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                if (tab_TonKho === 1) {
                    self.pageNumber_TK(1);
                    AllPage = Math.ceil(self.BaoCaoKho_TonKho().length / self.pageSize());
                }
                else {
                    self.pageNumber_TK_TH(1);
                    AllPage = Math.ceil(self.BaoCaoKho_TongKho_TongHop().length / self.pageSize());
                }
                break;
            case 2:
                self.pageNumber_NXT(1);
                AllPage = Math.ceil(self.BaoCaoKho_NhapXuatTon().length / self.pageSize());
                break;
            case 3:
                self.pageNumber_NXTCT(1);
                AllPage = Math.ceil(self.BaoCaoKho_NhapXuatTonChiTiet().length / self.pageSize());
                break;
            case 4:
                if (dk_tab === 1) {
                    if (_selectTab === 1) {
                        self.pageNumber_TQXH(1);
                        AllPage = Math.ceil(self.BaoCaoKho_XuatDieuChuyen().length / self.pageSize());
                    }
                    else {
                        self.pageNumber_CTDC(1);
                        AllPage = Math.ceil(self.BaoCaoKho_DieuChuyenChiTiet().length / self.pageSize());
                    }
                }
                else {
                    if (dk_tab === 1) {
                        self.pageNumber_TQNH(1);
                        AllPage = Math.ceil(self.BaoCaoKho_NhapDieuChuyen().length / self.pageSize());
                    }
                    else {
                        self.pageNumber_CTDC(1);
                        AllPage = Math.ceil(self.BaoCaoKho_DieuChuyenChiTiet().length / self.pageSize());
                    }
                }
                break;
            case 5:
                if (dk_tabtk === 1) {
                    self.pageNumber_THNHH(1);
                    AllPage = Math.ceil(self.BaoCaoKho_TongHopHangNhap().length / self.pageSize());
                }
                else {
                    self.pageNumber_THNGD(1);
                    AllPage = Math.ceil(self.BaoCaoKho_ChiTietHangNhap().length / self.pageSize());
                }
                break;
            case 6:
                switch (dk_tabxk) {
                    case 1:
                        self.pageNumber_THXHH(1);
                        AllPage = Math.ceil(self.BaoCaoKho_TongHopHangXuat().length / self.pageSize());
                        break;
                    case 2:
                        self.pageNumber_THXGD(1);
                        AllPage = Math.ceil(self.BaoCaoKho_ChiTietHangXuat().length / self.pageSize());
                        break;
                    case 3:
                        self.pageNumber_XDVDL(1);
                        AllPage = Math.ceil(self.BaoCaoKho_XuatDichVuDinhLuong().length / self.pageSize());
                        break;
                }
                break;
        }
        self.ReserPage();
    };
};
var reportWarehouse = new ViewModal();
ko.applyBindings(reportWarehouse);

$('#selec-all-DonVi').parent().on('hide.bs.dropdown', function () {
    reportWarehouse.LoadReport();
});