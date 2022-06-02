var ViewModal = function () {
    var self = this;
    var _rdTime = 1;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/'
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.MoiQuanTam = ko.observable('Báo cáo tổng hợp hoa hồng nhân viên');
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.LoaiBaoCao = ko.observable('hàng hóa')
    self.TenNVPrint = ko.observable();
    var dt1 = new Date();
    var currentWeekDay1 = dt1.getDay();
    var lessDays1 = currentWeekDay1 == 0 ? 6 : currentWeekDay1 - 1
    var _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - lessDays1))).format('YYYY-MM-DD'); // start of wwek
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 7))).format('YYYY-MM-DD'); // end of week
    var dtBC = new Date(_timeEnd);
    var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
    self.TodayBC = ko.observable('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
    self.check_MoiQuanTam = ko.observable(1);
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.NhomHangHoas = ko.observableArray();
    var _pageNumber = 1;
    var _pageSize = 10;
    self.SumRowsHangHoa = ko.observable();
    self.pageSize = ko.observable(10);
    var AllPage;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    self.pageNumber_TH = ko.observable(1);
    self.pageNumber_CT = ko.observable(1);
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    $('.ip_TimeReport').val("Tuần này");
    var tk = null;
    // TuanDL Cache Show Hide Column Grid
    self.listCheckbox = ko.observableArray();
    self.columnCheckType = ko.observable(1);
    var Key_Form = 'Key_ReportChietKhau';
    function loadCheckbox(type) {
        self.columnCheckType(type);
        $.getJSON("api/DanhMuc/ReportAPI/GetChecked?type=" + type + "&group=" + $('#ID_loaibaocao').val(), function (data) {
            self.listCheckbox(data);
            loadHtmlGrid();
        });
        $('.chose_kieubang li').each(function (i) {
            if (type === $(this).data('id')) {
                $(this).addClass("active");
            }
            else {
                $(this).removeClass("active");
            }
        });
        $('.tab-content .tab-pane').each(function (i) {
            if (type === $(this).data('id')) {
                $(this).addClass("active");
            }
            else {
                $(this).removeClass("active");
            }
        });
    }

    var IsLoadFirst = true;
    function loadHtmlGrid() {
        var KeyLo = Key_Form + self.columnCheckType() + "_LOHANG";
        if (IsLoadFirst) {
            $.getJSON("api/DanhMuc/ThietLapApi/CheckQuanLyLo", function (data) {
                var current = localStorage.getItem(KeyLo);
                if (data.toString() !== current) {
                    localStorage.removeItem(Key_Form + self.columnCheckType());
                    if (data.toString() === "false") {
                        $('#select-column .dropdown-list ul li').each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("lohang") >= 0) {
                                LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
                            }
                        });
                    }
                    LocalCaches.RemoveLoHang(Key_Form);
                    localStorage.setItem(LocalCaches.KeyQuanLyLo, data);
                    localStorage.setItem(KeyLo, data);
                }
                LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
                $('.table-reponsive').css('display', 'block');
                IsLoadFirst = false;
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
                    if (current === "false") {
                        $('#select-column .dropdown-list ul li').each(function (i) {
                            var valueCheck = $(this).find('input[type = checkbox]').val();
                            if (valueCheck.toLowerCase().indexOf("lohang") >= 0) {
                                LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
                            }
                        });
                    }
                    localStorage.setItem(KeyLo, current);
                }
            }
            LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
            $('.table-reponsive').css('display', 'block');

        }
    }
    loadCheckbox(1);
    $('#select-column').on('change', '.dropdown-list ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, valueCheck);
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
        LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });
    //trinhpv phân quyền
    self.BCCK_TongHop = ko.observable();
    self.BCCK_ChiTiet = ko.observable();
    self.BCCK_XuatFile = ko.observable();
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();

    self.NhanViens = ko.observableArray();
    self.searchNhanVien = ko.observableArray();
    self.MangNhanVien = ko.observableArray();
    function getQuyen_NguoiDung() {
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCK_TongHop", "GET").done(function (data) {
        //    self.BCCK_TongHop(data);
        //    console.log(data);
        //    getDonVi();
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCK_ChiTiet", "GET").done(function (data) {
        //    self.BCCK_ChiTiet(data);
        //    console.log(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCCK_XuatFile", "GET").done(function (data) {
        //    self.BCCK_XuatFile(data);
        //    console.log(data);
        //})

        if (VHeader.Quyen.indexOf('BCCK_TongHop') > -1) {
            self.BCCK_TongHop('BCCK_TongHop');
            getDonVi();
        }
        else {
            self.BCCK_TongHop('false');
        }

        if (VHeader.Quyen.indexOf('BCCK_ChiTiet') > -1) {
            self.BCCK_ChiTiet('BCCK_ChiTiet');
        }
        else {
            self.BCCK_ChiTiet('false');
        }

        if (VHeader.Quyen.indexOf('BCCK_XuatFile') > -1) {
            self.BCCK_XuatFile('BCCK_XuatFile');
        }
        else {
            self.BCCK_XuatFile('false');
        }
    }
    getQuyen_NguoiDung();
    //load đơn vị
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameDonViSeach, "GET").done(function (data) {
            self.DonVis(data);
            self.searchDonVi(data);
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            for (var i = 0; i < self.DonVis().length; i++) {
                if (self.DonVis()[i].ID == _idDonViSeach) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                    self.SelectedDonVi(self.DonVis()[i]);
                }
            }
        });
    }
    //Lua chon don vi
    self.CloseDonVi = function (item) {
        _idDonViSeach = null;
        var TenChiNhanh;
        self.MangChiNhanh.remove(item);
        for (var i = 0; i < self.MangChiNhanh().length; i++) {
            if (_idDonViSeach == null) {
                _idDonViSeach = self.MangChiNhanh()[i].ID;
                TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
            }
            else {
                _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
                TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
            }
        }
        if (self.MangChiNhanh().length === 0) {
            $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
            TenChiNhanh = 'Tất cả chi nhánh.'
            for (var i = 0; i < self.searchDonVi().length; i++) {
                if (_idDonViSeach == null)
                    _idDonViSeach = self.searchDonVi()[i].ID;
                else
                    _idDonViSeach = self.searchDonVi()[i].ID + "," + _idDonViSeach;
            }
        }
        self.TenChiNhanh(TenChiNhanh);
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }

    self.SelectedDonVi = function (item) {
        _idDonViSeach = null;
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
                if (_idDonViSeach == null) {
                    _idDonViSeach = self.MangChiNhanh()[i].ID;
                    TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                }
                else {
                    _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
                    TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                }
            }
            self.TenChiNhanh(TenChiNhanh);
            self.LoadReport();
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

    //load nhân viên
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var _idNhanVienSeach = null;
    function getNhanVien() {
        ajaxHelper(NhanVienUri + "GetNS_NhanVien", 'GET').done(function (data) {
            self.NhanViens(data);
            self.searchNhanVien(data);
            //for (var i = 0; i < self.searchNhanVien().length; i++) {
            //    if (_idNhanVienSeach == null)
            //        _idNhanVienSeach = self.searchNhanVien()[i].ID;
            //    else
            //        _idNhanVienSeach = self.searchNhanVien()[i].ID + "," + _idNhanVienSeach;
            //}
            if (self.NhanViens().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
        });
    }
    getNhanVien();
    //Lua chon nhân viên
    self.CloseNhanVien = function (item) {
        _idNhanVienSeach = null;
        var TenChiNhanh;
        self.MangNhanVien.remove(item);
        for (var i = 0; i < self.MangNhanVien().length; i++) {
            if (_idNhanVienSeach == null) {
                _idNhanVienSeach = self.MangNhanVien()[i].ID;
                TenChiNhanh = self.MangNhanVien()[i].TenNhanVien;
            }
            else {
                _idNhanVienSeach = self.MangNhanVien()[i].ID + "," + _idNhanVienSeach;
                TenChiNhanh = TenChiNhanh + ", " + self.MangNhanVien()[i].TenNhanVien;
            }
        }
        if (self.MangNhanVien().length === 0) {
            $("#NoteNameNhanVien").attr("placeholder", "Chọn nhân viên...");
            TenChiNhanh = 'Tất cả chi nhánh.'
            //for (var i = 0; i < self.searchNhanVien().length; i++) {
            //    if (_idNhanVienSeach == null)
            //        _idNhanVienSeach = self.searchNhanVien()[i].ID;
            //    else
            //        _idNhanVienSeach = self.searchNhanVien()[i].ID + "," + _idNhanVienSeach;
            //}
        }
        self.TenChiNhanh(TenChiNhanh);
        $('#selec-all-NhanVien li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }

    self.SelectedNhanVien = function (item) {
        _idNhanVienSeach = null;
        var TenChiNhanh;
        var arrIDNhanVien = [];
        for (var i = 0; i < self.MangNhanVien().length; i++) {
            if ($.inArray(self.MangNhanVien()[i], arrIDNhanVien) === -1) {
                arrIDNhanVien.push(self.MangNhanVien()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNhanVien) === -1) {
            self.MangNhanVien.push(item);
            $('#NoteNameNhanVien').removeAttr('placeholder');
            for (var i = 0; i < self.MangNhanVien().length; i++) {
                if (_idNhanVienSeach == null) {
                    _idNhanVienSeach = self.MangNhanVien()[i].ID;
                    TenChiNhanh = self.MangNhanVien()[i].TenNhanVien;
                }
                else {
                    _idNhanVienSeach = self.MangNhanVien()[i].ID + "," + _idNhanVienSeach;
                    TenChiNhanh = TenChiNhanh + ", " + self.MangNhanVien()[i].TenNhanVien;
                }
            }
            self.TenChiNhanh(TenChiNhanh);
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-NhanVien li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //lọc nhân viên
    self.NoteNameNhanVien = function () {
        var arrNhanVien = [];
        var itemSearch = locdau($('#NoteNameNhanVien').val().toLowerCase());
        for (var i = 0; i < self.searchNhanVien().length; i++) {
            var locdauInput = locdau(self.searchNhanVien()[i].TenNhanVien).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrNhanVien.push(self.searchNhanVien()[i]);
            }
        }
        self.NhanViens(arrNhanVien);
        if ($('#NoteNameNhanVien').val() == "") {
            self.NhanViens(self.searchNhanVien());
        }
    }
    $('#NoteNameNhanVien').keypress(function (e) {
        if (e.keyCode == 13 && self.NhanViens().length > 0) {
            self.SelectedNhanVien(self.NhanViens()[0]);
        }
    });

    $('.chose_kieubang').on('click', 'li', function () {
        $('.Show_NhomKhachHang').hide();
        //$("#txt_search").val('');
        //Text_search = '';
        loadCheckbox($(this).data('id'));
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        self.check_MoiQuanTam($(this).find('a input').val());
        if (self.check_MoiQuanTam() == 1) {
            $("#txt_search").attr("placeholder", "Theo mã, tên nhân viên").blur();
            self.LoaiBaoCao('nhân viên');
            self.MoiQuanTam('Báo cáo tổng hợp hoa hồng nhân viên');
        }
        else if (self.check_MoiQuanTam() == 2) {
            $("#txt_search").attr("placeholder", "Theo mã, tên nhân viên").blur();
            self.LoaiBaoCao('hóa đơn chi tiết');
            self.MoiQuanTam('Báo cáo chi tiết hoa hồng nhân viên');
        }
        self.LoadReport();
    });
    $('.choose_txtTime li').on('click', function () {
        //self.TodayBC($(this).text())
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày đặt: Toàn thời gian');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày đặt: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            self.TodayBC('Ngày đặt: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Tuần này
        else if (_rdoNgayPage === 3) {
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() + 6))).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));

        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() - datimeBC.getDay()))).format('YYYY-MM-DD');
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            var newtime = new Date();
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate()))).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Tháng này
        else if (_rdoNgayPage === 6) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Tháng trước
        else if (_rdoNgayPage === 7) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //30 ngày qua
        else if (_rdoNgayPage === 8) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Quý này
        else if (_rdoNgayPage === 9) {
            _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        // Quý trước
        else if (_rdoNgayPage === 10) {
            var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
            _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Năm này
        else if (_rdoNgayPage === 11) {
            _timeStart = moment().startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //năm trước
        else if (_rdoNgayPage === 12) {
            var prevYear = moment().year() - 1;
            _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().year(prevYear).endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            var dtBC = new Date(_timeEnd);
            var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        _pageNumber = 1;
        self.LoadReport();
    })
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        var dtBC = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
        if (_timeStart == _timeBC)
            self.TodayBC('Ngày đặt: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.choose_TimeReport input').on('click', function () {
        console.log('a');
        _rdTime = $(this).val()
        if ($(this).val() == 1) {
            $('.ip_TimeReport').removeAttr('disabled');
            $('.dr_TimeReport').attr("data-toggle", "dropdown");
            $('.ip_DateReport').attr('disabled', 'false');
            var _rdoNgayPage = $('.ip_TimeReport').val();
            var datime = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === "Toàn thời gian") {
                _timeStart = '2015-09-26'
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày đặt: Toàn thời gian');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày đặt: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.TodayBC('Ngày đặt: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Tuần này
            else if (_rdoNgayPage === "Tuần này") {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tuần trước
            else if (_rdoNgayPage === "Tuần trước") {
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //7 ngày qua
            else if (_rdoNgayPage === "7 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tháng trước
            else if (_rdoNgayPage === "Tháng trước") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //30 ngày qua
            else if (_rdoNgayPage === "30 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Quý này
            else if (_rdoNgayPage === "Quý này") {
                _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            // Quý trước
            else if (_rdoNgayPage === "Quý trước") {
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Năm này
            else if (_rdoNgayPage === "Năm này") {
                _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //năm trước
            else if (_rdoNgayPage === "Năm trước") {
                var prevYear = moment().year() - 1;
                _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                var dtBC = new Date(_timeEnd);
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            _pageNumber = 1;
            self.LoadReport();
        }
        else if ($(this).val() == 2) {
            $('.ip_DateReport').removeAttr('disabled');
            $('.ip_TimeReport').attr('disabled', 'false');
            $('.dr_TimeReport').removeAttr('data-toggle');
            if ($('.ip_DateReport').val() != "") {
                thisDate = $('.ip_DateReport').val();
                var t = thisDate.split("-");
                var checktime1 = t[0].trim().split("/");
                var yearStart = parseInt(checktime1[2]);
                var monthStart = parseInt(checktime1[1]);
                var dayStart = parseInt(checktime1[0]);
                var checktime2 = t[1].trim().split("/");
                var yearEnd = parseInt(checktime2[2]);
                var monthEnd = parseInt(checktime2[1]);
                var dayEnd = parseInt(checktime2[0]);
                var t1 = t[0].trim().split("/").reverse().join("-")
                var thisDateStart = moment(t1).format('MM/DD/YYYY')
                var t2 = t[1].trim().split("/").reverse().join("-")
                var thisDateEnd = moment(t2).format('MM/DD/YYYY')
                _timeStart = moment(new Date(thisDateStart)).format('YYYY-MM-DD');
                var dt = new Date(thisDateEnd);
                var dtBC = new Date(thisDateEnd);
                _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
                if (_timeStart == _timeBC)
                    self.TodayBC('Ngày đặt: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    })
    //Loại hàng
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
    $('.choose_LoaiHang input').on('click', function () {
        if ($(this).val() == 1) {
            if (_ckHangHoa == 1 & _ckDichVu == 1) {
                _ckHangHoa = 0;
                _LoaiChietKhau = 0;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 1) {
                _ckHangHoa = 1;
                _LoaiChietKhau = 2;
            }
            else if (_ckHangHoa == 1 & _ckDichVu == 0) {
                _ckHangHoa = 0;
                _LoaiChietKhau = 3;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 0) {
                _ckHangHoa = 1;
                _LoaiChietKhau = 1;
            }
        }
        if ($(this).val() == 2) {
            if (_ckHangHoa == 1 & _ckDichVu == 1) {
                _ckDichVu = 0;
                _LoaiChietKhau = 1;
            }
            else if (_ckHangHoa == 1 & _ckDichVu == 0) {
                _ckDichVu = 1;
                _LoaiChietKhau = 2;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 1) {
                _ckDichVu = 0;
                _LoaiChietKhau = 3;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 0) {
                _ckDichVu = 1;
                _LoaiChietKhau = 0;
            }
        }
        _pageNumber = 1;
        self.LoadReport();
    })

    var Text_search = "";
    self.Select_Text = function () {
        Text_search = $('#txt_search').val();
    }
    $('#txt_search').keypress(function (e) {
        if (e.keyCode == 13) {
            self.LoadReport();
        }
    })
    function LoadingForm(IsShow) {
        $('.tab-show .tab-pane').each(function () {
            if ($(this).hasClass('active')) {
                var top = $(this).find('.table-reponsive').height() / 2;
                var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                $(this).find('.table-reponsive').gridLoader({ show: IsShow, style: style });
            }
        });
    }
    var _LoaiChietKhau = 2;
    self.BaoCaoChietKhau_TongHop = ko.observableArray();
    self.BaoCaoChietKhau_ChiTiet = ko.observableArray();
    self.TH_HoaHongThucHien = ko.observable();
    self.TH_HoaHongTuVan = ko.observable();
    self.TH_HoaHongBanGoiDV = ko.observable();
    self.TH_TongChietKhau = ko.observable();
    self.CT_HoaHongThucHien = ko.observable();
    self.CT_HoaHongTuVan = ko.observable();
    self.CT_HoaHongBanGoiDV = ko.observable();
    self.LoadReport = function () {
        LoadingForm(true);
        $('.table-reponsive').css('display', 'none');
        _pageNumber_CT = 1;
        self.pageNumber_TH(1);
        self.pageNumber_CT(1);
        var array_Seach = {
            MaHangHoa: Text_search,
            timeStart: _timeStart,
            timeEnd: _timeEnd,
            ID_ChiNhanh: _idDonViSeach,
            ID_NhanVien: _idNhanVienSeach,
            ThucHien_TuVan: _LoaiChietKhau,
            columnsHide: null,
            TodayBC: null,
            TenChiNhanh: null
        }
        if (self.check_MoiQuanTam() == 1) {
            if (self.BCCK_TongHop() == "BCCK_TongHop") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoChietKhau_TongHop", "POST", array_Seach).done(function (data) {
                    self.BaoCaoChietKhau_TongHop(data.LstData);
                    AllPage = data.numberPage;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.TH_HoaHongThucHien(data.a1);
                    self.TH_HoaHongTuVan(data.a2);
                    self.TH_HoaHongBanGoiDV(data.a3);
                    self.TH_TongChietKhau(data.a4);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_TongHop").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() == 2) {
            if (self.BCCK_ChiTiet() == "BCCK_ChiTiet") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoChietKhau_ChiTiet", "POST", array_Seach).done(function (data) {
                    self.BaoCaoChietKhau_ChiTiet(data.LstData);
                    AllPage = data.numberPage;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.CT_HoaHongThucHien(data.a1);
                    self.CT_HoaHongTuVan(data.a2);
                    self.CT_HoaHongBanGoiDV(data.a3);
                    LoadingForm(false);
                });
            }   
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_ChiTiet").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
    }
    self.LoadLoHang_byMaLH = function (item) {
        localStorage.setItem('FindLoHang', item.TenLoHang);
        var url = "/#/Shipment";
        window.open(url);
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
                url = "/#/Invoices"; // hoadon
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
    }
    self.BaoCaoChietKhau_TongHop_Page = ko.computed(function (x) {
        var first = (self.pageNumber_TH() - 1) * self.pageSize();
        if (self.BaoCaoChietKhau_TongHop() !== null) {
            if (self.BaoCaoChietKhau_TongHop().length != 0) {
                $('.TC_TongHop').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_TH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_TH() - 1) * self.pageSize() + self.BaoCaoChietKhau_TongHop().slice(first, first + self.pageSize()).length)
            }
            else {
                $('.TC_TongHop').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoChietKhau_TongHop().slice(first, first + _pageSize);
        }
        return null;
    })
    self.BaoCaoChietKhau_ChiTiet_Page = ko.computed(function (x) {
        var first = (self.pageNumber_CT() - 1) * self.pageSize();
        if (self.BaoCaoChietKhau_ChiTiet() !== null) {
            if (self.BaoCaoChietKhau_ChiTiet().length != 0) {
                $('.TC_ChiTiet').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_CT() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_CT() - 1) * self.pageSize() + self.BaoCaoChietKhau_ChiTiet().slice(first, first + self.pageSize()).length)
            }
            else {
                $('.TC_ChiTiet').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoChietKhau_ChiTiet().slice(first, first + _pageSize);
        }
        return null;
    })
    //Phân trang
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
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
    }
    self.ReserPage = function (item) {
        loadHtmlGrid();
        if (_pageNumber > 1 && AllPage > 5/* && nextPage < AllPage - 1*/) {
            if (_pageNumber > 3 && _pageNumber < AllPage - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 2 });
                }
            }
            else if (parseInt(_pageNumber) === parseInt(AllPage) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 3 });
                }
            }
            else if (_pageNumber == AllPage) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 4 });
                }
            }
            else if (_pageNumber < 4) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumber == 1 && AllPage > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i });
            }
        }
        if (_pageNumber > 1) {
            $('#StartPage').show();
            $('#BackPage').show();
        }
        else {
            $('#StartPage').hide();
            $('#BackPage').hide();
        }
        if (_pageNumber == AllPage) {
            $('#NextPage').hide();
            $('#EndPage').hide();
        }
        else {
            $('#NextPage').show();
            $('#EndPage').show();
        }

        self.currentPage(parseInt(_pageNumber));
    }
    self.NextPage = function (item) {
        if (_pageNumber < AllPage) {
            _pageNumber = _pageNumber + 1;
            if (self.check_MoiQuanTam() == 1)
                self.pageNumber_TH(_pageNumber);
            else if (self.check_MoiQuanTam() == 2)
                self.pageNumber_CT(_pageNumber);
            else if (self.check_MoiQuanTam() == 3)
                self.pageNumber_NH(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            if (self.check_MoiQuanTam() == 1)
                self.pageNumber_TH(_pageNumber);
            else if (self.check_MoiQuanTam() == 2)
                self.pageNumber_CT(_pageNumber);
            else if (self.check_MoiQuanTam() == 3)
                self.pageNumber_NH(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_CT(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_NH(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_CT(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_NH(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_CT(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_NH(_pageNumber);
        self.ReserPage();
    }
    //Download file excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function (item) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    }
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
        for (var i = 0; i < arrayColumn.length; i++) {
            if (i == 0) {
                columnHide = arrayColumn[i];
            }
            else {
                columnHide = arrayColumn[i] + "_" + columnHide;
            }
        }
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo bán hàng",
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
                    MaHangHoa: Text_search,
                    timeStart: _timeStart,
                    timeEnd: _timeEnd,
                    ID_ChiNhanh: _idDonViSeach,
                    ID_NhanVien: _idNhanVienSeach,
                    ThucHien_TuVan: _LoaiChietKhau,
                    columnsHide: columnHide,
                    TodayBC: self.TodayBC(),
                    TenChiNhanh: self.TenChiNhanh()
                }
                if (self.BCCK_XuatFile() != "BCCK_XuatFile") {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    LoadingForm(false);
                    return false;
                }
                if (self.check_MoiQuanTam() == 1 && self.BaoCaoChietKhau_TongHop().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCCK_TongHop",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 2 && self.BaoCaoChietKhau_ChiTiet().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCCK_ChiTiet",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
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
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                LoadingForm(false);
            },
            complete: function () {
                LoadingForm(false);
            }
        })
    }
}
ko.applyBindings(new ViewModal());