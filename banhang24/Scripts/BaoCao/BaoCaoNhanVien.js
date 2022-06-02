var ViewModel = function () {
    var self = this;
    self.ColumnsExcelBHNV = ko.observableArray();
    self.ColumnsExcelLNNV = ko.observableArray();
    self.ColumnsExcelHBTNV = ko.observableArray();
    var cacheExcelBHNV = true;
    var cacheExcelLNNV = true;
    var cacheExcelHBTNV = true;
    self.MangChiNhanh = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _nameDonViSeach = null;
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _IDDoiTuong = $('.idnguoidung').text();
    var thisDate;
    self._ThanhTien = ko.observable(0);
    self._TienVon = ko.observable(0);
    self._LaiLo = ko.observable(0);
    self.Loc_Table = ko.observable('1');
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.HangHoas = ko.observableArray();
    self.ReportHH_BanHang = ko.observableArray();
    self.ReportHH_BanHangPrint = ko.observableArray();
    self.ReportHH_LoiNhuan = ko.observableArray();
    self.ReportHH_LoiNhuanPrint = ko.observableArray();
    self.ReportHangBan_NhanVien = ko.observableArray();
    self.ReportHangBan_NhanVienPrint = ko.observableArray();
    self.ReportHangBanChiTiet_NhanVien = ko.observableArray();
    self.ReportHangBanChiTiet_NhanVienPrint = ko.observableArray();
    self.MaNVPrint = ko.observable();
    self.TenNVPrint = ko.observable();
    self.HB_SoLuongPrint = ko.observable();
    self.HB_GiaTriPrint = ko.observable();

    $('#NoteNameNguoiBan').focus();
    self.MoiQuanTam = ko.observable('Báo cáo bán hàng theo nhân viên');
    //var _id_DonVi = $('.branch label').attr('id');
    var _id_DonVi = $('#hd_IDdDonVi').val();
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.TodayBC = ko.observable('Hôm nay');
    self.SumNumberPageReport = ko.observableArray();
    self.SumNumberPageReportNhapHang = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.RowsStartNhapHang = ko.observable('1');
    self.RowsEndNhapHang = ko.observable('10');
    self.SumRowsHangHoa = ko.observable();
    self.SumRowsHangHoaNhapHang = ko.observable();
    self.NhomHangHoas = ko.observableArray();
    self.Loc_Table = ko.observable('1');
    self.MangNguoiBan = ko.observableArray();
    self.searchNguoiban = ko.observableArray()
    self.NguoiBans = ko.observableArray();
    self.SumDoanhThu = ko.observable();
    self.SumGiaTriTra = ko.observable();
    self.SumDoanhThuThuan = ko.observable();
    self.SumTongTienHang = ko.observable();
    self.SumGiamGiaHD = ko.observable();
    self.SumDoanhThu_LN = ko.observable();
    self.SumGiaTriTra_LN = ko.observable();
    self.SumDoanhThuThuan_LN = ko.observable();
    self.SumTongGiaVon = ko.observable();
    self.SumLoiNhuanGop = ko.observable();
    self.SumSoLuongHangBan = ko.observable();
    self.SumGiaTriHangHang = ko.observable();
    var _tenNguoiBanSeach;
    var dt1 = new Date();
    var tk = null;
    //var _timeStart = '2015-09-26'
    //var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    var _maHH = null;
    var _laHangHoa = 2;
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var BH_KhuyenMaiUri = '/api/DanhMuc/BH_KhuyenMaiAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _id_NhanVien = $('.idnhanvien').text();
    $('.TongCongBanHang').hide();
    $('.TongCongLoiNhuan').hide();
    $('.TongCongHangBan').hide();
    //trinhpv phân quyền
    self.BCNhanVien = ko.observable();
    self.BCNV_BanHang = ko.observable();
    self.BCNV_BanHang_XuatFile = ko.observable();
    self.BCNV_HangBan = ko.observable();
    self.BCNV_HangBan_XuatFile = ko.observable();
    self.BCNV_LoiNhuan = ko.observable();
    self.BCNV_LoiNhuan_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNhanVien", "GET").done(function (data) {
            self.BCNhanVien(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_BanHang", "GET").done(function (data) {
            self.BCNV_BanHang(data);
            self.getListReportHH_BanHang();
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_BanHang_XuatFile", "GET").done(function (data) {
            self.BCNV_BanHang_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_HangBan", "GET").done(function (data) {
            self.BCNV_HangBan(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_HangBan_XuatFile", "GET").done(function (data) {
            self.BCNV_HangBan_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_LoiNhuan", "GET").done(function (data) {
            self.BCNV_LoiNhuan(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_LoiNhuan_XuatFile", "GET").done(function (data) {
            self.BCNV_LoiNhuan_XuatFile(data);
            console.log(data);
        })
    }
    getQuyen_NguoiDung();
    self.currentPage = ko.observable(1);
    self.currentPageNhapHang = ko.observable(1);
    var _ID_NhomHang = null;
    $('#home').removeClass("active")
    $('#info').addClass("active")
    self.check_kieubang = ko.observable('2');
    $('.chose_kieubang li').on('click', function () {
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
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
    })
    self.hideTableReport = function () {
        $('.table_BanHang').hide();
        $('.table_LoiNhuan').hide();
        $('.table_NhanVien').hide();
    }
    self.hideTableReport();
    self.hideradio = function () {
        $('.rd_HangHoa').hide();
        $('.rd_NhomHang').hide();
        $('.rd_LoaiHang').hide();
    }
    self.hideradio();
    $('.table_BanHang').show();
    $(".column-hide").hide();
    var _kieubang = 1;
    $('.chooseTableBC input').on('click', function () {
        self.Loc_Table($(this).val())
        _kieubang = $(this).val();
        self.hideTableReport();
        _pageNumber = 1;
        if ($(this).val() == 1) {
            $('.table_BanHang').show();
            $('#BieuDo').show();
            $(".list_NVBanHang").show();
            $(".list_NV").hide();
            $(".list_NVBanHangNV").hide();
            self.hideradio();
            self.MoiQuanTam('Báo cáo bán hàng theo nhân viên');
            if (self.check_kieubang() == 1)
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
        else if ($(this).val() == 2) {
            $('.table_LoiNhuan').show();
            $("#BieuDo a ").removeClass("box-tab");
            self.check_kieubang('2');
            $(".list_NVBanHang").hide();
            $(".list_NV").show();
            $(".list_NVBanHangNV").hide();
            $('#BieuDo').hide();
            $('#home').removeClass("active");
            $('#info').addClass("active");
            self.hideradio();
            self.MoiQuanTam('Báo cáo lợi nhuận theo nhân viên');
            self.getListReportHH_LoiNhuan();
        }
        else {
            $(".list_NVBanHang").hide();
            $(".list_NV").hide();
            $(".list_NVBanHangNV").show();
            $("#BieuDo a ").removeClass("box-tab");
            self.check_kieubang('2');
            $('#BieuDo').hide();
            $('#home').removeClass("active");
            $('#info').addClass("active");
            $('.table_NhanVien').show();
            $('.rd_HangHoa').show();
            $('.rd_NhomHang').show();
            $('.rd_LoaiHang').show();
            self.MoiQuanTam('Báo cáo danh sách hàng bán theo nhân viên');
            self.getListReportHangBan_NhanVien();
        }
    })

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
            $("#NoteNameNguoiBan").attr("placeholder", "Chọn người bán...");
            for (var i = 0; i < self.searchNguoiban().length; i++) {
                if (i == 0)
                    _tenNguoiBanSeach = self.searchNguoiban()[i].ID;
                else
                    _tenNguoiBanSeach = self.searchNguoiban()[i].ID + "," + _tenNguoiBanSeach;
            }
        }
        // remove check
        $('#selec-all-NguoiBan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        nextPage = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.getListReportHangBan_NhanVien();

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
            $('#NoteNameNguoiBan').removeAttr('placeholder');
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
        nextPage = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.getListReportHangBan_NhanVien();
    }
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
                    self.getListNV_BanHang_BieuDo();
                else
                    self.getListReportHH_BanHang();
            }
            else if (_kieubang == 2)
                self.getListReportHH_LoiNhuan();
            else if (_kieubang == 3)
                self.getListReportHangBan_NhanVien();
            self.ReserPage();
        }
        else if ($(this).val() == 2) {
            //console.log($('.ip_DateReport').val())
            $('.ip_DateReport').removeAttr('disabled');
            // $('.ip_DateReport').removeClass("StartImport");
            $('.ip_TimeReport').attr('disabled', 'false');
            $('.dr_TimeReport').removeAttr('data-toggle');
            //$('.ip_TimeReport').addClass("StartImport");
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
                        self.getListNV_BanHang_BieuDo();
                    else
                        self.getListReportHH_BanHang();
                }
                else if (_kieubang == 2)
                    self.getListReportHH_LoiNhuan();
                else if (_kieubang == 3)
                    self.getListReportHangBan_NhanVien();
                self.ReserPage();
            }
        }
    })
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
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.getListReportHangBan_NhanVien();
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
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.getListReportHangBan_NhanVien();
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
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.getListReportHangBan_NhanVien();
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
                self.getListNV_BanHang_BieuDo();
            else
                self.getListReportHH_BanHang();
        }
        else if (_kieubang == 2)
            self.getListReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.getListReportHangBan_NhanVien();
    })
    // Key Event maHH
    self.SelectMaHH = function () {
        _maHH = $('#txtMaHH').val();
        console.log(_maHH);
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListNV_BanHang_BieuDo();
                else
                    self.getListReportHH_BanHang();
            }
            else if (_kieubang == 2)
                self.getListReportHH_LoiNhuan();
            else if (_kieubang == 3)
                self.getListReportHangBan_NhanVien();
        }
    })

    function getAllNSNhanVien() {
        console.log(_idDonViSeach);
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
            //getDonVi();
            nextPage = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListNV_BanHang_BieuDo();
                else
                    self.getListReportHH_BanHang();
            }
            else if (_kieubang == 2)
                self.getListReportHH_LoiNhuan();
            else if (_kieubang == 3)
                self.getListReportHangBan_NhanVien();
        });

    }
    //getAllNSNhanVien();
    //GetListNhomHangHoa
    function GetAllNhomHH() {
        self.NhomHangHoas([]);
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

        self.getListReportHangBan_NhanVien();
    }
    $('.SelectALLNhomHang').on('click', function () {
        $('.SelectALLNhomHang li').addClass('SelectReport')
        $('.SelectNhomHang li').each(function () {
            $(this).removeClass('SelectReport');
        });
        _ID_NhomHang = null;
        _pageNumber = 1;
        self.getListReportHangBan_NhanVien();
    });
    var _pageNumber = 1;
    var _pageNumberNhapHang = 1;
    var _pageSize = 10;
    var AllPage;
    //$(".PhanQuyen").hide();
    //$(".TongCongBanHang").hide();
    //$(".page").hide();
    self.getListReportHH_BanHang = function () {
        if (self.BCNV_BanHang() == "BCNV_BanHang")
        {
            $(".PhanQuyen").hide();
            
            _maHH = null;
            _laHangHoa = 2;
            _ID_NhomHang = null;
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListReportNV_BanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach, "GET").done(function (data) {
                self.ReportHH_BanHang(data.LstData);
                self.ReportHH_BanHangPrint(data.LstDataPrint);
                self.SumDoanhThu(data._lailo);
                self.SumGiaTriTra(data._tienvon);
                self.SumDoanhThuThuan(data._thanhtien);
                if (self.ReportHH_BanHang().length != 0) {
                    $('.TongCongBanHang').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportHH_BanHang().length)
                }
                else {
                    $('.TongCongBanHang').hide();
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
                LoadHtmlGrid(cacheExcelBHNV, 1, "banhangNV");
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
    //self.getListReportHH_BanHang();
    self.SelectedPageNumberReportHH_BanHang = function () {
        _maHH = null;
        _laHangHoa = 2;
        _ID_NhomHang = null;
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListReportNV_BanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach, "GET").done(function (data) {
            self.ReportHH_BanHang(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportHH_BanHang().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelBHNV, 1, "banhangNV");
        });
    }

    self.getListReportHH_LoiNhuan = function () {
        if (self.BCNV_LoiNhuan() == "BCNV_LoiNhuan")
        {
            $(".PhanQuyen").hide();
            _maHH = null;
            _laHangHoa = 2;
            _ID_NhomHang = null;
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListReportNV_LoiNhuan?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach + "&ID_NguoiDung=" + _IDDoiTuong, "GET").done(function (data) {
                self.ReportHH_LoiNhuan(data.LstData);
                self.ReportHH_LoiNhuanPrint(data.LstDataPrint);
                self.SumTongTienHang(data.TongTienHang);
                self.SumGiamGiaHD(data.GiamGiaHD);
                self.SumDoanhThu_LN(data.DoanhThu);
                self.SumGiaTriTra_LN(data.GiaTriTra);
                self.SumDoanhThuThuan_LN(data.DoanhThuThuan);
                self.SumTongGiaVon(data.TongGiaVon);
                self.SumLoiNhuanGop(data.LoiNhuanGop);
                if (self.ReportHH_LoiNhuan().length != 0) {
                    $(".Report_Empty").hide();
                    $('.TongCongLoiNhuan').show();
                    $(".page").show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportHH_LoiNhuan().length)
                }
                else {
                    $(".Report_Empty").show();
                    $('.TongCongLoiNhuan').hide();
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
                LoadHtmlGrid(cacheExcelLNNV, 2, "loinhuanNV");
            });
        }
        else {
            $(".PhanQuyen").show();
            $(".Report_Empty").hide();
            $(".TongCongLoiNhuan").hide();
            $(".page").hide();
        }
    }
    self.SelectedPageNumberReportHH_LoiNhuan = function () {
        _maHH = null;
        _laHangHoa = 2;
        _ID_NhomHang = null;
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListReportNV_LoiNhuan?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach, "GET").done(function (data) {
            self.ReportHH_LoiNhuan(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportHH_LoiNhuan().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelLNNV, 2, "loinhuanNV");
        });
    }

    self.getListReportHangBan_NhanVien = function () {
        if (self.BCNV_HangBan() == "BCNV_HangBan")
        {
            hidewait('table_h');
            $(".PhanQuyen").hide();
           
            ajaxHelper(ReportUri + "getListReportHangBan_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach, "GET").done(function (data) {
                self.ReportHangBan_NhanVien(data.LstData);
                self.ReportHangBan_NhanVienPrint(data.LstDataPrint);
                self.SumSoLuongHangBan(data._lailo);
                self.SumGiaTriHangHang(data._thanhtien);
                if (self.ReportHangBan_NhanVien().length != 0) {
                    $('.TongCongHangBan').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportHangBan_NhanVien().length)
                }
                else {
                    $('.TongCongHangBan').hide();
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
                LoadHtmlGrid(cacheExcelHBTNV, 3, "hangbantheoNV");
            });
        }
        else
        {
            $(".PhanQuyen").show();
            $(".TongCongHangBan").hide();
            $(".Report_Empty").hide();
            $(".page").hide();
        }
    }
    self.SelectedPageNumberReportHangBan_NhanVien = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListReportHangBan_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach, "GET").done(function (data) {
            self.ReportHangBan_NhanVien(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportHangBan_NhanVien().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelHBTNV, 3, "hangbantheoNV");
        });
    }
    var id_nhanvien_select
    self.ChiTietHangHoa = function (item) {
        self.TenNVPrint(item.TenNhanVien);
        self.HB_SoLuongPrint(item.SoLuong);
        self.HB_GiaTriPrint(item.GiaTri);
        hidewait('table_h');
        _pageNumberNhapHang = 1;
        id_nhanvien_select = item.ID_NhanVien;
        ajaxHelper(ReportUri + "getListReportHangBanChiTiet_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumberNhapHang + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + id_nhanvien_select, "GET").done(function (data) {
            self.ReportHangBanChiTiet_NhanVien(data.LstData);
            self.ReportHangBanChiTiet_NhanVienPrint(data.LstDataPrint);
            if (self.ReportHangBanChiTiet_NhanVien().length != 0) {
                self.RowsStartNhapHang((_pageNumberNhapHang - 1) * _pageSize + 1);
                self.RowsEndNhapHang((_pageNumberNhapHang - 1) * _pageSize + self.ReportHangBanChiTiet_NhanVien().length)
            }
            else {
                self.RowsStartNhapHang('0');
                self.RowsEndNhapHang('0');
            }
            self.SumNumberPageReportNhapHang(data.LstPageNumber);
            AllPageNhapHang = self.SumNumberPageReportNhapHang().length;
            self.selecPageNhapHang();
            self.ReserPageNhapHang();
            self.SumRowsHangHoaNhapHang(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }

    self.SelectedPageNumberReportChiTietHangHoa = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListReportHangBanChiTiet_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumberNhapHang + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + id_nhanvien_select, "GET").done(function (data) {
            self.ReportHangBanChiTiet_NhanVien(data.LstData);
            self.RowsStartNhapHang((_pageNumberNhapHang - 1) * _pageSize + 1);
            self.RowsEndNhapHang((_pageNumberNhapHang - 1) * _pageSize + self.ReportHangBanChiTiet_NhanVien().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.GetClassNhapHang = function (page) {
        return (page.SoTrang === self.currentPageNhapHang()) ? "click" : "";
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
                self.SelectedPageNumberReportHH_BanHang();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportHH_LoiNhuan();
            else if (_kieubang == 3)
                self.SelectedPageNumberReportHangBan_NhanVien();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.ReserPage();
            if (_kieubang == 1)
                self.SelectedPageNumberReportHH_BanHang();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportHH_LoiNhuan();
            else if (_kieubang == 3)
                self.SelectedPageNumberReportHangBan_NhanVien();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportHH_BanHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportHangBan_NhanVien();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportHH_BanHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportHangBan_NhanVien();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportHH_BanHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportHH_LoiNhuan();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportHangBan_NhanVien();
    }

    // phân trang chi tiết nhập hàng
    self.selecPageNhapHang = function () {
        // AllPageNhapHang = self.SumNumberPageReport().length;

        if (AllPageNhapHang > 4) {
            for (var i = 3; i < AllPageNhapHang; i++) {
                self.SumNumberPageReportNhapHang.pop(i + 1);
            }
            self.SumNumberPageReportNhapHang.push({ SoTrang: 4 });
            self.SumNumberPageReportNhapHang.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportNhapHang.pop(i);
            }
            for (var j = 0; j < AllPageNhapHang; j++) {
                self.SumNumberPageReportNhapHang.push({ SoTrang: j + 1 });
            }
        }
        //$('#StartPageNhapHang .fa-step-backward').hide();
        //$('#BackPageNhapHang fa-caret-left').hide();
        //$('#NextPageNhapHang .fa-caret-right').show();
        //$('#EndPageNhapHang .fa-step-forward').show();
        $('#StartPageNhapHang').hide();
        $('#BackPageNhapHang').hide();
        $('#NextPageNhapHang').show();
        $('#EndPageNhapHang').show();
    }
    self.ReserPageNhapHang = function (item) {
        //self.selecPage();
        if (_pageNumberNhapHang > 1 && AllPageNhapHang > 5/* && nextPage < AllPageNhapHang - 1*/) {
            if (_pageNumberNhapHang > 3 && _pageNumberNhapHang < AllPageNhapHang - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportNhapHang.replace(self.SumNumberPageReportNhapHang()[i], { SoTrang: parseInt(_pageNumberNhapHang) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberNhapHang) === parseInt(AllPageNhapHang) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportNhapHang.replace(self.SumNumberPageReportNhapHang()[i], { SoTrang: parseInt(_pageNumberNhapHang) + i - 3 });
                }
            }
            else if (_pageNumberNhapHang == AllPageNhapHang) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportNhapHang.replace(self.SumNumberPageReportNhapHang()[i], { SoTrang: parseInt(_pageNumberNhapHang) + i - 4 });
                }
            }
            else if (_pageNumberNhapHang < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberNhapHang)
                    self.SumNumberPageReportNhapHang.replace(self.SumNumberPageReportNhapHang()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberNhapHang == 1 && AllPageNhapHang > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportNhapHang.replace(self.SumNumberPageReportNhapHang()[i], { SoTrang: parseInt(_pageNumberNhapHang) + i });
            }
        }
        if (_pageNumberNhapHang > 1) {
            $('#StartPageNhapHang').show();
            $('#BackPageNhapHang').show();
        }
        else {
            $('#StartPageNhapHang').hide();
            $('#BackPageNhapHang').hide();
        }
        if (_pageNumberNhapHang == AllPageNhapHang) {
            $('#NextPageNhapHang').hide();
            $('#EndPageNhapHang').hide();
        }
        else {
            $('#NextPageNhapHang').show();
            $('#EndPageNhapHang').show();
        }
        //if (_pageNumberNhapHang > 1) {
        //    $('#StartPageNhapHang .fa-step-backward').show();
        //    $('#BackPageNhapHang .fa-caret-left').show();
        //}
        //else {
        //    $('#StartPageNhapHang .fa-step-backward').hide();
        //    $('#BackPageNhapHang .fa-caret-left').hide();
        //}
        //if (_pageNumberNhapHang == AllPageNhapHang) {
        //    $('#NextPageNhapHang .fa-caret-right').hide();
        //    $('#EndPageNhapHang .fa-step-forward').hide();
        //}
        //else {
        //    $('#NextPageNhapHang .fa-caret-right').show();
        //    $('#EndPageNhapHang .fa-step-forward').show();
        //}
        self.currentPageNhapHang(parseInt(_pageNumberNhapHang));
    }
    self.NextPageNhapHang = function (item) {
        if (_pageNumberNhapHang < AllPageNhapHang) {
            _pageNumberNhapHang = _pageNumberNhapHang + 1;
            self.ReserPageNhapHang();
            self.SelectedPageNumberReportChiTietHangHoa();
        }
    };
    self.BackPageNhapHang = function (item) {
        if (_pageNumberNhapHang > 1) {
            _pageNumberNhapHang = _pageNumberNhapHang - 1;
            self.ReserPageNhapHang();
            self.SelectedPageNumberReportChiTietHangHoa();
        }
    };
    self.EndPageNhapHang = function (item) {
        _pageNumberNhapHang = AllPageNhapHang;
        self.ReserPageNhapHang();
        self.SelectedPageNumberReportChiTietHangHoa();
    };
    self.StartPageNhapHang = function (item) {
        _pageNumberNhapHang = 1;
        self.ReserPageNhapHang();
        self.SelectedPageNumberReportChiTietHangHoa();
    };
    self.gotoNextPageNhapHang = function (item) {
        _pageNumberNhapHang = item.SoTrang;
        self.ReserPageNhapHang();
        self.SelectedPageNumberReportChiTietHangHoa();
    }

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
    //lọc người bán

    self.NoteNameNhanVien = function () {
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
        }
    }
    $('#NoteNameNguoiBan').keypress(function (e) {
        if (e.keyCode == 13 && self.NguoiBans().length > 0) {
            self.SelectedNguoiBan(self.NguoiBans()[0]);
        }
    });
    //load đơn vị
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameDonViSeach, "GET").done(function (data) {
            self.DonVis(data);
            self.searchDonVi(data);
            for (var i = 0; i < self.DonVis().length; i++) {
                if (self.DonVis()[i].ID == _idDonViSeach) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi)
                    self.SelectedDonVi(self.DonVis()[i])
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
            //getDonVi();
        }
        else {
            //getAllHoaDon();
            //_SuKienLoad = null;
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
        //nextPage = 1;
        //if (_kieubang == 1) {
        //    if (self.check_kieubang() == 1)
        //        self.getListNV_BanHang_BieuDo();
        //    else
        //        self.getListReportHH_BanHang();
        //}
        //else if (_kieubang == 2)
        //    self.getListReportHH_LoiNhuan();
        //else if (_kieubang == 3)
        //    self.getListReportHangBan_NhanVien();
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
        //nextPage = 1;
        //if (_kieubang == 1) {
        //    if (self.check_kieubang() == 1)
        //        self.getListNV_BanHang_BieuDo();
        //    else
        //        self.getListReportHH_BanHang();
        //}
        //else if (_kieubang == 2)
        //    self.getListReportHH_LoiNhuan();
        //else if (_kieubang == 3)
        //    self.getListReportHangBan_NhanVien();
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

    //Xuất file excel
    $("#nvname").click(function () {
        $(".nvname").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvname", "nvname", $(this).val(), "loinhuanNV");
    });
    $("#nvtongtien").click(function () {
        $(".nvtongtien").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvtongtien", "nvtongtien", $(this).val(), "loinhuanNV");
    });
    $("#nvgiamgia").click(function () {
        $(".nvgiamgia").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvgiamgia", "nvgiamgia", $(this).val(), "loinhuanNV");
    });
    $("#nvdoanhthu").click(function () {
        $(".nvdoanhthu").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvdoanhthu", "nvdoanhthu", $(this).val(), "loinhuanNV");
    });
    $("#nvgiatritra").click(function () {
        $(".nvgiatritra").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvgiatritra", "nvgiatritra", $(this).val(), "loinhuanNV");
    });
    $("#nvdoanhthuthuan").click(function () {
        $(".nvdoanhthuthuan").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvdoanhthuthuan", "nvdoanhthuthuan", $(this).val(), "loinhuanNV");
    });
    $("#nvgiavon").click(function () {
        $(".nvgiavon").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvgiavon", "nvgiavon", $(this).val(), "loinhuanNV");
    });
    $("#nvloinhuagop").click(function () {
        $(".nvloinhuagop").toggle();
        self.addColum(2, $(this).val());
        addClass(".nvloinhuagop", "nvloinhuagop", $(this).val(), "loinhuanNV");
    });

    $('#nvbhnguoiban').click(function () {
        $(".nvbhnguoiban").toggle();
        self.addColum(1, $(this).val());
        addClass(".nvbhnguoiban", "nvbhnguoiban", $(this).val(), "banhangNV");
    });
    $('#nvbhdoanhthu').click(function () {
        $(".nvbhdoanhthu ").toggle();
        self.addColum(1, $(this).val());
        addClass(".nvbhdoanhthu", "nvbhdoanhthu", $(this).val(), "banhangNV");
    });
    $('#nvbhgiatri').click(function () {
        $(".nvbhgiatri ").toggle();
        self.addColum(1, $(this).val());
        addClass(".nvbhgiatri", "nvbhgiatri", $(this).val(), "banhangNV");
    });
    $('#nvdoanhthuthuant').click(function () {
        $(".nvdoanhthuthuant").toggle();
        self.addColum(1, $(this).val());
        addClass(".nvdoanhthuthuant", "nvdoanhthuthuant", $(this).val(), "banhangNV");
    });

    $('#nvtheonguoiban    ').click(function () {
        $(".nvtheonguoiban     ").toggle();
        self.addColum(3, $(this).val());
        addClass(".nvtheonguoiban", "nvtheonguoiban", $(this).val(), "hangbantheoNV");
    });
    $('#nvtheosl').click(function () {
        $(".nvtheosl").toggle();
        self.addColum(3, $(this).val());
        addClass(".nvtheosl", "nvtheosl", $(this).val(), "hangbantheoNV");
    });
    $('#nvtheogiatri').click(function () {
        $(".nvtheogiatri").toggle();
        self.addColum(3, $(this).val());
        addClass(".nvtheogiatri", "nvtheogiatri", $(this).val(), "hangbantheoNV");
    });

    self.addColum = function (values, item) {
        if (values == 1) {
            if (self.ColumnsExcelBHNV().length < 1) {
                self.ColumnsExcelBHNV.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelBHNV().length; i++) {
                    if (self.ColumnsExcelBHNV()[i] === item) {
                        self.ColumnsExcelBHNV.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelBHNV().length - 1) {
                        self.ColumnsExcelBHNV.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelBHNV.sort();
        }
        else if (values == 2) {
            if (self.ColumnsExcelLNNV().length < 1) {
                self.ColumnsExcelLNNV.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelLNNV().length; i++) {
                    if (self.ColumnsExcelLNNV()[i] === item) {
                        self.ColumnsExcelLNNV.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelLNNV().length - 1) {
                        self.ColumnsExcelLNNV.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelLNNV.sort();
        }
        else {
            if (self.ColumnsExcelHBTNV().length < 1) {
                self.ColumnsExcelHBTNV.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelHBTNV().length; i++) {
                    if (self.ColumnsExcelHBTNV()[i] === item) {
                        self.ColumnsExcelHBTNV.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelHBTNV().length - 1) {
                        self.ColumnsExcelHBTNV.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelHBTNV.sort();
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
                        else
                            self.addColum(3, current[i].Value);
                    }
                }
                if (vals === 1)
                    cacheExcelBHNV = false;
                else if (vals === 2)
                    cacheExcelLNNV = false;
                else
                    cacheExcelHBTNV = false;
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
            ChucNang: "Báo cáo nhân viên",
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
                if (_kieubang == 1 && self.ReportHH_BanHang().length != 0) {
                    if (self.BCNV_BanHang() == "BCNV_BanHang" && self.BCNV_BanHang_XuatFile == "BCNV_BanHang_XuatFile") {
                        for (var i = 0; i < self.ColumnsExcelBHNV().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelBHNV()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelBHNV()[i] + "_" + columnHide;
                            }
                        }
                        _maHH = null;
                        _laHangHoa = 2;
                        _ID_NhomHang = null;
                        var url = ReportUri + "ExportExcelNV_BanHang?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 2 && self.ReportHH_LoiNhuan().length != 0) {
                    if (self.BCNV_LoiNhuan() == "BCNV_LoiNhuan" && self.BCNV_LoiNhuan_XuatFile() == "BCNV_LoiNhuan_XuatFile") {
                        for (var i = 0; i < self.ColumnsExcelLNNV().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelLNNV()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelLNNV()[i] + "_" + columnHide;
                            }
                        }
                        _maHH = null;
                        _laHangHoa = 2;
                        _ID_NhomHang = null;
                        var url = ReportUri + "ExportExcelNV_LoiNhuan?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh() + "&ID_NguoiDung=" + _IDDoiTuong;
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 3 && self.ReportHangBan_NhanVien().length != 0) {
                    if (self.BCNV_HangBan() == "BCNV_HangBan" && self.BCNV_HangBan_XuatFile() == "BCNV_HangBan_XuatFile")
                    {
                        for (var i = 0; i < self.ColumnsExcelHBTNV().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelHBTNV()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelHBTNV()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelHangBan_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
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
            ChucNang: "Báo cáo nhân viên",
            NoiDung: "Xuất báo cáo danh sach hàng bán theo nhân viên",
            NoiDungChiTiet: "Xuất báo cáo danh sach hàng bán theo nhân viên",
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
                var url = ReportUri + "ExportExcelHangBanChiTiet_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + id_nhanvien_select + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
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

    //trinhpv load bieudo
    self.arrHH = ko.observableArray();
    self.arrDT = ko.observableArray();
    self.DoanhThuTT = ko.observableArray();
    self.DonViTinh = ko.observable("Đơn vị tính: hàng đơn vị");
    var _dataDS;
    var _data;
    var nameChar;
    var style;
    self.getListNV_BanHang_BieuDo = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListReportNV_BanHang_BieuDo?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + _tenNguoiBanSeach, "GET").done(function (data) {
            self.DoanhThuTT(data);
            self.arrDT([]);
            self.arrHH([]);
            if (self.DoanhThuTT().length != 0) {
                var _MauSoDVT = 1;
                var _loadHangHoa = "'";
                for (var i = 0; i < self.DoanhThuTT().length; i++) {
                    _loadHangHoa = self.DoanhThuTT()[i].Columnss;
                   //_data = parseFloat(self.DoanhThuTT()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                    self.arrDT.push(self.DoanhThuTT()[i].Rowsn);
                    self.arrHH.push(_loadHangHoa);
                }
                style = 'Top 10 nhân viên bán nhiều nhất (đã trừ trả hàng)';
                nameChar = "Doanh thu thuần";
            }
            else {
                style = "Báo cáo không có dữ liệu."
                self.DonViTinh([]);
            }
            self.loadBieuDo();
            $("div[id ^= 'wait']").text("");
        });
    }
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
            // hiển thị giá trị lên đầu cột
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: false
                    }
                }
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