var ViewModel = function () {
    var self = this;
    self.ColumnsExcelHH = ko.observableArray();
    self.ColumnsExcelGD = ko.observableArray();
    var cacheExcelHH = true;
    var cacheExcelGD = true;
    var thisDate;
    self.MangChiNhanh = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _nameDonViSeach = null;
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _IDDoiTuong = $('.idnguoidung').text();

    self.txtMoiQuanTam = ko.observable("hàng hóa")
    self.MangNguoiBan = ko.observableArray();
    self.searchNguoiban = ko.observableArray()
    self.NguoiBans = ko.observableArray();
    self.Loc_Table = ko.observable('1');
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.HangHoas = ko.observableArray();
    self.ReporttDatHang_HangHoa = ko.observableArray();
    self.ReporttDatHang_HangHoaPrint = ko.observableArray();
    self.ReportDatHang_GiaoDich = ko.observableArray();
    self.ReportDatHang_GiaoDichPrint = ko.observableArray();
    self.ReportHH_XuatNhapTon = ko.observableArray();
    self.ReportHH_XuatNhapTonChiTiet = ko.observableArray();
    self.ReportHH_NhanVien = ko.observableArray();
    self.ReportHH_XuatHuy = ko.observableArray();
    self.ReportHH_KhachHang = ko.observableArray();
    self.ReportHH_NhaCungCap = ko.observableArray();
    self.TongCongHH_BanHang = ko.observableArray();
    self.TongCongHH_LoiNhuan = ko.observableArray();
    self.TongCongHH_XuatNhapTon = ko.observableArray();
    self.TongCongHH_XuatNhapTonChiTiet = ko.observableArray();
    self.TongCongHH_NhanVien = ko.observableArray();
    self.TongCongHH_SoLuongXuatHuy = ko.observable();
    self.TongCongHH_GiaTriXuatHuy = ko.observable();
    self.TongCongHH_KhachHang = ko.observableArray();
    self.TongCongHH_NhaCungCap = ko.observableArray();
    self.MoiQuanTam = ko.observable('Báo cáo đặt hàng theo hàng hóa');
    var _id_DonVi = $('#hd_IDdDonVi').val();
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.TodayBC = ko.observable('Tuần này');
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.RowsStartHangHoa = ko.observable('1');
    self.RowsEndHangHoa = ko.observable('10');
    self.RowsStartGiaoDich = ko.observable('1');
    self.RowsEndGiaoDich = ko.observable('10');
    self.SumRowsHangHoa = ko.observable();
    self.NhomHangHoas = ko.observableArray();
    var _tenNguoiBanSeach = null;
    var dt1 = new Date();
    var tk = null;
    //var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    //var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');

    var currentWeekDay1 = dt1.getDay();
    var lessDays1 = currentWeekDay1 == 0 ? 6 : currentWeekDay1 - 1
    var _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - lessDays1))).format('YYYY-MM-DD'); // start of wwek
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 7))).format('YYYY-MM-DD'); // end of week

    var _maHH = null;
    var _maKH = null;
    var _laHangHoa = 2;
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var BH_KhuyenMaiUri = '/api/DanhMuc/BH_KhuyenMaiAPI/';
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _id_NhanVien = $('.idnhanvien').text();
    //trinhpv phân quyền
    self.BCDatHang = ko.observable();
    self.BCDH_HangHoa = ko.observable();
    self.BCDH_GiaoDich_XuatFile = ko.observable();
    self.BCDH_GiaoDich = ko.observable();
    self.BCDH_HangHoa_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCDatHang", "GET").done(function (data) {
            self.BCDatHang(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCDH_HangHoa", "GET").done(function (data) {
            self.BCDH_HangHoa(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCDH_GiaoDich_XuatFile", "GET").done(function (data) {
            self.BCDH_GiaoDich_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCDH_GiaoDich", "GET").done(function (data) {
            self.BCDH_GiaoDich(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCDH_HangHoa_XuatFile", "GET").done(function (data) {
            self.BCDH_HangHoa_XuatFile(data);
            console.log(data);
        })
    }
    getQuyen_NguoiDung();

    self.currentPage = ko.observable(1);
    self.currentPageHangHoa = ko.observable(1);
    self.currentPageGiaoDich = ko.observable(1);
    var _ID_NhomHang = null;
    $('#home').removeClass("active")
    $('#info').addClass("active")
    //Select table Report
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
        getAllNSNhanVien();
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });

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
            getAllNSNhanVien();
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

    var _kieubang = 1;
    self.check_kieubang = ko.observable('2');
    $('.chose_kieubang li').on('click', function () {
        self.check_kieubang($(this).val());
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }
    })

    $('.chooseTableBC input').on('click', function () {
        self.Loc_Table($(this).val())
        _kieubang = $(this).val();
        self.hideTableReport();
        _pageNumber = 1;
        if ($(this).val() == 1) {
            self.txtMoiQuanTam("hàng hóa")
            $(".list_DHHangHoa").show();
            $(".list_DHGiaoDich").hide();
            $('.table_HangHoa').show();
            self.MoiQuanTam('Báo cáo đặt hàng theo hàng hóa');
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else {
            self.txtMoiQuanTam("phiếu")
            $(".list_DHHangHoa").hide();
            $(".list_DHGiaoDich").show();
            $('.table_GiaoDich').show();
            self.MoiQuanTam('Báo cáo danh sách đơn đặt hàng');
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }
    })
    self.hideTableReport = function () {
        $('.table_HangHoa').hide();
        $('.table_GiaoDich').hide();
    }
    self.hideTableReport();
    $('.table_HangHoa').show();
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
                    self.getListReportDatHang_HangHoa_BieuDo();
                else
                    self.getListReportDatHang_HangHoa();
            }
            //self.getListReportDatHang_HangHoa();
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportDatHang_GiaoDich_BieuDo();
                else
                    self.getListReportDatHang_GiaoDich();
            }
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
                        self.getListReportDatHang_HangHoa_BieuDo();
                    else
                        self.getListReportDatHang_HangHoa();
                }
                else if (_kieubang == 2) {
                    if (self.check_kieubang() == 1)
                        self.getListReportDatHang_GiaoDich_BieuDo();
                    else
                        self.getListReportDatHang_GiaoDich();
                }
            }
        }
    })
    //function getAllNSNhanVien() {
    //    ajaxHelper(BH_KhuyenMaiUri + "getNhanViens?nameChinhanh=" + _id_DonVi, 'GET').done(function (data) {
    //        self.NguoiBans(data);
    //        self.searchNguoiban(data);
    //        if (_kieubang == 1)
    //        {
    //            if (self.check_kieubang() == 1)
    //                self.getListReportDatHang_HangHoa_BieuDo();
    //            else
    //                self.getListReportDatHang_HangHoa();
    //        }
    //        else if (_kieubang == 2)

    //        {
    //            if (self.check_kieubang() == 1)
    //                self.getListReportDatHang_GiaoDich_BieuDo();
    //            else
    //                self.getListReportDatHang_GiaoDich();
    //        }
    //    });

    //}
    //getAllNSNhanVien();
    function getAllNSNhanVien() {
        ajaxHelper(BH_KhuyenMaiUri + "getNhanViens?nameChinhanh=" + _idDonViSeach, 'GET').done(function (data) {
            self.NguoiBans(data);
            self.searchNguoiban(data);
            for (var i = 0; i < self.NguoiBans().length; i++) {
                if (i == 0) {
                    _tenNguoiBanSeach = self.NguoiBans()[i].ID;
                }
                else {
                    _tenNguoiBanSeach = self.NguoiBans()[i].ID + "," + _tenNguoiBanSeach;
                }
            }
            nextPage = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListReportDatHang_HangHoa_BieuDo();
                else
                    self.getListReportDatHang_HangHoa();
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportDatHang_GiaoDich_BieuDo();
                else
                    self.getListReportDatHang_GiaoDich();
            }
        });
    }
    self.CloseNguoiBan = function (item) {
        _tenNguoiBanSeach = null;
        self.MangNguoiBan.remove(item);
        for (var i = 0; i < self.MangNguoiBan().length; i++) {
            if (_tenNguoiBanSeach == null)
                _tenNguoiBanSeach = self.MangNguoiBan()[i].ID;
            else
                _tenNguoiBanSeach = self.MangNguoiBan()[i].ID + "," + _tenNguoiBanSeach;
        }
        if (self.MangNguoiBan().length === 0) {
            for (var i = 0; i < self.searchNguoiban().length; i++) {
                if (_tenNguoiBanSeach == null)
                    _tenNguoiBanSeach = self.searchNguoiban()[i].ID;
                else
                    _tenNguoiBanSeach = self.searchNguoiban()[i].ID + "," + _tenNguoiBanSeach;
            }
            //_tenNguoiBanSeach = null;
        }
        // remove check
        $('#selec-all-NguoiBan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNguoiBan())
        nextPage = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }
    }
    self.SelectedNguoiBan = function (item) {
        _tenNguoiBanSeach = null;
        var arrIDNguoiBan = [];
        for (var i = 0; i < self.MangNguoiBan().length; i++) {
            if ($.inArray(self.MangNguoiBan()[i], arrIDNguoiBan) === -1) {
                arrIDNguoiBan.push(self.MangNguoiBan()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNguoiBan) === -1) {
            self.MangNguoiBan.push(item);
            for (var i = 0; i < self.MangNguoiBan().length; i++) {
                if (_tenNguoiBanSeach == null)
                    _tenNguoiBanSeach = self.MangNguoiBan()[i].ID;
                else
                    _tenNguoiBanSeach = self.MangNguoiBan()[i].ID + "," + _tenNguoiBanSeach;
            }
        }

        $('#NoteNameNguoiBan').val('');
        self.NguoiBans(self.searchNguoiban());
        //đánh dấu check
        for (var i = 0; i < self.MangNguoiBan().length; i++) {
            $('#selec-all-NguoiBan li').each(function () {
                if ($(this).attr('id') === self.MangNguoiBan()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
        }
        console.log(self.MangNguoiBan())
        nextPage = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }

    }
    self.NoteNameNhanVien = function () {
        console.log(self.searchNguoiban())
        console.log($('#NoteNameNguoiBan').val())
        var arrNguoiBan = [];
        var itemSearch = locdau($('#NoteNameNguoiBan').val().toLowerCase());
        for (var i = 0; i < self.searchNguoiban().length; i++) {
            var locdauInput = locdau(self.searchNguoiban()[i].TenNhanVien).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrNguoiBan.push(self.searchNguoiban()[i]);
            }
        }
        self.NguoiBans(arrNguoiBan);
        if ($('#NoteNameNguoiBan').val() == "") {
            self.NguoiBans(self.searchNguoiban());
            //console.log(self.NguoiBans())
        }

    }
    $('#NoteNameNguoiBan').keypress(function (e) {
        if (e.keyCode == 13 && self.NguoiBans().length > 0) {
            self.SelectedNguoiBan(self.NguoiBans()[0]);
        }
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
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }

    });
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        console.log(picker.startDate.format('DD/MM/YYYY'), picker.endDate.format('DD/MM/YYYY'))
        LoaiBieuDo = 2;
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        console.log(_timeStart, _timeEnd);
        self.TodayBC($(this).val())
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }
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
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }
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
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }
    })
    // Key Event maHH
    self.SelectMaHH = function () {
        _maHH = $('#txtMaHH').val();
        console.log(_maHH);
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {
            _pageNumber = 1;
            console.log(_kieubang)
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListReportDatHang_HangHoa_BieuDo();
                else
                    self.getListReportDatHang_HangHoa();
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportDatHang_GiaoDich_BieuDo();
                else
                    self.getListReportDatHang_GiaoDich();
            }
        }
    })
    // Key Event maKH
    self.SelectMaKH = function () {
        _maKH = $('#txtMaKH').val();
        console.log(_maKH);
    }
    $('#txtMaKH').keypress(function (e) {
        if (e.keyCode == 13) {
            _pageNumber = 1;
            console.log(_kieubang)
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListReportDatHang_HangHoa_BieuDo();
                else
                    self.getListReportDatHang_HangHoa();
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListReportDatHang_GiaoDich_BieuDo();
                else
                    self.getListReportDatHang_GiaoDich();
            }
        }
    })
    //GetListNhomHangHoa
    //function getNhomHangHoa() {
    //    ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
    //        self.NhomHangHoas(data);
    //        console.log(self.NhomHangHoas());
    //    })
    //}
    //getNhomHangHoa();

    function GetAllNhomHH() {
        self.NhomHangHoas([]);
        ajaxHelper(ReportUri + "GetListID_NhomHangHoa?TenNhomHang=" + tk, 'GET').done(function (data) {
            //console.log(data);
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
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_HangHoa_BieuDo();
            else
                self.getListReportDatHang_HangHoa();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListReportDatHang_GiaoDich_BieuDo();
            else
                self.getListReportDatHang_GiaoDich();
        }
    }
    //$('.SelectALLNhomHang').on('click', function () {
    //    _ID_NhomHang = null;
    //    _pageNumber = 1;
    //    if (_kieubang == 1)
    //        self.getListReportDatHang_HangHoa();
    //    else if (_kieubang == 2)
    //        self.getListReportDatHang_GiaoDich();
    //});
    //GetListHangHoa_BanHang
    var _pageNumber = 1;
    var _pageSize = 10;
    var AllPage;
    var _pageNumberHangHoa = 1;
    var AllPageHangHoa;
    var _pageNumberGiaoDich = 1;
    var AllPageGiaoDich;
    self.HH_SoLuongDat = ko.observable();
    self.HH_TongTienHang = ko.observable();
    self.HH_GiamGiaHD = ko.observable();
    self.HH_GiaTriDat = ko.observable();
    //$('.PhanQuyen').hide();
    //$('.TongCongHangHoa').hide();
    //$('.page').hide();
    self.getListReportDatHang_HangHoa = function () {
        if (self.BCDH_HangHoa() == "BCDH_HangHoa") {
            $('.PhanQuyen').hide();
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListDatHang_HangHoa?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&laHangHoa=" + _laHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                console.log(data);
                self.ReporttDatHang_HangHoa(data.LstData);
                self.ReporttDatHang_HangHoaPrint(data.LstDataPrint);
                self.HH_SoLuongDat(data.SoLuongBan);
                self.HH_TongTienHang(data.GiaTriBan);
                self.HH_GiamGiaHD(data.GiaTriTra);
                self.HH_GiaTriDat(data.DoanhThuThuan);
                if (self.ReporttDatHang_HangHoa().length != 0) {
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReporttDatHang_HangHoa().length)
                }
                else {
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                if (self.ReporttDatHang_HangHoa().length > 0) {
                    $('.TongCongHangHoa').show();
                    $('.page').show();
                    $('.Report_Empty').hide();
                }
                else {
                    $('.Report_Empty').show();
                    $('.TongCongHangHoa').hide();
                    $('.page').hide();
                }
                LoadHtmlGrid(cacheExcelHH, 1, "dathangHH")
            });
        }
        else {
            $('.PhanQuyen').show();
            $('.Report_Empty').hide();
            $('.TongCongHangHoa').hide();
            $('.page').hide();
        }
    }
    //self.getListReportDatHang_HangHoa();
    self.SelectedPageNumberReporttDatHang_HangHoa = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListDatHang_HangHoa?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReporttDatHang_HangHoa(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReporttDatHang_HangHoa().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelHH, 1, "dathangHH")
        });
    }
    self.ReporttDatHang_HangHoaChiTiet = ko.observableArray();
    self.ReporttDatHang_HangHoaChiTietPrint = ko.observableArray();
    self.MaHangPrint = ko.observable();
    self.TenHangPrint = ko.observable();
    self.ThuocTinh_GiaTriPrint = ko.observable();
    self.TenDonViTinhPrint = ko.observable();
    self.TenLoHangPrint = ko.observable();
    self.SumRowsDatHang_HangHoa = ko.observable();
    self.SumNumberPageReport_HangHoa = ko.observableArray();
    self.HHCT_SoLuongDat = ko.observable();
    self.HHCT_TongTienHang = ko.observable();
    self.HHCT_GiamGiaHD = ko.observable();
    self.HHCT_GiaTriDat = ko.observable();

    var _maHangHoa;
    self.SelectDatHang_HangHoaChiTiet = function (item) {
        self.MaHangPrint(item.MaHangHoa);
        self.TenHangPrint(item.TenHangHoa);
        self.ThuocTinh_GiaTriPrint(item.ThuocTinh_GiaTri);
        self.TenDonViTinhPrint(item.TenDonViTinh);
        self.TenLoHangPrint(item.TenLoHang);
        self.HHCT_SoLuongDat(item.SoLuongDat);
        self.HHCT_TongTienHang(item.TongTienHang);
        self.HHCT_GiamGiaHD(item.GiamGiaHD);
        self.HHCT_GiaTriDat(item.GiaTriDat);
        hidewait('table_h');
        _maHangHoa = item.MaHangHoa;
        ajaxHelper(ReportUri + "getListDatHang_HangHoaChiTiet?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoa + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            console.log(data);
            self.ReporttDatHang_HangHoaChiTiet(data.LstData);
            self.ReporttDatHang_HangHoaChiTietPrint(data.LstDataPrint);
            if (self.ReporttDatHang_HangHoaChiTiet().length != 0) {
                self.RowsStartHangHoa((_pageNumberHangHoa - 1) * _pageSize + 1);
                self.RowsEndHangHoa((_pageNumberHangHoa - 1) * _pageSize + self.ReporttDatHang_HangHoaChiTiet().length)
            }
            else {
                self.RowsStartHangHoa('0');
                self.RowsEndHangHoa('0');
            }
            self.SumNumberPageReport_HangHoa(data.LstPageNumber);
            AllPageHangHoa = self.SumNumberPageReport_HangHoa().length;
            self.selecPageHangHoa();
            self.ReserPageHangHoa();
            self.SumRowsDatHang_HangHoa(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });

    }
    self.SelectedPageNumberReporttDatHang_HangHoaChiTiet = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListDatHang_HangHoaChiTiet?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberHangHoa + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReporttDatHang_HangHoaChiTiet(data.LstData);
            self.RowsStartHangHoa((_pageNumberHangHoa - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumberHangHoa - 1) * _pageSize + self.ReporttDatHang_HangHoaChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.ReportDatHang_GiaoDich = ko.observableArray();
    self.GD_SoLuongDat = ko.observable();
    self.GD_TongTienHang = ko.observable();
    self.GD_GiamGiaHD = ko.observable();
    self.GD_GiaTriDat = ko.observable();
    self.getListReportDatHang_GiaoDich = function () {
        if (self.BCDH_GiaoDich() == "BCDH_GiaoDich") {
            $('.PhanQuyen').hide();
           
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListDatHang_GiaoDich?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportDatHang_GiaoDich(data.LstData);
                self.ReportDatHang_GiaoDichPrint(data.LstDataPrint);
                self.GD_SoLuongDat(data.SoLuongBan);
                self.GD_GiaTriDat(data.DoanhThuThuan);
                self.GD_TongTienHang(data.GiaTriBan);
                self.GD_GiamGiaHD(data.GiaTriTra);
                if (self.ReportDatHang_GiaoDich().length != 0) {
                    $('.Report_Empty').hide();
                    $('.page').show();
                    $('.TongCongGiaoDich').show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportDatHang_GiaoDich().length)
                }
                else {
                    $('.Report_Empty').show();
                    $('.TongCongGiaoDich').hide();
                    $('.page').hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                LoadHtmlGrid(cacheExcelGD, 2, "dathangGD");
            });
        }
        else {
            $('.PhanQuyen').show();
            $('.TongCongGiaoDich').hide();
            $('.Report_Empty').hide();
            $('.page').hide();
        }
    }
    self.SelectedPageNumberReportDatHang_GiaoDich = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListDatHang_GiaoDich?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportDatHang_GiaoDich(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportDatHang_GiaoDich().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelGD, 2, "dathangGD");
        });
    }

    self.ReporttDatHang_GiaoDichChiTiet = ko.observableArray();
    self.ReporttDatHang_GiaoDichChiTietPrint = ko.observableArray();
    self.MaPhieuPrint = ko.observable();
    self.TenKhachPrint = ko.observable();
    self.SLDatPrint = ko.observable();
    self.SLNhanPrint = ko.observable();
    self.SumRowsDatHang_GiaoDich = ko.observable();
    self.SumNumberPageReport_GiaoDich = ko.observableArray();
    var _maGiaoDich;
    self.SelectDatHang_GiaoDichChiTiet = function (item) {
        self.MaPhieuPrint(item.MaHoaDon);
        self.TenKhachPrint(item.TenKhachHang);
        hidewait('table_h');
        _maGiaoDich = item.MaHoaDon;
        ajaxHelper(ReportUri + "getListDatHang_GiaoDichChiTiet?MaHoaDon=" + _maGiaoDich + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberGiaoDich + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReporttDatHang_GiaoDichChiTiet(data.LstData);
            self.ReporttDatHang_GiaoDichChiTietPrint(data.LstDataPrint);
            self.SLDatPrint(data._tienvon);
            self.SLNhanPrint(data._thanhtien);
            console.log(data);
            if (self.ReporttDatHang_GiaoDichChiTiet().length != 0) {
                self.RowsStartGiaoDich((_pageNumberGiaoDich - 1) * _pageSize + 1);
                self.RowsEndGiaoDich((_pageNumberGiaoDich - 1) * _pageSize + self.ReporttDatHang_GiaoDichChiTiet().length)
            }
            else {
                self.RowsStartGiaoDich('0');
                self.RowsEndGiaoDich('0');
            }
            self.SumNumberPageReport_GiaoDich(data.LstPageNumber);
            AllPageGiaoDich = self.SumNumberPageReport_GiaoDich().length;
            self.selecPageGiaoDich();
            self.ReserPageGiaoDich();
            self.SumRowsDatHang_GiaoDich(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });

    }
    self.SelectedPageNumberReporttDatHang_GiaoDichChiTiet = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListDatHang_GiaoDichChiTiet?MaHoaDon=" + _maGiaoDich + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberGiaoDich + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReporttDatHang_GiaoDichChiTiet(data.LstData);
            self.RowsStartGiaoDich((_pageNumberGiaoDich - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumberGiaoDich - 1) * _pageSize + self.ReporttDatHang_GiaoDichChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.GetClassHangHoa = function (page) {
        return (page.SoTrang === self.currentPageHangHoa()) ? "click" : "";
    };
    self.GetClassGiaoDich = function (page) {
        return (page.SoTrang === self.currentPageGiaoDich()) ? "click" : "";
    };
    // phân trang hàng hóa chi tiết\
    self.selecPageHangHoa = function () {
        // AllPageHangHoa = self.SumNumberPageReport().length;

        if (AllPageHangHoa > 4) {
            for (var i = 3; i < AllPageHangHoa; i++) {
                self.SumNumberPageReport_HangHoa.pop(i + 1);
            }
            self.SumNumberPageReport_HangHoa.push({ SoTrang: 4 });
            self.SumNumberPageReport_HangHoa.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReport_HangHoa.pop(i);
            }
            for (var j = 0; j < AllPageHangHoa; j++) {
                self.SumNumberPageReport_HangHoa.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageHangHoa').hide();
        $('#BackPageHangHoa').hide();
        $('#NextPageHangHoa').show();
        $('#EndPageHangHoa').show();
    }
    self.ReserPageHangHoa = function (item) {
        //self.selecPage();
        if (_pageNumberHangHoa > 1 && AllPageHangHoa > 5/* && nextPage < AllPageHangHoa - 1*/) {
            if (_pageNumberHangHoa > 3 && _pageNumberHangHoa < AllPageHangHoa - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_HangHoa.replace(self.SumNumberPageReport_HangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberHangHoa) === parseInt(AllPageHangHoa) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_HangHoa.replace(self.SumNumberPageReport_HangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 3 });
                }
            }
            else if (_pageNumberHangHoa == AllPageHangHoa) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_HangHoa.replace(self.SumNumberPageReport_HangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i - 4 });
                }
            }
            else if (_pageNumberHangHoa < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberHangHoa)
                    self.SumNumberPageReport_HangHoa.replace(self.SumNumberPageReport_HangHoa()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberHangHoa == 1 && AllPageHangHoa > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReport_HangHoa.replace(self.SumNumberPageReport_HangHoa()[i], { SoTrang: parseInt(_pageNumberHangHoa) + i });
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
    self.NextPageHangHoa = function (item) {
        if (_pageNumberHangHoa < AllPageHangHoa) {
            _pageNumberHangHoa = _pageNumberHangHoa + 1;
            self.ReserPageHangHoa();
            self.SelectedPageNumberReporttDatHang_HangHoaChiTiet();
        }
    };
    self.BackPageHangHoa = function (item) {
        if (_pageNumberHangHoa > 1) {
            _pageNumberHangHoa = _pageNumberHangHoa - 1;
            self.ReserPageHangHoa();
            self.SelectedPageNumberReporttDatHang_HangHoaChiTiet();
        }
    };
    self.EndPageHangHoa = function (item) {
        _pageNumberHangHoa = AllPageHangHoa;
        self.ReserPageHangHoa();
        self.SelectedPageNumberReporttDatHang_HangHoaChiTiet();
    };
    self.StartPageHangHoa = function (item) {
        _pageNumberHangHoa = 1;
        self.ReserPageHangHoa();
        self.SelectedPageNumberReporttDatHang_HangHoaChiTiet();
    };
    self.gotoNextPageHangHoa = function (item) {
        _pageNumberHangHoa = item.SoTrang;
        self.ReserPageHangHoa();
        self.SelectedPageNumberReporttDatHang_HangHoaChiTiet();
    }
    // phân trang GiaoDich chi tiết\
    self.selecPageGiaoDich = function () {
        // AllPageGiaoDich = self.SumNumberPageReport().length;

        if (AllPageGiaoDich > 4) {
            for (var i = 3; i < AllPageGiaoDich; i++) {
                self.SumNumberPageReport_GiaoDich.pop(i + 1);
            }
            self.SumNumberPageReport_GiaoDich.push({ SoTrang: 4 });
            self.SumNumberPageReport_GiaoDich.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReport_GiaoDich.pop(i);
            }
            for (var j = 0; j < AllPageGiaoDich; j++) {
                self.SumNumberPageReport_GiaoDich.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPageGiaoDich').hide();
        $('#BackPageGiaoDich').hide();
        $('#NextPageGiaoDich').show();
        $('#EndPageGiaoDich').show();
    }
    self.ReserPageGiaoDich = function (item) {
        //self.selecPage();
        if (_pageNumberGiaoDich > 1 && AllPageGiaoDich > 5/* && nextPage < AllPageGiaoDich - 1*/) {
            if (_pageNumberGiaoDich > 3 && _pageNumberGiaoDich < AllPageGiaoDich - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_GiaoDich.replace(self.SumNumberPageReport_GiaoDich()[i], { SoTrang: parseInt(_pageNumberGiaoDich) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberGiaoDich) === parseInt(AllPageGiaoDich) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_GiaoDich.replace(self.SumNumberPageReport_GiaoDich()[i], { SoTrang: parseInt(_pageNumberGiaoDich) + i - 3 });
                }
            }
            else if (_pageNumberGiaoDich == AllPageGiaoDich) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_GiaoDich.replace(self.SumNumberPageReport_GiaoDich()[i], { SoTrang: parseInt(_pageNumberGiaoDich) + i - 4 });
                }
            }
            else if (_pageNumberGiaoDich < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberGiaoDich)
                    self.SumNumberPageReport_GiaoDich.replace(self.SumNumberPageReport_GiaoDich()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberGiaoDich == 1 && AllPageGiaoDich > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReport_GiaoDich.replace(self.SumNumberPageReport_GiaoDich()[i], { SoTrang: parseInt(_pageNumberGiaoDich) + i });
            }
        }
        if (_pageNumberGiaoDich > 1) {
            $('#StartPageGiaoDich').show();
            $('#BackPageGiaoDich').show();
        }
        else {
            $('#StartPageGiaoDich').hide();
            $('#BackPageGiaoDich').hide();
        }
        if (_pageNumberGiaoDich == AllPageGiaoDich) {
            $('#NextPageGiaoDich').hide();
            $('#EndPageGiaoDich').hide();
        }
        else {
            $('#NextPageGiaoDich').show();
            $('#EndPageGiaoDich').show();
        }
        self.currentPageGiaoDich(parseInt(_pageNumberGiaoDich));
    }
    self.NextPageGiaoDich = function (item) {
        if (_pageNumberGiaoDich < AllPageGiaoDich) {
            _pageNumberGiaoDich = _pageNumberGiaoDich + 1;
            self.ReserPageGiaoDich();
            self.SelectedPageNumberReporttDatHang_GiaoDichChiTiet();
        }
    };
    self.BackPageGiaoDich = function (item) {
        if (_pageNumberGiaoDich > 1) {
            _pageNumberGiaoDich = _pageNumberGiaoDich - 1;
            self.ReserPageGiaoDich();
            self.SelectedPageNumberReporttDatHang_GiaoDichChiTiet();
        }
    };
    self.EndPageGiaoDich = function (item) {
        _pageNumberGiaoDich = AllPageGiaoDich;
        self.ReserPageGiaoDich();
        self.SelectedPageNumberReporttDatHang_GiaoDichChiTiet();
    };
    self.StartPageGiaoDich = function (item) {
        _pageNumberGiaoDich = 1;
        self.ReserPageGiaoDich();
        self.SelectedPageNumberReporttDatHang_GiaoDichChiTiet();
    };
    self.gotoNextPageGiaoDich = function (item) {
        _pageNumberGiaoDich = item.SoTrang;
        self.ReserPageGiaoDich();
        self.SelectedPageNumberReporttDatHang_GiaoDichChiTiet();
    }
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
                self.SelectedPageNumberReporttDatHang_HangHoa();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportDatHang_GiaoDich();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.ReserPage();
            if (_kieubang == 1)
                self.SelectedPageNumberReporttDatHang_HangHoa();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportDatHang_GiaoDich();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReporttDatHang_HangHoa();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportDatHang_GiaoDich();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReporttDatHang_HangHoa();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportDatHang_GiaoDich();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReporttDatHang_HangHoa();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportDatHang_GiaoDich();
    }
    //Xuất file excel
    self.addColumHH = function (item) {
        if (self.ColumnsExcelHH().length < 1) {
            self.ColumnsExcelHH.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcelHH().length; i++) {
                if (self.ColumnsExcelHH()[i] === item) {
                    self.ColumnsExcelHH.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcelHH().length - 1) {
                    self.ColumnsExcelHH.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcelHH.sort();
    }
    self.addColumGD = function (item) {
        if (self.ColumnsExcelGD().length < 1) {
            self.ColumnsExcelGD.push(item);
        }
        else {
            for (var i = 0; i < self.ColumnsExcelGD().length; i++) {
                if (self.ColumnsExcelGD()[i] === item) {
                    self.ColumnsExcelGD.splice(i, 1);
                    break;
                }
                if (i == self.ColumnsExcelGD().length - 1) {
                    self.ColumnsExcelGD.push(item);
                    break;
                }
            }
        }
        self.ColumnsExcelGD.sort();
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
                            self.addColumHH(current[i].Value);
                        else if (vals === 2)
                            self.addColumGD(current[i].Value);
                    }
                }
                if (vals === 1)
                    cacheExcelHH = false;
                else if (vals === 2)
                    cacheExcelGD = false;
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
    $('#dhma').click(function () {
        $(".dhma").toggle();
        self.addColumHH($(this).val());
        addClass(".dhma", "dhma", $(this).val(), "dathangHH");
    });
    $('#dhtenhang').click(function () {
        $(".dhtenhang").toggle();
        self.addColumHH($(this).val());
        addClass(".dhtenhang", "dhtenhang", $(this).val(), "dathangHH");
    });
    $('#dhsoluong ').click(function () {
        $(".dhsoluong ").toggle();
        self.addColumHH($(this).val());
        addClass(".dhsoluong", "dhsoluong", $(this).val(), "dathangHH");
    });
    $('#dhtongtien ').click(function () {
        $(".dhtongtien ").toggle();
        self.addColumHH($(this).val());
        addClass(".dhtongtien", "dhtongtien", $(this).val(), "dathangHH");
    });
    $('#dhgiamgia ').click(function () {
        $(".dhgiamgia ").toggle();
        self.addColumHH($(this).val());
        addClass(".dhgiamgia", "dhgiamgia", $(this).val(), "dathangHH");
    });
    $('#dhgiatri ').click(function () {
        $(".dhgiatri ").toggle();
        self.addColumHH($(this).val());
        addClass(".dhgiatri", "dhgiatri", $(this).val(), "dathangHH");
    });

    $('#gdma').click(function () {
        $(".gdma").toggle();
        self.addColumGD($(this).val());
        addClass(".gdma", "gdma", $(this).val(), "dathangGD");
    });
    $('#gdtime ').click(function () {
        $(".gdtime ").toggle();
        self.addColumGD($(this).val());
        addClass(".gdtime", "gdtime", $(this).val(), "dathangGD");
    });
    $('#gdkhachhang ').click(function () {
        $(".gdkhachhang ").toggle();
        self.addColumGD($(this).val());
        addClass(".gdkhachhang", "gdkhachhang", $(this).val(), "dathangGD");
    });
    $('#gdsoluong  ').click(function () {
        $(".gdsoluong  ").toggle();
        self.addColumGD($(this).val());
        addClass(".gdsoluong", "gdsoluong", $(this).val(), "dathangGD");
    });
    $('#gdtongtien').click(function () {
        $(".gdtongtien").toggle();
        self.addColumGD($(this).val());
        addClass(".gdtongtien", "gdtongtien", $(this).val(), "dathangGD");
    });
    $('#gdgiamgia  ').click(function () {
        $(".gdgiamgia  ").toggle();
        self.addColumGD($(this).val());
        addClass(".gdgiamgia", "gdgiamgia", $(this).val(), "dathangGD");
    });
    $('#gdgiatri   ').click(function () {
        $(".gdgiatri   ").toggle();
        self.addColumGD($(this).val());
        addClass(".gdgiatri", "gdgiatri", $(this).val(), "dathangGD");
    });
    self.ExportExcel = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo đặt hàng",
            NoiDung: "Xuất " + self.MoiQuanTam(),
            NoiDungChiTiet: "Xuất " + self.MoiQuanTam()
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
                if (_kieubang == 1 && self.ReporttDatHang_HangHoa().length != 0) {
                    if (self.BCDH_HangHoa() == "BCDH_HangHoa" && self.BCDH_HangHoa_XuatFile() == "BCDH_HangHoa_XuatFile") {
                        for (var i = 0; i < self.ColumnsExcelHH().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelHH()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelHH()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelDatHang_HangHoa?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&laHangHoa=" + _laHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");

                }
                else if (_kieubang == 2 && self.ReportDatHang_GiaoDich().length != 0) {
                    if (self.BCDH_GiaoDich() == "BCDH_GiaoDich" && self.BCDH_GiaoDich_XuatFile() == "BCDH_GiaoDich_XuatFile") {
                        for (var i = 0; i < self.ColumnsExcelGD().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelGD()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelGD()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelDatHang_GiaoDich?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&laHangHoa=" + _laHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
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
    self.ExportExcel_HangHoaChiTiet = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo đặt hàng",
            NoiDung: "Xuất báo cáo đặt hàng theo chi tiết hàng hóa, Mã hàng: " + _maHangHoa,
            NoiDungChiTiet: "Xuất báo cáo đặt hàng theo chi tiết hàng hóa, Mã hàng: " + _maHangHoa,
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
                var url = ReportUri + "ExportExcelDatHang_HangHoaChiTiet?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHangHoa + "&laHangHoa=" + _laHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
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
    self.ExportExcel_GiaoDichChiTiet = function () {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo đặt hàng",
            NoiDung: "Xuất báo cáo đặt hàng theo chi tiết giao dịch, Mã hóa đơn: " + _maGiaoDich,
            NoiDungChiTiet: "Xuất báo cáo đặt hàng theo chi tiết giao dịch, Mã hóa đơn: " + _maGiaoDich,
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
                var url = ReportUri + "ExportExcelDatHang_GiaoDichChiTiet?MaHoaDon=" + _maGiaoDich + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
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
    self.arrHH = ko.observableArray();
    self.arrDT = ko.observableArray();
    self.DoanhThuTT = ko.observableArray();
    self.DonViTinh = ko.observable("Đơn vị tính: hàng đơn vị");
    var _dataDS;
    var _data;
    var nameChar;
    var style;
    self.getListReportDatHang_HangHoa_BieuDo = function () {
        ajaxHelper(ReportUri + "getListDatHang_BanHang_BieuDo?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&laHangHoa=" + _laHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.DoanhThuTT(data);
            console.log(data);
            self.arrDT([]);
            self.arrHH([]);
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
                    //_data = parseFloat(self.DoanhThuTT()[i].Rowsn / _MauSoDVT).toFixed(2) * 1;
                    self.arrDT.push(self.DoanhThuTT()[i].Rowsn);
                    self.arrHH.push(_loadHangHoa);
                }
                style = 'Top 10 sản phẩm có số lượng đặt hàng lớn nhất';
                nameChar = "Số lượng";
            }
            else {
                style = "Báo cáo không có dữ liệu.";
                self.DonViTinh([]);
            }
            self.loadBieuDo();
        });
    }

    self.getListReportDatHang_GiaoDich_BieuDo = function () {
        ajaxHelper(ReportUri + "getListDatHang_GiaoDich_BieuDo?ID_NhanVien=" + _tenNguoiBanSeach + "&maKH=" + _maKH + "&maHH=" + _maHH + "&laHangHoa=" + _laHangHoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.DoanhThuTT(data);
            self.arrDT([]);
            self.arrHH([]);
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
                style = 'Top 10 giao dịch đặt hàng có giá trị lớn nhất';
                nameChar = "Giá trị đặt";
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
                text: style,
            },
            subtitle: {
                text: ''
            },
            tooltip: {
                footerFormat: "</table>",
                shared: false,
                useHTML: true,
                formatter: function () {
                    if (nameChar == "Số lượng")
                        return "<span style=\"font-size:10px\">" + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; \">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 1, '.', ',').replace('.0', '') + '</b>';
                    else
                        return "<span style=\"font-size:10px\">" + this.key + "<br> </span><table>" + '<b style=\"color:{series.color};padding:0; text-align:Left; text-transform: capitalize; \">' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 0, '.', ',') + '</b>';
                }
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
                    }
                }
            },
            colors: [
                "#32b7b3",
            ],
            // hiển thị giá trị lên đầu cột
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: false
                    }
                }
            },
            credits: {
                enabled: false,
            },
            legend: {
                enabled: false
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