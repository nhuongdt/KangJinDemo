var NewCalendar = function () {
    var self = this;
    self.Ma_TieuDe = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.KieuLap = ko.observable(0);
    self.SoLanLap = ko.observable(1);
    self.GiaTriLap = ko.observable();
    self.TuanLap = ko.observable();
    self.TenDichVus = ko.observable();
    self.NgayTao = ko.observable(moment(new Date()).format('DD/MM/YYYY'));
    self.ThoiGianTao = ko.observable(moment(new Date()).format('HH:mm'));
    self.TrangThaiKetThuc = ko.observable('1');
    self.GiaTriKetThuc = ko.observable();
    self.GhiChu = ko.observable();
}

var PartialCalendar = function () {
    var self = this;
    self.ID_NhanVien = ko.observable();
    self.ID_DonVi = ko.observable();
    self.UserLogin = ko.observable();
    self.AddProductSuccess = ko.observable(false);
    self.AllKhachHang = ko.observableArray();
    self.ListService = ko.observableArray();
    self.ListServiceChosed = ko.observableArray();
    self.KieuLapLai = ko.observableArray();
    self.LblSoLanLap = ko.observable();
    self.newCalendar = ko.observable(new NewCalendar());
    self.Monday = ko.observable(false);
    self.Tuesday = ko.observable(false);
    self.Wedday = ko.observable(false);
    self.Thurday = ko.observable(false);
    self.Friday = ko.observable(false);
    self.Satuday = ko.observable(false);
    self.Sunday = ko.observable(false);
    self.TrangThaiKT_0 = ko.observable(false);
    self.TrangThaiKT_1 = ko.observable(false);
    self.TrangThaiKT_2 = ko.observable(false);
    self.GiaTriKetThuc_1 = ko.observable();
    self.GiaTriKetThuc_2 = ko.observable();

    function GetListRepeat() {
        self.KieuLapLai.push({ ID: 0, Text: 'Không lặp' })
        self.KieuLapLai.push({ ID: 1, Text: 'Hằng ngày' })
        self.KieuLapLai.push({ ID: 2, Text: 'Hằng tuần' });
        self.KieuLapLai.push({ ID: 3, Text: 'Hằng tháng' });
        self.KieuLapLai.push({ ID: 4, Text: 'Hằng năm' });
        //self.KieuLapLai.push({ ID: 5, Text: 'Tùy chỉnh' });
    }

    GetListRepeat();


    var calendarDV_index = -1;
    var vueCalendarDV = new Vue({
        el: '#calendarVueDV',
        data: function () {
            return {
                query: '',
                data_DV: [],
            }
        },
        methods: {
            reset: function () {
                this.data_DV = [];
                this.query = '';
            },
            click: function (item) {
                self.Calendar_ChoseService(item);
                $('#calendar_lstDV').hide();
            },
            submit: function (event) {
                var keyCode = event.keyCode;
                switch (keyCode) {
                    case 13:
                        var result = this.filterDV(this.query);
                        var focus = false;
                        $('#calendar_lstDV ul li').each(function (i) {
                            if ($(this).hasClass('hoverenabled')) {
                                self.Calendar_ChoseService(result[i]);
                                $('#calendar_lstDV').hide();
                                focus = true;
                            }
                        });
                        if (result.length > 0 && this.query !== '' && focus === false) {
                            $('#calendar_lstDV').hide();
                        }
                        break;
                    case 38:
                        calendarDV_index = calendarDV_index - 1;
                        if (calendarDV_index < 0) {
                            calendarDV_index = $("#calendar_lstDV ul li").length - 1;
                            $('#calendar_lstDV').stop().animate({
                                scrollTop: $('#calendar_lstDV').offset().top + 500
                            }, 1000);
                        }
                        else if (calendarDV_index > 0 && calendarDV_index < 10) {
                            $('#calendar_lstDV').stop().animate({
                                scrollTop: $('#calendar_lstDV').offset().top - 600
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                    case 40:
                        calendarDV_index = calendarDV_index + 1;
                        if (calendarDV_index >= ($("#calendar_lstDV ul li").length)) {
                            calendarDV_index = 0;
                            $('#calendar_lstDV').stop().animate({
                                scrollTop: $('#calendar_lstDV').offset().top - 600
                            }, 1000);
                        }
                        else if (calendarDV_index > 9 && calendarDV_index < $("#calendar_lstDV ul li").length) {
                            $('#calendar_lstDV').stop().animate({
                                scrollTop: $('#calendar_lstDV').offset().top + 500
                            }, 1000);
                        }
                        this.loadFocus();
                        break;
                }
            },
            loadFocus: function () {
                $('#calendar_lstDV ul li').each(function (i) {
                    $(this).removeClass('hoverenabled');
                    if (calendarDV_index === i) {
                        $(this).addClass('hoverenabled');
                    }
                });
            },
            // Tìm kiếm khách hàg
            filterDV: function (value) {
                $('#calendar-listDVStart').hide()
                if (value === '') return self.ListService().slice(0, 30);
                let txt = locdau(value);
                return self.ListService().filter(function (item) {
                    return item['Name'].indexOf(txt) > -1;
                }).slice(0, 20);
            },
        },
        computed: {
            // Return Khách hàng
            SearchService: function () {
                var result = this.filterDV(this.query);
                if (result.length < 1) {
                    $('#calendar_lstDV').hide();
                }
                else {
                    calendarDV_index = 0;
                    $('#calendar_lstDV').show();
                }
                $('#calendar_lstDV ul li').each(function (i) {
                    if (i === 0) {
                        $(this).addClass('hoverenabled');
                    }
                    else {
                        $(this).removeClass('hoverenabled');
                    }
                });
                $('#calendar_lstDV').stop().animate({
                    scrollTop: $('#calendar_lstDV').offset().top - 600
                }, 1000);
                return result;
            }
        }
    });

    self.Calendar_ChoseCusTomer = function (item) {
        $('#calendar_txtKH').val(item.TenDoiTuong);
        self.newCalendar().ID_KhachHang(item.ID);
    }

    self.Calendar_ChoseService = function (item) {
        if (self.ListServiceChosed().length === 0) {
            $('.calendar-servicechosed li:eq(0)').remove();
        }
        var arr = [];
        for (let i = 0; i < self.ListServiceChosed().length; i++) {
            if ($.inArray(self.ListServiceChosed()[i], arr) === -1) {
                arr.push(self.ListServiceChosed()[i].ID);
            }
        }
        if ($.inArray(item.ID, arr) === -1) {
            self.ListServiceChosed.push(item);
        }

        var arrID_After = [];
        for (let i = 0; i < self.ListServiceChosed().length; i++) {
            arrID_After.push(self.ListServiceChosed()[i].ID)
        }
    }

    self.Calendar_RemoveService = function (item) {
        self.ListServiceChosed.remove(item);
    }

    self.ResetDateOfWeekChosed = function () {
        self.Monday(false);
        self.Tuesday(false);
        self.Wedday(false);
        self.Thurday(false);
        self.Friday(false);
        self.Satuday(false);
        self.Sunday(false);
    }

    self.Calendar_ResetNew = function () {
        $('#calendar_txtKH').val('');
        $('#calendar_txtDV').val('');
        $('#btnTypeRepeat').text('Không lặp');
        self.GiaTriKetThuc_1('');
        self.GiaTriKetThuc_2('');
        self.newCalendar().TrangThaiKetThuc('0');
        self.newCalendar().TrangThaiKetThuc('1');
        self.newCalendar().KieuLap(0);
        self.newCalendar().SoLanLap(1);
        self.ListServiceChosed([]);
    }

    self.ChangeNgayDatLich = function () {
        var typeChosing = $.grep(self.KieuLapLai(), function (x) {
            return x.ID === self.newCalendar().KieuLap();
        });
        if (typeChosing.length > 0) {
            self.ChoseRepeatType(typeChosing[0]);
        }
    }

    self.ChoseRepeatType = function (item) {
        $('#btnTypeRepeat').text(item.Text);
        $('#btnTypeRepeat').append('<i class="fa fa-caret-down" style="float:right"></i>');

        self.ResetDateOfWeekChosed();

        var ngaydatlich = new Date(moment(self.newCalendar().NgayTao(), 'DD/MM/YYYY').format('YYYY-MM-DD'));
        var dateOfWeek = ngaydatlich.getDay() + 1; // thu 2,3,4
        var dayOfMonth = ngaydatlich.getDate();
        var month = ngaydatlich.getMonth() + 1;
        var weekOfMonth = (0 | dayOfMonth / 7) + 1;

        switch (item.ID) {
            case 1:
                self.newCalendar().KieuLap(1);
                self.newCalendar().GiaTriLap('');
                self.newCalendar().TuanLap('');
                self.LblSoLanLap('Ngày/lần');
                break;
            case 2:
                self.newCalendar().KieuLap(2);
                self.newCalendar().TuanLap('');
                self.LblSoLanLap('Tuần/lần');
                let strDateOfWeek = dateOfWeek.toString();
                switch (dateOfWeek) {
                    case 2:
                        self.Monday(true);
                        break;
                    case 3:
                        self.Tuesday(true);
                        break;
                    case 4:
                        self.Wedday(true);
                        break;
                    case 5:
                        self.Thurday(true);
                        break;
                    case 6:
                        self.Friday(true);
                        break;
                    case 7:
                        self.Satuday(true);
                        break;
                    case 1:
                        self.Sunday(true);
                        break;
                }
                break;
            case 3:
                self.newCalendar().KieuLap(3);
                self.newCalendar().GiaTriLap(dayOfMonth);// thứ ...
                self.newCalendar().TuanLap(0); // tuần thứ ... của tháng
                self.LblSoLanLap('Tháng/lần');
                $('#btnRepeatMonth').text('Vào ngày ' + dayOfMonth);
                break;
            case 4:
                self.newCalendar().KieuLap(4);
                self.newCalendar().GiaTriLap(dayOfMonth); // ngày .. trong tháng
                self.newCalendar().TuanLap(month); // tháng thứ .. của năm
                self.LblSoLanLap('Năm/lần');
                $('#btnRepeatMonth').text('Vào ngày '.concat(dayOfMonth, ' tháng ', month));
                break;
            case 5: // tuy chinh
                self.newCalendar().KieuLap(5);
                self.newCalendar().GiaTriLap('');
                self.newCalendar().TuanLap('');
                break;
            default: // khong lap
                self.newCalendar().KieuLap(0);
                self.newCalendar().GiaTriLap('');
                self.newCalendar().TuanLap('');
                break;
        }
    }

    function Calendar_EnableBtnSave() {
        $('#calendar-btnSave').text('Đặt lịch');
        $('#calendar-btnSave').removeAttr('disabled');
        $('#partialCalendar .modal-body').gridLoader({ show: false });
    }

    self.SaveCalendar = function () {
        $('#calendar-btnSave').text('Đang lưu');
        $('#calendar-btnSave').attr('disabled', 'disabled');
        $('#partialCalendar .modal-body').gridLoader();

        var idKhachHang = self.newCalendar().ID_KhachHang();
        var sDichVu = '';
        var date = self.newCalendar().NgayTao();
        var time = self.newCalendar().ThoiGianTao();
        var timeAdd2 = moment.utc(time, 'HH:mm').add('hour', 1).format('HH:mm');
        var datetime = moment(date, 'DD/MM/YYYY').format('YYYY-MM-DD').concat(' ', time);
        var datetimeEnd = moment(date, 'DD/MM/YYYY').format('YYYY-MM-DD').concat(' ', timeAdd2);
        var kieulap = self.newCalendar().KieuLap();
        var gtriLap = '';
        var sLap = ' <br /> - Lặp định kỳ: ';
        var sKetThuc = '';
        var trangthaikt = self.newCalendar().TrangThaiKetThuc();
        if (idKhachHang === undefined) {
            ShowMessage_Danger('Vui lòng chọn khách hàng');
            Calendar_EnableBtnSave();
            return false;
        }
        if (self.ListServiceChosed().length === 0) {
            ShowMessage_Danger('Vui lòng chọn dịch vụ');
            Calendar_EnableBtnSave();
            return false;
        }

        if (kieulap === 0) {
            sLap = sLap.concat('Không lặp');
        }
        else {
            sLap = sLap.concat($('#btnTypeRepeat').text(), ': ', self.newCalendar().SoLanLap(), ' ', self.LblSoLanLap());
            sKetThuc = '<br /> - Kết thúc: ';
        }

        if (kieulap === 2) {
            if (self.Monday()) {
                gtriLap = '2,';
            }
            if (self.Tuesday()) {
                gtriLap += '3,';
            }
            if (self.Wedday()) {
                gtriLap += '4,';
            }
            if (self.Thurday()) {
                gtriLap += '5,';
            }
            if (self.Friday()) {
                gtriLap += '6,';
            }
            if (self.Satuday()) {
                gtriLap += '7,';
            }
            if (self.Sunday()) {
                gtriLap += '8,';
            }
            self.newCalendar().GiaTriLap(gtriLap);
            sLap = sLap.concat(' vào thứ: ', gtriLap);
        }
        switch (parseInt(trangthaikt)) {
            case 1:
                self.newCalendar().GiaTriKetThuc('');
                sKetThuc = kieulap === 0 ? '' : sKetThuc.concat('Không bao giờ')
                break;
            case 2:
                if (self.GiaTriKetThuc_1() === '') {
                    ShowMessage_Danger('Vui lòng nhập ngày kết thúc');
                    Calendar_EnableBtnSave();
                    return false;
                }
                if (moment(date, 'DD/MM/YYYY').format('YYYY-MM-DD') > moment(self.GiaTriKetThuc_1(), 'DD/MM/YYYY').format('YYYY-MM-DD')) {
                    ShowMessage_Danger('Ngày kết thúc phải lớn hơn hoặc bằng Thời gian tạo');
                    Calendar_EnableBtnSave();
                    return false;
                }
                self.newCalendar().GiaTriKetThuc(moment(self.GiaTriKetThuc_1(), 'DD/MM/YYYY').format('YYYY-MM-DD'));
                sKetThuc = sKetThuc.concat('Vào ngày: ', self.GiaTriKetThuc_1());
                break;
            case 3:
                self.newCalendar().GiaTriKetThuc(self.GiaTriKetThuc_2());
                sKetThuc = sKetThuc.concat('Sau: ', self.GiaTriKetThuc_2(), ' lần');
                break;
        }

        for (let i = 0; i < self.ListServiceChosed().length; i++) {
            sDichVu += self.ListServiceChosed()[i].TenHangHoa + ', ';
        }
        sDichVu = Remove_LastComma(sDichVu);

        var obj = {
            PhanLoai: 3,
            Ma_TieuDe: sDichVu,
            ID_DonVi: self.ID_DonVi(),
            ID_NhanVien: self.ID_NhanVien(),
            ID_NhanVienQuanLy: self.ID_NhanVien(),
            NguoiTao: self.UserLogin(),
            ID_KhachHang: self.newCalendar().ID_KhachHang(),
            NgayGio: datetime,
            NgayGioKetThuc: datetimeEnd,
            KieuLap: kieulap,
            SoLanLap: self.newCalendar().SoLanLap(),
            GiaTriLap: self.newCalendar().GiaTriLap(),
            TrangThaiKetThuc: trangthaikt,
            GiaTriKetThuc: self.newCalendar().GiaTriKetThuc(),
            TrangThai: '1', // dang xu ly (Dat lich)  ,
            TuanLap: self.newCalendar().TuanLap(),
            GhiChu: self.newCalendar().GhiChu(),
        }
        console.log(obj);
        var myData = {
            objCongViec: obj,
        }
        ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/' + 'Post_ChamSocKhachHang', 'POST', myData).done(function (x) {
            if (x.res === true) {
                ShowMessage_Success('Thêm mới lịch hẹn thành công');

                var noidung = 'lịch hẹn '.concat(' <b>', obj.Ma_TieuDe, ' </b>');
                var chitiet = noidung.concat('<br /> - Thời gian: ', date, ' <br /> - Khách hàng: ', $('#calendar_txtKH').val(), sLap, sKetThuc,
                    '<br /> - Người tạo: ', self.UserLogin());
                let objDiary = {
                    ID_NhanVien: self.ID_NhanVien(),
                    ID_DonVi: self.ID_DonVi(),
                    ChucNang: 'Bán hàng - Lịch hẹn',
                    LoaiNhatKy: 1,
                    NoiDung: 'Thêm mới '.concat(noidung),
                    NoiDungChiTiet: 'Thêm mới '.concat(chitiet),
                }
                Insert_NhatKyThaoTac_1Param(objDiary);
            }
            else {
                ShowMessage_Danger(x.mes);
            }
            $('#partialCalendar').modal('hide');
            $('#partialCalendar .modal-body').gridLoader({ show: false });
            Calendar_EnableBtnSave();
        });
    }
}

$('#calendar-date, #calendar-datefinish').datetimepicker({
    format: 'd/m/Y',
    mask: true,
    timepicker: false,
});
$('#calendar-time').datetimepicker({
    format: 'H:i',
    step: 5,
    datepicker: false,
})