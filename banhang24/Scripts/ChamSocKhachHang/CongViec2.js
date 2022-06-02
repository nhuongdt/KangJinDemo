
ko.observableArray.fn.refresh = function () {
    var data = this().slice(0);
    this([]);
    this(data);
};

var ViewModal = function () {
    var self = this;
    var user = $('#txtTenTaiKhoan').text().trim();
    var userID = $('.idnguoidung').text();
    var idNhanVien = $('.idnhanvien').text();
    var idDonVi = $('#hd_IDdDonVi').val();
    var ChamSocKhachHangs = '/api/DanhMuc/ChamSocKhachHangAPI/';
    var DMDoiTuongUri = '/api/DanhMuc/DM_DoiTuongAPI/';
    var DM_LoaiTVLHUri = '/api/DanhMuc/DM_LoaiTuVanLichHenAPI/';
    var DMHangHoaUri = '/api/DanhMuc/DM_HangHoaAPI/';
    var ThietLapAPI = '/api/DanhMuc/ThietLapApi/';
    var Key_Form = 'Key_CongViec';
    $('#txtNgayTao').val('Tháng này');

    self.ListRowChosed = ko.observableArray();
    self.checkDoiTuong = ko.observable(1);
    self.ListAllDoiTuong = ko.observableArray();
    self.check_kieubang = ko.observable('2');

    self.booleanAddCV = ko.observable(true);
    self.IsUpdate = ko.observable(false);
    self.checkLienHeLai = ko.observable(false);
    self.IsWork = ko.observable(true);
    self.IsAppointment = ko.observable(true);
    self.IsDangXuLy = ko.observable(true);
    self.IsHoanThanh = ko.observable(false);
    self.IsDaHuy = ko.observable(false);

    self.checkKhachHang = ko.observable(true);
    self.checkNhaCungCap = ko.observable(true);
    self.MucDoUT = ko.observable("3");
    self.DoiTuongs = ko.observableArray();

    self.LoaiCongViecs = ko.observableArray();
    self.NhanViens = ko.observableArray();
    self.NhanVienQuanLys = ko.observableArray();
    self.CongTy = ko.observableArray();
    self.filter = ko.observable();
    self.filterCV = ko.observable();
    self.columsort = ko.observable(null);
    self.sort = ko.observable(null);
    self.error = ko.observable();
    self.ListIDNhanVienQuyen = ko.observableArray();
    self.listUserContact = ko.observableArray();
    self.selectedLienHe = ko.observable();
    self.selectedChooseLienHe = ko.observable();
    self.selectedChooseLienHeNCC = ko.observable();
    self.selectedKH = ko.observable();
    self.selectedChooseNhanVien = ko.observable();
    self.selectedChooseKhachHang = ko.observable();
    self.selectedTenDoiTuongKH = ko.observable();
    self.selectedChooseNhaCungCap = ko.observable();
    self.selectedNhanVien = ko.observable();
    self.booleanAddLoaiCV = ko.observable(true);
    self.selectedLoaiCV = ko.observable();
    self.selectedChooseLoaiCV = ko.observable();
    self.MangNhomCongViec = ko.observableArray();
    self.MangNhanVien = ko.observableArray();
    self.MangNhanVienSearch = ko.observableArray();
    self.MangNhanVienPhoiHop = ko.observableArray();
    self.ChiNhanhs = ko.observableArray();
    self.MangNhomDV = ko.observableArray();
    self.MangIDDV = ko.observableArray();
    self.selectedTimeNhacTruoc = ko.observable("1");
    self.selectedTimeNhacLai = ko.observable("1");
    self.filterLoaiCongViec = ko.observable();
    self.CheckHienLienHeLai = ko.observable(false);
    self.filterKhachHang = ko.observable();
    self.filterNhaCungCap = ko.observable();
    self.filterLienHe = ko.observable();
    self.filterLienHeNCC = ko.observable();
    self.files = ko.observableArray([]);
    self.arrImage = ko.observableArray();
    self.pageSizes = [10, 20, 30, 40, 50];
    self.pageSize = ko.observable(self.pageSizes[0]);
    self.currentPage = ko.observable(0);
    self.fromitem = ko.observable(1);
    self.toitem = ko.observable();
    self.TotalRecord = ko.observable();
    self.PageCount = ko.observable();

    self.CongViecs = ko.observableArray();
    self.TotalRecordCV = ko.observable();
    self.PageCountCV = ko.observable();

    self.filterNgayLapHD = ko.observable("0");
    self.filterNgayLapHD_Input = ko.observable(); // ngày cụ thể
    self.filterNgayLapHD_Quy = ko.observable('6'); // default Month
    self.TodayBC = ko.observable();
    self.MangLoaiCongViec = ko.observableArray();
    self.MangIDNhanVien = ko.observableArray();
    self.MangIDNhanVienFilter = ko.observableArray();
    self.MangIDNhanVienPhoiHopFilter = ko.observableArray();
    self.TextBirthDate = ko.observable("Toàn thời gian");
    self.TrangThaiCongViec = ko.observableArray();
    self.TrangThaiKhachHang = ko.observableArray();
    self.TypeBirthDate = ko.observable(0);
    self.startDate = ko.observable();
    self.endDate = ko.observable();
    self.CalendarHeader = ko.observable();
    self.Quyen_NguoiDung = ko.observable();

    self.ListCheck = ko.observableArray([
        { Key: "phanloai", Value: 0 },
        { Key: "loaicongviec", Value: 1 },
        { Key: "tencongviec", Value: 2 },
        { Key: "mucdouutien", Value: 3 },
        { Key: "thoigianbatdau", Value: 4 },
        { Key: "thoigianketthuc", Value: 5 },
        { Key: "madoituong", Value: 6 },
        { Key: "khachhangncc", Value: 7 },
        { Key: "sodienthoai", Value: 8 },
        { Key: "nguonkhachhang", Value: 9 },
        { Key: "nhanvienphutrach", Value: 10 },
        { Key: "nhacnho", Value: 11 },
        { Key: "trangthai", Value: 12 },
        { Key: "ketqua", Value: 13 },
        { Key: "nguoitao", Value: 14 },
        { Key: "ngaytao", Value: 15 },
        { Key: "ghichu", Value: 16 },
    ])

    function PageLoad() {
        var kieubang = localStorage.getItem('calendarKieuBang');
        if (kieubang !== null) {
            self.check_kieubang(kieubang);
            $(".chose_kieubang li").each(function (i) {
                $(this).find('a').removeClass('box-tab');
            });
            if (kieubang === '1') {// dangbang table
                $('#gridTable').addClass('active');
                $('#gridCalendar').removeClass('active');
                //hiển thị tùy chọn cột
                $("#selected-column").show();
                $('a[href="#gridTable"]').addClass('box-tab');// checked radio
            }
            else {
                $('#gridCalendar').addClass('active');
                $('#gridTable').removeClass('active');
                $("#selected-column").hide();
                $('a[href="#gridCalendar"]').addClass('box-tab');
            }
        }
        else {
            // default: calendar
            localStorage.setItem('calendarKieuBang', 2);
            $('#gridCalendar').addClass('active');
            $('#gridTable').removeClass('active');
            $("#selected-column").hide();
            $('a[href="#gridCalendar"]').addClass('box-tab');
        }
        LoadTrangThai();
        getallLoaiCongViec();
        loadHtmlGrid();
        LoadID_NhanVien();
        getAllChiNhanh();
        LoadQuyen_NguoiDung();
    }

    function LoadQuyen_NguoiDung() {
        ajaxHelper('/api/DanhMuc/HT_NguoiDungAPI/' + "GetHT_NhomNguoiDung?idnguoidung=" + userID + '&iddonvi=' + idDonVi, 'GET').done(function (data) {
            if (data.ID !== null) {
                self.Quyen_NguoiDung(data.HT_Quyen_NhomDTO);
                let insert = $.grep(self.Quyen_NguoiDung(), function (x) {
                    return x.MaQuyen == 'CongViec_XemDS';
                });
                if (insert.length > 0) {
                    $('.txtCongViecThemMoi').show();
                }
            }
            else {
                ShowMessage_Danger('Không có quyền xem danh sách công việc');
            }
        });
    }

    function getallLoaiCongViec() {
        ajaxHelper(DM_LoaiTVLHUri + 'GetDM_LoaiCongViec', 'GET').done(function (x) {
            if (x.res === true) {
                self.LoaiCongViecs(x.data);
                partialWork.LoaiCongViecs(x.data);
            }
        });
    }

    $('#selected-column').on('change', '.dropdown-left ul li input[type = checkbox]', function (i) {
        var valueCheck = $(this).val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, $(this).closest('li').index());
        $('.' + valueCheck).toggle();
    });

    $('#selected-column').on('change', '.dropdown-right ul li input[type = checkbox]', function (i) {
        var valueCheck = $(this).val();
        let index = $(this).closest('li').index() + 9;
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, index);
        $('.' + valueCheck).toggle();
    });

    $('#selected-column').on('click', '.dropdown-right ul li', function (i) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, $(this).index());
        $('.' + valueCheck).toggle();
    });
    $('#selected-column').on('click', '.dropdown-right ul li', function (i) {
        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        let index = $(this).index() + 9;
        LocalCaches.AddColumnHidenGrid(Key_Form, valueCheck, index);
        $('.' + valueCheck).toggle();
    });

    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form, $('#selected-column .dropdown-list ul li input[type = checkbox]'), self.ListCheck());
    }

    function LoadID_NhanVien() {
        ajaxHelper(ChamSocKhachHangs + 'GetListNhanVienLienQuanByIDLoGin_inDepartment?idnvlogin=' + idNhanVien
            + '&idChiNhanh=' + idDonVi + '&funcName=' + funcName, 'GET').done(function (data) {
                self.ListIDNhanVienQuyen(data);
                GetAllLienHe();
                GetListStaff_Working_byChiNhanh();

                var addQuick = localStorage.getItem('InsertQuickly');
                if (addQuick !== null) {
                    addQuick = parseInt(addQuick);
                    if (addQuick === 4) {
                        localStorage.removeItem('InsertQuickly');
                        self.showPopupoAddNew();
                    }
                }
            });
    }

    function GetListStaff_Working_byChiNhanh() {
        ajaxHelper("/api/DanhMuc/NS_NhanVienAPI/" + "GetNS_NhanVien_InforBasic?idDonVi=" + idDonVi, 'GET').done(function (data) {
            if (data !== null) {
                self.NhanViens(data);
                partialWork.NhanViens(data);
                if (self.ListIDNhanVienQuyen().length > 0) {
                    let arrManager = $.grep(self.NhanViens(), function (x) {
                        return $.inArray(x.ID, self.ListIDNhanVienQuyen()) > -1;
                    });
                    self.NhanVienQuanLys(arrManager);
                }
                else {
                    self.NhanVienQuanLys(data);
                }
                SearchGrid_OrCalendar();
            }
        });
    }

    function GetAllLienHe() {
        var idManagers = '';
        for (var i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            idManagers += self.ListIDNhanVienQuyen()[i] + ',';
        }
        var sWhere = 'DM_LienHe.TrangThai !=0 AND dt.LoaiDoiTuong =' + self.checkDoiTuong();
        ajaxHelper('/api/DanhMuc/DM_LienHeAPI/' + 'GetAllUserContact_byWhere_FilterNhanVien?txtSearch=' + sWhere + '&idManagers=' + idManagers, 'GET').done(function (result) {
            if (result !== null) {
                self.listUserContact(result);
            }
        });
    }

    self.selectedCN = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
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
        SearchGrid_OrCalendar();
    }

    function getAllChiNhanh() {
        ajaxHelper('/api/DanhMuc/DM_DonViAPI/' + "GetListDonViByIDNguoiDung?idnhanvien=" + idNhanVien, 'GET').done(function (data) {
            data = data.sort((a, b) => a.TenDonVi.localeCompare(b.TenDonVi, undefined, { caseFirst: "upper" }));
            self.ChiNhanhs(data);
            var obj = {
                ID: idDonVi,
                TenDonVi: $('#_txtTenDonVi').html()
            }
            // assign mangChiNhanh, and set check: avoid load douple list HoaDon
            self.MangNhomDV.push(obj);
            $('#selec-all-DV li').each(function () {
                if ($(this).attr('id') === idDonVi) {
                    $(this).find('.fa-check').remove();
                    $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
                }
            });
            $('#choose_TenDonVi input').remove();
        });
    }

    self.CloseDV = function (item) {
        $("#iconSort").remove();
        self.columsort(null);
        self.sort(null);
        self.MangNhomDV.remove(item);
        if (self.MangNhomDV().length === 0) {
            $('#choose_TenDonVi').append('<input type="text" id="dllChiNhanh" readonly="readonly" class="dropdown" placeholder="Chọn chi nhánh">');
        }
        SearchGrid_OrCalendar(false);
        // remove check
        $('#selec-all-DV li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

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
        SearchGrid_OrCalendar(false);
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-CongViec li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_LoaiCongViec input').remove();
    }

    self.CloseCongViec = function (item) {
        self.MangNhomCongViec.remove(item);
        if (self.MangNhomCongViec().length === 0) {
            $('#choose_LoaiCongViec').append('<input type="text" id="dllCongViec" readonly="readonly" class="dropdown" placeholder="Chọn công việc">');
        }
        SearchGrid_OrCalendar(false);
        // remove checks
        $('#selec-all-CongViec li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.selectedNhanVienFilter = function (item) {
        if (self.selectedNhanVien() !== item.ID) {
            var arrLCV = [];
            for (var i = 0; i < self.MangNhanVien().length; i++) {
                if ($.inArray(self.MangNhanVien()[i], arrLCV) === -1) {
                    arrLCV.push(self.MangNhanVien()[i].ID);
                }
            }
            if ($.inArray(item.ID, arrLCV) === -1) {
                self.MangNhanVien.push(item);
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
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Nhân viên phối hợp không được trùng nhân viên phụ trách.", "danger");
            return false;
        }
    };

    self.searchNVQuanLy = ko.observable();

    self.FilterNhanVienPhuTrach = ko.computed(function () {
        var inputSearch = self.searchNVQuanLy();
        var locdauInput = locdau(inputSearch);
        if (inputSearch !== undefined && inputSearch !== '') {
            return $.grep(self.NhanVienQuanLys(), function (x) {
                var itemSearch = locdau(x.TenNhanVien);
                return itemSearch.indexOf(locdauInput) > -1;
            });
        }
        else {
            return self.NhanVienQuanLys();
        }
    });

    self.selectedNhanVienFilterSearch = function (item) {
        var arrLCV = [];
        for (var i = 0; i < self.MangNhanVienSearch().length; i++) {
            if ($.inArray(self.MangNhanVienSearch()[i], arrLCV) === -1) {
                arrLCV.push(self.MangNhanVienSearch()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrLCV) === -1) {
            self.MangNhanVienSearch.push(item);
        }
        SearchGrid_OrCalendar(false);
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-NhanVienSearch li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        //$('#choose_NhanVienSearch input').remove();
    }

    // not use
    self.selectedNhanVienFilterPhoiHop = function (item) {
        var arrLCV = [];
        for (var i = 0; i < self.MangNhanVienPhoiHop().length; i++) {
            if ($.inArray(self.MangNhanVienPhoiHop()[i], arrLCV) === -1) {
                arrLCV.push(self.MangNhanVienPhoiHop()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrLCV) === -1) {
            self.MangNhanVienPhoiHop.push(item);
        }
        SearchGrid_OrCalendar(false);
        //$('#choose_DonVi input').remove();
        // thêm dấu check vào đối tượng được chọn
        $('#selec-all-NhanVienPhoiHop li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
                $(this).append('<i class="fa fa-check check-after-li" style="display:block"></i>')
            }
        });
        $('#choose_NhanVienPhoiHop input').remove();
    }

    self.CloseNV = function (item) {
        self.MangNhanVien.remove(item);
        if (self.MangNhanVien().length === 0) {
            $('#choose_NhanVien').append('<input type="text" style="background:#f0f1f1" id="dllNhanVien" readonly="readonly" class="dropdown" placeholder="Chọn nhân viên">');
        }

        // remove checks
        $('#selec-all-NhanVien li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.CloseNVSearch = function (item) {
        self.MangNhanVienSearch.remove(item);
        if (self.MangNhanVienSearch().length === 0) {
            $('#choose_NhanVienSearch').attr('placeholder', 'Chọn nhân viên');
            //$('#choose_NhanVienSearch').append('<input type="text" id="dllNhanVienSearch" readonly="readonly" class="dropdown" placeholder="Chọn nhân viên">');
        }

        SearchGrid_OrCalendar(false);
        // remove checks
        $('#selec-all-NhanVienSearch li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.CloseNVPhoiHop = function (item) {
        self.MangNhanVienPhoiHop.remove(item);
        if (self.MangNhanVienPhoiHop().length === 0) {
            $('#choose_NhanVienPhoiHop').append('<input type="text" id="dllNhanVienPhoiHop" readonly="readonly" class="dropdown" placeholder="Chọn nhân viên">');
        }

        SearchGrid_OrCalendar(false);
        // remove checks
        $('#selec-all-NhanVienPhoiHop li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('.fa-check').remove();
            }
        });
    }

    self.xoaCongViec = function (item) {
        var sLoai = 'công việc';
        switch (item.PhanLoai) {
            case 3:
                sLoai = 'lịch hẹn';
                break;
        }
        dialogConfirm('Thông báo xóa', 'Bạn có chắc chắn muốn xóa ' + sLoai + ' <b > ' + item.Ma_TieuDe + ' </b>  ngày <b>' + moment(item.NgayGio).format('DD/MM/YYYY') + ' </b> không? ', function () {
            partialWork.DeleteWork(item, sLoai);
            SearchGrid_OrCalendar(false);
        });
    }

    self.arrFilterLoaiCongViec = ko.computed(function () {
        var _filter = self.filterLoaiCongViec();

        return arrFilter = ko.utils.arrayFilter(self.LoaiCongViecs(), function (prod) {
            var chon = true;
            var arr = locdau(prod.TenLoaiTuVanLichHen).toLowerCase().split(/\s+/);
            var sSearch = '';

            for (var i = 0; i < arr.length; i++) {
                sSearch += arr[i].toString().split('')[0];
            }

            if (chon && _filter) {
                chon = (locdau(prod.TenLoaiTuVanLichHen).toLowerCase().indexOf(locdau(_filter).toLowerCase()) >= 0 ||
                    sSearch.indexOf(locdau(_filter).toLowerCase()) >= 0
                );
            }
            return chon;
        });
    });

    self.checkDoiTuong.subscribe(function (value) {
        if (value !== undefined) {
            self.selectedKH(undefined);
            $('#txtNhaCungCap').text("--- Chọn nhà cung cấp ---");
            $('#lstNhaCungCap span').each(function () {
                $(this).empty();
            });
            $('#txtKhachHang').text("--- Chọn khách hàng ---");
            $('#lstKhachHang span').each(function () {
                $(this).empty();
            });
            $('#txtLienHe').text("--- Chọn liên hệ ---");
            $('#lstLienHe span').each(function () {
                $(this).empty();
            });
            $('#txtLienHeNCC').text("--- Chọn liên hệ ---");
            $('#lstLienHeNCC span').each(function () {
                $(this).empty();
            });
            if (value === 1) {
                $('#formKhachHang').show();
                $('#formNhaCungCap').hide();
                $('#formLienHeKhachHang').show();
                $('#formLienHeNhaCungCap').hide();
                $('#dlltrangthaikh').show();
                $('#SL_TrangThaiKH').val(undefined);

            }
            else {
                $('#formKhachHang').hide();
                $('#formNhaCungCap').show();
                $('#formLienHeNhaCungCap').show();
                $('#formLienHeKhachHang').hide();
                $('#dlltrangthaikh').hide();
                $('#SL_TrangThaiKH').val(undefined);
            }
            //getAllDMDoiTuong();
            GetAllLienHe();
        }
    });

    self.FilterPhanLoai = function () {
        self.currentPage(0);
        SearchGrid_OrCalendar(false);
    }

    $('#clickTaiFile').on('click', function () {
        $('#clickUpFile').click();
        $('#txtTenFile').html($('#clickUpFile').val());
    });

    self.deleteFile = function () {
        self.files([]);
        $('#txtTenFile').html("");
        $('#clickXoaFile').hide();
        $('#workfile').hide();
    };

    self.fileSelect = function (elemet, event) {
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
                self.files([]);
                reader.onload = (function (theFile) {
                    return function (e) {
                        self.files.push(new FileModel(theFile, e.target.result));
                        $('#txtTenFile').html(theFile.name);
                        $('#clickXoaFile').show();
                        $('#workfile').show();
                    };
                })(f);
                // Read in the image file as a data URL.
                reader.readAsDataURL(f);
            }
        }
    };

    self.UpLoadFile = function (id) {
        //self.files(self.DM_HangHoa_Anh());
        //for (var i = 0; i < self.files().length; i++) {
        var i = 0;
        if (i < self.files().length) {
            var formData = new FormData();
            formData.append("imageUpHHForm", self.files()[i].file);
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
            ajaxHelper(ChamSocKhachHangs + 'UpdateCongViecXoaFile?idcv=' + id, 'GET').done(function (data) {
            });
        }
    }

    self.DownLoadFileCongViec = function (item) {
        var url = DMHangHoaUri + "Download_fileExcel?fileSave=" + item.FileDinhKem;
        window.location.href = url;
    };

    self.LinkKhachHang = function (item) {
        localStorage.setItem('FindKhachHang', item.MaDoiTuong);
        if (item.LoaiDoiTuong === 1) {
            var url = "/#/Customers";
            window.open(url);
        }
        else {
            var url1 = "/#/Suppliers";
            window.open(url1);
        }
    };

    function GetParamSearch() {
        // loaicv
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

        // nhanvien phutrach
        var arrNV = [];
        if (self.MangNhanVienSearch().length === 0) {
            arrNV = self.NhanVienQuanLys().map(function (i, e) {
                return i.ID;
            });
            arrNV.push(const_GuidEmpty);
        }
        else {
            for (let i = 0; i < self.MangNhanVienSearch().length; i++) {
                if ($.inArray(self.MangNhanVienSearch()[i], arrNV) === -1) {
                    arrNV.push(self.MangNhanVienSearch()[i].ID);
                }
            }
        }

        // nhanvien phoihop
        var arrNVPH = [];
        if (self.MangNhanVienPhoiHop().length === 0) {
            arrNVPH = self.NhanViens().map(function (i, e) {
                return i.ID;
            });
            arrNVPH.push(const_GuidEmpty);
        }
        else {
            for (let i = 0; i < self.MangNhanVienPhoiHop().length; i++) {
                if ($.inArray(self.MangNhanVienPhoiHop()[i], arrNVPH) === -1) {
                    arrNVPH.push(self.MangNhanVienPhoiHop()[i].ID);
                }
            }
        }

        // chi nhanh
        var arrDV = [];
        var chinhanhs = '';
        if (self.MangNhomDV().length === 0) {
            arrDV = [idDonVi];
            chinhanhs = $('#_txtTenDonVi').text();
        }
        else {
            for (let i = 0; i < self.MangNhomDV().length; i++) {
                if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                    arrDV.push(self.MangNhomDV()[i].ID);
                    chinhanhs += self.MangNhomDV()[i].TenDonVi + ',';
                }
            }
            chinhanhs = Remove_LastComma(chinhanhs);
        }

        var loaidoituong = '%%';
        if (self.checkKhachHang()) {
            if (self.checkNhaCungCap() === false) {
                loaidoituong = 1;
            }
        }
        else {
            if (self.checkNhaCungCap()) {
                loaidoituong = 2;
            }
        }
        var mucdouutien;
        if ($('#txtUTAll').hasClass('active')) {
            mucdouutien = '%%';
        }
        if ($('#txtUTC').hasClass('active')) {
            mucdouutien = '1';
        }
        if ($('#txtUTTB').hasClass('active')) {
            mucdouutien = '2';
        }
        if ($('#txtUTT').hasClass('active')) {
            mucdouutien = '3';
        }

        // phanloai
        var phanloai = '%%';
        if (self.IsWork()) {
            if (self.IsAppointment() === false) {
                phanloai = '4';
            }
        }
        else {
            if (self.IsAppointment()) {
                phanloai = '3';
            }
        }

        // trang thai cv
        var arrTrangThaiCV = self.TrangThaiCongViec().filter(o => o.IsSelected === true);
        if (arrTrangThaiCV.length === 0) {
            arrTrangThaiCV = self.TrangThaiCongViec().map(function (i, e) {
                return i.ID;
            });
        }
        else {
            arrTrangThaiCV = arrTrangThaiCV.map(function (i, e) {
                return i.ID;
            });
        }

        // loc theo quyen xem cua nhan vien (todo)
        var arrIDNV = [];
        for (let i = 0; i < self.ListIDNhanVienQuyen().length; i++) {
            if ($.inArray(self.ListIDNhanVienQuyen()[i], arrIDNV) === -1) {
                arrIDNV.push(self.ListIDNhanVienQuyen()[i]);
            }
        }
        self.MangIDNhanVien(arrIDNV);

        // from - to
        var _now = new Date();
        var currentWeekDay = _now.getDay();
        var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
        var dayStart = '';
        var dayEnd = '';

        if (self.filterNgayLapHD() === '0') {
            switch (parseInt(self.filterNgayLapHD_Quy())) {
                case 0:
                    // all
                    self.TodayBC('Toàn thời gian');
                    dayStart = '2016-01-01';// get all lichhen from 2016 -> to next year
                    dayEnd = moment().endOf('year').add('year', 1).format('YYYY-MM-DD');
                    //dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 1:
                    // hom nay
                    self.TodayBC('Hôm nay');
                    dayStart = moment(_now).format('YYYY-MM-DD');
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 2:
                    // hom qua
                    self.TodayBC('Hôm qua');
                    dayEnd = moment(_now).format('YYYY-MM-DD');
                    dayStart = moment(new Date(_now.setDate(_now.getDate() - 1))).format('YYYY-MM-DD');
                    break;
                case 3:
                    // tuan nay
                    self.TodayBC('Tuần này');
                    dayStart = moment(new Date(_now.setDate(_now.getDate() - lessDays - 1))).format('YYYY-MM-DD'); // start of wwek
                    dayEnd = moment(new Date(_now.setDate(_now.getDate() + 6))).add('days', 1).format('YYYY-MM-DD'); // end of week
                    break;
                case 4:
                    // tuan truoc
                    self.TodayBC('Tuần trước');
                    dayStart = moment().weekday(-6).format('YYYY-MM-DD');
                    dayEnd = moment(dayStart, 'YYYY-MM-DD').add(6, 'days').add('days', 1).format('YYYY-MM-DD'); // add day in moment.js
                    break;
                case 5:
                    // 7 ngay qua
                    self.TodayBC('7 ngày qua');
                    dayEnd = moment(_now).add('days', 1).format('YYYY-MM-DD');
                    dayStart = moment(new Date(_now.setDate(_now.getDate() - 7))).format('YYYY-MM-DD');
                    break;
                case 6:
                    // thang nay
                    self.TodayBC('Tháng này');
                    dayStart = moment(new Date(_now.getFullYear(), _now.getMonth(), 1)).format('YYYY-MM-DD');
                    dayEnd = moment(new Date(_now.getFullYear(), _now.getMonth() + 1, 0)).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 7:
                    // thang truoc
                    self.TodayBC('Tháng trước');
                    dayStart = moment(new Date(_now.getFullYear(), _now.getMonth() - 1, 1)).format('YYYY-MM-DD');
                    dayEnd = moment(new Date(_now.getFullYear(), _now.getMonth(), 0)).add('days', 1).format('YYYY-MM-DD');
                    break;
                case 10:
                    // quy nay
                    self.TodayBC('Quý này');
                    dayStart = moment().startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 11:
                    // quy truoc = currQuarter -1; // if (currQuarter -1 == 0) --> (assign = 1)
                    self.TodayBC('Quý trước');
                    var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                    dayStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                    dayEnd = moment().quarter(prevQuarter).endOf('quarter').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 12:
                    // nam nay
                    self.TodayBC('Năm nay');
                    dayStart = moment().startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 13:
                    // nam truoc
                    self.TodayBC('Năm trước');
                    var prevYear = moment().year() - 1;
                    dayStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                    dayEnd = moment().year(prevYear).endOf('year').add('days', 1).format('YYYY-MM-DD');
                    break;
                case 8:
                    // ngay mai
                    self.TodayBC('Ngày mai');
                    dayStart = moment(_now).add('day', 1).format('YYYY-MM-DD');
                    dayEnd = moment(_now).add('day', 2).format('YYYY-MM-DD');
                    break;
                case 9:
                    // tuan toi
                    self.TodayBC('Tuần tới');
                    dayStart = moment().startOf('week').add('days', 8).format('YYYY-MM-DD');
                    dayEnd = moment().endOf('week').add('days', 9).format('YYYY-MM-DD');
                    break;
                case 14:
                    // thang toi
                    self.TodayBC('Tháng tới');
                    dayStart = moment().add('months', 1).startOf('month').format('YYYY-MM-DD');
                    dayEnd = moment().add('months', 1).add('days', 1).endOf('month').format('YYYY-MM-DD');
                    break;
            }
        }
        else {
            // chon ngay cu the
            var arrDate = self.filterNgayLapHD_Input().split('-');
            dayStart = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            dayEnd = moment(arrDate[1], 'DD/MM/YYYY').add(1, 'days').format('YYYY-MM-DD');
            self.TodayBC('Từ ' + moment(arrDate[0], 'DD/MM/YYYY').format('DD/MM/YYYY') + ' đến ' + moment(arrDate[1], 'DD/MM/YYYY').format('DD/MM/YYYY'));
        }
        var txtSearch = self.filterCV();
        if (txtSearch !== null && txtSearch !== undefined) {
            txtSearch = txtSearch.toLowerCase();
        }

        return {
            ID_DonVis: arrDV,
            IDLoaiTuVans: arrLoaiCV,
            IDNhanVienPhuTrachs: arrNV,
            TrangThaiCVs: arrTrangThaiCV,
            MucDoUuTien: mucdouutien,
            LoaiDoiTuong: loaidoituong,
            PhanLoai: phanloai,
            CurrentPage: self.currentPage(),
            PageSize: self.pageSize(),
            TenChiNhanhs: chinhanhs,
            TextSearch: txtSearch,
            FromDate: dayStart,
            ToDate: dayEnd,
            ID_KhachHang: '%%',
        }
    }

    function SearchGrid_OrCalendar(isExport) {
        if (self.check_kieubang() === '1') {
            SearchCongViec(isExport);
        }
        else {
            var param = GetParamSearch();
            $('#gridCalendar').addClass('active');
            $('#gridTable').removeClass('active');
            if ($(".fc-view").children().length === 0) {
                // nếu calendar chưa render event
                setTimeout(function () {
                    //load bảng thêm 1 lần để căn chỉnh lại vị trí từ các event
                    //$calendar.fullCalendar("render");
                    $calendar.fullCalendar('changeView', "month");
                    $calendar.fullCalendar('changeView', isView);
                }, 300);
            }
            else {
                $calendar.fullCalendar('removeEvents');
                $calendar.fullCalendar('addEventSource', param);
                $calendar.fullCalendar('refetchEvents');

                $calendar.fullCalendar('changeView', "month");
                $calendar.fullCalendar('changeView', isView);
            }
        }
    }

    function SearchCongViec(isExport) {
        isExport = isExport || false;
        var arrHidecolums = LocalCaches.LoadColumnGrid(Key_Form);
        var columnHide = null;
        for (let i = 0; i < arrHidecolums.length; i++) {
            if (i === 0) {
                columnHide = arrHidecolums[i].Value;
            }
            else {
                columnHide = arrHidecolums[i].Value + "_" + columnHide;
            }
        }

        var param = GetParamSearch();
        var model = {
            ID_DonVis: param.ID_DonVis,
            IDLoaiTuVans: param.IDLoaiTuVans,
            IDNhanVienPhuTrachs: param.IDNhanVienPhuTrachs,
            TrangThaiCVs: param.TrangThaiCVs,
            MucDoUuTien: param.MucDoUuTien,
            LoaiDoiTuong: param.LoaiDoiTuong,
            PhanLoai: param.PhanLoai,
            FromDate: param.FromDate,
            ToDate: param.ToDate,
            TextSearch: param.TextSearch,
            CurrentPage: self.currentPage(),
            PageSize: self.pageSize(),
            ColumnsHide: columnHide,
            TenChiNhanhs: param.TenChiNhanhs,
            TypeShow: self.check_kieubang(),
            ID_KhachHang: '%%',
        }

        var cacheCongViecTongQuan = localStorage.getItem('CongViecKhachHang');
        var cacheLichHenTongQuan = localStorage.getItem('LichHenKhachHang');
        if (cacheCongViecTongQuan !== null || cacheLichHenTongQuan !== null) {
            if (cacheCongViecTongQuan !== null) {
                cacheCongViecTongQuan = JSON.parse(cacheCongViecTongQuan);
                self.filterNgayLapHD('1');
                self.IsWork(true);
                self.IsAppointment(false);
                model.PhanLoai = '4';
                self.filterNgayLapHD_Input(cacheCongViecTongQuan.FromDate + ' - ' + cacheCongViecTongQuan.ToDate);
                model.FromDate = moment(cacheCongViecTongQuan.FromDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
                model.ToDate = moment(cacheCongViecTongQuan.ToDate, 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
            }
            if (cacheLichHenTongQuan !== null) {
                cacheLichHenTongQuan = JSON.parse(cacheLichHenTongQuan);
                self.filterNgayLapHD('1');
                model.PhanLoai = '3';
                self.IsWork(false);
                self.IsAppointment(true);

                self.filterNgayLapHD_Input(cacheLichHenTongQuan.FromDate + ' - ' + cacheLichHenTongQuan.ToDate);
                model.FromDate = moment(cacheLichHenTongQuan.FromDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
                model.ToDate = moment(cacheLichHenTongQuan.ToDate, 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
            }
        }

        localStorage.removeItem('CongViecKhachHang');
        localStorage.removeItem('LichHenKhachHang');

        $('#gridTable').gridLoader();
        if (isExport) {
            $('#gridTable').gridLoader({ show: false });
            ajaxHelper(ChamSocKhachHangs + 'ExportExcel_CongViec', 'POST', model).done(function (url) {
                self.DownloadFileExportXLSX(url);
            });
        }
        else {
            ajaxHelper(ChamSocKhachHangs + 'GetListLichHen_FullCalendar', 'POST', model).done(function (x) {
                $('#gridTable').gridLoader({ show: false });
                console.log(x)
                if (x.res === true) {
                    self.CongViecs(x.data);
                    self.TotalRecord(x.TotalRow);
                    self.PageCount(x.PageCount);
                    //$("#table-congviec").colResizable({
                    //    liveDrag: true,
                    //    gripInnerHtml: "<div class='grip'></div>",
                    //    draggingClass: "dragging",
                    //    resizeMode: 'overflow',
                    //    hoverCursor: "col-resize",
                    //    dragCursor: "col-resize",

                    //});

                    // check cache old
                    let column = localStorage.getItem(Key_Form);
                    if (column !== null) {
                        column = JSON.parse(column);
                        let arr = $.grep(column, function (x) {
                            return $.inArray(x.NameClass, ['tenlienhe', 'noidung', 'nhanvienphoihop']) > -1;
                        });
                        if (arr.length > 0) {
                            localStorage.removeItem(Key_Form);
                        }
                    }

                    loadHtmlGrid();

                    localStorage.removeItem('CongViecKhachHang');
                    localStorage.removeItem('LichHenKhachHang');
                }
            });
        }
    }

    self.XuatFileCongViec = function () {
        SearchCongViec(true);
        var objDiary = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: "Công việc",
            NoiDung: "Xuất excel danh sách công việc",
            NoiDungChiTiet: "Xuất excel danh sách công việc",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };
        Insert_NhatKyThaoTac_1Param(objDiary);
    }

    self.DownloadFileExportXLSX = function (url) {
        var url1 = DMHangHoaUri + "Download_fileExcel?fileSave=" + url;
        window.location.href = url1;
    };

    function LoadTrangThai() {
        ajaxHelper(ChamSocKhachHangs + 'GetTrangThaiTimKiem', 'GET').done(function (data) {
            if (data.res === true) {
                data.dataSoure.ttcongviec[0].IsSelected = true;// gan trang thai DangXuLy = true --> checked
                self.TrangThaiCongViec(data.dataSoure.ttcongviec);
                self.TrangThaiKhachHang(data.dataSoure.ttkhachhang);
            }
        });
    };

    self.RemoveTrangThaiCongViec = function (item) {
        item.IsSelected = !item.IsSelected;
        self.TrangThaiCongViec.refresh();
        SearchGrid_OrCalendar(false);
    }

    $('#txtUTAll').on('click', function () {
        $('#txtUTAll').addClass('active');
        $('#txtUTC').removeClass('active');
        $('#txtUTTB').removeClass('active');
        $('#txtUTT').removeClass('active');
        SearchGrid_OrCalendar(false);
    });

    $('#txtUTC').on('click', function () {
        $('#txtUTC').addClass('active');
        $('#txtUTTB').removeClass('active');
        $('#txtUTT').removeClass('active');
        $('#txtUTAll').removeClass('active');
        SearchGrid_OrCalendar(false);
    });

    $('#txtUTTB').on('click', function () {
        $('#txtUTC').removeClass('active');
        $('#txtUTTB').addClass('active');
        $('#txtUTT').removeClass('active');
        $('#txtUTAll').removeClass('active');
        SearchGrid_OrCalendar(false);
    });

    $('#txtUTT').on('click', function () {
        $('#txtUTC').removeClass('active');
        $('#txtUTTB').removeClass('active');
        $('#txtUTT').addClass('active');
        $('#txtUTAll').removeClass('active');
        SearchGrid_OrCalendar(false);
    });


    self.SelectTrangThaiCongViec = function (item) {
        item.IsSelected = !item.IsSelected;
        self.currentPage(0);
        self.TrangThaiCongViec.refresh();
        SearchGrid_OrCalendar(false);
    }

    self.RemoveTrangThaiKhachHang = function (item) {
        item.IsSelected = !item.IsSelected;
        self.TrangThaiKhachHang.refresh();
        SearchGrid_OrCalendar(false);
    }

    self.SelectTrangThaiKhachHang = function (item) {
        item.IsSelected = !item.IsSelected;
        self.TrangThaiKhachHang.refresh();
        SearchGrid_OrCalendar(false);
    }
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        self.startDate(picker.startDate.format('MM/DD/YYYY'));
        self.endDate(picker.endDate.format('MM/DD/YYYY'));

        SearchGrid_OrCalendar(false);
    });

    $('#SelectLienHeLai').on('click', 'ul li', function () {
        self.TextBirthDate($(this).find('a').text());
        self.TypeBirthDate($(this).val());

        SearchCongViec();
    });

    $('#thoigianlhl ').on('click', '.radio-menu input[type="radio"]', function () {
        var dataid = $(this).data('id');
        $('#thoigianlhl .form-group').each(function () {
            $(this).find('.conten-choose').find('input').removeAttr('disabled');
            if (dataid !== $(this).find('.radio-menu').find('input').data('id')) {
                $(this).find('.conten-choose').find('input').attr('disabled', 'disabled');
            }
        });
        if ($(this).data('id') !== 1) {
            self.TypeBirthDate(null);
        }
        else if (self.TypeBirthDate() === null || self.TypeBirthDate() === undefined) {
            self.TypeBirthDate(0);
        }

        SearchCongViec();
    });

    self.clickSearchCV = function () {
        SearchCongViec();
    }

    self.checkKhachHang.subscribe(function (newVal) {
        self.currentPage(0);
        SearchGrid_OrCalendar(false);
    });

    self.checkNhaCungCap.subscribe(function (newVal) {
        self.currentPage(0);
        SearchGrid_OrCalendar(false);
    });

    $('#txtFilterCongViec').keypress(function (e) {
        if (e.keyCode === 13) {
            self.currentPage(0);
            SearchGrid_OrCalendar(false);
        }
    })

    self.ResetCurrentPage = function () {
        self.currentPage(0);
        SearchGrid_OrCalendar(false);
    };

    self.filterNgayLapHD.subscribe(function (newVal) {
        self.currentPage(0);
        SearchGrid_OrCalendar(false);
    });

    $('#txtNgayTaoInput').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        SearchGrid_OrCalendar(false);
    });

    $('.choseNgayTao li').on('click', function () {
        $('#txtNgayTao').val($(this).text());
        self.filterNgayLapHD_Quy($(this).val());
        self.currentPage(0);
        self.ListRowChosed([]);
        SearchGrid_OrCalendar(false);
    });

    self.PageResults = ko.computed(function () {
        if (self.CongViecs() !== null) {

            self.fromitem((self.currentPage() * self.pageSize()) + 1);

            if (((self.currentPage() + 1) * self.pageSize()) > self.CongViecs().length) {
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

    self.PageCongViec = ko.computed(function () {
        var first = self.currentPage() * self.pageSize();
        if (self.CongViecs() !== null) {
            return self.CongViecs().slice(first, first + self.pageSize());
        }
    });

    function SetCheck_Input() {
        // find in list and set check
        var countCheck = 0;
        $('#table-congviec tr td.check-group input').each(function (x) {
            var id = $(this).attr('id');
            if ($.inArray(id, arrCongViec) > -1) {
                $(this).prop('checked', true);
                countCheck += 1;
            }
            else {
                $(this).prop('checked', false);
            }
        });
        // set again check header
        var ckHeader = $('#table-congviec thead tr th:eq(0) input');
        var lenList = $('#table-congviec tbody tr.prev-tr-hide').length;
        if (countCheck === lenList) {
            ckHeader.prop('checked', true);
        }
        else {
            ckHeader.prop('checked', false);
        }
    }

    self.SetCheck_Input = function () {
        var arrID = [];
        for (let i = 0; i < self.ListRowChosed().length; i++) {
            arrID.push(self.ListRowChosed()[i].ID);
        }
        $('#table-congviec .check-group input').each(function () {
            $(this).prop('checked', $.inArray($(this).attr('id'), arrID) > -1);
        });
        // set check hedear
        let count = 0;
        for (let i = 0; i < self.PageCongViec().length; i++) {
            if ($.inArray(self.PageCongViec()[i].ID, arrID) > -1) {
                count += 1;
            }
        }
        $('#table-congviec .ckHeader').prop('checked', count === self.PageCongViec().length);
        HideShowThaoTac();
    }

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

        self.SetCheck_Input();
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

    self.GoToPage = function (page) {
        if (page.pageNumber !== '.') {
            self.currentPage(page.pageNumber - 1);
            loadHtmlGrid();
        }
    };

    self.StartPage = function () {
        self.currentPage(0);
        loadHtmlGrid();
    }

    self.BackPage = function () {
        if (self.currentPage() > 1) {
            self.currentPage(self.currentPage() - 1);
            loadHtmlGrid();
        }
    }

    self.GoToNextPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.currentPage() + 1);
            loadHtmlGrid();
        }
    }

    self.EndPage = function () {
        if (self.currentPage() < self.PageCount() - 1) {
            self.currentPage(self.PageCount() - 1);
            loadHtmlGrid();
        }
    }
    self.GetClassHD = function (page) {
        return ((page.pageNumber - 1) === self.currentPage()) ? "click" : "";
    };

    // calendar
    self.FromDate = ko.observable();
    self.ToDate = ko.observable();
    var isView = localStorage.getItem('viewCalendar');
    if (isView === null) {
        isView = 'agendaWeek';
    }
    else {
        $('#btnChangeView').find('button').removeClass('active');
        switch (isView) {
            case 'agendaDay':
                $('#btnChangeView').find('button').eq(0).addClass('active');
                break;
            case 'agendaWeek':
                $('#btnChangeView').find('button').eq(1).addClass('active');
                break;
            case 'month':
                $('#btnChangeView').find('button').eq(2).addClass('active');
                break;
        }
    }

    function setTimeline(view) {
        var parentDiv = jQuery(".fc-slats:visible").parent();
        var timeline = parentDiv.children(".timeline");
        if (timeline.length == 0) { //if timeline isn't there, add it
            timeline = jQuery("<p>").addClass("timeline");
            parentDiv.prepend(timeline);
        }

        var curTime = new Date();
        if (view.start._d < curTime && view.end._d > curTime) {
            timeline.show();
        } else {
            timeline.hide();
            return;
        }

        var curSeconds = (curTime.getHours() * 60 * 60) + (curTime.getMinutes() * 60) + curTime.getSeconds();
        var percentOfDay = curSeconds / 86400; //24 * 60 * 60 = 86400, # of seconds in a day
        var topLoc = Math.floor(parentDiv.height() * percentOfDay);

        timeline.css("top", topLoc + "px");
        if (view.type == "agendaWeek") { //week view, don't want the timeline to go the whole way across
            var dayCol = jQuery(".fc-today:visible");
            var left = dayCol.position().left + 1;
            var width = dayCol.width() - 2;
            timeline.css({
                left: left + "px",
                width: width + "px"
            });
        }
        let currentHours = moment().format('HH');
        if (currentHours > 12) {
            $('.fc-scroller').scrollTop(topLoc + currentHours);
            $(document).scrollTop(topLoc);
        }
        else {
            $('.fc-scroller').scrollTop(-topLoc);
        }
    }
    self.AllWork = ko.observableArray();
    var $calendar = $('#calendar');
    var countLoad = 0;
    $calendar.fullCalendar({
        defaultDate: new Date(),
        defaultView: isView,
        editable: true,
        header: false,
        allDay: false,
        buttonText: {
            prev: "Trước",
            next: "Sau",
            today: "Hôm nay",
            month: "Tháng",
            week: "Tuần",
            day: "Ngày",
            list: "Lịch biểu"
        },
        views: {
            week: {
                columnFormat: 'dddd \n DD/MM',  // format header column
            }
        },
        weekLabel: "Tu",
        allDayText: "Cả ngày",
        dayNamesShort: [
            "Chủ nhật",
            "Thứ 2",
            "Thứ 3",
            "Thứ 4",
            "Thứ 5",
            "Thứ 6",
            "Thứ 7"
        ],
        dayNames: [
            "Chủ nhật",
            "Thứ 2",
            "Thứ 3",
            "Thứ 4",
            "Thứ 5",
            "Thứ 6",
            "Thứ 7"
        ],
        monthNames: [
            "Tháng 1",
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
        monthNamesShort: [
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12"
        ],
        events: function (start, end, timezone, callback) {
            var ParamCalendar = GetParamSearch();

            let cacheCongViecTongQuan = localStorage.getItem('CongViecKhachHang');
            let cacheLichHenTongQuan = localStorage.getItem('LichHenKhachHang');
            if (cacheCongViecTongQuan !== null || cacheLichHenTongQuan !== null) {
                if (cacheLichHenTongQuan !== null) {
                    cacheLichHenTongQuan = JSON.parse(cacheLichHenTongQuan);
                    ParamCalendar.PhanLoai = '3';
                    self.IsWork(false);
                    self.IsAppointment(true);
                    self.filterNgayLapHD_Input(cacheLichHenTongQuan.FromDate + ' - ' + cacheLichHenTongQuan.ToDate);

                    ParamCalendar.FromDate = moment(cacheLichHenTongQuan.FromDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
                    ParamCalendar.ToDate = moment(cacheLichHenTongQuan.ToDate, 'DD/MM/YYYY').format('YYYY-MM-DD');
                }
                if (cacheCongViecTongQuan !== null) {
                    cacheCongViecTongQuan = JSON.parse(cacheCongViecTongQuan);
                    ParamCalendar.PhanLoai = '3';
                    self.IsWork(true);
                    self.IsAppointment(false);
                    self.filterNgayLapHD_Input(cacheCongViecTongQuan.FromDate + ' - ' + cacheCongViecTongQuan.ToDate);

                    ParamCalendar.FromDate = moment(cacheCongViecTongQuan.FromDate, 'DD/MM/YYYY').format('YYYY-MM-DD');;
                    ParamCalendar.ToDate = moment(cacheCongViecTongQuan.ToDate, 'DD/MM/YYYY').format('YYYY-MM-DD');;
                }
            }
            else {
                ParamCalendar.FromDate = self.FromDate();
                ParamCalendar.ToDate = self.ToDate();
                localStorage.removeItem('CongViecKhachHang');
                localStorage.removeItem('LichHenKhachHang');
            }
            console.log('ParamCalendar-calendar', ParamCalendar)
            // chi load event khi da load xong lst ChiNhanh (first load)
            if (self.ChiNhanhs().length > 0) {
                ajaxHelper(ChamSocKhachHangs + 'GetListLichHen_FullCalendar', 'POST', ParamCalendar).done(function (x) {
                    self.AllWork(x.data);
                    //console.log('in ', x, ParamCalendar)
                    if (x.res === true) {
                        callback(x.data);
                    }
                    localStorage.removeItem('CongViecKhachHang');
                    localStorage.removeItem('LichHenKhachHang');
                });
            }
        },
        viewRender: function (view, element) {
            let date = '';
            let from = '';
            let to = '';
            let toSearch = '';
            switch (view.type) {
                case 'agendaDay':
                    date = view.start.format('dddd, DD-MM-YYYY'); // 'dd': thứ, DD: ngày, 
                    from = to = moment(view.start).format('YYYY-MM-DD');
                    toSearch = moment(view.start).add('days', 1).format('YYYY-MM-DD');
                    setTimeline(view);
                    break;
                case 'agendaWeek':
                    let start = moment(view.start).format('DD/MM/YYYY');
                    let end = moment(view.end).add('days', -1).format('DD/MM/YYYY');
                    date = start.concat(' - ', end);
                    from = moment(view.start).format('YYYY-MM-DD');
                    to = moment(view.end).format('YYYY-MM-DD');
                    toSearch = moment(view.end).format('YYYY-MM-DD');
                    setTimeline(view);
                    break;
                case 'month':
                    from = moment(view.start).format('YYYY-MM-DD');
                    to = moment(view.end).add('days', -1).format('YYYY-MM-DD');
                    date = moment(view.start).add('days', 7).format('MMMM - YYYY');
                    toSearch = moment(view.end).format('YYYY-MM-DD');
                    break;
            }
            //view.options.scrollTime = moment().format('HH:mm:ss');
            self.FromDate(from);
            self.ToDate(toSearch);
            self.CalendarHeader(date);
        },
        eventAfterAllRender: function (view) {
            switch (isView) {
                case 'month':
                    // if allday = false: show dot
                    $('.fc-day-grid-event').each(function () {
                        let content = $(this).children('.fc-content');
                        if ($(content).find('.fc-time').length > 0) {
                            $(content).css('color', 'black');
                            $(this).prepend('<span class="spandot"> </span>');
                            let bgColor = $(this).css('background-color');
                            $(this).find('.spandot').css('background-color', '' + bgColor + '');
                            $(this).css('background-color', '#ffffff');
                        }
                    });

                    $('.fc-content-skeleton').each(function () {
                        let tr = $(this).find('tbody tr');
                        // nếu số event > 5 --> show more
                        //if ($(tr).length > 5) {
                        //    console.log('tr ', $(tr).length)
                        //    $(this).find('tbody tr:gt(5)').remove();
                        //    let trLast = $(this).find('tbody tr:last-child');
                        //    trLast.find('.fc-title').text($(tr).length - 5 + ' more');
                        //    trLast.find('.fc-title').css('font-style', 'italic');
                        //    trLast.find('.fc-time').remove();
                        //    trLast.find('.spandot').remove();
                        //}
                    });
                    let day = $('.fc-today').text().split(' ');
                    day = day.length > 0 ? day[0] : '';
                    $('.fc-today').text('');
                    $('.fc-today').append('<div class="monthToday">' + day + ' </div>');
                    break;
                case 'agendaWeek':
                    let indexToday = $('.fc-today').index() - 1;
                    let todayHeader = $('.fc-day-header:eq(' + indexToday + ')');
                    $(todayHeader).css('background-color', 'rgb(171, 118, 118)');
                    $(todayHeader).css('color', 'white');
                    countLoad += 1;
                    //if (countLoad === 1) {
                    let from = self.FromDate();
                    let to = self.ToDate();
                    var nvDate = [];
                    var count = 0;
                    while (from < to) {
                        let nvien = '';
                        let arrIDNV = [];
                        for (let i = 0; i < self.AllWork().length; i++) {
                            let itFor = self.AllWork()[i];
                            let start = moment(itFor.NgayGio).format('YYYY-MM-DD');
                            if (start === from) {
                                if ($.inArray(itFor.ID_NhanVien, arrIDNV) === -1) {
                                    arrIDNV.push(itFor.ID_NhanVien);
                                    nvien += '<p class="fc-day-nvien"> ' + itFor.TenNhanVien + ' </p>'
                                }
                            }
                        }
                        from = moment(from).add('days', 1).format('YYYY-MM-DD');
                        count += 1;
                        var objNV = {
                            Column: count,
                            NViens: nvien,
                        }
                        nvDate.push(objNV);
                    }
                    $('.fc-day-grid .fc-content-skeleton td').each(function (index) {
                        let nvInDay = $.grep(nvDate, function (x) {
                            return x.Column === index;
                        });
                        if (nvInDay.length > 0) {
                            $(this).append(nvInDay[0].NViens);
                        }
                    });
                    //}
                    break;
            }
        },
        //eventRender: function (event, element) {
        //    console.log('event', event, 'element', element)

        //},
        dayClick: function (event) {
            var thisDay = moment(event.format());
            partialWork.DateFrom(thisDay.format('DD/MM/YYYY'));
            partialWork.DateTo(partialWork.DateFrom());

            if (event.hasTime()) {
                partialWork.TimeFrom(thisDay.format('HH:mm'));
                partialWork.TimeTo(thisDay.add('hours', 1).format('HH:mm'));
            }
            else {
                // allday
                partialWork.TimeFrom('00:00');
                partialWork.TimeTo('23:59');
            }
            partialWork.DateFromOld(partialWork.DateFrom());
            self.ShowPopup_AddWork(true);

            if (event.hasTime() === false) {
                partialWork.newCongViec().CaNgay(true);
            }
        },
        eventClick: function (event, jsEvent, view) {
            // update
            $('#modalPopuplg_Work').modal('show');
            self.Update_Work(event);
        },
        eventDrop: function (event, dayDelta, minuteDelta, allDay, revertFunc) {// kéo thả event
            // update time start,end to event
            let days = dayDelta._data.days;
            let hours = dayDelta._data.hours;
            let millisenconds = dayDelta._data.milliseconds;

            let startOld = event.NgayGio;
            let momentDay = moment(new Date(startOld));
            let dayNew = momentDay.add(days, 'days');
            let hourNew = momentDay.add(hours, 'hours');
            let timeNew = momentDay.add(millisenconds / 60000, 'seconds');

            let startNew = moment(dayNew).format('YYYY-MM-DD') + ' ' + moment(hourNew).format('HH') + ':' + moment(timeNew).format('mm');
            let diffrenceDateOld = new Date(event.NgayGioKetThuc) - new Date(event.NgayGio);
            let endNew = moment(new Date(startNew).getTime() + diffrenceDateOld).format('YYYY-MM-DD HH:mm');
            //let endNew = moment(new Date(timeNew)).add(1, 'hours').format('YYYY-MM-DD HH:mm');
            if (event.CaNgay) {
                startNew = moment(dayNew).format('YYYY-MM-DD') + ' 00:00';
                endNew = moment(dayNew).format('YYYY-MM-DD') + ' 23:59';
            }
            Drop_ResizeEvent(event, startNew, endNew);
        },
        eventResize: function (event, delta, revertFunc) {// tang/giam thoi gian
            let start = moment(event.start).format('YYYY-MM-DD HH:mm');
            let end = moment(event.end).format('YYYY-MM-DD HH:mm');
            Drop_ResizeEvent(event, start, end);
        },
        eventMouseover: function (event, jsEvent, view) {
            let status = '';
            switch (parseInt(event.TrangThai)) {
                case 1:
                    status = 'Đang xử lý';
                    break;
                case 2:
                    status = 'Hoàn thành';
                    break;
                case 3:
                    status = 'Hủy';
                    break;
            }
            var tooltip = '<div id=\"' + event.id + '\" class="event-hover">' +
                '<div>' +
                '<i class="fa fa-user"></i>' +
                '<span>' + event.TenDoiTuong + ' </span>' +
                '</div >' +
                '<div>' +
                '<i class="fa fa-phone"></i>' +
                '<span>' + event.DienThoai + '</span>' +
                '</div>' +
                '<div>' +
                '<i class="fa fa-tasks"></i>' +
                '<span>' + event.Ma_TieuDe + '</span>' +
                '</div>' +
                '<div>' +
                '<i class="fa fa-question"></i>' +
                '<span>' + status + '</span>' +
                '</div>' +
                '</div >';
            var $tooltip = $(tooltip).appendTo('body');

            $(this).mouseover(function (e) {
                $(this).css('z-index', 10000);
                $tooltip.fadeIn('500');
                $tooltip.fadeTo('10', 1.9);
            }).mousemove(function (e) {
                $tooltip.css('top', e.pageY + 10);
                $tooltip.css('left', e.pageX + 20);
            });
        },
        eventMouseout: function (event, jsEvent, view) {
            $('.event-hover').remove();
        },
        eventRightclick: function (event, jsEvent, view) {
            let divDelete = '<div class="question-delete">' +
                '<span >' +
                '<i class="fa fa-trash"></i>' +
                'Xóa' +
                '</span >' +
                '</div >';
            return false;
        }
    });

    // do chi thay doi thoi gian --> show 2 lua chon: (hide 'allEvent')
    function Drop_ResizeEvent(event, start, end) {
        console.log('start ', start, end)
        let sLoai = 'công việc';
        if (event.PhanLoai === 3) {
            sLoai = 'lịch hẹn';
        }
        let chitiet = 'Cập nhật '.concat(sLoai,
            ' <br/ > - Thời gian cũ: ', moment(event.NgayGio).format('DD/MM/YYYY HH:mm'), ' đến ', moment(event.NgayGioKetThuc).format('DD/MM/YYYY HH:mm'),
            ' <br/ > - Thời gian mới: ', moment(start, 'YYYY-MM-DD HH:mm').format('DD/MM/YYYY HH:mm'), ' đến ', moment(end).format('DD/MM/YYYY HH:mm'));
        let objDiary = {
            ID_NhanVien: idNhanVien,
            ID_DonVi: idDonVi,
            ChucNang: 'Công việc - Lịch hẹn',
            LoaiNhatKy: 2,
            NoiDung: 'Cập nhật '.concat(sLoai, ' <b>', event.Ma_TieuDe, ' </b>'),
            NoiDungChiTiet: chitiet,
        }

        if (event.KieuLap !== 0 && event.PhanLoai === 3) {
            if (event.NgayCu !== null) {
                partialWork.DateFromOld(event.NgayCu);
            }
            else {
                partialWork.DateFromOld(event.NgayGio);
            }
            partialWork.EventOld(event);
            partialWork.TypeUpdate('1'); // always update this event
            //partialWork.Confirm_UpdateEvent(3, function () {
            let myData = {};
            event.ID_LoaiTuVan = event.ID_LoaiTuVan === const_GuidEmpty ? null : event.ID_LoaiTuVan;
            myData.objCongViec = event;
            console.log(123, event);

            let idParent = event.ID_Parent;
            switch (parseInt(partialWork.TypeUpdate())) {
                case 1: // only this event
                    // keep event old, & add new Row
                    myData.objCongViec.ID_Parent = idParent;
                    myData.objCongViec.ID_DonVi = idDonVi;
                    myData.objCongViec.NgayCu = partialWork.DateFromOld();
                    myData.objCongViec.NgayGio = start;
                    myData.objCongViec.NgayGioKetThuc = end;
                    objDiary.NoiDungChiTiet = chitiet.concat(' <br > - Chỉnh sửa sự kiện lặp: chỉ sự kiện này');
                    if (event.ExistDB) {
                        if (event.ID_Parent === event.ID) {
                            partialWork.AddEventDB(myData, sLoai, objDiary);
                        }
                        else {
                            // update
                            let sql3 = " NgayGio='".concat(start, "' , NgayGioKetThuc='", end, "', NguoiSua='", user, "'");
                            partialWork.UpdateEvent_StartEnd(event.ID, sql3);
                        }
                    }
                    else {
                        partialWork.AddEventDB(myData, sLoai, objDiary);
                    }
                    break;
                case 2: // this event & all after
                    let sql = " TrangThaiKetThuc= 2".concat(" , GiaTriKetThuc='", moment(new Date(event.NgayGio)).add(-1, 'days').format('YYYY-MM-DD'), "', NguoiSua='", user, "'");
                    partialWork.UpdateEvent_StartEnd(idParent, sql);

                    objDiary.NoiDungChiTiet = chitiet.concat(' <br > - Chỉnh sửa sự kiện lặp: sự kiện này và các sự kiện sau');
                    myData.objCongViec.ID_Parent = null;
                    myData.objCongViec.ID_DonVi = idDonVi;

                    if (event.ExistDB) {
                        partialWork.UpdateEventDB(myData, sLoai, objDiary);
                    }
                    else {
                        myData.objCongViec.NgayGio = start;
                        partialWork.AddEventDB(myData, sLoai, objDiary);
                    }
                    break;
                case 3: // all event (only update event old)
                    objDiary.NoiDungChiTiet = chitiet.concat(' <br > - Chỉnh sửa sự kiện lặp: tất cả các sự kiện');
                    let sql2 = " NgayGio='".concat(start, "' , NgayGioKetThuc='", end, "', NguoiSua='", user, "'");
                    partialWork.UpdateEvent_StartEnd(event.ID_Parent, sql2);
                    Insert_NhatKyThaoTac_1Param(objDiary);
                    break;

            }
            $("#modal_QuestionUpdateEvent").modal('hide');
            //});
        }
        else {
            let sql = " NgayGio='".concat(start, "' , NgayGioKetThuc='", end, "', NguoiSua='", user, "'");
            //partialWork.UpdateEvent_StartEnd(event.id, sql);

            var obj = {
                ID: event.id,
                SqlSet: sql,
            }
            // nếu chỉ update ThoiGian, không refresh all calendar
            ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/' + 'Event_UpdateStartEnd', 'POST', obj).done(function (x) {
                Insert_NhatKyThaoTac_1Param(objDiary);
                event.NgayGio = start;
                event.NgayGioKetThuc = end;
                $('#calendar').fullCalendar('updateEvent', event);
            });
        }
    }

    self.ChangeKieuBang = function () {
        var $this = event.currentTarget;
        let val = $($this).val().toString();
        self.check_kieubang(val);
        localStorage.setItem('calendarKieuBang', val);
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $($this).find('a').addClass('box-tab');
        if (val === "2") {
            $("#selected-column").hide();
        }
        else {
            $("#selected-column").show();
        }
        //setTimeout(function () {
        //    $calendar.fullCalendar("render");
        //}, 300);
        SearchGrid_OrCalendar(false);
    }

    self.ClickPrev = function (val) {
        $calendar.fullCalendar(val);
    }

    self.ChangeView = function (val) {
        isView = val;
        countLoad = 0;
        var $this = event.currentTarget;
        $($this).siblings('button').removeClass('active');
        $($this).addClass('active');
        $calendar.fullCalendar('changeView', val);
        //switch (val) {
        //    case 'agendaWeek':
        //        $calendar.columnFormat = 'dddd DD/MM'; 
        //        break;
        //}
        localStorage.setItem('viewCalendar', val);
    }

    self.ShowPopup_AddWork = function (isCalendar) {
        $('#nameModelThemMoi').text('Thêm mới công việc/lịch hẹn');
        $('#modalPopuplg_Work').modal('show');

        if (parseInt(self.checkDoiTuong()) == 2) {
            partialWork.IsCustomer(false);
        }
        else {
            partialWork.IsCustomer(true);
        }
        // set default date = now
        if (isCalendar === false) {
            partialWork.DateFrom(moment(dtNow).format('DD/MM/YYYY'));
            partialWork.DateTo(moment(dtNow).format('DD/MM/YYYY'));
            partialWork.TimeFrom(moment(dtNow).format('HH:mm'));
            partialWork.TimeTo(moment(dtNow).add(1, 'hours').format('HH:mm'));
        }

        partialWork.newCongViec(new FormModel_NewCongViec());
        partialWork.booleanAddCV(true);
        partialWork.IsAddTypeWork(true);
        partialWork.ListStaffShare([]);
        partialWork.Calendar_ResetNew();
    }

    self.Update_Work = function (itemCV) {
        $('#nameModelThemMoi').text('Cập nhật công việc/lịch hẹn');
        $('#modalPopuplg_Work').modal('show');
        partialWork.Calendar_ResetText();
        partialWork.Calendar_RemoveCheckAfter();
        partialWork.ResetDateOfWeekChosed();

        if (itemCV.LoaiDoiTuong == 2) {
            partialWork.IsCustomer(false);
        }
        else {
            partialWork.IsCustomer(true);
        }
        partialWork.booleanAddCV(false);

        // typeWork
        if (itemCV.ID_LoaiTuVan != null) {
            partialWork.IsAddTypeWork(false);
        }
        else {
            partialWork.IsAddTypeWork(true);
        }
        partialWork.newCongViec().SetData(itemCV);
        if (itemCV.NgayCu !== null) {
            partialWork.DateFromOld(itemCV.NgayCu);
        }
        else {
            partialWork.DateFromOld(itemCV.NgayGio);
        }
        partialWork.EventOld(itemCV);
    }

    $('#modalPopuplg_Work').on('hidden.bs.modal', function () {
        if (partialWork.SaveSuscess() === 1) {
            SearchGrid_OrCalendar(false);
        }
    });

    $('#modalTypeWork').on('hidden.bs.modal', function () {
        if (partialWork.SaveSuscess() === 2) {
            self.LoaiCongViecs(partialWork.LoaiCongViecs());
        }
    });

    function getAllBrandName() {
        ajaxHelper(ThietLapAPI + 'GetallBrandName', 'GET').done(function (data) {
            data = data.filter(p => p.Status === 1);
            partialSMS.BrandNames(data);
        });
    };

    function GetSoDuTaiKhoan() {
        ajaxHelper(ThietLapAPI + 'GetSoDuCuaTaiKhoan?idnd=' + userID, "GET").done(function (data) {
            partialSMS.SoDuTaiKhoan(data);
        });
    };

    function GetAllSMSMau() {
        ajaxHelper(ThietLapAPI + 'GetAllMauTin', 'GET').done(function (data) {
            partialSMS.AllSMSMaus(data);
            partialSMS.SMSMaus(data);
        });
    };

    function PageLoadSMS() {
        PageLoad();
        getAllBrandName();
        GetSoDuTaiKhoan();
        GetAllSMSMau();
    };

    PageLoadSMS();

    self.ChangeCheck_Header = function () {
        var obj = $(event.currentTarget);
        var isChecked = obj.is(':checked');
        var tbl = obj.closest('table');
        var arrID = [];
        arrIDDoiTuong = [];

        $(tbl).find('tbody .check-group input').each(function () {
            $(this).prop('checked', isChecked);
        });

        for (let i = 0; i < self.PageCongViec().length; i++) {
            arrID.push(self.PageCongViec()[i].ID);
        }

        if (isChecked) {
            for (let i = 0; i < self.PageCongViec().length; i++) {
                self.ListRowChosed.push(self.PageCongViec()[i]);
            }
            let arrUnique = self.ListRowChosed().sort(function (a, b) {
                let x = a.ID, y = b.ID;
                return x > y ? 1 : x < y ? -1 : 0;
            });
            arrUnique = $.unique(arrUnique);
            self.ListRowChosed(arrUnique);
        }
        else {
            let arrAfter = $.grep(self.ListRowChosed(), function (x) {
                return $.inArray(x.ID, arrID) === -1;
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

    self.RemoveAllCheck = function (item) {
        $('#table-congviec input[type=checkbox]').prop('checked', false);
        arrIDDoiTuong = [];
        self.ListRowChosed([]);
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

    self.ShowPopSMS_ManyCustomer = function (item) {
        $('.smsPhone').hide();
        $('#txtNoiDungTin').val('');
        partialSMS.selectedMauTin(undefined);
        partialSMS.SaveSuscess(0);
        if (partialSMS.BrandNames().length === 1) {
            partialSMS.selectedBrandName(partialSMS.BrandNames()[0]);
        }
        partialSMS.LoaiTinNhanGui(4);
        var loaitin = partialSMS.LoaiTins().filter(x => x.ID === 4);
        partialSMS.selectedLoaiTin(loaitin[0]);
        arrIDDoiTuong = arrIDDoiTuong.filter(x => x !== null);
        var obj = {
            ID_NhanVienQuanLys: arrIDDoiTuong,
            CurrentPage: 0,
            PageSize: 0,
            TrangThai: 0,
        }
        ajaxHelper(DMDoiTuongUri + 'GetListCustomer_byIDs', 'POST', obj).done(function (x) {
            partialSMS.Popup_LichHenChosed(x.data);
            $('#modalSMS').modal('show');
        });
    }

    self.ShowPopSMS_byPhone = function (item) {
        $('.smsPhone').show();
        partialSMS.SaveSuscess(0);
        $('#txtNoiDungTin').val('');
        partialSMS.selectedMauTin(undefined);
        $('#sms_txtPhone').val(item.DienThoai);
        if (partialSMS.BrandNames().length === 1) {
            partialSMS.selectedBrandName(partialSMS.BrandNames()[0]);
        }
        partialSMS.Popup_LichHenChosed([item]);
        partialSMS.LoaiTinNhanGui(4);
        var loaitin = partialSMS.LoaiTins().filter(x => x.ID === 4);
        partialSMS.selectedLoaiTin(loaitin[0]);
        $('#modalSMS').modal('show');
    }

    $('#modalSMS').on('hidden.bs.modal', function () {
        if (partialSMS.SaveSuscess() === 2) {
            self.ListRowChosed([]);
            HideShowThaoTac();
        }
    });

    // tab cong viec
    self.TabCongViec_Status = ko.observable(0);
    self.CustomerWorks = ko.observableArray();
    self.Work_FromItem = ko.observable(0);
    self.Work_ToItem = ko.observable(0);

    self.LoadTab_CongViec = function (itemWork) {
        let tr = $('#DiaryWork_' + itemWork.ID);
        $(tr).find('.btn-tab').removeClass('active');
        $(tr).find('.btn-tab').eq(0).addClass('active');
        GetListCongViec_byDoiTuong(itemWork.ID_KhachHang, 0);
    }
    self.ChangeTab_CongViec = function (itemWork, type) {
        var $this = $(event.currentTarget);
        $this.siblings('button').removeClass('active');
        $this.addClass('active');
        $this.css("border-bottom-color");
        self.TabCongViec_Status(0);
        GetListCongViec_byDoiTuong(itemWork.ID_KhachHang, type);
    }
    function GetListCongViec_byDoiTuong(idDoiTuong, type) {
        var arrDV = [];
        for (let i = 0; i < self.MangNhomDV().length; i++) {
            if ($.inArray(self.MangNhomDV()[i], arrDV) === -1) {
                arrDV.push(self.MangNhomDV()[i].ID);
            }
        }
        let arrIDLoaiTV = partialWork.LoaiCongViecs().map(function (x) { return x.ID });
        arrIDLoaiTV.push(const_GuidEmpty);
        let arrNhanVien = self.NhanViens().map(function (x) { return x.ID });
        let from = '2018-01-01';
        let to = '';
        let status = [1];
        switch (type) {
            case 0:// dangthuchien: time >= today + dangxuly (get cv cuoi thang toi)
                from = moment(new Date()).format('YYYY-MM-DD');;
                to = moment().add('months', 1).endOf('month').format('YYYY-MM-DD');
                break;
            case 1:// chuathuchien < today + dangxuly
                to = moment(new Date()).format('YYYY-MM-DD');
                break;
            case 2: // hoanthanh: time <= today & TrangThaiCV= 3
                to = moment(new Date()).add(1, 'days').format('YYYY-MM-DD');
                status = [3];
                break;
        }
        self.TabCongViec_Status(type);
        var model = {
            ID_DonVis: arrDV,
            IDLoaiTuVans: arrIDLoaiTV,
            IDNhanVienPhuTrachs: arrNhanVien,
            TrangThaiCVs: status,
            MucDoUuTien: '%%',
            LoaiDoiTuong: 1,
            PhanLoai: '%%',
            FromDate: from,
            ToDate: to,
            TextSearch: null,
            CurrentPage: 0,
            PageSize: 10,
            ColumnsHide: null,
            TenChiNhanhs: '',
            TypeShow: 1,
            ID_KhachHang: idDoiTuong,
        };
        ajaxHelper('/api/DanhMuc/ChamSocKhachHangAPI/' + 'GetListLichHen_FullCalendar', 'POST', model).done(function (x) {
            if (x.res === true) {
                console.log(x.data)

                self.CustomerWorks(x.data);
                var lenData = x.data.length;
                self.Work_ToItem(lenData);
                if (lenData > 0) {
                    self.Work_FromItem = ko.observable(1);
                }
            }
        });
    }
};

var modelCV = new ViewModal();
ko.applyBindings(modelCV, document.getElementById('divPage'));
var partialWork = new PartialView_CongViec();
ko.applyBindings(partialWork, document.getElementById('modalWork'));
var partialSMS = new PartialView_SMS();
ko.applyBindings(partialSMS, document.getElementById('divSMS'));

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
            "Tháng 1",
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

var arrCongViec = [];
var arrIDDoiTuong = [];
function SetCheckAll(obj) {
    var isChecked = $(obj).is(":checked");
    $('.check-group input[type=checkbox]').each(function () {
        $(this).prop('checked', isChecked);
    })
    if (isChecked) {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            if (thisID !== undefined && !(jQuery.inArray(thisID, arrCongViec) > -1)) {
                arrCongViec.push(thisID);
            }
        });
    }
    else {
        $('.check-group input[type=checkbox]').each(function () {
            var thisID = $(this).attr('id');
            for (var i = 0; i < arrCongViec.length; i++) {
                if (arrCongViec[i] === thisID) {
                    arrCongViec.splice(i, 1);
                    break;
                }
            }
        })
    }

    if (arrCongViec.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrCongViec.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }

}

function ChoseCongViec(obj) {
    var thisID = $(obj).attr('id');

    if ($(obj).is(':checked')) {
        if ($.inArray(thisID, arrCongViec) === -1) {
            arrCongViec.push(thisID);
        }
    }
    else {
        // remove item in arrID
        arrCongViec = arrCongViec.filter(x => x !== thisID);
    }

    if (arrCongViec.length > 0) {
        $('#divThaoTac').show();
        $('.choose-commodity').show().trigger("RemoveClassForButtonNew");
        $('#count').text(arrCongViec.length);
    }
    else {
        $('#divThaoTac').hide();
        $('.choose-commodity').hide().trigger("addClassForButtonNew");
    }

    // count input is checked
    var countCheck = 0;
    $('#table-congviec tr td.check-group input').each(function (x) {
        var id = $(this).attr('id');
        if ($.inArray(id, arrCongViec) > -1) {
            countCheck += 1;
        }
    });

    // set check for header
    var ckHeader = $('#table-congviec thead tr th:eq(0) input');
    var lenList = $('#table-congviec tbody tr.prev-tr-hide').length;
    if (countCheck === lenList) {
        ckHeader.prop('checked', true);
    }
    else {
        ckHeader.prop('checked', false);
    }
}

function RemoveAllCheck() {
    $('input[type=checkbox]').prop('checked', false);
    arrCongViec = [];
    $('#divThaoTac').hide();
    $('.choose-commodity').hide();
}

$('.time-select').on('change', 'input[type=radio][name=rd_TimeReport]', function () {
    if ($(this).val() === "2") {
        $('#filterDate').removeAttr('disabled');
        $('.choose-date-show').attr('disabled', "disabled");
    }
    else {
        $('#filterDate').attr('disabled', "disabled");
        $('.choose-date-show').removeAttr('disabled');
    }
})

function HideLostFocust() {
    $('.divSearchVue').delay(300).hide(0, function () {
    });
}

ko.bindingHandlers.fileUpload = {
    init: function (element, valueAccessor) {
        $(element).change(function () {
            valueAccessor()(element.files[0]);
        });
    },
    update: function (element, valueAccessor) {
        if (ko.unwrap(valueAccessor()) === null) {
            $(element).wrap('<form>').closest('form').get(0).reset();
            $(element).unwrap();
        }
    }
};

$(window.document).on('shown.bs.modal', '.modal', function () {
    window.setTimeout(function () {
        $('[autofocus]', this).focus();
        $('[autofocus]').select();
    }.bind(this), 100);
});
