var Key_Form = "Key_SMS";
$('#selected-column').on('click', '.dropdown-list ul li input[type = checkbox]', function (i) {
    var valueCheck = $(this).val();
    LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, i);
    $('.' + valueCheck).toggle();
});
var Model_NewSMS = function () {
    var self = this;
    self.ID = ko.observable();
    self.ID_NguoiGui = ko.observable();
    self.ID_KhachHang = ko.observable();
    self.ID_DonVi = ko.observable();
    self.SoDienThoai = ko.observable();
    self.SoTinGui = ko.observable();
    self.NoiDung = ko.observable();
    self.ThoiGianGui = ko.observable();
    self.TrangThai = ko.observable();
    self.GiaTien = ko.observable();
    self.LoaiTinNhan = ko.observable();
    self.setdata = function (item) {
        self.ID(item.ID);
        self.ID_NguoiGui(item.ID_NguoiGui);
        self.ID_KhachHang(item.ID_KhachHang);
        self.ID_DonVi(item.ID_DonVi);
        self.SoDienThoai(item.SoDienThoai);
        self.SoTinGui(item.SoTinGui);
        self.NoiDung(item.NoiDung);
        self.ThoiGianGui(item.ThoiGianGui);
        self.TrangThai(item.TrangThai);
        self.GiaTien(item.GiaTien);
        self.LoaiTinNhan(item.LoaiTinNhan);
    };
};
var ViewModel = function () {
    var self = this;
    var idNhanVien = $('.idnhanvien').text();
    var _IDNguoiDung = $('.idnguoidung').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    var ThietLapAPI = '/api/DanhMuc/ThietLapApi/';
    var CSKHUri = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DMNhomDoiTuongUri = '/api/DanhMuc/DM_NhomDoiTuongAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';

    self.error = ko.observable();
    self.TypeSMS = ko.observable(0);
    self.ArrKhachHangCoSDT = ko.observableArray();
    self.ArrKhachHangCoSDTSN = ko.observableArray();
    self.ArrKhachHangCoSDTGD = ko.observableArray();
    self.ListIDNhanVienQuyen = ko.observableArray();
    self.customerBirthday = ko.observable('0');
    self.customerBirthday_Quy = ko.observable('5'); // loc ngay sinh theo thang, quy, nam
    self.customerBirthday_Input = ko.observable(); // loc ngay sinh from date --> to date
    self.NhomDoiTuongs = ko.observableArray();
    self.Loc_TrangThaiGui = ko.observable('0');
    self.Loc_LoaiTin = ko.observable('0');
    self.MangKhachHangGuiTin = ko.observableArray();
    self.MangKhachHangGuiTinSN = ko.observableArray();
    self.MangKhachHangGuiTinGD = ko.observableArray();
    self.MangKhachHangSaveDB = ko.observableArray();
    self.ListAllDoiTuong = ko.observableArray();
    self.LoaiTinNhanGui = ko.observable();
    self.NhomKhachHangChosed = ko.observableArray();
    self.JqAutoSelectKH = ko.observable();
    self.BrandNames = ko.observableArray();
    self.SMSMaus = ko.observableArray();
    self.SMSMauTG = ko.observableArray();
    self.SoDuTaiKhoan = ko.observable(0);
    self.ChiNhanhs = ko.observableArray();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();
    self.selectNhomDT = ko.observable();
    self.PageList_DisplayGD = ko.observableArray();
    self.PageList_DisplayKH = ko.observableArray();
    self.LoaiCongViecs = ko.observableArray();
    self.MangNhomCongViec = ko.observableArray();
    self.CongViecs = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.LichHen_byDate = ko.observableArray();
    self.Popup_LichHenChosed = ko.observableArray();
    self.numtext = ko.observable(160)

    function PageLoad() {
        loadHtmlGrid();
        GetListIDNhanVien_byUserLogin();
        GetSoDuTaiKhoan();
        getListNhomDT();
        getAllChiNhanh();
        GetAllSMSMau();
        getAllBrandName();
        SearchTinNhan();
        GetCauHinhHeThong();
        GetListTypeWork();
        GetListStaff_Working_byChiNhanh();
    }

    function GetListTypeWork() {
        ajaxHelper('/api/DanhMuc/DM_LoaiTuVanLichHenAPI/' + 'GetDM_LoaiCongViec', 'GET').done(function (x) {
            if (x.res === true) {
                self.LoaiCongViecs(x.data);
            }
        });
    };

    function GetListStaff_Working_byChiNhanh() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.NhanViens(data);
            }
        });
    };

    function SMS_LichHen() {
        var param = GetParamSearch();
        var arrLoaiCV = [];
        if (self.MangNhomCongViec().length === 0) {
            arrLoaiCV = self.LoaiCongViecs().map(function (i, e) {
                return i.ID;
            });
            arrLoaiCV.push(const_GuidEmpty);
        }
        else {
            for (let i = 0; i < self.MangNhomCongViec().length; i++) {
                if ($.inArray(self.MangNhomCongViec()[i], arrLoaiCV) === -1) {
                    arrLoaiCV.push(self.MangNhomCongViec()[i].ID);
                }
            }
        }

        let arrNV = self.NhanViens().map(function (i, e) {
            return i.ID;
        });
        arrNV.push(const_GuidEmpty);

        var model = {
            ID_DonVis: param.ArrIDDonVi,
            IDLoaiTuVans: arrLoaiCV,
            IDNhanVienPhuTrachs: arrNV,
            TrangThaiCVs: [1],// dangxuly,
            MucDoUuTien: '%%',//all
            LoaiDoiTuong: 1,
            PhanLoai: '%%',// lichhen + cv
            FromDate: param.DateFrom,
            ToDate: param.DateTo,
            TextSearch: '',
            CurrentPage: self.currentPageKH(),
            PageSize: self.pageSizeKH(),
            TypeShow: 1,
            ID_KhachHang: '%%',
            IDNhomKH: param.ID_NhomKH === null ? '' : param.ID_NhomKH,
        }
        console.log(model)
        $('#smslichhen').gridLoader();
        ajaxHelper(DMDoiTuongUri + 'SMS_LichHen?status=' + parseInt(self.Loc_TrangThaiGui()), 'POST', model).done(function (x) {
            $('#smslichhen').gridLoader({ show: false });
            console.log(x)
            if (x.res === true) {
                self.CongViecs(x.data);
                self.TotalRow_LichHen(x.TotalRow);
                self.TotalPage_LichHen(x.TotalPage);
                self.SetCheck_Input();

                loadHtmlGrid();
            }
        });
    };

    function SearchLichHen_Popup() {
        var arrIDDV = [];
        for (var j = 0; j < self.MangNhomDV().length; j++) {
            if ($.inArray(self.MangNhomDV()[j].ID, arrIDDV) === -1) {
                arrIDDV.push(self.MangNhomDV()[j].ID);
            }
        }

        var arrLoaiCV = self.LoaiCongViecs().map(function (i, e) {
            return i.ID;
        });
        arrLoaiCV.push(const_GuidEmpty);
        let arrNV = self.NhanViens().map(function (i, e) {
            return i.ID;
        });
        arrNV.push(const_GuidEmpty);

        var date = $('#txtNgayDatLich').val();
        var from = moment(date, 'DD/MM/YYYY').format('YYYY-MM-DD');
        var to = moment(date, 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');

        var model = {
            ID_DonVis: arrIDDV,
            IDLoaiTuVans: arrLoaiCV,
            IDNhanVienPhuTrachs: arrNV,
            TrangThaiCVs: [1],// dangxuly,
            MucDoUuTien: '%%',//all
            LoaiDoiTuong: 1,
            PhanLoai: '%%',// lichhen + cv
            FromDate: from,
            ToDate: to,
            TextSearch: '',
            CurrentPage: 0,
            PageSize: 100,
            TypeShow: 1,
            ID_KhachHang: '%%',
            IDNhomKH: '',
        }
        ajaxHelper(DMDoiTuongUri + 'SMS_LichHen?status=' + parseInt(self.Loc_TrangThaiGui()), 'POST', model).done(function (x) {
            self.Popup_LichHenChosed([]);
            if (x.res === true) {
                self.LichHen_byDate(x.data);
                console.log(2,x.data)
            }
        });
    }

    $('#txtNgayDatLich').on('change', function () {
        SearchLichHen_Popup();
    });

    self.Popup_ChoseAppointment = function (item) {
        if (item.ID !== undefined) {
            var all = $.grep(self.Popup_LichHenChosed(), function (x) {
                return x.ID === 0;
            });
            if (all.length > 0) {
                ShowMessage_Danger('Bạn đã chọn gửi tin cho toàn bộ khách hàng');
                return false;
            }
            var arrDT = [];
            for (var i = 0; i < self.Popup_LichHenChosed().length; i++) {
                if ($.inArray(self.Popup_LichHenChosed()[i], arrDT) === -1) {
                    arrDT.push(self.Popup_LichHenChosed()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrDT) === -1) {
                self.Popup_LichHenChosed.push(item);
            }
            $('#ddlAppointemnt li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
        }
        else {
            self.Popup_LichHenChosed([{ ID: 0, TenDoiTuong: 'Toàn bộ khách hàng' }]);
        }
    };

    self.Popup_RemoveAppointment = function (item) {
        self.Popup_LichHenChosed.remove(item);
        $('#ddlAppointemnt li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    };

    self.ListRowChosed = ko.observableArray();

    self.ChangeCheck_Header = function () {
        var obj = $(event.currentTarget);
        var isChecked = obj.is(':checked');
        var tbl = obj.closest('table');
        var arrID = [];
        arrIDDoiTuong = [];

        $(tbl).find('tbody .check-group input').each(function () {
            $(this).prop('checked', isChecked);
        });

        for (let i = 0; i < self.PageResults_LichHen().length; i++) {
            arrID.push(self.PageResults_LichHen()[i].ID);
        }

        if (isChecked) {
            switch (self.TypeSMS()) {
                case 3:// lichhen
                    for (let i = 0; i < self.PageResults_LichHen().length; i++) {
                        self.ListRowChosed.push(self.PageResults_LichHen()[i]);
                    }
                    let arrUnique = self.ListRowChosed().sort(function (a, b) {
                        let x = a.ID, y = b.ID;
                        return x > y ? 1 : x < y ? -1 : 0;
                    });
                    arrUnique = $.unique(arrUnique);
                    self.ListRowChosed(arrUnique);
                    break;
            }
        }
        else {
            let arrAfter = [];
            switch (self.TypeSMS()) {
                case 3:// lichhen
                    arrAfter = $.grep(self.ListRowChosed(), function (x) {
                        return $.inArray(x.ID, arrID) === -1;
                    });
                    self.ListRowChosed(arrAfter);
                    break;
            }
        }

        for (let i = 0; i < self.ListRowChosed().length; i++) {
            if ($.inArray(self.ListRowChosed()[i].ID_KhachHang, arrIDDoiTuong) === -1) {
                arrIDDoiTuong.push(self.ListRowChosed()[i].ID_KhachHang);
            }
        }
        HideShowThaoTac();
    }

    self.ChangeCheck_Row = function (item) {
        var obj = $(event.currentTarget);
        var isChecked = obj.is(':checked');
        var tbl = obj.closest('table');
        arrIDDoiTuong = [];

        if (isChecked) {
            self.ListRowChosed.push(item);
        }
        else {
            let arrAfter = $.grep(self.ListRowChosed(), function (x) {
                return x.ID !== item.ID;
            });
            self.ListRowChosed(arrAfter);
        }
        for (let i = 0; i < self.ListRowChosed().length; i++) {
            if ($.inArray(self.ListRowChosed()[i].ID_KhachHang, arrIDDoiTuong) === -1) {
                arrIDDoiTuong.push(self.ListRowChosed()[i].ID_KhachHang);
            }
        }
        HideShowThaoTac();
    }

    self.SetCheck_Input = function () {
        var arrID = [];
        for (let i = 0; i < self.ListRowChosed().length; i++) {
            arrID.push(self.ListRowChosed()[i].ID);
        }
        switch (self.TypeSMS()) {
            case 3:
                $('#tbllichhen .check-group input').each(function () {
                    $(this).prop('checked', $.inArray($(this).attr('id'), arrID) > -1);
                });
                // set check hedear
                let count = 0;
                for (let i = 0; i < self.PageResults_LichHen().length; i++) {
                    if ($.inArray(self.PageResults_LichHen()[i].ID, arrID) > -1) {
                        count += 1;
                    }
                }
                $('#tbllichhen .ckHeader').prop('checked', count === self.PageResults_LichHen().length)
                break;
        }
        HideShowThaoTac();
    }

    function HideShowThaoTac() {
        var len = self.ListRowChosed().length;
        if (len > 0) {
            $('#divThaoTac').show();
            $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        }
        else {
            $('#divThaoTac').hide();
            $('.choose-commodity').hide().trigger("addClassForButtonNew");
        }
        $('#count').text(len);
    }

    // paging lichhen
    self.TotalPage_LichHen = ko.observable(0);
    self.TotalRow_LichHen = ko.observable(0);
    self.PageSizes_LichHen = ko.observableArray([10,20, 30, 50]);
    self.PageSize_LichHen = ko.observable(self.PageSizes_LichHen()[0]);
    self.currentPage_LichHen = ko.observable(0);
    self.fromitem_LichHen = ko.observable(0);
    self.toitem_LichHen = ko.observable(0);

    self.PageResults_LichHen = ko.computed(function () {
        var first = self.currentPage_LichHen() * self.PageSize_LichHen();
        if (self.CongViecs() !== null) {
            return self.CongViecs().slice(first, first + self.PageSize_LichHen());
        }
    });
    self.PageList_LichHen = ko.computed(function () {
        var arrPage = [];
        var allPage = self.TotalPage_LichHen();
        var currentPage = self.currentPage_LichHen();
        if (allPage > 4) {
            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage_LichHen()) + 1;
            }
            else {
                i = self.currentPage_LichHen();
            }
            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                    else {
                        // get currentPage - 2 , currentPage, currentPage + 2
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumber: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        if (self.PageResults_LichHen().length > 0) {
            self.fromitem_LichHen((self.currentPage_LichHen() * self.PageSize_LichHen()) + 1);
            if (((self.currentPage_LichHen() + 1) * self.PageSize_LichHen()) > self.PageResults_LichHen().length) {
                var ss = (self.currentPage_LichHen() + 1) * self.PageSize_LichHen();
                var fromItem = (self.currentPage_LichHen() + 1) * self.PageSize_LichHen();
                if (fromItem < self.TotalRow_LichHen()) {
                    self.toitem_LichHen((self.currentPage_LichHen() + 1) * self.PageSize_LichHen());
                }
                else {
                    self.toitem_LichHen(self.TotalRow_LichHen());
                }
            } else {
                self.toitem_LichHen((self.currentPage_LichHen() * self.PageSize_LichHen()) + self.PageSize_LichHen());
            }
        }

        self.SetCheck_Input();
        return arrPage;
    });
    self.VisibleStartPage_LichHen = ko.computed(function () {
        if (self.PageList_LichHen().length > 0) {
            return self.PageList_LichHen()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPage_LichHen = ko.computed(function () {
        if (self.PageList_LichHen().length > 0) {
            return self.PageList_LichHen()[self.PageList_LichHen().length - 1].pageNumber !== self.TotalPage_LichHen();
        }
    })
    self.ResetCurrentPage_LichHen = function () {
        self.currentPage_LichHen(0);
    };
    self.GoToPage_LichHen = function (page) {
        self.currentPage_LichHen(page.pageNumber - 1);
    };
    self.GetClass_LichHen = function (page) {
        return ((page.pageNumber - 1) === self.currentPage_LichHen()) ? "click" : "";
    };
    self.StartPage_LichHen = function () {
        self.currentPage_LichHen(0);
    }
    self.BackPage_LichHen = function () {
        if (self.currentPage_LichHen() > 1) {
            self.currentPage_LichHen(self.currentPage_LichHen() - 1);
        }
    }
    self.GoToNextPage_LichHen = function () {
        if (self.currentPage_LichHen() < self.TotalPage_LichHen() - 1) {
            self.currentPage_LichHen(self.currentPage_LichHen() + 1);
        }
    }
    self.EndPage_LichHen = function () {
        if (self.currentPage_LichHen() < self.TotalPage_LichHen() - 1) {
            self.currentPage_LichHen(self.TotalPage_LichHen() - 1);
        }
    }

    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#selected-column ul li input[type = checkbox]'), []);
    };

    function GetListIDNhanVien_byUserLogin() {
        ajaxHelper(CSKHUri + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien
            + '&idChiNhanh=' + idDonVi + '&funcName=' + "KhachHang", 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);
            });
    }
    self.LoaiTins = ko.observableArray([
        { TenLoai: "Tin giao dịch", ID: "1" },
        { TenLoai: "Tin sinh nhật", ID: "2" },
        { TenLoai: "Tin thường", ID: "3" },
        { TenLoai: "Lịch hẹn", ID: "4" }
    ]);

    function GetSoDuTaiKhoan() {
        ajaxHelper(ThietLapAPI + 'GetSoDuCuaTaiKhoan?idnd=' + _IDNguoiDung, "GET").done(function (data) {
            self.SoDuTaiKhoan(data);
        })
    }
    function GetAllSMSMau() {
        ajaxHelper(ThietLapAPI + 'GetAllMauTin', 'GET').done(function (data) {
            self.SMSMaus(data);
            self.SMSMauTG(data);
        });
    }

    self.selectedLoaiTin = function (item) {
        if (item !== undefined) {
            var today = new Date();
            $('#txtLoaiTin').html(item.TenLoai);

            var id = parseInt(item.ID);
            self.SMSMaus(self.SMSMauTG().filter(p => p.LoaiTin === id));
            self.LoaiTinNhanGui(id);

            switch (id) {
                case 1:// giaodich
                    $('#hideDoiTuongGui').hide();
                    $('#sinhnhatkhachhang, #lichhen').hide();
                    $('#hoadongiaodich').show();
                    $('#btnGuiTinNhanGD').show();
                    $('#btnGuiTinNhan,#btnGuiTinNhanSN').hide();
                    $('#selectedHoaDon li').each(function () {
                        $(this).find('.fa-check').remove();
                    });
                    $('#txtNgaySN').val(moment(today).format('DD/MM/YYYY'));
                    self.MangKhachHangGuiTinGD([]);
                    GetKhachHangGDByNgay();
                    break;
                case 2:// sinhnhat
                    $('#hideDoiTuongGui').hide();
                    $('#sinhnhatkhachhang').show();
                    $('#hoadongiaodich, #lichhen').hide();
                    $('#btnGuiTinNhanSN').show();
                    $('#btnGuiTinNhan,#btnGuiTinNhanGD,#btnGuiTinLichHen').hide();
                    $('#selectedDoiTuongSN li').each(function () {
                        $(this).find('.fa-check').remove();
                    });
                    $('#txtNgaySN').val(moment(today).format('DD/MM/YYYY'));
                    self.MangKhachHangGuiTinSN([]);
                    GetKhachHangSNByNgay();
                    break;
                case 3: //tinthuong
                    $('#hideDoiTuongGui').show();
                    $('#sinhnhatkhachhang,#hoadongiaodich, #lichhen').hide();
                    $('#btnGuiTinNhan').show();
                    $('#btnGuiTinNhanSN,#btnGuiTinNhanGD,#btnGuiTinLichHen').hide();
                    break;
                case 4:
                    $('#hideDoiTuongGui').hide();
                    $('#lichhen').show();
                    $('#sinhnhatkhachhang, #hoadongiaodich').hide();
                    $('#btnGuiTinLichHen').show();
                    $('#btnGuiTinNhan,#btnGuiTinNhanGD, #btnGuiTinNhanSN').hide();
                    break;
            }

            $('#selectedLoaiTin li').each(function () {
                $(this).find('.fa-check').remove();
            });
            $('#selectedLoaiTin li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
        }
    };

    function GetKhachHangSNByNgay() {
        var _ngaySN = $('#txtNgaySN').val();
        var ngaysinh = moment(_ngaySN, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        ajaxHelper(DMDoiTuongUri + 'getKhachHangSNByNgay?ngaysinh=' + ngaysinh, "GET").done(function (data) {
            self.ArrKhachHangCoSDTSN(data);
        });
    }
    function GetKhachHangGDByNgay() {
        var _txtNgayHD = $('#txtNgayHD').val();
        var ngayHD = moment(_txtNgayHD, 'DD/MM/YYYY HH:mm').format('YYYY-MM-DD HH:mm');
        ajaxHelper(DMDoiTuongUri + 'getKhachHangGDByNgay?ngayhd=' + ngayHD, "GET").done(function (data) {
            self.ArrKhachHangCoSDTGD(data);
        });
    }
    $('#txtNgaySN').on('change', function () {
        GetKhachHangSNByNgay();
    });
    $('#txtNgayHD').on('change', function () {
        GetKhachHangGDByNgay();
    });

    function getListNhomDT() {
        ajaxHelper(DMNhomDoiTuongUri + "GetDM_NhomDoiTuong?loaiDoiTuong=" + 1, 'GET').done(function (data) {
            if (data !== '' && data !== null) {
                self.NhomDoiTuongs(data);
            }
            var newObj = {
                ID: const_GuidEmpty,
                TenNhomDoiTuong: 'Nhóm mặc định',
                Text_Search: 'nhom mac dinh nmd'
            };
            self.NhomDoiTuongs.unshift(newObj);
        });
    }

    self.selectNhomDT.subscribe(function (newval) {
        GetListData_byType();
    });
    var $remaining = $('#remaining'),
        $messages = $remaining.next();
    var $remainingSDT = $('#remainingSDT'),
        $messagesSDT = $remainingSDT.next();

    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + idNhanVien, 'GET').done(function (data) {
            data = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(data);
            var obj = {
                ID: idDonVi,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            self.selectedCN(obj);
        });
    }

    self.selectedCN = function (item) {
        var arrDV = [];
        for (var i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrDV) === -1) {
            self.MangNhomDV.push(item);
        }

        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_TenDonVi input').remove();
    }

    self.CloseDV = function (item) {
        self.MangNhomDV.remove(item);
        if (self.MangNhomDV().length === 0) {
            $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown" placeholder="Chọn chi nhánh">');
        }
        getKhachHangGD();
        // remove check
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.ID_HoaDonGiaoDich = ko.observable();

    self.showpopupSMS = function () {
        self.selectedBrandName(undefined);
        self.MangKhachHangGuiTin([]);
        self.NhomKhachHangChosed([]);
        self.NhomKHChosed([]);
        self.Popup_LichHenChosed([]);
        self.selectedMauTin(undefined);
        switch (self.TypeSMS()) {
            case 0:
                self.LoaiTinNhanGui(3);
                var item1 = {
                    TenLoai: "Tin thường",
                    ID: "3"
                };
                break;
            case 1:
                self.LoaiTinNhanGui(2);
                var item1 = {
                    TenLoai: "Tin sinh nhật",
                    ID: "2"
                };
                break;
            case 2:
                self.LoaiTinNhanGui(1);
                var item1 = {
                    TenLoai: "Tin giao dịch",
                    ID: "1"
                };
                break;
            case 3:
                self.LoaiTinNhanGui(4);
                var item1 = {
                    TenLoai: "Lịch hẹn",
                    ID: "4"
                };
                break;
        }

        self.selectedLoaiTin(item1);
        $('#exampleModal').modal('show');
        $('#dllCongViec').val("");
        $('#selectedDoiTuong li').each(function () {
            $(this).find('.fa-check').remove();
        });
        $('#exampleModal .form-modal-send-sms').gridLoader({ show: false });
    };
    self.showpopupSMS_Many = function () {
        if (arrIDDoiTuong.length > 0) {
            let item1 = [];
            switch (self.TypeSMS()) {
                case 1:
                    self.LoaiTinNhanGui(2);
                    item1 = {
                        TenLoai: "Tin sinh nhật",
                        ID: "2"
                    };
                    break;
                case 3:
                    //$('#')
                    self.LoaiTinNhanGui(4);
                    item1 = {
                        TenLoai: "Lịch hẹn",
                        ID: "4"
                    };
                    break;
            }
            self.selectedLoaiTin(item1);
            // mượn tạm object để truyền tham số
            var obj = {
                ID_NhanVienQuanLys: arrIDDoiTuong,
                CurrentPage: 0,
                PageSize: 0,
                TrangThai: 0,
            }
            // get all KH had chosed by id
            ajaxHelper(DMDoiTuongUri + 'GetListCustomer_byIDs', 'POST', obj).done(function (x) {
             
                if (x.res === true) {
                    switch (self.TypeSMS()) {
                        case 1:
                            self.MangKhachHangGuiTinSN(x.data);
                            break;
                        case 3:
                            self.Popup_LichHenChosed(x.data);
                            break;
                    }
                   
                }
            })
            $('#exampleModal').modal('show');
        }
    };
    self.showpopupGuiTinSN = function (item) {
        self.selectedBrandNameSDT(undefined);
        self.selectedMauTinSDT(undefined);
        self.LoaiTinNhanGui(2);
        //var item1 = {
        //    TenLoai: "Tin sinh nhật",
        //    ID: "2"
        //};
        //self.selectedLoaiTin(item1);
        $('#txtSoDienThoai').val(item.DienThoai);
        $('#exampleModalSDT').modal('show');
        //$('#exampleModal').on('shown.bs.modal', function () {
        //    $("#txtLoaiTin").prop('disabled', true);
        //});
    };
    self.showpopupGuiTinGD = function (item) {
        self.selectedBrandNameSDT(undefined);
        self.selectedMauTinSDT(undefined);
        self.LoaiTinNhanGui(1);
        self.ID_HoaDonGiaoDich(item.ID_HoaDon);
        $('#txtSoDienThoai').val(item.DienThoai);
        $('#exampleModalSDT').modal('show');
        //$('#exampleModal').on('shown.bs.modal', function () {
        //    $("#txtLoaiTin").prop('disabled', true);
        //});
    };
    self.showpopupGuiTinTuSMS = function (item) {
        self.selectedBrandNameSDT(undefined);
        self.selectedMauTinSDT(undefined);
        $('#txtSoDienThoai').val(item.SoDienThoai);
        $('#exampleModalSDT').modal('show');
    };
    function getAllBrandName() {
        ajaxHelper(ThietLapAPI + 'GetallBrandName', 'GET').done(function (data) {
            data = data.filter(p => p.Status === 1);
            self.BrandNames(data);
        });
    }
    function GetAllKhachHang() {
        ajaxHelper(DMDoiTuongUri + 'GetListKH_InforBasic?idChiNhanh=' + idDonVi + '&loaiDoiTuong=' + 1, 'GET').done(function (data) {
            if (data !== null) {
                self.ListAllDoiTuong(data);
                self.ArrKhachHangCoSDT(data.filter(p => p.DienThoai !== ""));// use send SMS
            }
        });
    }

    //GetAllKhachHang();
    self.IDBrandNameChoose = ko.observable();
    self.GiaTienMotTrangTin = ko.observable();
    self.selectedBrandName = function (item) {
        if (item !== undefined) {
            if (item.ID !== undefined) {
                $('#txtBrandNames').html(item.BrandName);
                self.IDBrandNameChoose(item.ID);
                ajaxHelper(ThietLapAPI + 'GetGiaTienTrenTinNhan?id_brand=' + item.ID, "GET").done(function (data) {
                    self.GiaTienMotTrangTin(data);
                })
                $('#selectedBrandName li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#selectedBrandName li').each(function () {
                    if ($(this).attr('id') === item.ID) {
                        $(this).find('.fa-check').remove();
                        $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                    }
                });
            }
            else {
                self.IDBrandNameChoose(undefined);
                $('#selectedBrandName li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#txtBrandNames').html("Chọn BrandName");
            }
        }
        else {
            self.IDBrandNameChoose(undefined);
            $('#txtBrandNames').html("Chọn BrandName");
            $('#selectedBrandName li').each(function () {
                $(this).find('.fa-check').remove();
            });
        }
    };
    self.selectedBrandNameSDT = function (item) {
        if (item !== undefined) {
            if (item.ID !== undefined) {
                $('#txtBrandNamesSDT').html(item.BrandName);
                self.IDBrandNameChoose(item.ID);
                $('#selectedBrandNameSDT li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#selectedBrandNameSDT li').each(function () {
                    if ($(this).attr('id') === item.ID) {
                        $(this).find('.fa-check').remove();
                        $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                    }
                });
            }
            else {
                self.IDBrandNameChoose(undefined);
                $('#selectedBrandNameSDT li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#txtBrandNamesSDT').html("Chọn BrandName");
            }
        }
        else {
            self.IDBrandNameChoose(undefined);
            $('#txtBrandNamesSDT').html("Chọn BrandName");
            $('#selectedBrandNameSDT li').each(function () {
                $(this).find('.fa-check').remove();
            });
        }
    };
    self.selectedMauTin = function (item) {
        if (item !== undefined) {
            if (item.ID !== undefined) {
                var data = self.SMSMaus().filter(p => p.ID === item.ID);
                $('#txtNoiDungTin').val(data[0].NoiDungTin);
                self.LoaiTinNhanGui(data[0].LoaiTin);
                var chars = data[0].NoiDungTin.length,
                    messages = Math.ceil(chars / self.numtext),
                    remaining = messages > 1 ? (messages - 1) * self.numtext + (chars % ((messages - 1) * self.numtext) || (messages - 1) * self.numtext) : (messages - 1) * self.numtext + (chars % (messages * self.numtext) || messages * self.numtext);
                var totalmes = messages * self.numtext;
                $remaining.text(remaining + '/' + totalmes);
                $messages.text(' (' + messages + ' tin nhắn)');
                if (item.NoiDungTin.length > 100) {
                    var tr = item.NoiDungTin.substr(0, 105);
                    var mangTr = tr.split(" ");
                    var chuoi = mangTr[0];
                    for (var j = 1; j < mangTr.length - 1; j++) {
                        chuoi = chuoi + " " + mangTr[j];
                    }
                    item.NoiDungTin = chuoi + "...";
                    $('#txtMauTinChoose').html(item.NoiDungTin);
                }
                else {
                    $('#txtMauTinChoose').html(item.NoiDungTin);
                }
                $('#ChooseMauTin li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#ChooseMauTin li').each(function () {
                    if ($(this).attr('id') === item.ID) {
                        $(this).find('.fa-check').remove();
                        $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                    }
                });
            }
            else {
                $('#txtMauTinChoose').html("Chọn mẫu tin");
                $('#txtNoiDungTin').val("");
                $remaining.text("");
                $messages.text("");
                $('#ChooseMauTin li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
        }
        else {
            $('#txtMauTinChoose').html("Chọn mẫu tin");
            $('#txtNoiDungTin').val("");
            $remaining.text("");
            $messages.text("");
            $('#ChooseMauTin li').each(function () {
                $(this).find('.fa-check').remove();
            });
        }
    };
    self.selectedMauTinSDT = function (item) {
        if (item !== undefined) {
            if (item.ID !== undefined) {
                var data = self.SMSMaus().filter(p => p.ID === item.ID);
                $('#txtNoiDungTinSDT').val(data[0].NoiDungTin);
                self.LoaiTinNhanGui(data[0].LoaiTin);
                var chars = data[0].NoiDungTin.length,
                    messages = Math.ceil(chars / self.numtext),
                    remaining = messages > 1 ? (messages - 1) * self.numtext + (chars % ((messages - 1) * self.numtext) || (messages - 1) * self.numtext) : (messages - 1) * self.numtext + (chars % (messages * self.numtext) || messages * self.numtext);
                var totalmes = messages * self.numtext;
                $remainingSDT.text(remaining + '/' + totalmes);
                $messagesSDT.text(' (' + messages + ' tin nhắn)');
                if (item.NoiDungTin.length > 100) {
                    var tr = item.NoiDungTin.substr(0, 105);
                    var mangTr = tr.split(" ");
                    var chuoi = mangTr[0];
                    for (var j = 1; j < mangTr.length - 1; j++) {
                        chuoi = chuoi + " " + mangTr[j];
                    }
                    item.NoiDungTin = chuoi + "...";
                    $('#txtMauTinChooseSDT').html(item.NoiDungTin);
                }
                else {
                    $('#txtMauTinChooseSDT').html(item.NoiDungTin);
                }
                $('#ChooseMauTinSDT li').each(function () {
                    $(this).find('.fa-check').remove();
                });
                $('#ChooseMauTinSDT li').each(function () {
                    if ($(this).attr('id') === item.ID) {
                        $(this).find('.fa-check').remove();
                        $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                    }
                });
            }
            else {
                $('#txtMauTinChooseSDT').html("Chọn mẫu tin");
                $('#txtNoiDungTin').val("");
                $remainingSDT.text("");
                $messagesSDT.text("");
                $('#ChooseMauTinSDT li').each(function () {
                    $(this).find('.fa-check').remove();
                });
            }
        }
        else {
            $('#txtMauTinChooseSDT').html("Chọn mẫu tin");
            $('#txtNoiDungTinSDT').val("");
            $remainingSDT.text("");
            $messagesSDT.text("");
            $('#ChooseMauTinSDT li').each(function () {
                $(this).find('.fa-check').remove();
            });
        }
    };
    self.ChooseDoiTuongGuiTin = function (item) {
        if (item.ID !== undefined) {
            var arrDT = [];
            for (var i = 0; i < self.MangKhachHangGuiTin().length; i++) {
                if ($.inArray(self.MangKhachHangGuiTin()[i], arrDT) === -1) {
                    arrDT.push(self.MangKhachHangGuiTin()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrDT) === -1) {
                self.MangKhachHangGuiTin.push(item);
            }
            $('#selectedDoiTuong li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
            $('#dllCongViec').val("");
        }
        else {
            var objadd = {
                TenDoiTuong: "Toàn bộ khách hàng",
                DienThoai: "999999999999999"
            };
            self.MangKhachHangGuiTin.push(objadd);
            $('#selectedAllDT li').each(function () {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>');
            });
        }
    };
    self.CloseDoiTuongGuiTin = function (item) {
        self.MangKhachHangGuiTin.remove(item);
        $('#selectedDoiTuong li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
        if (item.DienThoai === "999999999999999") {
            $('#selectedAllDT li').each(function () {
                $(this).find('.fa-check').remove();
            });
        }
    };
    self.CloseNhomDoiTuongGuiTin = function (item) {
        self.NhomKhachHangChosed.remove(item);
    };
    self.ChooseDoiTuongGuiTinSN = function (item) {
        if (item.ID !== undefined) {
            var all = $.grep(self.MangKhachHangGuiTinSN(), function (x) {
                return x.ID === 0;
            });
            if (all.length > 0) {
                ShowMessage_Danger('Bạn đã chọn gửi tin cho toàn bộ khách hàng');
                return false;
            }
            var arrDT = [];
            for (var i = 0; i < self.MangKhachHangGuiTinSN().length; i++) {
                if ($.inArray(self.MangKhachHangGuiTinSN()[i], arrDT) === -1) {
                    arrDT.push(self.MangKhachHangGuiTinSN()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrDT) === -1) {
                self.MangKhachHangGuiTinSN.push(item);
            }
            $('#selectedDoiTuongSN li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
        }
        else {
            self.MangKhachHangGuiTinSN([{ ID: 0, TenDoiTuong: 'Toàn bộ khách hàng' }]);
        }
    };
    self.CloseDoiTuongGuiTinSN = function (item) {
        self.MangKhachHangGuiTinSN.remove(item);
        $('#selectedDoiTuongSN li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    };
    self.ChooseHoaDon = function (item) {
        if (item.ID !== undefined) {
            var arrDT = [];
            for (var i = 0; i < self.MangKhachHangGuiTinGD().length; i++) {
                if ($.inArray(self.MangKhachHangGuiTinGD()[i], arrDT) === -1) {
                    arrDT.push(self.MangKhachHangGuiTinGD()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrDT) === -1) {
                self.MangKhachHangGuiTinGD.push(item);
            }
            $('#selectedHoaDon li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
        }
    };
    self.closeHoaDon = function (item) {
        self.MangKhachHangGuiTinGD.remove(item);
        $('#selectedHoaDon li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    };
    self.ArrMangForSearch = ko.observableArray();
    //$('.outselectmodalsms .input-khach-hang').on('keyup', function (e) {
    //    var txtSearch = $(this).val();
    //    if ($(this).val() !== '' && $(this).val() !== null) {
    //        $(this).closest('div').next().show();
    //        var objCT = [];
    //        self.ArrMangForSearch(self.ListAllDoiTuong().filter(p => p.DienThoai !== ""));
    //        for (var i = 0; i < self.ArrMangForSearch().length; i++) {
    //            var sSearch = '';
    //            var arr = locdau(self.ArrMangForSearch()[i].TenDoiTuong).toLowerCase().split(/\s+/);
    //            for (var j = 0; j < arr.length; j++) {
    //                sSearch += arr[j].toString().split('')[0];
    //            }
    //            var locdauTenDT = locdau(self.ArrMangForSearch()[i].TenDoiTuong).toLowerCase();
    //            var locdauSDT = locdau(self.ArrMangForSearch()[i].DienThoai).toLowerCase();
    //            var TDT = locdauTenDT.split(txtSearch);
    //            var SDT = locdauSDT.split(txtSearch);
    //            if (TDT.length > 1 || SDT.length > 1 || locdau(self.ArrMangForSearch()[i].TenDoiTuong).indexOf(locdau(txtSearch)) >= 0 || sSearch.indexOf(locdau(txtSearch)) >= 0) {
    //                objCT.push(self.ArrMangForSearch()[i]);
    //            }
    //        }
    //        self.ArrKhachHangCoSDT(objCT);
    //    }
    //    else {
    //        self.ArrKhachHangCoSDT(self.ListAllDoiTuong().filter(p => p.DienThoai !== ""));
    //    }
    //    if (e.keyCode === 13) {
    //        var specialChars = "aàáạảãâầấậẩẫăằắặẳẵbcdefghijklmnopqrstuvwxyzèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ' '";
    //        var check = function (string) {
    //            for (i = 0; i < specialChars.length; i++) {
    //                if (string.indexOf(specialChars[i]) > -1) {
    //                    return true;
    //                }
    //            }
    //            return false;
    //        };
    //        if (check(txtSearch) === false && txtSearch !== "") {
    //            var obj =
    //                {
    //                    TenDoiTuong: txtSearch,
    //                    DienThoai: txtSearch
    //                };
    //            if (self.MangKhachHangGuiTin().filter(p => p.DienThoai === txtSearch).length === 0) {
    //                self.MangKhachHangGuiTin.push(obj);
    //            }
    //        }
    //        $('#dllCongViec').val("");
    //    }
    //});
    function Enable_BtnGuiTin(idButton) {
        document.getElementById(idButton).disabled = false;
        document.getElementById(idButton).lastChild.data = " Gửi";
        $('#exampleModal .form-modal-send-sms').gridLoader({ show: false });
    }
    $('.outselectmodalsms .input-khach-hang').on('click', function () {
        $(this).closest('div').next().show();
    });
    self.GuiTinNhan = function () {
        document.getElementById("btnGuiTinNhan").disabled = true;
        document.getElementById("btnGuiTinNhan").lastChild.data = " Đang lưu";
        var _noiDungTin = $('#txtNoiDungTin').val();
        var lenghtTinNhan = _noiDungTin.length;
        var _soTinGui = Math.ceil(lenghtTinNhan / self.numtext);
        var _loaiTinNhan = self.LoaiTinNhanGui();
        var _idbrand = self.IDBrandNameChoose();
        if (_idbrand === undefined) {
            ShowMessage_Danger('Vui lòng chọn BrandName gửi tin');
            Enable_BtnGuiTin("btnGuiTinNhan");
            return false;
        }
        if (self.GiaTienMotTrangTin() * _soTinGui > self.SoDuTaiKhoan()) {
            ShowMessage_Danger('Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền');
            Enable_BtnGuiTin("btnGuiTinNhan");
            return false;
        }
        if (_noiDungTin === "" || _noiDungTin === null || _noiDungTin === undefined || _noiDungTin === "undefined") {
            ShowMessage_Danger('Vui lòng nhập nội dung tin nhắn');
            Enable_BtnGuiTin("btnGuiTinNhan");
            return false;
        }
        // get all KhachHang co sdt/ theo nhóm
        let all = $.grep(self.MangKhachHangGuiTin(), function (x) {
            return x.DienThoai === '999999999999999';
        });
        let arrNotnhom = $.grep(self.MangKhachHangGuiTin(), function (x) {
            return x.DienThoai !== '999999999999999';
        });
        if (all.length === 0 && self.NhomKhachHangChosed().length === 0 && self.NhomKHChosed().length === 0 && arrNotnhom.length === 0) {
            ShowMessage_Danger('Vui lòng chọn đối tượng cần gửi tin');
            Enable_BtnGuiTin("btnGuiTinNhan");
            return false;
        }
        var objTin = {
            NoiDung: _noiDungTin,
            SoTinGui: _soTinGui,
            LoaiTinNhan: _loaiTinNhan,
            ID_NguoiGui: _IDNguoiDung,
            ID_DonVi: idDonVi
        };
        $('#exampleModal .form-modal-send-sms').gridLoader();
        if (all.length > 0 || self.NhomKhachHangChosed().length > 0) {
            let idNhoms = '';
            for (let k = 0; k < self.NhomKhachHangChosed().length; k++) {
                idNhoms += self.NhomKhachHangChosed()[k].ID + ',';
            }
            idNhoms = Remove_LastComma(idNhoms);
            ajaxHelper(DMDoiTuongUri + 'GetDoiTuong_hasDienThoai?idNhoms=' + idNhoms, 'GET').done(function (x) {
                if (x.res === true) {
                    self.MangKhachHangSaveDB(x.data);
                    // co the chon dong thoi KhachHang rieng hoac chon theo nhom
                    // neu chon all KH --> khong can add them KH nua
                    if (all.length == 0) {
                        for (let i = 0; i < self.MangKhachHangGuiTin().length; i++) {
                            self.MangKhachHangSaveDB.push(self.MangKhachHangGuiTin()[i]);
                        }
                    }
                    if (self.GiaTienMotTrangTin() * _soTinGui * self.MangKhachHangSaveDB().length > self.SoDuTaiKhoan()) {
                        ShowMessage_Danger("Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền");
                        Enable_BtnGuiTin("btnGuiTinNhan");
                        return false;
                    }
                    let j = 0;
                    guitin(objTin, j);
                }
            })
        }
        else {
            // gui riêng từng khách (không theo nhóm)
            self.MangKhachHangSaveDB(self.MangKhachHangGuiTin());
            let j = 0;
            guitin(objTin, j);
        }
    };
    function guitin(objSMS, j) {
        if (j < self.MangKhachHangSaveDB().length) {
            var myData = {};
            var objTin = objSMS;
            objTin.SoDienThoai = self.MangKhachHangSaveDB()[j].DienThoai;
            myData.objTinNhan = objTin;
            myData.ID_BrandName = self.IDBrandNameChoose();
            ajaxHelper(ThietLapAPI + "PostTinNhan", 'POST', myData).done(function () {
                j++;
                guitin(objSMS, j);
            }).fail(function () {
            });
        }
        else {
            console.log('else ', j);
            $('#exampleModal').modal('hide');
            ShowMessage_Success('Gửi tin nhắn thành công');
            Enable_BtnGuiTin("btnGuiTinNhan");
        }
    }
    //guitin();
    self.GuiTinNhanSN = function () {
        document.getElementById("btnGuiTinNhanSN").disabled = true;
        document.getElementById("btnGuiTinNhanSN").lastChild.data = " Đang lưu";
        var _noiDungTin = $('#txtNoiDungTin').val();
        var lenghtTinNhan = _noiDungTin.length;
        var _soTinGui = Math.ceil(lenghtTinNhan / self.numtext);
        var _loaiTinNhan = self.LoaiTinNhanGui();
        var _idbrand = self.IDBrandNameChoose();
        if (_idbrand === undefined) {
            ShowMessage_Danger("Vui lòng chọn BrandName gửi tin");
            Enable_BtnGuiTin("btnGuiTinNhanSN");
            return false;
        }
        if (self.MangKhachHangGuiTinSN().length === 0) {
            ShowMessage_Danger("Vui lòng chọn đối tượng cần gửi tin");
            Enable_BtnGuiTin("btnGuiTinNhanSN");
            return false;
        }
        if (_noiDungTin === "" || _noiDungTin === null || _noiDungTin === undefined || _noiDungTin === "undefined") {
            ShowMessage_Danger('Vui lòng nhập nội dung tin nhắn');
            Enable_BtnGuiTin("btnGuiTinNhanSN");
            return false;
        }
        var all = $.grep(self.MangKhachHangGuiTinSN(), function (x) {
            return x.ID === 0;
        });
        if (all.length > 0) {
            self.MangKhachHangGuiTinSN(self.ArrKhachHangCoSDTSN());
        }
        if (self.GiaTienMotTrangTin() * _soTinGui * self.MangKhachHangGuiTinSN().length > self.SoDuTaiKhoan()) {
            ShowMessage_Danger("Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền");
            Enable_BtnGuiTin("btnGuiTinNhan");
            return false;
        }
        $('#exampleModal .form-modal-send-sms').gridLoader();
        var j = 0;
        function guitin() {
            if (j < self.MangKhachHangGuiTinSN().length) {
                var myData = {};
                var objTin = {
                    SoDienThoai: self.MangKhachHangGuiTinSN()[j].DienThoai,
                    NoiDung: _noiDungTin,
                    SoTinGui: _soTinGui,
                    LoaiTinNhan: _loaiTinNhan,
                    ID_NguoiGui: _IDNguoiDung,
                    ID_DonVi: idDonVi
                };
                myData.objTinNhan = objTin;
                myData.ID_BrandName = _idbrand;
                $.ajax({
                    url: ThietLapAPI + "PostTinNhan",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: myData,
                    success: function (item) {
                    },
                    statusCode: {
                        404: function () {
                            self.error("Page not found");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrow) {
                        bottomrightnotify(jqXHR.responseText.replace(/"/g, ''), "danger");
                    },
                    complete: function () {
                        j++;
                        guitin();
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrow) {
                        self.error(textStatus + ": " + errorThrow + ": " + jqXHR.responseText);
                    });
            }
            else {
                $('#exampleModal').modal('hide');
                ShowMessage_Success("Gửi tin nhắn thành công");
                Enable_BtnGuiTin("btnGuiTinNhanSN");
                ResetKhachHangChosed();
            }
        }
        guitin();
    };
    function ResetKhachHangChosed() {
        arrIDDoiTuong = [];
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
        $('#count').text(0);
        $('#smssinhnhat tr td.check-group input').prop('checked', false);
        $('#smslichhen tr td.check-group input').prop('checked', false);
    }
    self.GuiTinNhanGD = function () {
        document.getElementById("btnGuiTinNhanGD").disabled = true;
        document.getElementById("btnGuiTinNhanGD").lastChild.data = " Đang lưu";
        var _noiDungTin = $('#txtNoiDungTin').val();
        var lenghtTinNhan = _noiDungTin.length;
        var _soTinGui = Math.ceil(lenghtTinNhan / self.numtext);
        var _loaiTinNhan = self.LoaiTinNhanGui();
        var _idbrand = self.IDBrandNameChoose();
        if (_idbrand === undefined) {
            ShowMessage_Danger("Vui lòng chọn BrandName gửi tin");
            Enable_BtnGuiTin("btnGuiTinNhanGD");
            return false;
        }
        if (self.MangKhachHangGuiTinGD().length === 0) {
            ShowMessage_Danger("Vui lòng chọn đối tượng cần gửi tin");
            Enable_BtnGuiTin("btnGuiTinNhanGD");
            return false;
        }
        if (_noiDungTin === "" || _noiDungTin === null || _noiDungTin === undefined || _noiDungTin === "undefined") {
            ShowMessage_Danger('Vui lòng nhập nội dung tin nhắn');
            Enable_BtnGuiTin("btnGuiTinNhanGD");
            return false;
        }
        if (self.GiaTienMotTrangTin() * _soTinGui * self.MangKhachHangGuiTinGD().length > self.SoDuTaiKhoan()) {
            ShowMessage_Danger("Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền");
            Enable_BtnGuiTin("btnGuiTinNhanGD");
            return false;
        }
        var j = 0;
        function guitin() {
            if (j < self.MangKhachHangGuiTinGD().length) {
                var myData = {};
                var objTin = {
                    SoDienThoai: self.MangKhachHangGuiTinGD()[j].SoDienThoai,
                    NoiDung: _noiDungTin,
                    SoTinGui: _soTinGui,
                    LoaiTinNhan: _loaiTinNhan,
                    ID_NguoiGui: _IDNguoiDung,
                    ID_DonVi: idDonVi,
                    ID_HoaDon: self.MangKhachHangGuiTinGD()[j].ID
                };
                myData.objTinNhan = objTin;
                myData.ID_BrandName = _idbrand;
                $.ajax({
                    url: ThietLapAPI + "PostTinNhan",
                    type: 'POST',
                    async: true,
                    dataType: 'json',
                    contentType: "application/x-www-form-urlencoded; charset=UTF-8",
                    data: myData,
                    success: function (item) {
                    },
                    statusCode: {
                        404: function () {
                            self.error("Page not found");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrow) {
                        bottomrightnotify(jqXHR.responseText.replace(/"/g, ''), "danger");
                    },
                    complete: function () {
                        j++;
                        guitin();
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrow) {
                        self.error(textStatus + ": " + errorThrow + ": " + jqXHR.responseText);
                    });
            }
            else {
                ShowMessage_Success('Gửi tin nhắn thành công');
                Enable_BtnGuiTin("btnGuiTinNhanGD");
                $('#exampleModal').modal('hide');
            }
        }
        guitin();
    };
    self.GuiTinNhanSDT = function () {
        document.getElementById("btnGuiTinNhanSDT").disabled = true;
        document.getElementById("btnGuiTinNhanSDT").lastChild.data = " Đang lưu";
        var _soDienThoai = $('#txtSoDienThoai').val();
        var _noiDungTin = $('#txtNoiDungTinSDT').val();
        var lenghtTinNhan = _noiDungTin.length;
        var _soTinGui = Math.ceil(lenghtTinNhan / self.numtext);
        var _loaiTinNhan = self.LoaiTinNhanGui();
        var _idbrand = self.IDBrandNameChoose();
        var _id_hoaDon = null;
        if (_loaiTinNhan === 1) {
            _id_hoaDon = self.ID_HoaDonGiaoDich();
        }
        if (_soDienThoai === "" || _soDienThoai === null || _soDienThoai === undefined || _soDienThoai === "undefined") {
            ShowMessage_Danger("Vui lòng nhập số điện thoại");
            Enable_BtnGuiTin("btnGuiTinNhanSDT");
            return false;
        }
        if (_idbrand === undefined) {
            ShowMessage_Danger("Vui lòng chọn BrandName gửi tin");
            Enable_BtnGuiTin("btnGuiTinNhanSDT");
            return false;
        }
        if (_noiDungTin === "" || _noiDungTin === null || _noiDungTin === undefined || _noiDungTin === "undefined") {
            ShowMessage_Danger("Vui lòng nhập nội dung tin nhắn");
            Enable_BtnGuiTin("btnGuiTinNhanSDT");
            return false;
        }
        if (self.GiaTienMotTrangTin() * _soTinGui > self.SoDuTaiKhoan()) {
            ShowMessage_Danger("Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền");
            Enable_BtnGuiTin("btnGuiTinNhanSDT");
            return false;
        }
        var myData = {};
        var objTin = {
            SoDienThoai: _soDienThoai,
            NoiDung: _noiDungTin,
            SoTinGui: _soTinGui,
            LoaiTinNhan: _loaiTinNhan,
            ID_NguoiGui: _IDNguoiDung,
            ID_DonVi: idDonVi,
            ID_HoaDon: _id_hoaDon
        };
        myData.objTinNhan = objTin;
        myData.ID_BrandName = _idbrand;
        $.ajax({
            url: ThietLapAPI + "PostTinNhan",
            type: 'POST',
            async: true,
            dataType: 'json',
            contentType: "application/x-www-form-urlencoded; charset=UTF-8",
            data: myData,
            success: function (item) {
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Gửi tin nhắn thành công", "success");
                $('#exampleModalSDT').modal('hide');
            },
            statusCode: {
                404: function () {
                    self.error("Page not found");
                }
            },
            error: function (jqXHR, textStatus, errorThrow) {
                bottomrightnotify(jqXHR.responseText.replace(/"/g, ''), "danger");
            },
            complete: function () {
                ;
                Enable_BtnGuiTin("btnGuiTinNhanSDT");
                SearchTinNhan();
            }
        })
            .fail(function (jqXHR, textStatus, errorThrow) {
                self.error(textStatus + ": " + errorThrow + ": " + jqXHR.responseText);
            });
    };

    self.GuiTinLichHen = function () {
        document.getElementById("btnGuiTinLichHen").disabled = true;
        document.getElementById("btnGuiTinLichHen").lastChild.data = " Đang lưu";
        var _noiDungTin = $('#txtNoiDungTin').val();
        var lenghtTinNhan = _noiDungTin.length;
        var _soTinGui = Math.ceil(lenghtTinNhan / self.numtext);
        var _idbrand = self.IDBrandNameChoose();
        if (_idbrand === undefined) {
            ShowMessage_Danger("Vui lòng chọn BrandName gửi tin");
            Enable_BtnGuiTin("btnGuiTinLichHen");
            return false;
        }
        if (self.Popup_LichHenChosed().length === 0) {
            ShowMessage_Danger("Vui lòng chọn khách hàng cần gửi tin");
            Enable_BtnGuiTin("btnGuiTinLichHen");
            return false;
        }
        if (_noiDungTin === "" || _noiDungTin === null || _noiDungTin === undefined || _noiDungTin === "undefined") {
            ShowMessage_Danger('Vui lòng nhập nội dung tin nhắn');
            Enable_BtnGuiTin("btnGuiTinLichHen");
            return false;
        }
        if (self.GiaTienMotTrangTin() * _soTinGui * self.Popup_LichHenChosed().length > self.SoDuTaiKhoan()) {
            ShowMessage_Danger("Số dư không đủ để gửi tin. Vui lòng nạp thêm tiền");
            Enable_BtnGuiTin("btnGuiTinLichHen");
            return false;
        }
        console.log(111, self.Popup_LichHenChosed())
        var j = 0;
        function guitin() {
            if (j < self.Popup_LichHenChosed().length) {
                let itFor = self.Popup_LichHenChosed()[j];
                let myData = {};
                let objTin = {
                    SoDienThoai: itFor.DienThoai,
                    NoiDung: _noiDungTin,
                    SoTinGui: _soTinGui,
                    LoaiTinNhan: self.LoaiTinNhanGui(),
                    ID_NguoiGui: _IDNguoiDung,
                    ID_DonVi: idDonVi,
                    ID_KhachHang: itFor.ID
                };
                myData.objTinNhan = objTin;
                myData.ID_BrandName = _idbrand;
                ajaxHelper(ThietLapAPI + 'PostTinNhan', 'POST', myData).done(function () {
                    j++;
                    guitin();
                }).fail(function (err) {

                });
            }
            else {
                ShowMessage_Success('Gửi tin nhắn thành công');
                Enable_BtnGuiTin("btnGuiTinLichHen");
                $('#exampleModal').modal('hide');
                SMS_LichHen();
            }
        }
        guitin();
    };

    //Phân trang list KH sinh nhật
    self.pageSizesKH = [10,20, 30, 50];
    self.pageSizeKH = ko.observable(self.pageSizesKH[0]);
    self.currentPageKH = ko.observable(0);
    self.fromitemKH = ko.observable(1);
    self.toitemKH = ko.observable();
    self.arrPaggingKH = ko.observableArray();
    self.TotalRecordKH = ko.observable();
    self.PageCountKH = ko.observable();
    self.ListKhachHangSinhNhat = ko.observableArray();
    var user = $('#txtTenTaiKhoan').text(); // get at ViewBag

    function GetParamSearch() {
        var dateFrom = '';
        var dateTo = '';
        var _now = new Date();
        var typeCompare = 0; // 0. so sanh ngay/thang; 1. so sanh nam
        if (self.customerBirthday() === '0') {
            var cacheSN = localStorage.getItem('SinhNhatKhachHang');
            if (cacheSN !== null) {
                self.customerBirthday_Quy(1);
                $('#txtCusBirthDay').val('Hôm nay');
                localStorage.removeItem('SinhNhatKhachHang');
            }
            switch (parseInt(self.customerBirthday_Quy())) {
                case 0:
                    // all
                    dateFrom = '1918-01-01'; // 100 years ago
                    dateTo = moment(_now).add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    dateFrom = moment(_now).format('YYYY-MM-DD');
                    dateTo = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 2: // hom qua
                    dateTo = moment(_now).format('YYYY-MM-DD');
                    dateFrom = moment(_now).subtract('days', 1).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    dateFrom = moment().startOf('week').add(1, 'days').format('YYYY-MM-DD');
                    dateTo = moment().endOf('week').add(2, 'days').format('YYYY-MM-DD');
                    break;
                case 4:
                    // tuan truoc
                    dateFrom = moment().startOf('week').subtract(6, 'days').format('YYYY-MM-DD');
                    dateTo = moment().startOf('week').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 5:
                    // thang nay
                    dateFrom = moment().startOf('month').format('YYYY-MM-DD');
                    dateTo = moment().endOf('month').add(1, 'days').format('YYYY-MM-DD');
                    $('#txtCusBirthDay').val('Tháng này');
                    break;
                case 6:
                    // thang truoc
                    dateFrom = moment().subtract(1, 'months').startOf('month').format('YYYY-MM-DD');
                    dateTo = moment().subtract(1, 'months').endOf('month').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 7:
                    // quy nay
                    dateFrom = moment().startOf('quarter').format('YYYY-MM-DD');
                    dateTo = moment().endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 8:
                    // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                    let prevQuarter = moment().quarter() - 1;
                    if (prevQuarter === 0) {
                        prevQuarter = 4;
                        let prevYear = moment().year() - 1;
                        dateFrom = moment().year(prevYear).quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dateTo = moment().year(prevYear).quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    }
                    else {
                        dateFrom = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                        dateTo = moment().quarter(prevQuarter).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    }
                    break;
                case 9:
                    // nam nay
                    dateFrom = moment().startOf('year').format('YYYY-MM-DD');
                    dateTo = moment().endOf('year').add(1, 'days').format('YYYY-MM-DD');
                    typeCompare = 1;
                    break;
                case 10:
                    // nam truoc
                    let prevYear = moment().year() - 1;
                    dateFrom = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dateTo = moment().year(prevYear).endOf('year').format('YYYY-MM-DD');
                    typeCompare = 1;
                    break;
                case 11:
                    // ngay mai
                    dateFrom = moment(_now).add(1, 'days').format('YYYY-MM-DD');
                    dateTo = moment(_now).add(2, 'days').format('YYYY-MM-DD');
                    break;
                case 12:
                    // tuan toi
                    dateFrom = moment().startOf('week').add(8, 'days').format('YYYY-MM-DD');
                    dateTo = moment().endOf('week').add(9, 'days').format('YYYY-MM-DD');
                    break;
                case 13:// thang toi
                    dateFrom = moment().add(1, 'months').startOf('month').format('YYYY-MM-DD');
                    dateTo = moment().add(1, 'months').endOf('month').add(1, 'days').format('YYYY-MM-DD');
                    break;
                case 14:// quý tới
                    var nextQuater = (moment().quarter() + 1 === 5) ? 1 : moment().quarter() + 1;
                    dateFrom = moment().quarter(nextQuater).startOf('quarter').format('YYYY-MM-DD');
                    dateTo = moment().quarter(nextQuater).endOf('quarter').add(1, 'days').format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            // chon ngay cu the
            var arrDate = self.customerBirthday_Input().split('-');
            dateFrom = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dateTo = moment(arrDate[1], 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');
        }

        var arrIDManager = [];
        for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDManager) === -1) {
                arrIDManager.push(self.ListIDNhanVienQuyen()[i]);
            }
        }
        var arrIDDV = [];
        for (var j = 0; j < self.MangNhomDV().length; j++) {
            if ($.inArray(self.MangNhomDV()[j].ID, arrIDDV) === -1) {
                arrIDDV.push(self.MangNhomDV()[j].ID);
            }
        }

        var idNhomDT = self.selectNhomDT();
        if (idNhomDT === undefined) {
            idNhomDT = null;
        }
        // nhom mac dinh
        if (idNhomDT === 0) {
            idNhomDT = '00000000-0000-0000-0000-000000000000';
        }

        return {
            DateFrom: dateFrom,
            DateTo: dateTo,
            ID_NhomKH: idNhomDT,
            TypeCompare: typeCompare,
            ArrIDNhanVien: arrIDManager,
            ArrIDDonVi: arrIDDV,
        }
    }

    function GetAllKhachHangSinhNhat() {
        var param = GetParamSearch();
        var listParams = {
            ID_DonViArr: [idDonVi],
            ID_NhomDoiTuong: param.ID_NhomKH,
            NgaySinh_TuNgay: param.DateFrom,
            NgaySinh_DenNgay: moment(param.DateTo).add(-1, 'days').format('YYYY-MM-DD'),
            LoaiNgaySinh: param.TypeCompare,
            ID_NhanVienQuanLys: param.ArrIDNhanVien,
            NguoiTao: user.trim(),
            CurrentPage: self.currentPageKH(),
            PageSize: self.pageSizeKH(),
            TrangThai: parseInt(self.Loc_TrangThaiGui())
        };
        console.log('sn', listParams);

        $('.table-reponsive').gridLoader();
        ajaxHelper(DMDoiTuongUri + "SMS_KhachHangSinhNhat", 'POST', listParams).done(function (x) {
         
            $('.table-reponsive').gridLoader({ show: false });
            if (x.res === true && x.data.length > 0) {
                self.ListKhachHangSinhNhat(x.data);
                self.PageCountKH(x.data[0].TotalPage);
                self.TotalRecordKH(x.data[0].TotalRow);
                SMSSinhNhat_PageList();
                // get arrIDKH in this page
                //var arrID = self.ListKhachHangSinhNhat().map(function (x) {
                //    return x.ID;
                //});
                //// count arrID had check
                //let count = 0;
                //for (let i = 0; i < arrIDDoiTuong.length; i++) {
                //    if ($.inArray(arrIDDoiTuong[i], arrID) > -1) {
                //        count += 1;
                //    }
                //}
                //$('#count').text(count);
                //if (arrIDDoiTuong.length > 0) {
                //    $('#divThaoTac').show();
                //    $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
                //    $('#count').text(arrIDDoiTuong.length);
                //}
                //else {
                //    $('#divThaoTac').hide();
                //    $('.choose-commodity').hide().trigger("addClassForButtonNew");
                //}
                //// set check input
                //$('.ckHeader').prop('checked', count === 10);
                $('#smssinhnhat tr td.check-group input').each(function (x) {
                    var id = $(this).attr('id');
                    if ($.inArray(id, arrIDDoiTuong) > -1) {
                        $(this).prop('checked', true);
                    }
                    else {
                        $(this).prop('checked', false);
                    }
                });
            }
            else {
                self.ListKhachHangSinhNhat([]);
                self.PageList_DisplayKH([]);
                self.PageCountKH(0);
                self.TotalRecordKH(0);
            }
        });
    }
    self.Loc_TrangThaiGui.subscribe(function (newVal) {
        GetListData_byType();
    });

    self.Loc_LoaiTin.subscribe(function (newVal) {
        SearchTinNhan();
    });

    self.ResetCurrentPageKH = function () {
        self.currentPageKH(0);
        GetAllKhachHangSinhNhat();
    };
    //self.PageResultsKH = ko.computed(function () {
    //    if (self.ListKhachHangSinhNhat() !== null) {
    //        self.fromitemKH((self.currentPageKH() * self.pageSizeKH()) + 1);
    //        if (((self.currentPageKH() + 1) * self.pageSizeKH()) > self.ListKhachHangSinhNhat().length) {
    //            var fromItem = (self.currentPageKH() + 1) * self.pageSizeKH();
    //            if (fromItem < self.TotalRecordKH()) {
    //                self.toitemKH((self.currentPageKH() + 1) * self.pageSizeKH());
    //            }
    //            else {
    //                self.toitemKH(self.TotalRecordKH());
    //            }
    //        } else {
    //            self.toitemKH((self.currentPageKH() * self.pageSizeKH()) + self.pageSizeKH());
    //        }
    //    }
    //});

    function SMSSinhNhat_PageList() {
        var arrPage = [];
        var allPage = self.PageCountKH();
        var currentPage = self.currentPageKH();
        if (allPage > 4) {
            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageKH()) + 1;
            }
            else {
                i = self.currentPageKH();
            }
            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberKH: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberKH: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberKH: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberKH: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberKH: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumberKH: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        self.PageList_DisplayKH(arrPage);

        var arr = self.ListKhachHangSinhNhat();
        if (arr !== null) {
            self.fromitemKH((self.currentPageKH() * self.pageSizeKH()) + 1);
            if (((self.currentPageKH() + 1) * self.pageSizeKH()) > arr.length) {
                var fromItem = (self.currentPageKH() + 1) * self.pageSizeKH();
                if (fromItem < self.TotalRecordKH()) {
                    self.toitemKH((self.currentPageKH() + 1) * self.pageSizeKH());
                }
                else {
                    self.toitemKH(self.TotalRecordKH());
                }
            } else {
                self.toitemKH((self.currentPageKH() * self.pageSizeKH()) + self.pageSizeKH());
            }
        }
    }

    self.VisibleStartPageKH = ko.computed(function () {
        if (self.PageList_DisplayKH().length > 0) {
            return self.PageList_DisplayKH()[0].pageNumberKH !== 1;
        }
    });
    self.VisibleEndPageKH = ko.computed(function () {
        if (self.PageList_DisplayKH().length > 0) {
            return self.PageList_DisplayKH()[self.PageList_DisplayKH().length - 1].pageNumberKH !== self.PageCountKH();
        }
    });
    self.GoToPageHDKH = function (page) {
        if (page.pageNumberKH !== '.') {
            self.currentPageKH(page.pageNumberKH - 1);
            GetAllKhachHangSinhNhat();
        }
    };
    self.StartPageKH = function () {
        self.currentPageKH(0);
        GetAllKhachHangSinhNhat();
    };
    self.BackPageKH = function () {
        if (self.currentPageKH() > 1) {
            self.currentPageKH(self.currentPageKH() - 1);
            GetAllKhachHangSinhNhat();
        }
    };
    self.GoToNextPageKH = function () {
        if (self.currentPageKH() < self.PageCountKH() - 1) {
            self.currentPageKH(self.currentPageKH() + 1);
            GetAllKhachHangSinhNhat();
        }
    };
    self.EndPageKH = function () {
        if (self.currentPageKH() < self.PageCountKH() - 1) {
            self.currentPageKH(self.PageCountKH() - 1);
            GetAllKhachHangSinhNhat();
        }
    };
    self.GetClassHDKH = function (page) {
        return ((page.pageNumberKH - 1) === self.currentPageKH()) ? "click" : "";
    };
    //------------------------------------end phân trang khách hàng SN
    // khách hàng giao dịch bán lẻ và gói dịch vụ
    self.pageSizesGD = [10,20, 30, 50];
    self.pageSizeGD = ko.observable(self.pageSizesGD[0]);
    self.currentPageGD = ko.observable(0);
    self.fromitemGD = ko.observable(1);
    self.toitemGD = ko.observable();
    self.arrPaggingGD = ko.observableArray();
    self.TotalRecordGD = ko.observable();
    self.PageCountGD = ko.observable();
    self.ListKhachHangGD = ko.observableArray();
    self.LoaiGD_BL = ko.observable(true)
    self.LoaiGD_GDV = ko.observable(true)
    function getKhachHangGD() {
        let loaiGD = 0;
        if (self.LoaiGD_BL()) {
            if (self.LoaiGD_GDV()) {
                loaiGD = 2;
            }
            else {
                loaiGD = 0;
            }
        }
        else {
            if (self.LoaiGD_GDV()) {
                loaiGD = 1;
            }
            else {
                loaiGD = 4;
            }
        }

        var param = GetParamSearch();
        var listParams = {
            ID_NhomDoiTuong: param.ID_NhomKH,
            NgaySinh_TuNgay: param.DateFrom,
            NgaySinh_DenNgay: param.DateTo,
            ID_NhanVienQuanLys: param.ArrIDNhanVien,
            ID_DonViArr: param.ArrIDDonVi,
            LoaiGiaoDich: loaiGD,
            NguoiTao: user.trim(),
            CurrentPage: self.currentPageGD(),
            PageSize: self.pageSizeGD(),
            TrangThai: parseInt(self.Loc_TrangThaiGui()),
            iddonvi: idDonVi
        };
        console.log('gd ', listParams)
        $('.table-reponsive').gridLoader();
        ajaxHelper(DMDoiTuongUri + "SMS_GetListKhachHangGiaoDich", 'POST', listParams).done(function (x) {
            $('.table-reponsive').gridLoader({ show: false });
            if (x.res == true && x.data.length > 0) {
                self.ListKhachHangGD(x.data);
                self.PageCountGD(x.data[0].TotalPage);
                self.TotalRecordGD(x.data[0].TotalRow);
                SMS_ListPage();
            }
            else {
                self.ListKhachHangGD([]);
                self.PageList_DisplayGD([]);
                self.PageCountGD(0);
                self.TotalRecordGD(0);
            }
        });
    }
    self.LoaiGD_BL.subscribe(function (newVal) {
        self.currentPageGD(0);
        getKhachHangGD();
    });
    self.LoaiGD_GDV.subscribe(function (newVal) {
        self.currentPageGD(0);
        getKhachHangGD();
    });
    self.ResetCurrentPageGD = function () {
        self.currentPageGD(0);
        getKhachHangGD();
    };
    self.PageResultsGD = ko.computed(function () {
        if (self.ListKhachHangGD() !== null) {
            self.fromitemGD((self.currentPageGD() * self.pageSizeGD()) + 1);
            if (((self.currentPageGD() + 1) * self.pageSizeGD()) > self.ListKhachHangGD().length) {
                var fromItem = (self.currentPageGD() + 1) * self.pageSizeGD();
                if (fromItem < self.TotalRecordGD()) {
                    self.toitemGD((self.currentPageGD() + 1) * self.pageSizeGD());
                }
                else {
                    self.toitemGD(self.TotalRecordGD());
                }
            } else {
                self.toitemGD((self.currentPageGD() * self.pageSizeGD()) + self.pageSizeGD());
            }
        }
    });

    function SMS_ListPage() {
        var arrPage = [];
        var allPage = self.PageCountGD();
        var currentPage = self.currentPageGD();
        if (allPage > 4) {
            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPageGD()) + 1;
            }
            else {
                i = self.currentPageGD();
            }
            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumberGD: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumberGD: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumberGD: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberGD: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumberGD: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumberGD: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        self.PageList_DisplayGD(arrPage);
    }
    //self.PageList_DisplayGD = ko.computed(function () {

    //});
    self.VisibleStartPageGD = ko.computed(function () {
        if (self.PageList_DisplayGD().length > 0) {
            return self.PageList_DisplayGD()[0].pageNumberGD !== 1;
        }
    });
    self.VisibleEndPageGD = ko.computed(function () {
        if (self.PageList_DisplayGD().length > 0) {
            return self.PageList_DisplayGD()[self.PageList_DisplayGD().length - 1].pageNumberGD !== self.PageCountGD();
        }
    });
    self.GoToPageHDGD = function (page) {
        if (page.pageNumberGD !== '.') {
            self.currentPageGD(page.pageNumberGD - 1);
            getKhachHangGD();
        }
    };
    self.StartPageGD = function () {
        self.currentPageGD(0);
        getKhachHangGD();
    };
    self.BackPageGD = function () {
        if (self.currentPageGD() > 1) {
            self.currentPageGD(self.currentPageGD() - 1);
            getKhachHangGD();
        }
    };
    self.GoToNextPageGD = function () {
        if (self.currentPageGD() < self.PageCountGD() - 1) {
            self.currentPageGD(self.currentPageGD() + 1);
            getKhachHangGD();
        }
    };
    self.EndPageGD = function () {
        if (self.currentPageGD() < self.PageCountGD() - 1) {
            self.currentPageGD(self.PageCountGD() - 1);
            getKhachHangGD();
        }
    };
    self.GetClassHDGD = function (page) {
        return ((page.pageNumberGD - 1) === self.currentPageGD()) ? "click" : "";
    };
    //end khách hàng giao dịch bán lẻ và gói dịch vụ
    $('.chose_kieubang').on('click', 'li', function () {
        var index = $(this).val();
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        $('#danhsachtab .ct-tab-pane').each(function (i) {
            $(this).removeClass('active');
            if (index === (i + 1)) {
                $(this).addClass('active');
            }
        });
        ResetCurrentPage();
        self.Loc_TrangThaiGui('0');
        self.customerBirthday_Quy(5);// default month
        $('#txtCusBirthDay').val('Tháng này');

        switch (index) {
            case 1:
                self.TypeSMS(0);
                self.LoaiTinNhanGui(3);
                $('#loccotSMS').show();
                $('#locloaichungtu, #locchinhanh, #filterleft-typework').hide();
                $('.timesinhnhat').hide();
                $('._nhomkhachhang').hide();
                $('#loctrangthai, #loaitin').show();
                //$('.trangthai1').hide();
                //$('.trangthai2').show();
                SearchTinNhan();
                break;
            case 2:
                self.TypeSMS(2);
                self.LoaiTinNhanGui(1);
                $('#loccotSMS, #filterleft-typework').hide();
                $('#locloaichungtu, #locchinhanh').show();
                $('.timesinhnhat').hide();
                $('._nhomkhachhang').show();
                //$('.trangthai1').show();
                //$('.trangthai2').hide();
                $('#loctrangthai, #loaitin').hide();
                getKhachHangGD();
                break;
            case 3:
                self.TypeSMS(1);
                self.LoaiTinNhanGui(2);
                $('#loccotSMS').hide();
                $('#locloaichungtu, #locchinhanh, #filterleft-typework').hide();
                $('.timesinhnhat').show();
                $('._nhomkhachhang').show();
                //$('.trangthai1').show();
                //$('.trangthai2').hide();
                $('#loctrangthai, #loaitin').hide();
                GetAllKhachHangSinhNhat();
                break;
            case 4:
                self.TypeSMS(3);
                self.LoaiTinNhanGui(4);
                $('#loccotSMS').hide();
                $('#locloaichungtu, #locchinhanh').hide();
                $('.timesinhnhat, #filterleft-typework').show();
                $('._nhomkhachhang').show();
                //$('.trangthai1').show();
                //$('.trangthai2').hide();
                $('#loctrangthai, #loaitin').hide();
                SMS_LichHen();
                break;
        }
        ResetKhachHangChosed();
    });
    $('#txtCusBirthDay_Input').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var thisDate = $(this).val();
        self.customerBirthday_Input(thisDate);
        GetListData_byType();
    });

    function ResetCurrentPage() {
        self.currentPageGD(0);
        self.currentPageKH(0);
        self.currentPage(0);
        self.ListRowChosed([]);
    }

    function GetListData_byType() {
        ResetCurrentPage();
        switch (self.TypeSMS()) {
            case 0:
                SearchTinNhan();
                break;
            case 1:
                GetAllKhachHangSinhNhat();
                break;
            case 2:
                getKhachHangGD();
                break;
            case 3:
                SMS_LichHen();
                break;
        };
    };

    $('.choseNgaySinh li').on('click', function () {
        $('#txtCusBirthDay').val($(this).text());
        self.customerBirthday_Quy($(this).val());
        GetListData_byType();
    });
    self.customerBirthday.subscribe(function () {
        GetListData_byType();
    });
    //Phân trang list tin nhắn
    self.ArrayTinNhans = ko.observableArray();
    self.pageSizes = [10,20, 30, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.arrPagging = ko.observableArray();
    self.TotalRecord = ko.observable();
    self.PageCount = ko.observable();
    function SearchTinNhan() {
        var lc_CTQuyen = JSON.parse(localStorage.getItem('lc_CTQuyen'));
        if ($.inArray('GuiTinNhan_XemDS', lc_CTQuyen) > -1) {
            let param = GetParamSearch();
            var model = {
                pageSize: self.pageSize(),
                currentPage: self.currentPage(),
                FromDate: param.DateFrom,
                ToDate: param.DateTo,
                Status: parseInt(self.Loc_TrangThaiGui()),
                TypeSMS: parseInt(self.Loc_LoaiTin()),
            };
            console.log(1, model)
            $('.table-reponsive').gridLoader();
            ajaxHelper(ThietLapAPI + 'GetAllTinGui', 'POST', model).done(function (data) {
                self.ArrayTinNhans(data.data);
                self.TotalRecord(data.TotalRecord);
                self.PageCount(data.PageCount);
                $('.table-reponsive').gridLoader({ show: false });
            });
        }
    }
    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchTinNhan();
    };
    self.PageResults = ko.computed(function () {
        if (self.ArrayTinNhans() !== null) {
            self.fromitem((self.currentPage() * self.pageSize()) + 1);
            if (((self.currentPage() + 1) * self.pageSize()) > self.ArrayTinNhans().length) {
                var fromItem = (self.currentPage() + 1) * self.pageSize();
                if (fromItem < self.TotalRecord()) {
                    self.toitem((self.currentPage() + 1) * self.pageSize());
                }
                else {
                    self.toitem(self.TotalRecord());
                }
            } else {
                self.toitem((self.currentPage() * self.pageSize()) + self.pageSize());
            }
        }
    });
    self.PageList_Display = ko.computed(function () {
        var arrPage = [];
        var allPage = self.PageCount();
        var currentPage = self.currentPage();
        if (allPage > 4) {
            var i = 0;
            if (currentPage === 0) {
                i = parseInt(self.currentPage()) + 1;
            }
            else {
                i = self.currentPage();
            }
            if (allPage >= 5 && currentPage > allPage - 5) {
                if (currentPage >= allPage - 2) {
                    // get 5 trang cuoi cung
                    for (var i = allPage - 5; i < allPage; i++) {
                        var obj = {
                            pageNumber: i + 1,
                        };
                        arrPage.push(obj);
                    }
                }
                else {
                    // get currentPage - 2 , currentPage, currentPage + 2
                    if (currentPage == 1) {
                        for (var j = currentPage - 1; (j <= currentPage + 3) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    } else {
                        for (var j = currentPage - 2; (j <= currentPage + 2) && j < allPage; j++) {
                            var obj = {
                                pageNumber: j + 1,
                            };
                            arrPage.push(obj);
                        }
                    }
                }
            }
            else {
                // get 5 trang dau
                if (i >= 2) {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i - 1,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
                else {
                    while (arrPage.length < 5) {
                        var obj = {
                            pageNumber: i,
                        };
                        arrPage.push(obj);
                        i = i + 1;
                    }
                }
            }
        }
        else {
            // neu chi co 1 trang --> khong hien thi DS trang
            if (allPage > 1) {
                for (var i = 0; i < allPage; i++) {
                    var obj = {
                        pageNumber: i + 1,
                    };
                    arrPage.push(obj);
                }
            }
        }
        return arrPage;
    });
    self.VisibleStartPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[0].pageNumber !== 1;
        }
    });
    self.VisibleEndPage = ko.computed(function () {
        if (self.PageList_Display().length > 0) {
            return self.PageList_Display()[self.PageList_Display().length - 1].pageNumber !== self.PageCount();
        }
    });
    self.GoToPageHD = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber - 1);
            SearchTinNhan();
        }
    };
    self.StartPage = function () {
        self.currentPage(0);
        SearchTinNhan();
    }
    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            SearchTinNhan();
        }
    }
    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            SearchTinNhan();
        }
    }
    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            SearchTinNhan();
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };
    // gui theo nhom
    self.filterNhomKH = ko.observable();
    self.ThietLap = ko.observableArray();
    self.NhomKHChosed = ko.observableArray();
    function GetCauHinhHeThong() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/HT_ThietLapAPI/' + "GetCauHinhHeThong/" + idDonVi, 'GET').done(function (data) {
                if (data !== null) {
                    self.ThietLap(data);
                }
                GetNhomDoiTuong_DonVi();
            });
        }
    }
    function GetNhomDoiTuong_DonVi() {
        ajaxHelper(DMDoiTuongUri + 'GetNhomDoiTuong_DonVi?loaiDT=1', 'GET').done(function (obj) {
            if (obj.res === true) {
                let data = obj.data;
                for (var i = 0; i < data.length; i++) {
                    let tenNhom = data[i].TenNhomDoiTuong;
                    tenNhom = tenNhom.concat(' ', locdau(tenNhom), ' ', GetChartStart(tenNhom));
                    data[i].Text_Search = tenNhom;
                }
                if (self.ThietLap().QuanLyKhachHangTheoDonVi) {
                    // only get Nhom chua cai dat ChiNhanh or in this ChiNhanh
                    var arrNhom = [];
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].NhomDT_DonVi.length > 0) {
                            var ex = $.grep(data[i].NhomDT_DonVi, function (x) {
                                return x.ID === idDonVi;
                            });
                            if (ex.length) {
                                arrNhom.push(data[i]);
                            }
                        }
                        else {
                            arrNhom.push(data[i]);
                        }
                    }
                    self.NhomDoiTuongs(arrNhom);
                }
                else {
                    self.NhomDoiTuongs(data);
                }
            }
            var newObj = {
                ID: 0,
                TenNhomDoiTuong: 'Nhóm mặc định',
                Text_Search: 'nhom mac dinh nmd'
            }
            self.NhomDoiTuongs.unshift(newObj);
        });
    }
    self.ShowModalNhomKH = function () {
        let all = $.grep(self.MangKhachHangGuiTin(), function (x) {
            return x.DienThoai === '999999999999999';
        });
        if (all.length > 0) {
            ShowMessage_Danger('Bạn đã chọn gửi tin cho toàn bộ khách hàng');
            return false;
        }
        $('#modalNhomKHNCC').modal('show');
        $('#modalNhomKHNCC input[type=checkbox]').attr('checked', false);
        for (let i = 0; i < self.NhomKHChosed().length; i++) {
            $('#smsNhom_' + self.NhomKHChosed()[i].ID).attr('checked', true);
        }
    }
    self.ArrFiterNhomKH = ko.computed(function () {
        var _filter = locdau(self.filterNhomKH());
        return ko.utils.arrayFilter(self.NhomDoiTuongs(), function (prop) {
            var chon = true;
            let name = locdau(prop.TenNhomDoiTuong);
            if (_filter) {
                chon = (name.indexOf(_filter) > -1);
            }
            return chon;
        })
    });
    self.ChoseNhomKH = function (item) {
        var $this = event.currentTarget;
        var isCheck = $($this).is(':checked');
        if (isCheck) {
            var ex = $.grep(self.NhomKHChosed(), function (x) {
                return x.ID === item.ID;
            })
            if (ex.length === 0) {
                self.NhomKHChosed.push(item);
            }
        }
        else {
            for (let i = 0; i < self.NhomKHChosed().length; i++) {
                if (self.NhomKHChosed()[i].ID === item.ID) {
                    self.NhomKHChosed.splice(i, 1);
                    break;
                }
            }
        }
    }
    self.AgreeChoseNhom = function () {
        $('#modalNhomKHNCC').modal('hide');
        self.NhomKhachHangChosed(self.NhomKHChosed());
    }
    function SearchKhachHang_ByID(id) {
        if (id !== null && id.length === 36) {
            var ex = $.grep(self.MangKhachHangGuiTin(), function (x) {
                return x.ID === id;
            });
            if (ex.length === 0) {
                ajaxHelper('/api/DanhMuc/DM_DoiTuongAPI/' + 'GetDM_DoiTuong/' + id, 'GET').done(function (data) {
                    self.MangKhachHangGuiTin.push(data);
                });
            }
        }
    }
    self.JqAutoSelectKH.subscribe(function (val) {
        var all = $.grep(self.MangKhachHangGuiTin(), function (x) {
            return x.DienThoai === '999999999999999';
        });
        if (all.length > 0) {
            ShowMessage_Danger('Bạn đã chọn toàn bộ khách hàng');
            return false;
        }
        SearchKhachHang_ByID(val);
    });

    self.selectedLoaiCongViecFilter = function (item) {
        var arrLCV = [];
        for (var i = 0; i < self.MangNhomCongViec().length; i++) {
            if ($.inArray(self.MangNhomCongViec()[i], arrLCV) === -1) {
                arrLCV.push(self.MangNhomCongViec()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrLCV) === -1) {
            self.MangNhomCongViec.push(item);
        }
        SMS_LichHen();

        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-CongViec li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#divTypeWorkChose input').remove();
    }

    self.CloseCongViec = function (item) {
        self.MangNhomCongViec.remove(item);
        if (self.MangNhomCongViec().length === 0) {
            $('#divTypeWorkChose').append('<input type="text" id="txtTypeWork" readonly="readonly" class="dropdown" placeholder="Chọn loại công việc">');
        }
        SMS_LichHen();
        // remove checks
        $('#selec-all-CongViec li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    PageLoad();
};
ko.applyBindings(new ViewModel);
$(function () {
    $('.daterange').daterangepicker({
        locale: {
            "format": 'DD/MM/YYYY',
            "separator": " - ",
            "applyLabel": "Tìm kiếm",
            "cancelLabel": "Hủy",
            "fromLabel": "Từ",
            "toLabel": "Đến",
            "customRangeLabel": "Custom",
            "daysOfWeek": [
                "CN",
                "T2",
                "T3",
                "T4",
                "T5",
                "T6",
                "T7"
            ],
            "monthNames": [
                "Tháng 1 ",
                "Tháng 2",
                "Tháng 3",
                "Tháng 4",
                "Tháng 5",
                "Tháng 6",
                "Tháng 7",
                "Tháng 8",
                "Tháng 9",
                "Tháng 10",
                "Tháng 11",
                "Tháng 12"
            ],
            "firstDay": 1
        }
    });
});
var arrIDDoiTuong = [];
function SetCheckAll_SinhNhat(obj) {
    var isChecked = $(obj).is(":checked");
    $('#tblsinhnhat .check-group input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $('#tblsinhnhat .check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !($.inArray(thisID, arrIDDoiTuong) > -1)) {
                arrIDDoiTuong.push(thisID);
            }
        });
    }
    else {
        $('#tblsinhnhat .check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            for (var i = 0; i < arrIDDoiTuong.length; i++) {
                if (arrIDDoiTuong[i] === thisID) {
                    arrIDDoiTuong.splice(i, 1);
                    break;
                }
            }
        })
    }
    if (arrIDDoiTuong.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDDoiTuong.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
}
function ChoseDoiTuong(obj) {
    var thisID = $(obj).attr('id');
    if ($(obj).is(':checked')) {
        if ($.inArray(thisID, arrIDDoiTuong) === -1) {
            arrIDDoiTuong.push(thisID);
        }
    }
    else {
        // remove item in arrID
        arrIDDoiTuong = arrIDDoiTuong.filter(x => x !== thisID);
    }
    if (arrIDDoiTuong.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrIDDoiTuong.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }
    // count input is checked
    var countCheck = 0;
    $('#smssinhnhat tr td.check-group input').each(function (x) {
        var id = $(this).attr('id');
        if ($.inArray(id, arrIDDoiTuong) > -1) {
            countCheck += 1;
        }
    });
    // set check for header
    var ckHeader = $('#smssinhnhat thead tr th:eq(0) input');
    var lenList = $('#smssinhnhat tbody tr.prev-tr-hide').length;
    if (countCheck === lenList) {
        ckHeader.prop('checked', true);
    }
    else {
        ckHeader.prop('checked', false);
    }
}
function RemoveAllCheck() {
    $('input[type=checkbox]').prop('checked', false);
    arrIDDoiTuong = [];
    $('#divThaoTac').hide();
    $('.choose-commodity').hide();
}

function SetCheckSinhNhat_NextPage(obj) {
    // find in list and set check
    var countCheck = 0;
    var allRow = 0;
    $('#tblsinhnhat tbody .check-group input').each(function (x) {
        var id = $(this).attr('id');
        allRow += 1;
        if ($.inArray(id, arrIDDoiTuong) > -1) {
            $(this).prop('checked', true);
            countCheck += 1;
        }
        else {
            $(this).prop('checked', false);
        }
    });
    // set again check header
    var ckHeader = $('#tblsinhnhat thead input')
    if (countCheck === allRow) {
        ckHeader.prop('checked', true);
    }
    else {
        ckHeader.prop('checked', false);
    }
}
