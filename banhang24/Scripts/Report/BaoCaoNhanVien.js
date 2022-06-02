var ViewModal = function () {
    var self = this;
    var NhanVienUri = '/api/DanhMuc/NS_NhanVienAPI/';
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/'
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.TenChiNhanh = ko.observable($('#_txtTenDonVi').text());
    self.LoaiBaoCao = ko.observable('nhân viên')
    var _rdTime = 1;
    // TuanDL Cache Show Hide Column Grid
    self.listCheckbox = ko.observableArray();
    self.columnCheckType = ko.observable(1);
    var Key_Form = 'Key_ReportStaff';
    self.loadCheckbox = function (type) {
        self.columnCheckType(type);
        $.getJSON("api/DanhMuc/ReportAPI/GetChecked?type=" + self.columnCheckType() + "&group=" + $('#ID_loaibaocao').val(), function (data) {
            self.listCheckbox(data);
            loadHtmlGrid();
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
 
    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
        $('.table-reponsive').css('display', 'block');
        if (self.columnCheckType() === 31) {
            sleep(300).then(() => {
                colspanTr();
                    
                });
        }
    }
    self.loadCheckbox(31);
    $('#select-column').on('change', '.dropdown-list ul li input[type = checkbox]', function(i) {
        var valueCheck = $(this).val();
        var index = $('.dropdown-list ul li input').index($(this));

        LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, index);
        $('.' + valueCheck).toggle();
        colspanTr();
      
    });
    $('#select-column').on('click', '.dropdown-list ul li', function (i) {
        var index = $('.dropdown-list ul li').index($(this));

        if ($(this).find('input[type = checkbox]').is(':checked')) {
            $(this).find('input[type = checkbox]').prop("checked", false);
        }
        else {
            $(this).find('input[type = checkbox]').prop("checked", true);
        }
        var valueCheck = $(this).find('input[type = checkbox]').val();
        LocalCaches.AddColumnHidenGrid(Key_Form + self.columnCheckType(), valueCheck, index);
        $('.' + valueCheck).toggle();
        colspanTr();
    });
    
    function colspanTr() {// ẩn hiện các cột nhóm tiêu đề
        if (self.columnCheckType() === 31) {
            var current = localStorage.getItem(Key_Form + self.columnCheckType());
            if (current) {
                current = JSON.parse(current);
                var thongtin = current.filter(o => o.Value < 11).length;
                var chinhtri = current.filter(o => o.Value > 10).length;

                $('.thongtinchung').attr("colspan", (11 - thongtin));

                $('.thong-tin-chinh-tri').attr("colspan", (9 - chinhtri));
                if (thongtin === 11) {
                    $('.thongtinchung').hide();
                }
                else {
                    $('.thongtinchung').show();
                }
                if (chinhtri === 9) {
                    $('.thong-tin-chinh-tri').hide();
                }
                else {
                    $('.thong-tin-chinh-tri').show();
                }
            }

        }
    }
    //--- End TuanDl
    self.MoiQuanTam = ko.observable('Báo cáo tổng hợp danh sách nhân viên');
    var dt1 = new Date();
    var dt2 = new Date();
    var _timeStart = '2015-09-26'
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');
    self.TodayBC = ko.observable('Ngày tạo: Toàn thời gian');
    var _timeBirth_Start = '1900-01-01'
    var _timeBirth_End = moment(new Date(dt1.setDate(dt1.getDate() + 1))).format('YYYY-MM-DD');

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
    var _pageSize_CT = 10;
    var _pageNumber_CTKH = 1;
    var _pageSize_CTKH = 10;
    self.SumRowsHangHoa = ko.observable();
    self.SumRowsHangHoa_CT = ko.observable();
    self.SumRowsHangHoa_CTKH = ko.observable();
    self.pageSize = ko.observable(10);
    self.pageSizes = [10, 20, 30, 40, 50];
    var AllPage;
    var AllPage_CT;
    var AllPage_CTKH;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    self.pageNumber_TH = ko.observable(1);
    self.pageNumber_HD = ko.observable(1);
    self.pageNumber_BH = ko.observable(1);
    self.pageNumber_DT = ko.observable(1);
    self.pageNumber_KT = ko.observable(1);
    self.pageNumber_LPC = ko.observable(1);
    self.pageNumber_MGT = ko.observable(1);
    self.pageNumber_DAOTAO = ko.observable(1);
    self.pageNumber_QTCT = ko.observable(1);
    self.pageNumber_GD = ko.observable(1);
    self.pageNumber_SK = ko.observable(1);
    $('.ip_TimeReport').val("Toàn thời gian");
    self.Loc_TinhTrangKD = ko.observable('2');
    var tk = null;
    self.PhongBans = ko.observableArray();
    self.AllPhongBans = ko.observableArray();
    //trinhpv phân quyền
    self.BaoCaoBanHang = ko.observable();
    self.BCNV_TongHop = ko.observable();
    self.BCNV_TheoHopDong = ko.observable();
    self.BCNV_TheoBaoHiem = ko.observable();
    self.BCNV_TheoDoTuoi = ko.observable();
    self.BCNV_KhenThuongKyLuat = ko.observable();
    self.BCNV_LuongPhuCap = ko.observable();
    self.BCNV_MienGiamThue = ko.observable();
    self.BCNV_DaoTao = ko.observable();
    self.BCNV_QuaTrinhCongTac = ko.observable();
    self.BCNV_ThongTinGiaDinh = ko.observable();
    self.BCNV_ThongTinSucKhoe = ko.observable();
    self.BCNV_XuatFile = ko.observable();
    
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_TongHop", "GET").done(function (data) {
        //    self.BCNV_TongHop(data);
        //    self.LoadReport();
        //    //getDonVi();
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_TheoHopDong", "GET").done(function (data) {
        //    self.BCNV_TheoHopDong(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_TheoBaoHiem", "GET").done(function (data) {
        //    self.BCNV_TheoBaoHiem(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_TheoDoTuoi", "GET").done(function (data) {
        //    self.BCNV_TheoDoTuoi(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_KhenThuongKyLuat", "GET").done(function (data) {
        //    self.BCNV_KhenThuongKyLuat(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_LuongPhuCap", "GET").done(function (data) {
        //    self.BCNV_LuongPhuCap(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_MienGiamThue", "GET").done(function (data) {
        //    self.BCNV_MienGiamThue(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_DaoTao", "GET").done(function (data) {
        //    self.BCNV_DaoTao(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_QuaTrinhCongTac", "GET").done(function (data) {
        //    self.BCNV_QuaTrinhCongTac(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_ThongTinGiaDinh", "GET").done(function (data) {
        //    self.BCNV_ThongTinGiaDinh(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_ThongTinSucKhoe", "GET").done(function (data) {
        //    self.BCNV_ThongTinSucKhoe(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNV_XuatFile", "GET").done(function (data) {
        //    self.BCNV_XuatFile(data);
        //})

        if (VHeader.Quyen.indexOf('BCNV_TongHop') > -1) {
            self.BCNV_TongHop('BCNV_TongHop');
            self.LoadReport();
        }
        else {
            self.BCNV_TongHop('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_TheoHopDong') > -1) {
            self.BCNV_TheoHopDong('BCNV_TheoHopDong');
        }
        else {
            self.BCNV_TheoHopDong('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_TheoBaoHiem') > -1) {
            self.BCNV_TheoBaoHiem('BCNV_TheoBaoHiem');
        }
        else {
            self.BCNV_TheoBaoHiem('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_TheoDoTuoi') > -1) {
            self.BCNV_TheoDoTuoi('BCNV_TheoDoTuoi');
        }
        else {
            self.BCNV_TheoDoTuoi('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_KhenThuongKyLuat') > -1) {
            self.BCNV_KhenThuongKyLuat('BCNV_KhenThuongKyLuat');
        }
        else {
            self.BCNV_KhenThuongKyLuat('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_LuongPhuCap') > -1) {
            self.BCNV_LuongPhuCap('BCNV_LuongPhuCap');
        }
        else {
            self.BCNV_LuongPhuCap('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_MienGiamThue') > -1) {
            self.BCNV_MienGiamThue('BCNV_MienGiamThue');
        }
        else {
            self.BCNV_MienGiamThue('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_DaoTao') > -1) {
            self.BCNV_DaoTao('BCNV_DaoTao');
        }
        else {
            self.BCNV_DaoTao('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_QuaTrinhCongTac') > -1) {
            self.BCNV_QuaTrinhCongTac('BCNV_QuaTrinhCongTac');
        }
        else {
            self.BCNV_QuaTrinhCongTac('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_ThongTinGiaDinh') > -1) {
            self.BCNV_ThongTinGiaDinh('BCNV_ThongTinGiaDinh');
        }
        else {
            self.BCNV_ThongTinGiaDinh('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_ThongTinSucKhoe') > -1) {
            self.BCNV_ThongTinSucKhoe('BCNV_ThongTinSucKhoe');
        }
        else {
            self.BCNV_ThongTinSucKhoe('false');
        }

        if (VHeader.Quyen.indexOf('BCNV_XuatFile') > -1) {
            self.BCNV_XuatFile('BCNV_XuatFile');
        }
        else {
            self.BCNV_XuatFile('false');
        }
    }
    // load Nhóm khách hàng
    var _tennhomDT = null;
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
    //============================
    self.ListBaoHiem_filter = ko.observableArray();
    self.ListChinhTri_filter = ko.observableArray();
    self.ListLoaiHopDong_filter = ko.observableArray();
    self.ListDanToc_filter = ko.observableArray();
    //self.ListBaoHiem_filter_computed = ko.computed(function () {
    //    return self.ListBaoHiem_filter().filter(o => o.IsSelected === true);
    //});
    //self.ListChinhTri_filter_computed = ko.computed(function () {
    //    return self.ListChinhTri_filter().filter(o => o.IsSelected === true);
    //});
    //self.ListLoaiHopDong_filter_computed = ko.computed(function () {
    //    return self.ListLoaiHopDong_filter().filter(o => o.IsSelected === true);
    //});
    self.ListDanToc_filter_computed = ko.computed(function () {
        return self.ListDanToc_filter().filter(o => o.IsSelected === true);
    });
    function loadFilterOpenHRM() {
        $.getJSON(NhanVienUri + "GetBaoHienFilter", function (data) {
            self.ListBaoHiem_filter(data);
        });
        $.getJSON(NhanVienUri + "GetChinhTriFilter", function (data) {
            self.ListChinhTri_filter(data);
        });
        $.getJSON(NhanVienUri + "GetLOaiHopDongFilter", function (data) {
            self.ListLoaiHopDong_filter(data);
        });
        $.getJSON(NhanVienUri + "GetDanTocFilter", function (data) {
            self.ListDanToc_filter(data);
        });
    }
    loadFilterOpenHRM();
    //load hợp đồng
    var _LoaiHopDong = "0,1,2,3,4,5";
    self.List_arrayLoaiHopDong = ko.observableArray();
    self.CloseLoaiHopDong = function (item) {
        _LoaiHopDong = null;
        self.List_arrayLoaiHopDong.remove(item);
        for (var i = 0; i < self.List_arrayLoaiHopDong().length; i++) {
            if (_LoaiHopDong == null) {
                _LoaiHopDong = self.List_arrayLoaiHopDong()[i].ID;
            }
            else {
                _LoaiHopDong = self.List_arrayLoaiHopDong()[i].ID + "," + _LoaiHopDong;
            }
        }
        if (self.List_arrayLoaiHopDong().length === 0) {
            $("#NoteNameLoaiHopDong").attr("placeholder", "--Chọn--");
            _LoaiHopDong = "0,1,2,3,4,5";
        }
        $('#selec-all-LoaiHopDong li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }
    self.SelectedLoaiHopDong = function (item) {
        _LoaiHopDong = null;
        var arrIDLoaiHopDong = [];
        for (var i = 0; i < self.List_arrayLoaiHopDong().length; i++) {
            if ($.inArray(self.List_arrayLoaiHopDong()[i], arrIDLoaiHopDong) === -1) {
                arrIDLoaiHopDong.push(self.List_arrayLoaiHopDong()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDLoaiHopDong) === -1) {
            self.List_arrayLoaiHopDong.push(item);
            $('#NoteNameLoaiHopDong').removeAttr('placeholder');
            for (var i = 0; i < self.List_arrayLoaiHopDong().length; i++) {
                if (_LoaiHopDong == null) {
                    _LoaiHopDong = self.List_arrayLoaiHopDong()[i].ID;
                }
                else {
                    _LoaiHopDong = self.List_arrayLoaiHopDong()[i].ID + "," + _LoaiHopDong;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-LoaiHopDong li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //load chính trị
    var _LoaiChinhTri = "0,1,2,3";
    self.List_arrayLoaiChinhTri = ko.observableArray();
    self.CloseLoaiChinhTri = function (item) {
        _LoaiChinhTri = null;
        self.List_arrayLoaiChinhTri.remove(item);
        for (var i = 0; i < self.List_arrayLoaiChinhTri().length; i++) {
            if (_LoaiChinhTri == null) {
                _LoaiChinhTri = self.List_arrayLoaiChinhTri()[i].ID;
            }
            else {
                _LoaiChinhTri = self.List_arrayLoaiChinhTri()[i].ID + "," + _LoaiChinhTri;
            }
        }
        if (self.List_arrayLoaiChinhTri().length === 0) {
            $("#NoteNameLoaiChinhTri").attr("placeholder", "--Chọn--");
            _LoaiChinhTri = "0,1,2,3";
        }
        $('#selec-all-LoaiChinhTri li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }
    self.SelectedLoaiChinhTri = function (item) {
        _LoaiChinhTri = null;
        var arrIDLoaiChinhTri = [];
        for (var i = 0; i < self.List_arrayLoaiChinhTri().length; i++) {
            if ($.inArray(self.List_arrayLoaiChinhTri()[i], arrIDLoaiChinhTri) === -1) {
                arrIDLoaiChinhTri.push(self.List_arrayLoaiChinhTri()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDLoaiChinhTri) === -1) {
            self.List_arrayLoaiChinhTri.push(item);
            $('#NoteNameLoaiChinhTri').removeAttr('placeholder');
            for (var i = 0; i < self.List_arrayLoaiChinhTri().length; i++) {
                if (_LoaiChinhTri == null) {
                    _LoaiChinhTri = self.List_arrayLoaiChinhTri()[i].ID;
                }
                else {
                    _LoaiChinhTri = self.List_arrayLoaiChinhTri()[i].ID + "," + _LoaiChinhTri;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-LoaiChinhTri li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //load BaoHiem
    var _LoaiBaoHiem = "0,1,2,3";
    self.List_arrayLoaiBaoHiem = ko.observableArray();
    self.CloseLoaiBaoHiem = function (item) {
        _LoaiBaoHiem = null;
        self.List_arrayLoaiBaoHiem.remove(item);
        for (var i = 0; i < self.List_arrayLoaiBaoHiem().length; i++) {
            if (_LoaiBaoHiem == null) {
                _LoaiBaoHiem = self.List_arrayLoaiBaoHiem()[i].ID;
            }
            else {
                _LoaiBaoHiem = self.List_arrayLoaiBaoHiem()[i].ID + "," + _LoaiBaoHiem;
            }
        }
        if (self.List_arrayLoaiBaoHiem().length === 0) {
            $("#NoteNameLoaiBaoHiem").attr("placeholder", "--Chọn--");
            _LoaiBaoHiem = "0,1,2,3";
        }
        $('#selec-all-LoaiBaoHiem li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }
    self.SelectedLoaiBaoHiem = function (item) {
        _LoaiBaoHiem = null;
        var arrIDLoaiBaoHiem = [];
        for (var i = 0; i < self.List_arrayLoaiBaoHiem().length; i++) {
            if ($.inArray(self.List_arrayLoaiBaoHiem()[i], arrIDLoaiBaoHiem) === -1) {
                arrIDLoaiBaoHiem.push(self.List_arrayLoaiBaoHiem()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDLoaiBaoHiem) === -1) {
            self.List_arrayLoaiBaoHiem.push(item);
            $('#NoteNameLoaiBaoHiem').removeAttr('placeholder');
            for (var i = 0; i < self.List_arrayLoaiBaoHiem().length; i++) {
                if (_LoaiBaoHiem == null) {
                    _LoaiBaoHiem = self.List_arrayLoaiBaoHiem()[i].ID;
                }
                else {
                    _LoaiBaoHiem = self.List_arrayLoaiBaoHiem()[i].ID + "," + _LoaiBaoHiem;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-LoaiBaoHiem li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    //load Dân tộc
    var _LoaiDanToc = null;
    self.List_arrayLoaiDanToc = ko.observableArray();
    self.CloseLoaiDanToc = function (item) {
        _LoaiDanToc = null;
        self.List_arrayLoaiDanToc.remove(item);
        for (var i = 0; i < self.List_arrayLoaiDanToc().length; i++) {
            if (_LoaiDanToc == null) {
                _LoaiDanToc = self.List_arrayLoaiDanToc()[i].Name;
            }
            else {
                _LoaiDanToc = self.List_arrayLoaiDanToc()[i].Name + "," + _LoaiDanToc;
            }
        }
        if (self.List_arrayLoaiDanToc().length === 0) {
            $("#NoteNameLoaiDanToc").attr("placeholder", "--Chọn--");
        }
        $('#selec-all-LoaiDanToc li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    }
    self.SelectedLoaiDanToc = function (item) {
        _LoaiDanToc = null;
        var arrIDLoaiDanToc = [];
        for (var i = 0; i < self.List_arrayLoaiDanToc().length; i++) {
            if ($.inArray(self.List_arrayLoaiDanToc()[i], arrIDLoaiDanToc) === -1) {
                arrIDLoaiDanToc.push(self.List_arrayLoaiDanToc()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDLoaiDanToc) === -1) {
            self.List_arrayLoaiDanToc.push(item);
            $('#NoteNameLoaiDanToc').removeAttr('placeholder');
            for (var i = 0; i < self.List_arrayLoaiDanToc().length; i++) {
                if (_LoaiDanToc == null) {
                    _LoaiDanToc = self.List_arrayLoaiDanToc()[i].Name;
                }
                else {
                    _LoaiDanToc = self.List_arrayLoaiDanToc()[i].Name + "," + _LoaiDanToc;
                }
            }
            self.LoadReport();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-LoaiDanToc li').each(function () {
            if ($(this).attr('id').toString() === item.ID.toString()) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

    }
    ////load đơn vị
    //function getDonVi() {
    //    ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameDonViSeach, "GET").done(function (data) {
    //        self.DonVis(data);
    //         var objParentDV = {
    //            ID: '00000000-4000-0000-0004-000000000007',
    //            SoDienThoai: '0989861122',
    //            TenDonVi: 'Chi nhánh mặc định',
    //        }
    //         self.searchDonVi(data);
    //         self.DonVis.unshift(objParentDV)
    //         self.searchDonVi.unshift(objParentDV)
    //        if (self.DonVis().length < 2)
    //            $('.showChiNhanh').hide();
    //        else
    //            $('.showChiNhanh').show();
    //        for (var i = 0; i < self.DonVis().length; i++) {
    //            if (self.DonVis()[i].ID == _idDonViSeach) {
    //                self.TenChiNhanh(self.DonVis()[i].TenDonVi);
    //                self.SelectedDonVi(self.DonVis()[i]);
    //            }
    //        }
    //    });
    //}
    ////Lua chon don vi
    //self.CloseDonVi = function (item) {
    //    _idDonViSeach = null;
    //    var TenChiNhanh;
    //    self.MangChiNhanh.remove(item);
    //    for (var i = 0; i < self.MangChiNhanh().length; i++) {
    //        if (_idDonViSeach == null) {
    //            _idDonViSeach = self.MangChiNhanh()[i].ID;
    //            TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
    //        }
    //        else {
    //            _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
    //            TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
    //        }
    //    }
    //    if (self.MangChiNhanh().length === 0) {
    //        $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
    //        TenChiNhanh = 'Tất cả chi nhánh.'
    //        for (var i = 0; i < self.searchDonVi().length; i++) {
    //            if (_idDonViSeach == null)
    //                _idDonViSeach = self.searchDonVi()[i].ID;
    //            else
    //                _idDonViSeach = self.searchDonVi()[i].ID + "," + _idDonViSeach;
    //        }
    //    }
    //    self.TenChiNhanh(TenChiNhanh);
    //    $('#selec-all-DonVi li').each(function () {
    //        if ($(this).attr('id') === item.ID) {
    //            $(this).find('i').remove();
    //        }
    //    });
    //    self.LoadReport();
    //}

    //self.SelectedDonVi = function (item) {
    //    _idDonViSeach = null;
    //    var TenChiNhanh;
    //    var arrIDDonVi = [];
    //    for (var i = 0; i < self.MangChiNhanh().length; i++) {
    //        if ($.inArray(self.MangChiNhanh()[i], arrIDDonVi) === -1) {
    //            arrIDDonVi.push(self.MangChiNhanh()[i].ID);
    //        }
    //    }
    //    if ($.inArray(item.ID, arrIDDonVi) === -1) {
    //        self.MangChiNhanh.push(item);
    //        $('#NoteNameDonVi').removeAttr('placeholder');
    //        for (var i = 0; i < self.MangChiNhanh().length; i++) {
    //            if (_idDonViSeach == null) {
    //                _idDonViSeach = self.MangChiNhanh()[i].ID;
    //                TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
    //            }
    //            else {
    //                _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
    //                TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
    //            }
    //        }
    //        self.TenChiNhanh(TenChiNhanh);
    //        self.LoadReport();
    //    }
    //    //thêm dấu check vào đối tượng được chọn
    //    $('#selec-all-DonVi li').each(function () {
    //        if ($(this).attr('id') === item.ID) {
    //            $(this).find('i').remove();
    //            $(this).append('<i class="fa fa-check check-after-li"></i>')
    //        }
    //    });

    //}
    ////lọc đơn vị
    //self.NoteNameDonVi = function () {
    //    var arrDonVi = [];
    //    var itemSearch = locdau($('#NoteNameDonVi').val().toLowerCase());
    //    for (var i = 0; i < self.searchDonVi().length; i++) {
    //        var locdauInput = locdau(self.searchDonVi()[i].TenDonVi).toLowerCase();
    //        var R = locdauInput.split(itemSearch);
    //        if (R.length > 1) {
    //            arrDonVi.push(self.searchDonVi()[i]);
    //        }
    //    }
    //    self.DonVis(arrDonVi);
    //    if ($('#NoteNameDonVi').val() == "") {
    //        self.DonVis(self.searchDonVi());
    //    }
    //}
    //$('#NoteNameDonVi').keypress(function (e) {
    //    if (e.keyCode == 13 && self.DonVis().length > 0) {
    //        self.SelectedDonVi(self.DonVis()[0]);
    //    }
    //});
    //load phòng ban

    function GetListPhongBanTheoChiNhanh() {
        $.getJSON("/api/DanhMuc/NS_NhanSuAPI/GetListPhongBanTheoChiNhanh?id=" + _id_DonVi, function (data) {
            if (data.res) {
                self.PhongBans(data.dataSoure);
                self.AllPhongBans(data.dataSoure);
            }
            else {
                commonStatisJs.ShowMessageDanger(data.mess);
            }
        });
    }

    GetListPhongBanTheoChiNhanh();

    function GetChildren_NhomHH(arrParent, arrJson, txtSearch, arr, isRoot) {
        if (txtSearch === '') {
            return self.AllPhongBans();
        }
        for (var i = 0; i < arrJson.length; i++) {
            let tenNhom = locdau(arrJson[i].text);
            if (tenNhom.indexOf(txtSearch) > -1) {
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
                GetChildren_NhomHH(arrJson[i], arrJson[i].children, txtSearch, arr, false);
            }
        }
        return arr;
    }

    self.NotePhongBan = function (e) {
        let txt = locdau($(event.currentTarget).val());
        var arr = GetChildren_NhomHH([], self.AllPhongBans(), txt, [], true);
        self.PhongBans(arr);
    };
    var _ID_PhongBan = null;
    self.SelectReport_PhongBan = function (item) {
        _ID_PhongBan = item.id;
        _pageNumber = 1;
        if (item.id == undefined) {
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
    $('.SelectALLPhongBan').on('click', function () {
        _ID_PhongBan = null;
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.chose_TinhTrangKD input').on('click', function () {
        TinhTrangHH = $(this).val();
        _pageNumber = 1;
        self.Loc_TinhTrangKD($(this).val());
        self.LoadReport();
    });
    $("#txtDoTuoiFrom").keypress(function (e) {
        var key = e.keyCode
        if (e.keyCode == 13)
        {
            self.LoadReport();
        }
    });
    $("#txtDoTuoiTo").keypress(function (e) {
        var key = e.keyCode
        if (e.keyCode == 13) {
            self.LoadReport();
        }
    });
    $('.chose_kieubang').on('click', 'li', function () {
       
        $(".supplier").hide();
        self.loadCheckbox($(this).data('id'));
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        self.check_MoiQuanTam($(this).val());
        if (self.check_MoiQuanTam() == 1) {
            self.LoaiBaoCao('nhân viên');
            self.MoiQuanTam('Báo cáo tổng hợp danh sách nhân viên');
        }
        else if (self.check_MoiQuanTam() == 2) {
            self.LoaiBaoCao('hợp đồng');
            self.MoiQuanTam('Báo cáo nhân viên theo loại hợp đồng');
        }
        else if (self.check_MoiQuanTam() == 3) {
            self.LoaiBaoCao('bảo hiểm');
            self.MoiQuanTam('Báo cáo nhân viên theo bảo hiểm');
        }
        else if (self.check_MoiQuanTam() == 4) {
            $(".supplier").show();
            self.LoaiBaoCao('nhân viên');
            self.MoiQuanTam('Báo cáo nhân viên theo độ tuổi');
        }
        else if (self.check_MoiQuanTam() == 5) {
            self.MoiQuanTam('Báo cáo nhân viên theo khen thưởng kỷ luật');
        }
        else if (self.check_MoiQuanTam() == 6) {
            self.LoaiBaoCao('quyết định');
            self.MoiQuanTam('Báo cáo nhân viên theo lương, phụ cấp');
        }
        else if (self.check_MoiQuanTam() == 7) {
            self.LoaiBaoCao('quyết định');
            self.MoiQuanTam('Báo cáo nhân viên theo miễn giảm thuế');
        }
        else if (self.check_MoiQuanTam() == 8) {
            self.LoaiBaoCao('chi tiết đào tạo');
            self.MoiQuanTam('Báo cáo nhân viên theo quy trình đào tạo');
        }
        else if (self.check_MoiQuanTam() == 9) {
            self.LoaiBaoCao('quá trình công tác');
            self.MoiQuanTam('Báo cáo nhân viên theo quá trình công tác');
        }
        else if (self.check_MoiQuanTam() == 10) {
            self.LoaiBaoCao('thông tin gia đình');
            self.MoiQuanTam('Báo cáo nhân viên theo thông tin gia đình');
        }
        else if (self.check_MoiQuanTam() == 11) {
            self.LoaiBaoCao('thông tin sức khỏe');
            self.MoiQuanTam('Báo cáo nhân viên theo thông tin sức khỏe');
        }
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.choose_txtTime li').on('click', function () {
        //self.TodayBC($(this).text())
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeStart = '2015-09-26'
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày tạo: Toàn thời gian');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày tạo: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            self.TodayBC('Ngày tạo: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
    $('.choose_txtTimeSN li').on('click', function () {
        var _rdoNgayPage = $(this).val();
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeBirth_Start = '2015-09-26'
            _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeBirth_Start = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeBirth_Start = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeBirth_End = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
        }
        //Tuần này
        else if (_rdoNgayPage === 3) {
            var currentWeekDay = datime.getDay();
            var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
            _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() + 6))).format('YYYY-MM-DD');
            _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week

        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() - datimeBC.getDay()))).format('YYYY-MM-DD');
            _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            var newtime = new Date();
            var _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate()))).format('YYYY-MM-DD');
            _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Tháng này
        else if (_rdoNgayPage === 6) {
            _timeBirth_Start = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            _timeBirth_End = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
        }
        //Tháng trước
        else if (_rdoNgayPage === 7) {
            _timeBirth_Start = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            _timeBirth_End = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
        }
        //30 ngày qua
        else if (_rdoNgayPage === 8) {
            _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            var newtime = new Date();
            _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Quý này
        else if (_rdoNgayPage === 9) {
            _timeBirth_Start = moment().startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('quarter'));
            _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        // Quý trước
        else if (_rdoNgayPage === 10) {
            var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
            _timeBirth_Start = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
            var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
            _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //Năm này
        else if (_rdoNgayPage === 11) {
            _timeBirth_Start = moment().startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().endOf('year'));
            _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
        }
        //năm trước
        else if (_rdoNgayPage === 12) {
            var prevYear = moment().year() - 1;
            _timeBirth_Start = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            var newtime = new Date(moment().year(prevYear).endOf('year'));
            _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
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
            self.TodayBC('Ngày tạo: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.newDateTimeSN').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        var dtBC = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeBirth_Start = picker.startDate.format('YYYY-MM-DD');
        _timeBirth_End = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        _pageNumber = 1;
        self.LoadReport();
    });
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
                self.TodayBC('Ngày tạo: Toàn thời gian');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày tạo: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.TodayBC('Ngày tạo: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
                    self.TodayBC('Ngày tạo: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    })
    $('.choose_TimeSinhNhat input').on('click', function () {
        if ($(this).val() == 1) {
            $('.ip_TimeReportSN').removeAttr('disabled');
            $('.dr_TimeReportSN').attr("data-toggle", "dropdown");
            $('.ip_DateReportSN').attr('disabled', 'false');
            var _rdoNgayPage = $('.ip_TimeReport').val();
            var datime = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === "Toàn thời gian") {
                _timeBirth_Start = '2015-09-26'
                _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeBirth_Start = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeBirth_Start = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeBirth_End = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            }
            //Tuần này
            else if (_rdoNgayPage === "Tuần này") {
                var currentWeekDay = datime.getDay();
                var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
                _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            }
            //Tuần trước
            else if (_rdoNgayPage === "Tuần trước") {
                _timeBirth_End = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            }
            //7 ngày qua
            else if (_rdoNgayPage === "7 ngày qua") {
                _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                _timeBirth_Start = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeBirth_End = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            }
            //Tháng trước
            else if (_rdoNgayPage === "Tháng trước") {
                _timeBirth_Start = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                _timeBirth_End = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            }
            //30 ngày qua
            else if (_rdoNgayPage === "30 ngày qua") {
                _timeBirth_Start = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                var newtime = new Date();
                _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Quý này
            else if (_rdoNgayPage === "Quý này") {
                _timeBirth_Start = moment().startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('quarter'));
                _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            // Quý trước
            else if (_rdoNgayPage === "Quý trước") {
                var prevQuarter = (moment().quarter() - 1 === 0) ? 1 : moment().quarter() - 1;
                _timeBirth_Start = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                var newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //Năm này
            else if (_rdoNgayPage === "Năm này") {
                _timeBirth_Start = moment().startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().endOf('year'));
                _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            //năm trước
            else if (_rdoNgayPage === "Năm trước") {
                var prevYear = moment().year() - 1;
                _timeBirth_Start = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                var newtime = new Date(moment().year(prevYear).endOf('year'));
                _timeBirth_End = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            }
            _pageNumber = 1;
            self.LoadReport();
        }
        else if ($(this).val() == 2) {
            $('.ip_DateReportSN').removeAttr('disabled');
            $('.ip_TimeReportSN').attr('disabled', 'false');
            $('.dr_TimeReportSN').removeAttr('data-toggle');
            if ($('.ip_DateReportSN').val() != "") {
                thisDate = $('.ip_DateReportSN').val();
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
                _timeBirth_Start = moment(new Date(thisDateStart)).format('YYYY-MM-DD');
                var dt = new Date(thisDateEnd);
                _timeBirth_End = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    })
    // lựa chọn giới tính
    self.IsGioiTinh = ko.observable(2);
    $('#GioiTinhSL').on('click', 'ul li', function () {
        if (self.IsGioiTinh() != $(this).find('a').data('id') || self.IsGioiTinh() === undefined || $(this).find('a').data('id') == undefined) {
            self.IsGioiTinh($(this).find('a').data('id'));
            self.LoadReport();
        }
    });
    // lựa chọn trạng thái
    var _TrangThai = 0;
    $('#TrangThaiNV').on('click', 'ul li', function () {
        if (_TrangThai != $(this).find('a').data('id') || _TrangThai === undefined || $(this).find('a').data('id') == undefined) {
            _TrangThai = ($(this).find('a').data('id'));
            self.LoadReport();
        }
    });

    var Text_search = "";
    self.BaoCaoNhanVien_TongHop = ko.observableArray();
    self.BaoCaoNhanVien_TheoHopDong = ko.observableArray();
    self.BaoCaoNhanVien_TheoBaoHiem = ko.observableArray();
    self.BaoCaoNhanVien_TheoDoTuoi = ko.observableArray();
    self.BaoCaoNhanVien_KhenThuong = ko.observableArray();
    self.BaoCaoNhanVien_LuongPhuCap = ko.observableArray();
    self.BaoCaoNhanVien_MienThue = ko.observableArray();
    self.BaoCaoNhanVien_DaoTao = ko.observableArray();
    self.BaoCaoNhanVien_CongTac = ko.observableArray();
    self.BaoCaoNhanVien_GiaDinh = ko.observableArray();
    self.BaoCaoNhanVien_SucKhoe = ko.observableArray();
    function LoadingForm(IsShow) {
        $('.tab-show .tab-pane').each(function () {
            if ($(this).hasClass('active')) {
                var top = $(this).find('.table-reponsive').height() / 2;
                var style = "top:" + (top > 30 ? top - 30 : top) + "px";
                $(this).find('.table-reponsive').gridLoader({ show: IsShow, style: style });
            }
        });
    }
    $('#NoteNameChungTu').keypress(function (e) {
        if (e.keyCode == 13 && self.ChungTus().length > 0) {
            self.SelectedChungTu(self.ChungTus()[0]);
        }
    });
    self.LoadReport = function () {
        LoadingForm(true);
        $('.table-reponsive').css('display', 'none');
        self.pageNumber_TH(1);
        self.pageNumber_HD(1);
        self.pageNumber_BH(1);
        self.pageNumber_DT(1);
        self.pageNumber_KT(1);
        self.pageNumber_LPC(1);
        self.pageNumber_MGT(1);
        self.pageNumber_DAOTAO(1);
        self.pageNumber_QTCT(1);
        self.pageNumber_GD(1);
        self.pageNumber_SK(1);
        var array_Seach = {
            MaNhanVien: Text_search,
            ID_ChiNhanh: _id_DonVi,
            timeCreate_Start: _timeStart,
            timeCreate_End: _timeEnd,
            ID_PhongBan: _ID_PhongBan,
            GioiTinh: self.IsGioiTinh(),
            LoaiHopDong: _LoaiHopDong,
            timeBirthday_Start: _timeBirth_Start,
            timeBirthday_End: _timeBirth_End,
            LoaiChinhTri: _LoaiChinhTri,
            LoaiBaoHiem: _LoaiBaoHiem,
            LoaiDanToc: _LoaiDanToc,
            TrangThai: _TrangThai,
            columnsHide: null,
            TodayBC: null,
            TenChiNhanh: null,
            Min: $("#txtDoTuoiFrom").val() != "" ? $("#txtDoTuoiFrom").val(): 0,
            Max: $("#txtDoTuoiTo").val() != "" ? $("#txtDoTuoiTo").val(): 9999999
        }
        if (self.check_MoiQuanTam() == 1) {
            if (self.BCNV_TongHop() == "BCNV_TongHop") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_TongHop", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_TongHop(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 2) {
            if (self.BCNV_TheoHopDong() == "BCNV_TheoHopDong") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_TheoHopDong", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_TheoHopDong(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 3) {
            if (self.BCNV_TheoBaoHiem() == "BCNV_TheoBaoHiem") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_TheoBaoHiem", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_TheoBaoHiem(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 4) {
            if (self.BCNV_TheoDoTuoi() == "BCNV_TheoDoTuoi") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_TheoTuoi", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_TheoDoTuoi(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 5) {
            if (self.BCNV_KhenThuongKyLuat() == "BCNV_KhenThuongKyLuat") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_KhenThuong", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_KhenThuong(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 6) {
            if (self.BCNV_LuongPhuCap() == "BCNV_LuongPhuCap") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_LuongPhuCap", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_LuongPhuCap(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        } 
        if (self.check_MoiQuanTam() == 7) {
            if (self.BCNV_MienGiamThue() == "BCNV_MienGiamThue") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_MienGiamThue", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_MienThue(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 8) {
            if (self.BCNV_DaoTao() == "BCNV_DaoTao") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_DaoTao", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_DaoTao(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 9) {
            if (self.BCNV_QuaTrinhCongTac() == "BCNV_QuaTrinhCongTac") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_QuaTrinhCongTac", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_CongTac(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 10) {
            if (self.BCNV_ThongTinGiaDinh() == "BCNV_ThongTinGiaDinh") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_ThongTinGiaDinh", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_GiaDinh(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        if (self.check_MoiQuanTam() == 11) {
            if (self.BCNV_ThongTinSucKhoe() == "BCNV_ThongTinSucKhoe") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhanVien_ThongTinSucKhoe", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhanVien_SucKhoe(data.LstData);
                    AllPage = data.numberPage;
                    /*self.selecPage();*/
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
    }
    getQuyen_NguoiDung();
    self.BaoCaoNhanVien_TongHop_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 1) {
            var first = (self.pageNumber_TH() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_TongHop() !== null) {
                if (self.BaoCaoNhanVien_TongHop().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_TH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_TH() - 1) * self.pageSize() + self.BaoCaoNhanVien_TongHop().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_TongHop().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_TheoHopDong_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 2) {
            var first = (self.pageNumber_HD() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_TheoHopDong() !== null) {
                if (self.BaoCaoNhanVien_TheoHopDong().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_HD() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_HD() - 1) * self.pageSize() + self.BaoCaoNhanVien_TheoHopDong().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_TheoHopDong().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_TheoBaoHiem_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 3) {
            var first = (self.pageNumber_BH() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_TheoBaoHiem() !== null) {
                if (self.BaoCaoNhanVien_TheoBaoHiem().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_BH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_BH() - 1) * self.pageSize() + self.BaoCaoNhanVien_TheoBaoHiem().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_TheoBaoHiem().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_TheoDoTuoi_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 4) {
            var first = (self.pageNumber_DT() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_TheoDoTuoi() !== null) {
                if (self.BaoCaoNhanVien_TheoDoTuoi().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_DT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_DT() - 1) * self.pageSize() + self.BaoCaoNhanVien_TheoDoTuoi().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_TheoDoTuoi().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_KhenThuong_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 5) {
            var first = (self.pageNumber_KT() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_KhenThuong() !== null) {
                if (self.BaoCaoNhanVien_KhenThuong().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_KT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_KT() - 1) * self.pageSize() + self.BaoCaoNhanVien_KhenThuong().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_KhenThuong().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_LuongPhuCap_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 6) {
            var first = (self.pageNumber_LPC() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_LuongPhuCap() !== null) {
                if (self.BaoCaoNhanVien_LuongPhuCap().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_LPC() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_LPC() - 1) * self.pageSize() + self.BaoCaoNhanVien_LuongPhuCap().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_LuongPhuCap().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_MienThue_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 7) {
            var first = (self.pageNumber_MGT() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_MienThue() !== null) {
                if (self.BaoCaoNhanVien_MienThue().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_MGT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_MGT() - 1) * self.pageSize() + self.BaoCaoNhanVien_MienThue().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_MienThue().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_DaoTao_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 8) {
            var first = (self.pageNumber_DAOTAO() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_DaoTao() !== null) {
                if (self.BaoCaoNhanVien_DaoTao().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_DAOTAO() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_DAOTAO() - 1) * self.pageSize() + self.BaoCaoNhanVien_DaoTao().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_DaoTao().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_CongTac_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 9) {
            var first = (self.pageNumber_QTCT() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_CongTac() !== null) {
                if (self.BaoCaoNhanVien_CongTac().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_QTCT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_QTCT() - 1) * self.pageSize() + self.BaoCaoNhanVien_CongTac().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_CongTac().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_GiaDinh_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 10) {
            var first = (self.pageNumber_GD() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_GiaDinh() !== null) {
                if (self.BaoCaoNhanVien_GiaDinh().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_GD() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_GD() - 1) * self.pageSize() + self.BaoCaoNhanVien_GiaDinh().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_GiaDinh().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhanVien_SucKhoe_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 11) {
            var first = (self.pageNumber_SK() - 1) * self.pageSize();
            if (self.BaoCaoNhanVien_SucKhoe() !== null) {
                if (self.BaoCaoNhanVien_SucKhoe().length != 0) {
                    $('.Report_Empty').hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_SK() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_SK() - 1) * self.pageSize() + self.BaoCaoNhanVien_SucKhoe().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.Report_Empty').show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhanVien_SucKhoe().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.Select_Text = function () {
        Text_search = $('#txt_search').val();
    }
    $('#txt_search').keypress(function (e) {
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
    self.LoadKhachHang_byMaKH = function (item) {
        if (item.MaKhachHang != '' && item.MaKhachHang != null) {
            localStorage.setItem('FindKhachHang', item.MaKhachHang);
            var url = "/#/Customers";
            window.open(url);
        }
    }
    self.LoadNhanVien_byMaNV = function (item) {
        if (item.MaNhanVien != '' && item.MaNhanVien != null) {
            localStorage.setItem('FindNhanVien', item.MaNhanVien);
            var url = "/#/User";
            window.open(url);
        }
    }
    self.LoadHoaDon_byMaHD = function (item) {
        var maHD = item.MaHoaDon;
        if (maHD.indexOf('TT') > -1) {
            localStorage.setItem('FindMaPhieuChi', maHD);
            url = "/#/CashFlow"; // soquy
        }
        else {
            localStorage.setItem('FindHD', maHD);
            if (maHD.indexOf('HD') > -1) {
                url = "/#/Invoices"; // hoadon
            }
            else if (maHD.indexOf('GDV') > -1) {
                url = "/#/ServicePackage"; // hoadon
            }
            else {
                if (maHD.indexOf('TH') > -1) {
                    url = "/#/Returns"; // trahang
                }
                else {
                    url = "/#/Order"; // dathang
                }
            }
        }
        window.open(url);
    }
    self.LoadChungTu_byMaHD = function (item) {
        var maHD = item.MaChungTu;
        if (maHD.indexOf('TT') > -1) {
            localStorage.setItem('FindMaPhieuChi', maHD);
            url = "/#/CashFlow"; // soquy
        }
        else {
            localStorage.setItem('FindHD', maHD);
            if (maHD.indexOf('HD') > -1) {
                url = "/#/Invoices"; // hoadon
            }
            else if (maHD.indexOf('GDV') > -1) {
                url = "/#/Invoices"; // hoadon
            }
            else {
                if (maHD.indexOf('TH') > -1) {
                    url = "/#/Returns"; // trahang
                }
                else {
                    url = "/#/Order"; // dathang
                }
            }
        }
        window.open(url);
    }
    self.LoadChungTuGoc_byMaHD = function (item) {
        var maHD = item.MaChungTuGoc;
        if (maHD.indexOf('TT') > -1) {
            localStorage.setItem('FindMaPhieuChi', maHD);
            url = "/#/CashFlow"; // soquy
        }
        else {
            localStorage.setItem('FindHD', maHD);
            if (maHD.indexOf('HD') > -1) {
                url = "/#/Invoices"; // hoadon
            }
            else if (maHD.indexOf('GDV') > -1) {
                url = "/#/Invoices"; // hoadon
            }
            else {
                if (maHD.indexOf('TH') > -1) {
                    url = "/#/Returns"; // trahang
                }
                else {
                    url = "/#/Order"; // dathang
                }
            }
        }
        window.open(url);
    }

    //Download file excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function (item) {
        var url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    }
    // xuất file Excel
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
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo bán hàng",
            NoiDung: "Xuất " + self.MoiQuanTam().toLowerCase(),
            NoiDungChiTiet: "Xuất " + self.MoiQuanTam().toLowerCase(),
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
                var array_Seach = {
                    MaNhanVien: Text_search,
                    ID_ChiNhanh: _id_DonVi,
                    timeCreate_Start: _timeStart,
                    timeCreate_End: _timeEnd,
                    ID_PhongBan: _ID_PhongBan,
                    GioiTinh: self.IsGioiTinh(),
                    LoaiHopDong: _LoaiHopDong,
                    timeBirthday_Start: _timeBirth_Start,
                    timeBirthday_End: _timeBirth_End,
                    LoaiChinhTri: _LoaiChinhTri,
                    LoaiBaoHiem: _LoaiBaoHiem,
                    LoaiDanToc: _LoaiDanToc,
                    TrangThai: _TrangThai,
                    columnsHide: columnHide,
                    TodayBC: self.TodayBC(),
                    TenChiNhanh: self.TenChiNhanh(),
                    Min: $("#txtDoTuoiFrom").val() != "" ? $("#txtDoTuoiFrom").val() : 0,
                    Max: $("#txtDoTuoiTo").val() != "" ? $("#txtDoTuoiTo").val() : 9999999
                }

                if (self.BCNV_XuatFile() != "BCNV_XuatFile") {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này", "danger");
                    LoadingForm(false);
                    return false;
                }
                if (self.check_MoiQuanTam() == 1 && self.BaoCaoNhanVien_TongHop().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TongHop",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 2 && self.BaoCaoNhanVien_TheoHopDong().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoHopDong",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 3 && self.BaoCaoNhanVien_TheoBaoHiem().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoBaoHiem",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 4 && self.BaoCaoNhanVien_TheoDoTuoi().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoDoTuoi",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 5 && self.BaoCaoNhanVien_KhenThuong().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoKhenThuong",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 6 && self.BaoCaoNhanVien_LuongPhuCap().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoLuongPhuCap",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 7 && self.BaoCaoNhanVien_MienThue().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoMienGiamThue",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 8 && self.BaoCaoNhanVien_DaoTao().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoDaoTao",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 9 && self.BaoCaoNhanVien_CongTac().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoQuaTrinhCongTac",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 10 && self.BaoCaoNhanVien_GiaDinh().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoGiaDinh",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 11 && self.BaoCaoNhanVien_SucKhoe().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNV_TheoSucKhoe",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
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
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
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
            if (self.check_MoiQuanTam() == 1)
                self.pageNumber_TH(_pageNumber);
            else if (self.check_MoiQuanTam() == 2)
                self.pageNumber_HD(_pageNumber);
            else if (self.check_MoiQuanTam() == 3)
                self.pageNumber_BH(_pageNumber);
            else if (self.check_MoiQuanTam() == 4)
                self.pageNumber_DT(_pageNumber);
            else if (self.check_MoiQuanTam() == 5)
                self.pageNumber_KT(_pageNumber);
            else if (self.check_MoiQuanTam() == 6)
                self.pageNumber_LPC(_pageNumber);
            else if (self.check_MoiQuanTam() == 7)
                self.pageNumber_MGT(_pageNumber);
            else if (self.check_MoiQuanTam() == 8)
                self.pageNumber_DAOTAO(_pageNumber);
            else if (self.check_MoiQuanTam() == 9)
                self.pageNumber_QTCT(_pageNumber);
            else if (self.check_MoiQuanTam() == 10)
                self.pageNumber_GD(_pageNumber);
            else if (self.check_MoiQuanTam() == 11)
                self.pageNumber_SK(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            if (self.check_MoiQuanTam() == 1)
                self.pageNumber_TH(_pageNumber);
            else if (self.check_MoiQuanTam() == 2)
                self.pageNumber_HD(_pageNumber);
            else if (self.check_MoiQuanTam() == 3)
                self.pageNumber_BH(_pageNumber);
            else if (self.check_MoiQuanTam() == 4)
                self.pageNumber_DT(_pageNumber);
            else if (self.check_MoiQuanTam() == 5)
                self.pageNumber_KT(_pageNumber);
            else if (self.check_MoiQuanTam() == 6)
                self.pageNumber_LPC(_pageNumber);
            else if (self.check_MoiQuanTam() == 7)
                self.pageNumber_MGT(_pageNumber);
            else if (self.check_MoiQuanTam() == 8)
                self.pageNumber_DAOTAO(_pageNumber);
            else if (self.check_MoiQuanTam() == 9)
                self.pageNumber_QTCT(_pageNumber);
            else if (self.check_MoiQuanTam() == 10)
                self.pageNumber_GD(_pageNumber);
            else if (self.check_MoiQuanTam() == 11)
                self.pageNumber_SK(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_HD(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_BH(_pageNumber);
        else if (self.check_MoiQuanTam() == 4)
            self.pageNumber_DT(_pageNumber);
        else if (self.check_MoiQuanTam() == 5)
            self.pageNumber_KT(_pageNumber);
        else if (self.check_MoiQuanTam() == 6)
            self.pageNumber_LPC(_pageNumber);
        else if (self.check_MoiQuanTam() == 7)
            self.pageNumber_MGT(_pageNumber);
        else if (self.check_MoiQuanTam() == 8)
            self.pageNumber_DAOTAO(_pageNumber);
        else if (self.check_MoiQuanTam() == 9)
            self.pageNumber_QTCT(_pageNumber);
        else if (self.check_MoiQuanTam() == 10)
            self.pageNumber_GD(_pageNumber);
        else if (self.check_MoiQuanTam() == 11)
            self.pageNumber_SK(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_HD(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_BH(_pageNumber);
        else if (self.check_MoiQuanTam() == 4)
            self.pageNumber_DT(_pageNumber);
        else if (self.check_MoiQuanTam() == 5)
            self.pageNumber_KT(_pageNumber);
        else if (self.check_MoiQuanTam() == 6)
            self.pageNumber_LPC(_pageNumber);
        else if (self.check_MoiQuanTam() == 7)
            self.pageNumber_MGT(_pageNumber);
        else if (self.check_MoiQuanTam() == 8)
            self.pageNumber_DAOTAO(_pageNumber);
        else if (self.check_MoiQuanTam() == 9)
            self.pageNumber_QTCT(_pageNumber);
        else if (self.check_MoiQuanTam() == 10)
            self.pageNumber_GD(_pageNumber);
        else if (self.check_MoiQuanTam() == 11)
            self.pageNumber_SK(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_HD(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_BH(_pageNumber);
        else if (self.check_MoiQuanTam() == 4)
            self.pageNumber_DT(_pageNumber);
        else if (self.check_MoiQuanTam() == 5)
            self.pageNumber_KT(_pageNumber);
        else if (self.check_MoiQuanTam() == 6)
            self.pageNumber_LPC(_pageNumber);
        else if (self.check_MoiQuanTam() == 7)
            self.pageNumber_MGT(_pageNumber);
        else if (self.check_MoiQuanTam() == 8)
            self.pageNumber_DAOTAO(_pageNumber);
        else if (self.check_MoiQuanTam() == 9)
            self.pageNumber_QTCT(_pageNumber);
        else if (self.check_MoiQuanTam() == 10)
            self.pageNumber_GD(_pageNumber);
        else if (self.check_MoiQuanTam() == 11)
            self.pageNumber_SK(_pageNumber);
        self.ReserPage();
    }

    self.ResetCurrentPage = function () {
        _pageNumber = 1;
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                self.pageNumber_TH(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_TongHop().length / self.pageSize());
                break;
            case 2:
                self.pageNumber_HD(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_TheoHopDong().length / self.pageSize());
                break;
            case 3:
                self.pageNumber_BH(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_TheoBaoHiem().length / self.pageSize());
                break;
            case 4:
                self.pageNumber_DT(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_TheoDoTuoi().length / self.pageSize());
                break;
            case 5:
                self.pageNumber_KT(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_KhenThuong().length / self.pageSize());
                break;
            case 6:
                self.pageNumber_LPC(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_LuongPhuCap().length / self.pageSize());
                break;
            case 7:
                self.pageNumber_MGT(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_MienThue().length / self.pageSize());
                break;
            case 8:
                self.pageNumber_DAOTAO(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_DaoTao().length / self.pageSize());
                break;
            case 9:
                self.pageNumber_QTCT(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_CongTac().length / self.pageSize());
                break;
            case 10:
                self.pageNumber_GD(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_GiaDinh().length / self.pageSize());
                break;
            case 11:
                self.pageNumber_SK(1);
                AllPage = Math.ceil(self.BaoCaoNhanVien_SucKhoe().length / self.pageSize());
                break;
        }
        self.ReserPage();
    };

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
    return self;
}
var data = new ViewModal();
ko.applyBindings(data);