var ViewModal = function () {
    var self = this;
    self.LoaiTrangThai_AD = ko.observable(1);
    self.LoaiTrangThai_KAD = ko.observable(1);
    self.LoaiTrangThai_Xoa = ko.observable(0);
    var TT_ApDung = 1;
    var TT_KhongApDung = 1;
    var TT_CaLamViec = 1;
    var dt1 = new Date();
    var _timeStart = moment(new Date(dt1.getFullYear(), dt1.getMonth(), 1)).format('YYYY-MM-DD');
    var _timeEnd = moment(new Date(dt1.getFullYear(), dt1.getMonth() + 1, 1)).format('YYYY-MM-DD');
    var dtBC = new Date(_timeEnd);
    var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
    self.TodayBC = ko.observable('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
    self.NhanViens = ko.observableArray();
    self.ID_CaLamViec = ko.observable(null);
    self.MaCa = ko.observable(null)
    self.TenCa = ko.observable(null)
    self.GioVao = ko.observable(null)
    self.GioRa = ko.observable(null)
    self.TongGioCong = ko.observable(null)
    self.NghiGiuaCaTu = ko.observable(null)
    self.NghiGiuaCaDen = ko.observable(null)
    self.GioOTBanNgayTu = ko.observable(null)
    self.GioOTBanNgayDen = ko.observable(null)
    self.GioOTBanDemTu = ko.observable(null)
    self.GioOTBanDemDen = ko.observable(null)
    self.SoPhutDiMuon = ko.observable(null)
    self.SoPhutVeSom = ko.observable(null)
    self.GioVaoTu = ko.observable(null)
    self.GioVaoDen = ko.observable(null)
    self.GioRaTu = ko.observable(null)
    self.GioRaDen = ko.observable(null)
    self.TinhOTBanNgayTu = ko.observable(null)
    self.TinhOTBanNgayDen = ko.observable(null)
    self.TinhOTBanDemTu = ko.observable(null)
    self.TinhOTBanDemDen = ko.observable(null)
    self.LaCaDem = ko.observable(0)
    self.CachLayGioCong = ko.observable(1)
    self.SoGioOTToiThieu = ko.observable(null)
    self.GhiChuCaLamViec = ko.observable(null)
    self.GhiChuTinhGio = ko.observable(null)
    self.TrangThai = ko.observable(1)
    self.NguoiTao = ko.observable(null)
    self.ContinueImport = ko.observable(false);
    self.NhanSu_CaLamViec = ko.observableArray();
    var txt_seach = null;
    var AllPage;
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.pageSize = ko.observable(10);
    var _pageNumber = 1;
    var _pageSize = 10;
    self.pageSizes = [10, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.pageNumber_CLV = ko.observable(1);
    var _IDchinhanh = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.selectNameNV = ko.observable();
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var NhanSuUri = '/api/DanhMuc/NS_NhanSuAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    // trạng thái
    var _trangthaiAD = 1;
    var _trangthaiKAD = 2;
    var _trangthaiXoa = 0;
    function LoadingForm(IsShow) {
        $('.tab-show .tab-pane').each(function () {
            if ($(this).hasClass('active')) {
                var top = $(this).find('.table-reponsive').height() / 2;
                var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                $(this).find('.table-reponsive').gridLoader({ show: IsShow, style: style });
            }
        });
    }
    $('.choose_TrangThai input').on('click', function () {
        if ($(this).val() == 1) {
            if (_trangthaiAD == 1) {
                _trangthaiAD = 0;
            }
            else {
                _trangthaiAD = 1
            }
        }
        else if ($(this).val() == 2) {
            if (_trangthaiKAD == 2) {
                _trangthaiKAD = 0;
            }
            else {
                _trangthaiKAD = 2;
            }
        }
        else if ($(this).val() == 3) {
            if (_trangthaiXoa == 3) {
                _trangthaiXoa = 0;
            }
            else {
                _trangthaiXoa = 3;
            }
        }
        console.log(_trangthaiAD, _trangthaiKAD, _trangthaiXoa);
        _pageNumber = 1;
        self.LoadReport();
    })

    // check thời gian
    $('.choose_TimeReport input').on('click', function () {
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
                self.TodayBC('Ngày bán: Toàn thời gian');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
                    self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    })
    // chọn thời gian
    $('.choose_txtTime li').on('click', function () {
        //self.TodayBC($(this).text())
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày bán: Toàn thời gian');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
            self.TodayBC('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        _pageNumber = 1;
        self.LoadReport();
    });
    //$('#datetimepicker_mask').datetimepicker({
    //    timepicker: true,
    //    mask: true, // '9999/19/39 29:59' - digit is the maximum possible for a cell
    //    format: 'd/m/Y H:i',
    //    value: new Date()
    //});
    $("#datetimepicker_mask").attr("disabled", "disabled");
    $("#txtNguoiTao").attr("disabled", "disabled");
    $("#txtTongGioCong").attr("disabled", "disabale");
    // load Nhân viên
    self.nameNhanVien = ko.observable();
    self.getNameNhanVien = function () {
        ajaxHelper('/api/DanhMuc/TQ_DoanhThuAPI/' + "getNameNhanVien?ID_NhanVien=" + _id_NhanVien, "GET").done(function (data) {
            self.nameNhanVien(data);
        });
    }
    self.getNameNhanVien();
    var _tennhanvien_seach = null;
    var timer1 = null;
    self.NoteNhanVien = function () {
        clearTimeout(timer1);
        _tennhanvien_seach = $('#txtAuto').val();
        console.log(_tennhanvien_seach);
        timer1 = setTimeout(getAllNSNhanVien(), 500);
    }
    function getAllNSNhanVien() {
        ajaxHelper(NhanVienUri + "getListNhanVien_DonVi?ID_ChiNhanh=" + _IDchinhanh + "&nameNV=" + _tennhanvien_seach, 'GET').done(function (data) {
            self.NhanViens(data);
        });
    }
    //getAllNSNhanVien();
    self.selectIDNV = ko.observable();
    self.SelectNhanVien = function (item) {
        self.selectNameNV(item.TenNhanVien);
        self.selectIDNV(item.ID);
        console.log(self.selectIDNV());
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
    // cách lấy công
    var _cachLayCong = 1;
    $(".mySelectCachLayCong").on('change', function (item) {
        _cachLayCong = $(this).val()
        console.log(_cachLayCong);
    });
    var _TrangThaiCa = 1;
    $(".mySelectTrangThai").on('change', function (item) {
        _TrangThaiCa = $(this).val()
        console.log(_TrangThaiCa);
    });
    var _LoaiCa = 0;
    self.check_LoaiCa = ko.observable();
    $('.choose_LoaiCa input').on("click", function () {
        var genderMaleCheckbox = $('#gender_male_checkbox').prop('checked');
        if (genderMaleCheckbox == true) {
            _LoaiCa = 1;
            self.check_LoaiCa(_LoaiCa);
        }
        else {
            _LoaiCa = 0
            self.check_LoaiCa(_LoaiCa);
        }
        self.edit_GioCong();
        console.log(_LoaiCa);
    });
    
    self.SumRowsCaLamViec = ko.observable();
    self.LoadReport = function () {
        LoadingForm(true);
        ajaxHelper(NhanSuUri + "getListNhanSu_CaLamViec?trangthaiAD=" + _trangthaiAD + "&trangthaiKAD=" + _trangthaiKAD + "&trangthaiXoa=" + _trangthaiXoa + "&paperSize=" + _pageSize + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&MaCa=" + txt_seach, "GET").done(function (data) {
            self.NhanSu_CaLamViec(data.LstData);
            AllPage = data.numberPage;
            self.selecPage();
            self.ReserPage();
            self.SumRowsCaLamViec(data.Rowcount);
            LoadingForm(false);
            $('.line-right').height(0).css("margin-top", "0px");
            if (!$('.table-reponsive').hasClass('tablescroll'))
            { $('.table-reponsive').addClass('tablescroll'); }
        });
    };
    self.LoadReport();
    self.ResetCurrentPage = function () {
        $("#iconSort").remove();
        _pageSize = self.pageSize();
        _pageNumber = 1;
        self.pageNumber_CLV(_pageNumber);
        //self.ReserPage();
        self.LoadReport();
    };
    self.NhanSu_CaLamViec_Page = ko.computed(function (x) {
        var first = (self.pageNumber_CLV() - 1) * self.pageSize();
        if (self.NhanSu_CaLamViec() !== null) {
            if (self.NhanSu_CaLamViec().length != 0) {
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_CLV() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_CLV() - 1) * self.pageSize() + self.NhanSu_CaLamViec().slice(first, first + self.pageSize()).length)
            }
            else {
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.NhanSu_CaLamViec().slice(first, first + _pageSize);
        }
        return null;
    })
    // tính tổng giờ công
    self.TongGioCong = ko.observable();
    $('#txtGioVao').timepicker().on('change', function () {
        var time = $("#txtGioVao").timepicker('getTime');
        var array_time = time.split(":");
        self.GioVao(new Date(2000, 01, 01, array_time[0], array_time[1]))
        if ($('#txtGioRa').val() != "")
            self.edit_GioCong();
    });
    $('#txtGioRa').timepicker().on('change', function () {
        var time = $("#txtGioRa").timepicker('getTime');
        var array_time = time.split(":");
        self.GioRa(new Date(2000, 01, 01, array_time[0], array_time[1]))
        if ($('#txtGioVao').val() != "")
            self.edit_GioCong();
    });
    $('#txtNghiGiuaCaTu').timepicker().on('change', function () {
        var time = $("#txtNghiGiuaCaTu").timepicker('getTime');
        var array_time = time.split(":");
        self.NghiGiuaCaTu(new Date(2000, 01, 01, array_time[0], array_time[1]))
        if ($('#txtNghiGiuaCaDen').val() != "")
            self.edit_GioCong();
    });
    $('#txtNghiGiuaCaDen').timepicker().on('change', function () {
        var time = $("#txtNghiGiuaCaDen").timepicker('getTime');
        var array_time = time.split(":");
        self.NghiGiuaCaDen(new Date(2000, 01, 01, array_time[0], array_time[1]))
        if ($('#txtNghiGiuaCaTu').val() != "")
            self.edit_GioCong();
    });
    $('#txtLamThemNgayTu').timepicker().on('change', function () {
        var time = $("#txtLamThemNgayTu").timepicker('getTime');
        var array_time = time.split(":");
        self.GioOTBanNgayTu(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtLamThemNgayDen').timepicker().on('change', function () {
        var time = $("#txtLamThemNgayDen").timepicker('getTime');
        var array_time = time.split(":");
        self.GioOTBanNgayDen(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtLamThemDemTu').timepicker().on('change', function () {
        var time = $("#txtLamThemDemTu").timepicker('getTime');
        var array_time = time.split(":");
        self.GioOTBanDemTu(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtLamThemDemDen').timepicker().on('change', function () {
        var time = $("#txtLamThemDemDen").timepicker('getTime');
        var array_time = time.split(":");
        self.GioOTBanDemDen(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtTinhVaoTu').timepicker().on('change', function () {
        var time = $("#txtTinhVaoTu").timepicker('getTime');
        var array_time = time.split(":");
        self.GioVaoTu(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtTinhVaoDen').timepicker().on('change', function () {
        var time = $("#txtTinhVaoDen").timepicker('getTime');
        var array_time = time.split(":");
        self.GioVaoDen(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtTinhRaTu').timepicker().on('change', function () {
        var time = $("#txtTinhRaTu").timepicker('getTime');
        var array_time = time.split(":");
        self.GioRaTu(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtTinhRaDen').timepicker().on('change', function () {
        var time = $("#txtTinhRaDen").timepicker('getTime');
        var array_time = time.split(":");
        self.GioRaDen(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });

    $('#txtTinhThemNgayTu').timepicker().on('change', function () {
        var time = $("#txtTinhThemNgayTu").timepicker('getTime');
        var array_time = time.split(":");
        self.TinhOTBanNgayTu(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtTinhThemNgayDen').timepicker().on('change', function () {
        var time = $("#txtTinhThemNgayDen").timepicker('getTime');
        var array_time = time.split(":");
        self.TinhOTBanNgayDen(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtTinhThemDemTu').timepicker().on('change', function () {
        var time = $("#txtTinhThemDemTu").timepicker('getTime');
        var array_time = time.split(":");
        self.TinhOTBanDemTu(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    $('#txtTinhThemDemDen').timepicker().on('change', function () {
        var time = $("#txtTinhThemDemDen").timepicker('getTime');
        var array_time = time.split(":");
        self.TinhOTBanDemDen(new Date(2000, 01, 01, array_time[0], array_time[1]))
    });
    self.edit_GioCong = function () {
        self.TongGioCong([]);
        var time_GioVao = $("#txtGioVao").timepicker('getTime');
        var array_GioVao = time_GioVao.split(":");
        var time_GioRa = $("#txtGioRa").timepicker('getTime');
        var array_GioRa = time_GioRa.split(":");
        var time_NghiGiuaCaTu = $("#txtNghiGiuaCaTu").timepicker('getTime');
        var array_NghiGiuaCaTu = time_NghiGiuaCaTu.split(":");
        var time_NghiGiuaCaDen = $("#txtNghiGiuaCaDen").timepicker('getTime');
        var array_NghiGiuaCaDen = time_NghiGiuaCaDen.split(":");
        if (_LoaiCa == 0 || time_GioRa > time_GioVao)
        {
            var time1 = ((array_GioRa[0] - array_GioVao[0]) * 60) + parseInt(array_GioRa[1]) - parseInt(array_GioVao[1]);
            var time2 = 0;
            if ($("#txtNghiGiuaCaDen").val() != '' && $("#txtNghiGiuaCaTu").val() != '') {
                var time2 = ((array_NghiGiuaCaDen[0] - array_NghiGiuaCaTu[0]) * 60) + parseInt(array_NghiGiuaCaDen[1]) - parseInt(array_NghiGiuaCaTu[1]);
            }
            var time = (time1 - time2) / 60;
            self.TongGioCong(time);
        }
        else if (time_GioRa < time_GioVao)
        {
            var time_Truoc = ((24 - array_GioVao[0]) * 60) - parseInt(array_GioVao[1]);
            var time_Sau = (array_GioRa[0] * 60) + parseInt(array_GioRa[1]);
            var time1 = time_Truoc + time_Sau;
            var time2 = 0;
            if (time_NghiGiuaCaDen < time_NghiGiuaCaTu)
            {
                if ($("#txtNghiGiuaCaDen").val() != '' && $("#txtNghiGiuaCaTu").val() != '') {
                    var time_NghiTruoc = ((24 - array_NghiGiuaCaTu[0]) * 60) - parseInt(array_NghiGiuaCaTu[1]);
                    var time_NghiSau = (array_NghiGiuaCaDen[0] * 60) + parseInt(array_NghiGiuaCaDen[1]);
                    time2 = time_NghiTruoc + time_NghiSau;
                }
            }
            else
            {
                if ($("#txtNghiGiuaCaDen").val() != '' && $("#txtNghiGiuaCaTu").val() != '') {
                    var time2 = ((array_NghiGiuaCaDen[0] - array_NghiGiuaCaTu[0]) * 60) + parseInt(array_NghiGiuaCaDen[1]) - parseInt(array_NghiGiuaCaTu[1]);
                }
            }
            var time = (time1 - time2) / 60;
            self.TongGioCong(time);
        }
        console.log(self.TongGioCong());
    };
    self.Select_Text = function () {
        txt_seach = $('#txt_search').val();
    }
    $('#txt_search').keypress(function (e) {
        if (e.keyCode == 13) {
            self.LoadReport();
        }
    })
    // import file 
    self.SelectModal_import = function () {
        $("#ModalImport").modal("show");
    }
    self.fileNameExcel = ko.observable("Lựa chọn file Excel...");
    self.visibleImport = ko.observable(true);
    self.notvisibleImport = ko.computed(function () {
        return !self.visibleImport();

    });
    self.refreshFileSelect = function () {
        self.visibleImport(true);
        self.fileNameExcel("Lựa chọn file Excel...");
        $(".filterFileSelect").hide();
        $(".btnImportExcel").hide();
        document.getElementById('imageUploadFormKH').value = "";
    }
    self.ShowandHide = function () {
        self.insertArticleNews();
    }
    self.loiExcel = ko.observableArray();
    $(".BangBaoLoi").hide();
    self.DownloadFileTeamplateXLS = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "CaLamViec_ChamCong/FileImport_ThongTinCaLamViec.xls";
        window.open(url)
    }
    self.DownloadFileTeamplateXLSX = function () {
        var url = DMHangHoaUri + "Download_TeamplateImport?fileSave=" + "CaLamViec_ChamCong/FileImport_ThongTinCaLamViec.xlsx";
        window.open(url)
    }
    $(".filterFileSelect").hide();
    $(".btnImportExcel").hide();
    self.SelectedFileImport = function (vm, evt) {
        self.fileNameExcel(evt.target.files[0].name)
        $(".filterFileSelect").show();
        $(".btnImportExcel").show();
        $(".NoteImport").show();
        $(".BangBaoLoi").hide();
        self.visibleImport(false);
    }
    //check ignore error
    $('.startImport').attr('disabled', 'false');
    $('.startImport').removeClass("btn-green");
    $('.startImport').addClass("StartImport");
    $('.choseContinue input').on('click', function () {

        if ($(this).val() == 0) {
            $(this).val(1);
            $('.startImport').removeAttr('disabled');
            $('.startImport').addClass("btn-green");
            $('.startImport').removeClass("StartImport");
        }
        else {
            $(this).val(0);
            $('.startImport').attr('disabled', 'false');
            $('.startImport').removeClass("btn-green");
            $('.startImport').addClass("StartImport");
        }
    });
    self.insertArticleNews = function () {
        LoadingForm(true);
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
        }
        $.ajax({
            type: "POST",
            url: NhanSuUri + "ImportExcel_ThongTinCaLamViec?NguoiTao=" + self.nameNhanVien(),
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                self.loiExcel(item);
                if (self.loiExcel() != null) {
                    self.visibleImport(true);
                    $(".BangBaoLoi").show();
                    $(".NoteImport").hide();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                }
                else {
                    ShowMessage_Success('Import thông tin ca làm việc thành công');
                    Insert_NhatKyThaoTac(null, 1, 5);
                    document.getElementById('imageUploadFormKH').value = "";
                    self.visibleImport(true);
                    $(".NoteImport").show();
                    $(".filterFileSelect").hide();
                    $(".btnImportExcel").hide();
                    $(".BangBaoLoi").hide();
                    $("#ModalImport").modal("hide");
                    self.LoadReport();
                }
                LoadingForm(false);
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                },
                406: function (item) {
                    bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + item.responseJSON.Message, "danger")
                    LoadingForm(false);
                },
                500: function (item) {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Import thông tin ca làm việc thất bại: " + item.responseJSON.Message, "danger");
                    LoadingForm(false);
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
            },
        });
    }
    self.addRownError = ko.observableArray();
    self.DoneWithError = function () {
        var rownError = null;
        for (var i = 0; i < self.loiExcel().length; i++) {
            if (self.addRownError().length < 1) {
                self.addRownError.push(self.loiExcel()[i].rowError);
            }
            else {
                for (var j = 0; j < self.addRownError().length; j++) {
                    if (self.addRownError()[j] === self.loiExcel()[i].rowError) {
                        break;
                    }
                    if (j == self.addRownError().length - 1) {
                        self.addRownError.push(self.loiExcel()[i].rowError);
                        break;
                    }
                }
            }
        }
        // self.addRownError.sort();
        self.addRownError = self.addRownError.sort(function (a, b) {
            var x = a, y = b;
            return x > y ? 1 : x < y ? -1 : 0;
        })
        //console.log(self.addRownError());
        for (var i = 0; i < self.addRownError().length; i++) {
            if (i == 0)
                rownError = self.addRownError()[i];
            else
                rownError = rownError + "_" + self.addRownError()[i];
        }
        console.log(rownError);
        LoadingForm(true);
        var formData = new FormData();
        var totalFiles = document.getElementById("imageUploadFormKH").files.length;
        for (var i = 0; i < totalFiles; i++) {
            var file = document.getElementById("imageUploadFormKH").files[i];
            formData.append("imageUploadFormKH", file);
        }
        $.ajax({
            type: "POST",
            url: NhanSuUri + "ImportThongTinCaLamViec_WithError?RownError=" + rownError + "&NguoiTao=" + self.nameNhanVien(),
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (item) {
                ShowMessage_Success("Import thông tin ca làm việc thành công");
                Insert_NhatKyThaoTac(null, 1, 5);
                document.getElementById('imageUploadFormKH').value = "";
                $(".NoteImport").show();
                $(".filterFileSelect").hide();
                $(".btnImportExcel").hide();
                $(".BangBaoLoi").hide();
                $("#ModalImport").modal("hide");
                self.LoadReport();
                LoadingForm(false);
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                },
                500: function () {
                    ShowMessage_Danger("Import thông tin ca làm việc thất bại");
                    LoadingForm(false);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                LoadingForm(false);
            },
        });

    }
    function Insert_NhatKyThaoTac(objUsing, chucNang = 1, loaiNhatKy = 1) {
        // chuc nang (1.DoiTuong, 2.NhomDoiTuong, 3.PhieuThu, 4.Export, 5.Import)
        var tenChucNang = 'Danh mục ca làm việc';
        var noiDung = '';
        var noiDungChiTiet = '';
        var txtFirst = 'Import';
        var tenChucNangLowercase = 'ca làm việc';
        noiDung = txtFirst.concat('danh sách ', tenChucNangLowercase);
        noiDungChiTiet = noiDung.concat('<br /> Nhân viên thực hiện: ', $('#txtTenTaiKhoan').val().trim())
        // insert HT_NhatKySuDung
        var objNhatKy = {
            ID_NhanVien: $('.idnhanvien').text(),
            ID_DonVi: $('#hd_IDdDonVi').val(),
            ChucNang: tenChucNang,
            LoaiNhatKy: loaiNhatKy,
            NoiDung: noiDung,
            NoiDungChiTiet: noiDungChiTiet,
        };

        var myDataNK = {};
        myDataNK.objDiary = objNhatKy;
        $.ajax({
            url: DiaryUri + "post_NhatKySuDung",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myDataNK,
            success: function (x) {
            },
        });
    }
    //Thêm mới ca làm việc
    self.addCaLamViec = function () {
        self.ID_CaLamViec(null);
        self.MaCa(null);
        self.TenCa(null);
        $(".mySelectTrangThai").val(1);
        self.GioVao(null);
        self.GioRa(null);
        $('#datetimepicker_mask').datetimepicker({
            timepicker: true,
            mask: true,
            format: 'd/m/Y H:i',
            value: new Date()
        });
        self.NguoiTao(self.nameNhanVien());
        self.NghiGiuaCaTu(null);
        self.NghiGiuaCaDen(null);
        self.GioOTBanNgayTu(null);
        self.GioOTBanNgayDen(null);
        self.GioOTBanDemTu(null);
        self.GioOTBanDemDen(null);
        $(".mySelectCachLayCong").val(1);
        self.TongGioCong(null);
        self.LaCaDem(0);
        _LoaiCa = 0;
        self.GhiChuCaLamViec(null);

        self.SoPhutDiMuon(null);
        self.SoPhutVeSom(null);
        self.GioVaoTu(null);
        self.GioVaoDen(null);
        self.GioRaTu(null);
        self.GioRaDen(null);
        self.TinhOTBanNgayTu(null);
        self.TinhOTBanNgayDen(null);
        self.TinhOTBanDemTu(null);
        self.TinhOTBanDemDen(null);
        self.SoGioOTToiThieu(0);
        self.GhiChuTinhGio(null);
        $('#txtMaCa').attr('disabled', null);
        $('#tableThongTinCa').addClass('active');
        $('#tableThietLapGio').removeClass('active');
        $('#thongtinca').addClass('active');
        $('#thietlaptinhgio').removeClass('active');
        $('#txtGioToiThieu').val('');
        $('#modalthemmoicalamviec').modal('show');
    }
    //sửa ca làm việc
    self.editCaLamViec = function (item) {
        self.ID_CaLamViec(item.ID);
        self.MaCa(item.MaCa);
        self.TenCa(item.TenCa);
        $(".mySelectTrangThai").val(item.TrangThai);
        self.GioVao(item.GioVao);
        self.GioRa(item.GioRa);
        $('#datetimepicker_mask').datetimepicker({
            timepicker: true,
            mask: true, 
            format: 'd/m/Y H:i',
            value: moment(item.NgayTao).format('DD/MM/YYYY HH:mm')
        });
        self.NguoiTao(item.NguoiTao);
        self.NghiGiuaCaTu(item.NghiGiuaCaTu);
        self.NghiGiuaCaDen(item.NghiGiuaCaDen);
        self.GioOTBanNgayTu(item.GioOTBanNgayTu);
        self.GioOTBanNgayDen(item.GioOTBanNgayDen);
        self.GioOTBanDemTu(item.GioOTBanDemTu);
        self.GioOTBanDemDen(item.GioOTBanDemDen);
        $(".mySelectCachLayCong").val(item.CachLayGioCong);
        self.TongGioCong(item.TongGioCong);
        self.LaCaDem(item.LaCaDem);
        _LoaiCa = item.LaCaDem;
        self.GhiChuCaLamViec(item.GhiChuCaLamViec);

        self.SoPhutDiMuon(item.SoPhutDiMuon);
        self.SoPhutVeSom(item.SoPhutVeSom);
        self.GioVaoTu(item.GioVaoTu);
        self.GioVaoDen(item.GioVaoDen);
        self.GioRaTu(item.GioRaTu);
        self.GioRaDen(item.GioRaDen);
        self.TinhOTBanNgayTu(item.TinhOTBanNgayTu);
        self.TinhOTBanNgayDen(item.TinhOTBanNgayDen);
        self.TinhOTBanDemTu(item.TinhOTBanDemTu);
        self.TinhOTBanDemDen(item.TinhOTBanDemDen);
        self.SoGioOTToiThieu(parseFloat(item.SoGioOTToiThieu));
        self.GhiChuTinhGio(item.GhiChuTinhGio);
        $('#txtMaCa').attr('disabled', 'disabled');
        $('#tableThongTinCa').addClass('active');
        $('#tableThietLapGio').removeClass('active');
        $('#thongtinca').addClass('active');
        $('#thietlaptinhgio').removeClass('active');
        $('#modalthemmoicalamviec').modal('show');
    }
    // Xóa ca làm việc
    self.ShowDelete_CaLamViec = function (item)
    {
        self.MaCa(item.MaCa);
        self.ID_CaLamViec(item.ID);
        $('#modalpopup_delete').modal('show');
    }
    self.deleteCaLamViec = function ()
    {
        callAjaxDelete();
    }
    // Lưu ca làm việc
    function isValid(str) {
        return !/[~`!@#$%\^&*()+=\-\[\]\\';,/{}|\\":<>\?]/g.test(str);
    };
    self.add_CaLamViec = function () {
        var date = new Date();
        document.getElementById("Save_CaLamViec").disabled = true;
        document.getElementById("Save_CaLamViec").lastChild.data = " Đang lưu";
        var objCalamViec = {
            MaCa: $('#txtMaCa').val(),
            TenCa: $('#txtTenCa').val(),
            GioVao: moment(self.GioVao()).format('MM/DD/YYYY HH:mm'),
            GioRa: moment(self.GioRa()).format('MM/DD/YYYY HH:mm'),
            TongGioCong: $('#txtTongGioCong').val(),
            NghiGiuaCaTu: self.NghiGiuaCaTu() != null ? moment(self.NghiGiuaCaTu()).format('MM/DD/YYYY HH:mm') : null,
            NghiGiuaCaDen: self.NghiGiuaCaDen() != null ? moment(self.NghiGiuaCaDen()).format('MM/DD/YYYY HH:mm') : null,
            GioOTBanNgayTu: self.GioOTBanNgayTu() != null ? moment(self.GioOTBanNgayTu()).format('MM/DD/YYYY HH:mm') : null,
            GioOTBanNgayDen: self.GioOTBanNgayDen() != null ? moment(self.GioOTBanNgayDen()).format('MM/DD/YYYY HH:mm') : null,
            GioOTBanDemTu: self.GioOTBanDemTu() != null ? moment(self.GioOTBanDemTu()).format('MM/DD/YYYY HH:mm') : null,
            GioOTBanDemDen: self.GioOTBanDemDen() != null ? moment(self.GioOTBanDemDen()).format('MM/DD/YYYY HH:mm') : null,
            SoPhutDiMuon: $('#txtDiMuon').val(),
            SoPhutVeSom: $('#txtVeSom').val(),
            GioVaoTu: self.GioVaoTu() != null ? moment(self.GioVaoTu()).format('MM/DD/YYYY HH:mm') : null,
            GioVaoDen: self.GioVaoDen() != null ? moment(self.GioVaoDen()).format('MM/DD/YYYY HH:mm') : null,
            GioRaTu: self.GioRaTu() != null ? moment(self.GioRaTu()).format('MM/DD/YYYY HH:mm') : null,
            GioRaDen: self.GioRaDen() != null ? moment(self.GioRaDen()).format('MM/DD/YYYY HH:mm') : null,
            TinhOTBanNgayTu: self.TinhOTBanNgayTu() != null ? moment(self.TinhOTBanNgayTu()).format('MM/DD/YYYY HH:mm') : null,
            TinhOTBanNgayDen: self.TinhOTBanNgayDen() != null ? moment(self.TinhOTBanNgayDen()).format('MM/DD/YYYY HH:mm') : null,
            TinhOTBanDemTu: self.TinhOTBanDemTu() != null ? moment(self.TinhOTBanDemTu()).format('MM/DD/YYYY HH:mm') : null,
            TinhOTBanDemDen: self.TinhOTBanDemDen() != null ? moment(self.TinhOTBanDemDen()).format('MM/DD/YYYY HH:mm') : null,
            LaCaDem: _LoaiCa,
            CachLayGioCong: _cachLayCong,
            SoGioOTToiThieu: $('#txtGioToiThieu').val() != ""? $('#txtGioToiThieu').val(): 0,
            GhiChuCaLamViec: $('#txtGhiChuCaLamViec').val(),
            GhiChuTinhGio: $('#txtGhiChuTinhGio').val(),
            TrangThai: _TrangThaiCa,
            NguoiTao: self.NguoiTao(),
            NguoiSua: self.nameNhanVien(),
        };
        console.log(objCalamViec);
        if (!isValid($('#txtMaCa').val())) {
            ShowMessage_Danger("Mã ca làm việc không được nhập kí tự đặc biệt");
            document.getElementById("Save_CaLamViec").disabled = false;
            document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
            return false;
        }
        else if (objCalamViec.TenCa == "" || objCalamViec.TenCa == null) {
            ShowMessage_Danger("Tên ca làm việc không được để trống");
            document.getElementById("Save_CaLamViec").disabled = false;
            document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
            return false;
        }
        else if (objCalamViec.GioVao == "" || objCalamViec.GioVao == null) {
            ShowMessage_Danger("Giờ vào ca chưa được thiết lập");
            document.getElementById("Save_CaLamViec").disabled = false;
            document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
            return false;
        }
        else if (objCalamViec.GioRa == "" || objCalamViec.GioRa == null) {
            ShowMessage_Danger("Giờ ra ca chưa được thiết lập");
            document.getElementById("Save_CaLamViec").disabled = false;
            document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
            return false;
        }
        else if (objCalamViec.GioRa <= objCalamViec.GioVao && objCalamViec.LaCaDem == 0) {
            ShowMessage_Danger("Giờ ra phải lớn hơn giờ vào");
            document.getElementById("Save_CaLamViec").disabled = false;
            document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
            return false;
        }
        else if (objCalamViec.NghiGiuaCaDen != null && objCalamViec.NghiGiuaCaTu != null && (objCalamViec.NghiGiuaCaDen <= objCalamViec.NghiGiuaCaTu) && objCalamViec.LaCaDem == 0) {
            ShowMessage_Danger("Thiết lập thời gian nghỉ giữa ca sai");
            document.getElementById("Save_CaLamViec").disabled = false;
            document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
            return false;
        }
        else if (objCalamViec.GioOTBanNgayTu != null && objCalamViec.GioOTBanNgayDen != null && (objCalamViec.GioOTBanNgayDen <= objCalamViec.GioOTBanNgayTu) && objCalamViec.LaCaDem == 0) {
            ShowMessage_Danger("Thiết lập giờ làm thêm ban ngày sai");
            document.getElementById("Save_CaLamViec").disabled = false;
            document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
            return false;
        }
        //else if (objCalamViec.GioOTBanDemTu != null && objCalamViec.GioOTBanDemDen != null && (objCalamViec.GioOTBanDemDen <= objCalamViec.GioOTBanDemTu) && objCalamViec.LaCaDem == 0) {
        //    ShowMessage_Danger("Thiết lập giờ làm thêm ban đêm sai");
        //    document.getElementById("Save_CaLamViec").disabled = false;
        //    document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";

        //    return false;
        //}

        //else if (objCalamViec.GioVaoTu != null && objCalamViec.GioVaoDen != null && (objCalamViec.GioVaoDen <= objCalamViec.GioVaoTu)) {
        //    ShowMessage_Danger("Thiết lập khoảng thời gian tính giờ vào sai");
        //    document.getElementById("Save_CaLamViec").disabled = false;
        //    document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
        //    return false;
        //}
        //else if (objCalamViec.GioRaTu != null && objCalamViec.GioRaDen != null && (objCalamViec.GioRaDen <= objCalamViec.GioRaTu)) {
        //    ShowMessage_Danger("Thiết lập khoảng thời gian tính giờ ra sai");
        //    document.getElementById("Save_CaLamViec").disabled = false;
        //    document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
        //    return false;
        //}
        else {
            var myData = {};
            myData.objCaLamViec = objCalamViec;
            if (self.ID_CaLamViec() != null)
                callAjaxUpdate(myData);
            else
                callAjaxAdd(myData);
        }
    }
    function callAjaxAdd(myData) {
        $.ajax({
            data: myData,
            url: NhanSuUri + "PostNS_CaLamViec?ID_NhanVien=" + _id_NhanVien + "&ID_DonVi=" + _IDchinhanh,
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                //localStorage.removeItem('lc_SaoChep');
                ShowMessage_Success('Tạo mới ca làm việc thành công');
                //bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Tạo mới ca làm việc thành công", "success");
                document.getElementById("Save_CaLamViec").disabled = false;
                document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
                $('#modalthemmoicalamviec').modal('hide');
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                },
                500: function (item) {
                    if (item.responseJSON == "Mã ca làm việc đã tồn tại trong cơ sở dữ liệu")
                        ShowMessage_Danger(item.responseJSON);
                    else
                        ShowMessage_Danger("Tạo ca làm việc không thành công");
                }
            },
            complete: function (item) {
                document.getElementById("Save_CaLamViec").disabled = false;
                document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
                $('#modalthemmoicalamviec').modal('hide');
                self.LoadReport();
            }
        })
    }
    function callAjaxUpdate(myData) {
        $.ajax({
            data: myData,
            url: NhanSuUri + "PutNS_CaLamViec?ID_NhanVien=" + _id_NhanVien + "&ID_DonVi=" + _IDchinhanh + "&ID_CaLamViec=" + self.ID_CaLamViec(),
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",

            success: function (item) {
                //localStorage.removeItem('lc_SaoChep');
                ShowMessage_Success('Cập nhật ca làm việc thành công');
                //bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Tạo mới ca làm việc thành công", "success");
                document.getElementById("Save_CaLamViec").disabled = false;
                document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
                $('#modalthemmoicalamviec').modal('hide');
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                },
                500: function (item) {
                        ShowMessage_Danger("Cập nhật ca làm việc không thành công");
                }
            },
            complete: function (item) {
                document.getElementById("Save_CaLamViec").disabled = false;
                document.getElementById("Save_CaLamViec").lastChild.data = " Lưu";
                $('#modalthemmoicalamviec').modal('hide');
                self.LoadReport();
            }
        })
    }
    function callAjaxDelete() {
        $.ajax({
            url: NhanSuUri + "DeleteNS_CaLamViec?ID_CaLamViec=" + self.ID_CaLamViec(),
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            success: function (item) {
                ShowMessage_Success('Xóa ca làm việc thành công');
                $('#modalpopup_delete').modal('hide');
            },
            statusCode: {
                404: function (item) {
                    self.error("page not found");
                },
                500: function (item) {
                    ShowMessage_Danger("Xóa ca làm việc không thành công");
                }
            },
            complete: function (item) {
                ShowMessage_Success('Xóa ca làm việc thành công');
                $('#modalpopup_delete').modal('hide');
                self.LoadReport();
            }
        })
    }
    // xuất excel 
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
            ID_DonVi: _IDchinhanh,
            ChucNang: "Danh mục ca làm việc",
            NoiDung: "Xuất danh sách ca làm việc",
            NoiDungChiTiet: "Xuất danh sách ca làm việc",
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
                //if (self.BCBH_XuatFile() != "BCBH_XuatFile") {
                //    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                //    LoadingForm(false);
                //    return false;
                //}
                //if (self.check_MoiQuanTam() == 1 && self.BaoCaoBanHang_TongHop().length != 0) {
                //    $.ajax({
                //        type: "POST",
                //        dataType: "json",
                //        url: ReportUri + "Export_BCBH_TongHop",
                //        data: { objExcel: array_Seach },
                //        success: function (url) {
                //            self.DownloadFileTeamplateXLSX(url)
                //            LoadingForm(false);
                //        }
                //    });
                //}
                if (self.NhanSu_CaLamViec().length != 0) {
                    var url = NhanSuUri + "Export_NhanSu_CaLamViec?trangthaiAD=" + _trangthaiAD + "&trangthaiKAD=" + _trangthaiKAD + "&trangthaiXoa=" + _trangthaiXoa + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&MaCa=" + txt_seach + "&ColumnsHide=" + columnHide + "&TodayBC=" + self.TodayBC();
                    window.location.href = url;
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
        //loadHtmlGrid();
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
            self.pageNumber_CLV(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.pageNumber_CLV(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.pageNumber_CLV(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.pageNumber_CLV(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.pageNumber_CLV(_pageNumber);
        self.ReserPage();
    };
    // TuanDL ẩn hiện cột
    var Key_Form = 'Key_DanhSachCaLamViec';
    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#select-column .dropdown-list ul li input[type = checkbox]'),[]);
       
    }
    $('#select-column').on('change', '.dropdown-list ul li input[type = checkbox]', function () {
        var valueCheck = $(this).val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
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
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, valueCheck);
        $('.' + valueCheck).toggle();
    });

    loadHtmlGrid();

}
ko.applyBindings(new ViewModal());