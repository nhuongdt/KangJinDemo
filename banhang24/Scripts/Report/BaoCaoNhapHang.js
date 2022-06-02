var ViewModal = function () {
    var self = this;
    var _rdTime = 1;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/'
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.MoiQuanTam = ko.observable('Báo cáo nhập hàng tổng hợp');
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.LoaiBaoCao = ko.observable('hàng hóa')
    self.TenNVPrint = ko.observable();
    var dt1 = new Date();
    var currentWeekDay1 = dt1.getDay();
    var lessDays1 = currentWeekDay1 == 0 ? 6 : currentWeekDay1 - 1
    var _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - lessDays1))).format('YYYY-MM-DD'); // start of wwek
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 7))).format('YYYY-MM-DD'); // end of week
    var dtBC = new Date(_timeEnd);
    var _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
    self.TodayBC = ko.observable('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
    self.check_MoiQuanTam = ko.observable(1);
    self.SumNumberPageReport = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.SumNumberPageReport_CT = ko.observableArray();
    self.RowsStart_CT = ko.observable('1');
    self.RowsEnd_CT = ko.observable('10');
    self.NhomHangHoas = ko.observableArray();
    var _pageNumber = 1;
    var _pageSize = 10;
    var _pageNumber_CT = 1;
    var _pageSize_CT = 10;
    self.SumRowsHangHoa = ko.observable();
    self.SumRowsHangHoa_CT = ko.observable();
    self.pageSize = ko.observable(10);
    self.pageSizes = [10, 20, 30, 40, 50];
    var AllPage;
    var AllPage_CT;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    self.pageNumber_TH = ko.observable(1);
    self.pageNumber_CT = ko.observable(1);
    self.pageNumber_NH = ko.observable(1);
    self.pageNumber_NCC = ko.observable(1);
    self.pageNumber_THN = ko.observable(1);
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray([_id_DonVi]);

    $('.ip_TimeReport').val("Tuần này");
    self.Loc_TinhTrangKD = ko.observable('2');
    var tk = null;
    // TuanDL Cache Show Hide Column Grid
    self.listCheckbox = ko.observableArray();
    self.columnCheckType = ko.observable(1);
    var Key_Form = 'Key_ReportImport';
    function loadCheckbox(type) {
        self.columnCheckType(type);
        $.getJSON("api/DanhMuc/ReportAPI/GetChecked?type=" + type + "&group=" + $('#ID_loaibaocao').val(), function (data) {
            self.listCheckbox(data);
            loadHtmlGrid();
        });
        $('.chose_kieubang li').each(function (i) {
            if (type === $(this).data('id')) {
                if (!$(this).hasClass("active"))
                    $(this).addClass("active");
            }
            else {
                $(this).removeClass("active");
            }
        });
        $('.tab-content .tab-pane').each(function (i) {
            if (type === $(this).data('id')) {
                if (!$(this).hasClass("active"))
                    $(this).addClass("active");
            }
            else {
                $(this).removeClass("active");
            }
        });
    }
    loadCheckbox(1);

    var IsLoadFirst = true;
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
                    localStorage.removeItem((Key_Form + "2_LOHANG"));
                    localStorage.removeItem((Key_Form + "10_LOHANG"));
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
    //trinhpv phân quyền
    self.BaoCaoBanHang = ko.observable();
    self.BCNH_TongHop = ko.observable();
    self.BCNH_ChiTiet = ko.observable();
    self.BCNH_TheoNhomHang = ko.observable();
    self.BCNH_TheoNhaCungCap = ko.observable();
    self.BCNH_TraHangNhap = ko.observable();
    self.BCNH_XuatFile = ko.observable();
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    function getQuyen_NguoiDung() {
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNH_TongHop", "GET").done(function (data) {
        //    self.BCNH_TongHop(data);
        //    getDonVi();
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNH_ChiTiet", "GET").done(function (data) {
        //    self.BCNH_ChiTiet(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNH_TheoNhomHang", "GET").done(function (data) {
        //    self.BCNH_TheoNhomHang(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNH_TheoNhaCungCap", "GET").done(function (data) {
        //    self.BCNH_TheoNhaCungCap(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNH_TraHangNhap", "GET").done(function (data) {
        //    self.BCNH_TraHangNhap(data);
        //})
        //ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNH_XuatFile", "GET").done(function (data) {
        //    self.BCNH_XuatFile(data);
        //})

        if (VHeader.Quyen.indexOf('BCNH_TongHop') > -1) {
            self.BCNH_TongHop('BCNH_TongHop');
            getDonVi();
        }
        else {
            self.BCNH_TongHop('false');
        }

        if (VHeader.Quyen.indexOf('BCNH_ChiTiet') > -1) {
            self.BCNH_ChiTiet('BCNH_ChiTiet');
        }
        else {
            self.BCNH_ChiTiet('false');
        }

        if (VHeader.Quyen.indexOf('BCNH_TheoNhomHang') > -1) {
            self.BCNH_TheoNhomHang('BCNH_TheoNhomHang');
        }
        else {
            self.BCNH_TheoNhomHang('false');
        }

        if (VHeader.Quyen.indexOf('BCNH_TheoNhaCungCap') > -1) {
            self.BCNH_TheoNhaCungCap('BCNH_TheoNhaCungCap');
        }
        else {
            self.BCNH_TheoNhaCungCap('false');
        }

        if (VHeader.Quyen.indexOf('BCNH_TraHangNhap') > -1) {
            self.BCNH_TraHangNhap('BCNH_TraHangNhap');
        }
        else {
            self.BCNH_TraHangNhap('false');
        }

        if (VHeader.Quyen.indexOf('BCNH_XuatFile') > -1) {
            self.BCNH_XuatFile('BCNH_XuatFile');
        }
        else {
            self.BCNH_XuatFile('false');
        }
    }
    getQuyen_NguoiDung();
    //trinhpv Lọc nhóm nhà cung cấp
    self.NhomDoiTuongs = ko.observableArray();
    var _tennhomDT = null;
    var _tenNhomDoiTuongSeach = null;
    function getList_NhomDoiTuongs() {
        ajaxHelper(ReportUri + "getList_NhomDoiTuong?LoaiDoiTuong=2", "GET").done(function (data) {
            self.NhomDoiTuongs(data.LstData);
            self.searchNhomDoiTuong(data.LstData);
        });
    };
    getList_NhomDoiTuongs();
    self.CloseNhomDoiTuong = function (item) {
        _tenNhomDoiTuongSeach = null;
        self.MangNhomDoiTuong.remove(item);
        for (var i = 0; i < self.MangNhomDoiTuong().length; i++) {
            if (i == 0)
                _tenNhomDoiTuongSeach = self.MangNhomDoiTuong()[i].ID;
            else
                _tenNhomDoiTuongSeach = self.MangNhomDoiTuong()[i].ID + "," + _tenNhomDoiTuongSeach;
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
                if (i == 0)
                    _tenNhomDoiTuongSeach = self.MangNhomDoiTuong()[i].ID;
                else
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
    var _maNCC = null;
    self.SelectMaHH = function () {
        _maNCC = $('#txtMaHH').val();
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {
            _pageNumber = 1;
            self.LoadReport();
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

    self.SelectedDonVi = function (item, event) {
        if (event !== undefined) {
            event.stopPropagation();
        }
        _idDonViSeach = null;
        var TenChiNhanh = '';
        var arrIDDonVi = [];
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
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        if (event !== undefined) {
            event.preventDefault();
        }
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
    $('.chose_kieubang').on('click', 'li', function () {
        $('.Show_NhomKhachHang').hide();
        $('.showNhomNCC').hide();
        $('.showNhaCungCap').hide();
        //$("#txt_search").val('');
        //Text_search = '';
        loadCheckbox($(this).data('id'));
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        self.check_MoiQuanTam($(this).find('a input').val());
        if (self.check_MoiQuanTam() == 1) {
            $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
            self.LoaiBaoCao('hàng hóa');
            self.MoiQuanTam('Báo cáo nhập hàng tổng hợp');
        }
        else if (self.check_MoiQuanTam() == 2) {
            if (self.NhomDoiTuongs().length < 2)
                $('.showNhomNCC').hide();
            else
                $('.showNhomNCC').show();
            $('.showNhaCungCap').show();
            $("#txt_search").attr("placeholder", "Theo mã, tên hàng, mã chứng từ, ghi chú").blur();
            self.LoaiBaoCao('hàng hóa chi tiết');
            self.MoiQuanTam('Báo cáo nhập hàng chi tiết');
        }
        else if (self.check_MoiQuanTam() == 3) {
            $("#txt_search").attr("placeholder", "Theo tên nhóm hàng").blur();
            self.LoaiBaoCao('nhóm hàng hóa');
            self.MoiQuanTam('Báo cáo nhập hàng theo nhóm hàng hóa');
        }
        else if (self.check_MoiQuanTam() == 4) {
            if (self.NhomDoiTuongs().length < 2)
                $('.showNhomNCC').hide();
            else
                $('.showNhomNCC').show();
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại nhà cung cấp").blur();
            self.LoaiBaoCao('nhóm hàng hóa');
            self.MoiQuanTam('Báo cáo nhập hàng theo nhà cung cấp');
        }
        else if (self.check_MoiQuanTam() == 5) {
            $("#txt_search").attr("placeholder", "Theo mã, tên hàng, tên nhóm hàng").blur();
            self.LoaiBaoCao('nhóm hàng hóa');
            self.MoiQuanTam('Báo cáo hàng trả nhà cung cấp');
        }
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
            self.TodayBC('Ngày nhập: Toàn thời gian');
        }
        //Hôm nay
        else if (_rdoNgayPage === 1) {
            _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
        }
        //Hôm qua
        else if (_rdoNgayPage === 2) {
            var dt1 = new Date();
            var dt2 = new Date();
            _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
            _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
            self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
            self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
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
                self.TodayBC('Ngày nhập: Toàn thời gian');
            }
            //Hôm nay
            else if (_rdoNgayPage === "Hôm nay") {
                _timeStart = datime.getFullYear() + "-" + (datime.getMonth() + 1) + "-" + datime.getDate();
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 1))).format('YYYY-MM-DD');
                self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
            }
            //Hôm qua
            else if (_rdoNgayPage === "Hôm qua") {
                var dt1 = new Date();
                var dt2 = new Date();
                _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - 1))).format('YYYY-MM-DD');
                _timeEnd = dt2.getFullYear() + "-" + (dt2.getMonth() + 1) + "-" + dt2.getDate();
                self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
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
                    self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    })
    self.Select_Text = function () {
        Text_search = $('#txt_search').val();
    }
    $('#txt_search').keypress(function (e) {
        if (e.keyCode == 13) {
            self.LoadReport();
        }
    })
    var Text_search = "";
    var _laHangHoa = 2;
    var TinhTrangHH = 2;
    var _ID_NhomHang = null;
    self.BaoCaoNhapHang_TongHop = ko.observableArray();
    self.BaoCaoNhapHang_ChiTiet = ko.observableArray();
    self.BaoCaoNhapHang_TheoNhomHang = ko.observableArray();
    self.BaoCaoNhapHang_TheoNhaCungCap = ko.observableArray();
    self.BaoCaoNhapHang_TraHangNhap = ko.observableArray();
    self.TH_SoLuong = ko.observable();
    self.TH_ThanhTien = ko.observable();
    self.TH_GiaTriNhap = ko.observable();
    self.TH_TienThue = ko.observable();
    self.TH_GiamGiaHD = ko.observable();
    self.CT_SoLuong = ko.observable();
    self.CT_ThanhTien = ko.observable();
    self.CT_GiaTriNhap = ko.observable();
    self.CT_GiamGiaHD = ko.observable();
    self.TNH_SoLuongNhap = ko.observable();
    self.TNH_ThanhTien = ko.observable();
    self.TNH_GiaTriNhap = ko.observable();
    self.TNH_GiamGiaHD = ko.observable();
    self.NCC_SoLuongNhap = ko.observable();
    self.NCC_ThanhTien = ko.observable();
    self.NCC_GiaTriNhap = ko.observable();
    self.NCC_GiamGiaHD = ko.observable();
    self.THN_SoLuong = ko.observable();
    self.THN_ThanhTien = ko.observable();
    self.THN_GiamGiaHD = ko.observable();
    self.THN_GiaTriTra = ko.observable();
    self.LoadReport = function () {
        LoadingForm(true);
        $('.table-reponsive').css('display', 'none');
        _pageNumber = 1;
        _pageNumber_CT = 1;
        self.pageNumber_TH(1);
        self.pageNumber_CT(1);
        self.pageNumber_NH(1);
        self.pageNumber_CT(1);
        self.pageNumber_NCC(1);
        self.pageNumber_THN(1);
        var array_Seach = {
            MaHangHoa: Text_search,
            MaKhachHang: _maNCC,
            timeStart: _timeStart,
            timeEnd: _timeEnd,
            ID_ChiNhanh: _idDonViSeach,
            LaHangHoa: _laHangHoa,
            TinhTrang: TinhTrangHH,
            ID_NhomHang: _ID_NhomHang,
            ID_NhomDoiTuong: _tenNhomDoiTuongSeach,
            ID_NguoiDung: _IDDoiTuong,
            columnsHide: null,
            TodayBC: null,
            TenChiNhanh: null,
            lstIDChiNhanh: self.LstIDDonVi(),
            lstNhomKhach: self.MangNhomDoiTuong().map(function (x) { return x.ID }),
        }
        if (self.check_MoiQuanTam() == 1) {
            array_Seach.LoaiHangHoa = 1;
            if (self.BCNH_TongHop() == "BCNH_TongHop") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhapHang_TongHop", "POST", array_Seach).done(function (data) {
                    console.log('bcnh ', data)
                    if (data.res) {
                        self.BaoCaoNhapHang_TongHop(data.LstData);
                        AllPage = data.numberPage;
                        self.ResetCurrentPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.TH_SoLuong(data.a1);
                        self.TH_ThanhTien(data.a2);
                        self.TH_GiamGiaHD(data.a3);
                        self.TH_GiaTriNhap(data.a4);
                        self.TH_TienThue(data.sumTienThue);
                        LoadingForm(false);
                    }
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_TongHop").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() == 2) {
            if (self.BCNH_ChiTiet() == "BCNH_ChiTiet") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhapHang_ChiTiet", "POST", array_Seach).done(function (data) {
                    console.log('nhCT ', data)
                    if (data.res) {
                        self.BaoCaoNhapHang_ChiTiet(data.LstData);
                        AllPage = data.numberPage;
                        self.ResetCurrentPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.CT_SoLuong(data.a1);
                        self.CT_ThanhTien(data.a2);
                        self.CT_GiamGiaHD(data.a3);
                        self.CT_GiaTriNhap(data.a4);
                        self.TH_TienThue(data.sumTienThue);
                    }
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
        }
        else if (self.check_MoiQuanTam() == 3) {
            if (self.BCNH_TheoNhomHang() == "BCNH_TheoNhomHang") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhapHang_NhomHang", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhapHang_TheoNhomHang(data.LstData);
                    AllPage = data.numberPage;
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.TNH_SoLuongNhap(data.a1);
                    self.TNH_ThanhTien(data.a2);
                    self.TNH_GiamGiaHD(data.a3);
                    self.TNH_GiaTriNhap(data.a4);
                    self.TH_TienThue(data.sumTienThue);
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
        }
        else if (self.check_MoiQuanTam() == 4) {
            if (self.BCNH_TheoNhaCungCap() == "BCNH_TheoNhaCungCap") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhapHang_TheoNhaCungCap", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhapHang_TheoNhaCungCap(data.LstData);
                    AllPage = data.numberPage;
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.NCC_SoLuongNhap(data.a1);
                    self.NCC_ThanhTien(data.a2);
                    self.NCC_GiamGiaHD(data.a3);
                    self.NCC_GiaTriNhap(data.a4);
                    self.TH_TienThue(data.sumTienThue);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_TheoNhaCungCap").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() == 5) {
            if (self.BCNH_TraHangNhap() == "BCNH_TraHangNhap") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoNhapHang_TraHangNhap", "POST", array_Seach).done(function (data) {
                    self.BaoCaoNhapHang_TraHangNhap(data.LstData);
                    AllPage = data.numberPage;
                    self.ResetCurrentPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.THN_SoLuong(data.a1);
                    self.THN_ThanhTien(data.a2);
                    self.THN_GiamGiaHD(data.a3);
                    self.THN_GiaTriTra(data.a4);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_TraHangNhap").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
    }
    self.BaoCaoNhapHang_TongHop_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 1) {
            var first = (self.pageNumber_TH() - 1) * self.pageSize();
            if (self.BaoCaoNhapHang_TongHop() !== null) {
                if (self.BaoCaoNhapHang_TongHop().length != 0) {
                    $('.TC_TongHop').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_TH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_TH() - 1) * self.pageSize() + self.BaoCaoNhapHang_TongHop().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_TongHop').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhapHang_TongHop().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhapHang_ChiTiet_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 2) {
            var first = (self.pageNumber_CT() - 1) * self.pageSize();
            if (self.BaoCaoNhapHang_ChiTiet() !== null) {
                if (self.BaoCaoNhapHang_ChiTiet().length != 0) {
                    $('.TC_ChiTiet').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_CT() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_CT() - 1) * self.pageSize() + self.BaoCaoNhapHang_ChiTiet().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_ChiTiet').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhapHang_ChiTiet().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhapHang_TheoNhomHang_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 3) {
            var first = (self.pageNumber_NH() - 1) * self.pageSize();
            if (self.BaoCaoNhapHang_TheoNhomHang() !== null) {
                if (self.BaoCaoNhapHang_TheoNhomHang().length != 0) {
                    $('.TC_TheoNhomHang').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_NH() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_NH() - 1) * self.pageSize() + self.BaoCaoNhapHang_TheoNhomHang().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_TheoNhomHang').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhapHang_TheoNhomHang().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhapHang_TheoNhaCungCap_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 4) {
            var first = (self.pageNumber_NCC() - 1) * self.pageSize();
            if (self.BaoCaoNhapHang_TheoNhaCungCap() !== null) {
                if (self.BaoCaoNhapHang_TheoNhaCungCap().length != 0) {
                    $('.TC_TheoNhaCungCap').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_NCC() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_NCC() - 1) * self.pageSize() + self.BaoCaoNhapHang_TheoNhaCungCap().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_TheoNhaCungCap').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhapHang_TheoNhaCungCap().slice(first, first + self.pageSize());
            }
        }
        return null;
    })
    self.BaoCaoNhapHang_TraHangNhap_Page = ko.computed(function (x) {
        if (parseInt(self.check_MoiQuanTam()) === 5) {
            var first = (self.pageNumber_THN() - 1) * self.pageSize();
            if (self.BaoCaoNhapHang_TraHangNhap() !== null) {
                if (self.BaoCaoNhapHang_TraHangNhap().length != 0) {
                    $('.TC_TraHangNhap').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((self.pageNumber_THN() - 1) * self.pageSize() + 1);
                    self.RowsEnd((self.pageNumber_THN() - 1) * self.pageSize() + self.BaoCaoNhapHang_TraHangNhap().slice(first, first + self.pageSize()).length)
                }
                else {
                    $('.TC_TraHangNhap').hide();
                    $(".Report_Empty").show();
                    $(".page").hide();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                return self.BaoCaoNhapHang_TraHangNhap().slice(first, first + self.pageSize());
            }
        }
        return null;
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
    self.LoadNhaCungCap_byMaNCC = function (item) {
        if (item.MaNhaCungCap != '' && item.MaNhaCungCap != null && item.MaNhaCungCap != 'NCC Lẻ') {
            localStorage.setItem('FindKhachHang', item.MaNhaCungCap);
            var url = "/#/Suppliers";
            window.open(url);
        }
    }
    self.LoadHoaDon_byMaHD = function (item) {
        if (item.MaHoaDon != '' && item.MaHoaDon != null && item.MaHoaDon != 'HD trả nhanh') {
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
                else if (maHD.indexOf('PNK') > -1) {
                    var url = "/#/PurchaseOrder";
                }
                else if (maHD.indexOf('THN') > -1) {
                    var url = "/#/PurchaseReturns";
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
            else if (maHD.indexOf('PNK') > -1) {
                var url = "/#/PurchaseOrder";
            }
            else if (maHD.indexOf('THN') > -1) {
                var url = "/#/PurchaseReturns";
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
        if (item.MaChungTuGoc != '' && item.MaChungTuGoc != null && item.MaChungTuGoc != 'HD trả nhanh') {
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
                else if (maHD.indexOf('PNK') > -1) {
                    var url = "/#/PurchaseOrder";
                }
                else if (maHD.indexOf('THN') > -1) {
                    var url = "/#/PurchaseReturns";
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
                self.pageNumber_CT(_pageNumber);
            else if (self.check_MoiQuanTam() == 3)
                self.pageNumber_NH(_pageNumber);
            else if (self.check_MoiQuanTam() == 4)
                self.pageNumber_NCC(_pageNumber);
            else if (self.check_MoiQuanTam() == 5)
                self.pageNumber_THN(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            if (self.check_MoiQuanTam() == 1)
                self.pageNumber_TH(_pageNumber);
            else if (self.check_MoiQuanTam() == 2)
                self.pageNumber_CT(_pageNumber);
            else if (self.check_MoiQuanTam() == 3)
                self.pageNumber_NH(_pageNumber);
            else if (self.check_MoiQuanTam() == 4)
                self.pageNumber_NCC(_pageNumber);
            else if (self.check_MoiQuanTam() == 5)
                self.pageNumber_THN(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_CT(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_NH(_pageNumber);
        else if (self.check_MoiQuanTam() == 4)
            self.pageNumber_NCC(_pageNumber);
        else if (self.check_MoiQuanTam() == 5)
            self.pageNumber_THN(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_CT(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_NH(_pageNumber);
        else if (self.check_MoiQuanTam() == 4)
            self.pageNumber_NCC(_pageNumber);
        else if (self.check_MoiQuanTam() == 5)
            self.pageNumber_THN(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        if (self.check_MoiQuanTam() == 1)
            self.pageNumber_TH(_pageNumber);
        else if (self.check_MoiQuanTam() == 2)
            self.pageNumber_CT(_pageNumber);
        else if (self.check_MoiQuanTam() == 3)
            self.pageNumber_NH(_pageNumber);
        else if (self.check_MoiQuanTam() == 4)
            self.pageNumber_NCC(_pageNumber);
        else if (self.check_MoiQuanTam() == 5)
            self.pageNumber_THN(_pageNumber);
        self.ReserPage();
    }
    self.ResetCurrentPage = function () {
        _pageNumber = 1;
        switch (parseInt(self.check_MoiQuanTam())) {
            case 1:
                self.pageNumber_TH(1);
                AllPage = Math.ceil(self.BaoCaoNhapHang_TongHop().length / self.pageSize());
                break;
            case 2:
                self.pageNumber_CT(1);
                AllPage = Math.ceil(self.BaoCaoNhapHang_ChiTiet().length / self.pageSize());
                break;
            case 3:
                self.pageNumber_NH(1);
                AllPage = Math.ceil(self.BaoCaoNhapHang_TheoNhomHang().length / self.pageSize());
                break;
            case 4:
                self.pageNumber_NCC(1);
                AllPage = Math.ceil(self.BaoCaoNhapHang_TheoNhaCungCap().length / self.pageSize());
                break;
            case 5:
                self.pageNumber_THN(1);
                AllPage = Math.ceil(self.BaoCaoNhapHang_TraHangNhap().length / self.pageSize());
                break;
        }
        self.ReserPage();
    };

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
                    MaHangHoa: Text_search,
                    MaKhachHang: _maNCC,
                    timeStart: _timeStart,
                    timeEnd: _timeEnd,
                    ID_ChiNhanh: _idDonViSeach,
                    LaHangHoa: _laHangHoa,
                    TinhTrang: TinhTrangHH,
                    ID_NhomHang: _ID_NhomHang,
                    ID_NhomDoiTuong: _tenNhomDoiTuongSeach,
                    ID_NguoiDung: _IDDoiTuong,
                    columnsHide: columnHide,
                    TodayBC: self.TodayBC(),
                    TenChiNhanh: self.TenChiNhanh(),
                    lstIDChiNhanh: self.LstIDDonVi(),
                }
                if (self.BCNH_XuatFile() != "BCNH_XuatFile") {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    LoadingForm(false);
                    return false;
                }
                if (self.check_MoiQuanTam() == 1 && self.BaoCaoNhapHang_TongHop().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNH_TongHop",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 2 && self.BaoCaoNhapHang_ChiTiet().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNH_ChiTiet",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 3 && self.BaoCaoNhapHang_TheoNhomHang().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNH_TheoNhomHang",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 4 && self.BaoCaoNhapHang_TheoNhaCungCap().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNH_TheoNhaCungCap",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url)
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() == 5 && self.BaoCaoNhapHang_TraHangNhap().length != 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCNH_TraHangNhap",
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
}
var reportNhapHang = new ViewModal();
ko.applyBindings(reportNhapHang);
$('#selec-all-DonVi').parent().on('hide.bs.dropdown', function () {
    reportNhapHang.LoadReport();
});