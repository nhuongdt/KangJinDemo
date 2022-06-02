var ViewModel = function () {
    var self = this;
    self.ColumnsExcelBHKH = ko.observableArray();
    self.ColumnsExcelCNKH = ko.observableArray();
    self.ColumnsExcelLNKH = ko.observableArray();
    self.ColumnsExcelHBKH = ko.observableArray();
    var cacheExcelBHKH = true;
    var cacheExcelLNKH = true;
    var cacheExcelCNKH = true;
    var cacheExcelHBKH = true;
    var thisDate;
    self.MangChiNhanh = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _nameDonViSeach = null;
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _IDDoiTuong = $('.idnguoidung').text();

    self.filteredDMHangHoa = ko.observableArray();
    self.MoiQuanTam = ko.observable('Báo cáo bán hàng theo khách hàng');
    var _id_DonVi = $('#hd_IDdDonVi').val();
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.TodayBC = ko.observable('Hôm nay');
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.RowsStartBanHang = ko.observable('1');
    self.RowsEndBanHang = ko.observable('10');
    self.RowsStartCongNo = ko.observable('1');
    self.RowsEndCongNo = ko.observable('10');
    self.RowsStartMuaHang = ko.observable('1');
    self.RowsEndMuaHang = ko.observable('10');
    self.SumRowsHangHoa = ko.observable();
    self.ReportKH_BanHang = ko.observableArray();
    self.ReportKH_BanHangPrint = ko.observableArray();
    self.MaKHPrint = ko.observable();
    self.TenKHPrint = ko.observable();
    self.SumDoanhThu = ko.observable();
    self.SumGiaTriTra = ko.observable();
    self.SumDoanhThuThuan = ko.observable();
    self.DoanhThuThuan = ko.observable();
    self.Loc_Table = ko.observable('1');
    self.NguoiBans = ko.observableArray();
    self.ReportKH_BanHangChiTiet = ko.observableArray();
    self.ReportKH_BanHangChiTietPrint = ko.observableArray();
    self.SumNumberPageReportBanHang = ko.observableArray();
    self.SumRowsHangHoaBanHang = ko.observable();
    var dt1 = new Date();
    var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    var _maKH = null;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var BH_KhuyenMaiUri = '/api/DanhMuc/BH_KhuyenMaiAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _id_NhanVien = $('.idnhanvien').text();
    //trinhpv phân quyền
    self.BCKhachHang = ko.observable();
    self.BCKH_BanHang = ko.observable();
    self.BCKH_BanHang_XuatFile = ko.observable();
    self.BCKH_CongNo = ko.observable();
    self.BCKH_CongNo_XuatFile = ko.observable();
    self.BCKH_HangBan = ko.observable();
    self.BCKH_HangBan_XuatFile = ko.observable();
    self.BCKH_LoiNhuan = ko.observable();
    self.BCKH_LoiNhuan_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKhachHang", "GET").done(function (data) {
            self.BCKhachHang(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_BanHang", "GET").done(function (data) {
            self.BCKH_BanHang(data);
            self.getListReportKH_BanHang();
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_BanHang_XuatFile", "GET").done(function (data) {
            self.BCKH_BanHang_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_CongNo", "GET").done(function (data) {
            self.BCKH_CongNo(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_CongNo_XuatFile", "GET").done(function (data) {
            self.BCKH_CongNo_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_HangBan", "GET").done(function (data) {
            self.BCKH_HangBan(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_HangBan_XuatFile", "GET").done(function (data) {
            self.BCKH_HangBan_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_LoiNhuan", "GET").done(function (data) {
            self.BCKH_LoiNhuan(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCKH_LoiNhuan_XuatFile", "GET").done(function (data) {
            self.BCKH_LoiNhuan_XuatFile(data);
            console.log(data);
        })
    }
    getQuyen_NguoiDung();
    var _nohientaiFrom = 0;
    var _nohientaiTo = 0;
    self.currentPage = ko.observable(1);
    self.currentPageBanHang = ko.observable(1);
    self.currentPageCongNo = ko.observable(1);
    self.currentPageMuaHang = ko.observable(1);
    $('#txtMaHH').focus();
    $('#home').removeClass("active")
    $('#info').addClass("active")
    self.check_kieubang = ko.observable('2');
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
                if (self.DonVis()[i].ID == _id_DonVi) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                    self.SelectedDonVi(self.DonVis()[i]);
                }
            }
        });
    }
    getDonVi();
    //Lua chon don vi
    self.IDSelectedDV = ko.observableArray();
    $(document).on('click', '.per_ac1 li', function () {
        var ch = $(this).index();
        $(this).remove();
        var li = document.getElementById("selec-person");
        var list = li.getElementsByTagName("li");
        for (var i = 0; i < list.length; i++) {
            $("#selec-person ul li").eq(ch).find(".fa-check").css("display", "none");
        }
        var nameDV = _idDonViSeach.split('-');
        _idDonViSeach = null;
        for (var i = 0; i < nameDV.length; i++) {
            if (nameDV[i].trim() != $(this).text().trim()) {
                if (_idDonViSeach == null) {
                    _idDonViSeach = nameDV[i];
                }
                else {
                    _idDonViSeach = nameDV[i] + "-" + _idDonViSeach;
                }
            }
        }
        console.log(_idDonViSeach);
        if (_idDonViSeach.trim() == "null") {
        }
        else {
        }

    })

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
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportKH_LoiNhuan();
        else if (_kieubang == 3)
            self.getListReportKH_CongNo();
        else if (_kieubang == 4)
            self.getListReportKH_MuaHang();
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
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListKH_BanHang_BieuDo();
                else
                    self.getListReportKH_BanHang();
            }
            else if (_kieubang == 2)
                self.getListReportKH_LoiNhuan();
            else if (_kieubang == 3)
                self.getListReportKH_CongNo();
            else if (_kieubang == 4)
                self.getListReportKH_MuaHang();
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
    $('.chose_kieubang li').on('click', function () {
        console.log($(this).val());
        if ($(this).val() == 1) {
            self.check_kieubang('1');
            $('#info').removeClass("active")
            $('#home').addClass("active")
        }
        else if ($(this).val() == 2) {
            self.check_kieubang('2');
            $('#home').removeClass("active")
            $('#info').addClass("active")
        }

        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListKH_CongNo_BieuDo();
            else
                self.getListReportKH_CongNo();
        }


    })
    self.hideTableReport = function () {
        $('.table_BanHang').hide();
        $('.table_LoiNhuan').hide();
        $('.table_CongNo').hide();
        $('.table_HangTheoKhach').hide();
    }
    self.hideTableReport();
    self.hideradio = function () {
        $('.rd_CongNo').hide();
    }
    self.hideradio();
    $('.table_BanHang').show();
    var _kieubang = 1;
    $('.chooseTableBC input').on('click', function () {
        var TenChiNhanh = null;
        if (self.MangChiNhanh().length > 0) {
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if (TenChiNhanh == null) {
                    TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                }
                else {
                    TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                }
            }
        }
        else {
            TenChiNhanh = 'Tất cả chi nhánh.'
        }
        self.TenChiNhanh(TenChiNhanh);
        $('.showChiNhanh').show();

        self.Loc_Table($(this).val())
        _kieubang = $(this).val();
        self.hideTableReport();
        _pageNumber = 1;
        if ($(this).val() == 1) {
            $('.table_BanHang').show();
            $(".list_KhBanHang").show();
            $(".list_KHLoiNhuan").hide();
            $(".list_KHCongNo").hide();
            $(".list_KHHangHoa").hide();
            $('#BieuDo').show();
            self.hideradio();
            self.MoiQuanTam('Báo cáo bán hàng theo khách hàng');
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if ($(this).val() == 2) {
            $(".list_KhBanHang").hide();
            $(".list_KHLoiNhuan").show();
            $(".list_KHCongNo").hide();
            $(".list_KHHangHoa").hide();
            $('#BieuDo').hide();
            self.check_kieubang('2');
            $('#home').removeClass("active");
            $('#info').addClass("active");
            $('.table_LoiNhuan').show();
            self.hideradio();
            self.MoiQuanTam('Báo cáo lợi nhuận theo khách hàng');
            self.getListReportKH_LoiNhuan();
        }
        else if ($(this).val() == 3) {
            $(".list_KhBanHang").hide();
            $(".list_KHLoiNhuan").hide();
            $(".list_KHCongNo").show();
            $(".list_KHHangHoa").hide();
            $('.showChiNhanh').hide();
            $('#BieuDo').show();
            $('.table_CongNo').show();
            $('.rd_CongNo').show();
            self.MoiQuanTam('Báo cáo công nợ theo khách hàng');
            for (var i = 0; i < self.searchDonVi().length; i++) {
                if (self.searchDonVi()[i].ID == _id_DonVi) {
                    self.TenChiNhanh(self.searchDonVi()[i].TenDonVi);
                }
            }
            if (self.check_kieubang() == 1)
                self.getListKH_CongNo_BieuDo();
            else
                self.getListReportKH_CongNo();
        }
        else {
            $('.table_HangTheoKhach').show();
            $(".list_KhBanHang").hide();
            $(".list_KHLoiNhuan").hide();
            $(".list_KHCongNo").hide();
            $(".list_KHHangHoa").show();
            self.check_kieubang('2');
            $('#BieuDo').hide();
            $('#home').removeClass("active");
            $('#info').addClass("active");
            self.hideradio();
            self.MoiQuanTam('Báo cáo hàng bán theo khách');
            self.getListReportKH_MuaHang();
        }
    })

    //Select time report
    var _rdTime = 1;
    $('.ip_DateReport').attr('disabled', 'false');
    //$('.ip_DateReport').addClass("StartImport");
    $('.choose_TimeReport input').on('click', function () {
        _rdTime = $(this).val()
        if ($(this).val() == 1) {
            $('.ip_TimeReport').removeAttr('disabled');
            $('.dr_TimeReport').attr("data-toggle", "dropdown");
            //$('.ip_TimeReport').removeClass("StartImport");
            $('.ip_DateReport').attr('disabled', 'false');
            // $('.ip_DateReport').addClass("StartImport");
            self.TodayBC($('.ip_TimeReport').val())
            var _rdoNgayPage = $('.ip_TimeReport').val();
            var datime = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === "Toàn thời gian") {
                _timeStart = '2015-09-26'
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            }
            //Tuần này
            else if (_rdoNgayPage === "Tuần này") {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            }
            //Tuần trước
            else if (_rdoNgayPage === "Tuần trước") {
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            }
            //7 ngày qua
            else if (_rdoNgayPage === "7 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            }
            //Tháng trước
            else if (_rdoNgayPage === "Tháng trước") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            }
            //30 ngày qua
            else if (_rdoNgayPage === "30 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Quý này
            else if (_rdoNgayPage === "Quý này") {
                _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            // Quý trước
            else if (_rdoNgayPage === "Quý trước") {
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Năm này
            else if (_rdoNgayPage === "Năm này") {
                _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //năm trước
            else if (_rdoNgayPage === "Năm trước") {
                var prevYear = moment().year() - 1;
                _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            _pageNumber = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListKH_BanHang_BieuDo();
                else
                    self.getListReportKH_BanHang();
            }
            else if (_kieubang == 2)
                self.getListReportKH_LoiNhuan();
            else if (_kieubang == 3)
                self.getListReportKH_CongNo();
            else if (_kieubang == 4)
                self.getListReportKH_MuaHang();
            self.ReserPage();
        }
        else if ($(this).val() == 2) {
            $('.ip_DateReport').removeAttr('disabled');
            $('.ip_TimeReport').attr('disabled', 'false');
            $('.dr_TimeReport').removeAttr('data-toggle');
            if ($('.ip_DateReport').val() != "") {
                thisDate = $('.ip_DateReport').val();
                var t = thisDate.split("-");
                var t1 = t[0].trim().split("/").reverse().join("-")
                var thisDateStart = moment(t1).format('MM/DD/YYYY')
                var t2 = t[1].trim().split("/").reverse().join("-")
                var thisDateEnd = moment(t2).format('MM/DD/YYYY')
                _timeStart = moment(new Date(thisDateStart)).format('YYYY-MM-DD');
                var dt = new Date(thisDateEnd);
                _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC($('.ip_DateReport').val())
                _pageNumber = 1;
                if (_kieubang == 1) {
                    if (self.check_kieubang() == 1)
                        self.getListKH_BanHang_BieuDo();
                    else
                        self.getListReportKH_BanHang();
                }
                else if (_kieubang == 2)
                    self.getListReportKH_LoiNhuan();
                else if (_kieubang == 3)
                    self.getListReportKH_CongNo();
                else if (_kieubang == 4)
                    self.getListReportKH_MuaHang();
                self.ReserPage();
            }
        }
    })
    //set value công nợ
    self.valueNoHienTaiFrom = function () {
        _nohientaiFrom = $("#valueNoHienTaiFrom").val();
        _nohientaiFrom = formatNumberToFloat(_nohientaiFrom)
        if ($("#valueNoHienTaiFrom").val() == "") {
            _nohientaiFrom = 0;
        }
    }
    $('#valueNoHienTaiFrom').keypress(function (e) {
        if (e.keyCode == 13) {
            if (_nohientaiTo != 0 & _nohientaiTo < _nohientaiFrom)
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Giá trị 'tới' không được nhỏ hơn giá trị 'từ'!", "danger");
            else {
                if (self.check_kieubang() == 1)
                    self.getListKH_CongNo_BieuDo();
                else
                    self.getListReportKH_CongNo();
            }
        }
    });
    $('#valueNoHienTaiTo').keypress(function (e) {
        if (e.keyCode == 13) {
            if (_nohientaiTo < _nohientaiFrom)
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Giá trị 'tới' không được nhỏ hơn giá trị 'từ'!", "danger");
            else {
                if (self.check_kieubang() == 1)
                    self.getListKH_CongNo_BieuDo();
                else
                    self.getListReportKH_CongNo();
            }
        }
    });
    self.valueNoHienTaiTo = function () {
        _nohientaiTo = $("#valueNoHienTaiTo").val();
        _nohientaiTo = formatNumberToFloat(_nohientaiTo);
        if ($("#valueNoHienTaiTo").val() == "") {
            _nohientaiTo = 0;
        }

    }
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        console.log(picker.startDate.format('DD/MM/YYYY'), picker.endDate.format('DD/MM/YYYY'))
        LoaiBieuDo = 2;
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        self.TodayBC($(this).val())
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportKH_LoiNhuan();
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListKH_CongNo_BieuDo();
            else
                self.getListReportKH_CongNo();
        }
        else if (_kieubang == 4)
            self.getListReportKH_MuaHang();
        self.ReserPage();
    });
    $('#txtDate').on('dp.change', function (e) {
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        //console.log(thisDate);
        _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        self.TodayBC($(this).val())
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportKH_LoiNhuan();
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListKH_CongNo_BieuDo();
            else
                self.getListReportKH_CongNo();
        }
        else if (_kieubang == 4)
            self.getListReportKH_MuaHang();
        self.ReserPage();
    });

    $('.choose_txtTime li').on('click', function () {
        self.TodayBC($(this).text())
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
        }
        //Tuần này
        else if (_rdoNgayPage === 3) {
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Tháng này
        else if (_rdoNgayPage === 6) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
        }
        //Tháng trước
        else if (_rdoNgayPage === 7) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
        }
        //30 ngày qua
        else if (_rdoNgayPage === 8) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Quý này
        else if (_rdoNgayPage === 9) {
            _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        // Quý trước
        else if (_rdoNgayPage === 10) {
            var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
            _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Năm này
        else if (_rdoNgayPage === 11) {
            _timeStart = moment().startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //năm trước
        else if (_rdoNgayPage === 12) {
            var prevYear = moment().year() - 1;
            _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().year(prevYear).endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportKH_LoiNhuan();
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListKH_CongNo_BieuDo();
            else
                self.getListReportKH_CongNo();
        }
        else if (_kieubang == 4)
            self.getListReportKH_MuaHang();
        self.ReserPage();
    })
    // Key Event maHH
    self.SelectMaKH = function () {
        _maKH = $('#txtMaHH').val();
        console.log(_maKH);
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListKH_BanHang_BieuDo();
                else
                    self.getListReportKH_BanHang();
            }
            else if (_kieubang == 2)
                self.getListReportKH_LoiNhuan();
            else if (_kieubang == 3) {
                if (self.check_kieubang() == 1)
                    self.getListKH_CongNo_BieuDo();
                else
                    self.getListReportKH_CongNo();
            }
            else if (_kieubang == 4)
                self.getListReportKH_MuaHang();
        }
    })
    self.NoteNhomHang = function () {
        var tk = $('#SeachNhomHang').val();
        console.log(tk);
        if (tk.trim() != '') {
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
                self.NhomHangHoas(data);
            })
        }
        else {
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
                self.NhomHangHoas(data);
            })
        }
    }
    self.SelectRepoert_NhomHangHoa = function (item) {
        _ID_NhomHang = item.ID;
        $('.SelectNhomHang li').each(function () {
            if ($(this).attr('id_NhomHang') === item.ID) {
                $(this).addClass('SelectReport');
            }
            else {
                $(this).removeClass('SelectReport');
            }
        });
        $('.SelectALLNhomHang li').removeClass('SelectReport');
        _pageNumber = 1;

        self.getListReportKH_LoiNhuan();
    }
    $('.SelectALLNhomHang').on('click', function () {
        $('.SelectALLNhomHang li').addClass('SelectReport')
        $('.SelectNhomHang li').each(function () {
            $(this).removeClass('SelectReport');
        });
        _ID_NhomHang = null;
        _pageNumber = 1;
        self.getListReportKH_LoiNhuan();
    });
    var _pageNumber = 1;
    var _pageSize = 10;
    var AllPage;
    var AllPageBanHang;
    var _pageNumberBanHang = 1;
    var AllPageCongNo;
    var _pageNumberCongNo = 1;
    var AllPageMuaHang;
    var _pageNumberMuaHang = 1;
    //$(".PhanQuyen").hide();
    //$(".TongCongBanHang").hide();
    //$(".page").hide();
    self.getListReportKH_BanHang = function () {
        if (self.BCKH_BanHang() == "BCKH_BanHang")
        {
            hidewait('table_h');
            $(".PhanQuyen").hide();
            ajaxHelper(ReportUri + "getListKH_BanHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportKH_BanHang(data.LstData);
                self.ReportKH_BanHangPrint(data.LstDataPrint);
                console.log(data.LstData);
                self.SumDoanhThu(data._lailo);
                self.SumGiaTriTra(data._tienvon);
                self.SumDoanhThuThuan(data._thanhtien);
                if (self.ReportKH_BanHang().length != 0) {
                    $(".page").show();
                    $(".Report_Empty").hide();
                    $('.TongCongBanHang').show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_BanHang().length)
                }
                else {
                    $(".Report_Empty").show();
                    $(".page").hide();
                    $('.TongCongBanHang').hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                LoadHtmlGrid(cacheExcelBHKH, 1, "banhangKH");
            });
        }
        else
        {
            $(".PhanQuyen").show();
            $(".Report_Empty").hide();
            $(".TongCongBanHang").hide();
            $(".page").hide();
        }
    }

    self.SelectedPageNumberReportKH_BanHang = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListKH_BanHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportKH_BanHang(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_BanHang().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelBHKH, 1, "banhangKH");
        });
    }
    self.ReportKH_BanHangChiTiet = ko.observableArray();
    self.SumNumberPageReportBanHang = ko.observableArray();
    self.SumRowsHangHoaBanHang = ko.observable();
    self.BH_SoLuongPrint = ko.observable();
    self.BH_TongTienHangPrint = ko.observable();
    self.BH_GiamGiaHDPrint = ko.observable();
    self.BH_DoanhThuPrint = ko.observable();

    var _id_KhachHang;
    var _ma_KhachHang;
    self.SelectKH_BanHangChiTiet = function (item) {
        hidewait('table_h');
        _pageNumberBanHang = 1;
        _id_KhachHang = item.ID_KhachHang;
        _ma_KhachHang = item.MaKhachHang;
        self.MaKHPrint(item.MaKhachHang);
        self.TenKHPrint(item.TenKhachHang);
        ajaxHelper(ReportUri + "getListKH_BanHangChiTiet?ID_KhachHang=" + _id_KhachHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberBanHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportKH_BanHangChiTiet(data.LstData);
            self.ReportKH_BanHangChiTietPrint(data.LstDataPrint);
            self.BH_SoLuongPrint(data.TongGiaVon);
            self.BH_TongTienHangPrint(data.TongTienHang);
            self.BH_GiamGiaHDPrint(data.GiamGiaHD);
            self.BH_DoanhThuPrint(data.DoanhThu);
            if (self.ReportKH_BanHangChiTiet().length != 0) {
                self.RowsStartBanHang((_pageNumberBanHang - 1) * _pageSize + 1);
                self.RowsEndBanHang((_pageNumberBanHang - 1) * _pageSize + self.ReportKH_BanHangChiTiet().length)
            }
            else {
                self.RowsStartBanHang('0');
                self.RowsEndBanHang('0');
            }
            self.SumNumberPageReportBanHang(data.LstPageNumber);
            AllPageBanHang = self.SumNumberPageReportBanHang().length;
            self.selecPageBanHang();
            self.ReserPageBanHang();
            self.SumRowsHangHoaBanHang(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }
    self.SelectedPageNumberReportKH_BanHangChiTiet = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListKH_BanHangChiTiet?ID_KhachHang=" + _id_KhachHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberBanHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportKH_BanHangChiTiet(data.LstData);
            self.RowsStartBanHang((_pageNumberBanHang - 1) * _pageSize + 1);
            self.RowsEndBanHang((_pageNumberBanHang - 1) * _pageSize + self.ReportKH_BanHangChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.ReportKH_LoiNhuan = ko.observableArray();
    self.ReportKH_LoiNhuanPrint = ko.observableArray();
    self.LN_TongTienHang = ko.observable();
    self.LN_GiamGiaHD = ko.observable();
    self.LN_DoanhThu = ko.observable();
    self.LN_GiaTriTra = ko.observable();
    self.LN_DoanhThuThuan = ko.observable();
    self.LN_TongGiaVon = ko.observable();
    self.LN_LoiNhuanGop = ko.observable();
    self.getListReportKH_LoiNhuan = function () {
        if (self.BCKH_LoiNhuan() == "BCKH_LoiNhuan")
        {
            hidewait('table_h');
            $(".PhanQuyen").hide();
            ajaxHelper(ReportUri + "getListKH_LoiNhuan?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + $('.idnguoidung').text(), "GET").done(function (data) {
                self.ReportKH_LoiNhuan(data.LstData);
                self.ReportKH_LoiNhuanPrint(data.LstDataPrint);
                console.log(data)
                self.LN_TongTienHang(data.TongTienHang);
                self.LN_GiamGiaHD(data.GiamGiaHD);
                self.LN_DoanhThu(data.DoanhThu);
                self.LN_GiaTriTra(data.GiaTriTra);
                self.LN_DoanhThuThuan(data.DoanhThuThuan);
                self.LN_TongGiaVon(data.TongGiaVon);
                self.LN_LoiNhuanGop(data.LoiNhuanGop);
                if (self.ReportKH_LoiNhuan().length != 0) {
                    $(".Report_Empty").hide();
                    $(".page").show();
                    $('.TongCongLoiNhuan').show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_LoiNhuan().length)
                }
                else {
                    $('.TongCongLoiNhuan').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                LoadHtmlGrid(cacheExcelLNKH, 2, "loinhuanKH");
            });
        }
        else
        {
            $(".PhanQuyen").show();
            $(".Report_Empty").hide();
            $(".TongCongLoiNhuan").hide();
            $(".page").hide();
        }
    }
    self.SelectedPageNumberReportKH_LoiNhuan = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListKH_LoiNhuan?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + $('.idnguoidung').text(), "GET").done(function (data) {
            self.ReportKH_LoiNhuan(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_LoiNhuan().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelLNKH, 2, "loinhuanKH");
        });
    }
    //Công nợ
    self.ReportKH_CongNo = ko.observableArray();
    self.ReportKH_CongNoPrint = ko.observableArray();
    self.CN_NoDauKy = ko.observable();
    self.CN_GhiNo = ko.observable();
    self.CN_GhiCo = ko.observable();
    self.CN_NoCuoiKy = ko.observable();
    self.getListReportKH_CongNo = function () {
        if (self.BCKH_CongNo() == "BCKH_CongNo")
        {
            hidewait('table_h');
            $(".PhanQuyen").hide();
            ajaxHelper(ReportUri + "getListKH_CongNo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
                self.ReportKH_CongNo(data.LstData);
                self.ReportKH_CongNoPrint(data.LstDataPrint);
                console.log(data)
                self.CN_NoDauKy(data.TongTienHang);
                self.CN_GhiNo(data.GiamGiaHD);
                self.CN_GhiCo(data.DoanhThu);
                self.CN_NoCuoiKy(data.GiaTriTra);
                if (self.ReportKH_CongNo().length != 0) {
                    $('.TongCongCongNo').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_CongNo().length)
                }
                else {
                    $('.TongCongCongNo').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                LoadHtmlGrid(cacheExcelCNKH, 3, "congnoKH");
            });
        }
        else
        {
            $(".PhanQuyen").show();
            $(".Report_Empty").hide();
            $(".TongCongCongNo").hide();
            $(".page").hide();
        }
    }

    self.SelectedPageNumberReportKH_CongNo = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListKH_CongNo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            self.ReportKH_CongNo(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_CongNo().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelCNKH, 3, "congnoKH");
        });
    }
    //chi tiết công nợ
    var _id_DoiTuongCongNo = null;
    var _ma_KhachHangCongNo;
    self.ReportKH_CongNoChiTiet = ko.observableArray();
    self.ReportKH_CongNoChiTietPrint = ko.observableArray();
    self.SumNumberPageReportCongNo = ko.observableArray();
    self.SumRowsHangHoaCongNo = ko.observable();
    self.SelectedReportKH_CongNoChiTiet = function (item) {
        self.MaKHPrint(item.MaKhachHang);
        self.TenKHPrint(item.TenKhachHang);
        _pageNumberCongNo = 1;
        hidewait('table_h');
        _id_DoiTuongCongNo = item.ID_KhachHang;
        _ma_KhachHangCongNo = item.MaKhachHang;
        ajaxHelper(ReportUri + "getListKH_CongNoChiTiet?ID_KH=" + _id_DoiTuongCongNo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberCongNo + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            self.ReportKH_CongNoChiTiet(data.LstData);
            self.ReportKH_CongNoChiTietPrint(data.LstDataPrint);
            if (self.ReportKH_CongNoChiTiet().length != 0) {
                self.RowsStartCongNo((_pageNumberCongNo - 1) * _pageSize + 1);
                self.RowsEndCongNo((_pageNumberCongNo - 1) * _pageSize + self.ReportKH_CongNoChiTiet().length)
            }
            else {
                self.RowsStartCongNo('0');
                self.RowsEndCongNo('0');
            }
            self.SumNumberPageReportCongNo(data.LstPageNumber);
            AllPageCongNo = self.SumNumberPageReportCongNo().length;
            self.selecPageCongNo();
            self.ReserPageCongNo();
            self.SumRowsHangHoaCongNo(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }

    self.SelectedPageNumberReportKH_CongNoChiTiet = function (item) {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListKH_CongNoChiTiet?ID_KH=" + _id_DoiTuongCongNo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberCongNo + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            self.ReportKH_CongNoChiTiet(data.LstData);
            self.RowsStartCongNo((_pageNumberCongNo - 1) * _pageSize + 1);
            self.RowsEndCongNo((_pageNumberCongNo - 1) * _pageSize + self.ReportKH_CongNoChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.ReportKH_MuaHang = ko.observableArray();
    self.ReportKH_MuaHangPrint = ko.observableArray();
    self.MH_TongTichDiem = ko.observable();
    self.MH_GiaTriMua = ko.observable();
    self.MH_GiaTriThuan = ko.observable();
    self.MH_GiaTriTra = ko.observable();
    self.MH_SoLuongMua = ko.observable();
    self.MH_SoLuongTra = ko.observable();
    self.getListReportKH_MuaHang = function () {
        if (self.BCKH_HangBan() == "BCKH_HangBan")
        {
            hidewait('table_h');
            $(".PhanQuyen").hide();
            ajaxHelper(ReportUri + "getListKH_MuaHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportKH_MuaHang(data.LstData);
                self.ReportKH_MuaHangPrint(data.LstDataPrint);
                self.MH_SoLuongMua(data.TongTienHang);
                self.MH_GiaTriMua(data.DoanhThu);
                self.MH_SoLuongTra(data.TongGiaVon);
                self.MH_GiaTriTra(data.GiaTriTra);
                self.MH_GiaTriThuan(data.DoanhThuThuan);
                self.MH_TongTichDiem(data.GiamGiaHD);
                if (self.ReportKH_MuaHang().length != 0) {
                    $('.TongCongMuaHang').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_MuaHang().length)
                }
                else {
                    $(".Report_Empty").show();
                    $('.TongCongMuaHang').hide();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");                  
                LoadHtmlGrid(cacheExcelHBKH, 4, "hangbanKH");
            });
        }
        else
        {
            $(".PhanQuyen").show();
            $(".TongCongMuaHang").hide();
            $(".Report_Empty").hide();
            $(".page").hide();
        }
    }
    self.SelectedPageNumberReportKH_MuaHang = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListKH_MuaHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportKH_MuaHang(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportKH_MuaHang().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelHBKH, 4, "hangbanKH");
        });
    }
    //chi tiết mua hàng
    var _id_DoiTuongMuaHang = null;
    var _ma_KhachHangMuaHang;
    self.ReportKH_MuaHangChiTiet = ko.observableArray();
    self.ReportKH_MuaHangChiTietPrint = ko.observableArray();
    self.SumNumberPageReportMuaHang = ko.observableArray();
    self.SumRowsHangHoaMuaHang = ko.observable();

    self.MH_SoLuongMuaPrint = ko.observable();
    self.MH_GiaTriMuaPrint = ko.observable();
    self.MH_SoLuongTraPrint = ko.observable();
    self.MH_GiaTriTraPrint = ko.observable();
    self.MH_GiaTriThuanPrint = ko.observable();

    self.SelectedReportKH_MuaHangChiTiet = function (item) {
        self.MH_SoLuongMuaPrint(item.SoLuongMua);
        self.MH_GiaTriMuaPrint(item.GiaTriMua);
        self.MH_SoLuongTraPrint(item.SoLuongTra);
        self.MH_GiaTriTraPrint(item.GiaTriTra);
        self.MH_GiaTriThuanPrint(item.GiaTriThuan);
        self.MaKHPrint(item.MaKhachHang);
        self.TenKHPrint(item.TenKhachHang);
        _pageNumberMuaHang = 1;
        hidewait('table_h');
        _id_DoiTuongMuaHang = item.ID_KhachHang;
        _ma_KhachHangMuaHang = item.MaKhachHang;
        ajaxHelper(ReportUri + "getListKH_MuaHangChiTiet?ID_KhachHang=" + _id_DoiTuongMuaHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberMuaHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportKH_MuaHangChiTiet(data.LstData);
            self.ReportKH_MuaHangChiTietPrint(data.LstDataPrint);
            if (self.ReportKH_MuaHangChiTiet().length != 0) {
                self.RowsStartMuaHang((_pageNumberMuaHang - 1) * _pageSize + 1);
                self.RowsEndMuaHang((_pageNumberMuaHang - 1) * _pageSize + self.ReportKH_MuaHangChiTiet().length)
            }
            else {
                self.RowsStartMuaHang('0');
                self.RowsEndMuaHang('0');
            }
            self.SumNumberPageReportMuaHang(data.LstPageNumber);
            AllPageMuaHang = self.SumNumberPageReportMuaHang().length;
            self.selecPageMuaHang();
            self.ReserPageMuaHang();
            self.SumRowsHangHoaMuaHang(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }

    self.SelectedPageNumberReportKH_MuaHangChiTiet = function (item) {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListKH_MuaHangChiTiet?ID_KhachHang=" + _id_DoiTuongMuaHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberMuaHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportKH_MuaHangChiTiet(data.LstData);
            self.RowsStartMuaHang((_pageNumberMuaHang - 1) * _pageSize + 1);
            self.RowsEndMuaHang((_pageNumberMuaHang - 1) * _pageSize + self.ReportKH_MuaHangChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.ChiTietHangHoa = function (item) {
        console.log(item.ID_NhanVien)
        ajaxHelper(ReportUri + "getListReportHangBanChiTiet_NhanVien?maHH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _id_DonVi + "&ID_NhanVien=" + item.ID_NhanVien, "GET").done(function (data) {
            self.ReportHangBanChiTiet_NhanVien(data.LstData);
        });
    }
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.GetClassBanHang = function (page) {
        return (page.SoTrang === self.currentPageBanHang()) ? "click" : "";
    };
    self.GetClassCongNo = function (page) {
        return (page.SoTrang === self.currentPageCongNo()) ? "click" : "";
    };
    self.GetClassMuaHang = function (page) {
        return (page.SoTrang === self.currentPageMuaHang()) ? "click" : "";
    };
    //Phân trang
    self.selecPage = function () {
        // AllPage = self.SumNumberPageReport().length;
        if (AllPage > 4) {
            for (var i = 3; i < AllPage; i++) {
                self.SumNumberPageReport.pop(i + 1);
            }
            self.SumNumberPageReport.push({ SoTrang: 4 });
            self.SumNumberPageReport.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReport.pop(i);
            }
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
        //self.selecPage();
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
            self.ReserPage();
            if (_kieubang == 1)
                self.SelectedPageNumberReportKH_BanHang();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportKH_LoiNhuan();
            else if (_kieubang == 3)
                self.SelectedPageNumberReportKH_CongNo();
            else if (_kieubang == 4)
                self.SelectedPageNumberReportKH_MuaHang();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.ReserPage();
            if (_kieubang == 1)
                self.SelectedPageNumberReportKH_BanHang();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportKH_LoiNhuan();
            else if (_kieubang == 3)
                self.SelectedPageNumberReportKH_CongNo();
            else if (_kieubang == 4)
                self.SelectedPageNumberReportKH_MuaHang();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportKH_BanHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportKH_LoiNhuan();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportKH_CongNo();
        else if (_kieubang == 4)
            self.SelectedPageNumberReportKH_MuaHang();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportKH_BanHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportKH_LoiNhuan();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportKH_CongNo();
        else if (_kieubang == 4)
            self.SelectedPageNumberReportKH_MuaHang();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportKH_BanHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportKH_LoiNhuan();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportKH_CongNo();
        else if (_kieubang == 4)
            self.SelectedPageNumberReportKH_MuaHang();
    }
    // phân trang chi bán hàng theo khách hàng
    self.selecPageBanHang = function () {
        if (AllPageBanHang > 4) {
            for (var i = 3; i < AllPageBanHang; i++) {
                self.SumNumberPageReportBanHang.pop(i + 1);
            }
            self.SumNumberPageReportBanHang.push({ SoTrang: 4 });
            self.SumNumberPageReportBanHang.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportBanHang.pop(i);
            }
            for (var j = 0; j < AllPageBanHang; j++) {
                self.SumNumberPageReportBanHang.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageBanHang').hide();
        $('#BackPageBanHang').hide();
        $('#NextPageBanHang').show();
        $('#EndPageBanHang').show();
    }
    self.ReserPageBanHang = function (item) {
        //self.selecPage();
        if (_pageNumberBanHang > 1 && AllPageBanHang > 5/* && nextPage < AllPageBanHang - 1*/) {
            if (_pageNumberBanHang > 3 && _pageNumberBanHang < AllPageBanHang - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberBanHang) === parseInt(AllPageBanHang) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i - 3 });
                }
            }
            else if (_pageNumberBanHang == AllPageBanHang) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i - 4 });
                }
            }
            else if (_pageNumberBanHang < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberBanHang)
                    self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberBanHang == 1 && AllPageBanHang > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportBanHang.replace(self.SumNumberPageReportBanHang()[i], { SoTrang: parseInt(_pageNumberBanHang) + i });
            }
        }
        if (_pageNumberBanHang > 1) {
            $('#StartPageBanHang').show();
            $('#BackPageBanHang').show();
        }
        else {
            $('#StartPageBanHang').hide();
            $('#BackPageBanHang').hide();
        }
        if (_pageNumberBanHang == AllPageBanHang) {
            $('#NextPageBanHang').hide();
            $('#EndPageBanHang').hide();
        }
        else {
            $('#NextPageBanHang').show();
            $('#EndPageBanHang').show();
        }
        self.currentPageBanHang(parseInt(_pageNumberBanHang));
    }
    self.NextPageBanHang = function (item) {
        if (_pageNumberBanHang < AllPageBanHang) {
            _pageNumberBanHang = _pageNumberBanHang + 1;
            self.ReserPageBanHang();
            self.SelectedPageNumberReportKH_BanHangChiTiet();
        }
    };
    self.BackPageBanHang = function (item) {
        if (_pageNumberBanHang > 1) {
            _pageNumberBanHang = _pageNumberBanHang - 1;
            self.ReserPageBanHang();
            self.SelectedPageNumberReportKH_BanHangChiTiet();
        }
    };
    self.EndPageBanHang = function (item) {
        _pageNumberBanHang = AllPageBanHang;
        self.ReserPageBanHang();
        self.SelectedPageNumberReportKH_BanHangChiTiet();
    };
    self.StartPageBanHang = function (item) {
        _pageNumberBanHang = 1;
        self.ReserPageBanHang();
        self.SelectedPageNumberReportKH_BanHangChiTiet();
    };
    self.gotoNextPageBanHang = function (item) {
        _pageNumberBanHang = item.SoTrang;
        self.ReserPageBanHang();
        self.SelectedPageNumberReportKH_BanHangChiTiet();
    }
    // phân trang chi công nợ theo khách hàng
    self.selecPageCongNo = function () {
        if (AllPageCongNo > 4) {
            for (var i = 3; i < AllPageCongNo; i++) {
                self.SumNumberPageReportCongNo.pop(i + 1);
            }
            self.SumNumberPageReportCongNo.push({ SoTrang: 4 });
            self.SumNumberPageReportCongNo.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportCongNo.pop(i);
            }
            for (var j = 0; j < AllPageCongNo; j++) {
                self.SumNumberPageReportCongNo.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageCongNo').hide();
        $('#BackPageCongNo').hide();
        $('#NextPageCongNo').show();
        $('#EndPageCongNo').show();
    }
    self.ReserPageCongNo = function (item) {
        //self.selecPage();
        if (_pageNumberCongNo > 1 && AllPageCongNo > 5/* && nextPage < AllPageCongNo - 1*/) {
            if (_pageNumberCongNo > 3 && _pageNumberCongNo < AllPageCongNo - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberCongNo) === parseInt(AllPageCongNo) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i - 3 });
                }
            }
            else if (_pageNumberCongNo == AllPageCongNo) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i - 4 });
                }
            }
            else if (_pageNumberCongNo < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberCongNo)
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberCongNo == 1 && AllPageCongNo > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i });
            }
        }
        if (_pageNumberCongNo > 1) {
            $('#StartPageCongNo').show();
            $('#BackPageCongNo').show();
        }
        else {
            $('#StartPageCongNo').hide();
            $('#BackPageCongNo').hide();
        }
        if (_pageNumberCongNo == AllPageCongNo) {
            $('#NextPageCongNo').hide();
            $('#EndPageCongNo').hide();
        }
        else {
            $('#NextPageCongNo').show();
            $('#EndPageCongNo').show();
        }
        self.currentPageCongNo(parseInt(_pageNumberCongNo));
    }
    self.NextPageCongNo = function (item) {
        if (_pageNumberCongNo < AllPageCongNo) {
            _pageNumberCongNo = _pageNumberCongNo + 1;
            self.ReserPageCongNo();
            self.SelectedPageNumberReportKH_CongNoChiTiet();
        }
    };
    self.BackPageCongNo = function (item) {
        if (_pageNumberCongNo > 1) {
            _pageNumberCongNo = _pageNumberCongNo - 1;
            self.ReserPageCongNo();
            self.SelectedPageNumberReportKH_CongNoChiTiet();
        }
    };
    self.EndPageCongNo = function (item) {
        _pageNumberCongNo = AllPageCongNo;
        self.ReserPageCongNo();
        self.SelectedPageNumberReportKH_CongNoChiTiet();
    };
    self.StartPageCongNo = function (item) {
        _pageNumberCongNo = 1;
        self.ReserPageCongNo();
        self.SelectedPageNumberReportKH_CongNoChiTiet();
    };
    self.gotoNextPageCongNo = function (item) {
        _pageNumberCongNo = item.SoTrang;
        self.ReserPageCongNo();
        self.SelectedPageNumberReportKH_CongNoChiTiet();
    }
    // phân trang chi mua hàng theo khách hàng
    self.selecPageMuaHang = function () {
        if (AllPageMuaHang > 4) {
            for (var i = 3; i < AllPageMuaHang; i++) {
                self.SumNumberPageReportMuaHang.pop(i + 1);
            }
            self.SumNumberPageReportMuaHang.push({ SoTrang: 4 });
            self.SumNumberPageReportMuaHang.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportMuaHang.pop(i);
            }
            for (var j = 0; j < AllPageMuaHang; j++) {
                self.SumNumberPageReportMuaHang.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageMuaHang').hide();
        $('#BackPageMuaHang').hide();
        $('#NextPageMuaHang').show();
        $('#EndPageMuaHang').show();
    }
    self.ReserPageMuaHang = function (item) {
        //self.selecPage();
        if (_pageNumberMuaHang > 1 && AllPageMuaHang > 5/* && nextPage < AllPageMuaHang - 1*/) {
            if (_pageNumberMuaHang > 3 && _pageNumberMuaHang < AllPageMuaHang - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberMuaHang) === parseInt(AllPageMuaHang) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i - 3 });
                }
            }
            else if (_pageNumberMuaHang == AllPageMuaHang) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i - 4 });
                }
            }
            else if (_pageNumberMuaHang < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberMuaHang)
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberMuaHang == 1 && AllPageMuaHang > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i });
            }
        }
        if (_pageNumberMuaHang > 1) {
            $('#StartPageMuaHang').show();
            $('#BackPageMuaHang').show();
        }
        else {
            $('#StartPageMuaHang').hide();
            $('#BackPageMuaHang').hide();
        }
        if (_pageNumberMuaHang == AllPageMuaHang) {
            $('#NextPageMuaHang').hide();
            $('#EndPageMuaHang').hide();
        }
        else {
            $('#NextPageMuaHang').show();
            $('#EndPageMuaHang').show();
        }
        self.currentPageMuaHang(parseInt(_pageNumberMuaHang));
    }
    self.NextPageMuaHang = function (item) {
        if (_pageNumberMuaHang < AllPageMuaHang) {
            _pageNumberMuaHang = _pageNumberMuaHang + 1;
            self.ReserPageMuaHang();
            self.SelectedPageNumberReportKH_MuaHangChiTiet();
        }
    };
    self.BackPageMuaHang = function (item) {
        if (_pageNumberMuaHang > 1) {
            _pageNumberMuaHang = _pageNumberMuaHang - 1;
            self.ReserPageMuaHang();
            self.SelectedPageNumberReportKH_MuaHangChiTiet();
        }
    };
    self.EndPageMuaHang = function (item) {
        _pageNumberMuaHang = AllPageMuaHang;
        self.ReserPageMuaHang();
        self.SelectedPageNumberReportKH_MuaHangChiTiet();
    };
    self.StartPageMuaHang = function (item) {
        _pageNumberMuaHang = 1;
        self.ReserPageMuaHang();
        self.SelectedPageNumberReportKH_MuaHangChiTiet();
    };
    self.gotoNextPageMuaHang = function (item) {
        _pageNumberMuaHang = item.SoTrang;
        self.ReserPageMuaHang();
        self.SelectedPageNumberReportKH_MuaHangChiTiet();
    }

    //Xuất file excel
    $('#khma').click(function () {
        $(".khma").toggle();
        self.addColum(1, $(this).val());
        addClass(".khma", "khma", $(this).val(), "banhangKH")
    });
    $('#khname ').click(function () {
        $(".khname ").toggle();
        self.addColum(1, $(this).val());
        addClass(".khname", "khname", $(this).val(), "banhangKH")
    });
    $('#khdoanhthu ').click(function () {
        $(".khdoanhthu ").toggle();
        self.addColum(1, $(this).val());
        addClass(".khdoanhthu", "khdoanhthu", $(this).val(), "banhangKH")
    });
    $('#khgiatri ').click(function () {
        $(".khgiatri ").toggle();
        self.addColum(1, $(this).val());
        addClass(".khgiatri", "khgiatri", $(this).val(), "banhangKH")
    });
    $('#khdoanhthuthuan ').click(function () {
        $(".khdoanhthuthuan ").toggle();
        self.addColum(1, $(this).val());
        addClass(".khdoanhthuthuan", "khdoanhthuthuan", $(this).val(), "banhangKH")
    });

    $('#lnname').click(function () {
        $(".lnname").toggle();
        self.addColum(2, $(this).val());
        addClass(".khdoanhthuthuan", "khdoanhthuthuan", $(this).val(), "loinhuanKH")
    });
    $('#lntongtien').click(function () {
        $(".lntongtien").toggle();
        self.addColum(2, $(this).val());
        addClass(".lntongtien", "lntongtien", $(this).val(), "loinhuanKH")
    });
    $('#lngiamgia').click(function () {
        $(".lngiamgia").toggle();
        self.addColum(2, $(this).val());
        addClass(".lngiamgia", "lngiamgia", $(this).val(), "loinhuanKH")
    });
    $('#lndoanhthu').click(function () {
        $(".lndoanhthu").toggle();
        self.addColum(2, $(this).val());
        addClass(".lndoanhthu", "lndoanhthu", $(this).val(), "loinhuanKH")
    });
    $('#lngiatri').click(function () {
        $(".lngiatri").toggle();
        self.addColum(2, $(this).val());
        addClass(".lngiatri", "lngiatri", $(this).val(), "loinhuanKH")
    });
    $('#lndoanhthuthuan').click(function () {
        $(".lndoanhthuthuan").toggle();
        self.addColum(2, $(this).val());
        addClass(".lndoanhthuthuan", "lndoanhthuthuan", $(this).val(), "loinhuanKH")
    });
    $('#lntonggiavon').click(function () {
        $(".lntonggiavon").toggle();
        self.addColum(2, $(this).val());
        addClass(".lntonggiavon", "lntonggiavon", $(this).val(), "loinhuanKH")
    });
    $('#lngop').click(function () {
        $(".lngop").toggle();
        self.addColum(2, $(this).val());
        addClass(".lngop", "lngop", $(this).val(), "loinhuanKH")
    });

    $('#cnma').click(function () {
        $(".cnma").toggle();
        self.addColum(3, $(this).val());
        addClass(".cnma", "cnma", $(this).val(), "congnoKH")
    });
    $('#cnname').click(function () {
        $(".cnname").toggle();
        self.addColum(3, $(this).val());
        addClass(".cnname", "cnname", $(this).val(), "congnoKH")
    });
    $('#cndauky').click(function () {
        $(".cndauky").toggle();
        self.addColum(3, $(this).val());
        addClass(".cndauky", "cndauky", $(this).val(), "congnoKH")
    });
    $('#cnghino').click(function () {
        $(".cnghino").toggle();
        self.addColum(3, $(this).val());
        addClass(".cnghino", "cnghino", $(this).val(), "congnoKH")
    });
    $('#cnghico').click(function () {
        $(".cnghico").toggle();
        self.addColum(3, $(this).val());
        addClass(".cnghico", "cnghico", $(this).val(), "congnoKH")
    });
    $('#cncuoiky').click(function () {
        $(".cncuoiky").toggle();
        self.addColum(3, $(this).val());
        addClass(".cncuoiky", "cncuoiky", $(this).val(), "congnoKH")
    });

    $('#bhma').click(function () {
        $(".bhma").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhma", "bhma", $(this).val(), "hangbanKH")
    });
    $('#bhname').click(function () {
        $(".bhname").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhname", "bhname", $(this).val(), "hangbanKH")
    });
    $('#bhdiem').click(function () {
        $(".bhdiem").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhdiem", "bhdiem", $(this).val(), "hangbanKH")
    });
    $('#bhsl').click(function () {
        $(".bhsl").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhsl", "bhsl", $(this).val(), "hangbanKH")
    });
    $('#bhgiatri').click(function () {
        $(".bhgiatri").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhgiatri", "bhgiatri", $(this).val(), "hangbanKH")
    });
    $('#bhsotra').click(function () {
        $(".bhsotra").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhsotra", "bhsotra", $(this).val(), "hangbanKH")
    });
    $('#bhgiatritra').click(function () {
        $(".bhgiatritra").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhgiatritra", "bhgiatritra", $(this).val(), "hangbanKH")
    });
    $('#bhgiatrithuan').click(function () {
        $(".bhgiatrithuan").toggle();
        self.addColum(4, $(this).val());
        addClass(".bhgiatrithuan", "bhgiatrithuan", $(this).val(), "hangbanKH")
    });
    self.addColum = function (values, item) {
        if (values == 1) {
            if (self.ColumnsExcelBHKH().length < 1) {
                self.ColumnsExcelBHKH.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelBHKH().length; i++) {
                    if (self.ColumnsExcelBHKH()[i] === item) {
                        self.ColumnsExcelBHKH.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelBHKH().length - 1) {
                        self.ColumnsExcelBHKH.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelBHKH.sort();
        }
        else if (values == 2) {
            if (self.ColumnsExcelLNKH().length < 1) {
                self.ColumnsExcelLNKH.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelLNKH().length; i++) {
                    if (self.ColumnsExcelLNKH()[i] === item) {
                        self.ColumnsExcelLNKH.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelLNKH().length - 1) {
                        self.ColumnsExcelLNKH.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelLNKH.sort();
        }
        else if (values == 3) {
            if (self.ColumnsExcelCNKH().length < 1) {
                self.ColumnsExcelCNKH.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelCNKH().length; i++) {
                    if (self.ColumnsExcelCNKH()[i] === item) {
                        self.ColumnsExcelCNKH.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelCNKH().length - 1) {
                        self.ColumnsExcelCNKH.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelCNKH.sort();
        }
        else {
            if (self.ColumnsExcelHBKH().length < 1) {
                self.ColumnsExcelHBKH.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelHBKH().length; i++) {
                    if (self.ColumnsExcelHBKH()[i] === item) {
                        self.ColumnsExcelHBKH.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelHBKH().length - 1) {
                        self.ColumnsExcelHBKH.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelHBKH.sort();
        }
    }
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid 
    //===============================
    function LoadHtmlGrid(cacheExcel, vals, caches) {
        if (window.localStorage) {
            var current = localStorage.getItem(caches);
            if (!current) {
                current = [];
                cacheExcel = false;
                localStorage.setItem(caches, JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    document.getElementById(current[i].NameId).checked = false;
                    if (cacheExcel) {
                        if (vals === 1)
                            self.addColum(1, current[i].Value);
                        else if (vals === 2)
                            self.addColum(2, current[i].Value);
                        else if (vals === 3)
                            self.addColum(3, current[i].Value);
                        else
                            self.addColum(4, current[i].Value);
                    }
                }
                if (vals === 1)
                    cacheExcelBHKH = false;
                else if (vals === 2)
                    cacheExcelLNKH = false;
                else if (vals === 3)
                    cacheExcelCNKH = false;
                else
                    cacheExcelHBKH = false;
            }
        }
    }

    //===============================
    // Add Các tham số cần lưu lại để
    // cache khi load lại form  
    //===============================
    function addClass(name, id, value, caches) {

        var current = localStorage.getItem(caches);
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
        localStorage.setItem(caches, JSON.stringify(current));
    }
    self.ExportExcel = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo khách hàng",
            NoiDung: "Xuất " + self.MoiQuanTam(),
            NoiDungChiTiet: "Xuất " + self.MoiQuanTam(),
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
                console.log(_kieubang)
                if (_kieubang == 1 && self.ReportKH_BanHang().length != 0) {
                    if (self.BCKH_BanHang() == "BCKH_BanHang" && self.BCKH_BanHang_XuatFile() == "BCKH_BanHang_XuatFile")
                    {
                        for (var i = 0; i < self.ColumnsExcelBHKH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelBHKH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelBHKH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelKH_BanHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 2 && self.ReportKH_LoiNhuan().length != 0) {
                    if (self.BCKH_LoiNhuan() == "BCKH_LoiNhuan" && self.BCKH_LoiNhuan_XuatFile() == "BCKH_LoiNhuan_XuatFile")
                    {

                        for (var i = 0; i < self.ColumnsExcelLNKH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelLNKH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelLNKH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelKH_LoiNhuan?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + $('.idnguoidung').text();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 3 && self.ReportKH_CongNo().length != 0) {
                    if (self.BCKH_CongNo() == "BCKH_CongNo" && self.BCKH_CongNo_XuatFile() == "BCKH_CongNo_XuatFile")
                    {
                        for (var i = 0; i < self.ColumnsExcelCNKH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelCNKH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelCNKH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelKH_CongNo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _id_DonVi + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 4 && self.ReportKH_MuaHang().length != 0) {
                    if (self.BCKH_HangBan() == "BCKH_HangBan" && self.BCKH_HangBan_XuatFile() == "BCKH_HangBan_XuatFile")
                    {
                        for (var i = 0; i < self.ColumnsExcelHBKH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelHBKH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelHBKH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelKH_MuaHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel!", "danger");
                }
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        })
    }
    self.ExportChiTietHangBan = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo khách hàng",
            NoiDung: "Xuất báo cáo chi tiết hàng bán theo khách hàng, mã khách hàng: " + _ma_KhachHang,
            NoiDungChiTiet: "Xuất báo cáo chi tiết hàng bán theo khách hàng, mã khách hàng: " + _ma_KhachHang,
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
                var url = ReportUri + "ExportExcelHangBanChiTiet_NhanVien?maHH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + item.ID_NhanVien + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        })

    }
    self.ExportExcel_BanHangChiTiet = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo khách hàng",
            NoiDung: "Xuất báo cáo chi tiết hàng bán theo khách hàng, mã khách hàng: " + _ma_KhachHang,
            NoiDungChiTiet: "Xuất báo cáo chi tiết hàng bán theo khách hàng, mã khách hàng: " + _ma_KhachHang,
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
                var url = ReportUri + "ExportExcelKH_BanHangChiTiet?ID_KhachHang=" + _id_KhachHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        })

    }
    self.ExportExcel_CongNoChiTiet = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo khách hàng",
            NoiDung: "Xuất báo cáo chi tiết công nợ theo khách hàng, mã khách hàng: " + _ma_KhachHangCongNo,
            NoiDungChiTiet: "Xuất báo cáo chi tiết công nợ theo khách hàng, mã khách hàng: " + _ma_KhachHangCongNo,
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
                var url = ReportUri + "ExportExcelKH_CongNoChiTiet?ID_KhachHang=" + _id_DoiTuongCongNo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _id_DonVi + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        })
    }
    self.ExportExcel_MuaHangChiTiet = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo khách hàng",
            NoiDung: "Xuất báo cáo hàng bán theo khách hàng, mã khách hàng: " + _ma_KhachHangMuaHang,
            NoiDungChiTiet: "Xuất báo cáo hàng bán theo khách hàng, mã khách hàng: " + _ma_KhachHangMuaHang,
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
                var url = ReportUri + "ExportExcelKH_MuaHangChiTiet?ID_KhachHang=" + _id_DoiTuongMuaHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {

            }
        })

    }
    var _ID_NhomDoiTuong = null;
    self.SelectRepoert_NhomDoiTuong = function (item) {
        console.log(_ID_NhomDoiTuong);
        if (item.ID == undefined) {
            _ID_NhomDoiTuong = null;
            $('.li-oo').removeClass("yellow")
            $('#tatcanhh a').css("display", "block");
            $('#tatcanhh').addClass("yellow")
        }
        else {
            _ID_NhomDoiTuong = item.ID;
            $('.ss-li .li-oo').removeClass("yellow");
            $('#tatcanhh').removeClass("yellow")
            $('.li-pp').removeClass("yellow");
            $('#tatcanhh a').css("display", "none");
            $('#' + item.ID).addClass("yellow");
        }
        _pageNumber = 1;
        //self.getListReportHH_NhaCungCap();
    }
    var time = null
    self.NhomDoiTuongs = ko.observableArray();
    var _tennhomDT = null;
    self.NoteNhomDoiTuong = function () {
        clearTimeout(time);
        time = setTimeout(
            function () {
                //self.NhomDoiTuongs([]);
                _tennhomDT = $('#SeachNhomDoiTuong').val();
                ajaxHelper(ReportUri + "GetListID_NhomDoiTuong?TenNhomDoiTuong=" + _tennhomDT + "&loaidoituong=1", "GET").done(function (data) {
                    self.NhomDoiTuongs(data);
                });
            }, 300);
    };
    var _tenNhomDoiTuongSeach = null;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    function getList_NhomDoiTuongs() {
        ajaxHelper(ReportUri + "GetListID_NhomDoiTuong?TenNhomDoiTuong=" + _tennhomDT + "&loaidoituong=1", "GET").done(function (data) {
            self.NhomDoiTuongs(data);
            self.searchNhomDoiTuong(data);
        });
    };
    getList_NhomDoiTuongs();

    self.CloseNhomDoiTuong = function (item) {
        _tenNhomDoiTuongSeach = null;
        self.MangNhomDoiTuong.remove(item);
        for (var i = 0; i < self.MangNhomDoiTuong().length; i++) {
            _tenNhomDoiTuongSeach = self.MangNhomDoiTuong()[i].ID + "," + _tenNhomDoiTuongSeach;
        }
        if (self.MangNhomDoiTuong().length === 0) {
            _tenNhomDoiTuongSeach = null;
        }
        // remove check
        $('#selec-all-NhomDoiTuong li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNhomDoiTuong())
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportKH_LoiNhuan();
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListKH_CongNo_BieuDo();
            else
                self.getListReportKH_CongNo();
        }
        else if (_kieubang == 4)
            self.getListReportKH_MuaHang();

    }
    self.SelectedNhomDoiTuong = function (item) {
        _tenNhomDoiTuongSeach = null;
        var arrIDNhomDoiTuong = [];
        for (var i = 0; i < self.MangNhomDoiTuong().length; i++) {
            if ($.inArray(self.MangNhomDoiTuong()[i], arrIDNhomDoiTuong) === -1) {
                arrIDNhomDoiTuong.push(self.MangNhomDoiTuong()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNhomDoiTuong) === -1) {
            self.MangNhomDoiTuong.push(item);
            for (var i = 0; i < self.MangNhomDoiTuong().length; i++) {
                _tenNhomDoiTuongSeach = self.MangNhomDoiTuong()[i].ID + "," + _tenNhomDoiTuongSeach;
            }
        }
        // sau khi tìm kiếm thì trả về mặc định
        $('#NoteNameNhomDoiTuong').val('');
        self.NhomDoiTuongs(self.searchNhomDoiTuong());
        //đánh dấu check
        for (var i = 0; i < self.MangNhomDoiTuong().length; i++) {
            $('#selec-all-NhomDoiTuong li').each(function () {
                if ($(this).attr('id') === self.MangNhomDoiTuong()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListKH_BanHang_BieuDo();
            else
                self.getListReportKH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportKH_LoiNhuan();
        else if (_kieubang == 3) {
            if (self.check_kieubang() == 1)
                self.getListKH_CongNo_BieuDo();
            else
                self.getListReportKH_CongNo();
        }
        else if (_kieubang == 4)
            self.getListReportKH_MuaHang();
    }
    //lọc người bán
    function locdau(obj) {
        if (!obj)
            return "";
        var str = obj;
        str = str.toLowerCase();
        str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
        str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
        str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
        str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
        str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
        str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
        str = str.replace(/đ/g, "d");
        str = str.replace(/^\-+|\-+$/g, "");
        //str = str.replace(" ", "_");
        return str;
    }
    self.NoteNameNhomDoiTuong = function () {
        var arrNhomDoiTuong = [];
        var itemSearch = locdau($('#NoteNameNhomDoiTuong').val().toLowerCase());
        for (var i = 0; i < self.searchNhomDoiTuong().length; i++) {
            var locdau_kd = self.searchNhomDoiTuong()[i].TenNhomDoiTuong_KhongDau;
            var locdau_ktd = self.searchNhomDoiTuong()[i].TenNhomDoiTuong_KyTuDau;
            var R1 = locdau_kd.split(itemSearch);
            var R2 = locdau_ktd.split(itemSearch);
            if (R1.length > 1 || R2.length > 1) {
                arrNhomDoiTuong.push(self.searchNhomDoiTuong()[i]);
            }
        }
        self.NhomDoiTuongs(arrNhomDoiTuong);
        if ($('#NoteNameNhomDoiTuong').val() == "") {
            self.NhomDoiTuongs(self.searchNhomDoiTuong());
            // console.log(self.NhomDoiTuongs())
            for (var i = 0; i < self.MangNhomDoiTuong().length; i++) {
                $('#selec-all-NhomDoiTuong li').each(function () {
                    if ($(this).attr('id') === self.MangNhomDoiTuong()[i].ID) {
                        $(this).find('i').remove();
                        $(this).append('<i class="fa fa-check check-after-li"></i>')
                    }
                });
            }
        }
    }
    $('#NoteNameNhomDoiTuong').keypress(function (e) {
        if (e.keyCode == 13 && self.NhomDoiTuongs().length > 0) {
            self.SelectedNhomDoiTuong(self.NhomDoiTuongs()[0]);
        }
    });
    //self.getListReportKH_BanHang();
    self.arrHH = ko.observableArray();
    self.arrDT = ko.observableArray();
    self.DoanhThuTT = ko.observableArray();
    self.DonViTinh = ko.observable("Đơn vị tính: hàng đơn vị");
    var _dataDS;
    var _data;
    var nameChar;
    var style;
    self.getListKH_BanHang_BieuDo = function () {
        ajaxHelper(ReportUri + "getListKH_BanHang_BieuDo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.DoanhThuTT(data);
            self.arrDT([]);
            self.arrHH([]);
            $('.text-unit').css("bottom", "15px");
            if (self.DoanhThuTT().length != 0) {
                var _MauSoDVT = 1;
                var _loadHangHoa = "'";
                //if (data.length > 0)
                //    var dvt = self.DoanhThuTT()[0].Rowsn;
                //if (dvt >= 1000000000) {
                //    _MauSoDVT = 1000000000
                //    self.DonViTinh("Đơn vị tính: hàng tỷ")
                //}
                //if (dvt >= 1000000 & dvt < 1000000000) {
                //    _MauSoDVT = 1000000
                //    self.DonViTinh("Đơn vị tính: hàng triệu")
                //}
                //if (dvt >= 1000 & dvt < 1000000) {
                //    _MauSoDVT = 1000
                //    self.DonViTinh("Đơn vị tính: hàng nghìn")
                //}
                for (var i = 0; i < self.DoanhThuTT().length; i++) {
                    _loadHangHoa = self.DoanhThuTT()[i].Columnss;
                    //_data = parseFloat(self.DoanhThuTT()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                    self.arrDT.push(self.DoanhThuTT()[i].Rowsn);
                    self.arrHH.push(_loadHangHoa);
                }
                style = 'Top 10 khách hàng mua nhiều nhất (đã trừ trả hàng)';
                nameChar = "Doanh thu thuần";
            }
            else {
                style = "Báo cáo không có dữ liệu.";
                self.DonViTinh([]);
            }
            self.loadBieuDo();
        });
    }

    self.getListKH_CongNo_BieuDo = function () {
        ajaxHelper(ReportUri + "getListKH_CongNo_BieuDo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maKH=" + _maKH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            self.DoanhThuTT(data);
            self.arrDT([]);
            self.arrHH([]);
            $('.text-unit').css("bottom", "18px");
            if (self.DoanhThuTT().length != 0) {
                var _MauSoDVT = 1;
                var _loadHangHoa = "'";
                //if (data.length > 0)
                //    var dvt = self.DoanhThuTT()[0].Rowsn;
                //if (dvt >= 1000000000) {
                //    _MauSoDVT = 1000000000
                //    self.DonViTinh("Đơn vị tính: hàng tỷ")
                //}
                //if (dvt >= 1000000 & dvt < 1000000000) {
                //    _MauSoDVT = 1000000
                //    self.DonViTinh("Đơn vị tính: hàng triệu")
                //}
                //if (dvt >= 1000 & dvt < 1000000) {
                //    _MauSoDVT = 1000
                //    self.DonViTinh("Đơn vị tính: hàng nghìn")
                //}
                for (var i = 0; i < self.DoanhThuTT().length; i++) {
                    _loadHangHoa = self.DoanhThuTT()[i].Columnss;
                    //_data = parseFloat(self.DoanhThuTT()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                    self.arrDT.push(self.DoanhThuTT()[i].Rowsn);
                    self.arrHH.push(_loadHangHoa);
                }
                style = 'Top 10 khách hàng nợ nhiều nhất';
                nameChar = "Nợ cuối kỳ";
            }
            else {
                style = "Báo cáo không có dữ liệu.";
                self.DonViTinh([]);
            }
            self.loadBieuDo();
        });
    }
    //trinhpv load bieudo
    self.loadBieuDo = function () {
        var viewPrint = true;
        if (self.arrDT().length > 0)
            viewPrint = true;
        else
            viewPrint = false;
        var chart = Highcharts.chart('chart', {
            chart: {
                type: 'bar'
            },
            title: {
                text: style
            },
            subtitle: {
                text: ''
            },
            // đưa danh sách hàng hóa vào cột x
            xAxis: {
                categories: self.arrHH()
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Population (millions)',
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                },
                plotLines: [{
                    width: 30,
                }]
            },
            // hiển thị giá trị lên đầu cột
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: false
                    }
                }
            },
            tooltip: {
                footerFormat: "</table>",
                shared: false,
                useHTML: true,
                formatter: function () {
                    return "<span style=\"font-size:10px\">" + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; \">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 0, '.', ',') + '</b>';
                }
            },
            colors: [
                "#32b7b3",
            ],
            credits: {
                enabled: false,
            },
            legend: {
                enabled: false
            },
            navigation: {
                buttonOptions: {
                    theme: {
                        states: {
                            hover: {
                                fill: 'none'
                            },
                            select: {
                                fill: 'none'
                            }
                        }
                    }
                }
            },
            exporting: {
                enabled: viewPrint,
                buttons: {
                    contextButton: {
                        symbol: 'url(Template/DuLieuGoc/print_24.png)',
                        //x: -62,    
                    }
                }
            },
            // đưa giá trị tương ứng vào hàng hóa trong cột y
            series: [{
                name: nameChar,
                data: self.arrDT(),
                maxPointWidth: 30
            }]
        });

    }
}
ko.applyBindings(new ViewModel());

function hidewait(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /></div>')
}