var ViewModal = function () {
    var self = this;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.LoaiBaoCao = ko.observable('gói dịch vụ');

    self.LoaiNganhNghe = ko.observable(0);
    switch (VHeader.IdNganhNgheKinhDoanh.toUpperCase()) {
        case 'C16EDDA0-F6D0-43E1-A469-844FAB143014'://gara
            self.LoaiNganhNghe(1);
            break;
        case 'C1D14B5A-6E81-4893-9F73-E11C63C8E6BC'://nhahang
            self.LoaiNganhNghe(2);
            break;
    }

    self.TenNVPrint = ko.observable();
    self.TenKHPrint = ko.observable();
    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray([_id_DonVi]);
    self.TxtSearchDV = ko.observable('');

    self.QuanLyTheoLo = ko.observable(false);
    if (VHeader.ThietLapCuaHang.length > 0) {
        self.QuanLyTheoLo(VHeader.ThietLapCuaHang[0].LoHang);
    }

    $('#soduchitiet').show();
    // TuanDL Cache Show Hide Column Grid
    self.listCheckbox = ko.observableArray();
    self.columnCheckType = ko.observable(1);
    var Key_Form = 'Key_ReportGoiDV';
    self.loadCheckbox = function (type) {
        self.columnCheckType(type);
        $.getJSON("api/DanhMuc/ReportAPI/GetChecked?type=" + self.columnCheckType() + "&group=" + $('#ID_loaibaocao').val(), function (data) {
            if (self.LoaiNganhNghe() !== 1) {
                data = data.filter(x => $.inArray(x.Key, ['bienso', 'machuxe', 'tenchuxe']) === -1)
            }
            self.listCheckbox(data);
            loadHtmlGrid();
        });
    };
    self.loadCheckbox(1);
    var IsLoad = true;
    function addcacheFirst() {

        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#ID_tonghop').val(), [{
            NameClass: "nhomkhach",
            Value: 4
        },
        {
            NameClass: "nguonkhach",
            Value: 5
        },
        {
            NameClass: "dienthoai",
            Value: 6
        },
        {
            NameClass: "gioitinh",
            Value: 7
        },
        {
            NameClass: "nguoigioithieu",
            Value: 8
        }
        ]);
        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#ID_duchitiet').val(), [{
            NameClass: "nhomkhach",
            Value: 4
        },
        {
            NameClass: "nguonkhach",
            Value: 5
        },
        {
            NameClass: "dienthoai",
            Value: 6
        },
        {
            NameClass: "gioitinh",
            Value: 7
        },
        {
            NameClass: "nguoigioithieu",
            Value: 8
        }
        ]);
        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#ID_nhatkysdct').val(), [{
            NameClass: "nhomkhach",
            Value: 2
        },
        {
            NameClass: "nguonkhach",
            Value: 3
        },
        {
            NameClass: "dienthoai",
            Value: 4
        },
        {
            NameClass: "gioitinh",
            Value: 5
        },
        {
            NameClass: "nguoigioithieu",
            Value: 6
        },
        {
            NameClass: "nhomhanghoa",
            Value: 10
        },
        {
            NameClass: "ghichu",
            Value: 15
        }
        ]);
        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#ID_nhatkysdth').val(), [{
            NameClass: "nhomkhach",
            Value: 2
        },
        {
            NameClass: "nguonkhach",
            Value: 3
        },
        {
            NameClass: "dienthoai",
            Value: 4
        },
        {
            NameClass: "gioitinh",
            Value: 5
        },
        {
            NameClass: "nguoigioithieu",
            Value: 6
        },
        {
            NameClass: "nhomhanghoa",
            Value: 7
        },
        {
            NameClass: "ghichu",
            Value: 13
        }
        ]);
    }
    function loadHtmlGrid() {
        var KeyLo = Key_Form + self.columnCheckType() + "_LOHANG";
        if (IsLoad === true) {
            $.getJSON("api/DanhMuc/ThietLapApi/CheckQuanLyLo", function (data) {

                var current = localStorage.getItem(KeyLo);
                if (data.toString() !== current) {
                    localStorage.removeItem(Key_Form + self.columnCheckType());
                    addcacheFirst();
                    if (data.toString() === "false") {
                        $('#select-column .dropdown-list ul li').each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("lohanghoa") >= 0) {
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
                LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
                $('.table-reponsive').css('display', 'block');
                IsLoad = false;


            });
        }
        else {
            var current = localStorage.getItem(LocalCaches.KeyQuanLyLo);
            var page = localStorage.getItem(KeyLo);
            if (!current) {
                IsLoadFirst = true;
                LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
            }
            else {
                if (!page || page.toString() !== current) {
                    localStorage.removeItem(Key_Form + self.columnCheckType());
                    addcacheFirst();
                    if (current === "false") {
                        $('#select-column .dropdown-list ul li').each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("lohanghoa") >= 0) {
                                LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
                            }
                        });
                    }
                    localStorage.setItem(KeyLo, current);
                }
                else {
                    LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
                }
            }
            $('.table-reponsive').css('display', 'block');
            IsLoad = false;
        }

    }
    $('#select-column').on('change', '.dropdown-list ul li input[type = checkbox]', function (i) {
        var valueCheck = $(this).val();
        LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
        $('.' + valueCheck).toggle();
    });
    $('#select-column').on('click', '.dropdown-list ul li', function (i) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
        $('.' + valueCheck).toggle();
    });
    //--- End TuanDl
    self.MoiQuanTam = ko.observable('Báo cáo tổng hợp số dư gói dịch vụ');
    var dt1 = new Date();

    var _timeStart = moment().startOf('year').format('YYYY-MM-DD');
    let newtime = new Date(moment().endOf('year'));
    var _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
    let dtBC = new Date(_timeEnd);
    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
    self.TodayBC = ko.observable('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));

    var _tonkhoEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    var _tonkhoStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    self.TodayBC_TK = ko.observable('Đến ngày: ' + moment(_tonkhoStart).format('DD/MM/YYYY'));
    self.check_MoiQuanTam = ko.observable(1);
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.SumNumberPageReport_CT = ko.observableArray();
    self.SumNumberPageReport_CTKH = ko.observableArray();
    self.RowsStart_CT = ko.observable('1');
    self.RowsEnd_CT = ko.observable('10');
    self.RowsStart_CTKH = ko.observable('1');
    self.RowsEnd_CTKH = ko.observable('10');
    var _pageNumber = 1;
    var _pageSize = 10;
    self.SumRowsHangHoa = ko.observable();
    self.SumRowsHangHoa_CT = ko.observable();
    self.SumRowsHangHoa_CTKH = ko.observable();
    self.pageSize = ko.observable(10);
    self.pageSizes = [10, 20, 30, 40, 50];
    var AllPage;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    self.pageNumber_SDTH = ko.observable(1);
    self.pageNumber_SDCT = ko.observable(1);
    self.pageNumber_NKSDTH = ko.observable(1);
    self.pageNumber_NKSDCT = ko.observable(1);
    self.pageNumber_TCSD = ko.observable(1);
    self.pageNumber_NXT = ko.observable(1);
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    $('.ip_TimeReport').val("Năm này");
    self.Loc_TinhTrangKD = ko.observable('2');
    self.Loc_HanSuDungDV = ko.observable('2');
    var tk = null;
    self.NhomHangHoas = ko.observableArray();
    //trinhpv phân quyền
    self.BCGDV_SoDuTongHop = ko.observable();
    self.BCGDV_SoDuChiTiet = ko.observable();
    self.BCGDV_NhatKySuDungChiTiet = ko.observable();
    self.BCGDV_NhatKySuDungTongHop = ko.observable();
    self.BCGDV_TonChuaSuDung = ko.observable();
    self.BCGDV_NhapXuatTon = ko.observable();
    self.BCGDV_XuatFile = ko.observable();
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    function getQuyen_NguoiDung() {
        if (VHeader.Quyen.indexOf('BCGDV_SoDuTongHop') > -1) {
            self.BCGDV_SoDuTongHop('BCGDV_SoDuTongHop');
            getDonVi();
        }
        else {
            self.BCGDV_SoDuTongHop('false');
        }

        if (VHeader.Quyen.indexOf('BCGDV_SoDuChiTiet') > -1) {
            self.BCGDV_SoDuChiTiet('BCGDV_SoDuChiTiet');
        }
        else {
            self.BCGDV_SoDuChiTiet('false');
        }

        if (VHeader.Quyen.indexOf('BCGDV_NhatKySuDungTongHop') > -1) {
            self.BCGDV_NhatKySuDungTongHop('BCGDV_NhatKySuDungTongHop');
        }
        else {
            self.BCGDV_NhatKySuDungTongHop('false');
        }

        if (VHeader.Quyen.indexOf('BCGDV_NhatKySuDungChiTiet') > -1) {
            self.BCGDV_NhatKySuDungChiTiet('BCGDV_NhatKySuDungChiTiet');
        }
        else {
            self.BCGDV_NhatKySuDungChiTiet('false');
        }

        if (VHeader.Quyen.indexOf('BCGDV_TonChuaSuDung') > -1) {
            self.BCGDV_TonChuaSuDung('BCGDV_TonChuaSuDung');
        }
        else {
            self.BCGDV_TonChuaSuDung('false');
        }

        if (VHeader.Quyen.indexOf('BCGDV_NhapXuatTon') > -1) {
            self.BCGDV_NhapXuatTon('BCGDV_NhapXuatTon');
        }
        else {
            self.BCGDV_NhapXuatTon('false');
        }

        if (VHeader.Quyen.indexOf('BCGDV_XuatFile') > -1) {
            self.BCGDV_XuatFile('BCGDV_XuatFile');
        }
        else {
            self.BCGDV_XuatFile('false');
        }
    }
    getQuyen_NguoiDung();

    // load Nhóm khách hàng
    var _tennhomDT = null;
    var _tenNhomDoiTuongSeach = null;
    self.NhomDoiTuongs = ko.observableArray();
    function getList_NhomDoiTuongs() {
        ajaxHelper(ReportUri + "GetListID_NhomDoiTuong?TenNhomDoiTuong=" + _tennhomDT + "&loaidoituong=1", "GET").done(function (data) {
            self.NhomDoiTuongs(data);
            self.searchNhomDoiTuong(data);
            for (let i = 0; i < self.NhomDoiTuongs().length; i++) {
                _tenNhomDoiTuongSeach = self.NhomDoiTuongs()[i].ID + "," + _tenNhomDoiTuongSeach;
            }
        });
    }
    getList_NhomDoiTuongs();

    self.CloseNhomDoiTuong = function (item) {
        _tenNhomDoiTuongSeach = null;
        self.MangNhomDoiTuong.remove(item);
        for (let i = 0; i < self.MangNhomDoiTuong().length; i++) {
            _tenNhomDoiTuongSeach = self.MangNhomDoiTuong()[i].ID + "," + _tenNhomDoiTuongSeach;
        }
        if (self.MangNhomDoiTuong().length === 0) {
            for (let i = 0; i < self.searchNhomDoiTuong().length; i++) {
                _tenNhomDoiTuongSeach = self.searchNhomDoiTuong()[i].ID + "," + _tenNhomDoiTuongSeach;
            }
        }
        // remove check
        $('#selec-all-NhomDoiTuong li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNhomDoiTuong());
        _pageNumber = 1;
        self.LoadReport();
    };

    self.SelectedNhomDoiTuong = function (item) {
        _tenNhomDoiTuongSeach = null;
        var arrIDNhomDoiTuong = [];
        for (let i = 0; i < self.MangNhomDoiTuong().length; i++) {
            if ($.inArray(self.MangNhomDoiTuong()[i], arrIDNhomDoiTuong) === -1) {
                arrIDNhomDoiTuong.push(self.MangNhomDoiTuong()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNhomDoiTuong) === -1) {
            self.MangNhomDoiTuong.push(item);
            for (let i = 0; i < self.MangNhomDoiTuong().length; i++) {
                _tenNhomDoiTuongSeach = self.MangNhomDoiTuong()[i].ID + "," + _tenNhomDoiTuongSeach;
            }
        }
        // sau khi tìm kiếm thì trả về mặc định
        $('#NoteNameNhomDoiTuong').val('');
        self.NhomDoiTuongs(self.searchNhomDoiTuong());
        //đánh dấu check
        for (let i = 0; i < self.MangNhomDoiTuong().length; i++) {
            $('#selec-all-NhomDoiTuong li').each(function () {
                if ($(this).attr('id') === self.MangNhomDoiTuong()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>');
                }
            });
        }
        _pageNumber = 1;
        self.LoadReport();
    };
    self.NoteNameNhomDoiTuong = function () {
        var arrNhomDoiTuong = [];
        var itemSearch = locdau($('#NoteNameNhomDoiTuong').val().toLowerCase());
        for (let i = 0; i < self.searchNhomDoiTuong().length; i++) {
            var locdau_kd = self.searchNhomDoiTuong()[i].TenNhomDoiTuong_KhongDau;
            var locdau_ktd = self.searchNhomDoiTuong()[i].TenNhomDoiTuong_KyTuDau;
            var R1 = locdau_kd.split(itemSearch);
            var R2 = locdau_ktd.split(itemSearch);
            if (R1.length > 1 || R2.length > 1) {
                arrNhomDoiTuong.push(self.searchNhomDoiTuong()[i]);
            }
        }
        self.NhomDoiTuongs(arrNhomDoiTuong);
        if ($('#NoteNameNhomDoiTuong').val() === "") {
            self.NhomDoiTuongs(self.searchNhomDoiTuong());
            // console.log(self.NhomDoiTuongs())
            for (let i = 0; i < self.MangNhomDoiTuong().length; i++) {
                $('#selec-all-NhomDoiTuong li').each(function () {
                    if ($(this).attr('id') === self.MangNhomDoiTuong()[i].ID) {
                        $(this).find('i').remove();
                        $(this).append('<i class="fa fa-check check-after-li"></i>');
                    }
                });
            }
        }
    };
    $('#NoteNameNhomDoiTuong').keypress(function (e) {
        if (e.keyCode === 13 && self.NhomDoiTuongs().length > 0) {
            self.SelectedNhomDoiTuong(self.NhomDoiTuongs()[0]);
        }
    });
    //load đơn vị
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
    //Lua chon don vi
    self.CloseDonVi = function (item) {
        _idDonViSeach = null;
        var TenChiNhanh = '';
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
        self.LstIDDonVi(arrID);
        self.TenChiNhanh(TenChiNhanh);
        self.LoadReport();
    };

    self.SelectedDonVi = function (item) {
        event.stopPropagation();
        _idDonViSeach = null;
        var TenChiNhanh = '';
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
            }
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
        event.preventDefault();
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
                        console.log(data);
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
    self.tab_SoDu = ko.observable(1);
    self.tab_NhatKySuDung = ko.observable(1);

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
    $('.chose_ThoiHanSuDung input').on('click', function () {
        ThoiHanSuDung = $(this).val();
        console.log(ThoiHanSuDung);
        _pageNumber = 1;
        self.Loc_HanSuDungDV($(this).val());
        self.LoadReport();
    });
    self.select_SoDuTongHop = function () {
        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã gói dịch vụ, mã KH, tên KH").blur();
        self.LoaiBaoCao('gói dịch vụ');
        self.MoiQuanTam('Báo cáo tổng hợp số dư gói dịch vụ');
        self.tab_SoDu(1);
        self.LoadReport();
    };
    self.select_SoDuChiTiet = function () {
        $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã gói dịch vụ, mã KH, tên KH").blur();
        self.LoaiBaoCao('hàng hóa');
        self.MoiQuanTam('Báo cáo chi tiết số dư gói dịch vụ');
        self.tab_SoDu(2);
        self.LoadReport();
    };
    self.select_NhatKyTongHop = function () {
        $("#txt_search").attr("placeholder", "Theo mã chứng từ, khách hàng").blur();
        self.LoaiBaoCao('hàng hóa');
        self.MoiQuanTam('Tổng hợp nhật ký sử dụng gói dịch vụ');
        self.tab_NhatKySuDung(1);
        self.LoadReport();
    };
    self.select_NhatKyChiTiet = function () {
        $("#txt_search").attr("placeholder", "Theo mã chứng từ, khách hàng, dịch vụ").blur();
        self.LoaiBaoCao('chi tiết gói dịch vụ');
        self.MoiQuanTam('Chi tiết nhật ký sử dụng gói dịch vụ');
        self.tab_NhatKySuDung(2);
        self.LoadReport();
    };

    function LoadFirstTab(id) {
        $('.tab-show .tab').each(function (i) {
            if ($(this).data('id') === id) {
                $(this).find('.tab-content').find(".tab-pane").each(function (i) {
                    $(this).removeClass("active");
                    if (i === 0) {
                        self.tab_SoDu(1);
                        self.tab_NhatKySuDung(1);
                        $(this).addClass("active");
                    }
                });
                $(this).find('ul').find("li").each(function (i) {
                    $(this).removeClass("active");
                    if (i === 0) {
                        $(this).addClass("active");
                    }
                });
            }
        });
    }
    $('.tab-show').on('click', '.tab ul li', function () {
        self.loadCheckbox($(this).data('id'));
    });
    $('.chose_kieubang').on('click', 'li', function () {
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        LoadFirstTab($(this).data('id'));
        var type = $(this).data('id');
        $('.tab-show .tab').each(function (i) {
            if (type === $(this).data('id')) {
                $(this).addClass("active");
            }
            else {
                $(this).removeClass("active");
            }
        });
        self.loadCheckbox($(this).data('id'));
        $('.showNameKH').show();
        $('.showDate').hide();
        $('.showDateRange').show();
        $('#showTimeTK').hide();
        $('#showTime').show();
        $(this).find('a').addClass('box-tab');
        self.check_MoiQuanTam(parseInt($(this).find('a input').val()));
        switch (self.check_MoiQuanTam()) {
            case 1:// bc sodu
                if (self.tab_SoDu() === 1) {
                    $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã gói dịch vụ, mã KH, tên KH").blur();
                    self.LoaiBaoCao('gói dịch vụ');
                    self.MoiQuanTam('Báo cáo tổng hợp số dư gói dịch vụ');
                }
                else {
                    $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã gói dịch vụ, mã KH, tên KH").blur();
                    self.LoaiBaoCao('hàng hóa');
                    self.MoiQuanTam('Báo cáo chi tiết số dư gói dịch vụ');
                }
                break;
            case 2:// bc nhatky sudung
                if (self.tab_NhatKySuDung() === 1) {
                    $("#txt_search").attr("placeholder", "Theo mã chứng từ, khách hàng").blur();
                    self.LoaiBaoCao('hàng hóa');
                    self.MoiQuanTam('Tổng hợp nhật ký sử dụng gói dịch vụ');
                }
                else {
                    $("#txt_search").attr("placeholder", "Theo mã chứng từ, khách hàng, dịch vụ").blur();
                    self.LoaiBaoCao('chi tiết gói dịch vụ');
                    self.MoiQuanTam('Chi tiết nhật ký sử dụng gói dịch vụ');
                }
                break;
            case 3:// bc ton gdv
                {
                    $('.showNameKH').hide();
                    $('.showDate').show();
                    $('.showDateRange').hide();
                    $('#showTimeTK').show();
                    $('#showTime').hide();
                    $("#txt_search").attr("placeholder", "Theo mã, tên hàng").blur();
                    self.LoaiBaoCao('hàng hóa');
                    self.MoiQuanTam('Báo cáo tồn dịch vụ chưa sử dụng');
                }
                break;
            case 4:
                {
                    $('.showNameKH').hide();
                    $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã gói dịch vụ").blur();
                    self.LoaiBaoCao('hàng hóa');
                    self.MoiQuanTam('Báo cáo nhập xuất tồn dịch vụ');
                }
                break;
        }

        _pageNumber = 1;
        self.LoadReport();
    });
    $('.choose_txtTime li').on('click', function () {
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        switch (_rdoNgayPage) {
            case 13:
                {
                    _timeStart = '2015-09-26';
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                    self.TodayBC('Ngày bán: Toàn thời gian');
                }
                break;
            case 1:
                {
                    _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                    self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
                }
                break;
            case 2:
                {
                    let dt1 = new Date();
                    let dt2 = new Date();
                    _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                    _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                    self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
                }
                break;
            case 3:
                {
                    const currentWeekDay = datime.getDay();
                    const lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                    let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() + 6))).format('YYYY-MM-DD');
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 4:
                {
                    _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                    let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() - datimeBC.getDay()))).format('YYYY-MM-DD');
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 5:
                {
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                    let newtime = new Date();
                    let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate()))).format('YYYY-MM-DD');
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 6:
                {
                    _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                    _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                    let dtBC = new Date(_timeEnd);
                    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 7:
                {
                    _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                    _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                    let dtBC = new Date(_timeEnd);
                    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 8:
                {
                    _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                    let newtime = new Date();
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                    let dtBC = new Date(_timeEnd);
                    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 9:
                {
                    _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    let newtime = new Date(moment().endOf('quarter'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                    let dtBC = new Date(_timeEnd);
                    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 10:
                {
                    const prevQuarter = moment().quarter() - 1 === 0 ? 1 : moment().quarter() - 1;
                    _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                    let newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                    let dtBC = new Date(_timeEnd);
                    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 11:
                {
                    _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                    let newtime = new Date(moment().endOf('year'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                    let dtBC = new Date(_timeEnd);
                    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
            case 12:
                {
                    const prevYear = moment().year() - 1;
                    _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    let newtime = new Date(moment().year(prevYear).endOf('year'));
                    _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                    let dtBC = new Date(_timeEnd);
                    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                }
                break;
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
            self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        _pageNumber = 1;
        self.LoadReport();
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
            console.log(_timeEnd);
            if (thisDate !== 'Invalid date') {
                self.TodayBC_TK('Đến ngày: ' + $(this).val());
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
            self.TodayBC_TK('Đến ngày: ' + $(this).val());
            self.LoadReport();
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
        }
    });
    $('.choose_TimeReport input').on('click', function () {
        if (parseInt($(this).val()) === 1) {
            $('.ip_TimeReport').removeAttr('disabled');
            $('.dr_TimeReport').attr("data-toggle", "dropdown");
            $('.ip_DateReport').attr('disabled', 'false');
            var _rdoNgayPage = $('.ip_TimeReport').val();
            var datime = new Date();
            switch (_rdoNgayPage) {
                case "Toàn thời gian":
                    {
                        _timeStart = '2015-09-26';
                        _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                        self.TodayBC('Ngày bán: Toàn thời gian');
                    }
                    break;
                case "Hôm qua":
                    {
                        const dt1 = new Date();
                        const dt2 = new Date();
                        _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                        _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                        self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
                    }
                    break;
                case "Hôm nay":
                    {
                        _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                        _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                        self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
                    }
                    break;
                case "Tuần này":
                    {
                        const currentWeekDay = datime.getDay();
                        const lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
                        _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                        _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "Tuần trước":
                    {
                        _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                        _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "7 ngày qua":
                    {
                        _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                        let newtime = new Date();
                        _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "Tháng này":
                    {
                        _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                        _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "Tháng trước":
                    {
                        _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                        _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "30 ngày qua":
                    {
                        _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                        let newtime = new Date();
                        _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "Quý này":
                    {
                        _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                        let newtime = new Date(moment().endOf('quarter'));
                        _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "Quý trước":
                    {
                        const prevQuarter = moment().quarter() - 1 === 0 ? 1 : moment().quarter() - 1;
                        _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        let newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                        _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "Năm này":
                    {
                        _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                        let newtime = new Date(moment().endOf('year'));
                        _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
                case "Năm trước":
                    {
                        const prevYear = moment().year() - 1;
                        _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                        let newtime = new Date(moment().year(prevYear).endOf('year'));
                        _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                        let dtBC = new Date(_timeEnd);
                        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                        self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                    }
                    break;
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
                    self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    });
    var Text_search = "";
    //var _magioithieu = "";
    //var _maquanly = "";
    var _laHangHoa = 2;
    var TinhTrangHH = 2;
    var ThoiHanSuDung = 2;
    var _ID_NhomHang = null;
    self.BaoCaoGoiDichVu_SoDuTongHop = ko.observableArray();
    self.BaoCaoGoiDichVu_SoDuChiTiet = ko.observableArray();
    self.BaoCaoGoiDichVu_NhatKySuDungTongHop = ko.observableArray();
    self.BaoCaoGoiDichVu_NhatKySuDungChiTiet = ko.observableArray();
    self.BaoCaoGoiDichVu_TonChuaSuDung = ko.observableArray();
    self.BaoCaoGoiDichVu_NhapXuatTon = ko.observableArray();
    self.SDTH_SoLuong = ko.observable();
    self.SDTH_ThanhTien = ko.observable();
    self.SDTH_SoLuongTra = ko.observable();
    self.SDTH_GiaTriTra = ko.observable();
    self.SDTH_SoLuongSuDung = ko.observable();
    self.SDTH_GiaVon = ko.observable();
    self.SDTH_SoLuongConLai = ko.observable();

    self.SDCT_SoLuong = ko.observable();
    self.SDCT_ThanhTien = ko.observable();
    self.SDCT_ThanhTienChuaCK = ko.observable();
    self.SDCT_SoLuongTra = ko.observable();
    self.SDCT_GiaTriTra = ko.observable();
    self.SDCT_GiamGiaHD = ko.observable();
    self.SDCT_SoLuongSuDung = ko.observable();
    self.SDCT_SoLuongConLai = ko.observable();
    self.SDCT_GiaVon = ko.observable();
    self.NKTH_SoLuong = ko.observable();
    self.NKTH_SoLuongSuDung = ko.observable();
    self.NKTH_SoLuongTra = ko.observable();
    self.NKTH_SoLuongConLai = ko.observable();

    self.NKCT_SoLuong = ko.observable();
    self.NKCT_SoLuongSuDung = ko.observable();
    self.NKCT_SoLuongConLai = ko.observable();

    self.TCSD_SoLuong = ko.observable();
    self.TCSD_GiaTri = ko.observable();
    self.TCSD_SoLuongTra = ko.observable();
    self.TCSD_GiaTriTra = ko.observable();
    self.TCSD_SoLuongSuDung = ko.observable();
    self.TCSD_GiaTriSuDung = ko.observable();
    self.TCSD_SoLuongConLai = ko.observable();
    self.TCSD_GiaTriConLai = ko.observable();

    self.NXT_SoLuongConLaiDK = ko.observable();
    self.NXT_GiaTriConLaiDK = ko.observable();
    self.NXT_SoLuongBanGK = ko.observable();
    self.NXT_GiaTriBanGK = ko.observable();
    self.NXT_SoLuongTraGK = ko.observable();
    self.NXT_GiaTriTraGK = ko.observable();
    self.NXT_SoLuongSuDungGK = ko.observable();
    self.NXT_GiaTriSuDungGK = ko.observable();
    self.NXT_SoLuongConLaiCK = ko.observable();
    self.NXT_GiaTriConLaiCK = ko.observable();

    function LoadingForm(IsShow) {
        $('.tab-show .tab').each(function () {
            if ($(this).hasClass('active')) {
                if ($(this).find('.table-reponsive').length > 1) {
                    $(this).find('.tab-content').find('.tab-pane').each(function () {
                        if ($(this).hasClass('active')) {
                            var top = $(this).find('.table-reponsive').height() / 2;
                            var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                            $(this).find('.table-reponsive').gridLoader({ show: IsShow, style: style });
                        }
                    });
                }
                else {

                    var top = $(this).find('.table-reponsive').height() / 2;
                    var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                    $(this).find('.table-reponsive').gridLoader({ show: IsShow, style: style });
                }
            }

        });
    }
    self.LoadReport = function () {
        LoadingForm(true);
        $('.table-reponsive').css('display', 'none');
        _pageNumber = 1;
        self.pageNumber_SDTH(1);
        self.pageNumber_SDCT(1);
        self.pageNumber_NKSDTH(1);
        self.pageNumber_NKSDCT(1);
        self.pageNumber_TCSD(1);
        self.pageNumber_NXT(1);

        if (!commonStatisJs.CheckNull(Text_search)) {
            Text_search = Text_search.trim();
        }
        var array_Seach = {
            MaHangHoa: Text_search,
            timeStart: self.check_MoiQuanTam() !== 3 ? _timeStart : _tonkhoStart,
            timeEnd: self.check_MoiQuanTam() !== 3 ? _timeEnd : _tonkhoEnd,
            ID_ChiNhanh: _idDonViSeach,
            LaHangHoa: _laHangHoa,
            TinhTrang: TinhTrangHH,
            ThoiHanSuDung: ThoiHanSuDung,
            ID_NhomHang: _ID_NhomHang,
            ID_NguoiDung: _IDDoiTuong,
            columnsHide: null,
            TodayBC: null,
            TenChiNhanh: null,
            lstIDChiNhanh: self.LstIDDonVi(),
        };
        console.log('array_Seach ', array_Seach)

        switch (self.check_MoiQuanTam()) {
            case 1:
                {
                    if (self.tab_SoDu() === 1) {
                        if (self.BCGDV_SoDuTongHop() === "BCGDV_SoDuTongHop") {
                            $(".PhanQuyen").hide();
                            ajaxHelper(ReportUri + "BaoCaoDichVu_SoDuTongHop", "POST", array_Seach).done(function (data) {
                                self.BaoCaoGoiDichVu_SoDuTongHop(data.LstData);
                                AllPage = data.numberPage;
                                self.ResetCurrentPage();
                                self.SumRowsHangHoa(data.Rowcount);
                                self.SDTH_SoLuong(data.a1);
                                self.SDTH_ThanhTien(data.a2);
                                self.SDTH_SoLuongTra(data.a3);
                                self.SDTH_GiaTriTra(data.a4);
                                self.SDTH_SoLuongSuDung(data.a5);
                                self.SDTH_GiaVon(data.a6);
                                self.SDTH_SoLuongConLai(data.a7);
                                LoadingForm(false);
                            });
                        }
                        else {
                            $(".PhanQuyen").show();
                            $(".Report_Empty").hide();
                            $(".SD_TongHop").hide();
                            $(".page").hide();
                            LoadingForm(false);
                        }
                    }
                    else {
                        if (self.BCGDV_SoDuChiTiet() === "BCGDV_SoDuChiTiet") {
                            $(".PhanQuyen").hide();
                            ajaxHelper(ReportUri + "BaoCaoDichVu_SoDuChiTiet", "POST", array_Seach).done(function (data) {
                                self.BaoCaoGoiDichVu_SoDuChiTiet(data.LstData);
                                AllPage = data.numberPage;
                                self.ResetCurrentPage();
                                self.SumRowsHangHoa(data.Rowcount);
                                self.SDCT_SoLuong(data.a1);
                                self.SDCT_ThanhTien(data.a2);
                                self.SDCT_GiamGiaHD(data.a3);
                                self.SDCT_SoLuongTra(data.a4);
                                self.SDCT_GiaTriTra(data.a5);
                                self.SDCT_SoLuongSuDung(data.a6);
                                self.SDCT_GiaVon(data.a7);
                                self.SDCT_SoLuongConLai(data.a8);
                                let sumTTChuaCK = data.LstData.reduce(function (x, item) {
                                    return x + item.ThanhTienChuaCK;
                                }, 0);
                                self.SDCT_ThanhTienChuaCK(sumTTChuaCK);

                                LoadingForm(false);
                            });
                        }
                        else {
                            $(".PhanQuyen").show();
                            $(".Report_Empty").hide();
                            $(".SD_ChiTiet").hide();
                            $(".page").hide();
                            LoadingForm(false);
                        }
                    }
                }
                break;
            case 2:
                {
                    let txtDV = self.TxtSearchDV();
                    if (!commonStatisJs.CheckNull(txtDV)) {
                        txtDV = txtDV.trim();
                    }
                    const param = {
                        IDChiNhanhs: array_Seach.lstIDChiNhanh,
                        DateFrom: array_Seach.timeStart,
                        DateTo: array_Seach.timeEnd,
                        TextSearch: array_Seach.MaHangHoa,
                        TxtDVMua: txtDV,
                        TxtDVDoi: '',
                        CurrentPage: 0,
                        PageSize: 10,
                    }
                    if (self.tab_NhatKySuDung() === 1) {
                        ajaxHelper(ReportUri + "BaoCaoGoiDichVu_BanDoiTra", "POST", param).done(function (data) {
                            console.log('BaoCaoGoiDichVu_BanDoiTra ', data)
                            if (data.Rowcount > 0) {
                                self.BaoCaoGoiDichVu_NhatKySuDungTongHop(data.LstData);
                            }
                            else {
                                self.BaoCaoGoiDichVu_NhatKySuDungTongHop([]);
                            }
                            AllPage = data.numberPage;
                            self.ResetCurrentPage();
                            self.SumRowsHangHoa(data.Rowcount);
                        });
                        LoadingForm(false);
                    }
                    else {
                        if (self.BCGDV_NhatKySuDungChiTiet() === "BCGDV_NhatKySuDungChiTiet") {
                            $(".PhanQuyen").hide();
                            ajaxHelper(ReportUri + "BaoCaoDichVu_NhatKySuDungChiTiet", "POST", array_Seach).done(function (data) {
                                self.BaoCaoGoiDichVu_NhatKySuDungChiTiet(data.LstData);
                                AllPage = data.numberPage;
                                self.ResetCurrentPage();
                                self.SumRowsHangHoa(data.Rowcount);
                                self.NKCT_SoLuong(data.a1);
                                self.SDTH_ThanhTien(data.TongGiaTriSD);
                                self.SDTH_GiaVon(data.TongTienVon);
                                LoadingForm(false);
                            });
                        }
                        else {
                            $(".PhanQuyen").show();
                            $(".Report_Empty").hide();
                            $(".NK_ChiTiet").hide();
                            $(".page").hide();
                            LoadingForm(false);
                        }
                    }
                }
                break;
            case 3:
                {
                    if (self.BCGDV_TonChuaSuDung() === "BCGDV_TonChuaSuDung") {
                        $(".PhanQuyen").hide();
                        ajaxHelper(ReportUri + "BaoCaoDichVu_TonChuaSuDung", "POST", array_Seach).done(function (data) {
                            self.BaoCaoGoiDichVu_TonChuaSuDung(data.LstData);
                            AllPage = data.numberPage;
                            /*self.selecPage();*/
                            self.ResetCurrentPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.TCSD_SoLuong(data.a1);
                            self.TCSD_GiaTri(data.a2);
                            self.TCSD_SoLuongTra(data.a3);
                            self.TCSD_GiaTriTra(data.a4);
                            self.TCSD_SoLuongSuDung(data.a5);
                            self.TCSD_GiaTriSuDung(data.a6);
                            self.TCSD_SoLuongConLai(data.a7);
                            self.TCSD_GiaTriConLai(data.a8);
                            LoadingForm(false);
                        });
                    }
                    else {
                        $(".PhanQuyen").show();
                        $(".Report_Empty").hide();
                        $(".DV_TonChuaSuDung").hide();
                        $(".page").hide();
                        LoadingForm(false);
                    }
                }
                break;
            case 4:
                {
                    if (self.BCGDV_NhapXuatTon() === "BCGDV_NhapXuatTon") {
                        $(".PhanQuyen").hide();
                        ajaxHelper(ReportUri + "BaoCaoDichVu_NhapXuatTon", "POST", array_Seach).done(function (data) {
                            self.BaoCaoGoiDichVu_NhapXuatTon(data.LstData);
                            AllPage = data.numberPage;
                            /*self.selecPage();*/
                            self.ResetCurrentPage();
                            self.SumRowsHangHoa(data.Rowcount);
                            self.NXT_SoLuongConLaiDK(data.a1);
                            self.NXT_GiaTriConLaiDK(data.a2);
                            self.NXT_SoLuongBanGK(data.a3);
                            self.NXT_GiaTriBanGK(data.a4);
                            self.NXT_SoLuongTraGK(data.a5);
                            self.NXT_GiaTriTraGK(data.a6);
                            self.NXT_SoLuongSuDungGK(data.a7);
                            self.NXT_GiaTriSuDungGK(data.a8);
                            self.NXT_SoLuongConLaiCK(data.a9);
                            self.NXT_GiaTriConLaiCK(data.a10);
                            LoadingForm(false);
                        });
                    }
                    else {
                        $(".PhanQuyen").show();
                        $(".Report_Empty").hide();
                        $(".DV_NhapXuatTon").hide();
                        $(".page").hide();
                        LoadingForm(false);
                    }
                }
        }
    };
    self.BaoCaoGoiDichVu_SoDuTongHop_Page = ko.computed(function (x) {
        if (self.check_MoiQuanTam() === 1 && self.tab_SoDu() === 1) {
            var first = (self.pageNumber_SDTH() - 1) * self.pageSize();
            if (self.BaoCaoGoiDichVu_SoDuTongHop() !== null) {
                if (self.BaoCaoGoiDichVu_SoDuTongHop().length !== 0) {
                    $('.SD_TongHop').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_SDTH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_SDTH() - 1) * self.pageSize() + self.BaoCaoGoiDichVu_SoDuTongHop().slice(first, first + self.pageSize()).length);
                }
                else {
                    $('.SD_TongHop').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoGoiDichVu_SoDuTongHop().slice(first, first + self.pageSize());
            }
        }
        return null;
    });
    self.BaoCaoGoiDichVu_SoDuChiTiet_Page = ko.computed(function (x) {
        var first = (self.pageNumber_SDCT() - 1) * self.pageSize();
        if (self.check_MoiQuanTam() === 1 && self.tab_SoDu() === 2) {
            if (self.BaoCaoGoiDichVu_SoDuChiTiet() !== null) {
                if (self.BaoCaoGoiDichVu_SoDuChiTiet().length !== 0) {
                    $('.SD_ChiTiet').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_SDCT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_SDCT() - 1) * self.pageSize() + self.BaoCaoGoiDichVu_SoDuChiTiet().slice(first, first + self.pageSize()).length);
                }
                else {
                    $('.SD_ChiTiet').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoGoiDichVu_SoDuChiTiet().slice(first, first + self.pageSize());
            }
        }
        return null;
    });
    self.BaoCaoGoiDichVu_NhatKySuDungTongHop_Page = ko.computed(function (x) {
        if (self.check_MoiQuanTam() === 2 && self.tab_NhatKySuDung() === 1) {
            var first = (self.pageNumber_NKSDTH() - 1) * self.pageSize();
            if (self.BaoCaoGoiDichVu_NhatKySuDungTongHop() !== null) {
                if (self.BaoCaoGoiDichVu_NhatKySuDungTongHop().length !== 0) {
                    $('.NK_TongHop').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_NKSDTH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_NKSDTH() - 1) * self.pageSize() + self.BaoCaoGoiDichVu_NhatKySuDungTongHop().slice(first, first + self.pageSize()).length);
                }
                else {
                    $('.NK_TongHop').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoGoiDichVu_NhatKySuDungTongHop().slice(first, first + self.pageSize());
            }
        }
        return null;
    });
    self.BaoCaoGoiDichVu_NhatKySuDungChiTiet_Page = ko.computed(function (x) {
        if (self.check_MoiQuanTam() === 2 && self.tab_NhatKySuDung() === 2) {
            var first = (self.pageNumber_NKSDCT() - 1) * self.pageSize();
            if (self.BaoCaoGoiDichVu_NhatKySuDungChiTiet() !== null) {
                if (self.BaoCaoGoiDichVu_NhatKySuDungChiTiet().length !== 0) {
                    $('.NK_ChiTiet').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_NKSDCT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_NKSDCT() - 1) * self.pageSize() + self.BaoCaoGoiDichVu_NhatKySuDungChiTiet().slice(first, first + self.pageSize()).length);
                }
                else {
                    $('.NK_ChiTiet').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoGoiDichVu_NhatKySuDungChiTiet().slice(first, first + self.pageSize());
            }
        }
        return null;
    });
    self.BaoCaoGoiDichVu_TonChuaSuDung_Page = ko.computed(function (x) {
        if (self.check_MoiQuanTam() === 3) {
            var first = (self.pageNumber_TCSD() - 1) * self.pageSize();
            if (self.BaoCaoGoiDichVu_TonChuaSuDung() !== null) {
                if (self.BaoCaoGoiDichVu_TonChuaSuDung().length !== 0) {
                    $('.DV_TonChuaSuDung').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_TCSD() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_TCSD() - 1) * self.pageSize() + self.BaoCaoGoiDichVu_TonChuaSuDung().slice(first, first + self.pageSize()).length);
                }
                else {
                    $('.DV_TonChuaSuDung').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoGoiDichVu_TonChuaSuDung().slice(first, first + self.pageSize());
            }
        }
        return null;
    });
    self.BaoCaoGoiDichVu_NhapXuatTon_Page = ko.computed(function (x) {
        if (self.check_MoiQuanTam() === 4) {
            var first = (self.pageNumber_NXT() - 1) * self.pageSize();
            if (self.BaoCaoGoiDichVu_NhapXuatTon() !== null) {
                if (self.BaoCaoGoiDichVu_NhapXuatTon().length !== 0) {
                    $('.DV_NhapXuatTon').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_NXT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_NXT() - 1) * self.pageSize() + self.BaoCaoGoiDichVu_NhapXuatTon().slice(first, first + self.pageSize()).length);
                }
                else {
                    $('.DV_NhapXuatTon').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoGoiDichVu_NhapXuatTon().slice(first, first + self.pageSize());
            }
        }
        return null;
    });
    //Loại hàng
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
    $('.choose_LoaiHang input').on('click', function () {
        if ($(this).val() === 1) {
            if (_ckHangHoa === 1 & _ckDichVu === 1) {
                _ckHangHoa = 0;
                _laHangHoa = 0;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 1) {
                _ckHangHoa = 1;
                _laHangHoa = 2;
            }
            else if (_ckHangHoa === 1 & _ckDichVu === 0) {
                _ckHangHoa = 0;
                _laHangHoa = 3;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 0) {
                _ckHangHoa = 1;
                _laHangHoa = 1;
            }
        }
        if ($(this).val() === 2) {
            if (_ckHangHoa === 1 & _ckDichVu === 1) {
                _ckDichVu = 0;
                _laHangHoa = 1;
            }
            else if (_ckHangHoa === 1 & _ckDichVu === 0) {
                _ckDichVu = 1;
                _laHangHoa = 2;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 1) {
                _ckDichVu = 0;
                _laHangHoa = 3;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 0) {
                _ckDichVu = 1;
                _laHangHoa = 0;
            }
        }
        _pageNumber = 1;
        self.LoadReport();
    });

    self.Select_Text = function () {
        Text_search = $('#txt_search').val();
    };
    $('#txt_search').keypress(function (e) {
        if (e.keyCode === 13) {
            self.LoadReport();
        }
    }); 
    $('#txtSearchDV').keypress(function (e) {
        if (e.keyCode === 13) {
            self.LoadReport();
        }
    });
    $('#txtMaKH').keypress(function (e) {
        if (e.keyCode === 13) {
            self.LoadReport();
        }
    });
    self.SelectMaGT = function () {
        _magioithieu = $('#txtMaGT').val();
    };
    $('#txtMaGT').keypress(function (e) {
        if (e.keyCode === 13) {
            self.LoadReport();
        }
    });
    self.SelectMaQL = function () {
        _maquanly = $('#txtMaQL').val();
    };
    $('#txtMaQL').keypress(function (e) {
        if (e.keyCode === 13) {
            self.LoadReport();
        }
    });
    // load danh sách
    self.LoadHangHoa_byMaHH = function (item) {
        if (item.MaHangHoa !== '' && item.MaHangHoa !== null) {
            localStorage.setItem('loadMaHang', item.MaHangHoa);
            var url = "/#/Product";
            window.open(url);
        }
    };
    self.LoadLoHang_byMaLH = function (item) {
        localStorage.setItem('FindLoHang', item.TenLoHang);
        var url = "/#/Shipment";
        window.open(url);
    };
    self.LoadKhachHang_byMaKH = function (item) {
        if (item.MaKhachHang !== '' && item.MaKhachHang !== null) {
            localStorage.setItem('FindKhachHang', item.MaKhachHang);
            var url = "/#/Customers";
            window.open(url);
        }
    };
    self.LoadHoaDon_byMaHD = function (item) {
        var maHD = item.MaHoaDon;
        if (maHD.indexOf('TT') > -1) {
            localStorage.setItem('FindMaPhieuChi', maHD);
            url = "/#/CashFlow"; // soquy
        }
        else {
            localStorage.setItem('FindHD', maHD);
            if (maHD.indexOf('HD') > -1) {
                if (!commonStatisJs.CheckNull(item.BienSo)) {
                    url = "/#/HoaDonSuaChua";
                }
                else {
                    url = "/#/Invoices"; // hoadon
                }
            }
            else if (maHD.indexOf('GDV') > -1) {
                url = "/#/ServicePackage"; // hoadon
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
    };
    self.LoadChungTu_byMaHD = function (item) {
        var maHD = item.MaChungTu;
        if (maHD.indexOf('TT') > -1) {
            localStorage.setItem('FindMaPhieuChi', maHD);
            url = "/#/CashFlow"; // soquy
        }
        else {
            localStorage.setItem('FindHD', maHD);
            if (maHD.indexOf('HD') > -1) {
                url = "/#/Invoices"; // hoadon
            }
            else if (maHD.indexOf('GDV') > -1) {
                url = "/#/Invoices"; // hoadon
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
    };
    self.LoadChungTuGoc_byMaHD = function (item) {
        var maHD = item.MaChungTuGoc;
        if (maHD.indexOf('TT') > -1) {
            localStorage.setItem('FindMaPhieuChi', maHD);
            url = "/#/CashFlow"; // soquy
        }
        else {
            localStorage.setItem('FindHD', maHD);
            if (maHD.indexOf('HD') > -1) {
                url = "/#/Invoices"; // hoadon
            }
            else if (maHD.indexOf('GDV') > -1) {
                url = "/#/Invoices"; // hoadon
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
    };
    self.DownloadFileTeamplateXLSX = function (item) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    };
    // xuất file Excel
    self.ExportExcel = function () {
        LoadingForm(true);
        var arrayColumn = [];
        var columnHide = null;
        $("#select-column .dropdown-list ul li").each(function (i) {
            if (!$(this).find('input').is(':checked')) {
                arrayColumn.push(i);
            }
        });
        arrayColumn.sort();
        for (let i = 0; i < arrayColumn.length; i++) {
            if (i === 0) {
                columnHide = arrayColumn[i];
            }
            else {
                columnHide = arrayColumn[i] + "_" + columnHide;
            }
        }
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo gói dịch vụ",
            NoiDung: "Xuất " + self.MoiQuanTam().toLowerCase(),
            NoiDungChiTiet: "Xuất " + self.MoiQuanTam().toLowerCase(),
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
                var array_Seach = {
                    MaHangHoa: Text_search.trim(),
                    timeStart: self.check_MoiQuanTam() !== 3 ? _timeStart : _tonkhoStart,
                    timeEnd: self.check_MoiQuanTam() !== 3 ? _timeEnd : _tonkhoEnd,
                    ID_ChiNhanh: _idDonViSeach,
                    LaHangHoa: _laHangHoa,
                    TinhTrang: TinhTrangHH,
                    ThoiHanSuDung: ThoiHanSuDung,
                    ID_NhomHang: _ID_NhomHang,
                    ID_NguoiDung: _IDDoiTuong,
                    columnsHide: columnHide,
                    TodayBC: self.TodayBC(),
                    TenChiNhanh: self.TenChiNhanh(),
                    lstIDChiNhanh: self.LstIDDonVi(),
                };
                if (self.BCGDV_XuatFile() !== "BCGDV_XuatFile") {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này", "danger");
                    LoadingForm(false);
                    return false;
                }
                if (self.check_MoiQuanTam() === 1) {
                    if (self.tab_SoDu() === 1 && self.BaoCaoGoiDichVu_SoDuTongHop().length !== 0) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCGDV_SoDuTongHop",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else if (self.tab_SoDu() === 2 && self.BaoCaoGoiDichVu_SoDuChiTiet().length !== 0) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCGDV_SoDuChiTiet",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Báo cáo không có dữ liệu", "danger");
                        LoadingForm(false);
                    }
                }
                else if (self.check_MoiQuanTam() === 2) {
                    if (self.tab_NhatKySuDung() === 1 && self.BaoCaoGoiDichVu_NhatKySuDungTongHop().length !== 0) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCGDV_NhatKySuDungTongHop",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else if (self.tab_NhatKySuDung() === 2 && self.BaoCaoGoiDichVu_NhatKySuDungChiTiet().length !== 0) {
                        if (self.LoaiNganhNghe() !== 1) {
                            let columnAfter = '0_1_2_';
                            let arr = array_Seach.columnsHide.split('_');
                            for (let i = 0; i < arr.length; i++) {
                                columnAfter += (parseInt(arr[i]) + 3) + '_';
                            }
                            array_Seach.columnsHide = columnAfter;
                        }
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCGDV_NhatKySuDungChiTiet",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else {
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Báo cáo không có dữ liệu", "danger");
                        LoadingForm(false);
                    }
                }
                else if (self.check_MoiQuanTam() === 3 && self.BaoCaoGoiDichVu_TonChuaSuDung().length !== 0) {
                    array_Seach.TodayBC = self.TodayBC_TK();
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCGDV_TonChuaSuDung",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() === 4 && self.BaoCaoGoiDichVu_NhapXuatTon().length !== 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCGDV_NhapXuatTon",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Báo cáo không có dữ liệu", "danger");
                    LoadingForm(false);
                }
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                LoadingForm(false);
            },
            complete: function () {
                LoadingForm(false);
            }
        });
    };
    // not use
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
                var url = ReportUri + "Export_BCBHCT_TheoNhanVien?ID_NhanVien=" + _idNhanVienBanHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach + "&LaHangHoa=" + _laHangHoa + "&TinhTrang=" + TinhTrangHH + "&ID_NhomHang=" + _ID_NhomHang + "&ID_NguoiDung=" + _IDDoiTuong + "&columnsHide=" + columnHide + "&TodayBC=" + self.TodayBC() + "&TenChiNhanh=" + self.TenChiNhanh() + "&chitiet=" + _tenNhanVienBanHang;
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
    // not use
    self.ExportChiTietKhachHang = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo bán hàng",
            NoiDung: "Xuất báo cáo danh sách hàng bán theo khách hàng",
            NoiDungChiTiet: "Xuất báo cáo danh sách hàng bán theo khách hàng",
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
                var url = ReportUri + "Export_BCBHCT_TheoKhachHang?ID_KhachHang=" + _idKhachHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach + "&LaHangHoa=" + _laHangHoa + "&TinhTrang=" + TinhTrangHH + "&ID_NhomHang=" + _ID_NhomHang + "&ID_NguoiDung=" + _IDDoiTuong + "&columnsHide=" + columnHide + "&TodayBC=" + self.TodayBC() + "&TenChiNhanh=" + self.TenChiNhanh() + "&chitiet=" + _tenKhachHang;
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
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return page.SoTrang === self.currentPage() ? "click" : "";
    };
    //Phân trang
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
                if (self.tab_SoDu() === 1)
                    self.pageNumber_SDTH(_pageNumber);
                else
                    self.pageNumber_SDCT(_pageNumber);
            else if (self.check_MoiQuanTam() === 2)
                if (self.tab_NhatKySuDung() === 1)
                    self.pageNumber_NKSDTH(_pageNumber);
                else
                    self.pageNumber_NKSDCT(_pageNumber);
            else if (self.check_MoiQuanTam() === 3)
                self.pageNumber_TCSD(_pageNumber);
            else if (self.check_MoiQuanTam() === 4)
                self.pageNumber_NXT(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            if (self.check_MoiQuanTam() === 1)
                if (self.tab_SoDu() === 1)
                    self.pageNumber_SDTH(_pageNumber);
                else
                    self.pageNumber_SDCT(_pageNumber);
            else if (self.check_MoiQuanTam() === 2)
                if (self.tab_NhatKySuDung() === 1)
                    self.pageNumber_NKSDTH(_pageNumber);
                else
                    self.pageNumber_NKSDCT(_pageNumber);
            else if (self.check_MoiQuanTam() === 3)
                self.pageNumber_TCSD(_pageNumber);
            else if (self.check_MoiQuanTam() === 4)
                self.pageNumber_NXT(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        if (self.check_MoiQuanTam() === 1)
            if (self.tab_SoDu() === 1)
                self.pageNumber_SDTH(_pageNumber);
            else
                self.pageNumber_SDCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            if (self.tab_NhatKySuDung() === 1)
                self.pageNumber_NKSDTH(_pageNumber);
            else
                self.pageNumber_NKSDCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_TCSD(_pageNumber);
        else if (self.check_MoiQuanTam() === 4)
            self.pageNumber_NXT(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        if (self.check_MoiQuanTam() === 1)
            if (self.tab_SoDu() === 1)
                self.pageNumber_SDTH(_pageNumber);
            else
                self.pageNumber_SDCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            if (self.tab_NhatKySuDung() === 1)
                self.pageNumber_NKSDTH(_pageNumber);
            else
                self.pageNumber_NKSDCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_TCSD(_pageNumber);
        else if (self.check_MoiQuanTam() === 4)
            self.pageNumber_NXT(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        if (self.check_MoiQuanTam() === 1)
            if (self.tab_SoDu() === 1)
                self.pageNumber_SDTH(_pageNumber);
            else
                self.pageNumber_SDCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            if (self.tab_NhatKySuDung() === 1)
                self.pageNumber_NKSDTH(_pageNumber);
            else
                self.pageNumber_NKSDCT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_TCSD(_pageNumber);
        else if (self.check_MoiQuanTam() === 4)
            self.pageNumber_NXT(_pageNumber);
        self.ReserPage();
    };

    self.ResetCurrentPage = function () {
        _pageNumber = 1;
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                if (self.tab_SoDu() === 1) {
                    self.pageNumber_SDTH(_pageNumber);
                    AllPage = Math.ceil(self.BaoCaoGoiDichVu_SoDuTongHop().length / self.pageSize());
                }
                else {
                    self.pageNumber_SDCT(_pageNumber);
                    AllPage = Math.ceil(self.BaoCaoGoiDichVu_SoDuChiTiet().length / self.pageSize());
                }

                break;
            case 2:
                if (self.tab_NhatKySuDung() === 1) {
                    self.pageNumber_NKSDTH(_pageNumber);
                    AllPage = Math.ceil(self.BaoCaoGoiDichVu_NhatKySuDungTongHop().length / self.pageSize());
                }
                else {
                    self.pageNumber_NKSDCT(_pageNumber);
                    AllPage = Math.ceil(self.BaoCaoGoiDichVu_NhatKySuDungChiTiet().length / self.pageSize());
                }
                break;
            case 3:
                self.pageNumber_TCSD(1);
                AllPage = Math.ceil(self.BaoCaoGoiDichVu_TonChuaSuDung().length / self.pageSize());
                break;
            case 4:
                self.pageNumber_NXT(1);
                AllPage = Math.ceil(self.BaoCaoGoiDichVu_NhapXuatTon().length / self.pageSize());
                break;
        }
        self.ReserPage();
    };

    self.hide = function () {
        self.loadCheckbox(self.columnCheckType());
        var strPrint = '';
        $('#table-reponsivetr .tab-pane').each(function () {
            if (!$(this).hasClass('active')) {
                $(this).hide();
            }
        });
        strPrint += document.getElementById('printtable').innerHTML;
        PrintExtraReportTr(strPrint);
        $('#table-reponsivetr .tab-pane').each(function () {
            if (!$(this).hasClass('active')) {
                $(this).show();
            }
        });
    };
    return self;
};
var data = new ViewModal();
ko.applyBindings(data);
$('#selec-all-DonVi').parent().on('hide.bs.dropdown', function () {
    data.LoadReport();
});