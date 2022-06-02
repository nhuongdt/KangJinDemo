var dtNow = new Date();
var _idNhanVien = $('.idnhanvien').text();
var _tenNhanVien = $('#txtTenNhanVien').val();
var txtLoaiCongViecModal = '#txtLoaiCongViec';
var txtStaffInchargeModal = '#txtStaffIncharge';
var _idDonVi = $('#hd_IDdDonVi').val();

var FormModel_NewLoaiCongViec = function () {
    var self = this;
    self.ID = ko.observable();
    self.TenLoaiTuVanLichHen = ko.observable();
    self.TuVan_LichHen = ko.observable();
    self.TrangThai = ko.observable('2'); // 0.xoa, 1. chua xoa
    self.NguoiSua = ko.observable();
    self.NgaySua = ko.observable();

    self.setdata = function (item) {
        self.ID(item.ID);
        self.TenLoaiTuVanLichHen(item.TenLoaiTuVanLichHen);
        self.TuVan_LichHen(item.TuVan_LichHen);
        self.TrangThai(item.TrangThai);
        self.NguoiSua(item.NguoiSua);
        self.NgaySua(item.NgaySua);
    }
}

var FormModel_NewCongViec = function () {
    var self = this;
    self.ID = ko.observable();
    self.PhanLoai = ko.observable();
    self.ID_LoaiTuVan = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.ID_LienHe = ko.observable(); // not use
    self.ID_DonVi = ko.observable();
    self.NgayGio = ko.observable();
    self.NgayGioKetThuc = ko.observable();
    self.KieuNhacNho = ko.observable(0);
    self.NhacNho = ko.observable();
    self.ID_NhanVien = ko.observable(_idNhanVien);
    self.GhiChu = ko.observable('');
    self.KetQua = ko.observable();
    self.CaNgay = ko.observable(false);
    self.NhacTruocLienHeLai = ko.observable(); // not use
    self.TrangThai = ko.observable(1);
    self.Ma_TieuDe = ko.observable();
    self.MucDoUuTien = ko.observable(2);
    self.FileDinhKem = ko.observable(''); // not use
    self.NgayHoanThanh = ko.observable('');
    self.KieuLap = ko.observable(0);
    self.SoLanLap = ko.observable(1);
    self.GiaTriLap = ko.observable();
    self.TuanLap = ko.observable();
    self.TrangThaiKetThuc = ko.observable('1');
    self.GiaTriKetThuc = ko.observable();
    self.ExistDB = ko.observable(false);
    self.ID_Parent = ko.observable();

    self.SetData = function (item) {
        let trangthaiCV = parseInt(item.TrangThai);
        self.ID(item.ID);
        self.PhanLoai(item.PhanLoai);
        self.ID_LoaiTuVan(item.ID_LoaiTuVan);
        self.ID_KhachHang(item.ID_KhachHang);
        self.ID_LienHe(item.ID_LienHe);
        self.ID_DonVi(item.ID_DonVi);
        self.KieuNhacNho(item.KieuNhacNho);
        self.NhacNho(item.NhacNho);
        self.KieuLap(item.KieuLap);
        self.SoLanLap(item.SoLanLap);
        self.GiaTriLap(item.GiaTriLap);
        self.TuanLap(item.TuanLap);
        self.TrangThaiKetThuc(item.TrangThaiKetThuc.toString());
        self.GiaTriKetThuc(item.GiaTriKetThuc);
        self.GhiChu(item.GhiChu);
        self.KetQua(item.KetQua);
        self.CaNgay(item.CaNgay);
        self.TrangThai(trangthaiCV);
        self.Ma_TieuDe(item.Ma_TieuDe);
        self.MucDoUuTien(item.MucDoUuTien);
        self.ExistDB(item.ExistDB);
        self.ID_Parent(item.ID_Parent);
        self.NgayGio(item.NgayGio);
        self.NgayGioKetThuc(item.NgayGioKetThuc);
        partialWork.ListServiceChosed([]);
        partialWork.PhanLoai(item.PhanLoai);

        if (item.LoaiDoiTuong === 1 || item.LoaiDoiTuong === 0) {
            $('#calendar_txtKH').attr('placeholder', 'Tìm kiếm khách hàng');
            $('#calendar_txtKH2').attr('placeholder', 'Tìm kiếm khách hàng');

            $('#calendar_txtKH').val(item.TenDoiTuong);
            $('#calendar_txtKH2').val(item.TenDoiTuong);

            if (item.PhanLoai === 4) {
                $('#tabCalendar li').removeClass('active');
                $('#tabCalendar li:eq(0)').addClass('active');
                $('#lichhen, #congviec').removeClass('active');
                $('#congviec').addClass('active');
            }
            else {
                $('#tabCalendar li').removeClass('active');
                $('#tabCalendar li:eq(1)').addClass('active');
                $('#lichhen, #congviec').removeClass('active');
                $('#lichhen').addClass('active');

                let arrDV = item.Ma_TieuDe.split(',');
                for (let i = 0; i < arrDV.length; i++) {
                    ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/GetListDichVu_inLichHen_ByEventID/' + item.ID_Parent, 'GET').done(function (x) {
                        if (x.res === true) {
                            partialWork.ListServiceChosed(x.data);
                        }
                    });
                }
            }
        }
        else {
            $('#calendar_txtKH').val(item.TenDoiTuong);
            $('#calendar_txtKH').attr('placeholder', 'Tìm kiếm nhà cung cấp');
            $('#tabCalendar li').removeClass('active');
            $('#tabCalendar li:eq(0)').addClass('active');
            $('#lichhen, #congviec').removeClass('active');
            $('#congviec').addClass('active');
        }

        var statusCV = $.grep(partialWork.ListStatusWork(), function (x) {
            return x.ID === trangthaiCV;
        });
        $('#lblStatusWork').text(statusCV[0].Text);
        $('i[id=spanCheckStatusWork_' + trangthaiCV + ']').addClass('fa fa-check');

        if (item.KieuNhacNho !== 0) {
            let typeRemind = $.grep(partialWork.ListTypeRemind(), function (x) {
                return x.ID === item.KieuNhacNho;
            });
            $('#lblTypeRemind').text(typeRemind[0].Text);
            $('i[id=spanCheckTypeRemind_' + item.KieuNhacNho + ']').addClass('fa fa-check');
        }

        // kieulap
        let kieulap = item.KieuLap;
        let typeRepeat = $.grep(partialWork.KieuLapLai(), function (x) {
            return x.ID === kieulap;
        });
        $('#lblTypeRepeat').text(typeRepeat[0].Text);
        $('i[id=spanCheckRepeatType_' + item.KieuLap + ']').addClass('fa fa-check');

        let dayOfmonth_DateFrom = (new Date(item.NgayGio)).getDate();
        switch (kieulap) {
            case 1:// ngay
                partialWork.LblSoLanLap('Ngày/lần');
                break;
            case 2:// tuan
                partialWork.LblSoLanLap('Tuần/lần');
                var arrGtri = item.GiaTriLap.split(',');
                for (let i = 0; i < arrGtri.length; i++) {
                    switch (parseInt(arrGtri[i])) {
                        case 2:
                            partialWork.Monday(true);
                            break;
                        case 3:
                            partialWork.Tuesday(true);
                            break;
                        case 4:
                            partialWork.Wedday(true);
                            break;
                        case 5:
                            partialWork.Thurday(true);
                            break;
                        case 6:
                            partialWork.Friday(true);
                            break;
                        case 7:
                            partialWork.Satuday(true);
                            break;
                        case 8:
                            partialWork.Sunday(true);
                            break;
                    }
                }
                break;
            case 3:
                partialWork.LblSoLanLap('Tháng/lần');
                $('#btnRepeatMonth').text('Vào ngày ' + item.GiaTriLap);
                break;
            case 4:
                partialWork.LblSoLanLap('Năm/lần');
                $('#btnRepeatMonth').text('Vào ngày ' + item.GiaTriLap);
                break;
        }

        switch (item.TrangThaiKetThuc) {
            case 2:
                partialWork.GiaTriKetThuc_1(moment(item.GiaTriKetThuc).format('DD/MM/YYYY'));
                partialWork.GiaTriKetThuc_2('');
                break;
            case 3:
                partialWork.GiaTriKetThuc_1('');
                partialWork.GiaTriKetThuc_2(item.GiaTriKetThuc);
                break;
            default:
                break;
        }

        if (item.TenLoaiTuVanLichHen !== '') {
            $(txtLoaiCongViecModal).text(item.TenLoaiTuVanLichHen);
            $('i[id=spanCheckLoaiCongViec_' + item.ID_LoaiTuVan + ']')
                .addClass('fa fa-check');
        }

        //$('#lstNhanVien li span i').remove();
        $(txtStaffInchargeModal).text(_tenNhanVien);
        if (item.ID_NhanVien !== null && item.ID_NhanVien !== const_GuidEmpty) {
            self.ID_NhanVien(item.ID_NhanVien);
            $(txtStaffInchargeModal).text(item.TenNhanVien);
            $('i[id=spanCheckNhanVien_' + item.ID_NhanVien + ']').addClass('fa fa-check');
        }
        else {
            self.ID_NhanVien(_idNhanVien);
            $(txtStaffInchargeModal).text(_tenNhanVien);
            $('i[id=spanCheckNhanVien_' + _idNhanVien + ']').addClass('fa fa-check');
        }

        var dateFrom = moment(item.NgayGio).format('DD/MM/YYYY');
        var timeFrom = moment(item.NgayGio).format('HH:mm');
        partialWork.DateFrom(dateFrom);
        partialWork.TimeFrom(timeFrom);

        if (item.NgayGioKetThuc != null) {
            dateFrom = moment(item.NgayGioKetThuc).format('DD/MM/YYYY');
            timeFrom = moment(item.NgayGioKetThuc).format('HH:mm');
            partialWork.DateTo(dateFrom);
            partialWork.TimeTo(timeFrom);
        }
        else {
            partialWork.DateTo('');
            partialWork.TimeTo('');
        }
        self.FileDinhKem(item.FileDinhKem);
        if (item.FileDinhKem !== null) {
            $('#workfile').show();
        }
        else {
            $('#workfile').hide();
        }
    };
};

var PartialView_CongViec = function () {
    var self = this;
    var ChamSocKhachHangUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DM_LoaiTVLHUri = '/api/DanhMuc/DM_LoaiTuVanLichHenAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var userLogin = $('#txtTenTaiKhoan').text().trim();
    self.DateFromOld = ko.observable();
    self.EventOld = ko.observable();// used to compare event old - new
    self.DateFrom = ko.observable();
    self.DateTo = ko.observable();
    self.TimeFrom = ko.observable();
    self.TimeTo = ko.observable();
    self.DateFinish = ko.observable();
    self.newCongViec = ko.observable(new FormModel_NewCongViec());
    self.newLoaiCongViec = ko.observable(new FormModel_NewLoaiCongViec());

    self.LoaiCongViecs = ko.observableArray();
    self.ListAllDoiTuong = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.LoaiKhachHang = ko.observableArray();// not use
    self.listUserContact = ko.observableArray();// not use
    self.IsAddTypeWork = ko.observable(true);
    self.booleanAddCV = ko.observable(true);
    self.SaveSuscess = ko.observable(0);// 0. Khong thanh cong, 1.insert/update/delete CV, 2. insert/update/delete loaiCV
    self.IsCustomer = ko.observable(true);
    self.ListStaffShare = ko.observableArray([]);// nhan vien phoi hop thuc hien (not use)
    self.FileSelect = ko.observableArray(); // not use
    self.JqAutoSelectKH_CV = ko.observable();
    self.JqAutoSelectKH_LH = ko.observable();
    self.JqAutoSelectHH = ko.observable();
    self.InforCustomer = ko.observableArray();
    self.ListAllService = ko.observableArray();
    self.filterNVPhuTrach = ko.observable();
    self.filterLoaiCongViec = ko.observable();
    self.ListServiceChosed = ko.observableArray();
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
    self.KieuLapLai = ko.observableArray();
    self.LblSoLanLap = ko.observable();
    self.PhanLoai = ko.observable(4);
    self.TypeUpdate = ko.observable('1');

    self.ListTypeRemind = ko.observableArray([
        { ID: 1, Text: 'Phút' },
        { ID: 2, Text: 'Giờ' },
        { ID: 3, Text: 'Ngày' },
        { ID: 4, Text: 'Tháng' },
    ]);

    self.ListStatusWork = ko.observableArray([
        { ID: 1, Text: 'Đang xử lý' },
        { ID: 2, Text: 'Hoàn thành' },
        { ID: 3, Text: 'Hủy' },
    ]);

    self.KieuLapLai = ko.observableArray([
        { ID: 0, Text: 'Không lặp' },
        { ID: 1, Text: 'Hằng ngày' },
        { ID: 2, Text: 'Hằng tuần' },
        { ID: 3, Text: 'Hằng tháng' },
        { ID: 4, Text: 'Hằng năm' }
    ]);

    function SearchKhachHang(id) {
        ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'GetDM_DoiTuong/' + id, 'GET').done(function (data) {
            self.InforCustomer(data);
            if (self.booleanAddCV()) {
                $('#calendar_txtKH').val(data.TenDoiTuong);
                $('#calendar_txtKH2').val(data.TenDoiTuong);
            }
        });
    }

    self.JqAutoSelectKH_CV.subscribe(function (newVal) {
        if (newVal !== undefined && newVal.length === 36) {
            SearchKhachHang(newVal);
            self.newCongViec().ID_KhachHang(newVal);
        }
    });

    self.JqAutoSelectKH_LH.subscribe(function (newVal) {
        if (newVal !== undefined && newVal.length === 36) {
            SearchKhachHang(newVal);
            self.newCongViec().ID_KhachHang(newVal);
        }
    });

    self.JqAutoSelectItem = function (item) {
        self.Calendar_ChoseService(item);
    }

    //self.JqAutoSelectHH.subscribe(function (newVal) {
    //    console.log('newVal', newVal)
    //    if (newVal !== undefined && newVal.length === 36) {
    //        ajaxHelper('/api/DanhMuc/DM_HangHoaAPI/' + 'GetInforProduct_ByIDQuidoi?idQuiDoi=' + newVal + '&idChiNhanh=' + _idDonVi).done(function (x) {
    //         
    //            if (x.res === true) {
    //                self.Calendar_ChoseService(x.data);
    //            }
    //        });
    //    }
    //});

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
                if (value === '') return self.ListAllService().slice(0, 30);
                let txt = locdau(value);
                return self.ListAllService().filter(function (item) {
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

    self.Calendar_ChangeTab = function (val) {
        self.PhanLoai(val);
        if (self.booleanAddCV()) {
            // reset status if change status at tab congViec after click tab LichHen
            self.newCongViec().TrangThai(1);
            $('#lblStatusWork').text('Đang xử lý');
            $('#lstStatusWork li i').each(function () {
                $(this).removeClass('fa fa-check');
            });
            $('i[id=spanCheckStatusWork_' + 1 + ']').addClass('fa fa-check');
        }
    }

    self.Calendar_ChoseService = function (item) {
        if (self.ListServiceChosed().length === 0) {
            $('.calendar-servicechosed li:eq(0)').remove();
        }
        var arr = [];
        for (let i = 0; i < self.ListServiceChosed().length; i++) {
            if ($.inArray(self.ListServiceChosed()[i], arr) === -1) {
                arr.push(self.ListServiceChosed()[i].ID_DonViQuiDoi);
            }
        }
        if ($.inArray(item.ID, arr) === -1) {
            self.ListServiceChosed.push(item);
        }

        var arrID_After = [];
        for (let i = 0; i < self.ListServiceChosed().length; i++) {
            arrID_After.push(self.ListServiceChosed()[i].ID_DonViQuiDoi)
        }
    }

    self.Calendar_RemoveService = function (item) {
        self.ListServiceChosed.remove(item);
    }

    self.ChoseTypeRemind = function (item, event) {
        $('#lblTypeRemind').text(item.Text);
        $('#lstTypeRemind span').each(function () {
            $(this).empty();
        });

        $(event.currentTarget).parent().siblings().find('i.fa').removeClass('fa-check')
        if (item.ID == undefined) {
            $('#lblTypeRemind').text('--- Kiểu nhắc ---');
            self.newCongViec().KieuNhacNho(0);
        }
        else {
            $('#lblTypeRemind').text(item.Text);


            $('i[id=spanCheckTypeRemind_' + item.ID + ']').addClass('fa fa-check');
            self.newCongViec().KieuNhacNho(item.ID);
        }
    }

    self.Change_StatusWork = function (item) {
        self.newCongViec().TrangThai(item.ID);
        self.DateFinish(moment(dtNow).format('DD/MM/YYYY'));
        $('#lstStatusWork span').each(function () {
            $(this).empty();
        });
        $(event.currentTarget).parent().siblings().find('i.fa').removeClass('fa-check')
        if (item.ID == undefined) {
            $('#lblStatusWork').text('--- Trạng thái ---');
        }
        else {
            $('#lblStatusWork').text(item.Text);
            $('i[id=spanCheckStatusWork_' + item.ID + ']').addClass('fa fa-check');
        }
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

    self.Calendar_RemoveCheckAfter = function () {
        $('#lstNhanVien span, #lstTypeRemind span, #lstStatusWork span, #lstLoaiCongViec span, #lstRepeatType span').each(function () {  // remove check after
            $(this).empty();
        });
    }

    self.Calendar_ResetText = function () {
        $('#calendar_txtKH').val('');
        $('#calendar_txtKH2').val('');
        $('#calendar_txtDV').val('');
        $('#txtStaffIncharge').text(_tenNhanVien); // default NV login
        $('#calendar_txtKH').attr('placeholder', 'Tìm kiếm khách hàng');
        $('#lblTypeRepeat').text('Không lặp');
        $(txtLoaiCongViecModal).text('--- Chọn loại công việc ---');
        $('#lblStatusWork').text('Đang xử lý');
        $('#lblTypeRemind').text('---Kiểu nhắc---');
        $('#lblTypeRepeat').text('---Kiểu lặp---');
    }

    self.Calendar_ResetNew = function () {
        $('#tabCalendar li').removeClass('active');
        $('#tabCalendar li:eq(0)').addClass('active');
        $('#lichhen, #congviec').removeClass('active');
        $('#congviec').addClass('active');
        $(function () {
            $('#lstStatusWork i.fa-check').removeClass('fa fa-check'); // remove thẻ i đã checked khi click thêm mới lần 2
            $('i[id=spanCheckStatusWork_' + 1 + ']').addClass('fa fa-check');  // default: Dang xu ly
        });

        self.IsCustomer(true);
        self.PhanLoai(4);
        self.GiaTriKetThuc_1('');
        self.GiaTriKetThuc_2('');
        self.newCongViec().TrangThaiKetThuc('1');
        self.newCongViec().KieuLap(0);
        self.newCongViec().SoLanLap(1);

        self.DateFinish(self.DateFrom());
        self.ListServiceChosed([]);
        self.SaveSuscess(0);
        self.Calendar_ResetText();
        self.Calendar_RemoveCheckAfter();
        self.ResetDateOfWeekChosed();
    }

    self.ChangeNgayDatLich = function () {
        var typeChosing = $.grep(self.KieuLapLai(), function (x) {
            return x.ID === self.newCongViec().KieuLap();
        });
        if (typeChosing.length > 0) {
            self.ChoseRepeatType(typeChosing[0]);
        }
        self.DateTo(self.DateFrom());
    }

    self.SetValue_TrangThaiKetThuc = function (val) {
        if (parseInt(self.newCongViec().TrangThaiKetThuc()) !== parseInt(val)) {
            self.newCongViec().TrangThaiKetThuc(val);
        }
    }

    self.ChoseRepeatType = function (item) {
        $('#lstRepeatType li i').each(function () {
            $(this).removeClass('fa fa-check');
        });
        if (item.ID == undefined) {
            $('#lblTypeRepeat').text('--- Không lặp ---');
        }
        else {
            $('#lblTypeRepeat').text(item.Text);
            $('i[id=spanCheckRepeatType_' + item.ID + ']').addClass('fa fa-check');
            if (self.booleanAddCV()) {
                self.newCongViec().TrangThaiKetThuc('1');
            }
        }

        self.ResetDateOfWeekChosed();
        var ngaydatlich = new Date(moment(self.DateFrom(), 'DD/MM/YYYY').format('YYYY-MM-DD'));
        var dateOfWeek = ngaydatlich.getDay() + 1; // thu 2,3,4
        var dayOfMonth = ngaydatlich.getDate();
        var month = ngaydatlich.getMonth() + 1;
        var weekOfMonth = (0 | dayOfMonth / 7) + 1;

        switch (item.ID) {
            case 1:
                self.newCongViec().KieuLap(1);
                self.newCongViec().GiaTriLap('');
                self.newCongViec().TuanLap('');
                self.LblSoLanLap('Ngày/lần');
                break;
            case 2:
                self.newCongViec().KieuLap(2);
                self.newCongViec().TuanLap('');
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
                    case 8:
                        self.Sunday(true);
                        break;
                }
                break;
            case 3:
                self.newCongViec().KieuLap(3);
                self.newCongViec().GiaTriLap(dayOfMonth);// ngày .. trong tháng
                self.newCongViec().TuanLap(0); // tuần thứ ... của tháng (mặc định chỉ lặp theo ngày)
                self.LblSoLanLap('Tháng/lần');
                $('#btnRepeatMonth').text('Vào ngày ' + dayOfMonth);
                break;
            case 4:
                self.newCongViec().KieuLap(4);
                self.newCongViec().GiaTriLap(dayOfMonth); // ngày .. trong tháng
                self.newCongViec().TuanLap(month); // tháng thứ .. của năm
                self.LblSoLanLap('Năm/lần');
                $('#btnRepeatMonth').text('Vào ngày '.concat(dayOfMonth, ' tháng ', month));
                break;
            case 5: // tuy chinh
                self.newCongViec().KieuLap(5);
                self.newCongViec().GiaTriLap('');
                self.newCongViec().TuanLap('');
                break;
            default: // khong lap
                self.newCongViec().KieuLap(0);
                self.newCongViec().GiaTriLap('');
                self.newCongViec().TuanLap('');
                break;
        }
    }

    self.ChangeTimeFrom = function () {
        var datetime = moment(self.DateFrom(), 'DD/MM/YYYY').format('YYYY-MM-DD') + ' ' + self.TimeFrom();
        var timeto = moment(new Date(datetime)).add(1, 'hours').format('HH:mm');
        self.TimeTo(timeto);
    }

    function Enable_btnSaveCongViec() {
        document.getElementById("btnLuuCongViec").disabled = false;
        document.getElementById("btnLuuCongViec").lastChild.data = "Lưu";
    }

    self.addCongViec = function () {
        var id = self.newCongViec().ID();
        var idLoaiCongViec = self.newCongViec().ID_LoaiTuVan();
        var idKhachHang = self.newCongViec().ID_KhachHang();
        var idNhanVienPhuTrach = self.newCongViec().ID_NhanVien();
        var trangThai = self.newCongViec().TrangThai();
        var ketquaCongViec = self.newCongViec().KetQua();
        var ghichu = self.newCongViec().GhiChu();
        var workName = self.newCongViec().Ma_TieuDe();
        var dateFrom = self.DateFrom();
        var dateTo = self.DateTo();
        var timeFrom = self.TimeFrom();;
        var timeTo = self.TimeTo();;
        var thoiGianTu = dateFrom + " " + timeFrom;
        var thoiGianDen = dateTo + " " + timeTo;
        var priority = self.newCongViec().MucDoUuTien();
        var allDay = self.newCongViec().CaNgay();
        var sLoai = ' công việc';
        var datetimeFinish = null;
        var sLoaiCV = idLoaiCongViec === const_GuidEmpty || idLoaiCongViec === undefined ? '' : ' <br /> - Loại công việc: '.concat($('#txtLoaiCongViec').text());
        var sLap = ' <br /> - Lặp định kỳ: ';
        var sNhacNho = self.newCongViec().KieuNhacNho() === 0 ? ' <br /> - Nhắc nhở: '.concat('Không') : ' <br /> - Nhắc trước: '.concat(self.newCongViec().NhacNho(), ' ', $('#lblTypeRemind').text());
        var sKhachHang = (idKhachHang === undefined || idKhachHang === null) ? '' : phanloai === 4 ? ' <br /> - Khách hàng '.concat($('#calendar_txtKH').val()) : ' <br /> - Khách hàng '.concat($('#calendar_txtKH2').val());
        var sTrangThai = ' <br /> - Trạng thái: '.concat($('#lblStatusWork').text());

        // lich hen
        var gtriLap = '';
        var sDichVu = '';
        var sMaHangHoa = '';
        var kieulap = self.newCongViec().KieuLap();
        var trangthaikt = self.newCongViec().TrangThaiKetThuc();

        if (allDay === true) {
            thoiGianTu = dateFrom;
            thoiGianDen = dateTo + " 23:59";
        }
        if (moment(thoiGianTu, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') === "Invalid date") {
            ShowMessage_Danger('Vui lòng chọn thời gian bắt đầu công việc');
            Enable_btnSaveCongViec();
            $('#txtThoiGianTu').select();
            return false;
        }

        var phanloai = self.PhanLoai();
        if (phanloai === 4) {
            if (workName === "" || workName === undefined) {
                ShowMessage_Danger("Vui lòng nhập tên công việc");
                $('#txtTenCongViec').select();
                Enable_btnSaveCongViec();
                return false;
            }

            if (thoiGianDen !== ' ') {
                if ((moment(thoiGianTu, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') > moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm'))
                    && moment(thoiGianDen, "DD/MM/YYYY HH:mm").format('YYYY-MM-DD HH:mm') !== "Invalid date") {
                    ShowMessage_Danger('Thời gian kết thúc phải lớn hơn thời gian bắt đầu');
                    Enable_btnSaveCongViec();
                    return false;
                }
            }
            else {
                thoiGianDen = null;
            }

            if (trangThai !== 1) {
                datetimeFinish = moment(self.DateFinish(), 'DD/MM/YYYY').format('YYYY-MM-DD');
                datetimeFinish = datetimeFinish + ' ' + self.TimeTo();
                if (datetimeFinish === 'Invalid date') {
                    ShowMessage_Danger("Vui lòng chọn thời gian hoàn thành hoặc hủy");
                    $('.newDateTimesingle').select();
                    Enable_btnSaveCongViec();
                    return false;
                }

                if (datetimeFinish !== "Invalid date" && datetimeFinish < moment(thoiGianTu, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm')) {
                    ShowMessage_Danger("Thời gian hoàn thành phải lớn hơn thời gian bắt đầu");
                    Enable_btnSaveCongViec();
                    return false;
                }
                if (ketquaCongViec === "" || ketquaCongViec === null || ketquaCongViec === undefined) {
                    ShowMessage_Danger("Vui lòng nhập kết quả công việc");
                    $('#txtKetQuaCongViec').select();
                    Enable_btnSaveCongViec();
                    return false;
                }
            }
            sLap = sLap.concat('Không lặp');
        }
        else {
            sLoai = ' lịch hẹn';
            if (idKhachHang === undefined) {
                ShowMessage_Danger('Vui lòng chọn khách hàng');
                Enable_btnSaveCongViec();
                return false;
            }
            //if (self.ListServiceChosed().length === 0) {
            //    ShowMessage_Danger('Vui lòng chọn dịch vụ');
            //    Enable_btnSaveCongViec();
            //    return false;
            //}

            if (kieulap === 0) {
                sLap.concat('Không lặp');
            }
            else {
                sLap = sLap.concat($('#lblTypeRepeat').text(), ' ', self.newCongViec().SoLanLap(), ' ', self.LblSoLanLap());
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
                self.newCongViec().GiaTriLap(gtriLap);
                sLap = sLap.concat(' vào thứ ', gtriLap)
            }

            switch (parseInt(trangthaikt)) {
                case 1:
                    self.newCongViec().GiaTriKetThuc('');
                    break;
                case 2:
                    if (self.GiaTriKetThuc_1() === '') {
                        ShowMessage_Danger('Vui lòng nhập ngày kết thúc');
                        Enable_btnSaveCongViec();
                        return false;
                    }
                    if (moment(dateFrom, 'DD/MM/YYYY').format('YYYY-MM-DD') > moment(self.GiaTriKetThuc_1(), 'DD/MM/YYYY').format('YYYY-MM-DD')) {
                        ShowMessage_Danger('Ngày kết thúc phải lớn hơn hoặc bằng Thời gian tạo');
                        Enable_btnSaveCongViec();
                        return false;
                    }
                    self.newCongViec().GiaTriKetThuc(moment(self.GiaTriKetThuc_1(), 'DD/MM/YYYY').format('YYYY-MM-DD'));
                    break;
                case 3:
                    if (commonStatisJs.CheckNull(self.GiaTriKetThuc_2())) {
                        ShowMessage_Danger('Vui lòng nhập giá trị kết thúc');
                        Enable_btnSaveCongViec();
                        return false;
                    }
                    self.newCongViec().GiaTriKetThuc(self.GiaTriKetThuc_2());
                    break;
            }
            for (let i = 0; i < self.ListServiceChosed().length; i++) {
                sDichVu += self.ListServiceChosed()[i].TenHangHoa + ', ';
                sMaHangHoa += self.ListServiceChosed()[i].MaHangHoa + ', ';
            }
            sDichVu = Remove_LastComma(sDichVu);
            sMaHangHoa = Remove_LastComma(sMaHangHoa);
        }

        if (idNhanVienPhuTrach === const_GuidEmpty) {
            idNhanVienPhuTrach = null;
        }

        var CongViec = {
            ID: id,
            PhanLoai: phanloai,
            Ma_TieuDe: phanloai === 3 ? sDichVu : workName,
            MucDoUuTien: priority,
            ID_LoaiTuVan: idLoaiCongViec === const_GuidEmpty ? null : idLoaiCongViec,
            ID_KhachHang: idKhachHang === undefined ? null : idKhachHang,
            ID_DonVi: _idDonVi,
            ID_NhanVienQuanLy: _idNhanVien,// nhan vien dang nhap ht
            ID_NhanVien: idNhanVienPhuTrach === const_GuidEmpty ? null : idNhanVienPhuTrach,
            NguoiTao: userLogin,
            TrangThai: trangThai,
            KetQua: ketquaCongViec,
            GhiChu: ghichu,
            NoiDung: phanloai === 3 ? sMaHangHoa : '',// muon tam truong NoiDung (luu MaDichVu)
            NhacNho: self.newCongViec().NhacNho(),
            KieuNhacNho: self.newCongViec().KieuNhacNho(),
            NgayHoanThanh: datetimeFinish,
            CaNgay: allDay,
            NgayGio: moment(thoiGianTu, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm'),
            NgayGioKetThuc: thoiGianDen !== null ? moment(thoiGianDen, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm') : null,
            KieuLap: kieulap,
            SoLanLap: self.newCongViec().SoLanLap(),
            GiaTriLap: self.newCongViec().GiaTriLap(),
            TrangThaiKetThuc: phanloai === 3 ? trangthaikt : 0,
            GiaTriKetThuc: self.newCongViec().GiaTriKetThuc(),
            TuanLap: self.newCongViec().TuanLap(),
        }
        var myData = {};
        myData.objCongViec = CongViec;
        myData.LstStaffShare = self.ListStaffShare();

        console.log(CongViec)

        // used to save diary
        var loaiCV = $(txtLoaiCongViecModal).text();
        var nvThucHien = '';
        var itemNV = $.grep(self.NhanViens(), function (x) {
            return x.ID === _idNhanVien;
        });
        if (itemNV.length > 0) {
            nvThucHien = itemNV[0].TenNhanVien;
        }

        // nhatky thao tac
        var noidung = sLoai.concat(' <b>', CongViec.Ma_TieuDe, ' </b>');
        var chitiet = noidung.concat('<br /> - Thời gian: ', thoiGianTu, ' - ', thoiGianDen, sLoaiCV, sKhachHang, sLap, sNhacNho,
            '<br/>- Ghi chú: ', ghichu, '<br />- Nhân viên thực hiện: ', nvThucHien, sTrangThai);
        if (self.booleanAddCV() === true) {
            $.ajax({
                url: ChamSocKhachHangUri + "Post_ChamSocKhachHang",
                type: 'POST',
                async: true,
                dataType: 'json',
                contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                data: myData,
                success: function (obj) {
                    console.log(obj)
                    if (obj.res === true) {
                        ShowMessage_Success('Thêm mới' + sLoai + ' thành công');
                        self.SaveSuscess(1);
                        //self.UpLoadFile(obj.data.ID)

                        let objDiary = {
                            ID_NhanVien: _idNhanVien,
                            ID_DonVi: _idDonVi,
                            ChucNang: 'Công việc - Lịch hẹn',
                            LoaiNhatKy: 1,
                            NoiDung: 'Thêm mới '.concat(noidung),
                            NoiDungChiTiet: 'Thêm mới '.concat(chitiet),
                        }
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    }
                },
                error: function (jqXHR, textStatus, errorThrow) {
                    ShowMessage_Danger('Thêm mới' + sLoai + '  thất bại');
                },
                complete: function () {
                    Enable_btnSaveCongViec();
                    $('#modalPopuplg_Work').modal('hide');
                }
            })
        }
        else {
            CongViec.NguoiSua = userLogin;
            // neu update tu CongViec --> LichHen va nguoclai
            let kieulap_old = self.EventOld().KieuLap;
            let phanloai_old = self.EventOld().PhanLoai;
            let sKieu = ' không';
            let sTrangThai = ' Đang xử lý';

            sLoai = phanloai_old === 3 ? ' lịch hẹn' : 'công việc';
            noidung = sLoai.concat('<b> ', self.EventOld().Ma_TieuDe, ' </b>');

            let objDiary = {
                ID_NhanVien: _idNhanVien,
                ID_DonVi: _idDonVi,
                ChucNang: 'Công việc - Lịch hẹn',
                LoaiNhatKy: 2,
                NoiDung: 'Cập nhật '.concat(noidung),
                NoiDungChiTiet: '<b> Thông tin mới: </b> '.concat(chitiet),
            }

            switch (kieulap_old) {
                case 1:
                    sKieu = ' hằng ngày ('.concat(self.EventOld().SoLanLap, ' ngày/ lần)');
                    break;
                case 2:
                    sKieu = ' hằng tuần ('.concat(self.EventOld().SoLanLap, ' tuần/ lần)');
                    break;
                case 3:
                    sKieu = ' hằng tháng ('.concat(self.EventOld().SoLanLap, ' tháng/ lần)');
                    break;
                case 4:
                    sKieu = ' hằng năm ('.concat(self.EventOld().SoLanLap, ' năm/ lần)');
                    break;
            }

            switch (parseInt(self.EventOld().TrangThai)) {
                case 2:
                    sTrangThai = ' Hoàn thành';
                    break;
                case 3:
                    sTrangThai = ' Hủy';
                    break;
                default:
                    sTrangThai = ' Đang xử lý';
                    break;
            }

            chitiet = chitiet.concat(' <br />  <b> Thông tin cũ </b>: Phân loại: ', phanloai_old == 3 ? ' lịch hẹn' : 'công việc', ', Tiêu đề: ', self.EventOld().Ma_TieuDe,
                ', Lặp định kỳ:', sKieu, ', Trạng thái: ', sTrangThai);
            objDiary.NoiDungChiTiet = '<b> Thông tin mới: </b> '.concat(chitiet);

            if (kieulap_old !== 0 && (phanloai === 3 || phanloai_old === 3)) {

                var typeUpdate = CheckTypeUpdate(CongViec, self.EventOld());
                self.Confirm_UpdateEvent(typeUpdate, function () {
                    switch (parseInt(self.TypeUpdate())) {
                        case 1: // only this event
                            // keep event old, & add new Row
                            myData.objCongViec.ID_Parent = self.newCongViec().ID_Parent();
                            myData.objCongViec.NgayCu = self.DateFromOld();
                            objDiary.NoiDungChiTiet = objDiary.NoiDungChiTiet.concat('<br/ > - Chỉnh sửa sự kiện lặp: chỉ sự kiện này');

                            if (self.newCongViec().ExistDB()) {
                                if (event.ID_Parent === event.ID) {
                                    partialWork.AddEventDB(myData, sLoai, objDiary);
                                }
                                else {
                                    // update
                                    let sql3 = " NgayGio='".concat(CongViec.NgayGio, "' , NgayGioKetThuc='", CongViec.NgayGioKetThuc, "', NguoiSua='", userLogin, "'");
                                    partialWork.UpdateEvent_StartEnd(self.newCongViec().ID(), sql3);
                                }
                            }
                            else {
                                self.AddEventDB(myData, sLoai, objDiary);
                            }
                            break;
                        case 2: // this event & all after
                        case 3: // all evant
                            let sql = " TrangThaiKetThuc= 2".concat(" , GiaTriKetThuc='", moment(new Date(CongViec.NgayGio)).add(-1, 'days').format('YYYY-MM-DD'), "', NguoiSua='", userLogin, "'");
                            self.UpdateEvent_StartEnd(self.newCongViec().ID_Parent(), sql);
                            myData.objCongViec.NgayGioKetThuc = moment(new Date(CongViec.NgayGio)).add(1, 'hours').format('YYYY-MM-DD HH:mm');

                            if (parseInt(self.TypeUpdate()) === 2) {
                                objDiary.NoiDungChiTiet = objDiary.NoiDungChiTiet.concat('<br/ > - Chỉnh sửa sự kiện lặp: sự kiện này và các sự kiện sau');
                            }
                            else {
                                objDiary.NoiDungChiTiet = objDiary.NoiDungChiTiet.concat('<br/ > - Chỉnh sửa sự kiện lặp: tất cả các sự kiện');
                            }       

                            if (self.newCongViec().ExistDB()) {
                                self.UpdateEventDB(myData, sLoai, objDiary);
                            }
                            else {
                                self.AddEventDB(myData, sLoai, objDiary);
                            }
                            break;
                        //case 3: // all event (only update event old)
                        //    self.UpdateEventDB(myData, sLoai, objDiary);
                        //    break;

                    }
                    $("#modal_QuestionUpdateEvent").modal('hide');
                    $('#modalPopuplg_Work').modal('hide');
                });
            }
            else {
                self.UpdateEventDB(myData, sLoai, objDiary);
                $('#modalPopuplg_Work').modal('hide');
            }
        }
    }

    function CheckTypeUpdate(objNew, objOld) {
        var ngaydatlich = moment(self.DateFrom(), 'DD/MM/YYYY').format('YYYY-MM-DD');
        var ngaydatlichOld = moment(objOld.NgayGio).format('YYYY-MM-DD');

        if (objOld.KieuLap !== objNew.KieuLap
            || objOld.ID_KhachHang !== objNew.ID_KhachHang
            || ngaydatlich !== ngaydatlichOld
            || parseInt(objOld.TrangThaiKetThuc) !== parseInt(objNew.TrangThaiKetThuc)) {
            return 2;
        }
        else {
            if (objOld.SoLanLap !== objNew.SoLanLap) {
                return 2;
            }
            else {
                if (objNew.KieuLap === 2) {
                    let arrGtriLap = objNew.GiaTriLap.split(',');
                    let arrGtriLapOld = objOld.GiaTriLap.split(',');
                    if (JSON.stringify(arrGtriLap) !== JSON.stringify(arrGtriLapOld)) {
                        return 2;
                    }
                }
                if (objNew.TrangThaiKetThuc === 2) {
                    let gtriktOld = objOld.GiaTriKetThuc;
                    if (gtriktOld.split(' ').length > 1) {// check yyyy-mm-dd hh:mm
                        gtriktOld = gtriktOld.split(' ')[0];
                    }
                    let ngaykt = moment(self.GiaTriKetThuc_1(), 'DD/MM/YYYY').format('YYYY-MM-DD');
                    if (ngaykt !== gtriktOld) {
                        return 2;
                    }
                }
            }
        }
        return 1;
    }

    self.UpdateEvent_StartEnd = function (id, set) {
        var obj = {
            ID: id,
            SqlSet: set,
        }
        console.log('UpdateEvent_StartEnd ', obj);
        ajaxHelper(ChamSocKhachHangUri + 'Event_UpdateStartEnd', 'POST', obj).done(function (x) {

            let calendar = $('#calendar');
            if ($(calendar).length > 0) $('#calendar').fullCalendar('refetchEvents');// ony refresh if page CongViec
        })
    }

    self.AddEventDB = function (objUp, sLoai, objDiary) {
        ajaxHelper(ChamSocKhachHangUri + "Post_ChamSocKhachHang", 'POST', objUp).done(function (x) {

            if (x.res === true) {
                ShowMessage_Success('Cập nhật ' + sLoai + '  thành công');
                self.SaveSuscess(1);
                Enable_btnSaveCongViec();
                Insert_NhatKyThaoTac_1Param(objDiary);
                let calendar = $('#calendar');
                if ($(calendar).length > 0) $('#calendar').fullCalendar('refetchEvents');
            }
        });
    }

    self.UpdateEventDB = function (objUp, sLoai, objDiary) {
        ajaxHelper(ChamSocKhachHangUri + "Put_ChamSocKhachHang", 'POST', objUp).done(function (x) {
            if (x.res === true) {
                ShowMessage_Success('Cập nhật ' + sLoai + '  thành công');
                self.SaveSuscess(1);
                Enable_btnSaveCongViec();
                if (objDiary != null) {
                    Insert_NhatKyThaoTac_1Param(objDiary);
                }
            }
        });
    }

    self.Confirm_UpdateEvent = function (typeChange, onConfirm) {
        var fClose = function () {
            modal.modal("hide");
            return false;
        };
        switch (typeChange) {
            case 1:// update ghichu, tendichvu,...etc conlai
                self.TypeUpdate('1');
                $('#thisEvent').show();
                $('#thisEventandNext').show();
                $('#allEvent').show();
                break;
            case 2: // change kieulap, giatrilap, trangthai ketthuc, khachhang, ngaydatlich
                self.TypeUpdate('2');
                $('#thisEvent').hide();
                $('#thisEventandNext').show();
                $('#allEvent').show();
                break;
        }
        var modal = $("#modal_QuestionUpdateEvent");
        modal.modal("show");
        $("#btnQuestion_OK").off().one('click', onConfirm);
        $("#btnQuestion_Cancel").off().one("click", fClose);
    }

    self.DeleteLoaiCongViec = function (item) {
        var idDelete = self.newCongViec().ID_LoaiTuVan();
        var loaiCV = self.newLoaiCongViec().TenLoaiTuVanLichHen();
        var sLoai = 'công việc';
        if (self.newLoaiCongViec().TuVan_LichHen() === 3) {
            sLoai = 'lịch hẹn';
        }

        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa loại công việc "' + loaiCV + '"</b> không?', function () {
            $.ajax({
                type: "DELETE",
                url: ChamSocKhachHangUri + "Delete_LoaiCongViec/" + idDelete,
                dataType: 'json',
                contentType: 'application/json',
                success: function (result) {
                    if (result === '') {
                        for (var i = 0; i < self.LoaiCongViecs().length; i++) {
                            if (self.LoaiCongViecs()[i].ID === idDelete) {
                                self.LoaiCongViecs.remove(self.LoaiCongViecs()[i]);
                                break;
                            }
                        }
                        ShowMessage_Success('Xóa loại công việc thành công');
                        // reset infor loaiCV chosing
                        $('#modalTypeWork').modal('hide');
                        $('#txtLoaiCongViec').text('---Chọn loại công việc---');
                        self.IsAddTypeWork(true);
                        self.SaveSuscess(2);

                        let noidung = 'Xóa loại '.concat(sLoai, ' ', loaiCV);
                        var objDiary = {
                            ID_NhanVien: _idNhanVien,
                            ID_DonVi: _idDonVi,
                            ChucNang: 'Công việc - Lịch hẹn',
                            LoaiNhatKy: 3,
                            NoiDung: noidung,
                            NoiDungChiTiet: noidung,
                        }
                        Insert_NhatKyThaoTac_1Param(objDiary);
                    }
                    else {
                        ShowMessage_Danger(result);
                    }
                },
                error: function (error) {
                    $('#modalPopuplgDelete').modal('hide');
                    ShowMessage_Danger('Xóa loại công việc thất bại');
                }
            });
        })
    }

    self.ArrLoaiCongViec_byPhanLoai = ko.computed(function () {
        return $.grep(self.LoaiCongViecs(), function (x) {
            return x.TuVan_LichHen === self.PhanLoai();
        });
    });

    self.addLoaiCongViec = function () {
        var _id = self.newLoaiCongViec().ID();
        var _loaiCongViec = self.newLoaiCongViec().TenLoaiTuVanLichHen();
        if (_loaiCongViec === "" || _loaiCongViec === null || _loaiCongViec === undefined) {
            ShowMessage_Danger('Vui lòng nhập tên công việc');
            return false;
        }
        _loaiCongViec.trim();
        var sLoai = 'công việc';
        if (self.PhanLoai() === 3) {
            sLoai = 'lịch hẹn';
        }

        var myData = {};
        var objLoaiCV = {
            ID: _id,
            TenLoaiTuVanLichHen: _loaiCongViec,
            TuVan_LichHen: self.PhanLoai(),
            NguoiTao: userLogin,
            NgayTao: moment(new Date()).format('YYYY-MM-DD HH:mm:ss')
        }
        myData.objLoaiTVLH = objLoaiCV;

        if (self.IsAddTypeWork()) {
            _id = const_GuidEmpty; // assign to to check exist
        }

        ajaxHelper(ChamSocKhachHangUri + "Check_LoaiCongViec_Exist?loaicongviec=" + _loaiCongViec + '&idloaicv=' + _id + '&loaiTuVan=4', 'GET').done(function (boolReturn) {
            if (boolReturn) {
                ShowMessage_Danger('Loại ' + sLoai + ' đã tồn tại');
            }
            else {
                if (self.IsAddTypeWork()) {
                    $.ajax({
                        url: DM_LoaiTVLHUri + "Post_LoaiTuVanLichHen",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (obj) {
                            if (obj.res === true) {
                                var item = obj.data;
                                self.LoaiCongViecs.unshift(item);
                                self.newCongViec().ID_LoaiTuVan(item.ID);
                                self.IsAddTypeWork(false);
                                self.SaveSuscess(2);

                                $('i[id=spanCheckLoaiCongViec_' + item.ID + ']').addClass('fa fa-check');
                                $(txtLoaiCongViecModal).text(item.TenLoaiTuVanLichHen);
                                ShowMessage_Success('Thêm mới loại công việc thành công');

                                var noiDung = 'Thêm mới loại ' + sLoai + _loaiCongViec;
                                var noiDungChiTiet = noiDung.concat('<br />- Nhân viên tạo: ', userLogin);
                                var objDiary = {
                                    ID_NhanVien: _idNhanVien,
                                    ID_DonVi: _idDonVi,
                                    ChucNang: 'Công việc - Lịch hẹn',
                                    LoaiNhatKy: 1,
                                    NoiDung: noiDung,
                                    NoiDungChiTiet: noiDungChiTiet,
                                }
                                Insert_NhatKyThaoTac_1Param(objDiary);
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrow) {
                            ShowMessage_Danger('Thêm mới loại ' + sLoai + ' thất bại');
                        },
                        complete: function () {
                            $('#modalTypeWork').modal('hide');
                        }
                    })
                }
                else {
                    myData.objLoaiTVLH.NguoiSua = userLogin;

                    $.ajax({
                        url: DM_LoaiTVLHUri + "Put_LoaiTuVanLichHen",
                        type: 'POST',
                        async: true,
                        dataType: 'json',
                        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                        data: myData,
                        success: function (obj) {
                            if (obj.res === true) {
                                for (var i = 0; i < self.LoaiCongViecs().length; i++) {
                                    if (self.LoaiCongViecs()[i].ID === _id) {
                                        self.LoaiCongViecs.remove(self.LoaiCongViecs()[i]);
                                        break;
                                    }
                                }
                                self.LoaiCongViecs.push(objLoaiCV);
                                self.SaveSuscess(2);
                                $(txtLoaiCongViecModal).text(objLoaiCV.TenLoaiTuVanLichHen);
                                ShowMessage_Success('Cập nhật loại ' + sLoai + ' thành công');
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrow) {
                            ShowMessage_Danger('Cập nhật loại ' + sLoai + ' thất bại');
                        },
                        complete: function () {
                            $('#modalTypeWork').modal('hide');
                        }
                    })
                }
            }
        });
    }

    self.DeleteWork_Modal = function () {
        var objUp = {
            ID: self.newCongViec().ID(),
            Ma_TieuDe: self.newCongViec().Ma_TieuDe(),
            PhanLoai: self.newCongViec().PhanLoai(),
            ID_LoaiTuVan: self.newCongViec().ID_LoaiTuVan(),
            ID_KhachHang: self.newCongViec().ID_KhachHang(),
            ID_NhanVien: self.newCongViec().ID_NhanVien(),
            NgayGio: self.newCongViec().NgayGio(),
            NgayGioKetThuc: self.newCongViec().NgayGioKetThuc(),
            KieuNhacNho: self.newCongViec().KieuNhacNho(),
            NhacNho: self.newCongViec().NhacNho(),
            GhiChu: self.newCongViec().GhiChu(),
            KetQua: self.newCongViec().KetQua(),
            CaNgay: self.newCongViec().CaNgay(),
            NhacTruocLienHeLai: self.newCongViec().NhacTruocLienHeLai(),
            TrangThai: self.newCongViec().TrangThai(),
            MucDoUuTien: self.newCongViec().MucDoUuTien(),
            KieuLap: self.newCongViec().KieuLap(),
            SoLanLap: self.newCongViec().SoLanLap(),
            GiaTriLap: self.newCongViec().GiaTriLap(),
            TuanLap: self.newCongViec().TuanLap(),
            TrangThaiKetThuc: self.newCongViec().TrangThaiKetThuc(),
            GiaTriKetThuc: self.newCongViec().GiaTriKetThuc(),
            ExistDB: self.newCongViec().ExistDB(),
            ID_Parent: self.newCongViec().ID_Parent(),
        }
        self.DeleteWork(objUp, self.newCongViec().PhanLoai());
    }

    self.DeleteWork = function (item, sLoai) {
        self.DateFromOld(item.NgayGio);

        let objDiary = {
            ID_NhanVien: _idNhanVien,
            ID_DonVi: _idDonVi,
            ChucNang: 'Công việc - Lịch hẹn',
            LoaiNhatKy: 2,
            NoiDung: "Xóa " + sLoai + ' ' + item.Ma_TieuDe,
            NoiDungChiTiet: "Xóa " + sLoai + ' ' + item.Ma_TieuDe,
        }

        if (item.KieuLap !== 0) {
            // usefd to CheckTypeUpdate
            self.DateFrom(moment(item.NgayGio).format('DD/MM/YYY'));
            let gtrikt = item.GiaTriKetThuc;
            if (gtrikt.split(' ').length > 1) {// check yyyy-mm-dd hh:mm
                gtrikt = gtrikt.split(' ')[0];
            }
            self.GiaTriKetThuc_1(moment(gtrikt, 'YYYY-MM-DD').format('DD/MM/YYY'));

            var typeUpdate = CheckTypeUpdate(item, item);
            self.Confirm_UpdateEvent(typeUpdate, function () {
                let myData = {};
                item.ID_LoaiTuVan = item.ID_LoaiTuVan === const_GuidEmpty ? null : item.ID_LoaiTuVan;
                myData.objCongViec = item;
                myData.objCongViec.TrangThai = '3';
                myData.objCongViec.ID_DonVi = _idDonVi;

                switch (parseInt(self.TypeUpdate())) {
                    case 1: // only this event
                        // keep event old, & add new Row
                        myData.objCongViec.ID_Parent = item.ID_Parent;
                        myData.objCongViec.NgayCu = self.DateFromOld();
                        objDiary.NoiDungChiTiet = objDiary.NoiDungChiTiet.concat('<br/ > - Chỉnh sửa sự kiện lặp: chỉ sự kiện này');

                        if (item.ExistDB) {
                            if (item.ID_Parent === item.ID) {
                                partialWork.AddEventDB(myData, sLoai, objDiary);
                            }
                            else {
                                // update
                                let sql2 = " TrangThai='3' ".concat(", NguoiSua='", userLogin, "'");
                                self.UpdateEvent_StartEnd(item.ID, sql2);
                            }
                        }
                        else {
                            self.AddEventDB(myData, sLoai, objDiary);
                        }

                        break;
                    case 2: // this event & all after
                        let sql = " TrangThaiKetThuc= 2".concat(" , GiaTriKetThuc='", moment(new Date(item.NgayGio)).format('YYYY-MM-DD'), "', NguoiSua='", userLogin, "'");
                        self.UpdateEvent_StartEnd(item.ID_Parent, sql);

                        // add new event
                        myData.objCongViec.NgayGioKetThuc = moment(new Date(item.NgayGio)).add(1, 'hours').format('YYYY-MM-DD HH:mm');
                        objDiary.NoiDungChiTiet = objDiary.NoiDungChiTiet.concat('<br/ > - Chỉnh sửa sự kiện lặp: sự kiện này và các sự kiện sau');

                        if (item.ExistDB) {
                            self.UpdateEventDB(myData, sLoai, objDiary);
                        }
                        else {
                            self.AddEventDB(myData, sLoai, objDiary);
                        }
                        break;
                    case 3: // all event (only update event old)
                        objDiary.NoiDungChiTiet = objDiary.NoiDungChiTiet.concat('<br/ > - Chỉnh sửa sự kiện lặp: tất cả các sự kiện');
                        let sql4 = " TrangThai= '3'".concat(", NguoiSua='", userLogin, "'");
                        self.UpdateEvent_StartEnd(item.ID_Parent, sql4);
                        Insert_NhatKyThaoTac_1Param(objDiary);
                        ShowMessage_Success('Xóa ' + sLoai + ' thành công');
                        break;

                }
                $("#modal_QuestionUpdateEvent").modal('hide');
                $("#modalPopuplg_Work").modal('hide');
            })
        }
        else {
            let sql3 = " TrangThai='3' ".concat(", NguoiSua='", userLogin, "'");
            self.UpdateEvent_StartEnd(item.ID, sql3);
            Insert_NhatKyThaoTac_1Param(objDiary);
            ShowMessage_Success('Xóa ' + sLoai + ' thành công');
            $('#modalPopuplg_Work').modal('hide');
        }
    }

    self.showModalAddLoaiCV = function () {
        $('#modalTypeWork').modal('show');
        if (self.IsAddTypeWork()) {
            $('#titleTypeWork').text('Thêm mới loại công việc');
            self.newLoaiCongViec(new FormModel_NewLoaiCongViec());
        }
        else {
            $('#titleTypeWork').text('Cập nhật loại công việc');

            var itemEx = $.grep(self.LoaiCongViecs(), function (x) {
                return x.ID === self.newCongViec().ID_LoaiTuVan(); /*self.selectedLoaiCV();*/
            });
            if (itemEx.length > 0) {
                self.newLoaiCongViec().setdata(itemEx[0]);
            }
        }
    }

    self.arrFilterLoaiCongViec = ko.computed(function () {
        var _filter = self.filterLoaiCongViec();

        if (_filter) {
            _filter = locdau(_filter);
        }

        return arrFilter = ko.utils.arrayFilter(self.ArrLoaiCongViec_byPhanLoai(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenLoaiTuVanLichHen).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenLoaiTuVanLichHen).indexOf(_filter) >= 0 ||
                    sSearch.indexOf(_filter) >= 0
                );
            }
            return chon;
        });
    });

    self.arrFilterNhanVien = ko.computed(function () {
        var _filter = self.filterNVPhuTrach();
        if (_filter) {
            _filter = locdau(_filter);
        }

        var arrReturn = arrFilter = ko.utils.arrayFilter(self.NhanViens(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenNhanVien).split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = locdau(prod.TenNhanVien).indexOf(_filter) >= 0 ||
                    sSearch.indexOf(_filter) >= 0 || locdau(prod.MaNhanVien).indexOf(_filter) >= 0;
            }
            return chon;
        });
        return arrReturn.splice(0, 20);
    });

    self.ChoseTypeWork = function (item) {
        self.filterLoaiCongViec('');
        $('#lstLoaiCongViec span').each(function () {
            $(this).empty();
        });

        if (item.ID == undefined) {
            $(txtLoaiCongViecModal).text('--- Chọn loại công việc ---');
            $('#btnDeleteTyepWork').hide();
            self.IsAddTypeWork(true);
            self.newCongViec().ID_LoaiTuVan(undefined);
        }
        else {
            $(txtLoaiCongViecModal).text(item.TenLoaiTuVanLichHen);
            self.IsAddTypeWork(false);
            self.newCongViec().ID_LoaiTuVan(item.ID);
            $('i[id=spanCheckLoaiCongViec_' + item.ID + ']').addClass('fa fa-check');
            $('i[id=spanCheckLoaiCongViec_' + item.ID + ']').parent().parent().siblings().find("i.fa").removeClass('fa-check')
        }
    }

    self.ChoseStaffInCharge = function (item) {
        self.filterNVPhuTrach('');

        $('#lstNhanVien span').each(function () {
            $(this).empty();
        });

        if (item.ID == undefined) {
            $(txtStaffInchargeModal).text('--- Chọn nhân viên ---');
            self.newCongViec().ID_NhanVien(undefined);
        }
        else {
            $(txtStaffInchargeModal).text(item.TenNhanVien);
            self.newCongViec().ID_NhanVien(item.ID);
            $('i[id=spanCheckNhanVien_' + item.ID + ']').addClass('fa fa-check');
            $('i[id=spanCheckNhanVien_' + item.ID + ']').parent().parent().siblings().find("i.fa").removeClass('fa-check')
        }
    }

    // not use
    self.CloseNV = function (item) {
        self.ListStaffShare.remove(item);
        if (self.ListStaffShare().length === 0) {
            $('#choose_NhanVien').append('<input type="text" style="background:#f0f1f1" id="dllNhanVien" readonly="readonly" class="dropdown" placeholder="Chọn nhân viên">');
        }
        $('#selec-all-NhanVien li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }
    // not use
    self.ChoseStaffShare = function (item) {
        if (self.newCongViec().ID_NhanVien() !== item.ID) {
            var arrLCV = [];
            for (var i = 0; i < self.ListStaffShare().length; i++) {
                if ($.inArray(self.ListStaffShare()[i], arrLCV) === -1) {
                    arrLCV.push(self.ListStaffShare()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrLCV) === -1) {
                self.ListStaffShare.push(item);
            }
            $('#selec-all-NhanVien li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
            $('#choose_NhanVien input').remove();
        }
        else {
            ShowMessage_Danger("Nhân viên phối hợp không được trùng nhân viên phụ trách.");
            return false;
        }
    }
    // not use
    self.ChoseFileUpload = function (elemet, event) {

        var files = event.target.files;// FileList object
        // Loop through the FileList and render image files as thumbnails.
        for (var i = 0; i < files.length; i++) {
            var f = files[i];
            // Only process image files.
            var size = parseFloat(f.size / 1024).toFixed(2);
            $('.errorAnh').text("");
            $('.errorAnhHH').text("");
            if (size > 2048) {
                $('.errorAnh').text('Dung lượng file không được lớn quá 2Mb');
                $('.errorAnhHH').text('Dung lượng file không được lớn quá 2Mb');
            }
            if (size <= 2048) {
                var reader = new FileReader();
                // Closure to capture the file information.
                self.FileSelect([]);
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.FileSelect.push(new FileModel(theFile, e.target.result));
                        //$('#txtTenFile').html(theFile.name);
                        self.newCongViec().FileDinhKem(theFile.name);
                        $('#clickXoaFile').show();
                        $('#workfile').show();
                    };
                })(f);
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }
    };
    // not use
    self.DeleteFile = function () {
        self.FileSelect([]);
        self.newCongViec().FileDinhKem('');
        $('#clickXoaFile').hide();
        $('#workfile').hide();
    };
    // not use
    self.UpLoadFile = function (id) {
        var i = 0;
        if (i < self.FileSelect().length) {
            var formData = new FormData();
            formData.append("fileWork", self.FileSelect()[i].file);
            $.ajax({
                type: "POST",
                url: '/api/DanhMuc/DM_HangHoaAPI/' + "UpLoadFileCongViec/" + id,
                data: formData,
                dataType: 'json',
                contentType: false,
                processData: false,
                success: function (response) {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
        else {
            ajaxHelper(ChamSocKhachHangUri + 'UpdateCongViecXoaFile?idcv=' + id, 'GET').done(function (data) {
            });
        }
    }

    self.CheckCaNgay = function () {
        var $this = event.currentTarget;
        var isCheck = $($this).is(':checked');
        self.newCongViec().CaNgay(isCheck);
        //$('#congviec').find('.timepicker').prop('disabled', self.newCongViec().CaNgay());
    }

    $('.work-change-kh-ncc').on('click', function () {
        $(this).closest('label').find('span').each(function () {
            $(this).toggle();
        });

        $('#calendar_txtKH').val('');
        self.newCongViec().ID_KhachHang(undefined);
        if (self.IsCustomer()) {
            self.IsCustomer(false);
            modelLoaiDT.LoaiDoiTuong(2);
            $('#calendar_txtKH').attr('placeholder', 'Tìm kiếm nhà cung cấp');
        }
        else {
            self.IsCustomer(true);
            modelLoaiDT.LoaiDoiTuong(1);
            $('#calendar_txtKH').attr('placeholder', 'Tìm kiếm khách hàng');
        }
    });
}

$('.datepicker').datetimepicker({
    format: 'd/m/Y',
    mask: true,
    timepicker: false,
});
$('.timepicker').datetimepicker({
    format: 'H:i',
    datepicker: false,
    step: 15
});
$('.datetimepicker').datetimepicker({
    format: 'd/m/Y H:i',
    mask: true,
    timepicker: true,
});

function jqAutoSelectItem(item) {
    partialWork.JqAutoSelectItem(item);
}
