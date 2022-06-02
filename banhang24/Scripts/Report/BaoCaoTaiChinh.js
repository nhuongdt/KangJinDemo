var ViewModal = function () {
    let _year;
    var self = this;
    //var _rdTime = 1;
    var ReportUri = '/api/DanhMuc/ReportAPI/';
    var BH_HoaDonUri = '/api/DanhMuc/BH_HoaDonAPI/';
    var DiaryUri = '/api/DanhMuc/SaveDiary/';
    var _IDDoiTuong = $('.idnguoidung').text();
    var _id_DonVi = $('#hd_IDdDonVi').val();
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _id_NhanVien = $('.idnhanvien').text();
    self.MoiQuanTam = ko.observable('Báo cáo tổng hợp công nợ');
    var _kieubang = 1;
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.LoaiBaoCao = ko.observable('đối tác');
    self.MangLoaiThuChi = ko.observableArray();
    self.searchLoaiThuChi = ko.observableArray();
    self.ArrDonVi = ko.observableArray();
    self.LstIDDonVi = ko.observableArray([_id_DonVi]);
    self.TenNVPrint = ko.observable();
    self.newYear = ko.observable();
    self.check_kieubang = ko.observable('2');
    self.Loc_TonKho = ko.observable('1');
    self.Loc_LoaiTien = ko.observable('12');
    self.listReportTaiChinh_TheoThang = ko.observableArray();
    self.listReportTaiChinh_TheoQuy = ko.observableArray();
    self.listReportTaiChinh_TheoNam = ko.observableArray();
    self.listReportPhanTichThuChi_TheoThang = ko.observableArray();
    self.listReportPhanTichThuChi_TheoQuy = ko.observableArray();
    self.listReportPhanTichThuChi_TheoNam = ko.observableArray();
    var dt1 = new Date();
    var currentWeekDay1 = dt1.getDay();
    var lessDays1 = currentWeekDay1 === 0 ? 6 : currentWeekDay1 - 1;
    var _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - lessDays1))).format('YYYY-MM-DD'); // start of wwek
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 7))).format('YYYY-MM-DD'); // end of week
    let dtBC = new Date(_timeEnd);
    let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
    self.TodayBC = ko.observable('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
    var datimeCN = new Date();
    var _timeSQ_ChiNhanh = moment(new Date(datimeCN.setDate(datimeCN.getDate() + 1))).format('YYYY-MM-DD');
    self.TodayBC1 = ko.observable();
    self.TodayBC_ChiNhanh = ko.observable('Đến ngày: ' + moment(new Date()).format('DD/MM/YYYY'));
    self.check_MoiQuanTam = ko.observable(1);
    self.SumNumberPageReport = ko.observableArray();
    self.check_Time = ko.observable('1');
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.SumNumberPageReport_CT = ko.observableArray();
    self.NhomHangHoas = ko.observableArray();
    var _pageNumber = 1;
    var _pageSize = 10;
    self.SumRowsHangHoa = ko.observable();
    self.pageSize = ko.observable(10);
    var AllPage;
    self.MangNhomNhaCungCap = ko.observableArray();
    self.MangNhomKhachHang = ko.observableArray();
    self.searchNhomNhaCungCap = ko.observableArray();
    self.searchNhomKhachHang = ko.observableArray();

    self.pageNumber_CN = ko.observable(1);
    self.pageNumber_PT = ko.observable(1);
    self.pageNumber_PC = ko.observable(1);
    self.pageNumber_SQTM = ko.observable(1);
    self.pageNumber_SQNH = ko.observable(1);
    self.pageNumber_SQTQ = ko.observable(1);
    self.pageNumber_SQCN = ko.observable(1);
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    $('.ip_TimeReport').val("Tuần này");
    var tk = null;
    // TuanDL Cache Show Hide Column Grid
    self.listCheckbox = ko.observableArray();
    self.columnCheckType = ko.observable(1);
    var Key_Form = 'Key_ReportFinancials';
    function loadCheckbox(type, isTab = true) {
        if (parseInt($('#ID_soquy').val()) === type && isTab) {
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
            $('#soquy .nav-tabs li').each(function () {
                if ($(this).hasClass('active')) {
                    loadCheckbox($(this).data('id'), false);
                }
            });

        }
        else {
            self.columnCheckType(type);
            $.getJSON("api/DanhMuc/ReportAPI/GetChecked?type=" + type + "&group=" + $('#ID_loaibaocao').val(), function (data) {
                if (data === null) {
                    data = [];
                }
                self.listCheckbox(data);
                loadHtmlGrid();
            });
            if (isTab) {
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
        }
    }
    $('#soquy .nav-tabs').on('click', 'li', function () {
        loadCheckbox($(this).data('id'), false);
    });
    loadCheckbox(1);
    function loadHtmlGrid() {
        LocalCaches.LoadFirstColumnGrid(Key_Form + self.columnCheckType(), $('#select-column .dropdown-list ul li input[type = checkbox]'), self.listCheckbox());
        $('.table-reponsive').css('display', 'block');
    }
    $('#select-column').on('change', '.dropdown-list ul li input[type = checkbox]', function () {
        var valueCheck = parseInt($(this).val());
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
    self.hideTableReport = function () {
        $('.table_TheoThang').hide();
        $('.table_TheoQuy').hide();
        $('.table_TheoNam').hide();
        $('.table_PTTCTheoThang').hide();
        $('.table_PTTCTheoQuy').hide();
        $('.table_PTTCTheoNam').hide();
    };
    self.hideTableReport();
    //trinhpv lọc nhóm khách hàng
    var _tennhomDT = null;
    var _tenNhomKhachHangSeach = null;
    self.NhomKhachHangs = ko.observableArray();
    function getList_NhomKhachHangs() {
        ajaxHelper(ReportUri + "GetListID_NhomDoiTuong?TenNhomDoiTuong=" + _tennhomDT + "&loaiDoiTuong=1", "GET").done(function (data) {
            self.NhomKhachHangs(data);
            self.searchNhomKhachHang(data);
            //for (let i = 0; i < self.NhomKhachHangs().length; i++) {
            //    _tenNhomKhachHangSeach = self.NhomKhachHangs()[i].ID + "," + _tenNhomKhachHangSeach;
            //}
        });
    }
    getList_NhomKhachHangs();

    self.CloseNhomKhachHang = function (item) {
        _tenNhomKhachHangSeach = null;
        self.MangNhomKhachHang.remove(item);
        for (let i = 0; i < self.MangNhomKhachHang().length; i++) {
            _tenNhomKhachHangSeach = self.MangNhomKhachHang()[i].ID + "," + _tenNhomKhachHangSeach;
        }
        //if (self.MangNhomKhachHang().length === 0) {
        //    //for (let i = 0; i < self.searchNhomKhachHang().length; i++) {
        //    //    _tenNhomKhachHangSeach = self.searchNhomKhachHang()[i].ID + "," + _tenNhomKhachHangSeach;
        //    //}
        //}
        // remove check
        $('#selec-all-NhomKhachHang li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        _pageNumber = 1;
        self.LoadReport();
    };
    self.SelectedNhomKhachHang = function (item) {
        _tenNhomKhachHangSeach = null;
        var arrIDNhomKhachHang = [];
        for (let i = 0; i < self.MangNhomKhachHang().length; i++) {
            if ($.inArray(self.MangNhomKhachHang()[i], arrIDNhomKhachHang) === -1) {
                arrIDNhomKhachHang.push(self.MangNhomKhachHang()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNhomKhachHang) === -1) {
            self.MangNhomKhachHang.push(item);
            for (let i = 0; i < self.MangNhomKhachHang().length; i++) {
                _tenNhomKhachHangSeach = self.MangNhomKhachHang()[i].ID + "," + _tenNhomKhachHangSeach;
            }
        }
        // sau khi tìm kiếm thì trả về mặc định
        $('#NoteNameNhomKhachHang').val('');
        self.NhomKhachHangs(self.searchNhomKhachHang());
        //đánh dấu check
        for (let i = 0; i < self.MangNhomKhachHang().length; i++) {
            $('#selec-all-NhomKhachHang li').each(function () {
                if ($(this).attr('id') === self.MangNhomKhachHang()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>');
                }
            });
        }
        _pageNumber = 1;
        self.LoadReport();
    };
    self.NoteNameNhomKhachHang = function () {
        var arrNhomKhachHang = [];
        var itemSearch = locdau($('#NoteNameNhomKhachHang').val().toLowerCase());
        for (let i = 0; i < self.searchNhomKhachHang().length; i++) {
            var locdau_kd = self.searchNhomKhachHang()[i].TenNhomKhachHang_KhongDau;
            var locdau_ktd = self.searchNhomKhachHang()[i].TenNhomKhachHang_KyTuDau;
            var R1 = locdau_kd.split(itemSearch);
            var R2 = locdau_ktd.split(itemSearch);
            if (R1.length > 1 || R2.length > 1) {
                arrNhomKhachHang.push(self.searchNhomKhachHang()[i]);
            }
        }
        self.NhomKhachHangs(arrNhomKhachHang);
        if ($('#NoteNameNhomKhachHang').val() === "") {
            self.NhomKhachHangs(self.searchNhomKhachHang());
            for (let i = 0; i < self.MangNhomKhachHang().length; i++) {
                $('#selec-all-NhomKhachHang li').each(function () {
                    if ($(this).attr('id') === self.MangNhomKhachHang()[i].ID) {
                        $(this).find('i').remove();
                        $(this).append('<i class="fa fa-check check-after-li"></i>');
                    }
                });
            }
        }
    };
    $('#NoteNameNhomKhachHang').keypress(function (e) {
        if (e.keyCode === 13 && self.NhomKhachHangs().length > 0) {
            self.SelectedNhomKhachHang(self.NhomKhachHangs()[0]);
        }
    });
    //trinhpv Lọc nhóm nhà cung cấp
    self.NhomNhaCungCaps = ko.observableArray();
    var _tenNhomNhaCungCapSeach = null;
    function getList_NhomNhaCungCaps() {
        ajaxHelper(ReportUri + "getList_NhomDoiTuong?LoaiDoiTuong=2", "GET").done(function (data) {
            self.NhomNhaCungCaps(data.LstData);
            self.searchNhomNhaCungCap(data.LstData);
            //for (let i = 0; i < self.NhomNhaCungCaps().length; i++) {
            //    if (i === 0)
            //        _tenNhomNhaCungCapSeach = self.NhomNhaCungCaps()[i].ID;
            //    else
            //        _tenNhomNhaCungCapSeach = self.NhomNhaCungCaps()[i].ID + "," + _tenNhomNhaCungCapSeach;
            //}
        });
    }
    getList_NhomNhaCungCaps();
    self.CloseNhomNhaCungCap = function (item) {
        _tenNhomNhaCungCapSeach = null;
        self.MangNhomNhaCungCap.remove(item);
        for (let i = 0; i < self.MangNhomNhaCungCap().length; i++) {
            if (i === 0)
                _tenNhomNhaCungCapSeach = self.MangNhomNhaCungCap()[i].ID;
            else
                _tenNhomNhaCungCapSeach = self.MangNhomNhaCungCap()[i].ID + "," + _tenNhomNhaCungCapSeach;
        }
        //if (self.MangNhomNhaCungCap().length === 0) {
        //    for (let i = 0; i < self.searchNhomNhaCungCap().length; i++) {
        //        if (i === 0)
        //            _tenNhomNhaCungCapSeach = self.searchNhomNhaCungCap()[i].ID;
        //        else
        //            _tenNhomNhaCungCapSeach = self.searchNhomNhaCungCap()[i].ID + "," + _tenNhomNhaCungCapSeach;
        //    }
        //}
        // remove check
        $('#selec-all-NhomNhaCungCap li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        _pageNumber = 1;
        self.LoadReport();
    };
    self.SelectedNhomNhaCungCap = function (item) {
        _tenNhomNhaCungCapSeach = null;
        var arrIDNhomNhaCungCap = [];
        for (let i = 0; i < self.MangNhomNhaCungCap().length; i++) {
            if ($.inArray(self.MangNhomNhaCungCap()[i], arrIDNhomNhaCungCap) === -1) {
                arrIDNhomNhaCungCap.push(self.MangNhomNhaCungCap()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDNhomNhaCungCap) === -1) {
            self.MangNhomNhaCungCap.push(item);
            for (let i = 0; i < self.MangNhomNhaCungCap().length; i++) {
                if (i === 0)
                    _tenNhomNhaCungCapSeach = self.MangNhomNhaCungCap()[i].ID;
                else
                    _tenNhomNhaCungCapSeach = self.MangNhomNhaCungCap()[i].ID + "," + _tenNhomNhaCungCapSeach;
            }
        }
        // sau khi tìm kiếm thì trả về mặc định
        $('#NoteNameNhomNhaCungCap').val('');
        self.NhomNhaCungCaps(self.searchNhomNhaCungCap());
        //đánh dấu check
        for (let i = 0; i < self.MangNhomNhaCungCap().length; i++) {
            $('#selec-all-NhomNhaCungCap li').each(function () {
                if ($(this).attr('id') === self.MangNhomNhaCungCap()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>');
                }
            });
        }
        _pageNumber = 1;
        self.LoadReport();
    };
    self.NoteNameNhomNhaCungCap = function () {
        var arrNhomNhaCungCap = [];
        var itemSearch = locdau($('#NoteNameNhomNhaCungCap').val().toLowerCase());
        for (let i = 0; i < self.searchNhomNhaCungCap().length; i++) {
            var locdau_kd = self.searchNhomNhaCungCap()[i].TenNhomNhaCungCap_KhongDau;
            var locdau_ktd = self.searchNhomNhaCungCap()[i].TenNhomNhaCungCap_KyTuDau;
            var R1 = locdau_kd.split(itemSearch);
            var R2 = locdau_ktd.split(itemSearch);
            if (R1.length > 1 || R2.length > 1) {
                arrNhomNhaCungCap.push(self.searchNhomNhaCungCap()[i]);
            }
        }
        self.NhomNhaCungCaps(arrNhomNhaCungCap);
        if ($('#NoteNameNhomNhaCungCap').val() === "") {
            self.NhomNhaCungCaps(self.searchNhomNhaCungCap());
            for (let i = 0; i < self.MangNhomNhaCungCap().length; i++) {
                $('#selec-all-NhomNhaCungCap li').each(function () {
                    if ($(this).attr('id') === self.MangNhomNhaCungCap()[i].ID) {
                        $(this).find('i').remove();
                        $(this).append('<i class="fa fa-check check-after-li"></i>');
                    }
                });
            }
        }
    };
    $('#NoteNameNhomNhaCungCap').keypress(function (e) {
        if (e.keyCode === 13 && self.NhomNhaCungCaps().length > 0) {
            self.SelectedNhomNhaCungCap(self.NhomNhaCungCaps()[0]);
        }
    });
    //load Loại thu chi
    var _tenLoaiThuChiSeach = "1,3,5";
    var mang1 = { ID: "1", TenLoaiThuChi: "Phiếu thu khác" };
    var mang2 = { ID: "2", TenLoaiThuChi: "Phiếu chi khác" };
    var mang3 = { ID: "3", TenLoaiThuChi: "Thu tiền khách trả" };
    var mang4 = { ID: "4", TenLoaiThuChi: "Chi tiền trả khách" };
    var mang5 = { ID: "5", TenLoaiThuChi: "Thu tiền NCC trả" };
    var mang6 = { ID: "6", TenLoaiThuChi: "Chi tiền trả NCC" };
    self.LoaiThuChis = ko.observableArray();
    self.MangLoaiThuChi = ko.observableArray();
    self.CloseLoaiThuChi = function (item) {
        _tenLoaiThuChiSeach = null;
        self.MangLoaiThuChi.remove(item);
        for (let i = 0; i < self.MangLoaiThuChi().length; i++) {
            _tenLoaiThuChiSeach = self.MangLoaiThuChi()[i].ID + "," + _tenLoaiThuChiSeach;
        }
        if (self.MangLoaiThuChi().length === 0) {
            if (self.check_MoiQuanTam() === 2)
                _tenLoaiThuChiSeach = "1,3,5";
            else if (self.check_MoiQuanTam() === 3)
                _tenLoaiThuChiSeach = "2,4,6";
            else
                _tenLoaiThuChiSeach = "1,2,3,4,5,6";
        }
        // remove check
        $('#selec-all-LoaiThuChi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        self.LoadReport();
    };
    self.SelectedLoaiThuChi = function (item) {
        _tenLoaiThuChiSeach = null;
        var arrIDLoaiThuChi = [];
        for (let i = 0; i < self.MangLoaiThuChi().length; i++) {
            if ($.inArray(self.MangLoaiThuChi()[i], arrIDLoaiThuChi) === -1) {
                arrIDLoaiThuChi.push(self.MangLoaiThuChi()[i].ID);
            }
        }
        if ($.inArray(item.ID, arrIDLoaiThuChi) === -1) {
            self.MangLoaiThuChi.push(item);
            for (let i = 0; i < self.MangLoaiThuChi().length; i++) {
                if (_tenLoaiThuChiSeach === null)
                    _tenLoaiThuChiSeach = self.MangLoaiThuChi()[i].ID;
                else
                    _tenLoaiThuChiSeach = self.MangLoaiThuChi()[i].ID + "," + _tenLoaiThuChiSeach;
            }
        }
        $('#NoteNameLoaiThuChi').val('');
        self.LoaiThuChis(self.searchLoaiThuChi());

        //đánh dấu check
        for (let i = 0; i < self.MangLoaiThuChi().length; i++) {
            $('#selec-all-LoaiThuChi li').each(function () {
                if ($(this).attr('id') === self.MangLoaiThuChi()[i].ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>');
                }
            });
        }
        self.LoadReport();
    };
    self.NoteNameThuChi = function () {
        var arrLoaiThuChi = [];
        var itemSearch = locdau($('#NoteNameLoaiThuChi').val().toLowerCase());
        for (let i = 0; i < self.searchLoaiThuChi().length; i++) {
            var locdauInput = locdau(self.searchLoaiThuChi()[i].TenLoaiThuChi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrLoaiThuChi.push(self.searchLoaiThuChi()[i]);
            }
        }
        self.LoaiThuChis(arrLoaiThuChi);
        if ($('#NoteNameLoaiThuChi').val() === "") {
            self.LoaiThuChis(self.searchLoaiThuChi());
        }

    };
    $('#NoteNameLoaiThuChi').keypress(function (e) {
        if (e.keyCode === 13 && self.LoaiThuChis().length > 0) {
            self.SelectedLoaiThuChi(self.LoaiThuChis()[0]);
        }
    });
    $('.chose_Time input').on('click', function () {
        self.hideTableReport();
        _kieubang = parseInt($(this).val());
        if (parseInt($(this).val()) === 1) {
            if (self.check_MoiQuanTam() === 5) {
                self.MoiQuanTam('Báo cáo kết quả hoạt động kinh doanh theo tháng');
                $('.table_TheoThang').show();
            }
            else {
                self.MoiQuanTam('Báo cáo phân tích thu chi theo tháng');
                $('.table_PTTCTheoThang').show();
            }
            self.LoadReport();
        }
        else if (parseInt($(this).val()) === 2) {
            if (self.check_MoiQuanTam() === 5) {
                self.MoiQuanTam('Báo cáo kết quả hoạt động kinh doanh theo quý');
                $('.table_TheoQuy').show();
            }
            else {
                self.MoiQuanTam('Báo cáo phân tích thu chi theo quý');
                $('.table_PTTCTheoQuy').show();
            }
            self.LoadReport();
        }
        else if (parseInt($(this).val()) === 3) {
            if (self.check_MoiQuanTam() === 5) {
                self.MoiQuanTam('Báo cáo kết quả hoạt động kinh doanh theo năm');
                $('.table_TheoNam').show();
            }
            else {
                self.MoiQuanTam('Báo cáo phân tích thu chi theo năm');
                $('.table_PTTCTheoNam').show();
            }
            self.LoadReport();
        }
    });
    var _LoaiTienPTTC = 12;
    $('.chose_LoaiTien input').on('click', function () {
        _LoaiTienPTTC = parseInt($(this).val());
        console.log(_LoaiTienPTTC);
        self.LoadReport();
    });
    //trinhpv phân quyền
    self.BaoCaoBanHang = ko.observable();
    self.BCTC_TongHopCongNo = ko.observable();
    self.BCTC_NhatKyThuTien = ko.observable();
    self.BCTC_NhatKyChiTien = ko.observable();
    self.BCTC_SoQuy = ko.observable();
    self.BCTC_TaiChinh = ko.observable();
    self.BCTC_PhanTichThuChi = ko.observable();
    self.BCTC_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TongHopCongNo", "GET").done(function (data) {
            self.BCTC_TongHopCongNo(data);
            getDonVi();
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_NhatKyThuTien", "GET").done(function (data) {
            self.BCTC_NhatKyThuTien(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_NhatKyChiTien", "GET").done(function (data) {
            self.BCTC_NhatKyChiTien(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_SoQuy", "GET").done(function (data) {
            self.BCTC_SoQuy(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_TaiChinh", "GET").done(function (data) {
            self.BCTC_TaiChinh(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_PhanTichThuChi", "GET").done(function (data) {
            self.BCTC_PhanTichThuChi(data);
        });
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCTC_XuatFile", "GET").done(function (data) {
            self.BCTC_XuatFile(data);
        });
    }
    getQuyen_NguoiDung();
    //load đơn vị
    function getDonVi() {
        ajaxHelper(BH_DonViUri + "GetDonVi_byUserSearch?ID_NguoiDung=" + _IDDoiTuong + "&TenDonVi=" + _nameDonViSeach, "GET").done(function (data) {
            self.DonVis(data);
            self.searchDonVi(data);
            for (let i = 0; i < self.DonVis().length; i++) {
                if (self.DonVis()[i].ID === _idDonViSeach) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                    self.SelectedDonVi(self.DonVis()[i]);
                }
            }
            self.ArrDonVi(self.DonVis());
            self.LoadReport();
        });
    }
    //Lua chon don vi
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/';
    var _nameDonViSeach = null;
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray();
    self.MangChiNhanh = ko.observableArray();
    self.CloseDonVi = function (item) {
        _idDonViSeach = null;
        var TenChiNhanh;
        var arrID = [];
        self.MangChiNhanh.remove(item);

        if (item.ID === '00000000-0000-0000-0000-0000-000000000000') {
            arrID = $.map(self.DonVis(), function (x) {
                return x.ID;
            });
            TenChiNhanh = 'Tất cả chi nhánh';
        }
        else {
            self.ArrDonVi.unshift(item);

            if (self.MangChiNhanh().length === 0) {
                $("#NoteNameDonVi").attr("placeholder", "Chọn chi nhánh...");
                TenChiNhanh = 'Tất cả chi nhánh.';
                for (let i = 0; i < self.searchDonVi().length; i++) {
                    if (_idDonViSeach === null)
                        _idDonViSeach = self.searchDonVi()[i].ID;
                    else
                        _idDonViSeach = self.searchDonVi()[i].ID + "," + _idDonViSeach;
                }
                arrID = $.map(self.DonVis(), function (x) {
                    return x.ID;
                });
            }
            else {
                for (let i = 0; i < self.MangChiNhanh().length; i++) {
                    if (_idDonViSeach === null) {
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
        self.LstIDDonVi(arrID);
        self.TenChiNhanh(TenChiNhanh);
        self.LoadReport();
    };

    self.SelectedDonVi = function (item) {
        event.stopPropagation();
        _idDonViSeach = null;
        var TenChiNhanh;

        if (item.ID === undefined) {
            let arrID = $.map(self.DonVis(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);
            // push again lstDV has chosed
            for (let i = 0; i < self.MangChiNhanh().length; i++) {
                if (self.MangChiNhanh()[i].ID !== '00000000-0000-0000-0000-0000-000000000000') {
                    self.ArrDonVi().unshift(self.MangChiNhanh()[i]);
                }
            }
            self.MangChiNhanh([{
                ID: '00000000-0000-0000-0000-0000-000000000000', TenDonVi: 'Tất cả chi nhánh'
            }]);
            TenChiNhanh = 'Tất cả chi nhánh';
        }
        else {
            var arrIDDonVi = [];
            for (let i = 0; i < self.MangChiNhanh().length; i++) {
                if ($.inArray(self.MangChiNhanh()[i], arrIDDonVi) === -1) {
                    arrIDDonVi.push(self.MangChiNhanh()[i].ID);
                }
                if (self.MangChiNhanh()[i].ID === '00000000-0000-0000-0000-0000-000000000000') {
                    self.MangChiNhanh().splice(i, 1);
                }
            }
            if ($.inArray(item.ID, arrIDDonVi) === -1) {
                self.MangChiNhanh.push(item);
                $('#NoteNameDonVi').removeAttr('placeholder');
                for (let i = 0; i < self.MangChiNhanh().length; i++) {
                    if (_idDonViSeach === null) {
                        _idDonViSeach = self.MangChiNhanh()[i].ID;
                        TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                    }
                    else {
                        _idDonViSeach = self.MangChiNhanh()[i].ID + "," + _idDonViSeach;
                        TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                    }
                }
            }
            let arrID = $.map(self.MangChiNhanh(), function (x) {
                return x.ID;
            });
            self.LstIDDonVi(arrID);
            //thêm dấu check vào đối tượng được chọn
            $('#selec-all-DonVi li').each(function () {
                if ($(this).attr('id') === item.ID) {
                    $(this).find('i').remove();
                    $(this).append('<i class="fa fa-check check-after-li"></i>');
                }
            });
        }
        self.TenChiNhanh(TenChiNhanh);
        var arr = $.grep(self.ArrDonVi(), function (x) {
            return x.ID !== item.ID;
        });
        self.ArrDonVi(arr);
        event.preventDefault();
        return false;
    };
    //lọc đơn vị
    self.NoteNameDonVi = function () {
        var arrDonVi = [];
        var itemSearch = locdau($('#NoteNameDonVi').val().toLowerCase());
        for (let i = 0; i < self.searchDonVi().length; i++) {
            var locdauInput = locdau(self.searchDonVi()[i].TenDonVi).toLowerCase();
            var R = locdauInput.split(itemSearch);
            if (R.length > 1) {
                arrDonVi.push(self.searchDonVi()[i]);
            }
        }
        self.DonVis(arrDonVi);
        if ($('#NoteNameDonVi').val() === "") {
            self.DonVis(self.searchDonVi());
        }
    };
    $('#NoteNameDonVi').keypress(function (e) {
        if (e.keyCode === 13 && self.DonVis().length > 0) {
            self.SelectedDonVi(self.DonVis()[0]);
        }
    });
    self.ArrayYear = ko.observableArray();
    self.getListYear = function () {
        ajaxHelper(ReportUri + "getListYear", "GET").done(function (data) {
            self.ArrayYear(data);
            if (data.length > 0)
                _year = self.ArrayYear()[0].Year;
            else
                _year = moment(_timeStart).format('YYYY');
            self.newYear(_year);
            self.TodayBC1("Từ ngày 01/01/" + _year + " Đến ngày 31/12/" + _year);
        });
    };
    self.getListYear();

    self.SelectYearReport = function (item) {
        _year = item.Year;
        self.newYear(_year);
        self.TodayBC1("Từ ngày 01/01/" + _year + " Đến ngày 31/12/" + _year);
        $('#selec-all-Year li').each(function () {
            $(this).removeClass('SelectReport');
            if (parseInt($(this).attr('id')) === parseInt(_year)) {
                $(this).addClass('SelectReport');
            }
        });
        self.LoadReport();
    };
    self.check_LocKinhDoanh = ko.observable('1');
    $('.chose_LocKinhDoanh').on('click', 'li', function () {
        self.check_LocKinhDoanh($(this).find('input').val());
        self.LoadReport();
    });
    $('.chose_kieubang').on('click', 'li', function () {
        $('.show_SoQuy').hide();
        self.showHideTable();
        self.LoaiThuChis([]);
        self.MangLoaiThuChi([]);
        $('.showTimeTC').hide();
        $('.showLoaiTien').hide();
        $('.showChiNhanh').hide();
        $('.showLoaiThuChi').hide();
        $('.showLoaiDoiTac').show();
        $('.showNhomKH').show();
        $('.showNhomNCC').show();
        $('.showTimeCN').show();
        $('.showTodayBC').show();
        $('.showTodayBC_ChiNhanh').hide();
        $('.showInput').show();
        $('.showKQKD').show();
        $('.showDate').hide();
        $('#select-column').show();
        loadCheckbox($(this).data('id'));
        $(".chose_kieubang li").each(function (i) {
            $(this).find('a').removeClass('box-tab');
        });
        $(this).find('a').addClass('box-tab');
        self.check_MoiQuanTam(parseInt($(this).find('a input').val()));
        if (self.check_MoiQuanTam() === 1) {
            $('.showKQKD').hide();
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại đối tác").blur();
            self.LoaiBaoCao('đối tác');
            self.MoiQuanTam('Báo cáo tổng hợp công nợ');
        }
        else if (self.check_MoiQuanTam() === 2) {
            _tenLoaiThuChiSeach = "1,3,5";
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            $('.showLoaiThuChi').show();
            self.LoaiThuChis.push(mang1);
            self.LoaiThuChis.push(mang3);
            self.LoaiThuChis.push(mang5);
            self.searchLoaiThuChi(self.LoaiThuChis());
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại người nộp, mã phiếu").blur();
            self.LoaiBaoCao('phiếu thu');
            self.MoiQuanTam('Báo cáo nhật ký thu tiền');
        }
        else if (self.check_MoiQuanTam() === 3) {
            _tenLoaiThuChiSeach = "2,4,6";
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            $('.showLoaiThuChi').show();
            self.LoaiThuChis.push(mang2);
            self.LoaiThuChis.push(mang4);
            self.LoaiThuChis.push(mang6);
            self.searchLoaiThuChi(self.LoaiThuChis());
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại người nhận, mã phiếu").blur();
            self.LoaiBaoCao('phiếu chi');
            self.MoiQuanTam('Báo cáo nhật ký chi tiền');
        }
        else if (self.check_MoiQuanTam() === 4) {
            $('.show_SoQuy').show();
            $('.TonDauKy').show();
            _tenLoaiThuChiSeach = "1,2,3,4,5,6";
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            $('.showLoaiThuChi').show();
            self.LoaiThuChis.push(mang1);
            self.LoaiThuChis.push(mang2);
            self.LoaiThuChis.push(mang3);
            self.LoaiThuChis.push(mang4);
            self.LoaiThuChis.push(mang5);
            self.LoaiThuChis.push(mang6);
            self.searchLoaiThuChi(self.LoaiThuChis());
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại đối tác, mã phiếu").blur();
            self.LoaiBaoCao('phiếu thu chi');
            if (dk_TabSQ === 1) {
                $('.table_SoQuyTienMat').show();
                self.MoiQuanTam('Báo cáo sổ quỹ tiền mặt');
            }

            else if (dk_TabSQ === 2) {
                $('.table_SoQuyNganHang').show();
                self.MoiQuanTam('Báo cáo sổ quỹ ngân hàng');
            }
            else if (dk_TabSQ === 3) {
                $('.table_SoQuyTongQuy').show();
                self.MoiQuanTam('Báo cáo sổ quỹ tổng quỹ');
            }
            else {
                $('.show_SoQuy').hide();
                $('.TonDauKy').hide();
                $('.showDate').show();
                $('.showTimeCN').hide();
                $('.showTimeTC').hide();
                $('.showTodayBC_ChiNhanh').show();
                $('.showTodayBC').hide();
                self.LoaiBaoCao('chi nhánh');
                $('.table_SoQuychinhanh').show();
                self.MoiQuanTam('Báo cáo sổ quỹ theo chi nhánh');
            }
        }
        else if (self.check_MoiQuanTam() === 5) {
            $('.showKQKD').hide();
            $('.showInput').hide();
            $('.showTodayBC').hide();
            $('#select-column').hide();
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            $('.showTimeTC').show();
            $('.showTimeCN').hide();
            $('.showChiNhanh').show();
            $('.showLoaiThuChi').hide();
            $('.showLoaiDoiTac').hide();
            $('.showNhomKH').hide();
            $('.showNhomNCC').hide();
            if (_kieubang === 1) {
                self.MoiQuanTam('Báo cáo kết quả hoạt động kinh doanh theo tháng');
                $('.table_TheoThang').show();
            }
            else if (_kieubang === 2) {
                self.MoiQuanTam('Báo cáo kết quả hoạt động kinh doanh theo quý');
                $('.table_TheoQuy').show();
            }
            else {
                self.MoiQuanTam('Báo cáo kết quả hoạt động kinh doanh theo năm');
                $('.table_TheoNam').show();
            }
        }
        else if (self.check_MoiQuanTam() === 6) {
            _tenLoaiThuChiSeach = "1,2,3,4,5,6";
            if (self.DonVis().length < 2)
                $('.showChiNhanh').hide();
            else
                $('.showChiNhanh').show();
            $('.showLoaiThuChi').show();
            $('#select-column').hide();
            self.LoaiThuChis.push(mang1);
            self.LoaiThuChis.push(mang2);
            self.LoaiThuChis.push(mang3);
            self.LoaiThuChis.push(mang4);
            self.LoaiThuChis.push(mang5);
            self.LoaiThuChis.push(mang6);
            self.searchLoaiThuChi(self.LoaiThuChis());
            $("#txt_search").attr("placeholder", "Theo mã, tên, số điện thoại đối tác, mã phiếu").blur();
            $('.showTimeTC').show();
            $('.showLoaiTien').show();
            $('.showTimeCN').hide();
            $('.show_SoQuy').hide();
            if (_kieubang === 1) {
                self.MoiQuanTam('Báo cáo phân tích thu chi theo tháng');
                $('.table_PTTCTheoThang').show();
            }
            else if (_kieubang === 2) {
                self.MoiQuanTam('Báo cáo phân tích thu chi theo quý');
                $('.table_PTTCTheoQuy').show();
            }
            else {
                self.MoiQuanTam('Báo cáo phân tích thu chi theo năm');
                $('.table_PTTCTheoNam').show();
            }
        }
        self.LoadReport();
    });
    $('.choose_txtTime li').on('click', function () {
        //self.TodayBC($(this).text())
        var _rdoNgayPage = parseInt($(this).val());
        var datime = new Date();
        var datimeBC = new Date();
        //Toàn thời gian
        if (_rdoNgayPage === 13) {
            _timeStart = '2015-09-26';
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
            var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
            let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() + 6))).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));

        }
        //Tuần trước
        else if (_rdoNgayPage === 4) {
            _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
            let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate() - datimeBC.getDay()))).format('YYYY-MM-DD');
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //7 ngày qua
        else if (_rdoNgayPage === 5) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
            let newtime = new Date();
            let _timeBC = moment(new Date(datimeBC.setDate(datimeBC.getDate()))).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Tháng này
        else if (_rdoNgayPage === 6) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Tháng trước
        else if (_rdoNgayPage === 7) {
            _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
            _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //30 ngày qua
        else if (_rdoNgayPage === 8) {
            _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
            let newtime = new Date();
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Quý này
        else if (_rdoNgayPage === 9) {
            _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
            let newtime = new Date(moment().endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        // Quý trước
        else if (_rdoNgayPage === 10) {
            var prevQuarter = moment().quarter() - 1 === 0 ? 1 : moment().quarter() - 1;
            _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
            let newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //Năm này
        else if (_rdoNgayPage === 11) {
            _timeStart = moment().startOf('year').format('YYYY-MM-DD');
            let newtime = new Date(moment().endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        //năm trước
        else if (_rdoNgayPage === 12) {
            var prevYear = moment().year() - 1;
            _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
            let newtime = new Date(moment().year(prevYear).endOf('year'));
            _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
            let dtBC = new Date(_timeEnd);
            let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        }
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.newDateTime').on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('DD/MM/YYYY') + ' - ' + picker.endDate.format('DD/MM/YYYY'));
        var dt = new Date(picker.endDate.format('MM/DD/YYYY'));
        let dtBC = new Date(picker.endDate.format('MM/DD/YYYY'));
        _timeStart = picker.startDate.format('YYYY-MM-DD');
        _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');// picker.endDate.format('YYYY-MM-DD');
        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
        if (_timeStart === _timeBC)
            self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
        else
            self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
        _pageNumber = 1;
        self.LoadReport();
    });
    $('.choose_TimeReport input').on('click', function () {
        //_rdTime = $(this).val();
        if (parseInt($(this).val()) === 1) {
            $('.ip_TimeReport').removeAttr('disabled');
            $('.dr_TimeReport').attr("data-toggle", "dropdown");
            $('.ip_DateReport').attr('disabled', 'false');
            var _rdoNgayPage = $('.ip_TimeReport').val();
            var datime = new Date();
            //Toàn thời gian
            if (_rdoNgayPage === "Toàn thời gian") {
                _timeStart = '2015-09-26';
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
                var lessDays = currentWeekDay === 0 ? 6 : currentWeekDay - 1;
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() + 7))).format('YYYY-MM-DD'); // end of week
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tuần trước
            else if (_rdoNgayPage === "Tuần trước") {
                _timeEnd = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() + 1))).format('YYYY-MM-DD');
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - datime.getDay() - 6))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //7 ngày qua
            else if (_rdoNgayPage === "7 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 6))).format('YYYY-MM-DD');
                let newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tháng này
            else if (_rdoNgayPage === "Tháng này") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth() + 1, 1)).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Tháng trước
            else if (_rdoNgayPage === "Tháng trước") {
                _timeStart = moment(new Date(datime.getFullYear(), datime.getMonth() - 1, 1)).format('YYYY-MM-DD');
                _timeEnd = moment(new Date(datime.getFullYear(), datime.getMonth(), 1)).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //30 ngày qua
            else if (_rdoNgayPage === "30 ngày qua") {
                _timeStart = moment(new Date(datime.setDate(datime.getDate() - 29))).format('YYYY-MM-DD');
                let newtime = new Date();
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Quý này
            else if (_rdoNgayPage === "Quý này") {
                _timeStart = moment().startOf('quarter').format('YYYY-MM-DD');
                let newtime = new Date(moment().endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            // Quý trước
            else if (_rdoNgayPage === "Quý trước") {
                var prevQuarter = moment().quarter() - 1 === 0 ? 1 : moment().quarter() - 1;
                _timeStart = moment().quarter(prevQuarter).startOf('quarter').format('YYYY-MM-DD');
                let newtime = new Date(moment().quarter(prevQuarter).endOf('quarter'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //Năm này
            else if (_rdoNgayPage === "Năm này") {
                _timeStart = moment().startOf('year').format('YYYY-MM-DD');
                let newtime = new Date(moment().endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            //năm trước
            else if (_rdoNgayPage === "Năm trước") {
                var prevYear = moment().year() - 1;
                _timeStart = moment().year(prevYear).startOf('year').format('YYYY-MM-DD');
                let newtime = new Date(moment().year(prevYear).endOf('year'));
                _timeEnd = moment(new Date(newtime.setDate(newtime.getDate() + 1))).format('YYYY-MM-DD');
                let dtBC = new Date(_timeEnd);
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() - 1))).format('YYYY-MM-DD'); // end of week
                self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
            }
            _pageNumber = 1;
            self.LoadReport();
        }
        else if (parseInt($(this).val()) === 2) {
            $('.ip_DateReport').removeAttr('disabled');
            $('.ip_TimeReport').attr('disabled', 'false');
            $('.dr_TimeReport').removeAttr('data-toggle');
            if ($('.ip_DateReport').val() !== "") {
                thisDate = $('.ip_DateReport').val();
                var t = thisDate.split("-");
                var checktime1 = t[0].trim().split("/");
                var checktime2 = t[1].trim().split("/");
                var t1 = t[0].trim().split("/").reverse().join("-");
                var thisDateStart = moment(t1).format('MM/DD/YYYY');
                var t2 = t[1].trim().split("/").reverse().join("-");
                var thisDateEnd = moment(t2).format('MM/DD/YYYY');
                _timeStart = moment(new Date(thisDateStart)).format('YYYY-MM-DD');
                var dt = new Date(thisDateEnd);
                let dtBC = new Date(thisDateEnd);
                _timeEnd = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
                let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate()))).format('YYYY-MM-DD');
                if (_timeStart === _timeBC)
                    self.TodayBC('Ngày nhập: ' + moment(_timeStart).format('DD/MM/YYYY'));
                else
                    self.TodayBC('Từ ngày ' + moment(_timeStart).format('DD/MM/YYYY') + ' Đến ngày ' + moment(_timeBC).format('DD/MM/YYYY'));
                _pageNumber = 1;
                self.LoadReport();
            }
        }
    });
    $('#datetimepicker_mask').keypress(function (e) {
        if (e.keyCode === 13) {
            //dktime = $(this).val();
            thisDate = $(this).val();
            var t = thisDate.split(" ");
            var t1 = t[0].split("/").reverse().join("-");
            thisDate = moment(t1).format('MM/DD/YYYY');
            _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
            var dt = new Date(thisDate);
            _timeSQ_ChiNhanh = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
            console.log(_timeEnd);
            if (thisDate !== 'Invalid date') {
                self.TodayBC_ChiNhanh('Đến ngày: ' + $(this).val());
                self.LoadReport();
            }
            else {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
            }
        }
    });
    $('#datetimepicker_mask').on('change.dp', function (e) {
        //dktime = $(this).val();
        thisDate = $(this).val();
        var t = thisDate.split(" ");
        var t1 = t[0].split("/").reverse().join("-");
        thisDate = moment(t1).format('MM/DD/YYYY');
        _tonkhoStart = moment(new Date(thisDate)).format('YYYY-MM-DD');
        var dt = new Date(thisDate);
        _timeSQ_ChiNhanh = moment(new Date(dt.setDate(dt.getDate() + 1))).format('YYYY-MM-DD');
        if (thisDate !== 'Invalid date') {
            self.TodayBC_ChiNhanh('Đến ngày: ' + $(this).val());
            self.LoadReport();
        }
        else {
            bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Định dạng thời gian không chính xác!", "danger");
        }
    });
    self.Select_Text = function () {
        Text_search = $('#txt_search').val();
    };
    $('#txt_search').keypress(function (e) {
        if (e.keyCode === 13) {
            self.LoadReport();
        }
    });
    self.showHideTable = function () {
        //$('.show_SoQuy').show();
        $('.TonDauKy').show();
        $('.table_SoQuyTienMat').hide();
        $('.table_SoQuyNganHang').hide();
        $('.table_SoQuyTongQuy').hide();
        $('.table_SoQuychinhanh').hide();
    };
    self.showHideTable();
    var dk_TabSQ = 1;
    self.select_SoQuyTienMat = function () {
        $('.show_SoQuy').show();
        self.LoaiBaoCao('phiếu thu chi');
        self.MoiQuanTam('Báo cáo sổ quỹ tiền mặt');
        dk_TabSQ = 1;
        $('.showDate').hide();
        $('.showTimeCN').show();
        $('.showTimeTC').hide();
        $('.showTodayBC_ChiNhanh').hide();
        $('.showTodayBC').show();
        self.showHideTable();
        $('.table_SoQuyTienMat').show();
        self.LoadReport();
    };
    self.select_SoQuyNganHang = function () {
        $('.show_SoQuy').show();
        self.LoaiBaoCao('phiếu thu chi');
        self.MoiQuanTam('Báo cáo sổ quỹ ngân hàng');
        self.showHideTable();
        $('.table_SoQuyNganHang').show();
        dk_TabSQ = 2;
        $('.showDate').hide();
        $('.showTimeCN').show();
        $('.showTimeTC').hide();
        $('.showTodayBC_ChiNhanh').hide();
        $('.showTodayBC').show();
        self.LoadReport();
    };
    self.select_SoQuyTongQuy = function () {
        $('.show_SoQuy').show();
        self.LoaiBaoCao('phiếu thu chi');
        self.MoiQuanTam('Báo cáo sổ quỹ tổng quỹ');
        self.showHideTable();
        $('.table_SoQuyTongQuy').show();
        dk_TabSQ = 3;
        $('.showDate').hide();
        $('.showTimeCN').show();
        $('.showTimeTC').hide();
        $('.showTodayBC_ChiNhanh').hide();
        $('.showTodayBC').show();
        self.LoadReport();
    };
    self.select_SoQuyChiNhanh = function () {
        self.LoaiBaoCao('chi nhánh');
        self.MoiQuanTam('Báo cáo sổ quỹ theo chi nhánh');
        self.showHideTable();
        $('.show_SoQuy').hide();
        $('.TonDauKy').hide();
        $('.table_SoQuychinhanh').show();
        dk_TabSQ = 4;
        $('.showDate').show();
        $('.showTimeCN').hide();
        $('.showTimeTC').hide();
        $('.showTodayBC_ChiNhanh').show();
        $('.showTodayBC').hide();
        self.LoadReport();
    };
    var Text_search = "";
    var LoaiDT = 2;
    //Loại hàng
    var _ckHangHoa = 1;
    var _ckDichVu = 1;
    $('.choose_LoaiHang input').on('click', function () {
        if (parseInt($(this).val()) === 1) {
            if (_ckHangHoa === 1 & _ckDichVu === 1) {
                _ckHangHoa = 0;
                LoaiDT = 0;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 1) {
                _ckHangHoa = 1;
                LoaiDT = 2;
            }
            else if (_ckHangHoa === 1 & _ckDichVu === 0) {
                _ckHangHoa = 0;
                LoaiDT = 3;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 0) {
                _ckHangHoa = 1;
                LoaiDT = 1;
            }
        }
        if (parseInt($(this).val()) === 2) {
            if (_ckHangHoa === 1 & _ckDichVu === 1) {
                _ckDichVu = 0;
                LoaiDT = 1;
            }
            else if (_ckHangHoa === 1 & _ckDichVu === 0) {
                _ckDichVu = 1;
                LoaiDT = 2;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 1) {
                _ckDichVu = 0;
                LoaiDT = 3;
            }
            else if (_ckHangHoa === 0 & _ckDichVu === 0) {
                _ckDichVu = 1;
                LoaiDT = 0;
            }
        }
        _pageNumber = 1;
        self.LoadReport();
    });
    self.BaoCaoTaiChinh_CongNo = ko.observableArray();
    self.CN_PhaiThuDauKy = ko.observable();
    self.CN_PhaiTraDauKy = ko.observable();
    self.CN_TongTienChi = ko.observable();
    self.CN_TongTienThu = ko.observable();
    self.CN_PhaiThuCuoiKy = ko.observable();
    self.CN_PhaiTraCuoiKy = ko.observable();
    self.BaoCaoTaiChinh_NhatKyPhieuThu = ko.observableArray();
    self.PT_GiaTri = ko.observable();
    self.BaoCaoTaiChinh_NhatKyPhieuChi = ko.observableArray();
    self.PC_GiaTri = ko.observable();
    self.BaoCaoTaiChinh_SoQuyTienMat = ko.observableArray();
    self.SQ_TonDauKy = ko.observable();
    self.SQ_TongThu = ko.observable();
    self.SQ_TongChi = ko.observable();
    self.SQ_TonTrongKy = ko.observable();
    self.SQ_TonCuoiKy = ko.observable();
    self.SQCN_TonTienMat = ko.observable();
    self.SQCN_TonTienGui = ko.observable();
    self.SQCN_TongThuChi = ko.observable();
    self.BaoCaoTaiChinh_SoQuyNganHang = ko.observableArray();
    self.BaoCaoTaiChinh_SoQuyTongQuy = ko.observableArray();
    self.BaoCaoTaiChinh_SoQuyChiNhanh = ko.observableArray();

    self.LoadReport = function () {
        LoadingForm(true);
        $('.table-reponsive').css('display', 'none');
        _pageNumber = 1;
        self.pageNumber_CN(1);
        self.pageNumber_PT(1);
        self.pageNumber_PC(1);
        self.pageNumber_SQTM(1);
        self.pageNumber_SQNH(1);
        self.pageNumber_SQTQ(1);

        var array_Seach = {
            MaHangHoa: Text_search,
            year: _year,
            timeStart: _timeStart,
            timeEnd: self.check_MoiQuanTam() === 4 && dk_TabSQ === 4 ? _timeSQ_ChiNhanh : _timeEnd,
            ID_DonVi: _id_DonVi,
            ID_ChiNhanh: _idDonViSeach !== null ? _idDonViSeach : _id_DonVi,
            LoaiDoiTuong: LoaiDT,
            ID_NhomKhachHang: _tenNhomKhachHangSeach,
            ID_NhomNhaCungCap: _tenNhomNhaCungCapSeach,
            LoaiThuChi: _tenLoaiThuChiSeach,
            HachToanKD: self.check_LocKinhDoanh(),
            LoaiTien: self.check_MoiQuanTam() === 6 ? _LoaiTienPTTC : dk_TabSQ,
            columnsHide: null,
            TodayBC: null,
            TenChiNhanh: null,
            lstIDChiNhanh: self.LstIDDonVi(),
        };
        console.log(array_Seach);
        if (self.check_MoiQuanTam() === 1) {
            if (self.BCTC_TongHopCongNo() === "BCTC_TongHopCongNo") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoTaiChinh_CongNo", "POST", array_Seach).done(function (data) {
                    self.BaoCaoTaiChinh_CongNo(data.LstData);
                    AllPage = data.numberPage;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.CN_PhaiThuDauKy(data.a1);
                    self.CN_PhaiTraDauKy(data.a2);
                    self.CN_TongTienChi(data.a3);
                    self.CN_TongTienThu(data.a4);
                    self.CN_PhaiThuCuoiKy(data.a5);
                    self.CN_PhaiTraCuoiKy(data.a6);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_CongNo").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() === 2) {
            if (self.BCTC_NhatKyThuTien() === "BCTC_NhatKyThuTien") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoTaiChinh_ThuChi", "POST", array_Seach).done(function (data) {
                    self.BaoCaoTaiChinh_NhatKyPhieuThu(data.LstData);
                    AllPage = data.numberPage;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.PT_GiaTri(data.a1);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_PhieuThu").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() === 3) {
            if (self.BCTC_NhatKyChiTien() === "BCTC_NhatKyChiTien") {
                $(".PhanQuyen").hide();
                ajaxHelper(ReportUri + "BaoCaoTaiChinh_ThuChi", "POST", array_Seach).done(function (data) {
                    self.BaoCaoTaiChinh_NhatKyPhieuChi(data.LstData);
                    AllPage = data.numberPage;
                    self.selecPage();
                    self.ReserPage();
                    self.SumRowsHangHoa(data.Rowcount);
                    self.PC_GiaTri(data.a1);
                    LoadingForm(false);
                });
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_PhieuChi").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() === 4) {
            if (self.BCTC_SoQuy() === "BCTC_SoQuy") {
                $(".PhanQuyen").hide();
                if (dk_TabSQ === 1) {
                    ajaxHelper(ReportUri + "BaoCaoTaiChinh_SoQuy", "POST", array_Seach).done(function (data) {
                        if (data.LstData.length > 0 && data.LstData[0].MaPhieuThu !== null) {
                            self.BaoCaoTaiChinh_SoQuyTienMat(data.LstData);
                            self.SumRowsHangHoa(data.Rowcount);
                        }
                        else {
                            self.BaoCaoTaiChinh_SoQuyTienMat([]);
                            self.SumRowsHangHoa(0);
                        }
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SQ_TonDauKy(data.a1);
                        self.SQ_TongThu(data.a4);
                        self.SQ_TongChi(data.a5);
                        self.SQ_TonTrongKy(data.a10);
                        self.SQ_TonCuoiKy(data.a11);
                        LoadingForm(false);
                    });
                }
                else if (dk_TabSQ === 2) {
                    ajaxHelper(ReportUri + "BaoCaoTaiChinh_SoQuy", "POST", array_Seach).done(function (data) {
                        if (data.LstData.length > 0 && data.LstData[0].MaPhieuThu !== null) {
                            self.BaoCaoTaiChinh_SoQuyNganHang(data.LstData);
                            self.SumRowsHangHoa(data.Rowcount);
                        }
                        else {
                            self.BaoCaoTaiChinh_SoQuyNganHang([]);
                            self.SumRowsHangHoa(0);
                        }
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SQ_TonDauKy(data.a1);
                        self.SQ_TongThu(data.a6);
                        self.SQ_TongChi(data.a7);
                        self.SQ_TonTrongKy(data.a12);
                        self.SQ_TonCuoiKy(data.a13);
                        LoadingForm(false);
                    });
                }
                else if (dk_TabSQ === 3) {
                    ajaxHelper(ReportUri + "BaoCaoTaiChinh_SoQuy", "POST", array_Seach).done(function (data) {
                        if (data.LstData.length > 0 && data.LstData[0].MaPhieuThu !== null) {
                            self.BaoCaoTaiChinh_SoQuyTongQuy(data.LstData);
                            self.SumRowsHangHoa(data.Rowcount);
                        }
                        else {
                            self.BaoCaoTaiChinh_SoQuyTongQuy([]);
                            self.SumRowsHangHoa(0);
                        }
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SQ_TonDauKy(data.a1);
                        self.SQ_TongThu(data.a2);
                        self.SQ_TongChi(data.a3);
                        self.SQ_TonTrongKy(data.a8);
                        self.SQ_TonCuoiKy(data.a9);
                        LoadingForm(false);
                    });
                }
                else {
                    ajaxHelper(ReportUri + "BaoCaoTaiChinh_SoQuyTheoChiNhanh", "POST", array_Seach).done(function (data) {
                        self.BaoCaoTaiChinh_SoQuyChiNhanh(data.LstData);
                        AllPage = data.numberPage;
                        self.selecPage();
                        self.ReserPage();
                        self.SumRowsHangHoa(data.Rowcount);
                        self.SQCN_TonTienMat(data.a1);
                        self.SQCN_TonTienGui(data.a2);
                        self.SQCN_TongThuChi(data.a3);
                        LoadingForm(false);
                    });
                }
            }
            else {
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".TC_SoQuyTienMat").hide();
                $(".TC_SoQuyNganHang").hide();
                $(".TC_SoQuyTongQuy").hide();
                $(".TC_SoQuyChiNhanh").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() === 5) {
            if (self.BCTC_TaiChinh() === "BCTC_TaiChinh") {
                $('.table-reponsive').css('display', 'block');
                $(".page").hide();
                $(".PhanQuyen").hide();
                if (_kieubang === 1)
                    self.getListTaiChinh_TheoThang();
                else if (_kieubang === 2)
                    self.getListTaiChinh_TheoQuy();
                else
                    self.getListTaiChinh_TheoNam();
                LoadingForm(false);
            }
            else {
                $('.table-reponsive').css('display', 'block');
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
        else if (self.check_MoiQuanTam() === 6) {
            if (self.BCTC_PhanTichThuChi() === "BCTC_PhanTichThuChi") {
                $('.table-reponsive').css('display', 'block');
                $(".page").hide();
                $(".PhanQuyen").hide();
                if (_kieubang === 1)
                    self.PhanTichThuChi_TheoThang(array_Seach);
                else if (_kieubang === 2)
                    self.PhanTichThuChi_TheoQuy(array_Seach);
                else
                    self.PhanTichThuChi_TheoNam(array_Seach);
                LoadingForm(false);
            }
            else {
                $('.table-reponsive').css('display', 'block');
                $(".PhanQuyen").show();
                $(".Report_Empty").hide();
                $(".page").hide();
                LoadingForm(false);
            }
        }
    };
    self.BaoCaoTaiChinh_CongNo_Page = ko.computed(function (x) {
        var first = (self.pageNumber_CN() - 1) * self.pageSize();
        if (self.BaoCaoTaiChinh_CongNo() !== null) {
            if (self.BaoCaoTaiChinh_CongNo().length !== 0) {
                $('.TC_CongNo').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_CN() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_CN() - 1) * self.pageSize() + self.BaoCaoTaiChinh_CongNo().slice(first, first + self.pageSize()).length);
            }
            else {
                $('.TC_CongNo').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoTaiChinh_CongNo().slice(first, first + _pageSize);
        }
        return null;
    });
    self.BaoCaoTaiChinh_PhieuThu_Page = ko.computed(function (x) {
        var first = (self.pageNumber_PT() - 1) * self.pageSize();
        if (self.BaoCaoTaiChinh_NhatKyPhieuThu() !== null) {
            if (self.BaoCaoTaiChinh_NhatKyPhieuThu().length !== 0) {
                $('.TC_PhieuThu').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_PT() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_PT() - 1) * self.pageSize() + self.BaoCaoTaiChinh_NhatKyPhieuThu().slice(first, first + self.pageSize()).length);
            }
            else {
                $('.TC_PhieuThu').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoTaiChinh_NhatKyPhieuThu().slice(first, first + _pageSize);
        }
        return null;
    });
    self.BaoCaoTaiChinh_PhieuChi_Page = ko.computed(function (x) {
        var first = (self.pageNumber_PC() - 1) * self.pageSize();
        if (self.BaoCaoTaiChinh_NhatKyPhieuChi() !== null) {
            if (self.BaoCaoTaiChinh_NhatKyPhieuChi().length !== 0) {
                $('.TC_PhieuChi').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_PC() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_PC() - 1) * self.pageSize() + self.BaoCaoTaiChinh_NhatKyPhieuChi().slice(first, first + self.pageSize()).length);
            }
            else {
                $('.TC_PhieuChi').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoTaiChinh_NhatKyPhieuChi().slice(first, first + _pageSize);
        }
        return null;
    });

    self.BaoCaoTaiChinh_SoQuyTienMat_Page = ko.computed(function (x) {
        var first = (self.pageNumber_SQTM() - 1) * self.pageSize();
        if (self.BaoCaoTaiChinh_SoQuyTienMat() !== null) {
            if (self.BaoCaoTaiChinh_SoQuyTienMat().length !== 0) {
                $('.TC_SoQuyTienMat').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_SQTM() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_SQTM() - 1) * self.pageSize() + self.BaoCaoTaiChinh_SoQuyTienMat().slice(first, first + self.pageSize()).length);
            }
            else {
                $('.TC_SoQuyTienMat').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoTaiChinh_SoQuyTienMat().slice(first, first + _pageSize);
        }
        return null;
    });
    self.BaoCaoTaiChinh_SoQuyNganHang_Page = ko.computed(function (x) {
        var first = (self.pageNumber_SQNH() - 1) * self.pageSize();
        if (self.BaoCaoTaiChinh_SoQuyNganHang() !== null) {
            if (self.BaoCaoTaiChinh_SoQuyNganHang().length !== 0) {
                $('.TC_SoQuyNganHang').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_SQNH() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_SQNH() - 1) * self.pageSize() + self.BaoCaoTaiChinh_SoQuyNganHang().slice(first, first + self.pageSize()).length);
            }
            else {
                $('.TC_SoQuyNganHang').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoTaiChinh_SoQuyNganHang().slice(first, first + _pageSize);
        }
        return null;
    });
    self.BaoCaoTaiChinh_SoQuyTongQuy_Page = ko.computed(function (x) {
        var first = (self.pageNumber_SQTQ() - 1) * self.pageSize();
        if (self.BaoCaoTaiChinh_SoQuyTongQuy() !== null) {
            if (self.BaoCaoTaiChinh_SoQuyTongQuy().length !== 0) {
                $('.TC_SoQuyTongQuy').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_SQTQ() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_SQTQ() - 1) * self.pageSize() + self.BaoCaoTaiChinh_SoQuyTongQuy().slice(first, first + self.pageSize()).length);
            }
            else {
                $('.TC_SoQuyTongQuy').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoTaiChinh_SoQuyTongQuy().slice(first, first + _pageSize);
        }
        return null;
    });
    self.BaoCaoTaiChinh_SoQuyChiNhanh_Page = ko.computed(function (x) {
        var first = (self.pageNumber_SQCN() - 1) * self.pageSize();
        if (self.BaoCaoTaiChinh_SoQuyChiNhanh() !== null) {
            if (self.BaoCaoTaiChinh_SoQuyChiNhanh().length !== 0) {
                $('.TC_SoQuyChiNhanh').show();
                $(".Report_Empty").hide();
                $(".page").show();
                self.RowsStart((self.pageNumber_SQCN() - 1) * self.pageSize() + 1);
                self.RowsEnd((self.pageNumber_SQCN() - 1) * self.pageSize() + self.BaoCaoTaiChinh_SoQuyChiNhanh().slice(first, first + self.pageSize()).length);
            }
            else {
                $('.TC_SoQuyChiNhanh').hide();
                $(".Report_Empty").show();
                $(".page").hide();
                self.RowsStart('0');
                self.RowsEnd('0');
            }
            return self.BaoCaoTaiChinh_SoQuyChiNhanh().slice(first, first + _pageSize);
        }
        return null;
    });
    self.getListTaiChinh_TheoThang = function () {
        ajaxHelper(ReportUri + "getListTaiChinh_TheoThang?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            for (let i = 0; i < data.length; i++) {
                data[i].Padding33 = "";
                data[i].ColorText = "text-right itemRight";
                if (i === 2 || i === 3 || i === 9 || i === 10 || i === 11 || i === 14 || i === 15) {
                    data[i].Padding33 = "mahang padding33";
                    data[i].ColorText = "text-right color_text itemRight";
                }
            }
            self.listReportTaiChinh_TheoThang(data);
            $('.table-reponsive').css('display', 'block');
        });
    };
    self.getListTaiChinh_TheoQuy = function () {
        ajaxHelper(ReportUri + "getListTaiChinh_TheoQuy?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            for (let i = 0; i < data.length; i++) {
                data[i].Padding33 = "";
                data[i].ColorText = "text-right itemRight";
                if (i === 2 || i === 3 || i === 9 || i === 10 || i === 11 || i === 14 || i === 15) {
                    data[i].Padding33 = "mahang padding33";
                    data[i].ColorText = "text-right color_text itemRight";
                }
            }
            self.listReportTaiChinh_TheoQuy(data);
            $('.table-reponsive').css('display', 'block');
        });
    };
    self.getListTaiChinh_TheoNam = function () {
        ajaxHelper(ReportUri + "getListTaiChinh_TheoNam?year=" + _year + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            for (let i = 0; i < data.length; i++) {
                data[i].Padding33 = "";
                data[i].ColorText = "text-right itemRight";
                if (i === 2 || i === 3 || i === 9 || i === 10 || i === 11 || i === 14 || i === 15) {
                    data[i].Padding33 = "mahang padding33";
                    data[i].ColorText = "text-right color_text itemRight";
                }
            }
            self.listReportTaiChinh_TheoNam(data);
            $('.table-reponsive').css('display', 'block');
        });
    };
    self.PhanTichThuChi_TheoThang = function (array_Seach) {
        $(".PhanQuyen").hide();
        ajaxHelper(ReportUri + "BaoCaoTaiChinh_PhanTichThuChiTheoThang", "POST", array_Seach).done(function (data) {
            self.listReportPhanTichThuChi_TheoThang(data.LstData);
            $('.table-reponsive').css('display', 'block');
        });
    };
    self.PhanTichThuChi_TheoQuy = function (array_Seach) {
        $(".PhanQuyen").hide();
        ajaxHelper(ReportUri + "BaoCaoTaiChinh_PhanTichThuChiTheoQuy", "POST", array_Seach).done(function (data) {
            self.listReportPhanTichThuChi_TheoQuy(data.LstData);
            $('.table-reponsive').css('display', 'block');
        });
    };
    self.PhanTichThuChi_TheoNam = function (array_Seach) {
        $(".PhanQuyen").hide();
        ajaxHelper(ReportUri + "BaoCaoTaiChinh_PhanTichThuChiTheoNam", "POST", array_Seach).done(function (data) {
            self.listReportPhanTichThuChi_TheoNam(data.LstData);
            $('.table-reponsive').css('display', 'block');
        });
    };
    self.LoadSoQuy_byChiNhanh = function (item) {
        _idDonViSeach = item.ID;
        dk_TabSQ = 3;
        self.TenChiNhanh(item.TenDonVi);
        _timeEnd = _timeSQ_ChiNhanh;
        let dtBC = new Date(_timeEnd);
        _timeStart = moment(new Date(dtBC.setDate(dtBC.getDate() - 2))).format('YYYY-MM-DD');
        let _timeBC = moment(new Date(dtBC.setDate(dtBC.getDate() + 1))).format('YYYY-MM-DD');
        self.TodayBC('Ngày nhập:' + moment(_timeBC).format('DD/MM/YYYY'));
        $('#tableSoQuy_TongQuy').find('a').click();
        $('.ip_DateReport').removeAttr('disabled');
        $('.ip_TimeReport').attr('disabled', 'false');
        $('.dr_TimeReport').removeAttr('data-toggle');

        $('.newDateTime').val(moment(_timeBC).format('DD/MM/YYYY') + ' - ' + moment(_timeBC).format('DD/MM/YYYY'));
        self.check_Time('2');
        self.MangChiNhanh([]);
        self.SelectedDonVi(item);
        //self.LoadReport();
    };
    self.LoadSoQuy_byMaPhieu = function (item) {
        if (item.MaPhieuThu !== '' && item.MaPhieuThu !== null) {
            localStorage.setItem('FindMaPhieuChi', item.MaPhieuThu);
            let url = "/#/CashFlow";
            window.open(url);
        }
    };
    self.LoadDoiTac_byMaDT = function (item) {
        if (item.MaDoiTac !== '' && item.MaDoiTac !== null) {
            localStorage.setItem('FindKhachHang', item.MaDoiTac);
            let url;
            if (item.MaDoiTac.indexOf('NCC') > -1) {
                url = "/#/Suppliers";
            }
            else {
                url = "/#/Customers";
            }
            window.open(url);
        }
    };
    self.LoadHoaDon_byMaHD = function (item) {
        let url;
        if (item.MaHoaDon !== '' && item.MaHoaDon !== null && item.MaHoaDon !== 'HD trả nhanh') {
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
                    url = "/#/PurchaseOrder";
                }
                else if (maHD.indexOf('THN') > -1) {
                    url = "/#/PurchaseReturns";
                }
                else if (maHD.indexOf('CH') > -1) {
                    url = "/#/Transfers";
                }
                else if (maHD.indexOf('PKK') > -1) {
                    url = "/#/StockTakes";
                }
                else if (maHD.indexOf('XH') > -1) {
                    url = "/#/DamageItems";
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
    };
    //Phân trang
    self.currentPage = ko.observable(1);
    self.GetClass = function (page) {
        return page.SoTrang === self.currentPage() ? "click" : "";
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
    };
    self.ReserPage = function (item) {
        loadHtmlGrid();
        if (_pageNumber > 1 && AllPage > 5/* && nextPage < AllPage - 1*/) {
            if (_pageNumber > 3 && _pageNumber < AllPage - 1) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 2 });
                }
            }
            else if (parseInt(_pageNumber) === parseInt(AllPage) - 1) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 3 });
                }
            }
            else if (_pageNumber === AllPage) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: parseInt(_pageNumber) + i - 4 });
                }
            }
            else if (_pageNumber < 4) {
                for (let i = 0; i < 5; i++) {
                    self.SumNumberPageReport.replace(self.SumNumberPageReport()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumber === 1 && AllPage > 5) {
            for (let i = 0; i < 5; i++) {
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
        if (_pageNumber === AllPage) {
            $('#NextPage').hide();
            $('#EndPage').hide();
        }
        else {
            $('#NextPage').show();
            $('#EndPage').show();
        }

        self.currentPage(parseInt(_pageNumber));
    };
    self.NextPage = function (item) {
        if (_pageNumber < AllPage) {
            _pageNumber = _pageNumber + 1;
            if (self.check_MoiQuanTam() === 1)
                self.pageNumber_CN(_pageNumber);
            else if (self.check_MoiQuanTam() === 2)
                self.pageNumber_PT(_pageNumber);
            else if (self.check_MoiQuanTam() === 3)
                self.pageNumber_PC(_pageNumber);
            else if (self.check_MoiQuanTam() === 4)
                if (dk_TabSQ === 1)
                    self.pageNumber_SQTM(_pageNumber);
                else if (dk_TabSQ === 2)
                    self.pageNumber_SQNH(_pageNumber);
                else if (dk_TabSQ === 3)
                    self.pageNumber_SQTQ(_pageNumber);
                else
                    self.pageNumber_SQCN(_pageNumber);
            self.ReserPage();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            if (self.check_MoiQuanTam() === 1)
                self.pageNumber_CN(_pageNumber);
            else if (self.check_MoiQuanTam() === 2)
                self.pageNumber_PT(_pageNumber);
            else if (self.check_MoiQuanTam() === 3)
                self.pageNumber_PC(_pageNumber);
            else if (self.check_MoiQuanTam() === 4)
                if (dk_TabSQ === 1)
                    self.pageNumber_SQTM(_pageNumber);
                else if (dk_TabSQ === 2)
                    self.pageNumber_SQNH(_pageNumber);
                else if (dk_TabSQ === 3)
                    self.pageNumber_SQTQ(_pageNumber);
                else
                    self.pageNumber_SQCN(_pageNumber);
            self.ReserPage();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        if (self.check_MoiQuanTam() === 1)
            self.pageNumber_CN(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            self.pageNumber_PT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_PC(_pageNumber);
        else if (self.check_MoiQuanTam() === 4)
            if (dk_TabSQ === 1)
                self.pageNumber_SQTM(_pageNumber);
            else if (dk_TabSQ === 2)
                self.pageNumber_SQNH(_pageNumber);
            else if (dk_TabSQ === 3)
                self.pageNumber_SQTQ(_pageNumber);
            else
                self.pageNumber_SQCN(_pageNumber);
        self.ReserPage();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        if (self.check_MoiQuanTam() === 1)
            self.pageNumber_CN(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            self.pageNumber_PT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_PC(_pageNumber);
        else if (self.check_MoiQuanTam() === 4)
            if (dk_TabSQ === 1)
                self.pageNumber_SQTM(_pageNumber);
            else if (dk_TabSQ === 2)
                self.pageNumber_SQNH(_pageNumber);
            else if (dk_TabSQ === 3)
                self.pageNumber_SQTQ(_pageNumber);
            else
                self.pageNumber_SQCN(_pageNumber);
        self.ReserPage();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        if (self.check_MoiQuanTam() === 1)
            self.pageNumber_CN(_pageNumber);
        else if (self.check_MoiQuanTam() === 2)
            self.pageNumber_PT(_pageNumber);
        else if (self.check_MoiQuanTam() === 3)
            self.pageNumber_PC(_pageNumber);
        else if (self.check_MoiQuanTam() === 4)
            if (dk_TabSQ === 1)
                self.pageNumber_SQTM(_pageNumber);
            else if (dk_TabSQ === 2)
                self.pageNumber_SQNH(_pageNumber);
            else if (dk_TabSQ === 3)
                self.pageNumber_SQTQ(_pageNumber);
            else
                self.pageNumber_SQCN(_pageNumber);
        self.ReserPage();
    };
    //Download file excel format (*.xlsx)
    self.DownloadFileTeamplateXLSX = function (item) {
        let url = "/api/DanhMuc/DM_HangHoaAPI/Download_fileExcel?fileSave=" + item;
        window.location.href = url;
    };
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
        for (let i = 0; i < arrayColumn.length; i++) {
            if (i === 0) {
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
                    year: _year,
                    timeStart: _timeStart,
                    timeEnd: _timeEnd,
                    ID_DonVi: _id_DonVi,
                    ID_ChiNhanh: _idDonViSeach,
                    LoaiDoiTuong: LoaiDT,
                    ID_NhomKhachHang: _tenNhomKhachHangSeach,
                    ID_NhomNhaCungCap: _tenNhomNhaCungCapSeach,
                    LoaiThuChi: _tenLoaiThuChiSeach,
                    HachToanKD: self.check_LocKinhDoanh(),
                    LoaiTien: self.check_MoiQuanTam() === 6 ? _LoaiTienPTTC : dk_TabSQ,
                    columnsHide: columnHide,
                    TodayBC: self.check_MoiQuanTam() === 6 ? self.TodayBC1() : self.TodayBC(),
                    TenChiNhanh: self.TenChiNhanh(),
                    lstIDChiNhanh: self.LstIDDonVi(),
                };
                console.log(array_Seach);
                if (self.BCTC_XuatFile() !== "BCTC_XuatFile") {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                    LoadingForm(false);
                    return false;
                }
                if (self.check_MoiQuanTam() === 1 && self.BaoCaoTaiChinh_CongNo().length !== 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCTC_CongNo",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() === 2 && self.BaoCaoTaiChinh_NhatKyPhieuThu().length !== 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCTC_NhatKyThuTien",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() === 3 && self.BaoCaoTaiChinh_NhatKyPhieuChi().length !== 0) {
                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        url: ReportUri + "Export_BCTC_NhatKyChiTien",
                        data: { objExcel: array_Seach },
                        success: function (url) {
                            self.DownloadFileTeamplateXLSX(url);
                            LoadingForm(false);
                        }
                    });
                }
                else if (self.check_MoiQuanTam() === 4) {
                    if (dk_TabSQ === 1 && self.BaoCaoTaiChinh_SoQuyTienMat().length !== 0) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCTC_SoQuyTienMat",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else if (dk_TabSQ === 2 && self.BaoCaoTaiChinh_SoQuyNganHang().length !== 0) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCTC_SoQuyNganHang",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else if (dk_TabSQ === 3 && self.BaoCaoTaiChinh_SoQuyTongQuy().length !== 0) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCTC_SoQuyTongQuy",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else if (dk_TabSQ === 4 && self.BaoCaoTaiChinh_SoQuyChiNhanh().length !== 0) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCTC_SoQuyTheoChiNhanh",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                }
                else if (self.check_MoiQuanTam() === 5) {
                    columnHide = null;
                    let isDonVis = '';
                    for (let i = 0; i < self.LstIDDonVi().length; i++) {
                        isDonVis += self.LstIDDonVi()[i] +',';
                    }
                    isDonVis = Remove_LastComma(isDonVis);
                    let url = '';
                    switch (_kieubang){
                        case 1:
                            url = ReportUri + "Export_BCTC_TheoThang";
                            break;
                        case 2:
                            url = ReportUri + "Export_BCTC_TheoQuy";
                            break;
                        case 3:
                            url = ReportUri + "Export_BCTC_TheoNam";
                            break;
                    }
                    url = url + "?year=" + _year + "&ID_ChiNhanh=" + isDonVis + "&columnsHide=" + columnHide + "&TodayBC=" + self.TodayBC1() + "&TenChiNhanh=" + self.TenChiNhanh();
                    window.location.href = url;
                    LoadingForm(false);
                    
                }
                else if (self.check_MoiQuanTam() === 6) {
                    if (_kieubang === 1) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCTC_PhanTichThuChiTheoThang",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else if (_kieubang === 2) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCTC_PhanTichThuChiTheoQuy",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                    else if (_kieubang === 3) {
                        $.ajax({
                            type: "POST",
                            dataType: "json",
                            url: ReportUri + "Export_BCTC_PhanTichThuChiTheoNam",
                            data: { objExcel: array_Seach },
                            success: function (url) {
                                self.DownloadFileTeamplateXLSX(url);
                                LoadingForm(false);
                            }
                        });
                    }
                }
                else {
                    bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Báo cáo không có dữ liệu", "danger");
                    LoadingForm(false);
                }
            },
            statusCode: {
                404: function () {
                    LoadingForm(false);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
                LoadingForm(false);
            },
            complete: function () {
                LoadingForm(false);
            }
        });
    };
};
var reportFinancial = new ViewModal();
ko.applyBindings(reportFinancial);

$('#selec-all-DonVi').parent().on('hide.bs.dropdown', function () {
    reportFinancial.LoadReport();
});