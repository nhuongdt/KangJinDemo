var ViewModel = function () {
    var self = this;
    self.ColumnsExcelHNCC = ko.observableArray();
    self.ColumnsExcelCNCC = ko.observableArray();
    self.ColumnsExcelNHCC = ko.observableArray();
    var cacheExcelHNCC = true;
    var cacheExcelCNCC = true;
    var cacheExcelNHCC = true;

    var thisDate;
    self.MangChiNhanh = ko.observableArray();
    self.DonVis = ko.observableArray();
    self.searchDonVi = ko.observableArray()
    var _idDonViSeach = $('#hd_IDdDonVi').val();
    var _nameDonViSeach = null;
    var BH_DonViUri = '/api/DanhMuc/DM_DonViAPI/'
    var _IDDoiTuong = $('.idnguoidung').text();

    self._ThanhTien = ko.observable(0);
    self._TienVon = ko.observable(0);
    self._LaiLo = ko.observable(0);
    self.Loc_Table = ko.observable('1');
    self.LoaiSP_HH = ko.observable(true);
    self.LoaiSP_DV = ko.observable(true);
    self.HangHoas = ko.observableArray();
    self.ReportNCC_NhapHang = ko.observableArray();
    self.ReportNCC_NhapHangPrint = ko.observableArray();
    self.ReportNCC_CongNo = ko.observableArray();
    self.ReportNCC_CongNoPrint = ko.observableArray();
    self.SumNoDauKy = ko.observable();
    self.SumNoCuoiKy = ko.observable();
    self.SumGhiNo = ko.observable();
    self.SumGhiCo = ko.observable();
    self.ReportNCC_NhaCungCap = ko.observableArray();
    self.ReportNCC_NhaCungCapPrint = ko.observableArray();
    self.ReportHangBanChiTiet_NhanVien = ko.observableArray();
    self.ReportNCC_NhapHangChiTiet = ko.observableArray();
    self.ReportNCC_NhapHangChiTietPrint = ko.observableArray();
    self.ReportNCC_CongNoChiTiet = ko.observableArray();
    self.ReportNCC_CongNoChiTietPrint = ko.observableArray();
    self.SumSoLuongMuaNCC = ko.observable();
    self.SumGiaTriMuaNCC = ko.observable();
    var _nohientaiFrom = 0;
    var _nohientaiTo = 0;

    self.MoiQuanTam = ko.observable('Báo cáo nhập hàng theo nhà cung cấp');
    var _id_DonVi = $('#hd_IDdDonVi').val();
    self.TenChiNhanh = ko.observable($('.branch label').text());
    self.TodayBC = ko.observable('Tuần này');
    self.SumNumberPageReport = ko.observableArray();
    self.SumNumberPageReportNhapHang = ko.observableArray();
    self.SumNumberPageReportCongNo = ko.observableArray();
    self.RowsStart = ko.observable('1');
    self.RowsEnd = ko.observable('10');
    self.RowsStartNhapHang = ko.observable('1');
    self.RowsEndNhapHang = ko.observable('10');
    self.RowsStartCongNo = ko.observable('1');
    self.RowsEndCongNo = ko.observable('10');
    self.SumRowsHangHoa = ko.observable();
    self.SumRowsHangHoaNhapHang = ko.observable();
    self.SumRowsHangHoaCongNo = ko.observable();
    self.NhomHangHoas = ko.observableArray();
    self.Loc_Table = ko.observable('1');
    self.MangNguoiBan = ko.observableArray();
    self.searchNguoiban = ko.observableArray()
    self.NguoiBans = ko.observableArray();
    self.SumGiaTriNhap = ko.observable();
    self.SumGiaTriTra = ko.observable();
    self.SumGiaTriThuan = ko.observable();
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
    var currentWeekDay = dt1.getDay();
    var lessDays = currentWeekDay == 0 ? 6 : currentWeekDay - 1
    var _timeStart = moment(new Date(dt1.setDate(dt1.getDate() - lessDays))).format('YYYY-MM-DD'); // start of wwek
    var _timeEnd = moment(new Date(dt1.setDate(dt1.getDate() + 7))).format('YYYY-MM-DD'); // end of week
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
    //trinhpv phân quyền
    self.BCNhaCungCap = ko.observable();
    self.BCNCC_CongNo = ko.observable();
    self.BCNCC_CongNo_XuatFile = ko.observable();
    self.BCNCC_NhapHang = ko.observable();
    self.BCNCC_NhapHang_XuatFile = ko.observable();
    self.BCNCC_HangNhap = ko.observable();
    self.BCNCC_HangNhap_XuatFile = ko.observable();
    function getQuyen_NguoiDung() {
        //quyền xem báo cáo
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNhaCungCap", "GET").done(function (data) {
            self.BCNhaCungCap(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNCC_CongNo", "GET").done(function (data) {
            self.BCNCC_CongNo(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNCC_CongNo_XuatFile", "GET").done(function (data) {
            self.BCNCC_CongNo_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNCC_NhapHang", "GET").done(function (data) {
            self.BCNCC_NhapHang(data);
            console.log(data);
            self.getListNCC_NhapHang();
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNCC_NhapHang_XuatFile", "GET").done(function (data) {
            self.BCNCC_NhapHang_XuatFile(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNCC_HangNhap", "GET").done(function (data) {
            self.BCNCC_HangNhap(data);
            console.log(data);
        })
        ajaxHelper(ReportUri + "getQuyen_NguoiDung?ID_NguoiDung=" + _IDDoiTuong + "&ID_DonVi=" + _id_DonVi + "&MaQuyen=" + "BCNCC_HangNhap_XuatFile", "GET").done(function (data) {
            self.BCNCC_HangNhap_XuatFile(data);
            console.log(data);
        })
    }
    getQuyen_NguoiDung();
    self.currentPage = ko.observable(1);
    self.currentPageNhapHang = ko.observable(1);
    self.currentPageCongNo = ko.observable(1);
    self.currentPageMuaHang = ko.observable(1);
    var _ID_NhomHang = null;
    $('#txtMaHH').focus();
    $('#home').removeClass("active")
    $('#info').addClass("active")
    self.check_kieubang = ko.observable('2');
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
                if (self.DonVis()[i].ID == _id_DonVi) {
                    self.TenChiNhanh(self.DonVis()[i].TenDonVi);
                    self.SelectedDonVi(self.DonVis()[i]);
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
        }
        else {
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
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        nextPage = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
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
            nextPage = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListNCC_NhapHang_BieuDo();
                else
                    self.getListNCC_NhapHang();
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListNCC_CongNo_BieuDo();
                else
                    self.getListReportNCC_CongNo();
            }
            else if (_kieubang == 3)
                self.getListReportNCC_NhaCungCap();
        }
        //thêm dấu check vào đối tượng được chọn
        $('#selec-all-DonVi li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
                $(this).append('<i class="fa fa-check check-after-li"></i>')
            }
        });

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
    $('.chose_kieubang li').on('click', function () {
        //self.check_kieubang($(this).val());
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
            if (self.check_kieubang() == 1) {
                self.getListNCC_NhapHang_BieuDo();
            }
            else {
                self.getListNCC_NhapHang();
            }
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1) {
                self.getListNCC_CongNo_BieuDo();
            }
            else {
                self.getListReportNCC_CongNo();
            }
        }
    })
    self.hideTableReport = function () {
        $('.table_BanHang').hide();
        $('.table_LoiNhuan').hide();
        $('.table_NhanVien').hide();
        $(".list_NHNCC").hide();
        $(".list_NHCN").hide();
        $(".list_THCC").hide();
    }
    self.hideTableReport();
    self.hideradio = function () {
        $('.rd_banhang').hide();
    }
    self.hideradio();
    $('.table_BanHang').show();
    $(".list_NHNCC").show();
    var _kieubang = 1;
    $('.chooseTableBC input').on('click', function () {
        var TenChiNhanh = null;
        if (self.MangChiNhanh().length > 0) {
            for (var i = 0; i < self.MangChiNhanh().length; i++) {
                if (TenChiNhanh == null) {
                    TenChiNhanh = self.MangChiNhanh()[i].TenDonVi;
                }
                else {
                    TenChiNhanh = TenChiNhanh + ", " + self.MangChiNhanh()[i].TenDonVi;
                }
            }
        }
        else {
            TenChiNhanh = 'Tất cả chi nhánh.'
        }
        self.TenChiNhanh(TenChiNhanh);
        $('.showChiNhanh').show();
        self.Loc_Table($(this).val())
        _kieubang = $(this).val();
        self.hideTableReport();
        _pageNumber = 1;
        if ($(this).val() == 1) {
            $('.table_BanHang').show();
            $('#BieuDo').show();
            //self.check_kieubang(1);
            self.hideradio();
            $(".list_NHNCC").show();
            $(".list_NHCN").hide();
            $(".list_THCC").hide();
            self.MoiQuanTam('Báo cáo nhập hàng theo nhà cung cấp');
            if (self.check_kieubang() == 1)
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if ($(this).val() == 2) {
            $('.showChiNhanh').hide();
            $('.table_LoiNhuan').show();
            $('#BieuDo').show();
            $(".list_NHNCC").hide();
            $(".list_NHCN").show();
            $(".list_THCC").hide();
            //self.check_kieubang(1);
            $('.rd_banhang').show();
            self.MoiQuanTam('Báo cáo công nợ theo nhà cung cấp');
            //getDonVi();
            for (var i = 0; i < self.searchDonVi().length; i++) {
                if (self.searchDonVi()[i].ID == _id_DonVi) {
                    self.TenChiNhanh(self.searchDonVi()[i].TenDonVi);
                }
            }
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else {
            $('.table_NhanVien').show();
            $("#BieuDo a ").removeClass("box-tab");
            //self.check_kieubang(1);
            self.check_kieubang('2');
            $(".list_NHNCC").hide();
            $(".list_NHCN").hide();
            $(".list_THCC").show();
            $('#home').removeClass("active");
            $('#info').addClass("active");
            $('#BieuDo').hide();
            self.hideradio();
            self.MoiQuanTam('Báo cáo danh sách hàng nhập theo nhà cung cấp');
            self.getListReportNCC_NhaCungCap();
        }
    })

    self.CloseNguoiBan = function (item) {
        _tenNguoiBanSeach = null;
        self.MangNguoiBan.remove(item);
        for (var i = 0; i < self.MangNguoiBan().length; i++) {
            _tenNguoiBanSeach = self.MangNguoiBan()[i].ID + "," + _tenNguoiBanSeach;
        }
        if (self.MangNguoiBan().length === 0) {
            // $('#choose_NguoiBan').append('<input type="text" class="dropdown" placeholder="Chọn người bán...">');
            getAllNSNhanVien();
        }
        // remove check
        $('#selec-all-NguoiBan li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNguoiBan())
        nextPage = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();

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
            for (var i = 0; i < self.MangNguoiBan().length; i++) {
                _tenNguoiBanSeach = self.MangNguoiBan()[i].ID + "," + _tenNguoiBanSeach;
            }
        }
        //$('#choose_NguoiBan input').remove();

        //thêm dấu check vào đối tượng được chọn
        //$('#selec-all-NguoiBan li').each(function () {
        //    if ($(this).attr('id') === item.ID) {
        //        $(this).find('i').remove();
        //        $(this).append('<i class="fa fa-check check-after-li"></i>')
        //    }
        //});

        $('#NoteNameNguoiBan').val('');
        self.NguoiBans(self.searchNguoiban());
        //self.ThangKM(mangThang);
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
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
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
                    self.getListNCC_NhapHang_BieuDo();
                else
                    self.getListNCC_NhapHang();
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListNCC_CongNo_BieuDo();
                else
                    self.getListReportNCC_CongNo();
            }
            else if (_kieubang == 3)
                self.getListReportNCC_NhaCungCap();
            self.ReserPage();
        }
        else if ($(this).val() == 2) {
            $('.ip_DateReport').removeAttr('disabled');
            $('.ip_TimeReport').attr('disabled', 'false');
            $('.dr_TimeReport').removeAttr('data-toggle');
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
                        self.getListNCC_NhapHang_BieuDo();
                    else
                        self.getListNCC_NhapHang();
                }
                else if (_kieubang == 2) {
                    if (self.check_kieubang() == 1)
                        self.getListNCC_CongNo_BieuDo();
                    else
                        self.getListReportNCC_CongNo();
                }
                else if (_kieubang == 3)
                    self.getListReportNCC_NhaCungCap();
                self.ReserPage();
            }
        }
    })

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
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
        self.ReserPage();
    });
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
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
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
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
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
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
    })
    // Key Event maHH
    self.SelectMaHH = function () {
        _maHH = $('#txtMaHH').val();
        console.log(_maHH);
    }
    $('#txtMaHH').keypress(function (e) {
        if (e.keyCode == 13) {
            _pageNumber = 1;
            _pageNumberCongNo = 1;
            _pageNumberMuaHang = 1;
            if (_kieubang == 1) {
                if (self.check_kieubang() == 1)
                    self.getListNCC_NhapHang_BieuDo();
                else
                    self.getListNCC_NhapHang();
            }
            else if (_kieubang == 2) {
                if (self.check_kieubang() == 1)
                    self.getListNCC_CongNo_BieuDo();
                else
                    self.getListReportNCC_CongNo();
            }
            else if (_kieubang == 3)
                self.getListReportNCC_NhaCungCap();
        }
    })

    //function getAllNSNhanVien() {
    //    ajaxHelper(BH_KhuyenMaiUri + "getNhanViens?nameChinhanh=" + _id_DonVi, 'GET').done(function (data) {
    //        self.NguoiBans(data);
    //        self.searchNguoiban(data);
    //        for (var i = 0; i < self.NguoiBans().length; i++) {
    //            if (i == 0) {
    //                _tenNguoiBanSeach = self.NguoiBans()[i].ID;
    //            }
    //            else {
    //                _tenNguoiBanSeach = self.NguoiBans()[i].ID + "," + _tenNguoiBanSeach;
    //            }
    //        }
    //        console.log(_tenNguoiBanSeach)
    //        if (_kieubang == 1)
    //            self.getListNCC_NhapHang();
    //        else if (_kieubang == 2)
    //            self.getListReportNCC_CongNo();
    //        else if (_kieubang == 3)
    //            self.getListReportNCC_NhaCungCap();
    //    });

    //}
    //getAllNSNhanVien();
    //GetListNhomHangHoa
    //function getNhomHangHoa() {
    //    ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
    //        self.NhomHangHoas(data);
    //        console.log(self.NhomHangHoas());
    //    })
    //}
    //getNhomHangHoa();
    self.NoteNhomHang = function () {
        var tk = $('#SeachNhomHang').val();
        console.log(tk);
        if (tk.trim() != '') {
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "SeachDM_NhomHangHoa?TenNhom=" + tk, 'GET').done(function (data) {
                self.NhomHangHoas(data);
            })
        }
        else {
            ajaxHelper("/api/DanhMuc/DM_HangHoaAPI/" + "GetDM_NhomHangHoa", 'GET').done(function (data) {
                self.NhomHangHoas(data);
            })
        }
    }
    self.SelectRepoert_NhomHangHoa = function (item) {
        _ID_NhomHang = item.ID;
        $('.SelectNhomHang li').each(function () {
            if ($(this).attr('id_NhomHang') === item.ID) {
                $(this).addClass('SelectReport');
            }
            else {
                $(this).removeClass('SelectReport');
            }
        });
        $('.SelectALLNhomHang li').removeClass('SelectReport');
        _pageNumber = 1;

        self.getListReportNCC_NhaCungCap();
    }
    $('.SelectALLNhomHang').on('click', function () {
        $('.SelectALLNhomHang li').addClass('SelectReport')
        $('.SelectNhomHang li').each(function () {
            $(this).removeClass('SelectReport');
        });
        _ID_NhomHang = null;
        _pageNumber = 1;
        self.getListReportNCC_NhaCungCap();
    });
    var _pageNumber = 1;
    var _pageNumberNhapHang = 1;
    var _pageNumberCongNo = 1;
    var _pageNumberMuaHang = 1;
    var _pageSize = 10;
    var AllPage;
    var AllPageNhapHang;
    var AllPageCongNo;
    var AllPageMuaHang;
    //$(".PhanQuyen").hide();
    //$(".TongCongNhapHang").hide();
    //$(".page").hide();
    self.getListNCC_NhapHang = function () {
        if (self.BCNCC_NhapHang() == "BCNCC_NhapHang") {
            $(".PhanQuyen").hide();
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListNCC_NhapHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportNCC_NhapHang(data.LstData);
                self.ReportNCC_NhapHangPrint(data.LstDataPrint);
                console.log(data)
                self.SumGiaTriNhap(data._lailo);
                self.SumGiaTriTra(data._tienvon);
                self.SumGiaTriThuan(data._thanhtien);
                if (self.ReportNCC_NhapHang().length != 0) {
                    $(".Report_Empty").hide();
                    $(".page").show();
                    $('.TongCongNhapHang').show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportNCC_NhapHang().length)
                }
                else {
                    $(".Report_Empty").show();
                    $('.TongCongNhapHang').hide();
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
                LoadHtmlGrid(cacheExcelNHCC, 1, "nhaphangNCC_Trung");
            });
        }
        else {
            $(".PhanQuyen").show();
            $(".Report_Empty").hide();
            $(".TongCongNhapHang").hide();
            $(".page").hide();
        }

    }

    self.SelectedPageNumberReportNCC_NhapHang = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListNCC_NhapHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportNCC_NhapHang(data.LstData);
            console.log(data)
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportNCC_NhapHang().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelNHCC, 1, "nhaphangNCC_Trung");
        });
    }
    var _id_DoiTuong = null;
    self.MaNCCPrint = ko.observable();
    self.TenNCCPrint = ko.observable();
    self.NH_SoLuongPrint = ko.observable();
    self.NH_GiaTriPrint = ko.observable();

    self.SelectedReportNCC_NhapHangChiTiet = function (item) {
        self.MaNCCPrint(item.MaNCC);
        self.TenNCCPrint(item.TenNCC);
        _pageNumberNhapHang = 1;
        hidewait('table_h');
        _id_DoiTuong = item.ID_NCC;
        console.log(_id_DoiTuong);
        ajaxHelper(ReportUri + "getListNCC_NhapHangChiTiet?ID_NCC=" + item.ID_NCC + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberNhapHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportNCC_NhapHangChiTiet(data.LstData);
            self.ReportNCC_NhapHangChiTietPrint(data.LstDataPrint);
            self.NH_SoLuongPrint(data._tienvon);
            self.NH_GiaTriPrint(data._thanhtien);
            if (self.ReportNCC_NhapHangChiTiet().length != 0) {
                self.RowsStartNhapHang((_pageNumberNhapHang - 1) * _pageSize + 1);
                self.RowsEndNhapHang((_pageNumberNhapHang - 1) * _pageSize + self.ReportNCC_NhapHangChiTiet().length)
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

    self.SelectedPageNumberReportNCC_NhapHangChiTiet = function (item) {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListNCC_NhapHangChiTiet?ID_NCC=" + _id_DoiTuong + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberNhapHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportNCC_NhapHangChiTiet(data.LstData);
            self.RowsStartNhapHang((_pageNumberNhapHang - 1) * _pageSize + 1);
            self.RowsEndNhapHang((_pageNumberNhapHang - 1) * _pageSize + self.ReportNCC_NhapHangChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.getListReportNCC_CongNo = function () {
        if (self.BCNCC_CongNo() == "BCNCC_CongNo") {
            hidewait('table_h');
            $(".PhanQuyen").hide();
            ajaxHelper(ReportUri + "getListNCC_CongNo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
                self.ReportNCC_CongNo(data.LstData);
                self.ReportNCC_CongNoPrint(data.LstDataPrint);
                self.SumNoDauKy(data.TongTienHang);
                self.SumGhiNo(data.GiamGiaHD);
                self.SumGhiCo(data.DoanhThu);
                self.SumNoCuoiKy(data.GiaTriTra);
                if (self.ReportNCC_CongNo().length != 0) {
                    $(".Report_Empty").hide();
                    $(".page").show();
                    $('.TongCongCongNo').show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportNCC_CongNo().length)
                }
                else {
                    $(".Report_Empty").show();
                    $('.TongCongCongNo').hide();
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
                LoadHtmlGrid(cacheExcelCNCC, 2, "congnoNCC");
            });
        }
        else {
            $(".PhanQuyen").show();
            $(".Report_Empty").hide();
            $(".TongCongCongNo").hide();
            $(".page").hide();
        }
    }
    self.SelectedPageNumberReportNCC_CongNo = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListNCC_CongNo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            self.ReportNCC_CongNo(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportNCC_CongNo().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelCNCC, 2, "congnoNCC");
        });
    }
    //chi tiết công nợ
    var _id_DoiTuongCongNo = null;
    self.SelectedReportNCC_CongNoChiTiet = function (item) {
        _pageNumberCongNo = 1;
        hidewait('table_h');
        _id_DoiTuongCongNo = item.ID_KhachHang;
        ajaxHelper(ReportUri + "getListNCC_CongNoChiTiet?ID_NCC=" + item.ID_KhachHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberCongNo + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            console.log(data);
            self.ReportNCC_CongNoChiTiet(data.LstData);
            self.ReportNCC_CongNoChiTietPrint(data.LstDataPrint);
            self.MaNCCPrint(item.MaKhachHang);
            self.TenNCCPrint(item.TenKhachHang);
            if (self.ReportNCC_CongNoChiTiet().length != 0) {
                self.RowsStartCongNo((_pageNumberCongNo - 1) * _pageSize + 1);
                self.RowsEndCongNo((_pageNumberCongNo - 1) * _pageSize + self.ReportNCC_CongNoChiTiet().length)
            }
            else {
                self.RowsStartCongNo('0');
                self.RowsEndCongNo('0');
            }
            self.SumNumberPageReportCongNo(data.LstPageNumber);
            AllPageCongNo = self.SumNumberPageReportCongNo().length;
            self.selecPageCongNo();
            self.ReserPageCongNo();
            self.SumRowsHangHoaCongNo(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }

    self.SelectedPageNumberReportNCC_CongNoChiTiet = function (item) {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListNCC_CongNoChiTiet?ID_NCC=" + _id_DoiTuongCongNo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberCongNo + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            self.ReportNCC_CongNoChiTiet(data.LstData);
            self.RowsStartCongNo((_pageNumberCongNo - 1) * _pageSize + 1);
            self.RowsEndCongNo((_pageNumberCongNo - 1) * _pageSize + self.ReportNCC_CongNoChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }
    self.getListReportNCC_NhaCungCap = function () {
        if (self.BCNCC_HangNhap() == "BCNCC_HangNhap") {
            $(".PhanQuyen").hide();
            hidewait('table_h');
            ajaxHelper(ReportUri + "getListNCC_MuaHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
                self.ReportNCC_NhaCungCap(data.LstData);
                self.ReportNCC_NhaCungCapPrint(data.LstDataPrint);
                console.log(data)
                self.SumSoLuongMuaNCC(data._lailo);
                self.SumGiaTriMuaNCC(data._thanhtien);
                if (self.ReportNCC_NhaCungCap().length != 0) {
                    $('.TongCongMuaNCC').show();
                    $(".Report_Empty").hide();
                    $(".page").show();
                    self.RowsStart((_pageNumber - 1) * _pageSize + 1);
                    self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportNCC_NhaCungCap().length)
                }
                else {
                    $('.TongCongMuaNCC').hide();
                    $(".page").hide();
                    $(".Report_Empty").show();
                    self.RowsStart('0');
                    self.RowsEnd('0');
                }
                self.SumNumberPageReport(data.LstPageNumber);
                AllPage = self.SumNumberPageReport().length;
                self.selecPage();
                self.ReserPage();
                self.SumRowsHangHoa(data.Rowcount);
                $("div[id ^= 'wait']").text("");
                LoadHtmlGrid(cacheExcelHNCC, 3, "hangnhapNCC");
            });
        }
        else {
            $(".PhanQuyen").show();
            $(".Report_Empty").hide();
            $(".TongCongMuaNCC").hide();
            $(".page").hide();
        }
    }
    self.SelectedPageNumberReportNCC_NhaCungCap = function () {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListNCC_MuaHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportNCC_NhaCungCap(data.LstData);
            self.RowsStart((_pageNumber - 1) * _pageSize + 1);
            self.RowsEnd((_pageNumber - 1) * _pageSize + self.ReportNCC_NhaCungCap().length)
            $("div[id ^= 'wait']").text("");
            LoadHtmlGrid(cacheExcelHNCC, 3, "hangnhapNCC");
        });
    }
    // chi tiết muaHang
    self.ReportNCC_MuaHangChiTiet = ko.observableArray();
    self.ReportNCC_MuaHangChiTietPrint = ko.observableArray();
    self.MH_SoLuongPrint = ko.observableArray();
    self.MH_GiaTriPrint = ko.observableArray();
    var _id_DoiTuongMuaHang = null;
    self.RowsStartMuaHang = ko.observable('1');
    self.RowsEndMuaHang = ko.observable('10');
    self.SumNumberPageReportMuaHang = ko.observableArray();
    self.SumRowsHangHoaMuaHang = ko.observable();
    self.SelectedReportNCC_MuaHangChiTiet = function (item) {
        self.MaNCCPrint(item.MaNCC);
        self.TenNCCPrint(item.TenNCC);
        _pageNumberMuaHang = 1;
        hidewait('table_h');
        console.log(item.ID_NCC);
        _id_DoiTuongMuaHang = item.ID_NCC;
        ajaxHelper(ReportUri + "getListNCC_MuaHangChiTiet?ID_NCC=" + item.ID_NCC + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberMuaHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportNCC_MuaHangChiTiet(data.LstData);
            self.ReportNCC_MuaHangChiTietPrint(data.LstDataPrint);
            self.MH_SoLuongPrint(data._tienvon);
            self.MH_GiaTriPrint(data._thanhtien);

            if (self.ReportNCC_MuaHangChiTiet().length != 0) {
                self.RowsStartMuaHang((_pageNumberMuaHang - 1) * _pageSize + 1);
                self.RowsEndMuaHang((_pageNumberMuaHang - 1) * _pageSize + self.ReportNCC_MuaHangChiTiet().length)
            }
            else {
                self.RowsStartMuaHang('0');
                self.RowsEndMuaHang('0');
            }
            self.SumNumberPageReportMuaHang(data.LstPageNumber);
            AllPageMuaHang = self.SumNumberPageReportMuaHang().length;
            self.selecPageMuaHang();
            self.ReserPageMuaHang();
            self.SumRowsHangHoaMuaHang(data.Rowcount);
            $("div[id ^= 'wait']").text("");
        });
    }

    self.SelectedPageNumberReportNCC_MuaHangChiTiet = function (item) {
        hidewait('table_h');
        ajaxHelper(ReportUri + "getListNCC_MuaHangChiTiet?ID_NCC=" + _id_DoiTuongMuaHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumberMuaHang + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.ReportNCC_MuaHangChiTiet(data.LstData);
            self.RowsStartMuaHang((_pageNumberMuaHang - 1) * _pageSize + 1);
            self.RowsEndMuaHang((_pageNumberMuaHang - 1) * _pageSize + self.ReportNCC_MuaHangChiTiet().length)
            $("div[id ^= 'wait']").text("");
        });
    }

    self.ChiTietHangHoa = function (item) {
        console.log(item.ID_NhanVien)
        ajaxHelper(ReportUri + "getListReportHangBanChiTiet_NhanVien?maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&laHangHoa=" + _laHangHoa + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_NhomHang=" + _ID_NhomHang + "&ID_ChiNhanh=" + _idDonViSeach + "&ID_NhanVien=" + item.ID_NhanVien, "GET").done(function (data) {
            self.ReportHangBanChiTiet_NhanVien(data.LstData);
        });
    }
    self.GetClass = function (page) {
        return (page.SoTrang === self.currentPage()) ? "click" : "";
    };
    self.GetClassNhapHang = function (page) {
        return (page.SoTrang === self.currentPageNhapHang()) ? "click" : "";
    };
    self.GetClassCongNo = function (page) {
        return (page.SoTrang === self.currentPageCongNo()) ? "click" : "";
    };
    self.GetClassMuaHang = function (page) {
        return (page.SoTrang === self.currentPageMuaHang()) ? "click" : "";
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
                self.SelectedPageNumberReportNCC_NhapHang();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportNCC_CongNo();
            else if (_kieubang == 3)
                self.SelectedPageNumberReportNCC_NhaCungCap();
        }
    };
    self.BackPage = function (item) {
        if (_pageNumber > 1) {
            _pageNumber = _pageNumber - 1;
            self.ReserPage();
            if (_kieubang == 1)
                self.SelectedPageNumberReportNCC_NhapHang();
            else if (_kieubang == 2)
                self.SelectedPageNumberReportNCC_CongNo();
            else if (_kieubang == 3)
                self.SelectedPageNumberReportNCC_NhaCungCap();
        }
    };
    self.EndPage = function (item) {
        _pageNumber = AllPage;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportNCC_NhapHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportNCC_CongNo();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportNCC_NhaCungCap();
    };
    self.StartPage = function (item) {
        _pageNumber = 1;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportNCC_NhapHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportNCC_CongNo();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportNCC_NhaCungCap();
    };
    self.gotoNextPage = function (item) {
        _pageNumber = item.SoTrang;
        self.ReserPage();
        if (_kieubang == 1)
            self.SelectedPageNumberReportNCC_NhapHang();
        else if (_kieubang == 2)
            self.SelectedPageNumberReportNCC_CongNo();
        else if (_kieubang == 3)
            self.SelectedPageNumberReportNCC_NhaCungCap();
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
            self.SelectedPageNumberReportNCC_NhapHangChiTiet();
        }
    };
    self.BackPageNhapHang = function (item) {
        if (_pageNumberNhapHang > 1) {
            _pageNumberNhapHang = _pageNumberNhapHang - 1;
            self.ReserPageNhapHang();
            self.SelectedPageNumberReportNCC_NhapHangChiTiet();
        }
    };
    self.EndPageNhapHang = function (item) {
        _pageNumberNhapHang = AllPageNhapHang;
        self.ReserPageNhapHang();
        self.SelectedPageNumberReportNCC_NhapHangChiTiet();
    };
    self.StartPageNhapHang = function (item) {
        _pageNumberNhapHang = 1;
        self.ReserPageNhapHang();
        self.SelectedPageNumberReportNCC_NhapHangChiTiet();
    };
    self.gotoNextPageNhapHang = function (item) {
        _pageNumberNhapHang = item.SoTrang;
        self.ReserPageNhapHang();
        self.SelectedPageNumberReportNCC_NhapHangChiTiet();
    }
    // phân trang công nợ chi tiết\
    self.selecPageCongNo = function () {
        // AllPageCongNo = self.SumNumberPageReport().length;

        if (AllPageCongNo > 4) {
            for (var i = 3; i < AllPageCongNo; i++) {
                self.SumNumberPageReportCongNo.pop(i + 1);
            }
            self.SumNumberPageReportCongNo.push({ SoTrang: 4 });
            self.SumNumberPageReportCongNo.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportCongNo.pop(i);
            }
            for (var j = 0; j < AllPageCongNo; j++) {
                self.SumNumberPageReportCongNo.push({ SoTrang: j + 1 });
            }
        }
        //$('#StartPageCongNo .fa-step-backward').hide();
        //$('#BackPageCongNo fa-caret-left').hide();
        //$('#NextPageCongNo .fa-caret-right').show();
        //$('#EndPageCongNo .fa-step-forward').show();
        $('#StartPageCongNo').hide();
        $('#BackPageCongNo').hide();
        $('#NextPageCongNo').show();
        $('#EndPageCongNo').show();
    }
    self.ReserPageCongNo = function (item) {
        //self.selecPage();
        if (_pageNumberCongNo > 1 && AllPageCongNo > 5/* && nextPage < AllPageCongNo - 1*/) {
            if (_pageNumberCongNo > 3 && _pageNumberCongNo < AllPageCongNo - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberCongNo) === parseInt(AllPageCongNo) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i - 3 });
                }
            }
            else if (_pageNumberCongNo == AllPageCongNo) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i - 4 });
                }
            }
            else if (_pageNumberCongNo < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberCongNo)
                    self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberCongNo == 1 && AllPageCongNo > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportCongNo.replace(self.SumNumberPageReportCongNo()[i], { SoTrang: parseInt(_pageNumberCongNo) + i });
            }
        }
        if (_pageNumberCongNo > 1) {
            $('#StartPageCongNo').show();
            $('#BackPageCongNo').show();
        }
        else {
            $('#StartPageCongNo').hide();
            $('#BackPageCongNo').hide();
        }
        if (_pageNumberCongNo == AllPageCongNo) {
            $('#NextPageCongNo').hide();
            $('#EndPageCongNo').hide();
        }
        else {
            $('#NextPageCongNo').show();
            $('#EndPageCongNo').show();
        }
        //if (_pageNumberCongNo > 1) {
        //    $('#StartPageCongNo .fa-step-backward').show();
        //    $('#BackPageCongNo .fa-caret-left').show();
        //}
        //else {
        //    $('#StartPageCongNo .fa-step-backward').hide();
        //    $('#BackPageCongNo .fa-caret-left').hide();
        //}
        //if (_pageNumberCongNo == AllPageCongNo) {
        //    $('#NextPageCongNo .fa-caret-right').hide();
        //    $('#EndPageCongNo .fa-step-forward').hide();
        //}
        //else {
        //    $('#NextPageCongNo .fa-caret-right').show();
        //    $('#EndPageCongNo .fa-step-forward').show();
        //}
        self.currentPageCongNo(parseInt(_pageNumberCongNo));
    }
    self.NextPageCongNo = function (item) {
        if (_pageNumberCongNo < AllPageCongNo) {
            _pageNumberCongNo = _pageNumberCongNo + 1;
            self.ReserPageCongNo();
            self.SelectedPageNumberReportNCC_CongNoChiTiet();
        }
    };
    self.BackPageCongNo = function (item) {
        if (_pageNumberCongNo > 1) {
            _pageNumberCongNo = _pageNumberCongNo - 1;
            self.ReserPageCongNo();
            self.SelectedPageNumberReportNCC_CongNoChiTiet();
        }
    };
    self.EndPageCongNo = function (item) {
        _pageNumberCongNo = AllPageCongNo;
        self.ReserPageCongNo();
        self.SelectedPageNumberReportNCC_CongNoChiTiet();
    };
    self.StartPageCongNo = function (item) {
        _pageNumberCongNo = 1;
        self.ReserPageCongNo();
        self.SelectedPageNumberReportNCC_CongNoChiTiet();
    };
    self.gotoNextPageCongNo = function (item) {
        _pageNumberCongNo = item.SoTrang;
        self.ReserPageCongNo();
        self.SelectedPageNumberReportNCC_CongNoChiTiet();
    }
    // phân trang Mua hàng chi tiết\
    self.selecPageMuaHang = function () {
        // AllPageMuaHang = self.SumNumberPageReport().length;

        if (AllPageMuaHang > 4) {
            for (var i = 3; i < AllPageMuaHang; i++) {
                self.SumNumberPageReportMuaHang.pop(i + 1);
            }
            self.SumNumberPageReportMuaHang.push({ SoTrang: 4 });
            self.SumNumberPageReportMuaHang.push({ SoTrang: 5 });
        }
        else {
            for (var i = 0; i < 6; i++) {
                self.SumNumberPageReportMuaHang.pop(i);
            }
            for (var j = 0; j < AllPageMuaHang; j++) {
                self.SumNumberPageReportMuaHang.push({ SoTrang: j + 1 });
            }
        }
        //$('#StartPageMuaHang .fa-step-backward').hide();
        //$('#BackPageMuaHang fa-caret-left').hide();
        //$('#NextPageMuaHang .fa-caret-right').show();
        //$('#EndPageMuaHang .fa-step-forward').show();
        $('#StartPageMuaHang').hide();
        $('#BackPageMuaHang').hide();
        $('#NextPageMuaHang').show();
        $('#EndPageMuaHang').show();
    }
    self.ReserPageMuaHang = function (item) {
        //self.selecPage();
        if (_pageNumberMuaHang > 1 && AllPageMuaHang > 5/* && nextPage < AllPageMuaHang - 1*/) {
            if (_pageNumberMuaHang > 3 && _pageNumberMuaHang < AllPageMuaHang - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i - 2 });
                }
            }
            else if (parseInt(_pageNumberMuaHang) === parseInt(AllPageMuaHang) - 1) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i - 3 });
                }
            }
            else if (_pageNumberMuaHang == AllPageMuaHang) {
                for (var i = 0; i < 5; i++) {
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i - 4 });
                }
            }
            else if (_pageNumberMuaHang < 4) {
                for (var i = 0; i < 5; i++) {
                    //console.log(_pageNumberMuaHang)
                    self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: 1 + i });
                }
            }
        }
        else if (_pageNumberMuaHang == 1 && AllPageMuaHang > 5) {
            for (var i = 0; i < 5; i++) {
                self.SumNumberPageReportMuaHang.replace(self.SumNumberPageReportMuaHang()[i], { SoTrang: parseInt(_pageNumberMuaHang) + i });
            }
        }
        if (_pageNumberMuaHang > 1) {
            $('#StartPageMuaHang').show();
            $('#BackPageMuaHang').show();
        }
        else {
            $('#StartPageMuaHang').hide();
            $('#BackPageMuaHang').hide();
        }
        if (_pageNumberMuaHang == AllPageMuaHang) {
            $('#NextPageMuaHang').hide();
            $('#EndPageMuaHang').hide();
        }
        else {
            $('#NextPageMuaHang').show();
            $('#EndPageMuaHang').show();
        }
        //if (_pageNumberMuaHang > 1) {
        //    $('#StartPageMuaHang .fa-step-backward').show();
        //    $('#BackPageMuaHang .fa-caret-left').show();
        //}
        //else {
        //    $('#StartPageMuaHang .fa-step-backward').hide();
        //    $('#BackPageMuaHang .fa-caret-left').hide();
        //}
        //if (_pageNumberMuaHang == AllPageMuaHang) {
        //    $('#NextPageMuaHang .fa-caret-right').hide();
        //    $('#EndPageMuaHang .fa-step-forward').hide();
        //}
        //else {
        //    $('#NextPageMuaHang .fa-caret-right').show();
        //    $('#EndPageMuaHang .fa-step-forward').show();
        //}
        self.currentPageMuaHang(parseInt(_pageNumberMuaHang));
    }
    self.NextPageMuaHang = function (item) {
        if (_pageNumberMuaHang < AllPageMuaHang) {
            _pageNumberMuaHang = _pageNumberMuaHang + 1;
            self.ReserPageMuaHang();
            self.SelectedPageNumberReportNCC_MuaHangChiTiet();
        }
    };
    self.BackPageMuaHang = function (item) {
        if (_pageNumberMuaHang > 1) {
            _pageNumberMuaHang = _pageNumberMuaHang - 1;
            self.ReserPageMuaHang();
            self.SelectedPageNumberReportNCC_MuaHangChiTiet();
        }
    };
    self.EndPageMuaHang = function (item) {
        _pageNumberMuaHang = AllPageMuaHang;
        self.ReserPageMuaHang();
        self.SelectedPageNumberReportNCC_MuaHangChiTiet();
    };
    self.StartPageMuaHang = function (item) {
        _pageNumberMuaHang = 1;
        self.ReserPageMuaHang();
        self.SelectedPageNumberReportNCC_MuaHangChiTiet();
    };
    self.gotoNextPageMuaHang = function (item) {
        _pageNumberMuaHang = item.SoTrang;
        self.ReserPageMuaHang();
        self.SelectedPageNumberReportNCC_MuaHangChiTiet();
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
        console.log(self.searchNguoiban())
        console.log($('#NoteNameNguoiBan').val())
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
            console.log(self.NguoiBans())
        }

    }
    self.valueNoHienTaiFrom = function () {
        _nohientaiFrom = $("#valueNoHienTaiFrom").val();
        _nohientaiFrom = formatNumberToFloat(_nohientaiFrom)
        if ($("#valueNoHienTaiFrom").val() == "") {
            _nohientaiFrom = 0;
        }
        console.log(_nohientaiFrom);
    }
    $('#valueNoHienTaiFrom').keypress(function (e) {
        if (e.keyCode == 13) {
            if (_nohientaiTo != 0 & _nohientaiTo < _nohientaiFrom)
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Giá trị 'tới' không được nhỏ hơn giá trị 'từ'!", "danger");
            else {
                if (self.check_kieubang() == 1)
                    self.getListNCC_CongNo_BieuDo();
                else
                    self.getListReportNCC_CongNo();
            }
        }
    });
    $('#valueNoHienTaiTo').keypress(function (e) {
        if (e.keyCode == 13) {
            if (_nohientaiTo < _nohientaiFrom)
                bottomrightnotify('<i class="fa fa-check" aria-hidden="true"></i>' + "Giá trị 'tới' không được nhỏ hơn giá trị 'từ'!", "danger");
            else {
                if (self.check_kieubang() == 1)
                    self.getListNCC_CongNo_BieuDo();
                else
                    self.getListReportNCC_CongNo();
            }
        }
    });
    self.valueNoHienTaiTo = function () {
        _nohientaiTo = $("#valueNoHienTaiTo").val();
        _nohientaiTo = formatNumberToFloat(_nohientaiTo);
        if ($("#valueNoHienTaiTo").val() == "") {
            _nohientaiTo = 0;
        }
        console.log(_nohientaiTo);

    }
    $('#NoteNameNguoiBan').keypress(function (e) {
        if (e.keyCode == 13 && self.NguoiBans().length > 0) {
            self.SelectedNguoiBan(self.NguoiBans()[0]);
        }
    });
    //Xuất file excel
    $('#nccma').click(function () {
        $(".nccma").toggle();
        self.addColum(1, $(this).val());
        addClass(".nccma", "nccma", $(this).val(), "nhaphangNCC_Trung");
    });
    $('#nccname').click(function () {
        $(".nccname").toggle();
        self.addColum(1, $(this).val());
        addClass(".nccname", "nccname", $(this).val(), "nhaphangNCC_Trung");
    });
    $('#nccgiatrinhap ').click(function () {
        $(".nccgiatrinhap ").toggle();
        self.addColum(1, $(this).val());
        addClass(".nccgiatrinhap", "nccgiatrinhap", $(this).val(), "nhaphangNCC_Trung");
    });
    $('#nccgiatritra ').click(function () {
        $(".nccgiatritra ").toggle();
        self.addColum(1, $(this).val());
        addClass(".nccgiatritra", "nccgiatritra", $(this).val(), "nhaphangNCC_Trung");
    });
    $('#nccgiatrithuan ').click(function () {
        $(".nccgiatrithuan ").toggle();
        self.addColum(1, $(this).val());
        addClass(".nccgiatrithuan", "nccgiatrithuan", $(this).val(), "nhaphangNCC_Trung");
    });

    $('#cnma').click(function () {
        $(".cnma").toggle();
        self.addColum(2, $(this).val());
        addClass(".cnma", "cnma", $(this).val(), "congnoNCC");
    });
    $('#cndauky ').click(function () {
        $(".cndauky ").toggle();
        self.addColum(2, $(this).val());
        addClass(".cndauky", "cndauky", $(this).val(), "congnoNCC");
    });
    $('#cnghino  ').click(function () {
        $(".cnghino  ").toggle();
        self.addColum(2, $(this).val());
        addClass(".cnghino", "cnghino", $(this).val(), "congnoNCC");
    });
    $('#cnghichu  ').click(function () {
        $(".cnghichu  ").toggle();
        self.addColum(2, $(this).val());
        addClass(".cnghichu", "cnghichu", $(this).val(), "congnoNCC");
    });
    $('#cncuoiky  ').click(function () {
        $(".cncuoiky  ").toggle();
        self.addColum(2, $(this).val());
        addClass(".cncuoiky", "cncuoiky", $(this).val(), "congnoNCC");
    });
    $('#cnnamet  ').click(function () {
        $(".cnnamet  ").toggle();
        self.addColum(2, $(this).val());
        addClass(".cnnamet", "cnnamet", $(this).val(), "congnoNCC");
    });

    $('#tnccma ').click(function () {
        $(".tnccma ").toggle();
        self.addColum(3, $(this).val());
        addClass(".tnccma", "tnccma", $(this).val(), "hangnhapNCC");
    });
    $('#tnccname   ').click(function () {
        $(".tnccname   ").toggle();
        self.addColum(3, $(this).val());
        addClass(".tnccname", "tnccname", $(this).val(), "hangnhapNCC");
    });
    $('#tnccsoluong   ').click(function () {
        $(".tnccsoluong   ").toggle();
        self.addColum(3, $(this).val());
        addClass(".tnccsoluong", "tnccsoluong", $(this).val(), "hangnhapNCC");
    });
    $('#tnccgiatri   ').click(function () {
        $(".tnccgiatri   ").toggle();
        self.addColum(3, $(this).val());
        addClass(".tnccgiatri", "tnccgiatri", $(this).val(), "hangnhapNCC");
    });
    self.addColum = function (values, item) {
        if (values == 1) {
            if (self.ColumnsExcelNHCC().length < 1) {
                self.ColumnsExcelNHCC.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelNHCC().length; i++) {
                    if (self.ColumnsExcelNHCC()[i] === item) {
                        self.ColumnsExcelNHCC.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelNHCC().length - 1) {
                        self.ColumnsExcelNHCC.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelNHCC.sort();
        }
        else if (values == 2) {
            if (self.ColumnsExcelCNCC().length < 1) {
                self.ColumnsExcelCNCC.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelCNCC().length; i++) {
                    if (self.ColumnsExcelCNCC()[i] === item) {
                        self.ColumnsExcelCNCC.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelCNCC().length - 1) {
                        self.ColumnsExcelCNCC.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelCNCC.sort();
        }
        else {
            if (self.ColumnsExcelHNCC().length < 1) {
                self.ColumnsExcelHNCC.push(item);
            }
            else {
                for (var i = 0; i < self.ColumnsExcelHNCC().length; i++) {
                    if (self.ColumnsExcelHNCC()[i] === item) {
                        self.ColumnsExcelHNCC.splice(i, 1);
                        break;
                    }
                    if (i == self.ColumnsExcelHNCC().length - 1) {
                        self.ColumnsExcelHNCC.push(item);
                        break;
                    }
                }
            }
            self.ColumnsExcelHNCC.sort();
        }
    }
    //===============================
    // Load lai form lưu cache bộ lọc 
    // trên grid 
    //===============================
    function LoadHtmlGrid(cacheExcel, vals, caches) {
        //debugger;
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
                    cacheExcelNHCC = false;
                else if (vals === 2)
                    cacheExcelCNCC = false;
                else
                    cacheExcelHNCC = false;
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
            ChucNang: "Báo cáo nhà cung cấp",
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
                if (_kieubang == 1 && self.ReportNCC_NhapHang().length != 0) {
                    if (self.BCNCC_NhapHang() == "BCNCC_NhapHang" && self.BCNCC_NhapHang_XuatFile() == "BCNCC_NhapHang_XuatFile") {
                        for (var i = 0; i < self.ColumnsExcelNHCC().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelNHCC()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelNHCC()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelNCC_NhapHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maHH=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 2 && self.ReportNCC_CongNo().length != 0) {
                    if (self.BCNCC_CongNo() == "BCNCC_CongNo" && self.BCNCC_CongNo_XuatFile() == "BCNCC_CongNo_XuatFile") {
                        for (var i = 0; i < self.ColumnsExcelCNCC().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelCNCC()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelCNCC()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelNCC_CongNo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _id_DonVi + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                        window.location.href = url;
                    }
                    else
                        bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Bạn không có quyền xuất file báo cáo này!", "danger");
                }
                else if (_kieubang == 3 && self.ReportNCC_NhaCungCap().length != 0) {
                    if (self.BCNCC_HangNhap() == "BCNCC_HangNhap" && self.BCNCC_HangNhap_XuatFile() == "BCNCC_HangNhap_XuatFile") {
                        for (var i = 0; i < self.ColumnsExcelHNCC().length; i++) {
                            if (i == 0) {
                                columnHide = self.ColumnsExcelHNCC()[i];
                            }
                            else {
                                columnHide = self.ColumnsExcelHNCC()[i] + "_" + columnHide;
                            }
                        }
                        var url = ReportUri + "ExportExcelNCC_MuaHang?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
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
    self.ExportExcel_ChiTiet = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo nhà cung cấp",
            NoiDung: "Xuất báo cáo chi tiết nhập hàng theo nhà cung cấp",
            NoiDungChiTiet: "Xuất báo cáo chi tiết nhập hàng theo nhà cung cấp",
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

            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {
                var columnHide = null;
                var url = ReportUri + "ExportExcelNCC_NhapHangChiTiet?ID_NCC=" + item.ID_NCC + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
            }
        })

    }
    self.ExportExcel_CongNoChiTiet = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo nhà cung cấp",
            NoiDung: "Xuất báo cáo chi tiết công nợ theo nhà cung cấp",
            NoiDungChiTiet: "Xuất báo cáo chi tiết công nợ theo nhà cung cấp",
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

            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {
                var columnHide = null;
                var url = ReportUri + "ExportExcelNCC_CongNoChiTiet?ID_NCC=" + item.ID_KhachHang + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _id_DonVi + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
            }
        })

    }
    self.ExportExcel_HangNhapChiTiet = function (item) {
        var objDiary = {
            ID_NhanVien: _id_NhanVien,
            ID_DonVi: _id_DonVi,
            ChucNang: "Báo cáo nhà cung cấp",
            NoiDung: "Xuất báo cáo chi tiết hàng nhập theo nhà cung cấp",
            NoiDungChiTiet: "Xuất báo cáo chi tiết hàng nhập theo nhà cung cấp",
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

            },
            statusCode: {
                404: function () {
                },
            },
            error: function (jqXHR, textStatus, errorThrown) {
                bottomrightnotify('<i class="fa fa-exclamation-triangle" aria-hidden="true"></i> ' + "Ghi nhật ký sử dụng thất bại!", "danger");
            },
            complete: function () {
                var columnHide = null;
                var url = ReportUri + "ExportExcelNCC_MuaHangChiTiet?ID_NCC=" + item.ID_NCC + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&columnsHide=" + columnHide + "&ID_ChiNhanh=" + _idDonViSeach + "&time=" + self.TodayBC() + "&ChiNhanh=" + self.TenChiNhanh();
                window.location.href = url;
            }
        })

    }
    function formatNumberObj(obj) {
        var objVal = $(obj).val();
        $(obj).val(objVal.toString().replace(/,/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ","));
        return objVal;
    }
    //trinhpv Lọc nhóm nhà cung cấp
    self.NhomDoiTuongs = ko.observableArray();
    var _tennhomDT = null;
    var _tenNhomDoiTuongSeach = null;
    self.MangNhomDoiTuong = ko.observableArray();
    self.searchNhomDoiTuong = ko.observableArray();
    function getList_NhomDoiTuongs() {
        ajaxHelper(ReportUri + "GetListID_NhomDoiTuong?TenNhomDoiTuong=" + _tennhomDT + "&loaidoituong=2", "GET").done(function (data) {
            self.NhomDoiTuongs(data);
            self.searchNhomDoiTuong(data);
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
            _tenNhomDoiTuongSeach = null;
        }
        // remove check
        $('#selec-all-NhomDoiTuong li').each(function () {
            if ($(this).attr('id') === item.ID) {
                $(this).find('i').remove();
            }
        });
        console.log(self.MangNhomDoiTuong())
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
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
        console.log(self.MangNhomDoiTuong());
        _pageNumber = 1;
        if (_kieubang == 1) {
            if (self.check_kieubang() == 1)
                self.getListNCC_NhapHang_BieuDo();
            else
                self.getListNCC_NhapHang();
        }
        else if (_kieubang == 2) {
            if (self.check_kieubang() == 1)
                self.getListNCC_CongNo_BieuDo();
            else
                self.getListReportNCC_CongNo();
        }
        else if (_kieubang == 3)
            self.getListReportNCC_NhaCungCap();
    }
    //lọc người bán
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
            // console.log(self.NhomDoiTuongs())
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
    self.getListNCC_NhapHang();
    //trinhpv load bieudo
    self.arrHH = ko.observableArray();
    self.arrDT = ko.observableArray();
    self.DoanhThuTT = ko.observableArray();
    self.DonViTinh = ko.observable("Đơn vị tính: hàng đơn vị");
    var _dataDS;
    var _data;
    var nameChar;
    var style;
    self.getListNCC_NhapHang_BieuDo = function () {
        ajaxHelper(ReportUri + "getListNCC_NhapHang_BieuDo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&ID_ChiNhanh=" + _idDonViSeach, "GET").done(function (data) {
            self.DoanhThuTT(data);
            self.arrDT([]);
            self.arrHH([]);
            $('.text-unit').css("bottom", "15px");
            if (self.DoanhThuTT().length != 0) {
                var _MauSoDVT = 1;
                var _loadHangHoa = "'";
                //if (data.length > 0)
                //    var dvt = self.DoanhThuTT()[0].Rowsn;
                //if (dvt >= 1000000000) {
                //    _MauSoDVT = 1000000000
                //    self.DonViTinh("Đơn vị tính: hàng tỷ")
                //}
                //if (dvt >= 1000000 & dvt < 1000000000) {
                //    _MauSoDVT = 1000000
                //    self.DonViTinh("Đơn vị tính: hàng triệu")
                //}
                //if (dvt >= 1000 & dvt < 1000000) {
                //    _MauSoDVT = 1000
                //    self.DonViTinh("Đơn vị tính: hàng nghìn")
                //}
                for (var i = 0; i < self.DoanhThuTT().length; i++) {
                    _loadHangHoa = self.DoanhThuTT()[i].Columnss;
                    //_data = parseFloat(self.DoanhThuTT()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                    self.arrDT.push(self.DoanhThuTT()[i].Rowsn);
                    self.arrHH.push(_loadHangHoa);
                }
                style = 'Top 10 nhà cung cấp: Nhập hàng nhiều nhất (đã trừ trả hàng)';
                nameChar = "Doanh thu thuần";
            }
            else {
                style = "Báo cáo không có dữ liệu.";
                self.DonViTinh([]);
            }
            self.loadBieuDo();
        });
    }

    self.getListNCC_CongNo_BieuDo = function () {
        ajaxHelper(ReportUri + "getListNCC_CongNo_BieuDo?ID_NhomDoiTuongs=" + _tenNhomDoiTuongSeach + "&maNCC=" + _maHH + "&NoHienTaiFrom=" + _nohientaiFrom + "&NoHienTaiTo=" + _nohientaiTo + "&timeStart=" + _timeStart + "&timeEnd=" + _timeEnd + "&pageNumber=" + _pageNumber + "&pageSize=" + _pageSize + "&ID_ChiNhanh=" + _id_DonVi, "GET").done(function (data) {
            self.DoanhThuTT(data);
            self.arrDT([]);
            self.arrHH([]);

            if (self.DoanhThuTT().length != 0) {
                var _MauSoDVT = 1;
                var _loadHangHoa = "'";
                //if (data.length > 0)
                //    var dvt = self.DoanhThuTT()[0].Rowsn;
                //if (dvt >= 1000000000) {
                //    _MauSoDVT = 1000000000
                //    self.DonViTinh("Đơn vị tính: hàng tỷ")
                //}
                //if (dvt >= 1000000 & dvt < 1000000000) {
                //    _MauSoDVT = 1000000
                //    self.DonViTinh("Đơn vị tính: hàng triệu")
                //}
                //if (dvt >= 1000 & dvt < 1000000) {
                //    _MauSoDVT = 1000
                //    self.DonViTinh("Đơn vị tính: hàng nghìn")
                //}
                for (var i = 0; i < self.DoanhThuTT().length; i++) {
                    _loadHangHoa = self.DoanhThuTT()[i].Columnss;
                    // _data = parseFloat(self.DoanhThuTT()[i].Rowsn / _MauSoDVT).toFixed(3) * 1;
                    self.arrDT.push(self.DoanhThuTT()[i].Rowsn);
                    self.arrHH.push(_loadHangHoa);
                }
                style = 'Top 10 nhà cung cấp có công nợ nhiều nhất';
                nameChar = "Nợ cuối kỳ";
            }
            else {
                style = "Báo cáo không có dữ liệu.";
                self.DonViTinh([]);
            }
            self.loadBieuDo();
        });
    }
    //trinhpv load bieudo
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
                text: style,
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
            // hiển thị giá trị lên đầu cột
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: false
                    }
                }
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