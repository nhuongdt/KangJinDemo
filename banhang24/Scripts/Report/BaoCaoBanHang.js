var ViewModal = function () {
    var self = this;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/'
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.LoaiBaoCao = ko.observable('hàng hóa')
    self.TypeTime = ko.observable(1);// today

    self.pageSize = ko.observable(10);
    self.pageSizes = [10, 20, 30, 40, 50];

    self.TenNVPrint = ko.observable();
    self.TenKHPrint = ko.observable();

    self.ThoiGianHD_RdoTypeTime = ko.observable('1');
    self.ThoiGianHD_TypeTime = ko.observable('homnay');
    self.ThoiGianHD_TypeTimeText = ko.observable('Hôm nay');
    self.ThoiGianHD_DateRangeText = ko.observable();

    self.Customer_DoanhThuTu = ko.observable('');
    self.Customer_DoanhThuDen = ko.observable(null);
    self.Customer_SoLanDenFrom = ko.observable(0);
    self.Customer_SoLanDenTo = ko.observable(null);
    self.Customer_TrangThai = ko.observable('2');

    self.BaoCaoBanHang_TongHop_Print = ko.observableArray();
    self.BaoCaoBanHang_ChiTiet_Print = ko.observableArray();
    self.BaoCaoBanHang_TheoNhomHang_Print = ko.observableArray();
    self.BaoCaoBanHang_TheoKhachHang_Print = ko.observableArray();
    self.BaoCaoBanHang_TheoNhanVien_Print = ko.observableArray();
    self.BaoCaoBanHang_HangTraLai_Print = ko.observableArray();
    self.BaoCaoBanHang_LoiNhuan_Print = ko.observableArray();
    self.BaoCaoKhachHang_TanSuat = ko.observableArray();
    self.BaoCaoHangKhuyenMai = ko.observableArray();
    self.BaoCaoBanHang_DinhDanhDichVu = ko.observableArray();
    self.role_BCBH_DinhDanhDichVu = ko.observable(true);

    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray([_id_DonVi]);
    self.LoaiNganhNghe = ko.observable(0);
    //load Loai Chung Tu
    self.MangChungTu = ko.observableArray();
    self.ChungTus = ko.observableArray();
    self.searchChungTu = ko.observableArray();

    var _idChungTuSeach = '1,2,6,36';

    switch (VHeader.IdNganhNgheKinhDoanh.toUpperCase()) {
        case 'AC9DF2ED-FF08-488F-9A64-08433E541020':
            self.LoaiNganhNghe(0);//spa + banle
            getListDM_LoaiChungTuBanHang('1,2,6,36');
            break;
        case 'C16EDDA0-F6D0-43E1-A469-844FAB143014':
            self.LoaiNganhNghe(1);//gara
            _idChungTuSeach = '1,19,25';
            getListDM_LoaiChungTuBanHang('1,25,6');
            break;
        case 'C1D14B5A-6E81-4893-9F73-E11C63C8E6BC':
            self.LoaiNganhNghe(2);//nhahang
            getListDM_LoaiChungTuBanHang('1,2,6');
            break;
    }

    var _rdTime = 1;
    // TuanDL Cache Show Hide Column Grid
    self.listCheckbox = ko.observableArray();
    self.columnCheckType = ko.observable(1);
    var Key_Form = 'Key_ReportSales';

    self.loadCheckbox = function (type) {
        self.columnCheckType(type);
        $.getJSON("api/DanhMuc/ReportAPI/GetChecked?type=" + type + "&group=" + $('#ID_loaibaocao').val(), function (data) {
            switch (type) {
                case 2:
                    loadHtmlGrid();
                    break;
                case 10: // bcdinhdanh: c Huyen bảo bỏ 2 cột này (27/06/2024)
                    data = data.filter((x) => $.inArray(x.Key, ['Detail_maDinhDanhDV', 'Detail_donvitinh']) == -1)
                    break;
            }
            self.listCheckbox(data);
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
    self.loadCheckbox(1);
    var IsLoadFirst = true;
    function addcacheFirst() {
        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#ID_duchitiet').val(), [{
            NameClass: "Detail_nhomkhach",
            Value: 2
        },
        {
            NameClass: "Detail_nguonkhach",
            Value: 3
        },
        //{
        //    NameClass: "Detail_gioitinh",
        //    Value: 5
        //},
        {
            NameClass: "Detail_nguoigioithieu",
            Value: 6
        },
        {
            NameClass: "Detail_nhomhang",
            Value: 7
        }
        ]);
        LocalCaches.CheckColumnGridWithObj(Key_Form + $('#rp_khuyenmai').val(), [
            {
                NameClass: "madoituong",
                Value: 4
            },
            {
                NameClass: "nguoitao",
                Value: 11
            },
            {
                NameClass: "nvban",
                Value: 12
            },
        ]);
    }
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
                            if (valueCheck.toLowerCase().indexOf("_lohang") >= 0) {
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
                    if (self.columnCheckType() !== 2) {
                        localStorage.removeItem(Key_Form + self.columnCheckType());
                        if (current === "false") {
                            $('#select-column .dropdown-list ul li').each(function (i) {
                                var valueCheck = $(this).find('input[type = checkbox]').val();
                                if (valueCheck.toLowerCase().indexOf("_lohang") >= 0) {
                                    LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, i);
                                }
                            });
                        }
                        localStorage.setItem(KeyLo, current);
                    }
                }
            }
            LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
            $('.table-reponsive').css('display', 'block');

        }
    }
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
    //--- End TuanDl
    self.MoiQuanTam = ko.observable('Báo cáo bán hàng tổng hợp');
    var dt1 = new Date();
    var _timeStart = dt1.getFullYear() + "-" + (dt1.getMonth() + 1) + "-" + dt1.getDate();
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    self.TodayBC = ko.observable('Ngày bán: ' + moment(_timeStart).format('DD/MM/YYYY'));
    self.check_MoiQuanTam = ko.observable(1);
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.SumNumberPageReport_CT = ko.observableArray();
    self.SumNumberPageReport_CTKH = ko.observableArray();
    self.RowsStart_CT = ko.observable('1');
    self.RowsEnd_CT = ko.observable('10');
    self.RowsStart_CTKH = ko.observable('1');
    self.RowsEnd_CTKH = ko.observable('10');
    var _pageNumber = 1;
    var _pageSize = 10;
    var _pageNumber_CT = 1;
    var _pageSize_CT = 10;
    var _pageNumber_CTKH = 1;
    var _pageSize_CTKH = 10;
    self.SumRowsHangHoa = ko.observable();
    self.SumRowsHangHoa_CT = ko.observable();
    self.SumRowsHangHoa_CTKH = ko.observable();


    var AllPage;
    var AllPage_CT;
    var AllPage_CTKH;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    self.pageNumber_TH = ko.observable(1);
    self.pageNumber_CT = ko.observable(1);
    self.pageNumber_NKH = ko.observable(1);
    self.pageNumber_KH = ko.observable(1);
    self.pageNumber_NH = ko.observable(1);
    self.pageNumber_NV = ko.observable(1);
    self.pageNumber_HTL = ko.observable(1);
    self.pageNumber_LN = ko.observable(1);
    self.pageNumber_KM = ko.observable(1);
    self.pageNumber_CTNV = ko.observable(1);
    self.pageNumber_CTKH = ko.observable(1);
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.LoaiSP_CB = ko.observable(true);
    $('.ip_TimeReport').val("Hôm nay");
    self.Loc_TinhTrangKD = ko.observable('2');
    self.Loc_TinhTrangBH = ko.observable('1');
    var tk = null;
    self.NhomHangHoas = ko.observableArray();
    //trinhpv phân quyền
    self.BaoCaoBanHang = ko.observable();
    self.BCBH_TongHop = ko.observable();
    self.BCBH_ChiTiet = ko.observable();
    self.BCBH_TheoNhomHang = ko.observable();
    self.BCBH_TheoKhachHang = ko.observable();
    self.BCBH_TheoNhomKhachHang = ko.observable();
    self.BCBH_TheoNhanVien = ko.observable();
    self.BCBH_HangTraLai = ko.observable();
    self.BCBH_LoiNhuanTheoMatHang = ko.observable();
    self.BCBH_XuatFile = ko.observable();
    self.HangHoa_XemGiaVon = ko.observable();

    self.Role_BaoCaoBanHang_TheoKhachHang = ko.observable(false);

    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BaoCaoBanHang", "GET").done(function (data) {
        //    self.BaoCaoBanHang(data);
        //})
        if (VHeader.Quyen.indexOf('BaoCaoBanHang') > -1) {
            self.BaoCaoBanHang('BaoCaoBanHang');
        }
        else {
            self.BaoCaoBanHang('false');
        }

        if (VHeader.Quyen.indexOf('BCBH_TongHop') > -1) {
            self.BCBH_TongHop('BCBH_TongHop');
            getDonVi();
        }
        else {
            self.BCBH_TongHop('false');
        }

        if (VHeader.Quyen.indexOf('BCBH_ChiTiet') > -1) {
            self.BCBH_ChiTiet('BCBH_ChiTiet');

        }
        else {
            self.BCBH_ChiTiet('false');
        }

        self.role_BCBH_DinhDanhDichVu(VHeader.Quyen.indexOf('BCBH_DinhDanhDichVu') > -1);

        if (VHeader.Quyen.indexOf('BCBH_TheoNhomHang') > -1) {
            self.BCBH_TheoNhomHang('BCBH_TheoNhomHang');

        }
        else {
            self.BCBH_TheoNhomHang('false');
        }

        if (VHeader.Quyen.indexOf('BCBH_TheoKhachHang') > -1) {
            self.BCBH_TheoKhachHang('BCBH_TheoKhachHang');
            self.Role_BaoCaoBanHang_TheoKhachHang(true);

        }
        else {
            self.BCBH_TheoKhachHang('false');
            self.Role_BaoCaoBanHang_TheoKhachHang(false);
        }

        if (VHeader.Quyen.indexOf('BCBH_TheoNhomKhachHang') > -1) {
            self.BCBH_TheoNhomKhachHang('BCBH_TheoNhomKhachHang');
        }
        else {
            self.BCBH_TheoNhomKhachHang('false');
        }

        if (VHeader.Quyen.indexOf('BCBH_TheoNhanVien') > -1) {
            self.BCBH_TheoNhanVien('BCBH_TheoNhanVien');

        }
        else {
            self.BCBH_TheoNhanVien('false');
        }

        if (VHeader.Quyen.indexOf('BCBH_HangTraLai') > -1) {
            self.BCBH_HangTraLai('BCBH_HangTraLai');

        }
        else {
            self.BCBH_HangTraLai('false');
        }

        if (VHeader.Quyen.indexOf('BCBH_LoiNhuanTheoMatHang') > -1) {
            self.BCBH_LoiNhuanTheoMatHang('BCBH_LoiNhuanTheoMatHang');

        }
        else {
            self.BCBH_LoiNhuanTheoMatHang('false');
        }

        if (VHeader.Quyen.indexOf('BCBH_XuatFile') > -1) {
            self.BCBH_XuatFile('BCBH_XuatFile');
        }
        else {
            self.BCBH_XuatFile('false');
        }

        if (VHeader.Quyen.indexOf('HangHoa_XemGiaVon') > -1) {
            self.HangHoa_XemGiaVon('HangHoa_XemGiaVon');
        }
        else {
            self.HangHoa_XemGiaVon('false');
        }
    }
    getQuyen_NguoiDung();

    // load Nhóm khách hàng
    var _tennhomDT = null;
    var time = null
    var _tenNhomDoiTuongSeach = null;
    self.NhomDoiTuongs = ko.observableArray();
    function getList_NhomDoiTuongs() {
        ajaxHelper(ReportUri + "GetListID_NhomDoiTuong?TenNhomDoiTuong=" + _tennhomDT + "&loaidoituong=1", "GET").done(function (data) {
            self.NhomDoiTuongs(data);
            self.searchNhomDoiTuong(data);
            for (var i = 0; i < self.NhomDoiTuongs().length; i++) {
                _tenNhomDoiTuongSeach = self.NhomDoiTuongs()[i].ID + "," + _tenNhomDoiTuongSeach;
            }
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
            for (var i = 0; i < self.searchNhomDoiTuong().length; i++) {
                _tenNhomDoiTuongSeach = self.searchNhomDoiTuong()[i].ID + "," + _tenNhomDoiTuongSeach;
            }
        }
        // remove check
        $('#selec-all-NhomDoiTuong li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        _pageNumber = 1;
        self.LoadReport();
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
        self.LoadReport();
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
                    //self.MangChiNhanh.push(self.DonVis()[i]);
                    //self.LstIDDonVi([VHeader.IdDonVi]);
                    //self.TenChiNhanh(VHeader.TenDonVi);
                    //self.ArrDonVi(self.DonVis()[i]);
                }
            }
            self.ArrDonVi(self.DonVis());
            self.LoadReport();
        });
    }
    //Lua chon don vi
    self.CloseDonVi = function (item) {
        var TenChiNhanh = '';
        var arrID = [];
        self.MangChiNhanh.remove(item);
        if (item.ID === '00000000-0000-0000-0000-0000-000000000000') {
            arrID = $.map(self.DonVis(), function (x) {
                return x.ID;
            });
            TenChiNhanh = 'Tất cả chi nhánh';
        }
        else {
            _idDonViSeach = null;
            self.ArrDonVi.unshift(item);

            if (self.MangChiNhanh().length === 0) {
                $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
                TenChiNhanh = 'Tất cả chi nhánh.'
                for (var i = 0; i < self.searchDonVi().length; i++) {
                    if (_idDonViSeach == null)
                        _idDonViSeach = self.searchDonVi()[i].ID;
                    else
                        _idDonViSeach = self.searchDonVi()[i].ID + "," + _idDonViSeach;
                }
                arrID = $.map(self.DonVis(), function (x) {
                    return x.ID;
                });
            }
            else {
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
                arrID = $.map(self.MangChiNhanh(), function (x) {
                    return x.ID;
                });
            }
            $('#selec-all-DonVi li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                }
            });
        }
        self.TenChiNhanh(TenChiNhanh);
        self.LstIDDonVi(arrID);
        self.LoadReport();
    }

    self.SelectedDonVi = function (item, event) { /*đặt event sau biến*/
        /*var e = event || window.event;*/
        if (event !== undefined) {
            event.stopPropagation();
        }
        _idDonViSeach = null;
        var TenChiNhanh = '';
        var arrIDDonVi = [];
        // all
        if (item.ID === undefined) {
            let arrID = $.map(self.DonVis(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);
            arrIDDonVi = self.ArrDonVi().map(function (x) {
                return x.ID;
            });
            // push again lstDV has chosed
            for (let i = 0; i < self.MangChiNhanh().length; i++) {
                if ($.inArray(self.MangChiNhanh()[i].ID, arrIDDonVi) === -1 && self.MangChiNhanh()[i].ID !== '00000000-0000-0000-0000-0000-000000000000') {
                    self.ArrDonVi().unshift(self.MangChiNhanh()[i]);
                }
            }
            self.MangChiNhanh([{
                ID: '00000000-0000-0000-0000-0000-000000000000', TenDonVi: 'Tất cả chi nhánh'
            }]);
            TenChiNhanh = 'Tất cả chi nhánh';
        }
        else {
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if ($.inArray(self.MangChiNhanh()[i].ID, arrIDDonVi) === -1) {
                    arrIDDonVi.push(self.MangChiNhanh()[i].ID);
                }
                if (self.MangChiNhanh()[i].ID === '00000000-0000-0000-0000-0000-000000000000') {
                    self.MangChiNhanh().splice(i, 1);
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
            }

            //thêm dấu check vào đối tượng được chọn
            $('#selec-all-DonVi li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>')
                }
            });
            let arrID = $.map(self.MangChiNhanh(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);
        }
        self.TenChiNhanh(TenChiNhanh);
        // remove donvi has chosed in lst
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        if (event !== undefined) {
            event.preventDefault();
        }
        /*e.preventDefault();*/
        return false;
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
    //nhóm hàng
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
            //var objParentMD = {
            //    ID: '00000000-0000-0000-0000-000000000000',
            //    TenNhomHangHoa: 'Nhóm mặc định',
            //    Childs: [],
            //}
            //self.NhomHangHoas.unshift(objParentMD);
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
        self.LoadReport();
    }
    $('.SelectALLNhomHang').on('click', function () {
        _ID_NhomHang = null;
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.chose_TinhTrangKD input').on('click', function () {
        TinhTrangHH = $(this).val();
        _pageNumber = 1;
        self.Loc_TinhTrangKD($(this).val());
        self.LoadReport();
    });
    $('.chose_TinhTrangBH input').on('click', function () {
        //TinhTrangBH = $(this).val();
        _pageNumber = 1;
        if ($(this).val() == 1) {
            TinhTrangBH = '%';
        }
        else if ($(this).val() == 2) {
            TinhTrangBH = '%Còn hạn%';
        }
        else
            TinhTrangBH = '%Hết hạn%'
        self.Loc_TinhTrangBH($(this).val());
        self.LoadReport();
    });
    $('.chose_kieubang').on('click', 'li', function () {
        $('.Show_NhomKhachHang').hide();
        $('.showNameKH').hide();
        $('.showGioiThieu').hide();
        $('.showQuanLy,showDePartment').hide();
        $('#showhideBaoHanh').hide();

        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        self.check_MoiQuanTam($(this).find('a input').val());

        var dataid = parseInt($(this).data('id'));
        if (dataid == 4) {
            dataid = 52; //bckhachhang tonghop
            self.check_MoiQuanTam(52);
        }
        self.loadCheckbox(dataid);

        _pageNumber = 1;
        self.LoadReport();
    });

    self.BaoCaoBanHangTitle = function () {
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
                self.LoaiBaoCao('hàng hóa');
                self.MoiQuanTam('Báo cáo bán hàng tổng hợp');
                break;
            case 2:
                $('#showhideBaoHanh').show();
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã chứng từ, ghi chú").blur();
                self.LoaiBaoCao('hàng hóa chi tiết');
                self.MoiQuanTam('Báo cáo bán hàng chi tiết');
                break;
            case 3:
                $("#txt_search").attr("placeholder", "Theo tên nhóm hàng").blur();
                self.LoaiBaoCao('nhóm hàng hóa');
                self.MoiQuanTam('Báo cáo bán hàng theo nhóm hàng');
                break;
            case 4:
                self.LoaiBaoCao('khách hàng');
                $('.Show_NhomKhachHang').show();
                $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại khách hàng, người giới thiệu");
                self.MoiQuanTam('Báo cáo chi tiết bán hàng theo khách hàng');
                break;
            case 52:
                $('#kh_chitiet').removeClass('active');
                self.LoaiBaoCao('khách hàng');
                $('.Show_NhomKhachHang').show();
                $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại khách hàng");
                self.MoiQuanTam('Báo cáo tổng hợp bán hàng theo khách hàng');
                break;
            case 5:
                self.MoiQuanTam('Báo cáo bán hàng theo nhóm khách hàng');
                break;
            case 6:
                $("#txt_search").attr("placeholder", "Theo tên nhân viên").blur();
                $('.showDePartment').show();
                self.LoaiBaoCao('nhân viên');
                self.MoiQuanTam('Báo cáo bán hàng theo nhân viên');
                break;
            case 7:
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã chứng từ, ghi chú").blur();
                self.LoaiBaoCao('hàng hóa chi tiết');
                self.MoiQuanTam('Báo cáo hàng hóa trả lại');
                break;
            case 8:
                self.LoaiBaoCao('hàng hóa');
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng").blur();
                self.MoiQuanTam('Báo cáo lợi nhuận theo mặt hàng');
                break;
            case 9:
                self.LoaiBaoCao('hàng khuyến mại');
                $("#txt_search").attr("placeholder", "Theo mã/tên hàng, mã/tên khuyến mại, mã/tên khách hàng, hình thức ").blur();
                self.MoiQuanTam('Báo cáo hàng khuyến mại');
                break;
            case 10:
                self.LoaiBaoCao('định danh dịch vụ');
                $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên khách hàng").blur();
                self.MoiQuanTam('Báo cáo định danh dịch vụ');
                break;
        }
    };

    self.clearTabalePrint = function () {
        self.BaoCaoBanHang_TongHop_Print([]);
        self.BaoCaoBanHang_ChiTiet_Print([]);
        self.BaoCaoBanHang_TheoNhomHang_Print([]);
        self.BaoCaoBanHang_TheoKhachHang_Print([]);
        self.BaoCaoBanHang_TheoNhanVien_Print([]);
        self.BaoCaoBanHang_HangTraLai_Print([]);
        self.BaoCaoBanHang_LoiNhuan_Print([]);
    }
    var Text_search = "";
    var _maKhachHang = "";
    var _magioithieu = "";
    var _maquanly = "";
    var TinhTrangHH = 2;
    var TinhTrangBH = '%';
    var _ID_NhomHang = null;
    self.BaoCaoBanHang_TongHop = ko.observableArray();
    self.BaoCaoBanHang_ChiTiet = ko.observableArray();
    self.BaoCaoBanHang_TheoNhomHang = ko.observableArray();
    self.BaoCaoBanHang_TheoKhachHang = ko.observableArray();
    self.BaoCaoBanHang_TheoNhomKhachHang = ko.observableArray();
    self.BaoCaoBanHang_TheoNhanVien = ko.observableArray();
    self.BaoCaoBanHang_HangTraLai = ko.observableArray();
    self.BaoCaoBanHang_LoiNhuan = ko.observableArray();
    self.TH_SoLuong = ko.observable();
    self.TH_ThanhTien = ko.observable();
    self.TH_TienVon = ko.observable();
    self.TH_GiamGiaHD = ko.observable();
    self.TH_DoanhThuThuan = ko.observable();// dungchung truong nay cho all baocao
    self.TH_TienThue = ko.observable();
    self.TH_TongChiPhi = ko.observable();

    self.TH_LaiLo = ko.observable();
    self.CT_SoLuong = ko.observable();
    self.CT_ThanhTien = ko.observable();
    self.CT_TienVon = ko.observable();
    self.CT_GiamGiaHD = ko.observable();
    self.CT_LaiLo = ko.observable();

    self.TNH_SoLuong = ko.observable();
    self.TNH_ThanhTien = ko.observable();
    self.TNH_TienVon = ko.observable();
    self.TNH_GiamGiaHD = ko.observable();
    self.TNH_LaiLo = ko.observable();
    self.KH_SoLuong = ko.observable();
    self.KH_ThanhTien = ko.observable();
    self.KH_TienVon = ko.observable();
    self.KH_GiamGiaHD = ko.observable();
    self.KH_LaiLo = ko.observable();
    self.NV_SoLuongBan = ko.observable();
    self.NV_ThanhTien = ko.observable();
    self.NV_SoLuongTra = ko.observable();
    self.NV_GiaTriTra = ko.observable();
    self.NV_TienVon = ko.observable();
    self.NV_GiamGiaHD = ko.observable();
    self.NV_LaiLo = ko.observable();
    self.HTL_SoLuongBan = ko.observable();
    self.HTL_ThanhTien = ko.observable();
    self.HTL_GiamGiaHD = ko.observable();
    self.HTL_GiaTriTra = ko.observable();
    self.LN_SoLuongBan = ko.observable();
    self.LN_ThanhTien = ko.observable();
    self.LN_SoLuongTra = ko.observable();
    self.LN_GiaTriTra = ko.observable();
    self.LN_TienVon = ko.observable();
    self.LN_GiamGiaHD = ko.observable();
    self.LN_LaiLo = ko.observable();
    self.LN_DoanhThuThuan = ko.observable();
    self.LN_TySuat = ko.observable();
    function LoadingForm(IsShow) {
        $('.tab-show .tab-pane').each(function () {
            if ($(this).hasClass('active')) {
                var top = $(this).find('.table-reponsive').height() / 2;
                var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                $(this).find('.table-reponsive').eq(0).gridLoader({ show: IsShow, style: style });
            }
        });
    }


    function getListDM_LoaiChungTuBanHang(item) {
        self.MangChungTu([]);
        ajaxHelper(ReportUri + "getListDM_LoaiChungTu?LoaiChungTu=" + item).done(function (data) {
            self.ChungTus(data);
            self.searchChungTu(data);
            for (var i = 0; i < data.length; i++) {
                if (i == 0)
                    _idChungTuSeach = data[i].ID;
                else
                    _idChungTuSeach = _idChungTuSeach + "," + data[i].ID;
            }
        });
    }

    self.CloseChungTu = function (item) {
        _idChungTuSeach = null;
        self.MangChungTu.remove(item);
        for (var i = 0; i < self.MangChungTu().length; i++) {
            if (_idChungTuSeach == null) {
                _idChungTuSeach = self.MangChungTu()[i].ID;
            }
            else {
                _idChungTuSeach = self.MangChungTu()[i].ID + "," + _idChungTuSeach;
            }
        }
        if (self.MangChungTu().length === 0) {
            $("#NoteNameChungTu").attr("placeholder", "Chọn chứng từ...");
            for (var i = 0; i < self.searchChungTu().length; i++) {
                if (_idChungTuSeach == null)
                    _idChungTuSeach = self.searchChungTu()[i].ID;
                else
                    _idChungTuSeach = self.searchChungTu()[i].ID + "," + _idChungTuSeach;
            }
        }
        $('#selec-all-ChungTu li').each(function () {
            if ($(this).attr('id') == item.ID) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }
    self.SelectedChungTu = function (item) {
        _idChungTuSeach = null;
        _pageNumber = 1;
        var arrIDChungTu = [];
        for (var i = 0; i < self.MangChungTu().length; i++) {
            if ($.inArray(self.MangChungTu()[i], arrIDChungTu) === -1) {
                arrIDChungTu.push(self.MangChungTu()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDChungTu) === -1) {
            self.MangChungTu.push(item);
            $('#NoteNameChungTu').removeAttr('placeholder');
            for (var i = 0; i < self.MangChungTu().length; i++) {
                if (_idChungTuSeach == null) {
                    _idChungTuSeach = self.MangChungTu()[i].ID;
                }
                else {
                    _idChungTuSeach = self.MangChungTu()[i].ID + "," + _idChungTuSeach;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-ChungTu li').each(function () {
            if ($(this).attr('id') == item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    self.NoteNameChungTu = function () {
        var arrChungTu = [];
        var itemSearch = locdau($('#NoteNameChungTu').val().toLowerCase());
        for (var i = 0; i < self.searchChungTu().length; i++) {
            var locdauInput = locdau(self.searchChungTu()[i].TenChungTu).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrChungTu.push(self.searchChungTu()[i]);
            }
        }
        self.ChungTus(arrChungTu);
        if ($('#NoteNameChungTu').val() == "") {
            self.ChungTus(self.searchChungTu());
        }
    }
    $('#NoteNameChungTu').keypress(function (e) {
        if (e.keyCode == 13 && self.ChungTus().length > 0) {
            self.SelectedChungTu(self.ChungTus()[0]);
        }
    });

    function MenuLeft_ShowSearchHang() {
        $('.showNhomHang, .showLoaiHang, .showTrangThaiHang').show();
    }

    function MenuLeft_ShowSearchCustomer() {
        $('.cutomer_DoanhThu, .cutomer_SoLanDen, .cutomer_TrangThai, .customer_NgayGiaoDichMax, .customer_NgayTaoKH').show();
    }

    self.ReportKhachHang_ChangeType = function (isDetail) {
        if (isDetail) {
            self.check_MoiQuanTam(4);
            self.loadCheckbox(4);
            _pageNumber = 1;
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại khách hàng, người giới thiệu").blur();
            self.MoiQuanTam('Báo cáo chi tiết bán hàng theo khách hàng');
        }
        else {
            self.check_MoiQuanTam(52);
            self.loadCheckbox(52);
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại khách hàng");
            self.MoiQuanTam('Báo cáo tổng hợp bán hàng theo khách hàng');
        }
        $('#khachhang').addClass('active');
        self.LoadReport();
    }

    self.LoadReport = function () {
        LoadingForm(true);
        self.BaoCaoBanHangTitle();
        //$('.table-reponsive').css('display', 'none');
        $('.showNhomHang, .showLoaiHang, .showTrangThaiHang').hide();
        $('.cutomer_DoanhThu, .cutomer_SoLanDen, .cutomer_TrangThai, .customer_NgayGiaoDichMax,.customer_NgayTaoKH').hide();
        $("#iconSort").remove();
        self.clearTabalePrint();
        _pageNumber_CT = 1;
        self.pageNumber_CTNV(_pageNumber_CT);
        self.pageNumber_TH(1);
        self.pageNumber_CT(1);
        self.pageNumber_NH(1);
        self.pageNumber_KH(1);
        self.pageNumber_NV(1);
        self.pageNumber_HTL(1);
        self.pageNumber_LN(1);
        self.pageNumber_KM(1);

        var fromDate = '', toDate = '';
        if (parseInt(self.ThoiGianHD_RdoTypeTime()) === 1) {
            let obj = GetDate_FromTo(self.ThoiGianHD_TypeTime());
            fromDate = obj.FromDate;
            toDate = moment(obj.ToDate, 'YYYY-MM-DD').add('days', 1).format('YYYY-MM-DD');
        }
        else {
            let arrDate = self.ThoiGianHD_DateRangeText().split('-');
            fromDate = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            toDate = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
        }
        if (!commonStatisJs.CheckNull(Text_search)) {
            Text_search = Text_search.trim();
        }
        var array_Seach = {
            MaHangHoa: Text_search,
            MaKhachHang: _maKhachHang,
            NV_GioiThieu: _magioithieu,
            NV_QuanLy: _maquanly,
            timeStart: fromDate,
            timeEnd: toDate,
            ID_ChiNhanh: _idDonViSeach,
            LoaiHangHoa: GetLoaiHang(),
            TinhTrang: TinhTrangHH,
            ID_NhomHang: _ID_NhomHang,
            ID_NhomKhachHang: _tenNhomDoiTuongSeach,
            LoaiChungTu: _idChungTuSeach,
            HanBaoHanh: TinhTrangBH,
            ID_NguoiDung: _IDDoiTuong,
            pageNumber: _pageNumber,
            pageSize: _pageSize,
            columnsHide: null,
            TodayBC: null,
            TenChiNhanh: null,
            lstIDChiNhanh: self.LstIDDonVi(),
        }

        var arr = self.ChungTus().filter(x => x.ID !== 22);
        self.ChungTus(arr);
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                MenuLeft_ShowSearchHang();
                if (self.BCBH_TongHop() == "BCBH_TongHop") {
                    $(".PhanQuyen").hide();
                    ajaxHelper(ReportUri + "BaoCaoBanHang_TongHop", "POST", array_Seach).done(function (data) {
                        self.BaoCaoBanHang_TongHop(data.LstData);
                        if (self.BaoCaoBanHang_TongHop().length != 0) {
                            $('.TC_TongHop').show();
                            $(".Report_Empty").hide();
                            $(".page").show();
                            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                            self.RowsEnd((_pageNumber - 1) * _pageSize + self.BaoCaoBanHang_TongHop().length)
                        }
                        else {
                            $('.TC_TongHop').hide();
                            $(".Report_Empty").show();
                            $(".page").hide();
                            self.RowsStart('0');
                            self.RowsEnd('0');
                        }
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.TH_SoLuong(data.a1);
                        self.TH_ThanhTien(data.a2);
                        self.TH_TienVon(data.a3);
                        self.TH_GiamGiaHD(data.a4);
                        self.TH_LaiLo(data.a5);
                        self.TH_DoanhThuThuan(data.TongDoanhThuThuan);
                        self.TH_TienThue(data.SumTienThue);
                        self.TH_TongChiPhi(data.SumChiPhi);
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
                break;
            case 2:
                MenuLeft_ShowSearchHang();
                if (self.BCBH_ChiTiet() == "BCBH_ChiTiet") {
                    $(".PhanQuyen").hide();
                    ajaxHelper(ReportUri + "BaoCaoBanHang_ChiTiet", "POST", array_Seach).done(function (data) {

                        self.BaoCaoBanHang_ChiTiet(data.LstData);
                        if (self.BaoCaoBanHang_ChiTiet().length != 0) {
                            $('.TC_ChiTiet').show();
                            $(".Report_Empty").hide();
                            $(".page").show();
                            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                            self.RowsEnd((_pageNumber - 1) * _pageSize + self.BaoCaoBanHang_ChiTiet().length)
                        }
                        else {
                            $('.TC_ChiTiet').hide();
                            $(".Report_Empty").show();
                            $(".page").hide();
                            self.RowsStart('0');
                            self.RowsEnd('0');
                        }
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.CT_SoLuong(data.a1);
                        self.CT_ThanhTien(data.a2);
                        self.CT_TienVon(data.a3);
                        self.CT_GiamGiaHD(data.a4);
                        self.CT_LaiLo(data.a5);
                        self.TH_DoanhThuThuan(data.TongDoanhThuThuan);
                        self.TH_TienThue(data.SumTienThue);
                        self.TH_TongChiPhi(data.SumChiPhi);
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
                break;
            case 10:
                MenuLeft_ShowSearchHang();

                ajaxHelper(ReportUri + "BaoCaoBanHang_DinhDanhDichVu", "POST", array_Seach).done(function (data) {
                    self.BaoCaoBanHang_DinhDanhDichVu(data.LstData);
                    console.log(data)
                    if (self.BaoCaoBanHang_DinhDanhDichVu().length > 0) {
                        self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                        self.RowsEnd((_pageNumber - 1) * _pageSize + self.BaoCaoBanHang_DinhDanhDichVu().length)
                    }
                    else {
                        self.RowsStart('0');
                        self.RowsEnd('0');
                    }
                    AllPage = data.numberPage;
                    self.SumRowsHangHoa(data.TotalRow);

                    self.selecPage();
                    self.ReserPage();
                    LoadingForm(false);
                });

                break;
            case 3:
                MenuLeft_ShowSearchHang();
                if (self.BCBH_TheoNhomHang() == "BCBH_TheoNhomHang") {
                    $(".PhanQuyen").hide();
                    ajaxHelper(ReportUri + "BaoCaoBanHang_NhomHang", "POST", array_Seach).done(function (data) {
                        self.BaoCaoBanHang_TheoNhomHang(data.LstData);
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.TNH_SoLuong(data.a1);
                        self.TNH_ThanhTien(data.a2);
                        self.TNH_TienVon(data.a3);
                        self.TNH_GiamGiaHD(data.a4);
                        self.TNH_LaiLo(data.a5);
                        self.TH_DoanhThuThuan(data.TongDoanhThuThuan);
                        self.TH_TienThue(data.SumTienThue);
                        LoadingForm(false);
                    });
                }
                else {
                    $(".PhanQuyen").show();
                    $(".Report_Empty").hide();
                    $(".TC_TheoNhomHang").hide();
                    $(".page").hide();
                    LoadingForm(false);
                }
                break;
            case 4:
                // bc theo khachhang: get ca hdtra
                array_Seach.LoaiChungTu = array_Seach.LoaiChungTu + ',6';
                if (self.BCBH_TheoKhachHang() == "BCBH_TheoKhachHang") {
                    $(".PhanQuyen").hide();
                    MenuLeft_ShowSearchHang();
                    ajaxHelper(ReportUri + "BaoCaoBanHang_TheoKhachHang", "POST", array_Seach).done(function (x) {
                        if (x.res === true) {
                            self.BaoCaoBanHang_TheoKhachHang(x.LstData);
                            self.KH_ThanhTien(x.LstData);
                            self.SumRowsHangHoa(x.SoLuong);
                            self.LN_SoLuongBan(x.SoLuongMua);
                            self.LN_ThanhTien(x.GiaTriMua);
                            self.LN_SoLuongTra(x.SoLuongTra);
                            self.LN_GiaTriTra(x.GiaTriTra);

                            self.KH_SoLuong(x.SoLuong);
                            self.TH_DoanhThuThuan(x.DoanhThu);
                            self.TH_TienThue(x.TongTienThue);
                            self.KH_TienVon(x.TienVon);
                            self.KH_LaiLo(x.LaiLo);
                            self.TH_TongChiPhi(x.SumChiPhi);
                            self.SumRowsHangHoa(x.Rowcount);
                            AllPage = x.numberPage;
                        }
                        self.selecPage();
                        self.ReserPage();
                        LoadingForm(false);
                    });
                }
                else {
                    $(".PhanQuyen").show();
                    $(".Report_Empty").hide();
                    $(".TC_TheoKhachHang").hide();
                    $(".page").hide();
                    LoadingForm(false);
                }
                break;
            case 52:
                $('a[href $=kh_tonghop]').parent('li').addClass('active');
                $('a[href $=kh_chitiet]').parent('li').removeClass('active');
                //arr.push({ ID: 22, TenChungTu: 'Thẻ giá trị' });
                self.ChungTus($.extend(true, [], arr));
                MenuLeft_ShowSearchCustomer();

                array_Seach = BCTanSuat_GetParamSearch(false);
                ajaxHelper(ReportUri + "BaoCaoKhachHang_TanSuat", "POST", array_Seach).done(function (x) {
                    if (x.res) {
                        self.BaoCaoKhachHang_TanSuat(x.data);
                        if (x.data.length > 0) {
                            let firstR = x.data[0];
                            AllPage = firstR.TotalPage;
                            self.SumRowsHangHoa(firstR.TotalRow);
                            self.TNH_SoLuong(firstR.TongSoLanDen);
                            self.KH_ThanhTien(firstR.TongMua);
                            self.NV_GiaTriTra(firstR.TongTra);
                            self.TH_DoanhThuThuan(firstR.TongDoanhThu);

                            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                            let toItem = firstR.TotalRow < _pageNumber * _pageSize ? firstR.TotalRow : _pageNumber * _pageSize;
                            self.RowsEnd(toItem);
                            $('.page').show();
                        }
                        else {
                            AllPage = 0;
                            self.SumRowsHangHoa(0);
                            $('.page').hide();
                        }
                        self.selecPage();
                        self.ReserPage();
                    }
                    LoadingForm(false);
                });
                break;
            case 5:// not use
                ajaxHelper(ReportUri + "BaoCaoBanHang_TheoNhomKhachHang", "POST", array_Seach).done(function (data) {
                    self.BaoCaoBanHang_TongHop(data);
                })
                break;
            case 6:
                MenuLeft_ShowSearchHang();
                array_Seach.lstPhongBan = self.ListIDPhongBan_Chosed();
                array_Seach.LoaiChungTu = array_Seach.LoaiChungTu + ',6';

                if (self.BCBH_TheoNhanVien() == "BCBH_TheoNhanVien") {
                    $(".PhanQuyen").hide();
                    ajaxHelper(ReportUri + "BaoCaoBanHang_TheoNhanVien", "POST", array_Seach).done(function (data) {
                        self.BaoCaoBanHang_TheoNhanVien(data.LstData);
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.NV_SoLuongBan(data.a1);
                        self.NV_ThanhTien(data.a2);
                        self.NV_SoLuongTra(data.a3);
                        self.NV_GiaTriTra(data.a4);
                        self.NV_TienVon(data.a5);
                        self.NV_GiamGiaHD(data.a6);
                        self.NV_LaiLo(data.a7);
                        self.TH_DoanhThuThuan(data.TongDoanhThuThuan);
                        self.TH_TienThue(data.SumTienThue);
                        self.TH_TongChiPhi(data.SumChiPhi);
                        LoadingForm(false);
                    });
                }
                else {
                    $(".PhanQuyen").show();
                    $(".Report_Empty").hide();
                    $(".TC_TheoNhanVien").hide();
                    $(".page").hide();
                    LoadingForm(false);
                }
                break;
            case 7:
                MenuLeft_ShowSearchHang();
                if (self.BCBH_HangTraLai() == "BCBH_HangTraLai") {
                    $(".PhanQuyen").hide();
                    ajaxHelper(ReportUri + "BaoCaoBanHang_HangTraLai", "POST", array_Seach).done(function (data) {
                        self.BaoCaoBanHang_HangTraLai(data.LstData);
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.HTL_SoLuongBan(data.a1);
                        self.HTL_ThanhTien(data.a2);
                        self.HTL_GiamGiaHD(data.a3);
                        self.HTL_GiaTriTra(data.a4);
                        LoadingForm(false);
                    });
                }
                else {
                    $(".PhanQuyen").show();
                    $(".Report_Empty").hide();
                    $(".TC_HangTraLai").hide();
                    $(".page").hide();
                    LoadingForm(false);
                }
                break;
            case 8:
                MenuLeft_ShowSearchHang();
                array_Seach.LoaiChungTu = array_Seach.LoaiChungTu + ',6';
                if (self.BCBH_LoiNhuanTheoMatHang() == "BCBH_LoiNhuanTheoMatHang") {
                    $(".PhanQuyen").hide();
                    ajaxHelper(ReportUri + "BaoCaoBanHang_LoiNhuan", "POST", array_Seach).done(function (data) {
                        self.BaoCaoBanHang_LoiNhuan(data.LstData);
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.LN_SoLuongBan(data.a1);
                        self.LN_ThanhTien(data.a2);
                        self.LN_SoLuongTra(data.a3);
                        self.LN_GiaTriTra(data.a4);
                        self.LN_GiamGiaHD(data.a5);
                        self.LN_DoanhThuThuan(data.a6);
                        self.TH_TienThue(data.TongTienThue);
                        self.LN_TienVon(data.a7);
                        self.LN_LaiLo(data.a8);
                        self.LN_TySuat(data.a9);
                        self.TH_TongChiPhi(data.SumChiPhi);
                        LoadingForm(false);
                    });
                }
                else {
                    $(".PhanQuyen").show();
                    $(".Report_Empty").hide();
                    $(".TC_LoiNhuan").hide();
                    $(".page").hide();
                    LoadingForm(false);
                }
                break;
            case 9:
                MenuLeft_ShowSearchHang();
                ajaxHelper(ReportUri + "BaoCaoHangKhuyenMai", "POST", array_Seach).done(function (data) {
                    self.BaoCaoHangKhuyenMai(data.LstData);
                    AllPage = data.numberPage;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.HTL_SoLuongBan(data.TongSoLuong);
                    self.HTL_ThanhTien(data.TongDoanhThu);
                    self.HTL_GiamGiaHD(data.TongGiatriKM);
                    LoadingForm(false);
                });
                break;
        }
    }

    self.ColumnSort = ko.observable('');
    self.TypeSort = ko.observable();
    self.BCTanSuat_TypeTimeCheck = ko.observable('1');
    self.BCTanSuat_TypeTime = ko.observable('namnay');
    self.BCTanSuat_TypeTimeText = ko.observable('Năm này');
    self.BCTanSuat_DateRangeText = ko.observable();

    self.BCTanSuat_RdoTypeTime_NgayTao = ko.observable('1');
    self.BCTanSuat_TypeTime_NgayTao = ko.observable('toanthoigian');
    self.BCTanSuat_TypeTimeText_NgayTao = ko.observable('Toàn thời gian');
    self.BCTanSuat_DateRangeText_NgayTao = ko.observable();

    function getToDayBC() {
        if (parseInt(self.ThoiGianHD_RdoTypeTime()) === 1) {
            let obj = GetDate_FromTo(self.ThoiGianHD_TypeTime());
            fromDate = obj.FromDate;
            toDate = moment(obj.ToDate, 'YYYY-MM-DD').add('days', 1).format('YYYY-MM-DD');

            switch (self.ThoiGianHD_TypeTime()) {
                case 'homnay':
                case 'homqua':
                    self.TodayBC('Ngày bán: '.concat(moment(fromDate, 'YYYY-MM-DD').format('DD/MM/YYYY')))
                    break;
                default:
                    self.TodayBC('Từ ngày: '.concat(moment(fromDate, 'YYYY-MM-DD').format('DD/MM/YYYY'),
                        ' Đến ngày ', moment(toDate, 'YYYY-MM-DD').add('days', -1).format('DD/MM/YYYY')))
                    break;
            }
        }
        else {
            let arrDate = self.ThoiGianHD_DateRangeText().split('-');
            fromDate = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            toDate = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');

            self.TodayBC('Từ ngày: '.concat(moment(fromDate, 'YYYY-MM-DD').format('DD/MM/YYYY'),
                ' Đến ngày ', moment(toDate, 'YYYY-MM-DD').add('days', -1).format('DD/MM/YYYY')))
        }
        _timeStart = fromDate;
        _timeEnd = toDate;
    }

    $('.choose_TimeReport li').on('click', function () {
        self.ThoiGianHD_TypeTime($(this).attr('value'));
        self.ThoiGianHD_TypeTimeText($(this).text());
        getToDayBC();
        self.LoadReport();
    });

    $('.choose_TimeReport .newDateTime').on('apply.daterangepicker', function (e, picker) {
        self.ThoiGianHD_DateRangeText(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        getToDayBC();
        self.LoadReport();
    });

    $('.customer_NgayGiaoDichMax li').on('click', function () {
        self.BCTanSuat_TypeTime($(this).attr('value'));
        self.BCTanSuat_TypeTimeText($(this).text());
        self.LoadReport();
    });

    $('.customer_NgayGiaoDichMax .newDateTime').on('apply.daterangepicker', function (e, picker) {
        self.BCTanSuat_DateRangeText(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        self.LoadReport();
    });

    $('.customer_NgayTaoKH li').on('click', function () {
        self.BCTanSuat_TypeTime_NgayTao($(this).attr('value'));
        self.BCTanSuat_TypeTimeText_NgayTao($(this).text());
        self.LoadReport();
    });

    $('.customer_NgayTaoKH .newDateTime').on('apply.daterangepicker', function (e, picker) {
        self.BCTanSuat_DateRangeText_NgayTao(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        self.LoadReport();
    });

    self.BCTanSuat_TypeTimeCheck.subscribe(function (newVal) {
        self.LoadReport();
    });

    self.BCTanSuat_RdoTypeTime_NgayTao.subscribe(function (newVal) {
        self.LoadReport();
    });

    self.ThoiGianHD_RdoTypeTime.subscribe(function (newVal) {
        getToDayBC();
        self.LoadReport();
    });

    self.Customer_TrangThai.subscribe(function (newVal) {
        self.LoadReport();
    });

    self.BCTanSuat_ShowChiTiet = function (item) {
        var idKhachHang = item.ID_KhachHang;
        nhatkyGD.ChiNhanhs(self.DonVis());
        nhatkyGD.ArrDonVi(self.ArrDonVi());
        nhatkyGD.ChiNhanhChosed(self.MangChiNhanh());
        nhatkyGD.LstIDDonVi(self.LstIDDonVi());
        nhatkyGD.FromDate(moment(_timeStart, 'YYYY-MM-DD').format('DD/MM/YYYY'));
        nhatkyGD.ToDate(moment(_timeEnd, 'YYYY-MM-DD').add('days', -1).format('DD/MM/YYYY'));
        nhatkyGD.ID_KhachHang(item.ID_KhachHang);
        nhatkyGD.Export_ChiNhanh(self.TenChiNhanh());

        $('#modalDiaryTrans .txtKhachHang').val(item.TenKhachHang);
        $('#modalDiaryTrans').modal('show');
        nhatkyGD.SearchNhatKy();
    };

    self.BCTanSuat_KeyupTextSearch = function () {
        var keycode = event.keyCode || event.which;
        if (keycode === 13) {
            _pageNumber = 1;
            self.LoadReport();
        }
    }

    function BCTanSuat_GetParamSearch(isExport) {
        var lstIDNhomKH = [];
        if (self.MangNhomDoiTuong().length === 0) {
            lstIDNhomKH = self.NhomDoiTuongs().map(function (x) {
                return x.ID;
            });
        }
        else {
            lstIDNhomKH = self.MangNhomDoiTuong().map(function (x) {
                return x.ID;
            });
        }
        var trangthai = self.Customer_TrangThai();
        if (parseInt(trangthai) === 2) {
            trangthai = '%%';
        }
        var chungtus = '';
        if (self.MangChungTu().length === 0) {
            chungtus = '1,19,22,25,6';
        }
        else {
            chungtus = _idChungTuSeach;
        }
        var doanhthutu = self.Customer_DoanhThuTu();
        doanhthutu = doanhthutu === '' ? null : doanhthutu;
        var solandentu = self.Customer_SoLanDenFrom();
        solandentu = solandentu === '' ? 0 : solandentu;

        var giaodichFrom = '', giaodichTo = '';
        if (parseInt(self.BCTanSuat_TypeTimeCheck()) === 1) {
            let objNgayGiaDich = GetDate_FromTo(self.BCTanSuat_TypeTime());
            giaodichFrom = objNgayGiaDich.FromDate;
            giaodichTo = moment(objNgayGiaDich.ToDate, 'YYYY-MM-DD').add('days', 1).format('YYYY-MM-DD');
        }
        else {
            let arrDate = self.BCTanSuat_DateRangeText().split('-');
            giaodichFrom = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            giaodichTo = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
        }

        var ngaytaoFrom = '', ngaytaoTo = '';
        if (parseInt(self.BCTanSuat_RdoTypeTime_NgayTao()) === 1) {
            let objNgayTao = GetDate_FromTo(self.BCTanSuat_TypeTime_NgayTao());
            ngaytaoFrom = objNgayTao.FromDate;
            ngaytaoTo = moment(objNgayTao.ToDate, 'YYYY-MM-DD').add('days', 1).format('YYYY-MM-DD');
        }
        else {
            let arrDate = self.BCTanSuat_DateRangeText_NgayTao().split('-');
            ngaytaoFrom = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            ngaytaoTo = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
        }

        return {
            LstIDChiNhanh: self.LstIDDonVi(),
            LstIDNhomKhach: lstIDNhomKH,
            LoaiChungTus: chungtus,
            TrangThaiKhach: trangthai,
            FromDate: _timeStart,
            ToDate: _timeEnd,
            NgayGiaoDichFrom: giaodichFrom,
            NgayGiaoDichTo: giaodichTo,
            NgayTaoKHFrom: ngaytaoFrom,
            NgayTaoKHTo: ngaytaoTo,
            DoanhThuTu: doanhthutu,
            DoanhThuDen: self.Customer_DoanhThuDen(),
            SoLanDenFrom: solandentu,
            SoLanDenTo: self.Customer_SoLanDenTo(),
            TextSearch: Text_search,
            CurrentPage: isExport ? 0 : _pageNumber - 1,
            PageSize: isExport ? self.SumRowsHangHoa() : _pageSize,
            Export_Time: self.TodayBC(),
            Export_ChiNhanh: self.TenChiNhanh(),
            ColumnSort: self.ColumnSort(),
            TypeSort: self.TypeSort() === 1 ? 'desc' : 'asc',
        }
    }

    $('#kh_tonghop thead tr').on('click', 'th', function () {
        var id = $(this).attr('id');
        if (id !== undefined) {
            switch (id) {
                case "Customer_solanden":
                    self.ColumnSort("SoLanDen");
                    break;
                case "Customer_doanhthu":
                    self.ColumnSort("DoanhThu");
                    break;
                case "Customer_ngaygiaodichgannhat":
                    self.ColumnSort("NgayGiaoDichGanNhat");
                    break;
            }
            SortGrid(id);
        }
    });

    function SortGrid(item) {
        $("#iconSort").remove();
        self.LoadReport();
        if (self.TypeSort() === 1) {
            self.TypeSort(2);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-down' aria-hidden='true'></i>");
        }
        else {
            self.TypeSort(1);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-up' aria-hidden='true'></i>");
        }
    };

    self.BaoCaoBanHangChiTiet_TheoNhanVien = ko.observableArray();
    self.BaoCaoBanHangChiTiet_TheoKhachHang = ko.observableArray();
    self.NV_SoLuongBanPrint = ko.observable();
    self.NV_ThanhTienPrint = ko.observable();
    self.NV_SoLuongTraPrint = ko.observable();
    self.NV_GiaTriTraPrint = ko.observable();
    self.NV_GiamGiaHDPrint = ko.observable();
    self.NV_TienVonPrint = ko.observable();
    self.NV_LaiLoPrint = ko.observable();
    self.KH_SoLuongBanPrint = ko.observable();
    self.KH_ThanhTienPrint = ko.observable();
    self.KH_GiamGiaHDPrint = ko.observable();
    self.KH_TienVonPrint = ko.observable();
    self.KH_LaiLoPrint = ko.observable();
    var _idNhanVienBanHang;
    var _tenNhanVienBanHang;
    var _idKhachHang;
    var _tenKhachHang;

    function GetLoaiHang() {
        let arr = [];
        if (self.LoaiSP_HH()) {
            arr.push(1);
        }
        if (self.LoaiSP_DV()) {
            arr.push(2);
        }
        if (self.LoaiSP_CB()) {
            arr.push(3);
        }
        if (arr.length === 0) {
            arr = [1, 2, 3];
        }
        return arr.toString();
    }

    function LoadParamSearch() {
        var fromDate = '', toDate = '';
        if (parseInt(self.ThoiGianHD_RdoTypeTime()) === 1) {
            let obj = GetDate_FromTo(self.ThoiGianHD_TypeTime());
            fromDate = obj.FromDate;
            toDate = moment(obj.ToDate, 'YYYY-MM-DD').add('days', 1).format('YYYY-MM-DD');
        }
        else {
            let arrDate = self.ThoiGianHD_DateRangeText().split('-');
            fromDate = moment(arrDate[0], 'DD/MM/YYYY').format('YYYY-MM-DD');
            toDate = moment(arrDate[1], 'DD/MM/YYYY').add('days', 1).format('YYYY-MM-DD');
        }

        return obj =
        {
            MaHangHoa: Text_search,
            MaKhachHang: _maKhachHang,
            NV_GioiThieu: _magioithieu,
            NV_QuanLy: _maquanly,
            timeStart: fromDate,
            timeEnd: toDate,
            ID_ChiNhanh: _idDonViSeach,
            LoaiHangHoa: GetLoaiHang(),
            TinhTrang: TinhTrangHH,
            ID_NhomHang: _ID_NhomHang,
            ID_NhomKhachHang: _tenNhomDoiTuongSeach,
            LoaiChungTu: _idChungTuSeach,
            HanBaoHanh: TinhTrangBH,
            ID_NguoiDung: _IDDoiTuong,
            pageNumber: _pageNumber,
            pageSize: _pageSize,
            columnsHide: null,
            TodayBC: self.TodayBC(),
            TenChiNhanh: null,
            lstIDChiNhanh: self.LstIDDonVi(),
            TenChiNhanh: self.TenChiNhanh(),
        };
    }

    self.getList_BCBH_ChiTietTheoNhanVien = function (item) {
        _idNhanVienBanHang = item.ID_NhanVien;
        _tenNhanVienBanHang = 'Nhân viên: ' + item.TenNhanVien;
        self.TenNVPrint(item.TenNhanVien);
        self.NV_SoLuongBanPrint(item.SoLuongBan);
        self.NV_ThanhTienPrint(item.ThanhTien);
        self.NV_SoLuongTraPrint(item.SoLuongTra);
        self.NV_GiaTriTraPrint(item.GiaTriTra);
        self.NV_GiamGiaHDPrint(item.GiamGiaHD);
        self.NV_TienVonPrint(item.TienVon);
        self.NV_LaiLoPrint(item.LaiLo);
        $(".page").show();
        _pageNumber_CT = 1;
        self.pageNumber_CTNV(_pageNumber_CT);

        var obj = LoadParamSearch();
        obj.NV_GioiThieu = item.ID_NhanVien;// muon tamtruong
        obj.LoaiChungTu = obj.LoaiChungTu + ',6';
        ajaxHelper(ReportUri + 'BaoCaoBanHangChiTiet_TheoNhanVien', "POST", obj).done(function (data) {
            self.BaoCaoBanHangChiTiet_TheoNhanVien(data.LstData);
            self.SumNumberPageReport_CT(data.LstPageNumber);
            AllPage_CT = self.SumNumberPageReport_CT().length;
            self.selecPage_CT();
            self.ReserPage_CT();
            self.SumRowsHangHoa_CT(data.Rowcount);
            LoadingForm(false);
        });
    }
    self.BaoCaoBanHangChiTiet_TheoNhanVien_Page = ko.computed(function (x) {
        var first = (self.pageNumber_CTNV() - 1) * self.pageSize();
        if (self.BaoCaoBanHangChiTiet_TheoNhanVien() !== null) {
            self.RowsStart_CT((self.pageNumber_CTNV() - 1) * self.pageSize() + 1);
            self.RowsEnd_CT((self.pageNumber_CTNV() - 1) * self.pageSize() + self.BaoCaoBanHangChiTiet_TheoNhanVien().slice(first, first + self.pageSize()).length)
            return self.BaoCaoBanHangChiTiet_TheoNhanVien().slice(first, first + _pageSize_CT);
        }
        return null;
    })

    self.KH_SoLuongBanPrint = ko.observable(0);
    self.KH_GiatriBanPrint = ko.observable(0);
    self.KH_SoLuongTraPrint = ko.observable(0);
    self.KH_GiatriTraPrint = ko.observable(0);
    self.KH_SoLuongPrint = ko.observable(0);
    self.KH_ThanhTienPrint = ko.observable(0);
    self.KH_GiamGiaHDPrint = ko.observable(0);
    self.KH_TongThuePrint = ko.observable(0);
    self.KH_TienVonPrint = ko.observable(0);
    self.KH_LaiLoPrint = ko.observable(0);

    self.getList_BCBH_ChiTietTheoKhachHang = function (item) {
        _idKhachHang = item.ID_KhachHang;
        _tenKhachHang = 'Khách hàng: ' + item.TenKhachHang;

        self.KH_SoLuongBanPrint(item.SoLuongMua);
        self.KH_GiatriBanPrint(item.GiaTriMua);
        self.KH_SoLuongTraPrint(item.SoLuongTra);
        self.KH_GiatriTraPrint(item.GiaTriTra);
        self.KH_SoLuongPrint(item.SoLuong);
        self.KH_ThanhTienPrint(item.DoanhThu);
        self.KH_GiamGiaHDPrint(item.GiamGiaHD);
        self.KH_TongThuePrint(item.TongTienThue);
        self.KH_TienVonPrint(item.TienVon);
        self.KH_LaiLoPrint(item.LaiLo);

        $(".page").show();
        _pageNumber_CTKH = 1;
        self.pageNumber_CTKH(_pageNumber_CTKH);
        var obj = LoadParamSearch();
        obj.LoaiChungTu = obj.LoaiChungTu + ',6';
        obj.NV_GioiThieu = item.ID_KhachHang;// muon tamtruong
        ajaxHelper(ReportUri + 'BaoCaoBanHangChiTiet_TheoKhachHang', "POST", obj).done(function (data) {
            self.BaoCaoBanHangChiTiet_TheoKhachHang(data.LstData);
            self.SumNumberPageReport_CTKH(data.LstPageNumber);
            AllPage_CTKH = self.SumNumberPageReport_CTKH().length;
            self.selecPage_CTKH();
            self.ReserPage_CTKH();
            self.SumRowsHangHoa_CTKH(data.Rowcount);
            LoadingForm(false);

        });
    }
    self.BaoCaoBanHangChiTiet_TheoKhachHang_Page = ko.computed(function (x) {
        var first = (self.pageNumber_CTKH() - 1) * self.pageSize();
        if (self.BaoCaoBanHangChiTiet_TheoKhachHang() !== null) {
            self.RowsStart_CTKH((self.pageNumber_CTKH() - 1) * self.pageSize() + 1);
            self.RowsEnd_CTKH((self.pageNumber_CTKH() - 1) * self.pageSize() + self.BaoCaoBanHangChiTiet_TheoKhachHang().slice(first, first + self.pageSize()).length)
            return self.BaoCaoBanHangChiTiet_TheoKhachHang().slice(first, first + _pageSize_CTKH);
        }
        return null;
    })
    self.BaoCaoBanHang_ChiTiet_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) == 2) {
            var first = (self.pageNumber_CT() - 1) * self.pageSize();
            if (self.BaoCaoBanHang_ChiTiet() !== null) {
                if (self.BaoCaoBanHang_ChiTiet().length != 0) {
                    $('.TC_ChiTiet').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_CT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_CT() - 1) * self.pageSize() + self.BaoCaoBanHang_ChiTiet().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_ChiTiet').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoBanHang_ChiTiet().slice(first, first + self.pageSize());
            }
            return null;
        }
    })
    self.BaoCaoBanHang_TheoNhomHang_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) == 3) {
            var first = (self.pageNumber_NH() - 1) * self.pageSize();
            if (self.BaoCaoBanHang_TheoNhomHang() !== null) {
                if (self.BaoCaoBanHang_TheoNhomHang().length != 0) {
                    $('.TC_TheoNhomHang').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_NH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_NH() - 1) * self.pageSize() + self.BaoCaoBanHang_TheoNhomHang().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_TheoNhomHang').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoBanHang_TheoNhomHang().slice(first, first + self.pageSize());
            }
            return null;
        }
    })
    self.BaoCaoBanHang_TheoKhachHang_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 4) {
            var first = (self.pageNumber_KH() - 1) * self.pageSize();
            if (self.BaoCaoBanHang_TheoKhachHang() !== null) {
                if (self.BaoCaoBanHang_TheoKhachHang().length != 0) {
                    $('.TC_TheoKhachHang').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_KH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_KH() - 1) * self.pageSize() + self.BaoCaoBanHang_TheoKhachHang().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_TheoKhachHang').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoBanHang_TheoKhachHang().slice(first, first + self.pageSize());
            }
            return null;
        }
    })
    self.BaoCaoBanHang_TheoNhanVien_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 6) {
            var first = (self.pageNumber_NV() - 1) * self.pageSize();
            if (self.BaoCaoBanHang_TheoNhanVien() !== null) {
                if (self.BaoCaoBanHang_TheoNhanVien().length != 0) {
                    $('.TC_TheoNhanVien').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_NV() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_NV() - 1) * self.pageSize() + self.BaoCaoBanHang_TheoNhanVien().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_TheoNhanVien').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoBanHang_TheoNhanVien().slice(first, first + self.pageSize());
            }
            return null;
        }
    })
    self.BaoCaoBanHang_HangTraLai_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 7) {
            var first = (self.pageNumber_HTL() - 1) * self.pageSize();
            if (self.BaoCaoBanHang_HangTraLai() !== null) {
                if (self.BaoCaoBanHang_HangTraLai().length != 0) {
                    $('.TC_HangTraLai').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_HTL() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_HTL() - 1) * self.pageSize() + self.BaoCaoBanHang_HangTraLai().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_HangTraLai').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoBanHang_HangTraLai().slice(first, first + self.pageSize());
            }
            return null;
        }
    })
    self.BaoCaoBanHang_LoiNhuan_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 8) {
            var first = (self.pageNumber_LN() - 1) * self.pageSize();
            if (self.BaoCaoBanHang_LoiNhuan() !== null) {
                if (self.BaoCaoBanHang_LoiNhuan().length != 0) {
                    $('.TC_LoiNhuan').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_LN() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_LN() - 1) * self.pageSize() + self.BaoCaoBanHang_LoiNhuan().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_LoiNhuan').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoBanHang_LoiNhuan().slice(first, first + self.pageSize());
            }
            return null;
        }
    })
    self.BaoCaoHangKhuyenMai_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 9) {
            var first = (self.pageNumber_KM() - 1) * self.pageSize();
            if (self.BaoCaoHangKhuyenMai() !== null) {
                if (self.BaoCaoHangKhuyenMai().length != 0) {
                    $(".page").show();
                    self.RowsStart((self.pageNumber_KM() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_KM() - 1) * self.pageSize() + self.BaoCaoHangKhuyenMai().slice(first, first + self.pageSize()).length)
                }
                else {
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoHangKhuyenMai().slice(first, first + self.pageSize());
            }
            return null;
        }
    })

    self.LoaiSP_HH.subscribe(function () {
        _pageNumber = 1;
        self.LoadReport();
    })
    self.LoaiSP_DV.subscribe(function () {
        _pageNumber = 1;
        self.LoadReport();
    })
    self.LoaiSP_CB.subscribe(function () {
        _pageNumber = 1;
        self.LoadReport();
    })

    self.Select_Text = function () {
        Text_search = $('#txt_search').val();
    }
    $('#txt_search').keypress(function (e) {
        if (e.keyCode == 13) {
            _pageNumber = 1;
            self.LoadReport();
        }
    })

    self.SelectMaKH = function () {
        _maKhachHang = $('#txtMaKH').val();
    }
    $('#txtMaKH').keypress(function (e) {
        if (e.keyCode == 13) {
            self.LoadReport();
        }
    })
    self.SelectMaGT = function () {
        _magioithieu = $('#txtMaGT').val();
    }
    $('#txtMaGT').keypress(function (e) {
        if (e.keyCode == 13) {
            self.LoadReport();
        }
    })
    self.SelectMaQL = function () {
        _maquanly = $('#txtMaQL').val();
    }
    $('#txtMaQL').keypress(function (e) {
        if (e.keyCode == 13) {
            self.LoadReport();
        }
    })
    // load danh sách
    self.LoadHangHoa_byMaHH = function (item) {
        if (item.MaHangHoa != '' && item.MaHangHoa != null) {
            localStorage.setItem('loadMaHang', item.MaHangHoa);
            var url = "/#/Product";
            window.open(url);
        }
    }
    self.LoadLoHang_byMaLH = function (item) {
        localStorage.setItem('FindLoHang', item.TenLoHang);
        var url = "/#/Shipment";
        window.open(url);
    };
    self.LoadKhachHang_byMaKH = function (item) {
        if (item.MaKhachHang != '' && item.MaKhachHang != null) {
            localStorage.setItem('FindKhachHang', item.MaKhachHang);
            var url = "/#/Customers";
            window.open(url);
        }
    }

    function gotoPage(maHD, loaiHD) {
        var url = '';
        if (maHD.indexOf('TT') > -1) {
            localStorage.setItem('FindMaPhieuChi', maHD);
            url = "/#/CashFlow"; // soquy
        }
        else {
            localStorage.setItem('FindHD', maHD);
            switch (loaiHD) {
                case 1:
                case 36:
                    url = "/#/Invoices";
                    break;
                case 2:
                    url = "/#/HoaDonBaoHanh";
                    break;
                case 3:
                    url = "/#/Order";
                    break;
                case 6:
                    url = "/#/Returns";
                    break;
                case 19:
                    url = "/#/ServicePackage";
                    break;
                case 25:
                    url = "/#/HoaDonSuaChua";
                    break;
            }
        }
        window.open(url);
    }

    self.LoadHoaDon_byMaHD = function (item) {
        var maHD = item.MaHoaDon;
        gotoPage(maHD, item.LoaiHoaDon);
    }
    self.LoadChungTu_byMaHD = function (item) {
        var maHD = item.MaChungTu;
        gotoPage(maHD, 6);
    }
    self.LoadChungTuGoc_byMaHD = function (item) {
        var maHD = item.MaChungTuGoc;
        gotoPage(maHD, item.LoaiHoaDon);
    }

    //Download file excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function (item) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    }
    // xuất file Excel
    self.ExportExcel = async function () {
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

        var array_Seach = {
            MaHangHoa: Text_search,
            MaKhachHang: _maKhachHang,
            NV_GioiThieu: _magioithieu,
            NV_QuanLy: _maquanly,
            timeStart: _timeStart,
            timeEnd: _timeEnd,
            ID_ChiNhanh: _idDonViSeach,
            LoaiHangHoa: GetLoaiHang(),
            TinhTrang: TinhTrangHH,
            ID_NhomHang: _ID_NhomHang,
            ID_NhomKhachHang: _tenNhomDoiTuongSeach,
            LoaiChungTu: _idChungTuSeach,
            HanBaoHanh: TinhTrangBH,
            ID_NguoiDung: _IDDoiTuong,
            pageNumber: 1,
            pageSize: 100000,
            columnsHide: columnHide,
            TodayBC: self.TodayBC(),
            TenChiNhanh: self.TenChiNhanh(),
            lstIDChiNhanh: self.LstIDDonVi(),
        }
        var func = '';
        var lenData = 0;
        let fileNameExport = '';

        if (self.BCBH_XuatFile() != "BCBH_XuatFile") {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
            LoadingForm(false);
            return false;
        }
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                {
                    func = 'Export_BCBH_TongHop';
                    lenData = self.BaoCaoBanHang_TongHop().length;
                    fileNameExport = "BaoCaoBanHangTongHop.xlsx";
                }
                break;
            case 2:
                {
                    func = 'Export_BCBH_ChiTiet';
                    lenData = self.BaoCaoBanHang_ChiTiet().length;
                    fileNameExport = "BaoCaoBanHangChiTiet.xlsx";
                }
                break;
            case 10:
                {
                    func = 'Export_BaoCaoBanHang_DinhDanhDichVu';
                    lenData = self.BaoCaoBanHang_DinhDanhDichVu().length;
                    fileNameExport = "BaoCaoDinhDanhDichVu.xlsx";
                }
                break;
            case 3:
                {
                    func = 'Export_BCBH_TheoNhomHang';
                    lenData = self.BaoCaoBanHang_TheoNhomHang().length;
                    fileNameExport = "BaoCaoBanHang_TheoNhomHang.xlsx";
                }
                break;
            case 4:
                {
                    func = 'Export_BCBH_TheoKhachHang';
                    lenData = self.BaoCaoBanHang_TheoKhachHang().length;
                    fileNameExport = "BaoCaoBanHang_TheoKhachHang.xlsx";
                }
                break;
            case 52:
                {
                    func = 'Export_BaoCaoKhachHangTanSuat';
                    lenData = self.BaoCaoKhachHang_TanSuat().length;
                    fileNameExport = "BaoCaoBanHang_TanSuatKhachHang.xlsx";

                    array_Seach = BCTanSuat_GetParamSearch(true);

                    let arr = [];
                    let clHide = localStorage.getItem(Key_Form + self.columnCheckType());
                    if (clHide !== null) {
                        clHide = JSON.parse(clHide);
                        for (let i = 0; i < clHide.length; i++) {
                            for (let j = 0; j < self.listCheckbox().length; j++) {
                                if (clHide[i].NameClass === self.listCheckbox()[j].Key) {
                                    arr.push(j);
                                }
                            }
                        }
                    }

                    let sClHide = '';
                    for (let i = 0; i < arr.length; i++) {
                        sClHide += arr[i] + '_';
                    }
                    array_Seach.columnsHide = sClHide;
                }
                break;
            case 5:// khong co
                func = 'Export_BCBH_ChiTiet';
                lenData = self.BaoCaoBanHang_ChiTiet().length;
                break;
            case 6:
                {
                    func = 'Export_BCBH_TheoNhanVien';
                    lenData = self.BaoCaoBanHang_TheoNhanVien().length;
                    array_Seach.LoaiChungTu += ',6';
                    fileNameExport = "BaoCaoBanHangTheoNhanVien.xlsx";
                }
                break;
            case 7:
                {
                    func = 'Export_BCBH_HangTraLai';
                    lenData = self.BaoCaoBanHang_HangTraLai().length;
                    fileNameExport = "BaoCaoHangTraLai.xlsx";
                }
                break;
            case 8:
                {
                    func = 'Export_BCBH_LoiNhuan';
                    lenData = self.BaoCaoBanHang_LoiNhuan().length;
                    fileNameExport = "BaoCaoLoiNhuanTheoHangHoa.xlsx";
                }
                break;
            case 9:
                {
                    func = 'Export_BaoCaoHangKhuyenMai';
                    lenData = self.BaoCaoHangKhuyenMai().length;
                    fileNameExport = "BaoCaoBanHang_KhuyenMai.xlsx";
                }
                break;
        }
        if (lenData === 0) {
            ShowMessage_Danger('Báo cáo không có dữ liệu');
        }
        else {
            let exportOK = false;
            LoadingForm(false);
            if (parseInt(self.check_MoiQuanTam()) === 52) {
                exportOK = await commonStatisJs.NPOI_ExportExcel(ReportUri + func, 'POST', array_Seach, fileNameExport);
            }
            else {
                exportOK = await commonStatisJs.NPOI_ExportExcel(ReportUri + func, 'POST', { objExcel: array_Seach }, fileNameExport);
            }
            if (exportOK) {
                var objDiary = {
                    ID_NhanVien: _id_NhanVien,
                    ID_DonVi: _id_DonVi,
                    ChucNang: "Báo cáo bán hàng",
                    NoiDung: "Xuất " + self.MoiQuanTam().toLowerCase(),
                    NoiDungChiTiet: "Xuất " + self.MoiQuanTam().toLowerCase(),
                    LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
                };
                Insert_NhatKyThaoTac_1Param(objDiary)
            }

        }
    }
    self.ExportChiTietNhanVien = async function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo bán hàng",
            NoiDung: "Xuất báo cáo danh sách hàng bán theo nhân viên",
            NoiDungChiTiet: "Xuất báo cáo danh sách hàng bán theo nhân viên",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };

        var obj = LoadParamSearch();
        obj.NV_GioiThieu = item.ID_NhanVien;// muon tamtruong
        obj.chitietBC = _tenNhanVienBanHang;

        const exportOK = await commonStatisJs.NPOI_ExportExcel(ReportUri + 'Export_BCBHCT_TheoNhanVien', 'POST', { objExcel: obj }, "BaoCaoBanHang_ChiTietTheoNhanVien");
        if (exportOK) {
            Insert_NhatKyThaoTac_1Param(objDiary);
        }
    }
    self.ExportChiTietKhachHang = async function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo bán hàng",
            NoiDung: "Xuất báo cáo danh sách hàng bán theo khách hàng",
            NoiDungChiTiet: "Xuất báo cáo danh sách hàng bán theo khách hàng",
            LoaiNhatKy: 6 // 1: Thêm mới, 2: Cập nhật, 3: Xóa, 4: Hủy, 5: Import, 6: Export, 7: Đăng nhập
        };

        var obj = LoadParamSearch();
        obj.NV_GioiThieu = item.ID_KhachHang;// muon tamtruong
        obj.chitietBC = _tenKhachHang;
        var myData = {
            objExcel: obj,
        }
        const exportOK = await commonStatisJs.NPOI_ExportExcel(ReportUri + 'Export_BCBHCT_TheoKhachHang', 'POST', myData, "BaoCaoBanHang_ChiTietTheoKhachHang");
        if (exportOK) {
            Insert_NhatKyThaoTac_1Param(objDiary);
        }

    }
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    // sắp xếp bảng
    function sortByKeyTangDan(array, key) {
        return array.sort(function (a, b) {
            var x = a[key]; var y = b[key];
            return ((x < y) ? -1 : ((x > y) ? 1 : 0));
        });
    }
    function sortByKeyGiamGian(array, key) {
        return array.sort(function (a, b) {
            var x = a[key]; var y = b[key];
            return ((x > y) ? -1 : ((x < y) ? 1 : 0));
        });
    }
    self.sort = ko.observable(0);
    self.sortTable = function (array, key, item) {
        $("#iconSort").remove();
        if (self.sort() === 0) {
            array = sortByKeyTangDan(array, key);
            self.BaoCaoBanHang_TheoNhomHang(array);
            self.sort(1);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-down' aria-hidden='true'></i>");
            switch (self.check_MoiQuanTam()) {
                case 1:
                    self.BaoCaoBanHang_TongHop(array);
                    break;
                case 2:
                    self.BaoCaoBanHang_ChiTiet(array);
                    break;
                case 3:
                    self.BaoCaoBanHang_TheoNhomHang(array);
                    break;
                case 4:
                    self.BaoCaoBanHang_TheoKhachHang(array);
                    break;
                case 6:
                    self.BaoCaoBanHang_TheoNhanVien(array);
                    break;
                case 7:
                    self.BaoCaoBanHang_HangTraLai(array);
                    break;
                case 8:
                    self.BaoCaoBanHang_LoiNhuan(array);
                    break;
                case 9:
                    self.BaoCaoHangKhuyenMai(array);
                    break;
            }
        }
        else {
            array = sortByKeyGiamGian(array, key);
            self.BaoCaoBanHang_TheoNhomHang(array);
            self.sort(0);
            $('#' + item).append(' ' + "<i id='iconSort' class='fa fa-caret-up' aria-hidden='true'></i>");
            switch (self.check_MoiQuanTam()) {
                case 1:
                    self.BaoCaoBanHang_TongHop(array);
                    break;
                case 2:
                    self.BaoCaoBanHang_ChiTiet(array);
                    break;
                case 3:
                    self.BaoCaoBanHang_TheoNhomHang(array);
                    break;
                case 4:
                    self.BaoCaoBanHang_TheoKhachHang(array);
                    break;
                case 6:
                    self.BaoCaoBanHang_TheoNhanVien(array);
                    break;
                case 7:
                    self.BaoCaoBanHang_HangTraLai(array);
                    break;
                case 8:
                    self.BaoCaoBanHang_LoiNhuan(array);
                    break;
                case 9:
                    self.BaoCaoHangKhuyenMai(array);
                    break;
            }
        }
    }

    //Phân trang
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
        self.SumNumberPageReport([]);
        if (AllPage > 5) {
            if (_pageNumber > 2 && _pageNumber < (AllPage - 2)) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.push({ SoTrang: _pageNumber - 2 + i });
                }
            }
            else if (_pageNumber >= (AllPage - 2)) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.push({ SoTrang: AllPage - 4 + i });
                }
            }
            else {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.push({ SoTrang: 1 + i });
                }
            }
        }
        else {
            if (AllPage !== 0) {
                for (let i = 0; i < AllPage; i++) {
                    self.SumNumberPageReport.push({ SoTrang: 1 + i });
                }
            }
        }

        if (self.SumNumberPageReport().length > 0) {
            if (self.SumNumberPageReport()[0].SoTrang > 1) {
                $('#StartPage').show();
                $('#BackPage').show();
            }
            else {
                $('#StartPage').hide();
                $('#BackPage').hide();
            }
            if (self.SumNumberPageReport()[self.SumNumberPageReport().length - 1].SoTrang < AllPage) {
                $('#NextPage').show();
                $('#EndPage').show();
            }
            else {
                $('#NextPage').hide();
                $('#EndPage').hide();
            }
        }

        self.currentPage(parseInt(_pageNumber));
    }
    self.NextPage = function (item) {
        if (_pageNumber < AllPage) {
            _pageNumber = _pageNumber + 1;
            Assign_Page(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            Assign_Page(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        Assign_Page(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        Assign_Page(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        Assign_Page(_pageNumber);
        self.ReserPage();
    }

    function Assign_Page(_pageNumber) {
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
            case 2:
            case 10:
                self.LoadReport();
                break;
            case 3://  NhomHang
                self.pageNumber_NH(_pageNumber);
                break;
            case 4:// bckhachhang - chitiet
                self.pageNumber_KH(_pageNumber);
                break;
            case 52:
                self.pageNumber_KH(_pageNumber);
                self.LoadReport();
                break;
            case 5:
                self.pageNumber_NKH(_pageNumber);
                break;
            case 6:
                self.pageNumber_NV(_pageNumber);
                break;
            case 7:
                self.pageNumber_HTL(_pageNumber);
                break;
            case 8:
                self.pageNumber_LN(_pageNumber);
                break;
            case 9:
                self.pageNumber_KM(_pageNumber);
                break;
        }
    }

    self.currentPage_CT = ko.observable(1);
    self.GetClass_CT = function (page) {
        return (page.SoTrang === self.currentPage_CT()) ? "click" : "";
    };

    //Phân trang chi tiết nhân viên
    self.selecPage_CT = function () {
        if (AllPage_CT > 4) {
            for (var i = 3; i < AllPage_CT; i++) {
                self.SumNumberPageReport_CT.pop(i + 1);
            }
            self.SumNumberPageReport_CT.push({ SoTrang: 4 });
            self.SumNumberPageReport_CT.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReport_CT.pop(i);
            }
            for (var j = 0; j < AllPage_CT; j++) {
                self.SumNumberPageReport_CT.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage_CT').hide();
        $('#BackPage_CT').hide();
        $('#NextPage_CT').show();
        $('#EndPage_CT').show();
    }
    self.ReserPage_CT = function (item) {
        if (_pageNumber_CT > 1 && AllPage_CT > 5/* && nextPage < AllPage - 1*/) {
            if (_pageNumber_CT > 3 && _pageNumber_CT < AllPage_CT - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CT.replace(self.SumNumberPageReport_CT()[i], { SoTrang: parseInt(_pageNumber_CT) + i - 2 });
                }
            }
            else if (parseInt(_pageNumber_CT) === parseInt(AllPage_CT) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CT.replace(self.SumNumberPageReport_CT()[i], { SoTrang: parseInt(_pageNumber_CT) + i - 3 });
                }
            }
            else if (_pageNumber_CT == AllPage_CT) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CT.replace(self.SumNumberPageReport_CT()[i], { SoTrang: parseInt(_pageNumber_CT) + i - 4 });
                }
            }
            else if (_pageNumber_CT < 4) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CT.replace(self.SumNumberPageReport_CT()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumber_CT == 1 && AllPage_CT > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReport_CT.replace(self.SumNumberPageReport_CT()[i], { SoTrang: parseInt(_pageNumber_CT) + i });
            }
        }
        if (_pageNumber_CT > 1) {
            $('#StartPage_CT').show();
            $('#BackPage_CT').show();
        }
        else {
            $('#StartPage_CT').hide();
            $('#BackPage_CT').hide();
        }
        if (_pageNumber_CT == AllPage_CT) {
            $('#NextPage_CT').hide();
            $('#EndPage_CT').hide();
        }
        else {
            $('#NextPage_CT').show();
            $('#EndPage_CT').show();
        }
        self.currentPage_CT(parseInt(_pageNumber_CT));
    }
    self.NextPage_CT = function (item) {
        if (_pageNumber_CT < AllPage_CT) {
            _pageNumber_CT = _pageNumber_CT + 1;
            self.pageNumber_CTNV(_pageNumber_CT);
            self.ReserPage_CT();
        }
    };
    self.BackPage_CT = function (item) {
        if (_pageNumber_CT > 1) {
            _pageNumber_CT = _pageNumber_CT - 1;
            self.pageNumber_CTNV(_pageNumber_CT);
            self.ReserPage_CT();
        }
    };
    self.EndPage_CT = function (item) {
        _pageNumber_CT = AllPage_CT;
        self.pageNumber_CTNV(_pageNumber_CT);
        self.ReserPage_CT();
    };
    self.StartPage_CT = function (item) {
        _pageNumber_CT = 1;
        self.pageNumber_CTNV(_pageNumber_CT);
        self.ReserPage_CT();
    };
    self.gotoNextPage_CT = function (item) {
        _pageNumber_CT = item.SoTrang;
        self.pageNumber_CTNV(_pageNumber_CT);
        self.ReserPage_CT();
    }
    //Phân trang chi tiết Khách hàng
    self.currentPage_CTKH = ko.observable(1);
    self.GetClass_CTKH = function (page) {
        return (page.SoTrang === self.currentPage_CTKH()) ? "click" : "";
    };
    self.selecPage_CTKH = function () {
        if (AllPage_CTKH > 4) {
            for (var i = 3; i < AllPage_CTKH; i++) {
                self.SumNumberPageReport_CTKH.pop(i + 1);
            }
            self.SumNumberPageReport_CTKH.push({ SoTrang: 4 });
            self.SumNumberPageReport_CTKH.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReport_CTKH.pop(i);
            }
            for (var j = 0; j < AllPage_CTKH; j++) {
                self.SumNumberPageReport_CTKH.push({ SoTrang: j + 1 });
            }
        }
        $('#StartPage_CTKH').hide();
        $('#BackPage_CTKH').hide();
        $('#NextPage_CTKH').show();
        $('#EndPage_CTKH').show();
    }
    self.ReserPage_CTKH = function (item) {
        if (_pageNumber_CTKH > 1 && AllPage_CTKH > 5/* && nextPage < AllPage - 1*/) {
            if (_pageNumber_CTKH > 3 && _pageNumber_CTKH < AllPage_CTKH - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CTKH.replace(self.SumNumberPageReport_CTKH()[i], { SoTrang: parseInt(_pageNumber_CTKH) + i - 2 });
                }
            }
            else if (parseInt(_pageNumber_CTKH) === parseInt(AllPage_CTKH) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CTKH.replace(self.SumNumberPageReport_CTKH()[i], { SoTrang: parseInt(_pageNumber_CTKH) + i - 3 });
                }
            }
            else if (_pageNumber_CTKH == AllPage_CTKH) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CTKH.replace(self.SumNumberPageReport_CTKH()[i], { SoTrang: parseInt(_pageNumber_CTKH) + i - 4 });
                }
            }
            else if (_pageNumber_CTKH < 4) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReport_CTKH.replace(self.SumNumberPageReport_CTKH()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumber_CTKH == 1 && AllPage_CTKH > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReport_CTKH.replace(self.SumNumberPageReport_CTKH()[i], { SoTrang: parseInt(_pageNumber_CTKH) + i });
            }
        }
        if (_pageNumber_CTKH > 1) {
            $('#StartPage_CTKH').show();
            $('#BackPage_CTKH').show();
        }
        else {
            $('#StartPage_CTKH').hide();
            $('#BackPage_CTKH').hide();
        }
        if (_pageNumber_CTKH == AllPage_CTKH) {
            $('#NextPage_CTKH').hide();
            $('#EndPage_CTKH').hide();
        }
        else {
            $('#NextPage_CTKH').show();
            $('#EndPage_CTKH').show();
        }
        self.currentPage_CTKH(parseInt(_pageNumber_CTKH));
    }
    self.NextPage_CTKH = function (item) {
        if (_pageNumber_CTKH < AllPage_CTKH) {
            _pageNumber_CTKH = _pageNumber_CTKH + 1;
            self.pageNumber_CTKH(_pageNumber_CTKH);
            self.ReserPage_CTKH();
        }
    };
    self.BackPage_CTKH = function (item) {
        if (_pageNumber_CTKH > 1) {
            _pageNumber_CTKH = _pageNumber_CTKH - 1;
            self.pageNumber_CTKH(_pageNumber_CTKH);
            self.ReserPage_CTKH();
        }
    };
    self.EndPage_CTKH = function (item) {
        _pageNumber_CTKH = AllPage_CTKH;
        self.pageNumber_CTKH(_pageNumber_CTKH);
        self.ReserPage_CTKH();
    };
    self.StartPage_CTKH = function (item) {
        _pageNumber_CTKH = 1;
        self.pageNumber_CTKH(_pageNumber_CTKH);
        self.ReserPage_CTKH();
    };
    self.gotoNextPage_CTKH = function (item) {
        _pageNumber_CTKH = item.SoTrang;
        self.pageNumber_CTKH(_pageNumber_CTKH);
        self.ReserPage_CTKH();
    }
    // in báo cáo
    self.BaoCaoBanHang_TongHop_Print = ko.observableArray();
    self.BaoCaoBanHang_ChiTiet_Print = ko.observableArray();
    self.BaoCaoBanHang_TheoNhomHang_Print = ko.observableArray();
    self.BaoCaoBanHang_TheoKhachHang_Print = ko.observableArray();
    self.BaoCaoBanHang_TheoNhanVien_Print = ko.observableArray();
    self.BaoCaoBanHang_HangTraLai_Print = ko.observableArray();
    self.BaoCaoBanHang_LoiNhuan_Print = ko.observableArray();

    self.hide = function () {
        self.loadCheckbox(self.columnCheckType());
        var strPrint = '';
        $('#table-reponsivetr .tab-pane').each(function () {
            if (!$(this).hasClass('active')) {
                $(this).hide();
            }
        });
        strPrint += document.getElementById('printtable').innerHTML;
        PrintExtraReportTr(strPrint);
        $('#table-reponsivetr .tab-pane').each(function () {
            if (!$(this).hasClass('active')) {
                $(this).show();
            }
        });
    }
    //return self;

    // report by staff: filter depatment
    self.PhongBans = ko.observableArray();
    self.ListIDPhongBan_Chosed = ko.observableArray();
    var treeDepartment = '';

    function GetTree_NhomHangHoa() {
        if (navigator.onLine) {
            ajaxHelper('/api/DanhMuc/NS_NhanVienAPI/' + 'GetTreePhongBan?chinhanhId=' + _id_DonVi, 'GET').done(function (data) {
                if (data.length > 0) {
                    data = data.sort((a, b) => a.text.localeCompare(b.text, undefined, { caseFirst: "upper" }));
                }
                self.PhongBans(data);
                //  bind data on tree
                treeDepartment = $('#treeDepartment').tree({
                    primaryKey: 'id',
                    uiLibrary: 'bootstrap',
                    dataSource: data,
                    checkboxes: false,
                }).on('select', function (e, node, id) {
                    reportSale.GetChildenID_Department(id);
                    reportSale.LoadReport();
                });
            });
        }
    }
    GetTree_NhomHangHoa();

    self.GetChildenID_Department = function (idNhom) {
        var arrID = [];
        var nhom = $.grep(self.PhongBans(), function (x) {
            return x.id === idNhom;
        });
        if (nhom.length > 0) {
            for (let i = 0; i < nhom[0].children.length; i++) {
                arrID.push(nhom[0].children[i].id);

                for (let j = 0; j < nhom[0].children[i].children.length; j++) {
                    arrID.push(nhom[0].children[i].children[j].id);
                }
            }
        }
        arrID.push(idNhom);
        self.ListIDPhongBan_Chosed(arrID);
    }

    $('#txtSearchDepartment').keypress(function (e) {
        if (e.keyCode === 13) {
            var filter = locdau($(this).val());
            var arr = GetChildren_Department([], self.PhongBans(), filter, [], true);
            treeDepartment.destroy();
            treeDepartment = $('#treeDepartment').tree({
                primaryKey: 'id',
                uiLibrary: 'bootstrap',
                dataSource: arr,
                checkboxes: false,
            }).on('select', function (e, node, id) {
                reportSale.GetChildenID_Department(id);
                reportSale.LoadReport();
            });
        }
    });

    function GetChildren_Department(arrParent, arrJson, txtSearch, arr, isRoot) {
        if (txtSearch === '') {
            return self.PhongBans();
        }
        for (let i = 0; i < arrJson.length; i++) {
            let name = locdau(arrJson[i].text);
            if (name.indexOf(txtSearch) > -1) {
                if (isRoot) {
                    arr.push(arrJson[i]);
                }
                else {
                    var ex = $.grep(arr, function (x) {
                        return x.id === arrParent.id;
                    })
                    if (ex.length === 0) {
                        arr.push(arrParent);
                    }
                    else {
                        // neu da ton tai, thoat vong for of children
                        return;
                    }
                }
            }
            if (arrJson[i].children.length > 0) {
                GetChildren_Department(arrJson[i], arrJson[i].children, txtSearch, arr, false);
            }
        }
        return arr;
    }

    self.ResetCurrentPage = function () {
        _pageNumber = 1;
        _pageSize = self.pageSize();

        let moiquantam = parseInt(self.check_MoiQuanTam());
        if ($.inArray(moiquantam, [1, 2, 52, 10]) > -1) {//banhang tonghop + chitiet + bc tansuat + bc dinhdanh
            self.LoadReport();
        }
        else {
            switch (moiquantam) {
                case 3:
                    self.pageNumber_NH(1);
                    AllPage = Math.ceil(self.BaoCaoBanHang_TheoNhomHang().length / self.pageSize());
                    break;
                case 4:
                    self.pageNumber_KH(1);
                    AllPage = Math.ceil(self.BaoCaoBanHang_TheoKhachHang().length / self.pageSize());
                    break;
                case 6:
                    self.pageNumber_NV(1);
                    AllPage = Math.ceil(self.BaoCaoBanHang_TheoNhanVien().length / self.pageSize());
                    break;
                case 7:
                    self.pageNumber_HTL(1);
                    AllPage = Math.ceil(self.BaoCaoBanHang_HangTraLai().length / self.pageSize());
                    break;
                case 8:
                    self.pageNumber_LN(1);
                    AllPage = Math.ceil(self.BaoCaoBanHang_LoiNhuan().length / self.pageSize());
                    break;
                case 9:
                    self.pageNumber_KM(1);
                    AllPage = Math.ceil(self.BaoCaoHangKhuyenMai().length / self.pageSize());
                    break;
            }
            self.ReserPage();
        }
    };
}
var reportSale = new ViewModal();
ko.applyBindings(reportSale, document.getElementById('divPage'));
var nhatkyGD = new modelDiaryTrans();
ko.applyBindings(nhatkyGD, document.getElementById('modalDiaryTrans'));

$('#selec-all-DonVi').parent().on('hide.bs.dropdown', function () {
    reportSale.LoadReport();
});

$('#trans_ddlDonVi').on('hide.bs.dropdown', function () {
    nhatkyGD.SearchNhatKy();
});