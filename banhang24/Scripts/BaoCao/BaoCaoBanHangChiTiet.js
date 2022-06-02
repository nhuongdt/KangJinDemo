var ViewModel = function () {
    var self = this;
    self.ColumnsExcelBH = ko.observableArray();
    var cacheExcelBH = true;
    var thisDate;
    self.MangChiNhanh = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _nameDonViSeach = null;
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _IDDoiTuong = $('.idnguoidung').text();

    self._ThanhTien = ko.observable(0);
    self._TienVon = ko.observable(0);
    self._LaiLo = ko.observable(0);
    self.Loc_Table = ko.observable('1');
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.HangHoas = ko.observableArray();
    //self.ReportHH_BanHang = ko.observableArray();
    self.ReportHH_BanHangPrint = ko.observableArray();
    self.MoiQuanTam = ko.observable('bán hàng');
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.TodayBC = ko.observable('Hôm nay');
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.SumRowsHangHoa = ko.observable();
    self.NhomHangHoas = ko.observableArray();
    $('#txtMaHH').focus();
    var dt1 = new Date();
    var tk = null;
    self.pageNumber = ko.observable(1);
    self.pageSize = ko.observable(10);
    //var _timeStart = '2015-09-26'
    //var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    var _maHH = null;
    var _laHangHoa = 2;
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
    var LoaiBieuDo = 1;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/'
    //trinhpv phân quyền
    self.BCBH_BanHang = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();
    self.BCBH_BanHang_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCBH_BanHang", "GET").done(function (data) {
            self.BCBH_BanHang(data);
            self.getListReportHH_BanHang();
        })
        //quyền xem xuất báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCBH_BanHang_XuatFile", "GET").done(function (data) {
            self.BCBH_BanHang_XuatFile(data);
        })
        ajaxHelper(ReportUri + "getQuyenXemGiaVon?ID_NguoiDung=" + _IDDoiTuong + "&MaQuyen=" + "HangHoa_XemGiaVon", "GET").done(function (data) {
            self.HangHoa_XemGiaVon(data);
            console.log(data);
        })
    }
    getQuyen_NguoiDung();
    $(".TongCongBanHang").hide();
    $(".page").hide();
    $(".PhanQuyen").hide();
    self.currentPage = ko.observable(1);
    var _ID_NhomHang = null;
    $('#home').removeClass("active")
    $('#info').addClass("active")
    self.check_kieubang = ko.observable('2');
    $('.chose_kieubang li').on('click', function () {
        self.check_kieubang($(this).val());
        console.log(self.check_kieubang());
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
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
                LoaiBieuDo = 3;
                _timeStart = '2015-09-26'
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                LoaiBieuDo = 1;
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                LoaiBieuDo = 1;
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            }
            //Tuần này
            else if (_rdoNgayPage === "Tuần này") {
                LoaiBieuDo = 2;
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            }
            //Tuần trước
            else if (_rdoNgayPage === "Tuần trước") {
                LoaiBieuDo = 2;
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            }
            //7 ngày qua
            else if (_rdoNgayPage === "7 ngày qua") {
                LoaiBieuDo = 2;
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                LoaiBieuDo = 2;
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            }
            //Tháng trước
            else if (_rdoNgayPage === "Tháng trước") {
                LoaiBieuDo = 2;
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            }
            //30 ngày qua
            else if (_rdoNgayPage === "30 ngày qua") {
                LoaiBieuDo = 2;
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Quý này
            else if (_rdoNgayPage === "Quý này") {
                LoaiBieuDo = 3;
                _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            // Quý trước
            else if (_rdoNgayPage === "Quý trước") {
                LoaiBieuDo = 3;
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Năm này
            else if (_rdoNgayPage === "Năm này") {
                LoaiBieuDo = 3;
                _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //năm trước
            else if (_rdoNgayPage === "Năm trước") {
                LoaiBieuDo = 3;
                var prevYear = moment().year() - 1;
                _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            _pageNumber = 1;
            
            if (self.check_kieubang() == 1) {
                self.getList_LoadBieuDoBanHang();
            }
            else {
                self.getListReportHH_BanHang();
            }
            // self.getListReportHH_BanHang();
            self.ReserPage();
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
                _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                if (yearEnd - yearStart > 0)
                    LoaiBieuDo = 4;
                else if (monthEnd == monthStart & dayStart != dayEnd)
                    LoaiBieuDo = 2;
                else if (monthEnd != monthStart)
                    LoaiBieuDo = 3;
                else
                    LoaiBieuDo = 1;
                self.TodayBC($('.ip_DateReport').val())
                _pageNumber = 1;
                if (self.check_kieubang() == 1) {
                    self.getList_LoadBieuDoBanHang();
                }
                else {
                    self.getListReportHH_BanHang();
                }
                self.ReserPage();
            }
        }
    })

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
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
        }
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
            if (self.check_kieubang() == 1) {
                self.getList_LoadBieuDoBanHang();
            }
            else {
                self.getListReportHH_BanHang();
            }
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

    $('#txtDate').on('dp.change', function (e) {
        LoaiBieuDo = 1;
        thisDate = $(this).val();
        console.log(thisDate);
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-")
        thisDate = moment(t1).format('MM/DD/YYYY')
        //console.log(thisDate);
        _timeStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        self.TodayBC($(this).val())
        _pageNumber = 1;
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
        }
        //self.getListReportHH_BanHang();
        self.ReserPage();
    });

    $('.choose_txtTime li').on('click', function () {
        self.TodayBC($(this).text())
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            LoaiBieuDo = 4;
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            LoaiBieuDo = 1;
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            LoaiBieuDo = 1;
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
        }
        //Tuần này
        else if (_rdoNgayPage === 3) {
            LoaiBieuDo = 2;
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            LoaiBieuDo = 2;
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            LoaiBieuDo = 2;
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Tháng này
        else if (_rdoNgayPage === 6) {
            LoaiBieuDo = 2;
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
        }
        //Tháng trước
        else if (_rdoNgayPage === 7) {
            LoaiBieuDo = 2;
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
        }
        //30 ngày qua
        else if (_rdoNgayPage === 8) {
            LoaiBieuDo = 2;
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Quý này
        else if (_rdoNgayPage === 9) {
            LoaiBieuDo = 3;
            _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        // Quý trước
        else if (_rdoNgayPage === 10) {
            LoaiBieuDo = 3;
            var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
            _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Năm này
        else if (_rdoNgayPage === 11) {
            LoaiBieuDo = 3;
            _timeStart = moment().startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //năm trước
        else if (_rdoNgayPage === 12) {
            LoaiBieuDo = 3;
            var prevYear = moment().year() - 1;
            _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().year(prevYear).endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        _pageNumber = 1;
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
        }
        // self.getListReportHH_BanHang();
        self.ReserPage();
    })
    // Select LoaiHang
    $('.choose_LoaiHang input').on('click', function () {
        if ($(this).val() == 1) {
            if (_ckHangHoa == 1 & _ckDichVu == 1) {
                _ckHangHoa = 0;
                _laHangHoa = 0;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 1) {
                _ckHangHoa = 1;
                _laHangHoa = 2;
            }
            else if (_ckHangHoa == 1 & _ckDichVu == 0) {
                _ckHangHoa = 0;
                _laHangHoa = 3;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 0) {
                _ckHangHoa = 1;
                _laHangHoa = 1;
            }
        }
        if ($(this).val() == 2) {
            if (_ckHangHoa == 1 & _ckDichVu == 1) {
                _ckDichVu = 0;
                _laHangHoa = 1;
            }
            else if (_ckHangHoa == 1 & _ckDichVu == 0) {
                _ckDichVu = 1;
                _laHangHoa = 2;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 1) {
                _ckDichVu = 0;
                _laHangHoa = 3;
            }
            else if (_ckHangHoa == 0 & _ckDichVu == 0) {
                _ckDichVu = 1;
                _laHangHoa = 0;
            }
        }
        console.log(_laHangHoa);
        _pageNumber = 1;
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
        }
        //self.getListReportHH_BanHang();
    })
    // Key Event maHH
    self.SelectMaHH = function () {
        _maHH = $('#txtMaHH').val();
        console.log(_maHH);
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {
            if (self.check_kieubang() == 1) {
                self.getList_LoadBieuDoBanHang();
            }
            else {
                self.getListReportHH_BanHang();
            }
            //self.getListReportHH_BanHang();
        }
    })
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
    GetAllNhomHH();
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
            }, 300);
    };
    self.SelectRepoert_NhomHangHoa = function (item) {
        _ID_NhomHang = item.ID;
        _pageNumber = 1;
        if (item.ID == undefined) {
            $('.li-oo').removeClass("yellow")
            $('#tatcanhh a').css("display", "block");
            $('#tatcanhh').addClass("yellow")
        }
        else {
            $('.ss-li .li-oo').removeClass("yellow");
            $('#tatcanhh').removeClass("yellow")
            $('.li-pp').removeClass("yellow");
            $('#tatcanhh a').css("display", "none");
            $('#' + item.ID).addClass("yellow");
        }
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
        }
        // self.getListReportHH_BanHang();
    }
    $('.SelectALLNhomHang').on('click', function () {
        //$('.SelectALLNhomHang li').addClass('SelectReport')
        //$('.SelectNhomHang li').each(function () {
        //    $(this).removeClass('SelectReport');
        //});
        _ID_NhomHang = null;
        _pageNumber = 1;
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
        }
        // self.getListReportHH_BanHang();
    });
    //GetListHangHoa_BanHang
    var _pageNumber = 1;
    var _pageSize = 10;
    var AllPage;
    self._SoLuong = ko.observable();
   
    self.getListReportHH_BanHang = function () {
        if (self.BCBH_BanHang() == "BCBH_BanHang")
        {
            $(".PhanQuyen").hide();
            hidewait_TR('table_h');
            _pageNumber = 1;
            self.pageNumber(_pageNumber);
            $(".page").show();
            ajaxHelper(ReportUri + "getListReportBanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NguoiDung=" + $('.idnguoidung').text(), "GET").done(function (data) {
                self.ReportHH_BanHangPrint(data.LstDataPrint)
                if (self.ReportHH_BanHangPrint().length != 0)
                    $(".Report_Empty").hide();
                else
                    $(".Report_Empty").show();
                self._ThanhTien(data._thanhtien);
                self._TienVon(data._tienvon);
                self._LaiLo(data._lailo);
                self._SoLuong(data._soluong);
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                LoadHtmlGrid(cacheExcelBH, 1, "banhangBC");
            });
        }
        else
        {
            $(".PhanQuyen").show();
            $(".TongCongBanHang").hide();
            $(".page").hide();
        }
        
    }
    self.ReportHH_BanHang = ko.computed(function (x) {
        var first = (self.pageNumber() - 1) * self.pageSize();
        if (self.ReportHH_BanHangPrint() !== null) {
            if (self.ReportHH_BanHangPrint().length != 0) {
                $('#TongCongBanHang').show();
                self.RowsStart((self.pageNumber() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber() - 1) * self.pageSize() + self.ReportHH_BanHangPrint().slice(first, first + self.pageSize()).length)
            }
            else {
                $('#TongCongBanHang').hide();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.ReportHH_BanHangPrint().slice(first, first + _pageSize);
        }
        return null;
    })
    self.getMoneyBanHang = function () {
        self._ThanhTien(0);
        self._TienVon(0);
        self._LaiLo(0);
        ajaxHelper(BH_HoaDonUri + "getMoneyBanHang?dayStart=" + _timeStart + "&dayEnd=" + _timeEnd + "&IDchinhanh=" + _IDchinhanh, "GET").done(function (data) {
            self.MoneyBanHang(data);
            //self._ThanhTien(parseFloat(self.MoneyBanHang()[0].ThanhTien).toFixed(2));
            //self._TienVon(parseFloat(self.MoneyBanHang()[0].TienVon).toFixed(2));
            //self._LaiLo(parseFloat(self.MoneyBanHang()[0].LaiLo).toFixed(2));
            self._ThanhTien(self.MoneyBanHang()[0].ThanhTien);
            self._TienVon(self.MoneyBanHang()[0].TienVon);
            self._LaiLo(self.MoneyBanHang()[0].LaiLo);
        });
    }
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
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
        LoadHtmlGrid(cacheExcelBH, 1, "banhangBC");
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
            self.pageNumber(_pageNumber);
            self.ReserPage();
            //self.SelectedPageNumberReportHH_BanHang();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.pageNumber(_pageNumber);
            self.ReserPage();
            //self.SelectedPageNumberReportHH_BanHang();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.pageNumber(_pageNumber);
        self.ReserPage();
        //self.SelectedPageNumberReportHH_BanHang();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.pageNumber(_pageNumber);
        self.ReserPage();
        //self.SelectedPageNumberReportHH_BanHang();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.pageNumber(_pageNumber);
        self.ReserPage();
        //self.SelectedPageNumberReportHH_BanHang();
    }
    //Xuất file excel
    $('#bhmact').click(function () {
        $(".bhmact").toggle();
        self.addColumBH($(this).val())
        addClass(".bhmact", "bhmact", $(this).val(), "banhangBC");
    });
    $('#bhtime').click(function () {
        $(".bhtime").toggle();
        self.addColumBH($(this).val())
        addClass(".bhtime", "bhtime", $(this).val(), "banhangBC");
    });
    $('#bhmahh').click(function () {
        $(".bhmahh").toggle();
        self.addColumBH($(this).val())
        addClass(".bhmahh", "bhmahh", $(this).val(), "banhangBC");
    });
    $('#bhname').click(function () {
        $(".bhname").toggle();
        self.addColumBH($(this).val())
        addClass(".bhname", "bhname", $(this).val(), "banhangBC");
    });
    $('#bhsoluong').click(function () {
        $(".bhsoluong").toggle();
        self.addColumBH($(this).val())
        addClass(".bhsoluong", "bhsoluong", $(this).val(), "banhangBC");
    });
    $('#bhgiaban').click(function () {
        $(".bhgiaban").toggle();
        self.addColumBH($(this).val())
        addClass(".bhgiaban", "bhgiaban", $(this).val(), "banhangBC");
    });
    $('#bhchietkhau').click(function () {
        $(".bhchietkhau").toggle();
        self.addColumBH($(this).val())
        addClass(".bhchietkhau", "bhchietkhau", $(this).val(), "banhangBC");
    });

    $('#bhthanhtien').click(function () {
        $(".bhthanhtien").toggle();
        self.addColumBH($(this).val())
        addClass(".bhthanhtien", "bhthanhtien", $(this).val(), "banhangBC");
    });
    $('#bhgiavon ').click(function () {
        $(".bhgiavon ").toggle();
        self.addColumBH($(this).val())
        addClass(".bhgiavon", "bhgiavon", $(this).val(), "banhangBC");
    });
    $('#bhtienvon ').click(function () {
        $(".bhtienvon ").toggle();
        self.addColumBH($(this).val())
        addClass(".bhtienvon", "bhtienvon", $(this).val(), "banhangBC");
    });
    $('#bhgiamgia ').click(function () {
        $(".bhgiamgia ").toggle();
        self.addColumBH($(this).val())
        addClass(".bhgiamgia", "bhgiamgia", $(this).val(), "banhangBC");
    });
    $('#bhlailo').click(function () {
        $(".bhlailo  ").toggle();
        self.addColumBH($(this).val())
        addClass(".bhlailo", "bhlailo", $(this).val(), "banhangBC");
    });
    $('#bhnhanvien  ').click(function () {
        $(".bhnhanvien  ").toggle();
        self.addColumBH($(this).val())
        addClass(".bhnhanvien", "bhnhanvien", $(this).val(), "banhangBC");
    });
    self.addColumBH = function (item) {
        if (self.ColumnsExcelBH().length < 1) {
            self.ColumnsExcelBH.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcelBH().length; i++) {
                if (self.ColumnsExcelBH()[i] === item) {
                    self.ColumnsExcelBH.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcelBH().length - 1) {
                    self.ColumnsExcelBH.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcelBH.sort();
        //console.log(self.ColumnsExcelBH());
    }
    function LoadHtmlGrid(cacheExcel, vals, caches) {
        if (window.localStorage) {
            var current = localStorage.getItem(caches);
            if (!current) {
                current = [];
                cacheExcelBH = false;
                localStorage.setItem(caches, JSON.stringify(current));
            } else {
                current = JSON.parse(current);
                for (var i = 0; i < current.length; i++) {
                    $(current[i].NameClass).addClass("operation");
                    document.getElementById(current[i].NameId).checked = false;
                    if (cacheExcel) {
                        console.log(1, cacheExcel);
                        self.addColumBH(current[i].Value);
                    }
                }
                cacheExcelBH = false;
                console.log(2, cacheExcel);
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
        if (self.BCBH_BanHang() != "BCBH_BanHang" || self.BCBH_BanHang_XuatFile() != "BCBH_BanHang_XuatFile")
        {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
        }
        else
        {
            if (self.ReportHH_BanHang().length != 0) {
                var objDiary = {
                    ID_NhanVien: _id_NhanVien,
                    ID_DonVi: _id_DonVi,
                    ChucNang: "Báo cáo bán hàng",
                    NoiDung: "Xuất báo cáo bán hàng chi tiết",
                    NoiDungChiTiet: "Xuất báo cáo bán hàng chi tiết",
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
                        for (var i = 0; i < self.ColumnsExcelBH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelBH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelBH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelBanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + $('.idnguoidung').text();
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
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Không có dữ liệu để xuất file excel!", "danger");
            }
        }
       
    }
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        console.log(picker.startDate.format('DD/MM/YYYY'), picker.endDate.format('DD/MM/YYYY'))

        var checktime1 = picker.startDate.format('DD/MM/YYYY').trim().split("/");
        var yearStart = parseInt(checktime1[2]);
        var monthStart = parseInt(checktime1[1]);
        var dayStart = parseInt(checktime1[0]);
        var checktime2 = picker.endDate.format('DD/MM/YYYY').trim().split("/");
        var yearEnd = parseInt(checktime2[2]);
        var monthEnd = parseInt(checktime2[1]);
        var dayEnd = parseInt(checktime2[0]);
        if (yearEnd - yearStart > 0)
            LoaiBieuDo = 4;
        else if (monthEnd == monthStart & dayStart != dayEnd)
            LoaiBieuDo = 2;
        else if (monthEnd != monthStart)
            LoaiBieuDo = 3;
        else
            LoaiBieuDo = 1;

        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        self.TodayBC($(this).val())
        _pageNumber = 1;
        if (self.check_kieubang() == 1) {
            self.getList_LoadBieuDoBanHang();
        }
        else {
            self.getListReportHH_BanHang();
        }
        self.ReserPage();
    });
    // load biểu đồ
    self.TongDoanhSo = ko.observable('0');
    self.DoanhThuBanHang = ko.observableArray();
    self.DonViTinhDS = ko.observable();
    self.DonViTinh_DoanhSo = ko.observable();
    self.arrNT = ko.observableArray();
    self.arrDS = ko.observableArray();
    self.data_DoanhSo = ko.observableArray();
    var stypes;
    var viewPrint = true;
    self.getList_LoadBieuDoBanHang = function () {
        hidewait_TR('table_h');
        self.arrNT([]);
        self.arrDS([]);
        self.data_DoanhSo([]);
        ajaxHelper(ReportUri + "getList_LoadBieuDoBanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&LoaiBieuDo=" + LoaiBieuDo + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.DoanhThuBanHang(data.LstData);
            if (data.LstData.length > 0)
            {
                viewPrint = true;
                stypes = 'Doanh thu thuần: ' + self.TodayBC();
                var dvt = 0;
                for (var i = 0; i < self.DoanhThuBanHang().length; i++) {
                    if (dvt < self.DoanhThuBanHang()[i].ThanhTien) {
                        dvt = self.DoanhThuBanHang()[i].ThanhTien;
                    }
                }
                if (dvt >= 1000000000) {
                    self.DonViTinh_DoanhSo(1000000000)
                    self.DonViTinhDS(" tỷ");
                }
                if (dvt >= 1000000 & dvt < 1000000000) {
                    self.DonViTinh_DoanhSo(1000000)
                    self.DonViTinhDS(" tr");
                }
                if (dvt >= 1000 & dvt < 1000000) {
                    self.DonViTinh_DoanhSo(1000)
                    self.DonViTinhDS(" k");
                }
                for (var i = 0; i < data.LstChiNhanh.length; i++) {
                    for (var j = 0; j < data.LstDate.length; j++) {
                        for (var k = 0; k < data.LstData.length; k++) {
                            if (data.LstData[k].TenDonVi == data.LstChiNhanh[i].TenDonVi && data.LstData[k].Datetime == data.LstDate[j].Datetime) {
                                self.arrDS.push(data.LstData[k].ThanhTien);
                                break;
                            }
                            if (k == data.LstData.length - 1) {
                                self.arrDS.push(0);
                            }
                        }
                    }
                    var obj = {
                        name: data.LstChiNhanh[i].TenDonVi,
                        turboThreshold: i,
                        _colorIndex: i,
                        data: self.arrDS()
                    }
                    self.data_DoanhSo.push(obj);
                    self.arrDS([]);
                }

                if (LoaiBieuDo == 1) {
                    for (var i = 0; i < data.LstDate.length; i++) {
                        _loadDate = data.LstDate[i].Datetime + ":00";
                        self.arrNT.push(_loadDate);
                    }
                }
                else {
                    for (var i = 0; i < data.LstDate.length; i++) {
                        if (data.LstDate[i].Datetime.length == 1) {
                            _loadDate = "0" + data.LstDate[i].Datetime;
                        }
                        else {
                            _loadDate = data.LstDate[i].Datetime;
                        }
                        self.arrNT.push(_loadDate);
                    }
                }
            }
            else {
                stypes = "Báo cáo không có dữ liệu."
                viewPrint = false;
            }
            self.loadDoanhThuBanHang();
            $("div[id ^= 'wait']").text("");
        });
    }
    self.loadDoanhThuBanHang = function () {
        var chart = Highcharts.chart('chart', {
            chart: {
                type: 'column',
                polar: false
            },
            title: {
                text: stypes
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: self.arrNT(),
                crosshair: true
            },
            yAxis: {
                min: 0,
                title: {
                    text: ''
                },
                labels: {
                    formatter: function () {
                        if (this.value == 0)
                            return 0
                        else
                            return Highcharts.numberFormat(this.value / self.DonViTinh_DoanhSo(), 0, '.', ',') + self.DonViTinhDS();
                    }
                }
            },
            tooltip: {
                footerFormat: "</table>",
                shared: false,
                useHTML: true,
                formatter: function () {
                    return "</span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize;\">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 0, '.', ',') + "</b>";
                }
            },
            plotOptions: {
                series: {
                    stacking: "normal",
                    animation: false,
                    // pointWidth: 30,
                    maxPointWidth: 30,
                    dataLabels: {
                        style: {
                            color: "contrast",
                            fontSize: "11px",
                            fontWeight: "bold",
                            textOutline: "1px 1px contrast"
                        }
                    }
                },
                column: {
                    pointPadding: 0.2,
                    borderWidth: 0
                }
            },
            series: self.data_DoanhSo(),
            navigation: {
                buttonOptions: {
                    align: 'right',
                    verticalAlign: 'bottom',
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
                    }
                }
            },
            colors: [
                "#32b7b3",
                "#ef6c00",
                "#8085e9",
                "#2979ff",
                "#8085e9",
                "#f15c80",
                "#e4d354",
                "#2b908f",
                "#f45b5b",
                "#91e8e1"
            ],
            legend: {
                align: "center",
                verticalAlign: "bottom",
                x: 20
            },
            credits: {
                enabled: false,
            }
        });
    }
    
}
ko.applyBindings(new ViewModel());
function hidewait_TR(o) {
    $('.' + o).append('<div id="wait"><img src="/Content/images/wait.gif" width="64" height="64" /><div class="happy-wait">' +
        ' </div>' +
        '</div>')
}
